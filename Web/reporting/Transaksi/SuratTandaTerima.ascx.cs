using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_Transaksi_SuratTandaTerima : System.Web.UI.UserControl
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

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    DateTime date1 = DateTime.Today,
      date2 = DateTime.Today;
    List<string> lstData = new List<string>();
    string tmp = null,
      tmp1 = null;
    bool isAsync = false;

    rptParse.ReportingID = "21005";

    isAsync = chkAsync.Checked;

    tmp = txSTT1.Text.Trim();
    tmp1 = txSTT2.Text.Trim();

    #region Report Parameter

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(DateTime).FullName,
        ParameterName = "gdg",
        IsSqlParameter = true,
        ParameterValue = cbGudang.SelectedItem.Value == null ? "00" : cbGudang.SelectedItem.Value
    });

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

    if (txSTT1.Text.Length > 0)
    {
        if (txSTT2.Text.Length > 0)
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("({{temp_rptliststt.c_stno}} IN ['{0}','{1}'])", txSTT1.Text.Trim(), txSTT2.Text.Trim()),
                IsReportDirectValue = true
            });
        }
        else
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("({{temp_rptliststt.c_stno}} >= '{0}')", txSTT1.Text.Trim()),
                IsReportDirectValue = true
            });
        }
    }
    else if (txSTT2.Text.Length > 0)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{temp_rptliststt.c_stno}} <= '{0}')", txSTT2.Text.Trim()),
            IsReportDirectValue = true
        });
    }

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(char).FullName,
        ParameterName = "nosup",
        IsSqlParameter = true,
        ParameterValue = cbSuplier.SelectedItem.Value == null ? "00000" : cbSuplier.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(char).FullName,
        ParameterName = "divpri",
        IsSqlParameter = true,
        ParameterValue = cbDivPrinsipal.SelectedItem.Value == null ? "000" : cbDivPrinsipal.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(char).FullName,
        ParameterName = "iteno",
        IsSqlParameter = true,
        ParameterValue = cbItems.SelectedItem.Value == null ? "0000" : cbItems.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(char).FullName,
        ParameterName = "user",
        IsSqlParameter = true,
        ParameterValue = pag.Nip
    });

    if (rdgTRAll.Checked)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(char).FullName,
            ParameterName = "type",
            IsSqlParameter = true,
            ParameterValue = "00"
        });
    }
      else
    {
        if (rdgTRDonasi.Checked)
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(char).FullName,
                ParameterName = "type",
                IsSqlParameter = true,
                ParameterValue = "01"
            });
        }
        else if (rdgTRSample.Checked)
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(char).FullName,
                ParameterName = "type",
                IsSqlParameter = true,
                ParameterValue = "02"
            });
        }
    }
    #endregion

    rptParse.PaperID = "Letter";
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
          string rptName = string.Concat("Transaksi_STT", pag.Nip, ".", rptResult.Extension);

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
