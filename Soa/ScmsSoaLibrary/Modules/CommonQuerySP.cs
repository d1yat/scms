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
using ScmsSoaLibraryInterface.Core.Crypto; //Indra
using ScmsSoaLibrary.Parser;
//using ScmsSoaLibrary.Core.Crypto;

namespace ScmsSoaLibrary.Modules
{
  class CommonQuerySP
  {
    #region Internal Class

    internal class PROCESS_DO_TO_FJ
    {
      public string DoNo { get; set; }
      public string Via { get; set; }
      public string Item { get; set; }
      public decimal Qty { get; set; }
      public decimal Sisa { get; set; }
      public decimal Disc { get; set; }
      public decimal DiscOff { get; set; }
      public decimal HNA { get; set; }
      public decimal Bonus { get; set; }
      public string DiscountCode { get; set; }
    }

    internal class DOInformation
    {
      public string ID { get; set; }
      public bool IsStt { get; set; }
    }

    internal class RN_DOInformation
    {
      public string Supplier { get; set; }
      public string DONo { get; set; }      
    }

    internal class MSDivPriInformation
    {
      public string Method { get; set; }
      public FA_MsDivPri DivPri { get; set; }
    }

    static string GenerateNumbering<T>(ORMDataContext db, string headerCode, char portalKode, string tipeKode, DateTime tanggalAktif, string fieldCondition)
    {
      string result = string.Empty;

      //db.GetTable<T>().Where(

      int nCount = 0,
        nValue = 0,
        nCountTambah = 0;
      string tmpNum = null,
        hdrValue = null,
        tVal = null;
      char chr = char.MinValue;

      SysNo sysNum = (from q in db.SysNos
                      where q.c_portal == portalKode && q.c_type == tipeKode
                        && q.s_tahun == tanggalAktif.Year
                      select q).SingleOrDefault();

      if (sysNum != null)
      {
        switch (tanggalAktif.Month)
        {
          case 1: tmpNum = (sysNum.c_bln01 ?? string.Empty); break;
          case 2: tmpNum = (sysNum.c_bln02 ?? string.Empty); break;
          case 3: tmpNum = (sysNum.c_bln03 ?? string.Empty); break;
          case 4: tmpNum = (sysNum.c_bln04 ?? string.Empty); break;
          case 5: tmpNum = (sysNum.c_bln05 ?? string.Empty); break;
          case 6: tmpNum = (sysNum.c_bln06 ?? string.Empty); break;
          case 7: tmpNum = (sysNum.c_bln07 ?? string.Empty); break;
          case 8: tmpNum = (sysNum.c_bln08 ?? string.Empty); break;
          case 9: tmpNum = (sysNum.c_bln09 ?? string.Empty); break;
          case 10: tmpNum = (sysNum.c_bln10 ?? string.Empty); break;
          case 11: tmpNum = (sysNum.c_bln11 ?? string.Empty); break;
          case 12: tmpNum = (sysNum.c_bln12 ?? string.Empty); break;
          default: tmpNum = null; break;
        }

        if (!string.IsNullOrEmpty(tmpNum))
        {
          tmpNum = (string.IsNullOrEmpty(tmpNum) ? string.Empty : tmpNum.Trim());

          hdrValue = (string.IsNullOrEmpty(headerCode) ? "__" :
            ((headerCode.Length > 1) ? headerCode.PadLeft(2, '_') : headerCode.Substring(0, 2)));

          do
          {
            if (tmpNum.Length > 1)
            {
              if (!char.IsNumber(tmpNum, 0))
              {
                tVal = tmpNum.Substring(1);

                if (!int.TryParse(tVal, out nValue))
                {
                  result = string.Empty;

                  goto endLogic;
                }

                nValue++;

                if (nValue > 999)
                {
                    if (tmpNum == "Z999")
                    {
                        tmpNum = "0001";
                    }
                    else
                    {
                        chr = tmpNum[0];
                        chr++;

                        if (chr > 0x60)
                        {
                            chr = (char)0x7b;
                        }

                        tmpNum = string.Concat(chr, "001");
                    }
                }
                else
                {
                  tmpNum = string.Concat(tmpNum[0], nValue.ToString().PadLeft(3, '0'));
                }
              }
              else
              {
                if (!int.TryParse(tmpNum, out nValue))
                {
                  result = string.Empty;

                  goto endLogic;
                }

                nValue++;

                if (nValue > 9999)
                {
                  tmpNum = "A001";
                }
                else
                {
                  tmpNum = nValue.ToString().PadLeft(4, '0');
                }
              }
            }
            else
            {
              tmpNum = "0001";
            }

            result = string.Concat(hdrValue, tanggalAktif.ToString("yyMM"), tmpNum);
            var qry = (from q in db.LG_PLHs where q.c_plno == string.Concat(hdrValue, tanggalAktif.ToString("yyMM"),"Z999")
                           select q).SingleOrDefault();
            if (qry != null)
            {
                int tglbln = Convert.ToInt32(tanggalAktif.ToString("yyMM")) + 12;
                result = string.Concat(hdrValue, tglbln.ToString(), tmpNum);
            }

            nCount = db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count();
            

          } while (nCount != 0);

        endLogic:
          ;
        }
      }

      return result;
    }

    #endregion

    #region Structure



    #endregion

    #region Private

    #region Send DO
    
    private string GetDataDO(ORMDataContext db, string doNo, bool isStt)
    {
      string result = null;
      string cek = null;
      ScmsSoaLibrary.Parser.Class.DeliveryOrderPosting dop = null;
      List<ScmsSoaLibrary.Parser.Class.DeliveryOrderDetailPostings> listDetails = null,
        listDetailsFinal = null;

      ScmsSoaLibrary.Parser.Class.DeliveryOrderDetailPostings dodp = null;

      int nLoop = 0;

      try
      {
          #region Header DO
          cek = (from q in db.LG_DOHs
                 where (q.c_dono == doNo)
                 select
                 q.c_plphar
                     ).Single();
          
          if (cek != null)
          {
              dop = (from q in db.LG_DOHs
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
                     //suwandi 21 juni 2017
                     join q4 in db.LG_PLD2s on q.c_plno equals q4.c_plno
                     join q5 in db.LG_SPHs on q4.c_spno equals q5.c_spno
                     //suwandi 21 juni 2017
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
                         user = (q.c_po_outlet == null ? "SCMS" : "AUTO_PHAR"),
                         PoOutlet = q.c_po_outlet,
                         Outlet = q.c_outlet,
                         PLPHAR = q.c_plphar, //suwandi 13 juli 2017
                         SPPs = q5.c_spphar //suwandi 21 juni 2017
                     }).Distinct().Take(1).SingleOrDefault();
          }
          else
          {
              dop = (from q in db.LG_DOHs
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
                         user = (q.c_po_outlet == null ? "SCMS" : "AUTO_PHAR"),
                         PoOutlet = q.c_po_outlet,
                         Outlet = q.c_outlet,
                         PLPHAR = q.c_plphar, //suwandi 13 juli 2017
                     }).Distinct().Take(1).SingleOrDefault();
          }
          if (dop != null)
        {
          if (!string.IsNullOrEmpty(dop.PIN))
          {
            dop.PIN = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(dop.PIN));
          }

          dop.TanggalDO_Str = dop.TanggalDO.ToString("yyyy-MM-dd");

          dop.TanggalFJ_Str = dop.TanggalFJ.ToString("yyyy-MM-dd");
          #endregion
          #region Complex

          if (dop.ReferenceID.StartsWith("PL", StringComparison.OrdinalIgnoreCase))
          {
            #region PL

            //listDetails = (from q in db.LG_DOD1s
            //               join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
            //               join q2 in db.LG_PLD1s on new { c_plno = dop.ReferenceID, q.c_iteno } equals new { q2.c_plno, q2.c_iteno } into q_2
            //               from qPLDs in q_2.Where(t => t.n_qty > 0).DefaultIfEmpty()
            //               join q4 in db.LG_MsBatches on new { q.c_iteno, qPLDs.c_batch } equals new { q4.c_iteno, q4.c_batch } into q_4
            //               from qBat in q_4.DefaultIfEmpty()
            //               join q5 in
            //                 (from sq1 in db.LG_FJD1s
            //                  join sq2 in db.LG_FJD2s on new { sq1.c_fjno, sq1.c_iteno } equals new { sq2.c_fjno, sq2.c_iteno }
            //                  where (sq1.c_fjno == dop.FakturID)
            //                  select new
            //                  {
            //                    sq1.c_iteno,
            //                    sq1.n_salpri,
            //                    sq2.n_discon,
            //                    sq2.n_discoff
            //                  }).Distinct() on q.c_iteno equals q5.c_iteno into q_5
            //               from qFJs in q_5.DefaultIfEmpty()
            //               where (q.c_dono == dop.DO)
            //               select new ScmsSoaLibrary.Parser.Class.DeliveryOrderDetailPostings()
            //               {
            //                 Item = q.c_iteno,
            //                 NamaItem = q1.v_itnam,
            //                 Jumlah = (q.n_qty.HasValue ? q.n_qty.Value : 0),
            //                 Batch = (qPLDs != null ? (qPLDs.c_batch == null ? string.Empty : qPLDs.c_batch.Trim()) : string.Empty),
            //                 Expired = (qBat != null ? (qBat.d_expired.HasValue ? qBat.d_expired.Value : Functionals.StandardSqlDateTime) : Functionals.StandardSqlDateTime),
            //                 Harga = (qFJs != null ? (qFJs.n_salpri.HasValue ? qFJs.n_salpri.Value : 0) : 0),
            //                 Diskon = (qFJs != null ? (qFJs.n_discon.HasValue ? qFJs.n_discon.Value : 0) : 0),
            //                 SPs = null
            //               }).Distinct().ToList();

            listDetails = (from q in db.LG_DOD1s
                           join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                           join q2 in db.LG_PLD1s on new { c_plno = dop.ReferenceID, q.c_iteno } equals new { q2.c_plno, q2.c_iteno } into q_2
                           from qPLDs in q_2.Where(t => t.n_qty > 0).DefaultIfEmpty()
                           join q4 in db.LG_MsBatches on new { q.c_iteno, qPLDs.c_batch } equals new { q4.c_iteno, q4.c_batch } into q_4
                           from qBat in q_4.DefaultIfEmpty()
                           join q5 in
                             (from sq1 in db.LG_FJD1s
                              join sq2 in db.LG_FJD2s on new { sq1.c_fjno, sq1.c_iteno } equals new { sq2.c_fjno, sq2.c_iteno }
                              where (sq1.c_fjno == dop.FakturID)
                              select new
                              {
                                sq1.c_iteno,
                                sq1.n_salpri,
                                sq2.n_discon,
                                sq2.n_discoff
                              }).Distinct() on q.c_iteno equals q5.c_iteno into q_5
                           from qFJs in q_5.DefaultIfEmpty()
                           where (q.c_dono == dop.DO)
                           select new ScmsSoaLibrary.Parser.Class.DeliveryOrderDetailPostings()
                           {
                             Item = q.c_iteno,
                             //NamaItem = q1.v_itnam,
                             Jumlah = (qPLDs.n_qty.HasValue ? qPLDs.n_qty.Value : 0),
                             Batch = (qPLDs != null ? (qPLDs.c_batch == null ? string.Empty : qPLDs.c_batch.Trim()) : string.Empty),
                             Expired = (qBat != null ? (qBat.d_expired.HasValue ? qBat.d_expired.Value : Functionals.StandardSqlDateTime) : Functionals.StandardSqlDateTime),
                             Harga = (qFJs != null ? (qFJs.n_salpri.HasValue ? qFJs.n_salpri.Value : 0) : 0),
                             Diskon = (qFJs != null ? (qFJs.n_discon.HasValue ? qFJs.n_discon.Value : 0) : 0),
                             SPHs = (qPLDs != null ? (qPLDs.c_spno == null ? string.Empty : qPLDs.c_spno.Trim()) : string.Empty),
                             //PoOutlet = (qPLDs != null ? (qPLDs.c_po_outlet == null ? string.Empty : qPLDs.c_po_outlet.Trim()) : string.Empty),
                             SPs = null
                           }).Distinct().ToList();

            for (nLoop = 0; nLoop < listDetails.Count; nLoop++)
            {
              dodp = listDetails[nLoop];
                
              if (dodp != null)
              {
                  if (dodp.Diskon == 0)
                  {
                      var disc = (from q in db.FA_DiscDs
                                  where q.c_nodisc == "DSXXXXXX03" && q.c_iteno == dodp.Item
                                  select q.n_discon).Take(1).SingleOrDefault();

                      dodp.Diskon = (decimal)disc;
                  }

                dodp.Expired_Str = dodp.Expired.ToString("yyyy-MM-dd");
                
                if (!isStt)
                {
                  dodp.SPs = (from q in db.LG_PLD1s
                              //							join q1 in db.LG_SPD1s on new { q.C_spno, q.C_iteno } equals new { q1.C_spno, q1.C_iteno } into q_1
                              //							from qSPD1s in q_1.DefaultIfEmpty()
                              join q2 in db.LG_SPHs on q.c_spno equals q2.c_spno into q_2
                              from qSPHs in q_2.DefaultIfEmpty()

                              where (q.c_plno == dop.ReferenceID) && (q.c_iteno == dodp.Item)
                                && (q.n_qty > 0) && q.c_batch == dodp.Batch && q.c_spno == dodp.SPHs
                              group new { qSPHs, q } by new { qSPHs.c_sp, q.c_iteno, q.c_batch, q.n_qty } into gSum
                              select new ScmsSoaLibrary.Parser.Class.DeliveryOrderSPDetailPostings()
                              {
                                //SP = (gSum == null ? string.Empty : gSum.Key.c_sp.Trim()),
                                SP = (gSum == null ? string.Empty : gSum.Key.c_sp.Trim()),
                                Jumlah = (gSum.Sum(x => x.q.n_qty.HasValue ? x.q.n_qty.Value : 0))
                              }).Distinct().ToArray();

                  
                }

                //dodp.Batch = dodp.Batch.Replace("/", "<>");
                //dodp.NamaItem = dodp.NamaItem.Replace("&", " ");
              }
            }

            #endregion
          }
          else
          {
            #region STT

            listDetails = (from q in db.LG_DOD2s
                           //join qd2 in db.LG_DOD2s on q.c_dono equals qd2.c_dono
                           join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                           //join q2 in db.LG_STD1s on new { c_stno = dop.ReferenceID, q.c_iteno } equals new { q2.c_stno, q2.c_iteno } into q_2
                           //from qSTDs in q_2.DefaultIfEmpty()
                           join q4 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q4.c_iteno, q4.c_batch } into q_4
                           from qBat in q_4.DefaultIfEmpty()
                           join q5 in
                             (from sq1 in db.LG_FJD1s
                              join sq2 in db.LG_FJD2s on new { sq1.c_fjno, sq1.c_iteno } equals new { sq2.c_fjno, sq2.c_iteno }
                              where (sq1.c_fjno == dop.FakturID)
                              select new
                              {
                                sq1.c_iteno,
                                sq1.n_salpri,
                                sq2.n_discon,
                                sq2.n_discoff
                              }).Distinct() on q.c_iteno equals q5.c_iteno into q_5
                           from qFJs in q_5.DefaultIfEmpty()
                           where (q.c_dono == dop.DO)
                           select new ScmsSoaLibrary.Parser.Class.DeliveryOrderDetailPostings()
                           {
                             Item = q.c_iteno,
                             //NamaItem = q1.v_itnam,
                             Jumlah = (q.n_qty.HasValue ? q.n_qty.Value : 0),
                             Batch = (q != null ? (q.c_batch == null ? string.Empty : q.c_batch.Trim()) : string.Empty),
                             Expired = (qBat != null ? (qBat.d_expired.HasValue ? qBat.d_expired.Value : Functionals.StandardSqlDateTime) : Functionals.StandardSqlDateTime),
                             Harga = (qFJs != null ? (qFJs.n_salpri.HasValue ? qFJs.n_salpri.Value : 0) : 0),
                             Diskon = (qFJs != null ? (qFJs.n_discon.HasValue ? qFJs.n_discon.Value : 0) : 0),
                             //PoOutlet = null,
                             SPs = null
                           }).Distinct().ToList();

            for (nLoop = 0; nLoop < listDetails.Count; nLoop++)
            {
              dodp = listDetails[nLoop];

              if (dodp != null)
              {
                dodp.Expired_Str = dodp.Expired.ToString("yyyy-MM-dd");
              }
            }

            #endregion
          }

          #region Old Coded

          //listDetails = (from q in db.LG_DOD1s
          //               join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
          //               join q2 in db.LG_PLD1s on new { c_plno = dop.ReferenceID, q.c_iteno } equals new { q2.c_plno, q2.c_iteno } into q_2
          //               from qPLDs in q_2.DefaultIfEmpty()
          //               join q3 in db.LG_STD1s on new { c_gdg = dop.Gudang, c_stno = dop.ReferenceID, q.c_iteno } equals new { q3.c_gdg, q3.c_stno, q3.c_iteno } into q_3
          //               from qSTDs in q_3.DefaultIfEmpty()
          //               join q4 in db.LG_MsBatches on new { q.c_iteno, c_batch = (dop.TypeCode == "02" ? qSTDs.c_batch : qPLDs.c_batch) } equals new { q4.c_iteno, q4.c_batch } into q_4
          //               from qBat in q_4.DefaultIfEmpty()
          //               join q5 in
          //                 (from sq1 in db.LG_FJD1s
          //                  join sq2 in db.LG_FJD2s on sq1.c_fjno equals sq2.c_fjno
          //                  where (sq1.c_fjno == dop.FakturID)
          //                  select new
          //                  {
          //                    sq1.c_iteno,
          //                    sq1.n_salpri,
          //                    sq2.n_discon,
          //                    sq2.n_discoff
          //                  }) on q.c_iteno equals q5.c_iteno into q_5
          //               from qFJs in q_5.DefaultIfEmpty()
          //               where (q.c_dono == dop.DO)
          //               select new ScmsSoaLibrary.Parser.Class.DeliveryOrderDetailPostings()
          //               {
          //                 Item = q.c_iteno,
          //                 NamaItem = q1.v_itnam,
          //                 Jumlah = (q.n_qty.HasValue ? q.n_qty.Value : 0),
          //                 Batch = (dop.TypeCode == "02" ?
          //                  (qSTDs != null ?
          //                    (qSTDs.c_batch == null ? string.Empty : qSTDs.c_batch.Trim()) : string.Empty) :
          //                  (qPLDs != null ?
          //                    (qPLDs.c_batch == null ? string.Empty : qPLDs.c_batch.Trim()) : string.Empty)),
          //                 Expired = (qBat != null ?
          //                                (qBat.d_expired.HasValue ? qBat.d_expired.Value : Functionals.StandardSqlDateTime) : Functionals.StandardSqlDateTime),
          //                 Harga = (qFJs != null ?
          //                                (qFJs.n_salpri.HasValue ? qFJs.n_salpri.Value : 0) : 0),
          //                 Diskon = (qFJs != null ?
          //                                (qFJs.n_discon.HasValue ? qFJs.n_discon.Value : 0) : 0),
          //                 SPs = (from sq in db.LG_SPHs
          //                        join sq1 in db.LG_SPD1s on sq.c_spno equals sq1.c_spno
          //                        where (sq.c_spno ==
          //                          (qPLDs != null ?
          //                                (qPLDs.c_spno == null ? string.Empty : qPLDs.c_spno.Trim()) : string.Empty)) &&
          //                              (sq1.c_iteno ==
          //                          (qPLDs != null ?
          //                                (qPLDs.c_iteno == null ? string.Empty : qPLDs.c_iteno.Trim()) : string.Empty))
          //                        //group sq1 by new { sq1.c_spno, sq1.c_iteno } into g
          //                        select new ScmsSoaLibrary.Parser.Class.DeliveryOrderSPDetailPostings()
          //                        {
          //                          SP = sq.c_sp,
          //                          Jumlah = (qPLDs != null ?
          //                                         (qPLDs.n_qty.HasValue ? qPLDs.n_qty.Value : 0) : 0)
          //                        }).Distinct().ToArray()
          //               }).Distinct().ToList();

          #endregion

          #endregion

          if (listDetails.Count > 0)
          {
            //for (int nLoops = 0; nLoops < listDetails.Count; nLoops++)
            //{
            //  listDetails[nLoops].Batch.Replace("/", "<>");
            //  listDetailsFinal.Insert();
            //}

            dop.Fields = listDetails.ToArray();

            listDetails.Clear();
          }

          result = ScmsSoaLibrary.Parser.Class.DeliveryOrderPosting.Serialize(dop);
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine(string.Format("ScmsSoaLibrary.Modules.CommonQuerySP:SP_LG_CalcFJ - {0}", ex.Message));
      }

      return result;
    }

    private bool PostDataDODirect(ORMDataContext db, string doNo, bool isStt, bool directCommit)
    {
      string dataResult = GetDataDO(db, doNo, isStt);
      
      bool bResult = false;

      Config cfg = Functionals.Configuration;

      Dictionary<string, string> dicParam = new Dictionary<string, string>();
      dicParam.Add("param", dataResult);

      Dictionary<string, string> dicHeader = new Dictionary<string, string>();
      dicHeader.Add("X-Requested-With", "XMLHttpRequest");

      IDictionary<string, object> dic = new Dictionary<string, object>();

      ScmsSoaLibrary.Parser.ResponseParser res = new ScmsSoaLibrary.Parser.ResponseParser();

      ScmsSoaLibrary.Core.Response.DiscoreResponse ds = new ScmsSoaLibrary.Core.Response.DiscoreResponse();

      ScmsSoaLibrary.Parser.ParserDisCore pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

      pdc.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
      pdc.Referer = Functionals.DistCoreUrlBuilderString(cfg, "http://10.100.11.52/dcore/?m=com.ams.welcome");

      Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.28/dcore/?m=com.ams.json.ds&action=form&f=Submit&_q=trx_update_do_rn");
      
      //Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.28/dcore/?m=com.ams.json.ds&action=form&f=Submit&_q=trx_update_do_rnxx");

      string result = null;

      bool isError = false;

      StringBuilder sb = new StringBuilder();

      Encoding utf8 = Encoding.UTF8;

      DateTime date = DateTime.MinValue;

      sb.AppendLine(uri.ToString());
      sb.AppendLine(dataResult);

      Logger.WriteLine(uri.ToString());

      

      if (pdc.PostGetData(uri, dicParam, dicHeader))
      {
        result = utf8.GetString(pdc.Result);

        bResult = true;
      }
      else
      {
        result = pdc.ErrorMessage+ " "+ uri;
        isError = true;
      }

      Logger.WriteLine(result, true);

      Bussiness.Commons.InsertReceivedRespose(db, sb.ToString(), result, isError, directCommit, "DO", doNo);

      dic.Clear();

      dicHeader.Clear();
      dicParam.Clear();

      return bResult;
    }

    #endregion

    #endregion

    public string[] SP_LG_CalcFJ(string connectionString, IDictionary<string, Functionals.ParameterParser> parameters)
    {
      string cust = (parameters.ContainsKey("customer") ? (string)parameters["customer"].Value : string.Empty),
        dono1 = (parameters.ContainsKey("do_from") ? (string)parameters["do_from"].Value : string.Empty),
        dono2 = (parameters.ContainsKey("do_to") ? (string)parameters["do_to"].Value : string.Empty),
        user = (parameters.ContainsKey("user") ? (string)parameters["user"].Value : string.Empty);
      decimal SisaFJ = (parameters.ContainsKey("SisaFJ") ? (decimal)parameters["SisaFJ"].Value : 0); //Indra 20170613

      if (string.IsNullOrEmpty(cust) || string.IsNullOrEmpty(dono1) || string.IsNullOrEmpty(user))
      {
        return new string[0];
      }

      Config cfg = Functionals.Configuration;

      string result = null;

      List<string> listReturn = new List<string>();

      ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);

      db.Connection.Open();

      db.Transaction = db.Connection.BeginTransaction();

      DateTime date = DateTime.Now;

      dono2 = (dono2 == null ? dono1 : (dono2.Equals(dono1, StringComparison.OrdinalIgnoreCase) ? dono1 : dono2));

      LG_DOH doh = null;

      List<LG_FJH> listFJH = new List<LG_FJH>();
      List<LG_FJD1> listFJD1 = new List<LG_FJD1>();
      List<LG_FJD2> listFJD2 = new List<LG_FJD2>();
      List<LG_FJD3> listFJD3 = new List<LG_FJD3>();

      List<LG_DOH> listDOH = null;
      List<string> listDoStr = null;
      List<DOInformation> listDO = null;

      List<PROCESS_DO_TO_FJ> listDOFJ = null, 
        //listDOSTTFJ = null,
        listFiltDOFJ = null;

      PROCESS_DO_TO_FJ pDoFJ = null;

      List<ScmsSoaLibrary.Bussiness.Commons.FakturPajakInformation> listFPI = new List<ScmsSoaLibrary.Bussiness.Commons.FakturPajakInformation>();
      List<ScmsSoaLibrary.Bussiness.Commons.FakturPajakInformation> listFreeFPI = ScmsSoaLibrary.Bussiness.Commons.FreeFakturPajak(db);
      ScmsSoaLibrary.Bussiness.Commons.FakturPajakInformation fpi = null;

      int nLoop = 0,
        nLen = 0,
        nLoopC = 0,
        nLenC = 0,
        nTop = 0,
        nTopPjg = 0,
        nCount = 0;

      string noFj = null;

      decimal sumGrossDetail = 0,
        sumDisc = 0,
        sumHnaDisc = 0,
        hna = 0,
        disc = 0,
        discOff = 0,
        hnaDisc = 0,
        hnaDiscOff = 0,
        qtyTotal = 0,
        nTax = 0;

      bool isCabang = false,
        isDoPL = false,
        isFailed = false;
      LG_Cusma cus = null;

      DOInformation doinfo = null;

      try
      {
        cus = (from q in db.LG_Cusmas
               where q.c_cusno == cust
               select q).SingleOrDefault();

        if (cus != null)
        {
          nTop = (cus.t_top.HasValue ? cus.t_top.Value : 0);
          nTopPjg = (cus.t_toppjg.HasValue ? cus.t_toppjg.Value : 0);
          isCabang = (cus.l_cabang.HasValue ? cus.l_cabang.Value : false);
        }

        if (dono1.Equals(dono2, StringComparison.OrdinalIgnoreCase))
        {
          listDOH = (from q in db.LG_DOHs
                     join q1 in db.LG_DOD1s on q.c_dono equals q1.c_dono
                     where q.c_cusno == cust && q.c_dono == dono1
                      //&& ((q.l_send.HasValue ? q.l_send.Value : false) == true)
                      //&& ((q.l_sent.HasValue ? q.l_sent.Value : false) == false)
                     //&& q.l_receipt == false
                     && ((q.l_receipt.HasValue ? q.l_receipt.Value : false) == false)
                     && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                     select q).Distinct().ToList();
        }
        else
        {
          var qry = (from q in db.LG_DOHs
                     join q1 in db.LG_DOD1s on q.c_dono equals q1.c_dono
                     where q.c_cusno == cust
                      //&& ((q.l_send.HasValue ? q.l_send.Value : false) == true)
                      //&& ((q.l_sent.HasValue ? q.l_sent.Value : false) == false)
                      && q.l_receipt == false
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                     select q).AsQueryable();

          listDOH = qry.Between("c_dono", dono1, dono2).Distinct().ToList();
        }

        if (listDOH.Count > 0)
        {
          listDoStr = listDOH.GroupBy(x => x.c_dono).Select(y => (string.IsNullOrEmpty(y.Key) ? string.Empty : y.Key.Trim())).ToList();

          if (isCabang)
          {
            listDO = listDOH.GroupBy(x => new { x.c_dono, x.c_type }).Select(y =>
              new DOInformation()
              {
                ID = (string.IsNullOrEmpty(y.Key.c_dono) ? string.Empty : y.Key.c_dono.Trim()),
                IsStt = (string.IsNullOrEmpty(y.Key.c_type) ? false : (y.Key.c_type.Equals("02", StringComparison.OrdinalIgnoreCase) ? true : false))
              }).ToList();
          }
          else
          {
            listDO = new List<DOInformation>();
          }

          #region Old Coded

          //listDOFJ = (from q in db.LG_DOD1s
          //            join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
          //            join q2 in db.FA_DiscDs on new { q.c_iteno, c_nodisc = "DSXXXXXX03" } equals new { q2.c_iteno, q2.c_nodisc } into q_2
          //            from qDSC in q_2.DefaultIfEmpty()
          //            where listDo.Contains(q.c_dono)
          //              && (qDSC.l_aktif == true) && (qDSC.l_status == true)
          //            select new PROCESS_DO_TO_FJ()
          //            {
          //              DoNo = q.c_dono.Trim(),
          //              Item = q.c_iteno,
          //              Via = q.c_via,
          //              Qty = (q.n_qty.HasValue ? q.n_qty.Value : 0),
          //              Sisa = (q.n_sisa.HasValue ? q.n_sisa.Value : 0),
          //              HNA = (q1.n_salpri.HasValue ? q1.n_salpri.Value : 0),
          //              Disc = (qDSC.n_discon.HasValue ? qDSC.n_discon.Value : 0)
          //            }).Distinct().ToList();

          #endregion

          #region Populate DO Details
		  //Indra 20170904 Penambahan validasi SPPHAR dan TRIM
          if (listDOH[0].c_entry.ToUpper().Trim() == "SPPHAR1" || listDOH[0].c_entry.ToUpper().Trim() == "SPPHAR2")
          {
              listDOFJ = (from q0 in db.LG_DOHs
                          join q in db.LG_DOD1s on q0.c_dono equals q.c_dono
                          join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                          join q2 in
                              (
                                from sq in db.LG_DOPDs
                                where sq.c_po_outlet != ""
                                select new
                                {
                                    c_nodisc = "DSXXXXXX04",
                                    sq.c_iteno,
                                    n_discon = sq.n_disc,
                                    n_discoff = sq.n_damsoff,
                                    sq.c_po_outlet
                                }) on new { q.c_iteno, q0.c_po_outlet } equals new { q2.c_iteno, q2.c_po_outlet } into q_2
                          from qDSCT in q_2.DefaultIfEmpty()
                          join q3 in
                              (
                              from sq in db.LG_PLD1s
                              where (sq.c_type == "02") && (sq.n_qty > 0)
                              group sq by new { sq.c_plno, sq.c_iteno } into g
                              select new
                              {
                                  g.Key.c_plno,
                                  g.Key.c_iteno,
                                  n_qty = g.Sum(x => (x.n_qty.HasValue ? x.n_qty.Value : 0))
                              }) on new { q0.c_plno, q.c_iteno } equals new { q3.c_plno, q3.c_iteno } into q_3
                          from qPLD in q_3.DefaultIfEmpty()
                          where listDoStr.Contains(q.c_dono)
                          select new PROCESS_DO_TO_FJ()
                          {
                              DoNo = q.c_dono.Trim(),
                              Item = q.c_iteno,
                              Via = q.c_via,
                              Qty = (q.n_qty.HasValue ? q.n_qty.Value : 0),
                              Sisa = (q.n_sisa.HasValue ? q.n_sisa.Value : 0),
                              HNA = (q1.n_salpri.HasValue ? q1.n_salpri.Value : 0),
                              Disc = (qDSCT != null ? (qDSCT.n_discon.HasValue ? qDSCT.n_discon.Value : 0) : 0),
                              DiscOff = (qDSCT != null ? (qDSCT.n_discoff.HasValue ? qDSCT.n_discoff.Value : 0) : 0),
                              Bonus = (qPLD != null ? qPLD.n_qty : 0),
                              DiscountCode = (qDSCT != null ? (qDSCT.c_nodisc ?? "") : "")
                          }).Distinct().ToList();
          }
          else
          {
              listDOFJ = (from q0 in db.LG_DOHs
                          join q in db.LG_DOD1s on q0.c_dono equals q.c_dono
                          join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                          join q2 in
                              (
                                from sq in db.FA_DiscHes
                                join sq1 in db.FA_DiscDs on sq.c_nodisc equals sq1.c_nodisc
                                where sq.c_type == "03" && sq1.l_aktif == true && sq1.l_status == true
                                select new
                                {
                                    sq.c_nodisc,
                                    sq1.c_iteno,
                                    sq1.n_discon,
                                    sq1.n_discoff
                                }) on q.c_iteno equals q2.c_iteno into q_2
                          from qDSCT in q_2.DefaultIfEmpty()
                          //					join q2 in db.FA_DiscDs on new { q.C_iteno } equals new { q2.C_iteno } into q_2
                          //					from qDSC in q_2.DefaultIfEmpty()
                          //					join q3 in db.FA_DiscHes on new { qDSC.C_nodisc, C_type = "03" } equals new { q3.C_nodisc, q3.C_type } into q_3
                          //					from qDSCH in q_3.DefaultIfEmpty()
                          join q3 in
                              (
                              from sq in db.LG_PLD1s
                              where (sq.c_type == "02") && (sq.n_qty > 0)
                              group sq by new { sq.c_plno, sq.c_iteno } into g
                              select new
                              {
                                  g.Key.c_plno,
                                  g.Key.c_iteno,
                                  n_qty = g.Sum(x => (x.n_qty.HasValue ? x.n_qty.Value : 0))
                              }) on new { q0.c_plno, q.c_iteno } equals new { q3.c_plno, q3.c_iteno } into q_3
                          from qPLD in q_3.DefaultIfEmpty()
                          where listDoStr.Contains(q.c_dono) //&& (q0.c_type == "01")
                          select new PROCESS_DO_TO_FJ()
                          {
                              DoNo = q.c_dono.Trim(),
                              Item = q.c_iteno,
                              Via = q.c_via,
                              Qty = (q.n_qty.HasValue ? q.n_qty.Value : 0),
                              Sisa = (q.n_sisa.HasValue ? q.n_sisa.Value : 0),
                              HNA = (q1.n_salpri.HasValue ? q1.n_salpri.Value : 0),
                              Disc = (qDSCT != null ? (qDSCT.n_discon.HasValue ? qDSCT.n_discon.Value : 0) : 0),
                              DiscOff = (qDSCT != null ? (qDSCT.n_discoff.HasValue ? qDSCT.n_discoff.Value : 0) : 0),
                              Bonus = (qPLD != null ? qPLD.n_qty : 0),
                              DiscountCode = (qDSCT != null ? (qDSCT.c_nodisc ?? "") : "")
                          }).Distinct().ToList();
          }
          #region Old Coded

          //listDOSTTFJ = (from q0 in db.LG_DOHs
          //               join q in db.LG_DOD1s on q0.c_dono equals q.c_dono
          //               join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
          //               join q2 in
          //                 (
          //                   from sq in db.FA_DiscHes
          //                   join sq1 in db.FA_DiscDs on sq.c_nodisc equals sq1.c_nodisc
          //                   where sq.c_type == "03" && sq1.l_aktif == true && sq1.l_status == true
          //                   select new
          //                   {
          //                     sq.c_nodisc,
          //                     sq1.c_iteno,
          //                     sq1.n_discon,
          //                     sq1.n_discoff
          //                   }) on q.c_iteno equals q2.c_iteno into q_2
          //               from qDSCT in q_2.DefaultIfEmpty()
          //               where listDo.Contains(q.c_dono) && (q0.c_type == "02")
          //               select new PROCESS_DO_TO_FJ()
          //               {
          //                 DoNo = q.c_dono.Trim(),
          //                 Item = q.c_iteno,
          //                 Via = q.c_via,
          //                 Qty = (q.n_qty.HasValue ? q.n_qty.Value : 0),
          //                 Sisa = (q.n_sisa.HasValue ? q.n_sisa.Value : 0),
          //                 HNA = (q1.n_salpri.HasValue ? q1.n_salpri.Value : 0),
          //                 Disc = (qDSCT != null ? (qDSCT.n_discon.HasValue ? qDSCT.n_discon.Value : 0) : 0),
          //                 DiscOff = (qDSCT != null ? (qDSCT.n_discoff.HasValue ? qDSCT.n_discoff.Value : 0) : 0),
          //                 Bonus = 0,
          //                 DiscountCode = (qDSCT != null ? (qDSCT.c_nodisc ?? "") : "")
          //               }).Distinct().ToList();

          //listDOFJ.AddRange(listDOSTTFJ.ToArray());

          //listDOSTTFJ.Clear();

          #endregion

          #endregion

          #region Populate To FJ

          for (nLoop = 0, nLen = listDoStr.Count; nLoop < nLen; nLoop++)
          {
            dono1 = listDoStr[nLoop];

            if (string.IsNullOrEmpty(dono1))
            {
              continue;
            }

            doh = listDOH.Find(delegate(LG_DOH don)
            {
              return dono1.Equals(don.c_dono.Trim(), StringComparison.OrdinalIgnoreCase);
            });

            listFiltDOFJ = listDOFJ.FindAll(delegate(PROCESS_DO_TO_FJ dod)
            {
              return dono1.Equals(dod.DoNo, StringComparison.OrdinalIgnoreCase);
            });

            if ((doh != null) && (listFiltDOFJ.Count > 0))
            {
              noFj = string.Concat("FJ", dono1.Substring(2)).Trim();

              if (Bussiness.Commons.HasFJExists(db, noFj))
              {
                goto Akhir;
              }

              listReturn.Add(noFj);

              //typeCode = (doh.c_type == null ? "01" : doh.c_type);
              isDoPL = (string.IsNullOrEmpty(doh.c_type) || doh.c_type.Equals("01", StringComparison.OrdinalIgnoreCase) ? true : false);

              #region Populate Into Detail FJ

              LG_FJD1 fjd1 = null; //Indra 20170717

              for (nLoopC = 0, nLenC = listFiltDOFJ.Count; nLoopC < nLenC; nLoopC++)
              {
                pDoFJ = listFiltDOFJ[nLoopC];

                //qtyTotal = (typeCode.Equals("01", StringComparison.OrdinalIgnoreCase) ? (pDoFJ.Qty - pDoFJ.Bonus) : pDoFJ.Qty);
                qtyTotal = (isDoPL ? (pDoFJ.Qty - pDoFJ.Bonus) : pDoFJ.Qty);

                if (qtyTotal > 0)
                {
                  hna = (qtyTotal * pDoFJ.HNA);
                  disc = (hna * (pDoFJ.Disc / 100));
                  discOff = (hna * (pDoFJ.DiscOff / 100));

                  hnaDisc = (hna - disc);
                  hnaDiscOff = (hna - discOff);

                  //Indra 20170717 tambah kondisi
                  fjd1 = (from q in db.LG_FJD1s
                         where q.c_fjno == noFj && q.c_iteno == pDoFJ.Item
                         select q).Take(1).SingleOrDefault();

                  if (fjd1 == null)
                  {
                      listFJD1.Add(new LG_FJD1()
                      {
                          c_fjno = noFj,
                          c_iteno = pDoFJ.Item,
                          n_disc = (isDoPL ? pDoFJ.Bonus : 0),
                          n_qty = qtyTotal,
                          n_salpri = pDoFJ.HNA
                      });

                      listFJD2.Add(new LG_FJD2()
                      {
                          c_fjno = noFj,
                          c_iteno = pDoFJ.Item,
                          c_type = "03",
                          c_no = pDoFJ.DiscountCode,
                          n_discoff = pDoFJ.DiscOff,
                          n_discon = pDoFJ.Disc,
                          n_neton = disc,
                          n_netoff = discOff,
                          n_sisaon = hnaDisc,
                          n_sisaoff = hnaDiscOff
                      });
                  }

                  sumGrossDetail += hna;
                  sumDisc += disc;
                  sumHnaDisc += hnaDisc;
                }
              }

              #endregion
              
              #region Faktur Pajak

              if (isCabang)
              {
                fpi = null;

                nTax = 0;
              }
              else
              {
                //listFreeFPI.Add(item
                if (listFreeFPI.Count > 0)
                {
                  fpi = listFreeFPI[nLoop];

                  listFreeFPI.Remove(fpi);
                }
                else
                {
                  fpi = ScmsSoaLibrary.Bussiness.Commons.GetFakturPajakDisCore(cfg, noFj);
                }
                if ((fpi != null) && (!listFPI.Contains(fpi)))
                {
                  fpi.NoFaktur = noFj;

                  listFPI.Add(fpi);
                }

                nTax = ((sumGrossDetail - sumDisc) * 0.1m);
              }

              nCount++;

              #endregion

              listFJH.Add(new LG_FJH()
              {
                c_cusno = cust,
                c_entry = user,
                c_fjno = noFj,
                c_kurs = "01",
                c_taxno = (isCabang ? string.Empty : (fpi != null ? (string.IsNullOrEmpty(fpi.NoFakturPajak) ? string.Empty : fpi.NoFakturPajak.Trim()) : string.Empty)),
                c_update = user,
                d_entry = date,
                d_fjdate = doh.d_dodate,
                d_taxdate = (isCabang ? Functionals.StandardSqlDateTime : (fpi != null ? fpi.TanggalFakturPajak : Functionals.StandardSqlDateTime)),
                d_top = date.AddDays(nTop),
                d_toppjg = date.AddDays(nTopPjg),
                d_update = date,
                l_cabang = isCabang,
                l_print = false,
                n_disc = sumDisc,
                n_gross = sumGrossDetail,
                n_kurs = 1,
                n_net = (sumGrossDetail + nTax - sumDisc),
                //n_sisa = (sumGrossDetail + nTax - sumDisc), Indra 20170613
                n_sisa = (sumGrossDetail + nTax - sumDisc) + SisaFJ,
                n_tax = nTax,
                n_top = nTop,
                n_toppjg = nTopPjg,
                v_ket = "Sys: Auto Generate"
              });

              listFJD3.Add(new LG_FJD3()
              {
                c_dono = dono1,
                c_fjno = noFj
              });
            }

          Akhir:
            if ((doh != null) && isCabang)
            {
              doh.l_sent = true;
            }

            if (listFiltDOFJ != null)
            {
              listDOFJ.RemoveAll(delegate(PROCESS_DO_TO_FJ dod)
              {
                return dono1.Equals(dod.DoNo, StringComparison.OrdinalIgnoreCase);
              });
            }
            if ((listDOH != null) && (doh != null))
            {
              listDOH.Remove(doh);
            }
          }

          #endregion

          #region Insert To DB

          if (listFPI.Count > 0)
          {
            ScmsSoaLibrary.Bussiness.Commons.UpdateFakturPajak(db, listFPI.ToArray());
            //listFPI.Clear();
          }
          if (listFJH.Count > 0)
          {
            db.LG_FJHs.InsertAllOnSubmit(listFJH.ToArray());
            listFJH.Clear();
          }
          if (listFJD1.Count > 0)
          {
            db.LG_FJD1s.InsertAllOnSubmit(listFJD1.ToArray());
            listFJD1.Clear();
          }
          if (listFJD2.Count > 0)
          {
            db.LG_FJD2s.InsertAllOnSubmit(listFJD2.ToArray());
            listFJD2.Clear();
          }
          if (listFJD3.Count > 0)
          {
            db.LG_FJD3s.InsertAllOnSubmit(listFJD3.ToArray());
            listFJD3.Clear();
          }

          #endregion

          if (nCount > 0)
          {
            db.SubmitChanges();

            if (db.Transaction != null)
            {
              db.Transaction.Commit();
              //db.Transaction.Rollback();
            }
          }
          else
          {
            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }
          }
        }
      }
      catch (Exception ex)
      {
        isFailed = true;

        result = string.Format("ScmsSoaLibrary.Modules.CommonQuerySP:SP_LG_CalcFJ - {0}", ex.Message);

        Logger.WriteLine(result);
        Logger.WriteLine(ex.StackTrace);

        if (db.Transaction != null)
        {
          db.Transaction.Rollback();
        }
      }

      if (isFailed && (listFPI.Count > 0))
      {
        db.Transaction = db.Connection.BeginTransaction();

        try
        {
          ScmsSoaLibrary.Bussiness.Commons.UpdateFakturPajak(db, true, listFPI.ToArray());

          db.SubmitChanges();

          db.Transaction.Commit();
        }
        catch (Exception ex)
        {
          result = string.Format("ScmsSoaLibrary.Modules.CommonQuerySP:SP_LG_CalcFJ UpdateFakturPajak_markAllFree - {0}", ex.Message);

          Logger.WriteLine(result);
          Logger.WriteLine(ex.StackTrace);

          if (db.Transaction != null)
          {
            db.Transaction.Rollback();
          }
        }

        listFPI.Clear();
      }

      if ((listDO != null) && (listDO.Count > 0))
      {
          db.Transaction = db.Connection.BeginTransaction();

          for (nLoop = 0, nLen = listDO.Count; nLoop < nLen; nLoop++)
          {
              doinfo = listDO[nLoop];
              if ((doinfo != null) && (!string.IsNullOrEmpty(doinfo.ID)))
              {
                  if (PostDataDODirect(db, doinfo.ID, doinfo.IsStt, false))
                  {
                      nCount++;
                  }
              }
          }

          try
          {
              if (nCount > 0)
              {
                  db.SubmitChanges();

                  if (db.Transaction != null)
                  {
                      db.Transaction.Commit();
                      //db.Transaction.Rollback();
                  }
              }
              else
              {
                  if (db.Transaction != null)
                  {
                      db.Transaction.Rollback();
                  }
              }
          }
          catch (Exception ex)
          {
              result = string.Format("ScmsSoaLibrary.Modules.CommonQuerySP:SP_LG_CalcFJ PostDataDODirect - {0}", ex.Message);

              Logger.WriteLine(result);
              Logger.WriteLine(ex.StackTrace);

              if (db.Transaction != null)
              {
                  db.Transaction.Rollback();
              }
          }
      }

      if (listDOH != null)
      {
        listDOH.Clear();
      }

      if (listDO != null)
      {
        listDO.Clear();
      }

      if (listDoStr != null)
      {
        listDoStr.Clear();
      }

      db.Dispose();

      return listReturn.ToArray();
    }

    public string[] SP_LG_CalcClosingLog(string connectionString, string User, bool createFakturBeliRetur, CommonQueryProcess.LG_STOCKTMP[] StockTMP)
    {
      if (string.IsNullOrEmpty(User) || (StockTMP == null) || (StockTMP.Length < 1))
      {
        return new string[0];
      }
      string result = null;

      List<string> listReturn = new List<string>();

      ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);

      db.Connection.Open();

      db.Transaction = db.Connection.BeginTransaction();

      List<LG_Stock> lstStock = null;
      //List<LG_RSD3> listRSD3 = null;

      try
      {
        lstStock = (from q in StockTMP
                    select new LG_Stock
                    {
                      c_batch = q.c_batch,
                      c_gdg = q.c_gdg,
                      c_iteno = q.c_iteno,
                      c_no = q.c_noref,
                      n_bqty = q.n_bqty,
                      n_gqty = q.n_gqty,
                      s_tahun = q.s_tahun,
                      t_bulan = q.t_bulan
                    }).ToList();

        db.LG_Stocks.InsertAllOnSubmit(lstStock.ToArray());
      }
      catch (Exception ex)
      {
        result = string.Format("ScmsSoaLibrary.Modules.CommonQuerySP:SP_LG_CalcClosingLog - {0}", ex.Message);

        Logger.WriteLine(result);

        if (db.Transaction != null)
        {
          //db.Transaction.Commit();
          db.Transaction.Rollback();
        }
      }

      //StockTMP.Clear();

      db.Dispose();

      return listReturn.ToArray();

    }

    public string[] SP_LG_CalcRS(string connectionString, string User, bool createFakturBeliRetur, CommonUploadedQuery.Temporary_ImportRS[] importRSes)
    {
      if (string.IsNullOrEmpty(User) || (importRSes == null) || (importRSes.Length < 1))
      {
        return new string[0];
      }

      string result = null;

      List<string> listReturn = new List<string>();
      List<CommonUploadedQuery.Temporary_ImportRS> listTIRS = new List<CommonUploadedQuery.Temporary_ImportRS>();

      ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);

      db.CommandTimeout = 0;
      db.Connection.Open();

      db.Transaction = db.Connection.BeginTransaction();

      List<LG_RSD2> listRSD2 = null;
      //List<LG_RSD3> listRSD3 = null;

      LG_RSD2 rsd2 = null;

      List<string> listRSNo = importRSes.GroupBy(x => x.c_rsno).Select(y => y.Key).ToList();
      CommonUploadedQuery.Temporary_ImportRS tirs = null;

      //Dictionary<string, int> dicRsNo = new Dictionary<string, int>();

      int nLoop = 0,
        //nValue = 0,
        nLen = 0;
      //int? nReslt = 0;
      decimal nQtyBad = 0,
        nQtyGood = 0,
        nQtyBadPerform = 0,
        nQtyGoodPerform = 0,
        nQtyDb = 0,
        nQtyUpld = 0;

      DateTime date = DateTime.Now;
      ScmsSoaLibrary.Bussiness.Faktur faktur = new ScmsSoaLibrary.Bussiness.Faktur();

      try
      {
        if ((listRSNo != null) && (listRSNo.Count > 0))
        {
          listRSD2 = (from q in db.LG_RSHes
                      join q1 in db.LG_RSD2s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                      where listRSNo.Contains(q.c_rsno) && (q1.l_status == false)
                        && (q1.l_confirm == true) && (q.c_gdg == '1') && (q.c_type == "01")
                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      select q1).Distinct().ToList();

          if ((listRSD2 != null) && (listRSD2.Count > 0))
          {
            //listRSD3 = new List<LG_RSD3>();

            for (nLoop = 0, nLen = importRSes.Length; nLoop < nLen; nLoop++)
            {
              tirs = importRSes[nLoop];

              #region Old Coded

              //#region Get Current Urut

              //if (!dicRsNo.ContainsKey(tirs.c_rsno))
              //{
              //  nReslt = (from q in db.LG_RSD3s where q.c_gdg == '1' && q.c_rsno == tirs.c_rsno orderby q.i_urut descending select (q.i_urut.HasValue ? q.i_urut.Value : 1)).Take(1).SingleOrDefault();
              //  nValue = (nReslt.HasValue ? nReslt.Value : 0);
              //  if (nValue < 1)
              //  {
              //    nValue = 1;
              //  }
              //  else
              //  {
              //    nValue++;
              //  }

              //  dicRsNo.Add(tirs.c_rsno, nValue);
              //}
              //else
              //{
              //  nValue = dicRsNo[tirs.c_rsno];
              //  nValue++;
              //  dicRsNo[tirs.c_rsno] = nValue;
              //}

              //#endregion

              #endregion

              nQtyUpld = tirs.n_up_bsisa + tirs.n_up_gsisa;

            reRead:
              rsd2 = listRSD2.Find(delegate(LG_RSD2 rsd)
              {
                return (
                    tirs.c_rsno.Equals(rsd.c_rsno, StringComparison.OrdinalIgnoreCase) &&
                    tirs.c_iteno.Equals(rsd.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                    tirs.c_batch.Equals((string.IsNullOrEmpty(rsd.c_batch) ? string.Empty : rsd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    (((rsd.n_bsisa.HasValue ? rsd.n_bsisa.Value : 0) > 0) || ((rsd.n_gsisa.HasValue ? rsd.n_gsisa.Value : 0) > 0))
                  //&& tirs.n_bsisa.Equals((rsd.n_bsisa.HasValue ? rsd.n_bsisa.Value : 0)) 
                  //&& tirs.n_gsisa.Equals((rsd.n_gsisa.HasValue ? rsd.n_gsisa.Value : 0))
                      );
              });

              if ((rsd2 != null) && (nQtyUpld > 0))
              {
                tirs.c_rnno = rsd2.c_rnno;

                nQtyBad = (rsd2.n_bsisa.HasValue ? rsd2.n_bsisa.Value : 0);
                nQtyGood = (rsd2.n_gsisa.HasValue ? rsd2.n_gsisa.Value : 0);

                nQtyDb = nQtyBad + nQtyGood;
                
                nQtyBadPerform = (nQtyUpld > nQtyBad ? nQtyBad : nQtyUpld);
                nQtyUpld -= nQtyBadPerform;

                nQtyGoodPerform = (nQtyUpld > nQtyGood ? nQtyGood : nQtyUpld);
                nQtyUpld -= nQtyGoodPerform;

                rsd2.n_bsisa -= nQtyBadPerform;
                rsd2.n_gsisa -= nQtyGoodPerform;

                #region Old Coded

                //listRSD3.Add(new LG_RSD3()
                //{
                //  i_urut = nValue,
                //  c_gdg = rsd2.c_gdg,
                //  c_rsno = rsd2.c_rsno,
                //  c_iteno = rsd2.c_iteno,
                //  c_batch = rsd2.c_batch,
                //  c_fbno = tirs.c_up_exfaktur,
                //  c_taxno = tirs.c_up_taxno,
                //  d_taxdate = tirs.d_up_taxdate,
                //  n_bea = 0,
                //  n_bqty = nQtyBadPerform,
                //  n_bsisa = 0,
                //  //n_bqty = tirs.n_bsisa,
                //  //n_bsisa = tirs.n_bsisa,
                //  n_gqty = nQtyGoodPerform,
                //  n_gsisa = 0,
                //  //n_gqty = tirs.n_gsisa,
                //  //n_gsisa = tirs.n_gsisa,
                //  n_disc = tirs.n_up_disc,
                //  n_salpri = tirs.n_up_salpri,
                //  d_entry = date,
                //  c_entry = User
                //});

                #endregion

                listTIRS.Add(new CommonUploadedQuery.Temporary_ImportRS()
                {
                   c_rsno = rsd2.c_rsno,
                   c_rnno = rsd2.c_rnno,
                   n_bsisa = nQtyBadPerform,
                   n_gsisa = nQtyGoodPerform,
                   n_disc = tirs.n_up_disc,
                   n_salpri = tirs.n_up_salpri,
                   c_fb = tirs.c_up_exfaktur,
                   c_taxno = tirs.c_up_taxno,
                   d_taxdate = tirs.d_up_taxdate,
                });

                if (((rsd2.n_bsisa.HasValue ? rsd2.n_bsisa.Value : 0) <= 0) &&
                  ((rsd2.n_gsisa.HasValue ? rsd2.n_gsisa.Value : 0) <= 0))
                {
                  rsd2.l_status = true;
                  
                  listRSD2.Remove(rsd2);
                }

                tirs.n_up_bsisa -= (nQtyBadPerform > tirs.n_up_bsisa ? tirs.n_up_bsisa : nQtyBadPerform);
                tirs.n_up_gsisa -= (nQtyGoodPerform > tirs.n_up_gsisa ? tirs.n_up_gsisa : nQtyGoodPerform);

                importRSes[nLoop] = tirs;

                if (nQtyUpld > 0)
                {
                  goto reRead;
                }
              }
            }

            #region Old Coded

            //if ((listRSD3 != null) && (listRSD3.Count > 0))
            //{
            //  db.LG_RSD3s.InsertAllOnSubmit(listRSD3.ToArray());

            //  listRSD3.Clear();
            //}

            #endregion
            
            if ((listTIRS != null) && (listTIRS.Count > 0) && createFakturBeliRetur)
            {
              faktur.FakturBeliReturAuto(db, User, listTIRS.ToArray());
            }

            db.SubmitChanges();

            if (db.Transaction != null)
            {
              db.Transaction.Commit();
              //db.Transaction.Rollback();
            }
          }
        }
      }
      catch (Exception ex)
      {
        result = string.Format("ScmsSoaLibrary.Modules.CommonQuerySP:SP_LG_CalcRS - {0}", ex.Message);

        Logger.WriteLine(result);
        Logger.WriteLine(ex.StackTrace);

        if (db.Transaction != null)
        {
          db.Transaction.Rollback();
        }
      }

      listRSNo.Clear();

      db.Dispose();

      return listReturn.ToArray();
    }

    public string[] SP_LG_CalcFB(string connectionString, string User, bool createFakturBeli, CommonUploadedQuery.UploadImportFB[] importFBs)
    {
      if (string.IsNullOrEmpty(User) || (importFBs == null) || (importFBs.Length < 1))
      {
        return new string[0];
      }

      string result = null;

      List<string> listReturn = new List<string>();
      List<CommonUploadedQuery.Temporary_ImportRS> listTIRS = new List<CommonUploadedQuery.Temporary_ImportRS>();

      ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);

      db.CommandTimeout = 0;
      db.Connection.Open();

      db.Transaction = db.Connection.BeginTransaction();

      ScmsSoaLibrary.Bussiness.Faktur faktur = new ScmsSoaLibrary.Bussiness.Faktur();

      int nLoop = 0,
        //nValue = 0,
        nLen = 0;

      string noRn = null,
             noRn2 = null, //Indra 20170705
        reslt = null;

      DateTime date = DateTime.Now;
      
      ScmsSoaLibrary.Parser.Class.FakturBeliStructure structure = null;
      List<ScmsSoaLibrary.Parser.Class.FakturBeliStructureField> lstFields = null;

      ScmsSoaLibrary.Modules.CommonUploadedQuery.UploadImportFB uifb = null;
      ScmsSoaLibraryInterface.Components.PostDataParser parser = new ScmsSoaLibraryInterface.Components.PostDataParser();
      ScmsSoaLibraryInterface.Components.PostDataParser.StructureResponse resp = default(ScmsSoaLibraryInterface.Components.PostDataParser.StructureResponse);

      try
      {
        for (nLoop = 0, nLen = importFBs.Length; nLoop < nLen; nLoop++)
        {
          uifb = importFBs[nLoop];

          noRn = uifb.RnNo;

          if (string.IsNullOrEmpty(noRn))
          {
            noRn = (from q in db.LG_RNHs
                    where (q.c_gdg == uifb.Gudang) && (q.c_from == uifb.Supplier)
                      && (q.c_dono == uifb.DoNo)
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                    select q.c_rnno).Take(1).SingleOrDefault();
          }

          if (string.IsNullOrEmpty(noRn))
          {
            continue;
          }

          noRn2 = (from q in db.LG_DOPHs
                  where (q.c_nosup == uifb.Supplier)
                    && (q.c_dono == uifb.DoNo)
                    && (q.c_po_outlet != null)
                    && (q.c_plphar != null)
                  select q.c_dono).Take(1).SingleOrDefault();

          if (string.IsNullOrEmpty(noRn2))
          {
              #region Reguler
              lstFields = (from q in db.LG_RNHs
                           join q1 in db.LG_RND2s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                           join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno into q_2
                           from qItm in q_2.DefaultIfEmpty()
                           join q3 in
                               (from sq in db.FA_DiscHes
                                join sq1 in db.FA_DiscDs on sq.c_nodisc equals sq1.c_nodisc
                                where (sq.c_type == "01")
                                select new
                                {
                                    sq1.c_iteno,
                                    sq1.n_discon
                                }).Distinct() on q1.c_iteno equals q3.c_iteno into q_3
                           from qDis in q_3.DefaultIfEmpty()
                           where (q.c_gdg == uifb.Gudang) && (q.c_rnno == noRn)
                           //&& (q1.c_type == "01")
                           // On Production && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                           group q1 by new { q1.c_iteno, qItm.n_salpri, qDis.n_discon } into g
                           select new ScmsSoaLibrary.Parser.Class.FakturBeliStructureField()
                           {
                               IsNew = true,
                               Name = "Sys",
                               Item = g.Key.c_iteno,
                               Bea = 0,
                               Discount = (g.Key.n_discon.HasValue ? g.Key.n_discon.Value : 0),
                               Harga = (g.Key.n_salpri.HasValue ? g.Key.n_salpri.Value : 0),
                               Quantity = g.Sum(t => ((t.n_gqty.HasValue ? t.n_gqty.Value : 0) + (t.n_bqty.HasValue ? t.n_bqty.Value : 0))),
                               TipeBarang = "01",
                               n_ppph = uifb.n_ppph,
                           }).Distinct().ToList();
              #endregion
          }
          else
          {
              #region Pharmanet
              lstFields = (from q in db.LG_RNHs
                           join q1 in db.LG_RND2s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                           join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno into q_2
                           from qItm in q_2.DefaultIfEmpty()
                           //join q3 in db.LG_DOPDs on qItm.c_iteno equals q3.c_iteno into q_3

                           join q3 in
                               (from sq in db.LG_DOPDs
                                join sq1 in db.LG_DOPHs on new { sq.c_dono, sq.c_po_outlet } equals new { sq1.c_dono, sq1.c_po_outlet }
                                where sq1.c_dono == noRn2
                                select new
                                {
                                    sq.c_iteno,
                                    sq.n_disc
                                } 
                                ).Distinct() on qItm.c_iteno equals q3.c_iteno into q_3


                           from qDis in q_3.DefaultIfEmpty()
                           where (q.c_gdg == uifb.Gudang) && (q.c_rnno == noRn)
                           group q1 by new { q1.c_iteno, qItm.n_salpri, qDis.n_disc } into g
                           select new ScmsSoaLibrary.Parser.Class.FakturBeliStructureField()
                           {
                               IsNew = true,
                               Name = "Sys",
                               Item = g.Key.c_iteno,
                               Bea = 0,
                               Discount = (g.Key.n_disc.HasValue ? g.Key.n_disc.Value : 0),
                               Harga = (g.Key.n_salpri.HasValue ? g.Key.n_salpri.Value : 0),
                               Quantity = g.Sum(t => ((t.n_gqty.HasValue ? t.n_gqty.Value : 0) + (t.n_bqty.HasValue ? t.n_bqty.Value : 0))),
                               TipeBarang = "01",
                               n_ppph = uifb.n_ppph,
                           }).Distinct().ToList();
              #endregion
          }

          if (lstFields.Count > 0)
          {
            structure = new ScmsSoaLibrary.Parser.Class.FakturBeliStructure()
            {
              Method = "Add",
              Fields = new ScmsSoaLibrary.Parser.Class.FakturBeliStructureFields()
              {
                Entry = User,
                Gudang = uifb.Gudang.ToString(),
                FakturPrincipal = uifb.Faktur,
                Kurs = "01",
                KursValue = 1,
                NoReceive = noRn,
                NoTax = uifb.Tax,
                Suplier = uifb.Supplier,
                TanggalFakturDate = uifb.FbDate,
                TanggalTaxDate = uifb.TaxDate,
                Top = uifb.Top,
                TopPjg = uifb.TopPjg,
                ValueFaktur = uifb.ValueFaktur,
                XDisc = uifb.XDisc,
                Field = lstFields.ToArray()
              }
            };

            lstFields.Clear();

            reslt = faktur.FakturBeli(structure, db);

            resp = parser.ResponseParser(reslt);

            if (resp.IsSet && (resp.Response == ScmsSoaLibraryInterface.Components.PostDataParser.ResponseStatus.Success))
            {
              listReturn.Add(noRn);
            }
          }
        }

        db.SubmitChanges();

        if (db.Transaction != null)
        {
          db.Transaction.Commit();
          //db.Transaction.Rollback();
        }
      }
      catch (Exception ex)
      {
        result = string.Format("ScmsSoaLibrary.Modules.CommonQuerySP:SP_LG_CalcFB - {0}", ex.Message);

        Logger.WriteLine(result);
        Logger.WriteLine(ex.StackTrace);

        if (db.Transaction != null)
        {
          db.Transaction.Rollback();
        }
      }

      //listRSNo.Clear();

      db.Dispose();

      return listReturn.ToArray();
    }

    public string[] ExpedisiUpload(string connectionString, string User, bool createFakturBeliRetur, CommonUploadedQuery.Temporary_ImportExp[] importRSes)
    {
      if ((importRSes == null) || (importRSes.Length < 1))
      {
        return new string[0];
      }

      string result = null;

      List<string> listReturn = new List<string>();
      List<CommonUploadedQuery.Temporary_ImportExp> listTIEXP = new List<CommonUploadedQuery.Temporary_ImportExp>();

      ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);

      db.Connection.Open();

      db.Transaction = db.Connection.BeginTransaction();

      List<string> listRSNo = importRSes.GroupBy(x => x.expno).Select(y => y.Key).ToList();
      CommonUploadedQuery.Temporary_ImportExp[] tirs = null;

      //Dictionary<string, int> dicRsNo = new Dictionary<string, int>();

      int nLoop = 0,
        nLen = 0;

      DateTime date = DateTime.Now;

      try
      {
        if ((listRSNo != null) && (listRSNo.Count > 0))
        {
          for (nLoop = 0, nLen = importRSes.Length; nLoop < nLen; nLoop++)
          {
            listTIEXP.Add(new CommonUploadedQuery.Temporary_ImportExp()
            {
              expno = importRSes[nLoop].expno,
              customer = importRSes[nLoop].customer,
              Dtglexpcab = importRSes[nLoop].Dtglexpcab,
              resino = importRSes[nLoop].resino,
              Dtglresi = importRSes[nLoop].Dtglresi,
              Twktexpcab = importRSes[nLoop].Twktexpcab
            });
            
          }
          if (listTIEXP.Count > 0)
          {
            ScmsSoaLibrary.Bussiness.Penjualan penjualan = new ScmsSoaLibrary.Bussiness.Penjualan();
            penjualan.EkspedisiCabangAuto(db, null, listTIEXP.ToArray());
          }
          if (db.Transaction != null)
          {
            db.Transaction.Commit();
            //db.Transaction.Rollback();
          }
        }
      }
      catch (Exception ex)
      {
        result = string.Format("ScmsSoaLibrary.Modules.CommonQuerySP:ExpedisiUpload - {0}", ex.Message);

        Logger.WriteLine(result);

        if (db.Transaction != null)
        {
          //db.Transaction.Commit();
          db.Transaction.Rollback();
        }
      }

      listRSNo.Clear();

      db.Dispose();

      return listReturn.ToArray();
    }
    
    public bool PostDataDO(string connectionString, string doNo, bool isStt)
    {
      ORMDataContext db = new ORMDataContext(connectionString);

      bool bOk = PostDataDODirect(db, doNo, isStt, true);

      db.Dispose();

      return bOk;
    }

    public bool PostDataMasterDivisiPrinsipal(string connectionString, string methodSend, FA_MsDivPri divPri)
    {
      ORMDataContext db = new ORMDataContext(connectionString);

      bool bResult = false;

      MSDivPriInformation mdpi = new MSDivPriInformation();

      mdpi.Method = methodSend;
      mdpi.DivPri = divPri;

      string dataResult =  ScmsSoaLibraryInterface.Components.StructureBase<MSDivPriInformation>.DeserializeJson(mdpi);

      Config cfg = Functionals.Configuration;

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

      Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.11.16/dist_core/?m=com.ams.json.ds&action=form&f=Submit&_q=mst_divisiprincipal");

      string result = null;

      bool isError = false;

      StringBuilder sb = new StringBuilder();

      Encoding utf8 = Encoding.UTF8;

      DateTime date = DateTime.MinValue;

      sb.AppendLine(uri.ToString());
      sb.AppendLine(dataResult);

      Logger.WriteLine(uri.ToString());

      if (pdc.PostGetData(uri, dicParam, dicHeader))
      {
        result = utf8.GetString(pdc.Result);

        bResult = true;
      }
      else
      {
        result = pdc.ErrorMessage;
        isError = true;
      }

      Logger.WriteLine(result);

      Bussiness.Commons.InsertReceivedRespose(db, sb.ToString(), result, isError, true, null, null);

      dic.Clear();

      dicHeader.Clear();
      dicParam.Clear();

      db.Dispose();

      return bResult;
    }

    public bool PostDataReplySP(string connectionString, string rawData, string spNo)
    {
      ORMDataContext db = new ORMDataContext(connectionString);

      bool bResult = false;

      Config cfg = Functionals.Configuration;

      Dictionary<string, string> dicParam = new Dictionary<string, string>();
      dicParam.Add("param", rawData);

      Dictionary<string, string> dicHeader = new Dictionary<string, string>();
      dicHeader.Add("X-Requested-With", "XMLHttpRequest");

      IDictionary<string, object> dic = new Dictionary<string, object>();

      ScmsSoaLibrary.Parser.ResponseParser res = new ScmsSoaLibrary.Parser.ResponseParser();

      ScmsSoaLibrary.Core.Response.DiscoreResponse ds = new ScmsSoaLibrary.Core.Response.DiscoreResponse();

      ScmsSoaLibrary.Parser.ParserDisCore pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

      pdc.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
      pdc.Referer = Functionals.DistCoreUrlBuilderString(cfg, "http://10.100.10.28/dcore/?m=com.ams.welcome");

      //Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.11.16/dist_core/?m=com.ams.json.ds&action=form&f=AutoLookup&_q=trx_sp_outstand");
      Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.28/dcore/?m=com.ams.json.ds&action=form&f=Submit&_q=update_outstand_sp");

      string result = null;

      bool isError = false;

      StringBuilder sb = new StringBuilder();

      Encoding utf8 = Encoding.UTF8;

      DateTime date = DateTime.MinValue;

      sb.AppendLine(uri.ToString());
      sb.AppendLine(rawData);

      Logger.WriteLine(uri.ToString());

      if (pdc.PostGetData(uri, dicParam, dicHeader))
      {
        result = utf8.GetString(pdc.Result);

        bResult = true;
      }
      else
      {
        result = pdc.ErrorMessage;
        isError = true;
      }

      Logger.WriteLine(result);

      //Bussiness.Commons.InsertReceivedRespose(db, sb.ToString(), result, isError, true, "SP", spNo);

      dic.Clear();

      dicHeader.Clear();
      dicParam.Clear();

      db.Dispose();

      return bResult;
    }

    public bool PostDataReplySPM(string connectionString, string rawData, string spNo)
    {
        ORMDataContext db = new ORMDataContext(connectionString);

        bool bResult = false;

        Config cfg = Functionals.Configuration;

        Dictionary<string, string> dicParam = new Dictionary<string, string>();
        dicParam.Add("param", rawData);

        Dictionary<string, string> dicHeader = new Dictionary<string, string>();
        dicHeader.Add("X-Requested-With", "XMLHttpRequest");

        IDictionary<string, object> dic = new Dictionary<string, object>();

        ScmsSoaLibrary.Parser.ResponseParser res = new ScmsSoaLibrary.Parser.ResponseParser();

        ScmsSoaLibrary.Core.Response.DiscoreResponse ds = new ScmsSoaLibrary.Core.Response.DiscoreResponse();

        ScmsSoaLibrary.Parser.ParserDisCore pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

        pdc.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
        pdc.Referer = Functionals.DistCoreUrlBuilderString(cfg, "http://10.100.10.28/dcore/?m=com.ams.welcome");

        Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.28/dcore/?m=com.ams.json.ds&action=form&f=Submit&_q=insert_spm");

        string result = null;

        bool isError = false;

        StringBuilder sb = new StringBuilder();

        Encoding utf8 = Encoding.UTF8;

        DateTime date = DateTime.MinValue;

        sb.AppendLine(uri.ToString());
        sb.AppendLine(rawData);

        Logger.WriteLine(uri.ToString());

        if (pdc.PostGetData(uri, dicParam, dicHeader))
        {
            result = utf8.GetString(pdc.Result);

            bResult = true;
        }
        else
        {
            result = pdc.ErrorMessage;
            isError = true;
        }

        Logger.WriteLine(result);

        Bussiness.Commons.InsertReceivedRespose(db, sb.ToString(), result, isError, true, "SP", spNo);

        dic.Clear();

        dicHeader.Clear();
        dicParam.Clear();

        db.Dispose();

        return bResult;
    }

    public bool PostDataReplySPETA(string connectionString, string rawData, string spNo)
    {
        ORMDataContext db = new ORMDataContext(connectionString);

        bool bResult = false;
        string cek = null;

        Config cfg = Functionals.Configuration;

        Dictionary<string, string> dicParam = new Dictionary<string, string>();
        dicParam.Add("param", rawData);

        Dictionary<string, string> dicHeader = new Dictionary<string, string>();
        dicHeader.Add("X-Requested-With", "XMLHttpRequest");

        IDictionary<string, object> dic = new Dictionary<string, object>();

        ScmsSoaLibrary.Parser.ResponseParser res = new ScmsSoaLibrary.Parser.ResponseParser();

        ScmsSoaLibrary.Core.Response.DiscoreResponse ds = new ScmsSoaLibrary.Core.Response.DiscoreResponse();

        ScmsSoaLibrary.Parser.ParserDisCore pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

        pdc.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
        pdc.Referer = Functionals.DistCoreUrlBuilderString(cfg, "http://10.100.10.28/dcore/?m=com.ams.welcome");

        Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.28/dcore/index.php?m=com.ams.json.ds&f=Submit&_q=trx_spi_change_eta");

        string result = null;

        bool isError = false;

        StringBuilder sb = new StringBuilder();

        Encoding utf8 = Encoding.UTF8;

        DateTime date = DateTime.MinValue;

        sb.AppendLine(uri.ToString());
        sb.AppendLine(rawData);

        Logger.WriteLine(uri.ToString());

        if (pdc.PostGetData(uri, dicParam, dicHeader))
        {
            result = utf8.GetString(pdc.Result);

            if (result.Contains("success"))
            {
                bResult = true;
                cek = "Success";
            }
            else if (!result.Contains("success"))
            {
                bResult = false;
            }
        }
        else
        {
            result = pdc.ErrorMessage;
            isError = true;
        }

        Logger.WriteLine(result);

        Bussiness.Commons.InsertReceivedRespose(db, sb.ToString(), result, isError, true, "SP", spNo);

        dic.Clear();

        dicHeader.Clear();
        dicParam.Clear();

        db.Dispose();

        return bResult;
    }

    public bool PostDataReplyItm(string connectionString, string rawData, string iteNo)
    {
        ORMDataContext db = new ORMDataContext(connectionString);

        bool bResult = false;

        Config cfg = Functionals.Configuration;

        Dictionary<string, string> dicParam = new Dictionary<string, string>();
        dicParam.Add("param", rawData);

        Dictionary<string, string> dicHeader = new Dictionary<string, string>();
        dicHeader.Add("X-Requested-With", "XMLHttpRequest");

        IDictionary<string, object> dic = new Dictionary<string, object>();

        ScmsSoaLibrary.Parser.ResponseParser res = new ScmsSoaLibrary.Parser.ResponseParser();

        ScmsSoaLibrary.Core.Response.DiscoreResponse ds = new ScmsSoaLibrary.Core.Response.DiscoreResponse();

        ScmsSoaLibrary.Parser.ParserDisCore pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

        pdc.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
        pdc.Referer = Functionals.DistCoreUrlBuilderString(cfg, "http://10.100.10.28/dcore/?m=com.ams.welcome");

        //Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.11.16/dist_core/?m=com.ams.json.ds&action=form&f=AutoLookup&_q=trx_sp_outstand");
        Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.28/dcore/?m=com.ams.json.ds&action=form&f=Submit&_q=update_master_box");

        string result = null;

        bool isError = false;

        StringBuilder sb = new StringBuilder();

        Encoding utf8 = Encoding.UTF8;

        DateTime date = DateTime.MinValue;

        sb.AppendLine(uri.ToString());
        sb.AppendLine(rawData);

        Logger.WriteLine(uri.ToString());

        if (pdc.PostGetData(uri, dicParam, dicHeader))
        {
            result = utf8.GetString(pdc.Result);

            bResult = true;
        }
        else
        {
            result = pdc.ErrorMessage;
            isError = true;
        }

        Logger.WriteLine(result);

        Bussiness.Commons.InsertReceivedRespose(db, sb.ToString(), result, isError, true, "ITM", iteNo);

        dic.Clear();

        dicHeader.Clear();
        dicParam.Clear();

        db.Dispose();

        return bResult;
    }

    public bool PostingSPZip(string connectionString, System.Data.DataSet ds, bool isContexted)
    {
      bool isOk = false;

      int nLoop = 0,
        nQty = 0,
        nLoopC = 0,
        nTotal = 0;
      

      List<LG_SPH> lstSP = null;
      LG_SPH sph = null;
      List<LG_SPD1> ListSPD1 = null;
      List<LG_SPD2> ListSPD2 = null;

      //LG_SPD2 spd2 = null;

      ListSPD1 = new List<LG_SPD1>();
      ListSPD2 = new List<LG_SPD2>();
      sph = new LG_SPH();

      DateTime date = DateTime.Today,
        spDate = DateTime.MinValue;

      System.Data.DataRow row = null,
         sRow = null;

      System.Data.DataTable table = null;
      string[] SPNo = null;
      string Type = null,
        spID = null,
        Item = null,
        kd = null,
        spCab = null, sCabang = null,
        SingleCabang = null;
      bool hasChanges = false;

      Bussiness.Pembelian beli = new ScmsSoaLibrary.Bussiness.Pembelian();
      ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);
      if (ds.Tables != null)
      {
        try
        {
          if (!isContexted)
          {
            

            db.CommandTimeout = 0;
            db.Connection.Open();

            db.Transaction = db.Connection.BeginTransaction();

            SysNo sysNum = (from q in db.SysNos
                            where q.c_portal == '3' && q.c_type == "07"
                              && q.s_tahun == DateTime.Now.Year
                            select q).SingleOrDefault();

            for (nLoopC = 0; nLoopC < ds.Tables.Count; nLoopC++)
            {
              table = ds.Tables[nLoopC];

              lstSP = new List<LG_SPH>();

              sRow = null;

              spID = GenerateNumbering<LG_SPH>(db, "SP", '3', "07", date, "c_spno");

              sRow = table.Rows[0];

              for (nLoop = 0; nLoop < table.Rows.Count; nLoop++)
              {
                row = table.Rows[nLoop];

                //nQty = row.GetValue<int>("n_adjust", 0) + row.GetValue<int>("n_order", 0);

                if (row.GetValue<string>("c_pareto").ToString().Substring(1, 1).Equals("M"))
                {
                  nQty = row.GetValue<int>("n_adjust", 0);
                  Type = "02";
                }
                else
                {
                  if (row.GetValue<int>("n_adjust", 0) < 0)
                    nQty = row.GetValue<int>("n_order", 0) + row.GetValue<int>("n_adjust", 0);
                  else
                    nQty = row.GetValue<int>("n_order", 0);

                  Type = "01";
                }

                if (nQty > 0)
                {
                  kd = row.GetValue<string>("c_kdcab", string.Empty);

                  var Cab = (from q in db.LG_Cusmas
                             where q.c_cab == kd
                             select q).SingleOrDefault();

                  DateTime Now = DateTime.Now;

                  SPNo = new string[] { row.GetValue<string>("c_nosp", string.Empty),
                                      row.GetValue<string>("c_pareto", string.Empty), Cab.c_cusno,
                                      row.GetValue<string>("d_tglsp", string.Empty)};

                  Item = row.GetValue<string>("c_iteno", string.Empty);

                  decimal nSalpri = (from q in db.FA_MasItms
                                     where q.c_iteno == Item
                                     select
                                       (q.n_salpri.HasValue ? q.n_salpri.Value : 0)
                                      ).Take(1).SingleOrDefault();

                  var Disc = (from q in db.FA_DiscDs
                              where q.c_iteno == Item && q.c_nodisc == "DSXXXXXX03"
                              select q).SingleOrDefault();

                  ListSPD1.Add(new LG_SPD1()
                  {
                    c_iteno = Item,
                    c_spno = spID,
                    c_type = "01",
                    n_acc = nQty,
                    n_qty = nQty,
                    n_sisa = nQty,
                    n_salpri = nSalpri,
                    v_ket = ""
                  });

                  ListSPD2.Add(new LG_SPD2()
                  {
                    c_iteno = Item,
                    c_spno = spID,
                    c_type = "03",
                    c_no = Disc == null ? string.Empty : Disc.c_nodisc,
                    n_discoff = Disc == null ? 0 : Disc.n_discoff,
                    n_discon = Disc == null ? 0 : Disc.n_discon,
                  });

                  spCab = row.GetValue<string>("c_nosp") == null ? string.Empty : row.GetValue<string>("c_nosp").ToString();
                }
              }

              if ((ListSPD1 != null) && (ListSPD1.Count > 0))
              {
                sCabang = sRow.ItemArray[0].ToString();

                spDate = DateTime.Parse(sRow.ItemArray[23].ToString());

                if (spDate != DateTime.MinValue)
                {
                  SingleCabang = (from q in db.LG_Cusmas
                                  where q.c_cab == sCabang
                                  select q.c_cusno).Take(1).SingleOrDefault();

                  lstSP = (from q in db.LG_SPHs
                           where q.c_sp == spCab && q.c_cusno == SingleCabang
                           select q).ToList();

                  if (lstSP != null && lstSP.Count <= 0)
                  {
                    sph = new LG_SPH()
                    {
                      c_sp = spCab,
                      c_type = Type,
                      c_cusno = SingleCabang,
                      c_entry = "sysup",
                      c_spno = spID,
                      c_update = "sysup",
                      d_entry = DateTime.Now,
                      d_spdate = spDate,
                      d_spinsert = DateTime.Now,
                      d_update = DateTime.Now,
                      l_cek = false,
                      l_delete = false,
                      l_print = false,
                      v_ket = ""
                    };

                    nTotal++;
                  }
                }
              }

              if (nTotal > 0)
              {
                if (ListSPD1.Count > 0)
                {
                  db.LG_SPD1s.InsertAllOnSubmit(ListSPD1.ToArray());
                  ListSPD1.Clear();
                }
                if (ListSPD2.Count > 0)
                {
                  db.LG_SPD2s.InsertAllOnSubmit(ListSPD2.ToArray());
                  ListSPD2.Clear();
                }
                if (sph != null)
                {
                  db.LG_SPHs.InsertOnSubmit(sph);

                  if (!isContexted)
                  {
                    string tmpNum = spID.Substring(6, 4);
                    switch (DateTime.Now.Month)
                    {
                      case 1: sysNum.c_bln01 = tmpNum; break;
                      case 2: sysNum.c_bln02 = tmpNum; break;
                      case 3: sysNum.c_bln03 = tmpNum; break;
                      case 4: sysNum.c_bln04 = tmpNum; break;
                      case 5: sysNum.c_bln05 = tmpNum; break;
                      case 6: sysNum.c_bln06 = tmpNum; break;
                      case 7: sysNum.c_bln07 = tmpNum; break;
                      case 8: sysNum.c_bln08 = tmpNum; break;
                      case 9: sysNum.c_bln09 = tmpNum; break;
                      case 10: sysNum.c_bln10 = tmpNum; break;
                      case 11: sysNum.c_bln11 = tmpNum; break;
                      case 12: sysNum.c_bln12 = tmpNum; break;
                    }

                    db.SubmitChanges();
                    
                  }
                  else
                  {
                    db.Transaction.Rollback();
                  }

                  //db.SubmitChanges();
                  //db.Dispose();
                }

              }
              else
              {
                if (ListSPD1 != null && ListSPD1.Count > 0)
                {
                  ListSPD1.Clear();
                }

                if (ListSPD2 != null && ListSPD2.Count > 0)
                {
                  ListSPD2.Clear();
                }
              }
            }

            db.Transaction.Commit();
            db.Dispose();
          }
        }
        catch (Exception ex)
        {
          if (db.Transaction != null)
          {
            db.Transaction.Rollback();
          }
        }
      }

      return isOk;
    }

    public string PostingExcellVH(string connectionString, System.Data.DataSet ds, bool isContexted)
    {
        string message = string.Empty;

        int nLoop = 0,
          nLoopC = 0,
          nTotal = 0;

        List<LG_VDNH> ListVdnh = null;
        List<LG_VDNH> ListVdnhSum = null;
        List<LG_DNH> ListDnh = null;
        List<LG_DNH> ListDnhSum = null;
        List<LG_DND> ListDnd = null;

        ListVdnh = new List<LG_VDNH>();
        ListVdnhSum = new List<LG_VDNH>();
        ListDnh = new List<LG_DNH>();
        ListDnhSum = new List<LG_DNH>();
        ListDnd = new List<LG_DND>();

        DateTime date = DateTime.Today;
            //date1 = DateTime.MinValue,
            //date2 = DateTime.MinValue;

        System.Data.DataRow row = null;

        System.Data.DataTable table = null;
        string noSup = null,
          tipe = null,
          vdNo1 = null,
          vdNo2 = null,
          noSup1 = null,
          noSup2 = null;
        //bool hasChanges = false;

        Bussiness.Pembelian beli = new ScmsSoaLibrary.Bussiness.Pembelian();
        ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);
        if (ds.Tables != null)
        {
            try
            {
                if (!isContexted)
                {
                    db.CommandTimeout = 0;
                    db.Connection.Open();
                    db.Transaction = db.Connection.BeginTransaction();

                    for (nLoopC = 0; nLoopC < ds.Tables.Count; nLoopC++)
                    {
                        table = ds.Tables[nLoopC];
                        for (nLoop = 0; nLoop < table.Rows.Count; nLoop++)
                        {
                            row = table.Rows[nLoop];

                            date = row.GetValue<DateTime>("d_date", DateTime.MinValue);
                            if (date.Equals(DateTime.MinValue))
                            {
                                var tmp = row.GetValue<string>("d_date", string.Empty);
                                if (!Functionals.DateParser(tmp, "M/d/yyyy HH:mm:ss", out date))
                                {
                                    date = Functionals.StandardSqlDateTime;
                                }
                            }

                            var fbNo = row.GetValue<string>("c_fbno", string.Empty).Trim();
                            if (!string.IsNullOrEmpty(fbNo))
                            {
                                if (fbNo.Substring(0, 2).ToUpper() == "FB")
                                {
                                    noSup = (from q in db.LG_FBHs
                                             where q.c_fbno == fbNo
                                             select q.c_nosup).Take(1).SingleOrDefault();
                                    tipe = "01";
                                }
                                else
                                {
                                    noSup = (from q in db.LG_FBRHs
                                             where q.c_fbno == fbNo
                                             select q.c_nosup).Take(1).SingleOrDefault();
                                    tipe = "02";
                                }
                            }

                            var vdNo = (from q in db.LG_VDNHs
                                        where q.c_vdno == row.GetValue<string>("c_vdno", string.Empty).Trim()
                                        select q.c_vdno).Take(1).SingleOrDefault();

                            if (string.IsNullOrEmpty(vdNo))
                            {
                                var noRek = (from q in db.FA_MsBanks
                                             join q1 in db.FA_MsBankReks on q.c_bank equals q1.c_bank
                                             where q.c_bank == row.GetValue<string>("c_bank", string.Empty).Trim()
                                             && q1.c_type == "02"
                                             select q1.c_rekno).Take(1).SingleOrDefault();

                                ListVdnh.Add(new LG_VDNH()
                                {
                                    c_vdno = row.GetValue<string>("c_vdno", string.Empty).Trim(),
                                    d_vddate = date,
                                    c_nosup = noSup,
                                    c_bank = row.GetValue<string>("c_bank", string.Empty).Trim(),
                                    c_rekNo = noRek,
                                    n_bilva = row.GetValue<decimal>("n_value", 0),
                                    n_sisa = row.GetValue<decimal>("n_value", 0)
                                });
                            }

                            var noteNo = (from q in db.LG_DNHs
                                          where q.c_noteno == row.GetValue<string>("c_noteno", string.Empty).Trim()
                                          select q.c_noteno).Take(1).SingleOrDefault();

                            if (string.IsNullOrEmpty(noteNo))
                            {
                                ListDnh.Add(new LG_DNH()
                                {
                                    c_noteno = row.GetValue<string>("c_noteno", string.Empty).Trim(),
                                    d_notedate = date,
                                    c_nosup = noSup,
                                    n_bilva = 0,
                                    n_diff = 0
                                });
                            }

                            noteNo = (from q in db.LG_DNDs
                                      where q.c_noteno == row.GetValue<string>("c_noteno", string.Empty).Trim()
                                        && q.c_fbno == fbNo
                                        && q.c_type == tipe
                                        && q.c_vdno == row.GetValue<string>("c_vdno", string.Empty).Trim()
                                      select q.c_noteno).Take(1).SingleOrDefault();

                            if (string.IsNullOrEmpty(noteNo))
                            {
                                ListDnd.Add(new LG_DND()
                                {
                                    c_noteno = row.GetValue<string>("c_noteno", string.Empty).Trim(),
                                    c_fbno = fbNo,
                                    c_type = tipe,
                                    c_vdno = row.GetValue<string>("c_vdno", string.Empty).Trim(),
                                    c_trans = "00",
                                    n_value = row.GetValue<decimal>("n_value", 0)
                                });
                            }
                        }
                        
                        if (ListVdnh != null && ListVdnh.Count > 0)
                        {
                            ListVdnhSum = ListVdnh.GroupBy(x => new { x.c_vdno, x.d_vddate, x.c_nosup, x.c_bank, x.c_rekNo })
                                        .Select(x => new LG_VDNH()
                                         {
                                             c_vdno = x.Key.c_vdno,
                                             d_vddate = x.Key.d_vddate.Value,
                                             c_type = "01",
                                             c_nosup = x.Key.c_nosup,
                                             c_bank = x.Key.c_bank,
                                             c_rekNo = x.Key.c_rekNo,
                                             c_kurs = "01",
                                             n_kurs = 1,
                                             n_bilva = x.Sum(y => (y.n_bilva.HasValue ? y.n_bilva.Value : 0)),
                                             n_sisa = x.Sum(y => (y.n_sisa.HasValue ? y.n_sisa.Value : 0)),
                                             n_admin = 0,
                                             l_um = false,
                                             l_print = false,
                                             c_entry = "system",
                                             d_entry = DateTime.Now,
                                             c_update = "system",
                                             d_update = DateTime.Now
                                         }).ToList();

                            ListVdnh.Clear();
                        }

                        if (ListDnh != null && ListDnh.Count > 0)
                        {
                            ListDnhSum = ListDnh.GroupBy(x => new { x.c_noteno, x.d_notedate, x.c_nosup })
                                       .Select(x => new LG_DNH()
                                       {
                                           c_noteno = x.Key.c_noteno,
                                           d_notedate = x.Key.d_notedate.Value,
                                           c_nosup = x.Key.c_nosup,
                                           c_type = "01",
                                           c_kurs = "01",
                                           n_kurs = 1,
                                           n_bilva = x.Sum(y => (y.n_bilva.HasValue ? y.n_bilva.Value : 0)),
                                           n_diff = x.Sum(y => (y.n_diff.HasValue ? y.n_diff.Value : 0)),
                                           l_print = false,
                                           c_entry = "system",
                                           d_entry = DateTime.Now,
                                           c_update = "system",
                                           d_update = DateTime.Now
                                       }).ToList();

                            ListDnh.Clear();
                        }

                        if (ListVdnhSum != null && ListVdnhSum.Count > 0)
                        {
                            for (nLoop = 0; nLoop < ListVdnhSum.Count; nLoop++)
                            {
                                noSup1 = ListVdnhSum[nLoop].c_nosup;
                                vdNo1 = ListVdnhSum[nLoop].c_vdno;
                                //date1 =  ListVdnhSum[nLoop].d_vddate.Value;

                                //if (vdNo1 == "LD15080079")
                                //{
                                //    var tes = "";
                                //}

                                if (noSup1 != noSup2 && vdNo1 == vdNo2)
                                {
                                    ListVdnhSum.Clear();

                                    db.Transaction.Rollback();
                                    message = "Duplicate VDNO : " + vdNo1 + " Supllier : " + noSup1 + " dan " + noSup2 ;
                                    return message;
                                }

                                if(vdNo1 == vdNo2)
                                {
                                    ListVdnhSum.Clear();

                                    db.Transaction.Rollback();
                                    message = "Please Check Duplicate data VDNO : " + vdNo1;
                                    return message;
                                }

                                if (nLoop > 0)
                                {
                                    noSup2 = ListVdnhSum[nLoop].c_nosup;
                                    vdNo2 = ListVdnhSum[nLoop].c_vdno;
                                    //date2 = ListVdnhSum[nLoop].d_vddate.Value;
                                }
                            }

                            db.LG_VDNHs.InsertAllOnSubmit(ListVdnhSum.ToArray());
                            ListVdnhSum.Clear();
                            nTotal++;
                        }

                        if (ListDnhSum != null && ListDnhSum.Count > 0)
                        {
                            db.LG_DNHs.InsertAllOnSubmit(ListDnhSum.ToArray());
                            ListDnhSum.Clear();
                            nTotal++;
                        }

                        if (ListDnd != null && ListDnd.Count > 0)
                        {
                            db.LG_DNDs.InsertAllOnSubmit(ListDnd.ToArray());
                            ListDnd.Clear();
                            nTotal++;
                        }
                    }
                    db.SubmitChanges();
                    db.Transaction.Commit();
                    db.Dispose();
                    if (nTotal > 0)
                    {
                        message = "Upload data berhasil tersimpan";
                    }
                    else
                    {
                        message = "Data sudah pernah di upload. Mohon periksa kembali";
                    }
                }
                else
                {
                    db.Transaction.Rollback();
                    message = "Conection TimeOut. Please try again.";
                }
            }
            catch (Exception ex)
            {
                if (db.Transaction != null)
                {
                    db.Transaction.Rollback();
                }
                message = ex.Message;
            }
        }
        return message;
    }

    public string PostingExcellSO(string connectionString, System.Data.DataSet ds, bool isContexted, int year, int month, string tipe, string nipEntry)
    {
        string message = string.Empty;

        int nLoop = 0,
          nLoopC = 0,
          nTotal = 0;

        List<SCMS_SO> ListSoIns = null;
        List<SCMS_SO> ListSoInsSum = null;
        List<SCMS_SO> ListSoDel = null;

        ListSoIns = new List<SCMS_SO>();
        ListSoInsSum = new List<SCMS_SO>();
        ListSoDel = new List<SCMS_SO>();
        
        DateTime date = DateTime.Today;

        System.Data.DataRow row = null;

        System.Data.DataTable table = null;
        //bool hasChanges = false;
        string ed = null, 
            tahun = null,
            iteno = null;

        ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);
        if (ds.Tables != null)
        {
            try
            {
                if (!isContexted)
                {
                    db.CommandTimeout = 0;
                    db.Connection.Open();
                    db.Transaction = db.Connection.BeginTransaction();

                    ListSoDel = (from q in db.SCMS_SOs
                                where q.s_tahun == year
                                    && q.t_bulan == month
                                    && q.c_type == tipe
                                select q).ToList();

                    for (nLoopC = 0; nLoopC < ds.Tables.Count; nLoopC++)
                    {
                        table = ds.Tables[nLoopC];
                        for (nLoop = 0; nLoop < table.Rows.Count; nLoop++)
                        {
                            row = table.Rows[nLoop];

                            iteno = row.GetValue<string>("C_ITENO", string.Empty).Trim();
                            if (string.IsNullOrEmpty(iteno))
                            {
                                continue;
                            }

                            ed = row.GetValue<string>("ED", string.Empty).Trim();

                            if (string.IsNullOrEmpty(ed))
                            {
                                date = new DateTime(2099, 12, 1);
                            }
                            else
                            {
                                if (ed.Length == 5)
                                {
                                    if (int.Parse(ed.Substring(0, 2)) > 12)
                                    {
                                        date = new DateTime(int.Parse(tahun), 12, 1);
                                    }
                                    else
                                    {
                                        tahun = "20" + int.Parse(ed.Substring(3, 2));
                                        date = new DateTime(int.Parse(tahun), int.Parse(ed.Substring(0, 2)), 1);
                                    }
                                }
                                else
                                {
                                    date = new DateTime(2099, 12, 1);
                                }
                            }

                            ListSoIns.Add(new SCMS_SO()
                            {
                                    s_tahun = year,
                                    t_bulan = month,
                                    c_type = tipe,
                                    c_team = row.GetValue<string>("TEAM", string.Empty).Trim(),
                                    c_nosup = row.GetValue<string>("C_NOSUP", string.Empty).Trim(),
                                    c_iteno = iteno,
                                    n_qty = row.GetValue<decimal>("QTY", 0),
                                    c_batch = row.GetValue<string>("BATCH", string.Empty).Trim(),
                                    d_expired = date
                            });
                        }

                        if ((ListSoDel != null) && (ListSoDel.Count > 0))
                        {
                            db.SCMS_SOs.DeleteAllOnSubmit(ListSoDel.ToArray());
                            ListSoDel.Clear();
                        }

                        if (ListSoIns != null && ListSoIns.Count > 0)
                        {
                            ListSoInsSum = ListSoIns.GroupBy(x => new { x.s_tahun, x.t_bulan, x.c_type, x.c_team, x.c_iteno, x.c_batch, x.c_nosup, x.d_expired })
                                        .Select(x => new SCMS_SO()
                                        {
                                            s_tahun = x.Key.s_tahun,
                                            t_bulan = x.Key.t_bulan,
                                            c_type = x.Key.c_type,
                                            c_team = x.Key.c_team,
                                            c_nosup = x.Key.c_nosup,
                                            c_iteno = x.Key.c_iteno,
                                            n_qty = x.Sum(y => (y.n_qty.HasValue ? y.n_qty.Value : 0)),
                                            c_batch = x.Key.c_batch,
                                            d_expired = x.Key.d_expired,
                                            c_entry = nipEntry,
                                            d_entry = DateTime.Now
                                        }).ToList();

                            ListSoIns.Clear();
                        }

                        if (ListSoInsSum != null && ListSoInsSum.Count > 0)
                        {
                            db.SCMS_SOs.InsertAllOnSubmit(ListSoInsSum.ToArray());
                            ListSoInsSum.Clear();
                            nTotal++;
                        }

                        db.SubmitChanges();
                        db.Transaction.Commit();
                        db.Dispose();
                        if (nTotal > 0)
                        {
                            message = "Upload data berhasil tersimpan";
                        }
                    }
                }
                else
                {
                    db.Transaction.Rollback();
                    message = "Conection TimeOut. Please try again.";
                }
            }
            catch (Exception ex)
            {
                if (db.Transaction != null)
                {
                    db.Transaction.Rollback();
                }
                message = ex.Message;
            }
        }
        return message;
    }
    //Indra D.
    public string PostingExcellUSER(string connectionString, System.Data.DataSet ds, bool isContexted)
        {
            string message = string.Empty;

            int nLoop = 0,
              nLoopC = 0,
              nTotal = 0;

            List<SCMS_USER> ListUser = null;

            ListUser = new List<SCMS_USER>();

            DateTime date = DateTime.Today;
            //date1 = DateTime.MinValue,
            //date2 = DateTime.MinValue;

            System.Data.DataRow row = null;

            System.Data.DataTable table = null;

            string sNip = null,
            passWd = null,
            strongKey = null,
            sNama = null,
            sGudang = null,
            sAktif = null,
            tmp = null,
            DupName = null,
            sNipEntry = null;

            string result = null;

            Bussiness.Pembelian beli = new ScmsSoaLibrary.Bussiness.Pembelian();
            ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            MyAssembly myAsm = new MyAssembly();
            if (ds.Tables != null)
            {
                try
                {
                    if (!isContexted)
                    {
                        db.CommandTimeout = 0;
                        db.Connection.Open();
                        db.Transaction = db.Connection.BeginTransaction();

                        for (nLoopC = 0; nLoopC < ds.Tables.Count; nLoopC++)
                        {
                            nTotal = 0;
                            table = ds.Tables[nLoopC];
                            for (nLoop = 0; nLoop < table.Rows.Count; nLoop++)
                            {
                                row = table.Rows[nLoop];

                                strongKey = GlobalCrypto.Crypt1WayMD5String(string.Concat((string.IsNullOrEmpty(sNip) ? "Mochamad Rudi" : sNip), DateTime.Now.ToString("yyyyMMddHHmmssfff")));

                                sNipEntry = row.GetValue<string>("nipEntry", string.Empty); 
                                sNip = row.GetValue<string>("Nip", string.Empty);
                                sNama = row.GetValue<string>("Nama", string.Empty);
                                passWd = row.GetValue<string>("Tgl_Lahir", string.Empty);
                                sGudang = row.GetValue<string>("Gudang", string.Empty);
                                sAktif = row.GetValue<string>("Aktif", string.Empty);

                                if (string.IsNullOrEmpty(sNip))
                                {
                                    result = "Nip dibutuhkan.";

                                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                    break;
                                }

                                if (string.IsNullOrEmpty(sNama))
                                {
                                    result = "Nama dibutuhkan.";

                                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                    break;
                                }

                                if (string.IsNullOrEmpty(passWd))
                                {
                                    result = "Tanggal lahir dibutuhkan.";

                                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                    break;
                                }

                                if (string.IsNullOrEmpty(sGudang))
                                {
                                    result = "Gudang; dibutuhkan.";

                                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                    break;
                                }

                                if (string.IsNullOrEmpty(sAktif))
                                {
                                    result = "Status user dibutuhkan.";

                                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                    break;
                                }
                                // Check Duplikat Nip
                                var user = (from q in db.SCMS_USERs
                                              where q.c_nip == sNip
                                              select q.c_nip).Take(1).SingleOrDefault();


                                if (string.IsNullOrEmpty(user))
                                {
                                    ListUser.Add(new SCMS_USER()
                                    {
                                        c_nip = sNip,
                                        c_gdg = "1",
                                        l_aktif = true,
                                        v_username = sNama,
                                        c_entry = sNipEntry,
                                        d_entry = date,
                                        x_hash = strongKey,
                                        v_password = Functionals.CryptHashRjdnl(strongKey, passWd)
                                    });

                                    nTotal = nTotal + 1;
                                }

                                else
                                {
                                    DupName = DupName + sNip + " " + sNama + "; ";
                                }

                                db.SCMS_USERs.InsertAllOnSubmit(ListUser.ToArray());
                                ListUser.Clear();
                            }
                        }

                        db.SubmitChanges();

                        if (result == null)
                        {
                            if (DupName == null)
                            {
                                db.Transaction.Commit();
                                message = "Upload data berhasil tersimpan";
                            }

                            else
                            {
                                db.Transaction.Rollback();
                                message = DupName + "\n" + "Sudah pernah diupload. Upload data dibatalkan";
                            }
                        }
                        else
                        {
                            db.Transaction.Rollback();
                            nLoop = nLoop + 1;
                            message = "Kesalahan pada baris " + nLoop + " " + result + ". Upload data dibatalkan";
                        }
                        db.Dispose();

                    }

                    else
                    {
                        db.Transaction.Rollback();
                        message = "Conection TimeOut. Please try again.";
                    }
                }
                catch (Exception ex)
                {
                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }
                    message = ex.Message;
                }
            }
            return message;
        }
    //Indra D. 20170714
    public string PostingExcellResendDOBatch(string connectionString, System.Data.DataSet ds, bool isContexted)
    {
        string message = string.Empty;

        int nLoop = 0,
          nLoopC = 0,
          nTotal = 0;

        LG_DOH doh = null;

        DateTime date = DateTime.Today;

        System.Data.DataRow row = null;

        System.Data.DataTable table = null;

        string No = null, NO_DO = null;

        string result = null;

        Bussiness.Pembelian beli = new ScmsSoaLibrary.Bussiness.Pembelian();
        ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);
        ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
        MyAssembly myAsm = new MyAssembly();
        if (ds.Tables != null)
        {
            try
            {
                if (!isContexted)
                {
                    for (nLoopC = 0; nLoopC < ds.Tables.Count; nLoopC++)
                    {

                        nTotal = 0;
                        table = ds.Tables[nLoopC];
                        for (nLoop = 0; nLoop < table.Rows.Count; nLoop++)
                        {
                            row = table.Rows[nLoop];

                            NO_DO = row.GetValue<string>("NO_DO", string.Empty);

                            if (NO_DO == "100")
                            {
                                result = "No. DO Dibutuhkan";
                            }

                            if (NO_DO == "200")
                            {
                                result = "No. DO Dibutuhkan";
                            }

                            if (NO_DO == "500")
                            {
                                result = "No. DO Dibutuhkan";
                            }

                            if (NO_DO == "1000")
                            {
                                result = "No. DO Dibutuhkan";
                            }

                            if (No == "1500")
                            {
                                result = "No. DO Dibutuhkan";
                            }


                            doh = (from q in db.LG_DOHs
                            where (q.c_dono == NO_DO)
                            select q).Take(1).SingleOrDefault();

                            if (doh != null)
                            {
                                if (doh.c_rnno == null)
                                {
                                    Modules.CommonQuerySP sp = new ScmsSoaLibrary.Modules.CommonQuerySP();
                                    sp.PostDataDO(db.Connection.ConnectionString, NO_DO, false);

                                    for (int aTime = 0; aTime <= 10000; aTime++)
                                    {
                                        result = "Need Take Rest Guys";
                                    }
                                }
                            }

                            result = "Sukses";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }
        return message;
    }
  }
}
