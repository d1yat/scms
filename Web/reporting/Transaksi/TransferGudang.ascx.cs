using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_Transaksi_TransferGudang : System.Web.UI.UserControl
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

    rptParse.ReportingID = "21008";

    isAsync = chkAsync.Checked;

    tmp = txSJ1.Text.Trim();
    tmp1 = txSJ2.Text.Trim();

//lstData = rdgTipeSJ.CheckedTags;

if (rdPendingRN.Checked == true)
{

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
    }


            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(DateTime).FullName,
                ParameterName = "Date1",
                IsSqlParameter = true,
                ParameterValue = date1.ToString("d-M-yyyy")
            });

            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(DateTime).FullName,
                ParameterName = "Date2",
                IsSqlParameter = true,
                ParameterValue = date2.ToString("d-M-yyyy")
            });



            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = "Nosup",
                IsSqlParameter = true,
                ParameterValue = string.IsNullOrEmpty(cbSuplier.SelectedItem.Value) ? "00000" : cbSuplier.SelectedItem.Value
            });

            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = "Iteno",
                IsSqlParameter = true,
                ParameterValue = string.IsNullOrEmpty(cbItems.SelectedItem.Value) ? "0000" : cbItems.SelectedItem.Value
            });



            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(char).FullName,
                ParameterName = "gdg1",
                IsSqlParameter = true,
                ParameterValue = string.IsNullOrEmpty(cbGudang.SelectedItem.Value) ? "0" : cbGudang.SelectedItem.Value
            });

            lstRptParam.Add(new ReportParameter()
            {

                DataType = typeof(char).FullName,
                ParameterName = "gdg2",
                IsSqlParameter = true,
                ParameterValue = string.IsNullOrEmpty(cbGudangTo.SelectedItem.Value) ? "0" : cbGudangTo.SelectedItem.Value
            });


            if (rdPendingRN.Checked == true)
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "NIP",
                    IsSqlParameter = true,
                    ParameterValue = pag.Nip
                });

            }

        
}

    #endregion


else
{
   #region Report Parameter

    if (!string.IsNullOrEmpty(cbGudang.SelectedItem.Value))
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "LG_SJH.c_gdg",
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
                ParameterName = string.Format("({{LG_SJH.d_sjdate}} IN {0} to {1})", Functional.CrystalReportDateString(date1), Functional.CrystalReportDateString(date2)),
                IsReportDirectValue = true
            });
        }
        else if (!date1.Equals(DateTime.MinValue))
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = "LG_SJH.d_sjdate",
                ParameterValue = date1.ToString("yyyy-MM-dd")
            });
        }
    }

    if (txSJ1.Text.Length > 0)
    {
        if (txSJ2.Text.Length > 0)
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("({{LG_SJH.c_sjno}} IN ['{0}','{1}'])", txSJ1.Text, txSJ2.Text),
                IsReportDirectValue = true
            });
        }
        else
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("({{LG_SJH.c_sjno}} >= '{0}')", txSJ1.Text),
                IsReportDirectValue = true
            });
        }
    }
    else if (txSJ2.Text.Length > 0)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{LG_SJH.c_sjno}} <= '{0}')", txSJ2.Text),
            IsReportDirectValue = true
        });
    }

    if (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value))
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "LG_DatSup.c_nosup",
            ParameterValue = (cbSuplier.SelectedItem.Value.ToString() == null ? string.Empty : cbSuplier.SelectedItem.Value.ToString())
        });
    }

    if (!string.IsNullOrEmpty(cbDivPrinsipal.SelectedItem.Value))
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "FA_DivPri.c_kddivpri",
            ParameterValue = (cbDivPrinsipal.SelectedItem.Value.ToString() == null ? string.Empty : cbDivPrinsipal.SelectedItem.Value.ToString())
        });
    }

    if (!string.IsNullOrEmpty(cbItems.SelectedItem.Value))
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "LG_SJD1.c_iteno",
            ParameterValue = (cbItems.SelectedItem.Value.ToString() == null ? string.Empty : cbItems.SelectedItem.Value.ToString())
        });
    }


    if (rdAll.Checked == false)
    {
        if (rdSPG.Checked == true)
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = "({LG_SJH.c_type} = '01')",
                IsReportDirectValue = true
            });
        }
        else if (rdPindah.Checked == true)
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = "({LG_SJH.c_type} = '02')",
                IsReportDirectValue = true
            });
        }
        else if (rdClaimBonus.Checked == true)
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = "({LG_SJH.c_type} = '03')",
                IsReportDirectValue = true
            });
        }
    }



    #endregion

}           



    

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
        
          string rptName = null;

          if (rdPendingRN.Checked)
          {
              rptName = string.Concat("Transaksi_PendingRN_", pag.Nip, ".", rptResult.Extension);

          }

          else
          {
              rptName = string.Concat("Transaksi_SJ_", pag.Nip, ".", rptResult.Extension);

          }



          string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
          tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
          tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

          //wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
          Functional.GeneratorLoadedWindow(hidWndDown.Text, tmpUri, LoadMode.IFrame);

        
      }
      else
      {
        Functional.ShowMsgWarning(rptResult.MessageResponse);
      }
    }

    GC.Collect();
  }
}
