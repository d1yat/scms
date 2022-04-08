using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_pending_SuratJalan : System.Web.UI.UserControl
{
  public void InitializePage(string wndDownload)
  {
    hidWndDown.Text = wndDownload;
  }

  protected void Page_Load(object sender, EventArgs e)
  {
      rdgStatusSJ.Disabled = true;
  }

  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();
    List<ReportCustomizeText> lstCustTxt = new List<ReportCustomizeText>(); //Indra 20170809

    string TipeReport, KategoriReport; //Indra 20170809
    DateTime date1 = DateTime.Today,
      date2 = DateTime.Today;
    List<string> lstData = new List<string>();
    string tmp = null;
    bool isAsync = false;
    
    isAsync = chkAsync.Checked;

    rptParse.ReportingID = "20313";
    
    #region Report Parameter old mark by Indra 20170809

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
    //      ParameterName = string.Format("({{LG_SJH.d_sjdate}} In {0} To {1})",
    //          Functional.CrystalReportDateString(date1), Functional.CrystalReportDateString(date2)),
    //      IsReportDirectValue = true
    //    });
    //  }
    //  else if (!date1.Equals(DateTime.MinValue))
    //  {
    //    lstRptParam.Add(new ReportParameter()
    //    {
    //      DataType = typeof(string).FullName,
    //      ParameterName = "LG_SJH.d_sjdate",
    //      IsReportDirectValue = true,
    //      ParameterValue = Functional.CrystalReportDateString(date1)
    //    });
    //  }
    //}

    //lstRptParam.Add(new ReportParameter()
    //{
    //  DataType = typeof(string).FullName,
    //  ParameterName = "LG_SJH.c_gdg",
    //  ParameterValue = (cbGudangFrom.SelectedItem.Value == null ? string.Empty : cbGudangFrom.SelectedItem.Value)
    //});

    //lstRptParam.Add(new ReportParameter()
    //{
    //  DataType = typeof(string).FullName,
    //  ParameterName = "LG_SJH.c_gdg2",
    //  ParameterValue = (cbGudangTo.SelectedItem.Value == null ? string.Empty : cbGudangTo.SelectedItem.Value)
    //});

    //if (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value))
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "fa_masitm.c_nosup",
    //    ParameterValue = (cbSuplier.SelectedItem.Value.ToString() == null ? string.Empty : cbSuplier.SelectedItem.Value.ToString())
    //  });
    //}

    #region Old Coded

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
    //    ParameterName = string.Format("({{fa_masitm.c_nosup}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbSuplier.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "fa_masitm.c_nosup",
    //    ParameterValue = (cbSuplier.SelectedItems[0].Value == null ? string.Empty : cbSuplier.SelectedItems[0].Value)
    //  });
    //}

    #endregion

      //if (rdgConf.Checked == true)
      //{
      //  lstRptParam.Add(new ReportParameter()
      //  {
      //    DataType = typeof(bool).FullName,
      //    ParameterName = "LG_SJH.l_confirm",
      //    ParameterValue = "true"
      //  });
      //}
      //else if (rdgUnConf.Checked == true)
      //{
      //  lstRptParam.Add(new ReportParameter()
      //  {
      //    DataType = typeof(bool).FullName,
      //    ParameterName = "LG_SJH.l_confirm",
      //    ParameterValue = "false"
      //  });
      //}

    #endregion

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
        DataType = typeof(DateTime).FullName,
        ParameterName = "GdgAsal",
        IsSqlParameter = true,
        ParameterValue = cbGudangFrom.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(DateTime).FullName,
        ParameterName = "GdgTujuan",
        IsSqlParameter = true,
        ParameterValue = cbGudangTo.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(DateTime).FullName,
        ParameterName = "Pemasok",
        IsSqlParameter = true,
        ParameterValue = cbSuplier.SelectedItem.Value == null ? "00000" : cbSuplier.SelectedItem.Value
    });

    if (rdgAll.Checked)
    {
        TipeReport = "1";
        KategoriReport = "Semua Tipe Report Surat Jalan";
    }
    else if (rdgConf.Checked)
    {
        if (rdgSelectAll.Checked)
        {
            TipeReport = "2";
            KategoriReport = "Tipe Report Konfirm, SJ Sudah jadi EP dan Belum jadi EP";
        }
        else if (rdgNonEP.Checked)
        {
            TipeReport = "3";
            KategoriReport = "Tipe Report Konfirm, SJ Sudah jadi EP";
        }
        else
        {
            TipeReport = "4";
            KategoriReport = "Tipe Report Konfirm, SJ Sudah Belum jadi EP";
        }
    }
    else
    {
        TipeReport = "5";
        KategoriReport = "Surat Jalan Belum Konfirmasi";
    }

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(DateTime).FullName,
        ParameterName = "tipe",
        IsSqlParameter = true,
        ParameterValue = TipeReport
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "user",
        IsSqlParameter = true,
        ParameterValue = pag.Nip
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "TEMP_PENDINGSJ.C_ENTRY",
        ParameterValue = pag.Nip
    });

    lstCustTxt.Add(new ReportCustomizeText()
    {
        SectionName = "Section1",
        ControlName = "txtPeriode",
        Value = string.Format(": {0} {1}", date1.ToString("yyyy-M-d") + " s/d ", date2.ToString("yyyy-M-d"))
    });

    lstCustTxt.Add(new ReportCustomizeText()
    {
        SectionName = "Section2",
        ControlName = "txtGudangAsal",
        Value = string.Format(": Gudang {0} {1}", cbGudangFrom.SelectedItem.Value, "")
    });

    lstCustTxt.Add(new ReportCustomizeText()
    {
        SectionName = "Section3",
        ControlName = "txtGudangTujuan",
        Value = string.Format(": Gudang {0} {1}", cbGudangTo.SelectedItem.Value, "")
    });

    lstCustTxt.Add(new ReportCustomizeText()
    {
        SectionName = "Section4",
        ControlName = "txtPemasok",
        Value = string.Format(": {0} {1}", cbSuplier.SelectedItem.Value == null ? "Semua Pemasok" : cbSuplier.SelectedItem.Value + " - " + cbSuplier.SelectedItem.Text, "")
    });

    lstCustTxt.Add(new ReportCustomizeText()
    {
        SectionName = "Section5",
        ControlName = "txtTipeReport",
        Value = string.Format(": {0} {1}", KategoriReport, "")
    });

    #endregion

    rptParse.IsLandscape = true;
    rptParse.PaperID = "A3";
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
          string rptName = string.Concat("Pending_SuratJalan_", pag.Nip, ".", rptResult.Extension);

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
