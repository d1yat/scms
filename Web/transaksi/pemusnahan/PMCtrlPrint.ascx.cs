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


public partial class transaksi_pemusnahan_PMCtrlPrint : System.Web.UI.UserControl
{
  public void ShowPrintPage()
  {
    winPrintDetail.Title = "Cetak Transaksi Pemusnahan Barang";

    winPrintDetail.Hidden = false;
    winPrintDetail.ShowModal();

    Functional.SetComboData(cbGudang, "c_gdg", "Gudang Pemusnahan", "5");
    cbGudang.ReadOnly = true;
  }

  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    if (!pag.IsAllowPrinting)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
      return;
    }

    string gdgId = (e.ExtraParams["GudangID"] ?? string.Empty);
    string pm1 = (e.ExtraParams["PMID1"] ?? string.Empty);
    string pm2 = (e.ExtraParams["PMID2"] ?? string.Empty);
    string tmp = (e.ExtraParams["Async"] ?? string.Empty);

    if (string.IsNullOrEmpty(gdgId) &&
      string.IsNullOrEmpty(pm1) && string.IsNullOrEmpty(pm2))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");
      return;
    }

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();
    bool isAsync = false;

    bool.TryParse(tmp, out isAsync);
    rptParse.ReportingID = "10114";

    
    #region Linq Filter Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(char).FullName,
      ParameterName = "c_gdg = @0",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId),
      IsLinqFilterParameter = true
    });

    if (!string.IsNullOrEmpty(pm1))
    {
      if (string.IsNullOrEmpty(pm2))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "c_pmno = @0",
          ParameterValue = (string.IsNullOrEmpty(pm1) ? string.Empty : pm1),
          IsLinqFilterParameter = true
        });
      }
      else
      {
        if (pm1.CompareTo(pm2) >= 0)
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "c_pmno = @0",
            ParameterValue = (string.IsNullOrEmpty(pm1) ? string.Empty : pm1),
            IsLinqFilterParameter = true
          });
        }
        else
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "c_pmno",
            ParameterValue = (string.IsNullOrEmpty(pm1) ? string.Empty : pm1),
            IsLinqFilterParameter = true,
            BetweenValue = (string.IsNullOrEmpty(pm2) ? string.Empty : pm2)
          });
        }
      }
    }

    #endregion

    #region Report Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_PMH.c_gdg",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
    });

    if (!string.IsNullOrEmpty(pm1))
    {
      if (string.IsNullOrEmpty(pm2))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_PMH.c_pmno",
          ParameterValue = (string.IsNullOrEmpty(pm1) ? string.Empty : pm1)
        });
      }
      else
      {
        if (pm1.CompareTo(pm2) >= 0)
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "LG_PMH.c_pmno",
            ParameterValue = (string.IsNullOrEmpty(pm1) ? string.Empty : pm1)
          });
        }
        else
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{LG_PMH.c_pmno}} IN ('{0}' TO '{1}'))", pm1, pm2),
            IsReportDirectValue = true
          });
        }
      }
    }

    #endregion

    rptParse.PaperID = "8.5x5.5";
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;

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
          string rptName = null;
          rptName = string.Concat("Pemusnahan Barang ", pag.Nip, ".", rptResult.Extension);

          string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
          tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
            tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

          wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
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
