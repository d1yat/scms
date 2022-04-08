using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_pending_ReceiveNote : System.Web.UI.UserControl
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

    rptParse.ReportingID = "20308";

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

      if ((!date1.Equals(DateTime.MinValue)) && (!date1.Equals(DateTime.MinValue)) && (!date1.Equals(date2)))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = string.Format("({{LG_RNH.d_rndate}} In {0} To {1})",
              Functional.CrystalReportDateString(date1), Functional.CrystalReportDateString(date2)),
          IsReportDirectValue = true
        });
      }
      else if (!date1.Equals(DateTime.MinValue))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_RNH.d_rndate",
          ParameterValue = Functional.CrystalReportDateString(date1)
        });
      }
    }

    if (rdgFFloa.Checked)
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(bool).FullName,
        ParameterName = "LG_RNH.l_float",
        ParameterValue = "true"
      });
    }
    else if (rdgFUnFloa.Checked)
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(bool).FullName,
        ParameterName = "LG_RNH.l_float",
        ParameterValue = "false"
      });
    }

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_RNH.c_gdg",
      ParameterValue = (cbGudang.SelectedItem.Value == null ? string.Empty : cbGudang.SelectedItem.Value)
    });

    if (rdLunas.Checked)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "IsNull({lg_fbd2.c_fbno}) = false",
            IsReportDirectValue = true
        });
    }
    else if (rdPending.Checked)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "IsNull({lg_fbd2.c_fbno}) = true",
            IsReportDirectValue = true
        });
    }


    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "({LG_RNH.c_type} = '01')",
        IsReportDirectValue = true
    });

    if (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "LG_RNH.c_from",
        ParameterValue = (cbSuplier.SelectedItem.Value.ToString() == null ? string.Empty : cbSuplier.SelectedItem.Value.ToString())
      });
    }

    #region Old Coded

    //if (cbSuplier.SelectedItems.Count > 1)
    //{
    //  for (int nLoop = 0, nlen = cbSuplier.SelectedItems.Count; nLoop < nlen; nLoop++)
    //  {
    //    tmp = cbSuplier.SelectedItems[nLoop].Value;
    //    if (!string.IsNullOrEmpty(tmp))
    //    {
    //      lstData.Add(string.Concat("'", tmp, "'"));
    //    }
    //  }

    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = string.Format("({{LG_RNH.c_from}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbSuplier.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "LG_RNH.c_from",
    //    ParameterValue = (cbSuplier.SelectedItems[0].Value == null ? string.Empty : cbSuplier.SelectedItems[0].Value)
    //  });
    //}

    #endregion

    tmp = txRN1.Text.Trim();
    tmp1 = txRN2.Text.Trim();

    if (tmp.Length > 0)
    {
        if (tmp1.Length > 0)
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("({{LG_RNH.c_rnno}} IN ['{0}','{1}'])", tmp, tmp1),
                IsReportDirectValue = true
            });
        }
        else
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("({{LG_RNH.c_rnno}} >= '{0}')", tmp),
                IsReportDirectValue = true
            });
        }
    }
    else if (tmp1.Length > 0)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{LG_RNH.c_rnno}} <= '{0}')", tmp1),
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
          string rptName = string.Concat("Pending_RN_", pag.Nip, ".", rptResult.Extension);

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
