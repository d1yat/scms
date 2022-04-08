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
  class MK
  {
      internal class BASPBClassComponent
      {
          public string sjno { get; set; }
          public string Item { get; set; }
          public decimal? Qty { get; set; }
      }

    public string MemoCombo(ScmsSoaLibrary.Parser.Class.MKMemoComboStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false;
      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      MK_MemoH memoh = null;

      ScmsSoaLibrary.Parser.Class.MKMemoComboStructureField field = null;
      string nipEntry = null;
      string memoID = null;
      //string tmpNumbering = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      //decimal? spQty = 0,
      //  nPrice = 0;

      List<MK_MemoD> listMemoD = null;
      List<MK_MemoD1> listMemoD1 = null;

      MK_MemoD memod = null;
      //MK_MemoD1 memod1 = null;

      int nLoop = 0;

      IDictionary<string, string> dic = null;

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }
      int totalDetails = 0;

      memoID = (structure.Fields.MemoID ?? string.Empty);

      char gdg = (string.IsNullOrEmpty(structure.Fields.Gudang) ? char.MinValue : structure.Fields.Gudang[0]);

      if (gdg == char.MinValue)
      {
        result = "Gudang tidak boleh kosong.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          if (!string.IsNullOrEmpty(memoID))
          {
            result = "Nomor Memo Combo harus kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.MemoRequest))
          {
            result = "Nomor Memo harus terisi.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, date))
          {
            result = "Memo tidak dapat disimpan, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          nLoop = (from q in db.MK_MemoHs
                   where (q.c_gdg == gdg) && (q.c_memo == structure.Fields.MemoRequest)
                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                   select q).Count();
          if (nLoop > 0)
          {
            result = "Nomor memo telah ada.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "MKMECO");

          memoID = Commons.GenerateNumbering<MK_MemoH>(db, "MC", '7', "02", date, "c_memono");

          memoh = new MK_MemoH()
          {
            c_entry = nipEntry,
            c_gdg = gdg,
            c_memo = structure.Fields.MemoRequest,
            c_memono = memoID,
            c_update = nipEntry,
            d_entry = date,
            d_memodate = date,
            d_update = date,
            l_print = false,
            l_status = false,
            v_ket1 = structure.Fields.Keterangan1,
            v_ket2 = structure.Fields.Keterangan2,
          };

          db.MK_MemoHs.InsertOnSubmit(memoh);

          #region Old Code

          //db.SubmitChanges();

          //memoh = (from q in db.MK_MemoHs
          //       where (q.v_ket2 == tmpNumbering) && (q.c_gdg == gdg)
          //       select q).Take(1).SingleOrDefault();

          //if (memoh == null)
          //{
          //  result = "Nomor Memo Combo tidak dapat di raih.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}
          //else if (memoh.c_memono.Equals("XXXXXXXXXX"))
          //{
          //  result = "Trigger Memo Combo tidak aktif.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}

          //memoh.v_ket2 = structure.Fields.Keterangan2;

          //memoID = memoh.c_memono;

          #endregion

          #region Insert Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listMemoD = new List<MK_MemoD>();
            
            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0) && (field.Acceptance > 0))
              {
                listMemoD.Add(new MK_MemoD()
                {
                  c_gdg = gdg,
                  c_memono = memoID,
                  c_iteno = field.Item,
                  n_acc = field.Acceptance,
                  n_box = field.BoxQuantity,
                  n_qty = field.Quantity,
                  n_sisa = field.Quantity
                });

                totalDetails++;
              }
            }

            if (listMemoD.Count > 0)
            {
              db.MK_MemoDs.InsertAllOnSubmit(listMemoD.ToArray());

              listMemoD.Clear();
            }

          }

          #endregion

          dic = new Dictionary<string, string>();

          if (totalDetails > 0)
          {
            dic.Add("MEMO", memoID);
            dic.Add("Tanggal", date.ToString("yyyyMMdd"));

            result = string.Format("Total {0} detail(s)", totalDetails);

            hasAnyChanges = true;
          }

          #endregion
        }
        else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify
          
          if (string.IsNullOrEmpty(memoID))
          {
            result = "Nomor Memo dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }

          memoh = (from q in db.MK_MemoHs
                 where q.c_memono == memoID && q.c_gdg == gdg
                 select q).Take(1).SingleOrDefault();

          if (memoh == null)
          {
            result = "Nomor Memo tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (memoh.l_delete.HasValue && memoh.l_delete.Value)
          {
            result = "Tidak dapat mengubah Memo yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, memoh.d_memodate))
          {
            result = "Memo tidak dapat diubah, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Keterangan2))
          {
            memoh.v_ket2 = structure.Fields.Keterangan2;
          }

          memoh.c_update = nipEntry;
          memoh.d_update = DateTime.Now;
                    
          #region Populate Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listMemoD1 = new List<MK_MemoD1>();

            listMemoD = (from q in db.MK_MemoDs
                        where q.c_gdg == gdg && q.c_memono == memoID
                        select q).ToList();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified) && (field.Quantity > 0) && (field.Acceptance > 0))
              {
                #region New

                memod = new MK_MemoD()
                {
                  c_gdg = gdg,
                  c_memono = memoID,
                  c_iteno = field.Item,
                  n_acc = field.Acceptance,
                  n_box = field.BoxQuantity,
                  n_qty = field.Quantity,
                  n_sisa = field.Quantity
                };

                listMemoD1.Add(new MK_MemoD1()
                {
                  c_entry = nipEntry,
                  c_gdg = gdg,
                  c_iteno = field.Item,
                  c_memono = memoID,
                  d_entry = date,
                  n_acc = field.Acceptance,
                  n_box = field.BoxQuantity,
                  n_qty = field.Quantity,
                  n_sisa = field.Quantity,
                  v_type = "01"
                });

                db.MK_MemoDs.InsertOnSubmit(memod);

                #endregion
              }
              else if ((field != null) && (!field.IsNew) && field.IsDelete && (!field.IsModified))
              {
                #region Delete

                memod = listMemoD.Find(delegate(MK_MemoD memo)
                {
                  if ((!string.IsNullOrEmpty(memo.c_iteno)) && (memo.c_iteno.Trim().Equals(field.Item.Trim(), StringComparison.OrdinalIgnoreCase)))
                  {
                    return true;
                  }

                  return false;
                });

                if (memod != null)
                {
                  listMemoD1.Add(new MK_MemoD1()
                  {
                    c_entry = nipEntry,
                    c_gdg = gdg,
                    c_iteno = field.Item,
                    c_memono = memoID,
                    d_entry = date,
                    n_acc = memod.n_acc,
                    n_box = memod.n_box,
                    n_qty = memod.n_qty,
                    n_sisa = memod.n_sisa,
                    v_ket_del = (string.IsNullOrEmpty(field.Keterangan) ? "Human error" : field.Keterangan),
                    v_type = "03"
                  });

                  db.MK_MemoDs.DeleteOnSubmit(memod);
                }

                #endregion
              }
            }

            listMemoD.Clear();

            if (listMemoD1.Count > 0)
            {
              db.MK_MemoD1s.InsertAllOnSubmit(listMemoD1.ToArray());

              listMemoD1.Clear();
            }
          }

          #endregion

          hasAnyChanges = true;

          #endregion
        }
        else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete
          
          if (string.IsNullOrEmpty(memoID))
          {
            result = "Nomor Memo dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          memoh = (from q in db.MK_MemoHs
                   where q.c_memono == memoID && q.c_gdg == gdg
                   select q).Take(1).SingleOrDefault();

          if (memoh == null)
          {
            result = "Nomor Memo tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (memoh.l_delete.HasValue && memoh.l_delete.Value)
          {
            result = "Tidak dapat menghapus Memo yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, memoh.d_memodate))
          {
            result = "Memo tidak dapat dihapus, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          memoh.c_update = nipEntry;
          memoh.d_update = DateTime.Now;

          memoh.l_delete = true;
          memoh.v_ket_mark = structure.Fields.Keterangan;

          listMemoD1 = new List<MK_MemoD1>();

          listMemoD = (from q in db.MK_MemoDs
                       where q.c_gdg == gdg && q.c_memono == memoID
                       select q).ToList();

          if ((listMemoD != null) && (listMemoD.Count > 0))
          {
            for (nLoop = 0; nLoop < listMemoD.Count; nLoop++)
            {
              memod = listMemoD[nLoop];
              if (memod != null)
              {
                listMemoD1.Add(new MK_MemoD1()
                {
                  c_entry = nipEntry,
                  c_gdg = gdg,
                  c_iteno = memod.c_iteno,
                  c_memono = memoID,
                  d_entry = date,
                  n_acc = memod.n_acc,
                  n_box = memod.n_box,
                  n_qty = memod.n_qty,
                  n_sisa = memod.n_sisa,
                  v_ket_del = (string.IsNullOrEmpty(structure.Fields.Keterangan) ? "Human error" : structure.Fields.Keterangan),
                  v_type = "03"
                });
              }
            }

            if (listMemoD1.Count > 0)
            {
              db.MK_MemoD1s.InsertAllOnSubmit(listMemoD1.ToArray());

              listMemoD1.Clear();
            }

            db.MK_MemoDs.DeleteAllOnSubmit(listMemoD.ToArray());
            
            listMemoD.Clear();
          }

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

        result = string.Format("ScmsSoaLibrary.Bussiness.MK:MemoCombo - {0}", ex.Message);

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

    public string MemoSTT(ScmsSoaLibrary.Parser.Class.MKMemoSTTStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false;
      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      ScmsSoaLibrary.Parser.Class.MKMemoSTTStructureField field = null;
      string nipEntry = null;
      string memoID = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      MK_MTH mth = null;
      List<MK_MTD> listMemoD = null;
      List<MK_MTD1> listMemoD1 = null;
      List<FA_MasItm> listItem = null;

      List<string> lstItemStr = null;

      MK_MTD mtd = null;
      FA_MasItm item = null;

      int nLoop = 0;

      IDictionary<string, string> dic = null;

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }
      int totalDetails = 0;

      memoID = (structure.Fields.MemoID ?? string.Empty);

      char gdg = (string.IsNullOrEmpty(structure.Fields.Gudang) ? char.MinValue : structure.Fields.Gudang[0]);

      if (gdg == char.MinValue)
      {
        result = "Gudang tidak boleh kosong.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          if (!string.IsNullOrEmpty(memoID))
          {
            result = "Nomor Memo STT harus kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.MemoRequest))
          {
            result = "Nomor Memo harus terisi.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Nip))
          {
            result = "Nama Karyawan harus terisi.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, date))
          {
            result = "Memo tidak dapat disimpan, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "MK");

          memoID = Commons.GenerateNumbering<MK_MTH>(db, "MT", '7', "01", date, "c_mtno");

          HR_MsKry kry = new HR_MsKry();

          kry = (from q in db.HR_MsKries
                 where q.c_nip == structure.Fields.Nip
                 select q).Take(1).SingleOrDefault();

          mth = new MK_MTH()
          {
            c_mtno = memoID,
            c_cab = kry.c_cab,
            c_nip = structure.Fields.Nip,
            c_type = structure.Fields.Type,
            c_memo = structure.Fields.MemoRequest,
            c_gdg = gdg,
            c_entry = structure.Fields.Entry,
            c_jab = kry.c_jab,
            c_update = structure.Fields.Entry,
            d_entry = date,
            d_mtdate = date,
            d_update = date,
            l_delete = false,
            l_print = false,
            l_status = false,
            v_ket = structure.Fields.Keterangan
          };

          db.MK_MTHs.InsertOnSubmit(mth);

          #region Old Code

          //db.SubmitChanges();

          //mth = (from q in db.MK_MTHs
          //       where (q.v_ket == tmpNumbering) && (q.c_gdg == gdg)
          //       select q).Take(1).SingleOrDefault();

          //if (mth == null)
          //{
          //  result = "Nomor Memo STT tidak dapat di raih.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}
          //else if (mth.c_mtno.Equals("XXXXXXXXXX"))
          //{
          //  result = "Trigger Memo STT tidak aktif.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}

          //mth.v_ket = structure.Fields.Keterangan;

          //memoID = mth.c_mtno;

          #endregion

          #region Detil

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listMemoD = new List<MK_MTD>();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0))
              {
                var Item = (from q in db.FA_MasItms
                            where q.c_iteno == field.Item
                            select q).Take(1).SingleOrDefault();

                if (Item != null)
                {
                  listMemoD.Add(new MK_MTD()
                  {
                    c_gdg = gdg,
                    c_mtno = memoID,
                    c_iteno = field.Item,
                    n_qty = field.Quantity,
                    n_sisa = field.Quantity,
                    n_disc = Item.n_disc,
                    n_salpri = Item.n_salpri
                  });
                }

                totalDetails++;
              }
            }
          }
          if (listMemoD.Count > 0)
          {
            db.MK_MTDs.InsertAllOnSubmit(listMemoD.ToArray());

            listMemoD.Clear();
          }

          #endregion

          dic = new Dictionary<string, string>();

          dic.Add("MEMO", memoID);
          dic.Add("Tanggal", date.ToString("yyyyMMdd"));

          result = string.Format("Total {0} detail(s)", totalDetails);

          hasAnyChanges = true;

          #endregion
        }
        else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify

          if (string.IsNullOrEmpty(memoID))
          {
            result = "Nomor Memo dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }

          mth = (from q in db.MK_MTHs
                   where q.c_mtno == memoID && q.c_gdg == gdg
                   select q).Take(1).SingleOrDefault();

          if (mth == null)
          {
            result = "Nomor Memo tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (mth.l_delete.HasValue && mth.l_delete.Value)
          {
            result = "Tidak dapat mengubah Memo yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, mth.d_mtdate))
          {
            result = "Memo tidak dapat diubah, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
          {
            mth.v_ket = structure.Fields.Keterangan;
          }

          mth.c_update = nipEntry;
          mth.d_update = DateTime.Now;

          #region Populate Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listMemoD1 = new List<MK_MTD1>();
            
            lstItemStr = structure.Fields.Field.GroupBy(x => x.Item).Select(y => y.Key).ToList();

            listItem = (from q in db.FA_MasItms
                        //where q.c_iteno == field.Item
                        where lstItemStr.Contains(q.c_iteno)
                        select q).ToList();

            listMemoD = (from q in db.MK_MTDs
                         where q.c_mtno == memoID
                          && q.n_qty == q.n_sisa
                          //&& q.c_iteno == field.Item
                         select q).ToList();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified) && (field.Quantity > 0))
              {
                #region Add

                //var Item = (from q in db.FA_MasItms
                //            where q.c_iteno == field.Item
                //            select q).Take(1).SingleOrDefault();

                item = listItem.Find(delegate(FA_MasItm itm)
                {
                  return (string.IsNullOrEmpty(itm.c_iteno) ? string.Empty : itm.c_iteno).Equals(field.Item, StringComparison.OrdinalIgnoreCase);
                });

                if (item != null)
                {
                  mtd = new MK_MTD()
                  {
                    c_gdg = gdg,
                    c_mtno = memoID,
                    c_iteno = field.Item,
                    n_qty = field.Quantity,
                    n_sisa = field.Quantity,
                    n_disc = item.n_disc,
                    n_salpri = item.n_salpri
                  };

                  db.MK_MTDs.InsertOnSubmit(mtd);
                  
                  listMemoD1.Add(new MK_MTD1()
                  {
                    c_mtno = memoID,
                    c_iteno = field.Item,
                    n_salpri = item.n_salpri,
                    n_disc = item.n_disc,
                    n_qty = field.Quantity,
                    n_sisa = field.Quantity,
                    c_type = "01",
                    c_entry = nipEntry,
                    d_entry = date
                  });
                }

                #endregion
              }
              if ((field != null) && (!field.IsNew) && (field.IsDelete) && (!field.IsModified))
              {
                #region Delete

                if (listMemoD.Count > 0)
                {
                  //var SinMKD = listMemoD.SingleOrDefault();

                  //listMemoD1.Add(new MK_MTD1()
                  //{
                  //  c_entry = structure.Fields.Entry,
                  //  c_iteno = field.Item,
                  //  c_mtno = memoID,
                  //  c_type = "03",
                  //  d_entry = date,
                  //  n_disc = SinMKD.n_disc,
                  //  n_qty = SinMKD.n_qty,
                  //  n_salpri = SinMKD.n_salpri,
                  //  n_sisa = SinMKD.n_sisa
                  //});

                  mtd = listMemoD.Find(delegate(MK_MTD mt)
                  {
                    return (string.IsNullOrEmpty(mt.c_iteno) ? string.Empty : mt.c_iteno).Equals(field.Item, StringComparison.OrdinalIgnoreCase);
                  });

                  if(mtd != null)
                  {
                    db.MK_MTDs.DeleteOnSubmit(mtd);

                    listMemoD1.Add(new MK_MTD1()
                    {
                      c_mtno = memoID,
                      c_iteno = field.Item,
                      n_salpri = mtd.n_salpri,
                      n_disc = mtd.n_disc,
                      n_qty = mtd.n_qty,
                      n_sisa = mtd.n_sisa,
                      c_type = "03",
                      c_entry = nipEntry,
                      d_entry = date,
                      v_ket_del = (string.IsNullOrEmpty(field.Keterangan) ? "Human error" : field.Keterangan)
                    });
                  }
                }
                #endregion
              }
            }

            if (listMemoD.Count > 0)
            {
              listMemoD.Clear();
            }

            if (listMemoD1.Count > 0)
            {
              db.MK_MTD1s.InsertAllOnSubmit(listMemoD1.ToArray());
              
              listMemoD1.Clear();
            }
          }
          #endregion

          hasAnyChanges = true;

          #endregion
        }
        else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete

          if (string.IsNullOrEmpty(memoID))
          {
            result = "Nomor Memo dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          mth = (from q in db.MK_MTHs
                   where q.c_mtno == memoID && q.c_gdg == gdg
                   select q).Take(1).SingleOrDefault();

          if (mth == null)
          {
            result = "Nomor Memo tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (mth.l_delete.HasValue && mth.l_delete.Value)
          {
            result = "Tidak dapat menghapus Memo yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, mth.d_mtdate))
          {
            result = "Memo tidak dapat dihapus, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          mth.c_update = nipEntry;
          mth.d_update = DateTime.Now;

          mth.l_delete = true;
          mth.v_ket_mark = structure.Fields.Keterangan;

          listMemoD1 = new List<MK_MTD1>();
          listMemoD = new List<MK_MTD>();

          listMemoD = (from q in db.MK_MTDs
                       where q.c_gdg == gdg 
                        && q.c_mtno == memoID
                       select q).ToList();

          if ((listMemoD != null) && (listMemoD.Count > 0))
          {
            for (nLoop = 0; nLoop < listMemoD.Count; nLoop++)
            {
              mtd = listMemoD[nLoop];

              //if (listMemoD[nLoop].n_qty != listMemoD[nLoop].n_sisa)
              if ((mtd.n_qty.HasValue ? mtd.n_qty.Value : 0) != (mtd.n_sisa.HasValue ? mtd.n_sisa.Value : 0))
              {
                result = "Tidak dapat menghapus Memo yang Item sudah terpakai.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                if (db.Transaction != null)
                {
                  db.Transaction.Rollback();
                }

                goto endLogic;
              }
              else
              {
                listMemoD1.Add(new MK_MTD1()
                {
                  c_entry = structure.Fields.Entry,
                  c_iteno = mtd.c_iteno,
                  c_mtno = mtd.c_mtno,
                  c_type = "03",
                  n_disc = mtd.n_disc,
                  d_entry = date,
                  n_qty = mtd.n_qty,
                  n_salpri = mtd.n_salpri,
                  n_sisa = mtd.n_sisa,
                  v_ket_del = (string.IsNullOrEmpty(structure.Fields.Keterangan) ? "Human error" : structure.Fields.Keterangan)
                });
              }
            }

            if (listMemoD1.Count > 0)
            {
              db.MK_MTD1s.InsertAllOnSubmit(listMemoD1.ToArray());

              listMemoD1.Clear();
            }

            db.MK_MTDs.DeleteAllOnSubmit(listMemoD.ToArray());

            listMemoD.Clear();
          }

          hasAnyChanges = true;

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
        rpe = ResponseParser.ResponseParserEnum.IsError;

        result = string.Format("ScmsSoaLibrary.Bussiness.MK:MemoSTT - {0}", ex.Message);

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

    public string MemoPemusnahan(ScmsSoaLibrary.Parser.Class.MKMemoPemusnahanStructure structure)
    {
        if ((structure == null) || (structure.Fields == null))
        {
            return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
        }

        bool hasAnyChanges = false;
        string result = null;

        ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

        ScmsSoaLibrary.Parser.Class.MKMemoPemusnahanStructureField field = null;
        string nipEntry = null;
        string memoID = null;

        ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
        DateTime date = DateTime.Now;

        MK_MPH mph = null;
        List<MK_MPD> listMemoD = null;
        List<MK_MPD1> listMemoD1 = null;
        List<FA_MasItm> listItem = null;

        List<string> lstItemStr = null;

        MK_MPD mpd = null;
        FA_MasItm item = null;

        int nLoop = 0;

        IDictionary<string, string> dic = null;

        nipEntry = (structure.Fields.Entry ?? string.Empty);

        if (string.IsNullOrEmpty(nipEntry))
        {
            result = "Nip penanggung jawab dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
        }
        int totalDetails = 0;

        memoID = (structure.Fields.MemoID ?? string.Empty);

        char gdg = (string.IsNullOrEmpty(structure.Fields.Gudang) ? char.MinValue : structure.Fields.Gudang[0]);

        if (gdg == char.MinValue)
        {
            result = "Gudang tidak boleh kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
        }

        try
        {
            db.Connection.Open();

            db.Transaction = db.Connection.BeginTransaction();

            if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
            {
                #region Add

                if (!string.IsNullOrEmpty(memoID))
                {
                    result = "Nomor Memo Pemusnahan harus kosong.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (string.IsNullOrEmpty(structure.Fields.MemoRequest))
                {
                    result = "Nomor Memo harus terisi.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (Commons.IsClosingLogistik(db, date))
                {
                    result = "Memo tidak dapat disimpan, karena sudah closing.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "MK");

                memoID = Commons.GenerateNumbering<MK_MPH>(db, "MP", '7', "05", date, "c_mpno");

                HR_MsKry kry = new HR_MsKry();
                             
                mph = new MK_MPH()
                {
                    c_mpno = memoID,
                    c_memo = structure.Fields.MemoRequest,
                    c_gdg = gdg,
                    d_mpdate = date,
                    c_entry = structure.Fields.Entry,
                    c_update = structure.Fields.Entry,
                    d_entry = date,
                    d_update = date,
                    l_delete = false,
                    l_print = false,
                    l_status = false,
                    v_ket = structure.Fields.Keterangan
                };

                db.MK_MPHs.InsertOnSubmit(mph);

                #region Detil

                if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                {
                    listMemoD = new List<MK_MPD>();

                    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    {
                        field = structure.Fields.Field[nLoop];

                        if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0))
                        {
                            var Item = (from q in db.FA_MasItms
                                        where q.c_iteno == field.Item
                                        select q).Take(1).SingleOrDefault();

                            if (Item != null)
                            {
                                listMemoD.Add(new MK_MPD()
                                {
                                    c_gdg = gdg,
                                    c_mpno = memoID,
                                    c_iteno = field.Item,
                                    n_qty = field.Quantity,
                                    n_sisa = field.Quantity
                                });
                            }

                            totalDetails++;
                        }
                    }
                }
                if (listMemoD.Count > 0)
                {
                    db.MK_MPDs.InsertAllOnSubmit(listMemoD.ToArray());

                    listMemoD.Clear();
                }

                #endregion

                dic = new Dictionary<string, string>();

                dic.Add("MEMO", memoID);
                dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                result = string.Format("Total {0} detail(s)", totalDetails);

                hasAnyChanges = true;

                #endregion
            }
            else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
            {
                #region Modify

                if (string.IsNullOrEmpty(memoID))
                {
                    result = "Nomor Memo Pemusnahan dibutuhkan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    db.Transaction.Rollback();

                    goto endLogic;
                }

                mph = (from q in db.MK_MPHs
                       where q.c_mpno == memoID && q.c_gdg == gdg
                       select q).Take(1).SingleOrDefault();

                if (mph == null)
                {
                    result = "Nomor Memo Pemusnahan tidak ditemukan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (mph.l_delete.HasValue && mph.l_delete.Value)
                {
                    result = "Tidak dapat mengubah Memo Pemusnahan yang sudah terhapus.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (Commons.IsClosingLogistik(db, mph.d_mpdate))
                {
                    result = "Memo Pemusnahan tidak dapat diubah, karena sudah closing.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (mph.l_status.HasValue && mph.l_status.Value)
                {
                    result = "Memo Pemusnahan tidak dapat diubah, karena sudah diproses pemindahan ke gudang musnah.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
                {
                    mph.v_ket = structure.Fields.Keterangan;
                }

                mph.c_update = nipEntry;
                mph.d_update = DateTime.Now;

                #region Populate Detail

                if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                {
                    listMemoD1 = new List<MK_MPD1>();

                    lstItemStr = structure.Fields.Field.GroupBy(x => x.Item).Select(y => y.Key).ToList();

                    listItem = (from q in db.FA_MasItms
                                where lstItemStr.Contains(q.c_iteno)
                                select q).ToList();

                    listMemoD = (from q in db.MK_MPDs
                                 where q.c_mpno == memoID
                                  && q.n_qty == q.n_sisa
                                 select q).ToList();

                    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    {
                        field = structure.Fields.Field[nLoop];

                        if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified) && (field.Quantity > 0))
                        {
                            #region Add

                            item = listItem.Find(delegate(FA_MasItm itm)
                            {
                                return (string.IsNullOrEmpty(itm.c_iteno) ? string.Empty : itm.c_iteno).Equals(field.Item, StringComparison.OrdinalIgnoreCase);
                            });

                            if (item != null)
                            {
                                mpd = new MK_MPD()
                                {
                                    c_gdg = gdg,
                                    c_mpno = memoID,
                                    c_iteno = field.Item,
                                    n_qty = field.Quantity,
                                    n_sisa = field.Quantity
                                };

                                db.MK_MPDs.InsertOnSubmit(mpd);

                                listMemoD1.Add(new MK_MPD1()
                                {
                                    c_mpno = memoID,
                                    c_iteno = field.Item,
                                    n_qty = field.Quantity,
                                    n_sisa = field.Quantity,
                                    c_entry = nipEntry,
                                    d_entry = date,
                                    v_type = "01"
                                });
                            }

                            #endregion
                        }
                        if ((field != null) && (!field.IsNew) && (field.IsDelete) && (!field.IsModified))
                        {
                            #region Delete

                            if (listMemoD.Count > 0)
                            {
                                mpd = listMemoD.Find(delegate(MK_MPD mp)
                                {
                                    return (string.IsNullOrEmpty(mp.c_iteno) ? string.Empty : mp.c_iteno).Equals(field.Item, StringComparison.OrdinalIgnoreCase);
                                });

                                if (mpd != null)
                                {
                                    db.MK_MPDs.DeleteOnSubmit(mpd);

                                    listMemoD1.Add(new MK_MPD1()
                                    {
                                        c_mpno = memoID,
                                        c_iteno = field.Item,
                                        n_qty = mpd.n_qty,
                                        n_sisa = mpd.n_sisa,
                                        c_entry = nipEntry,
                                        d_entry = date,
                                        v_type = "03",
                                        v_ket_del = (string.IsNullOrEmpty(field.Keterangan) ? "Human error" : field.Keterangan)
                                    });
                                }
                            }
                            #endregion
                        }
                    }

                    if (listMemoD.Count > 0)
                    {
                        listMemoD.Clear();
                    }

                    if (listMemoD1.Count > 0)
                    {
                        db.MK_MPD1s.InsertAllOnSubmit(listMemoD1.ToArray());

                        listMemoD1.Clear();
                    }
                }
                #endregion

                hasAnyChanges = true;

                #endregion
            }
            else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
            {
                #region Delete

                if (string.IsNullOrEmpty(memoID))
                {
                    result = "Nomor Memo Pemusnahan dibutuhkan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                mph = (from q in db.MK_MPHs
                       where q.c_mpno == memoID && q.c_gdg == gdg
                       select q).Take(1).SingleOrDefault();

                if (mph == null)
                {
                    result = "Nomor Memo Pemusnahan tidak ditemukan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (mph.l_delete.HasValue && mph.l_delete.Value)
                {
                    result = "Tidak dapat menghapus Memo Pemusnahan yang sudah terhapus.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (Commons.IsClosingLogistik(db, mph.d_mpdate))
                {
                    result = "Memo Pemusnahan tidak dapat dihapus, karena sudah closing.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (mph.l_status.HasValue && mph.l_status.Value)
                {
                    result = "Memo Pemusnahan tidak dapat dihapus, karena sudah diproses pemindahan ke gudang musnah.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                mph.c_update = nipEntry;
                mph.d_update = DateTime.Now;

                mph.l_delete = true;
                mph.v_ket_mark = structure.Fields.Keterangan;

                listMemoD1 = new List<MK_MPD1>();
                listMemoD = new List<MK_MPD>();

                listMemoD = (from q in db.MK_MPDs
                             where q.c_gdg == gdg
                              && q.c_mpno == memoID
                             select q).ToList();

                if ((listMemoD != null) && (listMemoD.Count > 0))
                {
                    for (nLoop = 0; nLoop < listMemoD.Count; nLoop++)
                    {
                        mpd = listMemoD[nLoop];

                        if ((mpd.n_qty.HasValue ? mpd.n_qty.Value : 0) != (mpd.n_sisa.HasValue ? mpd.n_sisa.Value : 0))
                        {
                            result = "Tidak dapat menghapus Memo yang Item sudah terpakai.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                        else
                        {
                            listMemoD1.Add(new MK_MPD1()
                            {
                                c_entry = structure.Fields.Entry,
                                c_iteno = mpd.c_iteno,
                                c_mpno = mpd.c_mpno,
                                d_entry = date,
                                n_qty = mpd.n_qty,
                                n_sisa = mpd.n_sisa,
                                v_type = "03",
                                v_ket_del = (string.IsNullOrEmpty(structure.Fields.Keterangan) ? "Human error" : structure.Fields.Keterangan)
                            });
                        }
                    }

                    if (listMemoD1.Count > 0)
                    {
                        db.MK_MPD1s.InsertAllOnSubmit(listMemoD1.ToArray());

                        listMemoD1.Clear();
                    }

                    db.MK_MPDs.DeleteAllOnSubmit(listMemoD.ToArray());

                    listMemoD.Clear();
                }

                hasAnyChanges = true;

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
            rpe = ResponseParser.ResponseParserEnum.IsError;

            result = string.Format("ScmsSoaLibrary.Bussiness.MK:MemoPemusnahan - {0}", ex.Message);

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

    public string MemoBASPBSJ(ScmsSoaLibrary.Parser.Class.MKMemoBASPBSJStructure structure)
    {
        if ((structure == null) || (structure.Fields == null))
        {
            return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
        }

        bool hasAnyChanges = false;
        string result = null;

        ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

        ScmsSoaLibrary.Parser.Class.MKMemoBASPBSJStructureField field = null;
        string nipEntry = null;
        string memoID = null;

        ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
        DateTime date = DateTime.Now;

        SCMS_BASPBH baspbh = null;
        SCMS_BASPBD baspbd = null;
        List<SCMS_BASPBD> listBaspbd = null;
        List<SCMS_BASPBD1> listBaspbd1 = null;

        List<BASPBClassComponent> listsjd1temp;

        int nLoop = 0;
        decimal? totalqty = 0,
            qtydif = 0;

        IDictionary<string, string> dic = null;

        nipEntry = (structure.Fields.Entry ?? string.Empty);

        char gdgtujuan = (string.IsNullOrEmpty(structure.Fields.gdgtujuan) ? char.MinValue : structure.Fields.gdgtujuan[0]);


        if (string.IsNullOrEmpty(nipEntry))
        {
            result = "Nip penanggung jawab dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
        }
        int totalDetails = 0;

        memoID = (structure.Fields.MemoID ?? string.Empty);

        try
        {
            db.Connection.Open();

            db.Transaction = db.Connection.BeginTransaction();

            if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
            {
                #region Add

                if (!string.IsNullOrEmpty(memoID))
                {
                    result = "Nomor Memo BASPB harus kosong.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "MK");

                memoID = Commons.GenerateNumbering<SCMS_BASPBH>(db, "BA", '7', "06", date, "c_baspbno");


                baspbh = new SCMS_BASPBH()
                {
                    c_baspbno = memoID,
                    c_gdg = gdgtujuan,
                    c_cusno = structure.Fields.gdgasal,
                    d_baspb = date,
                    c_dono = structure.Fields.sjno,
                    v_ket = structure.Fields.Keterangan,
                    c_entry = nipEntry,
                    d_entry = date,
                    c_update = nipEntry,
                    d_update = date,
                    l_delete = false
                };

                db.SCMS_BASPBHs.InsertOnSubmit(baspbh);

                #region Detil

                if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                {
                    listBaspbd = new List<SCMS_BASPBD>();
                    listsjd1temp = new List<BASPBClassComponent>();

                    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    {
                        field = structure.Fields.Field[nLoop];

                        if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0))
                        {
                            listsjd1temp = (from q in db.LG_SJD1s
                                            where q.c_sjno == structure.Fields.sjno
                                            && q.c_iteno == field.Item
                                            select new BASPBClassComponent()
                                            {
                                                sjno = q.c_sjno,
                                                Item = q.c_iteno,
                                                Qty = q.n_gqty
                                            }).ToList();

                            totalqty = listsjd1temp.Sum(t => t.Qty);

                            qtydif = field.Quantity - totalqty;

                            char claimtype = (string.IsNullOrEmpty(field.claimType) ? char.MinValue : field.claimType[0]);

                            if (listsjd1temp != null)
                            {
                                listBaspbd.Add(new SCMS_BASPBD()
                                {
                                    c_baspbno = memoID,
                                    c_iteno = field.Item,
                                    n_qtydo = totalqty.HasValue ? totalqty : 0,
                                    n_qtyrn = totalqty.HasValue ? totalqty : 0,
                                    n_gqty = field.Quantity,
                                    n_bqty = 0,
                                    n_qtydiff = qtydif.HasValue ? qtydif : 0,
                                    n_gsisa = field.Quantity,
                                    n_bsisa = 0,
                                    c_claimtype = claimtype
                                });
                            }
                            else
                            {
                                result = "Kode item di sj tidak ditemukan.";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }

                                goto endLogic;
                            }

                            totalDetails++;
                        }
                    }
                }
                if (listBaspbd.Count > 0)
                {
                    db.SCMS_BASPBDs.InsertAllOnSubmit(listBaspbd.ToArray());

                    listBaspbd.Clear();
                }

                #endregion

                dic = new Dictionary<string, string>();

                dic.Add("MEMO", memoID);
                dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                result = string.Format("Total {0} detail(s)", totalDetails);

                hasAnyChanges = true;

                #endregion
            }
            else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
            {
                #region Modify

                if (string.IsNullOrEmpty(memoID))
                {
                    result = "Nomor Memo BASPB dibutuhkan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    db.Transaction.Rollback();

                    goto endLogic;
                }

                baspbh = (from q in db.SCMS_BASPBHs
                       where q.c_baspbno == memoID
                       && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                       select q).Take(1).SingleOrDefault();

                if (baspbh == null)
                {
                    result = "Nomor Memo BASPB tidak ditemukan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
                {
                    baspbh.v_ket = structure.Fields.Keterangan;
                }

                baspbh.c_update = nipEntry;
                baspbh.d_update = DateTime.Now;

                #region Populate Detail

                if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                {
                    listsjd1temp = new List<BASPBClassComponent>();
                    listBaspbd1 = new List<SCMS_BASPBD1>();

                    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    {
                        field = structure.Fields.Field[nLoop];
                        listsjd1temp = new List<BASPBClassComponent>();
                        listBaspbd = new List<SCMS_BASPBD>();

                        listBaspbd = (from q in db.SCMS_BASPBDs
                                      where q.c_baspbno == memoID
                                      && q.c_iteno == field.Item
                                      select q).ToList();

                        if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified) && (field.Quantity > 0))
                        {
                            #region Add

                            char claimtype = (string.IsNullOrEmpty(field.claimType) ? char.MinValue : field.claimType[0]);

                            listsjd1temp = (from q in db.LG_SJD1s
                                            where q.c_sjno == structure.Fields.sjno
                                            && q.c_iteno == field.Item
                                            select new BASPBClassComponent()
                                            {
                                                sjno = q.c_sjno,
                                                Item = q.c_iteno,
                                                Qty = q.n_gqty
                                            }).ToList();

                            totalqty = listsjd1temp.Sum(t => t.Qty);
                            qtydif = field.Quantity - totalqty;

                            if (listBaspbd.Count == 0 && listsjd1temp.Count > 0)
                            {
                                listBaspbd.Add(new SCMS_BASPBD()
                                {
                                    c_baspbno = memoID,
                                    c_iteno = field.Item,
                                    n_qtydo = totalqty.HasValue ? totalqty : 0,
                                    n_qtyrn = totalqty.HasValue ? totalqty : 0,
                                    n_gqty = field.Quantity,
                                    n_bqty = 0,
                                    n_qtydiff = qtydif.HasValue ? qtydif : 0,
                                    n_gsisa = field.Quantity,
                                    n_bsisa = 0,
                                    c_claimtype = claimtype
                                });
                            }
                            else
                            {
                                result = "Kode item sudah ada / item di sj tidak ditemukan.";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }

                                goto endLogic;
                            }

                            #endregion
                        }
                        if ((field != null) && (!field.IsNew) && (field.IsDelete) && (!field.IsModified))
                        {
                            #region Delete

                            if (listBaspbd.Count > 0)
                            {
                                baspbd = listBaspbd.Find(delegate(SCMS_BASPBD basp)
                                {
                                    return memoID.Equals((string.IsNullOrEmpty(basp.c_baspbno) ? string.Empty : basp.c_baspbno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                      field.Item.Equals((string.IsNullOrEmpty(basp.c_iteno) ? string.Empty : basp.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase);
                                });

                                if (baspbd != null)
                                {
                                    db.SCMS_BASPBDs.DeleteOnSubmit(baspbd);

                                    listBaspbd1.Add(new SCMS_BASPBD1()
                                    {
                                        c_baspbno = memoID,
                                        c_sjno = structure.Fields.sjno,
                                        c_iteno = baspbd.c_iteno,
                                        n_qtydo = baspbd.n_qtydo,
                                        n_qtyrn = baspbd.n_qtyrn,
                                        n_gqty = baspbd.n_gqty,
                                        n_bqty = baspbd.n_bqty,
                                        n_qtydiff = baspbd.n_qtydiff,
                                        n_gsisa = baspbd.n_gsisa,
                                        n_bsisa = baspbd.n_bsisa,
                                        c_claimtype = baspbd.c_claimtype,
                                        v_ket_del = (string.IsNullOrEmpty(field.Keterangan) ? "Human error" : field.Keterangan),
                                        c_entry = nipEntry,
                                        d_entry = date
                                    });
                                }
                            }
                            #endregion
                        }
                    }

                    if (listBaspbd.Count > 0)
                    {
                        db.SCMS_BASPBDs.InsertAllOnSubmit(listBaspbd.ToArray());

                        listBaspbd.Clear();
                    }

                    if (listBaspbd1.Count > 0)
                    {
                        db.SCMS_BASPBD1s.InsertAllOnSubmit(listBaspbd1.ToArray());

                        listBaspbd1.Clear();
                    }
                    
                    if (listsjd1temp.Count > 0)
                    {
                        listsjd1temp.Clear();
                    }
                }
                #endregion

                hasAnyChanges = true;

                #endregion
            }
            else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
            {
                #region Delete

                if (string.IsNullOrEmpty(memoID))
                {
                    result = "Nomor Memo BASPB dibutuhkan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                baspbh = (from q in db.SCMS_BASPBHs
                       where q.c_baspbno == memoID
                       && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                       select q).Take(1).SingleOrDefault();

                if (baspbh == null)
                {
                    result = "Nomor Memo BASPB tidak ditemukan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                baspbh.l_delete = true;

                listBaspbd = new List<SCMS_BASPBD>();
                listBaspbd1 = new List<SCMS_BASPBD1>();

                listBaspbd = (from q in db.SCMS_BASPBDs
                             where q.c_baspbno == memoID
                             select q).ToList();

                if ((listBaspbd != null) && (listBaspbd.Count > 0))
                {
                    for (nLoop = 0; nLoop < listBaspbd.Count; nLoop++)
                    {
                        baspbd = listBaspbd[nLoop];

                        listBaspbd1.Add(new SCMS_BASPBD1()
                        {
                            c_baspbno = memoID,
                            c_sjno = structure.Fields.sjno,
                            c_iteno = baspbd.c_iteno,
                            n_qtydo = baspbd.n_qtydo,
                            n_qtyrn = baspbd.n_qtyrn,
                            n_gqty = baspbd.n_gqty,
                            n_bqty = baspbd.n_bqty,
                            n_qtydiff = baspbd.n_qtydiff,
                            n_gsisa = baspbd.n_gsisa,
                            n_bsisa = baspbd.n_bsisa,
                            c_claimtype = baspbd.c_claimtype,
                            v_ket_del = (string.IsNullOrEmpty(structure.Fields.Keterangan) ? "Human error" : structure.Fields.Keterangan),
                            c_entry = nipEntry,
                            d_entry = date
                        });
                    }

                    if (listBaspbd1.Count > 0)
                    {
                        db.SCMS_BASPBD1s.InsertAllOnSubmit(listBaspbd1.ToArray());

                        listBaspbd1.Clear();
                    }

                    db.SCMS_BASPBDs.DeleteAllOnSubmit(listBaspbd.ToArray());

                    listBaspbd.Clear();
                }

                hasAnyChanges = true;

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
            rpe = ResponseParser.ResponseParserEnum.IsError;

            result = string.Format("ScmsSoaLibrary.Bussiness.MK:MemoPemusnahan - {0}", ex.Message);

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
