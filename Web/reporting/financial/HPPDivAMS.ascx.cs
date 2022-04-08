using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_financial_HPPDivAMS : System.Web.UI.UserControl
{
  public void InitializePage(string wndDownload)
  {
    hidWndDown.Text = wndDownload;
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      DateTime date = DateTime.Now;

      //cbPeriode1.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
      //date = date.AddYears(-1);
      //cbPeriode1.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
      //date = date.AddYears(-1);
      //cbPeriode1.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
      //cbPeriode1.SelectedIndex = 0;

      //Functional.PopulateBulan(cbPeriode2, date.Month);
      
      Functional.PopulateBulan(cbBulan, date.Month);

      Functional.PopulateTahun(cbTahun, date.Year, 1, 0);
    }
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
    //List<string> lstData = null;
    string tmp = null;
    bool isAsync = false, isCheck = false;

    isAsync = chkAsync.Checked;
    isCheck = chkClaim.Checked;

    tmp = Functional.GetCheckedRadioData(rdgTipe);

    if (tmp.Equals("det"))
    {
      if (isCheck == false)
      {
        rptParse.ReportingID = "20404-1-1";
      }
      else
      {
        rptParse.ReportingID = "20404-1-2";
      }
    }
    else if (tmp.Equals("sum"))
    {
      if (isCheck == false)
      {
        rptParse.ReportingID = "20404-2-1";
      }
      else
      {
        rptParse.ReportingID = "20404-2-2";
      }
    }
    else if (tmp.Equals("srt"))
    {
      rptParse.ReportingID = "20404-3";
    }

    #region Sql Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(int).FullName,
      ParameterName = "LG_HPP.t_bulan",
      ParameterValue = cbBulan.Value.ToString()
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(int).FullName,
      ParameterName = "LG_HPP.s_tahun",
      ParameterValue = cbTahun.Value.ToString()
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_HPP.c_type",
      ParameterValue = Functional.GetCheckedRadioData(rdgJenis)
    });

    if (!string.IsNullOrEmpty(cbDivAms.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "LG_HPP.c_kddivams",
        ParameterValue = cbDivAms.SelectedItem.Value
      });
    }

    if (!string.IsNullOrEmpty(cbItems.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "LG_HPP.c_iteno",
        ParameterValue = cbItems.SelectedItem.Value
      });
    }

    #endregion

    rptParse.PaperID = "A2";
    rptParse.IsLandscape = true;
    rptParse.ReportCustomizeText = null;
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;

    rptParse.OutputReport = ReportParser.ParsingOutputReport((cbRptTypeOutput.SelectedItem != null ? cbRptTypeOutput.SelectedItem.Value : string.Empty));
    rptParse.IsShared = chkShare.Checked;
    rptParse.UserDefinedName = txRptUName.Text.Trim();

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
          string rptName = string.Concat("Financial_HPP_DIV_AMS_", pag.Nip, ".", rptResult.Extension);

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
