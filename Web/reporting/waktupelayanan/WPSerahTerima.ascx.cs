using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;


public partial class reporting_waktupelayanan_WPSerahTerima : System.Web.UI.UserControl
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
        List<ReportCustomizeText> lstCustTxt = new List<ReportCustomizeText>();

        DateTime date1 = DateTime.Today,
          date2 = DateTime.Today;
        List<string> lstData = new List<string>();
        //string tmp = null;
        bool isAsync = false;

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

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "session",
            IsSqlParameter = true,
            ParameterValue = pag.Nip
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "gudang",
            IsSqlParameter = true,
            ParameterValue = cbGudang.SelectedItem.Value
        });
        #endregion

        #region Report Parameter

        string dateparam1 = string.Concat(date1.Year.ToString(), ",", date1.Month.ToString(), ",", date1.Day.ToString());
        string dateparam2 = string.Concat(date2.Year.ToString(), ",", date2.Month.ToString(), ",", date2.Day.ToString());
        string tipe = string.Empty;

        switch(cbTipeTransaksi.SelectedItem.Value)
        {
            case "01":
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = string.Format("({{LG_WP_ST.c_type}} = '{0}' OR {{LG_WP_ST.c_type}} = '{1}')", "01", "06"),
                    IsReportDirectValue = true
                });
                break;

            case "02":
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = string.Format("({{LG_WP_ST.c_type}} = '{0}' OR {{LG_WP_ST.c_type}} = '{1}')", "02", "07"),
                    IsReportDirectValue = true
                });
                break;

            case "03":
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = string.Format("({{LG_WP_ST.c_type}} = '{0}' OR {{LG_WP_ST.c_type}} = '{1}')", "03", "08"),
                    IsReportDirectValue = true
                });

                break;
            case "04":
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = string.Format("({{LG_WP_ST.c_type}} = '{0}' OR {{LG_WP_ST.c_type}} = '{1}')", "04", "09"),
                    IsReportDirectValue = true
                });
                break;

            case "05":
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = string.Format("({{LG_WP_ST.c_type}} = '{0}' OR {{LG_WP_ST.c_type}} = '{1}')", "05", "10"),
                    IsReportDirectValue = true
                });
                break;
            case "IJ":
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = string.Format("({{LG_WP_ST.c_type}} = '{0}' OR {{LG_WP_ST.c_type}} = '{1}')", "PL", "SJ"),
                    IsReportDirectValue = true
                });
                break;
            case "DO":
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = string.Format("({{LG_WP_ST.c_type}} = '{0}' OR {{LG_WP_ST.c_type}} = '{1}')", "DO", "DJ"),
                    IsReportDirectValue = true
                });
                break;
            case "06":
                tipe = "FULL";
                break;
        }

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "Tipe",
            IsSqlParameter = true,
            ParameterValue = tipe
        });

        if (string.IsNullOrEmpty(tipe))
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("({{LG_WP_ST.d_date2}} IN date({0}) to date({1}))", dateparam1, dateparam2),
                IsReportDirectValue = true
            });
        }
        else
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("({{Temp_LGWP_ST.c_user}} = '{0}')", pag.Nip),
                IsReportDirectValue = true
            });
        }

        if (cbGudang.SelectedItem.Value != null)
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.IsNullOrEmpty(tipe) 
                                ? string.Format("({{LG_WP_ST.c_gdg}} = '{0}')", cbGudang.SelectedItem.Value)
                                : string.Format("({{Temp_LGWP_ST.c_gdg}} = '{0}')", cbGudang.SelectedItem.Value),
                IsReportDirectValue = true
            });
        }
        if (cbCustomer.SelectedItem.Value != null)
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.IsNullOrEmpty(tipe)
                                ? string.Format("({{LG_WP_ST.c_cusno}} = '{0}')", cbCustomer.SelectedItem.Value)
                                : string.Format("({{Temp_LGWP_ST.c_cusno}} = '{0}')", cbCustomer.SelectedItem.Value),
                IsReportDirectValue = true
            });
        }
        if (cbSuplier.SelectedItem.Value != null)
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("({{FA_MasItm.c_nosup}} = '{0}')", cbSuplier.SelectedItem.Value),
                IsReportDirectValue = true
            });
        }
        if (cbDivPrinsipal.SelectedItem.Value != null)
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("({{FA_DivPri.c_kddivpri}} = '{0}')", cbDivPrinsipal.SelectedItem.Value),
                IsReportDirectValue = true
            });
        }
        if (cbItems.SelectedItem.Value != null)
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("({{FA_MasItm.c_iteno}} = '{0}')", cbItems.SelectedItem.Value),
                IsReportDirectValue = true
            });
        }

        lstData = rbgTipeReport.CheckedTags;
        string radiovalue = "01";
        if (rbgTRDtl.Checked)
        {
            radiovalue = "01";
        }
        else if (rbgTRSDP.Checked)
        {
            radiovalue = "02";
        }
        else if(rbgTRSC.Checked)
        {
            radiovalue = "03";
        }

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "TipeReport",
            IsSqlParameter = true,
            ParameterValue = radiovalue
        });

        rptParse.ReportingID = "20204";

        lstCustTxt.Add(new ReportCustomizeText()
        {
            SectionName = "Section2",
            ControlName = "txtPeriode",
            Value = string.Format("Periode : {0} s/d {1}", date1.ToString("dd-MMM-yyyy"), date2.ToString("dd-MMM-yyyy"))
        });

        lstCustTxt.Add(new ReportCustomizeText()
        {
            SectionName = "Section2",
            ControlName = "txtTitle",
            Value = string.Format("Laporan Waktu Pelayanan Serah Terima {0}", cbTipeTransaksi.SelectedItem.Text)
        });



        lstData.Clear();

        #endregion

        rptParse.PaperID = "Letter";
        rptParse.ReportCustomizeText = lstCustTxt.ToArray();
        rptParse.ReportParameter = lstRptParam.ToArray();
        rptParse.User = pag.Nip;
       

        rptParse.OutputReport = ReportParser.ParsingOutputReport((cbRptTypeOutput.SelectedItem != null ? cbRptTypeOutput.SelectedItem.Value : string.Empty));
        rptParse.IsShared = false; //chkShare.Checked;
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
                    string rptName = string.Concat("Report_Waktu_Pelayanan_Serah_Terima_",cbTipeTransaksi.SelectedItem.Text,"_", pag.Nip, ".", rptResult.Extension);

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
