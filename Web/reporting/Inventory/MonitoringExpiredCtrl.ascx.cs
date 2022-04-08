using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Ext.Net;
using Scms.Web.Core;

public partial class reporting_Inventory_MonitoringEDCtrl : System.Web.UI.UserControl
{

    public void InitializePage(string wndDownload)
    {
        hidWndDown.Text = wndDownload;
    }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack && this.Visible)
    {
      Scms.Web.Core.PageHandler page = this.Page as Scms.Web.Core.PageHandler;

      if ((!page.IsCabang) || (!page.IsSupplier))
      {
        Functional.SetComboData(cbPosisiStok, "c_gdg", page.ActiveGudangDescription, page.ActiveGudang);

        Functional.SetComboData(cbPosisiTrx, "c_gdg", page.ActiveGudangDescription, page.ActiveGudang);
          
      }
    }




  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void report_ongenerate(object sender, DirectEventArgs e)
  {
      Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

      string gdgId = (e.ExtraParams["PosisiStok"] ?? string.Empty); ;
      string idSup = (e.ExtraParams["Suplier"] ?? string.Empty);
      string tipeStok = (e.ExtraParams["TipeStock"] ?? string.Empty);

      if (string.IsNullOrEmpty(gdgId))
      {
          Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");

          return;
      }

      ReportParser rptParse = new ReportParser();

      List<ReportParameter> lstRptParam = new List<ReportParameter>();

      List<string> lstData = new List<string>();

      rptParse.ReportingID = "10011-y";

      #region Report Parameter

      lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(char).FullName,
          ParameterName = "gdg",
          IsSqlParameter = true,
          ParameterValue = gdgId
      });

      lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(string).FullName,
          ParameterName = "nosup",
          IsSqlParameter = true,
          ParameterValue = string.IsNullOrEmpty(idSup) ? "00000" : idSup 
      });

      lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(string).FullName,
          ParameterName = "type",
          IsSqlParameter = true,
          ParameterValue = SelectBoxTipeStock.SelectedItem.Value
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
              if (tipeStok == "01")
              {
                  rptName = string.Concat("Inventory_Stok_AktualPerBatch_Bad_", pag.Nip, ".", rptResult.Extension);
              }
              else
              {
                  rptName = string.Concat("Inventory_Stok_AktualPerBatch_Good_", pag.Nip, ".", rptResult.Extension);
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
