using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq.SqlClient;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsModel;
using ScmsModel.Core;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaLibrary.Core.Reports
{
  class ReportDatasetBind
  {
    public bool IsError
    { get; private set; }

    public string ErrorMessage
    { get; private set; }
    
    #region DSNG_SCMS

    internal class DSNG_SCMS_QuerySales
    {
      public string c_fjno { get; set; }
      public string d_fjdate { get; set; }
      public DateTime date_fjdate { get; set; }
      public string c_sektor { get; set; }
      public string c_cusno { get; set; }
      public string v_cunam { get; set; }
      public string c_iteno { get; set; }
      public decimal n_qty { get; set; }
      public decimal n_salpri { get; set; }
      public decimal n_disc { get; set; }
      public string c_kddivams { get; set; }
      public string v_nmdivams { get; set; }
      public bool l_clinic { get; set; }
      public bool l_cabang { get; set; }
      public string v_ket { get; set; }

    }

    internal class DSNG_SCMS_PurchasePerFB
    {
      public string c_rnno { get; set; }
      public DateTime d_rndate { get; set; }
      public DateTime d_fbdate { get; set; }
      public string c_fbno { get; set; }
      public string c_fb { get; set; }
      public string c_taxno { get; set; }
      public string c_iteno { get; set; }
      public string c_type { get; set; }
      public string c_nosup { get; set; }
      public string v_nama { get; set; }
      public string v_nmdivams { get; set; }
      public string c_kddivams { get; set; }      
      public decimal n_bruto { get; set; }
      public decimal bonus { get; set; }
      public decimal n_disc { get; set; }
      public decimal n_qty { get; set; }
      public decimal n_salpri { get; set; }
      public decimal n_xdisc { get; set; }
      public decimal dpp { get; set; }
      public decimal n_ppn { get; set; }
      public decimal n_bea { get; set; }
      public decimal invt { get; set; }
      public decimal bilva { get; set; }
    }

    internal class DSNG_SCMS_QueryPurchase
    {
      public string c_rnno { get; set; }
      public DateTime d_rndate { get; set; }
      public DateTime d_fbdate { get; set; }
      public string c_fbno { get; set; }
      public string c_fb { get; set; }
      public string c_taxno { get; set; }
      public string c_iteno { get; set; }
      public string c_type { get; set; }
      public string c_nosup { get; set; }
      public string v_nama { get; set; }
      public string v_nmdivams { get; set; }
      public string c_kddivams { get; set; }
      public decimal n_bruto { get; set; }
      public decimal bonus { get; set; }
      public decimal n_disc { get; set; }
      public decimal n_qty { get; set; }
      public decimal n_salpri { get; set; }
      public decimal n_xdisc { get; set; }
      public decimal dpp { get; set; }
      public decimal n_ppn { get; set; }
      public decimal n_bea { get; set; }
      public decimal invt { get; set; }
      public decimal bilva { get; set; }
    }

    internal class DSNG_SCMS_QueryReturBeli
    {
      public DateTime d_fbdate { get; set; }
      public string c_fbno { get; set; }
      public string c_taxno { get; set; }
      public string c_iteno { get; set; }
      public string c_type { get; set; }
      public string c_nosup { get; set; }
      public string v_nama { get; set; }
      public string v_nmdivams { get; set; }
      public string c_kddivams { get; set; }
      public decimal bruto { get; set; }
      public decimal bonus { get; set; }
      public decimal DiscVal { get; set; }
      public decimal n_qty { get; set; }
      public decimal n_salpri { get; set; }
      public decimal n_xdisc { get; set; }
      public decimal dpp { get; set; }
      public decimal n_ppn { get; set; }
      public decimal n_bea { get; set; }
      public decimal Invt { get; set; }
      public decimal bilva { get; set; }
    }

    internal class DSNG_SCMS_QueryReturJual
    {
      public DateTime d_fjdate { get; set; }
      public string c_fjno { get; set; }
      public DateTime d_taxdate { get; set; }
      public string c_taxno { get; set; }
      public string c_sektor { get; set; }
      public string v_ket { get; set; }
      public string c_iteno { get; set; }
      public string c_type { get; set; }
      public string c_cusno { get; set; }
      public string v_cunam { get; set; }
      public string v_nmdivams { get; set; }
      public string c_kddivams { get; set; }
      public decimal bruto { get; set; }
      public decimal bonus { get; set; }
      public decimal DiscVal { get; set; }
      public decimal n_qty { get; set; }
      public decimal n_salpri { get; set; }
      public decimal n_xdisc { get; set; }
      public decimal dpp { get; set; }
      public decimal n_ppn { get; set; }
      public decimal n_bea { get; set; }
      public decimal Invt { get; set; }
      public decimal bilva { get; set; }
    }

    internal class DSNG_SCMS_Claim
    {
      public char c_gdg { get; set; }
      public string c_rnno { get; set; }
      public string c_claimno { get; set; }
      public string c_no { get; set; }
      public string c_claimaccno { get; set; }
      public DateTime d_rndate { get; set; }
      public string c_from { get; set; }
      public string v_nama { get; set; }
      public string v_itnam { get; set; }
      public string c_kddivpri { get; set; }
      public string v_nmdivpri { get; set; }
      public string c_kddivams { get; set; }
      public string v_nmdivams { get; set; }
      public string c_iteno { get; set; }
      public decimal n_salpri { get; set; }
      public decimal n_disc { get; set; }
      public decimal n_qtyClaim { get; set; }
      public decimal n_qty { get; set; }
    }

    internal class DSNG_SCMS_SalKre
    {
      public string c_fbno { get; set; }
      public DateTime d_fbdate { get; set; }
      public string v_nama { get; set; }
      public string c_nosup { get; set; }
      public decimal n_sisa { get; set; }
      public decimal n_awal { get; set; }
      public decimal n_beli { get; set; }
      public decimal n_retur { get; set; }
      public string c_fbnoDN { get; set; }
      public decimal n_bayarDN { get; set; }
      public decimal n_bayar_returDN { get; set; }
      public string c_typeDN { get; set; }
      public string c_typeAJ { get; set; }
      public decimal n_adjustAJ { get; set; }
      public decimal n_1_30 { get; set; }
      public decimal n_31_37 { get; set; }
      public decimal n_38_45 { get; set; }
      public decimal n_46_60 { get; set; }
      public decimal n_61_90 { get; set; }
      public decimal n_91_120 { get; set; }
      public decimal n_120 { get; set; }
      public string c_fbnoAJ { get; set; }
    }

    internal class DSNG_SCMS_SalDeb
    {
      public string sektor { get; set; }
      public string c_fjno { get; set; }
      public DateTime d_fjdate { get; set; }
      public string v_cunam { get; set; }
      public string c_cusno { get; set; }
      public decimal n_sisa { get; set; }
      public decimal n_awal { get; set; }
      public decimal n_beli { get; set; }
      public decimal n_retur { get; set; }
      public string c_fjnoDN { get; set; }
      public decimal n_bayar { get; set; }
      public decimal n_bayar_retur { get; set; }
      public string c_typeDN { get; set; }
      public string c_typeAJ { get; set; }
      public decimal n_adjust { get; set; }
      public decimal n_1_30 { get; set; }
      public decimal n_31_37 { get; set; }
      public decimal n_38_45 { get; set; }
      public decimal n_46_60 { get; set; }
      public decimal n_61_90 { get; set; }
      public decimal n_91_120 { get; set; }
      public decimal n_120 { get; set; }
      public string c_fjnoAJ { get; set; }
    }

    internal class DSNG_SCMS_BEA
    {
      public string c_fbno { get; set; }
      public decimal n_bea { get; set; }
      public string c_iteno { get; set; }
      public string c_nosup { get; set; }
      public string v_nama { get; set; }
      public string c_kddivams { get; set; }
      public string v_nmdivams { get; set; }
      public string v_itnam { get; set; }
      public string v_undes { get; set; }
      public decimal n_salpri { get; set; }
    }

    internal class DSNG_SCMS_FLOATING
    {
      public char c_gdg { get; set; }
      public string c_rnno { get; set; }
      public string c_from { get; set; }
      public string v_nama { get; set; }
      public DateTime d_rndate { get; set; }
      public string c_dono { get; set; }
      public DateTime d_dodate { get; set; }
      public string c_iteno { get; set; }
      public string v_itnam { get; set; }
      public string v_undes { get; set; }
      public decimal n_bqty { get; set; }
      public decimal n_gqty { get; set; }
    }

    internal class DSNG_SCMS_KARTU_BARANG
    {
      public char c_gdg { get; set; }
      public string c_iteno { get; set; }
      public string c_batch { get; set; }
      public decimal n_gawal { get; set; }
      public decimal n_bawal { get; set; }
      public string c_no { get; set; }
      public DateTime d_date { get; set; }
      public decimal n_gqty { get; set; }
      public decimal n_bqty { get; set; }
      public string c_cusno { get; set; }
      public string v_cunam { get; set; }
      public string item_undes { get; set; }
      public string c_nosup { get; set; }
      public string v_nama { get; set; }
      public string c_kddivpri { get; set; }
      public string v_nmdivpri { get; set; }
      public string c_kddivams { get; set; }
      public string v_nmdivams { get; set; }
      public string v_item_nama { get; set; }
      public string v_gdgdesc { get; set; }
      public DateTime d_expired { get; set; }
      public string c_user { get; set; }
    }

    internal class DSNG_SCMS_LGPBF
    {
      public char c_gdg { get; set; }
      public string c_iteno { get; set; }
      public string v_itnam { get; set; }
      public string c_alkes { get; set; }
      public decimal n_awal { get; set; }
      public decimal n_inPabrik { get; set; }
      public decimal n_inpbf { get; set; }
      public decimal n_inLain { get; set; }
      public decimal n_OutRS { get; set; }
      public decimal n_outApotik { get; set; }
      public decimal n_outPBF { get; set; }
      public decimal n_outLain { get; set; }
      public decimal n_outPemerintah { get; set; }
      public decimal n_outSwasta { get; set; }
      public decimal n_pbf { get; set; }
      public decimal n_het { get; set; }
      public decimal n_hna { get; set; }
      public string c_entry { get; set; }
      public string d_awal { get; set; }
      public string d_akhir { get; set; }
      public string Periode { get; set; }
    }

    internal class Temp_LGHistorySP_Test
    {
      public string c_spno { get; set; }
      public DateTime? d_spdate { get; set; }
      public string c_sp { get; set; }
      public string c_cusno { get; set; }
      public string c_iteno { get; set; }
      public decimal? n_beliqty1 { get; set; }
      public decimal? n_bonusqty1 { get; set; }
      public decimal? n_beliacc { get; set; }
      public decimal? n_bonusacc { get; set; }
      public string c_gdg { get; set; }
      public string c_no { get; set; }
      public DateTime? d_date { get; set; }
      public bool? l_confirm { get; set; }
      public DateTime? d_update { get; set; }
      public decimal? n_beliqty2 { get; set; }
      public decimal? n_bonusqty2 { get; set; }
      public string c_user { get; set; }
      public string c_dono { get; set; }
      public DateTime? d_dodate { get; set; }
      public string c_resi { get; set; }
      public DateTime? d_resi { get; set; }
      public string v_itnam { get; set; }
      public string v_expdesc { get; set; }
      public string v_cunam { get; set; }
      public string v_gdgdesc { get; set; }
      public string c_nosup { get; set; }
      public string v_namasup { get; set; }
      public string c_kddivpri { get; set; }
      public string v_nmdivpri { get; set; }
      public DateTime? d_spinsert { get; set; }
      public string c_via { get; set; }
      public string v_ket { get; set; }
      public decimal? n_sisa_spd1 { get; set; }
      public string v_ket_spd1 { get; set; }
      public string v_ket_plh { get; set; }
      public decimal? n_waktu_a { get; set; }
      public decimal? n_waktu_b { get; set; }

    }

    #endregion

    #region Class Helper

    internal class LG_KB_PROCESS
    {
      public char c_gdg { get; set; }
      public string c_no { get; set; }
      public DateTime d_date { get; set; }
      public string c_iteno { get; set; }
      public string c_batch { get; set; }
      public decimal n_gqty { get; set; }
      public decimal n_bqty { get; set; }
      public decimal n_gawal { get; set; }
      public decimal n_bawal { get; set; }
      public string c_cusno { get; set; }
    }

    internal class CLASS_TEMP_AWAL
    {
      public char c_gdg { get; set; }
      public string c_iteno { get; set; }
      public string c_batch { get; set; }
      public decimal n_gawal { get; set; }
      public decimal n_bawal { get; set; }
    }

    internal class CLASS_TEMP_TRANSKSI
    {
      public char c_gdg { get; set; }
      public string c_iteno { get; set; }
      public decimal n_gqty { get; set; }
      public decimal n_bqty { get; set; }
      public decimal n_gawal { get; set; }
      public decimal n_bawal { get; set; }
      public string c_add1 { get; set; }
    }

    internal class CLASS_TEMP_STOCK
    {
      public char c_gdg { get; set; }
      public string c_iteno { get; set; }
      public decimal n_gawal { get; set; }
      public decimal n_bawal { get; set; }
      public decimal n_grnbeli { get; set; }
      public decimal n_brnbeli { get; set; }
      public decimal n_grnbonus { get; set; }
      public decimal n_brnbonus { get; set; }
      public decimal n_grnrs { get; set; }
      public decimal n_brnrs { get; set; }
      public decimal n_grnclaim { get; set; }
      public decimal n_brnclaim { get; set; }
      public decimal n_grnrepack { get; set; }
      public decimal n_brnrepack { get; set; }
      public decimal n_grngdg { get; set; }
      public decimal n_brngdg { get; set; }
      public decimal n_grc { get; set; }
      public decimal n_brc { get; set; }
      public decimal n_combo { get; set; }
      public decimal n_grsbeli { get; set; }
      public decimal n_brsbeli { get; set; }
      public decimal n_grsrepack { get; set; }
      public decimal n_brsrepack { get; set; }
      public decimal n_gsj { get; set; }
      public decimal n_bsj { get; set; }
      public decimal n_gsjclaim { get; set; }
      public decimal n_bsjclaim { get; set; }
      public decimal n_pl { get; set; }
      public decimal n_stt { get; set; }
      public decimal n_combod { get; set; }
      public decimal n_gadj { get; set; }
      public decimal n_badj { get; set; }
    }

    internal class CLASS_MASTER_NAME
    {
      public string KeyID { get; set; }
      public string CodeID { get; set; }
      public string DescName { get; set; }
      public string Custom1 { get; set; }
    }

    internal class CLASS_SCMS_KARTU_BARANG_TEMP
    {
      public char c_gdg { get; set; }
      public string c_iteno { get; set; }
      public string c_batch { get; set; }
      public decimal n_gawal { get; set; }
      public decimal n_bawal { get; set; }
      public string c_no { get; set; }
      public DateTime d_date { get; set; }
      public decimal n_gqty { get; set; }
      public decimal n_bqty { get; set; }
      public string c_cusno { get; set; }
      //public string item_undes { get; set; }
      //public string c_nosup { get; set; }
      //public string v_nama { get; set; }
      //public string c_kddivpri { get; set; }
      //public string v_nmdivpri { get; set; }
      //public string v_item_nama { get; set; }
      //public string v_gdgdesc { get; set; }
      //public string c_user { get; set; }
      public int n_count { get; set; }
    }

    internal class DSNG_SCMS_PEMBAYARAN
    {
      public string c_nosup { get; set; }
      public string v_nama { get; set; }
      public string c_fb { get; set; }
      public string c_fbno { get; set; }
      public DateTime d_fbdate { get; set; }
      public decimal n_bilva { get; set; }
      public DateTime d_top { get; set; }
      public decimal n_top { get; set; }
      public decimal n_sisa { get; set; }
      public string c_notenoVD { get; set; }
      public DateTime d_notedateVD { get; set; }
      public decimal n_bilvaVD { get; set; }
      public string c_vdnoVD { get; set; }
      public string c_fbnoVD { get; set; }
      public decimal n_valueVD { get; set; }
    }
	
	  internal class BatchItemData
    {
      public string c_iteno { get; set; }
      public string c_batch { get; set; }
      public DateTime d_expired { get; set; }
    }

    internal class BatchItemGetData
    {
      public string c_iteno { get; set; }
      public string c_batch { get; set; }
    }

    #endregion

    #region Comparer Helper

    internal class KartuBarangEqualKey
    {
      public char c_gdg { get; set; }
      public string c_iteno { get; set; }
      public string c_batch { get; set; }
    }

    internal class KartuBarangEqualComparer : IEqualityComparer<KartuBarangEqualKey>
    {
      #region IEqualityComparer<KartuBarangEqualKey> Members

      public bool Equals(KartuBarangEqualKey x, KartuBarangEqualKey y)
      {
        return ((x == null) || (y == null) ? false :
          x.c_gdg.Equals(y.c_gdg) &&
          (string.IsNullOrEmpty(x.c_iteno) ? string.Empty : x.c_iteno.Trim()).Equals((string.IsNullOrEmpty(y.c_iteno) ? string.Empty : y.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
          (string.IsNullOrEmpty(x.c_batch) ? string.Empty : x.c_batch.Trim()).Equals((string.IsNullOrEmpty(y.c_batch) ? string.Empty : y.c_batch.Trim()), StringComparison.OrdinalIgnoreCase));
      }

      public int GetHashCode(KartuBarangEqualKey obj)
      {
        return obj.GetHashCode();
      }

      #endregion
    }

    #endregion

    #region Enum Modes

    public enum ModeKodeBarangSwitcher
    {
      KartuBarangBatch = 1,
      KartuBarangTotal = 2
    }

    #endregion

    public void TestCode(ORMDataContext db)
    {
      List<LG_KB_PROCESS> listStokAwal = new List<LG_KB_PROCESS>(),
        listSATemp = null,
        listSAProsesAwal = null,
        listSAProsesJalan = new List<LG_KB_PROCESS>(),
        listSAProsesJalan1 = null;

      List<DSNG_SCMS_LGPBF> listTempPBF = null,
        listPBF = null;

      LG_KB_PROCESS kbp = null;
      CLASS_TEMP_TRANSKSI ttrx = null;
      CLASS_TEMP_STOCK ts = null;

      List<CLASS_TEMP_STOCK> lstTempStock = null;

      List<CLASS_TEMP_TRANSKSI> lstTempTrx = null;

      List<string> lstTemp = new List<string>();

      char gudang = '1';
      string tipePeriode = "01";
      int thnAktif = 2012;

      string tmp1 = null,
        tmp2 = null;

      DateTime date1 = DateTime.MinValue,
        date2 = DateTime.MinValue,
        dateX = DateTime.MinValue,
        datePrev1 = DateTime.MinValue,
        datePrev2 = DateTime.MinValue;

      date1 = new DateTime(thnAktif,
        (tipePeriode.Equals("01") ? 1 :
          (tipePeriode.Equals("02") ? 4 :
            (tipePeriode.Equals("03") ? 7 : 10))),
            1);
      date2 = date1.AddMonths(3);

      dateX = date1.AddMonths(-1);
      datePrev1 = new DateTime(date1.Year, date1.Month, 1);
      datePrev2 = date1.AddDays(-1);

      var qryItemsAktif = (from q in db.FA_MasItms
                           where ((q.l_aktif.HasValue ? q.l_aktif.Value : false) == true)
                             //&& ((q.l_hide.HasValue ? q.l_hide.Value : false) == false)
                              && ((q.l_dinkes.HasValue ? q.l_dinkes.Value : false) == true)
                            && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                           select q.c_iteno).AsQueryable();
      #region Awal

      // Stok Awal
      listSATemp = (from q in db.LG_Stocks
                    where (q.c_gdg == gudang)
                      && (q.s_tahun == dateX.Year) && (q.t_bulan == dateX.Month)
                      && qryItemsAktif.Contains(q.c_iteno)
                    group q by new { q.c_gdg, q.c_no, q.c_iteno } into g
                    select new LG_KB_PROCESS()
                    {
                      c_gdg = g.Key.c_gdg,
                      c_no = g.Key.c_no,
                      d_date = Functionals.StandardSqlDateTime,
                      c_iteno = g.Key.c_iteno,
                      n_gqty = g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
                      n_bqty = g.Sum(t => (t.n_bqty.HasValue ? t.n_bqty.Value : 0)),
                    }).Distinct().ToList();
      listStokAwal.AddRange(listSATemp.ToArray());
      listSATemp.Clear();

      #region Old Coded

      //// RN
      //listSATemp = (from q in db.LG_RNHs
      //              join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
      //              where (q.c_gdg == gudang) && (q.c_type != "05")
      //                && ((q.d_rndate >= datePrev1) && (q.d_rndate <= datePrev2))
      //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
      //                && qryItemsAktif.Contains(q1.c_iteno)
      //              group q1 by new { q.c_gdg, q.c_rnno, q.d_rndate, q1.c_iteno } into g
      //              select new LG_KB_PROCESS()
      //              {
      //                c_gdg = g.Key.c_gdg,
      //                c_no = g.Key.c_rnno,
      //                d_date = (g.Key.d_rndate.HasValue ? g.Key.d_rndate.Value : Functionals.StandardSqlDateTime),
      //                c_iteno = g.Key.c_iteno,
      //                n_gqty = g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
      //                n_bqty = g.Sum(t => (t.n_bqty.HasValue ? t.n_bqty.Value : 0)),
      //              }).Distinct().ToList();
      //listStokAwal.AddRange(listSATemp.ToArray());
      //listSATemp.Clear();

      //// RN Gudang
      //listSATemp = (from q in db.LG_SJHs
      //              join q1 in db.LG_SJD2s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
      //              where (q.c_gdg2 == gudang) && (q.l_status == true)
      //                && ((q.d_update >= datePrev1) && (q.d_update <= datePrev2))                      
      //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
      //                && qryItemsAktif.Contains(q1.c_iteno)
      //              group q1 by new { q.c_gdg2, q.c_sjno, q.d_update, q1.c_iteno } into g
      //              select new LG_KB_PROCESS()
      //              {
      //                c_gdg = (g.Key.c_gdg2.HasValue ? g.Key.c_gdg2.Value : '0'),
      //                c_no = g.Key.c_sjno,
      //                d_date = (g.Key.d_update.HasValue ? g.Key.d_update.Value : Functionals.StandardSqlDateTime),
      //                c_iteno = g.Key.c_iteno,
      //                n_gqty = g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
      //                n_bqty = g.Sum(t => (t.n_bqty.HasValue ? t.n_bqty.Value : 0)),
      //              }).Distinct().ToList();
      //listStokAwal.AddRange(listSATemp.ToArray());
      //listSATemp.Clear();

      //// Combo
      //listSATemp = (from q in db.LG_ComboHs
      //              where (q.c_gdg == gudang) && (q.c_type == "01") 
      //                && ((q.l_confirm.HasValue ? q.l_confirm.Value : false) == true)
      //                && ((q.d_combodate >= datePrev1) && (q.d_combodate <= datePrev2))
      //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
      //                && qryItemsAktif.Contains(q.c_iteno)
      //              group q by new { q.c_gdg, q.c_combono, q.d_combodate, q.c_iteno } into g
      //              select new LG_KB_PROCESS()
      //              {
      //                c_gdg = g.Key.c_gdg,
      //                c_no = g.Key.c_combono,
      //                d_date = (g.Key.d_combodate.HasValue ? g.Key.d_combodate.Value : Functionals.StandardSqlDateTime),
      //                c_iteno = g.Key.c_iteno,
      //                n_gqty = g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
      //                n_bqty = g.Sum(t => (t.n_bqty.HasValue ? t.n_bqty.Value : 0)),
      //              }).Distinct().ToList();
      //listStokAwal.AddRange(listSATemp.ToArray());
      //listSATemp.Clear();

      //// RC
      //listSATemp = (from q in db.LG_RCHes
      //              join q1 in db.LG_RCD1s on new { q.c_gdg, q.c_rcno } equals new { q1.c_gdg, q1.c_rcno }
      //              where (q.c_gdg == gudang)
      //                && ((q.d_rcdate >= datePrev1) && (q.d_rcdate <= datePrev2))
      //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
      //                && qryItemsAktif.Contains(q1.c_iteno)
      //              group q1 by new { q.c_gdg, q.c_rcno, q.d_rcdate, q1.c_iteno } into g
      //              select new LG_KB_PROCESS()
      //              {
      //                c_gdg = g.Key.c_gdg,
      //                c_no = g.Key.c_rcno,
      //                d_date = (g.Key.d_rcdate.HasValue ? g.Key.d_rcdate.Value : Functionals.StandardSqlDateTime),
      //                c_iteno = g.Key.c_iteno,
      //                n_gqty = g.Sum(t => (t.c_type == "01" ? (t.n_qty.HasValue ? t.n_qty.Value : 0) : 0)),
      //                n_bqty = g.Sum(t => (t.c_type != "01" ? (t.n_qty.HasValue ? t.n_qty.Value : 0) : 0)),
      //              }).Distinct().ToList();
      //listStokAwal.AddRange(listSATemp.ToArray());
      //listSATemp.Clear();

      //// RS
      //listSATemp = (from q in db.LG_RSHes
      //              join q1 in db.LG_RSD1s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
      //              where (q.c_gdg == gudang) 
      //                && ((q.d_rsdate >= datePrev1) && (q.d_rsdate <= datePrev2))
      //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
      //                && qryItemsAktif.Contains(q1.c_iteno)
      //              group q1 by new { q.c_gdg, q.c_rsno, q.d_rsdate, q1.c_iteno } into g
      //              select new LG_KB_PROCESS()
      //              {
      //                c_gdg = g.Key.c_gdg,
      //                c_no = g.Key.c_rsno,
      //                d_date = (g.Key.d_rsdate.HasValue ? g.Key.d_rsdate.Value : Functionals.StandardSqlDateTime),
      //                c_iteno = g.Key.c_iteno,
      //                n_gqty = -g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
      //                n_bqty = -g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
      //              }).Distinct().ToList();
      //listStokAwal.AddRange(listSATemp.ToArray());
      //listSATemp.Clear();
      
      //// SJ
      //listSATemp = (from q in db.LG_SJHs
      //              join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
      //              where (q.c_gdg == gudang)
      //                 && ((q.d_sjdate >= datePrev1) && (q.d_sjdate <= datePrev2))
      //                 && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
      //                 && qryItemsAktif.Contains(q1.c_iteno)
      //              group q1 by new { q.c_gdg, q.c_sjno, q.d_sjdate, q1.c_iteno } into g
      //              select new LG_KB_PROCESS()
      //              {
      //                c_gdg = g.Key.c_gdg,
      //                c_no = g.Key.c_sjno,
      //                d_date = (g.Key.d_sjdate.HasValue ? g.Key.d_sjdate.Value : Functionals.StandardSqlDateTime),
      //                c_iteno = g.Key.c_iteno,
      //                n_gqty = -g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
      //                n_bqty = -g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
      //              }).Distinct().ToList();
      //listStokAwal.AddRange(listSATemp.ToArray());
      //listSATemp.Clear();

      //// PL
      //listSATemp = (from q in db.LG_PLHs
      //              join q1 in db.LG_PLD1s on q.c_plno equals q1.c_plno
      //              where (q.c_gdg == gudang) && (q.l_confirm == true)
      //                && ((q.d_pldate >= datePrev1) && (q.d_pldate <= datePrev2))
      //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
      //                && qryItemsAktif.Contains(q1.c_iteno)
      //              group q1 by new { q.c_gdg, q.c_plno, q.d_pldate, q1.c_iteno } into g
      //              select new LG_KB_PROCESS()
      //              {
      //                c_gdg = (g.Key.c_gdg.HasValue ? g.Key.c_gdg.Value : '0'),
      //                c_no = g.Key.c_plno,
      //                d_date = (g.Key.d_pldate.HasValue ? g.Key.d_pldate.Value : Functionals.StandardSqlDateTime),
      //                c_iteno = g.Key.c_iteno,
      //                n_gqty = -g.Sum(t => (t.n_qty.HasValue ? t.n_qty.Value : 0)),
      //                n_bqty = 0,
      //              }).Distinct().ToList();
      //listStokAwal.AddRange(listSATemp.ToArray());
      //listSATemp.Clear();

      //// STT
      //listSATemp = (from q in db.LG_STHs
      //              join q1 in db.LG_STD1s on new { q.c_gdg, q.c_stno } equals new { q1.c_gdg, q1.c_stno }
      //              where (q.c_gdg == gudang)
      //                && ((q.d_stdate >= datePrev1) && (q.d_stdate <= datePrev2))
      //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
      //                && qryItemsAktif.Contains(q1.c_iteno)
      //              group q1 by new { q.c_gdg, q.c_stno, q.d_stdate, q1.c_iteno } into g
      //              select new LG_KB_PROCESS()
      //              {
      //                c_gdg = g.Key.c_gdg,
      //                c_no = g.Key.c_stno,
      //                d_date = (g.Key.d_stdate.HasValue ? g.Key.d_stdate.Value : Functionals.StandardSqlDateTime),
      //                c_iteno = g.Key.c_iteno,
      //                n_gqty = -g.Sum(t => (t.n_qty.HasValue ? t.n_qty.Value : 0)),
      //                n_bqty = 0,
      //              }).Distinct().ToList();
      //listStokAwal.AddRange(listSATemp.ToArray());
      //listSATemp.Clear();

      //// Combo Detail
      //listSATemp = (from q in db.LG_ComboHs
      //              join q1 in db.LG_ComboD1s on new { q.c_gdg, q.c_combono } equals new { q1.c_gdg, q1.c_combono }
      //              where (q.c_gdg == gudang) && (q.l_confirm == true)
      //                && ((q.d_combodate >= datePrev1) && (q.d_combodate <= datePrev2))
      //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
      //                && qryItemsAktif.Contains(q1.c_iteno)
      //              group q1 by new { q.c_gdg, q.c_combono, q.d_combodate, q1.c_iteno, q1.c_batch } into g
      //              select new LG_KB_PROCESS()
      //              {
      //                c_gdg = g.Key.c_gdg,
      //                c_no = g.Key.c_combono,
      //                d_date = (g.Key.d_combodate.HasValue ? g.Key.d_combodate.Value : Functionals.StandardSqlDateTime),
      //                c_iteno = g.Key.c_iteno,
      //                n_gqty = -g.Sum(t => (t.n_qty.HasValue ? t.n_qty.Value : 0)),
      //                n_bqty = 0,
      //              }).Distinct().ToList();
      //listStokAwal.AddRange(listSATemp.ToArray());
      //listSATemp.Clear();

      //// Adjustment
      //listSATemp = (from q in db.LG_AdjustHs
      //              join q1 in db.LG_AdjustD1s on new { q.c_gdg, q.c_adjno } equals new { q1.c_gdg, q1.c_adjno }
      //              where (q.c_gdg == gudang)
      //                && ((q.d_adjdate >= datePrev1) && (q.d_adjdate <= datePrev2))
      //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
      //                && qryItemsAktif.Contains(q1.c_iteno)
      //              group q1 by new { q.c_gdg, q.c_adjno, q.d_adjdate, q1.c_iteno, q1.c_batch } into g
      //              select new LG_KB_PROCESS()
      //              {
      //                c_gdg = g.Key.c_gdg,
      //                c_no = g.Key.c_adjno,
      //                d_date = (g.Key.d_adjdate.HasValue ? g.Key.d_adjdate.Value : Functionals.StandardSqlDateTime),
      //                c_iteno = g.Key.c_iteno,
      //                c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
      //                n_gqty = g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
      //                n_bqty = g.Sum(t => (t.n_bqty.HasValue ? t.n_bqty.Value : 0)),
      //                c_cusno = string.Empty
      //              }).Distinct().ToList();
      //listStokAwal.AddRange(listSATemp.ToArray());
      //listSATemp.Clear();

      #endregion

      #endregion

      lstTempStock = listStokAwal.GroupBy(t => t.c_iteno)
        .Select(x => new CLASS_TEMP_STOCK()
        {
          c_gdg = gudang,
          c_iteno = x.Key,
          n_gawal = x.Sum(y => y.n_gqty),
          n_bawal = x.Sum(y => y.n_bqty)
        }).ToList();
      listStokAwal.Clear();

      #region Transksi

      #region RN Beli

      lstTemp.Add("01");
      lstTemp.Add("06");
      lstTempTrx = (from q in db.LG_RNHs
                    join q1 in db.LG_RND2s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                    where (q.c_gdg == gudang) && lstTemp.Contains(q.c_type)
                      && ((q.d_rndate >= date1) && (q.d_rndate < date2))
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && qryItemsAktif.Contains(q1.c_iteno)
                    group q1 by new { q1.c_iteno, q1.c_type } into g
                    select new CLASS_TEMP_TRANSKSI()
                    {
                      c_gdg = gudang,
                      c_iteno = (g.Key.c_iteno == null ? string.Empty : g.Key.c_iteno.Trim()),
                      c_add1 = (g.Key.c_type == null ? string.Empty : g.Key.c_type.Trim()),
                      n_gqty = g.Sum(y => (y.n_gqty.HasValue ? y.n_gqty.Value : 0)),
                      n_bqty = g.Sum(y => (y.n_bqty.HasValue ? y.n_bqty.Value : 0))
                    }).ToList();
      #region Populate
      lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
      {
        ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
        {
          return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
        });

        if (ts == null)
        {
          if (cttrx.c_add1.Equals("01", StringComparison.OrdinalIgnoreCase))
          {
            lstTempStock.Add(new CLASS_TEMP_STOCK()
            {
              c_gdg = gudang,
              c_iteno = cttrx.c_iteno,
              n_grnbeli = cttrx.n_gqty,
              n_brnbeli = cttrx.n_bqty,
              n_grnbonus = 0,
              n_brnbonus = 0,
            });
          }
          else if (cttrx.c_add1.Equals("02", StringComparison.OrdinalIgnoreCase))
          {
            lstTempStock.Add(new CLASS_TEMP_STOCK()
            {
              c_gdg = gudang,
              c_iteno = cttrx.c_iteno,
              n_grnbeli = 0,
              n_brnbeli = 0,
              n_grnbonus = cttrx.n_gqty,
              n_brnbonus = cttrx.n_bqty,
            });
          }
        }
        else
        {
          if (cttrx.c_add1.Equals("01", StringComparison.OrdinalIgnoreCase))
          {
            ts.n_grnbeli = cttrx.n_gqty;
            ts.n_brnbeli = cttrx.n_bqty;
          }
          else if (cttrx.c_add1.Equals("02", StringComparison.OrdinalIgnoreCase))
          {
            ts.n_grnbonus = cttrx.n_gqty;
            ts.n_brnbonus = cttrx.n_bqty;
          }
        }
      });
      #endregion
      lstTempTrx.Clear();
      lstTemp.Clear();

      #endregion

      #region RN RS, Claim, Repack

      lstTemp.Add("02");
      lstTemp.Add("03");
      lstTemp.Add("04");
      lstTempTrx = (from q in db.LG_RNHs
                    join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                    where (q.c_gdg == gudang) && lstTemp.Contains(q.c_type) //(q.c_type == "02")
                      && ((q.d_rndate >= date1) && (q.d_rndate < date2))
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && qryItemsAktif.Contains(q1.c_iteno)
                    group q1 by new { q.c_type, q1.c_iteno } into g
                    select new CLASS_TEMP_TRANSKSI()
                    {
                      c_gdg = gudang,
                      c_add1 = (g.Key.c_type == null ? string.Empty : g.Key.c_type.Trim()),
                      c_iteno = (g.Key.c_iteno == null ? string.Empty : g.Key.c_iteno.Trim()),
                      n_gqty = g.Sum(y => (y.n_gqty.HasValue ? y.n_gqty.Value : 0)),
                      n_bqty = g.Sum(y => (y.n_bqty.HasValue ? y.n_bqty.Value : 0))
                    }).ToList();
      #region Populate
      lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
      {
        ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
        {
          return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
        });

        if (ts == null)
        {
          if (cttrx.c_add1.Equals("02", StringComparison.OrdinalIgnoreCase))
          {
            lstTempStock.Add(new CLASS_TEMP_STOCK()
            {
              c_gdg = gudang,
              c_iteno = cttrx.c_iteno,
              n_grnrs = cttrx.n_gqty,
              n_brnrs = cttrx.n_bqty,
            });
          }
          else if (cttrx.c_add1.Equals("03", StringComparison.OrdinalIgnoreCase))
          {
            lstTempStock.Add(new CLASS_TEMP_STOCK()
            {
              c_gdg = gudang,
              c_iteno = cttrx.c_iteno,
              n_grnclaim = cttrx.n_gqty,
              n_brnclaim = cttrx.n_bqty,
            });
          }
          else if (cttrx.c_add1.Equals("04", StringComparison.OrdinalIgnoreCase))
          {
            lstTempStock.Add(new CLASS_TEMP_STOCK()
            {
              c_gdg = gudang,
              c_iteno = cttrx.c_iteno,
              n_grnrepack = cttrx.n_gqty,
              n_brnrepack = cttrx.n_bqty,
            });
          }
        }
        else
        {
          if (cttrx.c_add1.Equals("02", StringComparison.OrdinalIgnoreCase))
          {
            ts.n_grnrs = cttrx.n_gqty;
            ts.n_brnrs = cttrx.n_bqty;
          }
          else if (cttrx.c_add1.Equals("03", StringComparison.OrdinalIgnoreCase))
          {
            ts.n_grnclaim = cttrx.n_gqty;
            ts.n_brnclaim = cttrx.n_bqty;
          }
          else if (cttrx.c_add1.Equals("04", StringComparison.OrdinalIgnoreCase))
          {
            ts.n_grnrepack = cttrx.n_gqty;
            ts.n_brnrepack = cttrx.n_bqty;
          }
        }
      });
      #endregion
      lstTempTrx.Clear();
      lstTemp.Clear();

      #endregion

      #region RN Gudang

      lstTempTrx = (from q in db.LG_SJHs
                    join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                    where (q.c_gdg2 == gudang)
                      && ((q.d_update >= date1) && (q.d_update < date2))
                      && ((q.l_status.HasValue ? q.l_status.Value : false) == true)
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && qryItemsAktif.Contains(q1.c_iteno)
                    group q1 by new { q.c_gdg2, q1.c_iteno } into g
                    select new CLASS_TEMP_TRANSKSI()
                    {
                      c_gdg = (g.Key.c_gdg2.HasValue ? g.Key.c_gdg2.Value : char.MinValue),
                      c_iteno = (g.Key.c_iteno == null ? string.Empty : g.Key.c_iteno.Trim()),
                      n_gqty = g.Sum(y => (y.n_gqty.HasValue ? y.n_gqty.Value : 0)),
                      n_bqty = g.Sum(y => (y.n_bqty.HasValue ? y.n_bqty.Value : 0))
                    }).ToList();
      #region Populate
      lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
      {
        ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
        {
          return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
        });

        if (ts == null)
        {
          lstTempStock.Add(new CLASS_TEMP_STOCK()
          {
            c_gdg = gudang,
            c_iteno = cttrx.c_iteno,
            n_grngdg = cttrx.n_gqty,
            n_brngdg = cttrx.n_bqty,
          });
        }
        else
        {
          ts.n_grngdg = cttrx.n_gqty;
          ts.n_brngdg = cttrx.n_bqty;
        }
      });
      #endregion
      lstTempTrx.Clear();

      #endregion

      #region RC

      lstTempTrx = (from q in db.LG_RCHes
                    join q1 in db.LG_RCD1s on new { q.c_gdg, q.c_rcno } equals new { q1.c_gdg, q1.c_rcno }
                    where (q.c_gdg == gudang)
                      && ((q.d_rcdate >= date1) && (q.d_rcdate < date2))
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && qryItemsAktif.Contains(q1.c_iteno)
                    group q1 by new { q1.c_iteno } into g
                    select new CLASS_TEMP_TRANSKSI()
                    {
                      c_gdg = gudang,
                      c_iteno = (g.Key.c_iteno == null ? string.Empty : g.Key.c_iteno.Trim()),
                      n_gqty = g.Sum(t => (t.c_type == "01" ? (t.n_qty.HasValue ? t.n_qty.Value : 0) : 0)),
                      n_bqty = g.Sum(t => (t.c_type != "01" ? (t.n_qty.HasValue ? t.n_qty.Value : 0) : 0)),
                    }).ToList();
      #region Populate
      lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
      {
        ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
        {
          return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
        });

        if (ts == null)
        {
          lstTempStock.Add(new CLASS_TEMP_STOCK()
          {
            c_gdg = gudang,
            c_iteno = cttrx.c_iteno,
            n_grc = cttrx.n_gqty,
            n_brc = cttrx.n_bqty,
          });
        }
        else
        {
          ts.n_grc = cttrx.n_gqty;
          ts.n_brc = cttrx.n_bqty;
        }
      });
      #endregion
      lstTempTrx.Clear();

      #endregion

      #region Combo

      lstTempTrx = (from q in db.LG_ComboHs
                    where (q.c_gdg == gudang) && (q.c_type == "01")
                      && ((q.l_confirm.HasValue ? q.l_confirm.Value : false) == true)
                      && ((q.d_combodate >= date1) && (q.d_combodate < date2))
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && qryItemsAktif.Contains(q.c_iteno)
                    group q by new { q.c_iteno } into g
                    select new CLASS_TEMP_TRANSKSI()
                    {
                      c_gdg = gudang,
                      c_iteno = (g.Key.c_iteno == null ? string.Empty : g.Key.c_iteno.Trim()),
                      n_gqty = g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
                      //n_bqty = g.Sum(t => (t.n_bqty.HasValue ? t.n_bqty.Value : 0)),
                      n_bqty = 0,
                    }).ToList();
      #region Populate
      lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
      {
        ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
        {
          return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
        });

        if (ts == null)
        {
          lstTempStock.Add(new CLASS_TEMP_STOCK()
          {
            c_gdg = gudang,
            c_iteno = cttrx.c_iteno,
            n_combo = cttrx.n_gqty,
          });
        }
        else
        {
          ts.n_combo = cttrx.n_gqty;
        }
      });
      #endregion
      lstTempTrx.Clear();

      #endregion

      #region RS Beli, Repack
      
      lstTemp.Add("01");
      lstTemp.Add("02");
      lstTempTrx = (from q in db.LG_RSHes
                    join q1 in db.LG_RSD1s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                    where (q.c_gdg == gudang) && lstTemp.Contains(q.c_type)
                      && ((q.d_rsdate >= date1) && (q.d_rsdate < date2))
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && qryItemsAktif.Contains(q1.c_iteno)
                    group q1 by new { q.c_type, q1.c_iteno } into g
                    select new CLASS_TEMP_TRANSKSI()
                    {
                      c_gdg = gudang,
                      c_add1 = (g.Key.c_type == null ? string.Empty : g.Key.c_type.Trim()),
                      c_iteno = (g.Key.c_iteno == null ? string.Empty : g.Key.c_iteno.Trim()),
                      n_gqty = g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
                      n_bqty = g.Sum(t => (t.n_bqty.HasValue ? t.n_bqty.Value : 0)),
                    }).ToList();
      #region Populate
      lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
      {
        ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
        {
          return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
        });

        if (ts == null)
        {
          if (cttrx.c_add1.Equals("01", StringComparison.OrdinalIgnoreCase))
          {
            lstTempStock.Add(new CLASS_TEMP_STOCK()
            {
              c_gdg = gudang,
              c_iteno = cttrx.c_iteno,
              n_grsbeli = cttrx.n_gqty,
              n_brsbeli = cttrx.n_bqty,
            });
          }
          else if (cttrx.c_add1.Equals("02", StringComparison.OrdinalIgnoreCase))
          {
            lstTempStock.Add(new CLASS_TEMP_STOCK()
            {
              c_gdg = gudang,
              c_iteno = cttrx.c_iteno,
              n_grsrepack = cttrx.n_gqty,
              n_brsrepack = cttrx.n_bqty,
            });
          }
        }
        else
        {
          if (cttrx.c_add1.Equals("01", StringComparison.OrdinalIgnoreCase))
          {
            ts.n_grsbeli = cttrx.n_gqty;
            ts.n_brsbeli = cttrx.n_bqty;
          }
          else if (cttrx.c_add1.Equals("02", StringComparison.OrdinalIgnoreCase))
          {
            ts.n_grsrepack = cttrx.n_gqty;
            ts.n_brsrepack = cttrx.n_bqty;
          }
        }
      });
      #endregion
      lstTempTrx.Clear();
      lstTemp.Clear();

      #endregion

      #region SJ Std, Claim

      lstTempTrx = (from q in db.LG_SJHs
                    join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                    where (q.c_gdg == gudang)
                      && ((q.d_sjdate >= date1) && (q.d_sjdate < date2))
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && qryItemsAktif.Contains(q1.c_iteno)
                    group q1 by new { q.c_type, q1.c_iteno } into g
                    select new CLASS_TEMP_TRANSKSI()
                    {
                      c_gdg = gudang,
                      c_add1 = (g.Key.c_type == null ? string.Empty : g.Key.c_type.Trim()),
                      c_iteno = (g.Key.c_iteno == null ? string.Empty : g.Key.c_iteno.Trim()),
                      n_gqty = g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
                      n_bqty = g.Sum(t => (t.n_bqty.HasValue ? t.n_bqty.Value : 0)),
                    }).ToList();
      #region Populate
      lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
      {
        ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
        {
          return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
        });

        if (ts == null)
        {
          if (cttrx.c_add1.Equals("03", StringComparison.OrdinalIgnoreCase))
          {
            lstTempStock.Add(new CLASS_TEMP_STOCK()
            {
              c_gdg = gudang,
              c_iteno = cttrx.c_iteno,
              n_gsjclaim = cttrx.n_gqty,
              n_bsjclaim = cttrx.n_bqty,
            });
          }
          else
          {
            lstTempStock.Add(new CLASS_TEMP_STOCK()
            {
              c_gdg = gudang,
              c_iteno = cttrx.c_iteno,
              n_gsj = cttrx.n_gqty,
              n_bsj = cttrx.n_bqty,
            });
          }
        }
        else
        {
          if (cttrx.c_add1.Equals("03", StringComparison.OrdinalIgnoreCase))
          {
            ts.n_gsjclaim += cttrx.n_gqty;
            ts.n_bsjclaim += cttrx.n_bqty;
          }
          else
          {
            ts.n_gsj += cttrx.n_gqty;
            ts.n_bsj += cttrx.n_bqty;
          }
        }
      });
      #endregion
      lstTempTrx.Clear();
      lstTemp.Clear();

      #endregion

      #region PL

      lstTempTrx = (from q in db.LG_PLHs
                    join q1 in db.LG_PLD1s on q.c_plno equals q1.c_plno
                    where (q.c_gdg == gudang) && (q.l_confirm == true)
                      && ((q.d_pldate >= date1) && (q.d_pldate < date2))
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && qryItemsAktif.Contains(q1.c_iteno)
                    group q1 by new { q1.c_iteno } into g
                    select new CLASS_TEMP_TRANSKSI()
                    {
                      c_gdg = gudang,
                      c_iteno = (g.Key.c_iteno == null ? string.Empty : g.Key.c_iteno.Trim()),
                      n_gqty = g.Sum(y => (y.n_qty.HasValue ? y.n_qty.Value : 0)),
                      n_bqty = 0
                    }).ToList();
      #region Populate
      lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
      {
        ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
        {
          return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
        });

        if (ts == null)
        {
          lstTempStock.Add(new CLASS_TEMP_STOCK()
          {
            c_gdg = gudang,
            c_iteno = cttrx.c_iteno,
            n_pl = cttrx.n_gqty,
          });
        }
        else
        {
          ts.n_pl = cttrx.n_gqty;
        }
      });
      #endregion
      lstTempTrx.Clear();

      #endregion

      #region STT

      lstTempTrx = (from q in db.LG_STHs
                    join q1 in db.LG_STD1s on new { q.c_gdg, q.c_stno } equals new { q1.c_gdg, q1.c_stno }
                    where (q.c_gdg == gudang)
                      && ((q.d_stdate >= date1) && (q.d_stdate < date2))
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && qryItemsAktif.Contains(q1.c_iteno)
                    group q1 by new { q1.c_iteno } into g
                    select new CLASS_TEMP_TRANSKSI()
                    {
                      c_gdg = gudang,
                      c_iteno = (g.Key.c_iteno == null ? string.Empty : g.Key.c_iteno.Trim()),
                      n_gqty = g.Sum(y => (y.n_qty.HasValue ? y.n_qty.Value : 0)),
                      n_bqty = 0
                    }).ToList();
      #region Populate
      lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
      {
        ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
        {
          return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
        });

        if (ts == null)
        {
          lstTempStock.Add(new CLASS_TEMP_STOCK()
          {
            c_gdg = gudang,
            c_iteno = cttrx.c_iteno,
            n_stt = cttrx.n_gqty,
          });
        }
        else
        {
          ts.n_stt = cttrx.n_gqty;
        }
      });
      #endregion
      lstTempTrx.Clear();

      #endregion

      #region Combo Detail

      lstTempTrx = (from q in db.LG_ComboHs
                    join q1 in db.LG_ComboD1s on new { q.c_gdg, q.c_combono } equals new { q1.c_gdg, q1.c_combono }
                    where (q.c_gdg == gudang) && (q.l_confirm == true)
                      && ((q.d_combodate >= date1) && (q.d_combodate < date2))
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && qryItemsAktif.Contains(q1.c_iteno)
                    group q1 by new { q.c_gdg, q1.c_iteno } into g
                    select new CLASS_TEMP_TRANSKSI()
                    {
                      c_gdg = g.Key.c_gdg,
                      c_iteno = g.Key.c_iteno,
                      n_gqty = g.Sum(t => (t.n_qty.HasValue ? t.n_qty.Value : 0)),
                      n_bqty = 0,
                    }).Distinct().ToList();
      #region Populate
      lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
      {
        ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
        {
          return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
        });

        if (ts == null)
        {
          lstTempStock.Add(new CLASS_TEMP_STOCK()
          {
            c_gdg = gudang,
            c_iteno = cttrx.c_iteno,
            n_combod = cttrx.n_gqty,
          });
        }
        else
        {
          ts.n_combod = cttrx.n_gqty;
        }
      });
      #endregion
      lstTempTrx.Clear();

      #endregion

      #region Adjustment

      lstTempTrx = (from q in db.LG_AdjustHs
                    join q1 in db.LG_AdjustD1s on new { q.c_gdg, q.c_adjno } equals new { q1.c_gdg, q1.c_adjno }
                    where (q.c_gdg == gudang)
                      && ((q.d_adjdate >= date1) && (q.d_adjdate < date2))
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && qryItemsAktif.Contains(q1.c_iteno)
                    group q1 by new { q.c_gdg, q1.c_iteno } into g
                    select new CLASS_TEMP_TRANSKSI()
                    {
                      c_gdg = g.Key.c_gdg,
                      c_iteno = g.Key.c_iteno,
                      n_gawal = g.Sum(t => ((t.n_gqty.HasValue ? t.n_gqty.Value : 0) > 0 ? (t.n_gqty.HasValue ? t.n_gqty.Value : 0) : 0)),
                      n_bawal = g.Sum(t => ((t.n_bqty.HasValue ? t.n_bqty.Value : 0) > 0 ? (t.n_bqty.HasValue ? t.n_bqty.Value : 0) : 0)),
                      n_gqty = g.Sum(t => ((t.n_gqty.HasValue ? t.n_gqty.Value : 0) < 0 ? -(t.n_gqty.HasValue ? t.n_gqty.Value : 0) : 0)),
                      n_bqty = g.Sum(t => ((t.n_bqty.HasValue ? t.n_bqty.Value : 0) < 0 ? -(t.n_bqty.HasValue ? t.n_bqty.Value : 0) : 0)),
                    }).Distinct().ToList();
      #region Populate
      lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
      {
        ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
        {
          return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
        });

        if (ts == null)
        {
          lstTempStock.Add(new CLASS_TEMP_STOCK()
          {
            c_gdg = gudang,
            c_iteno = cttrx.c_iteno,
            n_gadj = cttrx.n_gqty,
            n_badj = cttrx.n_bqty,
            n_grngdg = cttrx.n_gawal,
            n_brngdg = cttrx.n_bawal,
          });
        }
        else
        {
          ts.n_gadj = cttrx.n_gqty;
          ts.n_badj = cttrx.n_bqty;

          ts.n_grngdg += cttrx.n_gawal;
          ts.n_brngdg += cttrx.n_bawal;
        }
      });
      #endregion
      lstTempTrx.Clear();

      #endregion

      #endregion

      var qryInnerItemHpp = (from sq in db.FA_MasItms
                             //join sq1 in db.LG_HPPs on new { sq.c_iteno, s_tahun = (short)date2.Year, t_bulan = (byte)date2.Month, c_type = "01" } equals new { sq1.c_iteno, sq1.s_tahun, sq1.t_bulan, sq1.c_type } into sq_1
                             //from qHPP in sq_1.DefaultIfEmpty()
                             where ((sq.l_aktif.HasValue ? sq.l_aktif.Value : false) == true)
                               //&& ((sq.l_hide.HasValue ? sq.l_hide.Value : false) == false)
                               && ((sq.l_delete.HasValue ? sq.l_delete.Value : false) == false)
                             select new DSNG_SCMS_LGPBF()
                             {
                               c_iteno = sq.c_iteno,
                               v_itnam = sq.v_itnam,
                               c_alkes = sq.c_alkes,
                               n_pbf = ((sq.n_PBF.HasValue ? sq.n_PBF.Value : 0) == 0 ? 1 : (sq.n_PBF.HasValue ? sq.n_PBF.Value : 0)),
                               //n_hna = ((qHPP != null) && qHPP.n_akhirhpp.HasValue ? qHPP.n_akhirhpp.Value : (sq.n_salpri.HasValue ? sq.n_salpri.Value : 0)) / ((sq.n_PBF.HasValue ? sq.n_PBF.Value : 0) == 0 ? 1 : (sq.n_PBF.HasValue ? sq.n_PBF.Value : 0)) * 1.1m,
                               n_hna = (sq.n_salpri.HasValue ? sq.n_salpri.Value : 0),
                               n_het = (sq.n_het.HasValue ? sq.n_het.Value : 0),
                             }).AsQueryable();


      var qryInnerDivPri = (from sq in db.FA_MsDivPris
                            join sq1 in db.FA_Divpris on sq.c_kddivpri equals sq1.c_kddivpri
                            where ((sq.l_delete.HasValue ? sq.l_delete.Value : false) == false)
                            select new DSNG_SCMS_LGPBF()
                            {
                              n_hna = ((sq.n_het.HasValue ? sq.n_het.Value : 0) == 0 ? 1.25m : (sq.n_het.HasValue ? sq.n_het.Value : 0)),
                              c_iteno = sq1.c_iteno,
                            }).AsQueryable();

      listTempPBF = (from q in qryInnerItemHpp
                     join q1 in qryInnerDivPri on q.c_iteno equals q1.c_iteno into q_1
                     from qIDP in q_1.DefaultIfEmpty()
                     select new DSNG_SCMS_LGPBF()
                     {
                       c_iteno = q.c_iteno,
                       v_itnam = q.v_itnam,
                       c_alkes = q.c_alkes,
                       //n_het = (q.n_het * q1.n_het)
                       //n_hna = ((qIDP != null) && (qIDP.n_hna != 0) ? qIDP.n_hna : 1) * q.n_hna,
                       n_hna = ((q.n_hna == 0) ? (((qIDP != null) && (qIDP.n_hna != 0) ? qIDP.n_hna : 1) * q.n_hna) : q.n_hna),
                       n_het = q.n_het,
                       n_pbf = q.n_pbf
                     }).ToList();

      dateX = date2.AddDays(-1);

      tmp1 = date1.ToString("dd-MM-yyyy");
      tmp2 = dateX.ToString("dd-MM-yyyy");

      listPBF = (from q in lstTempStock
                 join q1 in listTempPBF on q.c_iteno equals q1.c_iteno
                 select new DSNG_SCMS_LGPBF()
                 {
                   c_gdg = q.c_gdg,
                   c_iteno = q.c_iteno,
                   n_awal = q.n_gawal,
                   n_inPabrik = (q.n_grnbeli + q.n_grnbonus + q.n_grnclaim + q.n_grnrs + q.n_grnrepack),
                   n_inpbf = 0,
                   n_inLain = (q.n_grc + q.n_grngdg + q.n_combo),
                   n_OutRS = (q.n_grsbeli + q.n_grsrepack),
                   n_outApotik = 0,
                   n_outPBF = (q.n_gsj + q.n_gsjclaim + q.n_pl + q.n_stt),
                   n_outLain = (q.n_combod + q.n_gadj),
                   n_outPemerintah = 0,
                   n_outSwasta = 0,
                   n_hna = q1.n_hna,
                   n_het = q1.n_het,
                   d_awal = tmp1,
                   d_akhir = tmp2,
                   n_pbf = q1.n_pbf,
                   Periode = tipePeriode,
                   c_alkes = q1.c_alkes,
                   v_itnam = q1.v_itnam
                 }).ToList();

      var tttt = listPBF.Where(x => x.c_iteno == "3732").ToList();

      var data = listPBF.OrderBy(x => x.c_iteno).CopyToDataTableObject();

      //data.WriteXml("d:\\out.xml");

      var rows = data.Select("c_iteno = '0017'");

    }

    public System.Data.DataSet ReportSales(ORMDataContext db, Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> dic)
    {
      ScmsSoaLibrary.Commons.Functionals.ParameterParser pp = default(ScmsSoaLibrary.Commons.Functionals.ParameterParser);

      DateTime date1 = DateTime.MinValue,
        date2 = DateTime.MinValue;
      string[] cusnos = null,
        items = null,
        divAmses = null,
        nosup = null;

      System.Data.DataSet dataSet = null;
      System.Data.DataTable table = null;

      List<DSNG_SCMS_QuerySales> ListFJ = null;
      List<DSNG_SCMS_QuerySales> ListTran = null;
      List<DSNG_SCMS_QuerySales> ListTranAMS = null;
      List<DSNG_SCMS_QuerySales> ListQuery = null;

      #region Parameter

      try
      {

        if (dic.ContainsKey("date1"))
        {
          pp = dic["date1"];
          if (pp.IsSet)
          {
            date1 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("date2"))
        {
          pp = dic["date2"];
          if (pp.IsSet)
          {
            date2 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("cusno"))
        {
          pp = dic["cusno"];
          if (pp.IsSet)
          {
            cusnos = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
        }
        else
        {
          cusnos = new string[0];
        }
        if (dic.ContainsKey("nosup"))
        {
          pp = dic["nosup"];
          if (pp.IsSet)
          {
            nosup = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
        }
        else
        {
          nosup = new string[0];
        }
        if (dic.ContainsKey("item"))
        {
          pp = dic["item"];
          if (pp.IsSet)
          {
            items = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
          else
          {
            items = new string[0];
          }
        }
        else
        {
          items = new string[0];
        }
        if (dic.ContainsKey("divams"))
        {
          pp = dic["divams"];
          if (pp.IsSet)
          {
            divAmses = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
          else
          {
            divAmses = new string[0];
          }
        }
        else
        {
          divAmses = new string[0];
        }
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales Paramter - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      //if (date1.Equals(DateTime.MinValue) || date2.Equals(DateTime.MinValue))
      if (date1.Equals(DateTime.MinValue))
      {
        return null;
      }
      else if (date2.Equals(DateTime.MinValue))
      {
        date2 = date1;
      }

      #endregion

      #region Query

      try
      {
        ListFJ = (from q in db.LG_FJHs
                  join q1 in db.LG_FJD1s on q.c_fjno equals q1.c_fjno
                  where ((q.d_fjdate >= date1) && (q.d_fjdate.Value <= date2))
                    && ((cusnos.Length > 0) ? cusnos.Contains(q.c_cusno) : true)
                    && ((items.Length > 0) ? items.Contains(q1.c_iteno) : true)
                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                  select new DSNG_SCMS_QuerySales()
                  {
                    c_fjno = q.c_fjno,
                    c_cusno = q.c_cusno,
                    date_fjdate = (q.d_fjdate.HasValue ? q.d_fjdate.Value : Functionals.StandardSqlDateTime),
                    c_iteno = q1.c_iteno,
                    n_qty = (q1.n_qty.HasValue ? q1.n_qty.Value : 0),
                    n_salpri = (q1.n_salpri.HasValue ? q1.n_salpri.Value : 0),
                    n_disc = (q1.n_disc.HasValue ? q1.n_disc.Value : 0)
                  }).Distinct().ToList();

        ListTran = (from q in db.LG_Cusmas
                    join q1 in db.MsTransDs on new { c_portal = '3', c_notrans = "59", c_type = q.c_sektor } equals new { q1.c_portal, q1.c_notrans, q1.c_type }
                    where ((cusnos.Length > 0) ? cusnos.Contains(q.c_cusno) : true)
                    select new DSNG_SCMS_QuerySales()
                    {
                      c_cusno = q.c_cusno,
                      v_cunam = q.v_cunam,
                      c_sektor = q.c_sektor,
                      v_ket = q1.v_ket
                    }).Distinct().ToList();

        ListTranAMS = (from q in db.FA_DivAMs
                       join q1 in db.FA_MsDivAMs on q.c_kddivams equals q1.c_kddivams
                       join q2 in db.FA_MasItms on q.c_iteno equals q2.c_iteno
                       where ((items.Length > 0) ? items.Contains(q.c_iteno) : true)
                        && ((divAmses.Length > 0) ? divAmses.Contains(q.c_kddivams) : true)
                        && ((nosup.Length > 0) ? nosup.Contains(q2.c_nosup) : true)
                       select new DSNG_SCMS_QuerySales()
                       {
                         c_iteno = q.c_iteno,
                         c_kddivams = q1.c_kddivams,
                         v_nmdivams = q1.v_nmdivams
                       }).Distinct().ToList();

        ListQuery = (from q in ListFJ
                     join q2 in ListTran on q.c_cusno equals q2.c_cusno
                     join q3 in ListTranAMS on q.c_iteno equals q3.c_iteno
                     select new DSNG_SCMS_QuerySales()
                     {
                       c_fjno = q.c_fjno,
                       c_cusno = q.c_cusno,
                       d_fjdate = q.date_fjdate.ToString("yyyy-MM-dd"),
                       c_iteno = q.c_iteno,
                       n_qty = q.n_qty,
                       n_salpri = q.n_salpri,
                       n_disc = q.n_disc,
                       c_sektor = q2.c_sektor,
                       v_cunam = q2.v_cunam,
                       v_ket = q2.v_ket,
                       c_kddivams = q3.c_kddivams,
                       v_nmdivams = q3.v_nmdivams,
                       l_cabang = (q2.c_sektor == "01" ? true : false),
                       l_clinic = (q2.c_sektor == "03" ? true : false),
                     }).ToList();
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales QueryLogic - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      #endregion

      #region Populate

      if (ListQuery != null)
      {
        try
        {
          table = ListQuery.CopyToDataTableObject();

          if (table != null)
          {
            dataSet = new System.Data.DataSet();

            dataSet.Tables.Add(table);
          }
        }
        catch (Exception ex)
        {
          this.IsError = true;
          this.ErrorMessage = ex.Message;

          Logger.WriteLine(
            "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales PopulateDataset - {0}", ex.StackTrace);
          Logger.WriteLine(ex.StackTrace);
        }
      }

      #endregion

      if (ListFJ != null)
      {
        ListFJ.Clear();
      }
      if (ListTran != null)
      {
        ListTran.Clear();
      }
      if (ListTranAMS != null)
      {
        ListTranAMS.Clear();
      }
      if (ListQuery != null)
      {
        ListQuery.Clear();
      }

      return dataSet;
    }

    public System.Data.DataSet PurchasePerFB(ORMDataContext db, Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> dic)
    {
      ScmsSoaLibrary.Commons.Functionals.ParameterParser pp = default(ScmsSoaLibrary.Commons.Functionals.ParameterParser);

      DateTime date1 = DateTime.MinValue,
        date2 = DateTime.MinValue;
      string[] cusnos = null,
        items = null,
        divAmses = null;

      System.Data.DataSet dataSet = null;
      System.Data.DataTable table = null;

      List<DSNG_SCMS_PurchasePerFB> ListFB = null;
      List<DSNG_SCMS_PurchasePerFB> ListSup = null;
      List<DSNG_SCMS_PurchasePerFB> ListTran = null;
      List<DSNG_SCMS_PurchasePerFB> ListTranAMS = null;
      List<DSNG_SCMS_PurchasePerFB> ListQuery = null;

      #region Parameter

      try
      {

        if (dic.ContainsKey("date1"))
        {
          pp = dic["date1"];
          if (pp.IsSet)
          {
            date1 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("date2"))
        {
          pp = dic["date2"];
          if (pp.IsSet)
          {
            date2 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("nosup"))
        {
          pp = dic["nosup"];
          if (pp.IsSet)
          {
            cusnos = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
        }
        else
        {
          cusnos = new string[0];
        }
        if (dic.ContainsKey("item"))
        {
          pp = dic["item"];
          if (pp.IsSet)
          {
            items = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
          else
          {
            items = new string[0];
          }
        }
        else
        {
          items = new string[0];
        }
        if (dic.ContainsKey("divams"))
        {
          pp = dic["divams"];
          if (pp.IsSet)
          {
            divAmses = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
          else
          {
            divAmses = new string[0];
          }
        }
        else
        {
          divAmses = new string[0];
        }
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales Paramter - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      //if (date1.Equals(DateTime.MinValue) || date2.Equals(DateTime.MinValue))
      if (date1.Equals(DateTime.MinValue))
      {
        return null;
      }
      else if (date2.Equals(DateTime.MinValue))
      {
        date2 = date1;
      }

      #endregion

      #region Query

      try
      {
        ListFB = (from q in db.LG_FBHs
                  join q1 in db.LG_FBD1s on q.c_fbno equals q1.c_fbno
                  join q2 in db.LG_FBD2s on q.c_fbno equals q2.c_fbno
                  join q3 in db.LG_RNHs on q2.c_rnno equals q3.c_rnno
                  where ((q.d_fbdate >= date1) && (q.d_fbdate.Value <= date2))
                    && ((cusnos.Length > 0) ? cusnos.Contains(q.c_nosup) : true)
                    && ((items.Length > 0) ? items.Contains(q1.c_iteno) : true)
                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                  select new DSNG_SCMS_PurchasePerFB()
                  {
                    c_rnno = q3.c_rnno,
                    d_rndate = (q3.d_rndate.HasValue ? q3.d_rndate.Value : Functionals.StandardSqlDateTime),
                    d_fbdate = (q.d_fbdate.HasValue ? q.d_fbdate.Value : Functionals.StandardSqlDateTime),
                    c_fbno = q.c_fbno,
                    c_fb = q.c_fb,
                    c_taxno = q.c_taxno,
                    c_nosup = q.c_nosup,
                    n_bruto = ((q1.n_qty.HasValue ? q1.n_qty.Value : 0) * (q1.n_salpri.HasValue ? q1.n_salpri.Value : 0) ),
                    bonus = (q1.c_type == "01" ? ((q1.n_qty.HasValue ? q1.n_qty.Value : 0) * (q1.n_salpri.HasValue ? q1.n_salpri.Value : 0)) : 0),
                    n_disc = 
                      (((q1.n_disc.HasValue ? q1.n_disc.Value : 0) / 100) * (q1.n_qty.HasValue ? q1.n_qty.Value : 0)) * (q1.n_salpri.HasValue ? q1.n_salpri.Value : 0),
                    n_xdisc = 
                      (q1.c_type == "01" ? 
                        (((q1.n_qty.HasValue ? q1.n_qty.Value : 0) * (q1.n_salpri.HasValue ? q1.n_salpri.Value : 0)) / (q.n_bruto.HasValue ? q.n_bruto.Value : 0)) * 
                          (q.n_xdisc.HasValue ? q.n_xdisc.Value : 0) : 0),
                    dpp = 0,
                    n_ppn = 0,
                    n_bea = (q1.n_bea.HasValue ? q1.n_bea.Value : 0),
                    invt = 0,
                    bilva = 0,
                    n_qty = (q1.n_qty.HasValue ? q1.n_qty.Value : 0),
                    n_salpri = (q1.n_salpri.HasValue ? q1.n_salpri.Value : 0),
                    c_iteno = q1.c_iteno,
                    c_type = q1.c_type,
                  }).Distinct().ToList();

        ListFB.ForEach(delegate(DSNG_SCMS_PurchasePerFB t)
        {
          t.dpp = t.n_bruto - t.n_disc - t.n_xdisc;
          t.n_ppn = (t.n_bruto - t.n_disc - t.n_xdisc) / 10;
          t.invt = (t.n_bruto - t.n_disc - t.n_xdisc) + t.n_bea;
          t.bilva = (t.n_bruto - t.n_disc - t.n_xdisc) + ((t.n_bruto - t.n_disc - t.n_xdisc) / 10);
        });

        ListSup = (from q1 in db.LG_DatSups
                    where ((cusnos.Length > 0) ? cusnos.Contains(q1.c_nosup) : true)
                    select new DSNG_SCMS_PurchasePerFB()
                    {
                      c_nosup = q1.c_nosup,
                      v_nama = q1.v_nama,
                    }).Distinct().ToList();

        ListTranAMS = (from q1 in db.FA_DivAMs
                       join q2 in db.FA_MsDivAMs on q1.c_kddivams equals q2.c_kddivams
                       where ((divAmses.Length > 0) ? divAmses.Contains(q1.c_kddivams) : true)
                       select new DSNG_SCMS_PurchasePerFB()
                       {
                         c_kddivams = q1.c_kddivams,
                         v_nmdivams = q2.v_nmdivams,
                         c_iteno = q1.c_iteno,
                       }).Distinct().ToList();

        ListQuery = (from q in ListFB
                     join q1 in ListSup on q.c_nosup equals q1.c_nosup
                     join q2 in ListTranAMS on q.c_iteno equals q2.c_iteno
                     select new DSNG_SCMS_PurchasePerFB()
                     {
                       c_rnno = q.c_rnno,
                       d_rndate = q.d_rndate,
                       d_fbdate = q.d_fbdate,
                       c_fbno = q.c_fbno,
                       c_fb = q.c_fb,
                       c_taxno = q.c_taxno,
                       c_nosup = q.c_nosup,
                       n_bruto = q.n_bruto,
                       bonus = q.bonus,
                       n_disc = q.n_disc,
                       n_xdisc = q.n_xdisc,
                       dpp = q.dpp,
                       n_ppn = q.n_ppn,
                       n_bea = q.n_bea,
                       invt = q.invt,
                       bilva = q.bilva,
                       n_qty = q.n_qty,
                       n_salpri = q.n_salpri,
                       c_iteno = q.c_iteno,
                       c_type = q.c_type,
                       v_nama = q1.v_nama,
                       c_kddivams = q2.c_kddivams,
                       v_nmdivams = q2.v_nmdivams
                     }).ToList();

      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales QueryLogic - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      #endregion

      #region Populate

      if (ListQuery != null)
      {
        try
        {
          table = ListQuery.CopyToDataTableObject();

          if (table != null)
          {
            dataSet = new System.Data.DataSet();

            dataSet.Tables.Add(table);
          }
        }
        catch (Exception ex)
        {
          this.IsError = true;
          this.ErrorMessage = ex.Message;

          Logger.WriteLine(
            "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales PopulateDataset - {0}", ex.StackTrace);
          Logger.WriteLine(ex.StackTrace);
        }
      }

      #endregion

      if (ListFB != null)
      {
        ListFB.Clear();
      }
      if (ListTran != null)
      {
        ListTran.Clear();
      }
      if (ListTranAMS != null)
      {
        ListTranAMS.Clear();
      }
      if (ListQuery != null)
      {
        ListQuery.Clear();
      }

      return dataSet;
    }

    public System.Data.DataSet QueryPurchase(ORMDataContext db, Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> dic)
    {
      ScmsSoaLibrary.Commons.Functionals.ParameterParser pp = default(ScmsSoaLibrary.Commons.Functionals.ParameterParser);

      DateTime date1 = DateTime.MinValue,
        date2 = DateTime.MinValue;
      string[] cusnos = null,
        items = null,
        divAmses = null;

      System.Data.DataSet dataSet = null;
      System.Data.DataTable table = null;

      List<DSNG_SCMS_QueryPurchase> ListFB = null;
      List<DSNG_SCMS_QueryPurchase> ListSup = null;
      List<DSNG_SCMS_QueryPurchase> ListQuery = null;

      #region Parameter

      try
      {

        if (dic.ContainsKey("date1"))
        {
          pp = dic["date1"];
          if (pp.IsSet)
          {
            date1 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("date2"))
        {
          pp = dic["date2"];
          if (pp.IsSet)
          {
            date2 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("nosup"))
        {
          pp = dic["nosup"];
          if (pp.IsSet)
          {
            cusnos = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
        }
        else
        {
          cusnos = new string[0];
        }
        if (dic.ContainsKey("item"))
        {
          pp = dic["item"];
          if (pp.IsSet)
          {
            items = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
          else
          {
            items = new string[0];
          }
        }
        else
        {
          items = new string[0];
        }
        if (dic.ContainsKey("divams"))
        {
          pp = dic["divams"];
          if (pp.IsSet)
          {
            divAmses = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
          else
          {
            divAmses = new string[0];
          }
        }
        else
        {
          divAmses = new string[0];
        }
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales Paramter - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      //if (date1.Equals(DateTime.MinValue) || date2.Equals(DateTime.MinValue))
      if (date1.Equals(DateTime.MinValue))
      {
        return null;
      }
      else if (date2.Equals(DateTime.MinValue))
      {
        date2 = date1;
      }

      #endregion

      #region Query

      try
      {
        ListFB = (from q in db.LG_FBHs
                  join q1 in
                    (from q4 in db.LG_FBD1s
                     join q2 in db.FA_DivAMs on q4.c_iteno equals q2.c_iteno
                     join q3 in db.FA_MsDivAMs on q2.c_kddivams equals q3.c_kddivams
                     where ((divAmses.Length > 0) ? divAmses.Contains(q2.c_kddivams) : true) &&
                     ((items.Length > 0) ? items.Contains(q4.c_iteno) : true)
                     group q4 by new { q4.c_fbno } into gSum
                     select new
                     {
                       c_fbno = gSum.Key.c_fbno,
                       c_bonus = (gSum.Sum(x => x.c_type == "01" ?
                                  ((x.n_qty.HasValue ? x.n_qty.Value : 0) *
                                    (x.n_salpri.HasValue ? x.n_salpri.Value : 0)) : 0))
                     }) on q.c_fbno equals q1.c_fbno
                  join q5 in db.LG_FBD2s on q.c_fbno equals q5.c_fbno
                  join q6 in db.LG_RNHs on q5.c_rnno equals q6.c_rnno
                  select new DSNG_SCMS_QueryPurchase()
                  {
                    d_fbdate = (q.d_fbdate.HasValue ? q.d_fbdate.Value : Functionals.StandardSqlDateTime),
                    c_fb = q.c_fb,
                    c_taxno = q.c_taxno,
                    c_nosup = q.c_nosup,
                    n_bruto = (q.n_bruto.HasValue ? q.n_bruto.Value : 0),
                    n_disc = (q.n_disc.HasValue ? q.n_disc.Value : 0),
                    n_xdisc = (q.n_xdisc.HasValue ? q.n_xdisc.Value : 0),
                    n_ppn = (q.n_ppn.HasValue ? q.n_ppn.Value : 0),
                    n_bea = (q.n_bea.HasValue ? q.n_bea.Value : 0),
                    bonus = q1.c_bonus,
                    c_rnno = q6.c_rnno,
                    d_rndate = (q6.d_rndate.HasValue ? q6.d_rndate.Value : Functionals.StandardSqlDateTime),
                  }).Distinct().ToList();

        ListFB.ForEach(delegate(DSNG_SCMS_QueryPurchase t)
        {
          t.dpp = t.n_bruto - t.n_disc - t.n_xdisc;
          t.invt = t.n_bruto - t.n_disc - t.n_xdisc + t.n_bea;
          t.bilva = t.n_bruto - t.n_disc - t.n_xdisc + t.n_ppn;
        });

        ListSup = (from q1 in db.LG_DatSups
                    where ((cusnos.Length > 0) ? cusnos.Contains(q1.c_nosup) : true)
                    select new DSNG_SCMS_QueryPurchase()
                    {
                      c_nosup = q1.c_nosup,
                      v_nama = q1.v_nama,
                    }).Distinct().ToList();

        ListQuery = (from q in ListFB
                     join q1 in ListSup on q.c_nosup equals q1.c_nosup
                     select new DSNG_SCMS_QueryPurchase()
                     {
                       d_fbdate = q.d_fbdate,
                       c_fb = q.c_fb,
                       c_taxno = q.c_taxno,
                       c_nosup = q.c_nosup,
                       n_bruto = q.n_bruto,
                       n_disc = q.n_disc,
                       n_xdisc = q.n_xdisc,
                       n_ppn = q.n_ppn,
                       n_bea = q.n_bea,
                       bonus = q.bonus,
                       c_rnno = q.c_rnno,
                       d_rndate = q.d_rndate,
                       dpp = q.dpp,
                       invt = q.invt,
                       bilva = q.bilva,
                       v_nama = q1.v_nama,
                     }).ToList();

      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales QueryLogic - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      #endregion

      #region Populate

      if (ListQuery != null)
      {
        try
        {
          table = ListQuery.CopyToDataTableObject();

          if (table != null)
          {
            dataSet = new System.Data.DataSet();

            dataSet.Tables.Add(table);
          }
        }
        catch (Exception ex)
        {
          this.IsError = true;
          this.ErrorMessage = ex.Message;

          Logger.WriteLine(
            "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales PopulateDataset - {0}", ex.StackTrace);
          Logger.WriteLine(ex.StackTrace);
        }
      }

      #endregion

      if (ListFB != null)
      {
        ListFB.Clear();
      }
      if (ListQuery != null)
      {
        ListQuery.Clear();
      }

      return dataSet;
    }

    public System.Data.DataSet QueryReturBeli(ORMDataContext db, Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> dic)
    {
      ScmsSoaLibrary.Commons.Functionals.ParameterParser pp = default(ScmsSoaLibrary.Commons.Functionals.ParameterParser);

      DateTime date1 = DateTime.MinValue,
        date2 = DateTime.MinValue;
      string[] cusnos = null,
        items = null,
        divAmses = null;

      System.Data.DataSet dataSet = null;
      System.Data.DataTable table = null;

      List<DSNG_SCMS_QueryReturBeli> ListFBR = null;
      List<DSNG_SCMS_QueryReturBeli> ListSup = null;
      List<DSNG_SCMS_QueryReturBeli> ListDivAMS = null;
      List<DSNG_SCMS_QueryReturBeli> ListQuery = null;

      #region Parameter

      try
      {

        if (dic.ContainsKey("date1"))
        {
          pp = dic["date1"];
          if (pp.IsSet)
          {
            date1 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("date2"))
        {
          pp = dic["date2"];
          if (pp.IsSet)
          {
            date2 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("nosup"))
        {
          pp = dic["nosup"];
          if (pp.IsSet)
          {
            cusnos = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
        }
        else
        {
          cusnos = new string[0];
        }
        if (dic.ContainsKey("item"))
        {
          pp = dic["item"];
          if (pp.IsSet)
          {
            items = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
          else
          {
            items = new string[0];
          }
        }
        else
        {
          items = new string[0];
        }
        if (dic.ContainsKey("divams"))
        {
          pp = dic["divams"];
          if (pp.IsSet)
          {
            divAmses = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
          else
          {
            divAmses = new string[0];
          }
        }
        else
        {
          divAmses = new string[0];
        }
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales Paramter - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      //if (date1.Equals(DateTime.MinValue) || date2.Equals(DateTime.MinValue))
      if (date1.Equals(DateTime.MinValue))
      {
        return null;
      }
      else if (date2.Equals(DateTime.MinValue))
      {
        date2 = date1;
      }

      #endregion

      #region Query

      try
      {
        ListFBR = (from q in db.LG_FBRHs
                   join q1 in db.LG_FBRD1s on q.c_fbno equals q1.c_fbno
                  join q2 in db.FA_DivAMs on q1.c_iteno equals q2.c_iteno
                  join q3 in db.FA_MsDivAMs on q2.c_kddivams equals q3.c_kddivams
                  where ((items.Length > 0) ? items.Contains(q1.c_iteno) : true)
                  && ((cusnos.Length > 0) ? cusnos.Contains(q.c_nosup) : true)
                  select new DSNG_SCMS_QueryReturBeli()
                  {
                    c_fbno = q.c_fbno,
                    d_fbdate = (q.d_fbdate.HasValue ? q.d_fbdate.Value : Functionals.StandardSqlDateTime),
                    c_taxno = q.c_taxno,
                    c_nosup = q.c_nosup,
                    bruto = 
                      (q1.n_gqty.HasValue ? q1.n_gqty.Value : 0) * (q1.n_salpri.HasValue ? q1.n_salpri.Value : 0),
                    DiscVal =
                      ((q1.n_disc.HasValue ? q1.n_disc.Value : 0) / 100) * ((q1.n_gqty.HasValue ? q1.n_gqty.Value : 0) * (q1.n_salpri.HasValue ? q1.n_salpri.Value : 0)),
                    c_iteno = q1.c_iteno,
                    n_qty = (q1.n_gqty.HasValue ? q1.n_gqty.Value : 0) + (q1.n_bqty.HasValue ? q1.n_bqty.Value : 0),
                    n_bea = (q1.n_bea.HasValue ? q1.n_bea.Value : 0)
                  }).Distinct().ToList();

        ListDivAMS = (from q in db.FA_DivAMs
                      join q1 in db.FA_MsDivAMs on q.c_kddivams equals q1.c_kddivams
                      where ((divAmses.Length > 0) ? divAmses.Contains(q.c_kddivams) : true)
                      && ((items.Length > 0) ? items.Contains(q.c_iteno) : true)
                      select new DSNG_SCMS_QueryReturBeli()
                      {
                        c_kddivams = q1.c_kddivams,
                        v_nmdivams = q1.v_nmdivams,
                      }).Distinct().ToList();

        ListFBR.ForEach(delegate(DSNG_SCMS_QueryReturBeli t)
        {
          t.dpp = t.bruto - t.DiscVal - t.n_xdisc;
          t.Invt = t.bruto - t.DiscVal - t.n_xdisc + t.n_bea;
          t.bilva = t.bruto - t.DiscVal - t.n_xdisc + t.n_ppn;
        });

        //ListFBR = (from q in ListFBR
        //          join q1 in db.LG_FBRHs on q.c_fbno equals q1.c_fbno
        //          select new DSNG_SCMS_QueryReturBeli()
        //          {
        //            c_fbno = q1.c_fbno,
        //            d_fbdate = (q1.d_fbdate.HasValue ? q1.d_fbdate.Value : Functionals.StandardSqlDateTime),
        //            c_taxno = q1.c_taxno,
        //            c_nosup = q1.c_nosup,
        //            dpp = q.bruto - q.DiscVal,
        //            n_ppn = (q.bruto - q.DiscVal) / 10,
        //            Invt = (q.bruto - q.DiscVal) + q.n_bea,
        //            bilva = (q.bruto - q.DiscVal) + ((q.bruto - q.DiscVal) / 10)
        //          }).ToList();

        ListSup = (from q in db.LG_DatSups
                  where ((cusnos.Length > 0) ? cusnos.Contains(q.c_nosup) : true)
                  select new DSNG_SCMS_QueryReturBeli()
                  {
                    c_nosup = q.c_nosup,
                    v_nama = q.v_nama,
                  }).Distinct().ToList();

        ListQuery = (from q in ListFBR
                     join q1 in ListDivAMS on q.c_iteno equals q1.c_iteno
                     join q2 in ListSup on q.c_nosup equals q2.c_nosup
                     select new DSNG_SCMS_QueryReturBeli()
                     {
                       c_fbno = q.c_fbno,
                       d_fbdate = q.d_fbdate,
                       c_taxno = q.c_taxno,
                       c_nosup = q.c_nosup,
                       bruto = q.bruto,
                       DiscVal = q.DiscVal,
                       c_iteno = q.c_iteno,
                       n_qty = q.n_qty,
                       n_bea = q.n_bea,
                       dpp = q.dpp,
                       Invt = q.Invt,
                       bilva = q.bilva,
                       c_kddivams = q1.c_kddivams,
                       v_nmdivams = q1.v_nmdivams,
                       v_nama = q2.v_nama,
                     }).ToList();
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales QueryLogic - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      #endregion

      #region Populate

      if (ListQuery != null)
      {
        try
        {
          table = ListQuery.CopyToDataTableObject();

          if (table != null)
          {
            dataSet = new System.Data.DataSet();

            dataSet.Tables.Add(table);
          }
        }
        catch (Exception ex)
        {
          this.IsError = true;
          this.ErrorMessage = ex.Message;

          Logger.WriteLine(
            "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales PopulateDataset - {0}", ex.StackTrace);
          Logger.WriteLine(ex.StackTrace);
        }
      }

      #endregion

      if (ListFBR != null)
      {
        ListFBR.Clear();
      }
      if (ListQuery != null)
      {
        ListQuery.Clear();
      }

      return dataSet;
    }

    public System.Data.DataSet QueryReturJual(ORMDataContext db, Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> dic)
    {
      ScmsSoaLibrary.Commons.Functionals.ParameterParser pp = default(ScmsSoaLibrary.Commons.Functionals.ParameterParser);

      DateTime date1 = DateTime.MinValue,
        date2 = DateTime.MinValue;
      string[] cusnos = null,
        items = null,
        divAmses = null,
        nosup = null ;

      System.Data.DataSet dataSet = null;
      System.Data.DataTable table = null;

      List<DSNG_SCMS_QueryReturJual> ListFJR = null;
      List<DSNG_SCMS_QueryReturJual> ListDivAMS = null;
      List<DSNG_SCMS_QueryReturJual> ListTrans = null;
      List<DSNG_SCMS_QueryReturJual> ListQuery = null;

      #region Parameter

      try
      {

        if (dic.ContainsKey("date1"))
        {
          pp = dic["date1"];
          if (pp.IsSet)
          {
            date1 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("date2"))
        {
          pp = dic["date2"];
          if (pp.IsSet)
          {
            date2 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("cusno"))
        {
          pp = dic["cusno"];
          if (pp.IsSet)
          {
            cusnos = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
        }
        else
        {
          cusnos = new string[0];
        }
        if (dic.ContainsKey("nosup"))
        {
          pp = dic["nosup"];
          if (pp.IsSet)
          {
            nosup = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
        }
        else
        {
          nosup = new string[0];
        }
        if (dic.ContainsKey("item"))
        {
          pp = dic["item"];
          if (pp.IsSet)
          {
            items = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
          else
          {
            items = new string[0];
          }
        }
        else
        {
          items = new string[0];
        }
        if (dic.ContainsKey("divams"))
        {
          pp = dic["divams"];
          if (pp.IsSet)
          {
            divAmses = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
          else
          {
            divAmses = new string[0];
          }
        }
        else
        {
          divAmses = new string[0];
        }
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales Paramter - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      //if (date1.Equals(DateTime.MinValue) || date2.Equals(DateTime.MinValue))
      if (date1.Equals(DateTime.MinValue))
      {
        return null;
      }
      else if (date2.Equals(DateTime.MinValue))
      {
        date2 = date1;
      }

      #endregion

      #region Query

      try
      {
        ListFJR = (from q in db.LG_FJRHs
                   join q1 in db.LG_FJRD1s on q.c_fjno equals q1.c_fjno
                   where ((items.Length > 0) ? items.Contains(q1.c_iteno) : true)
                   && ((cusnos.Length > 0) ? cusnos.Contains(q.c_cusno) : true)
                   && ((q.d_fjdate >= date1) && (q.d_fjdate.Value <= date2))
                   select new DSNG_SCMS_QueryReturJual()
                   {
                     c_fjno = q1.c_fjno,
                     bruto =
                       (q1.n_qty.HasValue ? q1.n_qty.Value : 0) * (q1.n_salpri.HasValue ? q1.n_salpri.Value : 0),
                     DiscVal =
                       ((q1.n_disc.HasValue ? q1.n_disc.Value : 0) / 100) * ((q1.n_qty.HasValue ? q1.n_qty.Value : 0) * (q1.n_salpri.HasValue ? q1.n_salpri.Value : 0)),
                     c_iteno = q1.c_iteno,
                     n_qty = (q1.n_qty.HasValue ? q1.n_qty.Value : 0),
                     c_taxno = q.c_taxno,
                     n_salpri = (q1.n_salpri.HasValue ? q1.n_salpri.Value : 0),
                     d_taxdate = (q.d_taxdate.HasValue ? q.d_taxdate.Value : Functionals.StandardSqlDateTime),
                     d_fjdate = (q.d_fjdate.HasValue ? q.d_fjdate.Value : Functionals.StandardSqlDateTime),
                     c_cusno = q.c_cusno,
                   }).Distinct().ToList();

        ListFJR.ForEach(delegate(DSNG_SCMS_QueryReturJual t)
        {
          t.dpp = t.bruto - t.DiscVal;
          t.n_ppn = (t.c_sektor == "01" ? 0 : ((t.bruto - t.DiscVal) / 10));
          t.bilva = (t.bruto - t.DiscVal) +
                     (t.c_sektor == "01" ? 0 : ((t.bruto - t.DiscVal) / 10));
        });

        ListDivAMS = (from q in db.FA_DivAMs
                      join q1 in db.FA_MsDivAMs on q.c_kddivams equals q1.c_kddivams
                      join q2 in db.FA_MasItms on q.c_iteno equals q2.c_iteno
                      where ((divAmses.Length > 0) ? divAmses.Contains(q1.c_kddivams) : true)
                      && ((items.Length > 0) ? items.Contains(q.c_iteno) : true)
                      && ((nosup.Length > 0) ? nosup.Contains(q2.c_nosup) : true)
                      select new DSNG_SCMS_QueryReturJual()
                     {
                       c_iteno = q.c_iteno,
                       c_kddivams = q1.c_kddivams,
                       v_nmdivams = q1.v_nmdivams
                     }).ToList();

        ListTrans = (from q in db.LG_Cusmas
                     join q1 in db.MsTransDs on new { c_portal = '3', c_notrans = "59", c_type = q.c_sektor } equals new { q1.c_portal, q1.c_notrans, q1.c_type }
                     where ((cusnos.Length > 0) ? cusnos.Contains(q.c_cusno) : true)
                     select new DSNG_SCMS_QueryReturJual()
                     {
                       c_cusno = q.c_cusno,
                       v_cunam = q.v_cunam,
                       c_sektor = q.c_sektor,
                       v_ket = q1.v_ket
                     }).Distinct().ToList();

        ListQuery = (from q in ListFJR
                     join q2 in ListTrans on q.c_cusno equals q2.c_cusno
                     join q3 in ListDivAMS on q.c_iteno equals q3.c_iteno
                     select new DSNG_SCMS_QueryReturJual()
                     {
                       c_fjno = q.c_fjno,
                       bruto = q.bruto,
                       DiscVal = q.DiscVal,
                       c_iteno = q.c_iteno,
                       n_qty = q.n_qty,
                       c_taxno = q.c_taxno,
                       d_taxdate = q.d_taxdate,
                       d_fjdate = q.d_fjdate,
                       c_cusno = q.c_cusno,
                       n_salpri = q.n_salpri,
                       dpp = q.dpp,
                       n_ppn = q.n_ppn,
                       bilva = q.bilva,
                       c_sektor = q2.c_sektor,
                       v_cunam = q2.v_cunam,
                       v_ket = q2.v_ket,
                       c_kddivams = q3.c_kddivams,
                       v_nmdivams = q3.v_nmdivams,
                     }).ToList();

        //ListFJR = (from q in ListFJR
        //           join q1 in db.LG_FJRHs on q.c_fjno equals q1.c_fjno
        //           join q2 in db.LG_Cusmas on q1.c_cusno equals q2.c_cusno
        //           where ((cusnos.Length > 0) ? cusnos.Contains(q.c_cusno) : true)
        //           select new DSNG_SCMS_QueryReturJual()
        //           {
        //             c_taxno = q1.c_taxno,
        //             d_taxdate = (q1.d_taxdate.HasValue ? q1.d_taxdate.Value : Functionals.StandardSqlDateTime),
        //             d_fjdate = (q1.d_fjdate.HasValue ? q1.d_fjdate.Value : Functionals.StandardSqlDateTime),
        //             c_sektor = q2.c_sektor,
        //             v_cunam = q2.v_cunam,
        //             dpp = q.bruto - q.DiscVal,
        //             n_ppn = (q.c_sektor == "01" ? 0 : ((q.bruto - q.DiscVal) / 10)),
        //             bilva = (q.bruto - q.DiscVal) + 
        //                        (q.c_sektor == "01" ? 0 : ((q.bruto - q.DiscVal) / 10)),
        //             c_cusno = q2.c_cusno,
        //           }).Distinct().ToList();

        //ListFJR = (from q in ListFJR
        //           join q1 in db.MsTransDs on q.c_sektor equals q1.c_type
        //           where q1.c_portal == '3' && q1.c_notrans == "59"
        //           select new DSNG_SCMS_QueryReturJual()
        //           {
        //             v_ket = q1.v_ket,
        //           }).ToList();
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales QueryLogic - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      #endregion

      #region Populate

      if (ListQuery != null)
      {
        try
        {
          table = ListQuery.CopyToDataTableObject();

          if (table != null)
          {
            dataSet = new System.Data.DataSet();

            dataSet.Tables.Add(table);
          }
        }
        catch (Exception ex)
        {
          this.IsError = true;
          this.ErrorMessage = ex.Message;

          Logger.WriteLine(
            "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales PopulateDataset - {0}", ex.StackTrace);
          Logger.WriteLine(ex.StackTrace);
        }
      }

      #endregion

      if (ListFJR != null)
      {
        ListFJR.Clear();
      }
      if (ListQuery != null)
      {
        ListQuery.Clear();
      }

      return dataSet;
    }

    public System.Data.DataSet QueryClaim(ORMDataContext db, Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> dic)
    {
      ScmsSoaLibrary.Commons.Functionals.ParameterParser pp = default(ScmsSoaLibrary.Commons.Functionals.ParameterParser);

      DateTime date1 = DateTime.MinValue,
        date2 = DateTime.MinValue;
      string[] cusnos = null,
        items = null,
        divAmses = null,
        divPries = null;
      string rn1 = null, 
        rn2 = null;

      System.Data.DataSet dataSet = null;
      System.Data.DataTable table = null;

      List<DSNG_SCMS_Claim> ListClaim = null;
      List<DSNG_SCMS_Claim> ListDivAMS = null;
      List<DSNG_SCMS_Claim> ListDivPrin = null;
      List<DSNG_SCMS_Claim> ListSup = null;
      List<DSNG_SCMS_Claim> ListQuery = null;

      #region Parameter

      try
      {

        if (dic.ContainsKey("date1"))
        {
          pp = dic["date1"];
          if (pp.IsSet)
          {
            date1 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("date2"))
        {
          pp = dic["date2"];
          if (pp.IsSet)
          {
            date2 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("nosup"))
        {
          pp = dic["nosup"];
          if (pp.IsSet)
          {
            cusnos = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
        }
        else
        {
          cusnos = new string[0];
        }
        if (dic.ContainsKey("item"))
        {
          pp = dic["item"];
          if (pp.IsSet)
          {
            items = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
          else
          {
            items = new string[0];
          }
        }
        else
        {
          items = new string[0];
        }
        if (dic.ContainsKey("divams"))
        {
          pp = dic["divams"];
          if (pp.IsSet)
          {
            divAmses = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
          else
          {
            divAmses = new string[0];
          }
        }
        else
        {
          divAmses = new string[0];
        }
        if (dic.ContainsKey("divpri"))
        {
          pp = dic["divpri"];
          if (pp.IsSet)
          {
            divPries = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
          else
          {
            divPries = new string[0];
          }
        }
        else
        {
          divPries = new string[0];
        }
        if (dic.ContainsKey("RN"))
        {
          pp = dic["RN"];
          if (pp.IsSet)
          {
            rn1 = (pp.Value == null ? string.Empty : (string)pp.Value);
            rn2 = (pp.Value_Next == null ? rn1 : (string)pp.Value_Next);
          }
          else
          {
            rn1 = rn2 = string.Empty;
          }
        }
        else
        {
          divAmses = new string[0];
        }
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales Paramter - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      //if (date1.Equals(DateTime.MinValue) || date2.Equals(DateTime.MinValue))
      if (date1.Equals(DateTime.MinValue))
      {
        return null;
      }
      else if (date2.Equals(DateTime.MinValue))
      {
        date2 = date1;
      }

      #endregion

      #region Query

      //List<DSNG_SCMS_Claim> qryClaim = null;
      //IQueryable<DSNG_SCMS_Claim> qryClaim1 = null;

      try
      {

        //var dd = (from q in db.LG_RNHs
        //         join q6 in db.LG_RND2s on new { q.c_gdg, q.c_rnno } equals new { q6.c_gdg, q6.c_rnno }
        //         where ((items.Length > 0) ? items.Contains(q6.c_iteno) : true)
        //         && (q.d_rndate >= date1 && q.d_rndate <= date2)
        //         && ((cusnos.Length > 0) ? cusnos.Contains(q.c_from) : true)
        //         group new { q, q6 } by new { q.c_gdg, q.c_from, q.c_rnno, q.d_rndate, q6.c_iteno, q6.c_no } into gsum
        //         select new
        //         {
        //           c_gdg = gsum.Key.c_gdg,
        //           c_from = gsum.Key.c_from,
        //           c_rnno = gsum.Key.c_rnno,
        //           d_rndate = (gsum.Key.d_rndate.HasValue ? gsum.Key.d_rndate.Value : Functionals.StandardSqlDateTime),
        //           c_iteno = gsum.Key.c_iteno,
        //           c_no = gsum.Key.c_no,
        //           n_gqty = gsum.Sum(x => x.q6.n_gqty.HasValue ? x.q6.n_gqty.Value : 0)
        //         }).ToList();

        var qClaim = (from q1 in
                      (from q in db.LG_RNHs
                       join q6 in db.LG_RND2s on new { q.c_gdg, q.c_rnno } equals new { q6.c_gdg, q6.c_rnno }
                       where ((items.Length > 0) ? items.Contains(q6.c_iteno) : true)
                       && (q.d_rndate >= date1 && q.d_rndate <= date2)
                       && ((cusnos.Length > 0) ? cusnos.Contains(q.c_from) : true)
                       group new { q, q6 } by new { q.c_gdg, q.c_from, q.c_rnno, q.d_rndate, q6.c_iteno, q6.c_no } into gsum
                       select new
                       {
                         c_gdg = gsum.Key.c_gdg,
                         c_from = gsum.Key.c_from,
                         c_rnno = gsum.Key.c_rnno,
                         d_rndate = (gsum.Key.d_rndate.HasValue ? gsum.Key.d_rndate.Value : Functionals.StandardSqlDateTime),
                         c_iteno = gsum.Key.c_iteno,
                         c_no = gsum.Key.c_no,
                         n_gqty = gsum.Sum(x => x.q6.n_gqty.HasValue ? x.q6.n_gqty.Value : 0)
                       })
                    join q3 in db.LG_ClaimAccHes on q1.c_no equals q3.c_claimaccno
                    join q4 in db.LG_ClaimAccDs on new { q3.c_claimaccno, q1.c_iteno } equals new { q4.c_claimaccno, q4.c_iteno }
                    join q5 in db.LG_ClaimD1s on new { q3.c_claimno, q4.c_iteno } equals new { q5.c_claimno, q5.c_iteno }
                    select new DSNG_SCMS_Claim()
                    {
                      c_from = q1.c_from,
                      c_gdg = q1.c_gdg,
                      c_rnno = q1.c_rnno,
                      d_rndate = q1.d_rndate,
                      c_iteno = q1.c_iteno,
                      c_no = q1.c_no,
                      n_qty = q1.n_gqty,
                      n_disc = (q5.n_disc.HasValue ? q5.n_disc.Value : 0),
                      n_salpri = (q5.n_salpri.HasValue ? q5.n_salpri.Value : 0),
                      c_claimaccno = q3.c_claimaccno,
                    }).AsQueryable();

        ListClaim = qClaim.Between("c_rnno", rn1, rn2).Distinct().ToList();
        
        //ListClaim = qryClaim.Between("c_rnno", rn1, rn2).Distinct().ToList();
        //ListClaim = qryClaim.Distinct().ToList();

        ListDivPrin = (from q in db.FA_Divpris
                       join q1 in db.FA_MsDivPris on q.c_kddivpri equals q1.c_kddivpri
                       where ((divPries.Length > 0) ? divPries.Contains(q1.c_kddivpri) : true)
                       && ((items.Length > 0) ? items.Contains(q.c_iteno) : true)
                       select new DSNG_SCMS_Claim()
                       {
                         c_iteno = q.c_iteno,
                         c_kddivpri = q.c_kddivpri,
                         v_nmdivpri = q1.v_nmdivpri,
                       }).ToList();

        ListSup = (from q in db.LG_DatSups
                   join q1 in db.FA_MasItms on q.c_nosup equals q1.c_nosup
                   select new DSNG_SCMS_Claim()
                   {
                     c_from = q.c_nosup,
                     c_iteno = q1.c_iteno,
                     v_itnam = q1.v_itnam,
                     v_nama = q.v_nama,
                   }).ToList();

        ListDivAMS = (from q in db.FA_DivAMs
                     join q1 in db.FA_MsDivAMs on q.c_kddivams equals q1.c_kddivams
                     where ((divAmses.Length > 0) ? divAmses.Contains(q1.c_kddivams) : true)
                     && ((items.Length > 0) ? items.Contains(q.c_iteno) : true)
                      select new DSNG_SCMS_Claim()
                     {
                       c_iteno = q.c_iteno,
                       c_kddivams = q.c_kddivams,
                       v_nmdivams = q1.v_nmdivams,
                     }).ToList();

        ListQuery = (from q in ListClaim
                     join q1 in ListDivPrin on q.c_iteno equals q1.c_iteno
                     join q2 in ListDivAMS on q.c_iteno equals q2.c_iteno
                     join q3 in ListSup on new { q.c_from, q.c_iteno } equals new { q3.c_from, q3.c_iteno }
                     select new DSNG_SCMS_Claim()
                     {
                       c_from = q.c_from,
                       c_gdg = q.c_gdg,
                       c_rnno = q.c_rnno,
                       d_rndate = q.d_rndate,
                       c_iteno = q.c_iteno,
                       c_no = q.c_no,
                       v_nama = q3.v_nama,
                       v_itnam = q3.v_itnam,
                       n_qty = q.n_qty,
                       n_disc = q.n_disc,
                       n_salpri = q.n_salpri,
                       c_claimaccno = q.c_claimaccno,
                       c_kddivpri = q1.c_kddivpri,
                       v_nmdivpri = q1.v_nmdivpri,
                       c_kddivams = q2.c_kddivams,
                       v_nmdivams = q2.v_nmdivams,
                     }).ToList();

      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales QueryLogic - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      #endregion

      #region Populate

      if (ListQuery != null)
      {
        try
        {
          table = ListQuery.CopyToDataTableObject();

          if (table != null)
          {
            dataSet = new System.Data.DataSet();

            dataSet.Tables.Add(table);
          }
        }
        catch (Exception ex)
        {
          this.IsError = true;
          this.ErrorMessage = ex.Message;

          Logger.WriteLine(
            "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales PopulateDataset - {0}", ex.StackTrace);
          Logger.WriteLine(ex.StackTrace);
        }
      }

      #endregion

      if (ListClaim != null)
      {
        ListClaim.Clear();
      }
      if (ListQuery != null)
      {
        ListQuery.Clear();
      }

      return dataSet;
    }

    public System.Data.DataSet ReportSalKret(ORMDataContext db, Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> dic)
    {
      ScmsSoaLibrary.Commons.Functionals.ParameterParser pp = default(ScmsSoaLibrary.Commons.Functionals.ParameterParser);

      DateTime date1 = DateTime.MinValue,
        date2 = DateTime.MinValue, date3 = DateTime.MinValue;
      //string[] cusnos = null;
      int tahun = 0, bulan = 0;

      System.Data.DataSet dataSet = null;
      System.Data.DataTable table = null;

      List<DSNG_SCMS_SalKre> ListFB = null;
      List<DSNG_SCMS_SalKre> ListDN = null;
      List<DSNG_SCMS_SalKre> ListAdj = null;
      List<DSNG_SCMS_SalKre> ListQuery = null;

      #region Parameter

      try
      {

        if (dic.ContainsKey("date1"))
        {
          pp = dic["date1"];
          if (pp.IsSet)
          {
            date1 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("date2"))
        {
          pp = dic["date2"];
          if (pp.IsSet)
          {
            date2 = (DateTime)pp.Value;
          }
        }
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales Paramter - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      //if (date1.Equals(DateTime.MinValue) || date2.Equals(DateTime.MinValue))
      if (date1.Equals(DateTime.MinValue))
      {
        return null;
      }
      else if (date2.Equals(DateTime.MinValue))
      {
        date2 = date1;
      }

      date3 = date2.AddDays(1);
      tahun = date1.Year;
      bulan = date1.Month;

      #endregion

      #region Query

      try
      {
        var closeFB = (from q in db.LG_CloseFBs
                       join q1 in db.LG_FBHs on q.c_fbno equals q1.c_fbno
                       join q2 in db.LG_DatSups on q1.c_nosup equals q2.c_nosup
                       where q.s_tahun == tahun && q.t_bulan == bulan
                       select new DSNG_SCMS_SalKre()
                       {
                         c_fbno = q.c_fbno,
                         d_fbdate = q1.d_fbdate.Value,
                         v_nama = q2.v_nama,
                         c_nosup = q2.c_nosup,
                         n_sisa = q.n_sisa.Value,
                         n_beli = 0,
                         n_retur = 0
                       }).ToList();

        var closeFBR = (from q in db.LG_CloseFBRs
                        join q1 in db.LG_FBRHs on q.c_fbno equals q1.c_fbno
                        join q2 in db.LG_DatSups on q1.c_nosup equals q2.c_nosup
                        where q.s_tahun == tahun && q.t_bulan == bulan
                        select new DSNG_SCMS_SalKre()
                        {
                          c_fbno = q.c_fbno,
                          d_fbdate = q1.d_fbdate.Value,
                          v_nama = q2.v_nama,
                          c_nosup = q2.c_nosup,
                          n_sisa = q.n_sisa.Value * -1,
                          n_beli = 0,
                          n_retur = q.n_sisa.Value
                        }).ToList();

        var FBH = (from q1 in db.LG_FBHs
                   join q2 in db.LG_DatSups on q1.c_nosup equals q2.c_nosup
                   where (q1.d_fbdate >= date1 && q1.d_fbdate <= date2)
                   select new DSNG_SCMS_SalKre()
                   {
                     c_fbno = q1.c_fbno,
                     d_fbdate = q1.d_fbdate.Value,
                     v_nama = q2.v_nama,
                     c_nosup = q2.c_nosup,
                     n_sisa = q1.n_sisa.Value,
                     n_beli = 0,
                     n_retur = 0
                   }).ToList();

        var FBRH = (from q1 in db.LG_FBRHs
                    join q2 in db.LG_DatSups on q1.c_nosup equals q2.c_nosup
                    where (q1.d_fbdate >= date1 && q1.d_fbdate <= date2)
                    select new DSNG_SCMS_SalKre()
                    {
                      c_fbno = q1.c_fbno,
                      d_fbdate = q1.d_fbdate.Value,
                      v_nama = q2.v_nama,
                      c_nosup = q2.c_nosup,
                      n_sisa = q1.n_sisa.Value,
                      n_beli = 0,
                      n_retur = q1.n_bilva.Value
                    }).ToList();

        var DNH1 = (from q in db.LG_DNHs
                    join q1 in db.LG_DNDs on q.c_noteno equals q1.c_noteno
                    join q2 in db.LG_FBHs on q1.c_fbno equals q2.c_fbno
                    where (q.d_notedate >= date1 && q.d_notedate <= date2)
                    && (q2.d_fbdate < date3) && (q1.c_type == "02")
                    group q1 by new { q1.c_vdno, q1.c_type } into g
                    select new DSNG_SCMS_SalKre()
                    {
                      c_fbnoDN = g.Key.c_vdno,
                      n_bayarDN = (g.Key.c_type == "01" ? g.Sum(x => x.n_value).Value : 0),
                      n_bayar_returDN = (g.Key.c_type == "02" ? g.Sum(x => x.n_value).Value * -1 : 0),
                      c_typeDN = g.Key.c_type
                    }).ToList();

        var DNH2 = (from q in db.LG_DNHs
                    join q1 in db.LG_DNDs on q.c_noteno equals q1.c_noteno
                    join q2 in db.LG_FBHs on q1.c_fbno equals q2.c_fbno
                    where (q.d_notedate >= date1 && q.d_notedate <= date2)
                    && (q2.d_fbdate < date3) && (q1.c_type == "01")
                    select new DSNG_SCMS_SalKre()
                    {
                      c_fbnoDN = q1.c_vdno,
                      n_bayarDN = (q1.c_type == "01" ? q1.n_value.Value : 0),
                      n_bayar_returDN = (q1.c_type == "01" ? q1.n_value.Value * -1 : 0),
                      c_typeDN = q1.c_type
                    }).ToList();

        var adjust = (from q in db.LG_AdjFBHs
                      join q1 in db.LG_AdjFBDs on q.c_adjno equals q1.c_adjno
                      where q.d_adjdate >= date1 && q.d_adjdate <= date2
                      group new { q, q1 } by new { q1.c_fbno, q.c_type } into g
                      select new DSNG_SCMS_SalKre()
                      {
                        c_fbnoAJ = g.Key.c_fbno,
                        n_adjustAJ = (g.Key.c_type == "01" ? ((-g.Sum(x => x.q1.n_value)).Value == null ? 0 : (-g.Sum(x => x.q1.n_value)).Value) : ((g.Sum(x => x.q1.n_value)).Value == null ? 0 : (g.Sum(x => x.q1.n_value)).Value)),
                        c_typeAJ = g.Key.c_type
                      }).ToList();



        ListFB = FBH.Union(FBRH).Union(closeFB).Union(closeFBR).ToList();
        ListDN = DNH1.Union(DNH2).ToList();
        ListAdj = adjust.ToList();

        ListQuery = (from q in ListFB
                     join q1 in ListDN on q.c_fbno equals q1.c_fbnoDN into q1Left
                     from q1S in q1Left.DefaultIfEmpty()
                     join q2 in ListAdj on q.c_fbno equals q2.c_fbnoAJ into q2Left
                     from q2S in q2Left.DefaultIfEmpty()
                     select new DSNG_SCMS_SalKre()
                     {

                       c_fbno = q.c_fbno,
                       d_fbdate = q.d_fbdate,
                       c_nosup = q.c_nosup,
                       v_nama = q.v_nama,
                       n_awal = q.n_sisa,
                       n_beli = q.n_beli,
                       n_retur = q.n_retur,
                       n_bayarDN = q1S == null ? 0m : q1S.n_bayarDN,
                       n_bayar_returDN = q1S == null ? 0m : q1S.n_bayar_returDN,
                       n_adjustAJ = q2S == null ? 0m : q2S.n_adjustAJ,
                       n_sisa = (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                                (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayarDN) +
                            (q1S == null ? 0m : q1S.n_bayar_returDN) + (q2S == null ? 0m : q2S.n_adjustAJ),
                       n_1_30 = (SqlMethods.DateDiffDay(q.d_fbdate, date2) <= 30 ?
                            (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                                (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayarDN) +
                            (q1S == null ? 0m : q1S.n_bayar_returDN) + (q2S == null ? 0m : q2S.n_adjustAJ) : 0),
                       n_31_37 = ((SqlMethods.DateDiffDay(q.d_fbdate, date2) >= 31 && (SqlMethods.DateDiffDay(q.d_fbdate, date2) <= 37)) ?
                          (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                              (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayarDN) +
                          (q1S == null ? 0m : q1S.n_bayar_returDN) + (q2S == null ? 0m : q2S.n_adjustAJ) : 0),
                       n_38_45 = ((SqlMethods.DateDiffDay(q.d_fbdate, date2) >= 38 && (SqlMethods.DateDiffDay(q.d_fbdate, date2) <= 45)) ?
                          (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                              (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayarDN) +
                          (q1S == null ? 0m : q1S.n_bayar_returDN) + (q2S == null ? 0m : q2S.n_adjustAJ) : 0),
                       n_46_60 = ((SqlMethods.DateDiffDay(q.d_fbdate, date2) >= 46 && (SqlMethods.DateDiffDay(q.d_fbdate, date2) <= 60)) ?
                          (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                              (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayarDN) +
                          (q1S == null ? 0m : q1S.n_bayar_returDN) + (q2S == null ? 0m : q2S.n_adjustAJ) : 0),
                       n_61_90 = ((SqlMethods.DateDiffDay(q.d_fbdate, date2) >= 61 && (SqlMethods.DateDiffDay(q.d_fbdate, date2) <= 90)) ?
                           (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                               (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayarDN) +
                           (q1S == null ? 0m : q1S.n_bayar_returDN) + (q2S == null ? 0m : q2S.n_adjustAJ) : 0),
                       n_91_120 = ((SqlMethods.DateDiffDay(q.d_fbdate, date2) >= 91 && (SqlMethods.DateDiffDay(q.d_fbdate, date2) <= 120)) ?
                          (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                              (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayarDN) +
                          (q1S == null ? 0m : q1S.n_bayar_returDN) + (q2S == null ? 0m : q2S.n_adjustAJ) : 0),
                       n_120 = ((SqlMethods.DateDiffDay(q.d_fbdate, date2) > 120) ?
                          (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                              (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayarDN) +
                          (q1S == null ? 0m : q1S.n_bayar_returDN) + (q2S == null ? 0m : q2S.n_adjustAJ) : 0)
                     }).ToList();

      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSalKret QueryLogic - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      #endregion

      #region Populate

      if (ListQuery != null)
      {
        try
        {
          table = ListQuery.CopyToDataTableObject();

          if (table != null)
          {
            dataSet = new System.Data.DataSet();

            dataSet.Tables.Add(table);
          }
        }
        catch (Exception ex)
        {
          this.IsError = true;
          this.ErrorMessage = ex.Message;

          Logger.WriteLine(
            "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSalKret PopulateDataset - {0}", ex.StackTrace);
          Logger.WriteLine(ex.StackTrace);
        }
      }

      #endregion

      if (ListFB != null)
      {
        ListFB.Clear();
      }
      if (ListDN != null)
      {
        ListDN.Clear();
      }
      if (ListAdj != null)
      {
        ListAdj.Clear();
      }

      return dataSet;
    }

    public System.Data.DataSet ReportSalDeb(ORMDataContext db, Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> dic)
    {
      ScmsSoaLibrary.Commons.Functionals.ParameterParser pp = default(ScmsSoaLibrary.Commons.Functionals.ParameterParser);

      DateTime date1 = DateTime.MinValue,
        date2 = DateTime.MinValue, date3 = DateTime.MinValue;
      //string[] cusnos = null;
      int tahun = 0, bulan = 0;

      System.Data.DataSet dataSet = null;
      System.Data.DataTable table = null;

      List<DSNG_SCMS_SalDeb> ListFJ = null;
      List<DSNG_SCMS_SalDeb> ListCN = null;
      List<DSNG_SCMS_SalDeb> ListAdj = null;
      List<DSNG_SCMS_SalDeb> ListQuery = null;

      #region Parameter

      try
      {

        if (dic.ContainsKey("date1"))
        {
          pp = dic["date1"];
          if (pp.IsSet)
          {
            date1 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("date2"))
        {
          pp = dic["date2"];
          if (pp.IsSet)
          {
            date2 = (DateTime)pp.Value;
          }
        }
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales Paramter - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      //if (date1.Equals(DateTime.MinValue) || date2.Equals(DateTime.MinValue))
      if (date1.Equals(DateTime.MinValue))
      {
        return null;
      }
      else if (date2.Equals(DateTime.MinValue))
      {
        date2 = date1;
      }

      date3 = date2.AddDays(1);
      tahun = date1.Year;
      bulan = date1.Month;

      #endregion

      #region Query

      try
      {
        var closeFJ = (from q in db.LG_CloseFJs
                       join q1 in db.LG_FJHs on q.c_fjno equals q1.c_fjno
                       join q2 in db.LG_Cusmas on q1.c_cusno equals q2.c_cusno
                       join q3 in db.MsTransDs on q2.c_sektor equals q3.c_type
                       where (q.s_tahun == tahun && q.t_bulan == bulan)
                       && ((q1.l_delete.HasValue ? q1.l_delete : false) == false)
                       && (q3.c_notrans == "59" && q3.c_portal == '3')
                       select new DSNG_SCMS_SalDeb()
                       {
                         sektor = q3.v_ket,
                         c_fjno = q.c_fjno,
                         d_fjdate = q1.d_fjdate.Value,
                         v_cunam = q2.v_cunam,
                         c_cusno = q2.c_cusno,
                         n_sisa = q.n_sisa.Value,
                         n_beli = 0,
                         n_retur = 0
                       }).ToList();

        var closeFJR = (from q in db.LG_CloseFJRs
                        join q1 in db.LG_FJRHs on q.c_fjno equals q1.c_fjno
                        join q2 in db.LG_Cusmas on q1.c_cusno equals q2.c_cusno
                        join q3 in db.MsTransDs on q2.c_sektor equals q3.c_type
                        where (q.s_tahun == tahun && q.t_bulan == bulan)
                        && ((q1.l_delete.HasValue ? q1.l_delete : false) == false)
                        && (q3.c_notrans == "59" && q3.c_portal == '3')
                        select new DSNG_SCMS_SalDeb()
                        {
                          sektor = q3.v_ket,
                          c_fjno = q.c_fjno,
                          d_fjdate = q1.d_fjdate.Value,
                          v_cunam = q2.v_cunam,
                          c_cusno = q2.c_cusno,
                          n_sisa = q.n_sisa.Value * -1,
                          n_beli = 0,
                          n_retur = 0
                        }).ToList();

        var FJH = (from q1 in db.LG_FJHs
                   join q2 in db.LG_Cusmas on q1.c_cusno equals q2.c_cusno
                   join q3 in db.MsTransDs on q2.c_sektor equals q3.c_type
                   where (q1.d_fjdate >= date1 && q1.d_fjdate <= date2)
                   && ((q1.l_delete.HasValue ? q1.l_delete : false) == false)
                   && (q3.c_notrans == "59" && q3.c_portal == '3')
                   select new DSNG_SCMS_SalDeb()
                   {
                     sektor = q3.v_ket,
                     c_fjno = q1.c_fjno,
                     d_fjdate = q1.d_fjdate.Value,
                     v_cunam = q2.v_cunam,
                     c_cusno = q2.c_cusno,
                     n_sisa = q1.n_sisa.Value,
                     n_beli = 0,
                     n_retur = 0
                   }).ToList();

        var FJRH = (from q1 in db.LG_FJRHs
                    join q2 in db.LG_Cusmas on q1.c_cusno equals q2.c_cusno
                    join q3 in db.MsTransDs on q2.c_sektor equals q3.c_type
                    where (q1.d_fjdate >= date1 && q1.d_fjdate <= date2)
                    && (q3.c_notrans == "59" && q3.c_portal == '3')
                    && ((q1.l_delete.HasValue ? q1.l_delete : false) == false)
                    select new DSNG_SCMS_SalDeb()
                    {
                      sektor = q3.v_ket,
                      c_fjno = q1.c_fjno,
                      d_fjdate = q1.d_fjdate.Value,
                      v_cunam = q2.v_cunam,
                      c_cusno = q2.c_cusno,
                      n_sisa = q1.n_sisa.Value,
                      n_beli = 0,
                      n_retur = q1.n_net.Value
                    }).ToList();

        var CNH1 = (from q in db.LG_CNHs
                    join q1 in db.LG_CNDs on q.c_noteno equals q1.c_noteno
                    join q2 in db.LG_FJHs on q1.c_fjno equals q2.c_fjno
                    where (q.d_notedate >= date1 && q.d_notedate <= date2)
                    && (q2.d_fjdate < date3) && (q1.c_type == "02")
                    && ((q.l_delete.HasValue ? q.l_delete : false) == false)
                    && (q2.l_delete == false)
                    group new { q1 } by new { q1.c_vcno, q1.c_type } into g
                    select new DSNG_SCMS_SalDeb()
                    {
                      c_fjnoDN = g.Key.c_vcno,
                      n_bayar = (g.Key.c_type == "01" ? g.Sum(x => x.q1.n_value).Value : 0),
                      n_bayar_retur = (g.Key.c_type == "02" ? g.Sum(x => x.q1.n_value).Value * -1 : 0),
                      c_typeDN = g.Key.c_type
                    }).ToList();

        var CNH2 = (from q in db.LG_CNHs
                    join q1 in db.LG_CNDs on q.c_noteno equals q1.c_noteno
                    join q2 in db.LG_FJHs on q1.c_fjno equals q2.c_fjno
                    where (q.d_notedate >= date1 && q.d_notedate <= date2)
                    && (q2.d_fjdate < date3) && (q1.c_type == "01")
                    && ((q.l_delete.HasValue ? q.l_delete : false) == false)
                    && (q2.l_delete == false)
                    select new DSNG_SCMS_SalDeb()
                    {
                      c_fjnoDN = q1.c_vcno,
                      n_bayar = (q1.c_type == "01" ? q1.n_value.Value : 0),
                      n_bayar_retur = (q1.c_type == "01" ? q1.n_value.Value * -1 : 0),
                      c_typeDN = q1.c_type
                    }).ToList();

        var adjust = (from q in db.LG_AdjFJHs
                      join q1 in db.LG_AdjFJDs on q.c_adjno equals q1.c_adjno
                      join q2 in db.LG_Cusmas on q.c_cusno equals q2.c_cusno
                      where (q.d_adjdate >= date1 && q.d_adjdate <= date2)
                      group new { q, q1 } by new { q1.c_fjno, q.c_type } into g
                      select new DSNG_SCMS_SalDeb()
                      {
                        c_fjnoAJ = g.Key.c_fjno,
                        n_adjust = (g.Key.c_type == "01" ? ((-g.Sum(x => x.q1.n_value)).Value == null ? 0 : (-g.Sum(x => x.q1.n_value)).Value) : ((g.Sum(x => x.q1.n_value)).Value == null ? 0 : (g.Sum(x => x.q1.n_value)).Value)),
                        c_typeAJ = g.Key.c_type
                      }).ToList();


        ListFJ = FJH.Union(FJRH).Union(closeFJ).Union(closeFJR).ToList();
        ListCN = CNH1.Union(CNH2).ToList();
        ListAdj = adjust.ToList();

        ListQuery = (from q in ListFJ
                     join q1 in ListCN on q.c_fjno equals q1.c_fjnoDN into q1Left
                     from q1S in q1Left.DefaultIfEmpty()
                     join q2 in ListAdj on q.c_fjno equals q2.c_fjnoAJ into q2Left
                     from q2S in q2Left.DefaultIfEmpty()
                     select new DSNG_SCMS_SalDeb()
                     {
                       sektor = q.sektor,
                       c_fjno = q.c_fjno,
                       d_fjdate = q.d_fjdate,
                       c_cusno = q.c_cusno,
                       v_cunam = q.v_cunam,
                       n_awal = (q.n_sisa == null ? 0m : q.n_sisa),
                       n_beli = (q.n_beli == null ? 0m : q.n_beli),
                       n_retur = (q.n_retur == null ? 0m : q.n_retur),
                       n_bayar = (q1S == null ? 0m : q1S.n_bayar),
                       n_bayar_retur = (q1S == null ? 0m : q1S.n_bayar_retur),
                       n_adjust = q2S == null ? 0m : q2S.n_adjust,
                       n_sisa = (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                                (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayar) +
                            (q1S == null ? 0m : q1S.n_bayar_retur) + (q2S == null ? 0m : q2S.n_adjust),
                       n_1_30 = (SqlMethods.DateDiffDay(q.d_fjdate, date2) <= 30 ?
                            (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                                (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayar) +
                            (q1S == null ? 0m : q1S.n_bayar_retur) + (q2S == null ? 0m : q2S.n_adjust) : 0),
                       n_31_37 = ((SqlMethods.DateDiffDay(q.d_fjdate, date2) >= 31 && (SqlMethods.DateDiffDay(q.d_fjdate, date2) <= 37)) ?
                          (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                              (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayar) +
                          (q1S == null ? 0m : q1S.n_bayar_retur) + (q2S == null ? 0m : q2S.n_adjust) : 0),
                       n_38_45 = ((SqlMethods.DateDiffDay(q.d_fjdate, date2) >= 38 && (SqlMethods.DateDiffDay(q.d_fjdate, date2) <= 45)) ?
                          (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                              (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayar) +
                          (q1S == null ? 0m : q1S.n_bayar_retur) + (q2S == null ? 0m : q2S.n_adjust) : 0),
                       n_46_60 = ((SqlMethods.DateDiffDay(q.d_fjdate, date2) >= 46 && (SqlMethods.DateDiffDay(q.d_fjdate, date2) <= 60)) ?
                          (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                              (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayar) +
                          (q1S == null ? 0m : q1S.n_bayar_retur) + (q2S == null ? 0m : q2S.n_adjust) : 0),
                       n_61_90 = ((SqlMethods.DateDiffDay(q.d_fjdate, date2) >= 61 && (SqlMethods.DateDiffDay(q.d_fjdate, date2) <= 90)) ?
                           (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                               (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayar) +
                           (q1S == null ? 0m : q1S.n_bayar_retur) + (q2S == null ? 0m : q2S.n_adjust) : 0),
                       n_91_120 = ((SqlMethods.DateDiffDay(q.d_fjdate, date2) >= 91 && (SqlMethods.DateDiffDay(q.d_fjdate, date2) <= 120)) ?
                          (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                              (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayar) +
                          (q1S == null ? 0m : q1S.n_bayar_retur) + (q2S == null ? 0m : q2S.n_adjust) : 0),
                       n_120 = ((SqlMethods.DateDiffDay(q.d_fjdate, date2) > 120) ?
                          (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                              (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayar) +
                          (q1S == null ? 0m : q1S.n_bayar_retur) + (q2S == null ? 0m : q2S.n_adjust) : 0)
                     }).ToList();
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSalDeb QueryLogic - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      #endregion

      #region Populate

      if (ListQuery != null)
      {
        try
        {
          table = ListQuery.CopyToDataTableObject();

          if (table != null)
          {
            dataSet = new System.Data.DataSet();

            dataSet.Tables.Add(table);
          }
        }
        catch (Exception ex)
        {
          this.IsError = true;
          this.ErrorMessage = ex.Message;

          Logger.WriteLine(
            "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSalDeb PopulateDataset - {0}", ex.StackTrace);
          Logger.WriteLine(ex.StackTrace);
        }
      }

      #endregion

      if (ListFJ != null)
      {
        ListFJ.Clear();
      }
      if (ListCN != null)
      {
        ListCN.Clear();
      }
      if (ListAdj != null)
      {
        ListAdj.Clear();
      }

      return dataSet;
    }

    public System.Data.DataSet ReportBeaKirim(ORMDataContext db, Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> dic)
    {
      ScmsSoaLibrary.Commons.Functionals.ParameterParser pp = default(ScmsSoaLibrary.Commons.Functionals.ParameterParser);

      DateTime date1 = DateTime.MinValue,
        date2 = DateTime.MinValue, date3 = DateTime.MinValue;
      string[] supplier = null,
        items = null,
        divAmses = null;
      int tahun = 0, bulan = 0;

      System.Data.DataSet dataSet = null;
      System.Data.DataTable table = null;

      List<DSNG_SCMS_BEA> FBH = null;
      List<DSNG_SCMS_BEA> DITM = null;
      List<DSNG_SCMS_BEA> ListQuery = null;

      #region Parameter

      try
      {

        if (dic.ContainsKey("date1"))
        {
          pp = dic["date1"];
          if (pp.IsSet)
          {
            date1 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("date2"))
        {
          pp = dic["date2"];
          if (pp.IsSet)
          {
            date2 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("supplier"))
        {
          pp = dic["supplier"];
          if (pp.IsSet)
          {
            supplier = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
          else
          {
            supplier = new string[0];
          }
        }
        else
        {
          supplier = new string[0];
        }
        if (dic.ContainsKey("item"))
        {
          pp = dic["item"];
          if (pp.IsSet)
          {
            items = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
          else
          {
            items = new string[0];
          }
        }
        else
        {
          items = new string[0];
        }
        if (dic.ContainsKey("divams"))
        {
          pp = dic["divams"];
          if (pp.IsSet)
          {
            divAmses = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
          else
          {
            divAmses = new string[0];
          }
        }
        else
        {
          divAmses = new string[0];
        }
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales Paramter - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      //if (date1.Equals(DateTime.MinValue) || date2.Equals(DateTime.MinValue))
      if (date1.Equals(DateTime.MinValue))
      {
        return null;
      }
      else if (date2.Equals(DateTime.MinValue))
      {
        date2 = date1;
      }

      #endregion

      #region Query

      try
      {
          FBH  = (from q in db.LG_FBHs
                  join q1 in db.LG_FBD1s on q.c_fbno equals q1.c_fbno
                  join q2 in db.LG_DatSups on q.c_nosup equals q2.c_nosup
                  where q.d_fbdate >= date1 && q.d_fbdate <= date2
                  && ((items.Length > 0) ? items.Contains(q1.c_iteno) : true)
                  && ((supplier.Length > 0) ? supplier.Contains(q.c_nosup) : true)
                  && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                  && q2.l_import == true
                  select new DSNG_SCMS_BEA()
                  {
                    c_fbno = q.c_fbno,
                    n_bea = (q1.n_bea.HasValue ? q1.n_bea.Value : decimal.Zero),
                    c_iteno = q1.c_iteno,
                    c_nosup = q2.c_nosup,
                    v_nama = q2.v_nama
                  }).ToList();

           DITM  = (from q in db.FA_DivAMs
                    join q1 in db.FA_MsDivAMs on q.c_kddivams equals q1.c_kddivams
                    join q3 in db.FA_MasItms on q.c_iteno equals q3.c_iteno
                    where ((items.Length > 0) ? items.Contains(q.c_iteno) : true)
                    && ((divAmses.Length > 0) ? divAmses.Contains(q1.c_kddivams) : true)
                    select new DSNG_SCMS_BEA()
                    {
                      c_kddivams = q1.c_kddivams,
                      v_nmdivams = q1.v_nmdivams,
                      v_itnam = q3.v_itnam,
                      c_iteno = q3.c_iteno,
                      v_undes = q3.v_undes,
                      n_salpri = q3.n_salpri.Value
                    }).ToList();

        ListQuery = (from q in FBH
                     join q1 in DITM on q.c_iteno equals q1.c_iteno
                     select new DSNG_SCMS_BEA()
                     {

                       c_iteno = q.c_iteno,
                       n_bea = q.n_bea,
                       c_fbno = q.c_fbno,
                       c_nosup = q.c_nosup,
                       v_itnam = q1.v_itnam,
                       v_undes = q1.v_undes,
                       n_salpri = q1.n_salpri,
                       c_kddivams = q1.c_kddivams,
                       v_nmdivams = q1.v_nmdivams,
                       v_nama = q.v_nama
                     }).Distinct().ToList();

      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportBeaKirim QueryLogic - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      #endregion

      #region Populate

      if (ListQuery != null)
      {
        try
        {
          table = ListQuery.CopyToDataTableObject();

          if (table != null)
          {
            dataSet = new System.Data.DataSet();

            dataSet.Tables.Add(table);
          }
        }
        catch (Exception ex)
        {
          this.IsError = true;
          this.ErrorMessage = ex.Message;

          Logger.WriteLine(
            "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportBeaKirim PopulateDataset - {0}", ex.StackTrace);
          Logger.WriteLine(ex.StackTrace);
        }
      }

      #endregion

      if (FBH != null)
      {
        FBH.Clear();
      }
      if (DITM != null)
      {
        DITM.Clear();
      }

      return dataSet;
    }

    public System.Data.DataSet ReportFloating(ORMDataContext db, Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> dic)
    {
      ScmsSoaLibrary.Commons.Functionals.ParameterParser pp = default(ScmsSoaLibrary.Commons.Functionals.ParameterParser);

      DateTime date1 = DateTime.MinValue,
        date2 = DateTime.MinValue, date3 = DateTime.MinValue;
      string[] supplier = null,
        items = null,
        divprin = null;

      System.Data.DataSet dataSet = null;
      System.Data.DataTable table = null;

      List<DSNG_SCMS_FLOATING> ListQuery = null;

      #region Parameter

      try
      {

        if (dic.ContainsKey("date1"))
        {
          pp = dic["date1"];
          if (pp.IsSet)
          {
            date1 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("date2"))
        {
          pp = dic["date2"];
          if (pp.IsSet)
          {
            date2 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("supplier"))
        {
          pp = dic["supplier"];
          if (pp.IsSet)
          {
            supplier = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
          else
          {
            supplier = new string[0];
          }
        }
        else
        {
          supplier = new string[0];
        }
        if (dic.ContainsKey("item"))
        {
          pp = dic["item"];
          if (pp.IsSet)
          {
            items = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
          else
          {
            items = new string[0];
          }
        }
        else
        {
          items = new string[0];
        }
        if (dic.ContainsKey("divprin"))
        {
          pp = dic["divprin"];
          if (pp.IsSet)
          {
            divprin = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
          else
          {
            divprin = new string[0];
          }
        }
        else
        {
          divprin = new string[0];
        }
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales Paramter - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      //if (date1.Equals(DateTime.MinValue) || date2.Equals(DateTime.MinValue))
      if (date1.Equals(DateTime.MinValue))
      {
        return null;
      }
      else if (date2.Equals(DateTime.MinValue))
      {
        date2 = date1;
      }

      #endregion

      #region Query

      try
      {
        ListQuery = (from q in db.LG_RNHs
                     join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                     join q2 in db.LG_DatSups on q.c_from equals q2.c_nosup
                     join q3 in db.FA_MasItms on q1.c_iteno equals q3.c_iteno
                     join q4 in db.FA_Divpris on q1.c_iteno equals q4.c_iteno into q_4
                     from qDP in q_4.DefaultIfEmpty()
                     where ((q.d_rndate >= date1) && (q.d_rndate <= date2))
                      && ((items.Length > 0) ? items.Contains(q1.c_iteno) : true)
                      && ((supplier.Length > 0) ? supplier.Contains(q.c_from) : true)
                      && ((divprin.Length > 0) ? divprin.Contains(qDP.c_kddivpri) : true)
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                     group new { q, q1, q2, q3 } by new
                     {
                       q.c_gdg,
                       q.c_rnno,
                       q.c_from,
                       q2.v_nama,
                       q.d_rndate,
                       q.c_dono,
                       q.d_dodate,
                       q1.c_iteno,
                       q3.v_itnam,
                       q3.v_undes
                     } into g
                     select new DSNG_SCMS_FLOATING()
                     {
                       c_gdg = g.Key.c_gdg,
                       c_rnno = g.Key.c_rnno,
                       c_from = g.Key.c_from,
                       v_nama = g.Key.v_nama,
                       d_rndate = g.Key.d_rndate.Value,
                       c_dono = g.Key.c_dono,
                       d_dodate = g.Key.d_dodate.Value,
                       c_iteno = g.Key.c_iteno,
                       v_itnam = g.Key.v_itnam,
                       v_undes = g.Key.v_undes,
                       n_bqty = (g.Sum(x => x.q1.n_bqty).HasValue ? g.Sum(x => x.q1.n_bqty).Value : 0),
                       n_gqty = (g.Sum(x => x.q1.n_gqty).HasValue ? g.Sum(x => x.q1.n_gqty).Value : 0)
                     }).ToList();

        //int i = ListQuery.Count;
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportFloating QueryLogic - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      #endregion

      #region Populate

      if (ListQuery != null)
      {
        try
        {
          table = ListQuery.CopyToDataTableObject();

          if (table != null)
          {
            dataSet = new System.Data.DataSet();

            dataSet.Tables.Add(table);
          }
        }
        catch (Exception ex)
        {
          this.IsError = true;
          this.ErrorMessage = ex.Message;

          Logger.WriteLine(
            "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportFloating PopulateDataset - {0}", ex.StackTrace);
          Logger.WriteLine(ex.StackTrace);
        }
      }

      #endregion

      if (ListQuery != null)
      {
        ListQuery.Clear();
      }

      return dataSet;
    }

    private List<DSNG_SCMS_KARTU_BARANG> ReportKartuBarang(ORMDataContext db, Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> dic)
    {
      ScmsSoaLibrary.Commons.Functionals.ParameterParser pp = default(ScmsSoaLibrary.Commons.Functionals.ParameterParser);

      string[] supplier = null;

      DateTime date1 = DateTime.MinValue,
        date2 = DateTime.MinValue,
        dateX = DateTime.MinValue,
        datePrev1 = DateTime.MinValue,
        datePrev2 = DateTime.MinValue;

      char gudang = char.MinValue;
      string userId = "SCMS";

      string[] arrItems = new string[0]; //{"4625","3434","3127"};
      string[] arrSupls = new string[0]; //{"00117","00120","00001"};

      List<string> lstTemp = new List<string>();

      List<LG_KB_PROCESS> listStokAwal = new List<LG_KB_PROCESS>(),
        listSATemp = null,
        listSAProsesAwal = null,
        listSAProsesJalan = new List<LG_KB_PROCESS>(),
        listSAProsesJalan1 = null;

      List<CLASS_TEMP_AWAL> listGrupStokAwal = null;

      List<CLASS_MASTER_NAME> listMasterID = new List<CLASS_MASTER_NAME>(),
        listTempMasterID = null;

      List<DSNG_SCMS_KARTU_BARANG> listKartuBarang = null;
      List<CLASS_SCMS_KARTU_BARANG_TEMP> listTempKartuBarang = null;

      #region Parameter

      try
      {
        if (dic.ContainsKey("date1"))
        {
          pp = dic["date1"];
          if (pp.IsSet)
          {
            date1 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("date2"))
        {
          pp = dic["date2"];
          if (pp.IsSet)
          {
            date2 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("supplier"))
        {
          pp = dic["supplier"];
          if (pp.IsSet)
          {
            supplier = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
        }
        else
        {
          supplier = new string[0];
        }
        if (dic.ContainsKey("item"))
        {
          pp = dic["item"];
          if (pp.IsSet)
          {
            arrItems = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
        }
        else
        {
          arrItems = new string[0];
        }
        //if (dic.ContainsKey("divprin"))
        //{
        //  pp = dic["divprin"];
        //  if (pp.IsSet)
        //  {
        //    divprin = (pp.Value == null ? new string[0] : (string[])pp.Value);
        //  }
        //}
        //else
        //{
        //  divprin = new string[0];
        //}
        if (dic.ContainsKey("gudang"))
        {
          pp = dic["gudang"];
          if (pp.IsSet)
          {
            gudang = (pp.Value == null ? char.MinValue : (pp.Value.ToString())[0]);
          }
        }
        else
        {
          gudang = char.MinValue;
        }

        if (dic.ContainsKey("user"))
        {
          pp = dic["user"];
          if (pp.IsSet)
          {
            userId = (pp.Value == null ? string.Empty : pp.Value.ToString().Trim());
          }
        }
        else
        {
          userId = string.Empty;
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportKartuBarang Paramter - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      //if (date1.Equals(DateTime.MinValue) || date2.Equals(DateTime.MinValue) || gudang.Equals(char.MinValue))
      //{
      //  return null;
      //}

      if (date1.Equals(DateTime.MinValue))
      {
        return null;
      }
      else if (date2.Equals(DateTime.MinValue))
      {
        date2 = date1;
      }

      #endregion

      dateX = date1.AddMonths(-1);
      datePrev1 = new DateTime(dateX.Year, dateX.Month, 1);
      datePrev2 = date1.AddDays(-1);

      try
      {
        #region Query

        #region Read All Master

        // Item
        listTempMasterID =
          (from q in db.FA_MasItms
           where ((arrItems.Length > 0) ? arrItems.Contains(q.c_iteno) : true)
            && ((arrSupls.Length > 0) ? arrSupls.Contains(q.c_nosup) : true)
           select new CLASS_MASTER_NAME()
           {
             KeyID = "01",
             CodeID = q.c_iteno,
             DescName = q.v_itnam,
             Custom1 = q.v_undes
           }).ToList();
        listMasterID.AddRange(listTempMasterID.ToArray());
        listTempMasterID.Clear();

        // Gudang
        listTempMasterID =
          (from q in db.LG_MsGudangs
           where (q.c_gdg == gudang)
           select new CLASS_MASTER_NAME()
           {
             KeyID = "02",
             CodeID = (q.c_gdg.Equals(char.MinValue) ? string.Empty : q.c_gdg.ToString()),
             DescName = q.v_gdgdesc,
             Custom1 = q.v_nama
           }).ToList();
        listMasterID.AddRange(listTempMasterID.ToArray());
        listTempMasterID.Clear();

        // Supplier
        listTempMasterID =
          (from q in db.LG_DatSups
           join q1 in db.FA_MasItms on q.c_nosup equals q1.c_nosup into q_1
           from qItem in q_1.DefaultIfEmpty()
           where ((arrSupls.Length > 0) ? arrSupls.Contains(q.c_nosup) : true)
           select new CLASS_MASTER_NAME()
           {
             KeyID = "03",
             CodeID = q.c_nosup,
             DescName = q.v_nama,
             Custom1 = qItem.c_iteno
           }).ToList();
        listMasterID.AddRange(listTempMasterID.ToArray());
        listTempMasterID.Clear();

        // Divisi Prinsipal
        listTempMasterID =
          (from q in db.FA_MsDivPris
           join q1 in db.FA_Divpris on q.c_kddivpri equals q1.c_kddivpri into q_1
           from qDivPri in q_1.DefaultIfEmpty()
           where ((arrSupls.Length > 0) ? arrSupls.Contains(q.c_nosup) : true)
           select new CLASS_MASTER_NAME()
           {
             KeyID = "04",
             CodeID = q.c_kddivpri,
             DescName = q.v_nmdivpri,
             Custom1 = (qDivPri == null ? string.Empty : (qDivPri.c_iteno == null ? string.Empty : qDivPri.c_iteno.Trim()))
           }).ToList();
        listMasterID.AddRange(listTempMasterID.ToArray());
        listTempMasterID.Clear();


        // Cabang
        listTempMasterID =
          (from q in db.LG_Cusmas
           select new CLASS_MASTER_NAME()
           {
             KeyID = "05",
             CodeID = q.c_cusno,
             DescName = q.v_cunam,
             Custom1 = q.c_cab
           }).ToList();
        listMasterID.AddRange(listTempMasterID.ToArray());
        listTempMasterID.Clear();

        // Divisi AMS
        listTempMasterID =
          (from q in db.FA_MsDivAMs
           join q1 in db.FA_DivAMs on q.c_kddivams equals q1.c_kddivams into q_1
           from qDivAms in q_1.DefaultIfEmpty()
           select new CLASS_MASTER_NAME()
           {
             KeyID = "06",
             CodeID = q.c_kddivams,
             DescName = q.v_nmdivams,
             Custom1 = (qDivAms == null ? string.Empty : (qDivAms.c_iteno == null ? string.Empty : qDivAms.c_iteno.Trim()))
           }).ToList();
        listMasterID.AddRange(listTempMasterID.ToArray());
        listTempMasterID.Clear();

        #endregion

        #region Stok Awal

        listSATemp = (from q in db.LG_Stocks
                      join qarrItems in db.FA_MasItms on q.c_iteno equals qarrItems.c_iteno
                      //join qDivAms in db.FA_DivAMs on q.c_iteno equals qDivAms.c_iteno into q_DivAms
                      //from qarrDivAms in q_DivAms.DefaultIfEmpty()
                      //join qDivPri in db.FA_Divpris on q.c_iteno equals qDivPri.c_iteno into q_DivPri
                      //from qarrDivPri in q_DivPri.DefaultIfEmpty()
                      where (q.s_tahun == datePrev1.Year) && (q.t_bulan == datePrev1.Month)
                        && (q.c_gdg == gudang)
                        && ((arrItems.Length > 0) ? arrItems.Contains(q.c_iteno) : true)
                        && ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_nosup) : true)
                      //&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
                      //&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
                      //&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
                      group q by new { q.c_gdg, q.c_no, q.c_iteno, q.c_batch } into g
                      select new LG_KB_PROCESS()
                      {
                        c_gdg = g.Key.c_gdg,
                        c_no = g.Key.c_no,
                        d_date = Functionals.StandardSqlDateTime,
                        c_iteno = g.Key.c_iteno,
                        c_batch = (string.IsNullOrEmpty(g.Key.c_batch) ? "<null>" : g.Key.c_batch.Trim()),
                        n_gqty = g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
                        n_bqty = g.Sum(t => (t.n_bqty.HasValue ? t.n_bqty.Value : 0)),
                        c_cusno = string.Empty
                      }).Distinct().ToList();
        listStokAwal.AddRange(listSATemp.ToArray());
        listSATemp.Clear();

        listSAProsesAwal = (from q in listStokAwal
                            //where ((string.IsNullOrEmpty(q.c_batch) ? string.Empty : q.c_batch.Trim()).Length != 0)
                            //where (string.IsNullOrEmpty(q.c_batch) ? "<null>" : q.c_batch.Trim()).Length > 0
                            group q by new { q.c_gdg, q.c_no, q.c_iteno, q.c_batch } into g
                            select new LG_KB_PROCESS()
                            {
                              c_gdg = g.Key.c_gdg,
                              c_iteno = g.Key.c_iteno,
                              c_no = g.Key.c_no,
                              c_batch = (string.IsNullOrEmpty(g.Key.c_batch) ? "<null>" : g.Key.c_batch.Trim()),
                              n_gqty = g.Sum(x => x.n_gqty),
                              n_bqty = g.Sum(x => x.n_bqty),
                              c_cusno = ""
                            }).Distinct().ToList();

        #endregion

        #region Transaksi Berjalan

        //RN
        lstTemp.Add("05");
        listSATemp = (from q in db.LG_RNHs
                      join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                      join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
                      //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
                      //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
                      //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
                      //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
                      where ((q.d_rndate >= date1) && (q.d_rndate <= date2))
                        && (q.c_gdg == gudang) && (!lstTemp.Contains(q.c_type))
                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                        && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
                        && ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_nosup) : true)
                      //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
                      //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
                      //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
                      group new { q, q1 } by new { q.c_gdg, q.c_rnno, q.d_rndate, q1.c_iteno, q1.c_batch } into g
                      select new LG_KB_PROCESS()
                      {
                        c_gdg = g.Key.c_gdg,
                        c_no = g.Key.c_rnno,
                        d_date = (g.Key.d_rndate.HasValue ? g.Key.d_rndate.Value : Functionals.StandardSqlDateTime),
                        c_iteno = g.Key.c_iteno,
                        c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
                        n_gqty = g.Sum(t => (t.q1.n_gqty.HasValue ? t.q1.n_gqty.Value : 0)),
                        n_bqty = g.Sum(t => (t.q1.n_bqty.HasValue ? t.q1.n_bqty.Value : 0)),
                        c_cusno = string.Empty
                      }).Distinct().ToList();
        listSAProsesJalan.AddRange(listSATemp.ToArray());
        listSATemp.Clear();

        // RN Gudang
        listSATemp = (from q in db.LG_SJHs
                      join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                      join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
                      //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
                      //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
                      //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
                      //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
                      where ((q.d_update >= date1) && (q.d_update <= date2))
                        && (q.c_gdg == gudang) && (q.l_status == true)
                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                        && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
                        && ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_nosup) : true)
                      //						&& ((arrSupls.Length > 0) ? arrSupls.contains(qarrItems.c_iteno) : true)
                      //						&& ((arrDivAms.Length > 0) ? arrDivAms.contains(qarrDivAms.c_iteno) : true)
                      //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.contains(qarrDivPri.c_iteno) : true)
                      group new { q, q1 } by new { q.c_gdg2, q.c_sjno, q.d_update, q1.c_iteno, q1.c_batch } into g
                      select new LG_KB_PROCESS()
                      {
                        c_gdg = (g.Key.c_gdg2.HasValue ? g.Key.c_gdg2.Value : '0'),
                        c_no = g.Key.c_sjno,
                        d_date = (g.Key.d_update.HasValue ? g.Key.d_update.Value : Functionals.StandardSqlDateTime),
                        c_iteno = g.Key.c_iteno,
                        c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
                        n_gqty = g.Sum(t => (t.q1.n_gqty.HasValue ? t.q1.n_gqty.Value : 0)),
                        n_bqty = g.Sum(t => (t.q1.n_bqty.HasValue ? t.q1.n_bqty.Value : 0)),
                        c_cusno = string.Empty
                      }).Distinct().ToList();
        listSAProsesJalan1 = listSAProsesJalan.Union(listSATemp).ToList();
        listSAProsesJalan.Clear();
        listSATemp.Clear();
        listSAProsesJalan.AddRange(listSAProsesJalan1.ToArray());
        listSAProsesJalan1.Clear();

        // Combo In
        listSATemp = (from q in db.LG_ComboHs
                      join qarrItems in db.FA_MasItms on q.c_iteno equals qarrItems.c_iteno
                      //					join qDivAms in db.FA_DivAMS on q.c_iteno equals qDivAms.c_iteno into q_DivAms
                      //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
                      //					join qDivPri in db.FA_Divpris on q.c_iteno equals qDivPri.c_iteno into q_DivPri
                      //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
                      where ((q.d_combodate >= date1) && (q.d_combodate <= date2))
                        && (q.l_confirm == true) && (q.c_type == "01") && (q.c_gdg == gudang)
                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                        && ((arrItems.Length > 0) ? arrItems.Contains(q.c_iteno) : true)
                        && ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_nosup) : true)
                      //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
                      //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
                      //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
                      group q by new { q.c_gdg, q.c_combono, q.d_combodate, q.c_iteno, q.c_batch } into g
                      select new LG_KB_PROCESS()
                      {
                        c_gdg = g.Key.c_gdg,
                        c_no = g.Key.c_combono,
                        d_date = (g.Key.d_combodate.HasValue ? g.Key.d_combodate.Value : Functionals.StandardSqlDateTime),
                        c_iteno = g.Key.c_iteno,
                        c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
                        n_gqty = g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
                        n_bqty = g.Sum(t => (t.n_bqty.HasValue ? t.n_bqty.Value : 0)),
                        c_cusno = string.Empty
                      }).Distinct().ToList();
        listSAProsesJalan1 = listSAProsesJalan.Union(listSATemp).ToList();
        listSAProsesJalan.Clear();
        listSATemp.Clear();
        listSAProsesJalan.AddRange(listSAProsesJalan1.ToArray());
        listSAProsesJalan1.Clear();

        // RC
        listSATemp = (from q in db.LG_RCHes
                      join q1 in db.LG_RCD1s on new { q.c_gdg, q.c_rcno } equals new { q1.c_gdg, q1.c_rcno }
                      join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
                      //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
                      //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
                      //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
                      //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
                      where ((q.d_rcdate >= date1) && (q.d_rcdate <= date2))
                        && (q.c_gdg == gudang)
                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                        && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
                        && ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_nosup) : true)
                      //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
                      //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
                      //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
                      group new { q, q1 } by new { q.c_gdg, q.c_rcno, q.d_rcdate, q1.c_iteno, q1.c_batch, q.c_cusno } into g
                      select new LG_KB_PROCESS()
                      {
                        c_gdg = g.Key.c_gdg,
                        c_no = g.Key.c_rcno,
                        d_date = (g.Key.d_rcdate.HasValue ? g.Key.d_rcdate.Value : Functionals.StandardSqlDateTime),
                        c_iteno = g.Key.c_iteno,
                        c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
                        n_gqty = g.Sum(t => (t.q1.c_type == "01" ? (t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0) : 0)),
                        n_bqty = g.Sum(t => (t.q1.c_type != "01" ? (t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0) : 0)),
                        c_cusno = g.Key.c_cusno
                      }).Distinct().ToList();
        listSAProsesJalan1 = listSAProsesJalan.Union(listSATemp).ToList();
        listSAProsesJalan.Clear();
        listSATemp.Clear();
        listSAProsesJalan.AddRange(listSAProsesJalan1.ToArray());
        listSAProsesJalan1.Clear();

        // RS
        listSATemp = (from q in db.LG_RSHes
                      join q1 in db.LG_RSD1s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                      join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
                      //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
                      //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
                      //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
                      //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
                      where ((q.d_rsdate >= date1) && (q.d_rsdate <= date2))
                        && (q.c_gdg == gudang)
                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                        && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
                        && ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_nosup) : true)
                      //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
                      //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
                      //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
                      group new { q, q1 } by new { q.c_gdg, q.c_rsno, q.d_rsdate, q1.c_iteno, q1.c_batch } into g
                      select new LG_KB_PROCESS()
                      {
                        c_gdg = g.Key.c_gdg,
                        c_no = g.Key.c_rsno,
                        d_date = (g.Key.d_rsdate.HasValue ? g.Key.d_rsdate.Value : Functionals.StandardSqlDateTime),
                        c_iteno = g.Key.c_iteno,
                        c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
                        n_gqty = -g.Sum(t => (t.q1.n_gqty.HasValue ? t.q1.n_gqty.Value : 0)),
                        n_bqty = -g.Sum(t => (t.q1.n_gqty.HasValue ? t.q1.n_gqty.Value : 0)),
                        c_cusno = string.Empty
                      }).Distinct().ToList();
        listSAProsesJalan1 = listSAProsesJalan.Union(listSATemp).ToList();
        listSAProsesJalan.Clear();
        listSATemp.Clear();
        listSAProsesJalan.AddRange(listSAProsesJalan1.ToArray());
        listSAProsesJalan1.Clear();

        // SJ
        listSATemp = (from q in db.LG_SJHs
                      join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                      join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
                      //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
                      //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
                      //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
                      //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
                      where ((q.d_sjdate >= date1) && (q.d_sjdate <= date2))
                        && (q.c_gdg == gudang)
                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                        && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
                        && ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_nosup) : true)
                      //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
                      //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
                      //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
                      group new { q, q1 } by new { q.c_gdg, q.c_sjno, q.d_sjdate, q1.c_iteno, q1.c_batch } into g
                      select new LG_KB_PROCESS()
                      {
                        c_gdg = g.Key.c_gdg,
                        c_no = g.Key.c_sjno,
                        d_date = (g.Key.d_sjdate.HasValue ? g.Key.d_sjdate.Value : Functionals.StandardSqlDateTime),
                        c_iteno = g.Key.c_iteno,
                        c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
                        n_gqty = -g.Sum(t => (t.q1.n_gqty.HasValue ? t.q1.n_gqty.Value : 0)),
                        n_bqty = -g.Sum(t => (t.q1.n_gqty.HasValue ? t.q1.n_gqty.Value : 0)),
                        c_cusno = string.Empty
                      }).Distinct().ToList();
        listSAProsesJalan1 = listSAProsesJalan.Union(listSATemp).ToList();
        listSAProsesJalan.Clear();
        listSATemp.Clear();
        listSAProsesJalan.AddRange(listSAProsesJalan1.ToArray());
        listSAProsesJalan1.Clear();

        // PL
        listSATemp = (from q in db.LG_PLHs
                      join q1 in db.LG_PLD1s on new { q.c_plno } equals new { q1.c_plno }
                      join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
                      //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
                      //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
                      //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
                      //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
                      where ((q.d_pldate >= date1) && (q.d_pldate <= date2))
                        && (q.l_confirm == true) && (q.c_gdg == gudang)
                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                        && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
                        && ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_nosup) : true)
                      //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
                      //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
                      //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
                      group new { q, q1 } by new { q.c_gdg, q.c_plno, q.d_pldate, q1.c_iteno, q1.c_batch, q.c_cusno } into g
                      select new LG_KB_PROCESS()
                      {
                        c_gdg = (g.Key.c_gdg.HasValue ? g.Key.c_gdg.Value : '0'),
                        c_no = g.Key.c_plno,
                        d_date = (g.Key.d_pldate.HasValue ? g.Key.d_pldate.Value : Functionals.StandardSqlDateTime),
                        c_iteno = g.Key.c_iteno,
                        c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
                        n_gqty = -g.Sum(t => (t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0)),
                        n_bqty = 0,
                        c_cusno = g.Key.c_cusno
                      }).Distinct().ToList();
        listSAProsesJalan1 = listSAProsesJalan.Union(listSATemp).ToList();
        listSAProsesJalan.Clear();
        listSATemp.Clear();
        listSAProsesJalan.AddRange(listSAProsesJalan1.ToArray());
        listSAProsesJalan1.Clear();

        // STT
        listSATemp = (from q in db.LG_STHs
                      join q1 in db.LG_STD1s on new { q.c_gdg, q.c_stno } equals new { q1.c_gdg, q1.c_stno }
                      join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
                      //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
                      //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
                      //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
                      //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
                      where ((q.d_stdate >= date1) && (q.d_stdate <= date2))
                        && (q.c_gdg == gudang)
                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                        && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
                        && ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_nosup) : true)
                      //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
                      //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
                      //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
                      group new { q, q1 } by new { q.c_gdg, q.c_stno, q.d_stdate, q1.c_iteno, q1.c_batch } into g
                      select new LG_KB_PROCESS()
                      {
                        c_gdg = g.Key.c_gdg,
                        c_no = g.Key.c_stno,
                        d_date = (g.Key.d_stdate.HasValue ? g.Key.d_stdate.Value : Functionals.StandardSqlDateTime),
                        c_iteno = g.Key.c_iteno,
                        c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
                        n_gqty = -g.Sum(t => (t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0)),
                        n_bqty = 0,
                        c_cusno = string.Empty
                      }).Distinct().ToList();
        listSAProsesJalan1 = listSAProsesJalan.Union(listSATemp).ToList();
        listSAProsesJalan.Clear();
        listSATemp.Clear();
        listSAProsesJalan.AddRange(listSAProsesJalan1.ToArray());
        listSAProsesJalan1.Clear();

        // Combo
        listSATemp = (from q in db.LG_ComboHs
                      join q1 in db.LG_ComboD1s on new { q.c_gdg, q.c_combono } equals new { q1.c_gdg, q1.c_combono }
                      join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
                      //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
                      //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
                      //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
                      //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
                      where ((q.d_combodate >= date1) && (q.d_combodate <= date2))
                        && (q.c_gdg == gudang) && (q.l_confirm == true)
                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                        && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
                        && ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_nosup) : true)
                      //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
                      //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
                      //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
                      group new { q, q1 } by new { q.c_gdg, q.c_combono, q.d_combodate, q1.c_iteno, q1.c_batch } into g
                      select new LG_KB_PROCESS()
                      {
                        c_gdg = g.Key.c_gdg,
                        c_no = g.Key.c_combono,
                        d_date = (g.Key.d_combodate.HasValue ? g.Key.d_combodate.Value : Functionals.StandardSqlDateTime),
                        c_iteno = g.Key.c_iteno,
                        c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
                        n_gqty = -g.Sum(t => (t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0)),
                        n_bqty = 0,
                        c_cusno = string.Empty
                      }).Distinct().ToList();
        listSAProsesJalan1 = listSAProsesJalan.Union(listSATemp).ToList();
        listSAProsesJalan.Clear();
        listSATemp.Clear();
        listSAProsesJalan.AddRange(listSAProsesJalan1.ToArray());
        listSAProsesJalan1.Clear();

        // Adjust
        listSATemp = (from q in db.LG_AdjustHs
                      join q1 in db.LG_AdjustD1s on new { q.c_gdg, q.c_adjno } equals new { q1.c_gdg, q1.c_adjno }
                      join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
                      //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
                      //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
                      //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
                      //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
                      where ((q.d_adjdate >= date1) && (q.d_adjdate <= date2))
                        && (q.c_gdg == gudang)
                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                        && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
                        && ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_nosup) : true)
                      //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
                      //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
                      //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
                      group new { q, q1 } by new { q.c_gdg, q.c_adjno, q.d_adjdate, q1.c_iteno, q1.c_batch } into g
                      select new LG_KB_PROCESS()
                      {
                        c_gdg = g.Key.c_gdg,
                        c_no = g.Key.c_adjno,
                        d_date = (g.Key.d_adjdate.HasValue ? g.Key.d_adjdate.Value : Functionals.StandardSqlDateTime),
                        c_iteno = g.Key.c_iteno,
                        c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
                        n_gqty = g.Sum(t => (t.q1.n_gqty.HasValue ? t.q1.n_gqty.Value : 0)),
                        n_bqty = g.Sum(t => (t.q1.n_bqty.HasValue ? t.q1.n_bqty.Value : 0)),
                        c_cusno = string.Empty
                      }).Distinct().ToList();
        listSAProsesJalan1 = listSAProsesJalan.Union(listSATemp).ToList();
        listSAProsesJalan.Clear();
        listSATemp.Clear();
        listSAProsesJalan.AddRange(listSAProsesJalan1.ToArray());
        listSAProsesJalan1.Clear();

        #endregion

        #region Combine Data

        listGrupStokAwal = listSAProsesAwal
          .GroupBy(t => new
          {
            t.c_gdg,
            t.c_iteno,
            t.c_batch
          })
          .Select(x => new CLASS_TEMP_AWAL()
          {
            c_gdg = x.Key.c_gdg,
            c_iteno = x.Key.c_iteno,
            c_batch = x.Key.c_batch,
            n_gawal = x.Sum(y => y.n_gqty),
            n_bawal = x.Sum(y => y.n_bqty)
          }).ToList();

        listTempKartuBarang = listGrupStokAwal.FullJoin(listSAProsesJalan,
          x => new KartuBarangEqualKey() { c_gdg = x.c_gdg, c_iteno = x.c_iteno, c_batch = x.c_batch },
          y => new KartuBarangEqualKey() { c_gdg = y.c_gdg, c_iteno = y.c_iteno, c_batch = y.c_batch },
          (kiri, kanan) => new CLASS_SCMS_KARTU_BARANG_TEMP()
          {
            c_gdg = kiri.c_gdg,
            c_iteno = kiri.c_iteno,
            c_batch = (string.IsNullOrEmpty(kiri.c_batch) ? string.Empty : kiri.c_batch.Trim()),
            n_gawal = kiri.n_gawal,
            n_bawal = kiri.n_bawal,
            n_count = 0
          },
          (kiri, kanan) => new CLASS_SCMS_KARTU_BARANG_TEMP()
          {
            c_gdg = kiri.c_gdg,
            c_iteno = kiri.c_iteno,
            c_batch = (string.IsNullOrEmpty(kiri.c_batch) ? string.Empty : kiri.c_batch.Trim()),
            n_gawal = kiri.n_gawal,
            n_bawal = kiri.n_bawal,
            n_count = (kanan == null ? 0 : kanan.Count())
          },
          t => (t.n_count < 1),
          (kanan, kiri) => new CLASS_SCMS_KARTU_BARANG_TEMP()
          {
            c_gdg = kanan.c_gdg,
            c_iteno = kanan.c_iteno,
            c_batch = (string.IsNullOrEmpty(kanan.c_batch) ? string.Empty : kanan.c_batch.Trim()),
            n_gawal = 0,
            n_bawal = 0,
            c_no = kanan.c_no,
            d_date = kanan.d_date,
            n_gqty = kanan.n_gqty,
            n_bqty = kanan.n_bqty,
            c_cusno = (string.IsNullOrEmpty(kanan.c_cusno) ? string.Empty : kanan.c_cusno.Trim()),
            n_count = (kiri == null ? 0 : kiri.Count())
          },
          t => (t.n_count < 1), new KartuBarangEqualComparer()).ToList();

        listKartuBarang = (from q in listTempKartuBarang
                           join q1 in listMasterID on new { KeyID = "01", CodeID = q.c_iteno } equals new { q1.KeyID, q1.CodeID } into q_1
                           from qlItem in q_1.DefaultIfEmpty()
                           join q2 in listMasterID on new { KeyID = "02", CodeID = q.c_gdg.ToString() } equals new { q2.KeyID, q2.CodeID } into q_2
                           from qlGudang in q_2.DefaultIfEmpty()
                           join q3 in listMasterID on new { KeyID = "03", CodeID = q.c_iteno } equals new { q3.KeyID, CodeID = q3.Custom1 } into q_3
                           from qlSupplier in q_3.DefaultIfEmpty()
                           join q4 in listMasterID on new { KeyID = "04", CodeID = q.c_iteno } equals new { q4.KeyID, CodeID = q4.Custom1 } into q_4
                           from qlDivSupplier in q_4.DefaultIfEmpty()
                           join q5 in listMasterID on new { KeyID = "05", CodeID = q.c_cusno } equals new { q5.KeyID, q5.CodeID } into q_5
                           from qlCabang in q_5.DefaultIfEmpty()
                           join q6 in listMasterID on new { KeyID = "06", CodeID = q.c_iteno } equals new { q6.KeyID, CodeID = q6.Custom1 } into q_6
                           from qlDivAms in q_6.DefaultIfEmpty()
                           select new DSNG_SCMS_KARTU_BARANG()
                           {
                             c_gdg = q.c_gdg,
                             v_gdgdesc = (qlGudang == null ? string.Empty : (string.IsNullOrEmpty(qlGudang.DescName) ? string.Empty : qlGudang.DescName.Trim())),
                             c_iteno = q.c_iteno,
                             v_item_nama = (qlItem == null ? string.Empty : (string.IsNullOrEmpty(qlItem.DescName) ? string.Empty : qlItem.DescName.Trim())),
                             item_undes = (qlItem == null ? string.Empty : (string.IsNullOrEmpty(qlItem.Custom1) ? string.Empty : qlItem.Custom1.Trim())),
                             c_batch = q.c_batch,
                             d_expired = Functionals.StandardSqlDateTime,
                             n_gawal = q.n_gawal,
                             n_bawal = q.n_bawal,
                             c_no = q.c_no,
                             d_date = q.d_date,
                             n_gqty = q.n_gqty,
                             n_bqty = q.n_bqty,
                             c_cusno = q.c_cusno,
                             v_cunam = (qlCabang == null ? string.Empty : (string.IsNullOrEmpty(qlCabang.CodeID) ? string.Empty : qlCabang.DescName.Trim())),
                             c_nosup = (qlSupplier == null ? string.Empty : (string.IsNullOrEmpty(qlSupplier.CodeID) ? string.Empty : qlSupplier.CodeID.Trim())),
                             v_nama = (qlSupplier == null ? string.Empty : (string.IsNullOrEmpty(qlSupplier.DescName) ? string.Empty : qlSupplier.DescName.Trim())),
                             c_kddivpri = (qlDivSupplier == null ? string.Empty : (string.IsNullOrEmpty(qlDivSupplier.CodeID) ? string.Empty : qlDivSupplier.CodeID.Trim())),
                             v_nmdivpri = (qlDivSupplier == null ? string.Empty : (string.IsNullOrEmpty(qlDivSupplier.DescName) ? string.Empty : qlDivSupplier.DescName.Trim())),
                             c_kddivams = (qlDivAms == null ? string.Empty : (string.IsNullOrEmpty(qlDivAms.CodeID) ? string.Empty : qlDivAms.CodeID.Trim())),
                             v_nmdivams = (qlDivAms == null ? string.Empty : (string.IsNullOrEmpty(qlDivAms.DescName) ? string.Empty : qlDivAms.DescName.Trim())),
                             c_user = userId
                           }).ToList();

        #endregion

        //listTempBatchItem
        //listKartuBarang

        #endregion
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportKartuBarang QueryLogic - {0}", ex.StackTrace);
        Logger.WriteLine(ex.StackTrace);
      }

      #region Clear

      if (listStokAwal != null)
      {
        listStokAwal.Clear();
      }
      if (listSATemp != null)
      {
        listSATemp.Clear();
      }
      if (listSAProsesAwal != null)
      {
        listSAProsesAwal.Clear();
      }
      if (listSAProsesJalan != null)
      {
        listSAProsesJalan.Clear();
      }
      if (listSAProsesJalan1 != null)
      {
        listSAProsesJalan1.Clear();
      }
      if (listTempMasterID != null)
      {
        listTempMasterID.Clear();
      }
      if (listMasterID != null)
      {
        listMasterID.Clear();
      }
      if (listTempKartuBarang != null)
      {
        listTempKartuBarang.Clear();
      }

      #endregion

      return listKartuBarang;
    }

    public System.Data.DataSet ReportKartuBarangSwitch(ScmsModel.ORMDataContext db, Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> dic, ModeKodeBarangSwitcher Mode)
    {
      System.Data.DataSet dataSet = null;
      System.Data.DataTable table1 = null;

      List<DSNG_SCMS_KARTU_BARANG> lists = null,
        lstTemp1 = null, lstTemp2 = null;
      
      List<BatchItemData> listBatchItem = null,
        listTempAllBatchItem = null, listTempDataBatchItem = null;

      List<BatchItemGetData> listTempBatchItem = null;

      BatchItemData bid = null;

      lists = ReportKartuBarang(db, dic);

      if (lists.Count > 0)
      {
        switch (Mode)
        {
          case ModeKodeBarangSwitcher.KartuBarangBatch:
            {
              #region KB BATCH

              if ((lists != null) && (lists.Count > 0))
              {
                lstTemp2 = (from q in
                              (from sq in lists
                               group sq by new { sq.c_gdg, sq.c_iteno, sq.c_batch, sq.d_expired, sq.d_date, sq.c_user } into g
                               select new LG_KB_PROCESS()
                               {
                                 c_gdg = g.Key.c_gdg,
                                 c_iteno = g.Key.c_iteno,
                                 c_batch = g.Key.c_batch,
                                 d_date = g.Key.d_expired,
                                 c_cusno = g.Key.c_user,
                                 n_gawal = g.Sum(t => t.n_gawal),
                                 n_bawal = g.Sum(t => t.n_bawal),
                               })
                            join q1 in
                              (from sq in lists
                               where ((sq.n_gqty != 0) || (sq.n_bqty != 0))
                               group sq by new { sq.c_gdg, sq.c_iteno, sq.c_user, sq.c_batch, sq.d_expired, sq.c_no, sq.d_date, sq.c_cusno, sq.v_cunam, sq.item_undes, sq.c_nosup, sq.v_nama, sq.c_kddivpri, sq.v_nmdivpri, sq.v_item_nama, sq.v_gdgdesc } into g
                               select new DSNG_SCMS_KARTU_BARANG()
                               {
                                 c_gdg = g.Key.c_gdg,
                                 c_iteno = g.Key.c_iteno,
                                 v_item_nama = g.Key.v_item_nama,
                                 item_undes = g.Key.item_undes,
                                 c_no = g.Key.c_no,
                                 d_date = g.Key.d_date,
                                 c_user = g.Key.c_user,
                                 c_cusno = g.Key.c_cusno,
                                 v_cunam = g.Key.v_cunam,
                                 c_nosup = g.Key.c_nosup,
                                 v_nama = g.Key.v_nama,
                                 c_kddivpri = g.Key.c_kddivpri,
                                 v_nmdivpri = g.Key.v_nmdivpri,
                                 v_gdgdesc = g.Key.v_gdgdesc,
                                 n_gqty = g.Sum(t => t.n_gqty),
                                 n_bqty = g.Sum(t => t.n_bqty),
                                 c_batch = g.Key.c_batch,
                                 d_expired = g.Key.d_expired,
                               }) on new { q.c_gdg, q.c_iteno, c_user = q.c_cusno, q.c_batch } equals new { q1.c_gdg, q1.c_iteno, q1.c_user, q1.c_batch } into q_1
                            from qDSND in q_1.DefaultIfEmpty()
                            select new DSNG_SCMS_KARTU_BARANG()
                            {
                              c_gdg = q.c_gdg,
                              c_iteno = q.c_iteno,
                              n_gawal = q.n_gawal,
                              n_bawal = q.n_bawal,
                              c_user = (qDSND == null ? string.Empty : (string.IsNullOrEmpty(qDSND.c_user) ? string.Empty : qDSND.c_user.Trim())),
                              c_no = (qDSND == null ? string.Empty : (string.IsNullOrEmpty(qDSND.c_no) ? string.Empty : qDSND.c_no.Trim())),
                              d_date = (qDSND == null ? Functionals.StandardSqlDateTime : qDSND.d_date),
                              n_gqty = (qDSND == null ? 0 : qDSND.n_gqty),
                              n_bqty = (qDSND == null ? 0 : qDSND.n_bqty),
                              v_cunam = (qDSND == null ? string.Empty : (string.IsNullOrEmpty(qDSND.v_cunam) ? string.Empty : qDSND.v_cunam.Trim())),
                              item_undes = (qDSND == null ? string.Empty : (string.IsNullOrEmpty(qDSND.item_undes) ? string.Empty : qDSND.item_undes.Trim())),
                              c_nosup = (qDSND == null ? string.Empty : (string.IsNullOrEmpty(qDSND.c_nosup) ? string.Empty : qDSND.c_nosup.Trim())),
                              v_nama = (qDSND == null ? string.Empty : (string.IsNullOrEmpty(qDSND.v_nama) ? string.Empty : qDSND.v_nama.Trim())),
                              c_kddivpri = (qDSND == null ? string.Empty : (string.IsNullOrEmpty(qDSND.c_kddivpri) ? string.Empty : qDSND.c_kddivpri.Trim())),
                              v_nmdivpri = (qDSND == null ? string.Empty : (string.IsNullOrEmpty(qDSND.v_nmdivpri) ? string.Empty : qDSND.v_nmdivpri.Trim())),
                              v_item_nama = (qDSND == null ? string.Empty : (string.IsNullOrEmpty(qDSND.v_item_nama) ? string.Empty : qDSND.v_item_nama.Trim())),
                              v_gdgdesc = (qDSND == null ? string.Empty : (string.IsNullOrEmpty(qDSND.v_gdgdesc) ? string.Empty : qDSND.v_gdgdesc.Trim())),
                              c_batch = (qDSND == null ? string.Empty : (string.IsNullOrEmpty(qDSND.c_batch) ? string.Empty : qDSND.c_batch.Trim())),
                              d_expired = (qDSND == null ? DateTime.MinValue : qDSND.d_expired),
                            }).ToList();

                #region Batch Info

                listTempAllBatchItem = lstTemp2
                  .GroupBy(t => new { t.c_iteno, t.c_batch })
                  .Select(x => new BatchItemData()
                  {
                    c_iteno = x.Key.c_iteno,
                    c_batch = x.Key.c_batch
                  }).ToList();

                listTempBatchItem = new List<BatchItemGetData>();
                listBatchItem = new List<BatchItemData>();

                for (int nLoop = 0, nCount = 0; nLoop < listTempAllBatchItem.Count; nLoop++, nCount++)
                {
                  if (nCount >= 500)
                  {
                    listTempDataBatchItem = (from q in db.LG_MsBatches
                                             where listTempBatchItem.Contains(new BatchItemGetData()
                                             {
                                               c_iteno = q.c_iteno,
                                               c_batch = q.c_batch
                                             })
                                             select new BatchItemData()
                                             {
                                               c_iteno = (q.c_iteno == null ? string.Empty : q.c_iteno.Trim()),
                                               c_batch = (q.c_batch == null ? string.Empty : q.c_batch.Trim()),
                                               d_expired = (q.d_expired.HasValue ? q.d_expired.Value : Functionals.StandardSqlDateTime)
                                             }).ToList();

                    listBatchItem.AddRange(listTempDataBatchItem.ToArray());

                    listTempDataBatchItem.Clear();
                    listTempBatchItem.Clear();
                    nCount = 0;
                  }
                  else
                  {
                    bid = listTempAllBatchItem[nLoop];

                    listTempBatchItem.Add(new BatchItemGetData()
                    {
                      c_iteno = bid.c_iteno,
                      c_batch = bid.c_batch
                    });
                  }
                }

                lstTemp1 = (from q in lstTemp2
                            join q1 in listBatchItem on new { q.c_iteno, q.c_batch } equals new { q1.c_iteno, q1.c_batch } into q_1
                            from qBat in q_1.DefaultIfEmpty()
                            select new DSNG_SCMS_KARTU_BARANG()
                            {
                              c_gdg = q.c_gdg,
                              c_iteno = q.c_iteno,
                              n_gawal = q.n_gawal,
                              n_bawal = q.n_bawal,
                              c_user = q.c_user,
                              c_no = q.c_no,
                              d_date = q.d_date,
                              n_gqty = q.n_gqty,
                              n_bqty = q.n_bqty,
                              v_cunam = q.v_cunam,
                              item_undes = q.item_undes,
                              c_nosup = q.c_nosup,
                              v_nama = q.v_nama,
                              c_kddivpri = q.c_kddivpri,
                              v_nmdivpri = q.v_nmdivpri,
                              v_item_nama = q.v_item_nama,
                              v_gdgdesc = q.v_gdgdesc,
                              c_batch = q.c_batch,
                              d_expired = (qBat == null ? DateTime.MinValue : qBat.d_expired),
                            }).ToList();

                #endregion

                table1 = new System.Data.DataTable("DSNG_SCMS_KARTU_BARANG_BATCH");

                lstTemp1.CopyToDataTableObject(table1);

                lstTemp1.Clear();

                lstTemp2.Clear();

                if (table1 != null)
                {
                  dataSet = new System.Data.DataSet();

                  dataSet.Tables.Add(table1);
                }
              }

              #endregion
            }
            break;
          case ModeKodeBarangSwitcher.KartuBarangTotal:
            {
              #region KB TOTAL

              if ((lists != null) && (lists.Count > 0))
              {
                lstTemp1 = (from q in
                              (from sq in lists
                               group sq by new { sq.c_gdg, sq.c_iteno, sq.c_user } into g
                               select new LG_KB_PROCESS()
                               {
                                 c_gdg = g.Key.c_gdg,
                                 c_iteno = g.Key.c_iteno,
                                 c_cusno = g.Key.c_user,
                                 n_gawal = g.Sum(t => t.n_gawal),
                                 n_bawal = g.Sum(t => t.n_bawal)
                               })
                            join q1 in
                              (from sq in lists
                               where ((sq.n_gqty != 0) || (sq.n_bqty != 0))
                               group sq by new { sq.c_gdg, sq.c_iteno, sq.c_user, sq.c_no, sq.d_date, sq.c_cusno, sq.v_cunam, sq.item_undes, sq.c_nosup, sq.v_nama, sq.c_kddivpri, sq.v_nmdivpri, sq.v_item_nama, sq.v_gdgdesc } into g
                               select new DSNG_SCMS_KARTU_BARANG()
                               {
                                 c_gdg = g.Key.c_gdg,
                                 c_iteno = g.Key.c_iteno,
                                 v_item_nama = g.Key.v_item_nama,
                                 item_undes = g.Key.item_undes,
                                 c_no = g.Key.c_no,
                                 d_date = g.Key.d_date,
                                 c_user = g.Key.c_user,
                                 c_cusno = g.Key.c_cusno,
                                 v_cunam = g.Key.v_cunam,
                                 c_nosup = g.Key.c_nosup,
                                 v_nama = g.Key.v_nama,
                                 c_kddivpri = g.Key.c_kddivpri,
                                 v_nmdivpri = g.Key.v_nmdivpri,
                                 v_gdgdesc = g.Key.v_gdgdesc,
                                 n_gqty = g.Sum(t => t.n_gqty),
                                 n_bqty = g.Sum(t => t.n_bqty),
                               }) on new { q.c_gdg, q.c_iteno, c_user = q.c_cusno } equals new { q1.c_gdg, q1.c_iteno, q1.c_user } into q_1
                            from qDSND in q_1.DefaultIfEmpty()
                            select new DSNG_SCMS_KARTU_BARANG()
                            {
                              c_gdg = q.c_gdg,
                              c_iteno = q.c_iteno,
                              n_gawal = q.n_gawal,
                              n_bawal = q.n_bawal,
                              c_user = (qDSND == null ? string.Empty : (string.IsNullOrEmpty(qDSND.c_user) ? string.Empty : qDSND.c_user.Trim())),
                              c_no = (qDSND == null ? string.Empty : (string.IsNullOrEmpty(qDSND.c_no) ? string.Empty : qDSND.c_no.Trim())),
                              d_date = (qDSND == null ? Functionals.StandardSqlDateTime : qDSND.d_date),
                              n_gqty = (qDSND == null ? 0 : qDSND.n_gqty),
                              n_bqty = (qDSND == null ? 0 : qDSND.n_bqty),
                              v_cunam = (qDSND == null ? string.Empty : (string.IsNullOrEmpty(qDSND.v_cunam) ? string.Empty : qDSND.v_cunam.Trim())),
                              item_undes = (qDSND == null ? string.Empty : (string.IsNullOrEmpty(qDSND.item_undes) ? string.Empty : qDSND.item_undes.Trim())),
                              c_nosup = (qDSND == null ? string.Empty : (string.IsNullOrEmpty(qDSND.c_nosup) ? string.Empty : qDSND.c_nosup.Trim())),
                              v_nama = (qDSND == null ? string.Empty : (string.IsNullOrEmpty(qDSND.v_nama) ? string.Empty : qDSND.v_nama.Trim())),
                              c_kddivpri = (qDSND == null ? string.Empty : (string.IsNullOrEmpty(qDSND.c_kddivpri) ? string.Empty : qDSND.c_kddivpri.Trim())),
                              v_nmdivpri = (qDSND == null ? string.Empty : (string.IsNullOrEmpty(qDSND.v_nmdivpri) ? string.Empty : qDSND.v_nmdivpri.Trim())),
                              v_item_nama = (qDSND == null ? string.Empty : (string.IsNullOrEmpty(qDSND.v_item_nama) ? string.Empty : qDSND.v_item_nama.Trim())),
                              v_gdgdesc = (qDSND == null ? string.Empty : (string.IsNullOrEmpty(qDSND.v_gdgdesc) ? string.Empty : qDSND.v_gdgdesc.Trim())),
                            }).ToList();

                table1 = new System.Data.DataTable("DSNG_SCMS_KARTU_BARANG_BATCH");

                lstTemp1.CopyToDataTableObject(table1);

                lstTemp1.Clear();

                if (table1 != null)
                {
                  dataSet = new System.Data.DataSet();

                  dataSet.Tables.Add(table1);
                }
              }

              #endregion
            }
            break;
        }
      }
      else
      {
        this.ErrorMessage = "Data not found.";

        this.IsError = true;
      }

      if (lists != null)
      {
        lists.Clear();
      }
      if (listBatchItem != null)
      {
        listBatchItem.Clear();
      }
      if (listTempBatchItem != null)
      {
        listTempBatchItem.Clear();
      }
      if (listTempDataBatchItem != null)
      {
        listTempDataBatchItem.Clear();
      }

      return dataSet;
    }

    public System.Data.DataSet ReportPembayaran(ORMDataContext db, Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> dic)
    {
      ScmsSoaLibrary.Commons.Functionals.ParameterParser pp = default(ScmsSoaLibrary.Commons.Functionals.ParameterParser);

      DateTime date1 = DateTime.MinValue,
        date2 = DateTime.MinValue;
      string[] supplier = null;
      string fb1 = null, fb2 = null;
      int fb12 = 0, fb22 = 0;

      System.Data.DataSet dataSet = null;
      System.Data.DataTable table = null;

      List<DSNG_SCMS_PEMBAYARAN> FBH = null;
      List<DSNG_SCMS_PEMBAYARAN> ListQuery = null;

      #region Parameter

      try
      {

        if (dic.ContainsKey("date1"))
        {
          pp = dic["date1"];
          if (pp.IsSet)
          {
            date1 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("date2"))
        {
          pp = dic["date2"];
          if (pp.IsSet)
          {
            date2 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("fb1"))
        {
          pp = dic["fb1"];
          if (pp.IsSet)
          {
            fb1 = (string)pp.Value;
          }
        }
        if (dic.ContainsKey("fb2"))
        {
          pp = dic["fb2"];
          if (pp.IsSet)
          {
            fb2 = (string)pp.Value;
          }
        }
        if (dic.ContainsKey("supplier"))
        {
          pp = dic["supplier"];
          if (pp.IsSet)
          {
            supplier = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
          else
          {
            supplier = new string[0];
          }
        }
        else
        {
          supplier = new string[0];
        }
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportPembayaran Paramter - {0}", ex.StackTrace);
      }

      //if (date1.Equals(DateTime.MinValue) || date2.Equals(DateTime.MinValue))
      if (date1.Equals(DateTime.MinValue))
      {
        return null;
      }
      else if (date2.Equals(DateTime.MinValue))
      {
        date2 = date1;
      }

      #endregion

      #region Query

      try
      {

        ListQuery = (from q in
                       (from q in db.LG_FBHs
                        join q1 in db.LG_DatSups on q.c_nosup equals q1.c_nosup
                        select new
                        {
                          c_nosup = q.c_nosup,
                          v_nama = q1.v_nama,
                          c_fb = q.c_fb,
                          c_fbno = q.c_fbno,
                          d_fbdate = q.d_fbdate.HasValue ? q.d_fbdate.Value : DateTime.Now,
                          n_bilva = q.n_bilva.HasValue ? q.n_bilva.Value : 0m,
                          n_top = q.n_top.HasValue ? q.n_top.Value : 0m,
                          d_top = q.d_top.HasValue ? q.d_top.Value : DateTime.Now,
                          n_sisa = q.n_sisa.HasValue ? q.n_sisa.Value : 0m
                        })
                     join q1 in
                       (from q in db.LG_DNHs
                        join q1 in db.LG_DNDs on q.c_noteno equals q1.c_noteno
                        select new
                        {
                          c_notenoVD = q.c_noteno,
                          d_notedateVD = q.d_notedate.HasValue ? q.d_notedate.Value : DateTime.Today,
                          n_bilvaVD = q.n_bilva.HasValue ? q.n_bilva.Value : 0m,
                          c_vdnoVD = q1.c_vdno,
                          c_fbnoVD = q1.c_fbno,
                          n_valueVD = q1.n_value.HasValue ? q1.n_value.Value : 0m
                        }) on q.c_fbno equals q1.c_fbnoVD into g
                     from gLeft in g.DefaultIfEmpty()
                     where ((supplier.Length > 0) ? supplier.Contains(q.c_nosup) : true)
                     && (q.d_fbdate >= date1) && (q.d_fbdate <= date2)
                     select new DSNG_SCMS_PEMBAYARAN()
                     {
                       c_nosup = q.c_nosup,
                       v_nama = q.v_nama,
                       c_fb = q.c_fb,
                       c_fbno = q.c_fbno,
                       d_fbdate = q.d_fbdate,
                       n_bilva = q.n_bilva,
                       n_top = q.n_top,
                       d_top = q.d_top,
                       n_sisa = q.n_sisa,
                       c_notenoVD = gLeft.c_notenoVD == null ? string.Empty : gLeft.c_notenoVD,
                       d_notedateVD = gLeft.d_notedateVD == null ? DateTime.Today : gLeft.d_notedateVD,
                       n_bilvaVD = gLeft == null ? 0m : gLeft.n_bilvaVD,
                       c_vdnoVD = gLeft == null ? string.Empty : gLeft.c_vdnoVD,
                       c_fbnoVD = gLeft == null ? string.Empty : gLeft.c_fbnoVD,
                       n_valueVD = gLeft == null ? 0m : gLeft.n_valueVD
                     }).ToList();
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales PopulateDataset - {0}", ex.StackTrace);
      }

      #endregion

      #region Populate

      if (ListQuery != null)
      {
        try
        {
          table = ListQuery.CopyToDataTableObject();

          if (table != null)
          {
            dataSet = new System.Data.DataSet();

            dataSet.Tables.Add(table);
          }
        }
        catch (Exception ex)
        {
          this.IsError = true;
          this.ErrorMessage = ex.Message;

          Logger.WriteLine(
            "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales PopulateDataset - {0}", ex.StackTrace);
        }
      }

      #endregion

      if (ListQuery != null)
      {
        ListQuery.Clear();
      }

      return dataSet;
    }

    public System.Data.DataSet ReportJatuhTempo(ORMDataContext db, Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> dic)
    {
      ScmsSoaLibrary.Commons.Functionals.ParameterParser pp = default(ScmsSoaLibrary.Commons.Functionals.ParameterParser);

      DateTime date1 = DateTime.MinValue,
        date2 = DateTime.MinValue, date3 = DateTime.MinValue;
      string[] supplier = null;

      System.Data.DataSet dataSet = null;
      System.Data.DataTable table = null;

      List<DSNG_SCMS_PEMBAYARAN> ListQuery = null;

      #region Parameter

      try
      {

        if (dic.ContainsKey("date1"))
        {
          pp = dic["date1"];
          if (pp.IsSet)
          {
            date1 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("date2"))
        {
          pp = dic["date2"];
          if (pp.IsSet)
          {
            date2 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("supplier"))
        {
          pp = dic["supplier"];
          if (pp.IsSet)
          {
            supplier = (pp.Value == null ? new string[0] : (string[])pp.Value);
          }
          else
          {
            supplier = new string[0];
          }
        }
        else
        {
          supplier = new string[0];
        }
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportJatuhTempo Paramter - {0}", ex.StackTrace);
      }

      //if (date1.Equals(DateTime.MinValue) || date2.Equals(DateTime.MinValue))
      if (date1.Equals(DateTime.MinValue))
      {
        return null;
      }
      else if (date2.Equals(DateTime.MinValue))
      {
        date2 = date1;
      }

      #endregion

      #region Query

      try
      {

        ListQuery = (from q in
                       (from q in db.LG_FBHs
                        join q1 in db.LG_DatSups on q.c_nosup equals q1.c_nosup
                        select new
                        {
                          c_nosup = q.c_nosup,
                          v_nama = q1.v_nama,
                          c_fb = q.c_fb,
                          c_fbno = q.c_fbno,
                          d_fbdate = q.d_fbdate.HasValue ? q.d_fbdate.Value : DateTime.Now,
                          n_bilva = q.n_bilva.HasValue ? q.n_bilva.Value : 0m,
                          n_top = q.n_top.HasValue ? q.n_top.Value : 0m,
                          d_top = q.d_top.HasValue ? q.d_top.Value : DateTime.Now,
                          n_sisa = q.n_sisa.HasValue ? q.n_sisa.Value : 0m
                        })
                     join q1 in
                       (from q in db.LG_DNHs
                        join q1 in db.LG_DNDs on q.c_noteno equals q1.c_noteno
                        select new
                        {
                          c_notenoVD = q.c_noteno,
                          d_notedateVD = q.d_notedate.HasValue ? q.d_notedate.Value : DateTime.Today,
                          n_bilvaVD = q.n_bilva.HasValue ? q.n_bilva.Value : 0m,
                          c_vdnoVD = q1.c_vdno,
                          c_fbnoVD = q1.c_fbno,
                          n_valueVD = q1.n_value.HasValue ? q1.n_value.Value : 0m
                        }) on q.c_fbno equals q1.c_fbnoVD into g
                     from gLeft in g.DefaultIfEmpty()
                     where ((supplier.Length > 0) ? supplier.Contains(q.c_nosup) : true)
                     && (q.d_top >= date1) && (q.d_top <= date2)
                     select new DSNG_SCMS_PEMBAYARAN()
                     {
                       c_nosup = q.c_nosup,
                       v_nama = q.v_nama,
                       c_fb = q.c_fb,
                       c_fbno = q.c_fbno,
                       d_fbdate = q.d_fbdate,
                       n_bilva = q.n_bilva,
                       n_top = q.n_top,
                       d_top = q.d_top,
                       n_sisa = q.n_sisa,
                       c_notenoVD = gLeft.c_notenoVD == null ? string.Empty : gLeft.c_notenoVD,
                       d_notedateVD = gLeft.d_notedateVD == null ? DateTime.Today : gLeft.d_notedateVD,
                       n_bilvaVD = gLeft == null ? 0m : gLeft.n_bilvaVD,
                       c_vdnoVD = gLeft == null ? string.Empty : gLeft.c_vdnoVD,
                       c_fbnoVD = gLeft == null ? string.Empty : gLeft.c_fbnoVD,
                       n_valueVD = gLeft == null ? 0m : gLeft.n_valueVD
                     }).ToList();
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportJatuhTempo PopulateDataset - {0}", ex.StackTrace);
      }

      #endregion

      #region Populate

      if (ListQuery != null)
      {
        try
        {
          table = ListQuery.CopyToDataTableObject();

          if (table != null)
          {
            dataSet = new System.Data.DataSet();

            dataSet.Tables.Add(table);
          }
        }
        catch (Exception ex)
        {
          this.IsError = true;
          this.ErrorMessage = ex.Message;

          Logger.WriteLine(
            "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportJatuhTempo PopulateDataset - {0}", ex.StackTrace);
        }
      }

      #endregion

      if (ListQuery != null)
      {
        ListQuery.Clear();
      }

      return dataSet;
    }

    //public System.Data.DataSet ReportPBF(ORMDataContext db, Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> dic)
    //{
    //  ScmsSoaLibrary.Commons.Functionals.ParameterParser pp = default(ScmsSoaLibrary.Commons.Functionals.ParameterParser);

    //  System.Data.DataSet dataSet = null;
    //  System.Data.DataTable table = null;

    //  List<LG_KB_PROCESS> listStokAwal = new List<LG_KB_PROCESS>(),
    //    listSATemp = null;

    //  List<DSNG_SCMS_LGPBF> listTempPBF = null,
    //    ListQuery = null;

    //  CLASS_TEMP_STOCK ts = null;

    //  List<CLASS_TEMP_STOCK> lstTempStock = null;

    //  List<CLASS_TEMP_TRANSKSI> lstTempTrx = null;

    //  List<string> lstTemp = new List<string>();

    //  char gudang = char.MinValue;
    //  string tipePeriode = "??",
    //    tmp1 = null,
    //    tmp2 = null;
    //  int thnAktif = 0;

    //  DateTime date1 = DateTime.MinValue,
    //    date2 = DateTime.MinValue,
    //    dateX = DateTime.MinValue,
    //    datePrev1 = DateTime.MinValue,
    //    datePrev2 = DateTime.MinValue;

    //  #region Parameter

    //  try
    //  {
    //    if (dic.ContainsKey("gudang"))
    //    {
    //      pp = dic["gudang"];
    //      if (pp.IsSet)
    //      {
    //        gudang = (char)pp.Value;
    //      }
    //    }
    //    if (dic.ContainsKey("periode"))
    //    {
    //      pp = dic["periode"];
    //      if (pp.IsSet)
    //      {
    //        tipePeriode = (string)pp.Value;
    //      }
    //    }
    //    if (dic.ContainsKey("tahun"))
    //    {
    //      pp = dic["tahun"];
    //      if (pp.IsSet)
    //      {
    //        thnAktif = (int)pp.Value;
    //      }
    //    }
    //    //if (dic.ContainsKey("supplier"))
    //    //{
    //    //  pp = dic["supplier"];
    //    //  if (pp.IsSet)
    //    //  {
    //    //    supplier = (pp.Value == null ? new string[0] : (string[])pp.Value);
    //    //  }
    //    //  else
    //    //  {
    //    //    supplier = new string[0];
    //    //  }
    //    //}
    //    //else
    //    //{
    //    //  supplier = new string[0];
    //    //}
    //  }
    //  catch (Exception ex)
    //  {
    //    this.IsError = true;
    //    this.ErrorMessage = ex.Message;

    //    Logger.WriteLine(
    //      "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportPBF Paramter - {0}", ex.StackTrace);
    //  }

    //  date1 = new DateTime(thnAktif,
    //    (tipePeriode.Equals("01") ? 1 :
    //      (tipePeriode.Equals("02") ? 4 :
    //        (tipePeriode.Equals("03") ? 7 : 10))),
    //        1);
    //  date2 = date1.AddMonths(3);

    //  dateX = date1.AddMonths(-1);
    //  datePrev1 = new DateTime(date1.Year, date1.Month, 1);
    //  datePrev2 = date1.AddDays(-1);

    //  #endregion

    //  #region Query

    //  try
    //  {
    //    var qryItemsAktif = (from q in db.FA_MasItms
    //                         where ((q.l_aktif.HasValue ? q.l_aktif.Value : false) == true)
    //                          //&& ((q.l_hide.HasValue ? q.l_hide.Value : false) == false)
    //                          && ((q.l_dinkes.HasValue ? q.l_dinkes.Value : false) == true)
    //                          && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //                         select q.c_iteno).AsQueryable();

    //    #region Awal

    //    // Stok Awal
    //    listSATemp = (from q in db.LG_Stocks
    //                  where (q.c_gdg == gudang)
    //                    && (q.s_tahun == dateX.Year) && (q.t_bulan == dateX.Month)
    //                    && qryItemsAktif.Contains(q.c_iteno)
    //                  group q by new { q.c_gdg, q.c_no, q.c_iteno } into g
    //                  select new LG_KB_PROCESS()
    //                  {
    //                    c_gdg = g.Key.c_gdg,
    //                    c_no = g.Key.c_no,
    //                    d_date = Functionals.StandardSqlDateTime,
    //                    c_iteno = g.Key.c_iteno,
    //                    n_gqty = g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
    //                    n_bqty = g.Sum(t => (t.n_bqty.HasValue ? t.n_bqty.Value : 0)),
    //                  }).Distinct().ToList();
    //    listStokAwal.AddRange(listSATemp.ToArray());
    //    listSATemp.Clear();

    //    #region Old Coded

    //    //// RN
    //    //listSATemp = (from q in db.LG_RNHs
    //    //              join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
    //    //              where (q.c_gdg == gudang) && (q.c_type != "05")
    //    //                && ((q.d_rndate >= datePrev1) && (q.d_rndate <= datePrev2))
    //    //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //    //                && qryItemsAktif.Contains(q1.c_iteno)
    //    //              group q1 by new { q.c_gdg, q.c_rnno, q.d_rndate, q1.c_iteno } into g
    //    //              select new LG_KB_PROCESS()
    //    //              {
    //    //                c_gdg = g.Key.c_gdg,
    //    //                c_no = g.Key.c_rnno,
    //    //                d_date = (g.Key.d_rndate.HasValue ? g.Key.d_rndate.Value : Functionals.StandardSqlDateTime),
    //    //                c_iteno = g.Key.c_iteno,
    //    //                n_gqty = g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
    //    //                n_bqty = g.Sum(t => (t.n_bqty.HasValue ? t.n_bqty.Value : 0)),
    //    //              }).Distinct().ToList();
    //    //listStokAwal.AddRange(listSATemp.ToArray());
    //    //listSATemp.Clear();

    //    //// RN Gudang
    //    //listSATemp = (from q in db.LG_SJHs
    //    //              join q1 in db.LG_SJD2s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
    //    //              where (q.c_gdg2 == gudang) && (q.l_status == true)
    //    //                && ((q.d_update >= datePrev1) && (q.d_update <= datePrev2))
    //    //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //    //                && qryItemsAktif.Contains(q1.c_iteno)
    //    //              group q1 by new { q.c_gdg2, q.c_sjno, q.d_update, q1.c_iteno } into g
    //    //              select new LG_KB_PROCESS()
    //    //              {
    //    //                c_gdg = (g.Key.c_gdg2.HasValue ? g.Key.c_gdg2.Value : '0'),
    //    //                c_no = g.Key.c_sjno,
    //    //                d_date = (g.Key.d_update.HasValue ? g.Key.d_update.Value : Functionals.StandardSqlDateTime),
    //    //                c_iteno = g.Key.c_iteno,
    //    //                n_gqty = g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
    //    //                n_bqty = g.Sum(t => (t.n_bqty.HasValue ? t.n_bqty.Value : 0)),
    //    //              }).Distinct().ToList();
    //    //listStokAwal.AddRange(listSATemp.ToArray());
    //    //listSATemp.Clear();

    //    //// Combo
    //    //listSATemp = (from q in db.LG_ComboHs
    //    //              where (q.c_gdg == gudang) && (q.c_type == "01")
    //    //                && ((q.l_confirm.HasValue ? q.l_confirm.Value : false) == true)
    //    //                && ((q.d_combodate >= datePrev1) && (q.d_combodate <= datePrev2))
    //    //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //    //                && qryItemsAktif.Contains(q.c_iteno)
    //    //              group q by new { q.c_gdg, q.c_combono, q.d_combodate, q.c_iteno } into g
    //    //              select new LG_KB_PROCESS()
    //    //              {
    //    //                c_gdg = g.Key.c_gdg,
    //    //                c_no = g.Key.c_combono,
    //    //                d_date = (g.Key.d_combodate.HasValue ? g.Key.d_combodate.Value : Functionals.StandardSqlDateTime),
    //    //                c_iteno = g.Key.c_iteno,
    //    //                n_gqty = g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
    //    //                n_bqty = g.Sum(t => (t.n_bqty.HasValue ? t.n_bqty.Value : 0)),
    //    //              }).Distinct().ToList();
    //    //listStokAwal.AddRange(listSATemp.ToArray());
    //    //listSATemp.Clear();

    //    //// RC
    //    //listSATemp = (from q in db.LG_RCHes
    //    //              join q1 in db.LG_RCD1s on new { q.c_gdg, q.c_rcno } equals new { q1.c_gdg, q1.c_rcno }
    //    //              where (q.c_gdg == gudang)
    //    //                && ((q.d_rcdate >= datePrev1) && (q.d_rcdate <= datePrev2))
    //    //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //    //                && qryItemsAktif.Contains(q1.c_iteno)
    //    //              group q1 by new { q.c_gdg, q.c_rcno, q.d_rcdate, q1.c_iteno } into g
    //    //              select new LG_KB_PROCESS()
    //    //              {
    //    //                c_gdg = g.Key.c_gdg,
    //    //                c_no = g.Key.c_rcno,
    //    //                d_date = (g.Key.d_rcdate.HasValue ? g.Key.d_rcdate.Value : Functionals.StandardSqlDateTime),
    //    //                c_iteno = g.Key.c_iteno,
    //    //                n_gqty = g.Sum(t => (t.c_type == "01" ? (t.n_qty.HasValue ? t.n_qty.Value : 0) : 0)),
    //    //                n_bqty = g.Sum(t => (t.c_type != "01" ? (t.n_qty.HasValue ? t.n_qty.Value : 0) : 0)),
    //    //              }).Distinct().ToList();
    //    //listStokAwal.AddRange(listSATemp.ToArray());
    //    //listSATemp.Clear();

    //    //// RS
    //    //listSATemp = (from q in db.LG_RSHes
    //    //              join q1 in db.LG_RSD1s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
    //    //              where (q.c_gdg == gudang)
    //    //                && ((q.d_rsdate >= datePrev1) && (q.d_rsdate <= datePrev2))
    //    //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //    //                && qryItemsAktif.Contains(q1.c_iteno)
    //    //              group q1 by new { q.c_gdg, q.c_rsno, q.d_rsdate, q1.c_iteno } into g
    //    //              select new LG_KB_PROCESS()
    //    //              {
    //    //                c_gdg = g.Key.c_gdg,
    //    //                c_no = g.Key.c_rsno,
    //    //                d_date = (g.Key.d_rsdate.HasValue ? g.Key.d_rsdate.Value : Functionals.StandardSqlDateTime),
    //    //                c_iteno = g.Key.c_iteno,
    //    //                n_gqty = -g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
    //    //                n_bqty = -g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
    //    //              }).Distinct().ToList();
    //    //listStokAwal.AddRange(listSATemp.ToArray());
    //    //listSATemp.Clear();

    //    //// SJ
    //    //listSATemp = (from q in db.LG_SJHs
    //    //              join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
    //    //              where (q.c_gdg == gudang)
    //    //                 && ((q.d_sjdate >= datePrev1) && (q.d_sjdate <= datePrev2))
    //    //                 && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //    //                 && qryItemsAktif.Contains(q1.c_iteno)
    //    //              group q1 by new { q.c_gdg, q.c_sjno, q.d_sjdate, q1.c_iteno } into g
    //    //              select new LG_KB_PROCESS()
    //    //              {
    //    //                c_gdg = g.Key.c_gdg,
    //    //                c_no = g.Key.c_sjno,
    //    //                d_date = (g.Key.d_sjdate.HasValue ? g.Key.d_sjdate.Value : Functionals.StandardSqlDateTime),
    //    //                c_iteno = g.Key.c_iteno,
    //    //                n_gqty = -g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
    //    //                n_bqty = -g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
    //    //              }).Distinct().ToList();
    //    //listStokAwal.AddRange(listSATemp.ToArray());
    //    //listSATemp.Clear();

    //    //// PL
    //    //listSATemp = (from q in db.LG_PLHs
    //    //              join q1 in db.LG_PLD1s on q.c_plno equals q1.c_plno
    //    //              where (q.c_gdg == gudang) && (q.l_confirm == true)
    //    //                && ((q.d_pldate >= datePrev1) && (q.d_pldate <= datePrev2))
    //    //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //    //                && qryItemsAktif.Contains(q1.c_iteno)
    //    //              group q1 by new { q.c_gdg, q.c_plno, q.d_pldate, q1.c_iteno } into g
    //    //              select new LG_KB_PROCESS()
    //    //              {
    //    //                c_gdg = (g.Key.c_gdg.HasValue ? g.Key.c_gdg.Value : '0'),
    //    //                c_no = g.Key.c_plno,
    //    //                d_date = (g.Key.d_pldate.HasValue ? g.Key.d_pldate.Value : Functionals.StandardSqlDateTime),
    //    //                c_iteno = g.Key.c_iteno,
    //    //                n_gqty = -g.Sum(t => (t.n_qty.HasValue ? t.n_qty.Value : 0)),
    //    //                n_bqty = 0,
    //    //              }).Distinct().ToList();
    //    //listStokAwal.AddRange(listSATemp.ToArray());
    //    //listSATemp.Clear();

    //    //// STT
    //    //listSATemp = (from q in db.LG_STHs
    //    //              join q1 in db.LG_STD1s on new { q.c_gdg, q.c_stno } equals new { q1.c_gdg, q1.c_stno }
    //    //              where (q.c_gdg == gudang)
    //    //                && ((q.d_stdate >= datePrev1) && (q.d_stdate <= datePrev2))
    //    //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //    //                && qryItemsAktif.Contains(q1.c_iteno)
    //    //              group q1 by new { q.c_gdg, q.c_stno, q.d_stdate, q1.c_iteno } into g
    //    //              select new LG_KB_PROCESS()
    //    //              {
    //    //                c_gdg = g.Key.c_gdg,
    //    //                c_no = g.Key.c_stno,
    //    //                d_date = (g.Key.d_stdate.HasValue ? g.Key.d_stdate.Value : Functionals.StandardSqlDateTime),
    //    //                c_iteno = g.Key.c_iteno,
    //    //                n_gqty = -g.Sum(t => (t.n_qty.HasValue ? t.n_qty.Value : 0)),
    //    //                n_bqty = 0,
    //    //              }).Distinct().ToList();
    //    //listStokAwal.AddRange(listSATemp.ToArray());
    //    //listSATemp.Clear();

    //    //// Combo Detail
    //    //listSATemp = (from q in db.LG_ComboHs
    //    //              join q1 in db.LG_ComboD1s on new { q.c_gdg, q.c_combono } equals new { q1.c_gdg, q1.c_combono }
    //    //              where (q.c_gdg == gudang) && (q.l_confirm == true)
    //    //                && ((q.d_combodate >= datePrev1) && (q.d_combodate <= datePrev2))
    //    //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //    //                && qryItemsAktif.Contains(q1.c_iteno)
    //    //              group q1 by new { q.c_gdg, q.c_combono, q.d_combodate, q1.c_iteno, q1.c_batch } into g
    //    //              select new LG_KB_PROCESS()
    //    //              {
    //    //                c_gdg = g.Key.c_gdg,
    //    //                c_no = g.Key.c_combono,
    //    //                d_date = (g.Key.d_combodate.HasValue ? g.Key.d_combodate.Value : Functionals.StandardSqlDateTime),
    //    //                c_iteno = g.Key.c_iteno,
    //    //                n_gqty = -g.Sum(t => (t.n_qty.HasValue ? t.n_qty.Value : 0)),
    //    //                n_bqty = 0,
    //    //              }).Distinct().ToList();
    //    //listStokAwal.AddRange(listSATemp.ToArray());
    //    //listSATemp.Clear();

    //    //// Adjustment
    //    //listSATemp = (from q in db.LG_AdjustHs
    //    //              join q1 in db.LG_AdjustD1s on new { q.c_gdg, q.c_adjno } equals new { q1.c_gdg, q1.c_adjno }
    //    //              where (q.c_gdg == gudang)
    //    //                && ((q.d_adjdate >= datePrev1) && (q.d_adjdate <= datePrev2))
    //    //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //    //                && qryItemsAktif.Contains(q1.c_iteno)
    //    //              group q1 by new { q.c_gdg, q.c_adjno, q.d_adjdate, q1.c_iteno, q1.c_batch } into g
    //    //              select new LG_KB_PROCESS()
    //    //              {
    //    //                c_gdg = g.Key.c_gdg,
    //    //                c_no = g.Key.c_adjno,
    //    //                d_date = (g.Key.d_adjdate.HasValue ? g.Key.d_adjdate.Value : Functionals.StandardSqlDateTime),
    //    //                c_iteno = g.Key.c_iteno,
    //    //                c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
    //    //                n_gqty = g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
    //    //                n_bqty = g.Sum(t => (t.n_bqty.HasValue ? t.n_bqty.Value : 0)),
    //    //                c_cusno = string.Empty
    //    //              }).Distinct().ToList();
    //    //listStokAwal.AddRange(listSATemp.ToArray());
    //    //listSATemp.Clear();

    //    #endregion

    //    #endregion

    //    lstTempStock = listStokAwal.GroupBy(t => t.c_iteno)
    //      .Select(x => new CLASS_TEMP_STOCK()
    //      {
    //        c_gdg = gudang,
    //        c_iteno = x.Key,
    //        n_gawal = x.Sum(y => y.n_gqty),
    //        n_bawal = x.Sum(y => y.n_bqty)
    //      }).ToList();
    //    listStokAwal.Clear();

    //    #region Transksi

    //    #region RN Beli

    //    lstTemp.Add("01");
    //    lstTemp.Add("06");
    //    lstTempTrx = (from q in db.LG_RNHs
    //                  join q1 in db.LG_RND2s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
    //                  where (q.c_gdg == gudang) && lstTemp.Contains(q.c_type)
    //                    && ((q.d_rndate >= date1) && (q.d_rndate < date2))
    //                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //                    && qryItemsAktif.Contains(q1.c_iteno)
    //                  group q1 by new { q1.c_iteno, q1.c_type } into g
    //                  select new CLASS_TEMP_TRANSKSI()
    //                  {
    //                    c_gdg = gudang,
    //                    c_iteno = (g.Key.c_iteno == null ? string.Empty : g.Key.c_iteno.Trim()),
    //                    c_add1 = (g.Key.c_type == null ? string.Empty : g.Key.c_type.Trim()),
    //                    n_gqty = g.Sum(y => (y.n_gqty.HasValue ? y.n_gqty.Value : 0)),
    //                    n_bqty = g.Sum(y => (y.n_bqty.HasValue ? y.n_bqty.Value : 0))
    //                  }).ToList();
    //    #region Populate
    //    lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
    //    {
    //      ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
    //      {
    //        return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
    //      });

    //      if (ts == null)
    //      {
    //        if (cttrx.c_add1.Equals("01", StringComparison.OrdinalIgnoreCase))
    //        {
    //          lstTempStock.Add(new CLASS_TEMP_STOCK()
    //          {
    //            c_gdg = gudang,
    //            c_iteno = cttrx.c_iteno,
    //            n_grnbeli = cttrx.n_gqty,
    //            n_brnbeli = cttrx.n_bqty,
    //            n_grnbonus = 0,
    //            n_brnbonus = 0,
    //          });
    //        }
    //        else if (cttrx.c_add1.Equals("02", StringComparison.OrdinalIgnoreCase))
    //        {
    //          lstTempStock.Add(new CLASS_TEMP_STOCK()
    //          {
    //            c_gdg = gudang,
    //            c_iteno = cttrx.c_iteno,
    //            n_grnbeli = 0,
    //            n_brnbeli = 0,
    //            n_grnbonus = cttrx.n_gqty,
    //            n_brnbonus = cttrx.n_bqty,
    //          });
    //        }
    //      }
    //      else
    //      {
    //        if (cttrx.c_add1.Equals("01", StringComparison.OrdinalIgnoreCase))
    //        {
    //          ts.n_grnbeli = cttrx.n_gqty;
    //          ts.n_brnbeli = cttrx.n_bqty;
    //        }
    //        else if (cttrx.c_add1.Equals("02", StringComparison.OrdinalIgnoreCase))
    //        {
    //          ts.n_grnbonus = cttrx.n_gqty;
    //          ts.n_brnbonus = cttrx.n_bqty;
    //        }
    //      }
    //    });
    //    #endregion
    //    lstTempTrx.Clear();
    //    lstTemp.Clear();

    //    #endregion

    //    #region RN RS, Claim, Repack

    //    lstTemp.Add("02");
    //    lstTemp.Add("03");
    //    lstTemp.Add("04");
    //    lstTempTrx = (from q in db.LG_RNHs
    //                  join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
    //                  where (q.c_gdg == gudang) && lstTemp.Contains(q.c_type) //(q.c_type == "02")
    //                    && ((q.d_rndate >= date1) && (q.d_rndate < date2))
    //                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //                    && qryItemsAktif.Contains(q1.c_iteno)
    //                  group q1 by new { q.c_type, q1.c_iteno } into g
    //                  select new CLASS_TEMP_TRANSKSI()
    //                  {
    //                    c_gdg = gudang,
    //                    c_add1 = (g.Key.c_type == null ? string.Empty : g.Key.c_type.Trim()),
    //                    c_iteno = (g.Key.c_iteno == null ? string.Empty : g.Key.c_iteno.Trim()),
    //                    n_gqty = g.Sum(y => (y.n_gqty.HasValue ? y.n_gqty.Value : 0)),
    //                    n_bqty = g.Sum(y => (y.n_bqty.HasValue ? y.n_bqty.Value : 0))
    //                  }).ToList();
    //    #region Populate
    //    lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
    //    {
    //      ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
    //      {
    //        return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
    //      });

    //      if (ts == null)
    //      {
    //        if (cttrx.c_add1.Equals("02", StringComparison.OrdinalIgnoreCase))
    //        {
    //          lstTempStock.Add(new CLASS_TEMP_STOCK()
    //          {
    //            c_gdg = gudang,
    //            c_iteno = cttrx.c_iteno,
    //            n_grnrs = cttrx.n_gqty,
    //            n_brnrs = cttrx.n_bqty,
    //          });
    //        }
    //        else if (cttrx.c_add1.Equals("03", StringComparison.OrdinalIgnoreCase))
    //        {
    //          lstTempStock.Add(new CLASS_TEMP_STOCK()
    //          {
    //            c_gdg = gudang,
    //            c_iteno = cttrx.c_iteno,
    //            n_grnclaim = cttrx.n_gqty,
    //            n_brnclaim = cttrx.n_bqty,
    //          });
    //        }
    //        else if (cttrx.c_add1.Equals("04", StringComparison.OrdinalIgnoreCase))
    //        {
    //          lstTempStock.Add(new CLASS_TEMP_STOCK()
    //          {
    //            c_gdg = gudang,
    //            c_iteno = cttrx.c_iteno,
    //            n_grnrepack = cttrx.n_gqty,
    //            n_brnrepack = cttrx.n_bqty,
    //          });
    //        }
    //      }
    //      else
    //      {
    //        if (cttrx.c_add1.Equals("02", StringComparison.OrdinalIgnoreCase))
    //        {
    //          ts.n_grnrs = cttrx.n_gqty;
    //          ts.n_brnrs = cttrx.n_bqty;
    //        }
    //        else if (cttrx.c_add1.Equals("03", StringComparison.OrdinalIgnoreCase))
    //        {
    //          ts.n_grnclaim = cttrx.n_gqty;
    //          ts.n_brnclaim = cttrx.n_bqty;
    //        }
    //        else if (cttrx.c_add1.Equals("04", StringComparison.OrdinalIgnoreCase))
    //        {
    //          ts.n_grnrepack = cttrx.n_gqty;
    //          ts.n_brnrepack = cttrx.n_bqty;
    //        }
    //      }
    //    });
    //    #endregion
    //    lstTempTrx.Clear();
    //    lstTemp.Clear();

    //    #endregion

    //    #region RN Gudang

    //    lstTempTrx = (from q in db.LG_SJHs
    //                  join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
    //                  where (q.c_gdg2 == gudang)
    //                    && ((q.d_update >= date1) && (q.d_update < date2))
    //                    && ((q.l_status.HasValue ? q.l_status.Value : false) == true)
    //                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //                    && qryItemsAktif.Contains(q1.c_iteno)
    //                  group q1 by new { q.c_gdg2, q1.c_iteno } into g
    //                  select new CLASS_TEMP_TRANSKSI()
    //                  {
    //                    c_gdg = (g.Key.c_gdg2.HasValue ? g.Key.c_gdg2.Value : char.MinValue),
    //                    c_iteno = (g.Key.c_iteno == null ? string.Empty : g.Key.c_iteno.Trim()),
    //                    n_gqty = g.Sum(y => (y.n_gqty.HasValue ? y.n_gqty.Value : 0)),
    //                    n_bqty = g.Sum(y => (y.n_bqty.HasValue ? y.n_bqty.Value : 0))
    //                  }).ToList();
    //    #region Populate
    //    lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
    //    {
    //      ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
    //      {
    //        return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
    //      });

    //      if (ts == null)
    //      {
    //        lstTempStock.Add(new CLASS_TEMP_STOCK()
    //        {
    //          c_gdg = gudang,
    //          c_iteno = cttrx.c_iteno,
    //          n_grngdg = cttrx.n_gqty,
    //          n_brngdg = cttrx.n_bqty,
    //        });
    //      }
    //      else
    //      {
    //        ts.n_grngdg = cttrx.n_gqty;
    //        ts.n_brngdg = cttrx.n_bqty;
    //      }
    //    });
    //    #endregion
    //    lstTempTrx.Clear();

    //    #endregion

    //    #region RC

    //    lstTempTrx = (from q in db.LG_RCHes
    //                  join q1 in db.LG_RCD1s on new { q.c_gdg, q.c_rcno } equals new { q1.c_gdg, q1.c_rcno }
    //                  where (q.c_gdg == gudang)
    //                    && ((q.d_rcdate >= date1) && (q.d_rcdate < date2))
    //                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //                    && qryItemsAktif.Contains(q1.c_iteno)
    //                  group q1 by new { q1.c_iteno } into g
    //                  select new CLASS_TEMP_TRANSKSI()
    //                  {
    //                    c_gdg = gudang,
    //                    c_iteno = (g.Key.c_iteno == null ? string.Empty : g.Key.c_iteno.Trim()),
    //                    n_gqty = g.Sum(t => (t.c_type == "01" ? (t.n_qty.HasValue ? t.n_qty.Value : 0) : 0)),
    //                    n_bqty = g.Sum(t => (t.c_type != "01" ? (t.n_qty.HasValue ? t.n_qty.Value : 0) : 0)),
    //                  }).ToList();
    //    #region Populate
    //    lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
    //    {
    //      ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
    //      {
    //        return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
    //      });

    //      if (ts == null)
    //      {
    //        lstTempStock.Add(new CLASS_TEMP_STOCK()
    //        {
    //          c_gdg = gudang,
    //          c_iteno = cttrx.c_iteno,
    //          n_grc = cttrx.n_gqty,
    //          n_brc = cttrx.n_bqty,
    //        });
    //      }
    //      else
    //      {
    //        ts.n_grc = cttrx.n_gqty;
    //        ts.n_brc = cttrx.n_bqty;
    //      }
    //    });
    //    #endregion
    //    lstTempTrx.Clear();

    //    #endregion

    //    #region Combo

    //    lstTempTrx = (from q in db.LG_ComboHs
    //                  where (q.c_gdg == gudang) && (q.c_type == "01")
    //                    && ((q.l_confirm.HasValue ? q.l_confirm.Value : false) == true)
    //                    && ((q.d_combodate >= date1) && (q.d_combodate < date2))
    //                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //                    && qryItemsAktif.Contains(q.c_iteno)
    //                  group q by new { q.c_iteno } into g
    //                  select new CLASS_TEMP_TRANSKSI()
    //                  {
    //                    c_gdg = gudang,
    //                    c_iteno = (g.Key.c_iteno == null ? string.Empty : g.Key.c_iteno.Trim()),
    //                    n_gqty = g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
    //                    //n_bqty = g.Sum(t => (t.n_bqty.HasValue ? t.n_bqty.Value : 0)),
    //                    n_bqty = 0,
    //                  }).ToList();
    //    #region Populate
    //    lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
    //    {
    //      ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
    //      {
    //        return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
    //      });

    //      if (ts == null)
    //      {
    //        lstTempStock.Add(new CLASS_TEMP_STOCK()
    //        {
    //          c_gdg = gudang,
    //          c_iteno = cttrx.c_iteno,
    //          n_combo = cttrx.n_gqty,
    //        });
    //      }
    //      else
    //      {
    //        ts.n_combo = cttrx.n_gqty;
    //      }
    //    });
    //    #endregion
    //    lstTempTrx.Clear();

    //    #endregion

    //    #region RS Beli, Repack

    //    lstTemp.Add("01");
    //    lstTemp.Add("02");
    //    lstTempTrx = (from q in db.LG_RSHes
    //                  join q1 in db.LG_RSD1s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
    //                  where (q.c_gdg == gudang) && lstTemp.Contains(q.c_type)
    //                    && ((q.d_rsdate >= date1) && (q.d_rsdate < date2))
    //                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //                    && qryItemsAktif.Contains(q1.c_iteno)
    //                  group q1 by new { q.c_type, q1.c_iteno } into g
    //                  select new CLASS_TEMP_TRANSKSI()
    //                  {
    //                    c_gdg = gudang,
    //                    c_add1 = (g.Key.c_type == null ? string.Empty : g.Key.c_type.Trim()),
    //                    c_iteno = (g.Key.c_iteno == null ? string.Empty : g.Key.c_iteno.Trim()),
    //                    n_gqty = g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
    //                    n_bqty = g.Sum(t => (t.n_bqty.HasValue ? t.n_bqty.Value : 0)),
    //                  }).ToList();
    //    #region Populate
    //    lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
    //    {
    //      ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
    //      {
    //        return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
    //      });

    //      if (ts == null)
    //      {
    //        if (cttrx.c_add1.Equals("01", StringComparison.OrdinalIgnoreCase))
    //        {
    //          lstTempStock.Add(new CLASS_TEMP_STOCK()
    //          {
    //            c_gdg = gudang,
    //            c_iteno = cttrx.c_iteno,
    //            n_grsbeli = cttrx.n_gqty,
    //            n_brsbeli = cttrx.n_bqty,
    //          });
    //        }
    //        else if (cttrx.c_add1.Equals("02", StringComparison.OrdinalIgnoreCase))
    //        {
    //          lstTempStock.Add(new CLASS_TEMP_STOCK()
    //          {
    //            c_gdg = gudang,
    //            c_iteno = cttrx.c_iteno,
    //            n_grsrepack = cttrx.n_gqty,
    //            n_brsrepack = cttrx.n_bqty,
    //          });
    //        }
    //      }
    //      else
    //      {
    //        if (cttrx.c_add1.Equals("01", StringComparison.OrdinalIgnoreCase))
    //        {
    //          ts.n_grsbeli = cttrx.n_gqty;
    //          ts.n_brsbeli = cttrx.n_bqty;
    //        }
    //        else if (cttrx.c_add1.Equals("02", StringComparison.OrdinalIgnoreCase))
    //        {
    //          ts.n_grsrepack = cttrx.n_gqty;
    //          ts.n_brsrepack = cttrx.n_bqty;
    //        }
    //      }
    //    });
    //    #endregion
    //    lstTempTrx.Clear();
    //    lstTemp.Clear();

    //    #endregion

    //    #region SJ Std, Claim

    //    lstTempTrx = (from q in db.LG_SJHs
    //                  join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
    //                  where (q.c_gdg == gudang)
    //                    && ((q.d_sjdate >= date1) && (q.d_sjdate < date2))
    //                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //                    && qryItemsAktif.Contains(q1.c_iteno)
    //                  group q1 by new { q.c_type, q1.c_iteno } into g
    //                  select new CLASS_TEMP_TRANSKSI()
    //                  {
    //                    c_gdg = gudang,
    //                    c_add1 = (g.Key.c_type == null ? string.Empty : g.Key.c_type.Trim()),
    //                    c_iteno = (g.Key.c_iteno == null ? string.Empty : g.Key.c_iteno.Trim()),
    //                    n_gqty = g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
    //                    n_bqty = g.Sum(t => (t.n_bqty.HasValue ? t.n_bqty.Value : 0)),
    //                  }).ToList();
    //    #region Populate
    //    lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
    //    {
    //      ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
    //      {
    //        return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
    //      });

    //      if (ts == null)
    //      {
    //        if (cttrx.c_add1.Equals("03", StringComparison.OrdinalIgnoreCase))
    //        {
    //          lstTempStock.Add(new CLASS_TEMP_STOCK()
    //          {
    //            c_gdg = gudang,
    //            c_iteno = cttrx.c_iteno,
    //            n_gsjclaim = cttrx.n_gqty,
    //            n_bsjclaim = cttrx.n_bqty,
    //          });
    //        }
    //        else
    //        {
    //          lstTempStock.Add(new CLASS_TEMP_STOCK()
    //          {
    //            c_gdg = gudang,
    //            c_iteno = cttrx.c_iteno,
    //            n_gsj = cttrx.n_gqty,
    //            n_bsj = cttrx.n_bqty,
    //          });
    //        }
    //      }
    //      else
    //      {
    //        if (cttrx.c_add1.Equals("03", StringComparison.OrdinalIgnoreCase))
    //        {
    //          ts.n_gsjclaim += cttrx.n_gqty;
    //          ts.n_bsjclaim += cttrx.n_bqty;
    //        }
    //        else
    //        {
    //          ts.n_gsj += cttrx.n_gqty;
    //          ts.n_bsj += cttrx.n_bqty;
    //        }
    //      }
    //    });
    //    #endregion
    //    lstTempTrx.Clear();
    //    lstTemp.Clear();

    //    #endregion

    //    #region PL

    //    lstTempTrx = (from q in db.LG_PLHs
    //                  join q1 in db.LG_PLD1s on q.c_plno equals q1.c_plno
    //                  where (q.c_gdg == gudang) && (q.l_confirm == true)
    //                    && ((q.d_pldate >= date1) && (q.d_pldate < date2))
    //                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //                    && qryItemsAktif.Contains(q1.c_iteno)
    //                  group q1 by new { q1.c_iteno } into g
    //                  select new CLASS_TEMP_TRANSKSI()
    //                  {
    //                    c_gdg = gudang,
    //                    c_iteno = (g.Key.c_iteno == null ? string.Empty : g.Key.c_iteno.Trim()),
    //                    n_gqty = g.Sum(y => (y.n_qty.HasValue ? y.n_qty.Value : 0)),
    //                    n_bqty = 0
    //                  }).ToList();
    //    #region Populate
    //    lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
    //    {
    //      ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
    //      {
    //        return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
    //      });

    //      if (ts == null)
    //      {
    //        lstTempStock.Add(new CLASS_TEMP_STOCK()
    //        {
    //          c_gdg = gudang,
    //          c_iteno = cttrx.c_iteno,
    //          n_pl = cttrx.n_gqty,
    //        });
    //      }
    //      else
    //      {
    //        ts.n_pl = cttrx.n_gqty;
    //      }
    //    });
    //    #endregion
    //    lstTempTrx.Clear();

    //    #endregion

    //    #region STT

    //    lstTempTrx = (from q in db.LG_STHs
    //                  join q1 in db.LG_STD1s on new { q.c_gdg, q.c_stno } equals new { q1.c_gdg, q1.c_stno }
    //                  where (q.c_gdg == gudang)
    //                    && ((q.d_stdate >= date1) && (q.d_stdate < date2))
    //                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //                    && qryItemsAktif.Contains(q1.c_iteno)
    //                  group q1 by new { q1.c_iteno } into g
    //                  select new CLASS_TEMP_TRANSKSI()
    //                  {
    //                    c_gdg = gudang,
    //                    c_iteno = (g.Key.c_iteno == null ? string.Empty : g.Key.c_iteno.Trim()),
    //                    n_gqty = g.Sum(y => (y.n_qty.HasValue ? y.n_qty.Value : 0)),
    //                    n_bqty = 0
    //                  }).ToList();
    //    #region Populate
    //    lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
    //    {
    //      ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
    //      {
    //        return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
    //      });

    //      if (ts == null)
    //      {
    //        lstTempStock.Add(new CLASS_TEMP_STOCK()
    //        {
    //          c_gdg = gudang,
    //          c_iteno = cttrx.c_iteno,
    //          n_stt = cttrx.n_gqty,
    //        });
    //      }
    //      else
    //      {
    //        ts.n_stt = cttrx.n_gqty;
    //      }
    //    });
    //    #endregion
    //    lstTempTrx.Clear();

    //    #endregion

    //    #region Combo Detail

    //    lstTempTrx = (from q in db.LG_ComboHs
    //                  join q1 in db.LG_ComboD1s on new { q.c_gdg, q.c_combono } equals new { q1.c_gdg, q1.c_combono }
    //                  where (q.c_gdg == gudang) && (q.l_confirm == true)
    //                    && ((q.d_combodate >= date1) && (q.d_combodate < date2))
    //                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //                    && qryItemsAktif.Contains(q1.c_iteno)
    //                  group q1 by new { q.c_gdg, q1.c_iteno } into g
    //                  select new CLASS_TEMP_TRANSKSI()
    //                  {
    //                    c_gdg = g.Key.c_gdg,
    //                    c_iteno = g.Key.c_iteno,
    //                    n_gqty = g.Sum(t => (t.n_qty.HasValue ? t.n_qty.Value : 0)),
    //                    n_bqty = 0,
    //                  }).Distinct().ToList();
    //    #region Populate
    //    lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
    //    {
    //      ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
    //      {
    //        return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
    //      });

    //      if (ts == null)
    //      {
    //        lstTempStock.Add(new CLASS_TEMP_STOCK()
    //        {
    //          c_gdg = gudang,
    //          c_iteno = cttrx.c_iteno,
    //          n_combod = cttrx.n_gqty,
    //        });
    //      }
    //      else
    //      {
    //        ts.n_combod = cttrx.n_gqty;
    //      }
    //    });
    //    #endregion
    //    lstTempTrx.Clear();

    //    #endregion

    //    #region Adjustment

    //    lstTempTrx = (from q in db.LG_AdjustHs
    //                  join q1 in db.LG_AdjustD1s on new { q.c_gdg, q.c_adjno } equals new { q1.c_gdg, q1.c_adjno }
    //                  where (q.c_gdg == gudang)
    //                    && ((q.d_adjdate >= date1) && (q.d_adjdate < date2))
    //                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
    //                    && qryItemsAktif.Contains(q1.c_iteno)
    //                  group q1 by new { q.c_gdg, q1.c_iteno } into g
    //                  select new CLASS_TEMP_TRANSKSI()
    //                  {
    //                    c_gdg = g.Key.c_gdg,
    //                    c_iteno = g.Key.c_iteno,
    //                    n_gawal = g.Sum(t => ((t.n_gqty.HasValue ? t.n_gqty.Value : 0) > 0 ? (t.n_gqty.HasValue ? t.n_gqty.Value : 0) : 0)),
    //                    n_bawal = g.Sum(t => ((t.n_bqty.HasValue ? t.n_bqty.Value : 0) > 0 ? (t.n_bqty.HasValue ? t.n_bqty.Value : 0) : 0)),
    //                    n_gqty = g.Sum(t => ((t.n_gqty.HasValue ? t.n_gqty.Value : 0) < 0 ? -(t.n_gqty.HasValue ? t.n_gqty.Value : 0) : 0)),
    //                    n_bqty = g.Sum(t => ((t.n_bqty.HasValue ? t.n_bqty.Value : 0) < 0 ? -(t.n_bqty.HasValue ? t.n_bqty.Value : 0) : 0)),
    //                  }).Distinct().ToList();
    //    #region Populate
    //    lstTempTrx.ForEach(delegate(CLASS_TEMP_TRANSKSI cttrx)
    //    {
    //      ts = lstTempStock.Find(delegate(CLASS_TEMP_STOCK cts)
    //      {
    //        return cts.c_iteno.Equals(cttrx.c_iteno, StringComparison.OrdinalIgnoreCase);
    //      });

    //      if (ts == null)
    //      {
    //        lstTempStock.Add(new CLASS_TEMP_STOCK()
    //        {
    //          c_gdg = gudang,
    //          c_iteno = cttrx.c_iteno,
    //          n_gadj = cttrx.n_gqty,
    //          n_badj = cttrx.n_bqty,
    //          n_grngdg = cttrx.n_gawal,
    //          n_brngdg = cttrx.n_bawal,
    //        });
    //      }
    //      else
    //      {
    //        ts.n_gadj = cttrx.n_gqty;
    //        ts.n_badj = cttrx.n_bqty;

    //        ts.n_grngdg += cttrx.n_gawal;
    //        ts.n_brngdg += cttrx.n_bawal;
    //      }
    //    });
    //    #endregion
    //    lstTempTrx.Clear();

    //    #endregion

    //    #endregion

    //    var qryInnerItemHpp = (from sq in db.FA_MasItms
    //                           //join sq1 in db.LG_HPPs on new { sq.c_iteno, s_tahun = (short)date2.Year, t_bulan = (byte)date2.Month, c_type = "01" } equals new { sq1.c_iteno, sq1.s_tahun, sq1.t_bulan, sq1.c_type } into sq_1
    //                           //from qHPP in sq_1.DefaultIfEmpty()
    //                           where ((sq.l_aktif.HasValue ? sq.l_aktif.Value : false) == true)
    //                             //&& ((sq.l_hide.HasValue ? sq.l_hide.Value : false) == false)
    //                             && ((sq.l_delete.HasValue ? sq.l_delete.Value : false) == false)
    //                           select new DSNG_SCMS_LGPBF()
    //                           {
    //                             c_iteno = sq.c_iteno,
    //                             v_itnam = sq.v_itnam,
    //                             c_alkes = sq.c_alkes,
    //                             n_pbf = ((sq.n_PBF.HasValue ? sq.n_PBF.Value : 0) == 0 ? 1 : (sq.n_PBF.HasValue ? sq.n_PBF.Value : 0)),
    //                             //n_hna = ((qHPP != null) && qHPP.n_akhirhpp.HasValue ? qHPP.n_akhirhpp.Value : (sq.n_salpri.HasValue ? sq.n_salpri.Value : 0)) / ((sq.n_PBF.HasValue ? sq.n_PBF.Value : 0) == 0 ? 1 : (sq.n_PBF.HasValue ? sq.n_PBF.Value : 0)) * 1.1m,
    //                             n_hna = (sq.n_salpri.HasValue ? sq.n_salpri.Value : 0),
    //                             n_het = (sq.n_het.HasValue ? sq.n_het.Value : 0),
    //                           }).AsQueryable();


    //    var qryInnerDivPri = (from sq in db.FA_MsDivPris
    //                          join sq1 in db.FA_Divpris on sq.c_kddivpri equals sq1.c_kddivpri
    //                          where ((sq.l_delete.HasValue ? sq.l_delete.Value : false) == false)
    //                          select new DSNG_SCMS_LGPBF()
    //                          {
    //                            n_hna = ((sq.n_het.HasValue ? sq.n_het.Value : 0) == 0 ? 1.25m : (sq.n_het.HasValue ? sq.n_het.Value : 0)),
    //                            c_iteno = sq1.c_iteno,
    //                          }).AsQueryable();

    //    listTempPBF = (from q in qryInnerItemHpp
    //                   join q1 in qryInnerDivPri on q.c_iteno equals q1.c_iteno into q_1
    //                   from qIDP in q_1.DefaultIfEmpty()
    //                   select new DSNG_SCMS_LGPBF()
    //                   {
    //                     c_iteno = q.c_iteno,
    //                     v_itnam = q.v_itnam,
    //                     c_alkes = q.c_alkes,
    //                     //n_het = (q.n_het * q1.n_het)
    //                     //n_hna = ((qIDP != null) && (qIDP.n_hna != 0) ? qIDP.n_hna : 1) * q.n_hna,
    //                     n_hna = ((q.n_hna == 0) ? (((qIDP != null) && (qIDP.n_hna != 0) ? qIDP.n_hna : 1) * q.n_hna): q.n_hna),
    //                     n_het = q.n_het,
    //                     n_pbf = q.n_pbf
    //                   }).ToList();

    //    dateX = date2.AddDays(-1);

    //    tmp1 = date1.ToString("dd-MM-yyyy");
    //    tmp2 = dateX.ToString("dd-MM-yyyy");

    //    ListQuery = (from q in lstTempStock
    //                 join q1 in listTempPBF on q.c_iteno equals q1.c_iteno
    //                 orderby q1.v_itnam
    //                 select new DSNG_SCMS_LGPBF()
    //                 {
    //                   c_gdg = q.c_gdg,
    //                   c_iteno = q.c_iteno,
    //                   n_awal = q.n_gawal * q1.n_pbf,
    //                   n_inPabrik = (q.n_grnbeli + q.n_grnbonus + q.n_grnclaim + q.n_grnrs + q.n_grnrepack) * q1.n_pbf,
    //                   n_inpbf = 0,
    //                   n_inLain = (q.n_grc + q.n_grngdg + q.n_combo) * q1.n_pbf,
    //                   n_OutRS = (q.n_grsbeli + q.n_grsrepack) * q1.n_pbf,
    //                   n_outApotik = 0,
    //                   n_outPBF = (q.n_gsj + q.n_gsjclaim + q.n_pl + q.n_stt) * q1.n_pbf,
    //                   n_outLain = (q.n_combod + q.n_gadj) * q1.n_pbf,
    //                   n_outPemerintah = 0,
    //                   n_outSwasta = 0,
    //                   n_hna = q1.n_hna,
    //                   n_het = q1.n_het,
    //                   d_awal = tmp1,
    //                   d_akhir = tmp2,
    //                   n_pbf = q1.n_pbf,
    //                   Periode = tipePeriode,
    //                   c_alkes = q1.c_alkes,
    //                   v_itnam = q1.v_itnam
    //                 }).ToList();
    //  }
    //  catch (Exception ex)
    //  {
    //    this.IsError = true;
    //    this.ErrorMessage = ex.Message;

    //    Logger.WriteLine(
    //      "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportPBF PopulateDataset - {0}", ex.StackTrace);
    //  }

    //  #endregion

    //  #region Populate

    //  if (ListQuery != null)
    //  {
    //    try
    //    {
    //      table = ListQuery.CopyToDataTableObject();

    //      if (table != null)
    //      {
    //        dataSet = new System.Data.DataSet();

    //        dataSet.Tables.Add(table);
    //      }
    //    }
    //    catch (Exception ex)
    //    {
    //      this.IsError = true;
    //      this.ErrorMessage = ex.Message;

    //      Logger.WriteLine(
    //        "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportPBF PopulateDataset - {0}", ex.StackTrace);
    //    }
    //  }

    //  #endregion

    //  if(listStokAwal != null)
    //  {
    //    listStokAwal.Clear();
    //  }
    //  if(listSATemp != null)
    //  {
    //    listSATemp.Clear();
    //  }
    //  if (listTempPBF != null)
    //  {
    //    listTempPBF.Clear();
    //  }
    //  if (lstTempStock != null)
    //  {
    //    lstTempStock.Clear();
    //  }
    //  if (lstTempTrx != null)
    //  {
    //    lstTempTrx.Clear();
    //  }
    //  if (ListQuery != null)
    //  {
    //    ListQuery.Clear();
    //  }

    //  return dataSet;
    //}

    public System.Data.DataSet ReportHistorySP(ORMDataContext db, Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> dic)
    {
      ScmsSoaLibrary.Commons.Functionals.ParameterParser pp = default(ScmsSoaLibrary.Commons.Functionals.ParameterParser);

      System.Data.DataSet dataSet = null;
      System.Data.DataTable table = null;

      List<Temp_LGHistorySP_Test> ListQuery = null;

      List<string> lstTemp = new List<string>();

      string item = null,
        sp1 = null, sp2 = null,
        cust = null,
        supl = null,
        divSupl = null,
        userData = null;

      StringBuilder sbQuery = new StringBuilder();

      DateTime date1 = DateTime.MinValue,
        date2 = DateTime.MinValue;

      #region Parameter

      try
      {
        if (dic.ContainsKey("date1"))
        {
          pp = dic["date1"];
          if (pp.IsSet)
          {
            date1 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("date2"))
        {
          pp = dic["date2"];
          if (pp.IsSet)
          {
            date2 = (DateTime)pp.Value;
          }
        }
        if (dic.ContainsKey("sp1"))
        {
          pp = dic["sp1"];
          if (pp.IsSet)
          {
            sp1 = (string)pp.Value;
          }
        }
        if (dic.ContainsKey("sp2"))
        {
          pp = dic["sp2"];
          if (pp.IsSet)
          {
            sp2 = (string)pp.Value;
          }
        }
        if (dic.ContainsKey("customer"))
        {
          pp = dic["customer"];
          if (pp.IsSet)
          {
            cust = (string)pp.Value;
          }
        }
        if (dic.ContainsKey("supplier"))
        {
          pp = dic["supplier"];
          if (pp.IsSet)
          {
            supl = (string)pp.Value;
          }
        }
        if (dic.ContainsKey("divprin"))
        {
          pp = dic["divprin"];
          if (pp.IsSet)
          {
            divSupl = (string)pp.Value;
          }
        }
        if (dic.ContainsKey("item"))
        {
          pp = dic["item"];
          if (pp.IsSet)
          {
            item = (string)pp.Value;
          }
        }
        if(dic.ContainsKey("user"))
        {
          pp = dic["user"];
          if(pp.IsSet){
              userData = (string)pp.Value;
          }
        }
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportHistorySP Paramter - {0}", ex.StackTrace);
      }
      
      //if (date1.Equals(DateTime.MinValue) || date2.Equals(DateTime.MinValue))
      if (date1.Equals(DateTime.MinValue))
      {
        return null;
      }
      else if (date2.Equals(DateTime.MinValue))
      {
        date2 = date1;
      }

      #endregion

      #region Query

      #region Parameter

      try
      {
        sbQuery.AppendLine(@"DECLARE
          @SPNO1 VARCHAR(20),
          @SPNO2 VARCHAR(20),
          @DATE1 DATE,
          @DATE2 DATE,
          @CUSNO CHAR(4),
          @ITENO CHAR(4),
          @SA CHAR(7),
          @NOSUP CHAR(5),
          @DIVPRI CHAR(3)

          set @SA = {0}

          set @SPNO1 = {1}
          set @SPNO2 = {2}
          set @DATE1 = {3}
          set @DATE2 = {4}
          set @CUSNO = {5}
          set @ITENO = {6}
          set @NOSUP = {7}
          set @DIVPRI = {8}

          set @SPNO1 = ISNULL(@SPNO1, '')
          set @SPNO2 = ISNULL(@SPNO2, '')
          set @CUSNO = ISNULL(@CUSNO, '')
          set @CUSNO = Case @CUSNO When '0000' Then '' Else @CUSNO End
          set @ITENO = ISNULL(@ITENO, '')
          set @ITENO = Case @ITENO When '0000' Then '' Else @ITENO End
          set @NOSUP = ISNULL(@NOSUP, '')
          set @NOSUP = Case @NOSUP When '00000' Then '' Else @NOSUP End
          set @DIVPRI = ISNULL(@DIVPRI, '')
          set @DIVPRI = Case @DIVPRI When '000' Then '' Else @DIVPRI End");

      #endregion

        if (string.IsNullOrEmpty(sp1) || string.IsNullOrEmpty(sp2))
        {
          #region Query
            sbQuery.AppendLine(@"--Insert Temp_LGHistorySP
             select distinct x.*,y.n_waktu_a,z.n_waktu_b from 
            (


            SELECT 
					            A.c_spno, 
					            A.d_spdate, 
					            A.c_sp, 
					            A.c_cusno, 
					            A.c_iteno, 
					            A.n_beliqty, 
					            A.n_bonusqty, 
					            A.n_beliacc, 
					            A.n_bonusacc, 
					            ISNULL(B.c_gdg,'') c_gdg, 
					            ISNULL(B.c_no,'') c_refno, 
					            B.d_date d_refdate, 
					            B.l_confirm l_refconfir, 
					            B.d_update d_refupdate, 
					            ISNULL(B.n_beliqty,0) n_beliqty2, 
					            ISNULL(B.n_bonusqty,0) n_bonusqty2, 
					            @SA c_user, 
					            C.c_dono c_dono, 
					            C.d_dodate d_dodate, 
					            D.c_resi c_resi, 
					            D.d_resi d_resi, 
					            E.v_itnam v_itnam, 
					            F.v_ket v_expdesc, 
					            G.v_cunam v_cunam, 
					            H.v_gdgdesc v_gdgdesc, 
					            E.c_nosup c_nosup, 
					            I.v_nama v_namasup, 
					            J.c_kddivpri c_kddivpri, 
					            J.v_nmdivpri v_nmdivpri, 
                                B.c_no, 
					            B.d_date,				             
					            A.d_spinsert, 
					            B.c_via, 
					            K.v_ket, 
					            a.n_sisa n_sisa_spd1, 
					            a.v_ket v_ket_spd1, 
					            B.v_ket_plh
            					
                        FROM 
                        (
					            SELECT 
							            A.c_spno, 
							            A.d_spdate, 
							            A.c_sp, 
							            A.c_cusno, 
							            B.c_iteno, 
							            cast(A.d_entry as DATE) as d_spinsert,
							            sum(case when b.c_type = '01' then b.n_qty else 0 end) as n_beliqty, 
							            sum(case when b.c_type = '02' then b.n_qty else 0 end) as n_bonusqty,
							            sum(case when b.c_type = '01' then b.n_acc else 0 end) as n_beliacc, 
							            sum(case when b.c_type = '02' then b.n_acc else 0 end) as n_bonusacc,
							            sum(b.n_sisa) n_sisa, 
							            b.v_ket
					            FROM LG_SPH A
					            INNER JOIN LG_SPD1 B ON A.c_spno = B.c_spno 
					            --WHERE A.c_spno BETWEEN @SPNO1 AND @SPNO2 OR A.c_sp BETWEEN @SPNO1 AND @SPNO2 And A.d_spdate BETWEEN @DATE1 AND @DATE2
					            WHERE (A.d_spdate BETWEEN @DATE1 AND @DATE2)
					            And (A.c_cusno = Case @CUSNO When '' Then A.c_cusno Else @CUSNO End)
					            And (B.c_iteno = Case @ITENO When '' Then B.c_iteno Else @ITENO End)
					            and a.c_type != '06'
					            GROUP BY A.c_spno, A.d_spdate, A.c_sp, A.c_cusno, B.c_iteno, cast(A.d_entry as DATE), b.v_ket
                        ) A LEFT OUTER JOIN
                        (
					            SELECT 
							            A.c_gdg, 
							            A.c_plno as c_no, 
							            A.d_pldate as d_date, 
							            A.l_confirm, 
							            A.d_update, 
							            B.c_spno, 
							            B.c_iteno, 
							            sum(case when b.c_type = '01' then b.n_qty else 0 end) as n_beliqty, 
							            sum(case when b.c_type = '02' then b.n_qty else 0 end) as n_bonusqty, 
							            a.c_via, 
							            A.v_ket v_ket_plh
					            FROM LG_PLH A
					            INNER JOIN LG_PLD1 B ON A.c_plno = B.c_plno 
					            INNER JOIN LG_SPH C ON B.c_spno = C.c_spno
					            --WHERE B.c_spno BETWEEN @SPNO1 AND @SPNO2 OR C.c_sp BETWEEN @SPNO1 AND @SPNO2 or C.d_spdate BETWEEN @DATE1 AND @DATE2
					            WHERE (C.d_spdate BETWEEN @DATE1 AND @DATE2)
					            And (C.c_cusno = Case @CUSNO When '' Then C.c_cusno Else @CUSNO End)
					            And (B.c_iteno = Case @ITENO When '' Then B.c_iteno Else @ITENO End)
					            and c.c_type != '06'
					            GROUP BY A.c_gdg, A.c_plno, A.d_pldate, A.l_confirm, A.d_update, B.c_spno, B.c_iteno, a.c_via, A.v_ket
            					
					            UNION ALL
            					
					            SELECT 
							            '1' as c_gdg, 
							             A.c_adjno as c_no, 
							             A.d_adjdate as d_date, 
							             1 as l_confirm, 
							             A.d_adjdate, 
							             B.c_spno, 
							             B.c_iteno, 
							            sum(case when b.c_type = '01' then b.n_qty else 0 end) as n_beliqty, 
							            sum(case when b.c_type = '02' then b.n_qty else 0 end) as n_bonusqty, '', ''
					            FROM LG_AdjSPH A
					            INNER JOIN LG_AdjSPD B ON A.c_adjno = B.c_adjno 
					            INNER JOIN LG_SPH C ON B.c_spno = C.c_spno 
					            --WHERE B.c_spno BETWEEN @SPNO1 AND @SPNO2 OR C.c_sp BETWEEN @SPNO1 AND @SPNO2 or C.d_spdate BETWEEN @DATE1 AND @DATE2
					            WHERE (C.d_spdate BETWEEN @DATE1 AND @DATE2)
					            And (C.c_cusno = Case @CUSNO When '' Then C.c_cusno Else @CUSNO End)
					            And (B.c_iteno = Case @ITENO When '' Then B.c_iteno Else @ITENO End)
					            and c.c_type != '06'
					            GROUP BY A.c_adjno, A.d_adjdate, A.d_adjdate, B.c_spno, B.c_iteno
                        ) B ON A.c_spno = B.c_spno AND A.c_iteno = B.c_iteno LEFT JOIN
                        (
					            SELECT 
							            A.c_dono, 
							            A.d_dodate, 
							            A.c_plno, 
							            B.c_iteno 
					            FROM LG_DOH A 
					            JOIN LG_DOD1 B ON A.c_dono = B.c_dono
					            GROUP BY A.c_dono, A.d_dodate, A.c_plno, B.c_iteno
                        ) C ON B.c_no = C.c_plno And B.c_iteno = C.c_iteno LEFT JOIN
                        (
					            SELECT 
							            A.c_expno, 
							            A.c_exp, 
							            A.c_resi, 
							            A.d_resi, 
							            B.c_dono 
					            FROM LG_ExpH A 
					            Join LG_ExpD B ON A.c_expno = B.c_expno
					            GROUP BY A.c_expno, A.c_exp, A.c_resi, A.d_resi, B.c_dono
            					
                        ) D ON C.c_dono = D.c_dono LEFT JOIN
					            FA_MasItm E ON A.c_iteno = E.c_iteno LEFT JOIN
					            LG_MsExp F ON D.c_exp = F.c_exp LEFT JOIN
					            LG_Cusmas G ON A.c_cusno = G.c_cusno LEFT JOIN
					            LG_MsGudang H ON B.c_gdg = H.c_gdg LEFT JOIN
					            LG_DatSup I ON E.c_nosup = I.c_nosup LEFT JOIN
                        (
					            SELECT 
							            A.c_kddivpri, 
							            A.v_nmdivpri, 
							            B.c_iteno 
					            FROM FA_MsDivPri A 
					            JOIN FA_Divpri B ON A.c_kddivpri = B.c_kddivpri
					            GROUP BY A.c_kddivpri, A.v_nmdivpri, B.c_iteno
                        ) J ON E.c_iteno = J.c_iteno LEFT JOIN
                        (
					            SELECT 
							            c_type, 
							            v_ket FROM MsTransD
					            where c_portal = 3 and c_notrans = '02'
                        ) K ON B.c_via = K.c_type
                        
                        WHERE (I.c_nosup = Case @NOSUP When '' Then I.c_nosup Else @NOSUP End)
                        And (J.c_kddivpri = Case @DIVPRI When '' Then J.c_kddivpri Else @DIVPRI End)

            )  x left join 
            (

				            select distinct
						            c_no1,
						            c_no2,
						            c_iteno,
						            n_waktu as n_waktu_a 
				            from LG_WP
				            where c_type = '03' and s_tahun BETWEEN YEAR(@DATE1) AND YEAR(@DATE2)
            				
            ) y on x.c_spno = y.c_no1 and x.c_iteno = y.c_iteno and y.c_no2 = x.c_dono
            left join
            (

				            select distinct
						            c_no1,
						            c_no2,
						            c_iteno,
						            n_waktu as n_waktu_b 
				            from LG_WP
				            where c_type = '10' and s_tahun BETWEEN YEAR(@DATE1) AND YEAR(@DATE2)
            				
            ) z on z.c_iteno = y.c_iteno and z.c_no1 = y.c_no2 and z.c_no2 = x.c_resi  ");

          #endregion
        }
        else
        {
            #region Query

            sbQuery.AppendLine(@"--Insert Temp_LGHistorySP
           select distinct x.*,y.n_waktu_a,z.n_waktu_b from 
                (


                SELECT 
					                A.c_spno, 
					                A.d_spdate, 
					                A.c_sp, 
					                A.c_cusno, 
					                A.c_iteno, 
					                A.n_beliqty, 
					                A.n_bonusqty, 
					                A.n_beliacc, 
					                A.n_bonusacc, 
					                ISNULL(B.c_gdg,'') c_gdg, 
					                ISNULL(B.c_no,'') c_refno, 
					                B.d_date d_refdate, 
					                B.l_confirm l_refconfir, 
					                B.d_update d_refupdate, 
					                ISNULL(B.n_beliqty,0) n_beliqty2, 
					                ISNULL(B.n_bonusqty,0) n_bonusqty2, 
					                @SA c_user, 
					                C.c_dono c_dono, 
					                C.d_dodate d_dodate, 
					                D.c_resi c_resi, 
					                D.d_resi d_resi, 
					                E.v_itnam v_itnam, 
					                F.v_ket v_expdesc, 
					                G.v_cunam v_cunam, 
					                H.v_gdgdesc v_gdgdesc, 
					                E.c_nosup c_nosup, 
					                I.v_nama v_namasup, 
					                J.c_kddivpri c_kddivpri, 
					                J.v_nmdivpri v_nmdivpri, 
    				                B.c_no, 
			                		B.d_date,
					                A.d_spinsert, 
					                B.c_via, 
					                K.v_ket, 
					                a.n_sisa n_sisa_spd1, 
					                a.v_ket v_ket_spd1, 
					                B.v_ket_plh
                					
                            FROM 
                            (
					                SELECT 
							                A.c_spno, 
							                A.d_spdate, 
							                A.c_sp, 
							                A.c_cusno, 
							                B.c_iteno, 
							                cast(A.d_entry as DATE) as d_spinsert, 
							                sum(case when b.c_type = '01' then b.n_qty else 0 end) as n_beliqty, 
							                sum(case when b.c_type = '02' then b.n_qty else 0 end) as n_bonusqty,
							                sum(case when b.c_type = '01' then b.n_acc else 0 end) as n_beliacc, 
							                sum(case when b.c_type = '02' then b.n_acc else 0 end) as n_bonusacc,
							                sum(n_sisa) n_sisa, 
							                b.v_ket
					                FROM LG_SPH A
					                INNER JOIN LG_SPD1 B ON A.c_spno = B.c_spno 
					                WHERE (A.d_spdate BETWEEN @DATE1 AND @DATE2) AND case when LEN(@SPNO1) = 10 then a.c_spno else a.c_sp end between @SPNO1 AND @SPNO2 
					                And (A.c_cusno = Case @CUSNO When '' Then A.c_cusno Else @CUSNO End)
					                And (B.c_iteno = Case @ITENO When '' Then B.c_iteno Else @ITENO End)
					                and a.c_type != '06'
					                GROUP BY A.c_spno, A.d_spdate, A.c_sp, A.c_cusno, B.c_iteno, cast(A.d_entry as DATE), b.v_ket
                					
		                 ) A	LEFT OUTER JOIN
					                (
					                SELECT 
							                A.c_gdg, 
							                A.c_plno as c_no, 
							                A.d_pldate as d_date, 
							                A.l_confirm, 
							                A.d_update, 
							                B.c_spno, 
							                B.c_iteno, 
							                sum(case when b.c_type = '01' then b.n_qty else 0 end) as n_beliqty, 
							                sum(case when b.c_type = '02' then b.n_qty else 0 end) as n_bonusqty, 
							                a.c_via, 
							                A.v_ket v_ket_plh
                							
					                FROM LG_PLH A
					                INNER JOIN LG_PLD1 B ON A.c_plno = B.c_plno 
					                INNER JOIN LG_SPH C ON B.c_spno = C.c_spno
					                WHERE (C.d_spdate BETWEEN @DATE1 AND @DATE2) AND case when LEN(@SPNO1) = 10 then b.c_spno else c.c_sp end between @SPNO1 AND @SPNO2 
					                And (C.c_cusno = Case @CUSNO When '' Then C.c_cusno Else @CUSNO End)
					                And (B.c_iteno = Case @ITENO When '' Then B.c_iteno Else @ITENO End)
					                and c.c_type != '06'
					                GROUP BY A.c_gdg, A.c_plno, A.d_pldate, A.l_confirm, A.d_update, B.c_spno, B.c_iteno, a.c_via, A.v_ket
                					
					                UNION ALL
                					
					                SELECT 
							                '1' as c_gdg, 
							                A.c_adjno as c_no, 
							                A.d_adjdate as d_date, 
							                1 as l_confirm, 
							                A.d_adjdate, 
							                B.c_spno, 
							                B.c_iteno, 
							                sum(case when b.c_type = '01' then b.n_qty else 0 end) as n_beliqty, 
							                sum(case when b.c_type = '02' then b.n_qty else 0 end) as n_bonusqty, 
							                '', ''
					                FROM LG_AdjSPH A
					                INNER JOIN LG_AdjSPD B ON A.c_adjno = B.c_adjno 
					                INNER JOIN LG_SPH C ON B.c_spno = C.c_spno 
					                WHERE (C.d_spdate BETWEEN @DATE1 AND @DATE2) AND  case when LEN(@SPNO1) = 10 then b.c_spno else c.c_sp end between @SPNO1 AND @SPNO2 
					                And (C.c_cusno = Case @CUSNO When '' Then C.c_cusno Else @CUSNO End)
					                And (B.c_iteno = Case @ITENO When '' Then B.c_iteno Else @ITENO End)
					                and c.c_type != '06'
					                GROUP BY A.c_adjno, A.d_adjdate, A.d_adjdate, B.c_spno, B.c_iteno
                            ) B ON A.c_spno = B.c_spno AND A.c_iteno = B.c_iteno LEFT JOIN
                            (
					                SELECT 
							                A.c_dono, 
							                A.d_dodate, 
							                A.c_plno, 
							                B.c_iteno 
					                FROM LG_DOH A 
					                JOIN LG_DOD1 B ON A.c_dono = B.c_dono
					                GROUP BY A.c_dono, A.d_dodate, A.c_plno, B.c_iteno
                					
                            ) C ON B.c_no = C.c_plno And B.c_iteno = C.c_iteno LEFT JOIN
                            (
					                SELECT 
							                A.c_expno, 
							                A.c_exp, 
							                A.c_resi, 
							                A.d_resi, 
							                B.c_dono 
					                FROM LG_ExpH A 
					                Join	LG_ExpD B ON A.c_expno = B.c_expno
					                GROUP BY A.c_expno, A.c_exp, A.c_resi, A.d_resi, B.c_dono
                            ) D ON C.c_dono = D.c_dono LEFT JOIN
					                FA_MasItm E ON A.c_iteno = E.c_iteno LEFT JOIN
					                LG_MsExp F ON D.c_exp = F.c_exp LEFT JOIN
					                LG_Cusmas G ON A.c_cusno = G.c_cusno LEFT JOIN
					                LG_MsGudang H ON B.c_gdg = H.c_gdg LEFT JOIN
					                LG_DatSup I ON E.c_nosup = I.c_nosup LEFT JOIN
                            (
					                SELECT 
							                A.c_kddivpri, 
							                A.v_nmdivpri, 
							                B.c_iteno 
					                FROM FA_MsDivPri A 
					                JOIN FA_Divpri B ON A.c_kddivpri = B.c_kddivpri
					                GROUP BY A.c_kddivpri, A.v_nmdivpri, B.c_iteno
                					
                            ) J ON E.c_iteno = J.c_iteno LEFT JOIN
                            (
					                SELECT 
							                c_type, 
							                v_ket FROM MsTransD
					                where c_portal = 3 and c_notrans = '02'
                            ) K ON B.c_via = K.c_type
                            
				                WHERE (I.c_nosup = Case @NOSUP When '' Then I.c_nosup Else @NOSUP End)
                                And (J.c_kddivpri = Case @DIVPRI When '' Then J.c_kddivpri Else @DIVPRI End)

                )  x left join 
                (

				                select distinct 
						                c_no1,
						                c_no2,
						                c_iteno,
						                n_waktu as n_waktu_a 
				                from LG_WP
				                where c_type = '03'
                				
                ) y on x.c_spno = y.c_no1 and x.c_iteno = y.c_iteno and y.c_no2 = x.c_dono
                left join
                (

				                select distinct
						                c_no1,
						                c_no2,
						                c_iteno,
						                n_waktu as n_waktu_b 
				                from LG_WP
				                where c_type = '10'
                				
                ) z on z.c_iteno = y.c_iteno and z.c_no1 = y.c_no2 and z.c_no2 = x.c_resi  ");

#region old
//            sbQuery.AppendLine(@"--Insert Temp_LGHistorySP
//                    SELECT A.c_spno, A.d_spdate, A.c_sp, A.c_cusno, A.c_iteno, A.n_beliqty, A.n_bonusqty, A.n_beliacc, A.n_bonusacc, ISNULL(B.c_gdg,'') c_gdg, ISNULL(B.c_no,'') c_refno, B.d_date d_refdate, B.l_confirm l_refconfir, B.d_update d_refupdate, ISNULL(B.n_beliqty,0) n_refbeliqty, ISNULL(B.n_bonusqty,0) n_refbonusqty, @SA c_user, C.c_dono c_dono, C.d_dodate d_dodate, D.c_resi c_resi, D.d_resi d_resi, E.v_itnam v_itnam, F.v_ket v_expdesc, G.v_cunam v_cunam, H.v_gdgdesc v_gdgdesc, E.c_nosup c_nosup, I.v_nama v_namasup, J.c_kddivpri c_kddivpri, J.v_nmdivpri v_nmdivpri, B.c_no, B.d_date, A.d_spinsert  FROM 
//	                (
//		                SELECT A.c_spno, A.d_spdate, A.c_sp, A.c_cusno, B.c_iteno, cast(A.d_entry as DATE) as d_spinsert,
//		                sum(case when b.c_type = '01' then b.n_qty else 0 end) as n_beliqty, 
//		                sum(case when b.c_type = '02' then b.n_qty else 0 end) as n_bonusqty,
//		                sum(case when b.c_type = '01' then b.n_acc else 0 end) as n_beliacc, 
//		                sum(case when b.c_type = '02' then b.n_acc else 0 end) as n_bonusacc
//		                FROM LG_SPH A
//                        INNER JOIN
//		                INNER JOIN LG_SPD1 B ON A.c_spno = B.c_spno 
//		                --WHERE A.c_spno BETWEEN @SPNO1 AND @SPNO2 OR A.c_sp BETWEEN @SPNO1 AND @SPNO2 And A.d_spdate BETWEEN @DATE1 AND @DATE2
//		                WHERE (A.d_spdate BETWEEN @DATE1 AND @DATE2)
//			                And (A.c_cusno = Case @CUSNO When '' Then A.c_cusno Else @CUSNO End)
//			                And (B.c_iteno = Case @ITENO When '' Then B.c_iteno Else @ITENO End)
//		                GROUP BY A.c_spno, A.d_spdate, A.c_sp, A.c_cusno, B.c_iteno, cast(A.d_entry as DATE)
//	                ) A LEFT OUTER JOIN
//	                (
//		                SELECT A.c_gdg, A.c_plno as c_no, A.d_pldate as d_date, A.l_confirm, A.d_update, B.c_spno, B.c_iteno, 
//		                sum(case when b.c_type = '01' then b.n_qty else 0 end) as n_beliqty, 
//		                sum(case when b.c_type = '02' then b.n_qty else 0 end) as n_bonusqty
//		                FROM LG_PLH A
//		                INNER JOIN LG_PLD1 B ON A.c_plno = B.c_plno 
//		                INNER JOIN LG_SPH C ON B.c_spno = C.c_spno
//		                --WHERE B.c_spno BETWEEN @SPNO1 AND @SPNO2 OR C.c_sp BETWEEN @SPNO1 AND @SPNO2 or C.d_spdate BETWEEN @DATE1 AND @DATE2
//		                WHERE (C.d_spdate BETWEEN @DATE1 AND @DATE2)
//			                And (C.c_cusno = Case @CUSNO When '' Then C.c_cusno Else @CUSNO End)
//			                And (B.c_iteno = Case @ITENO When '' Then B.c_iteno Else @ITENO End)
//		                GROUP BY A.c_gdg, A.c_plno, A.d_pldate, A.l_confirm, A.d_update, B.c_spno, B.c_iteno
//		                UNION ALL
//		                SELECT '1' as c_gdg, A.c_adjno as c_no, A.d_adjdate as d_date, 1 as l_confirm, A.d_adjdate, B.c_spno, B.c_iteno, 
//		                sum(case when b.c_type = '01' then b.n_qty else 0 end) as n_beliqty, 
//		                sum(case when b.c_type = '02' then b.n_qty else 0 end) as n_bonusqty
//		                FROM LG_AdjSPH A
//		                INNER JOIN LG_AdjSPD B ON A.c_adjno = B.c_adjno 
//		                INNER JOIN LG_SPH C ON B.c_spno = C.c_spno 
//		                --WHERE B.c_spno BETWEEN @SPNO1 AND @SPNO2 OR C.c_sp BETWEEN @SPNO1 AND @SPNO2 or C.d_spdate BETWEEN @DATE1 AND @DATE2
//		                WHERE (C.d_spdate BETWEEN @DATE1 AND @DATE2)
//			                And (C.c_cusno = Case @CUSNO When '' Then C.c_cusno Else @CUSNO End)
//			                And (B.c_iteno = Case @ITENO When '' Then B.c_iteno Else @ITENO End)
//		                GROUP BY A.c_adjno, A.d_adjdate, A.d_adjdate, B.c_spno, B.c_iteno
//	                ) B ON A.c_spno = B.c_spno AND A.c_iteno = B.c_iteno LEFT JOIN
//	                (
//		                SELECT A.c_dono, A.d_dodate, A.c_plno, B.c_iteno FROM LG_DOH A JOIN
//		                LG_DOD1 B ON A.c_dono = B.c_dono
//		                GROUP BY A.c_dono, A.d_dodate, A.c_plno, B.c_iteno
//	                ) C ON B.c_no = C.c_plno And B.c_iteno = C.c_iteno LEFT JOIN
//	                (
//		                SELECT A.c_expno, A.c_exp, A.c_resi, A.d_resi, B.c_dono FROM LG_ExpH A Join
//		                LG_ExpD B ON A.c_expno = B.c_expno
//		                GROUP BY A.c_expno, A.c_exp, A.c_resi, A.d_resi, B.c_dono
//	                ) D ON C.c_dono = D.c_dono LEFT JOIN
//	                FA_MasItm E ON A.c_iteno = E.c_iteno LEFT JOIN
//	                LG_MsExp F ON D.c_exp = F.c_exp LEFT JOIN
//	                LG_Cusmas G ON A.c_cusno = G.c_cusno LEFT JOIN
//	                LG_MsGudang H ON B.c_gdg = H.c_gdg LEFT JOIN
//	                LG_DatSup I ON E.c_nosup = I.c_nosup LEFT JOIN
//	                (
//		                SELECT A.c_kddivpri, A.v_nmdivpri, B.c_iteno FROM FA_MsDivPri A JOIN
//		                FA_Divpri B ON A.c_kddivpri = B.c_kddivpri
//		                GROUP BY A.c_kddivpri, A.v_nmdivpri, B.c_iteno
//	                ) J ON E.c_iteno = J.c_iteno
//	                WHERE (I.c_nosup = Case @NOSUP When '' Then I.c_nosup Else @NOSUP End)
//		                And (J.c_kddivpri = Case @DIVPRI When '' Then J.c_kddivpri Else @DIVPRI End)");

            
//          #endregion
//        }
//        else
//        {
//          #region Query
//            sbQuery.AppendLine(@"INSERT INTO Temp_LGHistorySP
//
//SELECT A.c_spno, A.d_spdate, A.c_sp, A.c_cusno, A.c_iteno, A.n_beliqty, A.n_bonusqty, A.n_beliacc, A.n_bonusacc, 
//ISNULL(B.c_gdg,''), ISNULL(B.c_no,''), B.d_date, B.l_confirm, B.d_update, ISNULL(B.n_beliqty,0), ISNULL(B.n_bonusqty,0), 
//@SA, A.d_spinsert FROM 
//  (SELECT A.c_spno, A.d_spdate, cast(A.d_entry as DATE) as d_spinsert, A.c_sp, A.c_cusno, B.c_iteno, 
//sum(case when b.c_type = '01' then b.n_qty else 0 end) as n_beliqty, 
//sum(case when b.c_type = '02' then b.n_qty else 0 end) as n_bonusqty,
//sum(case when b.c_type = '01' then b.n_acc else 0 end) as n_beliacc, 
//sum(case when b.c_type = '02' then b.n_acc else 0 end) as n_bonusacc
//FROM LG_SPH A
//INNER JOIN LG_SPD1 B ON A.c_spno = B.c_spno 
//WHERE A.c_cusno = case @CUSNO when '0000' then A.c_cusno else @CUSNO end  and B.c_iteno = case @ITENO when '0000' then B.c_iteno else @ITENO end  and 
//(A.c_sp BETWEEN @SPNO1 AND @SPNO2) And (A.d_spdate BETWEEN @DATE1 AND @DATE2)
//GROUP BY A.c_spno, A.d_spdate, cast(A.d_entry as DATE), A.c_sp, A.c_cusno, B.c_iteno) A
//LEFT OUTER JOIN
//  (SELECT A.c_gdg, A.c_plno as c_no, A.d_pldate as d_date, A.l_confirm, A.d_update, B.c_spno, B.c_iteno, 
//sum(case when b.c_type = '01' then b.n_qty else 0 end) as n_beliqty, 
//sum(case when b.c_type = '02' then b.n_qty else 0 end) as n_bonusqty
//FROM LG_PLH A
//INNER JOIN LG_PLD1 B ON A.c_plno = B.c_plno 
//INNER JOIN LG_SPH C ON B.c_spno = C.c_spno
//WHERE A.c_cusno = case @CUSNO when '0000' then A.c_cusno else @CUSNO end  and B.c_iteno = case @ITENO when '0000' then B.c_iteno else @ITENO end and 
//(C.c_sp BETWEEN @SPNO1 AND @SPNO2) AND (C.d_spdate BETWEEN @DATE1 AND @DATE2)
//GROUP BY A.c_gdg, A.c_plno, A.d_pldate, A.l_confirm, A.d_update, B.c_spno, B.c_iteno
//UNION ALL
//SELECT '1' as c_gdg, A.c_adjno as c_no, A.d_adjdate as d_date, 1 as l_confirm, A.d_adjdate, B.c_spno, B.c_iteno, 
//sum(case when b.c_type = '01' then b.n_qty else 0 end) as n_beliqty, 
//sum(case when b.c_type = '02' then b.n_qty else 0 end) as n_bonusqty
//FROM LG_AdjSPH A
//INNER JOIN LG_AdjSPD B ON A.c_adjno = B.c_adjno 
//INNER JOIN LG_SPH C ON B.c_spno = C.c_spno 
//WHERE C.c_cusno = case @CUSNO when '0000' then C.c_cusno else @CUSNO end  and B.c_iteno = case @ITENO when '0000' then B.c_iteno else @ITENO end and (C.c_sp BETWEEN @SPNO1 AND @SPNO2) AND (C.d_spdate BETWEEN @DATE1 AND @DATE2)
//GROUP BY A.c_adjno, A.d_adjdate, A.d_adjdate, B.c_spno, B.c_iteno) B ON A.c_spno = B.c_spno AND A.c_iteno = B.c_iteno");

//          sbQuery.AppendLine(@"--Insert Temp_LGHistorySP
//SELECT A.c_spno, A.d_spdate, A.c_sp, A.c_cusno, A.c_iteno, A.n_beliqty, A.n_bonusqty, A.n_beliacc, A.n_bonusacc, ISNULL(B.c_gdg,'') c_gdg, ISNULL(B.c_no,'') c_refno, B.d_date d_refdate, B.l_confirm l_refconfir, B.d_update d_refupdate, ISNULL(B.n_beliqty,0) n_refbeliqty, ISNULL(B.n_bonusqty,0) n_refbonusqty, @SA c_user, C.c_dono c_dono, C.d_dodate d_dodate, D.c_resi c_resi, D.d_resi d_resi, E.v_itnam v_itnam, F.v_ket v_expdesc, G.v_cunam v_cunam, H.v_gdgdesc v_gdgdesc, E.c_nosup c_nosup, I.v_nama v_namasup, J.c_kddivpri c_kddivpri, J.v_nmdivpri v_nmdivpri, B.c_no, B.d_date, A.d_spinsert  FROM 
//	                (
//		                SELECT A.c_spno, A.d_spdate, A.c_sp, A.c_cusno, B.c_iteno, cast(A.d_entry as DATE) as d_spinsert,
//		                sum(case when b.c_type = '01' then b.n_qty else 0 end) as n_beliqty, 
//		                sum(case when b.c_type = '02' then b.n_qty else 0 end) as n_bonusqty,
//		                sum(case when b.c_type = '01' then b.n_acc else 0 end) as n_beliacc, 
//		                sum(case when b.c_type = '02' then b.n_acc else 0 end) as n_bonusacc
//		                FROM LG_SPH A
//		                INNER JOIN LG_SPD1 B ON A.c_spno = B.c_spno 
//		                WHERE (A.d_spdate BETWEEN @DATE1 AND @DATE2) AND ((A.c_spno BETWEEN @SPNO1 AND @SPNO2) OR (A.c_sp BETWEEN @SPNO1 AND @SPNO2))
//			                And (A.c_cusno = Case @CUSNO When '' Then A.c_cusno Else @CUSNO End)
//			                And (B.c_iteno = Case @ITENO When '' Then B.c_iteno Else @ITENO End)
//		                GROUP BY A.c_spno, A.d_spdate, A.c_sp, A.c_cusno, B.c_iteno, cast(A.d_entry as DATE)
//	                ) A	LEFT OUTER JOIN
//	                (
//		                SELECT A.c_gdg, A.c_plno as c_no, A.d_pldate as d_date, A.l_confirm, A.d_update, B.c_spno, B.c_iteno, 
//		                sum(case when b.c_type = '01' then b.n_qty else 0 end) as n_beliqty, 
//		                sum(case when b.c_type = '02' then b.n_qty else 0 end) as n_bonusqty
//		                FROM LG_PLH A
//		                INNER JOIN LG_PLD1 B ON A.c_plno = B.c_plno 
//		                INNER JOIN LG_SPH C ON B.c_spno = C.c_spno
//		                WHERE (C.d_spdate BETWEEN @DATE1 AND @DATE2) AND ((B.c_spno BETWEEN @SPNO1 AND @SPNO2) OR (C.c_sp BETWEEN @SPNO1 AND @SPNO2))
//			                And (C.c_cusno = Case @CUSNO When '' Then C.c_cusno Else @CUSNO End)
//			                And (B.c_iteno = Case @ITENO When '' Then B.c_iteno Else @ITENO End)
//		                GROUP BY A.c_gdg, A.c_plno, A.d_pldate, A.l_confirm, A.d_update, B.c_spno, B.c_iteno
//		                UNION ALL
//		                SELECT '1' as c_gdg, A.c_adjno as c_no, A.d_adjdate as d_date, 1 as l_confirm, A.d_adjdate, B.c_spno, B.c_iteno, 
//		                sum(case when b.c_type = '01' then b.n_qty else 0 end) as n_beliqty, 
//		                sum(case when b.c_type = '02' then b.n_qty else 0 end) as n_bonusqty
//		                FROM LG_AdjSPH A
//		                INNER JOIN LG_AdjSPD B ON A.c_adjno = B.c_adjno 
//		                INNER JOIN LG_SPH C ON B.c_spno = C.c_spno 
//		                WHERE (C.d_spdate BETWEEN @DATE1 AND @DATE2) AND ((B.c_spno BETWEEN @SPNO1 AND @SPNO2) OR (C.c_sp BETWEEN @SPNO1 AND @SPNO2))
//			                And (C.c_cusno = Case @CUSNO When '' Then C.c_cusno Else @CUSNO End)
//			                And (B.c_iteno = Case @ITENO When '' Then B.c_iteno Else @ITENO End)
//		                GROUP BY A.c_adjno, A.d_adjdate, A.d_adjdate, B.c_spno, B.c_iteno
//	                ) B ON A.c_spno = B.c_spno AND A.c_iteno = B.c_iteno LEFT JOIN
//	                (
//		                SELECT A.c_dono, A.d_dodate, A.c_plno, B.c_iteno FROM LG_DOH A JOIN
//		                LG_DOD1 B ON A.c_dono = B.c_dono
//		                GROUP BY A.c_dono, A.d_dodate, A.c_plno, B.c_iteno
//	                ) C ON B.c_no = C.c_plno And B.c_iteno = C.c_iteno LEFT JOIN
//	                (
//		                SELECT A.c_expno, A.c_exp, A.c_resi, A.d_resi, B.c_dono FROM LG_ExpH A Join
//		                LG_ExpD B ON A.c_expno = B.c_expno
//		                GROUP BY A.c_expno, A.c_exp, A.c_resi, A.d_resi, B.c_dono
//	                ) D ON C.c_dono = D.c_dono LEFT JOIN
//	                FA_MasItm E ON A.c_iteno = E.c_iteno LEFT JOIN
//	                LG_MsExp F ON D.c_exp = F.c_exp LEFT JOIN
//	                LG_Cusmas G ON A.c_cusno = G.c_cusno LEFT JOIN
//	                LG_MsGudang H ON B.c_gdg = H.c_gdg LEFT JOIN
//	                LG_DatSup I ON E.c_nosup = I.c_nosup LEFT JOIN
//	                (
//		                SELECT A.c_kddivpri, A.v_nmdivpri, B.c_iteno FROM FA_MsDivPri A JOIN
//		                FA_Divpri B ON A.c_kddivpri = B.c_kddivpri
//		                GROUP BY A.c_kddivpri, A.v_nmdivpri, B.c_iteno
//	                ) J ON E.c_iteno = J.c_iteno
//	                WHERE (I.c_nosup = Case @NOSUP When '' Then I.c_nosup Else @NOSUP End)
            //		                And (J.c_kddivpri = Case @DIVPRI When '' Then J.c_kddivpri Else @DIVPRI End)");

#endregion
            #endregion
        }

        ListQuery = db.ExecuteQuery<Temp_LGHistorySP_Test>(sbQuery.ToString(),
          userData,
          (string.IsNullOrEmpty(sp1) ? string.Empty : sp1),
          (string.IsNullOrEmpty(sp2) ? string.Empty : sp2),
          //date1.ToString("yyyy-MM-dd"), date2.ToString("yyyy-MM-dd"),
          date1.Date, date2.Date,
          (string.IsNullOrEmpty(cust) ? string.Empty : cust),
          (string.IsNullOrEmpty(item) ? string.Empty : item),
          (string.IsNullOrEmpty(supl) ? string.Empty : supl),
          (string.IsNullOrEmpty(divSupl) ? string.Empty : divSupl)).ToList();

        #region Old Coded

        //ListQuery = (from q in listData
        //             select new Temp_LGHistorySP()
        //             {
        //               c_cusno = q.c_cusno,
        //               c_dono = q.c_dono,
        //               c_gdg = (string.IsNullOrEmpty(q.c_gdg) ? char.MinValue : (char?)q.c_gdg[0]),
        //               c_iteno = q.c_iteno,
        //               c_kddivpri = q.c_kddivpri,
        //               c_no = q.c_no,
        //               c_nosup = q.c_nosup,
        //               c_resi = q.c_resi,
        //               c_sp = q.c_sp,
        //               c_spno = q.c_spno,
        //               c_user = q.c_user,
        //               d_date = q.d_date,
        //               d_dodate = q.d_dodate,
        //               d_resi = q.d_resi,
        //               d_spdate = q.d_spdate,
        //               d_update = q.d_update,
        //               l_confirm = q.l_confirm,
        //               n_beliacc = q.n_beliacc,
        //               n_beliqty1 = q.n_beliqty1,
        //               n_beliqty2 = q.n_beliqty2,
        //               n_bonusacc = q.n_bonusacc,
        //               n_bonusqty1 = q.n_bonusqty1,
        //               n_bonusqty2 = q.n_bonusqty2,
        //               v_cunam = q.v_cunam,
        //               v_expdesc = q.v_expdesc,
        //               v_gdgdesc = q.v_gdgdesc,
        //               v_itnam = q.v_itnam,
        //               v_namasup = q.v_namasup,
        //               v_nmdivpri = q.v_nmdivpri,
        //             }).ToList();

        #endregion
      }
      catch (Exception ex)
      {
        this.IsError = true;
        this.ErrorMessage = ex.Message;

        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportHistorySP PopulateDataset - {0}", ex.StackTrace);
      }

      sbQuery.Remove(0, sbQuery.Length);

      #endregion

      #region Populate

      if (ListQuery != null)
      {
        try
        {
          table = ListQuery.CopyToDataTableObject();

          if (table != null)
          {
            dataSet = new System.Data.DataSet();

            dataSet.Tables.Add(table);
          }
        }
        catch (Exception ex)
        {
          this.IsError = true;
          this.ErrorMessage = ex.Message;

          Logger.WriteLine(
            "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportHistorySP PopulateDataset - {0}", ex.StackTrace);
        }
      }

      #endregion

      if (ListQuery != null)
      {
        ListQuery.Clear();
      }

      return dataSet;
    }
  }
}