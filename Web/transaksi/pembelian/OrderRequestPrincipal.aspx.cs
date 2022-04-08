using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_pembelian_OrderRequestPrincipal : Scms.Web.Core.PageHandler
{
  private const string PROCESS_MODE = "PCM";
  private const string STANDARD_MODE = "STDM";
  
  #region Private

  private bool IsProcessMode
  {
    get { return hfMode.Text.Equals(PROCESS_MODE, StringComparison.OrdinalIgnoreCase); }
  }

  private void GetTypeName()
  {
    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    Dictionary<string, object> dicSP = null;
    Dictionary<string, string> dicSPInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    string[][] paramX = new string[][]{
        new string[] { "c_notrans = @0", "04", "System.String"},
        new string[] { "c_portal = @0", "3", "System.Char"},
        new string[] { "c_type = @0", "05", "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "2001", paramX);

    try
    {
      dicSP = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicSP.ContainsKey("records") && (dicSP.ContainsKey("totalRows") && (((long)dicSP["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicSP["records"]);

        dicSPInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        hfTypeName.Text = dicSPInfo.GetValueParser<string>("v_ket");
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_pembelian_OrderRequestPrincipal:GetTypeName - ", ex.Message));
    }
    finally
    {
      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicSPInfo != null)
      {
        dicSPInfo.Clear();
      }
      if (dicSP != null)
      {
        dicSP.Clear();
      }
    }

    #endregion

    GC.Collect();
  }

  private void FilterMode()
  {
    if (IsProcessMode)
    {
      Ext.Net.Store store = gridMain.GetStore();
      string paramX = null;

      Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

      if (store != null)
      {
        paramX = (store.BaseParams["parameters"] ?? null);

        if (paramX == null)
        {
          paramX = string.Format(@"[['c_orno', paramValueGetter({0}) + '%', ''],
                    ['d_ordate = @0', paramRawValueGetter({1}) , 'System.DateTime'],
                    ['c_pono', paramValueGetter({2}) + '%', ''],
                    ['c_type = @0', paramValueGetter({3}) , 'System.String'],
                    ['c_nosup = @0', paramValueGetter({4}) , 'System.String'],
                    ['(l_delete == null ? false : l_delete) = @0', 'false' , 'System.Boolean'],
                    ['c_type <> @0', '05' , 'System.String'],
                    ['gudang', '{5}', 'System.Char']]", txORFltr.ClientID,
                    txDateFltr.ClientID, txPOFltr.ClientID, cbTipeFltr.ClientID,
                    cbSuplierFltr.ClientID, pag.ActiveGudang);

          store.BaseParams.Add(new Ext.Net.Parameter("parameters", paramX));
        }
        else
        {
          paramX = string.Format(@"[['c_orno', paramValueGetter({0}) + '%', ''],
                    ['d_ordate = @0', paramRawValueGetter({1}) , 'System.DateTime'],
                    ['c_pono', paramValueGetter({2}) + '%', ''],
                    ['c_type = @0', paramValueGetter({3}) , 'System.String'],
                    ['c_nosup = @0', paramValueGetter({4}) , 'System.String'],
                    ['(l_delete == null ? false : l_delete) = @0', 'false' , 'System.Boolean'],
                    ['c_type <> @0', '05' , 'System.String'],
                    ['gudang', '{5}', 'System.Char']]", txORFltr.ClientID,
                    txDateFltr.ClientID, txPOFltr.ClientID, cbTipeFltr.ClientID,
                    cbSuplierFltr.ClientID, pag.ActiveGudang);

          store.BaseParams["parameters"] = paramX;
        }
      }

      store = cbTipeFltr.GetStore();

      if (store != null)
      {
        paramX = (store.BaseParams["parameters"] ?? null);

        if (paramX == null)
        {
          paramX = @"[['c_notrans = @0', '04', 'System.String'],
                    ['c_portal = @0', '3', 'System.Char'],
                    ['c_type <> @0', '05', 'System.String']]";

          store.BaseParams.Add(new Ext.Net.Parameter("parameters", paramX));
        }
        else
        {
          paramX = @"[['c_notrans = @0', '04', 'System.String'],
                    ['c_portal = @0', '3', 'System.Char'],
                    ['c_type <> @0', '05', 'System.String']]";

          store.BaseParams["parameters"] = paramX;
        }
      }
    }
  }
  
  private PostDataParser.StructureResponse SubmitParser(string orNumber)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    
    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();
    
    pair.IsSet = true;
    pair.IsList = true;
    //pair.Value = orNumber;
    //pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", this.Nip);
    pair.DicAttributeValues.Add("OrderID", orNumber);
    pair.DicAttributeValues.Add("Gdg", "1");
    //pair.DicAttributeValues.Add("Tanggal", date.ToString("yyyyMMddHHmmssfff"));
    //pair.DicAttributeValues.Add("SPCabang", txSpCabang.Text.Trim());
    //pair.DicAttributeValues.Add("Keterangan", keteranganHeader);
    //pair.DicAttributeValues.Add("Tipe", "05");
    //pair.DicAttributeValues.Add("Cek", chkCheck.Checked.ToString().ToLower());
    
    try
    {
      varData = parser.ParserData("PurchaseOrder", "Submit", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_OrderRequestPrinsipalCtrl SubmitParser : {0} ", ex.Message);
    }

    string result = null;

    if (!string.IsNullOrEmpty(varData))
    {
      Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

      result = soa.PostData(varData);
      //result = null;

      responseResult = parser.ResponseParser(result);
    }

    return responseResult;
  }
  
  #endregion

  private Control MyLoadControl(bool isProcess)
  {
    Control c = null;

    const string CONT_PROCESS = "x1x1x1x1";
    const string CONT_NORMAL = "x1x1x1";

    if (isProcess)
    {
      c = phCtrl.FindControl(CONT_PROCESS);

      if (c == null)
      {
        c = this.LoadControl("OrderRequestProcessPrinsipalCtrl.ascx");

        c.ID = CONT_PROCESS;

        phCtrl.Controls.Add(c);
      }
    }
    else
    {
      c = phCtrl.FindControl(CONT_NORMAL);

      if (c == null)
      {
        c = this.LoadControl("OrderRequestPrinsipalCtrl.ascx");

        c.ID = CONT_NORMAL;

        phCtrl.Controls.Add(c);
      }
    }

    return c;
  }

  private PostDataParser.StructureResponse DeleteParser(string orNumber, string ket)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = orNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", this.Nip);

    pair.DicAttributeValues.Add("Keterangan", ket.Trim());

    try
    {
      varData = parser.ParserData("OrderRequestPrincipal", "Delete", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_OrderRequestPrincipal DeleteParser : {0} ", ex.Message);
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

  [DirectMethod(ShowMask = true)]
  public void DeleteMethod(string orNumber, string keterangan)
  {
    if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowDelete)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menghapus data.");
      return;
    }

    if (string.IsNullOrEmpty(orNumber))
    {
      Functional.ShowMsgWarning("Nomor OR tidak terbaca.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Nomor OR tidak terbaca.";

      return;
    }
    else if (string.IsNullOrEmpty(keterangan))
    {
      Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Keterangan tidak boleh kosong.";

      return;
    }

    PostDataParser.StructureResponse respon = DeleteParser(orNumber, keterangan);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r);{0}.commitChanges(); }}",
        storeGridOR.ClientID, orNumber));

      Functional.ShowMsgInformation(string.Format("Nomor OR '{0}' telah terhapus.", orNumber));
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = respon.Message;
    }
  }

  [DirectMethod(ShowMask = true)]
  public void SubmitMethod(string orNumber)
  {
    if (!this.IsAllowEdit)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menyimpan data.");
      return;
    }

    PostDataParser.StructureResponse respon = SubmitParser(orNumber);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;
        string storeId = gridMain.GetStore().ClientID;

        if (respon.Values != null)
        {
          if (!string.IsNullOrEmpty(storeId))
          {
            scrpt = string.Format(@"var vIdx = {0}.findExact('c_orno', '{1}');
                if(vIdx != -1) {{
                  var r = {0}.getAt(vIdx);
                  if(!Ext.isEmpty(r)) {{
                    r.set('c_pono', '{2}');
                    r.set('orProses', true);
                    r.commit();
                  }}
                }}", storeId, orNumber,
                respon.Values.GetValueParser<string>("PO", string.Empty));

            X.AddScript(scrpt);
          }
        }

        Functional.ShowMsgInformation("Data OR berhasil terproses.");
      }
      else
      {
        Functional.ShowMsgWarning(respon.Message);
      }
    }
    else
    {
      Functional.ShowMsgWarning("Unknown response");
    }
  }

  protected void Page_Init(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

      if (qryString.Equals("process", StringComparison.OrdinalIgnoreCase))
      {
        hfMode.Text = PROCESS_MODE;
      }
      else
      {
        hfMode.Text = STANDARD_MODE;
      }
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      GetTypeName();

      FilterMode();

      hfGdg.Text = this.ActiveGudang;
      //OrderRequestPrinsipalCtrl1.Initialize(storeGridOR.ClientID, hfTypeName.Text);
      //OrderRequestProcessPrinsipalCtrl1.Initialize(storeGridOR.ClientID, hfTypeName.Text);
    }

    //if (this.IsProcessMode)
    //{
    //  MyLoadControl(true, false);
    //  MyLoadControl(true, true);
    //}
    //else
    //{
    //  MyLoadControl(false, false);
    //}

    if (this.IsProcessMode)
    {
      transaksi_pembelian_OrderRequestProcessPrinsipalCtrl cp = MyLoadControl(true) as transaksi_pembelian_OrderRequestProcessPrinsipalCtrl;
      transaksi_pembelian_OrderRequestPrinsipalCtrl cn = MyLoadControl(false) as transaksi_pembelian_OrderRequestPrinsipalCtrl;

      cp.Initialize(storeGridOR.ClientID, hfTypeName.Text);
      cn.Initialize(storeGridOR.ClientID, hfTypeName.Text);
    }
    else
    {
      MyLoadControl(false);

      transaksi_pembelian_OrderRequestPrinsipalCtrl cn = MyLoadControl(false) as transaksi_pembelian_OrderRequestPrinsipalCtrl;

      cn.Initialize(storeGridOR.ClientID, hfTypeName.Text);
    }
  }

  protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
  {
    if (!this.IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    //OrderRequestGudangCtrl1.CommandPopulate(true, null);  
    //OrderRequestProcessPrinsipalCtrl1.CommandPopulate(true, null);
    
    if (this.IsProcessMode)
    {
      transaksi_pembelian_OrderRequestProcessPrinsipalCtrl orp = MyLoadControl(true) as transaksi_pembelian_OrderRequestProcessPrinsipalCtrl;

      if (orp == null)
      {
        Functional.ShowMsgError("Objek kontrol Proses Order Request tidak dapat dibuka.");
      }
      else
      {
        orp.CommandPopulate(true, null);
      }
    }
    else
    {
      transaksi_pembelian_OrderRequestPrinsipalCtrl orp = MyLoadControl(false) as transaksi_pembelian_OrderRequestPrinsipalCtrl;

      if (orp == null)
      {
        Functional.ShowMsgError("Objek kontrol Order Request tidak dapat dibuka.");
      }
      else
      {
        orp.CommandPopulate(true, null);
      }
    }
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    //if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    //{
    //  //OrderRequestGudangCtrl1.CommandPopulate(false, pID);
    //  OrderRequestProcessPrinsipalCtrl1.CommandPopulate(false, pID);
    //}

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      transaksi_pembelian_OrderRequestPrinsipalCtrl orp = MyLoadControl(false) as transaksi_pembelian_OrderRequestPrinsipalCtrl;

      if (orp == null)
      {
        Functional.ShowMsgError("Objek kontrol Order Request tidak dapat dibuka.");
      }
      else
      {
        orp.CommandPopulate(false, pID);
      }

      #region Old Coded

      //if (this.IsProcessMode)
      //{
      //  transaksi_pembelian_OrderRequestProcessPrinsipalCtrl orp = MyLoadControl(true) as transaksi_pembelian_OrderRequestProcessPrinsipalCtrl;

      //  if (orp == null)
      //  {
      //    Functional.ShowMsgError("Objek kontrol Proses Order Request tidak dapat dibuka.");
      //  }
      //  else
      //  {
      //    orp.CommandPopulate(false, pID);
      //  }
      //}
      //else
      //{
      //  transaksi_pembelian_OrderRequestPrinsipalCtrl orp = MyLoadControl(false) as transaksi_pembelian_OrderRequestPrinsipalCtrl;

      //  if (orp == null)
      //  {
      //    Functional.ShowMsgError("Objek kontrol Order Request tidak dapat dibuka.");
      //  }
      //  else
      //  {
      //    orp.CommandPopulate(false, pID);
      //  }
      //}

      #endregion
    }

    #region Old Coded

//    else if (cmd.Equals("Submit", StringComparison.OrdinalIgnoreCase))
//    {
//      PostDataParser.StructureResponse respon = SubmitParser(pID);

//      if (respon.IsSet)
//      {
//        if (respon.Response == PostDataParser.ResponseStatus.Success)
//        {
//          string scrpt = null;
//          string storeId = gridMain.GetStore().ClientID;

//          if (respon.Values != null)
//          {
//            if (!string.IsNullOrEmpty(storeId))
//            {
//              scrpt = string.Format(@"var vIdx = {0}.findExact('c_orno', '{1}');
//                if(vIdx != -1) {{
//                  var r = {0}.getAt(vIdx);
//                  if(!Ext.isEmpty(r)) {{
//                    r.set('c_pono', '{2}');
//                    r.set('orProses', true);
//                    r.commit();
//                  }}
//                }}", storeId, pID, 
//                  respon.Values.GetValueParser<string>("PO", string.Empty));

//              X.AddScript(scrpt);
//            }
//          }

//          Functional.ShowMsgInformation("Data OR berhasil terproses.");
//        }
//        else
//        {
//          e.ErrorMessage = respon.Message;

//          e.Success = false;
//        }
//      }
//      else
//      {
//        e.ErrorMessage = "Unknown response";

//        e.Success = false;
//      }
//    }

    #endregion

    GC.Collect();
  }
}
