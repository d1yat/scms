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
public partial class transaksi_transfer_TransferGudangCtrlPrint : System.Web.UI.UserControl
{
  protected void Page_Load(object sender, EventArgs e)
  {

  }

  public void ShowPrintPage(string typeCode)
  {
    hfType.Text = typeCode;

    switch (typeCode)
    {
      case "01":
        winPrintDetail.Title = "Cetak Surat Jalan - Pesanan Gudang";
        break;
      case "02":
        winPrintDetail.Title = "Cetak Surat Jalan - Transfer Gudang";
        break;
      case "03":
        winPrintDetail.Title = "Cetak Surat Jalan - Claim";
        break;
      default:
        winPrintDetail.Title = "Cetak Surat Jalan - ?";
        break;
    }

    winPrintDetail.Hidden = false;
    winPrintDetail.ShowModal();
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string gdgId = pag.ActiveGudang;
    string GudangID = (e.ExtraParams["GudangID"] ?? string.Empty);
    string sj1 = (e.ExtraParams["SJID1"] ?? string.Empty);
    string sj2 = (e.ExtraParams["SJID2"] ?? string.Empty);
    string typeCode = (e.ExtraParams["TypeCode"] ?? string.Empty);
    string tmp = (e.ExtraParams["Async"] ?? string.Empty);

    if (string.IsNullOrEmpty(gdgId) && string.IsNullOrEmpty(GudangID) &&
      string.IsNullOrEmpty(sj1) && string.IsNullOrEmpty(sj2) && 
      string.IsNullOrEmpty(typeCode))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");
      return;
    }

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();
    bool isAsync = false;

    bool.TryParse(tmp, out isAsync);
    
    switch (typeCode)
    {
      case "01":
        rptParse.ReportingID = "10110-a";
        break;
      case "02":
        rptParse.ReportingID = "10110-b";
        break;
      case "03":
        rptParse.ReportingID = "10110-c";
        break;
    }

    #region Linq Filter Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(char).FullName,
      ParameterName = "c_gdg = @0",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId),
      IsLinqFilterParameter = true
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(char).FullName,
      ParameterName = "c_gdg2 = @0",
      ParameterValue = (string.IsNullOrEmpty(GudangID) ? string.Empty : GudangID),
      IsLinqFilterParameter = true
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "c_type = @0",
      ParameterValue = (string.IsNullOrEmpty(typeCode) ? string.Empty : typeCode),
      IsLinqFilterParameter = true
    });

    if (!string.IsNullOrEmpty(sj1))
    {
      if (string.IsNullOrEmpty(sj2))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "c_sjno = @0",
          ParameterValue = (string.IsNullOrEmpty(sj1) ? string.Empty : sj1),
          IsLinqFilterParameter = true
        });
      }
      else
      {
        if (sj1.CompareTo(sj2) >= 0)
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "c_sjno = @0",
            ParameterValue = (string.IsNullOrEmpty(sj1) ? string.Empty : sj1),
            IsLinqFilterParameter = true
          });
        }
        else
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "c_sjno",
            ParameterValue = (string.IsNullOrEmpty(sj1) ? string.Empty : sj1),
            IsLinqFilterParameter = true,
            BetweenValue = (string.IsNullOrEmpty(sj2) ? string.Empty : sj2)
          });
        }
      }
    }

    #endregion

    #region Report Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_SJH.c_gdg",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_SJH.c_gdg2",
      ParameterValue = (string.IsNullOrEmpty(GudangID) ? string.Empty : GudangID)
    });
    
    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_SJH.c_type",
      ParameterValue = (string.IsNullOrEmpty(typeCode) ? string.Empty : typeCode)
    });

    if (!string.IsNullOrEmpty(sj1))
    {
      if (string.IsNullOrEmpty(sj2))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_SJH.c_sjno",
          ParameterValue = (string.IsNullOrEmpty(sj1) ? string.Empty : sj1)
        });
      }
      else
      {
        if (sj1.CompareTo(sj2) >= 0)
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "LG_SJH.c_sjno",
            ParameterValue = (string.IsNullOrEmpty(sj1) ? string.Empty : sj1)
          });
        }
        else
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{LG_SJH.c_sjno}} IN ('{0}' TO '{1}'))", sj1, sj2),
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
          string tmpUri = null;

          if (typeCode.Equals("01", StringComparison.OrdinalIgnoreCase))
          {
            tmpUri = Functional.UriDownloadGenerator(pag,
             rptResult.OutputFile, "Pesanan Gudang", rptResult.Extension);
          }
          else if (typeCode.Equals("02", StringComparison.OrdinalIgnoreCase))
          {
            tmpUri = Functional.UriDownloadGenerator(pag,
             rptResult.OutputFile, "Transfer Gudang", rptResult.Extension);
          }
          else if (typeCode.Equals("03", StringComparison.OrdinalIgnoreCase))
          {
            tmpUri = Functional.UriDownloadGenerator(pag,
             rptResult.OutputFile, "Claim", rptResult.Extension);
          }

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
