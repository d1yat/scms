using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_history_BiayaEkspedisi : System.Web.UI.UserControl
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
    List<string> lstData = new List<string>();
    string stype = null;
    bool isAsync = false;

    rptParse.ReportingID = "20023";

    isAsync = chkAsync.Checked;

    #region Sql Parameter

    #endregion

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
        ParameterName = "exp",
        IsSqlParameter = true,
        ParameterValue = cbExpedisi.SelectedIndex > -1 ? cbExpedisi.SelectedItem.Value : "AA"
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(char).FullName,
        ParameterName = "user",
        IsSqlParameter = true,
        ParameterValue = pag.Nip
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(char).FullName,
        ParameterName = "gdg",
        IsSqlParameter = true,
        ParameterValue = string.IsNullOrEmpty(cbGudang.SelectedItem.Value) ? "0" : cbGudang.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "iteno",
        IsSqlParameter = true,
        ParameterValue = "0000"
    });

    if (rdByResi.Checked)
    {
        stype = "resi";

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "tipereport",
            IsSqlParameter = true,
            ParameterValue = "01"
        });

        if (chkOutstanding.Checked)
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("(isnull({{LG_BED1.c_beno}}) = true)"),
                IsReportDirectValue = true
            });
        }
    }
    else if (rdByDO.Checked)
    {
        stype = "do";

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "tipereport",
            IsSqlParameter = true,
            ParameterValue = "02"
        });
    }
    else if (rdByItem.Checked)
    {
        stype = "item";

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "tipereport",
            IsSqlParameter = true,
            ParameterValue = "03"
        });
    }

    if (!string.IsNullOrEmpty(txIENo1.Text))
    {
        if (string.IsNullOrEmpty(txIENo2.Text))
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = "temp_biayaekspedisi" + stype + ".c_ieno",
                ParameterValue = (string.IsNullOrEmpty(txIENo1.Text) ? string.Empty : txIENo1.Text)
            });
        }
        else
        {
            if (txIENo1.Text.CompareTo(txIENo2.Text) >= 0)
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "temp_biayaekspedisi" + stype + ".c_ieno",
                    ParameterValue = (string.IsNullOrEmpty(txIENo1.Text) ? string.Empty : txIENo1.Text)
                });
            }
            else
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = string.Format("({{temp_biayaekspedisi" + stype + ".c_ieno}} IN ('{0}' TO '{1}'))", txIENo1.Text, txIENo2.Text),
                    IsReportDirectValue = true
                });
            }
        }
    }

    if (!string.IsNullOrEmpty(txNoResi1.Text))
    {
        if (string.IsNullOrEmpty(txNoResi2.Text))
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = "temp_biayaekspedisi" + stype + ".c_resi",
                ParameterValue = (string.IsNullOrEmpty(txNoResi1.Text) ? string.Empty : txNoResi1.Text)
            });
        }
        else
        {
            if (txNoResi1.Text.CompareTo(txNoResi2.Text) >= 0)
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "temp_biayaekspedisi" + stype + ".c_resi",
                    ParameterValue = (string.IsNullOrEmpty(txNoResi1.Text) ? string.Empty : txNoResi1.Text)
                });
            }
            else
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = string.Format("({{temp_biayaekspedisi" + stype + ".c_resi}} IN ('{0}' TO '{1}'))", txNoResi1.Text, txNoResi2.Text),
                    IsReportDirectValue = true
                });
            }
        }
    }

    if (!string.IsNullOrEmpty(txEPNo1.Text))
    {
        if (string.IsNullOrEmpty(txEPNo2.Text))
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = "temp_biayaekspedisi" + stype + ".c_expno",
                ParameterValue = (string.IsNullOrEmpty(txEPNo1.Text) ? string.Empty : txEPNo1.Text)
            });
        }
        else
        {
            if (txEPNo1.Text.CompareTo(txEPNo2.Text) >= 0)
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "temp_biayaekspedisi" + stype + ".c_expno",
                    ParameterValue = (string.IsNullOrEmpty(txEPNo1.Text) ? string.Empty : txEPNo1.Text)
                });
            }
            else
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = string.Format("({{temp_biayaekspedisi" + stype + ".c_expno}} IN ('{0}' TO '{1}'))", txEPNo1.Text, txEPNo2.Text),
                    IsReportDirectValue = true
                });
            }
        }
    }

    #endregion

    rptParse.PaperID = "17.5*11";
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;
    rptParse.IsLandscape = true;

    rptParse.ReportCustomizeText = new ReportCustomizeText[] {
      new ReportCustomizeText() {
         SectionName = "Section1",
          ControlName = "txtPeriode",
           Value = string.Format("Periode : {0} s/d {1}", date1.ToString("dd-MMM-yyyy"), date2.ToString("dd-MMM-yyyy"))
      }
    };

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
          string rptName = string.Concat("History_Biaya_Ekspedisi", pag.Nip, ".", rptResult.Extension);

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
