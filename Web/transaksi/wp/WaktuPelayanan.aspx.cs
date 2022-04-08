using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_wp_WaktuPelayanan : Scms.Web.Core.PageHandler
{
  private const string TIPE_PPIC = "01";
  private const string TIPE_DC = "02";
  private const string TIPE_PENGIRIMAN = "03";

  private string TIPE_DAYS = null;

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      hfGdg.Value = this.ActiveGudang;
      hfGdgDesc.Value = this.ActiveGudangDescription;

      Functional.SetComboData(cbGudang3, "c_gdg", (string.IsNullOrEmpty(this.ActiveGudangDescription) ? string.Empty : this.ActiveGudangDescription), (string.IsNullOrEmpty(this.ActiveGudang) ? string.Empty : this.ActiveGudang));
      cbGudang3.Disabled = true;

      Functional.SetComboData(cbGudang2, "c_gdg", (string.IsNullOrEmpty(this.ActiveGudangDescription) ? string.Empty : this.ActiveGudangDescription), (string.IsNullOrEmpty(this.ActiveGudang) ? string.Empty : this.ActiveGudang));
      cbGudang2.Disabled = true;

      Functional.SetComboData(cbGudang, "c_gdg", (string.IsNullOrEmpty(this.ActiveGudangDescription) ? string.Empty : this.ActiveGudangDescription), (string.IsNullOrEmpty(this.ActiveGudang) ? string.Empty : this.ActiveGudang));
      cbGudang.Disabled = true;

      this.storeGridWP.DataBind();
      RowSelectionModel sm = this.gridMain.SelectionModel.Primary as RowSelectionModel;
      sm.SelectedRows.Add(new SelectedRow(1));

      hfDate.Value = DateTime.Now.AddMonths(-3).AddDays(+1).ToString("dd-MM-yyyy");
      hfDateY.Value = DateTime.Now.AddMonths(-3).ToString("dd-MM-yyyy");
      hfDateM.Value = DateTime.Now.AddMonths(-3).AddDays(-1).ToString("dd-MM-yyyy");
    }
  }

  private void ClearEntrys()
  {
    Ext.Net.Store cbDOHdrstr = cbDODtl.GetStore();
    if (cbDOHdrstr != null)
    {
      cbDOHdrstr.RemoveAll();
    }
    cbDODtl.Clear();
  }

  private PostDataParser.StructureResponse SaveParser(Dictionary<string, string>[] listNum, string sGudang, string tipe, bool isAdd)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);


    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;
    Dictionary<string, string> dicData = null;


    string varData = null,
      tmp = null,
      noTrans = null;

    DateTime tanggal = DateTime.MinValue;

    pair.IsSet = true;
    pair.IsList = true;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);

    pair.DicAttributeValues.Add("Gudang", sGudang);
    pair.DicAttributeValues.Add("Tipe", hfType.Text);


    bool isNew = false,
      isVoid = false,
      isModify = false;

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    if ((listNum.Length > 0) || !string.IsNullOrEmpty(sGudang) || !string.IsNullOrEmpty(hfGdg.Text))
    {

      for (int nLoop = 0, nLen = listNum.Length; nLoop < nLen; nLoop++)
      {
        tmp = nLoop.ToString();

        dicData = listNum[nLoop];

        isNew = dicData.GetValueParser<bool>("l_new");

        if (isNew)
        {
          if (!pag.IsAllowAdd)
          {
            continue;
          }
          dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

          dicAttr.Add("New", isNew.ToString().ToLower());

          noTrans = dicData.GetValueParser<string>("c_notrans");

          if ((!string.IsNullOrEmpty(noTrans)))
          {
            dicAttr.Add("TransNo", noTrans);

            pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
            {
              IsSet = true,
              DicAttributeValues = dicAttr
            });
          }
        }
        else
        {
          if (!pag.IsAllowDelete)
          {
            continue;
          }
          dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

          dicAttr.Add("Delete", false.ToString());

          noTrans = dicData.GetValueParser<string>("c_notrans");

          if ((!string.IsNullOrEmpty(noTrans)))
          {
            dicAttr.Add("TransNo", noTrans);

            pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
            {
              IsSet = true,
              DicAttributeValues = dicAttr
            });
          }
          
        }
      }
    }
    try
    {
      varData = parser.ParserData("WaktuPelayanan",(isAdd ? "Add" : "Delete"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_wp_WaktuPelayanan SaveParser : {0} ", ex.Message);
    }
    string result = null;

    if (!string.IsNullOrEmpty(varData))
    {
      Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

      result = soa.PostData(varData);

      responseResult = parser.ResponseParser(result);
    }

    return responseResult;
  }

  private PostDataParser.StructureResponse DeleteParser(string wpNumber, string gdg, string ket)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = wpNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", this.Nip);
    pair.DicAttributeValues.Add("TransNo", wpNumber);
    pair.DicAttributeValues.Add("Gudang", gdg);
    pair.DicAttributeValues.Add("Keterangan", ket.Trim());

    try
    {
      varData = parser.ParserData("WaktuPelayanan", "Delete", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_wp_WaktuPelayanan DeleteParser : {0} ", ex.Message);
    }

    string result = null;

    if (!string.IsNullOrEmpty(varData))
    {
      Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

      result = soa.PostData(varData);

      responseResult = parser.ResponseParser(result);
    }

    return responseResult;
  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string Gudang = (e.ExtraParams["Gudang"] ?? string.Empty);
    string GudangDesc = (e.ExtraParams["GudangDesc"] ?? string.Empty);
    string NoTrans = (e.ExtraParams["NoTrans"] ?? string.Empty);

    if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    //PostDataParser.StructureResponse respon = SaveParser(NoTrans, Gudang, hfType.Text);

//    if (respon.IsSet)
//    {
//      if (respon.Response == PostDataParser.ResponseStatus.Success)
//      {
//        string scrpt = null;

//        string dateJs = null;
//        DateTime date = DateTime.Today;

//        if (respon.Values != null)
//        {
//          if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd HH:mm:ss", out date))
//          {
//            dateJs = Functional.DateToJson(date);
//          }

//          if (!string.IsNullOrEmpty(storeGridWP.ClientID))
//          {
//            scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
//                'v_gdgdesc': '{2}',
//                'c_notrans': '{1}',
//                'd_entry': {3}
//              }}));{0}.commitChanges();", storeGridWP.ClientID,
//                                      respon.Values.GetValueParser<string>("WP", string.Empty),
//                      this.ActiveGudangDescription, dateJs);

//            X.AddScript(scrpt);
//          }
//        }

//        this.ClearEntrys();

//        Functional.ShowMsgInformation("Data berhasil tersimpan.");
//      }
//      else
//      {
//        e.ErrorMessage = respon.Message;

//        e.Success = false;
//      }
//    }
//    else
//    {
//      e.ErrorMessage = "Unknown response";

//      e.Success = false;
//    }

  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SubmitSelection(object sender, DirectEventArgs e)
  {
    string json = e.ExtraParams["Values"];
    string tipeBtn = e.ExtraParams["BtnTipe"];
    string katcol = e.ExtraParams["katcol"];

    Dictionary<string, string>[] lstNTrans = JSON.Deserialize<Dictionary<string, string>[]>(json);

    System.Text.StringBuilder sb = new System.Text.StringBuilder();

    bool addHeader = true;
    int i = 0;

    TIPE_DAYS = katcol;

    //foreach (Dictionary<string, string> row in companies)
    //{
      
    //  foreach (KeyValuePair<string, string> keyValuePair in row)
    //  {
    //    sb.Append(keyValuePair.Value + " ");
    //  }

    //  i++;
    //}

    bool isAdd = (tipeBtn == "1" ? true : false);
    

    if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    WaktuPelayananCtrl.Save_btn(lstNTrans, hfGdg.Text, hfType.Text, isAdd, GridPanel3.GetStore().ClientID, GridPanel2.ClientID);

    

    //PostDataParser.StructureResponse respon = SaveParser(lstNTrans, hfGdg.Text, hfType.Text, isAdd);

    string scrpt = null,
      scrpt1 = null;

    //if (respon.IsSet)
    //{
    //   scrpt = string.Format(@"{0}.removeAll(); {0}.reload();", GridPanel3.GetStore().ClientID);

    // X.AddScript(scrpt);

    // if (TIPE_DAYS == "3")
    // {
    //   scrpt1 = string.Format(@"{0}.removeAll(); {0}.reload();", gridMain.ClientID);
    // }
    // else if (TIPE_DAYS == "2")
    // {
    //    scrpt1 = string.Format(@"{0}.removeAll(); {0}.reload();", GridPanel1.GetStore().ClientID); 
       
    //     #region Old Coded
    //     //sb.AppendFormat("{0}.removeAll(); {0}.reload();", GridPanel1.GetStore().ClientID);
    //     //X.AddScript(
    //     // string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r);{0}.commitChanges(); }}",
    //     // store2.ClientID, "PL1212E730"));
    //     //string scrpt1 = "var store = {0}; if (!Ext.isEmpty(store)) {store.clearFilter();store.removeAll();store.reload();};";

    //     //X.Js.Call("onGridRowUserClick", GridPanel1.ClientID);

    //     //btnAdd.Listeners.Click.Handler = string.Format(@"{0}.reload();", GridPanel1.GetStore().ClientID);
    //     //btnAdd.AddEvents(string.Format(@"{0}.reload();", GridPanel1.GetStore().ClientID));
    //     //btnAdd.Click += new EventHandler(btnAdd_Click);

    //     #endregion
    // }
    // else
    // {
    //   scrpt1 = string.Format(@"{0}.removeAll(); {0}.reload();", GridPanel2.ClientID);
    // }

    // X.AddScript(scrpt1);

    //}
    //else
    //{
    //  e.ErrorMessage = "Unknown response";

    //  e.Success = false;
    //}
  }

  void btnAdd_Click(object sender, EventArgs e)
  {
    throw new NotImplementedException();
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  public void DeleteMethod(string NoTrans, string gdg, string keterangan)
  {
    if (string.IsNullOrEmpty(NoTrans))
    {
      Functional.ShowMsgWarning("Nomor WP tidak terbaca.");

      return;
    }
    if (string.IsNullOrEmpty(keterangan))
    {
      Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");

      return;
    }
    PostDataParser.StructureResponse respon = DeleteParser(NoTrans, gdg, keterangan);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r);{0}.commitChanges(); }}",
        storeGridWP.ClientID, NoTrans));

      Functional.ShowMsgInformation(string.Format("Nomor Transaksi '{0}' telah terhapus.", NoTrans));
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
    }
  }

  protected void Page_Init(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

      if (qryString.Equals("ppic", StringComparison.OrdinalIgnoreCase))
      {
        hfType.Text = TIPE_PPIC;
      }
      else if (qryString.Equals("dc", StringComparison.OrdinalIgnoreCase))
      {
        hfType.Text = TIPE_DC;
      }
      else
      {
        hfType.Text = TIPE_PENGIRIMAN;
      }
    }
  }
}

