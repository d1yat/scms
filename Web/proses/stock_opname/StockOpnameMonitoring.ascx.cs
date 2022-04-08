/*
 * Created By Indra
 * 20171231FM
 * 
*/

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class proses_stock_opname_StockOpnameMonitoring : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            GetTypeName();
        }
    }

    private void GetTypeName()
    {
        Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

        Dictionary<string, object> dicSP = null;
        Dictionary<string, string> dicSPInfo = null;
        Newtonsoft.Json.Linq.JArray jarr = null;

        string[][] paramX = new string[][]{
        new string[] { "c_notrans = @0", "47", "System.String"},
        new string[] { "c_portal = @0", "3", "System.Char"},
        new string[] { "c_type = @0", "03", "System.String"}
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

                //hfTypeName.Text = dicSPInfo.GetValueParser<string>("v_ket");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
              string.Concat("transaksi_pembelian_SuratPesanan:GetTypeName - ", ex.Message));
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

    public void Initialize(string storeIDGridMain, string NoForm)
    {
        //hfStoreID.Text = storeIDGridMain;
        //txExpired.Text = DateTime.Now.ToString("dd-MM-yyyy");
    }

    public void CommandPopulate(bool isAdd, string pID, string NoForm)
    {
        if (isAdd)
        {
            winDetail3.Hidden = false;
            winDetail3.ShowModal();
            PopulateDetail();
        }
        else
        {

        }
    }

    private void PopulateDetail()
    {
        Dictionary<string, object> dicResult = null;
        Dictionary<string, string> dicResultInfo = null;
        List<Dictionary<string, string>> lstResultInfo = null;
        Newtonsoft.Json.Linq.JArray jarr = null;

        Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();
        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        string[][] paramX = new string[][]{
        new string[] { "gdgAsal", pag.ActiveGudang, "System.String"},
      };

        string res = soa.GlobalQueryService(0, -1, false, string.Empty, string.Empty, "0259", paramX);
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        Ext.Net.Store store = gridMainMonitor.GetStore();
        if (store != null)
        {
            store.RemoveAll();
        }

        sb.AppendFormat("{0}.removeAll(); {0}.commitChanges(); ", gridMainMonitor.GetStore().ClientID);

        string stage = null;

        int Nomor = 0;
        try
        {
            dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
            if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
            {
                jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

                lstResultInfo = JSON.Deserialize<List<Dictionary<string, string>>>(jarr.First.ToString());

                Nomor = lstResultInfo.Count;
                

                for (int nLoop = 0; nLoop < lstResultInfo.Count; nLoop++)
                {
                    dicResultInfo = lstResultInfo[nLoop];
                    stage = dicResultInfo.GetValueParser<string>("stage", string.Empty);

                    if(stage == "4")
                    {
                        sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                        'no': {1},
                        'noform': '{2}',
                        'kdprincipal': '{3}',
                        'principal': '{4}',
                        'kddivprincipal': '{5}',
                        'divprincipal': '{6}',
                        'hitungawal' : (7),
                        'recount1' : (8),
                        'recount2' : (9),
                        'adjustment' : (10),
                        'noadjust' : '{11}'
                        }})); ", gridMainMonitor.GetStore().ClientID,
                                 Nomor,
                                 dicResultInfo.GetValueParser<string>("noform", string.Empty),
                                 dicResultInfo.GetValueParser<string>("kdprincipal", string.Empty),
                                 dicResultInfo.GetValueParser<string>("principal", string.Empty),
                                 dicResultInfo.GetValueParser<string>("kddivprincipal", string.Empty),
                                 dicResultInfo.GetValueParser<string>("divprincipal", string.Empty),
                                 true,
                                 true,
                                 true,
                                 true,
                                 dicResultInfo.GetValueParser<string>("noadj", string.Empty)
                                 );
                    }
                    else if (stage == "3")
                    {
                        sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                        'no': {1},
                        'noform': '{2}',
                        'kdprincipal': '{3}',
                        'principal': '{4}',
                        'kddivprincipal': '{5}',
                        'divprincipal': '{6}',
                        'hitungawal' : (7),
                        'recount1' : (8),
                        'recount2' : (9),
                        'adjustment' : (10)
                        }})); ", gridMainMonitor.GetStore().ClientID,
                                 Nomor,
                                 dicResultInfo.GetValueParser<string>("noform", string.Empty),
                                 dicResultInfo.GetValueParser<string>("kdprincipal", string.Empty),
                                 dicResultInfo.GetValueParser<string>("principal", string.Empty),
                                 dicResultInfo.GetValueParser<string>("kddivprincipal", string.Empty),
                                 dicResultInfo.GetValueParser<string>("divprincipal", string.Empty)
                                 );
                    }
                    else if (stage == "2")
                    {
                        sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                        'no': {1},
                        'noform': '{2}',
                        'kdprincipal': '{3}',
                        'principal': '{4}',
                        'kddivprincipal': '{5}',
                        'divprincipal': '{6}',
                        'hitungawal' : (7),
                        'recount1' : (8),
                        'recount2' : (9)
                        }})); ", gridMainMonitor.GetStore().ClientID,
                                 Nomor,
                                 dicResultInfo.GetValueParser<string>("noform", string.Empty),
                                 dicResultInfo.GetValueParser<string>("kdprincipal", string.Empty),
                                 dicResultInfo.GetValueParser<string>("principal", string.Empty),
                                 dicResultInfo.GetValueParser<string>("kddivprincipal", string.Empty),
                                 dicResultInfo.GetValueParser<string>("divprincipal", string.Empty)
                                 );
                    }
                    else if (stage == "1")
                    {
                        sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                        'no': {1},
                        'noform': '{2}',
                        'kdprincipal': '{3}',
                        'principal': '{4}',
                        'kddivprincipal': '{5}',
                        'divprincipal': '{6}',
                        'hitungawal' : (7),
                        'recount1' : (8)
                        }})); ", gridMainMonitor.GetStore().ClientID,
                                 Nomor,
                                 dicResultInfo.GetValueParser<string>("noform", string.Empty),
                                 dicResultInfo.GetValueParser<string>("kdprincipal", string.Empty),
                                 dicResultInfo.GetValueParser<string>("principal", string.Empty),
                                 dicResultInfo.GetValueParser<string>("kddivprincipal", string.Empty),
                                 dicResultInfo.GetValueParser<string>("divprincipal", string.Empty)
                                 );
                    }
                    else if (stage == "0")
                    {
                        sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                        'no': {1},
                        'noform': '{2}',
                        'kdprincipal': '{3}',
                        'principal': '{4}',
                        'kddivprincipal': '{5}',
                        'divprincipal': '{6}',
                        'hitungawal' : (7)
                        }})); ", gridMainMonitor.GetStore().ClientID,
                                 Nomor,
                                 dicResultInfo.GetValueParser<string>("noform", string.Empty),
                                 dicResultInfo.GetValueParser<string>("kdprincipal", string.Empty),
                                 dicResultInfo.GetValueParser<string>("principal", string.Empty),
                                 dicResultInfo.GetValueParser<string>("kddivprincipal", string.Empty),
                                 dicResultInfo.GetValueParser<string>("divprincipal", string.Empty)
                                 );
                    }

                    dicResultInfo.Clear();

                    Nomor--;

                }

                X.AddScript(sb.ToString());
            }
            else
            {
                Functional.ShowMsgError("Data atau No. Form Tidak ditemukan.");

                return;
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

    #region Printing Stock Opname

    [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void btnMonitoringSO_OnClick(object sender, DirectEventArgs e)
    {
        string tipereport = (e.ExtraParams["NumberID"] ?? string.Empty);
        string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
        Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        PostDataParser parser = new PostDataParser();
        IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

        Dictionary<string, string> dicData = null;

        PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

        Dictionary<string, string> dicAttr = null;

        Dictionary<string, string>[] dics = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

        pair.IsSet = true;
        pair.IsList = true;
        pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
        pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (!Functional.CanCreateGenerateReport(pag))
        {
            return;
        }

        ReportParser rptParse = new ReportParser();

        List<ReportParameter> lstRptParam = new List<ReportParameter>();
        List<ReportCustomizeText> lstCustTxt = new List<ReportCustomizeText>();

        DateTime date1 = DateTime.Today,
          date2 = DateTime.Today;
        List<string> lstData = new List<string>();
        //string tmp = null;
        bool isAsync = false;

        #region Sql Parameter

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(DateTime).FullName,
            ParameterName = "Gudang",
            IsSqlParameter = true,
            ParameterValue = pag.ActiveGudang
        });

        #endregion

        rptParse.ReportingID = "0258-b";

        isAsync = false;        

        rptParse.PaperID = "A3";
        rptParse.IsLandscape = true;
        rptParse.ReportCustomizeText = lstCustTxt.ToArray();
        rptParse.ReportParameter = lstRptParam.ToArray();
        rptParse.User = pag.Nip;

        rptParse.OutputReport = ReportParser.ParsingOutputReport("03");

        string xmlFiles = ReportParser.Deserialize(rptParse);

        SoaReportCaller soa = new SoaReportCaller();

        string result = soa.GeneratorReport(isAsync, xmlFiles);

        ReportingResult rptResult = ReportingResult.Serialize(result);

        if (rptResult == null)
        {
            Functional.ShowMsgError("Pembuatan report gagal.");
        }
        else
        {
            if (rptResult.IsSuccess)
            {
                string rptName = string.Concat("Report_Stock_Opname_", pag.Nip, ".", rptResult.Extension);

                string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
                tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
                  tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

                //wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
                Functional.GeneratorLoadedWindow("ctl00_cphContent_wndDown", tmpUri, LoadMode.IFrame);
            }
            else
            {
                Functional.ShowMsgWarning(rptResult.MessageResponse);
            }
        }

        GC.Collect();

    }

    #endregion
}
