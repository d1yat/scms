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
  class AdjustFaktur
  {
    public string AdjustFak(ScmsSoaLibrary.Parser.Class.AdjustFakturStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false;
      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      ScmsSoaLibrary.Parser.Class.AdjustFakturStructureField field = null;
      string nipEntry = null;
      string adjID = null;
      //string tmpNumbering = null;

      int nLoop = 0,
        nLoopC = 0;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      List<LG_AdjustTransD1> ListTransD1 = null;
      List<LG_AdjustTransD> ListTransD = null;
      LG_AdjustTransD lg_transd = null;

      LG_FJRH fjrh = null;
      LG_FJH fjh = null;
      LG_adjustTransH transH = null;

      IDictionary<string, string> dic = null;

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }

      adjID = (structure.Fields.AdjustFakturID ?? string.Empty);

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "ADJFB");

          if (Commons.IsClosingFA(db, date))
          {
            result = "Adjustment tidak dapat disimpan, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          if (structure.Fields.Type == "05")
          {
            adjID = Commons.GenerateNumbering<LG_adjustTransH>(db, "AB", '3', "29", date, "c_adjno");
          }
          else
          {
            adjID = Commons.GenerateNumbering<LG_adjustTransH>(db, "AB", '3', "31", date, "c_adjno");
          }

          transH = new  LG_adjustTransH()
          {
            c_adjno = adjID,
            c_entry = structure.Fields.Entry,
            c_type = structure.Fields.Type,
            c_update = structure.Fields.Entry,
            d_adjdate = DateTime.Now,
            c_subtype = structure.Fields.SubType,
            c_beban = structure.Fields.Beban,
            d_entry = date,
            d_update = date,
            l_delete = false,
            v_ket = structure.Fields.Keterangan
          };

          db.LG_adjustTransHes.InsertOnSubmit(transH);

          #region Detil

          if (structure.Fields.SubType == "01")
          {
            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
            {
              ListTransD = new List<LG_AdjustTransD>();
              fjh = new LG_FJH();

              for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
              {
                field = structure.Fields.Field[nLoop];

                if ((field != null) && field.IsNew && (!field.IsDelete))
                {
                  ListTransD.Add(new LG_AdjustTransD()
                  {
                    c_adjno = adjID,
                    n_value = field.Value,
                    v_ket = field.Ket,
                    c_noref = field.noRef
                  });

                  fjh = (from q in db.LG_FJHs
                         where q.c_fjno == field.noRef
                         select q).Take(1).SingleOrDefault();

                  fjh.n_sisa -= field.Value;
                }
              }

              if (ListTransD != null)
              {
                db.LG_AdjustTransDs.InsertAllOnSubmit(ListTransD.ToArray());
                ListTransD.Clear();
              }
            }
          }
          else
          {
            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
            {
              ListTransD = new List<LG_AdjustTransD>();
              fjrh = new LG_FJRH();

              for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
              {
                field = structure.Fields.Field[nLoop];

                if ((field != null) && field.IsNew && (!field.IsDelete))
                {
                  ListTransD.Add(new LG_AdjustTransD()
                  {
                    c_adjno = adjID,
                    n_value = field.Value,
                    v_ket = field.KetDet,
                    c_noref = field.noRef
                  });

                  fjrh = (from q in db.LG_FJRHs
                         where q.c_fjno == field.noRef
                         select q).Take(1).SingleOrDefault();

                  fjrh.n_sisa -= field.Value;
                }
              }

              if (ListTransD != null)
              {
                db.LG_AdjustTransDs.InsertAllOnSubmit(ListTransD.ToArray());
                ListTransD.Clear();
              }
            }
          }
          #endregion

          dic = new Dictionary<string, string>();

          dic.Add("ADJ", adjID);
          dic.Add("Tanggal", date.ToString("yyyyMMdd"));

          #endregion

          hasAnyChanges = true;
        }
        if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify

          if (string.IsNullOrEmpty(adjID))
          {
            result = "Nomor Adjustment dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          

          transH = (from q in db.LG_adjustTransHes
                  where q.c_adjno == adjID
                  select q).Take(1).SingleOrDefault();

          if (transH == null)
          {
            result = "Nomor Adjustment tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, transH.d_adjdate))
          {
            result = "Adjustment tidak dapat diubah, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (transH.l_delete.HasValue && transH.l_delete.Value)
          {
            result = "Tidak dapat mengubah Nomor Adjustment yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
          {
            transH.v_ket = structure.Fields.Keterangan;
          }

          transH.c_update = nipEntry;
          transH.d_update = DateTime.Now;

          #region Modify

          if (structure.Fields.SubType == "01")
          {
            #region FJ

            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
            {
              ListTransD = new List<LG_AdjustTransD>();
              ListTransD1 = new List<LG_AdjustTransD1>();

              for (nLoopC = 0; nLoopC < structure.Fields.Field.Length; nLoopC++)
              {
                field = structure.Fields.Field[nLoopC];

                if ((field.IsNew) && !(field.IsModified) && !(field.IsDelete))
                {
                  lg_transd = new LG_AdjustTransD()
                  {
                    c_adjno = adjID,
                    n_value = field.Value,
                    v_ket = field.KetDet,
                    c_noref = field.noRef
                  };

                  db.LG_AdjustTransDs.InsertOnSubmit(lg_transd);

                  fjh = (from q in db.LG_FJHs
                         where q.c_fjno == field.noRef
                         select q).Take(1).SingleOrDefault();

                  fjh.n_sisa -= field.Value;

                  
                }
                else if (!(field.IsNew) && !(field.IsModified) && (field.IsDelete))
                {
                  lg_transd = (from q in db.LG_AdjustTransDs
                               where q.c_adjno == adjID &&
                               q.c_noref == field.noRef
                               select q).Take(1).SingleOrDefault();

                  ListTransD1.Add(new LG_AdjustTransD1()
                  {
                    c_adjno = adjID,
                    c_gdg = ListTransD[nLoop].c_gdg,
                    c_iteno = ListTransD[nLoop].c_iteno,
                    c_noref = ListTransD[nLoop].c_noref,
                    c_type = ListTransD[nLoop].c_type,
                    d_date = ListTransD[nLoop].d_date,
                    n_qty = ListTransD[nLoop].n_qty,
                    n_value = ListTransD[nLoop].n_value,
                    v_ket = ListTransD[nLoop].v_ket,
                    v_ket_del = structure.Fields.KetDel,
                    v_type_del = "02"
                  });

                  fjh = (from q in db.LG_FJHs
                         where q.c_fjno == field.noRef
                         select q).Take(1).SingleOrDefault();

                  fjh.n_sisa += field.Value;

                  db.LG_AdjustTransDs.DeleteOnSubmit(lg_transd);
                }
              }

              if (ListTransD1.Count > 0)
              {
                db.LG_AdjustTransD1s.InsertAllOnSubmit(ListTransD1.ToArray());
              }
            }

            #endregion
          }
          else
          {
            #region FJR

            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
            {
              ListTransD = new List<LG_AdjustTransD>();
              ListTransD1 = new List<LG_AdjustTransD1>();

              for (nLoopC = 0; nLoopC < structure.Fields.Field.Length; nLoopC++)
              {
                field = structure.Fields.Field[nLoopC];

                if ((field.IsNew) && !(field.IsModified) && !(field.IsDelete))
                {
                  lg_transd = new LG_AdjustTransD()
                  {
                    c_adjno = adjID,
                    n_value = field.Value,
                    v_ket = field.KetDet,
                    c_noref = field.noRef
                  };

                  db.LG_AdjustTransDs.InsertOnSubmit(lg_transd);

                  fjrh = (from q in db.LG_FJRHs
                         where q.c_fjno == field.noRef
                         select q).Take(1).SingleOrDefault();

                  fjrh.n_sisa -= field.Value;
                }
                else if (!(field.IsNew) && !(field.IsModified) && (field.IsDelete))
                {
                  lg_transd = (from q in db.LG_AdjustTransDs
                                where q.c_adjno == adjID &&
                                q.c_noref == field.noRef
                                select q).Take(1).SingleOrDefault();

                  ListTransD1.Add(new LG_AdjustTransD1()
                  {
                    c_adjno = adjID,
                    c_gdg = ListTransD[nLoop].c_gdg,
                    c_iteno = ListTransD[nLoop].c_iteno,
                    c_noref = ListTransD[nLoop].c_noref,
                    c_type = ListTransD[nLoop].c_type,
                    d_date = ListTransD[nLoop].d_date,
                    n_qty = ListTransD[nLoop].n_qty,
                    n_value = ListTransD[nLoop].n_value,
                    v_ket = ListTransD[nLoop].v_ket,
                    v_ket_del = structure.Fields.KetDel,
                    v_type_del = "02"
                  });

                  fjrh = (from q in db.LG_FJRHs
                          where q.c_fjno == field.noRef
                          select q).Take(1).SingleOrDefault();

                  fjrh.n_sisa += field.Value;

                  db.LG_AdjustTransDs.DeleteOnSubmit(lg_transd);
                }
              }

              if (ListTransD1.Count > 0)
              {
                db.LG_AdjustTransD1s.InsertAllOnSubmit(ListTransD1.ToArray());
              }
            }
            #endregion
          }

          #endregion

          #endregion

          hasAnyChanges = true;
        }
        if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete

          if (string.IsNullOrEmpty(adjID))
          {
            result = "Nomor Adjustment dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          transH = (from q in db.LG_adjustTransHes
                    where q.c_adjno == adjID
                    select q).Take(1).SingleOrDefault();

          if (transH == null)
          {
            result = "Nomor Adjustment tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, transH.d_adjdate))
          {
            result = "Adjustment tidak dapat dihapus, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (transH.l_delete.HasValue && transH.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor Adjustment yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          transH.c_update = nipEntry;
          transH.d_update = DateTime.Now;

          transH.l_delete = true;
          transH.v_ket_del = structure.Fields.Keterangan;

          
          ListTransD = new List<LG_AdjustTransD>();
          lg_transd = new LG_AdjustTransD();

          ListTransD = (from q in db.LG_AdjustTransDs
                        where q.c_adjno == adjID
                        select q).ToList();

          if ((ListTransD != null) && (ListTransD.Count > 0))
          {
            ListTransD1 = new List<LG_AdjustTransD1>();

            field = new ScmsSoaLibrary.Parser.Class.AdjustFakturStructureField();

            for (nLoopC = 0; nLoopC < ListTransD.Count; nLoopC++)
            {

              ListTransD1.Add(new LG_AdjustTransD1()
              {
                c_adjno = ListTransD[nLoopC].c_adjno,
                c_gdg = ListTransD[nLoopC].c_gdg,
                c_iteno = ListTransD[nLoopC].c_iteno,
                c_noref = ListTransD[nLoopC].c_noref,
                c_type = ListTransD[nLoopC].c_type,
                d_date = ListTransD[nLoopC].d_date,
                n_qty = ListTransD[nLoopC].n_qty,
                n_value = ListTransD[nLoopC].n_value,
                v_ket = ListTransD[nLoopC].v_ket,
                v_ket_del = structure.Fields.KetDel,
                v_type_del = "03"
              });

              if (transH.c_subtype.Equals("01"))
              {
                fjh = (from q in db.LG_FJHs
                       where q.c_fjno.Trim() == ListTransD[nLoop].c_noref.Trim()
                       select q).Take(1).SingleOrDefault();

                fjh.n_sisa += field.Value;
              }
              else
              {
                fjrh = (from q in db.LG_FJRHs
                       where q.c_fjno.Trim() == ListTransD[nLoop].c_noref.Trim()
                       select q).Take(1).SingleOrDefault();

                fjrh.n_sisa += field.Value;
              }
            }
          }
          if (ListTransD1 != null && ListTransD1.Count > 0)
          {
            db.LG_AdjustTransD1s.InsertAllOnSubmit(ListTransD1.ToArray());
            ListTransD1.Clear();
          }
          if (ListTransD != null && ListTransD.Count > 0)
          {
            db.LG_AdjustTransDs.DeleteAllOnSubmit(ListTransD.ToArray());
            ListTransD.Clear();
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

        result = string.Format("ScmsSoaLibrary.Bussiness.adjustFaktur:FakturJual - {0}", ex.Message);

        Logger.WriteLine(result);
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

    public string AdjustFakBeli(ScmsSoaLibrary.Parser.Class.AdjustFakturStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false;
      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      ScmsSoaLibrary.Parser.Class.AdjustFakturStructureField field = null;
      string nipEntry = null;
      string adjID = null;
      //string tmpNumbering = null;

      int nLoop = 0,
        nLoopC = 0;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      List<LG_AdjustTransD1> ListTransD1 = null;
      List<LG_AdjustTransD> ListTransD = null;
      LG_AdjustTransD lg_transd = null;

      LG_FBRH fbrh = null;
      LG_FBH fbh = null;
      LG_adjustTransH transH = null;

      IDictionary<string, string> dic = null;

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }

      adjID = (structure.Fields.AdjustFakturID ?? string.Empty);

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          if (Commons.IsClosingFA(db, date))
          {
            result = "Adjustment tidak dapat disimpan, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

            adjID = Commons.GenerateNumbering<LG_adjustTransH>(db, "FR", '3', "29", date, "c_adjno");
          

          transH = new LG_adjustTransH()
          {
            c_adjno = adjID,
            c_entry = structure.Fields.Entry,
            c_type = structure.Fields.Type,
            c_update = structure.Fields.Entry,
            d_adjdate = DateTime.Now,
            c_subtype = structure.Fields.SubType,
            c_beban = structure.Fields.Beban,
            d_entry = date,
            d_update = date,
            l_delete = false,
            v_ket = structure.Fields.Keterangan
          };

          db.LG_adjustTransHes.InsertOnSubmit(transH);

          #region Detil

          if (structure.Fields.SubType == "01")
          {
            #region FB

            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
            {
              ListTransD = new List<LG_AdjustTransD>();
              fbh = new LG_FBH();

              for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
              {
                field = structure.Fields.Field[nLoop];

                if ((field != null) && field.IsNew && (!field.IsDelete))
                {
                  ListTransD.Add(new LG_AdjustTransD()
                  {
                    c_adjno = adjID,
                    n_value = field.Value,
                    v_ket = field.KetDet,
                    c_noref = field.noRef
                  });

                  fbh = (from q in db.LG_FBHs
                         where q.c_fbno == field.noRef
                         select q).Take(1).SingleOrDefault();

                  fbh.n_sisa -= field.Value;
                }
              }

              if (ListTransD != null)
              {
                db.LG_AdjustTransDs.InsertAllOnSubmit(ListTransD.ToArray());
                ListTransD.Clear();
              }
            }
            #endregion
          }
          else
          {
            #region FBR

            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
            {
              ListTransD = new List<LG_AdjustTransD>();
              fbrh = new LG_FBRH();

              for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
              {
                field = structure.Fields.Field[nLoop];

                if ((field != null) && field.IsNew && (!field.IsDelete))
                {
                  ListTransD.Add(new LG_AdjustTransD()
                  {
                    c_adjno = adjID,
                    n_value = field.Value,
                    v_ket = field.KetDet,
                    c_noref = field.noRef
                  });

                  fbrh = (from q in db.LG_FBRHs
                          where q.c_fbno == field.noRef
                          select q).Take(1).SingleOrDefault();

                  fbrh.n_sisa -= field.Value;
                }
              }

              if (ListTransD != null)
              {
                db.LG_AdjustTransDs.InsertAllOnSubmit(ListTransD.ToArray());
                ListTransD.Clear();
              }
            }

            #endregion
          }
          #endregion

          dic = new Dictionary<string, string>();

          dic.Add("ADJ", adjID);
          dic.Add("Tanggal", date.ToString("yyyyMMdd"));

          #endregion

          hasAnyChanges = true;
        }
        if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify

          if (string.IsNullOrEmpty(adjID))
          {
            result = "Nomor Adjustment dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          transH = (from q in db.LG_adjustTransHes
                    where q.c_adjno == adjID
                    select q).Take(1).SingleOrDefault();

          if (transH == null)
          {
            result = "Nomor Adjustment tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingFA(db, transH.d_adjdate))
          {
            result = "Adjustment tidak dapat diubah, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (transH.l_delete.HasValue && transH.l_delete.Value)
          {
            result = "Tidak dapat mengubah Nomor Adjustment yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
          {
            transH.v_ket = structure.Fields.Keterangan;
          }

          transH.c_update = nipEntry;
          transH.d_update = DateTime.Now;

          #region Modify

          if (structure.Fields.SubType == "01")
          {
            #region FB

            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
            {
              ListTransD = new List<LG_AdjustTransD>();
              ListTransD1 = new List<LG_AdjustTransD1>();
              fbh = new LG_FBH();

              for (nLoopC = 0; nLoopC < structure.Fields.Field.Length; nLoopC++)
              {
                field = structure.Fields.Field[nLoopC];

                if ((field.IsNew) && !(field.IsModified) && !(field.IsDelete))
                {
                  lg_transd = new LG_AdjustTransD()
                  {
                    c_adjno = adjID,
                    n_value = field.Value,
                    v_ket = field.KetDet,
                    c_noref = field.noRef
                  };

                  fbh = (from q in db.LG_FBHs
                         where q.c_fbno == field.noRef
                         select q).Take(1).SingleOrDefault();

                  fbh.n_sisa -= field.Value;

                  db.LG_AdjustTransDs.InsertOnSubmit(lg_transd);
                }
                else if (!(field.IsNew) && !(field.IsModified) && (field.IsDelete))
                {
                  lg_transd = (from q in db.LG_AdjustTransDs
                               where q.c_adjno == adjID &&
                               q.c_noref == field.noRef
                               select q).Take(1).SingleOrDefault();

                  ListTransD1.Add(new LG_AdjustTransD1()
                  {
                    c_adjno = adjID,
                    c_gdg = ListTransD[nLoop].c_gdg,
                    c_iteno = ListTransD[nLoop].c_iteno,
                    c_noref = ListTransD[nLoop].c_noref,
                    c_type = ListTransD[nLoop].c_type,
                    d_date = ListTransD[nLoop].d_date,
                    n_qty = ListTransD[nLoop].n_qty,
                    n_value = ListTransD[nLoop].n_value,
                    v_ket = ListTransD[nLoop].v_ket,
                    v_ket_del = structure.Fields.KetDel,
                    v_type_del = "02"
                  });

                  fbh = (from q in db.LG_FBHs
                         where q.c_fbno == field.noRef
                         select q).Take(1).SingleOrDefault();

                  fbh.n_sisa += field.Value;

                  db.LG_AdjustTransDs.DeleteOnSubmit(lg_transd);
                }
              }

              if (ListTransD1.Count > 0)
              {
                db.LG_AdjustTransD1s.InsertAllOnSubmit(ListTransD1.ToArray());
              }
            }
            #endregion
          }
          else
          {
            #region FBR

            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
            {
              ListTransD = new List<LG_AdjustTransD>();
              ListTransD1 = new List<LG_AdjustTransD1>();
              fbrh = new LG_FBRH();

              for (nLoopC = 0; nLoopC < structure.Fields.Field.Length; nLoopC++)
              {
                field = structure.Fields.Field[nLoopC];

                if ((field.IsNew) && !(field.IsModified) && !(field.IsDelete))
                {
                  lg_transd = new LG_AdjustTransD()
                  {
                    c_adjno = adjID,
                    n_value = field.Value,
                    v_ket = field.KetDet,
                    c_noref = field.noRef
                  };

                  fbrh = (from q in db.LG_FBRHs
                         where q.c_fbno == field.noRef
                         select q).Take(1).SingleOrDefault();

                  fbrh.n_sisa -= field.Value;

                  db.LG_AdjustTransDs.InsertOnSubmit(lg_transd);
                }
                else if (!(field.IsNew) && !(field.IsModified) && (field.IsDelete))
                {

                  lg_transd = (from q in db.LG_AdjustTransDs
                               where q.c_adjno == adjID &&
                               q.c_noref == field.noRef
                               select q).Take(1).SingleOrDefault();

                  ListTransD1.Add(new LG_AdjustTransD1()
                  {
                    c_adjno = adjID,
                    c_gdg = ListTransD[nLoop].c_gdg,
                    c_iteno = ListTransD[nLoop].c_iteno,
                    c_noref = ListTransD[nLoop].c_noref,
                    c_type = ListTransD[nLoop].c_type,
                    d_date = ListTransD[nLoop].d_date,
                    n_qty = ListTransD[nLoop].n_qty,
                    n_value = ListTransD[nLoop].n_value,
                    v_ket = ListTransD[nLoop].v_ket,
                    v_ket_del = structure.Fields.KetDel,
                    v_type_del = "02"
                  });

                  fbrh = (from q in db.LG_FBRHs
                          where q.c_fbno == field.noRef
                          select q).Take(1).SingleOrDefault();

                  fbrh.n_sisa += field.Value;
                }
                db.LG_AdjustTransDs.DeleteOnSubmit(lg_transd);
              }

              if (ListTransD1.Count > 0)
              {
                db.LG_AdjustTransD1s.InsertAllOnSubmit(ListTransD1.ToArray());
              }
            }
            #endregion
          }

          #endregion

          #endregion

          hasAnyChanges = true;
        }
        if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete

          if (string.IsNullOrEmpty(adjID))
          {
            result = "Nomor Adjustment dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          transH = (from q in db.LG_adjustTransHes
                    where q.c_adjno == adjID
                    select q).Take(1).SingleOrDefault();

          if (transH == null)
          {
            result = "Nomor Adjustment tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingFA(db, transH.d_adjdate))
          {
            result = "Adjustment tidak dapat dihapus, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (transH.l_delete.HasValue && transH.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor Adjustment yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          transH.c_update = nipEntry;
          transH.d_update = DateTime.Now;

          transH.l_delete = true;
          transH.v_ket_del = structure.Fields.Keterangan;

          
          ListTransD = new List<LG_AdjustTransD>();
          lg_transd = new LG_AdjustTransD();

          
          ListTransD = (from q in db.LG_AdjustTransDs
                        where q.c_adjno == adjID
                        select q).ToList();

          if ((ListTransD != null) && (ListTransD.Count > 0))
          {
            ListTransD1 = new List<LG_AdjustTransD1>();

            field = new ScmsSoaLibrary.Parser.Class.AdjustFakturStructureField();

            for (nLoopC = 0; nLoopC < ListTransD.Count; nLoopC++)
            {

              ListTransD1.Add(new LG_AdjustTransD1()
              {
                c_adjno = ListTransD[nLoopC].c_adjno,
                c_gdg = ListTransD[nLoopC].c_gdg,
                c_iteno = ListTransD[nLoopC].c_iteno,
                c_noref = ListTransD[nLoopC].c_noref,
                c_type = ListTransD[nLoopC].c_type,
                d_date = ListTransD[nLoopC].d_date,
                n_qty = ListTransD[nLoopC].n_qty,
                n_value = ListTransD[nLoopC].n_value,
                v_ket = ListTransD[nLoopC].v_ket,
                v_ket_del = structure.Fields.KetDel,
                v_type_del = "03"
              });

              if (transH.c_subtype.Equals("01"))
              {
                fbh = (from q in db.LG_FBHs
                       where q.c_fbno == ListTransD[nLoop].c_noref
                       select q).Take(1).SingleOrDefault();

                fbh.n_sisa -= field.Value;
              }
              else
              {
                fbrh = (from q in db.LG_FBRHs
                       where q.c_fbno == ListTransD[nLoop].c_noref
                       select q).Take(1).SingleOrDefault();

                fbrh.n_sisa -= field.Value;
              }
            }
          }
          if (ListTransD1 != null && ListTransD1.Count > 0)
          {
            db.LG_AdjustTransD1s.InsertAllOnSubmit(ListTransD1.ToArray());
            ListTransD1.Clear();
          }
          if (ListTransD != null && ListTransD.Count > 0)
          { 
            db.LG_AdjustTransDs.DeleteAllOnSubmit(ListTransD.ToArray());
            ListTransD.Clear();
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

          rpe = ResponseParser.ResponseParserEnum.IsError;
        }
      }
      catch (Exception ex)
      {
        if (db.Transaction != null)
        {
          db.Transaction.Rollback();
        }

        result = string.Format("ScmsSoaLibrary.Bussiness.adjustFaktur:FakturBeli - {0}", ex.Message);

        Logger.WriteLine(result);
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

  class AdjustVoucher
  {
    public string AdjVoucher(ScmsSoaLibrary.Parser.Class.AdjustFakturStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false;
      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      ScmsSoaLibrary.Parser.Class.AdjustFakturStructureField field = null;
      string nipEntry = null;
      string adjID = null;
      //string tmpNumbering = null;

      int nLoop = 0,
        nLoopC = 0;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      List<LG_AdjustTransD1> ListTransD1 = null;
      List<LG_AdjustTransD> ListTransD = null;
      LG_AdjustTransD lg_transd = null;

      LG_VCNH vcnh = null;
      LG_VDNH vdnh = null;
      LG_adjustTransH transH = null;

      IDictionary<string, string> dic = null;

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }

      adjID = (structure.Fields.AdjustFakturID ?? string.Empty);

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "ADJFB");

          if (Commons.IsClosingFA(db, date))
          {
            result = "Adjustment tidak dapat disimpan, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          adjID = Commons.GenerateNumbering<LG_adjustTransH>(db, "AV", '3', "31", date, "c_adjno");

          transH = new LG_adjustTransH()
          {
            c_adjno = adjID,
            c_entry = structure.Fields.Entry,
            c_type = structure.Fields.Type,
            c_update = structure.Fields.Entry,
            d_adjdate = DateTime.Now,
            c_subtype = structure.Fields.SubType,
            c_beban = structure.Fields.Beban,
            d_entry = date,
            d_update = date,
            l_delete = false,
            v_ket = structure.Fields.Keterangan
          };

          db.LG_adjustTransHes.InsertOnSubmit(transH);

          #region Old Code

          //db.SubmitChanges();

          //transH = (from q in db.LG_adjustTransHes
          //          where q.v_ket == tmpNumbering
          //          select q).Take(1).SingleOrDefault();

          //if (transH == null)
          //{
          //  result = "Nomor Adjustment tidak dapat di raih.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}
          //else if (transH.c_adjno.Equals("XXXXXXXXXX"))
          //{
          //  result = "Trigger Adjustment tidak aktif.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}

          //transH.v_ket = structure.Fields.Keterangan;

          //adjID = transH.c_adjno;

          #endregion

          #region Detil

          if (structure.Fields.Type == "17")
          {
            #region Detil Credit

            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
            {
              ListTransD = new List<LG_AdjustTransD>();

              vcnh = new LG_VCNH();

              for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
              {
                field = structure.Fields.Field[nLoop];

                if ((field != null) && field.IsNew && (!field.IsDelete))
                {
                  ListTransD.Add(new LG_AdjustTransD()
                  {
                    c_adjno = adjID,
                    n_value = field.Value,
                    v_ket = field.KetDet,
                    c_noref = field.noRef,
                    c_type = field.TypeDet
                  });

                  vcnh = (from q in db.LG_VCNHs
                          where q.c_vcno == field.noRef
                          select q).Take(1).SingleOrDefault();

                  vcnh.n_sisa -= field.Value;
                }
              }

              if (ListTransD != null)
              {
                db.LG_AdjustTransDs.InsertAllOnSubmit(ListTransD.ToArray());
                ListTransD.Clear();
              }

            }
            #endregion
          }
          else if (structure.Fields.Type == "12")
          {
            #region Detil Debit

            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
            {
              ListTransD = new List<LG_AdjustTransD>();

              vdnh = new LG_VDNH();

              for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
              {
                field = structure.Fields.Field[nLoop];

                if ((field != null) && field.IsNew && (!field.IsDelete))
                {
                  ListTransD.Add(new LG_AdjustTransD()
                  {
                    c_adjno = adjID,
                    n_value = field.Value,
                    v_ket = field.KetDet,
                    c_noref = field.noRef,
                    c_type = field.TypeDet
                  });

                  vdnh = (from q in db.LG_VDNHs
                          where q.c_vdno == field.noRef
                          select q).Take(1).SingleOrDefault();

                  vdnh.n_sisa -= field.Value;
                }
              }

              if (ListTransD != null)
              {
                db.LG_AdjustTransDs.InsertAllOnSubmit(ListTransD.ToArray());
                ListTransD.Clear();
              }

            }
            #endregion
          }
          

          #endregion

          dic = new Dictionary<string, string>();

          dic.Add("ADJ", adjID);
          dic.Add("Tanggal", date.ToString("yyyyMMdd"));

          #endregion

          hasAnyChanges = true;
        }
        if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify

          if (string.IsNullOrEmpty(adjID))
          {
            result = "Nomor Adjustment dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          transH = (from q in db.LG_adjustTransHes
                    where q.c_adjno == adjID
                    select q).Take(1).SingleOrDefault();

          if (transH == null)
          {
            result = "Nomor Adjustment tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingFA(db, transH.d_adjdate))
          {
            result = "Adjustment tidak dapat diubah, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (transH.l_delete.HasValue && transH.l_delete.Value)
          {
            result = "Tidak dapat mengubah Nomor Adjustment yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
          {
            transH.v_ket = structure.Fields.Keterangan;
          }

          transH.c_update = nipEntry;
          transH.d_update = DateTime.Now;


          #region Modify Delete Detil

          ListTransD = new List<LG_AdjustTransD>();
          ListTransD1 = new List<LG_AdjustTransD1>();
          vcnh = new LG_VCNH();

          if (structure.Fields.Type == "17")
          {
            #region Credit

            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
            {
              for (nLoopC = 0; nLoopC < structure.Fields.Field.Length; nLoopC++)
              {
                field = structure.Fields.Field[nLoopC];

                if ((field.IsNew) && !(field.IsModified) && !(field.IsDelete))
                {
                  lg_transd = new LG_AdjustTransD()
                  {
                    c_adjno = adjID,
                    n_value = field.Value,
                    v_ket = field.KetDet,
                    c_noref = field.noRef,
                    c_type = field.TypeDet
                  };

                  db.LG_AdjustTransDs.InsertOnSubmit(lg_transd);

                  vcnh = (from q in db.LG_VCNHs
                          where q.c_vcno == ListTransD[nLoop].c_noref
                          select q).Take(1).SingleOrDefault();

                  vcnh.n_sisa -= ListTransD[nLoop].n_value;
                  
                }
                else if (!(field.IsNew) && !(field.IsModified) && (field.IsDelete))
                {
                  lg_transd = (from q in db.LG_AdjustTransDs
                                where q.c_adjno == adjID &&
                                q.c_noref == field.noRef
                                select q).Take(1).SingleOrDefault();

                  db.LG_AdjustTransDs.DeleteOnSubmit(lg_transd);

                  ListTransD1.Add(new LG_AdjustTransD1()
                  {
                    c_adjno = adjID,
                    c_gdg = ListTransD[nLoop].c_gdg,
                    c_iteno = ListTransD[nLoop].c_iteno,
                    c_noref = ListTransD[nLoop].c_noref,
                    c_type = ListTransD[nLoop].c_type,
                    d_date = ListTransD[nLoop].d_date,
                    n_qty = ListTransD[nLoop].n_qty,
                    n_value = ListTransD[nLoop].n_value,
                    v_ket = ListTransD[nLoop].v_ket,
                    v_ket_del = field.Keterangan,
                    v_type_del = "02"
                  });

                  vcnh = (from q in db.LG_VCNHs
                          where q.c_vcno == ListTransD[nLoop].c_noref
                          select q).Take(1).SingleOrDefault();

                  vcnh.n_sisa += ListTransD[nLoop].n_value;
                }
              }
              
            }
            #endregion
          }
          else if (structure.Fields.Type == "12")
          {
            #region Debit

            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
            {
              ListTransD = new List<LG_AdjustTransD>();
              ListTransD1 = new List<LG_AdjustTransD1>();
              vdnh = new LG_VDNH();

              for (nLoopC = 0; nLoopC < structure.Fields.Field.Length; nLoopC++)
              {
                field = structure.Fields.Field[nLoopC];

                if ((field.IsNew) && !(field.IsModified) && !(field.IsDelete))
                {
                  lg_transd = new LG_AdjustTransD()
                  {
                    c_adjno = adjID,
                    n_value = field.Value,
                    v_ket = field.KetDet,
                    c_noref = field.noRef,
                    c_type = field.TypeDet
                  };

                  db.LG_AdjustTransDs.InsertOnSubmit(lg_transd);

                  vdnh = (from q in db.LG_VDNHs
                          where q.c_vdno == field.noRef
                          select q).Take(1).SingleOrDefault();

                  vdnh.n_sisa -= field.Value;
                }
                else if (!(field.IsNew) && !(field.IsModified) && (field.IsDelete))
                {
                  lg_transd = (from q in db.LG_AdjustTransDs
                               where q.c_adjno == adjID &&
                               q.c_noref == field.noRef
                               select q).Take(1).SingleOrDefault();

                  db.LG_AdjustTransDs.DeleteOnSubmit(lg_transd);

                  ListTransD1.Add(new LG_AdjustTransD1()
                  {
                    c_adjno = adjID,
                    c_iteno = ListTransD[nLoop].c_iteno,
                    c_noref = ListTransD[nLoop].c_noref,
                    c_type = ListTransD[nLoop].c_type,
                    d_date = ListTransD[nLoop].d_date,
                    n_qty = ListTransD[nLoop].n_qty,
                    n_value = ListTransD[nLoop].n_value,
                    v_ket = ListTransD[nLoop].v_ket,
                    v_ket_del = field.Keterangan,
                    v_type_del = "02"
                  });

                  vdnh = (from q in db.LG_VDNHs
                          where q.c_vdno == ListTransD[nLoop].c_noref
                          select q).Take(1).SingleOrDefault();

                  vdnh.n_sisa += ListTransD[nLoop].n_value;
                }
              }
            }
            #endregion
          }

          if (ListTransD1.Count > 0 && ListTransD1 != null)
          {
            db.LG_AdjustTransD1s.InsertAllOnSubmit(ListTransD1.ToArray());
            ListTransD1.Clear();
          }

          #endregion

          #endregion

          hasAnyChanges = true;
        }
        if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete

          if (string.IsNullOrEmpty(adjID))
          {
            result = "Nomor Adjustment dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }

          transH = (from q in db.LG_adjustTransHes
                    where q.c_adjno == adjID
                    select q).Take(1).SingleOrDefault();

          if (transH == null)
          {
            result = "Nomor Adjustment tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }
          else if (Commons.IsClosingFA(db, transH.d_adjdate))
          {
            result = "Adjustment tidak dapat dihapus, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (transH.l_delete.HasValue && transH.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor Adjustment yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }

          transH.c_update = nipEntry;
          transH.d_update = DateTime.Now;

          transH.l_delete = true;
          transH.v_ket_del = structure.Fields.Keterangan;


          ListTransD = new List<LG_AdjustTransD>();
          lg_transd = new LG_AdjustTransD();

          ListTransD = (from q in db.LG_AdjustTransDs
                        where q.c_adjno == adjID
                        select q).ToList();

          if (transH.c_type == "17")
          {
            #region Credit

            if ((ListTransD != null) && (ListTransD.Count > 0))
            {
              ListTransD1 = new List<LG_AdjustTransD1>();
              vcnh = new LG_VCNH();

              for (nLoopC = 0; nLoopC < ListTransD.Count; nLoopC++)
              {

                ListTransD1.Add(new LG_AdjustTransD1()
                {
                  c_adjno = ListTransD[nLoopC].c_adjno,
                  c_noref = ListTransD[nLoopC].c_noref,
                  c_type = ListTransD[nLoopC].c_type,
                  d_date = ListTransD[nLoopC].d_date,
                  n_value = ListTransD[nLoopC].n_value,
                  v_ket = ListTransD[nLoopC].v_ket,
                  v_ket_del = structure.Fields.Keterangan,
                  v_type_del = "03"
                });

                lg_transd = (from q in db.LG_AdjustTransDs
                             where q.c_adjno == adjID
                             select q).Take(1).SingleOrDefault();

                vcnh = (from q in db.LG_VCNHs
                        where q.c_vcno == ListTransD[nLoopC].c_noref
                        select q).Take(1).SingleOrDefault();

                vcnh.n_sisa += ListTransD[nLoopC].n_value;
                db.SubmitChanges();
              }
              if (ListTransD1 != null && ListTransD1.Count > 0)
              {
                db.LG_AdjustTransD1s.InsertAllOnSubmit(ListTransD1.ToArray());
                db.LG_AdjustTransDs.DeleteAllOnSubmit(ListTransD.ToArray());
                db.SubmitChanges();
                ListTransD.Clear();
                ListTransD1.Clear();
              }
            }
            #endregion
          }
          if (transH.c_type == "12")
          {
            #region Debit

            if ((ListTransD != null) && (ListTransD.Count > 0))
            {
              ListTransD1 = new List<LG_AdjustTransD1>();
              vdnh = new LG_VDNH();

              for (nLoopC = 0; nLoopC < ListTransD.Count; nLoopC++)
              {

                ListTransD1.Add(new LG_AdjustTransD1()
                {
                  c_adjno = ListTransD[nLoopC].c_adjno,
                  c_noref = ListTransD[nLoopC].c_noref,
                  c_type = ListTransD[nLoopC].c_type,
                  d_date = ListTransD[nLoopC].d_date,
                  n_value = ListTransD[nLoopC].n_value,
                  v_ket = ListTransD[nLoopC].v_ket,
                  v_ket_del = structure.Fields.Keterangan,
                  v_type_del = "03"
                });

                lg_transd = (from q in db.LG_AdjustTransDs
                             where q.c_adjno == adjID
                             select q).Take(1).SingleOrDefault();

                vdnh = (from q in db.LG_VDNHs
                        where q.c_vdno == ListTransD[nLoopC].c_noref
                        select q).Take(1).SingleOrDefault();

                vdnh.n_sisa += ListTransD[nLoopC].n_value;
                db.SubmitChanges();
              }
              if (ListTransD1 != null && ListTransD1.Count > 0)
              {
                db.LG_AdjustTransD1s.InsertAllOnSubmit(ListTransD1.ToArray());
                db.LG_AdjustTransDs.DeleteAllOnSubmit(ListTransD.ToArray());
                db.SubmitChanges();
                ListTransD.Clear();
                ListTransD1.Clear();
              }
            }
            #endregion
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

        result = string.Format("ScmsSoaLibrary.Bussiness.Adjustment:Voucher - {0}", ex.Message);

        Logger.WriteLine(result);
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
