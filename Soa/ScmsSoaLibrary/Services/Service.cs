using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibrary.Modules;
using ScmsSoaLibrary.Core.Converter;
using Ext.Net;
using System.Collections;
using Newtonsoft.Json;
using System.ServiceModel.Activation;
using System.Xml.Linq;
using System.Diagnostics;

namespace ScmsSoaLibrary.Services
{
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
  [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
    InstanceContextMode = InstanceContextMode.PerCall)]
  // NOTE: If you change the class name "Service" here, you must also update the reference to "Service" in App.config.
  public class Service : ScmsSoaLibraryInterface.IScmsSoaLibrary
  {
    #region Private Coded

    private string QueryLogic(int start, int limit, bool allQuery, string sort, string dir, string model, string[][] parameters, bool jsonp)
    {
      return QueryLogic(start, limit, allQuery, sort, dir, model, parameters, jsonp, null);
    }

    private string QueryLogic(int start, int limit, bool allQuery, string sort, string dir, string model, string[][] parameters, bool jsonp, JsonConverter dateTimeConverter)
    {
      
      if (string.IsNullOrEmpty(model))
      {
        return null;
      }

      string result = null;

      IDictionary<string, Functionals.ParameterParser> dicParam = Functionals.ParserArrayParameter(parameters);

      model = model.ToLower();

      Config config = Functionals.Configuration;
      string connStr = Functionals.ActiveConnectionString;

      IDictionary<string, object> dic = null;

      #region Case

      switch (model)
      {
        #region CommonQuery.ModelGridQuery

        case Constant.MODEL_COMMON_QUERY_SINGLE_MSTRANSD:
        case Constant.MODEL_COMMON_QUERY_SINGLE_CUSTOMER:
        case Constant.MODEL_COMMON_QUERY_SINGLE_CUSTOMER_AND_GUDANG:
        case Constant.MODEL_COMMON_QUERY_SINGLE_CUSTOMERDCORE: //Indra Monitoring Process 20180523FM
        case Constant.MODEL_COMMON_QUERY_SINGLE_SUPLIER:
        case Constant.MODEL_COMMON_QUERY_SINGLE_SUPLIER_AUTO:
        case Constant.MODEL_COMMON_QUERY_SINGLE_GUDANG:
        case Constant.MODEL_COMMON_QUERY_SINGLE_GUDANG_FULL:
        case Constant.MODEL_COMMON_QUERY_SINGLE_GUDANG_CS:
        case Constant.MODEL_COMMON_QUERY_SINGLE_MSDIVAMS:
        case Constant.MODEL_COMMON_QUERY_SINGLE_MSDIVPRI:
        case Constant.MODEL_COMMON_QUERY_SINGLE_ITEM:
        case Constant.MODEL_COMMON_QUERY_SINGLE_ITEM_NONPSIKOTROPIKA:
        case Constant.MODEL_COMMON_QUERY_SINGLE_GROUP:
        case Constant.MODEL_COMMON_QUERY_SINGLE_REPORT:
        case Constant.MODEL_COMMON_QUERY_SINGLE_KURS:
        case Constant.MODEL_COMMON_QUERY_SINGLE_KURS_NONSYMBOL:
        case Constant.MODEL_COMMON_QUERY_SINGLE_EKSPEDISI:
        case Constant.MODEL_COMMON_QUERY_SINGLE_BANK:
        case Constant.MODEL_COMMON_QUERY_SINGLE_REKENING:
        case Constant.MODEL_COMMON_QUERY_SINGLE_BATCH:
        case Constant.MODEL_COMMON_QUERY_SINGLE_KARYAWAN:
        case Constant.MODEL_COMMON_QUERY_SINGLE_KOTA:
        case Constant.MODEL_COMMON_QUERY_SINGLE_FULLITEM:
        case Constant.MODEL_COMMON_QUERY_SINGLE_USERWP:
        case Constant.MODEL_COMMON_QUERY_SINGLE_WP_PICKER_DETAIL:
        case Constant.MODEL_COMMON_QUERY_SINGLE_WP_PACKER_DETAIL:
        case Constant.MODEL_COMMON_QUERY_SINGLE_USER_ST:
        case Constant.MODEL_COMMON_QUERY_SINGLE_OUTLET:
        case Constant.MODEL_COMMON_QUERY_SINGLE_REASON_RETUR:
        case Constant.MODEL_COMMON_QUERY_SINGLE_MSBATCH: //Indra 20181019FM
        case Constant.MODEL_COMMON_QUERY_SINGLE_CABANG:
        case Constant.MODEL_COMMON_QUERY_SEARCH_CABANG:
        case Constant.MODEL_COMMON_QUERY_SINGLE_TRANS_RECALL:
          case Constant.MODEL_COMMON_QUERY_SINGLE_RECEIVE_NOTE:
          case Constant.MODEL_COMMON_QUERY_SINGLE_HARD_TAX:

          dic = CommonQuery.ModelGridQuery(connStr, start, limit, allQuery, sort, dir, model.ToLower(), dicParam);

          break;

        #endregion

        #region CommonQueryMulti.ModelGridQuery

        //case Constant.MODEL_COMMON_QUERY_MULTIPLE_SUPLIER_PL:
        //case Constant.MODEL_COMMON_QUERY_MULTIPLE_CUSTOMER_PL:
        //case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_PL:
        //case Constant.MODEL_COMMON_QUERY_MULTIPLE_SPDETAIL_PL:
        //case Constant.MODEL_COMMON_QUERY_MULTIPLE_BATCHDETAIL_PL:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_SP:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_OR:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_OR_SP:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_ORG_SPG:
        case Constant.MODEL_PROCESS_QUERY_MULTIPLE_DETIL_INFO_ORP:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_SUPLIER_SJ:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_SUPLIER_SJ_GUDANG_3:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_SJ:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_SGDETAIL_SJ:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_SUPPLIER_SJ:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_BATCHDETAIL_SJ:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_SJ_AUTOGEN:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_SJ_REPACK:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_TF_PEMUSNAHAN:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_DOPL_NOPL:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_DOSTT_NOSTT_SAMPLE:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_GUDANGCABANG:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_CUSTOMER_EKSPEDISI:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_MASTER_EKSPEDISI:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_SUPPLIER_EKSPEDISI:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_DO_EKSPEDISI_SJ:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_DO_EKSPEDISI:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_DRIVER_EKSPEDISI:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_WP_EKSPEDISI:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RESI_EKSPEDISI:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_FAKTUR_EKSPEDISI:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_NO_EKSPEDISI:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_LIST_EP_EKSTERNAL:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_LIST_EP_INTERNAL:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_LIST_DO_RETURN:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RS_EKSPEDISI:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_STT_MEMO:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_STT_ITEM:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_STT_BATCH:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_PM_ITEM:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_PM_BATCH:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_BELI_ITEM:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_BELI_PO:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_BELI_PO_ULUJAMI:  // ulujami
        case Constant.MODEL_COMMON_QUERY_OUTSTANDPO_RN:
        case Constant.MODEL_COMMO_QUERY_PO_SP:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_KHUSUS_PRINCIPAL:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_KHUSUS_DO:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_KHUSUS_ITEM:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_KHUSUS_PO:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_KHUSUS_PO_ULUJAMI:  // ulujami

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_ClAIM_ITEM:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_ClAIM_NOCLAIM:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_ClAIM_ITEM_ULUJAMI:  // Ulujami Calim
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_ClAIM_NOCLAIM_ULUJAMI:  // Ulujami Calim
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_RETUR_ITEM:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_RETUR_RS:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_RETUR_BATCH:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_REPACK_ITEM:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_REPACK_RS:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_REPACK_BATCH:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_TRANSFER_SURATJALAN:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_LIST_SJ_BASPB:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_LIST_ITEM_SJ_BASPB:

        case Constant.MODEL_PROCESS_QUERY_MULTIPLE_ITEM_MKMEMOCOMBO:

        case Constant.MODEL_PROCESS_QUERY_MULTIPLE_MEMO_MEMO_MEMOCOMBO:
        case Constant.MODEL_PROCESS_QUERY_MULTIPLE_MEMO_ITEM_MEMOCOMBO:
        case Constant.MODEL_PROCESS_QUERY_MULTIPLE_MEMO_COMBO_ITEM_MEMOCOMBO:
        case Constant.MODEL_PROCESS_QUERY_MULTIPLE_MEMO_COMBO_BATCH_MEMOCOMBO:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_MEMO_PEMUSNAHAN:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RC_BATCH:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RC_ITEM:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RC_DO:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RC_RN:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RS_PEMBELIAN_ITEM:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RS_PEMBELIAN_BATCH:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_CLAIM_ITEM:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_CLAIM_MSDIVPRI:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_CLAIMACC_NOCLAIM:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_CLAIMACC_ITEM:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_CLAIMACC_CURRENT_ITEM:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_CLAIMACC_CURRENT_ITEM_DETIL:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ADJ_GOODBAD_BATCH:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ADJ_BATCH_ALL:
        case Constant.TEST_DATA:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_BATCHDETAIL_SJ_REPACK:
        case Constant.MODEL_COMMON_QUERY_SURAT_JALAN_AUTO:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_BATCHDETAIL_BAD:

        //case Constant.MODEL_COMMON_QUERY_MULTIPLE_SPDETAIL_PL_AUTO:
        //case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_PL_AUTO:
        //case Constant.MODEL_COMMON_QUERY_MULTIPLE_BATCH_PL_AUTO:
        //case Constant.MODEL_COMMON_QUERY_MULTIPLE_RNKHUSUS_PL_AUTO:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RS_CONF_PEMBELIAN_NORS:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RS_CONF_PEMBELIAN_NORS_HDR:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RS_CONF_PEMBELIAN_ITEM_RS:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RS_CONF_PEMBELIAN_BATCH_RS:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_WP_TRANSAKSI:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_WP_PICKER:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_WP_CHECKER:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_WP_PACKER:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_WP_TRANSPORTASI:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_WP_INKJET:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_WP_CHECKER_CekInkJet:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_WP_PO:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_CATEGORY:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_LANTAI:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_VIA:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_TFGUDANG_REGULAR:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_TFGUDANG_KARANTINA:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_TFGUDANG_PEMUSNAHAN:
        
        #region StockOpname Indra 20171231FM
        case Constant.MODEL_COMMON_QUERY_STOCK_OPNAME_GETDATA: 
        case Constant.MODEL_COMMON_QUERY_STOCK_OPNAME_MONITORING:        
        #endregion

        #region Monitoring PL Indra 20180523FM
        case Constant.MODEL_COMMON_QUERY_MONITORINGPL_GETDATA:
        #endregion

        #region Email Produk Kosong 20190411FM
        case Constant.MODEL_COMMON_QUERY_EMAIL_PRODUKKOSONG:
        case Constant.MODEL_COMMON_QUERY_EMAIL_HISTORYPRODUKKOSONG:
        case Constant.REPORT_PROSESEMAIL_PRODUKKOSONG:
        #endregion

          //case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_SP_SPG_PENDING:
          //case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_SiT_PENDING:
          //case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_PO_PENDING:

          dic = CommonQueryMulti.ModelGridQuery(connStr, start, limit, allQuery, sort, dir, model.ToLower(), dicParam);

          break;

        #endregion

        #region CommonQueryProcess.ModelGridQuery

        case Constant.MODEL_PROCESS_QUERY_MULTIPLE_PROCESS_ORP:
        case Constant.MODEL_PROCESS_QUERY_MULTIPLE_PROCESS_ORG:
        case Constant.MODEL_PROCESS_QUERY_MULTIPLE_PROCESS_DOKHUSUS:
        case Constant.MODEL_PROCESS_QUERY_MULTIPLE_PROCESS_RNTRANFER:
        case Constant.MODEL_PROCESS_QUERY_MULTIPLE_MEMO_PROCESS_ITEM_MEMOCOMBO:
        case Constant.MODEL_PROCESS_QUERY_MULTIPLE_PROCESS_CLAIM:
        case Constant.MODEL_PROCESS_QUERY_MULTIPLE_RETURCUSTOMER:
        case Constant.MODEL_COMMON_QUERY_PROSES_AUTO_PL:
        case Constant.MODEL_COMMON_QUERY_PROSES_AUTO_SJ:
        case Constant.MODEL_COMMON_QUERY_PROCESS_ITEMDETAIL_PL_AUTOGEN:
        case Constant.REPORT_INVENTORY_REPORT_CS:
        case Constant.REPORT_INVENTORY_SPPLDO:
        case Constant.REPORT_INVENTORY_SPPLDO_DETAIL:
        case Constant.REPORT_HISTORY_SERVICELEVEL_PO:
        case Constant.REPORT_INVENTORY_SERVICELEVEL_CABANG:  // SERVICE LEVEL CABANG 14052018 HAFIZH
        case Constant.REPORT_INVENTORY_SERVICELEVEL_PPIC:
        case Constant.REPORT_INVENTORY_RECALL:
		//case Constant.REPORT_FORECAST:

        case Constant.REPORT_INVENTORY_REPORT_MONITOTINGED:

        case Constant.REPORT_INVENTORY_MONITORINGEDCTRL:  //hafizh

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_PL_AUTOGEN:
        case Constant.MODEL_COMMON_QUERY_LAPORAN_ENAPZA: //Indra 20170821


          dic = CommonQueryProcess.ModelGridQuery(connStr, start, limit, allQuery, sort, dir, model.ToLower(), dicParam);

          break;

        #endregion

        #region GlobalQuery.ModelGridQuery

        case Constant.MODEL_QUERY_EKSPEDISIGRID_GUDANG:
        case Constant.MODEL_QUERY_EKSPEDISIGRID_GUDANG_DETAIL:
        case Constant.MODEL_QUERY_EKSPEDISIGRID_DETAIL_EXT:

        case Constant.MODEL_QUERY_EKSPEDISIGRID_RS:

        case Constant.MODEL_QUERY_INVOICEEKSPEDISIGRIDEKSTERNAL:
        case Constant.MODEL_QUERY_INVOICEEKSPEDISIGRIDINTERNAL:
        case Constant.MODEL_QUERY_INVOICEEKSPEDISIGRID_DETAIL:
        case Constant.MODEL_QUERY_INVOICEEKSPEDISIGRID2_DETAIL:
        case Constant.MODEL_QUERY_INVOICEEKSPEDISIINTERNALGRID2_DETAIL:

        case Constant.MODEL_QUERY_FAKTUREKSPEDISIGRID:
        case Constant.MODEL_QUERY_FAKTUREKSPEDISIGRID_DETAIL:

        case Constant.MODEL_QUERY_GRID_RETURN_DO:

        case Constant.MODEL_QUERY_GRID_LIMIT_PO_ITEM_GRID:
        case Constant.MODEL_QUERY_GRID_LIMIT_PO_ITEM_DETAIL:
        
        case Constant.MODEL_QUERY_GRID_LIMIT_PO_DIVPRI_GRID:
        case Constant.MODEL_QUERY_GRID_LIMIT_PO_DIVPRI_DETAIL:

        case Constant.MODEL_QUERY_GRID_LIMIT_PO_PRINCIPAL_GRID:
        case Constant.MODEL_QUERY_GRID_LIMIT_PO_PRINCIPAL_DETAIL:
              
        case Constant.MODEL_QUERY_EKSPEDISIGRID:
        case Constant.MODEL_QUERY_EKSPEDISIGRID_DETAIL:

        case Constant.MODEL_QUERY_SURATPESANANGRID:
        case Constant.MODEL_QUERY_SURATPESANANMANUALGRID:
        case Constant.MODEL_QUERY_SURATPESANANGRID_DETAIL:
        case Constant.MODEL_QUERY_PROSESPHARMANETGRID_DETAIL: // hafizh pharmanet
        case Constant.MODEL_QUERY_PROSESPHARMANETGRID: // hafizh pharmanet
        case Constant.MODEL_QUERY_SPOUTSTAND: //suwandi 22 oktober 2018
        case Constant.MODEL_QUERY_SPOUTSTAND_DETAIL: //suwandi 7 November 2018
        case Constant.MODEL_QUERY_SPOUTSTAND_PER_REGION: //suwandi 19 Desember 2018

        case Constant.MODEL_QUERY_STT:
        case Constant.MODEL_QUERY_STT_DETAIL:

        case Constant.MODEL_QUERY_ORDERREQUESTPRINCIPALGRID:
        case Constant.MODEL_QUERY_ORDERREQUESTPRINCIPALGRID_DETAIL:
        case Constant.MODEL_QUERY_ORDERREQUESTGUDANGGRID:
        case Constant.MODEL_QUERY_ORDERREQUESTGUDANGGRID_DETAIL:

        case Constant.MODEL_QUERY_PURCHASEORDER:
        case Constant.MODEL_QUERY_PURCHASEORDER_OUTSTAND:
        case Constant.MODEL_QUERY_PURCHASEORDER_DETAIL:

        case Constant.MODEL_QUERY_RNBELI:
        case Constant.MODEL_QUERY_RNBELI_ULUJAMI: // ULUJAMI
        case Constant.MODEL_QUERY_RNBELI_DETAIL:
        case Constant.MODEL_QUERY_RNBELI_DETAIL_ULUJAMI:  // ULUJAMI
        case Constant.MODEL_QUERY_KHUSUS:
        case Constant.MODEL_QUERY_KHUSUS_DETAIL:
        case Constant.MODEL_QUERY_RETUR:
        case Constant.MODEL_QUERY_RETUR_DETAIL:
        case Constant.MODEL_QUERY_CLAIM:
        case Constant.MODEL_QUERY_CLAIM_DETAIL:
        case Constant.MODEL_QUERY_REPACK:
        case Constant.MODEL_QUERY_REPACK_DETAIL:

        case Constant.MODEL_QUERY_MK_MEMOCOMBO:
        case Constant.MODEL_QUERY_MK_MEMOCOMBO_DETAIL:

        case Constant.MODEL_QUERY_MEMO_MEMOCOMBO:
        case Constant.MODEL_QUERY_MEMO_MEMOCOMBO_DETAIL:
        case Constant.MODEL_QUERY_MEMO_MEMODONASI:
        case Constant.MODEL_QUERY_MEMO_MEMODONASI_DETAIL:
        case Constant.MODEL_QUERY_MEMO_MEMOSAMPLE:
        case Constant.MODEL_QUERY_MEMO_MEMOSAMPLE_DETAIL:
        case Constant.MODEL_QUERY_MEMO_MEMOPEMUSNAHAN:
        case Constant.MODEL_QUERY_MEMO_MEMOPEMUSNAHAN_DETAIL:

        case Constant.MODEL_QUERY_RC:
        case Constant.MODEL_QUERY_RC_RESEND:
        case Constant.MODEL_QUERY_RC_DETAIL:

        case Constant.MODEL_QUERY_RS_PEMBELIAN:
        case Constant.MODEL_QUERY_RS_PEMBELIAN_DETAIL:
        case Constant.MODEL_QUERY_RS_PEMBELIAN_CONF:
        case Constant.MODEL_QUERY_RS_REPACK_DETAIL:
        case Constant.MODEL_QUERY_RS_PEMBELIAN_CONF_DETIL:

        case Constant.MODEL_QUERY_CLAIM_REGULAR:
        case Constant.MODEL_QUERY_CLAIM_STT:
        case Constant.MODEL_QUERY_CLAIM_REGULAR_STT_DETAIL:

        case Constant.MODEL_QUERY_CLAIM_ACC_REGULAR:

        case Constant.MODEL_QUERY_CLAIM_ACC_REGULAR_STT:
        case Constant.MODEL_QUERY_CLAIM_ACC_REGULAR_STT_DETIL:

        case Constant.MODEL_QUERY_ADJUST_STOCK:
        case Constant.MODEL_QUERY_ADJUST_STOCK_DETIL:
        case Constant.MODEL_QUERY_EKSPEDISIGRID_CABANG:
        case Constant.MODEL_QUERY_EKSPEDISI_DOSJ_Pending:

        case Constant.MODEL_QUERY_DOPRINSIPALGRID:
        case Constant.MODEL_QUERY_DOPRINSIPALGRIDDETAIL:

        case Constant.MODEL_QUERY_TRANSFERGUDANG:
        case Constant.MODEL_QUERY_TRANSFERGUDANG_DETAIL:
        case Constant.MODEL_QUERY_WP_LOGISTIK:
        case Constant.MODEL_QUERY_WP_LOGISTIK_GRID:
        case Constant.MODEL_QUERY_WP_SERAHTERIMA:
        case Constant.MODEL_QUERY_WP_SERAHTERIMA_DETAIL:
        case Constant.MODEL_QUERY_WP_SERAHTERIMA_DETAIL_EKSPEDISI:
        case Constant.MODEL_QUERY_WP_SERAHTERIMA_SEARCH:
        case Constant.MODEL_QUERY_WP_SERAHTERIMA_TRANSPORTASI_DO:
        case Constant.MODEL_QUERY_WP_SERAHTERIMA_TRANSPORTASI_SJ:
        case Constant.MODEL_QUERY_WP_SERAHTERIMA_TRANSPORTASI_GRIDDETAIL:

        case Constant.MODEL_QUERY_INVOICE_RESI_EKSPEDISI_ADD_DETAIL_RESI:
        case Constant.MODEL_QUERY_INVOICE_RESI_EKSPEDISI_ADD_DETAIL_EXP:
        case Constant.MODEL_QUERY_CALC_BIAYA_EKSPEDISI:
        case Constant.MODEL_QUERY_INVOICE_EP_EKSPEDISI_DETAIL:
        case Constant.MODEL_QUERY_FAKTUR_EKSPEDISI_DETAIL:
              
        case Constant.MODEL_QUERY_PM:
        case Constant.MODEL_QUERY_PM_DETAIL:

        case Constant.MODEL_QUERY_WP_SERAHTERIMA_PO:
        case Constant.MODEL_QUERY_WP_SERAHTERIMA_PODETAIL:
        case Constant.MODEL_QUERY_WP_MONITORING_PO:
        case Constant.MODEL_QUERY_WP_MONITORING_PODETAIL:
        case Constant.MODEL_QUERY_INV_PENDING_INTEGRITY:
        #region Indra Monitoring Process 20180523FM

        case Constant.MODEL_QUERY_WP_MONITORING_PL: //Indra 20180523FM
        case Constant.MODEL_QUERY_WP_MONITORING_PL_EKSPEDISI: //Indra 20180523FM
        case Constant.MODEL_QUERY_WP_MONITORING_PL_EKSPEDISI_DETAIL: //Indra 20180523FM
        case Constant.MODEL_QUERY_WP_MONITORING_PL_SUMMARY: //Indra 20180523FM
        case Constant.MODEL_QUERY_WP_MONITORING_PL_SUMMARY_V2: //Indra 20180523FM
        case Constant.MODEL_QUERY_WP_MONITORING_PL_DETAIL_PL: //Indra 20180523FM
        case Constant.MODEL_QUERY_WP_MONITORING_PL_FILTERCABANG: //Indra 20180909FM
        case Constant.MODEL_QUERY_WP_MONITORING_PL_CHART: //Indra 20190116FM
        
        #endregion

        case Constant.MODEL_QUERY_PO_APOTEKER_GRID:

        case Constant.MODEL_QUERY_MEMOBASPB_SJ_GRID:
        case Constant.MODEL_QUERY_MEMOBASPB_SJ_DETAIL:
        case Constant.MODEL_QUERY_MOVEMENT_STOCK:
        case Constant.MODEL_QUERY_MOVEMENT_STOCK_DETAIL:


          dic = GlobalQuery.ModelGridQuery(connStr, start, limit, allQuery, sort, dir, model.ToLower(), dicParam);

          break;

        #endregion

        #region GlobalQuery.ModelGridQueryPenjualan

        case Constant.MODEL_QUERY_PACKINGLISTGRID:
        case Constant.MODEL_QUERY_PACKINGLISTGRID_GUDANG3:
        case Constant.MODEL_QUERY_PACKINGLISTGRID_UNPRINT:
        case Constant.MODEL_QUERY_PACKINGLISTGRID_DETAIL:

        case Constant.MODEL_QUERY_PACKINGLISTGRID_DETAIL_AUTOGEN:

        case Constant.MODEL_QUERY_DOGRID_PL:
        case Constant.MODEL_QUERY_DOGRID_PL_SEND:
        case Constant.MODEL_QUERY_DOGRID_PL_DISCORE:
        case Constant.MODEL_QUERY_DOGRID_DETAIL_PL:
        case Constant.MODEL_QUERY_DOGRID_DETAIL_PL_NO:
        case Constant.MODEL_QUERY_DOGRID_DETAIL_PL_WBATCH:
        case Constant.MODEL_QUERY_DOGRID_STT:
        case Constant.MODEL_QUERY_DOGRID_DETAIL_STT:
        case Constant.MODEL_QUERY_DOGRID_DETAIL_STT_NO:

        case Constant.MODEL_QUERY_DOGRID_GLOBAL:

        case Constant.MODEL_QUERY_TRANSFERGUDANGGRID_GLOBAL:
        case Constant.MODEL_QUERY_TRANSFERGUDANGGRID:
        case Constant.MODEL_QUERY_TRANSFERGUDANGGRID_DETAIL:
        case Constant.MODEL_QUERY_TRANSFERGUDANGGRID_KARANTINA:
        case Constant.MODEL_QUERY_TRANSFERGUDANGGRID_PEMUSNAHAN:

          dic = GlobalQuery.ModelGridQueryOutgoing(connStr, start, limit, allQuery, sort, dir, model.ToLower(), dicParam);

          break;

        #endregion

        // Finance

        #region GlobalQuery.ModelGridQueryFinancial

        case Constant.MODEL_QUERY_PEMBAYARAN_HUTANGVC:
        case Constant.MODEL_QUERY_PEMBAYARAN_HUTANGVCDETIL:
        case Constant.MODEL_QUERY_PEMBAYARAN_PIUTANGVC:
        case Constant.MODEL_QUERY_PEMBAYARAN_PIUTANGVCDETIL:
        case Constant.MODEL_QUERY_PEMBAYARAN_HUTANGVCHEADER:
        case Constant.MODEL_QUERY_PEMBAYARAN_PIUTANGVCHEADER:
        case Constant.MODEL_QUERY_FAKTURJUALGRID:
        case Constant.MODEL_QUERY_FAKTURJUALGRID_DETAIL:
        case Constant.MODEL_QUERY_FAKTURBELIGRID:
        case Constant.MODEL_QUERY_FAKTURBELIGRID_DETAIL:
        case Constant.MODEL_QUERY_FAKTURBELIGRID_DETAIL_BEA:
        case Constant.MODEL_QUERY_FAKTURJUALRETURGRID:
        case Constant.MODEL_QUERY_FAKTURJUALRETURGRID_DETAIL:
        case Constant.MODEL_QUERY_FAKTURBELIRETURGRID:
        case Constant.MODEL_QUERY_FAKTURBELIRETURGRID_DETAIL:
        case Constant.MODEL_QUERY_FAKTURBELICLAIMRETURGRID:
        case Constant.MODEL_QUERY_FAKTURBELICLAIMRETURGRID_DETAIL:
        case Constant.MODEL_QUERY_FAKTURMANUAL_GRID:

          dic = GlobalQuery.ModelGridQueryFinance(connStr, start, limit, allQuery, sort, dir, model.ToLower(), dicParam);

          break;

        #endregion

        #region CommonQueryMulti.ModelGridQueryFinance

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_FAKTUR_SUPLIER:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_FAKTUR_CUSTOMER:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_FB_RN:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_FB_RN_ITEM:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_FB_RN_ITEM_FLOATING:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_FB_SUPPLIER:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_SUPLIER_FAKTURMANUAL:

          dic = CommonQueryMulti.ModelGridQueryFinance(connStr, start, limit, allQuery, sort, dir, model.ToLower(), dicParam);

          break;

        #endregion

        // Adjustment

        #region GlobalQuery.ModelGridQueryAdjusment

        case Constant.MODEL_QUERY_ADJUSTMENT_TRANS:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ADJ_NO_PO:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ADJ_NO_PO_ITEM:
        case Constant.MODEL_QUERY_ADJUSTMENT_TRANS_FAKTUR:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ADJ_NO_FAKTUR:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ADJ_NO_FAKTUR_FB:
        case Constant.MODEL_QUERY_ADJUSTMENT_TRANS_FAKTUR_DETIL:
        case Constant.MODEL_QUERY_ADJUSTMENT_TRANS_STT:
        case Constant.MODEL_QUERY_ADJUSTMENT_TRANS_STT_DETIL:
        case Constant.MODEL_QUERY_ADJUSTMENT_TRANS_STT_NO:
        case Constant.MODEL_QUERY_ADJUSTMENT_TRANS_STT_ITEM:
        case Constant.MODEL_QUERY_ADJUSTMENT_TRANS_STT_BATCH:
        case Constant.MODEL_QUERY_ADJUSTMENT_TRANS_VOUCHER:
        case Constant.MODEL_QUERY_ADJUSTMENT_TRANS_VOUCHER_DEBIT:
        case Constant.MODEL_QUERY_ADJUSTMENT_TRANS_VOUCHER_CREDIT:
        case Constant.MODEL_QUERY_ADJUSTMENT_TRANS_VOUCHER_CREDIT_DETIL:
        case Constant.MODEL_QUERY_ADJUSTMENT_TRANS_VOUCHER_DEBIT_DETIL:
        case Constant.MODEL_QUERY_ADJUSTMENT_TRANS_DETIL:
        case Constant.MODEL_QUERY_ADJUSTMENT_TRANS_MEMO:

          dic = GlobalQuery.ModelGridQueryAdjusment(connStr, start, limit, allQuery, sort, dir, model.ToLower(), dicParam);

          break;

        #endregion

        // Master

        #region GlobalQuery.ModelGridQueryMaster

        case Constant.MODEL_QUERY_MASTER_DISCOUNTGRID:
        case Constant.MODEL_QUERY_MASTER_DISCOUNTGRID_DETAIL:
        case Constant.MODEL_QUERY_MASTER_DISCOUNTGRID_DETAIL_SPESIFIK:
        case Constant.MODEL_QUERY_MASTER_ITEM:
        case Constant.MODEL_QUERY_MASTER_BATCH:
        case Constant.MODEL_QUERY_MASTER_BATCH_DETAIL:
        case Constant.MODEL_QUERY_MASTER_BUDGET:
        case Constant.MODEL_QUERY_MASTER_BUDGET_DETAIL:
        case Constant.MODEL_QUERY_MASTER_PRISIPAL:
        case Constant.MODEL_QUERY_MASTER_PRISIPAL_HISTORY_LEADTIME:
        case Constant.MODEL_QUERY_MASTER_DIVISIPRINSIPAL:
        case Constant.MODEL_QUERY_MASTER_CUSTOMER:
        case Constant.MODEL_QUERY_MASTER_EXPEDISI:
        case Constant.MODEL_QUERY_MASTER_CABANG_HARI:
        case Constant.MODEL_QUERY_CEK_USER_APPROVAL:
        case Constant.MODEL_QUERY_CEK_STATUS_APPROVAL:
        case Constant.MODEL_QUERY_MASTER_ESTIMASI:
        case Constant.MODEL_QUERY_MASTER_ESTIMASI_CABANG:
        case Constant.MODEL_QUERY_MASTER_BLOCKITEM:
        case Constant.MODEL_QUERY_MASTER_BLOCKITEM_DETAIL:
        case Constant.MODEL_QUERY_MASTER_BANK:
        case Constant.MODEL_QUERY_MASTER_BANK_REKENING:
        case Constant.MODEL_QUERY_MASTER_DIVISIPRINSIPAL_ITEM:
        case Constant.MODEL_QUERY_MASTER_DIVISIPRINSIPAL_DETIL:
        case Constant.MODEL_QUERY_MASTER_COMBO:
        case Constant.MODEL_QUERY_MASTER_COMBO_DETAIl:
        case Constant.MODEL_QUERY_MASTER_DIVISI_AMS:
        case Constant.MODEL_QUERY_MASTER_DIVISI_AMS_DETIL:
        case Constant.MODEL_QUERY_MASTER_ITEM_CATEGORY:
        case Constant.MODEL_QUERY_MASTER_ITEM_CATEGORY_ITEM:
        case Constant.MODEL_QUERY_MASTER_ITEM_LANTAI:
        case Constant.MODEL_QUERY_MASTER_USER_APJ:
        case Constant.MODEL_QUERY_MASTER_ITEM_VIA:
        case Constant.MODEL_QUERY_MASTER_DRIVER:
        case Constant.MODEL_QUERY_MASTER_COST_ESTIMASI:
        case Constant.MODEL_QUERY_MASTER_PKP_GRID:
        case Constant.MODEL_QUERY_MASTER_NOMOR_PAJAK:
        case Constant.MODEL_QUERY_HISTORY_SP:

          dic = GlobalQuery.ModelGridQueryMaster(connStr, start, limit, allQuery, sort, dir, model.ToLower(), dicParam);

          break;

        #endregion

        #region CommonQueryBridge.ModelGridQuery

        case Constant.MODEL_COMMON_QUERY_BRIGE_RC:
        case Constant.MODEL_COMMON_QUERY_BRIGE_EKSPEDISI:
        case Constant.MODEL_COMMON_QUERY_BRIGE_BASPB:
        case Constant.MODEL_COMMON_QUERY_BRIGE_DO_PRINCIPAL:

          dic = CommonQueryBrige.ModelGridQuery(connStr, start, limit, allQuery, sort, dir, model.ToLower(), dicParam);

          break;

        #endregion

        // Store Procedure

        #region CommonQueryProcess.ModelGridQueryStoreProcedure

        case Constant.MODEL_PROCESS_QUERY_SP_BDPRDP:
        case Constant.MODEL_PROCESS_QUERY_SP_CLOSINGPO:
        case Constant.MODEL_PROCESS_QUERY_SP_CLOSSINGLOG:
        case Constant.MODEL_COMMON_QUERY_AVG_SALES:
        case Constant.MODEL_COMMON_QUERY_FORECAST:

          dic = CommonQueryProcess.ModelGridQueryStoreProcedure(connStr, start, limit, allQuery, sort, dir, model.ToLower(), dicParam);

          break;

        #endregion

        // Right Management

        #region CommonQueryMulti.ModelGridQueryRightManagement

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_USERRIGHT:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_GROUPRIGHT:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_USERGROUPRIGHT:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_USERGROUPRIGHT_USER:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_USERGROUPRIGHT_USER_AVAIBLE:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_USERGROUPRIGHT_GROUP:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_USERGROUPRIGHT_GROUP_AVAIBLE:

          dic = CommonQueryMulti.ModelGridQueryRightManagement(connStr, start, limit, allQuery, sort, dir, model.ToLower(), dicParam);

          break;

        #endregion

        // Core

        #region CommonQueryMulti.ModelGridQueryCore

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_CUSTOMER_PL:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_SUPLIER_PL:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_PL:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_SPDETAIL_PL:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_BATCHDETAIL_PL:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_CATEGORI_PL:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_VIA_PL:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_BASPB:
        case Constant.MODEL_COMMON_QUERY_NO_PL_PHARMANET:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_RNKHUSUS_PL_AUTO:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_PL_AUTO:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_SPDETAIL_PL_AUTO:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_BATCH_PL_AUTO:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_SP_SPG_PENDING:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_SiT_PENDING:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_SOT_PENDING:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_Tot_PENDING:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_PLBOKING_PENDING:  //PL Boking Hafizh 08 maret 2018

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_PO_PENDING:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_SPG_PENDING:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_DETAIL_GOOD:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_DETAIL_BAD:

        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_DETAIL_GOOD_ALLGDG:
        case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_DETAIL_GOOD_CABANG:

          dic = CommonQueryMulti.ModelGridQueryCore(connStr, start, limit, allQuery, sort, dir, model.ToLower(), dicParam);

          break;

        #endregion

        #region Default

        default:
          {
            dic = new Dictionary<string, object>();

            dic.Add(Constant.DEFAULT_NAMING_RECORDS, null);

            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, 0);

            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);

            dic.Add(Constant.DEFAULT_NAMING_EXCEPTION, "Invalid model");
          }
          break;

        #endregion
      }

      #endregion

      #region Parser

      if (dic != null)
      {
        //result = JsonConvert.SerializeObject(dic);

        //int nCount = (dic.ContainsKey(Constant.DEFAULT_NAMING_TOTAL_ROWS) ? (int)dic[Constant.DEFAULT_NAMING_TOTAL_ROWS] : -1);

        if (!dic.ContainsKey(Constant.DEFAULT_NAMING_RECORDS))
        {
          dic.Add(Constant.DEFAULT_NAMING_RECORDS, string.Empty);
        }
        else if (dic.ContainsKey(Constant.DEFAULT_NAMING_RECORDS) && (dic[Constant.DEFAULT_NAMING_RECORDS] == null))
        {
          dic[Constant.DEFAULT_NAMING_RECORDS] = string.Empty;
        }

        JsonSerializerSettings ser = new JsonSerializerSettings();

        if ((dateTimeConverter != null) && dateTimeConverter.GetType().BaseType.Equals(typeof(Newtonsoft.Json.Converters.DateTimeConverterBase)))
        {
          ser.Converters.Add(dateTimeConverter);
        }
        else
        {
          JsonDateTime jDateTime = new JsonDateTime();
          ser.Converters.Add(jDateTime);
        }

        JsonString jString = new JsonString();
        ser.Converters.Add(jString);

        result = JsonConvert.SerializeObject(dic, Formatting.None, ser);

        //result = JsonConvert.SerializeObject(dic, Formatting.None);

        //result = string.Format("{{\"d\":{0}}}", result);

        dic.Clear();
      }

      #endregion

      dicParam.Clear();

      return result;
    }

    private string QueryLogicUploaded(string model, string originalName, byte[] data, string[][] parameters, bool jsonp)
    {
      return QueryLogicUploaded(model, originalName, data, parameters, jsonp, null);
    }

    private string QueryLogicUploaded(string model, string originalName, byte[] data, string[][] parameters, bool jsonp, JsonConverter dateTimeConverter)
    {
      if (string.IsNullOrEmpty(model))
      {
        return null;
      }

      string result = null;

      IDictionary<string, Functionals.ParameterParser> dicParam = Functionals.ParserArrayParameter(parameters);
      
      model = model.ToLower();

      Config config = Functionals.Configuration;
      string connStr = Functionals.ActiveConnectionString;

      IDictionary<string, object> dic = null;

      #region Case

      switch (model)
      {
        #region CommonQuery.ModelGridQuery

        case Constant.MODEL_COMMON_UPLOADED_QUERY_IMPORTRS:
        case Constant.MODEL_COMMON_UPLOADED_QUERY_IMPORTFB:
        case Constant.MODEL_COMMON_UPLOADED_QUERY_IMPORTRS_EXPEDISI:
        case Constant.MODEL_COMMON_UPLOADED_QUERY_ZIPDBF:
        case Constant.MODEL_COMMON_UPLOADED_QUERY_VOUCHER_EXCELL:
        case Constant.MODEL_COMMON_UPLOADED_QUERY_VOUCHER_EXCELL_USER:
        case Constant.MODEL_COMMON_UPLOADED_QUERY_SO:
        case Constant.MODEL_COMMON_UPLOADED_QUERY_SEND_BATCH: //Indra 20170714


          dic = CommonUploadedQuery.ModelUploadedQuery(connStr, model, originalName, data, dicParam);

          break;

        #endregion
      }

      #endregion

      #region Parser

      if (dic != null)
      {
        if (!dic.ContainsKey(Constant.DEFAULT_NAMING_RECORDS))
        {
          dic.Add(Constant.DEFAULT_NAMING_RECORDS, string.Empty);
        }
        else if (dic.ContainsKey(Constant.DEFAULT_NAMING_RECORDS) && (dic[Constant.DEFAULT_NAMING_RECORDS] == null))
        {
          dic[Constant.DEFAULT_NAMING_RECORDS] = string.Empty;
        }

        JsonSerializerSettings ser = new JsonSerializerSettings();

        if ((dateTimeConverter != null) && dateTimeConverter.GetType().BaseType.Equals(typeof(Newtonsoft.Json.Converters.DateTimeConverterBase)))
        {
          ser.Converters.Add(dateTimeConverter);
        }
        else
        {
          JsonDateTime jDateTime = new JsonDateTime();
          ser.Converters.Add(jDateTime);
        }

        JsonString jString = new JsonString();
        ser.Converters.Add(jString);

        result = JsonConvert.SerializeObject(dic, Formatting.None, ser);

        //result = JsonConvert.SerializeObject(dic, Formatting.None);

        //result = string.Format("{{\"d\":{0}}}", result);

        dic.Clear();
      }

      #endregion

      dicParam.Clear();

      return result;
    }

    #endregion

    public static bool IsJsonPaddingActive
    { get; set; }

    public string GlobalQueryJson(int start, int limit, bool allQuery, string sort, string dir, string model, string[][] parameters)
    {
      return QueryLogic(start, limit, allQuery, sort, dir, model, parameters, true);
    }

    public string GlobalQueryService(int start, int limit, bool allQuery, string sort, string dir, string model, string[][] parameters)
    {
      return QueryLogic(start, limit, allQuery, sort, dir, model, parameters, true);
    }

    public string GlobalQueryServiceClient(int start, int limit, bool allQuery, string sort, string dir, string model, string[][] parameters)
    {
      JsonDisCoreDateTime jdcDateTime = new JsonDisCoreDateTime();

      return QueryLogic(start, limit, allQuery, sort, dir, model, parameters, true, jdcDateTime);
    }

    public string GlobalPostUploadedData(string model, string originalName, byte[] data, string[][] parameters)
    {
      return QueryLogicUploaded(model, originalName, data, parameters, true);
    }

    public string CheckConnectionDatabase()
    {
      string result = null;

      ScmsModel.ORMDataContext db = null;

      try
      {
        db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

        db.Connection.Open();

        result = "Result : Ok";
      }
      catch (Exception ex)
      {
        result = string.Format("Result : {0}", ex.Message);
      }
      finally
      {
        if (db != null)
        {
          db.Dispose();
        }
      }

      return result;
    }

    public string PostData(string data)
    {
      return Parser.MyAssembly.MyAssemblyParser(data);
    }

    public string TestPostData(string data)
    {
      return string.Concat("Data : ", data);
    }

    public string GetData()
    {
      return string.Concat("Get data", DateTime.Now);
    }

    public void Testing()
    {
      //CommonQueryTesting cqt = new CommonQueryTesting();

      //cqt.TestPostDataDO(null, "DO12010013");

      ScmsSoaLibrary.Core.Reports.ReportDatasetBind rdb = new ScmsSoaLibrary.Core.Reports.ReportDatasetBind();
      ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);
      //db.Log = Console.Out;

      rdb.TestCode(db);
    }

    public string PostDataDiscore(XElement data)
    {
      //Master Untuk SQL Server Pakai parameter 
        Console.WriteLine(data);
      string result = null;
      try
      {
        if (!string.IsNullOrEmpty(data.ToString()))
        {

          string value = Parser.ParserDisCore.GeneralizeObject(data.ToString());

          result = Parser.MyAssembly.MyAssemblyParser(value);

        }
      }
      catch (Exception ex)
      {
        result = string.Format("Result : {0}", ex.Message);
      }

      return result;

      //throw new NotImplementedException();
    }
  }
}