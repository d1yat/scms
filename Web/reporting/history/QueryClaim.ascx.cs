using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_history_QueryClaim : System.Web.UI.UserControl
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
    //List<string> lstData = null;
    string tmp = null;
    bool isAsync = false;

    tmp = (string.IsNullOrEmpty(cbTipeReport.SelectedItem.Value) ? "00" : cbTipeReport.SelectedItem.Value.Trim());

    if (tmp.Equals("itm"))
    {
      rptParse.ReportingID = "20011";
    }
    else if (tmp.Equals("sup"))
    {
      rptParse.ReportingID = "20011-1";
    }
    else if(tmp.Equals("div"))
    {
      rptParse.ReportingID = "20011-2";
    }

    #region Sql Parameter

    //lstRptParam.Add(new ReportParameter()
    //{
    //  DataType = typeof(string).FullName,
    //  ParameterName = "({lg_rnh.c_type} = '03')",
    //  IsReportDirectValue = true
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
    //  if ((!date1.Equals(DateTime.MinValue)) && (!date1.Equals(DateTime.MinValue)) && (!date1.Equals(date2)))
    //  {
    //    lstRptParam.Add(new ReportParameter()
    //    {
    //      DataType = typeof(string).FullName,
    //      ParameterName = string.Format("({{LG_rnh.d_rndate}} IN {0} to {1})", Functional.CrystalReportDateString(date1), Functional.CrystalReportDateString(date2)),
    //      IsReportDirectValue = true
    //    });
    //  }
    //  else if (!date1.Equals(DateTime.MinValue))
    //  {
    //    lstRptParam.Add(new ReportParameter()
    //    {
    //      DataType = typeof(string).FullName,
    //      ParameterName = "LG_rnh.d_rndate",
    //      IsReportDirectValue = true,
    //      ParameterValue = date1.ToString("yyyy-MM-dd")
    //    });
    //  }
    //}

    //if (txRN1.Text.Length > 0)
    //{
    //  if (txRN2.Text.Length > 0)
    //  {
    //    lstRptParam.Add(new ReportParameter()
    //    {
    //      DataType = typeof(string).FullName,
    //      ParameterName = string.Format("({{DSNG_SCMS_Claim.c_rnno}} IN ['{0}','{1}'])", txRN1.Text, txRN2.Text),
    //      IsReportDirectValue = true
    //    });
    //  }
    //  else
    //  {
    //    lstRptParam.Add(new ReportParameter()
    //    {
    //      DataType = typeof(string).FullName,
    //      ParameterName = string.Format("({{DSNG_SCMS_Claim.c_rnno}} >= '{0}')", txRN1.Text),
    //      IsReportDirectValue = true
    //    });
    //  }
    //}
    //else if (txRN2.Text.Length > 0)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = string.Format("({{LG_RNH.c_rnno}} <= '{0}')", txRN2.Text),
    //    IsReportDirectValue = true
    //  });
    //}

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
    //    ParameterName = string.Format("({{FA_MsDivAMS.c_kddivams}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbDivAms.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "FA_MsDivAMS.c_kddivams",
    //    ParameterValue = (cbDivAms.SelectedItems[0].Value == null ? string.Empty : cbDivAms.SelectedItems[0].Value)
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
    //  for (int nLoop = 0, nlen = cbDivPrinsipal.SelectedItems.Count; nLoop < nlen; nLoop++)
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
    //    ParameterName = string.Format("({{FA_MsDivPri.c_kddivpri}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbDivPrinsipal.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "FA_MsDivPri.c_kddivpri",
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
    //    ParameterName = string.Format("({{fa_masitm.c_iteno}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbItems.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "fa_masitm.c_iteno",
    //    ParameterValue = (cbItems.SelectedItems[0].Value == null ? string.Empty : cbItems.SelectedItems[0].Value)
    //  });
    //}

    #endregion

    #region Dataset Parameter

    if (txRN1.Text.Length > 0)
    {
      if (txRN2.Text.Length > 0)
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          //ParameterName = string.Format("({{DSNG_SCMS_Claim.c_rnno}} IN ['{0}','{1}'])", txRN1.Text, txRN2.Text),
          ParameterName = "RN",
          ParameterValue = txRN1.Text,
          BetweenValue = txRN2.Text,
          //IsReportDirectValue = true
          IsDatasetParameter = true,
          IsBetweenValue = true
        });
      }
      else
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          //ParameterName = string.Format("({{DSNG_SCMS_Claim.c_rnno}} >= '{0}')", txRN1.Text),
          ParameterName = "RN",
          ParameterValue = txRN1.Text,
          //IsReportDirectValue = true
          IsDatasetParameter = true
        });
      }
    }
    else if (txRN2.Text.Length > 0)
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        //ParameterName = string.Format("({{LG_RNH.c_rnno}} <= '{0}')", txRN2.Text),
        ParameterName = "RN",
        ParameterValue = txRN2.Text,
        //IsReportDirectValue = true
        IsDatasetParameter = true
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

    if (cbSuplier.SelectedItem.Value.ToString().Trim().Length > 0)
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string[]).FullName,
        ParameterName = "nosup",
        ParameterValue = cbSuplier.SelectedItem.Value.ToString(),
        IsDatasetParameter = true,
        IsInValue = true
      });
    }
    if (cbDivPrinsipal.SelectedItem.Value.ToString().Trim().Length > 0)
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string[]).FullName,
        ParameterName = "divpri",
        ParameterValue = cbDivPrinsipal.SelectedItem.Value.ToString(),
        IsDatasetParameter = true,
        IsInValue = true
      });
    }
    if (cbDivAms.SelectedItem.Value.ToString().Trim().Length > 0)
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string[]).FullName,
        ParameterName = "divams",
        ParameterValue = cbDivAms.SelectedItem.Value.ToString(),
        IsDatasetParameter = true,
        IsInValue = true
      });
    }
    if (cbItems.SelectedItem.Value.ToString().Trim().Length > 0)
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string[]).FullName,
        ParameterName = "item",
        ParameterValue = cbItems.SelectedItem.Value.ToString(),
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
    //    ParameterName = "nosup",
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
    //    ParameterName = "nosup",
    //    ParameterValueArray = lstData.ToArray(),
    //    IsDatasetParameter = true,
    //    IsInValue = true
    //  });

    //  lstData.Clear();
    //}
    //if (cbDivPrinsipal.SelectedItems.Count > 1)
    //{
    //  for (int nLoop = 0, nlen = cbDivPrinsipal.SelectedItems.Count; nLoop < nlen; nLoop++)
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
    //    ParameterName = "divpri",
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
    //    ParameterName = "divpri",
    //    ParameterValueArray = lstData.ToArray(),
    //    IsDatasetParameter = true,
    //    IsInValue = true
    //  });

    //  lstData.Clear();
    //}
    //if (cbDivAms.SelectedItems.Count > 1)
    //{
    //  for (int nLoop = 0, nlen = cbDivAms.SelectedItems.Count; nLoop < nlen; nLoop++)
    //  {
    //    tmp = cbDivAms.SelectedItems[nLoop].Value;
    //    if (!string.IsNullOrEmpty(tmp))
    //    {
    //      //lstData.Add(string.Concat("'", tmp, "'"));
    //      lstData.Add(tmp);
    //    }
    //  }

    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string[]).FullName,
    //    ParameterName = "divams",
    //    ParameterValueArray = lstData.ToArray(),
    //    IsDatasetParameter = true,
    //    IsInValue = true,

    //  });

    //  lstData.Clear();
    //}
    //else if (cbDivAms.SelectedItems.Count == 1)
    //{
    //  lstData.Add((cbDivAms.SelectedItems[0].Value == null ? string.Empty : cbDivAms.SelectedItems[0].Value));

    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string[]).FullName,
    //    ParameterName = "divams",
    //    ParameterValueArray = lstData.ToArray(),
    //    IsDatasetParameter = true,
    //    IsInValue = true
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

    rptParse.PaperID = "Letter";
    rptParse.ReportCustomizeText = null;
    rptParse.IsLandscape = true;
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
          string rptName = string.Concat("History_Query_Claim_", pag.Nip, ".", rptResult.Extension);

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
