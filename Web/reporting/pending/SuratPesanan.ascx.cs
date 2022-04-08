using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_pending_SuratPesanan : System.Web.UI.UserControl
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
      DateTime date = DateTime.Now;

      cbPeriode1.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
      date = date.AddYears(-1);
      cbPeriode1.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
      date = date.AddYears(-1);
      cbPeriode1.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
      cbPeriode1.SelectedIndex = 0;

      Functional.PopulateBulan(cbPeriode2, date.Month);
      cbPeriode2.Items.Add(new Ext.Net.ListItem("2 Bln Terakhir", "00"));
      cbPeriode2.SelectedIndex = 12;



      if (page.IsCabang)
      {
          hidCab.Text = page.Cabang;

          Functional.SetComboData(cbCustomer, "c_cusno", page.CabangDescription, page.Cabang);
          Functional.SetComboData(cbTipeSP, "c_type", "SP HO", "03");
          rdgTRBulanan.Checked = true;
          rdgTipeReport.Disabled = true;
          rdgTipeReport.Disabled = true;
          cbGudang.Disabled = true;
          cbCustomer.Disabled = true;
          chkAsync.Clear();
      }
      else
      {
          Functional.SetComboData(cbGudang, "c_gdg", page.ActiveGudangDescription, page.ActiveGudang); 
      }
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    DateTime date1 = DateTime.Today,
      date2 = DateTime.Today;
    //List<string> lstData = new List<string>();
    List<ReportCustomizeText> lstCustom = new List<ReportCustomizeText>();
    bool isAsync = false;
    string tmp = null;

    isAsync = chkAsync.Checked;

    #region Report Parameter

    //lstData = rdgTipeReport.CheckedTags;
    tmp = Functional.GetCheckedRadioData(rdgTipeReport);

    if (tmp.Contains("01"))
    {
      rptParse.IsLandscape = false;
      rptParse.ReportingID = "20301-1";
    }
    else if (tmp.Contains("02"))
    {
      rptParse.IsLandscape = false;
      rptParse.ReportingID = "20301-2";
    }
    else if (tmp.Contains("03"))
    {
      rptParse.IsLandscape = false;
      rptParse.ReportingID = "20301-3";
    }
    else if (tmp.Contains("04"))
    {
      rptParse.IsLandscape = true;
      rptParse.ReportingID = "20301-4";
    }
    else if (tmp.Contains("05") || tmp.Contains("06"))
    {
      rptParse.IsLandscape = true;

      if (tmp.Contains("05"))
      {
          rptParse.ReportingID = "20301-5";
      }
      else
      {
          rptParse.ReportingID = "20301-6";
      }

      if (cbPeriode2.SelectedItem.Value != "00")
      {
          lstRptParam.Add(new ReportParameter()
          {
              DataType = typeof(string).FullName,
              ParameterName = string.Format("(Year(CDate({{LG_SPH.d_spdate}})) = {0})", cbPeriode1.SelectedItem.Value),
              IsReportDirectValue = true
          });
      }

      //exclude sp pharmanet
      lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(string).FullName,
          ParameterName = string.Format("{{LG_SPH.c_type}} <> '06'"),
          IsReportDirectValue = true
      });

      if (cbPeriode2.SelectedItem.Value != "00")
      {
          lstRptParam.Add(new ReportParameter()
          {
              DataType = typeof(string).FullName,
              ParameterName = string.Format("(Month(CDate({{LG_SPH.d_spdate}})) = {0})", cbPeriode2.SelectedItem.Value),
              IsReportDirectValue = true
          });
      }
      else
      {
          lstRptParam.Add(new ReportParameter()
              {
                  DataType = typeof(string).FullName,
                  ParameterName = string.Format("(if isnull({{LG_Cusmas.n_days}}) then " +
                    "(if Month (Date(CurrentDateTime)) = 1 then " +
                    "Date({{LG_SPH.d_spdate}}) in Date (Year (CurrentDate) -1,12 ,1) " +
                    "to Date (Year (CurrentDate), Month (CurrentDate), Day (CurrentDate)) " +
                    "else Month (Date({{LG_SPH.d_spdate}})) in Month (Date(CurrentDateTime)) to (Month (Date(CurrentDateTime)) - 1) " +
                    "and Year (Date({{LG_SPH.d_spdate}})) = Year(CurrentDate)) " +
                    "else (datediff('d',date({{LG_SPH.d_spdate}}), currentdate) < {{LG_Cusmas.n_days}}))"),
                  IsReportDirectValue = true
              });
      }

      lstCustom.Add(new ReportCustomizeText()
      {
        SectionName = "Section2",
        ControlName = "txtPeriodeMode",
        Value = string.Format("Periode SP Pending : {0}-{1}", cbPeriode2.SelectedItem.Text, cbPeriode1.SelectedItem.Value)
      });
    }

    if (tmp != "05")
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("(if isnull({{LG_Cusmas.n_days}}) then " +
              "(if Month (Date(CurrentDateTime)) = 1 then " +
              "Date({{LG_SPH.d_spdate}}) in Date (Year (CurrentDate) -1,12 ,1) " +
              "to Date (Year (CurrentDate), Month (CurrentDate), Day (CurrentDate)) " +
              "else Month (Date({{LG_SPH.d_spdate}})) in Month (Date(CurrentDateTime)) to (Month (Date(CurrentDateTime)) - 1) " +
              "and Year (Date({{LG_SPH.d_spdate}})) = Year(CurrentDate)) " +
              "else (datediff('d',date({{LG_SPH.d_spdate}}), currentdate) < {{LG_Cusmas.n_days}}))"),
            IsReportDirectValue = true
        });
    }
    //lstData.Clear();

    if (!string.IsNullOrEmpty(cbTipeSP.SelectedItem.Value))
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "LG_SPH.c_type",
            ParameterValue = cbTipeSP.SelectedItem.Value
        });
    }

    if (!string.IsNullOrEmpty(cbGudang.SelectedItem.Value))
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "LG_Cusmas.c_gdg",
            ParameterValue = cbGudang.SelectedItem.Value
        });
    }

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "({LG_SPD1.n_sisa} <> 0)",
      IsReportDirectValue = true
    });

    if (pag.IsCabang)
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "LG_SPH.c_cusno",
        ParameterValue = pag.Cabang,
      });
    }
    else
    {
      if (!string.IsNullOrEmpty(cbCustomer.SelectedItem.Value))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_SPH.c_cusno",
          ParameterValue = cbCustomer.SelectedItem.Value,
        });
      }
    }

    if (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "FA_Masitm.c_nosup",
        ParameterValue = cbSuplier.SelectedItem.Value
      });
    }

    if (!string.IsNullOrEmpty(cbItems.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "LG_SPD1.c_iteno",
        ParameterValue = cbItems.SelectedItem.Value
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
    //    ParameterName = string.Format("({{LG_SPH.c_cusno}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbCustomer.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "LG_SPH.c_cusno",
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
    //    ParameterName = string.Format("({{FA_Masitm.c_nosup}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbSuplier.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "FA_Masitm.c_nosup",
    //    ParameterValue = (cbSuplier.SelectedItems[0].Value == null ? string.Empty : cbSuplier.SelectedItems[0].Value)
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
    //    ParameterName = string.Format("({{LG_SPD1.c_iteno}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbItems.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "LG_SPD1.c_iteno",
    //    ParameterValue = (cbItems.SelectedItems[0].Value == null ? string.Empty : cbItems.SelectedItems[0].Value)
    //  });
    //}

    #endregion

    #endregion

    rptParse.PaperID = "Letter";
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
          string rptName = string.Concat("Pending_SP_", pag.Nip, ".", rptResult.Extension);

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
