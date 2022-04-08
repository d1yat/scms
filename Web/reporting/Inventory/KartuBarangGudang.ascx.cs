using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_Inventory_KartuBarangGudang : System.Web.UI.UserControl
{
  public void InitializePage(string wndDownload)
  {
    txPeriode1.Text = txPeriode2.Text = DateTime.Now.ToString("dd-MM-yyyy");
    chkAsync.Checked = true;
    rdgTipe.SetValue(rdTotal.ClientID, true);
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

    rptParse.ReportingID = "10003";

    if (rdBatch.Checked)
    {
      lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(string).FullName,
          ParameterName = "TipeReport",
          IsSqlParameter = true,
          ParameterValue = "01"
      });
    }
    else
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "TipeReport",
            IsSqlParameter = true,
            ParameterValue = "02"
        });
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
        DataType = typeof(char).FullName,
        ParameterName = "gdg",
        IsSqlParameter = true,
        ParameterValue = cbGudang.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(char).FullName,
        ParameterName = "divams",
        IsSqlParameter = true,
        ParameterValue = cbDivAms.SelectedItem.Value == null ? "000" : cbDivAms.SelectedItem.Value
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


    lstData.Clear();

    //lstRptParam.Add(new ReportParameter()
    //{
    //    DataType = typeof(char).FullName,
    //    ParameterName = "gudang",
    //    //ParameterValueArray = lstData.ToArray(),
    //    ParameterValue = cbGudang.SelectedItem.Value,
    //    IsDatasetParameter = true
    //});

    //if (!string.IsNullOrEmpty(cbItems.SelectedItem.Value))
    //{
    //    lstData.Add(cbItems.SelectedItem.Value);

    //    lstRptParam.Add(new ReportParameter()
    //    {
    //        DataType = typeof(string[]).FullName,
    //        ParameterName = "item",
    //        ParameterValueArray = lstData.ToArray(),
    //        IsDatasetParameter = true,
    //        IsInValue = true
    //    });

    //  
    //}

    //if (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value))
    //{
    //  lstData.Add(cbSuplier.SelectedItem.Value);

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

    #region Old Coded

    //if (cbItems.SelectedItems.Count > 1)
    //{
    //  for (int nLoop = 0, nlen = cbItems.SelectedItems.Count; nLoop < nlen; nLoop++)
    //  {
    //    tmp = cbItems.SelectedItems[nLoop].Value;
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

    #region Report Parameter

    if (rdBatch.Checked)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "temp_lgkartubarangbatch.c_user",
            ParameterValue = pag.Nip
        });
    }
      else if (rdTotal.Checked)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "temp_lgkartubarangtotal.c_user",
            ParameterValue = pag.Nip
        });
    }

    lstCustTxt.Add(new ReportCustomizeText()
    {
        SectionName = "Section2",
        ControlName = "txtPeriode",
        Value = string.Format("Periode : {0} s/d {1}", date1.ToString("dd-MMM-yyyy"), date2.ToString("dd-MMM-yyyy"))
    });

    //if (!string.IsNullOrEmpty(cbDivAms.SelectedItem.Value))
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "DSNG_SCMS_KARTU_BARANG_BATCH.c_kddivams",
    //    ParameterValue = cbDivAms.SelectedItem.Value
    //  });
    //}

    //if (!string.IsNullOrEmpty(cbDivPrinsipal.SelectedItem.Value))
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "DSNG_SCMS_KARTU_BARANG_BATCH.c_kddivpri",
    //    ParameterValue = cbDivPrinsipal.SelectedItem.Value
    //  });
    //}

    #region Old Coded

    //if (cbDivAms.SelectedItems.Count > 1)
    //{
    //  for (int nLoop = 0, nlen = cbDivAms.SelectedItems.Count; nLoop < nlen; nLoop++)
    //  {
    //    tmp = cbDivAms.SelectedItems[nLoop].Value;
    //    if (!string.IsNullOrEmpty(tmp))
    //    {
    //      lstData.Add(string.Concat("'", tmp, "'"));
    //    }
    //  }

    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = string.Format("({{DSNG_SCMS_KARTU_BARANG_BATCH.c_kddivams}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbDivAms.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "DSNG_SCMS_KARTU_BARANG_BATCH.c_kddivams",
    //    ParameterValue = (cbDivAms.SelectedItems[0].Value == null ? string.Empty : cbDivAms.SelectedItems[0].Value)
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
    //    ParameterName = string.Format("({{DSNG_SCMS_KARTU_BARANG_BATCH.c_kddivpri}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbDivPrinsipal.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "DSNG_SCMS_KARTU_BARANG_BATCH.c_kddivpri",
    //    ParameterValue = (cbDivPrinsipal.SelectedItems[0].Value == null ? string.Empty : cbDivPrinsipal.SelectedItems[0].Value)
    //  });
    //}

    #endregion

    #endregion

    rptParse.PaperID = "A3";
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
          string rptName = string.Concat("Inventory_KartuBarangGudang_", pag.Nip, ".", rptResult.Extension);

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
