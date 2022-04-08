using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Ext.Net;
using Scms.Web.Core;

public partial class reporting_ServiceLevel_PO : System.Web.UI.UserControl
{

    public void InitializePage(string wndDownload)
    {
        hidWndDown.Text = wndDownload;
        DateTime now = DateTime.Now;
        DateTime date = new DateTime(now.Year, now.Month, 1);
        txPeriode1.Text = date.ToString("dd-MM-yyyy");
        txPeriode2.Text = DateTime.Now.ToString("dd-MM-yyyy");

  
    }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack && this.Visible)
    {
      Scms.Web.Core.PageHandler page = this.Page as Scms.Web.Core.PageHandler;
      //cbPosisiTrx.Hidden = true;

      //gridMain.Hidden = true;

      

      //if ((!page.IsCabang) || (!page.IsSupplier))
      //{
      //  Functional.SetComboData(cbPosisiStok, "c_gdg", page.ActiveGudangDescription, page.ActiveGudang);

      //  Functional.SetComboData(cbPosisiTrx, "c_gdg", page.ActiveGudangDescription, page.ActiveGudang);
          
      //}
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void report_ongenerate(object sender, DirectEventArgs e)
  {


      Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;


      string tipeStok = (e.ExtraParams["TipeService"] ?? string.Empty);
      string NoPO = (e.ExtraParams["NoPO"] ?? string.Empty);
      string idSup = (e.ExtraParams["Suplier"] ?? string.Empty);
      string kddivpri = (e.ExtraParams["kddivpri"] ?? string.Empty);
      string kddivams = (e.ExtraParams["kddivams"] ?? string.Empty);


      
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




      rptParse.ReportingID = "20031";

      //isAsync = chkAsync.Checked;

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
          ParameterName = "Suplier",
          IsSqlParameter = true,
          ParameterValue = string.IsNullOrEmpty(cbSuplier.SelectedItem.Value) ? "00000" : cbSuplier.SelectedItem.Value
      });

      lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(string).FullName,
          ParameterName = "DivPrinsipal",
          IsSqlParameter = true,
          ParameterValue = string.IsNullOrEmpty(cbDivPrinsipal.SelectedItem.Value) ? "000" : cbDivPrinsipal.SelectedItem.Value
      });

      lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(string).FullName,
          ParameterName = "divisiAMS",
          IsSqlParameter = true,
          ParameterValue = string.IsNullOrEmpty(cbdivisiAMS.SelectedItem.Value) ? "000" : cbdivisiAMS.SelectedItem.Value
      });


      lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(string).FullName,
          ParameterName = "Type",
          IsSqlParameter = true,
          ParameterValue = string.IsNullOrEmpty(SelectBoxTipeServiceLevel.SelectedItem.Value) ? "0" : SelectBoxTipeServiceLevel.SelectedItem.Value
      });

      lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(string).FullName,
          ParameterName = "NoPO",
          IsSqlParameter = true,
          ParameterValue = string.IsNullOrEmpty(NoPO) ? "0000000000" : NoPO
      });


                
      

      

      #endregion

      
      rptParse.IsLandscape = false;
      rptParse.PaperID = "14*8.5";
      rptParse.ReportParameter = lstRptParam.ToArray();
      rptParse.User = pag.Nip;
      rptParse.OutputReport = ReportParser.ParsingOutputReport("02");

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
              if (tipeStok == "1")
              {
                  rptName = string.Concat("Service_Level_FulfilmentRate_", pag.Nip, ".", rptResult.Extension);
              }
              else if (tipeStok == "2")
              {
                  rptName = string.Concat("Service_Level_Line_FulfilmentRate_", pag.Nip, ".", rptResult.Extension);
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
