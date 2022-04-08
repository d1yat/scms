using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_Claim_Acc : System.Web.UI.UserControl
{
    //private const string sValStringRadio = "01";

  public void InitializePage(string wndDownload)
  {
    hidWndDown.Text = wndDownload;
    txPeriode1.Text = txPeriode2.Text = DateTime.Now.ToString("dd-MM-yyyy");
  }

  protected void Page_Load(object sender, EventArgs e)
  {
      if (!this.IsPostBack)
      {
          //DateTime date = DateTime.Now;
          //Functional.PopulateTahun(cbTahun, date.Year, 1, 0);
      }
  }

  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    DateTime date1 = DateTime.Today,
      date2 = DateTime.Today;
    List<string> lstData = new List<string>();
    string tmp = null,
      tmp1 = null;
    bool isAsync = false;
    
    isAsync = chkAsync.Checked;

    rptParse.ReportingID = "20410";

    #region Report Parameter

    if (Scms.Web.Common.Functional.DateParser(txPeriode1.RawText.Trim(), "d-M-yyyy", out date1))
    {
      if (Scms.Web.Common.Functional.DateParser(txPeriode2.RawText.Trim(), "d-M-yyyy", out date2))
      {
        if (date2.CompareTo(date1) <= 0)
        {
          date2 = date1;
        }
      }
      else
      {
        date2 = date1;
      }

      lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{LG_ClaimAccH.d_claimaccdate}} >= '{0}' AND {{LG_ClaimAccH.d_claimaccdate}} <= '{1}')", date1, date2),
            IsReportDirectValue = true
        });
    }

    if (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value))
    {
        lstRptParam.Add(new ReportParameter()
          {
              DataType = typeof(string).FullName,
              ParameterName = string.Format("({{LG_ClaimAccH.c_nosup}} = '{0}')", cbSuplier.SelectedItem.Value),
              IsReportDirectValue = true
          });
    }

    if (!string.IsNullOrEmpty(cbDivPrinsipal.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{FA_MsDivPri.c_kddivpri}} = '{0}')", cbDivPrinsipal.SelectedItem.Value),
            IsReportDirectValue = true
        });
    }

    if (!string.IsNullOrEmpty(cbItems.SelectedItem.Value))
    {
        lstRptParam.Add(new ReportParameter()
          {
              DataType = typeof(string).FullName,
              ParameterName = string.Format("({{FA_MasItm.c_iteno}} = '{0}')", cbItems.SelectedItem.Value),
              IsReportDirectValue = true
          });
    }


    lstData.Clear();

    #endregion

    rptParse.IsLandscape = true;
    rptParse.PaperID = "A4";
    rptParse.ReportCustomizeText = null;
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;

    rptParse.OutputReport = ReportParser.ParsingOutputReport((cbRptTypeOutput.SelectedItem != null ? cbRptTypeOutput.SelectedItem.Value : string.Empty));
    //rptParse.IsShared = chkShare.Checked;
    //rptParse.UserDefinedName = txRptUName.Text.Trim();

    string xmlFiles = ReportParser.Deserialize(rptParse);

    SoaReportCaller soa = new SoaReportCaller();

    //string result = "";
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
          string rptName = string.Concat("Report_Claim_Acc_", pag.Nip, ".", rptResult.Extension);

          string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
          tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
            tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

          //wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
          Functional.GeneratorLoadedWindow(hidWndDown.Text, tmpUri, LoadMode.IFrame);
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
