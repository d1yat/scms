using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_history_Shipment : System.Web.UI.UserControl
{
  public void InitializePage(string wndDownload)
  {
    hidWndDown.Text = wndDownload;
  }

  protected void Page_Load(object sender, EventArgs e)
  {
      DateTime date = DateTime.Now;
      //Indra 20170815
      //Functional.PopulateBulan(cbBulan, date.Month);
      //Functional.PopulateTahun(cbTahun, date.Year, 1, 0);
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
    List<ReportCustomizeText> lstCustTxt = new List<ReportCustomizeText>();
    string tmp = null;
    bool isAsync = false;

    rptParse.ReportingID = "20022";

    isAsync = chkAsync.Checked;

    #region Report Parameter
    
    #region Old By Indra 20170814
    //lstRptParam.Add(new ReportParameter()
    //    {
    //        DataType = typeof(string).FullName,
    //        ParameterName = string.Format("(Month(cDate({{LG_ExpH.d_expdate}})) = {0} AND Year(cDate({{LG_ExpH.d_expdate}})) = {1})", cbBulan.SelectedItem.Value, cbTahun.SelectedItem.Text.ToString()),
    //        IsReportDirectValue = true
    //    });

    //if (!string.IsNullOrEmpty(cbExpedisi.SelectedItem.Value))
    //{
    //    lstRptParam.Add(new ReportParameter()
    //    {
    //        DataType = typeof(string).FullName,
    //        ParameterName = "LG_exph.c_exp",
    //        ParameterValue = cbExpedisi.SelectedItem.Value
    //    });
    //}

    //if (!string.IsNullOrEmpty(cbCustomer.SelectedItem.Value))
    //{
    //    lstRptParam.Add(new ReportParameter()
    //    {
    //        DataType = typeof(string).FullName,
    //        ParameterName = "LG_exph.c_cusno",
    //        ParameterValue = cbCustomer.SelectedItem.Value
    //    });
    //}


    //if (!string.IsNullOrEmpty(txNoEP1.Text))
    //{
    //    if (string.IsNullOrEmpty(txNoEP2.Text))
    //    {
    //        lstRptParam.Add(new ReportParameter()
    //        {
    //            DataType = typeof(string).FullName,
    //            ParameterName = "lg_exph.c_expno",
    //            ParameterValue = (string.IsNullOrEmpty(txNoEP1.Text) ? string.Empty : txNoEP1.Text)
    //        });
    //    }
    //    else
    //    {
    //        if (txNoEP1.Text.CompareTo(txNoEP2.Text) >= 0)
    //        {
    //            lstRptParam.Add(new ReportParameter()
    //            {
    //                DataType = typeof(string).FullName,
    //                ParameterName = "lg_exph.c_expno",
    //                ParameterValue = (string.IsNullOrEmpty(txNoEP1.Text) ? string.Empty : txNoEP1.Text)
    //            });
    //        }
    //        else
    //        {
    //            lstRptParam.Add(new ReportParameter()
    //            {
    //                DataType = typeof(string).FullName,
    //                ParameterName = string.Format("({{lg_exph.c_expno}} IN ('{0}' TO '{1}'))", txNoEP1.Text, txNoEP2.Text),
    //                IsReportDirectValue = true
    //            });
    //        }
    //    }
    //}

    //if (cbGudang.SelectedIndex > -1)
    //{
    //    lstRptParam.Add(new ReportParameter()
    //    {
    //        DataType = typeof(string).FullName,
    //        ParameterName = "LG_exph.c_gdg",
    //        ParameterValue = cbGudang.SelectedItem.Value
    //    });
    //}
    #endregion

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
        ParameterName = "gdg",
        IsSqlParameter = true,
        ParameterValue = cbGudang.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "cusno",
        IsSqlParameter = true,
        ParameterValue = cbCustomer.SelectedItem.Value == null ? "0000" : cbCustomer.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "ekspedisi",
        IsSqlParameter = true,
        ParameterValue = cbExpedisi.SelectedItem.Value == null ? "00" : cbExpedisi.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "user",
        IsSqlParameter = true,
        ParameterValue = pag.Nip
    });

    if (!string.IsNullOrEmpty(txNoEP1.Text))
    {
        if (string.IsNullOrEmpty(txNoEP2.Text))
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = "TEMP_HISTORYSHIPMENT.c_expno",
                ParameterValue = (string.IsNullOrEmpty(txNoEP1.Text) ? string.Empty : txNoEP1.Text)
            });
        }
        else
        {
            if (txNoEP1.Text.CompareTo(txNoEP2.Text) >= 0)
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "TEMP_HISTORYSHIPMENT.c_expno",
                    ParameterValue = (string.IsNullOrEmpty(txNoEP1.Text) ? string.Empty : txNoEP1.Text)
                });
            }
            else
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = string.Format("({{TEMP_HISTORYSHIPMENT.c_expno}} IN ('{0}' TO '{1}'))", txNoEP1.Text, txNoEP2.Text),
                    IsReportDirectValue = true
                });
            }
        }
    }

    //Indra 20170815
    //lstCustTxt.Add(new ReportCustomizeText()
    //{
    //    SectionName = "Section1",
    //    ControlName = "txtPeriode",
    //    Value = string.Format("Periode : {0} {1}", cbBulan.SelectedItem.Text, cbTahun.SelectedItem.Text)
    //});

    
    lstCustTxt.Add(new ReportCustomizeText()
    {
        SectionName = "Section1",
        ControlName = "txtPeriode",
        Value = string.Format("Periode : {0} {1}", date1.ToString("d-M-yyyy") + " - ", date2.ToString("d-M-yyyy"))
    });

    #endregion

    rptParse.PaperID = "17.5*11";
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;
    rptParse.ReportCustomizeText = lstCustTxt.ToArray();
    rptParse.OutputReport = ReportParser.ParsingOutputReport((cbRptTypeOutput.SelectedItem != null ? cbRptTypeOutput.SelectedItem.Value : string.Empty));
    rptParse.IsShared = chkShare.Checked;
    rptParse.UserDefinedName = txRptUName.Text.Trim();

    string xmlFiles = ReportParser.Deserialize(rptParse);

    SoaReportCaller soa = new SoaReportCaller();

    //string result = "";
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
          string rptName = string.Concat("History_Shipment_", pag.Nip, ".", rptResult.Extension);

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
