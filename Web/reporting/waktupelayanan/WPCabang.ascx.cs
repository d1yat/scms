using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_konsolidasi_WPCabang : System.Web.UI.UserControl
{
  public void InitializePage(string wndDownload)
  {
    hidWndDown.Text = wndDownload;
    
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
    UserInformation userinfo = this.Session[Constant.SESSION_LOGIN_INFORMATION] as UserInformation;

    if (string.IsNullOrEmpty(pag.Supplier))
    {
        cbTipeTransaksiSup.Hidden = true;
        cbSuplier.Value = userinfo.Prinsipal;
        cbSuplier.Text = userinfo.PrinsipalDeskripsi;
        cbDivPrinsipal.Value = userinfo.DivisiPrinsipal;
        cbDivPrinsipal.Text = userinfo.DivisiPrinsipalDeskripsi;
    }
    else
    {
        Functional.SetComboData(cbSuplier, "c_nosup", pag.SupplierDescription, pag.Supplier);
        cbSuplier.Disabled = true;
        cbTipeTransaksi.Hidden = true;
        cbSuplier.Value = userinfo.Prinsipal;
        cbSuplier.Text = userinfo.PrinsipalDeskripsi;
        cbDivPrinsipal.Value = userinfo.DivisiPrinsipal;
        cbDivPrinsipal.Text = userinfo.DivisiPrinsipalDeskripsi;        
    }
    //string res = soa.GlobalQueryService(0, 0, true, string.Empty, string.Empty, "2011-b", param);

    //if (userinfo.IsCabang == false)
    //{
    //    cbCustomer.Hidden = true;
    //    cbCustomer2.Hidden = false;
    //}
    //else
    //{
    cbCustomer2.Hidden = true;
    //    cbCustomer.Hidden = false;
    //}
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
    
    if (!Functional.CanCreateGenerateReport(pag))
    {
      return;
    }

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();
    List<ReportCustomizeText> lstCustTxt = new List<ReportCustomizeText>();

    DateTime date1 = DateTime.Today,
      date2 = DateTime.Today;
    List<string> lstData = new List<string>();
    //string tmp = null;
    bool isAsync = false;

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

    if (string.IsNullOrEmpty(pag.Supplier))
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "tipeTrx",
            IsSqlParameter = true,
            ParameterValue = (cbTipeTransaksi.SelectedItem.Value == null ? "??" : cbTipeTransaksi.SelectedItem.Value)
        });
    }
    else
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "tipeTrx",
            IsSqlParameter = true,
            ParameterValue = (cbTipeTransaksiSup.SelectedItem.Value == null ? "??" : cbTipeTransaksiSup.SelectedItem.Value)
        });
    }
      
    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "gdg",
      IsSqlParameter = true,
      ParameterValue = (cbGudang.SelectedItem.Value == null ? "0" : cbGudang.SelectedItem.Value)
    });
    //if(pag.IsCabang == false)
    //{
    //    lstRptParam.Add(new ReportParameter()
    //    {
    //      DataType = typeof(string).FullName,
    //      ParameterName = "customer",
    //      IsSqlParameter = true,
    //      //ParameterValue = (cbCustomer.SelectedItem.Value == null ? "0000" : cbCustomer.SelectedItem.Value)
    //      ParameterValue = (cbCustomer2.SelectedItem.Value == null ? "0000" : cbCustomer2.SelectedItem.Value)
    //        });
    //}
    //else
    //{
    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "customer",
        IsSqlParameter = true,
        ParameterValue = (cbCustomer.SelectedItem.Value == null ? "0000" : cbCustomer.SelectedItem.Value)
        //ParameterValue = (cbCustomer2.SelectedItem.Value == null ? "0000" : cbCustomer2.SelectedItem.Value)
    });
    //}

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "suplier",
      IsSqlParameter = true,
      ParameterValue = (cbSuplier.SelectedItem.Value == null ? "00000" : cbSuplier.SelectedItem.Value)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "divSuplier",
      IsSqlParameter = true,
      ParameterValue = (cbDivPrinsipal.SelectedItem.Value == null ? "000" : cbDivPrinsipal.SelectedItem.Value)
    });
    
    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "item",
      IsSqlParameter = true,
      ParameterValue = (cbItems.SelectedItem.Value == null ? "0000" : cbItems.SelectedItem.Value)
    });

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
      ParameterName = "TEMP_LGWP.c_user",
      ParameterValue = pag.Nip
    });

    //lstData = rbgTipeReport.CheckedTags;


    if (rbgTRDtl.Checked == true)
      {
        rptParse.ReportingID = "20201-1";
      }
    else if (rbgTRSDP.Checked == true)
      {
        rptParse.ReportingID = "20201-2";
      }
    else if (rbgTRSC.Checked == true)
      {
        rptParse.ReportingID = "20201-3";
      }


    lstData.Clear();

    #endregion

    lstCustTxt.Add(new ReportCustomizeText()
    {
        SectionName = "Section2",
        ControlName = "txtPeriode",
        Value = string.Format("Periode : {0} s/d {1}", date1.ToString("dd-MMM-yyyy"), date2.ToString("dd-MMM-yyyy"))
    });


    rptParse.PaperID = "A3";
    rptParse.IsLandscape = false;
    rptParse.ReportCustomizeText = lstCustTxt.ToArray();
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
          string rptName = string.Concat("Report_Waktu_Pelayanan_Cabang_", pag.Nip, ".", rptResult.Extension);

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
