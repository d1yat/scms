using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsModel;
using ScmsSoaLibrary.Parser;
//using ScmsSoaLibrary.Core.Crypto;
using ScmsSoaLibrary.Commons;
using System.Data.Common;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaLibrary.Bussiness
{
  class Peminjaman
  {
    //private bool HasDO(ORMDataContext db, string sttNO)
    //{
    //  string tmp = (from q in db.LG_DOHs
    //                where q.c_plno == sttNO && q.l_delete == false
    //                select q.c_dono).Take(1).SingleOrDefault();

    //  return (!string.IsNullOrEmpty(tmp));
    //}

    public string STT(ScmsSoaLibrary.Parser.Class.STTStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      LG_STH sth = null;

      MK_MTD mtd = null;

      LG_RND1 rnd1 = null;
      LG_ComboH comboh = null;

      ScmsSoaLibrary.Parser.Class.STTStructureField field = null;
      string nipEntry = null;
      string sttID = null;
      //string tmpNumbering = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      decimal mkQtyReloc = 0,
        rnQty = 0, rnSisa = 0, rnAvaible = 0,
         mkQty = 0;

      List<LG_STD1> listStd1 = null;
      List<LG_STD2> listStd2 = null;
      List<LG_STD2> listStd2Copy = null;
      //List<string> listRN = null;
      List<LG_RND1> listResRND1 = null;
      List<LG_ComboH> listResComboh = null;
      List<LG_STD3> listStd3 = null;

      List<MK_MTD> listMTD = null;

      char Gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '1' : structure.Fields.Gudang[0]);

      LG_STD1 std1 = null;
      LG_STD2 std2 = null;

      int nLoop = 0,
        nLoopC = 0;

      IDictionary<string, string> dic = null;

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }
      int totalDetails = 0;
      bool hasAnyChanges = false;

      sttID = (structure.Fields.STTid ?? string.Empty);

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          if (!string.IsNullOrEmpty(sttID))
          {
            result = "Nomor STT harus kosong.";

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Gudang))
          {
            result = "Gudang dibutuhkan.";

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, date))
          {
            result = "STT tidak dapat disimpan, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          sttID = Commons.GenerateNumbering<LG_STH>(db, "ST", '3', "21", date, "c_stno");

          sth = new LG_STH()
          {
            c_entry = nipEntry,
            c_gdg = Gudang,
            c_mtno = structure.Fields.Memo,
            c_stno = sttID,
            c_type = structure.Fields.TipeTransaksi,
            c_update = nipEntry,
            d_entry = DateTime.Now,
            d_stdate = date.Date,
            d_update = DateTime.Now,
            l_delete = false,
            l_print = false,
            l_status = false,
            v_ket = structure.Fields.Keterangan
          };

          db.LG_STHs.InsertOnSubmit(sth);

          #region Old Code

          //db.SubmitChanges();

          //sth = (from q in db.LG_STHs
          //       where q.v_ket == tmpNumbering
          //       select q).Take(1).SingleOrDefault();

          //if (!string.IsNullOrEmpty(sttID))
          //{
          //  result = "No STT tidak dapat di raih";

          //  rpe = ResponseParser.ResponseParserEnum.IsError;

          //  goto endLogic;
          //}

          //if (sth.c_stno.Equals("XXXXXXXXXX"))
          //{
          //  result = "Trigger STT tidak aktif.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  goto endLogic;
          //}

          //sth.v_ket = structure.Fields.Keterangan;

          //sttID = sth.c_stno;

          //db.SubmitChanges();

          #endregion

          #region Insert Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            //listRN = new List<string>();
            listStd1 = new List<LG_STD1>();
            listStd2 = new List<LG_STD2>();
            
            listMTD = (from q in db.MK_MTHs
                       join q1 in db.MK_MTDs on new { q.c_gdg, q.c_mtno } equals new { q1.c_gdg, q1.c_mtno }
                       where (q.c_gdg == Gudang) && (q1.c_mtno == structure.Fields.Memo)
                        && (q.c_type == structure.Fields.TipeTransaksi)
                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                       select q1).Distinct().ToList();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0))
              {
                #region Cek MTD

                //mtd = (from q in db.MK_MTDs
                //       join q1 in db.MK_MTHs on new { q.c_gdg, q.c_mtno } equals new { q1.c_gdg, q1.c_mtno }
                //       where q.c_gdg == Gudang && q1.c_mtno == structure.Fields.Memo && q1.c_type == structure.Fields.TipeTransaksi
                //       select q).Distinct().Take(1).SingleOrDefault();

                mtd = listMTD.Find(delegate(MK_MTD mkMTD)
                {
                  mkQty = (mkMTD.n_sisa.HasValue ? mkMTD.n_sisa.Value : 0);

                  return field.Item.Equals((string.IsNullOrEmpty(mkMTD.c_iteno) ? string.Empty : mkMTD.c_iteno), StringComparison.OrdinalIgnoreCase) &&
                    (mkQty > 0);
                });

                if (mtd != null)
                {
                  mkQty = (mtd.n_sisa.HasValue ? mtd.n_sisa.Value : 0);

                  mkQtyReloc = mkQty =
                    (field.Quantity > mkQty ? mkQty : field.Quantity);

                  //if (field.Quantity > mtd.n_sisa.Value)
                  //{
                  //  mkQty = mtd.n_sisa.Value;
                  //}
                  //else
                  //{
                  //  mkQty = field.Quantity;
                  //}

                  //mkQtyReloc = mkQty.Value;

                  listResRND1 = (from q in db.LG_RND1s
                                 join q1 in db.LG_RNHs on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                 join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                 from qBat in q_2.DefaultIfEmpty()
                                 where q.c_gdg == Gudang && q.c_batch == field.Batch && q.c_iteno == field.Item && q.n_gsisa > 0
                                 orderby qBat.d_expired
                                 select q).Distinct().ToList();

                  #region Cek RN

                  if (listResRND1.Count > 0)
                  {
                      rnQty = 0;

                      for (nLoopC = 0; nLoopC < listResRND1.Count; nLoopC++)
                      {
                          rnd1 = listResRND1[nLoopC];

                          rnSisa = (rnd1.n_gsisa.HasValue ? rnd1.n_gsisa.Value : 0);

                          if (rnSisa > 0)
                          {
                              if (rnSisa >= mkQtyReloc)
                              {
                                  rnAvaible = mkQtyReloc;
                                  rnQty += mkQtyReloc;

                                  rnd1.n_gsisa -= mkQtyReloc;

                                  mkQtyReloc = 0;
                              }
                              else
                              {
                                  rnAvaible = rnSisa;
                                  rnQty += rnSisa;

                                  mkQtyReloc -= rnd1.n_gsisa.Value;

                                  rnd1.n_gsisa = 0;
                              }

                              listStd2.Add(new LG_STD2()
                                {
                                    c_batch = field.Batch,
                                    c_gdg = Gudang,
                                    c_iteno = field.Item,
                                    c_no = rnd1.c_rnno,
                                    c_stno = sttID,
                                    n_qty = rnAvaible
                                });
                          }

                          if (mkQtyReloc < 1)
                          {
                              break;
                          }
                      }

                      #region Jika dalam batch yang sama terdapat di RN & Combo

                      if (mkQtyReloc > 0)
                      {
                          listResComboh = (from q in db.LG_ComboHs
                                       join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                       from qBat in q_2.DefaultIfEmpty()
                                       where q.c_gdg == Gudang && q.c_batch == field.Batch && q.c_iteno == field.Item && q.n_gsisa > 0
                                       orderby qBat.d_expired
                                       select q).Distinct().ToList();

                          if (listResComboh.Count > 0)
                          {
                              for (nLoopC = 0; nLoopC < listResComboh.Count; nLoopC++)
                              {
                                  comboh = listResComboh[nLoopC];

                                  rnSisa = (comboh.n_gsisa.HasValue ? comboh.n_gsisa.Value : 0);

                                  if (rnSisa > 0)
                                  {
                                      if (rnSisa >= mkQtyReloc)
                                      {
                                          rnAvaible = mkQtyReloc;
                                          rnQty += mkQtyReloc;

                                          comboh.n_gsisa -= mkQtyReloc;

                                          mkQtyReloc = 0;
                                      }
                                      else
                                      {
                                          rnAvaible = rnSisa;
                                          rnQty += rnSisa;

                                          mkQtyReloc -= comboh.n_gsisa.Value;

                                          comboh.n_gsisa = 0;
                                      }

                                      listStd2.Add(new LG_STD2()
                                      {
                                          c_batch = field.Batch,
                                          c_gdg = Gudang,
                                          c_iteno = field.Item,
                                          c_no = comboh.c_combono,
                                          c_stno = sttID,
                                          n_qty = rnAvaible
                                      });
                                  }

                                  if (mkQtyReloc < 1)
                                  {
                                      break;
                                  }
                              }
                          }
                      }

                      #endregion

                      listStd1.Add(new LG_STD1()
                        {
                            c_batch = field.Batch,
                            c_gdg = Gudang,
                            c_iteno = field.Item,
                            c_stno = sttID,
                            n_qty = field.Quantity,
                            n_sisa = field.Quantity
                        });

                      mtd.n_sisa -= rnQty;

                      totalDetails++;
                  }
                  #endregion

                  #region Cek Combo
                  else
                  {
                      listResComboh = (from q in db.LG_ComboHs
                                       join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                       from qBat in q_2.DefaultIfEmpty()
                                       where q.c_gdg == Gudang && q.c_batch == field.Batch && q.c_iteno == field.Item && q.n_gsisa > 0
                                       orderby qBat.d_expired
                                       select q).Distinct().ToList();

                      if (listResComboh.Count > 0)
                      {
                          rnQty = 0;

                          for (nLoopC = 0; nLoopC < listResComboh.Count; nLoopC++)
                          {
                              comboh = listResComboh[nLoopC];

                              rnSisa = (comboh.n_gsisa.HasValue ? comboh.n_gsisa.Value : 0);

                              if (rnSisa > 0)
                              {
                                  if (rnSisa >= mkQtyReloc)
                                  {
                                      rnAvaible = mkQtyReloc;
                                      rnQty += mkQtyReloc;

                                      comboh.n_gsisa -= mkQtyReloc;

                                      mkQtyReloc = 0;
                                  }
                                  else
                                  {
                                      rnAvaible = rnSisa;
                                      rnQty += rnSisa;

                                      mkQtyReloc -= comboh.n_gsisa.Value;

                                      comboh.n_gsisa = 0;
                                  }

                                  listStd2.Add(new LG_STD2()
                                  {
                                      c_batch = field.Batch,
                                      c_gdg = Gudang,
                                      c_iteno = field.Item,
                                      c_no = comboh.c_combono,
                                      c_stno = sttID,
                                      n_qty = rnAvaible
                                  });
                              }

                              if (mkQtyReloc < 1)
                              {
                                  break;
                              }
                          }

                          listStd1.Add(new LG_STD1()
                          {
                              c_batch = field.Batch,
                              c_gdg = Gudang,
                              c_iteno = field.Item,
                              c_stno = sttID,
                              n_qty = field.Quantity,
                              n_sisa = field.Quantity
                          });

                          mtd.n_sisa -= rnQty;

                          totalDetails++;
                      }
                  }
                  #endregion

                }

                if (listResRND1 != null)
                {
                  listResRND1.Clear();
                }

                if (listResComboh != null)
                {
                    listResComboh.Clear();
                }
                #endregion
              }
            }

            if ((listStd1.Count > 0) && (listStd2.Count > 0))
            {
              db.LG_STD1s.InsertAllOnSubmit(listStd1.ToArray());
              db.LG_STD2s.InsertAllOnSubmit(listStd2.ToArray());

              listStd1.Clear();
              listStd2.Clear();
            }
          }
          #endregion

          dic = new Dictionary<string, string>();

          dic.Add("STT", sttID);
          dic.Add("Tanggal", date.ToString("yyyyMMdd"));

          result = string.Format("Total {0} detail(s)", totalDetails);

          hasAnyChanges = (totalDetails > 0);

          #endregion
        }
        else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify

          if (string.IsNullOrEmpty(sttID))
          {
            result = "Nomor STT dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }

          sth = (from q in db.LG_STHs
                 where q.c_stno == sttID
                 select q).Take(1).SingleOrDefault();

          if (sth == null)
          {
            result = "Nomor STT tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }
          else if (sth.l_delete.HasValue && sth.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor STT yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }
          if (Commons.HasDO(db, sttID))
          {
            result = "STT yang sudah terdapat Delivery Order tidak dapat diubah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, sth.d_stdate))
          {
            result = "STT tidak dapat diubah, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
          {
            sth.v_ket = structure.Fields.Keterangan;
          }

          sth.c_update = nipEntry;
          sth.d_update = DateTime.Now;

          #region Populate Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            //listRN = new List<string>();
            listStd3 = new List<LG_STD3>();
            listStd2Copy = new List<LG_STD2>();
            //listResRND1 = new List<LG_RND1>();

            listStd1 = (from q in db.LG_STD1s
                        where q.c_stno == sttID
                        select q).Distinct().ToList();

            listStd2 = (from q in db.LG_STD2s
                        where q.c_stno == sttID
                        select q).ToList();

            //listMTD = (from q in db.MK_MTDs
            //           where q.c_mtno == sth.c_mtno
            //           select q).Distinct().ToList();
            
            listMTD = (from q in db.MK_MTHs
                       join q1 in db.MK_MTDs on new { q.c_gdg, q.c_mtno} equals new { q1.c_gdg, q1.c_mtno }
                       where (q.c_gdg == Gudang) && (q1.c_mtno == structure.Fields.Memo)
                        && (q.c_type == structure.Fields.TipeTransaksi)
                        && ((q.l_delete.HasValue ? q.l_delete.Value: false) == false)
                        select q1).Distinct().ToList();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              mkQtyReloc = 0;

              if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0))
              {
                #region Cek MTD

                //mtd = (from q in db.MK_MTDs
                //       join q1 in db.MK_MTHs on new { q.c_gdg, q.c_mtno } equals new { q1.c_gdg, q1.c_mtno }
                //       where q.c_gdg == Gudang && q1.c_mtno == structure.Fields.Memo && q1.c_type == structure.Fields.TipeTransaksi
                //       select q).Distinct().Take(1).SingleOrDefault();

                mtd = listMTD.Find(delegate(MK_MTD mkMTD)
                {
                  mkQty = (mkMTD.n_sisa.HasValue ? mkMTD.n_sisa.Value : 0);

                  return field.Item.Equals((string.IsNullOrEmpty(mkMTD.c_iteno) ? string.Empty : mkMTD.c_iteno), StringComparison.OrdinalIgnoreCase) &&
                    (mkQty > 0);
                });

                if (mtd != null)
                {
                  mkQty = (mtd.n_sisa.HasValue ? mtd.n_sisa.Value : 0);

                  mkQtyReloc = mkQty =
                    (field.Quantity > mkQty ? mkQty : field.Quantity);

                  //if (field.Quantity > mtd.n_sisa.Value)
                  //{
                  //  mkQty = mtd.n_sisa.Value;
                  //}
                  //else
                  //{
                  //  mkQty = field.Quantity;
                  //}

                  //mkQtyReloc = mkQty.Value;

                  listResRND1 = (from q in db.LG_RND1s
                                 join q1 in db.LG_RNHs on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                 join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                 from qBat in q_2.DefaultIfEmpty()
                                 where q.c_gdg == Gudang && q.c_batch == field.Batch && q.c_iteno == field.Item && q.n_gsisa > 0
                                 orderby qBat.d_expired
                                 select q).Distinct().ToList();

                  #region Cek RN

                  if (listResRND1.Count > 0)
                  {
                      rnQty = 0;

                      for (nLoopC = 0; nLoopC < listResRND1.Count; nLoopC++)
                      {
                          rnd1 = listResRND1[nLoopC];

                          rnSisa = (rnd1.n_gsisa.HasValue ? rnd1.n_gsisa.Value : 0);

                          if (rnSisa > 0)
                          {
                              if (rnSisa >= mkQtyReloc)
                              {
                                  rnAvaible = mkQtyReloc;
                                  rnQty += mkQtyReloc;

                                  rnd1.n_gsisa -= mkQtyReloc;

                                  mkQtyReloc = 0;
                              }
                              else
                              {
                                  rnAvaible = rnSisa;
                                  rnQty += rnSisa;

                                  mkQtyReloc -= rnd1.n_gsisa.Value;

                                  rnd1.n_gsisa = 0;
                              }

                              listStd2Copy.Add(new LG_STD2()
                              {
                                  c_batch = field.Batch,
                                  c_gdg = Gudang,
                                  c_iteno = field.Item,
                                  c_no = rnd1.c_rnno,
                                  c_stno = sttID,
                                  n_qty = rnAvaible
                              });
                              
                              listStd3.Add(new LG_STD3()
                              {
                                  c_gdg = Gudang,
                                  c_stno = sttID,
                                  c_iteno = field.Item,
                                  c_batch = field.Batch,
                                  c_no = rnd1.c_rnno,
                                  n_qty = rnAvaible,
                                  n_sisa = rnAvaible,
                                  v_type = "01",
                                  c_entry = nipEntry,
                                  d_update = date
                              });
                          }

                          if (mkQtyReloc < 1)
                          {
                              break;
                          }
                      }

                      #region Jika dalam batch yang sama terdapat di RN & Combo

                      if (mkQtyReloc > 0)
                      {
                          listResComboh = (from q in db.LG_ComboHs
                                       join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                       from qBat in q_2.DefaultIfEmpty()
                                       where q.c_gdg == Gudang && q.c_batch == field.Batch && q.c_iteno == field.Item && q.n_gsisa > 0
                                       orderby qBat.d_expired
                                       select q).Distinct().ToList();

                          if (listResComboh.Count > 0)
                          {
                              for (nLoopC = 0; nLoopC < listResComboh.Count; nLoopC++)
                              {
                                  comboh = listResComboh[nLoopC];

                                  rnSisa = (comboh.n_gsisa.HasValue ? comboh.n_gsisa.Value : 0);

                                  if (rnSisa > 0)
                                  {
                                      if (rnSisa >= mkQtyReloc)
                                      {
                                          rnAvaible = mkQtyReloc;
                                          rnQty += mkQtyReloc;

                                          comboh.n_gsisa -= mkQtyReloc;

                                          mkQtyReloc = 0;
                                      }
                                      else
                                      {
                                          rnAvaible = rnSisa;
                                          rnQty += rnSisa;

                                          mkQtyReloc -= comboh.n_gsisa.Value;

                                          comboh.n_gsisa = 0;
                                      }

                                      listStd2Copy.Add(new LG_STD2()
                                      {
                                          c_batch = field.Batch,
                                          c_gdg = Gudang,
                                          c_iteno = field.Item,
                                          c_no = comboh.c_combono,
                                          c_stno = sttID,
                                          n_qty = rnAvaible
                                      });

                                      listStd3.Add(new LG_STD3()
                                      {
                                          c_gdg = Gudang,
                                          c_stno = sttID,
                                          c_iteno = field.Item,
                                          c_batch = field.Batch,
                                          c_no = comboh.c_combono,
                                          n_qty = rnAvaible,
                                          n_sisa = rnAvaible,
                                          v_type = "01",
                                          c_entry = nipEntry,
                                          d_update = date
                                      });
                                  }

                                  if (mkQtyReloc < 1)
                                  {
                                      break;
                                  }
                              }
                          }
                      }

                      #endregion

                      std1 = new LG_STD1()
                      {
                          c_batch = field.Batch,
                          c_gdg = Gudang,
                          c_iteno = field.Item,
                          c_stno = sttID,
                          n_qty = field.Quantity,
                          n_sisa = field.Quantity
                      };

                      db.LG_STD1s.InsertOnSubmit(std1);

                      mtd.n_sisa -= rnQty;

                      totalDetails++;
                  }

                  #endregion

                  #region Cek Combo

                  else
                  {
                      listResComboh = (from q in db.LG_ComboHs
                                       join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                       from qBat in q_2.DefaultIfEmpty()
                                       where q.c_gdg == Gudang && q.c_batch == field.Batch && q.c_iteno == field.Item && q.n_gsisa > 0
                                       orderby qBat.d_expired
                                       select q).Distinct().ToList();

                      if (listResComboh.Count > 0)
                      {
                          rnQty = 0;

                          for (nLoopC = 0; nLoopC < listResComboh.Count; nLoopC++)
                          {
                              comboh = listResComboh[nLoopC];

                              rnSisa = (comboh.n_gsisa.HasValue ? comboh.n_gsisa.Value : 0);

                              if (rnSisa > 0)
                              {
                                  if (rnSisa >= mkQtyReloc)
                                  {
                                      rnAvaible = mkQtyReloc;
                                      rnQty += mkQtyReloc;

                                      comboh.n_gsisa -= mkQtyReloc;

                                      mkQtyReloc = 0;
                                  }
                                  else
                                  {
                                      rnAvaible = rnSisa;
                                      rnQty += rnSisa;

                                      mkQtyReloc -= comboh.n_gsisa.Value;

                                      comboh.n_gsisa = 0;
                                  }

                                  listStd2Copy.Add(new LG_STD2()
                                  {
                                      c_batch = field.Batch,
                                      c_gdg = Gudang,
                                      c_iteno = field.Item,
                                      c_no = comboh.c_combono,
                                      c_stno = sttID,
                                      n_qty = rnAvaible
                                  });

                                  listStd3.Add(new LG_STD3()
                                  {
                                      c_gdg = Gudang,
                                      c_stno = sttID,
                                      c_iteno = field.Item,
                                      c_batch = field.Batch,
                                      c_no = comboh.c_combono,
                                      n_qty = rnAvaible,
                                      n_sisa = rnAvaible,
                                      v_type = "01",
                                      c_entry = nipEntry,
                                      d_update = date
                                  });
                              }

                              if (mkQtyReloc < 1)
                              {
                                  break;
                              }
                          }

                          std1 = new LG_STD1()
                          {
                              c_batch = field.Batch,
                              c_gdg = Gudang,
                              c_iteno = field.Item,
                              c_stno = sttID,
                              n_qty = field.Quantity,
                              n_sisa = field.Quantity
                          };

                          db.LG_STD1s.InsertOnSubmit(std1);

                          mtd.n_sisa -= rnQty;

                          totalDetails++;
                      }
                  }
                  #endregion

                  #region Old Coded

                  //#region Cek RN

                  //if (listResRND1.Count > 0)
                  //{
                  //  for (nLoopC = 0; nLoopC < listResRND1.Count; nLoopC++)
                  //  {
                  //    rnd1 = listResRND1[nLoopC];

                  //    if ((rnd1.n_gsisa.HasValue) && (rnd1.n_gsisa.Value > 0))
                  //    {
                  //      if (rnd1.n_gsisa.Value >= mkQtyReloc)
                  //      {
                  //        rnQty = mkQtyReloc;

                  //        rnd1.n_gsisa -= mkQtyReloc;
                  //        mkQtyReloc = 0;
                  //      }
                  //      else
                  //      {
                  //        rnQty = rnd1.n_gsisa.Value;

                  //        mkQtyReloc -= rnd1.n_gsisa.Value;
                  //        rnd1.n_gsisa = 0;
                  //      }

                  //      listStd2.Add(new LG_STD2()
                  //      {
                  //        c_batch = field.Batch,
                  //        c_gdg = Gudang,
                  //        c_iteno = field.Item,
                  //        c_no = rnd1.c_rnno,
                  //        c_stno = sttID,
                  //        n_qty = rnQty
                  //      });
                  //    }
                  //    if (mkQtyReloc < 1)
                  //    {
                  //      break;
                  //    }
                  //  }


                  //  listStd1.Add(new LG_STD1()
                  //  {
                  //    c_batch = field.Batch,
                  //    c_gdg = Gudang,
                  //    c_iteno = field.Item,
                  //    c_stno = sttID,
                  //    n_qty = field.Quantity,
                  //    n_sisa = field.Quantity
                  //  });
                  //}

                  //#endregion

                  //if ((listStd1.Count > 0) && (listStd2.Count > 0))
                  //{
                  //  if (mkAlloc <= mtd.n_sisa.Value)
                  //  {
                  //    mtd.n_sisa -= mkAlloc;
                  //  }

                  //  else
                  //  {
                  //    mtd.n_sisa -= 0;
                  //  }
                  //}

                  //totalDetails++;

                  #endregion
                }

                if (listStd2Copy.Count > 0)
                {
                    db.LG_STD2s.InsertAllOnSubmit(listStd2Copy.ToArray());
                    listStd2Copy.Clear();
                }

                if (listResRND1 != null)
                {
                  listResRND1.Clear();
                }

                if (listResComboh != null)
                {
                    listResComboh.Clear();
                }

                #endregion
              }
              else if ((field != null) && (!field.IsNew) && field.IsDelete)
              {
                #region Cek STT

                //std1 = (from q in db.LG_STD1s
                //        join q1 in db.LG_STHs on new { q.c_gdg, q.c_stno } equals new { q1.c_gdg, q1.c_stno }
                //        where q.c_batch == field.Batch && q.c_iteno == field.Item &&
                //        q.c_stno == sttID && q1.c_mtno == structure.Fields.Memo
                //        select q).Distinct().Take(1).SingleOrDefault();

                //sth = (from q in db.LG_STHs
                //       where q.c_mtno == structure.Fields.Memo && q.c_gdg == Gudang
                //       select q).Distinct().Take(1).SingleOrDefault();

                std1 = listStd1.Find(delegate(LG_STD1 std)
                {
                  return field.Item.Equals((string.IsNullOrEmpty(std.c_iteno) ? string.Empty : std.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    field.Batch.Equals((string.IsNullOrEmpty(std.c_batch) ? string.Empty : std.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                });

                if (std1 != null)
                {
                  mkQtyReloc = (std1.n_qty.HasValue ? std1.n_qty.Value : 0);

                  //mkQtyReloc += mkAlloc;

                  //mkAlloc = (std1.n_sisa.HasValue ? std1.n_sisa.Value : 0);
                  
                  #region Reverse MK

                  mtd = listMTD.Find(delegate(MK_MTD mkMtd)
                  {
                    return field.Item.Equals((string.IsNullOrEmpty(mkMtd.c_iteno) ? string.Empty : mkMtd.c_iteno), StringComparison.OrdinalIgnoreCase);
                  });

                  if (mtd != null)
                  {
                    mtd.n_sisa += mkQtyReloc;
                  }

                  #endregion

                  #region Reset STD2

                  listStd2Copy = listStd2.FindAll(delegate(LG_STD2 std)
                  {
                    return field.Item.Equals((string.IsNullOrEmpty(std.c_iteno) ? string.Empty : std.c_iteno), StringComparison.OrdinalIgnoreCase) &&
                      field.Batch.Equals((string.IsNullOrEmpty(std.c_batch) ? string.Empty : std.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                  });

                  if ((listStd2Copy != null) && (listStd2Copy.Count > 0))
                  {
                    for (nLoopC = 0; nLoopC < listStd2Copy.Count; nLoopC++)
                    {
                      std2 = listStd2Copy[nLoopC];

                      if (std2.c_no.Substring(0, 2).ToUpper() == "RN" || std2.c_no.Substring(0, 2).ToUpper() == "RR")
                      {
                          #region Reverse RN

                          rnQty = (std2.n_qty.HasValue ? std2.n_qty.Value : 0);

                          if (rnQty != 0)
                          {
                              rnd1 = (from q in db.LG_RND1s
                                      where q.c_gdg == sth.c_gdg && q.c_rnno == std2.c_no
                                        && q.c_iteno == std2.c_iteno && q.c_batch == std2.c_batch
                                      select q).Take(1).SingleOrDefault();

                              if (rnd1 != null)
                              {
                                  rnd1.n_gsisa += rnQty;
                              }
                          }

                          #endregion
                      }
                      else
                      {
                          #region Reverse Combo

                          rnQty = (std2.n_qty.HasValue ? std2.n_qty.Value : 0);

                          if (rnQty != 0)
                          {
                              comboh = (from q in db.LG_ComboHs
                                      where q.c_gdg == sth.c_gdg && q.c_combono == std2.c_no
                                        && q.c_iteno == std2.c_iteno && q.c_batch == std2.c_batch
                                      select q).Take(1).SingleOrDefault();

                              if (comboh != null)
                              {
                                  comboh.n_gsisa += rnQty;
                              }
                          }

                          #endregion
                      }

                      listStd3.Add(new LG_STD3()
                      {
                        c_gdg = sth.c_gdg,
                        c_stno = sttID,
                        c_iteno = std1.c_iteno,
                        c_batch = std1.c_batch,
                        c_no = std2.c_no,
                        n_qty = rnQty,
                        n_sisa = rnQty,
                        c_entry = nipEntry,
                        d_update = date,
                        v_ket_del = field.KeteranganMod,
                        v_type = "03"
                      });

                    }

                    db.LG_STD2s.DeleteAllOnSubmit(listStd2Copy.ToArray());
                    listStd2Copy.Clear();
                  }
                  else
                  {
                    listStd3.Add(new LG_STD3()
                    {
                      c_gdg = sth.c_gdg,
                      c_stno = sttID,
                      c_iteno = std1.c_iteno,
                      c_batch = std1.c_batch,
                      c_no = null,
                      n_qty = std1.n_qty,
                      n_sisa = std1.n_sisa,
                      c_entry = nipEntry,
                      d_update = date,
                      v_ket_del = field.KeteranganMod,
                      v_type = "03"
                    });
                  }

                  #endregion

                  db.LG_STD1s.DeleteOnSubmit(std1);

                  totalDetails++;
                }

                #endregion
              }
            }

            if (listStd3.Count > 0)
            {
              db.LG_STD3s.InsertAllOnSubmit(listStd3.ToArray());
              listStd3.Clear();
            }
          }

          #endregion

          hasAnyChanges = true;

          #endregion
        }
        else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete

          if (string.IsNullOrEmpty(sttID))
          {
            result = "Nomor STT dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }

          sth = (from q in db.LG_STHs
                 where q.c_stno == sttID
                 select q).Take(1).SingleOrDefault();

          if (sth == null)
          {
            result = "Nomor STT tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }
          else if (sth.l_delete.HasValue && sth.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor STT yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, sth.d_stdate))
          {
            result = "STT tidak dapat dihapus, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.HasDO(db, sttID))
          {
            result = "STT yang sudah terdapat Delivery Order tidak dapat diubah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }

          sth.c_update = nipEntry;
          sth.d_update = DateTime.Now;

          sth.l_delete = true;
          sth.v_ket_mark = structure.Fields.Keterangan;

          #region Cek STT

          listStd3 = new List<LG_STD3>();

          listStd1 = (from q in db.LG_STD1s
                      where q.c_stno == sttID
                      select q).Distinct().ToList();

          listStd2 = (from q in db.LG_STD2s
                      where q.c_stno == sttID
                      select q).ToList();

          listMTD = (from q in db.MK_MTDs
                     where q.c_mtno == sth.c_mtno
                     select q).Distinct().ToList();

          if ((listStd2 != null) && (listStd2.Count > 0))
          {
            for (nLoopC = 0; nLoopC < listStd2.Count; nLoopC++)
            {
              std2 = listStd2[nLoopC];

              if (std2 != null)
              {
                mkQtyReloc = (std2.n_qty.HasValue ? std2.n_qty.Value : 0);

                #region Reverse MK

                mtd = listMTD.Find(delegate(MK_MTD mkMtd)
                {
                  return ((string.IsNullOrEmpty(std2.c_iteno) ? string.Empty : std2.c_iteno) ==
                    (string.IsNullOrEmpty(mkMtd.c_iteno) ? string.Empty : mkMtd.c_iteno));
                });

                if (mtd != null)
                {
                  mtd.n_sisa += mkQtyReloc;
                }

                #endregion

                if (std2.c_no.Substring(0, 2).ToUpper() == "RN" || std2.c_no.Substring(0, 2).ToUpper() == "RR")
                {
                    #region Reverse RN

                    rnd1 = (from q in db.LG_RND1s
                            where q.c_gdg == sth.c_gdg && q.c_rnno == std2.c_no
                              && q.c_iteno == std2.c_iteno && q.c_batch == std2.c_batch
                            select q).Take(1).SingleOrDefault();

                    if (rnd1 != null)
                    {
                        rnd1.n_gsisa += mkQtyReloc;
                    }

                    #endregion
                }
                else
                {
                    #region Reverse Combo

                    comboh = (from q in db.LG_ComboHs
                            where q.c_gdg == sth.c_gdg && q.c_combono == std2.c_no
                              && q.c_iteno == std2.c_iteno && q.c_batch == std2.c_batch
                            select q).Take(1).SingleOrDefault();

                    if (comboh != null)
                    {
                        comboh.n_gsisa += mkQtyReloc;
                    }

                    #endregion
                }
                #region Log STD3

                listStd3.Add(new LG_STD3()
                {
                  c_batch = std2.c_batch,
                  c_entry = nipEntry,
                  d_update = date,
                  c_gdg = std2.c_gdg,
                  c_iteno = std2.c_iteno,
                  c_no = std2.c_no,
                  c_stno = std2.c_stno,
                  n_qty = std2.n_qty,
                  n_sisa = std2.n_qty,
                  v_ket_del = structure.Fields.Keterangan,
                  v_type = "03"
                });

                #endregion
              }
            }

            #region Insert Log STD3

            if (listStd3.Count > 0)
            {
              db.LG_STD3s.InsertAllOnSubmit(listStd3.ToArray());
              listStd3.Clear();
            }

            #endregion

            #region Delete STD2

            if (listStd2.Count > 0)
            {
              db.LG_STD2s.DeleteAllOnSubmit(listStd2.ToArray());
              listStd2.Clear();
            }

            #endregion

            #region Delete STD1

            if (listStd1.Count > 0)
            {
              db.LG_STD1s.DeleteAllOnSubmit(listStd1.ToArray());
              listStd1.Clear();
            }

            #endregion
          }

          #region Old Coded

          //std1 = (from q in db.LG_STD1s
          //        join q1 in db.LG_STHs on new { q.c_gdg, q.c_stno } equals new { q1.c_gdg, q1.c_stno }
          //        where q.c_batch == field.Batch && q.c_iteno == field.Item &&
          //        q.c_stno == sttID && q1.c_mtno == structure.Fields.Memo
          //        select q).Distinct().Take(1).SingleOrDefault();

          //if (std1 != null)
          //{
          //  mkQtyReloc = (std1.n_qty.HasValue ? std1.n_qty.Value : 0);
          //  mkAlloc = (std1.n_sisa.HasValue ? std1.n_sisa.Value : 0);

          //  if (mkAlloc.Equals(mkQtyReloc))
          //  {
          //    #region Reverse MK

          //    mtd = (from q in db.MK_MTDs
          //           where q.c_mtno == sth.c_mtno && q.c_iteno == std1.c_iteno
          //           select q).Distinct().Take(1).SingleOrDefault();

          //    if (mtd != null)
          //    {
          //      mtd.n_sisa += mkAlloc;
          //    }

          //    #endregion

          //    #region Reverse RN

          //    listStd2 = (from q in db.LG_STD2s
          //                where q.c_stno == sttID && q.c_iteno == std1.c_iteno &&
          //                q.c_batch == std1.c_batch
          //                select q).Distinct().ToList();

          //    listStd1 = (from q in db.LG_STD1s
          //                where q.c_stno == sttID && q.c_iteno == std1.c_iteno &&
          //                q.c_batch == std1.c_batch
          //                select q).Distinct().ToList();


          //    if ((listStd2 != null) && (listStd2.Count > 0))
          //    {
          //      for (nLoopC = 0; nLoopC < listStd2.Count; nLoopC++)
          //      {
          //        std2 = listStd2[nLoopC];
          //        std1 = listStd1[nLoopC];

          //        if (std2 != null)
          //        {
          //          rnd1 = (from q in db.LG_RND1s
          //                  where q.c_batch == std2.c_batch && q.c_iteno == std2.c_iteno &&
          //                  q.c_rnno == std2.c_no && q.c_gdg == std2.c_gdg
          //                  select q).Distinct().Take(1).SingleOrDefault();

          //          rnd1.n_gsisa += (std2.n_qty);

          //          listResRND1.Add(rnd1);

          //          #region Log STD3

          //          listStd3.Add(new LG_STD3()
          //          {
          //            c_batch = std2.c_batch,
          //            c_entry = nipEntry,
          //            d_update = DateTime.Now,
          //            c_gdg = std2.c_gdg,
          //            c_iteno = std2.c_iteno,
          //            c_no = std2.c_no,
          //            c_stno = std2.c_stno,
          //            n_qty = std2.n_qty,
          //            n_sisa = std1.n_sisa,
          //            v_ket_del = field.KeteranganMod,
          //            v_type = "03"
          //          });

          //          #endregion
          //        }
          //      }

          //      #region Delete STD2

          //      db.LG_STD2s.DeleteAllOnSubmit(listStd2.ToArray());

          //      #endregion
          //    }

          //    #endregion

          //    #region Delete STD1

          //    db.LG_STD1s.DeleteOnSubmit(std1);

          //    #endregion
          //  }
          //}

          #endregion

          #endregion

          hasAnyChanges = true;

          #endregion
        }

        if (hasAnyChanges)
        {
          db.SubmitChanges();

          db.Transaction.Commit();

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
        rpe = ResponseParser.ResponseParserEnum.IsError;

        result = string.Format("ScmsSoaLibrary.Bussiness.Peminjaman:STT - {0}", ex.Message);

        Logger.WriteLine(result, true);
        Logger.WriteLine(ex.StackTrace);
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