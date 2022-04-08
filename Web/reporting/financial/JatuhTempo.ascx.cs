﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_financial_JatuhTempo : System.Web.UI.UserControl
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
      //return;
    }

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    DateTime date1 = DateTime.Today,
      date2 = DateTime.Today;
    //List<string> lstData = null;
    //string tmp = null;
    bool isAsync = false;

    rptParse.ReportingID = "20408";

    isAsync = chkAsync.Checked;

    #region DataSet Parameter

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
          ParameterValue = date2.ToString("yyyy-MM-dd")
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
    if (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string[]).FullName,
        ParameterName = "supplier",
        ParameterValue = cbSuplier.SelectedItem.Value,
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

    #endregion

    #endregion

    rptParse.ReportCustomizeText = null;
    //rptParse.IsLandscape = true;
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
          string rptName = string.Concat("Financial_JatuhTempo_", pag.Nip, ".", rptResult.Extension);

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
