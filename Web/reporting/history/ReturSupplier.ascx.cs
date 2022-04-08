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

public partial class reporting_history_RS : System.Web.UI.UserControl
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
        rptParse.ReportingID = "20030";
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
            DataType = typeof(char).FullName,
            ParameterName = "type",
            IsSqlParameter = true,
            ParameterValue = cbTipeTransaksi.SelectedItem.Value
        });


        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "NIP",
            IsSqlParameter = true,
            ParameterValue = pag.Nip
        });

        // hafizh

        

        //lstRptParam.Add(new ReportParameter()
        //{
        //    DataType = typeof(char).FullName,
        //    ParameterName = "gudang",
        //    IsSqlParameter = true,
        //    ParameterValue = cbGudang.SelectedItem.Value
        //});

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(char).FullName,
            ParameterName = "Nosup",
            IsSqlParameter = true,
            ParameterValue = cbSuplier.SelectedItem.Value == null ? "00000" : cbSuplier.SelectedItem.Value
        });


        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "Iteno",
            IsSqlParameter = true,
            //ParameterValue = cbItems.SelectedItem.Value
            ParameterValue = cbItems.SelectedItem.Value == null ? "0000" : cbItems.SelectedItem.Value
        });
              

 


        #endregion

        rptParse.ReportCustomizeText = lstCustTxt.ToArray();
        
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
                    string rptName = string.Concat("Report_Produktifitas_RS_", cbTipeTransaksi.SelectedItem.Text, "_", pag.Nip, ".", rptResult.Extension);

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
