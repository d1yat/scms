using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Common;

public partial class reporting_Inventory_Default : Scms.Web.Core.PageHandler
{
  private const string RPT_STOK_GUDANG = "stockgudang";
  private const string RPT_STOK_NASIONAL = "stocknasional";
  private const string RPT_KARTU_GUDANG = "kartubaranggudang";
  private const string RPT_KARTU_NASIONAL = "kartubarangnasional";
  private const string RPT_STOKOPNAME = "stockopname";
  private const string RPT_CURRENTSTOK = "currentstock";
  private const string RPT_INDEX_STK = "indekstock";
  private const string RPT_UMUR_STK = "umurstock";
  private const string RPT_MUTASI_INV = "mutasiinventori";
  private const string RPT_EXPIRE_BATCH = "expirebatch";
  private const string RPT_CURRENTSTOCK_ED = "currentstocked";
  private const string RPT_STOKINTEGRITY = "stockintegrity";
  private const string RPT_MONITORINGED = "monitoringexpired";  // hafizh
  private const string RPT_MONITORINGEDCTRL = "monitoringexpiredctrl";  // hafizh
  private const string RPT_SPPLDO = "sppldo";


  protected void Page_Init(object sender, EventArgs e)
  {
    string qryString = null;

    if ((!this.IsPostBack) && (!Ext.Net.X.IsAjaxRequest))
    {
      qryString = (this.Request.QueryString.IsContainKey("mode") ? (this.Request.QueryString["mode"] ?? string.Empty).Trim() : string.Empty).ToLower();

      switch (qryString)
      {
        case RPT_STOK_GUDANG:
          {
            StokGudang.InitializePage(wndDown.ClientID);
            StokGudang.Visible = true;
          }
          break;
        case RPT_STOK_NASIONAL:
          {
            StockNasional.InitializePage(wndDown.ClientID);
            StockNasional.Visible = true;
          }
          break;
        case RPT_KARTU_GUDANG:
          {
            KartuBarangGudang.InitializePage(wndDown.ClientID);
            KartuBarangGudang.Visible = true;
          }
          break;
        case RPT_KARTU_NASIONAL:
          {
            KartuBarangNasional.InitializePage(wndDown.ClientID);
            KartuBarangNasional.Visible = true;
          }
          break;
        case RPT_STOKOPNAME:
          {
            StockOpname.InitializePage(wndDown.ClientID);
            StockOpname.Visible = true;
          }
          break;
        case RPT_STOKINTEGRITY:
          {
              StockIntegrity.InitializePage(wndDown.ClientID);
              StockIntegrity.Visible = true;
          }
          break;
        case RPT_CURRENTSTOK:
          {
            CurrentStock.InitializePage(wndDown.ClientID);
            CurrentStock.Visible = true;
          }
          break;
        case RPT_INDEX_STK:
          {
            IndekStock.InitializePage(wndDown.ClientID);
            IndekStock.Visible = true;
          }
          break;
        case RPT_UMUR_STK:
          {
            UmurStock.InitializePage(wndDown.ClientID);
            UmurStock.Visible = true;
          }
          break;
        case RPT_MUTASI_INV:
          {
            MutasiInventori.InitializePage(wndDown.ClientID);
            MutasiInventori.Visible = true;
          }
          break;
        case RPT_EXPIRE_BATCH:
          {
              ExpireBatch.InitializePage(wndDown.ClientID);
              ExpireBatch.Visible = true;
          }
          break;
        case RPT_CURRENTSTOCK_ED:
          {
              CurrentStockED.InitializePage(wndDown.ClientID);
              CurrentStockED.Visible = true;
          }
          break;


        // hafizh

        case RPT_MONITORINGED:
          {
              MonitoringExpired.InitializePage(wndDown.ClientID);
              MonitoringExpired.Visible = true;
          }
          break;
        case RPT_MONITORINGEDCTRL:
          {
              MonitoringExpiredCtrl.InitializePage(wndDown.ClientID);
              MonitoringExpiredCtrl.Visible = true;
          }
          break;

        case RPT_SPPLDO:
          {
              SPPLDO.InitializePage(wndDown.ClientID);
              SPPLDO.Visible = true;
          }
          break;

      }
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
  }
}