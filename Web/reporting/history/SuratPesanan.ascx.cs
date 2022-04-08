using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_history_SuratPesanan : System.Web.UI.UserControl
{
  public void InitializePage(string wndDownload)
  {
    hidWndDown.Text = wndDownload;
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      Scms.Web.Core.PageHandler page = this.Page as Scms.Web.Core.PageHandler;

      if (page.IsCabang)
      {
        hidCab.Text = page.Cabang;

        Functional.SetComboData(cbCustomer, "c_cusno", page.CabangDescription, page.Cabang);
      }
      if (page.IsSupplier)
      {
        hidSupl.Text = page.Supplier;
        Functional.SetComboData(cbSuplier, "c_nosup", page.SupplierDescription, page.Supplier);

        hidDivSupl.Text = page.DivisiSupplier;
        Functional.SetComboData(cbDivPrinsipal, "c_kddivpri", page.DivisiSupplierDescription, page.DivisiSupplier);
      }
    }
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
    string tmp = null,
      tmp1 = null;
    bool isAsync = false;
    int nTotalDays = 0;

    rptParse.ReportingID = "20003";

    isAsync = chkAsync.Checked;

    tmp = txSP1.Text.Trim();
    tmp1 = txSP2.Text.Trim();

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
      if ((!date1.Equals(DateTime.MinValue)) && (!date2.Equals(DateTime.MinValue)) && (!date1.Equals(date2)))
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

    nTotalDays = (int)date2.Subtract(date2).TotalDays;

    if (pag.IsSupplier)
    {
      if ((nTotalDays < 0) && (nTotalDays > 22))
      {
        e.Success = false;
        e.ErrorMessage = "Maaf, anda tidak dapat membaca data lebih dari 0-22 hari.";
        return;
      }
    }
    else if (pag.IsCabang)
    {
      if ((nTotalDays < 0) && (nTotalDays > 45))
      {
        e.Success = false;
        e.ErrorMessage = "Maaf, anda tidak dapat membaca data lebih dari 0-22 hari.";
        return;
      }
    }

    if (!string.IsNullOrEmpty(txSP1.Text))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "sp1",
        ParameterValue = txSP1.Text.Trim(),
        IsDatasetParameter = true,
      });
    }

    if (!string.IsNullOrEmpty(txSP2.Text))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "sp2",
        ParameterValue = txSP2.Text.Trim(),
        IsDatasetParameter = true,
      });
    }

    if (pag.IsCabang)
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "customer",
        ParameterValue = pag.Cabang,
        IsDatasetParameter = true,
      });
    }
    else
    {
      if (!string.IsNullOrEmpty(cbCustomer.SelectedItem.Value))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "customer",
          ParameterValue = cbCustomer.SelectedItem.Value,
          IsDatasetParameter = true,
        });
      }
    }

    if (pag.IsSupplier)
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "supplier",
        ParameterValue = pag.Supplier,
        IsDatasetParameter = true,
      });

      if (!string.IsNullOrEmpty(pag.DivisiSupplier))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "divprin",
          ParameterValue = pag.DivisiSupplier,
          IsDatasetParameter = true,
        });
      }
    }
    else
    {
      if (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "supplier",
          ParameterValue = cbSuplier.SelectedItem.Value,
          IsDatasetParameter = true,
        });
      }

      if (!string.IsNullOrEmpty(cbDivPrinsipal.SelectedItem.Value))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "divprin",
          ParameterValue = cbDivPrinsipal.SelectedItem.Value,
          IsDatasetParameter = true,
        });
      }
    }

    if (!string.IsNullOrEmpty(cbItems.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "item",
        ParameterValue = cbItems.SelectedItem.Value,
        IsDatasetParameter = true,
      });
    }

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "user",
      ParameterValue = pag.Nip,
      IsDatasetParameter = true,
    });

    #endregion

    #region Sql Parameter

    //lstRptParam.Add(new ReportParameter()
    //{
    //  DataType = typeof(string).FullName,
    //  ParameterName = "sp1",
    //  IsSqlParameter = true,
    //  ParameterValue = tmp
    //});

    //lstRptParam.Add(new ReportParameter()
    //{
    //  DataType = typeof(string).FullName,
    //  ParameterName = "sp2",
    //  IsSqlParameter = true,
    //  ParameterValue = tmp1
    //});
    
    //if (Scms.Web.Common.Functional.DateParser(txPeriode1.RawText.Trim(), "d-M-yyyy", out date1))
    //{
    //  if (Scms.Web.Common.Functional.DateParser(txPeriode2.RawText.Trim(), "d-M-yyyy", out date2))
    //  {
    //    if (date2.CompareTo(date1) <= 0)
    //    {
    //      date2 = date1;
    //    }
    //  }
    //  else
    //  {
    //    date2 = date1;
    //  }

    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(DateTime).FullName,
    //    ParameterName = "date1",
    //    IsSqlParameter = true,
    //    ParameterValue = date1.ToString("d-M-yyyy")
    //  });
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(DateTime).FullName,
    //    ParameterName = "date2",
    //    IsSqlParameter = true,
    //    ParameterValue = date2.ToString("d-M-yyyy")
    //  });
    //}

    //tmp = (pag.IsCabang ? pag.Cabang :
    //    (cbCustomer.SelectedItems.Count == 1 ? cbCustomer.SelectedItems[0].Value : "0000"));

    //lstRptParam.Add(new ReportParameter()
    //{
    //  DataType = typeof(string).FullName,
    //  ParameterName = "customer",
    //  IsSqlParameter = true,
    //  ParameterValue = tmp
    //});

    //lstRptParam.Add(new ReportParameter()
    //{
    //  DataType = typeof(string).FullName,
    //  ParameterName = "item",
    //  IsSqlParameter = true,
    //  ParameterValue = (cbItems.SelectedItems.Count == 1 ? cbItems.SelectedItems[0].Value : "0000")
    //});

    //lstRptParam.Add(new ReportParameter()
    //{
    //  DataType = typeof(string).FullName,
    //  ParameterName = "session",
    //  IsSqlParameter = true,
    //  ParameterValue = pag.Nip
    //});

    #endregion

    #region Report Parameter

    //lstRptParam.Add(new ReportParameter()
    //{
    //  DataType = typeof(string).FullName,
    //  ParameterName = "Temp_LGHistorySP.c_user",
    //  ParameterValue = pag.Nip
    //});

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
    //    ParameterName = string.Format("({{Temp_LGHistorySP.c_cusno}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
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
    //    ParameterName = string.Format("({{Temp_LGHistorySP.c_iteno}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}

    #endregion

    rptParse.PaperID = "A3";
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
          string rptName = string.Concat("History_PO_Transaksi_", pag.Nip, ".", rptResult.Extension);

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
