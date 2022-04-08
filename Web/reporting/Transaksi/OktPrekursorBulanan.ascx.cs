using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Ext.Net;

public partial class reporting_Transaksi_OktPrekursorBulanan : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        DateTime date = DateTime.Now;

        Functional.PopulateTahun(cbTahun, date.Year, 1, 0);
        
      DateTime date2 = DateTime.Today.AddMonths(-1);
      int bln = date2.Month;
      cbBulan.SelectedIndex = bln-1;
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

        List<ReportParameter> lstRptParam = new List<ReportParameter>();

        DateTime date1 = DateTime.Today;

        List<string> lstData = new List<string>();
        bool isAsync = false;

        if (cbPODO.SelectedItem.Value == "01")
        {
            rptParse.ReportingID = "21018";
        }
        else if (cbPODO.SelectedItem.Value == "02")
        {
            rptParse.ReportingID = "21016";
        }

        isAsync = false;

        #region Report Parameter

        switch (cbPODO.SelectedItem.Value)
        {
            case "01":
                if (cbJenis.SelectedIndex == 0)
                {
                    lstRptParam.Add(new ReportParameter()
                    {
                        DataType = typeof(string).FullName,
                        ParameterName = string.Format("({{FA_MASITM.c_type}} = '{0}') AND ({{LG_POH.c_nosup}} in ['{1}','{2}','{3}','{4}'])", "02", "00019", "00001", "00029", "00085"),
                        IsReportDirectValue = true
                    });
                }
                else if (cbJenis.SelectedIndex == 1)
                {
                    lstRptParam.Add(new ReportParameter()
                    {
                        DataType = typeof(string).FullName,
                        //ParameterName = string.Format("({{SCMS_MSITEM_CAT.c_type}} = '{0}') AND ({{LG_POH.c_nosup}} in ['{1}','{2}','{3}','{4}'])", "07", "00019", "00060", "00029", "00135"),
                          ParameterName = string.Format("({{SCMS_MSITEM_CAT.c_type}} = '{0}')", "07"),
                        IsReportDirectValue = true
                    });
                }


                else if (cbJenis.SelectedIndex == 2)
                {
                    lstRptParam.Add(new ReportParameter()
                    {
                        DataType = typeof(string).FullName,
                        ParameterName = string.Format("({{SCMS_MSITEM_CAT.c_type}} = '{0}') ", "09"),
                        IsReportDirectValue = true
                    });
                }

                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = string.Format("(year(cDate({{LG_POH.d_podate}})) = {0} and month(cDate({{LG_POH.d_podate}})) = {1})", cbTahun.Text, cbBulan.Text),
                    IsReportDirectValue = true
                });
            break;
            case "02":
                if (cbJenis.SelectedIndex == 0)
                {
                 lstRptParam.Add(new ReportParameter()
                 {
                     DataType = typeof(string).FullName,
                     ParameterName = string.Format("({{FA_MASITM.c_type}} = '{0}')", "02"),
                     IsReportDirectValue = true
                 });
                }
                else if (cbJenis.SelectedIndex == 1)
                {
                    lstRptParam.Add(new ReportParameter()
                    {
                        DataType = typeof(string).FullName,
                        ParameterName = string.Format("({{SCMS_MSITEM_CAT.c_type}} = '{0}')", "07"),
                        IsReportDirectValue = true
                    });
                }

                else if (cbJenis.SelectedIndex == 2)
                {
                    lstRptParam.Add(new ReportParameter()
                    {
                        DataType = typeof(string).FullName,
                        ParameterName = string.Format("({{SCMS_MSITEM_CAT.c_type}} = '{0}') ", "09"),
                        IsReportDirectValue = true
                    });
                }

                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = string.Format("(year(cDate({{LG_SPH.d_spdate}})) = {0} and month(cDate({{LG_SPH.d_spdate}})) = {1})", cbTahun.Text, cbBulan.Text),
                    IsReportDirectValue = true
                });
             break;
            
        }
       
        

        #endregion

        rptParse.PaperID = "Letter";
        rptParse.ReportCustomizeText = null;
        rptParse.ReportParameter = lstRptParam.ToArray();
        rptParse.User = pag.Nip;
        rptParse.IsLandscape = true;

        rptParse.OutputReport = ReportParser.ParsingOutputReport((cbRptTypeOutput.SelectedItem != null ? cbRptTypeOutput.SelectedItem.Value : string.Empty));
        rptParse.IsShared = false; //chkShare.Checked;

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
                else
                {
                    string rptName = "";
                    DateTime daterpt = DateTime.Today.AddMonths(-1);
                    if (cbPODO.SelectedItem.Value == "01")
                    {
                        if (cbJenis.SelectedIndex == 0)
                        {
                            rptName = string.Concat("Report_PO_Okt_", cbBulan.Text, "_", cbTahun.Text, ".", rptResult.Extension);
                        }
                        else if (cbJenis.SelectedIndex == 1)
                        {
                            rptName = string.Concat("Report_PO_Prekursor_", cbBulan.Text, "_", cbTahun.Text, ".", rptResult.Extension);
                        }
                        else if (cbJenis.SelectedIndex == 2)
                        {
                            rptName = string.Concat("Report_PO_OOT_", cbBulan.Text, "_", cbTahun.Text, ".", rptResult.Extension);
                        }

                    }
                    else if (cbPODO.SelectedItem.Value == "02")
                    {
                        if (cbJenis.SelectedIndex == 0)
                        {
                            rptName = string.Concat("Report_DO_Okt_", cbBulan.Text, "_", cbTahun.Text, ".", rptResult.Extension);
                        }
                        else if (cbJenis.SelectedIndex == 1)
                        {
                            rptName = string.Concat("Report_DO_Prekursor_", cbBulan.Text, "_", cbTahun.Text, ".", rptResult.Extension);
                        }

                        else if (cbJenis.SelectedIndex == 2)
                        {
                            rptName = string.Concat("Report_DO_OOT_", cbBulan.Text, "_", cbTahun.Text, ".", rptResult.Extension);
                        }
                    }
                    string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
                    tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
                      tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

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