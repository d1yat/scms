using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_history_Asuransi : System.Web.UI.UserControl
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

    //string tipeDetail = (e.ExtraParams["Total"] ?? string.Empty);

    if (!Functional.CanCreateGenerateReport(pag))
    {
      return;
    }

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    DateTime date1 = DateTime.Today,
      date2 = DateTime.Today;
    List<string> lstData = new List<string>();
    string tmp = null;
    bool isAsync = false;

    rptParse.ReportingID = "20001";

    //if (tipeDetail.Equals("Total"))
    //{
    //    rptParse.ReportingID = "20001";
    //}
    //else
    //{
    //    rptParse.ReportingID = "20001-a";
    //}



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
        ParameterName = "type",
        IsSqlParameter = true,
        ParameterValue = ChkDetail.Checked ? "Detail" : "Total"
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "ID",
        IsSqlParameter = true,
        ParameterValue = pag.Nip
    });

    if (ChkDetail.Checked)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "Temp_rptAsuransi.c_gdg",
            ParameterValue = (string.IsNullOrEmpty(cbGudang.SelectedItem.Value) ? "0" : cbGudang.SelectedItem.Value)
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
        DataType = typeof(char).FullName,
        ParameterName = "cusno",
        IsSqlParameter = true,
        ParameterValue = cbCustomer.SelectedItem.Value == null ? "0000" : cbCustomer.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(char).FullName,
        ParameterName = "exp",
        IsSqlParameter = true,
        ParameterValue = cbExpedisi.SelectedItem.Value == null ? "XX" : cbExpedisi.SelectedItem.Value
    });
    #endregion

    #region Report Parameter
    //if (tipeDetail == "Detail")
    //{
    //    lstRptParam.Add(new ReportParameter()
    //    {
    //        DataType = typeof(string).FullName,
    //        ParameterName = "Temp_rptasuransi.c_user",
    //        ParameterValue = pag.Nip
    //    });

    //    lstRptParam.Add(new ReportParameter()
    //    {
    //        DataType = typeof(string).FullName,
    //        ParameterName = "Temp_rptAsuransi.c_gdg",
    //        ParameterValue = (string.IsNullOrEmpty(cbGudang.SelectedItem.Value) ? "0" : cbGudang.SelectedItem.Value)
    //    });

    //    if (!string.IsNullOrEmpty(cbCustomer.SelectedItem.Value))
    //    {
    //        lstRptParam.Add(new ReportParameter()
    //        {
    //            DataType = typeof(string).FullName,
    //            ParameterName = "Temp_rptAsuransi.c_cusno",
    //            ParameterValue = cbCustomer.SelectedItem.Value
    //        });
    //    }

    //    if (!string.IsNullOrEmpty(cbExpedisi.SelectedItem.Value))
    //    {
    //        lstRptParam.Add(new ReportParameter()
    //        {
    //            DataType = typeof(string).FullName,
    //            ParameterName = "Temp_rptAsuransi.c_exp",
    //            ParameterValue = cbExpedisi.SelectedItem.Value
    //        });
    //    }
    //}
    #region Old Coded

    //if (cbCustomer.SelectedItems.Count > 1)
    //{
    //  for (int nLoop = 0, nlen = cbCustomer.SelectedItems.Count; nLoop < nlen; nLoop++)
    //  {
    //    tmp = cbCustomer.SelectedItems[nLoop].Value;
    //    if (!string.IsNullOrEmpty(tmp))
    //    {
    //      lstData.Add(string.Concat("'", tmp, "'"));
    //    }
    //  }

    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = string.Format("({{Temp_rptAsuransi.c_cusno}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbCustomer.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "Temp_rptAsuransi.c_cusno",
    //    ParameterValue = (cbCustomer.SelectedItems[0].Value == null ? string.Empty : cbCustomer.SelectedItems[0].Value)
    //  });
    //}

    //if (cbExpedisi.SelectedItems.Count > 1)
    //{
    //  for (int nLoop = 0, nlen = cbExpedisi.SelectedItems.Count; nLoop < nlen; nLoop++)
    //  {
    //    tmp = cbExpedisi.SelectedItems[nLoop].Value;
    //    if (!string.IsNullOrEmpty(tmp))
    //    {
    //      lstData.Add(string.Concat("'", tmp, "'"));
    //    }
    //  }

    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = string.Format("({{Temp_rptAsuransi.c_expno}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbExpedisi.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "Temp_rptAsuransi.c_expno",
    //    ParameterValue = (cbExpedisi.SelectedItems[0].Value == null ? string.Empty : cbExpedisi.SelectedItems[0].Value)
    //  });
    //}

    #endregion

    lstData = cbgTipeReport.CheckedTags;

    if (lstData.Count > 0)
    {
      if (lstData.Contains("DO"))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "IsNull({Temp_rptAsuransi.c_dono}) = false",
          IsReportDirectValue = true
        });
      }
      if (lstData.Contains("Expedisi"))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "IsNull({Temp_rptAsuransi.c_expno}) = false",
          IsReportDirectValue = true
        });
      }
    }

    lstData.Clear();

    #endregion

    rptParse.PaperID = "17.5*11";
    rptParse.ReportCustomizeText = null;
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;

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

            string rptName = string.Empty;

            if (ChkDetail.Checked)
            {
                rptName = string.Concat("History_Asuransi_Detail", pag.Nip, ".", rptResult.Extension);

            }

            else
            {

                rptName = string.Concat("History_Asuransi_Total", pag.Nip, ".", rptResult.Extension);

            }

                string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
                tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
                  tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

                //wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
                Functional.GeneratorLoadedWindow(hidWndDown.Text, tmpUri, LoadMode.IFrame);
        } 
            else
            {
                Functional.ShowMsgWarning(rptResult.MessageResponse);
            }
        }
    
    
    GC.Collect();
  }
}
