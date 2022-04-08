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
public partial class transaksi_penjualan_DO_STTCtrlPrint : System.Web.UI.UserControl
{
  protected void Page_Load(object sender, EventArgs e)
  {
    //if (!Functional.IsAllowView(this.Page as Scms.Web.Core.PageHandler))
    //{
    //  return;
    //}
  }

  public void ShowPrintPage()
  {
    winPrintDetail.Title = "Cetak Delivery Order (Surat Tanda Terima)";

    winPrintDetail.Hidden = false;
    winPrintDetail.ShowModal();
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    if (!pag.IsAllowPrinting)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
      return;
    }

    string gdgId = pag.ActiveGudang;
    string custId = (e.ExtraParams["CustomeID"] ?? string.Empty);
    string do1 = (e.ExtraParams["DOID1"] ?? string.Empty);
    string do2 = (e.ExtraParams["DOID2"] ?? string.Empty);
    string tmp = (e.ExtraParams["Async"] ?? string.Empty);

    if (string.IsNullOrEmpty(gdgId) && string.IsNullOrEmpty(custId) &&
      string.IsNullOrEmpty(do1) && string.IsNullOrEmpty(do2))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");
      return;
    }

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();
    bool isAsync = false;

    bool.TryParse(tmp, out isAsync);

    rptParse.ReportingID = "10105";

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
      ParameterName = "c_cusno = @0",
      ParameterValue = (string.IsNullOrEmpty(custId) ? string.Empty : custId),
      IsLinqFilterParameter = true
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "c_type = @0",
      ParameterValue = "02",
      IsLinqFilterParameter = true
    });

    if (!string.IsNullOrEmpty(do1))
    {
      if (string.IsNullOrEmpty(do2))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "c_dono = @0",
          ParameterValue = (string.IsNullOrEmpty(do1) ? string.Empty : do1),
          IsLinqFilterParameter = true
        });
      }
      else
      {
        if (do1.CompareTo(do2) >= 0)
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "c_dono = @0",
            ParameterValue = (string.IsNullOrEmpty(do1) ? string.Empty : do1),
            IsLinqFilterParameter = true
          });
        }
        else
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "c_dono",
            ParameterValue = (string.IsNullOrEmpty(do1) ? string.Empty : do1),
            IsLinqFilterParameter = true,
            BetweenValue = (string.IsNullOrEmpty(do2) ? string.Empty : do2)
          });
        }
      }
    }

    #endregion

    #region Report Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_DOH.c_gdg",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_DOH.c_cusno",
      ParameterValue = (string.IsNullOrEmpty(custId) ? string.Empty : custId)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_DOH.c_type",
      ParameterValue = "02"
    });

    if (!string.IsNullOrEmpty(do1))
    {
      if (string.IsNullOrEmpty(do2))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_DOH.c_dono",
          ParameterValue = (string.IsNullOrEmpty(do1) ? string.Empty : do1)
        });
      }
      else
      {
        if (do1.CompareTo(do2) >= 0)
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "LG_DOH.c_dono",
            ParameterValue = (string.IsNullOrEmpty(do1) ? string.Empty : do1)
          });
        }
        else
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{LG_DOH.c_dono}} IN ('{0}' TO '{1}'))", do1, do2),
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
          string rptName = string.Concat("Deliver Order (STT) ", pag.Nip, ".", rptResult.Extension);

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
