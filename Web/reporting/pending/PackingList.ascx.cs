using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_pending_PackingList : System.Web.UI.UserControl
{
    private const string sValStringRadio = "01";

  public void InitializePage(string wndDownload)
  {
    hidWndDown.Text = wndDownload;
    Functional.SetComboData(cbGudang, "c_gdg", "Gudang 1", "1");
  }

  protected void Page_Load(object sender, EventArgs e)
  {
      rdgStatusDO.Disabled = true;
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
      tmp1 = null,
      tmpHdFile = null;
    bool isAsync = false;

      tmpHdFile = (hfStatus.Value == null ? "01" : hfStatus.Value.ToString());
    
    isAsync = chkAsync.Checked;

    rptParse.ReportingID = "20311";
    
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

    #endregion

    #region Report Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_TempReportPendingPL.c_user",
      ParameterValue = pag.Nip
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_TempReportPendingPL.c_gdg",
      ParameterValue = (cbGudang.SelectedItem.Value == null ? string.Empty : cbGudang.SelectedItem.Value)
    });

    if (!string.IsNullOrEmpty(cbCustomer.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "LG_TempReportPendingPL.c_cusno",
        ParameterValue = (cbCustomer.SelectedItem.Value == null ? string.Empty : cbCustomer.SelectedItem.Value)
      });
    }

    if (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value))
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(string).FullName,
        ParameterName = "LG_TempReportPendingPL.c_nosup",
        ParameterValue = (cbSuplier.SelectedItem.Value == null ? string.Empty : cbSuplier.SelectedItem.Value)
      });
    }

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
    //    ParameterName = string.Format("({{LG_TempReportPendingPL.c_cusno}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbCustomer.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "LG_TempReportPendingPL.c_cusno",
    //    ParameterValue = (cbCustomer.SelectedItems[0].Value == null ? string.Empty : cbCustomer.SelectedItems[0].Value)
    //  });
    //}

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
    //    ParameterName = string.Format("({{LG_TempReportPendingPL.c_nosup}} IN [{0}])", string.Join(",", lstData.ToArray())),
    //    IsReportDirectValue = true
    //  });

    //  lstData.Clear();
    //}
    //else if (cbSuplier.SelectedItems.Count == 1)
    //{
    //  lstRptParam.Add(new ReportParameter()
    //  {
    //    DataType = typeof(string).FullName,
    //    ParameterName = "LG_TempReportPendingPL.c_nosup",
    //    ParameterValue = (cbSuplier.SelectedItems[0].Value == null ? string.Empty : cbSuplier.SelectedItems[0].Value)
    //  });
    //}

    #endregion

    if (!string.IsNullOrEmpty(tmp))
    {
      if ((!string.IsNullOrEmpty(tmp1)) && (!tmp.Equals(tmp1, StringComparison.OrdinalIgnoreCase)))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = string.Format("({{LG_TempReportPendingPL.c_dono}} In '{0}' To '{1}')", tmp, tmp1),
          IsReportDirectValue = true
        });
      }
      else
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_TempReportPendingPL.c_dono",
          ParameterValue = tmp1
        });
      }
    }

    if (rdgKConf.Checked == true)
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(bool).FullName,
        ParameterName = "LG_TempReportPendingPL.l_confirm",
        ParameterValue = "true"
      });
    }
    else if (rdgKUnConf.Checked == true)
    {
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(bool).FullName,
        ParameterName = "LG_TempReportPendingPL.l_confirm",
        ParameterValue = "false"
      });
    }

    if (tmpHdFile == sValStringRadio)
    {
        if (rdgDO.Checked == true)
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("isnull({{LG_TempReportPendingPL.c_dono}}) = false "),
                IsReportDirectValue = true
            });
        if (rdgNonDO.Checked == true)
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("isnull({{LG_TempReportPendingPL.c_dono}}) = true "),
                IsReportDirectValue = true
            });
        //Indra 20170711
        if (rdgNonEP.Checked == true)
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("isnull({{LG_TempReportPendingPL.c_resi}}) = true and isnull({{LG_TempReportPendingPL.c_dono}}) = false "),
                IsReportDirectValue = true
            });
        //End Indra 20170711
    }

    if (!string.IsNullOrEmpty(txPL1.Text))
    {
        if (string.IsNullOrEmpty(txPL2.Text))
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = "LG_TempReportPendingPL.c_plno",
                ParameterValue = (string.IsNullOrEmpty(txPL1.Text) ? string.Empty : txPL1.Text)
            });
        }
        else
        {
            if (txPL1.Text.CompareTo(txPL2.Text) >= 0)
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "LG_TempReportPendingPL.c_plno",
                    ParameterValue = (string.IsNullOrEmpty(txPL1.Text) ? string.Empty : txPL1.Text)
                });
            }
            else
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = string.Format("({{LG_TempReportPendingPL.c_plno}} IN ('{0}' TO '{1}'))", txPL1.Text, txPL2.Text),
                    IsReportDirectValue = true
                });
            }
        }
    }

    lstData.Clear();

    #endregion

    rptParse.IsLandscape = true;
    rptParse.PaperID = "A3";
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
          string rptName = string.Concat("Pending_PL_", pag.Nip, ".", rptResult.Extension);

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
