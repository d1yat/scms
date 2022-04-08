using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using System.Text;

public partial class proses_stock_opname_ProsesStockOpname : Scms.Web.Core.PageHandler
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //txPeriode1.Text = DateTime.Now.ToString("dd-MM-yyyy");
        if (!this.IsPostBack)
        {
            DateTime date = DateTime.Now;

            Functional.PopulateBulan(cbBulan, date.Month);

            Functional.PopulateTahun(cbTahun, date.Year, 2, 0);

            //cbBulan.Disabled = true;
            //cbTahun.Disabled = true;
        }
    }

    protected void ProsesSO_OnGenerate(object sender, DirectEventArgs e)
    {
        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

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
        bool isAsync = false;

        string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
        string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);

        bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);
        
        Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

        PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataPL);

        if (respon.IsSet)
        {
            if (respon.Response == PostDataParser.ResponseStatus.Success)
            {
                DateTime TglFrezzeSO = DateTime.Today;

                #region Sql Parameter

                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(DateTime).FullName,
                    ParameterName = "date1",
                    IsSqlParameter = true,
                    ParameterValue = TglFrezzeSO.ToString("d-M-yyyy")

                });

                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(char).FullName,
                    ParameterName = "user",
                    IsSqlParameter = true,
                    ParameterValue = pag.Nip
                });

                #endregion

                rptParse.ReportingID = "20214";
                rptParse.ReportCustomizeText = lstCustTxt.ToArray();
                rptParse.ReportParameter = lstRptParam.ToArray();

                rptParse.PaperID = "A4";
                rptParse.IsLandscape = false;
                rptParse.User = pag.Nip;

                rptParse.OutputReport = ReportParser.ParsingOutputReport(("03" != null ? "03" : string.Empty));
                string xmlFiles = ReportParser.Deserialize(rptParse);

                SoaReportCaller soa = new SoaReportCaller();

                string result = soa.GeneratorReport(isAsync, xmlFiles);

                ReportingResult rptResult = ReportingResult.Serialize(result);

                if (string.IsNullOrEmpty(result))
                {
                    e.ErrorMessage = "Unknown response";

                    e.Success = false;
                }
                else
                {
                    Functional.ShowMsgInformation("Proses Sedang dijalankan, harap tunggu 3 menit sebelum Adjustment Stock Opname");
                    
                    string rptName = string.Concat("Frezze_Stock_", pag.Nip, ".", rptResult.Extension);

                    string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
                    tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
                      tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

                    //Functional.GeneratorLoadedWindow(hidWndDown.Text, tmpUri, LoadMode.IFrame);
                }

                GC.Collect();
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

        string tmp = null, varData = null;

        string Tahun = null, Bulan = null ;
        Tahun = cbTahun.Text;
        Bulan = cbBulan.Text;

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        DateTime date = DateTime.Today;

        dic.Add("ID", pair);

        pair.DicAttributeValues.Add("Entry", pag.Nip);
        pair.DicAttributeValues.Add("Tahun", Tahun);
        pair.DicAttributeValues.Add("Bulan", Bulan);

        try
        {
            varData = parser.ParserData("ProcessStockOpname", (isAdd ? "Add" : "Modify"), dic);
        }
        catch (Exception ex)
        {
            Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_SuratPesananCtrl SaveParser : {0} ", ex.Message);
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
}
