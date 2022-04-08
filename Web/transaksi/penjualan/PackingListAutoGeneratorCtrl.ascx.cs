using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using System.Xml;

public partial class transaksi_penjualan_PackingListAutoGeneratorCtrl : System.Web.UI.UserControl
{
  protected void Page_Load(object sender, EventArgs e)
  {

  }

  private void ClearEntrys()
  {
    wnDetail.Title = "Packing List";

    hfPlNoAutoGen.Clear();

    cbCustomerAutoGenHdr.Clear();
    cbCustomerAutoGenHdr.ClearValue();
    cbCustomerAutoGenHdr.Disabled = false;

    cbPrincipalAutoGenHdr.Clear();
    cbPrincipalAutoGenHdr.ClearValue();
    cbPrincipalAutoGenHdr.Disabled = false;

    cbTipeAutoGenHdr.Items.Add(new Ext.Net.ListItem("Regular", "01"));
    cbTipeAutoGenHdr.Items.Add(new Ext.Net.ListItem("Regular", "01"));

    txKeteranganAutoGen.Clear();
    txKeteranganAutoGen.Disabled = false;

    btnSave.Hidden = false;
    btnDelete.Hidden = false;

    //Indra 20181226FM Penambahan filter ETD
    txPeriode1.Text = DateTime.Now.AddMonths(-1).AddDays(-DateTime.Now.Day + 1).ToString("dd-MM-yyyy");
    txPeriode2.Text = DateTime.Now.ToString("dd-MM-yyyy");
    txPeriode1.Disabled = txPeriode2.Disabled = false;

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  //Indra 20181226FM Penambahan filter ETD
  //private void PopulateAutoGenerate(string gdg, string cabang, string prinsipal, string categori)
  private void PopulateAutoGenerate(string gdg, string cabang, string prinsipal, string categori, string txPeriode1, string txPeriode2)
  {
    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    List<Dictionary<string, string>> lstResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "gdg", gdg, "System.Char"},
        new string[] { "cusno", cabang,  "System.String"},
        new string[] { "supl", prinsipal,  "System.String"},
        new string[] { "isbox", "false",  "System.String"},
        new string[] { "cat", categori,  "System.String"},
        //Indra 20181226FM Penambahan filter ETD
        new string[] { "txPeriode1", txPeriode1, "System.String"},
        new string[] { "txPeriode2", txPeriode2, "System.String"},
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    int MaxLimit = string.IsNullOrEmpty(txMax.Text) ? -1 :  int.Parse(txMax.Text);

    string res = soa.GlobalQueryService(0, MaxLimit, false, string.Empty, string.Empty, "3204", paramX);
    System.Text.StringBuilder sb = new System.Text.StringBuilder();

    decimal nSisa = 0;

    sb.AppendFormat("{0}.removeAll(); {0}.commitChanges(); ", gridDetail.GetStore().ClientID);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        lstResultInfo = JSON.Deserialize<List<Dictionary<string, string>>>(jarr.First.ToString());

        for (int nLoop = 0; nLoop < lstResultInfo.Count; nLoop++)
        {
          dicResultInfo = lstResultInfo[nLoop];

          nSisa = dicResultInfo.GetValueParser<decimal>("n_sisa", 0);

          DateTime date = DateTime.MinValue;
          DateTime dateSP = DateTime.MinValue;
          DateTime dateETD = DateTime.MinValue; //Indra 20181115FM ETD First

          date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_expired"));
          dateSP = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_spdate"));
          dateETD = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_etdsp")); //Indra 20181115FM ETD First

          if (nSisa > 0)
          {
            sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
            'c_iteno': '{1}',
            'v_itemdesc': '{2}',
            'c_sp': '{3}',
            'c_spc': '{4}',
            'c_batch': '{5}',
            'n_booked': {6},
            'n_QtyRequest': {6},
            'v_undes': '{7}',
            'd_expired' : '{8}',
            'l_expired' : {9},
            'd_spdate' : '{10}',
            'd_etdsp' : '{11}', 
            'l_new': true
          }})); ", gridDetail.GetStore().ClientID,
                    dicResultInfo.GetValueParser<string>("c_iteno", string.Empty),
                    dicResultInfo.GetValueParser<string>("v_itemdesc", string.Empty),
                    dicResultInfo.GetValueParser<string>("c_spno", string.Empty),
                    dicResultInfo.GetValueParser<string>("c_sp", string.Empty),
                    dicResultInfo.GetValueParser<string>("c_batch", string.Empty),
                    nSisa,
                    dicResultInfo.GetValueParser<string>("v_undes", string.Empty), 
                    date,
                    dicResultInfo.GetValueParser<bool>("l_expired", false).ToString().ToLower(),
                    dateSP,
                    dateETD); //Indra 20181115FM ETD First);
          }

          dicResultInfo.Clear();
        }

        X.AddScript(sb.ToString());
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_penjualan_PackingList:PopulateAutoGenerate - ", ex.Message));
    }

    sb.Remove(0, sb.Length);

    if (lstResultInfo != null)
    {
      lstResultInfo.Clear();
    }
    if (dicResult != null)
    {
      dicResult.Clear();
    }
    if (jarr != null)
    {
      jarr.Clear();
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void AutoGenBtn_Click(object sender, DirectEventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowAdd)
    {
      //Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      e.ErrorMessage = "Maaf, anda tidak mempunyai hak akses untuk menambah data.";

      e.Success = false;

      return;
    }

    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string custId = (e.ExtraParams["CustomerID"] ?? string.Empty);
    string suplId = (e.ExtraParams["PrinsipalID"] ?? string.Empty);
    string tipeCat = (e.ExtraParams["CatID"] ?? string.Empty);
    //Indra 20181226FM Penambahan filter ETD
    string txPeriode1 = (e.ExtraParams["txPeriode1"].Substring(0, 10) ?? string.Empty);
    string txPeriode2 = (e.ExtraParams["txPeriode2"].Substring(0, 10) ?? string.Empty);

    if (!string.IsNullOrEmpty(numberId))
    {
      e.ErrorMessage = "Generator hanya dapat di jalankan untuk pembuatan data baru.";
      e.Success = false;
      return;
    }
    else if (string.IsNullOrEmpty(custId))
    {
      e.ErrorMessage = "Nama cabang tidak terbaca.";
      e.Success = false;
      return;
    }

    //Indra 20181226FM Penambahan filter ETD
    //PopulateAutoGenerate(hfGudang.Text.Trim(), custId, suplId, tipeCat);
    PopulateAutoGenerate(hfGudang.Text.Trim(), custId, suplId, tipeCat, txPeriode1, txPeriode2);
  }

  public void Initialize(string gudang, string gudangDesc, string storeIDGridMain)
  {
    hfGudang.Text = gudang;
    hfStoreAutoGenID.Text = storeIDGridMain;
    hfGudangDesc.Text = gudangDesc;
    //Indra 20181226FM Penambahan filter ETD
    txPeriode1.Text = DateTime.Now.AddMonths(-1).AddDays(-DateTime.Now.Day + 1).ToString("dd-MM-yyyy");
    txPeriode2.Text = DateTime.Now.AddDays(28).ToString("dd-MM-yyyy");
    txPeriode1.Disabled = txPeriode2.Disabled = false;
  }

  public void CommandPopulate(bool isAdd, string pID)
  {
    if (isAdd)
    {
      ClearEntrys();

      wnDetail.Hidden = false;
      wnDetail.ShowModal();
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);
    string Supplier = (e.ExtraParams["Supplier"] ?? string.Empty);

    //bool isConfirm = false,
    //  isConfirmed = false;

    //if (bool.TryParse(hfConfMode.Text, out isConfirm))
    //{
    //  isConfirmed = (chkConfirm.Checked ? true : false);
    //}

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    if (isAdd)
    {
      if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
      {
        Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
        return;
      }
    }
    else if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowEdit)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menyimpan data ini.");
      return;
    }

    Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataPL);

    //string[] arrPL = null;

    //PackingListAutoGeneratorCtrlDetil.CommandPopulate(true, null, null);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        wnDetail.Hidden = true;

        //string scrpt = null;

        if (isAdd)
        {
          if (respon.Values != null)
          {
            string sIdPL = respon.Values.GetValueParser<string>("PL", string.Empty);

            string sIdPLVia = respon.Values.GetValueParser<string>("PLVIA", string.Empty);

            PackingListAutoGeneratorCtrlDetil.CommandPopulate(true, sIdPL, sIdPLVia, Supplier);

          }
        }
      }
    }
    else
    {
        if (respon.Message == "KeteranganED")
        {
            Functional.ShowMsgInformation("Periksa keterangan Acc ED");
            e.Success = true;
        }
        else
        {
            e.ErrorMessage = "Unknown response";
            e.Success = false;
        }
    }
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string plNumber, Dictionary<string, string>[] dics)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = plNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string tmp = null,
      sp = null,
      item = null,
      batch = null,
      ket = null,
      accket = null;

    decimal nQty = 0;
    bool isNew = false,
      isVoid = false,
      isModify = false,
      isED = false,
      isAccModify = false;
    string varData = null;

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", pag.Nip);
    pair.DicAttributeValues.Add("Customer", cbCustomerAutoGenHdr.Text);
    pair.DicAttributeValues.Add("Suplier", cbPrincipalAutoGenHdr.Text);
    pair.DicAttributeValues.Add("Type", cbPrincipalAutoGenHdr.Text);
    pair.DicAttributeValues.Add("Keterangan", txKeteranganAutoGen.Text.Trim());
    pair.DicAttributeValues.Add("Gudang", hfGudang.Text.Trim());
    pair.DicAttributeValues.Add("Confirm", "false");

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      isNew = dicData.GetValueParser<bool>("l_new");
      isVoid = dicData.GetValueParser<bool>("l_void");
      isModify = dicData.GetValueParser<bool>("l_modified");
      isED = dicData.GetValueParser<bool>("l_expired");
      isAccModify = dicData.GetValueParser<bool>("l_accmodify");

      //Verifikasi keterangan ED
      if (isED && string.IsNullOrEmpty(dicData.GetValueParser<string>("v_ket_ed")) && !isVoid)
      {
          responseResult.Message = "KeteranganED";
          responseResult.Response = ScmsSoaLibraryInterface.Components.PostDataParser.ResponseStatus.Failed;
          return responseResult;
      }

      if (isNew && (!isVoid) && (!isModify))
      {
        if (!pag.IsAllowAdd)
        {
          continue;
        }

        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        sp = dicData.GetValueParser<string>("c_sp");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");

        nQty = dicData.GetValueParser<decimal>("n_booked", 0);
        accket = dicData.GetValueParser<string>("v_ket_ed");

        if ((!string.IsNullOrEmpty(sp)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)) &&
          (nQty > 0))
        {
          dicAttr.Add("Sp", sp);
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("Qty", nQty.ToString());
          dicAttr.Add("isED", isED.ToString().ToLower());
          dicAttr.Add("accket", accket);

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr
          });
        }
      }
      else if ((!isNew) && isVoid && (!isModify))
      {
        if (!pag.IsAllowDelete)
        {
          continue;
        }

        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        sp = dicData.GetValueParser<string>("c_sp");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        ket = dicData.GetValueParser<string>("v_ket");

        nQty = dicData.GetValueParser<decimal>("n_QtyRequest", 0);

        if ((!string.IsNullOrEmpty(sp)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)))
        {
          dicAttr.Add("Sp", sp);
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("Qty", nQty.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
            DicAttributeValues = dicAttr
          });
        }
      }
      else if ((!isNew) && (!isVoid) && isModify)
      {
        if (!pag.IsAllowEdit)
        {
          continue;
        }

        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        sp = dicData.GetValueParser<string>("c_sp");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        ket = dicData.GetValueParser<string>("v_ket");

        nQty = dicData.GetValueParser<decimal>("n_QtyRequest", 0);
        accket = dicData.GetValueParser<string>("v_ket_ed");

        if ((!string.IsNullOrEmpty(sp)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)))
        {
          dicAttr.Add("Sp", sp);
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("Qty", nQty.ToString());
          dicAttr.Add("isED", isED.ToString().ToLower());
          dicAttr.Add("accket", accket);

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Modify at Confirm" : ket),
            DicAttributeValues = dicAttr
          });
        }
      }
      
      dicData.Clear();
    }

    try
    {
      varData = parser.ParserData("PackingListAutoGenerator", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_PackingListAutoGeneratorCtrl SaveParser : {0} ", ex.Message);
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

  protected void ReloadBtn_Click(object sender, DirectEventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void CommDetil_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["sId"] ?? string.Empty);
  }
}
