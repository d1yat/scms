using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_pending_DeliveryOrder : System.Web.UI.UserControl
{
  public void InitializePage(string wndDownload)
  {
    hidWndDown.Text = wndDownload;
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    DateTime date1 = DateTime.Today,
      date2 = DateTime.Today;
    List<string> lstData = new List<string>();
    string tmp = null,
      tmp1 = null;
    bool isAsync = false;
    
    isAsync = chkAsync.Checked;

    rptParse.ReportingID = "20306";


    #region Report Parameter

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
        //coba1 awal
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
        //coba1 akhir
    }

    
      
      
      lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "gdg",
        IsSqlParameter = true,
        ParameterValue = string.IsNullOrEmpty(cbGudang.SelectedItem.Value) ? "0" : cbGudang.SelectedItem.Value
        //ParameterValue = (cbGudang.SelectedItem.Value == null ? string.Empty : cbGudang.SelectedItem.Value)
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "cusno",
        IsSqlParameter = true,
        ParameterValue = string.IsNullOrEmpty(cbCustomer.SelectedItem.Value) ? "0000" : cbCustomer.SelectedItem.Value
    });




    //if (!string.IsNullOrEmpty(cbRptType.SelectedItem.Value))
    //{
    //    lstRptParam.Add(new ReportParameter()
    //    {
    //        DataType = typeof(string).FullName,
    //        ParameterName = "@List",
    //        IsSqlParameter = true,
    //        ParameterValue = (cbRptType.SelectedItem.Value.ToString() == null ? string.Empty : cbRptType.SelectedItem.Value.ToString())
    //    });
    //}

    //if (!string.IsNullOrEmpty(cbRptType.SelectedItem.Value))
    //    if (cbRptType.SelectedItem.Value == "Lunas")
    //    {
    //        lstRptParam.Add(new ReportParameter()
    //        {
    //            DataType = typeof(bool).FullName,
    //            ParameterName = "Temp_ListPendingDO.c_fjno",
    //            IsSqlParameter = true,
    //            ParameterValue = "true"
    //        });
    //    }
    //    else if (cbRptType.SelectedItem.Value == "Pending")
    //    {
    //        lstRptParam.Add(new ReportParameter()
    //        {
    //            DataType = typeof(bool).FullName,
    //            ParameterName = "Temp_ListPendingDO.c_fjno",
    //            IsSqlParameter = true,
    //            ParameterValue = "false"
    //        });
    //    }



    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(char).FullName,
        ParameterName = "List",
        IsSqlParameter = true,
        ParameterValue = cbRptType.SelectedItem.Value
    });






    //#region Old Coded

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
    //    ParameterName = string.Format("({{LG_DOH.c_cusno}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbCustomer.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "LG_DOH.c_cusno",
    //    ParameterValue = (cbCustomer.SelectedItems[0].Value == null ? string.Empty : cbCustomer.SelectedItems[0].Value)
    //  });
    //}

    //#endregion

    //tmp = txDO1.Text.Trim();
    //tmp1 = txDO2.Text.Trim();

    //if (!string.IsNullOrEmpty(tmp))
    //{
    //    if ((!string.IsNullOrEmpty(tmp1)) && (!tmp.Equals(tmp1, StringComparison.OrdinalIgnoreCase)))
    //    {
    //        lstRptParam.Add(new ReportParameter()
    //        {
    //            DataType = typeof(string).FullName,
    //            ParameterName = string.Format("({{Temp_ListPendingDO.c_dono}} In '{0}' To '{1}')", tmp, tmp1),
                
    //            IsSqlParameter = true,
                
    //            IsReportDirectValue = true
    //        });


    //    }
    //    else
    //    {
    //        lstRptParam.Add(new ReportParameter()
    //        {
    //            DataType = typeof(string).FullName,
    //            ParameterName = "Temp_ListPendingDO.c_dono",
                
    //            IsSqlParameter = true,
    //            ParameterValue = tmp1
    //        });
    //    }
    //}

    if (txDO1.Text.Length > 0)
    {
        if (txDO2.Text.Length > 0)
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("({{Temp_ListPendingDO.c_dono}} IN ['{0}','{1}'])", txDO1.Text.Trim(), txDO2.Text.Trim()),
                IsReportDirectValue = true
            });
        }
        else
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("({{Temp_ListPendingDO.c_dono}} >= '{0}')", txDO1.Text.Trim()),
                IsReportDirectValue = true
            });
        }
    }
    else if (txDO2.Text.Length > 0)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{Temp_ListPendingDO.c_dono}} <= '{0}')", txDO2.Text.Trim()),
            IsReportDirectValue = true
        });
    }


    #endregion

    rptParse.IsLandscape = true;
    rptParse.PaperID = "Letter";
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
        else
        {
          string rptName = string.Concat("Pending_DO_", pag.Nip, ".", rptResult.Extension);

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
