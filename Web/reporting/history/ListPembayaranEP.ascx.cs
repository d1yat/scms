using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_history_ListPembayaranEP : System.Web.UI.UserControl
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
    string tmp = null;
    bool isAsync = false;

    rptParse.ReportingID = "20026";

    isAsync = chkAsync.Checked;

    #region Sql Parameter
    #endregion

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
            DataType = typeof(DateTime).FullName,
            ParameterName = string.Format("Date({{lg_newekspedisi.tanggal_Ekspedisi}}) IN DateValue ('{0}') TO DateValue ('{1}')", date1.ToString("dd-MMM-yyyy"), date2.ToString("dd-MMM-yyyy")),
            IsReportDirectValue = true
        });
        
    }

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "lg_newekspedisi.exp",
        ParameterValue = cbExpedisi.SelectedItem.Value
    });

    if (!string.IsNullOrEmpty(txNoFak1.Text))
    {
        if (string.IsNullOrEmpty(txNoFak2.Text))
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = "lg_newekspedisi.Nomor_Ekspedisi_AMS",
                ParameterValue = (string.IsNullOrEmpty(txNoFak1.Text) ? string.Empty : txNoFak1.Text)
            });
        }
        else
        {
            if (txNoFak1.Text.CompareTo(txNoFak2.Text) >= 0)
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "lg_newekspedisi.Nomor_Ekspedisi_AMS",
                    ParameterValue = (string.IsNullOrEmpty(txNoFak1.Text) ? string.Empty : txNoFak1.Text)
                });
            }
            else
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = string.Format("({{lg_newekspedisi.Nomor_Ekspedisi_AMS}} IN ('{0}' TO '{1}'))", txNoFak1.Text, txNoFak2.Text),
                    IsReportDirectValue = true
                });
            }
        }
    }
         

    #endregion

    rptParse.PaperID = "17.5*11";
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;

    rptParse.ReportCustomizeText = new ReportCustomizeText[] {
      new ReportCustomizeText() {
         SectionName = "Section1",
          ControlName = "txtPeriode",
           Value = string.Format("Periode : {0} s/d {1}", date1.ToString("dd-MMM-yyyy"), date2.ToString("dd-MMM-yyyy"))
      }
    };

    rptParse.OutputReport = ReportParser.ParsingOutputReport((cbRptTypeOutput.SelectedItem != null ? cbRptTypeOutput.SelectedItem.Value : string.Empty));
    rptParse.IsShared = chkShare.Checked;
    rptParse.UserDefinedName = txRptUName.Text.Trim();

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
          string rptName = string.Concat("History_Biaya_Ekspedisi", pag.Nip, ".", rptResult.Extension);

          string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
          tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
            tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

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
