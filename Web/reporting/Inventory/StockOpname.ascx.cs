using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_Inventory_StockOpname : System.Web.UI.UserControl
{
  public void InitializePage(string wndDownload)
  {
    hidWndDown.Text = wndDownload;
  }

  protected void Page_Load(object sender, EventArgs e)
  {
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

    DateTime date1 = DateTime.Today,
      date2 = DateTime.Today;
    List<string> lstData = new List<string>();
    //string tmp = null;
    bool isAsync = false;
    string tipereport = null;

    rptParse.ReportingID = "10009";

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

    if (rdBatch.Checked)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "lg_vwSOBadNew.c_gdg",
            ParameterValue = (string.IsNullOrEmpty(cbGudang.SelectedItem.Value) ? "0" : cbGudang.SelectedItem.Value)
        });

        if (rdBad.Checked)
        {
            // Batch barang bad
            tipereport = "batchbad";
        }
        else
        {
            //batch barang good
            tipereport = "batchgood";
        }
    }
    else if (rdTotal.Checked)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "lg_vwsonew.c_gdg",
            ParameterValue = (string.IsNullOrEmpty(cbGudang.SelectedItem.Value) ? "0" : cbGudang.SelectedItem.Value)
        });

        if (rdBad.Checked)
        {
            // Total barang bad
            tipereport = "totalbad";
        }
        else
        {
            // Total barang good
            tipereport = "totalgood";
        }
    }

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "session",
      IsSqlParameter = true,
      ParameterValue = pag.Nip
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(char).FullName,
        ParameterName = "gdg",
        IsSqlParameter = true,
        ParameterValue = cbGudang.SelectedItem.Value == null ? "0" : cbGudang.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(char).FullName,
        ParameterName = "iteno",
        IsSqlParameter = true,
        ParameterValue = cbItems.SelectedItem.Value == null ? "0000" : cbItems.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(char).FullName,
        ParameterName = "nosup",
        IsSqlParameter = true,
        ParameterValue = cbSuplier.SelectedItem.Value == null ? "00000" : cbSuplier.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "TipeReport",
        IsSqlParameter = true,
        ParameterValue = tipereport
    });

    if (cbGudang.SelectedItem.Value == "2" && rdBatch.Checked && !rdBad.Checked)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "({LG_MsBatch.d_expired} >= totext(Date(DateAdd ('yyyy', -1, currentdate)), 'yyyy/MM/dd') or {lg_vwSOBadNew.n_qtygood} <> 0)", 
            IsReportDirectValue = true
        });
    }
    else if (cbGudang.SelectedItem.Value == "2" && rdBatch.Checked && rdBad.Checked)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "({LG_MsBatch.d_expired} >= totext(Date(DateAdd ('yyyy', -1, currentdate)), 'yyyy/MM/dd') or {lg_vwSOBadNew.n_qtybad} <> 0)",
            IsReportDirectValue = true
        });
    }

    #endregion

    #region Report Parameter

    //lstRptParam.Add(new ReportParameter()
    //{
    //  DataType = typeof(string).FullName,
    //  ParameterName = "Temp_LGStock.c_user",
    //  ParameterValue = pag.Nip
    //});

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(bool).FullName,
      ParameterName = "FA_MasItm.l_opname",
      ParameterValue = "true"
    });

    if (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "fa_masitm.c_nosup",
        ParameterValue = cbSuplier.SelectedItem.Value
      });
    }

    if (!string.IsNullOrEmpty(cbItems.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "fa_masitm.c_iteno",
        ParameterValue = cbItems.SelectedItem.Value
      });
    }

    #endregion

    rptParse.PaperID = "A3";
    rptParse.ReportCustomizeText = null;
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.IsLandscape = false;
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
      Functional.ShowMsgInformation("Pembuatan report gagal.");
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
          string rptName = string.Concat("Inventory_StokOpname_", pag.Nip, ".", rptResult.Extension);

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
