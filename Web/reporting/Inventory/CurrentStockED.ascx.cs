using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_Inventory_CurrentStockED : System.Web.UI.UserControl
{
  public void InitializePage(string wndDownload)
  {
    txPeriode2.Text = DateTime.Now.AddYears(1).ToString("dd-MM-yyyy");
    chkAsync.Checked = false;
    hidWndDown.Text = wndDownload;
    Functional.SetComboData(cbGudang, "c_gdg", "Gudang 1", "1");
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    //if (!this.IsPostBack)
    //{
    //  InitializeData();
    //}
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
    List<ReportCustomizeText> lstCustTxt = new List<ReportCustomizeText>();

    DateTime date1 = DateTime.Today,
      date2 = DateTime.Today;
    List<string> lstData = new List<string>();
    //string tmp = null;
    bool isAsync = false;

    rptParse.ReportingID = "10010";

    isAsync = chkAsync.Checked;

    #region Report Parameter

    if (Scms.Web.Common.Functional.DateParser(txPeriode2.RawText.Trim(), "d-M-yyyy", out date1))
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("{{LG_vwStock_ED.d_expired}} <= ('{0}')", date1.ToString("yyyy-MM-dd")),
            IsReportDirectValue = true
        });
    }

    if (!string.IsNullOrEmpty(cbGudang.SelectedItem.Value))
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(char).FullName,
            ParameterName = "LG_vwStock_ED.c_gdg",
            ParameterValue = cbGudang.SelectedItem.Value
        });
    }

    if (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value))
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "LG_vwStock_ED.c_nosup",
            ParameterValue = cbSuplier.SelectedItem.Value
        });
    }

    if (!string.IsNullOrEmpty(cbItems.SelectedItem.Value))
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "LG_vwStock_ED.c_iteno",
            ParameterValue = cbItems.SelectedItem.Value
        });
    }

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = string.Format("{{LG_vwStock_ED.n_gsisa}} > 0"),
        IsReportDirectValue = true
    });

    lstCustTxt.Add(new ReportCustomizeText()
    {
        SectionName = "Section1",
        ControlName = "txtPeriode",
        Value = string.Format("ED dibawah tanggal : {0}", date1.ToString("dd-MMM-yyyy"))
    });


    #endregion

    rptParse.PaperID = "A4";
    rptParse.ReportCustomizeText = lstCustTxt.ToArray();
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.IsLandscape = false;
    rptParse.User = pag.Nip;

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
          string rptName = string.Concat("Inventory_CurrentStock_ED_", pag.Nip, ".", rptResult.Extension);

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
