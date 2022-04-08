using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsModel.Core;
using ScmsModel;
using Ext.Net;
using System.Globalization;
using System.Data.Linq.SqlClient;
using ScmsSoaLibraryInterface.Commons;
//using ScmsSoaLibrary.Core.Crypto;
using System.Data.SqlClient;

namespace ScmsSoaLibrary.Modules
{
  class CommonQueryProcess
  {
    #region Internal Class

    internal class TemporaryProcessOR
    {
      public string c_iteno { get; set; }
      public string v_itnam { get; set; }
      public string c_nosup { get; set; }
      public decimal n_avgsls { get; set; }
      public decimal n_index { get; set; }
      public decimal n_soh { get; set; }
      public decimal n_sit { get; set; }
      public decimal n_bo { get; set; }
      public decimal n_spacc { get; set; }
      public decimal n_box { get; set; }
      public decimal n_salpri { get; set; }
      public decimal n_pminord { get; set; }
      public decimal n_qminord { get; set; }
      public decimal n_bonus { get; set; }
      public decimal n_beli { get; set; }
      public string c_via { get; set; }
      public string c_type { get; set; }
      public string c_type_cat { get; set; }
      public string c_kddivpri { get; set; }
      public string v_nmdivpri { get; set; }
      public decimal n_avgslsdivpri { get; set; }
      public decimal n_variabel { get; set; }
      public decimal n_idxp { get; set; }
      public decimal n_idxnp { get; set; }
      public decimal n_pareto { get; set; }
      public decimal n_ideal { get; set; }
      public decimal n_order { get; set; }
      public decimal n_deviasi { get; set; }
      public decimal n_qty { get; set; }
      public string NoID { get; set; }
      public string NoRef { get; set; }
      public decimal Quantity { get; set; }
      public decimal Acceptance { get; set; }
      public decimal QtySisa { get; set; }
      public bool l_combo { get; set; }
      public string ItemCombo { get; set; }
      public decimal Total { get; set; }
    }

    internal class TemporaryProsesRN
    {
      public string c_iteno { get; set; }
      public string v_itemdesc { get; set; }
      public string c_refno { get; set; }
      public string c_batch { get; set; }
      public string d_batchexpired { get; set; }
      public string c_type { get; set; }
      public string v_typedesc { get; set; }
      public decimal n_gqty { get; set; }
      public bool l_new { get; set; }

    }

    internal class TemporaryPendingInfo
    {
      public string NoID { get; set; }
      public string NoRef { get; set; }
      public string Item { get; set; }
      public decimal Quantity { get; set; }
      public decimal Acceptance { get; set; }
      public decimal QtySisa { get; set; }
      public string ItemCombo { get; set; }
    }

    //internal class SJ_FRMT_HEADER
    //{
    //  public bool headerExist { get; set; }
    //  public bool detailExist { get; set; }
    //  public char c_gdg { get; set; }
    //  public string c_refno { get; set; }
    //  public DateTime? d_ref { get; set; }
    //  public string c_type { get; set; }
    //  public string c_addtno { get; set; }
    //  public DateTime? d_addtdate { get; set; }
    //  public string v_ket { get; set; }
    //  public string c_nosup { get; set; }
    //  public bool l_float { get; set; }
    //  public decimal n_bea { get; set; }
    //  public bool l_print { get; set; }
    //  public bool l_status { get; set; }
    //  //public string user {get;set;}
    //  public string c_iteno { get; set; }
    //  public string c_batch { get; set; }
    //  public decimal n_acc { get; set; }
    //  public decimal n_bqty { get; set; }
    //  public decimal n_gqty { get; set; }
    //  public string v_item_desc { get; set; }
    //  public string v_gdgdesc { get; set; }
    //}

    internal class SJ_RN_FRMT_DATA
    {
      public char c_gdg { get; set; }
      public string c_iteno { get; set; }
      public string v_item_desc { get; set; }
      public string c_batch { get; set; }
      public decimal n_gqty { get; set; }
      public decimal n_bqty { get; set; }
      public string c_refno { get; set; }
      public string c_addtno { get; set; }
    }

    internal class Temporary_COMBOITEM_PACKAGE
    {
      public string c_iteno { get; set; }
      public decimal n_package { get; set; }
    }

    internal class Temporary_COMBOITEM_AUTO
    {
      public string c_iteno { get; set; }
      public string v_itemdesc { get; set; }
      public string c_batch { get; set; }
      //public string receiveNote { get; set; }
      public decimal n_qty { get; set; }
      public decimal n_qty_expected { get; set; }
      public DateTime? d_expired { get; set; }
      public decimal n_pack { get; set; }
    }

    internal class TempSales
    {
      public string c_nosup { get; set; }
      public string v_nama { get; set; }
      public string c_iteno { get; set; }
      public string v_itnam { get; set; }
      public decimal n_salpri { get; set; }
      public decimal qty { get; set; }
      public decimal gret { get; set; }
      public decimal bret { get; set; }

    }

    internal class Temporary_JualRetur
    {
      public string c_cusno { get; set; }
      public DateTime d_rcdate { get; set; }
      public string c_exno { get; set; }
      public string c_norc { get; set; }
      public string c_nodo { get; set; }
      public string c_iteno { get; set; }
      public string v_itnam { get; set; }
      //public string c_type { get; set; }
      //public decimal n_salpri { get; set; }
      //public decimal n_disc { get; set; }
      //public decimal n_qty { get; set; }
      public bool l_cab { get; set; }
    }

    internal class TempSPOKT
    {
      public string c_iteno { get; set; }
      public decimal n_sisa { get; set; }
    }

    internal class TempNonSPOKT
    {
      public string c_iteno { get; set; }
      public decimal n_sisa { get; set; }
    }

    internal class TempStokLogic
    {
      public string RefNo { get; set; }
      public string Item { get; set; }
      public decimal GQty { get; set; }
      public decimal BQty { get; set; }

        //
      public decimal total_good { get; set; }
      public decimal total_bad { get; set; }
        //

    }
    internal class TempStokLogicED
    {
      public string c_iteno { get; set; }
      public string v_itnam { get; set; }
      public string ItemUndes { get; set; }
      
      public string ItemPrice { get; set; }
      public bool IsAktif { get; set; }
      public bool IsHide { get; set; }

      public string c_nosup { get; set; }
      public string v_nama { get; set; }
      public string valueGood { get; set; }
      public string valueBad { get; set; }

      public string Goodvalue1 { get; set; }
      public string Goodvalue4 { get; set; }
      public string Goodvalue7 { get; set; }
      public string Goodvalue10 { get; set; }
      public string Good_1_3 { get; set; }
      public string Good_4_6 { get; set; }
      public string Good_7_9 { get; set; }
      public string Good_10_12 { get; set; }

      public string Badvalue1 { get; set; }
      public string Badvalue4 { get; set; }
      public string Badvalue7 { get; set; }
      public string Badvalue10 { get; set; }
      public string Bad_1_3 { get; set; }
      public string Bad_4_6 { get; set; }
      public string Bad_7_9 { get; set; }
      public string Bad_10_12 { get; set; }
      public string EDGOOD { get; set; }
      public string EDBAD { get; set; }
    }
    internal class TempStokLogicExpired
    {
        public string c_iteno { get; set; }
        public string v_itnam { get; set; }
        public string ItemUndes { get; set; }

        public string ItemPrice { get; set; }
        public bool IsAktif { get; set; }
        public bool IsHide { get; set; }

        public string c_nosup { get; set; }
        public string v_nama { get; set; }

        public string c_batch { get; set; }
        public string d_expired { get; set; }
        public string n_salpri { get; set; }
        public string n_gsisa { get; set; }
        public string valueGood { get; set; }
        public string n_bsisa { get; set; }
        public string valueBad { get; set; }

    }


    internal class TempStokLogicFull
    {
        public string Item { get; set; }
        public string ItemName { get; set; }
        public string ItemUndes { get; set; }
        public decimal SOH_GOOD { get; set; }
        public decimal SOH_BAD { get; set; }
        public decimal ORD_QTY { get; set; }
        public decimal SIT_QTY { get; set; }
        public decimal SOT_QTY { get; set; }
        public decimal SOT_QTY_GOOD { get; set; }
        public decimal SOT_QTY_BAD { get; set; }
        public decimal qREQ_QTY { get; set; }
        public decimal SPG_QTY { get; set; }
        //public string ItemPrice { get; set; }
        public bool IsAktif { get; set; }
        public bool IsHide { get; set; }

        public decimal Total_Stock_1 { get; set; }
        public decimal Total_Stock_2 { get; set; }
        public string ItemPrice { get; set; }
        //
        public decimal total_good { get; set; }
        public decimal total_bad { get; set; }
        //
        public string KodeDatsup { get; set; }
        public string NamaDatsup { get; set; }
        public string KodeDivAms { get; set; }
        public string NamaDivAms { get; set; }
        public string KodeDivPrin { get; set; }
        public string NamaDivPrin { get; set; }
        public decimal PLBoking { get; set; }
    }


    internal class TempStokLogicFull_2
    {
        public string Item { get; set; }
        public string ItemName { get; set; }
        public string ItemUndes { get; set; }
        public decimal SOH_GOOD { get; set; }
        public decimal SOH_BAD { get; set; }
        public decimal ORD_QTY { get; set; }
        public decimal SIT_QTY { get; set; }
        public decimal SOT_QTY { get; set; }
        public decimal SOT_QTY_GOOD { get; set; }
        public decimal SOT_QTY_BAD { get; set; }
        public decimal SOT_QTY_ALL { get; set; }
        public decimal qREQ_QTY { get; set; }
        public decimal SPG_QTY { get; set; }
        public decimal Total_Stock_1 { get; set; }
        public decimal Total_Stock_2 { get; set; }
        //public string ItemPrice { get; set; }
        public bool IsAktif { get; set; }
        public bool IsHide { get; set; }
        public string ItemPrice { get; set; }
        //
        public decimal total_good { get; set; }
        public decimal total_bad { get; set; }
        //
        public string KodeDatsup { get; set; }
        public string NamaDatsup { get; set; }
        public string KodeDivAms { get; set; }
        public string NamaDivAms { get; set; }
        public string KodeDivPrin { get; set; }
        public string NamaDivPrin { get; set; }
        public decimal PLBoking { get; set; }
    }


    internal class TempStokLogicFull_3
    {
        public string Item { get; set; }
        public string ItemName { get; set; }
        public string ItemUndes { get; set; }
        public decimal SOH_GOOD { get; set; }
        public decimal SOH_BAD { get; set; }
        public decimal ORD_QTY { get; set; }
        public decimal SIT_QTY { get; set; }
        public decimal SOT_QTY { get; set; }
        public decimal SOT_QTY_ALL { get; set; }
        public decimal qREQ_QTY { get; set; }
        public decimal SPG_QTY { get; set; }

        public decimal Total_Stock_All { get; set; }

        public bool IsAktif { get; set; }
        public bool IsHide { get; set; }
        public string ItemPrice { get; set; }
        public decimal total_good { get; set; }
        public decimal total_bad { get; set; }
        public string KodeDatsup { get; set; }
        public string NamaDatsup { get; set; }
        public string KodeDivAms { get; set; }
        public string NamaDivAms { get; set; }
        public string KodeDivPrin { get; set; }
        public string NamaDivPrin { get; set; }
        public decimal PLBoking { get; set; }
    }

    internal class PL_AUTO_RN
    {
        public char c_gdg { get; set; }
        public string c_rnno { get; set; }
        public string c_iteno { get; set; }
        public string v_itnam { get; set; }
        public string c_batch { get; set; }
        public decimal qtyRN { get; set; }
        public string c_noref { get; set; }
    }

    internal class SJ_AUTO_RN
    {
        public char c_gdg { get; set; }
        public string c_rnno { get; set; }
        public string c_iteno { get; set; }
        public string v_itnam { get; set; }
        public string c_batch { get; set; }
        public decimal n_gqty { get; set; }
        public decimal n_bqty { get; set; }
        public decimal n_booked { get; set; }
        public decimal n_booked_bad { get; set; }
        public bool l_modified { get; set; }
        public bool l_view { get; set; }
        public bool l_new { get; set; }
    }

    internal class PL_AUTO_SP
    {
      public string c_sp { get; set; }
      public string c_spno { get; set; }
      public DateTime d_spdate { get; set; }
      public string c_iteno { get; set; }
      public string c_typesp { get; set; }
      public decimal qtySP { get; set; }
    }

    internal class TempProsesPLAuto
    {
      public char c_gdg { get; set; }
      public string c_rnno { get; set; }
      public string c_iteno { get; set; }
      public string v_itnam { get; set; }
      public string c_batch { get; set; }
      public decimal n_qtyrn { get; set; }
      public string c_sp { get; set; }
      public string c_spno { get; set; }
      public string c_typesp { get; set; }
      public decimal n_qtysp { get; set; }
      public decimal n_qtysp_adj { get; set; }
      public bool l_new { get; set; }
    }

    internal class CurrentStockItemFilter
    {
        public string Item { get; set; }
        public string ItemName { get; set; }
        public string ItemUndes { get; set; }
        public string ItemPrice { get; set; }
        public bool IsAktif { get; set; }
        public bool IsHide { get; set; }
        public string KodeDatsup { get; set; }
        public string NamaDatsup { get; set; }
        public string KodeDivAms { get; set; }
        public string NamaDivAms { get; set; }
        public string KodeDivPrin { get; set; }
        public string NamaDivPrin { get; set; }
    }

    internal class CurrentStockMasterED
    {
        public string c_iteno { get; set; }
        public string v_itnam { get; set; }
        public string ItemUndes { get; set; }
        public string ItemPrice { get; set; }
       
        public string c_nosup { get; set; }
        public string v_nama { get; set; }
        public string valueGood { get; set; }
        public string valueBad { get; set; }


        public string Good_1_3 { get; set; }
        public string Good_4_6 { get; set; }
        public string Good_7_9 { get; set; }
        public string Good_10_12 { get; set; }

        public string Goodvalue_1 { get; set; }
        public string Goodvalue__4 { get; set; }
        public string Goodvalue_7 { get; set; }
        public string Goodvalue_10 { get; set; }


        public string Bad_1_3 { get; set; }
        public string Bad_4_6 { get; set; }
        public string Bad_7_9 { get; set; }
        public string Bad_10_12 { get; set; }

        public string Badvalue_1 { get; set; }
        public string Badvalue__4 { get; set; }
        public string Badvalue_7 { get; set; }
        public string Badvalue_10 { get; set; }
        public string EDGOOD { get; set; }
        public string EDBAD { get; set; }
    }
    internal class CurrentStockMasterExpired
    {
        public string c_iteno { get; set; }
        public string v_itnam { get; set; }
        public string ItemUndes { get; set; }
        public string ItemPrice { get; set; }

        public string c_nosup { get; set; }
        public string v_nama { get; set; }

        public string c_batch { get; set; }
        public string d_expired { get; set; }
        public string n_salpri { get; set; }
        public string n_gsisa { get; set; }
        public string valueGood { get; set; }
        public string n_bsisa { get; set; }
        public string valueBad { get; set; }
    }

        

    //    internal class CurrentStockMasterED
    //{
    //    public string c_iteno { get; set; }
    //    public string v_itnam { get; set; }
    //    public string ItemUndes { get; set; }
    //    public string ItemPrice { get; set; }
       
    //    public string c_nosup { get; set; }
    //    public string v_nama { get; set; }
    //    public string valueGood { get; set; }
    //    public string valueBad { get; set; }


    //    public string Good_1_3 { get; set; }
    //    public string Good_4_6 { get; set; }
    //    public string Good_7_9 { get; set; }
    //    public string Good_10_12 { get; set; }

    //    public string Goodvalue_1 { get; set; }
    //    public string Goodvalue__4 { get; set; }
    //    public string Goodvalue_7 { get; set; }
    //    public string Goodvalue_10 { get; set; }


    //    public string Bad_1_3 { get; set; }
    //    public string Bad_4_6 { get; set; }
    //    public string Bad_7_9 { get; set; }
    //    public string Bad_10_12 { get; set; }

    //    public string Badvalue_1 { get; set; }
    //    public string Badvalue__4 { get; set; }
    //    public string Badvalue_7 { get; set; }
    //    public string Badvalue_10 { get; set; }


    //}
    

    internal class ProgressCallingSPResult<T>
    {
      public string ID { get; set; }
      public T Result { get; set; }
    }
    
    internal class PLDetailGenerator
    {
      public string c_spno { get; set; }
      public string c_sp { get; set; }
      public DateTime? d_spdate { get; set; }
      public string c_iteno { get; set; }
      public string v_itemdesc { get; set; }
      public string v_undes { get; set; }
      public decimal n_sisa { get; set; }
      public string c_batch { get; set; }
      public bool isMaster { get; set; }
      public decimal n_box { get; set; }
      public bool l_expired { get; set; }
      public DateTime d_expired { get; set; }
      public DateTime? d_etdsp { get; set; } //Indra 20181115FM ETD First
    }

    internal class ItemStockAvaible
    {
      public string c_iteno { get; set; }
      public string c_batch { get; set; }
      public decimal n_gsisa { get; set; }
      public decimal n_bsisa { get; set; }
      public DateTime d_expired { get; set; }
    }

    internal class LG_TRANSAKSI
    {
      public char c_gdg { get; set; }
      public string c_no { get; set; }
      public string c_iteno { get; set; }
      public string c_noref { get; set; }
      public string c_batch { get; set; }
      public string c_type { get; set; }
      public decimal n_gqty { get; set; }
      public decimal n_bqty { get; set; }
      public decimal n_gawal { get; set; }
      public decimal n_bawal { get; set; }
    }

    internal class LG_STOCKTMP
    {
      public short s_tahun { get; set; }
      public byte t_bulan { get; set; }
      public char c_gdg { get; set; }
      public string c_rnno { get; set; }
      public string c_iteno { get; set; }
      public string c_noref { get; set; }
      public string c_batch { get; set; }
      public string c_type { get; set; }
      public decimal n_gqty { get; set; }
      public decimal n_bqty { get; set; }
      public decimal n_gawal { get; set; }
      public decimal n_bawal { get; set; }
    }

    internal class LG_Claim
    {
      public string c_nosup;
      public string v_cunam;
      public string c_cusno;
      public string c_iteno;
      public string v_itnam;
      public decimal n_salpri;
      public decimal qty;
      public decimal gret;
      public decimal bret;
      public decimal neto;
    }

    //Indra 20170821
    internal class RPT_Enapza
    {
        public string item { get; set; }
        public DateTime tgl { get; set; }
        public decimal saldoawal { get; set; }
        public string c_batch { get; set; }
        public string fakturin { get; set; }
        public string asal { get; set; }
        public decimal jumlahin { get; set; }
        public string batchin { get; set; }
        public string fakturout { get; set; }
        public string tujuan { get; set; }
        public decimal jumlahout { get; set; }
        public string batchout { get; set; }
        public decimal saldoakhir { get; set; }
        public string batchakhir { get; set; }
        public DateTime d_expired { get; set; }

    }

        //
        internal class Cab
        {


            public string c_cab { get; set; }
            public string v_cunam { get; set; }
            public string c_cusno { get; set; }
            public string c_cab_dcore { get; set; }
            public string c_sarana { get; set; }

        }

        internal class PLLat
        {
            public string TypeLat { get; set; }
            public string Item { get; set; }
            public string Ket { get; set; }
        }


        internal class Avg
        {
            public string C_ITENO { get; set; }
            public decimal TOTAL_QTY { get; set; }
            public decimal AVG_QTY { get; set; }
            public decimal AVG_MONTH { get; set; }
            public string YEARMONTH { get; set; }
            public string C_CLASS { get; set; }
        }

        internal class Forecast
        {
            public string c_cab { get; set; }
            public string c_iteno { get; set; }
            public string c_itnam { get; set; }
            public string uom { get; set; }
            public decimal hna { get; set; }
            public decimal n_forecast { get; set; }
            public decimal? forecast_value { get; set; }
            public decimal avg_qty { get; set; }
            public decimal? avg_value { get; set; }
            public decimal sls_to_dt { get; set; }
            public decimal? sls_to_dt_value { get; set; }
            public decimal sls_to_go { get; set; }
            public decimal? sls_to_go_value { get; set; }
            public decimal sls { get; set; }
            public decimal var { get; set; }
            public decimal hit { get; set; }
            public string c_tahun { get; set; }
            public string c_bulan { get; set; }
        }

    #endregion

    public static IDictionary<string, object> ModelGridQuery(string connectionString, int start, int limit, bool allQuery, string sort, string dir, string model, IDictionary<string, Functionals.ParameterParser> parameters)
    {
      IDictionary<string, object> dic = new Dictionary<string, object>();

      ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);

      DateTime date = DateTime.Now,
        dateStart = DateTime.MinValue,
        dateEnd = DateTime.MinValue;

      int nCount = 0;
      string paternLike = null;

      //Dictionary<string, decimal> dicTotalSOH = null;
      List<TemporaryPendingInfo> qryTotalSOH = null;
      List<TemporaryPendingInfo> qryTotalSOHPL = null;
      List<TemporaryPendingInfo> qryTotalSOHCB = null;

      decimal totalSpAcc = 0;

      List<string> lstSPExcl = new List<string>();
      lstSPExcl.Add("04");
      lstSPExcl.Add("05");

      try
      {
        switch (model)
        {
          #region PL

          #region MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_PL_AUTOGEN

          case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_PL_AUTOGEN:
            {
              char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
              string cusno = (parameters.ContainsKey("cusno") ? (string)((Functionals.ParameterParser)parameters["cusno"]).Value : string.Empty);
              string supl = (parameters.ContainsKey("supl") ? (string)((Functionals.ParameterParser)parameters["supl"]).Value : string.Empty);
              string itemCat = (parameters.ContainsKey("itemCat") ? (string)((Functionals.ParameterParser)parameters["itemCat"]).Value : string.Empty);
              string isBox = (parameters.ContainsKey("isbox") ? (string)((Functionals.ParameterParser)parameters["isbox"]).Value : "false");
              string itemLat = (parameters.ContainsKey("itemLat") ? (string)((Functionals.ParameterParser)parameters["itemLat"]).Value : string.Empty);
              string divPri = (parameters.ContainsKey("divPri") ? (string)((Functionals.ParameterParser)parameters["divPri"]).Value : string.Empty);
              //Indra 20181226FM Penambahan filter ETD
              string txPeriode1 = (parameters.ContainsKey("txPeriode1") ? (string)((Functionals.ParameterParser)parameters["txPeriode1"]).Value : string.Empty);
              string txPeriode2 = (parameters.ContainsKey("txPeriode2") ? (string)((Functionals.ParameterParser)parameters["txPeriode2"]).Value : string.Empty);
              int tgl1 = 0, tgl2 = 0;

                //Indra 20181226FM Penambahan filter ETD
              DateTime dtPeriode1 = DateTime.Now.AddMonths(-1).AddDays(-DateTime.Now.Day + 1).AddHours(-DateTime.Now.Hour).AddMinutes(-DateTime.Now.Minute);
              DateTime dtPeriode2 = DateTime.Now;

              if (txPeriode1 != "")
              {
                  dtPeriode1 = Convert.ToDateTime(txPeriode1);
                  dtPeriode2 = Convert.ToDateTime(txPeriode2);
              }

              tgl1 = SqlMethods.DateDiffDay(dtPeriode1, DateTime.Now);
              tgl2 = SqlMethods.DateDiffDay(dtPeriode2, DateTime.Now);
              bool isBoxs = bool.Parse(isBox);

              List<string> lstItems = null;
              List<ItemStockAvaible> listStock = null,
                listStockTemp = null,
                listStockGroupTemp = null;
              List<PLDetailGenerator> listSPTemp = null,
                listSP = null;
              List<PLDetailGenerator> lstPL = new List<PLDetailGenerator>();

              decimal nAvaible = 0;

              ItemStockAvaible isa = null;

              listSPTemp = (from q in db.LG_SPHs
                            join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
                            join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
                            join q3 in db.LG_Cusmas on q.c_cusno equals q3.c_cusno
                            join q4 in db.FA_Divpris on q1.c_iteno equals q4.c_iteno
                            where (q1.n_sisa > 0) && (q.c_cusno == cusno)
                             && (!(from sq in db.LG_ORHs
                                   join sq1 in db.LG_ORD2s on new { sq.c_gdg, sq.c_orno } equals new { sq1.c_gdg, sq1.c_orno }
                                   where (sq.c_type == "02") && (sq.c_gdg == gdg)
                                    && (sq1.c_spno == q.c_spno)
                                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                   select sq1.c_iteno).Contains(q1.c_iteno))

                            && (string.IsNullOrEmpty(itemCat) ? true : (from sq in db.SCMS_MSITEM_CATs
                                 where (sq.c_type == (string.IsNullOrEmpty(itemCat) || (itemCat == "00") ? sq.c_type : itemCat))
                                 select new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory()
                                 {
                                   c_iteno = (string.IsNullOrEmpty(itemCat) || (itemCat == "00") ? q1.c_iteno : sq.c_iteno)
                                 }).Contains(new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory() { c_iteno = q1.c_iteno }))

                            && (string.IsNullOrEmpty(itemLat) ? true : (from sq in db.SCMS_MSITEM_LATs
                                where (sq.c_type_lat == (string.IsNullOrEmpty(itemLat) || (itemLat == "00") ? sq.c_type_lat : itemLat))
                                    && sq.c_gdg == gdg
                                 select new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory()
                                 {
                                   c_iteno = (string.IsNullOrEmpty(itemLat) || (itemCat == "00") ? q1.c_iteno : sq.c_iteno)
                                 }).Contains(new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory() { c_iteno = q1.c_iteno }))
                             && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                             && (string.IsNullOrEmpty(supl) ? true : q2.c_nosup == supl)
                             //&& (q2.c_nosup == supl) //penambahan mark by suwandi 16 jan 2019 karena principal tidak mandatory lagi
                             && (q3.n_days.HasValue ? (SqlMethods.DateDiffDay(q.d_etasp, DateTime.Now.Date) <= q3.n_days) :
                                                      (SqlMethods.DateDiffMonth(q.d_etasp, DateTime.Now.Date) < 2))
                             && (q.c_type != "06")  //exclude sp pharmanet
                             && (string.IsNullOrEmpty(divPri) ? true : q4.c_kddivpri == divPri)
                             && SqlMethods.DateDiffDay(q.d_etasp,DateTime.Now) <= tgl1 && SqlMethods.DateDiffDay(q.d_etasp,DateTime.Now) >= tgl2
                             //Indra 20181226FM Penambahan filter ETD
                            select new PLDetailGenerator()
                            {
                              c_spno = q.c_spno,
                              d_spdate = q.d_spdate,
                              c_sp = q.c_sp,
                              c_iteno = q1.c_iteno,
                              v_itemdesc = q2.v_itnam,
                              n_sisa = (q1.n_sisa.HasValue ? q1.n_sisa.Value : 0),
                              v_undes = q2.v_undes,
                              d_etdsp = q.d_etdsp //Indra 20181115FM ETD First
                            }).Distinct().ToList();

              //Indra 20181115FM ETD First
              listSPTemp = listSPTemp.OrderBy(x => x.d_etdsp).Distinct().ToList();

              if (listSPTemp.Count > 0)
              {
                lstItems = listSPTemp.GroupBy(t => t.c_iteno).Select(t => t.Key.Trim()).ToList();

                #region Old Coded

                //listStock =
                //  (from q0 in
                //     (from q in GlobalQuery.ViewStockLiteContains(db, gdg, lstItems.ToArray())
                //      group q by new { q.c_iteno, q.c_batch } into g
                //      where (g.Sum(t => t.n_gsisa) > 0)
                //      select new ItemStockAvaible()
                //      {
                //        c_iteno = g.Key.c_iteno,
                //        c_batch = (g.Key.c_batch == null ? string.Empty : g.Key.c_batch.Trim()),
                //        n_gsisa = g.Sum(t => t.n_gsisa)
                //      })
                //   group q0 by new { q0.c_iteno, q0.c_batch } into g
                //   //where (g.Max(t => t.n_gsisa) 
                //   select new ItemStockAvaible()
                //   {
                //     c_iteno = g.Key.c_iteno,
                //     c_batch = g.Key.c_batch,
                //     n_gsisa = g.Max(t => t.n_gsisa)
                //   }).ToList();

                #endregion

                //listStockTemp = (from q in GlobalQuery.ViewStockLiteContains(db, gdg, lstItems.ToArray())
                //                 where (q.n_gsisa != 0)
                //                 group q by new { q.c_iteno, q.c_batch, q.d_date } into g
                //                 select new ItemStockAvaible()
                //                 {
                //                   c_iteno = g.Key.c_iteno,
                //                   c_batch = (g.Key.c_batch == null ? string.Empty : g.Key.c_batch.Trim()),
                //                   d_expired = g.Key.d_date,
                //                   n_gsisa = g.Sum(t => t.n_gsisa)
                //                 }).Distinct().ToList();
                var cek = (from q in GlobalQuery.ViewStockLiteContains(db, gdg, lstItems.ToArray())
                           where (q.n_gsisa != 0)
                           group q by new { q.c_iteno, q.c_batch, q.d_date } into g
                           orderby g.Key.d_date ascending
                           select new ItemStockAvaible()
                           {
                               c_iteno = g.Key.c_iteno,
                               c_batch = (g.Key.c_batch == null ? string.Empty : g.Key.c_batch.Trim()),
                               d_expired = g.Key.d_date,
                               n_gsisa = g.Sum(t => t.n_gsisa)
                           }).Distinct().ToList();

                listStockTemp = (from q in GlobalQuery.ViewStockLiteContains(db, gdg, lstItems.ToArray())
                                 where (q.n_gsisa != 0)
                                 group q by new { q.c_iteno, q.c_batch, q.d_date } into g
                                 orderby g.Key.d_date ascending
                                 select new ItemStockAvaible()
                                 {
                                   c_iteno = g.Key.c_iteno,
                                   c_batch = (g.Key.c_batch == null ? string.Empty : g.Key.c_batch.Trim()),
                                   d_expired = g.Key.d_date,
                                   n_gsisa = g.Sum(t => t.n_gsisa)
                                 }).Distinct().ToList();

                lstItems.Clear();

                #region Old Coded

                //listSP = (from q in listSPTemp
                //          join q1 in listStock on q.c_iteno equals q1.c_iteno
                //          select new PLDetailGenerator()
                //          {
                //            c_spno = q.c_spno,
                //            c_sp = q.c_sp,
                //            c_iteno = q.c_iteno,
                //            v_itemdesc = q.v_itemdesc,
                //            n_sisa = (q.n_sisa > q1.n_gsisa ? q1.n_gsisa : q.n_sisa),
                //            c_batch = q1.c_batch
                //          }).ToList();

                #endregion

                #region Recalculate Stock

                listStock = new List<ItemStockAvaible>();

                listStockGroupTemp = listStockTemp.GroupBy(t => t.c_iteno)
                  .Select(x => new ItemStockAvaible()
                  {
                    c_iteno = x.Key,
                    n_gsisa = x.Sum(g => g.n_gsisa)
                  })
                  .Where(y => y.n_gsisa > 0).ToList();

                listStockGroupTemp.ForEach(delegate(ItemStockAvaible isaX)
                {
                  nAvaible = isaX.n_gsisa;

                  //listStockTemp.FindAll
                  listStock.AddRange(
                    listStockTemp.FindAll(delegate(ItemStockAvaible isaY)
                  {
                    if (isaX.c_iteno != isaY.c_iteno)
                    {
                      return false;
                    }
                    else if (nAvaible <= 0)
                    {
                      return false;
                    }
                    else if(isaY.n_gsisa <= 0)
                    {
                      return false;
                    }

                    if (nAvaible >= isaY.n_gsisa)
                    {
                      nAvaible -= isaY.n_gsisa;

                      return true;
                    }
                    else
                    {
                      isaY.n_gsisa = nAvaible;

                      nAvaible = 0;

                      return true;
                    }

                  }).ToArray());

                  listStockTemp.RemoveAll(delegate(ItemStockAvaible isaY)
                  {
                    if (isaX.c_iteno == isaY.c_iteno)
                    {
                      return true;
                    }

                    return false;
                  });
                });

                #endregion

                listSP = new List<PLDetailGenerator>();

                listSPTemp.ForEach(delegate(PLDetailGenerator pldg)
                {
                  while (pldg.n_sisa > 0)
                  {
                    nAvaible = listStock.Where(t => t.c_iteno == pldg.c_iteno)
                      .GroupBy(t => t.c_iteno).Sum(x => x.Sum(y => y.n_gsisa));
                    if (nAvaible <= 0)
                    {
                      break;
                    }

                    isa = listStock.Where(x => pldg.c_iteno.Equals(x.c_iteno, StringComparison.OrdinalIgnoreCase)).Take(1).SingleOrDefault();
                    if (isa == null)
                    {
                      break;
                    }
                    else if (isa.n_gsisa <= 0)
                    {
                      listStock.Remove(isa);
                      break;
                    }

                    nAvaible = (pldg.n_sisa > isa.n_gsisa ? isa.n_gsisa : pldg.n_sisa);

                    if (nAvaible <= 0)
                    {
                      break;
                    }

                    listSP.Add(new PLDetailGenerator()
                    {
                      c_spno = pldg.c_spno,
                      c_sp = pldg.c_sp,
                      d_spdate = pldg.d_spdate,
                      d_etdsp = pldg.d_etdsp, //Indra 20181115FM ETD First
                      c_iteno = pldg.c_iteno,
                      v_itemdesc = pldg.v_itemdesc,
                      c_batch = isa.c_batch,
                      n_sisa = nAvaible,
                      v_undes = pldg.v_undes,
                      l_expired = isa.d_expired < date.AddYears(1) ? true : false,
                      d_expired = isa.d_expired
                      
                    });

                    pldg.n_sisa -= nAvaible;
                    isa.n_gsisa -= nAvaible;

                    if (isa.n_gsisa <= 0)
                    {
                      listStock.Remove(isa);
                    }

                    if (pldg.n_sisa < 0)
                    {
                      break;
                    }
                  }                  
                });

                if (isBoxs)
                {
                  int nILoop = 0,
                    isMaster = 0,
                    isNonMaster = 0;
                   decimal flor = 0,
                     receh = 0;
                  PLDetailGenerator pldgen = new PLDetailGenerator();
                  List<PLDetailGenerator> lstMaster = new List<PLDetailGenerator>();


                  for (nILoop = 0; nILoop < listSP.Count; nILoop++)
                  {
                    pldgen = listSP[nILoop];

                    var itm = (from q in db.FA_MasItms
                               where q.c_iteno == pldgen.c_iteno
                               select new
                               {
                                 boxes = q.n_box.HasValue ? q.n_box.Value : 0
                               }).SingleOrDefault();

                    if (lstMaster.Count > 0)
                    {
                      isMaster = lstMaster.Where(x => x.isMaster == true).Count();
                      isNonMaster = lstMaster.Where(x => x.isMaster == false).Count();
                    }

                    if ((pldgen.n_sisa / itm.boxes) >= 1)
                    {
                      

                      flor = Math.Floor((pldgen.n_sisa / itm.boxes));

                      if (isMaster < limit)
                      {
                        lstMaster.Add(new PLDetailGenerator()
                        {
                          c_spno = pldgen.c_spno,
                          c_sp = pldgen.c_sp,
                          d_spdate = pldgen.d_spdate,
                          c_iteno = pldgen.c_iteno,
                          v_itemdesc = pldgen.v_itemdesc,
                          c_batch = pldgen.c_batch,
                          n_sisa = (flor * itm.boxes),
                          v_undes = pldgen.v_undes,
                          isMaster = true,
                          n_box = itm.boxes,
                          l_expired = pldgen.d_expired < date.AddYears(1) ? true : false,
                          d_expired = pldgen.d_expired
                        });
                      }

                      if (isNonMaster < limit)
                      {
                        if ((pldgen.n_sisa % itm.boxes) > 0)
                        {
                          receh = (pldgen.n_sisa % itm.boxes);

                          lstMaster.Add(new PLDetailGenerator()
                          {
                            c_spno = pldgen.c_spno,
                            c_sp = pldgen.c_sp,
                            d_spdate = pldgen.d_spdate,
                            c_iteno = pldgen.c_iteno,
                            v_itemdesc = pldgen.v_itemdesc,
                            c_batch = pldgen.c_batch,
                            n_sisa = receh,
                            v_undes = pldgen.v_undes,
                            n_box = itm.boxes,
                            isMaster = false,
                            l_expired = pldgen.d_expired < date.AddYears(1) ? true : false,
                            d_expired = pldgen.d_expired
                          });
                        }
                      }
                    }
                    else
                    {
                      if (isNonMaster < limit)
                      {
                        lstMaster.Add(new PLDetailGenerator()
                        {
                          c_spno = pldgen.c_spno,
                          c_sp = pldgen.c_sp,
                          d_spdate = pldgen.d_spdate,
                          c_iteno = pldgen.c_iteno,
                          v_itemdesc = pldgen.v_itemdesc,
                          c_batch = pldgen.c_batch,
                          n_sisa = pldgen.n_sisa,
                          v_undes = pldgen.v_undes,
                          n_box = itm.boxes,
                          isMaster = false,
                          l_expired = pldgen.d_expired < date.AddYears(1) ? true : false,
                          d_expired = pldgen.d_expired
                        });
                      }
                    }
                  }

                  listSP.Clear();
                  listSP.AddRange(lstMaster);
                }
                
                nCount = listSP.Count;
                

                if (nCount > 0)
                {
                  if (limit == -1)
                  {
                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, listSP.ToArray());
                  }
                  else
                  {
                    if (isBoxs)
                    {
                      limit *= 2;

                      dic.Add(Constant.DEFAULT_NAMING_RECORDS, listSP.Take(limit).ToArray());
                    }
                    else
                    {
                      dic.Add(Constant.DEFAULT_NAMING_RECORDS, listSP.Take(limit).ToArray());
                    }
                    
                  }
                }

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                listStockGroupTemp.Clear();
                listStockTemp.Clear();
                listStock.Clear();
                listSP.Clear();
              }
              else
              {
                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);
              }

              dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

              listSPTemp.Clear();
            }
            break;

          #endregion

          #region MODEL_COMMON_QUERY_PROCESS_ITEMDETAIL_PL_AUTOGEN

          case Constant.MODEL_COMMON_QUERY_PROCESS_ITEMDETAIL_PL_AUTOGEN:
            {
              char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
              string cusno = (parameters.ContainsKey("cusno") ? (string)((Functionals.ParameterParser)parameters["cusno"]).Value : string.Empty);
              string supl = (parameters.ContainsKey("supl") ? (string)((Functionals.ParameterParser)parameters["supl"]).Value : string.Empty);
              string isBox = (parameters.ContainsKey("isbox") ? (string)((Functionals.ParameterParser)parameters["isbox"]).Value : "false");
              string cat = (parameters.ContainsKey("cat") ? (string)((Functionals.ParameterParser)parameters["cat"]).Value : string.Empty);
              //Indra 20181226FM Penambahan filter ETD
              string txPeriode1 = (parameters.ContainsKey("txPeriode1") ? (string)((Functionals.ParameterParser)parameters["txPeriode1"]).Value : string.Empty);
              string txPeriode2 = (parameters.ContainsKey("txPeriode2") ? (string)((Functionals.ParameterParser)parameters["txPeriode2"]).Value : string.Empty);

              //Indra 20181226FM Penambahan filter ETD
              DateTime dtPeriode1 = DateTime.Now.AddMonths(-1).AddDays(-DateTime.Now.Day + 1).AddHours(-DateTime.Now.Hour).AddMinutes(-DateTime.Now.Minute);
              DateTime dtPeriode2 = DateTime.Now;
              int tgl1 = 0, tgl2 = 0;

              if (txPeriode1 != "")
              {
                  dtPeriode1 = Convert.ToDateTime(txPeriode1);
                  dtPeriode2 = Convert.ToDateTime(txPeriode2);
              }
              tgl1 = SqlMethods.DateDiffDay(dtPeriode1, DateTime.Now);
              tgl2 = SqlMethods.DateDiffDay(dtPeriode2, DateTime.Now);
              bool isBoxs = bool.Parse(isBox);

              List<string> lstItems = null;
              List<ItemStockAvaible> listStock = null,
                listStockTemp = null,
                listStockGroupTemp = null;
              List<PLDetailGenerator> listSPTemp = null,
                listSP = null;
              List<PLDetailGenerator> lstPL = new List<PLDetailGenerator>();
              listStockTemp = new List<ItemStockAvaible>();

              //if (cat.Equals("99"))
              //{
              //  cat = "06";
              //}

              decimal nAvaible = 0;

              ItemStockAvaible isa = null;

              //listSPTemp = (from q in db.LG_SPHs
              //              join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
              //              join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
              //              join q3 in db.LG_Cusmas on q.c_cusno equals q3.c_cusno
              //              join q4 in db.FA_Divpris on q1.c_iteno equals q4.c_iteno
              //              where SqlMethods.DateDiffDay(q.d_etdsp, DateTime.Now) <= tgl1 && SqlMethods.DateDiffDay(q.d_etdsp, DateTime.Now) >= tgl2 && (q1.n_sisa > 0) && (q.c_cusno == cusno)
              //               //&& (!(from sq in db.LG_ORHs
              //               //      join sq1 in db.LG_ORD2s on new { sq.c_gdg, sq.c_orno } equals new { sq1.c_gdg, sq1.c_orno }
              //               //      where (sq.c_type == "02") //&& (sq.c_gdg == gdg) // indra 20190315 hilangin filter gudang, karena SP pending tetap muncul di DC 3
              //               //      && (sq.c_gdg == (gdg == '6' ? '1' : gdg))   
              //               //      && (sq1.c_spno == q.c_spno)
              //               //       && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
              //               //      select sq1.c_iteno).Contains(q1.c_iteno))
              //               && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
              //               && (string.IsNullOrEmpty(supl) ? true : (q2.c_nosup == supl))
              //               && (string.IsNullOrEmpty(cat) ? true : (from sq in db.SCMS_MSITEM_CATs
              //                   where (sq.c_type == (string.IsNullOrEmpty(cat) || (cat == "00") ? sq.c_type : cat))
              //                       select new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory()
              //                       {
              //                         c_iteno = (string.IsNullOrEmpty(cat) || (cat == "00") ? q1.c_iteno : sq.c_iteno)
              //                       }).Contains(new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory() { c_iteno = q1.c_iteno }))
              //               && (q3.n_days.HasValue ? (SqlMethods.DateDiffDay(q.d_etasp, DateTime.Now.Date) <= q3.n_days) :
              //                                        (SqlMethods.DateDiffMonth(q.d_etasp, DateTime.Now.Date) < 2))
              //               && (q.c_type != "06")
              //               && (!(from pr in db.scms_params where pr.v_var == "PL_Auto" select pr.v_values).Contains(q4.c_kddivpri))
              //               && q2.l_aktif == true
              //               //&& q.d_etdsp >= dtPeriode1 && q.d_etdsp <= dtPeriode2 //Indra 20181226FM Penambahan filter ETD
              //              select new PLDetailGenerator()
              //              {
              //                c_spno = q.c_spno,
              //                d_spdate = q.d_spdate,
              //                c_sp = q.c_sp,
              //                c_iteno = q1.c_iteno,
              //                v_itemdesc = q2.v_itnam,
              //                n_sisa = (q1.n_sisa.HasValue ? q1.n_sisa.Value : 0),
              //                v_undes = q2.v_undes,
              //                d_etdsp = q.d_etdsp //Indra 20181115FM ETD First
              //              }).Distinct().ToList();

              //Indra 20181115FM ETD First

              listSPTemp = (from q in db.VW_SPHPL_ITEM_V2s
                            where SqlMethods.DateDiffDay(q.d_etdsp, DateTime.Now) <= tgl1 && SqlMethods.DateDiffDay(q.d_etdsp, DateTime.Now) >= tgl2 && (q.c_cusno == cusno)
                            && (string.IsNullOrEmpty(supl) ? true : (q.c_nosup == supl))
                            && (q.c_type == (string.IsNullOrEmpty(cat) || (cat == "00") ? q.c_type : cat))
                            && q.c_gdg == gdg
                            select new PLDetailGenerator()
                            {
                                c_spno = q.c_spno,
                                d_spdate = q.d_spdate,
                                c_sp = q.c_sp,
                                c_iteno = q.c_iteno,
                                v_itemdesc = q.v_itnam,
                                n_sisa = (q.n_sisa.HasValue ? q.n_sisa.Value : 0),
                                v_undes = q.v_undes,
                                d_etdsp = Convert.ToDateTime(q.d_etdsp)
                            }
                            ).OrderBy(x => x.d_etdsp).ToList();
              listSPTemp = listSPTemp.OrderBy(x => x.d_etdsp).ToList();

              if (listSPTemp.Count > 0)
              {
                lstItems = listSPTemp.GroupBy(t => t.c_iteno).Select(t => t.Key.Trim()).ToList();

                var qry = (from q in GlobalQuery.ViewStockLiteContains(db, gdg, lstItems.ToArray())
                           where (q.n_gsisa != 0)
                           group q by new { q.c_iteno, q.c_batch, q.d_date } into g
                           orderby g.Key.d_date ascending
                           select new ItemStockAvaible()
                           {
                               c_iteno = g.Key.c_iteno,
                               c_batch = (g.Key.c_batch == null ? string.Empty : g.Key.c_batch.Trim()),
                               d_expired = g.Key.d_date,
                               n_gsisa = g.Sum(t => t.n_gsisa)
                           }).OrderBy(x => x.d_expired).ToList();

                listStockTemp = qry.OrderBy(x => x.d_expired).Distinct().ToList();
                //listStockTemp = (from q in GlobalQuery.ViewStockLiteContains(db, gdg, lstItems.ToArray())
                //                 where (q.n_gsisa != 0)
                //                 group q by new { q.c_iteno, q.c_batch, q.d_date } into g
                //                 orderby g.Key.d_date ascending
                //                 select new ItemStockAvaible()
                //                 {
                //                   c_iteno = g.Key.c_iteno,
                //                   c_batch = (g.Key.c_batch == null ? string.Empty : g.Key.c_batch.Trim()),
                //                   d_expired = g.Key.d_date,
                //                   n_gsisa = g.Sum(t => t.n_gsisa)
                //                 }).AsQueryable().ToList();

                lstItems.Clear();

                #region Recalculate Stock

                listStock = new List<ItemStockAvaible>();

                listStockGroupTemp = listStockTemp.GroupBy(t => t.c_iteno)
                  .Select(x => new ItemStockAvaible()
                  {
                    c_iteno = x.Key,
                    n_gsisa = x.Sum(g => g.n_gsisa)
                  })
                  .Where(y => y.n_gsisa > 0).ToList();

                listStockGroupTemp.ForEach(delegate(ItemStockAvaible isaX)
                {
                  nAvaible = isaX.n_gsisa;

                  //listStockTemp.FindAll
                  listStock.AddRange(
                    listStockTemp.FindAll(delegate(ItemStockAvaible isaY)
                    {
                      if (isaX.c_iteno != isaY.c_iteno)
                      {
                        return false;
                      }
                      else if (nAvaible <= 0)
                      {
                        return false;
                      }
                      else if (isaY.n_gsisa <= 0)
                      {
                        return false;
                      }

                      if (nAvaible >= isaY.n_gsisa)
                      {
                        nAvaible -= isaY.n_gsisa;

                        return true;
                      }
                      else
                      {
                        isaY.n_gsisa = nAvaible;

                        nAvaible = 0;

                        return true;
                      }

                    }).ToArray());
                  listStock = listStock.OrderBy(x => x.d_expired).ToList();
                  listStockTemp.RemoveAll(delegate(ItemStockAvaible isaY)
                  {
                    if (isaX.c_iteno == isaY.c_iteno)
                    {
                      return true;
                    }

                    return false;
                  });
                });

                #endregion

                listSP = new List<PLDetailGenerator>();

                listSPTemp.ForEach(delegate(PLDetailGenerator pldg)
                {
                  while (pldg.n_sisa > 0)
                  {
                    nAvaible = listStock.Where(t => t.c_iteno == pldg.c_iteno)
                      .GroupBy(t => t.c_iteno).Sum(x => x.Sum(y => y.n_gsisa));
                    if (nAvaible <= 0)
                    {
                      break;
                    }

                    isa = listStock.Where(x => pldg.c_iteno.Equals(x.c_iteno, StringComparison.OrdinalIgnoreCase)).Take(1).SingleOrDefault();
                    if (isa == null)
                    {
                      break;
                    }
                    else if (isa.n_gsisa <= 0)
                    {
                      listStock.Remove(isa);
                      break;
                    }

                    nAvaible = (pldg.n_sisa > isa.n_gsisa ? isa.n_gsisa : pldg.n_sisa);

                    if (nAvaible <= 0)
                    {
                      break;
                    }

                    listSP.Add(new PLDetailGenerator()
                    {
                      c_spno = pldg.c_spno,
                      c_sp = pldg.c_sp,
                      d_spdate = pldg.d_spdate,
                      d_etdsp = pldg.d_etdsp, //Indra 20181115FM ETD First
                      c_iteno = pldg.c_iteno,
                      v_itemdesc = pldg.v_itemdesc,
                      c_batch = isa.c_batch,
                      n_sisa = nAvaible,
                      v_undes = pldg.v_undes,
                      l_expired = isa.d_expired < date.AddYears(1) ? true : false,
                      d_expired = isa.d_expired
                    });

                    pldg.n_sisa -= nAvaible;
                    isa.n_gsisa -= nAvaible;

                    if (isa.n_gsisa <= 0)
                    {
                      listStock.Remove(isa);
                    }

                    if (pldg.n_sisa < 0)
                    {
                      break;
                    }
                  }
                });

                if (isBoxs)
                {
                  int nILoop = 0,
                    isMaster = 0,
                    isNonMaster = 0;
                  decimal flor = 0,
                    receh = 0;
                  PLDetailGenerator pldgen = new PLDetailGenerator();
                  List<PLDetailGenerator> lstMaster = new List<PLDetailGenerator>();


                  for (nILoop = 0; nILoop < listSP.Count; nILoop++)
                  {
                    pldgen = listSP[nILoop];

                    var itm = (from q in db.FA_MasItms
                               where q.c_iteno == pldgen.c_iteno
                               select new
                               {
                                 boxes = q.n_box.HasValue ? q.n_box.Value : 0
                               }).SingleOrDefault();

                    if (lstMaster.Count > 0)
                    {
                      isMaster = lstMaster.Where(x => x.isMaster == true).Count();
                      isNonMaster = lstMaster.Where(x => x.isMaster == false).Count();
                    }

                    if ((pldgen.n_sisa / itm.boxes) >= 1)
                    {


                      flor = Math.Floor((pldgen.n_sisa / itm.boxes));

                      if (isMaster < limit)
                      {
                        lstMaster.Add(new PLDetailGenerator()
                        {
                          c_spno = pldgen.c_spno,
                          c_sp = pldgen.c_sp,
                          d_spdate = pldgen.d_spdate,
                          c_iteno = pldgen.c_iteno,
                          v_itemdesc = pldgen.v_itemdesc,
                          c_batch = pldgen.c_batch,
                          n_sisa = (flor * itm.boxes),
                          v_undes = pldgen.v_undes,
                          isMaster = true,
                          n_box = itm.boxes,
                          l_expired = pldgen.d_expired < date.AddYears(1) ? true : false,
                          d_expired = pldgen.d_expired
                        });
                      }

                      if (isNonMaster < limit)
                      {
                        if ((pldgen.n_sisa % itm.boxes) > 0)
                        {
                          receh = (pldgen.n_sisa % itm.boxes);

                          lstMaster.Add(new PLDetailGenerator()
                          {
                            c_spno = pldgen.c_spno,
                            c_sp = pldgen.c_sp,
                            d_spdate = pldgen.d_spdate,
                            c_iteno = pldgen.c_iteno,
                            v_itemdesc = pldgen.v_itemdesc,
                            c_batch = pldgen.c_batch,
                            n_sisa = receh,
                            v_undes = pldgen.v_undes,
                            n_box = itm.boxes,
                            isMaster = false,
                            l_expired = pldgen.d_expired < date.AddYears(1) ? true : false,
                            d_expired = pldgen.d_expired
                          });
                        }
                      }
                    }
                    else
                    {
                      if (isNonMaster < limit)
                      {
                        lstMaster.Add(new PLDetailGenerator()
                        {
                          c_spno = pldgen.c_spno,
                          c_sp = pldgen.c_sp,
                          d_spdate = pldgen.d_spdate,
                          c_iteno = pldgen.c_iteno,
                          v_itemdesc = pldgen.v_itemdesc,
                          c_batch = pldgen.c_batch,
                          n_sisa = pldgen.n_sisa,
                          v_undes = pldgen.v_undes,
                          n_box = itm.boxes,
                          isMaster = false,
                          l_expired = pldgen.d_expired < date.AddYears(1) ? true : false,
                          d_expired = pldgen.d_expired
                        });
                      }
                    }
                  }

                  listSP.Clear();
                  listSP.AddRange(lstMaster);
                }

                nCount = listSP.Count;


                if (nCount > 0)
                {
                  if (limit == -1)
                  {
                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, listSP.ToArray());
                  }
                  else
                  {
                    if (isBoxs)
                    {
                      limit *= 2;

                      dic.Add(Constant.DEFAULT_NAMING_RECORDS, listSP.Take(limit).ToArray());
                    }
                    else
                    {
                      dic.Add(Constant.DEFAULT_NAMING_RECORDS, listSP.Take(limit).ToArray());
                    }

                  }
                }

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                listStockGroupTemp.Clear();
                listStockTemp.Clear();
                listStock.Clear();
                listSP.Clear();
              }
              else
              {
                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);
              }

              dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

              listSPTemp.Clear();
            }
            break;

          #endregion

          #endregion

          #region OR

          #region MODEL_COMMON_QUERY_MULTIPLE_PROCESS_ORP

          case Constant.MODEL_PROCESS_QUERY_MULTIPLE_PROCESS_ORP:
            {
              string divPrinc = (parameters.ContainsKey("divSuplier") ? (string)((Functionals.ParameterParser)parameters["divSuplier"]).Value : string.Empty);
              string viaKirim = (parameters.ContainsKey("via") ? (string)((Functionals.ParameterParser)parameters["via"]).Value : string.Empty);
              string tipeProd = (parameters.ContainsKey("typeItem") ? (string)((Functionals.ParameterParser)parameters["typeItem"]).Value : string.Empty);
              char gdg = (parameters.ContainsKey("gudang") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gudang"]).Value) : char.MinValue);
              string supl = (parameters.ContainsKey("suplier") ? (string)((Functionals.ParameterParser)parameters["suplier"]).Value : string.Empty);
              string tipeProcces = (parameters.ContainsKey("tipeProcess") ? (string)((Functionals.ParameterParser)parameters["tipeProcess"]).Value : string.Empty);
              string cust = (parameters.ContainsKey("customer") ? (string)((Functionals.ParameterParser)parameters["customer"]).Value : string.Empty);

              char gdgHO = '1';

              TemporaryPendingInfo tpi = null;
              TemporaryProcessOR tpor = null,
                tporNew = null;

              List<string> listItems = null;
              //List<string> listItemsCombo = null;

              int nLoop = 0;

              List<TemporaryProcessOR> qryTempPOR = null,
                listPOR = null,
                listAddTPI = null;

              //List<TemporaryPendingInfo> qryDataTPI = null,
              //  qrySpPendGdgNonOktCmb = null,
              //  qrySpPendGdgNonOkt = null,
              //  qrySpgPendNonOkt = null,
              //  qryTempTPI = null;

              List<TemporaryPendingInfo> qryDataTPI = null,
                qryTempTPI = null;

              decimal decParetoVar = (from q in db.MsVariables
                                      where q.c_portal == '3' && q.c_type == "01"
                                      select (q.n_value.HasValue ? q.n_value.Value : 0)).Take(1).SingleOrDefault();

              if (tipeProcces.Equals("04"))
              {
                qryTempPOR = (from q in db.FA_MasItms
                              join q1 in db.LG_DatSups on q.c_nosup equals q1.c_nosup
                              join q2 in db.FA_Divpris on q.c_iteno equals q2.c_iteno into q_2
                              from qDP in q_2.DefaultIfEmpty()
                              join q3 in db.FA_MsDivPris on qDP.c_kddivpri equals q3.c_kddivpri into q_3
                              from qMDP in q_3.DefaultIfEmpty()
                              join q4 in db.LG_DatSupAutos on q1.c_nosup equals q4.c_nosup into q_4
                              from qDSA in q_4.DefaultIfEmpty()
                              join q5 in db.LG_MsItmBlocks on q.c_iteno equals q5.c_iteno into q_5
                              from qBlok in q_5.Where(t => ((t.l_status.HasValue ? t.l_status.Value : false) == false)).DefaultIfEmpty()
                              where q.l_aktif == true && q1.l_aktif == true && q.c_nosup == supl
                                && ((q.l_combo.HasValue ? q.l_combo.Value : false) == false)
                                && (string.IsNullOrEmpty(viaKirim) ? viaKirim : q.c_via) == viaKirim
                                && q.c_type == tipeProd
                                && ((string.IsNullOrEmpty(divPrinc) ? divPrinc : qDP.c_kddivpri) == divPrinc)
                                && (qDSA.l_do == true && qDSA.l_exp == true && qDSA.l_or == true && qDSA.l_pl == true && qDSA.l_rn == true)
                              select new TemporaryProcessOR()
                              {
                                c_iteno = q.c_iteno,
                                v_itnam = q.v_itnam,
                                c_nosup = q.c_nosup,
                                n_avgsls = 0,
                                n_index = (q1.n_index.HasValue ? q1.n_index.Value : 0),
                                n_soh = 0,
                                n_sit = 0,
                                n_bo = 0,
                                n_spacc = 0,
                                n_box = (q.n_box.HasValue ? q.n_box.Value : 0),
                                n_salpri = (q.n_salpri.HasValue ? q.n_salpri.Value : 0),
                                n_pminord = (q.n_pminord.HasValue ? q.n_pminord.Value : 0),
                                n_qminord = (q.n_qminord.HasValue ? q.n_qminord.Value : 0),
                                n_bonus = (q.n_bonus.HasValue ? q.n_bonus.Value : 0),
                                n_beli = (q.n_beli.HasValue ? q.n_beli.Value : 0),
                                c_via = (q.c_via == null ? string.Empty : q.c_via),
                                c_type = (q.c_type == null ? string.Empty : q.c_type),
                                c_kddivpri = (qDP.c_kddivpri == null ? string.Empty : qDP.c_kddivpri),
                                v_nmdivpri = qMDP.v_nmdivpri,
                                n_avgslsdivpri = 0,
                                n_variabel = decParetoVar,
                                n_idxp = (qMDP.n_idxp.HasValue ? qMDP.n_idxp.Value : 0),
                                n_idxnp = (qMDP.n_idxnp.HasValue ? qMDP.n_idxnp.Value : 0),
                                n_pareto = 0,
                                n_ideal = 0,
                                n_order = 0,
                                n_deviasi = 0,
                                NoID = string.Empty,
                                NoRef = string.Empty,
                                Quantity = 0,
                                Acceptance = 0,
                                QtySisa = 0,
                                l_combo = (q.l_combo.HasValue ? q.l_combo.Value : false)
                              }).Distinct().ToList();
              }
              else
              {
                qryTempPOR = (from q in db.FA_MasItms
                              join q1 in db.LG_DatSups on q.c_nosup equals q1.c_nosup
                              join q2 in db.FA_Divpris on q.c_iteno equals q2.c_iteno into q_2
                              from qDP in q_2.DefaultIfEmpty()
                              join q3 in db.FA_MsDivPris on qDP.c_kddivpri equals q3.c_kddivpri into q_3
                              from qMDP in q_3.DefaultIfEmpty()
                              join q5 in db.LG_MsItmBlocks on q.c_iteno equals q5.c_iteno into q_5
                              from qBlok in q_5.Where(t => ((t.l_status.HasValue ? t.l_status.Value : false) == false)).DefaultIfEmpty()
                              where (q.l_aktif == true) && ((q.l_hide.HasValue ? q.l_hide.Value : false) == false)
                                && (q.c_nosup == supl) && (q1.l_aktif == true) 
                                && ((q.l_combo.HasValue ? q.l_combo.Value : false) == false)
                                && (string.IsNullOrEmpty(viaKirim) ? viaKirim : q.c_via) == viaKirim
                                && q.c_type == tipeProd
                                && ((string.IsNullOrEmpty(divPrinc) ? divPrinc : qDP.c_kddivpri) == divPrinc)
                              select new TemporaryProcessOR()
                              {
                                c_iteno = q.c_iteno,
                                v_itnam = q.v_itnam,
                                c_nosup = q.c_nosup,
                                n_avgsls = 0,
                                n_index = (q1.n_index.HasValue ? q1.n_index.Value : 0),
                                n_soh = 0,
                                n_sit = 0,
                                n_bo = 0,
                                n_spacc = 0,
                                n_box = (q.n_box.HasValue ? q.n_box.Value : 0),
                                n_salpri = (q.n_salpri.HasValue ? q.n_salpri.Value : 0),
                                n_pminord = (q.n_pminord.HasValue ? q.n_pminord.Value : 0),
                                n_qminord = (q.n_qminord.HasValue ? q.n_qminord.Value : 0),
                                n_bonus = (q.n_bonus.HasValue ? q.n_bonus.Value : 0),
                                n_beli = (q.n_beli.HasValue ? q.n_beli.Value : 0),
                                c_via = (q.c_via == null ? string.Empty : q.c_via),
                                c_type = (q.c_type == null ? string.Empty : q.c_type),
                                c_kddivpri = (qDP.c_kddivpri == null ? string.Empty : qDP.c_kddivpri),
                                v_nmdivpri = qMDP.v_nmdivpri,
                                n_avgslsdivpri = 0,
                                n_variabel = decParetoVar,
                                n_idxp = (qMDP.n_idxp.HasValue ? qMDP.n_idxp.Value : 0),
                                n_idxnp = (qMDP.n_idxnp.HasValue ? qMDP.n_idxnp.Value : 0),
                                n_pareto = 0,
                                n_ideal = 0,
                                n_order = 0,
                                n_deviasi = 0,
                                NoID = string.Empty,
                                NoRef = string.Empty,
                                Quantity = 0,
                                Acceptance = 0,
                                QtySisa = 0,
                                l_combo = (q.l_combo.HasValue ? q.l_combo.Value : false)
                              }).Distinct().ToList();
              }

              listItems = qryTempPOR.Where(x => x.l_combo == false).GroupBy(x => x.c_iteno).Select(y => y.Key).ToList();
              //listItemsCombo = qryTempPOR.Where(x => x.l_combo == true).GroupBy(x => x.c_iteno).Select(x => x.Key).ToList();

              #region Cek Stock On Hand

              //dicTotalSOH = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

              qryTotalSOH = (from q in GlobalQuery.ViewStockLite(db, gdgHO, 1)
                             where //(listItems.Contains(q.c_iteno) || listItemsCombo.Contains(q.c_iteno))
                              listItems.Contains(q.c_iteno)
                              && (q.n_gsisa != 0)
                             group q by q.c_iteno into g
                             select new TemporaryPendingInfo()
                             {
                               Item = g.Key,
                               QtySisa = g.Sum(x => x.n_gsisa)
                             }).Distinct().ToList();

              if ((qryTotalSOH != null) && (qryTotalSOH.Count > 0))
              {
                qryTempPOR.ForEach(delegate(TemporaryProcessOR t)
                {
                  tpi = qryTotalSOH.Find(x => x.Item == t.c_iteno);
                  if (tpi != null)
                  {
                    //if (dicTotalSOH.ContainsKey(t.c_iteno))
                    //{
                    //  nTotalSOH = dicTotalSOH[t.c_iteno];
                    //}
                    //else
                    //{
                    //  nTotalSOH = 0;

                    //  dicTotalSOH.Add(t.c_iteno, 0);
                    //}

                    t.n_soh = tpi.QtySisa;

                    //nTotalSOH += tpi.QtySisa;

                    //dicTotalSOH[t.c_iteno] = nTotalSOH;
                  }
                });

                qryTotalSOH.Clear();
              }

              #endregion

              #region Cek Stock In Transit Gudang 1

              qryTotalSOH = (from q in db.LG_SJHs
                             join q1 in db.LG_SJD2s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                             where listItems.Contains(q1.c_iteno)
                              && q.l_status == false && q1.n_gsisa != 0
                              && q.c_gdg2 == '1'
                              //Production
                              //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                             group q1 by new { q1.c_gdg, q1.c_iteno } into g
                             select new TemporaryPendingInfo()
                             {
                               Item = g.Key.c_iteno,
                               QtySisa = g.Sum(x => (x.n_gsisa.HasValue ? x.n_gsisa.Value : 0))
                             }).Distinct().ToList();

              if ((qryTotalSOH != null) && (qryTotalSOH.Count > 0))
              {
                qryTempPOR.ForEach(delegate(TemporaryProcessOR t)
                {
                  tpi = qryTotalSOH.Find(x => x.Item == t.c_iteno);
                  if (tpi != null)
                  {
                    t.n_sit = tpi.QtySisa;
                  }
                });

                qryTotalSOH.Clear();
              }

              #endregion

              #region Tipe Proses

              listAddTPI = new List<TemporaryProcessOR>();

              qryDataTPI = new List<TemporaryPendingInfo>();

              if (tipeProcces.Equals("02"))
              {
                #region Khusus

                var qryItemsCodesSP = (from q in db.LG_SPHs
                                       join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
                                       join q2 in db.LG_Cusmas on q.c_cusno equals q2.c_cusno
                                       where q1.n_sisa > 0 && (!lstSPExcl.Contains(q.c_type))
                                        //&& (q2.n_days.HasValue ? (SqlMethods.DateDiffDay(q.d_spdate, DateTime.Now.Date) <= q2.n_days) :
                                        //              (SqlMethods.DateDiffMonth(q.d_spdate, DateTime.Now.Date) < 2)) 
                                        && (SqlMethods.DateDiffMonth(q.d_etasp, DateTime.Now.Date) < 2)
                                        && q.c_cusno == cust
                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       select new
                                       {
                                         qSPH = q,
                                         qSPD1 = q1,
                                         qCus = q2
                                       }).AsQueryable();

                #region Non Combo

                if (listItems.Count > 0)
                {
                  qryTempTPI = (from q in qryItemsCodesSP
                                where listItems.Contains(q.qSPD1.c_iteno)
                                  && (!(from q_sq1 in db.LG_ORHs
                                        join q_sq2 in db.LG_ORD2s on new { q_sq1.c_gdg, q_sq1.c_orno } equals new { q_sq2.c_gdg, q_sq2.c_orno }
                                        where (q_sq2.c_spno == q.qSPH.c_spno)
                                          && ((q_sq2.c_itemcombo == null) || (q_sq2.c_itemcombo == "0000") || (q_sq2.c_itemcombo == "")) //(q_sq2.c_itemcombo == null || q_sq2.c_itemcombo == "0000")
                                          && ((q_sq1.l_delete.HasValue ? q_sq1.l_delete.Value : false) == false)
                                        select q_sq2.c_iteno).Contains(q.qSPD1.c_iteno))
                                select new TemporaryPendingInfo()
                                {
                                  NoID = q.qSPH.c_spno,
                                  NoRef = q.qSPH.c_sp.Trim(),
                                  Item = q.qSPD1.c_iteno,
                                  //Acceptance = (q.qSPD1.n_acc.HasValue ? q.qSPD1.n_acc.Value : 0),
                                  //Quantity = (q.qSPD1.n_qty.HasValue ? q.qSPD1.n_qty.Value : 0),
                                  Acceptance = (q.qSPD1.n_sisa.HasValue ? q.qSPD1.n_sisa.Value : 0),
                                  Quantity = (q.qSPD1.n_acc.HasValue ? q.qSPD1.n_acc.Value : 0),
                                  QtySisa = (q.qSPD1.n_sisa.HasValue ? q.qSPD1.n_sisa.Value : 0)
                                }).ToList();

                  //qrySpPendGdgNonOkt = (from q in qryItemsCodesSP
                  //                      join q1 in db.LG_ORD2s on new { q.qSPH.c_spno, q.qSPD1.c_iteno } equals new { q1.c_spno, q1.c_iteno } into q_1
                  //                      from qORD2 in q_1.DefaultIfEmpty()
                  //                      join q2 in db.LG_ORHs on new { qORD2.c_gdg, qORD2.c_orno } equals new { q2.c_gdg, q2.c_orno } into q_2
                  //                      from qORH in q_2.DefaultIfEmpty()
                  //                      where ((qORD2 == null) || (qORH.l_delete.Value == true))
                  //                        && (qORD2.c_itemcombo == null || qORD2.c_itemcombo == "0000")
                  //                        && SqlMethods.DateDiffMonth(q.qSPH.d_spdate, date) < 2
                  //                        && listItems.Contains(q.qSPD1.c_iteno)
                  //                      select new TemporaryPendingInfo()
                  //                      {
                  //                        NoID = q.qSPH.c_spno,
                  //                        NoRef = q.qSPH.c_sp.Trim(),
                  //                        Item = q.qSPD1.c_iteno,
                  //                        Acceptance = q.qSPD1.n_acc,
                  //                        Quantity = q.qSPD1.n_qty,
                  //                        QtySisa = q.qSPD1.n_sisa
                  //                      }).ToList();

                  qryDataTPI.AddRange(qryTempTPI.ToArray());
                  qryTempTPI.Clear();
                }

                #endregion

                #region Combo

                //if (listItemsCombo.Count > 0)
                //{
                //  qryTempTPI = (from q in qryItemsCodesSP
                //                join q1 in db.FA_Combos on q.qSPD1.c_iteno equals q1.c_combo
                //                where listItemsCombo.Contains(q.qSPD1.c_iteno)
                //                  && (!(from q_sq1 in db.LG_ORHs
                //                        join q_sq2 in db.LG_ORD2s on new { q_sq1.c_gdg, q_sq1.c_orno } equals new { q_sq2.c_gdg, q_sq2.c_orno }
                //                        where (q_sq2.c_spno == q.qSPH.c_spno) && (q_sq2.c_itemcombo == q1.c_combo)
                //                         && ((q_sq1.l_delete.HasValue ? q_sq1.l_delete.Value : false) == false)
                //                        select q_sq2.c_iteno).Contains(q.qSPD1.c_iteno))
                //                group new { q, q1 } by new { q.qSPH.c_sp, q.qSPH.c_spno, q1.c_combo, q1.c_iteno } into g
                //                select new TemporaryPendingInfo()
                //                {
                //                  NoID = g.Key.c_spno,
                //                  NoRef = g.Key.c_sp.Trim(),
                //                  Item = g.Key.c_iteno,
                //                  ItemCombo = g.Key.c_combo,
                //                  //Acceptance = g.Sum(x => ((x.q.qSPD1.n_acc.HasValue ? x.q.qSPD1.n_acc.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0))),
                //                  //Quantity = g.Sum(x => ((x.q.qSPD1.n_qty.HasValue ? x.q.qSPD1.n_qty.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0))),
                //                  Acceptance = g.Sum(x => ((x.q.qSPD1.n_sisa.HasValue ? x.q.qSPD1.n_sisa.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0))),
                //                  Quantity = g.Sum(x => ((x.q.qSPD1.n_acc.HasValue ? x.q.qSPD1.n_acc.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0))),
                //                  QtySisa = g.Sum(x => ((x.q.qSPD1.n_sisa.HasValue ? x.q.qSPD1.n_sisa.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0))),
                //                }).ToList();

                //  //qrySpPendGdgNonOktCmb = (from q in qryItemsCodesSP
                //  //                         join q1 in db.FA_Combos on q.qSPD1.c_iteno equals q1.c_combo
                //  //                         join q2 in db.LG_ORD2s on new { q.qSPH.c_spno, q.qSPD1.c_iteno, c_itemcombo = q1.c_combo } equals new { q2.c_spno, q2.c_iteno, q2.c_itemcombo } into q_2
                //  //                         from qORD2 in q_2.DefaultIfEmpty()
                //  //                         join q3 in db.LG_ORHs on new { qORD2.c_gdg, qORD2.c_orno } equals new { q3.c_gdg, q3.c_orno }
                //  //                         where ((q3.l_delete.Value == true) || (qORD2.c_iteno == null))
                //  //                           && SqlMethods.DateDiffMonth(q.qSPH.d_spdate, date) < 2
                //  //                          && listItemsCombo.Contains(q.qSPD1.c_iteno)
                //  //                         group new { q, q1 } by new { q.qSPH.c_sp, q.qSPH.c_spno, q1.c_combo, q1.c_iteno } into g
                //  //                         select new TemporaryPendingInfo()
                //  //                         {
                //  //                           NoID = g.Key.c_spno,
                //  //                           NoRef = g.Key.c_sp.Trim(),
                //  //                           Item = g.Key.c_iteno,
                //  //                           ItemCombo = g.Key.c_combo,
                //  //                           Acceptance = g.Sum(x => (x.q.qSPD1.n_acc * x.q1.n_qty)),
                //  //                           Quantity = g.Sum(x => (x.q.qSPD1.n_qty * x.q1.n_qty)),
                //  //                           QtySisa = g.Sum(x => (x.q.qSPD1.n_sisa * x.q1.n_qty))
                //  //                         }).ToList();

                //  qryDataTPI.AddRange(qryTempTPI.ToArray());
                //  qryTempTPI.Clear();
                //}

                #endregion

                qryDataTPI.ForEach(delegate(TemporaryPendingInfo t)
                {
                  tpor = qryTempPOR.Find(x => x.c_iteno == t.Item && x.NoID == t.NoID);
                  if (tpor == null)
                  {
                    tpor = qryTempPOR.Find(x => x.c_iteno == t.Item);
                    if (tpor != null)
                    {
                      totalSpAcc = qryDataTPI.Where(x => x.Item == t.Item).GroupBy(x => x.Item).Sum(x => x.Sum(y => y.QtySisa));

                      if (string.IsNullOrEmpty(tpor.NoID))
                      {
                        tpor.NoID = t.NoID;
                        tpor.NoRef = t.NoRef;
                        tpor.Quantity = t.Quantity;
                        tpor.Acceptance = t.Acceptance;
                        tpor.QtySisa = t.QtySisa;
                        tpor.n_spacc = totalSpAcc;
                        tpor.ItemCombo = (t.ItemCombo == null ? string.Empty : t.ItemCombo);
                        tpor.l_combo = (string.IsNullOrEmpty(t.ItemCombo) ? false : true);
                      }
                      else
                      {
                        tporNew = new TemporaryProcessOR()
                        {
                          c_iteno = tpor.c_iteno,
                          v_itnam = tpor.v_itnam,
                          c_nosup = tpor.c_nosup,
                          n_avgsls = tpor.n_avgsls,
                          n_index = tpor.n_index,
                          n_soh = tpor.n_soh,
                          n_sit = tpor.n_sit,
                          n_bo = 0,
                          n_spacc = totalSpAcc,
                          n_box = tpor.n_box,
                          n_salpri = tpor.n_salpri,
                          n_pminord = tpor.n_pminord,
                          n_qminord = tpor.n_qminord,
                          n_bonus = tpor.n_bonus,
                          n_beli = tpor.n_beli,
                          c_via = tpor.c_via,
                          c_type = tpor.c_type,
                          c_kddivpri = tpor.c_kddivpri,
                          v_nmdivpri = tpor.v_nmdivpri,
                          n_avgslsdivpri = tpor.n_avgslsdivpri,
                          n_variabel = tpor.n_variabel,
                          n_idxp = tpor.n_idxp,
                          n_idxnp = tpor.n_idxnp,
                          n_pareto = tpor.n_pareto,
                          n_ideal = tpor.n_ideal,
                          n_order = tpor.n_order,
                          n_deviasi = tpor.n_deviasi,
                          NoID = t.NoID,
                          NoRef = t.NoRef,
                          Quantity = t.Quantity,
                          Acceptance = t.Acceptance,
                          QtySisa = t.QtySisa,
                          l_combo = (string.IsNullOrEmpty(t.ItemCombo) ? false : true),
                          ItemCombo = (t.ItemCombo == null ? string.Empty : t.ItemCombo)
                        };

                        listAddTPI.Add(tporNew);
                      }
                    }
                  }
                });

                qryTempPOR.AddRange(listAddTPI.ToArray());

                listAddTPI.Clear();

                #endregion
              }
              else
              {
                if (gdg.Equals(char.MinValue))
                {
                  #region Combine Gudang

                  #region SP Pending

                  var qryItemsCodesSP = (from q in db.LG_SPHs
                                         join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
                                         join q2 in db.LG_Cusmas on q.c_cusno equals q2.c_cusno
                                         where q1.n_sisa > 0 && (!lstSPExcl.Contains(q.c_type)) //&& q.c_type != "04"
                                            && q2.c_gdg == gdgHO
                                            && (q.c_cusno == (string.IsNullOrEmpty(cust) ? q.c_cusno : cust))
                                            //&& (q2.n_days.HasValue ? (SqlMethods.DateDiffDay(q.d_spdate, DateTime.Now.Date) <= q2.n_days) :
                                            //          (SqlMethods.DateDiffMonth(q.d_spdate, DateTime.Now.Date) < 2)) 
                                            && (SqlMethods.DateDiffMonth(q.d_spdate, DateTime.Now.Date) < 2)
                                            && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                         select new
                                         {
                                           qSPH = q,
                                           qSPD1 = q1,
                                           qCus = q2
                                         }).AsQueryable();

                  #region Non Combo

                  if (listItems.Count > 0)
                  {
                    qryTempTPI = (from q in qryItemsCodesSP
                                  where listItems.Contains(q.qSPD1.c_iteno)
                                    && (!(from q_sq1 in db.LG_ORHs
                                          join q_sq2 in db.LG_ORD2s on new { q_sq1.c_gdg, q_sq1.c_orno } equals new { q_sq2.c_gdg, q_sq2.c_orno }
                                          join q_sq3 in db.LG_POD2s on new { q_sq1.c_gdg, q_sq1.c_orno} equals new { q_sq3.c_gdg, q_sq3.c_orno }
                                          where (q_sq2.c_spno == q.qSPH.c_spno)
                                            && ((q_sq2.c_itemcombo == null) || (q_sq2.c_itemcombo == "0000") || (q_sq2.c_itemcombo == "")) //(q_sq2.c_itemcombo == null || q_sq2.c_itemcombo == "0000")
                                            && ((q_sq1.l_delete.HasValue ? q_sq1.l_delete.Value : false) == false)
                                          select q_sq2.c_iteno).Contains(q.qSPD1.c_iteno))
                                  select new TemporaryPendingInfo()
                                  {
                                    NoID = q.qSPH.c_spno,
                                    NoRef = q.qSPH.c_sp.Trim(),
                                    Item = q.qSPD1.c_iteno,
                                    //Acceptance = (q.qSPD1.n_acc.HasValue ? q.qSPD1.n_acc.Value : 0),
                                    //Quantity = (q.qSPD1.n_qty.HasValue ? q.qSPD1.n_qty.Value : 0),
                                    Acceptance = (q.qSPD1.n_sisa.HasValue ? q.qSPD1.n_sisa.Value : 0),
                                    Quantity = (q.qSPD1.n_acc.HasValue ? q.qSPD1.n_acc.Value : 0),
                                    QtySisa = (q.qSPD1.n_sisa.HasValue ? q.qSPD1.n_sisa.Value : 0)
                                  }).ToList();

                    //qrySpPendGdgNonOkt = (from q in qryItemsCodesSP
                    //                      join q1 in db.LG_ORD2s on new { q.qSPH.c_spno, q.qSPD1.c_iteno } equals new { q1.c_spno, q1.c_iteno } into q_2
                    //                      from qORD2 in q_2.DefaultIfEmpty()
                    //                      join q2 in db.LG_ORHs on new { qORD2.c_gdg, qORD2.c_orno } equals new { q2.c_gdg, q2.c_orno }
                    //                      where ((q2.l_delete.Value == true) || (qORD2.c_iteno == null))
                    //                        //&& (qORD2.c_itemcombo == null || qORD2.c_itemcombo == "0000")
                    //                        //&& SqlMethods.DateDiffMonth(q.qSPH.d_spdate, date) < 2
                    //                        && listItems.Contains(q.qSPD1.c_iteno)
                    //                      select new TemporaryPendingInfo()
                    //                      {
                    //                        NoID = q.qSPH.c_spno,
                    //                        NoRef = q.qSPH.c_sp.Trim(),
                    //                        Item = q.qSPD1.c_iteno,
                    //                        Acceptance = q.qSPD1.n_acc,
                    //                        Quantity = q.qSPD1.n_qty,
                    //                        QtySisa = q.qSPD1.n_sisa
                    //                      }).ToList();

                    qryDataTPI.AddRange(qryTempTPI.ToArray());
                    qryTempTPI.Clear();
                  }

                  #endregion

                  #region Combo

                  //if (listItemsCombo.Count > 0)
                  //{
                  //  qryTempTPI = (from q in qryItemsCodesSP
                  //                join q1 in db.FA_Combos on q.qSPD1.c_iteno equals q1.c_combo
                  //                where listItemsCombo.Contains(q.qSPD1.c_iteno)
                  //                  && (!(from q_sq1 in db.LG_ORHs
                  //                        join q_sq2 in db.LG_ORD2s on new { q_sq1.c_gdg, q_sq1.c_orno } equals new { q_sq2.c_gdg, q_sq2.c_orno }
                  //                        where (q_sq2.c_spno == q.qSPH.c_spno) && (q_sq2.c_itemcombo == q1.c_combo)
                  //                         && ((q_sq1.l_delete.HasValue ? q_sq1.l_delete.Value : false) == false)
                  //                        select q_sq2.c_iteno).Contains(q.qSPD1.c_iteno))
                  //                group new { q, q1 } by new { q.qSPH.c_sp, q.qSPH.c_spno, q1.c_combo, q1.c_iteno } into g
                  //                select new TemporaryPendingInfo()
                  //                {
                  //                  NoID = g.Key.c_spno,
                  //                  NoRef = g.Key.c_sp.Trim(),
                  //                  Item = g.Key.c_iteno,
                  //                  ItemCombo = g.Key.c_combo,
                  //                  //Acceptance = g.Sum(x => ((x.q.qSPD1.n_acc.HasValue ? x.q.qSPD1.n_acc.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0))),
                  //                  //Quantity = g.Sum(x => ((x.q.qSPD1.n_qty.HasValue ? x.q.qSPD1.n_qty.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0))),
                  //                  Acceptance = g.Sum(x => ((x.q.qSPD1.n_sisa.HasValue ? x.q.qSPD1.n_sisa.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0))),
                  //                  Quantity = g.Sum(x => ((x.q.qSPD1.n_acc.HasValue ? x.q.qSPD1.n_acc.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0))),
                  //                  QtySisa = g.Sum(x => ((x.q.qSPD1.n_sisa.HasValue ? x.q.qSPD1.n_sisa.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0)))
                  //                }).ToList();

                  //  //qrySpPendGdgNonOktCmb = (from q in qryItemsCodesSP
                  //  //                         join q1 in db.FA_Combos on q.qSPD1.c_iteno equals q1.c_combo
                  //  //                         join q2 in db.LG_ORD2s on new { q.qSPH.c_spno, q.qSPD1.c_iteno, c_itemcombo = q1.c_combo } equals new { q2.c_spno, q2.c_iteno, q2.c_itemcombo } into q_2
                  //  //                         from qORD2 in q_2.DefaultIfEmpty()
                  //  //                         join q3 in db.LG_ORHs on new { qORD2.c_gdg, qORD2.c_orno } equals new { q3.c_gdg, q3.c_orno }
                  //  //                         where ((q3.l_delete.Value == true) || (qORD2.c_iteno == null))
                  //  //                           //&& SqlMethods.DateDiffMonth(q.qSPH.d_spdate, date) < 2
                  //  //                          && listItemsCombo.Contains(q.qSPD1.c_iteno)
                  //  //                         group new { q, q1 } by new { q.qSPH.c_sp, q.qSPH.c_spno, q1.c_combo, q1.c_iteno } into g
                  //  //                         select new TemporaryPendingInfo()
                  //  //                         {
                  //  //                           NoID = g.Key.c_spno,
                  //  //                           NoRef = g.Key.c_sp.Trim(),
                  //  //                           Item = g.Key.c_iteno,
                  //  //                           ItemCombo = g.Key.c_combo,
                  //  //                           Acceptance = g.Sum(x => (x.q.qSPD1.n_acc * x.q1.n_qty)),
                  //  //                           Quantity = g.Sum(x => (x.q.qSPD1.n_qty * x.q1.n_qty)),
                  //  //                           QtySisa = g.Sum(x => (x.q.qSPD1.n_sisa * x.q1.n_qty))
                  //  //                         }).ToList();

                  //  qryDataTPI.AddRange(qryTempTPI.ToArray());
                  //  qryTempTPI.Clear();
                  //}

                  #endregion

                  #endregion

                  #region SPG Pending



                  var qryItemsCodesSPG = (from q in db.LG_SPGHs
                                          join q1 in db.LG_SPGD1s on new { c_gdg = q.c_gdg1, q.c_spgno } equals new { q1.c_gdg, q1.c_spgno }
                                          where (q1.n_sisa > 0) && (q1.c_gdg != gdgHO)
                                            && (q.l_status == true)
                                            && SqlMethods.DateDiffMonth(q.d_spgdate, date) < 2
                                            && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                          select new
                                          {
                                            qSPGH = q,
                                            qSPGD1 = q1
                                          }).AsQueryable();

                  #region Non Combo

                  if (listItems.Count > 0)
                  {

                    #region Old Coded New

                    //qryTempTPI = (from q in qryItemsCodesSPG
                    //              where listItems.Contains(q.qSPGD1.c_iteno)
                    //                && (!(from q_sq1 in db.LG_ORHs
                    //                      join q_sq2 in db.LG_ORD2s on new { q_sq1.c_gdg, q_sq1.c_orno } equals new { q_sq2.c_gdg, q_sq2.c_orno }
                    //                      where (q_sq2.c_spno == q.qSPGH.c_spgno)
                    //                        && ((q_sq2.c_itemcombo == null) || (q_sq2.c_itemcombo == "0000") || (q_sq2.c_itemcombo == ""))
                    //                        && ((q_sq1.l_delete.HasValue ? q_sq1.l_delete.Value : false) == false)
                    //                      select q_sq2.c_iteno).Contains(q.qSPGD1.c_iteno))
                    //              group new { q } by new { q.qSPGD1.c_spgno, q.qSPGD1.c_iteno } into g
                    //              select new TemporaryPendingInfo()
                    //              {
                    //                NoID = g.Key.c_spgno,
                    //                NoRef = g.Key.c_spgno,
                    //                Item = g.Key.c_iteno,
                    //                Acceptance = g.Sum(x => (x.q.qSPGD1.n_sisa.HasValue ? x.q.qSPGD1.n_sisa.Value : 0)),
                    //                Quantity = g.Sum(x => (x.q.qSPGD1.n_sisa.HasValue ? x.q.qSPGD1.n_sisa.Value : 0)),
                    //                QtySisa = g.Sum(x => (x.q.qSPGD1.n_sisa.HasValue ? x.q.qSPGD1.n_sisa.Value : 0))
                    //              }).ToList();

                    #endregion

                    qryTempTPI = (from q in qryItemsCodesSPG
                                  where listItems.Contains(q.qSPGD1.c_iteno)
                                    && (!(from q_sq1 in db.LG_ORHs
                                          join q_sq2 in db.LG_ORD2s on new { q_sq1.c_gdg, q_sq1.c_orno } equals new { q_sq2.c_gdg, q_sq2.c_orno }
                                          where (q_sq2.c_spno == q.qSPGH.c_spgno)
                                            && ((q_sq2.c_itemcombo == null) || (q_sq2.c_itemcombo == "0000") || (q_sq2.c_itemcombo == ""))
                                            && ((q_sq1.l_delete.HasValue ? q_sq1.l_delete.Value : false) == false)
                                          select q_sq2.c_iteno).Contains(q.qSPGD1.c_iteno))
                                  group new { q } by new { q.qSPGD1.c_spgno, q.qSPGD1.c_iteno } into g
                                  select new TemporaryPendingInfo()
                                  {
                                    NoID = g.Key.c_spgno,
                                    NoRef = g.Key.c_spgno,
                                    Item = g.Key.c_iteno,
                                    Acceptance = g.Sum(x => (x.q.qSPGD1.n_sisa.HasValue ? x.q.qSPGD1.n_sisa.Value : 0)),
                                    Quantity = g.Sum(x => (x.q.qSPGD1.n_sisa.HasValue ? x.q.qSPGD1.n_sisa.Value : 0)),
                                    QtySisa = g.Sum(x => (x.q.qSPGD1.n_sisa.HasValue ? x.q.qSPGD1.n_sisa.Value : 0))
                                  }).ToList();

                    //qrySpgPendNonOkt = (from q in qryItemsCodesSPG
                    //                    join q2 in db.LG_ORD2s on new { c_spno = q.qSPGD1.c_spgno, q.qSPGD1.c_iteno } equals new { q2.c_spno, q2.c_iteno } into q_2
                    //                    from qORD2 in q_2.DefaultIfEmpty()
                    //                    where qORD2.c_iteno == null
                    //                      && q.qSPGH.l_status == true && q.qSPGD1.n_sisa > 0
                    //                      && listItems.Contains(q.qSPGD1.c_iteno)
                    //                    group new { q } by new { q.qSPGD1.c_spgno, q.qSPGD1.c_iteno } into g
                    //                    select new TemporaryPendingInfo()
                    //                    {
                    //                      NoID = g.Key.c_spgno,
                    //                      NoRef = g.Key.c_spgno,
                    //                      Item = g.Key.c_iteno,
                    //                      Acceptance = g.Sum(x => x.q.qSPGD1.n_sisa),
                    //                      Quantity = g.Sum(x => x.q.qSPGD1.n_sisa),
                    //                      QtySisa = g.Sum(x => x.q.qSPGD1.n_sisa)
                    //                    }).ToList();

                    qryDataTPI.AddRange(qryTempTPI.ToArray());
                    qryTempTPI.Clear();
                  }

                  #endregion

                  #region Combo

                  //if (listItemsCombo.Count > 0)
                  //{
                  //  qryTempTPI = (from q in qryItemsCodesSPG
                  //                join q1 in db.FA_Combos on q.qSPGD1.c_iteno equals q1.c_combo
                  //                where listItemsCombo.Contains(q.qSPGD1.c_iteno)
                  //                  && (!(from q_sq1 in db.LG_ORHs
                  //                        join q_sq2 in db.LG_ORD2s on new { q_sq1.c_gdg, q_sq1.c_orno } equals new { q_sq2.c_gdg, q_sq2.c_orno }
                  //                        where (q_sq2.c_spno == q.qSPGH.c_spgno) && (q_sq2.c_itemcombo == q1.c_combo)
                  //                          && ((q_sq1.l_delete.HasValue ? q_sq1.l_delete.Value : false) == false)
                  //                        select q_sq2.c_iteno).Contains(q.qSPGD1.c_iteno))
                  //                group new { q, q1 } by new { q.qSPGH.c_spgno, q1.c_combo, q1.c_iteno } into g
                  //                select new TemporaryPendingInfo()
                  //                {
                  //                  NoID = g.Key.c_spgno,
                  //                  NoRef = g.Key.c_spgno,
                  //                  Item = g.Key.c_iteno,
                  //                  ItemCombo = g.Key.c_combo,
                  //                  //Acceptance = g.Sum(x => ((x.q.qSPGD1.n_qty.HasValue ? x.q.qSPGD1.n_qty.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0))),
                  //                  //Quantity = g.Sum(x => ((x.q.qSPGD1.n_qty.HasValue ? x.q.qSPGD1.n_qty.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0))),
                  //                  Acceptance = g.Sum(x => (x.q.qSPGD1.n_sisa.HasValue ? x.q.qSPGD1.n_sisa.Value : 0)),
                  //                  Quantity = g.Sum(x => (x.q.qSPGD1.n_sisa.HasValue ? x.q.qSPGD1.n_sisa.Value : 0)),
                  //                  QtySisa = g.Sum(x => ((x.q.qSPGD1.n_sisa.HasValue ? x.q.qSPGD1.n_sisa.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0)))
                  //                }).ToList();

                  //  //qrySpPendGdgNonOktCmb = (from q in qryItemsCodesSPG
                  //  //                         join q2 in db.FA_Combos on q.qSPGD1.c_iteno equals q2.c_combo
                  //  //                         join q3 in db.LG_ORD2s on new { c_spno = q.qSPGD1.c_spgno, q.qSPGD1.c_iteno, c_itemcombo = q2.c_combo } equals new { q3.c_spno, q3.c_iteno, q3.c_itemcombo } into q_3
                  //  //                         from qORD2 in q_3.DefaultIfEmpty()
                  //  //                         where qORD2.c_iteno == null
                  //  //                           //&& SqlMethods.DateDiffMonth(q.qSPGH.d_spgdate, date) < 2
                  //  //                          && listItemsCombo.Contains(q.qSPGD1.c_iteno)
                  //  //                         group new { q, q2 } by new { q.qSPGD1.c_spgno, q2.c_combo, q2.c_iteno } into g
                  //  //                         select new TemporaryPendingInfo()
                  //  //                         {
                  //  //                           NoID = g.Key.c_spgno,
                  //  //                           NoRef = g.Key.c_spgno,
                  //  //                           Item = g.Key.c_iteno,
                  //  //                           ItemCombo = g.Key.c_combo,
                  //  //                           Acceptance = g.Sum(x => (x.q.qSPGD1.n_qty * x.q2.n_qty)),
                  //  //                           Quantity = g.Sum(x => (x.q.qSPGD1.n_qty * x.q2.n_qty)),
                  //  //                           QtySisa = g.Sum(x => (x.q.qSPGD1.n_sisa * x.q2.n_qty))
                  //  //                         }).ToList();

                  //  qryDataTPI.AddRange(qryTempTPI.ToArray());
                  //  qryTempTPI.Clear();
                  //}

                  #endregion

                  #endregion

                  #region Populate Data

                  qryDataTPI.ForEach(delegate(TemporaryPendingInfo t)
                  {
                    tpor = qryTempPOR.Find(x => x.c_iteno == t.Item && x.NoID == t.NoID);
                    if (tpor == null)
                    {
                      //t.n_sit = (tpor.n_gsisaSum.HasValue ? tpor.n_gsisaSum.Value : 0);
                      tpor = qryTempPOR.Find(x => x.c_iteno == t.Item);
                      if (tpor != null)
                      {
                        totalSpAcc = qryDataTPI.Where(x => x.Item == t.Item).GroupBy(x => x.Item).Sum(x => x.Sum(y => y.QtySisa));

                        if (string.IsNullOrEmpty(tpor.NoID))
                        {
                          tpor.NoID = t.NoID;
                          tpor.NoRef = t.NoRef;
                          tpor.Quantity = t.Quantity;
                          tpor.Acceptance = t.Acceptance;
                          tpor.QtySisa = t.QtySisa;
                          tpor.n_spacc = totalSpAcc;
                          tpor.ItemCombo = (t.ItemCombo == null ? string.Empty : t.ItemCombo);
                          tpor.l_combo = (string.IsNullOrEmpty(t.ItemCombo) ? false : true);
                        }
                        else
                        {
                          tporNew = new TemporaryProcessOR()
                          {
                            c_iteno = tpor.c_iteno,
                            v_itnam = tpor.v_itnam,
                            c_nosup = tpor.c_nosup,
                            n_avgsls = tpor.n_avgsls,
                            n_index = tpor.n_index,
                            n_soh = tpor.n_soh,
                            n_sit = tpor.n_sit,
                            n_bo = 0,
                            n_spacc = totalSpAcc,
                            n_box = tpor.n_box,
                            n_salpri = tpor.n_salpri,
                            n_pminord = tpor.n_pminord,
                            n_qminord = tpor.n_qminord,
                            n_bonus = tpor.n_bonus,
                            n_beli = tpor.n_beli,
                            c_via = tpor.c_via,
                            c_type = tpor.c_type,
                            c_kddivpri = tpor.c_kddivpri,
                            v_nmdivpri = tpor.v_nmdivpri,
                            n_avgslsdivpri = tpor.n_avgslsdivpri,
                            n_variabel = tpor.n_variabel,
                            n_idxp = tpor.n_idxp,
                            n_idxnp = tpor.n_idxnp,
                            n_pareto = tpor.n_pareto,
                            n_ideal = tpor.n_ideal,
                            n_order = tpor.n_order,
                            n_deviasi = tpor.n_deviasi,
                            NoID = t.NoID,
                            NoRef = t.NoRef,
                            Quantity = t.Quantity,
                            Acceptance = t.Acceptance,
                            QtySisa = t.QtySisa,
                            //l_combo = tpor.l_combo,
                            l_combo = (string.IsNullOrEmpty(t.ItemCombo) ? false : true),
                            ItemCombo = (t.ItemCombo == null ? string.Empty : t.ItemCombo)
                          };

                          listAddTPI.Add(tporNew);
                        }
                      }
                    }
                  });

                  qryTempPOR.AddRange(listAddTPI.ToArray());
                  listAddTPI.Clear();

                  #endregion

                  #endregion
                }
                else if (gdg.Equals(gdgHO))
                {
                  #region Gudang 1

                  #region SP Pending

                  var qryItemsCodesSP = (from q in db.LG_SPHs
                                         join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
                                         join q2 in db.LG_Cusmas on q.c_cusno equals q2.c_cusno
                                         where q1.n_sisa > 0 && (!lstSPExcl.Contains(q.c_type)) //&& q.c_type != "04"
                                          && q2.c_gdg == gdgHO //&& q.c_cusno == cust
                                          && (q.c_cusno == (string.IsNullOrEmpty(cust) ? q.c_cusno : cust))
                                          //&& (q2.n_days.HasValue ? (SqlMethods.DateDiffDay(q.d_spdate, DateTime.Now.Date) <= q2.n_days) :
                                          //            (SqlMethods.DateDiffMonth(q.d_spdate, DateTime.Now.Date) < 2)) 
                                          && (SqlMethods.DateDiffMonth(q.d_spdate, DateTime.Now.Date) < 2)
                                          && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                         select new
                                         {
                                           qSPH = q,
                                           qSPD1 = q1,
                                           qCus = q2
                                         }).AsQueryable();

                  #region Non Combo

                  if (listItems.Count > 0)
                  {
                    qryTempTPI = (from q in qryItemsCodesSP
                                  where listItems.Contains(q.qSPD1.c_iteno)
                                    && (!(from q_sq1 in db.LG_ORHs
                                          join q_sq2 in db.LG_ORD2s on new { q_sq1.c_gdg, q_sq1.c_orno } equals new { q_sq2.c_gdg, q_sq2.c_orno }
                                          where (q_sq2.c_spno == q.qSPH.c_spno)
                                            && ((q_sq2.c_itemcombo == null) || (q_sq2.c_itemcombo == "0000") || (q_sq2.c_itemcombo == "")) //(q_sq2.c_itemcombo == null || q_sq2.c_itemcombo == "0000")
                                            && ((q_sq1.l_delete.HasValue ? q_sq1.l_delete.Value : false) == false)
                                          select q_sq2.c_iteno).Contains(q.qSPD1.c_iteno))
                                  select new TemporaryPendingInfo()
                                  {
                                    NoID = q.qSPH.c_spno,
                                    NoRef = q.qSPH.c_sp.Trim(),
                                    Item = q.qSPD1.c_iteno,
                                    //Acceptance = (q.qSPD1.n_acc.HasValue ? q.qSPD1.n_acc.Value : 0),
                                    //Quantity = (q.qSPD1.n_qty.HasValue ? q.qSPD1.n_qty.Value : 0),
                                    Acceptance = (q.qSPD1.n_sisa.HasValue ? q.qSPD1.n_sisa.Value : 0),
                                    Quantity = (q.qSPD1.n_acc.HasValue ? q.qSPD1.n_acc.Value : 0),
                                    QtySisa = (q.qSPD1.n_sisa.HasValue ? q.qSPD1.n_sisa.Value : 0)
                                  }).ToList();

                    //qrySpPendGdgNonOkt = (from q in qryItemsCodesSP
                    //                      join q1 in db.LG_ORD2s on new { q.qSPH.c_spno, q.qSPD1.c_iteno } equals new { q1.c_spno, q1.c_iteno } into q_2
                    //                      from qORD2 in q_2.DefaultIfEmpty()
                    //                      join q2 in db.LG_ORHs on new { qORD2.c_gdg, qORD2.c_orno } equals new { q2.c_gdg, q2.c_orno }
                    //                      where ((q2.l_delete.Value == true) || (qORD2.c_iteno == null))
                    //                        //&& SqlMethods.DateDiffMonth(q.qSPH.d_spdate, date) < 2
                    //                        && listItems.Contains(q.qSPD1.c_iteno)
                    //                      select new TemporaryPendingInfo()
                    //                      {
                    //                        NoID = q.qSPH.c_spno,
                    //                        NoRef = q.qSPH.c_sp.Trim(),
                    //                        Item = q.qSPD1.c_iteno,
                    //                        Acceptance = q.qSPD1.n_acc,
                    //                        Quantity = q.qSPD1.n_qty,
                    //                        QtySisa = q.qSPD1.n_sisa
                    //                      }).ToList();

                    qryDataTPI.AddRange(qryTempTPI.ToArray());
                    qryTempTPI.Clear();
                  }

                  #endregion

                  #region Combo

                  //if (listItemsCombo.Count > 0)
                  //{
                  //  qryTempTPI = (from q in qryItemsCodesSP
                  //                join q1 in db.FA_Combos on q.qSPD1.c_iteno equals q1.c_combo
                  //                where listItemsCombo.Contains(q.qSPD1.c_iteno)
                  //                  && (!(from q_sq1 in db.LG_ORHs
                  //                        join q_sq2 in db.LG_ORD2s on new { q_sq1.c_gdg, q_sq1.c_orno } equals new { q_sq2.c_gdg, q_sq2.c_orno }
                  //                        where (q_sq2.c_spno == q.qSPH.c_spno) && (q_sq2.c_itemcombo == q1.c_combo)
                  //                         && ((q_sq1.l_delete.HasValue ? q_sq1.l_delete.Value : false) == false)
                  //                        select q_sq2.c_iteno).Contains(q.qSPD1.c_iteno))
                  //                group new { q, q1 } by new { q.qSPH.c_sp, q.qSPH.c_spno, q1.c_combo, q1.c_iteno } into g
                  //                select new TemporaryPendingInfo()
                  //                {
                  //                  NoID = g.Key.c_spno,
                  //                  NoRef = g.Key.c_sp.Trim(),
                  //                  Item = g.Key.c_iteno,
                  //                  ItemCombo = g.Key.c_combo,
                  //                  //Acceptance = g.Sum(x => ((x.q.qSPD1.n_acc.HasValue ? x.q.qSPD1.n_acc.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0))),
                  //                  //Quantity = g.Sum(x => ((x.q.qSPD1.n_qty.HasValue ? x.q.qSPD1.n_qty.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0))),
                  //                  Acceptance = g.Sum(x => ((x.q.qSPD1.n_sisa.HasValue ? x.q.qSPD1.n_sisa.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0))),
                  //                  Quantity = g.Sum(x => ((x.q.qSPD1.n_acc.HasValue ? x.q.qSPD1.n_acc.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0))),
                  //                  QtySisa = g.Sum(x => ((x.q.qSPD1.n_sisa.HasValue ? x.q.qSPD1.n_sisa.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0))),
                  //                }).ToList();

                  //  //qrySpPendGdgNonOktCmb = (from q in qryItemsCodesSP
                  //  //                         join q1 in db.FA_Combos on q.qSPD1.c_iteno equals q1.c_combo
                  //  //                         join q2 in db.LG_ORD2s on new { q.qSPH.c_spno, q.qSPD1.c_iteno } equals new { q2.c_spno, q2.c_iteno } into q_2
                  //  //                         from qORD2 in q_2.DefaultIfEmpty()
                  //  //                         where qORD2.c_iteno == null
                  //  //                           //&& SqlMethods.DateDiffMonth(q.qSPH.d_spdate, date) < 2
                  //  //                           && listItemsCombo.Contains(q.qSPD1.c_iteno)
                  //  //                         group new { q, q1 } by new { q.qSPH.c_sp, q.qSPH.c_spno, q1.c_combo, q1.c_iteno } into g
                  //  //                         select new TemporaryPendingInfo()
                  //  //                         {
                  //  //                           NoID = g.Key.c_spno,
                  //  //                           NoRef = g.Key.c_sp.Trim(),
                  //  //                           Item = g.Key.c_iteno,
                  //  //                           ItemCombo = g.Key.c_combo,
                  //  //                           Acceptance = g.Sum(x => (x.q.qSPD1.n_acc * x.q1.n_qty)),
                  //  //                           Quantity = g.Sum(x => (x.q.qSPD1.n_qty * x.q1.n_qty)),
                  //  //                           QtySisa = g.Sum(x => (x.q.qSPD1.n_sisa * x.q1.n_qty))
                  //  //                         }).ToList();

                  //  qryDataTPI.AddRange(qryTempTPI.ToArray());
                  //  qryTempTPI.Clear();
                  //}

                  #endregion

                  #endregion

                  #region Populate Data

                  qryDataTPI.ForEach(delegate(TemporaryPendingInfo t)
                  {
                    var qrySeek = qryTempPOR.Find(x => x.c_iteno == t.Item && x.NoID == t.NoID);
                    if (qrySeek == null)
                    {
                      //t.n_sit = (qrySeek.n_gsisaSum.HasValue ? qrySeek.n_gsisaSum.Value : 0);
                      qrySeek = qryTempPOR.Find(x => x.c_iteno == t.Item);
                      if (qrySeek != null)
                      {
                        totalSpAcc = qryDataTPI.Where(x => x.Item == t.Item).GroupBy(x => x.Item).Sum(x => x.Sum(y => y.QtySisa));

                        if (string.IsNullOrEmpty(qrySeek.NoID))
                        {
                          qrySeek.NoID = t.NoID;
                          qrySeek.NoRef = t.NoRef;
                          qrySeek.Quantity = t.Quantity;
                          qrySeek.Acceptance = t.Acceptance;
                          qrySeek.QtySisa = t.QtySisa;
                          qrySeek.n_spacc = totalSpAcc;
                          qrySeek.ItemCombo = (t.ItemCombo == null ? string.Empty : t.ItemCombo);
                          qrySeek.l_combo = (string.IsNullOrEmpty(t.ItemCombo) ? false : true);
                        }
                        else
                        {
                          var qrySeekNew = new TemporaryProcessOR()
                          {
                            c_iteno = qrySeek.c_iteno,
                            v_itnam = qrySeek.v_itnam,
                            c_nosup = qrySeek.c_nosup,
                            n_avgsls = qrySeek.n_avgsls,
                            n_index = qrySeek.n_index,
                            n_soh = qrySeek.n_soh,
                            n_sit = qrySeek.n_sit,
                            n_bo = qrySeek.n_bo,
                            n_spacc = totalSpAcc,
                            n_box = qrySeek.n_box,
                            n_salpri = qrySeek.n_salpri,
                            n_pminord = qrySeek.n_pminord,
                            n_qminord = qrySeek.n_qminord,
                            n_bonus = qrySeek.n_bonus,
                            n_beli = qrySeek.n_beli,
                            c_via = qrySeek.c_via,
                            c_type = qrySeek.c_type,
                            c_kddivpri = qrySeek.c_kddivpri,
                            v_nmdivpri = qrySeek.v_nmdivpri,
                            n_avgslsdivpri = qrySeek.n_avgslsdivpri,
                            n_variabel = qrySeek.n_variabel,
                            n_idxp = qrySeek.n_idxp,
                            n_idxnp = qrySeek.n_idxnp,
                            n_pareto = qrySeek.n_pareto,
                            n_ideal = qrySeek.n_ideal,
                            n_order = qrySeek.n_order,
                            n_deviasi = qrySeek.n_deviasi,
                            NoID = t.NoID,
                            NoRef = t.NoRef,
                            Quantity = t.Quantity,
                            Acceptance = t.Acceptance,
                            QtySisa = t.QtySisa,
                            //l_combo = qrySeek.l_combo,
                            l_combo = (string.IsNullOrEmpty(t.ItemCombo) ? false : true),
                            ItemCombo = (t.ItemCombo == null ? string.Empty : t.ItemCombo)
                          };

                          listAddTPI.Add(qrySeekNew);
                        }
                      }
                    }
                  });

                  qryTempPOR.AddRange(listAddTPI.ToArray());
                  listAddTPI.Clear();

                  #endregion

                  #endregion
                }
                else
                {
                  #region Selain Gudang 1

                  var qryItemsCodesSPG = (from q in db.LG_SPGHs
                                          join q1 in db.LG_SPGD1s on new { c_gdg = q.c_gdg1, q.c_spgno } equals new { q1.c_gdg, q1.c_spgno }
                                          where (q1.n_sisa > 0) && (q.c_gdg1 == gdg)
                                            && (q.l_status == true)
                                            && SqlMethods.DateDiffMonth(q.d_spgdate, date) < 2
                                            && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                          select new
                                          {
                                            qSPGH = q,
                                            qSPGD1 = q1
                                          }).AsQueryable();
                  #region Non Combo

                  #region Old Coded

                  //var qrySpgPendNonOkt = (from q in qryItemsCodesSPG
                  //                        join q1 in
                  //                          (from sq in db.LG_POD2s
                  //                           join sq1 in db.LG_POD1s on new { sq.c_gdg, sq.c_pono } equals new { sq1.c_gdg, sq1.c_pono }
                  //                           join sq2 in db.LG_ORD2s on new { sq.c_orno, sq1.c_iteno } equals new { sq2.c_orno, sq2.c_iteno }
                  //                           select new
                  //                           {
                  //                             sq1.c_iteno,
                  //                             sq2.c_spno
                  //                           }) on new { c_spno = q.qSPGH.c_spgno, q.qSPGD1.c_iteno } equals new { q1.c_spno, q1.c_iteno } into q_1
                  //                        from qSUBQ in q_1.DefaultIfEmpty()
                  //                        where q.qSPGD1.c_gdg == gdg && q.qSPGH.l_status == true &&
                  //                          qSUBQ.c_iteno == null && qSUBQ.c_spno == null
                  //                        group q by new { q.qSPGH.c_spgno, q.qSPGD1.c_iteno } into g
                  //                        select new TemporaryPendingInfo()
                  //                        {
                  //                          Item = g.Key.c_iteno,
                  //                          QtySisa = g.Sum(x => x.qSPGD1.n_sisa),
                  //                          Acceptance = g.Sum(x => x.qSPGD1.n_sisa),
                  //                          Quantity = g.Sum(x => x.qSPGD1.n_sisa),
                  //                          NoID = g.Key.c_spgno,
                  //                          NoRef = g.Key.c_spgno
                  //                        }).ToList();

                  #endregion

                  if (listItems.Count > 0)
                  {
                    qryTempTPI = (from q in qryItemsCodesSPG
                                  where listItems.Contains(q.qSPGD1.c_iteno)
                                    && (!(from q_sq1 in db.LG_ORHs
                                          join q_sq2 in db.LG_ORD2s on new { q_sq1.c_gdg, q_sq1.c_orno } equals new { q_sq2.c_gdg, q_sq2.c_orno }
                                          where (q_sq2.c_spno == q.qSPGH.c_spgno)
                                            && ((q_sq2.c_itemcombo == null) || (q_sq2.c_itemcombo == "0000") || (q_sq2.c_itemcombo == "")) //(q_sq2.c_itemcombo == null || q_sq2.c_itemcombo == "0000")
                                            && ((q_sq1.l_delete.HasValue ? q_sq1.l_delete.Value : false) == false)
                                          select q_sq2.c_iteno).Contains(q.qSPGD1.c_iteno))
                                  group new { q } by new { q.qSPGD1.c_spgno, q.qSPGD1.c_iteno } into g
                                  select new TemporaryPendingInfo()
                                  {
                                    NoID = g.Key.c_spgno,
                                    NoRef = g.Key.c_spgno,
                                    Item = g.Key.c_iteno,
                                    //Acceptance = g.Sum(x => x.q.qSPGD1.n_sisa),
                                    //Quantity = g.Sum(x => x.q.qSPGD1.n_sisa),
                                    //QtySisa = g.Sum(x => x.q.qSPGD1.n_sisa)
                                    Acceptance = g.Sum(x => (x.q.qSPGD1.n_sisa.HasValue ? x.q.qSPGD1.n_sisa.Value : 0)),
                                    Quantity = g.Sum(x => (x.q.qSPGD1.n_sisa.HasValue ? x.q.qSPGD1.n_sisa.Value : 0)),
                                    QtySisa = g.Sum(x => (x.q.qSPGD1.n_sisa.HasValue ? x.q.qSPGD1.n_sisa.Value : 0))
                                  }).ToList();

                    //qrySpgPendNonOkt = (from q in qryItemsCodesSPG
                    //                    join q2 in db.LG_ORD2s on new { c_spno = q.qSPGD1.c_spgno, q.qSPGD1.c_iteno } equals new { q2.c_spno, q2.c_iteno } into q_2
                    //                    from qORD2 in q_2.DefaultIfEmpty()
                    //                    join q3 in db.LG_ORHs on new { qORD2.c_gdg, qORD2.c_orno } equals new { q3.c_gdg, q3.c_orno }
                    //                    where ((q3.l_delete.Value == true) || (qORD2.c_iteno == null))
                    //                      && (qORD2.c_itemcombo == null || qORD2.c_itemcombo == "0000")
                    //                      && q.qSPGD1.c_gdg == gdg && q.qSPGH.l_status == true && q.qSPGD1.n_sisa > 0
                    //                      && listItems.Contains(q.qSPGD1.c_iteno)
                    //                    group new { q } by new { q.qSPGD1.c_spgno, q.qSPGD1.c_iteno } into g
                    //                    select new TemporaryPendingInfo()
                    //                    {
                    //                      NoID = g.Key.c_spgno,
                    //                      NoRef = g.Key.c_spgno,
                    //                      Item = g.Key.c_iteno,
                    //                      Acceptance = g.Sum(x => x.q.qSPGD1.n_sisa),
                    //                      Quantity = g.Sum(x => x.q.qSPGD1.n_sisa),
                    //                      QtySisa = g.Sum(x => x.q.qSPGD1.n_sisa)
                    //                    }).ToList();

                    //qrySpgPendNonOkt.Dump("Hasil");

                    qryDataTPI.AddRange(qryTempTPI.ToArray());
                    qryTempTPI.Clear();
                  }

                  #endregion

                  #region Combo

                  //if (listItemsCombo.Count > 0)
                  //{
                  //  qryTempTPI = (from q in qryItemsCodesSPG
                  //                join q1 in db.FA_Combos on q.qSPGD1.c_iteno equals q1.c_combo
                  //                where listItemsCombo.Contains(q.qSPGD1.c_iteno)
                  //                  && (!(from q_sq1 in db.LG_ORHs
                  //                        join q_sq2 in db.LG_ORD2s on new { q_sq1.c_gdg, q_sq1.c_orno } equals new { q_sq2.c_gdg, q_sq2.c_orno }
                  //                        where (q_sq2.c_spno == q.qSPGH.c_spgno)
                  //                          && ((q_sq2.c_itemcombo == null) || (q_sq2.c_itemcombo == "0000") || (q_sq2.c_itemcombo == "")) //(q_sq2.c_itemcombo == null || q_sq2.c_itemcombo == "0000")
                  //                          && ((q_sq1.l_delete.HasValue ? q_sq1.l_delete.Value : false) == false)
                  //                        select q_sq2.c_iteno).Contains(q.qSPGD1.c_iteno))
                  //                group new { q, q1 } by new { q.qSPGD1.c_spgno, q1.c_combo, q1.c_iteno } into g
                  //                select new TemporaryPendingInfo()
                  //                {
                  //                  NoID = g.Key.c_spgno,
                  //                  NoRef = g.Key.c_spgno,
                  //                  Item = g.Key.c_iteno,
                  //                  ItemCombo = g.Key.c_combo,
                  //                  //Acceptance = g.Sum(x => (x.q.qSPGD1.n_qty * x.q1.n_qty)),
                  //                  //Quantity = g.Sum(x => (x.q.qSPGD1.n_qty * x.q1.n_qty)),
                  //                  //QtySisa = g.Sum(x => (x.q.qSPGD1.n_sisa * x.q1.n_qty))
                  //                  //Acceptance = g.Sum(x => ((x.q.qSPGD1.n_qty.HasValue ? x.q.qSPGD1.n_qty.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0))),
                  //                  //Quantity = g.Sum(x => ((x.q.qSPGD1.n_qty.HasValue ? x.q.qSPGD1.n_qty.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0))),
                  //                  //QtySisa = g.Sum(x => ((x.q.qSPGD1.n_sisa.HasValue ? x.q.qSPGD1.n_sisa.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0)))
                  //                  Acceptance = g.Sum(x => ((x.q.qSPGD1.n_sisa.HasValue ? x.q.qSPGD1.n_sisa.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0))),
                  //                  Quantity = g.Sum(x => ((x.q.qSPGD1.n_sisa.HasValue ? x.q.qSPGD1.n_sisa.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0))),
                  //                  QtySisa = g.Sum(x => ((x.q.qSPGD1.n_sisa.HasValue ? x.q.qSPGD1.n_sisa.Value : 0) * (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0))),
                  //                }).ToList();

                  //  //qrySpPendGdgNonOktCmb = (from q in qryItemsCodesSPG
                  //  //                         join q2 in db.FA_Combos on q.qSPGD1.c_iteno equals q2.c_combo
                  //  //                         join q3 in db.LG_ORD2s on new { c_spno = q.qSPGD1.c_spgno, q.qSPGD1.c_iteno, c_itemcombo = q2.c_combo } equals new { q3.c_spno, q3.c_iteno, q3.c_itemcombo } into q_3
                  //  //                         from qORD2 in q_3.DefaultIfEmpty()
                  //  //                         join q4 in db.LG_ORHs on new { qORD2.c_gdg, qORD2.c_orno } equals new { q4.c_gdg, q4.c_orno }
                  //  //                         where ((q4.l_delete.Value == true) || (qORD2.c_iteno == null))
                  //  //                           && SqlMethods.DateDiffMonth(q.qSPGH.d_spgdate, date) < 2
                  //  //                           && listItemsCombo.Contains(q.qSPGD1.c_iteno)
                  //  //                         group new { q, q2 } by new { q.qSPGD1.c_spgno, q2.c_combo, q2.c_iteno } into g
                  //  //                         select new TemporaryPendingInfo()
                  //  //                         {
                  //  //                           NoID = g.Key.c_spgno,
                  //  //                           NoRef = g.Key.c_spgno,
                  //  //                           Item = g.Key.c_iteno,
                  //  //                           ItemCombo = g.Key.c_combo,
                  //  //                           Acceptance = g.Sum(x => (x.q.qSPGD1.n_qty * x.q2.n_qty)),
                  //  //                           Quantity = g.Sum(x => (x.q.qSPGD1.n_qty * x.q2.n_qty)),
                  //  //                           QtySisa = g.Sum(x => (x.q.qSPGD1.n_sisa * x.q2.n_qty))
                  //  //                         }).ToList();

                  //  //qrySpPendGdg1NonOktCmb.Dump("Hasil");

                  //  qryDataTPI.AddRange(qryTempTPI.ToArray());
                  //  qryTempTPI.Clear();
                  //}

                  #endregion

                  #region Populate Data

                  qryDataTPI.ForEach(delegate(TemporaryPendingInfo t)
                  {
                    var qrySeek = qryTempPOR.Find(x => x.c_iteno == t.Item && x.NoID == t.NoID);
                    if (qrySeek == null)
                    {
                      totalSpAcc = qryDataTPI.Sum(x => x.QtySisa);

                      //t.n_sit = (qrySeek.n_gsisaSum.HasValue ? qrySeek.n_gsisaSum.Value : 0);
                      qrySeek = qryTempPOR.Find(x => x.c_iteno == t.Item);
                      if (qrySeek != null)
                      {
                        if (string.IsNullOrEmpty(qrySeek.NoID))
                        {
                          qrySeek.NoID = t.NoID;
                          qrySeek.NoRef = t.NoRef;
                          qrySeek.Quantity = t.Quantity;
                          qrySeek.Acceptance = t.Acceptance;
                          qrySeek.QtySisa = t.QtySisa;
                          qrySeek.n_spacc = totalSpAcc;
                          qrySeek.ItemCombo = (t.ItemCombo == null ? string.Empty : t.ItemCombo);
                          qrySeek.l_combo = (string.IsNullOrEmpty(t.ItemCombo) ? false : true);
                        }
                        else
                        {
                          var qrySeekNew = new TemporaryProcessOR()
                          {
                            c_iteno = qrySeek.c_iteno,
                            v_itnam = qrySeek.v_itnam,
                            c_nosup = qrySeek.c_nosup,
                            n_avgsls = qrySeek.n_avgsls,
                            n_index = qrySeek.n_index,
                            n_soh = qrySeek.n_soh,
                            n_sit = qrySeek.n_sit,
                            n_bo = qrySeek.n_bo,
                            n_spacc = totalSpAcc,
                            n_box = qrySeek.n_box,
                            n_salpri = qrySeek.n_salpri,
                            n_pminord = qrySeek.n_pminord,
                            n_qminord = qrySeek.n_qminord,
                            n_bonus = qrySeek.n_bonus,
                            n_beli = qrySeek.n_beli,
                            c_via = qrySeek.c_via,
                            c_type = qrySeek.c_type,
                            c_kddivpri = qrySeek.c_kddivpri,
                            v_nmdivpri = qrySeek.v_nmdivpri,
                            n_avgslsdivpri = qrySeek.n_avgslsdivpri,
                            n_variabel = qrySeek.n_variabel,
                            n_idxp = qrySeek.n_idxp,
                            n_idxnp = qrySeek.n_idxnp,
                            n_pareto = qrySeek.n_pareto,
                            n_ideal = qrySeek.n_ideal,
                            n_order = qrySeek.n_order,
                            n_deviasi = qrySeek.n_deviasi,
                            NoID = t.NoID,
                            NoRef = t.NoRef,
                            Quantity = t.Quantity,
                            Acceptance = t.Acceptance,
                            QtySisa = t.QtySisa,
                            //l_combo = qrySeek.l_combo,
                            l_combo = (string.IsNullOrEmpty(t.ItemCombo) ? false : true),
                            ItemCombo = (t.ItemCombo == null ? string.Empty : t.ItemCombo)
                          };

                          listAddTPI.Add(qrySeekNew);
                        }
                      }
                    }
                  });

                  qryTempPOR.AddRange(listAddTPI.ToArray());

                  listAddTPI.Clear();

                  #endregion

                  #endregion
                }
              }

              qryDataTPI.Clear();

              #endregion

              #region Average Sales

              qryTempTPI = (from q in GlobalQuery.ViewAverageSales(db, date.AddMonths(-1))
                            join q1 in db.LG_Cusmas on q.Customer equals q1.c_cusno
                            where (q.Year == date.Year)
                              //&& (listItems.Contains(q.Item) || listItemsCombo.Contains(q.Item))
                              && listItems.Contains(q.Item)
                            group q by q.Item into g
                            select new TemporaryPendingInfo()
                            {
                              Item = g.Key,
                              Quantity = g.Sum(y => (y.Sales - y.Retur)),
                            }).ToList();
                            

              qryTempTPI.ForEach(delegate(TemporaryPendingInfo t)
              {
                if (t.Quantity != 0)
                {
                  listPOR = qryTempPOR.FindAll(x => x.c_iteno == t.Item);
                  if ((listPOR != null) && (listPOR.Count > 0))
                  {
                    for (nLoop = 0; nLoop < listPOR.Count; nLoop++)
                    {
                      tpor = listPOR[nLoop];
                      if (tpor != null)
                      {
                        tpor.n_avgsls = t.Quantity;
                      }
                    }

                    listPOR.Clear();
                  }
                }
              });

              qryTempTPI.Clear();

              #endregion

              #region Average Sales Divisi Principal

              qryTempTPI = (from sq in qryTempPOR
                            group sq by sq.c_kddivpri into g
                            select new TemporaryPendingInfo()
                            {
                              NoID = g.Key,
                              Quantity = g.Sum(x => x.n_avgsls * x.n_salpri)
                            }).ToList();

              qryTempTPI.ForEach(delegate(TemporaryPendingInfo t)
              {
                if (t.Quantity != 0)
                {
                  listPOR = qryTempPOR.FindAll(x => x.c_kddivpri == t.NoID);
                  if ((listPOR != null) && (listPOR.Count > 0))
                  {
                    for (nLoop = 0; nLoop < listPOR.Count; nLoop++)
                    {
                      tpor = listPOR[nLoop];
                      if (tpor != null)
                      {
                        tpor.n_avgslsdivpri = t.Quantity;
                      }
                    }

                    listPOR.Clear();
                  }
                }
              });

              qryTempTPI.Clear();

              #endregion

              #region Pareto, Stok Ideal, Denomination

              decimal nParStokIdeal = 0,
                avgVar = 0,
                halfDenom = 0;

              qryTempPOR.ForEach(delegate(TemporaryProcessOR t)
              {
                avgVar = (t.n_avgslsdivpri * t.n_variabel);

                #region Pareto

                t.n_pareto = (avgVar == 0 ? 0 : ((t.n_avgsls * t.n_salpri) / avgVar) * 100);

                #endregion

                #region Stok Ideal

                if (avgVar == 0)
                {
                  t.n_ideal = (t.n_avgsls * t.n_index);
                }
                else
                {
                  nParStokIdeal = ((t.n_avgsls * t.n_salpri) / avgVar);
                  if (((nParStokIdeal * 100) >= 100) && (t.n_idxp > 0))
                  {
                    t.n_ideal = (t.n_avgsls * t.n_idxp);
                  }
                  else if ((nParStokIdeal * 100) < 100 && (t.n_idxp > 0))
                  {
                    t.n_ideal = (t.n_avgsls * t.n_idxnp);
                  }
                }

                #endregion

                #region M O Q

                if (t.n_qminord > 0)
                {
                  halfDenom = (t.n_qminord / 2);
                  if (t.n_spacc < halfDenom)
                  {
                    t.n_spacc = 0;
                  }
                  else if ((t.n_spacc >= halfDenom) && (t.n_spacc <= t.n_qminord))
                  {
                    t.n_spacc = t.n_qminord;
                  }
                  else if ((t.n_spacc % t.n_qminord) < halfDenom)
                  {
                    t.n_spacc = (Math.Floor(t.n_spacc / halfDenom) * halfDenom);
                  }
                  else if (((t.n_spacc % t.n_qminord) % halfDenom) > 0)
                  {
                    t.n_spacc = ((Math.Floor(t.n_spacc / halfDenom) + 1) * halfDenom);
                  }
                  else
                  {
                    t.n_spacc = 0;
                  }
                }

                #endregion
              });

              #endregion

              #region Deviasi & Order

              if (tipeProcces.Equals("01"))
              {
                if (gdg.Equals(gdgHO))
                {
                  qryTempPOR.ForEach(delegate(TemporaryProcessOR t)
                  {
                    t.n_deviasi = Math.Round(t.n_spacc /
                            (t.n_box == 0 ? 1 : t.n_box), 0);
                    t.n_order = t.n_spacc;
                  });
                }
                else
                {
                  qryTempPOR.ForEach(delegate(TemporaryProcessOR t)
                  {
                    t.n_deviasi = Math.Round((t.n_box == 0 ? (t.n_ideal - t.n_soh - t.n_sit + t.n_spacc)
                      : ((t.n_ideal - t.n_soh - t.n_sit + t.n_spacc) / (t.n_box == 0 ? 1 : t.n_box))), 0);
                    t.n_order = (t.n_ideal - t.n_soh - t.n_sit - t.n_bo + t.n_spacc);
                  });
                }
              }
              else if (tipeProcces.Equals("02"))
              {
                qryTempPOR.ForEach(delegate(TemporaryProcessOR t)
                {
                  t.n_deviasi = Math.Round(t.n_spacc /
                            (t.n_box == 0 ? 1 : t.n_box), 0);
                  t.n_order = t.n_spacc;
                });
              }
              else if (tipeProcces.Equals("03"))
              {
                dateStart = new DateTime(date.Year, date.Month, 1);
                dateEnd = new DateTime(date.Year, date.Month, 15);

                qryTempTPI = (from q in db.SD_Aremas
                              join q1 in db.SD_Arebals on new { q.c_kdcab, q.c_invno } equals new { q1.c_kdcab, q1.c_invno }
                              join q2 in db.LG_Cusmas on q.c_kdcab equals q2.c_cab
                              where (q.c_jnstran == 'J') && (q2.l_cabang == true) && (q.l_tender == true)
                                && (q.d_invdate >= dateStart && q.d_invdate <= dateEnd)
                                //&& ((q.d_invdate.HasValue ? q.d_invdate.Value.Month : -1) == DateTime.Now.Month)
                                //&& ((q.d_invdate.HasValue ? q.d_invdate.Value.Year : -1) == DateTime.Now.Year)
                                //&& ((q.d_invdate.HasValue ? q.d_invdate.Value.Day : -1) <= 15)
                                //&& (listItems.Contains(q1.c_iteno) || listItemsCombo.Contains(q1.c_iteno))
                                && listItems.Contains(q1.c_iteno)
                                //&& (q2.c_cusno == cust)
                                && (q2.c_cusno == (string.IsNullOrEmpty(cust) ? q2.c_cusno : cust))
                              group q1 by q1.c_iteno into g
                              select new TemporaryPendingInfo()
                              {
                                Item = g.Key,
                                Quantity = g.Sum(x => (x.n_qtysal.HasValue ? x.n_qtysal.Value : 0))
                              }).ToList();

                qryTempTPI.ForEach(delegate(TemporaryPendingInfo t)
                {
                  if (t.Quantity != 0)
                  {
                    listPOR = qryTempPOR.FindAll(x => x.c_iteno == t.Item);

                    for (nLoop = 0; nLoop < listPOR.Count; nLoop++)
                    {
                      tpor = listPOR[nLoop];
                      if (tpor != null)
                      {
                        tpor.n_deviasi = Math.Round(tpor.n_spacc /
                                  (tpor.n_box == 0 ? 1 : tpor.n_box), 0);
                        tpor.n_order = t.Quantity;
                      }
                    }
                  }
                });
              }
              else if (tipeProcces.Equals("04"))
              {
                dateStart = (new DateTime(date.Year, date.Month, 16)).AddMonths(-1);
                dateEnd = (new DateTime(date.Year, date.Month, 1)).AddDays(-1);

                qryTempTPI = (from q in db.SD_Aremas
                              join q1 in db.SD_Arebals on new { q.c_kdcab, q.c_invno } equals new { q1.c_kdcab, q1.c_invno }
                              join q2 in db.LG_Cusmas on q.c_kdcab equals q2.c_cab
                              where (q.c_jnstran == 'J') && (q2.l_cabang == true) && (q.l_tender == true)
                                && (q.d_invdate >= dateStart && q.d_invdate <= dateEnd)
                                //&& ((q.d_invdate.HasValue ? q.d_invdate.Value.Month : -1) == DateTime.Now.Month) 
                                //&& ((q.d_invdate.HasValue ? q.d_invdate.Value.Year : -1) == DateTime.Now.Year)
                                //&& ((q.d_invdate.HasValue ? q.d_invdate.Value.Day : -1) > 15)
                                //&& (listItems.Contains(q1.c_iteno) || listItemsCombo.Contains(q1.c_iteno))
                                && listItems.Contains(q1.c_iteno)
                                //&& (q2.c_cusno == cust)
                                && (q2.c_cusno == (string.IsNullOrEmpty(cust) ? q2.c_cusno : cust))
                              group q1 by q1.c_iteno into g
                              select new TemporaryPendingInfo()
                              {
                                Item = g.Key,
                                Quantity = g.Sum(x => (x.n_qtysal.HasValue ? x.n_qtysal.Value : 0))
                              }).ToList();


                qryTempTPI.ForEach(delegate(TemporaryPendingInfo t)
                {
                  if (t.Quantity != 0)
                  {
                    listPOR = qryTempPOR.FindAll(x => x.c_iteno == t.Item);

                    for (nLoop = 0; nLoop < listPOR.Count; nLoop++)
                    {
                      tpor = listPOR[nLoop];
                      if (tpor != null)
                      {
                        tpor.n_deviasi = Math.Round(tpor.n_spacc /
                                  (tpor.n_box == 0 ? 1 : tpor.n_box), 0);
                        tpor.n_order = t.Quantity;
                      }
                    }
                  }
                });
              }

              #endregion

              #region Hapus Salah divisi prinsipal, SP Kosong, Quantity 0, SPAcc > SoH

              qryTempPOR.RemoveAll(delegate(TemporaryProcessOR t)
              {
                #region Calculate Sisa

                if (!tipeProcces.Equals("02"))
                {
                  t.n_qty = (t.n_box == 0 ? t.n_deviasi : (t.n_box * t.n_deviasi));
                }
                else
                {
                  t.n_qty = t.n_spacc;
                }
                #endregion


                #region Hapus yang tidak termasuk kode divisi prinsipal

                //if (!string.IsNullOrEmpty(divPrinc))
                //{
                //  if (!t.c_kddivpri.Equals(divPrinc, StringComparison.OrdinalIgnoreCase))
                //  {
                //    return true;
                //  }
                //}

                #endregion

                #region SP Kosong atau Quantiy 0

                if (tipeProcces.Equals("02"))
                {
                  if (string.IsNullOrEmpty(t.NoID) || (t.Quantity == 0) || (t.n_spacc <= 0))
                  {
                    return true;
                  }
                }
                else if (t.n_qty <= 0)
                {
                  return true;
                }

                #endregion

                return false;
              });

              #endregion
              
              listItems.Clear();
              //listItemsCombo.Clear();
              //dicTotalSOH.Clear();

              nCount = qryTempPOR.Count();

              if (nCount > 0)
              {
                dic.Add(Constant.DEFAULT_NAMING_RECORDS, qryTempPOR.ToArray());
              }

              dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

              dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

              qryTempPOR.Clear();
            }
            break;

          #endregion

          #region MODEL_COMMON_QUERY_MULTIPLE_PROCESS_ORG

          case Constant.MODEL_PROCESS_QUERY_MULTIPLE_PROCESS_ORG:
            {
              //char gdgFrom = '2';
              char gdgFrom = (parameters.ContainsKey("activeGudang") ? Convert.ToChar(((Functionals.ParameterParser)parameters["activeGudang"]).Value) : char.MinValue);
              //char gdgTo = '1';
              string divPrinc = (parameters.ContainsKey("divSuplier") ? (string)((Functionals.ParameterParser)parameters["divSuplier"]).Value : string.Empty);
              string viaKirim = (parameters.ContainsKey("via") ? (string)((Functionals.ParameterParser)parameters["via"]).Value : string.Empty);
              string tipeProd = (parameters.ContainsKey("typeItem") ? (string)((Functionals.ParameterParser)parameters["typeItem"]).Value : string.Empty);
              string supl = (parameters.ContainsKey("suplier") ? (string)((Functionals.ParameterParser)parameters["suplier"]).Value : string.Empty);

              TemporaryPendingInfo tpi = null;
              TemporaryProcessOR tpor = null,
                tporNew = null;

              List<string> listItems = null;
              List<string> listSG = null;

              int nLoop = 0;

              List<TemporaryProcessOR> qryTempPOR = null,
                listPOR = null,
                listAddTPI = null;

              List<TemporaryPendingInfo> qryDataTPI = null,
                qryTempTPI = null;

              decimal decParetoVar = (from q in db.MsVariables
                                      where q.c_type == "02" && q.c_portal == '3'
                                      select (q.n_value.HasValue ? q.n_value.Value : 0)).Take(1).SingleOrDefault();

              List<string> listTypeItem = new List<string>();

              listTypeItem.Add("07");
              listTypeItem.Add("09");
              listTypeItem.Add("10");
              listTypeItem.Add("11");

              qryTempPOR = (from q in db.FA_MasItms
                            join q1 in db.LG_DatSups on q.c_nosup equals q1.c_nosup
                            join q2 in db.FA_Divpris on q.c_iteno equals q2.c_iteno into q_2
                            from qDP in q_2.DefaultIfEmpty()
                            join q3 in db.FA_MsDivPris on qDP.c_kddivpri equals q3.c_kddivpri into q_3
                            from qMDP in q_3.DefaultIfEmpty()
                            join q4 in db.LG_MsIndexDs on new { qDP.c_kddivpri, c_gdg = gdgFrom } equals new { q4.c_kddivpri, q4.c_gdg } into q_4
                            from qMID in q_4.DefaultIfEmpty()
                            join q5 in db.LG_msIndexHs on q1.c_nosup equals q5.c_nosup
                            join q6 in db.SCMS_MSITEM_CATs on q.c_iteno equals q6.c_iteno into q_6
                            from qMCP in q_6.DefaultIfEmpty()
                            where q.l_aktif == true && q1.l_aktif == true && q.c_nosup == supl
                              && (string.IsNullOrEmpty(viaKirim) ? viaKirim : q.c_via) == viaKirim
                              && (listTypeItem.Contains(tipeProd) ? qMCP.c_type : q.c_type) == tipeProd
                              && q.c_type != "02"
                              && ((string.IsNullOrEmpty(divPrinc) ? divPrinc : qDP.c_kddivpri) == divPrinc)
                              && q5.c_gdg == '2'
                              && !(from sq1 in db.vw_indexSGs
                                 where sq1.n_index > 2
                                 select sq1.c_iteno).Contains(q.c_iteno)
                            select new TemporaryProcessOR()
                            {
                              c_iteno = q.c_iteno,
                              v_itnam = q.v_itnam,
                              c_nosup = q.c_nosup,
                              n_avgsls = 0,
                              n_index = (q5.n_index.HasValue ? q5.n_index.Value : 0),
                              n_soh = 0,
                              n_sit = 0,
                              n_bo = 0,
                              n_spacc = 0,
                              n_box = (q.n_box.HasValue ? q.n_box.Value : 0),
                              n_salpri = (q.n_salpri.HasValue ? q.n_salpri.Value : 0),
                              n_pminord = (q.n_pminord.HasValue ? q.n_pminord.Value : 0),
                              n_qminord = (q.n_qminord.HasValue ? q.n_qminord.Value : 0),
                              n_bonus = (q.n_bonus.HasValue ? q.n_bonus.Value : 0),
                              n_beli = (q.n_beli.HasValue ? q.n_beli.Value : 0),
                              c_via = q.c_via,
                              c_type = (listTypeItem.Contains(tipeProd) ? qMCP.c_type : q.c_type),
                              c_type_cat = qMCP.c_type,
                              c_kddivpri = (qDP.c_kddivpri == null ? string.Empty : qDP.c_kddivpri),
                              v_nmdivpri = qMDP.v_nmdivpri,
                              n_avgslsdivpri = 0,
                              n_variabel = decParetoVar,
                              n_idxp = (qMID.n_idxp.HasValue ? qMID.n_idxp.Value : 0),
                              n_idxnp = (qMID.n_idxnp.HasValue ? qMID.n_idxnp.Value : 0),
                              n_pareto = 0,
                              n_ideal = 0,
                              n_order = 0,
                              n_deviasi = 0,
                              NoID = string.Empty,
                              NoRef = string.Empty,
                              Quantity = 0,
                              Acceptance = 0,
                              QtySisa = 0,
                              l_combo = (q.l_combo.HasValue ? q.l_combo.Value : false)
                            }).Distinct().ToList();

                            var qryTempPOR2 = qryTempPOR.ToList();
                            if (!listTypeItem.Contains(tipeProd))
                            {
                                qryTempPOR = qryTempPOR.Where(x => !listTypeItem.Contains(x.c_type_cat)).Distinct().ToList();
                            }

              listItems = qryTempPOR.Select(x => x.c_iteno).Distinct().ToList();

              #region Cek Stock On Hand

              //dicTotalSOH = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

              #region SOH

              //qryTotalSOH = (from q in GlobalQuery.ViewStockLite(db, gdgFrom)
              //               where listItems.Contains(q.c_iteno)
              //                && (q.n_gsisa != 0)
              //               group q by q.c_iteno into g
              //               select new TemporaryPendingInfo()
              //               {
              //                 Item = g.Key,
              //                 QtySisa = g.Sum(x => x.n_gsisa)
              //               }).Distinct().ToList();

              //qryTotalSOH = (from q in db.LG_RNHs
              //            //join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno, c_iteno = item } equals new { q1.c_gdg, q1.c_rnno, q1.c_iteno }
              //            join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
              //            // --A Production -- for Deleted
              //            join q2 in db.LG_RND2s on new { q.c_gdg, q.c_rnno, q1.c_iteno } equals new { q2.c_gdg, q2.c_rnno, q2.c_iteno }
              //            join q3 in db.LG_POD2s on new { q.c_gdg, c_pono = q2.c_no } equals new { q3.c_gdg, q3.c_pono }
              //            join q4 in db.LG_ORHs on new { q3.c_gdg, q3.c_orno } equals new { q4.c_gdg, q4.c_orno }
              //            // --A
              //            where
              //              // Production
              //              //((q.c_type != "06")  || ((q.l_khusus.HasValue ? q.l_khusus.Value : false) == false))

              //              // --A Production -- for Deleted
              //              (q4.c_type != "02") &&

              //              // --A

              //              listItems.Contains(q1.c_iteno)
              //              && ((q1.n_gsisa != 0) || (q1.n_bsisa != 0))
              //              && q.c_gdg == gdgFrom
              //            // Production
              //            //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
              //               group q1 by q1.c_iteno into g
              //               select new TemporaryPendingInfo()
              //               {
              //                 Item = g.Key,
              //                 QtySisa = g.Sum(x=>x.n_gsisa.Value)
              //               }).Distinct().ToList();

              qryTotalSOHPL = (from q in db.LG_RNHs
                          join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                          where q.c_gdg == gdgFrom
                          && listItems.Contains(q1.c_iteno)
                          group q1 by q1.c_iteno into g
                          select new TemporaryPendingInfo()
                          {
                            Item = g.Key,
                            QtySisa = g.Sum(x => x.n_gsisa.Value)
                          }).Distinct().ToList();

              qryTotalSOHCB = (from q in db.LG_ComboHs
                             where q.c_gdg == gdgFrom
                             && listItems.Contains(q.c_iteno)
                             group q by q.c_iteno into g
                             select new TemporaryPendingInfo()
                             {
                                 Item = g.Key,
                                 QtySisa = g.Sum(x => x.n_gsisa.Value)
                             }).Distinct().ToList();

              qryTotalSOH = qryTotalSOHPL.Union(qryTotalSOHCB).ToList();

              qryTotalSOH = qryTotalSOH.GroupBy(x => x.Item).Select(x => new TemporaryPendingInfo { Item = x.Key, QtySisa = x.Sum(y => y.QtySisa) }).ToList();

              if ((qryTotalSOH != null) && (qryTotalSOH.Count > 0))
              {
                qryTempPOR.ForEach(delegate(TemporaryProcessOR t)
                {
                  tpi = qryTotalSOH.Find(x => x.Item == t.c_iteno);
                  if (tpi != null)
                  {
                    //if (dicTotalSOH.ContainsKey(t.c_iteno))
                    //{
                    //  nTotalSOH = dicTotalSOH[t.c_iteno];
                    //}
                    //else
                    //{
                    //  nTotalSOH = 0;

                    //  dicTotalSOH.Add(t.c_iteno, 0);
                    //}

                    t.n_soh = tpi.QtySisa;

                    //nTotalSOH += tpi.QtySisa;

                    //dicTotalSOH[t.c_iteno] = nTotalSOH;
                  }
                });

                qryTotalSOH.Clear();
              }

              #endregion

              #region SIT

              qryTotalSOH = (from q in db.LG_SJHs
                             join q1 in db.LG_SJD2s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                             where listItems.Contains(q1.c_iteno)
                              && (q.l_status == false) && (q1.n_gsisa != 0) && (q.c_gdg2 == gdgFrom)
                              // Production
                              //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                              group q1 by new { q1.c_iteno } into gSUM
                             select new TemporaryPendingInfo()
                             {
                               //Item = q1.c_iteno,
                               //QtySisa = (q1.n_gsisa.HasValue ? q1.n_gsisa.Value : 0),
                               Item = gSUM.Key.c_iteno,
                               QtySisa = gSUM.Sum(x => (x.n_gsisa.HasValue ? x.n_gsisa.Value : 0)),
                             }).Distinct().ToList();

              if ((qryTotalSOH != null) && (qryTotalSOH.Count > 0))
              {
                qryTempPOR.ForEach(delegate(TemporaryProcessOR t)
                {
                  //qryTotalSOH = qryTotalSOH.Where(x => x.Item == t.Item);
                  tpi = qryTotalSOH.Find(x => x.Item == t.c_iteno);
                  if (tpi != null)
                  {
                    //if (dicTotalSOH.ContainsKey(t.c_iteno))
                    //{
                    //  nTotalSOH = dicTotalSOH[t.c_iteno];
                    //}
                    //else
                    //{
                    //  nTotalSOH = 0;

                    //  dicTotalSOH.Add(t.c_iteno, 0);
                    //}

                    t.n_soh += tpi.QtySisa;

                    //t.n_sit = tpi.QtySisa;

                    //nTotalSOH += tpi.QtySisa;

                    //dicTotalSOH[t.c_iteno] = nTotalSOH;
                  }
                });

                qryTotalSOH.Clear();
              }

              #endregion

              #endregion

              #region Pending

              listAddTPI = new List<TemporaryProcessOR>();
              qryDataTPI = new List<TemporaryPendingInfo>();

              List<TemporaryPendingInfo> qtySP = null;

              #region Old Coded

              //#region SP Cabang

              //qtySP = (from q in db.LG_SPHs
              //             join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
              //             join q2 in db.LG_Cusmas on q.c_cusno equals q2.c_cusno
              //             where (q1.n_sisa > 0) && (!lstSPExcl.Contains(q.c_type)) //&& (q.c_type != "04")
              //                && (q2.c_gdg == gdgFrom)
              //                && (SqlMethods.DateDiffMonth(q.d_spdate, DateTime.Now) < 2)
              //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
              //             group q1 by q1.c_iteno into gSum
              //             select new TemporaryPendingInfo()
              //             {
              //                Item = gSum.Key,
              //                QtySisa = gSum.Sum(x=>x.n_sisa.HasValue ? x.n_sisa.Value : 0)
              //             }).ToList();

              //if ((qtySP != null) && (qtySP.Count > 0))
              //{
              //  qryTempPOR.ForEach(delegate(TemporaryProcessOR t)
              //  {
              //    tpi = qtySP.Find(x => x.Item == t.c_iteno);
              //    if (tpi != null)
              //    {

              //      t.n_spacc = tpi.QtySisa;

              //    }
              //  });

              //  qtySP.Clear();
              //}

              //#endregion

              //#region SG Pending

              //qtySP = (from q in db.LG_SPGHs
              //         join q1 in db.LG_SPGD1s on q.c_spgno equals q1.c_spgno
              //         where (q1.n_sisa > 0) && (!lstSPExcl.Contains(q.c_type)) //&& (q.c_type != "04")
              //            && (SqlMethods.DateDiffMonth(q.d_spgdate, DateTime.Now) < 2)
              //            && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
              //            && (q.c_gdg2 == gdgFrom) && (q.l_status == true)
              //         group q1 by q1.c_iteno into gSum
              //         select new TemporaryPendingInfo()
              //         {
              //           Item = gSum.Key,
              //           QtySisa = gSum.Sum(x => x.n_sisa.HasValue ? x.n_sisa.Value : 0)
              //         }).ToList();

              //if ((qtySP != null) && (qtySP.Count > 0))
              //{
              //  qryTempPOR.ForEach(delegate(TemporaryProcessOR t)
              //  {
              //    tpi = qtySP.Find(x => x.Item == t.c_iteno);
              //    if (tpi != null)
              //    {

              //      t.n_spacc += tpi.QtySisa;

              //    }
              //  });

              //  qtySP.Clear();
              //}

              //#endregion

              #endregion

              #region Pending SP

              var qryItemsCodesSP = (from q in db.LG_SPHs
                                     join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
                                     join q2 in db.LG_Cusmas on q.c_cusno equals q2.c_cusno
                                     where (q1.n_sisa > 0) //&& (!lstSPExcl.Contains(q.c_type)) //&& (q.c_type != "04")
                                        && (q2.c_gdg == gdgFrom)
                                        //&& (q2.n_days.HasValue ? (SqlMethods.DateDiffDay(q.d_spdate, DateTime.Now.Date) <= q2.n_days) :
                                        //              (SqlMethods.DateDiffMonth(q.d_spdate, DateTime.Now.Date) < 2)) 
                                        && (SqlMethods.DateDiffMonth(q.d_etasp, DateTime.Now.Date) < 2)
                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                        && (q.c_type != "06")
                                     select new
                                     {
                                       qSPH = q,
                                       qSPD1 = q1,
                                       qCus = q2
                                     }).AsQueryable();

              qryTempTPI = (from q in qryItemsCodesSP
                            where listItems.Contains(q.qSPD1.c_iteno)
                              //&&   (!(from q_sq1 in db.LG_ORHs
                              //      join q_sq2 in db.LG_ORD2s on new { q_sq1.c_gdg, q_sq1.c_orno } equals new { q_sq2.c_gdg, q_sq2.c_orno }
                              //      where (q_sq2.c_spno == q.qSPH.c_spno)
                              //        && ((q_sq2.c_itemcombo == null) || (q_sq2.c_itemcombo == "0000") || (q_sq2.c_itemcombo == "")) //(q_sq2.c_itemcombo == null || q_sq2.c_itemcombo == "0000")
                              //        && ((q_sq1.l_delete.HasValue ? q_sq1.l_delete.Value : false) == false)
                              //      select q_sq2.c_iteno).Contains(q.qSPD1.c_iteno))
                              //&& (!(from sq1 in db.LG_SPGHs
                              //      join sq2 in db.LG_SPGD2s on new { c_gdg = sq1.c_gdg1, sq1.c_spgno } equals new { sq2.c_gdg, sq2.c_spgno }
                              //      where (sq2.c_spno == q.qSPH.c_spno)
                              //        && ((sq1.l_delete.HasValue ? sq1.l_delete.Value : false) == false)
                              //      select sq2.c_iteno).Contains(q.qSPD1.c_iteno))
                            select new TemporaryPendingInfo()
                            {
                              NoID = q.qSPH.c_spno,
                              NoRef = q.qSPH.c_sp.Trim(),
                              Item = q.qSPD1.c_iteno,
                              //Acceptance = (q.qSPD1.n_acc.HasValue ? q.qSPD1.n_acc.Value : 0),
                              //Quantity = (q.qSPD1.n_qty.HasValue ? q.qSPD1.n_qty.Value : 0),
                              Quantity = (q.qSPD1.n_acc.HasValue ? q.qSPD1.n_acc.Value : 0),
                              Acceptance = (q.qSPD1.n_sisa.HasValue ? q.qSPD1.n_sisa.Value : 0),
                              QtySisa = (q.qSPD1.n_sisa.HasValue ? q.qSPD1.n_sisa.Value : 0)
                            }).ToList();

              //var i = qryTempTPI.Where(x => x.Item == "5290").ToList();

              qryDataTPI.AddRange(qryTempTPI.ToArray());
              qryTempTPI.Clear();

              #endregion

              #region Populate

              qryDataTPI.ForEach(delegate(TemporaryPendingInfo t)
              {
                tpor = qryTempPOR.Find(x => x.c_iteno == t.Item && x.NoID == t.NoID);
                if (tpor == null)
                {
                  tpor = qryTempPOR.Find(x => x.c_iteno == t.Item);
                  if (tpor != null)
                  {
                    totalSpAcc = qryDataTPI.Where(x => x.Item == t.Item).GroupBy(x => x.Item).Sum(x => x.Sum(y => y.QtySisa));

                    if (string.IsNullOrEmpty(tpor.NoID))
                    {
                      tpor.NoID = t.NoID;
                      tpor.NoRef = t.NoRef;
                      tpor.Quantity = t.Quantity;
                      tpor.Acceptance = t.Acceptance;
                      tpor.QtySisa = t.QtySisa;
                      tpor.n_spacc = totalSpAcc;
                      tpor.ItemCombo = (t.ItemCombo == null ? string.Empty : t.ItemCombo);
                      tpor.l_combo = (string.IsNullOrEmpty(t.ItemCombo) ? false : true);
                    }
                    else
                    {
                      tporNew = new TemporaryProcessOR()
                      {
                        c_iteno = tpor.c_iteno,
                        v_itnam = tpor.v_itnam,
                        c_nosup = tpor.c_nosup,
                        n_avgsls = tpor.n_avgsls,
                        n_index = tpor.n_index,
                        n_soh = tpor.n_soh,
                        n_sit = tpor.n_sit,
                        n_bo = 0,
                        n_spacc = totalSpAcc,
                        n_box = tpor.n_box,
                        n_salpri = tpor.n_salpri,
                        n_pminord = tpor.n_pminord,
                        n_qminord = tpor.n_qminord,
                        n_bonus = tpor.n_bonus,
                        n_beli = tpor.n_beli,
                        c_via = tpor.c_via,
                        c_type = tpor.c_type,
                        c_kddivpri = tpor.c_kddivpri,
                        v_nmdivpri = tpor.v_nmdivpri,
                        n_avgslsdivpri = tpor.n_avgslsdivpri,
                        n_variabel = tpor.n_variabel,
                        n_idxp = tpor.n_idxp,
                        n_idxnp = tpor.n_idxnp,
                        n_pareto = tpor.n_pareto,
                        n_ideal = tpor.n_ideal,
                        n_order = tpor.n_order,
                        n_deviasi = tpor.n_deviasi,
                        NoID = t.NoID,
                        NoRef = t.NoRef,
                        Quantity = t.Quantity,
                        Acceptance = t.Acceptance,
                        QtySisa = t.QtySisa,
                        //l_combo = tpor.l_combo,
                        l_combo = (string.IsNullOrEmpty(t.ItemCombo) ? false : true),
                        ItemCombo = (t.ItemCombo == null ? string.Empty : t.ItemCombo)
                      };

                      listAddTPI.Add(tporNew);
                    }
                  }
                }
              });

              qryTempPOR.AddRange(listAddTPI.ToArray());
              listAddTPI.Clear();

              #endregion



              #region SIT

              var qrySP1 = (from q in db.LG_SPGHs
                            join q1 in db.LG_SPGD1s on q.c_spgno equals q1.c_spgno
                            where (q1.n_sisa > 0) //&& (q.c_type != "04")
                               && (SqlMethods.DateDiffMonth(q.d_spgdate, DateTime.Now) < 2)
                               && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                               && (q.c_gdg1 == gdgFrom) && (q.l_status == true)
                               && (!q.d_sgsender.HasValue)
                            //&& string.IsNullOrEmpty(q.d_sgsender.Value.ToString())
                            //&& ((q.d_sgsender.HasValue ? q.d_sgsender.Value : DateTime.Now) == DateTime.Now)
                            group q1 by q1.c_iteno into gSum
                            select new TemporaryPendingInfo()
                            {
                                Item = gSum.Key,
                                QtySisa = gSum.Sum(x => x.n_sisa.HasValue ? x.n_sisa.Value : 0)
                            }).ToList();
              var qrySP2 = (from q in db.LG_SPGHs
                            join q1 in db.LG_SPGD1s on q.c_spgno equals q1.c_spgno
                            where (q1.n_sisa > 0) //&& (q.c_type != "04")
                               && (SqlMethods.DateDiffMonth(q.d_spgdate, DateTime.Now) < 2)
                               && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                               && (q.c_gdg1 == gdgFrom) && (q.l_status == true)
                               && ((q.d_sgsender.HasValue ? q.d_sgsender.Value.Month : 0) == DateTime.Now.Month)
                               && ((q.d_sgsender.HasValue ? q.d_sgsender.Value.Year : 0) == DateTime.Now.Year)
                            group q1 by q1.c_iteno into gSum
                            select new TemporaryPendingInfo()
                            {
                                Item = gSum.Key,
                                QtySisa = gSum.Sum(x => x.n_sisa.HasValue ? x.n_sisa.Value : 0)
                            }).ToList();

              var qry1 = qrySP1.Union(qrySP2).ToList();
                
                    //listPLSum =
                    //  listPld1.GroupBy(x => new { x.c_plno, x.c_iteno })
                    //  .Select(x => new LG_PLD1_SUM_BYBATCH() { c_plno = x.Key.c_plno, c_iteno = x.Key.c_iteno, n_qty = x.Sum(y => (y.n_qty.HasValue ? y.n_qty.Value : 0)) }).ToList();

                qtySP = qry1.GroupBy(x => new {x.Item})
                        .Select(x => new TemporaryPendingInfo() { Item = x.Key.Item, QtySisa = x.Sum(y => decimal.Parse((y.QtySisa.ToString())))}).ToList();
              //qtySP = (from q in (qrySP1.Union(qrySP2)) group q by q.Item into gSum
              //            select 
              //              {
              //                  Item = gSum.Key,
              //                  QtySisa = gSum.Sum(x => x.QtySisa.HasValue ? x.QtySisa.Value : 0)
              //              }).ToList();

              if ((qtySP != null) && (qtySP.Count > 0))
              {
                qryTempPOR.ForEach(delegate(TemporaryProcessOR t)
                {
                  tpi = qtySP.Find(x => x.Item == t.c_iteno);
                  if (tpi != null)
                  {

                    t.n_sit += tpi.QtySisa;

                  }
                });

                qtySP.Clear();
              }

              #endregion

              #endregion

              #region Average Sales

              //qryTempTPI = (from q in db.LG_AvgSales
              //              join q1 in db.LG_Cusmas on q.c_cusno equals q1.c_cusno
              //              where q.s_tahun == DateTime.Now.Year && q1.c_gdg == gdgFrom
              //                && listItems.Contains(q.c_iteno)
              //              group q by q.c_iteno into g
              //              select new TemporaryPendingInfo()
              //              {
              //                NoID = g.Key,
              //                Quantity = g.Sum(x =>
              //                  (DateTime.Now.Month == 1 ? (x.n_sls01.HasValue ? x.n_sls01.Value : 0) - (x.n_rtr01.HasValue ? x.n_rtr01.Value : 0) :
              //                  (DateTime.Now.Month == 2 ? (x.n_sls02.HasValue ? x.n_sls02.Value : 0) - (x.n_rtr02.HasValue ? x.n_rtr02.Value : 0) :
              //                  (DateTime.Now.Month == 3 ? (x.n_sls03.HasValue ? x.n_sls03.Value : 0) - (x.n_rtr03.HasValue ? x.n_rtr03.Value : 0) :
              //                  (DateTime.Now.Month == 4 ? (x.n_sls04.HasValue ? x.n_sls04.Value : 0) - (x.n_rtr04.HasValue ? x.n_rtr04.Value : 0) :
              //                  (DateTime.Now.Month == 5 ? (x.n_sls05.HasValue ? x.n_sls05.Value : 0) - (x.n_rtr05.HasValue ? x.n_rtr05.Value : 0) :
              //                  (DateTime.Now.Month == 6 ? (x.n_sls06.HasValue ? x.n_sls06.Value : 0) - (x.n_rtr06.HasValue ? x.n_rtr06.Value : 0) :
              //                  (DateTime.Now.Month == 7 ? (x.n_sls07.HasValue ? x.n_sls07.Value : 0) - (x.n_rtr07.HasValue ? x.n_rtr07.Value : 0) :
              //                  (DateTime.Now.Month == 8 ? (x.n_sls08.HasValue ? x.n_sls08.Value : 0) - (x.n_rtr08.HasValue ? x.n_rtr08.Value : 0) :
              //                  (DateTime.Now.Month == 9 ? (x.n_sls09.HasValue ? x.n_sls09.Value : 0) - (x.n_rtr09.HasValue ? x.n_rtr09.Value : 0) :
              //                  (DateTime.Now.Month == 10 ? (x.n_sls10.HasValue ? x.n_sls10.Value : 0) - (x.n_rtr10.HasValue ? x.n_rtr10.Value : 0) :
              //                  (DateTime.Now.Month == 11 ? (x.n_sls11.HasValue ? x.n_sls11.Value : 0) - (x.n_rtr11.HasValue ? x.n_rtr11.Value : 0) :
              //                  (DateTime.Now.Month == 12 ? (x.n_sls12.HasValue ? x.n_sls12.Value : 0) - (x.n_rtr12.HasValue ? x.n_rtr12.Value : 0) :
              //                  0))))))))))))
              //                )
              //              }).ToList();

              qryTempTPI = (from q in GlobalQuery.ViewAverageSales(db, date.AddMonths(-1))
                            join q1 in db.LG_Cusmas on q.Customer equals q1.c_cusno
                            where listItems.Contains(q.Item) && q1.c_gdg == gdgFrom
                            group q by q.Item into g
                            select new TemporaryPendingInfo()
                            {
                              Item = g.Key,
                              Quantity = g.Sum(y => (y.Sales - y.Retur)),
                            }).ToList();

              qryTempTPI.ForEach(delegate(TemporaryPendingInfo t)
              {
                listPOR = qryTempPOR.FindAll(x => x.c_iteno == t.Item);
                if ((listPOR != null) && (listPOR.Count > 0))
                {
                  for (nLoop = 0; nLoop < listPOR.Count; nLoop++)
                  {
                    tpor = listPOR[nLoop];
                    if (tpor != null)
                    {
                      tpor.n_avgsls = t.Quantity;
                    }
                  }

                  listPOR.Clear();
                }
              });

              qryTempTPI.Clear();

              #endregion

              #region Average Sales Divisi Principal

              qryTempTPI = (from sq in qryTempPOR
                            group sq by sq.c_kddivpri into g
                            select new TemporaryPendingInfo()
                            {
                              NoID = g.Key,
                              Quantity = g.Sum(x => x.n_avgsls * x.n_salpri)
                            }).ToList();

              

              qryTempTPI.ForEach(delegate(TemporaryPendingInfo t)
              {
                if (t.Quantity != 0)
                {
                  listPOR = qryTempPOR.FindAll(x => x.NoID == t.NoID);
                  if ((listPOR != null) && (listPOR.Count > 0))
                  {
                    for (nLoop = 0; nLoop < listPOR.Count; nLoop++)
                    {
                      tpor = listPOR[nLoop];
                      if (tpor != null)
                      {
                        tpor.n_avgslsdivpri = t.Quantity;
                      }
                    }

                    listPOR.Clear();
                  }
                }
              });

              qryTempTPI.Clear();

              #endregion

              #region Stok Pareto, Ideal, Order Deviasi

              //var ii = qryTempPOR.Where(x => x.c_iteno == "5290").ToList();

              decimal avgVar = 0;
              qryTempPOR.ForEach(delegate(TemporaryProcessOR t)
              {
                avgVar = (t.n_avgslsdivpri * t.n_variabel);

                #region Pareto

                t.n_pareto = (avgVar == 0 ? 0 : ((t.n_avgsls * t.n_salpri) / avgVar) * 100);

                #endregion

                #region Ideal

                if (t.n_pareto == 0)
                {
                  t.n_ideal = (t.n_avgsls * t.n_index);
                }
                else
                {
                  avgVar = ((t.n_avgsls * t.n_salpri) / avgVar);
                  if ((avgVar * 100) >= 100 && (t.n_idxp > 0))
                  {
                    t.n_ideal = (t.n_avgsls * t.n_idxp);
                  }
                  else if ((avgVar * 100) < 100 && (t.n_idxp > 0))
                  {
                    t.n_ideal = (t.n_avgsls * t.n_idxnp);
                  }
                }

                #endregion

                #region Order

                avgVar = (t.n_ideal - t.n_soh - t.n_sit + t.n_spacc);

                t.n_order = (avgVar < 0 ? 0 : avgVar);

                #endregion

                #region Deviasi

                t.n_deviasi = Math.Round((t.n_box == 0 ? t.n_order : t.n_order / t.n_box), 0);

                #endregion
              });

              #endregion

              #region Hapus Salah divisi prinsipal, SP Kosong, Quantity 0, SPAcc > SoH

              qryTempPOR.RemoveAll(delegate(TemporaryProcessOR t)
              {
                #region Calculate Sisa

                t.n_qty = (t.n_box * t.n_deviasi);

                t.Total = t.n_qty * t.n_salpri;
                #endregion

                #region Hapus yang tidak termasuk kode divisi prinsipal

                if (!string.IsNullOrEmpty(divPrinc))
                {
                  if (!t.c_kddivpri.Equals(divPrinc, StringComparison.OrdinalIgnoreCase))
                  {
                    return true;
                  }
                }

                #endregion

                #region SP Kosong atau Quantiy 0

                //if (t.n_qty <= 0)
                //{
                //  return true;
                //}

                //if (string.IsNullOrEmpty(t.NoID) || (t.Quantity == 0))
                //{
                //  return true;
                //}


                #endregion

                #region Old Coded

                //#region SPAcc > SoH

                //if (dicTotalSOH.ContainsKey(t.c_iteno))
                //{
                //  nTotalSOH = dicTotalSOH[t.c_iteno];
                //}
                //else
                //{
                //  nTotalSOH = 0;
                //  dicTotalSOH.Add(t.c_iteno, 0);
                //}

                //if (nTotalSOH > t.n_spacc)
                //{
                //  nTotalSOH -= t.n_spacc;

                //  dicTotalSOH[t.c_iteno] = nTotalSOH;

                //  return true;
                //}

                //#region Old Coded

                ////if (nTotalSOH > t.n_spacc)
                ////{
                ////  nTotalSOH -= t.n_spacc;

                ////  return true;
                ////}
                ////else if (t.n_soh >= t.n_spacc)
                ////{
                ////  return true;
                ////}

                //#endregion

                //#endregion

                #endregion

                return false;
              });

              #endregion

              listItems.Clear();
              qryDataTPI.Clear();
              //dicTotalSOH.Clear();

              nCount = qryTempPOR.Count();

              if (nCount > 0)
              {
                //dic.Add(Constant.DEFAULT_NAMING_RECORDS, qryTempPOR.ToArray());

                if ((limit == -1) || allQuery)
                {
                  dic.Add(Constant.DEFAULT_NAMING_RECORDS, qryTempPOR.ToList());
                }
                else
                {
                  dic.Add(Constant.DEFAULT_NAMING_RECORDS, qryTempPOR.Skip(start).Take(limit).ToList());
                }
              }

              dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

              dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

              qryTempPOR.Clear();
            }
            break;

          #endregion

          #endregion

          #region RN

          #region MODEL_PROCESS_QUERY_MULTIPLE_PROCESS_DOKHUSUS

          case Constant.MODEL_PROCESS_QUERY_MULTIPLE_PROCESS_DOKHUSUS:
            {
              string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);
              string doKhusus = (parameters.ContainsKey("doKhusus") ? (string)((Functionals.ParameterParser)parameters["doKhusus"]).Value : string.Empty);

              var qry = (from q in db.LG_DOPHs
                         join q1 in db.LG_DOPDs on new { q.c_nosup, q.c_dono } equals new { q1.c_nosup, q1.c_dono }
                         join q2 in db.LG_DatSupAutos on q.c_nosup equals q2.c_nosup
                         join q3 in db.LG_POHs on new { q.c_nosup, q1.c_pono } equals new { q3.c_nosup, q3.c_pono }
                         join q4 in db.LG_POD1s on new { q3.c_gdg, q3.c_pono, q1.c_iteno } equals new { q4.c_gdg, q4.c_pono, q4.c_iteno }
                         join q5 in db.FA_MasItms on q1.c_iteno equals q5.c_iteno
                         join q6 in db.MsTransDs on new { q1.c_type, c_portal = '3', c_notrans = "06" } equals new { q6.c_type, q6.c_portal, q6.c_notrans } into q_6
                         from qMTD in q_6.DefaultIfEmpty()
                         where (q.c_nosup == nosup) && (q.c_dono == doKhusus)
                           && (q2.l_rn == true)
                         select new TemporaryProsesRN()
                         {
                           c_iteno = q1.c_iteno,
                           v_itemdesc = q5.v_itnam,
                           c_refno = q1.c_pono,
                           c_batch = q1.c_batch,
                           d_batchexpired = q1.d_expired.Value.ToString(),//(q1.d_expired.HasValue ? q1.d_expired.Value.ToString() : Functionals.StandardSqlDateTime).ToString(),
                           c_type = q1.c_type,
                           v_typedesc = qMTD.v_ket,
                           n_gqty = ((q1.n_qty_sisa.HasValue ? q1.n_qty_sisa.Value : 0) > 0
                                 ? (((q1.n_qty_sisa.HasValue ? q1.n_qty_sisa.Value : 0) > (q4.n_sisa.HasValue ? q4.n_sisa.Value : 0)) ?
                                   (q4.n_sisa.HasValue ? q4.n_sisa.Value : 0) : (q1.n_qty_sisa.HasValue ? q1.n_qty_sisa.Value : 0))
                                 : 0),
                           l_new = true
                         }).AsQueryable();

              #region Old Coded

              //var qry = (from q in db.LG_DOPDs
              //           join q1 in db.LG_POD1s on new { q.c_pono, q.c_iteno } equals new { q1.c_pono, q1.c_iteno }
              //           join q2 in db.LG_POHs on new { q1.c_pono, q.c_nosup } equals new { q2.c_pono, q2.c_nosup }
              //           join q3 in db.LG_DatSupAutos on q.c_nosup equals q3.c_nosup
              //           join q4 in db.FA_MasItms on q.c_iteno equals q4.c_iteno
              //           join q5 in db.MsTransDs on new { q.c_type, c_portal = '3', c_notrans = "06" } equals new { q5.c_type, q5.c_portal, q5.c_notrans } into q_6
              //           from qMTD in q_6.DefaultIfEmpty()
              //           //join qSQ in
              //           //  (from sq1 in db.LG_RNHs
              //           //   join sq2 in db.LG_RND1s on new { sq1.c_gdg, sq1.c_rnno } equals new { sq2.c_gdg, sq2.c_rnno }
              //           //   where (sq1.l_delete == null || sq1.l_delete == false)
              //           //   group new { sq1, sq2 } by new { sq1.c_gdg, sq1.c_from, sq1.c_rnno, sq1.c_dono, sq2.c_iteno } into g
              //           //   select new
              //           //   {
              //           //     g.Key.c_gdg,
              //           //     c_nosup = g.Key.c_from,
              //           //     g.Key.c_rnno,
              //           //     g.Key.c_dono,
              //           //     g.Key.c_iteno,
              //           //     n_gqty = g.Sum(x => x.sq2.n_gqty),
              //           //     n_bqty = g.Sum(x => x.sq2.n_bqty)
              //           //   }) on new { q.c_nosup, q.c_dono, q.c_iteno } equals new { qSQ.c_nosup, qSQ.c_dono, qSQ.c_iteno } into q_SQ
              //           //from qSQRN in q_SQ.DefaultIfEmpty()
              //           where q.c_nosup == nosup && q.c_dono == doKhusus && q.n_qty > 0
              //             && ((q2.l_delete.HasValue ? q2.l_delete.Value : false) == false)
              //             //&& ((((q.n_qty > 0) ? ((q1.n_sisa > 0) ? ((q.n_qty > q1.n_sisa) ? q1.n_sisa : q.n_qty) : 0) : 0) - (qSQRN.n_gqty.HasValue ? qSQRN.n_gqty.Value : 0)) > 0)
              //             && (q.n_qty_sisa > 0)
              //             && (q3.l_rn == true)
              //           select new TemporaryProsesRN()
              //           {
              //             c_iteno = q.c_iteno,
              //             v_itemdesc = q4.v_itnam,
              //             c_refno = q2.c_pono,
              //             c_batch = q.c_batch,
              //             d_batchexpired = (q.d_expired.HasValue ? q.d_expired.Value : Functionals.StandardSqlDateTime),
              //             c_type = q.c_type,
              //             v_typedesc = qMTD.v_ket,
              //             n_gqty = ((q.n_qty_sisa.HasValue ? q.n_qty_sisa.Value : 0) > 0 
              //                ?  (((q.n_qty_sisa.HasValue ? q.n_qty_sisa.Value : 0) > (q1.n_sisa.HasValue ? q1.n_sisa.Value : 0)) ?
              //                      (q1.n_sisa.HasValue ? q1.n_sisa.Value : 0) : (q.n_qty_sisa.HasValue ? q.n_qty_sisa.Value : 0))
              //                : 0),
              //             l_new = true
              //           }).Distinct().AsQueryable();

              ////qry = qry.Where(x => x.n_gqty > 0).AsQueryable();

              #endregion

              if ((parameters != null) && (parameters.Count > 0))
              {
                

                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                {
                  if (kvp.Value.IsCondition)
                  {
                    if (kvp.Value.IsLike)
                    {
                      paternLike = kvp.Value.Value.ToString();
                      qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                    }
                    else if (kvp.Value.IsBetween)
                    {

                    }
                    else
                    {
                      qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                    }
                  }
                }
              }

              Logger.WriteLine(qry.Provider.ToString());

              nCount = qry.Count();
              if (nCount > 0)
              {
                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                {
                  qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                }

                if ((limit == -1) || allQuery)
                {
                  dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                }
                else
                {
                  dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                }
              }

              dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

              dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
            }
            break;

          #endregion

          #region MODEL_PROCESS_QUERY_MULTIPLE_PROCESS_RNTRANFER

          case Constant.MODEL_PROCESS_QUERY_MULTIPLE_PROCESS_RNTRANFER:
            {
              string sjID = (parameters.ContainsKey("suratID") ? (string)((Functionals.ParameterParser)parameters["suratID"]).Value : string.Empty);
              char gdg = (parameters.ContainsKey("gudang") ? (char)((Functionals.ParameterParser)parameters["gudang"]).Value : char.MinValue);
              //char gdgFrom = char.MinValue;
              string pinCode = (parameters.ContainsKey("pinCode") ? (string)((Functionals.ParameterParser)parameters["pinCode"]).Value : string.Empty);

              #region Old Coded

              //List<SJ_FRMT_HEADER> listHdr = new List<SJ_FRMT_HEADER>();

              //#region RN

              //var qry1 = (from q in
              //              (from sq in db.LG_SJHs
              //               join sq1 in db.LG_SJD2s on new { sq.c_gdg, sq.c_sjno } equals new { sq1.c_gdg, sq1.c_sjno }
              //               join sq2 in db.LG_RNHs on new { c_gdg = gdg, sq1.c_rnno } equals new { sq2.c_gdg, sq2.c_rnno }
              //               where (sq.c_gdg2 == gdg) &&
              //                 (sq.c_sjno == sjID) &&
              //                 (sq1.c_rnno.Substring(0, 2).ToUpper() == "RN") &&
              //                 (sq.l_delete == null || sq.l_delete == false) &&
              //                 (sq.l_status == false) &&
              //                 (sq.c_pin == pinCode) &&
              //                 (sq2.l_delete == null || sq2.l_delete == false)
              //               group new { sq, sq1, sq2 } by new
              //               {
              //                 sq.c_gdg2,
              //                 sq2.c_rnno,
              //                 sq2.d_rndate,
              //                 sq2.c_dono,
              //                 sq2.d_dodate,
              //                 sq2.c_from,
              //                 sq2.v_ket,
              //                 sq2.l_float,
              //                 sq2.l_print,
              //                 sq2.l_status,
              //                 sq2.n_bea,
              //                 sq1.c_iteno,
              //                 sq1.c_batch
              //               } into g
              //               select new
              //               {
              //                 c_gdg = (g.Key.c_gdg2.HasValue ? g.Key.c_gdg2.Value : '?'),
              //                 g.Key.c_rnno,
              //                 g.Key.d_rndate,
              //                 g.Key.c_dono,
              //                 g.Key.d_dodate,
              //                 g.Key.c_from,
              //                 g.Key.v_ket,
              //                 g.Key.l_float,
              //                 g.Key.l_print,
              //                 g.Key.l_status,
              //                 g.Key.n_bea,
              //                 g.Key.c_iteno,
              //                 g.Key.c_batch,
              //                 n_sum_gqty = g.Sum(x => x.sq1.n_gqty),
              //                 n_sum_bqty = g.Sum(x => x.sq1.n_bqty)
              //               }
              //                )
              //            join q1 in db.LG_RNHs on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno } into q_1
              //            from qRNNE in q_1.DefaultIfEmpty()
              //            join q2 in db.LG_RND1s on new { q.c_gdg, q.c_rnno, q.c_iteno, q.c_batch } equals new { q2.c_gdg, q2.c_rnno, q2.c_iteno, q2.c_batch } into q_2
              //            from qRNDNE in q_2.DefaultIfEmpty()
              //            join q3 in db.FA_MasItms on q.c_iteno equals q3.c_iteno into q_3
              //            from qItm in q_3.DefaultIfEmpty()
              //            join q4 in db.LG_MsGudangs on q.c_gdg equals q4.c_gdg into q_4
              //            from qGdg in q_4.DefaultIfEmpty()
              //            select new SJ_FRMT_HEADER()
              //            {
              //              headerExist = (qRNNE.c_rnno != null),
              //              detailExist = (qRNDNE.c_iteno != null),
              //              c_addtno = q.c_dono,
              //              c_gdg = q.c_gdg,
              //              c_nosup = q.c_from,
              //              c_refno = q.c_rnno,
              //              c_type = "05",
              //              d_addtdate = q.d_dodate,
              //              d_ref = q.d_rndate,
              //              l_float = (q.l_float.HasValue ? q.l_float.Value : false),
              //              l_print = (q.l_print.HasValue ? q.l_print.Value : false),
              //              l_status = (q.l_status.HasValue ? q.l_status.Value : false),
              //              n_bea = (q.n_bea.HasValue ? q.n_bea.Value : 0),
              //              //user = nipEntry,
              //              v_ket = q.v_ket,
              //              c_iteno = q.c_iteno,
              //              c_batch = q.c_batch,
              //              n_bqty = (q.n_sum_bqty.HasValue ? q.n_sum_bqty.Value : 0),
              //              n_gqty = (q.n_sum_gqty.HasValue ? q.n_sum_gqty.Value : 0),
              //              v_item_desc = qItm.v_itnam,
              //              v_gdgdesc = qGdg.v_gdgdesc
              //            }).Distinct().ToList();

              ////qry1.Dump("Hasil");

              //if ((qry1 != null) && (qry1.Count > 0))
              //{
              //  listHdr.AddRange(qry1.ToArray());
              //}

              //#endregion

              //#region Combo

              //var qry2 = (from q in
              //              (from sq in db.LG_SJHs
              //               join sq1 in db.LG_SJD2s on new { sq.c_gdg, sq.c_sjno } equals new { sq1.c_gdg, sq1.c_sjno }
              //               join sq2 in db.LG_ComboHs on new { sq1.c_gdg, c_combono = sq1.c_rnno, sq1.c_iteno, sq1.c_batch } equals new { sq2.c_gdg, sq2.c_combono, sq2.c_iteno, sq2.c_batch }
              //               where (sq.c_gdg2 == gdg) &&
              //                 (sq.c_sjno == sjID) &&
              //                 (sq1.c_rnno.Substring(0, 2).ToUpper() != "RN") &&
              //                 (sq.l_delete == null || sq.l_delete == false) &&
              //                 (sq.l_status == false) &&
              //                 (sq.c_pin == pinCode) &&
              //                 (sq2.l_delete == null || sq2.l_delete == false)
              //               group new { sq, sq1, sq2 } by new
              //               {
              //                 sq.c_gdg2,
              //                 sq1.c_rnno,
              //                 sq2.d_combodate,
              //                 sq2.c_memono,
              //                 sq1.c_iteno,
              //                 sq1.c_batch,
              //                 sq2.n_acc,
              //                 sq2.v_ket
              //               } into g
              //               select new
              //               {
              //                 c_gdg = (g.Key.c_gdg2.HasValue ? g.Key.c_gdg2.Value : char.MinValue),
              //                 c_combono = g.Key.c_rnno,
              //                 g.Key.d_combodate,
              //                 g.Key.c_memono,
              //                 g.Key.v_ket,
              //                 g.Key.c_iteno,
              //                 g.Key.c_batch,
              //                 g.Key.n_acc,
              //                 n_sum_gqty = g.Sum(x => x.sq1.n_gqty),
              //                 n_sum_bqty = g.Sum(x => x.sq1.n_bqty)
              //               }
              //                )
              //            join q1 in db.LG_ComboHs on new { q.c_gdg, q.c_combono, q.c_iteno, q.c_batch } equals new { q1.c_gdg, q1.c_combono, q1.c_iteno, q1.c_batch } into q_1
              //            from qCBNE in q_1.DefaultIfEmpty()
              //            join q2 in db.FA_MasItms on q.c_iteno equals q2.c_iteno into q_2
              //            from qItm in q_2.DefaultIfEmpty()
              //            join q3 in db.LG_MsGudangs on q.c_gdg equals q3.c_gdg into q_3
              //            from qGdg in q_3.DefaultIfEmpty()
              //            //where (qCBNE.C_combono == null)
              //            select new SJ_FRMT_HEADER()
              //            {
              //              headerExist = (qCBNE.c_combono != null),
              //              detailExist = (qCBNE.c_combono != null),
              //              c_gdg = q.c_gdg,
              //              c_type = "05",
              //              c_refno = q.c_combono,
              //              d_ref = q.d_combodate,
              //              c_addtno = q.c_memono,
              //              v_ket = q.v_ket,
              //              //user = nipEntry,
              //              c_iteno = q.c_iteno,
              //              c_batch = q.c_batch,
              //              n_acc = (q.n_acc.HasValue ? q.n_acc.Value : 0),
              //              n_bqty = (q.n_sum_bqty.HasValue ? q.n_sum_bqty.Value : 0),
              //              n_gqty = (q.n_sum_gqty.HasValue ? q.n_sum_gqty.Value : 0),
              //              v_item_desc = qItm.v_itnam,
              //              v_gdgdesc = qGdg.v_gdgdesc
              //            }).Distinct().ToList();

              ////qry2.Dump("Hasil");

              //if ((qry2 != null) && (qry2.Count > 0))
              //{
              //  listHdr.AddRange(qry2.ToArray());
              //}

              //#endregion

              //nCount = listHdr.Count();

              #endregion

              //List<SJ_RN_FRMT_DATA> listSJData = (from q in db.LG_SJHs
              //                                    join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
              //                                    join q2 in db.LG_SJD2s on new { q.c_gdg, q.c_sjno } equals new { q2.c_gdg, q2.c_sjno }
              //                                    join q3 in db.FA_MasItms on q1.c_iteno equals q3.c_iteno
              //                                    where (q.c_sjno == sjID)
              //                                      && (q.l_status == false) && (q.l_confirm == true) && (q.l_print == true)
              //                                      && (q.c_pin.Trim().ToLower() == pinCode.Trim().ToLower())
              //                                      && ((q2.n_bsisa > 0) || (q2.n_gsisa > 0))
              //                                      && ((q2.n_gqty > 0) || (q2.n_bqty > 0))
              //                                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
              //                                    group new { q1, q3 } by new { q1.c_gdg, q1.c_sjno, q1.c_iteno, q1.c_batch, q3.v_itnam } into g
              //                                    select new SJ_RN_FRMT_DATA()
              //                                    {
              //                                      //c_gdg = g.Key.c_gdg,
              //                                      //c_iteno = (g.Key.c_iteno == null ? string.Empty : g.Key.c_iteno.Trim()),
              //                                      //v_item_desc = (g.Key.v_itnam == null ? string.Empty : g.Key.v_itnam.Trim()),
              //                                      //c_batch = (g.Key.c_batch == null ? string.Empty : g.Key.c_batch.Trim()),
              //                                      //n_bqty = g.Sum(x => (x.q1.n_bqty.HasValue ? x.q1.n_bqty.Value : 0)),
              //                                      //n_gqty = g.Sum(x => (x.q1.n_gqty.HasValue ? x.q1.n_gqty.Value : 0)),
              //                                      c_gdg = g.Key.c_gdg,
              //                                      c_iteno = g.Key.c_iteno,
              //                                      v_item_desc = g.Key.v_itnam,
              //                                      c_batch = g.Key.c_batch,
              //                                      n_bqty = (q2.n_bsisa.HasValue ? q2.n_bsisa.Value : 0),
              //                                      n_gqty = (q2.n_gsisa.HasValue ? q2.n_gsisa.Value : 0),
              //                                      c_addtno = (q2.c_spgno == null ? string.Empty : q2.c_spgno.Trim()),
              //                                      c_refno = (q2.c_rnno == null ? string.Empty : q2.c_rnno.Trim())
              //                                    }).Distinct().ToList();


              #region Original

              List<SJ_RN_FRMT_DATA> listSJData = (from q in db.LG_SJHs
                                                  join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                                                  join q2 in db.LG_SJD2s on new { q.c_gdg, q.c_sjno, q1.c_iteno, q1.c_batch } equals new { q2.c_gdg, q2.c_sjno, q2.c_iteno, q2.c_batch }
                                                  join q3 in db.FA_MasItms on q1.c_iteno equals q3.c_iteno
                                                  where (q.c_sjno == sjID)
                                                    && (q.l_status == false) && (q.l_confirm == true) && (q.l_print == true)
                                                    && (q.c_pin.Trim().ToLower() == pinCode.Trim().ToLower())
                                                    && ((q2.n_bsisa > 0) || (q2.n_gsisa > 0))
                                                    && ((q2.n_gqty > 0) || (q2.n_bqty > 0))
                                                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                                  //group new { q1, q3 } by new { q1.c_gdg, q1.c_sjno, q1.c_iteno, q1.c_batch, q3.v_itnam } into g
                                                  select new SJ_RN_FRMT_DATA()
                                                  {
                                                    //c_gdg = g.Key.c_gdg,
                                                    //c_iteno = (g.Key.c_iteno == null ? string.Empty : g.Key.c_iteno.Trim()),
                                                    //v_item_desc = (g.Key.v_itnam == null ? string.Empty : g.Key.v_itnam.Trim()),
                                                    //c_batch = (g.Key.c_batch == null ? string.Empty : g.Key.c_batch.Trim()),
                                                    //n_bqty = g.Sum(x => (x.q1.n_bqty.HasValue ? x.q1.n_bqty.Value : 0)),
                                                    //n_gqty = g.Sum(x => (x.q1.n_gqty.HasValue ? x.q1.n_gqty.Value : 0)),
                                                    c_gdg = (q.c_gdg2.HasValue ? q.c_gdg2.Value : char.MinValue),
                                                    c_iteno = q2.c_iteno,
                                                    v_item_desc = q3.v_itnam,
                                                    c_batch = q2.c_batch,
                                                    n_bqty = (q2.n_bsisa.HasValue ? q2.n_bsisa.Value : 0),
                                                    n_gqty = (q2.n_gsisa.HasValue ? q2.n_gsisa.Value : 0),
                                                    c_addtno = (q2.c_spgno == null ? string.Empty : q2.c_spgno.Trim()),
                                                    c_refno = (q2.c_rnno == null ? string.Empty : q2.c_rnno.Trim())
                                                  }).Distinct().ToList();

              #endregion

              nCount = listSJData.Count;

              if (nCount > 0)
              {
                dic.Add(Constant.DEFAULT_NAMING_RECORDS, listSJData.ToArray());
              }

              listSJData.Clear();

              dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

              dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
            }
            break;

          #endregion

          #endregion

          #region Combo

          #region MODEL_PROCESS_QUERY_MULTIPLE_MEMO_PROCESS_ITEM_MEMOCOMBO

          case Constant.MODEL_PROCESS_QUERY_MULTIPLE_MEMO_PROCESS_ITEM_MEMOCOMBO:
            {
              char gdg = (parameters.ContainsKey("gudang") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gudang"]).Value) : char.MinValue);
              string comboItem = (parameters.ContainsKey("itemCombo") ? (string)((Functionals.ParameterParser)parameters["itemCombo"]).Value : string.Empty);
              decimal nQty = (parameters.ContainsKey("Quantity") ? (decimal)((Functionals.ParameterParser)parameters["Quantity"]).Value : 0m);

              int nLoop = 0,
                nLoopC = 0,
                nTotalItems = 0;

              List<Temporary_COMBOITEM_AUTO> lstTmpCIA = null;
              Temporary_COMBOITEM_PACKAGE lgcip = null;
              Temporary_COMBOITEM_AUTO lgcia = null;
              decimal reqSumQty = 0;

              List<Temporary_COMBOITEM_AUTO> listCIA = new List<Temporary_COMBOITEM_AUTO>();

              List<Temporary_COMBOITEM_PACKAGE> lstCmbItm = (from q in db.FA_Combos
                                                             where q.c_combo == comboItem
                                                             select new Temporary_COMBOITEM_PACKAGE()
                                                             {
                                                               c_iteno = q.c_iteno,
                                                               n_package = (q.n_qty.HasValue ? q.n_qty.Value : 0)
                                                             }).Distinct().ToList();

              var qryList = (from q in db.FA_Combos
                             join q1 in
                               (from sq in GlobalQuery.ViewStockLite(db, gdg)
                                where sq.c_gdg == gdg && sq.c_table == "RN"
                                group sq by new { sq.c_iteno, sq.c_batch } into g
                                select new
                                {
                                  g.Key.c_batch,
                                  g.Key.c_iteno,
                                  n_qty = g.Sum(x => x.n_gsisa)
                                }) on q.c_iteno equals q1.c_iteno
                             join q2 in db.FA_MasItms on q.c_iteno equals q2.c_iteno
                             join q3 in db.LG_MsBatches on new { q.c_iteno, q1.c_batch } equals new { q3.c_iteno, q3.c_batch } into q_3
                             from qBat in q_3.DefaultIfEmpty()
                             where q.c_combo == comboItem && (q1.n_qty > 0)
                             orderby q.c_iteno ascending, qBat.d_expired descending
                             select new Temporary_COMBOITEM_AUTO()
                             {
                               c_iteno = q.c_iteno,
                               v_itemdesc = q2.v_itnam,
                               c_batch = q1.c_batch,
                               n_qty = q1.n_qty,
                               d_expired = qBat.d_expired,
                             }).Distinct().ToList();

              for (nLoop = 0; nLoop < lstCmbItm.Count; nLoop++)
              {
                lgcip = lstCmbItm[nLoop];
                if ((lgcip != null) && (lgcip.n_package > 0))
                {
                  reqSumQty = (lgcip.n_package * nQty);

                  lstTmpCIA = qryList.Where(x => x.c_iteno == lgcip.c_iteno).ToList();

                  if ((lstTmpCIA != null) && (lstTmpCIA.Count > 0))
                  {
                    for (nLoopC = 0; nLoopC < lstTmpCIA.Count; nLoopC++)
                    {
                      lgcia = lstTmpCIA[nLoopC];

                      if (reqSumQty > lgcia.n_qty)
                      {
                        listCIA.Add(new Temporary_COMBOITEM_AUTO()
                        {
                          c_batch = lgcia.c_batch,
                          c_iteno = lgcia.c_iteno,
                          d_expired = lgcia.d_expired,
                          n_pack = lgcip.n_package,
                          n_qty = lgcia.n_qty,
                          n_qty_expected = lgcia.n_qty,
                          v_itemdesc = lgcia.v_itemdesc,
                        });

                        reqSumQty -= lgcia.n_qty;
                      }
                      else
                      {
                        listCIA.Add(new Temporary_COMBOITEM_AUTO()
                        {
                          c_batch = lgcia.c_batch,
                          c_iteno = lgcia.c_iteno,
                          d_expired = lgcia.d_expired,
                          n_pack = lgcip.n_package,
                          n_qty = reqSumQty,
                          n_qty_expected = reqSumQty,
                          v_itemdesc = lgcia.v_itemdesc,
                        });

                        reqSumQty = 0;
                      }

                      if (reqSumQty <= 0.00m)
                      {
                        break;
                      }
                    }

                    if (reqSumQty == 0.00m)
                    {
                      nTotalItems++;
                    }

                    lstTmpCIA.Clear();
                  }
                }
              }

              nCount = listCIA.Count();

              if (qryList != null)
              {
                qryList.Clear();
              }

              if (nTotalItems != lstCmbItm.Count)
              {
                dic.Add(Constant.DEFAULT_NAMING_RECORDS, null);

                dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, 0);

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);
              }
              else
              {
                if (nCount > 0)
                {
                  dic.Add(Constant.DEFAULT_NAMING_RECORDS, listCIA.ToArray());
                }

                dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
              }

              lstCmbItm.Clear();

              listCIA.Clear();
            }
            break;

          #endregion

          #endregion

          #region Claim

          case Constant.MODEL_PROCESS_QUERY_MULTIPLE_PROCESS_CLAIM:
            {
              decimal Tahun = Convert.ToDecimal((parameters.ContainsKey("Tahun") ? (string)((Functionals.ParameterParser)parameters["Tahun"]).Value : "0"));
              decimal Bulan = Convert.ToDecimal((parameters.ContainsKey("Bulan") ? (string)((Functionals.ParameterParser)parameters["Bulan"]).Value : "0"));
              string Supplier = (parameters.ContainsKey("Supplier") ? (string)((Functionals.ParameterParser)parameters["Supplier"]).Value : string.Empty);
              string divSupplier = (parameters.ContainsKey("divSupplier") ? (string)((Functionals.ParameterParser)parameters["divSupplier"]).Value : string.Empty);

              DateTime startDate = new DateTime(int.Parse(Tahun.ToString()), int.Parse(Bulan.ToString()), 1);
              DateTime endDate = startDate.AddMonths(1).AddDays(-1);

              List<LG_Claim> lstClaim = new List<LG_Claim>(),
                lstClaim1 = new List<LG_Claim>(), 
                lstClaim2 = new List<LG_Claim>();
              

              var sales = (from q in db.SD_Aremas
                           join q1 in db.SD_Arebals on new { q.c_kdcab, q.c_invno } equals new { q1.c_kdcab, q1.c_invno }
                           join itm in db.FA_MasItms on q1.c_iteno equals itm.c_iteno
                           join cus in db.LG_Cusmas on q.c_kdcab equals cus.c_cab
                           join qDiv in db.FA_Divpris on q1.c_iteno equals qDiv.c_iteno
                           where itm.c_nosup == Supplier
                           && q.d_invdate >= startDate && q.d_invdate <= endDate
                           && q1.n_qtybon != 0 && (string.IsNullOrEmpty(divSupplier) ? true : (qDiv.c_kddivpri == divSupplier)) 
                           group new { itm, q, q1 } by new { itm.c_nosup, cus.c_cusno, cus.v_cunam, itm.c_iteno, itm.v_itnam, itm.n_salpri } into gSum
                           select new
                           {
                             gSum.Key.c_nosup,
                             gSum.Key.c_cusno,
                             gSum.Key.v_cunam,
                             gSum.Key.c_iteno,
                             gSum.Key.v_itnam,
                             gSum.Key.n_salpri,
                             n_gBonus = gSum.Sum(x => x.q1.n_qtybon.HasValue ? x.q1.n_qtybon.Value : 0)
                           }).Distinct().AsQueryable();

              var lstSales = sales.ToList();

              //int s = sales.Count();

              var Retur = (from q in db.SD_Aremas
                           join q1 in db.SD_RetCusDs on new { q.c_kdcab, q.c_invno } equals new { q1.c_kdcab, q1.c_invno }
                           join itm in db.FA_MasItms on q1.c_iteno equals itm.c_iteno
                           join cus in db.LG_Cusmas on q.c_kdcab equals cus.c_cab
                           join qDiv in db.FA_Divpris on q1.c_iteno equals qDiv.c_iteno
                           where Convert.ToDateTime(q.d_invdate).Year == Tahun &&
                           Convert.ToDateTime(q.d_invdate).Month == Bulan && itm.c_nosup == Supplier
                           && (q1.n_qtybonb != 0 || q1.n_qtybong != 0) && (string.IsNullOrEmpty(divSupplier) ? true : (qDiv.c_kddivpri == divSupplier))
                           group new { itm, q, q1 } by new { itm.c_nosup, cus.c_cusno, cus.v_cunam, itm.c_iteno, itm.v_itnam, itm.n_salpri } into gSum
                           select new
                           {
                             gSum.Key.c_nosup,
                             gSum.Key.c_cusno,
                             gSum.Key.v_cunam,
                             gSum.Key.c_iteno,
                             gSum.Key.v_itnam,
                             gSum.Key.n_salpri,
                             n_gBonus = gSum.Sum(x => x.q1.n_qtybong.HasValue ? x.q1.n_qtybong.Value : 0),
                             n_bBonus = gSum.Sum(x => x.q1.n_qtybonb.HasValue ? x.q1.n_qtybonb.Value : 0),
                           }).AsQueryable();

            var lstRetur = Retur.ToList();

              //int t = Retur.Count();

              lstClaim = (from q in lstSales
                           join q1 in lstRetur on new { q.c_cusno, q.c_iteno, q.c_nosup } equals new { q1.c_cusno, q1.c_iteno, q1.c_nosup } into gLeftJoin
                           from qLeft in gLeftJoin.DefaultIfEmpty()
                           select new LG_Claim
                           {
                             c_cusno = (qLeft == null ? q.c_cusno : qLeft.c_cusno),
                             v_cunam = (qLeft == null ? q.v_cunam : qLeft.v_cunam),
                             c_iteno = (qLeft == null ? q.c_iteno : qLeft.c_iteno),
                             c_nosup = (qLeft == null ? q.c_nosup : qLeft.c_nosup),
                             v_itnam = (qLeft == null ? q.v_itnam : qLeft.v_itnam),
                             n_salpri = (qLeft == null ? q.n_salpri.Value : (qLeft.n_salpri.HasValue ? qLeft.n_salpri.Value : q.n_salpri.Value)),
                             qty = q.n_gBonus,  //(qLeft == null ? (q.n_gBonus) : 0),   MODIFIED BY HANDRY 29 SEPT 2015
                             bret = (qLeft == null ? 0 : qLeft.n_bBonus),
                             gret = (qLeft == null ? 0 : qLeft.n_gBonus),
                           }).ToList();
              lstClaim2.AddRange(lstClaim);

              lstClaim1 = (from q in lstRetur
                           join q1 in lstSales on new { q.c_cusno, q.c_iteno, q.c_nosup } equals new { q1.c_cusno, q1.c_iteno, q1.c_nosup } into gLeftJoin
                           from qLeft in gLeftJoin.DefaultIfEmpty()
                           where qLeft == null
                           select new LG_Claim
                           {
                             c_cusno = (qLeft == null ? q.c_cusno : qLeft.c_cusno),
                             v_cunam = (qLeft == null ? q.c_cusno : qLeft.v_cunam),
                             c_iteno = (qLeft == null ? q.c_iteno : qLeft.c_iteno),
                             c_nosup = (qLeft == null ? q.c_nosup : qLeft.c_nosup),
                             v_itnam = (qLeft == null ? q.v_itnam : qLeft.v_itnam),
                             n_salpri = (qLeft == null ? q.n_salpri.Value : (qLeft.n_salpri.HasValue ? qLeft.n_salpri.Value : q.n_salpri.Value)),
                             qty = (qLeft == null ? 0 : qLeft.n_gBonus),
                             bret = (qLeft == null ? q.n_bBonus : 0),
                             gret = (qLeft == null ? q.n_gBonus : 0),
                           }).ToList();

              lstClaim2.AddRange(lstClaim1);

              nCount = lstClaim2.Count();

              if (nCount > 0)
              {
                dic.Add(Constant.DEFAULT_NAMING_RECORDS, lstClaim2.ToArray());
              }

              dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

              dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

              lstClaim2.Clear();
            }
            break;

          #endregion

          #region SJ Auto

          case Constant.MODEL_COMMON_QUERY_PROSES_AUTO_SJ:
            {
                string noRn = (parameters.ContainsKey("noRn") ? (string)((Functionals.ParameterParser)parameters["noRn"]).Value : string.Empty);
                char gdg = (parameters.ContainsKey("Gudang") ? Convert.ToChar(((Functionals.ParameterParser)parameters["Gudang"]).Value) : char.MinValue);
                string Principal = (parameters.ContainsKey("Principal") ? (string)((Functionals.ParameterParser)parameters["Principal"]).Value : string.Empty);
                //string Status = "01";
                //if (gdg.ToString() == "6")
                //{
                //    gdg = '1';
                //}

                PL_AUTO_RN porn = null;
                //PL_AUTO_SP posp = null;

                //List<PL_AUTO_RN> lstPLAuto = null;
                //List<PL_AUTO_SP> lstPLAutoSP = null;
                //List<PL_AUTO_SP> lstPLAutoSPCopy = null;


                SJ_AUTO_RN sj = null;
                List<SJ_AUTO_RN> sjd = null;

                List<string> lstRef = null;

                List<TempProsesPLAuto> lstProsesPLA = new List<TempProsesPLAuto>();

                int nLoop = 0, nLen = 0,
                  nPos = 0;

                sjd = (from q in db.LG_RND1s
                       join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                       join q2 in db.LG_RNHs on new {q.c_gdg, q.c_rnno} equals new {q2.c_gdg, q2.c_rnno}
                       where q.c_gdg == gdg && q.c_rnno == noRn && q2.c_from == Principal
                       select new SJ_AUTO_RN()
                       {
                           c_gdg = q.c_gdg,
                           c_rnno = (q.c_rnno ?? string.Empty).Trim(),
                           c_iteno = (q.c_iteno ?? string.Empty).Trim(),
                           v_itnam = (q1.v_itnam ?? string.Empty).Trim(),
                           c_batch = (q.c_batch ?? string.Empty).Trim(),
                           n_gqty = (q.n_gsisa.HasValue ? q.n_gsisa.Value : 0),
                           n_bqty = 0,
                           n_booked = (q.n_gsisa.HasValue ? q.n_gsisa.Value : 0),
                           n_booked_bad = 0,
                           l_modified = false,
                           l_new = true,
                           l_view = false
                       }).Distinct().ToList();

                //lstPLAuto = (from q in db.LG_RNHs
                //             join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                //             join q2 in db.LG_RND2s on new { q.c_gdg, q.c_rnno, q1.c_iteno, q1.c_batch } equals new { q2.c_gdg, q2.c_rnno, q2.c_iteno, q2.c_batch }
                //             join q3 in db.FA_MasItms on q1.c_iteno equals q3.c_iteno
                //             where (q.c_gdg == gdg) && (q.c_type == "01") && (q.l_khusus == true)
                //               && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false) && q.c_rnno == noRn
                //             select new PL_AUTO_RN()
                //             {
                //                 c_gdg = q.c_gdg,
                //                 c_rnno = (q.c_rnno ?? string.Empty).Trim(),
                //                 c_iteno = (q1.c_iteno ?? string.Empty).Trim(),
                //                 v_itnam = (q3.v_itnam ?? string.Empty).Trim(),
                //                 c_batch = (q1.c_batch ?? string.Empty).Trim(),
                //                 qtyRN = (q1.n_gsisa.HasValue ? q1.n_gsisa.Value : 0),
                //                 //qtyRN = (q2.n_gqty.HasValue ? q2.n_gqty.Value : 0),
                //                 c_noref = (q2.c_no ?? string.Empty).Trim()
                //             }).Distinct().ToList();

                ////lstPLAuto.Dump("Hasil");

                //lstRef = lstPLAuto.Where(x => (!string.IsNullOrEmpty(x.c_noref))).GroupBy(x => x.c_noref).Select(y => y.Key.Trim()).ToList();

                //if (lstRef.Count > 0)
                //{
                //    #region Baca SP

                //    var lstPLAutoPharmanet = (from q in db.LG_POHs
                //                              where lstRef.Contains(q.c_pono) && q.c_type == "01"
                //                              select new
                //                              {
                //                                  q.c_pono
                //                              }).Distinct().ToList();

                //    if (lstPLAutoPharmanet.Count > 0)
                //    {
                //        lstPLAutoSP = (from q in db.LG_POD2s
                //                       join q2 in db.LG_SPHs on q.c_orno equals q2.c_spno
                //                       join q3 in db.LG_SPD1s on q2.c_spno equals q3.c_spno
                //                       where (q.c_gdg == gdg) && (q2.c_cusno == cusno) && (q3.n_sisa > 0)
                //                         && ((q2.l_delete.HasValue ? q2.l_delete.Value : false) == false)
                //                       select new PL_AUTO_SP()
                //                       {
                //                           c_sp = q2.c_sp,
                //                           c_spno = q2.c_spno,
                //                           d_spdate = (q2.d_spdate.HasValue ? q2.d_spdate.Value : Functionals.StandardSqlDateTime),
                //                           c_typesp = q2.c_type,
                //                           c_iteno = q3.c_iteno,
                //                           qtySP = (q3.n_sisa.HasValue ? q3.n_sisa.Value : 0)
                //                       }).Distinct().ToList();
                //    }
                //    else
                //    {
                //        if (gdg.ToString() == "6")
                //        {
                //            lstPLAutoSP = (from q in db.LG_POD2s
                //                           join q1 in db.LG_ORD2s on new { q.c_gdg, q.c_orno } equals new { q1.c_gdg, q1.c_orno }
                //                           join q2 in db.LG_SPHs on q1.c_spno equals q2.c_spno
                //                           join q3 in db.LG_SPD1s on q1.c_spno equals q3.c_spno
                //                           join q4 in db.LG_Cusmas on q2.c_cusno equals q4.c_cusno
                //                           where (q.c_gdg == '1') && (q2.c_cusno == cusno) && (q3.n_sisa > 0)
                //                             && ((q2.l_delete.HasValue ? q2.l_delete.Value : false) == false)
                //                               //&& (q4.n_days.HasValue ? (SqlMethods.DateDiffDay(q2.d_spdate, DateTime.Now.Date) <= q4.n_days) :
                //                               //                 (SqlMethods.DateDiffMonth(q2.d_spdate, DateTime.Now.Date) < 2))
                //                             && (SqlMethods.DateDiffMonth(q2.d_etasp, DateTime.Now.Date) < 2)
                //                           select new PL_AUTO_SP()
                //                           {
                //                               c_sp = q2.c_sp,
                //                               c_spno = q2.c_spno,
                //                               d_spdate = (q2.d_spdate.HasValue ? q2.d_spdate.Value : Functionals.StandardSqlDateTime),
                //                               c_typesp = q2.c_type,
                //                               c_iteno = q3.c_iteno,
                //                               qtySP = (q3.n_sisa.HasValue ? q3.n_sisa.Value : 0)
                //                           }).Distinct().ToList();

                //        }
                //        else
                //        {
                //            lstPLAutoSP = (from q in db.LG_POD2s
                //                           join q1 in db.LG_ORD2s on new { q.c_gdg, q.c_orno } equals new { q1.c_gdg, q1.c_orno }
                //                           join q2 in db.LG_SPHs on q1.c_spno equals q2.c_spno
                //                           join q3 in db.LG_SPD1s on q1.c_spno equals q3.c_spno
                //                           join q4 in db.LG_Cusmas on q2.c_cusno equals q4.c_cusno
                //                           where (q.c_gdg == gdg) && (q2.c_cusno == cusno) && (q3.n_sisa > 0)
                //                             && ((q2.l_delete.HasValue ? q2.l_delete.Value : false) == false)
                //                               //&& (q4.n_days.HasValue ? (SqlMethods.DateDiffDay(q2.d_spdate, DateTime.Now.Date) <= q4.n_days) :
                //                               //                 (SqlMethods.DateDiffMonth(q2.d_spdate, DateTime.Now.Date) < 2))
                //                             && (SqlMethods.DateDiffMonth(q2.d_etasp, DateTime.Now.Date) < 2)
                //                           select new PL_AUTO_SP()
                //                           {
                //                               c_sp = q2.c_sp,
                //                               c_spno = q2.c_spno,
                //                               d_spdate = (q2.d_spdate.HasValue ? q2.d_spdate.Value : Functionals.StandardSqlDateTime),
                //                               c_typesp = q2.c_type,
                //                               c_iteno = q3.c_iteno,
                //                               qtySP = (q3.n_sisa.HasValue ? q3.n_sisa.Value : 0)
                //                           }).Distinct().ToList();
                //        }
                //    }

                //    lstPLAutoSP = (from q in db.LG_RND2s
                //                   join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno, q.c_iteno, q.c_batch } equals new { q1.c_gdg, q1.c_rnno, q1.c_iteno, q1.c_batch }
                //                   join q2 in db.LG_POD2s on q.c_no equals q2.c_pono
                //                   join q3 in db.LG_ORD2s on new { q2.c_orno, q.c_iteno } equals new { q3.c_orno, q3.c_iteno }
                //                   join q4 in db.LG_SPHs on q3.c_spno equals q4.c_spno
                //                   join q5 in db.LG_SPD1s on new { q4.c_spno, q.c_iteno } equals new { q5.c_spno, q5.c_iteno }
                //                   where (q.c_gdg == gdg) && (q4.c_cusno == cusno) && (q1.n_gsisa > 0) && q5.n_spds > 0
                //                         && ((q4.l_delete.HasValue ? q4.l_delete.Value : false) == false)
                //                         && q.c_rnno == noRn
                //                   select new PL_AUTO_SP()
                //                   {
                //                       c_sp = q4.c_sp,
                //                       c_spno = q3.c_spno,
                //                       d_spdate = (q4.d_spdate.HasValue ? q4.d_spdate.Value : Functionals.StandardSqlDateTime),
                //                       c_typesp = q4.c_type,
                //                       c_iteno = q3.c_iteno,
                //                       qtySP = (q3.n_sisa.HasValue ? q3.n_sisa.Value : 0)
                //                   }).Distinct().ToList();

                //    if (lstPLAutoSP.Count > 0)
                //    {
                //        string tmp = null;

                //        decimal nQtySisa = 0;

                //        for (nLoop = 0, nLen = lstPLAuto.Count; nLoop < nLen; nLoop++)
                //        {
                //            porn = lstPLAuto[nLoop];

                //            tmp = porn.c_iteno;

                //            if (!string.IsNullOrEmpty(tmp))
                //            {
                //                #region Populate Data

                //                lstPLAutoSPCopy = lstPLAutoSP.Where(y => (y.c_iteno == tmp) && (y.qtySP > 0.00m))
                //                  .OrderBy(z => z.d_spdate).OrderBy(z => z.c_typesp).ToList();

                //                nPos = 0;

                //                while (porn.qtyRN > 0.00m)
                //                {
                //                    if (nPos >= lstPLAutoSPCopy.Count)
                //                    {
                //                        break;
                //                    }

                //                    posp = lstPLAutoSPCopy[nPos];

                //                    if (posp.qtySP > 0)
                //                    {
                //                        nQtySisa = (porn.qtyRN > posp.qtySP ? posp.qtySP : porn.qtyRN);

                //                        posp.qtySP -= nQtySisa;
                //                        porn.qtyRN -= nQtySisa;

                //                        lstProsesPLA.Add(new TempProsesPLAuto()
                //                        {
                //                            c_gdg = gdg,
                //                            c_rnno = noRn,
                //                            c_iteno = tmp,
                //                            v_itnam = porn.v_itnam,
                //                            c_batch = porn.c_batch,
                //                            n_qtyrn = nQtySisa,
                //                            c_sp = posp.c_sp,
                //                            c_spno = posp.c_spno,
                //                            c_typesp = posp.c_typesp,
                //                            n_qtysp = nQtySisa,
                //                            n_qtysp_adj = 0,
                //                            l_new = true
                //                        });
                //                    }

                //                    nPos++;
                //                }

                //                if (porn.qtyRN > 0)
                //                {
                //                    lstProsesPLA.Add(new TempProsesPLAuto()
                //                    {
                //                        c_gdg = gdg,
                //                        c_rnno = noRn,
                //                        c_iteno = tmp,
                //                        v_itnam = porn.v_itnam,
                //                        c_batch = porn.c_batch,
                //                        n_qtyrn = porn.qtyRN,
                //                        c_sp = "Auto Adjustment",
                //                        c_spno = "SPADJ",
                //                        c_typesp = "04",
                //                        n_qtysp = 0,
                //                        n_qtysp_adj = porn.qtyRN,
                //                        l_new = true
                //                    });
                //                }

                //                lstPLAutoSPCopy.Clear();

                //                #endregion
                //            }
                //        }

                //        lstPLAutoSP.Clear();
                //    }

                //    #endregion

                //    lstPLAuto.Clear();
                //}

                nCount = sjd.Count();

                if (nCount > 0)
                {
                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, sjd.ToArray());
                }

                dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                lstProsesPLA.Clear();
            }
            break;

          #endregion

          #region PL Auto

          case Constant.MODEL_COMMON_QUERY_PROSES_AUTO_PL:
            {
              string noRn = (parameters.ContainsKey("noRn") ? (string)((Functionals.ParameterParser)parameters["noRn"]).Value : string.Empty);
              string cusno = (parameters.ContainsKey("Customer") ? (string)((Functionals.ParameterParser)parameters["Customer"]).Value : string.Empty);
              char gdg = (parameters.ContainsKey("Gudang") ? Convert.ToChar(((Functionals.ParameterParser)parameters["Gudang"]).Value) : char.MinValue);
              //string Status = "01";
              //if (gdg.ToString() == "6")
              //{
              //    gdg = '1';
              //}

              PL_AUTO_RN porn = null;
              PL_AUTO_SP posp = null;

              List<PL_AUTO_RN> lstPLAuto = null;
              List<PL_AUTO_SP> lstPLAutoSP = null;
              List<PL_AUTO_SP> lstPLAutoSPCopy = null;

              List<string> lstRef = null;
              
              List<TempProsesPLAuto> lstProsesPLA = new List<TempProsesPLAuto>();

              int nLoop = 0, nLen = 0,
                nPos = 0;
                
              lstPLAuto = (from q in db.LG_RNHs
                           join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                           join q2 in db.LG_RND2s on new { q.c_gdg, q.c_rnno, q1.c_iteno, q1.c_batch } equals new { q2.c_gdg, q2.c_rnno, q2.c_iteno, q2.c_batch }
                           join q3 in db.FA_MasItms on q1.c_iteno equals q3.c_iteno
                           where (q.c_gdg == gdg) && (q.c_type == "01") && (q.l_khusus == true)
                             && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false) && q.c_rnno == noRn
                           select new PL_AUTO_RN()
                           {
                             c_gdg = q.c_gdg,
                             c_rnno = (q.c_rnno ?? string.Empty).Trim(),
                             c_iteno = (q1.c_iteno ?? string.Empty).Trim(),
                             v_itnam = (q3.v_itnam ?? string.Empty).Trim(),
                             c_batch = (q1.c_batch ?? string.Empty).Trim(),
                             qtyRN = (q1.n_gsisa.HasValue ? q1.n_gsisa.Value : 0),
                             //qtyRN = (q2.n_gqty.HasValue ? q2.n_gqty.Value : 0),
                             c_noref = (q2.c_no ?? string.Empty).Trim()
                           }).Distinct().ToList();

              //lstPLAuto.Dump("Hasil");

              lstRef = lstPLAuto.Where(x => (!string.IsNullOrEmpty(x.c_noref))).GroupBy(x => x.c_noref).Select(y => y.Key.Trim()).ToList();

              if (lstRef.Count > 0)
              {
                #region Baca SP

                var lstPLAutoPharmanet = (from q in db.LG_POHs
                                          where lstRef.Contains(q.c_pono) && q.c_type == "01"
                                          select new
                                          {
                                              q.c_pono
                                          }).Distinct().ToList();

                if (lstPLAutoPharmanet.Count > 0)
                {
                    lstPLAutoSP = (from q in db.LG_POD2s
                                   join q2 in db.LG_SPHs on q.c_orno equals q2.c_spno
                                   join q3 in db.LG_SPD1s on q2.c_spno equals q3.c_spno
                                   where (q.c_gdg == gdg) && (q2.c_cusno == cusno) && (q3.n_sisa > 0)
                                     && ((q2.l_delete.HasValue ? q2.l_delete.Value : false) == false)
                                   select new PL_AUTO_SP()
                                   {
                                       c_sp = q2.c_sp,
                                       c_spno = q2.c_spno,
                                       d_spdate = (q2.d_spdate.HasValue ? q2.d_spdate.Value : Functionals.StandardSqlDateTime),
                                       c_typesp = q2.c_type,
                                       c_iteno = q3.c_iteno,
                                       qtySP = (q3.n_sisa.HasValue ? q3.n_sisa.Value : 0)
                                   }).Distinct().ToList();
                }
                else
                {
                    if (gdg.ToString() == "6")
                    {
                        lstPLAutoSP = (from q in db.LG_POD2s
                                       join q1 in db.LG_ORD2s on new { q.c_gdg, q.c_orno } equals new { q1.c_gdg, q1.c_orno }
                                       join q2 in db.LG_SPHs on q1.c_spno equals q2.c_spno
                                       join q3 in db.LG_SPD1s on q1.c_spno equals q3.c_spno
                                       join q4 in db.LG_Cusmas on q2.c_cusno equals q4.c_cusno
                                       where (q.c_gdg == '1') && (q2.c_cusno == cusno) && (q3.n_sisa > 0)
                                         && ((q2.l_delete.HasValue ? q2.l_delete.Value : false) == false)
                                         //&& (q4.n_days.HasValue ? (SqlMethods.DateDiffDay(q2.d_spdate, DateTime.Now.Date) <= q4.n_days) :
                                         //                 (SqlMethods.DateDiffMonth(q2.d_spdate, DateTime.Now.Date) < 2))
                                         && (SqlMethods.DateDiffMonth(q2.d_etasp, DateTime.Now.Date) < 2)
                                       select new PL_AUTO_SP()
                                       {
                                           c_sp = q2.c_sp,
                                           c_spno = q2.c_spno,
                                           d_spdate = (q2.d_spdate.HasValue ? q2.d_spdate.Value : Functionals.StandardSqlDateTime),
                                           c_typesp = q2.c_type,
                                           c_iteno = q3.c_iteno,
                                           qtySP = (q3.n_sisa.HasValue ? q3.n_sisa.Value : 0)
                                       }).Distinct().ToList();

                    }
                    else
                    {
                        lstPLAutoSP = (from q in db.LG_POD2s
                                       join q1 in db.LG_ORD2s on new { q.c_gdg, q.c_orno } equals new { q1.c_gdg, q1.c_orno }
                                       join q2 in db.LG_SPHs on q1.c_spno equals q2.c_spno
                                       join q3 in db.LG_SPD1s on q1.c_spno equals q3.c_spno
                                       join q4 in db.LG_Cusmas on q2.c_cusno equals q4.c_cusno
                                       where (q.c_gdg == gdg) && (q2.c_cusno == cusno) && (q3.n_sisa > 0)
                                         && ((q2.l_delete.HasValue ? q2.l_delete.Value : false) == false)
                                         //&& (q4.n_days.HasValue ? (SqlMethods.DateDiffDay(q2.d_spdate, DateTime.Now.Date) <= q4.n_days) :
                                         //                 (SqlMethods.DateDiffMonth(q2.d_spdate, DateTime.Now.Date) < 2))
                                         && (SqlMethods.DateDiffMonth(q2.d_etasp, DateTime.Now.Date) < 2)
                                       select new PL_AUTO_SP()
                                       {
                                           c_sp = q2.c_sp,
                                           c_spno = q2.c_spno,
                                           d_spdate = (q2.d_spdate.HasValue ? q2.d_spdate.Value : Functionals.StandardSqlDateTime),
                                           c_typesp = q2.c_type,
                                           c_iteno = q3.c_iteno,
                                           qtySP = (q3.n_sisa.HasValue ? q3.n_sisa.Value : 0)
                                       }).Distinct().ToList();
                    }
                }

                lstPLAutoSP = (from q in db.LG_RND2s
                               join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno, q.c_iteno, q.c_batch } equals new { q1.c_gdg, q1.c_rnno, q1.c_iteno, q1.c_batch }
                               join q2 in db.LG_POD2s on q.c_no equals q2.c_pono
                               join q3 in db.LG_ORD2s on new { q2.c_orno, q.c_iteno } equals new { q3.c_orno, q3.c_iteno }
                               join q4 in db.LG_SPHs on q3.c_spno equals q4.c_spno
                               join q5 in db.LG_SPD1s on new { q4.c_spno, q.c_iteno } equals new { q5.c_spno, q5.c_iteno }
                               where (q.c_gdg == gdg) && (q4.c_cusno == cusno) && (q1.n_gsisa > 0) && q5.n_spds > 0
                                     && ((q4.l_delete.HasValue ? q4.l_delete.Value : false) == false)
                                     && q.c_rnno == noRn
                               select new PL_AUTO_SP()
                               {
                                   c_sp = q4.c_sp,
                                   c_spno = q3.c_spno,
                                   d_spdate = (q4.d_spdate.HasValue ? q4.d_spdate.Value : Functionals.StandardSqlDateTime),
                                   c_typesp = q4.c_type,
                                   c_iteno = q3.c_iteno,
                                   qtySP = (q3.n_sisa.HasValue ? q3.n_sisa.Value : 0)
                               }).Distinct().ToList();

                if (lstPLAutoSP.Count > 0)
                {
                  string tmp = null;

                  decimal nQtySisa = 0;

                  for (nLoop = 0, nLen = lstPLAuto.Count; nLoop < nLen; nLoop++)
                  {
                    porn = lstPLAuto[nLoop];

                    tmp = porn.c_iteno;

                    if (!string.IsNullOrEmpty(tmp))
                    {
                      #region Populate Data

                      lstPLAutoSPCopy = lstPLAutoSP.Where(y => (y.c_iteno == tmp) && (y.qtySP > 0.00m))
                        .OrderBy(z => z.d_spdate).OrderBy(z => z.c_typesp).ToList();

                      nPos = 0;

                      while (porn.qtyRN > 0.00m)
                      {
                        if (nPos >= lstPLAutoSPCopy.Count)
                        {
                          break;
                        }

                        posp = lstPLAutoSPCopy[nPos];

                        if (posp.qtySP > 0)
                        {
                          nQtySisa = (porn.qtyRN > posp.qtySP ? posp.qtySP : porn.qtyRN);

                          posp.qtySP -= nQtySisa;
                          porn.qtyRN -= nQtySisa;

                          lstProsesPLA.Add(new TempProsesPLAuto()
                          {
                            c_gdg = gdg,
                            c_rnno = noRn,
                            c_iteno = tmp,
                            v_itnam = porn.v_itnam,
                            c_batch = porn.c_batch,
                            n_qtyrn = nQtySisa,
                            c_sp = posp.c_sp, 
                            c_spno = posp.c_spno,
                            c_typesp = posp.c_typesp,
                            n_qtysp = nQtySisa,
                            n_qtysp_adj = 0,
                            l_new = true
                          });
                        }

                        nPos++;
                      }

                      if (porn.qtyRN > 0)
                      {
                        lstProsesPLA.Add(new TempProsesPLAuto()
                          {
                            c_gdg = gdg,
                            c_rnno = noRn,
                            c_iteno = tmp,
                            v_itnam = porn.v_itnam,
                            c_batch = porn.c_batch,
                            n_qtyrn = porn.qtyRN,
                            c_sp = "Auto Adjustment",
                            c_spno = "SPADJ",
                            c_typesp = "04",
                            n_qtysp = 0,
                            n_qtysp_adj = porn.qtyRN,
                            l_new = true
                          });
                      }

                      lstPLAutoSPCopy.Clear();

                      #endregion
                    }
                  }

                  lstPLAutoSP.Clear();
                }

                #endregion

                lstPLAuto.Clear();
              }

              #region Old Coded

              ////Cek Availability PL Auto
              //var query = (from q in db.LG_RNHs
              //             where q.c_gdg == Gudang && q.c_type == "06"
              //             && q.c_rnno == noRn
              //             select new
              //             {
              //               q.c_rnno,
              //               q.c_dono,
              //               q.c_from,
              //             }).Take(1).Distinct();


              //if (query != null)
              //{
              //  var DOPD = (from q in db.LG_DOPDs
              //              join q1 in db.FA_MasItms on new { q.c_nosup, q.c_iteno } equals new { q1.c_nosup, q1.c_iteno }
              //              where q.c_dono == query.Select(x => x.c_dono).SingleOrDefault() &&
              //              q.c_nosup == query.Select(x => x.c_from).SingleOrDefault()
              //              select new
              //              {
              //                q.c_dono,
              //                q.c_nosup
              //              }).ToList();

              //  var dopdRn = (from q in
              //                  (
              //                    from q in db.LG_DOPDs
              //                    join q1 in db.FA_MasItms on new { q.c_nosup, q.c_iteno } equals new { q1.c_nosup, q1.c_iteno }
              //                    where q.c_dono == query.Select(x => x.c_dono).SingleOrDefault() &&
              //                    q.c_nosup == query.Select(x => x.c_from).SingleOrDefault()
              //                    select new
              //                    {
              //                      q.c_dono,
              //                      q1.c_iteno
              //                    })
              //                join q1 in db.LG_RND1s on q.c_iteno equals q1.c_iteno into f
              //                from f1 in f.DefaultIfEmpty()
              //                where f1.c_rnno == query.Select(x => x.c_rnno).SingleOrDefault()
              //                && f1.c_iteno == null
              //                select new
              //                {

              //                  q.c_dono,
              //                  Price = f1.c_iteno == null ? "" : f1.c_iteno
              //                }).ToList();

              //  if (DOPD.Count > 0 && dopdRn.Count > 0)
              //  {
              //    Status = "02";
              //  }

              //  var RND1 = (from q in db.LG_RND2s
              //              where q.c_rnno == query.Select(x => x.c_rnno).SingleOrDefault()
              //              select new
              //              {
              //                q.c_gdg,
              //                q.c_rnno,
              //                q.c_iteno,
              //                q.c_no
              //              }).Distinct();

              //  var CekRND1 = (from q in db.LG_RND1s
              //                 join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
              //                 where q.c_gdg == '1' && q.c_rnno == query.Select(x => x.c_rnno).SingleOrDefault()
              //                 group new { q, q1 } by new { q.c_gdg, q.c_rnno, q.c_iteno, q1.v_itnam, q.c_batch } into g
              //                 select new
              //                 {
              //                   c_gdg = g.Key.c_gdg,
              //                   c_rnno = g.Key.c_rnno,
              //                   c_batch = g.Key.c_batch,
              //                   c_iteno = g.Key.c_iteno,
              //                   v_itnam = g.Key.v_itnam,
              //                   qty = g.Sum(x => x.q.n_gqty),
              //                   sisa = g.Sum(x => x.q.n_gsisa)
              //                 }).AsQueryable();

              //  var CekRND1List = CekRND1.ToList();

              //  var CekSP1 = (from q in RND1
              //                join q1 in db.LG_POD2s on q.c_no equals q1.c_pono
              //                join q2 in db.LG_ORD2s on new { q1.c_orno, q.c_iteno } equals new { q2.c_orno, q2.c_iteno }
              //                join q3 in db.LG_ORHs on q2.c_orno equals q3.c_orno
              //                join q4 in db.LG_SPD1s on new { q2.c_spno, q2.c_iteno, item = q.c_iteno } equals new { q4.c_spno, q4.c_iteno, item = q4.c_iteno }
              //                join q5 in db.LG_SPHs on q4.c_spno equals q5.c_spno
              //                where q.c_gdg == Gudang && q3.c_type == "02"
              //                && q5.c_cusno == Customer && q4.n_sisa > 0
              //                group new { q, q4 } by new { q.c_rnno, q.c_iteno, q4.c_spno } into sum
              //                select new
              //                {
              //                  c_iteno = sum.Key.c_iteno,
              //                  c_rnno = sum.Key.c_rnno,
              //                  c_spno = sum.Key.c_spno,
              //                  n_sisa = sum.Sum(x => x.q4.n_sisa)
              //                }).AsQueryable();

              //  var Cek = (from q in CekRND1
              //             join q1 in CekSP1 on new { q.c_rnno, q.c_iteno } equals new { q1.c_rnno, q1.c_iteno } into a
              //             from a1 in a.DefaultIfEmpty()
              //             where q.qty == q.sisa
              //             select new
              //             {
              //               a1.c_iteno,
              //               a1.n_sisa,
              //               q.c_gdg,
              //               a1.c_rnno,
              //               q.c_batch,
              //               iteno = q.c_iteno,
              //               q.v_itnam,
              //               a1.c_spno,
              //               q.qty,
              //               sisarn = q.sisa
              //             }).ToList();

              //  if (Cek.Count > 0)
              //  {
              //    decimal? QtyRN = 0, QtyF = 0, QtyRNO = 0, QtyRNI = 0, QtyRNT = 0, QtyAdj = 0;

              //    plauto = new List<PL_AUTO>();

              //    for (int nLoop = 0; nLoop < CekRND1List.Count; nLoop++)
              //    {
              //      QtyRN = CekRND1List[nLoop].sisa;

              //      QtyRNI = CekRND1List[nLoop].sisa;

              //      QtyRNT = QtyRNI;

              //      var CekL = Cek.Where(x => x.c_iteno == CekRND1List[nLoop].c_iteno).ToList();

              //      decimal? QtySP = 0, QtyE = 0, QtyRNS = 0;

              //      for (int nLoopC = 0; nLoopC < CekL.Count; nLoopC++)
              //      {
              //        bool Lop = true;

              //        if (CekL.Count > 1)
              //        {
              //          Lop = false;
              //        }
              //        QtySP = CekL[nLoopC].n_sisa;

              //        if (QtySP < QtyRN)
              //        {
              //          QtyRN -= QtySP;
              //          QtySP = 0;

              //        }
              //        else
              //        {
              //          QtySP -= QtyRN;
              //          QtyRN = 0;
              //          Lop = true;
              //        }

              //        if (Lop == true)
              //        {
              //          if (QtyRNT > 0)
              //          {

              //            QtyRNS = QtyRNI - QtyRNO;
              //            //QtyAdj = QtyRNS - QtyRN;
              //            //QtyAdj = QtyRNS - QtyAdj;
              //            plauto.Add(new PL_AUTO()
              //            {
              //              c_gdg = Gudang,
              //              c_iteno = CekL[nLoopC].iteno,
              //              c_rnno = CekL[nLoopC].c_rnno,
              //              v_itnam = CekL[nLoopC].v_itnam,
              //              c_spno = CekL[nLoopC].c_spno,
              //              qtyRN = QtyRNS,
              //              c_batch = CekL[nLoopC].c_batch,
              //              qtySP = CekL[nLoopC].n_sisa,
              //              qtySPAdj = QtyRN,
              //              isNew = true,
              //              l_new = true
              //            });
              //            QtyRNT -= QtyRNS;
              //          }
              //        }
              //        else
              //        {
              //          QtyRNO = CekRND1List[nLoop].sisa;
              //          QtyRNO -= QtyRN;
              //          QtyAdj = QtyRNO - CekL[nLoopC].n_sisa;
              //          plauto.Add(new PL_AUTO()
              //          {
              //            c_gdg = Gudang,
              //            c_iteno = CekL[nLoopC].iteno,
              //            v_itnam = CekL[nLoopC].v_itnam,
              //            c_rnno = CekL[nLoopC].c_rnno,
              //            c_spno = CekL[nLoopC].c_spno,
              //            c_batch = CekL[nLoopC].c_batch,
              //            qtyRN = QtyRNO,
              //            qtySP = CekL[nLoopC].n_sisa,
              //            qtySPAdj = QtyAdj,
              //            isNew = true,
              //            l_new = true
              //          });
              //        }
              //      }
              //    }
              //  }

              //}

              #endregion

              nCount = lstProsesPLA.Count();

              if (nCount > 0)
              {
                dic.Add(Constant.DEFAULT_NAMING_RECORDS, lstProsesPLA.ToArray());
              }

              dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

              dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

              lstProsesPLA.Clear();
            }
            break;

          #endregion

          #region Laporan Enapza Indra 20170821

          case Constant.MODEL_COMMON_QUERY_LAPORAN_ENAPZA:
            {

                RPT_Enapza porn = null;

                List<RPT_Enapza> lstEnapza = null, lstEnapza2 = null;

                int nLoop = 0,
                    nLoopC = 0;

                lstEnapza = (from q in db.temp_enapzas
                             join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                             orderby q.c_iteno ascending, 
                                     q.c_batch descending,
                                     q.asal descending,
                                     q.tujuan descending,
                                     q.batchout ascending,
                                     q.batchakhir descending

                             select new RPT_Enapza()
                             {
                                item        = q.c_iteno,
                                tgl         = q.tgl.Value,
                                saldoawal   = q.saldoawal.Value,
                                c_batch     = q.c_batch,
                                fakturin    = q.fakturin,
                                asal        = q.asal,
                                jumlahin    = q.jumlahin.Value,
                                batchin     = q.batchin,
                                fakturout   = q.fakturout,
                                tujuan      = q.tujuan,
                                jumlahout   = q.jumlahout.Value,
                                batchout    = q.batchout,
                                saldoakhir  = q.saldoakhir.Value,
                                batchakhir  = q.batchakhir,
                                d_expired   = q.d_expired.Value,
                             }).OrderBy(x => x.item).ToList();

                if (lstEnapza.Count > 0)
                {
                    lstEnapza2 = new List<RPT_Enapza>();

                    for (nLoopC = 0; nLoopC < lstEnapza.Count; nLoopC++)
                    {
                        RPT_Enapza sjdg = lstEnapza[nLoopC];

                        lstEnapza2.Add(new RPT_Enapza()
                        {
                            tgl         = sjdg.tgl,
                            saldoawal   = sjdg.saldoawal,
                            c_batch     = sjdg.c_batch,
                            fakturin    = sjdg.fakturin,
                            asal        = sjdg.asal,
                            jumlahin    = sjdg.jumlahin,
                            batchin     = sjdg.batchin,
                            fakturout   = sjdg.fakturout,
                            tujuan      = sjdg.tujuan,
                            jumlahout   = sjdg.jumlahout,
                            batchout    = sjdg.batchout,
                            saldoakhir  = sjdg.saldoakhir,
                            batchakhir  = sjdg.batchakhir,
                            d_expired   = sjdg.d_expired,
                        });
                    }

                    nCount = lstEnapza2.Count;

                    if (nCount > 0)
                    {
                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, lstEnapza2.ToArray());
                    }

                    dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                    dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                    lstEnapza.Clear();
                    lstEnapza2.Clear();
                
                }

                else
                {
                    dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);
                }

            }
            break;

          #endregion

          #region Jual Retur

          #region MODEL_PROCESS_QUERY_MULTIPLE_RETURCUSTOMER

          case Constant.MODEL_PROCESS_QUERY_MULTIPLE_RETURCUSTOMER:
            {
              #region Old Coded

              //string custNo = (parameters.ContainsKey("customerNo") ? (string)((Functionals.ParameterParser)parameters["customerNo"]).Value : string.Empty);
              //string rc1 = (parameters.ContainsKey("rcFrom") ? (string)((Functionals.ParameterParser)parameters["rcFrom"]).Value : string.Empty);
              //string rc2 = (parameters.ContainsKey("rcTo") ? (string)((Functionals.ParameterParser)parameters["rcTo"]).Value : string.Empty);

              //rc1 = (string.IsNullOrEmpty(rc1) ? "?" : rc1);
              //rc2 = (string.IsNullOrEmpty(rc2) ? rc1 : rc2);

              //List<Temporary_JualRetur> listReturProses = null;

              //listReturProses = (from q in db.LG_RCHes
              //                   join q1 in db.LG_RCD2s on new { q.c_gdg, q.c_rcno } equals new { q1.c_gdg, q1.c_rcno } into q_1
              //                   from qRCD2 in q_1.DefaultIfEmpty()
              //                   join q2 in db.LG_RCD3s on new { q.c_gdg, q.c_rcno, qRCD2.c_iteno } equals new { q2.c_gdg, q2.c_rcno, q2.c_iteno } into q_2
              //                   from qRCD3 in q_2.DefaultIfEmpty()
              //                   join q3 in db.FA_MasItms on qRCD2.c_iteno equals q3.c_iteno into q_3
              //                   from qItm in q_3.DefaultIfEmpty()
              //                   join q4 in db.LG_FJD3s on qRCD2.c_dono equals q4.c_dono into q_4
              //                   from qFJ in q_4.DefaultIfEmpty()
              //                   join q5 in db.LG_Cusmas on q.c_cusno equals q5.c_cusno
              //                   where q.c_cusno == custNo
              //                     && (q.c_rcno.CompareTo(rc1) >= 0 && q.c_rcno.CompareTo(rc2) <= 0)
              //                     && (qRCD2.n_sisa > 0)
              //                   orderby q.c_rcno ascending
              //                   group new { qRCD1 = qRCD2, qRCD2 = qRCD3 } by new { q.d_rcdate, qRCD2.c_iteno, qRCD2.c_rcno, qRCD2.c_dono, qRCD3.n_salpri, qRCD3.n_disc, qItm.v_itnam, qFJ.c_fjno, q5.l_cabang } into g
              //                   select new Temporary_JualRetur()
              //                   {
              //                     d_rcdate = g.Key.d_rcdate.Value,
              //                     c_exno = g.Key.c_fjno,
              //                     c_norc = g.Key.c_rcno,
              //                     c_nodo = g.Key.c_dono,
              //                     c_iteno = g.Key.c_iteno,
              //                     v_itnam = g.Key.v_itnam,
              //                     c_type = "03",
              //                     n_salpri = g.Key.n_salpri.Value,
              //                     n_disc = g.Key.n_disc.Value,
              //                     n_qty = g.Sum(x => x.qRCD1.n_sisa).Value,
              //                     l_cab = g.Key.l_cabang.Value
              //                   }).Distinct().ToList();

              //if (listReturProses.Count > 0)
              //{
              //  dic.Add(Constant.DEFAULT_NAMING_RECORDS, listReturProses.ToArray());
              //}

              //dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

              //dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, listReturProses.Count);

              //listReturProses.Clear();

              #endregion
              
              string custNo = (parameters.ContainsKey("customerNo") ? (string)((Functionals.ParameterParser)parameters["customerNo"]).Value : string.Empty);
              int bulan = (parameters.ContainsKey("bulan") ? (int)((Functionals.ParameterParser)parameters["bulan"]).Value : 0);
              int tahun = (parameters.ContainsKey("tahun") ? (int)((Functionals.ParameterParser)parameters["tahun"]).Value : 0);

              //rc1 = (string.IsNullOrEmpty(rc1) ? "?" : rc1);
              //rc2 = (string.IsNullOrEmpty(rc2) ? rc1 : rc2);

              List<Temporary_JualRetur> listReturProses = null;
              //var qry = (from q in db.LG_RCHes
              //           join q1 in db.LG_RCD2s on new { q.c_gdg, q.c_rcno } equals new { q1.c_gdg, q1.c_rcno }
              //           //join q2 in db.LG_RCD3s on new { q.c_gdg, q.c_rcno, q1.c_iteno } equals new { q2.c_gdg, q2.c_rcno, q2.c_iteno }
              //           join q2 in db.LG_Cusmas on q.c_cusno equals q2.c_cusno
              //           join q3 in db.LG_FJRD3s on q.c_rcno equals q3.c_rcno into q_3
              //           from qJRD3 in q_3.DefaultIfEmpty()
              //           join q4 in db.LG_FJD3s on q1.c_dono equals q4.c_dono into q_4
              //           from qFJ in q_4.DefaultIfEmpty()
              //           where ((string.IsNullOrEmpty(custNo) ? q.c_cusno : custNo) == q.c_cusno)
              //            && ((q.d_rcdate.Value.Month == bulan) && (q.d_rcdate.Value.Year == tahun))
              //            && (q1.n_sisa > 0)
              //            && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
              //           //group new { q, q1 } by new { q.d_rcdate, q.c_rcno, q1.c_iteno, qRCD3.n_salpri, qRCD3.n_disc, qItm.v_itnam, qFJ.c_fjno, q5.l_cabang } into g
              //           group new { q, q1 } by new { q.c_cusno, q.d_rcdate, q.c_rcno, qFJ.c_fjno, q1.c_dono, q2.l_cabang } into g
              //           select new Temporary_JualRetur()
              //           {
              //             c_cusno = g.Key.c_cusno,
              //             d_rcdate = g.Key.d_rcdate.Value,
              //             c_exno = g.Key.c_fjno,
              //             c_norc = g.Key.c_rcno,
              //             c_nodo = g.Key.c_dono,
              //             //c_iteno = g.Key.c_iteno,
              //             //v_itnam = string.Empty,
              //             //c_type = "03",
              //             //n_salpri = g.Key.n_salpri.Value,
              //             //n_disc = g.Key.n_disc.Value,
              //             //n_qty = g.Sum(x => x.qRCD1.n_sisa).Value,
              //             l_cab = g.Key.l_cabang.Value
              //           }).Distinct();

              listReturProses = (from q in db.LG_RCHes
                                 join q1 in db.LG_RCD2s on new { q.c_gdg, q.c_rcno } equals new { q1.c_gdg, q1.c_rcno }
                                 //join q2 in db.LG_RCD3s on new { q.c_gdg, q.c_rcno, q1.c_iteno } equals new { q2.c_gdg, q2.c_rcno, q2.c_iteno }
                                 join q2 in db.LG_Cusmas on q.c_cusno equals q2.c_cusno
                                 join q3 in db.LG_FJRD3s on q.c_rcno equals q3.c_rcno into q_3
                                 from qJRD3 in q_3.DefaultIfEmpty()
                                 join q4 in db.LG_FJD3s on q1.c_dono equals q4.c_dono into q_4
                                 from qFJ in q_4.DefaultIfEmpty()
                                 where ((string.IsNullOrEmpty(custNo) ? q.c_cusno : custNo) == q.c_cusno)
                                  && ((q.d_rcdate.Value.Month == bulan) && (q.d_rcdate.Value.Year == tahun))
                                  && (q1.n_sisa > 0)
                                  && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                 //group new { q, q1 } by new { q.d_rcdate, q.c_rcno, q1.c_iteno, qRCD3.n_salpri, qRCD3.n_disc, qItm.v_itnam, qFJ.c_fjno, q5.l_cabang } into g
                                 group new { q, q1 } by new { q.c_cusno, q.d_rcdate, q.c_rcno, qFJ.c_fjno, q1.c_dono, q2.l_cabang } into g
                                 select new Temporary_JualRetur()
                                 {
                                   c_cusno = g.Key.c_cusno,
                                   d_rcdate = g.Key.d_rcdate.Value,
                                   c_exno = g.Key.c_fjno,
                                   c_norc = g.Key.c_rcno,
                                   c_nodo = g.Key.c_dono,
                                   //c_iteno = g.Key.c_iteno,
                                   //v_itnam = string.Empty,
                                   //c_type = "03",
                                   //n_salpri = g.Key.n_salpri.Value,
                                   //n_disc = g.Key.n_disc.Value,
                                   //n_qty = g.Sum(x => x.qRCD1.n_sisa).Value,
                                   l_cab = g.Key.l_cabang.Value
                                 }).Distinct().ToList();

              //listReturProses = (from q in db.LG_RCHes
              //                   join q1 in db.LG_RCD2s on new { q.c_gdg, q.c_rcno } equals new { q1.c_gdg, q1.c_rcno } into q_1
              //                   from qRCD2 in q_1.DefaultIfEmpty()
              //                   join q2 in db.LG_RCD3s on new { q.c_gdg, q.c_rcno, qRCD2.c_iteno } equals new { q2.c_gdg, q2.c_rcno, q2.c_iteno } into q_2
              //                   from qRCD3 in q_2.DefaultIfEmpty()
              //                   join q3 in db.FA_MasItms on qRCD2.c_iteno equals q3.c_iteno into q_3
              //                   from qItm in q_3.DefaultIfEmpty()
              //                   join q4 in db.LG_FJD3s on qRCD2.c_dono equals q4.c_dono into q_4
              //                   from qFJ in q_4.DefaultIfEmpty()
              //                   join q5 in db.LG_Cusmas on q.c_cusno equals q5.c_cusno
              //                   where q.c_cusno == custNo
              //                     && (q.c_rcno.CompareTo(rc1) >= 0 && q.c_rcno.CompareTo(rc2) <= 0)
              //                     && (qRCD2.n_sisa > 0)
              //                   orderby q.c_rcno ascending
              //                   group new { qRCD1 = qRCD2, qRCD2 = qRCD3 } by new { q.d_rcdate, qRCD2.c_iteno, qRCD2.c_rcno, qRCD2.c_dono, qRCD3.n_salpri, qRCD3.n_disc, qItm.v_itnam, qFJ.c_fjno, q5.l_cabang } into g
              //                   select new Temporary_JualRetur()
              //                   {
              //                     d_rcdate = g.Key.d_rcdate.Value,
              //                     c_exno = g.Key.c_fjno,
              //                     c_norc = g.Key.c_rcno,
              //                     c_nodo = g.Key.c_dono,
              //                     c_iteno = g.Key.c_iteno,
              //                     v_itnam = g.Key.v_itnam,
              //                     c_type = "03",
              //                     n_salpri = g.Key.n_salpri.Value,
              //                     n_disc = g.Key.n_disc.Value,
              //                     n_qty = g.Sum(x => x.qRCD1.n_sisa).Value,
              //                     l_cab = g.Key.l_cabang.Value
              //                   }).Distinct().ToList();

              if (listReturProses.Count > 0)
              {
                dic.Add(Constant.DEFAULT_NAMING_RECORDS, listReturProses.ToArray());
              }

              dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

              dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, listReturProses.Count);

              listReturProses.Clear();
            }
            break;

          #endregion

          #endregion

          #region REPORT_INVENTORY_REPORT_CS Old

          //case Constant.REPORT_INVENTORY_REPORT_CS:
          //  {
          //    nCount = 0;

          //    char gdgPosStok = (parameters.ContainsKey("gdgStok") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgStok"]).Value) : '0');
          //    char gdgPosTrx = (parameters.ContainsKey("gdgTrx") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgTrx"]).Value) : '0');

          //    char[] avaibleGudangOkt = new char[] { '0', '1' };

          //    #region SOH

          //    #region Old Coded

          //    var sohRN = (from q in db.LG_RNHs
          //                 join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
          //                 where (q.c_type != "06")
          //                   && ((gdgPosStok == '0' ? q.c_gdg : gdgPosStok) == q.c_gdg)
          //                   && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
          //                 select new TempStokLogic()
          //                 {
          //                   RefNo = q.c_rnno,
          //                   Item = q1.c_iteno,
          //                   GQty = (q1.n_gsisa.HasValue ? q1.n_gsisa.Value : 0),
          //                   BQty = (q1.n_bsisa.HasValue ? q1.n_bsisa.Value : 0)
          //                 }).AsQueryable();

          //    var sohCombo = (from q in db.LG_ComboHs
          //                    where ((gdgPosStok == '0' ? q.c_gdg : gdgPosStok) == q.c_gdg)
          //                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
          //                    select new TempStokLogic()
          //                    {
          //                      RefNo = q.c_combono,
          //                      Item = q.c_iteno,
          //                      GQty = (q.n_gsisa.HasValue ? q.n_gsisa.Value : 0),
          //                      BQty = (q.n_bsisa.HasValue ? q.n_bsisa.Value : 0)
          //                    }).AsQueryable();

          //    var soh = sohRN.Union(sohCombo).GroupBy(x =>
          //                  x.Item
          //                ).Select(y => new TempStokLogic()
          //                {
          //                  Item = y.Key,
          //                  RefNo = y.Key,
          //                  GQty = y.Sum(z => z.GQty),
          //                  BQty = y.Sum(z => z.BQty)
          //                }).AsQueryable();

          //    #endregion

          //    //var sohs = (from q in GlobalQuery.ViewStockLite(db, gdgPosStok)
          //    //           where ((gdgPosStok == '0' ? q.c_gdg : gdgPosStok) == q.c_gdg)
          //    //           group q by q.c_iteno into g
          //    //           select new TempStokLogic()
          //    //           {
          //    //             Item = g.Key.Trim(),
          //    //             RefNo = g.Key.Trim(),
          //    //             GQty = g.Sum(x => x.n_gsisa),
          //    //             BQty = g.Sum(x => x.n_bsisa)
          //    //           }).AsQueryable();

          //    //var xx = soh.Provider.ToString();
          //    //var xxx = sohs.Provider.ToString();

          //    //soh.Dump("Hasil");

          //    #endregion

          //    #region Order

          //    var reqPO = (from q in db.LG_POHs
          //                 join q1 in db.LG_POD1s on new { q.c_gdg, q.c_pono } equals new { q1.c_gdg, q1.c_pono }
          //                 where (q1.n_sisa > 0)
          //                  && (SqlMethods.DateDiffMonth(q.d_podate, DateTime.Now) <
          //                      ((q.l_import.HasValue ? q.l_import.Value : false) ? 6 : 2))
          //                  && ((gdgPosTrx == '0' ? q.c_gdg : gdgPosTrx) == q.c_gdg)
          //                  && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
          //                 select new TempStokLogic()
          //                 {
          //                   RefNo = q.c_pono,
          //                   Item = q1.c_iteno,
          //                   GQty = (q1.n_qty.HasValue ? q1.n_qty.Value : 0),
          //                   BQty = 0
          //                 }).AsQueryable();

          //    var reqSPG = (from q in db.LG_SPGHs
          //                  join q1 in db.LG_SPGD1s on new { q.c_gdg1, q.c_spgno } equals new { c_gdg1 = q1.c_gdg, q1.c_spgno }
          //                  where (q1.n_sisa > 0)
          //                    && (SqlMethods.DateDiffMonth(q.d_spgdate, DateTime.Now) < 2)
          //                    && ((gdgPosTrx == '0' ? q.c_gdg1 : gdgPosTrx) == q.c_gdg1)
          //                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
          //                  select new TempStokLogic()
          //                  {
          //                    RefNo = q.c_spgno,
          //                    Item = q1.c_iteno,
          //                    GQty = (q1.n_qty.HasValue ? q1.n_qty.Value : 0),
          //                    BQty = 0
          //                  }).AsQueryable();

          //    //sitSPG.Dump("Hasil");

          //    //var reqOrder = reqPO.Union(reqSPG).AsQueryable();
          //    var reqOrder = reqPO.Union(reqSPG).GroupBy(x =>
          //                        x.Item
          //                      ).Select(y => new TempStokLogic()
          //                      {
          //                        Item = y.Key,
          //                        RefNo = y.Key,
          //                        GQty = y.Sum(z => z.GQty),
          //                        BQty = y.Sum(z => z.BQty)
          //                      }).AsQueryable();

          //    //reqOrder.Dump("Hasil");

          //    #endregion

          //    #region SIT

          //    var sit = (from q in db.LG_SJHs
          //               join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
          //               where (q.l_status == false)
          //                 && ((gdgPosTrx == '0' ? q.c_gdg : gdgPosTrx) == q.c_gdg)
          //                 && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
          //               select new TempStokLogic()
          //               {
          //                 RefNo = q.c_sjno,
          //                 Item = q1.c_iteno,
          //                 GQty = (q1.n_gqty.HasValue ? q1.n_gqty.Value : 0),
          //                 BQty = (q1.n_bqty.HasValue ? q1.n_bqty.Value : 0)
          //               }).AsQueryable();

          //    sit = sit.GroupBy(x =>
          //              x.Item
          //            ).Select(y => new TempStokLogic()
          //            {
          //              Item = y.Key,
          //              RefNo = y.Key,
          //              GQty = y.Sum(z => z.GQty),
          //              BQty = y.Sum(z => z.BQty)
          //            }).AsQueryable();
          //    //sit.Dump("Hasil");

          //    #endregion

          //    #region Pesanan

          //    var spCabReg = (from q in db.LG_SPHs
          //                    join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
          //                    join q2 in db.LG_Cusmas on q.c_cusno equals q2.c_cusno
          //                    join q3 in db.FA_MasItms on q1.c_iteno equals q3.c_iteno
          //                    where (q1.n_sisa > 0)
          //                      && (SqlMethods.DateDiffMonth(q.d_spdate, DateTime.Now) < 2)
          //                      && ((gdgPosTrx == '0' ? q2.c_gdg : gdgPosTrx) == q2.c_gdg)
          //                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
          //                    select new TempStokLogic()
          //                    {
          //                      RefNo = q.c_spno,
          //                      Item = q1.c_iteno,
          //                      GQty = (q1.n_sisa.HasValue ? q1.n_sisa.Value : 0),
          //                      BQty = 0
          //                    }).AsQueryable();

          //    //spCabReg.Dump("Hasil");

          //    var spCabOkt = (from q in db.LG_SPHs
          //                    join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
          //                    join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
          //                    where (q1.n_sisa > 0)
          //                      && ((avaibleGudangOkt.Contains(gdgPosTrx) ? "02" : "XX") == q.c_type)
          //                      && (SqlMethods.DateDiffMonth(q.d_spdate, DateTime.Now) < 2)
          //                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
          //                    select new TempStokLogic()
          //                    {
          //                      RefNo = q.c_spno,
          //                      Item = q1.c_iteno,
          //                      GQty = (q1.n_sisa.HasValue ? q1.n_sisa.Value : 0),
          //                      BQty = 0
          //                    }).AsQueryable();

          //    //spCabOkt.Dump("Hasil");

          //    var spSPG = (from q in db.LG_SPGHs
          //                 join q1 in db.LG_SPGD1s on new { q.c_gdg1, q.c_spgno } equals new { c_gdg1 = q1.c_gdg, q1.c_spgno }
          //                 where (q.l_status == true) && (q1.n_sisa > 0)
          //                   && ((gdgPosTrx == '0' ? q.c_gdg2 : gdgPosTrx) == q.c_gdg2)
          //                   && (SqlMethods.DateDiffMonth(q.d_spgdate, DateTime.Now) < 2)
          //                 select new TempStokLogic()
          //                 {
          //                   RefNo = q.c_spgno,
          //                   Item = q1.c_iteno,
          //                   GQty = (q1.n_sisa.HasValue ? q1.n_sisa.Value : 0),
          //                   BQty = 0
          //                 }).AsQueryable();

          //    //var spReq = spCabReg.Union(spCabOkt).Union(spSPG).AsQueryable();
          //    var spReq = spCabReg.Union(spCabOkt).Union(spSPG).GroupBy(x =>
          //                    x.Item
          //                  ).Select(y => new TempStokLogic()
          //                  {
          //                    Item = y.Key,
          //                    RefNo = y.Key,
          //                    GQty = y.Sum(z => z.GQty),
          //                    BQty = y.Sum(z => z.BQty)
          //                  }).AsQueryable();

          //    //spReq.Dump("Hasil");

          //    #endregion

          //    #region Old Coded

          //    //var curStok = (from q in db.FA_MasItms
          //    //               join q1 in soh on q.c_iteno equals q1.Item into q_1
          //    //               from qSOH in q_1.DefaultIfEmpty()
          //    //               join q2 in reqOrder on q.c_iteno equals q2.Item into q_2
          //    //               from qORD in q_2.DefaultIfEmpty()
          //    //               join q3 in sit on q.c_iteno equals q3.Item into q_3
          //    //               from qSIT in q_3.DefaultIfEmpty()
          //    //               join q4 in spReq on q.c_iteno equals q4.Item into q_4
          //    //               from qREQ in q_3.DefaultIfEmpty()
          //    //               //group new { q, qSOH, qORD, qSIT, qREQ } by new { q.c_iteno, q.v_itnam, q.v_undes } into g
          //    //               select new
          //    //               {
          //    //                 Item = g.Key.c_iteno,
          //    //                 ItemName = g.Key.v_itnam, 
          //    //                 ItemUndes = g.Key.v_undes,
          //    //                 SOH_GOOD = g.Sum(x => x.qSOH.GQty),
          //    //                 SOH_BAD = g.Sum(x => x.qSOH.BQty),
          //    //                 ORD_QTY = g.Sum(x => x.qORD.GQty),
          //    //                 SIT_GOOD = g.Sum(x => x.qSIT.GQty),
          //    //                 SIT_BAD = g.Sum(x => x.qSIT.BQty),
          //    //                 qREQ_QTY = g.Sum(x => x.qREQ.GQty)
          //    //               }).AsQueryable();

          //    #endregion

          //    var curStok = (from q in db.FA_MasItms
          //                   join q1 in soh on q.c_iteno equals q1.Item into q_1
          //                   from qSOH in q_1.DefaultIfEmpty()
          //                   join q2 in reqOrder on q.c_iteno equals q2.Item into q_2
          //                   from qORD in q_2.DefaultIfEmpty()
          //                   join q3 in sit on q.c_iteno equals q3.Item into q_3
          //                   from qSIT in q_3.DefaultIfEmpty()
          //                   join q4 in spReq on q.c_iteno equals q4.Item into q_4
          //                   from qREQ in q_4.DefaultIfEmpty()
          //                   where ((q.l_hide.HasValue ? q.l_hide.Value : false) == false)
          //                    && ((q.l_aktif.HasValue ? q.l_aktif.Value : false) == true)
          //                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
          //                   select new
          //                   {
          //                     q.c_nosup,
          //                     Item = q.c_iteno,
          //                     ItemName = q.v_itnam,
          //                     ItemUndes = q.v_undes,
          //                     SOH_GOOD = (qSOH != null ? qSOH.GQty : 0),
          //                     SOH_BAD = (qSOH != null ? qSOH.BQty : 0),
          //                     ORD_QTY = (qORD != null ? (qORD.GQty + qORD.BQty) : 0),
          //                     SIT_GOOD = (qSIT != null ? qSIT.GQty : 0),
          //                     SIT_BAD = (qSIT != null ? qSIT.BQty : 0),
          //                     qREQ_QTY = (qREQ != null ? (qREQ.GQty + qREQ.BQty) : 0)
          //                   }).AsQueryable();

          //    //string tmp = curStok.Provider.ToString();               
          //    var qItm = (from q in db.FA_MasItms
          //                where ((q.l_hide.HasValue ? q.l_hide.Value : false) == false)
          //                  && ((q.l_aktif.HasValue ? q.l_aktif.Value : false) == true)
          //                  && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
          //                select new
          //                {
          //                  q.c_nosup,
          //                  q.c_iteno
          //                }).AsQueryable();

          //    if ((parameters != null) && (parameters.Count > 0))
          //    {
          //      

          //      foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
          //      {
          //        if (kvp.Value.IsCondition)
          //        {
          //          if (kvp.Value.IsLike)
          //          {
          //            paternLike = kvp.Value.Value.ToString();
          //            qItm = qItm.Like(kvp.Key, paternLike).AsQueryable();
          //          }
          //          else if (kvp.Value.IsIn)
          //          {
          //            qItm = qItm.In(kvp.Key, kvp.Value.Value).AsQueryable();
          //          }
          //          else
          //          {
          //            qItm = qItm.Where(kvp.Key, kvp.Value.Value).AsQueryable();
          //          }
          //        }
          //      }
          //    }

          //    nCount = qItm.Count();

          //    //nCount = curStok.Count();

          //    if ((parameters != null) && (parameters.Count > 0))
          //    {
          //      

          //      foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
          //      {
          //        if (kvp.Value.IsCondition)
          //        {
          //          if (kvp.Value.IsLike)
          //          {
          //            paternLike = kvp.Value.Value.ToString();
          //            curStok = curStok.Like(kvp.Key, paternLike).AsQueryable();
          //          }
          //          else if (kvp.Value.IsIn)
          //          {
          //            curStok = curStok.In(kvp.Key, kvp.Value.Value).AsQueryable();
          //          }
          //          else
          //          {
          //            curStok = curStok.Where(kvp.Key, kvp.Value.Value).AsQueryable();
          //          }
          //        }
          //      }
          //    }

          //    if (nCount > 0)
          //    {
          //      if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
          //      {
          //        curStok = curStok.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
          //      }

          //      if ((limit == -1) || allQuery)
          //      {
          //        dic.Add(Constant.DEFAULT_NAMING_RECORDS, curStok.ToList());
          //      }
          //      else
          //      {
          //        dic.Add(Constant.DEFAULT_NAMING_RECORDS, curStok.Skip(start).Take(limit).ToList());
          //      }
          //    }

          //    dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

          //    dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
          //  }
          //  break;

          //#region Old Coded

          ////case Constant.REPORT_INVENTORY_REPORT_CS:
          ////  {
          //  //  char gdgPosStok = (parameters.ContainsKey("gdgStok") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgStok"]).Value) : '0');
          //  //  char gdgPosTrx = (parameters.ContainsKey("gdgTrx") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgTrx"]).Value) : '0');

          //  //  List<char> lstGdgAvaible = new List<char>();
          //  //  lstGdgAvaible.Add('0');
          //  //  lstGdgAvaible.Add('1');

          //  //  List<bool> isImport = new List<bool>();
          //  //  isImport.Add(true);

          //  //  List<TempSPOKT> listSPOkt = new List<TempSPOKT>();
          //  //  List<TempNonSPOKT> listNonSPOkt = new List<TempNonSPOKT>();

          //  //  var test = (from q in
          //  //                (from q in db.LG_SPHs
          //  //                 join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
          //  //                 join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
          //  //                 join q3 in db.LG_Cusmas on q.c_cusno equals q3.c_cusno
          //  //                 where (DateTime.Now.Month - q.d_spdate.Value.Month) < 2
          //  //                 && (DateTime.Now.Year - q.d_spdate.Value.Year) == 0
          //  //                 && q2.c_type == (lstGdgAvaible.Contains((char)gdgPosTrx) ? "02" : "00")

          //  //                 select new TempSPOKT()
          //  //                 {
          //  //                   c_iteno = q1.c_iteno,
          //  //                   n_sisa = q1.n_sisa.Value
          //  //                 }).Union
          //  //                  (from q in db.LG_SPHs
          //  //                   join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
          //  //                   join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
          //  //                   join q3 in db.LG_Cusmas on q.c_cusno equals q3.c_cusno
          //  //                   where q2.c_type != "02" && (q3.c_gdg == gdgPosTrx || gdgPosTrx == '0')
          //  //                   && (DateTime.Now.Month - q.d_spdate.Value.Month) < 2
          //  //                   && (DateTime.Now.Year - q.d_spdate.Value.Year) == 0
          //  //                   select new TempSPOKT()
          //  //                 {
          //  //                   c_iteno = q1.c_iteno,
          //  //                   n_sisa = q1.n_sisa.Value
          //  //                 }).Union(
          //  //                  from q in db.LG_SPGHs
          //  //                  join q1 in db.LG_SPGD1s on q.c_spgno equals q1.c_spgno
          //  //                  where q.l_status == true && (q.c_gdg2 == gdgPosTrx || gdgPosTrx == '0')
          //  //                  && q1.n_sisa > 0 && (DateTime.Now.Month - q.d_spgdate.Value.Month) < 2
          //  //                   && (DateTime.Now.Year - q.d_spgdate.Value.Year) == 0
          //  //                  select new TempSPOKT()
          //  //                  {
          //  //                    c_iteno = q1.c_iteno,
          //  //                    n_sisa = q1.n_sisa.Value
          //  //                  })
          //  //              select new TempNonSPOKT()
          //  //               {
          //  //                 c_iteno = q.c_iteno,
          //  //                 n_sisa = q.n_sisa
          //  //               }).AsQueryable();

          //  //  var SPTot = (from q in test
          //  //               group q by new { q.c_iteno } into g
          //  //               select new
          //  //               {
          //  //                 c_iteno = g.Key.c_iteno,
          //  //                 n_sisa = g.Sum(x => x.n_sisa)
          //  //               }).AsQueryable();

          //  //  var VwStockTot = (from q in GlobalQuery.ViewStock(db)
          //  //                    where (q.c_gdg == gdgPosStok || gdgPosStok == '0')
          //  //                    group q by new { q.c_iteno } into g
          //  //                    select new
          //  //                    {
          //  //                      c_iteno = g.Key.c_iteno,
          //  //                      n_sisa = g.Sum(x => x.n_gsisa)
          //  //                    }).AsQueryable();

          //  //  var POSJ = (from q in
          //  //                (from q in db.LG_POHs
          //  //                 join q1 in db.LG_POD1s on q.c_pono equals q1.c_pono
          //  //                 where q1.n_sisa > 0 && (q.c_gdg == gdgPosTrx || gdgPosTrx == '0')
          //  //                 && (DateTime.Now.Month - q.d_podate.Value.Month) < (isImport.Contains((bool)q.l_import) ? 9999 : 2)
          //  //                 && (DateTime.Now.Year - q.d_podate.Value.Year) <= (isImport.Contains((bool)q.l_import) ? 9999 : 0)
          //  //                 select new TempSPOKT
          //  //                 {
          //  //                   c_iteno = q1.c_iteno,
          //  //                   n_sisa = q1.n_sisa.Value
          //  //                 }).Union(from q in db.LG_SPGHs
          //  //                          join q1 in db.LG_SPGD1s on q.c_spgno equals q1.c_spgno
          //  //                          where q1.n_sisa > 0 && (q1.c_gdg == gdgPosTrx || gdgPosTrx == '0')
          //  //                          && (DateTime.Now.Month - q.d_spgdate.Value.Month) < 2
          //  //                          && (DateTime.Now.Year - q.d_spgdate.Value.Year) == 0
          //  //                          select new TempSPOKT
          //  //                          {
          //  //                            c_iteno = q1.c_iteno,
          //  //                            n_sisa = q1.n_sisa.Value
          //  //                          })
          //  //              select new TempNonSPOKT()
          //  //              {
          //  //                c_iteno = q.c_iteno,
          //  //                n_sisa = q.n_sisa
          //  //              }).AsQueryable();

          //  //  var POSJTot = (from q in POSJ
          //  //                 group q by new { q.c_iteno } into g
          //  //                 select new
          //  //                 {
          //  //                   c_iteno = g.Key.c_iteno,
          //  //                   n_sisa = g.Sum(x => x.n_sisa)
          //  //                 }).AsQueryable();

          //  //  var SJTot = (from q in db.LG_SJHs
          //  //               join q1 in db.LG_SJD1s on q.c_sjno equals q1.c_sjno
          //  //               where q.l_status == false && (q.c_gdg2 == gdgPosTrx || gdgPosTrx == '0')
          //  //               && (DateTime.Now.Month - q.d_sjdate.Value.Month) < 2
          //  //                && (DateTime.Now.Year - q.d_sjdate.Value.Year) == 0
          //  //               group q1 by new { q1.c_iteno } into g
          //  //               select new
          //  //               {
          //  //                 g.Key.c_iteno,
          //  //                 n_gqty = g.Sum(x => x.n_gqty),
          //  //                 n_bqty = g.Sum(x => x.n_bqty)
          //  //               }).AsQueryable();

          //  //  var Cs = (from q in db.FA_MasItms
          //  //            join q1 in SPTot on q.c_iteno equals q1.c_iteno into gq1
          //  //            from Lq1 in gq1.DefaultIfEmpty()
          //  //            join q2 in VwStockTot on q.c_iteno equals q2.c_iteno into gq2
          //  //            from Lq2 in gq2.DefaultIfEmpty()
          //  //            join q3 in POSJTot on q.c_iteno equals q3.c_iteno into gq3
          //  //            from Lq3 in gq3.DefaultIfEmpty()
          //  //            join q4 in SJTot on q.c_iteno equals q4.c_iteno into gq4
          //  //            from Lq4 in gq4.DefaultIfEmpty()
          //  //            select new
          //  //            {
          //  //              q.c_iteno,
          //  //              q.v_itnam,
          //  //              q.v_undes,
          //  //              SisaSP = Lq1.n_sisa == null ? 0m : Lq1.n_sisa,
          //  //              SoH = Lq2.n_sisa == null ? 0m : Lq2.n_sisa.Value,
          //  //              siT = Lq4.n_gqty == null ? 0m : Lq4.n_gqty.Value,
          //  //              Po = Lq3.n_sisa == null ? 0m : Lq3.n_sisa
          //  //            }).AsQueryable();

          //  //  nCount = Cs.Count();

          //  //  if (nCount > 0)
          //  //  {
          //  //    if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
          //  //    {
          //  //      Cs = Cs.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
          //  //    }

          //  //    if ((limit == -1) || allQuery)
          //  //    {
          //  //      dic.Add(Constant.DEFAULT_NAMING_RECORDS, Cs.ToList());
          //  //    }
          //  //    else
          //  //    {
          //  //      dic.Add(Constant.DEFAULT_NAMING_RECORDS, Cs.Skip(start).Take(limit).ToList());
          //  //    }
          //  //  }

          //  //  dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

          //  //  dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

          //  //}
          //  //break;

          //#endregion

          #endregion

          //#region Forecast

          //case Constant.REPORT_FORECAST:
          //  {
          //      nCount = 0;
          //      //char gdgPosStok = (parameters.ContainsKey("gdgStok") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgStok"]).Value) : '0');
          //      string cabang = (parameters.ContainsKey("c_cab_dcore") ? Convert.ToString(((Functionals.ParameterParser)parameters["c_cab_dcore"]).Value) : string.Empty);
          //      string bulan = (parameters.ContainsKey("c_bulan") ? Convert.ToString(((Functionals.ParameterParser)parameters["c_bulan"]).Value).Trim() : string.Empty);
          //      string tahun = (parameters.ContainsKey("c_tahun") ? Convert.ToString(((Functionals.ParameterParser)parameters["c_tahun"]).Value).Trim() : string.Empty);
          //      if (bulan == "")
          //      { 
          //          bulan = DateTime.Now.ToString("MM");
          //          //bulan = "8";
          //          if (bulan.Substring(0, 1) == "0")
          //          {
          //              bulan = bulan.Substring(1, 1);
          //          }
          //      }
          //      if (tahun == "")
          //      {
          //          tahun = DateTime.Now.ToString("yyyy");
          //      }
          //      var forecast = (from q in db.tmp_forecasts
          //                      join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno into q_1
          //                      from qfaitm in q_1.DefaultIfEmpty()
          //                      where 
          //                          q.c_bulan == bulan && q.c_tahun == tahun
          //                          && (q.c_cab.Contains(cabang))
          //                      select new Forecast() 
          //                      { 
          //                      c_cab = q.c_cab,
          //                      c_iteno = q.c_iteno,
          //                      c_itnam = q.c_itnam,
          //                      uom = qfaitm.v_undes,
          //                      hna = (qfaitm.n_salpri != null ? qfaitm.n_salpri.Value : 0),
          //                      n_forecast = (q.n_forecast != null ? q.n_forecast.Value : 0),
          //                      forecast_value = (q.n_forecast.Value * qfaitm.n_salpri), 
          //                      avg_qty = (q.avg_qty != null ? q.avg_qty.Value : 0),
          //                      avg_value = (q.avg_qty.Value * qfaitm.n_salpri),
          //                      sls_to_dt = (q.sls_to_dt != null ? q.sls_to_dt.Value : 0),
          //                      sls_to_dt_value = (q.sls_to_dt.Value * qfaitm.n_salpri),
          //                      sls_to_go = (q.sls_to_go != null ? q.sls_to_go.Value : 0),
          //                      sls_to_go_value = (q.sls_to_go.Value * qfaitm.n_salpri),
          //                      sls = (q.sls != null ? q.sls.Value : 0),
          //                      var = (q.variant != null ? q.sls.Value : 0),
          //                      hit = (q.hit != null ? q.hit.Value : 0),
          //                      c_tahun = q.c_tahun,
          //                      c_bulan = q.c_bulan
          //                      }).ToList().AsQueryable();
          //      var zzz = forecast.ToList();
          //      nCount = forecast.Count();
          //      if (nCount > 0)
          //      {
          //          if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
          //          {
          //              forecast = forecast.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
          //          }

          //          if ((limit == -1) || allQuery)
          //          {
          //              dic.Add(Constant.DEFAULT_NAMING_RECORDS, forecast.ToList());
          //          }
          //          else
          //          {
          //              dic.Add(Constant.DEFAULT_NAMING_RECORDS, forecast.Skip(start).Take(limit).ToList());
          //          }
          //      }

          //      dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

          //      dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
          //  }
          //  break;

          //#endregion
          #region REPORT_INVENTORY_SPPLDO_DETAIL
          case Constant.REPORT_INVENTORY_SPPLDO_DETAIL:
            {
                char gdg = (parameters.ContainsKey("gdgStok") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgStok"]).Value) : '0');
                string itemCode = (parameters.ContainsKey("item_sot") ? Convert.ToString(((Functionals.ParameterParser)parameters["item_sot"]).Value).Trim() : string.Empty);
                nCount = 0;

                var qry = (from q in db.LG_SPPLDO2s
                               where (itemCode.Length > 0 ? q.c_no.StartsWith(itemCode) : true)
                               select q).ToList();
                nCount = qry.Count();
                if (nCount > 0)
                {

                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                }
                if (qry != null)
                {
                    qry.Clear();
                }

                dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
            }
            break;
          #endregion

          #region REPORT_INVENTORY_SPPLDO
          case Constant.REPORT_INVENTORY_SPPLDO:
            {
                char gdg = (parameters.ContainsKey("gdgStok") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgStok"]).Value) : '0');
                string[] MoveType = (parameters.ContainsKey("noSup") ? (string[])((Functionals.ParameterParser)parameters["noSup"]).Value : new string[0]);
                string[] cab = (parameters.ContainsKey("cab") ? (string[])((Functionals.ParameterParser)parameters["cab"]).Value : new string[0]);
                nCount = 0;

                var qry = (from q in db.LG_SPPLDO_HEADERs
                           where q.c_gdg.ToString() == gdg.ToString()
                           && (MoveType.Length > 0 ? MoveType.Contains(q.MOVETYPE) : true)
                           && (cab.Length > 0 ? cab.Contains(q.c_cab_dcore) : true)
                                select q).OrderByDescending(x => x.overdue).ToList();
                nCount = qry.Count();
                
                if (nCount > 0)
                {

                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                }

                if (qry != null)
                {
                    qry.Clear();
                }

                dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

            }
            break;
          #endregion

          #region Current Stok

          case Constant.REPORT_INVENTORY_REPORT_CS:
            {
              nCount = 0;

              char gdgPosStok = (parameters.ContainsKey("gdgStok") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgStok"]).Value) : '0');
              char gdgPosTrx = (parameters.ContainsKey("gdgStok") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgStok"]).Value) : '0');
              //char gdgPosTrx = (parameters.ContainsKey("gdgTrx") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgTrx"]).Value) : '0');
              string[] listNoSupl = (parameters.ContainsKey("noSup") ? (string[])((Functionals.ParameterParser)parameters["noSup"]).Value : new string[0]);
              string itemCode = (parameters.ContainsKey("itemCode") ? Convert.ToString(((Functionals.ParameterParser)parameters["itemCode"]).Value).Trim() : string.Empty);
              string itemName = (parameters.ContainsKey("itemName") ? Convert.ToString(((Functionals.ParameterParser)parameters["itemName"]).Value).Trim() : string.Empty);
              string itemUndes = (parameters.ContainsKey("itemUndes") ? Convert.ToString(((Functionals.ParameterParser)parameters["itemUndes"]).Value).Trim() : string.Empty);
              string itemSup = (parameters.ContainsKey("itemSup") ? Convert.ToString(((Functionals.ParameterParser)parameters["itemSup"]).Value).Trim() : string.Empty);
              string kddivams = (parameters.ContainsKey("kddivams") ? Convert.ToString(((Functionals.ParameterParser)parameters["kddivams"]).Value).Trim() : string.Empty);
              string kddivpri = (parameters.ContainsKey("kddivpri") ? Convert.ToString(((Functionals.ParameterParser)parameters["kddivpri"]).Value).Trim() : string.Empty);

              char[] avaibleGudangOkt = new char[] { '0', '1' };

              //List<string> listItems = null;\
              char gdgawal = gdgPosStok;
              char gdgakhir = gdgPosTrx;
              
              var curStokLst = (from qItm in db.FA_MasItms
                                join q2 in db.LG_DatSups on qItm.c_nosup equals q2.c_nosup
                                join q3 in db.FA_DivAMs on qItm.c_iteno equals q3.c_iteno into q_4
                                from qDivAMS in q_4.DefaultIfEmpty()
                                join q5 in db.FA_MsDivAMs on qDivAMS.c_kddivams equals q5.c_kddivams
                                join q6 in db.FA_Divpris on qItm.c_iteno equals q6.c_iteno into q_7
                                from qDivPri in q_7.DefaultIfEmpty()
                                join q8 in db.FA_MsDivPris on qDivPri.c_kddivpri equals q8.c_kddivpri
                                where (listNoSupl.Length == 0 ? true : listNoSupl.Contains(qItm.c_nosup))
                                  && (itemCode.Length > 0 ? qItm.c_iteno.StartsWith(itemCode) : true)
                                  && (itemSup.Length > 0 ? qItm.c_nosup.StartsWith(itemSup) : true)
                                  && (itemName.Length > 0 ? qItm.v_itnam.Contains(itemName) : true)
                                  && (itemUndes.Length > 0 ? qItm.v_undes.Contains(itemUndes) : true)
                                  && (kddivams.Length > 0 ? qDivAMS.c_kddivams.Contains(kddivams) : true)
                                  && (kddivpri.Length > 0 ? qDivPri.c_kddivpri.Contains(kddivpri) : true)
                                select new CurrentStockItemFilter()
                                {
                                  Item = (qItm.c_iteno == null ? string.Empty : qItm.c_iteno.Trim()),
                                  ItemName = (qItm.v_itnam == null ? string.Empty : qItm.v_itnam.Trim()),
                                  ItemUndes = (qItm.v_undes == null ? string.Empty : qItm.v_undes.Trim()),
                                  ItemPrice = qItm.n_salpri.ToString(),
                                  IsAktif = (qItm.l_aktif.HasValue ? qItm.l_aktif.Value : false),
                                  IsHide = (qItm.l_hide.HasValue ? qItm.l_hide.Value : false),
                                  KodeDatsup = (qItm.c_nosup == null ? string.Empty : qItm.c_nosup.Trim()),
                                  NamaDatsup = (q2.v_nama == null ? string.Empty : q2.v_nama.Trim()),
                                  //add by hafizh 13 maret 2018
                                  KodeDivAms = (qDivAMS.c_kddivams == null ? string.Empty : qDivAMS.c_kddivams.Trim()),
                                  KodeDivPrin = (qDivPri.c_kddivpri == null ? string.Empty : qDivPri.c_kddivpri.Trim()),

                                  NamaDivAms = (q5.v_nmdivams == null ? string.Empty : q5.v_nmdivams.Trim()),
                                  NamaDivPrin = (q8.v_nmdivpri == null ? string.Empty : q8.v_nmdivpri.Trim()),
                                }).ToList();

              //var curStokLst = qryStokList.Select(x =>
              //  new CurrentStockItemFilter()
              //  {
              //    Item = (x.c_iteno == null ? string.Empty : x.c_iteno.Trim()),
              //    ItemName = (x.v_itnam == null ? string.Empty : x.v_itnam.Trim()),
              //    ItemUndes = (x.v_undes == null ? string.Empty : x.v_undes.Trim())
              //  }).ToList();

              //listItems = curStokLst.GroupBy(x => x.Item).Select(y => y.Key).ToList();

              #region Stok On Hand atau stock Good dan Bad
              // ((q.c_type != "06") || ((q.c_type == "06") && (q.l_khusus == false))) 

                //old
              //var sohRN = (from q in db.LG_RNHs
              //             join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
              //             join qItm in db.FA_MasItms on q1.c_iteno equals qItm.c_iteno
              //             where ((q.c_type != "06") || ((q.l_khusus.HasValue ? q.l_khusus.Value : false) == false)) && ((q1.n_gsisa != 0) || (q1.n_bsisa != 0))
              //               && ((gdgPosStok == '0' ? q.c_gdg : gdgPosStok) == q.c_gdg)
              //               //&& (listItems == null ? false : listItems.Contains(q1.c_iteno))
              //               && (listNoSupl.Length == 0 ? true : listNoSupl.Contains(qItm.c_nosup))
              //               && (itemCode.Length > 0 ? qItm.c_iteno.StartsWith(itemCode) : true)
              //               && (itemName.Length > 0 ? qItm.v_itnam.Contains(itemName) : true)
              //               && (itemUndes.Length > 0 ? qItm.v_undes.Contains(itemUndes) : true)
              //               && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
              //             select new TempStokLogic()
              //             {
              //               RefNo = q.c_rnno,
              //               Item = q1.c_iteno,
              //               GQty = (q1.n_gsisa.HasValue ? q1.n_gsisa.Value : 0),
              //               BQty = (q1.n_bsisa.HasValue ? q1.n_bsisa.Value : 0)
              //             }).AsQueryable();



              if (gdgPosTrx == '1')
              {
                  gdgPosTrx = '1';
              }

              else if (gdgPosTrx == '2')
              {
                  gdgPosTrx = '2';
              }

              else if (gdgPosTrx == '0')
              {
                  gdgPosTrx = '0';
              }

              var sohRN = (//from q in db.vwStockhargas
                  
                  
                            from q in db.LG_vwStocks
                           //join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                           join qItm in db.FA_MasItms on q.c_iteno equals qItm.c_iteno

                           //join  q2 in db.vwStockhargas on q.c_iteno equals q2.c_iteno

                           where //((q.n_gsisa != 0) || (q.n_bsisa != 0))
                              ((gdgPosStok == '0' ? q.c_gdg : gdgPosStok) == q.c_gdg)
                               //&& (listItems == null ? false : listItems.Contains(q1.c_iteno))
                              //((gdgPosTrx == '0' ? q.c_gdg : gdgPosTrx) == q.c_gdg)
                             && (listNoSupl.Length == 0 ? true : listNoSupl.Contains(qItm.c_nosup))
                             && (itemCode.Length > 0 ? q.c_iteno.StartsWith(itemCode) : true)
                             && (itemName.Length > 0 ? qItm.v_itnam.Contains(itemName) : true)
                             && (itemUndes.Length > 0 ? qItm.v_undes.Contains(itemUndes) : true)
                             //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                           select new TempStokLogic()
                           {
                               RefNo = q.c_no,
                               Item = q.c_iteno,
                               GQty = (q.n_gsisa.HasValue ? q.n_gsisa.Value : 0),
                               BQty = (q.n_bsisa.HasValue ? q.n_bsisa.Value : 0),

                               //total_good = (q.value_n_gsisa.HasValue ? q.value_n_gsisa.Value : 0),
                               //total_bad = (q.value_n_bsisa.HasValue ? q.value_n_bsisa.Value : 0), 
                           }).AsQueryable();

              //var sohCombo = (from q in db.LG_ComboHs
              //                join qItm in db.FA_MasItms on q.c_iteno equals qItm.c_iteno
              //                where ((q.n_gsisa != 0) || (q.n_bsisa != 0))
              //                  && ((gdgPosStok == '0' ? q.c_gdg : gdgPosStok) == q.c_gdg)
              //                  && ((q.l_confirm.HasValue ? q.l_confirm.Value : false) == true)
              //                  //&& (listItems == null ? false : listItems.Contains(q.c_iteno))
              //                  && (listNoSupl.Length == 0 ? true : listNoSupl.Contains(qItm.c_nosup))
              //                  && (itemCode.Length > 0 ? qItm.c_iteno.StartsWith(itemCode) : true)
              //                  && (itemName.Length > 0 ? qItm.v_itnam.Contains(itemName) : true)
              //                  && (itemUndes.Length > 0 ? qItm.v_undes.Contains(itemUndes) : true)
              //                  && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
              //                select new TempStokLogic()
              //                {
              //                  RefNo = q.c_combono,
              //                  Item = q.c_iteno,
              //                  GQty = (q.n_gsisa.HasValue ? q.n_gsisa.Value : 0),
              //                  BQty = (q.n_bsisa.HasValue ? q.n_bsisa.Value : 0)
              //                }).AsQueryable();

              var soh = sohRN.GroupBy(x =>
                      x.Item
                    ).Select(y => new TempStokLogic()
                    {
                      Item = y.Key,
                      RefNo = y.Key,
                      GQty = y.Sum(z => z.GQty),
                      BQty = y.Sum(z => z.BQty),

                      //total_good = y.Sum(z => z.total_good),

                      //total_bad = y.Sum(z => z.total_bad),

                      
                    }).ToList();

              var aa = soh.ToList();
              gdgPosStok = gdgawal;
              gdgPosTrx = gdgakhir;

              #region DC dan Ulujami
              if (gdgPosTrx == '7')
              {
                  sohRN = (//from q in db.vwStockhargas
                            from q in db.LG_vwStocks
                            //join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                            join qItm in db.FA_MasItms on q.c_iteno equals qItm.c_iteno

                            //join  q2 in db.vwStockhargas on q.c_iteno equals q2.c_iteno

                            where //((q.n_gsisa != 0) || (q.n_bsisa != 0))
                                //((gdgPosStok == '0' ? q.c_gdg : gdgPosStok) == q.c_gdg)
                                   (q.c_gdg == '1' || q.c_gdg == '6')
                                //&& (listItems == null ? false : listItems.Contains(q1.c_iteno))
                                //((gdgPosTrx == '0' ? q.c_gdg : gdgPosTrx) == q.c_gdg)
                              && (listNoSupl.Length == 0 ? true : listNoSupl.Contains(qItm.c_nosup))
                              && (itemCode.Length > 0 ? q.c_iteno.StartsWith(itemCode) : true)
                              && (itemName.Length > 0 ? qItm.v_itnam.Contains(itemName) : true)
                              && (itemUndes.Length > 0 ? qItm.v_undes.Contains(itemUndes) : true)
                            //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                            select new TempStokLogic()
                            {
                                RefNo = q.c_no,
                                Item = q.c_iteno,
                                GQty = (q.n_gsisa.HasValue ? q.n_gsisa.Value : 0),
                                BQty = (q.n_bsisa.HasValue ? q.n_bsisa.Value : 0),

                                //total_good = (q.value_n_gsisa.HasValue ? q.value_n_gsisa.Value : 0),
                                //total_bad = (q.value_n_bsisa.HasValue ? q.value_n_bsisa.Value : 0), 
                            }).AsQueryable();
                  
                  soh = sohRN.GroupBy(x =>
                      x.Item
                    ).Select(y => new TempStokLogic()
                    {
                        Item = y.Key,
                        RefNo = y.Key,
                        GQty = y.Sum(z => z.GQty),
                        BQty = y.Sum(z => z.BQty),

                        //total_good = y.Sum(z => z.total_good),

                        //total_bad = y.Sum(z => z.total_bad),


                    }).ToList();
                  aa = soh.ToList();
              }
              #endregion
              //soh.Dump("Hasil");

              #endregion

              #region PO 
              //  old
              //var reqPO = (from q in db.LG_POHs
              //             join q1 in db.LG_POD1s on new { q.c_gdg, q.c_pono } equals new { q1.c_gdg, q1.c_pono }
              //             join qItm in db.FA_MasItms on q1.c_iteno equals qItm.c_iteno
              //             where (q1.n_sisa > 0)
              //              && (SqlMethods.DateDiffMonth(q.d_podate, DateTime.Now) <
              //                  ((q.l_import.HasValue ? q.l_import.Value : false) ? int.MaxValue : 2))
              //              && ((gdgPosTrx == '0' ? q.c_gdg : gdgPosTrx) == q.c_gdg)
              //              && (listNoSupl.Length == 0 ? true : listNoSupl.Contains(qItm.c_nosup))
              //              && (itemCode.Length > 0 ? qItm.c_iteno.StartsWith(itemCode) : true)
              //              && (itemName.Length > 0 ? qItm.v_itnam.Contains(itemName) : true)
              //              && (itemUndes.Length > 0 ? qItm.v_undes.Contains(itemUndes) : true)
              //              && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
              //             select new TempStokLogic()
              //             {
              //               RefNo = q.c_pono,
              //               Item = q1.c_iteno,
              //               GQty = (q1.n_sisa.HasValue ? q1.n_sisa.Value : 0),
              //               BQty = 0
              //             }).AsQueryable();

              if (gdgPosTrx == '2')
              {
                  gdgPosTrx = '7';
              }

              else if (gdgPosTrx == '0')
              {
                  gdgPosTrx = '1';
              }
              
              var reqPO = (from q in db.LG_vwPOPending_news
                           //join q1 in db.LG_POD1s on new { q.c_gdg, q.c_pono } equals new { q1.c_gdg, q1.c_pono }
                           join qItm in db.FA_MasItms on q.c_iteno equals qItm.c_iteno
                           where (q.n_sisa > 0)
                            //&& (SqlMethods.DateDiffMonth(q.d_podate, DateTime.Now) <
                             //   ((q.l_import.HasValue ? q.l_import.Value : false) ? int.MaxValue : 2))
                            && ((gdgPosTrx == '0' ? q.c_gdg : gdgPosTrx) == q.c_gdg)
                            && (listNoSupl.Length == 0 ? true : listNoSupl.Contains(qItm.c_nosup))
                            && (itemCode.Length > 0 ? qItm.c_iteno.StartsWith(itemCode) : true)
                            && (itemName.Length > 0 ? qItm.v_itnam.Contains(itemName) : true)
                            && (itemUndes.Length > 0 ? qItm.v_undes.Contains(itemUndes) : true)
                            //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                           select new TempStokLogic()
                           {
                               RefNo = q.c_pono,
                               Item = q.c_iteno,
                               GQty = (q.n_sisa.HasValue ? q.n_sisa.Value : 0),
                               BQty = 0
                           }).AsQueryable();

              //var reqSPG = (from q in db.LG_SPGHs
              //              join q1 in db.LG_SPGD1s on new { q.c_gdg1, q.c_spgno } equals new { c_gdg1 = q1.c_gdg, q1.c_spgno }
              //              join qItm in db.FA_MasItms on q1.c_iteno equals qItm.c_iteno
              //              where (q1.n_sisa > 0)
              //                && (SqlMethods.DateDiffMonth(q.d_spgdate, DateTime.Now) < 2)
              //                && ((gdgPosTrx == '0' ? q.c_gdg1 : gdgPosTrx) == q.c_gdg1)
              //                && (listNoSupl.Length == 0 ? true : listNoSupl.Contains(qItm.c_nosup))
              //                && (itemCode.Length > 0 ? qItm.c_iteno.StartsWith(itemCode) : true)
              //                && (itemName.Length > 0 ? qItm.v_itnam.Contains(itemName) : true)
              //                && (itemUndes.Length > 0 ? qItm.v_undes.Contains(itemUndes) : true)
              //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
              //              select new TempStokLogic()
              //              {
              //                RefNo = q.c_spgno,
              //                Item = q1.c_iteno,
              //                GQty = (q1.n_sisa.HasValue ? q1.n_sisa.Value : 0),
              //                BQty = 0
              //              }).AsQueryable();

              //sitSPG.Dump("Hasil");

              //var reqOrder = reqPO.Union(reqSPG).AsQueryable();	
              //var reqOrder = reqPO.Union(reqSPG).GroupBy(x =>
              var reqOrder = reqPO.GroupBy(x =>

                        x.Item
                      ).Select(y => new TempStokLogic()
                      {
                        Item = y.Key,
                        RefNo = y.Key,
                        GQty = y.Sum(z => z.GQty),
                        BQty = y.Sum(z => z.BQty)
                      }).ToList();

              //reqOrder.Dump("Hasil");

              gdgPosStok = gdgawal;
              gdgPosTrx = gdgakhir;

              #region DC dan Ulujami
              if (gdgPosTrx == '7')
              {
                  reqPO = (from q in db.LG_vwPOPending_news
                           //join q1 in db.LG_POD1s on new { q.c_gdg, q.c_pono } equals new { q1.c_gdg, q1.c_pono }
                           join qItm in db.FA_MasItms on q.c_iteno equals qItm.c_iteno
                           where (q.n_sisa > 0)
                               //&& (SqlMethods.DateDiffMonth(q.d_podate, DateTime.Now) <
                               //   ((q.l_import.HasValue ? q.l_import.Value : false) ? int.MaxValue : 2))
                            && q.c_gdg == '1'
                            && (listNoSupl.Length == 0 ? true : listNoSupl.Contains(qItm.c_nosup))
                            && (itemCode.Length > 0 ? qItm.c_iteno.StartsWith(itemCode) : true)
                            && (itemName.Length > 0 ? qItm.v_itnam.Contains(itemName) : true)
                            && (itemUndes.Length > 0 ? qItm.v_undes.Contains(itemUndes) : true)
                           //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                           select new TempStokLogic()
                           {
                               RefNo = q.c_pono,
                               Item = q.c_iteno,
                               GQty = (q.n_sisa.HasValue ? q.n_sisa.Value : 0),
                               BQty = 0
                           }).AsQueryable();
                  reqOrder = reqPO.GroupBy(x =>

                        x.Item
                      ).Select(y => new TempStokLogic()
                      {
                          Item = y.Key,
                          RefNo = y.Key,
                          GQty = y.Sum(z => z.GQty),
                          BQty = y.Sum(z => z.BQty)
                      }).ToList();
              }
              #endregion

              #endregion

              #region SPG


              if (gdgPosTrx == '1')
              {
                  gdgPosTrx = '9';
              }
              else if (gdgPosTrx == '2')
              {
                  gdgPosTrx = '2';
              }
              else if (gdgPosTrx == '0')
              {
                  gdgPosTrx = '2';
              }
              var reqSPG = (from q in db.LG_vwPOPending_news
                           join qItm in db.FA_MasItms on q.c_iteno equals qItm.c_iteno

                            where (q.n_sisa > 0)
                            && ((gdgPosTrx == '0' ? q.c_gdg : gdgPosTrx) == q.c_gdg)
                            //&& ((gdgPosStok == '0' ? q.c_gdg : gdgPosStok) == q.c_gdg)
                            && (listNoSupl.Length == 0 ? true : listNoSupl.Contains(qItm.c_nosup))
                            && (itemCode.Length > 0 ? qItm.c_iteno.StartsWith(itemCode) : true)
                            && (itemName.Length > 0 ? qItm.v_itnam.Contains(itemName) : true)
                            && (itemUndes.Length > 0 ? qItm.v_undes.Contains(itemUndes) : true)
                           //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                           select new TempStokLogic()
                           {
                               RefNo = q.c_pono,
                               Item = q.c_iteno,
                               GQty = (q.n_sisa.HasValue ? q.n_sisa.Value : 0),
                               BQty = 0,

                           }).AsQueryable();

              //var zzz = reqSPG.ToList();

              var reqSPG2 = reqSPG.GroupBy(x =>

                        x.Item
                      ).Select(y => new TempStokLogic()
                      {
                          Item = y.Key,
                          RefNo = y.Key,
                          GQty = y.Sum(z => z.GQty),
                          BQty = y.Sum(z => z.BQty),


                      }).ToList();

              //reqOrder.Dump("Hasil");

              //var bbb = reqSPG2.ToList();

              gdgPosStok = gdgawal;
              gdgPosTrx = gdgakhir;

              #region DC dan Ulujami
              if (gdgPosTrx == '7')
              {
                  reqSPG = (from q in db.LG_vwPOPending_news
                            join qItm in db.FA_MasItms on q.c_iteno equals qItm.c_iteno

                            where (q.n_sisa > 0)
                            && q.c_gdg == '9'
                                //&& ((gdgPosStok == '0' ? q.c_gdg : gdgPosStok) == q.c_gdg)
                            && (listNoSupl.Length == 0 ? true : listNoSupl.Contains(qItm.c_nosup))
                            && (itemCode.Length > 0 ? qItm.c_iteno.StartsWith(itemCode) : true)
                            && (itemName.Length > 0 ? qItm.v_itnam.Contains(itemName) : true)
                            && (itemUndes.Length > 0 ? qItm.v_undes.Contains(itemUndes) : true)
                            //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                            select new TempStokLogic()
                            {
                                RefNo = q.c_pono,
                                Item = q.c_iteno,
                                GQty = (q.n_sisa.HasValue ? q.n_sisa.Value : 0),
                                BQty = 0,

                            }).AsQueryable();

                  reqSPG2 = reqSPG.GroupBy(x =>
                        x.Item
                      ).Select(y => new TempStokLogic()
                      {
                          Item = y.Key,
                          RefNo = y.Key,
                          GQty = y.Sum(z => z.GQty),
                          BQty = y.Sum(z => z.BQty),
                      }).ToList();
              }
              #endregion

              #endregion

              #region GIT IN

              if (gdgPosStok == '0')
              {
                  gdgPosStok = '9';
              }

              var sit = (from q in db.LG_SJHs
                         join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                         join qItm in db.FA_MasItms on q1.c_iteno equals qItm.c_iteno
                         where (q.l_status == false)
                           && ((gdgPosStok == '0' ? q.c_gdg2 : gdgPosStok) == q.c_gdg2)
                           && (listNoSupl.Length == 0 ? true : listNoSupl.Contains(qItm.c_nosup))
                           && (itemCode.Length > 0 ? qItm.c_iteno.StartsWith(itemCode) : true)
                           && (itemName.Length > 0 ? qItm.v_itnam.Contains(itemName) : true)
                           && (itemUndes.Length > 0 ? qItm.v_undes.Contains(itemUndes) : true)
                           && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                         select new TempStokLogic()
                         {
                           RefNo = q.c_sjno,
                           Item = q1.c_iteno,
                           GQty = (q1.n_gqty.HasValue ? q1.n_gqty.Value : 0),
                           BQty = (q1.n_bqty.HasValue ? q1.n_bqty.Value : 0)
                         }).ToList();

              sit = sit.GroupBy(x =>
                        x.Item
                      ).Select(y => new TempStokLogic()
                      {
                        Item = y.Key,
                        RefNo = y.Key,
                        GQty = y.Sum(z => z.GQty),
                        BQty = y.Sum(z => z.BQty)
                      }).ToList();

              //sit.Dump("Hasil");
              //var cc = sit.ToList();

              gdgPosStok = gdgawal;

              #region DC dan Ulujami
              if (gdgPosStok == '7')
              {
                  sit = (from q in db.LG_SJHs
                         join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                         join qItm in db.FA_MasItms on q1.c_iteno equals qItm.c_iteno
                         where (q.l_status == false)
                           && (q.c_gdg2 == '1' || q.c_gdg2 == '6') 
                           && (listNoSupl.Length == 0 ? true : listNoSupl.Contains(qItm.c_nosup))
                           && (itemCode.Length > 0 ? qItm.c_iteno.StartsWith(itemCode) : true)
                           && (itemName.Length > 0 ? qItm.v_itnam.Contains(itemName) : true)
                           && (itemUndes.Length > 0 ? qItm.v_undes.Contains(itemUndes) : true)
                           && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                         select new TempStokLogic()
                         {
                             RefNo = q.c_sjno,
                             Item = q1.c_iteno,
                             GQty = (q1.n_gqty.HasValue ? q1.n_gqty.Value : 0),
                             BQty = (q1.n_bqty.HasValue ? q1.n_bqty.Value : 0)
                         }).ToList();
                  sit = sit.GroupBy(x =>
                        x.Item
                      ).Select(y => new TempStokLogic()
                      {
                          Item = y.Key,
                          RefNo = y.Key,
                          GQty = y.Sum(z => z.GQty),
                          BQty = y.Sum(z => z.BQty)
                      }).ToList();
              }
              #endregion

              #endregion


              #region GIT OUT


              if (gdgPosStok == '0')
              {
                  gdgPosStok = '0';
              }

              var got = (from q in db.vwSJdanDO_GOTs
                  
                         where (q.l_status == false)
                          && ((gdgPosStok == '0' ? q.gudang_asal : gdgPosStok) == q.gudang_asal)
                             //&& ((gdgPosTrx == '0' ? q.c_gdg2 : gdgPosTrx) != q.c_gdg2)
                          && (listNoSupl.Length == 0 ? true : listNoSupl.Contains(q.c_nosup))
                          && (itemCode.Length > 0 ? q.c_iteno.StartsWith(itemCode) : true)
                          && (itemName.Length > 0 ? q.v_itnam.Contains(itemName) : true)
                          && (itemUndes.Length > 0 ? q.v_undes.Contains(itemUndes) : true)
                          && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)

                         select new TempStokLogic()
                         {
                             RefNo = q.c_sjno,
                             Item = q.c_iteno,
                             GQty = (q.n_gqty.HasValue ? q.n_gqty.Value : 0),
                             BQty = (q.n_bqty.HasValue ? q.n_bqty.Value : 0),

                         }).ToList();

              got = got.GroupBy(x =>
                        x.Item
                      ).Select(y => new TempStokLogic()
                      {
                          Item = y.Key,
                          RefNo = y.Key,
                          GQty = y.Sum(z => z.GQty),
                          BQty = y.Sum(z => z.BQty),
                          
                      }).ToList();

              //sit.Dump("Hasil");
              //var uuuuu = got.ToList();

              gdgPosStok = gdgawal;
              gdgPosTrx = gdgakhir;

              #region DC dan Ulujami

              if (gdgPosStok == '7')
              {
                  got = (from q in db.vwSJdanDO_GOTs

                         where (q.l_status == false)
                          && (q.gudang_asal == '1' || q.gudang_asal == '6')
                             //&& ((gdgPosTrx == '0' ? q.c_gdg2 : gdgPosTrx) != q.c_gdg2)
                          && (listNoSupl.Length == 0 ? true : listNoSupl.Contains(q.c_nosup))
                          && (itemCode.Length > 0 ? q.c_iteno.StartsWith(itemCode) : true)
                          && (itemName.Length > 0 ? q.v_itnam.Contains(itemName) : true)
                          && (itemUndes.Length > 0 ? q.v_undes.Contains(itemUndes) : true)
                          && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)

                         select new TempStokLogic()
                         {
                             RefNo = q.c_sjno,
                             Item = q.c_iteno,
                             GQty = (q.n_gqty.HasValue ? q.n_gqty.Value : 0),
                             BQty = (q.n_bqty.HasValue ? q.n_bqty.Value : 0),

                         }).ToList();
                              
                  got = got.GroupBy(x =>
                      x.Item
                    ).Select(y => new TempStokLogic()
                    {
                        Item = y.Key,
                        RefNo = y.Key,
                        GQty = y.Sum(z => z.GQty),
                        BQty = y.Sum(z => z.BQty),

                    }).ToList();

              }

              #endregion

              #endregion

              //PL Boking Hafizh 08 maret 2018
              #region PL Boking


              if (gdgPosStok == '0')
              {
                  gdgPosStok = '0';
              }

              var Plboking = (from q in db.vwPLBokings

                              where //(q.l_status == false)
                                  //&& 
                               ((gdgPosStok == '0' ? q.c_gdg : gdgPosStok) == q.c_gdg)
                                  //&& ((gdgPosTrx == '0' ? q.c_gdg2 : gdgPosTrx) != q.c_gdg2)
                               && (listNoSupl.Length == 0 ? true : listNoSupl.Contains(q.c_nosup))
                               && (itemCode.Length > 0 ? q.c_iteno.StartsWith(itemCode) : true)
                               && (itemName.Length > 0 ? q.v_itnam.Contains(itemName) : true)
                               && (itemUndes.Length > 0 ? q.v_undes.Contains(itemUndes) : true)
                               && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)

                              select new TempStokLogic()
                              {
                                  RefNo = q.c_plno,
                                  Item = q.c_iteno,
                                  GQty = (q.n_qty.HasValue ? q.n_qty.Value : 0),
                                  BQty = (q.n_sisa.HasValue ? q.n_sisa.Value : 0),

                              }).ToList();

              Plboking = Plboking.GroupBy(x =>
                        x.Item
                      ).Select(y => new TempStokLogic()
                      {
                          Item = y.Key,
                          RefNo = y.Key,
                          GQty = y.Sum(z => z.GQty),
                          BQty = y.Sum(z => z.BQty),

                      }).ToList();

              //sit.Dump("Hasil");
              //var uuuuu = got.ToList();

              gdgPosStok = gdgawal;
              gdgPosTrx = gdgakhir;

              #endregion



              #region  Surat Pesanan

              //var spCabReg = (from q in db.LG_SPHs
              //                join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
              //                join q2 in db.LG_Cusmas on q.c_cusno equals q2.c_cusno
              //                join q3 in db.FA_MasItms on q1.c_iteno equals q3.c_iteno
              //                join qItm in db.FA_MasItms on q1.c_iteno equals qItm.c_iteno
              //                where (q1.n_sisa > 0)
              //                  && (q2.n_days.HasValue ? (SqlMethods.DateDiffDay(q.d_spdate, DateTime.Now.Date) <= q2.n_days) :
              //                                        (SqlMethods.DateDiffMonth(q.d_spdate, DateTime.Now.Date) < 2)) 
              //                  && ((gdgPosTrx == '0' ? q2.c_gdg : gdgPosTrx) == q2.c_gdg)
              //                  && (listNoSupl.Length == 0 ? true : listNoSupl.Contains(qItm.c_nosup))
              //                  && (itemCode.Length > 0 ? qItm.c_iteno.StartsWith(itemCode) : true)
              //                  && (itemName.Length > 0 ? qItm.v_itnam.Contains(itemName) : true)
              //                  && (itemUndes.Length > 0 ? qItm.v_undes.Contains(itemUndes) : true)
              //                  && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
              //                  && (q3.c_type != "02")
              //                select new TempStokLogic()
              //                {
              //                  RefNo = q.c_spno,
              //                  Item = q1.c_iteno,
              //                  GQty = (q1.n_sisa.HasValue ? q1.n_sisa.Value : 0),
              //                  BQty = 0
              //                }).AsQueryable();



              if (gdgPosTrx == '1')
              {
                  gdgPosTrx = '1';
              }

              else if (gdgPosTrx == '2')
              {
                  gdgPosTrx = '2';
              }
              else if (gdgPosTrx == '0')
              {
                  gdgPosTrx = '0';
              }

              var spnew = (from q in db.LG_vwSPPending_PSPs
                              //join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
                              //join q2 in db.LG_Cusmas on q.c_cusno equals q2.c_cusno
                              join q3 in db.FA_MasItms on q.c_iteno equals q3.c_iteno
                              join q4 in db.SCMS_MSITEM_CATs on q.c_iteno equals q4.c_iteno
                              //join qItm in db.FA_MasItms on q1.c_iteno equals qItm.c_iteno
                                where
                           (listNoSupl.Length == 0 ? true : listNoSupl.Contains(q3.c_nosup))
                             && (itemCode.Length > 0 ? q3.c_iteno.StartsWith(itemCode) : true)
                                && ((gdgPosTrx == '0' ? q.c_gdg : gdgPosTrx) == q.c_gdg)
                                //&& ((gdgPosStok == '0' ? q.c_gdg : gdgPosStok) == q.c_gdg)
                                //&& (q3.c_type != "02" ) && (q4.c_type != "07")
                                && (q.c_gdg ==
                                (q3.c_type == "02" ? (avaibleGudangOkt.Contains(gdgPosTrx) ? q.c_gdg : '0') :
                                    (q4.c_type == "07" ? (avaibleGudangOkt.Contains(gdgPosTrx) ? q.c_gdg : '0') : (gdgPosTrx == '0' ? q.c_gdg : gdgPosTrx))))
                                //&& ((q3.c_type == "02" ? q.c_gdg : gdgPosTrx) == q.c_gdg)
                              select new TempStokLogic()
                              {
                                  RefNo = q.c_spno,
                                  Item = q.c_iteno,
                                  GQty = (q.n_sisasp.HasValue ? q.n_sisasp.Value : 0),
                                  BQty = 0
                              }).AsQueryable();

              //var spnew = spReg.Union(spOktPre).AsQueryable();
              //spCabReg.Dump("Hasil");

              //var spCabOkt = (from q in db.LG_SPHs
              //                join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
              //                join qItm in db.FA_MasItms on q1.c_iteno equals qItm.c_iteno
              //                join qCusmas in db.LG_Cusmas on q.c_cusno equals qCusmas.c_cusno
              //                where (q1.n_sisa > 0)
              //                  && ((avaibleGudangOkt.Contains(gdgPosTrx) ? "02" : "XX") == qItm.c_type)
              //                  && (qCusmas.n_days.HasValue ? (SqlMethods.DateDiffDay(q.d_spdate, DateTime.Now.Date) <= qCusmas.n_days) :
              //                                        (SqlMethods.DateDiffMonth(q.d_spdate, DateTime.Now.Date) < 2)) 
              //                  && (listNoSupl.Length == 0 ? true : listNoSupl.Contains(qItm.c_nosup))
              //                  && (itemCode.Length > 0 ? qItm.c_iteno.StartsWith(itemCode) : true)
              //                  && (itemName.Length > 0 ? qItm.v_itnam.Contains(itemName) : true)
              //                  && (itemUndes.Length > 0 ? qItm.v_undes.Contains(itemUndes) : true)
              //                  && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
              //                select new TempStokLogic()
              //                {
              //                  RefNo = q.c_spno,
              //                  Item = q1.c_iteno,
              //                  GQty = (q1.n_sisa.HasValue ? q1.n_sisa.Value : 0),
              //                  BQty = 0
              //                }).AsQueryable();

              //spCabOkt.Dump("Hasil");

              //var spSPG = (from q in db.LG_SPGHs
              //             join q1 in db.LG_SPGD1s on new { q.c_gdg1, q.c_spgno } equals new { c_gdg1 = q1.c_gdg, q1.c_spgno }
              //             join qItm in db.FA_MasItms on q1.c_iteno equals qItm.c_iteno
              //    I-\         where (q.l_status == true) && (q1.n_sisa > 0)


              //               && ((gdgPosTrx == '0' ? q.c_gdg2 : gdgPosTrx) == q.c_gdg2)
              //               && (SqlMethods.DateDiffMonth(q.d_spgdate, DateTime.Now) < 2)
              //               && (listNoSupl.Length == 0 ? true : listNoSupl.Contains(qItm.c_nosup))
              //               && (itemCode.Length > 0 ? qItm.c_iteno.StartsWith(itemCode) : true)
              //               && (itemName.Length > 0 ? qItm.v_itnam.Contains(itemName) : true)
              //               && (itemUndes.Length > 0 ? qItm.v_undes.Contains(itemUndes) : true)
              //               && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
              //             select new TempStokLogic()
              //             {
              //               RefNo = q.c_spgno,
              //               Item = q1.c_iteno,
              //               GQty = (q1.n_sisa.HasValue ? q1.n_sisa.Value : 0),
              //               BQty = 0
              //             }).AsQueryable();

              //var spReq = spCabReg.Union(spCabOkt).Union(spSPG).AsQueryable();
              //var spReq = spnew.Union(spCabOkt).Union(spSPG)
              //        .GroupBy(x =>
              //          x.Item
              //        ).Select(y => new TempStokLogic()
              //        {
              //          Item = y.Key,
              //          RefNo = y.Key,
              //          GQty = y.Sum(z => z.GQty),
              //          BQty = y.Sum(z => z.BQty)
              //        }).ToList();

              var spReq = spnew.GroupBy(x =>
                      x.Item
                    ).Select(y => new TempStokLogic()
                    {
                        Item = y.Key,
                        RefNo = y.Key,
                        GQty = y.Sum(z => z.GQty),
                        BQty = y.Sum(z => z.BQty)
                      }).ToList();

              if (gdgPosTrx == '7')
              {
                  spnew = (from q in db.LG_vwSPPending_PSPs
                           //join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
                           //join q2 in db.LG_Cusmas on q.c_cusno equals q2.c_cusno
                           join q3 in db.FA_MasItms on q.c_iteno equals q3.c_iteno
                           join q4 in db.SCMS_MSITEM_CATs on q.c_iteno equals q4.c_iteno
                           //join qItm in db.FA_MasItms on q1.c_iteno equals qItm.c_iteno
                           where (listNoSupl.Length == 0 ? true : listNoSupl.Contains(q3.c_nosup))
                             && (itemCode.Length > 0 ? q3.c_iteno.StartsWith(itemCode) : true)
                             //&& ((gdgPosTrx == '0' ? q.c_gdg : gdgPosTrx) == q.c_gdg)
                               //&& ((gdgPosStok == '0' ? q.c_gdg : gdgPosStok) == q.c_gdg)
                               //&& (q3.c_type != "02" ) && (q4.c_type != "07")
                             && (q.c_gdg == '1')
                           //(q3.c_type == "02" ? (avaibleGudangOkt.Contains(gdgPosTrx) ? q.c_gdg : '0') :
                           //    (q4.c_type == "07" ? (avaibleGudangOkt.Contains(gdgPosTrx) ? q.c_gdg : '0') : (gdgPosTrx == '0' ? q.c_gdg : gdgPosTrx))))
                           //&& ((q3.c_type == "02" ? q.c_gdg : gdgPosTrx) == q.c_gdg)
                           select new TempStokLogic()
                           {
                               RefNo = q.c_spno,
                               Item = q.c_iteno,
                               GQty = (q.n_sisasp.HasValue ? q.n_sisasp.Value : 0),
                               BQty = 0
                           }).AsQueryable();
                  spReq = spnew.GroupBy(x =>
                      x.Item
                    ).Select(y => new TempStokLogic()
                    {
                        Item = y.Key,
                        RefNo = y.Key,
                        GQty = y.Sum(z => z.GQty),
                        BQty = y.Sum(z => z.BQty)
                    }).ToList();
              }
              //spReq.Dump("Hasil");

              #endregion

              var curStokNew2 = (from q in curStokLst
                                 join q1 in soh on q.Item equals q1.Item into q_1
                                 from qSOH in q_1.DefaultIfEmpty()
                                 join q2 in reqOrder on q.Item equals q2.Item into q_2
                                 from qORD in q_2.DefaultIfEmpty()
                                 join q3 in sit on q.Item equals q3.Item into q_3
                                 from qSIT in q_3.DefaultIfEmpty()
                                 join q4 in spReq on q.Item equals q4.Item into q_4
                                 from qREQ in q_4.DefaultIfEmpty()
                                 join q5 in got on q.Item equals q5.Item into q_5
                                 from qSOT in q_5.DefaultIfEmpty()
                                 join q6 in reqSPG2 on q.Item equals q6.Item into q_6
                                 from qSPG in q_6.DefaultIfEmpty()
                                 join q7 in Plboking on q.Item equals q7.Item into q_7
                                 from qPLBok in q_7.DefaultIfEmpty()



                                 select new TempStokLogicFull()
                                 {
                                     Item = q.Item,
                                     ItemName = q.ItemName,
                                     KodeDatsup = q.KodeDatsup,
                                     NamaDatsup = q.NamaDatsup,
                                     KodeDivAms = q.KodeDivAms,
                                     NamaDivAms = q.NamaDivAms,
                                     KodeDivPrin = q.KodeDivPrin,
                                     NamaDivPrin = q.NamaDivPrin,
                                     ItemUndes = q.ItemUndes,
                                     SOH_GOOD = (qSOH != null ? qSOH.GQty : 0),
                                     SOH_BAD = (qSOH != null ? qSOH.BQty : 0),
                                     ORD_QTY = (qORD != null ? (qORD.GQty + qORD.BQty) : 0),
                                     SIT_QTY = (qSIT != null ? (qSIT.GQty + qSIT.BQty) : 0),
                                     SOT_QTY = (qSOT != null ? (qSOT.GQty + qSOT.BQty) : 0),  // GIT OUT
                                     SOT_QTY_GOOD = (qSOT != null ? (qSOT.GQty) : 0),  // GIT OUT GOOD
                                     SOT_QTY_BAD = (qSOT != null ? (qSOT.BQty) : 0),  // GIT OUT BAD
                                     PLBoking = (qPLBok != null ? (qPLBok.GQty) : 0),  // PL Boking
                                     qREQ_QTY = (qREQ != null ? (qREQ.GQty + qREQ.BQty) : 0),
                                     SPG_QTY = (qSPG != null ? (qSPG.GQty + qSPG.BQty) : 0),
                                     ItemPrice = q.ItemPrice,
                                     IsAktif = q.IsAktif,
                                     IsHide = q.IsHide,
                                     total_good = (qSOH != null ? qSOH.total_good : 0),
                                     total_bad = (qSOH != null ? qSOH.total_bad : 0),

                                 }).ToList().AsQueryable();



              //var xx = curStokNew2.ToList();



              var curStokNew3 = (from q in curStokNew2

                                 select new TempStokLogicFull_2()
                                 {
                                     Item = q.Item,
                                     ItemName = q.ItemName,
                                     KodeDatsup = q.KodeDatsup,
                                     NamaDatsup = q.NamaDatsup,
                                     KodeDivAms = q.KodeDivAms,
                                     NamaDivAms = q.NamaDivAms,
                                     KodeDivPrin = q.KodeDivPrin,
                                     NamaDivPrin = q.NamaDivPrin,
                                     ItemUndes = q.ItemUndes,
                                     SOH_GOOD = q.SOH_GOOD,
                                     SOH_BAD = q.SOH_BAD,
                                     ORD_QTY = q.ORD_QTY,
                                     SIT_QTY = q.SIT_QTY,
                                     SOT_QTY = q.SOT_QTY,
                                     SOT_QTY_GOOD = q.SOT_QTY_GOOD,
                                     SOT_QTY_BAD = q.SOT_QTY_BAD,
                                     qREQ_QTY = q.qREQ_QTY,
                                     SPG_QTY = q.SPG_QTY,
                                     ItemPrice = q.ItemPrice.ToString(),
                                     IsAktif = q.IsAktif,
                                     IsHide = q.IsHide,
                                     //total_good = (q.SOH_GOOD + q.SOT_QTY) * Convert.ToDecimal(q.ItemPrice),
                                     total_good = (q.SOH_GOOD),
                                     total_bad = (q.SOH_BAD),
                                     PLBoking = q.PLBoking

                                 }).ToList().AsQueryable();

             // var ssssy = curStokNew3.ToList();
               



              var curStokNew = (from q in curStokNew3


                                select new TempStokLogicFull_3()
                                {
                                    Item = q.Item,
                                    ItemName = q.ItemName,
                                    KodeDatsup = q.KodeDatsup,
                                    NamaDatsup = q.NamaDatsup,
                                    KodeDivAms = q.KodeDivAms,
                                    NamaDivAms = q.NamaDivAms,
                                    KodeDivPrin = q.KodeDivPrin,
                                    NamaDivPrin = q.NamaDivPrin,
                                    ItemUndes = q.ItemUndes,
                                    SOH_GOOD = q.SOH_GOOD,
                                    SOH_BAD = q.SOH_BAD,
                                    ORD_QTY = q.ORD_QTY,
                                    SIT_QTY = q.SIT_QTY,
                                    SOT_QTY = q.SOT_QTY,
                                    qREQ_QTY = q.qREQ_QTY,
                                    SPG_QTY = q.SPG_QTY,
                                    //Total_Stock_All = (gdgawal == '1' ? (q.Total_Stock_1) : 
                                    //                   gdgawal == '2'? (q.Total_Stock_2): 
                                    //                   gdgawal == '0'? ((q.Total_Stock_1)+(q.Total_Stock_2)):0),

                                    Total_Stock_All = (q.SOH_GOOD + q.SOH_BAD + q.SOT_QTY),
                                    ItemPrice = q.ItemPrice.ToString(),
                                    IsAktif = q.IsAktif,
                                    IsHide = q.IsHide,
                                    //total_good = (q.SOH_GOOD + q.SOT_QTY) * Convert.ToDecimal(q.ItemPrice),
                                    //total_bad = (q.SOH_BAD + q.SOT_QTY) * Convert.ToDecimal(q.ItemPrice),
                                    total_good = q.total_good * Convert.ToDecimal(q.ItemPrice),
                                    total_bad = q.total_bad * Convert.ToDecimal(q.ItemPrice),
                                    PLBoking = q.PLBoking
                                }).ToList().AsQueryable();

              var ssss = curStokNew.ToList();

              //var curStokNew = (from q in curStokNew3


              //                  select new TempStokLogicFull2()
              //                  {
              //                      Item = q.Item,
              //                      ItemName = q.ItemName,
              //                      ItemUndes = q.ItemUndes,
              //                      SOH_GOOD = q.SOH_GOOD,
              //                      SOH_BAD = q.SOH_BAD,
              //                      ORD_QTY = q.ORD_QTY,
              //                      SIT_QTY = q.SIT_QTY,
              //                      qREQ_QTY = q.qREQ_QTY,
              //                      ItemPrice = q.ItemPrice,
              //                      IsAktif = q.IsAktif,
              //                      IsHide = q.IsHide,
              //                      total = q.SOH_GOOD * (q.ItemPrice),

              //                  }).ToList().AsQueryable();




              //curStokNew.Dump("Hasil");

              nCount = curStokNew.Count();

              soh.Clear();
              reqOrder.Clear();
              sit.Clear();
              spReq.Clear();
              curStokLst.Clear();

              if (nCount > 0)
              {
                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                {
                  curStokNew = curStokNew.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                }

                if ((limit == -1) || allQuery)
                {
                  dic.Add(Constant.DEFAULT_NAMING_RECORDS, curStokNew.ToList());
                }
                else
                {
                  dic.Add(Constant.DEFAULT_NAMING_RECORDS, curStokNew.Skip(start).Take(limit).ToList());
                }
              }

              if (curStokLst != null)
              {
                curStokLst.Clear();
              }
              if (reqOrder != null)
              {
                reqOrder.Clear();
              }
              if (sit != null)
              {
                sit.Clear();
              }
              if (spReq != null)
              {
                spReq.Clear();
              }

              dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

              dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
            }
            break;

          #endregion





          //SERVICE LEVEL AWAL

          #region Service Level PO

          case Constant.REPORT_HISTORY_SERVICELEVEL_PO:
            {
                nCount = 0;


                string txPeriode1 = (parameters.ContainsKey("txPeriode1") ? Convert.ToString(((Functionals.ParameterParser)parameters["txPeriode1"]).Value).Trim() : string.Empty);
                string txPeriode2 = (parameters.ContainsKey("txPeriode2") ? Convert.ToString(((Functionals.ParameterParser)parameters["txPeriode2"]).Value).Trim() : string.Empty);
                string listNoSupl2 = (parameters.ContainsKey("noSup2") ? Convert.ToString(((Functionals.ParameterParser)parameters["noSup2"]).Value).Trim() : string.Empty);
                string kddivams = (parameters.ContainsKey("kddivams") ? Convert.ToString(((Functionals.ParameterParser)parameters["kddivams"]).Value).Trim() : string.Empty);
                string kddivpri = (parameters.ContainsKey("kddivpri") ? Convert.ToString(((Functionals.ParameterParser)parameters["kddivpri"]).Value).Trim() : string.Empty);
                string fil_no_po = (parameters.ContainsKey("itemPOCode") ? Convert.ToString(((Functionals.ParameterParser)parameters["itemPOCode"]).Value).Trim() : string.Empty);
                string itemCode = (parameters.ContainsKey("itemCode") ? Convert.ToString(((Functionals.ParameterParser)parameters["itemCode"]).Value).Trim() : string.Empty);
                string itemUndes = (parameters.ContainsKey("itemUndes") ? Convert.ToString(((Functionals.ParameterParser)parameters["itemUndes"]).Value).Trim() : string.Empty);
                string itemSup = (parameters.ContainsKey("itemSup") ? Convert.ToString(((Functionals.ParameterParser)parameters["itemSup"]).Value).Trim() : string.Empty);
                string TypeService = (parameters.ContainsKey("TypeService") ? Convert.ToString(((Functionals.ParameterParser)parameters["TypeService"]).Value).Trim() : string.Empty);



                if (TypeService == "1")
                  
                {
                    var qry = (from q in db.vwService_Level_Fullfilment_ALL_NEW_POSEs
                               where
                              ((listNoSupl2.Length == 0) || q.c_nosup == listNoSupl2 && listNoSupl2.Length != 0)
                              && ((fil_no_po.Length == 0) || q.c_pono == fil_no_po && fil_no_po.Length != 0)
                              && q.ETA >= Convert.ToDateTime(txPeriode1)
                              && q.ETA <= Convert.ToDateTime(txPeriode2)

                             select new
                               {
                                   q.l_delete,
                                   q.c_nosup,
                                   q.v_nama,
                                   q.c_kddivams,
                                   q.v_nmdivams,
                                   q.c_kddivpri,
                                   q.v_nmdivpri,
                                   q.c_gdg,
                                   q.c_pono,
                                   q.c_iteno,
                                   q.v_itnam,
                                   q.Qty_PO,
                                   q.Tgl_Submit_PO,
                                   q.Tgl_Kirim_PO,
                                   q.LeadTime,
                                   q.ETA,
                                   q.Nomor_ST,
                                   q.Tgl_ST,
                                   q.Tgl_DO_Prin,
                                   q.Kode_DO_Prin,
                                   q.Nomor_RN,
                                   q.Tgl_RN,
                                   q.Qty_Received,
                                   q.Outstanding_PO,
                                   q.Value_PO_Hit_MIN,
                                   q.Value_PO_Hit_Plus,
                                   q.Total_Waktu_Pelayanan,
                                   q.Over_Lead_Time,
                                   q.Over_Lead_Time_Ket,
                                   q.On_Time_Order,
                                   q.Date_Minus,
                                   q.Date_Plus,
                                   q.Score_By_QTY,
                                   q.Score_By_Time,


                               }).Distinct().AsQueryable();





                    var fff = qry.ToList();
                    nCount = qry.Count();


                    if (nCount >= 0)
                    {
                        if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                        {
                            qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                        }

                        if ((limit == -1) || allQuery)
                        {
                            dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                        }
                        else
                        {
                            dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                            
                        }
                    }




                    dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                    dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);



                }
                else if (TypeService == "2")
                {
   
                    var qry = (from q in db.vwService_Level_Line_Fullfilment_ALL_NEW_PROSESSes
                               where
                              ((listNoSupl2.Length == 0) || q.c_nosup == listNoSupl2 && listNoSupl2.Length != 0)
                              && ((kddivams.Length == 0) || q.c_kddivams == kddivams && kddivams.Length != 0)
                              && ((kddivpri.Length == 0) || q.c_kddivpri == kddivpri && kddivpri.Length != 0)
                              && ((fil_no_po.Length == 0) || q.c_pono == fil_no_po && fil_no_po.Length != 0)
                              && ((itemCode.Length == 0) || q.c_iteno == itemCode && itemCode.Length != 0)
                              && q.ETA >= Convert.ToDateTime(txPeriode1)
                              && q.ETA <= Convert.ToDateTime(txPeriode2)

                             
                               select new
                               {
                                   q.l_delete,
                                   q.c_nosup,
                                   q.v_nama,
                                   q.c_kddivams,
                                   q.v_nmdivams,
                                   q.c_kddivpri,
                                   q.v_nmdivpri,
                                   q.c_gdg,
                                   q.c_pono,
                                   q.c_iteno,
                                   q.v_itnam,
                                   q.Qty_PO,
                                   q.Tgl_Submit_PO,
                                   q.Tgl_Kirim_PO,
                                   q.LeadTime,
                                   q.ETA,
                                   q.Nomor_ST,
                                   q.Tgl_ST,
                                   q.Tgl_DO_Prin,
                                   q.Kode_DO_Prin,
                                   q.Nomor_RN,
                                   q.Tgl_RN,
                                   q.Qty_Received,
                                   q.Outstanding_PO,
                                   q.Value_PO_Hit_MIN,
                                   q.Value_PO_Hit_Plus,
                                   q.Total_Waktu_Pelayanan,
                                   q.Over_Lead_Time,
                                   q.Over_Lead_Time_Ket,
                                   q.On_Time_Order,
                                   q.Date_Minus,
                                   q.Date_Plus,
                                   q.Score_By_QTY,
                                   q.Score_By_Time,
                                   
                               }).Distinct().AsQueryable();




                    var fff = qry.ToList();
                    nCount = qry.Count();


                    if (nCount > 0)
                    {
                        if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                        {
                            qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                        }

                        if ((limit == -1) || allQuery)
                        {
                            dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                        }
                        else
                        {
                            dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                        }
                    }




                    dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                    dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                
                }

 

            }
            break;

          #endregion

          //SERVICE LEVEL AKHIR

          //SERVICE LEVEL CABANG AWAL

          #region Service Level Cabang

          case Constant.REPORT_INVENTORY_SERVICELEVEL_CABANG:
            {
                nCount = 0;


                string fil_no_spho = (parameters.ContainsKey("Nomor_SP") ? Convert.ToString(((Functionals.ParameterParser)parameters["Nomor_SP"]).Value).Trim() : string.Empty);
                string fil_no_spcbg = (parameters.ContainsKey("Nomor_SP_Cabang") ? Convert.ToString(((Functionals.ParameterParser)parameters["Nomor_SP_Cabang"]).Value).Trim() : string.Empty);


                string txPeriode1 = (parameters.ContainsKey("txPeriode1") ? Convert.ToString(((Functionals.ParameterParser)parameters["txPeriode1"]).Value).Trim() : string.Empty);
                string txPeriode2 = (parameters.ContainsKey("txPeriode2") ? Convert.ToString(((Functionals.ParameterParser)parameters["txPeriode2"]).Value).Trim() : string.Empty);
                string TypeService = (parameters.ContainsKey("TypeService") ? Convert.ToString(((Functionals.ParameterParser)parameters["TypeService"]).Value).Trim() : string.Empty);
                string[] cabang = (parameters.ContainsKey("noSup") ? (string[])((Functionals.ParameterParser)parameters["noSup"]).Value : new string[0]);
                string uNip = (parameters.ContainsKey("Entry") ? (string)((Functionals.ParameterParser)parameters["Entry"]).Value : string.Empty);
                char Gdg = (parameters.ContainsKey("Gudang") ? Convert.ToChar(((Functionals.ParameterParser)parameters["Gudang"]).Value) : '0');


                DateTime now = DateTime.Now;
                var date1 = new DateTime(now.Year, now.Month, 1);
                var date2 = date1.AddMonths(1).AddDays(-1);
                int dif1 = 0, dif2 = 0;
                date1 = Convert.ToDateTime(txPeriode1);
                date2 = Convert.ToDateTime(txPeriode2);
                dif1 = SqlMethods.DateDiffDay(date1, DateTime.Now);
                dif2 = SqlMethods.DateDiffDay(date2, DateTime.Now);
                if (TypeService == "1")
                {
                    var qry = (from q in db.CSL_PPICs where SqlMethods.DateDiffDay(q.ETA_SP,DateTime.Now) <= dif1 && SqlMethods.DateDiffDay(q.ETA_SP,DateTime.Now) >= dif2 select q).ToList();
                    nCount = qry.Count();
                    if (nCount >= 0)
                    {
                        if ((limit == -1) || allQuery)
                        {
                            dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                        }
                        else
                        {
                            dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());

                        }
                    }
                    dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                    dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                }
                else if (TypeService == "2")
                {
                    var qry = (from q in db.NEW_CSL_TIMEs where SqlMethods.DateDiffDay(q.Tanggal_PL, DateTime.Now) <= dif1 && SqlMethods.DateDiffDay(q.Tanggal_PL, DateTime.Now) >= dif2 select q).ToList();
                    nCount = qry.Count();
                    if (nCount >= 0)
                    {
                        if ((limit == -1) || allQuery)
                        {
                            dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                        }
                        else
                        {
                            dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());

                        }
                    }
                    dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                    dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                }
                else if (TypeService == "3")
                {
                    #region Old
                    var qry = (from q in db.SL_Heds
                               where q.Tgl_SP_Entry >= Convert.ToDateTime(date1) && q.Tgl_SP_Entry <= Convert.ToDateTime(date2)
                                     && (cabang.Length == 0 ? true : cabang.Contains(q.Kode_Cabang.ToString()))
                                     && ((fil_no_spho.Length == 0) || q.Nomor_SP == fil_no_spho && fil_no_spho.Length != 0)
                                     && ((fil_no_spcbg.Length == 0) || q.Nomor_SP_Cabang == fil_no_spcbg && fil_no_spcbg.Length != 0)
                                     && ((Gdg == '0' ? q.Gudang_PL : Gdg) == q.Gudang_PL)
                               select new
                               {
                                   q.Nama_Cabang,
                                   q.Kode_Cabang,
                                   q.Nomor_SP_Cabang,
                                   q.Nomor_SP,
                                   q.Tgl_SP_Entry,

                                   q.Jenis_SP,
                                   q.Nama_Jenis_SP,
                                   q.Qty_Pesan_SP,
                                   q.Qty_Acc_SP,
                                   q.Gudang_PL,

                                   q.Nama_Gudang,
                                   q.Nomor_PL,
                                   q.Jenis_PL,
                                   q.Nama_Jenis_PL,
                                   q.Tgl_PL_Entry,


                                   q.Durasi_Buat_Pl,
                                   q.Nomor_Serah_Terima_PL,
                                   q.Waktu_Serah_Terima_PL,
                                   q.Durasi_Serah_Terima_PL,
                                   q.Nomor_Goods_Picker,


                                   q.Waktu_Goods_Picked,
                                   q.Durasi_Goods_Picked,
                                   q.Nomor_Goods_checked,
                                   q.Waktu_Goods_Checked,
                                   q.Durasi_Goods_Checked,


                                   q.Nomor_DO,
                                   q.Waktu_Buat_DO,
                                   q.Durasi_Buat_DO,
                                   q.Nomor_Pakcing_Palletizing,
                                   q.Waktu_Buat_WP,


                                   q.Durasi_Pakcing_Palletizing,
                                   q.Nomor_EP,
                                   q.Waktu_Buat_EP,
                                   q.Durasi_Buat_EP,
                                   q.Waktu_EP_Berangkat,


                                   q.Durasi_Loading,
                                   q.Nomor_RNCabang,
                                   q.Tgl_RNCabang,
                                   q.Durasi_Pengiriman,
                                   q.Qty_Diterima,


                                   q.Total_Waktu_Pemenuhan_SP,
                                   q.Outstanding_Qty,
                                   q.Leadtime,
                                   q.Deviasi_Waktu_Pemenuhan,
                                   q.Score_QtySP_Plus2,


                                   q.Score_QtySP_Min2,
                                   q.Score_By_QTY,
                                   q.Status_Pemenuhan_Qty,
                                   q.Score_by_time_plus,
                                   q.Score_by_time_min,
                                   q.Score_By_Time,
                                   q.Status_Pemenuhan_Waktu,
                                   q.SLA

                               }).Distinct().AsQueryable();






                    var fff = qry.ToList();
                    nCount = qry.Count();


                    if (nCount >= 0)
                    {
                        if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                        {
                            qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                        }

                        if ((limit == -1) || allQuery)
                        {
                            dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                        }
                        else
                        {
                            dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());

                        }
                    }




                    dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                    dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);


                    #endregion
                }

                //
                //
                else if (TypeService == "4")
                {



                    var qry = (from q in db.SL_Dets
                               where q.Tgl_SP_Entry >= Convert.ToDateTime(date1) && q.Tgl_SP_Entry <= Convert.ToDateTime(date2)
                                     && (cabang.Length == 0 ? true : cabang.Contains(q.Kode_Cabang.ToString()))
                                     && ((fil_no_spho.Length == 0) || q.Nomor_SP == fil_no_spho && fil_no_spho.Length != 0)
                                     && ((fil_no_spcbg.Length == 0) || q.Nomor_SP_Cabang == fil_no_spcbg && fil_no_spcbg.Length != 0)
                                     && ((Gdg == '0' ? q.Gudang_PL : Gdg) == q.Gudang_PL)
                               select new
                               {
                                   q.Nama_Cabang,
                                   q.Kode_Cabang,
                                   q.Kode_Barang,
                                   q.Nama_barang,
                                   q.Nomor_SP_Cabang,
                                   q.Nomor_SP,
                                   q.Tgl_SP_Entry,
                                   q.Jenis_SP,
                                   q.Nama_Jenis_SP,
                                   q.Qty_Pesan_SP,
                                   q.Qty_Acc_SP,
                                   q.Gudang_PL,
                                   q.Nama_Gudang,
                                   q.Nomor_PL,
                                   q.Jenis_PL,
                                   q.Nama_Jenis_PL,
                                   q.Tgl_PL_Entry,
                                   q.Durasi_Buat_Pl,
                                   q.Nomor_Serah_Terima_PL,
                                   q.Waktu_Serah_Terima_PL,
                                   q.Durasi_Serah_Terima_PL,
                                   q.Nomor_Goods_Picker,
                                   q.Waktu_Goods_Picked,
                                   q.Durasi_Goods_Picked,
                                   q.Nomor_Goods_checked,
                                   q.Waktu_Goods_Checked,
                                   q.Durasi_Goods_Checked,
                                   q.Nomor_DO,
                                   q.Waktu_Buat_DO,
                                   q.Durasi_Buat_DO,
                                   q.Nomor_Pakcing_Palletizing,
                                   q.Waktu_Buat_WP,
                                   q.Durasi_Pakcing_Palletizing,
                                   q.Nomor_EP,
                                   q.Waktu_Buat_EP,
                                   q.Durasi_Buat_EP,
                                   q.Waktu_EP_Berangkat,
                                   q.Durasi_Loading,
                                   q.Nomor_RNCabang,
                                   q.Tgl_RNCabang,
                                   q.Durasi_Pengiriman,
                                   q.Qty_Diterima,
                                   q.Total_Waktu_Pemenuhan_SP,
                                   q.Outstanding_Qty,
                                   q.Leadtime,
                                   q.Deviasi_Waktu_Pemenuhan,
                                   q.Score_QtySP_Plus2,
                                   q.Score_QtySP_Min2,
                                   q.Score_By_QTY,
                                   q.Status_Pemenuhan_Qty,
                                   q.Score_by_time_plus,
                                   q.Score_by_time_min,
                                   q.Score_By_Time,
                                   q.Status_Pemenuhan_Waktu,
                                   q.SLA
                               }).Distinct().AsQueryable();

                    var fff = qry.ToList();
                    nCount = qry.Count();

                    if (nCount >= 0)
                    {
                        if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                        {
                            qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                        }

                        if ((limit == -1) || allQuery)
                        {
                            dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                        }
                        else
                        {
                            dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());

                        }
                    }

                    dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                    dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);



                }

                //




            }
            break;

          #endregion

          //SERVICE LEVEL CABANG AKHIR

          #region Recall

          case Constant.REPORT_INVENTORY_RECALL:
            {
                nCount = 0;

                //Indra 20181019FM
                string fil_no_po = (parameters.ContainsKey("Nomor_SP") ? Convert.ToString(((Functionals.ParameterParser)parameters["Nomor_SP"]).Value).Trim() : string.Empty);
                //string txPeriode1 = (parameters.ContainsKey("txPeriode1") ? Convert.ToString(((Functionals.ParameterParser)parameters["txPeriode1"]).Value).Trim() : string.Empty);
                string txPeriode1 = Convert.ToString(((Functionals.ParameterParser)parameters["txPeriode1"]).Value).Trim();
                
                    
                string txPeriode2 = (parameters.ContainsKey("txPeriode2") ? Convert.ToString(((Functionals.ParameterParser)parameters["txPeriode2"]).Value).Trim() : string.Empty);
                string txPeriode3 = (parameters.ContainsKey("txPeriode3") ? Convert.ToString(((Functionals.ParameterParser)parameters["txPeriode3"]).Value).Trim() : string.Empty);
                string itemCode = (parameters.ContainsKey("itemCode") ? Convert.ToString(((Functionals.ParameterParser)parameters["itemCode"]).Value).Trim() : string.Empty);
                string itembatch = (parameters.ContainsKey("Batch") ? Convert.ToString(((Functionals.ParameterParser)parameters["Batch"]).Value).Trim() : string.Empty);
                //string TypeService = (parameters.ContainsKey("TypeService") ? Convert.ToString(((Functionals.ParameterParser)parameters["TypeService"]).Value).Trim() : string.Empty);
                //string[] cabang = (parameters.ContainsKey("noSup") ? (string[])((Functionals.ParameterParser)parameters["noSup"]).Value : new string[0]);
                //string uNip = (parameters.ContainsKey("Entry") ? (string)((Functionals.ParameterParser)parameters["Entry"]).Value : string.Empty);
                //char Gdg = (parameters.ContainsKey("Gudang") ? Convert.ToChar(((Functionals.ParameterParser)parameters["Gudang"]).Value) : '0');


                DateTime now = DateTime.Now;
                var date1 = new DateTime(now.Year, now.Month, 1);
                var date2 = date1.AddMonths(1).AddDays(-1);
                var date3 = date1.AddMonths(1).AddDays(-1);

                date1 = Convert.ToDateTime(txPeriode1);
                date2 = Convert.ToDateTime(txPeriode2);
                date3 = Convert.ToDateTime(txPeriode3);

                txPeriode1 = date1.ToString("yyyyMMdd");
                txPeriode2 = date2.ToString("yyyyMMdd");
                txPeriode3 = date3.ToString("yyyyMMdd");

                if (itemCode == "")
                {
                    Logger.WriteLine(
                                "ScmsSoaLibrary.Modules.CommonQueryProcess:ModelGridQuery <-> Switch {0} - {1}", model, "Belum ada Item");


                    dic.Add(Constant.DEFAULT_NAMING_EXCEPTION, "Produk belum dipilih, silahkan pilih produk terlebih dahulu.");

                    dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);
                }

                else
                {

                    if (itembatch == "")
                    {
                        itembatch = "00000";
                    }

                    using (SqlConnection cn = new SqlConnection(Functionals.ActiveConnectionString))
                    {
                        cn.Open();
                        SqlCommand cmd = cn.CreateCommand();
                        cmd.CommandText = "exec LG_GETDATA_RECALL '" + txPeriode1 + "', '" + txPeriode2 + "', '" + txPeriode3 + "', '" + itemCode + "', '" + itembatch + "'";
                        cmd.ExecuteNonQuery();
                        cn.Close();
                    }

                    var qry = (from q in db.LG_HISTORY_RECALLs
                               select new
                               {
                                   q.CABANG_DC,
                                   q.SOH_GOOD,
                                   q.SOH_BAD,
                                   q.DISTIRBUSI,
                                   q.RECALLN_QTY,
                                   q.SUPPLIER,
                                   q.PEMBELIAN
                               }).Distinct().AsQueryable();

                    if (nCount >= 0)
                    {
                        if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                        {
                            qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                        }

                        if ((limit == -1) || allQuery)
                        {
                            dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                        }
                        else
                        {
                            dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());

                        }
                    }

                    dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);
                    dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                    //Indra 20181019FM
                    #region old

                    //if (TypeService == "1")
                    //{

                    //    #region lama

                    //    //var qry = (from q in db.vwRecall_DO_SJ_Headers
                    //    //           where q.Tgl_do_sj >= Convert.ToDateTime(date1) && q.Tgl_do_sj <= Convert.ToDateTime(date2)
                    //    //               //&& (itemCode.Length > 0 ? q.Kode_Barang.StartsWith(itemCode) : true)
                    //    //                 && ((itemCode == "") || q.Kode_Barang == itemCode && itemCode != "")
                    //    //                 && ((itembatch == "") || q.c_batch == itembatch && itembatch != "")
                    //    //           //&& (itemCode.Length == 0 ? true : itemCode.Contains(q.Kode_Barang))
                    //    //           //&& q.Tgl_do_sj >= Convert.ToDateTime("20180101")
                    //    //           //&& (cabang.Length == 0 ? true : cabang.Contains(q.Kode_Cabang.ToString()))
                    //    //           //&& ((fil_no_po.Length == 0) || q.Nomor_SP == fil_no_po && fil_no_po.Length != 0)
                    //    //           //&& ((Gdg == '0' ? q.Gudang_PL : Gdg) == q.Gudang_PL)
                    //    //           select new
                    //    //           {
                    //    //               q.gudang_asal,
                    //    //               q.tujuan,
                    //    //               q.c_cusno,
                    //    //               q.Tgl_do_sj,
                    //    //               q.Kode_Barang,
                    //    //               q.Nama_barang,
                    //    //               q.n_qty_do_sj,
                    //    //               //q.n_sisa_do_sj,
                    //    //               q.Kode_Supplier,
                    //    //               q.Nama_Supplier,
                    //    //               q.c_batch,
                    //    //               //q.n_sisa_do_rn,

                    //    //               q.n_qty_rc_sj,
                    //    //               q.perhitungan,
                    //    //               q.kembali



                    //    //           }).Distinct().AsQueryable();






                    //    //var fff = qry.ToList();
                    //    //nCount = qry.Count();


                    //    //if (nCount >= 0)
                    //    //{
                    //    //    if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                    //    //    {
                    //    //        qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                    //    //    }

                    //    //    if ((limit == -1) || allQuery)
                    //    //    {
                    //    //        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                    //    //    }
                    //    //    else
                    //    //    {
                    //    //        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());

                    //    //    }
                    //    //}




                    //    //dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                    //    //dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                    //    #endregion

                    //    //group t by t.name into g

                    //    #region old

                    //    //var qryDO = from q in db.LG_DOHs
                    //    //            join q1 in db.LG_DOD2s on q.c_dono equals q1.c_dono
                    //    //            join q2 in db.LG_Cusmas on q.c_cusno equals q2.c_cusno
                    //    //            where q1.c_iteno == "6134" && q.c_dono == null && q1.c_batch == "C2M134C" && q.c_cusno == "0045" 
                    //    //            group new { q, q1 } by new { q2.v_cunam, q1.c_iteno, q1.c_batch } into g
                    //    //            select new
                    //    //            {
                    //    //                DOCBG = g.Key.v_cunam,
                    //    //                SOH_GOOD = 0,
                    //    //                SOH_BAD = 0,                                    
                    //    //                DON_QTY = g.Sum(x => x.q1.n_qty),
                    //    //                RCN_QTY = 0,
                    //    //                CALLN_QTY = 0,
                    //    //                RSN_QTY = 0
                    //    //            };

                    //    //var qrySJ = from q in db.LG_RCHes
                    //    //            join q1 in db.LG_RCD1s on q.c_rcno equals q1.c_rcno
                    //    //            join q2 in db.LG_Cusmas on q.c_cusno equals q2.c_cusno
                    //    //            where q1.c_type == "02" && q1.c_iteno == "6134" && q1.c_batch == "C1G096G" && q.c_cusno == "0049"
                    //    //            group new { q, q1 } by new { q2.v_cunam, q1.c_iteno, q1.c_batch } into g
                    //    //            select new
                    //    //            {
                    //    //                DOCBG = g.Key.v_cunam,
                    //    //                SOH_GOOD = 0,
                    //    //                SOH_BAD = 0,
                    //    //                DON_QTY = 0,
                    //    //                RCN_QTY = g.Sum(x => x.q1.n_qty),
                    //    //                CALLN_QTY = 0,
                    //    //                RSN_QTY = 0
                    //    //            };

                    //    //join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno into q_2
                    //    //                   from qItm in q_2.DefaultIfEmpty(f                    

                    //    #endregion                     

                    //    //if (nCount >= 0)
                    //    //{
                    //    //    if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                    //    //    {
                    //    //        qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                    //    //    }

                    //    //    if ((limit == -1) || allQuery)
                    //    //    {
                    //    //        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                    //    //    }
                    //    //    else
                    //    //    {
                    //    //        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());

                    //    //    }
                    //    //}

                    //    dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);
                    //    dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                    //}
                    //else if (TypeService == "2")
                    //{


                    //    var qry = (from q in db.temp_recalls
                    //               where q.Tgl_do_sj >= Convert.ToDateTime(date1) && q.Tgl_do_sj <= Convert.ToDateTime(date2)
                    //                    && (itemCode.Length > 0 ? q.Kode_Barang.StartsWith(itemCode) : true)
                    //                    && ((itembatch == "") || q.c_batch == itembatch && itembatch != "")

                    //               select new
                    //               {
                    //                   q.gudang_asal,
                    //                   q.tujuan,
                    //                   q.c_cusno,
                    //                   q.Nomor_do_sj,
                    //                   q.Tgl_do_sj,
                    //                   q.Kode_Barang,
                    //                   q.Nama_barang,
                    //                   q.n_qty_do_sj,
                    //                   q.v_undes,
                    //                   q.Kode_Supplier,
                    //                   q.Nama_Supplier,
                    //                   q.c_batch,
                    //                   q.Nomor_rc_sj,
                    //                   q.Tgl_rc_sj,
                    //                   q.Nomor_do_rn,
                    //                   q.n_qty_rc_sj,
                    //                   q.perhitungan,
                    //                   q.c_rcno_all,
                    //                   q.d_rcdate_all,
                    //                   q.kembali,


                    //               }).Distinct().AsQueryable();





                    //    var fff = qry.ToList();
                    //    nCount = qry.Count();


                    //    if (nCount >= 0)
                    //    {
                    //        if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                    //        {
                    //            qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                    //        }

                    //        if ((limit == -1) || allQuery)
                    //        {
                    //            dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                    //        }
                    //        else
                    //        {
                    //            dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());

                    //        }
                    //    }




                    //    dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                    //    dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                    //}

                    #endregion
                }

            }
            break;

          #endregion


          //monitoringED awal

          //#region monitoringED

          //case Constant.REPORT_INVENTORY_REPORT_MONITOTINGED:
          //  {
                            


          //          var qry = (from q in db.temp_recalls
          //                     where q.Tgl_do_sj >= Convert.ToDateTime(date1) && q.Tgl_do_sj <= Convert.ToDateTime(date2)
          //                          && (itemCode.Length > 0 ? q.Kode_Barang.StartsWith(itemCode) : true)
          //                          && ((itembatch == "") || q.c_batch == itembatch && itembatch != "")

          //                     select new
          //                     {
          //                         q.gudang_asal,
          //                         q.tujuan,
          //                         q.c_cusno,
          //                         q.Nomor_do_sj,
          //                         q.Tgl_do_sj,
          //                         q.Kode_Barang,
          //                         q.Nama_barang,
          //                         q.n_qty_do_sj,
          //                         q.v_undes,
          //                         q.Kode_Supplier,
          //                         q.Nama_Supplier,
          //                         q.c_batch,
          //                         q.Nomor_rc_sj,
          //                         q.Tgl_rc_sj,
          //                         q.Nomor_do_rn,
          //                         q.n_qty_rc_sj,
          //                         q.perhitungan,
          //                         q.c_rcno_all,
          //                         q.d_rcdate_all,
          //                         q.kembali,


          //                     }).Distinct().AsQueryable();





          //          var fff = qry.ToList();
          //          nCount = qry.Count();


          //          if (nCount >= 0)
          //          {
          //              if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
          //              {
          //                  qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
          //              }

          //              if ((limit == -1) || allQuery)
          //              {
          //                  dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
          //              }
          //              else
          //              {
          //                  dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());

          //              }
          //          }




          //          dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

          //          dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

          //      }


          //      //





          //  }
          //  break;

          //#endregion


          //monitoringED awal

          #region monitoringED

          case Constant.REPORT_INVENTORY_REPORT_MONITOTINGED:
            {
                            

                    nCount = 0;

                    char gdgPosStok = (parameters.ContainsKey("gdgStok") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgStok"]).Value) : '0');
                    char gdgPosTrx = (parameters.ContainsKey("gdgStok") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgStok"]).Value) : '0');
                    string c_iteno = (parameters.ContainsKey("c_iteno") ? Convert.ToString(((Functionals.ParameterParser)parameters["c_iteno"]).Value).Trim() : string.Empty);
                    string vitnam = (parameters.ContainsKey("v_itnam") ? Convert.ToString(((Functionals.ParameterParser)parameters["v_itnam"]).Value).Trim() : string.Empty);
                    string[] listNoSupl = (parameters.ContainsKey("c_nosup") ? (string[])((Functionals.ParameterParser)parameters["c_nosup"]).Value : new string[0]);
                    //char PilihGood = (parameters.ContainsKey("pilihgood") ? Convert.ToChar(((Functionals.ParameterParser)parameters["pilihgood"]).Value) : '0');

                    char[] avaibleGudangOkt = new char[] { '0', '1' };

                    var qry = (
                                      from qItm in db.LG_vwMonitoringEDs
                                      where (listNoSupl.Length == 0 ? true : listNoSupl.Contains(qItm.c_nosup))
                                        && (c_iteno.Length > 0 ? qItm.c_iteno.StartsWith(c_iteno) : true)
                                        && (vitnam.Length > 0 ? qItm.v_itnam.Contains(vitnam) : true)
                                        && ((gdgPosStok == '0' ? qItm.c_gdg : gdgPosStok) == qItm.c_gdg)
                                      select new CurrentStockMasterED()
                                      {
                                          c_iteno = (qItm.c_iteno == null ? string.Empty : qItm.c_iteno.Trim()),
                                          v_itnam = (qItm.v_itnam == null ? string.Empty : qItm.v_itnam.Trim()),
                                          //ItemPrice = qItm.n_salpri.ToString(),
                                          c_nosup = qItm.c_nosup.ToString(),
                                          v_nama = qItm.v_nama.ToString(),
                                          valueGood  = qItm.valueGood.ToString(),
                                          Good_1_3 = qItm.Good_1_3.ToString(),
                                          Goodvalue_1 = qItm.Goodvalue_1.ToString(),
                                          Good_4_6 = qItm.Good_4_6.ToString(),
                                          Goodvalue__4 = qItm.Goodvalue__4.ToString(),
                                          Good_7_9 = qItm.Good_7_9.ToString(),
                                          Goodvalue_7 = qItm.Goodvalue_7.ToString(),
                                          Good_10_12 = qItm.Good_10_12.ToString(),
                                          Goodvalue_10 = qItm.Goodvalue_10.ToString(),
                                          valueBad = qItm.valueBad.ToString(),
                                          Bad_1_3 = qItm.Bad_1_3.ToString(),
                                          Badvalue_1 = qItm.Badvalue_1.ToString(),
                                          Bad_4_6 = qItm.Bad_4_6.ToString(),
                                          Badvalue__4 = qItm.Badvalue__4.ToString(),
                                          Bad_7_9 = qItm.Bad_7_9.ToString(),
                                          Badvalue_7 = qItm.Badvalue_7.ToString(),
                                          Bad_10_12 = qItm.Bad_10_12.ToString(),
                                          Badvalue_10 = qItm.Badvalue_10.ToString(),
                                          EDBAD = qItm.EDBAD.ToString(), //add by suwandi 04 september 2017
                                          EDGOOD = qItm.EDGOOD.ToString()
                                      }).ToList();

                    //var yy = qry.ToList();


                    var curStokNew = (from q in qry

                                      select new TempStokLogicED()
                                      {
                                          c_iteno = q.c_iteno,
                                          v_itnam = q.v_itnam,
                                          ItemUndes = q.ItemUndes,
                                          //ItemPrice = q.ItemPrice,
                                          c_nosup = q.c_nosup,
                                          v_nama = q.v_nama,
                                          valueGood = q.valueGood,
                                          valueBad = q.valueBad,
                                          Good_1_3 = q.Good_1_3,
                                          Goodvalue1 = q.Goodvalue_1,
                                          Good_4_6 = q.Good_4_6,
                                          Goodvalue4 = q.Goodvalue__4,
                                          Good_7_9 = q.Good_7_9,
                                          Goodvalue7 = q.Goodvalue_7,
                                          Good_10_12 = q.Good_10_12,
                                          Goodvalue10 = q.Goodvalue_10,
                                          Bad_1_3 = q.Bad_1_3,
                                          Badvalue1 = q.Badvalue_1,
                                          Bad_4_6 = q.Bad_4_6,
                                          Badvalue4 = q.Badvalue__4,
                                          Bad_7_9 = q.Bad_7_9,
                                          Badvalue7 = q.Badvalue_7,
                                          Bad_10_12 = q.Bad_10_12,
                                          Badvalue10 = q.Badvalue_10,
                                          EDBAD = q.EDBAD, //add by suwandi 04 september 2017
                                          EDGOOD = q.EDGOOD
                                      }).ToList().AsQueryable();

                    var zz = curStokNew.ToList();

                    Logger.WriteLine(curStokNew.Provider.ToString());

                    nCount = qry.Count();


                    

                    if (nCount > 0)
                    {
                        if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                        {
                            curStokNew = curStokNew.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                        }

                        if ((limit == -1) || allQuery)
                        {
                            dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                        }
                        else
                        {
                            dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                        }
                    }

                    if (qry != null)
                       {
                         qry.Clear();
                       }

                    dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                    dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);




               

                
            }
            break;

          #endregion


          //monitoringED akhir


         //Monitoring_Expired_2 awal

          #region Monitoring Expired 2

          case Constant.REPORT_INVENTORY_MONITORINGEDCTRL:
            {


                nCount = 0;

                char gdgPosStok = (parameters.ContainsKey("gdgStok") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgStok"]).Value) : '0');
                char gdgPosTrx = (parameters.ContainsKey("gdgTrx") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgTrx"]).Value) : '0');
                string c_iteno = (parameters.ContainsKey("c_iteno") ? Convert.ToString(((Functionals.ParameterParser)parameters["c_iteno"]).Value).Trim() : string.Empty);
                string vitnam = (parameters.ContainsKey("v_itnam") ? Convert.ToString(((Functionals.ParameterParser)parameters["v_itnam"]).Value).Trim() : string.Empty);
                string[] listNoSupl = (parameters.ContainsKey("c_nosup") ? (string[])((Functionals.ParameterParser)parameters["c_nosup"]).Value : new string[0]);
                //char PilihGood = (parameters.ContainsKey("pilihgood") ? Convert.ToChar(((Functionals.ParameterParser)parameters["pilihgood"]).Value) : '0');



                char[] avaibleGudangOkt = new char[] { '0', '1' };
                
                var qry = (

                                      from qItm in db.vwMonitoringExpired_Alls

                                      where (listNoSupl.Length == 0 ? true : listNoSupl.Contains(qItm.c_nosup))
                                        && (c_iteno.Length > 0 ? qItm.c_iteno.StartsWith(c_iteno) : true)
                                        && (vitnam.Length > 0 ? qItm.v_itnam.Contains(vitnam) : true)
                                        && ((gdgPosStok == '0' ? qItm.c_gdg : gdgPosStok) == qItm.c_gdg)
                                        
                                      select new CurrentStockMasterExpired()
                                      {
                                          c_iteno = (qItm.c_iteno == null ? string.Empty : qItm.c_iteno.Trim()),
                                          v_itnam = (qItm.v_itnam == null ? string.Empty : qItm.v_itnam.Trim()),
                                          ItemPrice = qItm.n_salpri.ToString(),
                                          c_nosup = qItm.c_nosup.ToString(),
                                          v_nama = qItm.v_nama.ToString(),
                                          c_batch = qItm.c_batch.ToString(),
                                          d_expired = qItm.d_expired.ToString(),
                                          n_salpri = qItm.n_salpri.ToString(),
                                          n_gsisa = qItm.n_gsisa.ToString(),
                                          valueGood = qItm.valueGood.ToString(),
                                          n_bsisa = qItm.n_bsisa.ToString(),
                                          valueBad = qItm.valueBad.ToString(),
                                      }).ToList();
                    //var yy = qry.ToList();

                    var curStokNew = (from q in qry

                                      select new TempStokLogicExpired()
                                      {
                                          c_iteno = q.c_iteno,
                                          v_itnam = q.v_itnam,
                                          //ItemUndes = q.ItemUndes,
                                          ItemPrice = q.ItemPrice,
                                          c_nosup = q.c_nosup,
                                          v_nama = q.v_nama,
                                          c_batch = q.c_batch,
                                          d_expired = q.d_expired,
                                          n_salpri = q.n_salpri,
                                          n_gsisa = q.n_gsisa,
                                          valueGood = q.valueGood,
                                          n_bsisa = q.n_bsisa,
                                          valueBad = q.valueBad,
                                      }).ToList().AsQueryable();

                    //var zz = curStokNew.ToList();

                    Logger.WriteLine(curStokNew.Provider.ToString());

                    nCount = curStokNew.Count();

                    qry.Clear();


                    if (nCount > 0)
                    {
                        if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                        {
                            curStokNew = curStokNew.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                        }

                        if ((limit == -1) || allQuery)
                        {
                            dic.Add(Constant.DEFAULT_NAMING_RECORDS, curStokNew.ToList());
                        }
                        else
                        {
                            dic.Add(Constant.DEFAULT_NAMING_RECORDS, curStokNew.Skip(start).Take(limit).ToList());
                        }
                    }

                    if (qry != null)
                    {
                        qry.Clear();
                    }


                    dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                    dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
            }
            break;



          #endregion

         //Monitoring_Expired_2 akhir
        }
      }
  catch (Exception ex)
      {
        Logger.WriteLine(
          "ScmsSoaLibrary.Modules.CommonQueryProcess:ModelGridQuery <-> Switch {0} - {1}", model, ex.Message);
        Logger.WriteLine(ex.StackTrace);

        dic.Add(Constant.DEFAULT_NAMING_EXCEPTION, ex.Message);

        dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);
      }

      lstSPExcl.Clear();

      db.Dispose();

      return dic;
    }

    public static IDictionary<string, object> ModelGridQueryStoreProcedure(string connectionString, int start, int limit, bool allQuery, string sort, string dir, string model, IDictionary<string, Functionals.ParameterParser> parameters)
    {
      IDictionary<string, object> dic = new Dictionary<string, object>();

        Config cfg = Functionals.Configuration;
        Encoding utf8 = Encoding.UTF8;
        ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);
        //db.CommandTimeout = 1000;

      DateTime date = DateTime.Now,
        dateStart = DateTime.MinValue,
        dateEnd = DateTime.MinValue;

      string tmp = null;
      int iTmp = 0,
        iTmp1 = 0;
      int nResult = 0;
      List<ProgressCallingSPResult<bool>> listPCSPR = new List<ProgressCallingSPResult<bool>>();

      try
      {
        switch (model)
        {
          #region MODEL_PROCESS_QUERY_SP_BDPRDP

          case Constant.MODEL_PROCESS_QUERY_SP_BDPRDP:
            {
              db.CommandTimeout = 0;

              iTmp = (parameters.ContainsKey("Tahun") ? (int)((Functionals.ParameterParser)parameters["Tahun"]).Value : 0);
              if ((iTmp < 1900) || (iTmp > 2999))
              {
                dic.Add(Constant.DEFAULT_NAMING_EXCEPTION, "Format penulisan tahun salah.");

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);

                break;
              }

              iTmp1 = (parameters.ContainsKey("Bulan") ? (int)((Functionals.ParameterParser)parameters["Bulan"]).Value : 0);
              if ((iTmp1 < 1) || (iTmp1 > 12))
              {
                dic.Add(Constant.DEFAULT_NAMING_EXCEPTION, "Format penulisan bulan salah.");

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);

                break;
              }

              dateStart = new DateTime(iTmp, iTmp1, 1);
              dateEnd = dateStart.AddMonths(1).AddDays(-1);

              bool isBdp = (parameters.ContainsKey("bdp") ? (bool)((Functionals.ParameterParser)parameters["bdp"]).Value : false);
              
              string[] arrTmp = (parameters.ContainsKey("tipe") && ((Functionals.ParameterParser)parameters["tipe"]).IsIn  ? (string[])((Functionals.ParameterParser)parameters["tipe"]).Value : new string[0]);
              if ((arrTmp == null) || (arrTmp.Length < 1))
              {
                dic.Add(Constant.DEFAULT_NAMING_EXCEPTION, "Tipe tidak terbaca.");

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);

                break;
              }

              for (int nLoop = 0; nLoop < arrTmp.Length; nLoop++)
              {
                tmp = arrTmp[nLoop];

                nResult = db.SnapshotBDPRDP(dateStart, dateEnd, tmp, dateStart, isBdp);

                listPCSPR.Add(new ProgressCallingSPResult<bool>()
                {
                  ID = tmp,
                  Result = (nResult > 0)
                });
              }

              dic.Add(Constant.DEFAULT_NAMING_RECORDS, listPCSPR.ToArray());

              dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, listPCSPR.Count);

              dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

              listPCSPR.Clear();
            }
            break;

          #endregion

          #region MODEL_PROCESS_QUERY_SP_CLOSINGPO

          case Constant.MODEL_PROCESS_QUERY_SP_CLOSINGPO:
            {
              db.CommandTimeout = 0;

              iTmp = (parameters.ContainsKey("Tahun") ? (int)((Functionals.ParameterParser)parameters["Tahun"]).Value : 0);
              iTmp1 = (parameters.ContainsKey("Bulan") ? (int)((Functionals.ParameterParser)parameters["Bulan"]).Value : 0);
              tmp = (parameters.ContainsKey("User") ? (string)((Functionals.ParameterParser)parameters["User"]).Value : string.Empty);
              string itemSelected = (parameters.ContainsKey("Item") ? (string)((Functionals.ParameterParser)parameters["Item"]).Value : string.Empty);

              if (string.IsNullOrEmpty(tmp))
              {
                throw new Exception("Nama pemakai tidak terbaca");
              }
              else if ((iTmp < 1900) || (iTmp > 9999))
              {
                throw new Exception("Tahun tidak terbaca/tidak valid");
              }
              else if ((iTmp1 < 1) || (iTmp1 > 12))
              {
                throw new Exception("Bulan tidak terbaca/tidak valid");
              }

              nResult = db.ClosingPOSnapshot((short?)iTmp, (byte?)iTmp1, tmp, itemSelected);

              listPCSPR.Add(new ProgressCallingSPResult<bool>()
              {
                ID = tmp,
                Result = (nResult > 0)
              });

              dic.Add(Constant.DEFAULT_NAMING_RECORDS, listPCSPR.ToArray());

              dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, listPCSPR.Count);

              dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

              listPCSPR.Clear();
            }
            break;

          #endregion


                    /////

                    #region tes
                    #region MODEL_COMMON_QUERY_AVG_SALES

                    case Constant.MODEL_COMMON_QUERY_AVG_SALES:
                        {
                            ScmsSoaLibrary.Modules.CommonQuerySP spRun = null;
                            string Tahun = null, Bulan = null;
                            //IDictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> param = null;

                            List<LG_CusmasCab> cabDcore = new  List<LG_CusmasCab>();
                            List<Temp_Avg> ListAvgSales = null;

                            cabDcore = (from q in db.LG_CusmasCabs
                                        where q.c_cab_dcore != null //&& q.c_cab_dcore =="JK1"
                                        select q).ToList();

                            decimal idx = 0;
                            for (int nLoopC = 0, nLenC = cabDcore.Count; nLoopC < nLenC; nLoopC++)
                            {

                                Dictionary<string, string> param = new Dictionary<string, string>();

                                //IDictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> param = null;

                                //Uri uri = Functionals.DistCoreUrlBuilder(cfg, " http://10.100.10.52/dcore/?m=com.ams.json.ds&action=form&f=Business&open=mst_avg_qty&C_KODECAB=JK1");
                                Uri uri = Functionals.DistCoreUrlBuilder(cfg, " http://10.100.10.28/dcore/?m=com.ams.json.ds&action=form&f=Business&open=mst_avg_qty");

                                Bulan = (parameters.ContainsKey("BULAN") ? (string)((Functionals.ParameterParser)parameters["BULAN"]).Value : string.Empty);
                                Tahun = (parameters.ContainsKey("TAHUN") ? (string)((Functionals.ParameterParser)parameters["TAHUN"]).Value : string.Empty);

                                string cusmas = cabDcore[nLoopC].c_cab_dcore;

                                param.Add("C_KODECAB", cusmas);

                               
                                Dictionary<string, string> header = new Dictionary<string, string>();
                                header.Add("X-Requested-With", "XMLHttpRequest");

                                bool getSuccess = false;

                                string result = null;
                                Dictionary<string, object> dicHeader = null;
                                List<Dictionary<string, string>> list = null;
                                Dictionary<string, string> dataRow = null;

                                ScmsSoaLibrary.Parser.ParserDisCore pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

                                pdc.Referer = Functionals.DistCoreUrlBuilderString(cfg, "http://10.100.10.40/dcore/?m=com.ams.trx.pbbpbr");
                                pdc.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                                if (pdc.PostGetData(uri, param, header))
                                {
                                    result = utf8.GetString(pdc.Result);

                                    dicHeader = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCore(result);

                                    list = dicHeader[Constant.DEFAULT_NAMING_RECORDS] as List<Dictionary<string, string>>;

                                    if (list == null)
                                    {
                                        continue;
                                    }

                                    ListAvgSales = new List<Temp_Avg>();

                                    Dictionary<string, object> dicResult = null;
                                    dicResult = JSON.Deserialize<Dictionary<string, object>>(result);

                                    foreach (KeyValuePair<string, object> kvp in dicResult)
                                    {
                                        if (kvp.Key.ToUpper() == "DATAROW")
                                        {
                                            object s = kvp.Value;
                                            Dictionary<string, string>[] dicEXP1 = JSON.Deserialize<Dictionary<string, string>[]>(s.ToString());

                                            foreach (Dictionary<string, string> dics in dicEXP1)
                                            {
                                                var Temp_Avg = new Temp_Avg();
                                                idx = idx + 1;

                                                foreach (KeyValuePair<string, string> kvps in dics)
                                                {
                                                    if (kvps.Key.Equals("C_ITENO"))
                                                    {
                                                        Temp_Avg.c_iteno = (string)kvps.Value;
                                                    }
                                                    if (kvps.Key.Equals("TOTAL_QTY"))
                                                    {
                                                        Temp_Avg.Total_qty = decimal.Parse(kvps.Value);
                                                    }
                                                    if (kvps.Key.Equals("AVG_QTY"))
                                                    {
                                                        Temp_Avg.avg_qty = decimal.Parse(kvps.Value);
                                                    }
                                                    if (kvps.Key.Equals("AVG_MONTH"))
                                                    {
                                                        Temp_Avg.avg_month = decimal.Parse(kvps.Value);
                                                    }
                                                    if (kvps.Key.Equals("YEARMONTH"))
                                                    {
                                                        Temp_Avg.yearmonth = (string)kvps.Value;
                                                    }
                                                    if (kvps.Key.Equals("C_CLASS"))
                                                    {
                                                        Temp_Avg.c_class = (string)kvps.Value;
                                                    }

                                                    Temp_Avg.c_cab = cusmas;
                                                    Temp_Avg.idx = idx;
                                                   
                                                }

                                                ListAvgSales.Add(Temp_Avg);

                                            }

                                        }

                                        db.Temp_Avgs.InsertAllOnSubmit(ListAvgSales.ToArray());
                                    }

                                    ///ss
                                    //var up = (from q in db.LG_AvgSales_tes
                                    //          join q1 in db.Temp_Avgs on q.c_iteno equals q1.c_iteno
                                    //          where (q1.yearmonth.Substring(0,4) == Bulan) &&  (q1.yearmonth.Substring(4,0) ==  
                                     // )

                                }

                                else
                                {
                                    result = pdc.ErrorMessage;

                                    Logger.WriteLine(result);
                                }

                            }

                            if ((nResult > 0))
                            {
                                //ScmsSoaLibrary.Bussiness.Commons.RunningGenerateRS(db, nipEntry, true,
                                //  lstResultNew.ToArray());

                                //CommonQuerySP cqSP = new CommonQuerySP();
                                //cqSP.SP_LG_CalcAvgSales(db.Connection.ConnectionString, cabDcore, ListAvgSales.ToArray());

                            }

                            if (nResult > 0)
                            {
                                //dic.Add(Constant.DEFAULT_NAMING_RECORDS, lstStock.ToArray());

                                dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nResult);

                                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                                ListAvgSales.Clear();
                            }

                            db.SubmitChanges();

                            List<SqlParameter> lstParams = new List<SqlParameter>();

                            lstParams.Add(new SqlParameter(string.Concat("@", "TAHUN1"),
                                            System.Data.SqlDbType.VarChar)
                            {
                                Size = 15,
                                Value = Tahun
                            });

                            lstParams.Add(new SqlParameter(string.Concat("@", "BULAN1"),
                                            System.Data.SqlDbType.VarChar)
                            {
                                Size = 15,
                                Value = Bulan  
                            });

                            string tmpQuery = "Exec LG_CalcAvgSales_New @TAHUN1,@BULAN1";

                            Functionals.ExecProcedures(cfg, tmpQuery, lstParams.ToArray());

                        }
                        break;

                    #endregion
                    #endregion

                    #region MODEL_COMMON_QUERY_FORECAST

                    case Constant.MODEL_COMMON_QUERY_FORECAST:
                        {
                            ScmsSoaLibrary.Modules.CommonQuerySP spRun = null;
                            string Tahun = null, Bulan = null;
                            string cek = null, cek2 = null;
                            //IDictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> param = null;

                            List<LG_CusmasCab> cabDcore = new List<LG_CusmasCab>();
                            List<tmp_forecast> listForeCast = null;

                            cabDcore = (from q in db.LG_CusmasCabs
                                        where q.c_cab_dcore != null //&& q.c_cab_dcore =="JK1"
                                        select q).ToList();

                            //listForeCast = (from q in db.tmp_forecasts where q.c_periode = );
                            

                            decimal idx = 0;
                            for (int nLoopC = 0, nLenC = cabDcore.Count; nLoopC < nLenC; nLoopC++)
                            {
                                
                                Dictionary<string, string> param = new Dictionary<string, string>();

                                //IDictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> param = null;

                                //Uri uri = Functionals.DistCoreUrlBuilder(cfg, " http://10.100.10.52/dcore/?m=com.ams.json.ds&action=form&f=Business&open=mst_avg_qty&C_KODECAB=JK1");
                                Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.28/dcore/?m=com.ams.json.ds&action=form&f=Business&open=forecast_accuration");
                                //&C_KODECAB=JK1&C_MONTH=7&C_YEAR=2017
                                Bulan = (parameters.ContainsKey("BULAN") ? (string)((Functionals.ParameterParser)parameters["BULAN"]).Value : string.Empty);
                                Tahun = (parameters.ContainsKey("TAHUN") ? (string)((Functionals.ParameterParser)parameters["TAHUN"]).Value : string.Empty);

                                string cusmas = cabDcore[nLoopC].c_cab_dcore;
                                if (date.Day <= 15)
                                {
                                    cek = (from q in db.tmp_forecasts
                                           where q.c_tahun == Tahun && q.c_bulan == Bulan && q.c_cab == cusmas && q.c_periode == "1"
                                           select q.c_tahun).Max();
                                }
                                else
                                {
                                    cek = (from q in db.tmp_forecasts
                                            where q.c_tahun == Tahun && q.c_bulan == Bulan && q.c_cab == cusmas && q.c_periode == "2"
                                            select q.c_tahun).Max();
                                }

                                param.Add("C_KODECAB", cusmas);
                                param.Add("C_MONTH", Bulan);
                                param.Add("C_YEAR",Tahun);

                                Dictionary<string, string> header = new Dictionary<string, string>();
                                header.Add("X-Requested-With", "XMLHttpRequest");

                                bool getSuccess = false;

                                string result = null;
                                Dictionary<string, object> dicHeader = null;
                                List<Dictionary<string, string>> list = null;
                                Dictionary<string, string> dataRow = null;

                                ScmsSoaLibrary.Parser.ParserDisCore pdc = new ScmsSoaLibrary.Parser.ParserDisCore();
                                if (cek == null)
                                {
                                    pdc.Referer = Functionals.DistCoreUrlBuilderString(cfg, "http://10.100.10.40/dcore/?m=com.ams.trx.pbbpbr");
                                    pdc.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                                    if (pdc.PostGetData(uri, param, header))
                                    {
                                        result = utf8.GetString(pdc.Result);

                                        dicHeader = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCore(result);

                                        list = dicHeader[Constant.DEFAULT_NAMING_RECORDS] as List<Dictionary<string, string>>;

                                        if (list == null)
                                        {
                                            continue;
                                        }

                                        listForeCast = new List<tmp_forecast>();

                                        Dictionary<string, object> dicResult = null;
                                        dicResult = JSON.Deserialize<Dictionary<string, object>>(result);

                                        foreach (KeyValuePair<string, object> kvp in dicResult)
                                        {
                                            if (kvp.Key.ToUpper() == "DATAROW")
                                            {
                                                object s = kvp.Value;
                                                Dictionary<string, string>[] dicEXP1 = JSON.Deserialize<Dictionary<string, string>[]>(s.ToString());

                                                foreach (Dictionary<string, string> dics in dicEXP1)
                                                {
                                                    var Temp_forecast = new tmp_forecast();
                                                    idx = idx + 1;

                                                    foreach (KeyValuePair<string, string> kvps in dics)
                                                    {
                                                        if (kvps.Key.Equals("C_ITENO"))
                                                        {
                                                            Temp_forecast.c_iteno = (string)kvps.Value;
                                                            //tmp_forecast.c_iteno = (string)kvps.Value;
                                                        }
                                                        if (kvps.Key.Equals("C_PERIODE"))
                                                        {
                                                            Temp_forecast.c_periode = (string)kvps.Value;
                                                        }
                                                        if (kvps.Key.Equals("AVG_QTY"))
                                                        {
                                                            if (kvps.Value == null)
                                                            {
                                                                Temp_forecast.avg_qty = 0;
                                                            }
                                                            else
                                                            { Temp_forecast.avg_qty = decimal.Parse(kvps.Value); }
                                                        }
                                                        if (kvps.Key.Equals("C_ITNAM"))
                                                        {
                                                            Temp_forecast.c_itnam = (string)kvps.Value;
                                                        }
                                                        if (kvps.Key.Equals("YEARMONTH"))
                                                        {
                                                            Temp_forecast.YearMonth = (string)kvps.Value;
                                                        }
                                                        if (kvps.Key.Equals("N_FORECAST"))
                                                        {
                                                            if (kvps.Value == null)
                                                            {
                                                                Temp_forecast.n_forecast = 0;
                                                            }
                                                            else
                                                            {
                                                                Temp_forecast.n_forecast = decimal.Parse(kvps.Value);
                                                            }
                                                        }
                                                        if (kvps.Key.Equals("SLS_TO_DT"))
                                                        {
                                                            if (kvps.Value == null)
                                                            {
                                                                Temp_forecast.sls_to_dt = 0;
                                                            }
                                                            else
                                                            {
                                                                Temp_forecast.sls_to_dt = decimal.Parse(kvps.Value);
                                                            }
                                                        }
                                                        if (kvps.Key.Equals("SLS_TO_GO"))
                                                        {
                                                            if (kvps.Value == null)
                                                            {
                                                                Temp_forecast.sls_to_go = 0;
                                                            }
                                                            else
                                                            {
                                                                Temp_forecast.sls_to_go = decimal.Parse(kvps.Value);
                                                            }
                                                        }
                                                        if (kvps.Key.Equals("SLS"))
                                                        {
                                                            if (kvps.Value == null)
                                                            {
                                                                Temp_forecast.sls = 0;
                                                            }
                                                            else
                                                            {
                                                                Temp_forecast.sls = decimal.Parse(kvps.Value);
                                                            }
                                                        }
                                                        if (kvps.Key.Equals("HIT"))
                                                        {
                                                            if (kvps.Value == null)
                                                            {
                                                                Temp_forecast.hit = 0;
                                                            }
                                                            else
                                                            {
                                                                Temp_forecast.hit = decimal.Parse(kvps.Value);
                                                            }
                                                        }
                                                        if (kvps.Key.Equals("VAR"))
                                                        {
                                                            if (kvps.Value == null)
                                                            {
                                                                Temp_forecast.variant = 0;
                                                            }
                                                            else
                                                            {
                                                                Temp_forecast.variant = decimal.Parse(kvps.Value);
                                                            }
                                                        }

                                                        Temp_forecast.c_cab = cusmas;
                                                        Temp_forecast.c_tahun = Tahun;
                                                        Temp_forecast.c_bulan = Bulan;

                                                    }

                                                    listForeCast.Add(Temp_forecast);

                                                }

                                            }

                                            db.tmp_forecasts.InsertAllOnSubmit(listForeCast.ToArray());
                                        }

                                        ///ss
                                        //var up = (from q in db.LG_AvgSales_tes
                                        //          join q1 in db.Temp_Avgs on q.c_iteno equals q1.c_iteno
                                        //          where (q1.yearmonth.Substring(0,4) == Bulan) &&  (q1.yearmonth.Substring(4,0) ==  
                                        // )

                                    }

                                    else
                                    {
                                        result = pdc.ErrorMessage;

                                        Logger.WriteLine(result);
                                    }
                                }
                            }

                            if ((nResult > 0))
                            {
                                //ScmsSoaLibrary.Bussiness.Commons.RunningGenerateRS(db, nipEntry, true,
                                //  lstResultNew.ToArray());

                                //CommonQuerySP cqSP = new CommonQuerySP();
                                //cqSP.SP_LG_CalcAvgSales(db.Connection.ConnectionString, cabDcore, ListAvgSales.ToArray());

                            }

                            if (nResult > 0)
                            {
                                //dic.Add(Constant.DEFAULT_NAMING_RECORDS, lstStock.ToArray());

                                dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nResult);

                                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                                listForeCast.Clear();
                            }

                            db.SubmitChanges();

                            List<SqlParameter> lstParams = new List<SqlParameter>();

                            lstParams.Add(new SqlParameter(string.Concat("@", "TAHUN1"),
                                            System.Data.SqlDbType.VarChar)
                            {
                                Size = 15,
                                Value = Tahun
                            });

                            lstParams.Add(new SqlParameter(string.Concat("@", "BULAN1"),
                                            System.Data.SqlDbType.VarChar)
                            {
                                Size = 15,
                                Value = Bulan
                            });

                            //string tmpQuery = "Exec LG_CalcAvgSales_New @TAHUN1,@BULAN1";

                            //Functionals.ExecProcedures(cfg, tmpQuery, lstParams.ToArray());

                        }
                        break;
                    #endregion
                    /////


                    #region MODEL_PROCESS_QUERY_SP_CLOSSINGLOG

          case Constant.MODEL_PROCESS_QUERY_SP_CLOSSINGLOG:
            {
              int Tahun = 0, Bulan = 0;
              Tahun = (parameters.ContainsKey("Tahun") ? int.Parse(((Functionals.ParameterParser)parameters["Tahun"]).Value.ToString()) : 0);
              Bulan = (parameters.ContainsKey("Bulan") ? int.Parse(((Functionals.ParameterParser)parameters["Bulan"]).Value.ToString()) : 0);
              string Nip = (parameters.ContainsKey("Entry").ToString() == null ? string.Empty : ((Functionals.ParameterParser)parameters["Entry"]).Value.ToString() );
              int ExtYear = (Bulan == 1 ? Tahun - 1 : Tahun);
              int ExtMonth = (Bulan == 1 ? 12 : Bulan - 1);
              string[] a = new string[] { "01", "02" };
              string[] tipeRNBeli = new string[] { "01", "03" };
              string tipRNRS = "02";
              string tipRNRetur = "04";

              List<LG_STOCKTMP> Stock = new List<LG_STOCKTMP>();
              List<LG_STOCKTMP> StockFinal = new List<LG_STOCKTMP>();
              List<LG_STOCKTMP> tmpStock = null;
              List<LG_STOCKTMP> tmpStock1 = new List<LG_STOCKTMP>();
              List<LG_STOCKTMP> tmpStock2 = new List<LG_STOCKTMP>();
              List<LG_STOCKTMP> tmpStock3 = new List<LG_STOCKTMP>();
              List<LG_STOCKTMP> tmpStock4 = new List<LG_STOCKTMP>();
              List<LG_Stock> lstStockST = new List<LG_Stock>();

              #region Closing Stock

              tmpStock = (from q in db.LG_Stocks
                          where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                          select new LG_STOCKTMP()
                          {
                            c_gdg = q.c_gdg,
                            c_rnno = q.c_no,
                            c_iteno = q.c_iteno,
                            c_batch = q.c_batch,
                            n_gqty = (q.n_gqty.Value),
                            n_bqty = (q.n_bqty.Value)
                          }).ToList();

              //lstStockST = db.LG_Stocks.Where(x => x.s_tahun == ExtYear && x.t_bulan == ExtMonth).ToList();

              tmpStock1.AddRange(tmpStock);
              tmpStock.Clear();

              tmpStock = (from q in db.LG_RNHs
                          join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                          where (q.d_rndate.Value.Year == Tahun) && (q.d_rndate.Value.Month == Bulan)
                          && tipeRNBeli.Contains(q.c_type)
                          group new { q, q1 } by new { q.c_gdg, q.c_rnno, q1.c_iteno, q1.c_batch } into sumg
                          select new LG_STOCKTMP()
                          {
                            c_gdg = sumg.Key.c_gdg,
                            c_rnno = sumg.Key.c_rnno,
                            c_iteno = sumg.Key.c_iteno,
                            c_batch = sumg.Key.c_batch,
                            n_gqty = (sumg.Sum(x => x.q1.n_gqty.HasValue ? x.q1.n_gqty.Value : 0m)),
                            n_bqty = (sumg.Sum(x => x.q1.n_bqty.HasValue ? x.q1.n_bqty.Value : 0m))
                          }).ToList();

              tmpStock1.AddRange(tmpStock);
              tmpStock.Clear();

              tmpStock = (from q in db.LG_RNHs
                          join q1 in db.LG_RND3s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                          where (q.d_rndate.Value.Year == Tahun) && (q.d_rndate.Value.Month == Bulan)
                          && q.c_type == tipRNRS
                          group new { q, q1 } by new { q.c_gdg, q1.c_rn, q1.c_iteno, q1.c_batch } into sumg
                          select new LG_STOCKTMP()
                          {
                            c_gdg = sumg.Key.c_gdg,
                            c_rnno = sumg.Key.c_rn,
                            c_iteno = sumg.Key.c_iteno,
                            c_batch = sumg.Key.c_batch,
                            n_gqty = (sumg.Sum(x => x.q1.n_gqty.HasValue ? x.q1.n_gqty.Value : 0m)),
                            n_bqty = (sumg.Sum(x => x.q1.n_bqty.HasValue ? x.q1.n_bqty.Value : 0m))
                          }).ToList();

              tmpStock1.AddRange(tmpStock);
              tmpStock.Clear();

              tmpStock = (from q in db.LG_RNHs
                          join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                          where (q.d_rndate.Value.Year == Tahun) && (q.d_rndate.Value.Month == Bulan)
                          && q.c_type == tipRNRetur
                          group new { q, q1 } by new { q.c_gdg, q.c_rnno, q1.c_iteno, q1.c_batch } into sumg
                          select new LG_STOCKTMP()
                          {
                            c_gdg = sumg.Key.c_gdg,
                            c_rnno = sumg.Key.c_rnno,
                            c_iteno = sumg.Key.c_iteno,
                            c_batch = sumg.Key.c_batch,
                            n_gqty = (sumg.Sum(x => x.q1.n_gqty.HasValue ? x.q1.n_gqty.Value : 0m)),
                            n_bqty = (sumg.Sum(x => x.q1.n_bqty.HasValue ? x.q1.n_bqty.Value : 0m))
                          }).ToList();

              tmpStock1.AddRange(tmpStock);
              tmpStock.Clear();

              // RN Gudang

              tmpStock = (from q in db.LG_SJHs
                          join q1 in db.LG_SJD2s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                          where (q.d_update.Value.Year == Tahun) && (q.d_update.Value.Month == Bulan)
                          && q.l_status == true
                          && q1.c_batch.Trim() != ""
                          group new { q, q1 } by new { q.c_gdg2, q1.c_rnno, q1.c_iteno, c_batch = q1.c_batch.Trim().ToUpper() } into sumg
                          select new LG_STOCKTMP()
                          {
                            c_gdg = (sumg.Key.c_gdg2.HasValue ? sumg.Key.c_gdg2.Value : '1'),
                            c_rnno = sumg.Key.c_rnno,
                            c_iteno = sumg.Key.c_iteno,
                            c_batch = sumg.Key.c_batch.Trim().ToUpper(),
                            n_gqty = (sumg.Sum(x => x.q1.n_gqty.HasValue ? x.q1.n_gqty.Value : 0m)),
                            n_bqty = (sumg.Sum(x => x.q1.n_bqty.HasValue ? x.q1.n_bqty.Value : 0m))
                          }).ToList();



              tmpStock1.AddRange(tmpStock);
              tmpStock.Clear();

              // Combo
              tmpStock = (from q in db.LG_ComboHs
                          where (q.d_combodate.Value.Year == Tahun) && (q.d_combodate.Value.Month == Bulan)
                          && q.c_type == "01"
                          select new LG_STOCKTMP()
                          {
                            c_gdg = q.c_gdg,
                            c_rnno = q.c_combono,
                            c_iteno = q.c_iteno,
                            c_batch = q.c_batch,
                            n_gqty = (q.n_gqty.Value),
                            n_bqty = (q.n_bqty.Value)
                          }).ToList();

              tmpStock1.AddRange(tmpStock);
              tmpStock.Clear();

              //insert stock Input
              Stock = (from q in tmpStock1
                       group q by new { q.c_gdg, q.c_rnno, q.c_iteno, c_batch = q.c_batch.TrimEnd().ToLower() } into g
                       select new LG_STOCKTMP()
                       {
                         c_gdg = g.Key.c_gdg,
                         c_rnno = g.Key.c_rnno,
                         c_iteno = g.Key.c_iteno,
                         c_batch = g.Key.c_batch,
                         n_gqty = g.Sum(x => x.n_gqty),
                         n_bqty = g.Sum(x => x.n_bqty)
                       }).ToList();

              var ssTT = Stock.Sum(x=>x.n_gqty);

              tmpStock1.Clear();

              // Input RC

              tmpStock2 = (from q2 in db.LG_RCHes
                           join q3 in db.LG_RCD1s on new { q2.c_gdg, q2.c_rcno } equals new { q3.c_gdg, q3.c_rcno }
                           where (q2.d_rcdate.Value.Year == Tahun) && (q2.d_rcdate.Value.Month == Bulan)
                           group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                           select new LG_STOCKTMP()
                           {
                             c_gdg = sumg.Key.c_gdg,
                             c_rnno = sumg.Key.c_rnno,
                             c_iteno = sumg.Key.c_iteno,
                             c_batch = sumg.Key.c_batch,
                             n_gqty = sumg.Sum(x=>x.q3.c_type == "01" ? x.q3.n_qty.Value : 0),
                             n_bqty = sumg.Sum(x => x.q3.c_type == "02" ? x.q3.n_qty.Value : 0)
                           }).ToList();

              tmpStock3 = (from q in Stock
                           select new LG_STOCKTMP()
                          {
                            c_gdg = q.c_gdg,
                            c_rnno = q.c_rnno,
                            c_iteno = q.c_iteno,
                            c_batch = q.c_batch,
                            n_gqty = q.n_gqty,
                            n_bqty = q.n_bqty
                          }).ToList();

              tmpStock4 = (from q in tmpStock2
                           join q1 in tmpStock3 on new 
                           { 
                             q.c_gdg, 
                             q.c_rnno, 
                             q.c_iteno, 
                             c_batch = q.c_batch.Trim().ToUpper() 
                           } equals new 
                           { 
                             q1.c_gdg, 
                             q1.c_rnno, 
                             q1.c_iteno, 
                             c_batch = q1.c_batch.Trim().ToUpper() 
                           } into g
                           from gLeft in g.DefaultIfEmpty()
                           where gLeft == null
                           select new LG_STOCKTMP()
                           {
                             c_gdg = q.c_gdg,
                             c_rnno = q.c_rnno,
                             c_iteno = q.c_iteno,
                             c_batch = q.c_batch,
                             n_gqty = q.n_gqty,
                             n_bqty = q.n_bqty
                           }).OrderBy(x=>x.c_iteno).ToList();
              
              Stock.AddRange(tmpStock4);
              tmpStock2.Clear();
              tmpStock3.Clear();
              tmpStock4.Clear();

              // Input RS

              tmpStock2 = (from q2 in db.LG_RSHes
                             join q3 in db.LG_RSD2s on new { q2.c_gdg, q2.c_rsno } equals new { q3.c_gdg, q3.c_rsno }
                             where (q2.d_rsdate.Value.Year == Tahun) && (q2.d_rsdate.Value.Month == Bulan)
                             group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                             select new LG_STOCKTMP()
                             {
                               c_gdg = sumg.Key.c_gdg,
                               c_rnno = sumg.Key.c_rnno,
                               c_iteno = sumg.Key.c_iteno,
                               c_batch = sumg.Key.c_batch,
                               n_gqty = 0m,
                               n_bqty = 0m
                             }).ToList();

              tmpStock3 = (from q in Stock
                           select new LG_STOCKTMP()
                          {
                            c_gdg = q.c_gdg,
                            c_rnno = q.c_rnno,
                            c_iteno = q.c_iteno,
                            c_batch = q.c_batch,
                            n_gqty = q.n_gqty,
                            n_bqty = q.n_bqty
                          }).ToList();

              tmpStock4 = (from q in tmpStock2
                          join q1 in tmpStock3 on new 
                          { 
                            q.c_gdg, 
                            q.c_rnno, 
                            c_iteno = q.c_iteno.Trim(), 
                            c_batch = q.c_batch.Trim().ToUpper() 
                          } equals new 
                          { 
                            q1.c_gdg,
                            q1.c_rnno,
                            c_iteno = q1.c_iteno.Trim(),
                            c_batch = q1.c_batch.Trim().ToUpper() 
                          } into Left
                          from gLeft in Left.DefaultIfEmpty()
                          where gLeft == null
                           select new LG_STOCKTMP()
                          {
                            c_gdg = q.c_gdg,
                            c_rnno = q.c_rnno,
                            c_iteno = q.c_iteno,
                            c_batch = q.c_batch,
                            n_gqty = q.n_gqty,
                            n_bqty = q.n_bqty
                          }).ToList();

              Stock.AddRange(tmpStock4);
              tmpStock2.Clear();
              tmpStock3.Clear();
              tmpStock4.Clear();

              //tmpStock1.AddRange(tmpStock);
              //tmpStock.Clear();


              // Input SJ

              tmpStock2 = (from q2 in db.LG_SJHs
                             join q3 in db.LG_SJD2s on new { q2.c_gdg, q2.c_sjno } equals new { q3.c_gdg, q3.c_sjno }
                             where (q2.d_sjdate.Value.Year == Tahun) && (q2.d_sjdate.Value.Month == Bulan)
                             group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                           select new LG_STOCKTMP()
                             {
                               c_gdg = sumg.Key.c_gdg,
                               c_rnno = sumg.Key.c_rnno,
                               c_iteno = sumg.Key.c_iteno,
                               c_batch = sumg.Key.c_batch,
                               n_gqty = 0m,
                               n_bqty = 0m
                             }).ToList();


              tmpStock3 = (from q in tmpStock2
                          join q1 in Stock on new 
                          { 
                            q.c_gdg, 
                            q.c_rnno, 
                            c_iteno = q.c_iteno.Trim(), 
                            c_batch = q.c_batch.Trim().ToUpper() 
                          } equals new 
                          { 
                            q1.c_gdg, 
                            q1.c_rnno, 
                            c_iteno = q1.c_iteno.Trim(), 
                            c_batch = q1.c_batch.Trim().ToUpper() 
                          } into Left
                          from gLeft in Left.DefaultIfEmpty()
                          where gLeft == null
                           select new LG_STOCKTMP()
                          {
                            c_gdg = q.c_gdg,
                            c_rnno = q.c_rnno,
                            c_iteno = q.c_iteno,
                            c_batch = q.c_batch,
                            n_gqty = q.n_gqty,
                            n_bqty = q.n_bqty
                          }).ToList();

              Stock.AddRange(tmpStock3);
              tmpStock2.Clear();
              tmpStock3.Clear();

              //tmpStock1.AddRange(tmpStock);
              //tmpStock.Clear();

              // Input PL

              tmpStock2 = (from q2 in db.LG_PLHs
                             join q3 in db.LG_PLD2s on q2.c_plno equals q3.c_plno
                             where (q2.d_pldate.Value.Year == Tahun) && (q2.d_pldate.Value.Month == Bulan)
                             group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                           select new LG_STOCKTMP()
                             {
                               c_gdg = sumg.Key.c_gdg.HasValue ? sumg.Key.c_gdg.Value : '1',
                               c_rnno = sumg.Key.c_rnno,
                               c_iteno = sumg.Key.c_iteno,
                               c_batch = sumg.Key.c_batch,
                               n_gqty = 0m,
                               n_bqty = 0m
                             }).ToList();


              tmpStock3 = (from q in tmpStock2
                          join q1 in Stock on new 
                          { 
                            q.c_gdg, 
                            q.c_rnno, 
                            c_iteno = q.c_iteno.Trim(), 
                            c_batch = q.c_batch.Trim().ToUpper() 
                          } equals new 
                          { 
                            q1.c_gdg, 
                            q1.c_rnno, 
                            c_iteno = q1.c_iteno.Trim(), 
                            c_batch = q1.c_batch.Trim().ToUpper() 
                          } into Left
                          from gLeft in Left.DefaultIfEmpty()
                          where gLeft == null
                           select new LG_STOCKTMP()
                          {
                            c_gdg = q.c_gdg,
                            c_rnno = q.c_rnno,
                            c_iteno = q.c_iteno,
                            c_batch = q.c_batch,
                            n_gqty = q.n_gqty,
                            n_bqty = q.n_bqty
                          }).ToList();

              Stock.AddRange(tmpStock3);
              tmpStock2.Clear();
              tmpStock3.Clear();

              //tmpStock1.AddRange(tmpStock);
              //tmpStock.Clear();

              // Input STH

              tmpStock2 = (from q2 in db.LG_STHs
                             join q3 in db.LG_STD2s on new { q2.c_gdg, q2.c_stno } equals new { q3.c_gdg, q3.c_stno }
                             where (q2.d_stdate.Value.Year == Tahun) && (q2.d_stdate.Value.Month == Bulan)
                             group new { q2, q3 } by new { q2.c_gdg, q3.c_no, q3.c_iteno, q3.c_batch } into sumg
                           select new LG_STOCKTMP()
                             {
                               c_gdg = sumg.Key.c_gdg,
                               c_rnno = sumg.Key.c_no,
                               c_iteno = sumg.Key.c_iteno,
                               c_batch = sumg.Key.c_batch,
                               n_gqty = 0m,
                               n_bqty = 0m
                             }).ToList();

              tmpStock3 = (from q in tmpStock2
                          join q1 in Stock on new 
                          { 
                            q.c_gdg, 
                            q.c_rnno, 
                            c_iteno = q.c_iteno.Trim(),
                            c_batch = q.c_batch.Trim().ToUpper() 
                          } equals new 
                          { 
                            q1.c_gdg, 
                            q1.c_rnno, 
                            c_iteno = q1.c_iteno.Trim(), 
                            c_batch = q1.c_batch.Trim().ToUpper()
                          } into Left
                          from gLeft in Left.DefaultIfEmpty()
                          where Left == null
                           select new LG_STOCKTMP()
                          {
                            c_gdg = q.c_gdg,
                            c_rnno = q.c_rnno,
                            c_iteno = q.c_iteno,
                            c_batch = q.c_batch,
                            n_gqty = q.n_gqty,
                            n_bqty = q.n_bqty
                          }).ToList();

              Stock.AddRange(tmpStock3);
              tmpStock2.Clear();
              tmpStock3.Clear();

              //tmpStock1.AddRange(tmpStock);
              //tmpStock.Clear();

              // Input Combo

              tmpStock2 = (from q2 in db.LG_ComboHs
                             join q3 in db.LG_ComboD2s on new { q2.c_gdg, q2.c_combono } equals new { q3.c_gdg, q3.c_combono }
                             where (q2.d_combodate.Value.Year == Tahun) && (q2.d_combodate.Value.Month == Bulan)
                             && q2.c_type == "01"
                             group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                           select new LG_STOCKTMP()
                             {
                               c_gdg = sumg.Key.c_gdg,
                               c_rnno = sumg.Key.c_rnno,
                               c_iteno = sumg.Key.c_iteno,
                               c_batch = sumg.Key.c_batch,
                               n_gqty = 0m,
                               n_bqty = 0m
                             }).ToList();

              tmpStock3 = (from q in tmpStock2
                          join q1 in Stock on new 
                          { 
                            q.c_gdg, 
                            q.c_rnno, 
                            c_iteno = q.c_iteno.Trim(), 
                            c_batch = q.c_batch.Trim().ToUpper() 
                          } equals new 
                          { 
                            q1.c_gdg, 
                            q1.c_rnno, 
                            c_iteno = q1.c_iteno.Trim(), 
                            c_batch = q1.c_batch.Trim().ToUpper() 
                          } into Left
                          from gLeft in Left.DefaultIfEmpty()
                          where Left == null
                           select new LG_STOCKTMP()
                          {
                            c_gdg = q.c_gdg,
                            c_rnno = q.c_rnno,
                            c_iteno = q.c_iteno,
                            c_batch = q.c_batch,
                            n_gqty = q.n_gqty,
                            n_bqty = q.n_bqty
                          }).ToList();

              Stock.AddRange(tmpStock3);
              tmpStock2.Clear();
              tmpStock3.Clear();

              //tmpStock1.AddRange(tmpStock);
              //tmpStock.Clear();

              // Input Adjustment

              tmpStock2 = (from q2 in db.LG_AdjustHs
                             join q3 in db.LG_AdjustD2s on new { q2.c_gdg, q2.c_adjno } equals new { q3.c_gdg, q3.c_adjno }
                             where (q2.d_adjdate.Value.Year == Tahun) && (q2.d_adjdate.Value.Month == Bulan)
                             group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                           select new LG_STOCKTMP()
                             {
                               c_gdg = sumg.Key.c_gdg,
                               c_rnno = sumg.Key.c_rnno,
                               c_iteno = sumg.Key.c_iteno,
                               c_batch = sumg.Key.c_batch,
                               n_gqty = 0m,
                               n_bqty = 0m
                             }).ToList();

              tmpStock3 = (from q in tmpStock2
                          join q1 in Stock on new 
                          { 
                            q.c_gdg, 
                            q.c_rnno, 
                            c_iteno = q.c_iteno.Trim(), 
                            c_batch = q.c_batch.Trim().ToUpper() 
                          } equals new 
                          { 
                            q1.c_gdg, 
                            q1.c_rnno, 
                            c_iteno = q1.c_iteno.Trim(), 
                            c_batch = q1.c_batch.Trim().ToUpper() 
                          } into Left
                          from gLeft in Left.DefaultIfEmpty()
                           where gLeft == null
                           select new LG_STOCKTMP()
                          {
                            c_gdg = q.c_gdg,
                            c_rnno = q.c_rnno,
                            c_iteno = q.c_iteno,
                            c_batch = q.c_batch,
                            n_gqty = q.n_gqty,
                            n_bqty = q.n_bqty
                          }).ToList();

              Stock.AddRange(tmpStock3);
              tmpStock2.Clear();
              tmpStock3.Clear();

              //tmpStock1.AddRange(tmpStock);
              //tmpStock.Clear();

              Stock = (from q in Stock
                       group q by new { q.c_gdg, q.c_rnno, q.c_iteno, q.c_batch } into g
                       select new LG_STOCKTMP()
                       {
                         c_gdg = g.Key.c_gdg,
                         c_rnno = g.Key.c_rnno,
                         c_iteno = g.Key.c_iteno,
                         c_batch = g.Key.c_batch,
                         n_gqty = g.Sum(x => x.n_gqty),
                         n_bqty = g.Sum(x => x.n_bqty)
                       }).ToList();

              //Update stock Add Output

              // Update RC

              tmpStock2 = (from q2 in db.LG_RCHes
                          join q3 in db.LG_RCD1s on new { q2.c_gdg, q2.c_rcno } equals new { q3.c_gdg, q3.c_rcno }
                          where (q2.d_rcdate.Value.Year == Tahun) && (q2.d_rcdate.Value.Month == Bulan)
                          group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                           select new LG_STOCKTMP()
                          {
                            c_gdg = sumg.Key.c_gdg,
                            c_rnno = sumg.Key.c_rnno,
                            c_iteno = sumg.Key.c_iteno,
                            c_batch = sumg.Key.c_batch,
                            n_gqty = sumg.Sum(x => x.q3.c_type == "01" ? x.q3.n_qty.Value : 0m),
                            n_bqty = sumg.Sum(x => x.q3.c_type == "01" ? x.q3.n_qty.Value : 0m)
                          }).ToList();

              Stock = (from q in Stock
                       join q1 in tmpStock2 on new 
                       { 
                         q.c_gdg, 
                         q.c_rnno, 
                         c_iteno = q.c_iteno.Trim(), 
                         c_batch = q.c_batch.Trim().ToUpper() 
                       } equals new 
                       { 
                         q1.c_gdg, 
                         q1.c_rnno, 
                         c_iteno = q1.c_iteno.Trim(), 
                         c_batch = q1.c_batch.Trim().ToUpper() 
                       } into Left
                       from gLeft in Left.DefaultIfEmpty()
                       select new LG_STOCKTMP()
                       {
                         c_gdg = q.c_gdg,
                         c_rnno = q.c_rnno,
                         c_iteno = q.c_iteno,
                         c_batch = q.c_batch,
                         n_gqty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty),
                         n_bqty = q.n_bqty - (gLeft == null ? 0m : gLeft.n_bqty)
                       }).ToList();

              tmpStock2.Clear();

              // Update RS		

              tmpStock2 = (from q2 in db.LG_RSHes
                          join q3 in db.LG_RSD2s on new { q2.c_gdg, q2.c_rsno } equals new { q3.c_gdg, q3.c_rsno }
                          where (q2.d_rsdate.Value.Year == Tahun) && (q2.d_rsdate.Value.Month == Bulan)
                          group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                           select new LG_STOCKTMP()
                          {
                            c_gdg = sumg.Key.c_gdg,
                            c_rnno = sumg.Key.c_rnno,
                            c_iteno = sumg.Key.c_iteno,
                            c_batch = sumg.Key.c_batch,
                            n_gqty = sumg.Sum(x => x.q3.n_gqty.HasValue ? x.q3.n_gqty.Value : 0m),
                            n_bqty = sumg.Sum(x => x.q3.n_bqty.HasValue ? x.q3.n_bqty.Value : 0m)
                          }).ToList();

              Stock = (from q in Stock
                       join q1 in tmpStock2 on new 
                       { 
                         q.c_gdg, 
                         q.c_rnno, 
                         c_iteno = q.c_iteno.Trim(), 
                         c_batch = q.c_batch.Trim().ToUpper() 
                       } equals new 
                       { 
                         q1.c_gdg, 
                         q1.c_rnno, 
                         c_iteno = q1.c_iteno.Trim(), 
                         c_batch = q1.c_batch.Trim().ToUpper() 
                       } into Left
                       from gLeft in Left.DefaultIfEmpty()
                       select new LG_STOCKTMP()
                       {
                         c_gdg = q.c_gdg,
                         c_rnno = q.c_rnno,
                         c_iteno = q.c_iteno,
                         c_batch = q.c_batch,
                         n_gqty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty),
                         n_bqty = q.n_bqty - (gLeft == null ? 0m : gLeft.n_bqty)
                       }).ToList();

              tmpStock2.Clear();

              // Update SJ		

              tmpStock2 = (from q2 in db.LG_SJHs
                          join q3 in db.LG_SJD2s on new { q2.c_gdg, q2.c_sjno } equals new { q3.c_gdg, q3.c_sjno }
                          where (q2.d_sjdate.Value.Year == Tahun) && (q2.d_sjdate.Value.Month == Bulan)
                          group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                           select new LG_STOCKTMP()
                          {
                            c_gdg = sumg.Key.c_gdg,
                            c_rnno = sumg.Key.c_rnno,
                            c_iteno = sumg.Key.c_iteno,
                            c_batch = sumg.Key.c_batch,
                            n_gqty = sumg.Sum(x => x.q3.n_gqty.HasValue ? x.q3.n_gqty.Value : 0m),
                            n_bqty = sumg.Sum(x => x.q3.n_bqty.HasValue ? x.q3.n_bqty.Value : 0m)
                          }).ToList();

              Stock = (from q in Stock
                       join q1 in tmpStock2 on new 
                       { 
                         q.c_gdg, 
                         q.c_rnno, 
                         c_iteno = q.c_iteno.Trim(), 
                         c_batch = q.c_batch.Trim().ToUpper() 
                       } equals new 
                       { 
                         q1.c_gdg, 
                         q1.c_rnno, 
                         c_iteno = q1.c_iteno.Trim(), 
                         c_batch = q1.c_batch.Trim().ToUpper() 
                       } into Left
                       from gLeft in Left.DefaultIfEmpty()
                       select new LG_STOCKTMP()
                       {
                         c_gdg = q.c_gdg,
                         c_rnno = q.c_rnno,
                         c_iteno = q.c_iteno,
                         c_batch = q.c_batch,
                         n_gqty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty),
                         n_bqty = q.n_bqty - (gLeft == null ? 0m : gLeft.n_bqty)
                       }).ToList();

              tmpStock2.Clear();

              // Update PL		

              tmpStock2 = (from q2 in db.LG_PLHs
                          join q3 in db.LG_PLD2s on q2.c_plno equals q3.c_plno
                          where (q2.d_pldate.Value.Year == Tahun) && (q2.d_pldate.Value.Month == Bulan)
                          group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                           select new LG_STOCKTMP()
                          {
                            c_gdg = sumg.Key.c_gdg.HasValue ? sumg.Key.c_gdg.Value : '1',
                            c_rnno = sumg.Key.c_rnno,
                            c_iteno = sumg.Key.c_iteno,
                            c_batch = sumg.Key.c_batch,
                            n_gqty = sumg.Sum(x => x.q3.n_qty.HasValue ? x.q3.n_qty.Value : 0m),
                            n_bqty = 0m
                          }).ToList();

              Stock = (from q in Stock
                       join q1 in tmpStock2 on new 
                       { 
                         q.c_gdg, 
                         q.c_rnno, 
                         c_iteno = q.c_iteno.Trim(), 
                         c_batch = q.c_batch.Trim().ToUpper() 
                       } equals new 
                       { 
                         q1.c_gdg, 
                         q1.c_rnno, 
                         c_iteno = q1.c_iteno.Trim(), 
                         c_batch = q1.c_batch.Trim().ToUpper() 
                       } into Left
                       from gLeft in Left.DefaultIfEmpty()
                       select new LG_STOCKTMP()
                       {
                         c_gdg = q.c_gdg,
                         c_rnno = q.c_rnno,
                         c_iteno = q.c_iteno,
                         c_batch = q.c_batch,
                         n_gqty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty),
                         n_bqty = q.n_bqty - (gLeft == null ? 0m : gLeft.n_bqty)
                       }).ToList();

              tmpStock2.Clear();

              // Update STT		

              tmpStock2 = (from q2 in db.LG_STHs
                          join q3 in db.LG_STD2s on new { q2.c_gdg, q2.c_stno } equals new { q3.c_gdg, q3.c_stno }
                          where (q2.d_stdate.Value.Year == Tahun) && (q2.d_stdate.Value.Month == Bulan)
                          group new { q2, q3 } by new { q2.c_gdg, q3.c_no, q3.c_iteno, q3.c_batch } into sumg
                           select new LG_STOCKTMP()
                          {
                            c_gdg = sumg.Key.c_gdg,
                            c_rnno = sumg.Key.c_no,
                            c_iteno = sumg.Key.c_iteno,
                            c_batch = sumg.Key.c_batch,
                            n_gqty = sumg.Sum(x => x.q3.n_qty.HasValue ? x.q3.n_qty.Value : 0m),
                            n_bqty = 0m
                          }).ToList();

              Stock = (from q in Stock
                       join q1 in tmpStock2 on new 
                       { 
                         q.c_gdg, 
                         q.c_rnno, 
                         c_iteno = q.c_iteno.Trim(), 
                         c_batch = q.c_batch.Trim().ToUpper() 
                       } equals new 
                       { 
                         q1.c_gdg, 
                         q1.c_rnno, 
                         c_iteno = q1.c_iteno.Trim(), 
                         c_batch = q1.c_batch.Trim().ToUpper() 
                       } into Left
                       from gLeft in Left.DefaultIfEmpty()
                       select new LG_STOCKTMP()
                       {
                         c_gdg = q.c_gdg,
                         c_rnno = q.c_rnno,
                         c_iteno = q.c_iteno,
                         c_batch = q.c_batch,
                         n_gqty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty),
                         n_bqty = q.n_bqty - (gLeft == null ? 0m : gLeft.n_bqty)
                       }).ToList();

              tmpStock2.Clear();

              // Update Combo

              tmpStock2 = (from q2 in db.LG_ComboHs
                          join q3 in db.LG_ComboD2s on new { q2.c_gdg, q2.c_combono } equals new { q3.c_gdg, q3.c_combono }
                          where (q2.d_combodate.Value.Year == Tahun) && (q2.d_combodate.Value.Month == Bulan)
                          group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                           select new LG_STOCKTMP()
                          {
                            c_gdg = sumg.Key.c_gdg,
                            c_rnno = sumg.Key.c_rnno,
                            c_iteno = sumg.Key.c_iteno,
                            c_batch = sumg.Key.c_batch,
                            n_gqty = sumg.Sum(x => x.q3.n_qty.HasValue ? x.q3.n_qty.Value : 0m),
                            n_bqty = 0
                          }).ToList();

              Stock = (from q in Stock
                       join q1 in tmpStock2 on new 
                       { 
                         q.c_gdg, 
                         q.c_rnno, 
                         c_iteno = q.c_iteno.Trim(), 
                         c_batch = q.c_batch.Trim().ToUpper() 
                       } equals new 
                       { 
                         q1.c_gdg, 
                         q1.c_rnno, 
                         c_iteno = q1.c_iteno.Trim(), 
                         c_batch = q1.c_batch.Trim().ToUpper() 
                       } into Left
                       from gLeft in Left.DefaultIfEmpty()
                       select new LG_STOCKTMP()
                       {
                         c_gdg = q.c_gdg,
                         c_rnno = q.c_rnno,
                         c_iteno = q.c_iteno,
                         c_batch = q.c_batch,
                         n_gqty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty),
                         n_bqty = q.n_bqty - (gLeft == null ? 0m : gLeft.n_bqty)
                       }).ToList();

              tmpStock2.Clear();

              // Update Adjustment

              tmpStock2 = (from q2 in db.LG_AdjustHs
                          join q3 in db.LG_AdjustD2s on new { q2.c_gdg, q2.c_adjno } equals new { q3.c_gdg, q3.c_adjno }
                          where (q2.d_adjdate.Value.Year == Tahun) && (q2.d_adjdate.Value.Month == Bulan)
                          group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                           select new LG_STOCKTMP()
                          {
                            c_gdg = sumg.Key.c_gdg,
                            c_rnno = sumg.Key.c_rnno,
                            c_iteno = sumg.Key.c_iteno,
                            c_batch = sumg.Key.c_batch,
                            n_gqty = sumg.Sum(x => x.q3.n_gqty.HasValue ? x.q3.n_gqty.Value : 0m),
                            n_bqty = sumg.Sum(x => x.q3.n_bqty.HasValue ? x.q3.n_bqty.Value : 0m)
                          }).ToList();

              Stock = (from q in Stock
                       join q1 in tmpStock2 on new 
                       { 
                         q.c_gdg, 
                         q.c_rnno, 
                         c_iteno = q.c_iteno.Trim(), 
                         c_batch = q.c_batch.Trim().ToUpper() 
                       } equals new 
                       { 
                         q1.c_gdg, 
                         q1.c_rnno, 
                         c_iteno = q1.c_iteno.Trim(), 
                         c_batch = q1.c_batch.Trim().ToUpper() 
                       } into Left
                       from gLeft in Left.DefaultIfEmpty()
                       select new LG_STOCKTMP()
                       {
                         c_gdg = q.c_gdg,
                         c_rnno = q.c_rnno,
                         c_iteno = q.c_iteno,
                         c_batch = q.c_batch,
                         n_gqty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty),
                         n_bqty = q.n_bqty - (gLeft == null ? 0m : gLeft.n_bqty)
                       }).ToList();

              tmpStock2.Clear();

              StockFinal = (from q in Stock
                            select new LG_STOCKTMP()
                          {
                            s_tahun = short.Parse(Tahun.ToString()),
                            t_bulan = byte.Parse(Bulan.ToString()),
                            c_gdg = q.c_gdg,
                            c_noref = q.c_rnno,
                            c_iteno = q.c_iteno,
                            c_batch = q.c_batch,
                            n_gqty = q.n_gqty,
                            n_bqty = q.n_bqty
                          }).ToList();

              nResult = StockFinal.Count;

              if ((nResult > 0))
              {
                //ScmsSoaLibrary.Bussiness.Commons.RunningGenerateRS(db, nipEntry, true,
                //  lstResultNew.ToArray());

                CommonQuerySP cqSP = new CommonQuerySP();
                cqSP.SP_LG_CalcClosingLog(db.Connection.ConnectionString, Nip, true, StockFinal.ToArray());

              }

              if (nResult > 0)
              {
                //dic.Add(Constant.DEFAULT_NAMING_RECORDS, lstStock.ToArray());

                dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nResult);

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                lstStockST.Clear();
              }

              #endregion
            }
            break;

          #endregion

          #region MODEL_PROCESS_QUERY_SP_CLOSSINGLOG_TRANSAKSI

          case Constant.MODEL_PROCESS_QUERY_SP_CLOSSINGLOG_TRANSAKSI:
            {
              int Tahun = 0, Bulan = 0;
              Tahun = (parameters.ContainsKey("Tahun") ? int.Parse(((Functionals.ParameterParser)parameters["Tahun"]).Value.ToString()) : 0);
              Bulan = (parameters.ContainsKey("Bulan") ? int.Parse(((Functionals.ParameterParser)parameters["Bulan"]).Value.ToString()) : 0);
              string Nip = (parameters.ContainsKey("Entry").ToString() == null ? string.Empty : ((Functionals.ParameterParser)parameters["Entry"]).Value.ToString());
              int ExtYear = (Bulan == 1 ? Tahun - 1 : Tahun);
              int ExtMonth = (Bulan == 1 ? 12 : Bulan - 1);
              string[] a = new string[] { "01", "02" };
              string[] tipeRNBeli = new string[] { "01", "03" };
              string tipRNRS = "02";
              string tipRNRetur = "04";

              List<LG_ClosePO> lstClosePO = null;
              List<LG_CloseClaim> lstCloseClaim = null;
              List<LG_CloseClaimAcc> lstCloseClaimAcc = null;
              List<LG_CloseR> lstCloseRS = null;
              List<LG_ClosePL> lstClosePL = null;
              List<LG_ClosePLConfirm> lstClosePLConf = null;
              List<LG_CloseDO> lstCloseDO = null;
              List<LG_CloseSP> lstCloseSP = null;
              List<LG_CloseSPG> lstCloseSPG = null;
              List<LG_CloseSJExp> lstCloseSJExp = null;
              List<LG_CloseSJRN> lstCloseSJRN = null;
              List<LG_CloseMemo> lstCloseMemo = null;
              List<LG_CloseMT> lstCloseMT = null;

              List<LG_TRANSAKSI> Transaksi = new List<LG_TRANSAKSI>();
              List<LG_TRANSAKSI> tmpS = null;
              List<LG_TRANSAKSI> tmp3 = null;
              List<LG_TRANSAKSI> TmpPo = new List<LG_TRANSAKSI>();
              List<LG_TRANSAKSI> TmpClaim = new List<LG_TRANSAKSI>();
              List<LG_TRANSAKSI> TmpClaimAcc = new List<LG_TRANSAKSI>();
              List<LG_TRANSAKSI> TmpRS = new List<LG_TRANSAKSI>();
              List<LG_TRANSAKSI> TmpPL = new List<LG_TRANSAKSI>();
              List<LG_TRANSAKSI> TmpPLConf = new List<LG_TRANSAKSI>();
              List<LG_TRANSAKSI> TmpDO = new List<LG_TRANSAKSI>();
              List<LG_TRANSAKSI> TmpSPG = new List<LG_TRANSAKSI>();
              List<LG_TRANSAKSI> TmpSJExp = new List<LG_TRANSAKSI>();
              List<LG_TRANSAKSI> TmpSJRN = new List<LG_TRANSAKSI>();
              List<LG_TRANSAKSI> TmpMemo = new List<LG_TRANSAKSI>();
              List<LG_TRANSAKSI> TmpST = new List<LG_TRANSAKSI>();
              List<LG_TRANSAKSI> tmp2 = new List<LG_TRANSAKSI>();
              List<LG_TRANSAKSI> tmp1 = new List<LG_TRANSAKSI>();
              LG_TRANSAKSI LopTmp = new LG_TRANSAKSI();

              #region Closing Transaksi

              tmpS = (from q in db.LG_ClosePOs
                      where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                      select new LG_TRANSAKSI()
                      {
                        c_gdg = q.c_gdg,
                        c_no = q.c_pono,
                        c_iteno = q.c_iteno,
                        n_gqty = q.n_qty.Value
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              tmpS = (from q in db.LG_POHs
                      join q1 in db.LG_POD1s on new { q.c_gdg, q.c_pono } equals new { q1.c_gdg, q1.c_pono }
                      where q.d_podate.Value.Year == Tahun && q.d_podate.Value.Month == Bulan
                      select new LG_TRANSAKSI()
                      {
                        c_gdg = q.c_gdg,
                        c_no = q.c_pono,
                        c_iteno = q1.c_iteno,
                        n_gqty = (q1.n_qty.HasValue ? q1.n_qty.Value : 0)
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              lstClosePO = (from q in Transaksi
                            join q1 in
                              (from q1 in db.LG_RNHs
                               join q2 in db.LG_RND2s on new { q1.c_gdg, q1.c_rnno } equals new { q2.c_gdg, q2.c_rnno }
                               where q1.d_rndate.Value.Year == Tahun && q1.d_rndate.Value.Month == Bulan &&
                               q1.c_type == "01" && q2.c_type == "01"
                               group new { q1, q2 } by new { q1.c_gdg, q2.c_no, q2.c_iteno } into g
                               select new LG_TRANSAKSI()
                               {
                                 c_no = g.Key.c_no,
                                 c_iteno = g.Key.c_iteno,
                                 n_gqty = g.Sum(x => (x.q2.n_gqty.Value)),
                               }).ToList() on new { q.c_no, q.c_iteno } equals new { q1.c_no, q1.c_iteno } into Left
                            from gLeft in Left.DefaultIfEmpty()
                            select new LG_ClosePO()
                            {
                              c_gdg = q.c_gdg,
                              c_pono = q.c_no,
                              c_iteno = q.c_iteno,
                              n_qty = (q.n_gqty - (gLeft == null ? 0 : gLeft.n_gqty))
                            }).ToList();


              lstClosePO = (from q in lstClosePO
                            join q1 in
                              (from q1 in db.LG_AdjPOHs
                               join q2 in db.LG_AdjPODs on new { q1.c_gdg, q1.c_adjno } equals new { q2.c_gdg, q2.c_adjno }
                               where q1.d_adjdate.Value.Year == Tahun && q1.d_adjdate.Value.Month == Bulan
                               group new { q1, q2 } by new { q1.c_adjno, q2.c_iteno } into g
                               select new LG_TRANSAKSI()
                               {
                                 c_no = g.Key.c_adjno,
                                 c_iteno = g.Key.c_iteno,
                                 n_gqty = g.Sum(x => x.q2.n_qty.Value),
                               }).ToList() on new { q.c_pono, q.c_iteno } equals new { c_pono = q1.c_no, q1.c_iteno } into Left
                            from gLeft in Left.DefaultIfEmpty()
                            select new LG_ClosePO()
                            {
                              c_gdg = q.c_gdg,
                              c_pono = q.c_pono,
                              c_iteno = q.c_iteno,
                              n_qty = (q.n_qty - (gLeft == null ? 0 : gLeft.n_gqty))
                            }).ToList();

              lstClosePO = lstClosePO.Where(x => x.n_qty > 0).ToList();

              Transaksi.Clear();

              //claim

              tmpS = (from q in db.LG_CloseClaims
                      where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                      select new LG_TRANSAKSI()
                      {
                        c_no = q.c_claimno,
                        c_iteno = q.c_iteno,
                        n_gqty = q.n_qty.Value
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              tmpS = (from q in db.LG_ClaimHs
                      join q1 in db.LG_ClaimD1s on new { q.c_claimno } equals new { q1.c_claimno }
                      where q.d_claimdate.Value.Year == Tahun && q.d_claimdate.Value.Month == Bulan
                      group q1 by new { q1.c_claimno, q1.c_iteno } into g
                      select new LG_TRANSAKSI()
                      {
                        c_no = g.Key.c_claimno,
                        c_iteno = g.Key.c_iteno,
                        n_gqty = g.Sum(x => x.n_qty.Value)
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              lstCloseClaim = (from q in Transaksi
                               join q3 in
                                 (from q1 in db.LG_ClaimAccHes
                                  join q2 in db.LG_ClaimAccDs on new { q1.c_claimaccno } equals new { q2.c_claimaccno }
                                  where q1.d_claimaccdate.Value.Year == Tahun && q1.d_claimaccdate.Value.Month == Bulan
                                  group new { q1, q2 } by new { q2.c_claimaccno, q2.c_iteno } into g
                                  select new LG_TRANSAKSI()
                                  {
                                    c_no = g.Key.c_claimaccno,
                                    c_iteno = g.Key.c_iteno,
                                    n_gqty = g.Sum(x => x.q2.n_qtyacc.Value) + g.Sum(x => x.q2.n_qtytolak.Value),
                                  }) on new { q.c_no, q.c_iteno } equals new { q3.c_no, q3.c_iteno } into Left
                               from gLeft in Left.DefaultIfEmpty()
                               select new LG_CloseClaim()
                               {
                                 c_claimno = q.c_no,
                                 c_iteno = q.c_iteno,
                                 n_qty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty)
                               }).ToList();

              lstCloseClaim = lstCloseClaim.Where(x => x.n_qty > 0).ToList();

              Transaksi.Clear();

              //claim Acc

              tmpS = (from q in db.LG_CloseClaimAccs
                      where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                      select new LG_TRANSAKSI()
                      {
                        c_no = q.c_claimaccno,
                        c_iteno = q.c_iteno,
                        n_gqty = q.n_qty.Value
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              tmpS = (from q in db.LG_ClaimAccHes
                      join q1 in db.LG_ClaimAccDs on new { q.c_claimaccno } equals new { q1.c_claimaccno }
                      where q.d_claimaccdate.Value.Year == Tahun && q.d_claimaccdate.Value.Month == Bulan
                      group q1 by new { q1.c_claimaccno, q1.c_iteno } into g
                      select new LG_TRANSAKSI()
                      {
                        c_no = g.Key.c_claimaccno,
                        c_iteno = g.Key.c_iteno,
                        n_gqty = g.Sum(x => x.n_qtyacc.HasValue ? x.n_qtyacc.Value : 0)
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              lstCloseClaimAcc = (from q in Transaksi
                                  join q3 in
                                    (from q1 in db.LG_FBRHs
                                     join q2 in db.LG_FBRD1s on q1.c_fbno equals q2.c_fbno
                                     join q4 in db.LG_FBRD3s on q2.c_fbno equals q4.c_fbno
                                     where q1.d_fbdate.Value.Year == Tahun && q1.d_fbdate.Value.Month == Bulan
                                     && q1.c_type == "03"
                                     group new { q4, q2 } by new { q4.c_claimaccno, q2.c_iteno } into g
                                     select new LG_TRANSAKSI()
                                     {
                                       c_no = g.Key.c_claimaccno,
                                       c_iteno = g.Key.c_iteno,
                                       n_gqty = g.Sum(x => x.q2.n_gqty.Value),
                                     }) on new { q.c_gdg, q.c_no, q.c_iteno } equals new { q3.c_gdg, q3.c_no, q3.c_iteno } into Left
                                  from gLeft in Left.DefaultIfEmpty()
                                  select new LG_CloseClaimAcc()
                                  {
                                    c_claimaccno = q.c_no,
                                    c_iteno = q.c_iteno,
                                    n_qty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty)
                                  }).ToList();

              lstCloseClaimAcc = (from q in lstCloseClaimAcc
                                  join q3 in
                                    (from q1 in db.LG_SJHs
                                     join q2 in db.LG_SJD1s on new { q1.c_gdg, q1.c_sjno } equals new { q2.c_gdg, q2.c_sjno }
                                     where q1.d_sjdate.Value.Year == Tahun && q1.d_sjdate.Value.Month == Bulan
                                     && q1.c_type == "03"
                                     group new { q1, q2 } by new { q2.c_spgno, q2.c_iteno } into g
                                     select new LG_TRANSAKSI()
                                     {
                                       c_no = g.Key.c_spgno,
                                       c_iteno = g.Key.c_iteno,
                                       n_gqty = g.Sum(x => x.q2.n_gqty.Value),
                                     }) on new { q.c_claimaccno, q.c_iteno } equals new { c_claimaccno = q3.c_no, q3.c_iteno } into Left
                                  from gLeft in Left.DefaultIfEmpty()
                                  select new LG_CloseClaimAcc()
                                  {
                                    c_claimaccno = q.c_claimaccno,
                                    c_iteno = q.c_iteno,
                                    n_qty = q.n_qty.Value - (gLeft == null ? 0m : gLeft.n_gqty)
                                  }).ToList();

              lstCloseClaimAcc = (from q in lstCloseClaimAcc
                                  join q3 in
                                    (from q1 in db.LG_RNHs
                                     join q2 in db.LG_RND2s on new { q1.c_gdg, q1.c_rnno } equals new { q2.c_gdg, q2.c_rnno }
                                     where q1.d_rndate.Value.Year == Tahun && q1.d_rndate.Value.Month == Bulan
                                     && q1.c_type == "03"
                                     group new { q1, q2 } by new { q2.c_no, q2.c_iteno } into g
                                     select new LG_TRANSAKSI()
                                     {
                                       c_no = g.Key.c_no,
                                       c_iteno = g.Key.c_iteno,
                                       n_gqty = g.Sum(x => x.q2.n_gqty.Value),
                                     }) on new { q.c_claimaccno, q.c_iteno } equals new { c_claimaccno = q3.c_no, q3.c_iteno } into Left
                                  from gLeft in Left.DefaultIfEmpty()
                                  select new LG_CloseClaimAcc()
                                  {
                                    c_claimaccno = q.c_claimaccno,
                                    c_iteno = q.c_iteno,
                                    n_qty = q.n_qty.Value - (gLeft == null ? 0m : gLeft.n_gqty)
                                  }).ToList();

              lstCloseClaimAcc = lstCloseClaimAcc.Where(x => x.n_qty > 0).ToList();

              Transaksi.Clear();

              //Retur Supplier

              tmpS = (from q in db.LG_CloseRs
                      where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                      select new LG_TRANSAKSI()
                      {
                        c_gdg = q.c_gdg,
                        c_no = q.c_rsno,
                        c_iteno = q.c_iteno,
                        c_batch = q.c_batch,
                        c_noref = q.c_rnno,
                        n_gqty = q.n_gqty.Value,
                        n_bqty = q.n_bqty.Value
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              tmpS = (from q in db.LG_RSHes
                      join q1 in db.LG_RSD2s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                      where q.d_rsdate.Value.Year == Tahun && q.d_rsdate.Value.Month == Bulan
                      group q1 by new { q1.c_gdg, q1.c_rsno, q1.c_iteno, q1.c_batch, q1.c_rnno } into g
                      select new LG_TRANSAKSI()
                      {
                        c_gdg = g.Key.c_gdg,
                        c_no = g.Key.c_rsno,
                        c_iteno = g.Key.c_iteno,
                        c_batch = g.Key.c_batch,
                        c_noref = g.Key.c_rnno,
                        n_gqty = g.Sum(x => x.n_gqty.HasValue ? x.n_gqty.Value : 0),
                        n_bqty = g.Sum(x => x.n_bqty.HasValue ? x.n_bqty.Value : 0)
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              lstCloseRS = (from q in Transaksi
                            join q3 in
                              (from q1 in db.LG_RNHs
                               join q2 in db.LG_RND3s on new { q1.c_gdg, q1.c_rnno } equals new { q2.c_gdg, q2.c_rnno }
                               where q1.d_rndate.Value.Year == Tahun && q1.d_rndate.Value.Month == Bulan
                               && a.Contains(q1.c_type)
                               group q2 by new { q2.c_iteno, q2.c_no, q2.c_batch, q2.c_type, q2.c_rn } into g
                               select new LG_TRANSAKSI()
                               {
                                 c_no = g.Key.c_no,
                                 c_iteno = g.Key.c_iteno,
                                 c_batch = g.Key.c_batch,
                                 c_type = g.Key.c_type,
                                 c_noref = g.Key.c_rn,
                                 n_gqty = g.Sum(x => x.n_gqty.Value),
                                 n_bqty = g.Sum(x => x.n_bqty.Value)
                               }) on new { q.c_gdg, q.c_no, q.c_iteno, q.c_batch, q.c_noref } equals new { q3.c_gdg, q3.c_no, q3.c_iteno, q3.c_batch, q3.c_noref } into Left
                            from gLeft in Left.DefaultIfEmpty()
                            select new LG_CloseR()
                            {
                              c_gdg = q.c_gdg,
                              c_rsno = q.c_no,
                              c_iteno = q.c_iteno,
                              c_batch = q.c_batch,
                              c_rnno = q.c_noref,
                              n_gqty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty),
                              n_bqty = q.n_bqty - (gLeft == null ? 0m : gLeft.n_bqty)
                            }).ToList();

              lstCloseRS = (from q in lstCloseRS
                            join q3 in
                              (from q1 in db.LG_FBRHs
                               join q2 in db.LG_FBRD2s on q1.c_fbno equals q2.c_fbno
                               where q1.d_fbdate.Value.Year == Tahun && q1.d_fbdate.Value.Month == Bulan
                               select new LG_TRANSAKSI()
                               {
                                 c_no = q2.c_rsno,
                                 c_noref = q2.c_rnno
                               }).Distinct().ToList() on new { q.c_rsno, q.c_rnno } equals new { c_rsno = q3.c_no, c_rnno = q3.c_noref } into Left
                            from gLeft in Left.DefaultIfEmpty()
                            select new LG_CloseR()
                            {
                              c_gdg = q.c_gdg,
                              c_rsno = q.c_rsno,
                              c_iteno = q.c_iteno,
                              c_batch = q.c_batch,
                              c_rnno = q.c_rnno,
                              n_gqty = (gLeft == null ? q.n_gqty : 0m),
                              n_bqty = (gLeft == null ? q.n_bqty : 0m)
                            }).ToList();

              lstCloseRS = lstCloseRS.Where(x => (x.n_gqty > 0 || x.n_bqty > 0)).ToList();

              Transaksi.Clear();

              //Surat Pesanan

              tmpS = (from q in db.LG_CloseSPs
                      where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                      select new LG_TRANSAKSI()
                      {
                        c_no = q.c_spno,
                        c_iteno = q.c_iteno,
                        c_type = q.c_type,
                        n_gqty = (q.n_qty.HasValue ? q.n_qty.Value : 0)
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              tmpS = (from q in db.LG_SPHs
                      join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
                      where q.d_spdate.Value.Year == Tahun && q.d_spdate.Value.Month == Bulan
                      select new LG_TRANSAKSI()
                      {
                        c_no = q.c_spno,
                        c_iteno = q1.c_iteno,
                        c_type = q1.c_type,
                        n_gqty = (q1.n_acc.HasValue ? q1.n_acc.Value : 0)
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              lstCloseSP = (from q in Transaksi
                            join q3 in
                              (from q1 in db.LG_PLHs
                               join q2 in db.LG_PLD1s on new { q1.c_plno } equals new { q2.c_plno }
                               where q1.d_pldate.Value.Year == Tahun && q1.d_pldate.Value.Month == Bulan
                               group q2 by new { q2.c_spno, q2.c_iteno, q2.c_type } into g
                               select new LG_TRANSAKSI()
                               {
                                 c_no = g.Key.c_spno,
                                 c_iteno = g.Key.c_iteno,
                                 c_type = g.Key.c_type,
                                 n_gqty = g.Sum(x => (x.n_qty.HasValue ? x.n_qty.Value : 0))
                               }) on new { q.c_no, q.c_iteno, q.c_type } equals new { q3.c_no, q3.c_iteno, q3.c_type } into Left
                            from gLeft in Left.DefaultIfEmpty()
                            select new LG_CloseSP()
                            {
                              c_spno = q.c_no,
                              c_iteno = q.c_iteno,
                              c_type = q.c_type,
                              n_qty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty)
                            }).ToList();

              lstCloseSP = (from q in lstCloseSP
                            join q3 in
                              (from q1 in db.LG_AdjSPHs
                               join q2 in db.LG_AdjSPDs on new { q1.c_adjno } equals new { q2.c_adjno }
                               where q1.d_adjdate.Value.Year == Tahun && q1.d_adjdate.Value.Month == Bulan
                               group q2 by new { q2.c_spno, q2.c_iteno, q2.c_type } into g
                               select new LG_TRANSAKSI()
                               {
                                 c_no = g.Key.c_spno,
                                 c_iteno = g.Key.c_iteno,
                                 c_type = g.Key.c_type,
                                 n_gqty = g.Sum(x => (x.n_qty.HasValue ? x.n_qty.Value : 0))
                               }) on new { q.c_spno, q.c_iteno, q.c_type } equals new { c_spno = q3.c_no, q3.c_iteno, q3.c_type } into Left
                            from gLeft in Left.DefaultIfEmpty()
                            select new LG_CloseSP()
                            {
                              c_spno = q.c_spno,
                              c_iteno = q.c_iteno,
                              c_type = q.c_type,
                              n_qty = q.n_qty - (gLeft == null ? 0m : gLeft.n_gqty)
                            }).ToList();

              lstCloseSP = lstCloseSP.Where(x => x.n_qty > 0).ToList();

              Transaksi.Clear();

              //Packing List

              tmpS = (from q in db.LG_ClosePLs
                      where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                      select new LG_TRANSAKSI()
                      {
                        c_no = q.c_plno
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              tmpS = (from q in db.LG_PLHs
                      where q.d_pldate.Value.Year == Tahun && q.d_pldate.Value.Month == Bulan
                      && q.l_confirm == false
                      select new LG_TRANSAKSI()
                      {
                        c_no = q.c_plno
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              lstClosePL = (from q in Transaksi
                            select new LG_ClosePL()
                            {
                              c_plno = q.c_no
                            }).ToList();

              Transaksi.Clear();

              //Packing List Confirm

              tmpS = (from q in db.LG_ClosePLConfirms
                      where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                      select new LG_TRANSAKSI()
                      {
                        c_no = q.c_plno,
                        c_iteno = q.c_iteno,
                        c_noref = q.c_spno,
                        c_type = q.c_type,
                        c_batch = q.c_batch,
                        n_gqty = (q.n_qty.HasValue ? q.n_qty.Value : 0)
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              tmpS = (from q in db.LG_PLHs
                      join q1 in db.LG_PLD1s on q.c_plno equals q1.c_plno
                      where q.d_pldate.Value.Year == Tahun && q.d_pldate.Value.Month == Bulan
                      select new LG_TRANSAKSI()
                      {
                        c_no = q.c_plno,
                        c_iteno = q1.c_iteno,
                        c_noref = q1.c_spno,
                        c_type = q1.c_type,
                        c_batch = q1.c_batch,
                        n_gqty = (q1.n_qty.HasValue ? q1.n_qty.Value : 0)
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              lstClosePLConf = (from q in Transaksi
                                join q3 in
                                  (from q1 in db.LG_DOHs
                                   join q2 in db.LG_DOD1s on q1.c_dono equals q2.c_dono
                                   where q1.d_dodate.Value.Year == Tahun && q1.d_dodate.Value.Month == Bulan
                                   select new LG_TRANSAKSI()
                                   {
                                     c_no = q1.c_plno,
                                     c_iteno = q2.c_iteno,
                                     n_gqty = (q2.n_qty.HasValue ? q2.n_qty.Value : 0)
                                   }) on new { q.c_no, q.c_iteno } equals new { q3.c_no, q3.c_iteno } into Left
                                from gLeft in Left.DefaultIfEmpty()
                                select new LG_ClosePLConfirm()
                                {
                                  c_plno = q.c_no,
                                  c_iteno = q.c_iteno,
                                  c_spno = q.c_noref,
                                  c_type = q.c_type,
                                  c_batch = q.c_batch,
                                  n_qty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty)
                                }).ToList();

              lstClosePLConf = lstClosePLConf.Where(x => x.n_qty > 0).ToList();

              Transaksi.Clear();

              //Delivery Ordert

              tmpS = (from q in db.LG_CloseDOs
                      where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                      select new LG_TRANSAKSI()
                      {
                        c_no = q.c_dono
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              tmpS = (from q in db.LG_DOHs
                      where q.d_dodate.Value.Year == Tahun && q.d_dodate.Value.Month == Bulan
                      select new LG_TRANSAKSI()
                      {
                        c_no = q.c_dono
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              lstCloseDO = (from q in Transaksi
                            join q3 in
                              (from q1 in db.LG_ExpHs
                               join q2 in db.LG_ExpDs on q1.c_expno equals q2.c_expno
                               where q1.d_expdate.Value.Year == Tahun && q1.d_expdate.Value.Month == Bulan
                               && q1.c_type == "01"
                               select new LG_TRANSAKSI()
                               {
                                 c_no = q2.c_dono
                               }) on q.c_no equals q3.c_no into Left
                            from gLeft in Left.DefaultIfEmpty()
                            where gLeft == null
                            select new LG_CloseDO()
                            {
                              c_dono = q.c_no
                            }).ToList();

              Transaksi.Clear();

              //SPG

              tmpS = (from q in db.LG_CloseSPGs
                      where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                      select new LG_TRANSAKSI()
                      {
                        c_gdg = q.c_gdg,
                        c_no = q.c_spgno,
                        c_iteno = q.c_iteno,
                        n_gqty = Convert.ToDecimal(q.n_qty)
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              tmpS = (from q in db.LG_SPGHs
                      join q1 in db.LG_SPGD1s on q.c_spgno equals q1.c_spgno
                      where q.d_spgdate.Value.Year == Tahun && q.d_spgdate.Value.Month == Bulan
                      select new LG_TRANSAKSI()
                      {
                        c_gdg = q.c_gdg1,
                        c_no = q.c_spgno,
                        c_iteno = q1.c_iteno,
                        n_gqty = q1.n_qty.HasValue ? q1.n_qty.Value : 0m
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              lstCloseSPG = (from q in Transaksi
                             join q3 in
                               (from q1 in db.LG_SJHs
                                join q2 in db.LG_SJD1s on new { q1.c_gdg, q1.c_sjno } equals new { q2.c_gdg, q2.c_sjno }
                                where q1.d_sjdate.Value.Year == Tahun && q1.d_sjdate.Value.Month == Bulan
                                group new { q1, q2 } by new { q1.c_gdg2, q2.c_spgno, q2.c_iteno } into g
                                select new LG_TRANSAKSI()
                                {
                                  c_gdg = g.Key.c_gdg2.HasValue ? g.Key.c_gdg2.Value : '2',
                                  c_no = g.Key.c_spgno,
                                  c_iteno = g.Key.c_iteno,
                                  n_gqty = g.Sum(x => x.q2.n_gqty.HasValue ? x.q2.n_gqty.Value : 0)
                                }) on new { q.c_gdg, q.c_no, q.c_iteno } equals new { q3.c_gdg, q3.c_no, q3.c_iteno } into Left
                             from gLeft in Left.DefaultIfEmpty()
                             select new LG_CloseSPG()
                             {
                               c_gdg = q.c_gdg,
                               c_spgno = q.c_no,
                               c_iteno = q.c_no,
                               n_qty = (q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty)).ToString()
                             }).ToList();

              lstCloseSPG = lstCloseSPG.Where(x => Convert.ToDecimal(x.n_qty) > 0).ToList();

              Transaksi.Clear();

              //SJ Exp

              tmpS = (from q in db.LG_CloseSJExps
                      where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                      select new LG_TRANSAKSI()
                      {
                        c_gdg = q.c_gdg,
                        c_no = q.c_sjno
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              tmpS = (from q in db.LG_SJHs
                      where q.d_sjdate.Value.Year == Tahun && q.d_sjdate.Value.Month == Bulan
                      select new LG_TRANSAKSI()
                      {
                        c_gdg = q.c_gdg,
                        c_no = q.c_sjno
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();


              lstCloseSJExp = (from q in Transaksi
                               join q3 in
                                 (from q1 in db.LG_ExpHs
                                  join q2 in db.LG_ExpDs on q1.c_expno equals q2.c_expno
                                  where q1.d_expdate.Value.Year == Tahun && q1.d_expdate.Value.Month == Bulan
                                  && q1.c_type == "02"
                                  select new LG_TRANSAKSI()
                                  {
                                    c_gdg = q1.c_gdg.HasValue ? q1.c_gdg.Value : '1',
                                    c_noref = q2.c_dono
                                  }) on new { q.c_gdg, q.c_no } equals new { q3.c_gdg, c_no = q3.c_noref } into Left
                               from gLeft in Left.DefaultIfEmpty()
                               where gLeft == null
                               select new LG_CloseSJExp()
                               {
                                 c_gdg = q.c_gdg,
                                 c_sjno = q.c_no
                               }).ToList();

              Transaksi.Clear();

              //SJ RN

              tmpS = (from q in db.LG_CloseSJRNs
                      where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                      select new LG_TRANSAKSI()
                      {
                        c_gdg = q.c_gdg,
                        c_no = q.c_sjno
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              tmpS = (from q in db.LG_SJHs
                      where q.d_sjdate.Value.Year == Tahun && q.d_sjdate.Value.Month == Bulan
                      select new LG_TRANSAKSI()
                      {
                        c_gdg = q.c_gdg,
                        c_no = q.c_sjno
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              var tt = (from q in db.LG_SJHs
                        where q.d_update.Value.Year == Tahun && q.d_update.Value.Month == Bulan
                        && q.l_status == false
                        select new
                        {
                          c_gdg = q.c_gdg,
                          c_no = q.c_sjno
                        }).ToList().Count();

              lstCloseSJRN = (from q in Transaksi
                              join q3 in
                                (from q in db.LG_SJHs
                                 where q.d_update.Value.Year == Tahun && q.d_update.Value.Month == Bulan
                                 && q.l_status == true
                                 select new LG_TRANSAKSI()
                                 {
                                   c_gdg = q.c_gdg,
                                   c_no = q.c_sjno
                                 }).ToList() on new { q.c_gdg, q.c_no } equals new { q3.c_gdg, q3.c_no } into Left
                              from gLeft in Left.DefaultIfEmpty()
                              where gLeft == null
                              select new LG_CloseSJRN()
                              {
                                c_gdg = q.c_gdg,
                                c_sjno = q.c_no
                              }).ToList();

              Transaksi.Clear();

              //Memo Combo

              tmpS = (from q in db.LG_CloseMemos
                      where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                      select new LG_TRANSAKSI()
                      {
                        c_gdg = q.c_gdg,
                        c_no = q.c_memono,
                        c_iteno = q.c_iteno,
                        n_gqty = q.n_qty.Value
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              tmpS = (from q in db.MK_MemoHs
                      join q1 in db.MK_MemoDs on new { q.c_gdg, q.c_memono } equals new { q1.c_gdg, q1.c_memono }
                      where q.d_memodate.Value.Year == Tahun && q.d_memodate.Value.Month == Bulan
                      select new LG_TRANSAKSI()
                      {
                        c_gdg = q.c_gdg,
                        c_no = q.c_memono,
                        c_iteno = q1.c_iteno,
                        n_gqty = q1.n_qty.Value
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              lstCloseMemo = (from q in Transaksi
                              join q3 in
                                (from q1 in db.LG_ComboHs
                                 where q1.d_combodate.Value.Year == Tahun && q1.d_combodate.Value.Month == Bulan
                                 select new LG_TRANSAKSI()
                                 {
                                   c_gdg = q1.c_gdg,
                                   c_noref = q1.c_memono,
                                   c_iteno = q1.c_iteno
                                 }) on new { q.c_gdg, q.c_no, q.c_iteno } equals new { q3.c_gdg, c_no = q3.c_noref, q3.c_iteno } into Left
                              from gLeft in Left.DefaultIfEmpty()
                              where gLeft == null
                              select new LG_CloseMemo()
                              {
                                c_gdg = q.c_gdg,
                                c_memono = q.c_no,
                                c_iteno = q.c_iteno,
                                n_qty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty)
                              }).ToList();

              lstCloseMemo = (from q in lstCloseMemo
                              join q3 in
                                (from q1 in db.LG_AdjComboHs
                                 join q2 in db.LG_AdjComboDs on new { q1.c_gdg, q1.c_adjno } equals new { q2.c_gdg, q2.c_adjno }
                                 where q1.d_adjdate.Value.Year == Tahun && q1.d_adjdate.Value.Month == Bulan
                                 group q2 by new { q1.c_gdg, q2.c_memono, q2.c_iteno } into g
                                 select new LG_TRANSAKSI()
                                 {
                                   c_gdg = g.Key.c_gdg,
                                   c_noref = g.Key.c_memono,
                                   c_iteno = g.Key.c_iteno,
                                   n_gqty = g.Sum(x => (x.n_qty.HasValue ? x.n_qty.Value : 0m))
                                 }) on new { q.c_gdg, q.c_memono } equals new { q3.c_gdg, c_memono = q3.c_noref } into Left
                              from gLeft in Left.DefaultIfEmpty()
                              select new LG_CloseMemo()
                              {
                                c_gdg = q.c_gdg,
                                c_memono = q.c_memono,
                                c_iteno = q.c_iteno,
                                n_qty = q.n_qty - (gLeft == null ? 0m : gLeft.n_gqty)
                              }).ToList();

              lstCloseMemo = lstCloseMemo.Where(x => x.n_qty > 0).ToList();

              Transaksi.Clear();

              //Donasi

              tmpS = (from q in db.LG_CloseMTs
                      where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                      select new LG_TRANSAKSI()
                      {
                        c_gdg = q.c_gdg,
                        c_no = q.c_mtno,
                        c_iteno = q.c_iteno,
                        n_gqty = q.n_qty.Value
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              tmpS = (from q in db.MK_MTHs
                      join q1 in db.MK_MTDs on new { q.c_gdg, q.c_mtno } equals new { q1.c_gdg, q1.c_mtno }
                      where q.d_mtdate.Value.Year == Tahun && q.d_mtdate.Value.Month == Bulan
                      select new LG_TRANSAKSI()
                      {
                        c_gdg = q.c_gdg,
                        c_no = q.c_mtno,
                        c_iteno = q1.c_iteno,
                        n_gqty = q1.n_qty.Value
                      }).ToList();

              Transaksi.AddRange(tmpS);
              tmpS.Clear();

              lstCloseMT = (from q in Transaksi
                            join q3 in
                              (from q1 in db.LG_STHs
                               join q2 in db.LG_STD1s on new { q1.c_gdg, q1.c_stno } equals new { q2.c_gdg, q2.c_stno }
                               where q1.d_stdate.Value.Year == Tahun && q1.d_stdate.Value.Month == Bulan
                               group new { q1, q2 } by new { q1.c_gdg, q1.c_mtno, q2.c_iteno } into g
                               select new LG_TRANSAKSI()
                               {
                                 c_gdg = g.Key.c_gdg,
                                 c_noref = g.Key.c_mtno,
                                 c_iteno = g.Key.c_iteno,
                                 n_gqty = g.Sum(x => x.q2.n_qty.HasValue ? x.q2.n_qty.Value : 0)
                               }) on new { q.c_gdg, q.c_no, q.c_iteno } equals new { q3.c_gdg, c_no = q3.c_noref, q3.c_iteno } into Left
                            from gLeft in Left.DefaultIfEmpty()
                            select new LG_CloseMT()
                            {
                              c_gdg = q.c_gdg,
                              c_mtno = q.c_no,
                              c_iteno = q.c_iteno,
                              n_qty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty)
                            }).ToList();

              lstCloseMT = (from q in lstCloseMT
                            join q3 in
                              (from q1 in db.LG_AdjMTHs
                               join q2 in db.LG_AdjMTDs on new { q1.c_gdg, q1.c_adjno } equals new { q2.c_gdg, q2.c_adjno }
                               where q1.d_adjdate.Value.Year == Tahun && q1.d_adjdate.Value.Month == Bulan
                               group q2 by new { q1.c_gdg, q2.c_mtno, q2.c_iteno } into g
                               select new LG_TRANSAKSI()
                               {
                                 c_gdg = g.Key.c_gdg,
                                 c_noref = g.Key.c_mtno,
                                 c_iteno = g.Key.c_iteno,
                                 n_gqty = g.Sum(x => (x.n_qty.HasValue ? x.n_qty.Value : 0m))
                               }) on new { q.c_gdg, q.c_mtno, q.c_iteno } equals new { q3.c_gdg, c_mtno = q3.c_no, q3.c_iteno } into Left
                            from gLeft in Left.DefaultIfEmpty()
                            select new LG_CloseMT()
                            {
                              c_gdg = q.c_gdg,
                              c_mtno = q.c_mtno,
                              c_iteno = q.c_iteno,
                              n_qty = q.n_qty - (gLeft == null ? 0m : gLeft.n_gqty)
                            }).ToList();

              lstCloseMT = lstCloseMT.Where(x => x.n_qty > 0).ToList();

              Transaksi.Clear();

              #endregion
            }
            break;

          #endregion
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          "ScmsSoaLibrary.Modules.CommonQueryProcess:ModelGridQueryStoreProcedure <-> Switch {0} - {1}", model, ex.Message);
        Logger.WriteLine(ex.StackTrace);

        dic.Add(Constant.DEFAULT_NAMING_EXCEPTION, ex.Message);

        dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);
      }

      if (listPCSPR != null)
      {
        listPCSPR.Clear();
      }

      db.Dispose();

      return dic;
    }
  }
}