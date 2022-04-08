using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_Transaksi_LaporanPBF : System.Web.UI.UserControl
{
  public void InitializePage(string wndDownload,string tipe)
  {
    hidWndDown.Text = wndDownload;
    txPeriode1.Text = DateTime.Now.ToString("01-MM-yyyy");
    txPeriode2.Text = DateTime.Now.ToString("dd-MM-yyyy");

      //hafizh
    hfTipe.Text = tipe;
      // hafizh end

    //Indra 20170421
    if (tipe == "NonRegulerBPOM")
    {
        rdgTipeReport.Enabled = true;
        CompositeField1.Visible = true;
        rdgTipeReport.Visible = true;
        CompositeField2.Visible = false;
    }
    else
    {
        rdgTipeReport.Visible = false;
        CompositeField1.Visible = false;
        rdgTipeReport.Visible = false;
        CompositeField2.Visible = true;
    }
      //Indra End

  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if ((!this.IsPostBack) && this.Visible)
    {
      DateTime date = DateTime.Now;

      //Functional.PopulateTahun(cbTahun, date.Year, 1, 0);

      Functional.SetComboData(cbGudang, "c_gdg", "Gudang 1", "1");

      //Functional.SetComboData(cbPeriode, "c_type", "Periode 1", "01");
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

      // hafizh
    string TipeID = (e.ExtraParams["TipeID"] ?? string.Empty);
      // end hafizh

    if (!Functional.CanCreateGenerateReport(pag))
    {
      return;
    }

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    DateTime date1 = DateTime.Today,
      date2 = DateTime.Today;
    //List<string> lstData = new List<string>();
    //string tmp = null,
    //  tmp1 = null;
    bool isAsync = false;

      
    rptParse.ReportingID = "21015";

    isAsync = chkAsync.Checked;

    String TipeReport = "07"; //Indra 20190320FM

    #region Sql Parameter

    if (TipeReport == "07")
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "TipeReport",
            IsSqlParameter = true,
            ParameterValue = TipeReport 
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(DateTime).FullName,
            ParameterName = "periode",
            IsSqlParameter = true,
            ParameterValue = SBPeriode.SelectedItem.Value
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(DateTime).FullName,
            ParameterName = "tahun",
            IsSqlParameter = true,
            ParameterValue = sbTahun.SelectedItem.Value
        });
    }
    else
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
    }

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(DateTime).FullName,
        ParameterName = "gdg",
        IsSqlParameter = true,
        ParameterValue = cbGudang.SelectedItem.Value
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "user",
        IsSqlParameter = true,
        ParameterValue = pag.Nip
    });

    #endregion

    #region Report Parameter

    //String TipeReport; //Indra 20170404

    if (rdgNonRetur.Checked)
    {
        //Indra 20170404
        if (TipeID.ToLower() == "reguler")
        {
            TipeReport = "01";
        }
        else if (TipeID.ToLower() == "nonreguler")
        {
            TipeReport = "03";
        }
        else
        {
            TipeReport = "05";
        }
        //IndraEnd

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "TipeReport",
            IsSqlParameter = true,
            //ParameterValue = TipeID.ToLower() == "reguler" ? "01" : "3" //hafizh
            ParameterValue = TipeReport //Indra 20170404
        });
    }
    else if (rdgRetur.Checked)
    {
        //Indra 20170404
        if (TipeID.ToLower() == "reguler")
        {
            TipeReport = "02";
        }
        else if (TipeID.ToLower() == "nonreguler")
        {
            TipeReport = "04";
        }
        else
        {
            TipeReport = "06";
        }
        //IndraEnd

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "TipeReport",
            IsSqlParameter = true,
            //ParameterValue = TipeID.ToLower() == "reguler" ? "02" : "4" //hafizh
            ParameterValue = TipeReport //Indra 20170404
        });
    }

   
    #endregion

    rptParse.PaperID = "A4";
    rptParse.IsLandscape = true;
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
          string rptName = string.Concat("Laporan_PBF_", pag.Nip, ".", rptResult.Extension);

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
