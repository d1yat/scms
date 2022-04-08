using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_penjualan_DO_STTCtrl : System.Web.UI.UserControl
{
  #region Private 
  
  private void ClearEntrys()
  {
    winDetail.Title = "Delivery Order Surat Tanda Terima (STT)";

    hfDONoStt.Clear();

    cbGudangHdr.Clear();
    cbGudangHdr.Disabled = false;

    cbCustomerHdr.Clear();
    cbCustomerHdr.Disabled = false;

    cbSTTSampleHdr.Clear();
    cbSTTSampleHdr.Disabled = false;

    Ext.Net.Store cbSTTSampleHdrstr = cbSTTSampleHdr.GetStore();
    if (cbSTTSampleHdrstr != null)
    {
        cbSTTSampleHdrstr.RemoveAll();
    }

    cbViaHdr.Clear();
    cbViaHdr.Disabled = false;

    txKeterangan.Clear();
    txKeterangan.Disabled = false;

    btnPrint.Hidden = true;

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
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
            Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_DO_STT SubmitParser : {0} ", ex.Message);
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

    pair.DicAttributeValues.Add("Via", cbViaHdr.Text);
    pair.DicAttributeValues.Add("Customer", cbCustomerHdr.Text);
    pair.DicAttributeValues.Add("sttNO", cbSTTSampleHdr.Text);
    pair.DicAttributeValues.Add("Keterangan", txKeterangan.Text);

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      isNew = dicData.GetValueParser<bool>("l_new");
      isVoid = dicData.GetValueParser<bool>("l_void");
      isModify = dicData.GetValueParser<bool>("l_modified");

      if (isVoid && (!isModify) && (!isNew))
      {

        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        nQty = dicData.GetValueParser<int>("n_qty", 0);
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
      if ((!isVoid) && (!isModify) && (isNew))
      {

        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", "true");
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        nQty = dicData.GetValueParser<int>("n_qty", 0);

        if ((!string.IsNullOrEmpty(item)) &&
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
      varData = parser.ParserData("DOSTT", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_DO_STTCtrl SaveParser : {0} ", ex.Message);
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

  public void CommandPopulate(bool isAdd, string pID)
  {
    if (isAdd)
    {
      this.ClearEntrys();

      winDetail.Hidden = false;
    }
  }

  public void PopulateDetail(string pName, string pID, string Case)
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
                                      model: '0010a',
                                      sort: '',
                                      dir: '',
                                      parameters: [['noStt', paramValueGetter({0}), 'System.String']]
                                    }}
                                  }};", cbSTTSampleHdr.ClientID);

            X.AddScript(tmp);

            X.AddScript(string.Format("{0}.removeAll();{0}.reload(xOpts);", store.ClientID));

          }
          break;
        #endregion

        #region View

        case "View":
          {
            winDetail.Hidden = false;
            this.ClearEntrys();

            string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0010", paramX);

            dicDO = JSON.Deserialize<Dictionary<string, object>>(res);
            if (dicDO.ContainsKey("records") && (dicDO.ContainsKey("totalRows") && (((long)dicDO["totalRows"]) > 0)))
            {

              jarr = new Newtonsoft.Json.Linq.JArray(dicDO["records"]);

              dicDOInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

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

              //cbGudangHdr.ToBuilder().AddItem(
              //        dicDOInfo.ContainsKey("v_gdgdesc") ? dicDOInfo["v_gdgdesc"] : string.Empty,
              //        dicDOInfo.ContainsKey("c_gdg") ? dicDOInfo["c_gdg"] : string.Empty
              //        );
              //if (cbGudangHdr.GetStore() != null)
              //{
              //  cbGudangHdr.GetStore().CommitChanges();
              //}
              //cbGudangHdr.SetValueAndFireSelect((dicDOInfo.ContainsKey("c_gdg") ? dicDOInfo["c_gdg"] : string.Empty));
              //cbGudangHdr.Disabled = true;
              Functional.SetComboData(cbGudangHdr, "c_gdg", dicDOInfo.GetValueParser<string>("v_gdgdesc", string.Empty), dicDOInfo.GetValueParser<string>("c_gdg", string.Empty));
              cbGudangHdr.Disabled = true;

              //cbSTTSampleHdr.ToBuilder().AddItem(
              //        dicDOInfo.ContainsKey("c_stno") ? dicDOInfo["c_stno"] : string.Empty,
              //        dicDOInfo.ContainsKey("c_mtno") ? dicDOInfo["c_mtno"] : string.Empty
              //        );
              //if (cbSTTSampleHdr.GetStore() != null)
              //{
              //  cbSTTSampleHdr.GetStore().CommitChanges();
              //}
              //cbSTTSampleHdr.SetValueAndFireSelect((dicDOInfo.ContainsKey("c_stno") ? dicDOInfo["c_stno"] : string.Empty));
              //cbSTTSampleHdr.Disabled = true;
              Functional.SetComboData(cbSTTSampleHdr, "c_stno", dicDOInfo.GetValueParser<string>("c_stno", string.Empty), dicDOInfo.GetValueParser<string>("c_stno", string.Empty));
              cbSTTSampleHdr.Disabled = true;

              //cbViaHdr.ToBuilder().AddItem(
              //        dicDOInfo.ContainsKey("c_via") ? dicDOInfo["c_via"] : string.Empty,
              //        dicDOInfo.ContainsKey("trans") ? dicDOInfo["trans"] : string.Empty
              //        );
              //if (cbViaHdr.GetStore() != null)
              //{
              //  cbViaHdr.GetStore().CommitChanges();
              //}
              //cbViaHdr.SetValueAndFireSelect((dicDOInfo.ContainsKey("c_via") ? dicDOInfo["c_via"] : string.Empty));
              //cbViaHdr.Disabled = true;
              Functional.SetComboData(cbViaHdr, "c_via", dicDOInfo.GetValueParser<string>("trans", string.Empty), dicDOInfo.GetValueParser<string>("c_via", string.Empty));

              txKeterangan.Text = ((dicDOInfo.ContainsKey("v_ket") ? dicDOInfo["v_ket"] : string.Empty));
              txKeterangan.Disabled = false;

              btnPrint.Hidden = false;

              #region Parser Detail

              Ext.Net.Store store = gridDetail.GetStore();

              tmp = string.Format(@"var xOpts = {{
                                  params: {{
                                      start: '0',
                                      limit: '-1',
                                      allQuery: 'true',
                                      model: '0010',
                                      sort: '',
                                      dir: '',
                                      parameters: [['{0} = @0', '{1}', 'System.String']]
                                    }}
                                  }};", pName, pID);

              X.AddScript(tmp);

              hfDONoStt.Text = pID;

              winDetail.ShowModal();
              winDetail.Title = string.Format("Delivery Order - {0}", pID);
              X.AddScript(string.Format("{0}.removeAll();{0}.reload(xOpts);", store.ClientID));

              GC.Collect();

              #endregion
            }
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

  public void Initialize(string storeIDGridMain)
  {
    hfStoreID.Text = storeIDGridMain;
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  protected void OnEvenAddGrid(object sender, DirectEventArgs e)
  {
    //string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);
    
    PopulateDetail(pName, pID, "Add");
    
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

    Dictionary<string, string>[] gridDataDO = JSON.Deserialize<Dictionary<string, string>[]>(jSonGridValues);
    
    PostDataParser.StructureResponse respon = SaveParser(isAdd, NumberID, gridDataDO);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;
        string storeId = hfStoreID.Text;

        string cust = (cbCustomerHdr.SelectedItem != null ? cbCustomerHdr.SelectedItem.Text : string.Empty);
        string Via = (cbViaHdr.SelectedItem != null ? cbViaHdr.SelectedItem.Text : string.Empty);
        string noPl = (cbSTTSampleHdr.SelectedItem != null ? cbSTTSampleHdr.SelectedItem.Text : string.Empty);
        string gudang = (cbGudangHdr.SelectedItem != null ? cbGudangHdr.SelectedItem.Text : string.Empty);

        string dateJs = null;
        DateTime date = DateTime.Today;

        if (isAdd)
        {
          if (respon.Values != null)
          {
            NumberID = respon.Values.GetValueParser<string>("DO", string.Empty);

            if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
            {
              dateJs = Functional.DateToJson(date);
            }

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
                                    ,'c_expno': ''
                                    ,'l_confirm': false }}));{0}.commitChanges();",
                                          storeId, NumberID,
                                          dateJs, gudang, cust, txKeterangan.Text, noPl,
                                          respon.Values.GetValueParser<string>("Pin", string.Empty));

              X.AddScript(scrpt);
            }
          }
        }

        //this.ClearEntrys();
        this.PopulateDetail("c_dono", NumberID, "View");

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

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

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

    rptParse.ReportingID = "10105";

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
      ParameterName = "LG_DOH.c_dono",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_DOH.c_type",
      ParameterValue = "02"
    });

    #endregion

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
      ParameterName = "c_dono = @0",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID),
      IsLinqFilterParameter = true
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "c_type = @0",
      ParameterValue = "02",
      IsLinqFilterParameter = true
    });

    #endregion

    rptParse.PaperID = "8.5x5.5";
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;
    rptParse.UserDefinedName = numberID;

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
          rptResult.OutputFile, "Deliver Order (STT) ", rptResult.Extension);

                wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
                
                PostDataParser.StructureResponse respon = SubmitParser(numberID);
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
