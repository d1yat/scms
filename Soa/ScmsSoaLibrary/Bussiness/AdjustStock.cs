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
    internal class AdjDetailGenerator
    {
        public string RefID { get; set; }
        public string SignID { get; set; }
        public string Item { get; set; }
        public decimal n_gsisa { get; set; }
        public decimal n_bsisa { get; set; }
        public string Supplier { get; set; }
        public string BatchID { get; set; }
        public decimal SumGood { get; set; }
        public decimal SumBad { get; set; }
    }

    class AdjustStockGoodBad
    {
        public string AdjustGoodBad(ScmsSoaLibrary.Parser.Class.AdjustStockStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false;
            string result = null;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            ScmsSoaLibrary.Parser.Class.AdjustStockStructureField field = null;
            string nipEntry = null;
            string adjID = null;
            string rnID = null;
            string suplId = null;
            //string tmpNumbering = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now;

            List<LG_AdjustD1> lisAdjStock1 = null;
            List<LG_AdjustD2> lisAdjStock2 = null;
            List<LG_AdjustD3> lisAdjStock3 = null;
            List<LG_RND1> varLisRnd1 = null;
            List<AdjDetailGenerator> listAdjTemp = null,
              istSPAC = null, listRnh = null;
            AdjDetailGenerator AdjTemp = null;
            LG_ComboH comboh = null;

            LG_AdjustH adjH = null;
            LG_AdjustD1 adjD1 = null;
            LG_AdjustD2 adjD2 = null;
            Dictionary<string, List<AdjDetailGenerator>> dicItemStock = null;
            LG_RND1 rnd1 = null;

            LG_RNH rnh = null;

            #region New Stock Indra 20180305FM

            LG_DAILY_STOCK_v2 daily2 = null;
            LG_MOVEMENT_STOCK_v2 movement2 = null;

            #endregion

            int nLoop = 0,
              nLoopC = 0;
            int totalDetails = 0;

            decimal
              nQtyAllocGood = 0,
              nQtyAllocBad = 0,
              totalCurrentStockGood = 0,
              totalCurrentStockBad = 0,
              nGAllocated = 0, nBAllocated = 0;

            decimal nGQtyReloc = 0,
               nBQtyReloc = 0, StockPerBatchGood = 0,
                    StockPerBatchBad = 0;

            decimal bE = 0,
              gE = 0;

            string[] rnhType = new string[] { "01", "05" };

            IDictionary<string, string> dic = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            #region Validasi 20171120

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                if (db.Transaction != null)
                {
                    db.Transaction.Rollback();
                }

                goto endLogic;
            }

            char gdg = (string.IsNullOrEmpty(structure.Fields.Gudang) ? char.MinValue : structure.Fields.Gudang[0]);

            if (gdg == char.MinValue)
            {
                result = "Gudang tidak boleh kosong.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                if (db.Transaction != null)
                {
                    db.Transaction.Rollback();
                }

                goto endLogic;
            }

            #endregion

            adjID = (structure.Fields.AdjustStockID ?? string.Empty);

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
                {
                    #region Add

                    #region Validasi 20171120

                    if (string.IsNullOrEmpty(structure.Fields.Gudang))
                    {
                        result = "Gudang tidak boleh kosong.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, date))
                    {
                        result = "Adjustment tidak dapat disimpan, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    #endregion

                    //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "ADJ");
                    adjID = Commons.GenerateNumbering<LG_AdjustH>(db, "AJ", '3', "15", date, "c_adjno");

                    adjH = new LG_AdjustH()
                    {
                        c_adjno = adjID,
                        c_entry = structure.Fields.Entry,
                        c_gdg = gdg,
                        c_type = structure.Fields.Type,
                        c_update = structure.Fields.Entry,
                        d_adjdate = DateTime.Now,
                        d_entry = date,
                        d_update = date,
                        l_delete = false,
                        v_ket = structure.Fields.Keterangan
                    };

                    db.LG_AdjustHs.InsertOnSubmit(adjH);

                    #region Detil

                    if (structure.Fields.Type.Equals("03", StringComparison.OrdinalIgnoreCase))
                    {
                        #region Tipe Adjust Stock (03)

                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            lisAdjStock1 = new List<LG_AdjustD1>();
                            lisAdjStock2 = new List<LG_AdjustD2>();
                            rnd1 = new LG_RND1();
                            varLisRnd1 = new List<LG_RND1>();
                            dicItemStock = new Dictionary<string, List<AdjDetailGenerator>>(StringComparer.OrdinalIgnoreCase);
                            listAdjTemp = new List<AdjDetailGenerator>();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                field = structure.Fields.Field[nLoop];

                                if ((field != null) && field.IsNew && (!field.IsDelete))
                                {
                                    nQtyAllocGood = field.GQty;
                                    nQtyAllocBad = field.BQty;

                                    if (dicItemStock.ContainsKey(field.Item))
                                    {
                                        listAdjTemp = dicItemStock[field.Item];
                                    }
                                    else
                                    {
                                        listAdjTemp = (from q in GlobalQuery.ViewStockLiteAdjContains(db, gdg, field.Item)
                                                       join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                                       join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                                       //where (q.n_gsisa != 0) || (q.n_bsisa != 0)   <-- Modified 9 jun 2015
                                                       //group q by new { q.c_iteno } into g
                                                       //where (g.Sum(x => x.n_gsisa) >= field.GQty) && (g.Sum(x => x.n_bsisa) >= field.BQty)
                                                       select new AdjDetailGenerator()
                                                       {
                                                           RefID = q.c_no,
                                                           SignID = (q.c_table == null ? string.Empty : q.c_table.Trim()),
                                                           Item = q.c_iteno,
                                                           n_gsisa = q.n_gsisa,
                                                           n_bsisa = q.n_bsisa,
                                                           Supplier = q1.c_nosup,
                                                           BatchID = q.c_batch.Trim(),
                                                           SumGood = 0
                                                       }).ToList();

                                        dicItemStock.Add(field.Item, listAdjTemp);

                                    }

                                    totalCurrentStockGood = listAdjTemp.Sum(t => t.n_gsisa);
                                    totalCurrentStockBad = listAdjTemp.Sum(t => t.n_bsisa);

                                    if (field.GQty < 0)
                                    {
                                        if (totalCurrentStockGood < (field.GQty * -1))
                                        {
                                            continue;
                                        }
                                    }
                                    if (field.BQty < 0)
                                    {
                                        if (totalCurrentStockBad < (field.BQty * -1))
                                        {
                                            continue;
                                        }
                                    }

                                    if (listAdjTemp.Count > 0)
                                    {
                                        lisAdjStock1.Add(new LG_AdjustD1()
                                        {
                                            c_adjno = adjID,
                                            c_batch = field.Batch,
                                            c_gdg = gdg,
                                            c_iteno = field.Item,
                                            n_bqty = field.BQty,
                                            n_gqty = field.GQty,
                                            v_ket = field.KetDet
                                        });

                                        istSPAC = listAdjTemp.FindAll(delegate(AdjDetailGenerator adj)
                                        {
                                            if (!adj.BatchID.Trim().Equals(field.Batch.Trim(), StringComparison.OrdinalIgnoreCase))
                                            {
                                                return false;
                                            }

                                            return true;
                                        });

                                        nQtyAllocBad = field.BQty;
                                        nQtyAllocGood = field.GQty;

                                        #region New Stock Indra 20180305FM

                                        if ((SavingStock.DailyStock(db, gdg.ToString(),
                                                                        adjID,
                                                                        structure.Fields.Type,
                                                                        field.Item,
                                                                        field.Batch,
                                                                        nQtyAllocGood,
                                                                        nQtyAllocBad,
                                                                        "KOSONG",
                                                                        "03",
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

                                        if ((istSPAC != null) && (istSPAC.Count > 0))
                                        {
                                            for (nLoopC = 0; nLoopC < istSPAC.Count; nLoopC++)
                                            {
                                                AdjTemp = istSPAC[nLoopC];

                                                if (nQtyAllocGood != 0 || nQtyAllocBad != 0)
                                                {
                                                    if (AdjTemp.SignID.Equals("RN") || AdjTemp.SignID.Equals("RR"))
                                                    {
                                                        rnd1 = (from q in db.LG_RND1s
                                                                where q.c_gdg == gdg && q.c_rnno == AdjTemp.RefID
                                                                && q.c_iteno == field.Item && q.c_batch == field.Batch
                                                                select q).Take(1).SingleOrDefault();

                                                        if (rnd1 != null)
                                                        {
                                                            if (nQtyAllocGood > 0)
                                                            {
                                                                nGQtyReloc = nQtyAllocGood;
                                                            }
                                                            else
                                                            {
                                                                nGQtyReloc = (Math.Abs(nQtyAllocGood) > AdjTemp.n_gsisa ? AdjTemp.n_gsisa : Math.Abs(nQtyAllocGood));

                                                                nGQtyReloc *= -1;
                                                            }

                                                            if (nQtyAllocBad > 0)
                                                            {
                                                                nBQtyReloc = nQtyAllocBad;
                                                            }
                                                            else
                                                            {
                                                                nBQtyReloc = (Math.Abs(nQtyAllocBad) > AdjTemp.n_bsisa ? AdjTemp.n_bsisa : Math.Abs(nQtyAllocBad));

                                                                nBQtyReloc *= -1;
                                                            }

                                                            if ((nBQtyReloc != 0) || (nGQtyReloc != 0))
                                                            {

                                                                rnd1.n_bsisa += nBQtyReloc;
                                                                rnd1.n_gsisa += nGQtyReloc;

                                                                AdjTemp.n_bsisa += nBQtyReloc;
                                                                AdjTemp.n_gsisa += nGQtyReloc;

                                                                nQtyAllocGood -= nGQtyReloc;
                                                                nQtyAllocBad -= nBQtyReloc;

                                                                lisAdjStock2.Add(new LG_AdjustD2()
                                                                {
                                                                    c_adjno = adjID,
                                                                    c_batch = field.Batch,
                                                                    c_gdg = gdg,
                                                                    c_iteno = field.Item,
                                                                    c_rnno = rnd1.c_rnno,
                                                                    n_bqty = nBQtyReloc,
                                                                    n_gqty = nGQtyReloc,
                                                                });

                                                                totalDetails++;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        comboh = (from q in db.LG_ComboHs
                                                                  where q.c_gdg == gdg && q.c_combono == AdjTemp.RefID
                                                                  && q.c_iteno == field.Item && q.c_batch == field.Batch
                                                                  select q).Take(1).SingleOrDefault();

                                                        if (comboh != null)
                                                        {
                                                            if (nQtyAllocGood > 0)
                                                            {
                                                                nGQtyReloc = nQtyAllocGood;
                                                            }
                                                            else
                                                            {
                                                                nGQtyReloc = (Math.Abs(nQtyAllocGood) > AdjTemp.n_gsisa ? AdjTemp.n_gsisa : Math.Abs(nQtyAllocGood));

                                                                nGQtyReloc *= -1;
                                                            }

                                                            if (nQtyAllocBad > 0)
                                                            {
                                                                nBQtyReloc = nQtyAllocBad;
                                                            }
                                                            else
                                                            {
                                                                nBQtyReloc = (Math.Abs(nQtyAllocBad) > AdjTemp.n_bsisa ? AdjTemp.n_bsisa : Math.Abs(nQtyAllocBad));

                                                                nBQtyReloc *= -1;
                                                            }

                                                            if ((nBQtyReloc != 0) || (nGQtyReloc != 0))
                                                            {

                                                                comboh.n_bsisa += nBQtyReloc;
                                                                comboh.n_gsisa += nGQtyReloc;

                                                                nQtyAllocGood -= nGQtyReloc;
                                                                nQtyAllocBad -= nBQtyReloc;

                                                                lisAdjStock2.Add(new LG_AdjustD2()
                                                                {
                                                                    c_adjno = adjID,
                                                                    c_batch = field.Batch,
                                                                    c_gdg = gdg,
                                                                    c_iteno = field.Item,
                                                                    c_rnno = comboh.c_combono,
                                                                    n_bqty = nBQtyReloc,
                                                                    n_gqty = nGQtyReloc,
                                                                });

                                                                totalDetails++;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            suplId = (from q in db.FA_MasItms
                                                      where (q.c_iteno == field.Item)
                                                      select (q.c_nosup == null ? string.Empty : q.c_nosup.Trim())).Take(1).SingleOrDefault();

                                            rnID = string.Concat("RNXXD", (suplId ?? "ADJST"));

                                            rnd1 = (from q in db.LG_RND1s
                                                    where (q.c_gdg == gdg) && (q.c_rnno == rnID)
                                                    && (q.c_iteno == field.Item) && q.c_batch == field.Batch
                                                    select q).Take(1).SingleOrDefault();

                                            if (rnd1 == null)
                                            {
                                                rnd1 = new LG_RND1()
                                                {
                                                    c_gdg = gdg,
                                                    c_rnno = rnID,
                                                    c_iteno = field.Item,
                                                    c_batch = field.Batch,
                                                    n_bqty = 0,
                                                    n_bsisa = field.BQty,
                                                    n_gqty = 0,
                                                    n_gsisa = field.GQty
                                                };

                                                db.LG_RND1s.InsertOnSubmit(rnd1);

                                                const string MSBATCHITM = "MSBATCHITM";

                                                rnh = (from q in db.LG_RNHs
                                                       where (q.c_gdg == gdg) && (q.c_rnno == rnID) 
                                                       && (q.c_dono == MSBATCHITM) 
                                                       select q).Take(1).SingleOrDefault();

                                                listRnh = listAdjTemp.FindAll(delegate(AdjDetailGenerator adj)
                                                {
                                                    return adj.RefID.Equals(rnID, StringComparison.OrdinalIgnoreCase);
                                                });

                                                if (listRnh.Count == 0 && rnh == null)
                                                {
                                                    rnh = new LG_RNH()
                                                    {
                                                        c_gdg = gdg,
                                                        c_rnno = rnID,
                                                        d_rndate = date,
                                                        c_type = "07",
                                                        l_float = false,
                                                        c_dono = MSBATCHITM,
                                                        d_dodate = date,
                                                        v_ket = "Auto Create From Adjust Stock",
                                                        c_from = suplId,
                                                        n_bea = 0,
                                                        l_print = false,
                                                        l_status = false,
                                                        c_entry = nipEntry,
                                                        d_entry = date,
                                                        c_update = nipEntry,
                                                        d_update = date,
                                                        l_delete = false,
                                                        v_ket_mark = string.Empty
                                                    };

                                                    dicItemStock[field.Item].Add(new AdjDetailGenerator()
                                                    {
                                                        SignID = "RN",
                                                        RefID = rnID,
                                                        Item = field.Item,
                                                        BatchID = field.Batch,
                                                        n_gsisa = field.GQty,
                                                        n_bsisa = field.BQty
                                                    });

                                                    db.LG_RNHs.InsertOnSubmit(rnh);
                                                }

                                                lisAdjStock2.Add(new LG_AdjustD2()
                                                {
                                                    c_adjno = adjID,
                                                    c_batch = field.Batch,
                                                    c_gdg = gdg,
                                                    c_iteno = field.Item,
                                                    c_rnno = rnID,
                                                    n_bqty = field.BQty,
                                                    n_gqty = field.GQty
                                                });

                                                totalDetails++;
                                                continue;
                                            }
                                            else
                                            {
                                                result = "Kode barang " + field.Item + " stocknya tidak cukup atau sudah terpakai!";

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

                        #endregion
                    }
                    else if (structure.Fields.Type.Equals("01", StringComparison.OrdinalIgnoreCase))
                    {
                        #region Tipe Adjust Good <-> Bad (01)

                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            lisAdjStock1 = new List<LG_AdjustD1>();
                            lisAdjStock2 = new List<LG_AdjustD2>();
                            varLisRnd1 = new List<LG_RND1>();
                            rnd1 = new LG_RND1();
                            dicItemStock = new Dictionary<string, List<AdjDetailGenerator>>(StringComparer.OrdinalIgnoreCase);
                            listAdjTemp = new List<AdjDetailGenerator>();
                            AdjTemp = new AdjDetailGenerator();
                            comboh = new LG_ComboH();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                field = structure.Fields.Field[nLoop];

                                if ((field != null) && field.IsNew && (!field.IsDelete))
                                {

                                    if (dicItemStock.ContainsKey(field.Item))
                                    {
                                        listAdjTemp = dicItemStock[field.Item];
                                    }
                                    else
                                    {
                                        listAdjTemp = (from q in GlobalQuery.ViewStockLite(db, gdg, field.Item)
                                                       join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                                       join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                                       where (q.n_gsisa != 0) || (q.n_bsisa != 0)
                                                       //group q by new { q.c_iteno } into g
                                                       //where (g.Sum(x => x.n_gsisa) >= field.GQty) && (g.Sum(x => x.n_bsisa) >= field.BQty)
                                                       select new AdjDetailGenerator()
                                                       {
                                                           RefID = q.c_no,
                                                           SignID = (q.c_table == null ? string.Empty : q.c_table.Trim()),
                                                           Item = q.c_iteno,
                                                           n_gsisa = q.n_gsisa,
                                                           n_bsisa = q.n_bsisa,
                                                           Supplier = q1.c_nosup,
                                                           BatchID = q.c_batch.Trim(),
                                                           SumGood = 0
                                                       }).ToList();

                                        dicItemStock.Add(field.Item, listAdjTemp);
                                    }

                                    totalCurrentStockGood = listAdjTemp.Sum(t => t.n_gsisa);
                                    totalCurrentStockBad = listAdjTemp.Sum(t => t.n_bsisa);

                                    if (field.GQty < 0)
                                    {
                                        if (totalCurrentStockGood < (field.GQty * -1))
                                        {
                                            continue;
                                        }
                                    }
                                    if (field.BQty < 0)
                                    {
                                        if (totalCurrentStockBad < (field.BQty * -1))
                                        {
                                            continue;
                                        }
                                    }

                                    #region New Stock Indra 20180305FM

                                    if ((SavingStock.DailyStock(db, gdg.ToString(),
                                                                        adjID,
                                                                        structure.Fields.Type,
                                                                        field.Item,
                                                                        field.Batch,
                                                                        field.GQty,
                                                                        field.GQty * -1,
                                                                        "KOSONG",
                                                                        "03",
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

                                    if (listAdjTemp.Count > 0)
                                    {
                                        lisAdjStock1.Add(new LG_AdjustD1()
                                        {
                                            c_adjno = adjID,
                                            c_batch = field.Batch,
                                            c_gdg = gdg,
                                            c_iteno = field.Item,
                                            n_bqty = (field.GQty * -1),
                                            n_gqty = field.GQty,
                                            v_ket = field.KetDet
                                        });

                                        istSPAC = listAdjTemp.FindAll(delegate(AdjDetailGenerator adj)
                                        {
                                            if (string.IsNullOrEmpty(adj.BatchID.Trim()))
                                            {
                                                return false;
                                            }

                                            if (!adj.BatchID.Trim().Equals(field.Batch.Trim(), StringComparison.OrdinalIgnoreCase))
                                            {
                                                return false;
                                            }

                                            return true;
                                        });

                                        nQtyAllocGood = field.GQty;

                                        if (field.GQty < 0)
                                        {
                                            nQtyAllocGood = (nQtyAllocGood * -1);

                                            for (nLoopC = 0; nLoopC < istSPAC.Count; nLoopC++)
                                            {
                                                AdjTemp = istSPAC[nLoopC];

                                                if (AdjTemp.n_gsisa > 0)
                                                {
                                                    nGAllocated = (nQtyAllocGood > AdjTemp.n_gsisa ? AdjTemp.n_gsisa : nQtyAllocGood);

                                                    if (nQtyAllocGood != 0)
                                                    {
                                                        if (AdjTemp.SignID.Equals("RN") || AdjTemp.SignID.Equals("RR"))
                                                        {
                                                            rnd1 = (from q in db.LG_RND1s
                                                                    where q.c_gdg == gdg && q.c_rnno == AdjTemp.RefID
                                                                    && q.c_iteno == AdjTemp.Item && q.c_batch == AdjTemp.BatchID
                                                                    select q).Take(1).SingleOrDefault();

                                                            if (rnd1 != null)
                                                            {
                                                                lisAdjStock2.Add(new LG_AdjustD2()
                                                                {
                                                                    c_adjno = adjID,
                                                                    c_batch = field.Batch,
                                                                    c_gdg = gdg,
                                                                    c_iteno = field.Item,
                                                                    c_rnno = rnd1.c_rnno,
                                                                    n_bqty = nGAllocated,
                                                                    n_gqty = (nGAllocated * -1),
                                                                });

                                                                rnd1.n_gsisa -= nGAllocated;
                                                                rnd1.n_bsisa += nGAllocated;

                                                                AdjTemp.n_gsisa -= nGAllocated;
                                                                AdjTemp.n_bsisa += nGAllocated;

                                                                nQtyAllocGood -= nGAllocated;

                                                                totalDetails++;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            comboh = (from q in db.LG_ComboHs
                                                                      where q.c_gdg == gdg && q.c_combono == AdjTemp.RefID
                                                                      && q.c_iteno == field.Item && q.c_batch == field.Batch
                                                                      select q).Take(1).SingleOrDefault();

                                                            if (comboh != null)
                                                            {
                                                                lisAdjStock2.Add(new LG_AdjustD2()
                                                                {
                                                                    c_adjno = adjID,
                                                                    c_batch = field.Batch,
                                                                    c_gdg = gdg,
                                                                    c_iteno = field.Item,
                                                                    c_rnno = comboh.c_combono,
                                                                    n_bqty = nGAllocated,
                                                                    n_gqty = (nGAllocated * -1),
                                                                });

                                                                comboh.n_gsisa -= nGAllocated;
                                                                comboh.n_bsisa += nGAllocated;

                                                                AdjTemp.n_gsisa -= nGAllocated;
                                                                AdjTemp.n_bsisa += nGAllocated;

                                                                nQtyAllocGood -= nGAllocated;

                                                                totalDetails++;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            nQtyAllocBad = (nQtyAllocGood);

                                            for (nLoopC = 0; nLoopC < istSPAC.Count; nLoopC++)
                                            {
                                                AdjTemp = istSPAC[nLoopC];

                                                if (AdjTemp.n_bsisa > 0)
                                                {
                                                    nBAllocated = (nQtyAllocBad > AdjTemp.n_bsisa ? AdjTemp.n_bsisa : nQtyAllocBad);

                                                    if (nQtyAllocBad != 0)
                                                    {
                                                        if (AdjTemp.SignID.Equals("RN") || AdjTemp.SignID.Equals("RR"))
                                                        {
                                                            rnd1 = (from q in db.LG_RND1s
                                                                    where q.c_gdg == gdg && q.c_rnno == AdjTemp.RefID
                                                                    && q.c_iteno == AdjTemp.Item && q.c_batch == AdjTemp.BatchID
                                                                    select q).Take(1).SingleOrDefault();

                                                            if (rnd1 != null)
                                                            {
                                                                lisAdjStock2.Add(new LG_AdjustD2()
                                                                {
                                                                    c_adjno = adjID,
                                                                    c_batch = field.Batch,
                                                                    c_gdg = gdg,
                                                                    c_iteno = field.Item,
                                                                    c_rnno = AdjTemp.RefID,
                                                                    n_bqty = (nBAllocated * -1),
                                                                    n_gqty = nBAllocated,
                                                                });

                                                                rnd1.n_gsisa += nBAllocated;
                                                                rnd1.n_bsisa -= nBAllocated;

                                                                AdjTemp.n_gsisa += nBAllocated;
                                                                AdjTemp.n_bsisa -= nBAllocated;

                                                                nQtyAllocBad -= nBAllocated;

                                                                totalDetails++;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            comboh = (from q in db.LG_ComboHs
                                                                      where q.c_gdg == gdg && q.c_combono == AdjTemp.RefID
                                                                      && q.c_iteno == field.Item && q.c_batch == field.Batch
                                                                      select q).Take(1).SingleOrDefault();

                                                            if (comboh != null)
                                                            {
                                                                lisAdjStock2.Add(new LG_AdjustD2()
                                                                {
                                                                    c_adjno = adjID,
                                                                    c_batch = field.Batch,
                                                                    c_gdg = gdg,
                                                                    c_iteno = field.Item,
                                                                    c_rnno = comboh.c_combono,
                                                                    n_bqty = (nBAllocated * -1),
                                                                    n_gqty = nBAllocated,
                                                                });

                                                                comboh.n_gsisa += nBAllocated;
                                                                comboh.n_bsisa -= nBAllocated;

                                                                AdjTemp.n_gsisa += nBAllocated;
                                                                AdjTemp.n_bsisa -= nBAllocated;

                                                                nQtyAllocBad -= nBAllocated;

                                                                totalDetails++;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        #region Tipe Adjust Batch (02)

                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            lisAdjStock1 = new List<LG_AdjustD1>();
                            lisAdjStock2 = new List<LG_AdjustD2>();
                            varLisRnd1 = new List<LG_RND1>();
                            rnd1 = new LG_RND1();
                            AdjTemp = new AdjDetailGenerator();
                            listAdjTemp = new List<AdjDetailGenerator>();
                            dicItemStock = new Dictionary<string, List<AdjDetailGenerator>>();

                            listRnh = new List<AdjDetailGenerator>();
                            rnh = new LG_RNH();
                            comboh = new LG_ComboH();

                            structure.Fields.Field.OrderBy(x => x.Item).ToList();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                field = structure.Fields.Field[nLoop];

                                var item = (from q in db.FA_MasItms where q.c_iteno == field.Item
                                                select q).SingleOrDefault();
                                if (item.l_combo == true)
                                {
                                    #region barang combo
                                    if (field.BQty > 0 || field.GQty > 0)
                                    {
                                        comboh = (from q in db.LG_ComboHs
                                                where q.c_iteno == field.Item && q.c_batch == field.Batch && q.c_gdg == gdg
                                                select q).Take(1).SingleOrDefault();
                                    }
                                    else
                                    {
                                        if (field.BQty < 0)
                                        {
                                            comboh = (from q in db.LG_ComboHs
                                                    where q.c_iteno == field.Item && q.c_batch == field.Batch && q.c_gdg == gdg && q.n_bsisa != 0
                                                    select q).Take(1).SingleOrDefault();
                                        }
                                        else if (field.GQty < 0)
                                        {
                                            comboh = (from q in db.LG_ComboHs
                                                    where q.c_iteno == field.Item && q.c_batch == field.Batch && q.c_gdg == gdg && q.n_gsisa != 0
                                                    select q).Take(1).SingleOrDefault();
                                        }
                                    }
                                    lisAdjStock1.Add(new LG_AdjustD1()
                                    {
                                        c_adjno = adjID,
                                        c_batch = field.Batch,
                                        c_gdg = gdg,
                                        c_iteno = field.Item,
                                        n_bqty = field.BQty,
                                        n_gqty = field.GQty,
                                        v_ket = field.KetDet
                                    });
                                    lisAdjStock2.Add(new LG_AdjustD2()
                                    {
                                        c_adjno = adjID,
                                        c_batch = field.Batch,
                                        c_gdg = gdg,
                                        c_iteno = field.Item,
                                        c_rnno = comboh.c_combono,
                                        n_bqty = field.BQty,
                                        n_gqty = field.GQty,
                                    });
                                    comboh.n_bsisa = comboh.n_bsisa + field.BQty;
                                    comboh.n_gsisa = comboh.n_gsisa + field.GQty;
                                    totalDetails++;
                                    #endregion
                                }
                                else
                                {
                                    #region barang reguler
                                    if (field.BQty > 0 || field.GQty > 0)
                                    {
                                        rnd1 = (from q in db.LG_RND1s
                                                where q.c_iteno == field.Item && q.c_batch == field.Batch && q.c_gdg == gdg
                                                select q).Take(1).SingleOrDefault();
                                    }
                                    else
                                    {
                                        if (field.BQty < 0)
                                        {
                                            rnd1 = (from q in db.LG_RND1s
                                                    where q.c_iteno == field.Item && q.c_batch == field.Batch && q.c_gdg == gdg && q.n_bsisa != 0
                                                    select q).Take(1).SingleOrDefault();
                                        }
                                        else if (field.GQty < 0)
                                        {
                                            rnd1 = (from q in db.LG_RND1s
                                                    where q.c_iteno == field.Item && q.c_batch == field.Batch && q.c_gdg == gdg && q.n_gsisa != 0
                                                    select q).Take(1).SingleOrDefault();
                                        }
                                    }
                                    lisAdjStock1.Add(new LG_AdjustD1()
                                    {
                                        c_adjno = adjID,
                                        c_batch = field.Batch,
                                        c_gdg = gdg,
                                        c_iteno = field.Item,
                                        n_bqty = field.BQty,
                                        n_gqty = field.GQty,
                                        v_ket = field.KetDet
                                    });
                                    lisAdjStock2.Add(new LG_AdjustD2()
                                    {
                                        c_adjno = adjID,
                                        c_batch = field.Batch,
                                        c_gdg = gdg,
                                        c_iteno = field.Item,
                                        c_rnno = rnd1.c_rnno,
                                        n_bqty = field.BQty,
                                        n_gqty = field.GQty,
                                    });
                                    rnd1.n_gsisa = rnd1.n_gsisa + field.GQty;
                                    rnd1.n_bsisa = rnd1.n_bsisa + field.BQty;
                                    totalDetails++;
                                    #endregion
                                }
                                #region Old Code
                                //if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified))
                                //{

                                //    if (dicItemStock.ContainsKey(field.Item))
                                //    {
                                //        listAdjTemp = dicItemStock[field.Item];
                                //    }
                                //    else
                                //    {

                                //        listAdjTemp = (from q in db.LG_RNHs
                                //                       join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                //                       where ((q.c_type != "06") || ((q.l_khusus.HasValue ? q.l_khusus.Value : false) == false))
                                //                         && q1.c_iteno == field.Item
                                //                         && q.c_gdg == gdg
                                //                         && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                //                       select new AdjDetailGenerator()
                                //                       {
                                //                           Item = q1.c_iteno,
                                //                           BatchID = q1.c_batch,
                                //                           SignID = "RN",
                                //                           RefID = q1.c_rnno,
                                //                           n_gsisa = (q1.n_gsisa.HasValue ? q1.n_gsisa.Value : 0),
                                //                           n_bsisa = (q1.n_bsisa.HasValue ? q1.n_bsisa.Value : 0)
                                //                       }).Union(
                                //                      from q in db.LG_ComboHs
                                //                      where q.c_gdg == gdg
                                //                        && q.c_iteno == field.Item
                                //                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                //                      select new AdjDetailGenerator()
                                //                      {
                                //                          SignID = "CB",
                                //                          RefID = q.c_combono,
                                //                          Item = q.c_iteno,
                                //                          BatchID = q.c_batch,
                                //                          n_gsisa = (q.n_gsisa.HasValue ? q.n_gsisa.Value : 0),
                                //                          n_bsisa = (q.n_bsisa.HasValue ? q.n_bsisa.Value : 0)
                                //                      }).Distinct().ToList();

                                //        dicItemStock.Add(field.Item, listAdjTemp);
                                //    }

                                //    totalCurrentStockGood = listAdjTemp.Sum(t => t.n_gsisa);
                                //    totalCurrentStockBad = listAdjTemp.Sum(t => t.n_bsisa);

                                //    if (field.GQty < 0)
                                //    {
                                //        if (totalCurrentStockGood < (field.GQty * -1))
                                //        {
                                //            continue;
                                //        }
                                //    }
                                //    //else      //Modified 11 juni 2015
                                //    //{
                                //    //    if (totalCurrentStockGood < (field.GQty))
                                //    //    {
                                //    //        continue;
                                //    //    }
                                //    //}
                                //    if (field.BQty < 0)
                                //    {
                                //        if (totalCurrentStockBad < (field.BQty * -1))
                                //        {
                                //            continue;
                                //        }
                                //    }
                                //    //else     // Modified 11 juni 2015
                                //    //{
                                //    //    if (totalCurrentStockBad < (field.BQty))
                                //    //    {
                                //    //        continue;
                                //    //    }
                                //    //}

                                //    if (listAdjTemp.Count > 0)
                                //    {
                                //        lisAdjStock1.Add(new LG_AdjustD1()
                                //        {
                                //            c_adjno = adjID,
                                //            c_batch = field.Batch,
                                //            c_gdg = gdg,
                                //            c_iteno = field.Item,
                                //            n_bqty = field.BQty,
                                //            n_gqty = field.GQty,
                                //            v_ket = field.KetDet
                                //        });

                                //        istSPAC = listAdjTemp.FindAll(delegate(AdjDetailGenerator adj)
                                //        {
                                //            if (string.IsNullOrEmpty(adj.BatchID.Trim()))
                                //            {
                                //                return false;
                                //            }

                                //            if (!adj.BatchID.Trim().Equals(field.Batch.Trim(), StringComparison.OrdinalIgnoreCase))
                                //            {
                                //                return false;
                                //            }

                                //            return true;
                                //        });

                                //        nQtyAllocGood = field.GQty;
                                //        nQtyAllocBad = field.BQty;

                                //        #region New Stock Indra 20180305FM

                                //        if ((SavingStock.DailyStock(db, gdg.ToString(),
                                //                                        adjID,
                                //                                        structure.Fields.Type,
                                //                                        field.Item,
                                //                                        field.Batch,
                                //                                        nQtyAllocGood,
                                //                                        nQtyAllocBad,
                                //                                        "KOSONG",
                                //                                        "03",
                                //                                        "01",
                                //                                        nipEntry,
                                //                                        "01")) == 0)
                                //        {
                                //            result = "Terdapat Kesalahan pada Item " + field.Item + " dengan Batch " + field.Batch + ". Harap Hubungi MIS.";

                                //            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                //            if (db.Transaction != null)
                                //            {
                                //                db.Transaction.Rollback();
                                //            }

                                //            goto endLogic;
                                //        }

                                //        #endregion

                                //        if (istSPAC.Count > 0)
                                //        {
                                //            for (nLoopC = 0; nLoopC < istSPAC.Count; nLoopC++)
                                //            {
                                //                AdjTemp = istSPAC[nLoopC];

                                //                //GAqum = AdjTemp.n_gsisa + nQtyAllocGood;

                                //                gE = 0;
                                //                bE = 0;

                                //                if (nQtyAllocGood != 0)
                                //                {
                                //                    if (nQtyAllocGood < 0)
                                //                    {
                                //                        //if (GAqum >= 0)
                                //                        //{
                                //                            gE = (Math.Abs(nQtyAllocGood) > AdjTemp.n_gsisa ? AdjTemp.n_gsisa : Math.Abs(nQtyAllocGood));

                                //                            gE *= -1;
                                //                        //}
                                //                    }
                                //                    else
                                //                    {
                                //                        gE = nQtyAllocGood;
                                //                    }
                                //                }
                                //                if (nQtyAllocBad != 0)
                                //                {
                                //                    if (nQtyAllocBad < 0)
                                //                    {
                                //                        //if (BAqum >= 0)
                                //                        //{
                                //                            bE = (Math.Abs(nQtyAllocBad) > AdjTemp.n_bsisa ? AdjTemp.n_bsisa : Math.Abs(nQtyAllocBad));

                                //                            bE *= -1;
                                //                        //}
                                //                    }
                                //                    else
                                //                    {
                                //                        bE = nQtyAllocBad;
                                //                    }
                                //                }
                                //                if (gE != 0 || bE != 0)
                                //                {
                                //                    lisAdjStock2.Add(new LG_AdjustD2()
                                //                    {
                                //                        c_adjno = adjID,
                                //                        c_batch = field.Batch,
                                //                        c_gdg = gdg,
                                //                        c_iteno = field.Item,
                                //                        c_rnno = AdjTemp.RefID,
                                //                        n_bqty = bE,
                                //                        n_gqty = gE,
                                //                    });

                                //                    if (AdjTemp.SignID.Equals("RN"))
                                //                    {
                                //                        rnd1 = (from q in db.LG_RND1s
                                //                                where q.c_gdg == gdg && q.c_iteno == field.Item
                                //                                && q.c_rnno == AdjTemp.RefID && q.c_batch == field.Batch
                                //                                select q).Take(1).SingleOrDefault();
                                //                        if (rnd1 != null)
                                //                        {
                                //                            rnd1.n_bsisa += bE;
                                //                            rnd1.n_gsisa += gE;

                                //                            nQtyAllocGood -= gE;
                                //                            nQtyAllocBad -= bE;

                                //                            AdjTemp.n_bsisa += bE;
                                //                            AdjTemp.n_gsisa += gE;

                                //                            totalDetails++;
                                //                        }
                                //                    }
                                //                    else
                                //                    {
                                //                        comboh = (from q in db.LG_ComboHs
                                //                                  where q.c_gdg == gdg && q.c_iteno == field.Item
                                //                                  && q.c_combono == AdjTemp.RefID && q.c_batch == field.Batch
                                //                                  select q).Take(1).SingleOrDefault();
                                //                        if (comboh != null)
                                //                        {
                                //                            comboh.n_bsisa += bE;
                                //                            comboh.n_gsisa += gE;

                                //                            nQtyAllocGood -= gE;
                                //                            nQtyAllocBad -= bE;

                                //                            AdjTemp.n_bsisa += bE;
                                //                            AdjTemp.n_gsisa += gE;

                                //                            totalDetails++;
                                //                        }
                                //                    }
                                //                }
                                //            }

                                //            if (nQtyAllocGood != 0 || nQtyAllocBad != 0)
                                //            {
                                //                result = "Kode barang " + AdjTemp.Item + " stocknya tidak cukup atau sudah terpakai";

                                //                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                //                if (db.Transaction != null)
                                //                {
                                //                    db.Transaction.Rollback();
                                //                }

                                //                goto endLogic;
                                //            }
                                //        }
                                //        else
                                //        {
                                //            suplId = (from q in db.FA_MasItms
                                //                      where (q.c_iteno == field.Item)
                                //                      select (q.c_nosup == null ? string.Empty : q.c_nosup.Trim())).Take(1).SingleOrDefault();

                                //            rnID = string.Concat("RNXXD", (suplId ?? "ADJST"));

                                //            rnd1 = (from q in db.LG_RND1s
                                //                    where (q.c_gdg == gdg) && (q.c_rnno == rnID)
                                //                    && (q.c_iteno == field.Item) && q.c_batch == field.Batch
                                //                    select q).Take(1).SingleOrDefault();

                                //            if (rnd1 == null)
                                //            {
                                //                rnd1 = new LG_RND1()
                                //                {
                                //                    c_gdg = gdg,
                                //                    c_rnno = rnID,
                                //                    c_iteno = field.Item,
                                //                    c_batch = field.Batch,
                                //                    n_bqty = 0,
                                //                    n_bsisa = field.BQty,
                                //                    n_gqty = 0,
                                //                    n_gsisa = field.GQty
                                //                };

                                //                db.LG_RND1s.InsertOnSubmit(rnd1);

                                //                const string MSBATCHITM = "MSBATCHITM";

                                //                rnh = (from q in db.LG_RNHs
                                //                       where (q.c_gdg == gdg) && (q.c_rnno == rnID)
                                //                        && (q.c_dono == MSBATCHITM)
                                //                       select q).Take(1).SingleOrDefault();

                                //                listRnh = listAdjTemp.FindAll(delegate(AdjDetailGenerator adj)
                                //                {
                                //                    return adj.RefID.Equals(rnID, StringComparison.OrdinalIgnoreCase);
                                //                });

                                //                if (listRnh.Count == 0 && rnh == null)
                                //                {
                                //                    rnh = new LG_RNH()
                                //                    {
                                //                        c_gdg = gdg,
                                //                        c_rnno = rnID,
                                //                        d_rndate = date,
                                //                        c_type = "07",
                                //                        l_float = false,
                                //                        c_dono = MSBATCHITM,
                                //                        d_dodate = date,
                                //                        v_ket = "Auto Create From Adjust Batch",
                                //                        c_from = suplId,
                                //                        n_bea = 0,
                                //                        l_print = false,
                                //                        l_status = false,
                                //                        c_entry = nipEntry,
                                //                        d_entry = date,
                                //                        c_update = nipEntry,
                                //                        d_update = date,
                                //                        l_delete = false,
                                //                        v_ket_mark = string.Empty
                                //                    };

                                //                    dicItemStock[field.Item].Add(new AdjDetailGenerator()
                                //                    {
                                //                        SignID = "RN",
                                //                        RefID = rnID,
                                //                        Item = field.Item,
                                //                        BatchID = field.Batch,
                                //                        n_gsisa = field.GQty,
                                //                        n_bsisa = field.BQty
                                //                    });

                                //                    db.LG_RNHs.InsertOnSubmit(rnh);
                                //                }



                                //                lisAdjStock2.Add(new LG_AdjustD2()
                                //                {
                                //                    c_adjno = adjID,
                                //                    c_batch = field.Batch,
                                //                    c_gdg = gdg,
                                //                    c_iteno = field.Item,
                                //                    c_rnno = rnID,
                                //                    n_bqty = field.BQty,
                                //                    n_gqty = field.GQty
                                //                });

                                //                totalDetails++;
                                //                continue;
                                //            }
                                //            else
                                //            {
                                //                result = "Kode barang " + AdjTemp.Item + " stocknya tidak cukup atau sudah terpakai!";

                                //                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                //                if (db.Transaction != null)
                                //                {
                                //                    db.Transaction.Rollback();
                                //                }

                                //                goto endLogic;
                                //            }
                                //        }
                                //    }
                                //}
                                #endregion
                            }
                        }

                        #endregion
                    }

                    #endregion

                    dic = new Dictionary<string, string>();

                    if (totalDetails > 0)
                    {
                        if ((lisAdjStock1 != null) && (lisAdjStock1.Count > 0))
                        {
                            db.LG_AdjustD1s.InsertAllOnSubmit(lisAdjStock1.ToArray());
                            lisAdjStock1.Clear();
                        }
                        if ((lisAdjStock2 != null) && (lisAdjStock2.Count > 0))
                        {
                            db.LG_AdjustD2s.InsertAllOnSubmit(lisAdjStock2.ToArray());
                            lisAdjStock2.Clear();
                        }

                        dic.Add("Adj", adjID);
                        dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                        result = string.Format("Total {0} detail(s)", totalDetails);

                        hasAnyChanges = true;
                    }

                    #endregion
                }
                if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    #region Validasi Indra 20171120

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

                    adjH = (from q in db.LG_AdjustHs
                            where q.c_adjno == adjID && q.c_gdg == gdg
                            select q).Take(1).SingleOrDefault();

                    if (adjH == null)
                    {
                        result = "Nomor Adjustment tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (adjH.l_delete.HasValue && adjH.l_delete.Value)
                    {
                        result = "Tidak dapat mengubah Nomor Adjustment yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, adjH.d_adjdate))
                    {
                        result = "Adjustment tidak dapat diubah, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
                    {
                        adjH.v_ket = structure.Fields.Keterangan;
                    }

                    #endregion

                    adjH.c_update = nipEntry;
                    adjH.d_update = DateTime.Now;

                    #region Type 01 (Good-Bad)

                    if (structure.Fields.Type.Equals("01", StringComparison.OrdinalIgnoreCase))
                    {
                        #region old coded
                        //  lisAdjStock3 = new List<LG_AdjustD3>();
                        //  lisAdjStock1 = new List<LG_AdjustD1>();
                        //  lisAdjStock2 = new List<LG_AdjustD2>();
                        //  rnd1 = new LG_RND1();
                        //  if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        //  {
                        //    field = structure.Fields.Field[nLoop];

                        //    lisAdjStock2 = (from q in db.LG_AdjustD2s
                        //                    where q.c_gdg == gdg && q.c_adjno == adjID &&
                        //                    q.c_iteno == field.Item && q.c_batch == field.Batch
                        //                    select q).ToList();

                        //    for (nLoop = 0; nLoop < lisAdjStock1.Count; nLoop ++ )
                        //    {
                        //      var varListRnd1 = (from q in db.LG_RND1s
                        //                      where q.c_gdg == lisAdjStock2[nLoop].c_gdg &&
                        //                      q.c_rnno == lisAdjStock2[nLoop].c_rnno &&
                        //                      q.c_iteno == lisAdjStock2[nLoop].c_iteno &&
                        //                      q.c_batch == lisAdjStock2[nLoop].c_batch
                        //                      select q).Where(x => (x.n_gsisa.Value >= field.GQty) && (x.n_bsisa >= field.BQty)).ToList();
                        //      if (varListRnd1.Count > 0)
                        //      {
                        //        rnd1 = (from q in db.LG_RND1s
                        //                where q.c_gdg == lisAdjStock2[nLoop].c_gdg &&
                        //                q.c_rnno == lisAdjStock2[nLoop].c_rnno &&
                        //                q.c_iteno == lisAdjStock2[nLoop].c_iteno &&
                        //                q.c_batch == lisAdjStock2[nLoop].c_batch
                        //                select q).Take(1).SingleOrDefault();

                        //        rnd1.n_gsisa -= field.GQty;
                        //        rnd1.n_bsisa -= field.BQty;

                        //      }
                        //      else
                        //      {
                        //        result = "Tidak dapat mengubah hapus untuk batch " + lisAdjStock2[nLoop].c_batch + " yang sudah terhapus.";

                        //        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        //        db.Transaction.Rollback();
                        //      }

                        //    }
                        //  }

                        #endregion old coded

                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            lisAdjStock3 = new List<LG_AdjustD3>();
                            lisAdjStock1 = new List<LG_AdjustD1>();
                            lisAdjStock2 = new List<LG_AdjustD2>();
                            rnd1 = new LG_RND1();
                            dicItemStock = new Dictionary<string, List<AdjDetailGenerator>>();
                            listAdjTemp = new List<AdjDetailGenerator>();
                            comboh = new LG_ComboH();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                field = structure.Fields.Field[nLoop];

                                if ((field != null) && (field.IsNew) && (!field.IsDelete) && (!field.IsModified))
                                {
                                    #region New

                                    if (dicItemStock.ContainsKey(field.Item))
                                    {
                                        listAdjTemp = dicItemStock[field.Item];
                                    }
                                    else
                                    {
                                        listAdjTemp = (from q in GlobalQuery.ViewStockLite(db, gdg, field.Item)
                                                       join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                                       join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                                       where (q.n_gsisa != 0) || (q.n_bsisa != 0)
                                                       //group q by new { q.c_iteno } into g
                                                       //where (g.Sum(x => x.n_gsisa) >= field.GQty) && (g.Sum(x => x.n_bsisa) >= field.BQty)
                                                       select new AdjDetailGenerator()
                                                       {
                                                           RefID = q.c_no,
                                                           SignID = (q.c_table == null ? string.Empty : q.c_table.Trim()),
                                                           Item = q.c_iteno,
                                                           n_gsisa = q.n_gsisa,
                                                           n_bsisa = q.n_bsisa,
                                                           Supplier = q1.c_nosup,
                                                           BatchID = q.c_batch.Trim(),
                                                           SumGood = 0
                                                       }).ToList();

                                        dicItemStock.Add(field.Item, listAdjTemp);
                                    }

                                    totalCurrentStockGood = listAdjTemp.Sum(t => t.n_gsisa);
                                    totalCurrentStockBad = listAdjTemp.Sum(t => t.n_bsisa);

                                    if (field.GQty < 0)
                                    {
                                        if (totalCurrentStockGood < (field.GQty * -1))
                                        {
                                            continue;
                                        }
                                    }
                                    if (field.BQty < 0)
                                    {
                                        if (totalCurrentStockBad < (field.BQty * -1))
                                        {
                                            continue;
                                        }
                                    }

                                    #region New Stock Indra 20180305FM

                                    if ((SavingStock.DailyStock(db, gdg.ToString(),
                                                                    adjID,
                                                                    adjH.c_type,
                                                                    field.Item,
                                                                    field.Batch,
                                                                    field.GQty,
                                                                    field.GQty * -1,
                                                                    "KOSONG",
                                                                    "03",
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

                                    if (listAdjTemp.Count > 0)
                                    {
                                        lisAdjStock1.Add(new LG_AdjustD1()
                                        {
                                            c_adjno = adjID,
                                            c_batch = field.Batch,
                                            c_gdg = gdg,
                                            c_iteno = field.Item,
                                            n_bqty = (field.GQty * -1),
                                            n_gqty = field.GQty,
                                            v_ket = field.KetDet
                                        });

                                        istSPAC = listAdjTemp.FindAll(delegate(AdjDetailGenerator adj)
                                        {
                                            if (string.IsNullOrEmpty(adj.BatchID.Trim()))
                                            {
                                                return false;
                                            }

                                            if (!adj.BatchID.Trim().Equals(field.Batch.Trim(), StringComparison.OrdinalIgnoreCase))
                                            {
                                                return false;
                                            }

                                            return true;
                                        });

                                        nQtyAllocGood = field.GQty;

                                        if (field.GQty < 0)
                                        {
                                            nQtyAllocGood = (nQtyAllocGood * -1);

                                            for (nLoopC = 0; nLoopC < istSPAC.Count; nLoopC++)
                                            {
                                                AdjTemp = istSPAC[nLoopC];

                                                if (AdjTemp.n_gsisa > 0)
                                                {
                                                    nGAllocated = (nQtyAllocGood > AdjTemp.n_gsisa ? AdjTemp.n_gsisa : nQtyAllocGood);

                                                    if (nQtyAllocGood != 0)
                                                    {
                                                        if (AdjTemp.SignID.Equals("RN") || AdjTemp.SignID.Equals("RR"))
                                                        {
                                                            rnd1 = (from q in db.LG_RND1s
                                                                    where q.c_gdg == gdg && q.c_rnno == AdjTemp.RefID
                                                                    && q.c_iteno == AdjTemp.Item && q.c_batch == AdjTemp.BatchID
                                                                    select q).Take(1).SingleOrDefault();

                                                            if (rnd1 != null)
                                                            {
                                                                lisAdjStock2.Add(new LG_AdjustD2()
                                                                {
                                                                    c_adjno = adjID,
                                                                    c_batch = field.Batch,
                                                                    c_gdg = gdg,
                                                                    c_iteno = field.Item,
                                                                    c_rnno = rnd1.c_rnno,
                                                                    n_bqty = nGAllocated,
                                                                    n_gqty = (nGAllocated * -1),
                                                                });

                                                                rnd1.n_gsisa -= nGAllocated;
                                                                rnd1.n_bsisa += nGAllocated;

                                                                AdjTemp.n_gsisa -= nGAllocated;
                                                                AdjTemp.n_bsisa += nGAllocated;

                                                                nQtyAllocGood -= nGAllocated;

                                                                totalDetails++;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            comboh = (from q in db.LG_ComboHs
                                                                      where q.c_gdg == gdg && q.c_combono == AdjTemp.RefID
                                                                      && q.c_iteno == field.Item && q.c_batch == field.Batch
                                                                      select q).Take(1).SingleOrDefault();

                                                            if (comboh != null)
                                                            {
                                                                lisAdjStock2.Add(new LG_AdjustD2()
                                                                {
                                                                    c_adjno = adjID,
                                                                    c_batch = field.Batch,
                                                                    c_gdg = gdg,
                                                                    c_iteno = field.Item,
                                                                    c_rnno = comboh.c_combono,
                                                                    n_bqty = nGAllocated,
                                                                    n_gqty = (nGAllocated * -1),
                                                                });

                                                                comboh.n_gsisa -= nGAllocated;
                                                                comboh.n_bsisa += nGAllocated;

                                                                AdjTemp.n_gsisa -= nGAllocated;
                                                                AdjTemp.n_bsisa += nGAllocated;

                                                                nQtyAllocGood -= nGAllocated;

                                                                totalDetails++;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            nQtyAllocBad = (nQtyAllocGood);

                                            for (nLoopC = 0; nLoopC < istSPAC.Count; nLoopC++)
                                            {
                                                AdjTemp = istSPAC[nLoopC];

                                                if (AdjTemp.n_bsisa > 0)
                                                {
                                                    nBAllocated = (nQtyAllocBad > AdjTemp.n_bsisa ? AdjTemp.n_bsisa : nQtyAllocBad);

                                                    if (nQtyAllocBad != 0)
                                                    {
                                                        if (AdjTemp.SignID.Equals("RN") || AdjTemp.SignID.Equals("RR"))
                                                        {
                                                            rnd1 = (from q in db.LG_RND1s
                                                                    where q.c_gdg == gdg && q.c_rnno == AdjTemp.RefID
                                                                    && q.c_iteno == AdjTemp.Item && q.c_batch == AdjTemp.BatchID
                                                                    select q).Take(1).SingleOrDefault();

                                                            if (rnd1 != null)
                                                            {
                                                                lisAdjStock2.Add(new LG_AdjustD2()
                                                                {
                                                                    c_adjno = adjID,
                                                                    c_batch = field.Batch,
                                                                    c_gdg = gdg,
                                                                    c_iteno = field.Item,
                                                                    c_rnno = AdjTemp.RefID,
                                                                    n_bqty = (nBAllocated * -1),
                                                                    n_gqty = nBAllocated,
                                                                });

                                                                rnd1.n_gsisa += nBAllocated;
                                                                rnd1.n_bsisa -= nBAllocated;

                                                                AdjTemp.n_gsisa += nBAllocated;
                                                                AdjTemp.n_bsisa -= nBAllocated;

                                                                nQtyAllocBad -= nBAllocated;

                                                                totalDetails++;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            comboh = (from q in db.LG_ComboHs
                                                                      where q.c_gdg == gdg && q.c_combono == AdjTemp.RefID
                                                                      && q.c_iteno == field.Item && q.c_batch == field.Batch
                                                                      select q).Take(1).SingleOrDefault();

                                                            if (comboh != null)
                                                            {
                                                                lisAdjStock2.Add(new LG_AdjustD2()
                                                                {
                                                                    c_adjno = adjID,
                                                                    c_batch = field.Batch,
                                                                    c_gdg = gdg,
                                                                    c_iteno = field.Item,
                                                                    c_rnno = comboh.c_combono,
                                                                    n_bqty = (nBAllocated * -1),
                                                                    n_gqty = nBAllocated,
                                                                });

                                                                comboh.n_gsisa += nBAllocated;
                                                                comboh.n_bsisa -= nBAllocated;

                                                                AdjTemp.n_gsisa += nBAllocated;
                                                                AdjTemp.n_bsisa -= nBAllocated;

                                                                nQtyAllocBad -= nBAllocated;

                                                                totalDetails++;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (totalDetails > 0)
                                    {
                                        if ((lisAdjStock1 != null) && (lisAdjStock1.Count > 0))
                                        {
                                            db.LG_AdjustD1s.InsertAllOnSubmit(lisAdjStock1.ToArray());
                                            lisAdjStock1.Clear();
                                        }
                                        if ((lisAdjStock2 != null) && (lisAdjStock2.Count > 0))
                                        {
                                            db.LG_AdjustD2s.InsertAllOnSubmit(lisAdjStock2.ToArray());
                                            lisAdjStock2.Clear();
                                        }

                                        hasAnyChanges = true;
                                    }

                                    #endregion
                                }
                                else if ((field != null) && (!field.IsNew) && (field.IsDelete) && (!field.IsModified))
                                {
                                    #region Delete

                                    nQtyAllocBad = field.BQty;
                                    nQtyAllocGood = field.GQty;

                                    lisAdjStock2 = (from q in db.LG_AdjustD2s
                                                    where q.c_adjno == adjID && q.c_batch == field.Batch
                                                    && q.c_gdg == gdg && q.c_iteno == field.Item
                                                    select q).ToList();

                                    lisAdjStock1 = (from q in db.LG_AdjustD1s
                                                    where q.c_adjno == adjID && q.c_batch == field.Batch
                                                    && q.c_gdg == gdg && q.c_iteno == field.Item
                                                    select q).ToList();

                                    if (dicItemStock.ContainsKey(field.Item))
                                    {
                                        listAdjTemp = dicItemStock[field.Item];
                                    }
                                    else
                                    {
                                        listAdjTemp = (from q in GlobalQuery.ViewStockLite(db, gdg, field.Item)
                                                       join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                                       join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                                       where (q.n_gsisa != 0) || (q.n_bsisa != 0)
                                                       select new AdjDetailGenerator()
                                                       {
                                                           RefID = q.c_no,
                                                           SignID = (q.c_table == null ? string.Empty : q.c_table.Trim()),
                                                           Item = q.c_iteno,
                                                           n_gsisa = q.n_gsisa,
                                                           n_bsisa = q.n_bsisa,
                                                           Supplier = q1.c_nosup,
                                                           BatchID = q.c_batch.Trim(),
                                                           SumGood = 0
                                                       }).ToList();

                                        dicItemStock.Add(field.Item, listAdjTemp);
                                    }

                                    totalCurrentStockGood = listAdjTemp.Sum(t => t.n_gsisa);
                                    totalCurrentStockBad = listAdjTemp.Sum(t => t.n_bsisa);

                                    StockPerBatchGood = listAdjTemp.Where(x => x.BatchID.Trim() == field.Batch.Trim()).GroupBy(x => x.BatchID).Sum(y => y.Sum(z => z.n_gsisa));
                                    StockPerBatchBad = listAdjTemp.Where(x => x.BatchID.Trim() == field.Batch.Trim()).GroupBy(x => x.BatchID).Sum(y => y.Sum(z => z.n_bsisa));

                                    // Cek Stock Per Item
                                    if (field.GQty > 0)
                                    {
                                        if (totalCurrentStockGood < (field.GQty))
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            //Cek Stock Per Batch
                                            if (StockPerBatchGood < (field.GQty))
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                    if (field.BQty > 0)
                                    {
                                        if (totalCurrentStockBad < (field.BQty))
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            //Cek Stock Per Batch
                                            if (StockPerBatchBad < (field.BQty))
                                            {
                                                continue;
                                            }
                                        }
                                    }

                                    if ((lisAdjStock1 != null) && (lisAdjStock2 != null))
                                    {
                                        if (lisAdjStock1.Count > 0 && lisAdjStock2.Count > 0)
                                        {
                                            nQtyAllocGood = field.GQty;
                                            nQtyAllocBad = field.BQty;

                                            #region New Stock Indra 20180305FM

                                            if ((SavingStock.DailyStock(db, gdg.ToString(),
                                                                            adjID,
                                                                            adjH.c_type,
                                                                            field.Item,
                                                                            field.Batch,
                                                                            nQtyAllocGood * -1,
                                                                            nQtyAllocBad * -1,
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

                                            adjD1 = lisAdjStock1.Find(delegate(LG_AdjustD1 adj)
                                            {
                                                return (
                                                  field.Batch.Trim().Equals(adj.c_batch.Trim(), StringComparison.OrdinalIgnoreCase) &&
                                                  field.Item.Trim().Equals(adj.c_iteno.Trim(), StringComparison.OrdinalIgnoreCase) &&
                                                  adjID.Equals(adj.c_adjno.Trim(), StringComparison.OrdinalIgnoreCase));
                                            });

                                            if (adjD1 != null)
                                            {
                                                db.LG_AdjustD1s.DeleteOnSubmit(adjD1);
                                            }
                                            for (nLoopC = 0; nLoopC < lisAdjStock2.Count; nLoopC++)
                                            {
                                                adjD2 = lisAdjStock2[nLoopC];

                                                lisAdjStock3.Add(new LG_AdjustD3()
                                                {
                                                    c_adjno = adjID,
                                                    c_batch = adjD2.c_batch,
                                                    c_iteno = adjD2.c_iteno,
                                                    c_entry = structure.Fields.Entry,
                                                    d_entry = date,
                                                    n_bqty = adjD2.n_bqty,
                                                    n_gqty = adjD2.n_gqty,
                                                    v_ket_del = field.Keterangan,
                                                    v_type = "02"
                                                });

                                                if (adjD2.c_rnno.StartsWith("RN") || adjD2.c_rnno.StartsWith("RR"))
                                                {
                                                    if (adjD2.n_bqty > 0 || adjD2.n_gqty > 0)
                                                    {
                                                        rnd1 = (from q in db.LG_RND1s
                                                                where q.c_batch.Trim() == adjD2.c_batch.Trim()
                                                                && q.c_gdg == adjD2.c_gdg && q.c_iteno == adjD2.c_iteno
                                                                && q.c_rnno == adjD2.c_rnno && q.n_bsisa >= (adjD2.n_bqty.HasValue ? adjD2.n_bqty.Value : 0)
                                                                && q.n_gsisa >= (adjD2.n_gqty.HasValue ? adjD2.n_gqty.Value : 0)
                                                                select q).Take(1).SingleOrDefault();
                                                    }
                                                    else
                                                    {
                                                        rnd1 = (from q in db.LG_RND1s
                                                                where q.c_batch.Trim() == adjD2.c_batch.Trim()
                                                                && q.c_gdg == adjD2.c_gdg && q.c_iteno == adjD2.c_iteno
                                                                && q.c_rnno == adjD2.c_rnno
                                                                select q).Take(1).SingleOrDefault();
                                                    }

                                                    if (rnd1 != null)
                                                    {
                                                        rnd1.n_gsisa -= adjD2.n_gqty;
                                                        rnd1.n_bsisa -= adjD2.n_bqty;

                                                        nQtyAllocGood -= (adjD2.n_gqty.HasValue ? adjD2.n_gqty.Value : 0);
                                                        nQtyAllocBad -= (adjD2.n_bqty.HasValue ? adjD2.n_bqty.Value : 0);

                                                        db.LG_AdjustD2s.DeleteOnSubmit(adjD2);

                                                        totalDetails++;
                                                    }
                                                    else
                                                    {
                                                        result = "Adjustment tidak dapat diubah, Pengecekan Stock Gagal.";

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
                                                    if (adjD2.n_bqty > 0 || adjD2.n_gqty > 0)
                                                    {
                                                        comboh = (from q in db.LG_ComboHs
                                                                  where q.c_batch.Trim() == adjD2.c_batch.Trim()
                                                                  && q.c_gdg == adjD2.c_gdg && q.c_iteno == adjD2.c_iteno
                                                                  && q.c_combono == adjD2.c_rnno && q.n_bsisa >= (adjD2.n_bqty.HasValue ? adjD2.n_bqty.Value : 0)
                                                                && q.n_gsisa >= (adjD2.n_gqty.HasValue ? adjD2.n_gqty.Value : 0)
                                                                  select q).Take(1).SingleOrDefault();
                                                    }
                                                    else
                                                    {
                                                        comboh = (from q in db.LG_ComboHs
                                                                  where q.c_batch.Trim() == adjD2.c_batch.Trim()
                                                                  && q.c_gdg == adjD2.c_gdg && q.c_iteno == adjD2.c_iteno
                                                                  && q.c_combono == adjD2.c_rnno
                                                                  select q).Take(1).SingleOrDefault();
                                                    }

                                                    if (comboh != null)
                                                    {
                                                        comboh.n_gsisa -= adjD2.n_gqty;
                                                        comboh.n_bsisa -= adjD2.n_bqty;

                                                        nQtyAllocGood -= (adjD2.n_gqty.HasValue ? adjD2.n_gqty.Value : 0);
                                                        nQtyAllocBad -= (adjD2.n_bqty.HasValue ? adjD2.n_bqty.Value : 0);

                                                        db.LG_AdjustD2s.DeleteOnSubmit(adjD2);

                                                        totalDetails++;
                                                    }
                                                    else
                                                    {
                                                        result = "Adjustment tidak dapat diubah, Pengecekan Stock Gagal.";

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

                                    if (totalDetails > 0)
                                    {
                                        lisAdjStock1.Clear();
                                        lisAdjStock2.Clear();

                                        if (lisAdjStock3.Count > 0 && lisAdjStock3 != null)
                                        {
                                            db.LG_AdjustD3s.InsertAllOnSubmit(lisAdjStock3.ToArray());
                                            lisAdjStock3.Clear();
                                        }

                                        hasAnyChanges = true;
                                    }

                                    #endregion
                                }
                            }
                        }
                    }

                    #endregion

                    #region Type 02 (Batch)

                    if (structure.Fields.Type.Equals("02", StringComparison.OrdinalIgnoreCase))
                    {
                        #region old coded
                        //  lisAdjStock3 = new List<LG_AdjustD3>();
                        //  lisAdjStock1 = new List<LG_AdjustD1>();
                        //  lisAdjStock2 = new List<LG_AdjustD2>();
                        //  rnd1 = new LG_RND1();

                        //  if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        //  {
                        //    field = structure.Fields.Field[nLoop];

                        //    lisAdjStock2 = (from q in db.LG_AdjustD2s
                        //                    where q.c_gdg == gdg && q.c_adjno == adjID &&
                        //                    q.c_iteno == field.Item && q.c_batch == field.Batch
                        //                    select q).ToList();

                        //    for (nLoop = 0; nLoop < lisAdjStock1.Count; nLoop++)
                        //    {
                        //      var varListRnd1 = (from q in db.LG_RND1s
                        //                         where q.c_gdg == lisAdjStock2[nLoop].c_gdg &&
                        //                         q.c_rnno == lisAdjStock2[nLoop].c_rnno &&
                        //                         q.c_iteno == lisAdjStock2[nLoop].c_iteno &&
                        //                         q.c_batch == lisAdjStock2[nLoop].c_batch
                        //                         select q).Where(x => (x.n_gsisa.Value >= field.GQty) && (x.n_bsisa >= field.BQty)).ToList();

                        //      if (varListRnd1.Count > 0)
                        //      {
                        //        rnd1 = (from q in db.LG_RND1s
                        //                where q.c_gdg == lisAdjStock2[nLoop].c_gdg &&
                        //                q.c_rnno == lisAdjStock2[nLoop].c_rnno &&
                        //                q.c_iteno == lisAdjStock2[nLoop].c_iteno &&
                        //                q.c_batch == lisAdjStock2[nLoop].c_batch
                        //                select q).Take(1).SingleOrDefault();

                        //        rnd1.n_gsisa -= field.GQty;
                        //        rnd1.n_bsisa -= field.BQty;

                        //      }
                        //      else
                        //      {
                        //        db.Transaction.Rollback();
                        //      }
                        //    }
                        //  }
                        #endregion old coded

                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            lisAdjStock1 = new List<LG_AdjustD1>();
                            lisAdjStock2 = new List<LG_AdjustD2>();
                            varLisRnd1 = new List<LG_RND1>();
                            rnd1 = new LG_RND1();
                            AdjTemp = new AdjDetailGenerator();
                            listAdjTemp = new List<AdjDetailGenerator>();
                            dicItemStock = new Dictionary<string, List<AdjDetailGenerator>>();

                            listRnh = new List<AdjDetailGenerator>();
                            rnh = new LG_RNH();

                            structure.Fields.Field.OrderBy(x => x.Item).ToList();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                field = structure.Fields.Field[nLoop];

                                if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified))
                                {

                                    if (dicItemStock.ContainsKey(field.Item))
                                    {
                                        listAdjTemp = dicItemStock[field.Item];
                                    }
                                    else
                                    {

                                        listAdjTemp = (from q in db.LG_RNHs
                                                       join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                                       where ((q.c_type != "06") || ((q.l_khusus.HasValue ? q.l_khusus.Value : false) == false))
                                                         && (q1.c_iteno == (string.IsNullOrEmpty(field.Item) ? q1.c_iteno : field.Item))
                                                         && ((gdg == '0' ? q.c_gdg : gdg) == q.c_gdg)
                                                         && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                                       select new AdjDetailGenerator()
                                                       {
                                                           Item = q1.c_iteno,
                                                           BatchID = q1.c_batch,
                                                           SignID = "RN",
                                                           RefID = q1.c_rnno,
                                                           n_gsisa = (q1.n_gsisa.HasValue ? q1.n_gsisa.Value : 0),
                                                           n_bsisa = (q1.n_bsisa.HasValue ? q1.n_bsisa.Value : 0)
                                                       }).Union(
                                                      from q in db.LG_ComboHs
                                                      where ((gdg == '0' ? q.c_gdg : gdg) == q.c_gdg)
                                                        && q.c_iteno == field.Item
                                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                                      select new AdjDetailGenerator()
                                                      {
                                                          SignID = "CB",
                                                          RefID = q.c_combono,
                                                          Item = q.c_iteno,
                                                          BatchID = q.c_batch,
                                                          n_gsisa = (q.n_gsisa.HasValue ? q.n_gsisa.Value : 0),
                                                          n_bsisa = (q.n_bsisa.HasValue ? q.n_bsisa.Value : 0)
                                                      }).Distinct().ToList();

                                        dicItemStock.Add(field.Item, listAdjTemp);
                                    }

                                    totalCurrentStockGood = listAdjTemp.Sum(t => t.n_gsisa);
                                    totalCurrentStockBad = listAdjTemp.Sum(t => t.n_bsisa);

                                    if (field.GQty < 0)
                                    {
                                        if (totalCurrentStockGood < (field.GQty * -1))
                                        {
                                            continue;
                                        }
                                    }
                                    //else  Modified 11 juni 2015
                                    //{
                                    //    if (totalCurrentStockGood < (field.GQty))
                                    //    {
                                    //        continue;
                                    //    }
                                    //}
                                    if (field.BQty < 0)
                                    {
                                        if (totalCurrentStockBad < (field.BQty * -1))
                                        {
                                            continue;
                                        }
                                    }
                                    //else   Modified 11 juni 2015
                                    //{
                                    //    if (totalCurrentStockBad < (field.BQty))
                                    //    {
                                    //        continue;
                                    //    }
                                    //}

                                    if (listAdjTemp.Count > 0)
                                    {
                                        lisAdjStock1.Add(new LG_AdjustD1()
                                        {
                                            c_adjno = adjID,
                                            c_batch = field.Batch,
                                            c_gdg = gdg,
                                            c_iteno = field.Item,
                                            n_bqty = field.BQty,
                                            n_gqty = field.GQty,
                                            v_ket = field.KetDet
                                        });

                                        istSPAC = listAdjTemp.FindAll(delegate(AdjDetailGenerator adj)
                                        {
                                            if (string.IsNullOrEmpty(adj.BatchID.Trim()))
                                            {
                                                return false;
                                            }

                                            if (!adj.BatchID.Trim().Equals(field.Batch.Trim(), StringComparison.OrdinalIgnoreCase))
                                            {
                                                return false;
                                            }

                                            return true;
                                        });

                                        nQtyAllocGood = field.GQty;
                                        nQtyAllocBad = field.BQty;

                                        #region New Stock Indra 20180305FM

                                        if ((SavingStock.DailyStock(db, gdg.ToString(),
                                                                        adjID,
                                                                        adjH.c_type,
                                                                        field.Item,
                                                                        field.Batch,
                                                                        nQtyAllocGood,
                                                                        nQtyAllocBad,
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

                                        if (istSPAC.Count > 0)
                                        {
                                            for (nLoopC = 0; nLoopC < istSPAC.Count; nLoopC++)
                                            {
                                                AdjTemp = istSPAC[nLoopC];

                                                //GAqum = AdjTemp.n_gsisa + nQtyAllocGood;

                                                gE = 0;
                                                bE = 0;

                                                if (nQtyAllocGood != 0)
                                                {
                                                    if (nQtyAllocGood < 0)
                                                    {
                                                        //if (GAqum >= 0)   Modified 18 juni 2015
                                                        //{
                                                            gE = (Math.Abs(nQtyAllocGood) > AdjTemp.n_gsisa ? AdjTemp.n_gsisa : Math.Abs(nQtyAllocGood));

                                                            gE *= -1;
                                                        //}
                                                    }
                                                    else
                                                    {
                                                        gE = nQtyAllocGood;
                                                    }
                                                }
                                                if (nQtyAllocBad != 0)
                                                {
                                                    if (nQtyAllocBad < 0)
                                                    {
                                                        //if (BAqum >= 0)   Modified 11 juni 2015
                                                        //{
                                                            bE = (Math.Abs(nQtyAllocBad) > AdjTemp.n_bsisa ? AdjTemp.n_bsisa : Math.Abs(nQtyAllocBad));

                                                            bE *= -1;
                                                        //}
                                                    }
                                                    else
                                                    {
                                                        bE = nQtyAllocBad;
                                                    }
                                                }
                                                if (gE != 0 || bE != 0)
                                                {
                                                    lisAdjStock2.Add(new LG_AdjustD2()
                                                    {
                                                        c_adjno = adjID,
                                                        c_batch = field.Batch,
                                                        c_gdg = gdg,
                                                        c_iteno = field.Item,
                                                        c_rnno = AdjTemp.RefID,
                                                        n_bqty = bE,
                                                        n_gqty = gE,
                                                    });

                                                    if (AdjTemp.SignID.Equals("RN") || AdjTemp.SignID.Equals("RR"))
                                                    {
                                                        rnd1 = (from q in db.LG_RND1s
                                                                where q.c_gdg == gdg && q.c_iteno == field.Item
                                                                && q.c_rnno == AdjTemp.RefID && q.c_batch == field.Batch
                                                                select q).Take(1).SingleOrDefault();
                                                        if (rnd1 != null)
                                                        {
                                                            rnd1.n_bsisa += bE;
                                                            rnd1.n_gsisa += gE;

                                                            nQtyAllocGood -= gE;
                                                            nQtyAllocBad -= bE;

                                                            AdjTemp.n_bsisa += bE;
                                                            AdjTemp.n_gsisa += gE;

                                                            totalDetails++;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        comboh = (from q in db.LG_ComboHs
                                                                  where q.c_gdg == gdg && q.c_iteno == field.Item
                                                                  && q.c_combono == AdjTemp.RefID && q.c_batch == field.Batch
                                                                  select q).Take(1).SingleOrDefault();
                                                        if (comboh != null)
                                                        {
                                                            comboh.n_bsisa += bE;
                                                            comboh.n_gsisa += gE;

                                                            nQtyAllocGood -= gE;
                                                            nQtyAllocBad -= bE;

                                                            AdjTemp.n_bsisa += bE;
                                                            AdjTemp.n_gsisa += gE;

                                                            totalDetails++;
                                                        }
                                                    }
                                                }
                                            }

                                            if (nQtyAllocGood != 0 || nQtyAllocBad != 0)
                                            {
                                                result = "Kode barang " + AdjTemp.Item + " stocknya tidak cukup atau sudah terpakai";

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
                                            suplId = (from q in db.FA_MasItms
                                                      where (q.c_iteno == field.Item)
                                                      select (q.c_nosup == null ? string.Empty : q.c_nosup.Trim())).Take(1).SingleOrDefault();

                                            rnID = string.Concat("RNXXD", (suplId ?? "ADJST"));

                                            rnd1 = (from q in db.LG_RND1s
                                                    where (q.c_gdg == gdg) && (q.c_rnno == rnID)
                                                    && (q.c_iteno == field.Item) && q.c_batch == field.Batch
                                                    select q).Take(1).SingleOrDefault();

                                            if (rnd1 == null)
                                            {
                                                rnd1 = new LG_RND1()
                                                {
                                                    c_gdg = gdg,
                                                    c_rnno = rnID,
                                                    c_iteno = field.Item,
                                                    c_batch = field.Batch,
                                                    n_bqty = 0,
                                                    n_bsisa = field.BQty,
                                                    n_gqty = 0,
                                                    n_gsisa = field.GQty
                                                };

                                                db.LG_RND1s.InsertOnSubmit(rnd1);

                                                const string MSBATCHITM = "MSBATCHITM";

                                                rnh = (from q in db.LG_RNHs
                                                       where (q.c_gdg == gdg) && (q.c_rnno == rnID)
                                                        && (q.c_dono == MSBATCHITM)
                                                       select q).Take(1).SingleOrDefault();

                                                listRnh = listAdjTemp.FindAll(delegate(AdjDetailGenerator adj)
                                                {
                                                    return adj.RefID.Equals(rnID, StringComparison.OrdinalIgnoreCase);
                                                });

                                                if (listRnh.Count == 0 && rnh == null)
                                                {
                                                    rnh = new LG_RNH()
                                                    {
                                                        c_gdg = gdg,
                                                        c_rnno = rnID,
                                                        d_rndate = date,
                                                        c_type = "07",
                                                        l_float = false,
                                                        c_dono = MSBATCHITM,
                                                        d_dodate = date,
                                                        v_ket = "Auto Create From Adjust Batch",
                                                        c_from = suplId,
                                                        n_bea = 0,
                                                        l_print = false,
                                                        l_status = false,
                                                        c_entry = nipEntry,
                                                        d_entry = date,
                                                        c_update = nipEntry,
                                                        d_update = date,
                                                        l_delete = false,
                                                        v_ket_mark = string.Empty
                                                    };

                                                    dicItemStock[field.Item].Add(new AdjDetailGenerator()
                                                    {
                                                        SignID = "RN",
                                                        RefID = rnID,
                                                        Item = field.Item,
                                                        BatchID = field.Batch,
                                                        n_gsisa = field.GQty,
                                                        n_bsisa = field.BQty
                                                    });

                                                    db.LG_RNHs.InsertOnSubmit(rnh);
                                                }

                                                lisAdjStock2.Add(new LG_AdjustD2()
                                                {
                                                    c_adjno = adjID,
                                                    c_batch = field.Batch,
                                                    c_gdg = gdg,
                                                    c_iteno = field.Item,
                                                    c_rnno = rnID,
                                                    n_bqty = field.BQty,
                                                    n_gqty = field.GQty
                                                });

                                                totalDetails++;
                                                continue;
                                            }
                                            else
                                            {
                                                result = "Kode barang " + AdjTemp.Item + " stocknya tidak cukup atau sudah terpakai!";

                                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                                if (db.Transaction != null)
                                                {
                                                    db.Transaction.Rollback();
                                                }

                                                goto endLogic;
                                            }
                                        }
                                    }

                                    if (totalDetails > 0)
                                    {
                                        if ((lisAdjStock1 != null) && (lisAdjStock1.Count > 0))
                                        {
                                            db.LG_AdjustD1s.InsertAllOnSubmit(lisAdjStock1.ToArray());
                                            lisAdjStock1.Clear();
                                        }
                                        if ((lisAdjStock2 != null) && (lisAdjStock2.Count > 0))
                                        {
                                            db.LG_AdjustD2s.InsertAllOnSubmit(lisAdjStock2.ToArray());
                                            lisAdjStock2.Clear();
                                        }

                                        hasAnyChanges = true;
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region Type 03 (Stock)

                    if (structure.Fields.Type.Equals("03", StringComparison.OrdinalIgnoreCase))
                    {
                        #region old coded
                        //  lisAdjStock3 = new List<LG_AdjustD3>();
                        //  lisAdjStock1 = new List<LG_AdjustD1>();
                        //  lisAdjStock2 = new List<LG_AdjustD2>();
                        //  rnd1 = new LG_RND1();
                        //  if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        //  {
                        //    field = structure.Fields.Field[nLoop];

                        //    lisAdjStock2 = (from q in db.LG_AdjustD2s
                        //                    where q.c_gdg == gdg && q.c_adjno == adjID &&
                        //                    q.c_iteno == field.Item && q.c_batch == field.Batch
                        //                    select q).ToList();

                        //    for (nLoop = 0; nLoop < lisAdjStock1.Count; nLoop ++ )
                        //    {
                        //      var varListRnd1 = (from q in db.LG_RND1s
                        //                      where q.c_gdg == lisAdjStock2[nLoop].c_gdg &&
                        //                      q.c_rnno == lisAdjStock2[nLoop].c_rnno &&
                        //                      q.c_iteno == lisAdjStock2[nLoop].c_iteno &&
                        //                      q.c_batch == lisAdjStock2[nLoop].c_batch
                        //                      select q).Where(x => (x.n_gsisa.Value >= field.GQty) && (x.n_bsisa >= field.BQty)).ToList();
                        //      if (varListRnd1.Count > 0)
                        //      {
                        //        rnd1 = (from q in db.LG_RND1s
                        //                where q.c_gdg == lisAdjStock2[nLoop].c_gdg &&
                        //                q.c_rnno == lisAdjStock2[nLoop].c_rnno &&
                        //                q.c_iteno == lisAdjStock2[nLoop].c_iteno &&
                        //                q.c_batch == lisAdjStock2[nLoop].c_batch
                        //                select q).Take(1).SingleOrDefault();

                        //        rnd1.n_gsisa -= field.GQty;
                        //        rnd1.n_bsisa -= field.BQty;

                        //      }
                        //      else
                        //      {
                        //        result = "Tidak dapat mengubah hapus untuk batch " + lisAdjStock2[nLoop].c_batch + " yang sudah terhapus.";

                        //        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        //        db.Transaction.Rollback();
                        //      }

                        //    }
                        //  }
                        #endregion old coded

                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            lisAdjStock3 = new List<LG_AdjustD3>();
                            lisAdjStock1 = new List<LG_AdjustD1>();
                            lisAdjStock2 = new List<LG_AdjustD2>();
                            rnd1 = new LG_RND1();
                            dicItemStock = new Dictionary<string, List<AdjDetailGenerator>>();
                            listAdjTemp = new List<AdjDetailGenerator>();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                field = structure.Fields.Field[nLoop];

                                if ((field != null) && (field.IsNew) && (!field.IsDelete) && (!field.IsModified))
                                {
                                    #region New Indra 20171120

                                    nQtyAllocGood = field.GQty;
                                    nQtyAllocBad = field.BQty;

                                    if (dicItemStock.ContainsKey(field.Item))
                                    {
                                        listAdjTemp = dicItemStock[field.Item];
                                    }
                                    else
                                    {
                                        listAdjTemp = (from q in GlobalQuery.ViewStockLiteAdjContains(db, gdg, field.Item)
                                                       join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                                       join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                                       //where (q.n_gsisa != 0) || (q.n_bsisa != 0) <-- Modified 9 jun 2015
                                                       //group q by new { q.c_iteno } into g
                                                       //where (g.Sum(x => x.n_gsisa) >= field.GQty) && (g.Sum(x => x.n_bsisa) >= field.BQty)
                                                       select new AdjDetailGenerator()
                                                       {
                                                           RefID = q.c_no,
                                                           SignID = (q.c_table == null ? string.Empty : q.c_table.Trim()),
                                                           Item = q.c_iteno,
                                                           n_gsisa = q.n_gsisa,
                                                           n_bsisa = q.n_bsisa,
                                                           Supplier = q1.c_nosup,
                                                           BatchID = q.c_batch.Trim(),
                                                           SumGood = 0
                                                       }).ToList();

                                        dicItemStock.Add(field.Item, listAdjTemp);

                                    }

                                    totalCurrentStockGood = listAdjTemp.Sum(t => t.n_gsisa);
                                    totalCurrentStockBad = listAdjTemp.Sum(t => t.n_bsisa);

                                    if (field.GQty < 0)
                                    {
                                        if (totalCurrentStockGood < (field.GQty * -1))
                                        {
                                            continue;
                                        }
                                    }
                                    if (field.BQty < 0)
                                    {
                                        if (totalCurrentStockBad < (field.BQty * -1))
                                        {
                                            continue;
                                        }
                                    }

                                    if (listAdjTemp.Count > 0)
                                    {
                                        lisAdjStock1.Add(new LG_AdjustD1()
                                        {
                                            c_adjno = adjID,
                                            c_batch = field.Batch,
                                            c_gdg = gdg,
                                            c_iteno = field.Item,
                                            n_bqty = field.BQty,
                                            n_gqty = field.GQty,
                                            v_ket = field.KetDet
                                        });

                                        istSPAC = listAdjTemp.FindAll(delegate(AdjDetailGenerator adj)
                                        {
                                            if (!adj.BatchID.Trim().Equals(field.Batch.Trim(), StringComparison.OrdinalIgnoreCase))
                                            {
                                                return false;
                                            }

                                            return true;
                                        });

                                        nQtyAllocBad = field.BQty;
                                        nQtyAllocGood = field.GQty;

                                        #region New Stock Indra 20180305FM

                                        if ((SavingStock.DailyStock(db, gdg.ToString(),
                                                                        adjID,
                                                                        adjH.c_type,
                                                                        field.Item,
                                                                        field.Batch,
                                                                        nQtyAllocGood,
                                                                        nQtyAllocBad,
                                                                        "KOSONG",
                                                                        "03",
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

                                        if ((istSPAC != null) && (istSPAC.Count > 0))
                                        {
                                            for (nLoopC = 0; nLoopC < istSPAC.Count; nLoopC++)
                                            {
                                                AdjTemp = istSPAC[nLoopC];

                                                if (nQtyAllocGood != 0 || nQtyAllocBad != 0)
                                                {
                                                    if (AdjTemp.SignID.Equals("RN") || AdjTemp.SignID.Equals("RR"))
                                                    {
                                                        rnd1 = (from q in db.LG_RND1s
                                                                where q.c_gdg == gdg && q.c_rnno == AdjTemp.RefID
                                                                && q.c_iteno == field.Item && q.c_batch == field.Batch
                                                                select q).Take(1).SingleOrDefault();

                                                        if (rnd1 != null)
                                                        {
                                                            if (nQtyAllocGood > 0)
                                                            {
                                                                nGQtyReloc = nQtyAllocGood;
                                                            }
                                                            else
                                                            {
                                                                nGQtyReloc = (Math.Abs(nQtyAllocGood) > AdjTemp.n_gsisa ? AdjTemp.n_gsisa : Math.Abs(nQtyAllocGood));

                                                                nGQtyReloc *= -1;
                                                            }

                                                            if (nQtyAllocBad > 0)
                                                            {
                                                                nBQtyReloc = nQtyAllocBad;
                                                            }
                                                            else
                                                            {
                                                                nBQtyReloc = (Math.Abs(nQtyAllocBad) > AdjTemp.n_bsisa ? AdjTemp.n_bsisa : Math.Abs(nQtyAllocBad));

                                                                nBQtyReloc *= -1;
                                                            }

                                                            if ((nBQtyReloc != 0) || (nGQtyReloc != 0))
                                                            {

                                                                rnd1.n_bsisa += nBQtyReloc;
                                                                rnd1.n_gsisa += nGQtyReloc;

                                                                AdjTemp.n_bsisa += nBQtyReloc;
                                                                AdjTemp.n_gsisa += nGQtyReloc;

                                                                nQtyAllocGood -= nGQtyReloc;
                                                                nQtyAllocBad -= nBQtyReloc;

                                                                lisAdjStock2.Add(new LG_AdjustD2()
                                                                {
                                                                    c_adjno = adjID,
                                                                    c_batch = field.Batch,
                                                                    c_gdg = gdg,
                                                                    c_iteno = field.Item,
                                                                    c_rnno = rnd1.c_rnno,
                                                                    n_bqty = nBQtyReloc,
                                                                    n_gqty = nGQtyReloc,
                                                                });

                                                                totalDetails++;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        comboh = (from q in db.LG_ComboHs
                                                                  where q.c_gdg == gdg && q.c_combono == AdjTemp.RefID
                                                                  && q.c_iteno == field.Item && q.c_batch == field.Batch
                                                                  select q).Take(1).SingleOrDefault();

                                                        if (comboh != null)
                                                        {
                                                            if (nQtyAllocGood > 0)
                                                            {
                                                                nGQtyReloc = nQtyAllocGood;
                                                            }
                                                            else
                                                            {
                                                                nGQtyReloc = (Math.Abs(nQtyAllocGood) > AdjTemp.n_gsisa ? AdjTemp.n_gsisa : Math.Abs(nQtyAllocGood));

                                                                nGQtyReloc *= -1;
                                                            }

                                                            if (nQtyAllocBad > 0)
                                                            {
                                                                nBQtyReloc = nQtyAllocBad;
                                                            }
                                                            else
                                                            {
                                                                nBQtyReloc = (Math.Abs(nQtyAllocBad) > AdjTemp.n_bsisa ? AdjTemp.n_bsisa : Math.Abs(nQtyAllocBad));

                                                                nBQtyReloc *= -1;
                                                            }

                                                            if ((nBQtyReloc != 0) || (nGQtyReloc != 0))
                                                            {

                                                                comboh.n_bsisa += nBQtyReloc;
                                                                comboh.n_gsisa += nGQtyReloc;

                                                                nQtyAllocGood -= nGQtyReloc;
                                                                nQtyAllocBad -= nBQtyReloc;

                                                                lisAdjStock2.Add(new LG_AdjustD2()
                                                                {
                                                                    c_adjno = adjID,
                                                                    c_batch = field.Batch,
                                                                    c_gdg = gdg,
                                                                    c_iteno = field.Item,
                                                                    c_rnno = comboh.c_combono,
                                                                    n_bqty = nBQtyReloc,
                                                                    n_gqty = nGQtyReloc,
                                                                });

                                                                totalDetails++;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            suplId = (from q in db.FA_MasItms
                                                      where (q.c_iteno == field.Item)
                                                      select (q.c_nosup == null ? string.Empty : q.c_nosup.Trim())).Take(1).SingleOrDefault();

                                            rnID = string.Concat("RNXXD", (suplId ?? "ADJST"));

                                            rnd1 = (from q in db.LG_RND1s
                                                    where (q.c_gdg == gdg) && (q.c_rnno == rnID)
                                                    && (q.c_iteno == field.Item) && q.c_batch == field.Batch
                                                    select q).Take(1).SingleOrDefault();

                                            if (rnd1 == null)
                                            {
                                                rnd1 = new LG_RND1()
                                                {
                                                    c_gdg = gdg,
                                                    c_rnno = rnID,
                                                    c_iteno = field.Item,
                                                    c_batch = field.Batch,
                                                    n_bqty = 0,
                                                    n_bsisa = field.BQty,
                                                    n_gqty = 0,
                                                    n_gsisa = field.GQty
                                                };

                                                db.LG_RND1s.InsertOnSubmit(rnd1);

                                                const string MSBATCHITM = "MSBATCHITM";

                                                rnh = (from q in db.LG_RNHs
                                                       where (q.c_gdg == gdg) && (q.c_rnno == rnID)
                                                        && (q.c_dono == MSBATCHITM)
                                                       select q).Take(1).SingleOrDefault();

                                                listRnh = listAdjTemp.FindAll(delegate(AdjDetailGenerator adj)
                                                {
                                                    return adj.RefID.Equals(rnID, StringComparison.OrdinalIgnoreCase);
                                                });

                                                if (listRnh.Count == 0 && rnh == null)
                                                {
                                                    rnh = new LG_RNH()
                                                    {
                                                        c_gdg = gdg,
                                                        c_rnno = rnID,
                                                        d_rndate = date,
                                                        c_type = "07",
                                                        l_float = false,
                                                        c_dono = MSBATCHITM,
                                                        d_dodate = date,
                                                        v_ket = "Auto Create From Adjust Stock",
                                                        c_from = suplId,
                                                        n_bea = 0,
                                                        l_print = false,
                                                        l_status = false,
                                                        c_entry = nipEntry,
                                                        d_entry = date,
                                                        c_update = nipEntry,
                                                        d_update = date,
                                                        l_delete = false,
                                                        v_ket_mark = string.Empty
                                                    };

                                                    dicItemStock[field.Item].Add(new AdjDetailGenerator()
                                                    {
                                                        SignID = "RN",
                                                        RefID = rnID,
                                                        Item = field.Item,
                                                        BatchID = field.Batch,
                                                        n_gsisa = field.GQty,
                                                        n_bsisa = field.BQty
                                                    });

                                                    db.LG_RNHs.InsertOnSubmit(rnh);
                                                }

                                                lisAdjStock2.Add(new LG_AdjustD2()
                                                {
                                                    c_adjno = adjID,
                                                    c_batch = field.Batch,
                                                    c_gdg = gdg,
                                                    c_iteno = field.Item,
                                                    c_rnno = rnID,
                                                    n_bqty = field.BQty,
                                                    n_gqty = field.GQty
                                                });

                                                totalDetails++;
                                                continue;
                                            }
                                            else
                                            {
                                                result = "Kode barang " + field.Item + " stocknya tidak cukup atau sudah terpakai!";

                                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                                if (db.Transaction != null)
                                                {
                                                    db.Transaction.Rollback();
                                                }

                                                goto endLogic;
                                            }
                                        }
                                    }

                                    if (totalDetails > 0)
                                    {
                                        if ((lisAdjStock1 != null) && (lisAdjStock1.Count > 0))
                                        {
                                            db.LG_AdjustD1s.InsertAllOnSubmit(lisAdjStock1.ToArray());
                                            lisAdjStock1.Clear();
                                        }
                                        if ((lisAdjStock2 != null) && (lisAdjStock2.Count > 0))
                                        {
                                            db.LG_AdjustD2s.InsertAllOnSubmit(lisAdjStock2.ToArray());
                                            lisAdjStock2.Clear();
                                        }

                                        hasAnyChanges = true;
                                    }

                                    #endregion
                                }
                                else if ((field != null) && (!field.IsNew) && (field.IsDelete) && (!field.IsModified))
                                {
                                    #region Delete

                                    nQtyAllocBad = field.BQty;
                                    nQtyAllocGood = field.GQty;

                                    lisAdjStock2 = (from q in db.LG_AdjustD2s
                                                    where q.c_adjno == adjID && q.c_batch == field.Batch
                                                    && q.c_gdg == gdg && q.c_iteno == field.Item
                                                    select q).ToList();

                                    lisAdjStock1 = (from q in db.LG_AdjustD1s
                                                    where q.c_adjno == adjID && q.c_batch == field.Batch
                                                    && q.c_gdg == gdg && q.c_iteno == field.Item
                                                    select q).ToList();

                                    if (dicItemStock.ContainsKey(field.Item))
                                    {
                                        listAdjTemp = dicItemStock[field.Item];
                                    }
                                    else
                                    {
                                        listAdjTemp = (from q in GlobalQuery.ViewStockLite(db, gdg, field.Item)
                                                       join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                                       join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                                       where (q.n_gsisa != 0) || (q.n_bsisa != 0)
                                                       select new AdjDetailGenerator()
                                                       {
                                                           RefID = q.c_no,
                                                           SignID = (q.c_table == null ? string.Empty : q.c_table.Trim()),
                                                           Item = q.c_iteno,
                                                           n_gsisa = q.n_gsisa,
                                                           n_bsisa = q.n_bsisa,
                                                           Supplier = q1.c_nosup,
                                                           BatchID = q.c_batch.Trim(),
                                                           SumGood = 0
                                                       }).ToList();

                                        dicItemStock.Add(field.Item, listAdjTemp);
                                    }

                                    totalCurrentStockGood = listAdjTemp.Sum(t => t.n_gsisa);
                                    totalCurrentStockBad = listAdjTemp.Sum(t => t.n_bsisa);

                                    StockPerBatchGood = listAdjTemp.Where(x => x.BatchID.Trim() == field.Batch.Trim()).GroupBy(x => x.BatchID).Sum(y => y.Sum(z => z.n_gsisa));
                                    StockPerBatchBad = listAdjTemp.Where(x => x.BatchID.Trim() == field.Batch.Trim()).GroupBy(x => x.BatchID).Sum(y => y.Sum(z => z.n_bsisa));

                                    // Cek Stock Per Item
                                    if (field.GQty > 0)
                                    {
                                        if (totalCurrentStockGood < (field.GQty))
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            //Cek Stock Per Batch
                                            if (StockPerBatchGood < (field.GQty))
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                    if (field.BQty > 0)
                                    {
                                        if (totalCurrentStockBad < (field.BQty))
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            //Cek Stock Per Batch
                                            if (StockPerBatchBad < (field.BQty))
                                            {
                                                continue;
                                            }
                                        }
                                    }

                                    if ((lisAdjStock1 != null) && (lisAdjStock2 != null))
                                    {
                                        if (lisAdjStock1.Count > 0 && lisAdjStock2.Count > 0)
                                        {
                                            nQtyAllocGood = field.GQty;
                                            nQtyAllocBad = field.BQty;

                                            #region New Stock Indra 20180305FM

                                            if ((SavingStock.DailyStock(db, gdg.ToString(),
                                                                        adjID,
                                                                        adjH.c_type,
                                                                        field.Item,
                                                                        field.Batch,
                                                                        nQtyAllocGood * -1,
                                                                        nQtyAllocBad * -1,
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

                                            adjD1 = lisAdjStock1.Find(delegate(LG_AdjustD1 adj)
                                            {
                                                return (
                                                  field.Batch.Trim().Equals(adj.c_batch.Trim(), StringComparison.OrdinalIgnoreCase) &&
                                                  field.Item.Trim().Equals(adj.c_iteno.Trim(), StringComparison.OrdinalIgnoreCase) &&
                                                  adjID.Equals(adj.c_adjno.Trim(), StringComparison.OrdinalIgnoreCase));
                                            });

                                            if (adjD1 != null)
                                            {
                                                db.LG_AdjustD1s.DeleteOnSubmit(adjD1);
                                            }
                                            for (nLoopC = 0; nLoopC < lisAdjStock2.Count; nLoopC++)
                                            {
                                                adjD2 = lisAdjStock2[nLoopC];

                                                lisAdjStock3.Add(new LG_AdjustD3()
                                                {
                                                    c_adjno = adjID,
                                                    c_batch = adjD2.c_batch,
                                                    c_iteno = adjD2.c_iteno,
                                                    c_entry = structure.Fields.Entry,
                                                    d_entry = date,
                                                    n_bqty = adjD2.n_bqty,
                                                    n_gqty = adjD2.n_gqty,
                                                    v_ket_del = field.Keterangan,
                                                    v_type = "02"
                                                });

                                                if (adjD2.c_rnno.StartsWith("RN") || adjD2.c_rnno.StartsWith("RR"))
                                                {
                                                    if (adjD2.n_bqty > 0 || adjD2.n_gqty > 0)
                                                    {
                                                        rnd1 = (from q in db.LG_RND1s
                                                                where q.c_batch.Trim() == adjD2.c_batch.Trim()
                                                                && q.c_gdg == adjD2.c_gdg && q.c_iteno == adjD2.c_iteno
                                                                && q.c_rnno == adjD2.c_rnno && q.n_bsisa >= (adjD2.n_bqty.HasValue ? adjD2.n_bqty.Value : 0)
                                                                && q.n_gsisa >= (adjD2.n_gqty.HasValue ? adjD2.n_gqty.Value : 0)
                                                                select q).Take(1).SingleOrDefault();
                                                    }
                                                    else
                                                    {
                                                        rnd1 = (from q in db.LG_RND1s
                                                                where q.c_batch.Trim() == adjD2.c_batch.Trim()
                                                                && q.c_gdg == adjD2.c_gdg && q.c_iteno == adjD2.c_iteno
                                                                && q.c_rnno == adjD2.c_rnno
                                                                select q).Take(1).SingleOrDefault();
                                                    }

                                                    if (rnd1 != null)
                                                    {
                                                        rnd1.n_gsisa -= adjD2.n_gqty;
                                                        rnd1.n_bsisa -= adjD2.n_bqty;

                                                        nQtyAllocGood -= (adjD2.n_gqty.HasValue ? adjD2.n_gqty.Value : 0);
                                                        nQtyAllocBad -= (adjD2.n_bqty.HasValue ? adjD2.n_bqty.Value : 0);

                                                        db.LG_AdjustD2s.DeleteOnSubmit(adjD2);

                                                        totalDetails++;
                                                    }
                                                    else
                                                    {
                                                        result = "Adjustment tidak dapat diubah, Pengecekan Stock Gagal.";

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
                                                    if (adjD2.n_bqty > 0 || adjD2.n_gqty > 0)
                                                    {
                                                        comboh = (from q in db.LG_ComboHs
                                                                  where q.c_batch.Trim() == adjD2.c_batch.Trim()
                                                                  && q.c_gdg == adjD2.c_gdg && q.c_iteno == adjD2.c_iteno
                                                                  && q.c_combono == adjD2.c_rnno && q.n_bsisa >= (adjD2.n_bqty.HasValue ? adjD2.n_bqty.Value : 0)
                                                                && q.n_gsisa >= (adjD2.n_gqty.HasValue ? adjD2.n_gqty.Value : 0)
                                                                  select q).Take(1).SingleOrDefault();
                                                    }
                                                    else
                                                    {
                                                        comboh = (from q in db.LG_ComboHs
                                                                  where q.c_batch.Trim() == adjD2.c_batch.Trim()
                                                                  && q.c_gdg == adjD2.c_gdg && q.c_iteno == adjD2.c_iteno
                                                                  && q.c_combono == adjD2.c_rnno
                                                                  select q).Take(1).SingleOrDefault();
                                                    }

                                                    if (comboh != null)
                                                    {
                                                        comboh.n_gsisa -= adjD2.n_gqty;
                                                        comboh.n_bsisa -= adjD2.n_bqty;

                                                        nQtyAllocGood -= (adjD2.n_gqty.HasValue ? adjD2.n_gqty.Value : 0);
                                                        nQtyAllocBad -= (adjD2.n_bqty.HasValue ? adjD2.n_bqty.Value : 0);

                                                        db.LG_AdjustD2s.DeleteOnSubmit(adjD2);

                                                        totalDetails++;
                                                    }
                                                    else
                                                    {
                                                        result = "Adjustment tidak dapat diubah, Pengecekan Stock Gagal.";

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

                                    if (totalDetails > 0)
                                    {
                                        lisAdjStock1.Clear();
                                        lisAdjStock2.Clear();

                                        if (lisAdjStock3.Count > 0 && lisAdjStock3 != null)
                                        {
                                            db.LG_AdjustD3s.InsertAllOnSubmit(lisAdjStock3.ToArray());
                                            lisAdjStock3.Clear();
                                        }

                                        hasAnyChanges = true;
                                    }

                                    #endregion
                                }
                            }
                        }
                    }

                    #endregion

                    #endregion
                }                    
                if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    #region Validasi Indra 20171120

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

                    adjH = (from q in db.LG_AdjustHs
                            where q.c_gdg == gdg && q.c_adjno == adjID && q.c_type == structure.Fields.Type
                            select q).Take(1).SingleOrDefault();

                    if (adjH == null)
                    {
                        result = "Nomor Adjustment tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (adjH.l_delete.HasValue && adjH.l_delete.Value)
                    {
                        result = "Tidak dapat menghapus Nomor Adjustment yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, adjH.d_adjdate))
                    {
                        result = "Adjustment tidak dapat dihapus, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    #endregion

                    adjH.c_update = nipEntry;
                    adjH.d_update = DateTime.Now;

                    adjH.l_delete = true;
                    adjH.v_ket_mark = structure.Fields.Keterangan;

                    bool CheckRn = false;

                    lisAdjStock3 = new List<LG_AdjustD3>();
                    lisAdjStock1 = new List<LG_AdjustD1>();
                    lisAdjStock2 = new List<LG_AdjustD2>();

                    rnd1 = new LG_RND1();

                    lisAdjStock1 = (from q in db.LG_AdjustD1s
                                    where q.c_adjno == adjID
                                    && q.c_gdg == gdg
                                    select q).ToList();

                    #region Delete

                    if ((lisAdjStock1 != null) && (lisAdjStock1.Count > 0))
                    {
                        for (nLoopC = 0; nLoopC < lisAdjStock1.Count; nLoopC++)
                        {
                            adjD1 = lisAdjStock1[nLoopC];

                            nQtyAllocBad = (adjD1.n_bqty.HasValue ? adjD1.n_bqty.Value : 0);
                            nQtyAllocGood = (adjD1.n_gqty.HasValue ? adjD1.n_gqty.Value : 0);

                            lisAdjStock2 = (from q in db.LG_AdjustD2s
                                            where q.c_adjno == adjID && q.c_batch.Trim() == adjD1.c_batch.Trim()
                                            && q.c_gdg == gdg && q.c_iteno == adjD1.c_iteno.Trim()
                                            select q).ToList();

                            if (dicItemStock.ContainsKey(adjD1.c_iteno))
                            {
                                listAdjTemp = dicItemStock[adjD1.c_iteno];
                            }
                            else
                            {
                                listAdjTemp = (from q in GlobalQuery.ViewStockLite(db, gdg, adjD1.c_iteno)
                                               join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                               join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                               where (q.n_gsisa != 0) || (q.n_bsisa != 0)
                                               select new AdjDetailGenerator()
                                               {
                                                   RefID = q.c_no,
                                                   SignID = (q.c_table == null ? string.Empty : q.c_table.Trim()),
                                                   Item = q.c_iteno,
                                                   n_gsisa = q.n_gsisa,
                                                   n_bsisa = q.n_bsisa,
                                                   Supplier = q1.c_nosup,
                                                   BatchID = q.c_batch.Trim(),
                                                   SumGood = 0
                                               }).ToList();

                                dicItemStock.Add(adjD1.c_iteno, listAdjTemp);
                            }

                            totalCurrentStockGood = listAdjTemp.Sum(t => t.n_gsisa);
                            totalCurrentStockBad = listAdjTemp.Sum(t => t.n_bsisa);

                            StockPerBatchGood = listAdjTemp.Where(x => x.BatchID.Trim() == adjD1.c_batch.Trim()).GroupBy(x => x.BatchID).Sum(y => y.Sum(z => z.n_gsisa));
                            StockPerBatchBad = listAdjTemp.Where(x => x.BatchID.Trim() == adjD1.c_batch.Trim()).GroupBy(x => x.BatchID).Sum(y => y.Sum(z => z.n_bsisa));

                            // Cek Stock Per Item
                            if (adjD1.n_gqty > 0)
                            {
                                if (totalCurrentStockGood < (adjD1.n_gqty))
                                {
                                    continue;
                                }
                                else
                                {
                                    //Cek Stock Per Batch
                                    if (StockPerBatchGood < (adjD1.n_gqty))
                                    {
                                        continue;
                                    }
                                }
                            }
                            if (field.BQty > 0)
                            {
                                if (totalCurrentStockBad < (adjD1.n_bqty))
                                {
                                    continue;
                                }
                                else
                                {
                                    //Cek Stock Per Batch
                                    if (StockPerBatchBad < (adjD1.n_bqty))
                                    {
                                        continue;
                                    }
                                }
                            }

                            if ((lisAdjStock1 != null) && (lisAdjStock2 != null))
                            {
                                if (lisAdjStock1.Count > 0 && lisAdjStock2.Count > 0)
                                {
                                    nQtyAllocGood = (adjD1.n_gqty.HasValue ? adjD1.n_gqty.Value : 0);
                                    nQtyAllocBad = (adjD1.n_bqty.HasValue ? adjD1.n_bqty.Value : 0);

                                    #region New Stock Indra 20180305FM

                                    if ((SavingStock.DailyStock(db, gdg.ToString(),
                                                                    adjID,
                                                                    adjH.c_type,
                                                                    field.Item,
                                                                    field.Batch,
                                                                    nQtyAllocGood * -1,
                                                                    nQtyAllocBad * -1,
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

                                    adjD1 = lisAdjStock1.Find(delegate(LG_AdjustD1 adj)
                                    {
                                        return (adjD1.c_batch.Trim().Equals(adj.c_batch.Trim(), StringComparison.OrdinalIgnoreCase) &&
                                          adjD1.c_iteno.Trim().Equals(adj.c_iteno.Trim(), StringComparison.OrdinalIgnoreCase) &&
                                          adjID.Equals(adj.c_adjno.Trim(), StringComparison.OrdinalIgnoreCase));
                                    });

                                    if (adjD1 != null)
                                    {
                                        db.LG_AdjustD1s.DeleteOnSubmit(adjD1);
                                    }
                                    for (nLoop = 0; nLoop < lisAdjStock2.Count; nLoop++)
                                    {
                                        adjD2 = lisAdjStock2[nLoop];

                                        lisAdjStock3.Add(new LG_AdjustD3()
                                        {
                                            c_adjno = adjID,
                                            c_batch = adjD2.c_batch,
                                            c_iteno = adjD2.c_iteno,
                                            c_entry = structure.Fields.Entry,
                                            d_entry = date,
                                            n_bqty = adjD2.n_bqty,
                                            n_gqty = adjD2.n_gqty,
                                            v_ket_del = field.Keterangan,
                                            v_type = "02"
                                        });

                                        if (adjD2.c_rnno.StartsWith("RN") || adjD2.c_rnno.StartsWith("RR"))
                                        {
                                            if (adjD2.n_bqty > 0 || adjD2.n_gqty > 0)
                                            {
                                                rnd1 = (from q in db.LG_RND1s
                                                        where q.c_batch.Trim() == adjD2.c_batch.Trim()
                                                        && q.c_gdg == adjD2.c_gdg && q.c_iteno == adjD2.c_iteno
                                                        && q.c_rnno == adjD2.c_rnno && q.n_bsisa >= (adjD2.n_bqty.HasValue ? adjD2.n_bqty.Value : 0)
                                                        && q.n_gsisa >= (adjD2.n_gqty.HasValue ? adjD2.n_gqty.Value : 0)
                                                        select q).Take(1).SingleOrDefault();
                                            }
                                            else
                                            {
                                                rnd1 = (from q in db.LG_RND1s
                                                        where q.c_batch.Trim() == adjD2.c_batch.Trim()
                                                        && q.c_gdg == adjD2.c_gdg && q.c_iteno == adjD2.c_iteno
                                                        && q.c_rnno == adjD2.c_rnno
                                                        select q).Take(1).SingleOrDefault();
                                            }

                                            if (rnd1 != null)
                                            {
                                                rnd1.n_gsisa -= adjD2.n_gqty;
                                                rnd1.n_bsisa -= adjD2.n_bqty;

                                                nQtyAllocGood -= (adjD2.n_gqty.HasValue ? adjD2.n_gqty.Value : 0);
                                                nQtyAllocBad -= (adjD2.n_bqty.HasValue ? adjD2.n_bqty.Value : 0);

                                                db.LG_AdjustD2s.DeleteOnSubmit(adjD2);

                                                totalDetails++;
                                            }
                                            else
                                            {
                                                result = "Adjustment tidak dapat diubah, Pengecekan Stock Gagal.";

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
                                            if (adjD2.n_bqty > 0 || adjD2.n_gqty > 0)
                                            {
                                                comboh = (from q in db.LG_ComboHs
                                                          where q.c_batch.Trim() == adjD2.c_batch.Trim()
                                                          && q.c_gdg == adjD2.c_gdg && q.c_iteno == adjD2.c_iteno
                                                          && q.c_combono == adjD2.c_rnno && q.n_bsisa >= (adjD2.n_bqty.HasValue ? adjD2.n_bqty.Value : 0)
                                                        && q.n_gsisa >= (adjD2.n_gqty.HasValue ? adjD2.n_gqty.Value : 0)
                                                          select q).Take(1).SingleOrDefault();
                                            }
                                            else
                                            {
                                                comboh = (from q in db.LG_ComboHs
                                                          where q.c_batch.Trim() == adjD2.c_batch.Trim()
                                                          && q.c_gdg == adjD2.c_gdg && q.c_iteno == adjD2.c_iteno
                                                          && q.c_combono == adjD2.c_rnno
                                                          select q).Take(1).SingleOrDefault();
                                            }

                                            if (comboh != null)
                                            {
                                                comboh.n_gsisa -= adjD2.n_gqty;
                                                comboh.n_bsisa -= adjD2.n_bqty;

                                                nQtyAllocGood -= (adjD2.n_gqty.HasValue ? adjD2.n_gqty.Value : 0);
                                                nQtyAllocBad -= (adjD2.n_bqty.HasValue ? adjD2.n_bqty.Value : 0);

                                                db.LG_AdjustD2s.DeleteOnSubmit(adjD2);

                                                totalDetails++;
                                            }
                                            else
                                            {
                                                result = "Adjustment tidak dapat diubah, Pengecekan Stock Gagal.";

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

                        db.LG_AdjustD1s.DeleteAllOnSubmit(lisAdjStock1.ToArray());
                    }

                    if ((lisAdjStock3 != null) && (lisAdjStock3.Count > 0))
                    {
                        db.LG_AdjustD3s.InsertAllOnSubmit(lisAdjStock3.ToArray());
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
                }
            }
            catch (Exception ex)
            {
                if (db.Transaction != null)
                {
                    db.Transaction.Rollback();
                }

                result = string.Format("ScmsSoaLibrary.Bussiness.Adjustment:AdjustmentStock - {0}", ex.Message);

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

    class AdjustTransaksi
    {

        public string AdjustTrans(ScmsSoaLibrary.Parser.Class.AdjustTransStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false;
            string result = null;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            ScmsSoaLibrary.Parser.Class.AdjustTransStructureField field = null;
            string nipEntry = null;
            string adjID = null;
            //string tmpNumbering = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now;


            List<LG_AdjustTransD> lisAdjTrans = null;
            List<LG_AdjustTransD1> lisAdjTrans1 = null;
            LG_POD1 pod1 = null;

            LG_adjustTransH adjTransH = null;
            LG_AdjustTransD adjTransD = null;

            LG_RND1 rnd1 = null;

            int nLoop = 0,
              nLoopC = 0;

            //decimal
            //  nQtyAllocGood = 0,
            //  nQtyAllocBad = 0;

            IDictionary<string, string> dic = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            char gdg = (string.IsNullOrEmpty(structure.Fields.Gudang) ? char.MinValue : structure.Fields.Gudang[0]);

            if (gdg == char.MinValue)
            {
                result = "Gudang tidak boleh kosong.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            int totalDetails = 0;

            adjID = (structure.Fields.AdjustStockID ?? string.Empty);

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
                {
                    #region Add

                    if (string.IsNullOrEmpty(structure.Fields.Gudang))
                    {
                        result = "Gudang tidak boleh kosong.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, date))
                    {
                        result = "Adjustment tidak dapat disimpan, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    adjID = Commons.GenerateNumbering<LG_adjustTransH>(db, "AP", '3', "17", date, "c_adjno");

                    adjTransH = new LG_adjustTransH()
                    {
                        c_adjno = adjID,
                        c_entry = structure.Fields.Entry,
                        c_gdg = gdg,
                        c_type = structure.Fields.Type,
                        c_update = structure.Fields.Entry,
                        d_adjdate = DateTime.Now,
                        d_entry = date,
                        d_update = date,
                        l_delete = false,
                        v_ket = structure.Fields.Keterangan
                    };

                    db.LG_adjustTransHes.InsertOnSubmit(adjTransH);

                    #region Old Code

                    //db.SubmitChanges();

                    //adjTransH = (from q in db.LG_adjustTransHes
                    //             where q.v_ket == tmpNumbering
                    //             select q).Take(1).SingleOrDefault();

                    //if (adjTransH == null)
                    //{
                    //  result = "Nomor Adjustment tidak dapat di raih.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}
                    //else if (adjTransH.c_adjno.Equals("XXXXXXXXXX"))
                    //{
                    //  result = "Trigger Adjustment tidak aktif.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}

                    //adjTransH.v_ket = structure.Fields.Keterangan;

                    //adjID = adjTransH.c_adjno;

                    #endregion

                    #region type Claim

                    if (structure.Fields.Type.Equals("01", StringComparison.OrdinalIgnoreCase))
                    {
                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            lisAdjTrans = new List<LG_AdjustTransD>();
                            lisAdjTrans1 = new List<LG_AdjustTransD1>();
                            pod1 = new LG_POD1();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                field = structure.Fields.Field[nLoop];

                                if ((field != null) && field.IsNew && (!field.IsDelete))
                                {

                                }
                            }
                        }
                    }

                    #endregion

                    #region type PO

                    if (structure.Fields.Type.Equals("08", StringComparison.OrdinalIgnoreCase))
                    {
                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            lisAdjTrans = new List<LG_AdjustTransD>();
                            lisAdjTrans1 = new List<LG_AdjustTransD1>();
                            pod1 = new LG_POD1();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                field = structure.Fields.Field[nLoop];

                                if ((field != null) && field.IsNew && (!field.IsDelete))
                                {
                                    lisAdjTrans.Add(new LG_AdjustTransD()
                                    {
                                        c_adjno = adjID,
                                        c_gdg = gdg,
                                        c_iteno = field.Item,
                                        c_noref = field.NoRef,
                                        n_qty = field.Qty
                                    });

                                    pod1 = (from q in db.LG_POD1s
                                            where q.c_pono == field.NoRef &&
                                            q.c_iteno == field.Item && q.n_sisa >= field.Qty
                                            select q).Take(1).SingleOrDefault();

                                    // Cek PO
                                    if (pod1 != null)
                                    {
                                        pod1.n_sisa -= field.Qty;

                                        totalDetails++;
                                    }
                                    else
                                    {
                                        result = "Item PO Sudah Terpakai.";

                                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                        if (db.Transaction != null)
                                        {
                                            db.Transaction.Rollback();
                                        }

                                        goto endLogic;
                                    }
                                }
                                if (lisAdjTrans.Count > 0)
                                {
                                    db.LG_AdjustTransDs.InsertAllOnSubmit(lisAdjTrans.ToArray());
                                    lisAdjTrans.Clear();
                                }
                            }
                        }
                    }

                    #endregion

                    if (totalDetails > 0)
                    {
                        dic = new Dictionary<string, string>();

                        dic.Add("ADJ", adjID);
                        dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                        hasAnyChanges = true;
                    }

                    #endregion

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

                    adjTransH = (from q in db.LG_adjustTransHes
                                 where q.c_adjno == adjID
                                 select q).Take(1).SingleOrDefault();

                    if (adjTransH == null)
                    {
                        result = "Nomor Adjustment tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (adjTransH.l_delete.HasValue && adjTransH.l_delete.Value)
                    {
                        result = "Tidak dapat mengubah Nomor Adjustment yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, adjTransH.d_adjdate))
                    {
                        result = "Adjustment tidak dapat diubah, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
                    {
                        adjTransH.v_ket = structure.Fields.Keterangan;
                    }

                    adjTransH.c_update = nipEntry;
                    adjTransH.d_update = DateTime.Now;

                    #region Modify Delete Detil PO

                    if (structure.Fields.Type == "08")
                    {
                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            lisAdjTrans = new List<LG_AdjustTransD>();
                            lisAdjTrans1 = new List<LG_AdjustTransD1>();
                            pod1 = new LG_POD1();

                            for (nLoopC = 0; nLoopC < structure.Fields.Field.Length; nLoopC++)
                            {
                                field = structure.Fields.Field[nLoopC];

                                if ((field != null) && (field.IsNew) && (!field.IsModified) && (!field.IsDelete))
                                {
                                    #region Add

                                    adjTransD = new LG_AdjustTransD()
                                    {
                                        c_adjno = adjID,
                                        c_gdg = gdg,
                                        c_iteno = field.Item,
                                        c_noref = field.NoRef,
                                        n_qty = field.Qty
                                    };

                                    db.LG_AdjustTransDs.InsertOnSubmit(adjTransD);

                                    pod1 = (from q in db.LG_POD1s
                                            where q.c_pono == field.NoRef &&
                                            q.c_iteno == field.Item && q.n_sisa >= field.Qty
                                            select q).Take(1).SingleOrDefault();

                                    // Cek PO
                                    if (pod1 != null)
                                    {
                                        pod1.n_sisa -= field.Qty;
                                    }
                                    else
                                    {
                                        result = "Item PO Sudah Terpakai.";

                                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                        if (db.Transaction != null)
                                        {
                                            db.Transaction.Rollback();
                                        }

                                        goto endLogic;
                                    }

                                    #endregion
                                }
                                else if ((field != null) && (!field.IsNew) && (!field.IsModified) && (field.IsDelete))
                                {
                                    #region Delete

                                    lisAdjTrans = (from q in db.LG_AdjustTransDs
                                                   where q.c_adjno == adjID &&
                                                   q.c_noref == field.NoRef &&
                                                   q.c_iteno == field.Item && q.c_gdg == gdg
                                                   select q).ToList();

                                    for (nLoop = 0; nLoop < lisAdjTrans.Count; nLoop++)
                                    {
                                        adjTransD = lisAdjTrans[nLoop];

                                        db.LG_AdjustTransDs.DeleteOnSubmit(adjTransD);

                                        lisAdjTrans1.Add(new LG_AdjustTransD1()
                                        {
                                            c_adjno = lisAdjTrans[nLoop].c_adjno,
                                            c_gdg = lisAdjTrans[nLoop].c_gdg,
                                            c_iteno = lisAdjTrans[nLoop].c_iteno,
                                            c_noref = lisAdjTrans[nLoop].c_noref,
                                            n_qty = lisAdjTrans[nLoop].n_qty,
                                            v_ket_del = field.Keterangan,
                                            v_type_del = "03"
                                        });

                                        pod1 = (from q in db.LG_POD1s
                                                where q.c_pono == lisAdjTrans[nLoop].c_noref
                                                && q.c_iteno == lisAdjTrans[nLoop].c_iteno
                                                && q.c_gdg == lisAdjTrans[nLoop].c_gdg
                                                select q).Take(1).SingleOrDefault();

                                        pod1.n_sisa += lisAdjTrans[nLoop].n_qty;
                                    }
                                    #endregion
                                }
                            }
                            if (lisAdjTrans1.Count > 0 && lisAdjTrans1.Count > 0)
                            {
                                db.LG_AdjustTransD1s.InsertAllOnSubmit(lisAdjTrans1.ToArray());
                                lisAdjTrans1.Clear();
                            }
                        }
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

                    adjTransH = (from q in db.LG_adjustTransHes
                                 where q.c_adjno == adjID
                                 select q).Take(1).SingleOrDefault();

                    if (adjTransH == null)
                    {
                        result = "Nomor Adjustment tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (adjTransH.l_delete.HasValue && adjTransH.l_delete.Value)
                    {
                        result = "Tidak dapat menghapus nomor Adjustment yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    adjTransH.c_update = nipEntry;
                    adjTransH.d_update = DateTime.Now;

                    adjTransH.l_delete = true;
                    adjTransH.v_ket_del = structure.Fields.Keterangan;

                    lisAdjTrans = new List<LG_AdjustTransD>();
                    lisAdjTrans1 = new List<LG_AdjustTransD1>();
                    pod1 = new LG_POD1();

                    lisAdjTrans = (from q in db.LG_AdjustTransDs
                                   where q.c_adjno == adjID
                                   select q).ToList();

                    if ((lisAdjTrans != null) && (lisAdjTrans.Count > 0))
                    {
                        for (nLoopC = 0; nLoopC < lisAdjTrans.Count; nLoopC++)
                        {
                            lisAdjTrans1.Add(new LG_AdjustTransD1()
                            {
                                c_adjno = lisAdjTrans[nLoopC].c_adjno,
                                c_gdg = lisAdjTrans[nLoopC].c_gdg,
                                c_iteno = lisAdjTrans[nLoopC].c_iteno,
                                c_noref = lisAdjTrans[nLoopC].c_noref,
                                c_type = lisAdjTrans[nLoopC].c_type,
                                d_date = lisAdjTrans[nLoopC].d_date,
                                n_qty = lisAdjTrans[nLoopC].n_qty,
                                v_ket = lisAdjTrans[nLoopC].v_ket,
                                v_ket_del = structure.Fields.Keterangan,
                                v_type_del = "03"
                            });

                            pod1 = (from q in db.LG_POD1s
                                    where q.c_pono == lisAdjTrans[nLoopC].c_noref
                                    && q.c_iteno == lisAdjTrans[nLoopC].c_iteno
                                    && q.c_gdg == lisAdjTrans[nLoopC].c_gdg
                                    select q).Take(1).SingleOrDefault();

                            pod1.n_sisa += lisAdjTrans[nLoopC].n_qty;
                        }

                        if (lisAdjTrans.Count > 0 && lisAdjTrans != null)
                        {
                            db.LG_AdjustTransDs.DeleteAllOnSubmit(lisAdjTrans.ToArray());
                            lisAdjTrans.Clear();
                        }
                        if (lisAdjTrans1.Count > 0 && lisAdjTrans1 != null)
                        {
                            db.LG_AdjustTransD1s.InsertAllOnSubmit(lisAdjTrans1.ToArray());
                            lisAdjTrans1.Clear();
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

                result = string.Format("ScmsSoaLibrary.Bussiness.Adjustment:Penyesuaian - {0}", ex.Message);

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

        public string AdjustTransSTT(ScmsSoaLibrary.Parser.Class.AdjustTransStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;
            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            ScmsSoaLibrary.Parser.Class.AdjustTransStructureField field = null;
            string nipEntry = null;
            string adjID = null;
            //string tmpNumbering = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now;

            List<LG_AdjSTD> lisAdjSTD = null;
            List<LG_AdjSTD1> lisAdjSTD1 = null;
            List<LG_AdjSTD2> lisAdjSTD2 = null;
            LG_AdjSTD adjSTD = new LG_AdjSTD();
            LG_AdjSTD2 adjSTD2 = new LG_AdjSTD2();

            LG_STD1 std1 = null;
            List<LG_STD2> ListStd2 = null;
            List<LG_RND1> ListRND1 = null;


            LG_AdjSTH AdjSTH = null;

            LG_RND1 rnd1 = null;

            LG_ComboH comboh = null;

            #region New Stock Indra 20180305FM

            LG_DAILY_STOCK_v2 daily2 = null;
            LG_MOVEMENT_STOCK_v2 movement2 = null;

            #endregion

            int nLoop = 0,
              nLoopC = 0,
              nLoopRN = 0;

            decimal
              nQtyAllocGood = 0;

            IDictionary<string, string> dic = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            #region Validasi Indra 20171120

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            char gdg = (string.IsNullOrEmpty(structure.Fields.Gudang) ? char.MinValue : structure.Fields.Gudang[0]);

            if (gdg == char.MinValue)
            {
                result = "Gudang tidak boleh kosong.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            #endregion

            adjID = (structure.Fields.AdjustStockID ?? string.Empty);

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
                {
                    #region Add

                    #region Validasi Indra 20171120

                    if (string.IsNullOrEmpty(structure.Fields.Gudang))
                    {
                        result = "Gudang tidak boleh kosong.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, date))
                    {
                        result = "Adjustment tidak dapat disimpan, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    #endregion

                    adjID = Commons.GenerateNumbering<LG_AdjSTH>(db, "AS", '3', "28", date, "c_adjno");

                    AdjSTH = new LG_AdjSTH()
                    {
                        c_adjno = adjID,
                        c_beban = structure.Fields.Beban,
                        c_gdg = gdg,
                        c_type = structure.Fields.Type,
                        c_entry = structure.Fields.Entry,
                        c_update = structure.Fields.Entry,
                        d_update = date,
                        d_adjdate = date,
                        d_entry = date,
                        l_auto = false,
                        v_ket = structure.Fields.Keterangan,
                        l_delete = false,
                        l_new = true
                    };

                    db.LG_AdjSTHs.InsertOnSubmit(AdjSTH);

                    #region Insert Detil

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        lisAdjSTD = new List<LG_AdjSTD>();
                        lisAdjSTD1 = new List<LG_AdjSTD1>();
                        lisAdjSTD2 = new List<LG_AdjSTD2>();
                        std1 = new LG_STD1();
                        ListStd2 = new List<LG_STD2>();

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if ((field != null) && field.IsNew && (!field.IsDelete))
                            {
                                lisAdjSTD.Add(new LG_AdjSTD()
                                {
                                    c_adjno = adjID,
                                    c_gdg = gdg,
                                    c_iteno = field.Item,
                                    c_stno = field.NoRef,
                                    c_batch = field.Batch,
                                    n_qty = field.Qty
                                });

                                std1 = (from q in db.LG_STD1s
                                        where q.c_gdg == gdg && q.c_stno == field.NoRef
                                        && q.c_iteno == field.Item && q.c_batch == field.Batch
                                        select q).Take(1).SingleOrDefault();

                                std1.n_sisa -= field.Qty;

                                #region New Stock Indra 20180305FM

                                SavingStock.DailyStock(db, gdg.ToString(),
                                                           adjID,
                                                           structure.Fields.Type,
                                                           field.Item,
                                                           field.Batch,
                                                           field.Qty,
                                                           0,
                                                           "KOSONG",
                                                           "01",
                                                           "01",
                                                           nipEntry,
                                                           "01");

                                #endregion

                                #region Update RN

                                ListStd2 = (from q in db.LG_STD2s
                                            where q.c_gdg == gdg &&
                                            q.c_stno == field.NoRef && q.c_iteno == field.Item
                                            && q.c_batch == field.Batch
                                            select q).ToList();

                                nQtyAllocGood = field.Qty;

                                decimal GAqum = 0, Gadj = 0;

                                GAqum = nQtyAllocGood;

                                if (structure.Fields.Type == "02")
                                {
                                    for (nLoopC = 0; nLoopC < ListStd2.Count; nLoopC++)
                                    {
                                        if (ListStd2[nLoopC].c_no.Substring(0, 2) == "RN" || ListStd2[nLoopC].c_no.Substring(0, 2) == "RR")
                                        {
                                            rnd1 = (from q in db.LG_RND1s
                                                    where q.c_gdg == ListStd2[nLoopC].c_gdg &&
                                                    q.c_iteno == ListStd2[nLoopC].c_iteno &&
                                                    q.c_batch == ListStd2[nLoopC].c_batch &&
                                                    q.c_rnno == ListStd2[nLoopC].c_no
                                                    select q).Take(1).SingleOrDefault();

                                            nQtyAllocGood = nQtyAllocGood - ListStd2[nLoopC].n_qty.Value;

                                            if (GAqum < 0)
                                            {
                                                continue;
                                            }

                                            if (0 < nQtyAllocGood)
                                            {
                                                rnd1.n_gsisa += ListStd2[nLoopC].n_qty;
                                                Gadj = ListStd2[nLoopC].n_qty.HasValue ? ListStd2[nLoopC].n_qty.Value : 0;
                                            }
                                            else
                                            {
                                                if (GAqum > 0)
                                                {
                                                    rnd1.n_gsisa += GAqum;
                                                    Gadj = GAqum;
                                                }
                                            }

                                            GAqum = GAqum - ListStd2[nLoopC].n_qty.Value;


                                            lisAdjSTD2.Add(new LG_AdjSTD2()
                                            {
                                                c_adjno = adjID,
                                                c_gdg = gdg,
                                                c_iteno = field.Item,
                                                c_stno = field.NoRef,
                                                c_batch = ListStd2[nLoopC].c_batch,
                                                c_no = ListStd2[nLoopC].c_no,
                                                n_qty = Gadj
                                            });
                                        }
                                        else if (ListStd2[nLoopC].c_no.Substring(0, 2) == "CB")
                                        {
                                            comboh = (from q in db.LG_ComboHs
                                                      where q.c_gdg == ListStd2[nLoopC].c_gdg &&
                                                      q.c_iteno == ListStd2[nLoopC].c_iteno &&
                                                      q.c_batch == ListStd2[nLoopC].c_batch &&
                                                      q.c_combono == ListStd2[nLoopC].c_no
                                                      select q).Take(1).SingleOrDefault();

                                            nQtyAllocGood = nQtyAllocGood - ListStd2[nLoopC].n_qty.Value;

                                            if (GAqum < 0)
                                            {
                                                continue;
                                            }

                                            if (0 < nQtyAllocGood)
                                            {
                                                comboh.n_gsisa += ListStd2[nLoopC].n_qty;
                                                Gadj = ListStd2[nLoopC].n_qty.HasValue ? ListStd2[nLoopC].n_qty.Value : 0;
                                            }
                                            else
                                            {
                                                if (GAqum > 0)
                                                {
                                                    comboh.n_gsisa += GAqum;
                                                    Gadj = GAqum;
                                                }
                                            }

                                            GAqum = GAqum - ListStd2[nLoopC].n_qty.Value;

                                            lisAdjSTD2.Add(new LG_AdjSTD2()
                                            {
                                                c_adjno = adjID,
                                                c_gdg = gdg,
                                                c_iteno = field.Item,
                                                c_stno = field.NoRef,
                                                c_batch = ListStd2[nLoopC].c_batch,
                                                c_no = ListStd2[nLoopC].c_no,
                                                n_qty = Gadj
                                            });
                                        }
                                        else
                                        {
                                            result = "Nomor RN tidak ditemukan.";
                                            rpe = ResponseParser.ResponseParserEnum.IsFailed;
                                            if (db.Transaction != null)
                                            {
                                                db.Transaction.Rollback();
                                            }
                                            goto endLogic;
                                        }
                                    }

                                    if (lisAdjSTD2.Count > 0 && lisAdjSTD2 != null)
                                    {
                                        db.LG_AdjSTD2s.InsertAllOnSubmit(lisAdjSTD2.ToArray());
                                        lisAdjSTD2.Clear();
                                    }
                                }

                                #endregion
                            }
                            if (lisAdjSTD.Count > 0 && lisAdjSTD != null)
                            {
                                db.LG_AdjSTDs.InsertAllOnSubmit(lisAdjSTD.ToArray());
                                lisAdjSTD.Clear();
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

                    #region Validasi Indra 20171120

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

                    AdjSTH = (from q in db.LG_AdjSTHs
                              where q.c_adjno == adjID && q.c_gdg == gdg
                              select q).Take(1).SingleOrDefault();

                    if (AdjSTH == null)
                    {
                        result = "Nomor Adjustment tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (AdjSTH.l_delete.HasValue && AdjSTH.l_delete.Value)
                    {
                        result = "Tidak dapat mengubah Nomor Adjustment yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, AdjSTH.d_adjdate))
                    {
                        result = "Adjustment tidak dapat diubah, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    #endregion

                    if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
                    {
                        AdjSTH.v_ket = structure.Fields.Keterangan;
                    }

                    AdjSTH.c_update = nipEntry;
                    AdjSTH.d_update = DateTime.Now;

                    #region Populate Detil

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {

                        lisAdjSTD = new List<LG_AdjSTD>();
                        lisAdjSTD1 = new List<LG_AdjSTD1>();
                        lisAdjSTD2 = new List<LG_AdjSTD2>();
                        ListRND1 = new List<LG_RND1>();
                        std1 = new LG_STD1();
                        adjSTD2 = new LG_AdjSTD2();
                        List<AdjDetailGenerator> listAdjTemp = null;

                        for (nLoopC = 0; nLoopC < structure.Fields.Field.Length; nLoopC++)
                        {
                            field = structure.Fields.Field[nLoopC];

                            if ((field != null) && field.IsNew && (!field.IsDelete))
                            {
                                #region New Indra 20171120

                                adjSTD = new LG_AdjSTD()
                                {
                                    c_adjno = adjID,
                                    c_gdg = gdg,
                                    c_iteno = field.Item,
                                    c_stno = field.NoRef,
                                    c_batch = field.Batch,
                                    n_qty = field.Qty
                                };

                                db.LG_AdjSTDs.InsertOnSubmit(adjSTD);

                                std1 = (from q in db.LG_STD1s
                                        where q.c_gdg == gdg && q.c_stno == field.NoRef
                                        && q.c_iteno == field.Item && q.c_batch == field.Batch
                                        select q).Take(1).SingleOrDefault();

                                std1.n_sisa -= field.Qty;

                                #region New Stock Indra 20180305FM

                                SavingStock.DailyStock(db, gdg.ToString(),
                                                           adjID,
                                                           structure.Fields.Type,
                                                           field.Item,
                                                           field.Batch,
                                                           field.Qty,
                                                           0,
                                                           "KOSONG",
                                                           "01",
                                                           "01",
                                                           nipEntry,
                                                           "01");

                                #endregion

                                #region Update RN

                                ListStd2 = (from q in db.LG_STD2s
                                            where q.c_gdg == gdg &&
                                            q.c_stno == field.NoRef && q.c_iteno == field.Item
                                            && q.c_batch == field.Batch
                                            select q).ToList();

                                nQtyAllocGood = field.Qty;

                                decimal GAqum = 0, Gadj = 0;

                                GAqum = nQtyAllocGood;

                                if (AdjSTH.c_type == "02")
                                {
                                    for (nLoopRN = 0; nLoopRN < ListStd2.Count; nLoopRN++)
                                    {
                                        if (ListStd2[nLoopRN].c_no.Substring(0, 2) == "RN" || ListStd2[nLoopRN].c_no.Substring(0, 2) == "RR")
                                        {
                                            rnd1 = (from q in db.LG_RND1s
                                                    where q.c_gdg == ListStd2[nLoopRN].c_gdg &&
                                                    q.c_iteno == ListStd2[nLoopRN].c_iteno &&
                                                    q.c_batch == ListStd2[nLoopRN].c_batch &&
                                                    q.c_rnno == ListStd2[nLoopRN].c_no
                                                    select q).Take(1).SingleOrDefault();

                                            nQtyAllocGood = nQtyAllocGood - ListStd2[nLoopRN].n_qty.Value;

                                            if (GAqum < 0)
                                            {
                                                continue;
                                            }

                                            if (0 < nQtyAllocGood)
                                            {
                                                rnd1.n_gsisa += ListStd2[nLoopRN].n_qty;
                                                Gadj = ListStd2[nLoopRN].n_qty.HasValue ? ListStd2[nLoopRN].n_qty.Value : 0;
                                            }
                                            else
                                            {
                                                if (GAqum > 0)
                                                {
                                                    rnd1.n_gsisa += GAqum;
                                                    Gadj = GAqum;
                                                }
                                            }

                                            GAqum = GAqum - ListStd2[nLoopRN].n_qty.Value;

                                            lisAdjSTD2.Add(new LG_AdjSTD2()
                                            {
                                                c_adjno = adjID,
                                                c_gdg = gdg,
                                                c_iteno = field.Item,
                                                c_stno = field.NoRef,
                                                c_batch = ListStd2[nLoopRN].c_batch,
                                                c_no = ListStd2[nLoopRN].c_no,
                                                n_qty = Gadj
                                            });
                                        }
                                        else if (ListStd2[nLoopRN].c_no.Substring(0, 2) == "CB")
                                        {
                                            comboh = (from q in db.LG_ComboHs
                                                      where q.c_gdg == ListStd2[nLoopRN].c_gdg &&
                                                      q.c_iteno == ListStd2[nLoopRN].c_iteno &&
                                                      q.c_batch == ListStd2[nLoopRN].c_batch &&
                                                      q.c_combono == ListStd2[nLoopRN].c_no
                                                      select q).Take(1).SingleOrDefault();

                                            nQtyAllocGood = nQtyAllocGood - ListStd2[nLoopRN].n_qty.Value;

                                            if (GAqum < 0)
                                            {
                                                continue;
                                            }

                                            if (0 < nQtyAllocGood)
                                            {
                                                comboh.n_gsisa += ListStd2[nLoopRN].n_qty;
                                                Gadj = ListStd2[nLoopRN].n_qty.HasValue ? ListStd2[nLoopRN].n_qty.Value : 0;
                                            }
                                            else
                                            {
                                                if (GAqum > 0)
                                                {
                                                    comboh.n_gsisa += GAqum;
                                                    Gadj = GAqum;
                                                }
                                            }

                                            GAqum = GAqum - ListStd2[nLoopRN].n_qty.Value;

                                            lisAdjSTD2.Add(new LG_AdjSTD2()
                                            {
                                                c_adjno = adjID,
                                                c_gdg = gdg,
                                                c_iteno = field.Item,
                                                c_stno = field.NoRef,
                                                c_batch = ListStd2[nLoopRN].c_batch,
                                                c_no = ListStd2[nLoopRN].c_no,
                                                n_qty = Gadj
                                            });
                                        }
                                        else
                                        {
                                            result = "Nomor RN tidak ditemukan.";
                                            rpe = ResponseParser.ResponseParserEnum.IsFailed;
                                            if (db.Transaction != null)
                                            {
                                                db.Transaction.Rollback();
                                            }
                                            goto endLogic;
                                        }
                                    }

                                    if (lisAdjSTD2.Count > 0 && lisAdjSTD2 != null)
                                    {
                                        db.LG_AdjSTD2s.InsertAllOnSubmit(lisAdjSTD2.ToArray());
                                        lisAdjSTD2.Clear();
                                    }
                                }
                                #endregion

                                #endregion
                            }
                            else if (!(field.IsNew) && !(field.IsModified) && (field.IsDelete))
                            {
                                #region Delete Indra 20171120

                                lisAdjSTD = (from q in db.LG_AdjSTDs
                                             where q.c_adjno == adjID &&
                                             q.c_stno == field.NoRef && q.c_iteno == field.Item
                                             && q.c_batch == field.Batch
                                             select q).ToList();

                                for (nLoop = 0; nLoop < lisAdjSTD.Count; nLoop++)
                                {
                                    adjSTD = lisAdjSTD[nLoop];

                                    listAdjTemp = (from q in GlobalQuery.ViewStockLite(db, gdg, field.Item)
                                                   group q by new { q.c_iteno } into g
                                                   where (g.Sum(x => x.n_gsisa) >= adjSTD.n_qty)
                                                   select new AdjDetailGenerator()
                                                   {
                                                       Item = g.Key.c_iteno,
                                                       n_gsisa = g.Sum(x => x.n_gsisa)
                                                   }).ToList();

                                    if (listAdjTemp.Count > 0)
                                    {
                                        db.LG_AdjSTDs.DeleteOnSubmit(adjSTD);

                                        lisAdjSTD1.Add(new LG_AdjSTD1()
                                        {
                                            c_adjno = adjID,
                                            c_gdg = lisAdjSTD[nLoop].c_gdg,
                                            c_iteno = lisAdjSTD[nLoop].c_iteno,
                                            c_stno = lisAdjSTD[nLoop].c_stno,
                                            n_qty = lisAdjSTD[nLoop].n_qty,
                                            v_ket_del = field.Keterangan,
                                            c_batch = lisAdjSTD[nLoop].c_batch,
                                            v_type = "02",
                                            c_entry = structure.Fields.Entry,
                                            d_entry = date
                                        });


                                        std1 = (from q in db.LG_STD1s
                                                where q.c_gdg == gdg && q.c_stno == lisAdjSTD[nLoop].c_stno
                                                && q.c_iteno == lisAdjSTD[nLoop].c_iteno
                                                && q.c_batch == lisAdjSTD[nLoop].c_batch
                                                select q).Take(1).SingleOrDefault();

                                        std1.n_sisa += lisAdjSTD[nLoop].n_qty;

                                        #region New Stock Indra 20180305FM

                                        if ((SavingStock.DailyStock(db, gdg.ToString(),
                                                                        adjID,
                                                                        AdjSTH.c_type,
                                                                        lisAdjSTD[nLoop].c_iteno,
                                                                        lisAdjSTD[nLoop].c_batch,
                                                                        lisAdjSTD[nLoop].n_qty.Value * -1,
                                                                        0,
                                                                        "KOSONG",
                                                                        "02",
                                                                        "01",
                                                                        nipEntry,
                                                                        "01")) == 0)
                                        {
                                            result = "Terdapat Kesalahan pada Item " + lisAdjSTD[nLoop].c_iteno + " dengan Batch " + lisAdjSTD[nLoop].c_batch + ". Harap Hubungi MIS.";

                                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                            if (db.Transaction != null)
                                            {
                                                db.Transaction.Rollback();
                                            }

                                            goto endLogic;
                                        }

                                        #endregion

                                        #region Update RN

                                        lisAdjSTD2 = (from q in db.LG_AdjSTD2s
                                                    where q.c_gdg == gdg &&
                                                    q.c_stno == field.NoRef && q.c_iteno == field.Item
                                                    && q.c_batch == field.Batch
                                                    select q).ToList();

                                        nQtyAllocGood = field.Qty;

                                        decimal GAqum = 0;

                                        GAqum = nQtyAllocGood;

                                        if (AdjSTH.c_type == "02")
                                        {
                                            for (nLoopRN = 0; nLoopRN < lisAdjSTD2.Count; nLoopRN++)
                                            {
                                                if (lisAdjSTD2[nLoopRN].c_no.Substring(0, 2) == "RN" || lisAdjSTD2[nLoopRN].c_no.Substring(0, 2) == "RR")
                                                {
                                                    rnd1 = (from q in db.LG_RND1s
                                                            where q.c_gdg == lisAdjSTD2[nLoopRN].c_gdg &&
                                                            q.c_iteno == lisAdjSTD2[nLoopRN].c_iteno &&
                                                            q.c_batch == lisAdjSTD2[nLoopRN].c_batch &&
                                                            q.c_rnno == lisAdjSTD2[nLoopRN].c_no &&
                                                            q.n_gsisa >= 0
                                                            select q).Take(1).SingleOrDefault();

                                                    if (rnd1 != null)
                                                    {

                                                        nQtyAllocGood = nQtyAllocGood - lisAdjSTD2[nLoopRN].n_qty.Value;

                                                        if (GAqum < 0)
                                                        {
                                                            continue;
                                                        }

                                                        if (0 < nQtyAllocGood)
                                                        {
                                                            rnd1.n_gsisa -= lisAdjSTD2[nLoopRN].n_qty;
                                                        }
                                                        else
                                                        {
                                                            if (GAqum > 0 && rnd1.n_gsisa >= GAqum)
                                                            {
                                                                rnd1.n_gsisa -= GAqum;
                                                            }
                                                            else
                                                            {
                                                                rpe = ResponseParser.ResponseParserEnum.IsFailed;
                                                                result = "Quantity Minus";
                                                                if (db.Transaction != null)
                                                                {
                                                                    db.Transaction.Rollback();
                                                                }
                                                                goto endLogic;
                                                            }
                                                        }

                                                        GAqum = GAqum - lisAdjSTD2[nLoopRN].n_qty.Value;
                                                    }
                                                    else
                                                    {
                                                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                                        result = "Quantiti Sudah Terpakai ";

                                                        if (db.Transaction != null)
                                                        {
                                                            db.Transaction.Rollback();
                                                        }

                                                        goto endLogic;
                                                    }
                                                }
                                                else if (lisAdjSTD2[nLoopRN].c_no.Substring(0, 2) == "CB")
                                                {
                                                    comboh = (from q in db.LG_ComboHs
                                                              where q.c_gdg == lisAdjSTD2[nLoopRN].c_gdg &&
                                                              q.c_iteno == lisAdjSTD2[nLoopRN].c_iteno &&
                                                              q.c_batch == lisAdjSTD2[nLoopRN].c_batch &&
                                                              q.c_combono == lisAdjSTD2[nLoopRN].c_no &&
                                                              q.n_gsisa > 0
                                                              select q).Take(1).SingleOrDefault();

                                                    if (comboh != null)
                                                    {

                                                        nQtyAllocGood = nQtyAllocGood - lisAdjSTD2[nLoopRN].n_qty.Value;

                                                        if (GAqum < 0)
                                                        {
                                                            continue;
                                                        }

                                                        if (0 < nQtyAllocGood)
                                                        {
                                                            comboh.n_gsisa -= lisAdjSTD2[nLoopRN].n_qty;
                                                        }
                                                        else
                                                        {
                                                            if (GAqum > 0 && comboh.n_gsisa >= GAqum)
                                                            {
                                                                comboh.n_gsisa -= GAqum;
                                                            }
                                                            else
                                                            {
                                                                rpe = ResponseParser.ResponseParserEnum.IsFailed;
                                                                result = "Quantity Minus";
                                                                if (db.Transaction != null)
                                                                {
                                                                    db.Transaction.Rollback();
                                                                }
                                                                goto endLogic;
                                                            }
                                                        }

                                                        GAqum = GAqum - lisAdjSTD2[nLoopRN].n_qty.Value;
                                                    }
                                                    else
                                                    {
                                                        rpe = ResponseParser.ResponseParserEnum.IsFailed;
                                                        result = "Quantiti Sudah Terpakai ";
                                                        if (db.Transaction != null)
                                                        {
                                                            db.Transaction.Rollback();
                                                        }
                                                        goto endLogic;
                                                    }
                                                }
                                                else
                                                {
                                                    result = "Nomor RN tidak ditemukan.";
                                                    rpe = ResponseParser.ResponseParserEnum.IsFailed;
                                                    if (db.Transaction != null)
                                                    {
                                                        db.Transaction.Rollback();
                                                    }
                                                    goto endLogic;
                                                }

                                                adjSTD2 = (from q in db.LG_AdjSTD2s
                                                              where q.c_adjno == adjID &&
                                                            q.c_gdg == lisAdjSTD2[nLoopRN].c_gdg &&
                                                            q.c_iteno == lisAdjSTD2[nLoopRN].c_iteno &&
                                                            q.c_batch == lisAdjSTD2[nLoopRN].c_batch &&
                                                            q.c_no == lisAdjSTD2[nLoopRN].c_no &&
                                                            q.c_stno == lisAdjSTD2[nLoopRN].c_stno
                                                           select q).Take(1).SingleOrDefault();

                                                if (adjSTD2 != null)
                                                {
                                                    db.LG_AdjSTD2s.DeleteOnSubmit(adjSTD2);
                                                }
                                                else
                                                {
                                                    result = "Cant Find STD2.";
                                                    rpe = ResponseParser.ResponseParserEnum.IsFailed;
                                                    if (db.Transaction != null)
                                                    {
                                                        db.Transaction.Rollback();
                                                    }
                                                    goto endLogic;
                                                }
                                            }
                                        }
                                        lisAdjSTD2.Clear();
                                        #endregion

                                    }
                                }

                                #endregion
                            }

                            if (lisAdjSTD1.Count > 0)
                            {
                                db.LG_AdjSTD1s.InsertAllOnSubmit(lisAdjSTD1.ToArray());
                                lisAdjSTD1.Clear();
                            }

                        }

                    }

                    #endregion

                    #endregion

                    hasAnyChanges = true;
                }
                if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    #region Validasi Indra 20171120

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

                    AdjSTH = (from q in db.LG_AdjSTHs
                              where q.c_adjno == adjID
                              select q).Take(1).SingleOrDefault();

                    if (AdjSTH == null)
                    {
                        result = "Nomor Adjustment tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (AdjSTH.l_delete.HasValue && AdjSTH.l_delete.Value)
                    {
                        result = "Tidak dapat menghapus nomor Adjustment yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, AdjSTH.d_adjdate))
                    {
                        result = "Adjustment tidak dapat dihapus, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    #endregion
                    
                    AdjSTH.c_update = nipEntry;
                    AdjSTH.d_update = DateTime.Now;

                    AdjSTH.l_delete = true;
                    AdjSTH.v_ket_mark = structure.Fields.Keterangan;

                    lisAdjSTD = (from q in db.LG_AdjSTDs
                                 where q.c_adjno == adjID
                                 select q).ToList();

                    if ((lisAdjSTD != null) && (lisAdjSTD.Count > 0))
                    {
                        lisAdjSTD1 = new List<LG_AdjSTD1>();
                        ListStd2 = new List<LG_STD2>();
                        rnd1 = new LG_RND1();
                        ListRND1 = new List<LG_RND1>();
                        std1 = new LG_STD1();
                        adjSTD2 = new LG_AdjSTD2();
                        List<AdjDetailGenerator> listAdjTemp = null;

                        for (nLoopC = 0; nLoopC < lisAdjSTD.Count; nLoopC++)
                        {
                            listAdjTemp = (from q in GlobalQuery.ViewStockLite(db, gdg, lisAdjSTD[nLoopC].c_iteno)
                                           group q by new { q.c_iteno } into g
                                           where (g.Sum(x => x.n_gsisa) >= lisAdjSTD[nLoopC].n_qty)
                                           select new AdjDetailGenerator()
                                           {
                                               Item = g.Key.c_iteno
                                           }).ToList();

                            if (listAdjTemp.Count > 0 && listAdjTemp != null)
                            {
                                lisAdjSTD1.Add(new LG_AdjSTD1()
                                {
                                    c_adjno = lisAdjSTD[nLoopC].c_adjno,
                                    c_gdg = lisAdjSTD[nLoopC].c_gdg,
                                    c_iteno = lisAdjSTD[nLoopC].c_iteno,
                                    c_stno = lisAdjSTD[nLoopC].c_stno,
                                    n_qty = lisAdjSTD[nLoopC].n_qty,
                                    v_ket_del = structure.Fields.Keterangan,
                                    c_batch = lisAdjSTD[nLoop].c_batch,
                                    v_type = "03",
                                    c_entry = structure.Fields.Entry,
                                    d_entry = date
                                });

                                std1 = (from q in db.LG_STD1s
                                        where q.c_stno == lisAdjSTD[nLoopC].c_stno
                                        && q.c_iteno == lisAdjSTD[nLoopC].c_iteno
                                        && q.c_batch == lisAdjSTD[nLoopC].c_batch
                                        select q).Take(1).SingleOrDefault();

                                std1.n_sisa += lisAdjSTD[nLoopC].n_qty;

                                #region New Stock Indra 20180305FM

                                if ((SavingStock.DailyStock(db, lisAdjSTD[nLoopC].c_gdg.ToString(),
                                                                lisAdjSTD[nLoopC].c_adjno,
                                                                AdjSTH.c_type,
                                                                lisAdjSTD[nLoopC].c_iteno,
                                                                lisAdjSTD[nLoopC].c_batch,
                                                                lisAdjSTD[nLoopC].n_qty.Value * -1,
                                                                0,
                                                                "KOSONG",
                                                                "02",
                                                                "01",
                                                                nipEntry,
                                                                "01")) == 0)
                                {
                                    result = "Terdapat Kesalahan pada Item " + lisAdjSTD[nLoop].c_iteno + " dengan Batch " + lisAdjSTD[nLoop].c_batch + ". Harap Hubungi MIS.";

                                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                    if (db.Transaction != null)
                                    {
                                        db.Transaction.Rollback();
                                    }

                                    goto endLogic;
                                }

                                #endregion

                                #region Update RN

                                lisAdjSTD2 = (from q in db.LG_AdjSTD2s
                                            where q.c_gdg == gdg &&
                                            q.c_stno == lisAdjSTD[nLoopC].c_stno && q.c_iteno == lisAdjSTD[nLoopC].c_iteno
                                            && q.c_batch == lisAdjSTD[nLoopC].c_batch
                                            select q).ToList();

                                nQtyAllocGood = lisAdjSTD[nLoopC].n_qty.Value;

                                decimal GAqum = 0;

                                GAqum = nQtyAllocGood;

                                if (AdjSTH.c_type == "02")
                                {
                                    for (nLoop = 0; nLoop < lisAdjSTD2.Count; nLoop++)
                                    {
                                        if (lisAdjSTD2[nLoop].c_no.Substring(0, 2) == "RN" || lisAdjSTD2[nLoop].c_no.Substring(0, 2) == "RR")
                                        {
                                            rnd1 = (from q in db.LG_RND1s
                                                    where q.c_gdg == lisAdjSTD2[nLoop].c_gdg &&
                                                    q.c_iteno == lisAdjSTD2[nLoop].c_iteno &&
                                                    q.c_batch == lisAdjSTD2[nLoop].c_batch &&
                                                    q.c_rnno == lisAdjSTD2[nLoop].c_no &&
                                                    q.n_gsisa > 0
                                                    select q).Take(1).SingleOrDefault();

                                            if (rnd1 != null)
                                            {
                                                nQtyAllocGood = nQtyAllocGood - lisAdjSTD2[nLoop].n_qty.Value;

                                                if (GAqum < 0)
                                                {
                                                    continue;
                                                }
                                                if (0 < nQtyAllocGood)
                                                {
                                                    rnd1.n_gsisa -= lisAdjSTD2[nLoop].n_qty;
                                                }
                                                else
                                                {
                                                    if (GAqum > 0)
                                                    {
                                                        rnd1.n_gsisa -= GAqum;
                                                    }
                                                }

                                                GAqum = GAqum - lisAdjSTD2[nLoop].n_qty.Value;
                                            }
                                            else
                                            {
                                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                                result = "Quantiti Sudah Terpakai ";

                                                if (db.Transaction != null)
                                                {
                                                    db.Transaction.Rollback();
                                                }

                                                goto endLogic;
                                            }
                                        }
                                        else if (lisAdjSTD2[nLoop].c_no.Substring(0, 2) == "CB")
                                        {
                                            comboh = (from q in db.LG_ComboHs
                                                      where q.c_gdg == lisAdjSTD2[nLoop].c_gdg &&
                                                      q.c_iteno == lisAdjSTD2[nLoop].c_iteno &&
                                                      q.c_batch == lisAdjSTD2[nLoop].c_batch &&
                                                      q.c_combono == lisAdjSTD2[nLoop].c_no &&
                                                      q.n_gsisa > 0
                                                      select q).Take(1).SingleOrDefault();

                                            if (comboh != null)
                                            {

                                                nQtyAllocGood = nQtyAllocGood - lisAdjSTD2[nLoop].n_qty.Value;

                                                if (GAqum < 0)
                                                {
                                                    continue;
                                                }
                                                if (0 < nQtyAllocGood)
                                                {
                                                    comboh.n_gsisa -= lisAdjSTD2[nLoop].n_qty;
                                                }
                                                else
                                                {
                                                    if (GAqum > 0)
                                                    {
                                                        comboh.n_gsisa -= GAqum;
                                                    }
                                                }

                                                GAqum = GAqum - lisAdjSTD2[nLoop].n_qty.Value;
                                            }
                                            else
                                            {
                                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                                result = "Quantiti Sudah Terpakai ";

                                                if (db.Transaction != null)
                                                {
                                                    db.Transaction.Rollback();
                                                }

                                                goto endLogic;
                                            }
                                        }
                                        else
                                        {
                                            result = "Nomor RN tidak ditemukan.";
                                            rpe = ResponseParser.ResponseParserEnum.IsFailed;
                                            if (db.Transaction != null)
                                            {
                                                db.Transaction.Rollback();
                                            }
                                            goto endLogic;
                                        }

                                        adjSTD2 = (from q in db.LG_AdjSTD2s
                                                   where q.c_adjno == adjID &&
                                                 q.c_gdg == lisAdjSTD2[nLoop].c_gdg &&
                                                 q.c_iteno == lisAdjSTD2[nLoop].c_iteno &&
                                                 q.c_batch == lisAdjSTD2[nLoop].c_batch &&
                                                 q.c_no == lisAdjSTD2[nLoop].c_no &&
                                                 q.c_stno == lisAdjSTD2[nLoop].c_stno
                                                   select q).Take(1).SingleOrDefault();

                                        if (adjSTD2 != null)
                                        {
                                            db.LG_AdjSTD2s.DeleteOnSubmit(adjSTD2);
                                        }
                                        else
                                        {
                                            result = "Cant Find STD2.";
                                            rpe = ResponseParser.ResponseParserEnum.IsFailed;
                                            if (db.Transaction != null)
                                            {
                                                db.Transaction.Rollback();
                                            }
                                            goto endLogic;
                                        }
                                    }
                                }
                                lisAdjSTD2.Clear();
                                #endregion
                            }
                            else
                            {
                                result = "Stok Tidak mencukupi.";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }

                                goto endLogic;
                            }
                        }
                    }
                    if ((lisAdjSTD1 != null) && (lisAdjSTD1.Count > 0))
                    {
                        db.LG_AdjSTD1s.InsertAllOnSubmit(lisAdjSTD1.ToArray());
                        lisAdjSTD1.Clear();
                    }
                    if ((lisAdjSTD != null) && (lisAdjSTD.Count > 0))
                    {
                        db.LG_AdjSTDs.DeleteAllOnSubmit(lisAdjSTD.ToArray());
                        lisAdjSTD.Clear();
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

                result = string.Format("ScmsSoaLibrary.Bussiness.Adjustment:Penyesuaian - {0}", ex.Message);

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
