using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Ext.Net;
using Scms.Web.Core;
using System.Text;


public partial class recall : System.Web.UI.UserControl
{

    public void InitializePage(string wndDownload)
    {
        hidWndDown.Text = wndDownload;
        DateTime now = DateTime.Now;
        DateTime date = new DateTime(now.Year, now.Month, 1);
        txPeriode1.Text = date.ToString("dd-MM-yyyy");
        txPeriode2.Text = DateTime.Now.ToString("dd-MM-yyyy");

        //Indra 20181019FM
        //txDayResiHdr.Text = DateTime.Now.ToString("dd-MM-yyyy");
        txPeriode3.Text = DateTime.Now.ToString("dd-MM-yyyy");

  
    }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack && this.Visible)
    {
      Scms.Web.Core.PageHandler page = this.Page as Scms.Web.Core.PageHandler;
           

    

      if (!this.IsPostBack)
      {
          //ServiceLevelCabang.Initialize(Store5.ClientID);
      }


     
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void report_ongenerate(object sender, DirectEventArgs e)
  {

      Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

      string Kode_Cabang = (e.ExtraParams["cabang"] ?? string.Empty);
      string Gudang = (e.ExtraParams["Gudang"] ?? string.Empty);
      string Type = (e.ExtraParams["TypeService"] ?? string.Empty);
      string Proses = (e.ExtraParams["Proses"] ?? string.Empty);
      string NoSP = (e.ExtraParams["noSP"] ?? string.Empty);
      string Batch = (e.ExtraParams["Batch"] ?? string.Empty);
      
      if (!Functional.CanCreateGenerateReport(pag))
      {
          return;
      }

      ReportParser rptParse = new ReportParser();

      List<ReportParameter> lstRptParam = new List<ReportParameter>();
      List<ReportCustomizeText> lstCustTxt = new List<ReportCustomizeText>();

      DateTime date1 = DateTime.Today,
               date3 = DateTime.Today,
               date2 = DateTime.Today;
       
      List<string> lstData = new List<string>();
      string stype = null;
      bool isAsync = false;

      isAsync = chkAsync.Checked;

      //Indra 20181019FM
      #region Validasi 

      if (cbItems.SelectedItem.Text == "")
      {
          Functional.ShowMsgWarning("Nama produk belum dipilih, silahkan pilih terlebih dahulu");
          return;
      }

      if (sbTipeLaporan.SelectedItem.Text == "")
      {
          Functional.ShowMsgWarning("Tipe laporan belum dipilih, silahkan pilih terlebih dahulu");
          return;
      }

      if (sbFormatLaporan.SelectedItem.Text == "")
      {
          Functional.ShowMsgWarning("Format laporan belum dipilih, silahkan pilih terlebih dahulu");
          return;
      }

      #endregion

      //Indra 20181019FM
      #region Sql Parameter

      //if (Scms.Web.Common.Functional.DateParser(txPeriode1.RawText.Trim(), "d-M-yyyy", out date1))
      //{
      //    if (Scms.Web.Common.Functional.DateParser(txPeriode2.RawText.Trim(), "d-M-yyyy", out date2))
      //    {
      //        if (date2.CompareTo(date1) <= 0)
      //        {
      //            date2 = date1;
      //        }
      //    }
      //    else
      //    {
      //        date2 = date1;
      //    }




      //    //if (Scms.Web.Common.Functional.DateParser(txDayResiHdr.RawText.Trim(), "d-M-yyyy", out date3))
      //    if (Scms.Web.Common.Functional.DateParser(txPeriode3.RawText.Trim(), "d-M-yyyy", out date3))
      //    {
      //        if (date3.CompareTo(date1) <= 0)
      //        {
      //            date3 = date1;
      //        }

      //    }
      //    else
      //    {
      //        date3 = date1;
      //    }



      //    lstRptParam.Add(new ReportParameter()
      //    {
      //        DataType = typeof(DateTime).FullName,
      //        ParameterName = "date1",
      //        IsSqlParameter = true,
      //        ParameterValue = date1.ToString("d-M-yyyy")
      //    });
      //    lstRptParam.Add(new ReportParameter()
      //    {
      //        DataType = typeof(DateTime).FullName,
      //        ParameterName = "date2",
      //        IsSqlParameter = true,
      //        ParameterValue = date2.ToString("d-M-yyyy")
      //    });

      //    lstRptParam.Add(new ReportParameter()
      //    {
      //        DataType = typeof(DateTime).FullName,
      //        ParameterName = "date3",
      //        IsSqlParameter = true,
      //        ParameterValue = date3.ToString("d-M-yyyy")
      //    });

      //}

     

      //lstRptParam.Add(new ReportParameter()
      //{
      //    DataType = typeof(string).FullName,
      //    ParameterName = "Kode_Cabang",
      //    IsSqlParameter = true,
      //    ParameterValue = string.IsNullOrEmpty(cbCustomerHdr.SelectedItem.Value) ? "0000" : cbCustomerHdr.SelectedItem.Value
      //});



      //lstRptParam.Add(new ReportParameter()
      //{
      //    DataType = typeof(string).FullName,
      //    ParameterName = "Gudang",
      //    IsSqlParameter = true,
      //    ParameterValue = string.IsNullOrEmpty(cbGudang.SelectedItem.Value) ? "0" : cbGudang.SelectedItem.Value
      //});



      //lstRptParam.Add(new ReportParameter()
      //{
      //    DataType = typeof(string).FullName,
      //    ParameterName = "Type",
      //    IsSqlParameter = true,
      //    ParameterValue = string.IsNullOrEmpty(SelectBoxTipeServiceLevel.SelectedItem.Value) ? "0" : SelectBoxTipeServiceLevel.SelectedItem.Value
      //});



      //lstRptParam.Add(new ReportParameter()
      //{
      //    DataType = typeof(string).FullName,
      //    ParameterName = "Nip",
      //    IsSqlParameter = true,
      //    ParameterValue = pag.Nip
      //});


      //lstRptParam.Add(new ReportParameter()
      //{
      //    DataType = typeof(string).FullName,
      //    ParameterName = "item",
      //    IsSqlParameter = true,
      //    ParameterValue = string.IsNullOrEmpty(cbItems.SelectedItem.Value) ? "0000" : cbItems.SelectedItem.Value
      //});

      //lstRptParam.Add(new ReportParameter()
      //{
      //    DataType = typeof(string).FullName,
      //    ParameterName = "Batch",
      //    IsSqlParameter = true,
      //    ParameterValue = string.IsNullOrEmpty(cbBatDtl.SelectedItem.Value) ? "000000000000000" : cbBatDtl.SelectedItem.Value
      //});


      #endregion

      //Indra 20181019FM
      #region report parameter

      //lstCustTxt.Add(new ReportCustomizeText()
      //{
      //    SectionName = "Section1",
      //    ControlName = "txtPeriode",
      //    Value = string.Format(": {0} {1}", date1.ToString("yyyy-M-d") + " s/d ", date2.ToString("yyyy-M-d"))
      //});



      
      
      // lstCustTxt.Add(new ReportCustomizeText()
      //  {
      //      SectionName = "Section2",
      //      ControlName = "txtProduct",
      //     //Value = ": " + pag.Nip + " - " + pag.Username
      //      Value = string.IsNullOrEmpty(cbItems.SelectedItem.Value) ? "0000" : cbItems.SelectedItem.Value
      //  });


      // lstCustTxt.Add(new ReportCustomizeText()
      //{
      //    SectionName = "Section3",
      //    ControlName = "txPenarikan",
      //    Value = ": " + date3.ToString("yyyy-M-d")
      //});


       #endregion


      lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(string).FullName,
          ParameterName = "Type",
          IsSqlParameter = true,
          ParameterValue = sbTipeLaporan.SelectedItem.Value
      });

      //Indra 20181019FM
      #region Parameters Report

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

           if (Scms.Web.Common.Functional.DateParser(txPeriode3.RawText.Trim(), "d-M-yyyy", out date3))
           {
               if (date3.CompareTo(date2) <= 0)
               {
                   date3 = date2;
               }
           }
           else
           {
               date3 = date2;
           }
      }

      lstCustTxt.Add(new ReportCustomizeText()
      {
           SectionName = "Section1",
           ControlName = "txtPeriodeDistribusi",
           Value = string.Format(": {0} {1}", date1.ToString("yyyy-M-d") + " s/d ", date2.ToString("yyyy-M-d"))
      });

      lstCustTxt.Add(new ReportCustomizeText()
      {
           SectionName = "Section2",
           ControlName = "txtPeriodeRecall",
           Value = ": " + date3.ToString("yyyy-M-d")
      });

      lstCustTxt.Add(new ReportCustomizeText()
      {
            SectionName = "Section3",
            ControlName = "txtProduct",
            Value = ": " + cbItems.SelectedItem.Text
      });
      #endregion

      //Indra 20181019FM
      #region old code
      //if (Proses == "1")
      //{
      //    rptParse.ReportingID = "20036";

      //    rptParse.ReportCustomizeText = lstCustTxt.ToArray();
      //    rptParse.ReportParameter = lstRptParam.ToArray();


      //    string xmlFiles = ReportParser.Deserialize(rptParse);

      //    SoaReportCaller soa = new SoaReportCaller();

      //    string result = soa.GeneratorReport(isAsync, xmlFiles);

      //    ReportingResult rptResult = ReportingResult.Serialize(result);

      //    if (string.IsNullOrEmpty(result))
      //    {
      //        e.ErrorMessage = "Unknown response";

      //        e.Success = false;
      //    }
      //    else
      //    {
      //        {
      //            Functional.ShowMsgInformation("Data telah terproses.");
      //        }
      //    }

      //    GC.Collect();


      //}
      #endregion

      if (Type == "1" || Type == "2" )
      {
          rptParse.ReportingID = "2003346";

          //rptParse.IsLandscape = false;
          //rptParse.PaperID = "14*8.5";
          rptParse.IsLandscape = true;
          rptParse.PaperID = "A4";
          rptParse.ReportCustomizeText = lstCustTxt.ToArray();
          rptParse.ReportParameter = lstRptParam.ToArray();
          rptParse.User = pag.Nip;
          //rptParse.OutputReport = ReportParser.ParsingOutputReport("02");
          rptParse.OutputReport = ReportParser.ParsingOutputReport(sbFormatLaporan.SelectedItem.Value);

          string xmlFiles = ReportParser.Deserialize(rptParse);

          SoaReportCaller soa = new SoaReportCaller();

          string result = soa.GeneratorReport(xmlFiles);

          ReportingResult rptResult = ReportingResult.Serialize(result);

          if (rptResult == null)
          {
              Functional.ShowMsgError("Pembuatan report gagal.");
          }
          else
          {
              if (rptResult.IsSuccess)
              {
                  string rptName = null;
                  if (Type == "1")
                  {
                      //Indra 20181019FM
                      //rptName = string.Concat("Service_Level_Cabang_FL", pag.Nip, ".", rptResult.Extension);
                      rptName = string.Concat("History_Recall_", pag.Nip, ".", rptResult.Extension);
                  }
                  else if (Type == "2")
                  {
                      rptName = string.Concat("Service_Level_Cabang_LFR", pag.Nip, ".", rptResult.Extension);
                  }
                  else if (Type == "3")
                  {
                      rptName = string.Concat("Service_Level_Cabang_FL_MENIT", pag.Nip, ".", rptResult.Extension);
                  }

                  else if (Type == "4")
                  {
                      rptName = string.Concat("Service_Level_Cabang_LFR_MENIT", pag.Nip, ".", rptResult.Extension);
                  }

                  string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
                  tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
                    tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

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

}
