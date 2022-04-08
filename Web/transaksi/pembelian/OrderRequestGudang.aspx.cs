using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_pembelian_OrderRequestGudang : Scms.Web.Core.PageHandler
{
  private const string PROCESS_MODE = "PCM";
  private const string STANDARD_MODE = "STDM";

  #region Private

  private bool IsProcessMode
  {
    get { return hfMode.Text.Equals(PROCESS_MODE, StringComparison.OrdinalIgnoreCase); }
  }

  private void GetTypeName(string typeCode)
  {
    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    Dictionary<string, object> dicSP = null;
    Dictionary<string, string> dicSPInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    string[][] paramX = new string[][]{
        new string[] { "c_notrans = @0", "48", "System.String"},
        new string[] { "c_portal = @0", "3", "System.Char"},
        new string[] { "c_type = @0", typeCode, "System.String"}
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
        string.Concat("transaksi_pembelian_OrderRequestGudang:GetTypeName - ", ex.Message));
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
    Ext.Net.Store store = gridMain.GetStore();
    string paramX = null;

    if (IsProcessMode)
    {
      if (store != null)
      {
        paramX = (store.BaseParams["parameters"] ?? null);

        if (paramX == null)
        {
          paramX = string.Format(@"[['c_spgno', paramValueGetter({0}) + '%', ''],
                    ['d_spgdate = @0', paramRawValueGetter({1}) , 'System.DateTime'],
                    ['c_type = @0', '{2}', 'System.String'],
                    ['c_nosup = @0', paramValueGetter({3}) , 'System.String'],
                    ['(l_delete == null ? false : l_delete) = @0', 'false', 'System.Boolean'],
                    ['c_gdg1 = @0', paramValueGetter({4}) , 'System.Char']]", 
                    txSPGFltr.ClientID, txDateFltr.ClientID, "00", 
                    cbSuplierFltr.ClientID, hfGudang.ClientID);

          store.BaseParams.Add(new Ext.Net.Parameter("parameters", paramX));
        }
        else
        {
          paramX = string.Format(@"[['c_spgno', paramValueGetter({0}) + '%', ''],
                    ['d_spgdate = @0', paramRawValueGetter({1}) , 'System.DateTime'],
                    ['c_type = @0', '{2}', 'System.String'],
                    ['c_nosup = @0', paramValueGetter({3}) , 'System.String'],
                    ['(l_delete == null ? false : l_delete) = @0', 'false', 'System.Boolean'],
                    ['c_gdg1 = @0', paramValueGetter({4}), 'System.Char']]",
                    txSPGFltr.ClientID, txDateFltr.ClientID, "00",
                    cbSuplierFltr.ClientID, hfGudang.ClientID);

          store.BaseParams["parameters"] = paramX;
        }
      }

      store = cbTipeFltr.GetStore();

      if (store != null)
      {
        paramX = (store.BaseParams["parameters"] ?? null);

        if (paramX == null)
        {
          paramX = @"[['c_notrans = @0', '48', 'System.String'],
                    ['c_portal = @0', '3', 'System.Char'],
                    ['c_type = @0', '00', 'System.String']]";

          store.BaseParams.Add(new Ext.Net.Parameter("parameters", paramX));
        }
        else
        {
          paramX = @"[['c_notrans = @0', '48', 'System.String'],
                    ['c_portal = @0', '3', 'System.Char'],
                    ['c_type = @0', '00', 'System.String']]";

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
    pair.Value = orNumber;
    //pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", this.Nip);
    //pair.DicAttributeValues.Add("OrderID", orNumber);
    pair.DicAttributeValues.Add("From", this.ActiveGudang);
    pair.DicAttributeValues.Add("Confirm", "true");

    try
    {
      varData = parser.ParserData("OrderRequestGudang", "Submit", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_OrderRequestGudang SubmitParser : {0} ", ex.Message);
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
        c = this.LoadControl("OrderRequestProcessGudangCtrl.ascx");

        c.ID = CONT_PROCESS;

        phCtrl.Controls.Add(c);
      }
    }
    else
    {
      c = phCtrl.FindControl(CONT_NORMAL);

      if (c == null)
      {
        c = this.LoadControl("OrderRequestGudangCtrl.ascx");

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
    pair.DicAttributeValues.Add("From", this.ActiveGudang);
    pair.DicAttributeValues.Add("Keterangan", ket.Trim());

    try
    {
      varData = parser.ParserData("OrderRequestGudang", "Delete", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_OrderRequestGudang DeleteParser : {0} ", ex.Message);
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

  #endregion

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
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

        if (!string.IsNullOrEmpty(storeId))
        {
          scrpt = string.Format(@"var vIdx = {0}.findExact('c_spgno', '{1}');
                if(vIdx != -1) {{
                  var r = {0}.getAt(vIdx);
                  if(!Ext.isEmpty(r)) {{
                    r.set('c_spgno', '{2}');
                    r.set('l_status', true);
                    r.commit();
                  }}
                }}", storeId, orNumber, orNumber);

          X.AddScript(scrpt);
        }

        Functional.ShowMsgInformation("Data Order Gudang berhasil terproses.");
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

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  public void DeleteMethod(string orgNumber, string keterangan)
  {
    if (!this.IsAllowDelete)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menghapus data.");
      return;
    }

    if (string.IsNullOrEmpty(orgNumber))
    {
      Functional.ShowMsgWarning("Nomor OR Gudang tidak terbaca.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Nomor OR Gudang tidak terbaca.";

      return;
    }
    else if (string.IsNullOrEmpty(keterangan))
    {
      Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Keterangan tidak boleh kosong.";

      return;
    }

    PostDataParser.StructureResponse respon = DeleteParser(orgNumber, keterangan);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r);{0}.commitChanges(); }}",
        storeGridOR.ClientID, orgNumber));

      Functional.ShowMsgInformation(string.Format("Nomor OR '{0}' telah terhapus.", orgNumber));
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = respon.Message;
    }
  }

  protected void Page_Init(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

      if (qryString.Equals("process", StringComparison.OrdinalIgnoreCase))
      {
        GetTypeName("00");

        hfMode.Text = PROCESS_MODE;
      }
      else
      {
        GetTypeName("01");

        hfMode.Text = STANDARD_MODE;
      }

      //hfGudang.Text = this.ActiveGudang;
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      FilterMode();

      //OrderRequestGudangCtrl1.Initialize(storeGridOR.ClientID, hfTypeName.Text);
      //OrderRequestProcessGudangCtrl1.Initialize(storeGridOR.ClientID, hfTypeName.Text);
    }

    if (this.IsProcessMode)
    {
      transaksi_pembelian_OrderRequestProcessGudangCtrl cp = MyLoadControl(true) as transaksi_pembelian_OrderRequestProcessGudangCtrl;
      transaksi_pembelian_OrderRequestGudangCtrl cn = MyLoadControl(false) as transaksi_pembelian_OrderRequestGudangCtrl;

      cp.Initialize(storeGridOR.ClientID, hfTypeName.Text);
      cn.Initialize(storeGridOR.ClientID, hfTypeName.Text);
    }
    else
    {
      MyLoadControl(false);

      transaksi_pembelian_OrderRequestGudangCtrl cn = MyLoadControl(false) as transaksi_pembelian_OrderRequestGudangCtrl;

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
    //OrderRequestProcessGudangCtrl1.CommandPopulate(true, null);

    if (this.IsProcessMode)
    {
      transaksi_pembelian_OrderRequestProcessGudangCtrl orp = MyLoadControl(true) as transaksi_pembelian_OrderRequestProcessGudangCtrl;

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
      transaksi_pembelian_OrderRequestGudangCtrl orp = MyLoadControl(false) as transaksi_pembelian_OrderRequestGudangCtrl;

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
    //  OrderRequestProcessGudangCtrl1.CommandPopulate(false, pID);
    //}

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      transaksi_pembelian_OrderRequestGudangCtrl orp = MyLoadControl(false) as transaksi_pembelian_OrderRequestGudangCtrl;

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
      //  transaksi_pembelian_OrderRequestProcessGudangCtrl orp = MyLoadControl(true) as transaksi_pembelian_OrderRequestProcessGudangCtrl;

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
      //  transaksi_pembelian_OrderRequestGudangCtrl orp = MyLoadControl(false) as transaksi_pembelian_OrderRequestGudangCtrl;

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

    GC.Collect();
  }
}
