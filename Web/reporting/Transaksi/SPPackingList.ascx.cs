using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_Transaksi_SPPackingList : System.Web.UI.UserControl
{
  protected void Page_Load(object sender, EventArgs e)
  {

  }

  public void InitializePage(string wndDownload)
  {
    hidWndDown.Text = wndDownload;
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
    string tmp = null,
      tmp1 = null;
    List<string> lstData = new List<string>();
    bool isAsync = false;

    if (rdDetil.Checked == true)
    {
      rptParse.ReportingID = "21010-a";
    }
    if (rdItm.Checked == true)
    {
      rptParse.ReportingID = "21010-b";
    }
    if (rdDivPri.Checked == true)
    {
      rptParse.ReportingID = "21010-c";
    }

    isAsync = chkAsync.Checked;


    tmp = txNoSP1.Text.Trim();
    tmp1 = txNoSP2.Text.Trim();

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
      if ((!date1.Equals(DateTime.MinValue)) && (!date1.Equals(DateTime.MinValue)) && (!date1.Equals(date2)))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = string.Format("({{LG_SPH.d_spdate}} IN '{0}' to '{1}')", date1.ToString("yyyy-MM-dd"), date2.ToString("yyyy-MM-dd")),
          IsReportDirectValue = true
        });
      }
      else if (!date1.Equals(DateTime.MinValue))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_SPH.d_spdate",
          ParameterValue = date1.ToString("yyyy-MM-dd")
        });
      }
    }

    if (txNoSP1.Text.Length > 0)
    {
      if (txNoSP2.Text.Length > 0)
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = string.Format("({{LG_SPH.c_spno}} IN ['{0}','{1}'])", txNoSP1.Text, txNoSP2.Text),
          IsReportDirectValue = true
        });
      }
      else
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = string.Format("({{lg_sph.c_spno}} >= '{0}')", txNoSP1.Text),
          IsReportDirectValue = true
        });
      }
    }
    else if (txNoSP2.Text.Length > 0)
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = string.Format("({{lg_sph.c_spno}} <= '{0}')", txNoSP2.Text),
        IsReportDirectValue = true
      });
    }

    if (!string.IsNullOrEmpty(cbCustomer.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "LG_sph.c_cusno",
        ParameterValue = (cbCustomer.SelectedItem.Value.ToString() == null ? string.Empty : cbCustomer.SelectedItem.Value.ToString())
      });
    }
    if (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "LG_plh.c_nosup",
        ParameterValue = (cbSuplier.SelectedItem.Value.ToString() == null ? string.Empty : cbSuplier.SelectedItem.Value.ToString())
      });
    }

    if (!string.IsNullOrEmpty(cbDivPrinsipal.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "FA_MsDivPri.c_kddivpri",
        ParameterValue = (cbDivPrinsipal.SelectedItem.Value.ToString() == null ? string.Empty : cbDivPrinsipal.SelectedItem.Value.ToString())
      });
    }

    if (!string.IsNullOrEmpty(cbItems.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "lg_spd1.c_iteno",
        ParameterValue = (cbItems.SelectedItem.Value.ToString() == null ? string.Empty : cbItems.SelectedItem.Value.ToString())
      });
    }

    #region Old Coded

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
    //    ParameterName = string.Format("({{LG_Cusmas.c_cusno}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbCustomer.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "LG_Cusmas.c_cusno",
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
    //    ParameterName = string.Format("({{FA_MasItm.c_iteno}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });
    //  lstData.Clear();

    //}
    //else if (cbItems.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "FA_MasItm.c_iteno",
    //    ParameterValue = (cbItems.SelectedItems[0].Value == null ? string.Empty : cbItems.SelectedItems[0].Value)
    //  });
    //}

    #endregion

    if (!rdAll.Checked)
    {
      if (rdSPO.Checked)
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "({lg_sph.c_type} = '01')",
          IsReportDirectValue = true
        });
      }
      if (rdSPM.Checked)
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "({lg_sph.c_type} = '02')",
          IsReportDirectValue = true
        });
      }
      if (rdSPHO.Checked)
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "({lg_sph.c_type} = '03')",
          IsReportDirectValue = true
        });
      }
      if (rdSPADJ.Checked)
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "({lg_sph.c_type} = '04')",
          IsReportDirectValue = true
        });
      }
    }

    #endregion

    rptParse.PaperID = "Letter";
    rptParse.ReportCustomizeText = null;
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;
    rptParse.IsLandscape = true;

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
          string rptName = string.Concat("Report_SPPL_", pag.Nip, ".", rptResult.Extension);

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
