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
    class Memo
    {
        #region Internal Class

        internal class ItemBatch
        {
            public string Item { get; set; }
            public string Batch { get; set; }
        }

        internal class ItemKomposisi
        {
            public string Item { get; set; }
            public decimal Komposisi { get; set; }
        }

        internal class ItemQuantity
        {
            public string Item { get; set; }
            public string Batch { get; set; }
            public decimal Quantity { get; set; }
        }

        #endregion

        private enum EMVerify
        {
            EMV_Add,
            EMV_Modify,
            EMV_Delete
        }

        private bool VerifikasiAddCombo(EMVerify mode, char gdg, ORMDataContext db, ScmsSoaLibrary.Parser.Class.MemoComboStructure structure)
        {
            if (structure.Fields.Field == null)
            {
                return false;
            }

            bool bOk = false;
            List<string> listTmps = null;
            int nLoop = 0,
              nLen = 0,
              nCnt = 0;
            ScmsSoaLibrary.Parser.Class.MemoComboStructureField field = null;
            List<ItemKomposisi> lstKomp = null;
            List<ItemQuantity> lstQty = null;

            switch (mode)
            {
                case EMVerify.EMV_Add:
                    {
                        listTmps = new List<string>();

                        for (nLoop = 0, nLen = structure.Fields.Field.Length; nLoop < nLen; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if ((field != null) && field.IsNew)
                            {
                                #region Total Summary Benar

                                if (field.Quantity > (field.Komposisi * structure.Fields.Quantity))
                                {
                                    listTmps.Clear();

                                    goto breakOut;
                                }

                                #endregion

                                if (!listTmps.Contains(field.Item))
                                {
                                    listTmps.Add(field.Item);
                                }
                            }
                        }

                        if (listTmps.Count > 0)
                        {
                            #region RN Mencukupi

                            lstQty = (from q in db.LG_RNHs
                                      join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                      where (q.c_gdg == gdg)
                                          //&& ((q.c_type != "04") || ((q.c_type != "06") && (q.l_khusus == true)))
                                        && ((q.c_type != "06") && ((q.l_khusus.HasValue ? q.l_khusus.Value : false) == false)
                                        && listTmps.Contains(q1.c_iteno)
                                        && (q1.n_gsisa > 0)
                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false))
                                      //group q1 by new { q1.c_iteno, q1.c_batch } into g
                                      group q1 by new { q1.c_iteno } into g
                                      select new ItemQuantity()
                                      {
                                          //Batch = g.Key.c_batch.TrimEnd(),
                                          Item = g.Key.c_iteno,
                                          Quantity = (g.Sum(x => x.n_gsisa).HasValue ? g.Sum(x => x.n_gsisa).Value : 0)
                                      }).ToList();

                            if ((lstQty != null) && (lstQty.Count > 0))
                            {
                                for (nLoop = 0, nLen = structure.Fields.Field.Length; nLoop < nLen; nLoop++)
                                {
                                    field = structure.Fields.Field[nLoop];

                                    //nCnt = lstQty.Where(x => x.Item == field.Item && x.Quantity >= field.Quantity && x.Batch == field.Batch).Count();
                                    nCnt = lstQty.Where(x => x.Item == field.Item && x.Quantity >= field.Quantity).Count();

                                    if (nCnt < 1)
                                    {
                                        lstQty.Clear();
                                        listTmps.Clear();

                                        goto breakOut;
                                    }
                                }

                                lstQty.Clear();
                            }
                            else
                            {
                                goto breakOut;
                            }

                            #endregion

                            #region Komposisi Benar

                            lstKomp = (from q in db.FA_Combos
                                       where listTmps.Contains(q.c_iteno) && (q.n_qty > 0)
                                        && (q.c_combo == structure.Fields.ComboItem)
                                       select new ItemKomposisi()
                                       {
                                           Item = q.c_iteno,
                                           Komposisi = (q.n_qty.HasValue ? q.n_qty.Value : 0)
                                       }).ToList();

                            if ((lstKomp != null) && (lstKomp.Count > 0) && (lstKomp.Count == listTmps.Count))
                            {
                                for (nLoop = 0, nLen = lstKomp.Count; nLoop < nLen; nLoop++)
                                {
                                    field = structure.Fields.Field[nLoop];

                                    nCnt = lstKomp.Where(x => x.Item == field.Item && x.Komposisi != field.Komposisi).Count();

                                    if (nCnt > 0)
                                    {
                                        lstKomp.Clear();
                                        listTmps.Clear();

                                        goto breakOut;
                                    }
                                }

                                lstKomp.Clear();
                            }
                            else
                            {
                                goto breakOut;
                            }

                            #endregion

                            listTmps.Clear();

                            bOk = true;
                        }
                    }
                    break;
            }

        breakOut:
            return bOk;
        }

        public string MemoCombo(ScmsSoaLibrary.Parser.Class.MemoComboStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false;
            string result = null;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            LG_ComboH comboh = null;

            List<LG_ComboD1> listCombod1 = null;
            List<LG_ComboD2> listCombod2 = null;
            List<LG_ComboD3> listCombod3 = null;

            List<LG_ComboD2> listCombod2Copy = null;

            List<LG_RND1> listRND1 = null;

            List<ItemBatch> listRNNO = null;

            LG_ComboD1 combod1 = null;
            LG_ComboD2 combod2 = null;

            MK_MemoD memod = null;

            LG_RND1 rnd1 = null;

            ScmsSoaLibrary.Parser.Class.MemoComboStructureField field = null;
            string nipEntry = null;
            string comboID = null;
            string memoID = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now;

            decimal comboQty = 0, rnSisa = 0,
                rnAvaible = 0, mkQtyReloc = 0;

            int nLoop = 0,
              nLoopC;

            string[] rnType = { "01", "03", "05" };       //type RN

            bool isConf = false;

            IDictionary<string, string> dic = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }
            int totalDetails = 0;

            comboID = (structure.Fields.ComboID ?? string.Empty);

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

                    if (!VerifikasiAddCombo(EMVerify.EMV_Add, gdg, db, structure))
                    {
                        result = "Verifikasi combo gagal, cek komposisi dan ketersedian stok.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (!string.IsNullOrEmpty(comboID))
                    {
                        result = "Nomor Combo harus kosong.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (string.IsNullOrEmpty(structure.Fields.MemoID))
                    {
                        result = "Nomor Memo harus terisi.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (string.IsNullOrEmpty(structure.Fields.ComboItem))
                    {
                        result = "Nomor Item combo harus terisi.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (string.IsNullOrEmpty(structure.Fields.Batch))
                    {
                        result = "Nomor Batch harus terisi.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (structure.Fields.Quantity < 1)
                    {
                        result = "Quantity harus lebih besar dari 0.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, date))
                    {
                        result = "Combo tidak dapat disimpan, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "LGCOMBO");

                    comboID = Commons.GenerateNumbering<LG_ComboH>(db, "CB", '3', "22", date, "c_combono");

                    comboh = new LG_ComboH()
                    {
                        c_entry = nipEntry,
                        c_gdg = gdg,
                        c_batch = structure.Fields.Batch,
                        c_combono = comboID,
                        c_iteno = structure.Fields.ComboItem,
                        c_memono = structure.Fields.MemoID,
                        c_type = "01",
                        c_update = nipEntry,
                        d_combodate = date,
                        d_entry = date,
                        d_update = date,
                        l_confirm = false,
                        n_acc = structure.Fields.Quantity,
                        v_ket = structure.Fields.Keterangan,
                        n_bqty = 0,
                        n_bsisa = 0,
                        n_gqty = structure.Fields.Quantity,
                        n_gsisa = 0
                    };

                    db.LG_ComboHs.InsertOnSubmit(comboh);

                    #region Old Code

                    //db.SubmitChanges();

                    //comboh = (from q in db.LG_ComboHs
                    //         where (q.v_ket == tmpNumbering) && (q.c_gdg == gdg)
                    //         select q).Take(1).SingleOrDefault();

                    //if (comboh == null)
                    //{
                    //  result = "Nomor Combo tidak dapat di raih.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}
                    //else if (comboh.c_combono.Equals("XXXXXXXXXX"))
                    //{
                    //  result = "Trigger Combo tidak aktif.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}

                    //Commons.ModifyBatch(db, structure.Fields.ComboItem, structure.Fields.Batch, date, nipEntry);

                    //comboh.v_ket = structure.Fields.Keterangan;

                    //comboID = comboh.c_memono;

                    #endregion

                    #region Insert Detail

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                     {
                        Commons.CheckAndProcessBatch(db, structure.Fields.ComboItem, structure.Fields.Batch, date, nipEntry);

                        listCombod1 = new List<LG_ComboD1>();
                        listCombod2 = new List<LG_ComboD2>();

                        listRNNO = structure.Fields.Field.GroupBy(x => new { x.Batch, x.Item }).Select(y => new ItemBatch() { Batch = y.Key.Batch, Item = y.Key.Item }).ToList();

                        listRND1 = (from q in db.LG_RNHs
                                    join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                    where (q.c_gdg == gdg)
                                      && listRNNO.Contains(new ItemBatch()
                                      {
                                          Batch = q1.c_batch,
                                          Item = q1.c_iteno
                                      })
                                      && ((rnType.Contains(q.c_type)) && ((q.l_khusus.HasValue ? q.l_khusus.Value : false) == false))
                                      && (q1.n_gsisa > 0)
                                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                    select q1).ToList();

                        memod = (from q in db.MK_MemoHs
                                 join q1 in db.MK_MemoDs on new { q.c_gdg, q.c_memono } equals new { q1.c_gdg, q1.c_memono }
                                 where (q.c_gdg == gdg) && (q.c_memono == structure.Fields.MemoID)
                                  && (q1.c_iteno == structure.Fields.ComboItem)
                                  && (q1.n_sisa > 0)
                                  && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                 select q1).Take(1).SingleOrDefault();

                        if (memod == null)
                        {
                            result = "Memo dengan item tersebut tidak ditemukan.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }

                        comboQty = (memod.n_sisa.HasValue ? memod.n_sisa.Value : 0);

                        if (structure.Fields.Quantity > comboQty)
                        {
                            result = "Jumlah combo melebihi dari jumlah memo.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }

                        memod.n_sisa -= structure.Fields.Quantity;

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            mkQtyReloc = field.Quantity;
                            if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0))
                            {
                                #region Modify RN

                                for (nLoopC = 0; nLoopC < listRND1.Count; nLoopC++)
                                {
                                    rnd1 = listRND1[nLoopC];

                                    if (rnd1.c_iteno != field.Item)
                                    {
                                        continue;
                                    }

                                    rnSisa = (rnd1.n_gsisa.HasValue ? rnd1.n_gsisa.Value : 0);

                                    if (rnSisa > 0)
                                    {
                                        if (rnSisa >= mkQtyReloc)
                                        {
                                            rnAvaible = mkQtyReloc;

                                            rnd1.n_gsisa -= mkQtyReloc;

                                            mkQtyReloc = 0;
                                        }
                                        else
                                        {
                                            rnAvaible = rnSisa;

                                            mkQtyReloc -= rnd1.n_gsisa.Value;

                                            rnd1.n_gsisa = 0;
                                        }

                                        combod2 = listCombod2.Find(delegate(LG_ComboD2 cb)
                                        {
                                            return field.Item.Equals((string.IsNullOrEmpty(cb.c_iteno) ? string.Empty : cb.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              field.Batch.Equals((string.IsNullOrEmpty(cb.c_batch) ? string.Empty : cb.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              (string.IsNullOrEmpty(rnd1.c_rnno) ? string.Empty : rnd1.c_rnno.Trim()).Equals((string.IsNullOrEmpty(cb.c_rnno) ? string.Empty : cb.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
                                        });

                                        if (combod2 == null)
                                        {
                                            listCombod2.Add(new LG_ComboD2()
                                            {
                                                c_batch = field.Batch,
                                                c_combono = comboID,
                                                c_gdg = gdg,
                                                c_iteno = field.Item,
                                                c_rnno = rnd1.c_rnno,
                                                n_qty = rnAvaible
                                            });
                                        }
                                        else
                                        {
                                            combod2.n_qty += rnAvaible;
                                        }
                                    }

                                    if (mkQtyReloc < 1)
                                    {
                                        break;
                                    }
                                }

                                combod1 = listCombod1.Find(delegate(LG_ComboD1 cb)
                                {
                                    return field.Item.Equals((string.IsNullOrEmpty(cb.c_iteno) ? string.Empty : cb.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                      field.Batch.Equals((string.IsNullOrEmpty(cb.c_batch) ? string.Empty : cb.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                });

                                if (combod1 == null)
                                {
                                    listCombod1.Add(new LG_ComboD1()
                                    {
                                        c_batch = field.Batch,
                                        c_combono = comboID,
                                        c_gdg = gdg,
                                        c_iteno = field.Item,
                                        n_qty = field.Quantity
                                    });
                                }
                                else
                                {
                                    combod1.n_qty += field.Quantity;
                                }

                                totalDetails++;
                                
                                #endregion

                                #region old
                                //rnd1 = listRND1.Find(delegate(LG_RND1 rnd)
                                //{
                                //    return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                //      field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                //});

                                //if ((rnd1 != null) && ((rnd1.n_gsisa.HasValue ? rnd1.n_gsisa.Value : 0) >= field.Quantity))
                                //{
                                //    rnd1.n_gsisa -= field.Quantity;

                                //    #region Populate Combo

                                //    combod2 = listCombod2.Find(delegate(LG_ComboD2 cb)
                                //    {
                                //        return field.Item.Equals((string.IsNullOrEmpty(cb.c_iteno) ? string.Empty : cb.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                //          field.Batch.Equals((string.IsNullOrEmpty(cb.c_batch) ? string.Empty : cb.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                //          (string.IsNullOrEmpty(rnd1.c_rnno) ? string.Empty : rnd1.c_rnno.Trim()).Equals((string.IsNullOrEmpty(cb.c_rnno) ? string.Empty : cb.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
                                //    });

                                //    if (combod2 == null)
                                //    {
                                //        listCombod2.Add(new LG_ComboD2()
                                //        {
                                //            c_batch = field.Batch,
                                //            c_combono = comboID,
                                //            c_gdg = gdg,
                                //            c_iteno = field.Item,
                                //            c_rnno = rnd1.c_rnno,
                                //            n_qty = field.Quantity
                                //        });
                                //    }
                                //    else
                                //    {
                                //        combod2.n_qty += field.Quantity;
                                //    }

                                //    //combod1 = listCombod1.Where(x => x.c_iteno == field.Item).Take(1).SingleOrDefault();
                                //    combod1 = listCombod1.Find(delegate(LG_ComboD1 cb)
                                //    {
                                //        return field.Item.Equals((string.IsNullOrEmpty(cb.c_iteno) ? string.Empty : cb.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                //          field.Batch.Equals((string.IsNullOrEmpty(cb.c_batch) ? string.Empty : cb.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                //    });

                                //    if (combod1 == null)
                                //    {
                                //        listCombod1.Add(new LG_ComboD1()
                                //        {
                                //            c_batch = field.Batch,
                                //            c_combono = comboID,
                                //            c_gdg = gdg,
                                //            c_iteno = field.Item,
                                //            n_qty = field.Quantity
                                //        });
                                //    }
                                //    else
                                //    {
                                //        combod1.n_qty += field.Quantity;
                                //    }

                                //    totalDetails++;

                                //    #endregion
                                //}
                                #endregion
                            }
                        }

                        if ((listCombod1 != null) && (listCombod1.Count > 0))
                        {
                            db.LG_ComboD1s.InsertAllOnSubmit(listCombod1.ToArray());
                            listCombod1.Clear();
                        }

                        if ((listCombod2 != null) && (listCombod2.Count > 0))
                        {
                            db.LG_ComboD2s.InsertAllOnSubmit(listCombod2.ToArray());
                            listCombod2.Clear();
                        }
                    }

                    #endregion

                    dic = new Dictionary<string, string>();

                    if (totalDetails > 0)
                    {
                        dic.Add("COMBO", comboID);
                        dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                        result = string.Format("Total {0} detail(s)", totalDetails);

                        hasAnyChanges = true;
                    }

                    #endregion
                }
                else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    if (string.IsNullOrEmpty(comboID))
                    {
                        result = "Nomor Combo dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    comboh = (from q in db.LG_ComboHs
                              where (q.c_combono == comboID) && (q.c_gdg == gdg)
                              select q).Take(1).SingleOrDefault();

                    if (comboh == null)
                    {
                        result = "Nomor Combo tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (comboh.l_delete.HasValue && comboh.l_delete.Value)
                    {
                        result = "Tidak dapat merubah Combo yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (comboh.l_confirm.HasValue && comboh.l_confirm.Value)
                    {
                        result = "Tidak dapat merubah Combo yang sudah diconfirm.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, comboh.d_combodate))
                    {
                        result = "Combo tidak dapat diubah, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
                    {
                        comboh.v_ket = structure.Fields.Keterangan;
                    }

                    comboh.c_update = nipEntry;
                    comboh.d_update = DateTime.Now;

                    #region Populate Detail

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        listCombod3 = new List<LG_ComboD3>();
                        listCombod2Copy = new List<LG_ComboD2>();

                        listCombod1 = (from q in db.LG_ComboD1s
                                       where q.c_gdg == gdg && q.c_combono == comboID
                                       select q).ToList();

                        listCombod2 = (from q in db.LG_ComboD2s
                                       where q.c_gdg == gdg && q.c_combono == comboID
                                       select q).ToList();


                        listRNNO = listCombod2.GroupBy(x => new { x.c_batch, x.c_iteno }).Select(y => new ItemBatch() { Batch = y.Key.c_batch.TrimEnd(), Item = y.Key.c_iteno }).ToList();

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];
                            if (!listRNNO.Exists(delegate(ItemBatch ib)
                            {
                                if ((ib.Batch == field.Batch) && (ib.Item == field.Item))
                                {
                                    return true;
                                }

                                return false;
                            }))
                            {
                                listRNNO.Add(new ItemBatch()
                                {
                                    Batch = field.Batch,
                                    Item = field.Item
                                });
                            }
                        }

                        listRND1 = (from q in db.LG_RNHs
                                    join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                    where (q.c_gdg == gdg) && listRNNO.Contains(new ItemBatch()
                                    {
                                        Batch = q1.c_batch,
                                        Item = q1.c_iteno
                                    }) && ((rnType.Contains(q.c_type)) && ((q.l_khusus.HasValue ? q.l_khusus.Value : false) == false))
                                    select q1).ToList();

                        #region Logic Detail

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if ((field != null) && (!field.IsNew) && field.IsDelete && (!field.IsModified))
                            {
                                #region Delete

                                combod1 = listCombod1.Find(delegate(LG_ComboD1 cbd1)
                                {
                                    if ((cbd1.c_gdg == gdg) && (cbd1.c_combono == comboID) && (cbd1.c_iteno == field.Item))
                                    {
                                        return true;
                                    }

                                    return false;
                                });

                                if (combod1 != null)
                                {
                                    comboQty = (combod1.n_qty.HasValue ? combod1.n_qty.Value : 0);

                                    //listCombod2Copy = listCombod2.Where(x => x.c_iteno == field.Item && x.c_batch == field.Batch).ToList();
                                    listCombod2Copy = listCombod2.FindAll(delegate(LG_ComboD2 cbd2)
                                    {
                                        if ((cbd2.c_iteno == field.Item) && ((string.IsNullOrEmpty(cbd2.c_batch) ? string.Empty : cbd2.c_batch.TrimEnd()) == field.Batch))
                                        {
                                            return true;
                                        }

                                        return false;
                                    });

                                    for (nLoopC = 0; nLoopC < listCombod2Copy.Count; nLoopC++)
                                    {
                                        combod2 = listCombod2Copy[nLoopC];

                                        if (combod2 != null)
                                        {
                                            //rnd1 = listRND1.Where(x => x.c_gdg == combod2.c_gdg && x.c_rnno == combod2.c_rnno && x.c_iteno == combod2.c_iteno && (string.IsNullOrEmpty(x.c_batch) ? string.Empty : x.c_batch.TrimEnd()) == (string.IsNullOrEmpty(combod2.c_batch) ? string.Empty : combod2.c_batch.TrimEnd())).Take(1).SingleOrDefault();
                                            rnd1 = listRND1.Find(delegate(LG_RND1 rnd)
                                            {
                                                if ((rnd.c_gdg == combod2.c_gdg) && (rnd.c_rnno == combod2.c_rnno) && (rnd.c_iteno == combod2.c_iteno) && (rnd.c_batch == combod2.c_batch))
                                                {
                                                    return true;
                                                }
                                                return false;
                                            });

                                            if (rnd1 != null)
                                            {
                                                rnd1.n_gsisa += combod2.n_qty;

                                                comboQty -= (combod2.n_qty.HasValue ? combod2.n_qty.Value : 0);

                                                db.LG_ComboD2s.DeleteOnSubmit(combod2);

                                                listCombod3.Add(new LG_ComboD3()
                                                {
                                                    c_batch = combod2.c_batch,
                                                    c_combono = combod2.c_combono,
                                                    c_entry = nipEntry,
                                                    c_gdg = gdg,
                                                    c_iteno = combod2.c_iteno,
                                                    c_rnno = combod2.c_rnno,
                                                    d_entry = date,
                                                    n_qty = combod2.n_qty,
                                                    v_ket_del = field.Keterangan,
                                                    v_type = "03"
                                                });
                                            }
                                        }
                                    }

                                    if (comboQty <= 0)
                                    {
                                        db.LG_ComboD1s.DeleteOnSubmit(combod1);
                                    }

                                    listCombod2Copy.Clear();

                                    listCombod1.Remove(combod1);

                                    totalDetails++;
                                }

                                #endregion
                            }
                        }

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            mkQtyReloc = field.Quantity;
                            if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified) && (field.Quantity > 0))
                            {
                                #region New

                                for (nLoopC = 0; nLoopC < listRND1.Count; nLoopC++)
                                {
                                    rnd1 = listRND1[nLoopC];

                                    if (rnd1.c_iteno != field.Item)
                                    {
                                        continue;
                                    }

                                    rnSisa = (rnd1.n_gsisa.HasValue ? rnd1.n_gsisa.Value : 0);

                                    if (rnSisa > 0)
                                    {
                                        if (rnSisa >= mkQtyReloc)
                                        {
                                            rnAvaible = mkQtyReloc;

                                            rnd1.n_gsisa -= mkQtyReloc;

                                            mkQtyReloc = 0;
                                        }
                                        else
                                        {
                                            rnAvaible = rnSisa;

                                            mkQtyReloc -= rnd1.n_gsisa.Value;

                                            rnd1.n_gsisa = 0;
                                        }

                                        combod2 = listCombod2.Find(delegate(LG_ComboD2 cb)
                                        {
                                            return field.Item.Equals((string.IsNullOrEmpty(cb.c_iteno) ? string.Empty : cb.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              field.Batch.Equals((string.IsNullOrEmpty(cb.c_batch) ? string.Empty : cb.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              (string.IsNullOrEmpty(rnd1.c_rnno) ? string.Empty : rnd1.c_rnno.Trim()).Equals((string.IsNullOrEmpty(cb.c_rnno) ? string.Empty : cb.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
                                        });

                                        if (combod2 == null)
                                        {
                                            listCombod2Copy.Add(new LG_ComboD2()
                                            {
                                                c_batch = field.Batch,
                                                c_combono = comboID,
                                                c_gdg = gdg,
                                                c_iteno = field.Item,
                                                c_rnno = rnd1.c_rnno,
                                                n_qty = rnAvaible
                                            });
                                        }
                                        else
                                        {
                                            combod2.n_qty += rnAvaible;
                                        }
                                    }

                                    if (mkQtyReloc < 1)
                                    {
                                        break;
                                    }
                                }

                                combod1 = listCombod1.Find(delegate(LG_ComboD1 cb)
                                {
                                    return field.Item.Equals((string.IsNullOrEmpty(cb.c_iteno) ? string.Empty : cb.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                      field.Batch.Equals((string.IsNullOrEmpty(cb.c_batch) ? string.Empty : cb.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                });

                                if (combod1 == null)
                                {
                                    listCombod1.Add(new LG_ComboD1()
                                    {
                                        c_batch = field.Batch,
                                        c_combono = comboID,
                                        c_gdg = gdg,
                                        c_iteno = field.Item,
                                        n_qty = field.Quantity
                                    });
                                }
                                else
                                {
                                    combod1.n_qty += field.Quantity;
                                }

                                listCombod3.Add(new LG_ComboD3()
                                {
                                    c_batch = field.Batch,
                                    c_combono = comboID,
                                    c_entry = nipEntry,
                                    c_gdg = gdg,
                                    c_iteno = field.Item,
                                    c_rnno = rnd1.c_rnno,
                                    d_entry = date,
                                    n_qty = field.Quantity,
                                    v_ket_del = field.Keterangan,
                                    v_type = "01"
                                });

                                totalDetails++;

                                #region old
                                //rnd1 = listRND1.Find(delegate(LG_RND1 rnd)
                                //{
                                //    if ((rnd.c_gdg == gdg) && (rnd.c_iteno == field.Item) && ((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()) == field.Batch))
                                //    {
                                //        return true;
                                //    }
                                //    return false;
                                //});

                                //if ((rnd1 != null) && ((rnd1.n_gsisa.HasValue ? rnd1.n_gsisa.Value : 0) >= field.Quantity))
                                //{
                                //    rnd1.n_gsisa -= field.Quantity;

                                //    combod1 = listCombod1.Find(delegate(LG_ComboD1 cbd1)
                                //    {
                                //        if ((cbd1.c_gdg == gdg) && (cbd1.c_combono == comboID) && (cbd1.c_iteno == field.Item) && ((string.IsNullOrEmpty(cbd1.c_batch) ? string.Empty : cbd1.c_batch.Trim()) == field.Batch))
                                //        {
                                //            return true;
                                //        }

                                //        return false;
                                //    });

                                //    if (combod1 == null)
                                //    {
                                //        combod1 = new LG_ComboD1()
                                //        {
                                //            c_batch = field.Batch,
                                //            c_combono = comboID,
                                //            c_gdg = gdg,
                                //            c_iteno = field.Item,
                                //            n_qty = 0
                                //        };

                                //        listCombod1.Add(combod1);

                                //        db.LG_ComboD1s.InsertOnSubmit(combod1);
                                //    }

                                //    combod2 = new LG_ComboD2()
                                //    {
                                //        c_batch = field.Batch,
                                //        c_combono = comboID,
                                //        c_gdg = gdg,
                                //        c_iteno = field.Item,
                                //        c_rnno = rnd1.c_rnno,
                                //        n_qty = field.Quantity
                                //    };

                                //    listCombod2.Add(combod2);

                                //    db.LG_ComboD2s.InsertOnSubmit(combod2);

                                //    combod1.n_qty += field.Quantity;

                                //    listCombod3.Add(new LG_ComboD3()
                                //    {
                                //        c_batch = field.Batch,
                                //        c_combono = comboID,
                                //        c_entry = nipEntry,
                                //        c_gdg = gdg,
                                //        c_iteno = field.Item,
                                //        c_rnno = rnd1.c_rnno,
                                //        d_entry = date,
                                //        n_qty = field.Quantity,
                                //        v_ket_del = field.Keterangan,
                                //        v_type = "01"
                                //    });

                                //    totalDetails++;
                                //}
                                #endregion

                                #endregion
                            }
                        }

                        #endregion

                        listCombod1.Clear();

                        listCombod2.Clear();

                        listRNNO.Clear();

                        listRND1.Clear();

                        if (listCombod2Copy.Count > 0)
                        {
                            db.LG_ComboD2s.InsertAllOnSubmit(listCombod2Copy.ToArray());

                            listCombod2Copy.Clear();
                        }

                        if (listCombod3.Count > 0)
                        {
                            db.LG_ComboD3s.InsertAllOnSubmit(listCombod3.ToArray());

                            listCombod3.Clear();
                        }
                    }

                    #endregion

                    hasAnyChanges = true;

                    #endregion
                }
                else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    if (string.IsNullOrEmpty(comboID))
                    {
                        result = "Nomor Combo dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    comboh = (from q in db.LG_ComboHs
                              where (q.c_combono == comboID) && (q.c_gdg == gdg)
                              select q).Take(1).SingleOrDefault();

                    if (comboh == null)
                    {
                        result = "Nomor Combo tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (comboh.l_delete.HasValue && comboh.l_delete.Value)
                    {
                        result = "Tidak dapat menghapus Combo yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, comboh.d_combodate))
                    {
                        result = "Combo tidak dapat dihapus, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    //comboh.c_update = nipEntry;
                    //comboh.d_update = DateTime.Now;

                    //comboh.l_delete = true;
                    //comboh.v_ket_mark = structure.Fields.Keterangan;

                    //listMemoD1 = new List<MK_MemoD1>();
                    listCombod3 = new List<LG_ComboD3>();

                    listCombod1 = (from q in db.LG_ComboD1s
                                   where q.c_gdg == gdg && q.c_combono == comboID
                                   select q).ToList();

                    listCombod2 = (from q in db.LG_ComboD2s
                                   where q.c_gdg == gdg && q.c_combono == comboID
                                   select q).ToList();

                    listRNNO = listCombod2.GroupBy(x => new { x.c_batch, x.c_iteno }).Select(y => new ItemBatch() { Batch = y.Key.c_batch, Item = y.Key.c_iteno }).ToList();

                    listRND1 = (from q in db.LG_RNHs
                                join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                where (q.c_gdg == gdg) && listRNNO.Contains(new ItemBatch()
                                {
                                    Batch = q1.c_batch,
                                    Item = q1.c_iteno
                                }) && ((rnType.Contains(q.c_type)) && ((q.l_khusus.HasValue ? q.l_khusus.Value : false) == false))
                                select q1).ToList();

                    memoID = (string.IsNullOrEmpty(comboh.c_memono) ? string.Empty : comboh.c_memono.Trim());
                    comboQty = (comboh.n_gqty.HasValue ? comboh.n_gqty.Value : 0);

                    //memod = (from q in db.MK_MemoHs
                    //         join q1 in db.MK_MemoDs on new { q.c_gdg, q.c_memono } equals new {q1.c_gdg, q1.c_memono}
                    //         select q1.c_memono)

                    memod = (from q in db.MK_MemoHs
                             join q1 in db.MK_MemoDs on new { q.c_gdg, q.c_memono } equals new { q1.c_gdg, q1.c_memono }
                             where (q.c_gdg == gdg) && (q.c_memono == memoID)
                              && (q1.c_iteno == (string.IsNullOrEmpty(comboh.c_iteno) ? string.Empty : comboh.c_iteno))
                              && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                             select q1).Take(1).SingleOrDefault();

                    if (memod == null)
                    {
                        result = "Tidak dapat menghapus Combo, karena item memo tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    memod.n_sisa += comboQty;

                    //listRNNO = listCombod2.GroupBy(x => x.c_rnno).Select(y => y.Key).ToList();

                    ////listRND1 = db.LG_RND1s.Where(x => x.c_gdg == gdg && listRNNO.Contains(x.c_iteno)).ToList();

                    //listRND1 = (from q in db.LG_RNHs
                    //            join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                    //            where (q.c_gdg == gdg) && listRNNO.Contains(q1.c_rnno) && q.c_type != "02"
                    //            select q1).ToList();

                    for (nLoop = 0; nLoop < listCombod2.Count; nLoop++)
                    {
                        combod2 = listCombod2[nLoop];

                        if (combod2 != null)
                        {
                            #region Revert RN

                            rnd1 = listRND1.Where(x => x.c_rnno == combod2.c_rnno && x.c_iteno == combod2.c_iteno && x.c_batch == combod2.c_batch).Take(1).SingleOrDefault();

                            if (rnd1 != null)
                            {
                                listCombod3.Add(new LG_ComboD3()
                                {
                                    c_batch = combod2.c_batch,
                                    c_combono = comboID,
                                    c_entry = nipEntry,
                                    c_gdg = gdg,
                                    c_iteno = combod2.c_iteno,
                                    c_rnno = combod2.c_rnno,
                                    d_entry = date,
                                    n_qty = combod2.n_qty,
                                    v_type = "03",
                                    v_ket_del = structure.Fields.Keterangan
                                });

                                rnd1.n_gsisa += (combod2.n_qty.HasValue ? combod2.n_qty.Value : 0);

                                db.LG_ComboD2s.DeleteOnSubmit(combod2);
                            }

                            #endregion
                        }
                    }

                    if (comboh != null)
                    {
                        db.LG_ComboHs.DeleteOnSubmit(comboh);
                    }

                    if (listCombod1.Count > 0)
                    {
                        db.LG_ComboD1s.DeleteAllOnSubmit(listCombod1.ToArray());

                        listCombod1.Clear();
                    }

                    if (listCombod3.Count > 0)
                    {
                        db.LG_ComboD3s.InsertAllOnSubmit(listCombod3.ToArray());

                        listCombod3.Clear();
                    }

                    hasAnyChanges = true;

                    #endregion
                }
                else if (structure.Method.Equals("Confirm", StringComparison.OrdinalIgnoreCase))
                {
                    #region Confirm

                    if (string.IsNullOrEmpty(comboID))
                    {
                        result = "Nomor Combo dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    //cek komposisi combo
                    var chkCombo = (from q in db.FA_Combos
                                    join q1 in db.LG_ComboHs on q.c_combo equals q1.c_iteno
                                    join q2 in (from qq in db.LG_ComboD1s
                                                    group qq by new {qq.c_gdg, qq.c_combono, qq.c_iteno} into g
                                                    select new
                                                    {
                                                        c_gdg = g.Key.c_gdg,
                                                        c_combono = g.Key.c_combono,
                                                        c_iteno = g.Key.c_iteno,
                                                        n_qty = g.Sum(x => x.n_qty)
                                                    }) on new { q1.c_combono, q.c_iteno } equals new { q2.c_combono, q2.c_iteno } into q_2
                                    from qCB in q_2.DefaultIfEmpty()
                                    where ((q1.c_combono == structure.Fields.ComboID
                                    && qCB.c_iteno == null)
                                    ||
                                    ((q1.n_gqty * q.n_qty) != qCB.n_qty)
                                    && q1.c_combono == structure.Fields.ComboID)
                                    select new
                                    {
                                        q1.c_combono,
                                        qCB.c_iteno
                                    }).Distinct().ToList();

                    if (chkCombo.Count > 0)
                    {
                        result = "Komposisi item tidak sesuai dengan master combo.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    comboh = (from q in db.LG_ComboHs
                              where (q.c_gdg == gdg) && (q.c_combono == comboID)
                                && (q.c_type == "01")
                              select q).Take(1).SingleOrDefault();

                    if (comboh == null)
                    {
                        result = "Nomor Combo tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (comboh.l_delete.HasValue && comboh.l_delete.Value)
                    {
                        result = "Tidak dapat mengkonfirmasi Combo yang terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    isConf = (comboh.l_confirm.HasValue ? comboh.l_confirm.Value : false);

                    if (structure.Fields.IsConfirm && (!isConf))
                    {
                        comboh.l_confirm = structure.Fields.IsConfirm;

                        comboh.n_gsisa = comboh.n_gqty;
                        //comboh.n_bsisa = comboh.n_bqty;

                        comboh.c_update = nipEntry;
                        comboh.d_update = date;

                        hasAnyChanges = true;
                    }
                    else
                    {
                        result = "Status combo terkonfirmasi.";
                    }

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
                }
            }
            catch (Exception ex)
            {
                if (db.Transaction != null)
                {
                    db.Transaction.Rollback();
                }

                result = string.Format("ScmsSoaLibrary.Bussiness.Memo:MemoCombo - {0}", ex.Message);

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
