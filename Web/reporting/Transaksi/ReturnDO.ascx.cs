using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_Transaksi_ReturnDO : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void InitializePage(string wndDownload)
    {
        hidWndDown.Text = wndDownload;
        Functional.SetComboData(cbGudang, "c_gdg", "Gudang 1", "1");
    }

    [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
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

        string TipeReport, KategoriReport;

        bool isAsync = false;

        rptParse.ReportingID = "21012-a";

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
            ParameterName = "gdg",
            IsSqlParameter = true,
            ParameterValue = cbGudang.SelectedItem.Value
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(DateTime).FullName,
            ParameterName = "cusno",
            IsSqlParameter = true,
            ParameterValue = cbCustomer.SelectedItem.Value == null ? "0000" : cbCustomer.SelectedItem.Value
        });

        if (rdALL.Checked)
        {
            TipeReport = "2";
            KategoriReport = "Semua Kategori";
        }
        else if (rdReturnDO.Checked)
        {
            TipeReport = "1";
            KategoriReport = "DO Kembali";
        }
        else
        {
            TipeReport = "0";
            KategoriReport = "DO Belum Kembali";
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
            ParameterName = "TEMP_DORETURN.C_ENTRY",
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
            ControlName = "txtGudang",
            Value = string.Format(": Gudang {0} {1}", cbGudang.SelectedItem.Value, "")
        });

        lstCustTxt.Add(new ReportCustomizeText()
        {
            SectionName = "Section3",
            ControlName = "txtCabang",
            Value = string.Format(": {0} {1}", cbCustomer.SelectedItem.Value == null ? "Semua Cabang" : cbCustomer.SelectedItem.Value + " - " + cbCustomer.SelectedItem.Text, "")
        });

        lstCustTxt.Add(new ReportCustomizeText()
        {
            SectionName = "Section4",
            ControlName = "txtTipeReport",
            Value = string.Format(": {0} {1}", KategoriReport, "")
        });

        #endregion

        rptParse.PaperID = "A4";
        rptParse.IsLandscape = true;
        rptParse.ReportCustomizeText = lstCustTxt.ToArray();
        rptParse.ReportParameter = lstRptParam.ToArray();
        rptParse.User = pag.Nip;

        rptParse.OutputReport = ReportParser.ParsingOutputReport((cbRptTypeOutput.SelectedItem != null ? cbRptTypeOutput.SelectedItem.Value : string.Empty));
        rptParse.IsShared = chkShare.Checked;
        rptParse.UserDefinedName = txRptUName.Text.Trim();

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
                    string rptName = string.Concat("Laporan_ReturnDO_", pag.Nip, ".", rptResult.Extension);

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
