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
  class Auto
  {
      #region Class
    internal class PLCat
    {
      public string TypeCat { get; set; }
      public string TypeLat { get; set; }
      public string Item { get; set; }
      public string Ket { get; set; }
    }

    internal class PLDetailGenerator
    {
      public string c_spno { get; set; }
      public string c_sp { get; set; }
      public string c_iteno { get; set; }
      public string v_itemdesc { get; set; }
      public string v_undes { get; set; }
      public decimal n_sisa { get; set; }
      public string c_batch { get; set; }
      public bool isMaster { get; set; }
      public decimal n_box { get; set; }
    }

    internal class PLLat
    {
      public string TypeLat { get; set; }
      public string Item { get; set; }
      public string Ket { get; set; }
    }

    internal class PLVia
    {
      public string TypeVia { get; set; }
      public string Item { get; set; }
      public string Ket { get; set; }
    }

    internal class PLDiv
    {
        public string TypeDiv { get; set; }
        public string Item { get; set; }
        public string Ket { get; set; }
    }

    internal class PLClassItemComponen
    {
      public string SpID { get; set; }
      public string CatID { get; set; }
      public string ViaID { get; set; }
      public string LatID { get; set; }
      public string BatchID { get; set; }
      public string Item { get; set; }
      public decimal Qty { get; set; }
      public decimal nSisa { get; set; }
      public string nGropName { get; set; }
      public string Supplier { get; set; }
      public decimal Box { get; set; }
      public bool isBox { get; set; }
      public bool isED { get; set; }
      public bool isAccModify { get; set; }
      public string accket { get; set; }
      public string DivID { get; set; }
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

    internal class PLClassItem
    {
      public string RefID { get; set; }
      public string SignID { get; set; }
      public string BatchID { get; set; }
      public string Item { get; set; }
    }

    internal class PLClassItemSum
    {
      public string BatchID { get; set; }
      public string Item { get; set; }
      public decimal Qty { get; set; }
      public string groupName { get; set; }
      public decimal box { get; set; }
    }
      #endregion

    public string DOPrinsipal(ScmsSoaLibrary.Parser.Class.DOPrinsipalStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false;

      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      //LG_PLH plh = null;

      //LG_SPD1 spd1 = null;

      //LG_RND1 rnd1 = null;

      ScmsSoaLibrary.Parser.Class.DOPrinsipalStructureField field = null;
      string nipEntry = null;
      string doID = null,
        suplId = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      List<LG_DOPD> listDOPD = null;

      LG_DOPH doph = null;
      //LG_DOPD dopd = null;

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

      doID = (structure.Fields.DOPrinsipalID ?? string.Empty);
      suplId = (structure.Fields.Prinsipal ?? string.Empty);

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          if (string.IsNullOrEmpty(doID))
          {
            result = "Nomor Delivery dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(suplId))
          {
            result = "Nama pemasok dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          doph = new LG_DOPH()
          {
            c_nosup = suplId,
            c_dono = doID,
            d_dodate = structure.Fields.TanggalDODate,
            d_fjno = structure.Fields.Faktur,
            d_fjdate = structure.Fields.TanggalFakturDate,
            l_status = false,
            c_cab = structure.Fields.Customer,
            c_via = structure.Fields.Via,
            c_taxno = structure.Fields.Pajak,
            d_entry = date
          };

          db.LG_DOPHs.InsertOnSubmit(doph);

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listDOPD = new List<LG_DOPD>();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              listDOPD.Add(new LG_DOPD()
              {
                c_nosup = suplId,
                c_dono = doID,
                c_iteno = field.Item,
                c_type = field.TipeTransaksi,
                c_itenopri = field.ItemSuplier,
                v_itnam = null,
                v_undes = null,
                c_pono = field.NomorPO,
                n_qty = field.Quantity,
                n_qty_sisa = field.Quantity,
                c_batch = field.Batch,
                d_expired = field.BatchExpiredDate,
                n_disc = field.Disc,
                l_claim = field.IsClaim,
                l_pending = field.IsPending,
                l_done = false,
                d_entry = date
              });

              totalDetails++;
            }

            if (listDOPD.Count > 0)
            {
              db.LG_DOPDs.InsertAllOnSubmit(listDOPD.ToArray());
              listDOPD.Clear();
            }
          }

          if (totalDetails > 0)
          {
            hasAnyChanges = true;
          }

          #endregion
        }
        else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete
          
          if (string.IsNullOrEmpty(doID))
          {
            result = "Nomor Delivery dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(suplId))
          {
            result = "Nama pemasok dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          doph = (from q in db.LG_DOPHs
                  where (q.c_nosup == suplId) && (q.c_dono == doID)
                  select q).Take(1).SingleOrDefault();
          
          if (doph == null)
          {
            result = "Nomor DO Prinsipal tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          db.LG_DOPHs.DeleteOnSubmit(doph);

          listDOPD = (from q in db.LG_DOPDs
                      where (q.c_nosup == suplId) && (q.c_dono == doID)
                      select q).ToList();

          if ((listDOPD != null) && (listDOPD.Count > 0))
          {
            db.LG_DOPDs.DeleteAllOnSubmit(listDOPD.ToArray());
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

        result = string.Format("ScmsSoaLibrary.Bussiness.Auto:DOPrinsipal - {0}", ex.Message);

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

    public string PackingList(ScmsSoaLibrary.Parser.Class.PackingListStructure structure, bool isSync)
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
      string plID = null,
        refID = null,
        sItemAndBatch = null;

      List<string> lstBatch = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      //decimal? spQty = 0;
      decimal spQtyReloc = 0,
        rnQty = 0,
        spAlloc = 0,
        spSisa = 0,
        totalCurrentStock = 0,
        nQtyTemp = 0,
        nQtySisa = 0;

      PLClassComponent spac = null;

      List<PLClassComponent> listSPAC = null,
        listSPACTemp = null;

      List<LG_PLD1> listPld1 = null,
        listPld1_1 = null;
      List<LG_PLD2> listPld2 = null,
        listPld2Copy = null,
        listPld2_1 = null;
      List<LG_PLD3> listPld3 = null;

      List<string> listRN = null;
      //List<LG_RND1> listResRND1 = null;

      Dictionary<string, List<PLClassComponent>> dicItemStock = null;
      
      Dictionary<string, List<PLClassItem>> dicItem = null;


      char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '1' : structure.Fields.Gudang[0]);

      LG_PLD1 pld1 = null;
      LG_PLD2 pld2 = null;
      LG_PLD3 pld3 = null;

      #region New Stock Indra 20180305FM

      LG_DAILY_STOCK_v2 daily2 = null;
      LG_MOVEMENT_STOCK_v2 movement2 = null;

      #endregion

      int nLoop = 0,
        nLoopC = 0;

      IDictionary<string, string> dic = null;

      IDictionary<string, List<Functionals.PL1>> sdfdd = new Dictionary<string, List<Functionals.PL1>>();

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }

      int totalDetails = 0;
      //bool isConfirmed = false;

      plID = (structure.Fields.PackingListID ?? string.Empty);

      string sIDGab = null,
         sIDGabVia = null;

      isConfirmMode = structure.Method.Equals("ModifyConfirm", StringComparison.OrdinalIgnoreCase);

      try
      {
        //db.Connection.Open();

        //db.Transaction = db.Connection.BeginTransaction();

        PLClassItemComponen plComp = new PLClassItemComponen();
        List<PLClassItemComponen> lstplComp = new List<PLClassItemComponen>(),
          lstplComp1 = new List<PLClassItemComponen>(),
          lstplCompStatic = new List<PLClassItemComponen>() ;
        List<PLClassItem> lstItem = new List<PLClassItem>();
        dicItem = new Dictionary<string, List<PLClassItem>>(StringComparer.OrdinalIgnoreCase);


        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
        {
          field = structure.Fields.Field[nLoop];

          FA_MasItm masitm = (from q in db.FA_MasItms
                              where q.c_iteno == field.Item
                              select q).Take(1).SingleOrDefault();

          //PLCat itmCat = (from q in db.SCMS_MSITEM_CATs
          //                join q1 in db.MsTransDs on q.c_type equals q1.c_type
          //                where q.c_iteno == field.Item && q1.c_portal == '9' && q1.c_notrans == "001"
          //                select new PLCat
          //                {
          //                  Item = q.c_iteno,
          //                  TypeCat = q.c_type,
          //                  Ket = q1.v_ket,
          //                }).Take(1).SingleOrDefault();

          //PLLat itmLat = (from q in db.SCMS_MSITEM_LATs
          //                join q1 in db.MsTransDs on q.c_type_lat equals q1.c_type
          //               where q.c_iteno == field.Item && q1.c_portal == '9' && q1.c_notrans == "003"  && q.c_gdg == gudang
          //                select new PLLat
          //               {
          //                 Item = q.c_iteno,
          //                 TypeLat = q.c_type_lat,
          //                 Ket = q1.v_ket,
          //               }).Take(1).SingleOrDefault();

          //PLVia via = (from q in db.SCMS_MSITEM_VIAs
          //                join q1 in db.MsTransDs on q.c_via equals q1.c_type
          //              where q.c_iteno == field.Item && q1.c_portal == '3' && q1.c_notrans == "02"
          //              && q.c_cusno == structure.Fields.Customer
          //              select new PLVia
          //               {
          //                 Item = q.c_iteno,
          //                 TypeVia = q.c_via,
          //                 Ket = q1.v_ket,
          //               }).Take(1).SingleOrDefault();

          PLDiv div = (from q in db.FA_Divpris
                        join q1 in db.FA_MsDivPris on q.c_kddivpri equals q1.c_kddivpri
                        where q.c_iteno == field.Item
                        select new PLDiv
                        {
                           Item = q.c_iteno,
                           TypeDiv = q.c_kddivpri,
                           Ket = q1.v_nmdivpri,
                        }).Take(1).SingleOrDefault();
             
          string sItemBatch = string.Concat(field.Item, field.Batch);

          plComp = new PLClassItemComponen()
          {
            BatchID = field.Batch,
            Item = field.Item,
            Qty = field.Quantity,
            nSisa = field.Quantity,
            ViaID = structure.Fields.Via,
            SpID = field.NomorSP,
            Supplier = string.IsNullOrEmpty(structure.Fields.Suplier) ? masitm.c_nosup : structure.Fields.Suplier,
            Box = masitm.n_box.HasValue ? masitm.n_box.Value : 0,
            //LatID = (itmLat == null ? "00" : itmLat.TypeLat),
            LatID = "00",
            CatID = structure.Fields.TypeKategori,
            isED = field.isED,
            accket = field.accket,
          };

          if (plComp != null)
          {
              if (structure.Fields.Suplier == "00019")
              {
                  plComp.DivID = div.TypeDiv;
              }
            lstplComp.Add(plComp);
          }
        }

        Dictionary<string, PLClassItemComponen> sData = null;
        Dictionary<string, List<PLClassItemComponen>> lstsData = null,
          sContData = null;

        if (lstplComp.Count > 0)
        {
          sData = new Dictionary<string, PLClassItemComponen>();
          lstsData = new Dictionary<string, List<PLClassItemComponen>>();
          for (nLoop = 0; nLoop < lstplComp.Count; nLoop++)
          {
            lstplComp1 = new List<PLClassItemComponen>();
            plComp = lstplComp[nLoop];

            string sNames = string.Concat(plComp.Supplier,plComp.ViaID,plComp.CatID);
            //string sNames = string.Concat(plComp.Supplier,plComp.ViaID,plComp.CatID,plComp.LatID);
            //if (structure.Fields.Suplier == "00019")
            //{
            //    sNames = string.Concat(plComp.Supplier, plComp.DivID ,plComp.ViaID, plComp.CatID, plComp.LatID);
            //}
            plComp.nGropName = sNames;
            lstplCompStatic.Add(plComp);
            lstplComp1.Add(plComp);

            if (lstsData.Count > 0)
            {
              if (lstsData.ContainsKey(sNames))
              {
                lstsData[sNames].Add(plComp);
              }
              else
              {
                lstsData.Add(sNames, lstplComp1);
              }
              
            }
            else
            {
              lstsData.Add(sNames, lstplComp1);
            }
            
          }
        }

        #region pemisahan master box dan receh
        List<PLClassItemSum> itmSum = null,
          itmSumTake = null;
        PLClassItemComponen itmComp = null,
          itmComp1 = null ;
        List<PLClassItemComponen> lstplComp2 = null,
          lstplComp2ToAdd = null, lstplComp2Temp = null;
        sContData = new Dictionary<string, List<PLClassItemComponen>>();
        itmSum = new List<PLClassItemSum>();
        itmSumTake = new List<PLClassItemSum>();
        itmSum = lstplCompStatic.GroupBy(x => new { nGropName  = x.nGropName, BatchID = x.BatchID, Item = x.Item, box = x.Box }).Select(f => new PLClassItemSum { groupName = f.Key.nGropName,BatchID = f.Key.BatchID, Item = f.Key.Item, box = f.Key.box, Qty = f.Sum(g => g.Qty) }).ToList();

        lstplComp2ToAdd = new List<PLClassItemComponen>();

        foreach (KeyValuePair<string, List<PLClassItemComponen>> kvps in lstsData)
        {
            lstplComp1 = kvps.Value;
            string sNames = kvps.Key;
            decimal flor = 0,
                       receh = 0;
            itmSumTake = itmSum.Where(x => x.groupName == sNames).ToList();
            lstplComp2Temp = new List<PLClassItemComponen>();

            lstplComp2 = new List<PLClassItemComponen>();

            for (nLoop = 0; nLoop < itmSumTake.Count; nLoop++)
            {
                lstplComp2 = lstplComp1.Where(x => x.Item == itmSumTake[nLoop].Item && x.BatchID == itmSumTake[nLoop].BatchID).ToList();

                decimal nTotal = itmSumTake[nLoop].Qty;
                if (lstplComp2.Count() > 0)
                {
                    for (nLoopC = 0; nLoopC < lstplComp2.Count; nLoopC++)
                    {
                        itmComp = lstplComp2[nLoopC];

                        itmComp.Box = itmComp.Box <= 0 ? 1 : itmComp.Box;
                        #region Old
                        //if (itmComp.Qty / itmComp.Box >= 1)
                        //{
                        //    flor = Math.Floor((itmComp.Qty / itmComp.Box));
                        //    itmComp1 = new PLClassItemComponen()
                        //    {
                        //        BatchID = itmComp.BatchID,
                        //        Box = itmComp.Box,
                        //        CatID = itmComp.CatID,
                        //        isBox = true,
                        //        Item = itmComp.Item,
                        //        LatID = itmComp.LatID,
                        //        nGropName = itmComp.nGropName,
                        //        nSisa = (flor * itmComp.Box),
                        //        Qty = (flor * itmComp.Box),
                        //        SpID = itmComp.SpID,
                        //        Supplier = itmComp.Supplier,
                        //        ViaID = itmComp.ViaID,
                        //        isED = itmComp.isED,
                        //        accket = itmComp.accket,
                        //        DivID = itmComp.DivID
                        //    };
                        //    lstplComp2ToAdd.Add(itmComp1);

                        //    if ((itmComp.Qty % itmComp.Box) > 0)
                        //    {
                        //        receh = (itmComp.Qty % itmComp.Box);

                        //        itmComp1 = new PLClassItemComponen()
                        //        {
                        //            BatchID = itmComp.BatchID,
                        //            Box = itmComp.Box,
                        //            CatID = itmComp.CatID,
                        //            isBox = false,
                        //            Item = itmComp.Item,
                        //            LatID = itmComp.LatID,
                        //            nGropName = itmComp.nGropName,
                        //            nSisa = receh,
                        //            Qty = receh,
                        //            SpID = itmComp.SpID,
                        //            Supplier = itmComp.Supplier,
                        //            ViaID = itmComp.ViaID,
                        //            isED = itmComp.isED,
                        //            accket = itmComp.accket,
                        //            DivID = itmComp.DivID
                        //        };
                        //    }
                        //    lstplComp2ToAdd.Add(itmComp1);
                        //}
                        //else
                        //{
                        //itmComp1 = new PLClassItemComponen()
                        //{
                        //    BatchID = itmComp.BatchID,
                        //    Box = itmComp.Box,
                        //    CatID = itmComp.CatID,
                        //    isBox = false,
                        //    Item = itmComp.Item,
                        //    LatID = itmComp.LatID,
                        //    nGropName = itmComp.nGropName,
                        //    nSisa = itmComp.nSisa,
                        //    Qty = itmComp.Qty,
                        //    SpID = itmComp.SpID,
                        //    Supplier = itmComp.Supplier,
                        //    ViaID = itmComp.ViaID,
                        //    isED = itmComp.isED,
                        //    accket = itmComp.accket,
                        //    DivID = itmComp.DivID
                        //};
                        //lstplComp2ToAdd.Add(itmComp1);
                        //}
                        #endregion
                        itmComp1 = new PLClassItemComponen()
                            {
                                BatchID = itmComp.BatchID,
                                Box = itmComp.Box,
                                CatID = itmComp.CatID,
                                isBox = false,
                                Item = itmComp.Item,
                                LatID = itmComp.LatID,
                                nGropName = itmComp.nGropName,
                                nSisa = itmComp.nSisa,
                                Qty = itmComp.Qty,
                                SpID = itmComp.SpID,
                                Supplier = itmComp.Supplier,
                                ViaID = itmComp.ViaID,
                                isED = itmComp.isED,
                                accket = itmComp.accket,
                                DivID = itmComp.DivID
                            };
                            lstplComp2ToAdd.Add(itmComp1);

                    }
                }
            }

        }
        #endregion

        lstplComp2Temp = new List<PLClassItemComponen>();

        //sContData = lstplComp2ToAdd.GroupBy(x => x.nGropName).ToDictionary(x => x.Key, x => x.ToList());

        sContData = lstplComp2ToAdd.GroupBy(x => string.Concat(x.nGropName, x.isBox.ToString())).ToDictionary(x => x.Key, x => x.ToList());

        lstsData.Clear();

        lstsData = sContData;

        if (lstsData.Count > 0)
        {
          lstplComp1 = new List<PLClassItemComponen>();

          listPld1_1 = new List<LG_PLD1>();
          List<Functionals.PL1> lstPL1 = null;

          foreach (KeyValuePair<string, List<PLClassItemComponen>> kvps in lstsData)
          {
            string sd = kvps.Key;
            lstplComp1 = kvps.Value;
            List<string> itemcek = new List<string>();
            plID = null;
            int index = 0;
        StartLogic:
            itemcek.Clear();
            if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
            {
              #region Add

              #region Check Header

              if (!string.IsNullOrEmpty(plID) && index == 0)
              {
                result = "Nomor Packing List harus kosong.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
              }
              else if (string.IsNullOrEmpty(lstplComp1[0].Supplier))
              {
                result = "Nama pemasok dibutuhkan.";

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
              db.Connection.Open();

              db.Transaction = db.Connection.BeginTransaction();

              var data = db.Transaction.IsolationLevel;

              data = System.Data.IsolationLevel.RepeatableRead;

              if (Commons.IsClosingLogistik(db, date))
              {
                result = "Packing List tidak dapat disimpan, karena sudah closing.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
              }

              Constant.TRANSID = (from q in db.LG_PLHs
                                  where q.d_pldate.Value.Month == date.Date.Month
                                  select q.c_plno).Max();

              Constant.Gudang = gudang;

              plID = Commons.GenerateNumbering<LG_PLH>(db, "PL", '3', "08", date, "c_plno");

              plh = new LG_PLH()
              {
                c_plno = plID,
                c_cusno = structure.Fields.Customer,
                c_entry = nipEntry,
                c_gdg = gudang,
                c_nosup = lstplComp1[0].Supplier,
                c_type = structure.Fields.TypePackingList,
                c_update = nipEntry,
                c_via = lstplComp1[0].ViaID,
                d_entry = date,
                d_pldate = date.Date,
                d_update = date,
                l_confirm = false,
                l_print = false,
                v_ket = structure.Fields.Keterangan,
                c_type_cat = lstplComp1[0].CatID,
                c_type_lat = lstplComp1[0].LatID,
                l_delete = false,
                l_cek = false,
                l_box = lstplComp1[0].isBox,
                c_plnum = Constant.NUMBERID_GUDANG,
                c_kddivpri = structure.Fields.DivPriID,
              };

              sIDGab += string.Concat(plID, ",");
              sIDGabVia += string.Concat(plID, lstplComp1[0].ViaID, ",");             

              db.LG_PLHs.InsertOnSubmit(plh);
              db.SubmitChanges();

              db.Transaction.Commit();
              db.Connection.Close();

              #endregion

              #region Insert Detail
              db.Connection.Open();

              db.Transaction = db.Connection.BeginTransaction();
              if ((lstplComp1.Count > 0))
              {
                listRN = new List<string>();
                listPld1 = new List<LG_PLD1>();
                listPld2 = new List<LG_PLD2>();
                dicItemStock = new Dictionary<string, List<PLClassComponent>>(StringComparer.OrdinalIgnoreCase);
                lstPL1 = new List<Functionals.PL1>();
                //lstBatch = new List<string>();
                listPld3 = new List<LG_PLD3>();
                for (nLoop = 0; nLoop < lstplComp1.Count; nLoop++)
                {


                    listPld3.Add(new LG_PLD3()
                    {
                      c_plno = plID,
                      c_iteno = plComp.Item,
                      c_batch = plComp.BatchID,
                      c_spno = plComp.SpID,
                      c_rnno = "RNCEK",
                      c_type = "01",
                      n_qty = spAlloc,
                      n_sisa = spAlloc,
                      l_expired = plComp.isED,
                      v_ket_ed = plComp.isED ? plComp.accket : null,
                      c_acc_ed = plComp.isED ? nipEntry : null,
                      v_ket_del = "Log item yang masuk"
                    });
                    if (index != 0 && index > nLoop)
                    {
                        nLoop = index + 1;
                    }
                  plComp = lstplComp1[nLoop];

                  #region Cek SP

                  spd1 = (from q in db.LG_SPD1s
                          join q1 in
                            (from sq in db.LG_ORHs
                             join sq1 in db.LG_ORD2s on new { sq.c_gdg, sq.c_orno } equals new { sq1.c_gdg, sq1.c_orno }
                             where sq.c_type == "02" && sq1.c_spno == plComp.SpID && sq1.c_iteno == plComp.Item
                             select new
                             {
                               sq.c_gdg,
                               sq1.c_spno,
                               sq1.c_iteno
                             }) on new { q.c_spno, q.c_iteno } equals new { q1.c_spno, q1.c_iteno } into q_1
                          from qSQ in q_1.DefaultIfEmpty()
                          where q.c_spno == plComp.SpID && q.c_iteno == plComp.Item //&& qORH.c_type == "02"
                          select q).Distinct().Take(1).SingleOrDefault();

                  if (spd1 != null)
                  {
                    spSisa = (spd1.n_sisa.HasValue ? spd1.n_sisa.Value : 0);

                    if (spSisa <= 0)
                    {
                      continue;
                    }

                    listSPAC = new List<PLClassComponent>();

                    sItemAndBatch = plComp.Item.Trim() + plComp.BatchID.Trim();

                    if (dicItemStock.ContainsKey(sItemAndBatch))
                    {
                      listSPACTemp = dicItemStock[sItemAndBatch];
                    }
                    else
                    {
                      listSPACTemp = (from q in ScmsSoaLibrary.Modules.GlobalQuery.ViewStockLiteContains(db, gudang, plComp.Item)
                                      //join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                      //join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                      //from qBat in q_2.DefaultIfEmpty()
                                      //where (q.n_gsisa > 0) && (q.c_batch == field.Batch)
                                      where (q.n_gsisa != 0) && (q.c_batch == plComp.BatchID.Trim())
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
                        else if (!plcc.BatchID.Equals(plComp.BatchID, StringComparison.OrdinalIgnoreCase))
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

                      nQtyTemp = plComp.Qty;

                      #region New Stock Indra 20180305FM

                      if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                      plID,
                                                      "01",
                                                      plComp.Item,
                                                      plComp.BatchID,
                                                      nQtyTemp * -1,
                                                      0,
                                                      "KOSONG",
                                                      "02",
                                                      "01",
                                                      nipEntry,
                                                      "01")) == 0)
                      {
                          result = "Terdapat Kesalahan pada Item " + plComp.Item + " dengan Batch " + plComp.BatchID + ". Harap Hubungi MIS.";

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
                                    && (q.c_iteno == plComp.Item) && (q.c_batch == spac.BatchID)
                                    && (q.n_gsisa > 0)
                                   select q).Take(1).SingleOrDefault();

                          if (combo != null)
                          {

                            spAlloc = (spSisa > plComp.Qty ? plComp.Qty : spSisa);
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
                                    join q1 in db.LG_MsBatches on new { q.c_batch, q.c_iteno } equals new { q1.c_batch, q1.c_iteno }
                                    where (q.c_gdg == gudang) && (q.c_rnno == spac.RefID)
                                      && (q.c_iteno == plComp.Item) && (q.c_batch == plComp.BatchID)
                                      && (q.n_gsisa > 0) orderby q1.d_expired
                                    select q).Take(1).SingleOrDefault();

                          if (rnd1 != null)
                          {

                            spAlloc = (spSisa > plComp.Qty ? plComp.Qty : spSisa);
                            spAlloc = (spac.Qty > spAlloc ? spAlloc : spac.Qty);
                            spAlloc = (totalCurrentStock > spAlloc ? spAlloc : totalCurrentStock);

                            refID = rnd1.c_rnno;

                            totalCurrentStock -= spAlloc;

                            spac.Qty -= spAlloc;
                            rnd1.n_gsisa -= spAlloc;

                            if (rnd1.n_gsisa < 0) //cek rn minus
                            {
                                result = "Qty RN Kurang (add PL) " + plComp.Item + " " + plComp.BatchID;
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
                            return plComp.Item.Equals(pld.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                              plComp.BatchID.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
                              //spac.BatchID.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
                              plComp.SpID.Equals(pld.c_spno, StringComparison.OrdinalIgnoreCase);
                          });

                          if (pld1 == null)
                          {
                            listPld1.Add(new LG_PLD1()
                            {
                              c_plno = plID,
                              c_iteno = plComp.Item,
                              c_batch = plComp.BatchID,
                              c_spno = plComp.SpID,
                              c_type = "01",
                              n_booked = spAlloc,
                              n_qty = spAlloc,
                              n_sisa = spAlloc,
                              l_expired = plComp.isED,
                              v_ket_ed = plComp.isED ? plComp.accket : null,
                              c_acc_ed = plComp.isED ? nipEntry : null
                            });

                            lstPL1.Add(new Functionals.PL1()
                            {
                              c_plno = plID,
                              c_iteno = plComp.Item,
                              c_batch = plComp.BatchID,
                              c_spno = plComp.SpID,
                              c_type = "01",
                              n_booked = spAlloc,
                              n_qty = spAlloc,
                              n_sisa = spAlloc 
                            });
                          }
                          else
                          {
                            pld1.n_booked = pld1.n_qty = pld1.n_sisa += spAlloc;
                          }

                          pld2 = listPld2.Find(delegate(LG_PLD2 pld)
                          {
                            return plComp.Item.Equals(pld.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                              plComp.BatchID.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
                              //spac.BatchID.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
                              plComp.SpID.Equals(pld.c_spno, StringComparison.OrdinalIgnoreCase) &&
                              refID.Equals(pld.c_rnno, StringComparison.OrdinalIgnoreCase);
                          });

                          if (pld2 == null)
                          {
                            listPld2.Add(new LG_PLD2()
                            {
                              c_plno = plID,
                              c_iteno = plComp.Item,
                              c_batch = plComp.BatchID,
                              //c_batch = spac.BatchID,
                              c_spno = plComp.SpID,
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
                          if (nLoop == 0)
                          {
                              itemcek.Add(plComp.Item);
                          }
                          else if (nLoop != 0)
                          {
                              int loop = 0;
                              bool res = false;
                              for (loop = 0; loop < itemcek.Count(); loop++)
                              {
                                  if (itemcek[loop].ToString() != plComp.Item)
                                  {
                                      res = false;
                                  }
                                  else
                                  {
                                      res = true;
                                      goto truelogic;
                                  }
                              }
                              truelogic:
                              if (res == false)
                              {
                                  itemcek.Add(plComp.Item);
                              }
                          }
                          if (itemcek.Count == 10)
                          {
                              int i = nLoop + 1;
                              if (i != lstplComp1.Count())
                              {
                                  index = nLoop;
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
                                  if ((listPld3 != null) && (listPld3.Count > 0))
                                  {
                                      db.LG_PLD3s.InsertAllOnSubmit(listPld3.ToArray());
                                      listPld3.Clear();
                                  }
                                  db.SubmitChanges();
                                  db.Transaction.Commit();
                                  db.Connection.Close();
                                  goto StartLogic;
                              }
                          }
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

                #region Cleaning

                foreach (KeyValuePair<string, List<PLClassComponent>> kvp in dicItemStock)
                {
                  kvp.Value.Clear();
                }

                dicItemStock.Clear();

                #endregion

                if ((listPld1 != null) && (listPld1.Count > 0))
                {
                  sdfdd.Add(string.Concat(plID, kvps.Key), lstPL1);
                  db.LG_PLD1s.InsertAllOnSubmit(listPld1.ToArray());
                  listPld1.Clear();
                }

                if ((listPld2 != null) && (listPld2.Count > 0))
                {
                  db.LG_PLD2s.InsertAllOnSubmit(listPld2.ToArray());
                  listPld2.Clear();
                }
                if (listPld3 != null && listPld3.Count > 0)
                {
                  db.LG_PLD3s.InsertAllOnSubmit(listPld3.ToArray());
                  listPld3.Clear();
                }
              }

              #endregion

              #endregion

              db.SubmitChanges();

              db.Transaction.Commit();
              db.Connection.Close();

            }


            dic = new Dictionary<string, string>();
          }

          if (totalDetails > 0)
          {
            sIDGab = sIDGab.Remove((sIDGab.Length - 1), 1);
            sIDGabVia = sIDGabVia.Remove((sIDGabVia.Length - 1), 1);

            dic.Add("PL", sIDGab);
            dic.Add("PLVIA", sIDGabVia);
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
        }

        if (hasAnyChanges)
        {
          //db.SubmitChanges();

          //db.Transaction.Commit();
          //db.Transaction.Rollback();

          rpe = ResponseParser.ResponseParserEnum.IsSuccess;
        }
        else
        {
            var cek = (from q in db.LG_PLD1s where q.c_plno == plID select q).Distinct().SingleOrDefault();
            if (cek.c_plno == null)
            {
                db.ExecuteCommand("delete from lg_plh where c_plno = '" + plID + "'");
            }
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

  }
}
