using System;
using System.Text;
using ScmsSoaLibraryInterface.Components;
using ScmsSoaLibrary.Commons;

namespace ScmsSoaLibrary.Core.Reports
{
  public class ReportCaller
  {
    public struct ReportStructureGenerator
    {
      public bool isAsync;
      public string Configuration;
    }

    public static string Generate(bool async, string config)
    {
      string sResult = null;

      ReportParser rpt = ReportParser.Serialize(config);
      Config cfg = Functionals.Configuration;
      ReportingResult result = null;
      Functionals.ReportingGeneratorResult rptGenRes = default(Functionals.ReportingGeneratorResult);

      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {

          #region master Indra 20180830FM

          case Constant.REPORT_MS_PRINCIPAL:
          case Constant.REPORT_MS_PRINCIPAL_HISTORY_LEADTIME:
                
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Master.Generate(cfg, rpt, async);
            break;

          #endregion

          #region Financial

          case Constant.REPORT_AP_FAKTUR_PENDING_RINGKASAN:
          case Constant.REPORT_AP_FAKTUR_PENDING_DETILFAKTUR:
          case Constant.REPORT_AP_GL:
          case Constant.REPORT_AP_LIST_RINGKASAN:
          case Constant.REPORT_AP_LIST_DETILFAKTUR:
          case Constant.REPORT_AP_LIST_DETILTRANSAKSI:
          case Constant.REPORT_AP_LIST_BAYAR_TIPE_A:
          case Constant.REPORT_AP_LIST_BAYAR_TIPE_B:
          case Constant.REPORT_AR_FAKTUR_PENDING_RINGKASAN:
          case Constant.REPORT_AR_FAKTUR_PENDING_DETILFAKTUR:
          case Constant.REPORT_AR_GL:
          case Constant.REPORT_AR_LIST_RINGKASAN:
          case Constant.REPORT_AR_LIST_DETILFAKTUR:
          case Constant.REPORT_AR_LIST_DETILTRANSAKSI:
          case Constant.REPORT_AR_LIST_BAYAR_TIPE_A:
          case Constant.REPORT_AR_LIST_BAYAR_TIPE_B:
          case Constant.REPORT_AR_SALDO_DEBIT:
          case Constant.REPORT_HPP_DIV_AMS_DETIL_CLAIM:
          case Constant.REPORT_HPP_DIV_AMS_DETIL_NON_CLAIM:
          case Constant.REPORT_HPP_DIV_AMS_SHORT:
          case Constant.REPORT_HPP_DIV_AMS_SUMMARI_CLAIM:
          case Constant.REPORT_HPP_DIV_AMS_SUMMARI_NON_CLAIM:
          case Constant.REPORT_HPP_DIV_PRINS_DETIL_CLAIM:
          case Constant.REPORT_HPP_DIV_PRINS_DETIL_NON_CLAIM:
          case Constant.REPORT_HPP_DIV_PRINS_SUMMARI_CLAIM:
          case Constant.REPORT_HPP_DIV_PRINS_SUMMARI_NON_CLAIM:
          case Constant.REPORT_BEA_DETIL:
          case Constant.REPORT_BEA_FAKTUR:
          case Constant.REPORT_BEA_SUMMARI:
          case Constant.REPORT_PEMABAYARAN_SISA_FAKTUR:
          case Constant.REPORT_JATUH_TEMPO:
          case Constant.REPORT_CLAIM:
          case Constant.REPORT_CLAIM_ACC:
          case Constant.REPORT_EFAKTUR_FJ:
          case Constant.REPORT_FAKTUR_MANUAL:
          case Constant.REPORT_EFAKTUR_FM:

            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Financial.Generate(cfg, rpt, async);
            break;

          #endregion

          #region Inventory

          case Constant.REPORT_INVENTORY_STOK_GUDANG_BATCH:
          case Constant.REPORT_INVENTORY_STOK_GUDANG_TOTAL:
          case Constant.REPORT_INVENTORY_STOK_NASIONAL_BATCH:
          case Constant.REPORT_INVENTORY_STOK_NASIONAL_TOTAL:
          case Constant.REPORT_INVENTORY_STOK_KARTU_BARANG_GUDANG:
          case Constant.REPORT_INVENTORY_STOK_KARTU_BARANG_NASIONAL_BATCH:
          case Constant.REPORT_INVENTORY_STOK_KARTU_BARANG_NASIONAL_TOTAL:
          case Constant.REPORT_INVENTORY_STOK_OPNAME:
          case Constant.REPORT_INVENTORY_STOK_AKTUAL:
          case Constant.REPORT_INVENTORY_MONITORINGED:  // hafizh
          case Constant.REPORT_INVENTORY_REPORT_MONITOTINGEDCTRL:  // hafizh
          case Constant.REPORT_INVENTORY_INDEX_STOCK:
		  case Constant.REPORT_INVENTORY_UMUR_STOCK_DETIL:
          case Constant.REPORT_INVENTORY_UMUR_STOCK_DIV_PRINSIPAL:
          case Constant.REPORT_INVENTORY_UMUR_STOCK_ITEM:
          case Constant.REPORT_INVENTORY_UMUR_STOCK_SUPPLIER:
          case Constant.REPORT_INVENTORY_MUTASI_INV:
          case Constant.REPORT_INVENTORY_MUTASI_INV_DETAIL: //20170516 Indra D.
          case Constant.REPORT_INVENTORY_EXPIRE_BATCH:
          case Constant.REPORT_INVENTORY_CURRENTSTOCK_ED:
          case Constant.REPORT_MASTER_ITEM:
          //case Constant.REPORT_PROSES_STOCK_OPNAME: //Indra 20171105

            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Inventory.Generate(cfg, rpt, async);
            break;

          #endregion

          #region ListTransaksi

          case Constant.REPORT_LIST_TRANSAKSI_PO_DETIL:
          case Constant.REPORT_LIST_TRANSAKSI_PO_PERPO:
          case Constant.REPORT_LIST_TRANSAKSI_COMBO:
          case Constant.REPORT_LIST_TRANSAKSI_RN:
          case Constant.REPORT_LIST_TRANSAKSI_RC:
          case Constant.REPORT_LIST_TRANSAKSI_STT:
          case Constant.REPORT_LIST_TRANSAKSI_RS:
          case Constant.REPORT_LIST_TRANSAKSI_PL:
          case Constant.REPORT_LIST_TRANSAKSI_SJ:
          case Constant.REPORT_LIST_TRANSAKSI_POK:
          case Constant.REPORT_LIST_TRANSAKSI_SPPL_DETIL:
          case Constant.REPORT_LIST_TRANSAKSI_SPPL_PER_PER_DIVPRI:
          case Constant.REPORT_LIST_TRANSAKSI_SPPL_PERITEM:
          case Constant.REPORT_LIST_TRANSAKSI_PENJUALAN:
          case Constant.REPORT_LIST_TRANSAKSI_RETURNDO: //Indra 20170803
          case Constant.REPORT_LIST_TRANSAKSI_RETUR_PENJUALAN:
          case Constant.REPORT_LIST_TRANSAKSI_NET_PENJUALAN:
          case Constant.REPORT_LIST_TRANSAKSI_SPEXP:
          case Constant.REPORT_LIST_TRANSAKSI_ADJ_STOCK:
          case Constant.REPORT_LIST_TRANSAKSI_ADJ_SP:
          case Constant.REPORT_LIST_TRANSAKSI_ADJ_PO:
          case Constant.REPORT_LIST_TRANSAKSI_ADJ_STT:
          case Constant.REPORT_LIST_TRANSAKSI_ADJ_COMBO:
          case Constant.REPORT_LIST_TRANSAKSI_ADJ_FB:
          case Constant.REPORT_LIST_TRANSAKSI_ADJ_FJ:
          case Constant.REPORT_LIST_FLOATING:
          case Constant.REPORT_LIST_PBF:
          case Constant.REPORT_LIST_OKTPREKURSOR_BULANAN_DO:
          case Constant.REPORT_LIST_TRANSAKSI_SP:
          case Constant.REPORT_LIST_OKTPREKURSOR_BULANAN_PO:
          case Constant.REPORT_LIST_TRANSAKSI_PEMBELIAN:
          case Constant.REPORT_LIST_ENAPZA:
          case Constant.REPORT_LIST_EALKES:
          case Constant.REPORT_LIST_CSL:
          case Constant.REPORT_TRANSAKSI_PO_APOTEKER:

            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.ListTransaksi.Generate(cfg, rpt, async);

            break;

          #endregion
		  
		      #region History

          case Constant.REPORT_HISTORY_ASURANSI:
          case Constant.REPORT_HISTORY_PO_TRANSAKSI:
          case Constant.REPORT_HISTORY_PO_LIMIT_DETAIL:
          case Constant.REPORT_HISTORY_PO_LIMIT_TOTAL:
          case Constant.REPORT_HISTORY_SURATPESANAN:
          case Constant.REPORT_HISTORY_SURATPESANANGUDANG:
          case Constant.REPORT_HISTORY_SURATPESANANBATAL:
          case Constant.REPORT_HISTORY_STT:
          case Constant.REPORT_HISTORY_ITEMBLOCK:
          case Constant.REPORT_HISTORY_COMBO:
          case Constant.REPORT_HISTORY_ITEMBATCH_GUDANG:
          case Constant.REPORT_HISTORY_ITEMBATCH_NASIONAL:
          case Constant.REPORT_HISTORY_EXPEDISI:
          case Constant.REPORT_HISTORY_QUERY_CLAIM:
          case Constant.REPORT_HISTORY_QUERY_CLAIM_PER_SUPPLIER:
          case Constant.REPORT_HISTORY_QUERY_CLAIM_PER_DIV_AMS:
          case Constant.REPORT_HISTORY_QUERY_FB:
          case Constant.REPORT_HISTORY_QUERY_FBR:
          case Constant.REPORT_HISTORY_QUERY_FJ:
          case Constant.REPORT_HISTORY_QUERY_FJR:
          case Constant.REPORT_HISTORY_QUERY_SALES_RETUR_SUM:
          case Constant.REPORT_HISTORY_QUERY_SALES_RETUR_CABANG:
          case Constant.REPORT_HISTORY_QUERY_SALES_RETUR_FAKTUR:
          case Constant.REPORT_HISTORY_QUERY_PURCHASE_SUM:
          case Constant.REPORT_HISTORY_QUERY_PURCHASE_CABANG:
          case Constant.REPORT_HISTORY_QUERY_PURCHASE_FAKTUR:
          case Constant.REPORT_HISTORY_QUERY_RETUR_PURCHASE_SUM:
          case Constant.REPORT_HISTORY_QUERY_RETUR_PURCHASE_CABANG:
          case Constant.REPORT_HISTORY_QUERY_RETUR_PURCHASE_FAKTUR:
          case Constant.REPORT_HISTORY_QUERY_SALES_NONRETUR_SUM:
          case Constant.REPORT_HISTORY_QUERY_SALES_NONRETUR_CABANG:
          case Constant.REPORT_HISTORY_QUERY_SALES_NONRETUR_FAKTUR:
          case Constant.REPORT_HISTORY_QUERY_SALDO_DEBIT:
          case Constant.REPORT_HISTORY_QUERY_SALDO_DEBIT_SUM:
          case Constant.REPORT_HISTORY_QUERY_SALDO_KREDIT:
          case Constant.REPORT_HISTORY_QUERY_SALDO_KREDIT_SUM:
          case Constant.REPORT_HISTORY_RESI_EKSPEDISI:
          case Constant.REPORT_HISTORY_RESI_EKSPEDISI_DO:
          case Constant.REPORT_HISTORY_RESI_EKSPEDISI_SJ:
          case Constant.REPORT_HISTORY_SHIPMENT:
          case Constant.REPORT_HISTORY_BIAYA_EKSPEDISI:
          case Constant.REPORT_HISTORY_INVOICEVSRESI:
          case Constant.REPORT_HISTORY_REKAP_INVOICE:
          case Constant.REPORT_HISTORY_LIST_BAYAR_EKSPEDISI:
          case Constant.REPORT_HISTORY_PO_PENDING:
          case Constant.REPORT_HISTORY_RN_CABANG:
          case Constant.REPORT_HISTORY_PEMUSNAHAN:
          case Constant.REPORT_HISTORY_RETURSUPPLIER:
          case Constant.REPORT_HISTORY_SERVICELEVEL_PO:  // SERVICE LEVEL PO 22032018 HAFIZH
          case Constant.REPORT_HISTORY_SERVICELEVEL_PROSESCABANG:  // SERVICE LEVEL CABANG PROSES
          case Constant.REPORT_HISTORY_SERVICELEVEL_REPORT_CABANG:  // SERVICE LEVEL CABANG REPORT CABANG
          case Constant.REPORT_HISTORY_RECALL_PROSESCABANG:
          case Constant.REPORT_HISTORY_RECALL_REPORTGENERATE:              
                            
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.History.Generate(cfg, rpt, async); 
            break;

          #endregion

          #region Monitoring

          case Constant.REPORT_MONITORING_DATA_BOOKED:
          case Constant.REPORT_MONITORING_DATA_CONF:
          case Constant.REPORT_MONITORING_DATA_PL:
          case Constant.REPORT_MONITORING_SEND_DO:
          case Constant.REPORT_MONITORING_SEND_RC:
          case Constant.REPORT_MONITORING_SALES_NASIONAL:
           
          case Constant.REPORT_PRODUKTIFITAS_DC:


            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Monitoring.Generate(cfg, rpt, async);
            break;

          #endregion

          #region Konsolidasi

          case Constant.REPORT_KONSOLIDASI_BDP:
          case Constant.REPORT_KONSOLIDASI_RDP:
          case Constant.REPORT_KONSOLIDASI_STOKNASIONAL:
          case Constant.REPORT_KONSOLIDASI_STOKNASIONAL_NONDIVPRI:
          case Constant.REPORT_KONSOLIDASI_BDP_PHAROS:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Konsolidasi.Generate(cfg, rpt, async); 
            break;

          #endregion

          #region Waktu Pelayanan

          case Constant.REPORT_WAKTUPELAYANAN_CABANG_DETAIL:
          case Constant.REPORT_WAKTUPELAYANAN_CABANG_SUM_DIVPRIN:
          case Constant.REPORT_WAKTUPELAYANAN_CABANG_SUM_CABANG:
          case Constant.REPORT_WAKTUPELAYANAN_PRINCIPAL_DETAIL:
          case Constant.REPORT_WAKTUPELAYANAN_PRINCIPAL_SUM_DIVPRIN:
          case Constant.REPORT_WAKTUPELAYANAN_PRINCIPAL_SUM_CABANG:
          case Constant.REPORT_WAKTUPELAYANAN_PRINCIPAL_CABANG:
          case Constant.REPORT_WAKTUPELAYANAN_SERAH_TERIMA:
          case Constant.REPORT_WAKTUPELAYANAN_ANALISA_SPPLCONFIRM_DETAIL:
          case Constant.REPORT_WAKTUPELAYANAN_ANALISA_SPPLCONFIRM_SUM:
          case Constant.REPORT_WAKTUPELAYANAN_SERAH_TERIMA_GUDANG_RETUR:
          case Constant.REPORT_PROCESS_MONITORING: //Indra Process Monitoring 20180523FM

            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.WaktuPelayanan.Generate(cfg, rpt, async);
            break;

          #endregion

          #region Transaksi

          case Constant.REPORT_TRANSAKSI_CLAIM_BONUS:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateClaimBonus(cfg, rpt, async);
            break;

          case Constant.REPORT_TRANSAKSI_PACKINGLIST:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GeneratePackingList(cfg, rpt, async);
            break;

          case Constant.REPORT_TRANSAKSI_PACKINGLIST_AUTO:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GeneratePackingListAuto(cfg, rpt, async);
            break;

         case Constant.REPORT_TRANSAKSI_DOPL:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateDOPL(cfg, rpt, async);
            break;

          case Constant.REPORT_TRANSAKSI_DOPL_PRINT_HO2:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateDOPLHO2(cfg, rpt, async);
            break;

          case Constant.REPORT_TRANSAKSI_DOSTT:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateDOSTT(cfg, rpt, async);
            break;

          case Constant.REPORT_TRANSAKSI_PO:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GeneratePO(cfg, rpt, async);
            break;

          case Constant.REPORT_TRANSAKSI_RSBELI:
          case Constant.REPORT_TRANSAKSI_RSBELI_CONF:
          case Constant.REPORT_TRANSAKSI_RSREPACK:
          case Constant.REPORT_TRANSAKSI_RSBELI_UPLOAD:
          case Constant.REPORT_TRANSAKSI_RSREPACK_UPLOAD:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateRS(cfg, rpt, async);
            break;

          case Constant.REPORT_TRANSAKSI_COMBO:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateCombo(cfg, rpt, async);
            break;

          case Constant.REPORT_TRANSAKSI_STT_DONASI:
          case Constant.REPORT_TRANSAKSI_STT_SAMPLE:

            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateSTT(cfg, rpt, async);
            break;

          case Constant.REPORT_TRANSAKSI_SURATJALAN_PESANANGUDANG:
          case Constant.REPORT_TRANSAKSI_SURATJALAN_TRANSFERGUDANG:
          case Constant.REPORT_TRANSAKSI_SURATJALAN_CLAIM:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateSuratJalan(cfg, rpt, async);
            break;

          case Constant.REPORT_TRANSAKSI_FAKTURJUAL:
          case Constant.REPORT_TRANSAKSI_FAKTURJUALRETUR:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateFakturJual_Retur(cfg, rpt, async);
            break;

          case Constant.REPORT_TRANSAKSI_ADJUSTMENTSTT:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateAdjustSTT(cfg, rpt, async);
            break;

          case Constant.REPORT_TRANSAKSI_SURATPESANAN:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateSuratPesanan(cfg, rpt, async);
            break;

          // hafizh pharmanet
          case Constant.REPORT_TRANSAKSI_PROSESPHARMANET:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateSuratPesanan(cfg, rpt, async);
            break;

          case Constant.PROSES_WAKTUPELAYANAN:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateProsesWaktuPelayanan(cfg, rpt, async);
            break;

            // Hafizh
          case Constant.PROSES_FJR:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateProsesWaktuPelayanan(cfg, rpt, async);
            break;

          // Hafizh
          case Constant.PROSES_DISC_CLAIM:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateProsesWaktuPelayanan(cfg, rpt, async);
            break;

          case Constant.PROSES_COMPARE_SO:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateProsesCompareSo(cfg, rpt, async);
            break;

          case Constant.PROSES_GENERATE_SO:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateProsesSo(cfg, rpt, async);
            break;

          case Constant.REPORT_TRANSAKSI_EKSPEDISI_SHIPMENT:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateEkspedisiShipment(cfg, rpt, async);
            break;

          case Constant.REPORT_TRANSAKSI_INVOICE_EKSPEDISI_EKSTERNAL:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateInvoiceEkspedisi(cfg, rpt, async);
            break;

          case Constant.REPORT_TRANSAKSI_INVOICE_EKSPEDISI_INTERNAL:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateInvoiceEkspedisi(cfg, rpt, async);
            break;

 	      case Constant.REPORT_TRANSAKSI_FAKTUR_EKSPEDISI:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateFakturEkspedisi(cfg, rpt, async);
            break;

          case Constant.REPORT_TRANSAKSI_PEMUSNAHAN:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GeneratePemusnahan(cfg, rpt, async);
            break;

          case Constant.REPORT_MEMO_PEMUSNAHAN:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GeneratePemusnahan(cfg, rpt, async);
            break;

          case Constant.REPORT_MEMO_BASPB_SJ:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateBASPBSJ(cfg, rpt, async);
            break;

          case Constant.REPORT_WAKTUPELAYANAN_SERAHTERIMA_TIKET:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateSerahTerimaTiket(cfg, rpt, async);
            break;

          case Constant.REPORT_PROSES_SERAHTERIMA:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateSerahTerimaPBBR(cfg, rpt, async);
            break;

          case Constant.REPORT_TRANSAKSI_MOVEMENT_STOCK:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateMovementStock(cfg, rpt, async);
            break;

          case Constant.REPORT_SERAH_TERIMA_RETUR:
            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transaksi.GenerateSerahTerimaRetur(cfg, rpt, async);
            break;
          #endregion

          #region Transfer

          case Constant.REPORT_TRANSAKSI_TransferGudang:

            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Transfer.GenerateTransferGudang(cfg, rpt, async);
            break;

          #endregion

          #region Pending

          case Constant.REPORT_PENDING_SURATPESANAN_TOTAL:
          case Constant.REPORT_PENDING_SURATPESANAN_DETAIL_PERITEM:
          case Constant.REPORT_PENDING_SURATPESANAN_DETAIL_PERSP:
          case Constant.REPORT_PENDING_SURATPESANAN_DETAIL_PERCUSTOMER:
          case Constant.REPORT_PENDING_SURATPESANAN_GUDANG:
          case Constant.REPORT_PENDING_SURATPESANAN_BULANAN:
          case Constant.REPORT_PENDING_SURATPESANAN_BULANAN_PO:
          case Constant.REPORT_PENDING_PURCHASEORDER_LOGISTIK_TYPE_1:
          case Constant.REPORT_PENDING_PURCHASEORDER_LOGISTIK_TYPE_2:
          case Constant.REPORT_PENDING_PURCHASEORDER_FINANCE_DETAIL:
          case Constant.REPORT_PENDING_PURCHASEORDER_FINANCE_TOTAL:
          case Constant.REPORT_PENDING_PURCHASEORDER_VS_SURATPESANAN:
          case Constant.REPORT_PENDING_DELIVERYORDER:
          case Constant.REPORT_PENDING_RETURCUSTOMER:
          case Constant.REPORT_PENDING_RECEIVENOTE:
          case Constant.REPORT_PENDING_RETURSUPLIER_SUMMARY:
          case Constant.REPORT_PENDING_RETURSUPLIER_DETAIL:
          case Constant.REPORT_PENDING_SURATTANDATERIMA:
          case Constant.REPORT_PENDING_PACKINGLIST:
          case Constant.REPORT_PENDING_COMBO:
          case Constant.REPORT_PENDING_SURATJALAN:
          case Constant.REPORT_PENDING_POPERIODIK:
          case Constant.REPORT_PENDING_DOBELUMRN: //Indra D. 20170312

            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Pending.Generate(cfg, rpt, async);
            break;

          #endregion

          #region StockOpname Indra 20171231FM

          case Constant.REPORT_PROSES_PRINT_FORM_SO:

            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Proses.Generate(cfg, rpt, async);

            break;
          
          case Constant.REPORT_PROSES_PRINT_MONITORING_SO:

            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Proses.Generate(cfg, rpt, async);

            break;

          #endregion

          #region Produk Kosong

          case Constant.REPORT_PROSESEMAIL_PRODUKKOSONG:

            rptGenRes = ScmsSoaLibrary.Core.Reports.Module.Proses.Generate(cfg, rpt, async);

            break;

          #endregion

          #region Default Data

          default:
            {

              rptGenRes = new Functionals.ReportingGeneratorResult()
              {
                IsSet = true,
                IsSuccess = false,
                Messages = "Invalid report id.",
                Extension = null,
                OutputFile = null,
                OutputPath = null,
                Size = null
              };
            }
            break;

          #endregion
        }
      }

      #region Deserialize Result

      result = new ReportingResult()
      {
        IsSuccess = rptGenRes.IsSuccess,
        Extension = rptGenRes.Extension,
        OutputFile = rptGenRes.OutputFile,
        MessageResponse = (rptGenRes.IsSuccess ? string.Empty : rptGenRes.Messages)
      };

      sResult = ReportingResult.Deserialize(result);

      #endregion

      return sResult;
    }

    public static string GenerateAsync(string config)
    {
      bool isProcessing = false;

      string sResult = null;

      try
      {
        isProcessing = System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(GenerateProcessingAsync),
        new ReportStructureGenerator()
        {
          Configuration = config,
          isAsync = true
        });
      }
      catch (Exception ex)
      {
        sResult = ex.Message;
      }

      #region Deserialize Result

      ReportingResult result = new ReportingResult()
      {
        IsSuccess = isProcessing,
        Extension = string.Empty,
        OutputFile = string.Empty,
        MessageResponse = (isProcessing ? "Queued" : sResult)
      };

      sResult = ReportingResult.Deserialize(result);

      #endregion

      return sResult;
    }

    private static void GenerateProcessingAsync(object state)
    {
      ReportStructureGenerator rsg = (ReportStructureGenerator)state;

      Generate(rsg.isAsync, rsg.Configuration);
    }
  }
}
