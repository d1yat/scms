using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_pending_POPeriodik : System.Web.UI.UserControl
{
  public void InitializePage(string wndDownload)
  {
    hidWndDown.Text = wndDownload;
    txPeriode2.Text = DateTime.Now.ToString("dd-MM-yyyy");
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    DateTime date1 = DateTime.Today,
      date2 = DateTime.Today;
    string tmp = null;
    bool isAsync = false;
    
    isAsync = chkAsync.Checked;

    rptParse.ReportingID = "20314";


    #region Sql Parameter

    //if (Scms.Web.Common.Functional.DateParser(txPeriode1.RawText.Trim(), "d-M-yyyy", out date1))
    //{
    //    if (Scms.Web.Common.Functional.DateParser(txPeriode2.RawText.Trim(), "d-M-yyyy", out date2))
    //    {
    //        if (date2.CompareTo(date1) <= 0)
    //        {
    //            date2 = date1;
    //        }
    //    }
    //    else
    //    {
    //        date2 = date1;
    //    }

    //    lstRptParam.Add(new ReportParameter()
    //    {
    //        DataType = typeof(DateTime).FullName,
    //        ParameterName = "date1",
    //        IsSqlParameter = true,
    //        ParameterValue = date1.ToString("d-M-yyyy")
    //    });
    //    lstRptParam.Add(new ReportParameter()
    //    {
    //        DataType = typeof(DateTime).FullName,
    //        ParameterName = "date2",
    //        IsSqlParameter = true,
    //        ParameterValue = date2.ToString("d-M-yyyy")
    //    });
    //}


    if (Scms.Web.Common.Functional.DateParser(txPeriode2.RawText.Trim(), "d-M-yyyy", out date2))
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(DateTime).FullName,
            ParameterName = "date2",
            IsSqlParameter = true,
            ParameterValue = date2.ToString("d-M-yyyy")
        });
    }
      
    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "session",
        IsSqlParameter = true,
        ParameterValue = pag.Nip
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "principal",
        IsSqlParameter = true,
        ParameterValue = cbSuplier.SelectedItem.Value == null ? "00000" : cbSuplier.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "divprincipal",
        IsSqlParameter = true,
        ParameterValue = cbDivPrinsipal.SelectedItem.Value == null ? "000" : cbDivPrinsipal.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "divams",
        IsSqlParameter = true,
        ParameterValue = cbDivAms.SelectedItem.Value == null ? "000" : cbDivAms.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "iteno",
        IsSqlParameter = true,
        ParameterValue = cbItems.SelectedItem.Value == null ? "0000" : cbItems.SelectedItem.Value
    });

    #endregion

    rptParse.PaperID = "A3";
    rptParse.IsLandscape = true;   
    rptParse.ReportCustomizeText = new ReportCustomizeText[] {
      new ReportCustomizeText()
      {
        SectionName = "Section2",
        ControlName = "txtPeriode",
        Value = string.Format("Periode PO : {0} s/d {1}", "01-Jan-2014", date2.ToString("dd-MMM-yyyy"))
      }};
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
          string rptName = string.Concat("Pending_PO_FA_", pag.Nip, ".", rptResult.Extension);

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
