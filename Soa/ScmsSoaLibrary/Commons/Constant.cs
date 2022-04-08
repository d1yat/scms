using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScmsSoaLibrary.Commons
{
  public static class Constant
  {
    public const string DEFAULT_NAMING_TOTAL_ROWS = "totalRows";
    public const string DEFAULT_NAMING_RECORDS = "records";
    public const string DEFAULT_NAMING_SUCCESS = "success";
    public const string DEFAULT_NAMING_EXCEPTION = "exception";

    public const int DEFAULT_COUNT_LIMIT_GRID = 100;
    public const int MAX_QUERY_LIST_SIZE = 1300;

    public const string SYSTEM_USERNAME = "system";

    public static string TRANSID = "";
    public static char Gudang = '?';
    public static string REFID = "";
    public static string NUMBERID_GUDANG = "";
    public static bool isDcoreError = false;

    public const string DEFAULT_NAMING_FIRSTDATE = "firstdate";
    public const string DEFAULT_NAMING_ENDDATE = "enddate";

    #region Query

    #region Global Query

    public const string MODEL_QUERY_PACKINGLISTGRID = "0001";
    public const string MODEL_QUERY_PACKINGLISTGRID_GUDANG3 = "0001-a";
    public const string MODEL_QUERY_PACKINGLISTGRID_UNPRINT = "0001-b";
    public const string MODEL_QUERY_PACKINGLISTGRID_DETAIL = "0002";
    public const string MODEL_QUERY_TRANSFERGUDANGGRID = "0003";
    public const string MODEL_QUERY_TRANSFERGUDANGGRID_GLOBAL = "0003-a";
    public const string MODEL_QUERY_TRANSFERGUDANGGRID_DETAIL = "0004";
    public const string MODEL_QUERY_SURATPESANANGRID = "0011";
    public const string MODEL_QUERY_SURATPESANANMANUALGRID = "0011-a";
    public const string MODEL_QUERY_PROSESPHARMANETGRID = "0024"; // Hafizh Pharmanet
    public const string MODEL_QUERY_PROSESPHARMANETGRID_DETAIL = "2500"; // hafizh Pharmanet
    public const string MODEL_QUERY_SURATPESANANGRID_DETAIL = "0012";
    public const string MODEL_QUERY_ORDERREQUESTPRINCIPALGRID = "0013";
    public const string MODEL_QUERY_ORDERREQUESTPRINCIPALGRID_DETAIL = "0014";
    public const string MODEL_QUERY_ORDERREQUESTGUDANGGRID = "0015";
    public const string MODEL_QUERY_ORDERREQUESTGUDANGGRID_DETAIL = "0016";
    public const string MODEL_QUERY_SPOUTSTAND = "0011-b";
    public const string MODEL_QUERY_SPOUTSTAND_DETAIL = "0011-c";
    public const string MODEL_QUERY_SPOUTSTAND_PER_REGION = "0011-d";

    public const string MODEL_QUERY_TRANSFERGUDANGGRID_KARANTINA = "0019";
    public const string MODEL_QUERY_TRANSFERGUDANGGRID_PEMUSNAHAN = "0020";

    public const string MODEL_QUERY_PO_APOTEKER_GRID = "0022";

    public const string MODEL_QUERY_PACKINGLISTGRID_DETAIL_AUTOGEN = "0203";

    public const string MODEL_QUERY_EKSPEDISIGRID_GUDANG = "0211";
    public const string MODEL_QUERY_EKSPEDISIGRID_GUDANG_DETAIL = "0212";
    public const string MODEL_QUERY_EKSPEDISIGRID_RS = "0216";
    public const string MODEL_QUERY_EKSPEDISIGRID = "0005";
    public const string MODEL_QUERY_EKSPEDISIGRID_DETAIL = "0006";
    public const string MODEL_QUERY_EKSPEDISIGRID_DETAIL_EXT = "03106";
    public const string MODEL_QUERY_INVOICEEKSPEDISIGRIDEKSTERNAL = "04001";
    public const string MODEL_QUERY_INVOICEEKSPEDISIGRID_DETAIL = "04002";
    public const string MODEL_QUERY_INVOICEEKSPEDISIGRIDINTERNAL = "04003";
    public const string MODEL_QUERY_INVOICEEKSPEDISIGRID2_DETAIL = "04004";
    public const string MODEL_QUERY_INVOICEEKSPEDISIINTERNALGRID2_DETAIL = "04005";
    public const string MODEL_QUERY_FAKTUREKSPEDISIGRID = "05001";
    public const string MODEL_QUERY_FAKTUREKSPEDISIGRID_DETAIL = "05002";
    public const string MODEL_QUERY_GRID_RETURN_DO = "05003";

    public const string MODEL_QUERY_MEMOBASPB_SJ_GRID = "05004";
    public const string MODEL_QUERY_MEMOBASPB_SJ_DETAIL = "05005";

    public const string MODEL_QUERY_GRID_LIMIT_PO_ITEM_GRID = "05006";
    public const string MODEL_QUERY_GRID_LIMIT_PO_ITEM_DETAIL = "05007";
    
    public const string MODEL_QUERY_GRID_LIMIT_PO_DIVPRI_GRID = "05008";
    public const string MODEL_QUERY_GRID_LIMIT_PO_DIVPRI_DETAIL = "05009";

    public const string MODEL_QUERY_GRID_LIMIT_PO_PRINCIPAL_GRID = "05010";
    public const string MODEL_QUERY_GRID_LIMIT_PO_PRINCIPAL_DETAIL = "05011";

    
    public const string MODEL_QUERY_DOGRID_PL = "0007";
    public const string MODEL_QUERY_DOGRID_PL_SEND = "0007s";
    public const string MODEL_QUERY_DOGRID_PL_DISCORE = "0007a";
    public const string MODEL_QUERY_DOGRID_GLOBAL = "0007-9-a";
    public const string MODEL_QUERY_DOGRID_STT = "0009";
    public const string MODEL_QUERY_DOGRID_DETAIL_PL = "0008";
    public const string MODEL_QUERY_DOGRID_DETAIL_PL_NO = "0008a";
    public const string MODEL_QUERY_DOGRID_DETAIL_PL_WBATCH = "0008b";
    public const string MODEL_QUERY_DOGRID_DETAIL_STT = "0010";
    public const string MODEL_QUERY_DOGRID_DETAIL_STT_NO = "0010a";
    public const string MODEL_QUERY_EKSPEDISIGRID_CABANG = "0181";
    public const string MODEL_QUERY_EKSPEDISI_DOSJ_Pending = "0182";

    public const string MODEL_QUERY_PURCHASEORDER = "0017";
    public const string MODEL_QUERY_PURCHASEORDER_OUTSTAND = "0017-a";
    public const string MODEL_QUERY_PURCHASEORDER_DETAIL = "0018";

    public const string MODEL_QUERY_STT = "0021";
    public const string MODEL_QUERY_STT_DETAIL = "0025";

    public const string MODEL_QUERY_RNBELI = "0026";
    public const string MODEL_QUERY_RNBELI_ULUJAMI = "0050-a"; // HAFIZH ULUJAMI
    public const string MODEL_QUERY_RNBELI_DETAIL = "0027";
    public const string MODEL_QUERY_RNBELI_DETAIL_ULUJAMI = "0027-b"; // HAFIZH ULUJAMI 
    public const string MODEL_QUERY_KHUSUS = "0028";
    public const string MODEL_QUERY_KHUSUS_DETAIL = "0029";
    public const string MODEL_QUERY_RETUR = "0030";
    public const string MODEL_QUERY_RETUR_DETAIL = "0031";

    public const string MODEL_QUERY_RC_DETAIL = "0032";
    public const string MODEL_QUERY_RC = "0037";
    public const string MODEL_QUERY_RC_RESEND = "0037a";
      
    public const string MODEL_QUERY_CLAIM = "0038";
    public const string MODEL_QUERY_CLAIM_DETAIL = "0039";
    public const string MODEL_QUERY_REPACK = "0040";
    public const string MODEL_QUERY_REPACK_DETAIL = "0041";
    public const string MODEL_QUERY_TRANSFERGUDANG = "0042";
    public const string MODEL_QUERY_TRANSFERGUDANG_DETAIL = "0043";

    public const string MODEL_QUERY_MK_MEMOCOMBO = "0044";
    public const string MODEL_QUERY_MK_MEMOCOMBO_DETAIL = "0045";

    public const string MODEL_QUERY_MEMO_MEMOCOMBO = "0046";
    public const string MODEL_QUERY_MEMO_MEMOCOMBO_DETAIL = "0047";
    public const string MODEL_QUERY_MEMO_MEMODONASI = "0048";
    public const string MODEL_QUERY_MEMO_MEMODONASI_DETAIL = "0048a";
    public const string MODEL_QUERY_MEMO_MEMOSAMPLE = "0049";
    public const string MODEL_QUERY_MEMO_MEMOSAMPLE_DETAIL = "0049a";
    public const string MODEL_QUERY_MEMO_MEMOPEMUSNAHAN = "0050";
    public const string MODEL_QUERY_MEMO_MEMOPEMUSNAHAN_DETAIL = "0050a";

    public const string MODEL_QUERY_RS_PEMBELIAN = "0051";
    public const string MODEL_QUERY_RS_PEMBELIAN_CONF = "0052";
    public const string MODEL_QUERY_RS_PEMBELIAN_CONF_DETIL = "0137";
    public const string MODEL_QUERY_RS_PEMBELIAN_DETAIL = "0053";
    public const string MODEL_QUERY_RS_REPACK_DETAIL = "0054";

    public const string MODEL_QUERY_CLAIM_REGULAR = "0061";
    public const string MODEL_QUERY_CLAIM_STT = "0062";
    public const string MODEL_QUERY_CLAIM_REGULAR_STT_DETAIL = "0063";
    public const string MODEL_QUERY_CLAIM_ACC_REGULAR = "0067";
    public const string MODEL_QUERY_CLAIM_ACC_STT = "0068";

    public const string MODEL_QUERY_CLAIM_ACC_REGULAR_STT = "0070";
    public const string MODEL_QUERY_CLAIM_ACC_REGULAR_STT_DETIL = "0073";

    public const string MODEL_QUERY_ADJUST_STOCK = "0074";
    public const string MODEL_QUERY_ADJUST_STOCK_DETIL = "0075";

    public const string MODEL_QUERY_PEMBAYARAN_HUTANGVC = "0080";
    public const string MODEL_QUERY_PEMBAYARAN_HUTANGVCDETIL = "0081";
    public const string MODEL_QUERY_PEMBAYARAN_PIUTANGVC = "0082";
    public const string MODEL_QUERY_PEMBAYARAN_PIUTANGVCDETIL = "0083";
    public const string MODEL_QUERY_PEMBAYARAN_HUTANGVCHEADER = "0084";
    public const string MODEL_QUERY_PEMBAYARAN_PIUTANGVCHEADER = "0085";

    public const string MODEL_QUERY_FAKTURJUALGRID = "0100";
    public const string MODEL_QUERY_FAKTURJUALGRID_DETAIL = "0101";
    public const string MODEL_QUERY_FAKTURBELIGRID = "0102";
    public const string MODEL_QUERY_FAKTURBELIGRID_DETAIL = "0103";
    public const string MODEL_QUERY_FAKTURBELIGRID_DETAIL_BEA = "0104";
    public const string MODEL_QUERY_FAKTURJUALRETURGRID = "0105";
    public const string MODEL_QUERY_FAKTURJUALRETURGRID_DETAIL = "0106";
    public const string MODEL_QUERY_FAKTURBELIRETURGRID = "0107";
    public const string MODEL_QUERY_FAKTURBELIRETURGRID_DETAIL = "0108";
    public const string MODEL_QUERY_FAKTURBELICLAIMRETURGRID = "0109";
    public const string MODEL_QUERY_FAKTURBELICLAIMRETURGRID_DETAIL = "0110";
    public const string MODEL_QUERY_FAKTURMANUAL_GRID = "0200";


    public const string MODEL_QUERY_ADJUSTMENT_TRANS_FAKTUR = "0086";
    public const string MODEL_QUERY_ADJUSTMENT_TRANS_FAKTUR_DETIL = "0090";
    public const string MODEL_QUERY_ADJUSTMENT_TRANS_FAKTUR_RETUR = "0087";
    public const string MODEL_QUERY_ADJUSTMENT_TRANS_STT = "0097";
    public const string MODEL_QUERY_ADJUSTMENT_TRANS_STT_DETIL = "0060";
    public const string MODEL_QUERY_ADJUSTMENT_TRANS_VOUCHER = "0120";
    public const string MODEL_QUERY_ADJUSTMENT_TRANS_VOUCHER_DEBIT = "0121";
    public const string MODEL_QUERY_ADJUSTMENT_TRANS_VOUCHER_CREDIT = "0122";
    public const string MODEL_QUERY_ADJUSTMENT_TRANS_VOUCHER_CREDIT_DETIL = "0123";
    public const string MODEL_QUERY_ADJUSTMENT_TRANS_VOUCHER_DEBIT_DETIL = "0124";

    public const string MODEL_QUERY_ADJUSTMENT_TRANS_MEMO = "0127";

    public const string MODEL_QUERY_ADJUSTMENT_TRANS = "0125";
    public const string MODEL_QUERY_ADJUSTMENT_TRANS_DETIL = "0126";

    public const string MODEL_QUERY_MASTER_DISCOUNTGRID = "0111";
    public const string MODEL_QUERY_MASTER_DISCOUNTGRID_DETAIL = "0112";
    public const string MODEL_QUERY_MASTER_DISCOUNTGRID_DETAIL_SPESIFIK = "0113";

    public const string MODEL_QUERY_MASTER_ITEM = "0136";

    public const string MODEL_QUERY_MASTER_BATCH = "0139";
    public const string MODEL_QUERY_MASTER_BATCH_DETAIL = "0140";

    public const string MODEL_QUERY_MASTER_DIVISIPRINSIPAL = "0141";
    public const string MODEL_QUERY_MASTER_CUSTOMER = "0142";
    //public const string MODEL_QUERY_MASTER_DIVISIAMS = "0143";
    //public const string MODEL_QUERY_MASTER_GUDANG = "0144";
    //public const string MODEL_QUERY_MASTER_INDEXDGUDANG = "0145";
    public const string MODEL_QUERY_MASTER_ESTIMASI = "0146";
    public const string MODEL_QUERY_MASTER_COST_ESTIMASI = "0199";
    //public const string MODEL_QUERY_MASTER_ITEMBLOCK = "0147";
    //public const string MODEL_QUERY_MASTER_LIMITPO = "0148";
    public const string MODEL_QUERY_MASTER_EXPEDISI = "0149";
    public const string MODEL_QUERY_MASTER_CABANG_HARI = "0149-a";

    public const string MODEL_QUERY_MASTER_BANK = "0150";
    public const string MODEL_QUERY_MASTER_BANK_REKENING = "0169";
    public const string MODEL_QUERY_MASTER_ESTIMASI_CABANG = "0151";
    public const string MODEL_QUERY_MASTER_PRISIPAL = "0152";
    public const string MODEL_QUERY_MASTER_PRISIPAL_HISTORY_LEADTIME = "0152b";
    public const string MODEL_QUERY_MASTER_COMBO = "0153";
    public const string MODEL_QUERY_MASTER_COMBO_DETAIl = "0154";
    public const string MODEL_QUERY_CEK_USER_APPROVAL = "0155";
    public const string MODEL_QUERY_CEK_STATUS_APPROVAL = "0156";
    public const string MODEL_QUERY_MASTER_USER_APJ = "0302";
    public const string MODEL_QUERY_HISTORY_SP = "0157";

    public const string MODEL_QUERY_MASTER_BUDGET = "0161";
    public const string MODEL_QUERY_MASTER_BUDGET_DETAIL = "0162";
    public const string MODEL_QUERY_MASTER_BLOCKITEM = "0163";
    public const string MODEL_QUERY_MASTER_BLOCKITEM_DETAIL = "0164";

    public const string MODEL_QUERY_MASTER_DIVISIPRINSIPAL_ITEM = "0167";
    public const string MODEL_QUERY_MASTER_DIVISIPRINSIPAL_DETIL = "0168";

    public const string MODEL_QUERY_MASTER_ITEM_CATEGORY = "0170";
    public const string MODEL_QUERY_MASTER_ITEM_CATEGORY_ITEM = "0171";
    public const string MODEL_QUERY_MASTER_ITEM_LANTAI = "0180";
    public const string MODEL_QUERY_MASTER_ITEM_VIA = "0190";
    public const string MODEL_QUERY_MASTER_DIVISI_AMS = "0201";
    public const string MODEL_QUERY_MASTER_DIVISI_AMS_DETIL = "0202";
    public const string MODEL_QUERY_MASTER_DRIVER = "0210";
    public const string MODEL_QUERY_MASTER_PKP_GRID = "0214";
    public const string MODEL_QUERY_MASTER_NOMOR_PAJAK = "0215";

    public const string MODEL_QUERY_DOPRINSIPALGRID = "0300";
    public const string MODEL_QUERY_DOPRINSIPALGRIDDETAIL = "0301";
    public const string MODEL_QUERY_WP_LOGISTIK = "0303";
    public const string MODEL_QUERY_WP_LOGISTIK_GRID = "0304";
    public const string MODEL_QUERY_WP_SERAHTERIMA = "0307";
    public const string MODEL_QUERY_WP_SERAHTERIMA_DETAIL = "0308";
    public const string MODEL_QUERY_WP_SERAHTERIMA_DETAIL_EKSPEDISI = "0309";
    public const string MODEL_QUERY_WP_SERAHTERIMA_SEARCH = "0311";
    public const string MODEL_QUERY_WP_SERAHTERIMA_TRANSPORTASI_DO = "0317";
    public const string MODEL_QUERY_WP_SERAHTERIMA_TRANSPORTASI_SJ = "0318";
    public const string MODEL_QUERY_WP_SERAHTERIMA_TRANSPORTASI_GRIDDETAIL = "0319";

    public const string MODEL_QUERY_INVOICE_RESI_EKSPEDISI_ADD_DETAIL_RESI = "0312";
    public const string MODEL_QUERY_CALC_BIAYA_EKSPEDISI = "0313";
    public const string MODEL_QUERY_FAKTUR_EKSPEDISI_DETAIL = "0314";
    public const string MODEL_QUERY_INVOICE_RESI_EKSPEDISI_ADD_DETAIL_EXP = "0315";
    public const string MODEL_QUERY_INVOICE_EP_EKSPEDISI_DETAIL = "0316";

    public const string MODEL_QUERY_PM = "0320";
    public const string MODEL_QUERY_PM_DETAIL = "0321";
    public const string MODEL_QUERY_WP_SERAHTERIMA_PO = "0340";
    public const string MODEL_QUERY_WP_SERAHTERIMA_PODETAIL = "0341";
    public const string MODEL_QUERY_WP_MONITORING_PO = "0350";
    public const string MODEL_QUERY_WP_MONITORING_PODETAIL = "0351";
    public const string MODEL_QUERY_INV_PENDING_INTEGRITY = "0360";
    public const string MODEL_QUERY_MOVEMENT_STOCK = "0361"; //suwandi 13 Agustus 2019
    public const string MODEL_QUERY_MOVEMENT_STOCK_DETAIL = "0362"; //suwandi 15 Agustus 2019

    #region Indra Monitoring Process 20180523FM

    public const string MODEL_QUERY_WP_MONITORING_PL = "0380";
    public const string MODEL_QUERY_WP_MONITORING_PL_EKSPEDISI = "0380-a";
    public const string MODEL_QUERY_WP_MONITORING_PL_SUMMARY = "0380-b";
    public const string MODEL_QUERY_WP_MONITORING_PL_EKSPEDISI_DETAIL = "0380-c";
    public const string MODEL_QUERY_WP_MONITORING_PL_SUMMARY_V2 = "0380-d";
    public const string MODEL_QUERY_WP_MONITORING_PL_DETAIL_PL = "0380-e";
    public const string MODEL_QUERY_WP_MONITORING_PL_FILTERCABANG = "0380-g";
    public const string MODEL_QUERY_WP_MONITORING_PL_CHART = "0380-h";

    #endregion

    #endregion

    #region Common Query Single

    public const string MODEL_COMMON_QUERY_SINGLE_MSTRANSD = "2001";

    public const string MODEL_COMMON_QUERY_SINGLE_CUSTOMER = "2011";

    public const string MODEL_COMMON_QUERY_SINGLE_CUSTOMER_AND_GUDANG = "2011-a";

	//public const string MODEL_COMMON_QUERY_SINGLE_CUSTOMERDCORE = "2011-b"; //Indra 20180523FM //sudah tidak di pakai ada di bawah

    public const string MODEL_COMMON_QUERY_DUS = "2012";

    public const string MODEL_COMMON_QUERY_SINGLE_SUPLIER = "2021";

    public const string MODEL_COMMON_QUERY_SINGLE_SUPLIER_AUTO = "2021-a";

    public const string MODEL_COMMON_QUERY_SINGLE_GUDANG = "2031";

    public const string MODEL_COMMON_QUERY_SINGLE_GUDANG_FULL = "2032";

    public const string MODEL_COMMON_QUERY_SINGLE_GUDANG_CS = "2033";
            
    public const string MODEL_COMMON_QUERY_SINGLE_MSDIVAMS = "2041";

    public const string MODEL_COMMON_QUERY_SINGLE_MSDIVPRI = "2051";

    public const string MODEL_COMMON_QUERY_SINGLE_ITEM = "2061";

    public const string MODEL_COMMON_QUERY_SINGLE_ITEM_NONPSIKOTROPIKA = "2061-a";

    public const string MODEL_COMMON_QUERY_SINGLE_KURS = "2071";

    public const string MODEL_COMMON_QUERY_SINGLE_KURS_NONSYMBOL = "2072";

    public const string MODEL_COMMON_QUERY_SINGLE_EKSPEDISI = "2081";

    public const string MODEL_COMMON_QUERY_SINGLE_BANK = "2091";

    public const string MODEL_COMMON_QUERY_SINGLE_REKENING = "2101";

    public const string MODEL_COMMON_QUERY_SINGLE_BATCH = "2111";

    public const string MODEL_COMMON_QUERY_SINGLE_KARYAWAN = "2121";

    public const string MODEL_COMMON_QUERY_SINGLE_KOTA = "2131";

    public const string MODEL_COMMON_QUERY_SINGLE_FULLITEM = "2141";

    public const string MODEL_COMMON_QUERY_SINGLE_USERWP = "2151";

    public const string MODEL_COMMON_QUERY_SINGLE_WP_PICKER_DETAIL = "2161";

    public const string MODEL_COMMON_QUERY_SINGLE_WP_PACKER_DETAIL = "2162";

    public const string MODEL_COMMON_QUERY_SINGLE_USER_ST = "2171";

    public const string MODEL_COMMON_QUERY_SINGLE_OUTLET = "2181";

    public const string MODEL_COMMON_QUERY_SINGLE_REASON_RETUR = "2191";

    public const string MODEL_COMMON_QUERY_SINGLE_GROUP = "5001";

    public const string MODEL_COMMON_QUERY_SINGLE_REPORT = "1000001";

    public const string MODEL_COMMON_QUERY_SINGLE_CABANG = "2192";

    public const string MODEL_COMMON_QUERY_SEARCH_CABANG = "2193";

    public const string MODEL_COMMON_QUERY_SINGLE_MSBATCH = "2194"; //Indra 20181019FM

    public const string MODEL_COMMON_QUERY_SINGLE_TRANS_RECALL = "2195"; //suwandi 20181227 

    public const string MODEL_COMMON_QUERY_SINGLE_RECEIVE_NOTE = "2196";
    
    public const string MODEL_COMMON_QUERY_SINGLE_HARD_TAX = "2197";

    #endregion

    #region Common Query Multiple

    #region Right Management

    public const string MODEL_COMMON_QUERY_MULTIPLE_USERRIGHT = "100001";
    public const string MODEL_COMMON_QUERY_MULTIPLE_GROUPRIGHT = "101001";
    public const string MODEL_COMMON_QUERY_MULTIPLE_USERGROUPRIGHT = "101101";
    public const string MODEL_COMMON_QUERY_MULTIPLE_USERGROUPRIGHT_USER = "102001";
    public const string MODEL_COMMON_QUERY_MULTIPLE_USERGROUPRIGHT_USER_AVAIBLE = "102002";
    public const string MODEL_COMMON_QUERY_MULTIPLE_USERGROUPRIGHT_GROUP = "102101";
    public const string MODEL_COMMON_QUERY_MULTIPLE_USERGROUPRIGHT_GROUP_AVAIBLE = "102102";

    #endregion

    #region PL

    public const string MODEL_COMMON_QUERY_MULTIPLE_CUSTOMER_PL = "3001";

    public const string MODEL_COMMON_QUERY_MULTIPLE_SUPLIER_PL = "3101";

    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_PL = "3201";

    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_PL_AUTOGEN = "3202";

    public const string MODEL_COMMON_QUERY_PROCESS_ITEMDETAIL_PL_AUTOGEN = "3204";

    public const string MODEL_COMMON_QUERY_MULTIPLE_SPDETAIL_PL = "3301";

    public const string MODEL_COMMON_QUERY_MULTIPLE_BATCHDETAIL_PL = "3401";

    public const string MODEL_COMMON_QUERY_MULTIPLE_BASPB = "3501";

    public const string MODEL_COMMON_QUERY_NO_PL_PHARMANET = "3502";

    public const string MODEL_COMMON_QUERY_PROSES_AUTO_PL = "0128";

    public const string MODEL_COMMON_QUERY_PROSES_AUTO_SJ = "0128-a";

    public const string MODEL_COMMON_QUERY_MULTIPLE_SPDETAIL_PL_AUTO = "0129";

    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_PL_AUTO = "0130";

    public const string MODEL_COMMON_QUERY_MULTIPLE_BATCH_PL_AUTO = "0131";

    public const string MODEL_COMMON_QUERY_MULTIPLE_RNKHUSUS_PL_AUTO = "0138";

    public const string MODEL_COMMON_QUERY_MULTIPLE_CATEGORI_PL = "4091";

    public const string MODEL_COMMON_QUERY_MULTIPLE_VIA_PL = "4092";

    #endregion

    #region SJ

    public const string MODEL_COMMON_QUERY_MULTIPLE_SUPLIER_SJ = "4001";

    public const string MODEL_COMMON_QUERY_MULTIPLE_SUPLIER_SJ_GUDANG_3 = "4002";

    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_SJ = "4101";

    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_SJ_AUTOGEN = "3203";

    public const string MODEL_COMMON_QUERY_MULTIPLE_SGDETAIL_SJ = "4201";

    public const string MODEL_COMMON_QUERY_MULTIPLE_SUPPLIER_SJ = "1901";

    public const string MODEL_COMMON_QUERY_MULTIPLE_BATCHDETAIL_SJ = "4301";

    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_SJ_REPACK = "4103";

    public const string MODEL_COMMON_QUERY_MULTIPLE_BATCHDETAIL_SJ_REPACK = "4302";

    public const string MODEL_COMMON_QUERY_MULTIPLE_BATCHDETAIL_BAD = "4303";

    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_TF_PEMUSNAHAN = "4304";

    public const string MODEL_COMMON_QUERY_MULTIPLE_INSERT_SJH = "100004";

    public const string MODEL_COMMON_QUERY_SURAT_JALAN_AUTO = "100003";

    public const string MODEL_COMMON_QUERY_LAPORAN_ENAPZA = "100005"; //Indra 20170821

    #endregion

    #region Ekspedisi

    public const string MODEL_COMMON_QUERY_MULTIPLE_CUSTOMER_EKSPEDISI = "5005";
    public const string MODEL_COMMON_QUERY_MULTIPLE_MASTER_EKSPEDISI = "5002";
    public const string MODEL_COMMON_QUERY_MULTIPLE_SUPPLIER_EKSPEDISI = "5003";
    public const string MODEL_COMMON_QUERY_MULTIPLE_DO_EKSPEDISI = "5004";
    public const string MODEL_COMMON_QUERY_MULTIPLE_DRIVER_EKSPEDISI = "5006";
    public const string MODEL_COMMON_QUERY_MULTIPLE_WP_EKSPEDISI = "5007";
    public const string MODEL_COMMON_QUERY_MULTIPLE_DO_EKSPEDISI_SJ = "0213";
    public const string MODEL_COMMON_UPLOADED_QUERY_IMPORTRS_EXPEDISI = "100010101";
    public const string MODEL_COMMON_QUERY_MULTIPLE_RESI_EKSPEDISI = "5008";
    public const string MODEL_COMMON_QUERY_MULTIPLE_NO_EKSPEDISI = "5009";
    public const string MODEL_COMMON_QUERY_MULTIPLE_FAKTUR_EKSPEDISI = "5010";
    public const string MODEL_COMMON_QUERY_MULTIPLE_LIST_EP_EKSTERNAL = "5011";
    public const string MODEL_COMMON_QUERY_MULTIPLE_LIST_EP_INTERNAL = "5012";
    public const string MODEL_COMMON_QUERY_MULTIPLE_LIST_DO_RETURN = "5013";
    public const string MODEL_COMMON_QUERY_MULTIPLE_RS_EKSPEDISI = "5014";


    #endregion

    #region DO

    public const string MODEL_COMMON_QUERY_MULTIPLE_DOPL_NOPL = "6001";
    public const string MODEL_COMMON_QUERY_MULTIPLE_DOSTT_NOSTT_SAMPLE = "6002";
    //public const string MODEL_COMMON_QUERY_MULTIPLE_DOPL_NOPL = "6001";

    #endregion

    #region SP

    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_SP = "7001";

    #endregion

    #region OR

    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_OR = "8001";

    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_OR_SP = "8011";

    public const string MODEL_PROCESS_QUERY_MULTIPLE_PROCESS_ORP = "8091";

    public const string MODEL_PROCESS_QUERY_MULTIPLE_DETIL_INFO_ORP = "8096";

    public const string MODEL_PROCESS_QUERY_MULTIPLE_PROCESS_ORG = "8095";

    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_ORG_SPG = "8021";

    #endregion

    #region STT

    public const string MODEL_COMMON_QUERY_MULTIPLE_STT_MEMO = "9022";

    public const string MODEL_COMMON_QUERY_MULTIPLE_STT_ITEM = "9023";

    public const string MODEL_COMMON_QUERY_MULTIPLE_STT_BATCH = "9024";

    #endregion

    #region Pemusnahan
    public const string MODEL_COMMON_QUERY_MULTIPLE_PM_ITEM = "9031";
    public const string MODEL_COMMON_QUERY_MULTIPLE_PM_BATCH = "9032";
    public const string MODEL_COMMON_QUERY_MULTIPLE_MEMO_PEMUSNAHAN = "9033";
    #endregion
    #region RN

    #region Pembelian

    public const string MODEL_COMMON_QUERY_MULTIPLE_RN_BELI_ITEM = "10001";
    public const string MODEL_COMMON_QUERY_MULTIPLE_RN_BELI_PO = "10011";
    public const string MODEL_COMMON_QUERY_MULTIPLE_RN_BELI_PO_ULUJAMI = "10012";  // HAFIZH ULUJAMI

    #endregion

    #region Khusus

    public const string MODEL_COMMON_QUERY_MULTIPLE_RN_KHUSUS_PRINCIPAL = "10021";

    public const string MODEL_COMMON_QUERY_MULTIPLE_RN_KHUSUS_DO = "10031";

    public const string MODEL_PROCESS_QUERY_MULTIPLE_PROCESS_DOKHUSUS = "10041";

    public const string MODEL_COMMON_QUERY_MULTIPLE_RN_KHUSUS_ITEM = "10141";

    public const string MODEL_COMMON_QUERY_MULTIPLE_RN_KHUSUS_PO = "10241";

    public const string MODEL_COMMON_QUERY_MULTIPLE_RN_KHUSUS_PO_ULUJAMI = "10244";  // HAFIZH ULUJAMI

    #endregion

    #region Transfer Gudang

    public const string MODEL_COMMON_QUERY_MULTIPLE_RN_TRANSFER_SURATJALAN = "10051";

    public const string MODEL_PROCESS_QUERY_MULTIPLE_PROCESS_RNTRANFER = "10061";

    public const string MODEL_COMMON_QUERY_MULTIPLE_LIST_SJ_BASPB = "10062";
    public const string MODEL_COMMON_QUERY_MULTIPLE_LIST_ITEM_SJ_BASPB = "10063";

    #endregion

    #region Claim

    public const string MODEL_COMMON_QUERY_MULTIPLE_RN_ClAIM_ITEM = "10071";

    public const string MODEL_COMMON_QUERY_MULTIPLE_RN_ClAIM_NOCLAIM = "10081";
    public const string MODEL_COMMON_QUERY_MULTIPLE_RN_ClAIM_ITEM_ULUJAMI = "1007103";  // Ulujami Calim
    public const string MODEL_COMMON_QUERY_MULTIPLE_RN_ClAIM_NOCLAIM_ULUJAMI = "1008103"; // Ulujami Calim

    #endregion

    #region Retur

    public const string MODEL_COMMON_QUERY_MULTIPLE_RN_RETUR_ITEM = "10091";

    public const string MODEL_COMMON_QUERY_MULTIPLE_RN_RETUR_RS = "10092";

    public const string MODEL_COMMON_QUERY_MULTIPLE_RN_RETUR_BATCH = "10093";

    #endregion

    #region Repack

    public const string MODEL_COMMON_QUERY_MULTIPLE_RN_REPACK_ITEM = "10094";

    public const string MODEL_COMMON_QUERY_MULTIPLE_RN_REPACK_RS = "10095";

    public const string MODEL_COMMON_QUERY_MULTIPLE_RN_REPACK_BATCH = "10096";

    #endregion

    #region Outstand PO VS RN

    public const string MODEL_COMMON_QUERY_OUTSTANDPO_RN = "10013";
    public const string MODEL_COMMO_QUERY_PO_SP = "10014";

    #endregion

    #endregion

    #region RC

    public const string MODEL_COMMON_QUERY_MULTIPLE_RC_BATCH = "0033";
    public const string MODEL_COMMON_QUERY_MULTIPLE_RC_ITEM = "0034";
    public const string MODEL_COMMON_QUERY_MULTIPLE_RC_DO = "0035";
    public const string MODEL_COMMON_QUERY_MULTIPLE_RC_RN = "0036";
    public const string MODEL_COMMON_QUERY_MULTIPLE_RC_Type = "0037";

    #endregion

    #region RS

    #region Pembelian

    public const string MODEL_COMMON_QUERY_MULTIPLE_RS_PEMBELIAN_ITEM = "0055";
    public const string MODEL_COMMON_QUERY_MULTIPLE_RS_PEMBELIAN_BATCH = "0056";

    #endregion

    #region Repack

    public const string MODEL_COMMON_QUERY_MULTIPLE_RS_REPACK_ITEM = "0057";
    public const string MODEL_COMMON_QUERY_MULTIPLE_RS_REPACK_BATCH = "0058";

    #endregion

    #region Pemb Con

    public const string MODEL_COMMON_QUERY_MULTIPLE_RS_CONF_PEMBELIAN_NORS = "0132";

    public const string MODEL_COMMON_QUERY_MULTIPLE_RS_CONF_PEMBELIAN_NORS_HDR = "0133";

    public const string MODEL_COMMON_QUERY_MULTIPLE_RS_CONF_PEMBELIAN_ITEM_RS = "0134";

    public const string MODEL_COMMON_QUERY_MULTIPLE_RS_CONF_PEMBELIAN_BATCH_RS = "0135";

    #endregion

    #endregion

    #region Adjustment

    #region Stock Good Bad

    public const string MODEL_COMMON_QUERY_MULTIPLE_ADJ_GOODBAD_ITEM = "0076";
    public const string MODEL_COMMON_QUERY_MULTIPLE_ADJ_GOODBAD_BATCH = "0077";
    public const string MODEL_COMMON_QUERY_MULTIPLE_ADJ_BATCH_ALL = "0077-1";

    #endregion

    #region AdjustTransaction

    public const string MODEL_COMMON_QUERY_MULTIPLE_ADJ_NO_PO = "0078";
    public const string MODEL_COMMON_QUERY_MULTIPLE_ADJ_NO_PO_ITEM = "0079";
    public const string MODEL_QUERY_ADJUSTMENT_TRANS_STT_NO = "0098";
    public const string MODEL_QUERY_ADJUSTMENT_TRANS_STT_ITEM = "0099";
    public const string MODEL_QUERY_ADJUSTMENT_TRANS_STT_BATCH = "0059";

    #endregion

    #region Faktur

    public const string MODEL_COMMON_QUERY_MULTIPLE_ADJ_NO_FAKTUR = "0088";
    public const string MODEL_COMMON_QUERY_MULTIPLE_ADJ_NO_FAKTUR_FB = "0089";

    #endregion

    #endregion

    #region Claim

    public const string MODEL_COMMON_QUERY_MULTIPLE_CLAIM_ITEM = "0065";

    public const string MODEL_COMMON_QUERY_MULTIPLE_CLAIM_MSDIVPRI = "0066";
    public const string MODEL_PROCESS_QUERY_MULTIPLE_PROCESS_CLAIM = "0069";

    public const string MODEL_COMMON_QUERY_MULTIPLE_CLAIMACC_NOCLAIM = "0071";

    public const string MODEL_COMMON_QUERY_MULTIPLE_CLAIMACC_ITEM = "0072";

    public const string MODEL_COMMON_QUERY_MULTIPLE_CLAIMACC_CURRENT_ITEM = "0159";

    public const string MODEL_COMMON_QUERY_MULTIPLE_CLAIMACC_CURRENT_ITEM_DETIL = "0160";

    #endregion

    #region WP No Transaksi

    public const string MODEL_COMMON_QUERY_MULTIPLE_WP_TRANSAKSI = "0305";
    public const string MODEL_COMMON_QUERY_MULTIPLE_WP_PICKER = "0306";
    public const string MODEL_COMMON_QUERY_MULTIPLE_WP_CHECKER = "0306a";
    public const string MODEL_COMMON_QUERY_MULTIPLE_WP_PACKER = "0306b";
    public const string MODEL_COMMON_QUERY_MULTIPLE_WP_TRANSPORTASI = "0306c";
    public const string MODEL_COMMON_QUERY_MULTIPLE_WP_INKJET = "0306x";
    public const string MODEL_COMMON_QUERY_MULTIPLE_WP_CHECKER_CekInkJet = "0310";
    public const string MODEL_COMMON_QUERY_MULTIPLE_WP_PO = "0330";

    #endregion

    #region Custom

    public const string MODEL_COMMON_QUERY_MULTIPLE_GUDANGCABANG = "110001";

    #endregion

    #region Reporting

    #region Inventory



    #endregion

    #endregion

    #region MK

    #region Memo Combo

    public const string MODEL_PROCESS_QUERY_MULTIPLE_ITEM_MKMEMOCOMBO = "11001";

    #endregion

    #endregion

    #region Memo

    #region Memo Combo

    public const string MODEL_PROCESS_QUERY_MULTIPLE_MEMO_MEMO_MEMOCOMBO = "12001";

    public const string MODEL_PROCESS_QUERY_MULTIPLE_MEMO_ITEM_MEMOCOMBO = "12101";

    public const string MODEL_PROCESS_QUERY_MULTIPLE_MEMO_PROCESS_ITEM_MEMOCOMBO = "12201";

    public const string MODEL_PROCESS_QUERY_MULTIPLE_MEMO_COMBO_ITEM_MEMOCOMBO = "12301";

    public const string MODEL_PROCESS_QUERY_MULTIPLE_MEMO_COMBO_BATCH_MEMOCOMBO = "12401";

    #endregion

    #endregion

    #region Pembayaran

    public const string MODEL_COMMON_QUERY_MULTIPLE_FAKTUR_SUPLIER = "13001";

    public const string MODEL_COMMON_QUERY_MULTIPLE_FAKTUR_CUSTOMER = "13011";

    #endregion

    #region Faktur

    #region Beli

    public const string MODEL_COMMON_QUERY_MULTIPLE_FB_RN = "14001";

    public const string MODEL_COMMON_QUERY_MULTIPLE_FB_RN_ITEM = "14011";

    public const string MODEL_COMMON_QUERY_MULTIPLE_FB_RN_ITEM_FLOATING = "14012";

    public const string MODEL_COMMON_QUERY_MULTIPLE_FB_SUPPLIER = "14013";

    public const string MODEL_COMMON_QUERY_MULTIPLE_SUPLIER_FAKTURMANUAL = "14014";

    #endregion

    #region Jual Retur

    public const string MODEL_PROCESS_QUERY_MULTIPLE_RETURCUSTOMER = "14101";

    #endregion

    #endregion

    #region Current Stok Multiple

    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_SP_SPG_PENDING = "50001";
    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_SiT_PENDING = "50011";
    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_SOT_PENDING = "50035";
    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_Tot_PENDING = "50037";

    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_PLBOKING_PENDING = "50038";  //PL Boking Hafizh 08 maret 2018

    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_PO_PENDING = "50021";
    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_SPG_PENDING = "50036";

    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_DETAIL_GOOD = "50031";
    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_DETAIL_BAD = "50032";

    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_DETAIL_GOOD_ALLGDG = "50033";
    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_DETAIL_GOOD_CABANG = "50034";


    #endregion

    #region Mater Item Category

    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_CATEGORY = "0173";

    #endregion

    #region Mater Item Lantai

    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_LANTAI = "0174";

    #endregion

    #region Mater Item Via

    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_VIA = "0175";

    #endregion

    #region Transfer Gudang

    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_TFGUDANG_REGULAR = "0176";
    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_TFGUDANG_KARANTINA = "0177";
    public const string MODEL_COMMON_QUERY_MULTIPLE_ITEM_TFGUDANG_PEMUSNAHAN = "0178";

    #endregion

    #region StockOpname Indra 20171231FM

    public const string MODEL_COMMON_QUERY_STOCK_OPNAME_GETDATA = "0257";

    public const string REPORT_PROSES_PRINT_FORM_SO = "0258";

    public const string REPORT_PROSES_PRINT_MONITORING_SO = "0258-b";

    public const string MODEL_COMMON_QUERY_STOCK_OPNAME_MONITORING = "0259";

    #endregion

    #region Indra Monitoring Process 20180523FM

    public const string MODEL_COMMON_QUERY_MONITORINGPL_GETDATA = "0230";

    public const string MODEL_COMMON_QUERY_SINGLE_CUSTOMERDCORE = "2011-b";

    #endregion

    #region Email Produk Kosong 20190411FM

    public const string MODEL_COMMON_QUERY_EMAIL_PRODUKKOSONG = "0231";
    public const string MODEL_COMMON_QUERY_EMAIL_HISTORYPRODUKKOSONG = "0231-b";
    public const string CLASS_NAME_PROSESEMAIL_PRODUKKOSONG = "ProsesEmailProdukKosong";
    public const string REPORT_PROSESEMAIL_PRODUKKOSONG = "0231-c";

    #endregion

    #endregion

    #region Common Query Bridge

    #region RC

    public const string MODEL_COMMON_QUERY_BRIGE_RC = "a-15001";
    public const string MODEL_COMMON_QUERY_BRIGE_EKSPEDISI = "a-15002";
    public const string MODEL_COMMON_QUERY_BRIGE_BASPB = "a-15003";
    public const string MODEL_COMMON_QUERY_BRIGE_DO_PRINCIPAL = "a-15004";

    #endregion

    #endregion

    #region Common Uploaded Query

    #region Import RS

    public const string MODEL_COMMON_UPLOADED_QUERY_IMPORTRS = "up100";

    #endregion

    #region Import FB

    public const string MODEL_COMMON_UPLOADED_QUERY_IMPORTFB = "up110";

    #endregion

    #region Upload Zip DBF

    public const string MODEL_COMMON_UPLOADED_QUERY_ZIPDBF = "up111";

    #endregion

    #region Upload Excell

    public const string MODEL_COMMON_UPLOADED_QUERY_VOUCHER_EXCELL = "up222";
    public const string MODEL_COMMON_UPLOADED_QUERY_VOUCHER_EXCELL_USER = "up223"; //Indra
    public const string MODEL_COMMON_UPLOADED_QUERY_SEND_BATCH = "up224"; //Indra

    #endregion

    #region Upload SO

    public const string MODEL_COMMON_UPLOADED_QUERY_SO = "up333";

    #endregion

    #endregion

    #region Common Query StoreProcedure

    public const string MODEL_PROCESS_QUERY_SP_BDPRDP = "30001";

    public const string MODEL_PROCESS_QUERY_SP_CLOSINGPO = "30002";

    public const string MODEL_PROCESS_QUERY_SP_CLOSSINGLOG = "30003";

    public const string MODEL_PROCESS_QUERY_SP_CLOSSINGLOG_TRANSAKSI = "30004";

    public const string MODEL_COMMON_QUERY_AVG_SALES = "30005";

    public const string MODEL_COMMON_QUERY_FORECAST = "30006";

    #endregion

    #endregion

    #region TestData

    public const string TEST_DATA = "00010";

    #endregion

    #region Reporting

    #region Master
    //Indra 20180830FM
    public const string REPORT_MS_PRINCIPAL = "10000";
    public const string REPORT_MS_PRINCIPAL_HISTORY_LEADTIME = "11000";

    #endregion

    #region Claim Bonus

    public const string REPORT_TRANSAKSI_CLAIM_BONUS = "20501";

    #endregion

    #region Financial

    public const string REPORT_AP_FAKTUR_PENDING_RINGKASAN = "20401-1-1";
    public const string REPORT_AP_FAKTUR_PENDING_DETILFAKTUR = "20401-1-2";
    public const string REPORT_AP_GL = "20401-2";
    public const string REPORT_AP_LIST_RINGKASAN = "20401-3-1";
    public const string REPORT_AP_LIST_DETILFAKTUR = "20401-3-2";
    public const string REPORT_AP_LIST_DETILTRANSAKSI = "20401-3-3";
    public const string REPORT_AP_LIST_BAYAR_TIPE_A = "20401-4-1";
    public const string REPORT_AP_LIST_BAYAR_TIPE_B = "20401-4-2";
    public const string REPORT_AR_FAKTUR_PENDING_RINGKASAN = "20402-1-1";
    public const string REPORT_AR_FAKTUR_PENDING_DETILFAKTUR = "20402-1-2";
    public const string REPORT_AR_GL = "20402-2";
    public const string REPORT_AR_LIST_RINGKASAN = "20402-3-1";
    public const string REPORT_AR_LIST_DETILFAKTUR = "20402-3-2";
    public const string REPORT_AR_LIST_DETILTRANSAKSI = "20402-3-3";
    public const string REPORT_AR_LIST_BAYAR_TIPE_A = "20402-4-1";
    public const string REPORT_AR_LIST_BAYAR_TIPE_B = "20402-4-2";
    public const string REPORT_AR_SALDO_DEBIT = "20403";
    public const string REPORT_HPP_DIV_AMS_DETIL_CLAIM = "20404-1-1";
    public const string REPORT_HPP_DIV_AMS_DETIL_NON_CLAIM = "20404-1-2";
    public const string REPORT_HPP_DIV_AMS_SUMMARI_CLAIM = "20404-2-1";
    public const string REPORT_HPP_DIV_AMS_SUMMARI_NON_CLAIM = "20404-2-2";
    public const string REPORT_HPP_DIV_AMS_SHORT = "20404-3";
    public const string REPORT_HPP_DIV_PRINS_DETIL_CLAIM = "20405-1-1";
    public const string REPORT_HPP_DIV_PRINS_DETIL_NON_CLAIM = "20405-1-2";
    public const string REPORT_HPP_DIV_PRINS_SUMMARI_CLAIM = "20405-2-1";
    public const string REPORT_HPP_DIV_PRINS_SUMMARI_NON_CLAIM = "20405-2-2";
    public const string REPORT_BEA_DETIL = "20406-1";
    public const string REPORT_BEA_FAKTUR = "20406-2";
    public const string REPORT_BEA_SUMMARI = "20406-3";
    public const string REPORT_PEMABAYARAN_SISA_FAKTUR = "20407";
    public const string REPORT_JATUH_TEMPO = "20408";
    public const string REPORT_CLAIM = "20409";
    public const string REPORT_CLAIM_ACC = "20410";
    public const string REPORT_EFAKTUR_FJ = "20411";
    public const string REPORT_FAKTUR_MANUAL = "20412";
    public const string REPORT_EFAKTUR_FM = "20413";


    #endregion

    #region Inventory

    public const string REPORT_INVENTORY_STOK_GUDANG_BATCH = "10001-a";
    public const string REPORT_INVENTORY_STOK_GUDANG_TOTAL = "10001-b";
    public const string REPORT_INVENTORY_STOK_NASIONAL_BATCH = "10002-a";
    public const string REPORT_INVENTORY_STOK_NASIONAL_TOTAL = "10002-b";
    public const string REPORT_INVENTORY_STOK_KARTU_BARANG_GUDANG = "10003";
    public const string REPORT_INVENTORY_STOK_KARTU_BARANG_NASIONAL_BATCH = "10004-a";
    public const string REPORT_INVENTORY_STOK_KARTU_BARANG_NASIONAL_TOTAL = "10004-b";
    public const string REPORT_INVENTORY_INDEX_STOCK = "10005";
    public const string REPORT_INVENTORY_UMUR_STOCK_DETIL = "10006-a";
    public const string REPORT_INVENTORY_UMUR_STOCK_ITEM = "10006-b";
    public const string REPORT_INVENTORY_UMUR_STOCK_DIV_PRINSIPAL = "10006-c";
    public const string REPORT_INVENTORY_UMUR_STOCK_SUPPLIER = "10006-d";
    public const string REPORT_INVENTORY_STOK_OPNAME = "10009";
    public const string REPORT_INVENTORY_STOK_AKTUAL = "10010-a";
    public const string REPORT_INVENTORY_REPORT_CS = "10006";
    public const string REPORT_INVENTORY_SPPLDO = "100060-a";
    public const string REPORT_INVENTORY_SPPLDO_DETAIL = "100060-b";
    public const string REPORT_INVENTORY_SERVICELEVEL_PO = "10050";  // SERVICE LEVEL PO 22032018 HAFIZH
    public const string REPORT_INVENTORY_SERVICELEVEL_CABANG = "10060";  // SERVICE LEVEL CABANG 14052018 HAFIZH
    public const string REPORT_INVENTORY_SERVICELEVEL_PPIC = "10060-a";
    public const string REPORT_INVENTORY_RECALL = "10070"; 

    //public const string REPORT_FORECAST = "10012";

    public const string REPORT_INVENTORY_REPORT_MONITOTINGED = "10011-a"; //gread
    public const string REPORT_INVENTORY_MONITORINGED = "10011-b"; //report

    public const string REPORT_INVENTORY_MONITORINGEDCTRL = "10011-x"; // gread
    public const string REPORT_INVENTORY_REPORT_MONITOTINGEDCTRL = "10011-y"; //report

    public const string REPORT_INVENTORY_MUTASI_INV = "10007";
    public const string REPORT_INVENTORY_MUTASI_INV_DETAIL = "10007-b"; //20170516 Indra D.
    public const string REPORT_INVENTORY_EXPIRE_BATCH = "10008";
    public const string REPORT_INVENTORY_CURRENTSTOCK_ED = "10010";
    public const string REPORT_MASTER_ITEM = "10030";

    #endregion

    #region ListTransaksi

    public const string REPORT_LIST_TRANSAKSI_PO_DETIL = "21001-a";
    public const string REPORT_LIST_TRANSAKSI_PO_PERPO = "21001-b";
    public const string REPORT_LIST_TRANSAKSI_COMBO = "21002";
    public const string REPORT_LIST_TRANSAKSI_RN = "21003";
    public const string REPORT_LIST_TRANSAKSI_RC = "21004";
    public const string REPORT_LIST_TRANSAKSI_STT = "21005";
    public const string REPORT_LIST_TRANSAKSI_RS = "21006";
    public const string REPORT_LIST_TRANSAKSI_PL = "21007";
    public const string REPORT_LIST_TRANSAKSI_SJ = "21008";
    public const string REPORT_LIST_TRANSAKSI_POK = "21009";
    public const string REPORT_LIST_TRANSAKSI_SPPL_DETIL = "21010-a";
    public const string REPORT_LIST_TRANSAKSI_SPPL_PERITEM = "21010-b";
    public const string REPORT_LIST_TRANSAKSI_SPPL_PER_PER_DIVPRI = "21010-c";
    public const string REPORT_LIST_TRANSAKSI_PENJUALAN = "21011-a";
    public const string REPORT_LIST_TRANSAKSI_RETURNDO = "21012-a"; //Indra 2017080
    public const string REPORT_LIST_TRANSAKSI_RETUR_PENJUALAN = "21011-b";
    public const string REPORT_LIST_TRANSAKSI_NET_PENJUALAN = "21011-c";
    public const string REPORT_LIST_TRANSAKSI_SPEXP = "21012";
    public const string REPORT_LIST_TRANSAKSI_ADJ_STOCK = "21013-1";
    public const string REPORT_LIST_TRANSAKSI_ADJ_SP = "21013-2";
    public const string REPORT_LIST_TRANSAKSI_ADJ_PO = "21013-3";
    public const string REPORT_LIST_TRANSAKSI_ADJ_STT = "21013-4";
    public const string REPORT_LIST_TRANSAKSI_ADJ_COMBO = "21013-5";
    public const string REPORT_LIST_TRANSAKSI_ADJ_FJ = "21013-6";
    public const string REPORT_LIST_TRANSAKSI_ADJ_FB = "21013-7";
    public const string REPORT_LIST_FLOATING = "21014";
    public const string REPORT_LIST_PBF = "21015";
    public const string REPORT_LIST_OKTPREKURSOR_BULANAN_DO = "21016";
    public const string REPORT_LIST_TRANSAKSI_SP = "21017";
    public const string REPORT_LIST_OKTPREKURSOR_BULANAN_PO = "21018";
    public const string REPORT_LIST_TRANSAKSI_PEMBELIAN = "21019";
    public const string REPORT_LIST_ENAPZA = "21020";
    public const string REPORT_LIST_EALKES = "21021";
    public const string REPORT_LIST_CSL = "21022";


    #endregion

    #region History

    public const string REPORT_HISTORY_ASURANSI = "20001";
    public const string REPORT_HISTORY_PO_TRANSAKSI = "20002-1";
    public const string REPORT_HISTORY_PO_LIMIT_DETAIL = "20002-2-1";
    public const string REPORT_HISTORY_PO_LIMIT_TOTAL = "20002-2-2";
    public const string REPORT_HISTORY_SURATPESANAN = "20003";
    public const string REPORT_HISTORY_SURATPESANANGUDANG = "20004";
    public const string REPORT_HISTORY_SURATPESANANBATAL = "20005";
    public const string REPORT_HISTORY_STT = "20006";
    public const string REPORT_HISTORY_ITEMBLOCK = "20007";
    public const string REPORT_HISTORY_COMBO = "20008";
    public const string REPORT_HISTORY_ITEMBATCH_GUDANG = "20009-1";
    public const string REPORT_HISTORY_ITEMBATCH_NASIONAL = "20009-2";
    public const string REPORT_HISTORY_EXPEDISI = "20010";
    public const string REPORT_HISTORY_QUERY_CLAIM = "20011";
    public const string REPORT_HISTORY_QUERY_CLAIM_PER_SUPPLIER = "20011-1";
    public const string REPORT_HISTORY_QUERY_CLAIM_PER_DIV_AMS = "20011-2";
    public const string REPORT_HISTORY_QUERY_FJ = "20012-1";
    public const string REPORT_HISTORY_QUERY_FJR = "20012-2";
    public const string REPORT_HISTORY_QUERY_FB = "20013-1";
    public const string REPORT_HISTORY_QUERY_FBR = "20013-2";
    public const string REPORT_HISTORY_QUERY_SALES_RETUR_SUM = "20014-1";
    public const string REPORT_HISTORY_QUERY_SALES_RETUR_CABANG = "20014-2";
    public const string REPORT_HISTORY_QUERY_SALES_RETUR_FAKTUR = "20014-3";
    public const string REPORT_HISTORY_QUERY_PURCHASE_SUM = "20015-1";
    public const string REPORT_HISTORY_QUERY_PURCHASE_CABANG = "20015-2";
    public const string REPORT_HISTORY_QUERY_PURCHASE_FAKTUR = "20015-3";
    public const string REPORT_HISTORY_QUERY_SALES_NONRETUR_SUM = "20014-4";
    public const string REPORT_HISTORY_QUERY_SALES_NONRETUR_CABANG = "20014-5";
    public const string REPORT_HISTORY_QUERY_SALES_NONRETUR_FAKTUR = "20014-6";
    public const string REPORT_HISTORY_QUERY_RETUR_PURCHASE_SUM = "20017-1";
    public const string REPORT_HISTORY_QUERY_RETUR_PURCHASE_CABANG = "20017-2";
    public const string REPORT_HISTORY_QUERY_RETUR_PURCHASE_FAKTUR = "20017-3";
    public const string REPORT_HISTORY_QUERY_SALDO_DEBIT = "20018-1";
    public const string REPORT_HISTORY_QUERY_SALDO_DEBIT_SUM = "20018-2";
    public const string REPORT_HISTORY_QUERY_SALDO_KREDIT = "20019-1";
    public const string REPORT_HISTORY_QUERY_SALDO_KREDIT_SUM = "20019-2";
    public const string REPORT_HISTORY_RESI_EKSPEDISI = "20020";
    public const string REPORT_HISTORY_RESI_EKSPEDISI_DO = "20021-1";
    public const string REPORT_HISTORY_RESI_EKSPEDISI_SJ = "20021-2";
    public const string REPORT_HISTORY_SHIPMENT = "20022";
    public const string REPORT_HISTORY_BIAYA_EKSPEDISI = "20023";
    public const string REPORT_HISTORY_INVOICEVSRESI = "20024";
    public const string REPORT_HISTORY_REKAP_INVOICE = "20025";
    public const string REPORT_HISTORY_LIST_BAYAR_EKSPEDISI = "20026";
    public const string REPORT_HISTORY_PO_PENDING = "20027";
    public const string REPORT_HISTORY_RN_CABANG = "20028";
    public const string REPORT_HISTORY_PEMUSNAHAN = "20029";
    public const string REPORT_HISTORY_RETURSUPPLIER = "20030";
    public const string REPORT_HISTORY_SERVICELEVEL_PO = "20031"; // SERVICE LEVEL PO HAFIZH
    public const string REPORT_HISTORY_SERVICELEVEL_PROSESCABANG = "20033"; // SERVICE LEVEL CABANG PROSES
    public const string REPORT_HISTORY_SERVICELEVEL_REPORT_CABANG = "2003345"; // SERVICE LEVEL CABANG REPORT CABANG
    public const string REPORT_HISTORY_RECALL_PROSESCABANG = "20036"; // recall
    public const string REPORT_HISTORY_RECALL_REPORTGENERATE = "2003346";

    #endregion

    #region Monitoring

    public const string REPORT_MONITORING_DATA_PL = "21051-1";
    public const string REPORT_MONITORING_DATA_CONF = "21051-2";
    public const string REPORT_MONITORING_DATA_BOOKED = "21051-3";
    public const string REPORT_MONITORING_SEND_DO = "21052-1";
    public const string REPORT_MONITORING_SEND_RC = "21052-2";
    public const string REPORT_MONITORING_SALES_NASIONAL = "21053";
 
    public const string REPORT_PRODUKTIFITAS_DC = "21054";

    #endregion

    #region Konsolidasi

    public const string REPORT_KONSOLIDASI_BDP = "20101";
    public const string REPORT_KONSOLIDASI_RDP = "20102";
    public const string REPORT_KONSOLIDASI_STOKNASIONAL = "20103-1";
    public const string REPORT_KONSOLIDASI_STOKNASIONAL_NONDIVPRI = "20103-2";
    public const string REPORT_KONSOLIDASI_BDP_PHAROS = "20104";

    #endregion

    #region Waktu Pelayanan

    public const string REPORT_WAKTUPELAYANAN_CABANG_DETAIL = "20201-1";
    public const string REPORT_WAKTUPELAYANAN_CABANG_SUM_DIVPRIN = "20201-2";
    public const string REPORT_WAKTUPELAYANAN_CABANG_SUM_CABANG = "20201-3";
    public const string REPORT_WAKTUPELAYANAN_PRINCIPAL_DETAIL = "20202-1";
    public const string REPORT_WAKTUPELAYANAN_PRINCIPAL_SUM_DIVPRIN = "20202-2";
    public const string REPORT_WAKTUPELAYANAN_PRINCIPAL_SUM_CABANG = "20202-3";
    public const string REPORT_WAKTUPELAYANAN_PRINCIPAL_CABANG = "20203";
    public const string REPORT_WAKTUPELAYANAN_SERAH_TERIMA = "20204";
    public const string REPORT_WAKTUPELAYANAN_ANALISA_SPPLCONFIRM_DETAIL = "20205-1";
    public const string REPORT_WAKTUPELAYANAN_ANALISA_SPPLCONFIRM_SUM = "20205-2";
    public const string PROSES_WAKTUPELAYANAN = "20206";
    public const string PROSES_FJR = "20211";
    public const string PROSES_DISC_CLAIM = "30213"; // hafizh


    public const string PROSES_COMPARE_SO = "20207";
    public const string PROSES_GENERATE_SO = "20208";
    public const string REPORT_WAKTUPELAYANAN_SERAH_TERIMA_GUDANG_RETUR = "20209";
    public const string REPORT_WAKTUPELAYANAN_SERAHTERIMA_TIKET = "20210";
    public const string REPORT_PROSES_SERAHTERIMA = "10122";
    public const string REPORT_PROCESS_MONITORING = "10123"; //Indra Process Monitoring 20180523FM

    #endregion

    #region Pending

    public const string REPORT_PENDING_SURATPESANAN_TOTAL = "20301-1";
    public const string REPORT_PENDING_SURATPESANAN_DETAIL_PERITEM = "20301-2";
    public const string REPORT_PENDING_SURATPESANAN_DETAIL_PERSP = "20301-3";
    public const string REPORT_PENDING_SURATPESANAN_DETAIL_PERCUSTOMER = "20301-4";
    public const string REPORT_PENDING_SURATPESANAN_BULANAN = "20301-5";
    public const string REPORT_PENDING_SURATPESANAN_BULANAN_PO = "20301-6";
    public const string REPORT_PENDING_SURATPESANAN_GUDANG = "20302";
    public const string REPORT_PENDING_PURCHASEORDER_LOGISTIK_TYPE_1 = "20303-1";
    public const string REPORT_PENDING_PURCHASEORDER_LOGISTIK_TYPE_2 = "20303-2";
    public const string REPORT_PENDING_PURCHASEORDER_FINANCE_DETAIL = "20304-1";
    public const string REPORT_PENDING_PURCHASEORDER_FINANCE_TOTAL = "20304-2";
    public const string REPORT_PENDING_PURCHASEORDER_VS_SURATPESANAN = "20305";
    public const string REPORT_PENDING_DELIVERYORDER = "20306";
    public const string REPORT_PENDING_RETURCUSTOMER = "20307";
    public const string REPORT_PENDING_RECEIVENOTE = "20308";
    public const string REPORT_PENDING_RETURSUPLIER_SUMMARY = "20309-1";
    public const string REPORT_PENDING_RETURSUPLIER_DETAIL = "20309-2";
    public const string REPORT_PENDING_SURATTANDATERIMA = "20310";
    public const string REPORT_PENDING_PACKINGLIST = "20311";
    public const string REPORT_PENDING_COMBO = "20312";
    public const string REPORT_PENDING_SURATJALAN = "20313";
    public const string REPORT_PENDING_POPERIODIK = "20314";
    public const string REPORT_PENDING_DOBELUMRN = "20315"; //Indra D. 20170312

    #endregion

    #region Packing List

    public const string REPORT_TRANSAKSI_PACKINGLIST = "10101";
    public const string REPORT_TRANSAKSI_PACKINGLIST_AUTO = "10201";
           
    #endregion

    #region Movement Stock

    public const string REPORT_TRANSAKSI_MOVEMENT_STOCK = "10301";

    #endregion

    #region DO PL

    public const string REPORT_TRANSAKSI_DOPL = "10102";
    public const string REPORT_TRANSAKSI_DOPL_PRINT_HO2 = "101021";

    #endregion

    #region Transfer

    public const string REPORT_TRANSAKSI_TransferGudang = "10103";

    #endregion

    #region STT

    public const string REPORT_TRANSAKSI_STT_DONASI = "10104";
    public const string REPORT_TRANSAKSI_STT_SAMPLE = "10104-a";

    #endregion

    #region DOSTT

    public const string REPORT_TRANSAKSI_DOSTT = "10105";

    #endregion

    #region DO PL

    public const string REPORT_TRANSAKSI_EKSPEDISI_SHIPMENT = "10115";
    public const string REPORT_TRANSAKSI_INVOICE_EKSPEDISI_EKSTERNAL = "10116";
    public const string REPORT_TRANSAKSI_INVOICE_EKSPEDISI_INTERNAL = "10117";
    public const string REPORT_TRANSAKSI_FAKTUR_EKSPEDISI = "10118";

    #endregion

    #region PO

    public const string REPORT_TRANSAKSI_PO = "10106";
    public const string REPORT_TRANSAKSI_PO_APOTEKER = "10202";

    #endregion

    #region RS

    public const string REPORT_TRANSAKSI_RSBELI = "10107";
    public const string REPORT_TRANSAKSI_RSBELI_CONF = "10107-a";
    public const string REPORT_TRANSAKSI_RSREPACK = "10108";
    public const string REPORT_TRANSAKSI_RSBELI_UPLOAD = "10107-u";
    public const string REPORT_TRANSAKSI_RSREPACK_UPLOAD = "10108-u";

    #endregion

    #region Combo

    public const string REPORT_TRANSAKSI_COMBO = "10109";

    #endregion

    #region Surat Jalan

    public const string REPORT_TRANSAKSI_SURATJALAN_PESANANGUDANG = "10110-a";
    public const string REPORT_TRANSAKSI_SURATJALAN_TRANSFERGUDANG = "10110-b";
    public const string REPORT_TRANSAKSI_SURATJALAN_CLAIM = "10110-c";

    #endregion

    #region Faktur

    public const string REPORT_TRANSAKSI_FAKTURJUAL = "10111-a";
    public const string REPORT_TRANSAKSI_FAKTURJUALRETUR = "10111-b";

    #endregion
    
    #region Adjustment STT

    public const string REPORT_TRANSAKSI_ADJUSTMENTSTT = "10112";

    #endregion

    #region Surat Pesanan

    public const string REPORT_TRANSAKSI_SURATPESANAN = "10113";
    public const string REPORT_TRANSAKSI_PROSESPHARMANET = "10140"; // pharmanet

    #endregion

    #region Report Serah Terima Retur

    public const string REPORT_SERAH_TERIMA_RETUR = "10124";

    #endregion

    #region Pemusnahan
    public const string REPORT_TRANSAKSI_PEMUSNAHAN = "10114";
    public const string REPORT_MEMO_PEMUSNAHAN = "10119";
    #endregion

    #region BASPB SJ
    public const string REPORT_MEMO_BASPB_SJ = "10120";
    #endregion

    #region DO_PHARMANET

    public const string REPORT_DO_PHARMANET = "10121";

    #endregion

    #endregion

    #region Class Name

    #region Right Management

    public const string CLASS_NAME_RIGHTMANGEMENT_USER = "User";
    public const string CLASS_NAME_RIGHTMANGEMENT_GROUP = "Group";
    public const string CLASS_NAME_RIGHTMANGEMENT_USERGROUPACCESS = "UserGroupAccess";
    public const string CLASS_NAME_RIGHTMANGEMENT_GROUPUSERACCESS = "GroupUserAccess";

    #endregion

    #region Packing List

    public const string CLASS_NAME_PACKINGLIST = "PackingList";
    public const string CLASS_NAME_PACKINGLIST_AUTO = "PackingListAuto";
    public const string CLASS_NAME_PACKINGLIST_MASTERBOX = "PackingListMasterBox";
    public const string CLASS_NAME_PACKINGLIST_AUTOGENERATOR = "PackingListAutoGenerator";
    

    #endregion

    #region Stock Opname

    public const string CLASS_NAME_PROCESSSTOCKOPNAME = "ProcessStockOpname"; 

    public const string CLASS_NAME_STOCKOPNAME = "StockOpname";

    public const string CLASS_NAME_STOCKOPNAME_BUATFORMSO = "StockOpnameBuatFormSO";

    public const string CLASS_NAME_STOCKOPNAME_FORMSOBATAL = "StockOpnameFormSOBatal";

    public const string CLASS_NAME_STOCKOPNAME_CONFIRMSO = "StockOpnameConfirmSO";

    public const string CLASS_NAME_STOCKOPNAME_NEWBATCH = "StockOpnameCreateNewBatch";

    public const string CLASS_NAME_STOCKOPNAME_ADJUST = "StockOpnameAdjust";

    public const string CLASS_NAME_STOCKOPNAME_SOULANG = "StockOpnameSOUlang";

    public const string CLASS_NAME_ADJSTOCKOPNAME = "AdjStockOpname";

    #endregion

    #region DO

    #region DOPL

    public const string CLASS_NAME_DOPL = "DOPL";

    #endregion

    #region DOSTT

    public const string CLASS_NAME_DOSTT = "DOSTT";

    #endregion

    #region DOSEND

    public const string CLASS_NAME_DOSEND = "DOSEND";

    #endregion

    #endregion

    #region Ekspedisi

    public const string CLASS_NAME_Ekspedisi = "Ekspedisi";
    public const string CLASS_NAME_Ekspedisi_Cabang = "EkspedisiCabang";
    public const string CLASS_NAME_Ekspedisi_Cabang_Proses = "EkspedisiCabangProses";

    public const string CLASS_NAME_Return_DO = "ClassReturnDO";

    #endregion

    #region Transfer

    public const string CLASS_NAME_Transfer = "TransferGudang";
    public const string CLASS_NAME_Transfer_Repack = "TransferGudangRepack";

    #endregion

    #region STT

    public const string CLASS_NAME_STT = "STT";

    #endregion

    #region Pemusnahan
    public const string CLASS_NAME_PEMUSNAHAN = "Pemusnahan";
    #endregion

    #region Surat Pesanan

    public const string CLASS_NAME_SURATPESANAN = "SuratPesanan";
    public const string CLASS_NAME_SURATPESANANMANUAL = "SuratPesananManual";
    public const string CLASS_NAME_SURATPESANAN_ADMIN = "SuratPesananAdmin";

    #endregion

    #region Order Request

    public const string CLASS_NAME_ORDERREQUESTPRINCIPAL = "OrderRequestPrincipal";
    public const string CLASS_NAME_ORDERREQUESTPROCESSPRINCIPAL = "OrderRequestProcessPrincipal";
    public const string CLASS_NAME_ORDERREQUESTGUDANG = "OrderRequestGudang";
    public const string CLASS_NAME_ORDERREQUESTPROCESSGUDANG = "OrderRequestProcessGudang";

    #endregion

    #region Purchase Order

    public const string CLASS_NAME_PURCHASE_ORDER = "PurchaseOrder";
    public const string CLASS_NAME_PURCHASE_ORDER_APOTEKER = "PurchaseOrderApoteker";

    public const string CLASS_NAME_PURCHASE_ORDER_LIMIT_ITEM = "LimitPOItem";
    public const string CLASS_NAME_PURCHASE_ORDER_LIMIT_DIVPRI = "LimitPODivPri";
    public const string CLASS_NAME_PURCHASE_ORDER_LIMIT_PRINCIPAL = "LimitPOPrincipal";

    #endregion

    #region Receive Note

    public const string CLASS_NAME_RN_BELI = "RNPembelian";
    public const string CLASS_NAME_RN_KHUSUS = "RNKhusus";
    public const string CLASS_NAME_RN_RETUR = "RNRetur";
    public const string CLASS_NAME_RN_CLAIM = "RNClaim";
    public const string CLASS_NAME_RN_REPACK = "RNRepack";
    public const string CLASS_NAME_RN_TRANSFERGUDANG = "RNTransferGudang";

    #endregion

    #region Adjustment

    public const string CLASS_NAME_ADJ_STOCK_GOODBAD = "ADJGOODBAD";
    public const string CLASS_NAME_ADJ_STOCK_BATCH = "ADJBATCH";
    public const string CLASS_NAME_ADJ_STOCK_STOCK = "ADJSTOCK";
    public const string CLASS_NAME_ADJ_STOCK_TRANS = "ADJTRANS";
    public const string CLASS_NAME_ADJ_FJ = "ADJFJ";
    public const string CLASS_NAME_ADJ_FB = "ADJFB";
    public const string CLASS_NAME_ADJ_STT_DONASI = "ADJSTTDONASI";
    public const string CLASS_NAME_ADJ_VOUCHER = "ADJVOUCHER";

    #endregion

    #region MK

    public const string CLASS_NAME_MK_MEMO_COMBO = "MKMemoCombo";
    public const string CLASS_NAME_MK_MEMO_DONASI = "MKMemoDonasi";
    public const string CLASS_NAME_MK_MEMO_SAMPLE = "MKMemoSample";
    public const string CLASS_NAME_MK_MEMO_PEMUSNAHAN = "MKMemoPemusnahan";
    public const string CLASS_NAME_MK_MEMO_BASPB_SJ = "MemoBASPBSJ";

    #endregion

    #region Memo

    public const string CLASS_NAME_LG_MEMO_COMBO = "LGMemoCombo";
    public const string CLASS_NAME_LG_MEMO_DONASI = "LGMemoDonasi";
    public const string CLASS_NAME_LG_MEMO_SAMPLE = "LGMemoSample";


    #endregion

    #region Waktu Pelayanan

    public const string CLASS_NAME_WAKTU_PELAYANAN = "WaktuPelayanan";
    public const string CLASS_NAME_SERAH_TERIMA = "SerahTerima";
    public const string CLASS_NAME_SERAHTERIMA_TIKET = "SerahTerimaTiket";
    public const string CLASS_NAME_SERAHTERIMA_TIKETPO = "SerahTerimaTiketPO";

    #endregion

    #region RC

    public const string CLASS_NAME_RC = "RC";
    public const string CLASS_NAME_RCIN = "RCIN";
    public const string CLASS_NAME_RCRS = "RCRS";
    public const string CLASS_NAME_PROSESST = "PROSESST";
    public const string CLASS_NAME_SAVE_MOVEMENT_STOCK = "UPDATEST";

    #endregion

    #region RS

    public const string CLASS_NAME_RS_PEMBELIAN = "RSBELI";
    public const string CLASS_NAME_RS_REPACK = "RSREPACK";
    public const string CLASS_NAME_RS_CONF = "RSCONF";
    public const string CLASS_NAME_RSDISPOSISI = "RSDISPOSISI";

    #endregion

    #region Claim

    public const string CLASS_NAME_CLAIM = "CLAIM";
    public const string CLASS_NAME_CLAIM_PROCCESS = "CLAIM_PROCCESS";

    #endregion

    #region Claim ACC

    public const string CLASS_NAME_CLAIM_ACC = "CLAIMACC";

    #endregion

    #region Pembayaran

    public const string CLASS_NAME_VOUCHER_DEBIT = "VoucherDebit";
    public const string CLASS_NAME_VOUCHER_CREDIT = "VoucherCredit";

    #endregion

    #region Faktur

    public const string CLASS_NAME_FAKTUR_JUAL = "FakturJual";
    public const string CLASS_NAME_FAKTUR_BELI = "FakturBeli";
    public const string CLASS_NAME_FAKTUR_JUAL_RETUR = "FakturJualRetur";
    public const string CLASS_NAME_FAKTUR_BELI_RETUR = "FakturBeliRetur";
    public const string CLASS_NAME_FAKTUR_MANUAL = "FakturManual";
 
    public const string CLASS_NAME_Invoice_Ekspedisi_Eksternal = "InvoiceEkspedisiEksternal";
    public const string CLASS_NAME_Invoice_Ekspedisi_Internal = "InvoiceEkspedisiInternal";
    public const string CLASS_NAME_Faktur_Ekspedisi = "FakturEkspedisi";

    #endregion

    #region Master

    public const string CLASS_NAME_MASTER_DISCOUNT = "MasterDiscount";

    public const string CLASS_NAME_MASTER_ITEM = "MasterItem";

    public const string CLASS_NAME_MASTER_BATCH = "MasterBatch";

    public const string CLASS_NAME_MASTER_BUDGET = "MasterBudget";

    public const string CLASS_NAME_MASTER_GUDANG = "MasterGudang";

    public const string CLASS_NAME_MASTER_KURS = "MasterKurs";

    public const string CLASS_NAME_MASTER_PRINSIPAL = "MasterPrisipal";

    public const string CLASS_NAME_MASTER_DIVISI_PRINSIPAL = "MasterDivisiPrisipal";

    public const string CLASS_NAME_MASTER_DIVISI_PRINSIPAL_ITEM = "MasterDivisiPrisipalItem";

    public const string CLASS_NAME_MASTER_PELANGGAN = "MasterPelanngan";

    public const string CLASS_NAME_APPROVE_REJECT_MASTER_PELANGGAN = "ApproveRejectMasterPelanggan";

    public const string CLASS_NAME_MASTER_EXPEDISI = "MasterExpedisi";
      //
    public const string CLASS_NAME_MASTER_CABANG_HARI = "MasterCabangHari";

    public const string CLASS_NAME_MASTER_EXPEDISI_ESTIMASI = "MasterExpedisiEstimasi";

    public const string CLASS_NAME_MASTER_EXPEDISI_BIAYA_ESTIMASI = "MasterExpedisiBiaya";

    public const string CLASS_NAME_MASTER_BLOCK_ITEM = "MasterBlockItem";

    public const string CLASS_NAME_MASTER_BANK = "MasterBank";

    public const string CLASS_NAME_MASTER_COMBO = "MasterCombo";

    public const string CLASS_NAME_MASTER_DIV_AMS = "MasterDivAMS";

    public const string CLASS_NAME_MASTER_TRANSAKSI = "MasterTransaksi";

    public const string CLASS_NAME_MASTER_ITEM_CATEGORY = "MasterItemCategory";

    public const string CLASS_NAME_MASTER_ITEM_LANTAI = "MasterItemLantai";

    public const string CLASS_NAME_MASTER_USER_APJ = "MasterUserApj";

    public const string CLASS_NAME_MASTER_ITEM_VIA = "MasterItemVia";

    public const string CLASS_NAME_MASTER_DRIVER = "MasterDriver";

    public const string CLASS_NAME_MASTER_PKP = "MasterPKP";

    public const string CLASS_NAME_MASTER_NOMOR_PAJAK = "MasterNomorPajak";

    #endregion

    #region Syncrone Data

    public const string CLASS_NAME_RN_CABANG = "SyncRNCabang";
    public const string CLASS_NAME_RS_CABANG = "SyncRSCabang";
    public const string CLASS_NAME_MASTER_ITEM_CABANG = "SyncMasterItem";
    public const string CLASS_NAME_SURAT_PESANAN_CABANG = "SyncSuratPesanan";
    public const string CLASS_NAME_RECALL = "SyncRecall";
    public const string CLASS_NAME_RELOKASI = "SyncRelokasi";
    public const string CLASS_NAME_TERIMA_RELOKASI = "SyncReceiveRelokasi";
    public const string CLASS_NAME_CANCEL_PB_RELOKASI = "SyncCancelPBRelocation";
    public const string CLASS_NAME_MASTER_RELOKASI = "SyncMasterRelocation";
    public const string CLASS_NAME_CANCEL_SP_RELOKASI = "SyncCancelSPRelocation";
    public const string CLASS_NAME_RECEIVE_PO = "SyncReceivePO";

    #endregion

    #region Auto

    public const string CLASS_AUTO_DOPRINSIPAL_MANUAL = "DOPrinsipal";
    public const string CLASS_AUTO_DO_PHARMANET = "ProsesPharmanet";
    public const string CLASS_VERIFIKASI_PHARMANET = "VerifikasiPharmanet";
    public const string CLASS_CANCEL_PHARMANET = "CancelPharmanet";
    public const string CLASS_SAVE_BATCH = "savebatch";

    #endregion

    #region Reporting

    #region Reporting Download

    public const string CLASS_NAME_REPORT_MODIFY_DOWNLOAD = "ReportDownload";

    #endregion

    public const string CLASS_NAME_REPORTING = "Reporting";

    #endregion

    #endregion
  }
}