using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using System.Text;

public partial class proses_wp : Scms.Web.Core.PageHandler
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            DateTime date = DateTime.Now;

            Functional.PopulateBulan(cbBulan, date.Month);

            Functional.PopulateTahun(cbTahun, date.Year, 1, 0);
        }
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
        bool isAsync = false;
        isAsync = chkAsync.Checked;
  

        #region Sql Parameter

        lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(String).FullName,
                ParameterName = "tahun",
                IsSqlParameter = true,
                ParameterValue = cbTahun.SelectedItem.Text.ToString()
            });
        lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(String).FullName,
                ParameterName = "bulan",
                IsSqlParameter = true,
                ParameterValue = cbBulan.SelectedItem.Value
            });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(String).FullName,
            ParameterName = "tipeProses",
            IsSqlParameter = true,
            ParameterValue = cbTipe.SelectedItem.Value
        });


        #endregion

        rptParse.ReportingID = "20206";
        rptParse.ReportCustomizeText = lstCustTxt.ToArray();
        rptParse.ReportParameter = lstRptParam.ToArray();


        string xmlFiles = ReportParser.Deserialize(rptParse);

        SoaReportCaller soa = new SoaReportCaller();

        string result = soa.GeneratorReport(isAsync, xmlFiles);

        ReportingResult rptResult = ReportingResult.Serialize(result);

        if (string.IsNullOrEmpty(result))
        {
            e.ErrorMessage = "Unknown response";

            e.Success = false;
        }
        else
        {
            {
                Functional.ShowMsgInformation("Proses Sedang dijalankan, harap tunggu 3 menit sebelum menarik report WP");
            }
        }

        GC.Collect();
    }
}
