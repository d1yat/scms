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
using ScmsSoaLibrary.Core.Crypto;
using System.Data.SqlClient;
using System.Data;


namespace ScmsSoaLibrary.Core.Reports.Query
{
  class History
  {
    internal class TemporaryProcessOR
    {
      public string c_fjno { get; set; }
      public string c_cusno { get; set; }
      public DateTime d_fjdate { get; set; }
      public string c_iteno { get; set; }
      public decimal n_qty { get; set; }
      public decimal n_salpri { get; set; }
      public decimal n_disc { get; set; }
      public string c_sektor { get; set; }
      public string v_cunam { get; set; }
      public string v_ket { get; set; }
      public string c_kddivams { get; set; }
      public string v_nmdivams { get; set; }
      public string v_itnam { get; set; }
      public string c_nosup { get; set; }
    }

    internal class DataA
    {
      public string c_fjno { get; set; }
      public DateTime d_fjdate { get; set; }
      public string v_cunam { get; set; }
      public string c_cusno { get; set; }
      public decimal n_sisa { get; set; }
      public decimal n_awal { get; set; }
      public decimal n_beli { get; set; }
      public decimal n_retur { get; set; }
      public string c_fjnoDN { get; set; }
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
      public string c_fjnoAJ { get; set; }
    }

    private static IQueryable Filter(IQueryable qry, IDictionary<string, Functionals.ParameterParser> parameters, params string[] nameUsed)
    {
      if ((parameters != null) && (parameters.Count > 0))
      {
        string paternLike = null;

        Functionals.ParameterParser fp = default(Functionals.ParameterParser);

        for (int nLoop = 0, nLen = 0; nLoop < nLen; nLoop++)
        {
          if (parameters.ContainsKey(nameUsed[nLoop]))
          {
            fp = parameters[nameUsed[nLoop]];
            if (fp.IsCondition)
            {
              if (fp.IsBetween)
              {

              }
              else if (fp.IsIn)
              {

              }
              else
              {
                //qry = qry.Where(fp.Query, fp.Value).AsQueryable();
              }
            }
          }
        }
      }

      return qry;
    }

    public static IDictionary<string, object> QueryReport(string connectionString, string Model, IDictionary<string, Functionals.ParameterParser> parameters)
    {
      IDictionary<string, object> dic = new Dictionary<string, object>();

      ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);

      try
      {
        switch (Model)
        {
          #region REPORT_HISTORY_QUERY_SALES

          case Constant.REPORT_HISTORY_QUERY_SALES_NONRETUR_SUM:
            {
              DateTime date1 = (parameters.ContainsKey("date1") ? (DateTime)((Functionals.ParameterParser)parameters["date1"]).Value : DateTime.MinValue);
              DateTime date2 = (parameters.ContainsKey("date2") ? (DateTime)((Functionals.ParameterParser)parameters["date2"]).Value : DateTime.MinValue);
              string cusno = (parameters.ContainsKey("cusno") ? (string)((Functionals.ParameterParser)parameters["cusno"]).Value : string.Empty);
              string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);

              var ListFJ = (from q in db.LG_FJHs
                            join q1 in db.LG_FJD1s on q.c_fjno equals q1.c_fjno
                            join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
                            where ((q.d_fjdate >= date1) &&
                            (q.d_fjdate.Value <= date2))
                            select new
                            {
                              q.c_fjno,
                              q.c_cusno,
                              q.d_fjdate,
                              q1.c_iteno,
                              q1.n_qty,
                              q1.n_salpri,
                              q1.n_disc,
                              q2.v_itnam,
                              q2.c_nosup
                            }).AsQueryable();

              var ListTran = (from q in db.MsTransDs
                              join q1 in db.LG_Cusmas on q.c_type equals q1.c_sektor
                              where (q.c_notrans == "59") &&
                              (q.c_portal == '3')
                              select new
                              {
                                q1.c_cusno,
                                q1.v_cunam,
                                q1.c_sektor,
                                q.c_type,
                                q.v_ket
                              }).AsQueryable();

              var ListTranAMS = (from q in db.FA_DivAMs
                                 join q1 in db.FA_MsDivAMs on q.c_kddivams equals q1.c_kddivams
                                 select new
                                 {
                                   q.c_iteno,
                                   q1.c_kddivams,
                                   q1.v_nmdivams
                                 }).AsQueryable();

              var List = (from q in ListFJ
                          join q2 in ListTran on q.c_cusno equals q2.c_cusno
                          join q3 in ListTranAMS on q.c_iteno equals q3.c_iteno
                          select new
                          {
                            q.c_fjno,
                            q.c_cusno,
                            q.d_fjdate,
                            q.c_iteno,
                            q.n_qty,
                            q.n_salpri,
                            q.n_disc,
                            q2.c_sektor,
                            q2.v_cunam,
                            q2.v_ket,
                            q3.c_kddivams,
                            q3.v_nmdivams,
                            q.v_itnam,
                            q.c_nosup
                          }).AsQueryable();

             Filter(List, parameters).AsQueryable();

              //dic.Add(Constant.DEFAULT_NAMING_SUCCESS, qry.AsQueryable());

            }
            break;

          #endregion
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.Query:QueryReport <-> Switch {0} - {1}", Model, ex.Message);

        dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);
      }

      //db.Dispose();

      return dic;
    }

    public static DataSet ds(string connectionString, string Model, IDictionary<string, Functionals.ParameterParser> parameters)
    {
      DataSet DataSet = new DataSet();
      ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);
      SqlConnection connect = new SqlConnection(connectionString);
      SqlCommand command = null;

      IQueryable a = null;
      DataSet ds = new DataSet();
      DataTable table = null;
      SqlDataAdapter adapter = null;
      StringBuilder strBuild = new StringBuilder();
      DataRow row = null;
      char sprt = '\t';

      try
      {
        switch (Model)
        {
          #region REPORT_HISTORY_QUERY_SALDO_DEBIT - SUM

          case Constant.REPORT_HISTORY_QUERY_SALDO_DEBIT:
          case Constant.REPORT_HISTORY_QUERY_SALDO_DEBIT_SUM:
            {
              DateTime date1 = (parameters.ContainsKey("date1") ? (DateTime)((Functionals.ParameterParser)parameters["date1"]).Value : DateTime.MinValue);
              DateTime date2 = (parameters.ContainsKey("date2") ? (DateTime)((Functionals.ParameterParser)parameters["date2"]).Value : DateTime.MinValue);
              //string cusno = (parameters.ContainsKey("cusno") ? (string)((Functionals.ParameterParser)parameters["cusno"]).Value : string.Empty);
              //string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);

              //DateTime date1 = Convert.ToDateTime("2010-10-01");
              //DateTime date2 = Convert.ToDateTime("2010-10-30");
              DateTime date3 = date1.AddDays(1);
              string cusid = "0105";
              int tahun = 2010;
              int bulan = 10;

              var closeFJ = (from q in db.LG_CloseFJs
                             join q1 in db.LG_FJHs on q.c_fjno equals q1.c_fjno
                             join q2 in db.LG_Cusmas on q1.c_cusno equals q2.c_cusno
                             where (q.s_tahun == tahun && q.t_bulan == bulan)
                             && (q2.c_cusno == cusid) && ((q1.l_delete.HasValue ? q1.l_delete : false) == false)
                             select new DataA()
                             {
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
                              where (q.s_tahun == tahun && q.t_bulan == bulan)
                              && q2.c_cusno == cusid && ((q1.l_delete.HasValue ? q1.l_delete : false) == false)
                              select new DataA()
                              {
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
                         where (q1.d_fjdate >= date1 && q1.d_fjdate <= date2)
                         && (q2.c_cusno == cusid) && ((q1.l_delete.HasValue ? q1.l_delete : false) == false)
                         select new DataA()
                         {
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
                          where (q1.d_fjdate >= date1 && q1.d_fjdate <= date2)
                          && (q2.c_cusno == cusid) && ((q1.l_delete.HasValue ? q1.l_delete : false) == false)
                          select new DataA()
                          {
                            c_fjno = q1.c_fjno,
                            d_fjdate = q1.d_fjdate.Value,
                            v_cunam = q2.v_cunam,
                            c_cusno = q2.c_cusno,
                            n_sisa = q1.n_sisa.Value,
                            n_beli = 0,
                            n_retur = 0
                          }).ToList();

              var CNH1 = (from q in db.LG_CNHs
                          join q1 in db.LG_CNDs on q.c_noteno equals q1.c_noteno
                          join q2 in db.LG_FJHs on q1.c_fjno equals q2.c_fjno
                          where (q.d_notedate >= date1 && q.d_notedate <= date2)
                          && (q2.d_fjdate < date3) && (q1.c_type == "02")
                          && (q2.c_cusno == cusid) && ((q.l_delete.HasValue ? q.l_delete : false) == false)
                          && (q2.l_delete == false)
                          group q1 by new { q1.c_vcno, q1.c_type } into g
                          select new DataA()
                          {
                            c_fjnoDN = g.Key.c_vcno,
                            n_bayarDN = (g.Key.c_type == "01" ? g.Sum(x => x.n_value).Value : 0),
                            n_bayar_returDN = (g.Key.c_type == "02" ? g.Sum(x => x.n_value).Value * -1 : 0),
                            c_typeDN = g.Key.c_type
                          }).ToList();

              var CNH2 = (from q in db.LG_CNHs
                          join q1 in db.LG_CNDs on q.c_noteno equals q1.c_noteno
                          join q2 in db.LG_FJHs on q1.c_fjno equals q2.c_fjno
                          where (q.d_notedate >= date1 && q.d_notedate <= date2)
                          && (q2.d_fjdate < date3) && (q1.c_type == "01")
                          && (q2.c_cusno == cusid) && ((q.l_delete.HasValue ? q.l_delete : false) == false)
                          && (q2.l_delete == false)
                          select new DataA()
                          {
                            c_fjnoDN = q1.c_vcno,
                            n_bayarDN = (q1.c_type == "01" ? q1.n_value.Value : 0),
                            n_bayar_returDN = (q1.c_type == "01" ? q1.n_value.Value * -1 : 0),
                            c_typeDN = q1.c_type
                          }).ToList();

              var adjust = (from q in db.LG_AdjFJHs
                            join q1 in db.LG_AdjFJDs on q.c_adjno equals q1.c_adjno
                            where (q.d_adjdate >= date1 && q.d_adjdate <= date2)
                            && (q.c_cusno == cusid)
                            group new { q, q1 } by new { q1.c_fjno, q.c_type } into g
                            select new DataA()
                            {
                              c_fjnoAJ = g.Key.c_fjno,
                              n_adjustAJ = (g.Key.c_type == "01" ? ((-g.Sum(x => x.q1.n_value)).Value == null ? 0 : (-g.Sum(x => x.q1.n_value)).Value) : ((g.Sum(x => x.q1.n_value)).Value == null ? 0 : (g.Sum(x => x.q1.n_value)).Value)),
                              c_typeAJ = g.Key.c_type
                            }).ToList();


              List<DataA> FJ = null;
              List<DataA> CN = null;
              List<DataA> Adj = null;

              FJ = FJH.Union(FJRH).Union(closeFJ).Union(closeFJR).ToList();
              CN = CNH1.Union(CNH2).ToList();
              Adj = adjust.ToList();

              var Saldeb = (from q in FJ
                            join q1 in CN on q.c_fjno equals q1.c_fjnoDN into q1Left
                            from q1S in q1Left.DefaultIfEmpty()
                            join q2 in Adj on q.c_fjno equals q2.c_fjnoAJ into q2Left
                            from q2S in q2Left.DefaultIfEmpty()
                            select new DataA()
                            {

                              c_fjno = q.c_fjno,
                              d_fjdate = q.d_fjdate,
                              c_cusno = q.c_cusno,
                              v_cunam = q.v_cunam,
                              n_awal = q.n_sisa,
                              n_beli = q.n_beli,
                              n_retur = q.n_retur,
                              n_bayarDN = q1S == null ? 0m : q1S.n_bayarDN,
                              n_bayar_returDN = q1S == null ? 0m : q1S.n_bayar_returDN,
                              n_adjustAJ = q2S == null ? 0m : q2S.n_adjustAJ,
                              n_sisa = (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                                       (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayarDN) +
                                   (q1S == null ? 0m : q1S.n_bayar_returDN) + (q2S == null ? 0m : q2S.n_adjustAJ),
                              n_1_30 = (SqlMethods.DateDiffDay(q.d_fjdate, date2) <= 30 ?
                                   (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                                       (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayarDN) +
                                   (q1S == null ? 0m : q1S.n_bayar_returDN) + (q2S == null ? 0m : q2S.n_adjustAJ) : 0),
                              n_31_37 = ((SqlMethods.DateDiffDay(q.d_fjdate, date2) >= 31 && (SqlMethods.DateDiffDay(q.d_fjdate, date2) <= 37)) ?
                                 (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                                     (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayarDN) +
                                 (q1S == null ? 0m : q1S.n_bayar_returDN) + (q2S == null ? 0m : q2S.n_adjustAJ) : 0),
                              n_38_45 = ((SqlMethods.DateDiffDay(q.d_fjdate, date2) >= 38 && (SqlMethods.DateDiffDay(q.d_fjdate, date2) <= 45)) ?
                                 (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                                     (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayarDN) +
                                 (q1S == null ? 0m : q1S.n_bayar_returDN) + (q2S == null ? 0m : q2S.n_adjustAJ) : 0),
                              n_46_60 = ((SqlMethods.DateDiffDay(q.d_fjdate, date2) >= 46 && (SqlMethods.DateDiffDay(q.d_fjdate, date2) <= 60)) ?
                                 (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                                     (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayarDN) +
                                 (q1S == null ? 0m : q1S.n_bayar_returDN) + (q2S == null ? 0m : q2S.n_adjustAJ) : 0),
                              n_61_90 = ((SqlMethods.DateDiffDay(q.d_fjdate, date2) >= 61 && (SqlMethods.DateDiffDay(q.d_fjdate, date2) <= 90)) ?
                                  (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                                      (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayarDN) +
                                  (q1S == null ? 0m : q1S.n_bayar_returDN) + (q2S == null ? 0m : q2S.n_adjustAJ) : 0),
                              n_91_120 = ((SqlMethods.DateDiffDay(q.d_fjdate, date2) >= 91 && (SqlMethods.DateDiffDay(q.d_fjdate, date2) <= 120)) ?
                                 (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                                     (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayarDN) +
                                 (q1S == null ? 0m : q1S.n_bayar_returDN) + (q2S == null ? 0m : q2S.n_adjustAJ) : 0),
                              n_120 = ((SqlMethods.DateDiffDay(q.d_fjdate, date2) > 120) ?
                                 (q == null ? 0m : q.n_sisa) - (q == null ? 0m : q.n_beli) -
                                     (q == null ? 0m : q.n_retur) - (q1S == null ? 0m : q1S.n_bayarDN) +
                                 (q1S == null ? 0m : q1S.n_bayar_returDN) + (q2S == null ? 0m : q2S.n_adjustAJ) : 0)
                            }).ToList();

            
            }
            break;

          #endregion
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          "ScmsSoaLibrary.Core.Reports.Query:QueryReport <-> Switch {0} - {1}", Model, ex.Message);

        
      }

      return DataSet;
    }
  }
}
