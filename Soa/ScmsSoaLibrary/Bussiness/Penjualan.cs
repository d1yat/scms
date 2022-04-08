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
  class Penjualan
  {
    #region Internal Class

    internal class LG_PLD1_SUM_BYBATCH
    {
      public string c_plno { get; set; }
      public string c_iteno { get; set; }
      public string c_batch { get; set; }
      public decimal n_qty { get; set; }
    }

    internal class EKSP_KOLI
    {
      public string c_part { get; set; }
    }

    internal class PLClassComponent
    {
      public string RefID { get; set; }
      public string SignID { get; set; }
      public string BatchID { get; set; }
      public string Item { get; set; }
      public decimal Qty { get; set; }
      public string Supplier { get; set; }
      public decimal Box { get; set; }
    }

    internal class DOClassComponent
    {
      public string Doid { get; set; }
      public bool Status { get; set; }
    }

    #endregion

    private LG_SPH PackingListSPAdj(ORMDataContext db, ScmsSoaLibrary.Parser.Class.PackingListAutoStructureField field, ScmsSoaLibrary.Parser.Class.PackingListAutoStructure structure, DateTime date)
    {
      LG_SPH sph = null;

      #region SPH

      //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "SP");

      string spID = Commons.GenerateNumbering<LG_SPH>(db, "SP", '3', "07", date, "c_spno");

      sph = new LG_SPH()
      {
        c_cusno = structure.Fields.Customer,
        c_entry = structure.Fields.Entry,
        c_sp = "SPADJ",
        c_spno = spID,
        c_type = "04",
        c_update = structure.Fields.Entry,
        d_entry = DateTime.Now,
        d_spdate = DateTime.Now,
        d_spinsert = DateTime.Now,
        d_update = DateTime.Now,
        l_cek = false,
        l_print = false,
        v_ket = "Adjusment Auto"
      };

      //db.LG_SPHs.InsertOnSubmit(sph);

      //db.SubmitChanges();

      //spID = (from q in db.LG_SPHs
      //        where q.v_ket == tmpNumbering
      //        select q.c_spno).Take(1).SingleOrDefault();

      #endregion

      return sph;
    }

    private LG_DOH PackingListDO(ORMDataContext db, char gdg, string plID, ScmsSoaLibrary.Parser.Class.PackingListAutoStructure structure, DateTime date)
    {
      LG_DOH doh = null;
      
      #region DOH

      //tmpNumberingDO = Functionals.GeneratedRandomUniqueId(50, "DOA");

      string doID = Commons.GenerateNumbering<LG_DOH>(db, "DO", '3', "09", date, "c_dono");

      doh = new LG_DOH()
      {
        c_dono = doID,
        d_dodate = DateTime.Now,
        c_gdg = gdg,
        c_type = "01",
        c_cusno = structure.Fields.Customer,
        c_plno = plID,
        c_via = structure.Fields.Via,
        v_ket = "Auto DO",
        c_pin = Functionals.GeneratedRandomPinId(10, string.Empty),
        l_status = false,
        l_print = false,
        l_send = false,
        c_entry = structure.Fields.Entry,
        d_entry = DateTime.Now,
        c_update = structure.Fields.Entry,
        d_update = DateTime.Now,
        l_delete = false
      };

      //db.LG_DOHs.InsertOnSubmit(doh);

      //db.SubmitChanges();

      //doh = (from q in db.LG_DOHs
      //       where q.v_ket == tmpNumberingDO
      //       select q).Take(1).SingleOrDefault();

      //doID = doh.c_dono;

      #endregion

      return doh;
    }

    public string PackingList(ScmsSoaLibrary.Parser.Class.PackingListStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false,
        isConfirmMode = false;

      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      LG_PLH plh = null;

      LG_SPD1 spd1 = null;

      LG_RND1 rnd1 = null;

      #region New Stock Indra 20180305FM

      LG_DAILY_STOCK_v2 daily2 = null;
      LG_MOVEMENT_STOCK_v2 movement2 = null;

      #endregion

      LG_ComboH combo = null;

      ScmsSoaLibrary.Parser.Class.PackingListStructureField field = null;
      string nipEntry = null;
      string plID = null,
        refID = null,
        sItemAndBatch = null;

      List<string> lstBatch = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      sysnoNumber sysNumAut = null;

      //decimal? spQty = 0;
      decimal spQtyReloc = 0,
        rnQty = 0,
        spAlloc = 0,
        spSisa = 0,
        totalCurrentStock = 0,
        nQtyTemp = 0,
        nQtySisa = 0,
        iNumberAuth = 0;

      PLClassComponent spac = null;
      SCMS_BASPBD baspbd = null;
      List<PLClassComponent> listSPAC = null,
        listSPACTemp = null;

      List<LG_PLD1> listPld1 = null;
      List<LG_PLD2> listPld2 = null,
        listPld2Copy = null;
      List<LG_PLD3> listPld3 = null;

      List<string> listRN = null;
      //List<LG_RND1> listResRND1 = null;

      Dictionary<string, List<PLClassComponent>> dicItemStock = null;

      char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '1' : structure.Fields.Gudang[0]);

      LG_PLD1 pld1 = null;
      LG_PLD2 pld2 = null;

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

      int totalDetails = 0,
          totalDelete = 0;
      //bool isConfirmed = false;

      plID = (structure.Fields.PackingListID ?? string.Empty);

      isConfirmMode = structure.Method.Equals("ModifyConfirm", StringComparison.OrdinalIgnoreCase);

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          #region Validasi Indra 20171120

          if (!string.IsNullOrEmpty(plID))
          {
            result = "Nomor Packing List harus kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Suplier))
          {
            result = "Nama pemasok dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Customer))
          {
            result = "Nama cabang dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Gudang))
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
            result = "Packing List tidak dapat disimpan, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          #endregion

          //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "PL");

          Constant.TRANSID = (from q in db.LG_PLHs
                            where q.d_pldate.Value.Month == date.Date.Month
                            select q.c_plno).Max();

          Constant.Gudang = gudang;


          plID = Commons.GenerateNumbering<LG_PLH>(db, "PL", '3', "08", date, "c_plno");

          #region Auto Number Gudang

          
          //decimal iNumSys = sysNumAut.i_number.Value;
          //string sNum = sysNumAut.i_number.Value.ToString("00000");
          //int iGudang = int.Parse(Constant.Gudang.ToString());
          //string sGudang = iGudang.ToString("00");

          //string autNumber = string.Concat("PL", date.Year.ToString().Substring(2, 2), date.Month.ToString("00"), sGudang, sNum);

          #endregion


          plh = new LG_PLH()
          {
            c_plno = plID,
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
            v_ket = structure.Fields.Keterangan,
            c_type_cat = structure.Fields.TypeKategori,
            c_type_lat = structure.Fields.Lantai, 
            l_delete = false,
            c_plnum = Constant.NUMBERID_GUDANG,
            c_baspbno = structure.Fields.BaspbNo,
            c_kddivpri = (structure.Fields.DivPriID == " " ? null : structure.Fields.DivPriID),
          };

          db.LG_PLHs.InsertOnSubmit(plh);

          #region Old Coded

          //db.SubmitChanges();

          //plh = (from q in db.LG_PLHs
          //       where q.v_ket == tmpNumbering
          //       select q).Take(1).SingleOrDefault();

          //if (!string.IsNullOrEmpty(plID))
          //{
          //  result = "Nomor Packing List tidak dapat di raih.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}
          //else 
          //if (plh.c_plno.Equals("XXXXXXXXXX"))
          //{
          //  result = "Trigger Packing List tidak aktif.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}

          //plh.v_ket = structure.Fields.Keterangan;

          //plID = plh.c_plno;

          #endregion

          #region Insert Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            
            listRN = new List<string>();
            listPld1 = new List<LG_PLD1>();
            listPld2 = new List<LG_PLD2>();
            dicItemStock = new Dictionary<string, List<PLClassComponent>>(StringComparer.OrdinalIgnoreCase);

            //lstBatch = new List<string>();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0))
              {
                #region Cek SP

                spd1 = (from q in db.LG_SPD1s
                        join q1 in
                          (from sq in db.LG_ORHs
                           join sq1 in db.LG_ORD2s on new { sq.c_gdg, sq.c_orno } equals new { sq1.c_gdg, sq1.c_orno }
                           where sq.c_type == "02" && sq1.c_spno == field.NomorSP && sq1.c_iteno == field.Item
                           select new
                           {
                             sq.c_gdg,
                             sq1.c_spno,
                             sq1.c_iteno
                           }) on new { q.c_spno, q.c_iteno } equals new { q1.c_spno, q1.c_iteno } into q_1
                        from qSQ in q_1.DefaultIfEmpty()
                        where q.c_spno == field.NomorSP && q.c_iteno == field.Item //&& qORH.c_type == "02"
                        select q).Distinct().Take(1).SingleOrDefault();

                if (spd1 != null)
                {
                  spSisa = (spd1.n_sisa.HasValue ? spd1.n_sisa.Value : 0);

                  if (spSisa <= 0)
                  {
                    continue;
                  }

                  listSPAC = new List<PLClassComponent>();

                  sItemAndBatch = field.Item.Trim() + field.Batch.Trim();

                  if (dicItemStock.ContainsKey(sItemAndBatch))
                  {
                    listSPACTemp = dicItemStock[sItemAndBatch];
                  }
                  else
                  {
                    listSPACTemp = (from q in ScmsSoaLibrary.Modules.GlobalQuery.ViewStockLiteContains(db, gudang, field.Item)
                                    //join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                    //join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                    //from qBat in q_2.DefaultIfEmpty()
                                    //where (q.n_gsisa > 0) && (q.c_batch == field.Batch)
                                    where (q.n_gsisa != 0) && (q.c_batch == field.Batch.Trim())
                                    //orderby qBat.d_expired ascending
                                    select new PLClassComponent()
                                    {
                                      RefID = q.c_no,
                                      SignID = (q.c_table == null ? string.Empty : q.c_table.Trim()),
                                      Item = q.c_iteno,
                                      Qty = q.n_gsisa,
                                      //Supplier = q1.c_nosup,
                                      //Box = (q1.n_box.HasValue ? q1.n_box.Value : 0),
                                      BatchID = q.c_batch.Trim()
                                    }).Distinct().ToList();


                    dicItemStock.Add(sItemAndBatch, listSPACTemp);
                    
                  }

                  totalCurrentStock = listSPACTemp.Sum(t => t.Qty);

                  #region Recalculate

                  if (totalCurrentStock > 0)
                  {
                    listSPAC = listSPACTemp.FindAll(delegate(PLClassComponent plcc)
                    {
                      if (totalCurrentStock < 0)
                      {
                        return false;
                      }
                      else if (plcc.Qty <= 0)
                      {
                        return false;
                      }
                      else if (!plcc.BatchID.Equals(field.Batch, StringComparison.OrdinalIgnoreCase))
                      {
                        return false;
                      }

                      return true;
                    });
                  }

                  #region Old Coded

                  //listSPACGroupTemp = listSPACTemp.GroupBy(t => t.Item)
                  //  .Select(x => new PLClassComponent()
                  //  {
                  //    Item = x.Key,
                  //    Qty = x.Sum(g => g.Qty)
                  //  })
                  //  .Where(y => y.Qty > 0).ToList();

                  //listSPACGroupTemp.ForEach(delegate(PLClassComponent plcc)
                  //{
                  //  rnQty = plcc.Qty;

                  //  //listStockTemp.FindAll
                  //  listSPAC.AddRange(
                  //    listSPACTemp.FindAll(delegate(PLClassComponent plccY)
                  //    {
                  //      if (plcc.Item != plccY.Item)
                  //      {
                  //        return false;
                  //      }
                  //      else if (rnQty <= 0)
                  //      {
                  //        return false;
                  //      }
                  //      else if (plccY.Qty <= 0)
                  //      {
                  //        return false;
                  //      }

                  //      if (rnQty >= plccY.Qty)
                  //      {
                  //        rnQty -= plccY.Qty;

                  //        return true;
                  //      }
                  //      else
                  //      {
                  //        plccY.Qty = rnQty;

                  //        rnQty = 0;

                  //        return true;
                  //      }

                  //    }).ToArray());

                  //  listSPACTemp.RemoveAll(delegate(PLClassComponent plccY)
                  //  {
                  //    if (plcc.Item == plccY.Item)
                  //    {
                  //      return true;
                  //    }

                  //    return false;
                  //  });
                  //});

                  #endregion

                  #endregion

                  if (listSPAC.Count > 0)
                  {
                    //rnQty = totalCurrentStock;
                    if (totalCurrentStock <= 0)
                    {
                      continue;
                    }

                    nQtyTemp = field.Quantity;

                    #region New Stock Indra 20180305FM

                    if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                    plID,
                                                    structure.Fields.TypePackingList,
                                                    field.Item,
                                                    field.Batch,
                                                    field.Quantity * -1,
                                                    0,
                                                    "KOSONG",
                                                    "02",
                                                    "01",
                                                    nipEntry,
                                                    "01")) == 0)
                    {
                        result = "Terdapat Kesalahan pada Item " + field.Item + " dengan Batch " + field.Batch + ". Harap Hubungi MIS.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    #endregion

                    for (nLoopC = 0; nLoopC < listSPAC.Count; nLoopC++)
                    {
                      spac = listSPAC[nLoopC];
                      spAlloc = 0;

                      if (nQtyTemp <= 0)
                      {
                        continue;
                      }

                      #region Expand Data

                      if (spac.SignID.Equals("CB", StringComparison.OrdinalIgnoreCase))
                      {
                        combo = (from q in db.LG_ComboHs
                                 where (q.c_gdg == gudang) && (q.c_combono == spac.RefID)
                                  //&& (q.c_iteno == field.Item) && (q.c_batch == field.Batch)
                                  && (q.c_iteno == field.Item) && (q.c_batch == spac.BatchID)
                                  && (q.n_gsisa > 0)
                                 select q).Take(1).SingleOrDefault();

                        if (combo != null)
                        {
                          //spAlloc = (spSisa > spac.Qty ? spac.Qty : spSisa);

                          spAlloc = (spSisa > field.Quantity ? field.Quantity : spSisa);
                          spAlloc = (spac.Qty > spAlloc ? spAlloc : spac.Qty);
                          spAlloc = (totalCurrentStock > spAlloc ? spAlloc : totalCurrentStock);

                          refID = combo.c_combono;

                          totalCurrentStock -= spAlloc;

                          spac.Qty -= spAlloc;
                          combo.n_gsisa -= spAlloc;

                          nQtyTemp -= spAlloc;
                        }
                        else
                        {
                          spAlloc = 0;
                        }
                      }
                      else if (spac.SignID.Equals("RN", StringComparison.OrdinalIgnoreCase) || spac.SignID.Equals("RR", StringComparison.OrdinalIgnoreCase))
                      {
                        rnd1 = (from q in db.LG_RND1s
                                where (q.c_gdg == gudang) && (q.c_rnno == spac.RefID)
                                  && (q.c_iteno == field.Item) && (q.c_batch == field.Batch)
                                  && (q.n_gsisa > 0)
                                select q).Take(1).SingleOrDefault();

                        if (rnd1 != null)
                        {
                          //spAlloc = (spSisa > spac.Qty ? spac.Qty : spSisa);

                          spAlloc = (spSisa > field.Quantity ? field.Quantity : spSisa);
                          spAlloc = (spac.Qty > spAlloc ? spAlloc : spac.Qty);
                          spAlloc = (totalCurrentStock > spAlloc ? spAlloc : totalCurrentStock);

                          refID = rnd1.c_rnno;

                          totalCurrentStock -= spAlloc;

                          spac.Qty -= spAlloc;
                          rnd1.n_gsisa -= spAlloc;

                          if (rnd1.n_gsisa < 0) //cek rn minus
                          {
                              result = "Qty RN Kurang (add PL) " + field.Item + " " + field.Batch;
                              rpe = ResponseParser.ResponseParserEnum.IsFailed;
                              if (db.Transaction != null)
                              {
                                  db.Transaction.Rollback();
                              }
                              goto endLogic;
                          }

                          nQtyTemp -= spAlloc;
                        }
                        else
                        {
                          spAlloc = 0;
                        }
                      }
                      else
                      {
                        spAlloc = 0;
                      }

                      if (spac.Qty <= 0)
                      {
                        listSPACTemp.Remove(spac);
                      }

                      #endregion

                      if (spAlloc > 0)
                      {
                        #region Populate PLD

                        pld1 = listPld1.Find(delegate(LG_PLD1 pld)
                        {
                          return field.Item.Equals(pld.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                            field.Batch.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
                            //spac.BatchID.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
                            field.NomorSP.Equals(pld.c_spno, StringComparison.OrdinalIgnoreCase);
                        });

                        if (pld1 == null)
                        {
                          listPld1.Add(new LG_PLD1()
                          {
                            c_plno = plID,
                            c_iteno = field.Item,
                            c_batch = field.Batch,
                            //c_batch = spac.BatchID,
                            c_spno = field.NomorSP,
                            c_type = "01",
                            n_booked = spAlloc,
                            n_qty = spAlloc,
                            n_sisa = spAlloc,
                            l_expired = field.isED,
                            v_ket_ed = field.isED ? field.accket : null,
                            c_acc_ed = field.isED ? nipEntry : null
                          });
                        }
                        else
                        {
                          pld1.n_booked = pld1.n_qty = pld1.n_sisa += spAlloc;
                        }

                        pld2 = listPld2.Find(delegate(LG_PLD2 pld)
                        {
                          return field.Item.Equals(pld.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                            field.Batch.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
                            //spac.BatchID.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
                            field.NomorSP.Equals(pld.c_spno, StringComparison.OrdinalIgnoreCase) &&
                            refID.Equals(pld.c_rnno, StringComparison.OrdinalIgnoreCase);
                        });

                        if (pld2 == null)
                        {
                          listPld2.Add(new LG_PLD2()
                          {
                            c_plno = plID,
                            c_iteno = field.Item,
                            c_batch = field.Batch,
                            //c_batch = spac.BatchID,
                            c_spno = field.NomorSP,
                            c_type = "01",
                            c_rnno = refID,
                            n_qty = spAlloc,
                            n_sisa = spAlloc
                          });
                        }
                        else
                        {
                          pld2.n_qty = pld2.n_sisa += spAlloc;
                        }

                        spSisa -= spAlloc;
                        spd1.n_sisa = spSisa;

                        #endregion
                      }

                      if (spSisa <= 0)
                      {
                        break;
                      }
                      else if (totalCurrentStock <= 0)
                      {
                        break;
                      }
                    }

                    listSPAC.Clear();

                    totalDetails++;
                  }

                  #region Old Coded

                  //if (field.Quantity > spd1.n_sisa.Value)
                  //{
                  //  spQty = spd1.n_sisa.Value;
                  //}
                  //else
                  //{
                  //  spQty = field.Quantity;
                  //}

                  //spQtyReloc = spQty.Value;

                  ////listResRND1 = (from q in db.LG_RND1s
                  ////               join q1 in db.LG_RNHs on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                  ////               join q2 in db.LG_MsBatches on q.c_batch equals q2.c_batch
                  ////               join q3 in db.LG_RND2s on new { q.c_gdg, q.c_rnno } equals new { q3.c_gdg, q3.c_rnno }
                  ////               join q4 in db.LG_POD2s on new { q3.c_gdg, c_pono = q3.c_no } equals new { q4.c_gdg, q4.c_pono }
                  ////               join q5 in db.LG_ORHs on new { q4.c_gdg, q4.c_orno } equals new { q5.c_gdg, q5.c_orno }
                  ////               where q.c_gdg == gudang && q.c_batch == field.Batch && q.c_iteno == field.Item && q.n_gsisa > 0 && q5.c_type != "02"
                  ////               orderby q2.d_expired descending, q1.d_rndate
                  ////               select q).Distinct().ToList();

                  //listResRND1 = (from q in db.LG_RNHs
                  //               join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                  //               join q2 in db.LG_MsBatches on new { q1.c_iteno, q1.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                  //               from qBAT in q_2.DefaultIfEmpty()
                  //               where (q.c_gdg == gudang) && ((q.c_type != "06") || ((q.l_khusus.HasValue ? q.l_khusus.Value : false) == false)) //&& (q.c_type != "06")
                  //                 && (q1.c_batch == field.Batch) && (q1.c_iteno == field.Item)
                  //                 && (q1.n_gsisa > 0)
                  //               orderby qBAT.d_expired
                  //               select q1).Distinct().ToList();

                  //#region Cek RN

                  //if (listResRND1.Count > 0)
                  //{
                  //  for (nLoopC = 0; nLoopC < listResRND1.Count; nLoopC++)
                  //  {
                  //    rnd1 = listResRND1[nLoopC];

                  //    if ((rnd1.n_gsisa.HasValue) && (rnd1.n_gsisa.Value > 0))
                  //    {
                  //      if (rnd1.n_gsisa.Value >= spQtyReloc)
                  //      {
                  //        rnQty = spQtyReloc;

                  //        rnd1.n_gsisa -= spQtyReloc;
                  //        spQtyReloc = 0;
                  //      }
                  //      else
                  //      {
                  //        rnQty = rnd1.n_gsisa.Value;

                  //        spQtyReloc -= rnd1.n_gsisa.Value;
                  //        rnd1.n_gsisa = 0;
                  //      }

                  //      listPld2.Add(new LG_PLD2()
                  //      {
                  //        c_plno = plID,
                  //        c_batch = field.Batch,
                  //        c_iteno = field.Item,
                  //        c_rnno = rnd1.c_rnno,
                  //        c_spno = field.NomorSP,
                  //        c_type = "01",
                  //        n_qty = rnQty,
                  //        n_sisa = rnQty
                  //      });
                  //    }

                  //    if (spQtyReloc <= 0)
                  //    {
                  //      break;
                  //    }
                  //  }

                  //  spAlloc = (spQty.Value - spQtyReloc);

                  //  listPld1.Add(new LG_PLD1()
                  //  {
                  //    c_plno = plID,
                  //    c_batch = field.Batch,
                  //    c_iteno = field.Item,
                  //    c_spno = field.NomorSP,
                  //    c_type = "01",
                  //    n_booked = spAlloc,
                  //    n_qty = spAlloc,
                  //    n_sisa = spAlloc,
                  //  });
                  //}

                  //#endregion

                  //if ((listPld1.Count > 0) && (listPld2.Count > 0))
                  //{
                  //  if (spAlloc <= spd1.n_sisa.Value)
                  //  {
                  //    spd1.n_sisa -= spAlloc;
                  //  }
                  //  else
                  //  {
                  //    spd1.n_sisa -= 0;
                  //  }
                  //}

                  //listResRND1.Clear();

                  #endregion
                }

                #endregion

                #region BASPB
                if (!string.IsNullOrEmpty(structure.Fields.BaspbNo))
                {
                    baspbd = (from q in db.SCMS_BASPBDs
                              where q.c_baspbno == structure.Fields.BaspbNo
                                    && q.c_iteno == field.Item
                              select q).Take(1).SingleOrDefault();

                    if (baspbd != null)
                    {
                        baspbd.n_gsisa -= field.Quantity;
                    }
                }
                #endregion
              }
            }

            #region Cleaning

            foreach (KeyValuePair<string, List<PLClassComponent>> kvp in dicItemStock)
            {
              kvp.Value.Clear();
            }

            dicItemStock.Clear();

            #endregion

            if ((listPld1 != null) && (listPld1.Count > 0))
            {
              db.LG_PLD1s.InsertAllOnSubmit(listPld1.ToArray());
              listPld1.Clear();
            }

            if ((listPld2 != null) && (listPld2.Count > 0))
            {
              db.LG_PLD2s.InsertAllOnSubmit(listPld2.ToArray());
              listPld2.Clear();
            }
          }

          #endregion

          dic = new Dictionary<string, string>();

          if (totalDetails > 0)
          {
            dic.Add("PL", plID);
            dic.Add("Tanggal", date.ToString("yyyyMMdd"));

            result = string.Format("Total {0} detail(s)", totalDetails);

            hasAnyChanges = true;
          }
          else
          {
            hasAnyChanges = false;

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            result = "Detail tidak dapat di proses, cek sisa SP, OR dan RN.";
          }

          #endregion
        }
        else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase) ||
          structure.Method.Equals("ModifyConfirm", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify

          #region Validasi

          if (string.IsNullOrEmpty(plID))
          {
            result = "Nomor Packing List dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          plh = (from q in db.LG_PLHs
                 where q.c_plno == plID
                 select q).Take(1).SingleOrDefault();

          if (plh == null)
          {
            result = "Nomor Packing List tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (plh.l_delete.HasValue && plh.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor Packing List yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if ((plh.l_confirm.HasValue && plh.l_confirm.Value) && (!isConfirmMode))
          {
            result = "Tidak dapat mengubah nomor Packing List yang sudah terkonfirm.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, plh.d_pldate))
          {
            result = "Packing List tidak dapat diubah, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.HasDO(db, plID))
          {
            result = "Packing List yang sudah terdapat Delivery Order tidak dapat diubah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          #endregion

          if (isConfirmMode)
          {
            plh.l_confirm = structure.Fields.IsConfirm;
          }
          //else
          //{
          //  if (plh.l_confirm.HasValue && plh.l_confirm.Value)
          //  {
          //    result = "Tidak dapat mengubah nomor packing list yang sudah terkonfirm.";

          //    rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //    if (db.Transaction != null)
          //    {
          //      db.Transaction.Rollback();
          //    }

          //    goto endLogic;
          //  }
          //}

          if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
          {
            plh.v_ket = structure.Fields.Keterangan;
          }

          if (!string.IsNullOrEmpty(structure.Fields.TypePackingList))
          {
              plh.c_type = structure.Fields.TypePackingList;
          }
          
          if (!string.IsNullOrEmpty(structure.Fields.Via))
          {
              plh.c_via = structure.Fields.Via;
          }

          plh.c_update = nipEntry;
          plh.d_update = date;


          #region Populate Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            //listRN = new List<string>();
            listPld3 = new List<LG_PLD3>();
            //listResRND1 = new List<LG_RND1>();
            dicItemStock = new Dictionary<string, List<PLClassComponent>>(StringComparer.OrdinalIgnoreCase);
            lstBatch = new List<string>();

            listPld1 = (from q in db.LG_PLD1s
                        where q.c_plno == plID
                        select q).Distinct().ToList();

            listPld2 = (from q in db.LG_PLD2s
                        where q.c_plno == plID
                        select q).Distinct().ToList();

            pld2 = new LG_PLD2();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified) && (field.Quantity > 0))
              {
                #region Cek SP

                listSPAC = new List<PLClassComponent>();

                sItemAndBatch = field.Item.Trim() + field.Batch.Trim();

                if (dicItemStock.ContainsKey(sItemAndBatch))
                {
                  listSPACTemp = dicItemStock[sItemAndBatch];
                }
                else
                {
                  listSPACTemp = (from q in ScmsSoaLibrary.Modules.GlobalQuery.ViewStockLiteContains(db, gudang, field.Item)
                                  join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                  join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                  from qBat in q_2.DefaultIfEmpty()
                                  //where (q.n_gsisa > 0) && (q.c_batch == field.Batch)
                                  where (q.n_gsisa != 0) && (q.c_batch == field.Batch)
                                  orderby qBat.d_expired ascending
                                  select new PLClassComponent()
                                  {
                                    RefID = q.c_no,
                                    SignID = (q.c_table == null ? string.Empty : q.c_table.Trim()),
                                    Item = q.c_iteno,
                                    Qty = q.n_gsisa,
                                    Supplier = q1.c_nosup,
                                    Box = (q1.n_box.HasValue ? q1.n_box.Value : 0),
                                    BatchID = q.c_batch.Trim()
                                  }).Distinct().ToList();

                  dicItemStock.Add(sItemAndBatch, listSPACTemp);
                }

                totalCurrentStock = listSPACTemp.Sum(t => t.Qty);

                #region Recalculate
                
                if (totalCurrentStock > 0)
                {
                  listSPAC = listSPACTemp.FindAll(delegate(PLClassComponent plcc)
                  {
                    if (totalCurrentStock < 0)
                    {
                      return false;
                    }
                    else if (plcc.Qty <= 0)
                    {
                      return false;
                    }
                    else if (!plcc.BatchID.Equals(field.Batch, StringComparison.OrdinalIgnoreCase))
                    {
                      return false;
                    }

                    return true;
                  });
                }

                #region Old Coded

                //listSPACGroupTemp = listSPACTemp.GroupBy(t => t.Item)
                //  .Select(x => new PLClassComponent()
                //  {
                //    Item = x.Key,
                //    Qty = x.Sum(g => g.Qty)
                //  })
                //  .Where(y => y.Qty > 0).ToList();

                //listSPACGroupTemp.ForEach(delegate(PLClassComponent plcc)
                //{
                //  rnQty = plcc.Qty;

                //  //listStockTemp.FindAll
                //  listSPAC.AddRange(
                //    listSPACTemp.FindAll(delegate(PLClassComponent plccY)
                //    {
                //      if (plcc.Item != plccY.Item)
                //      {
                //        return false;
                //      }
                //      else if (rnQty <= 0)
                //      {
                //        return false;
                //      }
                //      else if (plccY.Qty <= 0)
                //      {
                //        return false;
                //      }

                //      if (rnQty >= plccY.Qty)
                //      {
                //        rnQty -= plccY.Qty;

                //        return true;
                //      }
                //      else
                //      {
                //        plccY.Qty = rnQty;

                //        rnQty = 0;

                //        return true;
                //      }

                //    }).ToArray());

                //  listSPACTemp.RemoveAll(delegate(PLClassComponent plccY)
                //  {
                //    if (plcc.Item == plccY.Item)
                //    {
                //      return true;
                //    }

                //    return false;
                //  });
                //});

                #endregion

                #endregion

                //spd1 = (from q in db.LG_SPD1s
                //        join q1 in db.LG_ORD2s on new { q.c_spno, q.c_iteno } equals new { q1.c_spno, q1.c_iteno }
                //        join q2 in db.LG_ORHs on q1.c_orno equals q2.c_orno
                //        where q.c_spno == field.NomorSP && q.c_iteno == field.Item && q2.c_type != "02"
                //        select q).Distinct().Take(1).SingleOrDefault();

                spd1 = (from q in db.LG_SPD1s
                        join q1 in
                          (from sq in db.LG_ORHs
                           join sq1 in db.LG_ORD2s on new { sq.c_gdg, sq.c_orno } equals new { sq1.c_gdg, sq1.c_orno }
                           where sq.c_type == "02" && sq1.c_spno == field.NomorSP && sq1.c_iteno == field.Item
                           select new
                           {
                             sq.c_gdg,
                             sq1.c_spno,
                             sq1.c_iteno
                           }) on new { q.c_spno, q.c_iteno } equals new { q1.c_spno, q1.c_iteno } into q_1
                        from qSQ in q_1.DefaultIfEmpty()
                        where q.c_spno == field.NomorSP && q.c_iteno == field.Item //&& qORH.c_type == "02"
                        select q).Distinct().Take(1).SingleOrDefault();

                if (spd1 != null)
                {
                  spSisa = (spd1.n_sisa.HasValue ? spd1.n_sisa.Value : 0);

                  if (spSisa <= 0)
                  {
                    continue;
                  }

                  if (listSPAC.Count > 0)
                  {
                    //rnQty = listSPAC.Sum(x => x.Qty);
                    //if (rnQty <= 0)
                    //{
                    //  continue;
                    //}

                    if (totalCurrentStock <= 0)
                    {
                      continue;
                    }

                    nQtyTemp = field.Quantity;

                    #region New Stock Indra 20180305FM

                    if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                    plID,
                                                    structure.Fields.TypePackingList,
                                                    field.Item,
                                                    field.Batch,
                                                    field.Quantity * -1,
                                                    0,
                                                    "KOSONG",
                                                    "02",
                                                    "01",
                                                    nipEntry,
                                                    "01")) == 0)
                    {
                        result = "Terdapat Kesalahan pada Item " + field.Item + " dengan Batch " + field.Batch + ". Harap Hubungi MIS.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    #endregion

                    for (nLoopC = 0; nLoopC < listSPAC.Count; nLoopC++)
                    {
                      spac = listSPAC[nLoopC];
                      spAlloc = 0;

                      if (nQtyTemp <= 0)
                      {
                        continue;
                      }

                      #region Expand Data

                      string sRef = null;

                      if (spac.SignID.Equals("CB", StringComparison.OrdinalIgnoreCase))
                      {
                        combo = (from q in db.LG_ComboHs
                                 where (q.c_gdg == gudang) && (q.c_combono == spac.RefID)
                                  && (q.c_iteno == field.Item) && (q.c_batch == field.Batch)
                                  && (q.n_gsisa > 0)
                                 select q).Take(1).SingleOrDefault();

                        if (combo != null)
                        {
                          //spAlloc = (spSisa > spac.Qty ? spac.Qty : spSisa);

                          spAlloc = (spSisa > field.Quantity ? field.Quantity : spSisa);
                          spAlloc = (spac.Qty > spAlloc ? spAlloc : spac.Qty);
                          spAlloc = (totalCurrentStock > spAlloc ? spAlloc : totalCurrentStock);

                          refID = combo.c_combono;

                          totalCurrentStock -= spAlloc;

                          spac.Qty -= spAlloc;
                          combo.n_gsisa -= spAlloc;

                          nQtyTemp -= spAlloc;
                        }
                        else
                        {
                          spAlloc = 0;
                        }
                      }
                      else if (spac.SignID.Equals("RN", StringComparison.OrdinalIgnoreCase) || spac.SignID.Equals("RR", StringComparison.OrdinalIgnoreCase))
                      {
                        rnd1 = (from q in db.LG_RND1s
                                where (q.c_gdg == gudang) && (q.c_rnno == spac.RefID)
                                  && (q.c_iteno == field.Item) && (q.c_batch == field.Batch)
                                  && (q.n_gsisa > 0)
                                select q).Take(1).SingleOrDefault();

                        if (rnd1 != null)
                        {
                          //spAlloc = (spSisa > spac.Qty ? spac.Qty : spSisa);

                          spAlloc = (spSisa > field.Quantity ? field.Quantity : spSisa);
                          spAlloc = (spac.Qty > spAlloc ? spAlloc : spac.Qty);
                          spAlloc = (totalCurrentStock > spAlloc ? spAlloc : totalCurrentStock);

                          refID = rnd1.c_rnno;

                          totalCurrentStock -= spAlloc;

                          spac.Qty -= spAlloc;
                          rnd1.n_gsisa -= spAlloc;

                          nQtyTemp -= spAlloc;

                          if (rnd1.n_gsisa < 0) //cek rn minus
                          {
                              result = "Qty RN Kurang (modify PL) " + field.Item + " " + field.Batch;
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
                          spAlloc = 0;
                        }
                      }
                      else
                      {
                        spAlloc = 0;
                      }

                      if (spac.Qty <= 0)
                      {
                        listSPACTemp.Remove(spac);
                      }

                      #endregion

                      if (spAlloc > 0)
                      {
                        #region Populate PLD

                        pld1 = listPld1.Find(delegate(LG_PLD1 pld)
                        {
                          return field.Item.Equals(pld.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                            field.Batch.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
                            field.NomorSP.Equals(pld.c_spno, StringComparison.OrdinalIgnoreCase);
                        });

                        if (pld1 == null)
                        {
                          pld1 = new LG_PLD1()
                          {
                            c_plno = plID,
                            c_iteno = field.Item,
                            c_batch = field.Batch,
                            c_spno = field.NomorSP,
                            c_type = "01",
                            n_booked = spAlloc,
                            n_qty = spAlloc,
                            n_sisa = spAlloc,
                            l_expired = field.isED,
                            v_ket_ed = field.isED ? field.accket : null,
                            c_acc_ed = field.isED ? nipEntry : null
                          };

                          listPld1.Add(pld1);

                          db.LG_PLD1s.InsertOnSubmit(pld1);
                        }
                        else
                        {
                          pld1.n_booked = pld1.n_qty = pld1.n_sisa += spAlloc;
                        }

                        pld2 = listPld2.Find(delegate(LG_PLD2 pld)
                        {
                          return field.Item.Equals(pld.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                            field.Batch.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
                            field.NomorSP.Equals(pld.c_spno, StringComparison.OrdinalIgnoreCase) &&
                            refID.Equals(pld.c_rnno, StringComparison.OrdinalIgnoreCase);
                        });

                        if (pld2 == null)
                        {
                          pld2 = new LG_PLD2()
                          {
                            c_plno = plID,
                            c_batch = field.Batch,
                            c_iteno = field.Item,
                            c_rnno = refID,
                            c_spno = field.NomorSP,
                            c_type = "01",
                            n_qty = spAlloc,
                            n_sisa = spAlloc
                          };

                          listPld2.Add(pld2);

                          db.LG_PLD2s.InsertOnSubmit(pld2);
                        }
                        else
                        {
                          pld2.n_qty = pld2.n_sisa += spAlloc;
                        }

                        #region Log PLD3

                        listPld3.Add(new LG_PLD3()
                        {
                          c_batch = field.Batch,
                          c_iteno = field.Item,
                          c_plno = plID,
                          c_rnno = refID,
                          c_spno = field.NomorSP,
                          c_type = "01",
                          n_qty = field.Quantity,
                          n_sisa = field.Quantity,
                          v_ket_del = field.Keterangan,
                          v_type = "01",
                          c_entry = nipEntry,
                          d_entry = date,
                          l_expired = field.isED,
                          v_ket_ed = field.isED ? field.accket : null,
                          c_acc_ed = field.isED ? nipEntry : null
                        });

                        #endregion

                        spSisa -= spAlloc;
                        spd1.n_sisa = spSisa;

                        #endregion
                      }

                      if (spSisa <= 0)
                      {
                        break;
                      }
                      else if (totalCurrentStock <= 0)
                      {
                        break;
                      }
                    }

                    listSPAC.Clear();

                    totalDetails++;
                  }

                  #region Old Coded

                  //if (field.Quantity > spd1.n_sisa.Value)
                  //{
                  //  spQty = spd1.n_sisa.Value;
                  //}
                  //else
                  //{
                  //  spQty = field.Quantity;
                  //}

                  //spQtyReloc = spQty.Value;

                  ////listResRND1 = (from q in db.LG_RND1s
                  ////               join q1 in db.LG_RNHs on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                  ////               join q2 in db.LG_MsBatches on q.c_batch equals q2.c_batch
                  ////               join q3 in db.LG_RND2s on new { q.c_gdg, q.c_rnno } equals new { q3.c_gdg, q3.c_rnno }
                  ////               join q4 in db.LG_POD2s on new { q3.c_gdg, c_pono = q3.c_no } equals new { q4.c_gdg, q4.c_pono }
                  ////               join q5 in db.LG_ORHs on new { q4.c_gdg, q4.c_orno } equals new { q5.c_gdg, q5.c_orno }
                  ////               where q.c_gdg == gudang && q.c_batch == field.Batch && q.c_iteno == field.Item && q.n_gsisa > 0 && q5.c_type != "02"
                  ////               orderby q2.d_expired descending, q1.d_rndate
                  ////               select q).Distinct().ToList();

                  //listResRND1 = (from q in db.LG_RNHs
                  //               join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                  //               join q2 in db.LG_MsBatches on new { q1.c_iteno, q1.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                  //               from qBAT in q_2.DefaultIfEmpty()
                  //               where (q.c_gdg == gudang) && ((q.c_type != "06") || ((q.l_khusus.HasValue ? q.l_khusus.Value : false) == false)) //&& (q.c_type != "06")
                  //                 && (q1.c_batch == field.Batch) && (q1.c_iteno == field.Item)
                  //                 && (q1.n_gsisa > 0)
                  //               orderby qBAT.d_expired
                  //               select q1).Distinct().ToList();

                  //#region Cek RN

                  //if (listResRND1.Count > 0)
                  //{
                  //  for (nLoopC = 0; nLoopC < listResRND1.Count; nLoopC++)
                  //  {
                  //    rnd1 = listResRND1[nLoopC];

                  //    if ((rnd1.n_gsisa.HasValue) && (rnd1.n_gsisa.Value > 0))
                  //    {
                  //      //rnQty = rnd1.n_gsisa.Value;

                  //      if (rnd1.n_gsisa.Value >= spQtyReloc)
                  //      {
                  //        rnQty = spQtyReloc;

                  //        rnd1.n_gsisa -= spQtyReloc;
                  //        spQtyReloc = 0;
                  //      }
                  //      else
                  //      {
                  //        rnQty = rnd1.n_gsisa.Value;

                  //        spQtyReloc -= rnd1.n_gsisa.Value;
                  //        rnd1.n_gsisa = 0;
                  //      }

                  //      pld2 = new LG_PLD2()
                  //      {
                  //        c_plno = plID,
                  //        c_batch = field.Batch,
                  //        c_iteno = field.Item,
                  //        c_rnno = rnd1.c_rnno,
                  //        c_spno = field.NomorSP,
                  //        c_type = "01",
                  //        n_qty = rnQty,
                  //        n_sisa = rnQty
                  //      };

                  //      listPld2.Add(pld2);

                  //      db.LG_PLD2s.InsertOnSubmit(pld2);

                  //      #region Log PLD3

                  //      listPld3.Add(new LG_PLD3()
                  //      {
                  //        c_batch = field.Batch,
                  //        c_iteno = field.Item,
                  //        c_plno = plID,
                  //        c_rnno = rnd1.c_rnno,
                  //        c_spno = field.NomorSP,
                  //        c_type = "01",
                  //        n_qty = field.Quantity,
                  //        n_sisa = field.Quantity,
                  //        v_ket_del = field.Keterangan,
                  //        v_type = "01",
                  //        c_entry = nipEntry,
                  //        d_entry = date
                  //      });

                  //      #endregion
                  //    }

                  //    if (spQtyReloc <= 0)
                  //    {
                  //      break;
                  //    }
                  //  }

                  //  spAlloc = (spQty.Value - spQtyReloc);

                  //  pld1 = new LG_PLD1()
                  //  {
                  //    c_plno = plID,
                  //    c_batch = field.Batch,
                  //    c_iteno = field.Item,
                  //    c_spno = field.NomorSP,
                  //    c_type = "01",
                  //    n_booked = spAlloc,
                  //    n_qty = spAlloc,
                  //    n_sisa = spAlloc,                      
                  //  };

                  //  listPld1.Add(pld1);

                  //  db.LG_PLD1s.InsertOnSubmit(pld1);
                  //}

                  //#endregion

                  //if ((listPld1.Count > 0) && (listPld2.Count > 0))
                  //{
                  //  if (spAlloc <= spd1.n_sisa.Value)
                  //  {
                  //    spd1.n_sisa -= spAlloc;
                  //  }
                  //  else
                  //  {
                  //    spd1.n_sisa -= 0;
                  //  }
                  //}

                  #endregion
                }

                #endregion

                #region BASPB
                if (!string.IsNullOrEmpty(structure.Fields.BaspbNo))
                {
                    baspbd = (from q in db.SCMS_BASPBDs
                              where q.c_baspbno == structure.Fields.BaspbNo
                                    && q.c_iteno == field.Item
                              select q).Take(1).SingleOrDefault();

                    if (baspbd != null)
                    {
                        baspbd.n_gsisa -= field.Quantity;
                    }
                }
                #endregion
              }
              else if ((field != null) && (!field.IsNew) && field.IsDelete && (!field.IsModified))
              {
                #region Cek SP

                //if (isConfirmMode)
                //{
                //  continue;
                //}

                //pld1 = (from q in db.LG_PLD1s
                //        where q.c_batch == field.Batch && q.c_iteno == field.Item &&
                //        q.c_plno == plID && q.c_spno == field.NomorSP
                //        select q).Distinct().Take(1).SingleOrDefault();

                pld1 = listPld1.Find(delegate(LG_PLD1 pld)
                {
                  return field.Item.Trim().Equals((string.IsNullOrEmpty(pld.c_iteno) ? string.Empty : pld.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    field.Batch.Trim().Equals((string.IsNullOrEmpty(pld.c_batch) ? string.Empty : pld.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    field.NomorSP.Trim().Equals((string.IsNullOrEmpty(pld.c_spno) ? string.Empty : pld.c_spno.Trim()), StringComparison.OrdinalIgnoreCase);
                });

                if (pld1 != null)
                {
                  spQtyReloc = (pld1.n_qty.HasValue ? pld1.n_qty.Value : 0);
                  spAlloc = (pld1.n_sisa.HasValue ? pld1.n_sisa.Value : 0);

                  #region New Stock Indra 20180305FM

                  if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                  plID,
                                                  structure.Fields.TypePackingList,
                                                  field.Item,
                                                  field.Batch,
                                                  field.Quantity,
                                                  0,
                                                  "KOSONG",
                                                  "01",
                                                  "01",
                                                  nipEntry,
                                                  "01")) == 0)
                  {
                        result = "Terdapat Kesalahan pada Item " + field.Item + " dengan Batch " + field.Batch + ". Harap Hubungi MIS.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                  }

                  #endregion

                  if (spAlloc.Equals(spQtyReloc))
                  {
                    #region Reverse SP

                    spd1 = (from q in db.LG_SPD1s
                            where q.c_spno == field.NomorSP && q.c_iteno == field.Item
                            select q).Distinct().Take(1).SingleOrDefault();

                    //if (spd1 != null)
                    //{
                    //  spd1.n_sisa += spAlloc;
                    //}

                    if (spd1 == null)
                    {
                      continue;
                    }

                    spd1.n_sisa += spAlloc;

                    #endregion

                    #region Reverse RN

                    //listPld2 = (from q in db.LG_PLD2s
                    //            where q.c_plno == plID && q.c_iteno == pld1.c_iteno &&
                    //            q.c_batch == pld1.c_batch
                    //            select q).Distinct().ToList();

                    listPld2Copy = listPld2.FindAll(delegate(LG_PLD2 pld)
                    {
                      return field.Item.Trim().Equals((string.IsNullOrEmpty(pld.c_iteno) ? string.Empty : pld.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                        field.Batch.Trim().Equals((string.IsNullOrEmpty(pld.c_batch) ? string.Empty : pld.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                        field.NomorSP.Trim().Equals((string.IsNullOrEmpty(pld.c_spno) ? string.Empty : pld.c_spno.Trim()), StringComparison.OrdinalIgnoreCase);
                    });

                    nQtySisa = field.Quantity;

                    if ((listPld2Copy != null) && (listPld2Copy.Count > 0))
                    {
                      for (nLoopC = 0; nLoopC < listPld2Copy.Count; nLoopC++)
                      {
                        pld2 = listPld2Copy[nLoopC];

                        if (pld2 != null && (nQtySisa > 0))
                        {
                          #region Reverse RN

                          refID = pld2.c_rnno;

                          if (refID.StartsWith("CB", StringComparison.OrdinalIgnoreCase))
                          {
                            combo = (from q in db.LG_ComboHs
                                     where (q.c_gdg == gudang) && (q.c_combono == refID)
                                      && (q.c_iteno == field.Item) && (q.c_batch == field.Batch.Trim())
                                     select q).Take(1).SingleOrDefault();

                            if (combo != null)
                            {
                              combo.n_gsisa += nQtySisa;

                              nQtySisa -= spAlloc;
                            }
                          }
                          else if (refID.StartsWith("RN", StringComparison.OrdinalIgnoreCase) || refID.StartsWith("RR", StringComparison.OrdinalIgnoreCase))
                          {
                            rnd1 = (from q in db.LG_RND1s
                                    where (q.c_gdg == gudang) && (q.c_rnno == refID)
                                     && (q.c_iteno == field.Item) && (q.c_batch == field.Batch.Trim())
                                    select q).Take(1).SingleOrDefault();

                            if (rnd1 != null)
                            {
                              rnd1.n_gsisa += nQtySisa;

                              nQtySisa -= spAlloc;
                            }
                          }

                          #endregion

                          #region Old Coded

                          //rnd1 = (from q in db.LG_RND1s
                          //        where q.c_batch == pld2.c_batch && q.c_iteno == pld2.c_iteno &&
                          //        q.c_rnno == pld2.c_rnno && q.c_gdg == plh.c_gdg
                          //        select q).Distinct().Take(1).SingleOrDefault();

                          //if (rnd1 != null)
                          //{
                          //  rnd1.n_gsisa += (pld2.n_qty);
                          //}

                          ////listResRND1.Add(rnd1);

                          #endregion

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
                            d_entry = date,
                            c_type_dc = pld1.c_type_dc,
                            l_expired = pld1.l_expired,
                            v_ket_ed = pld1.v_ket_ed,
                            c_acc_ed = pld1.c_acc_ed
                          });

                          #endregion
                        }
                      }

                      #region Delete PLD2

                      db.LG_PLD2s.DeleteAllOnSubmit(listPld2Copy.ToArray());

                      listPld2Copy.Clear();

                      #endregion
                    }

                    #endregion

                    #region Delete PLD1

                    db.LG_PLD1s.DeleteOnSubmit(pld1);

                    totalDelete++;

                    #endregion

                  }
                }

                #endregion

                #region BASPB
                if (!string.IsNullOrEmpty(structure.Fields.BaspbNo))
                {
                    baspbd = (from q in db.SCMS_BASPBDs
                              where q.c_baspbno == structure.Fields.BaspbNo
                                    && q.c_iteno == field.Item
                              select q).Take(1).SingleOrDefault();

                    if (baspbd != null)
                    {
                        baspbd.n_gsisa += field.Quantity;
                    }
                }
                #endregion
              }
              else if ((field != null) && (!field.IsNew) && (!field.IsDelete) && field.IsModified)
              {
                if (!isConfirmMode)
                {
                  continue;
                }
                //modify 15 april 2015
                //else if (string.IsNullOrEmpty(field.ConfirmType) || (field.ConfirmType.Equals("00")))
                else if (string.IsNullOrEmpty(field.ConfirmType))
                {
                  continue;
                }


                #region Cek SP
                //pld1 = (from q in db.LG_PLD1s
                //        where q.c_batch == field.Batch && q.c_iteno == field.Item &&
                //        q.c_plno == plID && q.c_spno == field.NomorSP
                //        select q).Distinct().Take(1).SingleOrDefault();

                pld1 = listPld1.Find(delegate(LG_PLD1 pld)
                {
                  return field.Item.Trim().Equals((string.IsNullOrEmpty(pld.c_iteno) ? string.Empty : pld.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    field.Batch.Trim().Equals((string.IsNullOrEmpty(pld.c_batch) ? string.Empty : pld.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                    field.NomorSP.Trim().Equals((string.IsNullOrEmpty(pld.c_spno) ? string.Empty : pld.c_spno.Trim()), StringComparison.OrdinalIgnoreCase);
                });

                if (pld1 != null)
                {
                  spQtyReloc = (pld1.n_qty.HasValue ? pld1.n_qty.Value : 0);
                  spAlloc = (pld1.n_sisa.HasValue ? pld1.n_sisa.Value : 0);

                  if (spAlloc.Equals(spQtyReloc) && (field.Quantity <= spQtyReloc))
                  {
                    //spQty = (spAlloc - field.Quantity);
                    spAlloc -= field.Quantity;

                    if (spAlloc <= 0)
                    {
                      continue;
                    }

                    #region Reverse SP

                    spd1 = (from q in db.LG_SPD1s
                            where q.c_spno == pld1.c_spno && q.c_iteno == pld1.c_iteno
                            select q).Distinct().Take(1).SingleOrDefault();

                    if (spd1 == null)
                    {
                      continue;
                    }

                    spd1.n_sisa += spAlloc;

                    //if (spd1 != null)
                    //{
                    //  spSisa = ((spd1.n_sisa.HasValue ? spd1.n_sisa.Value : 0) + (spQty.HasValue ? spQty.Value : 0));

                    //  if (spSisa < 0)
                    //  {
                    //    continue;
                    //  }

                    //  spd1.n_sisa += spQty;
                    //}

                    #endregion

                    #region Reverse RN

                    //listSPAC = (from q in ScmsSoaLibrary.Modules.GlobalQuery.ViewStockLite(db, gudang, field.Item)
                    //            join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                    //            join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                    //            from qBat in q_2.DefaultIfEmpty()
                    //            //where (q.n_gsisa > 0) && (q.c_batch == field.Batch)
                    //            where (q.n_gsisa != 0) && (q.c_batch == field.Batch)
                    //            orderby qBat.d_expired ascending
                    //            select new PLClassComponent()
                    //            {
                    //              RefID = q.c_no,
                    //              SignID = (q.c_table == null ? string.Empty : q.c_table.Trim()),
                    //              Item = q.c_iteno,
                    //              Qty = q.n_gsisa,
                    //              Supplier = q1.c_nosup,
                    //              Box = (q1.n_box.HasValue ? q1.n_box.Value : 0)
                    //            }).Distinct().ToList();

                    //listPld2 = (from q in db.LG_PLD2s
                    //            where q.c_plno == plID && q.c_iteno == pld1.c_iteno &&
                    //            q.c_batch == pld1.c_batch
                    //            select q).Distinct().ToList();

                    listPld2Copy = listPld2.FindAll(delegate(LG_PLD2 pld)
                    {
                      return field.Item.Trim().Equals((string.IsNullOrEmpty(pld.c_iteno) ? string.Empty : pld.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                        field.Batch.Trim().Equals((string.IsNullOrEmpty(pld.c_batch) ? string.Empty : pld.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                        field.NomorSP.Trim().Equals((string.IsNullOrEmpty(pld.c_spno) ? string.Empty : pld.c_spno.Trim()), StringComparison.OrdinalIgnoreCase);
                    });

                    if ((listPld2Copy != null) && (listPld2Copy.Count > 0))
                    {
                      rnQty = spAlloc;

                      for (nLoopC = (listPld2Copy.Count - 1); nLoopC >= 0; nLoopC++)
                      {
                        pld2 = listPld2Copy[nLoopC];

                        if (pld2 != null)
                        {
                          #region Reverse RN

                          refID = pld2.c_rnno;

                          if (refID.StartsWith("CB", StringComparison.OrdinalIgnoreCase))
                          {
                            combo = (from q in db.LG_ComboHs
                                     where (q.c_gdg == gudang) && (q.c_combono == refID)
                                      && (q.c_iteno == field.Item) && (q.c_batch == field.Batch.Trim())
                                     select q).Take(1).SingleOrDefault();

                            if (combo != null)
                            {
                              combo.n_gsisa += spAlloc;
                            }
                          }
                          else if (refID.StartsWith("RN", StringComparison.OrdinalIgnoreCase) || refID.StartsWith("RR", StringComparison.OrdinalIgnoreCase))
                          {
                            rnd1 = (from q in db.LG_RND1s
                                    where (q.c_gdg == gudang) && (q.c_rnno == refID)
                                     && (q.c_iteno == field.Item) && (q.c_batch == field.Batch.Trim())
                                    select q).Take(1).SingleOrDefault();

                            if (rnd1 != null)
                            {
                              rnd1.n_gsisa += spAlloc;
                            }
                          }

                          #endregion

                          #region Old Coded

                          //rnd1 = (from q in db.LG_RND1s
                          //        where q.c_batch == pld2.c_batch && q.c_iteno == pld2.c_iteno &&
                          //        q.c_rnno == pld2.c_rnno && q.c_gdg == plh.c_gdg
                          //        select q).Distinct().Take(1).SingleOrDefault();

                          //spAlloc = ((pld2.n_qty.HasValue ? pld2.n_qty.Value : 0) >= rnQty ?
                          //              rnQty : (pld2.n_qty.HasValue ? pld2.n_qty.Value : 0));

                          //rnd1.n_gsisa += spAlloc;

                          //pld2.n_sisa = pld2.n_qty -= spAlloc;

                          //rnQty -= spAlloc;

                          //if ((pld2.n_qty.HasValue ? pld2.n_qty.Value : 0m) <= 0.00m)
                          //{
                          //  #region Delete PLD2

                          //  db.LG_PLD2s.DeleteOnSubmit(pld2);

                          //  #endregion

                          //  #region Log PLD3

                          //  listPld3.Add(new LG_PLD3()
                          //  {
                          //    c_batch = pld2.c_batch,
                          //    c_iteno = pld2.c_iteno,
                          //    c_plno = pld2.c_plno,
                          //    c_rnno = pld2.c_rnno,
                          //    c_spno = pld2.c_spno,
                          //    c_type = pld2.c_type,
                          //    n_qty = pld2.n_qty,
                          //    n_sisa = pld2.n_sisa,
                          //    v_ket_del = field.Keterangan,
                          //    v_type = "02",
                          //    c_entry = nipEntry,
                          //    d_entry = date,
                          //    c_type_dc = pld1.c_type_dc
                          //  });

                          //  #endregion
                          //}

                          //listResRND1.Add(rnd1);

                          #endregion

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
                            d_entry = date,
                            c_type_dc = pld1.c_type_dc,
                            l_expired = pld1.l_expired,
                            v_ket_ed = pld1.v_ket_ed,
                            c_acc_ed = pld1.c_acc_ed
                          });

                          #endregion

                          pld2.n_sisa = pld2.n_qty -= spAlloc;

                          rnQty -= spAlloc;

                          if (rnQty <= 0)
                          {
                            break;
                          }
                        }
                      }

                      listPld2Copy.Clear();
                    }

                    #endregion

                    #region Reverse PLD1

                    pld1.c_type_dc = field.ConfirmType;

                    pld1.n_sisa = pld1.n_qty -= spAlloc;

                    pld1.l_expired = field.isED;
                    pld1.v_ket_ed = field.isED ? field.accket : null;
                    pld1.c_acc_ed = field.isED ? nipEntry : null;

                    #endregion

                    #region Insert Log PLD3

                    db.LG_PLD3s.InsertAllOnSubmit(listPld3.ToArray());

                    #endregion
                  }
                }

                #endregion
              }
              if ((field != null) && (!field.IsNew) && (!field.IsModified) && (field.isAccModify) && (!field.IsDelete))
              {

                  pld1 = listPld1.Find(delegate(LG_PLD1 pld)
                  {
                      return field.Item.Trim().Equals((string.IsNullOrEmpty(pld.c_iteno) ? string.Empty : pld.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                        field.Batch.Trim().Equals((string.IsNullOrEmpty(pld.c_batch) ? string.Empty : pld.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                        field.NomorSP.Trim().Equals((string.IsNullOrEmpty(pld.c_spno) ? string.Empty : pld.c_spno.Trim()), StringComparison.OrdinalIgnoreCase);
                  });

                  if (pld1 == null)
                  {
                      continue;
                  }
                  else
                  {
                      pld1.l_expired = field.isED;
                      pld1.v_ket_ed = field.isED ? field.accket : null;
                      pld1.c_acc_ed = field.isED ? nipEntry : null;
                  }
              }
            }

            #region Update PLH (Delete)
            if (listPld1.Count > 0 && !structure.Method.Equals("ModifyConfirm", StringComparison.OrdinalIgnoreCase))
            {
                if (listPld1.Count == totalDelete)
                {
                    plh.l_delete = true;
                }
            }
            #endregion
            //listRN.Clear();
            listPld1.Clear();
            listPld2.Clear();

            #region Insert Log PLD3

            if (listPld3.Count > 0)
            {
              db.LG_PLD3s.InsertAllOnSubmit(listPld3.ToArray());

              listPld3.Clear();
            }

            #endregion
              //listResRND1.Clear();
          }

          #endregion 

          hasAnyChanges = true;

          #endregion
        }
        else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete

          #region Validasi Indra 20171120

          if (string.IsNullOrEmpty(plID))
          {
            result = "Nomor Packing List dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }

          plh = (from q in db.LG_PLHs
                 where q.c_plno == plID
                 select q).Take(1).SingleOrDefault();

          if (plh == null)
          {
            result = "Nomor Packing List tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (plh.l_delete.HasValue && plh.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor Packing List yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (plh.l_confirm.HasValue && plh.l_confirm.Value)
          {
            result = "Tidak dapat menghapus nomor Packing List yang sudah terkonfirm.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, plh.d_pldate))
          {
            result = "Packing List tidak dapat dihapus, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.HasDO(db, plID))
          {
            result = "Packing List yang sudah terdapat Delivery Order tidak dapat diubah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          #endregion

          plh.c_update = nipEntry;
          plh.d_update = date;

          plh.l_delete = true;
          plh.v_ket_mark = structure.Fields.Keterangan;

          listPld3 = new List<LG_PLD3>();
          //listResRND1 = new List<LG_RND1>();

          listPld1 = (from q in db.LG_PLD1s
                      where (q.c_plno == plID)
                      select q).Distinct().ToList();

          listPld2 = (from q in db.LG_PLD2s
                      where q.c_plno == plID
                      select q).ToList();

          if (listPld2.Count > 0)
          {
            for (nLoopC = 0; nLoopC < listPld2.Count; nLoopC++)
            {
              pld2 = listPld2[nLoopC];

              if (pld2 != null)
              {
                #region Cek SP

                pld1 = listPld1.Find(delegate(LG_PLD1 pld)
                {
                  return ((string.IsNullOrEmpty(pld2.c_iteno) ? string.Empty : pld2.c_iteno.Trim()) == (string.IsNullOrEmpty(pld.c_iteno) ? string.Empty : pld.c_iteno.Trim()))
                    && ((string.IsNullOrEmpty(pld2.c_batch) ? string.Empty : pld2.c_batch.Trim()) == (string.IsNullOrEmpty(pld.c_batch) ? string.Empty : pld.c_batch.Trim()))
                    && ((string.IsNullOrEmpty(pld2.c_spno) ? string.Empty : pld2.c_spno.Trim()) == (string.IsNullOrEmpty(pld.c_spno) ? string.Empty : pld.c_spno.Trim()));
                });

                if (pld1 != null)
                {
                  spQtyReloc = (pld1.n_qty.HasValue ? pld1.n_qty.Value : 0);
                  spAlloc = (pld1.n_sisa.HasValue ? pld1.n_sisa.Value : 0);

                  if (spAlloc.Equals(spQtyReloc))
                  {
                    spAlloc = (pld2.n_qty.HasValue ? pld2.n_qty.Value : 0);

                    #region New Stock Indra 20180305FM

                    if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                    plID,
                                                    plh.c_type,
                                                    pld1.c_iteno,
                                                    pld1.c_batch,
                                                    spAlloc,
                                                    0,
                                                    "KOSONG",
                                                    "01",
                                                    "01",
                                                    nipEntry,
                                                    "01")) == 0)
                    {
                        result = "Terdapat Kesalahan pada Item " + pld1.c_iteno + " dengan Batch " + pld1.c_batch + ". Harap Hubungi MIS.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    #endregion

                    #region Reverse SP

                    spd1 = (from q in db.LG_SPD1s
                            where q.c_spno == pld1.c_spno && q.c_iteno == pld1.c_iteno
                            select q).Distinct().Take(1).SingleOrDefault();

                    if (spd1 != null)
                    {
                      spd1.n_sisa += spAlloc;
                    }

                    #endregion

                    #region Reverse RN

                    refID = pld2.c_rnno;

                    if (refID.StartsWith("CB", StringComparison.OrdinalIgnoreCase))
                    {
                      combo = (from q in db.LG_ComboHs
                               where (q.c_gdg == gudang) && (q.c_combono == refID)
                                && (q.c_iteno == pld1.c_iteno) && (q.c_batch == pld1.c_batch)
                               select q).Take(1).SingleOrDefault();

                      if (combo != null)
                      {
                        combo.n_gsisa += spAlloc;
                      }
                    }
                    else if (refID.StartsWith("RN", StringComparison.OrdinalIgnoreCase) || refID.StartsWith("RR", StringComparison.OrdinalIgnoreCase))
                    {
                      rnd1 = (from q in db.LG_RND1s
                              where (q.c_gdg == gudang) && (q.c_rnno == refID)
                               && (q.c_iteno == pld1.c_iteno) && (q.c_batch == pld1.c_batch)
                              select q).Take(1).SingleOrDefault();

                      if (rnd1 != null)
                      {
                        rnd1.n_gsisa += spAlloc;
                      }
                    }

                    #region Old Coded

                    //rnd1 = (from q in db.LG_RND1s
                    //        where q.c_batch == pld2.c_batch && q.c_iteno == pld2.c_iteno &&
                    //        q.c_rnno == pld2.c_rnno && q.c_gdg == plh.c_gdg
                    //        select q).Distinct().Take(1).SingleOrDefault();

                    //if (rnd1 != null)
                    //{
                    //  rnd1.n_gsisa += spAlloc;
                    //}

                    #endregion

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
                      d_entry = date,
                      c_type_dc = pld1.c_type_dc
                    });

                    #endregion

                    #region Old Coded

                    //listPld2 = (from q in db.LG_PLD2s
                    //            where q.c_plno == plID && q.c_iteno == pld1.c_iteno &&
                    //            q.c_batch == pld1.c_batch
                    //            select q).Distinct().ToList();

                    //if ((listPld2 != null) && (listPld2.Count > 0))
                    //{
                    //  for (nLoopC = 0; nLoopC < listPld2.Count; nLoopC++)
                    //  {
                    //    pld2 = listPld2[nLoopC];

                    //    if (pld2 != null)
                    //    {
                    //      rnd1 = (from q in db.LG_RND1s
                    //              where q.c_batch == pld2.c_batch && q.c_iteno == pld2.c_iteno &&
                    //              q.c_rnno == pld2.c_rnno && q.c_gdg == plh.c_gdg
                    //              select q).Distinct().Take(1).SingleOrDefault();

                    //      rnd1.n_gsisa += (pld2.n_qty);

                    //      listResRND1.Add(rnd1);


                    //    }
                    //  }
                    //}

                    #endregion

                    #endregion

                  }
                }

                #endregion

                #region Old Coded

                //rnd1 = (from q in db.LG_RND1s
                //        where q.c_batch == pld2.c_batch && q.c_iteno == pld2.c_iteno &&
                //        q.c_rnno == pld2.c_rnno && q.c_gdg == plh.c_gdg
                //        select q).Distinct().Take(1).SingleOrDefault();

                //rnd1.n_gsisa += (pld2.n_qty);

                //listResRND1.Add(rnd1);

                //#region Log PLD3

                //listPld3.Add(new LG_PLD3()
                //{
                //  c_batch = pld2.c_batch,
                //  c_iteno = pld2.c_iteno,
                //  c_plno = pld2.c_plno,
                //  c_rnno = pld2.c_rnno,
                //  c_spno = pld2.c_spno,
                //  c_type = pld2.c_type,
                //  n_qty = pld2.n_qty,
                //  n_sisa = pld2.n_sisa,
                //  v_ket_del = structure.Fields.Keterangan,
                //  v_type = "03",
                //  c_entry = nipEntry,
                //  d_entry = date
                //});

                //#endregion

                #endregion
              }
            }

            if ((listPld1 != null) && (listPld1.Count > 0))
            {
              db.LG_PLD1s.DeleteAllOnSubmit(listPld1.ToArray());
              listPld1.Clear();
            }

            db.LG_PLD2s.DeleteAllOnSubmit(listPld2.ToArray());
            listPld2.Clear();
          }

          if (listPld3.Count > 0)
          {
            db.LG_PLD3s.InsertAllOnSubmit(listPld3.ToArray());
            listPld3.Clear();
          }

          hasAnyChanges = true;

          #endregion
        }

        #region Old Coded

        //else if (structure.Method.Equals("ModifyConfirm", StringComparison.OrdinalIgnoreCase))
        //{
        //  #region ModifyConfirm

        //  if (string.IsNullOrEmpty(plID))
        //  {
        //    result = "Nomor Packing List dibutuhkan.";

        //    rpe = ResponseParser.ResponseParserEnum.IsFailed;

        //    goto endLogic;
        //  }

        //  plh = (from q in db.LG_PLHs
        //         where q.c_plno == plID
        //         select q).Take(1).SingleOrDefault();

        //  if (plh == null)
        //  {
        //    result = "Nomor Packing List tidak ditemukan.";

        //    rpe = ResponseParser.ResponseParserEnum.IsFailed;

        //    if (db.Transaction != null)
        //    {
        //      db.Transaction.Rollback();
        //    }

        //    goto endLogic;
        //  }
        //  else if (plh.l_delete.HasValue && plh.l_delete.Value)
        //  {
        //    result = "Tidak dapat menghapus nomor Packing List yang sudah terhapus.";

        //    rpe = ResponseParser.ResponseParserEnum.IsFailed;

        //    if (db.Transaction != null)
        //    {
        //      db.Transaction.Rollback();
        //    }

        //    goto endLogic;
        //  }
        //  else if (Commons.HasDO(db, plID))
        //  {
        //    result = "Packing List yang sudah terdapat Delivery Order tidak dapat diubah.";

        //    rpe = ResponseParser.ResponseParserEnum.IsFailed;

        //    if (db.Transaction != null)
        //    {
        //      db.Transaction.Rollback();
        //    }

        //    goto endLogic;
        //  }
        //  else if (Commons.IsClosingLogistik(db, plh.d_pldate))
        //  {
        //    result = "Packing List tidak dapat di Confirm, karena sudah closing.";

        //    rpe = ResponseParser.ResponseParserEnum.IsFailed;

        //    if (db.Transaction != null)
        //    {
        //      db.Transaction.Rollback();
        //    }

        //    goto endLogic;
        //  }

        //  //isConfirmed = ((((plh.l_confirm == null) || (!plh.l_confirm.Value)) && structure.Fields.IsConfirm) ? true :
        //  //  ((plh.l_confirm.Value && (!structure.Fields.IsConfirm)) ? true : false));

        //  //isConfirmed = (plh.l_confirm.HasValue ? plh.l_confirm.Value : false);

        //  if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
        //  {
        //    plh.v_ket = structure.Fields.Keterangan;
        //  }

        //  plh.l_confirm = structure.Fields.IsConfirm;

        //  plh.c_update = nipEntry;
        //  plh.d_update = date;

        //  #region Populate Detail

        //  if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
        //  {
        //    listRN = new List<string>();

        //    //listPld1 = new List<LG_PLD1>();
        //    //listPld2 = new List<LG_PLD2>();

        //    listPld1 = (from q in db.LG_PLD1s
        //                where q.c_plno == plID
        //                select q).Distinct().ToList();

        //    listPld2 = (from q in db.LG_PLD2s
        //                where q.c_plno == plID
        //                select q).Distinct().ToList();

        //    listPld3 = new List<LG_PLD3>();
        //    listResRND1 = new List<LG_RND1>();

        //    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
        //    {
        //      field = structure.Fields.Field[nLoop];

        //      if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified) && (field.Quantity > 0))
        //      {
        //        #region Cek SP

        //        listSPAC = (from q in ScmsSoaLibrary.Modules.GlobalQuery.ViewStockLite(db, gudang, field.Item)
        //                    join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
        //                    join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
        //                    from qBat in q_2.DefaultIfEmpty()
        //                    //where (q.n_gsisa > 0) && (q.c_batch == field.Batch)
        //                    where (q.n_gsisa != 0) && (q.c_batch == field.Batch)
        //                    orderby qBat.d_expired ascending
        //                    select new PLClassComponent()
        //                    {
        //                      RefID = q.c_no,
        //                      SignID = (q.c_table == null ? string.Empty : q.c_table.Trim()),
        //                      Item = q.c_iteno,
        //                      Qty = q.n_gsisa,
        //                      Supplier = q1.c_nosup,
        //                      Box = (q1.n_box.HasValue ? q1.n_box.Value : 0)
        //                    }).Distinct().ToList();

        //        //spd1 = (from q in db.LG_SPD1s
        //        //        join q1 in db.LG_ORD2s on new { q.c_spno, q.c_iteno } equals new { q1.c_spno, q1.c_iteno }
        //        //        join q2 in db.LG_ORHs on q1.c_orno equals q2.c_orno
        //        //        where q.c_spno == field.NomorSP && q.c_iteno == field.Item && q2.c_type != "02"
        //        //        select q).Distinct().Take(1).SingleOrDefault();

        //        spd1 = (from q in db.LG_SPD1s
        //                join q1 in
        //                  (from sq in db.LG_ORHs
        //                   join sq1 in db.LG_ORD2s on new { sq.c_gdg, sq.c_orno } equals new { sq1.c_gdg, sq1.c_orno }
        //                   where sq.c_type == "02" && sq1.c_spno == field.NomorSP && sq1.c_iteno == field.Item
        //                   select new
        //                   {
        //                     sq.c_gdg,
        //                     sq1.c_spno,
        //                     sq1.c_iteno
        //                   }) on new { q.c_spno, q.c_iteno } equals new { q1.c_spno, q1.c_iteno } into q_1
        //                from qSQ in q_1.DefaultIfEmpty()
        //                where q.c_spno == field.NomorSP && q.c_iteno == field.Item //&& qORH.c_type == "02"
        //                select q).Distinct().Take(1).SingleOrDefault();

        //        if (spd1 != null)
        //        {
        //          spSisa = (spd1.n_sisa.HasValue ? spd1.n_sisa.Value : 0);

        //          if (spSisa <= 0)
        //          {
        //            continue;
        //          }

        //          if (listSPAC.Count > 0)
        //          {
        //            rnQty = listSPAC.Sum(x => x.Qty);
        //            if (rnQty <= 0)
        //            {
        //              continue;
        //            }

        //            for (nLoopC = 0; nLoopC < listSPAC.Count; nLoopC++)
        //            {
        //              spac = listSPAC[nLoopC];
        //              spAlloc = 0;

        //              #region Expand Data

        //              if (spac.SignID.Equals("CB", StringComparison.OrdinalIgnoreCase))
        //              {
        //                combo = (from q in db.LG_ComboHs
        //                         where (q.c_gdg == gudang) && (q.c_combono == spac.RefID)
        //                          && (q.c_iteno == field.Item) && (q.c_batch == field.Batch)
        //                          && (q.n_gsisa > 0)
        //                         select q).Take(1).SingleOrDefault();

        //                if (combo != null)
        //                {
        //                  //spAlloc = (spSisa > spac.Qty ? spac.Qty : spSisa);

        //                  spAlloc = (spSisa > field.Quantity ? field.Quantity : spSisa);
        //                  spAlloc = (spac.Qty > spAlloc ? spAlloc : spac.Qty);

        //                  refID = combo.c_combono;

        //                  combo.n_gsisa -= spAlloc;
        //                }
        //              }
        //              else if (spac.SignID.Equals("RN", StringComparison.OrdinalIgnoreCase))
        //              {
        //                rnd1 = (from q in db.LG_RND1s
        //                        where (q.c_gdg == gudang) && (q.c_rnno == spac.RefID)
        //                          && (q.c_iteno == field.Item) && (q.c_batch == field.Batch)
        //                          && (q.n_gsisa > 0)
        //                        select q).Take(1).SingleOrDefault();

        //                if (rnd1 != null)
        //                {
        //                  //spAlloc = (spSisa > spac.Qty ? spac.Qty : spSisa);

        //                  spAlloc = (spSisa > field.Quantity ? field.Quantity : spSisa);
        //                  spAlloc = (spac.Qty > spAlloc ? spAlloc : spac.Qty);

        //                  refID = rnd1.c_rnno;

        //                  rnd1.n_gsisa -= spAlloc;
        //                }
        //              }

        //              #endregion

        //              if (spAlloc > 0)
        //              {
        //                #region Populate PLD

        //                pld1 = listPld1.Find(delegate(LG_PLD1 pld)
        //                {
        //                  return field.Item.Equals(pld.c_iteno, StringComparison.OrdinalIgnoreCase) &&
        //                    field.Batch.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
        //                    field.NomorSP.Equals(pld.c_spno, StringComparison.OrdinalIgnoreCase);
        //                });

        //                if (pld1 == null)
        //                {
        //                  pld1 = new LG_PLD1()
        //                  {
        //                    c_plno = plID,
        //                    c_iteno = field.Item,
        //                    c_batch = field.Batch,
        //                    c_spno = field.NomorSP,
        //                    c_type = "01",
        //                    n_booked = spAlloc,
        //                    n_qty = spAlloc,
        //                    n_sisa = spAlloc
        //                  };

        //                  listPld1.Add(pld1);

        //                  db.LG_PLD1s.InsertOnSubmit(pld1);
        //                }
        //                else
        //                {
        //                  pld1.n_booked = pld1.n_qty = pld1.n_sisa += spAlloc;
        //                }

        //                pld2 = listPld2.Find(delegate(LG_PLD2 pld)
        //                {
        //                  return field.Item.Equals(pld.c_iteno, StringComparison.OrdinalIgnoreCase) &&
        //                    field.Batch.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
        //                    field.NomorSP.Equals(pld.c_spno, StringComparison.OrdinalIgnoreCase) &&
        //                    refID.Equals(pld.c_rnno, StringComparison.OrdinalIgnoreCase);
        //                });

        //                if (pld2 == null)
        //                {
        //                  pld2 = new LG_PLD2()
        //                  {
        //                    c_plno = plID,
        //                    c_batch = field.Batch,
        //                    c_iteno = field.Item,
        //                    c_rnno = rnd1.c_rnno,
        //                    c_spno = field.NomorSP,
        //                    c_type = "01",
        //                    n_qty = rnQty,
        //                    n_sisa = rnQty
        //                  };

        //                  listPld2.Add(pld2);

        //                  db.LG_PLD2s.InsertOnSubmit(pld2);
        //                }
        //                else
        //                {
        //                  pld2.n_qty = pld2.n_sisa += spAlloc;
        //                }

        //                #region Log PLD3

        //                listPld3.Add(new LG_PLD3()
        //                {
        //                  c_batch = field.Batch,
        //                  c_iteno = field.Item,
        //                  c_plno = plID,
        //                  c_rnno = rnd1.c_rnno,
        //                  c_spno = field.NomorSP,
        //                  c_type = "01",
        //                  n_qty = field.Quantity,
        //                  n_sisa = field.Quantity,
        //                  v_ket_del = field.Keterangan,
        //                  v_type = "01",
        //                  c_entry = nipEntry,
        //                  d_entry = date
        //                });

        //                #endregion

        //                spSisa -= spAlloc;
        //                spd1.n_sisa = spSisa;

        //                #endregion
        //              }

        //              if (spSisa <= 0)
        //              {
        //                break;
        //              }
        //            }

        //            listSPAC.Clear();
        //          }

        //          totalDetails++;

        //          #region Old Coded

        //          //if (field.Quantity > spd1.n_sisa.Value)
        //          //{
        //          //  spQty = spd1.n_sisa.Value;
        //          //}
        //          //else
        //          //{
        //          //  spQty = field.Quantity;
        //          //}

        //          //spQtyReloc = spQty.Value;

        //          ////listResRND1 = (from q in db.LG_RND1s
        //          ////               join q1 in db.LG_RNHs on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
        //          ////               join q2 in db.LG_MsBatches on q.c_batch equals q2.c_batch
        //          ////               join q3 in db.LG_RND2s on new { q.c_gdg, q.c_rnno } equals new { q3.c_gdg, q3.c_rnno }
        //          ////               join q4 in db.LG_POD2s on new { q3.c_gdg, c_pono = q3.c_no } equals new { q4.c_gdg, q4.c_pono }
        //          ////               join q5 in db.LG_ORHs on new { q4.c_gdg, q4.c_orno } equals new { q5.c_gdg, q5.c_orno }
        //          ////               where q.c_gdg == gudang && q.c_batch == field.Batch && q.c_iteno == field.Item && q.n_gsisa > 0 && q5.c_type != "02"
        //          ////               orderby q2.d_expired descending, q1.d_rndate
        //          ////               select q).Distinct().ToList();

        //          //listResRND1 = (from q in db.LG_RNHs
        //          //               join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
        //          //               join q2 in db.LG_MsBatches on new { q1.c_iteno, q1.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
        //          //               from qBAT in q_2.DefaultIfEmpty()
        //          //               where (q.c_gdg == gudang) && ((q.c_type != "06") || ((q.l_khusus.HasValue ? q.l_khusus.Value : false) == false)) //&& (q.c_type != "06")
        //          //                 && (q1.c_batch == field.Batch) && (q1.c_iteno == field.Item)
        //          //                 && (q1.n_gsisa > 0)
        //          //               orderby qBAT.d_expired
        //          //               select q1).Distinct().ToList();

        //          //#region Cek RN

        //          //if (listResRND1.Count > 0)
        //          //{
        //          //  for (nLoopC = 0; nLoopC < listResRND1.Count; nLoopC++)
        //          //  {
        //          //    rnd1 = listResRND1[nLoopC];

        //          //    if ((rnd1.n_gsisa.HasValue) && (rnd1.n_gsisa.Value > 0))
        //          //    {
        //          //      //rnQty = rnd1.n_gsisa.Value;

        //          //      if (rnd1.n_gsisa.Value >= spQtyReloc)
        //          //      {
        //          //        rnQty = spQtyReloc;

        //          //        rnd1.n_gsisa -= spQtyReloc;
        //          //        spQtyReloc = 0;
        //          //      }
        //          //      else
        //          //      {
        //          //        rnQty = rnd1.n_gsisa.Value;

        //          //        spQtyReloc -= rnd1.n_gsisa.Value;
        //          //        rnd1.n_gsisa = 0;
        //          //      }

        //          //      listPld2.Add(new LG_PLD2()
        //          //      {
        //          //        c_plno = plID,
        //          //        c_batch = field.Batch,
        //          //        c_iteno = field.Item,
        //          //        c_rnno = rnd1.c_rnno,
        //          //        c_spno = field.NomorSP,
        //          //        c_type = "01",
        //          //        n_qty = rnQty,
        //          //        n_sisa = rnQty
        //          //      });
        //          //    }

        //          //    if (spQtyReloc <= 0)
        //          //    {
        //          //      break;
        //          //    }
        //          //  }

        //          //  spAlloc = (spQty.Value - spQtyReloc);

        //          //  listPld1.Add(new LG_PLD1()
        //          //  {
        //          //    c_plno = plID,
        //          //    c_batch = field.Batch,
        //          //    c_iteno = field.Item,
        //          //    c_spno = field.NomorSP,
        //          //    c_type = "01",
        //          //    n_booked = spAlloc,
        //          //    n_qty = spAlloc,
        //          //    n_sisa = spAlloc
        //          //  });
        //          //}

        //          //#endregion

        //          //if ((listPld1.Count > 0) && (listPld2.Count > 0))
        //          //{
        //          //  if (spAlloc <= spd1.n_sisa.Value)
        //          //  {
        //          //    spd1.n_sisa -= spAlloc;
        //          //  }
        //          //  else
        //          //  {
        //          //    spd1.n_sisa -= 0;
        //          //  }
        //          //}

        //          #endregion
        //        }

        //        #region Old Coded

        //        //if ((listPld1.Count > 0) && (listPld2.Count > 0))
        //        //{
        //        //  #region Log PLD3

        //        //  for (nLoopC = 0; nLoopC < listPld2.Count; nLoopC++)
        //        //  {
        //        //    pld2 = listPld2[nLoopC];

        //        //    if (pld2 != null)
        //        //    {
        //        //      db.LG_PLD3s.InsertOnSubmit(new LG_PLD3()
        //        //      {
        //        //        c_batch = pld2.c_batch,
        //        //        c_iteno = pld2.c_iteno,
        //        //        c_plno = pld2.c_plno,
        //        //        c_rnno = pld2.c_rnno,
        //        //        c_spno = pld2.c_spno,
        //        //        c_type = pld2.c_type,
        //        //        n_qty = pld2.n_qty,
        //        //        n_sisa = pld2.n_sisa,
        //        //        c_entry = nipEntry,
        //        //        d_entry = date,
        //        //        v_ket_del = "Insert on Confirm",
        //        //        v_type = "01"
        //        //      });
        //        //    }
        //        //  }

        //        //  #endregion

        //        //  db.LG_PLD1s.InsertAllOnSubmit(listPld1.ToArray());
        //        //  db.LG_PLD2s.InsertAllOnSubmit(listPld2.ToArray());

        //        //  totalDetails++;
        //        //}

        //        #endregion

        //        #endregion
        //      }
        //      else if ((field != null) && (!field.IsNew) && (!field.IsDelete) && field.IsModified)
        //      {
        //        #region Cek SP

        //        if (string.IsNullOrEmpty(field.ConfirmType) || (field.ConfirmType.Equals("00")))
        //        {
        //          continue;
        //        }

        //        //pld1 = (from q in db.LG_PLD1s
        //        //        where q.c_batch == field.Batch && q.c_iteno == field.Item &&
        //        //        q.c_plno == plID && q.c_spno == field.NomorSP
        //        //        select q).Distinct().Take(1).SingleOrDefault();

        //        pld1 = listPld1.Find(delegate(LG_PLD1 pld)
        //        {
        //          return field.Item.Trim().Equals((string.IsNullOrEmpty(pld.c_iteno) ? string.Empty : pld.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
        //            field.Batch.Trim().Equals((string.IsNullOrEmpty(pld.c_batch) ? string.Empty : pld.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
        //            field.NomorSP.Trim().Equals((string.IsNullOrEmpty(pld.c_spno) ? string.Empty : pld.c_spno.Trim()), StringComparison.OrdinalIgnoreCase);
        //        });

        //        if (pld1 != null)
        //        {
        //          spQtyReloc = (pld1.n_qty.HasValue ? pld1.n_qty.Value : 0);
        //          spAlloc = (pld1.n_sisa.HasValue ? pld1.n_sisa.Value : 0);

        //          if (spAlloc.Equals(spQtyReloc) && (field.Quantity <= spQtyReloc))
        //          {
        //            //spQty = (spAlloc - field.Quantity);
        //            spAlloc = (spAlloc - field.Quantity);

        //            if (spAlloc <= 0)
        //            {
        //              continue;
        //            }

        //            #region Reverse SP

        //            spd1 = (from q in db.LG_SPD1s
        //                    where q.c_spno == pld1.c_spno && q.c_iteno == pld1.c_iteno
        //                    select q).Distinct().Take(1).SingleOrDefault();

        //            if (spd1 == null)
        //            {
        //              continue;
        //            }

        //            spd1.n_sisa += spAlloc;

        //            //if (spd1 != null)
        //            //{
        //            //  spSisa = ((spd1.n_sisa.HasValue ? spd1.n_sisa.Value : 0) + (spQty.HasValue ? spQty.Value : 0));

        //            //  if (spSisa < 0)
        //            //  {
        //            //    continue;
        //            //  }

        //            //  spd1.n_sisa += spQty;
        //            //}

        //            #endregion

        //            #region Reverse RN

        //            listSPAC = (from q in ScmsSoaLibrary.Modules.GlobalQuery.ViewStockLite(db, gudang, field.Item)
        //                        join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
        //                        join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
        //                        from qBat in q_2.DefaultIfEmpty()
        //                        //where (q.n_gsisa > 0) && (q.c_batch == field.Batch)
        //                        where (q.n_gsisa != 0) && (q.c_batch == field.Batch)
        //                        orderby qBat.d_expired ascending
        //                        select new PLClassComponent()
        //                        {
        //                          RefID = q.c_no,
        //                          SignID = (q.c_table == null ? string.Empty : q.c_table.Trim()),
        //                          Item = q.c_iteno,
        //                          Qty = q.n_gsisa,
        //                          Supplier = q1.c_nosup,
        //                          Box = (q1.n_box.HasValue ? q1.n_box.Value : 0)
        //                        }).Distinct().ToList();

        //            //listPld2 = (from q in db.LG_PLD2s
        //            //            where q.c_plno == plID && q.c_iteno == pld1.c_iteno &&
        //            //            q.c_batch == pld1.c_batch
        //            //            select q).Distinct().ToList();

        //            listPld2Copy = listPld2.FindAll(delegate(LG_PLD2 pld)
        //            {
        //              return field.Item.Trim().Equals((string.IsNullOrEmpty(pld.c_iteno) ? string.Empty : pld.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
        //                field.Batch.Trim().Equals((string.IsNullOrEmpty(pld.c_batch) ? string.Empty : pld.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
        //                field.NomorSP.Trim().Equals((string.IsNullOrEmpty(pld.c_spno) ? string.Empty : pld.c_spno.Trim()), StringComparison.OrdinalIgnoreCase);
        //            });

        //            if ((listPld2Copy != null) && (listPld2Copy.Count > 0))
        //            {
        //              rnQty = spAlloc;

        //              for (nLoopC = (listPld2Copy.Count - 1); nLoopC >= 0; nLoopC++)
        //              {
        //                pld2 = listPld2Copy[nLoopC];

        //                if (pld2 != null)
        //                {
        //                  #region Reverse RN

        //                  refID = pld2.c_rnno;

        //                  if (refID.StartsWith("CB", StringComparison.OrdinalIgnoreCase))
        //                  {
        //                    combo = (from q in db.LG_ComboHs
        //                             where (q.c_gdg == gudang) && (q.c_combono == refID)
        //                              && (q.c_iteno == field.Item) && (q.c_batch == field.Batch.Trim())
        //                             select q).Take(1).SingleOrDefault();

        //                    if (combo != null)
        //                    {
        //                      combo.n_gsisa += spAlloc;
        //                    }
        //                  }
        //                  else if (refID.StartsWith("RN", StringComparison.OrdinalIgnoreCase))
        //                  {
        //                    rnd1 = (from q in db.LG_RND1s
        //                            where (q.c_gdg == gudang) && (q.c_rnno == refID)
        //                             && (q.c_iteno == field.Item) && (q.c_batch == field.Batch.Trim())
        //                            select q).Take(1).SingleOrDefault();

        //                    if (rnd1 != null)
        //                    {
        //                      rnd1.n_gsisa += spAlloc;
        //                    }
        //                  }

        //                  #endregion

        //                  #region Old Coded

        //                  //rnd1 = (from q in db.LG_RND1s
        //                  //        where q.c_batch == pld2.c_batch && q.c_iteno == pld2.c_iteno &&
        //                  //        q.c_rnno == pld2.c_rnno && q.c_gdg == plh.c_gdg
        //                  //        select q).Distinct().Take(1).SingleOrDefault();

        //                  //spAlloc = ((pld2.n_qty.HasValue ? pld2.n_qty.Value : 0) >= rnQty ?
        //                  //              rnQty : (pld2.n_qty.HasValue ? pld2.n_qty.Value : 0));

        //                  //rnd1.n_gsisa += spAlloc;

        //                  //pld2.n_sisa = pld2.n_qty -= spAlloc;

        //                  //rnQty -= spAlloc;

        //                  //if ((pld2.n_qty.HasValue ? pld2.n_qty.Value : 0m) <= 0.00m)
        //                  //{
        //                  //  #region Delete PLD2

        //                  //  db.LG_PLD2s.DeleteOnSubmit(pld2);

        //                  //  #endregion

        //                  //  #region Log PLD3

        //                  //  listPld3.Add(new LG_PLD3()
        //                  //  {
        //                  //    c_batch = pld2.c_batch,
        //                  //    c_iteno = pld2.c_iteno,
        //                  //    c_plno = pld2.c_plno,
        //                  //    c_rnno = pld2.c_rnno,
        //                  //    c_spno = pld2.c_spno,
        //                  //    c_type = pld2.c_type,
        //                  //    n_qty = pld2.n_qty,
        //                  //    n_sisa = pld2.n_sisa,
        //                  //    v_ket_del = field.Keterangan,
        //                  //    v_type = "02",
        //                  //    c_entry = nipEntry,
        //                  //    d_entry = date,
        //                  //    c_type_dc = pld1.c_type_dc
        //                  //  });

        //                  //  #endregion
        //                  //}

        //                  //listResRND1.Add(rnd1);

        //                  #endregion

        //                  #region Log PLD3

        //                  listPld3.Add(new LG_PLD3()
        //                  {
        //                    c_batch = pld2.c_batch,
        //                    c_iteno = pld2.c_iteno,
        //                    c_plno = pld2.c_plno,
        //                    c_rnno = pld2.c_rnno,
        //                    c_spno = pld2.c_spno,
        //                    c_type = pld2.c_type,
        //                    n_qty = pld2.n_qty,
        //                    n_sisa = pld2.n_sisa,
        //                    v_ket_del = field.Keterangan,
        //                    v_type = "02",
        //                    c_entry = nipEntry,
        //                    d_entry = date,
        //                    c_type_dc = pld1.c_type_dc
        //                  });

        //                  #endregion

        //                  pld2.n_sisa = pld2.n_qty -= spAlloc;

        //                  rnQty -= spAlloc;

        //                  if (rnQty <= 0)
        //                  {
        //                    break;
        //                  }
        //                }
        //              }

        //              listPld2Copy.Clear();
        //            }

        //            #endregion

        //            #region Reverse PLD1

        //            pld1.c_type_dc = field.ConfirmType;

        //            pld1.n_sisa = pld1.n_qty -= spAlloc;

        //            #endregion

        //            #region Insert Log PLD3

        //            db.LG_PLD3s.InsertAllOnSubmit(listPld3.ToArray());

        //            #endregion
        //          }
        //        }

        //        #endregion
        //      }

        //      #region Old Coded

        //      //else if ((field != null) && (!field.IsNew) && field.IsDelete && (!field.IsModified))
        //      //{
        //      //  #region Cek SP

        //      //  pld1 = (from q in db.LG_PLD1s
        //      //          where q.c_batch == field.Batch && q.c_iteno == field.Item &&
        //      //          q.c_plno == plID && q.c_spno == field.NomorSP
        //      //          select q).Distinct().Take(1).SingleOrDefault();

        //      //  if (pld1 != null)
        //      //  {
        //      //    spQtyReloc = (pld1.n_qty.HasValue ? pld1.n_qty.Value : 0);
        //      //    spAlloc = (pld1.n_sisa.HasValue ? pld1.n_sisa.Value : 0);

        //      //    if (spAlloc.Equals(spQtyReloc))
        //      //    {
        //      //      #region Reverse SP

        //      //      spd1 = (from q in db.LG_SPD1s
        //      //              where q.c_spno == pld1.c_spno && q.c_iteno == pld1.c_iteno
        //      //              select q).Distinct().Take(1).SingleOrDefault();

        //      //      if (spd1 == null)
        //      //      {
        //      //        continue;
        //      //      }

        //      //      spd1.n_sisa += spAlloc;

        //      //      #endregion

        //      //      #region Reverse RN

        //      //      //listPld2 = (from q in db.LG_PLD2s
        //      //      //            where q.c_plno == plID && q.c_iteno == pld1.c_iteno &&
        //      //      //            q.c_batch == pld1.c_batch
        //      //      //            select q).Distinct().ToList();

        //      //      listPld2Copy = listPld2.FindAll(delegate(LG_PLD2 pld)
        //      //      {
        //      //        return field.Item.Trim().Equals((string.IsNullOrEmpty(pld.c_iteno) ? string.Empty : pld.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
        //      //          field.Batch.Trim().Equals((string.IsNullOrEmpty(pld.c_batch) ? string.Empty : pld.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
        //      //          field.NomorSP.Trim().Equals((string.IsNullOrEmpty(pld.c_spno) ? string.Empty : pld.c_spno.Trim()), StringComparison.OrdinalIgnoreCase);
        //      //      });

        //      //      if ((listPld2Copy != null) && (listPld2Copy.Count > 0))
        //      //      {
        //      //        for (nLoopC = 0; nLoopC < listPld2Copy.Count; nLoopC++)
        //      //        {
        //      //          pld2 = listPld2Copy[nLoopC];

        //      //          if (pld2 != null)
        //      //          {
        //      //            #region Reverse RN

        //      //            refID = pld2.c_rnno;

        //      //            if (refID.StartsWith("CB", StringComparison.OrdinalIgnoreCase))
        //      //            {
        //      //              combo = (from q in db.LG_ComboHs
        //      //                       where (q.c_gdg == gudang) && (q.c_combono == refID)
        //      //                        && (q.c_iteno == field.Item) && (q.c_batch == field.Batch.Trim())
        //      //                       select q).Take(1).SingleOrDefault();

        //      //              if (combo != null)
        //      //              {
        //      //                combo.n_gsisa += spAlloc;
        //      //              }
        //      //            }
        //      //            else if (refID.StartsWith("RN", StringComparison.OrdinalIgnoreCase))
        //      //            {
        //      //              rnd1 = (from q in db.LG_RND1s
        //      //                      where (q.c_gdg == gudang) && (q.c_rnno == refID)
        //      //                       && (q.c_iteno == pld1.c_iteno) && (q.c_batch == pld1.c_batch)
        //      //                      select q).Take(1).SingleOrDefault();

        //      //              if (rnd1 != null)
        //      //              {
        //      //                rnd1.n_gsisa += spAlloc;
        //      //              }
        //      //            }

        //      //            #endregion

        //      //            #region Old Coded

        //      //            //rnd1 = (from q in db.LG_RND1s
        //      //            //        where q.c_batch == pld2.c_batch && q.c_iteno == pld2.c_iteno &&
        //      //            //        q.c_rnno == pld2.c_rnno && q.c_gdg == plh.c_gdg
        //      //            //        select q).Distinct().Take(1).SingleOrDefault();

        //      //            //rnd1.n_gsisa += (pld2.n_qty);

        //      //            //listResRND1.Add(rnd1);

        //      //            #endregion

        //      //            #region Log PLD3

        //      //            listPld3.Add(new LG_PLD3()
        //      //            {
        //      //              c_batch = pld2.c_batch,
        //      //              c_iteno = pld2.c_iteno,
        //      //              c_plno = pld2.c_plno,
        //      //              c_rnno = pld2.c_rnno,
        //      //              c_spno = pld2.c_spno,
        //      //              c_type = pld2.c_type,
        //      //              n_qty = pld2.n_qty,
        //      //              n_sisa = pld2.n_sisa,
        //      //              v_ket_del = field.Keterangan,
        //      //              v_type = "03",
        //      //              c_entry = nipEntry,
        //      //              d_entry = date,
        //      //              c_type_dc = pld1.c_type_dc
        //      //            });

        //      //            #endregion
        //      //          }
        //      //        }

        //      //        #region Delete PLD2

        //      //        db.LG_PLD2s.DeleteAllOnSubmit(listPld2Copy.ToArray());

        //      //        listPld2Copy.Clear();

        //      //        #endregion
        //      //      }

        //      //      #endregion

        //      //      #region Delete PLD1

        //      //      db.LG_PLD1s.DeleteOnSubmit(pld1);

        //      //      #endregion
        //      //    }
        //      //  }

        //      //  #endregion
        //      //}

        //      #endregion
        //    }

        //    #region Clear

        //    if (listRN != null)
        //    {
        //      listRN.Clear();
        //    }
        //    if (listPld1 != null)
        //    {
        //      listPld1.Clear();
        //    }
        //    if (listPld2 != null)
        //    {
        //      listPld2.Clear();
        //    }
        //    if (listPld3 != null)
        //    {
        //      listPld3.Clear();
        //    }
        //    if (listResRND1 != null)
        //    {
        //      listResRND1.Clear();
        //    }

        //    #endregion

        //    #region Insert Log PLD3

        //    if (listPld3.Count > 0)
        //    {
        //      db.LG_PLD3s.InsertAllOnSubmit(listPld3.ToArray());

        //      listPld3.Clear();
        //    }

        //    #endregion
        //  }

        //  #endregion

        //  hasAnyChanges = true;

        //  #region Old Coded

        //  //if (structure.Fields.IsConfirm)
        //  //{
        //  //  if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
        //  //  {
        //  //    plh.v_ket = structure.Fields.Keterangan;
        //  //  }

        //  //  plh.l_confirm = structure.Fields.IsConfirm;

        //  //  plh.c_update = nipEntry;
        //  //  plh.d_update = date;

        //  //  #region Populate Detail

        //  //  if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
        //  //  {
        //  //    listRN = new List<string>();
        //  //    listPld1 = new List<LG_PLD1>();
        //  //    listPld2 = new List<LG_PLD2>();
        //  //    listPld3 = new List<LG_PLD3>();
        //  //    listResRND1 = new List<LG_RND1>();

        //  //    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
        //  //    {
        //  //      field = structure.Fields.Field[nLoop];

        //  //      if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified) && (field.Quantity > 0))
        //  //      {
        //  //        #region Cek SP

        //  //        spd1 = (from q in db.LG_SPD1s
        //  //                join q1 in db.LG_ORD2s on new { q.c_spno, q.c_iteno } equals new { q1.c_spno, q1.c_iteno }
        //  //                join q2 in db.LG_ORHs on q1.c_orno equals q2.c_orno
        //  //                where q.c_spno == field.NomorSP && q.c_iteno == field.Item && q2.c_type != "02"
        //  //                select q).Distinct().Take(1).SingleOrDefault();

        //  //        if ((spd1 != null) && (spd1.n_sisa.HasValue) && (spd1.n_sisa.Value > 0))
        //  //        {
        //  //          if (field.Quantity > spd1.n_sisa.Value)
        //  //          {
        //  //            spQty = spd1.n_sisa.Value;
        //  //          }
        //  //          else
        //  //          {
        //  //            spQty = field.Quantity;
        //  //          }

        //  //          spQtyReloc = spQty.Value;

        //  //          listResRND1 = (from q in db.LG_RND1s
        //  //                         join q1 in db.LG_RNHs on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
        //  //                         join q2 in db.LG_MsBatches on q.c_batch equals q2.c_batch
        //  //                         join q3 in db.LG_RND2s on new { q.c_gdg, q.c_rnno } equals new { q3.c_gdg, q3.c_rnno }
        //  //                         join q4 in db.LG_POD2s on new { q3.c_gdg, c_pono = q3.c_no } equals new { q4.c_gdg, q4.c_pono }
        //  //                         join q5 in db.LG_ORHs on new { q4.c_gdg, q4.c_orno } equals new { q5.c_gdg, q5.c_orno }
        //  //                         where q.c_gdg == gudang && q.c_batch == field.Batch && q.c_iteno == field.Item && q.n_gsisa > 0 && q5.c_type != "02"
        //  //                         orderby q2.d_expired descending, q1.d_rndate
        //  //                         select q).Distinct().ToList();

        //  //          #region Cek RN

        //  //          if (listResRND1.Count > 0)
        //  //          {
        //  //            for (nLoopC = 0; nLoopC < listResRND1.Count; nLoopC++)
        //  //            {
        //  //              rnd1 = listResRND1[nLoopC];

        //  //              if ((rnd1.n_gsisa.HasValue) && (rnd1.n_gsisa.Value > 0))
        //  //              {
        //  //                //rnQty = rnd1.n_gsisa.Value;

        //  //                if (rnd1.n_gsisa.Value >= spQtyReloc)
        //  //                {
        //  //                  rnQty = spQtyReloc;

        //  //                  rnd1.n_gsisa -= spQtyReloc;
        //  //                  spQtyReloc = 0;
        //  //                }
        //  //                else
        //  //                {
        //  //                  rnQty = rnd1.n_gsisa.Value;

        //  //                  spQtyReloc -= rnd1.n_gsisa.Value;
        //  //                  rnd1.n_gsisa = 0;
        //  //                }

        //  //                listPld2.Add(new LG_PLD2()
        //  //                {
        //  //                  c_plno = plID,
        //  //                  c_batch = field.Batch,
        //  //                  c_iteno = field.Item,
        //  //                  c_rnno = rnd1.c_rnno,
        //  //                  c_spno = field.NomorSP,
        //  //                  c_type = "01",
        //  //                  n_qty = rnQty,
        //  //                  n_sisa = rnQty
        //  //                });
        //  //              }

        //  //              if (spQtyReloc <= 0)
        //  //              {
        //  //                break;
        //  //              }
        //  //            }

        //  //            spAlloc = (spQty.Value - spQtyReloc);

        //  //            listPld1.Add(new LG_PLD1()
        //  //            {
        //  //              c_plno = plID,
        //  //              c_batch = field.Batch,
        //  //              c_iteno = field.Item,
        //  //              c_spno = field.NomorSP,
        //  //              c_type = "01",
        //  //              n_booked = spAlloc,
        //  //              n_qty = spAlloc,
        //  //              n_sisa = spAlloc
        //  //            });
        //  //          }

        //  //          #endregion

        //  //          if ((listPld1.Count > 0) && (listPld2.Count > 0))
        //  //          {
        //  //            if (spAlloc <= spd1.n_sisa.Value)
        //  //            {
        //  //              spd1.n_sisa -= spAlloc;
        //  //            }
        //  //            else
        //  //            {
        //  //              spd1.n_sisa -= 0;
        //  //            }
        //  //          }
        //  //        }

        //  //        if ((listPld1.Count > 0) && (listPld2.Count > 0))
        //  //        {
        //  //          #region Log PLD3

        //  //          for (nLoopC = 0; nLoopC < listPld2.Count; nLoopC++)
        //  //          {
        //  //            pld2 = listPld2[nLoopC];

        //  //            if (pld2 != null)
        //  //            {
        //  //              db.LG_PLD3s.InsertOnSubmit(new LG_PLD3()
        //  //              {
        //  //                c_batch = pld2.c_batch,
        //  //                c_iteno = pld2.c_iteno,
        //  //                c_plno = pld2.c_plno,
        //  //                c_rnno = pld2.c_rnno,
        //  //                c_spno = pld2.c_spno,
        //  //                c_type = pld2.c_type,
        //  //                n_qty = pld2.n_qty,
        //  //                n_sisa = pld2.n_sisa,
        //  //                c_entry = nipEntry,
        //  //                d_entry = date,
        //  //                v_ket_del = "Insert on Confirm",
        //  //                v_type = "01"
        //  //              });
        //  //            }
        //  //          }

        //  //          #endregion

        //  //          db.LG_PLD1s.InsertAllOnSubmit(listPld1.ToArray());
        //  //          db.LG_PLD2s.InsertAllOnSubmit(listPld2.ToArray());

        //  //          totalDetails++;
        //  //        }

        //  //        #endregion
        //  //      }
        //  //      else if ((field != null) && (!field.IsNew) && field.IsDelete && (!field.IsModified))
        //  //      {
        //  //        #region Cek SP

        //  //        pld1 = (from q in db.LG_PLD1s
        //  //                where q.c_batch == field.Batch && q.c_iteno == field.Item &&
        //  //                q.c_plno == plID && q.c_spno == field.NomorSP
        //  //                select q).Distinct().Take(1).SingleOrDefault();

        //  //        if (pld1 != null)
        //  //        {
        //  //          spQtyReloc = (pld1.n_qty.HasValue ? pld1.n_qty.Value : 0);
        //  //          spAlloc = (pld1.n_sisa.HasValue ? pld1.n_sisa.Value : 0);

        //  //          if (spAlloc.Equals(spQtyReloc))
        //  //          {
        //  //            #region Reverse SP

        //  //            spd1 = (from q in db.LG_SPD1s
        //  //                    where q.c_spno == pld1.c_spno && q.c_iteno == pld1.c_iteno
        //  //                    select q).Distinct().Take(1).SingleOrDefault();

        //  //            if (spd1 != null)
        //  //            {
        //  //              spd1.n_sisa += spAlloc;
        //  //            }

        //  //            #endregion

        //  //            #region Reverse RN

        //  //            listPld2 = (from q in db.LG_PLD2s
        //  //                        where q.c_plno == plID && q.c_iteno == pld1.c_iteno &&
        //  //                        q.c_batch == pld1.c_batch
        //  //                        select q).Distinct().ToList();


        //  //            if ((listPld2 != null) && (listPld2.Count > 0))
        //  //            {
        //  //              for (nLoopC = 0; nLoopC < listPld2.Count; nLoopC++)
        //  //              {
        //  //                pld2 = listPld2[nLoopC];

        //  //                if (pld2 != null)
        //  //                {
        //  //                  rnd1 = (from q in db.LG_RND1s
        //  //                          where q.c_batch == pld2.c_batch && q.c_iteno == pld2.c_iteno &&
        //  //                          q.c_rnno == pld2.c_rnno && q.c_gdg == plh.c_gdg
        //  //                          select q).Distinct().Take(1).SingleOrDefault();

        //  //                  rnd1.n_gsisa += (pld2.n_qty);

        //  //                  listResRND1.Add(rnd1);

        //  //                  #region Log PLD3

        //  //                  listPld3.Add(new LG_PLD3()
        //  //                  {
        //  //                    c_batch = pld2.c_batch,
        //  //                    c_iteno = pld2.c_iteno,
        //  //                    c_plno = pld2.c_plno,
        //  //                    c_rnno = pld2.c_rnno,
        //  //                    c_spno = pld2.c_spno,
        //  //                    c_type = pld2.c_type,
        //  //                    n_qty = pld2.n_qty,
        //  //                    n_sisa = pld2.n_sisa,
        //  //                    v_ket_del = field.Keterangan,
        //  //                    v_type = "03",
        //  //                    c_entry = nipEntry,
        //  //                    d_entry = date
        //  //                  });

        //  //                  #endregion
        //  //                }
        //  //              }

        //  //              #region Delete PLD2

        //  //              db.LG_PLD2s.DeleteAllOnSubmit(listPld2.ToArray());

        //  //              #endregion
        //  //            }

        //  //            #endregion

        //  //            #region Delete PLD1

        //  //            db.LG_PLD1s.DeleteOnSubmit(pld1);

        //  //            #endregion

        //  //            #region Insert Log PLD3

        //  //            db.LG_PLD3s.InsertAllOnSubmit(listPld3.ToArray());

        //  //            #endregion
        //  //          }
        //  //        }

        //  //        #endregion
        //  //      }
        //  //      else if ((field != null) && (!field.IsNew) && (!field.IsDelete) && field.IsModified)
        //  //      {
        //  //        #region Cek SP

        //  //        pld1 = (from q in db.LG_PLD1s
        //  //                where q.c_batch == field.Batch && q.c_iteno == field.Item &&
        //  //                q.c_plno == plID && q.c_spno == field.NomorSP
        //  //                select q).Distinct().Take(1).SingleOrDefault();

        //  //        if (pld1 != null)
        //  //        {
        //  //          spQtyReloc = (pld1.n_qty.HasValue ? pld1.n_qty.Value : 0);
        //  //          spAlloc = (pld1.n_sisa.HasValue ? pld1.n_sisa.Value : 0);

        //  //          if (spAlloc.Equals(spQtyReloc))
        //  //          {
        //  //            spQty = (spAlloc - field.Quantity);

        //  //            #region Reverse SP

        //  //            spd1 = (from q in db.LG_SPD1s
        //  //                    where q.c_spno == pld1.c_spno && q.c_iteno == pld1.c_iteno
        //  //                    select q).Distinct().Take(1).SingleOrDefault();

        //  //            if (spd1 != null)
        //  //            {
        //  //              spSisa = ((spd1.n_sisa.HasValue ? spd1.n_sisa.Value : 0) + (spQty.HasValue ? spQty.Value : 0));

        //  //              if (spSisa < 0)
        //  //              {
        //  //                continue;
        //  //              }

        //  //              spd1.n_sisa += spQty;
        //  //            }

        //  //            #endregion

        //  //            #region Reverse RN

        //  //            listPld2 = (from q in db.LG_PLD2s
        //  //                        where q.c_plno == plID && q.c_iteno == pld1.c_iteno &&
        //  //                        q.c_batch == pld1.c_batch
        //  //                        select q).Distinct().ToList();

        //  //            if ((listPld2 != null) && (listPld2.Count > 0))
        //  //            {
        //  //              rnQty = spQty.Value;

        //  //              for (nLoopC = (listPld2.Count - 1); nLoopC >= 0; nLoopC++)
        //  //              {
        //  //                pld2 = listPld2[nLoopC];

        //  //                if (pld2 != null)
        //  //                {
        //  //                  rnd1 = (from q in db.LG_RND1s
        //  //                          where q.c_batch == pld2.c_batch && q.c_iteno == pld2.c_iteno &&
        //  //                          q.c_rnno == pld2.c_rnno && q.c_gdg == plh.c_gdg
        //  //                          select q).Distinct().Take(1).SingleOrDefault();

        //  //                  spAlloc = ((pld2.n_qty.HasValue ? pld2.n_qty.Value : 0) >= rnQty ?
        //  //                                rnQty : (pld2.n_qty.HasValue ? pld2.n_qty.Value : 0));

        //  //                  rnd1.n_gsisa += spAlloc;

        //  //                  pld2.n_sisa = pld2.n_qty -= spAlloc;

        //  //                  rnQty -= spAlloc;

        //  //                  if ((pld2.n_qty.HasValue ? pld2.n_qty.Value : 0m) <= 0.00m)
        //  //                  {
        //  //                    #region Delete PLD2

        //  //                    db.LG_PLD2s.DeleteOnSubmit(pld2);

        //  //                    #endregion

        //  //                    #region Log PLD3

        //  //                    listPld3.Add(new LG_PLD3()
        //  //                    {
        //  //                      c_batch = pld2.c_batch,
        //  //                      c_iteno = pld2.c_iteno,
        //  //                      c_plno = pld2.c_plno,
        //  //                      c_rnno = pld2.c_rnno,
        //  //                      c_spno = pld2.c_spno,
        //  //                      c_type = pld2.c_type,
        //  //                      n_qty = pld2.n_qty,
        //  //                      n_sisa = pld2.n_sisa,
        //  //                      v_ket_del = field.Keterangan,
        //  //                      v_type = "02",
        //  //                      c_entry = nipEntry,
        //  //                      d_entry = date
        //  //                    });

        //  //                    #endregion
        //  //                  }

        //  //                  listResRND1.Add(rnd1);

        //  //                  if (rnQty <= 0)
        //  //                  {
        //  //                    break;
        //  //                  }
        //  //                }

        //  //                nLoopC++;

        //  //                if (nLoopC >= listPld2.Count)
        //  //                {
        //  //                  break;
        //  //                }
        //  //              }
        //  //            }

        //  //            #endregion

        //  //            #region Reverse PLD1

        //  //            pld1.n_sisa = pld1.n_qty -= spQty;

        //  //            #endregion

        //  //            #region Insert Log PLD3

        //  //            db.LG_PLD3s.InsertAllOnSubmit(listPld3.ToArray());

        //  //            #endregion
        //  //          }
        //  //        }

        //  //        #endregion
        //  //      }

        //  //      hasAnyChanges = true;

        //  //      #region Clear

        //  //      if (listRN != null)
        //  //      {
        //  //        listRN.Clear();
        //  //      }
        //  //      if (listPld1 != null)
        //  //      {
        //  //        listPld1.Clear();
        //  //      }
        //  //      if (listPld2 != null)
        //  //      {
        //  //        listPld2.Clear();
        //  //      }
        //  //      if (listPld3 != null)
        //  //      {
        //  //        listPld3.Clear();
        //  //      }
        //  //      if (listResRND1 != null)
        //  //      {
        //  //        listResRND1.Clear();
        //  //      }

        //  //      #endregion
        //  //    }
        //  //  }

        //  //  #endregion
        //  //}
        //  //else if (isConfirmed == (!structure.Fields.IsConfirm))
        //  //{
        //  //  plh.l_confirm = structure.Fields.IsConfirm;

        //  //  if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
        //  //  {
        //  //    plh.v_ket = structure.Fields.Keterangan;
        //  //  }

        //  //  plh.c_update = nipEntry;
        //  //  plh.d_update = date;

        //  //  hasAnyChanges = true;
        //  //}

        //  #endregion

        //  #endregion
        //}

        #endregion

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

        result = string.Format("ScmsSoaLibrary.Bussiness.Penjualan:PackingList - {0}", ex.Message);

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

    public string PackingListAuto(ScmsSoaLibrary.Parser.Class.PackingListAutoStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      LG_PLH plh = null;

      LG_SPD1 spd1 = null;
      LG_SPD2 spd2 = null;

      LG_RNH rnh = null;
      LG_RND1 rnd1 = null;

      #region New Stock Indra 20180305FM

      LG_DAILY_STOCK_v2 daily2 = null;
      LG_MOVEMENT_STOCK_v2 movement2 = null;

      #endregion

      ScmsSoaLibrary.Parser.Class.PackingListAutoStructureField field = null;
      string nipEntry = null;
      string plID = null,
        doID = null,
        tmp = null;
      string tmpCustomer = null;
      //  tmpNumberingDO = null;
      bool hasAnyChanges = false;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      //decimal? nQtyPL = 0;

      List<LG_PLH> listPlh = null;
      List<LG_PLD1> listPld1 = null;
      List<LG_PLD2> listPld2 = null;

      List<LG_SPH> listSPH = null;
      List<LG_SPD1> listSPD1 = null;
      List<LG_SPD2> listSPD2 = null;

      List<LG_DOH> listDOH = null;
      List<LG_DOD1> listDOD1 = null;
      List<LG_DOD2> listDOD2 = null;

      //List<string> listRN = null;
      //List<LG_RND1> listResRND1 = null;
      //List<LG_PLD3> listPld3 = null;
      //List<LG_DOD1> listDod1 = null;
      //List<LG_DOD2> lisDod2 = null;
      //List<LG_SPD1> listSpd1 = null;
      //List<LG_SPD2> listSpd2 = null;
      List<LG_PLD1_SUM_BYBATCH> listPLSum = null;

      LG_DOH doh = null;
      LG_SPH sph = null;
      FA_MasItm masitm = null;

      LG_PLD1_SUM_BYBATCH pld1Sum = null;

      char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '1' : structure.Fields.Gudang[0]);

      LG_PLD1 pld1 = null;
      LG_PLD2 pld2 = null;

      int nLoop = 0;

      string noSpAdj = null,
          noSP = null;

      decimal nPrice = 0,
        nDisc = 0,
        nBox = 0,
        koliKarton = 0,
        receh = 0;

      bool hasAnyInvalid = false;

      List<string> lstItems = null,
        lstSP = null, lstRN = null;

      List<FA_MasItm> lstFaItm = null;
      List<LG_SPD1> lstSPD1 = null;
      List<LG_RND1> lstRND1 = null;

      FA_MasItm faItm = null;

      IDictionary<string, string> dic = null;

      tmpCustomer = structure.Fields.Customer;

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }
      int totalDetails = 0;

      plID = (structure.Fields.PackingListID ?? string.Empty);

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          #region Validasi Indra 20171120

          if (!string.IsNullOrEmpty(plID))
          {
            result = "Nomor Packing List harus kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Customer))
          {
            result = "Nama cabang dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Gudang))
          {
            result = "Gudang dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, date))
          {
            result = "Packing List tidak dapat disimpan, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          #endregion

          listPlh = new List<LG_PLH>();
          listPld1 = new List<LG_PLD1>();
          listPld2 = new List<LG_PLD2>();

          listSPH = new List<LG_SPH>();
          listSPD1 = new List<LG_SPD1>();
          listSPD2 = new List<LG_SPD2>();

          listDOH = new List<LG_DOH>();
          listDOD1 = new List<LG_DOD1>();
          listDOD2 = new List<LG_DOD2>();

          //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "PLA");
          Constant.TRANSID = (from q in db.LG_PLHs
                              where q.d_pldate.Value.Month == date.Date.Month
                              select q.c_plno).Max();

          Constant.Gudang = gudang;

          plID = Commons.GenerateNumbering<LG_PLH>(db, "PL", '3', "08", date, "c_plno");

          //var RNSingle = (from q in db.LG_RNHs
          //                where q.c_gdg == gudang &&
          //                q.c_rnno == structure.Fields.NoRN
          //                select q).Take(1).SingleOrDefault();

          plh = new LG_PLH()
          {
            c_plno = plID,
            c_cusno = structure.Fields.Customer,
            c_entry = nipEntry,
            c_gdg = gudang,
            c_nosup = structure.Fields.Suplier,
            c_type = structure.Fields.Tipe,
            c_update = nipEntry,
            c_via = structure.Fields.Via,
            d_entry = date,
            d_pldate = date.Date,
            d_update = date,
            l_confirm = true,
            l_print = true,
            //v_ket = "Sys: Auto Generate",
            v_ket = structure.Fields.Keterangan,
            c_type_cat = string.Empty,
            c_plnum = Constant.NUMBERID_GUDANG,
          };

          //db.LG_PLHs.InsertOnSubmit(plh);

          listPlh.Add(plh);

          #region Old Code

          //db.SubmitChanges();

          //plh = (from q in db.LG_PLHs
          //       where q.v_ket == tmpNumbering
          //       select q).Take(1).SingleOrDefault();

          //if (!string.IsNullOrEmpty(plID))
          //{
          //  result = "Nomor Packing List Auto tidak dapat di raih.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  goto endLogic;
          //}

          //if (plh.c_plno.Equals("XXXXXXXXXX"))
          //{
          //  result = "Trigger Packing List Auto tidak aktif.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  goto endLogic;
          //}

          //plID = plh.c_plno;

          #endregion

          #region Insert Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            //listPld1 = new List<LG_PLD1>();
            //listRN = new List<string>();
            //listPld2 = new List<LG_PLD2>();
            //lisDod1 = new List<LG_DOD1>();
            ////lisDod2 = new List<LG_DOD2>();
            //listSpd1 = new List<LG_SPD1>();
            //listSpd2 = new List<LG_SPD2>();

            lstItems = structure.Fields.Field.GroupBy(x => x.Item).Select(y => y.Key).ToList();

            lstSP = structure.Fields.Field.GroupBy(x => x.NomorSP).Select(y => y.Key).ToList();

            lstRN = structure.Fields.Field.GroupBy(x => x.NomorRN).Select(y => y.Key).ToList();

            lstFaItm = (from q in db.FA_MasItms
                        where lstItems.Contains(q.c_iteno)
                        select q).Distinct().ToList();

            lstSPD1 = (from q in db.LG_SPHs
                       join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
                       where lstSP.Contains(q.c_spno) && (q1.n_spds > 0) && lstItems.Contains(q1.c_iteno)
                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                       select q1).Distinct().ToList();
            if (lstSPD1.Count() == 0)
            {
                if (db.Transaction != null)
                {
                    db.Transaction.Rollback();
                }
                rpe = ResponseParser.ResponseParserEnum.IsError;

                result = "SP direct shipment nya tidak ada";

                Logger.WriteLine(result, true);
                goto endLogic;
            }
            lstRND1 = (from q in db.LG_RNHs
                       join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                       where (q.c_gdg == gudang) && ((q.c_type == "01") && ((q.l_khusus.HasValue ? q.l_khusus.Value : false) == true)) && (q1.n_gsisa > 0)
                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false) && q.c_rnno == structure.Fields.NoRN
                       select q1).Distinct().ToList();

            //List<FA_DiscD> lstDiscItm = (from q in db.FA_DiscHes
            //                              join q1 in db.FA_DiscDs on q.c_nodisc equals q1.c_nodisc
            //                              where q.c_type == "03"
            //                                && q1.l_aktif == true && q1.l_status == true
            //                                && lstItems.Contains(q1.c_iteno)
            //                              select q1).ToList();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              if (hasAnyInvalid)
              {
                break;
              }

              if ((field != null) && field.IsNew && (!field.IsDelete) && (field.qtyRN > 0))
              {
                #region PL

                if (field.qtySPAdj > 0)
                {
                  #region SP ADJ

                  if (sph == null)
                  {
                    sph = PackingListSPAdj(db, field, structure, date);

                    listSPH.Add(sph);
                      
                    noSpAdj = sph.c_spno;
                  }

                  #region Detil

                  //nPrice = (from q in db.FA_MasItms
                  //          where q.c_iteno == field.Item && q.l_aktif == true && q.l_hide == false
                  //          select (q.n_salpri.HasValue ? q.n_salpri.Value : 0)).Take(1).SingleOrDefault();

                  faItm = lstFaItm.Find(delegate(FA_MasItm fam)
                  {
                    return (string.IsNullOrEmpty(fam.c_iteno) ? string.Empty : fam.c_iteno.Trim()).Equals(field.Item.Trim(), StringComparison.OrdinalIgnoreCase);
                  });

                  if (faItm == null)
                  {
                      nPrice = 0;
                      nDisc = 0;
                  }
                  else
                  {
                    nPrice = (faItm.n_salpri.HasValue ? faItm.n_salpri.Value : 0);
                    nDisc = (faItm.n_disc.HasValue ? faItm.n_disc.Value : 0);
                  }

                  spd1 = listSPD1.Find(delegate(LG_SPD1 spd)
                  {
                      return (string.IsNullOrEmpty(spd.c_spno) ? string.Empty : spd.c_spno.Trim()).Equals(noSpAdj, StringComparison.OrdinalIgnoreCase) &&
                        (string.IsNullOrEmpty(spd.c_iteno) ? string.Empty : spd.c_iteno.Trim()).Equals(field.Item, StringComparison.OrdinalIgnoreCase);
                      //((spd.n_sisa.HasValue ? spd.n_sisa.Value : 0) > 0);
                  });

              if (spd1 != null)
              {
                  listSPD1[listSPD1.Count -1].n_acc += field.qtySPAdj;
                  listSPD1[listSPD1.Count - 1].n_qty += field.qtySPAdj;
              }
              else
              {
                  spd1 = new LG_SPD1()
                  {
                      c_iteno = field.Item,
                      c_spno = noSpAdj,
                      c_type = "01",
                      n_acc = field.qtySPAdj,
                      n_qty = field.qtySPAdj,
                      n_salpri = nPrice,
                      n_sisa = 0,
                      v_ket = "Adjustment"
                  };
                  if (nDisc == 0)
                  {
                    spd2 = new LG_SPD2()
                    {
                      c_iteno = field.Item,
                      c_no = "??????????",
                      c_spno = noSpAdj,
                      c_type = "??",
                      n_discoff = 0,
                      n_discon = 0
                    };
                  }
                  else
                  {
                      spd2 = new LG_SPD2()
                      {
                          c_iteno = field.Item,
                          c_no = "XXXXXXXXXX",
                          c_spno = noSpAdj,
                          c_type = "03",
                          n_discoff = 0,
                          n_discon = nDisc
                      };
                  }

                  listSPD1.Add(spd1);
                  listSPD2.Add(spd2);
              }   
                  
                  //var qryDisc = (from q in db.FA_DiscHes
                  //               join q1 in db.FA_DiscDs on q.c_nodisc equals q1.c_nodisc
                  //               where q1.c_iteno == field.Item && q1.l_aktif == true && q1.l_status == true && q.c_type == "03"
                  //               select new
                  //               {
                  //                 q.c_nodisc,
                  //                 q.c_type,
                  //                 q1.n_discon
                  //               }).Take(1).SingleOrDefault();

                  

                  #endregion

                  if (noSpAdj != null)
                  {
                    pld1 = new LG_PLD1()
                    {
                      c_batch = field.Batch,
                      c_iteno = field.Item,
                      c_plno = plID,
                      c_spno = noSpAdj,
                      c_type = "01",
                      n_booked = field.qtySPAdj,
                      n_qty = field.qtySPAdj,
                      n_sisa = 0
                    };

                    pld2 = new LG_PLD2()
                    {
                      c_batch = field.Batch,
                      c_iteno = field.Item,
                      c_plno = plID,
                      c_rnno = structure.Fields.NoRN,
                      c_spno = noSpAdj,
                      c_type = "01",
                      n_qty = field.qtySPAdj,
                      n_sisa = field.qtySPAdj
                    };

                    #region New Stock Indra 20180305FM

                    if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                      plID,
                                                      structure.Fields.Tipe,
                                                      field.Item,
                                                      field.Batch,
                                                      field.qtySPAdj * -1,
                                                      0,
                                                      "KOSONG",
                                                      "02",
                                                      "01",
                                                      nipEntry,
                                                      "01")) == 0)
                    {
                        result = "Terdapat Kesalahan pada Item " + field.Item + " dengan Batch " + field.Batch + ". Harap Hubungi MIS.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                      goto endLogic;
                    }
                    
                    #endregion

                    rnh = (from q in db.LG_RNHs
                           where q.c_rnno == structure.Fields.NoRN
                           && q.c_gdg == gudang && q.l_khusus == true
                           select q).Take(1).SingleOrDefault();

                    if (rnh == null)
                    {
                        hasAnyInvalid = true;
                    }
                    else
                    {
                        rnh.l_khusus = false;
                    }

                    rnd1 = lstRND1.Find(delegate(LG_RND1 rnd)
                    {
                      return (string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno.Trim()).Equals(field.NomorRN, StringComparison.OrdinalIgnoreCase) &&
                        (string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()).Equals(field.Item, StringComparison.OrdinalIgnoreCase) &&
                        (string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()).Equals(field.Batch, StringComparison.OrdinalIgnoreCase) &&
                        ((rnd.n_gsisa.HasValue ? rnd.n_gsisa.Value : 0) > 0) &&
                        ((rnd.n_gsisa.HasValue ? rnd.n_gsisa.Value : 0) >= field.qtyRN);
                    });

                    if (rnd1 == null)
                    {
                      hasAnyInvalid = true;
                    }
                    else
                    {
                      hasAnyInvalid = ((spd1 == null) ||
                        (spd2 == null) || (pld1 == null) || (pld2 == null));

                      //listSpd1.Add(spd1);
                      //listSpd2.Add(spd2);

                      if (!hasAnyInvalid)
                      {
                        //listSPD1.Add(spd1);                        
                        //listSPD2.Add(spd2);

                        listPld1.Add(pld1);
                        listPld2.Add(pld2);

                        //db.LG_SPD1s.InsertOnSubmit(spd1);
                        //db.LG_SPD2s.InsertOnSubmit(spd2);

                        //db.LG_PLD1s.InsertOnSubmit(pld1);
                        //db.LG_PLD2s.InsertOnSubmit(pld2);

                        rnd1.n_gsisa -= field.qtyRN;

                        if (rnd1.n_gsisa.Value <= 0.00m)
                        {
                          lstRND1.Remove(rnd1);
                        }
                      }
                    }

                    //if (listPld2.Count > 0)
                    //{
                    //  rnd1 = (from q in db.LG_RND1s
                    //          where q.c_gdg == gudang && q.c_rnno == structure.Fields.NoRN
                    //          && q.c_iteno == field.Item && q.c_batch == field.Batch
                    //          select q).Take(1).SingleOrDefault();

                    //  rnd1.n_gsisa -= field.qtySPAdj;
                    //}
                  }
                  noSP = noSpAdj;
                  #endregion
                }
                else
                {
                  #region Sp Normal

                  spd1 = lstSPD1.Find(delegate(LG_SPD1 spd)
                  {
                    return (string.IsNullOrEmpty(spd.c_spno) ? string.Empty : spd.c_spno.Trim()).Equals(field.NomorSP, StringComparison.OrdinalIgnoreCase) &&
                      (string.IsNullOrEmpty(spd.c_iteno) ? string.Empty : spd.c_iteno.Trim()).Equals(field.Item, StringComparison.OrdinalIgnoreCase) &&
                      ((spd.n_spds.HasValue ? spd.n_spds.Value : 0) > 0);
                  });

                  if (spd1 != null)
                  {
                    pld1 = new LG_PLD1()
                    {
                      c_batch = field.Batch,
                      c_iteno = field.Item,
                      c_plno = plID,
                      c_spno = field.NomorSP,
                      c_type = "01",
                      n_booked = field.qtySP,
                      n_qty = field.qtySP,
                      n_sisa = 0
                    };

                    //spd1 = (from q in db.LG_SPD1s
                    //        where q.c_spno == field.NomorSP
                    //        && q.c_iteno == field.Item
                    //        select q).Take(1).SingleOrDefault();


                    spd1.n_spds -= field.qtySP;
                    noSP = field.NomorSP;
                    #region PLD2

                    pld2 = new LG_PLD2()
                    {
                      c_batch = field.Batch,
                      c_iteno = field.Item,
                      c_plno = plID,
                      c_rnno = structure.Fields.NoRN,
                      c_spno = field.NomorSP,
                      c_type = "01",
                      n_qty = field.qtySP,
                      n_sisa = field.qtySP
                    };

                    //rnd1 = (from q in db.LG_RND1s
                    //        where q.c_gdg == gudang && q.c_rnno == structure.Fields.NoRN
                    //        && q.c_iteno == field.Item && q.c_batch == field.Batch
                    //        select q).Take(1).SingleOrDefault();

                    //rnd1.n_gsisa -= field.qtySP;
                    rnh = (from q in db.LG_RNHs
                           where q.c_rnno == structure.Fields.NoRN
                           && q.c_gdg == gudang && q.l_khusus == true
                           select q).Take(1).SingleOrDefault();

                    if (rnh == null)
                    {
                        hasAnyInvalid = true;
                    }
                    else
                    {
                        rnh.l_khusus = false;
                    }

                    rnd1 = lstRND1.Find(delegate(LG_RND1 rnd)
                    {
                      return (string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno.Trim()).Equals(field.NomorRN, StringComparison.OrdinalIgnoreCase) &&
                        (string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()).Equals(field.Item, StringComparison.OrdinalIgnoreCase) &&
                        (string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim().ToUpper()).Equals(field.Batch.Trim().ToUpper(), StringComparison.OrdinalIgnoreCase) &&
                        ((rnd.n_gsisa.HasValue ? rnd.n_gsisa.Value : 0) > 0) &&
                        ((rnd.n_gsisa.HasValue ? rnd.n_gsisa.Value : 0) >= field.qtyRN);
                    });

                    if (rnd1 == null)
                    {
                      hasAnyInvalid = true;
                    }
                    else
                    {
                      hasAnyInvalid = ((pld1 == null) || (pld2 == null));

                      if (!hasAnyInvalid)
                      {
                        listPld1.Add(pld1);
                        listPld2.Add(pld2);

                        //db.LG_PLD1s.InsertOnSubmit(pld1);
                        //db.LG_PLD2s.InsertOnSubmit(pld2);

                        rnd1.n_gsisa -= field.qtyRN;

                        if (rnd1.n_gsisa.Value <= 0.00m)
                        {
                          lstRND1.Remove(rnd1);
                        }
                      }
                    }

                    #endregion

                    #region New Stock Indra 20180305FM

                    if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                    plID,
                                                    structure.Fields.Tipe,
                                                    field.Item,
                                                    field.Batch,
                                                    field.qtySP * -1,
                                                    0,
                                                    "KOSONG",
                                                    "02",
                                                    "01",
                                                    nipEntry,
                                                    "01")) == 0)
                    {
                        result = "Terdapat Kesalahan pada Item " + field.Item + " dengan Batch " + field.Batch + ". Harap Hubungi MIS.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    #endregion
                  }

                  #endregion
                }

                #endregion
              }
            }

            lstItems.Clear();
            lstSP.Clear();
            lstRN.Clear();

            lstFaItm.Clear();
            lstSPD1.Clear();
            lstRND1.Clear();

            if (!hasAnyInvalid)
            {
              #region DO

              //List<string> sItem = listPld1.Select(x => x.Item).ToList();

              #region Pin

              //listResPLD1 = (from q in db.LG_PLD1s
              //               where q.c_plno == structure.Fields.nopl && q.n_qty != 0
              //               && sItem.Contains(q.c_iteno)
              //               select q).ToList();

              int nLoopPl = listPld1.Count();
              int Hit = 1, lBacth = 0, nHit = 0;
              decimal value = 0,
                      valueS = 0;
              while (Hit <= nLoopPl)
              {
                lBacth = listPld1[nHit].c_batch.Trim().Length;

                lBacth -= 1;

                while (lBacth > -1)
                {
                  char[] sc = listPld1[nHit].c_batch.Trim().Substring(lBacth, 1).ToCharArray();
                  foreach (char letter in sc)
                  {
                    value = Convert.ToInt64(letter);
                    valueS += value;
                  }
                  lBacth -= 1;
                }

                valueS += listPld1[nHit].n_qty.Value;

                //Indra 20171018
                //valueS += decimal.Parse(listPld1[nHit].c_iteno);

                lBacth = listPld1[nHit].c_iteno.Trim().Length;

                lBacth -= 1;

                while (lBacth > -1)
                {
                    char[] sc = listPld1[nHit].c_iteno.Trim().Substring(lBacth, 1).ToCharArray();
                    foreach (char letter in sc)
                    {
                        value = Convert.ToInt64(letter);
                        valueS += value;
                    }
                    lBacth -= 1;
                }
                Hit++;
                nHit++;
              }

              valueS = Math.Round(valueS);

              

              #endregion

              listPLSum =
                listPld1.GroupBy(x => new { x.c_plno, x.c_iteno })
                .Select(x => new LG_PLD1_SUM_BYBATCH() { c_plno = x.Key.c_plno, c_iteno = x.Key.c_iteno, n_qty = x.Sum(y => (y.n_qty.HasValue ? y.n_qty.Value : 0)) }).ToList();

              doh = PackingListDO(db, gudang, plID, structure, date);

              doh.c_pin = valueS.ToString();

              //var lstOutlet = (from q in db.LG_SPHs where q.c_spno == noSP select new { q.c_outlet }).ToList();

              //if (lstOutlet.Count > 0)
              //{
              //    doh.c_outlet = lstOutlet[0].c_outlet;
              //}

              if (doh == null)
              {
                hasAnyInvalid = true;
              }
              else
              {
                listDOH.Add(doh);

                doID = doh.c_dono;

                #region Old Coded

                //if (PL2.Count > 0)
                //{
                //  #region DOD2

                //  for (nLoopC = 0; nLoopC < PL2.Count; nLoopC++)
                //  {
                //    lisDod2.Add(new LG_DOD2()
                //    {
                //      c_dono = doID,
                //      c_iteno = PL2[nLoopC].c_iteno,
                //      c_via = structure.Fields.Via,
                //      n_qty = PL2[nLoopC].qty,
                //      n_sisa = PL2[nLoopC].qty,
                //      c_batch = PL2[nLoopC].c_batch
                //    });
                //  }

                //  #endregion
                //}

                #endregion

                if (listPLSum.Count > 0)
                {
                  #region DOD1

                  for (nLoop = 0; nLoop < listPLSum.Count; nLoop++)
                  {
                    pld1Sum = listPLSum[nLoop];

                    if (pld1Sum != null)
                    {
                      listDOD1.Add(new LG_DOD1()
                      {
                        c_dono = doID,
                        c_iteno = pld1Sum.c_iteno,
                        c_via = structure.Fields.Via,
                        n_qty = pld1Sum.n_qty,
                        n_sisa = pld1Sum.n_qty
                      });
                    }
                  }

                                    listPLSum.Clear();

                                    #endregion
                                }

                                listPLSum =
                                 listPld1.GroupBy(x => new { x.c_plno, x.c_iteno, x.c_batch })
                                 .Select(x => new LG_PLD1_SUM_BYBATCH()
                                 {
                                     c_plno = x.Key.c_plno,
                                     c_iteno = x.Key.c_iteno,
                                     c_batch = x.Key.c_batch,
                                     n_qty = x.Sum(y => (y.n_qty.HasValue ? y.n_qty.Value : 0))
                                 }).ToList();
                                if (listPLSum.Count > 0)
                                {
                                    #region DOD2

                    for (nLoop = 0; nLoop < listPLSum.Count; nLoop++)
                    {
                        pld1Sum = listPLSum[nLoop];

                        if (pld1Sum != null)
                        {
                            listDOD2.Add(new LG_DOD2()
                            {
                                c_dono = doID,
                                c_iteno = pld1Sum.c_iteno,
                                c_via = structure.Fields.Via,
                                c_batch = pld1Sum.c_batch,
                                n_qty = pld1Sum.n_qty,
                                n_sisa = pld1Sum.n_qty
                            });
                        }
                    }

                    listPLSum.Clear();

                    #endregion
                }

                //#region DOD2

                //for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                //{
                //    field = structure.Fields.Field[nLoop];

                //    if (hasAnyInvalid)
                //    {
                //        break;
                //    }
                //    listDOD2.Add(new LG_DOD2()
                //    {
                //        c_dono = doID,
                //        c_via = structure.Fields.Via,
                //        c_iteno = field.Item,
                //        c_batch = field.Batch,
                //        n_qty = field.qtySPAdj,
                //        n_sisa = field.qtySPAdj
                //    });
                //}
                //#endregion

              }

              #endregion

              if (!hasAnyInvalid)
              {
                  if (string.IsNullOrEmpty(plh.v_ket))
                  {
                      plh.v_ket = string.Concat(plID, " - ", doID);
                      doh.v_ket = string.Concat("Auto DO : ", plID, " - ", doID);
                  }
                  else
                  {
                      doh.v_ket = plh.v_ket + " (" + string.Concat("Auto DO : ", plID, " - ", doID) + ")";
                      plh.v_ket = plh.v_ket + " (" + string.Concat(plID, " - ", doID) + ")";
                  }
                //doh = (from q in db.LG_DOHs
                //       where q.c_dono == doID
                //       select q).Take(1).SingleOrDefault();
                  
                //if (listPld1.Count > 0 && listPld2.Count > 0 &&
                //    lisDod1.Count > 0)
                //{
                //  db.LG_PLD1s.InsertAllOnSubmit(listPld1.ToArray());
                //  db.LG_PLD2s.InsertAllOnSubmit(listPld2.ToArray());
                //  db.LG_DOD1s.InsertAllOnSubmit(lisDod1.ToArray());

                if (sph != null)
                {
                  sph.v_ket = string.Concat("Auto Adjustment : ", plID, " - ", doID);
                }

                //  //db.LG_DOD2s.InsertAllOnSubmit(lisDod2.ToArray());

                //  listPld1.Clear();
                //  listPld2.Clear();
                //  lisDod1.Clear();

                //  //lisDod2.Clear();
                //}
                //if (listSpd1.Count > 0 && listSpd2.Count > 0)
                //{
                //  db.LG_SPD1s.InsertAllOnSubmit(listSpd1.ToArray());
                //  db.LG_SPD2s.InsertAllOnSubmit(listSpd2.ToArray());

                //  listSpd1.Clear();
                //  listSpd2.Clear();
                //}

                #region Insert All

                if ((listPlh != null) && (listPlh.Count > 0))
                {
                  db.LG_PLHs.InsertAllOnSubmit(listPlh.ToArray());
                  listPlh.Clear();
                }

                if ((listPld1 != null) && (listPld1.Count > 0))
                {
                  db.LG_PLD1s.InsertAllOnSubmit(listPld1.ToArray());
                  listPld1.Clear();
                }

                if ((listPld2 != null) && (listPld2.Count > 0))
                {
                  db.LG_PLD2s.InsertAllOnSubmit(listPld2.ToArray());
                  listPld2.Clear();
                }

                if ((listSPH != null) && (listSPH.Count > 0))
                {
                  db.LG_SPHs.InsertAllOnSubmit(listSPH.ToArray());
                  listSPH.Clear();
                }

                if ((listSPD1 != null) && (listSPD1.Count > 0))
                {
                  db.LG_SPD1s.InsertAllOnSubmit(listSPD1.ToArray());
                  listSPD1.Clear();
                }

                if ((listSPD2 != null) && (listSPD2.Count > 0))
                {
                  db.LG_SPD2s.InsertAllOnSubmit(listSPD2.ToArray());
                  listSPD2.Clear();
                }

                if ((listDOH != null) && (listDOH.Count > 0))
                {
                  db.LG_DOHs.InsertAllOnSubmit(listDOH.ToArray());
                  listDOH.Clear();
                }

                if (listDOD1 != null)
                {
                  db.LG_DOD1s.InsertAllOnSubmit(listDOD1.ToArray());
                  listDOD1.Clear();
                }

                if (listDOD2 != null)
                {
                    db.LG_DOD2s.InsertAllOnSubmit(listDOD2.ToArray());
                    listDOD2.Clear();
                }

                if (doh != null)
                {
                    doh.n_karton = koliKarton;
                    doh.n_receh = receh;
                }

                #endregion
              }
            }
          }

          #endregion

          if (hasAnyInvalid)
          {
            result = "Satu atau lebih data ada yang tidak valid, mohon proses ulang dahulu.";

            hasAnyChanges = false;
          }
          else
          {
            dic = new Dictionary<string, string>();

            //tmp = (from q in db.LG_DatSups
            //           where q.c_nosup == RNSingle.c_from
            //           select q.v_nama.Trim()).Take(1).SingleOrDefault();

            dic.Add("PL", plID);
            dic.Add("Tanggal", date.ToString("yyyyMMdd"));
            //dic.Add("Prinsipal", tmp);

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

          if (!string.IsNullOrEmpty(tmpCustomer))
          {
              //Indra 20170613
              //Commons.RunningGenerateFJ(db, tmpCustomer, doID, doID, nipEntry);
              Commons.RunningGenerateFJ(db, tmpCustomer, doID, doID, nipEntry, 0);
          }

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

        result = string.Format("ScmsSoaLibrary.Bussiness.Penjualan:PackingListAuto - {0}", ex.StackTrace);

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

    public string PackingListMasterBox(ScmsSoaLibrary.Parser.Class.PackingListStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false,
        isConfirmMode = false;

      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      LG_PLH plh = null;

      LG_SPD1 spd1 = null;

      LG_RND1 rnd1 = null;

      LG_ComboH combo = null;

      ScmsSoaLibrary.Parser.Class.PackingListStructureField field = null;
      string nipEntry = null;
      string plIDBoxes = null,
        plIDReceh = null,
        refID = null,
        sItemAndBatch = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      //decimal? spQty = 0;
      decimal spQtyReloc = 0,
        rnQty = 0,
        spAlloc = 0,
        spSisa = 0,
        totalCurrentStock = 0,
        nQtyTemp = 0,
        nQtyPlAlloc = 0;

      PLClassComponent spac = null;

      List<PLClassComponent> listSPAC = null,
        listSPACTemp = null;

      List<LG_PLD1> listPld1 = null;
      List<LG_PLD2> listPld2 = null,
        listPld2Copy = null;
      List<LG_PLD3> listPld3 = null;
      List<LG_PLD1> listPldReceh1 = null;
      List<LG_PLD2> listPldReceh2 = null;

      List<string> listRN = null;
      //List<LG_RND1> listResRND1 = null;

      Dictionary<string, List<PLClassComponent>> dicItemStock = null;

      

      char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '1' : structure.Fields.Gudang[0]);

      LG_PLD1 pld1 = null;
      LG_PLD2 pld2 = null;

      LG_PLD1 pldReceh1 = null;
      LG_PLD2 pldReceh2 = null;

      #region New Stock Indra 20180305FM

      LG_DAILY_STOCK_v2 daily2 = null;
      LG_MOVEMENT_STOCK_v2 movement2 = null;

      #endregion

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
      //bool isConfirmed = false;
      bool isHeaderBox = false,
        isHeaderReceh = false;


      string plID = (structure.Fields.PackingListID ?? string.Empty);

      isConfirmMode = structure.Method.Equals("ModifyConfirm", StringComparison.OrdinalIgnoreCase);

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          #region Validasi 20171120

          if (!string.IsNullOrEmpty(plID))
          {
            result = "Nomor Packing List harus kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Suplier))
          {
            result = "Nama pemasok dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Customer))
          {
            result = "Nama cabang dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Gudang))
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
            result = "Packing List tidak dapat disimpan, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          #endregion

          #region Insert Detail

          List<string> plhMulti = new List<string>();

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listRN = new List<string>();
            listPld1 = new List<LG_PLD1>();
            listPld2 = new List<LG_PLD2>();
            listPldReceh1 = new List<LG_PLD1>();
            listPldReceh2 = new List<LG_PLD2>();

            dicItemStock = new Dictionary<string, List<PLClassComponent>>(StringComparer.OrdinalIgnoreCase);

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0) && (field.isBox))
              {
                if (!isHeaderBox)
                {
                  Constant.Gudang = gudang;

                  plIDBoxes = Commons.GenerateNumbering<LG_PLH>(db, "PL", '3', "08", date, "c_plno");

                  plh = new LG_PLH()
                  {
                    c_plno = plIDBoxes,
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
                    v_ket = structure.Fields.Keterangan,
                    c_type_cat = structure.Fields.TypeKategori,
                    c_type_lat = structure.Fields.Lantai,
                    c_plnum = Constant.NUMBERID_GUDANG,
                    c_kddivpri = (structure.Fields.DivPriID == " " ? null : structure.Fields.DivPriID),
                  };

                  plhMulti.Add(plIDBoxes);
                  db.LG_PLHs.InsertOnSubmit(plh);
                  isHeaderBox = true;
                }

                #region Cek SP

                spd1 = (from q in db.LG_SPD1s
                        join q1 in
                          (from sq in db.LG_ORHs
                           join sq1 in db.LG_ORD2s on new { sq.c_gdg, sq.c_orno } equals new { sq1.c_gdg, sq1.c_orno }
                           where sq.c_type == "02" && sq1.c_spno == field.NomorSP && sq1.c_iteno == field.Item
                           select new
                           {
                             sq.c_gdg,
                             sq1.c_spno,
                             sq1.c_iteno
                           }) on new { q.c_spno, q.c_iteno } equals new { q1.c_spno, q1.c_iteno } into q_1
                        from qSQ in q_1.DefaultIfEmpty()
                        where q.c_spno == field.NomorSP && q.c_iteno == field.Item //&& qORH.c_type == "02"
                        select q).Distinct().Take(1).SingleOrDefault();

                if (spd1 != null)
                {
                  spSisa = (spd1.n_sisa.HasValue ? spd1.n_sisa.Value : 0);

                  if (spSisa <= 0)
                  {
                    continue;
                  }

                  listSPAC = new List<PLClassComponent>();

                  sItemAndBatch = field.Item.Trim() + field.Batch.Trim();

                  if (dicItemStock.ContainsKey(sItemAndBatch))
                  {
                    listSPACTemp = dicItemStock[sItemAndBatch];
                  }
                  else
                  {
                    listSPACTemp = (from q in ScmsSoaLibrary.Modules.GlobalQuery.ViewStockLiteContains(db, gudang, field.Item)
                                    //join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                    //join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                    //from qBat in q_2.DefaultIfEmpty()
                                    //where (q.n_gsisa > 0) && (q.c_batch == field.Batch)
                                    where (q.n_gsisa != 0) && (q.c_batch == field.Batch)
                                    //orderby qBat.d_expired ascending
                                    select new PLClassComponent()
                                    {
                                      RefID = q.c_no,
                                      SignID = (q.c_table == null ? string.Empty : q.c_table.Trim()),
                                      Item = q.c_iteno,
                                      Qty = q.n_gsisa,
                                      //Supplier = q1.c_nosup,
                                      //Box = (q1.n_box.HasValue ? q1.n_box.Value : 0),
                                      BatchID = q.c_batch.Trim()
                                    }).Distinct().ToList();


                    dicItemStock.Add(sItemAndBatch, listSPACTemp);
                  }

                  totalCurrentStock = listSPACTemp.Sum(t => t.Qty);

                  #region Recalculate

                  if (totalCurrentStock > 0)
                  {
                    listSPAC = listSPACTemp.FindAll(delegate(PLClassComponent plcc)
                    {
                      if (totalCurrentStock < 0)
                      {
                        return false;
                      }
                      else if (plcc.Qty <= 0)
                      {
                        return false;
                      }
                      else if (!plcc.BatchID.Equals(field.Batch, StringComparison.OrdinalIgnoreCase))
                      {
                        return false;
                      }

                      return true;
                    });
                  }
                  #endregion

                  if (listSPAC.Count > 0)
                  {
                    if (totalCurrentStock <= 0)
                    {
                      continue;
                    }

                    nQtyTemp = field.Quantity;

                    #region New Stock Indra 20180305FM

                    if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                    plIDBoxes,
                                                    structure.Fields.TypePackingList,
                                                    field.Item,
                                                    field.Batch,
                                                    nQtyTemp * -1,
                                                    0,
                                                    "KOSONG",
                                                    "02",
                                                    "01",
                                                    nipEntry,
                                                    "01")) == 0)
                    {
                        result = "Terdapat Kesalahan pada Item " + field.Item + " dengan Batch " + field.Batch + ". Harap Hubungi MIS.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    #endregion

                    for (nLoopC = 0; nLoopC < listSPAC.Count; nLoopC++)
                    {
                      spac = listSPAC[nLoopC];
                      spAlloc = 0;

                      if (nQtyTemp > 0)
                      {

                        #region Expand Data

                        if (spac.SignID.Equals("CB", StringComparison.OrdinalIgnoreCase))
                        {
                          combo = (from q in db.LG_ComboHs
                                   where (q.c_gdg == gudang) && (q.c_combono == spac.RefID)
                                     //&& (q.c_iteno == field.Item) && (q.c_batch == field.Batch)
                                    && (q.c_iteno == field.Item) && (q.c_batch == spac.BatchID)
                                    && (q.n_gsisa > 0)
                                   select q).Take(1).SingleOrDefault();

                          if (combo != null)
                          {
                            //spAlloc = (spSisa > spac.Qty ? spac.Qty : spSisa);

                            spAlloc = (spSisa > field.Quantity ? field.Quantity : spSisa);
                            spAlloc = (spac.Qty > spAlloc ? spAlloc : spac.Qty);
                            spAlloc = (totalCurrentStock > spAlloc ? spAlloc : totalCurrentStock);

                            refID = combo.c_combono;

                            nQtyPlAlloc = (spAlloc > nQtyTemp ? nQtyTemp : spAlloc);
                            totalCurrentStock -= nQtyPlAlloc;

                            spac.Qty -= nQtyPlAlloc;
                            
                            combo.n_gsisa -= nQtyPlAlloc;

                            nQtyTemp -= nQtyPlAlloc;
                          }
                          else
                          {
                            spAlloc = 0;
                          }
                        }
                        else if (spac.SignID.Equals("RN", StringComparison.OrdinalIgnoreCase))
                        {
                          rnd1 = (from q in db.LG_RND1s
                                  where (q.c_gdg == gudang) && (q.c_rnno == spac.RefID)
                                    && (q.c_iteno == field.Item) && (q.c_batch == field.Batch)
                                    && (q.n_gsisa > 0)
                                  select q).Take(1).SingleOrDefault();

                          if (rnd1 != null)
                          {
                            //spAlloc = (spSisa > spac.Qty ? spac.Qty : spSisa);

                            spAlloc = (spSisa > field.Quantity ? field.Quantity : spSisa);
                            spAlloc = (spac.Qty > spAlloc ? spAlloc : spac.Qty);
                            spAlloc = (totalCurrentStock > spAlloc ? spAlloc : totalCurrentStock);

                            refID = rnd1.c_rnno;

                            nQtyPlAlloc = (spAlloc > nQtyTemp ? nQtyTemp : spAlloc);

                            totalCurrentStock -= nQtyPlAlloc;

                            spac.Qty -= nQtyPlAlloc;
                            
                            rnd1.n_gsisa -= nQtyPlAlloc;

                            nQtyTemp -= nQtyPlAlloc;
                          }
                          else
                          {
                            spAlloc = 0;
                          }
                        }
                        else
                        {
                          spAlloc = 0;
                        }

                        if (spac.Qty <= 0)
                        {
                          listSPACTemp.Remove(spac);
                        }

                        #endregion

                        if (spAlloc > 0)
                        {
                          #region Populate PLD

                          pld1 = listPld1.Find(delegate(LG_PLD1 pld)
                          {
                            return field.Item.Equals(pld.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                              field.Batch.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
                              //spac.BatchID.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
                              field.NomorSP.Equals(pld.c_spno, StringComparison.OrdinalIgnoreCase);
                          });

                          if (pld1 == null)
                          {
                            listPld1.Add(new LG_PLD1()
                            {
                              c_plno = plIDBoxes,
                              c_iteno = field.Item,
                              c_batch = field.Batch,
                              //c_batch = spac.BatchID,
                              c_spno = field.NomorSP,
                              c_type = "01",
                              n_booked = nQtyPlAlloc,
                              n_qty = nQtyPlAlloc,
                              n_sisa = nQtyPlAlloc,
                              l_expired = field.isED,
                              v_ket_ed = field.isED ? field.accket : null,
                              c_acc_ed = field.isED ? nipEntry : null
                            });
                          }
                          else
                          {
                            pld1.n_booked = pld1.n_qty = pld1.n_sisa += nQtyPlAlloc;
                          }

                          pld2 = listPld2.Find(delegate(LG_PLD2 pld)
                          {
                            return field.Item.Equals(pld.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                              field.Batch.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
                              //spac.BatchID.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
                              field.NomorSP.Equals(pld.c_spno, StringComparison.OrdinalIgnoreCase) &&
                              refID.Equals(pld.c_rnno, StringComparison.OrdinalIgnoreCase);
                          });

                          if (pld2 == null)
                          {
                            listPld2.Add(new LG_PLD2()
                            {
                              c_plno = plIDBoxes,
                              c_iteno = field.Item,
                              c_batch = field.Batch,
                              //c_batch = spac.BatchID,
                              c_spno = field.NomorSP,
                              c_type = "01",
                              c_rnno = refID,
                              n_qty = nQtyPlAlloc,
                              n_sisa = nQtyPlAlloc
                            });
                          }
                          else
                          {
                            pld2.n_qty = pld2.n_sisa += nQtyPlAlloc;
                          }

                          spSisa -= nQtyPlAlloc;
                          spd1.n_sisa = spSisa;

                          #endregion
                        }

                        if (spSisa <= 0)
                        {
                          break;
                        }
                        else if (totalCurrentStock <= 0)
                        {
                          break;
                        }
                      }
                    }

                    listSPAC.Clear();

                    totalDetails++;
                  }

                }

                #endregion
              }
              else if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0) && (!field.isBox))
              {
                if (!isHeaderReceh)
                {
                  Constant.Gudang = gudang;

                  plIDReceh = Commons.GenerateNumbering<LG_PLH>(db, "PL", '3', "08", date, "c_plno");

                  plh = new LG_PLH()
                  {
                    c_plno = plIDReceh,
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
                    v_ket = structure.Fields.Keterangan,
                    c_type_cat = structure.Fields.TypeKategori,
                    c_type_lat = structure.Fields.Lantai,
                    c_plnum = Constant.NUMBERID_GUDANG,
                    c_kddivpri = (structure.Fields.DivPriID == " " ? null : structure.Fields.DivPriID),
                  };

                    db.LG_PLHs.InsertOnSubmit(plh);

                    plhMulti.Add(plIDReceh);

                    isHeaderReceh = true;

                }

                #region Cek SP

                spd1 = (from q in db.LG_SPD1s
                        join q1 in
                          (from sq in db.LG_ORHs
                           join sq1 in db.LG_ORD2s on new { sq.c_gdg, sq.c_orno } equals new { sq1.c_gdg, sq1.c_orno }
                           where sq.c_type == "02" && sq1.c_spno == field.NomorSP && sq1.c_iteno == field.Item
                           select new
                           {
                             sq.c_gdg,
                             sq1.c_spno,
                             sq1.c_iteno
                           }) on new { q.c_spno, q.c_iteno } equals new { q1.c_spno, q1.c_iteno } into q_1
                        from qSQ in q_1.DefaultIfEmpty()
                        where q.c_spno == field.NomorSP && q.c_iteno == field.Item //&& qORH.c_type == "02"
                        select q).Distinct().Take(1).SingleOrDefault();

                if (spd1 != null)
                {
                  spSisa = (spd1.n_sisa.HasValue ? spd1.n_sisa.Value : 0);

                  if (spSisa <= 0)
                  {
                    continue;
                  }

                  listSPAC = new List<PLClassComponent>();

                  sItemAndBatch = field.Item.Trim() + field.Batch.Trim();

                  if (dicItemStock.ContainsKey(sItemAndBatch))
                  {
                    listSPACTemp = dicItemStock[sItemAndBatch];
                  }
                  else
                  {
                    listSPACTemp = (from q in ScmsSoaLibrary.Modules.GlobalQuery.ViewStockLiteContains(db, gudang, field.Item)
                                    //join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                    //join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                    //from qBat in q_2.DefaultIfEmpty()
                                    //where (q.n_gsisa > 0) && (q.c_batch == field.Batch)
                                    where (q.n_gsisa != 0) && (q.c_batch == field.Batch)
                                    //orderby qBat.d_expired ascending
                                    select new PLClassComponent()
                                    {
                                      RefID = q.c_no,
                                      SignID = (q.c_table == null ? string.Empty : q.c_table.Trim()),
                                      Item = q.c_iteno,
                                      Qty = q.n_gsisa,
                                      //Supplier = q1.c_nosup,
                                      //Box = (q1.n_box.HasValue ? q1.n_box.Value : 0),
                                      BatchID = q.c_batch.Trim()
                                    }).Distinct().ToList();


                    dicItemStock.Add(sItemAndBatch, listSPACTemp);
                  }

                  totalCurrentStock = listSPACTemp.Sum(t => t.Qty);

                  #region Recalculate

                  if (totalCurrentStock > 0)
                  {
                    listSPAC = listSPACTemp.FindAll(delegate(PLClassComponent plcc)
                    {
                      if (totalCurrentStock < 0)
                      {
                        return false;
                      }
                      else if (plcc.Qty <= 0)
                      {
                        return false;
                      }
                      else if (!plcc.BatchID.Equals(field.Batch, StringComparison.OrdinalIgnoreCase))
                      {
                        return false;
                      }

                      return true;
                    });
                  }
                  #endregion

                  if (listSPAC.Count > 0)
                  {
                    if (totalCurrentStock <= 0)
                    {
                      continue;
                    }

                    nQtyTemp = field.Quantity;

                    #region New Stock Indra 20180305FM

                    if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                    plIDReceh,
                                                    structure.Fields.TypePackingList,
                                                    field.Item,
                                                    field.Batch,
                                                    nQtyTemp * -1,
                                                    0,
                                                    "KOSONG",
                                                    "02",
                                                    "01",
                                                    nipEntry,
                                                    "01")) == 0)
                    {
                        result = "Terdapat Kesalahan pada Item " + field.Item + " dengan Batch " + field.Batch + ". Harap Hubungi MIS.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    #endregion

                    for (nLoopC = 0; nLoopC < listSPAC.Count; nLoopC++)
                    {
                      spac = listSPAC[nLoopC];
                      spAlloc = 0;

                      #region Expand Data

                      if (spac.SignID.Equals("CB", StringComparison.OrdinalIgnoreCase))
                      {
                        combo = (from q in db.LG_ComboHs
                                 where (q.c_gdg == gudang) && (q.c_combono == spac.RefID)
                                   //&& (q.c_iteno == field.Item) && (q.c_batch == field.Batch)
                                  && (q.c_iteno == field.Item) && (q.c_batch == spac.BatchID)
                                  && (q.n_gsisa > 0)
                                 select q).Take(1).SingleOrDefault();

                        if (combo != null)
                        {
                          //spAlloc = (spSisa > spac.Qty ? spac.Qty : spSisa);

                          spAlloc = (spSisa > field.Quantity ? field.Quantity : spSisa);
                          spAlloc = (spac.Qty > spAlloc ? spAlloc : spac.Qty);
                          spAlloc = (totalCurrentStock > spAlloc ? spAlloc : totalCurrentStock);

                          refID = combo.c_combono;

                          nQtyPlAlloc = (spAlloc > nQtyTemp ? nQtyTemp : spAlloc);
                          totalCurrentStock -= nQtyPlAlloc;

                          spac.Qty -= nQtyPlAlloc;
                          combo.n_gsisa -= nQtyPlAlloc;

                          nQtyTemp -= nQtyPlAlloc;
                        }
                        else
                        {
                          spAlloc = 0;
                        }
                      }
                      else if (spac.SignID.Equals("RN", StringComparison.OrdinalIgnoreCase) || spac.SignID.Equals("RR", StringComparison.OrdinalIgnoreCase))
                      {
                        rnd1 = (from q in db.LG_RND1s
                                where (q.c_gdg == gudang) && (q.c_rnno == spac.RefID)
                                  && (q.c_iteno == field.Item) && (q.c_batch == field.Batch)
                                  && (q.n_gsisa > 0)
                                select q).Take(1).SingleOrDefault();

                        if (rnd1 != null)
                        {
                          //spAlloc = (spSisa > spac.Qty ? spac.Qty : spSisa);

                          spAlloc = (spSisa > field.Quantity ? field.Quantity : spSisa);
                          spAlloc = (spac.Qty > spAlloc ? spAlloc : spac.Qty);
                          spAlloc = (totalCurrentStock > spAlloc ? spAlloc : totalCurrentStock);

                          refID = rnd1.c_rnno;

                          nQtyPlAlloc = (spAlloc > nQtyTemp ? nQtyTemp : spAlloc);
                          totalCurrentStock -= nQtyPlAlloc;

                          spac.Qty -= nQtyPlAlloc;
                          rnd1.n_gsisa -= nQtyPlAlloc;

                          nQtyTemp -= nQtyPlAlloc;
                        }
                        else
                        {
                          spAlloc = 0;
                        }
                      }
                      else
                      {
                        spAlloc = 0;
                      }

                      if (spac.Qty <= 0)
                      {
                        listSPACTemp.Remove(spac);
                      }

                      #endregion

                      if (spAlloc > 0)
                      {
                        #region Populate PLD

                        pldReceh1 = listPldReceh1.Find(delegate(LG_PLD1 pld)
                        {
                          return field.Item.Equals(pld.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                            field.Batch.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
                            //spac.BatchID.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
                            field.NomorSP.Equals(pld.c_spno, StringComparison.OrdinalIgnoreCase);
                        });

                        if (pldReceh1 == null)
                        {
                          listPldReceh1.Add(new LG_PLD1()
                          {
                            c_plno = plIDReceh,
                            c_iteno = field.Item,
                            c_batch = field.Batch,
                            //c_batch = spac.BatchID,
                            c_spno = field.NomorSP,
                            c_type = "01",
                            n_booked = nQtyPlAlloc,
                            n_qty = nQtyPlAlloc,
                            n_sisa = nQtyPlAlloc,
                            l_expired = field.isED,
                            v_ket_ed = field.isED ? field.accket : null,
                            c_acc_ed = field.isED ? nipEntry : null
                          });
                        }
                        else
                        {
                          pldReceh1.n_booked = pldReceh1.n_qty = pldReceh1.n_sisa += nQtyPlAlloc;
                        }

                        pldReceh2 = listPldReceh2.Find(delegate(LG_PLD2 pld)
                        {
                          return field.Item.Equals(pld.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                            field.Batch.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
                            //spac.BatchID.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
                            field.NomorSP.Equals(pld.c_spno, StringComparison.OrdinalIgnoreCase) &&
                            refID.Equals(pld.c_rnno, StringComparison.OrdinalIgnoreCase);
                        });

                        if (pldReceh2 == null)
                        {
                          listPldReceh2.Add(new LG_PLD2()
                          {
                            c_plno = plIDReceh,
                            c_iteno = field.Item,
                            c_batch = field.Batch,
                            //c_batch = spac.BatchID,
                            c_spno = field.NomorSP,
                            c_type = "01",
                            c_rnno = refID,
                            n_qty = nQtyPlAlloc,
                            n_sisa = nQtyPlAlloc
                          });
                        }
                        else
                        {
                          pldReceh2.n_qty = pldReceh2.n_sisa += nQtyPlAlloc;
                        }

                        spSisa -= nQtyPlAlloc;
                        spd1.n_sisa = spSisa;

                        #endregion
                      }

                      if (spSisa <= 0)
                      {
                        break;
                      }
                      else if (totalCurrentStock <= 0)
                      {
                        break;
                      }
                    }

                    listSPAC.Clear();

                    totalDetails++;
                  }

                }

                #endregion
              }
            }

            #region Cleaning

            foreach (KeyValuePair<string, List<PLClassComponent>> kvp in dicItemStock)
            {
              kvp.Value.Clear();
            }

            dicItemStock.Clear();

            #endregion

            if ((listPld1 != null) && (listPld1.Count > 0))
            {
              db.LG_PLD1s.InsertAllOnSubmit(listPld1.ToArray());
              db.LG_PLD1s.InsertAllOnSubmit(listPldReceh1.ToArray());
              listPldReceh1.Clear();
              listPld1.Clear();
            }

            if ((listPld2 != null) && (listPld2.Count > 0))
            {
              db.LG_PLD2s.InsertAllOnSubmit(listPld2.ToArray());
              db.LG_PLD2s.InsertAllOnSubmit(listPldReceh2.ToArray());
              listPld2.Clear();
              listPldReceh2.Clear();
            }
          }

          #endregion

          dic = new Dictionary<string, string>();
          IDictionary<string, object> dicMulti = new Dictionary<string, object>();

          if (totalDetails > 0)
          {
            dic.Add("PL1", plIDBoxes);
            dic.Add("PL2", plIDReceh);
            dic.Add("NIP", nipEntry);
            dic.Add("Tanggal", date.ToString("yyyyMMdd"));

            result = string.Format("Total {0} detail(s)", totalDetails);

            hasAnyChanges = true;
          }
          else
          {
            hasAnyChanges = false;

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            result = "Detail tidak dapat di proses, cek sisa SP, OR dan RN.";
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
        rpe = ResponseParser.ResponseParserEnum.IsError;

        result = string.Format("ScmsSoaLibrary.Bussiness.Penjualan:PackingList - {0}", ex.Message);

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

    public string DOPackingList(ScmsSoaLibrary.Parser.Class.DoPLStructure structure)
    {
      return DOPackingList(structure, null);
    }

    public string DOPackingList(ScmsSoaLibrary.Parser.Class.DoPLStructure structure, ORMDataContext dbContext)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      string result = null;

      bool isContexted = false;
      ORMDataContext db = null;

      if (dbContext == null)
      {
        db = new ORMDataContext(Functionals.ActiveConnectionString);
      }
      else
      {
        isContexted = true;
        db = dbContext;
      }

      LG_DOH doh = null;

      LG_PLD1 pld1 = null;

      ScmsSoaLibrary.Parser.Class.DoPLStructureField field = null;

      string nipEntry = null;
      string doID = null,
        custNo = null,
        plID = null,
        itemID = null;
      //string tmpNumbering = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      //decimal? plQty = 0;
      decimal plQtyReloc = 0,
          koliKarton = 0,
          receh = 0;

      List<LG_DOD1> listDod1 = null;
      List<LG_DOD2> listDod2 = null;
      List<string> listPl = null;
      List<LG_PLD1> listResPLD1 = null;
      List<LG_PLD1> listResPLD1Temp = null;
      List<LG_DOD3> listDod3 = null;
      List<LG_PLD1_SUM_BYBATCH> listSumPLD1ByBatch = null;
      //List<LG_PLD1_SUM_BYBATCH> listSumPLD1ByBatchSum = null;
      LG_PLD1_SUM_BYBATCH pld1Sum = null;

      LG_DOH doCheck = null;
      LG_DOD1 dod1 = null;
      LG_DOD2 dod2 = null;
      LG_DOD3 dod3 = null;

      //LG_PLH plh = null;
      //LG_PLD1_SUM_BYBATCH pldByBatch = null;

      char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '1' : structure.Fields.Gudang[0]);

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

      string strPinCode = null,
          noWP = null,
          tipePL = null;

      int totalDetails = 0;
      bool hasAnyChanges = false,
        isSent = false,
        isSendMethod = false;
      //string gudangPL;

      doID = (structure.Fields.DOid ?? string.Empty);

      try
      {
        if (!isContexted)
        {
          db.Connection.Open();

          db.Transaction = db.Connection.BeginTransaction();
        }

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          if (!string.IsNullOrEmpty(doID))
          {
            result = "Nomor DO List harus kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Customer))
          {
            result = "Nama Cabang dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.nopl))
          {
            result = "NO PL dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, date))
          {
            result = "DO tidak dapat disimpan, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          
          //Indra 20180905FM
          //string[] typePL = new string[] { "01", "04", "05"};
          string[] typePL = new string[] { "01", "04", "05", "06"};

          tipePL = (from q in db.LG_PLHs
                    where q.c_plno == structure.Fields.nopl
                    select q.c_type).Take(1).SingleOrDefault();

          if (typePL.Contains(tipePL))
          {
              string[] typeWP = new string[] { "01", "02" };
              noWP = (from q in db.SCMS_STHs
                      join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                      where q1.c_no == structure.Fields.nopl && typeWP.Contains(q.c_type)
                      select q.c_nodoc).Take(1).SingleOrDefault();

              if (string.IsNullOrEmpty(noWP))
              {
                  result = "PL belum discan Team Picker. Mohon dikembalikan ke Team Picker";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  if (db.Transaction != null)
                  {
                      db.Transaction.Rollback();
                  }

                  goto endLogic;
              }

              noWP = (from q in db.SCMS_STHs
                      join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                      where q1.c_no == structure.Fields.nopl && q.c_type == "03"
                      select q.c_nodoc).Take(1).SingleOrDefault();

              if (string.IsNullOrEmpty(noWP))
              {
                  result = "PL belum discan Team Checker. Mohon dikembalikan ke Team Checker";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  if (db.Transaction != null)
                  {
                      db.Transaction.Rollback();
                  }

                  goto endLogic;
              }
          }

          strPinCode = Functionals.GeneratedRandomPinId(8, string.Empty);

          Constant.TRANSID = (from q in db.LG_DOHs
                              where q.d_dodate.Value.Month == date.Date.Month
                              select q.c_dono).Max();

          doID = Commons.GenerateNumbering<LG_DOH>(db, "DO", '3', "09", date, "c_dono");

          List<string> sItem = structure.Fields.Field.Select(x => x.Item).ToList();

          #region Pin

          listResPLD1 = (from q in db.LG_PLD1s
                         where q.c_plno == structure.Fields.nopl && q.n_qty != 0
                         && sItem.Contains(q.c_iteno)
                         select q).ToList();

          int nLoopPl = listResPLD1.Count();
          int Hit = 1, lBacth = 0, nHit = 0;
          decimal value = 0,
                  valueS = 0;
          while (Hit <= nLoopPl)
          {
            lBacth = listResPLD1[nHit].c_batch.Trim().Length;

            lBacth -= 1;

            while (lBacth > -1)
            {
              char[] sc = listResPLD1[nHit].c_batch.Trim().Substring(lBacth, 1).ToCharArray();
              foreach (char letter in sc)
              {
                value = Convert.ToInt64(letter);
                valueS += value;
              }
              lBacth -= 1;
            }

            valueS += listResPLD1[nHit].n_qty.Value;

            //valueS += decimal.Parse(listResPLD1[nHit].c_iteno); //error by suwandi
            char[] item = listResPLD1[nHit].c_iteno.ToCharArray();
            foreach (char letter in item)
            {
                value = Convert.ToInt64(letter);
                valueS += value;
            }

            Hit++;
            nHit++;
          }

          if (listResPLD1.Count > 0)
          {
            listResPLD1.Clear();

            valueS = Math.Round(valueS);
          }

          #endregion

          doh = new LG_DOH()
          {
            c_dono = doID,
            d_dodate = date.Date,
            c_gdg = gudang,
            c_type = "01",
            c_cusno = structure.Fields.Customer,
            c_plno = structure.Fields.nopl,
            c_via = structure.Fields.Via,
            v_ket = structure.Fields.Keterangan,
            c_pin = valueS.ToString(),
            l_status = false,
            l_print = false,
            l_send = false,
            l_sent = false,
            c_entry = nipEntry,
            d_entry = date,
            c_update = nipEntry,
            d_update = date,
            l_auto = false,
            l_wpdc = false,
            c_rnno = null,
            l_delete = false,
            l_receipt = false,
            v_ket_mark = null,
            
          };

          db.LG_DOHs.InsertOnSubmit(doh);

          

          #region Insert Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            totalDetails = 0;

            listPl = new List<string>();
            listDod1 = new List<LG_DOD1>();
            listDod2 = new List<LG_DOD2>();

            if (isContexted)
            {
              for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
              {
                field = structure.Fields.Field[nLoop];

                doCheck = new LG_DOH();
                doCheck = (from q in db.LG_DOHs
                           join q1 in db.LG_DOD1s on q.c_dono equals q1.c_dono
                           where q.c_plno == structure.Fields.nopl && q1.c_iteno == field.Item
                           select q).Take(1).SingleOrDefault();

                if (doCheck != null)
                {
                    result = "No.PL " + structure.Fields.nopl + " dengan Item " + field.Item + " sudah pernah dibuatkan dengan No.DO : " + doCheck.c_dono;

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                listDod1.Add(new LG_DOD1()
                {
                  c_dono = doID,
                  c_iteno = field.Item,
                  c_via = "00",
                  n_qty = field.Quantity,
                  n_sisa = field.Quantity
                });

                totalDetails++;
              }
              #region DOD2

                            listSumPLD1ByBatch =
                               listResPLD1.GroupBy(x => new { x.c_plno, x.c_iteno, x.c_batch })
                               .Select(x => new LG_PLD1_SUM_BYBATCH()
                               {
                                   c_plno = x.Key.c_plno,
                                   c_iteno = x.Key.c_iteno,
                                   c_batch = x.Key.c_batch,
                                   n_qty = x.Sum(y => (y.n_qty.HasValue ? y.n_qty.Value : 0))
                               }).ToList();
                            if (listSumPLD1ByBatch.Count > 0)
                            {
                                for (nLoop = 0; nLoop < listSumPLD1ByBatch.Count; nLoop++)
                                {
                                    pld1Sum = listSumPLD1ByBatch[nLoop];

                        if (pld1Sum != null)
                        {
                            listDod2.Add(new LG_DOD2()
                            {
                                c_dono = doID,
                                c_iteno = pld1Sum.c_iteno,
                                c_via = structure.Fields.Via,
                                c_batch = pld1Sum.c_batch,
                                n_qty = pld1Sum.n_qty,
                                n_sisa = pld1Sum.n_qty
                            });
                        }
                    }

                    listSumPLD1ByBatch.Clear();
                }

              #endregion
            }
            else
            {
              #region Non Context

              listResPLD1 = (from q in db.LG_PLD1s
                             where q.c_plno == structure.Fields.nopl
                             select q).Distinct().ToList();

              for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
              {
                field = structure.Fields.Field[nLoop];

                  doCheck = new LG_DOH();
                  doCheck = (from q in db.LG_DOHs 
                             join q1 in db.LG_DOD1s on q.c_dono equals q1.c_dono
                             where q.c_plno == structure.Fields.nopl && q1.c_iteno == field.Item
                             select q).Take(1).SingleOrDefault();

                  if (doCheck != null)
                  {
                      result = "No.PL " + structure.Fields.nopl + " dengan Item " + field.Item + " sudah pernah dibuatkan dengan No.DO : " + doCheck.c_dono;

                      rpe = ResponseParser.ResponseParserEnum.IsFailed;

                      if (db.Transaction != null)
                      {
                          db.Transaction.Rollback();
                      }

                      goto endLogic;
                  }

                if ((field != null) && field.IsNew && (field.Quantity > 0))
                {
                  listResPLD1Temp = listResPLD1.FindAll(delegate(LG_PLD1 pld)
                  {
                    return field.Item.Equals((string.IsNullOrEmpty(pld.c_iteno) ? string.Empty : pld.c_iteno), StringComparison.OrdinalIgnoreCase);
                  });

                  for (nLoopC = 0; nLoopC < listResPLD1Temp.Count; nLoopC++)
                  {
                    pld1 = listResPLD1Temp[nLoopC];

                    dod1 = listDod1.Find(delegate(LG_DOD1 dod)
                    {
                      return field.Item.Equals(dod.c_iteno);
                    });
                    
                    plQtyReloc = (pld1.n_qty.HasValue ? pld1.n_qty .Value : 0);

                    if (dod1 == null)
                    {
                      dod1 = new LG_DOD1()
                      {
                        c_dono = doID,
                        c_iteno = field.Item,
                        c_via = structure.Fields.Via,
                        n_qty = plQtyReloc,
                        n_sisa = plQtyReloc
                      };

                      listDod1.Add(dod1);
                    }
                    else
                    {
                      dod1.n_qty = dod1.n_sisa += plQtyReloc;
                    }

                    pld1.n_sisa = 0;


                    totalDetails++;
                  }

                  listResPLD1Temp.Clear();
                }
              }

              #region DOD2

              listSumPLD1ByBatch =
                 listResPLD1.GroupBy(x => new { x.c_plno, x.c_iteno, x.c_batch })
                 .Select(x => new LG_PLD1_SUM_BYBATCH()
                 {
                     c_plno = x.Key.c_plno,
                     c_iteno = x.Key.c_iteno,
                     c_batch = x.Key.c_batch,
                     n_qty = x.Sum(y => (y.n_qty.HasValue ? y.n_qty.Value : 0))
                 }).ToList();
              if (listSumPLD1ByBatch.Count > 0)
              {
                  for (nLoop = 0; nLoop < listSumPLD1ByBatch.Count; nLoop++)
                  {
                      pld1Sum = listSumPLD1ByBatch[nLoop];

                      if (pld1Sum != null)
                      {
                          dod2 = new LG_DOD2()
                          {
                              c_dono = doID,
                              c_iteno = pld1Sum.c_iteno,
                              c_via = structure.Fields.Via,
                              c_batch = pld1Sum.c_batch.Trim(),
                              n_qty = pld1Sum.n_qty,
                              n_sisa = pld1Sum.n_qty
                          };

                          listDod2.Add(dod2);
                      }
                  }

                  listSumPLD1ByBatch.Clear();
              }

              #endregion

              if (listResPLD1 != null)
              {
                listResPLD1.Clear();
              }

              #endregion

              #region Old Coded

              //#region Non Context

              //listResPLD1 = (from q in db.LG_PLD1s
              //               where q.c_plno == structure.Fields.nopl
              //               select q).Distinct().ToList();

              //listSumPLD1ByBatch = listResPLD1.GroupBy(x => new
              //{
              //  x.c_plno,
              //  x.c_iteno
              //}).Select(y => new LG_PLD1_SUM_BYBATCH()
              //{
              //  c_plno = y.Key.c_plno,
              //  //c_batch = y.Key.c_batch,
              //  c_iteno = y.Key.c_iteno,
              //  n_qty = y.Sum(z => (z.n_qty.HasValue ? z.n_qty.Value : 0))
              //}).ToList();

              //for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
              //{
              //  field = structure.Fields.Field[nLoop];

              //  if ((field != null) && field.IsNew && (field.Quantity > 0))
              //  {
              //    pldByBatch = listSumPLD1ByBatch.Find(delegate(LG_PLD1_SUM_BYBATCH pld)
              //    {
              //      return pld.c_plno.Equals(structure.Fields.nopl, StringComparison.OrdinalIgnoreCase) &&
              //        pld.c_iteno.Equals(field.Item, StringComparison.OrdinalIgnoreCase);
              //    });

              //    if (pldByBatch != null)
              //    {
              //      #region DOD1

              //      doQty = pldByBatch.n_qty;

              //      listDod1.Add(new LG_DOD1()
              //      {
              //        c_dono = doID,
              //        c_iteno = field.Item,
              //        c_via = "00",
              //        n_qty = doQty,
              //        n_sisa = doQty
              //      });

              //      #region Kurang Sisa PL

              //      listResPLD1Temp = listResPLD1.FindAll(delegate(LG_PLD1 pld)
              //      {
              //        return pld.c_plno.Equals(structure.Fields.nopl, StringComparison.OrdinalIgnoreCase) &&
              //          pld.c_iteno.Equals(field.Item, StringComparison.OrdinalIgnoreCase);
              //      });

              //      for (nLoopC = 0; nLoopC < listResPLD1Temp.Count; nLoopC++)
              //      {
              //        pld1 = listResPLD1Temp[nLoopC];

              //        plQtyReloc = (pld1.n_sisa.HasValue ? pld1.n_sisa.Value : 0);

              //        if (plQtyReloc > 0)
              //        {
              //          plAlloc = ((plQtyReloc > doQty) ? doQty : plQtyReloc);

              //          doQty -= plAlloc;

              //          pld1.n_sisa -= plAlloc;

              //          totalDetails++;
              //        }
              //      }

              //      listResPLD1Temp.Clear();

              //      #endregion

              //      #endregion
              //    }

              //    #region Old Coded

              //    ////var t = (from q in db.LG_PLD1s
              //    ////         where q.c_iteno == field.Item
              //    ////         && q.c_plno == structure.Fields.nopl
              //    ////         group q by new { q.c_plno, q.c_iteno, q.c_batch } into sumdat
              //    ////         select new
              //    ////         {
              //    ////           c_plno = sumdat.Key.c_plno,
              //    ////           c_iteno = sumdat.Key.c_iteno,
              //    ////           c_batch = sumdat.Key.c_batch,
              //    ////           n_qty = sumdat.Sum(x => x.n_qty)
              //    ////         }).AsQueryable().ToList();

              //    //pld1 = listResPLD1.Find(delegate(LG_PLD1 pld)
              //    //{
              //    //  return pld.c_plno.Equals(structure.Fields.nopl, StringComparison.OrdinalIgnoreCase) &&
              //    //    pld.c_iteno.Equals(field.Item, StringComparison.OrdinalIgnoreCase);
              //    //});

              //    ////pld1 = (from q in db.LG_PLD1s
              //    ////        where q.c_plno == structure.Fields.nopl 
              //    ////          && q.c_iteno == field.Item
              //    ////        select q).Distinct().Take(1).SingleOrDefault();

              //    //for (nLoopC = 0; nLoopC < listSumPLD1ByBatch.Count; nLoopC++)
              //    //{
              //    //  string batch = t[nLoopC].c_batch ?? string.Empty;

              //    //  var pldVar = (from q in t
              //    //                where q.c_plno == structure.Fields.nopl && q.c_iteno == field.Item
              //    //                 && q.c_batch == batch
              //    //                select q).Distinct().Take(1).SingleOrDefault();

              //    //  listDod2.Add(new LG_DOD2
              //    //    {
              //    //      c_dono = doID,
              //    //      c_iteno = field.Item,
              //    //      c_via = structure.Fields.Via,
              //    //      c_batch = pldVar.c_batch,
              //    //      n_qty = pldVar.n_qty,
              //    //      n_sisa = pldVar.n_qty
              //    //    });

              //    //}

              //    //db.LG_DOD2s.InsertAllOnSubmit(listDod2.ToArray());

              //    //decimal Qty = listDod2.GroupBy(x => x.c_iteno).Sum(s => s.Sum(x => x.n_sisa).Value);

              //    //listDod1.Add(new LG_DOD1
              //    //{
              //    //  c_dono = doID,
              //    //  c_via = structure.Fields.Via,
              //    //  c_iteno = field.Item,
              //    //  n_qty = Qty,
              //    //  n_sisa = Qty
              //    //});

              //    //db.LG_DOD1s.InsertAllOnSubmit(listDod1.ToArray());

              //    //listDod1.Clear();
              //    //listDod2.Clear();

              //    #endregion
              //  }
              //}

              //if (listSumPLD1ByBatch != null)
              //{
              //  listSumPLD1ByBatch.Clear();
              //}

              //if (listResPLD1 != null)
              //{
              //  listResPLD1.Clear();
              //}

              //#endregion              

              #endregion
            }

            if (doh != null)
            {
                doh.n_karton = koliKarton;
                doh.n_receh = receh;
            }

            if (listDod1.Count > 0 && listDod2.Count > 0)
            {
              db.LG_DOD1s.InsertAllOnSubmit(listDod1.ToArray());
              db.LG_DOD2s.InsertAllOnSubmit(listDod2.ToArray());

              listDod1.Clear();
              listDod2.Clear();
            }
          }

          #endregion

          dic = new Dictionary<string, string>();

          if (totalDetails > 0)
          {
            dic.Add("DO", doID);
            dic.Add("Tanggal", date.ToString("yyyyMMdd"));
            dic.Add("Gudang", gudang.ToString());
            dic.Add("Pin", strPinCode);

            hasAnyChanges = true;
          }

          #endregion
        }
        else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete

          if (string.IsNullOrEmpty(doID))
          {
            result = "Nomor DO dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          doh = (from q in db.LG_DOHs
                 where q.c_dono == doID
                 select q).Take(1).SingleOrDefault();

          if (doh == null)
          {
            result = "Nomor DO tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (doh.l_delete.HasValue && doh.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor DO yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, doh.d_dodate))
          {
            result = "DO tidak dapat dihapus, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.HasFJ(db, doID))
          {
            result = "DO tidak dapat dihapus, karena sudah dibuat faktur penjualan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (doh.l_send.HasValue && doh.l_send.Value)
          {
            result = "Tidak dapat menghapus nomor DO yang sudah terproses.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.HasExp(db, doID))
          {
            result = "DO yang sudah terdapat Ekspedisi tidak dapat diubah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          plID = (string.IsNullOrEmpty(doh.c_plno) ? string.Empty : doh.c_plno.Trim());

          doh.c_update = nipEntry;
          doh.d_update = date;

          doh.l_delete = true;
          doh.v_ket_mark = structure.Fields.Keterangan;

          #region Pupulate Detail

          #region old Code

          //if ((listDod2 != null) && (listDod2.Count > 0))
          //{
          //  for (nLoopC = 0; nLoopC < listDod2.Count; nLoopC++)
          //  {
          //    dod2 = listDod2[nLoopC];
          //    if (dod2 != null)
          //    {
          //      listDod3.Add(new LG_DOD3()
          //        {
          //          c_dono = doID,
          //          d_dodate = date,
          //          c_gdg = doh.c_gdg,
          //          c_cusno = doh.c_cusno,
          //          c_typeH = doh.c_type,
          //          c_plno = doh.c_plno,
          //          c_via = doh.c_via,
          //          v_ket = doh.v_ket,
          //          c_iteno = dod2.c_iteno,
          //          c_batch = dod2.c_batch,
          //          c_entry = doh.c_entry,
          //          c_update = doh.c_update,
          //          d_entry = doh.d_entry,
          //          d_update = doh.d_update,
          //          n_qty = dod2.n_qty,
          //          n_sisa = dod2.n_sisa
          //        });

          //    }
          //  }
          //  db.LG_DOD2s.DeleteAllOnSubmit(listDod2);

          //  listDod2.Clear();
          //}
          //if (listDod3.Count > 0)
          //{
          //  db.LG_DOD3s.InsertAllOnSubmit(listDod3.ToArray());

          //  listDod3.Clear();
          //  db.SubmitChanges();
          //}

          #endregion

          #region Old Coded

          //if ((listDod2 != null) && (listDod2.Count > 0))
          //{
          //  listDod3 = new List<LG_DOD3>();

          //  listDod2 = (from q in db.LG_DOD2s
          //              where q.c_dono == doID
          //              select q).ToList();


          //  var td = (from q in db.LG_DOD2s
          //            where q.c_dono == doID
          //            group q by new { q.c_dono, q.c_iteno } into sumdat
          //            select new
          //            {
          //              c_dono = sumdat.Key.c_dono,
          //              c_iteno = sumdat.Key.c_iteno,
          //              n_qty = sumdat.Sum(x => x.n_qty),
          //              n_sisa = sumdat.Sum(x => x.n_sisa),
          //            }).AsQueryable().ToList();


          //  for (nLoop = 0; nLoop < td.Count; nLoop++)
          //  {

          //    string item = td[nLoop].c_iteno ?? string.Empty;

          //    var ItemDO = (from q in db.LG_DOD2s
          //                  where q.c_dono == doID && q.c_iteno == item
          //                  group q by new { q.c_dono, q.c_iteno } into sumdat
          //                  select new
          //                  {
          //                    c_dono = sumdat.Key.c_dono,
          //                    c_iteno = sumdat.Key.c_iteno,
          //                    n_qty = sumdat.Sum(x => x.n_qty),
          //                    n_sisa = sumdat.Sum(x => x.n_sisa),
          //                  }).AsQueryable().ToList();

          //    decimal QtyItemDO = ItemDO.GroupBy(x => x.c_iteno).Sum(s => s.Sum(x => x.n_sisa).Value);

          //    var batchVar = (from q in db.LG_DOD2s
          //                    where q.c_dono == doID && q.c_iteno == item
          //                    group q by new { q.c_dono, q.c_iteno, q.c_batch } into sumdat
          //                    select new
          //                    {
          //                      c_dono = sumdat.Key.c_dono,
          //                      c_iteno = sumdat.Key.c_iteno,
          //                      c_batch = sumdat.Key.c_batch,
          //                      n_qty = sumdat.Sum(x => x.n_qty),
          //                      n_sisa = sumdat.Sum(x => x.n_sisa),
          //                    }).AsQueryable().ToList();

          //    for (nLoopC = 0; nLoopC < batchVar.Count; nLoopC++)
          //    {
          //      string batch = batchVar[nLoopC].c_batch ?? string.Empty;

          //      var spCount = (from q in db.LG_PLD1s
          //                     where q.c_plno == doh.c_plno
          //                     && q.c_iteno == item && q.c_batch == batch
          //                     group q by new { q.c_plno, q.c_iteno, q.c_batch, q.c_spno } into g
          //                     select new
          //                     {
          //                       g.Key.c_plno,
          //                       g.Key.c_iteno,
          //                       g.Key.c_batch,
          //                       g.Key.c_spno,
          //                       n_qty = g.Sum(x => x.n_qty),
          //                       n_sisa = g.Sum(x => x.n_sisa)
          //                     }).AsQueryable().ToList();

          //      listDod2 = (from q in db.LG_DOD2s
          //                  where q.c_dono == doID && q.c_iteno == item &&
          //                    q.c_batch == batch
          //                  select q).Distinct().ToList();

          //      var dod2Var = (from q in batchVar
          //                     where q.c_dono == doID && q.c_iteno == item
          //                     && q.c_batch == batch
          //                     select q
          //                 ).Distinct().Take(1).SingleOrDefault();

          //      for (int spLoop = 0; spLoop < spCount.Count; spLoop++)
          //      {
          //        string spNo = spCount[spLoop].c_spno ?? string.Empty;

          //        decimal QtySP = spCount.GroupBy(x => x.c_iteno).Sum(s => s.Sum(x => x.n_qty).Value);

          //        pld1 = (from q in db.LG_PLD1s
          //                where q.c_plno == doh.c_plno &&
          //                q.c_iteno == item && q.c_batch == batch
          //                && q.c_spno == spNo
          //                select q).Distinct().Take(1).SingleOrDefault();

          //        if (QtyItemDO >= QtySP)
          //        {
          //          pld1.n_sisa += QtySP;
          //        }
          //        else if (QtyItemDO < QtySP)
          //        {

          //        }
          //      }
          //      listDod3.Add(new LG_DOD3()
          //      {
          //        c_dono = doID,
          //        c_batch = batch,
          //        c_iteno = item,
          //        c_via = doh.c_via,
          //        n_qty = dod2Var.n_qty,
          //        n_sisa = dod2Var.n_sisa,
          //        d_update = date,
          //        c_update = nipEntry,
          //        v_ket = structure.Fields.Keterangan
          //      });

          //      db.LG_DOD3s.InsertAllOnSubmit(listDod3.ToArray());

          //      db.LG_DOD2s.DeleteAllOnSubmit(listDod2.ToArray());

          //    }

          //    listDod1 = (from q in db.LG_DOD1s
          //                where q.c_dono == doID && q.c_iteno == item
          //                select q).Distinct().ToList();

          //    db.LG_DOD1s.DeleteAllOnSubmit(listDod1);

          //    listDod1.Clear();
          //    listDod2.Clear();
          //    listDod3.Clear();
          //  }
          //}

          #endregion

          listDod1 = new List<LG_DOD1>();
          listResPLD1 = new List<LG_PLD1>();

          listDod1 = (from q in db.LG_DOD1s
                      where q.c_dono == doID
                      select q).ToList();

          listDod2 = (from q in db.LG_DOD2s
                      where q.c_dono == doID
                      select q).ToList();

          listResPLD1 = (from q in db.LG_PLD1s
                         where (q.c_plno == plID)
                         select q).ToList();

          if (listResPLD1.Count > 0)
          {
            if (listDod1.Count > 0)
            {
              listDod3 = new List<LG_DOD3>();
              dod1 = new LG_DOD1();
              pld1 = new LG_PLD1();
              listResPLD1Temp = new List<LG_PLD1>();

              for (nLoop = 0; nLoop < listDod1.Count; nLoop++)
              {
                dod1 = listDod1[nLoop];
                pld1 = listResPLD1[nLoop];

                itemID = (string.IsNullOrEmpty(dod1.c_iteno) ? string.Empty : dod1.c_iteno.Trim());

                listDod3.Add(new LG_DOD3()
                {
                  c_dono = doID,
                  c_iteno = itemID,
                  c_via = dod1.c_via,
                  c_batch = pld1.c_batch,
                  n_qty = dod1.n_qty,
                  n_sisa = dod1.n_sisa,
                  v_type = "03",
                  v_ket_del = structure.Fields.Keterangan,
                  c_entry = nipEntry,
                  d_entry = date
                });
                
                listResPLD1Temp = listResPLD1.FindAll(delegate(LG_PLD1 pld)
                {
                  return itemID.Equals((string.IsNullOrEmpty(pld.c_iteno) ? string.Empty : pld.c_iteno), StringComparison.OrdinalIgnoreCase);
                });

                if (listResPLD1Temp.Count > 0)
                {
                  for (nLoopC = 0; nLoopC < listResPLD1Temp.Count; nLoopC++)
                  {
                    pld1 = listResPLD1Temp[nLoopC];

                    pld1.n_sisa = pld1.n_qty;
                  }
                }
              }

              if (listDod3.Count > 0)
              {
                db.LG_DOD3s.InsertAllOnSubmit(listDod3.ToArray());
                listDod3.Clear();
              }

              db.LG_DOD1s.DeleteAllOnSubmit(listDod1.ToArray());
              listDod1.Clear();
            }
            if (listDod2.Count > 0)
            {
                db.LG_DOD2s.DeleteAllOnSubmit(listDod2.ToArray());
                listDod2.Clear();
            }

            listResPLD1.Clear();

            hasAnyChanges = true;
          }

          #endregion
          
          #endregion
        }
        else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify

            if (string.IsNullOrEmpty(doID))
          {
            result = "Nomor DO dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          doh = (from q in db.LG_DOHs
                 where q.c_dono == doID
                 select q).Take(1).SingleOrDefault();

          if (doh == null)
          {
            result = "Nomor Do tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (doh.l_delete.HasValue && doh.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor DO yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.HasFJ(db, doID))
          {
            result = "Delivery order tidak dapat diubah, karena sudah dibuat faktur penjualan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.HasExp(db, doID))
          {
            result = "DO yang sudah terdapat Ekspedisi tidak dapat diubah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, doh.d_dodate))
          {
            result = "DO tidak dapat dihapus, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
          {
            doh.v_ket = structure.Fields.Keterangan;
          }
          doh.c_update = nipEntry;
          doh.d_update = date;

          #region Populate Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listPl = new List<string>();
            listDod1 = new List<LG_DOD1>();
            listDod2 = new List<LG_DOD2>();
            listDod3 = new List<LG_DOD3>();
            listResPLD1 = new List<LG_PLD1>();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              if ((field != null) && field.IsDelete && (field.Quantity > 0))
              {
                #region Old Code

                //dod1 = (from q in db.LG_DOD1s
                //        where q.c_dono == doID && q.c_iteno == field.Item
                //        select q).Distinct().Take(1).SingleOrDefault();

                //if (dod1 != null)
                //{
                //  doQty = (dod1.n_qty.HasValue ? dod1.n_qty.Value : 0);
                //  doSisa = (dod1.n_sisa.HasValue ? dod1.n_sisa.Value : 0);
                //  if (doQty.Equals(doSisa))
                //  {

                //    #region update PLd1

                //    listDod2 = (from q in db.LG_DOD2s
                //            where q.c_dono == doID && q.c_iteno == field.Item
                //            orderby q.c_iteno
                //             select q).Distinct().ToList();


                //    listResPLD1 = (from q in db.LG_PLD1s
                //             where q.c_plno == structure.Fields.nopl && q.c_iteno == field.Item
                //             select q).Distinct().ToList();


                //    dod2 = (from q in db.LG_DOD2s
                //                where q.c_dono == doID && q.c_iteno == field.Item
                //                orderby q.c_iteno
                //                select q).Distinct().Take(1).SingleOrDefault();

                //    if (listDod1.Sum(x=>x.n_sisa).Equals(listResPLD1.Sum(x=>x.n_sisa)))
                //    {
                //      pld1 = (from q in db.LG_PLD1s
                //              where q.c_plno == structure.Fields.nopl && q.c_iteno == field.Item
                //              select q).Distinct().Take(1).SingleOrDefault();

                //      pld1.n_sisa = pld1.n_qty;

                //      listDod3.Add(new LG_DOD3()
                //        {
                //          c_batch = dod2.c_batch,
                //          c_cusno = doh.c_cusno,
                //          c_dono = doh.c_dono,
                //          c_entry = nipEntry,
                //          c_gdg = doh.c_gdg,
                //          c_iteno = field.Item,
                //          c_plno = doh.c_plno,
                //          c_typeH = doh.c_type,
                //          c_via = doh.c_type,
                //          c_update = doh.c_entry,
                //          d_dodate = doh.d_dodate,
                //          n_sisa = dod2.n_sisa,
                //          n_qty = dod2.n_qty,
                //          v_ket = field.KeteranganMod
                //        });
                //      db.LG_DOD2s.DeleteAllOnSubmit(listDod2.ToArray());

                //    }

                //    #endregion

                //    db.LG_DOD3s.InsertAllOnSubmit(listDod3.ToArray());
                //  }
                //  db.LG_DOD1s.DeleteOnSubmit(dod1);
                //}

                #endregion

                dod1 = (from q in db.LG_DOD1s
                        where q.c_dono == doID && q.c_iteno == field.Item
                        select q).Take(1).SingleOrDefault();

                dod2 = (from q in db.LG_DOD2s
                        where q.c_dono == doID && q.c_iteno == field.Item
                        select q).Take(1).SingleOrDefault();

                if (dod1 != null)
                {
                  listResPLD1 = (from q in db.LG_PLD1s
                                 where q.c_plno == doh.c_plno && q.c_iteno == field.Item
                                 select q).ToList();

                  if ((listResPLD1 != null) && (listResPLD1.Count > 0))
                  {
                    for (nLoopC = 0; nLoopC < listResPLD1.Count; nLoopC++)
                    {
                      pld1 = listResPLD1.Find(delegate(LG_PLD1 pld)
                      {
                        return pld.c_batch.Equals(listResPLD1[nLoopC].c_batch, StringComparison.OrdinalIgnoreCase) &&
                          pld.c_spno.Equals(listResPLD1[nLoopC].c_spno, StringComparison.OrdinalIgnoreCase);
                      });

                      if (pld1 != null)
                      {
                        pld1.n_sisa = pld1.n_qty;
                      }
                    }
                    dod3 = new LG_DOD3()
                    {
                      c_dono = dod1.c_dono,
                      c_iteno = field.Item,
                      c_via = dod1.c_via,
                      c_batch = pld1.c_batch,
                      c_entry = nipEntry,
                      d_entry = date,
                      n_sisa = dod1.n_sisa,
                      n_qty = dod1.n_qty,
                      v_ket_del = field.KeteranganMod
                    };
                  }
                  db.LG_DOD1s.DeleteOnSubmit(dod1);
                }

                if (dod2 != null)
                {
                    db.LG_DOD2s.DeleteOnSubmit(dod2); 
                }

                if (dod3 != null)
                {
                  db.LG_DOD3s.InsertOnSubmit(dod3);
                }

                #region Old Coded

                //var td = (from q in db.LG_DOD2s
                //          where q.c_iteno == field.Item
                //          && q.c_dono == doID
                //          group q by new { q.c_dono, q.c_iteno, q.c_batch } into sumdat
                //          select new
                //          {
                //            c_dono = sumdat.Key.c_dono,
                //            c_iteno = sumdat.Key.c_iteno,
                //            c_batch = sumdat.Key.c_batch,
                //            n_qty = sumdat.Sum(x => x.n_qty),
                //            n_sisa = sumdat.Sum(x => x.n_sisa),
                //          }).AsQueryable().ToList();

                //for (nLoopC = 0; nLoopC < td.Count; nLoopC++)
                //{
                //  string batch = td[nLoopC].c_batch ?? string.Empty;

                //  listDod2 = (from q in db.LG_DOD2s
                //              where q.c_dono == doID && q.c_iteno == field.Item &&
                //                q.c_batch == batch
                //              select q).Distinct().ToList();

                //  var dod2Var = (from q in td
                //                 where q.c_dono == doID && q.c_iteno == field.Item
                //                 && q.c_batch == batch
                //                 select q
                //             ).Distinct().Take(1).SingleOrDefault();

                //  pld1 = (from q in db.LG_PLD1s
                //          where q.c_plno == structure.Fields.nopl &&
                //          q.c_iteno == field.Item && q.c_batch == batch
                //          select q).Distinct().Take(1).SingleOrDefault();

                //  decimal Qty = listDod2.GroupBy(x => x.c_batch).Sum(s => s.Sum(x => x.n_qty).Value);


                //  pld1.n_sisa += Qty;

                //  listDod3.Add(new LG_DOD3()
                //        {
                //          //c_batch = dod2Var.c_batch,
                //          c_dono = dod2Var.c_dono,
                //          //c_gdg = doh.c_gdg,
                //          c_iteno = field.Item,
                //          c_entry = nipEntry,
                //          d_entry = date,
                //          n_sisa = dod2Var.n_sisa,
                //          n_qty = dod2Var.n_qty,
                //          v_ket_del = field.KeteranganMod
                //        });

                //  db.LG_DOD3s.InsertAllOnSubmit(listDod3.ToArray());

                //  db.LG_DOD2s.DeleteAllOnSubmit(listDod2.ToArray());

                //}
                //listDod1 = (from q in db.LG_DOD1s
                //            where q.c_dono == doID && q.c_iteno == field.Item
                //            select q).Distinct().ToList();

                //db.LG_DOD1s.DeleteAllOnSubmit(listDod1);

                //listDod1.Clear();
                //listDod2.Clear();
                //listDod3.Clear();

                #endregion
              }
            }

            if (doh != null)
            {
                doh.n_karton -= koliKarton;
                doh.n_receh -= receh;
            }

            hasAnyChanges = true;
          }
          #endregion

          //LG_MsGudang gud = null;

          //gud = (from q in db.LG_MsGudangs
          //       where q.c_gdg == doh.c_gdg
          //       select q).Take(1).SingleOrDefault();

          //dic = new Dictionary<string, string>();

          //string gudangPL;

          //gudangPL = Convert.ToString(gud.v_gdgdesc);

          //dic.Add("DO", doID);
          //dic.Add("Tanggal", date.ToString("yyyyMMdd"));

          #endregion
        }
        else if (structure.Method.Equals("ConfirmSent", StringComparison.OrdinalIgnoreCase))
        {
          #region ConfirmSent

          if (string.IsNullOrEmpty(doID))
          {
            result = "Nomor DO dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          doh = (from q in db.LG_DOHs
                 where q.c_dono == doID
                 select q).Take(1).SingleOrDefault();

          decimal nDod1 = (from q in db.LG_DOD1s
                           where q.c_dono == doID
                           select q.n_qty.Value).Sum(x => x);


          decimal nDod2 = (from q in db.LG_DOD2s
                           where q.c_dono == doID
                           select q.n_qty.Value).Sum(x => x);


          if (doh == null)
          {
            result = "Nomor Do tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (doh.l_delete.HasValue && doh.l_delete.Value)
          {
            result = "Tidak dapat mengirim DO yang terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (doh.l_print.HasValue && (!doh.l_print.Value))
          {
            result = "Tidak dapat mengirim DO yang belum tercetak.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, doh.d_dodate))
          {
            result = "DO tidak dapat diubah, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          custNo = (string.IsNullOrEmpty(doh.c_cusno) ? string.Empty : doh.c_cusno.Trim());

          //isSent = (doh.l_send.HasValue ? doh.l_send.Value : false);

          isSendMethod = true;

          if (structure.Fields.ConfirmedSent)
          {
            if (nDod1 == nDod2)
            {
              doh.l_send = structure.Fields.ConfirmedSent;
              doh.l_sent = structure.Fields.ConfirmedSent;

              doh.c_update = nipEntry;
              doh.d_update = date;

              hasAnyChanges = true;
            }
            else
            {
              result = "Tidak dapat mengirim DO, Detil berbeda.";

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
            result = "Status kirim DO terkonfirmasi.";
          }

          #endregion
        }

        if (!isContexted)
        {
          if (hasAnyChanges)
          {
            db.SubmitChanges();

            db.Transaction.Commit();
            //db.Transaction.Rollback();

            if (isSendMethod)
            {
              //Indra 20170613
              //Commons.RunningGenerateFJ(db, custNo, doID, doID, nipEntry);
              Commons.RunningGenerateFJ(db, custNo, doID, doID, nipEntry, 0);
            }

            rpe = ResponseParser.ResponseParserEnum.IsSuccess;
          }
          else
          {
            db.Transaction.Rollback();

            rpe = ResponseParser.ResponseParserEnum.IsFailed;
          }
        }
        else
        {
          if (hasAnyChanges)
          {
            //if (isSendMethod)
            //{
            //  Commons.RunningGenerateFJ(db, custNo, doID, doID, nipEntry);
            //}

            rpe = ResponseParser.ResponseParserEnum.IsSuccess;
          }
          else
          {
            rpe = ResponseParser.ResponseParserEnum.IsFailed;
          }
        }
      }
      catch (Exception ex)
      {
        if (db.Transaction != null)
        {
          db.Transaction.Rollback();
        }
        rpe = ResponseParser.ResponseParserEnum.IsError;

        result = string.Format("ScmsSoaLibrary.Bussiness.Penjualan:DOPackingList - {0}", ex.Message);

        Logger.WriteLine(result, true);
        Logger.WriteLine(ex.StackTrace);
      }

    endLogic:
      result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

      if (dic != null)
      {
        dic.Clear();
      }

      if (!isContexted)
      {
        db.Dispose();
      }

      return result;
    }

    public string DOSTT(ScmsSoaLibrary.Parser.Class.DOSTTStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false;

      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      LG_DOH doh = null;

      LG_STD1 std1 = null;
      //LG_STD2 std2 = null;

      ScmsSoaLibrary.Parser.Class.DOSTTStructureField field = null;

      string nipEntry = null;
      string doID = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      decimal sttQtyReloc = 0, doQty = 0, doSisa = 0,
        sttAlloc = 0, sttQty = 0;

      //bool isSent = false,
      //  isSendMethod = false;

      List<LG_DOD1> listDod1 = null;
      //List<LG_DOD2> listDod2 = null;
      List<string> listST = null;
      List<LG_STD1> listResSTD1 = null;
      List<LG_STD1> listResSTD1Copy = null;
      List<LG_DOD3> listDod3 = null;
      List<LG_DOD2> listDod2 = null;
      LG_DOH sttCheck = null;
      LG_DOD1 dod1 = null;
      //LG_DOD2 dod2 = null;

      //LG_STH sth = null;

      char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '1' : structure.Fields.Gudang[0]);

      int nLoop = 0,
        nLoopC = 0;

      string strPinCode = null;

      IDictionary<string, string> dic = null;

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }
      int totalDetails = 0;

      doID = (structure.Fields.DOid ?? string.Empty);

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          if (!string.IsNullOrEmpty(doID))
          {
            result = "Nomor DO List harus kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Customer))
          {
            result = "Nama Cabang dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.nopl))
          {
            result = "NO STT dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, date))
          {
            result = "Receive note tidak dapat dihapus, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          //sth = (from q in db.LG_STHs
          //       join q1 in db.LG_MsGudangs on q.c_gdg equals q1.c_gdg
          //       where q.c_stno == structure.Fields.nopl
          //       select q).Take(1).SingleOrDefault();

          //strPinCode = Functionals.GeneratedRandomPinId(8, string.Empty);

          List<string> sItem = structure.Fields.Field.Select(x => x.Item).ToList();

          #region Pin

          listResSTD1Copy = (from q in db.LG_STD1s
                             where q.c_stno == structure.Fields.nopl && q.n_qty != 0
                             && sItem.Contains(q.c_iteno)
                             select q).ToList();

          int nLoopPl = listResSTD1Copy.Count();
          int Hit = 1, lBacth = 0, nHit = 0;
          decimal value = 0,
                  valueS = 0;
          while (Hit <= nLoopPl)
          {
            lBacth = listResSTD1Copy[nHit].c_batch.Trim().Length;

            lBacth -= 1;

            while (lBacth > -1)
            {
              char[] sc = listResSTD1Copy[nHit].c_batch.Trim().Substring(lBacth, 1).ToCharArray();
              foreach (char letter in sc)
              {
                value = Convert.ToInt64(letter);
                valueS += value;
              }
              lBacth -= 1;
            }

            valueS += listResSTD1Copy[nHit].n_qty.Value;

            //Indra 20171018
            //valueS += decimal.Parse(listResSTD1Copy[nHit].c_iteno);


            lBacth = listResSTD1Copy[nHit].c_iteno.Trim().Length;

            lBacth -= 1;

            while (lBacth > -1)
            {
                char[] sc = listResSTD1Copy[nHit].c_iteno.Trim().Substring(lBacth, 1).ToCharArray();
                foreach (char letter in sc)
                {
                    value = Convert.ToInt64(letter);
                    valueS += value;
                }
                lBacth -= 1;
            }

            valueS += listResSTD1Copy[nHit].n_qty.Value;

            Hit++;
            nHit++;
          }

          if (listResSTD1Copy.Count > 0)
          {
            listResSTD1Copy.Clear();

            valueS = Math.Round(valueS);
          }

          #endregion

          doID = Commons.GenerateNumbering<LG_DOH>(db, "DO", '3', "09", date, "c_dono");

          //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "DO");

          doh = new LG_DOH()
          {
            c_dono = doID,
            d_dodate = date.Date,
            c_gdg = gudang,
            c_type = "02",
            c_cusno = structure.Fields.Customer,
            c_plno = structure.Fields.nopl,
            c_via = structure.Fields.Via,
            v_ket = structure.Fields.Keterangan,
            c_pin = valueS.ToString(),
            l_status = false,
            l_print = false,
            l_send = false,
            l_sent = false,
            c_entry = nipEntry,
            d_entry = date,
            c_update = nipEntry,
            d_update = date,
            l_auto = false,
            l_wpdc = false,
            c_rnno = null,
            l_delete = false,
            l_receipt = false,
            v_ket_mark = null
          };

          db.LG_DOHs.InsertOnSubmit(doh);

          #region Old Code

          //db.SubmitChanges();

          //doh = (from q in db.LG_DOHs
          //       where q.v_ket == tmpNumbering
          //       select q).Take(1).SingleOrDefault();

          //if (!string.IsNullOrEmpty(doID))
          //{
          //  result = "No DO tidak dapat di raih";

          //  rpe = ResponseParser.ResponseParserEnum.IsError;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}
          //else if (doh.c_dono.Equals("XXXXXXXXXX"))
          //{
          //  result = "Trigger DO tidak aktif.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  goto endLogic;
          //}

          //doh.v_ket = structure.Fields.Keterangan;

          //doID = doh.c_dono;

          //db.SubmitChanges();

          #endregion

          #region Insert Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listST = new List<string>();
            listDod1 = new List<LG_DOD1>();
            listDod2 = new List<LG_DOD2>();

            listResSTD1 = (from q in db.LG_STHs
                           join q1 in db.LG_STD1s on new { q.c_gdg, q.c_stno } equals new { q1.c_gdg, q1.c_stno }
                           where q1.c_stno == structure.Fields.nopl
                            && q1.n_sisa > 0
                            && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                           select q1).Distinct().ToList();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              sttCheck = new LG_DOH();
              sttCheck = (from q in db.LG_DOHs 
                          join q1 in db.LG_DOD1s on q.c_dono equals q1.c_dono
                          where q.c_plno == structure.Fields.nopl && q1.c_iteno == field.Item
                          select q).Take(1).SingleOrDefault();

              if (sttCheck != null)
              {
                  result = "STT " + structure.Fields.nopl + " dengan item " + field.Item + " sudah pernah dibuatkan dengan No.DO : " + sttCheck.c_dono;

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  if (db.Transaction != null)
                  {
                      db.Transaction.Rollback();
                  }

                  goto endLogic;
              }

              if (field.IsNew && (field.Quantity > 0))
              {
                #region Old Coded

                //var td = (from dat in db.LG_STD1s
                //          where dat.c_iteno == field.Item
                //          && dat.c_stno == structure.Fields.nopl
                //          group dat by new { dat.c_stno, dat.c_iteno, dat.c_batch } into sumdat
                //          select new
                //          {
                //            c_stno = sumdat.Key.c_stno,
                //            c_iteno = sumdat.Key.c_iteno,
                //            c_batch = sumdat.Key.c_batch,
                //            n_sisa = sumdat.Sum(x => x.n_qty)
                //          }).AsQueryable().ToList();

                #endregion

                listResSTD1Copy = listResSTD1.Where(x => x.c_iteno == field.Item).ToList();

                sttAlloc = 0;

                for (nLoopC = 0; nLoopC < listResSTD1Copy.Count; nLoopC++)
                {
                  std1 = listResSTD1Copy[nLoopC];

                  sttQty = (std1.n_sisa.HasValue ? std1.n_sisa.Value : 0);

                  listDod2.Add(new LG_DOD2()
                  {
                    c_batch = std1.c_batch,
                    c_dono = doID,
                    c_iteno = std1.c_iteno,
                    c_via = structure.Fields.Via,
                    n_qty = sttQty,
                    n_sisa = sttQty,
                  });

                  if (sttQty > 0)
                  {
                    sttAlloc += sttQty;

                    std1.n_sisa = 0;
                  }

                  
                }

                if (sttAlloc > 0)
                {
                  listDod1.Add(new LG_DOD1()
                  {
                    c_dono = doID,
                    c_iteno = field.Item,
                    c_via = structure.Fields.Via,
                    n_sisa = sttAlloc,
                    n_qty = sttAlloc
                  });

                  totalDetails++;
                }

                #region Old Coded

                //for (nLoopC = 0; nLoopC < td.Count; nLoopC++)
                //{

                //  string batch = td[nLoopC].c_batch ?? string.Empty;

                //  var std = (from q in td
                //             where q.c_stno == structure.Fields.nopl && q.c_iteno == field.Item
                //             && q.c_batch == batch
                //             select q
                //             ).Distinct().Take(1).SingleOrDefault();

                //  std1 = (from q in db.LG_STD1s
                //          where q.c_iteno == field.Item && q.c_batch == batch
                //          select q).Distinct().Take(1).SingleOrDefault();


                //  //listDod2.Add(new LG_DOD2
                //  //{
                //  //  c_dono = doID,
                //  //  c_iteno = field.Item,
                //  //  c_via = structure.Fields.Via,
                //  //  c_batch = std.c_batch,
                //  //  n_qty = std.n_sisa,
                //  //  n_sisa = std.n_sisa
                //  //});

                //  //db.LG_DOD2s.InsertAllOnSubmit(listDod2.ToArray());

                //  //decimal QtyAdd = listDod2.GroupBy(x => x.c_batch).Sum(s => s.Sum(x => x.n_sisa)).Value;

                //  //std1.n_sisa -= QtyAdd;

                //  listDod2.Clear();
                //}

                //var dod2Var = (from q in db.LG_DOD2s
                //               where q.c_dono == doID && q.c_iteno == field.Item
                //               select q).Distinct().ToList();

                //decimal t = dod2Var.GroupBy(x => x.c_iteno).Sum(s => s.Sum(x => x.n_qty).Value);

                //listDod1.Add(new LG_DOD1()
                //{
                //  n_qty = t,
                //  c_dono = doID,
                //  c_iteno = field.Item,
                //  c_via = structure.Fields.Via,
                //  n_sisa = t
                //});

                #endregion
              }
            }

            if (listDod1.Count > 0)
            {
              db.LG_DOD1s.InsertAllOnSubmit(listDod1.ToArray());
              listDod1.Clear();
            }

            if (listDod2.Count > 0)
            {
              db.LG_DOD2s.InsertAllOnSubmit(listDod2.ToArray());
              listDod2.Clear();
            }

            listResSTD1.Clear();
          }

          #endregion

          dic = new Dictionary<string, string>();

          if (totalDetails > 0)
          {
            dic.Add("DO", doID);
            dic.Add("Tanggal", date.ToString("yyyyMMdd"));
            dic.Add("Pin", strPinCode);

            hasAnyChanges = true;
          }

          #endregion
        }
        else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify

          if (string.IsNullOrEmpty(doID))
          {
            result = "Nomor DO dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          doh = (from q in db.LG_DOHs
                 where q.c_dono == doID
                 select q).Take(1).SingleOrDefault();

          if (doh == null)
          {
            result = "Nomor Do tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (doh.l_delete.HasValue && doh.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor DO yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, doh.d_dodate))
          {
            result = "DO tidak dapat diubah, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.HasExp(db, doID))
          {
            result = "DO yang sudah terdapat Ekspedisi tidak dapat diubah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
          {
            doh.v_ket = structure.Fields.Keterangan;
          }

          doh.c_update = nipEntry;
          doh.d_update = date;

          #region Populate Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            //listST = new List<string>();
            //listDod1 = new List<LG_DOD1>();
            //listDod2 = new List<LG_DOD2>();
            listDod3 = new List<LG_DOD3>();
            //listResSTD1 = new List<LG_STD1>();

            listDod1 = (from q in db.LG_DOD1s
                        where q.c_dono == structure.Fields.DOid
                        select q).ToList();

            listResSTD1 = (from q in db.LG_STHs
                           join q1 in db.LG_STD1s on new { q.c_gdg, q.c_stno } equals new { q1.c_gdg, q1.c_stno }
                           where q1.c_stno == structure.Fields.nopl
                            && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                           select q1).Distinct().ToList();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              dod1 = listDod1.Find(delegate(LG_DOD1 dod)
              {
                return field.Item.Equals((string.IsNullOrEmpty(dod.c_iteno) ? string.Empty : dod.c_iteno), StringComparison.OrdinalIgnoreCase);
              });

              if (dod1 != null)
              {
                listResSTD1Copy = listResSTD1.Where(x => x.c_iteno == field.Item).ToList();

                #region Old Coded

                //if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0))
                //{
                //  #region New

                //  sttAlloc = 0;
                //  for (nLoopC = 0; nLoopC < listResSTD1Copy.Count; nLoopC++)
                //  {
                //    std1 = listResSTD1Copy[nLoopC];

                //    sttQty = (std1.n_sisa.HasValue ? std1.n_sisa.Value : 0);

                //    if (sttQty > 0)
                //    {
                //      sttAlloc += sttQty;

                //      std1.n_sisa = 0;
                //    }
                //  }

                //  if (sttAlloc > 0)
                //  {
                //    listDod1.Add(new LG_DOD1()
                //    {
                //      c_dono = doID,
                //      c_iteno = field.Item,
                //      c_via = structure.Fields.Via,
                //      n_sisa = sttAlloc,
                //      n_qty = sttAlloc
                //    });

                //    totalDetails++;
                //  }

                //  #endregion
                //}

                #endregion

                if ((field != null) && (!field.IsNew) && field.IsDelete)
                {
                  #region Delete

                  doQty = (dod1.n_qty.HasValue ? dod1.n_qty.Value : 0);

                  for (nLoopC = 0; nLoopC < listResSTD1Copy.Count; nLoopC++)
                  {
                    std1 = listResSTD1Copy[nLoopC];

                    sttQty = (std1.n_qty.HasValue ? std1.n_qty.Value : 0);

                    sttQtyReloc = (doQty > sttQty ? sttQty : doQty);

                    std1.n_sisa += sttQtyReloc;

                    doQty -= sttQtyReloc;

                    totalDetails++;

                    if (doQty <= 0)
                    {
                      break;
                    }
                  }

                  listDod3.Add(new LG_DOD3()
                  {
                    c_dono = doID,
                    c_iteno = dod1.c_iteno,
                    //c_via = doh.c_via,
                    n_qty = doQty,
                    n_sisa = doSisa,
                    c_entry = nipEntry,
                    d_entry = date,
                    v_ket_del = field.KeteranganMod,
                    //c_batch = string.Empty,
                    v_type = "03"
                  });

                  #endregion
                }

                listResSTD1Copy.Clear();
              }
            }

            if (listDod3.Count > 0)
            {
              db.LG_DOD3s.InsertAllOnSubmit(listDod3.ToArray());
              listDod3.Clear();
            }

            #region Old Coded

            //for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            //{
            //  field = structure.Fields.Field[nLoop];

            //  if ((field != null) && (field.IsDelete) && (field.Quantity > 0))
            //  {

            //    var td = (from q in db.LG_DOD2s
            //              where q.c_iteno == field.Item
            //              && q.c_dono == doID
            //              group q by new { q.c_dono, q.c_iteno, q.c_batch } into sumdat
            //              select new
            //              {
            //                c_dono = sumdat.Key.c_dono,
            //                c_iteno = sumdat.Key.c_iteno,
            //                c_batch = sumdat.Key.c_batch,
            //                n_qty = sumdat.Sum(x => x.n_qty),
            //                n_sisa = sumdat.Sum(x => x.n_sisa),
            //              }).AsQueryable().ToList();

            //    for (nLoopC = 0; nLoopC < td.Count; nLoopC++)
            //    {

            //      string batch = td[nLoopC].c_batch ?? string.Empty;

            //      listDod2 = (from q in db.LG_DOD2s
            //                  where q.c_dono == doID && q.c_iteno == field.Item &&
            //                    q.c_batch == batch
            //                  select q).Distinct().ToList();

            //      var dod2Var = (from q in td
            //                     where q.c_dono == doID && q.c_iteno == field.Item
            //                     && q.c_batch == batch
            //                     select q
            //                 ).Distinct().Take(1).SingleOrDefault();

            //      std1 = (from q in db.LG_STD1s
            //              where q.c_stno == structure.Fields.nopl && q.c_iteno == field.Item
            //              && q.c_batch == batch
            //              select q).Distinct().Take(1).SingleOrDefault();


            //      decimal t = listDod2.GroupBy(x => x.c_batch).Sum(s => s.Sum(x => x.n_qty).Value);

            //      std1.n_sisa += t;

            //      listDod3.Add(new LG_DOD3()
            //        {
            //          c_dono = doID,
            //          c_batch = batch,
            //          c_iteno = field.Item,
            //          c_via = structure.Fields.Via,
            //          n_qty = dod2Var.n_qty,
            //          n_sisa = dod2Var.n_sisa,
            //          d_update = date,
            //          c_update = nipEntry,
            //          v_ket = field.KeteranganMod
            //        });

            //      db.LG_DOD3s.InsertAllOnSubmit(listDod3.ToArray());

            //      db.LG_DOD2s.DeleteAllOnSubmit(listDod2.ToArray());

            //    }

            //    listDod1 = (from q in db.LG_DOD1s
            //                where q.c_dono == doID && q.c_iteno == field.Item
            //                select q).Distinct().ToList();

            //    db.LG_DOD1s.DeleteAllOnSubmit(listDod1);

            //    listDod1.Clear();
            //    //listDod2.Clear();
            //    listDod3.Clear();

            //  }
            //}

            #endregion
          }

          #endregion

          hasAnyChanges = true;

          #endregion
        }
        else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete

          if (string.IsNullOrEmpty(doID))
          {
            result = "Nomor DO dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          doh = (from q in db.LG_DOHs
                 where q.c_dono == doID
                 select q).Take(1).SingleOrDefault();

          if (doh == null)
          {
            result = "Nomor DO tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (doh.l_delete.HasValue && doh.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor DO yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, doh.d_dodate))
          {
            result = "DO tidak dapat dihapus, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.HasExp(db, doID))
          {
            result = "DO yang sudah terdapat Ekspedisi tidak dapat diubah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          doh.c_update = nipEntry;
          doh.d_update = date;

          doh.l_delete = true;
          doh.v_ket_mark = structure.Fields.Keterangan;

          //listDod2 = (from q in db.LG_DOD2s
          //            where q.c_dono == doID
          //            select q).ToList();

          #region Populate Detail

          listDod3 = new List<LG_DOD3>();

          listDod1 = (from q in db.LG_DOD1s
                      where q.c_dono == doID
                      select q).Distinct().ToList();

          listResSTD1 = (from q in db.LG_STD1s
                         where q.c_stno == doh.c_plno
                         select q).Distinct().ToList();

          for (nLoop = 0; nLoop < listDod1.Count; nLoop++)
          {

            dod1 = listDod1[nLoop];

            listResSTD1Copy = listResSTD1.FindAll(delegate(LG_STD1 std)
            {
              return
                (string.IsNullOrEmpty(dod1.c_iteno) ? string.Empty : dod1.c_iteno).Equals((string.IsNullOrEmpty(std.c_iteno) ? string.Empty : std.c_iteno));
            });

            doQty = (dod1.n_qty.HasValue ? dod1.n_qty.Value : 0);
            doSisa = (dod1.n_sisa.HasValue ? dod1.n_sisa.Value : 0);

            listDod3.Add(new LG_DOD3()
            {
              c_dono = doID,
              c_iteno = dod1.c_iteno,
              //c_via = doh.c_via,
              n_qty = doQty,
              n_sisa = doSisa,
              //d_update = date,
              //c_update = nipEntry,
              v_ket_del = structure.Fields.Keterangan,
              //c_batch = string.Empty,
              c_entry = nipEntry,
              d_entry = date,
              v_type = "03"
            });

            if ((listResSTD1Copy != null) && (listResSTD1Copy.Count > 0))
            {
              for (nLoopC = 0; nLoopC < listResSTD1Copy.Count; nLoopC++)
              {
                std1 = listResSTD1Copy[nLoopC];

                sttQty = (std1.n_qty.HasValue ? std1.n_qty.Value : 0);

                sttAlloc = (doSisa > sttQty ? sttQty : doSisa);

                std1.n_sisa += sttAlloc;

                doSisa -= sttAlloc;

                if (doSisa <= 0)
                {
                  break;
                }
              }

              listResSTD1Copy.Clear();
            }
          }

          if (listDod1.Count > 0)
          {
            db.LG_DOD1s.DeleteAllOnSubmit(listDod1.ToArray());

            listDod1.Clear();
          }

          if (listDod3.Count > 0)
          {
            db.LG_DOD3s.InsertAllOnSubmit(listDod3.ToArray());

            listDod3.Clear();
          }

          #region Old Coded

          //if ((listDod2 != null) && (listDod2.Count > 0))
          //{

          //  var td = (from q in db.LG_DOD2s
          //            where q.c_dono == doID
          //            group q by new { q.c_dono, q.c_iteno } into sumdat
          //            select new
          //            {
          //              c_dono = sumdat.Key.c_dono,
          //              c_iteno = sumdat.Key.c_iteno,
          //              n_qty = sumdat.Sum(x => x.n_qty),
          //              n_sisa = sumdat.Sum(x => x.n_sisa),
          //            }).AsQueryable().ToList();

          //  for (nLoop = 0; nLoop < td.Count; nLoop++)
          //  {

          //    string item = td[nLoop].c_iteno ?? string.Empty;

          //    var batchVar = (from q in db.LG_DOD2s
          //                    where q.c_dono == doID && q.c_iteno == item
          //                    group q by new { q.c_dono, q.c_iteno, q.c_batch } into sumdat
          //                    select new
          //                    {
          //                      c_dono = sumdat.Key.c_dono,
          //                      c_iteno = sumdat.Key.c_iteno,
          //                      c_batch = sumdat.Key.c_batch,
          //                      n_qty = sumdat.Sum(x => x.n_qty),
          //                      n_sisa = sumdat.Sum(x => x.n_sisa),
          //                    }).AsQueryable().ToList();

          //    for (nLoopC = 0; nLoopC < batchVar.Count; nLoopC++)
          //    {
          //      string batch = batchVar[nLoopC].c_batch ?? string.Empty;

          //      listDod2 = (from q in db.LG_DOD2s
          //                  where q.c_dono == doID && q.c_iteno == item &&
          //                    q.c_batch == batch
          //                  select q).Distinct().ToList();

          //      var dod2Var = (from q in batchVar
          //                     where q.c_dono == doID && q.c_iteno == item
          //                     && q.c_batch == batch
          //                     select q
          //                 ).Distinct().Take(1).SingleOrDefault();

          //      std1 = (from q in db.LG_STD1s
          //              where q.c_stno == doh.c_plno && q.c_iteno == item
          //              && q.c_batch == batch
          //              select q).Distinct().Take(1).SingleOrDefault();


          //      decimal t = listDod2.GroupBy(x => x.c_batch).Sum(s => s.Sum(x => x.n_qty).Value);

          //      std1.n_sisa += t;

          //      listDod3.Add(new LG_DOD3()
          //      {
          //        c_dono = doID,
          //        c_batch = batch,
          //        c_iteno = item,
          //        c_via = doh.c_via,
          //        n_qty = dod2Var.n_qty,
          //        n_sisa = dod2Var.n_sisa,
          //        d_update = date,
          //        c_update = nipEntry,
          //        v_ket = structure.Fields.Keterangan
          //      });

          //      db.LG_DOD3s.InsertAllOnSubmit(listDod3.ToArray());

          //      db.LG_DOD2s.DeleteAllOnSubmit(listDod2.ToArray());
          //    }

          //    listDod1 = (from q in db.LG_DOD1s
          //                where q.c_dono == doID && q.c_iteno == item
          //                select q).Distinct().ToList();

          //    db.LG_DOD1s.DeleteAllOnSubmit(listDod1);

          //    db.SubmitChanges();

          //    listDod1.Clear();
          //    listDod2.Clear();
          //    listDod3.Clear();
          //  }
          //}

          #endregion

          #endregion

          hasAnyChanges = true;

          #endregion
        }

        #region Old Coded

        //else if (structure.Method.Equals("ConfirmSent", StringComparison.OrdinalIgnoreCase))
        //{
        //  #region ConfirmSent

        //  if (string.IsNullOrEmpty(doID))
        //  {
        //    result = "Nomor DO dibutuhkan.";

        //    rpe = ResponseParser.ResponseParserEnum.IsFailed;

        //    if (db.Transaction != null)
        //    {
        //      db.Transaction.Rollback();
        //    }

        //    goto endLogic;
        //  }
        //  doh = (from q in db.LG_DOHs
        //         where q.c_dono == doID
        //         select q).Take(1).SingleOrDefault();

        //  if (doh == null)
        //  {
        //    result = "Nomor Do tidak ditemukan.";

        //    rpe = ResponseParser.ResponseParserEnum.IsFailed;

        //    if (db.Transaction != null)
        //    {
        //      db.Transaction.Rollback();
        //    }

        //    goto endLogic;
        //  }
        //  else if (doh.l_delete.HasValue && doh.l_delete.Value)
        //  {
        //    result = "Tidak dapat mengirim DO yang terhapus.";

        //    rpe = ResponseParser.ResponseParserEnum.IsFailed;

        //    if (db.Transaction != null)
        //    {
        //      db.Transaction.Rollback();
        //    }

        //    goto endLogic;
        //  }
        //  else if (doh.l_print.HasValue && (!doh.l_print.Value))
        //  {
        //    result = "Tidak dapat mengirim DO yang belum tercetak.";

        //    rpe = ResponseParser.ResponseParserEnum.IsFailed;

        //    if (db.Transaction != null)
        //    {
        //      db.Transaction.Rollback();
        //    }

        //    goto endLogic;
        //  }
        //  else if (Commons.IsClosingLogistik(db, doh.d_dodate))
        //  {
        //    result = "DO tidak dapat diubah, karena sudah closing.";

        //    rpe = ResponseParser.ResponseParserEnum.IsFailed;

        //    if (db.Transaction != null)
        //    {
        //      db.Transaction.Rollback();
        //    }

        //    goto endLogic;
        //  }

        //  custNo = (string.IsNullOrEmpty(doh.c_cusno) ? string.Empty : doh.c_cusno.Trim());

        //  isSent = (doh.l_send.HasValue ? doh.l_send.Value : false);

        //  isSendMethod = true;

        //  if (structure.Fields.ConfirmedSent && (!isSent))
        //  {
        //    doh.l_send = structure.Fields.ConfirmedSent;

        //    doh.c_update = nipEntry;
        //    doh.d_update = date;

        //    hasAnyChanges = true;
        //  }
        //  else
        //  {
        //    result = "Status kirim DO terkonfirmasi.";
        //  }

        //  #endregion
        //}

        #endregion

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

        result = string.Format("ScmsSoaLibrary.Bussiness.Penjualan:DOSTT - {0}", ex.StackTrace);

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

    public string DOSEND(ScmsSoaLibrary.Parser.Class.DoPLSendStructure structure)
    {
        if ((structure == null) || (structure.Fields == null))
        {
            return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
        }

        string result = null;

        ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

        LG_DOH doh = null;
        LG_FJH fjh = null;
        LG_STH sth = null;
        LG_FJD3 fjd3 = null;

        
        ScmsSoaLibrary.Parser.Class.DoPLSendStructureField field = null;

        string nipEntry = null;
        string doID = null,
               custNo = null;
        decimal NetFJNO, SisaFJNO = 0; //Indra 20170613

        ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
        DateTime date = DateTime.Now;   
       
        IDictionary<string, string> dic = null;

        nipEntry = (structure.Fields.Entry ?? string.Empty);
        doID = structure.Fields.Field[0].DOid;

        doh = (from q in db.LG_DOHs
               where q.c_dono == doID
               select q).Take(1).SingleOrDefault();


        decimal nDod1 = (from q in db.LG_DOD1s
                         where q.c_dono == doID
                         select q.n_qty.Value).Sum(x => x);


        decimal nDod2 = (from q in db.LG_DOD2s
                         where q.c_dono == doID
                         select q.n_qty.Value).Sum(x => x);

        if (string.IsNullOrEmpty(nipEntry))
        {
            result = "Nip penanggung jawab dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
        }

        bool hasAnyChanges = false,
          isSent = false,
          isSendMethod = false;
        
        //doID = (structure.Fields.DOid ?? string.Empty);

        try
        {
            db.Connection.Open();
            db.Transaction = db.Connection.BeginTransaction();
            int nLoop;
            bool send;
            
            if (structure.Method.Equals("ConfirmSent", StringComparison.OrdinalIgnoreCase))
            {

             #region ConfirmSent

              for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
              {
                field = structure.Fields.Field[nLoop];
                char gudang = (string.IsNullOrEmpty(field.Gudang) || (field.Gudang.Length < 1) ? '1' : field.Gudang[0]);

                doID = field.DOid;
                send = field.Send;

                bool isDO = (doID.Substring(0, 2).Equals("DO"));

                if (string.IsNullOrEmpty(doID))
                {
                  result = "Nomor DO dibutuhkan.";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  if (db.Transaction != null)
                  {
                    db.Transaction.Rollback();
                  }

                  goto endLogic;
                }

                if (isDO)
                {
                  #region DO


                  doh = (from q in db.LG_DOHs
                         where q.c_dono == doID
                         select q).Take(1).SingleOrDefault();

                  if (doh == null)
                  {
                    result = "Nomor Do tidak ditemukan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                      db.Transaction.Rollback();
                    }

                    goto endLogic;
                  }
                  else if (doh.l_delete.HasValue && doh.l_delete.Value)
                  {
                    result = "Tidak dapat mengirim DO yang terhapus.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                      db.Transaction.Rollback();
                    }

                    goto endLogic;
                  }
                  else if (doh.l_print.HasValue && (!doh.l_print.Value))
                  {
                    result = "Tidak dapat mengirim DO yang belum tercetak.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                      db.Transaction.Rollback();
                    }

                    goto endLogic;
                  }
                  else if (!string.IsNullOrEmpty(doh.c_rnno))
                  {
                      result = "Tidak dapat mengirim DO yang sudah jadi RN Cabang.";

                      rpe = ResponseParser.ResponseParserEnum.IsFailed;

                      if (db.Transaction != null)
                      {
                          db.Transaction.Rollback();
                      }

                      goto endLogic;
                  }

                  if (!string.IsNullOrEmpty(doh.c_plphar))
                  {
                      List<LG_FJD1> lstFjd1 = new List<LG_FJD1>();
                      List<LG_FJD2> lstFjd2 = new List<LG_FJD2>();

                      string noFj = string.Concat("FJ", doh.c_dono.Substring(2)).Trim();

                      fjh = (from q in db.LG_FJHs
                             where q.c_fjno == noFj
                             select q).SingleOrDefault();

                      //Indra 20170613
                      if (fjh != null)
                      {
                          NetFJNO = (fjh.n_net.HasValue ? fjh.n_net.Value : 0);
                          SisaFJNO = (fjh.n_sisa.HasValue ? fjh.n_sisa.Value : 0);
                          SisaFJNO = SisaFJNO - NetFJNO;
                      }
                      else
                      {
                          SisaFJNO = 0;
                      }
                      //End

                      if (fjh != null)
                      {
                          db.LG_FJHs.DeleteOnSubmit(fjh);
                      }

                      #region Mark By Indra FJ Detail Tidak udah di delete
                      //lstFjd1 = (from q in db.LG_FJD1s
                      //           where q.c_fjno == noFj
                      //           select q).Distinct().ToList();

                      //if (lstFjd1.Count > 0)
                      //{
                      //    db.LG_FJD1s.DeleteAllOnSubmit(lstFjd1.ToArray());
                      //    lstFjd1.Clear();
                      //}

                      //lstFjd2 = (from q in db.LG_FJD2s
                      //           where q.c_fjno == noFj
                      //           select q).Distinct().ToList();

                      //if (lstFjd2.Count > 0)
                      //{
                      //    db.LG_FJD2s.DeleteAllOnSubmit(lstFjd2.ToArray());
                      //    lstFjd2.Clear();
                      //}

                     
                      #endregion

                      fjd3 = (from q in db.LG_FJD3s
                              where q.c_fjno == noFj
                              select q).SingleOrDefault();

                      if (fjd3 != null)
                      {
                          db.LG_FJD3s.DeleteOnSubmit(fjd3);
                      }

                      doh.l_receipt = false;

                      db.SubmitChanges();
                  }

                  fjd3 = new LG_FJD3();
                  fjd3 = (from q in db.LG_FJD3s
                          where q.c_dono == doID
                          select q).Take(1).SingleOrDefault();

                  isSent = (doh.l_sent.HasValue ? doh.l_sent.Value : false);
                  custNo = (string.IsNullOrEmpty(doh.c_cusno) ? string.Empty : doh.c_cusno.Trim());

                  isSendMethod = true;

                  if ((isSent))
                  {
                    doh.l_send = field.Send;
                    doh.l_sent = field.Send;
                    doh.c_update = nipEntry;
                    doh.d_update = date;

                    hasAnyChanges = true;
                    result = "DO berhasil terkirim ulang";
                    bool isReceipt = (doh.l_receipt.HasValue ? doh.l_receipt.Value : false);

                    if (isSendMethod)
                    {
                      if (nDod1 == nDod2)
                      {
                        if (fjd3 == null)
                        {
                          //Indra D. 20170613
                          //Commons.RunningGenerateFJ(db, custNo, doID, doID, nipEntry);
                            Commons.RunningGenerateFJ(db, custNo, doID, doID, nipEntry, SisaFJNO);
                        }
                        else
                        {
                          //if (!isReceipt)
                          //{

                          //}
                          Modules.CommonQuerySP sp = new ScmsSoaLibrary.Modules.CommonQuerySP();
                          if (sp.PostDataDO(db.Connection.ConnectionString, doID, false))
                          {
                              hasAnyChanges = false;
                          }
                        }
                      }
                      else
                      {
                        result = "Tidak dapat mengirim DO, Detil berbeda.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                          db.Transaction.Rollback();
                        }
                      }
                    }
                  }
                  else
                  {
                    result = "DO masih pending di cabang";
                  }

                  #endregion
                }
              }
             #endregion
            }

            if (hasAnyChanges)
            {
                db.SubmitChanges();

                db.Transaction.Commit();
                //db.Transaction.Rollback();

                rpe = ResponseParser.ResponseParserEnum.IsSuccess;
                result = "Data berhasil terkirim.";
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

            result = string.Format("ScmsSoaLibrary.Bussiness.Penjualan:DOSEND - {0}", ex.Message);

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

    public string EkspedisiDO(ScmsSoaLibrary.Parser.Class.ExpedisiStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false;

      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      LG_ExpH exph = null;
      LG_ExpH exph1 = null;
      LG_ExpD expd = null;
      LG_ExpD2 expd2 = null;
      LG_ExpErr expErr = null;
      LG_IED1 ied = null;

      ScmsSoaLibrary.Parser.Class.ExpedisiStructureField field = null;
      string nipEntry = null;
      string expID = null;
      string noWP = null;
      //string tmpNumbering = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      List<LG_ExpD> listExpD = null;
      List<LG_ExpD2> listExpD2 = null;
      List<LG_ExpD1> listExp1 = null;
      List<LG_ExpErr> listExpErr = null;

      List<string> listDO = null;
      List<LG_DOH> listResDOH = null;
      List<LG_DOD1> listDOD1 = null;
      LG_DOH doh = null;
      LG_DOD1 dod1 = null;

      List<string> listSJ = null;
      List<LG_SJH> listResSJH = null;
      //List<LG_SJH1> listResSJH1 = null;
      LG_SJH sjh = null;
      //LG_SJH1 sjh1 = null;

      List<string> listRS = null;
      List<LG_RSH> listResRSH = null;
      LG_RSH rsh = null;

      List<SCMS_DRIVER> listDriver = null;

      DOClassComponent doc = new DOClassComponent();
      List<DOClassComponent> lstDoc = new List<DOClassComponent>();
      
      string doID = null,
         sTempDOcancel = null,
         sTempDOErr = null,
         supplier = null;

      char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '1' : structure.Fields.Gudang[0]);

      decimal berat = (structure.Fields.Berat <= 0 ? 0 : structure.Fields.Berat);

      decimal koli = (structure.Fields.Koli <= 0 ? 0 : structure.Fields.Koli);

      decimal receh = (structure.Fields.Receh <= 0 ? 0 : structure.Fields.Receh);

      decimal volume = (structure.Fields.Volume <= 0 ? 0 : structure.Fields.Volume);

      //DateTime Wtime = (string.IsNullOrEmpty(structure.Fields.DResi) ? DateTime.Today.ToUniversalTime() : );

      int nLoop = 0,
       nLoopC = 0;

      IDictionary<string, string> dic = null;

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        //goto endLogic;
      }

      //listDriver = (from q in db.SCMS_DRIVERs
      //              where q.c_nip == structure.Fields.Driver
      //              select q).AsQueryable().ToList();

      //if (listDriver.Count == 0)
      //{
      //    result = "Driver tidak terdaftar. Mohon daftarkan di Master Driver terlebih dahulu.";

      //    rpe = ResponseParser.ResponseParserEnum.IsFailed;

      //    if (db.Transaction != null)
      //    {
      //        db.Transaction.Rollback();
      //    }

      //    goto endLogic;
      //}
      //else
      //{
      //    listDriver.Clear();
      //}
      int totalDetails = 0;

      expID = (structure.Fields.EkspedisiID ?? string.Empty);

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          if (!string.IsNullOrEmpty(expID))
          {
            result = "Nomor Exp harus kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Expedisi))
          {
            result = "Nama Ekspedisi dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Customer))
          {
            result = "Nama cabang dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Gudang))
          {
            result = "Gudang dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          //else if (string.IsNullOrEmpty(structure.Fields.Driver) || string.IsNullOrEmpty(structure.Fields.Nopol))
          //{
          //    result = "Info Driver dibutuhkan.";

          //    rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //    if (db.Transaction != null)
          //    {
          //        db.Transaction.Rollback();
          //    }

          //    goto endLogic;
          //}
          else if (Commons.IsClosingLogistik(db, date))
          {
            result = "DO tidak dapat disimpan, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (!string.IsNullOrEmpty(structure.Fields.Ref))
          {
              exph1 = (from q in db.LG_ExpHs
                       where q.c_expno == structure.Fields.Ref && (q.c_ref != null && q.c_ref != "")
                       select q).Take(1).SingleOrDefault();

              if (exph1 != null)
              {
                  result = "Tidak dapat referensikan EP yang ada referensi lain.";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  if (db.Transaction != null)
                  {
                      db.Transaction.Rollback();
                  }

                  goto endLogic;
              }
          }

          //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "PL");

          expID = Commons.GenerateNumbering<LG_ExpH>(db, "EP", '3', "10", date, "c_expno");

          exph = new LG_ExpH()
          {
            c_expno = expID,
            c_cusno = structure.Fields.Customer,
            c_asal = structure.Fields.AsalKirim, //penambahan direct shipment by suwandi 8 juni 2018
            c_by = structure.Fields.By,
            c_gdg = gudang,
            d_resi = structure.Fields.DateResi,
            c_entry = structure.Fields.Entry,
            c_exp = structure.Fields.Expedisi,
            v_exp = structure.Fields.ExpedisiPlus,
            c_resi = structure.Fields.By == "01" || structure.Fields.By == "4" || structure.Fields.By == "04" ? structure.Fields.Resi : expID, //penambahan direct shipment by suwandi 8 juni 2018
            c_via = structure.Fields.Via,
            c_update = structure.Fields.Entry,
            c_type = structure.Fields.TipeExpedisi,
            d_expdate = date.Date,
            d_entry = date,
            l_print = false,
            v_ket = structure.Fields.Ket,
            n_berat = berat,
            n_koli = koli,
            n_receh = receh,
            n_vol = volume,
            c_driver = structure.Fields.Driver,
            c_nopol = structure.Fields.Nopol,
            d_update = date,
            c_ref = (string.IsNullOrEmpty(structure.Fields.Ref) == true ? "" : structure.Fields.Ref),
            c_exptype = structure.Fields.TipeExp,
            n_biayalain = structure.Fields.BiayaLain,
            n_totalbiaya = structure.Fields.TotalBiaya,
            n_biayakg = structure.Fields.BiayaKg,
            n_expmin = structure.Fields.expMin,
            l_iscab = structure.Fields.isCabang,
            c_cusno2 = (string.IsNullOrEmpty(structure.Fields.Customer2) == true ? null : structure.Fields.Customer2)
          };

          db.LG_ExpHs.InsertOnSubmit(exph);      

          #region Old Code

          //db.SubmitChanges();

          //exph = (from q in db.LG_ExpHs
          //        where q.v_ket == tmpNumbering
          //        select q).Take(1).SingleOrDefault();

          //if (!string.IsNullOrEmpty(expID))
          //{
          //  result = "Nomor Ekspedisi tidak dapat di raih.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}
          //else if (exph.c_expno.Equals("XXXXXXXXXX"))
          //{
          //  result = "Trigger Ekspedisi tidak aktif.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}

          //exph.v_ket = structure.Fields.Ket;

          //expID = exph.c_expno;

          #endregion

          #region Insert Detail

          Dictionary<string, List<EKSP_KOLI>> sKey = new Dictionary<string, List<EKSP_KOLI>>();
          listExpD2 = new List<LG_ExpD2>();

          if (structure.Fields.TipeExpedisi.Equals("01"))
          { 
            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
            {
                listExpD = new List<LG_ExpD>();     
                listExpErr = new List<LG_ExpErr>();
                

              listDO = structure.Fields.Field.GroupBy(x => x.DO).Select(y => y.Key).ToList();

              // Prodoction
              //listResDOH = (from q in db.LG_DOHs
              //              where listDO.Contains(q.c_dono)
              //              select q).Distinct().ToList();

              //Will Be Delete (A)
              listResDOH = (from q in db.LG_DOHs
                            where listDO.Contains(q.c_dono)
                            select q).Distinct().ToList();
              //(A)

              for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
              {
                field = structure.Fields.Field[nLoop];
                if ((field != null) && field.IsNew && (!field.IsDelete))
                {
                  //Production

                  //if ((doh != null) && (!(doh.l_status.HasValue ? doh.l_status.Value : false)))
                 if (structure.Fields.By.ToString() == "4")
                 {
                     string principal = (from q in db.LG_RNHs
                                         where q.c_dono == field.DO && q.c_gdg == '1' && (q.l_delete == false || q.l_delete == null)
                                             select q.c_from).SingleOrDefault();
                     
                     var cek2 = (from q in db.LG_ExpDs
                                 where q.c_dono == field.DO
                                 select q).SingleOrDefault();
                     #region remark by suwandi 10 Oktober 2018 karena penyatuan direct shipment
                     //var cek = (from q in db.LG_RNHs
                     //           where q.c_dono == field.DO && q.c_gdg == '1' && (q.l_delete == false || q.l_delete == null)
                     //           select q).SingleOrDefault();
                     //if (cek == null)
                     //{
                     //    result = "Nomor DO " + field.DO + " tidak ada";
                     //    rpe = ResponseParser.ResponseParserEnum.IsFailed;
                     //    if (db.Transaction != null)
                     //    { 
                     //        db.Transaction.Rollback();
                     //    }
                     //    goto endLogic;
                     //}
                     #endregion

                     if (cek2 != null)
                     {
                         result = "Nomor DO " + field.DO + " sudah pernah di masukkan";
                         rpe = ResponseParser.ResponseParserEnum.IsFailed;
                         if (db.Transaction != null)
                         {
                             db.Transaction.Rollback();
                         }
                         goto endLogic;
                     }

                     if (supplier == null)
                     {
                         supplier = principal;
                     }
                     else if (supplier != null)
                     {
                         if (supplier != principal)
                         {
                             result = "tidak dapat menginput DO untuk 2 principal yang berbeda dalam 1 ekspedisi";
                             rpe = ResponseParser.ResponseParserEnum.IsFailed;
                             if (db.Transaction != null)
                             {
                                 db.Transaction.Rollback();
                             }
                             goto endLogic;
                         }
                         else if (supplier == principal)
                         {
                             supplier = principal;
                         }
                     }

                     if (!sKey.ContainsKey(field.partno))
                     {
                         listExpD2.Add(new LG_ExpD2()
                         {
                             c_expno = expID,
                             n_koli = field.koli,
                             n_receh = field.receh,
                             n_berat = field.berat,
                             n_vol = field.volume,
                             c_nopart = field.partno,
                         });
                     }

                     listExpD.Add(new LG_ExpD()
                     {
                         c_dono = field.DO,
                         c_expno = expID,
                         c_nopart = field.partno,
                     });

                     // Production

                     totalDetails++;

                 }
                 else if (structure.Fields.By.ToString() != "4") //penambahan direct shipment by suwandi 
                 {
                     doh = listResDOH.Find(delegate(LG_DOH dohdr)
                     {
                         return field.DO.Equals((string.IsNullOrEmpty(dohdr.c_dono) ? string.Empty : dohdr.c_dono), StringComparison.OrdinalIgnoreCase)
                           && structure.Fields.Customer.Equals((string.IsNullOrEmpty(dohdr.c_cusno) ? string.Empty : dohdr.c_cusno), StringComparison.OrdinalIgnoreCase);
                     });
                     noWP = (from q in db.SCMS_STHs
                             join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                             where q1.c_no == field.DO && q.c_type == "04"
                             select q.c_nodoc).Take(1).SingleOrDefault();
                     if (structure.Fields.By != "4" || structure.Fields.By != "04") //penambahan direct shipment by suwandi 
                     {

                         if (string.IsNullOrEmpty(noWP))
                         {
                             result = field.DO + " belum discan Team Packer. Mohon dikembalikan ke Team Packer";

                             rpe = ResponseParser.ResponseParserEnum.IsFailed;

                             if (db.Transaction != null)
                             {
                                 db.Transaction.Rollback();
                             }

                             goto endLogic;
                         }
                     }
                     if ((doh != null))
                     {
                         if (doh.c_gdg != gudang)
                         {
                             sTempDOcancel = sTempDOcancel + "," + field.DO;
                             continue;
                         }

                         if (doh.d_entry.Value > structure.Fields.DateResi)
                         {
                             listExpErr.Add(new LG_ExpErr()
                             {
                                 c_gdg = gudang,
                                 c_expno = expID,
                                 d_resi = structure.Fields.DateResi,
                                 d_doentry = doh.d_entry,
                                 c_dono = field.DO,
                             });

                             sTempDOErr = sTempDOErr + Environment.NewLine + field.DO + " - Waktu DO Entry " + doh.d_entry.Value.ToString("dd.MM.yyyy hh:mm:ss");
                             rpe = ResponseParser.ResponseParserEnum.IsFailed;

                             result = sTempDOErr;
                             goto endLogic;

                             //continue; 
                         }

                         listDOD1 = (from q in db.LG_DOD1s
                                     where (q.c_dono == field.DO)
                                     select q).ToList();

                         //for (nLoopC = 0; nLoopC < listDOD1.Count; nLoopC++)
                         //{
                         //  dod1 = listDOD1[nLoopC];

                         //  //berat = (dod1.n_qty.HasValue ? dod1.n_qty.Value : 0);
                         //}

                         listDOD1.Clear();

                         List<EKSP_KOLI> lstPart = new List<EKSP_KOLI>()
                    {
                      new EKSP_KOLI()
                      {
                        c_part = field.partno,
                      }
                    };


                         if (!sKey.ContainsKey(field.partno))
                         {
                             sKey.Add(field.partno, lstPart);

                             listExpD2.Add(new LG_ExpD2()
                             {
                                 c_expno = expID,
                                 n_koli = field.koli,
                                 n_receh = field.receh,
                                 n_berat = field.berat,
                                 n_vol = field.volume,
                                 c_nopart = field.partno,
                             });
                         }

                         listExpD.Add(new LG_ExpD()
                         {
                             c_dono = field.DO,
                             c_expno = expID,
                             c_nopart = field.partno,
                         });

                         // Production
                         doh.l_status = true;

                         doh.l_auto = true;

                         totalDetails++;
                     }
                     else
                     {
                         sTempDOcancel = sTempDOcancel + "," + field.DO;
                     }
                 }
                }
              }

              // Production
              listResDOH.Clear();

              listDO.Clear();
            }
          }
          else if (structure.Fields.TipeExpedisi.Equals("02"))
          {
            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
            {
              listExpD = new List<LG_ExpD>();
              listExpErr = new List<LG_ExpErr>();

              listSJ = structure.Fields.Field.GroupBy(x => x.DO).Select(y => y.Key).ToList();

              // Production
              //listResSJH = (from q in db.LG_SJHs
              //              where listSJ.Contains(q.c_sjno)
              //              select q).Distinct().ToList();

              //Will Be Delete (A)
              listResSJH = (from q in db.LG_SJHs
                            where listSJ.Contains(q.c_sjno) && q.l_confirm == true 
                            select q).Distinct().ToList();
              //(A)

              for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
              {
                field = structure.Fields.Field[nLoop];
                if ((field != null) && field.IsNew && (!field.IsDelete))
                {
                    // Production
                  //sjh = listResSJH.Find(delegate(LG_SJH dohdr)
                  //{
                  //  return field.DO.Equals((string.IsNullOrEmpty(dohdr.c_sjno) ? string.Empty : dohdr.c_sjno), StringComparison.OrdinalIgnoreCase);
                  //});

                  //Will Be Delete (A)
                  sjh = listResSJH.Find(delegate(LG_SJH sjhdr)
                  {
                      return field.DO.Equals((string.IsNullOrEmpty(sjhdr.c_sjno) ? string.Empty : sjhdr.c_sjno), StringComparison.OrdinalIgnoreCase);
                  });
                  //(A)

                  //if ((sjh != null) && (!(sjh.l_status.HasValue ? sjh.l_status.Value : false)))

                  if ((sjh != null) && (!(sjh.l_exp.HasValue ? sjh.l_exp.Value : false)))
                  {
                      #region validasi serah terima
                      if (sjh.c_type == "01")
                      {
                          if (sjh.l_auto == null || sjh.l_auto == false)
                          {
                              string[] typeWP = new string[] { "01", "02" };
                              noWP = (from q in db.SCMS_STHs
                                      join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                      where q1.c_no == field.DO && typeWP.Contains(q.c_type)
                                      select q.c_nodoc).Take(1).SingleOrDefault();

                              if (string.IsNullOrEmpty(noWP))
                              {
                                  result = field.DO + " belum discan Team Picker. Mohon dikembalikan ke Team Picker";

                                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                  if (db.Transaction != null)
                                  {
                                      db.Transaction.Rollback();
                                  }

                                  goto endLogic;
                              }

                              noWP = (from q in db.SCMS_STHs
                                      join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                      where q1.c_no == field.DO && q.c_type == "03"
                                      select q.c_nodoc).Take(1).SingleOrDefault();

                              if (string.IsNullOrEmpty(noWP))
                              {
                                  result = field.DO + " belum discan Team Checker. Mohon dikembalikan ke Team Checker";

                                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                  if (db.Transaction != null)
                                  {
                                      db.Transaction.Rollback();
                                  }

                                  goto endLogic;
                              }
                          }
                          noWP = (from q in db.SCMS_STHs
                                  join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                  where q1.c_no == field.DO && q.c_type == "04"
                                  select q.c_nodoc).Take(1).SingleOrDefault();

                          if (string.IsNullOrEmpty(noWP))
                          {
                              result = field.DO + " belum discan Team Packer. Mohon dikembalikan ke Team Packer";

                              rpe = ResponseParser.ResponseParserEnum.IsFailed;

                              if (db.Transaction != null)
                              {
                                  db.Transaction.Rollback();
                              }

                              goto endLogic;
                          }
                      }
                      #endregion

                      if (sjh.c_gdg != gudang)
                      {
                          sTempDOcancel = sTempDOcancel + "," + field.DO;
                          continue;
                      }

                      if (sjh.d_entry.Value > structure.Fields.DateResi)
                      {
                          listExpErr.Add(new LG_ExpErr()
                          {
                              c_gdg = gudang,
                              c_expno = expID,
                              d_resi = structure.Fields.DateResi,
                              d_doentry = sjh.d_entry,
                              c_dono = field.DO,
                          });

                          sTempDOErr = sTempDOErr + Environment.NewLine + field.DO + " - Waktu SJ Entry " + sjh.d_entry.Value.ToString("dd.MM.yyyy hh:mm:ss") + Environment.NewLine;

                          rpe = ResponseParser.ResponseParserEnum.IsFailed;

                          result = sTempDOErr;
                          //continue;
                          goto endLogic;
                      }


                      List<EKSP_KOLI> lstPart = new List<EKSP_KOLI>()
                      {
                        new EKSP_KOLI()
                        {
                          c_part = field.partno,
                        }
                      };


                      if (!sKey.ContainsKey(field.partno))
                      {
                        sKey.Add(field.partno, lstPart);

                        listExpD2.Add(new LG_ExpD2()
                        {
                          c_expno = expID,
                          c_nopart = field.partno,
                          n_berat = field.berat,
                          n_koli = field.koli,
                          n_receh = field.receh,
                          n_vol = field.volume
                        });
                      }

                      listExpD.Add(new LG_ExpD()
                      {
                          c_dono = field.DO,
                          c_expno = expID,
                          c_nopart = field.partno
                      });

                      // Production
                      //sjh.l_status = true;

                      //Will Be Delete
                      sjh.l_exp = true;

                      totalDetails++;
                  }
                  else
                  {
                      sTempDOcancel = sTempDOcancel + "," + field.DO;
                  }
                }
              }
              // Production
              //listResSJH.Clear();

              // Will Be Delete (A)
              listResSJH.Clear();
              //(A)
              listSJ.Clear();
            }
          }
          else if (structure.Fields.TipeExpedisi.Equals("03"))
          {
              if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
              {
                  listExpD = new List<LG_ExpD>();
                  listExpErr = new List<LG_ExpErr>();

                  listRS = structure.Fields.Field.GroupBy(x => x.DO).Select(y => y.Key).ToList();

                  listResRSH = (from q in db.LG_RSHes
                                where listRS.Contains(q.c_rsno) && q.l_print == true
                                select q).Distinct().ToList();

                  for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                  {
                      field = structure.Fields.Field[nLoop];
                      if ((field != null) && field.IsNew && (!field.IsDelete))
                      {
                          rsh = listResRSH.Find(delegate(LG_RSH rshdr)
                          {
                              return field.DO.Equals((string.IsNullOrEmpty(rshdr.c_rsno) ? string.Empty : rshdr.c_rsno), StringComparison.OrdinalIgnoreCase);
                          });
                          //(A)

                          if ((rsh != null) && (!(rsh.l_exp.HasValue ? rsh.l_exp.Value : false)))
                          {
                              if (rsh.c_gdg != gudang)
                              {
                                  sTempDOcancel = sTempDOcancel + "," + field.DO;
                                  continue;
                              }

                              if (rsh.d_entry.Value > structure.Fields.DateResi)
                              {
                                  listExpErr.Add(new LG_ExpErr()
                                  {
                                      c_gdg = gudang,
                                      c_expno = expID,
                                      d_resi = structure.Fields.DateResi,
                                      d_doentry = rsh.d_entry,
                                      c_dono = field.DO,
                                  });

                                  sTempDOErr = sTempDOErr + Environment.NewLine + field.DO + " - Waktu RS Entry " + rsh.d_entry.Value.ToString("dd.MM.yyyy hh:mm:ss") + Environment.NewLine;

                                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                  result = sTempDOErr;
                                  //continue;
                                  goto endLogic;
                              }


                              List<EKSP_KOLI> lstPart = new List<EKSP_KOLI>()
                      {
                        new EKSP_KOLI()
                        {
                          c_part = field.partno,
                        }
                      };


                              if (!sKey.ContainsKey(field.partno))
                              {
                                  sKey.Add(field.partno, lstPart);

                                  listExpD2.Add(new LG_ExpD2()
                                  {
                                      c_expno = expID,
                                      c_nopart = field.partno,
                                      n_berat = field.berat,
                                      n_koli = field.koli,
                                      n_receh = field.receh,
                                      n_vol = field.volume
                                  });
                              }

                              listExpD.Add(new LG_ExpD()
                              {
                                  c_dono = field.DO,
                                  c_expno = expID,
                                  c_nopart = field.partno
                              });


                              rsh.l_exp = true;

                              totalDetails++;
                          }
                          else
                          {
                              sTempDOcancel = sTempDOcancel + "," + field.DO;
                          }
                      }
                  }

                  listResRSH.Clear();

                  listRS.Clear();
              }
          }

          if (listExpD2.Count > 0)
          {
            db.LG_ExpD2s.InsertAllOnSubmit(listExpD2.ToArray());
            listExpD2.Clear();
          }

          if (listExpD.Count > 0)
          {
            db.LG_ExpDs.InsertAllOnSubmit(listExpD.ToArray());
            listExpD.Clear();
          }

          if (listExpD.Count > 0)
          {
            db.LG_ExpDs.InsertAllOnSubmit(listExpD.ToArray());
            listExpD.Clear();
          }

          if (listExpErr.Count > 0)
          {
              db.LG_ExpErrs.InsertAllOnSubmit(listExpErr.ToArray());
              listExpErr.Clear();
          }

          #endregion

          dic = new Dictionary<string, string>();

          if (totalDetails > 0)
          {
            dic.Add("EXP", expID);
            dic.Add("Tanggal", date.ToString("yyyyMMdd"));
            dic.Add("DO_Salah", sTempDOcancel);
            dic.Add("DO_Err_WP", sTempDOErr);

            result = string.Format("Total {0} detail(s)", totalDetails);

            hasAnyChanges = true;
          }
          else
          {
            hasAnyChanges = false;
          }

          #endregion
        }
        if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify

          if (string.IsNullOrEmpty(expID))
          {
            result = "Nomor Ekspedisi dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
         
          exph = (from q in db.LG_ExpHs
                  where q.c_expno == expID
                  select q).Take(1).SingleOrDefault();

          if (exph == null)
          {
            result = "Nomor Ekspedisi tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (exph.l_delete.HasValue && exph.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor Packing List yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          //else if (Commons.IsClosingLogistik(db, exph.d_expdate))
          //{
          //  result = "Expedisi tidak dapat diubah, karena sudah closing.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}

          ied = (from q in db.LG_IED1s
                 where q.c_expno == expID
                 select q).Take(1).SingleOrDefault();

          if (ied != null)
          {
              result = "Ekspedisi sudah dibuat invoice";

              rpe = ResponseParser.ResponseParserEnum.IsFailed;

              if (db.Transaction != null)
              {
                  db.Transaction.Rollback();
              }

              goto endLogic;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Ref))
          {
              exph1 = (from q in db.LG_ExpHs
                       where q.c_ref == expID
                       select q).Take(1).SingleOrDefault();

              if (exph1 != null)
              {
                  result = "No Exp. sudah direferensikan";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  if (db.Transaction != null)
                  {
                      db.Transaction.Rollback();
                  }

                  goto endLogic;
              }
          }

          if (!string.IsNullOrEmpty(structure.Fields.Ket))
          {
            exph.v_ket = structure.Fields.Ket;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Resi))
          {
            exph.c_resi = structure.Fields.Resi;
          }

          if (!string.IsNullOrEmpty(structure.Fields.DateResi.ToShortDateString()))
          {
            exph.d_resi = structure.Fields.DateResi;
          }

          if (koli >= 0)
          {
            exph.n_koli = koli;
          }

          if (receh >= 0)
          {
              exph.n_receh = receh;
          }

          if (berat >= 0)
          {
            exph.n_berat = berat;
          }

          if (volume >= 0)
          {
              exph.n_vol = volume;
          }

          if (structure.Fields.DateResi.Millisecond > 0)
          {
            exph.d_resi = structure.Fields.DateResi;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Expedisi))
          {
            exph.c_exp = structure.Fields.Expedisi;
          }

          if (!string.IsNullOrEmpty(structure.Fields.ExpedisiPlus))
          {
              exph.v_exp= structure.Fields.ExpedisiPlus;
          }

          if (!string.IsNullOrEmpty(structure.Fields.By))
          {
            exph.c_by = structure.Fields.By;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Driver))
          {
              exph.c_driver = structure.Fields.Driver;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Nopol))
          {
              exph.c_nopol = structure.Fields.Nopol;
          }

          exph.c_ref = string.IsNullOrEmpty(structure.Fields.Ref) ? "" : structure.Fields.Ref;
          exph.c_via = structure.Fields.Via;
          exph.c_exptype = structure.Fields.TipeExp;
          exph.n_biayalain = structure.Fields.BiayaLain;
          exph.n_totalbiaya = structure.Fields.TotalBiaya;
          exph.n_biayakg = structure.Fields.BiayaKg;
          exph.n_expmin = structure.Fields.expMin;
          exph.l_iscab = structure.Fields.isCabang;
          exph.c_cusno2 = (string.IsNullOrEmpty(structure.Fields.Customer2) == true ? null : structure.Fields.Customer2);
          exph.c_asal = structure.Fields.AsalKirim; //penambahan direct shipment by suwandi 8 juni 2018
          exph.d_update = DateTime.Now;
          #region populate detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            //listDO = new List<string>();
            //listExpD = new List<LG_ExpD>();
            //listResDOD1 = new List<LG_DOD1>();
            listExp1 = new List<LG_ExpD1>();
            listExpErr = new List<LG_ExpErr>();
            Dictionary<string, List<EKSP_KOLI>> sKey = new Dictionary<string, List<EKSP_KOLI>>();
            listExpD2 = new List<LG_ExpD2>();

            listExpD = (from q in db.LG_ExpDs
                        where q.c_expno == expID
                        select q).Distinct().ToList();

            listDO = structure.Fields.Field.GroupBy(x => x.DO).Select(y => y.Key).ToList();


            listResDOH = (from q in db.LG_DOHs
                          where listDO.Contains(q.c_dono)
                          select q).Distinct().ToList();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              if ((field != null) && field.IsNew && (!field.IsDelete))
              {
                #region New

                if (structure.Fields.TipeExpedisi.Equals("01"))
                {
                    if (structure.Fields.By != "04" && structure.Fields.By != "4")
                    {
                        noWP = (from q in db.SCMS_STHs
                                join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                where q1.c_no == field.DO && q.c_type == "04"
                                select q.c_nodoc).Take(1).SingleOrDefault();

                        if (string.IsNullOrEmpty(noWP))
                        {
                            result = field.DO + " belum discan Team Packer. Mohon dikembalikan ke Team Packer";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                    }

                  doh = (from q in db.LG_DOHs
                         where q.c_dono == field.DO
                         select q).Take(1).SingleOrDefault();

                  doh = listResDOH.Find(delegate(LG_DOH dohdr)
                  {
                    return field.DO.Equals((string.IsNullOrEmpty(dohdr.c_dono) ? string.Empty : dohdr.c_dono), StringComparison.OrdinalIgnoreCase)
                      && structure.Fields.Customer.Equals((string.IsNullOrEmpty(dohdr.c_cusno) ? string.Empty : dohdr.c_cusno), StringComparison.OrdinalIgnoreCase);
                  });

                  if ((doh != null))
                  {
                    if (doh.c_gdg != gudang)
                    {
                        sTempDOcancel = sTempDOcancel + "," + field.DO;
                        continue;
                    }

                    if (doh.d_entry.Value > structure.Fields.DateResi)
                    {
                        //expErr = new LG_ExpErr()
                        //{
                        //    c_gdg = gudang,
                        //    c_expno = expID,
                        //    d_resi = structure.Fields.DateResi,
                        //    d_doentry = doh.d_entry,
                        //    c_dono = field.DO,
                        //};

                        sTempDOErr = sTempDOErr + Environment.NewLine + field.DO + " - Waktu DO Entry " + doh.d_entry.Value.ToString("dd.MM.yyyy hh:mm:ss");
                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        result = sTempDOErr;
                        goto endLogic;
                        //db.LG_ExpErrs.InsertOnSubmit(expErr);                                                
                        //continue;
                    }

                    listExpErr = (from q in db.LG_ExpErrs
                              where q.c_dono == field.DO
                              select q).ToList();

                    if (listExpErr.Count > 0)
                    {
                        db.LG_ExpErrs.DeleteAllOnSubmit(listExpErr);
                        listExpErr.Clear();                        
                    }

                    doh.l_status = true;
                    doh.l_auto = true;

                    listDOD1 = (from q in db.LG_DOD1s
                                where (q.c_dono == field.DO)
                                select q).ToList();

                    for (nLoopC = 0; nLoopC < listDOD1.Count; nLoopC++)
                    {
                      dod1 = listDOD1[nLoopC];
                    }

                    listDOD1.Clear();

                    List<EKSP_KOLI> lstPart = new List<EKSP_KOLI>()
                    {
                      new EKSP_KOLI()
                      {
                        c_part = field.partno,
                      }
                    };

                    if (!sKey.ContainsKey(field.partno))
                    {
                        expd2 = (from q in db.LG_ExpD2s
                                 where q.c_nopart == field.partno && q.c_expno == expID
                                 select q).Take(1).SingleOrDefault();
                        if (expd2 == null)
                        {
                            sKey.Add(field.partno, lstPart);
                            listExpD2.Add(new LG_ExpD2()
                            {
                                c_expno = expID,
                                n_koli = field.koli,
                                n_receh = field.receh,
                                n_berat = field.berat,
                                n_vol = field.volume,
                                c_nopart = field.partno,
                            });
                        }
                    }
                    expd = new LG_ExpD()
                    {
                      c_expno = expID,
                      c_dono = field.DO,
                      c_nopart = field.partno,
                    };

                    db.LG_ExpDs.InsertOnSubmit(expd);

                  }
                  else
                  {
                      sTempDOcancel = sTempDOcancel + "," + field.DO;
                  }
                }
                else if (structure.Fields.TipeExpedisi.Equals("02"))
                {
                  // Production
                  //sjh = (from q in db.LG_SJHs
                  //       where q.c_sjno == field.DO
                  //       select q).Take(1).SingleOrDefault();

                  // Will Delete (A)
                  sjh = (from q in db.LG_SJHs
                          where q.c_sjno == field.DO && q.l_confirm == true
                         select q).Take(1).SingleOrDefault();
                  // (A)

                  //Production
                  //if ((sjh != null) && (!(sjh.l_status.HasValue ? sjh.l_status.Value : false)))

                  if ((sjh != null) && (!(sjh.l_exp.HasValue ? sjh.l_exp.Value : false)))
                  {
                      #region validasi serah terima
                      if (sjh.c_type == "01")
                      {
                          if (sjh.l_auto == null || sjh.l_auto == false)
                          {
                              string[] typeWP = new string[] { "01", "02" };
                              noWP = (from q in db.SCMS_STHs
                                      join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                      where q1.c_no == field.DO && typeWP.Contains(q.c_type)
                                      select q.c_nodoc).Take(1).SingleOrDefault();

                              if (string.IsNullOrEmpty(noWP))
                              {
                                  result = field.DO + " belum discan Team Picker. Mohon dikembalikan ke Team Picker";

                                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                  if (db.Transaction != null)
                                  {
                                      db.Transaction.Rollback();
                                  }

                                  goto endLogic;
                              }

                              noWP = (from q in db.SCMS_STHs
                                      join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                      where q1.c_no == field.DO && q.c_type == "03"
                                      select q.c_nodoc).Take(1).SingleOrDefault();

                              if (string.IsNullOrEmpty(noWP))
                              {
                                  result = field.DO + " belum discan Team Checker. Mohon dikembalikan ke Team Checker";

                                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                  if (db.Transaction != null)
                                  {
                                      db.Transaction.Rollback();
                                  }

                                  goto endLogic;
                              }
                          }
                          noWP = (from q in db.SCMS_STHs
                                  join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                  where q1.c_no == field.DO && q.c_type == "04" 
                                  select q.c_nodoc).Take(1).SingleOrDefault();

                          if (string.IsNullOrEmpty(noWP))
                          {
                              result = field.DO + " belum discan Team Packer. Mohon dikembalikan ke Team Packer";

                              rpe = ResponseParser.ResponseParserEnum.IsFailed;

                              if (db.Transaction != null)
                              {
                                  db.Transaction.Rollback();
                              }

                              goto endLogic;
                          }
                      }
                      #endregion

                    if (sjh.c_gdg != gudang)
                    {
                        sTempDOcancel = sTempDOcancel + "," + field.DO;
                        continue;
                    }

                    if (sjh.d_entry.Value > structure.Fields.DateResi)
                    {
                        //expErr = new LG_ExpErr()
                        //{
                        //    c_gdg = gudang,
                        //    c_expno = expID,
                        //    d_resi = structure.Fields.DateResi,
                        //    d_doentry = sjh.d_entry,
                        //    c_dono = field.DO,
                        //};

                        sTempDOErr = sTempDOErr + Environment.NewLine + field.DO + " - Waktu SJ Entry " + sjh.d_entry.Value.ToString("dd.MM.yyyy hh:mm:ss");
                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        result = sTempDOErr;
                        goto endLogic;
                        //db.LG_ExpErrs.InsertOnSubmit(expErr);                        
                        //continue;
                    }

                    listExpErr = (from q in db.LG_ExpErrs
                                  where q.c_dono == field.DO
                                  select q).ToList();

                    if (listExpErr.Count > 0)
                    {
                        db.LG_ExpErrs.DeleteAllOnSubmit(listExpErr);
                        listExpErr.Clear();
                    }

                    // Productin
                    //sjh.l_status = true;

                    // Will Delete (A)
                    sjh.l_exp = true;
                    // (A)


                    List<EKSP_KOLI> lstPart = new List<EKSP_KOLI>()
                    {
                      new EKSP_KOLI()
                      {
                        c_part = field.partno,
                      }
                    };

                    if (!sKey.ContainsKey(field.partno))
                    {
                        expd2 = (from q in db.LG_ExpD2s
                                 where q.c_nopart == field.partno && q.c_expno == expID
                                 select q).Take(1).SingleOrDefault();
                        if (expd2 == null)
                        {
                            sKey.Add(field.partno, lstPart);

                            listExpD2.Add(new LG_ExpD2()
                            {
                                c_expno = expID,
                                n_koli = field.koli,
                                n_receh = field.receh,
                                n_berat = field.berat,
                                n_vol = field.volume,
                                c_nopart = field.partno,
                            });
                        }
                    }

                    expd = new LG_ExpD()
                    {
                      c_expno = expID,
                      c_dono = field.DO,
                      c_nopart = field.partno,
                    };

                    db.LG_ExpDs.InsertOnSubmit(expd);
                  }
                  else
                  {
                      sTempDOcancel = sTempDOcancel + "," + field.DO;
                  }
                }
                else if (structure.Fields.TipeExpedisi.Equals("03"))
                {

                    rsh = (from q in db.LG_RSHes
                           where q.c_rsno == field.DO && q.l_print == true
                           select q).Take(1).SingleOrDefault();

                    if ((rsh != null) && (!(rsh.l_exp.HasValue ? rsh.l_exp.Value : false)))
                    {
                        if (rsh.c_gdg != gudang)
                        {
                            sTempDOcancel = sTempDOcancel + "," + field.DO;
                            continue;
                        }

                        if (rsh.d_entry.Value > structure.Fields.DateResi)
                        {

                            sTempDOErr = sTempDOErr + Environment.NewLine + field.DO + " - Waktu RS Entry " + rsh.d_entry.Value.ToString("dd.MM.yyyy hh:mm:ss");
                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            result = sTempDOErr;
                            goto endLogic;
                        }

                        listExpErr = (from q in db.LG_ExpErrs
                                      where q.c_dono == field.DO
                                      select q).ToList();

                        if (listExpErr.Count > 0)
                        {
                            db.LG_ExpErrs.DeleteAllOnSubmit(listExpErr);
                            listExpErr.Clear();
                        }

                        rsh.l_exp = true;

                        List<EKSP_KOLI> lstPart = new List<EKSP_KOLI>()
                    {
                      new EKSP_KOLI()
                      {
                        c_part = field.partno,
                      }
                    };

                        if (!sKey.ContainsKey(field.partno))
                        {
                            expd2 = (from q in db.LG_ExpD2s
                                     where q.c_nopart == field.partno && q.c_expno == expID
                                     select q).Take(1).SingleOrDefault();
                            if (expd2 == null)
                            {
                                sKey.Add(field.partno, lstPart);

                                listExpD2.Add(new LG_ExpD2()
                                {
                                    c_expno = expID,
                                    n_koli = field.koli,
                                    n_receh = field.receh,
                                    n_berat = field.berat,
                                    n_vol = field.volume,
                                    c_nopart = field.partno,
                                });
                            }
                        }

                        expd = new LG_ExpD()
                        {
                            c_expno = expID,
                            c_dono = field.DO,
                            c_nopart = field.partno,
                        };

                        db.LG_ExpDs.InsertOnSubmit(expd);
                    }
                    else
                    {
                        sTempDOcancel = sTempDOcancel + "," + field.DO;
                    }
                }

                #endregion
              }
              else if ((field != null) && (!field.IsNew) && field.IsModify)
              {
                #region Modify

                  expd2 = (from q in db.LG_ExpD2s
                           where q.c_nopart == field.partno && q.c_expno == expID
                           select q).Take(1).SingleOrDefault();

                  if (expd2 != null && listExpD.Count > 0)
                  {
                      expd2.n_berat = field.berat;
                      expd2.n_koli = field.koli;
                      expd2.n_receh = field.receh;
                      expd2.n_vol = field.volume;
                  }

                  #endregion
              }
              else if ((field != null) && (!field.IsNew) && field.IsDelete)
              {
                #region Delete

                expd = listExpD.Find(delegate(LG_ExpD exp)
                {
                  return field.DO.Equals((string.IsNullOrEmpty(exp.c_dono) ? string.Empty : exp.c_dono.Trim()), StringComparison.OrdinalIgnoreCase);
                });


                expd2 = (from q in db.LG_ExpD2s
                         where q.c_nopart == field.partno && q.c_expno == expID 
                         select q).Take(1).SingleOrDefault();

                List<EKSP_KOLI> lstPart = new List<EKSP_KOLI>()
                {
                  new EKSP_KOLI()
                  {
                    c_part = field.partno,
                  }
                };

                if (!sKey.ContainsKey(field.partno))
                {
                  expd2 = (from q in db.LG_ExpD2s
                           where q.c_nopart == field.partno && q.c_expno == expID
                           select q).Take(1).SingleOrDefault();

                  db.LG_ExpD2s.DeleteOnSubmit(expd2);
                }

                if (expd2 != null && listExpD.Count > 0)
                {
                  if (structure.Fields.TipeExpedisi.Equals("01"))
                  {
                    doh = (from q in db.LG_DOHs
                           where q.c_dono == field.DO
                           select q).Take(1).SingleOrDefault();

                    if (doh != null)
                    {

                      if (listDOD1 != null)
                      {
                        listDOD1.Clear();
                      }

                      doh.l_status = false;

                      doh.l_auto = false;
                    }
                  }
                  else if (structure.Fields.TipeExpedisi.Equals("02"))
                  {
                    sjh = (from q in db.LG_SJHs
                           where q.c_sjno == field.DO
                           select q).Take(1).SingleOrDefault();

                    if (sjh != null)
                    {
                      sjh.l_exp = false;
                    }
                  }
                  else if (structure.Fields.TipeExpedisi.Equals("03"))
                  {
                      rsh = (from q in db.LG_RSHes
                             where q.c_rsno == field.DO
                             select q).Take(1).SingleOrDefault();

                      if (rsh != null)
                      {
                          rsh.l_exp = false;
                      }
                  }

                  listExp1.Add(new LG_ExpD1()
                  {
                    c_expno = expID,
                    c_dono = field.DO,
                    v_ket = field.Keterangan,
                    c_update = nipEntry,
                    d_update = date,
                    v_type = "03"
                  });

                  db.LG_ExpDs.DeleteOnSubmit(expd);

                  
                }

                #endregion
              }
            }

            if ((listExp1 != null) && (listExp1.Count > 0))
            {
              db.LG_ExpD1s.InsertAllOnSubmit(listExp1.ToArray());
              listExp1.Clear();
            }

            if ((listExpD2 != null) && (listExpD2.Count > 0))
            {
              db.LG_ExpD2s.InsertAllOnSubmit(listExpD2.ToArray());
              listExpD2.Clear();
            }

          }

          #endregion

          dic = new Dictionary<string, string>();
          dic.Add("EXP", expID);

          if (!string.IsNullOrEmpty(sTempDOcancel) || !string.IsNullOrEmpty(sTempDOErr))
          {
              dic.Add("DO_Salah", sTempDOcancel);
              dic.Add("DO_Err_WP", sTempDOErr);
          }

          result = string.Format("Total {0} detail(s)", totalDetails);

          hasAnyChanges = true;

          #endregion
        }
        if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete

          if (string.IsNullOrEmpty(expID))
          {
            result = "Nomor Ekspedisi dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          exph1 = (from q in db.LG_ExpHs
                  where q.c_ref == expID
                  select q).Take(1).SingleOrDefault();

          if (exph1 != null)
          {
              result = "Nomor Ekspedisi yang sudah direferensikan tidak dapat dihapus";

              rpe = ResponseParser.ResponseParserEnum.IsFailed;

              if (db.Transaction != null)
              {
                  db.Transaction.Rollback();
              }

              goto endLogic;
          }

          exph = (from q in db.LG_ExpHs
                  where q.c_expno == expID
                  select q).Take(1).SingleOrDefault();

          if (exph == null)
          {
            result = "Nomor Ekspedisi tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (exph.l_delete.HasValue && exph.l_delete.Value)
          {
              result = "Tidak dapat menghapus nomor Ekspedisi yang sudah terhapus.";

              rpe = ResponseParser.ResponseParserEnum.IsFailed;

              if (db.Transaction != null)
              {
                  db.Transaction.Rollback();
              }

              goto endLogic;
          }
          else if (Commons.IsClosingLogistik(db, exph.d_expdate))
          {
              result = "Expedisi tidak dapat dihapus, karena sudah closing.";

              rpe = ResponseParser.ResponseParserEnum.IsFailed;

              if (db.Transaction != null)
              {
                  db.Transaction.Rollback();
              }

              goto endLogic;
          }

          exph.c_update = nipEntry;
          exph.d_update = date;

          exph.l_delete = true;
          exph.v_ket_mark = structure.Fields.Ket;

          listExp1 = new List<LG_ExpD1>();
          listExpD = (from q in db.LG_ExpDs
                      where q.c_expno == expID
                      select q).ToList();

          if ((listExpD != null) && (listExpD.Count > 0))
          {
            if (exph.c_type.Equals("01"))
            {
              listDO = listExpD.GroupBy(x => x.c_dono).Select(y => y.Key).ToList();

              listResDOH = (from q in db.LG_DOHs
                            where listDO.Contains(q.c_dono)
                            select q).Distinct().ToList();


              for (nLoopC = 0; nLoopC < listExpD.Count; nLoopC++)
              {
                expd = listExpD[nLoopC];

                doID = (string.IsNullOrEmpty(expd.c_dono) ? string.Empty : expd.c_dono.Trim());
                doh = listResDOH.Find(delegate(LG_DOH doHdr)
                {
                  return doID.Equals((string.IsNullOrEmpty(doHdr.c_dono) ? string.Empty : doHdr.c_dono.Trim()));
                });

                if (doh != null)
                {
                  listDOD1 = (from q in db.LG_DOD1s
                              where (q.c_dono == doID)
                              select q).ToList();

                  for (nLoopC = 0; nLoopC < listDOD1.Count; nLoopC++)
                  {
                    dod1 = listDOD1[nLoopC];
                  }

                  listDOD1.Clear();

                  doh.l_status = false;
                  doh.l_auto = false;
                }

                listExp1.Add(new LG_ExpD1()
                {
                  c_expno = expID,
                  c_dono = doID,
                  v_ket = structure.Fields.Ket,
                  c_update = nipEntry,
                  d_update = date,
                  v_type = "03"
                });
              }
            }
            else if (exph.c_type.Equals("02"))
            {
              listSJ = listExpD.GroupBy(x => x.c_dono).Select(y => y.Key).ToList();

              //Production
              //listResSJH = (from q in db.LG_SJHs
              //              where listDO.Contains(q.c_sjno)
              //              select q).Distinct().ToList();


              // 
              listResSJH = (from q in db.LG_SJHs
                            where listDO.Contains(q.c_sjno)
                            select q).Distinct().ToList();

              for (nLoopC = 0; nLoopC < listExpD.Count; nLoopC++)
              {
                expd = listExpD[nLoopC];

                doID = (string.IsNullOrEmpty(expd.c_dono) ? string.Empty : expd.c_dono.Trim());

                // Production 
                //sjh = listResSJH.Find(delegate(LG_SJH doHdr)
                //{
                //  return doID.Equals((string.IsNullOrEmpty(doHdr.c_sjno) ? string.Empty : doHdr.c_sjno.Trim()));
                //});

                // Will Be Delete (A)
                sjh = listResSJH.Find(delegate(LG_SJH sjHdr)
                {
                    return doID.Equals((string.IsNullOrEmpty(sjHdr.c_sjno) ? string.Empty : sjHdr.c_sjno.Trim()));
                });
                //(A)

                // Production
                //if (sjh != null)

                // Will Be Delete (A)
                if (sjh != null)
                {
                  sjh.l_exp = false;
                }
                //(A)

                listExp1.Add(new LG_ExpD1()
                {
                  c_expno = expID,
                  c_dono = doID,
                  v_ket = structure.Fields.Ket,
                  c_update = nipEntry,
                  d_update = date,
                  v_type = "03"
                });
              }
            }
            else if (exph.c_type.Equals("03"))
            {
                listRS = listExpD.GroupBy(x => x.c_dono).Select(y => y.Key).ToList();

                listResRSH = (from q in db.LG_RSHes
                              where listRS.Contains(q.c_rsno)
                              select q).Distinct().ToList();

                for (nLoopC = 0; nLoopC < listExpD.Count; nLoopC++)
                {
                    expd = listExpD[nLoopC];

                    doID = (string.IsNullOrEmpty(expd.c_dono) ? string.Empty : expd.c_dono.Trim());

                    rsh = listResRSH.Find(delegate(LG_RSH rsHdr)
                    {
                        return doID.Equals((string.IsNullOrEmpty(rsHdr.c_rsno) ? string.Empty : rsHdr.c_rsno.Trim()));
                    });

                    if (rsh != null)
                    {
                        rsh.l_exp = false;
                    }

                    listExp1.Add(new LG_ExpD1()
                    {
                        c_expno = expID,
                        c_dono = doID,
                        v_ket = structure.Fields.Ket,
                        c_update = nipEntry,
                        d_update = date,
                        v_type = "03"
                    });
                }
            }

            db.LG_ExpDs.DeleteAllOnSubmit(listExpD);

            if ((listResDOH != null))
            {
              listResDOH.Clear();
            }
            if ((listResSJH != null))
            {
                listResSJH.Clear();
            } 
            if ((listResRSH != null))
            {
                listResRSH.Clear();
            }
            listExpD.Clear();
          }

          if (listExp1.Count > 0)
          {
            db.LG_ExpD1s.InsertAllOnSubmit(listExp1.ToArray());
            listExp1.Clear();
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

        result = string.Format("ScmsSoaLibrary.Bussiness.Penjualan:EkspedisiDO - {0}", ex.Message);

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

    public string EkspedisiCabang(ScmsSoaLibrary.Parser.Class.ExpedisiCabangStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false;

      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      LG_ExpCabH exph = null;
      List<LG_ExpCabH> ListExpH = null;

      ScmsSoaLibrary.Parser.Class.ExpedisiCabangStructureFields field = null;
      ScmsSoaLibrary.Parser.Class.ExpedisiCabangProcessStructureField pField = null;
      //ScmsSoaLibrary.Parser.Class.ExpedisiCabangStructureExtraField fieldEx = null;

      string nipEntry = null;
      string expID = null;
      //string tmpNumbering = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      int nLoop = 0;

      IDictionary<string, string> dic = null;

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        //goto endLogic;
      }
      //int totalDetails = 0;

      expID = (structure.Fields.ID ?? string.Empty);

      field = structure.Fields;

      //DateTime day = new DateTime(), time = DateTime.Now, res = new DateTime();
      //day = Convert.ToDateTime(field.Day);
      //TimeSpan ts = Convert.ToDateTime(field.Time).TimeOfDay;
      //res = Convert.ToDateTime(field.DateResi);

      //string sTime = time.ToString("hh:mm:ss");
      //string sDay = day.ToString("dd-MM-yyyy");
      //string sRTime = res.ToString("hh:mm:ss");
      //string sRDay = res.ToString("dd-mm-yyyy");

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          expID = Commons.GenerateNumbering<LG_ExpCabH>(db, "EC", '3', "37", date, "c_noexpcab");

          exph = new LG_ExpCabH()
          {
            c_entry = field.Entry,
            c_expno = field.ExpId,
            c_update = field.Entry,
            c_noexpcab = expID,
            d_cabang = field.DayXml.ToString("yyyy-MM-dd"),
            d_entry = date,
            t_cabang = field.TimeXml.ToString("HH:mm:ss"),
            d_update = date,
            d_kirim = field.DateResiXml.Date,
            t_kirim = field.DateResiXml.TimeOfDay,
            l_status = true
          };

          dic = new Dictionary<string, string>();

          if (exph != null)
          {
            hasAnyChanges = true;
            dic.Add("ExpCabang", expID);
            dic.Add("ExpId", field.ExpId);
            db.LG_ExpCabHs.InsertOnSubmit(exph);
          }

          #endregion
        }
        else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify

          exph = (from q in db.LG_ExpCabHs
                  where q.c_noexpcab == field.ID
                  select q).SingleOrDefault();

          exph.c_update = field.Entry;
          exph.d_cabang = field.DayXml.ToString("yyyy-MM-dd");
          exph.t_cabang = field.TimeXml.ToString("HH:mm:ss");
          exph.d_update = date;

          hasAnyChanges = true;

          #endregion
        }
        else if (structure.Method.Equals("Process", StringComparison.OrdinalIgnoreCase))
        {
          #region Process

          ListExpH = new List<LG_ExpCabH>();

          System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("id-ID");

          for (nLoop = 0; nLoop < structure.ExtraFields.ProcessField.Length; nLoop++)
          {
            pField = structure.ExtraFields.ProcessField[nLoop];

            expID = Commons.GenerateNumbering<LG_ExpCabH>(db, "EC", '3', "37", date, "c_noexpcab");
           
            exph = new LG_ExpCabH()
            {
              c_entry = structure.Fields.Entry,
              c_expno = pField.ExpId,
              c_update = field.Entry,
              c_noexpcab = expID,
              d_cabang = pField.DayXml.ToString("yyyy-MM-dd"),
              d_entry = date,
              t_cabang = pField.Time,
              d_update = date,
              d_kirim = pField.DateResiXml,
              t_kirim = pField.DateResiXml.TimeOfDay,
              l_status = true
            };

            ListExpH.Add(exph);
          }

          if (ListExpH.Count > 0)
          {
            db.LG_ExpCabHs.InsertAllOnSubmit(ListExpH.ToArray());
            ListExpH.Clear();
          }

          hasAnyChanges = true;

          if (exph != null)
          {
            db.LG_ExpCabHs.InsertOnSubmit(exph);
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
        rpe = ResponseParser.ResponseParserEnum.IsError;

        result = string.Format("ScmsSoaLibrary.Bussiness.Penjualan:EkspedisiCabang - {0}", ex.Message);

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

    public void EkspedisiCabangAuto(ORMDataContext db, string nipEntry, ScmsSoaLibrary.Modules.CommonUploadedQuery.Temporary_ImportExp[] tempExpValues)
    {
      string result = null;

      int nLoop = 0;

      //bool hasChange = false;

      List<LG_ExpCabH> listexpcabH = new List<LG_ExpCabH>();

      string Entry = string.IsNullOrEmpty(nipEntry) ? string.Empty : nipEntry;

      ScmsSoaLibrary.Modules.CommonUploadedQuery.Temporary_ImportExp tiexp = null;

      string expcabID = null;

      DateTime date = DateTime.Now,
        dateF = Functionals.StandardSqlDateTime;

      TimeSpan ts = DateTime.Today.TimeOfDay;

      Dictionary<string, LG_ExpCabH> dicExpCabH = new Dictionary<string, LG_ExpCabH>(StringComparer.OrdinalIgnoreCase);

      try
      {
        for (nLoop = 0; tempExpValues.Length > nLoop; nLoop++)
        {
          tiexp = tempExpValues[nLoop];
          expcabID = Commons.GenerateNumbering<LG_ExpCabH>(db, "EC", '3', "37", date, "c_noexpcab");

          listexpcabH.Add(new LG_ExpCabH()
          {
            c_entry = Entry,
            c_expno = tiexp.expno,
            c_noexpcab = expcabID,
            c_update = Entry,
            d_cabang = tiexp.Dtglexpcab.ToString("dd/MM/yyyy"),
            d_entry = date,
            d_kirim = tiexp.Dtglresi,
            d_update = date,
            l_status = false,
            t_cabang = tiexp.Twktexpcab.ToString(),
            t_kirim = ts
          });
        }

        //hasChange = true;

        if (listexpcabH.Count > 0)
        {
          db.LG_ExpCabHs.InsertAllOnSubmit(listexpcabH.ToArray());

          listexpcabH.Clear();
        }

        db.SubmitChanges();
      }
      catch (Exception ex)
      {
        result = string.Format("ScmsSoaLibrary.Bussiness.Faktur:EkspedisiCabangAuto - {0}", ex.Message);

        Logger.WriteLine(result, true);
        Logger.WriteLine(ex.StackTrace);
      }
    }

    public string InvoiceEkspedisiEksternal(ScmsSoaLibrary.Parser.Class.InvoiceEkspedisiEksternalStructure structure)
    {
        if ((structure == null) || (structure.Fields == null))
        {
            return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
        }

        string result = null;

        bool hasAnyChanges = false;

        ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

        IDictionary<string, string> dic = null;

        LG_IEH ieh = null;

        LG_BED1 bed1 = null;

        LG_IED1 ied1 = null;

        LG_IED2 ied2 = null;

        LG_IED3 ied3 = null;

        LG_ExpH exph = null;

        List<LG_IED1> listIED1 = null;

        List<LG_IED2> listIED2 = null;

        List<LG_IED3> listIED3 = null;

        int nLoop = 0,
        nLoopC = 0;

        DateTime date = DateTime.Now;

        ScmsSoaLibrary.Parser.Class.InvoiceEkspedisiEksternalStructureField field = null;
        ScmsSoaLibrary.Parser.Class.InvoiceEkspedisiEksternalClaimStructureField fieldClaim = null;

        ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

        string nipEntry = null,
            fakturID = null;

        char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) ? char.MinValue : structure.Fields.Gudang[0]);

        fakturID = (structure.Fields.FakturID ?? string.Empty);

        nipEntry = structure.Fields.Entry;

        if (string.IsNullOrEmpty(nipEntry))
        {
            result = "Nip penanggung jawab dibutuhkan.";

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

                #region Standard Validation Header

                if (gudang == char.MinValue)
                {
                    result = "Gudang tidak terbaca.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (string.IsNullOrEmpty(structure.Fields.Ekspedisi))
                {
                    result = "Kode ekspedisi tidak terbaca.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (!string.IsNullOrEmpty(fakturID))
                {
                    result = "Nomor Faktur ID harus kosong.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                #endregion

                
                fakturID = Commons.GenerateNumbering<LG_IEH>(db, "IE", '3', "39", date, "c_ieno");



                ieh = new LG_IEH()
                {
                    c_gdg = gudang,
                    c_exp = structure.Fields.Ekspedisi,
                    c_ieno = fakturID,
                    c_ie = structure.Fields.Faktur,
                    d_iedate = structure.Fields.TanggalFakturDate,
                    n_bilva_faktur = structure.Fields.FisikFaktur,
                    n_top = structure.Fields.TOP,
                    v_ket = structure.Fields.Ket,
                    n_materai = structure.Fields.Materai,
                    n_bruto = structure.Fields.Gross,
                    n_disc = structure.Fields.totalPotongan,
                    n_tax = structure.Fields.Pajak,
                    n_totaltax = structure.Fields.TotalTax,
                    n_totalbiayalain = structure.Fields.TotalBiayaLain,
                    n_net = structure.Fields.NetBerat,
                    n_netvol = structure.Fields.NetVol,
                    c_entry = structure.Fields.Entry,
                    d_entry = date,
                    c_update = structure.Fields.Entry,
                    d_update = date,
                    n_netsisa = structure.Fields.NetVol
                };

                db.LG_IEHs.InsertOnSubmit(ieh);

                #region Insert Detail

                if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                {
                    listIED1 = new List<LG_IED1>();

                    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    {
                        field = structure.Fields.Field[nLoop];

                        listIED1.Add(new LG_IED1()
                        {
                            c_gdg = gudang,
                            c_ieno = fakturID,
                            c_resi = field.resiNo,
                            c_expno = field.epNo,
                            c_cusno = field.Cusno,
                            n_koli = field.Koli,
                            n_berat = field.Berat,
                            n_vol = field.Volume,
                            n_tonase = field.Tonase,
                            n_biaya = field.Biaya,
                            n_expmin = field.expMin,
                            c_via = field.Via,
                            c_exptype = field.expType,
                            n_biayalain = field.biayaLain,
                            n_totalbiaya = field.totalCost,
                            i_urut = field.urut
                        });

                        exph = (from q in db.LG_ExpHs
                                where q.c_expno == field.epNo
                                select q).Distinct().Take(1).SingleOrDefault();

                        if (exph != null)
                        {
                            exph.l_ie = true;
                        }
                        else
                        {
                            result = "Nomor EP " + field.epNo + " tidak ditemukan";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                    }
                    if ((listIED1 != null) && (listIED1.Count > 0))
                    {
                        db.LG_IED1s.InsertAllOnSubmit(listIED1.ToArray());

                        listIED1.Clear();
                    }
                }

                hasAnyChanges = true;

                dic = new Dictionary<string, string>();

                dic.Add("ieno", fakturID);
                dic.Add("FakturDate", structure.Fields.TanggalFakturDate.ToString("yyyyMMdd"));
                dic.Add("FisikFaktur", structure.Fields.FisikFaktur.ToString());
                dic.Add("NetVol", structure.Fields.NetVol.ToString());

                #endregion

                #region Insert Detail Claim

                if ((structure.ExtraFields != null) && (structure.ExtraFields.FieldClaim != null) && (structure.ExtraFields.FieldClaim.Length > 0))
                {
                    listIED3 = new List<LG_IED3>();

                    for (nLoop = 0; nLoop < structure.ExtraFields.FieldClaim.Length; nLoop++)
                    {
                        fieldClaim = structure.ExtraFields.FieldClaim[nLoop];

                        listIED3.Add(new LG_IED3()
                        {
                            c_ieno = fakturID,
                            c_claimno = fieldClaim.claimNo,
                            n_disc = fieldClaim.Potongan
                        });
                    }

                    if ((listIED3 != null) && (listIED3.Count > 0))
                    {
                        db.LG_IED3s.InsertAllOnSubmit(listIED3.ToArray());

                        listIED3.Clear();
                    }
                }

                hasAnyChanges = true;
                #endregion

                #endregion
            }
            else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
            {
                #region Modify

                if (string.IsNullOrEmpty(fakturID))
                {
                    result = "Nomor faktur dibutuhkan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                ieh = (from q in db.LG_IEHs
                       where q.c_ieno == fakturID
                       select q).Take(1).SingleOrDefault();

                if (ieh == null)
                {
                    result = "Nomor faktur tidak ditemukan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (ieh.l_delete.HasValue && ieh.l_delete.Value)
                {
                    result = "Tidak dapat mengubah nomor faktur yang sudah terhapus.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (string.IsNullOrEmpty(structure.Fields.Faktur))
                {
                    result = "Nomor faktur dibutuhkan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                bed1 = (from q in db.LG_BED1s
                       where q.c_ieno == fakturID
                       select q).Take(1).SingleOrDefault();

                if (bed1 != null)
                {
                    result = "Invoice sudah jadi faktur";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                ieh.c_ie = structure.Fields.Faktur;
                ieh.d_iedate = structure.Fields.TanggalFakturDate;
                ieh.n_bilva_faktur = structure.Fields.FisikFaktur;
                ieh.n_top = structure.Fields.TOP;
                ieh.v_ket = structure.Fields.Ket;

                ieh.n_materai = structure.Fields.Materai;
                ieh.n_disc = structure.Fields.totalPotongan;
                if (!string.IsNullOrEmpty(structure.Fields.Ket))
                {
                    ieh.v_ket = structure.Fields.Ket;
                }
                ieh.c_update = nipEntry;
                ieh.d_update = date;
                ieh.n_tax = structure.Fields.Pajak;
                ieh.n_totaltax = structure.Fields.TotalTax;
                ieh.n_bruto = structure.Fields.Gross;
                ieh.n_totalbiayalain = structure.Fields.TotalBiayaLain;
                ieh.n_net = structure.Fields.NetBerat;
                ieh.n_netvol = structure.Fields.NetVol;
                ieh.n_netsisa = structure.Fields.NetVol;
                

                #region Populate Detail

                if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                {

                    listIED1 = (from q in db.LG_IED1s
                                where q.c_ieno == fakturID
                                select q).Distinct().ToList();

                    listIED2 = new List<LG_IED2>();

                    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    {
                        field = structure.Fields.Field[nLoop];

                        if ((!field.IsNew) && field.IsModified && (!field.IsDelete))
                        {
                            #region Modify

                            listIED2.Add(new LG_IED2()
                            {
                                c_ieno = fakturID,
                                c_resi = field.resiNo,
                                c_expno = field.epNo,
                                n_koli = field.Koli,
                                n_berat = field.Berat,
                                n_vol = field.Volume,
                                //c_typebiaya = field.tipeBiaya,
                                n_biayalain = field.biayaLain,
                                n_totalbiaya = field.totalCost,
                                c_entry = nipEntry,
                                d_entry = date,
                                v_ket_del = "tes"
                            });

                            ied1 = (from q in db.LG_IED1s
                                    where q.c_ieno == fakturID && q.c_resi == field.resiNo && q.c_expno == field.epNo
                                    select q).Distinct().Take(1).SingleOrDefault();

                            if (ied1 == null)
                            {
                                continue;
                            }
                            ied1.n_vol = field.Volume;
                            //ied1.n_tonase = field.Tonase;
                            ied1.n_biayalain = field.biayaLain;
                            ied1.n_totalbiaya = field.totalCost;
                            ied1.n_tonase = field.Tonase;
                            #endregion
                        }
                        else if ((!field.IsNew) && (!field.IsModified) && field.IsDelete)
                        {
                            #region Delete

                            ied1 = listIED1.Find(delegate(LG_IED1 isd)
                            {
                                return field.epNo.Trim().Equals((string.IsNullOrEmpty(isd.c_expno) ? string.Empty : isd.c_expno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                  field.resiNo.Trim().Equals((string.IsNullOrEmpty(isd.c_resi) ? string.Empty : isd.c_resi.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                  fakturID.Equals((string.IsNullOrEmpty(isd.c_ieno) ? string.Empty : isd.c_ieno.Trim()), StringComparison.OrdinalIgnoreCase);
                            });

                            ied2 = new LG_IED2()
                            {
                                c_ieno = fakturID,
                                c_resi = field.resiNo,
                                c_expno = field.epNo,
                                n_koli = field.Koli,
                                n_berat = field.Berat,
                                n_vol = field.Volume,
                                n_biayalain = field.biayaLain,
                                n_totalbiaya = field.totalCost,
                                c_entry = nipEntry,
                                d_entry = date,
                                v_ket_del = field.KeteranganMod
                            };
                                                        
                            exph = (from q in db.LG_ExpHs
                                    where q.c_expno == field.epNo
                                    select q).Distinct().Take(1).SingleOrDefault();

                            if (exph != null)
                            {
                                exph.l_ie = false;
                            }
                            
                            db.LG_IED2s.InsertOnSubmit(ied2);

                            db.LG_IED1s.DeleteOnSubmit(ied1);

                            listIED1.Remove(ied1);

                            #endregion
                        }
                        else if (field.IsNew && (!field.IsModified) && (!field.IsDelete))
                        {
                            #region Add

                            ied1 = listIED1.Find(delegate(LG_IED1 isd)
                            {
                                return field.resiNo.Equals(isd.c_resi, StringComparison.OrdinalIgnoreCase) &&
                                  field.epNo.Equals(isd.c_expno, StringComparison.OrdinalIgnoreCase);
                            });

                            if (ied1 == null)
                            {
                                ied1 = new LG_IED1()
                                {
                                    c_gdg = gudang,
                                    c_ieno = fakturID,
                                    c_resi = field.resiNo,
                                    c_expno = field.epNo,
                                    c_cusno = field.Cusno,
                                    n_koli = field.Koli,
                                    n_berat = field.Berat,
                                    n_vol = field.Volume,
                                    //n_tonase = field.Tonase,
                                    n_biaya = field.Biaya,
                                    n_expmin = field.expMin,
                                    c_via = field.Via,
                                    c_exptype = field.expType,
                                    n_biayalain = field.biayaLain,
                                    n_totalbiaya = field.totalCost,
                                    i_urut = field.urut,
                                    n_tonase = field.Tonase // berat yang ditagih
                                };

                                listIED1.Add(ied1);

                                db.LG_IED1s.InsertOnSubmit(ied1);
                            }

                            if (ied1 == null)
                            {
                                continue;
                            }
                            ied1.n_vol = field.Volume;
                            ied1.n_biayalain = field.biayaLain;

                            exph = (from q in db.LG_ExpHs
                                    where q.c_expno == field.epNo
                                    select q).Distinct().Take(1).SingleOrDefault();

                            if (exph != null)
                            {
                                exph.l_ie = true;
                            }
                            else
                            {
                                result = "Nomor EP " + field.epNo + " tidak ditemukan";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }

                                goto endLogic;
                            }

                            #endregion
                        }
                        //}
                    }
                }

                #endregion

                #region Populate Detail Claim

                if ((structure.ExtraFields != null) && (structure.ExtraFields.FieldClaim != null) && (structure.ExtraFields.FieldClaim.Length > 0))
                {

                    listIED3 = (from q in db.LG_IED3s
                                where q.c_ieno == fakturID
                                select q).Distinct().ToList();

                    for (nLoop = 0; nLoop < structure.ExtraFields.FieldClaim.Length; nLoop++)
                    {
                        fieldClaim = structure.ExtraFields.FieldClaim[nLoop];

                        if ((!fieldClaim.IsNew) && fieldClaim.IsModified && (!fieldClaim.IsDelete))
                        {
                            #region Modify

                            ied3 = (from q in db.LG_IED3s
                                    where q.c_ieno == fakturID && q.c_claimno == fieldClaim.claimNo
                                    select q).Distinct().Take(1).SingleOrDefault();

                            ied3.n_disc = fieldClaim.Potongan;
                            #endregion
                        }
                        else if ((!fieldClaim.IsNew) && (!fieldClaim.IsModified) && fieldClaim.IsDelete)
                        {
                            #region Delete

                            ied3 = listIED3.Find(delegate(LG_IED3 iedel3)
                            {
                                return fakturID.Equals((string.IsNullOrEmpty(iedel3.c_ieno) ? string.Empty : iedel3.c_ieno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                    fieldClaim.claimNo.Trim().Equals((string.IsNullOrEmpty(iedel3.c_claimno) ? string.Empty : iedel3.c_claimno.Trim()), StringComparison.OrdinalIgnoreCase);
                            });

                            db.LG_IED3s.DeleteOnSubmit(ied3);

                            listIED3.Remove(ied3);

                            #endregion
                        }
                        else if (fieldClaim.IsNew && (!fieldClaim.IsModified) && (!fieldClaim.IsDelete))
                        {
                            #region Add

                            ied3 = listIED3.Find(delegate(LG_IED3 iedel3)
                            {
                                return fakturID.Equals((string.IsNullOrEmpty(iedel3.c_ieno) ? string.Empty : iedel3.c_ieno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                    fieldClaim.claimNo.Trim().Equals((string.IsNullOrEmpty(iedel3.c_claimno) ? string.Empty : iedel3.c_claimno.Trim()), StringComparison.OrdinalIgnoreCase);
                            });

                            if (ied3 == null)
                            {
                                ied3 = new LG_IED3()
                                {
                                   c_ieno = fakturID,
                                   c_claimno = fieldClaim.claimNo,
                                   n_disc = fieldClaim.Potongan
                                };

                                listIED3.Add(ied3);

                                db.LG_IED3s.InsertOnSubmit(ied3);
                            }                        

                            #endregion
                        }
                        //}
                    }
                }

                #endregion



                hasAnyChanges = true;
                dic = new Dictionary<string, string>();

                dic.Add("ieno", fakturID);
                dic.Add("FakturDate", structure.Fields.TanggalFakturDate.ToString("yyyyMMdd"));
                dic.Add("FisikFaktur", structure.Fields.FisikFaktur.ToString());
                dic.Add("NetVol", structure.Fields.NetVol.ToString());


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

            result = string.Format("ScmsSoaLibrary.Bussiness.Penjualan:InvoiceEkspedisiEksternal - {0}", ex.Message);

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

    public string InvoiceEkspedisiInternal(ScmsSoaLibrary.Parser.Class.InvoiceEkspedisiInternalStructure structure)
    {
        if ((structure == null) || (structure.Fields == null))
        {
            return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
        }

        string result = null;

        bool hasAnyChanges = false;

        ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

        IDictionary<string, string> dic = null;

        LG_IEH ieh = null;

        LG_IED1 ied1 = null;

        LG_IED2 ied2 = null;

        LG_IED4 ied4 = null;

        List<LG_IED1> listIED1 = null;

        List<LG_IED2> listIED2 = null;

        List<LG_IED4> listIED4 = null;

        LG_ExpH exph = null;

        LG_ExpH exph2 = null;


        int nLoop = 0,
        nLoopC = 0;

        DateTime date = DateTime.Now;

        ScmsSoaLibrary.Parser.Class.InvoiceEkspedisiInternalStructureField field = null;
        ScmsSoaLibrary.Parser.Class.InvoiceEkspedisiInternalTolStructureField fieldTol = null;

        ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

        string nipEntry = null,
            fakturID = null;

        char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) ? char.MinValue : structure.Fields.Gudang[0]);

        fakturID = (structure.Fields.FakturID ?? string.Empty);

        nipEntry = structure.Fields.Entry;

        if (string.IsNullOrEmpty(nipEntry))
        {
            result = "Nip penanggung jawab dibutuhkan.";

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

                #region Standard Validation Header

                if (gudang == char.MinValue)
                {
                    result = "Gudang tidak terbaca.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (!string.IsNullOrEmpty(fakturID))
                {
                    result = "Nomor Faktur ID harus kosong.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                #endregion


                fakturID = Commons.GenerateNumbering<LG_IEH>(db, "IE", '3', "39", date, "c_ieno");



                ieh = new LG_IEH()
                {
                    c_gdg = gudang,
                    c_exp = "00",
                    c_ieno = fakturID,
                    c_ie = fakturID,
                    d_iedate = structure.Fields.TanggalFakturDate,
                    n_bilva_faktur = structure.Fields.FisikFaktur,
                    n_top = 0,
                    v_ket = structure.Fields.Ket,
                    n_awalkm = structure.Fields.awalKM,
                    n_akhirkm = structure.Fields.akhirKM,
                    n_materai = 0,
                    n_bruto = 0,
                    n_disc = 0,
                    //c_claimno = null,
                    n_tax = 0,
                    n_totaltax = 0,
                    n_totalbiayalain = structure.Fields.TotalBiayaLain,
                    n_net = structure.Fields.Net,
                    n_netvol = structure.Fields.Net,
                    c_entry = structure.Fields.Entry,
                    d_entry = date,
                    c_update = structure.Fields.Entry,
                    d_update = date,
                    n_bbmliter = structure.Fields.BBMLiter,
                    n_bbmprice = structure.Fields.BiayaBBM,
                    n_tol = structure.Fields.BiayaTol,
                    //Indra 20170426
                    //l_solar = structure.Fields.BBMType
                    l_solar = null,
                    c_type = structure.Fields.TipeBBM 
                    

                };

                db.LG_IEHs.InsertOnSubmit(ieh);

                #region Insert Detail

                if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                {
                    listIED1 = new List<LG_IED1>();

                    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    {
                        field = structure.Fields.Field[nLoop];

                        listIED1.Add(new LG_IED1()
                        {
                            c_gdg = gudang,
                            c_ieno = fakturID,
                            c_resi = field.resiNo,
                            c_expno = field.epNo,
                            c_cusno = field.Cusno,
                            n_koli = field.Koli,
                            n_berat = field.Berat,
                            n_vol = field.Volume,
                            n_biaya = 0,
                            c_via = field.Via,
                            c_exptype = "01",
                            n_biayalain = field.biayaLain,
                            n_totalbiaya = 0,
                            i_urut = field.urut
                        });
                    }
                    if ((listIED1 != null) && (listIED1.Count > 0))
                    {
                        db.LG_IED1s.InsertAllOnSubmit(listIED1.ToArray());

                        listIED1.Clear();
                    }

                    exph = (from q in db.LG_ExpHs
                            where q.c_expno == field.epNo
                            select q).Distinct().Take(1).SingleOrDefault();

                    if (exph != null)
                    {
                        exph.l_ie = true;
                    }

                    //if (!string.IsNullOrEmpty(exph.c_ref) || string.IsNullOrEmpty(exph.c_by))
                    //{
                    //    exph2 = (from q in db.LG_ExpHs
                    //            where q.c_expno == exph.c_ref
                    //            select q).Distinct().Take(1).SingleOrDefault();

                    //    exph2.l_ie = true;
                    //}
                }

                hasAnyChanges = true;

                dic = new Dictionary<string, string>();

                dic.Add("ieno", fakturID);
                dic.Add("FakturDate", date.ToString("yyyyMMdd"));
                dic.Add("FisikFaktur", fakturID);
                dic.Add("Net", structure.Fields.Net.ToString());

                #endregion

                #region Insert Detail Tol

                if ((structure.ExtraFields != null) && (structure.ExtraFields.FieldTol != null) && (structure.ExtraFields.FieldTol.Length > 0))
                {
                    listIED4 = new List<LG_IED4>();

                    for (nLoop = 0; nLoop < structure.ExtraFields.FieldTol.Length; nLoop++)
                    {
                        fieldTol = structure.ExtraFields.FieldTol[nLoop];

                        listIED4.Add(new LG_IED4()
                        {
                            c_ieno = fakturID,
                            n_detailtol = fieldTol.detailtol
                        });
                    }

                    if ((listIED4 != null) && (listIED4.Count > 0))
                    {
                        db.LG_IED4s.InsertAllOnSubmit(listIED4.ToArray());

                        listIED4.Clear();
                    }
                }

                hasAnyChanges = true;
                #endregion

                #endregion
            }
            else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
            {
                #region Modify

                if (string.IsNullOrEmpty(fakturID))
                {
                    result = "Nomor faktur dibutuhkan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                ieh = (from q in db.LG_IEHs
                       where q.c_ieno == fakturID
                       select q).Take(1).SingleOrDefault();

                if (ieh == null)
                {
                    result = "Nomor faktur tidak ditemukan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (ieh.l_delete.HasValue && ieh.l_delete.Value)
                {
                    result = "Tidak dapat mengubah nomor faktur yang sudah terhapus.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (string.IsNullOrEmpty(structure.Fields.Faktur))
                {
                    result = "Nomor faktur dibutuhkan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                //ieh.c_ie = structure.Fields.Faktur;
                ieh.d_iedate = structure.Fields.TanggalFakturDate;
                ieh.n_bilva_faktur = structure.Fields.FisikFaktur;
                //ieh.n_top = structure.Fields.TOP;
                ieh.v_ket = structure.Fields.Ket;

                ieh.n_awalkm = structure.Fields.awalKM;
                ieh.n_akhirkm = structure.Fields.akhirKM;
                ieh.n_materai = 0;
                //ieh.n_disc = structure.Fields.Potongan;
                //ieh.c_claimno = structure.Fields.ClaimNo;
                if (!string.IsNullOrEmpty(structure.Fields.Ket))
                {
                    ieh.v_ket = structure.Fields.Ket;
                }
                ieh.c_update = nipEntry;
                ieh.d_update = date;
                //ieh.n_tax = structure.Fields.Pajak;
                //ieh.n_totaltax = structure.Fields.TotalTax;
                //ieh.n_bruto = structure.Fields.Gross;
                ieh.n_totalbiayalain = structure.Fields.TotalBiayaLain;
                ieh.n_net = structure.Fields.Net;
                ieh.n_netvol = structure.Fields.Net;

                ieh.n_bbmliter = structure.Fields.BBMLiter;
                ieh.n_bbmprice = structure.Fields.BiayaBBM;
                ieh.n_tol = structure.Fields.BiayaTol;
                //Indra 20170426
                //ieh.l_solar = structure.Fields.BBMType;
                ieh.l_solar = null;
                ieh.c_type = structure.Fields.TipeBBM;


                #region Populate Detail

                if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                {

                    listIED1 = (from q in db.LG_IED1s
                                where q.c_ieno == fakturID
                                select q).Distinct().ToList();

                    listIED2 = new List<LG_IED2>();

                    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    {
                        field = structure.Fields.Field[nLoop];

                        if ((!field.IsNew) && field.IsModified && (!field.IsDelete))
                        {
                            #region Modify

                            listIED2.Add(new LG_IED2()
                            {
                                c_ieno = fakturID,
                                c_resi = field.resiNo,
                                c_expno = field.epNo,
                                n_koli = field.Koli,
                                n_berat = field.Berat,
                                n_vol = field.Volume,
                                //c_typebiaya = field.tipeBiaya,
                                n_biayalain = field.biayaLain,
                                n_totalbiaya = 0,
                                c_entry = nipEntry,
                                d_entry = date,
                                v_ket_del = "tes"
                            });

                            ied1 = (from q in db.LG_IED1s
                                    where q.c_ieno == fakturID && q.c_resi == field.resiNo && q.c_expno == field.epNo
                                    select q).Distinct().Take(1).SingleOrDefault();

                            if (ied1 == null)
                            {
                                continue;
                            }
                            ied1.n_biayalain = field.biayaLain;

                            exph = (from q in db.LG_ExpHs
                                    where q.c_resi == field.resiNo && q.c_expno == field.epNo
                                    select q).Distinct().Take(1).SingleOrDefault();

                            if (exph == null)
                            {
                                continue;
                            }
                            //exph.n_biayalain = field.biayaLain;




                            #endregion
                        }
                        else if ((!field.IsNew) && (!field.IsModified) && field.IsDelete)
                        {
                            #region Delete

                            ied1 = listIED1.Find(delegate(LG_IED1 isd)
                            {
                                return field.epNo.Trim().Equals((string.IsNullOrEmpty(isd.c_expno) ? string.Empty : isd.c_expno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                  field.resiNo.Trim().Equals((string.IsNullOrEmpty(isd.c_resi) ? string.Empty : isd.c_resi.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                  fakturID.Equals((string.IsNullOrEmpty(isd.c_resi) ? string.Empty : isd.c_ieno.Trim()), StringComparison.OrdinalIgnoreCase);
                            });

                            ied2 = new LG_IED2()
                            {
                                c_ieno = fakturID,
                                c_resi = field.resiNo,
                                c_expno = field.epNo,
                                n_koli = field.Koli,
                                n_berat = field.Berat,
                                n_vol = field.Volume,
                                n_biayalain = field.biayaLain,
                                n_totalbiaya = 0,
                                c_entry = nipEntry,
                                d_entry = date,
                                v_ket_del = field.KeteranganMod
                            };


                            exph = (from q in db.LG_ExpHs
                                    where q.c_expno == field.epNo
                                    select q).Distinct().Take(1).SingleOrDefault();

                            if (exph != null)
                            {
                                exph.l_ie = false;
                            }

                            //if (exph.c_ref != null || exph.c_by == "")
                            //{
                            //    exph2 = (from q in db.LG_ExpHs
                            //            where q.c_expno == exph.c_ref
                            //            select q).Distinct().Take(1).SingleOrDefault();

                            //    exph2.l_ie = false;
                            //}

                            db.LG_IED2s.InsertOnSubmit(ied2);

                            db.LG_IED1s.DeleteOnSubmit(ied1);

                            listIED1.Remove(ied1);

                            #endregion
                        }
                        else if (field.IsNew && (!field.IsModified) && (!field.IsDelete))
                        {
                            #region Add

                            ied1 = listIED1.Find(delegate(LG_IED1 isd)
                            {
                                return field.resiNo.Equals(isd.c_resi, StringComparison.OrdinalIgnoreCase) &&
                                  field.epNo.Equals(isd.c_expno, StringComparison.OrdinalIgnoreCase);
                            });

                            if (ied1 == null)
                            {
                                ied1 = new LG_IED1()
                                {
                                    c_gdg = gudang,
                                    c_ieno = fakturID,
                                    c_resi = field.resiNo,
                                    c_expno = field.epNo,
                                    c_cusno = field.Cusno,
                                    n_koli = field.Koli,
                                    n_berat = field.Berat,
                                    n_vol = field.Volume,
                                    n_biaya = 0,
                                    c_via = field.Via,
                                    c_exptype = "01",
                                    n_biayalain = field.biayaLain,
                                    n_totalbiaya = 0,
                                    i_urut = field.urut
                                };

                                exph = (from q in db.LG_ExpHs
                                        where q.c_expno == field.epNo
                                        select q).Distinct().Take(1).SingleOrDefault();

                                if (exph != null)
                                {
                                    exph.l_ie = true;
                                }

                                //if (exph.c_ref != null || exph.c_by == "")
                                //{
                                //    exph = (from q in db.LG_ExpHs
                                //            where q.c_expno == exph.c_ref
                                //            select q).Distinct().Take(1).SingleOrDefault();

                                //    exph.l_ie = true;
                                //}

                                listIED1.Add(ied1);

                                db.LG_IED1s.InsertOnSubmit(ied1);
                            }

                            #endregion
                        }
                        //}
                    }
                }

                #endregion

                #region Populate Detail Tol

                if ((structure.ExtraFields != null) && (structure.ExtraFields.FieldTol != null) && (structure.ExtraFields.FieldTol.Length > 0))
                {
                    listIED4 = (from q in db.LG_IED4s
                                where q.c_ieno == fakturID
                                select q).Distinct().ToList();

                    for (nLoop = 0; nLoop < structure.ExtraFields.FieldTol.Length; nLoop++)
                    {
                        fieldTol = structure.ExtraFields.FieldTol[nLoop];

                        if ((!fieldTol.IsNew) && fieldTol.IsModified && (!fieldTol.IsDelete))
                        {
                            #region Modify

                            ied4 = (from q in db.LG_IED4s
                                    where q.c_ieno == fakturID && q.IDX == decimal.Parse(fieldTol.idx)
                                    select q).Distinct().Take(1).SingleOrDefault();

                            ied4.n_detailtol = fieldTol.detailtol;
                            #endregion
                        }
                        else if ((!fieldTol.IsNew) && (!fieldTol.IsModified) && fieldTol.IsDelete)
                        {
                            #region Delete

                            ied4 = listIED4.Find(delegate(LG_IED4 iedel4)
                            {
                                return fakturID.Equals((string.IsNullOrEmpty(iedel4.c_ieno) ? string.Empty : iedel4.c_ieno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                    fieldTol.idx.Trim().Equals((string.IsNullOrEmpty(iedel4.IDX.ToString()) ? string.Empty : iedel4.IDX.ToString().Trim()), StringComparison.OrdinalIgnoreCase);
                            });

                            db.LG_IED4s.DeleteOnSubmit(ied4);

                            listIED4.Remove(ied4);

                            #endregion
                        }
                        else if (fieldTol.IsNew && (!fieldTol.IsModified) && (!fieldTol.IsDelete))
                        {
                            #region Add

                            ied4 = new LG_IED4()
                            {
                                c_ieno = fakturID,
                                n_detailtol = fieldTol.detailtol
                            };

                            listIED4.Add(ied4);

                            db.LG_IED4s.InsertOnSubmit(ied4);

                            #endregion
                        }
                    }
                }

                #endregion

                hasAnyChanges = true;
                dic = new Dictionary<string, string>();

                dic.Add("ieno", fakturID);
                dic.Add("FakturDate", structure.Fields.TanggalFakturDate.ToString("yyyyMMdd"));
                dic.Add("FisikFaktur", fakturID);
                dic.Add("Net", structure.Fields.Net.ToString());


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

            result = string.Format("ScmsSoaLibrary.Bussiness.Penjualan:InvoiceEkspedisiInternal - {0}", ex.Message);

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

    public string FakturEkspedisi(ScmsSoaLibrary.Parser.Class.FakturEkspedisiStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;

            LG_BEH beh = null;
            LG_BED1 bed1 = null;
            LG_IEH ieh = null;
            LG_BED2 bed2 = null;

            List<LG_BED1> listBED1 = null;

            int nLoop = 0,
            nLoopC = 0;

            DateTime date = DateTime.Now;

            ScmsSoaLibrary.Parser.Class.FakturEkspedisiStructureField field = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            string nipEntry = null,
                fakturID = null;

            char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) ? char.MinValue : structure.Fields.Gudang[0]);

            int totalDetails = 0;

            fakturID = (structure.Fields.FakturID ?? string.Empty);

            nipEntry = structure.Fields.Entry;

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

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

                    #region Standard Validation Header

                    if (gudang == char.MinValue)
                    {
                        result = "Gudang tidak terbaca.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (string.IsNullOrEmpty(structure.Fields.Ekspedisi))
                    {
                        result = "Kode ekspedisi tidak terbaca.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (!string.IsNullOrEmpty(fakturID))
                    {
                        result = "Nomor Faktur ID harus kosong.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    #endregion

                    fakturID = Commons.GenerateNumbering<LG_BEH>(db, "BE", '3', "40", structure.Fields.TanggalFakturDate, "c_beno");

                    beh = new LG_BEH()
                    {
                        c_gdg = gudang,
                        c_exp = structure.Fields.Ekspedisi,
                        c_beno = fakturID,
                        d_bedate = structure.Fields.TanggalFakturDate,
                        n_bilvafaktur = structure.Fields.BilvaFaktur,
                        n_net = structure.Fields.Net,
                        n_claim = structure.Fields.Claim,
                        n_pinalty = structure.Fields.Pinalty,
                        n_selisihbe = structure.Fields.Selisih,
                        n_be = structure.Fields.TotalNet,
                        n_lain = structure.Fields.Lain2,
                        v_alasan = structure.Fields.Alasan,
                        v_ket = structure.Fields.Ket,
                        c_entry = structure.Fields.Entry,
                        d_entry = date,
                        c_update = structure.Fields.Entry,
                        d_update = date,
                        l_delete = false,
                        n_pph = structure.Fields.Pph
                    };

                    db.LG_BEHs.InsertOnSubmit(beh);

                    #region Insert Detail

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        listBED1 = new List<LG_BED1>();

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            ieh = (from q in db.LG_IEHs
                                   where q.c_ieno == field.InvNo
                                   && q.c_exp == structure.Fields.Ekspedisi
                                   && q.n_netsisa >= field.BilvaD
                                   select q).Take(1).SingleOrDefault();

                            if (ieh == null)
                            {
                                result = "Nominal Sisa Invoice kurang. Silahkan dicek invoice no." + field.InvNo;

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }

                                goto endLogic;
                            }
                            else
                            {
                                listBED1.Add(new LG_BED1()
                                {
                                    c_beno = fakturID,
                                    c_ieno = field.InvNo,
                                    c_resi = field.Resi,
                                    n_bilvad = field.BilvaD,
                                    n_bed = field.NetD,
                                    n_selisihbed = field.SelisihD
                                });

                                //ieh.n_netsisa -= field.NetD;
                                ieh.n_netsisa -= field.BilvaD;
                            }
                        }

                        if ((listBED1 != null) && (listBED1.Count > 0))
                        {
                            db.LG_BED1s.InsertAllOnSubmit(listBED1.ToArray());

                            listBED1.Clear();

                            totalDetails++;
                        }
                    }

                    hasAnyChanges = true;

                    dic = new Dictionary<string, string>();

                    if (totalDetails > 0)
                    {
                        dic.Add("Faktur", fakturID);
                        dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                        result = string.Format("Total {0} detail(s)", totalDetails);

                        hasAnyChanges = true;
                    }
                    else
                    {
                        hasAnyChanges = false;
                    }

                    #endregion

                    #endregion
                }
                else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    if (string.IsNullOrEmpty(fakturID))
                    {
                        result = "Nomor faktur dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    beh = (from q in db.LG_BEHs
                           where q.c_beno == fakturID
                           select q).Take(1).SingleOrDefault();

                    if (beh == null)
                    {
                        result = "Nomor faktur tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (beh.l_delete.HasValue && beh.l_delete.Value)
                    {
                        result = "Tidak dapat mengubah nomor faktur yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    beh.d_bedate = structure.Fields.TanggalFakturDate;
                    beh.n_bilvafaktur = structure.Fields.BilvaFaktur;
                    beh.n_net = structure.Fields.Net;
                    beh.n_selisihbe = structure.Fields.Selisih;
                    beh.n_claim = structure.Fields.Claim;
                    beh.n_pinalty = structure.Fields.Pinalty;
                    beh.n_be = structure.Fields.TotalNet;
                    beh.n_lain = structure.Fields.Lain2;
                    beh.v_alasan = structure.Fields.Alasan;
                    beh.n_pph = structure.Fields.Pph;

                    if (!string.IsNullOrEmpty(structure.Fields.Ket))
                    {
                        beh.v_ket = structure.Fields.Ket;
                    }

                    beh.c_update = nipEntry;
                    beh.d_update = date;


                    #region Populate Detail

                    listBED1 = new List<LG_BED1>();

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        listBED1 = (from q in db.LG_BED1s
                                    where q.c_beno == fakturID
                                    select q).Distinct().ToList();

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if ((!field.IsNew) && field.IsModified && (!field.IsDelete))
                            {
                                #region Modify


                                #endregion
                            }
                            else if ((!field.IsNew) && (!field.IsModified) && field.IsDelete)
                            {
                                #region Delete

                                bed1 = listBED1.Find(delegate(LG_BED1 bed)
                                {
                                    return field.InvNo.Trim().Equals((string.IsNullOrEmpty(bed.c_ieno) ? string.Empty : bed.c_ieno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                      field.Resi.Trim().Equals((string.IsNullOrEmpty(bed.c_resi) ? string.Empty : bed.c_resi.Trim()), StringComparison.OrdinalIgnoreCase);
                                });

                                if (bed1 != null)
                                {
                                    ieh = (from q in db.LG_IEHs
                                           where q.c_ieno == field.InvNo
                                           && q.c_exp == structure.Fields.Ekspedisi
                                           select q).Take(1).SingleOrDefault();

                                    if (ieh == null)
                                    {
                                        result = "Invoice tidak ada. Silahkan dicek invoice no." + field.InvNo;

                                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                        if (db.Transaction != null)
                                        {
                                            db.Transaction.Rollback();
                                        }

                                        goto endLogic;
                                    }
                                    else
                                    {
                                        ieh.n_netsisa += field.NetD;
                                    }

                                    bed2 = new LG_BED2()
                                    {
                                        c_beno = fakturID,
                                        c_ieno = field.InvNo,
                                        n_bilvad = field.BilvaD,
                                        n_bed = field.NetD,
                                        n_selisihbed = field.SelisihD,
                                        c_entry = nipEntry,
                                        d_entry = date,
                                        v_ket_del = field.KeteranganMod,
                                        v_type = "03"
                                    };

                                    db.LG_BED2s.InsertOnSubmit(bed2);

                                    db.LG_BED1s.DeleteOnSubmit(bed1);

                                    listBED1.Remove(bed1);
                                }
                                #endregion
                            }
                            else if (field.IsNew && (!field.IsModified) && (!field.IsDelete))
                            {
                                #region Add

                                bed1 = listBED1.Find(delegate(LG_BED1 bed)
                                {
                                    return field.InvNo.Trim().Equals((string.IsNullOrEmpty(bed.c_ieno) ? string.Empty : bed.c_ieno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                      field.Resi.Trim().Equals((string.IsNullOrEmpty(bed.c_resi) ? string.Empty : bed.c_resi.Trim()), StringComparison.OrdinalIgnoreCase);
                                });


                                if (bed1 == null)
                                {
                                    ieh = (from q in db.LG_IEHs
                                           where q.c_ieno == field.InvNo
                                           && q.c_exp == structure.Fields.Ekspedisi
                                           && q.n_netsisa >= field.NetD
                                           select q).Take(1).SingleOrDefault();

                                    if (ieh == null)
                                    {
                                        result = "Nominal Sisa Invoice kurang. Silahkan dicek invoice no." + field.InvNo;

                                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                        if (db.Transaction != null)
                                        {
                                            db.Transaction.Rollback();
                                        }

                                        goto endLogic;
                                    }
                                    else
                                    {
                                        listBED1.Add(new LG_BED1()
                                        {
                                            c_beno = fakturID,
                                            c_ieno = field.InvNo,
                                            c_resi = field.Resi,
                                            n_bilvad = field.BilvaD,
                                            n_bed = field.NetD,
                                            n_selisihbed = field.SelisihD
                                        });

                                        ieh.n_netsisa -= field.NetD;
                                    }

                                    bed1 = new LG_BED1()
                                    {
                                        c_beno = fakturID,
                                        c_ieno = field.InvNo,
                                        c_resi = field.Resi,
                                        n_bilvad = field.BilvaD,
                                        n_bed = field.NetD,
                                        n_selisihbed = field.SelisihD
                                    };

                                    listBED1.Add(bed1);

                                    db.LG_BED1s.InsertOnSubmit(bed1);
                                }

                                #endregion
                            }
                        }
                    }

                    #endregion

                    dic = new Dictionary<string, string>();
                    dic.Add("Faktur", fakturID);
                    dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                    result = string.Format("Total {0} detail(s)", totalDetails);
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

                result = string.Format("ScmsSoaLibrary.Bussiness.Penjualan:FakturEkspedisi - {0}", ex.Message);

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

    public string ReturnDO(ScmsSoaLibrary.Parser.Class.ReturnDOStructure structure)
    {
        if ((structure == null) || (structure.Fields == null))
        {
            return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
        }

        string result = null;

        bool hasAnyChanges = false;

        ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

        IDictionary<string, string> dic = null;

        LG_ReturnDO ReturnDO = null;
        LG_ReturnDO ReturnDODel = null;

        List<LG_ReturnDO> listReturnDO = null;

        List<LG_ExpD> listexpdcek = null;

        int nLoop = 0,
        nLoopC = 0;

        DateTime date = DateTime.Now;

        ScmsSoaLibrary.Parser.Class.ReturnDOStructureField field = null;

        ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

        string nipEntry = null,
            fakturID = null;

        char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) ? char.MinValue : structure.Fields.Gudang[0]);

        nipEntry = structure.Fields.Entry;

        if (string.IsNullOrEmpty(nipEntry))
        {
            result = "Nip penanggung jawab dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
        }

        try
        {
            db.Connection.Open();

            db.Transaction = db.Connection.BeginTransaction();

            if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
            {
                #region Add & Delete
                #region Populate Detail

                if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                {
                    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    {
                        field = structure.Fields.Field[nLoop];

                        if (field.IsNew && (!field.IsModified) && (!field.IsDelete))
                        {
                            #region Add

                            listReturnDO = (from q in db.LG_ReturnDOs
                                            where q.c_dono == field.dono
                                            select q).Distinct().ToList();

                            listexpdcek = (from q in db.LG_ExpDs
                                           where q.c_dono == field.dono
                                           select q).Distinct().ToList();

                            if (listexpdcek.Count != 0)
                            {
                                if (listReturnDO.Count == 0)
                                {
                                    listReturnDO.Add(new LG_ReturnDO()
                                    {
                                        c_gdg = gudang,
                                        c_dono = field.dono,
                                        d_terima = field.tglTerimaFormated,
                                        d_balik = field.tglBalikFormated,
                                        c_entry = nipEntry,
                                        d_entry = DateTime.Now
                                    });

                                    if ((listReturnDO != null) && (listReturnDO.Count > 0))
                                    {
                                        db.LG_ReturnDOs.InsertAllOnSubmit(listReturnDO.ToArray());

                                        listReturnDO.Clear();
                                    }
                                }
                                else
                                {
                                    result = "Nomor DO " + field.dono + "sudah ada";

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
                                result = "Nomor DO " + field.dono + "tidak ditemukan";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }

                                goto endLogic;
                            }

                            #endregion
                        }
                        else if ((!field.IsNew) && (!field.IsModified) && field.IsDelete)
                        {
                            #region Delete

                            listReturnDO = (from q in db.LG_ReturnDOs
                                            where q.c_dono == field.dono
                                            select q).Distinct().ToList();

                            ReturnDODel = listReturnDO.Find(delegate(LG_ReturnDO rdo)
                            {
                                return field.dono.Equals(rdo.c_dono, StringComparison.OrdinalIgnoreCase);
                            });

                            if (ReturnDODel != null)
                            {
                                db.LG_ReturnDOs.DeleteOnSubmit(ReturnDODel);
                                listReturnDO.Remove(ReturnDODel);
                            }

                            #endregion
                        }

                        if (listReturnDO != null)
                        {
                            listReturnDO.Clear();
                        }
                        if (listexpdcek != null)
                        {
                            listexpdcek.Clear();
                        }
                    }
                }

                #endregion

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

            result = string.Format("ScmsSoaLibrary.Bussiness.Penjualan:ReturnDO - {0}", ex.Message);

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