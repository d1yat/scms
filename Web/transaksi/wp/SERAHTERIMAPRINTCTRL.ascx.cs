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
//using System.Xml.Linq;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using System.Collections.Generic;
using Scms.Web.Core;
using Ext.Net;

public partial class TRANSAKSI_WP_SERAHTERIMAPRINTCTRL : System.Web.UI.UserControl
{

    public void InitializePage(string Type)
    {
        hfType.Text = Type;
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        //if (!Functional.IsAllowView(this.Page as Scms.Web.Core.PageHandler))
        //{
        //  return;
        //}
    }

    public void ShowPrintPage()
    {
        winPrintDetail.Title = "Cetak Serah Terima";

        winPrintDetail.Hidden = false;
        winPrintDetail.ShowModal();
    }

    [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void Report_OnGenerate(object sender, DirectEventArgs e)
    {
        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        if (!pag.IsAllowPrinting)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
            return;
        }

        //string gdgId = pag.ActiveGudang;
        string custId = (e.ExtraParams["CustomeID"] ?? string.Empty);       
        string tmp = (e.ExtraParams["Async"] ?? string.Empty);

       
        ReportParser rptParse = new ReportParser();

        List<ReportParameter> lstRptParam = new List<ReportParameter>();

        List<string> lstData = new List<string>();
        bool isAsync = false;

        bool.TryParse(tmp, out isAsync);

        rptParse.ReportingID = "10122";

       
        DateTime date1 = DateTime.Today,
          date2 = DateTime.Today;

        
        
        isAsync = chkAsync.Checked;

        //if (Scms.Web.Common.Functional.DateParser(txPeriode1.RawText.Trim(), "d-M-yyyy", out date1))
        //{
        //    if (Scms.Web.Common.Functional.DateParser(txPeriode2.RawText.Trim(), "d-M-yyyy", out date2))
        //    {
        //        if (date2.CompareTo(date1) <= 0)
        //        {
        //            date2 = date1;
        //        }
                
        //    }
        //    else
        //    {
        //        date2 = date1;
        //    }
        //}

        //#region linq filter parameter


        //lstrptparam.add(new reportparameter()
        //{
        //    datatype = typeof(string).fullname,
        //    parametername = "c_cusno = @0",
        //    parametervalue = (string.isnullorempty(custid) ? string.empty : custid),
        //    islinqfilterparameter = true
        //});

        //if (!string.isnullorempty(pl1))
        //{
        //    if (string.isnullorempty(pl2))
        //    {
        //        lstrptparam.add(new reportparameter()
        //        {
        //            datatype = typeof(string).fullname,
        //            parametername = "d_date = @0",
        //            parametervalue = (string.isnullorempty(date1.tostring("yyyymmdd")) ? string.empty : date1.tostring("yyyymmdd")),
        //            islinqfilterparameter = true
        //        });
        //    }
        //    else
        //    {
        //        if (pl1.compareto(pl2) >= 0)
        //        {
        //            lstrptparam.add(new reportparameter()
        //            {
        //                datatype = typeof(string).fullname,
        //                parametername = "d_date = @0",
        //                parametervalue = (string.isnullorempty(date1.tostring("yyyymmdd")) ? string.empty : date1.tostring("yyyymmdd")),
        //                islinqfilterparameter = true
        //            });
        //        }
        //        else
        //        {
        //            lstrptparam.add(new reportparameter()
        //            {
        //                datatype = typeof(string).fullname,
        //                parametername = "d_date",
        //                parametervalue = (string.isnullorempty(date1.tostring("yyyymmdd")) ? string.empty : date1.tostring("yyyymmdd")),
        //                islinqfilterparameter = true,
        //                betweenvalue = (string.isnullorempty(date2.tostring("yyyymmdd")) ? string.empty : date2.tostring("yyyymmdd"))
        //            });
        //        }
        //    }
        //}

        //#endregion


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
            DataType = typeof(string).FullName,
            ParameterName = "cusno",
            IsSqlParameter = true,
            ParameterValue = string.IsNullOrEmpty(cbCustomer.SelectedItem.Value) ? "0000" : cbCustomer.SelectedItem.Value
        });


        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(char).FullName,
            ParameterName = "Type",
            IsSqlParameter = true,
            ParameterValue = hfType.Text


        });


        //lstRptParam.Add(new ReportParameter()
        //{
        //    DataType = typeof(char).FullName,
        //    ParameterName = "user",
        //    IsSqlParameter = true,
        //    ParameterValue = pag.Nip
        //});

       
        #endregion


        //#region Report Parameter

        //lstRptParam.Add(new ReportParameter()
        //{
        //    DataType = typeof(string).FullName,
        //    ParameterName = "scms_std.c_cusno",
        //    ParameterValue = (string.IsNullOrEmpty(custId) ? string.Empty : custId)
        //});

        //#endregion

        rptParse.PaperID = "8.5x5.5";
        rptParse.ReportParameter = lstRptParam.ToArray();
        rptParse.User = pag.Nip;
        rptParse.OutputReport = ReportParser.ParsingOutputReport((cbRptTypeOutput.SelectedItem != null ? cbRptTypeOutput.SelectedItem.Value : string.Empty));

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
               

                    string rptName = null;

                    if (hfType.Text == "05")
                    {
                        rptName = string.Concat("Serah_Terima_Gudang", pag.Nip, ".", rptResult.Extension);
                        
                    }

                    else if (hfType.Text == "06")
                    {
                        rptName = string.Concat("Serah_Terima_Admin", pag.Nip, ".", rptResult.Extension);
                        
                    }
                      

                    string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
                    tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
                    tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);
                    Functional.UriDownloadGenerator(pag, rptResult.OutputFile, rptName, rptResult.Extension);

                    wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
                }
            
            else
            {
                Functional.ShowMsgWarning(rptResult.MessageResponse);
            }
        }

        GC.Collect();
    }



}
