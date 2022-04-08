using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_pending_ReturSuplier : System.Web.UI.UserControl
{
  public void InitializePage(string wndDownload)
  {
    hidWndDown.Text = wndDownload;
    Functional.SetComboData(cbGudang, "c_gdg", "Gudang 1", "1");
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

    if (rdSum.Checked)
    {
        rptParse.ReportingID = "20309-1";
        rptParse.IsLandscape = false;
        rptParse.PaperID = "Letter";
    }
    else
    {
        rptParse.ReportingID = "20309-2";
        rptParse.IsLandscape = true;
        rptParse.PaperID = "A3";
    }


    #region Report Parameter

    if (rdSum.Checked)
    {
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
                    ParameterName = string.Format("({{LG_RSH.d_rsdate}} In {0} To {1})",
                        Functional.CrystalReportDateString(date1), Functional.CrystalReportDateString(date2)),
                    IsReportDirectValue = true
                });
            }
            else if (!date1.Equals(DateTime.MinValue))
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "LG_RSH.d_rsdate",
                    IsReportDirectValue = true,
                    ParameterValue = Functional.CrystalReportDateString(date1)
                });
            }
        }

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "({LG_RSH.c_type} = '01')",
            IsReportDirectValue = true
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "LG_RSH.c_gdg",
            ParameterValue = (cbGudang.SelectedItem.Value == null ? string.Empty : cbGudang.SelectedItem.Value)
        });

        if (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value))
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = "LG_RSH.c_nosup",
                ParameterValue = (cbSuplier.SelectedItem.Value.ToString() == null ? string.Empty : cbSuplier.SelectedItem.Value.ToString())
            });
        }

        if (cbReportType.SelectedItem.Value == "01")
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = "IsNull({LG_FBRD2.c_fbno}) = false",
                IsReportDirectValue = true
            });
        }
        else if (cbReportType.SelectedItem.Value == "02")
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = "IsNull({LG_FBRD2.c_fbno}) = true",
                IsReportDirectValue = true
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
        //    ParameterName = string.Format("({{LG_RSH.c_nosup}} IN [{0}])", string.Join(",", lstData.ToArray())),
        //    IsReportDirectValue = true
        //  });

        //  lstData.Clear();
        //}
        //else if (cbSuplier.SelectedItems.Count == 1)
        //{
        //  lstRptParam.Add(new ReportParameter()
        //  {
        //    DataType = typeof(string).FullName,
        //    ParameterName = "LG_RSH.c_nosup",
        //    ParameterValue = (cbSuplier.SelectedItems[0].Value == null ? string.Empty : cbSuplier.SelectedItems[0].Value)
        //  });
        //}

        #endregion

        tmp = txRS1.Text.Trim();
        tmp1 = txRS2.Text.Trim();

        if (!string.IsNullOrEmpty(tmp))
        {
            if ((!string.IsNullOrEmpty(tmp1)) && (!tmp.Equals(tmp1, StringComparison.OrdinalIgnoreCase)))
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = string.Format("({{LG_RSH.c_rsno}} In '{0}' To '{1}')", tmp, tmp1),
                    IsReportDirectValue = true
                });
            }
            else
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "LG_RSH.c_rsno",
                    ParameterValue = tmp1
                });
            }
        }
    }
    else if (rdDet.Checked)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "rtype",
            IsSqlParameter = true,
            ParameterValue = cbReportType.SelectedItem.Value
        });

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
                ParameterName = "nosup",
                IsSqlParameter = true,
                ParameterValue = cbSuplier.SelectedItem.Value == null ? "00000" : cbSuplier.SelectedItem.Value
            });

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
            ParameterName = "rsno1",
            IsSqlParameter = true,
            ParameterValue = string.IsNullOrEmpty(txRS1.Text) ? "0" : txRS1.Text
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "rsno2",
            IsSqlParameter = true,
            ParameterValue = string.IsNullOrEmpty(txRS2.Text) ? "0" : txRS2.Text
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "iteno",
            IsSqlParameter = true,
            ParameterValue = cbItems.Value == null ? "0000" : cbItems.SelectedItem.Value
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "session",
            IsSqlParameter = true,
            ParameterValue = pag.Nip
        });
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
          string rptName = string.Concat("Pending_RS_", pag.Nip, ".", rptResult.Extension);

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
