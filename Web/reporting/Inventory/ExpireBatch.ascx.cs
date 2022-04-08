using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_Inventory_ExpireBatch : System.Web.UI.UserControl
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

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    DateTime date1 = DateTime.Today,
      date2 = DateTime.Today;
    List<ReportCustomizeText> lstCustom = new List<ReportCustomizeText>();
    bool isAsync = false;
    string tmp = null;

    isAsync = chkAsync.Checked;

    #region Report Parameter

    tmp = Functional.GetCheckedRadioData(rdgTipeReport);

    rptParse.IsLandscape = false;
    rptParse.ReportingID = "10008";

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "lg_tempexpirebatch.c_user",
        ParameterValue = pag.Nip
    });

    //if (tmp.Contains("02"))
    //{
    //    lstRptParam.Add(new ReportParameter()
    //      {
    //          DataType = typeof(string).FullName,
    //          ParameterName = string.Format("({{lg_msbatch.d_expired}} <= '{0}')", DateTime.Today.ToString("yyyy-MM-dd")),
    //          IsSqlParameter = true
    //      });
    //}
    //else if (tmp.Contains("03"))
    //{
    //    lstRptParam.Add(new ReportParameter()
    //    {
    //        DataType = typeof(string).FullName,
    //        ParameterName = string.Format("({{lg_msbatch.d_expired}} > '{0}')", DateTime.Today.ToString("yyyy-MM-dd")),
    //        IsReportDirectValue = true
    //    });
    //}

  lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(string).FullName,
          ParameterName = "expireid",
          ParameterValue = tmp,
          IsSqlParameter = true
      });

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
        ParameterName = "iteno",
        IsSqlParameter = true,
        ParameterValue = cbItems.SelectedItem.Value == null ? "0000" : cbItems.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(char).FullName,
        ParameterName = "session",
        IsSqlParameter = true,
        ParameterValue = pag.Nip
    });
    
    #endregion

    rptParse.PaperID = "A4";
    rptParse.ReportCustomizeText = lstCustom.ToArray();
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
          string rptName = string.Concat("Expire_Batch_", pag.Nip, ".", rptResult.Extension);

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
