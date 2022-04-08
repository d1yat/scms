using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_history_SuratPesananGudang : System.Web.UI.UserControl
{
  public void InitializePage(string wndDownload)
  {
    hidWndDown.Text = wndDownload;

    DateTime now = DateTime.Now;
    DateTime date = new DateTime(now.Year, now.Month, 1);
    txPeriode1.Text = date.ToString("dd-MM-yyyy");
    txPeriode2.Text = DateTime.Now.ToString("dd-MM-yyyy");
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

    rptParse.ReportingID = "20004";

    isAsync = chkAsync.Checked;

    tmp = txSPG1.Text.Trim();
    tmp1 = txSPG2.Text.Trim();

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
        ParameterName = "nosup",
        IsSqlParameter = true,
        ParameterValue = string.IsNullOrEmpty(cbSuplier.SelectedItem.Value) ? "00000" : cbSuplier.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "divpri",
        IsSqlParameter = true,
        ParameterValue = string.IsNullOrEmpty(cbDivPrinsipal.SelectedItem.Value) ? "000" : cbDivPrinsipal.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "iteno",
        IsSqlParameter = true,
        ParameterValue = string.IsNullOrEmpty(cbItems.SelectedItem.Value) ? "0000" : cbItems.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "session",
      IsSqlParameter = true,
      ParameterValue = pag.Nip
    });

    #endregion

    #region Report Parameter

    if (!string.IsNullOrEmpty(txSPG1.Text.Trim()))
    {
        if (string.IsNullOrEmpty(txSPG2.Text.Trim()))
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = "Temp_LGHistorySPG.c_spgno",
                ParameterValue = (string.IsNullOrEmpty(txSPG1.Text.Trim()) ? string.Empty : txSPG1.Text.Trim())
            });
        }
        else
        {
            if (txSPG1.Text.Trim().CompareTo(txSPG2.Text.Trim()) >= 0)
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "Temp_LGHistorySPG.c_spgno",
                    ParameterValue = (string.IsNullOrEmpty(txSPG1.Text.Trim()) ? string.Empty : txSPG1.Text.Trim())
                });
            }
            else
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = string.Format("({{Temp_LGHistorySPG.c_spgno}} IN ('{0}' TO '{1}'))", txSPG1.Text.Trim(), txSPG2.Text.Trim()),
                    IsReportDirectValue = true
                });
            }
        }
    }

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "Temp_LGHistorySPG.c_user",
      ParameterValue = pag.Nip
    });

    if (cbGudangFrom.SelectedIndex != -1)
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "Temp_LGHistorySPG.c_gdg1",
        ParameterValue = cbGudangFrom.SelectedItem.Value
      });      
    }

    if (cbGudangTo.SelectedIndex != -1)
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "Temp_LGHistorySPG.c_gdg2",
        ParameterValue = cbGudangTo.SelectedItem.Value
      });
    }

    #region Old Coded

    //if (cbItems.SelectedItems.Count > 1)
    //{
    //  for (int nLoop = 0, nlen = cbItems.SelectedItems.Count; nLoop < nlen; nLoop++)
    //  {
    //    tmp = cbItems.SelectedItems[nLoop].Value;
    //    if (!string.IsNullOrEmpty(tmp))
    //    {
    //      lstData.Add(string.Concat("'", tmp, "'"));
    //    }
    //  }

    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = string.Format("({{Temp_LGHistorySPG.c_iteno}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbItems.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "Temp_LGHistorySPG.c_iteno",
    //    ParameterValue = (cbItems.SelectedItems[0].Value == null ? string.Empty : cbItems.SelectedItems[0].Value)
    //  });
    //}

    #endregion

    #endregion

    rptParse.PaperID = "Letter";
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
          string rptName = string.Concat("History_SPG_", pag.Nip, ".", rptResult.Extension);

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
