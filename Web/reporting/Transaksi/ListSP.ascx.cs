using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_Transaksi_ListSP : System.Web.UI.UserControl
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
    bool isAsync = false;

    rptParse.ReportingID = "21017";

    isAsync = chkAsync.Checked;

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
          ParameterName = string.Format("({{LG_SPH.d_spdate}} IN {0} to {1})", Functional.CrystalReportDateString(date1), Functional.CrystalReportDateString(date2)),
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

    if (txSPHO1.Text.Length > 0)
    {
        if (txSPHO2.Text.Length > 0)
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("({{LG_SPH.c_spno}} IN ['{0}','{1}'])", txSPHO1.Text, txSPHO2.Text),
                IsReportDirectValue = true
            });
        }
        else
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("({{LG_SPH.c_spno}} >= '{0}')", txSPHO1.Text),
                IsReportDirectValue = true
            });
        }
    }
    else if (txSPHO2.Text.Length > 0)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("(({{LG_SPH.c_spno}} <= '{0}')", txSPHO2.Text),
            IsReportDirectValue = true
        });
    }

    #region old code
    //if (txSPHO1.Text.Length > 0)
    //{
    //    if (txSPHO2.Text.Length > 0)
    //  {
    //    lstRptParam.Add(new ReportParameter()
    //    {
    //      DataType = typeof(string).FullName,
    //      ParameterName = string.Format("({{LG_SPH.c_type}} IN '01' to '02') AND ({{LG_SPH.c_spno}} IN ['{0}','{1}'])", txSPHO1.Text, txSPHO2.Text),
    //      IsReportDirectValue = true
    //    });
    //  }
    //  else
    //  {
    //    lstRptParam.Add(new ReportParameter()
    //    {
    //      DataType = typeof(string).FullName,
    //      ParameterName = string.Format("({{LG_SPH.c_type}} IN '01' to '02') AND ({{LG_SPH.c_spno}} >= '{0}')", txSPHO1.Text),
    //      IsReportDirectValue = true
    //    });
    //  }
    //}
    //else if (txSPHO2.Text.Length > 0)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = string.Format("({{LG_SPH.c_type}} IN '01' to '02') AND ({{LG_SPH.c_spno}} <= '{0}')", txSPHO2.Text),
    //    IsReportDirectValue = true
    //  });
    //}

    //if (txSPB1.Text.Length > 0)
    //{
    //    if (txSPB2.Text.Length > 0)
    //    {
    //        lstRptParam.Add(new ReportParameter()
    //        {
    //            DataType = typeof(string).FullName,
    //            ParameterName = string.Format("({{LG_SPH.c_type}} IN '03' to '04') AND ({{LG_SPH.c_spno}} IN ['{0}','{1}'])", txSPB1.Text, txSPB2.Text),
    //            IsReportDirectValue = true
    //        });
    //    }
    //    else
    //    {
    //        lstRptParam.Add(new ReportParameter()
    //        {
    //            DataType = typeof(string).FullName,
    //            ParameterName = string.Format("({{LG_SPH.c_type}} IN '03' to '04') AND ({{LG_SPH.c_spno}} >= '{0}')", txSPB1.Text),
    //            IsReportDirectValue = true
    //        });
    //    }
    //}
    //else if (txSPB2.Text.Length > 0)
    //{
    //    lstRptParam.Add(new ReportParameter()
    //    {
    //        DataType = typeof(string).FullName,
    //        ParameterName = string.Format("({{LG_SPH.c_type}} IN '03' to '04') AND ({{LG_SPH.c_spno}} <= '{0}')", txSPB2.Text),
    //        IsReportDirectValue = true
    //    });
    //}
    #endregion

    if (!string.IsNullOrEmpty(cbCustomer.SelectedItem.Value))
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "lg_sph.c_cusno",
            ParameterValue = cbCustomer.SelectedItem.Value
        });
    }

    if (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "LG_DatSup.c_nosup",
        ParameterValue = cbSuplier.SelectedItem.Value
      });
    }

    if (!string.IsNullOrEmpty(cbDivPrinsipal.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "FA_MsDivPri.c_kddivpri",
        ParameterValue = cbDivPrinsipal.SelectedItem.Value
      });
    }

    if (!string.IsNullOrEmpty(cbItems.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "lg_spd1.c_iteno",
        ParameterValue = cbItems.SelectedItem.Value
      });
    }

    if (rdAll.Checked == false)
    {
      if (rdSPO.Checked == true)
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "({lg_sph.c_type} = '01')",
          IsReportDirectValue = true
        });
      }
      else if(rdSPM.Checked == true)
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "({lg_sph.c_type} = '02')",
          IsReportDirectValue = true
        });
      }
      else if (rdSPHO.Checked == true)
      {
          lstRptParam.Add(new ReportParameter()
          {
              DataType = typeof(string).FullName,
              ParameterName = "({lg_sph.c_type} = '03')",
              IsReportDirectValue = true
          });
      }
      else if (rdSPAdj.Checked == true)
      {
          lstRptParam.Add(new ReportParameter()
          {
              DataType = typeof(string).FullName,
              ParameterName = "({lg_sph.c_type} = '04')",
              IsReportDirectValue = true
          });
      }
      // Indra 20170516
      else if (rdSPDrop.Checked == true)
      {
          lstRptParam.Add(new ReportParameter()
          {
              DataType = typeof(string).FullName,
              ParameterName = "({lg_sph.c_type} = '05')",
              IsReportDirectValue = true
          });
      }
    }

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
          string rptName = string.Concat("List_Transaksi_SP_", pag.Nip, ".", rptResult.Extension);

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
