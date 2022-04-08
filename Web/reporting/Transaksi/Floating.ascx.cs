using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_Transaksi_Floating : System.Web.UI.UserControl
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
    string tmp = null,
      tmp1 = null;
    bool isAsync = false;

    rptParse.ReportingID = "21014";

    isAsync = chkAsync.Checked;

    tmp = txFloat1.Text.Trim();
    tmp1 = txFloat2.Text.Trim();

    #region Dateset Parameter

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
      if ((!date1.Equals(DateTime.MinValue)) && (!date1.Equals(DateTime.MinValue)) && (!date1.Equals(date2)))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(DateTime).FullName,
          ParameterName = "date1",
          IsDatasetParameter = true,
          ParameterValue = date1.ToString("yyyy-MM-dd")
        });
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(DateTime).FullName,
          ParameterName = "date2",
          IsDatasetParameter = true,
          ParameterValue = date1.ToString("yyyy-MM-dd")
        });
      }
      else if (!date1.Equals(DateTime.MinValue))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(DateTime).FullName,
          ParameterName = "date1",
          IsDatasetParameter = true,
          ParameterValue = date1.ToString("yyyy-MM-dd")
        });
      }
    }
    if (txFloat1.Text.Length > 0)
    {
      if (txFloat2.Text.Length > 0)
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = string.Format("({{DSNG_SCMS_FLOATING.c_rnno}} IN ['{0}','{1}'])", txFloat1.Text, txFloat2.Text),
          IsReportDirectValue = true
        });
      }
      else
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = string.Format("({{DSNG_SCMS_FLOATING.c_rnno}} >= '{0}')", txFloat1.Text),
          IsReportDirectValue = true
        });
      }
    }
    else if (txFloat2.Text.Length > 0)
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = string.Format("({{DSNG_SCMS_FLOATING.c_rnno}} <= '{0}')", txFloat2.Text),
        IsReportDirectValue = true
      });
    }

    if (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "supplier",
        ParameterValue = (cbSuplier.SelectedItem.Value.ToString() == null ? string.Empty : cbSuplier.SelectedItem.Value.ToString()),
        IsDatasetParameter = true,
        IsInValue = true
      });
    }
    if (!string.IsNullOrEmpty(cbDivPrinsipal.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "divprin",
        ParameterValue = (cbDivPrinsipal.SelectedItem.Value.ToString() == null ? string.Empty : cbDivPrinsipal.SelectedItem.Value.ToString()),
        IsDatasetParameter = true,
        IsInValue = true
      });
    }
    if (!string.IsNullOrEmpty(cbItems.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "item",
        ParameterValue = (cbItems.SelectedItem.Value.ToString() == null ? string.Empty : cbItems.SelectedItem.Value.ToString()),
        IsDatasetParameter = true,
        IsInValue = true
      });
    }

    #region Old Coded

    //if (cbSuplier.SelectedItems.Count > 1)
    //{
    //  for (int nLoop = 0, nlen = cbSuplier.SelectedItems.Count; nLoop < nlen; nLoop++)
    //  {
    //    tmp = cbSuplier.SelectedItems[nLoop].Value;
    //    if (!string.IsNullOrEmpty(tmp))
    //    {
    //      //lstData.Add(string.Concat("'", tmp, "'"));
    //      lstData.Add(tmp);
    //    }
    //  }

    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string[]).FullName,
    //    ParameterName = "supplier",
    //    ParameterValueArray = lstData.ToArray(),
    //    IsDatasetParameter = true,
    //    IsInValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbSuplier.SelectedItems.Count == 1)
    //{
    //  lstData.Add((cbSuplier.SelectedItems[0].Value == null ? string.Empty : cbSuplier.SelectedItems[0].Value));

    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string[]).FullName,
    //    ParameterName = "supplier",
    //    ParameterValueArray = lstData.ToArray(),
    //    IsDatasetParameter = true,
    //    IsInValue = true
    //  });

    //  lstData.Clear();
    //}
    //if (cbDivPrinsipal.SelectedItems.Count > 1)
    //{
    //  for (int nLoop = 0, nlen = cbSuplier.SelectedItems.Count; nLoop < nlen; nLoop++)
    //  {
    //    tmp = cbDivPrinsipal.SelectedItems[nLoop].Value;
    //    if (!string.IsNullOrEmpty(tmp))
    //    {
    //      //lstData.Add(string.Concat("'", tmp, "'"));
    //      lstData.Add(tmp);
    //    }
    //  }

    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string[]).FullName,
    //    ParameterName = "divprin",
    //    ParameterValueArray = lstData.ToArray(),
    //    IsDatasetParameter = true,
    //    IsInValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbDivPrinsipal.SelectedItems.Count == 1)
    //{
    //  lstData.Add((cbDivPrinsipal.SelectedItems[0].Value == null ? string.Empty : cbDivPrinsipal.SelectedItems[0].Value));

    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string[]).FullName,
    //    ParameterName = "divprin",
    //    ParameterValueArray = lstData.ToArray(),
    //    IsDatasetParameter = true,
    //    IsInValue = true
    //  });

    //  lstData.Clear();
    //}
    //if (cbItems.SelectedItems.Count > 1)
    //{
    //  for (int nLoop = 0, nlen = cbSuplier.SelectedItems.Count; nLoop < nlen; nLoop++)
    //  {
    //    tmp = cbDivPrinsipal.SelectedItems[nLoop].Value;
    //    if (!string.IsNullOrEmpty(tmp))
    //    {
    //      //lstData.Add(string.Concat("'", tmp, "'"));
    //      lstData.Add(tmp);
    //    }
    //  }

    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string[]).FullName,
    //    ParameterName = "item",
    //    ParameterValueArray = lstData.ToArray(),
    //    IsDatasetParameter = true,
    //    IsInValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbItems.SelectedItems.Count == 1)
    //{
    //  lstData.Add((cbItems.SelectedItems[0].Value == null ? string.Empty : cbItems.SelectedItems[0].Value));

    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string[]).FullName,
    //    ParameterName = "item",
    //    ParameterValueArray = lstData.ToArray(),
    //    IsDatasetParameter = true,
    //    IsInValue = true
    //  });

    //  lstData.Clear();
    //}

    #endregion

    #endregion

    rptParse.PaperID = "Default";
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
          string rptName = string.Concat("Transaksi_Floating_", pag.Nip, ".", rptResult.Extension);

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
