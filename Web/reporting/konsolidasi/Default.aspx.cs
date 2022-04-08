using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Common;

public partial class reporting_konsolidasi_Default : Scms.Web.Core.PageHandler
{
  private const string KONSOLIDASI_REPORT_BDP = "bdp";
  private const string KONSOLIDASI_REPORT_RDP = "rdp";
  private const string KONSOLIDASI_REPORT_BDPPHAROS = "bdppharos";
  private const string KONSOLIDASI_REPORT_STOKNASIONAL = "stoknasional";

  protected void Page_Init(object sender, EventArgs e)
  {
    string qryString = null;

    if ((!this.IsPostBack) && (!Ext.Net.X.IsAjaxRequest))
    {
      qryString = (this.Request.QueryString.IsContainKey("mode") ? (this.Request.QueryString["mode"] ?? string.Empty).Trim() : string.Empty).ToLower();

      switch (qryString)
      {
        case KONSOLIDASI_REPORT_BDP:
          BDP1.InitializePage(wndDown.ClientID);
          BDP1.Visible = true;
          break;
        case KONSOLIDASI_REPORT_RDP:
          RDP1.InitializePage(wndDown.ClientID);
          RDP1.Visible = true;
          break;
        case KONSOLIDASI_REPORT_STOKNASIONAL:
          StokNasional1.InitializePage(wndDown.ClientID);
          StokNasional1.Visible = true;
          break;
        case KONSOLIDASI_REPORT_BDPPHAROS:
          BDPPharos1.InitializePage(wndDown.ClientID);
          BDPPharos1.Visible = true;
          break;
      }
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }
}
