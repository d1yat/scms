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
  class Pemusnahan
  {
    public string ProsesPemusnahan(ScmsSoaLibrary.Parser.Class.PemusnahanStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      LG_PMH pmh = null;

      MK_MTD mtd = null;

      LG_RND1 rnd1 = null;
      LG_ComboH comboh = null;

      ScmsSoaLibrary.Parser.Class.PemusnahanStructureField field = null;
      string nipEntry = null;
      string pmID = null;
      //string tmpNumbering = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      decimal mkQtyReloc = 0,
        rnQty = 0, rnSisa = 0, rnAvaible = 0,
         mkQty = 0;

      List<LG_PMD1> listPmd1 = null;
      List<LG_PMD2> listPmd2 = null;
      List<LG_PMD2> listPmd2Copy = null;
      //List<string> listRN = null;
      List<LG_RND1> listResRND1 = null;
      List<LG_ComboH> listResComboh = null;
      List<LG_PMD3> listPmd3 = null;

      List<MK_MTD> listMTD = null;

      char Gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '1' : structure.Fields.Gudang[0]);

      LG_PMD1 pmd1 = null;
      LG_PMD2 pmd2 = null;

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

      pmID = (structure.Fields.PMid ?? string.Empty);

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          if (!string.IsNullOrEmpty(pmID))
          {
            result = "Nomor transaksi pemusnahan harus kosong.";

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
            result = "Transaksi pemusnahan tidak dapat disimpan, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          pmID = Commons.GenerateNumbering<LG_PMH>(db, "PM", '3', "42", date, "c_pmno");

          pmh = new LG_PMH()
          {
            c_entry = nipEntry,
            c_gdg = Gudang,
            c_memo = structure.Fields.Memo,
            c_pmno = pmID,
            c_update = nipEntry,
            d_entry = DateTime.Now,
            d_pmdate = date.Date,
            d_update = DateTime.Now,
            l_delete = false,
            l_print = false,
            v_ket = structure.Fields.Keterangan
          };

          db.LG_PMHs.InsertOnSubmit(pmh);

          #region Insert Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            //listRN = new List<string>();
            listPmd1 = new List<LG_PMD1>();
            listPmd2 = new List<LG_PMD2>();
            
            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0))
              {
                  listResRND1 = (from q in db.LG_RND1s
                                 join q1 in db.LG_RNHs on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                 join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                 from qBat in q_2.DefaultIfEmpty()
                                 where q.c_gdg == Gudang && q.c_batch == field.Batch && q.c_iteno == field.Item && q.n_bsisa > 0
                                 orderby qBat.d_expired
                                 select q).Distinct().ToList();

                  mkQtyReloc = field.Quantity;

                  #region Cek RN

                  if (listResRND1.Count > 0)
                  {
                      rnQty = 0;

                      for (nLoopC = 0; nLoopC < listResRND1.Count; nLoopC++)
                      {
                          rnd1 = listResRND1[nLoopC];

                          rnSisa = (rnd1.n_bsisa.HasValue ? rnd1.n_bsisa.Value : 0);

                          if (rnSisa > 0)
                          {
                              if (rnSisa >= mkQtyReloc)
                              {
                                  rnAvaible = mkQtyReloc;
                                  rnQty += mkQtyReloc;
                                  rnd1.n_bsisa -= mkQtyReloc;
                                  mkQtyReloc = 0;
                              }
                              else
                              {
                                  rnAvaible = rnSisa;
                                  rnQty += rnSisa;
                                  mkQtyReloc -= rnd1.n_bsisa.Value;
                                  rnd1.n_bsisa = 0;
                              }
                              listPmd2.Add(new LG_PMD2()
                                {
                                    c_batch = field.Batch,
                                    c_gdg = Gudang,
                                    c_iteno = field.Item,
                                    c_no = rnd1.c_rnno,
                                    c_pmno = pmID,
                                    n_qty = rnAvaible
                                });
                          }

                          if (mkQtyReloc < 1)
                          {
                              break;
                          }
                      }

                      #region Jika dalam batch yang sama terdapat di RN & Combo

                      //if (mkQtyReloc > 0)
                      //{
                      //    listResComboh = (from q in db.LG_ComboHs
                      //                 join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                      //                 from qBat in q_2.DefaultIfEmpty()
                      //                 where q.c_gdg == Gudang && q.c_batch == field.Batch && q.c_iteno == field.Item && q.n_gsisa > 0
                      //                 orderby qBat.d_expired
                      //                 select q).Distinct().ToList();

                      //    if (listResComboh.Count > 0)
                      //    {
                      //        for (nLoopC = 0; nLoopC < listResComboh.Count; nLoopC++)
                      //        {
                      //            comboh = listResComboh[nLoopC];

                      //            rnSisa = (comboh.n_gsisa.HasValue ? comboh.n_gsisa.Value : 0);

                      //            if (rnSisa > 0)
                      //            {
                      //                if (rnSisa >= mkQtyReloc)
                      //                {
                      //                    rnAvaible = mkQtyReloc;
                      //                    rnQty += mkQtyReloc;

                      //                    comboh.n_gsisa -= mkQtyReloc;

                      //                    mkQtyReloc = 0;
                      //                }
                      //                else
                      //                {
                      //                    rnAvaible = rnSisa;
                      //                    rnQty += rnSisa;

                      //                    mkQtyReloc -= comboh.n_gsisa.Value;

                      //                    comboh.n_gsisa = 0;
                      //                }

                      //                listPmd2.Add(new LG_STD2()
                      //                {
                      //                    c_batch = field.Batch,
                      //                    c_gdg = Gudang,
                      //                    c_iteno = field.Item,
                      //                    c_no = comboh.c_combono,
                      //                    c_stno = pmID,
                      //                    n_qty = rnAvaible
                      //                });
                      //            }

                      //            if (mkQtyReloc < 1)
                      //            {
                      //                break;
                      //            }
                      //        }
                      //    }
                      //}

                      #endregion

                      listPmd1.Add(new LG_PMD1()
                        {
                            c_batch = field.Batch,
                            c_gdg = Gudang,
                            c_iteno = field.Item,
                            c_pmno = pmID,
                            n_qty = field.Quantity,
                            n_sisa = field.Quantity
                        });

                      totalDetails++;
                  }
                  #endregion

                  #region Cek Combo
                  //else
                  //{
                  //    listResComboh = (from q in db.LG_ComboHs
                  //                     join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                  //                     from qBat in q_2.DefaultIfEmpty()
                  //                     where q.c_gdg == Gudang && q.c_batch == field.Batch && q.c_iteno == field.Item && q.n_gsisa > 0
                  //                     orderby qBat.d_expired
                  //                     select q).Distinct().ToList();

                  //    if (listResComboh.Count > 0)
                  //    {
                  //        rnQty = 0;

                  //        for (nLoopC = 0; nLoopC < listResComboh.Count; nLoopC++)
                  //        {
                  //            comboh = listResComboh[nLoopC];

                  //            rnSisa = (comboh.n_gsisa.HasValue ? comboh.n_gsisa.Value : 0);

                  //            if (rnSisa > 0)
                  //            {
                  //                if (rnSisa >= mkQtyReloc)
                  //                {
                  //                    rnAvaible = mkQtyReloc;
                  //                    rnQty += mkQtyReloc;

                  //                    comboh.n_gsisa -= mkQtyReloc;

                  //                    mkQtyReloc = 0;
                  //                }
                  //                else
                  //                {
                  //                    rnAvaible = rnSisa;
                  //                    rnQty += rnSisa;

                  //                    mkQtyReloc -= comboh.n_gsisa.Value;

                  //                    comboh.n_gsisa = 0;
                  //                }

                  //                listPmd2.Add(new LG_STD2()
                  //                {
                  //                    c_batch = field.Batch,
                  //                    c_gdg = Gudang,
                  //                    c_iteno = field.Item,
                  //                    c_no = comboh.c_combono,
                  //                    c_stno = pmID,
                  //                    n_qty = rnAvaible
                  //                });
                  //            }

                  //            if (mkQtyReloc < 1)
                  //            {
                  //                break;
                  //            }
                  //        }

                  //        listPmd1.Add(new LG_STD1()
                  //        {
                  //            c_batch = field.Batch,
                  //            c_gdg = Gudang,
                  //            c_iteno = field.Item,
                  //            c_stno = pmID,
                  //            n_qty = field.Quantity,
                  //            n_sisa = field.Quantity
                  //        });

                  //        mtd.n_sisa -= rnQty;

                  //        totalDetails++;
                  //    }
                  //}
                  #endregion

                

                if (listResRND1 != null)
                {
                  listResRND1.Clear();
                }

                if (listResComboh != null)
                {
                    listResComboh.Clear();
                }
              }
            }

            if ((listPmd1.Count > 0) && (listPmd2.Count > 0))
            {
              db.LG_PMD1s.InsertAllOnSubmit(listPmd1.ToArray());
              db.LG_PMD2s.InsertAllOnSubmit(listPmd2.ToArray());

              listPmd1.Clear();
              listPmd2.Clear();
            }
          }
          #endregion

          dic = new Dictionary<string, string>();

          dic.Add("PM", pmID);
          dic.Add("Tanggal", date.ToString("yyyyMMdd"));

          result = string.Format("Total {0} detail(s)", totalDetails);

          hasAnyChanges = (totalDetails > 0);

          #endregion
        }
        else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify

          if (string.IsNullOrEmpty(pmID))
          {
            result = "Nomor transaksi pemusnahan dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }

          pmh = (from q in db.LG_PMHs
                 where q.c_pmno == pmID
                 select q).Take(1).SingleOrDefault();

          if (pmh == null)
          {
            result = "Nomor transaksi pemusnahan tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }
          else if (pmh.l_delete.HasValue && pmh.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor transaksi pemusnahan yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, pmh.d_pmdate))
          {
            result = "Transkasi pemusnahan tidak dapat diubah, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
          {
            pmh.v_ket = structure.Fields.Keterangan;
          }

          pmh.c_update = nipEntry;
          pmh.d_update = DateTime.Now;

          #region Populate Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listPmd3 = new List<LG_PMD3>();
            listPmd2Copy = new List<LG_PMD2>();
            
            listPmd1 = (from q in db.LG_PMD1s
                        where q.c_pmno == pmID
                        select q).Distinct().ToList();

            listPmd2 = (from q in db.LG_PMD2s
                        where q.c_pmno == pmID
                        select q).ToList();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              mkQtyReloc = 0;

              if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0))
              {
                #region Pemusnahan jika detail ditambah

                  mkQtyReloc = field.Quantity;

                  listResRND1 = (from q in db.LG_RND1s
                                 join q1 in db.LG_RNHs on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                 join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                 from qBat in q_2.DefaultIfEmpty()
                                 where q.c_gdg == Gudang && q.c_batch == field.Batch && q.c_iteno == field.Item && q.n_bsisa > 0
                                 orderby qBat.d_expired
                                 select q).Distinct().ToList();

                  #region Cek RN

                  if (listResRND1.Count > 0)
                  {
                      rnQty = 0;

                      for (nLoopC = 0; nLoopC < listResRND1.Count; nLoopC++)
                      {
                          rnd1 = listResRND1[nLoopC];

                          rnSisa = (rnd1.n_bsisa.HasValue ? rnd1.n_bsisa.Value : 0);

                          if (rnSisa > 0)
                          {
                              if (rnSisa >= mkQtyReloc)
                              {
                                  rnAvaible = mkQtyReloc;
                                  rnQty += mkQtyReloc;

                                  rnd1.n_bsisa -= mkQtyReloc;

                                  mkQtyReloc = 0;
                              }
                              else
                              {
                                  rnAvaible = rnSisa;
                                  rnQty += rnSisa;

                                  mkQtyReloc -= rnd1.n_bsisa.Value;

                                  rnd1.n_bsisa = 0;
                              }

                              listPmd2Copy.Add(new LG_PMD2()
                              {
                                  c_batch = field.Batch,
                                  c_gdg = Gudang,
                                  c_iteno = field.Item,
                                  c_no = rnd1.c_rnno,
                                  c_pmno = pmID,
                                  n_qty = rnAvaible
                              });
                              
                              listPmd3.Add(new LG_PMD3()
                              {
                                  c_gdg = Gudang,
                                  c_pmno = pmID,
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

                      //if (mkQtyReloc > 0)
                      //{
                      //    listResComboh = (from q in db.LG_ComboHs
                      //                 join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                      //                 from qBat in q_2.DefaultIfEmpty()
                      //                 where q.c_gdg == Gudang && q.c_batch == field.Batch && q.c_iteno == field.Item && q.n_gsisa > 0
                      //                 orderby qBat.d_expired
                      //                 select q).Distinct().ToList();

                      //    if (listResComboh.Count > 0)
                      //    {
                      //        for (nLoopC = 0; nLoopC < listResComboh.Count; nLoopC++)
                      //        {
                      //            comboh = listResComboh[nLoopC];

                      //            rnSisa = (comboh.n_gsisa.HasValue ? comboh.n_gsisa.Value : 0);

                      //            if (rnSisa > 0)
                      //            {
                      //                if (rnSisa >= mkQtyReloc)
                      //                {
                      //                    rnAvaible = mkQtyReloc;
                      //                    rnQty += mkQtyReloc;

                      //                    comboh.n_gsisa -= mkQtyReloc;

                      //                    mkQtyReloc = 0;
                      //                }
                      //                else
                      //                {
                      //                    rnAvaible = rnSisa;
                      //                    rnQty += rnSisa;

                      //                    mkQtyReloc -= comboh.n_gsisa.Value;

                      //                    comboh.n_gsisa = 0;
                      //                }

                      //                listPmd2Copy.Add(new LG_STD2()
                      //                {
                      //                    c_batch = field.Batch,
                      //                    c_gdg = Gudang,
                      //                    c_iteno = field.Item,
                      //                    c_no = comboh.c_combono,
                      //                    c_stno = pmID,
                      //                    n_qty = rnAvaible
                      //                });

                      //                listPmd3.Add(new LG_STD3()
                      //                {
                      //                    c_gdg = Gudang,
                      //                    c_stno = pmID,
                      //                    c_iteno = field.Item,
                      //                    c_batch = field.Batch,
                      //                    c_no = comboh.c_combono,
                      //                    n_qty = rnAvaible,
                      //                    n_sisa = rnAvaible,
                      //                    v_type = "01",
                      //                    c_entry = nipEntry,
                      //                    d_update = date
                      //                });
                      //            }

                      //            if (mkQtyReloc < 1)
                      //            {
                      //                break;
                      //            }
                      //        }
                      //    }
                      //}

                      #endregion

                      pmd1 = new LG_PMD1()
                      {
                          c_batch = field.Batch,
                          c_gdg = Gudang,
                          c_iteno = field.Item,
                          c_pmno = pmID,
                          n_qty = field.Quantity,
                          n_sisa = field.Quantity
                      };

                      db.LG_PMD1s.InsertOnSubmit(pmd1);

                      totalDetails++;
                  }

                  #endregion

                  #region Cek Combo

                  //else
                  //{
                  //    listResComboh = (from q in db.LG_ComboHs
                  //                     join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                  //                     from qBat in q_2.DefaultIfEmpty()
                  //                     where q.c_gdg == Gudang && q.c_batch == field.Batch && q.c_iteno == field.Item && q.n_gsisa > 0
                  //                     orderby qBat.d_expired
                  //                     select q).Distinct().ToList();

                  //    if (listResComboh.Count > 0)
                  //    {
                  //        rnQty = 0;

                  //        for (nLoopC = 0; nLoopC < listResComboh.Count; nLoopC++)
                  //        {
                  //            comboh = listResComboh[nLoopC];

                  //            rnSisa = (comboh.n_gsisa.HasValue ? comboh.n_gsisa.Value : 0);

                  //            if (rnSisa > 0)
                  //            {
                  //                if (rnSisa >= mkQtyReloc)
                  //                {
                  //                    rnAvaible = mkQtyReloc;
                  //                    rnQty += mkQtyReloc;

                  //                    comboh.n_gsisa -= mkQtyReloc;

                  //                    mkQtyReloc = 0;
                  //                }
                  //                else
                  //                {
                  //                    rnAvaible = rnSisa;
                  //                    rnQty += rnSisa;

                  //                    mkQtyReloc -= comboh.n_gsisa.Value;

                  //                    comboh.n_gsisa = 0;
                  //                }

                  //                listPmd2Copy.Add(new LG_PMD2()
                  //                {
                  //                    c_batch = field.Batch,
                  //                    c_gdg = Gudang,
                  //                    c_iteno = field.Item,
                  //                    c_no = comboh.c_combono,
                  //                    c_pmno = pmID,
                  //                    n_qty = rnAvaible
                  //                });

                  //                listPmd3.Add(new LG_PMD3()
                  //                {
                  //                    c_gdg = Gudang,
                  //                    c_pmno = pmID,
                  //                    c_iteno = field.Item,
                  //                    c_batch = field.Batch,
                  //                    c_no = comboh.c_combono,
                  //                    n_qty = rnAvaible,
                  //                    n_sisa = rnAvaible,
                  //                    v_type = "01",
                  //                    c_entry = nipEntry,
                  //                    d_update = date
                  //                });
                  //            }

                  //            if (mkQtyReloc < 1)
                  //            {
                  //                break;
                  //            }
                  //        }

                  //        pmd1 = new LG_PMD1()
                  //        {
                  //            c_batch = field.Batch,
                  //            c_gdg = Gudang,
                  //            c_iteno = field.Item,
                  //            c_pmno = pmID,
                  //            n_qty = field.Quantity,
                  //            n_sisa = field.Quantity
                  //        };

                  //        db.LG_PMD1s.InsertOnSubmit(pmd1);

                  //        //mtd.n_sisa -= rnQty;

                  //        totalDetails++;
                  //    }
                  //}
                  #endregion
            
                if (listPmd2Copy.Count > 0)
                {
                    db.LG_PMD2s.InsertAllOnSubmit(listPmd2Copy.ToArray());
                    listPmd2Copy.Clear();
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
                #region Pemusnahan jika detail dihapus

                //std1 = (from q in db.LG_STD1s
                //        join q1 in db.LG_STHs on new { q.c_gdg, q.c_stno } equals new { q1.c_gdg, q1.c_stno }
                //        where q.c_batch == field.Batch && q.c_iteno == field.Item &&
                //        q.c_stno == sttID && q1.c_mtno == structure.Fields.Memo
                //        select q).Distinct().Take(1).SingleOrDefault();

                //sth = (from q in db.LG_STHs
                //       where q.c_mtno == structure.Fields.Memo && q.c_gdg == Gudang
                //       select q).Distinct().Take(1).SingleOrDefault();

                pmd1 = listPmd1.Find(delegate(LG_PMD1 pmd)
                {
                  return field.Item.Equals((string.IsNullOrEmpty(pmd.c_iteno) ? string.Empty : pmd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    field.Batch.Equals((string.IsNullOrEmpty(pmd.c_batch) ? string.Empty : pmd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                });

                if (pmd1 != null)
                {
                  mkQtyReloc = (pmd1.n_qty.HasValue ? pmd1.n_qty.Value : 0);

                  //mkQtyReloc += mkAlloc;

                  //mkAlloc = (std1.n_sisa.HasValue ? std1.n_sisa.Value : 0);
                  
                  
                  #region Reset STD2

                  listPmd2Copy = listPmd2.FindAll(delegate(LG_PMD2 pmd)
                  {
                    return field.Item.Equals((string.IsNullOrEmpty(pmd.c_iteno) ? string.Empty : pmd.c_iteno), StringComparison.OrdinalIgnoreCase) &&
                      field.Batch.Equals((string.IsNullOrEmpty(pmd.c_batch) ? string.Empty : pmd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                  });

                  if ((listPmd2Copy != null) && (listPmd2Copy.Count > 0))
                  {
                    for (nLoopC = 0; nLoopC < listPmd2Copy.Count; nLoopC++)
                    {
                      pmd2 = listPmd2Copy[nLoopC];

                      if (pmd2.c_no.Substring(0, 2).ToUpper() == "RN" || pmd2.c_no.Substring(0, 2).ToUpper() == "RR")
                      {
                          #region Reverse RN

                          rnQty = (pmd2.n_qty.HasValue ? pmd2.n_qty.Value : 0);

                          if (rnQty != 0)
                          {
                              rnd1 = (from q in db.LG_RND1s
                                      where q.c_gdg == pmh.c_gdg && q.c_rnno == pmd2.c_no
                                        && q.c_iteno == pmd2.c_iteno && q.c_batch == pmd2.c_batch
                                      select q).Take(1).SingleOrDefault();

                              if (rnd1 != null)
                              {
                                  rnd1.n_bsisa += rnQty;
                              }
                          }

                          #endregion
                      }
                      else
                      {
                          #region Reverse Combo

                          rnQty = (pmd2.n_qty.HasValue ? pmd2.n_qty.Value : 0);

                          if (rnQty != 0)
                          {
                              comboh = (from q in db.LG_ComboHs
                                      where q.c_gdg == pmh.c_gdg && q.c_combono == pmd2.c_no
                                        && q.c_iteno == pmd2.c_iteno && q.c_batch == pmd2.c_batch
                                      select q).Take(1).SingleOrDefault();

                              if (comboh != null)
                              {
                                  comboh.n_bsisa += rnQty;
                              }
                          }

                          #endregion
                      }

                      listPmd3.Add(new LG_PMD3()
                      {
                        c_gdg = pmh.c_gdg,
                        c_pmno = pmID,
                        c_iteno = pmd1.c_iteno,
                        c_batch = pmd1.c_batch,
                        c_no = pmd2.c_no,
                        n_qty = rnQty,
                        n_sisa = rnQty,
                        c_entry = nipEntry,
                        d_update = date,
                        v_ket_del = field.KeteranganMod,
                        v_type = "03"
                      });

                    }

                    db.LG_PMD2s.DeleteAllOnSubmit(listPmd2Copy.ToArray());
                    listPmd2Copy.Clear();
                  }
                  else
                  {
                    listPmd3.Add(new LG_PMD3()
                    {
                      c_gdg = pmh.c_gdg,
                      c_pmno = pmID,
                      c_iteno = pmd1.c_iteno,
                      c_batch = pmd1.c_batch,
                      c_no = null,
                      n_qty = pmd1.n_qty,
                      n_sisa = pmd1.n_sisa,
                      c_entry = nipEntry,
                      d_update = date,
                      v_ket_del = field.KeteranganMod,
                      v_type = "03"
                    });
                  }

                  #endregion

                  db.LG_PMD1s.DeleteOnSubmit(pmd1);

                  totalDetails++;
                }

                #endregion
              }
            }

            if (listPmd3.Count > 0)
            {
              db.LG_PMD3s.InsertAllOnSubmit(listPmd3.ToArray());
              listPmd3.Clear();
            }
          }

          #endregion

          hasAnyChanges = true;

          #endregion
        }
        else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete

          if (string.IsNullOrEmpty(pmID))
          {
            result = "Nomor transaksi pemusnahan dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }

          pmh = (from q in db.LG_PMHs
                 where q.c_pmno == pmID
                 select q).Take(1).SingleOrDefault();

          if (pmh == null)
          {
            result = "Nomor transaksi pemusnahan tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }
          else if (pmh.l_delete.HasValue && pmh.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor transaksi pemusnahan yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, pmh.d_pmdate))
          {
            result = "Transaksi pemusnahan tidak dapat dihapus, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
         
          pmh.c_update = nipEntry;
          pmh.d_update = DateTime.Now;

          pmh.l_delete = true;
          pmh.v_ket_mark = structure.Fields.Keterangan;

          #region Cek PM

          listPmd3 = new List<LG_PMD3>();

          listPmd1 = (from q in db.LG_PMD1s
                      where q.c_pmno == pmID
                      select q).Distinct().ToList();

          listPmd2 = (from q in db.LG_PMD2s
                      where q.c_pmno == pmID
                      select q).ToList();

          if ((listPmd2 != null) && (listPmd2.Count > 0))
          {
            for (nLoopC = 0; nLoopC < listPmd2.Count; nLoopC++)
            {
              pmd2 = listPmd2[nLoopC];

              if (pmd2 != null)
              {
                mkQtyReloc = (pmd2.n_qty.HasValue ? pmd2.n_qty.Value : 0);

                if (pmd2.c_no.Substring(0, 2).ToUpper() == "RN" || pmd2.c_no.Substring(0, 2).ToUpper() == "RR")
                {
                    #region Reverse RN

                    rnd1 = (from q in db.LG_RND1s
                            where q.c_gdg == pmh.c_gdg && q.c_rnno == pmd2.c_no
                              && q.c_iteno == pmd2.c_iteno && q.c_batch == pmd2.c_batch
                            select q).Take(1).SingleOrDefault();

                    if (rnd1 != null)
                    {
                        rnd1.n_bsisa += mkQtyReloc;
                    }

                    #endregion
                }
                else
                {
                    #region Reverse Combo

                    comboh = (from q in db.LG_ComboHs
                            where q.c_gdg == pmh.c_gdg && q.c_combono == pmd2.c_no
                              && q.c_iteno == pmd2.c_iteno && q.c_batch == pmd2.c_batch
                            select q).Take(1).SingleOrDefault();

                    if (comboh != null)
                    {
                        comboh.n_bsisa += mkQtyReloc;
                    }

                    #endregion
                }
                #region Log STD3

                listPmd3.Add(new LG_PMD3()
                {
                  c_batch = pmd2.c_batch,
                  c_entry = nipEntry,
                  d_update = date,
                  c_gdg = pmd2.c_gdg,
                  c_iteno = pmd2.c_iteno,
                  c_no = pmd2.c_no,
                  c_pmno = pmd2.c_pmno,
                  n_qty = pmd2.n_qty,
                  n_sisa = pmd2.n_qty,
                  v_ket_del = structure.Fields.Keterangan,
                  v_type = "03"
                });

                #endregion
              }
            }

            #region Insert Log STD3

            if (listPmd3.Count > 0)
            {
              db.LG_PMD3s.InsertAllOnSubmit(listPmd3.ToArray());
              listPmd3.Clear();
            }

            #endregion

            #region Delete STD2

            if (listPmd2.Count > 0)
            {
              db.LG_PMD2s.DeleteAllOnSubmit(listPmd2.ToArray());
              listPmd2.Clear();
            }

            #endregion

            #region Delete STD1

            if (listPmd1.Count > 0)
            {
              db.LG_PMD1s.DeleteAllOnSubmit(listPmd1.ToArray());
              listPmd1.Clear();
            }

            #endregion
          }

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

        result = string.Format("ScmsSoaLibrary.Bussiness.Pemusnahan:PM - {0}", ex.Message);

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