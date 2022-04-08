using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsModel;
using ScmsSoaLibrary.Parser;
//using ScmsSoaLibraryInterface.Core.Crypto;
using ScmsSoaLibrary.Commons;
using System.Data.Common;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaLibrary.Core.Reports.Module
{
  class Penjualan
  {
    private bool HasDO(ORMDataContext db, string packingId)
    {
      return ((from q in db.LG_DOHs
                  where q.c_plno == packingId && q.l_delete == false
                  select q.c_dono).Count() > 0);
    }

    public string PackingList(ScmsSoaLibrary.Parser.Class.PackingListStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      string result = null;

      ORMDataContext db = new ORMDataContext();

      LG_PLH plh = null;

      LG_SPD1 spd1 = null;

      LG_RND1 rnd1 = null;

      ScmsSoaLibrary.Parser.Class.PackingListStructureField field = null;
      string nipEntry = null;
      string plID = null;
      string tmpNumbering = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      decimal? spQty = 0;
      decimal spQtyReloc = 0,
        rnQty = 0,
        spAlloc = 0;

      List<LG_PLD1> listPld1 = null;
      List<LG_PLD2> listPld2 = null;
      List<string> listRN = null;
      List<LG_RND1> listResRND1 = null;
      List<LG_PLD3> listPld3 = null;

      char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '1' : structure.Fields.Gudang[0]);

      LG_PLD1 pld1 = null;
      LG_PLD2 pld2 = null;

      int nLoop = 0,
        nLoopC = 0;

      IDictionary<string, string> dic = null;

      int totalDetails = 0;
      bool isConfirmed = false;

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }

      plID = (structure.Fields.PackingListID ?? string.Empty);

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          if (!string.IsNullOrEmpty(plID))
          {
            result = "Nomor Packing List harus kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }

          if (string.IsNullOrEmpty(structure.Fields.Suplier))
          {
            result = "Nama pemasok dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }

          if (string.IsNullOrEmpty(structure.Fields.Customer))
          {
            result = "Nama cabang dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }

          if (string.IsNullOrEmpty(structure.Fields.Gudang))
          {
            result = "Gudang dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }

          tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "PL");

          plh = new LG_PLH()
          {
            c_plno = "XXXXXXXXXX",
            c_cusno = structure.Fields.Customer,
            c_entry = nipEntry,
            c_gdg = gudang,
            c_nosup = structure.Fields.Suplier,
            c_type = structure.Fields.TypePackingList,
            c_update = nipEntry,
            c_via = structure.Fields.Via,
            d_entry = date,
            d_pldate = date.Date,
            d_update = date,
            l_confirm = false,
            l_print = false,
            v_ket = tmpNumbering
          };

          db.LG_PLHs.InsertOnSubmit(plh);

          db.SubmitChanges();

          plh = (from q in db.LG_PLHs
                 where q.v_ket == tmpNumbering
                 select q).SingleOrDefault();

          if (plh == null)
          {
            result = "Nomor Packing List tidak dapat di raih.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }

          if (plh.c_plno.Equals("XXXXXXXXXX"))
          {
            result = "Trigger Packing List tidak aktif.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }

          plh.v_ket = structure.Fields.Keterangan;

          plID = plh.c_plno;

          #region Insert Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listRN = new List<string>();
            listPld1 = new List<LG_PLD1>();
            listPld2 = new List<LG_PLD2>();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];
              if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0))
              {
                #region Cek SP

                spd1 = (from q in db.LG_SPD1s
                         join q1 in db.LG_ORD2s on new { q.c_spno, q.c_iteno } equals new { q1.c_spno, q1.c_iteno }
                         join q2 in db.LG_ORHs on q1.c_orno equals q2.c_orno
                         where q.c_spno == field.NomorSP && q.c_iteno == field.Item && q2.c_type != "02"
                         select q).Distinct().Take(1).SingleOrDefault();

                if ((spd1 != null) && (spd1.n_sisa.HasValue) && (spd1.n_sisa.Value > 0))
                {
                  if (field.Quantity > spd1.n_sisa.Value)
                  {
                    spQty = spd1.n_sisa.Value;
                  }
                  else
                  {
                    spQty = field.Quantity;
                  }

                  spQtyReloc = spQty.Value;

                  listResRND1 = (from q in db.LG_RND1s
                                 join q1 in db.LG_RNHs on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                 join q2 in db.LG_MsBatches on q.c_batch equals q2.c_batch
                                 join q3 in db.LG_RND2s on new { q.c_gdg, q.c_rnno } equals new { q3.c_gdg, q3.c_rnno }
                                 join q4 in db.LG_POD2s on new { q3.c_gdg, c_pono = q3.c_no } equals new { q4.c_gdg, q4.c_pono }
                                 join q5 in db.LG_ORHs on new { q4.c_gdg, q4.c_orno } equals new { q5.c_gdg, q5.c_orno }
                                 where q.c_gdg == gudang && q.c_batch == field.Batch && q.c_iteno == field.Item && q.n_gsisa > 0 && q5.c_type != "02"
                                 orderby q2.d_expired descending, q1.d_rndate
                                 select q).Distinct().ToList();

                  #region Cek RN

                  if (listResRND1.Count > 0)
                  {
                    for (nLoopC = 0; nLoopC < listResRND1.Count; nLoopC++)
                    {
                      rnd1 = listResRND1[nLoopC];

                      if ((rnd1.n_gsisa.HasValue) && (rnd1.n_gsisa.Value > 0))
                      {
                        if (rnd1.n_gsisa.Value >= spQtyReloc)
                        {
                          rnQty = spQtyReloc;

                          rnd1.n_gsisa -= spQtyReloc;
                          spQtyReloc = 0;
                        }
                        else
                        {
                          rnQty = rnd1.n_gsisa.Value;

                          spQtyReloc -= rnd1.n_gsisa.Value;
                          rnd1.n_gsisa = 0;
                        }

                        listPld2.Add(new LG_PLD2()
                        {
                          c_plno = plID,
                          c_batch = field.Batch,
                          c_iteno = field.Item,
                          c_rnno = rnd1.c_rnno,
                          c_spno = field.NomorSP,
                          c_type = "01",
                          n_qty = rnQty,
                          n_sisa = rnQty
                        });
                      }

                      if (spQtyReloc < 1)
                      {
                        break;
                      }
                    }

                    spAlloc = (spQty.Value - spQtyReloc);

                    listPld1.Add(new LG_PLD1()
                    {
                      c_plno = plID,
                      c_batch = field.Batch,
                      c_iteno = field.Item,
                      c_spno = field.NomorSP,
                      c_type = "01",
                      n_booked = spAlloc,
                      n_qty = spAlloc,
                      n_sisa = spAlloc
                    });
                  }

                  #endregion

                  if ((listPld1.Count > 0) && (listPld2.Count > 0))
                  {
                    if (spAlloc <= spd1.n_sisa.Value)
                    {
                      spd1.n_sisa -= spAlloc;
                    }
                    else
                    {
                      spd1.n_sisa -= 0;
                    }
                  }
                }

                if (listResRND1 != null)
                {
                  listResRND1.Clear();
                }

                #endregion
              }

              if ((listPld1.Count > 0) && (listPld2.Count > 0))
              {
                db.LG_PLD1s.InsertAllOnSubmit(listPld1.ToArray());
                db.LG_PLD2s.InsertAllOnSubmit(listPld2.ToArray());

                totalDetails++;
              }

              db.SubmitChanges();

              listPld1.Clear();
              listPld2.Clear();
            }
          }

          #endregion

          dic = new Dictionary<string, string>();

          dic.Add("PL", plID);
          dic.Add("Tanggal", date.ToString("yyyyMMdd"));

          result = string.Format("Total {0} detail(s)", totalDetails);

          #endregion
        }
        else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify

          if (string.IsNullOrEmpty(plID))
          {
            result = "Nomor Packing List dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }
          
          plh = (from q in db.LG_PLHs
                 where q.c_plno == plID
                 select q).SingleOrDefault();

          if (plh == null)
          {
            result = "Nomor Packing List tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }
          else if (plh.l_delete.HasValue && plh.l_delete.Value)
          {
            result = "Tidak dapat mengubah Packing List yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }

          if (this.HasDO(db, plID))
          {
            result = "Packing List yang sudah terdapat Delivery Order tidak dapat diubah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
          {
            plh.v_ket = structure.Fields.Keterangan;
          }

          plh.c_update = nipEntry;
          plh.d_update = DateTime.Now;

          #region Populate Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listRN = new List<string>();
            listPld1 = new List<LG_PLD1>();
            listPld2 = new List<LG_PLD2>();
            listPld3 = new List<LG_PLD3>();
            listResRND1 = new List<LG_RND1>();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified) && (field.Quantity > 0))
              {
                #region Cek SP

                spd1 = (from q in db.LG_SPD1s
                        join q1 in db.LG_ORD2s on new { q.c_spno, q.c_iteno } equals new { q1.c_spno, q1.c_iteno }
                        join q2 in db.LG_ORHs on q1.c_orno equals q2.c_orno
                        where q.c_spno == field.NomorSP && q.c_iteno == field.Item && q2.c_type != "02" 
                        select q).Distinct().Take(1).SingleOrDefault();

                if ((spd1 != null) && (spd1.n_sisa.HasValue) && (spd1.n_sisa.Value > 0))
                {
                  if (field.Quantity > spd1.n_sisa.Value)
                  {
                    spQty = spd1.n_sisa.Value;
                  }
                  else
                  {
                    spQty = field.Quantity;
                  }

                  spQtyReloc = spQty.Value;

                  listResRND1 = (from q in db.LG_RND1s
                                 join q1 in db.LG_RNHs on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                 join q2 in db.LG_MsBatches on q.c_batch equals q2.c_batch
                                 join q3 in db.LG_RND2s on new { q.c_gdg, q.c_rnno } equals new { q3.c_gdg, q3.c_rnno }
                                 join q4 in db.LG_POD2s on new { q3.c_gdg, c_pono = q3.c_no } equals new { q4.c_gdg, q4.c_pono }
                                 join q5 in db.LG_ORHs on new { q4.c_gdg, q4.c_orno } equals new { q5.c_gdg, q5.c_orno }
                                 where q.c_gdg == gudang && q.c_batch == field.Batch && q.c_iteno == field.Item && q.n_gsisa > 0 && q5.c_type != "02"
                                 orderby q2.d_expired descending, q1.d_rndate
                                 select q).Distinct().ToList();

                  #region Cek RN

                  if (listResRND1.Count > 0)
                  {
                    for (nLoopC = 0; nLoopC < listResRND1.Count; nLoopC++)
                    {
                      rnd1 = listResRND1[nLoopC];

                      if ((rnd1.n_gsisa.HasValue) && (rnd1.n_gsisa.Value > 0))
                      {
                        //rnQty = rnd1.n_gsisa.Value;

                        if (rnd1.n_gsisa.Value >= spQtyReloc)
                        {
                          rnQty = spQtyReloc;

                          rnd1.n_gsisa -= spQtyReloc;
                          spQtyReloc = 0;
                        }
                        else
                        {
                          rnQty = rnd1.n_gsisa.Value;

                          spQtyReloc -= rnd1.n_gsisa.Value;
                          rnd1.n_gsisa = 0;
                        }

                        listPld2.Add(new LG_PLD2()
                        {
                          c_plno = plID,
                          c_batch = field.Batch,
                          c_iteno = field.Item,
                          c_rnno = rnd1.c_rnno,
                          c_spno = field.NomorSP,
                          c_type = "01",
                          n_qty = rnQty,
                          n_sisa = rnQty
                        });
                      }

                      if (spQtyReloc < 1)
                      {
                        break;
                      }
                    }

                    spAlloc = (spQty.Value - spQtyReloc);

                    listPld1.Add(new LG_PLD1()
                    {
                      c_plno = plID,
                      c_batch = field.Batch,
                      c_iteno = field.Item,
                      c_spno = field.NomorSP,
                      c_type = "01",
                      n_booked = spAlloc,
                      n_qty = spAlloc,
                      n_sisa = spAlloc
                    });
                  }

                  #endregion

                  if ((listPld1.Count > 0) && (listPld2.Count > 0))
                  {
                    if (spAlloc <= spd1.n_sisa.Value)
                    {
                      spd1.n_sisa -= spAlloc;
                    }
                    else
                    {
                      spd1.n_sisa -= 0;
                    }
                  }
                }

                if (listResRND1 != null)
                {
                  listResRND1.Clear();
                }
                
                if ((listPld1.Count > 0) && (listPld2.Count > 0))
                {
                  db.LG_PLD1s.InsertAllOnSubmit(listPld1.ToArray());
                  db.LG_PLD2s.InsertAllOnSubmit(listPld2.ToArray());

                  totalDetails++;
                }

                #endregion
              }
              else if ((field != null) && (!field.IsNew) && field.IsDelete && (!field.IsModified))
              {
                #region Cek SP

                pld1 = (from q in db.LG_PLD1s
                        where q.c_batch == field.Batch && q.c_iteno == field.Item &&
                        q.c_plno == plID && q.c_spno == field.NomorSP
                        select q).Distinct().SingleOrDefault();

                if (pld1 != null)
                {
                  spQtyReloc = (pld1.n_qty.HasValue ? pld1.n_qty.Value : 0);
                  spAlloc = (pld1.n_sisa.HasValue ? pld1.n_sisa.Value : 0);

                  if (spAlloc.Equals(spQtyReloc))
                  {
                    #region Reverse SP

                    spd1 = (from q in db.LG_SPD1s
                            where q.c_spno == pld1.c_spno && q.c_iteno == pld1.c_iteno
                            select q).Distinct().SingleOrDefault();

                    if (spd1 != null)
                    {
                      spd1.n_sisa += spAlloc;
                    }

                    #endregion

                    #region Reverse RN

                    listPld2 = (from q in db.LG_PLD2s
                                where q.c_plno == plID && q.c_iteno == pld1.c_iteno &&
                                q.c_batch == pld1.c_batch
                                select q).Distinct().ToList();


                    if ((listPld2 != null) && (listPld2.Count > 0))
                    {
                      for (nLoopC = 0; nLoopC < listPld2.Count; nLoopC++)
                      {
                        pld2 = listPld2[nLoopC];

                        if (pld2 != null)
                        {
                          rnd1 = (from q in db.LG_RND1s
                                  where q.c_batch == pld2.c_batch && q.c_iteno == pld2.c_iteno &&
                                  q.c_rnno == pld2.c_rnno && q.c_gdg == plh.c_gdg
                                  select q).Distinct().SingleOrDefault();

                          rnd1.n_gsisa += (pld2.n_qty);

                          listResRND1.Add(rnd1);

                          #region Log PLD3

                          listPld3.Add(new LG_PLD3()
                          {
                            c_batch = pld2.c_batch,
                            c_iteno = pld2.c_iteno,
                            c_plno = pld2.c_plno,
                            c_rnno = pld2.c_rnno,
                            c_spno = pld2.c_spno,
                            c_type = pld2.c_type,
                            n_qty = pld2.n_qty,
                            n_sisa = pld2.n_sisa,
                            v_ket_del = field.Keterangan,
                            v_type = "03",
                            c_entry = nipEntry,
                            d_entry = DateTime.Now
                          });

                          #endregion
                        }
                      }

                      #region Delete PLD2

                      db.LG_PLD2s.DeleteAllOnSubmit(listPld2.ToArray());

                      #endregion
                    }

                    #endregion

                    #region Delete PLD1

                    db.LG_PLD1s.DeleteOnSubmit(pld1);

                    #endregion

                    #region Insert Log PLD3

                    db.LG_PLD3s.InsertAllOnSubmit(listPld3.ToArray());

                    #endregion
                  }
                }

                #endregion
              }

              db.SubmitChanges();

              listRN.Clear();
              listPld1.Clear();
              listPld2.Clear();
              listPld3.Clear();
              listResRND1.Clear();
            }
          }

          #endregion

          #endregion
        }
        else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete

          if (string.IsNullOrEmpty(plID))
          {
            result = "Nomor Packing List dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }

          plh = (from q in db.LG_PLHs
                 where q.c_plno == plID
                 select q).SingleOrDefault();

          if (plh == null)
          {
            result = "Nomor Packing List tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }
          else if (plh.l_delete.HasValue && plh.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor Packing List yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }
          
          if (this.HasDO(db, plID))
          {
            result = "Packing List yang sudah terdapat Delivery Order tidak dapat diubah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }

          plh.c_update = nipEntry;
          plh.d_update = DateTime.Now;

          plh.l_delete = true;
          plh.v_ket_mark = structure.Fields.Keterangan;

          listPld3 = new List<LG_PLD3>();
          listResRND1 = new List<LG_RND1>();

          listPld2 = (from q in db.LG_PLD2s
                      where q.c_plno == plID
                      select q).ToList();

          if ((listPld2 != null) && (listPld2.Count > 0))
          {
            for (nLoopC = 0; nLoopC < listPld2.Count; nLoopC++)
            {
              pld2 = listPld2[nLoopC];

              if (pld2 != null)
              {
                rnd1 = (from q in db.LG_RND1s
                        where q.c_batch == pld2.c_batch && q.c_iteno == pld2.c_iteno &&
                        q.c_rnno == pld2.c_rnno && q.c_gdg == plh.c_gdg
                        select q).Distinct().SingleOrDefault();

                rnd1.n_gsisa += (pld2.n_qty);

                listResRND1.Add(rnd1);

                #region Log PLD3

                listPld3.Add(new LG_PLD3()
                {
                  c_batch = pld2.c_batch,
                  c_iteno = pld2.c_iteno,
                  c_plno = pld2.c_plno,
                  c_rnno = pld2.c_rnno,
                  c_spno = pld2.c_spno,
                  c_type = pld2.c_type,
                  n_qty = pld2.n_qty,
                  n_sisa = pld2.n_sisa,
                  v_ket_del = structure.Fields.Keterangan,
                  v_type = "03",
                  c_entry = nipEntry,
                  d_entry = DateTime.Now
                });

                #endregion
              }
            }

            db.LG_PLD2s.DeleteAllOnSubmit(listPld2.ToArray());

            listPld2.Clear();
          }

          if (listPld3.Count > 0)
          {
            db.LG_PLD3s.InsertAllOnSubmit(listPld3.ToArray());

            listPld3.Clear();
          }

          listResRND1.Clear();

          db.SubmitChanges();

          #endregion
        }
        else if (structure.Method.Equals("ModifyConfirm", StringComparison.OrdinalIgnoreCase))
        {
          #region ModifyConfirm

          if (string.IsNullOrEmpty(plID))
          {
            result = "Nomor Packing List dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }

          plh = (from q in db.LG_PLHs
                 where q.c_plno == plID
                 select q).SingleOrDefault();

          if (plh == null)
          {
            result = "Nomor Packing List tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }
          else if (plh.l_delete.HasValue && plh.l_delete.Value)
          {
            result = "Tidak dapat mengubah Packing List yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }
          else if (this.HasDO(db, plID))
          {
            result = "Packing List yang sudah terdapat Delivery Order tidak dapat diubah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }

          //isConfirmed = ((((plh.l_confirm == null) || (!plh.l_confirm.Value)) && structure.Fields.IsConfirm) ? true :
          //  ((plh.l_confirm.Value && (!structure.Fields.IsConfirm)) ? true : false));

          isConfirmed = (plh.l_confirm.HasValue ? plh.l_confirm.Value : false);

          if (isConfirmed && structure.Fields.IsConfirm)
          {
            if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
            {
              plh.v_ket = structure.Fields.Keterangan;
            }

            plh.c_update = nipEntry;
            plh.d_update = DateTime.Now;

            #region Populate Detail

            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
            {
              listRN = new List<string>();
              listPld1 = new List<LG_PLD1>();
              listPld2 = new List<LG_PLD2>();
              listPld3 = new List<LG_PLD3>();
              listResRND1 = new List<LG_RND1>();

              for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
              {
                field = structure.Fields.Field[nLoop];

                if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified) && (field.Quantity > 0))
                {
                  #region Cek SP

                  spd1 = (from q in db.LG_SPD1s
                          join q1 in db.LG_ORD2s on new { q.c_spno, q.c_iteno } equals new { q1.c_spno, q1.c_iteno }
                          join q2 in db.LG_ORHs on q1.c_orno equals q2.c_orno
                          where q.c_spno == field.NomorSP && q.c_iteno == field.Item && q2.c_type != "02"
                          select q).Distinct().Take(1).SingleOrDefault();

                  if ((spd1 != null) && (spd1.n_sisa.HasValue) && (spd1.n_sisa.Value > 0))
                  {
                    if (field.Quantity > spd1.n_sisa.Value)
                    {
                      spQty = spd1.n_sisa.Value;
                    }
                    else
                    {
                      spQty = field.Quantity;
                    }

                    spQtyReloc = spQty.Value;

                    listResRND1 = (from q in db.LG_RND1s
                                   join q1 in db.LG_RNHs on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                   join q2 in db.LG_MsBatches on q.c_batch equals q2.c_batch
                                   join q3 in db.LG_RND2s on new { q.c_gdg, q.c_rnno } equals new { q3.c_gdg, q3.c_rnno }
                                   join q4 in db.LG_POD2s on new { q3.c_gdg, c_pono = q3.c_no } equals new { q4.c_gdg, q4.c_pono }
                                   join q5 in db.LG_ORHs on new { q4.c_gdg, q4.c_orno } equals new { q5.c_gdg, q5.c_orno }
                                   where q.c_gdg == gudang && q.c_batch == field.Batch && q.c_iteno == field.Item && q.n_gsisa > 0 && q5.c_type != "02"
                                   orderby q2.d_expired descending, q1.d_rndate
                                   select q).Distinct().ToList();

                    #region Cek RN

                    if (listResRND1.Count > 0)
                    {
                      for (nLoopC = 0; nLoopC < listResRND1.Count; nLoopC++)
                      {
                        rnd1 = listResRND1[nLoopC];

                        if ((rnd1.n_gsisa.HasValue) && (rnd1.n_gsisa.Value > 0))
                        {
                          //rnQty = rnd1.n_gsisa.Value;

                          if (rnd1.n_gsisa.Value >= spQtyReloc)
                          {
                            rnQty = spQtyReloc;

                            rnd1.n_gsisa -= spQtyReloc;
                            spQtyReloc = 0;
                          }
                          else
                          {
                            rnQty = rnd1.n_gsisa.Value;

                            spQtyReloc -= rnd1.n_gsisa.Value;
                            rnd1.n_gsisa = 0;
                          }

                          listPld2.Add(new LG_PLD2()
                          {
                            c_plno = plID,
                            c_batch = field.Batch,
                            c_iteno = field.Item,
                            c_rnno = rnd1.c_rnno,
                            c_spno = field.NomorSP,
                            c_type = "01",
                            n_qty = rnQty,
                            n_sisa = rnQty
                          });
                        }

                        if (spQtyReloc < 1)
                        {
                          break;
                        }
                      }

                      spAlloc = (spQty.Value - spQtyReloc);

                      listPld1.Add(new LG_PLD1()
                      {
                        c_plno = plID,
                        c_batch = field.Batch,
                        c_iteno = field.Item,
                        c_spno = field.NomorSP,
                        c_type = "01",
                        n_booked = spAlloc,
                        n_qty = spAlloc,
                        n_sisa = spAlloc
                      });
                    }

                    #endregion

                    if ((listPld1.Count > 0) && (listPld2.Count > 0))
                    {
                      if (spAlloc <= spd1.n_sisa.Value)
                      {
                        spd1.n_sisa -= spAlloc;
                      }
                      else
                      {
                        spd1.n_sisa -= 0;
                      }
                    }
                  }

                  if ((listPld1.Count > 0) && (listPld2.Count > 0))
                  {
                    #region Log PLD3

                    for (nLoopC = 0; nLoopC < listPld2.Count; nLoopC++)
                    {
                      pld2 = listPld2[nLoopC];

                      if (pld2 != null)
                      {
                        db.LG_PLD3s.InsertOnSubmit(new LG_PLD3()
                        {
                          c_batch = pld2.c_batch,
                          c_iteno = pld2.c_iteno,
                          c_plno = pld2.c_plno,
                          c_rnno = pld2.c_rnno,
                          c_spno = pld2.c_spno,
                          c_type = pld2.c_type,
                          n_qty = pld2.n_qty,
                          n_sisa = pld2.n_sisa,
                          c_entry = nipEntry,
                          d_entry = DateTime.Now,
                          v_ket_del = "Insert on Confirm",
                          v_type = "01"
                        });
                      }
                    }

                    #endregion

                    db.LG_PLD1s.InsertAllOnSubmit(listPld1.ToArray());
                    db.LG_PLD2s.InsertAllOnSubmit(listPld2.ToArray());

                    totalDetails++;
                  }

                  #endregion
                }
                else if ((field != null) && (!field.IsNew) && field.IsDelete && (!field.IsModified))
                {
                  #region Cek SP

                  pld1 = (from q in db.LG_PLD1s
                          where q.c_batch == field.Batch && q.c_iteno == field.Item &&
                          q.c_plno == plID && q.c_spno == field.NomorSP
                          select q).Distinct().SingleOrDefault();

                  if (pld1 != null)
                  {
                    spQtyReloc = (pld1.n_qty.HasValue ? pld1.n_qty.Value : 0);
                    spAlloc = (pld1.n_sisa.HasValue ? pld1.n_sisa.Value : 0);

                    if (spAlloc.Equals(spQtyReloc))
                    {
                      #region Reverse SP

                      spd1 = (from q in db.LG_SPD1s
                              where q.c_spno == pld1.c_spno && q.c_iteno == pld1.c_iteno
                              select q).Distinct().SingleOrDefault();

                      if (spd1 != null)
                      {
                        spd1.n_sisa += spAlloc;
                      }

                      #endregion

                      #region Reverse RN

                      listPld2 = (from q in db.LG_PLD2s
                                  where q.c_plno == plID && q.c_iteno == pld1.c_iteno &&
                                  q.c_batch == pld1.c_batch
                                  select q).Distinct().ToList();


                      if ((listPld2 != null) && (listPld2.Count > 0))
                      {
                        for (nLoopC = 0; nLoopC < listPld2.Count; nLoopC++)
                        {
                          pld2 = listPld2[nLoopC];

                          if (pld2 != null)
                          {
                            rnd1 = (from q in db.LG_RND1s
                                    where q.c_batch == pld2.c_batch && q.c_iteno == pld2.c_iteno &&
                                    q.c_rnno == pld2.c_rnno && q.c_gdg == plh.c_gdg
                                    select q).Distinct().SingleOrDefault();

                            rnd1.n_gsisa += (pld2.n_qty);

                            listResRND1.Add(rnd1);

                            #region Log PLD3

                            listPld3.Add(new LG_PLD3()
                            {
                              c_batch = pld2.c_batch,
                              c_iteno = pld2.c_iteno,
                              c_plno = pld2.c_plno,
                              c_rnno = pld2.c_rnno,
                              c_spno = pld2.c_spno,
                              c_type = pld2.c_type,
                              n_qty = pld2.n_qty,
                              n_sisa = pld2.n_sisa,
                              v_ket_del = field.Keterangan,
                              v_type = "03",
                              c_entry = nipEntry,
                              d_entry = DateTime.Now
                            });

                            #endregion
                          }
                        }

                        #region Delete PLD2

                        db.LG_PLD2s.DeleteAllOnSubmit(listPld2.ToArray());

                        #endregion
                      }

                      #endregion

                      #region Delete PLD1

                      db.LG_PLD1s.DeleteOnSubmit(pld1);

                      #endregion

                      #region Insert Log PLD3

                      db.LG_PLD3s.InsertAllOnSubmit(listPld3.ToArray());

                      #endregion
                    }
                  }

                  #endregion
                }
                else if ((field != null) && (!field.IsNew) && (!field.IsDelete) && field.IsModified)
                {
                  #region Cek SP

                  pld1 = (from q in db.LG_PLD1s
                          where q.c_batch == field.Batch && q.c_iteno == field.Item &&
                          q.c_plno == plID && q.c_spno == field.NomorSP
                          select q).Distinct().SingleOrDefault();

                  if (pld1 != null)
                  {
                    spQtyReloc = (pld1.n_qty.HasValue ? pld1.n_qty.Value : 0);
                    spAlloc = (pld1.n_sisa.HasValue ? pld1.n_sisa.Value : 0);

                    if (spAlloc.Equals(spQtyReloc))
                    {
                      spQty = (spAlloc - field.Quantity);

                      #region Reverse SP

                      spd1 = (from q in db.LG_SPD1s
                              where q.c_spno == pld1.c_spno && q.c_iteno == pld1.c_iteno
                              select q).Distinct().SingleOrDefault();

                      if (spd1 != null)
                      {
                        spd1.n_sisa += spQty;
                      }

                      #endregion

                      #region Reverse RN

                      listPld2 = (from q in db.LG_PLD2s
                                  where q.c_plno == plID && q.c_iteno == pld1.c_iteno &&
                                  q.c_batch == pld1.c_batch
                                  select q).Distinct().ToList();

                      if ((listPld2 != null) && (listPld2.Count > 0))
                      {
                        rnQty = spQty.Value;

                        for (nLoopC = (listPld2.Count - 1); nLoopC >= 0; nLoopC++)
                        {
                          pld2 = listPld2[nLoopC];

                          if (pld2 != null)
                          {
                            rnd1 = (from q in db.LG_RND1s
                                    where q.c_batch == pld2.c_batch && q.c_iteno == pld2.c_iteno &&
                                    q.c_rnno == pld2.c_rnno && q.c_gdg == plh.c_gdg
                                    select q).Distinct().SingleOrDefault();


                            #region Log PLD3

                            listPld3.Add(new LG_PLD3()
                            {
                              c_batch = pld2.c_batch,
                              c_iteno = pld2.c_iteno,
                              c_plno = pld2.c_plno,
                              c_rnno = pld2.c_rnno,
                              c_spno = pld2.c_spno,
                              c_type = pld2.c_type,
                              n_qty = pld2.n_qty,
                              n_sisa = pld2.n_sisa,
                              v_ket_del = field.Keterangan,
                              v_type = "02",
                              c_entry = nipEntry,
                              d_entry = DateTime.Now
                            });

                            #endregion

                            spAlloc = ((pld2.n_qty.HasValue ? pld2.n_qty.Value : 0) >= rnQty ?
                                          rnQty : (pld2.n_qty.HasValue ? pld2.n_qty.Value : 0));

                            rnd1.n_gsisa += spAlloc;

                            pld2.n_sisa = pld2.n_qty -= spAlloc;

                            rnQty -= spAlloc;

                            if ((pld2.n_qty.HasValue ? pld2.n_qty.Value : 0m) <= 0.00m)
                            {
                              #region Delete PLD2

                              db.LG_PLD2s.DeleteOnSubmit(pld2);

                              #endregion
                            }

                            listResRND1.Add(rnd1);

                            if (rnQty < 0.00m)
                            {
                              break;
                            }
                          }

                          nLoopC++;

                          if (nLoopC >= listPld2.Count)
                          {
                            break;
                          }
                        }
                      }

                      #endregion

                      #region Reverse PLD1

                      pld1.n_sisa = pld1.n_qty -= spQty;

                      #endregion

                      #region Insert Log PLD3

                      db.LG_PLD3s.InsertAllOnSubmit(listPld3.ToArray());

                      #endregion
                    }
                  }

                  #endregion
                }

                db.SubmitChanges();

                #region Clear

                if (listRN != null)
                {
                  listRN.Clear();
                }
                if (listPld1 != null)
                {
                  listPld1.Clear();
                }
                if (listPld2 != null)
                {
                  listPld2.Clear();
                }
                if (listPld3 != null)
                {
                  listPld3.Clear();
                }
                if (listResRND1 != null)
                {
                  listResRND1.Clear();
                }

                #endregion
              }
            }

            #endregion
          }
          else if (isConfirmed == (!structure.Fields.IsConfirm))
          {
            plh.l_confirm = structure.Fields.IsConfirm;

            if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
            {
              plh.v_ket = structure.Fields.Keterangan;
            }

            plh.c_update = nipEntry;
            plh.d_update = DateTime.Now;

            db.SubmitChanges();
          }

          #endregion
        }

        //db.Transaction.Commit();
        db.Transaction.Rollback();

        rpe = ResponseParser.ResponseParserEnum.IsSuccess;
      }
      catch (Exception ex)
      {
        if (db.Transaction != null)
        {
          db.Transaction.Rollback();
        }

        result = string.Format("ScmsSoaLibrary.Bussiness.Penjualan:PackingList - {0}", ex.Message);

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
