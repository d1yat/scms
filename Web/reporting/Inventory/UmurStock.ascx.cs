using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_Inventory_UmurStock : System.Web.UI.UserControl
{
  public void InitializePage(string wndDownload)
  {
    hidWndDown.Text = wndDownload;
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    DateTime date1 = DateTime.Today,
      date2 = DateTime.Today;
    //List<string> lstData = null;
    string tmp = null;
    bool isAsync = false;

    tmp = Functional.GetCheckedRadioData(rdgTipeReport);

    if (tmp.Equals("01"))
    {
      rptParse.IsLandscape = true;

      rptParse.ReportingID = "10006-a";
    }
    else if (tmp.Equals("02"))
    {
      rptParse.IsLandscape = true;

      rptParse.ReportingID = "10006-b";
    }
    else if (tmp.Equals("03"))
    {
      rptParse.IsLandscape = false;

      rptParse.ReportingID = "10006-c";
    }
    else if (tmp.Equals("04"))
    {
      rptParse.IsLandscape = false;

      rptParse.ReportingID = "10006-d";
    }

    isAsync = chkAsync.Checked;

    #region Sql Parameter

    if (Scms.Web.Common.Functional.DateParser(txPeriode1.RawText.Trim(), "d-M-yyyy", out date1))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(DateTime).FullName,
        ParameterName = "date",
        IsSqlParameter = true,
        ParameterValue = date1.ToString("d-M-yyyy")
      });
    }

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "type",
      IsSqlParameter = true,
      ParameterValue = Functional.GetCheckedRadioData(rdgTipeProduk)
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

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "Temp_LGUmurStock.c_user",
      ParameterValue = pag.Nip
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "Temp_LGUmurStock.c_gdg",
      ParameterValue = (string.IsNullOrEmpty(cbGudang.SelectedItem.Value) ? "0" : cbGudang.SelectedItem.Value)
    });

    if (!string.IsNullOrEmpty(cbDivAms.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "FA_MsDivAMS.c_kddivams",
        ParameterValue = cbDivAms.SelectedItem.Value
      });
    }

    if (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "LG_DatSup.c_nosup",
        ParameterValue = cbSuplier.SelectedItem.Value
      });
    }

    if (!string.IsNullOrEmpty(cbDivPrinsipal.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "FA_MsDivPri.c_kddivpri",
        ParameterValue = cbDivPrinsipal.SelectedItem.Value
      });
    }

    if (!string.IsNullOrEmpty(cbItems.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "Temp_LGUmurStock.c_iteno",
        ParameterValue = cbItems.SelectedItem.Value
      });
    }

    //if (txUmur1.Text.Length > 0)
    //{
    //  if (txUmur2.Text.Length > 0)
    //  {
    //    lstRptParam.Add(new ReportParameter()
    //    {
    //      DataType = typeof(string).FullName,
    //      ParameterName = string.Format("({{Temp_LGUmurStock.n_umur}} IN [{0},{1}])", txUmur1.Text, txUmur2.Text),
    //      IsReportDirectValue = true
    //    });
    //  }
    //  else
    //  {
    //    lstRptParam.Add(new ReportParameter()
    //    {
    //      DataType = typeof(string).FullName,
    //      ParameterName = string.Format("({{Temp_LGUmurStock.n_umur}} >= {0})", txUmur1.Text),
    //      IsReportDirectValue = true
    //    });
    //  }
    //}
    //else if (txUmur2.Text.Length > 0)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = string.Format("({{Temp_LGUmurStock.n_umur}} <= {0})", txUmur2.Text),
    //    IsReportDirectValue = true
    //  });
    //}

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
          string rptName = string.Concat("Inventory_UmurStok_", pag.Nip, ".", rptResult.Extension);

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
