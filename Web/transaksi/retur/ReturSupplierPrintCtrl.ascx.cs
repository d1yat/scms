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

public partial class transaksi_penjualan_ReturSupplierPrintCtrl : System.Web.UI.UserControl
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
        winPrintDetail.Title = "Cetak Retur Suplier Pembelian";
        break;
      case "02":
        winPrintDetail.Title = "Cetak Retur Suplier Repack";
        break;
      case "03":
        winPrintDetail.Title = "Cetak Retur Suplier Konfirmasi";
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
    string suplId = (e.ExtraParams["SuplierID"] ?? string.Empty);
    string rs1 = (e.ExtraParams["RSID1"] ?? string.Empty);
    string rs2 = (e.ExtraParams["RSID2"] ?? string.Empty);
    string tmp = (e.ExtraParams["Async"] ?? string.Empty);
    string tipeCode = (e.ExtraParams["TypeCode"] ?? string.Empty);

    if (string.IsNullOrEmpty(gdgId) && string.IsNullOrEmpty(suplId) &&
      string.IsNullOrEmpty(rs1) && string.IsNullOrEmpty(rs2) && string.IsNullOrEmpty(tipeCode))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");
      return;
    }

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();
    bool isAsync = false;

    bool.TryParse(tmp, out isAsync);

    if (tipeCode.Equals("01", StringComparison.OrdinalIgnoreCase))
    {
      rptParse.ReportingID = "10107";
    }
    else if (tipeCode.Equals("02", StringComparison.OrdinalIgnoreCase))
    {
      rptParse.ReportingID = "10108";
    }
    else if (tipeCode.Equals("03", StringComparison.OrdinalIgnoreCase))
    {
      rptParse.ReportingID = "10107-a";
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
      DataType = typeof(string).FullName,
      ParameterName = "c_nosup = @0",
      ParameterValue = (string.IsNullOrEmpty(suplId) ? string.Empty : suplId),
      IsLinqFilterParameter = true
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "c_type = @0",
      ParameterValue = (string.IsNullOrEmpty(tipeCode) ? string.Empty : tipeCode),
      IsLinqFilterParameter = true
    });

    if (!string.IsNullOrEmpty(rs1))
    {
      if (string.IsNullOrEmpty(rs2))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "c_rsno = @0",
          ParameterValue = (string.IsNullOrEmpty(rs1) ? string.Empty : rs1),
          IsLinqFilterParameter = true
        });
      }
      else
      {
        if (rs1.CompareTo(rs2) >= 0)
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "c_rsno = @0",
            ParameterValue = (string.IsNullOrEmpty(rs1) ? string.Empty : rs1),
            IsLinqFilterParameter = true
          });
        }
        else
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "c_rsno",
            ParameterValue = (string.IsNullOrEmpty(rs1) ? string.Empty : rs1),
            IsLinqFilterParameter = true,
            BetweenValue = (string.IsNullOrEmpty(rs2) ? string.Empty : rs2)
          });
        }
      }
    }

    #endregion

    #region Report Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(char).FullName,
      ParameterName = "LG_RSH.c_gdg",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_RSH.c_nosup",
      ParameterValue = (string.IsNullOrEmpty(suplId) ? string.Empty : suplId)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_RSH.c_type",
      ParameterValue = tipeCode
    });

    if (!string.IsNullOrEmpty(rs1))
    {
      if (string.IsNullOrEmpty(rs2))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_RSH.c_rsno",
          ParameterValue = (string.IsNullOrEmpty(rs1) ? string.Empty : rs1)
        });
      }
      else
      {
        if (rs1.CompareTo(rs2) >= 0)
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "LG_RSH.c_rsno",
            ParameterValue = (string.IsNullOrEmpty(rs1) ? string.Empty : rs1)
          });
        }
        else
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{LG_RSH.c_rsno}} IN ('{0}' TO '{1}'))", rs1, rs2),
            IsReportDirectValue = true
          });
        }
      }
    }

    #endregion

    rptParse.PaperID = "11*8.5";
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;

    rptParse.OutputReport = ReportParser.ParsingOutputReport((cbRptTypeOutput.SelectedItem != null ? cbRptTypeOutput.SelectedItem.Value : string.Empty));

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

          string tmpUri = null;

          if (tipeCode.Equals("01", StringComparison.OrdinalIgnoreCase))
          {
            tmpUri = Functional.UriDownloadGenerator(pag,
             rptResult.OutputFile, "Return Suplier Pembelian ", rptResult.Extension);
          }
          else if (tipeCode.Equals("02", StringComparison.OrdinalIgnoreCase))
          {
            tmpUri = Functional.UriDownloadGenerator(pag,
             rptResult.OutputFile, "Return Suplier Repack ", rptResult.Extension);
          }
          else if (tipeCode.Equals("03", StringComparison.OrdinalIgnoreCase))
          {
            tmpUri = Functional.UriDownloadGenerator(pag,
             rptResult.OutputFile, "Return Suplier Konfirmasi ", rptResult.Extension);
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
