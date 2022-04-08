using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_konsolidasi_StokNasional : System.Web.UI.UserControl
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
    string tmp = null;
    bool isAsync = false;
    int nIdx1 = -1,
      nIdx2 = -1;

    if (chkDivPri.Checked)
    {
      rptParse.ReportingID = "20103-1";
    }
    else
    {
      rptParse.ReportingID = "20103-2";
    }

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
      ParameterName = "session",
      IsSqlParameter = true,
      ParameterValue = pag.Nip
    });

    #endregion
    
    #region Report Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "Temp_LGStockNas.c_user",
      ParameterValue = pag.Nip
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "( {Temp_LGStockNas.n_qtyg} <> 0 Or {Temp_LGStockNas.n_qtyb} <> 0 Or {Temp_LGStockNas.n_avgsales} <> 0 )",
      IsReportDirectValue = true
    });

    if (cbSuplier.SelectedItems.Count > 1)
    {
      for (int nLoop = 0, nlen = cbSuplier.SelectedItems.Count; nLoop < nlen; nLoop++)
      {
        tmp = cbSuplier.SelectedItems[nLoop].Value;
        if (!string.IsNullOrEmpty(tmp))
        {
          lstData.Add(string.Concat("'", tmp, "'"));
        }
      }

      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = string.Format("({{LG_DatSup.c_nosup}} IN [{0}])", string.Join(",", lstData.ToArray())),
        IsReportDirectValue = true
      });

      lstData.Clear();
    }
    else if (cbSuplier.SelectedItems.Count == 1)
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "LG_DatSup.c_nosup",
        ParameterValue = (cbSuplier.SelectedItems[0].Value == null ? string.Empty : cbSuplier.SelectedItems[0].Value)
      });
    }

    if (cbDivPrinsipal.SelectedItems.Count > 1)
    {
      for (int nLoop = 0, nlen = cbDivPrinsipal.SelectedItems.Count; nLoop < nlen; nLoop++)
      {
        tmp = cbDivPrinsipal.SelectedItems[nLoop].Value;
        if (!string.IsNullOrEmpty(tmp))
        {
          lstData.Add(string.Concat("'", tmp, "'"));
        }
      }

      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = string.Format("({{FA_DivPri.c_kddivpri}} IN [{0}])", string.Join(",", lstData.ToArray())),
        IsReportDirectValue = true
      });

      lstData.Clear();
    }
    else if (cbDivPrinsipal.SelectedItems.Count == 1)
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "FA_DivPri.c_kddivpri",
        ParameterValue = (cbDivPrinsipal.SelectedItems[0].Value == null ? string.Empty : cbDivPrinsipal.SelectedItems[0].Value)
      });
    }

    if (cbItems.SelectedItems.Count > 1)
    {
      for (int nLoop = 0, nlen = cbItems.SelectedItems.Count; nLoop < nlen; nLoop++)
      {
        tmp = cbItems.SelectedItems[nLoop].Value;
        if (!string.IsNullOrEmpty(tmp))
        {
          lstData.Add(string.Concat("'", tmp, "'"));
        }
      }

      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = string.Format("({{Temp_LGStockNas.c_iteno}} IN [{0}])", string.Join(",", lstData.ToArray())),
        IsReportDirectValue = true
      });

      lstData.Clear();
    }
    else if (cbItems.SelectedItems.Count == 1)
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "Temp_LGStockNas.c_iteno",
        ParameterValue = (cbItems.SelectedItems[0].Value == null ? string.Empty : cbItems.SelectedItems[0].Value)
      });
    }

    if (int.TryParse(txIndex1.Text.Trim(), out nIdx1))
    {
      if (int.TryParse(txIndex2.Text.Trim(), out nIdx2))
      {
        if (nIdx2.CompareTo(nIdx1) <= 0)
        {
          nIdx2 = nIdx1;
        }
      }
      else
      {
        nIdx2 = nIdx1;
      }
      
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = string.Format("( {{@n_index}} IN {0} TO {1} )", nIdx1, nIdx2),
        IsReportDirectValue = true
      });      
    }

    #endregion

    rptParse.IsLandscape = true;
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
          string rptName = string.Concat("Konsolidasi_Stok_Nasional_", pag.Nip, ".", rptResult.Extension);

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
