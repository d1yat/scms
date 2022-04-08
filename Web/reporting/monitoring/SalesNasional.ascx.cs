using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_salesnasional : System.Web.UI.UserControl
{
  public void InitializePage(string wndDownload)
  {
    hidWndDown.Text = wndDownload;
  }

  protected void Page_Load(object sender, EventArgs e)
  {
      DateTime date = DateTime.Now;

      cbPeriode1.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
      date = date.AddYears(-1);
      cbPeriode1.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
      date = date.AddYears(-1);
      cbPeriode1.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
      cbPeriode1.SelectedIndex = 0;

      Functional.PopulateBulan(cbPeriode2, date.Month);
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    if (!Functional.CanCreateGenerateReport(pag))
    {
      return;
    }

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    DateTime date1 = DateTime.Today,
      date2 = DateTime.Today;
    List<string> lstData = new List<string>();
    bool isAsync = false;

    rptParse.ReportingID = "21053";

    isAsync = chkAsync.Checked;

    #region Sql Parameter

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(DateTime).FullName,
        ParameterName = "tahun",
        IsSqlParameter = true,
        ParameterValue = cbPeriode1.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(DateTime).FullName,
        ParameterName = "bulan",
        IsSqlParameter = true,
        ParameterValue = cbPeriode2.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(DateTime).FullName,
        ParameterName = "nosup",
        IsSqlParameter = true,
        ParameterValue = string.IsNullOrEmpty(cbSuplier.SelectedItem.Value) ? "00000" : cbSuplier.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "user",
      IsSqlParameter = true,
      ParameterValue = pag.Nip
    });

    #endregion

    #region Report Parameter

    //lstRptParam.Add(new ReportParameter()
    //{
    //    DataType = typeof(string).FullName,
    //    ParameterName = string.Format("(Month(CDate({{LG_SPH.d_spdate}})) = {0})", cbPeriode2.SelectedItem.Value),
    //    IsReportDirectValue = true
    //});

    //lstRptParam.Add(new ReportParameter()
    //{
    //    DataType = typeof(string).FullName,
    //    ParameterName = "cusnosup",
    //    IsSqlParameter = true,
    //    ParameterValue = string.IsNullOrEmpty(cbCustomer.SelectedItem.Value) ? "0000" : cbCustomer.SelectedItem.Value
    //});

    #endregion

    rptParse.PaperID = "A3";
    rptParse.IsLandscape = true;
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;

    //rptParse.ReportCustomizeText = new ReportCustomizeText[] {
    //  new ReportCustomizeText() {
    //     SectionName = "Section2",
    //      ControlName = "txtPeriode",
    //       Value = string.Format("Periode : {0} s/d {1}", date1.ToString("dd-MMM-yyyy"), date2.ToString("dd-MMM-yyyy"))
    //  }
    //};

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
          string rptName = string.Concat("Report_SalesNasional_", cbPeriode2.SelectedItem.Text, cbPeriode1.SelectedItem.Text, ".", rptResult.Extension);

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
