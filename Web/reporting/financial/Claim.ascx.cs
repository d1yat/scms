using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_Claim : System.Web.UI.UserControl
{
    private const string sValStringRadio = "01";

  public void InitializePage(string wndDownload)
  {
    hidWndDown.Text = wndDownload;
  }

  protected void Page_Load(object sender, EventArgs e)
  {
      if (!this.IsPostBack)
      {
          DateTime date = DateTime.Now;
          Functional.PopulateTahun(cbTahun, date.Year, 1, 0);
      }
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

      tmpHdFile = (hfStatus.Value == "" ? "01" : hfStatus.Value.ToString());
    
    isAsync = chkAsync.Checked;

    rptParse.ReportingID = "20409";
    
    #region Sql Parameter

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "tipereport",
        IsSqlParameter = true,
        ParameterValue = tmpHdFile
    });


    #region Report Parameter Claim Total


    if (tmpHdFile == "01")
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(int).FullName,
            ParameterName = "tahun",
            IsSqlParameter = true,
            ParameterValue = cbTahun.SelectedItem.Value
        });

        Scms.Web.Common.Functional.DateParser(txPeriode2.RawText.Trim(), "d-M-yyyy", out date1);
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(DateTime).FullName,
            ParameterName = "date1",
            IsSqlParameter = true,
            ParameterValue = date1.ToString("d-M-yyyy")
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "baseondetail",
            IsSqlParameter = true,
            ParameterValue = cbTipeDetail.SelectedItem.Value
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
    #endregion

    #region Report Parameter

    #region Claim Total
    if (tmpHdFile == "01")
    {

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{Temp_LGClaim.c_user}} = '{0}')", pag.Nip),
            IsReportDirectValue = true
        });


        if (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value))
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("({{LG_DatSup.c_nosup}} = '{0}')", cbSuplier.SelectedItem.Value),
                IsReportDirectValue = true
            });
        }

        if (!string.IsNullOrEmpty(cbDivPrinsipal.SelectedItem.Value))
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("({{FA_MsDivPri.c_kddivpri}} = '{0}')", cbDivPrinsipal.SelectedItem.Value),
                IsReportDirectValue = true
            });
        }

        if (!string.IsNullOrEmpty(cbType.SelectedItem.Value))
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("({{Temp_LGClaim.c_type}} = '{0}')", cbType.SelectedItem.Value),
                IsReportDirectValue = true
            });
        }
    }
    #endregion

    #region Claim Detail

    if (tmpHdFile == "02")
    {
        if (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value))
        {
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = string.Format("({{LG_DatSup.c_nosup}} = '{0}')", cbSuplier.SelectedItem.Value),
                IsReportDirectValue = true
            });
        }

        if (txClaimDetail1.Text.Trim().Length > 0)
        {
            if (txClaimDetail2.Text.Trim().Length > 0)
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = string.Format("({LG_ClaimH.c_nosup} In '{0}' To '{1}')", txClaimDetail1.Text, txClaimDetail2.Text),
                    IsReportDirectValue = true
                });
            }
            else
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = string.Format("({{LG_ClaimH.c_nosup}} = '{0}')", txClaimDetail1.Text),
                    IsReportDirectValue = true
                });
            }
        }
    }
    #endregion


    lstData.Clear();

    #endregion

    rptParse.IsLandscape = true;
    rptParse.PaperID = "A3";
    rptParse.ReportCustomizeText = null;
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;

    rptParse.OutputReport = ReportParser.ParsingOutputReport((cbRptTypeOutput.SelectedItem != null ? cbRptTypeOutput.SelectedItem.Value : string.Empty));
    //rptParse.IsShared = chkShare.Checked;
    //rptParse.UserDefinedName = txRptUName.Text.Trim();

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
          string rptName = string.Concat("Report_Claim_", pag.Nip, ".", rptResult.Extension);

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
