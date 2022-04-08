using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_konsolidasi_RDP : System.Web.UI.UserControl
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
    string tmp = null,
    rbgCheck = null;
    bool isAsync = false;

    rptParse.ReportingID = "20102";

    isAsync = chkAsync.Checked;

    #region Report Parameter
    
    #region old
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
    //    ParameterName = string.Format("({{LG_SNAP_RDP.c_cusno}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbCustomer.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "LG_SNAP_RDP.c_cusno",
    //    ParameterValue = (cbCustomer.SelectedItems[0].Value == null ? string.Empty : cbCustomer.SelectedItems[0].Value)
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
    //    ParameterName = string.Format("({{LG_SNAP_RDP.c_kddivams}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbDivAms.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "LG_SNAP_RDP.c_kddivams",
    //    ParameterValue = (cbDivAms.SelectedItems[0].Value == null ? string.Empty : cbDivAms.SelectedItems[0].Value)
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
    //    ParameterName = string.Format("({{LG_SNAP_RDP.c_iteno}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbItems.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "LG_SNAP_RDP.c_iteno",
    //    ParameterValue = (cbItems.SelectedItems[0].Value == null ? string.Empty : cbItems.SelectedItems[0].Value)
    //  });
    //}
#endregion

    if (!string.IsNullOrEmpty(cbCustomer.SelectedItem.Value))
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{LG_SNAP_RDP.c_cusno}} = '{0}')", cbCustomer.SelectedItem.Value),
            IsReportDirectValue = true
        });
    }

    if (!string.IsNullOrEmpty(cbDivAms.SelectedItem.Value))
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{LG_SNAP_RDP.c_kddivams}} = '{0}')", cbDivAms.SelectedItem.Value),
            IsReportDirectValue = true
        });
    }

    if (!string.IsNullOrEmpty(cbItems.SelectedItem.Value))
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{LG_SNAP_RDP.c_iteno}} = '{0}')", cbItems.SelectedItem.Value),
            IsReportDirectValue = true
        });
    }

    //lstData = rbgTipeReport.CheckedTags;

    if (rbgTRRDP.Checked == true)
    {
        rbgCheck = "01";
    }
    else if (rbgTRRRDP.Checked == true)
    {
        rbgCheck = "02";
    }
    else if (rbgTRPLL.Checked == true)
    {
        rbgCheck = "03";
    }
    else if (rbgTRSELI.Checked == true)
    {
        rbgCheck = "04";
    }
    else if (rbgTRRSC.Checked == true)
    {
        rbgCheck = "05";
    }
    else if (rbgTRRSCS.Checked == true)
    {
        rbgCheck = "06";
    }
    else if (rbgTRPEND.Checked == true)
    {
        rbgCheck = "07";
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
      }

      if (rbgCheck == "01")
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_SNAP_RDP.v_type",
          ParameterValue = "01"
        });

        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = string.Format("( {{LG_SNAP_RDP.d_rcdate}} In {0} To {1} )",
            Functional.CrystalReportDateString(date1), Functional.CrystalReportDateString(date2)),
          IsReportDirectValue = true
        });

        lstCustTxt.Add(new ReportCustomizeText()
        {
            SectionName = "Section2",
            ControlName = "txtPeriodeMode",
            Value = string.Format("Periode Retur : {0} s/d {1}", date1.ToString("dd-MMM-yyyy"), date2.ToString("dd-MMM-yyyy"))
        });
      }
      else if (rbgCheck == "02")
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_SNAP_RDP.v_type",
          ParameterValue = "02"
        });

        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = string.Format("( {{LG_SNAP_RDP.d_rcdate}} In {0} To {1} )",
            Functional.CrystalReportDateString(date1), Functional.CrystalReportDateString(date2)),
          IsReportDirectValue = true
        });
        
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = string.Format("( IsNull({{LG_SNAP_RDP.d_invdate}}) = true Or {{LG_SNAP_RDP.d_invdate}} < {0} Or {{LG_SNAP_RDP.d_invdate}} > {1} )",
            Functional.CrystalReportDateString(date1), Functional.CrystalReportDateString(date2)),
          IsReportDirectValue = true
        });

        lstCustTxt.Add(new ReportCustomizeText()
        {
            SectionName = "Section2",
            ControlName = "txtPeriodeMode",
            Value = string.Format("Periode Retur : {0} s/d {1}", date1.ToString("dd-MMM-yyyy"), date2.ToString("dd-MMM-yyyy"))
        });
      }
      else if (rbgCheck == "03")
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_SNAP_RDP.v_type",
          ParameterValue = "03"
        });

        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = string.Format("( {{LG_SNAP_RDP.d_invdate}} In {0} To {1} )",
            Functional.CrystalReportDateString(date1), Functional.CrystalReportDateString(date2)),
          IsReportDirectValue = true
        });

        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          //ParameterName = string.Format("( IsNull({{LG_SNAP_RDP.d_rcdate}}) = true ) Or ( {{LG_SNAP_RDP.d_rcdate}} < {0} ) Or ( {{LG_SNAP_RDP.d_rcdate}} > {1} )",
            ParameterName = string.Format("( isNull({{LG_SNAP_RDP.d_rcdate}}) = true OR {{LG_SNAP_RDP.d_rcdate}} < {0} OR {{LG_SNAP_RDP.d_rcdate}} > {1} )",
            Functional.CrystalReportDateString(date1), Functional.CrystalReportDateString(date2)),
          IsReportDirectValue = true
        });

        lstCustTxt.Add(new ReportCustomizeText()
        {
            SectionName = "Section2",
            ControlName = "txtPeriodeMode",
            Value = string.Format("Periode Invent : {0} s/d {1}", date1.ToString("dd-MMM-yyyy"), date2.ToString("dd-MMM-yyyy"))
        });
      }
      else if (rbgCheck == "04")
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_SNAP_RDP.v_type",
          ParameterValue = "04"
        });

        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = string.Format("({{LG_SNAP_RDP.d_rcdate}} In {0} To {1} OR {{LG_SNAP_RDP.d_invdate}} In {0} To {1})",
            Functional.CrystalReportDateString(date1), Functional.CrystalReportDateString(date2)),
          IsReportDirectValue = true
        });
        
        //lstRptParam.Add(new ReportParameter()
        //{
        //  DataType = typeof(string).FullName,
        //  ParameterName = string.Format("( {{LG_SNAP_RDP.d_invdate}} In {0} To {1} )",
        //    Functional.CrystalReportDateString(date1), Functional.CrystalReportDateString(date2)),
        //  IsReportDirectValue = true
        //});

        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "( {@n_gbqty} <> {@n_gbqtyrcv} OR {LG_SNAP_RDP.n_salpri} <> {LG_SNAP_RDP.n_salprircv} OR {LG_SNAP_RDP.n_disc} <> {LG_SNAP_RDP.n_discrcv} )",
          IsReportDirectValue = true
        });

        lstCustTxt.Add(new ReportCustomizeText()
        {
            SectionName = "Section2",
            ControlName = "txtPeriodeMode",
            Value = string.Format("Periode Retur & Invent : {0} s/d {1}", date1.ToString("dd-MMM-yyyy"), date2.ToString("dd-MMM-yyyy"))
        });
      }
      else if (rbgCheck == "05")
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_SNAP_RDP.v_type",
          ParameterValue = "05"
        });

        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = string.Format("( {{LG_SNAP_RDP.d_invdate}} In {0} To {1} )",
            Functional.CrystalReportDateString(date1), Functional.CrystalReportDateString(date2)),
          IsReportDirectValue = true
        });

        lstCustTxt.Add(new ReportCustomizeText()
        {
            SectionName = "Section2",
            ControlName = "txtPeriodeMode",
            Value = string.Format("Periode Invent : {0} s/d {1}", date1.ToString("dd-MMM-yyyy"), date2.ToString("dd-MMM-yyyy"))
        });
      }
      else if (rbgCheck == "06")
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_SNAP_RDP.v_type",
          ParameterValue = "06"
        });

        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = string.Format("( {{LG_SNAP_RDP.d_invdate}} In {0} To {1} )",
            Functional.CrystalReportDateString(date1), Functional.CrystalReportDateString(date2)),
          IsReportDirectValue = true
        });

        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "( IsNull({LG_SNAP_RDP.d_rcdate}) = false )",
          IsReportDirectValue = true
        });

        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "( {LG_SNAP_RDP.n_gqty} <> {LG_SNAP_RDP.n_gqtyrs} OR {LG_SNAP_RDP.n_bqty} <> {LG_SNAP_RDP.n_bqtyrs} )",
          IsReportDirectValue = true
        });

        lstCustTxt.Add(new ReportCustomizeText()
        {
            SectionName = "Section2",
            ControlName = "txtPeriodeMode",
            Value = string.Format("Periode Invent : {0} s/d {1}", date1.ToString("dd-MMM-yyyy"), date2.ToString("dd-MMM-yyyy"))
        });
      }
      else if (rbgCheck == "07")
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_SNAP_RDP.v_type",
          ParameterValue = "07"
        });

        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = string.Format("( {{LG_SNAP_RDP.d_rcdate}} In {0} To {1} )",
            Functional.CrystalReportDateString(date1), Functional.CrystalReportDateString(date2)),
          IsReportDirectValue = true
        });

        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "IsNull({LG_SNAP_RDP.d_invdate}) = true",
          IsReportDirectValue = true
        });

        lstCustTxt.Add(new ReportCustomizeText()
        {
            SectionName = "Section2",
            ControlName = "txtPeriodeMode",
            Value = string.Format("Periode Invent : {0} s/d {1}", date1.ToString("dd-MMM-yyyy"), date2.ToString("dd-MMM-yyyy"))
        });
      }
      else
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_SNAP_RDP.v_type",
          ParameterValue = "??"
        });
      }

    lstData.Clear();

    #endregion

    rptParse.PaperID = "17.5*11";
    rptParse.ReportCustomizeText = lstCustTxt.ToArray();
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;

    rptParse.OutputReport = ReportParser.ParsingOutputReport((cbRptTypeOutput.SelectedItem != null ? cbRptTypeOutput.SelectedItem.Value : string.Empty));
    //rptParse.IsShared = chkShare.Checked;
    //rptParse.UserDefinedName = txRptUName.Text.Trim();

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
          string rptName = string.Concat("Konsolidasi_RDP_", pag.Nip, ".", rptResult.Extension);

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
