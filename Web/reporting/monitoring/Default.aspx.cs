using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Common;

public partial class reporting_monitoring_Default : Scms.Web.Core.PageHandler
{
  private const string MONITORING_DATA_PL = "pl";
  private const string MONITORING_DATA_PL_CONF = "plconf";
  private const string MONITORING_DATA_PL_BOOKED = "plbooked";
  private const string MONITORING_SEND_DO = "senddo";
  private const string MONITORING_SEND_RC = "sendrc";
  private const string MONITORING_SALES_NASIONAL = "salesnasional";
  private const string MONITORING_PENDING_INTEGRITY = "pendingintegrity";
  private const string MONITORING_PRODUKTIFITAS_DC = "produktifitasdc";   

  protected void Page_Load(object sender, EventArgs e)
  {
    string qryString = null;

    if ((!this.IsPostBack) && (!Ext.Net.X.IsAjaxRequest))
    {
      qryString = (this.Request.QueryString.IsContainKey("mode") ? (this.Request.QueryString["mode"] ?? string.Empty).Trim() : string.Empty).ToLower();

      switch (qryString)
      {
        case MONITORING_DATA_PL:
          {
            PackingList.Visible = true;
            PackingList.InitializePage(wndDown.ClientID);
          }
          break;

        case MONITORING_DATA_PL_BOOKED:
          {
            PackingListBooked.Visible = true;
            PackingListBooked.InitializePage(wndDown.ClientID);
          }
          break;

        case MONITORING_DATA_PL_CONF:
          {
            PackingListConf.Visible = true;
            PackingListConf.InitializePage(wndDown.ClientID);
          }
          break;

        case MONITORING_SEND_DO:
          {
            SendDeliveryOrder.Visible = true;
            SendDeliveryOrder.InitializePage(wndDown.ClientID);
          }
          break;

        case MONITORING_SEND_RC:
          {
              SendReturCustomer.Visible = true;
              SendReturCustomer.InitializePage(wndDown.ClientID);
          }
          break;

        case MONITORING_SALES_NASIONAL:
          {
              SalesNasional.Visible = true;
              SalesNasional.InitializePage(wndDown.ClientID);
          }
          break;
        case MONITORING_PENDING_INTEGRITY:
          {
              StockIntegrityPending.Visible = true;
              StockIntegrityPending.Initialize(wndDown.ClientID, this.ActiveGudang);
          }
          break;
        case MONITORING_PRODUKTIFITAS_DC:
          {
              ProduktifitasDC.Visible = true;
              ProduktifitasDC.Initialize(wndDown.ClientID, this.ActiveGudang);
          }
          break;
      }
    }
  }
}
