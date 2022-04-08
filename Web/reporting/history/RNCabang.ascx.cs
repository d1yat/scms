using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_history_RNCabang : System.Web.UI.UserControl
{
  public void InitializePage(string wndDownload)
  {
    hidWndDown.Text = wndDownload;
  }

  protected void Page_Load(object sender, EventArgs e)
  {

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
    string stype = null;
    bool isAsync = false;

    rptParse.ReportingID = "20028";

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

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "gdg",
        IsSqlParameter = true,
        ParameterValue = string.IsNullOrEmpty(cbGudang.SelectedItem.Value) ? "0" : cbGudang.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "cusno",
        IsSqlParameter = true,
        ParameterValue = string.IsNullOrEmpty(cbCustomer.SelectedItem.Value) ? "0000" : cbCustomer.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(char).FullName,
        ParameterName = "user",
        IsSqlParameter = true,
        ParameterValue = pag.Nip
    });

    //lstRptParam.Add(new ReportParameter()
    //{
        //DataType = typeof(char).FullName,
        //ParameterName = "phar",
        //IsSqlParameter = true,
        //ParameterValue = chkPhar.Checked ? "1" : "0"
    //});


    // coba hafizh

    //string c_entry = null;
    //if (SelectBoxTipePharmanet.SelectedItem.Value == "01")
    //{
    //    c_entry = "spphar1";
    //}
    //else if (SelectBoxTipePharmanet.SelectedItem.Value == "02")
    //{
    //    c_entry = "spphar2";
    //}

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(char).FullName,
        ParameterName = "c_entry",
        IsSqlParameter = true,
        ParameterValue = SelectBoxTipePharmanet.SelectedItem.Value
    });

    #endregion

    #region Report Parameter

    if (rdNonRN.Checked)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "IsNull({LG_tempHistoryRNCabang.c_rnno}) = true",
            IsReportDirectValue = true
        });
    }
    else if (rdRN.Checked)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "IsNull({LG_tempHistoryRNCabang.c_rnno}) = false",
            IsReportDirectValue = true
        });
    }

    if (txDO1.Text.Length > 0)
    {
        if (txDO2.Text.Length > 0)
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("({{LG_tempHistoryRNCabang.c_dono}} IN ['{0}','{1}'])", txDO1.Text.Trim(), txDO2.Text.Trim()),
                IsReportDirectValue = true
            });
        }
        else
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("({{LG_tempHistoryRNCabang.c_dono}} >= '{0}')", txDO1.Text.Trim()),
                IsReportDirectValue = true
            });
        }
    }
    else if (txDO2.Text.Length > 0)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{LG_tempHistoryRNCabang.c_dono}} <= '{0}')", txDO2.Text.Trim()),
            IsReportDirectValue = true
        });
    }

    #endregion

    rptParse.PaperID = "17.5*11";
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;
    rptParse.IsLandscape = false;

    //rptParse.ReportCustomizeText = new ReportCustomizeText[] {
    //  new ReportCustomizeText() {
    //     SectionName = "Section1",
    //      ControlName = "txtPeriode",
    //       Value = string.Format("Periode : {0} s/d {1}", date1.ToString("dd-MMM-yyyy"), date2.ToString("dd-MMM-yyyy"))
    //  }
    //};

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
          string rptName = string.Concat("History_RN_Cabang", pag.Nip, ".", rptResult.Extension);

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
