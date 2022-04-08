using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using System.Collections.Generic;
using Scms.Web.Core;
using Ext.Net;

public partial class REPORTING_MONITORING_MONITORINGPRODUKTIFITASDC : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void Initialize(string wndDownload, string gudang)
    {
        hidWndDown.Text = wndDownload;
        hfGdg.Text = gudang;
    }

    protected void Report_OnGenerate(object sender, DirectEventArgs e)
    {
        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        if (!Functional.CanCreateGenerateReport(pag))
        {
            return;
        }
       
        ReportParser rptParse = new ReportParser();
        rptParse.ReportingID = "21054";
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
            ParameterName = "NIP",
            IsSqlParameter = true,
            ParameterValue = pag.Nip
        });

        // hafizh

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(char).FullName,
            ParameterName = "type",
            IsSqlParameter = true,
            ParameterValue = cbTipeTransaksi.SelectedItem.Value
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(char).FullName,
            ParameterName = "gudang",
            IsSqlParameter = true,
            ParameterValue = cbGudang.SelectedItem.Value
        });
        #endregion

        //#region Report Parameter

        //string dateparam1 = string.Concat(date1.Year.ToString(), ",", date1.Month.ToString(), ",", date1.Day.ToString());
        //string dateparam2 = string.Concat(date2.Year.ToString(), ",", date2.Month.ToString(), ",", date2.Day.ToString());
        //string tipe = string.Empty;

        //switch (cbTipeTransaksi.SelectedItem.Value)
        //{
        //    //case "01":
        //    //    lstRptParam.Add(new ReportParameter()
        //    //    {
        //    //        DataType = typeof(string).FullName,
        //    //        ParameterName = string.Format("({{LG_WP_ST.c_type}} = '{0}' OR {{LG_WP_ST.c_type}} = '{1}')", "01", "06"),
        //    //        IsReportDirectValue = true
        //    //    });
        //    //    break;

        //    case "02":
        //        lstRptParam.Add(new ReportParameter()
        //        {
        //            DataType = typeof(string).FullName,
        //            //ParameterName = string.Format("({{LG_WP_ST.c_type}} = '{0}' OR {{LG_WP_ST.c_type}} = '{1}')", "02", "07"),
        //            ParameterName = string.Format("({{LG_RptProdDC.@type}} = '{0}' OR {{LG_RptProdDC.@type}} = '{1}')", "02"),
        //            IsReportDirectValue = true
        //        });
        //        break;

        //    case "03":
        //        lstRptParam.Add(new ReportParameter()
        //        {
        //            DataType = typeof(string).FullName,
        //            //ParameterName = string.Format("({{LG_WP_ST.c_type}} = '{0}' OR {{LG_WP_ST.c_type}} = '{1}')", "03", "08"),
        //            ParameterName = string.Format("({{LG_RptProdDC.@type}} = '{0}' OR {{LG_RptProdDC.@type}} = '{1}')", "03"),
        //            IsReportDirectValue = true
        //        });

        //        break;
        //    case "04":
        //        lstRptParam.Add(new ReportParameter()
        //        {
        //            DataType = typeof(string).FullName,
        //            //ParameterName = string.Format("({{LG_WP_ST.c_type}} = '{0}' OR {{LG_WP_ST.c_type}} = '{1}')", "04", "09"),
        //            ParameterName = string.Format("({{LG_RptProdDC.@type}} = '{0}' OR {{LG_RptProdDC.@type}} = '{1}')", "04"),
        //            IsReportDirectValue = true
        //        });
        //        break;

        //    case "05":
        //        lstRptParam.Add(new ReportParameter()
        //        {
        //            DataType = typeof(string).FullName,
        //            //ParameterName = string.Format("({{LG_WP_ST.c_type}} = '{0}' OR {{LG_WP_ST.c_type}} = '{1}')", "05", "10"),
        //            ParameterName = string.Format("({{LG_RptProdDC.@type}} = '{0}' OR {{LG_RptProdDC.@type}} = '{1}')", "05"),
        //            IsReportDirectValue = true
        //        });
        //        break;
        //    //case "IJ":
        //    //    lstRptParam.Add(new ReportParameter()
        //    //    {
        //    //        DataType = typeof(string).FullName,
        //    //        ParameterName = string.Format("({{LG_WP_ST.c_type}} = '{0}' OR {{LG_WP_ST.c_type}} = '{1}')", "PL", "SJ"),
        //    //        IsReportDirectValue = true
        //    //    });
        //    //    break;
        //    case "DO":
        //        lstRptParam.Add(new ReportParameter()
        //        {
        //            DataType = typeof(string).FullName,
        //            //ParameterName = string.Format("({{LG_WP_ST.c_type}} = '{0}' OR {{LG_WP_ST.c_type}} = '{1}')", "DO", "DJ"),
        //            ParameterName = string.Format("({{LG_RptProdDC.@type}} = '{0}' OR {{LG_RptProdDC.@type}} = '{1}')", "DO"),
        //            IsReportDirectValue = true
        //        });
        //        break;
        //    //case "06":
        //    //    tipe = "FULL";
        //    //    break;
        //}

        //lstRptParam.Add(new ReportParameter()
        //{
        //    DataType = typeof(string).FullName,
        //    ParameterName = "Tipe",
        //    IsSqlParameter = true,
        //    ParameterValue = tipe
        //});

        //if (string.IsNullOrEmpty(tipe))
        //{
        //    lstRptParam.Add(new ReportParameter()
        //    {
        //        DataType = typeof(string).FullName,
        //        ParameterName = string.Format("({{LG_WP_ST.d_date2}} IN date({0}) to date({1}))", dateparam1, dateparam2),
        //        IsReportDirectValue = true
        //    });
        //}
        //else
        //{
        //    lstRptParam.Add(new ReportParameter()
        //    {
        //        DataType = typeof(string).FullName,
        //        ParameterName = string.Format("({{Temp_LGWP_ST.c_user}} = '{0}')", pag.Nip),
        //        IsReportDirectValue = true
        //    });
        //}

        //if (cbGudang.SelectedItem.Value != null)
        //{
        //    lstRptParam.Add(new ReportParameter()
        //    {
        //        DataType = typeof(string).FullName,
        //        ParameterName = string.IsNullOrEmpty(tipe)
        //                        ? string.Format("({{LG_WP_ST.c_gdg}} = '{0}')", cbGudang.SelectedItem.Value)
        //                        : string.Format("({{Temp_LGWP_ST.c_gdg}} = '{0}')", cbGudang.SelectedItem.Value),
        //        IsReportDirectValue = true
        //    });
        //}


        

        //lstCustTxt.Add(new ReportCustomizeText()
        //{
        //    SectionName = "Section2",
        //    ControlName = "txtPeriode",
        //    Value = string.Format("Periode : {0} s/d {1}", date1.ToString("dd-MMM-yyyy"), date2.ToString("dd-MMM-yyyy"))
        //});

       
        //lstData.Clear();

        //#endregion
        
        rptParse.PaperID = "Letter";
        rptParse.IsLandscape = true;
        
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
                    string rptName = string.Concat("Report_Produktifitas_DC_", cbTipeTransaksi.SelectedItem.Text, "_", pag.Nip, ".", rptResult.Extension);

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
