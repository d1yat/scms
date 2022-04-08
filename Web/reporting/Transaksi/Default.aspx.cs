using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Common;

public partial class reporting_Inventory_Default : Scms.Web.Core.PageHandler
{
  private const string TRANSAKSI_PO = "purchaseorder";
  private const string TRANSAKSI_COMBO = "combo";
  private const string TRANSAKSI_RN = "receivenote";
  private const string TRANSAKSI_RC = "returcustomer";
  private const string TRANSAKSI_RS = "retursupplier";
  private const string TRANSAKSI_PL = "packinglist";
  private const string TRANSAKSI_SJ = "suratjalan";
  private const string TRANSAKSI_POK = "pokhusus";
  private const string TRANSAKSI_SPPACKING = "sppackinglist";
  private const string TRANSAKSI_PENJUALAN = "penjualan";
  private const string TRANSAKSI_RETURNDO = "returndo"; //Indra 20170803
  private const string TRANSAKSI_SPEXP = "spekspedisi";
  private const string TRANSAKSI_ADJUSTSTOCK = "adjuststock";
  private const string TRANSAKSI_ADJUSTSP = "adjustsp";
  private const string TRANSAKSI_ADJUSTPO = "adjustpo";
  private const string TRANSAKSI_ADJUSTSTT = "adjuststt";
  private const string TRANSAKSI_ADJUSTCOMBO = "adjustcombo";
  private const string TRANSAKSI_ADJUSTFB = "adjustfb";
  private const string TRANSAKSI_ADJUSTFJ = "adjustfj";
  private const string TRANSAKSI_FLOATING = "floating";
  private const string TRANSAKSI_LAPORAN_PBF = "pbf";
  private const string TRANSAKSI_LAPORAN_PBF_new = "pbfnew";
  private const string TRANSAKSI_LAPORAN_PBF_BPOM = "pbfbpom"; //Indra 20170404
  private const string TRANSAKSI_OKTPREKURSORBULANAN = "oktprekursor";
  private const string TRANSAKSI_STT = "liststt";
  private const string TRANSAKSI_LISTSP = "listsp";
  private const string TRANSAKSI_LISTPEMBELIAN = "listpembelian";
  private const string TRANSAKSI_REPORT_ENAPZA = "enapza";
  private const string TRANSAKSI_REPORT_EREPORT_ALKES = "ealkes";
  private const string TRANSAKSI_REPORT_CSL = "csl";



  protected void Page_Init(object sender, EventArgs e)
  {
    string qryString = null;

    if ((!this.IsPostBack) && (!Ext.Net.X.IsAjaxRequest))
    {
      qryString = (this.Request.QueryString.IsContainKey("mode") ? (this.Request.QueryString["mode"] ?? string.Empty).Trim() : string.Empty).ToLower();

      switch (qryString)
      {
        case TRANSAKSI_PO:
          {
            PurchaseOrder.Visible = true;
            PurchaseOrder.InitializePage(wndDown.ClientID);
          }
          break;
        case TRANSAKSI_COMBO:
          {
              Combo.Visible = true;
              Combo.InitializePage(wndDown.ClientID);
          }
          break;
        case TRANSAKSI_RN:
          {
            ReceiveNote.Visible = true;
            ReceiveNote.InitializePage(wndDown.ClientID);
          }
          break;
        case TRANSAKSI_RC:
          {
            ReturCustomer.Visible = true;
            ReturCustomer.InitializePage(wndDown.ClientID);
          }
          break;
        case TRANSAKSI_RS:
          {
            ReturSupplier.Visible = true;
            ReturSupplier.InitializePage(wndDown.ClientID);
          }
          break;
        case TRANSAKSI_PL:
          {
            PackingList.Visible = true;
            PackingList.InitializePage(wndDown.ClientID);
          }
          break;
        case TRANSAKSI_SJ:
          {
            TransferGudang.Visible = true;
            TransferGudang.InitializePage(wndDown.ClientID);
          }
          break;
        case TRANSAKSI_POK:
          {
            POKhusuSP.Visible = true;
            POKhusuSP.InitializePage(wndDown.ClientID);
          }
          break;
        case TRANSAKSI_SPPACKING:
          {
            SPPackingList.InitializePage(wndDown.ClientID);
            SPPackingList.Visible = true;
          }
          break;
        case TRANSAKSI_PENJUALAN:
          {
            Penjualan.InitializePage(wndDown.ClientID);
            Penjualan.Visible = true;
          }
          break;
        //Indra 20170803 tambah return DO
        case TRANSAKSI_RETURNDO:
          {
            ReturnDO.InitializePage(wndDown.ClientID);
            ReturnDO.Visible = true;
          }
          break;
        case TRANSAKSI_SPEXP:
          {
            SPExpedisi.InitializePage(wndDown.ClientID);
            SPExpedisi.Visible = true;
          }
          break;
        case TRANSAKSI_ADJUSTSTOCK:
          {
            AdjustStock.Visible = true;
            AdjustStock.InitializePage(wndDown.ClientID);
          }
          break;
        case TRANSAKSI_ADJUSTSP:
          {
            AdjustSuratPesanan.Visible = true;
            AdjustSuratPesanan.InitializePage(wndDown.ClientID);
          }
          break;
        case TRANSAKSI_ADJUSTPO:
          {
            AdjustPurchaseOrder.Visible = true;
            AdjustPurchaseOrder.InitializePage(wndDown.ClientID);
          }
          break;
        case TRANSAKSI_ADJUSTSTT:
          {
            AdjustSTT.Visible = true;
            AdjustSTT.InitializePage(wndDown.ClientID);
          }
          break;
        case TRANSAKSI_ADJUSTCOMBO:
          {
            AdjustCombo.InitializePage(wndDown.ClientID);
            AdjustCombo.Visible = true;
          }
          break;
        case TRANSAKSI_ADJUSTFB:
          {
            AdjustFB.Visible = true;
            AdjustFB.InitializePage(wndDown.ClientID);
          }
          break;
        case TRANSAKSI_ADJUSTFJ:
          {
            AdjustFJ.Visible = true;
            AdjustFJ.InitializePage(wndDown.ClientID);
          }
          break;
        case TRANSAKSI_FLOATING:
          {
            Floating.Visible = true;
            Floating.InitializePage(wndDown.ClientID);
          }
          break;
        case TRANSAKSI_LAPORAN_PBF:
          {
            LaporanPBF.Visible = true;
            LaporanPBF.InitializePage(wndDown.ClientID,"reguler");
          }
          break;
        case TRANSAKSI_LAPORAN_PBF_new:
          {
              LaporanPBF.Visible = true;
              LaporanPBF.InitializePage(wndDown.ClientID,"NonReguler");
          }
          break;
        //Indra 20170404
        case TRANSAKSI_LAPORAN_PBF_BPOM:
          {
              LaporanPBF.Visible = true;
              LaporanPBF.InitializePage(wndDown.ClientID, "NonRegulerBPOM");
          }
          break;
        case TRANSAKSI_OKTPREKURSORBULANAN:
          {
              OktPrekursorBulanan.Visible = true;
              OktPrekursorBulanan.InitializePage(wndDown.ClientID);
          }
          break;
        case TRANSAKSI_STT:
          {
              STT.Visible = true;
              STT.InitializePage(wndDown.ClientID);
          }
          break;
        case TRANSAKSI_LISTSP:
          {
              ListSP.Visible = true;
              ListSP.InitializePage(wndDown.ClientID);
          }
          break;
        case TRANSAKSI_LISTPEMBELIAN:
          {
              Pembelian.Visible = true;
              Pembelian.InitializePage(wndDown.ClientID);
          }
          break;
        case TRANSAKSI_REPORT_ENAPZA:
          Enapza.InitializePage(wndDown.ClientID);
          Enapza.Visible = true;
          break;
        case TRANSAKSI_REPORT_EREPORT_ALKES:
          Ealkes.InitializePage(wndDown.ClientID);
          Ealkes.Visible = true;
          break;
        case TRANSAKSI_REPORT_CSL:
          CSL.InitializePage(wndDown.ClientID);
          CSL.Visible = true;
          break;
      }
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
  }
}
