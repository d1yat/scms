using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsModel;
using ScmsSoaLibrary.Parser;
//using ScmsSoaLibrary.Core.Crypto;
using ScmsSoaLibrary.Commons;
using System.Data.Common;
using ScmsSoaLibrary.Core.Threading;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaLibrary.Bussiness
{
  class Closing
  {
    #region Internal Class

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

    internal class LG_STOCK
    {
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

    #endregion

    public string ClosingLog(ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false;
      string result = null;
      
      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      //ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructureField field = null;
      string nipEntry = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      Closing closing = new Closing();

      //int nLoop = 0;

      IDictionary<string, string> dic = null;

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }

      //int totalDetails = 0;

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Submit", StringComparison.OrdinalIgnoreCase))
        {
          #region Submit

          int Tahun = 2011;
          int Bulan = 11;
          int ExtYear = (Bulan == 1 ? Tahun - 1 : Tahun);
          int ExtMonth = (Bulan == 1 ? 12 : Bulan - 1);
          string[] a = new string[] { "01", "02" };
          string[] tipeRNBeli = new string[] { "01", "03" };
          string tipRNRS = "02";
          string tipRNRetur = "04";

          List<LG_TRANSAKSI> Transaksi = new List<LG_TRANSAKSI>();
          List<LG_TRANSAKSI> tmp = null;
          List<LG_TRANSAKSI> tmp3 = new List<LG_TRANSAKSI>();
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

          List<LG_STOCK> Stock = new List<LG_STOCK>();
          List<LG_STOCK> tmpStock = null;
          List<LG_STOCK> tmpStock1 = new List<LG_STOCK>();

          #region Closing Transaksi

          tmp = (from q in db.LG_ClosePOs
                 where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                 select new LG_TRANSAKSI()
                 {
                   c_gdg = q.c_gdg,
                   c_no = q.c_pono,
                   c_iteno = q.c_iteno,
                   n_gqty = q.n_qty.Value
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          tmp = (from q in db.LG_POHs
                 join q1 in db.LG_POD1s on new { q.c_gdg, q.c_pono } equals new { q1.c_gdg, q1.c_pono }
                 where q.d_podate.Value.Year == Tahun && q.d_podate.Value.Month == Bulan
                 select new LG_TRANSAKSI()
                 {
                   c_no = q.c_pono,
                   c_iteno = q1.c_iteno,
                   n_gqty = q1.n_qty.Value
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          tmp2 = (from q1 in db.LG_RNHs
                  join q2 in db.LG_RND2s on new { q1.c_gdg, q1.c_rnno } equals new { q2.c_gdg, q2.c_rnno }
                  where q1.d_rndate.Value.Year == Tahun && q1.d_rndate.Value.Month == Bulan &&
                  q1.c_type == "01" && q2.c_type == "01"
                  group new { q1, q2 } by new { q1.c_gdg, q2.c_no, q2.c_iteno } into g
                  select new LG_TRANSAKSI()
                  {
                    c_no = g.Key.c_no,
                    c_iteno = g.Key.c_iteno,
                    n_gqty = g.Sum(x => x.q2.n_gqty.Value),
                  }).ToList();


          tmp2 = (from q in tmp1
                  join q3 in
                    (from q1 in db.LG_RNHs
                     join q2 in db.LG_RND2s on new { q1.c_gdg, q1.c_rnno } equals new { q2.c_gdg, q2.c_rnno }
                     where q1.d_rndate.Value.Year == Tahun && q1.d_rndate.Value.Month == Bulan &&
                     q1.c_type == "01" && q2.c_type == "01"
                     group new { q1, q2 } by new { q1.c_gdg, q2.c_no, q2.c_iteno } into g
                     select new LG_TRANSAKSI()
                     {
                       c_gdg = g.Key.c_gdg,
                       c_no = g.Key.c_no,
                       c_iteno = g.Key.c_iteno,
                       n_gqty = g.Sum(x => x.q2.n_gqty.Value),
                     }) on new { q.c_gdg, q.c_no, q.c_iteno } equals new { q3.c_gdg, q3.c_no, q3.c_iteno } into Left
                  from gLeft in Left.DefaultIfEmpty()
                  select new LG_TRANSAKSI()
                  {
                    c_gdg = q.c_gdg,
                    c_no = q.c_no,
                    c_iteno = q.c_iteno,
                    n_gqty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty)
                  }).ToList();

          tmp3 = (from q in tmp2
                  join q3 in
                    (from q1 in db.LG_AdjPOHs
                     join q2 in db.LG_AdjPODs on new { q1.c_gdg, q1.c_adjno } equals new { q2.c_gdg, q2.c_adjno }
                     where q1.d_adjdate.Value.Year == Tahun && q1.d_adjdate.Value.Month == Bulan
                     group new { q1, q2 } by new { q1.c_gdg, q2.c_adjno, q2.c_iteno } into g
                     select new LG_TRANSAKSI()
                     {
                       c_gdg = g.Key.c_gdg,
                       c_no = g.Key.c_adjno,
                       c_iteno = g.Key.c_iteno,
                       n_gqty = g.Sum(x => x.q2.n_qty.Value),
                     }) on new { q.c_gdg, q.c_no, q.c_iteno } equals new { q3.c_gdg, q3.c_no, q3.c_iteno } into Left
                  from gLeft in Left.DefaultIfEmpty()
                  select new LG_TRANSAKSI()
                  {
                    c_gdg = q.c_gdg,
                    c_no = q.c_no,
                    c_iteno = q.c_iteno,
                    n_gqty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty)
                  }).Distinct().ToList();

          TmpPo = (from q in tmp3
                   where q.n_gqty > 0
                   select new LG_TRANSAKSI()
                   {
                     c_gdg = q.c_gdg,
                     c_no = q.c_no,
                     c_iteno = q.c_iteno,
                     n_gqty = q.n_gqty
                   }).ToList();

          tmp3.Clear();
          tmp2.Clear();

          //claim

          tmp = (from q in db.LG_CloseClaims
                 where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                 select new LG_TRANSAKSI()
                 {
                   c_no = q.c_claimno,
                   c_iteno = q.c_iteno,
                   n_gqty = q.n_qty.Value
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          tmp = (from q in db.LG_ClaimHs
                 join q1 in db.LG_ClaimD1s on new { q.c_claimno } equals new { q1.c_claimno }
                 where q.d_claimdate.Value.Year == Tahun && q.d_claimdate.Value.Month == Bulan
                 group q1 by new { q1.c_claimno, q1.c_iteno } into g
                 select new LG_TRANSAKSI()
                 {
                   c_no = g.Key.c_claimno,
                   c_iteno = g.Key.c_iteno,
                   n_gqty = g.Sum(x => x.n_qty.Value)
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          tmp2 = (from q in tmp1
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
                     }) on new { q.c_gdg, q.c_no, q.c_iteno } equals new { q3.c_gdg, q3.c_no, q3.c_iteno } into Left
                  from gLeft in Left.DefaultIfEmpty()
                  select new LG_TRANSAKSI()
                  {
                    c_gdg = q.c_gdg,
                    c_no = q.c_no,
                    c_iteno = q.c_iteno,
                    n_gqty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty)
                  }).ToList();

          TmpClaim = (from q in tmp3
                      where q.n_gqty > 0
                      select new LG_TRANSAKSI()
                      {
                        c_no = q.c_no,
                        c_iteno = q.c_iteno,
                        n_gqty = q.n_gqty
                      }).ToList();

          tmp3.Clear();
          tmp2.Clear();

          //claim Acc

          tmp = (from q in db.LG_CloseClaimAccs
                 where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                 select new LG_TRANSAKSI()
                 {
                   c_no = q.c_claimaccno,
                   c_iteno = q.c_iteno,
                   n_gqty = q.n_qty.Value
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          tmp = (from q in db.LG_ClaimAccHes
                 join q1 in db.LG_ClaimAccDs on new { q.c_claimaccno } equals new { q1.c_claimaccno }
                 where q.d_claimaccdate.Value.Year == Tahun && q.d_claimaccdate.Value.Month == Bulan
                 group q1 by new { q1.c_claimaccno, q1.c_iteno } into g
                 select new LG_TRANSAKSI()
                 {
                   c_no = g.Key.c_claimaccno,
                   c_iteno = g.Key.c_iteno,
                   n_gqty = g.Sum(x => x.n_qtyacc.Value)
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          tmp2 = (from q in tmp1
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
                  select new LG_TRANSAKSI()
                  {
                    c_no = q.c_no,
                    c_iteno = q.c_iteno,
                    n_gqty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty)
                  }).ToList();

          tmp3 = (from q in tmp2
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
                     }) on new { q.c_no, q.c_iteno } equals new { q3.c_no, q3.c_iteno } into Left
                  from gLeft in Left.DefaultIfEmpty()
                  select new LG_TRANSAKSI()
                  {
                    c_no = q.c_no,
                    c_iteno = q.c_iteno,
                    n_gqty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty)
                  }).ToList();

          TmpClaimAcc = (from q in tmp3
                         where q.n_gqty > 0
                         select new LG_TRANSAKSI()
                         {
                           c_no = q.c_no,
                           c_iteno = q.c_iteno,
                           n_gqty = q.n_gqty
                         }).ToList();

          tmp3.Clear();
          tmp2.Clear();

          //Retur Supplier

          tmp = (from q in db.LG_CloseRs
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

          tmp1.AddRange(tmp);
          tmp.Clear();

          tmp = (from q in db.LG_RSHes
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
                   n_gqty = g.Sum(x => x.n_gqty.Value),
                   n_bqty = g.Sum(x => x.n_bqty.Value)
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          tmp2 = (from q in tmp1
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
                  select new LG_TRANSAKSI()
                  {
                    c_gdg = q.c_gdg,
                    c_no = q.c_no,
                    c_iteno = q.c_iteno,
                    c_batch = q.c_batch,
                    c_noref = q.c_noref,
                    n_gqty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty),
                    n_bqty = q.n_bqty - (gLeft == null ? 0m : gLeft.n_bqty)
                  }).ToList();

          tmp3 = (from q in tmp2
                  join q3 in
                    (from q1 in db.LG_FBRHs
                     join q2 in db.LG_FBRD2s on q1.c_fbno equals q2.c_fbno
                     where q1.d_fbdate.Value.Year == Tahun && q1.d_fbdate.Value.Month == Bulan
                     group q2 by new { q2.c_rsno, q2.c_rnno } into g
                     select new LG_TRANSAKSI()
                     {
                       c_no = g.Key.c_rsno,
                       c_iteno = g.Key.c_rnno
                     }) on new { q.c_no, q.c_noref } equals new { q3.c_no, q3.c_noref } into Left
                  from gLeft in Left.DefaultIfEmpty()
                  select new LG_TRANSAKSI()
                  {
                    c_gdg = q.c_gdg,
                    c_no = q.c_no,
                    c_iteno = q.c_iteno,
                    c_batch = q.c_batch,
                    c_noref = q.c_noref,
                    n_gqty = (gLeft == null ? q.n_gqty : 0m),
                    n_bqty = (gLeft == null ? q.n_bqty : 0m)
                  }).ToList();


          TmpRS = (from q in tmp3
                   where q.n_gqty > 0
                   select new LG_TRANSAKSI()
                   {
                     c_no = q.c_no,
                     c_iteno = q.c_iteno,
                     n_gqty = q.n_gqty
                   }).ToList();

          tmp3.Clear();
          tmp2.Clear();

          //Packing List

          tmp = (from q in db.LG_ClosePLs
                 where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                 select new LG_TRANSAKSI()
                 {
                   c_no = q.c_plno
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          tmp = (from q in db.LG_PLHs
                 join q1 in db.LG_PLD1s on q.c_plno equals q1.c_plno
                 where q.d_pldate.Value.Year == Tahun && q.d_pldate.Value.Month == Bulan
                 && q.l_confirm == false
                 select new LG_TRANSAKSI()
                 {
                   c_no = q.c_plno
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          TmpPL = (from q in tmp
                   select new LG_TRANSAKSI()
                   {
                     c_no = q.c_no
                   }).ToList();

          tmp.Clear();

          //Packing List Confirm

          tmp = (from q in db.LG_ClosePLConfirms
                 where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                 select new LG_TRANSAKSI()
                 {
                   c_no = q.c_plno,
                   c_iteno = q.c_iteno,
                   c_noref = q.c_spno,
                   c_type = q.c_type,
                   c_batch = q.c_batch,
                   n_gqty = q.n_qty.Value
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          tmp = (from q in db.LG_PLHs
                 join q1 in db.LG_PLD1s on q.c_plno equals q1.c_plno
                 where q.d_pldate.Value.Year == Tahun && q.d_pldate.Value.Month == Bulan
                 select new LG_TRANSAKSI()
                 {
                   c_no = q.c_plno,
                   c_iteno = q1.c_iteno,
                   c_noref = q1.c_spno,
                   c_type = q1.c_type,
                   c_batch = q1.c_batch,
                   n_gqty = q1.n_qty.Value
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          tmp2 = (from q in tmp1
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
                  select new LG_TRANSAKSI()
                  {
                    c_no = q.c_no,
                    c_iteno = q.c_iteno,
                    c_noref = q.c_noref,
                    c_type = q.c_type,
                    c_batch = q.c_batch,
                    n_gqty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty)
                  }).ToList();

          TmpPLConf = (from q in tmp2
                       where q.n_gqty > 0
                       select new LG_TRANSAKSI()
                       {
                         c_no = q.c_no,
                         c_iteno = q.c_iteno,
                         c_noref = q.c_noref,
                         c_type = q.c_type,
                         c_batch = q.c_batch,
                         n_gqty = q.n_gqty
                       }).ToList();

          tmp2.Clear();
          tmp3.Clear();

          //Delivery Ordert

          tmp = (from q in db.LG_CloseDOs
                 where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                 select new LG_TRANSAKSI()
                 {
                   c_no = q.c_dono
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          tmp = (from q in db.LG_DOHs
                 join q1 in db.LG_DOD1s on q.c_dono equals q1.c_dono
                 where q.d_dodate.Value.Year == Tahun && q.d_dodate.Value.Month == Bulan
                 select new LG_TRANSAKSI()
                 {
                   c_no = q.c_dono
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          tmp2 = (from q in tmp1
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
                  select new LG_TRANSAKSI()
                  {
                    c_no = q.c_no
                  }).ToList();

          TmpDO = (from q in tmp2
                   select new LG_TRANSAKSI()
                   {
                     c_no = q.c_no
                   }).ToList();

          tmp3.Clear();
          tmp2.Clear();

          //SPG

          tmp = (from q in db.LG_CloseSPGs
                 where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                 select new LG_TRANSAKSI()
                 {
                   c_gdg = q.c_gdg,
                   c_no = q.c_spgno,
                   c_iteno = q.c_iteno,
                   n_gqty = Convert.ToDecimal(q.n_qty)
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          tmp = (from q in db.LG_SPGHs
                 join q1 in db.LG_SPGD1s on q.c_spgno equals q1.c_spgno
                 where q.d_spgdate.Value.Year == Tahun && q.d_spgdate.Value.Month == Bulan
                 select new LG_TRANSAKSI()
                 {
                   c_gdg = q.c_gdg1,
                   c_no = q.c_spgno,
                   c_iteno = q1.c_iteno,
                   n_gqty = q1.n_qty.HasValue ? q1.n_qty.Value : 0m
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          tmp2 = (from q in tmp1
                  join q3 in
                    (from q1 in db.LG_SJHs
                     join q2 in db.LG_SJD1s on new { q1.c_gdg, q1.c_sjno } equals new { q2.c_gdg, q2.c_sjno }
                     where q1.d_sjdate.Value.Year == Tahun && q1.d_sjdate.Value.Month == Bulan
                     group new { q1, q2 } by new { q1.c_gdg2, q2.c_spgno, q2.c_iteno } into g
                     select new LG_TRANSAKSI()
                     {
                       c_gdg = g.Key.c_gdg2.HasValue ? g.Key.c_gdg2.Value : '1',
                       c_no = g.Key.c_spgno,
                       c_iteno = g.Key.c_iteno,
                       n_gqty = g.Sum(x => x.q2.n_gqty.Value)
                     }) on new { q.c_gdg, q.c_no, q.c_iteno } equals new { q3.c_gdg, q3.c_no, q3.c_iteno } into Left
                  from gLeft in Left.DefaultIfEmpty()
                  select new LG_TRANSAKSI()
                  {
                    c_gdg = q.c_gdg,
                    c_no = q.c_no,
                    c_iteno = q.c_no,
                    n_gqty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty)
                  }).ToList();

          TmpSPG = (from q in tmp2
                    where q.n_gqty > 0
                    select new LG_TRANSAKSI()
                    {
                      c_gdg = q.c_gdg,
                      c_no = q.c_no,
                      c_iteno = q.c_iteno,
                      n_gqty = q.n_gqty
                    }).ToList();

          tmp3.Clear();
          tmp2.Clear();

          //SJ Exp

          tmp = (from q in db.LG_CloseSJExps
                 where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                 select new LG_TRANSAKSI()
                 {
                   c_gdg = q.c_gdg,
                   c_no = q.c_sjno
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          tmp = (from q in db.LG_SJHs
                 where q.d_sjdate.Value.Year == Tahun && q.d_sjdate.Value.Month == Bulan
                 select new LG_TRANSAKSI()
                 {
                   c_gdg = q.c_gdg,
                   c_no = q.c_sjno
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();


          tmp2 = (from q in tmp1
                  join q3 in
                    (from q1 in db.LG_ExpHs
                     join q2 in db.LG_ExpDs on q1.c_expno equals q2.c_expno
                     where q1.d_expdate.Value.Year == Tahun && q1.d_expdate.Value.Month == Bulan
                     && q1.c_type == "01"
                     select new LG_TRANSAKSI()
                     {
                       c_gdg = q1.c_gdg.HasValue ? q1.c_gdg.Value : '1',
                       c_noref = q2.c_dono
                     }) on new { q.c_gdg, q.c_no } equals new { q3.c_gdg, c_no = q3.c_noref } into Left
                  from gLeft in Left.DefaultIfEmpty()
                  where gLeft == null
                  select new LG_TRANSAKSI()
                  {
                    c_gdg = q.c_gdg,
                    c_no = q.c_no
                  }).ToList();

          TmpSJExp = (from q in tmp2
                      where q.n_gqty > 0
                      select new LG_TRANSAKSI()
                      {
                        c_gdg = q.c_gdg,
                        c_no = q.c_no
                      }).ToList();

          tmp3.Clear();
          tmp2.Clear();

          //SJ RN

          tmp = (from q in db.LG_CloseSJExps
                 where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                 select new LG_TRANSAKSI()
                 {
                   c_gdg = q.c_gdg,
                   c_no = q.c_sjno
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          tmp = (from q in db.LG_SJHs
                 where q.d_sjdate.Value.Year == Tahun && q.d_sjdate.Value.Month == Bulan
                 && q.l_confirm == false
                 select new LG_TRANSAKSI()
                 {
                   c_gdg = q.c_gdg,
                   c_no = q.c_sjno
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          TmpSJRN = (from q in tmp1
                     where q.n_gqty > 0
                     select new LG_TRANSAKSI()
                     {
                       c_gdg = q.c_gdg,
                       c_no = q.c_no
                     }).ToList();

          tmp3.Clear();
          tmp2.Clear();

          //Memo Combo

          tmp = (from q in db.LG_CloseMemos
                 where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                 select new LG_TRANSAKSI()
                 {
                   c_gdg = q.c_gdg,
                   c_no = q.c_memono,
                   c_iteno = q.c_iteno,
                   n_gqty = q.n_qty.Value
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          tmp = (from q in db.MK_MemoHs
                 join q1 in db.MK_MemoD1s on new { q.c_gdg, q.c_memono } equals new { q1.c_gdg, q1.c_memono }
                 where q.d_memodate.Value.Year == Tahun && q.d_memodate.Value.Month == Bulan
                 select new LG_TRANSAKSI()
                 {
                   c_gdg = q.c_gdg,
                   c_no = q.c_memono,
                   c_iteno = q1.c_iteno,
                   n_gqty = q1.n_qty.Value
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          tmp2 = (from q in tmp1
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
                  select new LG_TRANSAKSI()
                  {
                    c_gdg = q.c_gdg,
                    c_no = q.c_no,
                    c_iteno = q.c_iteno,
                    n_gqty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty)
                  }).ToList();

          tmp3 = (from q in tmp2
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
                     }) on new { q.c_no, q.c_noref } equals new { q3.c_no, q3.c_noref } into Left
                  from gLeft in Left.DefaultIfEmpty()
                  select new LG_TRANSAKSI()
                  {
                    c_gdg = q.c_gdg,
                    c_no = q.c_no,
                    c_iteno = q.c_iteno,
                    n_gqty = q.n_gqty - (gLeft == null ? q.n_gqty : 0m)
                  }).ToList();

          TmpMemo = (from q in tmp3
                     where q.n_gqty > 0
                     select new LG_TRANSAKSI()
                     {
                       c_gdg = q.c_gdg,
                       c_no = q.c_no
                     }).ToList();

          tmp2.Clear();
          tmp3.Clear();

          //Donasi

          tmp = (from q in db.LG_CloseMTs
                 where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                 select new LG_TRANSAKSI()
                 {
                   c_gdg = q.c_gdg,
                   c_no = q.c_mtno,
                   c_iteno = q.c_iteno,
                   n_gqty = q.n_qty.Value
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          tmp = (from q in db.MK_MTHs
                 join q1 in db.MK_MTDs on new { q.c_gdg, q.c_mtno } equals new { q1.c_gdg, q1.c_mtno }
                 where q.d_mtdate.Value.Year == Tahun && q.d_mtdate.Value.Month == Bulan
                 select new LG_TRANSAKSI()
                 {
                   c_gdg = q.c_gdg,
                   c_no = q.c_mtno,
                   c_iteno = q1.c_iteno,
                   n_gqty = q1.n_qty.Value
                 }).ToList();

          tmp1.AddRange(tmp);
          tmp.Clear();

          tmp2 = (from q in tmp1
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
                  where gLeft == null
                  select new LG_TRANSAKSI()
                  {
                    c_gdg = q.c_gdg,
                    c_no = q.c_no,
                    c_iteno = q.c_iteno,
                    n_gqty = q.n_gqty - (gLeft == null ? 0m : gLeft.n_gqty)
                  }).ToList();

          tmp3 = (from q in tmp2
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
                     }) on new { q.c_no, q.c_noref } equals new { q3.c_no, q3.c_noref } into Left
                  from gLeft in Left.DefaultIfEmpty()
                  select new LG_TRANSAKSI()
                  {
                    c_gdg = q.c_gdg,
                    c_no = q.c_no,
                    c_iteno = q.c_iteno,
                    n_gqty = q.n_gqty - (gLeft == null ? q.n_gqty : 0m)
                  }).ToList();

          TmpST = (from q in tmp3
                   where q.n_gqty > 0
                   select new LG_TRANSAKSI()
                   {
                     c_gdg = q.c_gdg,
                     c_no = q.c_no
                   }).ToList();

          tmp2.Clear();
          tmp3.Clear();

          #endregion

          #region Closing Stock

          tmpStock = (from q in db.LG_Stocks
                      where (q.s_tahun == ExtYear) && (q.t_bulan == ExtMonth)
                      select new LG_STOCK()
                      {
                        c_gdg = q.c_gdg,
                        c_rnno = q.c_no,
                        c_iteno = q.c_iteno,
                        c_batch = q.c_batch,
                        n_gqty = (q.n_gqty.Value),
                        n_bqty = (q.n_bqty.Value)
                      }).ToList();

          tmpStock1.AddRange(tmpStock);
          tmpStock.Clear();

          tmpStock = (from q in db.LG_RNHs
                      join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                      where (q.d_rndate.Value.Year == Tahun) && (q.d_rndate.Value.Month == Bulan)
                      && tipeRNBeli.Contains(q.c_type)
                      group new { q, q1 } by new { q.c_gdg, q.c_rnno, q1.c_iteno, q1.c_batch } into sumg
                      select new LG_STOCK()
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
                      select new LG_STOCK()
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
                      select new LG_STOCK()
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
                      where (q.d_sjdate.Value.Year == Tahun) && (q.d_sjdate.Value.Month == Bulan)
                      && q.l_confirm == true
                      group new { q, q1 } by new { q.c_gdg2, q1.c_rnno, q1.c_iteno, q1.c_batch } into sumg
                      select new LG_STOCK()
                      {
                        c_gdg = (sumg.Key.c_gdg2.HasValue ? sumg.Key.c_gdg2.Value : '1'),
                        c_rnno = sumg.Key.c_rnno,
                        c_iteno = sumg.Key.c_iteno,
                        c_batch = sumg.Key.c_batch,
                        n_gqty = (sumg.Sum(x => x.q1.n_gqty.HasValue ? x.q1.n_gqty.Value : 0m)),
                        n_bqty = (sumg.Sum(x => x.q1.n_bqty.HasValue ? x.q1.n_bqty.Value : 0m))
                      }).ToList();

          tmpStock1.AddRange(tmpStock);
          tmpStock.Clear();

          // Combo
          tmpStock = (from q in db.LG_ComboHs
                      where (q.d_combodate.Value.Year == Tahun) && (q.d_combodate.Value.Month == Bulan)
                      && q.c_type == "01"
                      select new LG_STOCK()
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
                   group q by new { q.c_gdg, q.c_rnno, q.c_iteno, q.c_batch } into g
                   select new LG_STOCK()
                   {
                     c_gdg = g.Key.c_gdg,
                     c_rnno = g.Key.c_rnno,
                     c_iteno = g.Key.c_iteno,
                     c_batch = g.Key.c_batch,
                     n_gqty = g.Sum(x => x.n_gqty),
                     n_bqty = g.Sum(x => x.n_bqty)
                   }).ToList();

          tmpStock1.Clear();

          // Input RC
          tmpStock = (from q in
                        (from q2 in db.LG_RCHes
                         join q3 in db.LG_RCD1s on new { q2.c_gdg, q2.c_rcno } equals new { q3.c_gdg, q3.c_rcno }
                         where (q2.d_rcdate.Value.Year == Tahun) && (q2.d_rcdate.Value.Month == Bulan)
                         group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                         select new LG_STOCK()
                         {
                           c_gdg = sumg.Key.c_gdg,
                           c_rnno = sumg.Key.c_rnno,
                           c_iteno = sumg.Key.c_iteno,
                           c_batch = sumg.Key.c_batch,
                           n_gqty = 0m,
                           n_bqty = 0m
                         })
                      join q1 in Stock on new { q.c_gdg, q.c_rnno, q.c_iteno, q.c_batch } equals new { q1.c_gdg, q1.c_rnno, q1.c_iteno, q1.c_batch } into Left
                      from gLeft in Left.DefaultIfEmpty()
                      where Left == null
                      select new LG_STOCK()
                      {
                        c_gdg = q.c_gdg,
                        c_rnno = q.c_rnno,
                        c_iteno = q.c_iteno,
                        c_batch = q.c_batch,
                        n_gqty = q.n_gqty,
                        n_bqty = q.n_bqty
                      }).ToList();

          tmpStock1.AddRange(tmpStock);
          tmpStock.Clear();

          // Input RS
          tmpStock = (from q in
                        (from q2 in db.LG_RSHes
                         join q3 in db.LG_RSD2s on new { q2.c_gdg, q2.c_rsno } equals new { q3.c_gdg, q3.c_rsno }
                         where (q2.d_rsdate.Value.Year == Tahun) && (q2.d_rsdate.Value.Month == Bulan)
                         group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                         select new LG_STOCK()
                         {
                           c_gdg = sumg.Key.c_gdg,
                           c_rnno = sumg.Key.c_rnno,
                           c_iteno = sumg.Key.c_iteno,
                           c_batch = sumg.Key.c_batch,
                           n_gqty = 0m,
                           n_bqty = 0m
                         })
                      join q1 in Stock on new { q.c_gdg, q.c_rnno, q.c_iteno, q.c_batch } equals new { q1.c_gdg, q1.c_rnno, q1.c_iteno, q1.c_batch } into Left
                      from gLeft in Left.DefaultIfEmpty()
                      where Left == null
                      select new LG_STOCK()
                      {
                        c_gdg = q.c_gdg,
                        c_rnno = q.c_rnno,
                        c_iteno = q.c_iteno,
                        c_batch = q.c_batch,
                        n_gqty = q.n_gqty,
                        n_bqty = q.n_bqty
                      }).ToList();

          tmpStock1.AddRange(tmpStock);
          tmpStock.Clear();


          // Input SJ
          tmpStock = (from q in
                        (from q2 in db.LG_SJHs
                         join q3 in db.LG_SJD2s on new { q2.c_gdg, q2.c_sjno } equals new { q3.c_gdg, q3.c_sjno }
                         where (q2.d_sjdate.Value.Year == Tahun) && (q2.d_sjdate.Value.Month == Bulan)
                         group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                         select new LG_STOCK()
                         {
                           c_gdg = sumg.Key.c_gdg,
                           c_rnno = sumg.Key.c_rnno,
                           c_iteno = sumg.Key.c_iteno,
                           c_batch = sumg.Key.c_batch,
                           n_gqty = 0m,
                           n_bqty = 0m
                         })
                      join q1 in Stock on new { q.c_gdg, q.c_rnno, q.c_iteno, q.c_batch } equals new { q1.c_gdg, q1.c_rnno, q1.c_iteno, q1.c_batch } into Left
                      from gLeft in Left.DefaultIfEmpty()
                      where Left == null
                      select new LG_STOCK()
                      {
                        c_gdg = q.c_gdg,
                        c_rnno = q.c_rnno,
                        c_iteno = q.c_iteno,
                        c_batch = q.c_batch,
                        n_gqty = q.n_gqty,
                        n_bqty = q.n_bqty
                      }).ToList();

          tmpStock1.AddRange(tmpStock);
          tmpStock.Clear();

          // Input PL
          tmpStock = (from q in
                        (from q2 in db.LG_PLHs
                         join q3 in db.LG_PLD2s on q2.c_plno equals q3.c_plno
                         where (q2.d_pldate.Value.Year == Tahun) && (q2.d_pldate.Value.Month == Bulan)
                         group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                         select new LG_STOCK()
                         {
                           c_gdg = sumg.Key.c_gdg.HasValue ? sumg.Key.c_gdg.Value : '1',
                           c_rnno = sumg.Key.c_rnno,
                           c_iteno = sumg.Key.c_iteno,
                           c_batch = sumg.Key.c_batch,
                           n_gqty = 0m,
                           n_bqty = 0m
                         })
                      join q1 in Stock on new { q.c_gdg, q.c_rnno, q.c_iteno, q.c_batch } equals new { q1.c_gdg, q1.c_rnno, q1.c_iteno, q1.c_batch } into Left
                      from gLeft in Left.DefaultIfEmpty()
                      where Left == null
                      select new LG_STOCK()
                      {
                        c_gdg = q.c_gdg,
                        c_rnno = q.c_rnno,
                        c_iteno = q.c_iteno,
                        c_batch = q.c_batch,
                        n_gqty = q.n_gqty,
                        n_bqty = q.n_bqty
                      }).ToList();

          tmpStock1.AddRange(tmpStock);
          tmpStock.Clear();

          // Input STH
          tmpStock = (from q in
                        (from q2 in db.LG_STHs
                         join q3 in db.LG_STD2s on new { q2.c_gdg, q2.c_stno } equals new { q3.c_gdg, q3.c_stno }
                         where (q2.d_stdate.Value.Year == Tahun) && (q2.d_stdate.Value.Month == Bulan)
                         group new { q2, q3 } by new { q2.c_gdg, q3.c_no, q3.c_iteno, q3.c_batch } into sumg
                         select new LG_STOCK()
                         {
                           c_gdg = sumg.Key.c_gdg,
                           c_rnno = sumg.Key.c_no,
                           c_iteno = sumg.Key.c_iteno,
                           c_batch = sumg.Key.c_batch,
                           n_gqty = 0m,
                           n_bqty = 0m
                         })
                      join q1 in Stock on new { q.c_gdg, q.c_rnno, q.c_iteno, q.c_batch } equals new { q1.c_gdg, q1.c_rnno, q1.c_iteno, q1.c_batch } into Left
                      from gLeft in Left.DefaultIfEmpty()
                      where Left == null
                      select new LG_STOCK()
                      {
                        c_gdg = q.c_gdg,
                        c_rnno = q.c_rnno,
                        c_iteno = q.c_iteno,
                        c_batch = q.c_batch,
                        n_gqty = q.n_gqty,
                        n_bqty = q.n_bqty
                      }).ToList();

          tmpStock1.AddRange(tmpStock);
          tmpStock.Clear();

          // Input Combo
          tmpStock = (from q in
                        (from q2 in db.LG_ComboHs
                         join q3 in db.LG_ComboD2s on new { q2.c_gdg, q2.c_combono } equals new { q3.c_gdg, q3.c_combono }
                         where (q2.d_combodate.Value.Year == Tahun) && (q2.d_combodate.Value.Month == Bulan)
                         && q2.c_type == "01"
                         group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                         select new LG_STOCK()
                         {
                           c_gdg = sumg.Key.c_gdg,
                           c_rnno = sumg.Key.c_rnno,
                           c_iteno = sumg.Key.c_iteno,
                           c_batch = sumg.Key.c_batch,
                           n_gqty = 0m,
                           n_bqty = 0m
                         })
                      join q1 in Stock on new { q.c_gdg, q.c_rnno, q.c_iteno, q.c_batch } equals new { q1.c_gdg, q1.c_rnno, q1.c_iteno, q1.c_batch } into Left
                      from gLeft in Left.DefaultIfEmpty()
                      where Left == null
                      select new LG_STOCK()
                      {
                        c_gdg = q.c_gdg,
                        c_rnno = q.c_rnno,
                        c_iteno = q.c_iteno,
                        c_batch = q.c_batch,
                        n_gqty = q.n_gqty,
                        n_bqty = q.n_bqty
                      }).ToList();

          tmpStock1.AddRange(tmpStock);
          tmpStock.Clear();

          // Input Adjustment
          tmpStock = (from q in
                        (from q2 in db.LG_AdjustHs
                         join q3 in db.LG_AdjustD2s on new { q2.c_gdg, q2.c_adjno } equals new { q3.c_gdg, q3.c_adjno }
                         where (q2.d_adjdate.Value.Year == Tahun) && (q2.d_adjdate.Value.Month == Bulan)
                         group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                         select new LG_STOCK()
                         {
                           c_gdg = sumg.Key.c_gdg,
                           c_rnno = sumg.Key.c_rnno,
                           c_iteno = sumg.Key.c_iteno,
                           c_batch = sumg.Key.c_batch,
                           n_gqty = 0m,
                           n_bqty = 0m
                         })
                      join q1 in Stock on new { q.c_gdg, q.c_rnno, q.c_iteno, q.c_batch } equals new { q1.c_gdg, q1.c_rnno, q1.c_iteno, q1.c_batch } into Left
                      from gLeft in Left.DefaultIfEmpty()
                      where Left == null
                      select new LG_STOCK()
                      {
                        c_gdg = q.c_gdg,
                        c_rnno = q.c_rnno,
                        c_iteno = q.c_iteno,
                        c_batch = q.c_batch,
                        n_gqty = q.n_gqty,
                        n_bqty = q.n_bqty
                      }).ToList();

          tmpStock1.AddRange(tmpStock);
          tmpStock.Clear();

          Stock = (from q in tmpStock1
                   group q by new { q.c_gdg, q.c_rnno, q.c_iteno, q.c_batch } into g
                   select new LG_STOCK()
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
          Stock = (from q in
                     (from q2 in db.LG_RCHes
                      join q3 in db.LG_RCD1s on new { q2.c_gdg, q2.c_rcno } equals new { q3.c_gdg, q3.c_rcno }
                      where (q2.d_rcdate.Value.Year == Tahun) && (q2.d_rcdate.Value.Month == Bulan)
                      group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                      select new LG_STOCK()
                      {
                        c_gdg = sumg.Key.c_gdg,
                        c_rnno = sumg.Key.c_rnno,
                        c_iteno = sumg.Key.c_iteno,
                        c_batch = sumg.Key.c_batch,
                        n_gqty = sumg.Sum(x => x.q3.c_type == "01" ? x.q3.n_qty.Value : 0m),
                        n_bqty = sumg.Sum(x => x.q3.c_type == "01" ? x.q3.n_qty.Value : 0m)
                      })
                   join q1 in Stock on new { q.c_gdg, q.c_rnno, q.c_iteno, q.c_batch } equals new { q1.c_gdg, q1.c_rnno, q1.c_iteno, q1.c_batch } into Left
                   from gLeft in Left.DefaultIfEmpty()
                   select new LG_STOCK()
                   {
                     c_gdg = q.c_gdg,
                     c_rnno = q.c_rnno,
                     c_iteno = q.c_iteno,
                     c_batch = q.c_batch,
                     n_gqty = q.n_gqty - (q == null ? 0m : q.n_gqty),
                     n_bqty = q.n_bqty - (q == null ? 0m : q.n_bqty)
                   }).ToList();

          // Update RS		
          Stock = (from q in
                     (from q2 in db.LG_RSHes
                      join q3 in db.LG_RSD2s on new { q2.c_gdg, q2.c_rsno } equals new { q3.c_gdg, q3.c_rsno }
                      where (q2.d_rsdate.Value.Year == Tahun) && (q2.d_rsdate.Value.Month == Bulan)
                      group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                      select new LG_STOCK()
                      {
                        c_gdg = sumg.Key.c_gdg,
                        c_rnno = sumg.Key.c_rnno,
                        c_iteno = sumg.Key.c_iteno,
                        c_batch = sumg.Key.c_batch,
                        n_gqty = sumg.Sum(x => x.q3.n_gqty.HasValue ? x.q3.n_gqty.Value : 0m),
                        n_bqty = sumg.Sum(x => x.q3.n_bqty.HasValue ? x.q3.n_bqty.Value : 0m)
                      })
                   join q1 in Stock on new { q.c_gdg, q.c_rnno, q.c_iteno, q.c_batch } equals new { q1.c_gdg, q1.c_rnno, q1.c_iteno, q1.c_batch } into Left
                   from gLeft in Left.DefaultIfEmpty()
                   select new LG_STOCK()
                   {
                     c_gdg = q.c_gdg,
                     c_rnno = q.c_rnno,
                     c_iteno = q.c_iteno,
                     c_batch = q.c_batch,
                     n_gqty = q.n_gqty - (q == null ? 0m : q.n_gqty),
                     n_bqty = q.n_bqty - (q == null ? 0m : q.n_bqty)
                   }).ToList();


          // Update SJ		
          Stock = (from q in
                     (from q2 in db.LG_SJHs
                      join q3 in db.LG_SJD2s on new { q2.c_gdg, q2.c_sjno } equals new { q3.c_gdg, q3.c_sjno }
                      where (q2.d_sjdate.Value.Year == Tahun) && (q2.d_sjdate.Value.Month == Bulan)
                      group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                      select new LG_STOCK()
                      {
                        c_gdg = sumg.Key.c_gdg,
                        c_rnno = sumg.Key.c_rnno,
                        c_iteno = sumg.Key.c_iteno,
                        c_batch = sumg.Key.c_batch,
                        n_gqty = sumg.Sum(x => x.q3.n_gqty.HasValue ? x.q3.n_gqty.Value : 0m),
                        n_bqty = sumg.Sum(x => x.q3.n_bqty.HasValue ? x.q3.n_bqty.Value : 0m)
                      })
                   join q1 in Stock on new { q.c_gdg, q.c_rnno, q.c_iteno, q.c_batch } equals new { q1.c_gdg, q1.c_rnno, q1.c_iteno, q1.c_batch } into Left
                   from gLeft in Left.DefaultIfEmpty()
                   select new LG_STOCK()
                   {
                     c_gdg = q.c_gdg,
                     c_rnno = q.c_rnno,
                     c_iteno = q.c_iteno,
                     c_batch = q.c_batch,
                     n_gqty = q.n_gqty - (q == null ? 0m : q.n_gqty),
                     n_bqty = q.n_bqty - (q == null ? 0m : q.n_bqty)
                   }).ToList();


          // Update PL		
          Stock = (from q in
                     (from q2 in db.LG_PLHs
                      join q3 in db.LG_PLD2s on q2.c_plno equals q3.c_plno
                      where (q2.d_pldate.Value.Year == Tahun) && (q2.d_pldate.Value.Month == Bulan)
                      group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                      select new LG_STOCK()
                      {
                        c_gdg = sumg.Key.c_gdg.HasValue ? sumg.Key.c_gdg.Value : '1',
                        c_rnno = sumg.Key.c_rnno,
                        c_iteno = sumg.Key.c_iteno,
                        c_batch = sumg.Key.c_batch,
                        n_gqty = sumg.Sum(x => x.q3.n_qty.HasValue ? x.q3.n_qty.Value : 0m),
                        n_bqty = 0m
                      })
                   join q1 in Stock on new { q.c_gdg, q.c_rnno, q.c_iteno, q.c_batch } equals new { q1.c_gdg, q1.c_rnno, q1.c_iteno, q1.c_batch } into Left
                   from gLeft in Left.DefaultIfEmpty()
                   select new LG_STOCK()
                   {
                     c_gdg = q.c_gdg,
                     c_rnno = q.c_rnno,
                     c_iteno = q.c_iteno,
                     c_batch = q.c_batch,
                     n_gqty = q.n_gqty - (q == null ? 0m : q.n_gqty),
                     n_bqty = q.n_bqty - (q == null ? 0m : q.n_bqty)
                   }).ToList();


          // Update STT		
          Stock = (from q in
                     (from q2 in db.LG_STHs
                      join q3 in db.LG_STD2s on new { q2.c_gdg, q2.c_stno } equals new { q3.c_gdg, q3.c_stno }
                      where (q2.d_stdate.Value.Year == Tahun) && (q2.d_stdate.Value.Month == Bulan)
                      group new { q2, q3 } by new { q2.c_gdg, q3.c_no, q3.c_iteno, q3.c_batch } into sumg
                      select new LG_STOCK()
                      {
                        c_gdg = sumg.Key.c_gdg,
                        c_rnno = sumg.Key.c_no,
                        c_iteno = sumg.Key.c_iteno,
                        c_batch = sumg.Key.c_batch,
                        n_gqty = sumg.Sum(x => x.q3.n_qty.HasValue ? x.q3.n_qty.Value : 0m),
                        n_bqty = 0m
                      })
                   join q1 in Stock on new { q.c_gdg, q.c_rnno, q.c_iteno, q.c_batch } equals new { q1.c_gdg, q1.c_rnno, q1.c_iteno, q1.c_batch } into Left
                   from gLeft in Left.DefaultIfEmpty()
                   select new LG_STOCK()
                   {
                     c_gdg = q.c_gdg,
                     c_rnno = q.c_rnno,
                     c_iteno = q.c_iteno,
                     c_batch = q.c_batch,
                     n_gqty = q.n_gqty - (q == null ? 0m : q.n_gqty),
                     n_bqty = q.n_bqty - (q == null ? 0m : q.n_bqty)
                   }).ToList();


          // Update Combo
          Stock = (from q in
                     (from q2 in db.LG_ComboHs
                      join q3 in db.LG_ComboD2s on new { q2.c_gdg, q2.c_combono } equals new { q3.c_gdg, q3.c_combono }
                      where (q2.d_combodate.Value.Year == Tahun) && (q2.d_combodate.Value.Month == Bulan)
                      group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                      select new LG_STOCK()
                      {
                        c_gdg = sumg.Key.c_gdg,
                        c_rnno = sumg.Key.c_rnno,
                        c_iteno = sumg.Key.c_iteno,
                        c_batch = sumg.Key.c_batch,
                        n_gqty = sumg.Sum(x => x.q3.n_qty.HasValue ? x.q3.n_qty.Value : 0m),
                        n_bqty = 0
                      })
                   join q1 in Stock on new { q.c_gdg, q.c_rnno, q.c_iteno, q.c_batch } equals new { q1.c_gdg, q1.c_rnno, q1.c_iteno, q1.c_batch } into Left
                   from gLeft in Left.DefaultIfEmpty()
                   select new LG_STOCK()
                   {
                     c_gdg = q.c_gdg,
                     c_rnno = q.c_rnno,
                     c_iteno = q.c_iteno,
                     c_batch = q.c_batch,
                     n_gqty = q.n_gqty - (q == null ? 0m : q.n_gqty),
                     n_bqty = q.n_bqty - (q == null ? 0m : q.n_bqty)
                   }).ToList();

          // Update Adjustment
          Stock = (from q in
                     (from q2 in db.LG_AdjustHs
                      join q3 in db.LG_AdjustD2s on new { q2.c_gdg, q2.c_adjno } equals new { q3.c_gdg, q3.c_adjno }
                      where (q2.d_adjdate.Value.Year == Tahun) && (q2.d_adjdate.Value.Month == Bulan)
                      group new { q2, q3 } by new { q2.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into sumg
                      select new LG_STOCK()
                      {
                        c_gdg = sumg.Key.c_gdg,
                        c_rnno = sumg.Key.c_rnno,
                        c_iteno = sumg.Key.c_iteno,
                        c_batch = sumg.Key.c_batch,
                        n_gqty = sumg.Sum(x => x.q3.n_gqty.HasValue ? x.q3.n_gqty.Value : 0m),
                        n_bqty = sumg.Sum(x => x.q3.n_bqty.HasValue ? x.q3.n_bqty.Value : 0m)
                      })
                   join q1 in Stock on new { q.c_gdg, q.c_rnno, q.c_iteno, q.c_batch } equals new { q1.c_gdg, q1.c_rnno, q1.c_iteno, q1.c_batch } into Left
                   from gLeft in Left.DefaultIfEmpty()
                   select new LG_STOCK()
                   {
                     c_gdg = q.c_gdg,
                     c_rnno = q.c_rnno,
                     c_iteno = q.c_iteno,
                     c_batch = q.c_batch,
                     n_gqty = q.n_gqty - (q == null ? 0m : q.n_gqty),
                     n_bqty = q.n_bqty - (q == null ? 0m : q.n_bqty)
                   }).ToList();	

          #endregion

          #endregion
        }

        if (hasAnyChanges)
        {
          db.SubmitChanges();

          db.Transaction.Commit();
          //db.Transaction.Rollback();

          rpe = ResponseParser.ResponseParserEnum.IsSuccess;
        }
        else
        {
          db.Transaction.Rollback();

          rpe = ResponseParser.ResponseParserEnum.IsFailed;
        }
      }
      catch (Exception ex)
      {
        if (db.Transaction != null)
        {
          db.Transaction.Rollback();
        }

        result = string.Format("ScmsSoaLibrary.Bussiness.Closing:ColsingLog - {0}", ex.Message);

        Logger.WriteLine(result);
      }

    endLogic:
      result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

      if (dic != null)
      {
        dic.Clear();
      }

      db.Dispose();

      return result;
    }
  }
}
