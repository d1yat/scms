using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_konsolidasi_WPSuplier : System.Web.UI.UserControl
{
  public void InitializePage(string wndDownload)
  {
    hidWndDown.Text = wndDownload;
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }

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
    List<ReportCustomizeText> lstCustTxt = new List<ReportCustomizeText>();
    bool isAsync = false;

    isAsync = chkAsync.Checked;

    #region Sql Parameter

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
        DataType = typeof(DateTime).FullName,
        ParameterName = "date1",
        IsSqlParameter = true,
        ParameterValue = date1.ToString("d-M-yyyy")
      });
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(DateTime).FullName,
        ParameterName = "date2",
        IsSqlParameter = true,
        ParameterValue = date2.ToString("d-M-yyyy")
      });
    }
    
    lstData = rbgTipeTransaksi.CheckedTags;
    if (lstData.Count > 0)
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "tipeTrx",
        IsSqlParameter = true,
        ParameterValue = lstData[0]
      });
    }
    else
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "tipeTrx",
        IsSqlParameter = true,
        ParameterValue = "??"
      });
    }

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "gdg",
      IsSqlParameter = true,
      ParameterValue = (cbGudang.SelectedItem != null ? cbGudang.SelectedItem.Value : "0")
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "suplier",
      IsSqlParameter = true,
      ParameterValue = (cbSuplier.SelectedItem != null ? cbSuplier.SelectedItem.Value : "00000")
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "divSuplier",
      IsSqlParameter = true,
      ParameterValue = (cbSuplier.SelectedItem != null ? cbSuplier.SelectedItem.Value : "000")
    });
    
    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "item",
      IsSqlParameter = true,
      ParameterValue = (cbItem.SelectedItem != null ? cbItem.SelectedItem.Value : "0000")
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "session",
      IsSqlParameter = true,
      ParameterValue = pag.Nip
    });

    #endregion

    #region Report Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "Temp_LGWP_supplier.c_user",
      ParameterValue = pag.Nip
    });

    lstData = rbgTipeReport.CheckedTags;

    if (lstData.Count > 0)
    {
      if (lstData.Contains("01"))
      {
        rptParse.ReportingID = "20202-1";
      }
      else if (lstData.Contains("02"))
      {
        rptParse.ReportingID = "20202-2";
      }
      else if (lstData.Contains("03"))
      {
        rptParse.ReportingID = "20202-3";
      }
    }

    lstData.Clear();

    #endregion

    rptParse.PaperID = "Letter";
    rptParse.ReportCustomizeText = lstCustTxt.ToArray();
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
          string rptName = string.Concat("Konsolidasi_BDP_", pag.Nip, ".", rptResult.Extension);

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
