using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Common;

public partial class reporting_waktupelayanan_Default : Scms.Web.Core.PageHandler
{
  private const string WAKTUPELAYANAN_REPORT_CABANG = "cabang";
  private const string WAKTUPELAYANAN_REPORT_SUPLIER = "suplier";
  private const string WAKTUPELAYANAN_REPORT_SUPLIER_CABANG = "supliercabang";
  private const string WAKTUPELAYANAN_REPORT_SERAH_TERIMA = "serahterima";
  private const string WAKTUPELAYANAN_REPORT_SPPL_CONFIRM = "spplconfirm";
  private const string WAKTUPELAYANAN_REPORT_WPST_GDGRETUR = "gdgretur";



  protected void Page_Init(object sender, EventArgs e)
  {
    string qryString = null;

    if ((!this.IsPostBack) && (!Ext.Net.X.IsAjaxRequest))
    {
      qryString = (this.Request.QueryString.IsContainKey("mode") ? (this.Request.QueryString["mode"] ?? string.Empty).Trim() : string.Empty).ToLower();

      switch (qryString)
      {
        case WAKTUPELAYANAN_REPORT_CABANG:
          WPCabang1.InitializePage(wndDown.ClientID);
          WPCabang1.Visible = true;
          break;
        case WAKTUPELAYANAN_REPORT_SUPLIER:
          WPSuplier1.InitializePage(wndDown.ClientID);
          WPSuplier1.Visible = true;
          break;
        case WAKTUPELAYANAN_REPORT_SUPLIER_CABANG:
          WPSuplierCabang1.InitializePage(wndDown.ClientID);
          WPSuplierCabang1.Visible = true;
          break;
        case WAKTUPELAYANAN_REPORT_SERAH_TERIMA:
          WPSerahTerima1.InitializePage(wndDown.ClientID);
          WPSerahTerima1.Visible = true;
          break;
        case WAKTUPELAYANAN_REPORT_SPPL_CONFIRM:
          WPSPPLConfirm1.InitializePage(wndDown.ClientID);
          WPSPPLConfirm1.Visible = true;
          break;
        case WAKTUPELAYANAN_REPORT_WPST_GDGRETUR:
          WPSTGudangRetur1.InitializePage(wndDown.ClientID);
          WPSTGudangRetur1.Visible = true;
          break;
      }
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }
}
