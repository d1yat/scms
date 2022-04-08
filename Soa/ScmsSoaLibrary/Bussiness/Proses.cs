/*
 * Created By Indra
 * 20171231FM
 * 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsModel;
using ScmsSoaLibrary.Parser;
using ScmsSoaLibrary.Commons;
using System.Data.Common;
using ScmsSoaLibraryInterface.Commons;
using System.Data.SqlClient;

namespace ScmsSoaLibrary.Bussiness
{
    class Proses
    {
        #region Internal Class

        internal class StockOpname
        {
            public string C_BatQtyAfterSO { get; set; }
            public string S_TAHUN { get; set; }
            public string T_BULAN { get; set; }
            public string D_TGL { get; set; }
            public string C_GDG { get; set; }
            public string C_NOSUP { get; set; }
            public string C_KDDIVPRI { get; set; }
            public string LOCATION { get; set; }
            public string C_ITENO { get; set; }
            public string STBARANG { get; set; }
            public string C_BATCH { get; set; }
            public Decimal QTYSYS { get; set; }
            public DateTime D_EXPIRED { get; set; }
            public string C_ENTRY { get; set; }
            public DateTime D_ENTRY { get; set; }
            public string C_REFF { get; set; }
        }

        internal class tblProdukKosong
        {
            public string c_pkno { get; set; }
            public DateTime d_pkdate { get; set; }
            public string v_nama { get; set; }
            public string v_nmdivpri { get; set; }
            public string c_iteno { get; set; }
            public string v_itnam { get; set; }
            public DateTime d_abe { get; set; }
            public string c_tipe { get; set; }
        }

        #endregion

        #region StockOpname Indra 20171231FM

        public string StokOpname(ScmsSoaLibrary.Parser.Class.StockOpnameStructure structure)
        {
            return StokOpname(structure, null, false);
        }

        public string StokOpname(ScmsSoaLibrary.Parser.Class.StockOpnameStructure structure, ORMDataContext dbContext, bool isRemoveSisa)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

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

            ScmsSoaLibrary.Parser.Class.StockOpnameStructureField field = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime dateSO = DateTime.MinValue;

            List<LG_StockSaving> listSO = null;

            LG_StockSaving SO = null;

            int nLoop = 0;

            IDictionary<string, string> dic = null;

            int totalDetails = 0;

            DateTime date = DateTime.Now;

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                dic = new Dictionary<string, string>();

                if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                {
                    listSO = new List<LG_StockSaving>();

                    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    {
                        field = structure.Fields.Field[nLoop];

                        SO = (from q in db.LG_StockSavings
                              where q.C_GDG     == Convert.ToChar(field.Gudang) &&
                                    q.V_NAMA    == field.Principal &&
                                    q.C_ITENO   == field.KdBarang &&
                                    q.C_BATCH   == field.Batch &&
                                    q.STBARANG  == field.StBarang
                              select q).Take(1).SingleOrDefault();

                        if (SO == null)
                        {
                            result = "Kode item : " + field.KdBarang + " dengan batch : " + field.Batch + " tidak ditemukan. Input hitung SO dibatalkan";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                        else
                        {
                            if (field.Stage == "0")
                            {
                                SO.SOQTY            = field.SOQty;
                                SO.D_SOQTY          = DateTime.Now;
                            }
                            else if (field.Stage == "1")
                            {
                                SO.RECOUNT1         = field.Recount1;
                                SO.D_RECOUNT1       = DateTime.Now;
                            }
                            else if (field.Stage == "2")
                            {
                                SO.RECOUNT2 = field.Recount2;
                                SO.D_RECOUNT2 = DateTime.Now;
                            }
                            
                            SO.SELISIH          = field.Selisih;
                            SO.D_UPDATEEXPIRED  = field.ExpiredDateFormated;
                            SO.D_UPDATE         = DateTime.Now;
                        }



                        totalDetails++;

                    }

                }

                if (totalDetails > 0)
                {
                    dic.Add("STOCK OPNAME", "0");
                    dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                    result = string.Format("Total {0} detail(s)", totalDetails);

                    hasAnyChanges = true;
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

                result = string.Format("ScmsSoaLibrary.Bussiness.Proses:ProcessStokOpname - {0}", ex.Message);

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

        public string StokOpnameBUATFORMSO(ScmsSoaLibrary.Parser.Class.StockOpnameStructure structure)
        {
            return StokOpnameBUATFORMSO(structure, null, false);
        }

        public string StokOpnameBUATFORMSO(ScmsSoaLibrary.Parser.Class.StockOpnameStructure structure, ORMDataContext dbContext, bool isRemoveSisa)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

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

            ScmsSoaLibrary.Parser.Class.StockOpnameStructureField field = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime dateSO = DateTime.MinValue;

            List<LG_StockSaving> listSO = null;
            List<LG_BatQtyFreeze> listBatch = null;
            List<LG_TotQtyFreeze> listTotal = null;

            LG_StockSaving SO, FREEZE = null;
            LG_TotQtyFreeze TotSO = null;
            LG_BatQtyFreeze BatSO = null;

            IDictionary<string, string> dic = null;

            int totalDetails = 0, nLoop = 0;

            DateTime date = DateTime.Now;

            string NoForm = "";

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                dic = new Dictionary<string, string>();

                if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                {
                    listSO = new List<LG_StockSaving>();
                    listBatch = new List<LG_BatQtyFreeze>();
                    listTotal = new List<LG_TotQtyFreeze>();

                    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    {
                        field = structure.Fields.Field[nLoop];

                        SO = (from q in db.LG_StockSavings
                              where q.C_GDG     == Convert.ToChar(field.Gudang) &&
                                    q.V_NAMA    == field.Principal &&
                                    q.C_ITENO   == field.KdBarang &&
                                    q.C_BATCH   == field.Batch.Replace("'", "\\'") &&
                                    q.STBARANG  == field.StBarang
                              select q).Take(1).SingleOrDefault();

                        if (SO == null)
                        {
                            #region Create No. Form

                            if (structure.Fields.BuatForm == "0")
                            {
                                NoForm = string.Concat(structure.Fields.Gudang, field.KdPrincipal, structure.Fields.Kategori, structure.Fields.Item, structure.Fields.Status);
                            }
                            else
                            {
                                NoForm = string.Concat(structure.Fields.Gudang, field.KdPrincipal, field.KdDivPrincipal, structure.Fields.Kategori, structure.Fields.Item, structure.Fields.Status);
                            }

                            #endregion

                            #region Validasi Freeze

                            FREEZE = (from q in db.LG_StockSavings
                                      where q.C_REFF == NoForm &&
                                            q.C_STAGE > 0
                                      select q).Take(1).SingleOrDefault();

                            if (FREEZE != null)
                            {
                                result = "Form : " + NoForm + " sudah tahap konfirmasi, tidak dapat menambahkan item : " + field.KdBarang + ". Buat atau penambahan form dibatalkan";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }

                                goto endLogic;
                            }

                            #endregion

                            #region Validasi Backup

                            BatSO = (from q in db.LG_BatQtyFreezes
                                 where q.C_GDG    == Convert.ToChar(field.Gudang) &&
                                       q.C_ITENO  == field.KdBarang &&
                                       q.C_BATCH  == field.Batch &&
                                       q.STBARANG == field.StBarang &&
                                       q.S_TAHUN  == date.Year.ToString() &&
                                       q.T_BULAN  == date.Month.ToString() &&
                                       q.D_TGL    == date.Day.ToString()
                                 select q).Take(1).SingleOrDefault();

                            if (BatSO != null)
                            {
                                result = "Kode item : " + BatSO.C_ITENO + " dengan batch : " + BatSO.C_BATCH + " Periode : " + BatSO.D_TGL + "/" + BatSO.T_BULAN + "/" + BatSO.S_TAHUN + " sudah ada. Tidak bisa membuat form dengan periode yang sama";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }

                                goto endLogic;
                            }

                            #endregion                            

                            #region Freeze

                            listSO.Add(new LG_StockSaving()
                            {
                                C_GDG           = Convert.ToChar(field.Gudang),
                                C_NOSUP         = field.KdPrincipal,
                                V_NAMA          = field.Principal,
                                C_KDDIVPRI      = field.KdDivPrincipal,
                                V_NMDIVPRI      = field.DivPrincipal,
                                LOCATION        = field.Location,
                                C_ITENO         = field.KdBarang,
                                V_ITNAM         = field.NmBarang,
                                STBARANG        = field.StBarang,
                                C_BATCH         = field.Batch,
                                QTYSYS          = field.QtySys,
                                SOQTY           = 0,
                                D_SOQTY         = DateTime.Now,
                                RECOUNT1        = 0,
                                D_RECOUNT1      = DateTime.Now,
                                RECOUNT2        = 0,
                                D_RECOUNT2      = DateTime.Now,
                                SELISIH         = field.SOQty - field.QtySys,
                                D_EXPIRED       = field.ExpiredDateFormated,
                                Box             = field.Box,
                                D_UPDATEEXPIRED = field.ExpiredDateFormated,
                                C_ENTRY         = field.Entry,
                                D_ENTRY         = DateTime.Now,
                                D_UPDATE        = DateTime.Now,
                                C_STAGE         = 0,
                                C_REFF          = NoForm
                            });

                            #endregion

                            #region Backup per Batch

                            listBatch.Add(new LG_BatQtyFreeze()
                            {   
                                C_BatQtyFreeze  = string.Concat(date.Year.ToString().Substring(2,2), date.Month.ToString(), date.Day.ToString(), NoForm),
                                T_BULAN         = date.Month.ToString(),
                                S_TAHUN         = date.Year.ToString(),
                                D_TGL           = date.Day.ToString(),
                                C_GDG           = Convert.ToChar(field.Gudang),
                                C_NOSUP         = field.KdPrincipal,
                                C_KDDIVPRI      = field.KdDivPrincipal,
                                LOCATION        = field.Location,
                                C_ITENO         = field.KdBarang,
                                STBARANG        = field.StBarang,
                                C_BATCH         = field.Batch,
                                QTYSYS          = field.QtySys,
                                D_EXPIRED       = field.ExpiredDateFormated,
                                C_ENTRY         = field.Entry,
                                D_ENTRY         = DateTime.Now,
                                C_REFF          = NoForm
                            });

                            #endregion

                            #region Backup per Item

                            TotSO = (from q in db.LG_TotQtyFreezes
                                     where q.C_GDG    == Convert.ToChar(field.Gudang) &&
                                           q.C_ITENO  == field.KdBarang &&
                                           q.STBARANG == field.StBarang &&
                                           q.S_TAHUN  == date.Year.ToString() &&
                                           q.T_BULAN  == date.Month.ToString() &&
                                           q.D_TGL    == date.Day.ToString()
                                     select q).Take(1).SingleOrDefault();

                            if (TotSO == null)
                            {
                                listTotal.Add(new LG_TotQtyFreeze()
                                {   
                                    C_TotQtyFreeze  = string.Concat(date.Year.ToString().Substring(2,2), date.Month.ToString(), date.Day.ToString(), NoForm),
                                    T_BULAN         = date.Month.ToString(),
                                    S_TAHUN         = date.Year.ToString(),
                                    D_TGL           = date.Day.ToString(),
                                    C_GDG           = Convert.ToChar(field.Gudang),
                                    C_NOSUP         = field.KdPrincipal,
                                    C_KDDIVPRI      = field.KdDivPrincipal,
                                    LOCATION        = field.Location,
                                    C_ITENO         = field.KdBarang,
                                    STBARANG        = field.StBarang,
                                    QTYSYS          = field.QtySys,
                                    C_ENTRY         = field.Entry,
                                    D_ENTRY         = DateTime.Now,
                                    C_REFF          = NoForm
                                });
                            }
                            else
                            {
                                TotSO.QTYSYS += field.QtySys;
                            }

                            #endregion

                            if (listTotal.Count > 0)
                            {
                                db.LG_TotQtyFreezes.InsertAllOnSubmit(listTotal.ToArray());
                                listTotal.Clear();
                                db.SubmitChanges();
                            }
                        }

                        totalDetails++;

                    }

                    if (listSO.Count > 0)
                    {                    
                        db.LG_StockSavings.InsertAllOnSubmit(listSO.ToArray());
                        db.LG_BatQtyFreezes.InsertAllOnSubmit(listBatch.ToArray());
                        listSO.Clear();
                        listBatch.Clear();
                    }

                }

                if (totalDetails > 0)
                {
                    dic.Add("STOCK OPNAME", "0");
                    dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                    result = string.Format("Total {0} detail(s)", totalDetails);

                    hasAnyChanges = true;
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

                result = string.Format("ScmsSoaLibrary.Bussiness.Proses:ProcessStokOpname - {0}", ex.Message);

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

        public string StokOpnameFORMSOBATAL(ScmsSoaLibrary.Parser.Class.StockOpnameStructure structure)
        {
            return StokOpnameFORMSOBATAL(structure, null, false);
        }

        public string StokOpnameFORMSOBATAL(ScmsSoaLibrary.Parser.Class.StockOpnameStructure structure, ORMDataContext dbContext, bool isRemoveSisa)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

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

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime dateSO = DateTime.MinValue;

            List<LG_StockSaving> listSO = null;
            List<LG_BatQtyFreeze> listBatch = null;
            List<LG_TotQtyFreeze> listTotal = null;

            LG_StockSaving SO = null;

            IDictionary<string, string> dic = null;

            int totalDetails = 0;

            DateTime date = DateTime.Now;

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                dic = new Dictionary<string, string>();

                listSO = new List<LG_StockSaving>();
                listBatch = new List<LG_BatQtyFreeze>();
                listTotal = new List<LG_TotQtyFreeze>();

                SO = (from q in db.LG_StockSavings
                      where q.NOADJ     == structure.Fields.Noform &&
                            q.C_STAGE   > 0
                      select q).Take(1).SingleOrDefault();

                if (SO != null)
                {
                    result = "No form : " + structure.Fields.Noform + " sudah tahap konfirmasi. Batal form dibatalkan";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                listSO = (from q in db.LG_StockSavings
                          where q.C_REFF    == structure.Fields.Noform &&
                                q.C_STAGE   == 0
                          select q).Distinct().ToList();

                if ((listSO != null) && (listSO.Count > 0))
                {
                    listBatch = (from q in db.LG_BatQtyFreezes
                                 where q.C_REFF     == structure.Fields.Noform &&
                                       q.S_TAHUN    == date.Year.ToString() &&
                                       q.T_BULAN    == date.Month.ToString()
                                 select q).Distinct().ToList();

                    if ((listBatch != null) && (listBatch.Count > 0))
                    {
                        db.LG_BatQtyFreezes.DeleteAllOnSubmit(listBatch.ToArray());
                        listBatch.Clear();
                    }
                    else
                    {
                        result = "Backup data perbatch tidak ditemukan, Pembatalan Form tidak bisa dilakukan";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    listTotal = (from q in db.LG_TotQtyFreezes
                                 where q.C_REFF     == structure.Fields.Noform &&
                                       q.S_TAHUN    == date.Year.ToString() &&
                                       q.T_BULAN    == date.Month.ToString()
                                 select q).Distinct().ToList();

                    if ((listTotal != null) && (listTotal.Count > 0))
                    {
                        db.LG_TotQtyFreezes.DeleteAllOnSubmit(listTotal.ToArray());
                        listTotal.Clear();
                    }
                    else
                    {
                        result = "Backup data total tidak ditemukan, Pembatalan Form tidak bisa dilakukan";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    db.LG_StockSavings.DeleteAllOnSubmit(listSO.ToArray());
                    listSO.Clear();

                    totalDetails++;
                }
                else
                {
                    result = "No. Form tidak ditemukan atau sudah melakukan confirm, Pembatalan Form tidak bisa dilakukan";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                if (totalDetails > 0)
                {
                    dic.Add("STOCK OPNAME", "0");
                    dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                    result = string.Format("Total {0} detail(s)", totalDetails);

                    hasAnyChanges = true;
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

                result = string.Format("ScmsSoaLibrary.Bussiness.Proses:ProcessStokOpname - {0}", ex.Message);

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

        public string StokOpnameCONFIRMSO(ScmsSoaLibrary.Parser.Class.StockOpnameStructure structure)
        {
            return StokOpnameCONFIRMSO(structure, null, false);
        }

        public string StokOpnameCONFIRMSO(ScmsSoaLibrary.Parser.Class.StockOpnameStructure structure, ORMDataContext dbContext, bool isRemoveSisa)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

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

            ScmsSoaLibrary.Parser.Class.StockOpnameStructureField field = null;
            
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime dateSO = DateTime.MinValue;

            List<LG_StockSaving> listSO = null;

            LG_StockSaving SO = null;

            int nLoop = 0;

            IDictionary<string, string> dic = null;

            int totalDetails = 0;

            DateTime date = DateTime.Now;

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                dic = new Dictionary<string, string>();

                if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                {
                    listSO = new List<LG_StockSaving>();

                    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    {
                        field = structure.Fields.Field[nLoop];

                        SO = (from q in db.LG_StockSavings
                              where q.C_GDG     == Convert.ToChar(field.Gudang) &&
                                    q.V_NAMA    == field.Principal &&
                                    q.C_ITENO   == field.KdBarang &&
                                    q.C_BATCH   == field.Batch &&
                                    q.STBARANG  == field.StBarang
                              select q).Take(1).SingleOrDefault();

                        if (SO == null)
                        {
                            result = "Kode item : " + field.KdBarang + " dengan batch : " + field.Batch + " tidak ditemukan. Confirm form SO dibatalkan";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                        else
                        {
                            if (SO.C_STAGE > 2)
                            {
                                result = "Sudah Tahap Adjustment Stock, Confirm Hasil tidak bisa dilakukan";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }

                                goto endLogic;
                            }
                            else
                            {
                                SO.C_STAGE = SO.C_STAGE + 1;

                                if (SO.C_STAGE == 1)
                                {
                                    SO.RECOUNT1 = SO.SOQTY;
                                    SO.SELISIH  = SO.RECOUNT1 - SO.QTYSYS;
                                }
                                else if (SO.C_STAGE == 2)
                                {
                                    SO.RECOUNT2 = SO.RECOUNT1;
                                    SO.SELISIH  = SO.RECOUNT2 - SO.QTYSYS;
                                }
                            }

                        }

                        totalDetails++;

                    }

                    if (listSO.Count > 0)
                    {
                        db.LG_StockSavings.InsertAllOnSubmit(listSO.ToArray());
                        listSO.Clear();
                    }

                }

                if (totalDetails > 0)
                {
                    dic.Add("STOCK OPNAME", "0");
                    dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                    result = string.Format("Total {0} detail(s)", totalDetails);

                    hasAnyChanges = true;
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

                result = string.Format("ScmsSoaLibrary.Bussiness.Proses:ProcessStokOpname - {0}", ex.Message);

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

        public string StokOpnameNEWBATCH(ScmsSoaLibrary.Parser.Class.StockOpnameStructure structure)
        {
            return StokOpnameNEWBATCH(structure, null, false);
        }

        public string StokOpnameNEWBATCH(ScmsSoaLibrary.Parser.Class.StockOpnameStructure structure, ORMDataContext dbContext, bool isRemoveSisa)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

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

            ScmsSoaLibrary.Parser.Class.StockOpnameStructureField field = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime dateSO = DateTime.MinValue;

            LG_StockSaving SO = null;
            FA_MasItm FM, FM1 = null;
            LG_DatSup LD = null;
            FA_Divpri FD = null;
            FA_MsDivPri FM2 = null;
            LG_MsBatch batch = null;

            int nLoop = 0;

            IDictionary<string, string> dic = null;

            int totalDetails = 0;

            DateTime date = DateTime.Now;

            date = Convert.ToDateTime(structure.Fields.Expired);

            if ((structure.Fields.Batch.Contains("'")) || (structure.Fields.Batch.Contains("*")) || (structure.Fields.Batch.Contains("%")))
            {
                result = "Tidak Boleh Karakter ' ( Kutip) atau  * (Bintang) atau % (Persen) ";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                if (db.Transaction != null)
                {
                    db.Transaction.Rollback();
                }

                goto endLogic;
            }

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                string reffnewbatch = structure.Fields.Noform.Substring(structure.Fields.Noform.Length - 1, 1);

                if (reffnewbatch == "G")
                {
                    SO = (from q in db.LG_StockSavings
                          where q.C_BATCH   == structure.Fields.Batch &&
                                q.C_ITENO   == structure.Fields.Item &&
                                q.C_GDG     == Convert.ToChar(structure.Fields.Gudang) &&
                                q.STBARANG  == "Stock Good"
                          select q).Take(1).SingleOrDefault();
                }
                else
                {
                    SO = (from q in db.LG_StockSavings
                          where q.C_BATCH == structure.Fields.Batch &&
                                q.C_ITENO == structure.Fields.Item &&
                                q.C_GDG == Convert.ToChar(structure.Fields.Gudang) &&
                                q.STBARANG == "Stock Bad"
                          select q).Take(1).SingleOrDefault();
                }
                    

                if (SO != null)
                {
                    result = "Item " + structure.Fields.Item + " dengan Batch " + structure.Fields.Batch + " Sudah ada.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }                

                FM = (from q in db.FA_MasItms
                      where q.c_iteno == structure.Fields.Item
                      select q).SingleOrDefault();

                LD = (from q in db.LG_DatSups
                      where q.c_nosup == FM.c_nosup
                      select q).SingleOrDefault();
                
                FD = (from q in db.FA_Divpris
                      where q.c_iteno == structure.Fields.Item
                      select q).SingleOrDefault();

                FM2 =(from q in db.FA_MsDivPris
                      where q.c_kddivpri == FD.c_kddivpri && q.c_nosup == FM.c_nosup
                      select q).SingleOrDefault();

                SO = (from q in db.LG_StockSavings
                      where q.C_REFF == structure.Fields.Noform
                      select q).Take(1).SingleOrDefault();

                FM1 = (from q in db.FA_MasItms
                       where q.c_iteno == structure.Fields.Item &&
                             q.c_nosup == SO.C_NOSUP
                       select q).SingleOrDefault();

                if (FM1 == null)
                {
                    result = "Tidak terdapat kode item " + structure.Fields.Item + " pada principle " + SO.V_NAMA;

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }   

                for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                {
                    field = structure.Fields.Field[nLoop];

                    if (structure.Fields.Noform.Substring(structure.Fields.Noform.Length-1,1) == "G")
                    {
                        SO = new LG_StockSaving()
                        {
                            C_GDG           = Convert.ToChar(structure.Fields.Gudang),
                            C_NOSUP         = LD.c_nosup,
                            V_NAMA          = LD.v_nama,
                            C_KDDIVPRI      = FM2.c_kddivpri,
                            V_NMDIVPRI      = FM2.v_nmdivpri,
                            LOCATION        = "KOSONG",
                            C_ITENO         = FM.c_iteno,
                            V_ITNAM         = FM.v_itnam,
                            STBARANG        = "Stock Good",
                            C_BATCH         = structure.Fields.Batch,
                            QTYSYS          = 0,
                            SOQTY           = 0,
                            D_SOQTY         = DateTime.Now,
                            RECOUNT1        = 0,
                            D_RECOUNT1      = DateTime.Now,
                            RECOUNT2        = 0,
                            D_RECOUNT2      = DateTime.Now,
                            SELISIH         = 0,
                            D_EXPIRED       = field.ExpiredDateFormated,
                            Box             = FM.n_box,
                            D_UPDATEEXPIRED = field.ExpiredDateFormated,
                            C_ENTRY         = structure.Fields.Entry,
                            D_ENTRY         = DateTime.Now,
                            D_UPDATE        = DateTime.Now,
                            C_STAGE         = SO.C_STAGE,
                            C_REFF          = structure.Fields.Noform
                        };

                        db.LG_StockSavings.InsertOnSubmit(SO);
                    }

                    else
                    {
                        SO = new LG_StockSaving()
                        {
                            C_GDG           = Convert.ToChar(structure.Fields.Gudang),
                            C_NOSUP         = LD.c_nosup,
                            V_NAMA          = LD.v_nama,
                            C_KDDIVPRI      = FM2.c_kddivpri,
                            V_NMDIVPRI      = FM2.v_nmdivpri,
                            LOCATION        = "KOSONG",
                            C_ITENO         = FM.c_iteno,
                            V_ITNAM         = FM.v_itnam,
                            STBARANG        = "Stock Bad",
                            C_BATCH         = structure.Fields.Batch,
                            QTYSYS          = 0,
                            SOQTY           = 0,
                            D_SOQTY         = DateTime.Now,
                            RECOUNT1        = 0,
                            D_RECOUNT1      = DateTime.Now,
                            RECOUNT2        = 0,
                            D_RECOUNT2      = DateTime.Now,
                            SELISIH         = 0,
                            D_EXPIRED       = field.ExpiredDateFormated,
                            Box             = FM.n_box,
                            D_UPDATEEXPIRED = field.ExpiredDateFormated,
                            C_ENTRY         = structure.Fields.Entry,
                            D_ENTRY         = DateTime.Now,
                            D_UPDATE        = DateTime.Now,
                            C_STAGE         = SO.C_STAGE,
                            C_REFF          = structure.Fields.Noform

                        };

                        db.LG_StockSavings.InsertOnSubmit(SO);
                    }

                    #region Update Master Batch

                    batch = (from q in db.LG_MsBatches
                             where q.c_iteno == FM.c_iteno && q.c_batch == structure.Fields.Batch
                             select q).Take(1).SingleOrDefault();

                    if (batch == null)
                    {
                        batch = new LG_MsBatch()
                        {
                            c_batch     = structure.Fields.Batch,
                            c_entry     = structure.Fields.Entry,
                            c_iteno     = FM.c_iteno,
                            c_update    = structure.Fields.Entry,
                            d_entry     = DateTime.Now,
                            d_expired   = field.ExpiredDateFormated,
                            d_update    = DateTime.Now
                        };

                        db.LG_MsBatches.InsertOnSubmit(batch);

                    }
                    else
                    {
                        batch.d_expired = field.ExpiredDateFormated;
                    }

                    #endregion

                    totalDetails++;
                }

                if (totalDetails > 0)
                {
                    result = string.Format("Total {0} detail(s)", totalDetails);

                    hasAnyChanges = true;
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

                result = string.Format("ScmsSoaLibrary.Bussiness.Proses:ProcessStokOpname - {0}", ex.Message);

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

        public string StokOpnameAdjust(ScmsSoaLibrary.Parser.Class.StockOpnameStructure structure)
        {
            return StokOpnameAdjust(structure, null, false);
        }

        public string StokOpnameAdjust(ScmsSoaLibrary.Parser.Class.StockOpnameStructure structure, ORMDataContext dbContext, bool isRemoveSisa)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

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

            ScmsSoaLibrary.Parser.Class.StockOpnameStructureField field = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime dateSO = DateTime.MinValue;

            LG_StockSaving SO = null;
            LG_AdjustH adjH = null;
            LG_AdjustD1 adjD1 = null;
            LG_AdjustD2 adjD2 = null;
            LG_RNH rnh = null;
            LG_RND1 rnd1 = null;
            LG_ComboH CombH = null;
            FA_MasItm itm = null;
            LG_MsBatch batch = null;
            LG_TotQtyAfterSO TotSO = null;

            List<LG_StockSaving> listSO = null;
            List<LG_AdjustD1> lisAdjStock1 = null;
            List<LG_AdjustD2> lisAdjStock2 = null;
            List<LG_RNH> lisRnh = null;
            List<LG_RND1> lisRnd1, lisRnd1Delete = null;
            List<LG_ComboH> lisCombH, lisCombHDelete = null;
            List<LG_RND2> lisRnd2 = null;
            List<LG_Adjust_RND1> lisadjrnd1 = null;
            List<LG_BatQtyAfterSO> lisBatAfter = null;
            List<LG_TotQtyAfterSO> lisTotAfter = null;

            int nLoop = 0;

            IDictionary<string, string> dic = null;

            int totalDetails = 0, nLoopC = 0;

            DateTime date = DateTime.Now;
            string adjID, 
                   rnID, 
                   NoForm = null, 
                   Entry = null;
            
            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                adjID = Commons.GenerateNumbering<LG_AdjustH>(db, "AJ", '3', "15", date, "c_adjno");

                #region Adjustment Header

                adjH = new LG_AdjustH()
                {
                    c_adjno     = adjID,
                    c_entry     = structure.Fields.Entry,
                    c_gdg       = Convert.ToChar(structure.Fields.Gudang),
                    c_type      = "03",
                    c_update    = structure.Fields.Entry,
                    d_adjdate   = DateTime.Now,
                    d_entry     = date,
                    d_update    = date,
                    l_delete    = false,
                    v_ket       = "SO " + DateTime.Now
                };

                #endregion

                db.LG_AdjustHs.InsertOnSubmit(adjH);

                #region Receive Note Header
                
                rnID = Commons.GenerateNumbering<LG_RNH>(db, "RN", '3', "03", date, "c_rnno");

                rnh = new LG_RNH()
                {
                    c_gdg       = Convert.ToChar(structure.Fields.Gudang),
                    c_rnno      = rnID,
                    d_rndate    = DateTime.Now,
                    c_type      = Convert.ToChar(structure.Fields.Gudang) == '1' ? "01" : "05",
                    l_float     = false,
                    c_dono      = "DOADJSO",
                    d_dodate    = DateTime.Now,
                    v_ket       = "RN Adjusment Stock, DOADJSO",
                    c_from      = structure.Fields.Principal,
                    n_bea       = 0,
                    l_print     = true,
                    l_status    = true,
                    c_entry     = structure.Fields.Entry,
                    d_entry     = DateTime.Now,
                    c_update    = structure.Fields.Entry,
                    d_update    = DateTime.Now,
                    l_delete    = null,
                    v_ket_mark  = null,
                    l_khusus    = null,
                    l_rnkhusus  = null,
                    c_po_outlet = null
                };

                db.LG_RNHs.InsertOnSubmit(rnh);
                

                #endregion

                Entry = structure.Fields.Entry;

                dic = new Dictionary<string, string>();

                if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                {
                    listSO = new List<LG_StockSaving>();
                    lisRnh = new List<LG_RNH>();
                    lisRnd1 = new List<LG_RND1>();
                    lisRnd2 = new List<LG_RND2>();
                    lisAdjStock1 = new List<LG_AdjustD1>();
                    lisAdjStock2 = new List<LG_AdjustD2>();
                    lisadjrnd1 = new List<LG_Adjust_RND1>();

                    #region Stage 1

                    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    {
                        field = structure.Fields.Field[nLoop];

                        SO = (from q in db.LG_StockSavings
                              where q.C_GDG     == Convert.ToChar(field.Gudang) &&
                                    q.V_NAMA    == field.Principal &&
                                    q.C_ITENO   == field.KdBarang &&
                                    q.C_BATCH   == field.Batch &&
                                    q.STBARANG  == field.StBarang &&
                                    q.C_STAGE   == 3
                              select q).Take(1).SingleOrDefault();

                        if (SO != null)
                        {

                            #region Set Variable Value

                            NoForm      = SO.C_REFF;

                            #endregion

                            #region Update Stage SO dan Get No. Adjustment

                            SO.C_STAGE = SO.C_STAGE + 1;
                            SO.NOADJ   = adjID;

                            #endregion

                            #region Delete Qty Receive Note Detail 1
                            //Selisih 0 ikut adjusment
                            //if (field.Selisih != 0)
                            //{
                            itm = (from q in db.FA_MasItms
                                   where q.c_iteno == field.KdBarang                                         
                                   select q).Take(1).SingleOrDefault();
                            
                            if(itm.l_combo == false)
                            {
                                #region Item Reguler

                                if (field.StBarang == "Stock Bad")
                                {
                                    lisRnd1Delete = (from q in db.LG_RND1s
                                                    where q.c_gdg   == Convert.ToChar(field.Gudang) &&
                                                          q.c_iteno == field.KdBarang &&
                                                          q.c_batch == field.Batch &&
                                                          q.n_bsisa != 0
                                                          select q).ToList();

                                    for (nLoopC = 0; nLoopC < lisRnd1Delete.Count; nLoopC++)
                                    {
                                        rnd1 = (from q in db.LG_RND1s
                                                where q.c_gdg   == Convert.ToChar(field.Gudang) &&
                                                      q.c_rnno  == lisRnd1Delete[nLoopC].c_rnno &&
                                                      q.c_iteno == lisRnd1Delete[nLoopC].c_iteno &&
                                                      q.c_batch == lisRnd1Delete[nLoopC].c_batch
                                                select q).Take(1).SingleOrDefault();

                                        if (rnd1 != null)
                                        {
                                            #region BackupUp RND1

                                            lisadjrnd1.Add(new LG_Adjust_RND1()
                                            {
                                                c_gdg   = Convert.ToChar(structure.Fields.Gudang),
                                                c_adjno = adjID,
                                                c_rnno  = lisRnd1Delete[nLoopC].c_rnno,
                                                c_iteno = lisRnd1Delete[nLoopC].c_iteno,
                                                c_batch = lisRnd1Delete[nLoopC].c_batch,
                                                n_gqty  = lisRnd1Delete[nLoopC].n_gqty.Value,
                                                n_bqty  = lisRnd1Delete[nLoopC].n_bqty.Value,
                                                n_gsisa = lisRnd1Delete[nLoopC].n_gsisa.Value,
                                                n_bsisa = lisRnd1Delete[nLoopC].n_bsisa.Value
                                            });

                                            #endregion

                                            rnd1.n_bsisa = 0;
                                        }
                                    }
                                }
                                else
                                {
                                    lisRnd1Delete = (from q in db.LG_RND1s
                                                     where q.c_gdg   == Convert.ToChar(field.Gudang) &&
                                                           q.c_iteno == field.KdBarang &&
                                                           q.c_batch == field.Batch &&
                                                           (q.n_gsisa > 0 || q.n_bsisa > 0)
                                                     select q).ToList();

                                    for (nLoopC = 0; nLoopC < lisRnd1Delete.Count; nLoopC++)
                                    {
                                        rnd1 = (from q in db.LG_RND1s
                                                where q.c_gdg   == Convert.ToChar(field.Gudang) &&
                                                      q.c_rnno  == lisRnd1Delete[nLoopC].c_rnno &&
                                                      q.c_iteno == lisRnd1Delete[nLoopC].c_iteno &&
                                                      q.c_batch == lisRnd1Delete[nLoopC].c_batch
                                                select q).Take(1).SingleOrDefault();

                                        if (rnd1 != null)
                                        {
                                            #region BackUp RND1

                                            lisadjrnd1.Add(new LG_Adjust_RND1()
                                            {
                                                c_gdg   = Convert.ToChar(structure.Fields.Gudang),
                                                c_adjno = adjID,
                                                c_rnno  = lisRnd1Delete[nLoopC].c_rnno,
                                                c_iteno = lisRnd1Delete[nLoopC].c_iteno,
                                                c_batch = lisRnd1Delete[nLoopC].c_batch,
                                                n_gqty  = lisRnd1Delete[nLoopC].n_gqty.Value,
                                                n_bqty  = lisRnd1Delete[nLoopC].n_bqty.Value,
                                                n_gsisa = lisRnd1Delete[nLoopC].n_gsisa.Value,
                                                n_bsisa = lisRnd1Delete[nLoopC].n_bsisa.Value
                                            });

                                            #endregion

                                            rnd1.n_gsisa = 0;
                                        }
                                    }
                                }

                                #endregion
                                //}
                            }

                            else
                            {
                                #region Item Combo

                                if (field.StBarang == "Stock Bad")
                                {
                                    lisCombHDelete = (from q in db.LG_ComboHs
                                                      where q.c_gdg   == Convert.ToChar(field.Gudang) &&
                                                            q.c_iteno == field.KdBarang &&
                                                            q.c_batch == field.Batch &&
                                                            q.n_bsisa > 0
                                                      select q).ToList();

                                    for (nLoopC = 0; nLoopC < lisCombHDelete.Count; nLoopC++)
                                    {
                                        CombH = (from q in db.LG_ComboHs
                                                where q.c_gdg       == Convert.ToChar(field.Gudang) &&
                                                      q.c_combono   == lisCombHDelete[nLoopC].c_combono &&
                                                      q.c_iteno     == lisCombHDelete[nLoopC].c_iteno &&
                                                      q.c_batch     == lisCombHDelete[nLoopC].c_batch
                                                select q).Take(1).SingleOrDefault();

                                        if (CombH != null)
                                        {
                                            #region BackupUp RND1

                                            lisadjrnd1.Add(new LG_Adjust_RND1()
                                            {
                                                c_gdg   = Convert.ToChar(structure.Fields.Gudang),
                                                c_adjno = adjID,
                                                c_rnno  = lisCombHDelete[nLoopC].c_combono,
                                                c_iteno = lisCombHDelete[nLoopC].c_iteno,
                                                c_batch = lisCombHDelete[nLoopC].c_batch,
                                                n_gqty  = lisCombHDelete[nLoopC].n_gqty.Value,
                                                n_bqty  = lisCombHDelete[nLoopC].n_bqty.Value,
                                                n_gsisa = lisCombHDelete[nLoopC].n_gsisa.Value,
                                                n_bsisa = lisCombHDelete[nLoopC].n_bsisa.Value
                                            });

                                            #endregion

                                            CombH.n_bsisa = 0;
                                        }
                                    }
                                }
                                else
                                {
                                    lisCombHDelete = (from q in db.LG_ComboHs
                                                      where q.c_gdg   == Convert.ToChar(field.Gudang) &&
                                                            q.c_iteno == field.KdBarang &&
                                                            q.c_batch == field.Batch &&
                                                            q.n_gsisa > 0
                                                      select q).ToList();

                                    for (nLoopC = 0; nLoopC < lisCombHDelete.Count; nLoopC++)
                                    {
                                        CombH = (from q in db.LG_ComboHs
                                                 where q.c_gdg       == Convert.ToChar(field.Gudang) &&
                                                       q.c_combono   == lisCombHDelete[nLoopC].c_combono &&
                                                       q.c_iteno     == lisCombHDelete[nLoopC].c_iteno &&
                                                       q.c_batch     == lisCombHDelete[nLoopC].c_batch
                                                 select q).Take(1).SingleOrDefault();

                                        if (CombH != null)
                                        {
                                            #region BackUp RND1

                                            lisadjrnd1.Add(new LG_Adjust_RND1()
                                            {
                                                c_gdg   = Convert.ToChar(structure.Fields.Gudang),
                                                c_adjno = adjID,
                                                c_rnno  = lisCombHDelete[nLoopC].c_combono,
                                                c_iteno = lisCombHDelete[nLoopC].c_iteno,
                                                c_batch = lisCombHDelete[nLoopC].c_batch,
                                                n_gqty  = lisCombHDelete[nLoopC].n_gqty.Value,
                                                n_bqty  = lisCombHDelete[nLoopC].n_bqty.Value,
                                                n_gsisa = lisCombHDelete[nLoopC].n_gsisa.Value,
                                                n_bsisa = lisCombHDelete[nLoopC].n_bsisa.Value
                                            });

                                            #endregion

                                            CombH.n_gsisa = 0;
                                        }
                                    }
                                }

                                #endregion

                                #region Item Reguler

                                if (field.StBarang == "Stock Bad")
                                {
                                    lisRnd1Delete = (from q in db.LG_RND1s
                                                    where q.c_gdg   == Convert.ToChar(field.Gudang) &&
                                                          q.c_iteno == field.KdBarang &&
                                                          q.c_batch == field.Batch
                                                          select q).ToList();

                                    for (nLoopC = 0; nLoopC < lisRnd1Delete.Count; nLoopC++)
                                    {
                                        rnd1 = (from q in db.LG_RND1s
                                                where q.c_gdg   == Convert.ToChar(field.Gudang) &&
                                                      q.c_rnno  == lisRnd1Delete[nLoopC].c_rnno &&
                                                      q.c_iteno == lisRnd1Delete[nLoopC].c_iteno &&
                                                      q.c_batch == lisRnd1Delete[nLoopC].c_batch
                                                select q).Take(1).SingleOrDefault();

                                        if (rnd1 != null)
                                        {
                                            #region BackupUp RND1

                                            lisadjrnd1.Add(new LG_Adjust_RND1()
                                            {
                                                c_gdg   = Convert.ToChar(structure.Fields.Gudang),
                                                c_adjno = adjID,
                                                c_rnno  = lisRnd1Delete[nLoopC].c_rnno,
                                                c_iteno = lisRnd1Delete[nLoopC].c_iteno,
                                                c_batch = lisRnd1Delete[nLoopC].c_batch,
                                                n_gqty  = lisRnd1Delete[nLoopC].n_gqty.Value,
                                                n_bqty  = lisRnd1Delete[nLoopC].n_bqty.Value,
                                                n_gsisa = lisRnd1Delete[nLoopC].n_gsisa.Value,
                                                n_bsisa = lisRnd1Delete[nLoopC].n_bsisa.Value
                                            });

                                            #endregion

                                            rnd1.n_bsisa = 0;
                                        }
                                    }
                                }
                                else
                                {
                                    lisRnd1Delete = (from q in db.LG_RND1s
                                                     where q.c_gdg   == Convert.ToChar(field.Gudang) &&
                                                           q.c_iteno == field.KdBarang &&
                                                           q.c_batch == field.Batch 
                                                     select q).ToList();

                                    for (nLoopC = 0; nLoopC < lisRnd1Delete.Count; nLoopC++)
                                    {
                                        rnd1 = (from q in db.LG_RND1s
                                                where q.c_gdg   == Convert.ToChar(field.Gudang) &&
                                                      q.c_rnno  == lisRnd1Delete[nLoopC].c_rnno &&
                                                      q.c_iteno == lisRnd1Delete[nLoopC].c_iteno &&
                                                      q.c_batch == lisRnd1Delete[nLoopC].c_batch
                                                select q).Take(1).SingleOrDefault();

                                        if (rnd1 != null)
                                        {
                                            #region BackUp RND1

                                            lisadjrnd1.Add(new LG_Adjust_RND1()
                                            {
                                                c_gdg   = Convert.ToChar(structure.Fields.Gudang),
                                                c_adjno = adjID,
                                                c_rnno  = lisRnd1Delete[nLoopC].c_rnno,
                                                c_iteno = lisRnd1Delete[nLoopC].c_iteno,
                                                c_batch = lisRnd1Delete[nLoopC].c_batch,
                                                n_gqty  = lisRnd1Delete[nLoopC].n_gqty.Value,
                                                n_bqty  = lisRnd1Delete[nLoopC].n_bqty.Value,
                                                n_gsisa = lisRnd1Delete[nLoopC].n_gsisa.Value,
                                                n_bsisa = lisRnd1Delete[nLoopC].n_bsisa.Value
                                            });

                                            #endregion

                                            rnd1.n_gsisa = 0;
                                        }
                                    }
                                }

                                #endregion
                            }

                            #endregion


                            #region Receive Note Detail 1
                            //Selisih 0 ikut adjusment
                            //if (field.Selisih != 0)
                            //{
                            #region old
                            /*
                            if(itm.l_combo == false)
                            {
                                #region Item Reguler

                                if (field.StBarang == "Stock Bad")
                                {
                                    rnd1 = (from q in db.LG_RND1s
                                            where q.c_rnno  == rnID &&
                                                  q.c_gdg   == Convert.ToChar(structure.Fields.Gudang) &&
                                                  q.c_iteno == field.KdBarang &&
                                                  q.c_batch == field.Batch
                                            select q).Take(1).SingleOrDefault();

                                    if (rnd1 == null)
                                    {
                                        lisRnd1.Add(new LG_RND1()
                                        {
                                            c_gdg   = Convert.ToChar(structure.Fields.Gudang),
                                            c_rnno  = rnID,
                                            c_iteno = field.KdBarang,
                                            c_batch = field.Batch,
                                            n_gqty  = 0,
                                            n_bqty  = 0,
                                            n_gsisa = 0,
                                            n_bsisa = field.Recount2
                                        });
                                    }
                                    else
                                    {
                                        rnd1.n_bsisa = field.Recount2;
                                    }                                
                                }
                                else
                                {
                                    rnd1 = (from q in db.LG_RND1s
                                            where q.c_rnno  == rnID &&
                                                  q.c_gdg   == Convert.ToChar(structure.Fields.Gudang) &&
                                                  q.c_iteno == field.KdBarang &&
                                                  q.c_batch == field.Batch
                                            select q).Take(1).SingleOrDefault();

                                    if (rnd1 == null)
                                    {
                                        lisRnd1.Add(new LG_RND1()
                                        {
                                            c_gdg   = Convert.ToChar(structure.Fields.Gudang),
                                            c_rnno  = rnID,
                                            c_iteno = field.KdBarang,
                                            c_batch = field.Batch,
                                            n_gqty  = 0,
                                            n_bqty  = 0,
                                            n_gsisa = field.Recount2,
                                            n_bsisa = 0
                                        });
                                    }
                                    else
                                    {
                                        rnd1.n_gsisa = field.Recount2;
                                    }
                                }

                                #endregion
                                //}
                            }
                            else
                            {
                                #region Item Combo

                                var CombH2 = (from q in db.LG_ComboHs
                                              where q.c_gdg == Convert.ToChar(structure.Fields.Gudang) &&
                                                    q.c_iteno == field.KdBarang &&
                                                    q.c_batch == field.Batch
                                              select q.c_combono).Max();

                                CombH = (from q in db.LG_ComboHs
                                        where q.c_gdg       == Convert.ToChar(structure.Fields.Gudang) &&
                                              q.c_combono   == CombH2 &&
                                              q.c_iteno     == field.KdBarang &&
                                              q.c_batch     == field.Batch
                                        select q).Take(1).SingleOrDefault();

                                if (CombH != null)
                                {                                    
                                    if (field.StBarang == "Stock Bad")
                                    {
                                        CombH.n_bsisa = field.Recount2;
                                    }
                                    else
                                    {
                                        CombH.n_gsisa = field.Recount2;
                                    }
                                }

                                

                                #endregion
                            }
                            */

                            #endregion

                            #region Item Reguler

                            if (field.StBarang == "Stock Bad")
                            {
                                rnd1 = (from q in db.LG_RND1s
                                        where q.c_rnno  == rnID &&
                                              q.c_gdg   == Convert.ToChar(structure.Fields.Gudang) &&
                                              q.c_iteno == field.KdBarang &&
                                              q.c_batch == field.Batch
                                        select q).Take(1).SingleOrDefault();

                                if (rnd1 == null)
                                {
                                    lisRnd1.Add(new LG_RND1()
                                    {
                                        c_gdg   = Convert.ToChar(structure.Fields.Gudang),
                                        c_rnno  = rnID,
                                        c_iteno = field.KdBarang,
                                        c_batch = field.Batch,
                                        n_gqty  = 0,
                                        n_bqty  = 0,
                                        n_gsisa = 0,
                                        n_bsisa = field.Recount2
                                    });
                                }
                                else
                                {
                                    rnd1.n_bsisa = field.Recount2;
                                }                                
                            }
                            else
                            {
                                rnd1 = (from q in db.LG_RND1s
                                        where q.c_rnno  == rnID &&
                                              q.c_gdg   == Convert.ToChar(structure.Fields.Gudang) &&
                                              q.c_iteno == field.KdBarang &&
                                              q.c_batch == field.Batch
                                        select q).Take(1).SingleOrDefault();

                                if (rnd1 == null)
                                {
                                    lisRnd1.Add(new LG_RND1()
                                    {
                                        c_gdg   = Convert.ToChar(structure.Fields.Gudang),
                                        c_rnno  = rnID,
                                        c_iteno = field.KdBarang,
                                        c_batch = field.Batch,
                                        n_gqty  = 0,
                                        n_bqty  = 0,
                                        n_gsisa = field.Recount2,
                                        n_bsisa = 0
                                    });
                                }
                                else
                                {
                                    rnd1.n_gsisa = field.Recount2;
                                }
                            }

                            #endregion

                            #endregion

                            #region Adjustment Detail
                            //Selisih 0 ikut adjusment
                            //if (field.Selisih != 0)
                            //{
                                #region Detil 1

                                if (field.StBarang == "Stock Bad")                                
                                {
                                    adjD1 = (from q in db.LG_AdjustD1s
                                            where q.c_adjno == adjID &&
                                                  q.c_gdg   == Convert.ToChar(structure.Fields.Gudang) &&
                                                  q.c_iteno == field.KdBarang &&
                                                  q.c_batch == field.Batch
                                            select q).Take(1).SingleOrDefault();

                                    if (adjD1 == null)
                                    {
                                        lisAdjStock1.Add(new LG_AdjustD1()
                                        {
                                            c_gdg   = Convert.ToChar(structure.Fields.Gudang),
                                            c_adjno = adjID,
                                            c_iteno = field.KdBarang,
                                            c_batch = field.Batch,
                                            n_gqty  = 0,
                                            n_bqty  = field.Selisih,
                                            v_ket   = "SO " + DateTime.Now
                                        });
                                    }
                                    else
                                    {
                                        adjD1.n_bqty = field.Selisih;

                                    }                                       
                                }
                                else 
                                {
                                    adjD1 = (from q in db.LG_AdjustD1s
                                             where q.c_adjno == adjID &&
                                                   q.c_gdg   == Convert.ToChar(structure.Fields.Gudang) &&
                                                   q.c_iteno == field.KdBarang &&
                                                   q.c_batch == field.Batch
                                             select q).Take(1).SingleOrDefault();

                                    if (adjD1 == null)
                                    {
                                        lisAdjStock1.Add(new LG_AdjustD1()
                                        {
                                            c_gdg   = Convert.ToChar(structure.Fields.Gudang),
                                            c_adjno = adjID,
                                            c_iteno = field.KdBarang,
                                            c_batch = field.Batch,
                                            n_gqty  = field.Selisih,
                                            n_bqty  = 0,
                                            v_ket   = "SO " + DateTime.Now
                                        });
                                    }
                                    else
                                    {
                                        adjD1.n_gqty = field.Selisih;

                                    }
                                }                            

                                #endregion

                                #region Detil 2

                                if (field.StBarang == "Stock Bad")
                                {
                                    adjD2 = (from q in db.LG_AdjustD2s
                                             where q.c_adjno == adjID &&
                                                   q.c_gdg   == Convert.ToChar(structure.Fields.Gudang) &&
                                                   q.c_iteno == field.KdBarang &&
                                                   q.c_batch == field.Batch
                                             select q).Take(1).SingleOrDefault();

                                    if (adjD2 == null)
                                    {
                                        lisAdjStock2.Add(new LG_AdjustD2()
                                        {
                                            c_gdg   = Convert.ToChar(structure.Fields.Gudang),
                                            c_adjno = adjID,
                                            c_iteno = field.KdBarang,
                                            c_batch = field.Batch,
                                            c_rnno  = rnID,
                                            n_gqty  = 0,
                                            n_bqty  = field.Selisih
                                        });
                                    }
                                    else
                                    {
                                        adjD2.n_bqty = field.Selisih;
                                    }
                                }
                                else
                                {
                                    adjD2 = (from q in db.LG_AdjustD2s
                                             where q.c_adjno == adjID &&
                                                   q.c_gdg   == Convert.ToChar(structure.Fields.Gudang) &&
                                                   q.c_iteno == field.KdBarang &&
                                                   q.c_batch == field.Batch
                                             select q).Take(1).SingleOrDefault();

                                    if (adjD2 == null)
                                    {
                                        lisAdjStock2.Add(new LG_AdjustD2()
                                        {
                                            c_gdg   = Convert.ToChar(structure.Fields.Gudang),
                                            c_adjno = adjID,
                                            c_iteno = field.KdBarang,
                                            c_batch = field.Batch,
                                            c_rnno  = rnID,
                                            n_gqty  = field.Selisih,
                                            n_bqty  = 0
                                        });
                                    }
                                    else
                                    {
                                        adjD2.n_gqty = field.Selisih;

                                    }
                                }

                                #endregion
                            //}

                            #endregion

                            #region Update Master Batch

                            if (SO.D_EXPIRED != SO.D_UPDATEEXPIRED)
                            {
                                batch = (from q in db.LG_MsBatches
                                         where q.c_iteno == field.KdBarang && q.c_batch == field.Batch
                                         select q).Take(1).SingleOrDefault();

                                if (batch == null)
                                {
                                    batch = new LG_MsBatch()
                                    {
                                        c_batch   = field.Batch,
                                        c_entry   = structure.Fields.Entry,
                                        c_iteno   = field.KdBarang,
                                        c_update  = structure.Fields.Entry,
                                        d_entry   = DateTime.Now,
                                        d_expired = field.ExpiredDateFormated,
                                        d_update  = DateTime.Now
                                    };

                                    db.LG_MsBatches.InsertOnSubmit(batch);

                                }
                                else
                                {
                                    batch.d_expired = field.ExpiredDateFormated;
                                }
                            }

                            #endregion
                        }

                        else
                        {
                            result = "Kode item : " + field.KdBarang + " dengan batch : " + field.Batch + " tidak ditemukan. Adjusment SO dibatalkan(Set Adjusment)";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }

                        totalDetails++;

                        #region Data saving

                        if (listSO.Count > 0)
                        {
                            db.LG_StockSavings.InsertAllOnSubmit(listSO.ToArray());
                            listSO.Clear();
                        }
                        if (lisRnh.Count > 0)
                        {
                            db.LG_RNHs.InsertAllOnSubmit(lisRnh.ToArray());
                            lisRnh.Clear();
                        }
                        if (lisRnd1.Count > 0)
                        {
                            db.LG_RND1s.InsertAllOnSubmit(lisRnd1.ToArray());
                            lisRnd1.Clear();
                            lisRnd1Delete.Clear();
                        }
                        if (lisadjrnd1.Count > 0)
                        {
                            db.LG_Adjust_RND1s.InsertAllOnSubmit(lisadjrnd1.ToArray());
                            lisadjrnd1.Clear();
                        }
                        if (lisAdjStock1.Count > 0)
                        {
                            db.LG_AdjustD1s.InsertAllOnSubmit(lisAdjStock1.ToArray());
                            lisAdjStock1.Clear();
                        }
                        if (lisAdjStock2.Count > 0)
                        {
                            db.LG_AdjustD2s.InsertAllOnSubmit(lisAdjStock2.ToArray());
                            lisAdjStock2.Clear();
                        }

                        db.SubmitChanges();

                        #endregion
                    }

                    #endregion

                    #region Stage 2

                    List<StockOpname> listStockOpname = null;
                    lisBatAfter = new List<LG_BatQtyAfterSO>();
                    lisTotAfter = new List<LG_TotQtyAfterSO>();

                    listStockOpname = (from q in db.Vw_StockOpnames
                                       where q.Gudang       == Convert.ToChar(structure.Fields.Gudang) &&
                                             q.KdPrincipal  == structure.Fields.Principal &&
                                             q.Reff         == NoForm

                                       select new StockOpname()
                                       {    
                                           C_BatQtyAfterSO  = string.Concat(date.Year.ToString().Substring(2,2),date.Month.ToString(),date.Day.ToString(),q.Reff),
                                           S_TAHUN          = date.Year.ToString(),
                                           T_BULAN          = date.Month.ToString(),
                                           D_TGL            = date.Day.ToString(),
                                           C_GDG            = q.Gudang.ToString(),
                                           C_NOSUP          = q.KdPrincipal,
                                           C_KDDIVPRI       = q.KdDivPrincipal,
                                           LOCATION         = "",
                                           C_ITENO          = q.KdBarang,
                                           STBARANG         = q.StBarang,
                                           C_BATCH          = q.Batch,
                                           QTYSYS           = q.QtySys.Value,
                                           D_EXPIRED        = q.Expired.Value,
                                           C_ENTRY          = structure.Fields.Entry,
                                           D_ENTRY          = DateTime.Now,
                                           C_REFF           = q.Reff

                                       }).ToList();


                    for (nLoopC = 0; nLoopC < listStockOpname.Count; nLoopC++)
                    {
                        lisBatAfter.Add(new LG_BatQtyAfterSO()
                        {   
                            C_BatQtyAfterSO  = listStockOpname[nLoopC].C_BatQtyAfterSO,
                            S_TAHUN          = listStockOpname[nLoopC].S_TAHUN,
                            T_BULAN          = listStockOpname[nLoopC].T_BULAN,
                            D_TGL            = listStockOpname[nLoopC].D_TGL,
                            C_GDG            = Convert.ToChar(listStockOpname[nLoopC].C_GDG),
                            C_NOSUP          = listStockOpname[nLoopC].C_NOSUP,
                            C_KDDIVPRI       = listStockOpname[nLoopC].C_KDDIVPRI,
                            LOCATION         = "",
                            C_ITENO          = listStockOpname[nLoopC].C_ITENO,
                            STBARANG         = listStockOpname[nLoopC].STBARANG,
                            C_BATCH          = listStockOpname[nLoopC].C_BATCH,
                            QTYSYS           = listStockOpname[nLoopC].QTYSYS,
                            D_EXPIRED        = listStockOpname[nLoopC].D_EXPIRED,
                            C_ENTRY          = listStockOpname[nLoopC].C_ENTRY,
                            D_ENTRY          = listStockOpname[nLoopC].D_ENTRY,
                            C_REFF           = listStockOpname[nLoopC].C_REFF
                        });

                        TotSO = (from q in db.LG_TotQtyAfterSOs
                                 where q.C_GDG      == Convert.ToChar(listStockOpname[nLoopC].C_GDG) &&
                                       q.C_ITENO    == listStockOpname[nLoopC].C_ITENO &&
                                       q.STBARANG   == listStockOpname[nLoopC].STBARANG &&
                                       q.S_TAHUN    == listStockOpname[nLoopC].S_TAHUN &&
                                       q.T_BULAN    == listStockOpname[nLoopC].T_BULAN &&
                                       q.D_TGL      == listStockOpname[nLoopC].D_TGL
                                 select q).Take(1).SingleOrDefault();

                        if (TotSO == null)
                        {
                            lisTotAfter.Add(new LG_TotQtyAfterSO()
                            {
                                C_TotQtyAfterSO  = listStockOpname[nLoopC].C_BatQtyAfterSO,
                                S_TAHUN          = listStockOpname[nLoopC].S_TAHUN,
                                T_BULAN          = listStockOpname[nLoopC].T_BULAN,
                                D_TGL            = listStockOpname[nLoopC].D_TGL,
                                C_GDG            = Convert.ToChar(listStockOpname[nLoopC].C_GDG),
                                C_NOSUP          = listStockOpname[nLoopC].C_NOSUP,
                                C_KDDIVPRI       = listStockOpname[nLoopC].C_KDDIVPRI,
                                LOCATION         = "",
                                C_ITENO          = listStockOpname[nLoopC].C_ITENO,
                                STBARANG         = listStockOpname[nLoopC].STBARANG,
                                QTYSYS           = listStockOpname[nLoopC].QTYSYS,
                                C_ENTRY          = listStockOpname[nLoopC].C_ENTRY,
                                D_ENTRY          = listStockOpname[nLoopC].D_ENTRY,
                                C_REFF           = listStockOpname[nLoopC].C_REFF
                            });

                            if (lisTotAfter.Count > 0)
                            {
                                
                                db.LG_TotQtyAfterSOs.InsertAllOnSubmit(lisTotAfter.ToArray());
                                lisTotAfter.Clear();
                                db.SubmitChanges();
                            }
                        }
                        else
                        {
                            TotSO.QTYSYS += listStockOpname[nLoopC].QTYSYS;
                        }

                    }

                    if (lisBatAfter.Count > 0)
                    {
                        db.LG_BatQtyAfterSOs.InsertAllOnSubmit(lisBatAfter.ToArray());
                        lisadjrnd1.Clear();
                    }

                    db.SubmitChanges();

                    #endregion
                }

                if (totalDetails > 0)
                {
                    dic.Add("STOCK OPNAME", "0");
                    dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                    result = string.Format("Total {0} detail(s)", totalDetails);

                    hasAnyChanges = true;
                }

                if (hasAnyChanges)
                {
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

                result = string.Format("ScmsSoaLibrary.Bussiness.Proses:ProcessStokOpname - {0}", ex.Message);

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

        public string StokOpnameSOUlang(ScmsSoaLibrary.Parser.Class.StockOpnameStructure structure)
        {
            return StokOpnameSOUlang(structure, null, false);
        }

        public string StokOpnameSOUlang(ScmsSoaLibrary.Parser.Class.StockOpnameStructure structure, ORMDataContext dbContext, bool isRemoveSisa)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

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

            ScmsSoaLibrary.Parser.Class.StockOpnameStructureField field = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime dateSO = DateTime.MinValue;

            List<LG_StockSaving> listSO = null;
            List<LG_StockSaving_Adjust> listSOAdj = null;

            LG_StockSaving SO = null;

            int nLoop = 0;

            IDictionary<string, string> dic = null;

            int totalDetails = 0;

            DateTime date = DateTime.Now;

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                dic = new Dictionary<string, string>();

                

                if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                {
                    listSOAdj = new List<LG_StockSaving_Adjust>();
                    listSO = new List<LG_StockSaving>();

                    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    {
                        field = structure.Fields.Field[nLoop];

                        SO = (from q in db.LG_StockSavings
                              where q.C_GDG     == Convert.ToChar(field.Gudang) &&
                                    q.V_NAMA    == field.Principal &&
                                    q.C_ITENO   == field.KdBarang &&
                                    q.C_BATCH   == field.Batch &&
                                    q.STBARANG  == field.StBarang &&
                                    q.C_REFF    == structure.Fields.Noform
                              select q).Take(1).SingleOrDefault();

                        var TglSO = (from q in db.LG_StockSavings
                                     where q.C_REFF     == structure.Fields.Noform
                                     select q.D_ENTRY).Min();
                        
                        if (SO != null)
                        {   
                            listSOAdj.Add(new LG_StockSaving_Adjust()
                            {
                                C_GDG           = Convert.ToChar(field.Gudang),
                                D_SANO          = TglSO,
                                C_NOSUP         = field.KdPrincipal,
                                V_NAMA          = field.Principal,
                                C_KDDIVPRI      = field.KdDivPrincipal,
                                V_NMDIVPRI      = field.DivPrincipal,
                                LOCATION        = field.Location,
                                C_ITENO         = field.KdBarang,
                                V_ITNAM         = field.NmBarang,
                                STBARANG        = field.StBarang,
                                C_BATCH         = field.Batch,
                                QTYSYS          = field.QtySys,
                                SOQTY           = field.SOQty,
                                RECOUNT1        = field.Recount1,
                                RECOUNT2        = field.Recount2,
                                SELISIH         = field.Selisih,
                                D_EXPIRED       = field.ExpiredDateFormated,
                                Box             = field.Box,
                                C_REFF          = structure.Fields.Noform,
                                C_ENTRY         = structure.Fields.Entry,
                                D_ENTRY         = DateTime.Now,
                                NOADJ           = SO.NOADJ,
                            });
                        }

                        else
                        {
                            result = "Kode item : " + field.KdBarang + " dengan batch : " + field.Batch + " tidak ditemukan. SO ulang dibatalkan";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                        totalDetails++;

                    }

                    if (listSOAdj.Count > 0)
                    {

                        listSO = (from q in db.LG_StockSavings
                                  where q.C_REFF == structure.Fields.Noform
                                  select q).Distinct().ToList();

                        if ((listSO != null) && (listSO.Count > 0))
                        {
                            db.LG_StockSavings.DeleteAllOnSubmit(listSO.ToArray());
                            listSO.Clear();

                            totalDetails++;
                        }

                        db.LG_StockSaving_Adjusts.InsertAllOnSubmit(listSOAdj.ToArray());
                        listSOAdj.Clear();
                    }

                }

                if (totalDetails > 0)
                {
                    dic.Add("STOCK OPNAME", "0");
                    dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                    result = string.Format("Total {0} detail(s)", totalDetails);

                    hasAnyChanges = true;
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

                result = string.Format("ScmsSoaLibrary.Bussiness.Proses:ProcessStokOpname - {0}", ex.Message);

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

        #endregion  
      
        #region Proses Email Produk Kosong 20190411FM

        public string ProdukKosong(ScmsSoaLibrary.Parser.Class.ProdukKosongStructure structure)
        {
            return ProdukKosong(structure, null, false);
        }

        public string ProdukKosong(ScmsSoaLibrary.Parser.Class.ProdukKosongStructure structure, ORMDataContext dbContext, bool isRemoveSisa)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

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

            ScmsSoaLibrary.Parser.Class.ProdukKosongStructureFields field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            DateTime date = DateTime.Today;

            string idPK = null;

            IDictionary<string, string> dic = null;

            SCMS_ProdukKosong spk = null, 
                              spkModify = null, 
                              spkEmail = null,
                              spkDelete = null;
            FA_MasItm itm = null, itmAktif = null;
            LG_DatSup sup = null;
            FA_Divpri divsup = null;
            FA_MsDivPri nmdivsup = null;
            List<tblProdukKosong> tbProdukKosong = null;

            int totalDetails = 0;

            string Sender = "scms.dophar@ams.co.id";
            string TextSender = "PPIC Info";
            string Received = "";
            string CarbonCopy = "";
            string Subject = "";
            string EmailHeader = "";
            string EmailContent = "";
            string ProductContent = "";
            string EmailFooter = "";

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
                {
                    #region Add

                    #region Validasi

                    if (string.IsNullOrEmpty(structure.Fields.Produk))
                    {
                        result = "Produk belum diisi, harap isi produk terlebih dahulu.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    if (string.IsNullOrEmpty(structure.Fields.Entry))
                    {
                        result = "Nip penanggung jawab dibutuhkan, harap login ulang.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    itm = (from q in db.FA_MasItms
                           where q.c_iteno == structure.Fields.Produk
                           select q).Take(1).SingleOrDefault();

                    if (itm == null)
                    {
                        result = "Produk tidak terdaftar, simpan data dibatalkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    sup = (from q in db.LG_DatSups
                           where q.c_nosup == itm.c_nosup
                           select q).Take(1).SingleOrDefault();

                    if (sup == null)
                    {
                        result = "Principal tidak terdaftar, simpan data dibatalkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    divsup = (from q in db.FA_Divpris
                              where q.c_iteno == structure.Fields.Produk
                              select q).Take(1).SingleOrDefault();

                    if (divsup == null)
                    {
                        result = "Div. Principal tidak terdaftar, simpan data dibatalkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    nmdivsup = (from q in db.FA_MsDivPris
                                where q.c_kddivpri == divsup.c_kddivpri
                                select q).Take(1).SingleOrDefault();

                    if (nmdivsup == null)
                    {
                        result = "Div. Principal tidak terdaftar, simpan data dibatalkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    
                    #endregion

                    spk = (from q in db.SCMS_ProdukKosongs
                           where q.c_iteno  == structure.Fields.Produk &&
                                 q.l_aktif  == true &&
                                 q.l_delete == false &&
                                 q.c_tipe   == structure.Fields.Tipeinput
                           select q).Take(1).SingleOrDefault();

                    if (spk != null)
                    {
                        itmAktif = (from q in db.FA_MasItms
                                    where q.c_iteno == spk.c_iteno
                                    select q).Take(1).SingleOrDefault();

                        if (itm != null)
                        {
                            result = "Status produk " + itm.v_itnam + " aktif, simpan data dibatalkan.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                        else
                        {
                            result = "Produk " + itm.v_itnam + " tidak ditemukan, simpan data dibatalkan.";

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
                        idPK = Commons.GenerateNumbering<SCMS_ProdukKosong>(db, "PK", '8', "09", date, "c_pkno");

                        spk = new SCMS_ProdukKosong()
                        {
                            c_pkno      = idPK,
                            d_pkdate    = date.Date,
                            c_iteno     = structure.Fields.Produk,
                            c_entry     = structure.Fields.Entry,
                            d_entry     = DateTime.Now,
                            d_abe       = structure.Fields.ABE,
                            l_sent      = structure.Fields.SendEmail,
                            l_aktif     = true,
                            l_delete    = false,
                            c_tipe      = structure.Fields.Tipeinput
                        };

                        if (spk != null)
                        {
                            db.SCMS_ProdukKosongs.InsertOnSubmit(spk);

                            dic = new Dictionary<string, string>();

                            dic.Add("PKno", idPK);
                            dic.Add("Pemasok", sup.v_nama);
                            dic.Add("DivPemasok", nmdivsup.v_nmdivpri);

                            result = string.Format("Total {0} detail(s)", totalDetails);

                            hasAnyChanges   = true;
                            spk             = null;
                            itm             = null;
                            itmAktif        = null;
                            sup             = null;
                            divsup          = null;
                            nmdivsup        = null;
                        }
                    }

                    #endregion
                }
                if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    spkModify = (from q in db.SCMS_ProdukKosongs
                                 where q.c_pkno == structure.Fields.Pkno
                                 select q).Take(1).SingleOrDefault();

                    if (spkModify != null)
                    {
                        spkModify.d_abe     = structure.Fields.ABE;
                        spkModify.l_sent    = structure.Fields.SendEmail;
                        spkModify.l_aktif   = structure.Fields.Aktif;
                        spkModify.c_update  = structure.Fields.Entry;
                        spkModify.d_update  = DateTime.Now;

                        hasAnyChanges = true;
                        spkModify = null;
                    }
                    else
                    {
                        result = "Status produk tidak aktif, simpan data dibatalkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;

                    }

                    #endregion
                }
                if (structure.Method.Equals("Email", StringComparison.OrdinalIgnoreCase))
                {
                    #region Email

                    tbProdukKosong = (from q in db.SCMS_ProdukKosongs
                                      join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                      join q2 in db.LG_DatSups on q1.c_nosup equals q2.c_nosup
                                      join q3 in db.FA_Divpris on q1.c_iteno equals q3.c_iteno
                                      join q4 in db.FA_MsDivPris on q3.c_kddivpri equals q4.c_kddivpri
                                      join q5 in db.SCMS_USERs on q.c_entry equals q5.c_nip
                                      where q.l_aktif == true &&
                                            q.l_sent == true &&
                                            q.l_delete == false
                                      select new tblProdukKosong()
                                      {
                                          c_pkno        = q.c_pkno,
                                          d_pkdate      = q.d_pkdate.Value,
                                          v_nama        = q2.v_nama,
                                          v_nmdivpri    = q4.v_nmdivpri,
                                          c_iteno       = q1.c_iteno,
                                          v_itnam       = q1.v_itnam,
                                          d_abe         = q.d_abe.Value,
                                          c_tipe        = q.c_tipe == "01" ? "PK" : "NE"
                                      }).OrderBy(x => x.c_tipe).ThenBy(y => y.v_nama).ToList();

                    if (tbProdukKosong.Count() > 0)
                    {
                        ProductContent = "<Table border = '1'>";
                        ProductContent = ProductContent + "<th>No. Dokumen</th>";
                        ProductContent = ProductContent + "<th>Tanggal Input</th>";
                        ProductContent = ProductContent + "<th>Nama Principal</th>";
                        ProductContent = ProductContent + "<th>Nama Div. Principal</th>";
                        ProductContent = ProductContent + "<th>Kd. Item</th>";
                        ProductContent = ProductContent + "<th>Nama Item</th>";
                        ProductContent = ProductContent + "<th>ABE/Expired</th>";
                        ProductContent = ProductContent + "<th>Tipe</th>";

                        for (int nLoopC = 0; nLoopC < tbProdukKosong.Count; nLoopC++)
                        {
                            ProductContent = ProductContent + "<tr>";
                            ProductContent = ProductContent + "<td>" + tbProdukKosong[nLoopC].c_pkno + "</td>";
                            ProductContent = ProductContent + "<td>" + tbProdukKosong[nLoopC].d_pkdate.ToString("dd-MM-yyyy") + "</td>";
                            ProductContent = ProductContent + "<td>" + tbProdukKosong[nLoopC].v_nama + "</td>";
                            ProductContent = ProductContent + "<td>" + tbProdukKosong[nLoopC].v_nmdivpri + "</td>";
                            ProductContent = ProductContent + "<td>" + tbProdukKosong[nLoopC].c_iteno + "</td>";
                            ProductContent = ProductContent + "<td>" + tbProdukKosong[nLoopC].v_itnam + "</td>";
                            ProductContent = ProductContent + "<td>" + tbProdukKosong[nLoopC].d_abe.ToString("dd-MM-yyyy") + "</td>";
                            ProductContent = ProductContent + "<td>" + tbProdukKosong[nLoopC].c_tipe + "</td>";
                            ProductContent = ProductContent + "</tr>";

                            if (tbProdukKosong[nLoopC].d_abe.Date < DateTime.Now.Date)
                            {
                                if (tbProdukKosong[nLoopC].c_tipe == "PK")
                                {
                                    result = "Pada No. Dokumen " + tbProdukKosong[nLoopC].c_pkno + ", tanggal estimasi ketersediaan barang di Principal sudah kadaluarsa - PPIC perbaiki/update ABE sebelum data dikirim ke cabang.";
                                }
                                else
                                {
                                    result = "Pada No. Dokumen " + tbProdukKosong[nLoopC].c_pkno + ", tanggal expired produk sudah lewat - PPIC perbaiki/update Expired sebelum data dikirim ke cabang.";
                                }

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }

                                goto endLogic;
                            }

                            spkEmail = (from q in db.SCMS_ProdukKosongs
                                        where q.c_pkno == tbProdukKosong[nLoopC].c_pkno
                                        select q).Take(1).SingleOrDefault();

                            if (spkEmail != null)
                            {
                                spkEmail.d_sent = DateTime.Now;
                            }
                            else
                            {
                                result = "No. dokumen tidak ditemukan, kirim email dibatalkan.";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }

                                goto endLogic;
                            }
                        }

                        ProductContent = ProductContent + "</Table>";
                    }
                    else
                    {
                        result = "Tidak ada produk yang dapat dikirim email, kirim email dibatalkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    #region Receiver

                    Received = "allksl@ams.co.id";

                    #endregion

                    #region CC

                    CarbonCopy = "allkacab@ams.co.id;nita@log.ams.co.id;beni.bustertong@ams.co.id;suwandi@ams.co.id";

                    #endregion

                    #region Subject Email

                    Subject = "Daftar Produk Kosong Principal";

                    #endregion

                    #region Header Email

                    EmailHeader = "Yth Bpk/Ibu,<br /><br /><br />";

                    #endregion

                    #region Konten Email

                    EmailContent = "Berikut adalah informasi tentang hal sebagai berikut, : <br />";
                    EmailContent = EmailContent + "1) PK : Produk Kosong, dengan ABE(Perkiraan waktu ketersediaan barang). <br />";
                    EmailContent = EmailContent + "2) NE : Produk Prinsipal yang memiliki ED dekat, harus diperhitungkan dan segera bisa dijual. <br /><br />";
                    EmailContent = EmailContent + ProductContent + "<br /><br /><br />";

                    #endregion

                    #region Footer Email

                    EmailFooter = "Salam<br />PPIC<br />";

                    #endregion

                    if ((EmailSender.EmailParameter(db,
                                                   Sender,
                                                   TextSender,
                                                   Received + ";",
                                                   CarbonCopy + ";",
                                                   Subject,
                                                   EmailHeader,
                                                   EmailContent,
                                                   "",
                                                   EmailFooter)) == 0)
                    {
                        result = "Terdapat Kesalahan pada saat pengiriman email.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else
                    {
                        hasAnyChanges = true;
                        tbProdukKosong.Clear();
                        spkEmail = null;
                    }

                    #endregion
                }
                if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    #region Validasi
                   
                    if (string.IsNullOrEmpty(structure.Fields.Entry))
                    {
                        result = "Nip penanggung jawab dibutuhkan, harap login ulang.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }                    
                    
                    #endregion

                    spkDelete = (from q in db.SCMS_ProdukKosongs
                                 where q.c_pkno   == structure.Fields.Pkno &&
                                       q.l_delete == false
                                 select q).Take(1).SingleOrDefault();

                    if (spkDelete != null)
                    {
                        spkDelete.c_update      = structure.Fields.Entry;
                        spkDelete.d_update      = DateTime.Now;
                        spkDelete.l_delete      = true;
                        spkDelete.c_keterangan  = structure.Fields.Keterangan;

                        hasAnyChanges = true;
                        spkDelete = null;
                    }
                    else
                    {
                        result = "No. Dokumen " + structure.Fields.Pkno + " tidak ditemukan, hapus data dibatalkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Proses:ProcessStokOpname - {0}", ex.Message);

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

        #endregion
    }
}
