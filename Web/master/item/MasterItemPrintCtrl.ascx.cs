using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Ext.Net;

public partial class master_item_MasterItemPrintCtrl : System.Web.UI.UserControl
{
  protected void Page_Load(object sender, EventArgs e)
  {

  }

  public void ShowPrintPage()
  {
    winPrintDetail.Title = "Cetak Master Item";

    winPrintDetail.Hidden = false;
    winPrintDetail.ShowModal();
  }

  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    if (!pag.IsAllowPrinting)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
      return;
    }

    string supId = (e.ExtraParams["SuplierID"] ?? string.Empty);
    string tmp = (e.ExtraParams["Async"] ?? string.Empty);
    string outputRpt = (e.ExtraParams["outputRpt"] ?? string.Empty);

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();
    bool isAsync = false;

    bool.TryParse(tmp, out isAsync);

    rptParse.ReportingID = "10030";

    #region Report Parameter

    if (!string.IsNullOrEmpty(supId))
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "fa_masitm.c_nosup",
            ParameterValue = supId
        });
    }

    #endregion

    rptParse.PaperID = "A4";
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;
    rptParse.OutputReport = ReportParser.ParsingOutputReport(outputRpt);

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
        if (isAsync)
        {
          Functional.ShowMsgInformation("Report sedang dibuat, silahkan cek di halaman report beberapa saat lagi.");
        }
        else
        {
          //string rptName = string.Concat("Packing_List_", pag.Nip, ".", rptResult.Extension);

          //string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
          //tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
          //  tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

          string tmpUri = Functional.UriDownloadGenerator(pag,
            rptResult.OutputFile, "Report Master Item ", rptResult.Extension);

          wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
        }
      }
      else
      {
        Functional.ShowMsgWarning(rptResult.MessageResponse);
      }
    }

    GC.Collect();
  }
}
