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
using ScmsSoaLibrary.Modules;

namespace ScmsSoaLibrary.Bussiness
{
  class Transfer
  {
    #region Internal Class

    internal class ItemBatch
    {
      public string Item { get; set; }
      public string Batch { get; set; }
    }

    internal class SJ_RN_FRMT_DATA
    {
      public char c_gdg { get; set; }
      public string c_iteno { get; set; }
      public string v_item_desc { get; set; }
      public string c_batch { get; set; }
      public decimal n_gqty { get; set; }
      public decimal n_bqty { get; set; }
      public string c_refno { get; set; }
      public string c_addtno { get; set; }
    }

    internal class ItemInfo
    {
      public string Item { get; set; }
      public string Suplier { get; set; }
      public bool IsCombo { get; set; }
    }

    internal class SJClassComponent
    {
      public string RefID { get; set; }
      public string SignID { get; set; }
      public string Item { get; set; }
      public string BatchID { get; set; }
      public decimal Qty { get; set; }
      public string Supplier { get; set; }
      public decimal Box { get; set; }
      public decimal QtyBad { get; set; }
    }

    internal class LG_SJD1_SUM_BYBATCH
    {
        public string c_sjno { get; set; }
        public string c_iteno { get; set; }
        public string c_batch { get; set; }
        public decimal n_qty { get; set; }
    }

    #endregion

    private LG_SPGH SuratJalanSPGAdj(ORMDataContext db, ScmsSoaLibrary.Parser.Class.TranStructureField field, ScmsSoaLibrary.Parser.Class.TranStructure structure, DateTime date, string sjID)
    {
      LG_SPGH sph = null;

      #region SPH

      //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "SP");

      string spID = Commons.GenerateNumbering<LG_SPGH>(db, "SG", '3', "20", date, "c_spgno");

      char gdgAsal = (string.IsNullOrEmpty(structure.Fields.From) || (structure.Fields.From.Length < 1) ? '0' : structure.Fields.From[0]);
      char gdgTujuan = (string.IsNullOrEmpty(structure.Fields.To) || (structure.Fields.To.Length < 1) ? '0' : structure.Fields.To[0]);

      sph = new LG_SPGH()
      {
        c_gdg1 = gdgTujuan,
        c_gdg2 = gdgAsal,
        c_entry = structure.Fields.Entry,
        c_nosup = structure.Fields.Supplier,
        c_spgno = spID,
        c_type = "04",
        c_update = structure.Fields.Entry,
        d_entry = DateTime.Now,
        d_spgdate = DateTime.Now,
        d_update = DateTime.Now,
        l_status = true,
        l_print = false,
        v_ket = "Adjusment Auto " + sjID,
        d_spsubmit = DateTime.Now,
        l_delete = false,
      };

      //db.LG_SPHs.InsertOnSubmit(sph);

      //db.SubmitChanges();

      //spID = (from q in db.LG_SPHs
      //        where q.v_ket == tmpNumbering
      //        select q.c_spno).Take(1).SingleOrDefault();

      #endregion

      return sph;
    }

    public string TransferGudang(ScmsSoaLibrary.Parser.Class.TranStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false;
      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      LG_SJH sjh = null;

      LG_SPGH spgh = null;
      LG_SPGH spghAdj = null;
      LG_SPGD1 spgd1 = null;

      LG_RND1 rnd1 = null;

      LG_ComboH comboh = null;

      List<SJClassComponent> listSPAC = null,
        istSPAC = null;
      //SJClassComponent spac = null;

      ScmsSoaLibrary.Parser.Class.TranStructureField field = null;
      string nipEntry = null;
      string sjID = null,
        rnID = null,
        itemID = null,
        batchCode = null,
        sgID = null, spgId = null,
        spgIdAdjust = null;
      string pinNumber = null;
      int totalDetails = 0;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      //decimal? spQty = 0;
      decimal spQtyReloc = 0,
        spAlloc = 0;

      List<LG_SJD1> listSjd1 = null;
      List<LG_SJD2> listSjd2 = null;
      List<LG_SJD2> listSjd2Copy = null;
      List<LG_SJD3> listSjd3 = null;

      List<LG_RND1> listRND1 = null;
      List<LG_SPGD1> listSPGD1 = null;
      List<LG_SPGD1> listSPGD1Auto = null;
      List<string> listRN = null;
      Dictionary<string, List<SJClassComponent>> dicItemStock = null;


      List<LG_SJD1_SUM_BYBATCH> listSJSum = null;

      LG_SJD1_SUM_BYBATCH sjd1Sum = null;

      List<LG_ComboH> listComboh = null;

      List<string> lstItemCombo = null;

      char gdgAsal = (string.IsNullOrEmpty(structure.Fields.From) || (structure.Fields.From.Length < 1) ? '0' : structure.Fields.From[0]);
      char gdgTujuan = (string.IsNullOrEmpty(structure.Fields.To) || (structure.Fields.To.Length < 1) ? '0' : structure.Fields.To[0]);

      LG_SJD1 sjd1 = null;
      LG_SJD2 sjd2 = null;

      FA_MasItm masitm = null;

            //Indra 20180920FM
            //SerahTerimaTransportasi
            LG_RNH RNH = null;
      int nLoop = 0,
        nLoopC = 0;

      decimal nQtyAllocGood = 0,
        totalCurrentStockGood = 0,
        trapsjd2 = 0,
        nBox = 0,
        koliKarton = 0,
        receh = 0;

      IDictionary<string, string> dic = null;

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }

      if (gdgAsal.Equals(char.MinValue))
      {
        result = "Gudang asal tidak dapat dibaca.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }

      bool isConfirmed = false;

      sjID = (structure.Fields.sjID ?? string.Empty);

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

                if (!string.IsNullOrEmpty(sjID))
                {
                    result = "Nomor Surat Jalan harus kosong.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                else if (string.IsNullOrEmpty(structure.Fields.Supplier))
                {
                    result = "Nama pemasok dibutuhkan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                else if (string.IsNullOrEmpty(structure.Fields.From))
                {
                    result = "Gudang dibutuhkan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                else if (Commons.IsClosingLogistik(db, date))
                {
                    result = "Surat Jalan tidak dapat disimpan, karena sudah closing.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

          sjID = Commons.GenerateNumbering<LG_SJH>(db, "SJ", '3', "06", date, "c_sjno");

                pinNumber = Functionals.GeneratedRandomPinId(8, string.Empty);

          sjh = new LG_SJH()
          {
            c_sjno = sjID,
            c_entry = nipEntry,
            c_gdg = gdgAsal,
            c_nosup = structure.Fields.Supplier,
            c_type = "01",
            c_update = nipEntry,
            d_entry = date,
            d_update = date,
            l_confirm = false,
            l_print = false,
            v_ket = structure.Fields.Keterangan,
            c_gdg2 = gdgTujuan,
            c_pin = pinNumber,
            d_sjdate = date.Date,
            l_exp = false,
            l_status = false,
            c_type_cat = structure.Fields.TypeKategori,
            c_type_lat = structure.Fields.TypeLantai,
            c_type_sj = structure.Fields.TypeSJ,
            l_auto = false
          };

                db.LG_SJHs.InsertOnSubmit(sjh);

          #region Insert Detail

                if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                {
                    listSjd1 = new List<LG_SJD1>();
                    listSjd2 = new List<LG_SJD2>();
                    //listComboh = new List<LG_ComboH>();
                    listRN = new List<string>();
                    comboh = new LG_ComboH();
                    rnd1 = new LG_RND1();
                    dicItemStock = new Dictionary<string, List<SJClassComponent>>(StringComparer.OrdinalIgnoreCase);
                    spgh = new LG_SPGH();
                    listSPGD1Auto = new List<LG_SPGD1>();

                    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    {
                        field = structure.Fields.Field[nLoop];

                        if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0))
                        {

                            spgd1 = (from q in db.LG_SPGHs
                                     join q1 in db.LG_SPGD1s on new { c_gdg = q.c_gdg1, q.c_spgno } equals new { q1.c_gdg, q1.c_spgno }
                                     where (q.c_spgno == field.NomorSP)
                                      && (q.c_gdg1 == gdgTujuan) && (q1.c_iteno == field.Item)
                                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                     select q1).Take(1).SingleOrDefault();

                            if (spgd1 != null)
                            {
                                nQtyAllocGood = 0;
                                spAlloc = (spgd1.n_sisa.HasValue ? spgd1.n_sisa.Value : 0);
                                spQtyReloc = (spAlloc > field.Quantity ? field.Quantity : spAlloc);
                                trapsjd2 = 0;

                                if (spAlloc <= 0)
                                {
                                    continue;
                                }

                  if (dicItemStock.ContainsKey(field.Item))
                  {
                    listSPAC = dicItemStock[field.Item];
                  }
                  else
                  {
                    listSPAC = (from q in ScmsSoaLibrary.Modules.GlobalQuery.ViewStockLite(db, gdgAsal, field.Item)
                                join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                from qBat in q_2.DefaultIfEmpty()
                                where (q.n_gsisa != 0)
                                orderby qBat.d_expired ascending
                                select new SJClassComponent()
                                {
                                  RefID = q.c_no,
                                  SignID = (q.c_table == null ? string.Empty : q.c_table.Trim()),
                                  Item = q.c_iteno,
                                  Qty = q.n_gsisa,
                                  Supplier = q1.c_nosup,
                                  Box = (q1.n_box.HasValue ? q1.n_box.Value : 0),
                                  BatchID = q.c_batch
                                }).Distinct().ToList();

                                    dicItemStock.Add(field.Item, listSPAC);

                                    totalCurrentStockGood = listSPAC.Sum(t => t.Qty);
                                }
                                if (field.Quantity > 0)
                                {
                                    if (totalCurrentStockGood > 0)
                                    {

                                    }
                                    if (totalCurrentStockGood < (field.Quantity))
                                    {
                                        totalCurrentStockGood = (totalCurrentStockGood < field.Quantity ? totalCurrentStockGood : field.Quantity);
                                    }
                                }

                                if (listSPAC.Count > 0)
                                {
                                    nQtyAllocGood = (field.Quantity < totalCurrentStockGood ? field.Quantity : totalCurrentStockGood);
                                    decimal GAqum = 0, gE = 0, gI = 0;
                                    bool GLop = true;

                                    istSPAC = listSPAC.FindAll(delegate(SJClassComponent sjd)
                                    {
                                        if (!sjd.BatchID.Trim().Equals(field.Batch.Trim(), StringComparison.OrdinalIgnoreCase))
                                        {
                                            return false;
                                        }

                                        return true;
                                    });

                                    for (nLoopC = 0; nLoopC < istSPAC.Count; nLoopC++)
                                    {
                                        if (istSPAC[nLoopC].Qty > 0)
                                        {
                                            gI = nQtyAllocGood - istSPAC[nLoopC].Qty;

                                            if (GLop == true)
                                            {
                                                gE = nQtyAllocGood - gI;
                                                GAqum = GAqum + gE;
                                            }
                                            if (GAqum > nQtyAllocGood)
                                            {
                                                gE = nQtyAllocGood - (GAqum - gE);
                                                GAqum = GAqum + gE;
                                            }
                                            if (GLop == false)
                                            {
                                                gE = 0;
                                            }


                                            if (gE != 0)
                                            {
                                                listSjd2.Add(new LG_SJD2()
                                                {
                                                    c_batch = field.Batch,
                                                    c_gdg = gdgAsal,
                                                    c_iteno = field.Item,
                                                    c_sjno = sjID,
                                                    c_spgno = field.NomorSP,
                                                    c_rnno = istSPAC[nLoopC].RefID,
                                                    n_bqty = 0,
                                                    n_gqty = gE,
                                                    n_bsisa = 0,
                                                    n_gsisa = gE
                                                });

                                                trapsjd2 += gE;

                                                totalCurrentStockGood -= gE;

                                                if (istSPAC[nLoopC].SignID.Equals("RN") || istSPAC[nLoopC].SignID.Equals("RR"))
                                                {
                                                    rnd1 = (from q in db.LG_RND1s
                                                            where q.c_gdg == gdgAsal &&
                                                            q.c_iteno == istSPAC[nLoopC].Item
                                                            && q.c_batch == field.Batch
                                                            && q.c_rnno == istSPAC[nLoopC].RefID
                                                            select q).Take(1).SingleOrDefault();
                                                    if (rnd1 != null)
                                                    {
                                                        rnd1.n_gsisa -= gE;

                                                        istSPAC[nLoopC].Qty -= gE;

                                                        spgd1.n_sisa -= gE;

                                                        if (rnd1.n_gsisa < 0) //cek rn minus
                                                        {
                                                            result = "Qty RN Kurang (add SJ) " + field.Item + " " + field.Batch;
                                                            rpe = ResponseParser.ResponseParserEnum.IsFailed;
                                                            if (db.Transaction != null)
                                                            {
                                                                db.Transaction.Rollback();
                                                            }
                                                            goto endLogic;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    comboh = (from q in db.LG_ComboHs
                                                              where q.c_gdg == gdgAsal &&
                                                              q.c_iteno == istSPAC[nLoopC].Item
                                                              && q.c_batch == field.Batch
                                                              && q.c_combono == istSPAC[nLoopC].RefID
                                                              select q).Take(1).SingleOrDefault();

                                                    if (comboh != null)
                                                    {
                                                        comboh.n_gsisa -= gE;

                                                        spgd1.n_sisa -= gE;
                                                    }
                                                }

                                                totalDetails++;
                                            }

                                            if (GAqum > nQtyAllocGood)
                                            {
                                                GLop = false;
                                            }
                                        }
                                    }

                                    if (listSjd2.Count > 0)
                                    {
                                        listSjd1.Add(new LG_SJD1()
                                        {
                                            c_batch = field.Batch,
                                            c_gdg = gdgAsal,
                                            c_iteno = field.Item,
                                            c_sjno = sjID,
                                            c_spgno = field.NomorSP,
                                            n_bqty = field.BadQuantity,
                                            n_booked_bad = field.BadQuantity,
                                            n_booked = nQtyAllocGood,
                                            n_gqty = nQtyAllocGood,
                                            l_expired = field.isED,
                                            v_ket_ed = field.isED ? field.accket : null,
                                            c_acc_ed = field.isED ? nipEntry : null
                                        });

                                        if (trapsjd2 != nQtyAllocGood)
                                        {
                                            result = "Qty SJ selisih " + field.Item + " " + field.Batch + "(add SJ)";
                                            rpe = ResponseParser.ResponseParserEnum.IsFailed;
                                            if (db.Transaction != null)
                                            {
                                                db.Transaction.Rollback();
                                            }
                                            goto endLogic;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            
            #endregion

                dic = new Dictionary<string, string>();

                if (totalDetails > 0)
                {
                    if ((listSjd1 != null) && (listSjd1.Count > 0))
                    {
                        db.LG_SJD1s.InsertAllOnSubmit(listSjd1.ToArray());
                        listSjd1.Clear();

                        if ((listSjd2 != null) && (listSjd2.Count > 0))
                        {
                            db.LG_SJD2s.InsertAllOnSubmit(listSjd2.ToArray());
                            listSjd2.Clear();
                        }
                    }

            dic.Add("SJ", sjID);
            dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                    result = string.Format("Total {0} detail(s)", totalDetails);

            hasAnyChanges = true;
          }

            #endregion
        } 
        else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify

          if (string.IsNullOrEmpty(sjID))
          {
            result = "Nomor Surat Jalan dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          sjh = (from q in db.LG_SJHs
                 where (q.c_gdg == gdgAsal) && (q.c_sjno == sjID)
                 select q).Take(1).SingleOrDefault();

          if (sjh == null)
          {
            result = "Nomor Surat Jalan tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (sjh.l_delete.HasValue && sjh.l_delete.Value)
          {
            result = "Tidak dapat mengubah nomor Surat Jalan yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (sjh.l_confirm.HasValue && sjh.l_confirm.Value)
          {
            result = "Tidak dapat mengubah nomor Surat Jalan yang sudah terkonfirm.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (sjh.l_confirm.HasValue && sjh.l_confirm.Value)
          {
            result = "Tidak dapat mengubah nomor Surat Jalan yang sudah terkonfirm.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, sjh.d_sjdate))
          {
            result = "Surat Jalan tidak dapat diubah, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.HasExp(db, sjID))
          {
            result = "Surat Jalan yang sudah terdapat Delivery Order tidak dapat diubah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
          {
            sjh.v_ket = structure.Fields.Keterangan;
          }

          //isConfirmed = (sjh.l_confirm.HasValue ? sjh.l_confirm.Value : false);

          if (structure.Method.Equals("ModifyConfirm", StringComparison.OrdinalIgnoreCase))
          {
            sjh.l_confirm = structure.Fields.IsConfirm;

            isConfirmed = true;

            sjh.c_confirm = nipEntry;
            sjh.d_confirm = DateTime.Now;
          }
          else
          {
            isConfirmed = (sjh.l_confirm.HasValue ? sjh.l_confirm.Value : false);
          }

          sjh.c_update = nipEntry;
          if (sjh.l_status == false)
          {
              sjh.d_update = DateTime.Now;
          }
          #region Populate Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listSjd1 = new List<LG_SJD1>();
            listSjd2 = new List<LG_SJD2>();
            listSPAC = new List<SJClassComponent>();
            listSPGD1 = new List<LG_SPGD1>();
            listSjd3 = new List<LG_SJD3>();
            listRND1 = new List<LG_RND1>();
            listComboh = new List<LG_ComboH>();
            rnd1 = new LG_RND1();
            comboh = new LG_ComboH();
            spgd1 = new LG_SPGD1();
            dicItemStock = new Dictionary<string, List<SJClassComponent>>(StringComparer.OrdinalIgnoreCase);
            trapsjd2 = 0;

            listSjd1 = (from q in db.LG_SJHs
                        join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                        where (q.c_gdg == gdgAsal) && (q.c_sjno == sjID)
                          && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                        select q1).Distinct().ToList();

            listSjd2 = (from q in db.LG_SJHs
                        join q1 in db.LG_SJD2s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                        where (q.c_gdg == gdgAsal) && (q.c_sjno == sjID)
                          && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                        select q1).Distinct().ToList();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified) && (field.Quantity > 0))
              {
                #region New

                spgd1 = (from q in db.LG_SPGHs
                         join q1 in db.LG_SPGD1s on new { c_gdg = q.c_gdg1, q.c_spgno } equals new { q1.c_gdg, q1.c_spgno }
                         where (q.c_spgno == field.NomorSP)
                          && (q.c_gdg1 == gdgTujuan) && (q1.c_iteno == field.Item)
                          && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                         select q1).Take(1).SingleOrDefault();

                if (spgd1 != null)
                {
                  nQtyAllocGood = 0;
                  spAlloc = (spgd1.n_sisa.HasValue ? spgd1.n_sisa.Value : 0);
                  spQtyReloc = (spAlloc > field.Quantity ? field.Quantity : spAlloc);
                  trapsjd2 = 0;

                  if (spAlloc <= 0)
                  {
                    continue;
                  }

                  if (dicItemStock.ContainsKey(field.Item))
                  {
                    listSPAC = dicItemStock[field.Item];
                  }
                  else
                  {
                    listSPAC = (from q in ScmsSoaLibrary.Modules.GlobalQuery.ViewStockLite(db, gdgAsal, field.Item)
                                join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                from qBat in q_2.DefaultIfEmpty()
                                where (q.n_gsisa > 0) && (q.c_batch == field.Batch)
                                orderby qBat.d_expired ascending
                                select new SJClassComponent()
                                {
                                  RefID = q.c_no,
                                  SignID = (q.c_table == null ? string.Empty : q.c_table.Trim()),
                                  Item = q.c_iteno,
                                  Qty = q.n_gsisa,
                                  Supplier = q1.c_nosup,
                                  Box = (q1.n_box.HasValue ? q1.n_box.Value : 0)
                                }).Distinct().ToList();

                    dicItemStock.Add(field.Item, listSPAC);

                    totalCurrentStockGood = listSPAC.Sum(t => t.Qty);
                  }
                  if (field.Quantity > 0)
                  {
                    if (totalCurrentStockGood > 0)
                    {

                    }
                    if (totalCurrentStockGood < (field.Quantity))
                    {
                      totalCurrentStockGood = (totalCurrentStockGood < field.Quantity ? totalCurrentStockGood : field.Quantity);
                    }
                  }
                  if (listSPAC.Count > 0)
                  {
                    nQtyAllocGood = field.Quantity;
                    decimal GAqum = 0, gE = 0, gI = 0;
                    bool GLop = true;

                    for (nLoopC = 0; nLoopC < listSPAC.Count; nLoopC++)
                    {
                      if (listSPAC[nLoopC].Qty > 0)
                      {
                        gI = nQtyAllocGood - listSPAC[nLoopC].Qty;

                        if (GLop == true)
                        {
                          gE = nQtyAllocGood - gI;
                          GAqum = GAqum + gE;
                        }
                        if (GAqum > nQtyAllocGood)
                        {
                          gE = nQtyAllocGood - (GAqum - gE);
                          GAqum = GAqum + gE;
                        }
                        if (GLop == false)
                        {
                          gE = 0;
                        }

                        if (gE != 0)
                        {
                          sjd2 = new LG_SJD2()
                          {
                            c_batch = field.Batch,
                            c_gdg = gdgAsal,
                            c_iteno = field.Item,
                            c_sjno = sjID,
                            c_spgno = field.NomorSP,
                            c_rnno = listSPAC[nLoopC].RefID,
                            n_bqty = 0,
                            n_gqty = gE,
                            n_bsisa = 0,
                            n_gsisa = gE
                          };

                          trapsjd2 += gE;

                          db.LG_SJD2s.InsertOnSubmit(sjd2);

                          #region Old

                          //listSjd2.Add(new LG_SJD2()
                          //{
                          //  c_batch = field.Batch,
                          //  c_gdg = gdgAsal,
                          //  c_iteno = field.Item,
                          //  c_sjno = sjID,
                          //  c_spgno = field.NomorSP,
                          //  c_rnno = listSPAC[nLoopC].RefID,
                          //  n_bqty = 0,
                          //  n_gqty = gE,
                          //  n_bsisa = 0,
                          //  n_gsisa = gE
                          //});

                          #endregion

                          if (listSPAC[nLoopC].SignID.Equals("RN") || listSPAC[nLoopC].SignID.Equals("RR"))
                          {
                            rnd1 = (from q in db.LG_RND1s
                                    where q.c_gdg == gdgAsal &&
                                    q.c_iteno == listSPAC[nLoopC].Item
                                    && q.c_batch == field.Batch
                                    && q.c_rnno == listSPAC[nLoopC].RefID
                                    select q).Take(1).SingleOrDefault();
                            if (rnd1 != null)
                            {
                              rnd1.n_gsisa -= gE;

                              listSPAC[nLoopC].Qty -= gE;

                              if (rnd1.n_gsisa < 0) //cek rn minus
                              {
                                  result = "Qty RN Kurang (modify SJ) " + field.Item + " " + field.Batch;
                                  rpe = ResponseParser.ResponseParserEnum.IsFailed;
                                  if (db.Transaction != null)
                                  {
                                      db.Transaction.Rollback();
                                  }
                                  goto endLogic;
                              }
                            }
                            else
                            {
                              result = "Surat Jalan untuk item " + sjd2.c_iteno + " dan batch " + sjd2.c_batch + " tersebut Stock - nya tidak terbaca system .";

                              rpe = ResponseParser.ResponseParserEnum.IsFailed;

                              if (db.Transaction != null)
                              {
                                db.Transaction.Rollback();
                              }

                              goto endLogic;
                            }
                          }
                          else
                          {
                            comboh = (from q in db.LG_ComboHs
                                      where q.c_gdg == gdgAsal &&
                                      q.c_iteno == listSPAC[nLoopC].Item
                                      && q.c_batch == field.Batch
                                      && q.c_combono == listSPAC[nLoopC].RefID
                                      select q).Take(1).SingleOrDefault();

                            if (comboh != null)
                            {
                              comboh.n_gsisa -= gE;
                            }
                            else
                            {
                              result = "Surat Jalan untuk item " + sjd2.c_iteno + " dan batch " + sjd2.c_batch + " tersebut Stock - nya tidak terbaca system .";

                              rpe = ResponseParser.ResponseParserEnum.IsFailed;

                              if (db.Transaction != null)
                              {
                                db.Transaction.Rollback();
                              }

                              goto endLogic;
                            }
                          }

                          totalDetails++;
                        }

                        if (GAqum > nQtyAllocGood)
                        {
                          GLop = false;
                        }
                      }
                    }
                  }

                  sjd1 = new LG_SJD1()
                  {
                    c_batch = field.Batch,
                    c_gdg = gdgAsal,
                    c_iteno = field.Item,
                    c_sjno = sjID,
                    c_spgno = field.NomorSP,
                    n_bqty = field.BadQuantity,
                    n_booked_bad = field.BadQuantity,
                    n_booked = field.Quantity,
                    n_gqty = field.Quantity,
                    l_expired = field.isED,
                    v_ket_ed = string.IsNullOrEmpty(field.accket) ? null : field.accket,
                    c_acc_ed = field.isED ? nipEntry : null
                  };

                  if (trapsjd2 != nQtyAllocGood)
                  {
                      result = "Qty SJ selisih " + field.Item + " " + field.Batch + "(modify add SJ)";
                      rpe = ResponseParser.ResponseParserEnum.IsFailed;
                      if (db.Transaction != null)
                      {
                          db.Transaction.Rollback();
                      }
                      goto endLogic;
                  }

                  db.LG_SJD1s.InsertOnSubmit(sjd1);

                  spgd1.n_sisa -= field.Quantity;

                  #region Old

                  //if (listSjd2.Count > 0)
                  //{
                  //  listSjd1.Add(new LG_SJD1()
                  //  {
                  //    c_batch = field.Batch,
                  //    c_gdg = gdgAsal,
                  //    c_iteno = field.Item,
                  //    c_sjno = sjID,
                  //    c_spgno = field.NomorSP,
                  //    n_bqty = field.BadQuantity,
                  //    n_booked_bad = field.BadQuantity,
                  //    n_booked = field.Quantity,
                  //    n_gqty = field.Quantity
                  //  });


                  //  spgd1.n_sisa -= field.Quantity;
                  //}

                  #endregion
                }

                #region Old

                //if ((listSjd1 != null) && (listSjd1.Count > 0))
                //{
                //  db.LG_SJD1s.InsertAllOnSubmit(listSjd1.ToArray());
                //  db.LG_SJD2s.InsertAllOnSubmit(listSjd2.ToArray());
                //  listSjd2.Clear();
                //  listSjd1.Clear();
                //}
                #endregion

                #endregion
              }
              else if ((field != null) && (!field.IsNew) && field.IsDelete && (!field.IsModified))
              {
                #region Delete

                sjd2 = (from q in db.LG_SJD2s
                        where q.c_batch == field.Batch && q.c_iteno == field.Item &&
                        q.c_sjno == sjID && q.c_spgno == field.NomorSP
                        select q).Distinct().Take(1).SingleOrDefault();

                sjd1 = (from q in db.LG_SJD1s
                        where q.c_batch == field.Batch && q.c_iteno == field.Item
                        && q.c_sjno == sjID && q.c_spgno == field.NomorSP
                        select q).Distinct().Take(1).SingleOrDefault();

                if (sjd1 != null && sjd2 != null)
                {
                  spQtyReloc = (sjd2.n_gqty.HasValue ? sjd2.n_gqty.Value : 0);
                  spAlloc = (sjd2.n_gsisa.HasValue ? sjd2.n_gsisa.Value : 0);

                  if (spAlloc.Equals(spQtyReloc))
                  {
                    #region Reverse RN

                    listSjd2 = (from q in db.LG_SJD2s
                                where q.c_sjno == sjID && q.c_iteno == sjd2.c_iteno &&
                                q.c_batch == sjd2.c_batch && q.c_spgno == field.NomorSP
                                select q).Distinct().ToList();


                    if ((listSjd2 != null) && (listSjd2.Count > 0))
                    {
                      for (nLoopC = 0; nLoopC < listSjd2.Count; nLoopC++)
                      {
                        sjd2 = listSjd2[nLoopC];

                        if (sjd2 != null)
                        {
                            if (sjd2.c_rnno.Substring(0, 2) == "RN" || sjd2.c_rnno.Substring(0, 2) == "RR")
                          {
                            rnd1 = (from q in db.LG_RND1s
                                    where q.c_batch == sjd2.c_batch && q.c_iteno == sjd2.c_iteno &&
                                    q.c_rnno == sjd2.c_rnno && q.c_gdg == sjh.c_gdg
                                    select q).Distinct().Take(1).SingleOrDefault();
                            if (rnd1 != null)
                            {
                              rnd1.n_gsisa += (sjd2.n_gsisa);
                              rnd1.n_bsisa += (sjd2.n_bsisa);
                            }
                            else
                            {
                              result = "Surat Jalan untuk item " + sjd2.c_iteno + " dan batch " + sjd2.c_batch + " tersebut Stock - nya tidak terbaca system .";

                              rpe = ResponseParser.ResponseParserEnum.IsFailed;

                              if (db.Transaction != null)
                              {
                                db.Transaction.Rollback();
                              }

                              goto endLogic;
                            }
                          }
                          else
                          {
                            comboh = (from q in db.LG_ComboHs
                                      where q.c_gdg == gdgAsal &&
                                      q.c_iteno == sjd2.c_iteno
                                      && q.c_batch == sjd2.c_batch
                                      && q.c_combono == sjd2.c_rnno
                                      select q).Take(1).SingleOrDefault();

                            if (comboh != null)
                            {
                              comboh.n_gsisa += (sjd1.n_gqty);
                              comboh.n_bsisa += (sjd1.n_bqty);
                            }
                            else
                            {
                              result = "Surat Jalan untuk item " + sjd2.c_iteno + " dan batch " + sjd2.c_batch + " tersebut Stock - nya tidak terbaca system .";

                              rpe = ResponseParser.ResponseParserEnum.IsFailed;

                              if (db.Transaction != null)
                              {
                                db.Transaction.Rollback();
                              }

                              goto endLogic;
                            }
                          }

                          #region Log SJD3

                          listSjd3.Add(new LG_SJD3()
                          {
                            c_sjno = sjID,
                            c_batch = sjd2.c_batch,
                            c_entry = nipEntry,
                            c_gdg = sjd2.c_gdg,
                            c_iteno = sjd2.c_iteno,
                            c_rnno = sjd2.c_rnno,
                            c_spgno = sjd2.c_spgno,
                            d_entry = date.Date,
                            n_bqty = sjd2.n_bqty,
                            n_bsisa = sjd2.n_bsisa,
                            n_gqty = sjd2.n_gqty,
                            n_gsisa = sjd2.n_gsisa,
                            v_type = "03",
                            v_ket_del = field.KeteranganMod,
                            l_expired = sjd1.l_expired,
                            v_ket_ed = sjd1.l_expired == true ? sjd1.v_ket_ed : null,
                            c_acc_ed = sjd1.l_expired == true ? sjd1.c_acc_ed : null
                          });

                          #region Delete SJD2

                          db.LG_SJD2s.DeleteOnSubmit(sjd2);

                          #endregion

                          #endregion
                        }
                      }
                    }

                    #endregion

                    #region Delete SJD1

                    if (sjd1 != null)
                    {
                      db.LG_SJD1s.DeleteOnSubmit(sjd1);

                      spgd1 = (from q in db.LG_SPGHs
                               join q1 in db.LG_SPGD1s on new { c_gdg = q.c_gdg1, q.c_spgno } equals new { q1.c_gdg, q1.c_spgno }
                               where (q.c_spgno == sjd1.c_spgno)
                                && (q.c_gdg1 == gdgTujuan) && (q1.c_iteno == sjd1.c_iteno)
                                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                               select q1).Take(1).SingleOrDefault();

                      if (spgd1 != null)
                      {
                        spgd1.n_sisa += sjd1.n_gqty;
                      }
                    }

                    #endregion
                  }
                }

                #endregion
              }
              if ((field != null) && (!field.IsNew) && (field.IsModified) && (!field.IsDelete) && (field.Quantity > 0))
              {
                #region Modify

                sjd1 = listSjd1.Find(delegate(LG_SJD1 sjd)
                {
                    return field.Item.Equals((string.IsNullOrEmpty(sjd.c_iteno) ? string.Empty : sjd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                      field.Batch.Equals((string.IsNullOrEmpty(sjd.c_batch) ? string.Empty : sjd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                      field.NomorSP.Equals((string.IsNullOrEmpty(sjd.c_spgno) ? string.Empty : sjd.c_spgno.Trim()), StringComparison.OrdinalIgnoreCase);
                });


                spgd1 = (from q in db.LG_SPGD1s
                         where q.c_gdg == gdgTujuan && q.c_spgno == field.NomorSP
                          && q.c_iteno == field.Item
                         select q).Take(1).SingleOrDefault();



                if (listSjd2.Count > 0)
                {
                    var LisSJ2T = (from q in db.LG_SJD2s
                                   where q.c_sjno == sjID && q.c_batch == field.Batch
                                   && q.c_gdg == gdgAsal && q.c_iteno == field.Item
                                   && q.c_spgno == field.NomorSP
                                   select q).ToList();

                    decimal SumSisaSj1 = 0,
                       SumQtySj1  = 0;

                    SumSisaSj1 = LisSJ2T.Sum(x => x.n_gsisa).Value;
                    SumQtySj1 = LisSJ2T.Sum(x => x.n_gqty).Value;

                    if (SumSisaSj1 == SumQtySj1)
                    {
                        decimal SumQty = field.Quantity,
                          Alokasi = 0,
                          nToAlloc = 0;

                        nToAlloc = SumQtySj1 - field.Quantity;

                        for (nLoopC = 0; nLoopC < LisSJ2T.Count; nLoopC++)
                        {
                            sjd2 = (from q in db.LG_SJD2s
                                    where q.c_gdg == gdgAsal && q.c_spgno == field.NomorSP
                                    && q.c_sjno == sjID
                                    && q.c_iteno == field.Item && q.c_batch == field.Batch
                                    && q.c_rnno == LisSJ2T[nLoopC].c_rnno
                                    select q).Take(1).SingleOrDefault();

                            if (sjd2.c_rnno.Substring(0, 2) == "RN" || sjd2.c_rnno.Substring(0, 2) == "RR")
                            {
                                rnd1 = (from q in db.LG_RND1s
                                        where q.c_gdg == gdgAsal
                                        && q.c_iteno == field.Item && q.c_batch == field.Batch
                                        && q.c_rnno == LisSJ2T[nLoopC].c_rnno
                                        select q).Take(1).SingleOrDefault();

                                sjd2 = (from q in db.LG_SJD2s
                                        where q.c_gdg == gdgAsal && q.c_spgno == field.NomorSP
                                        && q.c_sjno == sjID
                                        && q.c_iteno == field.Item && q.c_batch == field.Batch
                                        && q.c_rnno == LisSJ2T[nLoopC].c_rnno
                                        select q).Take(1).SingleOrDefault();

                                Alokasi = sjd2.n_gsisa.Value;

                                

                                if (sjd2 != null)
                                {
                                    sjd2.n_gsisa -= ((nToAlloc > Alokasi ? Alokasi : nToAlloc) > 0 ? (nToAlloc > Alokasi ? Alokasi : nToAlloc) : 0);
                                    sjd2.n_gqty -= ((nToAlloc > Alokasi ? Alokasi : nToAlloc) > 0 ? (nToAlloc > Alokasi ? Alokasi : nToAlloc) : 0);

                                    //SumQty -= Alokasi;
                                }
                                if (spgd1 != null)
                                {
                                  
                                  spgd1.n_sisa += ((nToAlloc > Alokasi ? Alokasi : nToAlloc) > 0 ? (nToAlloc > Alokasi ? Alokasi : nToAlloc) : 0);
                                }

                                if (rnd1 != null)
                                {
                                  rnd1.n_gsisa += ((nToAlloc > Alokasi ? Alokasi : nToAlloc) > 0 ? (nToAlloc > Alokasi ? Alokasi : nToAlloc) : 0);
                                  
                                  if (rnd1.n_gsisa < 0) //cek rn minus
                                  {
                                      result = "Qty RN Kurang (modify SJ) " + field.Item + " " + field.Batch;
                                      rpe = ResponseParser.ResponseParserEnum.IsFailed;
                                      if (db.Transaction != null)
                                      {
                                          db.Transaction.Rollback();
                                      }
                                      goto endLogic;
                                  }
                                }
                                else
                                {
                                  result = "Surat Jalan untuk item " + sjd2.c_iteno + " dan batch " + sjd2.c_batch + " tersebut Stock - nya tidak terbaca system .";

                                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                  if (db.Transaction != null)
                                  {
                                    db.Transaction.Rollback();
                                  }

                                  goto endLogic;
                                }
                            }
                            else
                            {

                                comboh = (from q in db.LG_ComboHs
                                          where q.c_gdg == gdgAsal
                                          && q.c_iteno == field.Item && q.c_batch == field.Batch
                                          && q.c_combono == LisSJ2T[nLoopC].c_rnno
                                          select q).Take(1).SingleOrDefault();

                                Alokasi = sjd2.n_gsisa.Value;

                                if (sjd2 != null)
                                {
                                    sjd2.n_gsisa -= ((nToAlloc > Alokasi ? Alokasi : nToAlloc) > 0 ? (nToAlloc > Alokasi ? Alokasi : nToAlloc) : 0);
                                    sjd2.n_gqty -= ((nToAlloc > Alokasi ? Alokasi : nToAlloc) > 0 ? (nToAlloc > Alokasi ? Alokasi : nToAlloc) : 0);

                                    //SumQty -= Alokasi;
                                }
                                if (spgd1 != null)
                                {

                                  spgd1.n_sisa += ((nToAlloc > Alokasi ? Alokasi : nToAlloc) > 0 ? (nToAlloc > Alokasi ? Alokasi : nToAlloc) : 0);
                                }

                                if (comboh != null)
                                {
                                  comboh.n_gsisa += ((nToAlloc > Alokasi ? Alokasi : nToAlloc) > 0 ? (nToAlloc > Alokasi ? Alokasi : nToAlloc) : 0);
                                }
                                else
                                {
                                  result = "Surat Jalan untuk item " + sjd2.c_iteno + " dan batch " + sjd2.c_batch + " tersebut Stock - nya tidak terbaca system .";

                                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                  if (db.Transaction != null)
                                  {
                                    db.Transaction.Rollback();
                                  }

                                  goto endLogic;
                                }
                            }

                            nToAlloc -= ((nToAlloc > Alokasi ? Alokasi : nToAlloc) > 0 ? (nToAlloc > Alokasi ? Alokasi : nToAlloc) : 0); 
                        }
                        if (sjd1 != null)
                        {
                          sjd1.n_gqty = field.Quantity;
                          sjd1.c_type_dc = field.tipe_dc;
                          sjd1.l_expired = field.isED;
                          sjd1.v_ket_ed = field.accket;
                          sjd1.c_acc_ed = nipEntry;
                        }
                    }
                }               

                #endregion
              }
              if ((field != null) && (!field.IsNew) && (!field.IsModified) && (field.isAccModify) && (!field.IsDelete))
              {

                  sjd1 = listSjd1.Find(delegate(LG_SJD1 sjd)
                  {
                      return field.Item.Equals((string.IsNullOrEmpty(sjd.c_iteno) ? string.Empty : sjd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                        field.Batch.Equals((string.IsNullOrEmpty(sjd.c_batch) ? string.Empty : sjd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                      field.NomorSP.Equals((string.IsNullOrEmpty(sjd.c_spgno) ? string.Empty : sjd.c_spgno.Trim()), StringComparison.OrdinalIgnoreCase);
                  });

                  if (sjd1 == null)
                  {
                      continue;
                  }
                  else
                  {
                      sjd1.l_expired = field.isED;
                      sjd1.v_ket_ed = field.isED ? field.accket : null;
                      sjd1.c_acc_ed = field.isED ? nipEntry : null;
                  }
              }
            }

            if (listSjd3.Count > 0)
            {
              db.LG_SJD3s.InsertAllOnSubmit(listSjd3.ToArray());
              listSjd3.Clear();
            }

            listSjd1.Clear();
            listSjd2.Clear();
            listSjd3.Clear();

            listRND1.Clear();
            listSPGD1.Clear();
          }

          #endregion

          hasAnyChanges = true;

          #endregion
        }
        else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete

          if (string.IsNullOrEmpty(sjID))
          {
            result = "Nomor Surat Jalan dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          sjh = (from q in db.LG_SJHs
                 where (q.c_gdg == gdgAsal) && (q.c_sjno == sjID)
                 select q).Take(1).SingleOrDefault();

          if (sjh == null)
          {
            result = "Nomor Surat Jalan tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (sjh.l_delete.HasValue && sjh.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor Surat Jalan yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (sjh.l_confirm.HasValue && sjh.l_confirm.Value)
          {
            result = "Tidak dapat menghapus nomor Surat Jalan yang sudah terkonfirm.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, sjh.d_sjdate))
          {
            result = "Surat Jalan tidak dapat dihapus, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.HasExp(db, sjID))
          {
            result = "Surat Jalan yang sudah terdapat Ekspedisi tidak dapat diubah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          gdgTujuan = (sjh.c_gdg2.HasValue ? sjh.c_gdg2.Value : char.MinValue);

          sjh.c_update = nipEntry;
          if (sjh.l_status == false)
          {
              sjh.d_update = DateTime.Now;
          }
          sjh.l_delete = true;
          sjh.v_ket_mark = structure.Fields.Keterangan;

          gdgAsal = sjh.c_gdg;

          listRND1 = new List<LG_RND1>();
          listSPGD1 = new List<LG_SPGD1>();
          listSjd3 = new List<LG_SJD3>();
          listComboh = new List<LG_ComboH>();

          listSjd1 = (from q in db.LG_SJHs
                      join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                      where (q.c_gdg == gdgAsal) && (q.c_sjno == sjID)
                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      select q1).Distinct().ToList();

          listSjd2 = (from q in db.LG_SJHs
                      join q1 in db.LG_SJD2s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                      where (q.c_gdg == gdgAsal) && (q.c_sjno == sjID)
                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      select q1).Distinct().ToList();

          for (nLoop = 0; nLoop < listSjd2.Count; nLoop++)
          {
            sjd2 = listSjd2[nLoop];

            if (sjd2 != null)
            {
              rnID = (string.IsNullOrEmpty(sjd2.c_rnno) ? string.Empty : sjd2.c_rnno.Trim());
              itemID = (string.IsNullOrEmpty(sjd2.c_iteno) ? string.Empty : sjd2.c_iteno.Trim());
              batchCode = (string.IsNullOrEmpty(sjd2.c_batch) ? string.Empty : sjd2.c_batch.Trim());
              sgID = (string.IsNullOrEmpty(sjd2.c_spgno) ? string.Empty : sjd2.c_spgno.Trim());

              spAlloc = (sjd2.n_gqty.HasValue ? sjd2.n_gqty.Value : 0);
              spQtyReloc = (sjd2.n_gsisa.HasValue ? sjd2.n_gsisa.Value : 0);

              if (spQtyReloc != spAlloc)
              {
                result = "Detail Surat Jalan pernah dipakai";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                if (db.Transaction != null)
                {
                  db.Transaction.Rollback();
                }

                goto endLogic;
              }

              if (sjd2.c_rnno.Substring(0, 2) == "RN" || sjd2.c_rnno.Substring(0, 2) == "RR")
              {
                #region RN

                rnd1 = listRND1.Find(delegate(LG_RND1 rnd)
                {
                  return gdgAsal.Equals(rnd.c_gdg) &&
                    rnID.Equals((string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    itemID.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    batchCode.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                });

                if (rnd1 == null)
                {
                  rnd1 = (from q in db.LG_RNHs
                          join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                          where (q.c_gdg == gdgAsal) && (q.c_rnno == rnID)
                            && (q1.c_iteno == itemID) && (q1.c_batch == batchCode)
                            && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                          select q1).Take(1).SingleOrDefault();

                  if (rnd1 != null)
                  {
                    listRND1.Add(rnd1);
                  }
                }

                if (rnd1 == null)
                {
                  result = "RN Tidak ditemukan";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  if (db.Transaction != null)
                  {
                    db.Transaction.Rollback();
                  }

                  goto endLogic;
                }

                rnd1.n_gsisa += spAlloc;

                #endregion
              }
              else
              {
                #region Data Combo

                comboh = listComboh.Find(delegate(LG_ComboH cmb)
                {
                  return gdgAsal.Equals(cmb.c_gdg) &&
                    rnID.Equals((string.IsNullOrEmpty(cmb.c_combono) ? string.Empty : cmb.c_combono.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    itemID.Equals((string.IsNullOrEmpty(cmb.c_iteno) ? string.Empty : cmb.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    batchCode.Equals((string.IsNullOrEmpty(cmb.c_batch) ? string.Empty : cmb.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                });

                if (comboh == null)
                {
                  comboh = (from q in db.LG_ComboHs
                            where (q.c_gdg == gdgAsal) && (q.c_iteno == itemID) && (q.c_batch == batchCode)
                                     && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                            select q).Take(1).SingleOrDefault();

                  if (comboh != null)
                  {
                    listComboh.Add(comboh);
                  }
                }

                if (comboh == null)
                {
                  result = "Combo Tidak ditemukan";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  if (db.Transaction != null)
                  {
                    db.Transaction.Rollback();
                  }

                  goto endLogic;
                }

                comboh.n_gsisa += spAlloc;

                #endregion
              }

              #region SG

              spgd1 = listSPGD1.Find(delegate(LG_SPGD1 spgd)
              {
                return gdgTujuan.Equals(spgd.c_gdg) &&
                  sgID.Equals((string.IsNullOrEmpty(spgd.c_spgno) ? string.Empty : spgd.c_spgno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                  itemID.Equals((string.IsNullOrEmpty(spgd.c_iteno) ? string.Empty : spgd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase);
              });

              if (spgd1 == null)
              {
                spgd1 = (from q in db.LG_SPGHs
                         join q1 in db.LG_SPGD1s on new { c_gdg = q.c_gdg1, q.c_spgno } equals new { q1.c_gdg, q1.c_spgno }
                         where (q.c_gdg1 == gdgTujuan) && (q.c_spgno == sgID)
                           && (q1.c_iteno == sjd2.c_iteno)
                           && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                         select q1).Take(1).SingleOrDefault();

                if (spgd1 != null)
                {
                  listSPGD1.Add(spgd1);
                }
              }

              if (spgd1 == null)
              {
                result = "Surat Gudang Tidak ditemukan";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                if (db.Transaction != null)
                {
                  db.Transaction.Rollback();
                }

                goto endLogic;
              }

              #endregion

              spgd1.n_sisa += spAlloc;
              

              listSjd3.Add(new LG_SJD3()
              {
                c_batch = sjd2.c_batch,
                c_iteno = sjd2.c_iteno,
                c_gdg = sjd2.c_gdg,
                c_rnno = sjd2.c_rnno,
                c_sjno = sjd2.c_sjno,
                c_spgno = sjd2.c_spgno,
                n_bqty = sjd2.n_bqty,
                n_bsisa = sjd2.n_bsisa,
                n_gqty = sjd2.n_gqty,
                n_gsisa = sjd2.n_gsisa,
                c_entry = nipEntry,
                d_entry = date,
                v_type = "03",
                v_ket_del = string.Concat((isConfirmed ? "Confirm : " : string.Empty),
                      (string.IsNullOrEmpty(structure.Fields.Keterangan) ? "Human error" : structure.Fields.Keterangan)),
                l_expired = sjd1.l_expired,
                v_ket_ed = sjd1.l_expired == true ? sjd1.v_ket_ed : null,
                c_acc_ed = sjd1.l_expired == true ? sjd1.c_acc_ed : null
              });

            }
          }

          if (listSjd1.Count > 0)
          {
            db.LG_SJD1s.DeleteAllOnSubmit(listSjd1.ToArray());
            listSjd1.Clear();
          }
          if (listSjd2.Count > 0)
          {
            db.LG_SJD2s.DeleteAllOnSubmit(listSjd2.ToArray());
            listSjd2.Clear();
          }
          if (listSjd3.Count > 0)
          {
            db.LG_SJD3s.InsertAllOnSubmit(listSjd3.ToArray());
            listSjd3.Clear();
          }

          hasAnyChanges = true;

          #endregion
        }
        else if (structure.Method.Equals("ModifyConfirm", StringComparison.OrdinalIgnoreCase))
        {
          #region ModifyConfirm

          if (string.IsNullOrEmpty(sjID))
          {
            result = "Nomor Surat Jalan dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          sjh = (from q in db.LG_SJHs
                 where (q.c_gdg == gdgAsal) && (q.c_sjno == sjID)
                 select q).Take(1).SingleOrDefault();

          if (sjh == null)
          {
            result = "Nomor Surat Jalan tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (sjh.l_delete.HasValue && sjh.l_delete.Value)
          {
            result = "Tidak dapat mengubah nomor Surat Jalan yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          //else if (sjh.l_confirm.HasValue && sjh.l_confirm.Value)
          //{
          //  result = "Tidak dapat mengubah nomor Surat Jalan yang sudah terkonfirm.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}
          else if (Commons.IsClosingLogistik(db, sjh.d_sjdate))
          {
            result = "Surat Jalan tidak dapat diubah, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.HasExp(db, sjID))
          {
            result = "Surat Jalan yang sudah terdapat Delivery Order tidak dapat diubah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
          {
            sjh.v_ket = structure.Fields.Keterangan;
          }

          //isConfirmed = (sjh.l_confirm.HasValue ? sjh.l_confirm.Value : false);

          sjh.l_confirm = structure.Fields.IsConfirm;

          sjh.c_confirm = nipEntry;
          sjh.d_confirm = DateTime.Now;

          #region Populate Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listSjd1 = (from q in db.LG_SJHs
                        join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                        where (q.c_gdg == gdgAsal) && (q.c_sjno == sjID)
                          && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                        select q1).Distinct().ToList();

            listSjd2 = (from q in db.LG_SJHs
                        join q1 in db.LG_SJD2s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                        where (q.c_gdg == gdgAsal) && (q.c_sjno == sjID)
                          && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                        select q1).Distinct().ToList();

            listSPAC = new List<SJClassComponent>();
            listSPGD1 = new List<LG_SPGD1>();
            listSjd3 = new List<LG_SJD3>();
            listRND1 = new List<LG_RND1>();
            listComboh = new List<LG_ComboH>();
            rnd1 = new LG_RND1();
            comboh = new LG_ComboH();
            spgd1 = new LG_SPGD1();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified))
              {
                #region New

                spgd1 = (from q in db.LG_SPGHs
                         join q1 in db.LG_SPGD1s on new { c_gdg = q.c_gdg1, q.c_spgno } equals new { q1.c_gdg, q1.c_spgno }
                         where (q.c_spgno == field.NomorSP)
                          && (q.c_gdg1 == gdgTujuan) && (q1.c_iteno == field.Item)
                          && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                         select q1).Take(1).SingleOrDefault();

                if (spgd1 != null)
                {
                  nQtyAllocGood = 0;
                  spAlloc = (spgd1.n_sisa.HasValue ? spgd1.n_sisa.Value : 0);
                  spQtyReloc = (spAlloc > field.Quantity ? field.Quantity : spAlloc);

                  if (spAlloc <= 0)
                  {
                    continue;
                  }

                  listSPAC = (from q in ScmsSoaLibrary.Modules.GlobalQuery.ViewStockLite(db, gdgAsal, field.Item)
                              join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                              join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                              from qBat in q_2.DefaultIfEmpty()
                              where (q.n_gsisa > 0) && (q.c_batch == field.Batch)
                              orderby qBat.d_expired ascending
                              select new SJClassComponent()
                              {
                                RefID = q.c_no,
                                SignID = (q.c_table == null ? string.Empty : q.c_table.Trim()),
                                Item = q.c_iteno,
                                Qty = q.n_gsisa,
                                Supplier = q1.c_nosup,
                                Box = (q1.n_box.HasValue ? q1.n_box.Value : 0)
                              }).Distinct().ToList();

                  if (listSPAC.Count > 0)
                  {
                    nQtyAllocGood = field.Quantity;
                    decimal GAqum = 0, gE = 0, gI = 0;
                    bool GLop = true;

                    for (nLoopC = 0; nLoopC < listSPAC.Count; nLoopC++)
                    {
                      if (listSPAC[nLoopC].Qty > 0)
                      {
                        gI = nQtyAllocGood - listSPAC[nLoopC].Qty;

                        if (GLop == true)
                        {
                          gE = nQtyAllocGood - gI;
                          GAqum = GAqum + gE;
                        }
                        if (GAqum > nQtyAllocGood)
                        {
                          gE = nQtyAllocGood - (GAqum - gE);
                          GAqum = GAqum + gE;
                        }
                        if (GLop == false)
                        {
                          gE = 0;
                        }

                        if (gE != 0)
                        {
                          sjd2 = new LG_SJD2()
                          {
                            c_batch = field.Batch,
                            c_gdg = gdgAsal,
                            c_iteno = field.Item,
                            c_sjno = sjID,
                            c_spgno = field.NomorSP,
                            c_rnno = listSPAC[nLoopC].RefID,
                            n_bqty = 0,
                            n_gqty = gE,
                            n_bsisa = 0,
                            n_gsisa = gE
                          };

                          db.LG_SJD2s.InsertOnSubmit(sjd2);

                          #region Old

                          //listSjd2.Add(new LG_SJD2()
                          //{
                          //  c_batch = field.Batch,
                          //  c_gdg = gdgAsal,
                          //  c_iteno = field.Item,
                          //  c_sjno = sjID,
                          //  c_spgno = field.NomorSP,
                          //  c_rnno = listSPAC[nLoopC].RefID,
                          //  n_bqty = 0,
                          //  n_gqty = gE,
                          //  n_bsisa = 0,
                          //  n_gsisa = gE
                          //});

                          #endregion

                          if (listSPAC[nLoopC].SignID.Equals("RN") || listSPAC[nLoopC].SignID.Equals("RR"))
                          {
                            rnd1 = (from q in db.LG_RND1s
                                    where q.c_gdg == gdgAsal &&
                                    q.c_iteno == listSPAC[nLoopC].Item
                                    && q.c_batch == field.Batch
                                    && q.c_rnno == listSPAC[nLoopC].RefID
                                    select q).Take(1).SingleOrDefault();
                            if (rnd1 != null)
                            {
                              rnd1.n_gsisa -= gE;
                            }
                            else
                            {
                              result = "Surat Jalan untuk item " + sjd2.c_iteno + " dan batch " + sjd2.c_batch + " tersebut Stock - nya tidak terbaca system .";

                              rpe = ResponseParser.ResponseParserEnum.IsFailed;

                              if (db.Transaction != null)
                              {
                                db.Transaction.Rollback();
                              }

                              goto endLogic;
                            }
                          }
                          else
                          {
                            comboh = (from q in db.LG_ComboHs
                                      where q.c_gdg == gdgAsal &&
                                      q.c_iteno == listSPAC[nLoopC].Item
                                      && q.c_batch == field.Batch
                                      && q.c_combono == listSPAC[nLoopC].RefID
                                      select q).Take(1).SingleOrDefault();

                            if (comboh != null)
                            {
                              comboh.n_gsisa -= gE;
                            }
                            else
                            {
                              result = "Surat Jalan untuk item " + sjd2.c_iteno + " dan batch " + sjd2.c_batch + " tersebut Stock - nya tidak terbaca system .";

                              rpe = ResponseParser.ResponseParserEnum.IsFailed;

                              if (db.Transaction != null)
                              {
                                db.Transaction.Rollback();
                              }

                              goto endLogic;
                            }
                          }

                          totalDetails++;
                        }

                        if (GAqum > nQtyAllocGood)
                        {
                          GLop = false;
                        }
                      }
                    }
                  }

                  sjd1 = new LG_SJD1()
                  {
                    c_batch = field.Batch,
                    c_gdg = gdgAsal,
                    c_iteno = field.Item,
                    c_sjno = sjID,
                    c_spgno = field.NomorSP,
                    n_bqty = field.BadQuantity,
                    n_booked_bad = field.BadQuantity,
                    n_booked = field.Quantity,
                    n_gqty = field.Quantity,
                    l_expired = field.isED,
                    v_ket_ed = field.isED ? field.accket : null,
                    c_acc_ed = field.isED ? nipEntry : null
                  };

                  db.LG_SJD1s.InsertOnSubmit(sjd1);

                  spgd1.n_sisa -= field.Quantity;

                  #region Old

                  //if (listSjd2.Count > 0)
                  //{
                  //  listSjd1.Add(new LG_SJD1()
                  //  {
                  //    c_batch = field.Batch,
                  //    c_gdg = gdgAsal,
                  //    c_iteno = field.Item,
                  //    c_sjno = sjID,
                  //    c_spgno = field.NomorSP,
                  //    n_bqty = field.BadQuantity,
                  //    n_booked_bad = field.BadQuantity,
                  //    n_booked = field.Quantity,
                  //    n_gqty = field.Quantity
                  //  });


                  //  spgd1.n_sisa -= field.Quantity;
                  //}

                  #endregion
                }

                #region Old

                //if ((listSjd1 != null) && (listSjd1.Count > 0))
                //{
                //  db.LG_SJD1s.InsertAllOnSubmit(listSjd1.ToArray());
                //  db.LG_SJD2s.InsertAllOnSubmit(listSjd2.ToArray());
                //  listSjd2.Clear();
                //  listSjd1.Clear();
                //}

                #endregion

                #endregion
              }
              else if ((field != null) && (!field.IsNew) && field.IsDelete && (!field.IsModified))
              {
                #region Delete

                sjd2 = (from q in db.LG_SJD2s
                        where q.c_batch == field.Batch && q.c_iteno == field.Item &&
                        q.c_sjno == sjID
                        select q).Distinct().Take(1).SingleOrDefault();

                sjd1 = (from q in db.LG_SJD1s
                        where q.c_batch == field.Batch && q.c_iteno == field.Item
                        && q.c_sjno == sjID && q.c_spgno == field.NomorSP
                        select q).Distinct().Take(1).SingleOrDefault();

                if (sjd1 != null)
                {
                  spQtyReloc = (sjd2.n_gqty.HasValue ? sjd2.n_gqty.Value : 0);
                  spAlloc = (sjd2.n_gsisa.HasValue ? sjd2.n_gsisa.Value : 0);

                  if (spAlloc.Equals(spQtyReloc))
                  {
                    #region Reverse RN

                    listSjd2 = (from q in db.LG_SJD2s
                                where q.c_sjno == sjID && q.c_iteno == sjd2.c_iteno &&
                                q.c_batch == sjd2.c_batch && q.c_spgno == field.NomorSP
                                select q).Distinct().ToList();


                    if ((listSjd2 != null) && (listSjd2.Count > 0))
                    {
                      for (nLoopC = 0; nLoopC < listSjd2.Count; nLoopC++)
                      {
                        sjd2 = listSjd2[nLoopC];

                        if (sjd2 != null)
                        {
                            if (sjd2.c_rnno.Substring(0, 2) == "RN" || sjd2.c_rnno.Substring(0, 2) == "RR")
                          {
                            rnd1 = (from q in db.LG_RND1s
                                    where q.c_batch == sjd2.c_batch && q.c_iteno == sjd2.c_iteno &&
                                    q.c_rnno == sjd2.c_rnno && q.c_gdg == sjh.c_gdg
                                    select q).Distinct().Take(1).SingleOrDefault();

                            if (rnd1 != null)
                            {
                              rnd1.n_gsisa += (sjd2.n_gsisa);
                              rnd1.n_bsisa += (sjd2.n_bsisa);
                            }
                            else
                            {
                              result = "Surat Jalan untuk item " + sjd2.c_iteno + " dan batch "+ sjd2.c_batch +" tersebut Stock - nya tidak terbaca system .";

                              rpe = ResponseParser.ResponseParserEnum.IsFailed;

                              if (db.Transaction != null)
                              {
                                db.Transaction.Rollback();
                              }

                              goto endLogic;
                            }
                          }
                          else
                          {
                            comboh = (from q in db.LG_ComboHs
                                      where q.c_gdg == gdgAsal &&
                                      q.c_iteno == sjd2.c_iteno
                                      && q.c_batch == sjd2.c_batch
                                      && q.c_combono == sjd2.c_rnno
                                      select q).Take(1).SingleOrDefault();

                            if (comboh != null)
                            {
                              comboh.n_gsisa += (sjd1.n_gqty);
                              comboh.n_bsisa += (sjd1.n_bqty);
                            }
                            else
                            {
                              result = "Surat Jalan untuk item " + sjd2.c_iteno + " dan batch " + sjd2.c_batch + " tersebut Stock - nya tidak terbaca system .";

                              rpe = ResponseParser.ResponseParserEnum.IsFailed;

                              if (db.Transaction != null)
                              {
                                db.Transaction.Rollback();
                              }

                              goto endLogic;
                            }
                          }

                          #region Log SJD3

                          listSjd3.Add(new LG_SJD3()
                          {
                            c_sjno = sjID,
                            c_batch = sjd2.c_batch,
                            c_entry = nipEntry,
                            c_gdg = sjd2.c_gdg,
                            c_iteno = sjd2.c_iteno,
                            c_rnno = sjd2.c_rnno,
                            c_spgno = sjd2.c_spgno,
                            d_entry = date.Date,
                            n_bqty = sjd2.n_bqty,
                            n_bsisa = sjd2.n_bsisa,
                            n_gqty = sjd2.n_gqty,
                            n_gsisa = sjd2.n_gsisa,
                            v_type = "03",
                            v_ket_del = field.KeteranganMod,
                            l_expired = sjd1.l_expired,
                            v_ket_ed = sjd1.l_expired == true ? sjd1.v_ket_ed : null,
                            c_acc_ed = sjd1.l_expired == true ? sjd1.c_acc_ed : null
                          });

                          #region Delete SJD2

                          db.LG_SJD2s.DeleteOnSubmit(sjd2);

                          #endregion

                          #endregion
                        }
                      }
                    }

                    #endregion

                    #region Delete SJD1

                    if (sjd1 != null)
                    {
                      spgd1 = (from q in db.LG_SPGHs
                               join q1 in db.LG_SPGD1s on new { c_gdg = q.c_gdg1, q.c_spgno } equals new { q1.c_gdg, q1.c_spgno }
                               where (q.c_spgno == sjd1.c_spgno)
                                && (q.c_gdg1 == gdgTujuan) && (q1.c_iteno == sjd1.c_iteno)
                                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                               select q1).Take(1).SingleOrDefault();

                      if (spgd1 != null)
                      {
                        spgd1.n_sisa += sjd1.n_gqty;
                      }

                      db.LG_SJD1s.DeleteOnSubmit(sjd1);
                    }

                    #endregion
                  }
                }

                #endregion
              }
              if ((field != null) && (!field.IsNew) && (field.IsModified) && (!field.IsDelete))
              {
                #region Modified

                #region RN

                listSjd2Copy = listSjd2.FindAll(delegate(LG_SJD2 sjd)
                {
                  return field.Item.Equals((string.IsNullOrEmpty(sjd.c_iteno) ? string.Empty : sjd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    field.Batch.Equals((string.IsNullOrEmpty(sjd.c_batch) ? string.Empty : sjd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    field.NomorSP.Equals((string.IsNullOrEmpty(sjd.c_spgno) ? string.Empty : sjd.c_spgno.Trim()), StringComparison.OrdinalIgnoreCase);
                });

                decimal  GAqum = 0, bE = 0, gE = 0, gI = 0,
                  nGAllocated = 0;
                bool GLop = true;

                sjd1 = listSjd1.Find(delegate(LG_SJD1 sjd)
                {
                  return field.Item.Equals((string.IsNullOrEmpty(sjd.c_iteno) ? string.Empty : sjd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    field.Batch.Equals((string.IsNullOrEmpty(sjd.c_batch) ? string.Empty : sjd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                  field.NomorSP.Equals((string.IsNullOrEmpty(sjd.c_spgno) ? string.Empty : sjd.c_spgno.Trim()), StringComparison.OrdinalIgnoreCase);
                });

                if (sjd1 == null)
                {
                  continue;
                }

                nQtyAllocGood = GAqum = (field.Quantity - (sjd1.n_gqty.HasValue ? sjd1.n_gqty.Value : 0));

                for (nLoopC = 0; nLoopC < listSjd2Copy.Count; nLoopC++)
                {
                  sjd2 = listSjd2Copy[nLoopC];

                  rnID = (string.IsNullOrEmpty(sjd2.c_rnno) ? string.Empty : sjd2.c_rnno.Trim());

                  if (GLop == false)
                  {
                    gE = 0;
                  }
                  if (GLop == true)
                  {
                    if (nQtyAllocGood < 0)
                    {
                      gI = GAqum + sjd2.n_gsisa.Value;
                      if (gI <= 0)
                      {
                        gE = GAqum - gI;
                      }
                      else
                      {
                        gE = GAqum;
                      }
                      GAqum += sjd2.n_gsisa.Value;
                      nGAllocated += gE;
                    }
                    else
                    {
                      gE = nQtyAllocGood;
                      nGAllocated += gE;
                    }
                  }
                  if (gE != 0)
                  {
                    sjd2.n_gqty += gE;
                    sjd2.n_gsisa += gE;

                    #region Detil

                    if (sjd2.c_rnno.Substring(0, 2) == "RN" || sjd2.c_rnno.Substring(0, 2) == "RR")
                    {
                      rnd1 = (from q in db.LG_RND1s
                              where q.c_gdg == gdgAsal &&
                              q.c_iteno == sjd2.c_iteno
                              && q.c_batch == sjd2.c_batch
                              && q.c_rnno == sjd2.c_rnno
                              select q).Take(1).SingleOrDefault();
                      if (rnd1 != null)
                      {
                        rnd1.n_gsisa -= gE;
                      }
                      else
                      {
                        result = "Surat Jalan untuk item " + sjd2.c_iteno + " dan batch " + sjd2.c_batch + " tersebut Stock - nya tidak terbaca system .";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                          db.Transaction.Rollback();
                        }

                        goto endLogic;
                      }
                    }
                    else
                    {
                      comboh = (from q in db.LG_ComboHs
                                where q.c_gdg == gdgAsal &&
                                q.c_iteno == sjd2.c_iteno
                                && q.c_batch == sjd2.c_batch
                                && q.c_combono == sjd2.c_rnno
                                select q).Take(1).SingleOrDefault();
                      if (comboh != null)
                      {
                        comboh.n_gsisa -= bE;
                      }
                      else
                      {
                        result = "Surat Jalan untuk item " + sjd2.c_iteno + " dan batch " + sjd2.c_batch + " tersebut Stock - nya tidak terbaca system .";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                          db.Transaction.Rollback();
                        }

                        goto endLogic;
                      }
                    }

                    #endregion

                    totalDetails++;
                  }

                  if (nGAllocated == field.sGQty)
                  {
                    GLop = false;
                  }

                  listSjd3.Add(new LG_SJD3()
                  {
                    c_gdg = gdgAsal,
                    c_sjno = sjID,
                    c_iteno = field.Item,
                    c_batch = field.Batch,
                    c_rnno = rnID,
                    c_spgno = string.Empty,
                    n_bqty = sjd2.n_bqty,
                    n_bsisa = sjd2.n_bsisa,
                    n_gqty = sjd2.n_gqty,
                    n_gsisa = sjd2.n_gsisa,
                    c_entry = nipEntry,
                    d_entry = date,
                    v_type = "03",
                    v_ket_del = string.Concat((isConfirmed ? "Confirm : " : string.Empty),
                          (string.IsNullOrEmpty(structure.Fields.Keterangan) ? "Human error" : structure.Fields.Keterangan)),
                    l_expired = sjd1.l_expired,
                    v_ket_ed = sjd1.l_expired == true ? sjd1.v_ket_ed : null,
                    c_acc_ed = sjd1.l_expired == true ? sjd1.c_acc_ed : null
                  });

                }

                #region SJD1

                #endregion

                if (sjd1 != null)
                {
                  spgd1 = (from q in db.LG_SPGHs
                           join q1 in db.LG_SPGD1s on new { c_gdg = q.c_gdg1, q.c_spgno } equals new { q1.c_gdg, q1.c_spgno }
                           where (q.c_spgno == field.NomorSP)
                            && (q.c_gdg1 == gdgTujuan) && (q1.c_iteno == field.Item)
                            && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                           select q1).Take(1).SingleOrDefault();
                  if (spgd1 != null)
                  {
                    spgd1.n_sisa -= (field.Quantity - (sjd1.n_gqty.HasValue ? sjd1.n_gqty.Value : 0));
                  }

                  sjd1.n_gqty += (field.Quantity - (sjd1.n_gqty.HasValue ? sjd1.n_gqty.Value : 0));
                  sjd1.c_type_dc = field.tipe_dc;
                }

                #endregion

                #endregion
              }
            }

            if (listSjd3.Count > 0)
            {
              db.LG_SJD3s.InsertAllOnSubmit(listSjd3.ToArray());
              listSjd3.Clear();
            }

            listSjd1.Clear();
            listSjd2.Clear();
            listSjd3.Clear();

            listRND1.Clear();
            listSPGD1.Clear();
          }

          #endregion


          listSjd1 = (from q in db.LG_SJD1s
                      where q.c_sjno == structure.Fields.sjID
                      select q).Distinct().ToList();

          listSJSum =
          listSjd1.GroupBy(x => new { x.c_sjno, x.c_iteno })
          .Select(x => new LG_SJD1_SUM_BYBATCH() { c_sjno = x.Key.c_sjno, c_iteno = x.Key.c_iteno, n_qty = x.Sum(y => (y.n_gqty.HasValue ? y.n_gqty.Value : 0)) }).ToList();

          if (listSJSum.Count > 0)
          {
              for (nLoop = 0; nLoop < listSJSum.Count; nLoop++)
              {
                  sjd1Sum = listSJSum[nLoop];

                  if (sjd1Sum != null)
                  {
                      //calc koli
                      masitm = (from q in db.FA_MasItms
                                where q.c_iteno == sjd1Sum.c_iteno
                                select q).Take(1).SingleOrDefault();

                      nBox = masitm.n_box ?? 0;

                      koliKarton += Math.Floor(sjd1Sum.n_qty / nBox);
                      receh += sjd1Sum.n_qty % nBox;
                  }
              }

              sjh.n_karton = koliKarton;
              sjh.n_receh = receh;

              listSJSum.Clear();
              listSjd1.Clear();
          }

          hasAnyChanges = true;

          #endregion
        }
        else if (structure.Method.Equals("Auto", StringComparison.OrdinalIgnoreCase))
        {
          #region Add Auto

          if (!string.IsNullOrEmpty(sjID))
          {
            result = "Nomor Surat Jalan harus kosong.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (string.IsNullOrEmpty(structure.Fields.NoRN))
                {
                    result = "Nomer RN dibutuhkan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (string.IsNullOrEmpty(structure.Fields.From))
                {
                    result = "Gudang dibutuhkan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (Commons.IsClosingLogistik(db, date))
                {
                    result = "Surat Jalan tidak dapat disimpan, karena sudah closing.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                sjID = Commons.GenerateNumbering<LG_SJH>(db, "SJ", '3', "06", date, "c_sjno");

                //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "SJ");
                pinNumber = Functionals.GeneratedRandomPinId(8, string.Empty);
                    //Indra 20180920FM
                    //SerahTerimaTransportasi
                    RNH = (from q in db.LG_RNHs
                           where q.c_rnno == structure.Fields.NoRN
                           select q).Take(1).SingleOrDefault();

                    if (RNH == null)
                    {
                        result = "Supllier atas RN ." + RNH.c_rnno + " tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    sjh = new LG_SJH()
                    {
                        c_sjno = sjID,
                        c_entry = nipEntry,
                        c_gdg = gdgAsal,
                        //Indra 20180920FM
                        //c_nosup = structure.Fields.Supplier,
                        c_nosup = RNH.c_from,
                        c_type = "01",
                        c_update = nipEntry,
                        d_entry = date,
                        d_update = date,
                        l_confirm = false,
                        l_print = false,
                        v_ket = structure.Fields.Keterangan,
                        c_gdg2 = gdgTujuan,
                        c_pin = pinNumber,
                        d_sjdate = date.Date,
                        l_exp = false,
                        l_status = false,
                        c_type_cat = structure.Fields.TypeKategori,
                        c_type_lat = structure.Fields.TypeLantai,
                        c_type_sj = structure.Fields.TypeSJ,
                        l_auto = true
                    };

                db.LG_SJHs.InsertOnSubmit(sjh);

                #region Insert Detail

                if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                {
                    listSjd1 = new List<LG_SJD1>();
                    listSjd2 = new List<LG_SJD2>();
                    //listComboh = new List<LG_ComboH>();
                    listRN = new List<string>();
                    comboh = new LG_ComboH();
                    rnd1 = new LG_RND1();
                    dicItemStock = new Dictionary<string, List<SJClassComponent>>(StringComparer.OrdinalIgnoreCase);
                    spgh = new LG_SPGH();
                    listSPGD1Auto = new List<LG_SPGD1>();

                    spghAdj = new LG_SPGH();
                    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    {
                      field = structure.Fields.Field[nLoop];

                      if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0))
                      {

                        spgd1 = (from q in db.LG_SPGHs
                                 join q1 in db.LG_SPGD1s on new { c_gdg = q.c_gdg1, q.c_spgno } equals new { q1.c_gdg, q1.c_spgno }
                                 where (q.c_spgno == field.NomorSP)
                                  && (q.c_gdg1 == gdgTujuan) && (q1.c_iteno == field.Item)
                                  && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                 select q1).Take(1).SingleOrDefault();

                        if (spgd1 != null)
                        {
                          nQtyAllocGood = 0;
                          spAlloc = (spgd1.n_sisa.HasValue ? spgd1.n_sisa.Value : 0);
                          spQtyReloc = (spAlloc > field.Quantity ? field.Quantity : spAlloc);

                          if (spAlloc <= 0)
                          {
                            continue;
                          }

                  if (dicItemStock.ContainsKey(field.Item))
                  {
                    listSPAC = dicItemStock[field.Item];
                  }
                  else
                  {
                    listSPAC = (from q in db.LG_RND1s 
                                join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                from qBat in q_2.DefaultIfEmpty()
                                where (q.n_gsisa != 0) && q.c_rnno == structure.Fields.NoRN && q.c_iteno == field.Item &&
                                q.c_gdg == gdgAsal
                                orderby qBat.d_expired ascending
                                select new SJClassComponent()
                                {
                                  RefID = q.c_rnno,
                                  SignID = "RN",
                                  Item = q.c_iteno,
                                  Qty = q.n_gsisa.HasValue ? q.n_gsisa.Value : 0,
                                  Supplier = q1.c_nosup,
                                  Box = (q1.n_box.HasValue ? q1.n_box.Value : 0),
                                  BatchID = q.c_batch
                                }).Distinct().ToList();

                            dicItemStock.Add(field.Item, listSPAC);

                            totalCurrentStockGood = listSPAC.Sum(t => t.Qty);
                          }
                          if (field.Quantity > 0)
                          {
                            if (totalCurrentStockGood > 0)
                            {

                            }
                            if (totalCurrentStockGood < (field.Quantity))
                            {
                              totalCurrentStockGood = (totalCurrentStockGood < field.Quantity ? totalCurrentStockGood : field.Quantity);
                            }
                          }

                          if (listSPAC.Count > 0)
                          {
                            nQtyAllocGood = (field.Quantity < totalCurrentStockGood ? field.Quantity : totalCurrentStockGood);
                            decimal GAqum = 0, gE = 0, gI = 0;
                            bool GLop = true;

                            istSPAC = listSPAC.FindAll(delegate(SJClassComponent sjd)
                            {
                              if (!sjd.BatchID.Trim().Equals(field.Batch.Trim(), StringComparison.OrdinalIgnoreCase))
                              {
                                return false;
                              }

                              return true;
                            });

                            for (nLoopC = 0; nLoopC < istSPAC.Count; nLoopC++)
                            {
                              if (istSPAC[nLoopC].Qty > 0)
                              {
                                if (istSPAC[nLoopC].Qty > 0)
                                {
                                  gI = nQtyAllocGood - istSPAC[nLoopC].Qty;

                                  if (GLop == true)
                                  {
                                    gE = nQtyAllocGood - gI;
                                    GAqum = GAqum + gE;
                                  }
                                  if (GAqum > nQtyAllocGood)
                                  {
                                    gE = nQtyAllocGood - (GAqum - gE);
                                    GAqum = GAqum + gE;
                                  }
                                  if (GLop == false)
                                  {
                                    gE = 0;
                                  }

                                  if (gE != 0)
                                  {
                                      listSjd2.Add(new LG_SJD2()
                                      {
                                          c_batch = field.Batch,
                                          c_gdg = gdgAsal,
                                          c_iteno = field.Item,
                                          c_sjno = sjID,
                                          c_spgno = field.NomorSP,
                                          c_rnno = structure.Fields.NoRN,
                                          n_bqty = 0,
                                          n_gqty = gE,
                                          n_bsisa = 0,
                                          n_gsisa = gE
                                      });

                                      totalCurrentStockGood -= gE;

                                      if (istSPAC[nLoopC].SignID.Equals("RN") || istSPAC[nLoopC].SignID.Equals("RR"))
                                      {
                                          rnd1 = (from q in db.LG_RND1s
                                                  where q.c_gdg == gdgAsal &&
                                                  q.c_iteno == istSPAC[nLoopC].Item
                                                  && q.c_batch == field.Batch
                                                  && q.c_rnno == structure.Fields.NoRN
                                                  select q).Take(1).SingleOrDefault();
                                          if (rnd1 != null)
                                          {
                                              rnd1.n_gsisa -= gE;
                                              spgd1.n_sisa -= gE;
                                          }
                                      }
                                      else
                                      {
                                          comboh = (from q in db.LG_ComboHs
                                                    where q.c_gdg == gdgAsal &&
                                                    q.c_iteno == istSPAC[nLoopC].Item
                                                    && q.c_batch == field.Batch
                                                    && q.c_combono == istSPAC[nLoopC].RefID
                                                    select q).Take(1).SingleOrDefault();

                                          if (comboh != null)
                                          {
                                              comboh.n_gsisa -= gE;
                                              spgd1.n_sisa -= gE;
                                          }
                                      }

                                      totalDetails++;
                                  }


                                  if (GAqum > nQtyAllocGood)
                                  {
                                    GLop = false;
                                  }
                                }
                              }
                            }

                            if (listSjd2.Count > 0)
                            {
                              listSjd1.Add(new LG_SJD1()
                              {
                                c_batch = field.Batch,
                                c_gdg = gdgAsal,
                                c_iteno = field.Item,
                                c_sjno = sjID,
                                c_spgno = field.NomorSP,
                                n_bqty = field.BadQuantity,
                                n_booked_bad = field.BadQuantity,
                                n_booked = nQtyAllocGood,
                                n_gqty = nQtyAllocGood
                              });

                              if (field.nQtyAdj > 0)
                              {
                                if (spgIdAdjust == null)
                                {

                                  spghAdj = SuratJalanSPGAdj(db, field, structure, date, sjID);
                                  spgIdAdjust = spghAdj.c_spgno;
                                }

                                listSPGD1Auto.Add(new LG_SPGD1()
                                {
                                  c_gdg = gdgTujuan,
                                  c_iteno = field.Item,
                                  c_spgno = spgIdAdjust,
                                  n_qty = field.nQtyAdj,
                                  n_sisa = 0
                                });

                                rnd1 = (from q in db.LG_RND1s
                                        where q.c_gdg == gdgAsal &&
                                        q.c_iteno == field.Item
                                        && q.c_batch == field.Batch
                                        && q.c_rnno == structure.Fields.NoRN
                                        select q).Take(1).SingleOrDefault();

                                if (rnd1 != null && spgIdAdjust != null)
                                {
                                  rnd1.n_gsisa -= field.nQtyAdj;

                                  listSjd2.Add(new LG_SJD2()
                                  {
                                    c_batch = field.Batch,
                                    c_gdg = gdgAsal,
                                    c_iteno = field.Item,
                                    c_sjno = sjID,
                                    c_spgno = spgIdAdjust,
                                    c_rnno = structure.Fields.NoRN,
                                    n_bqty = 0,
                                    n_gqty = field.nQtyAdj,
                                    n_bsisa = 0,
                                    n_gsisa = field.nQtyAdj
                                  });

                                  listSjd1.Add(new LG_SJD1()
                                  {
                                    c_batch = field.Batch,
                                    c_gdg = gdgAsal,
                                    c_iteno = field.Item,
                                    c_sjno = sjID,
                                    c_spgno = spgIdAdjust,
                                    n_bqty = field.BadQuantity,
                                    n_booked_bad = 0,
                                    n_booked = field.nQtyAdj,
                                    n_gqty = field.nQtyAdj
                                  });
                                }
                                
                              }
                            }
                          }
                        }
                      }
                    }
                }

                #endregion

                dic = new Dictionary<string, string>();

                if (totalDetails > 0)
                {
                    if ((listSjd1 != null) && (listSjd1.Count > 0))
                    {
                        db.LG_SJD1s.InsertAllOnSubmit(listSjd1.ToArray());
                        listSjd1.Clear();

                        if ((listSjd2 != null) && (listSjd2.Count > 0))
                        {
                            db.LG_SJD2s.InsertAllOnSubmit(listSjd2.ToArray());
                            listSjd2.Clear();

                            if (spghAdj != null && (listSPGD1Auto.Count > 0))
                            {
                                db.LG_SPGHs.InsertOnSubmit(spghAdj);

                  db.LG_SPGD1s.InsertAllOnSubmit(listSPGD1Auto.ToArray());
                }
              }
            }

            

            dic.Add("SJ", sjID);
            dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                    result = string.Format("Total {0} detail(s)", totalDetails);

                    hasAnyChanges = true;
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

          rpe = ResponseParser.ResponseParserEnum.IsFailed;
        }

      }
      catch (Exception ex)
      {
        if (db.Transaction != null)
        {
          db.Transaction.Rollback();
        }

        result = string.Format("ScmsSoaLibrary.Bussiness.Transfer:TransferGudang - {0}", ex.Message);

        Logger.WriteLine(result, true);
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

    public string TransferGudangRepack(ScmsSoaLibrary.Parser.Class.TranStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false;
      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      ScmsSoaLibrary.Parser.Class.TranStructureField field = null;
      string nipEntry = null;
      string sjID = null,
        rnID = null,
        itemID = null,
        batchCode = null,
        suplID = null,
        pinNumber = null,
        refID = null,
        sjDO = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      decimal spQtyReloc = 0,
        spAllocGood = 0,
        spAllocBad = 0,
        spAlloc = 0;

      decimal nQtyAllocGood = 0,
        nQtyAllocBad = 0;
      decimal nQtyRelocGood = 0,
         nQtyRelocBad = 0;

      List<LG_SJD1> listSjd1 = null;
      List<LG_SJD2> listSjd2 = null;
      List<LG_SJD2> listSjd2Copy = null;
      List<LG_SJD3> listSjd3 = null;

      List<LG_RND1> listRND1 = null;
      List<LG_RND1> listResRND1 = null;
      List<LG_RNH> listRNH = null;

      List<LG_ComboH> listComboh = null;
      List<LG_ComboH> listResComboh = null;
      List<SJClassComponent> SJStock = null,
        SJStockRn = null;
      SJClassComponent SJStockBatch = null;

      LG_ComboH comboh = null;

      MK_MPD mpd = null;


      char gdgAsal = (string.IsNullOrEmpty(structure.Fields.From) || (structure.Fields.From.Length < 1) ? '1' : structure.Fields.From[0]);
      char gdgTujuan = (string.IsNullOrEmpty(structure.Fields.To) || (structure.Fields.To.Length < 1) ? '1' : structure.Fields.To[0]);

      LG_SJH sjh = null;
      LG_SJD1 sjd1 = null;
      LG_SJD2 sjd2 = null;
      LG_RNH rnh = null;
      LG_RND1 rnd1 = null;

      int nLoop = 0,
        nLoopC = 0,
        nLoopD = 0;

      IDictionary<string, string> dic = null;

      nipEntry = (structure.Fields.Entry ?? string.Empty);
      suplID = (structure.Fields.Supplier ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }
      int totalDetails = 0;
      bool isConfirmed = false,
        isCombo = false,
        isDisposal = false,
        sjhConfirm = false;

      sjID = (structure.Fields.sjID ?? string.Empty);

      if (gdgAsal.Equals(char.MinValue))
      {
        result = "Gudang asal tidak dapat dibaca.";

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

          if (!string.IsNullOrEmpty(sjID))
          {
            result = "Nomor Surat Jalan Retur harus kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          //if (string.IsNullOrEmpty(structure.Fields.Supplier))
          //{
          //  result = "Nama pemasok dibutuhkan.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}

          if (string.IsNullOrEmpty(structure.Fields.From))
          {
            result = "Gudang dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          if (Commons.IsClosingLogistik(db, date))
          {
            result = "Surat Jalan tidak dapat disimpan, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "SJ");

          sjID = Commons.GenerateNumbering<LG_SJH>(db, "SJ", '3', "06", date, "c_sjno");

          pinNumber = Functionals.GeneratedRandomPinId(8, string.Empty);

          sjh = new LG_SJH()
          {
            c_gdg = gdgAsal,
            c_sjno = sjID,
            c_entry = nipEntry,
            c_nosup = string.IsNullOrEmpty(structure.Fields.Supplier) ? null : structure.Fields.Supplier,
            c_type = "02",
            c_update = nipEntry,
            d_entry = date,
            d_update = date,
            l_confirm = false,
            l_print = false,
            v_ket = structure.Fields.Keterangan,
            c_gdg2 = gdgTujuan,
            c_pin = pinNumber,
            d_sjdate = date.Date,
            l_exp = false,
            l_status = false,
            c_type_cat = string.Empty,
            l_auto = structure.Fields.StatusSJ,
            c_product_origin = string.IsNullOrEmpty(structure.Fields.asalProduk) ? null : structure.Fields.asalProduk,
            v_nodok = string.IsNullOrEmpty(structure.Fields.noDok) ? null : structure.Fields.noDok,
            v_product_origin = string.IsNullOrEmpty(structure.Fields.cabangExp) ? null : structure.Fields.cabangExp,
            c_mpno = string.IsNullOrEmpty(structure.Fields.memo) ? null : structure.Fields.memo
          };

          db.LG_SJHs.InsertOnSubmit(sjh);

          #region Old Code

          //db.SubmitChanges();

          //sjh = (from q in db.LG_SJHs
          //       where q.v_ket == tmpNumbering
          //       select q).Take(1).SingleOrDefault();

          //if (!string.IsNullOrEmpty(sjID))
          //{
          //  result = "No Surat Jalan Retur tidak dapat di raih";

          //  rpe = ResponseParser.ResponseParserEnum.IsError;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}

          //if (sjh.c_sjno.Equals("XXXXXXXXXX"))
          //{
          //  result = "Trigger Surat Jalan Retur tidak aktif.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}

          //sjh.v_ket = structure.Fields.Keterangan;

          //sjID = sjh.c_sjno;

          #endregion

          #region Insert Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            //listRN = new List<string>();

            listSjd1 = new List<LG_SJD1>();
            listSjd2 = new List<LG_SJD2>();

            listRND1 = new List<LG_RND1>();
            listComboh = new List<LG_ComboH>();
            comboh = new LG_ComboH();
            rnd1 = new LG_RND1();
            SJStock = new List<SJClassComponent>();
            SJStockBatch =new SJClassComponent();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              //SJStock = (from q in GlobalQuery.ViewStockLite(db, gdgAsal, field.Item)
              //           where q.c_batch == field.Batch && q.n_gsisa != 0
              //           group q by new { q.c_gdg, q.c_iteno } into gSum
              //           where (gSum.Sum(x => x.n_bsisa) >= field.BadQuantity)
              //           && (gSum.Sum(x => x.n_gsisa) >= field.Quantity)
              //           select new SJClassComponent()
              //           {
              //             Item = gSum.Key.c_iteno,
              //             Good = gSum.Sum(x => x.n_gsisa),
              //             Bad = gSum.Sum(x => x.n_bsisa)
              //           }).ToList();

              SJStock = (from q in GlobalQuery.ViewStockLite(db, gdgAsal, field.Item)
                           where (q.n_gsisa != 0 || q.n_bsisa != 0)
                           select new SJClassComponent()
                           {
                             Item = q.c_iteno,
                             RefID = q.c_no,
                             BatchID = q.c_batch,
                             Qty = q.n_gsisa,
                             QtyBad = q.n_bsisa,
                             SignID = q.c_table
                           }).ToList();

              listResRND1 = (from q in db.LG_RND1s
                             where q.c_gdg == gdgAsal && q.c_iteno == field.Item 
                             select q).ToList();

              listResComboh = (from q in db.LG_ComboHs
                               where q.c_gdg == gdgAsal && q.c_iteno == field.Item
                               select q).ToList();

              if ((field != null) && field.IsNew && (!field.IsDelete) && ((field.Quantity > 0) || (field.BadQuantity > 0)))
              {
                #region New

                nLoopC = SJStock.GroupBy(y => new { y.Item })
                  .Where(x => (x.Sum(z => z.QtyBad) >= field.BadQuantity)
                  || (x.Sum(z => z.Qty) >= field.Quantity)).Count();

                nQtyAllocGood = field.Quantity;
                nQtyAllocBad = field.BadQuantity;

                if (nLoopC <= 0)
                {
                  result = "Stock Tidak Mencukupi Untuk Item "+ field.Item + ".";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  if (db.Transaction != null)
                  {
                    db.Transaction.Rollback();
                  }

                  goto endLogic;
                }

                if (SJStock != null && SJStock.Count > 0)
                {
                  SJStockRn = SJStock.FindAll(delegate(SJClassComponent sjc)
                  {
                    return field.Item.Trim().Equals((string.IsNullOrEmpty(sjc.Item) ? string.Empty : sjc.Item.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    field.Batch.Trim().Equals((string.IsNullOrEmpty(sjc.BatchID) ? string.Empty : sjc.BatchID.Trim()), StringComparison.OrdinalIgnoreCase);
                  });

                  if (SJStockRn.Count > 0)
                  {
                      if (structure.Fields.NoRN.ToString() == "")
                      {
                          for (nLoopC = 0; nLoopC < SJStockRn.Count; nLoopC++)
                          {
                              SJStockBatch = SJStockRn[nLoopC];

                              if (SJStockBatch.SignID.Equals("RN") || SJStockBatch.SignID.Equals("RR"))
                              {
                                  rnd1 = listResRND1.Find(delegate(LG_RND1 rnd)
                                  {
                                      return field.Item.Trim().Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                        field.Batch.Trim().Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                        SJStockBatch.RefID.Trim().Equals((string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
                                  });

                                  nQtyRelocGood = (nQtyAllocGood > rnd1.n_gsisa ? rnd1.n_gsisa.Value : nQtyAllocGood);
                                  nQtyRelocBad = (nQtyAllocBad > rnd1.n_bsisa ? rnd1.n_bsisa.Value : nQtyAllocBad);

                                  if (rnd1 != null)
                                  {
                                      listSjd2.Add(new LG_SJD2()
                                      {
                                          c_batch = field.Batch,
                                          c_gdg = gdgAsal,
                                          c_iteno = field.Item,
                                          c_sjno = sjID,
                                          c_spgno = string.Empty,
                                          c_rnno = rnd1.c_rnno,
                                          n_bqty = nQtyRelocBad,
                                          n_bsisa = nQtyRelocBad,
                                          n_gqty = nQtyRelocGood,
                                          n_gsisa = nQtyRelocGood,
                                      });

                                      rnd1.n_gsisa -= nQtyRelocGood;
                                      rnd1.n_bsisa -= nQtyRelocBad;

                                      if (rnd1.n_gsisa < 0) //cek rn minus
                                      {
                                          result = "Qty RN Kurang (add Transfer Gudang) " + field.Item + " " + field.Batch;
                                          rpe = ResponseParser.ResponseParserEnum.IsFailed;
                                          if (db.Transaction != null)
                                          {
                                              db.Transaction.Rollback();
                                          }
                                          goto endLogic;
                                      }

                                      nQtyAllocGood -= nQtyRelocGood;
                                      nQtyAllocBad -= nQtyRelocBad;

                                      totalDetails++;
                                  }
                              }
                              else
                              {
                                  comboh = listResComboh.Find(delegate(LG_ComboH cmb)
                                  {
                                      return field.Item.Trim().Equals((string.IsNullOrEmpty(cmb.c_iteno) ? string.Empty : cmb.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                        field.Batch.Trim().Equals((string.IsNullOrEmpty(cmb.c_batch) ? string.Empty : cmb.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                        SJStockBatch.RefID.Trim().Equals((string.IsNullOrEmpty(cmb.c_combono) ? string.Empty : cmb.c_combono.Trim()), StringComparison.OrdinalIgnoreCase);
                                  });

                                  nQtyRelocGood = (nQtyAllocGood > rnd1.n_gsisa ? rnd1.n_gsisa.Value : nQtyAllocGood);
                                  nQtyRelocBad = (nQtyAllocBad > rnd1.n_bsisa ? rnd1.n_bsisa.Value : nQtyAllocBad);

                                  if (rnd1 != null)
                                  {
                                      listSjd2.Add(new LG_SJD2()
                                      {
                                          c_batch = field.Batch,
                                          c_gdg = gdgAsal,
                                          c_iteno = field.Item,
                                          c_sjno = sjID,
                                          c_spgno = string.Empty,
                                          c_rnno = comboh.c_combono,
                                          n_bqty = nQtyRelocBad,
                                          n_bsisa = nQtyRelocBad,
                                          n_gqty = nQtyRelocGood,
                                          n_gsisa = nQtyRelocGood,
                                      });

                                      comboh.n_gsisa -= nQtyRelocGood;
                                      comboh.n_bsisa -= nQtyRelocBad;

                                      nQtyAllocGood -= nQtyRelocGood;
                                      nQtyAllocBad -= nQtyRelocBad;

                                      totalDetails++;
                                  }
                              }
                          }
                      }
                      else
                      {
                          listSjd2.Add(new LG_SJD2()
                          {
                              c_batch = field.Batch,
                              c_gdg = gdgAsal,
                              c_iteno = field.Item,
                              c_sjno = sjID,
                              c_spgno = string.Empty,
                              c_rnno = structure.Fields.NoRN,
                              n_bqty = field.BadQuantity,
                              n_bsisa = field.BadQuantity,
                              n_gqty = field.Quantity,
                              n_gsisa = field.Quantity,
                          });
                          rnd1 = (from q in db.LG_RND1s where q.c_gdg == gdgAsal && q.c_rnno == structure.Fields.NoRN && q.c_iteno == field.Item && q.c_batch == field.Batch
                                      select q).Distinct().SingleOrDefault();
                          rnd1.n_gsisa -= field.Quantity;
                          totalDetails++;
                      }
                  }

                  listSjd1.Add(new LG_SJD1()
                  {
                    c_batch = field.Batch,
                    c_gdg = gdgAsal,
                    c_iteno = field.Item,
                    c_sjno = sjID,
                    c_spgno = string.Empty,
                    n_booked = field.Quantity,
                    n_booked_bad = field.BadQuantity,
                    n_bqty = field.BadQuantity,
                    n_gqty = field.Quantity
                  });

                  if(!string.IsNullOrEmpty(structure.Fields.memo))
                    {
                        mpd = (from q in db.MK_MPHs 
                             join q1 in db.MK_MPDs on new { c_gdg = q.c_gdg, q.c_mpno } equals new { q1.c_gdg, q1.c_mpno }
                             where (q.c_mpno == structure.Fields.memo)
                              && (q.c_gdg == gdgAsal) && (q1.c_iteno == field.Item)
                              && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                             select q1).Take(1).SingleOrDefault();

                        if(mpd != null)
                        {
                            mpd.n_sisa -= field.BadQuantity;
                        }
                        else
                        {
                            result = "Memo tidak ditemukan";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                              db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                    }
                }

                #region Old

                //if (lstItemCombo.Contains(field.Item))
                //{
                //  #region Combo

                //  #region Data Combo

                //  listResComboh = listComboh.FindAll(delegate(LG_ComboH cmb)
                //  {
                //    return gdgAsal.Equals(cmb.c_gdg) &&
                //      field.Item.Equals((string.IsNullOrEmpty(cmb.c_iteno) ? string.Empty : cmb.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                //      field.Batch.Equals((string.IsNullOrEmpty(cmb.c_batch) ? string.Empty : cmb.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                //      (((cmb.n_gsisa.HasValue ? cmb.n_gsisa.Value : 0) > 0) || ((cmb.n_bsisa.HasValue ? cmb.n_bsisa.Value : 0) > 0));
                //  });

                //  if ((listResComboh == null) || (listResComboh.Count < 1))
                //  {
                //    listResComboh = (from q in db.LG_ComboHs
                //                     where (q.c_gdg == gdgAsal) && (q.c_iteno == field.Item) && (q.c_batch == field.Batch)
                //                       && ((q.n_gsisa > 0) || (q.n_bsisa > 0))
                //                       && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                //                     select q).ToList();

                //    if (listResComboh.Count > 0)
                //    {
                //      listComboh.AddRange(listResComboh.ToArray());
                //    }
                //  }

                //  #endregion

                //  if ((listResComboh != null) && (listResComboh.Count > 0))
                //  {
                //    spQtyReloc = field.Quantity + field.BadQuantity;

                //    for (nLoopC = 0; nLoopC < listResComboh.Count; nLoopC++)
                //    {
                //      comboh = listResComboh[nLoopC];

                //      rnID = (string.IsNullOrEmpty(comboh.c_combono) ? string.Empty : comboh.c_combono.Trim());

                //      rnQtyGood = (comboh.n_gsisa.HasValue ? comboh.n_gsisa.Value : 0);
                //      rnQtyBad = (comboh.n_bsisa.HasValue ? comboh.n_bsisa.Value : 0);

                //      spAllocGood = (rnQtyGood > field.Quantity ? field.Quantity : rnQtyGood);
                //      spAllocBad = (rnQtyBad > field.BadQuantity ? field.BadQuantity : rnQtyBad);

                //      field.BadQuantity -= spAllocBad;
                //      field.Quantity -= spAllocGood;

                //      comboh.n_bsisa -= spAllocBad;
                //      comboh.n_gsisa -= spAllocGood;

                //      rnQtyBad -= spAllocBad;
                //      rnQtyGood -= spAllocGood;

                //      spQtyReloc -= (spAllocBad + spAllocGood);

                //      #region SJD1

                //      sjd1 = listSjd1.Find(delegate(LG_SJD1 sjd)
                //      {
                //        return field.Item.Equals((string.IsNullOrEmpty(sjd.c_iteno) ? string.Empty : sjd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                //          field.Batch.Equals((string.IsNullOrEmpty(sjd.c_batch) ? string.Empty : sjd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                //      });

                //      if (sjd1 == null)
                //      {
                //        sjd1 = new LG_SJD1()
                //        {
                //          c_gdg = gdgAsal,
                //          c_sjno = sjID,
                //          c_iteno = field.Item,
                //          c_batch = field.Batch,
                //          c_spgno = string.Empty,
                //          n_bqty = spAllocBad,
                //          n_gqty = spAllocGood,
                //          n_booked = spAllocGood,
                //          n_booked_bad = spAllocBad
                //        };

                //        //db.LG_SJD1s.InsertOnSubmit(sjd1);

                //        listSjd1.Add(sjd1);
                //      }
                //      else
                //      {
                //        sjd1.n_bqty += spAllocBad;
                //        sjd1.n_gqty += spAllocGood;
                //        sjd1.n_booked_bad += spAllocBad;
                //        sjd1.n_booked += spAllocGood;
                //      }

                //      #endregion

                //      #region SJD2

                //      sjd2 = listSjd2.Find(delegate(LG_SJD2 sjd)
                //      {
                //        return field.Item.Equals((string.IsNullOrEmpty(sjd.c_iteno) ? string.Empty : sjd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                //          field.Batch.Equals((string.IsNullOrEmpty(sjd.c_batch) ? string.Empty : sjd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                //          rnID.Equals((string.IsNullOrEmpty(sjd.c_rnno) ? string.Empty : sjd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
                //      });

                //      if (sjd2 == null)
                //      {
                //        listSjd2.Add(new LG_SJD2()
                //        {
                //          c_gdg = gdgAsal,
                //          c_sjno = sjID,
                //          c_iteno = field.Item,
                //          c_batch = field.Batch,
                //          c_rnno = rnID,
                //          c_spgno = string.Empty,
                //          n_bsisa = spAllocBad,
                //          n_bqty = spAllocBad,
                //          n_gsisa = spAllocGood,
                //          n_gqty = spAllocGood
                //        });
                //      }
                //      else
                //      {
                //        sjd2.n_bsisa += spAllocBad;
                //        sjd2.n_bqty += spAllocBad;
                //        sjd2.n_gsisa += spAllocGood;
                //        sjd2.n_gqty += spAllocBad;
                //      }

                //      #endregion

                //      //rnQtyGood = (comboh.n_gsisa.HasValue ? comboh.n_gsisa.Value : 0);
                //      //rnQtyBad = (comboh.n_bsisa.HasValue ? comboh.n_bsisa.Value : 0);

                //      if ((rnQtyBad == 0) && (rnQtyGood == 0))
                //      {
                //        listComboh.Remove(comboh);
                //      }

                //      totalDetails++;

                //      if (spQtyReloc <= 0)
                //      {
                //        break;
                //      }
                //    }

                //    listResComboh.Clear();
                //  }

                //  #endregion
                //}
                //else
                //{
                //  #region RN

                //  #region RN

                //  listResRND1 = listRND1.FindAll(delegate(LG_RND1 rnd)
                //  {
                //    return gdgAsal.Equals(rnd.c_gdg) &&
                //      field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                //      field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                //      (((rnd.n_gsisa.HasValue ? rnd.n_gsisa.Value : 0) > 0) || ((rnd.n_bsisa.HasValue ? rnd.n_bsisa.Value : 0) > 0));
                //  });

                //  if ((listResRND1 == null) || (listResRND1.Count < 1))
                //  {
                //    listResRND1 = (from q in db.LG_RNHs
                //                   join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                //                   where (q.c_gdg == gdgAsal)
                //                     && (q1.c_iteno == field.Item) && (q1.c_batch == field.Batch)
                //                     && ((q1.n_gsisa > 0) || (q1.n_bsisa > 0))
                //                     && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                //                   select q1).ToList();

                //    if (listResRND1.Count > 0)
                //    {
                //      listRND1.AddRange(listResRND1.ToArray());
                //    }
                //  }

                //  #endregion

                //  if ((listResRND1 != null) && (listResRND1.Count > 0))
                //  {
                //    spQtyReloc = field.Quantity + field.BadQuantity;

                //    for (nLoopC = 0; nLoopC < listResRND1.Count; nLoopC++)
                //    {
                //      rnd1 = listResRND1[nLoopC];

                //      rnID = (string.IsNullOrEmpty(rnd1.c_rnno) ? string.Empty : rnd1.c_rnno.Trim());

                //      rnQtyGood = (rnd1.n_gsisa.HasValue ? rnd1.n_gsisa.Value : 0);
                //      rnQtyBad = (rnd1.n_bsisa.HasValue ? rnd1.n_bsisa.Value : 0);

                //      spAllocGood = (rnQtyGood > field.Quantity ? field.Quantity : rnQtyGood);
                //      spAllocBad = (rnQtyBad > field.BadQuantity ? field.BadQuantity : rnQtyBad);

                //      field.BadQuantity -= spAllocBad;
                //      field.Quantity -= spAllocGood;

                //      rnd1.n_bsisa -= spAllocBad;
                //      rnd1.n_gsisa -= spAllocGood;

                //      rnQtyBad -= spAllocBad;
                //      rnQtyGood -= spAllocGood;

                //      spQtyReloc -= (spAllocBad + spAllocGood);

                //      #region SJD1

                //      sjd1 = listSjd1.Find(delegate(LG_SJD1 sjd)
                //      {
                //        return field.Item.Equals((string.IsNullOrEmpty(sjd.c_iteno) ? string.Empty : sjd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                //          field.Batch.Equals((string.IsNullOrEmpty(sjd.c_batch) ? string.Empty : sjd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                //      });

                //      if (sjd1 == null)
                //      {
                //        sjd1 = new LG_SJD1()
                //        {
                //          c_gdg = gdgAsal,
                //          c_sjno = sjID,
                //          c_iteno = field.Item,
                //          c_batch = field.Batch,
                //          c_spgno = string.Empty,
                //          n_bqty = spAllocBad,
                //          n_gqty = spAllocGood,
                //          n_booked = spAllocGood,
                //          n_booked_bad = spAllocBad
                //        };

                //        //db.LG_SJD1s.InsertOnSubmit(sjd1);

                //        listSjd1.Add(sjd1);
                //      }
                //      else
                //      {
                //        sjd1.n_bqty += spAllocBad;
                //        sjd1.n_gqty += spAllocGood;
                //        sjd1.n_booked_bad += spAllocBad;
                //        sjd1.n_booked += spAllocGood;
                //      }

                //      #endregion

                //      #region SJD2

                //      sjd2 = listSjd2.Find(delegate(LG_SJD2 sjd)
                //      {
                //        return field.Item.Equals((string.IsNullOrEmpty(sjd.c_iteno) ? string.Empty : sjd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                //          field.Batch.Equals((string.IsNullOrEmpty(sjd.c_batch) ? string.Empty : sjd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                //          rnID.Equals((string.IsNullOrEmpty(sjd.c_rnno) ? string.Empty : sjd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
                //      });

                //      if (sjd2 == null)
                //      {
                //        listSjd2.Add(new LG_SJD2()
                //        {
                //          c_gdg = gdgAsal,
                //          c_sjno = sjID,
                //          c_iteno = field.Item,
                //          c_batch = field.Batch,
                //          c_rnno = rnID,
                //          c_spgno = string.Empty,
                //          n_bsisa = spAllocBad,
                //          n_bqty = spAllocBad,
                //          n_gsisa = spAllocGood,
                //          n_gqty = spAllocGood
                //        });
                //      }
                //      else
                //      {
                //        sjd2.n_bsisa += spAllocBad;
                //        sjd2.n_bqty += spAllocBad;
                //        sjd2.n_gsisa += spAllocGood;
                //        sjd2.n_gqty += spAllocBad;
                //      }

                //      #endregion

                //      //rnQtyGood = (rnd1.n_gsisa.HasValue ? rnd1.n_gsisa.Value : 0);
                //      //rnQtyBad = (rnd1.n_bsisa.HasValue ? rnd1.n_bsisa.Value : 0);

                //      if ((rnQtyBad == 0) && (rnQtyGood == 0))
                //      {
                //        listRND1.Remove(rnd1);
                //      }

                //      totalDetails++;

                //      if (spQtyReloc <= 0)
                //      {
                //        break;
                //      }
                //    }

                //    listResRND1.Clear();
                //  }

                //  #endregion
                //}

                #endregion

                #region Old

                //comboh = new LG_ComboH();

                //if (field.Quantity > 0 && field.BadQuantity == 0)
                //{
                //  #region Good > 0

                //  var Noref = (from q in GlobalQuery.ViewStockLite(db, gdgAsal, field.Item)
                //               where q.c_batch == field.Batch && q.n_gsisa > 0
                //                select q).ToList();


                //  if (Noref.Count > 0)
                //  {
                //    nQtyAllocGood = field.Quantity;
                //    decimal GAqum = 0, gE = 0, gI = 0;
                //    bool GLop = true;

                //    for (nLoopC = 0; nLoopC < Noref.Count; nLoopC++)
                //    {
                //      if (Noref[nLoopC].n_gsisa > 0)
                //      {
                //        gI = nQtyAllocGood - Noref[nLoopC].n_gsisa;

                //        if (GLop == true)
                //        {
                //          gE = nQtyAllocGood - gI;
                //          GAqum = GAqum + gE;
                //        }
                //        if (GAqum > nQtyAllocGood)
                //        {
                //          gE = nQtyAllocGood - (GAqum - gE);
                //          GAqum = GAqum + gE;
                //        }
                //        if (GLop == false)
                //        {
                //          gE = 0;
                //        }

                //        if (gE != 0)
                //        {
                //          listSjd2.Add(new LG_SJD2()
                //          {
                //            c_batch = field.Batch,
                //            c_gdg = gdgAsal,
                //            c_iteno = field.Item,
                //            c_sjno = sjID,
                //            c_spgno = string.Empty,
                //            c_rnno = Noref[nLoopC].c_no,
                //            n_bqty = 0,
                //            n_gqty = gE,
                //            n_bsisa = 0,
                //            n_gsisa = gE
                //          });

                //          if (Noref[nLoopC].c_table.Equals("RN"))
                //          {
                //            rnd1 = (from q in db.LG_RND1s
                //                    where q.c_gdg == gdgAsal &&
                //                    q.c_iteno == Noref[nLoopC].c_iteno
                //                    && q.c_batch == Noref[nLoopC].c_batch
                //                    && q.c_rnno == Noref[nLoopC].c_no
                //                    select q).Take(1).SingleOrDefault();

                //            rnd1.n_gsisa -= gE;
                //          }
                //          else
                //          {
                //            comboh = (from q in db.LG_ComboHs
                //                      where q.c_gdg == gdgAsal &&
                //                      q.c_iteno == Noref[nLoopC].c_iteno
                //                      && q.c_batch == Noref[nLoopC].c_batch
                //                      && q.c_combono == Noref[nLoopC].c_no
                //                      select q).Take(1).SingleOrDefault();

                //            comboh.n_gsisa -= gE;
                //          }

                //          totalDetails++;
                //        }

                //        if (GAqum > nQtyAllocGood)
                //        {
                //          GLop = false;
                //        }
                //      }
                //    }
                //  }

                //  #endregion
                //}
                //if (field.Quantity == 0 && field.BadQuantity > 0)
                //{
                //  #region Bad > 0

                //  //listResRND1 = (from q in db.LG_RND1s
                //  //               join q1 in db.LG_RNHs on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                //  //               join q2 in db.LG_MsBatches on q.c_batch equals q2.c_batch
                //  //               where q.c_gdg == gdgAsal && q.c_batch == field.Batch
                //  //               && q.c_iteno == field.Item && q.n_bsisa > 0
                //  //               orderby q2.d_expired descending, q1.d_rndate
                //  //               select q).Distinct().ToList();

                //  var Noref = (from q in GlobalQuery.ViewStockLite(db, gdgAsal, field.Item)
                //               where q.c_batch == field.Batch && q.n_bsisa > 0
                //               select q).ToList();

                //  if (Noref.Count > 0)
                //  {
                //    nQtyAllocGood = field.Quantity;
                //    nQtyAllocBad = field.BadQuantity;
                //    decimal BAqum = 0, bE = 0, bI = 0;
                //    bool BLop = true;

                //    for (nLoopC = 0; nLoopC < Noref.Count; nLoopC++)
                //    {
                //      if (Noref[nLoopC].n_bsisa > 0)
                //      {
                //        bI = nQtyAllocBad - Noref[nLoopC].n_bsisa;

                //        if (BLop == true)
                //        {
                //          bE = nQtyAllocBad - bI;
                //          BAqum = BAqum + bE;
                //        }
                //        if (BAqum > nQtyAllocBad)
                //        {
                //          bE = nQtyAllocBad - (BAqum - bE);
                //          BAqum = BAqum + bE;
                //        }
                //        if (BLop == false)
                //        {
                //          bE = 0;
                //        }
                //        if (bE != 0)
                //        {

                //          listSjd2.Add(new LG_SJD2()
                //          {
                //            c_batch = field.Batch,
                //            c_gdg = gdgAsal,
                //            c_iteno = field.Item,
                //            c_sjno = sjID,
                //            c_spgno = string.Empty,
                //            c_rnno = Noref[nLoopC].c_no,
                //            n_bqty = bE,
                //            n_gqty = 0,
                //            n_bsisa = bE,
                //            n_gsisa = 0
                //          });

                //          if (Noref[nLoopC].c_table.Equals("RN"))
                //          {
                //            rnd1 = (from q in db.LG_RND1s
                //                    where q.c_gdg == gdgAsal &&
                //                    q.c_iteno == Noref[nLoopC].c_iteno
                //                    && q.c_batch == Noref[nLoopC].c_batch
                //                    && q.c_rnno == Noref[nLoopC].c_no
                //                    select q).Take(1).SingleOrDefault();

                //            rnd1.n_bsisa -= bE;
                //          }
                //          else
                //          {
                //            comboh = (from q in db.LG_ComboHs
                //                      where q.c_gdg == gdgAsal &&
                //                      q.c_iteno == Noref[nLoopC].c_iteno
                //                      && q.c_batch == Noref[nLoopC].c_batch
                //                      && q.c_combono == Noref[nLoopC].c_no
                //                      select q).Take(1).SingleOrDefault();

                //            comboh.n_bsisa -= bE;
                //          }

                //          totalDetails++;
                //        }

                //        if (BAqum > nQtyAllocBad)
                //        {
                //          BLop = false;
                //        }
                //      }
                //    }
                //  }
                //  #endregion
                //}
                //if (field.Quantity > 0 && field.BadQuantity > 0)
                //{
                //  #region Good > 0 And Bad > 0

                //  listResRND1 = (from q in db.LG_RND1s
                //                 join q1 in db.LG_RNHs on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                //                 join q2 in db.LG_MsBatches on q.c_batch equals q2.c_batch
                //                 where q.c_gdg == gdgAsal && q.c_batch == field.Batch
                //                 && q.c_iteno == field.Item && q.n_bsisa > 0 && q.n_gsisa > 0
                //                 orderby q2.d_expired descending, q1.d_rndate
                //                 select q).Distinct().ToList();

                //  SJStock = (from q in GlobalQuery.ViewStockLite(db, gdgAsal, field.Item)
                //               where q.c_batch == field.Batch && q.n_bsisa > 0 && q.n_gsisa > 0
                //               select new SJClassComponent
                //               {
                //                 Box
                //               }).ToList();

                //  if (Noref.Count > 0)
                //  {
                //    nQtyAllocGood = field.Quantity;
                //    nQtyAllocBad = field.BadQuantity;
                //    decimal BAqum = 0, GAqum = 0, bE = 0, gE = 0, bI = 0, gI = 0;
                //    bool GLop = true, BLop = true;

                //    for (nLoopC = 0; nLoopC < listResRND1.Count; nLoopC++)
                //    {
                //      if (listResRND1[nLoopC].n_bsisa > 0 || Noref[nLoopC].n_gsisa > 0)
                //      {
                //        bI = nQtyAllocBad - Noref[nLoopC].n_bsisa;
                //        gI = nQtyAllocGood - Noref[nLoopC].n_gsisa;

                //        if (BLop == true)
                //        {
                //          bE = nQtyAllocBad - bI;
                //          BAqum = BAqum + bE;
                //        }
                //        if (GLop == true)
                //        {
                //          gE = nQtyAllocGood - gI;
                //          GAqum = GAqum + gE;
                //        }
                //        if (BAqum > nQtyAllocBad)
                //        {
                //          bE = nQtyAllocBad - (BAqum - bE);
                //          BAqum = BAqum + bE;
                //        }
                //        if (GAqum > nQtyAllocGood)
                //        {
                //          gE = nQtyAllocGood - (GAqum - gE);
                //          GAqum = GAqum + gE;
                //        }
                //        if (BLop == false)
                //        {
                //          bE = 0;
                //        }
                //        if (GLop == false)
                //        {
                //          gE = 0;
                //        }

                //        if (bE != 0 && gE != 0)
                //        {
                //          listSjd2.Add(new LG_SJD2()
                //          {
                //            c_batch = field.Batch,
                //            c_gdg = gdgAsal,
                //            c_iteno = field.Item,
                //            c_spgno = string.Empty,
                //            c_rnno = Noref[nLoopC].c_no,
                //            c_sjno = sjID,
                //            n_bqty = bE,
                //            n_gqty = gE,
                //            n_bsisa = bE,
                //            n_gsisa = gE
                //          });

                //          if (Noref[nLoopC].c_table.Equals("RN"))
                //          {
                //            rnd1 = (from q in db.LG_RND1s
                //                    where q.c_gdg == gdgAsal &&
                //                    q.c_iteno == Noref[nLoopC].c_iteno
                //                    && q.c_batch == Noref[nLoopC].c_batch
                //                    && q.c_rnno == Noref[nLoopC].c_no
                //                    select q).Take(1).SingleOrDefault();

                //            rnd1.n_gsisa -= gE;
                //            rnd1.n_bsisa -= bE;
                //          }
                //          else
                //          {
                //            comboh = (from q in db.LG_ComboHs
                //                      where q.c_gdg == gdgAsal &&
                //                      q.c_iteno == Noref[nLoopC].c_iteno
                //                      && q.c_batch == Noref[nLoopC].c_batch
                //                      && q.c_combono == Noref[nLoopC].c_no
                //                      select q).Take(1).SingleOrDefault();

                //            comboh.n_gsisa -= gE;
                //            comboh.n_bsisa -= bE;
                //          }

                //          totalDetails++;
                //        }
                //        if (BAqum > nQtyAllocBad)
                //        {
                //          BLop = false;
                //        }
                //        if (GAqum > nQtyAllocGood)
                //        {
                //          GLop = false;
                //        }
                //      }
                //    }
                //  }
                //  #endregion
                //}

                //listSjd1.Add(new LG_SJD1()
                //{
                //  c_batch = field.Batch,
                //  c_gdg = gdgAsal,
                //  c_iteno = field.Item,
                //  c_sjno = sjID,
                //  c_spgno = string.Empty,
                //  n_bqty = field.BadQuantity,
                //  n_booked_bad = field.BadQuantity,
                //  n_booked =field.Quantity,
                //  n_gqty = field.Quantity
                //});

                #endregion

                #endregion
              }
            }

            //lstItemCombo.Clear();
            //lstItem.Clear();

          }

          #endregion

          dic = new Dictionary<string, string>();

          if (totalDetails > 0)
          {
            if ((listSjd1 != null) && (listSjd1.Count > 0))
            {
              db.LG_SJD1s.InsertAllOnSubmit(listSjd1.ToArray());
              listSjd1.Clear();
            }

            if ((listSjd2 != null) && (listSjd2.Count > 0))
            {
              db.LG_SJD2s.InsertAllOnSubmit(listSjd2.ToArray());
              listSjd2.Clear();
            }

            dic.Add("SJ", sjID);
            dic.Add("Tanggal", date.ToString("yyyyMMdd"));

            result = string.Format("Total {0} detail(s)", totalDetails);

            hasAnyChanges = true;
          }

          #endregion
        }
        else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify

          if (string.IsNullOrEmpty(sjID))
          {
            result = "Nomor Surat Jalan dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          sjh = (from q in db.LG_SJHs
                 where (q.c_gdg == gdgAsal) && (q.c_sjno == sjID)
                 select q).Take(1).SingleOrDefault();

          if (sjh == null)
          {
            result = "Nomor Surat Jalan tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (sjh.l_delete.HasValue && sjh.l_delete.Value)
          {
            result = "Tidak dapat mengubah nomor Surat Jalan yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (sjh.l_confirm.HasValue && sjh.l_confirm.Value)
          {
            result = "Tidak dapat mengubah nomor Surat Jalan yang sudah terkonfirm.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (sjh.l_confirm.HasValue && sjh.l_confirm.Value)
          {
            result = "Tidak dapat mengubah nomor Transfer Gudang yang sudah terkonfirm.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, sjh.d_sjdate))
          {
            result = "Surat Jalan tidak dapat diubah, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.HasExp(db, sjID))
          {
            result = "Surat Jalan yang sudah terdapat Delivery Order tidak dapat diubah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
          {
            sjh.v_ket = structure.Fields.Keterangan;
          }

          if (!string.IsNullOrEmpty(structure.Fields.asalProduk))
          {
            sjh.c_product_origin = structure.Fields.asalProduk;
          }

   
          sjh.v_product_origin = string.IsNullOrEmpty(structure.Fields.cabangExp) ? null : structure.Fields.cabangExp;


          if (!string.IsNullOrEmpty(structure.Fields.noDok))
          {
            sjh.v_nodok = structure.Fields.noDok;
          }

          if (!string.IsNullOrEmpty(structure.Fields.memo))
          {
            sjh.c_mpno = structure.Fields.memo;
          }

          if (structure.Method.Equals("ModifyConfirm", StringComparison.OrdinalIgnoreCase))
          {
            sjh.l_confirm = structure.Fields.IsConfirm;

            isConfirmed = true;


            sjh.c_confirm = nipEntry;
            sjh.d_confirm = DateTime.Now;
          }
          else
          {
            isConfirmed = (sjh.l_confirm.HasValue ? sjh.l_confirm.Value : false);
          }

          sjh.c_update = nipEntry;
          if (sjh.l_status == false)
          {
              sjh.d_update = DateTime.Now;
          }
          #region Populate Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {

            listSjd1 = new List<LG_SJD1>();
            listSjd2 = new List<LG_SJD2>();
            comboh = new LG_ComboH();
            rnd1 = new LG_RND1();

            listSjd3 = new List<LG_SJD3>();
            listRND1 = new List<LG_RND1>();
            listComboh = new List<LG_ComboH>();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              listResRND1 = (from q in db.LG_RND1s
                             where q.c_gdg == gdgAsal && q.c_iteno == field.Item
                             select q).ToList();

              listResComboh = (from q in db.LG_ComboHs
                               where q.c_gdg == gdgAsal && q.c_iteno == field.Item
                               select q).ToList();

              if ((field != null) && field.IsNew && (!field.IsDelete) && ((field.Quantity > 0) || (field.BadQuantity > 0)))
              {
                #region New

                SJStock = (from q in GlobalQuery.ViewStockLite(db, gdgAsal, field.Item)
                           where (q.n_gsisa != 0 || q.n_bsisa > 0)
                           select new SJClassComponent()
                           {
                             Item = q.c_iteno,
                             RefID = q.c_no,
                             BatchID = q.c_batch,
                             Qty = q.n_gsisa,
                             QtyBad = q.n_bsisa,
                             SignID = q.c_table
                           }).ToList();

                nLoopC = SJStock.GroupBy(y => new { y.Item }).Where(x => (x.Sum(z => z.QtyBad) >= field.BadQuantity)
                  && (x.Sum(z => z.Qty) >= field.Quantity)).Count();

                nQtyAllocGood = field.Quantity;
                nQtyAllocBad = field.BadQuantity;

                if (nLoopC <= 0)
                {
                  result = "Tidak dapat menghapus Nomor Receive jika ada salah satu data detil ada yang berubah.";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  if (db.Transaction != null)
                  {
                    db.Transaction.Rollback();
                  }

                  goto endLogic;
                }

                if (SJStock != null && SJStock.Count > 0)
                {
                  SJStockRn = SJStock.FindAll(delegate(SJClassComponent sjc)
                  {
                    return field.Item.Trim().Equals((string.IsNullOrEmpty(sjc.Item) ? string.Empty : sjc.Item.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    field.Batch.Trim().Equals((string.IsNullOrEmpty(sjc.BatchID) ? string.Empty : sjc.BatchID.Trim()), StringComparison.OrdinalIgnoreCase);
                  });

                  if (SJStockRn.Count > 0)
                  {
                    for (nLoopC = 0; nLoopC < SJStockRn.Count; nLoopC++)
                    {
                      SJStockBatch = SJStockRn[nLoopC];

                      if (SJStockBatch.SignID.Equals("RN") || SJStockBatch.SignID.Equals("RR"))
                      {
                        rnd1 = listResRND1.Find(delegate(LG_RND1 rnd)
                        {
                          return field.Item.Trim().Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                            field.Batch.Trim().Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                            SJStockBatch.RefID.Trim().Equals((string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
                        });

                        nQtyRelocGood = (nQtyAllocGood > rnd1.n_gsisa ? rnd1.n_gsisa.Value : nQtyAllocGood);
                        nQtyRelocBad = (nQtyAllocBad > rnd1.n_bsisa ? rnd1.n_bsisa.Value : nQtyAllocBad);

                        if (rnd1 != null)
                        {
                          sjd2 = new LG_SJD2()
                          {
                            c_batch = field.Batch,
                            c_gdg = gdgAsal,
                            c_iteno = field.Item,
                            c_sjno = sjID,
                            c_spgno = string.Empty,
                            c_rnno = rnd1.c_rnno,
                            n_bqty = nQtyRelocBad,
                            n_bsisa = nQtyRelocBad,
                            n_gqty = nQtyRelocGood,
                            n_gsisa = nQtyRelocGood,
                          };

                          if (sjd2 != null)
                          {
                            db.LG_SJD2s.InsertOnSubmit(sjd2);

                            rnd1.n_gsisa -= nQtyRelocGood;
                            rnd1.n_bsisa -= nQtyRelocBad;

                            if (rnd1.n_gsisa < 0) //cek rn minus
                            {
                                result = "Qty RN Kurang (modify Transfer Gudang) " + field.Item + " " + field.Batch;
                                rpe = ResponseParser.ResponseParserEnum.IsFailed;
                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }
                                goto endLogic;
                            }

                            nQtyAllocGood -= nQtyRelocGood;
                            nQtyAllocBad -= nQtyRelocBad;

                            totalDetails++;
                          }
                          
                        }
                      }
                      else
                      {
                        comboh = listComboh.Find(delegate(LG_ComboH cmb)
                        {
                          return field.Item.Trim().Equals((string.IsNullOrEmpty(cmb.c_iteno) ? string.Empty : cmb.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                            field.Batch.Trim().Equals((string.IsNullOrEmpty(cmb.c_batch) ? string.Empty : cmb.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                            SJStockBatch.RefID.Trim().Equals((string.IsNullOrEmpty(cmb.c_combono) ? string.Empty : cmb.c_combono.Trim()), StringComparison.OrdinalIgnoreCase);
                        });

                        nQtyRelocGood = (nQtyAllocGood > rnd1.n_gsisa ? rnd1.n_gsisa.Value : nQtyAllocGood);
                        nQtyRelocBad = (nQtyAllocBad > rnd1.n_bsisa ? rnd1.n_bsisa.Value : nQtyAllocBad);

                        if (rnd1 != null)
                        {
                          sjd2 = new LG_SJD2()
                          {
                            c_batch = field.Batch,
                            c_gdg = gdgAsal,
                            c_iteno = field.Item,
                            c_sjno = sjID,
                            c_spgno = string.Empty,
                            c_rnno = rnd1.c_rnno,
                            n_bqty = nQtyRelocBad,
                            n_bsisa = nQtyRelocBad,
                            n_gqty = nQtyRelocGood,
                            n_gsisa = nQtyRelocGood,
                          };

                          if (sjd2 != null)
                          {
                            db.LG_SJD2s.InsertOnSubmit(sjd2);

                            comboh.n_gsisa -= nQtyRelocGood;
                            comboh.n_bsisa -= nQtyRelocBad;

                            nQtyAllocGood -= nQtyRelocGood;
                            nQtyAllocBad -= nQtyRelocBad;
                            totalDetails++;
                          }
                        }
                      }
                    }
                  }

                  sjd1 = new LG_SJD1()
                  {
                    c_batch = field.Batch,
                    c_gdg = gdgAsal,
                    c_iteno = field.Item,
                    c_sjno = sjID,
                    c_spgno = string.Empty,
                    n_booked = field.Quantity,
                    n_booked_bad = field.BadQuantity,
                    n_bqty = field.BadQuantity,
                    n_gqty = field.Quantity
                  };

                  if (sjd1 != null)
                  {
                    db.LG_SJD1s.InsertOnSubmit(sjd1);
                  }

                    if(!string.IsNullOrEmpty(structure.Fields.memo))
                    {
                        mpd = (from q in db.MK_MPHs 
                             join q1 in db.MK_MPDs on new { c_gdg = q.c_gdg, q.c_mpno } equals new { q1.c_gdg, q1.c_mpno }
                             where (q.c_mpno == structure.Fields.memo)
                              && (q.c_gdg == gdgAsal) && (q1.c_iteno == field.Item)
                              && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                             select q1).Take(1).SingleOrDefault();

                        if(mpd != null)
                        {
                            mpd.n_sisa -= field.BadQuantity;
                        }
                        else
                        {
                            result = "Memo tidak ditemukan";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                              db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                    }
                }

                #endregion
              }
              else if ((field != null) && (!field.IsNew) && field.IsDelete && ((field.Quantity > 0) || (field.BadQuantity > 0)))
              {
                #region Delete

                #region Old Code

                //if (lstItemCombo.Contains(field.Item))
                //{
                //  #region Combo

                //  listSjd2Copy = listSjd2.FindAll(delegate(LG_SJD2 sjd)
                //  {
                //    return field.Item.Equals((string.IsNullOrEmpty(sjd.c_iteno) ? string.Empty : sjd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                //      field.Batch.Equals((string.IsNullOrEmpty(sjd.c_batch) ? string.Empty : sjd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                //  });

                //  for (nLoopC = 0; nLoopC < listSjd2Copy.Count; nLoopC++)
                //  {
                //    sjd2 = listSjd2Copy[nLoopC];

                //    rnID = (string.IsNullOrEmpty(sjd2.c_rnno) ? string.Empty : sjd2.c_rnno.Trim());

                //    //itemID = (string.IsNullOrEmpty(sjd2.c_iteno) ? string.Empty : sjd2.c_iteno.Trim());
                //    //batchCode = (string.IsNullOrEmpty(sjd2.c_batch) ? string.Empty : sjd2.c_batch.Trim());
                //    ////sgID = (string.IsNullOrEmpty(sjd2.c_spgno) ? string.Empty : sjd2.c_spgno.Trim());

                //    spAllocBad = (sjd2.n_bqty.HasValue ? sjd2.n_bqty.Value : 0);
                //    spAllocGood = (sjd2.n_gqty.HasValue ? sjd2.n_gqty.Value : 0);

                //    if ((spAllocBad != (sjd2.n_bsisa.HasValue ? sjd2.n_bsisa.Value : 0)) ||
                //      (spAllocGood != (sjd2.n_gsisa.HasValue ? sjd2.n_gsisa.Value : 0)))
                //    {
                //      continue;
                //    }

                //    #region Data Combo

                //    listResComboh = listComboh.FindAll(delegate(LG_ComboH cmb)
                //    {
                //      return gdgAsal.Equals(cmb.c_gdg) &&
                //        field.Item.Equals((string.IsNullOrEmpty(cmb.c_iteno) ? string.Empty : cmb.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                //        field.Batch.Equals((string.IsNullOrEmpty(cmb.c_batch) ? string.Empty : cmb.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                //        (((cmb.n_gsisa.HasValue ? cmb.n_gsisa.Value : 0) > 0) || ((cmb.n_bsisa.HasValue ? cmb.n_bsisa.Value : 0) > 0));
                //    });

                //    if ((listResComboh == null) || (listResComboh.Count < 1))
                //    {
                //      listResComboh = (from q in db.LG_ComboHs
                //                       where (q.c_gdg == gdgAsal) && (q.c_iteno == field.Item) && (q.c_batch == field.Batch)
                //                         && ((q.n_gsisa > 0) || (q.n_bsisa > 0))
                //                         && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                //                       select q).ToList();

                //      if (listResComboh.Count > 0)
                //      {
                //        listComboh.AddRange(listResComboh.ToArray());
                //      }
                //    }

                //    #endregion

                //    #region SJD1

                //    sjd1 = listSjd1.Find(delegate(LG_SJD1 sjd)
                //    {
                //      return field.Item.Equals((string.IsNullOrEmpty(sjd.c_iteno) ? string.Empty : sjd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                //        field.Batch.Equals((string.IsNullOrEmpty(sjd.c_batch) ? string.Empty : sjd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                //    });

                //    if (sjd1 == null)
                //    {
                //      continue;
                //    }

                //    #endregion

                //    db.LG_SJD2s.DeleteOnSubmit(sjd2);

                //    listSjd2.Remove(sjd2);

                //    sjd1.n_bqty -= spAllocBad;
                //    sjd1.n_gqty -= spAllocGood;

                //    comboh.n_bsisa += spAllocBad;
                //    comboh.n_gsisa += spAllocGood;

                //    listSjd3.Add(new LG_SJD3()
                //    {
                //      c_gdg = gdgAsal,
                //      c_sjno = sjID,
                //      c_iteno = field.Item,
                //      c_batch = field.Batch,
                //      c_rnno = rnID,
                //      c_spgno = string.Empty,
                //      n_bqty = spAllocBad,
                //      n_bsisa = spAllocBad,
                //      n_gqty = spAllocGood,
                //      n_gsisa = spAllocGood,
                //      c_entry = nipEntry,
                //      d_entry = date,
                //      v_type = "03",
                //      v_ket_del = string.Concat((isConfirmed ? "Confirm : " : string.Empty),
                //            (string.IsNullOrEmpty(structure.Fields.Keterangan) ? "Human error" : structure.Fields.Keterangan))
                //    });

                //    spAllocBad = (sjd1.n_bqty.HasValue ? sjd1.n_bqty.Value : 0);
                //    spAllocGood = (sjd1.n_gqty.HasValue ? sjd1.n_gqty.Value : 0);

                //    if ((spAllocBad == 0) && (spAllocGood == 0))
                //    {
                //      db.LG_SJD1s.DeleteOnSubmit(sjd1);

                //      listSjd1.Remove(sjd1);
                //    }
                //  }

                //  listSjd2Copy.Clear();

                //  #endregion
                //}
                //else
                //{
                //  #region RN

                //  listSjd2Copy = listSjd2.FindAll(delegate(LG_SJD2 sjd)
                //  {
                //    return field.Item.Equals((string.IsNullOrEmpty(sjd.c_iteno) ? string.Empty : sjd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                //      field.Batch.Equals((string.IsNullOrEmpty(sjd.c_batch) ? string.Empty : sjd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                //  });

                //  for (nLoopC = 0; nLoopC < listSjd2Copy.Count; nLoopC++)
                //  {
                //    sjd2 = listSjd2Copy[nLoopC];

                //    rnID = (string.IsNullOrEmpty(sjd2.c_rnno) ? string.Empty : sjd2.c_rnno.Trim());

                //    //itemID = (string.IsNullOrEmpty(sjd2.c_iteno) ? string.Empty : sjd2.c_iteno.Trim());
                //    //batchCode = (string.IsNullOrEmpty(sjd2.c_batch) ? string.Empty : sjd2.c_batch.Trim());
                //    ////sgID = (string.IsNullOrEmpty(sjd2.c_spgno) ? string.Empty : sjd2.c_spgno.Trim());

                //    spAllocBad = (sjd2.n_bqty.HasValue ? sjd2.n_bqty.Value : 0);
                //    spAllocGood = (sjd2.n_gqty.HasValue ? sjd2.n_gqty.Value : 0);

                //    if ((spAllocBad != (sjd2.n_bsisa.HasValue ? sjd2.n_bsisa.Value : 0)) ||
                //      (spAllocGood != (sjd2.n_gsisa.HasValue ? sjd2.n_gsisa.Value : 0)))
                //    {
                //      continue;
                //    }

                //    #region RN

                //    rnd1 = listRND1.Find(delegate(LG_RND1 rnd)
                //    {
                //      return gdgAsal.Equals(rnd.c_gdg) &&
                //        rnID.Equals((string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                //        field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                //        field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                //    });

                //    if (rnd1 == null)
                //    {
                //      rnd1 = (from q in db.LG_RNHs
                //              join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                //              where (q.c_gdg == gdgAsal) && (q.c_rnno == rnID)
                //                && (q1.c_iteno == field.Item) && (q1.c_batch == field.Batch)
                //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                //              select q1).Take(1).SingleOrDefault();

                //      if (rnd1 != null)
                //      {
                //        listRND1.Add(rnd1);
                //      }
                //    }

                //    if (rnd1 == null)
                //    {
                //      continue;
                //    }

                //    #endregion

                //    #region SJD1

                //    sjd1 = listSjd1.Find(delegate(LG_SJD1 sjd)
                //    {
                //      return field.Item.Equals((string.IsNullOrEmpty(sjd.c_iteno) ? string.Empty : sjd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                //        field.Batch.Equals((string.IsNullOrEmpty(sjd.c_batch) ? string.Empty : sjd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                //    });

                //    if (sjd1 == null)
                //    {
                //      continue;
                //    }

                //    #endregion

                //    db.LG_SJD2s.DeleteOnSubmit(sjd2);

                //    listSjd2.Remove(sjd2);

                //    sjd1.n_bqty -= spAllocBad;
                //    sjd1.n_gqty -= spAllocGood;

                //    rnd1.n_bsisa += spAllocBad;
                //    rnd1.n_gsisa += spAllocGood;

                //    listSjd3.Add(new LG_SJD3()
                //    {
                //      c_gdg = gdgAsal,
                //      c_sjno = sjID,
                //      c_iteno = field.Item,
                //      c_batch = field.Batch,
                //      c_rnno = rnID,
                //      c_spgno = string.Empty,
                //      n_bqty = spAllocBad,
                //      n_bsisa = spAllocBad,
                //      n_gqty = spAllocGood,
                //      n_gsisa = spAllocGood,
                //      c_entry = nipEntry,
                //      d_entry = date,
                //      v_type = "03",
                //      v_ket_del = string.Concat((isConfirmed ? "Confirm : " : string.Empty),
                //            (string.IsNullOrEmpty(structure.Fields.Keterangan) ? "Human error" : structure.Fields.Keterangan))
                //    });

                //    spAllocBad = (sjd1.n_bqty.HasValue ? sjd1.n_bqty.Value : 0);
                //    spAllocGood = (sjd1.n_gqty.HasValue ? sjd1.n_gqty.Value : 0);

                //    if ((spAllocBad == 0) && (spAllocGood == 0))
                //    {
                //      db.LG_SJD1s.DeleteOnSubmit(sjd1);

                //      listSjd1.Remove(sjd1);
                //    }
                //  }

                //  listSjd2Copy.Clear();

                //  #endregion
                //}

                #endregion

                sjd2 = (from q in db.LG_SJD2s
                        where q.c_batch == field.Batch && q.c_iteno == field.Item &&
                        q.c_sjno == sjID
                        select q).Distinct().Take(1).SingleOrDefault();

                sjd1 = (from q in db.LG_SJD1s
                        where q.c_batch == field.Batch && q.c_iteno == field.Item
                        && q.c_sjno == sjID
                        select q).Distinct().Take(1).SingleOrDefault();

                if (sjd1 != null)
                {
                  spQtyReloc = (sjd2.n_gqty.HasValue ? sjd2.n_gqty.Value : 0);
                  spAlloc = (sjd2.n_gsisa.HasValue ? sjd2.n_gsisa.Value : 0);

                  if (spAlloc.Equals(spQtyReloc))
                  {
                    #region Reverse Data

                    listSjd2 = (from q in db.LG_SJD2s
                                where q.c_sjno == sjID && q.c_iteno == sjd2.c_iteno &&
                                q.c_batch == sjd2.c_batch
                                select q).Distinct().ToList();


                    if ((listSjd2 != null) && (listSjd2.Count > 0))
                    {
                      for (nLoopC = 0; nLoopC < listSjd2.Count; nLoopC++)
                      {
                        sjd2 = listSjd2[nLoopC];

                        if (sjd2 != null)
                        {
                            if (sjd2.c_rnno.Substring(0, 2) == "RN" || sjd2.c_rnno.Substring(0, 2) == "RR")
                          {
                            rnd1 = (from q in db.LG_RND1s
                                    where q.c_batch == sjd2.c_batch && q.c_iteno == sjd2.c_iteno &&
                                    q.c_rnno == sjd2.c_rnno && q.c_gdg == sjh.c_gdg
                                    select q).Distinct().Take(1).SingleOrDefault();

                            rnd1.n_gsisa += (sjd2.n_gsisa);
                            rnd1.n_bsisa += (sjd2.n_bsisa);
                          }
                          else
                          {
                            comboh = (from q in db.LG_ComboHs
                                      where q.c_gdg == gdgAsal &&
                                      q.c_iteno == sjd2.c_iteno
                                      && q.c_batch == sjd2.c_batch
                                      && q.c_combono == sjd2.c_rnno
                                      select q).Take(1).SingleOrDefault();

                            comboh.n_gsisa += (sjd2.n_gsisa);
                            comboh.n_bsisa += (sjd2.n_bsisa);
                          }
                          //listResRND1.Add(rnd1);

                          #region Log SJD3

                          listSjd3.Add(new LG_SJD3()
                            {
                              c_sjno = sjID,
                              c_batch = sjd2.c_batch,
                              c_entry = nipEntry,
                              c_gdg = sjd2.c_gdg,
                              c_iteno = sjd2.c_iteno,
                              c_rnno = sjd2.c_rnno,
                              c_spgno = sjd2.c_spgno,
                              d_entry = date.Date,
                              n_bqty = sjd2.n_bqty,
                              n_bsisa = sjd2.n_bsisa,
                              n_gqty = sjd2.n_gqty,
                              n_gsisa = sjd2.n_gsisa,
                              v_type = "03",
                              v_ket_del = field.KeteranganMod,
                              c_product_origin = sjh.c_product_origin,
                              v_nodok = sjh.v_nodok,
                              v_product_origin = sjh.v_product_origin,
                              c_mpno = sjh.c_mpno
                            });

                          #region Delete SJD2

                          db.LG_SJD2s.DeleteOnSubmit(sjd2);

                          #endregion

                          #endregion
                        }
                      }

                      if(!string.IsNullOrEmpty(structure.Fields.memo))  //reverse MPD
                        {
                            mpd = (from q in db.MK_MPHs 
                                 join q1 in db.MK_MPDs on new { c_gdg = q.c_gdg, q.c_mpno } equals new { q1.c_gdg, q1.c_mpno }
                                 where (q.c_mpno == structure.Fields.memo)
                                  && (q.c_gdg == gdgAsal) && (q1.c_iteno == field.Item)
                                  && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                 select q1).Take(1).SingleOrDefault();

                            if(mpd != null)
                            {
                                mpd.n_sisa += field.BadQuantity;
                            }
                            else
                            {
                                result = "Memo tidak ditemukan";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                if (db.Transaction != null)
                                {
                                  db.Transaction.Rollback();
                                }

                                goto endLogic;
                            }
                        }
                    }

                    #endregion

                    #region Delete SJD1

                    if (sjd1 != null)
                    {
                      db.LG_SJD1s.DeleteOnSubmit(sjd1);
                    }

                    #endregion
                  }
                }

                #endregion
              }
            }

            //lstItemCombo.Clear();
            //lstItem.Clear();

            if (listSjd3.Count > 0)
            {
              db.LG_SJD3s.InsertAllOnSubmit(listSjd3.ToArray());
              listSjd3.Clear();
            }
            //if (listSjd1.Count > 0 && listSjd2.Count > 0)
            //{
            //  db.LG_SJD1s.InsertAllOnSubmit(listSjd1.ToArray());
            //  db.LG_SJD2s.InsertAllOnSubmit(listSjd2.ToArray());
            //  listSjd2.Clear();
            //  listSjd1.Clear();
            //}

            if (listRND1 != null && listRND1.Count > 0)
            {
              listRND1.Clear();
            }
          }

          #endregion

          hasAnyChanges = true;

          #endregion
        }
        else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete

          if (string.IsNullOrEmpty(sjID))
          {
            result = "Nomor Surat Jalan dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          sjh = (from q in db.LG_SJHs
                 where (q.c_gdg == gdgAsal) && (q.c_sjno == sjID)
                 select q).Take(1).SingleOrDefault();

          if (sjh == null)
          {
            result = "Nomor Surat Jalan tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (sjh.l_delete.HasValue && sjh.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor Surat Jalan yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (sjh.l_confirm.HasValue && sjh.l_confirm.Value)
          {
            result = "Tidak dapat menghapus nomor Surat Jalan yang sudah terkonfirm.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, sjh.d_sjdate))
          {
            result = "Surat Jalan tidak dapat dihapus, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.HasExp(db, sjID))
          {
            result = "Surat Jalan yang sudah terdapat Ekspedisi tidak dapat diubah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          gdgTujuan = (sjh.c_gdg2.HasValue ? sjh.c_gdg2.Value : char.MinValue);

          sjh.c_update = nipEntry;
          if (sjh.l_status == false)
          {
              sjh.d_update = DateTime.Now;
          }
          sjh.l_delete = true;
          sjh.v_ket_mark = structure.Fields.Keterangan;

          listRND1 = new List<LG_RND1>();
          //listSPGD1 = new List<LG_SPGD1>();
          listSjd3 = new List<LG_SJD3>();
          listComboh = new List<LG_ComboH>();

          listSjd1 = (from q in db.LG_SJHs
                      join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                      where (q.c_gdg == gdgAsal) && (q.c_sjno == sjID)
                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      select q1).Distinct().ToList();

          listSjd2 = (from q in db.LG_SJHs
                      join q1 in db.LG_SJD2s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                      where (q.c_gdg == gdgAsal) && (q.c_sjno == sjID)
                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      select q1).Distinct().ToList();

          //lstItem = listSjd1.GroupBy(x => (string.IsNullOrEmpty(x.c_iteno) ? string.Empty : x.c_iteno.Trim())).Select(y => y.Key.Trim()).ToList();

          //lstItemCombo = (from q in db.FA_MasItms
          //                where lstItem.Contains(q.c_iteno)
          //                  && ((q.l_combo.HasValue ? q.l_combo.Value : false) == true)
          //                select (q.c_iteno == null ? string.Empty : q.c_iteno.Trim().ToLower())).Distinct().ToList();

          for (nLoop = 0; nLoop < listSjd2.Count; nLoop++)
          {
            sjd2 = listSjd2[nLoop];

            if (sjd2 != null)
            {
              rnID = (string.IsNullOrEmpty(sjd2.c_rnno) ? string.Empty : sjd2.c_rnno.Trim());
              itemID = (string.IsNullOrEmpty(sjd2.c_iteno) ? string.Empty : sjd2.c_iteno.Trim());
              batchCode = (string.IsNullOrEmpty(sjd2.c_batch) ? string.Empty : sjd2.c_batch.Trim());
              //sgID = (string.IsNullOrEmpty(sjd2.c_spgno) ? string.Empty : sjd2.c_spgno.Trim());

              #region Delete

              spAllocBad = (sjd2.n_bqty.HasValue ? sjd2.n_bqty.Value : 0);
              spAllocGood = (sjd2.n_gqty.HasValue ? sjd2.n_gqty.Value : 0);

              if ((spAllocBad != (sjd2.n_bsisa.HasValue ? sjd2.n_bsisa.Value : 0)) ||
                (spAllocGood != (sjd2.n_gsisa.HasValue ? sjd2.n_gsisa.Value : 0)))
              {
                result = "Detail Surat Jalan pernah dipakai";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                if (db.Transaction != null)
                {
                  db.Transaction.Rollback();
                }

                goto endLogic;
              }

              if (sjd2.c_rnno.Substring(0, 2) == "RN" || sjd2.c_rnno.Substring(0, 2) == "RR")
              {

                #region RN

                rnd1 = listRND1.Find(delegate(LG_RND1 rnd)
                {
                  return gdgAsal.Equals(rnd.c_gdg) &&
                    rnID.Equals((string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    itemID.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    batchCode.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                });

                if (rnd1 == null)
                {
                  rnd1 = (from q in db.LG_RNHs
                          join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                          where (q.c_gdg == gdgAsal) && (q.c_rnno == rnID)
                            && (q1.c_iteno == sjd2.c_iteno) && (q1.c_batch == batchCode)
                            && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                          select q1).Take(1).SingleOrDefault();

                  if (rnd1 != null)
                  {
                    listRND1.Add(rnd1);
                  }
                }

                if (rnd1 == null)
                {
                  result = "RN Tidak ditemukan";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  if (db.Transaction != null)
                  {
                    db.Transaction.Rollback();
                  }

                  goto endLogic;
                }

                #endregion

                if (rnd1 != null)
                {
                  rnd1.n_bsisa += spAllocBad;
                  rnd1.n_gsisa += spAllocGood;
                }
              }
              else
              {

                #region Data Combo

                comboh = listComboh.Find(delegate(LG_ComboH cmb)
                {
                  return gdgAsal.Equals(cmb.c_gdg) &&
                    rnID.Equals((string.IsNullOrEmpty(cmb.c_combono) ? string.Empty : cmb.c_combono.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    itemID.Equals((string.IsNullOrEmpty(cmb.c_iteno) ? string.Empty : cmb.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    batchCode.Equals((string.IsNullOrEmpty(cmb.c_batch) ? string.Empty : cmb.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                });

                if (comboh == null)
                {
                  comboh = (from q in db.LG_ComboHs
                            where (q.c_gdg == gdgAsal) && (q.c_iteno == itemID) && (q.c_batch == batchCode)
                                     && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                            select q).Take(1).SingleOrDefault();

                  if (comboh != null)
                  {
                    listComboh.Add(comboh);
                  }
                }

                if (comboh == null)
                {
                  result = "Combo Tidak ditemukan";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  if (db.Transaction != null)
                  {
                    db.Transaction.Rollback();
                  }

                  goto endLogic;
                }

                #endregion

                if (comboh != null)
                {
                  comboh.n_bsisa += spAllocBad;
                  comboh.n_gsisa += spAllocGood;
                }
              }

              listSjd3.Add(new LG_SJD3()
              {
                c_batch = sjd2.c_batch,
                c_iteno = sjd2.c_iteno,
                c_gdg = sjd2.c_gdg,
                c_rnno = sjd2.c_rnno,
                c_sjno = sjd2.c_sjno,
                c_spgno = sjd2.c_spgno,
                n_bqty = sjd2.n_bqty,
                n_bsisa = sjd2.n_bsisa,
                n_gqty = sjd2.n_gqty,
                n_gsisa = sjd2.n_gsisa,
                c_entry = nipEntry,
                d_entry = date,
                v_type = "03",
                v_ket_del = string.Concat((isConfirmed ? "Confirm : " : string.Empty),
                      (string.IsNullOrEmpty(structure.Fields.Keterangan) ? "Human error" : structure.Fields.Keterangan)),
                c_product_origin = sjh.c_product_origin,
                v_nodok = sjh.v_nodok,
                v_product_origin = sjh.v_product_origin,
                c_mpno = sjh.c_mpno
              });

              #endregion

            }
          }
          if (listSjd1.Count > 0)
          {
            db.LG_SJD1s.DeleteAllOnSubmit(listSjd1.ToArray());
            listSjd1.Clear();
          }
          if (listSjd2.Count > 0)
          {
            db.LG_SJD2s.DeleteAllOnSubmit(listSjd2.ToArray());
            listSjd2.Clear();
          }
          if (listSjd3.Count > 0)
          {
            db.LG_SJD3s.InsertAllOnSubmit(listSjd3.ToArray());
            listSjd3.Clear();
          }

          hasAnyChanges = true;

          #endregion
        }
        else if (structure.Method.Equals("ModifyConfirm", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify Confirm

          if (string.IsNullOrEmpty(sjID))
          {
            result = "Nomor Surat Jalan dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          sjh = (from q in db.LG_SJHs
                 where (q.c_gdg == gdgAsal) && (q.c_sjno == sjID)
                 select q).Take(1).SingleOrDefault();

          if (sjh == null)
          {
            result = "Nomor Surat Jalan tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (sjh.l_delete.HasValue && sjh.l_delete.Value)
          {
            result = "Tidak dapat mengubah nomor Surat Jalan yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          //else if (sjh.l_confirm.HasValue && sjh.l_confirm.Value)
          //{
          //  result = "Tidak dapat mengubah nomor Surat Jalan yang sudah terkonfirm.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}
          else if (Commons.IsClosingLogistik(db, sjh.d_sjdate))
          {
            result = "Surat Jalan tidak dapat diubah, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.HasExp(db, sjID))
          {
            result = "Surat Jalan yang sudah terdapat Delivery Order tidak dapat diubah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
          {
              sjh.v_ket = structure.Fields.Keterangan;
          }

          if (!string.IsNullOrEmpty(structure.Fields.asalProduk))
          {
              sjh.c_product_origin = structure.Fields.asalProduk;
          }

          if (!string.IsNullOrEmpty(structure.Fields.noDok))
          {
              sjh.v_nodok = structure.Fields.noDok;
          }

          if (!string.IsNullOrEmpty(structure.Fields.memo))
          {
            sjh.c_mpno = structure.Fields.memo;
          }

          sjh.v_product_origin = string.IsNullOrEmpty(structure.Fields.cabangExp) ? null : structure.Fields.cabangExp;

          sjhConfirm = sjh.l_confirm.HasValue ? sjh.l_confirm.Value : false;

          sjh.l_confirm = structure.Fields.IsConfirm;

          sjh.c_confirm = nipEntry;
          sjh.d_confirm = DateTime.Now;

          #region Populate Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            //lstTemp = new List<string>();

            //listSjd1 = new List<LG_SJD1>();
            //listSjd2 = new List<LG_SJD2>();

            listSjd1 = (from q in db.LG_SJHs
                        join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                        where (q.c_gdg == gdgAsal) && (q.c_sjno == sjID)
                          && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                        select q1).Distinct().ToList();

            listSjd2 = (from q in db.LG_SJHs
                        join q1 in db.LG_SJD2s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                        where (q.c_gdg == gdgAsal) && (q.c_sjno == sjID)
                          && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                        select q1).Distinct().ToList();

            listSjd3 = new List<LG_SJD3>();
            listRND1 = new List<LG_RND1>();
            listComboh = new List<LG_ComboH>();
            comboh = new LG_ComboH();
            rnd1 = new LG_RND1();

            //lstItem = structure.Fields.Field.GroupBy(x => x.Item).Select(y => y.Key.Trim()).ToList();

            //lstItemCombo = (from q in db.FA_MasItms
            //                where lstItem.Contains(q.c_iteno)
            //                  && ((q.l_combo.HasValue ? q.l_combo.Value : false) == true)
            //                select (q.c_iteno == null ? string.Empty : q.c_iteno.Trim().ToLower())).Distinct().ToList();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              listResRND1 = (from q in db.LG_RND1s
                             where q.c_gdg == gdgAsal && q.c_iteno == field.Item
                             select q).ToList();

              listResComboh = (from q in db.LG_ComboHs
                               where q.c_gdg == gdgAsal && q.c_iteno == field.Item
                               select q).ToList();

              if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified))
              {
                #region New

                SJStock = (from q in GlobalQuery.ViewStockLite(db, gdgAsal, field.Item)
                           where (q.n_gsisa != 0 || q.n_bsisa > 0)
                           select new SJClassComponent()
                           {
                             Item = q.c_iteno,
                             RefID = q.c_no,
                             BatchID = q.c_batch,
                             Qty = q.n_gsisa,
                             QtyBad = q.n_bsisa,
                             SignID = q.c_table
                           }).ToList();

                nLoopC = SJStock.GroupBy(y => new { y.Item }).Where(x => (x.Sum(z => z.QtyBad) >= field.BadQuantity)
                  && (x.Sum(z => z.Qty) >= field.Quantity)).Count();

                nQtyAllocGood = field.Quantity;
                nQtyAllocBad = field.BadQuantity;

                if (nLoopC <= 0)
                {
                  result = "Tidak dapat menghapus Nomor Receive jika ada salah satu data detil ada yang berubah.";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  if (db.Transaction != null)
                  {
                    db.Transaction.Rollback();
                  }

                  goto endLogic;
                }

                if (SJStock != null && SJStock.Count > 0)
                {
                  SJStockRn = SJStock.FindAll(delegate(SJClassComponent sjc)
                  {
                    return field.Item.Trim().Equals((string.IsNullOrEmpty(sjc.Item) ? string.Empty : sjc.Item.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    field.Batch.Trim().Equals((string.IsNullOrEmpty(sjc.BatchID) ? string.Empty : sjc.BatchID.Trim()), StringComparison.OrdinalIgnoreCase);
                  });

                  if (SJStockRn.Count > 0)
                  {
                    for (nLoopC = 0; nLoopC < SJStockRn.Count; nLoopC++)
                    {
                      SJStockBatch = SJStockRn[nLoopC];

                      if (SJStockBatch.SignID.Equals("RN") || SJStockBatch.SignID.Equals("RR"))
                      {
                        rnd1 = listResRND1.Find(delegate(LG_RND1 rnd)
                        {
                          return field.Item.Trim().Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                            field.Batch.Trim().Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                            SJStockBatch.RefID.Trim().Equals((string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
                        });

                        nQtyRelocGood = (nQtyAllocGood > rnd1.n_gsisa ? rnd1.n_gsisa.Value : nQtyAllocGood);
                        nQtyRelocBad = (nQtyAllocBad > rnd1.n_bsisa ? rnd1.n_bsisa.Value : nQtyAllocBad);

                        if (rnd1 != null)
                        {
                          sjd2 = new LG_SJD2()
                          {
                            c_batch = field.Batch,
                            c_gdg = gdgAsal,
                            c_iteno = field.Item,
                            c_sjno = sjID,
                            c_spgno = string.Empty,
                            c_rnno = rnd1.c_rnno,
                            n_bqty = nQtyRelocBad,
                            n_bsisa = nQtyRelocBad,
                            n_gqty = nQtyRelocGood,
                            n_gsisa = nQtyRelocGood,
                          };

                          if (sjd2 != null)
                          {
                            db.LG_SJD2s.InsertOnSubmit(sjd2);

                            rnd1.n_gsisa -= nQtyRelocGood;
                            rnd1.n_bsisa -= nQtyRelocBad;

                            if (rnd1.n_gsisa < 0) //cek rn minus
                            {
                                result = "Qty RN Kurang (modify confirm Transfer Gudang) " + field.Item + " " + field.Batch;
                                rpe = ResponseParser.ResponseParserEnum.IsFailed;
                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }
                                goto endLogic;
                            }

                            nQtyAllocGood -= nQtyRelocGood;
                            nQtyAllocBad -= nQtyRelocBad;

                            totalDetails++;
                          }

                        }
                      }
                      else
                      {
                        comboh = listComboh.Find(delegate(LG_ComboH cmb)
                        {
                          return field.Item.Trim().Equals((string.IsNullOrEmpty(cmb.c_iteno) ? string.Empty : cmb.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                            field.Batch.Trim().Equals((string.IsNullOrEmpty(cmb.c_batch) ? string.Empty : cmb.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                            SJStockBatch.RefID.Trim().Equals((string.IsNullOrEmpty(cmb.c_combono) ? string.Empty : cmb.c_combono.Trim()), StringComparison.OrdinalIgnoreCase);
                        });

                        nQtyRelocGood = (nQtyAllocGood > rnd1.n_gsisa ? rnd1.n_gsisa.Value : nQtyAllocGood);
                        nQtyRelocBad = (nQtyAllocBad > rnd1.n_bsisa ? rnd1.n_bsisa.Value : nQtyAllocBad);

                        if (rnd1 != null)
                        {
                          sjd2 = new LG_SJD2()
                          {
                            c_batch = field.Batch,
                            c_gdg = gdgAsal,
                            c_iteno = field.Item,
                            c_sjno = sjID,
                            c_spgno = string.Empty,
                            c_rnno = rnd1.c_rnno,
                            n_bqty = nQtyRelocBad,
                            n_bsisa = nQtyRelocBad,
                            n_gqty = nQtyRelocGood,
                            n_gsisa = nQtyRelocGood,
                          };

                          if (sjd2 != null)
                          {
                            db.LG_SJD2s.InsertOnSubmit(sjd2);

                            comboh.n_gsisa -= nQtyRelocGood;
                            comboh.n_bsisa -= nQtyRelocBad;

                            nQtyAllocGood -= nQtyRelocGood;
                            nQtyAllocBad -= nQtyRelocBad;
                            totalDetails++;
                          }
                        }
                      }
                    }
                  }

                  sjd1 = new LG_SJD1()
                  {
                    c_batch = field.Batch,
                    c_gdg = gdgAsal,
                    c_iteno = field.Item,
                    c_sjno = sjID,
                    c_spgno = string.Empty,
                    n_booked = field.Quantity,
                    n_booked_bad = field.BadQuantity,
                    n_bqty = field.BadQuantity,
                    n_gqty = field.Quantity
                  };

                  if (sjd1 != null)
                  {
                    db.LG_SJD1s.InsertOnSubmit(sjd1);
                  }
                }

                

                #endregion
              }
              else if ((field != null) && (!field.IsNew) && field.IsDelete && (!field.IsModified))
              {
                #region Delete

                listSjd2 = listSjd2.Where(x => x.c_iteno == field.Item 
                                            && x.c_batch.Trim() == field.Batch.Trim()
                                            //&& x.n_bsisa == x.n_bqty  
                                            //&& x.n_gqty == x.n_gsisa
                                            ).ToList();

                listSjd1 = listSjd1.Where(x => x.c_iteno == field.Item && x.c_batch.Trim() == field.Batch.Trim()).ToList();

                for (nLoopC = 0; nLoopC < listSjd2.Count; nLoopC++)
                {
                  sjd2 = listSjd2[nLoopC];

                  if (sjd2.c_rnno.Substring(0, 2) == "RN" || sjd2.c_rnno.Substring(0, 2) == "RR")
                  {
                    rnd1 = (from q in db.LG_RND1s
                            where q.c_gdg == gdgAsal &&
                            q.c_iteno == sjd2.c_iteno
                            && q.c_batch == sjd2.c_batch
                            && q.c_rnno == sjd2.c_rnno
                            select q).Take(1).SingleOrDefault();
                    if (rnd1 != null)
                    {
                      rnd1.n_gsisa += sjd2.n_gsisa;
                      rnd1.n_bsisa += sjd2.n_bsisa;
                    }
                  }
                  else
                  {
                    comboh = (from q in db.LG_ComboHs
                              where q.c_gdg == gdgAsal &&
                              q.c_iteno == sjd2.c_iteno
                              && q.c_batch == sjd2.c_batch
                              && q.c_combono == sjd2.c_rnno
                              select q).Take(1).SingleOrDefault();
                    if (comboh != null)
                    {
                      comboh.n_gsisa += sjd2.n_gsisa;
                      comboh.n_bsisa += sjd2.n_bsisa;
                    }
                  }

                  db.LG_SJD2s.DeleteOnSubmit(sjd2);

                  listSjd3.Add(new LG_SJD3()
                  {
                    c_gdg = gdgAsal,
                    c_sjno = sjID,
                    c_iteno = field.Item,
                    c_batch = field.Batch,
                    c_rnno = rnID,
                    c_spgno = string.Empty,
                    n_bqty = sjd2.n_bqty,
                    n_bsisa = sjd2.n_bsisa,
                    n_gqty = sjd2.n_gqty,
                    n_gsisa = sjd2.n_gsisa,
                    c_entry = nipEntry,
                    d_entry = date,
                    v_type = "03",
                    v_ket_del = string.Concat((isConfirmed ? "Confirm : " : string.Empty),
                          (string.IsNullOrEmpty(structure.Fields.Keterangan) ? "Human error" : structure.Fields.Keterangan)),
                    c_product_origin = sjh.c_product_origin,
                    v_nodok = sjh.v_nodok,
                    v_product_origin = sjh.v_product_origin,
                    c_mpno = sjh.c_mpno
                  });
                }

                if (listSjd1.Count > 0)
                {
                  db.LG_SJD1s.DeleteAllOnSubmit(listSjd1.ToArray());
                }

                #endregion
              }
              else if ((field != null) && (!field.IsNew) && (!field.IsDelete) && (field.IsModified))
              {
                #region Modified

                #region RN

                listSjd2Copy = listSjd2.FindAll(delegate(LG_SJD2 sjd)
                {
                  return field.Item.Equals((string.IsNullOrEmpty(sjd.c_iteno) ? string.Empty : sjd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    field.Batch.Equals((string.IsNullOrEmpty(sjd.c_batch) ? string.Empty : sjd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                });

                decimal nGAllocated = 0, nBAllocated = 0;

                nQtyAllocGood = field.sGQty;
                nQtyAllocBad = field.sBQty;

                for (nLoopC = 0; nLoopC < listSjd2Copy.Count; nLoopC++)
                {
                  sjd2 = listSjd2Copy[nLoopC];

                  if (nQtyAllocGood > 0 || nQtyAllocBad > 0)
                  {
                    rnID = (string.IsNullOrEmpty(sjd2.c_rnno) ? string.Empty : sjd2.c_rnno.Trim());

                    nGAllocated = (nQtyAllocGood > sjd2.n_gsisa.Value ? sjd2.n_gsisa.Value : nQtyAllocGood);
                    nBAllocated = (nQtyAllocBad > sjd2.n_bsisa.Value ? sjd2.n_bsisa.Value : nQtyAllocBad);

                    sjd2.n_bqty -= nBAllocated;
                    sjd2.n_bsisa -= nBAllocated;
                    sjd2.n_gqty -= nGAllocated;
                    sjd2.n_gsisa -= nGAllocated;

                    #region RN

                    if (nGAllocated > 0 || nBAllocated > 0)
                    {
                        if (sjd2.c_rnno.Substring(0, 2) == "RN" || sjd2.c_rnno.Substring(0, 2) == "RR")
                      {
                        rnd1 = (from q in db.LG_RND1s
                                where q.c_gdg == gdgAsal &&
                                q.c_iteno == sjd2.c_iteno
                                && q.c_batch == sjd2.c_batch
                                && q.c_rnno == sjd2.c_rnno
                                select q).Take(1).SingleOrDefault();
                        if (rnd1 != null)
                        {
                          rnd1.n_bsisa += nBAllocated;
                          rnd1.n_gsisa += nGAllocated;
                        }
                        totalDetails++;
                      }
                      else
                      {
                        comboh = (from q in db.LG_ComboHs
                                  where q.c_gdg == gdgAsal &&
                                  q.c_iteno == sjd2.c_iteno
                                  && q.c_batch == sjd2.c_batch
                                  && q.c_combono == sjd2.c_rnno
                                  select q).Take(1).SingleOrDefault();
                        if (comboh != null)
                        {
                          comboh.n_gsisa += nGAllocated;
                          comboh.n_bsisa += nBAllocated;
                        }
                        totalDetails++;
                      }

                      listSjd3.Add(new LG_SJD3()
                      {
                        c_gdg = gdgAsal,
                        c_sjno = sjID,
                        c_iteno = field.Item,
                        c_batch = field.Batch,
                        c_rnno = rnID,
                        c_spgno = string.Empty,
                        n_bqty = spAllocBad,
                        n_bsisa = spAllocBad,
                        n_gqty = spAllocGood,
                        n_gsisa = spAllocGood,
                        c_entry = nipEntry,
                        d_entry = date,
                        v_type = "03",
                        v_ket_del = string.Concat((isConfirmed ? "Confirm : " : string.Empty),
                              (string.IsNullOrEmpty(structure.Fields.Keterangan) ? "Human error" : structure.Fields.Keterangan)),
                        c_product_origin = string.IsNullOrEmpty(structure.Fields.asalProduk) ? null : structure.Fields.asalProduk,
                        v_nodok = string.IsNullOrEmpty(structure.Fields.memo) ? structure.Fields.noDok : sjID,
                        v_product_origin = string.IsNullOrEmpty(structure.Fields.cabangExp) ? null : structure.Fields.cabangExp,
                        c_mpno = string.IsNullOrEmpty(structure.Fields.memo) ? null : structure.Fields.memo
                      });
                    }

                    #endregion

                    nQtyAllocGood -= nGAllocated;
                    nQtyAllocBad -= nBAllocated;
                  }
                  
                }

                #region SJD1

                sjd1 = listSjd1.Find(delegate(LG_SJD1 sjd)
                {
                  return field.Item.Equals((string.IsNullOrEmpty(sjd.c_iteno) ? string.Empty : sjd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    field.Batch.Equals((string.IsNullOrEmpty(sjd.c_batch) ? string.Empty : sjd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                });

                if (sjd1 == null)
                {
                  continue;
                }

                #endregion

                if (sjd1 != null)
                {
                  sjd1.n_bqty -= field.sBQty;
                  sjd1.n_gqty -= field.sGQty;
                  sjd1.c_type_dc = field.tipe_dc;
                }

                #endregion

                #endregion
              }
            }

            //lstItemCombo.Clear();
            //lstItem.Clear();

            if (listSjd3.Count > 0)
            {
              db.LG_SJD3s.InsertAllOnSubmit(listSjd3.ToArray());
              listSjd3.Clear();
            }

            listSjd1.Clear();
            listSjd2.Clear();
            listSjd3.Clear();

            listRND1.Clear();
          }

          #endregion

          isDisposal = (structure.Fields.isDisposal ? true : false);

          if (isDisposal && structure.Fields.IsConfirm && !sjhConfirm)
          {
              listRNH = new List<LG_RNH>();
              listRND1 = new List<LG_RND1>();

              sjh.l_status = true;

              listSjd2 = (from q in db.LG_SJD2s
                          where q.c_sjno == sjID
                          select q).ToList();

              for (nLoopD = 0; nLoopD < listSjd2.Count; nLoopD++)
              {
                  #region RN

                  sjd2 = listSjd2[nLoopD];

                  refID = (sjd2.c_rnno ?? string.Empty);
                  sjDO = string.Concat("SJXX", gdgTujuan.ToString(), suplID);

                  rnh = listRNH.Find(delegate(LG_RNH rn)
                  {
                      return refID.Equals(rn.c_rnno, StringComparison.OrdinalIgnoreCase) &&
                        suplID.Equals((string.IsNullOrEmpty(rn.c_from) ? string.Empty : rn.c_from.Trim()), StringComparison.OrdinalIgnoreCase);
                  });

                  if (rnh == null)
                  {
                      rnh = (from q in db.LG_RNHs
                             where (q.c_gdg == gdgTujuan) && (q.c_rnno == refID)
                             //&& (q.c_from == suplID)
                             //&& (q.c_type == "05")
                             //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                             select q).Take(1).SingleOrDefault();
                  }

                  if (rnh == null)
                  {
                      rnh = new LG_RNH()
                      {
                          c_gdg = gdgTujuan,
                          c_rnno = refID,
                          d_rndate = date,
                          c_type = "05",
                          l_float = false,
                          c_dono = sjDO,
                          d_dodate = date,
                          c_from = suplID,
                          n_bea = 0,
                          l_print = false,
                          l_status = false,
                          c_entry = nipEntry,
                          c_update = nipEntry,
                          d_entry = date,
                          d_update = date,
                          v_ket = (string.IsNullOrEmpty(sjh.v_ket) ? string.Empty : sjh.v_ket.Trim()),
                          l_delete = false,
                          v_ket_mark = string.Empty
                      };

                      db.LG_RNHs.InsertOnSubmit(rnh);

                      listRNH.Add(rnh);
                  }

                  #region Detail

                  rnd1 = listRND1.Find(delegate(LG_RND1 rnd)
                  {
                      return sjd2.c_iteno.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                        sjd2.c_batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                  });

                  if (rnd1 == null)
                  {
                      rnd1 = (from q in db.LG_RND1s
                              where (q.c_gdg == gdgTujuan) && (q.c_rnno == refID)
                              && (q.c_iteno == sjd2.c_iteno) && (q.c_batch == sjd2.c_batch)
                              select q).Take(1).SingleOrDefault();
                  }

                  if (rnd1 == null)
                  {
                      if (Commons.CheckAndProcessBatch(db, sjd2.c_iteno, sjd2.c_batch, date, nipEntry) > 0)
                      {
                          rnd1 = new LG_RND1()
                          {
                              c_gdg = gdgTujuan,
                              c_rnno = refID,
                              c_iteno = sjd2.c_iteno,
                              c_batch = sjd2.c_batch,
                              //n_floqty = 0,
                              n_gqty = 0,
                              n_gsisa = 0,
                              n_bqty = 0,
                              n_bsisa = sjd2.n_bsisa
                          };

                          db.LG_RND1s.InsertOnSubmit(rnd1);

                          listRND1.Add(rnd1);
                      }
                  }
                  else
                  {
                      //rnd1.n_gqty += nQtyGood;
                      //rnd1.n_gsisa += nQtyRelocBad;
                      //rnd1.n_bqty += nQtyBad;
                      rnd1.n_bsisa += sjd2.n_bsisa;
                  }

                  #endregion

                  //sjd2.n_gsisa -= 0;
                  sjd2.n_bsisa -= sjd2.n_bsisa;

                  totalDetails++;

                  #endregion
              }
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
          rpe = ResponseParser.ResponseParserEnum.IsFailed;

          db.Transaction.Rollback();
        }

      }
      catch (Exception ex)
      {
        if (db.Transaction != null)
        {
          db.Transaction.Rollback();
        }

        result = string.Format("ScmsSoaLibrary.Bussiness.Transfer:TransferGudangRepack - {0}", ex.Message);

        Logger.WriteLine(result, true);
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

    public string ReceiveNoteGudang(ScmsSoaLibrary.Parser.Class.ReceiveNoteGudangStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false;
      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      ScmsSoaLibrary.Parser.Class.ReceiveNoteGudangStructureField field = null;
      string nipEntry = null;
      string sjID = null,
        refID = null,
        suplID = null,
        pinID = null,
        itemId = null, 
        batchCode = null,
        sjDO = null,
        ketSJ = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now,
        dateSJ = DateTime.MinValue;

      List<LG_RNH> listRnh = null;
      List<LG_RND1> listRnd1 = null;
      List<LG_ComboH> listComboh = null;

      List<LG_SJD2> listSjd2 = null;

      LG_ComboH comboh = null;
      LG_RNH rnh = null;
      LG_RND1 rnd1 = null;
      //LG_RND2 rnd2 = null;

      LG_SJH sjh = null;
      //LG_SJD1 sjd1 = null;
      LG_SJD2 sjd2 = null;
      //LG_RND3 rnd3 = null;
      //LG_RND4 rnd4 = null;
      //LG_RND5 rnd5 = null;

      //LG_POD1 pod1 = null;

      int nLoop = 0;

      decimal nQtyGood = 0,
        nQtyBad = 0;

      //SJ_RN_FRMT_DATA srfd = null;
      //ItemInfo itmInfo = null;
      List<string> lstTemp = null;
      //List<ItemInfo> lstItemInfo = null;
      Dictionary<string, string> dicItemSupl = null;
      
      nipEntry = (structure.Fields.Entry ?? string.Empty);

      IDictionary<string, string> dic = null;

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }

      int totalDetails = 0,
        iTmp = 0;

      char gdg = (string.IsNullOrEmpty(structure.Fields.Gudang) ? char.MinValue : structure.Fields.Gudang[0]),
        gdgAsal = char.MinValue;
      //char gdgTarget = char.MinValue;

      string typeRNData = structure.Fields.TypeRN;

      if (gdg == char.MinValue)
      {
        result = "Gudang tidak boleh kosong.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        db.Transaction.Rollback();

        goto endLogic;
      }

      sjID = (structure.Fields.SuratID ?? string.Empty);

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Processing", StringComparison.OrdinalIgnoreCase))
        {
          #region Process

          if (string.IsNullOrEmpty(sjID))
          {
            result = "Nomor Surat dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          sjh = (from q in db.LG_SJHs
                 where q.c_sjno == sjID && q.c_gdg2 == gdg
                 select q).Take(1).SingleOrDefault();

          if (sjh == null)
          {
            result = "Nomor Surat tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (sjh.l_delete.HasValue && sjh.l_delete.Value)
          {
            result = "Tidak dapat mengubah Nomor Surat yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (sjh.l_status.HasValue && sjh.l_status.Value)
          {
            result = "Tidak dapat memproses Nomor Surat yang sudah terproses.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          //else if (Commons.IsClosingLogistik(db, sjh.d_sjdate))
          //{
          //  result = "Receive note tidak dapat di buat, karena sudah closing.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}

          pinID = (string.IsNullOrEmpty(sjh.c_pin) ? string.Empty : sjh.c_pin.Trim());

          if (!pinID.Equals(structure.Fields.PIN, StringComparison.OrdinalIgnoreCase))
          {
            result = "Kesalahan pada PIN.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          gdgAsal = sjh.c_gdg;

          sjh.l_status = true;

          sjh.c_update = nipEntry;
          sjh.d_update = date;

          ketSJ = (string.IsNullOrEmpty(sjh.v_ket) ? string.Empty : sjh.v_ket.Trim());

          dateSJ = (sjh.d_sjdate.HasValue ? sjh.d_sjdate.Value : date);

          listSjd2 = (from q in db.LG_SJHs
                      join q2 in db.LG_SJD2s on new { q.c_gdg, q.c_sjno } equals new { q2.c_gdg, q2.c_sjno }
                      where (q.c_sjno == sjID) && (q.c_gdg2 == gdg)
                        && (q.l_status == false) && (q.l_confirm == true) && (q.l_print == true)
                        && (q.c_pin.Trim().ToLower() == structure.Fields.PIN.Trim().ToLower())
                        && ((q2.n_bsisa > 0) || (q2.n_gsisa > 0))
                        && ((q2.n_gqty > 0) || (q2.n_bqty > 0))
                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      select q2).Distinct().ToList();

          listComboh = new List<LG_ComboH>();
          listRnh = new List<LG_RNH>();
          listRnd1 = new List<LG_RND1>();
          
          lstTemp = listSjd2
            .Where(t => !string.IsNullOrEmpty(t.c_iteno))
            .GroupBy(t => t.c_iteno).Select(x => x.Key).ToList();

          dicItemSupl = (from q in db.FA_MasItms
                         where lstTemp.Contains(q.c_iteno)
                         select new ItemInfo()
                          {
                            Item = q.c_iteno,
                            Suplier = q.c_nosup,
                          }).ToDictionary(x => x.Item, x => x.Suplier);

          for (nLoop = 0; nLoop < listSjd2.Count; nLoop++)
          {
            sjd2 = listSjd2[nLoop];

            itemId = (sjd2.c_iteno ?? string.Empty);
            batchCode = (string.IsNullOrEmpty(sjd2.c_batch) ? string.Empty : sjd2.c_batch.Trim());

            refID = (sjd2.c_rnno ?? string.Empty);
            
            nQtyGood = (sjd2.n_gsisa.HasValue ? sjd2.n_gsisa.Value : 0);
            nQtyBad = (sjd2.n_bsisa.HasValue ? sjd2.n_bsisa.Value : 0);

            suplID = dicItemSupl.GetValueParser<string, string, string>(itemId, "00000");

            sjDO = string.Concat("SJXX", gdg.ToString(), suplID);

            if (refID.StartsWith("CB", StringComparison.OrdinalIgnoreCase))
            {
                #region Combo

                comboh = listComboh.Find(delegate(LG_ComboH cmb)
                {
                    return refID.Equals(cmb.c_combono, StringComparison.OrdinalIgnoreCase) &&
                      itemId.Equals((string.IsNullOrEmpty(cmb.c_iteno) ? string.Empty : cmb.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                      batchCode.Equals((string.IsNullOrEmpty(cmb.c_batch) ? string.Empty : cmb.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                });

                if (comboh == null)
                {
                    comboh = (from q in db.LG_ComboHs
                              where (q.c_gdg == gdg) && (q.c_combono == refID)
                                && (q.c_iteno == itemId) && (q.c_batch == batchCode)
                              //&& (q.c_type == "05")
                              //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                              select q).Take(1).SingleOrDefault();
                }

                if (comboh == null)
                {
                    if (Commons.CheckAndProcessBatch(db, itemId, batchCode, date, nipEntry) > 0)
                    {
                        comboh = new LG_ComboH()
                        {
                            c_gdg = gdg,
                            c_type = "05",
                            c_combono = refID,
                            d_combodate = date,
                            c_memono = sjDO,
                            c_iteno = itemId,
                            c_batch = batchCode,
                            n_acc = 0,
                            n_gqty = nQtyGood,
                            n_gsisa = nQtyGood,
                            n_bqty = nQtyBad,
                            n_bsisa = nQtyBad,
                            v_ket = ketSJ,
                            l_confirm = true,
                            c_entry = nipEntry,
                            d_entry = date,
                            c_update = nipEntry,
                            d_update = date,
                            l_delete = false,
                            v_ket_mark = string.Empty
                        };

                        db.LG_ComboHs.InsertOnSubmit(comboh);

                        listComboh.Add(comboh);
                    }
                }
                else
                {
                    //comboh.n_gqty += nQtyGood;
                    comboh.n_gsisa += nQtyGood;
                    //comboh.n_bqty += nQtyBad;
                    comboh.n_bsisa += nQtyBad;
                }

                totalDetails++;

                sjd2.n_gsisa -= nQtyGood;
                sjd2.n_bsisa -= nQtyBad;

                #endregion
            }
            else if (refID.StartsWith("RN", StringComparison.OrdinalIgnoreCase) || refID.StartsWith("RR", StringComparison.OrdinalIgnoreCase))
            {
                #region RN

                rnh = listRnh.Find(delegate(LG_RNH rn)
                {
                    return refID.Equals(rn.c_rnno, StringComparison.OrdinalIgnoreCase) &&
                      suplID.Equals((string.IsNullOrEmpty(rn.c_from) ? string.Empty : rn.c_from.Trim()), StringComparison.OrdinalIgnoreCase);
                });

                if (rnh == null)
                {
                    rnh = (from q in db.LG_RNHs
                           where (q.c_gdg == gdg) && (q.c_rnno == refID)
                           //&& (q.c_from == suplID)
                           //&& (q.c_type == "05")
                           //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                           select q).Take(1).SingleOrDefault();
                }

                if (rnh == null)
                {
                    rnh = new LG_RNH()
                    {
                        c_gdg = gdg,
                        c_rnno = refID,
                        d_rndate = date,
                        c_type = "05",
                        l_float = false,
                        c_dono = sjDO,
                        d_dodate = dateSJ,
                        c_from = suplID,
                        n_bea = 0,
                        l_print = false,
                        l_status = false,
                        c_entry = nipEntry,
                        c_update = nipEntry,
                        d_entry = date,
                        d_update = date,
                        v_ket = (string.IsNullOrEmpty(sjh.v_ket) ? string.Empty : sjh.v_ket.Trim()),
                        l_delete = false,
                        v_ket_mark = string.Empty
                    };

                    db.LG_RNHs.InsertOnSubmit(rnh);

                    listRnh.Add(rnh);
                }

                #region Detail

                rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
                {
                    return itemId.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                      batchCode.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                });

                if (rnd1 == null)
                {
                    rnd1 = (from q in db.LG_RND1s
                            where (q.c_gdg == gdg) && (q.c_rnno == refID)
                            && (q.c_iteno == itemId) && (q.c_batch == batchCode)
                            select q).Take(1).SingleOrDefault();
                }

                if (rnd1 == null)
                {
                    if (Commons.CheckAndProcessBatch(db, itemId, batchCode, date, nipEntry) > 0)
                    {
                        rnd1 = new LG_RND1()
                        {
                            c_gdg = gdg,
                            c_rnno = refID,
                            c_iteno = itemId,
                            c_batch = batchCode,
                            //n_floqty = 0,
                            n_gqty = nQtyGood,
                            n_gsisa = nQtyGood,
                            n_bqty = nQtyBad,
                            n_bsisa = nQtyBad
                        };

                        db.LG_RND1s.InsertOnSubmit(rnd1);

                        listRnd1.Add(rnd1);
                    }
                }
                else
                {
                    //rnd1.n_gqty += nQtyGood;
                    rnd1.n_gsisa += nQtyGood;
                    //rnd1.n_bqty += nQtyBad;
                    rnd1.n_bsisa += nQtyBad;
                }

                #endregion

                totalDetails++;

                sjd2.n_gsisa -= nQtyGood;
                sjd2.n_bsisa -= nQtyBad;

                #endregion
            }
            else if (refID.StartsWith("RR", StringComparison.OrdinalIgnoreCase))
            {
                #region RR

                rnh = listRnh.Find(delegate(LG_RNH rn)
                {
                    return refID.Equals(rn.c_rnno, StringComparison.OrdinalIgnoreCase) &&
                      suplID.Equals((string.IsNullOrEmpty(rn.c_from) ? string.Empty : rn.c_from.Trim()), StringComparison.OrdinalIgnoreCase);
                });

                if (rnh == null)
                {
                    rnh = (from q in db.LG_RNHs
                           where (q.c_gdg == gdg) && (q.c_rnno == refID)
                           //&& (q.c_from == suplID)
                           //&& (q.c_type == "05")
                           //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                           select q).Take(1).SingleOrDefault();
                }

                if (rnh == null)
                {
                    rnh = new LG_RNH()
                    {
                        c_gdg = gdg,
                        c_rnno = refID,
                        d_rndate = date,
                        c_type = "05",
                        l_float = false,
                        c_dono = sjDO,
                        d_dodate = dateSJ,
                        c_from = suplID,
                        n_bea = 0,
                        l_print = false,
                        l_status = false,
                        c_entry = nipEntry,
                        c_update = nipEntry,
                        d_entry = date,
                        d_update = date,
                        v_ket = (string.IsNullOrEmpty(sjh.v_ket) ? string.Empty : sjh.v_ket.Trim()),
                        l_delete = false,
                        v_ket_mark = string.Empty
                    };

                    db.LG_RNHs.InsertOnSubmit(rnh);

                    listRnh.Add(rnh);
                }

                #region Detail

                rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
                {
                    return itemId.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                      batchCode.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                });

                if (rnd1 == null)
                {
                    rnd1 = (from q in db.LG_RND1s
                            where (q.c_gdg == gdg) && (q.c_rnno == refID)
                            && (q.c_iteno == itemId) && (q.c_batch == batchCode)
                            select q).Take(1).SingleOrDefault();
                }

                if (rnd1 == null)
                {
                    if (Commons.CheckAndProcessBatch(db, itemId, batchCode, date, nipEntry) > 0)
                    {
                        rnd1 = new LG_RND1()
                        {
                            c_gdg = gdg,
                            c_rnno = refID,
                            c_iteno = itemId,
                            c_batch = batchCode,
                            //n_floqty = 0,
                            n_gqty = nQtyGood,
                            n_gsisa = nQtyGood,
                            n_bqty = nQtyBad,
                            n_bsisa = nQtyBad
                        };

                        db.LG_RND1s.InsertOnSubmit(rnd1);

                        listRnd1.Add(rnd1);
                    }
                }
                else
                {
                    //rnd1.n_gqty += nQtyGood;
                    rnd1.n_gsisa += nQtyGood;
                    //rnd1.n_bqty += nQtyBad;
                    rnd1.n_bsisa += nQtyBad;
                }

                #endregion

                totalDetails++;

                sjd2.n_gsisa -= nQtyGood;
                sjd2.n_bsisa -= nQtyBad;

                #endregion
            }
          }

          #region Old Coded

          //if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          //{
          //  dateSJ = (sjh.d_sjdate.HasValue ? sjh.d_sjdate.Value : date);

          //  listRnd1 = new List<LG_RND1>();
          //  listRnh = new List<LG_RNH>();
          //  listComboh = new List<LG_ComboH>();
          //  listSjd2 = new List<LG_SJD2>();

          //  List<SJ_RN_FRMT_DATA> listSJData = (from q in db.LG_SJHs
          //                                      join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
          //                                      join q2 in db.LG_SJD2s on new { q.c_gdg, q.c_sjno } equals new { q2.c_gdg, q2.c_sjno }
          //                                      join q3 in db.FA_MasItms on q1.c_iteno equals q3.c_iteno
          //                                      where (q.c_sjno == sjID) && (q.c_gdg2 == gdg)
          //                                        && (q.l_status == false) && (q.l_confirm == true) && (q.l_print == true)
          //                                        && (q.c_pin.Trim().ToLower() == structure.Fields.PIN.Trim().ToLower())
          //                                        && ((q2.n_bsisa > 0) || (q2.n_gsisa > 0))
          //                                        && ((q2.n_gqty > 0) || (q2.n_bqty > 0))
          //                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
          //                                      select new SJ_RN_FRMT_DATA()
          //                                      {
          //                                        c_gdg = (q.c_gdg2.HasValue ? q.c_gdg2.Value : char.MinValue),
          //                                        c_iteno = q2.c_iteno,
          //                                        v_item_desc = q3.v_itnam,
          //                                        c_batch = q2.c_batch,
          //                                        n_bqty = (q2.n_bsisa.HasValue ? q2.n_bsisa.Value : 0),
          //                                        n_gqty = (q2.n_gsisa.HasValue ? q2.n_gsisa.Value : 0),
          //                                        c_addtno = (q2.c_spgno == null ? string.Empty : q2.c_spgno.Trim()),
          //                                        c_refno = (q2.c_rnno == null ? string.Empty : q2.c_rnno.Trim())
          //                                      }).Distinct().ToList();

          //  if (listSJData.Count > 0)
          //  {
          //    lstTemp = listSJData.Where(t => (!string.IsNullOrEmpty(t.c_iteno))).GroupBy(x => x.c_iteno).Select(y => y.Key.ToString().Trim()).ToList();

          //    lstItemInfo = (from q in db.FA_MasItms
          //                   where lstTemp.Contains(q.c_iteno)
          //                   select new ItemInfo()
          //                   {
          //                     Item = (q.c_iteno == null ? string.Empty : q.c_iteno.Trim()),
          //                     Suplier = (q.c_nosup == null ? string.Empty : q.c_nosup.Trim()),
          //                     IsCombo = (q.l_combo.HasValue ? q.l_combo.Value : false)
          //                   }).Distinct().ToList();

          //    for (nLoop = 0; nLoop < listSJData.Count; nLoop++)
          //    {
          //      srfd = listSJData[nLoop];

          //      itemId = (string.IsNullOrEmpty(srfd.c_iteno) ? string.Empty : srfd.c_iteno.Trim());
          //      batchCode = (string.IsNullOrEmpty(srfd.c_batch) ? string.Empty : srfd.c_batch.Trim());

          //      if (string.IsNullOrEmpty(itemId) || string.IsNullOrEmpty(batchCode))
          //      {
          //        continue;
          //      }

          //      itmInfo = lstItemInfo.Find(delegate(ItemInfo ii)
          //      {
          //        return itemId.Equals(ii.Item, StringComparison.OrdinalIgnoreCase);
          //      });

          //      if ((itmInfo != null) && (!string.IsNullOrEmpty(itmInfo.Suplier)) && (!string.IsNullOrEmpty(itmInfo.Item)))
          //      {
          //        #region Cek SJD

          //        sjd2 = listSjd2.Find(delegate(LG_SJD2 sjd)
          //        {
          //          return itemId.Equals((string.IsNullOrEmpty(sjd.c_iteno) ? string.Empty : sjd2.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
          //            batchCode.Equals((string.IsNullOrEmpty(sjd.c_batch) ? string.Empty : sjd2.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
          //            (((sjd.n_gsisa.HasValue ? sjd.n_gsisa.Value : 0) > 0) || ((sjd.n_bsisa.HasValue ? sjd.n_bsisa.Value : 0) > 0));
          //        });

          //        if (sjd2 == null)
          //        {
          //          sjd2 = (from q in db.LG_SJD2s
          //                  where (q.c_gdg == gdgAsal) && (q.c_sjno == sjID)
          //                    && (itemId == q.c_iteno) && (batchCode == q.c_batch)
          //                    && ((q.n_gsisa > 0) || (q.n_bsisa > 0))
          //                  select q).Take(1).SingleOrDefault();

          //          if (sjd2 != null)
          //          {
          //            if (listSjd2.Exists(delegate(LG_SJD2 sjd)
          //            {
          //              return itemId.Equals((string.IsNullOrEmpty(sjd2.c_iteno) ? string.Empty : sjd2.c_iteno), StringComparison.OrdinalIgnoreCase) &&
          //                batchCode.Equals((string.IsNullOrEmpty(sjd2.c_batch) ? string.Empty : sjd2.c_batch), StringComparison.OrdinalIgnoreCase) &&
          //                (string.IsNullOrEmpty(sjd2.c_spgno) ? string.Empty : sjd2.c_spgno.Trim()).Equals((string.IsNullOrEmpty(sjd.c_spgno) ? string.Empty : sjd.c_spgno.Trim()), StringComparison.OrdinalIgnoreCase) &&
          //                (string.IsNullOrEmpty(sjd2.c_rnno) ? string.Empty : sjd2.c_rnno.Trim()).Equals((string.IsNullOrEmpty(sjd.c_rnno) ? string.Empty : sjd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
          //            }))
          //            {
          //              sjd2 = null;
          //            }
          //            else
          //            {
          //              listSjd2.Add(sjd2);
          //            }
          //          }
          //        }

          //        if (sjd2 == null)
          //        {
          //          continue;
          //        }
          //        else
          //        {
          //          field = structure.Fields.Field.Where(x => (itemId == x.Item) && (batchCode == x.Batch) && ((x.Quantity > 0) || (x.QuantityBad > 0))).Take(1).SingleOrDefault();
          //          if (field == null)
          //          {
          //            continue;
          //          }
          //        }

          //        nQtyAllocGood = (sjd2.n_gsisa.HasValue ? sjd2.n_gsisa.Value : 0);
          //        nQtyAllocBad = (sjd2.n_bsisa.HasValue ? sjd2.n_bsisa.Value : 0);

          //        if ((field.Quantity > 0) && (nQtyAllocGood > 0))
          //        {
          //          nQtyGood = ((field.Quantity > nQtyAllocGood) ? (field.Quantity - (field.Quantity - nQtyAllocGood)) : (nQtyAllocGood - (nQtyAllocGood - field.Quantity)));
          //        }
          //        else
          //        {
          //          nQtyGood = 0;
          //        }
          //        if ((field.QuantityBad > 0) && (nQtyAllocBad > 0))
          //        {
          //          nQtyBad = ((field.Quantity > nQtyAllocBad) ? (field.QuantityBad - (field.QuantityBad - nQtyAllocBad)) : (nQtyAllocBad - (nQtyAllocBad - field.QuantityBad)));
          //        }
          //        else
          //        {
          //          nQtyBad = 0;
          //        }

          //        if ((nQtyGood <= 0) && (nQtyBad <= 0))
          //        {
          //          continue;
          //        }

          //        field.Quantity -= nQtyGood;
          //        field.QuantityBad -= nQtyBad;

          //        sjd2.n_gsisa -= nQtyGood;
          //        sjd2.n_bsisa -= nQtyBad;

          //        #endregion

          //        if (srfd.c_refno.StartsWith("CB"))
          //        {
          //          #region Combo

          //          comboh = listComboh.Find(delegate(LG_ComboH cmb)
          //          {
          //            return srfd.c_refno.Equals(cmb.c_combono, StringComparison.OrdinalIgnoreCase) &&
          //              srfd.c_iteno.Equals(cmb.c_iteno, StringComparison.OrdinalIgnoreCase) &&
          //              srfd.c_batch.Equals(cmb.c_batch, StringComparison.OrdinalIgnoreCase);
          //          });

          //          if (comboh == null)
          //          {
          //            comboh = (from q in db.LG_ComboHs
          //                      where (q.c_gdg == gdg) && (q.c_iteno == srfd.c_iteno)
          //                        && (q.c_batch == srfd.c_batch) && (q.c_type == "05")
          //                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
          //                      select q).Take(1).SingleOrDefault();
          //          }

          //          if (comboh == null)
          //          {
          //            listComboh.Add(new LG_ComboH()
          //            {
          //              c_gdg = gdg,
          //              c_type = "05",
          //              c_combono = srfd.c_refno,
          //              d_combodate = date,
          //              c_memono = string.Empty,
          //              c_iteno = srfd.c_iteno,
          //              c_batch = srfd.c_batch,
          //              n_acc = (srfd.n_bqty + srfd.n_gqty),
          //              n_gqty = srfd.n_gqty,
          //              n_bqty = srfd.n_bqty,
          //              n_gsisa = srfd.n_gqty,
          //              n_bsisa = srfd.n_bqty,
          //              v_ket = ketSJ,
          //              l_confirm = true,
          //              c_entry = nipEntry,
          //              d_entry = date,
          //              c_update = nipEntry,
          //              d_update = date,
          //              l_delete = false,
          //              v_ket_mark = string.Empty
          //            });
          //          }
          //          else
          //          {
          //            comboh.n_acc += (srfd.n_gqty + srfd.n_bqty);
          //            comboh.n_gqty += srfd.n_gqty;
          //            comboh.n_gsisa += srfd.n_gqty;
          //            comboh.n_bqty += srfd.n_bqty;
          //            comboh.n_bsisa += srfd.n_bqty;
          //          }

          //          totalDetails++;

          //          #endregion
          //        }
          //        else
          //        {
          //          #region RN

          //          #region RNH

          //          //rnh = listRnh.Find(delegate(LG_RNH rn)
          //          //{
          //          //  return itmInfo.Suplier.Trim().Equals(rn.c_from, StringComparison.OrdinalIgnoreCase) &&
          //          //  srfd.c_refno.Trim().Equals(rn.c_rnno, StringComparison.OrdinalIgnoreCase);
          //          //});

          //          rnh = (from q in db.LG_RNHs
          //                 where (q.c_from == itmInfo.Suplier.Trim())
          //                 && (q.c_rnno == srfd.c_refno.Trim())
          //                 && (q.c_gdg == gdg) && (q.c_type == "05")
          //                 select q).Take(1).SingleOrDefault();

          //          if (rnh == null)
          //          {
          //            //refID = Commons.GenerateNumbering<LG_RNH>(db, "RN", '3', "03", date, "c_rnno");

          //            listRnh.Add(new LG_RNH()
          //            {
          //              c_gdg = gdg,
          //              c_rnno = srfd.c_refno,
          //              d_rndate = date,
          //              c_type = "05",
          //              l_float = false,
          //              c_dono = sjID,
          //              d_dodate = dateSJ,
          //              c_from = itmInfo.Suplier,
          //              n_bea = 0,
          //              l_print = false,
          //              l_status = false,
          //              c_entry = nipEntry,
          //              c_update = nipEntry,
          //              d_entry = date,
          //              d_update = date,
          //              v_ket = (string.IsNullOrEmpty(sjh.v_ket) ? string.Empty : sjh.v_ket.Trim()),
          //              l_delete = false,
          //              v_ket_mark = string.Empty
          //            });
          //          }

          //          #endregion

          //          #region Detail

          //          if (!string.IsNullOrEmpty(srfd.c_refno))
          //          {
          //            //rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
          //            //{
          //            //  return srfd.c_iteno.Trim().Equals(rnd.c_iteno, StringComparison.OrdinalIgnoreCase) &&
          //            //    srfd.c_batch.Trim().Equals(rnd.c_batch, StringComparison.OrdinalIgnoreCase);
          //            //});

          //            rnd1 = (from q in db.LG_RND1s
          //                    where q.c_gdg == gdg
          //                    && q.c_rnno == srfd.c_refno.Trim()
          //                    && q.c_batch.Trim() == srfd.c_batch.Trim()
          //                    && q.c_iteno == srfd.c_iteno
          //                    select q).Take(1).SingleOrDefault();

          //            if (rnd1 == null)
          //            {
          //              if (Commons.CheckAndProcessBatch(db, srfd.c_iteno, srfd.c_batch, date, nipEntry) > 0)
          //              {
          //                listRnd1.Add(new LG_RND1()
          //                {
          //                  c_gdg = gdg,
          //                  c_rnno = srfd.c_refno,
          //                  c_iteno = srfd.c_iteno,
          //                  c_batch = srfd.c_batch,
          //                  n_floqty = 0,
          //                  n_gqty = srfd.n_gqty,
          //                  n_gsisa = srfd.n_gqty,
          //                  n_bqty = srfd.n_bqty,
          //                  n_bsisa = srfd.n_bqty
          //                });
          //              }
          //            }
          //            else
          //            {
          //              rnd1.n_gqty += rnd1.n_gsisa += srfd.n_gqty;
          //              rnd1.n_bqty += rnd1.n_bsisa += srfd.n_bqty;
          //            }

          //            totalDetails++;
          //          }

          //          #endregion

          //          #endregion
          //        }
          //      }
          //    }

          //    lstTemp.Clear();
          //    lstItemInfo.Clear();
          //  }

          //  listSJData.Clear();

          //  listSjd2.Clear();

          //  #region Old Coded

          //  //for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
          //  //{
          //  //  field = structure.Fields.Field[nLoop];

          //  //  if(!string.IsNullOrEmpty(field.ReferenceID))
          //  //  {
          //  //    tmpNumbering = field.ReferenceID.Substring(0, 2);

          //  //    if (tmpNumbering.Equals("CB", StringComparison.OrdinalIgnoreCase))
          //  //    {
          //  //      #region Combo

          //  //      //gdgTarget = (string.IsNullOrEmpty(field.GudangID) ? char.MinValue : field.GudangID[0]);

          //  //      comboh = (from q in db.LG_ComboHs
          //  //                where q.c_gdg == gdg && q.c_combono == field.ReferenceID &&
          //  //                  q.c_iteno == field.Item && q.c_batch == field.Batch &&
          //  //                  q.c_type == typeRNData
          //  //                select q).Take(1).SingleOrDefault();

          //  //      if (comboh == null)
          //  //      {
          //  //        listComboh.Add(new LG_ComboH()
          //  //        {
          //  //          c_batch = field.Batch,
          //  //          c_combono = field.ReferenceID,
          //  //          c_entry = nipEntry,
          //  //          c_gdg = gdg,
          //  //          c_iteno = field.Item,
          //  //          c_memono = field.ReferenceID,
          //  //          c_type = typeRNData,
          //  //          c_update = nipEntry,
          //  //          d_combodate = field.ReferenceDateFormated,
          //  //          d_entry = date,
          //  //          d_update = date,
          //  //          l_confirm = true,
          //  //          n_acc = field.Acceptance,
          //  //          //n_bqty = nQtyBad,
          //  //          //n_bsisa = nQtyBad,
          //  //          //n_gqty = nQtyGood,
          //  //          //n_gsisa = nQtyGood,
          //  //          n_bqty = field.QuantityBad,
          //  //          n_bsisa = field.QuantityBad,
          //  //          n_gqty = field.Quantity,
          //  //          n_gsisa = field.Quantity,
          //  //          v_ket = field.Keterangan
          //  //        });
          //  //      }
          //  //      else
          //  //      {
          //  //        //comboh.n_gsisa += nQtyGood;
          //  //        //comboh.n_bsisa += nQtyBad;
          //  //        comboh.n_gsisa += field.Quantity;
          //  //        comboh.n_bsisa += field.QuantityBad;
          //  //      }

          //  //      #endregion
          //  //    }
          //  //    else if (tmpNumbering.Equals("RN", StringComparison.OrdinalIgnoreCase))
          //  //    {
          //  //      #region RN

          //  //      rnh = (from q in db.LG_RNHs
          //  //             where q.c_gdg == gdg && q.c_rnno == field.ReferenceID &&
          //  //              q.c_type == typeRNData
          //  //             select q).Take(1).SingleOrDefault();

          //  //      if (rnh == null)
          //  //      {
          //  //        listRnh.Add(new LG_RNH()
          //  //        {
          //  //          c_dono = field.AdditionalID,
          //  //          c_entry = nipEntry,
          //  //          c_from = field.SuplierID,
          //  //          c_gdg = gdg,
          //  //          c_rnno = field.ReferenceID,
          //  //          c_type = typeRNData,
          //  //          c_update = nipEntry,
          //  //          d_dodate = field.AdditionalDateFormated,
          //  //          d_entry = date,
          //  //          d_rndate = field.ReferenceDateFormated,
          //  //          d_update = date,
          //  //          l_delete = null,
          //  //          l_float = field.IsFloat,
          //  //          l_print = field.IsPrint,
          //  //          l_status = field.IsStatus,
          //  //          n_bea = field.Bea,
          //  //          v_ket = field.Keterangan
          //  //        });
          //  //      }

          //  //      rnd1 = (from q in db.LG_RND1s
          //  //             //join q1 in db.LG_RNHs on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
          //  //             where q.c_gdg == gdg && q.c_rnno == field.ReferenceID &&
          //  //              //q1.c_type == typeRNData &&
          //  //              q.c_iteno == field.Item &&
          //  //              q.c_batch == field.Batch
          //  //             select q).Take(1).SingleOrDefault();

          //  //      if (rnd1 == null)
          //  //      {
          //  //        listRnd1.Add(new LG_RND1()
          //  //        {
          //  //          c_batch = field.Batch,
          //  //          c_gdg = gdg,
          //  //          c_iteno = field.Item,
          //  //          c_rnno = field.ReferenceID,
          //  //          //n_bqty = nQtyBad,
          //  //          //n_bsisa = nQtyBad,
          //  //          //n_gqty = nQtyGood,
          //  //          //n_gsisa = nQtyGood
          //  //          n_bqty = field.QuantityBad,
          //  //          n_bsisa = field.QuantityBad,
          //  //          n_gqty = field.Quantity,
          //  //          n_gsisa = field.Quantity,
          //  //        });
          //  //      }
          //  //      else
          //  //      {
          //  //        //rnd1.n_gsisa += nQtyGood;
          //  //        //rnd1.n_bsisa += nQtyBad;
          //  //        rnd1.n_gsisa += field.Quantity;
          //  //        rnd1.n_bsisa += field.QuantityBad;
          //  //      }

          //  //      #endregion
          //  //    }
          //  //  }
          //  //}

          //  #endregion

          //  if (listComboh.Count > 0)
          //  {
          //    db.LG_ComboHs.InsertAllOnSubmit(listComboh.ToArray());
          //    listComboh.Clear();
          //  }

          //  if (listRnd1.Count > 0)
          //  {
          //    db.LG_RND1s.InsertAllOnSubmit(listRnd1.ToArray());
          //    listRnd1.Clear();
          //  }

          //  if (listRnh.Count > 0)
          //  {
          //    db.LG_RNHs.InsertAllOnSubmit(listRnh.ToArray());
          //    listRnh.Clear();
          //  }
          //}

          #endregion

          dic = new Dictionary<string, string>();

          if (totalDetails > 0)
          {
            dic.Add("Tanggal", date.ToString("yyyyMMdd"));

            result = string.Format("Total {0} detail(s)", totalDetails);

            hasAnyChanges = true;
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

          rpe = ResponseParser.ResponseParserEnum.IsFailed;
        }
      }
      catch (Exception ex)
      {
        if (db.Transaction != null)
        {
          db.Transaction.Rollback();
        }

        result = string.Format("ScmsSoaLibrary.Bussiness.Pembelian:ReceiveNoteGudang - {0}", ex.Message);

        Logger.WriteLine(result, true);
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