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

public partial class transaksi_pengiriman_FakturEkspedisiPrintCtrl : System.Web.UI.UserControl
{
  protected void Page_Load(object sender, EventArgs e)
  {

  }

  public void ShowPrintPage()
  {
    winPrintDetail.Title = "Cetak Faktur Ekspedisi";

    winPrintDetail.Hidden = false;
    winPrintDetail.ShowModal();
  }

  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    if (!pag.IsAllowPrinting)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
      return;
    }

    string EkspId = (e.ExtraParams["EkspID"] ?? string.Empty);
    string faktur1 = (e.ExtraParams["FAKTURID1"] ?? string.Empty);
    string faktur2 = (e.ExtraParams["FAKTURID2"] ?? string.Empty);
    string tmp = (e.ExtraParams["Async"] ?? string.Empty);
    string outputRpt = (e.ExtraParams["OutputRpt"] ?? string.Empty);

    //if (string.IsNullOrEmpty(EkspId) &&
    //  string.IsNullOrEmpty(faktur1) && string.IsNullOrEmpty(faktur2))
    //{
    //  Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");
    //  return;
    //}

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();
    bool isAsync = false;

    bool.TryParse(tmp, out isAsync);

    rptParse.ReportingID = "10118";

    #region Linq Filter Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "c_exp = @0",
      ParameterValue = (string.IsNullOrEmpty(EkspId) ? string.Empty : EkspId),
      IsLinqFilterParameter = true
    });

    if (!string.IsNullOrEmpty(faktur1))
    {
      if (string.IsNullOrEmpty(faktur2))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "c_beno = @0",
          ParameterValue = (string.IsNullOrEmpty(faktur1) ? string.Empty : faktur1),
          IsLinqFilterParameter = true
        });
      }
      else
      {
        if (faktur1.CompareTo(faktur2) >= 0)
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "c_beno = @0",
            ParameterValue = (string.IsNullOrEmpty(faktur1) ? string.Empty : faktur1),
            IsLinqFilterParameter = true
          });
        }
        else
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "c_beno",
            ParameterValue = (string.IsNullOrEmpty(faktur1) ? string.Empty : faktur1),
            IsLinqFilterParameter = true,
            BetweenValue = (string.IsNullOrEmpty(faktur2) ? string.Empty : faktur2)
          });
        }
      }
    }

    #endregion

    #region Report Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_BEH.c_exp",
      ParameterValue = (string.IsNullOrEmpty(EkspId) ? string.Empty : EkspId)
    });

    if (!string.IsNullOrEmpty(faktur1))
    {
      if (string.IsNullOrEmpty(faktur2))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_BEH.c_beno",
          ParameterValue = (string.IsNullOrEmpty(faktur1) ? string.Empty : faktur1)
        });
      }
      else
      {
        if (faktur1.CompareTo(faktur2) >= 0)
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "LG_BEH.c_beno",
            ParameterValue = (string.IsNullOrEmpty(faktur1) ? string.Empty : faktur1)
          });
        }
        else
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{LG_BEH.c_beno}} IN ('{0}' TO '{1}'))", faktur1, faktur2),
            IsReportDirectValue = true
          });
        }
      }
    }

    #endregion

    rptParse.PaperID = "Letter";
    rptParse.IsLandscape = true;
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;
    rptParse.OutputReport = ReportParser.ParsingOutputReport(outputRpt);

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
          //string rptName = string.Concat("Packing_List_", pag.Nip, ".", rptResult.Extension);

          //string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
          //tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
          //  tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

          string tmpUri = Functional.UriDownloadGenerator(pag,
            rptResult.OutputFile, "Faktur Jual ", rptResult.Extension);

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
