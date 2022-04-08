using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_Transaksi_ReturCustomer : System.Web.UI.UserControl
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

    rptParse.ReportingID = "21004";

    isAsync = chkAsync.Checked;

    tmp = txRC1.Text.Trim();
    tmp1 = txRC2.Text.Trim();

    #region Report Parameter

    if (!string.IsNullOrEmpty(cbGudang.SelectedItem.Value))
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "LG_RCH.c_gdg",
            ParameterValue = cbGudang.SelectedItem.Value
        });
    }

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
          DataType = typeof(string).FullName,
          ParameterName = string.Format("({{LG_RCH.d_rcdate}} IN {0} to {1})", Functional.CrystalReportDateString(date1), Functional.CrystalReportDateString(date2)),
          IsReportDirectValue = true
        });
      }
      else if (!date1.Equals(DateTime.MinValue))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_RCH.d_rcdate",
          IsReportDirectValue = true,
          ParameterValue = date1.ToString("yyyy-MM-dd")
        });
      }
    }

    if (txRC1.Text.Length > 0)
    {
      if (txRC2.Text.Length > 0)
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = string.Format("({{LG_RCH.c_rcno}} IN ['{0}','{1}'])", txRC1.Text, txRC2.Text),
          IsReportDirectValue = true
        });
      }
      else
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = string.Format("({{LG_RCH.c_rcno}} >= '{0}')", txRC1.Text),
          IsReportDirectValue = true
        });
      }
    }
    else if (txRC2.Text.Length > 0)
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = string.Format("({{LG_RCH.c_rcno}} <= '{0}')", txRC2.Text),
        IsReportDirectValue = true
      });
    }

    #region Old Code
    //if (cbCustomer.SelectedItems.Count > 1)
    //{
    //  for (int nLoop = 0, nlen = cbCustomer.SelectedItems.Count; nLoop < nlen; nLoop++)
    //  {
    //    tmp = cbCustomer.SelectedItems[nLoop].Value;
    //    if (!string.IsNullOrEmpty(tmp))
    //    {
    //      lstData.Add(string.Concat("'", tmp, "'"));
    //    }
    //  }

    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = string.Format("({{LG_RCH.c_cusno}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbCustomer.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "LG_RCH.c_cusno",
    //    ParameterValue = (cbCustomer.SelectedItems[0].Value == null ? string.Empty : cbCustomer.SelectedItems[0].Value)
    //  });
    //}

    //if (cbSuplier.SelectedItems.Count > 1)
    //{
    //  for (int nLoop = 0, nlen = cbSuplier.SelectedItems.Count; nLoop < nlen; nLoop++)
    //  {
    //    tmp = cbSuplier.SelectedItems[nLoop].Value;
    //    if (!string.IsNullOrEmpty(tmp))
    //    {
    //      lstData.Add(string.Concat("'", tmp, "'"));
    //    }
    //  }

    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = string.Format("({{LG_DatSup.c_nosup}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbSuplier.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "LG_DatSup.c_nosup",
    //    ParameterValue = (cbSuplier.SelectedItems[0].Value == null ? string.Empty : cbSuplier.SelectedItems[0].Value)
    //  });
    //}

    //if (cbDivPrinsipal.SelectedItems.Count > 1)
    //{
    //  for (int nLoop = 0, nlen = cbSuplier.SelectedItems.Count; nLoop < nlen; nLoop++)
    //  {
    //    tmp = cbDivPrinsipal.SelectedItems[nLoop].Value;
    //    if (!string.IsNullOrEmpty(tmp))
    //    {
    //      lstData.Add(string.Concat("'", tmp, "'"));
    //    }
    //  }

    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = string.Format("({{FA_DivPri.c_kddivpri}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbDivPrinsipal.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "FA_DivPri.c_kddivpri",
    //    ParameterValue = (cbDivPrinsipal.SelectedItems[0].Value == null ? string.Empty : cbDivPrinsipal.SelectedItems[0].Value)
    //  });
    //}
    //if (cbItems.SelectedItems.Count > 1)
    //{
    //  for (int nLoop = 0, nlen = cbItems.SelectedItems.Count; nLoop < nlen; nLoop++)
    //  {
    //    tmp = cbItems.SelectedItems[nLoop].Value;
    //    if (!string.IsNullOrEmpty(tmp))
    //    {
    //      lstData.Add(string.Concat("'", tmp, "'"));
    //    }
    //  }

    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = string.Format("({{LG_RCD1.c_iteno}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbItems.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "LG_RCD1.c_iteno",
    //    ParameterValue = (cbItems.SelectedItems[0].Value == null ? string.Empty : cbItems.SelectedItems[0].Value)
    //  });
    //}

    #endregion

    if (!string.IsNullOrEmpty(cbCustomer.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string[]).FullName,
        ParameterName = "LG_RCH.c_cusno",
        ParameterValue = cbCustomer.SelectedItem.Value
      });
    }

    if (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string[]).FullName,
        ParameterName = "LG_DatSup.c_nosup",
        ParameterValue = cbSuplier.SelectedItem.Value
      });
    }

    if (!string.IsNullOrEmpty(cbDivPrinsipal.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "FA_DivPri.c_kddivpri",
        ParameterValue = cbDivPrinsipal.SelectedItem.Value
      });
    }

    if (!string.IsNullOrEmpty(cbItems.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "LG_RCD1.c_iteno",
        ParameterValue = cbItems.SelectedItem.Value
      });
    }

    #endregion

    rptParse.PaperID = "17.5*11";
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
          string rptName = string.Concat("Transaksi_RC_", pag.Nip, ".", rptResult.Extension);

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
