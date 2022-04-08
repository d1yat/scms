using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Common;

public partial class reporting_pending_Default : Scms.Web.Core.PageHandler
{
  private const string PENDING_REPORT_SPTRANSAKSI = "sptransaksi";
  private const string PENDING_REPORT_SPGUDANG = "spgudang";
  private const string PENDING_REPORT_SURATPESANAN_PO = "sptransaksipo";
  private const string PENDING_REPORT_POTRANSAKSI_LOGISTIK = "potransaksilog";
  private const string PENDING_REPORT_POTRANSAKSI_FINANCE = "potransaksifa";
  private const string PENDING_REPORT_DELIVERYORDER = "dotransaksi";
  private const string PENDING_REPORT_RETURCUSTOMER = "rctransaksi";
  private const string PENDING_REPORT_RECEIVENOTE = "rntransaksi";
  private const string PENDING_REPORT_RETURSUPLIER = "rstransaksi";
  private const string PENDING_REPORT_STT = "memostttransaksi";
  private const string PENDING_REPORT_PACKINGLIST = "pltransaksi";
  private const string PENDING_REPORT_COMBO = "combotransaksi";
  private const string PENDING_REPORT_SURATJALAN = "sjtransaksi";
  private const string PENDING_REPORT_POPERIODIK = "poperiodik";
  private const string PENDING_REPORT_DOBELUMRN = "dobelumrn"; //Indra D. 20170312

  protected void Page_Init(object sender, EventArgs e)
  {
    string qryString = null;

    if ((!this.IsPostBack) && (!Ext.Net.X.IsAjaxRequest))
    {
      qryString = (this.Request.QueryString.IsContainKey("mode") ? (this.Request.QueryString["mode"] ?? string.Empty).Trim() : string.Empty).ToLower();

      switch (qryString)
      {
        case PENDING_REPORT_SPTRANSAKSI:
          SuratPesanan1.InitializePage(wndDown.ClientID);
          SuratPesanan1.Visible = true;
          break;
        case PENDING_REPORT_SPGUDANG:
          SuratPesananGudang1.InitializePage(wndDown.ClientID);
          SuratPesananGudang1.Visible = true;
          break;
        case PENDING_REPORT_SURATPESANAN_PO:
          SuratPesananPO1.InitializePage(wndDown.ClientID);
          SuratPesananPO1.Visible = true;
          break;
        case PENDING_REPORT_POTRANSAKSI_LOGISTIK:
          PurchaseOrderLogistik1.InitializePage(wndDown.ClientID);
          PurchaseOrderLogistik1.Visible = true;
          break;
        case PENDING_REPORT_POTRANSAKSI_FINANCE:
          PurchaseOrderFinance1.InitializePage(wndDown.ClientID);
          PurchaseOrderFinance1.Visible = true;
          break;
        case PENDING_REPORT_DELIVERYORDER:
          DeliveryOrder1.InitializePage(wndDown.ClientID);
          DeliveryOrder1.Visible = true;
          break;
        case PENDING_REPORT_RETURCUSTOMER:
          ReturCustomer1.InitializePage(wndDown.ClientID);
          ReturCustomer1.Visible = true;
          break;
        case PENDING_REPORT_RECEIVENOTE:
          ReceiveNote1.InitializePage(wndDown.ClientID);
          ReceiveNote1.Visible = true;
          break;
        case PENDING_REPORT_RETURSUPLIER:
          ReturSuplier1.InitializePage(wndDown.ClientID);
          ReturSuplier1.Visible = true;
          break;
        case PENDING_REPORT_STT:
          SuratTandaTerima1.InitializePage(wndDown.ClientID);
          SuratTandaTerima1.Visible = true;
          break;
        case PENDING_REPORT_PACKINGLIST:
          PackingList1.InitializePage(wndDown.ClientID);
          PackingList1.Visible = true;
          break;
        case PENDING_REPORT_COMBO:
          Combo1.InitializePage(wndDown.ClientID);
          Combo1.Visible = true;
          break;
        case PENDING_REPORT_SURATJALAN:
          SuratJalan1.InitializePage(wndDown.ClientID);
          SuratJalan1.Visible = true;
          break;
        case PENDING_REPORT_POPERIODIK:
          PendingPOPeriodik.InitializePage(wndDown.ClientID);
          PendingPOPeriodik.Visible = true;
          break;
        //Indra D. 20170312
        case PENDING_REPORT_DOBELUMRN:
          PendingDOBelumRN. InitializePage(wndDown.ClientID);
          PendingDOBelumRN.Visible = true;
          break;
      }
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
  }
}
