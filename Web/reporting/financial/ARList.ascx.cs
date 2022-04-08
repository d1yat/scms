using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_financial_ARList : System.Web.UI.UserControl
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
    //List<string> lstData = null;
    string tmp = null, 
      mode = null,
      Cus = null;
    bool isAsync = false;

    tmp = Functional.GetCheckedRadioData(rdgTipe);

    if (tmp.Equals("ringkasan"))
    {
      rptParse.ReportingID = "20402-3-1";
      rptParse.PaperID = "17.5*11";
      rptParse.IsLandscape = false;
      mode = "ringkasan";
    }
    else if (tmp.Equals("detilFak"))
    {
      rptParse.ReportingID = "20402-3-2";
      rptParse.PaperID = "17.5*11";
      rptParse.IsLandscape = false;
      mode = "detilFak";
    }
    else if (tmp.Equals("detilTrans"))
    {
      rptParse.ReportingID = "20402-3-3";
      rptParse.PaperID = "17.5*11";
      rptParse.IsLandscape = true;
      mode = "detilTrans";
    }

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

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "session",
      IsSqlParameter = true,
      ParameterValue = pag.Nip
    });

    Cus = (string.IsNullOrEmpty(cbCustomer.SelectedItem.Value) ? string.Empty : cbCustomer.SelectedItem.Value);

    if (string.IsNullOrEmpty(Cus))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "customer",
        ParameterValue = "0000",
        IsSqlParameter = true
      });
    }
    else
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "customer",
        ParameterValue = Cus,
        IsSqlParameter = true
      });
    }

    #endregion

    #region Report Parameter

    switch (mode)
    {
      case "ringkasan":
      case "detilFak":
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "Temp_LGJual2_1.c_user",
            ParameterValue = pag.Nip
          });

          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "Temp_LGJual2_1.c_type",
            ParameterValue = Functional.GetCheckedRadioData(rdgTipeFak)
          });

          if (!(string.IsNullOrEmpty(Cus)))
          {
            lstRptParam.Add(new ReportParameter()
            {
              DataType = typeof(string).FullName,
              ParameterName = "customer",
              ParameterValue = Cus,
              IsSqlParameter = true
            });
          }
        }
        break;
      case "detilTrans":
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "Temp_LGJual2.c_user",
            ParameterValue = pag.Nip
          });

          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "Temp_LGJual2.c_type",
            ParameterValue = Functional.GetCheckedRadioData(rdgTipeFak)
          });

          if (!(string.IsNullOrEmpty(Cus)))
          {
            lstRptParam.Add(new ReportParameter()
            {
              DataType = typeof(string).FullName,
              ParameterName = "customer",
              ParameterValue = Cus,
              IsSqlParameter = true
            });
          }
        }
        break;
    }

    #endregion

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
          string rptName = string.Concat("AR_List_", pag.Nip, ".", rptResult.Extension);

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
