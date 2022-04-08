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
using ScmsSoaLibraryInterface.Core.Crypto;
using System.Linq.Expressions;
using System.Reflection;
using ScmsSoaLibrary.Parser;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaLibrary.Modules
{
  class CommonQueryTesting
  {
    public void Testing()
    {
      //Config config = new Config();

      //string connectionString = config.ConnectionString;

      //ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);

      //var qry = (from q in db.LG_DOHs
      //           select q).AsQueryable();

      ////qry = qry.Between(x => x.c_dono.CompareTo("DO1101A270") > 0, y => y.c_dono.CompareTo("DO1101A300") < 0).AsQueryable();
      ////2010-12-01
      //qry = qry.Between(x => x.d_dodate.Value, new DateTime(2010, 12, 10), new DateTime(2010, 12, 13)).AsQueryable();

      //string tmp = qry.Provider.ToString();
    }

    #region Get Faktur Pajak

    public ScmsSoaLibrary.Bussiness.Commons.FakturPajakInformation TestGetFakturPajakDisCore()
    {
      ScmsSoaLibrary.Bussiness.Commons.FakturPajakInformation fpi = null;

      Dictionary<string, string> dicParam = new Dictionary<string, string>();
      dicParam.Add("C_SOURCE_TYPE", "SCMS");

      Dictionary<string, string> dicHeader = new Dictionary<string, string>();
      dicHeader.Add("X-Requested-With", "XMLHttpRequest");

      IDictionary<string, object> dic = new Dictionary<string, object>();

      ScmsSoaLibrary.Parser.ResponseParser res = new ScmsSoaLibrary.Parser.ResponseParser();

      ScmsSoaLibrary.Core.Response.DiscoreResponse ds = new ScmsSoaLibrary.Core.Response.DiscoreResponse();


      ScmsSoaLibrary.Parser.ParserDisCore pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

      pdc.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
      pdc.Referer = "http://10.100.11.12/dist_core/?m=com.ams.welcome";

      UriBuilder uriB = new UriBuilder("http://10.100.11.16/dist_core/?m=com.ams.json.ds&action=form&f=AutoLookup&_q=trx_generate_no_tax");

      string result = null;

      Encoding utf8 = Encoding.UTF8;

      DateTime date = DateTime.MinValue;
      int nRows = 0;
      string tmp = null;

      if (pdc.PostGetData(uriB.Uri, dicParam, dicHeader))
      {
        result = utf8.GetString(pdc.Result);

        dic = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCore(result);

        nRows = dic.GetValueParser<string, object, int>(Constant.DEFAULT_NAMING_TOTAL_ROWS);
        if (nRows > 0)
        {
          dicHeader.Clear();

          dicHeader = dic.GetValueParser<string, object, Dictionary<string, string>>(Constant.DEFAULT_NAMING_RECORDS, null);

          if (dicHeader != null)
          {
            fpi = new ScmsSoaLibrary.Bussiness.Commons.FakturPajakInformation();

            fpi.NoFakturPajak = dicHeader.GetValueParser<string, string, string>("C_INVOICETAXNO", string.Empty).Trim();
            tmp = dicHeader.GetValueParser<string, string, string>("TANGGAL_TAX");

            if (!string.IsNullOrEmpty(tmp))
            {
              if (!Functionals.DateParser(tmp, "yyyy-MM-dd HH:mm:ss", out date))
              {
                date = Functionals.StandardSqlDateTime;
              }
            }
            else
            {
              date = Functionals.StandardSqlDateTime;
            }

            fpi.TanggalFakturPajak = date;
          }
        }
      }
      else
      {
        result = pdc.ErrorMessage;
      }

      Logger.WriteLine(result);

      dic.Clear();

      dicHeader.Clear();
      dicParam.Clear();

      return fpi;
    }

    #endregion

    #region Send DO To DisCore

    private string TestGetDataDO(ORMDataContext db, string doNo)
    {
      string result = null;

      ScmsSoaLibrary.Parser.Class.DeliveryOrderPosting dop = null;
      List<ScmsSoaLibrary.Parser.Class.DeliveryOrderDetailPostings> listDetails = null;

      ScmsSoaLibrary.Parser.Class.DeliveryOrderDetailPostings dodp = null;

      var qqq = (from q in db.LG_DOHs
                 join q1 in db.LG_MsGudangs on q.c_gdg equals q1.c_gdg
                 join q2 in db.MsTransDs on new { c_portal = '3', c_notrans = "02", c_type = q.c_via } equals new { q2.c_portal, q2.c_notrans, q2.c_type }
                 join q3 in
                   (from sq1 in db.LG_FJHs
                    join sq2 in db.LG_FJD3s on sq1.c_fjno equals sq2.c_fjno
                    where ((sq1.l_delete.HasValue ? sq1.l_delete.Value : false) == false)
                    select new
                    {
                      c_fjno = (sq1.c_fjno == null ? string.Empty : sq1.c_fjno.Trim()),
                      d_fjdate = (sq1.d_fjdate.HasValue ? sq1.d_fjdate.Value : Functionals.StandardSqlDateTime),
                      sq2.c_dono
                    }) on q.c_dono equals q3.c_dono into q_3
                 from qFJs in q_3.DefaultIfEmpty()
                 where (q.c_dono == doNo)
                  && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                 select new ScmsSoaLibrary.Parser.Class.DeliveryOrderPosting()
                 {
                   PIN = (q.c_pin == null ? string.Empty : q.c_pin.Trim()),
                   Cabang = (q.c_cusno == null ? string.Empty : q.c_cusno.Trim()),
                   Gudang = (q.c_gdg.HasValue ? q.c_gdg.Value : char.MinValue),
                   GudangDesc = q1.v_gdgdesc,
                   TypeCode = (q.c_type == null ? string.Empty : q.c_type.Trim()),
                   Via = q2.v_ket,
                   DO = (q.c_dono == null ? string.Empty : q.c_dono.Trim()),
                   ReferenceID = (q.c_plno == null ? string.Empty : q.c_plno.Trim()),
                   TanggalDO = (q.d_dodate.HasValue ? q.d_dodate.Value : Functionals.StandardSqlDateTime),
                   FakturID = (qFJs != null ? qFJs.c_fjno : string.Empty),
                   TanggalFJ = (qFJs != null ? qFJs.d_fjdate : Functionals.StandardSqlDateTime),
                 }).Distinct().AsQueryable();

      var sss = qqq.Provider.ToString();

      dop = (from q in db.LG_DOHs
             join q1 in db.LG_MsGudangs on q.c_gdg equals q1.c_gdg
             join q2 in db.MsTransDs on new { c_portal = '3', c_notrans = "02", c_type = q.c_via } equals new { q2.c_portal, q2.c_notrans, q2.c_type }
             join q3 in (from sq1 in db.LG_FJHs
                         join sq2 in db.LG_FJD3s on sq1.c_fjno equals sq2.c_fjno
                         where ((sq1.l_delete.HasValue ? sq1.l_delete.Value : false) == false)
                         select new {
                           c_fjno = (sq1.c_fjno == null ? string.Empty : sq1.c_fjno.Trim()),
                           d_fjdate = (sq1.d_fjdate.HasValue ? sq1.d_fjdate.Value : Functionals.StandardSqlDateTime),
                           sq2.c_dono
                         }) on q.c_dono equals q3.c_dono into q_3
             from qFJs in q_3.DefaultIfEmpty()
             where (q.c_dono == doNo)
              && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
             select new ScmsSoaLibrary.Parser.Class.DeliveryOrderPosting()
             {
               PIN = (q.c_pin == null ? string.Empty : q.c_pin.Trim()),
               Cabang = (q.c_cusno == null ? string.Empty : q.c_cusno.Trim()),
               Gudang = (q.c_gdg.HasValue ? q.c_gdg.Value : char.MinValue),
               GudangDesc = q1.v_gdgdesc,
               TypeCode = (q.c_type == null ? string.Empty : q.c_type.Trim()),
               Via = q2.v_ket,
               DO = (q.c_dono == null ? string.Empty : q.c_dono.Trim()),
               ReferenceID = (q.c_plno == null ? string.Empty : q.c_plno.Trim()),
               TanggalDO = (q.d_dodate.HasValue ? q.d_dodate.Value : Functionals.StandardSqlDateTime),
               FakturID = (qFJs != null ? qFJs.c_fjno : string.Empty),
               TanggalFJ = (qFJs != null ? qFJs.d_fjdate : Functionals.StandardSqlDateTime),
             }).Distinct().Take(1).SingleOrDefault();

      if (dop != null)
      {
        if (!string.IsNullOrEmpty(dop.PIN))
        {
          dop.PIN = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(dop.PIN));
        }

        dop.TanggalDO_Str = dop.TanggalDO.ToString("yyyy-MM-dd");
        
        dop.TanggalFJ_Str = dop.TanggalFJ.ToString("yyyy-MM-dd");

        #region Complex

        var qqqq = (from q in db.LG_DOD1s
                    join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                    join q2 in db.LG_PLD1s on new { c_plno = dop.ReferenceID, q.c_iteno } equals new { q2.c_plno, q2.c_iteno } into q_2
                    from qPLDs in q_2.DefaultIfEmpty()
                    join q3 in db.LG_STD1s on new { c_gdg = dop.Gudang, c_stno = dop.ReferenceID, q.c_iteno } equals new { q3.c_gdg, q3.c_stno, q3.c_iteno } into q_3
                    from qSTDs in q_3.DefaultIfEmpty()
                    join q4 in db.LG_MsBatches on new { q.c_iteno, c_batch = (dop.TypeCode == "02" ? qSTDs.c_batch : qPLDs.c_batch) } equals new { q4.c_iteno, q4.c_batch } into q_4
                    from qBat in q_4.DefaultIfEmpty()
                    join q5 in
                      (from sq1 in db.LG_FJD1s
                       join sq2 in db.LG_FJD2s on sq1.c_fjno equals sq2.c_fjno
                       where (sq1.c_fjno == dop.FakturID)
                       select new
                       {
                         sq1.c_iteno,
                         sq1.n_salpri,
                         sq2.n_discon,
                         sq2.n_discoff
                       }) on q.c_iteno equals q5.c_iteno into q_5
                    from qFJs in q_5.DefaultIfEmpty()
                    where (q.c_dono == dop.DO)
                    select new ScmsSoaLibrary.Parser.Class.DeliveryOrderDetailPostings()
                    {
                      Item = q.c_iteno,
                      NamaItem = q1.v_itnam,
                      Jumlah = (q.n_qty.HasValue ? q.n_qty.Value : 0),
                      Batch = (dop.TypeCode == "02" ?
                       (qSTDs != null ?
                         (qSTDs.c_batch == null ? string.Empty : qSTDs.c_batch.Trim()) : string.Empty) :
                       (qPLDs != null ?
                         (qPLDs.c_batch == null ? string.Empty : qPLDs.c_batch.Trim()) : string.Empty)),
                      Expired = (qBat != null ?
                                     (qBat.d_expired.HasValue ? qBat.d_expired.Value : Functionals.StandardSqlDateTime) : Functionals.StandardSqlDateTime),
                      Harga = (qFJs != null ?
                                     (qFJs.n_salpri.HasValue ? qFJs.n_salpri.Value : 0) : 0),
                      Diskon = (qFJs != null ?
                                     (qFJs.n_discon.HasValue ? qFJs.n_discon.Value : 0) : 0),
                      SPs = (from sq in db.LG_SPHs
                             join sq1 in db.LG_SPD1s on sq.c_spno equals sq1.c_spno
                             where (sq.c_spno ==
                               (qPLDs != null ?
                                     (qPLDs.c_spno == null ? string.Empty : qPLDs.c_spno.Trim()) : string.Empty)) &&
                                   (sq1.c_iteno ==
                               (qPLDs != null ?
                                     (qPLDs.c_iteno == null ? string.Empty : qPLDs.c_iteno.Trim()) : string.Empty))
                             //group sq1 by new { sq1.c_spno, sq1.c_iteno } into g
                             select new ScmsSoaLibrary.Parser.Class.DeliveryOrderSPDetailPostings()
                             {
                               SP = sq.c_sp,
                               Jumlah = (qPLDs != null ?
                                              (qPLDs.n_qty.HasValue ? qPLDs.n_qty.Value : 0) : 0)
                             }).Distinct().ToArray()
                    }).Distinct().AsQueryable();

        sss = qqqq.Provider.ToString();

        listDetails = (from q in db.LG_DOD1s
                       join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                       join q2 in db.LG_PLD1s on new { c_plno = dop.ReferenceID, q.c_iteno } equals new { q2.c_plno, q2.c_iteno } into q_2
                       from qPLDs in q_2.DefaultIfEmpty()
                       join q3 in db.LG_STD1s on new { c_gdg = dop.Gudang, c_stno = dop.ReferenceID, q.c_iteno } equals new { q3.c_gdg, q3.c_stno, q3.c_iteno } into q_3
                       from qSTDs in q_3.DefaultIfEmpty()
                       join q4 in db.LG_MsBatches on new { q.c_iteno, c_batch = (dop.TypeCode == "02" ? qSTDs.c_batch : qPLDs.c_batch)} equals new { q4.c_iteno, q4.c_batch } into q_4
                       from qBat in q_4.DefaultIfEmpty()
                       join q5 in
                         (from sq1 in db.LG_FJD1s
                          join sq2 in db.LG_FJD2s on sq1.c_fjno equals sq2.c_fjno
                          where (sq1.c_fjno == dop.FakturID)
                          select new
                          {
                            sq1.c_iteno,
                            sq1.n_salpri,
                            sq2.n_discon,
                            sq2.n_discoff
                          }) on q.c_iteno equals q5.c_iteno into q_5
                       from qFJs in q_5.DefaultIfEmpty()
                       where (q.c_dono == dop.DO)
                       select new ScmsSoaLibrary.Parser.Class.DeliveryOrderDetailPostings()
                       {
                         Item = q.c_iteno,
                         NamaItem = q1.v_itnam,
                         Jumlah = (q.n_qty.HasValue ? q.n_qty.Value : 0),
                         Batch = (dop.TypeCode == "02" ?
                          (qSTDs != null ?
                            (qSTDs.c_batch == null ? string.Empty : qSTDs.c_batch.Trim()) : string.Empty) :
                          (qPLDs != null ?
                            (qPLDs.c_batch == null ? string.Empty : qPLDs.c_batch.Trim()) : string.Empty)),
                         Expired = (qBat != null ?
                                        (qBat.d_expired.HasValue ? qBat.d_expired.Value : Functionals.StandardSqlDateTime) : Functionals.StandardSqlDateTime),
                         Harga = (qFJs != null ?
                                        (qFJs.n_salpri.HasValue ? qFJs.n_salpri.Value : 0) : 0),
                         Diskon = (qFJs != null ?
                                        (qFJs.n_discon.HasValue ? qFJs.n_discon.Value : 0) : 0),
                         SPs = (from sq in db.LG_SPHs
                                join sq1 in db.LG_SPD1s on sq.c_spno equals sq1.c_spno
                                where (sq.c_spno ==
                                  (qPLDs != null ?
                                        (qPLDs.c_spno == null ? string.Empty : qPLDs.c_spno.Trim()) : string.Empty)) &&
                                      (sq1.c_iteno ==
                                  (qPLDs != null ?
                                        (qPLDs.c_iteno == null ? string.Empty : qPLDs.c_iteno.Trim()) : string.Empty))
                                //group sq1 by new { sq1.c_spno, sq1.c_iteno } into g
                                select new ScmsSoaLibrary.Parser.Class.DeliveryOrderSPDetailPostings()
                                {
                                  SP = sq.c_sp,
                                  Jumlah = (qPLDs != null ?
                                                 (qPLDs.n_qty.HasValue ? qPLDs.n_qty.Value : 0) : 0)
                                }).Distinct().ToArray()
                       }).Distinct().ToList();

        #endregion

        if (listDetails.Count > 0)
        {
          for (int nLoop = 0; nLoop < listDetails.Count; nLoop++)
          {
            dodp = listDetails[nLoop];
            if (dodp != null)
            {
              dodp.Expired_Str = dodp.Expired.ToString("yyyy-MM-dd");
            }
          }

          dop.Fields = listDetails.ToArray();

          listDetails.Clear();
        }

        result = ScmsSoaLibrary.Parser.Class.DeliveryOrderPosting.Serialize(dop);
      }

      return result;
    }

    public ScmsSoaLibrary.Bussiness.Commons.FakturPajakInformation TestPostDataDO(ORMDataContext db, string doNo)
    {
      db = new ORMDataContext(Functionals.ActiveConnectionString);

      string dataResult = TestGetDataDO(db, doNo);

      Config cfg = new Config();

      ScmsSoaLibrary.Bussiness.Commons.FakturPajakInformation fpi = null;

      Dictionary<string, string> dicParam = new Dictionary<string, string>();
      dicParam.Add("param", dataResult);

      Dictionary<string, string> dicHeader = new Dictionary<string, string>();
      dicHeader.Add("X-Requested-With", "XMLHttpRequest");

      IDictionary<string, object> dic = new Dictionary<string, object>();

      ScmsSoaLibrary.Parser.ResponseParser res = new ScmsSoaLibrary.Parser.ResponseParser();

      ScmsSoaLibrary.Core.Response.DiscoreResponse ds = new ScmsSoaLibrary.Core.Response.DiscoreResponse();
      
      ScmsSoaLibrary.Parser.ParserDisCore pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

      pdc.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
      pdc.Referer = Functionals.DistCoreUrlBuilderString(cfg, "http://10.100.11.12/dist_core/?m=com.ams.welcome");

      Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.11.16/dist_core/?m=com.ams.json.ds&action=form&f=AutoLookup&_q=trx_update_do_rn");

      string result = null;

      Encoding utf8 = Encoding.UTF8;

      DateTime date = DateTime.MinValue;
      //int nRows = 0;
      //string tmp = null;

      if (pdc.PostGetData(uri, dicParam, dicHeader))
      {
        result = utf8.GetString(pdc.Result);
      }
      else
      {
        result = pdc.ErrorMessage;
      }

      Logger.WriteLine(result);

      dic.Clear();

      dicHeader.Clear();
      dicParam.Clear();

      return fpi;
    }

    #endregion
  }

  //public static class BetweenExtensions
  //{
  //  public static IQueryable<T> Between<T, TKey>(this IQueryable<T> source, Expression<Func<T, TKey>> keySelector, TKey low, TKey high) where TKey : IComparable<TKey>
  //  {
  //    Expression<Func<T, bool>> lambda = null;
  //    Expression key = Expression.Invoke(keySelector,
  //                                         keySelector.Parameters.ToArray());
  //    Expression lowerBound = null;
  //    Expression upperBound = null;
  //    Expression and = null;

  //    if (typeof(TKey).Equals(typeof(string)))
  //    {
  //      MethodInfo mi = typeof(string).GetMethod("CompareTo", new[] { typeof(string) });

  //      lowerBound = Expression.GreaterThanOrEqual(
  //        Expression.Call(key, mi, Expression.Constant(low, typeof(string))),
  //        Expression.Constant(0, typeof(int)));

  //      upperBound = Expression.LessThanOrEqual(
  //        Expression.Call(key, mi, Expression.Constant(high, typeof(string))),
  //        Expression.Constant(0, typeof(int)));
  //    }
  //    else
  //    {
  //      lowerBound = Expression.GreaterThanOrEqual(key, 
  //        Expression.Constant(low));
  //      upperBound = Expression.LessThanOrEqual(key, 
  //        Expression.Constant(high));
  //    }

  //    and = Expression.AndAlso(lowerBound, upperBound);

  //    lambda = Expression.Lambda<Func<T, bool>>(and, keySelector.Parameters);

  //    return source.Where(lambda);
  //  }

  //  class Error
  //  {
  //    internal static Exception ArgumentNull(string paramName)
  //    {
  //      return new ArgumentNullException(paramName);
  //    }
  //  }
  //}
}