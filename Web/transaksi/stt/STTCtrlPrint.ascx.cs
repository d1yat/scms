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


public partial class transaksi_stt_STTCtrlPrint : System.Web.UI.UserControl
{
  protected void Page_Load(object sender, EventArgs e)
  {
    //if (!Functional.IsAllowView(this.Page as Scms.Web.Core.PageHandler))
    //{
    //  return;
    //}
  }

  public void ShowPrintPage(string typeCode)
  {
    typeCode = (typeCode  ?? "01");

    hfType.Text = typeCode;

    if (typeCode.Equals("01", StringComparison.OrdinalIgnoreCase))
    {
      winPrintDetail.Title = "Cetak Surat Tanda Terima Donasi";
    }
    else if (typeCode.Equals("02", StringComparison.OrdinalIgnoreCase))
    {
      winPrintDetail.Title = "Cetak Surat Tanda Terima Sample";
    }

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

    //string gdgId = pag.ActiveGudang;
    string gdgId = (e.ExtraParams["GudangID"] ?? string.Empty);
    string stt1 = (e.ExtraParams["STTID1"] ?? string.Empty);
    string stt2 = (e.ExtraParams["STTID2"] ?? string.Empty);
    string typeCode = (e.ExtraParams["TypeCode"] ?? string.Empty);
    string tmp = (e.ExtraParams["Async"] ?? string.Empty);

    if (string.IsNullOrEmpty(gdgId) &&
      string.IsNullOrEmpty(stt1) && string.IsNullOrEmpty(stt2) && 
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

    if (typeCode.Equals("01", StringComparison.OrdinalIgnoreCase))
    {
      rptParse.ReportingID = "10104";
    }
    else if (typeCode.Equals("02", StringComparison.OrdinalIgnoreCase))
    {
      rptParse.ReportingID = "10104-a";
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
      ParameterName = "c_type = @0",
      ParameterValue = (string.IsNullOrEmpty(typeCode) ? string.Empty : typeCode),
      IsLinqFilterParameter = true
    });

    if (!string.IsNullOrEmpty(stt1))
    {
      if (string.IsNullOrEmpty(stt2))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "c_stno = @0",
          ParameterValue = (string.IsNullOrEmpty(stt1) ? string.Empty : stt1),
          IsLinqFilterParameter = true
        });
      }
      else
      {
        if (stt1.CompareTo(stt2) >= 0)
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "c_stno = @0",
            ParameterValue = (string.IsNullOrEmpty(stt1) ? string.Empty : stt1),
            IsLinqFilterParameter = true
          });
        }
        else
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "c_stno",
            ParameterValue = (string.IsNullOrEmpty(stt1) ? string.Empty : stt1),
            IsLinqFilterParameter = true,
            BetweenValue = (string.IsNullOrEmpty(stt2) ? string.Empty : stt2)
          });
        }
      }
    }

    #endregion

    #region Report Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_STH.c_gdg",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_STH.c_type",
      ParameterValue = (string.IsNullOrEmpty(typeCode) ? string.Empty : typeCode)
    });

    if (!string.IsNullOrEmpty(stt1))
    {
      if (string.IsNullOrEmpty(stt2))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_STH.c_stno",
          ParameterValue = (string.IsNullOrEmpty(stt1) ? string.Empty : stt1)
        });
      }
      else
      {
        if (stt1.CompareTo(stt2) >= 0)
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "LG_STH.c_stno",
            ParameterValue = (string.IsNullOrEmpty(stt1) ? string.Empty : stt1)
          });
        }
        else
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{LG_STH.c_stno}} IN ('{0}' TO '{1}'))", stt1, stt2),
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

          if (typeCode.Equals("01", StringComparison.OrdinalIgnoreCase))
          {
            rptName = string.Concat("Surat Tanda Terima - Donasi ", pag.Nip, ".", rptResult.Extension);
          }
          else if (typeCode.Equals("02", StringComparison.OrdinalIgnoreCase))
          {
            rptName = string.Concat("Surat Tanda Terima - Sample ", pag.Nip, ".", rptResult.Extension);
          }

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
