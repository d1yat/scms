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
  class ClaimBonusSupplier
  {
    private bool HasClaimAcc(ORMDataContext db, string ClaimBonusID)
    {
      string tmp = (from q in db.LG_ClaimAccHes
                    where q.c_claimno == ClaimBonusID //&& q.l_delete == false
                    select q.c_claimno).Take(1).SingleOrDefault();

      return (!string.IsNullOrEmpty(tmp));
    }

    public string ClaimBonus(ScmsSoaLibrary.Parser.Class.ClaimStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false;
      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      LG_ClaimH claimH = null;
      LG_ClaimD1 claimD1 = null;
      LG_ClaimD2 claimD2 = null;


      List<LG_ClaimD1> listClaimD1 = null;
      //List<string> ListStringClaim = null;
      List<LG_ClaimD2> listClaimD2 = null;
      List<string> ListStringD1 = null;
      List<LG_ClaimD3> ListclaimD3 = null;

      ScmsSoaLibrary.Parser.Class.ClaimStructureField field = null;
      string nipEntry = null;
      string ClaimID = null;
      //string tmpNumbering = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      //decimal? QtyReloc = 0;

      int nLoop = 0, totalDetails = 0,
        nLoopC = 0;

      IDictionary<string, string> dic = null;

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }

      ClaimID = (structure.Fields.ClaimID ?? string.Empty);

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          if (!string.IsNullOrEmpty(ClaimID))
          {
            result = "Nomor Claim harus kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          if (string.IsNullOrEmpty(structure.Fields.Tahun))
          {
            result = "Tahun dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          if (string.IsNullOrEmpty(structure.Fields.Bulan))
          {
            result = "Bulan dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          if (string.IsNullOrEmpty(structure.Fields.Suplier))
          {
            result = "Nama pemasok dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingFA(db, date))
          {
            result = "Claim tidak dapat disimpan, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "CLAIM");

          ClaimID = Commons.GenerateNumberingFormat<LG_ClaimH>(db, "/MS/FA/KD/", '3', "13", date, "c_claimno", "claim");

          claimH = new LG_ClaimH()
          {
            c_claimno = ClaimID,
            c_kurs = (structure.Fields.KursDesc == null ? "01" : structure.Fields.KursDesc),
            n_kurs = Convert.ToDecimal(structure.Fields.KursVal == null ? "0" : structure.Fields.KursVal),
            c_entry = nipEntry,
            c_nosup = structure.Fields.Suplier,
            c_type = structure.Fields.Type,
            c_update = nipEntry,
            d_claimdate = DateTime.Now,
            d_top = DateTime.Now.AddDays(Convert.ToDouble(structure.Fields.Top)),
            d_entry = date,
            l_status = false,
            n_bilva = 0,
            n_bruto = 0,
            n_disc = 0,
            n_tax = 0,
            s_tahun = Convert.ToInt16(structure.Fields.Tahun),
            t_bulan = Convert.ToByte(structure.Fields.Bulan),
            n_top = Convert.ToDecimal(structure.Fields.Top),
            d_update = date,
            l_print = false,
            v_ket = structure.Fields.Keterangan
          };

          db.LG_ClaimHs.InsertOnSubmit(claimH);

          #region Old Code

          //db.SubmitChanges();

          //claimH = (from q in db.LG_ClaimHs
          //       where q.v_ket == tmpNumbering
          //       select q).Take(1).SingleOrDefault();

          //if (!string.IsNullOrEmpty(ClaimID))
          //{
          //  result = "Nomor Claim tidak dapat di raih.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}

          //if (claimH.c_claimno.Equals("XXXXXXXXXX"))
          //{
          //  result = "Trigger Claim tidak aktif.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}

          //claimH.v_ket = structure.Fields.Keterangan;

          //ClaimID = claimH.c_claimno;

          #endregion

          #region Insert Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listClaimD1 = new List<LG_ClaimD1>();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              listClaimD1.Add(new LG_ClaimD1()
              {
                c_claimno = ClaimID,
                c_iteno = field.Item,
                n_disc = field.Disc,
                n_qty = field.Quantity,
                n_salpri = field.Salpri,
                n_sisa = field.Quantity
              });

              totalDetails++;
            }

            db.LG_ClaimD1s.InsertAllOnSubmit(listClaimD1.ToArray());
            listClaimD1.Clear();

          }


          #endregion

          dic = new Dictionary<string, string>();

          dic.Add("CLAIM", ClaimID);
          dic.Add("Tanggal", date.ToString("yyyyMMdd"));

          result = string.Format("Total {0} detail(s)", totalDetails);

          #endregion

          hasAnyChanges = true;
        }
        else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify

          claimH = (from q in db.LG_ClaimHs
                    where q.c_claimno == structure.Fields.ClaimID
                    select q).Take(1).SingleOrDefault();

          if (Commons.IsClosingFA(db, claimH.d_claimdate))
          {
            result = "Claim tidak dapat diubah, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listClaimD1 = new List<LG_ClaimD1>();
            ListclaimD3 = new List<LG_ClaimD3>();
            ListStringD1 = new List<string>();
            claimD1 = new LG_ClaimD1();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              if ((field != null) && (!field.IsNew) && field.IsDelete && (!field.IsModified))
              {
                #region Delete Detil

                claimD1 = (from q in db.LG_ClaimD1s
                           where q.c_iteno == field.Item
                           && q.c_claimno == ClaimID && q.n_qty == q.n_sisa
                           select q).Take(1).SingleOrDefault();

                if (claimD1 != null)
                {

                  listClaimD2 = (from q in db.LG_ClaimD2s
                                 where q.c_iteno == field.Item
                                 && q.c_claimno == ClaimID
                                 select q).ToList();

                  #region Adding CLaim 3

                  if (listClaimD2.Count > 0)
                  {
                    for (nLoopC = 0; nLoopC < listClaimD2.Count; nLoopC++)
                    {
                      claimD2 = listClaimD2[nLoopC];

                      ListclaimD3.Add(new LG_ClaimD3()
                      {
                        c_claimno = ClaimID,
                        c_iteno = field.Item,
                        n_qty = field.Quantity,
                        c_entry = field.Name,
                        d_entry = DateTime.Now,
                        n_disc = field.Disc,
                        n_salpri = field.Salpri,
                        n_sisa = field.Quantity,
                        v_ket_del = field.KeteranganMod,
                        v_type = "03",
                        c_cusno = claimD2.c_cusno
                      });
                    }

                    db.LG_ClaimD2s.DeleteAllOnSubmit(listClaimD2.ToArray());
                    listClaimD2.Clear();
                  }
                  else
                  {
                    ListclaimD3.Add(new LG_ClaimD3()
                    {
                      c_claimno = ClaimID,
                      c_iteno = field.Item,
                      n_qty = field.Quantity,
                      c_entry = field.Name,
                      d_entry = DateTime.Now,
                      n_disc = field.Disc,
                      n_salpri = field.Salpri,
                      n_sisa = field.Quantity,
                      v_ket_del = field.KeteranganMod,
                      v_type = "03",
                    });
                  }

                  #endregion

                  db.LG_ClaimD1s.DeleteOnSubmit(claimD1);
                }
                else
                {
                  result = "Claim tidak dapat diubah, karena sudah Qty sudah di Acc.";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  if (db.Transaction != null)
                  {
                    db.Transaction.Rollback();
                  }

                  goto endLogic;
                }

                #endregion
              }
              else if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified))
              {
                #region Add New Detil

                if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                {
                  claimD1 = new LG_ClaimD1()
                  {
                    c_claimno = ClaimID,
                    c_iteno = field.Item,
                    n_disc = field.Disc,
                    n_qty = field.Quantity,
                    n_salpri = field.Salpri,
                    n_sisa = field.Quantity
                  };

                  if (claimD1 != null)
                  {
                    db.LG_ClaimD1s.InsertOnSubmit(claimD1);
                  }
                }

                #endregion
              }
              else if ((field != null) && (!field.IsNew) && (!field.IsDelete) && (field.IsModified))
              {
                #region Modify Detil

                if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                {
                  claimD1 = (from q in db.LG_ClaimD1s
                             where q.c_claimno == ClaimID
                             && q.c_iteno == field.Item
                             && q.n_sisa == q.n_qty
                             select q).Take(1).SingleOrDefault();

                  if (claimD1 != null)
                  {
                    claimD1.n_qty = field.Quantity;
                    claimD1.n_sisa = field.Quantity;
                  }
                  else
                  {
                    result = "Claim tidak dapat diubah, karena data sudah di acc.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                      db.Transaction.Rollback();
                    }

                    goto endLogic;
                  }
                }

                #endregion
              }
            }
            if (ListclaimD3.Count > 0)
            {
              db.LG_ClaimD3s.InsertAllOnSubmit(ListclaimD3.ToArray());
              ListclaimD3.Clear();
            }
          }

          #endregion

          hasAnyChanges = true;
        }
        else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete

          claimH = (from q in db.LG_ClaimHs
                    where q.c_claimno == structure.Fields.ClaimID
                    select q).Take(1).SingleOrDefault();

          if (string.IsNullOrEmpty(ClaimID))
          {
            result = "Nomor Claim ID dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }
          }
          else if (Commons.IsClosingFA(db, claimH.d_claimdate))
          {
            result = "Claim tidak dapat dihapus, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          claimH = (from q in db.LG_ClaimHs
                    where q.c_claimno == ClaimID
                    select q).Take(1).SingleOrDefault();

          if (claimH == null)
          {
            result = "Nomor Claim tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (claimH.l_delete.HasValue && claimH.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor Claim yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          if (this.HasClaimAcc(db, ClaimID))
          {
            result = "Claim yang sudah Terdapat Di Claim ACC tidak dapat diubah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          claimH.c_update = nipEntry;
          claimH.d_update = DateTime.Now;

          claimH.l_delete = true;
          claimH.v_ket_mark = structure.Fields.Keterangan;

          #region Delete

          listClaimD1 = new List<LG_ClaimD1>();
          listClaimD2 = new List<LG_ClaimD2>();
          ListclaimD3 = new List<LG_ClaimD3>();

          listClaimD1 = (from q in db.LG_ClaimD1s
                         where q.c_claimno == ClaimID
                         select q).ToList();

          if (listClaimD1.Count > 0)
          {
            for (nLoop = 0; nLoop < listClaimD1.Count; nLoop++)
            {
              //claimD1 = (from q in db.LG_ClaimD1s
              //           where q.c_iteno == listClaimD1[nLoop].c_iteno
              //          && q.c_claimno == ClaimID && listClaimD1[nLoop].n_qty == listClaimD1[nLoop].n_sisa
              //           select q).Take(1).SingleOrDefault();

              claimD1 = listClaimD1[nLoop];

              if ((claimD1 != null) && ((claimD1.n_qty.HasValue ? claimD1.n_qty.Value : 0) == (claimD1.n_sisa.HasValue ? claimD1.n_sisa.Value : 0)))
              {
                listClaimD2 = (from q in db.LG_ClaimD2s
                               where q.c_iteno == listClaimD1[nLoop].c_iteno
                               && q.c_claimno == ClaimID
                               select q).ToList();

                #region Adding CLaim 3

                if (listClaimD2.Count > 0)
                {
                  for (nLoopC = 0; nLoopC < listClaimD2.Count; nLoopC++)
                  {
                    claimD2 = listClaimD2[nLoopC];

                    ListclaimD3.Add(new LG_ClaimD3()
                    {
                      c_claimno = ClaimID,
                      c_iteno = listClaimD2[nLoopC].c_iteno,
                      n_qty = listClaimD1[nLoop].n_qty,
                      c_entry = structure.Fields.Entry,
                      d_entry = DateTime.Now,
                      n_disc = listClaimD1[nLoop].n_disc,
                      n_salpri = listClaimD1[nLoop].n_salpri,
                      n_sisa = listClaimD1[nLoop].n_sisa,
                      v_ket_del = "",
                      v_type = "03",
                      c_cusno = listClaimD2[nLoopC].c_cusno
                    });
                  }

                  db.LG_ClaimD2s.DeleteAllOnSubmit(listClaimD2.ToArray());
                  listClaimD2.Clear();
                }
                else
                {
                  ListclaimD3.Add(new LG_ClaimD3()
                  {
                    c_claimno = ClaimID,
                    c_iteno = listClaimD1[nLoop].c_iteno,
                    n_qty = listClaimD1[nLoop].n_qty,
                    c_entry = structure.Fields.Entry,
                    d_entry = DateTime.Now,
                    n_disc = listClaimD1[nLoop].n_disc,
                    n_salpri = listClaimD1[nLoop].n_salpri,
                    n_sisa = listClaimD1[nLoop].n_sisa,
                    v_ket_del = "",
                    v_type = "03",
                  });
                }

                #endregion

                db.LG_ClaimD1s.DeleteOnSubmit(claimD1);
              }
              else
              {
                result = "Tidak dapat menghapus nomor Claim yang Terpakai.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                if (db.Transaction != null)
                {
                  db.Transaction.Rollback();
                }

                goto endLogic;
              }
            }

            if (ListclaimD3.Count > 0)
            {
              db.LG_ClaimD3s.InsertAllOnSubmit(ListclaimD3.ToArray());
              ListclaimD3.Clear();
            }
          }

          #endregion

          #endregion

          hasAnyChanges = true;
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

        result = string.Format("ScmsSoaLibrary.Bussiness.Bonus:ClaimBonus - {0}", ex.Message);

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

    public string ClaimBonusProcess(ScmsSoaLibrary.Parser.Class.ClaimStructureProcess structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false;
      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      LG_ClaimH claimH = null;
      LG_ClaimD1 claimD1 = null;
      LG_ClaimD2 claimD2 = null;


      List<LG_ClaimD1> listClaimD1 = null;
      List<LG_ClaimD2> listClaimD2 = null, ListStringClaim = null;
      List<string> ListStringD1 = null;
      List<LG_ClaimD3> ListclaimD3 = null;
      FA_DiscD discD = null;

      ScmsSoaLibrary.Parser.Class.ClaimStructureProcessField field = null;
      string nipEntry = null;
      string ClaimID = null;
      //string tmpNumbering = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      decimal nQtyAcc = 0,
        nFADisc = 0;

      int nLoop = 0, totalDetails = 0,
        nLoopC = 0;

      IDictionary<string, string> dic = null;

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }

      ClaimID = (structure.Fields.ClaimID ?? string.Empty);

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          if (!string.IsNullOrEmpty(ClaimID))
          {
            result = "Nomor Claim harus kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          if (string.IsNullOrEmpty(structure.Fields.Tahun))
          {
            result = "Tahun dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          if (string.IsNullOrEmpty(structure.Fields.Bulan))
          {
            result = "Bulan dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          if (string.IsNullOrEmpty(structure.Fields.Suplier))
          {
            result = "Nama pemasok dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingFA(db, date))
          {
            result = "Claim tidak dapat disimpan, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "CLAIM");

          ClaimID = Commons.GenerateNumberingFormat<LG_ClaimH>(db, "/MS/FA/KD/", '3', "13", date, "c_claimno", "claim");

          claimH = new LG_ClaimH()
          {
            c_claimno = ClaimID,
            c_kurs = (structure.Fields.KursDesc == null ? "01" : structure.Fields.KursDesc),
            n_kurs = Convert.ToDecimal(structure.Fields.KursVal == null ? "0" : structure.Fields.KursVal),
            c_entry = nipEntry,
            c_nosup = structure.Fields.Suplier,
            c_type = structure.Fields.Type,
            c_update = nipEntry,
            d_claimdate = DateTime.Now,
            d_top = DateTime.Now.AddDays(Convert.ToDouble(structure.Fields.Top)),
            d_entry = date,
            l_status = false,
            n_bilva = 0,
            n_bruto = 0,
            n_disc = 0,
            n_tax = 0,
            s_tahun = Convert.ToInt16(structure.Fields.Tahun),
            t_bulan = Convert.ToByte(structure.Fields.Bulan),
            n_top = Convert.ToDecimal(structure.Fields.Top),
            d_update = date,
            l_print = false,
            v_ket = structure.Fields.Keterangan
          };

          db.LG_ClaimHs.InsertOnSubmit(claimH);

          #region Insert Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listClaimD1 = new List<LG_ClaimD1>();
            listClaimD2 = new List<LG_ClaimD2>();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              nQtyAcc = field.Quantity - (field.nBret + field.nGret);

              listClaimD2.Add(new LG_ClaimD2()
              {
                c_claimno = ClaimID,
                c_iteno = field.Item,
                c_cusno = field.cusno,
                n_qty = field.Quantity,
                n_acc = nQtyAcc,
                n_tolak = 0,
              });

              if (listClaimD1.Where(x=>x.c_iteno.Equals(field.Item)).Count() > 0)
              {
                claimD1 = listClaimD1.Find(delegate(LG_ClaimD1 claim1)
                {
                  return field.Item.Equals((string.IsNullOrEmpty(claim1.c_iteno) ? string.Empty : claim1.c_iteno), StringComparison.OrdinalIgnoreCase);
                });

                claimD1.n_qty += nQtyAcc;
                claimD1.n_sisa += nQtyAcc;
              }
              else
              {
                nFADisc = (from q in db.FA_DiscDs
                           where q.c_iteno == field.Item
                           && q.c_nodisc == "DSXXXXXX01"
                           select q.n_discon.HasValue ? q.n_discon.Value : 0).SingleOrDefault();

                claimD1 = new LG_ClaimD1()
                {
                  c_claimno = ClaimID,
                  c_iteno = field.Item,
                  n_disc = nFADisc,
                  n_qty = nQtyAcc,
                  n_salpri = field.Salpri,
                  n_sisa = nQtyAcc,
                };

                listClaimD1.Add(claimD1);
              }

              totalDetails++;
            }

            db.LG_ClaimD1s.InsertAllOnSubmit(listClaimD1.ToArray());
            listClaimD1.Clear();

          }


          #endregion

          dic = new Dictionary<string, string>();

          dic.Add("CLAIM", ClaimID);
          dic.Add("Tanggal", date.ToString("yyyyMMdd"));

          result = string.Format("Total {0} detail(s)", totalDetails);

          #endregion

          hasAnyChanges = true;
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

        result = string.Format("ScmsSoaLibrary.Bussiness.Bonus:ClaimBonusProccess - {0}", ex.Message);

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

  class ClaimBonusAccSupplier
  {
    private bool HasRN(ORMDataContext db, string ClaimBonusAccID)
    {
      string tmp = (from q in db.LG_RND2s
                    where q.c_no == ClaimBonusAccID //&& q.l_delete == false
                    select q.c_no).Take(1).SingleOrDefault();

      return (!string.IsNullOrEmpty(tmp));
    }

    public string ClaimBonusAcc(ScmsSoaLibrary.Parser.Class.ClaimAccStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false;
      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      LG_ClaimAccH claimAccH = null;
      LG_ClaimAccD claimAccD = null;

      LG_ClaimD1 LG_ClaimD1 = null;

      List<LG_ClaimAccD> listClaimAccD = null;
      List<string> ListStringD1 = null;
      List<LG_ClaimAccD2> ListclaimAccD2 = null;
      List<LG_ClaimD1> ListClaimD1 = null;

      ScmsSoaLibrary.Parser.Class.ClaimAccStructureField field = null;
      string nipEntry = null;
      string ClaimAccID = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now,
        datePrins = DateTime.Now;
      
      int nLoop = 0, 
        totalDetails = 0;

      IDictionary<string, string> dic = null;

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }

      ClaimAccID = (structure.Fields.ID ?? string.Empty);

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          if (!string.IsNullOrEmpty(ClaimAccID))
          {
            result = "Nomor Claim Acc harus kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          if (string.IsNullOrEmpty(structure.Fields.TglPrinsipal))
          {
            result = "Tgl Claim Prinsipal dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          if (string.IsNullOrEmpty(structure.Fields.NoPrinsipal))
          {
            result = "No DO Prinsipal dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          if (string.IsNullOrEmpty(structure.Fields.Suplier))
          {
            result = "Nama pemasok dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingFA(db, date))
          {
            result = "Claim acc tidak dapat disimpan, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "CLAIMACC");

          ClaimAccID = Commons.GenerateNumbering<LG_ClaimAccH>(db, "CA", '3', "24", date, "c_claimaccno");

          if (!string.IsNullOrEmpty(structure.Fields.TglPrinsipal))
          {
            datePrins = Convert.ToDateTime(structure.Fields.TglPrinsipal);
          }
          
          claimAccH = new LG_ClaimAccH()
          {
            c_claimaccno = ClaimAccID,
            c_claimno = structure.Fields.claimno,
            c_entry = nipEntry,
            l_delete = false,
            c_claimnoprinc = structure.Fields.NoPrinsipal,
            d_claimaccdate = date,
            d_claimdateprinc = datePrins,
            c_nosup = structure.Fields.Suplier,
            c_type = structure.Fields.Type,
            c_update = nipEntry,
            d_entry = date,
            d_update = date,
            v_ket = structure.Fields.Keterangan
          };

          db.LG_ClaimAccHes.InsertOnSubmit(claimAccH);

          #region

          //db.SubmitChanges();

          //claimAccH = (from q in db.LG_ClaimAccHes
          //          where q.v_ket == tmpNumbering
          //          select q).Take(1).SingleOrDefault();

          //if (!string.IsNullOrEmpty(ClaimAccID))
          //{
          //  result = "Nomor Claim Acc tidak dapat di raih.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}

          //if (claimAccH.c_claimno.Equals("XXXXXXXXXX"))
          //{
          //  result = "Trigger Claim Acc tidak aktif.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}

          //claimAccH.v_ket = structure.Fields.Keterangan;

          //ClaimAccID = claimAccH.c_claimaccno;

          #endregion

          #region Insert Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listClaimAccD = new List<LG_ClaimAccD>();
            LG_ClaimD1 = new LG_ClaimD1();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              listClaimAccD.Add(new LG_ClaimAccD()
                {
                  c_claimaccno = ClaimAccID,
                  c_iteno = field.Item,
                  n_qtyacc = field.qtyAcc,
                  n_qtytolak = field.qtyTolak,
                  n_sisa = field.qtyAcc
                });

              LG_ClaimD1 = (from q in db.LG_ClaimD1s
                            where q.c_claimno == structure.Fields.claimno
                            && q.c_iteno == field.Item
                            select q).Take(1).SingleOrDefault();

              LG_ClaimD1.n_sisa -= field.qtyAcc;

              totalDetails++;

            }
            if (listClaimAccD.Count > 0)
            {
              db.LG_ClaimAccDs.InsertAllOnSubmit(listClaimAccD.ToArray());
              listClaimAccD.Clear();
            }
          }

          #endregion

          dic = new Dictionary<string, string>();

          dic.Add("CLAIMACC", ClaimAccID);
          dic.Add("Tanggal", date.ToString("yyyyMMdd"));
          dic.Add("Tanggal_Prinsipal", datePrins.ToString("yyyyMMdd"));

          result = string.Format("Total {0} detail(s)", totalDetails);

          #endregion

          hasAnyChanges = true;
        }
        if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify

          listClaimAccD = new List<LG_ClaimAccD>();
          ListStringD1 = new List<string>();
          ListclaimAccD2 = new List<LG_ClaimAccD2>();
          claimAccH = new LG_ClaimAccH();
          LG_ClaimD1 = new LG_ClaimD1();

          claimAccH = (from q in db.LG_ClaimAccHes
                       where q.c_claimaccno == ClaimAccID
                       select q).Take(1).SingleOrDefault();

          claimAccH.c_update = structure.Fields.Entry;
          claimAccH.d_update = date;

          if (Commons.IsClosingFA(db, claimAccH.d_claimaccdate))
          {
            result = "Claim Acc tidak dapat diubah, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {


            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              if ((field != null) && (!field.IsNew) && field.IsDelete && (!field.IsModified))
              {
                #region Delete Detil

                claimAccD = (from q in db.LG_ClaimAccDs
                             where q.c_claimaccno == ClaimAccID
                             && q.c_iteno == field.Item && q.n_qtyacc == q.n_sisa
                             select q).Take(1).SingleOrDefault();

                if (claimAccD != null)
                {
                  LG_ClaimD1 = (from q in db.LG_ClaimD1s
                                where q.c_claimno == structure.Fields.claimno
                                && q.c_iteno == field.Item
                                select q).SingleOrDefault();

                  if (LG_ClaimD1 != null)
                  {
                    LG_ClaimD1.n_sisa += field.qtyAcc;

                    ListclaimAccD2.Add(new LG_ClaimAccD2()
                    {
                      c_claimaccno = ClaimAccID,
                      c_iteno = claimAccD.c_iteno,
                      n_qtyacc = claimAccD.n_qtyacc,
                      n_qtytolak = claimAccD.n_qtyacc,
                      n_sisa = claimAccD.n_sisa,
                      v_ket_del = field.KeteranganMod,
                      v_type = "02"
                    });

                    db.LG_ClaimAccDs.DeleteOnSubmit(claimAccD);
                  }

                }
                #endregion
              }
              else if ((field != null) && (field.IsNew) && (!field.IsDelete) && (!field.IsModified))
              {
                #region Add

                claimAccD = new LG_ClaimAccD()
                {
                  c_claimaccno = ClaimAccID,
                  c_iteno = field.Item,
                  n_qtyacc = field.qtyAcc,
                  n_qtytolak = field.qtyTolak,
                  n_sisa = field.qtyAcc
                };

                if (claimAccD != null)
                {
                  LG_ClaimD1 = (from q in db.LG_ClaimD1s
                                where q.c_claimno == structure.Fields.claimno
                                && q.c_iteno == field.Item
                                select q).Take(1).SingleOrDefault();

                  LG_ClaimD1.n_sisa -= field.qtyAcc;

                  db.LG_ClaimAccDs.InsertOnSubmit(claimAccD);
                }

                totalDetails++;

                #endregion
              }

              if (ListclaimAccD2.Count > 0)
              {
                db.LG_ClaimAccD2s.InsertAllOnSubmit(ListclaimAccD2.ToArray());
                ListclaimAccD2.Clear();
              }
            }
          }

          #endregion

          hasAnyChanges = true;
        }
        if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete

          if (string.IsNullOrEmpty(ClaimAccID))
          {
            result = "Nomor Claim Acc dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }

          claimAccH = (from q in db.LG_ClaimAccHes
                       where q.c_claimaccno == ClaimAccID
                       select q).Take(1).SingleOrDefault();

          if (claimAccH == null)
          {
            result = "Nomor Claim Acc tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (claimAccH.l_delete.HasValue && claimAccH.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor Claim Acct yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingFA(db, claimAccH.d_claimaccdate))
          {
            result = "Claim tidak dapat dihapus, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          claimAccH.c_update = nipEntry;
          claimAccH.d_update = DateTime.Now;

          claimAccH.l_delete = true;
          claimAccH.v_ket_mark = structure.Fields.Keterangan;

          ListclaimAccD2 = new List<LG_ClaimAccD2>();
          ListClaimD1 = new List<LG_ClaimD1>();
          LG_ClaimD1 = new LG_ClaimD1();

          listClaimAccD = (from q in db.LG_ClaimAccDs
                           where q.c_claimaccno == ClaimAccID
                           select q).ToList();

          if ((listClaimAccD != null) && (listClaimAccD.Count > 0))
          {
            for (nLoop = 0; nLoop < listClaimAccD.Count; nLoop++)
            {
              claimAccD = (from q in db.LG_ClaimAccDs
                           where q.c_iteno == listClaimAccD[nLoop].c_iteno
                           && q.c_claimaccno == ClaimAccID
                           && listClaimAccD[nLoop].n_qtyacc == listClaimAccD[nLoop].n_sisa
                           select q).Take(1).SingleOrDefault();

              if (claimAccD != null)
              {
                LG_ClaimD1 = (from q in db.LG_ClaimD1s
                              where q.c_iteno == claimAccD.c_iteno
                              && q.c_claimno == claimAccH.c_claimno
                              select q).Take(1).SingleOrDefault();

                LG_ClaimD1.n_sisa += (claimAccD.n_qtyacc);

                #region Insert CLaim Acc D2

                ListclaimAccD2.Add(new LG_ClaimAccD2()
                {
                  c_claimaccno = ClaimAccID,
                  c_iteno = claimAccD.c_iteno,
                  n_qtyacc = claimAccD.n_qtyacc,
                  n_qtytolak = claimAccD.n_qtytolak,
                  n_sisa = claimAccD.n_sisa,
                  v_ket_del = structure.Fields.Keterangan,
                  v_type = "03"
                });

                #endregion

                db.LG_ClaimAccDs.DeleteOnSubmit(claimAccD);
              }
              else
              {
                result = "Tidak Dapat Menhapus.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                if (db.Transaction != null)
                {
                  db.Transaction.Rollback();
                }

                goto endLogic;
              }
            }

            if (ListclaimAccD2.Count > 0)
            {
              db.LG_ClaimAccD2s.InsertAllOnSubmit(ListclaimAccD2.ToArray());
              ListclaimAccD2.Clear();
            }
          }

          #endregion

          hasAnyChanges = true;
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
        }
      }
      catch (Exception ex)
      {
        if (db.Transaction != null)
        {
          db.Transaction.Rollback();
        }

        result = string.Format("ScmsSoaLibrary.Bussiness.Penjualan:ClaimAcc - {0}", ex.Message);

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