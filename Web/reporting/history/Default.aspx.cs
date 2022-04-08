using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Common;

public partial class reporting_history_Default : Scms.Web.Core.PageHandler
{
  private const string HISTORY_REPORT_ASURANSI = "asuransi";
  private const string HISTORY_REPORT_POTRANSAKSI = "potransaksi";
  private const string HISTORY_REPORT_POLIMIT = "polimit";
  private const string HISTORY_REPORT_SPTRANSAKSI = "sptransaksi";
  private const string HISTORY_REPORT_SPBATAL = "spbatal";
  private const string HISTORY_REPORT_SPGUDANG = "spgudang";
  private const string HISTORY_REPORT_MEMOSTT = "memostt";
  private const string HISTORY_REPORT_MEMOCOMBO = "memocombo";
  private const string HISTORY_REPORT_ITEMBLOCK= "itemblock";
  private const string HISTORY_REPORT_ITEMBATCHGUDANG = "itembatchgudang";
  private const string HISTORY_REPORT_ITEMBATCHNASIONAL = "itembatchnasional";
  private const string HISTORY_REPORT_EXPEDISI = "expedisi";
  private const string HISTORY_REPORT_QUERY_PEMBELIAN = "querypembelian";
  private const string HISTORY_REPORT_QUERY_PENJUALAN = "querypenjualan";
  private const string HISTORY_REPORT_QUERY_SALDO = "querysaldo";
  private const string HISTORY_REPORT_QUERY_PURCHASE = "querypurchase";
  private const string HISTORY_REPORT_QUERY_SALES = "querysales";
  private const string HISTORY_REPORT_QUERY_CLAIM = "queryclaim";
  private const string HISTORY_REPORT_RESI_EKSPEDISI = "resiekspedisi";
  private const string HISTORY_REPORT_RESI_EKSPEDISI_DOSJ = "resiekspedisidosj";
  private const string HISTORY_REPORT_SHIPMENT = "shipment";
  private const string HISTORY_REPORT_BIAYA_EKSPEDISI = "biayaekspedisi";
  private const string HISTORY_REPORT_BIAYA_INVOICE_VS_RESI = "biayainvoicevsresi";
  private const string HISTORY_REPORT_REKAP_INVOICE = "rekapinvoice";
  private const string HISTORY_REPORT_LIST_PEMBAYARAN_EKSPEDISI = "listpembayaranep";
  private const string HISTORY_REPORT_PO_PENDING = "popending";
  private const string HISTORY_REPORT_RN_CABANG = "rncabang";
  private const string HISTORY_REPORT_PEMUSNAHAN = "pemusnahan";
  private const string REPORT_HISTORY_RETURSUPPLIER = "historyretursupplier";


  protected void Page_Init(object sender, EventArgs e)
  {
    string qryString = null;

    if ((!this.IsPostBack) && (!Ext.Net.X.IsAjaxRequest))
    {
      qryString = (this.Request.QueryString.IsContainKey("mode") ? (this.Request.QueryString["mode"] ?? string.Empty).Trim() : string.Empty).ToLower();

      switch (qryString)
      {
        case HISTORY_REPORT_ASURANSI:
          Asuransi1.InitializePage(wndDown.ClientID);
          Asuransi1.Visible = true;
          break;
        case HISTORY_REPORT_POTRANSAKSI:
          POTransaksi1.InitializePage(wndDown.ClientID);
          POTransaksi1.Visible = true;
          break;
        case HISTORY_REPORT_POLIMIT:
          POLimit1.InitializePage(wndDown.ClientID);
          POLimit1.Visible = true;
          break;
        case HISTORY_REPORT_SPTRANSAKSI:
          SuratPesanan1.InitializePage(wndDown.ClientID);
          SuratPesanan1.Visible = true;
          break;
        case HISTORY_REPORT_SPGUDANG:
          SuratPesananGudang1.InitializePage(wndDown.ClientID);
          SuratPesananGudang1.Visible = true;
          break;
        case HISTORY_REPORT_SPBATAL:
          SuratPesananBatal1.InitializePage(wndDown.ClientID);
          SuratPesananBatal1.Visible = true;
          break;
        case HISTORY_REPORT_MEMOSTT:
          SuratTandaTerima1.InitializePage(wndDown.ClientID);
          SuratTandaTerima1.Visible = true;
          break;
        case HISTORY_REPORT_ITEMBLOCK:
          BlockItem1.InitializePage(wndDown.ClientID);
          BlockItem1.Visible = true;
          break;
        case HISTORY_REPORT_MEMOCOMBO:
          Combo1.InitializePage(wndDown.ClientID);
          Combo1.Visible = true;
          break;
        case HISTORY_REPORT_ITEMBATCHGUDANG:
          ItemBatchGudang1.InitializePage(wndDown.ClientID);
          ItemBatchGudang1.Visible = true;
          break;
        case HISTORY_REPORT_ITEMBATCHNASIONAL:
          ItemBatchNasionall.InitializePage(wndDown.ClientID);
          ItemBatchNasionall.Visible = true;
          break;
        case HISTORY_REPORT_EXPEDISI:
          Expedisi1.InitializePage(wndDown.ClientID);
          Expedisi1.Visible = true;
          break;
        case HISTORY_REPORT_QUERY_PENJUALAN:
          QueryPenjualan1.InitializePage(wndDown.ClientID);
          QueryPenjualan1.Visible = true;
          break;
        case HISTORY_REPORT_QUERY_CLAIM:
          QueryClaim1.InitializePage(wndDown.ClientID);
          QueryClaim1.Visible = true;
          break;
        case HISTORY_REPORT_QUERY_PEMBELIAN:
          QueryPembelian1.InitializePage(wndDown.ClientID);
          QueryPembelian1.Visible = true;
          break;
        case HISTORY_REPORT_QUERY_SALES:
          QuerySales1.InitializePage(wndDown.ClientID);
          QuerySales1.Visible = true;
          break;
        case HISTORY_REPORT_QUERY_PURCHASE:
          QueryPurchase.InitializePage(wndDown.ClientID);
          QueryPurchase.Visible = true;
          break;
        case HISTORY_REPORT_QUERY_SALDO:
          QuerySaldo.InitializePage(wndDown.ClientID);
          QuerySaldo.Visible = true;
          break;
        case HISTORY_REPORT_RESI_EKSPEDISI:
          ResiEkspedisi.InitializePage(wndDown.ClientID);
          ResiEkspedisi.Visible = true;
          break;
        case HISTORY_REPORT_RESI_EKSPEDISI_DOSJ:
          ResiEkspedisiDOSJ.InitializePage(wndDown.ClientID);
          ResiEkspedisiDOSJ.Visible = true;
          break;
        case HISTORY_REPORT_SHIPMENT:
          Shipment.InitializePage(wndDown.ClientID);
          Shipment.Visible = true;
          break;
        case HISTORY_REPORT_BIAYA_EKSPEDISI:
          BiayaEkspedisi.InitializePage(wndDown.ClientID);
          BiayaEkspedisi.Visible = true;
          break;
        case HISTORY_REPORT_BIAYA_INVOICE_VS_RESI:
          BiayaInvoiceVsResi.InitializePage(wndDown.ClientID);
          BiayaInvoiceVsResi.Visible = true;
          break;
        case HISTORY_REPORT_REKAP_INVOICE:
          RekapInvoice.InitializePage(wndDown.ClientID);
          RekapInvoice.Visible = true;
          break;
        case HISTORY_REPORT_LIST_PEMBAYARAN_EKSPEDISI:
          ListPembayaranEP.InitializePage(wndDown.ClientID);
          ListPembayaranEP.Visible = true;
          break;
        case HISTORY_REPORT_PO_PENDING:
          POPending.InitializePage(wndDown.ClientID);
          POPending.Visible = true;
          break;
        case HISTORY_REPORT_RN_CABANG:
          RNCabang.InitializePage(wndDown.ClientID);
          RNCabang.Visible = true;
          break;
        case HISTORY_REPORT_PEMUSNAHAN:
          Pemusnahan.InitializePage(wndDown.ClientID);
          Pemusnahan.Visible = true;
          break;
        case REPORT_HISTORY_RETURSUPPLIER:
          ReturSupplier1.InitializePage(wndDown.ClientID);
          ReturSupplier1.Visible = true;
          break;
      }
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
  }
}
