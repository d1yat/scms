using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_penjualan_DO_PL : System.Web.UI.UserControl
{
  #region Private

  private static string sCabang = null;

  private void ClearEntrys()
  {
    winDetail.Title = "Delivery Order";

    hfDONo.Clear();

    cbCustomerHdr.Clear();
    cbCustomerHdr.Disabled = false;

    cbViaPLDtlHdr.Clear();
    cbViaPLDtlHdr.Disabled = false;

    cbPLHdr.Clear();
    cbPLHdr.Disabled = false;

    Ext.Net.Store cbPLHdrstr = cbPLHdr.GetStore();
    if (cbPLHdrstr != null)
    {
        cbPLHdrstr.RemoveAll();
    }

    txKet.Clear();
    txKet.Disabled = false;

    btnPrint.Hidden = true;
    lblTot.Hidden = true;

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private void PopulateDetailPL(string pName, string gdgId, string pID)
  {
    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"},
        new string[] { "c_gdg = @0", gdgId, "System.Char"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0001", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        //cbViaHdr.SetValueAndFireSelect((dicResultInfo.ContainsKey("c_via") ? dicResultInfo["c_via"] : string.Empty));
        //cbViaHdr.Disabled = true;
        Functional.SetComboData(cbViaPLDtlHdr, "c_type", dicResultInfo.GetValueParser<string>("v_ket_via", string.Empty), dicResultInfo.GetValueParser<string>("c_via", string.Empty));
        //XSetComboData(cbViaHdr, "c_type", dicResultInfo.GetValueParser<string>("v_ket_via", string.Empty), dicResultInfo.GetValueParser<string>("c_via", string.Empty), false);
        
        var baspbNo = dicResultInfo.GetValueParser<string>("c_baspbno", string.Empty);
        txKet.Text = dicResultInfo.GetValueParser<string>("v_ket", string.Empty) + " " + baspbNo;
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_penjualan_DO_PL:PopulateDetailPL Header - ", ex.Message));
    }
    finally
    {
      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicResultInfo != null)
      {
        dicResultInfo.Clear();
      }
      if (dicResult != null)
      {
        dicResult.Clear();
      }
    }

    #endregion
  }

  private void PopulateDetail(string pName, string gdgId, string pID, string Case)
  {
    Dictionary<string, object> dicDO = null;
    Dictionary<string, string> dicDOInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string tmp = null;
    try
    {
      switch (Case)
      {
        #region Add
        case "Add":
          {
            Ext.Net.Store store = gridDetail.GetStore();

            tmp = string.Format(@"var xOpts = {{
                                  params: {{
                                      start: '0',
                                      limit: '-1',
                                      allQuery: 'true',
                                      model: '0008a',
                                      sort: '',
                                      dir: '',
                                      parameters: [['c_plno = @0', paramValueGetter({0}), 'System.String'],
                                        ['c_gdg = @0', paramValueGetter({1}), 'System.Char']]
                                    }}
                                  }};", cbPLHdr.ClientID, hfGudang.ClientID);

            PopulateDetailPL("c_plno", gdgId, pID);

            X.AddScript(tmp);
            X.AddScript(string.Format("{0}.removeAll();{0}.reload(xOpts);", store.ClientID));

          }
          break;
        #endregion
        
        #region View

        case "View":

          string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0007", paramX);

          dicDO = JSON.Deserialize<Dictionary<string, object>>(res);
          if (dicDO.ContainsKey("records") && (dicDO.ContainsKey("totalRows") && (((long)dicDO["totalRows"]) > 0)))
          {
            jarr = new Newtonsoft.Json.Linq.JArray(dicDO["records"]);

            dicDOInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());
            sCabang = dicDOInfo.ContainsKey("c_cusno") ? dicDOInfo["c_cusno"] : string.Empty;

            //cbCustomerHdr.ToBuilder().AddItem(
            //        dicDOInfo.ContainsKey("v_cunam") ? dicDOInfo["v_cunam"] : string.Empty,
            //        dicDOInfo.ContainsKey("c_cusno") ? dicDOInfo["c_cusno"] : string.Empty
            //        );
            //if (cbCustomerHdr.GetStore() != null)
            //{
            //  cbCustomerHdr.GetStore().CommitChanges();
            //}
            //cbCustomerHdr.SetValueAndFireSelect((dicDOInfo.ContainsKey("c_cusno") ? dicDOInfo["c_cusno"] : string.Empty));
            //cbCustomerHdr.Disabled = true;
            Functional.SetComboData(cbCustomerHdr, "c_cusno", dicDOInfo.GetValueParser<string>("v_cunam", string.Empty), dicDOInfo.GetValueParser<string>("c_cusno", string.Empty));
            cbCustomerHdr.Disabled = true;

            cbPLHdr.SetValueAndFireSelect((dicDOInfo.ContainsKey("c_plno") ? dicDOInfo["c_plno"] : string.Empty));
            cbPLHdr.Disabled = true;

            cbViaPLDtlHdr.SetValueAndFireSelect((dicDOInfo.ContainsKey("v_ket") ? dicDOInfo["v_ket"] : string.Empty));
            cbViaPLDtlHdr.Disabled = false;

            txKet.Text = ((dicDOInfo.ContainsKey("v_ketdo") ? dicDOInfo["v_ketdo"] : string.Empty));
            txKet.Disabled = false;

            lblTot.Hidden = false;

            //X.AddScript(string.Format("clearRelatedComboRecursive(true, {0});", cbPLHdr.ClientID));

            jarr.Clear();

            btnPrint.Hidden = false;

            #region Parser Detail
              
            Ext.Net.Store store = gridDetail.GetStore();

             // ListItemCollection lc = gridMain.Items
            //if (store.Proxy.Count > 0)
            //{
            //  Ext.Net.ScriptTagProxy stp = store.Proxy[0] as Ext.Net.ScriptTagProxy;
            //  if ((stp != null) && stp.CallbackParam.Equals(Functional.NAME_SOA_SCMS_CALLBACK, StringComparison.OrdinalIgnoreCase))
            //  {
            //    string param = (store.BaseParams["parameters"] ?? string.Empty);
            //    if (string.IsNullOrEmpty(param))
            //    {
            //      store.BaseParams.Add(new Ext.Net.Parameter("parameters", string.Format("[['{0} = @0', '{1}', 'System.String']]", pName, pID), ParameterMode.Raw));
            //    }
            //    else
            //    {
            //      store.BaseParams["parameters"] = string.Format("[['{0} = @0', '{1}', 'System.String']]", pName, pID);
            //    }
            //  }
            //}

            tmp = string.Format(@"var xOpts = {{
                                  params: {{
                                      start: '0',
                                      limit: '-1',
                                      allQuery: 'true',
                                      model: '0008',
                                      sort: '',
                                      dir: '',
                                      parameters: [['{0} = @0', '{1}', 'System.String']]
                                    }}
                                  }};",pName, pID);

            X.AddScript(tmp);
            

            hfDONo.Text = pID;

            winDetail.Hidden = false;
            winDetail.ShowModal();
            winDetail.Title = string.Format("Delivery Order - {0}", pID);
            X.AddScript(string.Format("{0}.removeAll();{0}.reload(xOpts);", store.ClientID));

            GC.Collect();
            #endregion
          }

          break;

          #endregion
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
      string.Concat("transaction_sales_PackingList:PopulateDetail Detail - ", ex.Message));
    }
    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string doNumberID, Dictionary<string, string>[] dics)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = doNumberID;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    
    string tmp = null,
      item = null, ket = null;

    int nQty = 0;
    bool isNew = false,
      isVoid = false, isModify = false;
    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);

    pair.DicAttributeValues.Add("Via", cbViaPLDtlHdr.Text);
    pair.DicAttributeValues.Add("Customer", cbCustomerHdr.SelectedItem.Value);
    pair.DicAttributeValues.Add("Plno", cbPLHdr.SelectedItem.Value);
    pair.DicAttributeValues.Add("Keterangan", txKet.Text);
    pair.DicAttributeValues.Add("Gudang", hfGudang.Text.Trim());

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      isNew = dicData.GetValueParser<bool>("l_new");
      isVoid = dicData.GetValueParser<bool>("l_void");
      isModify = dicData.GetValueParser<bool>("l_modified");

      if (isVoid && (!isModify))
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        nQty = dicData.GetValueParser<int>("n_sisa", 0);
        ket = dicData.GetValueParser<string>("v_ket");

        if (
          (!string.IsNullOrEmpty(item)) &&
          (nQty > 0))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Qty", nQty.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
            DicAttributeValues = dicAttr
          });
        }
      }
      else if (isNew && !isModify && !isVoid)
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", "true");
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        nQty = dicData.GetValueParser<int>("n_sisa", 0);

        if (
          (!string.IsNullOrEmpty(item)) &&
          (nQty > 0))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Qty", nQty.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr
          });
        }
      }
    }

    dicData.Clear();

    try
    {
      varData = parser.ParserData("DOPL", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_DO_PL SaveParser : {0} ", ex.Message);
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

  private PostDataParser.StructureResponse DeleteParser(string doNumber, string ket)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = doNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);

    pair.DicAttributeValues.Add("Keterangan", ket.Trim());

    try
    {
      varData = parser.ParserData("DOPL", "Delete", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_DO_PL SaveParser : {0} ", ex.Message);
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

  private PostDataParser.StructureResponse SubmitParser(string doNumberID)
  {
    Scms.Web.Core.PageHandler page = this.Page as Scms.Web.Core.PageHandler;

    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = doNumberID;
    //pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", page.Nip);
    pair.DicAttributeValues.Add("ConfirmSent", "true");

    try
    {
      varData = parser.ParserData("DOPL", "ConfirmSent", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_DO_PL SubmitParser : {0} ", ex.Message);
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

  protected void Page_Load(object sender, EventArgs e)
  {
    //if (!Functional.IsAllowView(this.Page as Scms.Web.Core.PageHandler))
    //{
    //  return;
    //}

    if (!this.IsPostBack)
    {
      hfGudang.Text = ((Scms.Web.Core.PageHandler)this.Page).ActiveGudang;

      winDetail.Hidden = true;
      hfStoreID.Text = storeGridPL.ClientID;
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  public void DeleteMethod(string doNumber, string keterangan)
  {
    if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowDelete)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menghapus data.");
      return;
    }

    if (string.IsNullOrEmpty(doNumber))
    {
      Functional.ShowMsgWarning("Nomor DO tidak terbaca.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Nomor DO tidak terbaca.";

      return;
    }
    else if (string.IsNullOrEmpty(keterangan))
    {
      Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Keterangan tidak boleh kosong.";

      return;
    }
    PostDataParser.StructureResponse respon = DeleteParser(doNumber, keterangan);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r); {0}.commitChanges(); }}",
        storeGridPL.ClientID, doNumber));

      Functional.ShowMsgInformation(string.Format("Nomor DO '{0}' telah terhapus.", doNumber));
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = respon.Message;
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  public void SubmitMethod(string doNumber)
  {
    if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowEdit)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menyimpan data.");
      return;
    }

    if (string.IsNullOrEmpty(doNumber))
    {
      Functional.ShowMsgWarning("Nomor DO tidak terbaca.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Nomor DO tidak terbaca.";

      return;
    }

    PostDataParser.StructureResponse respon = SubmitParser(doNumber);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;

        scrpt = string.Format(@"var vIdx = {0}.findExact('c_dono', '{1}'); 
                if(vIdx != -1) {{
                  var r = {0}.getAt(vIdx);
                  if(!Ext.isEmpty(r)) {{
                    r.set('l_sent', true);
                    r.commit();
                  }}
                }}", gridMain.GetStore().ClientID, doNumber);

        X.AddScript(scrpt);

        Functional.ShowMsgInformation("Data DO berhasil terproses.");
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

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);
    string gdgId = (e.ExtraParams["GudangID"] ?? string.Empty);
    
    if (cmd.Equals("Delete", StringComparison.OrdinalIgnoreCase))
    {
      //DeleteCommand(pName, pID);
    } 
    else if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      PopulateDetail(pName, gdgId, pID, "View");
    }
  }

  protected void btnAddNew_OnClick(object sender, EventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    this.ClearEntrys();

    winDetail.Hidden = false;
    winDetail.ShowModal();

    //Ext.Net.Store store = gridDetail.GetStore();

//    string tmp = string.Format(@"{0}.setBaseParam('model','0008a');
//        {0}.setBaseParam('parameters',[['c_plno = @0', paramValueGetter({1}), 'System.String']]);", store.ClientID,
//      cbPLHdr.ClientID);

//    X.AddScript(tmp);
    
    //store.BaseParams.Add(new Ext.Net.Parameter("Model", "0008a", ParameterMode.Value));

    //store.BaseParams.Add(new Ext.Net.Parameter("Model",
    //  string.Format("[['c_plno = @0', paramValueGetter({0}), 'System.String']]", cbPLHdr.ClientID), ParameterMode.Raw));
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void OnEvenAddGrid(object sender, DirectEventArgs e)
  {
    //string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);
    string gdgId = (e.ExtraParams["GudangID"] ?? string.Empty);

    PopulateDetail(pName, gdgId, pID, "Add");

    GC.Collect();
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string NumberID = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jSonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    
    bool isAdd = (string.IsNullOrEmpty(NumberID) ? true : false);

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
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menyimpan data.");
      return;
    }

    if (cbPLHdr.SelectedIndex < 0)
    {
        Functional.ShowMsgInformation("Nomor PL belum dipilih.");
        return;
    }

    Dictionary<string, string>[] gridDataDO = JSON.Deserialize<Dictionary<string, string>[]>(jSonGridValues);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, NumberID, gridDataDO);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;
        string storeId = hfStoreID.Text;

        string cust = (cbCustomerHdr.SelectedItem != null ? cbCustomerHdr.SelectedItem.Text : string.Empty);
        string Via = (cbViaPLDtlHdr.SelectedItem != null ? cbViaPLDtlHdr.SelectedItem.Text : string.Empty);
        string noPl = (cbPLHdr.SelectedItem != null ? cbPLHdr.SelectedItem.Text : string.Empty);

        string dateJs = null;
        DateTime date = DateTime.Today;

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
        {
          dateJs = Functional.DateToJson(date);
        }

        if (isAdd)
        {
          NumberID = respon.Values.GetValueParser<string>("DO", string.Empty);

          if (!string.IsNullOrEmpty(storeId))
          {
            scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                                    'c_dono': '{1}'
                                    ,'d_dodate': {2}
                                    ,'v_gdgdesc': '{3}'
                                    ,'v_cunam': '{4}'
                                    ,'v_ket': '{5}'
                                    ,'c_plno': '{6}'
                                    ,'c_pin': '{7}'
                                    ,'c_expno': '{8}'
                                    ,'l_confirm': false }}));{0}.commitChanges();",
                                        storeId, NumberID,
                                        dateJs,
                                        pag.ActiveGudangDescription,
                                        cust, Via, noPl,
                                        respon.Values.GetValueParser<string>("Pin", string.Empty),
                                        string.Empty);

            X.AddScript(scrpt);
          }
        }

        //this.ClearEntrys();
        this.PopulateDetail("c_dono", pag.ActiveGudang, NumberID, "View");

        Functional.ShowMsgInformation("Data berhasil tersimpan.");
      }
      else
      {
        e.ErrorMessage = respon.Message;

        e.Success = false;
      }
    }
    else
    {
      e.ErrorMessage = "Unknown response";

      e.Success = false;
    }
  }

  protected void btnPrintDO_OnClick(object sender, DirectEventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowPrinting)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
      return;
    }

    DOPLPrint.ShowPrintPage();
    //DO_PL_Print.ShowPrintPage();
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    if (!pag.IsAllowPrinting)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
      return;
    }

    string gdgId = pag.ActiveGudang;
    string numberID = (e.ExtraParams["NumberID"] ?? string.Empty);

    if (string.IsNullOrEmpty(numberID))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");

      return;
    }

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();

    if (hfGudang.Text.Equals("2"))
    {
      rptParse.ReportingID = "101021";
    }
    else
    {
      rptParse.ReportingID = "10102";
    }
    

    #region Linq Filter Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(char).FullName,
      ParameterName = "c_gdg = @0",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId),
      IsLinqFilterParameter = true
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "c_type = @0",
      ParameterValue = "01",
      IsLinqFilterParameter = true
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "c_dono = @0",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID),
      IsLinqFilterParameter = true
    });

    #endregion

    #region Report Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(char).FullName,
      ParameterName = "LG_DOH.c_gdg",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_DOH.c_type",
      ParameterValue = "01"
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_DOH.c_dono",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "({LG_PLD1.n_qty} <> 0)",
      IsReportDirectValue = true
    });

    #endregion

    rptParse.PaperID = "8.5x5.5";
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;
    rptParse.UserDefinedName = numberID;
    //rptParse.IsBarcode = true;

    string xmlFiles = ReportParser.Deserialize(rptParse);

    SoaReportCaller soa = new SoaReportCaller();

    string result = soa.GeneratorReport(xmlFiles);

    ReportingResult rptResult = ReportingResult.Serialize(result);

    if (rptResult == null)
    {
      Functional.ShowMsgError("Pembuatan report gagal.");
    }
    else
    {
      if (rptResult.IsSuccess)
      {
        //string rptName = string.Concat("Packing_List_", pag.Nip, ".", rptResult.Extension);

        //string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
        //tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
        //  tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

        string tmpUri = Functional.UriDownloadGenerator(pag,
          rptResult.OutputFile, "Delivery Order ", rptResult.Extension);

                wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));

                PostDataParser.StructureResponse respon = SubmitParser(numberID);

                if (respon.IsSet)
                {
                    if (respon.Response == PostDataParser.ResponseStatus.Success)
                    {
                        string scrpt = null;

                        scrpt = string.Format(@"var vIdx = {0}.findExact('c_dono', '{1}'); 
                            if(vIdx != -1) {{
                              var r = {0}.getAt(vIdx);
                              if(!Ext.isEmpty(r)) {{
                                r.set('l_sent', true);
                                r.commit();
                              }}
                            }}", gridMain.GetStore().ClientID, numberID);

                        X.AddScript(scrpt);

                        //Functional.ShowMsgInformation("Data DO berhasil terproses.");
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
            else
            {
                Functional.ShowMsgWarning(rptResult.MessageResponse);
            }
        }

    GC.Collect();
  }

  protected void ReloadBtn_Click(object sender, DirectEventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    this.ClearEntrys();
  }
}
