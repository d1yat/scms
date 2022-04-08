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

public partial class transaksi_penjualan_MemoComboPrintCtrl : System.Web.UI.UserControl
{
  protected void Page_Load(object sender, EventArgs e)
  {

  }

  public void ShowPrintPage()
  {
    winPrintDetail.Title = "Cetak Combo";

    winPrintDetail.Hidden = false;
    winPrintDetail.ShowModal();
  }
  
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string gdgId = (e.ExtraParams["GudangID"] ?? string.Empty);
    string combo1 = (e.ExtraParams["Combo1"] ?? string.Empty);
    string combo2 = (e.ExtraParams["Combo2"] ?? string.Empty);
    string tmp = (e.ExtraParams["Async"] ?? string.Empty);

    if (string.IsNullOrEmpty(gdgId) && 
      string.IsNullOrEmpty(combo1) && string.IsNullOrEmpty(combo2))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");
      return;
    }

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();
    bool isAsync = false;

    bool.TryParse(tmp, out isAsync);

    rptParse.ReportingID = "10109";

    #region Linq Filter Parameter

    //lstRptParam.Add(new ReportParameter()
    //{
    //  DataType = typeof(char).FullName,
    //  ParameterName = "c_gdg = @0",
    //  ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId),
    //  IsLinqFilterParameter = true
    //});

    //lstRptParam.Add(new ReportParameter()
    //{
    //  DataType = typeof(string).FullName,
    //  ParameterName = "c_nosup = @0",
    //  ParameterValue = (string.IsNullOrEmpty(custId) ? string.Empty : custId),
    //  IsLinqFilterParameter = true
    //});

    //if (!string.IsNullOrEmpty(po1))
    //{
    //  if (string.IsNullOrEmpty(po2))
    //  {
    //    lstRptParam.Add(new ReportParameter()
    //    {
    //      DataType = typeof(string).FullName,
    //      ParameterName = "c_pono = @0",
    //      ParameterValue = (string.IsNullOrEmpty(po1) ? string.Empty : po1),
    //      IsLinqFilterParameter = true
    //    });
    //  }
    //  else
    //  {
    //    if (po1.CompareTo(po2) >= 0)
    //    {
    //      lstRptParam.Add(new ReportParameter()
    //      {
    //        DataType = typeof(string).FullName,
    //        ParameterName = "c_pono = @0",
    //        ParameterValue = (string.IsNullOrEmpty(po1) ? string.Empty : po1),
    //        IsLinqFilterParameter = true
    //      });
    //    }
    //    else
    //    {
    //      lstRptParam.Add(new ReportParameter()
    //      {
    //        DataType = typeof(string).FullName,
    //        ParameterName = "c_pono",
    //        ParameterValue = (string.IsNullOrEmpty(po1) ? string.Empty : po1),
    //        IsLinqFilterParameter = true,
    //        BetweenValue = (string.IsNullOrEmpty(po2) ? string.Empty : po2)
    //      });
    //    }
    //  }
    //}

    #endregion

    #region Report Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(char).FullName,
      ParameterName = "LG_COMBOH.c_gdg",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_COMBOH.c_type",
      ParameterValue = "01"
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "({LG_COMBOD1.N_QTY} <> 0)",
      IsReportDirectValue = true
    });

    if (!string.IsNullOrEmpty(combo1))
    {
      if (string.IsNullOrEmpty(combo2))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_COMBOD1.c_combono",
          ParameterValue = (string.IsNullOrEmpty(combo1) ? string.Empty : combo1)
        });
      }
      else
      {
        if (combo1.CompareTo(combo2) >= 0)
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "LG_COMBOD1.c_combono",
            ParameterValue = (string.IsNullOrEmpty(combo1) ? string.Empty : combo1)
          });
        }
        else
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{LG_COMBOD1.c_combono}} IN ('{0}' TO '{1}'))", combo1, combo2),
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
          //string rptName = string.Concat("Packing_List_", pag.Nip, ".", rptResult.Extension);

          //string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
          //tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
          //  tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

          string tmpUri = Functional.UriDownloadGenerator(pag,
            rptResult.OutputFile, "Combo ", rptResult.Extension);

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
