using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsModel;
using ScmsSoaLibrary.Parser;
using ScmsSoaLibrary.Commons;
using System.Data.Common;
using ScmsSoaLibrary.Core.Threading;
using ScmsSoaLibraryInterface.Commons;
using System.Data.Common;
using System.Data.Odbc;

namespace ScmsSoaLibrary.Bussiness
{
    #region Internal Class

    internal class PO_BUDGET_RECALCULATE
    {
        public string POID { get; set; }
        public decimal TotalTrx { get; set; }
        public string NipUser { get; set; }
        public DateTime DateEntry { get; set; }
        public bool IsDelete { get; set; }
        public string Keterangan { get; set; }
    }

    #endregion

    
    class Master
    {
        #region Private

        private void PostDataReplyItm(string connectionString, ScmsSoaLibrary.Parser.Class.MasterItemResponse strt, string iteNo)
        {
            string dataResult = ScmsSoaLibrary.Parser.Class.MasterItemResponse.Serialize(strt);

            Commons.RunningSendingReplyItem(connectionString, dataResult, iteNo);
        }

        #endregion

        public string MasterItemBisnis(ScmsSoaLibrary.Parser.Class.MasterItemStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false;
            string result = null;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            ScmsSoaLibrary.Parser.Class.MasterItemStructureFields field = null;
            string nipEntry = null;
            //string memoID = null;
            //string tmpNumbering = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now;

            FA_MasItm itm = null;

            IDictionary<string, string> dic = null;

            ScmsSoaLibrary.Parser.Class.MasterItemResponse itmResp = null;
            List<ScmsSoaLibrary.Parser.Class.MasterItemResponseJSONStructureField> listItmJsonField = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }
            if (string.IsNullOrEmpty(structure.Fields.ItemID))
            {
                result = "Kode Item Tidak dapat Di baca.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }
            //int totalDetails = 0;

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
                {
                    #region Add

                    field = structure.Fields;

                    if ((field != null))
                    {
                        //if (string.IsNullOrEmpty(field))
                        //{
                        //  result = "Kode Item Tidak dapat Di baca.";

                        //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        //  goto endLogic;
                        //}


                    }

                    #endregion
                }

                else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify
                    listItmJsonField = new List<ScmsSoaLibrary.Parser.Class.MasterItemResponseJSONStructureField>();
                    itmResp = new ScmsSoaLibrary.Parser.Class.MasterItemResponse();

                    itm = (from q in db.FA_MasItms
                           where q.c_iteno == structure.Fields.ItemID
                           select q).Take(1).SingleOrDefault();

                    itm.n_qminord = structure.Fields.MinQ;
                    itm.l_dinkes = structure.Fields.Dinkes;
                    itm.n_het = structure.Fields.HET;
                    itm.c_alkes = structure.Fields.Alkes;
                    itm.c_nie = structure.Fields.Nie;
                    itm.d_nie = structure.Fields.TanggalNie;
                    itm.c_golongan = structure.Fields.Golongan;
                    itm.v_komposisi = structure.Fields.Komposisi;
                    itm.n_box = structure.Fields.Box;
                    itm.d_update = DateTime.Now;
                    itm.c_update = structure.Fields.Entry;

                    if (itm != null)
                    {
                        listItmJsonField.Add(new ScmsSoaLibrary.Parser.Class.MasterItemResponseJSONStructureField()
                        {
                            C_ITENO = structure.Fields.ItemID,
                            N_BOX = structure.Fields.Box
                        });
                    }
                    hasAnyChanges = true;

                    #endregion
                }

                if (hasAnyChanges)
                {
                    if ((listItmJsonField != null) && (listItmJsonField.Count > 0))
                    {
                        itmResp.Fields = listItmJsonField.ToArray();

                        listItmJsonField.Clear();

                        PostDataReplyItm(db.Connection.ConnectionString, itmResp, structure.Fields.ItemID);
                    }

                    if (Constant.isDcoreError)
                    {
                        db.Transaction.Rollback();
                        rpe = ResponseParser.ResponseParserEnum.IsFailed;
                        Constant.isDcoreError = false;
                    }
                    else
                    {
                        db.SubmitChanges();

                        db.Transaction.Commit();

                        rpe = ResponseParser.ResponseParserEnum.IsSuccess;
                    }
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

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:MasterItemBisnis - {0}", ex.Message);

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

        public string Discount(ScmsSoaLibrary.Parser.Class.DiscountStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            ScmsSoaLibrary.Parser.Class.DiscountStructureField field = null;

            int totalDetails = 0;

            string nipEntry = null;
            string discId = null,
              itemId = null;
            //string tmpNumbering = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now,
              date1 = DateTime.MinValue,
              date2 = DateTime.MinValue;

            //FA_DiscH disch = null;
            FA_DiscD discd = null;
            FA_DiscD1 discd1 = null;

            //List<FA_DiscD> listDiscD = null;
            List<FA_DiscD1> listDiscD1 = null;
            StringBuilder sbError = new StringBuilder();

            int nLoop = 0;

            bool isExistInRange = false;

            IDictionary<string, string> dic = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            discId = (structure.Fields.DiscountID ?? string.Empty);
            itemId = (structure.Fields.Item ?? string.Empty);

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    if (string.IsNullOrEmpty(discId))
                    {
                        result = "Kode discount dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (string.IsNullOrEmpty(itemId))
                    {
                        result = "Kode item dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    discd = (from q in db.FA_DiscDs
                             where q.c_nodisc == discId && q.c_iteno == itemId
                             select q).Take(1).SingleOrDefault();

                    #region Verifikasi

                    if (structure.Fields.TanggalAwalDate.Equals(DateTime.MinValue))
                    {
                        result = "Periode awal salah.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (structure.Fields.TanggalAkhirDate.Equals(DateTime.MinValue))
                    {
                        result = "Periode akhir salah.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (structure.Fields.TanggalAwalDate.CompareTo(structure.Fields.TanggalAkhirDate) > 0)
                    {
                        result = "Periode awal tidak boleh lebih besar dari akhir.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    //else if (structure.Fields.DiscountOn < 0)
                    //{
                    //  result = "Diskon On tidak boleh dibawah 0.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}
                    //else if (structure.Fields.DiscountOff < 0)
                    //{
                    //  result = "Diskon Off tidak boleh dibawah 0.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}
                    else if ((structure.Fields.DiscountOn <= 0) && (structure.Fields.DiscountOff <= 0))
                    {
                        result = "Diskon On dan Off tidak boleh keduanya <= 0.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    #endregion

                    if (discd == null)
                    {
                        discd = new FA_DiscD()
                        {
                            c_entry = nipEntry,
                            c_iteno = structure.Fields.Item,
                            c_nodisc = discId,
                            c_update = nipEntry,
                            d_entry = date,
                            d_finish = structure.Fields.TanggalAkhirDate,
                            d_start = structure.Fields.TanggalAwalDate,
                            d_update = date,
                            l_aktif = true,
                            l_status = true,
                            n_discoff = structure.Fields.DiscountOff,
                            n_discon = structure.Fields.DiscountOn
                        };

                        db.FA_DiscDs.InsertOnSubmit(discd);
                    }
                    else
                    {
                        discd.n_discon = structure.Fields.DiscountOn;
                        discd.n_discoff = structure.Fields.DiscountOff;
                    }

                    #region Insert Details

                    listDiscD1 = (from q in db.FA_DiscD1s
                                  where q.c_nodisc == discId && q.c_iteno == itemId
                                      //&& (q.l_delete == null ? false : q.l_delete.Value) == false
                                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                  //&& (q.l_aktif == true) && (q.l_status == true)
                                  select q).ToList();

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if (field.IsNew && (!field.IsModified) & (!field.IsDelete))
                            {
                                #region New

                                #region Verifikasi

                                if (field.TanggalAwalDate.Equals(DateTime.MinValue))
                                {
                                    sbError.AppendLine(string.Format("On : {0}, Off : {1}, Date1 : {2}, Date2 : {3}, Periode awal salah.",
                                      field.DiscountOn, field.DiscountOff, field.TanggalAwalDate.ToString("dd-MM-yyyy"), field.TanggalAkhirDate.ToString("dd-MM-yyyy")));

                                    continue;
                                }
                                else if (field.TanggalAkhirDate.Equals(DateTime.MinValue))
                                {
                                    sbError.AppendLine(string.Format("On : {0}, Off : {1}, Date1 : {2}, Date2 : {3}, Periode akhir salah.",
                                      field.DiscountOn, field.DiscountOff, field.TanggalAwalDate.ToString("dd-MM-yyyy"), field.TanggalAkhirDate.ToString("dd-MM-yyyy")));

                                    continue;
                                }
                                else if (field.TanggalAwalDate.CompareTo(field.TanggalAkhirDate) > 0)
                                {
                                    sbError.AppendLine(string.Format("On : {0}, Off : {1}, Date1 : {2}, Date2 : {3}, Periode awal tidak boleh lebih besar dari akhir.",
                                      field.DiscountOn, field.DiscountOff, field.TanggalAwalDate.ToString("dd-MM-yyyy"), field.TanggalAkhirDate.ToString("dd-MM-yyyy")));

                                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                    continue;
                                }
                                else if ((field.DiscountOn <= 0) && (field.DiscountOff <= 0))
                                {
                                    sbError.AppendLine(string.Format("On : {0}, Off : {1}, Date1 : {2}, Date2 : {3}, Diskon On dan Off tidak boleh keduanya <= 0.",
                                      field.DiscountOn, field.DiscountOff, field.TanggalAwalDate.ToString("dd-MM-yyyy"), field.TanggalAkhirDate.ToString("dd-MM-yyyy")));

                                    continue;
                                }

                                #endregion

                                isExistInRange = listDiscD1.Exists(delegate(FA_DiscD1 fad)
                                {
                                    #region Old Coded

                                    ////return
                                    ////  ((((fad.d_start.HasValue ? fad.d_start.Value : DateTime.MinValue).CompareTo(field.TanggalAwalDate) >= 0) &&
                                    ////  ((fad.d_finish.HasValue ? fad.d_finish.Value : DateTime.MinValue).CompareTo(field.TanggalAwalDate) <= 0)) ||
                                    ////  (((fad.d_start.HasValue ? fad.d_start.Value : DateTime.MinValue).CompareTo(field.TanggalAkhirDate) >= 0) &&
                                    ////  ((fad.d_finish.HasValue ? fad.d_finish.Value : DateTime.MinValue).CompareTo(field.TanggalAkhirDate) <= 0)));

                                    ////return
                                    ////  (((field.TanggalAwalDate.CompareTo(fad.d_start.HasValue ? fad.d_start.Value : DateTime.MinValue) >= 0) &&
                                    ////  (field.TanggalAwalDate.CompareTo(fad.d_finish.HasValue ? fad.d_finish.Value : DateTime.MinValue) <= 0)) ||
                                    ////  ((field.TanggalAkhirDate.CompareTo(fad.d_start.HasValue ? fad.d_start.Value : DateTime.MinValue) >= 0) &&
                                    ////  (field.TanggalAkhirDate.CompareTo(fad.d_finish.HasValue ? fad.d_finish.Value : DateTime.MinValue) <= 0)));

                                    //date1 = (fad.d_start.HasValue ? fad.d_start.Value : DateTime.MinValue);
                                    //date2 = (fad.d_finish.HasValue ? fad.d_finish.Value : DateTime.MinValue);

                                    //bool b1 =
                                    //    ((date1.CompareTo(field.TanggalAwalDate) >= 0) &&
                                    //    (date2.CompareTo(field.TanggalAkhirDate) <= 0));

                                    //bool b2 =
                                    //    (((field.TanggalAwalDate.CompareTo(date1) >= 0) &&
                                    //    (field.TanggalAwalDate.CompareTo(date2) <= 0)) ||
                                    //    ((field.TanggalAkhirDate.CompareTo(date1) >= 0) &&
                                    //    (field.TanggalAkhirDate.CompareTo(date2) <= 0)));

                                    //bool b3 =
                                    //    (((date1.CompareTo(field.TanggalAwalDate) >= 0) ||
                                    //    (date2.CompareTo(field.TanggalAwalDate) <= 0)) &&
                                    //    ((date1.CompareTo(field.TanggalAkhirDate) >= 0) ||
                                    //    (date2.CompareTo(field.TanggalAkhirDate) <= 0)));

                                    #endregion

                                    return
                                      (
                                        ((date1.CompareTo(field.TanggalAwalDate) >= 0) &&
                                        (date2.CompareTo(field.TanggalAkhirDate) <= 0))

                                        ||

                                        (((field.TanggalAwalDate.CompareTo(date1) >= 0) &&
                                        (field.TanggalAwalDate.CompareTo(date2) <= 0)) ||
                                        ((field.TanggalAkhirDate.CompareTo(date1) >= 0) &&
                                        (field.TanggalAkhirDate.CompareTo(date2) <= 0)))
                                      );
                                });

                                if (isExistInRange)
                                {
                                    sbError.AppendLine(string.Format("On : {0}, Off : {1}, Date1 : {2}, Date2 : {3}, Telah ada di jangkauan sebelumnya tanggal tersebut.",
                                      field.DiscountOn, field.DiscountOff, field.TanggalAwalDate.ToString("dd-MM-yyyy"), field.TanggalAkhirDate.ToString("dd-MM-yyyy")));
                                }
                                else
                                {
                                    discd1 = new FA_DiscD1()
                                    {
                                        c_entry = nipEntry,
                                        c_iteno = itemId,
                                        c_nodisc = discId,
                                        c_update = nipEntry,
                                        d_entry = date,
                                        d_finish = field.TanggalAkhirDate,
                                        d_start = field.TanggalAwalDate,
                                        d_update = date,
                                        l_aktif = true,
                                        l_status = false,
                                        n_discoff = field.DiscountOff,
                                        n_discon = field.DiscountOn,
                                        v_type = "01"
                                    };

                                    listDiscD1.Add(discd1);

                                    db.FA_DiscD1s.InsertOnSubmit(discd1);

                                    totalDetails++;
                                }

                                #endregion
                            }
                            else if ((!field.IsNew) && field.IsModified & (!field.IsDelete))
                            {
                                #region Modified

                                if (field.IndexID != 0)
                                {
                                    discd1 = listDiscD1.Find(delegate(FA_DiscD1 fad1)
                                    {
                                        return fad1.IDX == field.IndexID;
                                    });

                                    if (discd1 != null)
                                    {
                                        //discd1.n_discon = field.DiscountOn;
                                        //discd1.n_discoff = field.DiscountOff;

                                        discd1.l_aktif = field.Aktif;

                                        discd1.c_update = nipEntry;
                                        discd1.d_update = date;

                                        discd1.v_type = "02";

                                        totalDetails++;
                                    }
                                }

                                #endregion
                            }
                            else if ((!field.IsNew) && (!field.IsModified) & field.IsDelete)
                            {
                                #region Delete

                                if (field.IndexID != 0)
                                {
                                    discd1 = listDiscD1.Find(delegate(FA_DiscD1 fad1)
                                    {
                                        return fad1.IDX == field.IndexID;
                                    });

                                    if (discd1 != null)
                                    {
                                        discd1.l_aktif = false;
                                        discd1.l_status = false;

                                        discd1.c_update = nipEntry;
                                        discd1.d_update = date;

                                        discd1.l_delete = true;
                                        discd1.v_ket_del = field.KeteranganMod;
                                        discd1.v_type = "03";

                                        totalDetails++;
                                    }
                                }

                                #endregion
                            }
                        }
                    }

                    listDiscD1.Clear();

                    #endregion

                    hasAnyChanges = (totalDetails > 0);

                    if (sbError.Length > 0)
                    {
                        result = sbError.ToString();
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

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:Discount - {0}", ex.Message);

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

        public string MasterBatch(ScmsSoaLibrary.Parser.Class.BatchItemStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false;
            string result = null,
              btchId = null,
              rnId = null,
              suplId = null;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            //ScmsSoaLibrary.Parser.Class.MasterItemStructureField field = null;
            string nipEntry = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now;

            IDictionary<string, string> dic = null;

            LG_MsBatch batch = null;

            LG_MsBatchLog batchLog = null;

            LG_RNH rnh = null;

            LG_RND1 rnd1 = null;

            bool isAddNew = false;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }
            else if (string.IsNullOrEmpty(structure.Fields.ItemID))
            {
                result = "Kode Item Tidak dapat di baca.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            try
            {
                btchId = (structure.Fields.BatchID ?? string.Empty).Trim().ToUpper();

                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                if (string.IsNullOrEmpty(btchId))
                {
                    result = "Kode Batch Tidak dapat di baca.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (structure.Fields.TanggalExpireDate.Equals(DateTime.MinValue) || structure.Fields.TanggalExpireDate.Equals(DateTime.MaxValue))
                {
                    result = "Tanggal tidak dapat di baca";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }



                else if ( (structure.Fields.BatchID.Contains("'")) || (structure.Fields.BatchID.Contains("*")) || (structure.Fields.BatchID.Contains("%")) )
                {
                    result = "Tidak Boleh Karakter ' ( Kutip) atau  * (Bintang) atau %(Persentase) ";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }



                //char GUDANG_HO = '1';
                char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '1' : structure.Fields.Gudang[0]);
                
                const string MSBATCHITM = "MSBATCHITM";

                
                if (structure.Method.Equals("AddModify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Add or Modify

                    suplId = (from q in db.FA_MasItms
                              where (q.c_iteno == structure.Fields.ItemID)
                              select (q.c_nosup == null ? string.Empty : q.c_nosup.Trim())).Take(1).SingleOrDefault();

                    batch = (from q in db.LG_MsBatches
                             where (q.c_iteno == structure.Fields.ItemID) && (q.c_batch == btchId)
                             select q).Take(1).SingleOrDefault();

                    if (batch == null)
                    {
                        isAddNew = true;

                        batch = new LG_MsBatch()
                        {
                            c_iteno = structure.Fields.ItemID,
                            c_batch = btchId,
                            d_expired = structure.Fields.TanggalExpireDate,
                            c_entry = nipEntry,
                            c_update = nipEntry,
                            d_entry = date,
                            d_update = date
                        };

                        db.LG_MsBatches.InsertOnSubmit(batch);

                        batchLog = new LG_MsBatchLog()
                        {
                            c_iteno = structure.Fields.ItemID,
                            c_batch = btchId,
                            d_expired = structure.Fields.TanggalExpireDate,
                            c_entry = nipEntry,
                            c_update = nipEntry,
                            d_entry = date,
                            d_update = date,
                            v_ket_del = string.Empty,
                            v_type = "01",
                            c_no = rnId
                        };
                    }
                    else
                    {
                        batch.d_expired = structure.Fields.TanggalExpireDate;
                        //batch.c_batch = btchId;

                        batchLog = new LG_MsBatchLog()
                        {
                            c_iteno = structure.Fields.ItemID,
                            c_batch = btchId,
                            d_expired = structure.Fields.TanggalExpireDate,
                            c_entry = nipEntry,
                            d_entry = date,
                            d_update = date,
                            c_update = nipEntry,
                            v_ket_del = string.Empty,
                            v_type = "02",
                            c_no = string.Empty
                        };
                    }

                    #region RN

                    rnId = string.Concat("RNXXC", (suplId ?? "BATCH"));

                    rnd1 = (from q in db.LG_RND1s
                            where (q.c_gdg == gudang) && (q.c_rnno == rnId)
                            && (q.c_iteno == structure.Fields.ItemID)
                            && (q.c_batch == btchId)
                            select q).Take(1).SingleOrDefault();

                    if (rnd1 == null)
                    {
                        rnd1 = new LG_RND1()
                        {
                            c_gdg = gudang,
                            c_rnno = rnId,
                            c_iteno = structure.Fields.ItemID,
                            c_batch = btchId,
                            n_bqty = 0,
                            n_bsisa = 0,
                            //n_floqty = 0,
                            n_gqty = 0,
                            n_gsisa = 0
                        };

                        db.LG_RND1s.InsertOnSubmit(rnd1);

                        rnh = (from q in db.LG_RNHs
                               where (q.c_gdg == gudang) && (q.c_rnno == rnId) //&& (q.c_type == "07")
                                //&& (q.c_dono == MSBATCHITM)
                               select q).Take(1).SingleOrDefault();

                        if (rnh == null)
                        {
                            rnh = new LG_RNH()
                            {
                                c_gdg = gudang,
                                c_rnno = rnId,
                                d_rndate = date,
                                c_type = "07",
                                l_float = false,
                                c_dono = MSBATCHITM,
                                d_dodate = date,
                                v_ket = "Auto Create From Master Batch",
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

                            db.LG_RNHs.InsertOnSubmit(rnh);
                        }
                    }

                    #endregion

                    dic = new Dictionary<string, string>();

                    dic.Add("Batch", btchId);
                    dic.Add("Item", structure.Fields.ItemID);
                    dic.Add("Add", isAddNew.ToString().ToLower());
                    dic.Add("Tanggal", date.ToString("yyyyMMdd"));

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:MasterBatch - {0}", ex.Message);

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

        public string BudgetLimit(ScmsSoaLibrary.Parser.Class.BudgetLimitStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false;
            string result = null,
              tmp = null;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            //ScmsSoaLibrary.Parser.Class.MasterItemStructureField field = null;
            string nipEntry = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now,
              firstDate = Functionals.StandardSqlDateTime;

            IDictionary<string, string> dic = null;

            List<PO_BUDGET_RECALCULATE> listPOBR = null;
            PO_BUDGET_RECALCULATE pobr = null;

            List<LG_DatSupServiceD> listDSD = null;
            LG_DatSupServiceH dsh = null;
            LG_DatSupServiceD dsd = null;

            int nLoop = 0;

            decimal decTotalUsed = 0,
              decAvaible = 0,
              decSisaAvaible = 0,
              decRetrAvaible = 0,
              decPOAvaible = 0;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }
            else if (string.IsNullOrEmpty(structure.Fields.SupplierID))
            {
                result = "Kode pemasok Tidak dapat di baca.";

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

                    if ((structure.Fields.PeriodeTahun < 1) || (structure.Fields.PeriodeTahun > 9999))
                    {
                        result = "Periode tahun salah.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if ((structure.Fields.PeriodeBulan < 1) || (structure.Fields.PeriodeBulan > 12))
                    {
                        result = "Periode bulan salah.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    dsh = (from q in db.LG_DatSupServiceHs
                           where (q.c_nosup == structure.Fields.SupplierID)
                            && ((q.n_tahun == structure.Fields.PeriodeTahun) && (q.n_bulan == structure.Fields.PeriodeBulan))
                           select q).Take(1).SingleOrDefault();

                    if (dsh != null)
                    {
                        result = "Anggaran periode tersebut telah ada.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    dsh = new LG_DatSupServiceH()
                    {
                        c_nosup = structure.Fields.SupplierID,
                        n_tahun = structure.Fields.PeriodeTahun,
                        n_bulan = structure.Fields.PeriodeBulan,
                        limit = structure.Fields.Limit,
                        avaiblelimit = structure.Fields.Limit,
                        d_nextlimit = structure.Fields.Persentase,
                        c_entry = nipEntry,
                        d_entry = date,
                        c_update = nipEntry,
                        d_update = date
                    };

                    #region Detail

                    listPOBR = (from q in db.LG_POHs
                                join q1 in db.LG_POD1s on q.c_pono equals q1.c_pono
                                where (q.c_nosup == structure.Fields.SupplierID)
                                  && (q.d_podate.Value.Year == structure.Fields.PeriodeTahun) && (q.d_podate.Value.Month == structure.Fields.PeriodeBulan)
                                //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                group new { q, q1 } by new { q.c_pono, q.c_entry, q.d_entry, q.l_delete, q.v_ket_mark } into g
                                select new PO_BUDGET_RECALCULATE()
                                {
                                    POID = (g.Key.c_pono == null ? string.Empty : g.Key.c_pono.Trim()),
                                    NipUser = (g.Key.c_entry == null ? string.Empty : g.Key.c_entry.Trim()),
                                    DateEntry = (g.Key.d_entry.HasValue ? g.Key.d_entry.Value : firstDate),
                                    TotalTrx = g.Sum(t => ((t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0) * (t.q1.n_salpri.HasValue ? t.q1.n_salpri.Value : 0))),
                                    IsDelete = (g.Key.l_delete.HasValue ? g.Key.l_delete.Value : false),
                                    Keterangan = (g.Key.v_ket_mark == null ? string.Empty : g.Key.v_ket_mark.Trim())
                                }).Distinct().ToList();

                    if (listPOBR.Count > 0)
                    {
                        listDSD = new List<LG_DatSupServiceD>();

                        for (nLoop = 0; nLoop < listPOBR.Count; nLoop++)
                        {
                            pobr = listPOBR[nLoop];

                            if (!pobr.IsDelete)
                            {
                                decTotalUsed += pobr.TotalTrx;
                            }

                            listDSD.Add(new LG_DatSupServiceD()
                            {
                                c_nosup = structure.Fields.SupplierID,
                                n_tahun = structure.Fields.PeriodeTahun,
                                n_bulan = structure.Fields.PeriodeBulan,
                                c_pono = pobr.POID,
                                n_bilva = pobr.TotalTrx,
                                c_entry = pobr.NipUser,
                                d_entry = pobr.DateEntry,
                                c_update = string.Empty,
                                d_update = firstDate,
                                l_delete = pobr.IsDelete,
                                v_ket = pobr.Keterangan
                            });
                        }

                        db.LG_DatSupServiceDs.InsertAllOnSubmit(listDSD.ToArray());

                        listDSD.Clear();
                        listPOBR.Clear();

                        dsh.avaiblelimit -= decTotalUsed;
                    }

                    #endregion

                    db.LG_DatSupServiceHs.InsertOnSubmit(dsh);

                    dic = new Dictionary<string, string>();

                    dic.Add("Tanggal", date.ToString("yyyyMMdd"));
                    dic.Add("UsedLimit", decTotalUsed.ToString());

                    hasAnyChanges = true;

                    hasAnyChanges = true;

                    #endregion
                }
                else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    if ((structure.Fields.PeriodeTahun < 1) || (structure.Fields.PeriodeTahun > 9999))
                    {
                        result = "Periode tahun salah.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if ((structure.Fields.PeriodeBulan < 1) || (structure.Fields.PeriodeBulan > 12))
                    {
                        result = "Periode bulan salah.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    dsh = (from q in db.LG_DatSupServiceHs
                           where (q.c_nosup == structure.Fields.SupplierID)
                            && ((q.n_tahun == structure.Fields.PeriodeTahun) && (q.n_bulan == structure.Fields.PeriodeBulan))
                           select q).Take(1).SingleOrDefault();

                    if (dsh == null)
                    {
                        result = "Anggaran periode tersebut tidak diteumkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    decAvaible = (dsh.limit.HasValue ? dsh.limit.Value : 0);
                    decSisaAvaible = (dsh.avaiblelimit.HasValue ? dsh.avaiblelimit.Value : 0);

                    #region Detail

                    decTotalUsed = 0;

                    #region Check Data

                    listDSD = (from q in db.LG_DatSupServiceDs
                               where (q.c_nosup == structure.Fields.SupplierID)
                                && (q.n_tahun == structure.Fields.PeriodeTahun) && (q.n_bulan == structure.Fields.PeriodeBulan)
                                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                               select q).Distinct().ToList();

                    for (nLoop = 0; nLoop < listDSD.Count; nLoop++)
                    {
                        dsd = listDSD[nLoop];

                        tmp = (string.IsNullOrEmpty(dsd.c_pono) ? string.Empty : dsd.c_pono.Trim());

                        decPOAvaible = (dsd.n_bilva.HasValue ? dsd.n_bilva.Value : 0);

                        pobr = (from q in db.LG_POHs
                                join q1 in db.LG_POD1s on q.c_pono equals q1.c_pono
                                where (q.c_nosup == structure.Fields.SupplierID)
                                  && (q.c_pono == tmp)
                                  && (q.d_podate.Value.Year == structure.Fields.PeriodeTahun)
                                  && (q.d_podate.Value.Month == structure.Fields.PeriodeBulan)
                                group new { q, q1 } by new { q.c_pono, q.c_entry, q.d_entry, q.l_delete, q.v_ket_mark } into g
                                select new PO_BUDGET_RECALCULATE()
                                {
                                    POID = (g.Key.c_pono == null ? string.Empty : g.Key.c_pono.Trim()),
                                    NipUser = (g.Key.c_entry == null ? string.Empty : g.Key.c_entry.Trim()),
                                    DateEntry = (g.Key.d_entry.HasValue ? g.Key.d_entry.Value : firstDate),
                                    TotalTrx = g.Sum(t => ((t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0) * (t.q1.n_salpri.HasValue ? t.q1.n_salpri.Value : 0))),
                                    IsDelete = (g.Key.l_delete.HasValue ? g.Key.l_delete.Value : false),
                                    Keterangan = (g.Key.v_ket_mark == null ? string.Empty : g.Key.v_ket_mark.Trim())
                                }).Take(1).SingleOrDefault();

                        if (pobr == null)
                        {
                            decRetrAvaible += (dsd.n_bilva.HasValue ? dsd.n_bilva.Value : 0);

                            dsd.c_update = Constant.SYSTEM_USERNAME;
                            dsd.d_update = date;

                            dsd.l_delete = true;
                            dsd.v_ket = string.Concat("Delete @ ", date.ToString("dd-MM-yyyy"));
                        }
                        else if (pobr.IsDelete)
                        {
                            decRetrAvaible += (dsd.n_bilva.HasValue ? dsd.n_bilva.Value : 0);

                            dsd.c_update = pobr.NipUser;
                            dsd.d_update = pobr.DateEntry;

                            dsd.l_delete = true;
                            dsd.v_ket = string.Concat("@@ - ", date.ToString("dd-MM-yyyy"), " - ", pobr.Keterangan);
                        }
                        else if (decPOAvaible != pobr.TotalTrx)
                        {
                            decTotalUsed += pobr.TotalTrx;

                            pobr.TotalTrx = pobr.TotalTrx;
                        }
                        else
                        {
                            decTotalUsed += pobr.TotalTrx;
                        }
                    }

                    #endregion

                    #region Check From PO

                    listPOBR = (from q in db.LG_POHs
                                join q1 in db.LG_POD1s on q.c_pono equals q1.c_pono
                                where (q.d_podate.Value.Year == structure.Fields.PeriodeTahun) && (q.d_podate.Value.Month == structure.Fields.PeriodeBulan)
                                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                group new { q, q1 } by new { q.c_pono, q.c_entry, q.d_entry, q.l_delete, q.v_ket_mark } into g
                                select new PO_BUDGET_RECALCULATE()
                                {
                                    POID = (g.Key.c_pono == null ? string.Empty : g.Key.c_pono.Trim()),
                                    NipUser = (g.Key.c_entry == null ? string.Empty : g.Key.c_entry.Trim()),
                                    DateEntry = (g.Key.d_entry.HasValue ? g.Key.d_entry.Value : firstDate),
                                    TotalTrx = g.Sum(t => ((t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0) * (t.q1.n_salpri.HasValue ? t.q1.n_salpri.Value : 0))),
                                    IsDelete = (g.Key.l_delete.HasValue ? g.Key.l_delete.Value : false),
                                    Keterangan = (g.Key.v_ket_mark == null ? string.Empty : g.Key.v_ket_mark.Trim())
                                }).Distinct().ToList();

                    if (listPOBR.Count > 0)
                    {
                        listDSD = new List<LG_DatSupServiceD>();

                        decTotalUsed = 0;

                        for (nLoop = 0; nLoop < listPOBR.Count; nLoop++)
                        {
                            pobr = listPOBR[nLoop];

                            if (!pobr.IsDelete)
                            {
                                continue;
                            }

                            dsd = listDSD.Find(delegate(LG_DatSupServiceD dssd)
                            {
                                return pobr.POID.Equals((string.IsNullOrEmpty(dssd.c_pono) ? string.Empty : dssd.c_pono.Trim()), StringComparison.OrdinalIgnoreCase)
                                  && ((dssd.l_delete.HasValue ? dssd.l_delete.Value : false) == false);
                            });

                            if (dsd == null)
                            {
                                decTotalUsed += pobr.TotalTrx;

                                dsd = new LG_DatSupServiceD()
                                {
                                    c_nosup = structure.Fields.SupplierID,
                                    n_tahun = structure.Fields.PeriodeTahun,
                                    n_bulan = structure.Fields.PeriodeBulan,
                                    c_pono = pobr.POID,
                                    n_bilva = pobr.TotalTrx,
                                    c_entry = pobr.NipUser,
                                    d_entry = pobr.DateEntry,
                                    c_update = string.Empty,
                                    d_update = firstDate,
                                    l_delete = pobr.IsDelete,
                                    v_ket = pobr.Keterangan
                                };

                                db.LG_DatSupServiceDs.InsertOnSubmit(dsd);

                                listDSD.Add(dsd);
                            }
                        }

                        dsh.avaiblelimit -= decTotalUsed;

                        listPOBR.Clear();
                    }

                    listDSD.Clear();

                    #endregion

                    #endregion

                    decSisaAvaible += decRetrAvaible;
                    decSisaAvaible -= decTotalUsed;

                    decPOAvaible = (decAvaible > structure.Fields.Limit ? -(decAvaible - structure.Fields.Limit) : (structure.Fields.Limit - decAvaible));

                    decTotalUsed = (decPOAvaible + decSisaAvaible);

                    dsh.limit = structure.Fields.Limit;
                    dsh.avaiblelimit = decTotalUsed;

                    if (decTotalUsed >= 0)
                    {
                        dic = new Dictionary<string, string>();

                        dic.Add("Tanggal", date.ToString("yyyyMMdd"));
                        dic.Add("UsedLimit", decTotalUsed.ToString());

                        hasAnyChanges = true;
                    }
                    else
                    {
                        result = "Jumlah anggaran tidak menutupi total transaksi.";

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:BudgetLimit - {0}", ex.Message);

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

        public string MasterGudang(Parser.Parser.StructureXmlHeaderParser xmlParser, Parser.Parser.StructureDataNamingHeader dataParser)
        {
            if ((xmlParser == null) || (dataParser == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false;
            string result = null;

            MyAssembly myAsm = new MyAssembly();

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            char gudang = char.MinValue;

            string nipEntry = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now,
              firstDate = Functionals.StandardSqlDateTime;

            IDictionary<string, string> dic = null;

            LG_MsGudang tblGudang = null,
              tblGudangEdit = null;

            int nCount = 0;

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                tblGudang = myAsm.Populate<LG_MsGudang>(xmlParser, dataParser);

                if (tblGudang == null)
                {
                    result = "Struktur gudang tidak terbaca.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                nipEntry = (tblGudang.c_entry ?? string.Empty);

                if (string.IsNullOrEmpty(nipEntry))
                {
                    result = "Nip dibutuhkan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                if (dataParser.Method == ScmsSoaLibrary.Parser.Parser.InterpreterMethod.IsAdd)
                {
                    #region Add

                    gudang = (from q in db.LG_MsGudangs
                              orderby q.c_gdg descending
                              select q.c_gdg).Take(1).SingleOrDefault();

                    if (gudang == '9')
                    {
                        gudang = 'a';
                    }
                    else if (gudang == 'z')
                    {
                        result = "Gudang tidak dapat ditambahkan lagi.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else
                    {
                        gudang++;
                    }

                    nCount = (from q in db.LG_MsGudangs
                              where (q.c_gdg == gudang)
                              select q.c_gdg).Count();

                    if (nCount != 0)
                    {
                        result = "Kode gudang sudah ada.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    tblGudang.c_gdg = gudang;

                    tblGudang.d_entry = date;
                    tblGudang.d_update = date;

                    db.LG_MsGudangs.InsertOnSubmit(tblGudang);

                    dic = new Dictionary<string, string>();

                    dic.Add("Gudang", gudang.ToString());
                    dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                    hasAnyChanges = true;

                    #endregion
                }
                else if (dataParser.Method == ScmsSoaLibrary.Parser.Parser.InterpreterMethod.IsUpdate)
                {
                    #region Modify

                    gudang = tblGudang.c_gdg;

                    tblGudangEdit = (from q in db.LG_MsGudangs
                                     where (q.c_gdg == gudang)
                                     select q).Take(1).SingleOrDefault();

                    if (tblGudangEdit == null)
                    {
                        result = "Gudang tidak dapat terbaca dari database.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    tblGudangEdit.c_kodepos = (string.IsNullOrEmpty(tblGudang.c_kodepos) ? tblGudangEdit.c_kodepos : tblGudang.c_kodepos.Trim());
                    tblGudangEdit.c_kota = (string.IsNullOrEmpty(tblGudang.c_kota) ? tblGudangEdit.c_kota : tblGudang.c_kota.Trim());
                    tblGudangEdit.c_rt = (string.IsNullOrEmpty(tblGudang.c_rt) ? tblGudangEdit.c_rt : tblGudang.c_rt.Trim());
                    tblGudangEdit.c_rw = (string.IsNullOrEmpty(tblGudang.c_rw) ? tblGudangEdit.c_rw : tblGudang.c_rw.Trim());
                    tblGudangEdit.l_aktif = (tblGudang.l_aktif.HasValue ? tblGudang.l_aktif.Value : tblGudangEdit.l_aktif);
                    tblGudangEdit.v_alamat = (string.IsNullOrEmpty(tblGudang.v_alamat) ? tblGudangEdit.v_alamat : tblGudang.v_alamat.Trim());
                    tblGudangEdit.v_camat = (string.IsNullOrEmpty(tblGudang.v_camat) ? tblGudangEdit.v_camat : tblGudang.v_camat.Trim());
                    tblGudangEdit.v_gdgdesc = (string.IsNullOrEmpty(tblGudang.v_gdgdesc) ? tblGudangEdit.v_gdgdesc : tblGudang.v_gdgdesc.Trim());
                    tblGudangEdit.v_lurah = (string.IsNullOrEmpty(tblGudang.v_lurah) ? tblGudangEdit.v_lurah : tblGudang.v_lurah.Trim());
                    tblGudangEdit.v_nama = (string.IsNullOrEmpty(tblGudang.v_nama) ? tblGudangEdit.v_nama : tblGudang.v_nama.Trim());
                    tblGudangEdit.v_telp = (string.IsNullOrEmpty(tblGudang.v_telp) ? tblGudangEdit.v_telp : tblGudang.c_kodepos.Trim());

                    tblGudangEdit.c_update = nipEntry;
                    tblGudangEdit.d_update = date;

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
                rpe = ResponseParser.ResponseParserEnum.IsError;

                if (db.Transaction != null)
                {
                    db.Transaction.Rollback();
                }

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:MasterGudang - {0}", ex.Message);

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

        public string MasterKurs(Parser.Parser.StructureXmlHeaderParser xmlParser, Parser.Parser.StructureDataNamingHeader dataParser)
        {
            if ((xmlParser == null) || (dataParser == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false;
            string result = null;

            MyAssembly myAsm = new MyAssembly();

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            //char gudang = char.MinValue;

            string kursId = null;
            string nipEntry = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now,
              firstDate = Functionals.StandardSqlDateTime;

            IDictionary<string, string> dic = null;

            FA_Kur tblKurs = null,
              tblKursEdit = null;

            int nCount = 0;

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                tblKurs = myAsm.Populate<FA_Kur>(xmlParser, dataParser);

                if (tblKurs == null)
                {
                    result = "Struktur kurs tidak terbaca.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                nipEntry = (tblKurs.c_entry ?? string.Empty);

                if (string.IsNullOrEmpty(nipEntry))
                {
                    result = "Nip dibutuhkan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                if (dataParser.Method == ScmsSoaLibrary.Parser.Parser.InterpreterMethod.IsAdd)
                {
                    #region Add

                    kursId = (from q in db.FA_Kurs
                              orderby q.c_kurs descending
                              select q.c_kurs).Take(1).SingleOrDefault();

                    if (string.IsNullOrEmpty(kursId))
                    {
                        kursId = "01";
                    }
                    else if (kursId.Equals("99"))
                    {
                        result = "Kurs tidak dapat ditambahkan lagi.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else
                    {
                        if (int.TryParse(kursId, out nCount))
                        {
                            nCount++;
                            kursId = nCount.ToString().PadLeft(2, '0');
                        }
                        else
                        {
                            kursId = "01";
                        }
                    }

                    nCount = (from q in db.FA_Kurs
                              where (q.c_kurs == kursId)
                              select q.c_kurs).Count();

                    if (nCount != 0)
                    {
                        result = "Kode kurs sudah ada.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    tblKurs.c_kurs = kursId;

                    tblKurs.d_entry = date;
                    tblKurs.d_update = date;

                    db.FA_Kurs.InsertOnSubmit(tblKurs);

                    dic = new Dictionary<string, string>();

                    dic.Add("Kurs", kursId);
                    dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                    hasAnyChanges = true;

                    #endregion
                }
                else if (dataParser.Method == ScmsSoaLibrary.Parser.Parser.InterpreterMethod.IsUpdate)
                {
                    #region Modify

                    kursId = tblKurs.c_kurs;

                    tblKursEdit = (from q in db.FA_Kurs
                                   where (q.c_kurs == kursId)
                                   select q).Take(1).SingleOrDefault();

                    if (tblKursEdit == null)
                    {
                        result = "Gudang tidak dapat terbaca dari database.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    tblKursEdit.c_symbol = (string.IsNullOrEmpty(tblKurs.c_symbol) ? tblKursEdit.c_symbol : tblKurs.c_symbol.Trim());
                    tblKursEdit.c_desc = (string.IsNullOrEmpty(tblKurs.c_desc) ? tblKursEdit.c_desc : tblKurs.c_desc.Trim());
                    tblKursEdit.v_desc = (string.IsNullOrEmpty(tblKurs.v_desc) ? tblKursEdit.v_desc : tblKurs.v_desc.Trim());
                    tblKursEdit.n_currency = (tblKurs.n_currency.HasValue ? tblKursEdit.n_currency.Value : tblKurs.n_currency);
                    tblKursEdit.l_aktif = (tblKurs.l_aktif.HasValue ? tblKurs.l_aktif.Value : tblKursEdit.l_aktif);

                    tblKursEdit.c_update = nipEntry;
                    tblKursEdit.d_update = date;

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
                rpe = ResponseParser.ResponseParserEnum.IsError;

                if (db.Transaction != null)
                {
                    db.Transaction.Rollback();
                }

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:MasterKurs - {0}", ex.Message);

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

        public string Prinsipal(ScmsSoaLibrary.Parser.Class.MasterPrinsipalStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;
            LG_DatSup sup = null;
            Lg_Datsup_Approval supapp = null;
            HR_Email email = null;
            HR_Email email2 = null;
            HR_Bawahan bawahan = null;

            DateTime date = DateTime.Today;

            ScmsSoaLibrary.Parser.Class.MasterPrinsipalStructureFields field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            //int totalDetails = 0;

            string nipEntry = null;
            string SupplierId = null;
            string idDA = null;
            string TipeApprove = null;

            string Sender = "scms.dophar@ams.co.id";
            string TextSender = "Supply Chain Management System";
            string Received = "";
            string CarbonCopy = "";
            string Subject = "";
            string EmailHeader = "";
            string EmailContent = "";
            string EmailFooter = "";
            string ModulPath = "";

            nipEntry = (structure.Fields.Entry ?? string.Empty);

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

                    if (string.IsNullOrEmpty(structure.Fields.NamaTax))
                    {
                        result = "Nama Prinsipal.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    SupplierId = Commons.GenerateNumberingMaster(db, "00000", "c_nosup");

                    field = structure.Fields;

                    sup = new LG_DatSup()
                    {
                        c_area = field.Area,
                        c_entry = field.Entry,
                        c_nosup = SupplierId,
                        c_kdgol = field.KodeGol,
                        c_update = field.Entry,
                        d_entry = date,
                        d_tglpkp = (field.DateNpkp == DateTime.MinValue ? Convert.ToDateTime(field.Date) : field.DateNpkp),
                        d_update = date,
                        l_aktif = field.IsAktif,
                        l_fax = field.IsFax,
                        l_hide = field.IsHide,
                        l_import = field.IsImport,
                        l_konsinyasi = field.IsKons,
                        n_index = field.Index,
                        n_leadtime = field.Lead,
                        n_top = field.Top,
                        n_xdisc = field.Disc,
                        v_acccode = field.Acc,
                        v_accno1 = field.Acc1,
                        v_accno2 = field.Acc2,
                        v_alamat1 = field.Alamat1,
                        v_alamat2 = field.Alamat2,
                        v_alamatbank = field.AlamatBank,
                        v_boss = field.Owner,
                        v_contact = field.Contact,
                        v_fax1 = field.Fax1.ToString(),
                        v_fax2 = field.Fax2.ToString(),
                        v_fax3 = field.Fax3.ToString(),
                        v_nama = field.Nama,
                        v_namabank = field.Bank,
                        v_namatax = field.NamaTax,
                        v_nppkp = field.Nppkp,
                        v_npwp = field.Npwp,
                        v_taxseri = field.Tax,
                        v_telepon1 = field.Phone1.ToString(),
                        v_telepon2 = field.Phone2.ToString(),
                        v_telepon3 = field.Phone3.ToString(),
                        v_alamat1_tax = field.AlamatTax1,
                        v_alamat2_tax = field.AlamatTax2
                    };

                    dic = new Dictionary<string, string>();

                    dic.Add("SupplierID", SupplierId);

                    if (sup != null)
                    {
                        db.LG_DatSups.InsertOnSubmit(sup);
                        hasAnyChanges = true;
                    }

                    #endregion
                }
                else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    //sup = new LG_DatSup();

                    sup = (from q in db.LG_DatSups
                           where q.c_nosup == structure.Fields.SupplierID
                           select q).Take(1).SingleOrDefault();

                    if (sup == null)
                    {
                        result = "Supplier tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        goto endLogic;
                    }
                    else
                    {


                        #region Leadtime

                        if (structure.Fields.TipePerubahan == "01")
                        {
                            supapp = (from q in db.Lg_Datsup_Approvals
                                      where q.c_nosup == structure.Fields.SupplierID &&
                                            q.c_status == "02"
                                      select q).Take(1).SingleOrDefault();

                            if (supapp == null)
                            {

                                idDA = Commons.GenerateNumbering<Lg_Datsup_Approval>(db, "DA", '3', "68", date, "c_noda");

                                supapp = new Lg_Datsup_Approval()
                                {
                                    c_noda = idDA,
                                    c_nosup = structure.Fields.SupplierID,
                                    n_leadtime_awal = structure.Fields.LeadtimeAwal,
                                    n_leadtime_akhir = structure.Fields.LeadtimePerubahan,
                                    c_alasan_perubahan = structure.Fields.AlasanPerubahan,
                                    c_requestor = structure.Fields.Entry,
                                    d_requestor = date,
                                    d_efectivedate = structure.Fields.DateEfektiveDate,
                                    c_status = structure.Fields.TypeDo,
                                    d_entry = DateTime.Now
                                };

                                sup.c_noda = idDA;

                                if (supapp != null)
                                {

                                    db.Lg_Datsup_Approvals.InsertOnSubmit(supapp);

                                    TipeApprove = "02";
                                                                        
                                }
                            }

                            else
                            {
                                supapp.c_approval = structure.Fields.Entry;
                                supapp.d_approval = date;
                                supapp.c_alasan_approval = structure.Fields.AlasanTolakSetuju;
                                supapp.c_status = structure.Fields.TypeDo;
                                supapp.d_update = DateTime.Now;

                                if (structure.Fields.TypeDo == "01")
                                {
                                    sup.n_leadtime = structure.Fields.LeadtimePerubahan;
                                    sup.c_noda = idDA;

                                    TipeApprove = "01";
                                }

                                TipeApprove = structure.Fields.TypeDo;

                                hasAnyChanges = true;
                            }

                            #region Subject Email

                            if (TipeApprove == "01")
                            {
                                Subject = "Persetujuan Perubahan Leadtime";
                            }
                            else if (TipeApprove == "02")
                            {
                                Subject = "Permintaan Perubahan Leadtime";
                            }
                            else if (TipeApprove == "03")
                            {
                                Subject = "Pembatalan Perubahan Leadtime";
                            }
                            else if (TipeApprove == "04")
                            {
                                Subject = "Penolakan Perubahan Leadtime";
                            }

                            else
                            {
                                result = "Tipe approval tidak ditemukan.";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                goto endLogic;
                            }

                            #endregion

                            if ((TipeApprove == "01") || (TipeApprove == "04"))
                            {
                                bawahan = (from q in db.HR_Bawahans
                                           where q.c_nipatsn == structure.Fields.Entry &&
                                                 q.c_nipbwhn == supapp.c_requestor
                                           select q).Take(1).SingleOrDefault();

                                email = (from q in db.HR_Emails
                                         where q.c_nip == bawahan.c_nipatsn
                                         select q).Take(1).SingleOrDefault();

                                email2 = (from q in db.HR_Emails
                                          where q.c_nip == bawahan.c_nipbwhn
                                          select q).Take(1).SingleOrDefault();

                                Received = email2.v_email;
                                CarbonCopy = email.v_email;

                                EmailHeader = "Yth Bpk/Ibu " + bawahan.v_namabwhn + ", <br /><br /><br /><br />";



                            }
                            else
                            {
                                bawahan = (from q in db.HR_Bawahans
                                           where q.c_nipbwhn == structure.Fields.Entry
                                           select q).Take(1).SingleOrDefault();

                                email = (from q in db.HR_Emails
                                         where q.c_nip == bawahan.c_nipbwhn
                                         select q).Take(1).SingleOrDefault();

                                email2 = (from q in db.HR_Emails
                                          where q.c_nip == bawahan.c_nipatsn
                                          select q).Take(1).SingleOrDefault();

                                Received = email2.v_email;
                                CarbonCopy = email.v_email;

                                EmailHeader = "Yth Bpk/Ibu " + bawahan.v_namaatsn + ", <br /><br /><br /><br />";
                            }

                            if (bawahan != null)
                            {

                                #region Konten Email

                                if (TipeApprove == "01")
                                {
                                    EmailContent = "Persetujuan perubahan leadtime principle " + sup.v_nama + ". <br />Klik link di bawah ini untuk menuju modul perubahan leadtime<br />http://10.100.41.30/web/master/prinsipal/MasterPrinsipal.aspx <br /><br /><br /><br />";
                                }
                                else if (TipeApprove == "02")
                                {
                                    EmailContent = "Permintaan perubahan leadtime principle " + sup.v_nama + ". <br />Mohon untuk ditindak lanjuti.<br />Klik link di bawah ini untuk menuju modul perubahan leadtime<br />http://10.100.41.30/web/master/prinsipal/MasterPrinsipal.aspx <br /><br /><br /><br />";
                                }
                                else if (TipeApprove == "03")
                                {
                                    EmailContent = "Pembatalan perubahan leadtime principle " + sup.v_nama + ". <br />Klik link di bawah ini untuk menuju modul perubahan leadtime<br />http://10.100.41.30/web/master/prinsipal/MasterPrinsipal.aspx <br /><br /><br /><br />";
                                }
                                else if (TipeApprove == "04")
                                {
                                    EmailContent = "Penolakan perubahan leadtime principle " + sup.v_nama + ". <br />Klik link di bawah ini untuk menuju modul perubahan leadtime<br />http://10.100.41.30/web/master/prinsipal/MasterPrinsipal.aspx <br /><br /><br /><br />";
                                }

                                else
                                {
                                    result = "Tipe approval tidak ditemukan.";

                                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                    goto endLogic;
                                }

                                #endregion
                               
                                EmailFooter = "Terima Kasih <br /> Salam SCMS Team;";

                                EmailSender.EmailParameter(db,
                                                       Sender,
                                                       TextSender,
                                                       Received + ";",
                                                       CarbonCopy + ";",
                                                       Subject,
                                                       EmailHeader,
                                                       EmailContent,
                                                       "",
                                                       EmailFooter);

                                hasAnyChanges = true;
                            }

                            else
                            {
                                result = "Nip atasan tidak ditemukan.";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                goto endLogic;
                            }

                        }

                        #endregion

                        #region Data Principal

                        else
                        {
                            field = structure.Fields;

                            sup.c_area = field.Area;
                            sup.c_kdgol = field.KodeGol;
                            sup.c_update = field.Entry;
                            sup.d_tglpkp = (field.DateNpkp == DateTime.MinValue ? Convert.ToDateTime(field.Date) : field.DateNpkp);
                            sup.d_update = date;
                            sup.l_aktif = field.IsAktif;
                            sup.l_fax = field.IsFax;
                            sup.l_hide = field.IsHide;
                            sup.l_import = field.IsImport;
                            sup.l_konsinyasi = field.IsKons;
                            sup.n_index = field.Index;
                            //sup.n_leadtime = field.Lead;
                            sup.l_hide = field.IsHide;
                            sup.n_top = field.Top;
                            sup.n_xdisc = field.Disc;
                            sup.v_acccode = field.Acc;
                            sup.v_accno1 = field.Acc1;
                            sup.v_accno2 = field.Acc2;
                            sup.v_alamat1 = field.Alamat1;
                            sup.v_alamat2 = field.Alamat2;
                            sup.v_alamatbank = field.AlamatBank;
                            sup.v_boss = field.Owner;
                            sup.v_contact = field.Contact;
                            sup.v_fax1 = field.Fax1.ToString();
                            sup.v_fax2 = field.Fax2.ToString();
                            sup.v_fax3 = field.Fax3.ToString();
                            sup.v_nama = field.Nama;
                            sup.v_namabank = field.Bank;
                            sup.v_namatax = field.NamaTax;
                            sup.v_nppkp = field.Nppkp;
                            sup.v_npwp = field.Npwp;
                            sup.v_taxseri = field.Tax;
                            sup.v_telepon1 = field.Phone1.ToString();
                            sup.v_telepon2 = field.Phone2.ToString();
                            sup.v_telepon3 = field.Phone3.ToString();
                            sup.v_alamat1_tax = field.AlamatTax1;
                            sup.v_alamat2_tax = field.AlamatTax2;

                            hasAnyChanges = true;
                        }

                        #endregion

                        dic = new Dictionary<string, string>();

                        dic.Add("SupplierID", SupplierId);
                        
                    }
                    

                    #endregion
                }
                else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    //sup = (from q in db.LG_DatSups
                    //       where q.c_nosup == structure.Fields.SupplierID
                    //       select q).Take(1).SingleOrDefault();

                    //sup.l_delete = true;
                    //sup.d_update = date;
                    //sup.c_update = structure.Fields.Entry;
                    //sup.v_ket_del = structure.Fields.Keterangan;

                    //hasAnyChanges = true;

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:Prisipal - {0}", ex.Message);

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

        public string DivisiPrinsipal(ScmsSoaLibrary.Parser.Class.MasterDivisiPrinsipalStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;
            FA_MsDivPri DivSup = null;

            DateTime date = DateTime.Today;

            ScmsSoaLibrary.Parser.Class.MasterDivisiPrinsipalStructureFields field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            //int totalDetails = 0;

            string nipEntry = null;
            string DivSupplierId = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

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

                    if (string.IsNullOrEmpty(structure.Fields.Nama))
                    {
                        result = "Nama Divisi Prinsipal.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    DivSupplierId = Commons.GenerateNumberingMaster(db, "000", "c_kddivpri");

                    field = structure.Fields;

                    DivSup = new FA_MsDivPri()
                    {
                        c_entry = field.Entry,
                        c_kddivpri = DivSupplierId,
                        c_nosup = field.SupplierId,
                        c_update = field.Entry,
                        d_entry = date,
                        d_update = date,
                        l_aktif = field.isAktif,
                        l_delete = false,
                        l_hide = field.isHide,
                        n_het = field.het,
                        n_idxnp = field.idxnp,
                        n_idxp = field.idxp,
                        v_nmdivpri = field.Nama
                    };

                    if (DivSup != null)
                    {
                        db.FA_MsDivPris.InsertOnSubmit(DivSup);
                        hasAnyChanges = true;
                    }

                    dic = new Dictionary<string, string>();

                    dic.Add("DivSupplierID", DivSupplierId);

                    #endregion
                }
                if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    field = structure.Fields;

                    if (string.IsNullOrEmpty(field.DivSupplierID))
                    {
                        result = "Nomor Master Divisi Prinsipal dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    DivSup = (from q in db.FA_MsDivPris
                              where (q.c_nosup == field.SupplierId) && (q.c_kddivpri == field.DivSupplierID)
                              select q).Take(1).SingleOrDefault();

                    if (DivSup == null)
                    {
                        result = "Master divisi prinsipal tidak dapat terbaca dari database.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    DivSup.c_update = field.Entry;
                    DivSup.d_update = date;
                    DivSup.l_aktif = field.isAktif;
                    DivSup.l_hide = field.isHide;
                    DivSup.n_het = field.het;
                    DivSup.n_idxnp = field.idxnp;
                    DivSup.n_idxp = field.idxp;
                    DivSup.v_nmdivpri = field.Nama;

                    hasAnyChanges = true;

                    #endregion
                }
                if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    field = structure.Fields;

                    if (string.IsNullOrEmpty(field.DivSupplierID))
                    {
                        result = "Nomor Master Divisi Prinsipal dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    DivSup = (from q in db.FA_MsDivPris
                              where (q.c_nosup == field.SupplierId) && (q.c_kddivpri == field.DivSupplierID)
                              select q).Take(1).SingleOrDefault();

                    if (DivSup == null)
                    {
                        result = "Master divisi prinsipal tidak dapat terbaca dari database.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    DivSup.l_delete = true;

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:DivisiPrisipal - {0}", ex.Message);

                Logger.WriteLine(result, true);
                Logger.WriteLine(ex.StackTrace);
            }

        endLogic:
            result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

            if (dic != null)
            {
                dic.Clear();
            }

            if (rpe == ResponseParser.ResponseParserEnum.IsSuccess)
            {
                Commons.RunningSendMasterDivPrinsipal(db, structure.Method, DivSup);
            }

            db.Dispose();

            return result;
        }

        public string Customer(ScmsSoaLibrary.Parser.Class.MasterCustomerStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;
            LG_Cusma Cusmas = null;
            LG_TRapproval approval = null;
            DateTime date = DateTime.Today;

            ScmsSoaLibrary.Parser.Class.MasterCustomerStructureFields field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            char gdg = (string.IsNullOrEmpty(structure.Fields.Gudang) ? char.MinValue : structure.Fields.Gudang[0]);

            //int totalDetails = 0;

            string nipEntry = null;
            string CustomerId = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

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

                    if (string.IsNullOrEmpty(structure.Fields.Nama))
                    {
                        result = "Nama Customer.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    CustomerId = Commons.GenerateNumberingMaster(db, "0000", "c_cusno");

                    field = structure.Fields;

                    DateTime DateOpen = DateTime.Parse(field.DateOpen);
                    DateTime DateClose = DateTime.Parse(field.DateClose);
                    DateTime DateNP = DateTime.Parse(field.DateNP);

                    if (!string.IsNullOrEmpty(field.SpDays))
                    {
                        int spdays = Int16.Parse(field.SpDays);
                        int spdaysint = Int16.Parse(field.SpDaysInternal);
                        int spdayseksp = Int16.Parse(field.SpDaysEksternal);

                        Cusmas = new LG_Cusma()
                        {
                            c_cab = field.Kodecab,
                            c_cusno = CustomerId,
                            c_entry = field.Entry,
                            d_buka = DateOpen,
                            c_gdg = gdg,
                            c_npkp = field.NPKP,
                            c_npwp = field.NPWP,
                            c_update = field.Entry,
                            d_entry = date,
                            d_npkp = DateNP,
                            d_tutup = DateClose,
                            d_update = date,
                            l_askes = field.isAkses,
                            l_cabang = field.isCabang,
                            l_dispen = field.isDispen,
                            l_hide = field.isHide,
                            l_materai = field.isMaterai,
                            l_stscus = field.isStatus,
                            n_crlimit = field.Limit,
                            n_fee = field.Fee,
                            t_top = field.TOP,
                            t_toppjg = field.TOPPjg,
                            v_accno = field.Account,
                            v_adrbill1 = field.Addr1,
                            v_adrbill2 = field.Addr2,
                            v_adrtax1 = field.AddrTax1,
                            v_adrtax2 = field.AddrTax2,
                            v_citybill = field.Kota,
                            v_citytax = field.KotaTax,
                            v_cunam = field.Nama,
                            v_fax = field.Fax,
                            v_nmbank = field.NmBank,
                            v_nmowner = field.Pemilik,
                            v_tagih = field.Tagih,
                            v_taxname = field.TaxName,
                            v_telp1 = field.Telp1,
                            v_telp2 = field.Telp2,
                            v_zipbill = field.KdPos,
                            v_ziptax = field.KdPosTax,
                            c_sektor = field.Sektor,
                            n_days = null,
                            n_days_internal = spdaysint,
                            n_days_ekspedisi = spdayseksp
                        };
                    }
                    else
                    {
                        Cusmas = new LG_Cusma()
                        {
                            c_cab = field.Kodecab,
                            c_cusno = CustomerId,
                            c_entry = field.Entry,
                            d_buka = DateOpen,
                            c_gdg = gdg,
                            c_npkp = field.NPKP,
                            c_npwp = field.NPWP,
                            c_update = field.Entry,
                            d_entry = date,
                            d_npkp = DateNP,
                            d_tutup = DateClose,
                            d_update = date,
                            l_askes = field.isAkses,
                            l_cabang = field.isCabang,
                            l_dispen = field.isDispen,
                            l_hide = field.isHide,
                            l_materai = field.isMaterai,
                            l_stscus = field.isStatus,
                            n_crlimit = field.Limit,
                            n_fee = field.Fee,
                            t_top = field.TOP,
                            t_toppjg = field.TOPPjg,
                            v_accno = field.Account,
                            v_adrbill1 = field.Addr1,
                            v_adrbill2 = field.Addr2,
                            v_adrtax1 = field.AddrTax1,
                            v_adrtax2 = field.AddrTax2,
                            v_citybill = field.Kota,
                            v_citytax = field.KotaTax,
                            v_cunam = field.Nama,
                            v_fax = field.Fax,
                            v_nmbank = field.NmBank,
                            v_nmowner = field.Pemilik,
                            v_tagih = field.Tagih,
                            v_taxname = field.TaxName,
                            v_telp1 = field.Telp1,
                            v_telp2 = field.Telp2,
                            v_zipbill = field.KdPos,
                            v_ziptax = field.KdPosTax,
                            c_sektor = field.Sektor
                        };
                    }

                    dic = new Dictionary<string, string>();
                    dic.Add("customerId", CustomerId);

                    if (Cusmas != null)
                    {
                        db.LG_Cusmas.InsertOnSubmit(Cusmas);
                        hasAnyChanges = true;

                    }

                    #endregion
                }
                else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    field = structure.Fields;

                    Cusmas = (from q in db.LG_Cusmas
                              where q.c_cusno == field.CustomerID
                              select q).Take(1).SingleOrDefault();

                    DateTime DateOpen = DateTime.Parse(field.DateOpen);
                    DateTime DateClose = DateTime.Parse(field.DateClose);
                    DateTime DateNP = DateTime.Parse(field.DateNP);
                    if (DateOpen <= DateTime.Parse("1/1/1900 12:00:00 AM"))
                    {
                        DateOpen = DateTime.Parse("1/1/1900 12:00:00 AM");
                    }
                    if (DateClose <= DateTime.Parse("1/1/1900 12:00:00 AM"))
                    {
                        DateClose = DateTime.Parse("1/1/1900 12:00:00 AM");
                    }
                    if (DateNP <= DateTime.Parse("1/1/1900 12:00:00 AM"))
                    {
                        DateNP = DateTime.Parse("1/1/1900 12:00:00 AM");
                    }
                    Cusmas.c_cab = field.Kodecab;
                    Cusmas.c_entry = field.Entry;
                    Cusmas.d_buka = DateOpen;
                    Cusmas.c_gdg = gdg;
                    Cusmas.c_npkp = field.NPKP;
                    Cusmas.c_npwp = field.NPWP;
                    Cusmas.c_update = field.Entry;
                    Cusmas.d_entry = date;
                    Cusmas.d_npkp = DateNP;
                    Cusmas.d_tutup = DateClose;
                    Cusmas.d_update = date;
                    Cusmas.l_askes = field.isAkses;
                    Cusmas.l_cabang = field.isCabang;
                    Cusmas.l_dispen = field.isDispen;
                    Cusmas.l_hide = field.isHide;
                    Cusmas.l_materai = field.isMaterai;
                    Cusmas.l_stscus = field.isStatus;
                    Cusmas.n_crlimit = field.Limit;
                    Cusmas.n_fee = field.Fee;
                    Cusmas.t_top = field.TOP;
                    Cusmas.t_toppjg = field.TOPPjg;
                    Cusmas.v_accno = field.Account;
                    Cusmas.v_adrbill1 = field.Addr1;
                    Cusmas.v_adrbill2 = field.Addr2;
                    Cusmas.v_adrtax1 = field.AddrTax1;
                    Cusmas.v_adrtax2 = field.AddrTax2;
                    Cusmas.v_citybill = field.Kota;
                    Cusmas.v_citytax = field.KotaTax;
                    Cusmas.v_cunam = field.Nama;
                    Cusmas.v_fax = field.Fax;
                    Cusmas.v_nmbank = field.NmBank;
                    Cusmas.v_nmowner = field.Pemilik;
                    Cusmas.v_tagih = field.Tagih;
                    Cusmas.v_taxname = field.TaxName;
                    Cusmas.v_telp1 = field.Telp1;
                    Cusmas.v_telp2 = field.Telp2;
                    Cusmas.v_zipbill = field.KdPos;
                    Cusmas.v_ziptax = field.KdPosTax;
                    //if (!string.IsNullOrEmpty(field.SpDays))
                    //{
                    //    Cusmas.n_days = Int16.Parse(field.SpDays);
                    //}
                    Cusmas.c_sektor = field.Sektor;

                    approval = new LG_TRapproval()
                    {
                        kd_approval = "MSCAB",
                        NIP_request = structure.Fields.Entry,
                        d_entry = DateTime.Now,
                        param = field.SpDaysInternal,
                        param2 = field.SpDaysEksternal,
                        c_cusno = structure.Fields.CustomerID,
                        status = "waiting"                        
                    };
                    db.LG_TRapprovals.InsertOnSubmit(approval);
                    hasAnyChanges = true;
                    
                    System.Net.Mail.SmtpClient smtp = null;
                    StringBuilder sb = new StringBuilder();
                    try
                    {
                        using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                        {
                            // send mail containing the file here

                            mail.From = new System.Net.Mail.MailAddress("scms.sph@ams.co.id", "Supply Chain Management System");

                            mail.Subject = "Approval Leadtime Cabang " + structure.Fields.Nama + "";

                            mail.To.Add("bungaran@ams.co.id");
                            mail.CC.Add("hasudungan.pakpahan@ams.co.id");
                            sb.AppendLine("Dear Bapak/Ibu,");
                            sb.AppendLine("");
                            sb.AppendLine("Pengajuan atas perubahan Leadtime cabang " + structure.Fields.Nama + " ");
                            sb.AppendLine("Mohon di approve/reject melalui aplikasi SCMS di http://10.100.41.30/web");
                            sb.AppendLine("");
                            sb.AppendLine("Lead Time Internal Awal = " + Cusmas.n_days_internal + ", Pengajuan Lead Time Internal = " + structure.Fields.SpDaysInternal);
                            sb.AppendLine("Lead Time Ekspedisi Awal = " + Cusmas.n_days_ekspedisi + ", Pengajuan Lead Time Ekspedisi = " + structure.Fields.SpDaysEksternal);
                            sb.AppendLine("");
                            sb.AppendLine("Terima Kasih,");
                            sb.AppendLine("AMS - MIS Team");

                            mail.Body = sb.ToString();

                            smtp = new System.Net.Mail.SmtpClient("10.100.10.9", 25);

                            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new System.Net.NetworkCredential("scms.sph@ams.co.id", "scms");

                            smtp.Send(mail);
                            sb.Length = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                    #endregion
                }
                else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    //field = structure.Fields;

                    //Cusmas = (from q in db.LG_Cusmas
                    //          where q.c_cusno == field.CustomerID
                    //          select q).Take(1).SingleOrDefault();

                    //Cusmas.l_delete = true;
                    //Cusmas.v_ket_del = (structure.Fields.Keterangan ?? "Human Error");
                    //Cusmas.d_update = date;
                    //Cusmas.c_update = field.Entry;

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:Customer - {0}", ex.Message);

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

        //suwandi 15 agustus 2018
        public string ApproveRejectMasterCustomer(ScmsSoaLibrary.Parser.Class.MasterApprovalStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }
            string result = null;
            int days = 0;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            IDictionary<string, string> dic = null;
            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);
            string nipEntry = (structure.Fields.NIP ?? string.Empty);
            LG_TRapproval approval = null;
            LG_Cusma cusmas = new LG_Cusma();
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
                approval = new LG_TRapproval()
                {
                    kd_approval = structure.Fields.kdapproval,
                    NIP_request = structure.Fields.NIP,
                    d_entry = DateTime.Now,
                    param = structure.Fields.param,
                    param2 = structure.Fields.param2,
                    c_cusno = structure.Fields.cusno,
                    status = structure.Fields.status
                };
                db.LG_TRapprovals.InsertOnSubmit(approval);
                cusmas = (from q in db.LG_Cusmas where q.c_cusno == structure.Fields.cusno select q).Take(1).SingleOrDefault();
                string ketcusmas = cusmas.v_cunam;
                days = Int16.Parse(structure.Fields.param) + Int16.Parse(structure.Fields.param2);
                if (structure.Fields.status == "approve")
                {
                    cusmas.n_days_internal = Int16.Parse(structure.Fields.param);
                    cusmas.n_days_ekspedisi = Int16.Parse(structure.Fields.param2);
                    cusmas.n_days = null;
                    cusmas.d_update = DateTime.Now;
                    System.Net.Mail.SmtpClient smtp = null;
                    StringBuilder sb = new StringBuilder();
                    try
                    {
                        using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                        {
                            // send mail containing the file here

                            mail.From = new System.Net.Mail.MailAddress("scms.sph@ams.co.id", "Supply Chain Management System");

                            mail.Subject = "Approval Leadtime Cabang " + ketcusmas + " telah di approve";

                            mail.To.Add("hasudungan.pakpahan@ams.co.id");
                            mail.CC.Add("bungaran@ams.co.id");
                            sb.AppendLine("Dear Bapak/Ibu,");
                            sb.AppendLine("");
                            sb.AppendLine("Pengajuan anda atas perubahan Leadtime cabang " + ketcusmas + " telah di approve");
                            sb.AppendLine("");
                            sb.AppendLine("");
                            sb.AppendLine("Terima Kasih,");
                            sb.AppendLine("AMS - MIS Team");

                            mail.Body = sb.ToString();

                            smtp = new System.Net.Mail.SmtpClient("10.100.10.9", 25);

                            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new System.Net.NetworkCredential("scms.sph@ams.co.id", "scms");

                            smtp.Send(mail);
                            sb.Length = 0;
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                    rpe = ResponseParser.ResponseParserEnum.IsSuccess;
                }
                else if (structure.Fields.status == "reject")
                {
                    System.Net.Mail.SmtpClient smtp = null;
                    StringBuilder sb = new StringBuilder();
                    try
                    {
                        using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                        {
                            // send mail containing the file here

                            mail.From = new System.Net.Mail.MailAddress("scms.sph@ams.co.id", "Supply Chain Management System");

                            mail.Subject = "Approval Leadtime Cabang " + ketcusmas + " telah di Reject";

                            mail.To.Add("hasudungan.pakpahan@ams.co.id");
                            mail.CC.Add("bungaran@ams.co.id");
                            sb.AppendLine("Dear Bapak/Ibu,");
                            sb.AppendLine("");
                            sb.AppendLine("Pengajuan anda atas perubahan Leadtime cabang " + ketcusmas + " telah di Reject");
                            sb.AppendLine("");
                            sb.AppendLine("");
                            sb.AppendLine("Terima Kasih,");
                            sb.AppendLine("AMS - MIS Team");

                            mail.Body = sb.ToString();

                            smtp = new System.Net.Mail.SmtpClient("10.100.10.9", 25);

                            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new System.Net.NetworkCredential("scms.sph@ams.co.id", "scms");

                            smtp.Send(mail);
                            sb.Length = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                    rpe = ResponseParser.ResponseParserEnum.IsSuccess;
                    result = "Anda telah mereject perubahan yang di minta.";
                    db.SubmitChanges();
                    db.Transaction.Commit();
                    goto endLogic;
                }
            }
            catch (Exception ex)
            {
                db.Transaction.Rollback();
                rpe = ResponseParser.ResponseParserEnum.IsFailed;
                result = string.Format("ScmsSoaLibrary.Bussiness.Master:Customer - {0}", ex.Message);
                goto endLogic;
            }
            db.SubmitChanges();
            db.Transaction.Commit();
            var qry = (from q in db.LG_CusmasCabs
                       where q.c_cusno == structure.Fields.cusno
                       select q).SingleOrDefault();
            var connectionStringSql = "Driver={SQL Server};Server=10.100.41.29;Database=AMS;Uid=sa;Pwd=4M5M1s2015";
            using (OdbcConnection con = new OdbcConnection(connectionStringSql))
            {
                con.Open();
                OdbcCommand cmd = new OdbcCommand();
                cmd.Connection = con;
                string sSql = "update openquery(dcoreprod,'select * from tbl_param_calc_sp where c_iteno = '''' and c_kodecab = ''" + qry.c_cab_dcore + "''') set n_leadin = " + structure.Fields.param + ", n_leadex = " + structure.Fields.param2;
                cmd.CommandText = sSql;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = new OdbcCommand();
                cmd.Connection = con;
                sSql = "update lg_cusmas set n_days_internal = '" + structure.Fields.param + "', n_days_ekspedisi = '" + structure.Fields.param2 + "', n_days = '" + days + "' where c_cusno = '" + structure.Fields.cusno + "'";
                cmd.CommandText = sSql;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            endLogic:
            result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

            return result;
        }

        public string Expedisi(ScmsSoaLibrary.Parser.Class.MasterExpedisiStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;
            LG_MsExp exp = null;

            DateTime date = DateTime.Today;

            ScmsSoaLibrary.Parser.Class.MasterExpedisiStructureFields field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            int totalDetails = 0;

            string nipEntry = null;
            string ExpId = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

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

                    field = structure.Fields;

                    if (string.IsNullOrEmpty(field.Nama))
                    {
                        result = "Nama ekspedisi dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        goto endLogic;
                    }

                    ExpId = Commons.GenerateNumberingMaster(db, "00", "c_exp");

                    exp = new LG_MsExp()
                    {
                        c_exp = ExpId,
                        l_aktif = field.isAktif,
                        l_darat = field.isDarat,
                        l_delete = false,
                        l_import = field.isImport,
                        l_laut = field.isLaut,
                        l_udara = field.isUdara,
                        v_ket = field.Nama,
                        l_npwp = field.isNpwp,
                        c_npwp = field.Npwp
                    };

                    dic = new Dictionary<string, string>();

                    if (exp != null)
                    {
                        db.LG_MsExps.InsertOnSubmit(exp);

                        dic.Add("ExpID", ExpId);
                        dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                        result = string.Format("Total {0} detail(s)", totalDetails);

                        hasAnyChanges = true;
                    }

                    #endregion
                }
                else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    field = structure.Fields;

                    if (string.IsNullOrEmpty(field.Nama))
                    {
                        result = "Nama ekspedisi dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        goto endLogic;
                    }

                    exp = (from q in db.LG_MsExps
                           where q.c_exp == field.ExpID
                           select q).Take(1).SingleOrDefault();

                    field = structure.Fields;

                    exp.l_aktif = field.isAktif;
                    exp.l_darat = field.isDarat;
                    exp.l_import = field.isImport;
                    exp.l_laut = field.isLaut;
                    exp.l_udara = field.isUdara;
                    exp.v_ket = field.Nama;
                    exp.l_npwp = field.isNpwp;
                    exp.c_npwp = field.Npwp;

                    hasAnyChanges = true;

                    #endregion
                }
                else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    //exp = (from q in db.LG_MsExps
                    //       where q.c_exp == field.ExpID
                    //       select q).Take(1).SingleOrDefault();

                    //exp.l_delete = true;

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:Expedisi - {0}", ex.Message);

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
        //
        public string CabangHari(ScmsSoaLibrary.Parser.Class.MasterCabangHariStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;
            LG_MsHari exp = null;

            DateTime date = DateTime.Today;

            ScmsSoaLibrary.Parser.Class.MasterCabangHariStructureFields field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            int totalDetails = 0;

            string nipEntry = null;
            string ExpId = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

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
                    #region Modify

                    field = structure.Fields;

                    if (string.IsNullOrEmpty(field.Nama))
                    {
                        result = "Nama ekspedisi dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        goto endLogic;
                    }

                    exp = (from q in db.LG_MsHaris
                           where q.c_cusno == field.ExpID
                           select q).Take(1).SingleOrDefault();

                    field = structure.Fields;

                    exp.v_cunam = field.Nama;
                    exp.l_senin = field.issenin;
                    exp.l_selasa = field.isselasa;
                    exp.l_rabu = field.israbu;
                    exp.l_kamis = field.iskamis;
                    exp.l_jumat = field.isjumat;
                    exp.l_sabtu = field.issabtu;
                    

                    hasAnyChanges = true;

                    #endregion
                }
                else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    //exp = (from q in db.LG_MsExps
                    //       where q.c_exp == field.ExpID
                    //       select q).Take(1).SingleOrDefault();

                    //exp.l_delete = true;

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:Expedisi - {0}", ex.Message);

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
        //

        public string ExpedisiEstimasi(ScmsSoaLibrary.Parser.Class.MasterExpedisiEstimasiStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;
            List<LG_MsExpEst> EstExp = null;
            LG_MsExpEst EstE = null;

            DateTime date = DateTime.Today;

            ScmsSoaLibrary.Parser.Class.MasterExpedisiEstimasiStructureField field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            //int totalDetails = 0;

            string nipEntry = null;
            int nLoop = 0;
            //string ExpId = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

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

                    if (string.IsNullOrEmpty(structure.Fields.ExpId))
                    {
                        result = "Ekspedisi dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        goto endLogic;
                    }

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        EstExp = new List<LG_MsExpEst>();
                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            EstExp.Add(new LG_MsExpEst()
                            {
                                c_cusno = field.Customer,
                                c_cab = field.Customer,
                                c_exp = structure.Fields.ExpId,
                                l_delete = false,
                                t_daratlaut = field.nDarat,
                                t_icepack = field.nIce,
                                t_import = field.nImport,
                                t_udara = field.nUdara,
                                c_insert = nipEntry,
                                c_update = nipEntry,
                                d_insert = date,
                                d_update = date
                            });
                        }

                        if (EstExp.Count > 0)
                        {
                            db.LG_MsExpEsts.InsertAllOnSubmit(EstExp.ToArray());
                        }
                    }

                    hasAnyChanges = true;

                    #endregion
                }
                else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    if (string.IsNullOrEmpty(structure.Fields.ExpId))
                    {
                        result = "Ekspedisi dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        goto endLogic;
                    }

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        EstExp = new List<LG_MsExpEst>();
                        EstE = new LG_MsExpEst();

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if (field.IsNew && (!field.IsModified) && (!field.IsDelete))
                            {
                                EstExp.Add(new LG_MsExpEst()
                                {
                                    c_cusno = field.Customer,
                                    c_cab = field.Customer,
                                    c_exp = structure.Fields.ExpId,
                                    l_delete = false,
                                    t_daratlaut = field.nDarat,
                                    t_icepack = field.nIce,
                                    t_import = field.nImport,
                                    t_udara = field.nUdara,
                                    c_insert = nipEntry,
                                    c_update = nipEntry,
                                    d_insert = date,
                                    d_update = date
                                });

                            }
                            else if ((!field.IsNew) && field.IsModified && (!field.IsDelete))
                            {

                                EstE = (from q in db.LG_MsExpEsts
                                        where q.c_exp == structure.Fields.ExpId
                                        && q.c_cusno == field.Customer
                                        select q).Take(1).SingleOrDefault();

                                EstE.t_daratlaut = field.nDarat;
                                EstE.t_icepack = field.nIce;
                                EstE.t_import = field.nImport;
                                EstE.t_udara = field.nUdara;
                                EstE.d_update = date;
                                EstE.c_update = nipEntry;
                            }
                            else if ((!field.IsNew) && (!field.IsModified) && field.IsDelete)
                            {

                                EstE = (from q in db.LG_MsExpEsts
                                        where q.c_exp == structure.Fields.ExpId
                                        && q.c_cusno == field.Customer
                                        select q).Take(1).SingleOrDefault();

                                EstE.l_delete = true;
                                EstE.c_update = nipEntry;
                                EstE.d_update = date;
                                EstE.v_ket_del = field.KeteranganMod;
                            }
                        }

                        if (EstExp.Count > 0)
                        {
                            db.LG_MsExpEsts.InsertAllOnSubmit(EstExp.ToArray());
                        }
                    }

                    hasAnyChanges = true;

                    #endregion
                }
                else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    if (string.IsNullOrEmpty(structure.Fields.ExpId))
                    {
                        result = "Ekspedisi dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        goto endLogic;
                    }

                    var estimasi = (from q in db.LG_MsExpEsts
                                    where q.c_exp == structure.Fields.ExpId
                                    select q).ToList();

                    EstE = new LG_MsExpEst();

                    for (nLoop = 0; estimasi.Count > nLoop; nLoop++)
                    {
                        EstE = (from q in db.LG_MsExpEsts
                                where q.c_exp == structure.Fields.ExpId
                                && q.c_cusno == estimasi[nLoop].c_cusno
                                select q).Take(1).SingleOrDefault();

                        EstE.l_delete = true;
                        EstE.c_update = nipEntry;
                        EstE.d_update = date;
                        EstE.v_ket_del = structure.Fields.Keterangan;
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

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:ExpedisiEstimasi - {0}", ex.Message);

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

        public string ExpedisiBiaya(ScmsSoaLibrary.Parser.Class.MasterExpedisiEstimasiBiayaStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;
            List<LG_MsExpCostEst> ExpBiaya = null;
            LG_MsExpCostEst ExpB = null;

            DateTime date = DateTime.Today;

            ScmsSoaLibrary.Parser.Class.MasterExpedisiEstimasiBiayaStructureField field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            //int totalDetails = 0;

            string nipEntry = null;
            int nLoop = 0;
            char gdg;
            
            //string ExpId = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);
            //char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '1' : structure.Fields.Gudang[0]);


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

                    if (string.IsNullOrEmpty(structure.Fields.ExpId))
                    {
                        result = "Ekspedisi dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        goto endLogic;
                    }

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        ExpBiaya = new List<LG_MsExpCostEst>();
                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];
                            gdg = string.IsNullOrEmpty(field.Gudang) ? '1' : field.Gudang[0];

                            ExpBiaya.Add(new LG_MsExpCostEst()
                            {
                                c_gdg = gdg,
                                c_cusno = field.Customer,
                                c_exp = structure.Fields.ExpId,
                                n_udara = field.nUdara,
                                n_daratlaut = field.nDarat,
                                n_icepack = field.nIce,
                                n_cdd = field.nCdd,
                                n_fuso = field.nFuso,
                                n_tronton = field.nTronton,
                                n_container = field.nContainer,
                                n_cde = field.nCde,
                                n_l300 = field.nL300,
                                n_expmin = field.nExpMin,
                                d_effective = field.EffectiveDateFormated,
                                c_entry = nipEntry,
                                c_update = nipEntry,
                                d_entry = DateTime.Now,
                                d_update = DateTime.Now,
                                l_delete = false
                            });
                        }

                        if (ExpBiaya.Count > 0)
                        {
                            db.LG_MsExpCostEsts.InsertAllOnSubmit(ExpBiaya.ToArray());
                        }
                    }

                    hasAnyChanges = true;

                    #endregion
                }
                else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    if (string.IsNullOrEmpty(structure.Fields.ExpId))
                    {
                        result = "Ekspedisi dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        goto endLogic;
                    }

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        ExpBiaya = new List<LG_MsExpCostEst>();
                        ExpB = new LG_MsExpCostEst();

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];
                            gdg = string.IsNullOrEmpty(field.Gudang) ? '1' : field.Gudang[0];

                            if (field.IsNew && (!field.IsModified) && (!field.IsDelete))
                            {
                                //ExpB = (from q in db.LG_MsExpCostEsts
                                //        where q.c_exp == structure.Fields.ExpId
                                //        && q.c_cusno == field.Customer
                                //        && q.c_type == field.TipeKirim
                                //        && q.c_gdg == gudang
                                //        && q.n_expmin == field.nExpMin
                                //        && q.d_effective == field.EffectiveDateFormated
                                //        select q).Take(1).SingleOrDefault();

                                //if (ExpB == null)
                                //{
                                    ExpBiaya.Add(new LG_MsExpCostEst()
                                    {
                                        c_gdg = gdg,
                                        c_exp = structure.Fields.ExpId,
                                        c_cusno = field.Customer,
                                        c_type = field.TipeKirim,
                                        n_udara = field.nUdara,
                                        n_daratlaut = field.nDarat,
                                        n_icepack = field.nIce,
                                        n_cdd = field.nCdd,
                                        n_fuso = field.nFuso,
                                        n_tronton = field.nTronton,
                                        n_container = field.nContainer,
                                        n_cde = field.nCde,
                                        n_l300 = field.nL300,
                                        n_expmin = field.nExpMin,
                                        d_effective = field.EffectiveDateFormated,
                                        c_entry = nipEntry,
                                        c_update = nipEntry,
                                        d_entry = DateTime.Now,
                                        d_update = DateTime.Now
                                    });
                                //}
                                //else
                                //{
                                //    ExpB.n_udara = field.nUdara;
                                //    ExpB.n_daratlaut = field.nDarat;
                                //    ExpB.n_icepack = field.nIce;
                                //    ExpB.n_cdd = field.nCdd;
                                //    ExpB.n_fuso = field.nFuso;
                                //    ExpB.n_tronton = field.nTronton;
                                //    ExpB.n_container = field.nContainer;
                                //    ExpB.d_update = DateTime.Now;
                                //    ExpB.c_update = nipEntry;
                                //    ExpB.l_delete = false;
                                //    ExpB.v_ket_del = string.Empty;
                                //}
                            }
                            else if ((!field.IsNew) && field.IsModified && (!field.IsDelete))
                            {

                                ExpB = (from q in db.LG_MsExpCostEsts
                                        where q.c_exp == structure.Fields.ExpId
                                        && q.c_cusno == field.Customer
                                        && q.c_type == field.TipeKirim
                                        && q.c_gdg == gdg
                                        && q.n_expmin == field.nExpMin
                                        && q.d_effective == field.EffectiveDateFormated
                                        select q).Take(1).SingleOrDefault();

                                ExpB.n_udara = field.nUdara;
                                ExpB.n_daratlaut = field.nDarat;
                                ExpB.n_icepack = field.nIce;
                                ExpB.n_cdd = field.nCdd;
                                ExpB.n_fuso = field.nFuso;
                                ExpB.n_tronton = field.nTronton;
                                ExpB.n_container = field.nContainer;
                                ExpB.n_cde = field.nCde;
                                ExpB.n_l300 = field.nL300;
                                ExpB.d_update = DateTime.Now;
                                ExpB.c_update = nipEntry;
                                ExpB.l_delete = false;
                            }
                            else if ((!field.IsNew) && (!field.IsModified) && field.IsDelete)
                            {

                                ExpB = (from q in db.LG_MsExpCostEsts
                                        where q.c_exp == structure.Fields.ExpId
                                        && q.c_cusno == field.Customer
                                        && q.c_type == field.TipeKirim
                                        && q.c_gdg == gdg
                                        && q.n_expmin == field.nExpMin
                                        && q.d_effective == field.EffectiveDateFormated
                                        select q).Take(1).SingleOrDefault();

                                ExpB.l_delete = true;
                                ExpB.c_update = nipEntry;
                                ExpB.d_update = DateTime.Now;
                                ExpB.v_ket_del = field.KeteranganMod;
                            }
                        }

                        if (ExpBiaya.Count > 0)
                        {
                            db.LG_MsExpCostEsts.InsertAllOnSubmit(ExpBiaya.ToArray());
                        }
                    }

                    hasAnyChanges = true;

                    #endregion
                }
                //else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                //{
                //    #region Delete

                //    if (string.IsNullOrEmpty(structure.Fields.ExpId))
                //    {
                //        result = "Ekspedisi dibutuhkan.";

                //        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                //        goto endLogic;
                //    }

                //    var estimasi = (from q in db.LG_MsExpEsts
                //                    where q.c_exp == structure.Fields.ExpId
                //                    select q).ToList();

                //    ExpB = new LG_MsExpEst();

                //    for (nLoop = 0; estimasi.Count > nLoop; nLoop++)
                //    {
                //        ExpB = (from q in db.LG_MsExpEsts
                //                where q.c_exp == structure.Fields.ExpId
                //                && q.c_cusno == estimasi[nLoop].c_cusno
                //                select q).Take(1).SingleOrDefault();

                //        ExpB.l_delete = true;
                //        ExpB.c_update = nipEntry;
                //        ExpB.d_update = date;
                //        ExpB.v_ket_del = structure.Fields.Keterangan;
                //    }

                //    hasAnyChanges = true;

                //    #endregion
                //}

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:ExpedisiEstimasi - {0}", ex.Message);

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

        public string Bank(ScmsSoaLibrary.Parser.Class.MasterBankStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;
            FA_MsBank msBank = null;
            List<FA_MsBankRek> listRek = null;
            List<FA_MsBankRekD1> listRekD1 = null;
            FA_MsBankRek msRek = null;

            DateTime date = DateTime.Today;

            ScmsSoaLibrary.Parser.Class.MasterBankStructureField field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            int totalDetails = 0;

            string nipEntry = null;
            int nLoop = 0;
            string BankId = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

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

                    BankId = Commons.GenerateNumberingMaster(db, string.Empty, "c_bank");

                    if (string.IsNullOrEmpty(structure.Fields.v_bank))
                    {
                        result = "Nama Bank dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        goto endLogic;
                    }

                    if (string.IsNullOrEmpty(structure.Fields.v_bankcab))
                    {
                        result = "Nama Cabank Bank dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        goto endLogic;
                    }

                    msBank = new FA_MsBank()
                    {
                        c_bank = BankId,
                        c_cab1 = "X9",
                        c_cab2 = string.Empty,
                        c_cab3 = string.Empty,
                        c_cab4 = string.Empty,
                        c_entry = structure.Fields.Entry,
                        c_update = structure.Fields.Entry,
                        d_entry = date,
                        d_update = date,
                        l_aktif = structure.Fields.l_aktif,
                        l_delete = false,
                        v_bank = structure.Fields.v_bank,
                        v_bankcab = structure.Fields.v_bankcab
                    };

                    if (structure.Fields.Field.Length > 0 && structure.Fields.Field != null)
                    {
                        listRek = new List<FA_MsBankRek>();

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if ((!field.IsDelete) && (!field.IsModified) && (field.IsNew))
                            {
                                listRek.Add(new FA_MsBankRek()
                                {
                                    c_bank = BankId,
                                    c_entry = structure.Fields.Entry,
                                    c_update = structure.Fields.Entry,
                                    d_entry = date,
                                    d_update = date,
                                    l_delete = false,
                                    c_glno = field.Glno,
                                    c_rekno = field.Rekening,
                                    c_type = field.Tipe,
                                    v_pemilk = field.Pemilik
                                });
                            }

                        }

                        if (listRek.Count > 0)
                        {
                            db.FA_MsBankReks.InsertAllOnSubmit(listRek.ToArray());
                            listRek.Clear();
                        }
                    }

                    hasAnyChanges = true;

                    if (msBank != null)
                    {
                        db.FA_MsBanks.InsertOnSubmit(msBank);
                    }

                    dic = new Dictionary<string, string>();

                    dic.Add("BankId", BankId);

                    #endregion
                }
                else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    if (string.IsNullOrEmpty(structure.Fields.v_bank))
                    {
                        result = "Nama Bank dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        goto endLogic;
                    }

                    if (string.IsNullOrEmpty(structure.Fields.v_bankcab))
                    {
                        result = "Nama Cabank Bank dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        goto endLogic;
                    }


                    msBank = (from q in db.FA_MsBanks
                              where q.c_bank == structure.Fields.c_bank
                              select q).SingleOrDefault();

                    msBank.v_bank = structure.Fields.v_bank;
                    msBank.v_bankcab = structure.Fields.v_bankcab;
                    msBank.l_aktif = structure.Fields.l_aktif;
                    msBank.c_update = structure.Fields.Entry;
                    msBank.d_update = date;

                    if (structure.Fields.Field.Length > 0 && structure.Fields.Field != null)
                    {
                        msRek = new FA_MsBankRek();
                        listRekD1 = new List<FA_MsBankRekD1>();
                        listRek = new List<FA_MsBankRek>();

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if (string.IsNullOrEmpty(field.idx.ToString()))
                            {
                                result = "id rek bank dibutuhkan.";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                goto endLogic;
                            }

                            if ((!field.IsDelete) && (field.IsModified) && (!field.IsNew))
                            {
                                msRek = (from q in db.FA_MsBankReks
                                         where q.IDX == field.idx
                                         && q.c_bank == structure.Fields.c_bank
                                         select q).SingleOrDefault();

                                msRek.c_glno = field.Glno;
                                msRek.c_update = structure.Fields.Entry;
                                msRek.d_update = date;
                                msRek.c_rekno = field.Rekening;
                                msRek.v_pemilk = field.Pemilik;
                                msRek.c_type = field.Tipe;
                            }

                            if ((field.IsDelete) && (!field.IsModified) && (!field.IsNew))
                            {
                                msRek = (from q in db.FA_MsBankReks
                                         where q.IDX == field.idx
                                         && q.c_bank == structure.Fields.c_bank
                                         select q).SingleOrDefault();

                                msRek.l_delete = true;
                                msRek.c_update = structure.Fields.Entry;
                                msRek.d_update = date;
                                msRek.v_ket_del = field.KeteranganMod;
                            }
                            if ((!field.IsDelete) && (!field.IsModified) && (field.IsNew))
                            {
                                listRek.Add(new FA_MsBankRek()
                                {
                                    c_bank = structure.Fields.c_bank,
                                    c_entry = structure.Fields.Entry,
                                    c_glno = field.Glno,
                                    c_rekno = field.Rekening,
                                    c_type = field.Tipe,
                                    c_update = structure.Fields.Entry,
                                    d_entry = date,
                                    d_update = date,
                                    l_delete = false,
                                    v_pemilk = field.Pemilik
                                });
                            }
                        }

                        if (listRek.Count > 0)
                        {
                            db.FA_MsBankReks.InsertAllOnSubmit(listRek.ToArray());
                            listRek.Clear();
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
                    db.Transaction.Rollback();

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;
                }
            }
            catch (Exception ex)
            {
                if (db.Transaction != null)
                {
                    db.Transaction.Rollback();

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;
                }

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:Expedisi - {0}", ex.Message);

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

        public string BlockItem(ScmsSoaLibrary.Parser.Class.MasterBlockItemStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false,
              isBlocked = false,
              newItemBlock = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;
            List<LG_MsItmBlock> lstItmBlok = new List<LG_MsItmBlock>();
            List<LG_MsItmBlockHistory> lstItmBlokHistory = new List<LG_MsItmBlockHistory>();

            LG_MsItmBlock msItmBlok = null;
            LG_MsItmBlockHistory msItmBlokHist = null;

            DateTime date = DateTime.Today;

            ScmsSoaLibrary.Parser.Class.MasterBlockItemStructureField field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            //int totalDetails = 0;

            string nipEntry = null,
              genRandKey = Functionals.GeneratedRandomUniqueId(50, string.Empty),
              randKey = null;

            int nLoop = 0;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

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
                    #region Modify

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            newItemBlock = false;

                            #region Header

                            msItmBlok = lstItmBlok.Find(delegate(LG_MsItmBlock itm)
                            {
                                return field.Item.Equals(itm.c_iteno.Trim(), StringComparison.OrdinalIgnoreCase);
                            });

                            if (msItmBlok == null)
                            {
                                msItmBlok = (from q in db.LG_MsItmBlocks
                                             where (q.c_iteno == field.Item)
                                             select q).Take(1).SingleOrDefault();

                                if (msItmBlok == null)
                                {
                                    msItmBlok = new LG_MsItmBlock()
                                    {
                                        c_iteno = field.Item,
                                        l_status = field.IsBlocked,
                                        v_mod = genRandKey
                                    };

                                    newItemBlock = true;

                                    db.LG_MsItmBlocks.InsertOnSubmit(msItmBlok);

                                    lstItmBlok.Add(msItmBlok);
                                }
                                else
                                {
                                    isBlocked = (msItmBlok.l_status.HasValue ? msItmBlok.l_status.Value : false);

                                    if (isBlocked == field.IsBlocked)
                                    {
                                        continue;
                                    }

                                    lstItmBlok.Add(msItmBlok);
                                }
                            }
                            else
                            {
                                continue;
                            }

                            if (field.IsBlocked)
                            {
                                randKey = genRandKey;

                                msItmBlok.l_status = true;
                                msItmBlok.v_mod = randKey;
                            }
                            else
                            {
                                randKey = (msItmBlok.v_mod ?? string.Empty);

                                msItmBlok.l_status = false;
                                msItmBlok.v_mod = null;
                            }

                            #endregion

                            if (!string.IsNullOrEmpty(randKey))
                            {
                                #region Detail

                                if (newItemBlock)
                                {
                                    msItmBlokHist = new LG_MsItmBlockHistory()
                                    {
                                        c_iteno = field.Item,
                                        v_mod = randKey,
                                        c_entry = nipEntry,
                                        d_entry = date
                                    };

                                    db.LG_MsItmBlockHistories.InsertOnSubmit(msItmBlokHist);

                                    lstItmBlokHistory.Add(msItmBlokHist);
                                }
                                else
                                {
                                    msItmBlokHist = lstItmBlokHistory.Find(delegate(LG_MsItmBlockHistory itm)
                                    {
                                        return field.Item.Equals(itm.c_iteno.Trim(), StringComparison.OrdinalIgnoreCase) &&
                                          randKey.Equals((string.IsNullOrEmpty(itm.v_mod) ? string.Empty : itm.v_mod.Trim()), StringComparison.OrdinalIgnoreCase);
                                    });

                                    if (msItmBlokHist == null)
                                    {
                                        msItmBlokHist = (from q in db.LG_MsItmBlockHistories
                                                         where (q.c_iteno == field.Item) && (q.v_mod == randKey)
                                                         //&& ((q.l_status.HasValue ? q.l_status.Value : false) == false)
                                                         select q).Take(1).SingleOrDefault();

                                        if (msItmBlokHist == null)
                                        {
                                            msItmBlokHist = new LG_MsItmBlockHistory()
                                            {
                                                c_iteno = field.Item,
                                                l_status = false,
                                                c_entry = nipEntry,
                                                d_entry = date,
                                                v_mod = randKey
                                            };

                                            db.LG_MsItmBlockHistories.InsertOnSubmit(msItmBlokHist);

                                            lstItmBlokHistory.Add(msItmBlokHist);
                                        }
                                        else
                                        {
                                            isBlocked = (msItmBlokHist.l_status.HasValue ? msItmBlokHist.l_status.Value : false);

                                            if (isBlocked == (!field.IsBlocked))
                                            {
                                                continue;
                                            }

                                            lstItmBlokHistory.Add(msItmBlokHist);
                                        }
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                if (!field.IsBlocked)
                                {
                                    msItmBlokHist.l_status = true;
                                    msItmBlokHist.c_update = nipEntry;
                                    msItmBlokHist.d_update = date;
                                }

                                #endregion
                            }
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

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:BlockItem - {0}", ex.Message);

                Logger.WriteLine(result, true);
                Logger.WriteLine(ex.StackTrace);
            }

        endLogic:

            lstItmBlok.Clear();

            lstItmBlokHistory.Clear();

            result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

            if (dic != null)
            {
                dic.Clear();
            }

            db.Dispose();

            return result;
        }

        public string Combo(ScmsSoaLibrary.Parser.Class.MasterComboStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;
            List<FA_Combo> lstCombo = null;
            List<FA_Combo_BAK> lstComboLog = new List<FA_Combo_BAK>();

            FA_Combo msCombo = null;
            FA_Combo_BAK msComboLog = new FA_Combo_BAK();
            FA_MasItm msItem = null;

            DateTime date = DateTime.Today;

            ScmsSoaLibrary.Parser.Class.MasterComboStructureField field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            decimal nQty = 0;

            bool isCombo = false;

            int totalDetails = 0;

            string nipEntry = null;

            int nLoop = 0;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

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
                    #region Modify

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        lstCombo = (from q in db.FA_Combos
                                    where (q.c_combo == structure.Fields.ComboID)
                                    select q).Distinct().ToList();

                        if (lstCombo == null)
                        {
                            lstCombo = new List<FA_Combo>();
                        }

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if (field.IsNew && (!field.IsDelete) && (!field.IsModified) && (!string.IsNullOrEmpty(field.Item)) && (field.Quantity > 0))
                            {
                                #region New

                                msCombo = lstCombo.Find(delegate(FA_Combo fac)
                                {
                                    return (string.IsNullOrEmpty(fac.c_iteno) ? string.Empty : fac.c_iteno.Trim()).Equals(field.Item, StringComparison.OrdinalIgnoreCase);
                                });

                                if (msCombo != null)
                                {
                                    continue;
                                }

                                msCombo = new FA_Combo()
                                {
                                    c_combo = structure.Fields.ComboID,
                                    c_iteno = field.Item,
                                    n_qty = field.Quantity,
                                    c_entry = nipEntry,
                                    d_entry = date
                                };

                                lstCombo.Add(msCombo);

                                //lstComboLog.Add(new FA_Combo_BAK()
                                //{
                                //  c_combo = structure.Fields.ComboID,
                                //  c_iteno = field.Item,
                                //  n_qty = field.Quantity,
                                //  c_entry = nipEntry,
                                //  d_entry = date,
                                //  v_type = "01"
                                //});

                                db.FA_Combos.InsertOnSubmit(msCombo);

                                totalDetails++;

                                #endregion
                            }
                            else if ((!field.IsNew) && (!field.IsDelete) && field.IsModified && (!string.IsNullOrEmpty(field.Item)) && (field.Quantity > 0))
                            {
                                #region Modified

                                msCombo = lstCombo.Find(delegate(FA_Combo cmb)
                                {
                                    return field.Item.Equals((string.IsNullOrEmpty(cmb.c_iteno) ? string.Empty : cmb.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase);
                                });

                                if (msCombo != null)
                                {
                                    nQty = (msCombo.n_qty.HasValue ? msCombo.n_qty.Value : 0);

                                    if (field.Quantity <= 0)
                                    {
                                        lstCombo.Remove(msCombo);

                                        db.FA_Combos.DeleteOnSubmit(msCombo);

                                        lstComboLog.Add(new FA_Combo_BAK()
                                        {
                                            c_combo = structure.Fields.ComboID,
                                            c_iteno = field.Item,
                                            n_qty = nQty,
                                            c_entry = nipEntry,
                                            d_entry = date,
                                            v_type = "03",
                                            v_ket_del = "Qty 0, force to delete."
                                        });
                                    }
                                    else
                                    {
                                        msCombo.n_qty = field.Quantity;

                                        lstComboLog.Add(new FA_Combo_BAK()
                                        {
                                            c_combo = structure.Fields.ComboID,
                                            c_iteno = field.Item,
                                            n_qty = nQty,
                                            c_entry = msCombo.c_entry,
                                            d_entry = msCombo.d_entry,
                                            v_type = "02"
                                        });
                                    }
                                }

                                totalDetails++;

                                #endregion
                            }
                            else if ((!field.IsNew) && field.IsDelete && (!field.IsModified) && (!string.IsNullOrEmpty(field.Item)))
                            {
                                #region Delete

                                msCombo = lstCombo.Find(delegate(FA_Combo cmb)
                                {
                                    return field.Item.Equals((string.IsNullOrEmpty(cmb.c_iteno) ? string.Empty : cmb.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase);
                                });

                                if (msCombo != null)
                                {
                                    lstCombo.Remove(msCombo);

                                    db.FA_Combos.DeleteOnSubmit(msCombo);

                                    lstComboLog.Add(new FA_Combo_BAK()
                                    {
                                        c_combo = structure.Fields.ComboID,
                                        c_iteno = field.Item,
                                        n_qty = msCombo.n_qty,
                                        c_entry = nipEntry,
                                        d_entry = date,
                                        v_type = "03",
                                        v_ket_del = (field.Keterangan ?? "Human error")
                                    });
                                }

                                totalDetails++;

                                #endregion
                            }
                        }

                        isCombo = (lstCombo.Count > 0);

                        msItem = (from q in db.FA_MasItms
                                  where (q.c_iteno == structure.Fields.ComboID)
                                  select q).Take(1).SingleOrDefault();

                        if (msItem != null)
                        {
                            msItem.l_combo = isCombo;
                        }

                        dic = new Dictionary<string, string>();

                        dic.Add("Combo", structure.Fields.ComboID);
                        dic.Add("IsCombo", isCombo.ToString().ToLower());
                        dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                        lstCombo.Clear();
                    }

                    hasAnyChanges = (totalDetails > 0);

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:Combo - {0}", ex.Message);

                Logger.WriteLine(result, true);
                Logger.WriteLine(ex.StackTrace);
            }

        endLogic:

            lstComboLog.Clear();

            result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

            if (dic != null)
            {
                dic.Clear();
            }

            db.Dispose();

            return result;
        }

        public string MasterTransaksi(ScmsSoaLibrary.Parser.Class.MasterTransaksiStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;
            //List<FA_Combo> lstCombo = null;
            //List<FA_Combo_BAK> lstComboLog = new List<FA_Combo_BAK>();

            //FA_Combo msCombo = null;
            //FA_Combo_BAK msComboLog = new FA_Combo_BAK();
            //FA_MasItm msItem = null;

            MsTransH transH = null;
            MsTransD transD = null;

            List<SCMS_MSITEM_CAT> listMsIC = null;
            SCMS_MSITEM_CAT msic = null;

            DateTime date = DateTime.Today;

            ScmsSoaLibrary.Parser.Class.MasterTransaksiStructureField field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            //decimal nQty = 0;

            //int totalDetails = 0;
            int lastTipeId = 0;

            string nipEntry = null,
              tipeId = null;

            int nLoop = 0;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            char portalID = (string.IsNullOrEmpty(structure.Fields.PortalID) || (structure.Fields.PortalID.Length < 1) ? char.MinValue : structure.Fields.PortalID[0]); ;

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            try
            {
                if (portalID.Equals(char.MinValue))
                {
                    result = "Portal ID dibutuhkan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    goto endLogic;
                }
                else if (string.IsNullOrEmpty(structure.Fields.TransaksiID))
                {
                    result = "Transksi ID dibutuhkan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    goto endLogic;
                }

                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase) ||
                  (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase)))
                {
                    #region Add / Modify

                    transH = (from q in db.MsTransHes
                              where (q.c_portal == portalID) && (q.c_notrans == structure.Fields.TransaksiID)
                              select q).Take(1).SingleOrDefault();

                    if (transH == null)
                    {
                        transH = new MsTransH()
                        {
                            c_portal = portalID,
                            c_notrans = structure.Fields.TransaksiID,
                            v_ket = structure.Fields.Deskripsi
                        };

                        db.MsTransHes.InsertOnSubmit(transH);
                    }
                    else
                    {
                        transH.v_ket = structure.Fields.Deskripsi;
                    }

                    field = (((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0)) ? structure.Fields.Field[0] : null);

                    if (field != null)
                    {
                        transD = (from q in db.MsTransDs
                                  where (q.c_portal == portalID) && (q.c_notrans == structure.Fields.TransaksiID)
                                    && (q.c_type == field.TipeID)
                                  select q).Take(1).SingleOrDefault();

                        if (transD == null)
                        {
                            //tipeId = Commons.GenerateNumberingMaster(db, "00", "c_type_trx");
                            tipeId = (from q in db.MsTransDs
                                      where (q.c_portal == portalID) && (q.c_notrans == structure.Fields.TransaksiID)
                                      orderby q.c_type descending
                                      select q.c_type).Take(1).SingleOrDefault();

                            if (string.IsNullOrEmpty(tipeId))
                            {
                                tipeId = "00";
                            }

                            lastTipeId = Convert.ToInt32(tipeId) + 1;

                            tipeId = lastTipeId.ToString("00");

                            transD = new MsTransD()
                            {
                                c_portal = portalID,
                                c_notrans = structure.Fields.TransaksiID,
                                c_type = tipeId,
                                v_ket = field.Deskripsi
                            };

                            db.MsTransDs.InsertOnSubmit(transD);
                        }
                        else
                        {
                            tipeId = (string.IsNullOrEmpty(transD.c_type) ? string.Empty : transD.c_type.Trim());

                            transD.v_ket = field.Deskripsi;
                        }
                    }

                    dic = new Dictionary<string, string>();

                    dic.Add("TipeID", tipeId);
                    dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                    hasAnyChanges = true;

                    #endregion
                }
                else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    field = (((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0)) ? structure.Fields.Field[0] : null);

                    if (field != null)
                    {
                        transD = (from q in db.MsTransDs
                                  where (q.c_portal == portalID) && (q.c_notrans == structure.Fields.TransaksiID)
                                    && (q.c_type == field.TipeID)
                                  select q).Take(1).SingleOrDefault();

                        if (transD == null)
                        {
                            result = "TipeID tidak ditemukan.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }

                        tipeId = (string.IsNullOrEmpty(transD.c_type) ? string.Empty : transD.c_type.Trim());

                        listMsIC = (from q in db.SCMS_MSITEM_CATs
                                    where (q.c_type == tipeId)
                                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                    select q).Distinct().ToList();

                        if ((listMsIC != null) && (listMsIC.Count > 0))
                        {
                            //db.SCMS_MSITEM_CATs.DeleteAllOnSubmit(listMsIC.ToArray());

                            for (nLoop = 0; nLoop < listMsIC.Count; nLoop++)
                            {
                                msic = listMsIC[nLoop];

                                msic.l_delete = true;
                            }

                            listMsIC.Clear();
                        }

                        db.MsTransDs.DeleteOnSubmit(transD);

                        dic = new Dictionary<string, string>();

                        dic.Add("TipeID", tipeId);
                        dic.Add("Tanggal", date.ToString("yyyyMMdd"));

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:MasterTransaksi - {0}", ex.Message);

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

        public string MasterItemCategory(ScmsSoaLibrary.Parser.Class.MasterItemCategoryStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;

            SCMS_MSITEM_CAT msic = null;

            List<SCMS_MSITEM_CAT> listMsIC = null;

            DateTime date = DateTime.Now;

            ScmsSoaLibrary.Parser.Class.MasterItemCategoryStructureField field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            string nipEntry = null,
              tipeId = null;

            int nLoop = 0,
              totalDetails = 0;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            try
            {
                tipeId = structure.Fields.TipeID;

                if (string.IsNullOrEmpty(tipeId))
                {
                    result = "Tipe kategori dibutuhkan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    goto endLogic;
                }

                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        listMsIC = (from q in db.SCMS_MSITEM_CATs
                                    where (q.c_type == tipeId)
                                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                    select q).Distinct().ToList();

                        if (listMsIC == null)
                        {
                            listMsIC = null;
                        }

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if (field.IsNew && (!field.IsDelete) && (!field.IsModified) && (!string.IsNullOrEmpty(field.Item)))
                            {
                                #region New

                                msic = listMsIC.Find(delegate(SCMS_MSITEM_CAT mic)
                                {
                                    return (string.IsNullOrEmpty(mic.c_iteno) ? string.Empty : mic.c_iteno.Trim()).Equals(field.Item, StringComparison.OrdinalIgnoreCase);
                                });

                                if (msic != null)
                                {
                                    continue;
                                }

                                msic = new SCMS_MSITEM_CAT()
                                {
                                    c_type = tipeId,
                                    c_iteno = field.Item,
                                    c_entry = nipEntry,
                                    c_update = nipEntry,
                                    d_entry = date,
                                    d_update = date,
                                };

                                listMsIC.Add(msic);

                                db.SCMS_MSITEM_CATs.InsertOnSubmit(msic);

                                totalDetails++;

                                #endregion
                            }
                            else if ((!field.IsNew) && field.IsDelete && (!field.IsModified) && (!string.IsNullOrEmpty(field.Item)))
                            {
                                #region Delete

                                msic = listMsIC.Find(delegate(SCMS_MSITEM_CAT mic)
                                {
                                    return field.Item.Equals((string.IsNullOrEmpty(mic.c_iteno) ? string.Empty : mic.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                      structure.Fields.TipeID.Equals((string.IsNullOrEmpty(mic.c_type) ? string.Empty : mic.c_type.Trim()), StringComparison.OrdinalIgnoreCase);
                                });

                                if (msic != null)
                                {
                                    listMsIC.Remove(msic);

                                    db.SCMS_MSITEM_CATs.DeleteOnSubmit(msic);

                                    //db.SCMS_MSITEM_CATs.DeleteOnSubmit(msic);

                                    totalDetails++;
                                }

                                #endregion
                            }
                        }

                        listMsIC.Clear();
                    }

                    hasAnyChanges = (totalDetails > 0);

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:MasterItemCategory - {0}", ex.Message);

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

        public string MasterItemVia(ScmsSoaLibrary.Parser.Class.MasterItemViaStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;

            SCMS_MSITEM_VIA msic = null,
              msic_1 = null;

            List<SCMS_MSITEM_VIA> listMsIC = null;

            //SCMS_MSITEM_VIA1 msic1 = null;

            DateTime date = DateTime.Now;

            ScmsSoaLibrary.Parser.Class.MasterItemViaStructureField field = null;


            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            string nipEntry = null,
              tipeId = null;

            int nLoop = 0,
              totalDetails = 0;

            char gdg = (string.IsNullOrEmpty(structure.Fields.Gdg) ? char.MinValue : structure.Fields.Gdg[0]);


            nipEntry = (structure.Fields.Entry ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            try
            {
                tipeId = structure.Fields.TipeID;

                //if (string.IsNullOrEmpty(tipeId))
                //{
                //  result = "Tipe kategori dibutuhkan.";

                //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                //  goto endLogic;
                //}

                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
                {
                    #region Add

                    msic_1 = new SCMS_MSITEM_VIA();
                    dic = new Dictionary<string, string>();

                    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    {
                        field = structure.Fields.Field[nLoop];
                        msic = new SCMS_MSITEM_VIA()
                        {
                            c_gdg = gdg,
                            c_cusno = structure.Fields.Cusno,
                            c_via = field.Via,
                            c_iteno = field.Item,
                            c_entry = nipEntry,
                            d_entry = date
                        };


                        //string sNamaItem = (from q in db.FA_MasItms
                        //                    where q.c_iteno == field.Item
                        //                    select q.v_itnam).Take(1).SingleOrDefault();



                        //string n_idx = (from q in db.SCMS_MSITEM_VIAs
                        //               orderby q.idx descending
                        //               select q.idx.ToString())
                        //               .Take(1)
                        //               .SingleOrDefault()
                        //               ;

                        //decimal idx = 0;
                        //if (string.IsNullOrEmpty(n_idx))
                        //  idx = 1;
                        //else
                        //  idx = decimal.Parse(n_idx);

                        //string namaSup = (from q in db.FA_MasItms
                        //                  join q1 in db.LG_DatSups on q.c_nosup equals q1.c_nosup
                        //                  where q.c_iteno == field.Item
                        //                  select q1.v_nama).Take(1).SingleOrDefault();

                        //string ketVia = (from q in db.MsTransDs
                        //                 where q.c_portal == '3' && q.c_notrans == "02" && q.c_type == field.Via
                        //                 select q.v_ket).Take(1).SingleOrDefault();


                        //

                        //dic.Add("idx"+nLoop, idx.ToString());
                        //dic.Add("c_iteno" + nLoop, field.Item);
                        //dic.Add("v_itnam" + nLoop, sNamaItem);
                        //dic.Add("v_nama" + nLoop, namaSup);
                        //dic.Add("v_ket" + nLoop, ketVia);

                        //for (int a = 0; a < 10; a++)
                        //{
                        //  dic.Add("asd"+a, a.ToString());
                        //}
                        db.SCMS_MSITEM_VIAs.InsertOnSubmit(msic);
                        db.SubmitChanges();
                        totalDetails++;
                    }
                    hasAnyChanges = (totalDetails > 0);
                }
                    #endregion

                if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify


                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        listMsIC = (from q in db.SCMS_MSITEM_VIAs
                                    where (q.idx == int.Parse(tipeId))
                                    select q).Distinct().ToList();

                        if (listMsIC == null)
                        {
                            result = "Data Tidak terbaca.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            goto endLogic;

                        }

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if ((!field.IsNew) && field.IsDelete && (!field.IsModified) && (!string.IsNullOrEmpty(field.idx)))
                            {
                                #region Delete

                                msic = listMsIC.Find(delegate(SCMS_MSITEM_VIA mic)
                                {
                                    return field.idx.Equals((string.IsNullOrEmpty(mic.idx.ToString()) ? string.Empty : mic.idx.ToString()), StringComparison.OrdinalIgnoreCase);
                                    // && structure.Fields.TipeID.Equals((string.IsNullOrEmpty(mic.c_type) ? string.Empty : mic.c_type.Trim()), StringComparison.OrdinalIgnoreCase);
                                });

                                if (msic != null)
                                {
                                    listMsIC.Remove(msic);

                                    //msic1 = new SCMS_MSITEM_VIA1()
                                    //{
                                    //    c_cusno = msic.c_cusno,
                                    //    c_entry = nipEntry,
                                    //    d_entry = date,
                                    //    c_gdg = msic.c_gdg,
                                    //    c_iteno = msic.c_iteno,
                                    //    c_via = msic.c_via,
                                    //    v_ket = structure.Fields.Keterangan,
                                    //};
                                    //db.SCMS_MSITEM_VIA1s.InsertOnSubmit(msic1);
                                    db.SCMS_MSITEM_VIAs.DeleteOnSubmit(msic);

                                    //db.SCMS_MSITEM_CATs.DeleteOnSubmit(msic);

                                    totalDetails++;
                                }

                                #endregion
                            }
                        }

                        listMsIC.Clear();
                    }

                    hasAnyChanges = (totalDetails > 0);

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:MasterItemVia - {0}", ex.Message);

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

        public string MasterDriver(ScmsSoaLibrary.Parser.Class.MasterDriverStructure structure)
        {
            if ((structure == null)) // || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;

            SCMS_DRIVER scmsdriver = null;

            DateTime date = DateTime.Now;

            ScmsSoaLibrary.Parser.Class.MasterDriverStructureField field = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            string nipEntry = null;

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

                    scmsdriver = new SCMS_DRIVER();
                    dic = new Dictionary<string, string>();
                    if (structure.Fields.Tipe == "01")
                    {
                        scmsdriver = new SCMS_DRIVER()
                        {
                            c_nip = structure.Fields.Nip,
                            v_nama = structure.Fields.Nama,
                            c_type = structure.Fields.Tipe,
                            c_nopol = structure.Fields.Nopol,
                            l_aktif = structure.Fields.Aktif,
                            c_entry = structure.Fields.Entry,
                            d_entry = date
                        };
                        dic.Add("sub", structure.Fields.Nip);
                    }
                    else
                    {
                        string tipe = (from q in db.SCMS_DRIVERs
                                       where q.c_type == "02"
                                       select q.c_nip).Max();

                        if (tipe == null)
                        {
                            tipe = "X000000";
                        }
                        string sub = tipe.Substring(1, 6);
                        int addnip = Int16.Parse(sub) + 1;

                        sub = addnip.ToString();
                        sub = "X" + sub.PadLeft(6, '0');
                        dic.Add("sub", sub);

                        scmsdriver = new SCMS_DRIVER()
                        {
                            c_nip = sub,
                            v_nama = structure.Fields.Nama,
                            c_type = structure.Fields.Tipe,
                            c_nopol = structure.Fields.Nopol,
                            l_aktif = structure.Fields.Aktif,
                            c_entry = structure.Fields.Entry,
                            d_entry = date
                        };
                    }
                    db.SCMS_DRIVERs.InsertOnSubmit(scmsdriver);
                    db.SubmitChanges();

                }
                hasAnyChanges = true;

                    #endregion

                if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    scmsdriver = (from q in db.SCMS_DRIVERs
                                  where (q.c_nip == structure.Fields.Nip)
                                  select q).Take(1).SingleOrDefault();

                    if (scmsdriver == null)
                    {
                        result = "Data driver tidak terbaca dari database.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    scmsdriver.v_nama = structure.Fields.Nama;
                    scmsdriver.c_type = structure.Fields.Tipe;
                    scmsdriver.c_nopol = structure.Fields.Nopol;
                    scmsdriver.l_aktif = structure.Fields.Aktif;
                    scmsdriver.c_update = structure.Fields.Entry;
                    scmsdriver.d_update = date;

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:MasterDriver - {0}", ex.Message);

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

        public string MasterItemLantai(ScmsSoaLibrary.Parser.Class.MasterItemLantaiStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;

            SCMS_MSITEM_LAT msil = null;

            List<SCMS_MSITEM_LAT> listMsIL = null;

            DateTime date = DateTime.Now;

            ScmsSoaLibrary.Parser.Class.MasterItemLantaiStructureField field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            string nipEntry = null,
              tipeId = null;

            int nLoop = 0,
              totalDetails = 0;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            try
            {
                tipeId = structure.Fields.TipeID;

                if (string.IsNullOrEmpty(tipeId))
                {
                    result = "Tipe kategori dibutuhkan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    goto endLogic;
                }

                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '1' : structure.Fields.Gudang[0]);

                        listMsIL = (from q in db.SCMS_MSITEM_LATs
                                    where (q.c_type_lat == tipeId)
                                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                      && q.c_gdg == gudang
                                    select q).Distinct().ToList();

                        if (listMsIL == null)
                        {
                            listMsIL = null;
                        }

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if (field.IsNew && (!field.IsDelete) && (!field.IsModified) && (!string.IsNullOrEmpty(field.Item)))
                            {
                                #region New

                                msil = listMsIL.Find(delegate(SCMS_MSITEM_LAT mil)
                                {
                                    return (string.IsNullOrEmpty(mil.c_iteno) ? string.Empty : mil.c_iteno.Trim()).Equals(field.Item, StringComparison.OrdinalIgnoreCase);
                                });

                                if (msil != null)
                                {
                                    continue;
                                }

                                msil = new SCMS_MSITEM_LAT()
                                {
                                    c_type_lat = tipeId,
                                    c_iteno = field.Item,
                                    c_entry = nipEntry,
                                    c_update = nipEntry,
                                    d_entry = date,
                                    d_update = date,
                                    c_gdg = gudang
                                };

                                listMsIL.Add(msil);

                                db.SCMS_MSITEM_LATs.InsertOnSubmit(msil);

                                totalDetails++;

                                #endregion
                            }
                            else if ((!field.IsNew) && field.IsDelete && (!field.IsModified) && (!string.IsNullOrEmpty(field.Item)))
                            {
                                #region Delete

                                msil = listMsIL.Find(delegate(SCMS_MSITEM_LAT mic)
                                {
                                    return field.Item.Equals((string.IsNullOrEmpty(mic.c_iteno) ? string.Empty : mic.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                      gudang.Equals(mic.c_gdg) &&
                                      structure.Fields.TipeID.Equals((string.IsNullOrEmpty(mic.c_type_lat) ? string.Empty : mic.c_type_lat.Trim()), StringComparison.OrdinalIgnoreCase);
                                });

                                if (msil != null)
                                {
                                    listMsIL.Remove(msil);

                                    db.SCMS_MSITEM_LATs.DeleteOnSubmit(msil);

                                    totalDetails++;
                                }

                                #endregion
                            }
                        }

                        listMsIL.Clear();
                    }

                    hasAnyChanges = (totalDetails > 0);

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:MasterItemLantai - {0}", ex.Message);

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

        public string DivisiPrinsipalItem(ScmsSoaLibrary.Parser.Class.MasterDivPrinsipalItemStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;
            FA_Divpri divpri = null;
            List<FA_Divpri> lstDivPri = null;
            List<fa_divpriD1> lstDivPriD1 = null;

            DateTime date = DateTime.Today;

            ScmsSoaLibrary.Parser.Class.MasterDivPrinsipalItemStructureField field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            int totalDetails = 0,
              nLoop = 0;

            string nipEntry = null;
            string DivSupplierId = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

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
                    #region Modify

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        lstDivPri = new List<FA_Divpri>();
                        lstDivPriD1 = new List<fa_divpriD1>();

                        if (string.IsNullOrEmpty(structure.Fields.DivID))
                        {
                            result = "ID Div. Prinsipal di butuhkan.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            goto endLogic;
                        }

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if (field.IsNew && (!field.IsModified) & (!field.IsDelete))
                            {
                                divpri = new FA_Divpri()
                                {
                                    c_entry = nipEntry,
                                    c_iteno = field.Item,
                                    c_kddivpri = structure.Fields.DivID,
                                    c_update = nipEntry,
                                    d_entry = date,
                                    d_update = date
                                };

                                db.FA_Divpris.InsertOnSubmit(divpri);
                            }
                            if ((!field.IsNew) && (!field.IsModified) & (field.IsDelete))
                            {
                                lstDivPriD1.Add(new fa_divpriD1()
                                {
                                    c_entry = nipEntry,
                                    c_iteno = field.Item,
                                    c_kddivri = structure.Fields.DivID,
                                    d_entry = date,
                                    v_ket_del = field.KeteranganMod,
                                    v_type = "01"
                                });

                                divpri = (from q in db.FA_Divpris
                                          where q.c_kddivpri == structure.Fields.DivID
                                          && q.c_iteno == field.Item
                                          select q).Take(1).SingleOrDefault();

                                db.FA_Divpris.DeleteOnSubmit(divpri);

                            }
                        }

                        if (lstDivPriD1.Count > 0 && lstDivPriD1 != null)
                        {
                            db.fa_divpriD1s.InsertAllOnSubmit(lstDivPriD1.ToArray());
                            lstDivPriD1.Clear();
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
            }
            catch (Exception ex)
            {
                if (db.Transaction != null)
                {
                    db.Transaction.Rollback();
                }

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:DivPrisipal - {0}", ex.Message);

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

        public string DivisiAMSItem(ScmsSoaLibrary.Parser.Class.MasterDivAMSItemStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;
            List<FA_DivAM> lstDivAMS = null;
            List<FA_DivAMSD1> lstDivAMSD1 = null;

            FA_DivAM divAMS = null;
            FA_MsDivAM msDivAMS = null;

            DateTime date = DateTime.Today;

            ScmsSoaLibrary.Parser.Class.MasterDivAMSItemStructureField field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            decimal nQty = 0;

            int totalDetails = 0;

            string nipEntry = null,
              DivID = null;

            int nLoop = 0;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            DivID = (structure.Fields.DivID ?? string.Empty);

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
                {
                    #region Add

                    if (!string.IsNullOrEmpty(DivID))
                    {
                        result = "Nomor Div AMS harus kosong.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (string.IsNullOrEmpty(structure.Fields.NamaDiv))
                    {
                        result = "Nama Divisi dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    DivID = Commons.GenerateNumberingMaster(db, "000", "c_kddivams");

                    msDivAMS = new FA_MsDivAM()
                    {
                        c_entry = structure.Fields.Entry,
                        c_update = structure.Fields.Entry,
                        d_entry = date,
                        d_update = date,
                        c_kddivams = DivID,
                        l_aktif = structure.Fields.Aktif,
                        l_hide = structure.Fields.Hide,
                        v_nmdivams = structure.Fields.NamaDiv,
                        l_delete = false,
                    };

                    db.FA_MsDivAMs.InsertOnSubmit(msDivAMS);

                    #region Detil

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        lstDivAMS = new List<FA_DivAM>();

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if ((field != null) && field.IsNew && (!field.IsDelete))
                            {
                                divAMS = new FA_DivAM()
                                {
                                    c_entry = structure.Fields.Entry,
                                    c_update = structure.Fields.Entry,
                                    d_entry = date,
                                    d_update = date,
                                    c_iteno = field.Item,
                                    c_kddivams = DivID,
                                };

                                db.FA_DivAMs.InsertOnSubmit(divAMS);

                                totalDetails++;
                            }
                        }
                    }

                    #endregion

                    if (totalDetails > 0)
                    {
                        dic = new Dictionary<string, string>();
                        dic.Add("DIVID", DivID);
                        dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                        result = string.Format("Total {0} detail(s)", totalDetails);

                        hasAnyChanges = true;
                    }

                    #endregion
                }
                else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    if (string.IsNullOrEmpty(DivID))
                    {
                        result = "Nomor Divisi AMS dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    msDivAMS = (from q in db.FA_MsDivAMs
                                where q.c_kddivams == DivID
                                select q).Take(1).SingleOrDefault();

                    if (msDivAMS == null)
                    {
                        result = "Nomor Divisi AMS tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (msDivAMS.l_delete.HasValue && msDivAMS.l_delete.Value)
                    {
                        result = "Tidak dapat menghapus nomor Divisi AMS yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    msDivAMS.v_nmdivams = structure.Fields.NamaDiv;
                    msDivAMS.l_aktif = structure.Fields.Aktif;
                    msDivAMS.l_hide = structure.Fields.Hide;

                    #region Populate Detail

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        lstDivAMS = new List<FA_DivAM>();
                        lstDivAMSD1 = new List<FA_DivAMSD1>();

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified))
                            {
                                divAMS = new FA_DivAM()
                                {
                                    c_entry = structure.Fields.Entry,
                                    c_update = structure.Fields.Entry,
                                    d_entry = date,
                                    d_update = date,
                                    c_iteno = field.Item,
                                    c_kddivams = DivID,
                                };

                                db.FA_DivAMs.InsertOnSubmit(divAMS);
                            }
                            if ((field != null) && (!field.IsNew) && field.IsDelete && (!field.IsModified))
                            {
                                divAMS = (from q in db.FA_DivAMs
                                          where (q.c_kddivams == DivID)
                                          && (q.c_iteno == field.Item)
                                          select q).Take(1).SingleOrDefault();

                                db.FA_DivAMs.DeleteOnSubmit(divAMS);

                                lstDivAMSD1.Add(new FA_DivAMSD1()
                                {
                                    c_entry = structure.Fields.Entry,
                                    c_iteno = field.Item,
                                    c_kddivmas = DivID,
                                    d_entry = date,
                                    v_ket_del = structure.Fields.Keterangan,
                                    v_type = "02"
                                });
                            }
                        }
                        if (lstDivAMSD1.Count > 0 || lstDivAMSD1 != null)
                        {
                            db.FA_DivAMSD1s.InsertAllOnSubmit(lstDivAMSD1.ToArray());
                            lstDivAMSD1.Clear();
                        }
                    }

                    hasAnyChanges = true;

                    #endregion

                    #endregion
                }
                else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    if (string.IsNullOrEmpty(DivID))
                    {
                        result = "Nomor Div AMS dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        goto endLogic;
                    }

                    msDivAMS = (from q in db.FA_MsDivAMs
                                where q.c_kddivams == DivID
                                select q).Take(1).SingleOrDefault();

                    if (msDivAMS == null)
                    {
                        result = "Nomor Divisi AMS tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (msDivAMS.l_delete.HasValue && msDivAMS.l_delete.Value)
                    {
                        result = "Tidak dapat menghapus nomor Divisi AMS yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    msDivAMS.c_update = nipEntry;
                    msDivAMS.d_update = date;

                    msDivAMS.l_delete = true;
                    msDivAMS.v_ket_mark = structure.Fields.Keterangan;

                    lstDivAMS = (from q in db.FA_DivAMs
                                 where q.c_kddivams == DivID
                                 select q).ToList();
                    if (lstDivAMS.Count > 0)
                    {
                        lstDivAMSD1 = new List<FA_DivAMSD1>();

                        for (nLoop = 0; nLoop < lstDivAMS.Count; nLoop++)
                        {
                            divAMS = lstDivAMS[nLoop];

                            lstDivAMSD1.Add(new FA_DivAMSD1()
                            {
                                c_entry = structure.Fields.Entry,
                                c_iteno = divAMS.c_iteno,
                                c_kddivmas = divAMS.c_kddivams,
                                d_entry = date,
                                v_ket_del = structure.Fields.Keterangan,
                                v_type = "03"
                            });

                            db.FA_DivAMs.DeleteOnSubmit(divAMS);
                        }
                        if (lstDivAMSD1.Count > 0 || lstDivAMSD1 != null)
                        {
                            db.FA_DivAMSD1s.InsertAllOnSubmit(lstDivAMSD1.ToArray());
                            lstDivAMSD1.Clear();
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

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:DivAMS - {0}", ex.Message);

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

        public string MasterUserApj(ScmsSoaLibrary.Parser.Class.MasterUserApjStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;
            LG_USERSP_A_APJ UserApj = null;

            DateTime date = DateTime.Today;

            ScmsSoaLibrary.Parser.Class.MasterUserApjStructureFields field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            char gdg = (string.IsNullOrEmpty(structure.Fields.Gudang) ? char.MinValue : structure.Fields.Gudang[0]);

            //int totalDetails = 0;

            string nipEntry = null;
            string NipId = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            NipId = (structure.Fields.NipID ?? string.Empty);
            //NipId = (structure.Fields.Nip ?? string.Empty);

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
                {
                    #region Add

                    if (string.IsNullOrEmpty(structure.Fields.Nip))
                    {
                        result = "NIP dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (string.IsNullOrEmpty(structure.Fields.CusNo))
                    {
                        result = "Kode Cabang dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    field = structure.Fields;

                    UserApj = new LG_USERSP_A_APJ()
                    {
                        c_nip = field.Nip,
                        v_nama = field.Nama,
                        c_cusno = field.CusNo,
                        c_nosik = field.NoSik,
                        c_gdg = gdg,
                        v_imagepath = field.ImagePath,
                        //v_imageraw = field.ImageRaw,
                        c_nopbf = field.NoPbf,
                        c_kodearea = field.KodeArea,
                        c_entry = nipEntry,
                        d_entry = date,
                        l_delete = false
                    };

                    dic = new Dictionary<string, string>();
                    dic.Add("NipId", NipId);

                    if (UserApj != null)
                    {
                        db.LG_USERSP_A_APJs.InsertOnSubmit(UserApj);
                        hasAnyChanges = true;

                    }

                    #endregion
                }
                else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    field = structure.Fields;

                    UserApj = (from q in db.LG_USERSP_A_APJs
                               where q.c_nip == field.NipID
                               select q).Take(1).SingleOrDefault();

                    UserApj.v_nama = field.Nama;
                    //UserApj.c_cusno = field.CusNo;
                    UserApj.c_nosik = field.NoSik;
                    UserApj.c_gdg = gdg;
                    //UserApj.v_imagepath = field.ImagePath;
                    //UserApj.v_imageraw = field.ImageRaw;
                    UserApj.c_nopbf = field.NoPbf;
                    UserApj.c_update = nipEntry;
                    UserApj.d_update = date;


                    hasAnyChanges = true;

                    #endregion
                }
                else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    if (string.IsNullOrEmpty(NipId))
                    {
                        result = "NIP dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        goto endLogic;
                    }

                    UserApj = (from q in db.LG_USERSP_A_APJs
                               where q.c_nip == NipId
                               select q).Take(1).SingleOrDefault();

                    if (UserApj == null)
                    {
                        result = "NIP tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (UserApj.l_delete.HasValue && UserApj.l_delete.Value)
                    {
                        result = "Tidak dapat menghapus NIP yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    UserApj.c_delete = nipEntry;
                    UserApj.d_update = date;

                    UserApj.l_delete = true;
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

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:APJ - {0}", ex.Message);

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

        public string MasterPKP(ScmsSoaLibrary.Parser.Class.MasterPKPStructure structure)
        {
            if ((structure == null)) // || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;

            LG_NonDatsup nondatsup = null;

            DateTime date = DateTime.Now;

            //ScmsSoaLibrary.Parser.Class.FakturManualStructureField field = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            string nipEntry = null,
                pkpID = null;

            pkpID = string.IsNullOrEmpty(structure.Fields.pkpno) ? string.Empty : structure.Fields.pkpno;

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

                    nondatsup = new LG_NonDatsup();


                    pkpID = (from q in db.LG_NonDatsups
                                 select q.c_pkpno).Max();

                    if (string.IsNullOrEmpty(pkpID))
                    {
                        pkpID = "P0000";
                    }
                    string sub = pkpID.Substring(1, 4);
                    int addnum = Int16.Parse(sub) + 1;

                    sub = addnum.ToString();
                    pkpID = "P" + sub.PadLeft(4, '0');

                    nondatsup = new LG_NonDatsup()
                    {
                        c_pkpno = pkpID,
                        v_nama = structure.Fields.pkpname,
                        v_npwp = structure.Fields.npwp,
                        v_nppkp = structure.Fields.nppkp,
                        d_tglpkp = structure.Fields.nppkpdateC,
                        v_alamat1 = structure.Fields.alamat1,
                        v_alamat2 = structure.Fields.alamat2,
                        v_telepon1 = structure.Fields.telepon1,
                        v_fax1 = structure.Fields.fax1,
                        v_fax2 = structure.Fields.fax2,
                        l_aktif = structure.Fields.isAktif,
                        c_entry = nipEntry,
                        d_entry = date,
                        c_update = nipEntry,
                        d_update = date
                    };


                    db.LG_NonDatsups.InsertOnSubmit(nondatsup);
                    db.SubmitChanges();


                    hasAnyChanges = true;
                }

                    #endregion

                if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    if (string.IsNullOrEmpty(pkpID))
                    {
                        result = "Nomor PKP dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    nondatsup = (from q in db.LG_NonDatsups
                             where q.c_pkpno == pkpID
                             select q).Take(1).SingleOrDefault();

                    if (nondatsup == null)
                    {
                        result = "Nomor PKP tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    if (nondatsup != null)
                    {
                        nondatsup.c_pkpno = pkpID;
                        nondatsup.v_nama = structure.Fields.pkpname;
                        nondatsup.v_npwp = structure.Fields.npwp;
                        nondatsup.v_nppkp = structure.Fields.nppkp;
                        nondatsup.d_tglpkp = structure.Fields.nppkpdateC;
                        nondatsup.v_alamat1 = structure.Fields.alamat1;
                        nondatsup.v_alamat2 = structure.Fields.alamat2;
                        nondatsup.v_telepon1 = structure.Fields.telepon1;
                        nondatsup.v_fax1 = structure.Fields.fax1;
                        nondatsup.v_fax2 = structure.Fields.fax2;
                        nondatsup.l_aktif = structure.Fields.isAktif;
                        nondatsup.c_update = nipEntry;
                        nondatsup.d_update = date;

                        hasAnyChanges = true;
                    }
                    else
                    {
                        result = "Data PKP tidak terbaca dari database.";

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
                    dic = new Dictionary<string, string>();
                    dic.Add("pkpno", pkpID);
                    dic.Add("tglpkp", structure.Fields.nppkpdateC.ToString("yyyyMMdd"));

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Faktur:FakturManual - {0}", ex.Message);

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

        public string MasterNomorPajak(ScmsSoaLibrary.Parser.Class.MasterNomorPajakStructure structure)
        {
            if ((structure == null)) // || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;

            LG_MsNomorPajak msnomorpajak = null;

            DateTime date = DateTime.Now;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            string nipEntry = null,
                idx = null;

            int id = 0;

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

                    msnomorpajak = new LG_MsNomorPajak();
                    dic = new Dictionary<string, string>();

                        msnomorpajak = new LG_MsNomorPajak()
                        {
                            s_tahun = short.Parse(structure.Fields.tahun),
                            c_digit1 = structure.Fields.digit1,
                            c_digit2 = structure.Fields.digit2,
                            c_awal = structure.Fields.awal,
                            c_akhir = structure.Fields.akhir,
                            c_current = structure.Fields.current,
                            c_entry = structure.Fields.Entry,
                            d_entry = date
                        };

                        db.LG_MsNomorPajaks.InsertOnSubmit(msnomorpajak);
                        db.SubmitChanges();

                }
                hasAnyChanges = true;

                    #endregion

                if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    id = short.Parse(structure.Fields.idx);

                    msnomorpajak = (from q in db.LG_MsNomorPajaks
                                  where (q.IDX == id)
                                  select q).Take(1).SingleOrDefault();

                    if (msnomorpajak == null)
                    {
                        result = "Data tidak terbaca dari database.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else
                    {

                        msnomorpajak.s_tahun = short.Parse(structure.Fields.tahun);
                        msnomorpajak.c_digit1 = structure.Fields.digit1;
                        msnomorpajak.c_digit2 = structure.Fields.digit2;
                        msnomorpajak.c_awal = structure.Fields.awal;
                        msnomorpajak.c_akhir = structure.Fields.akhir;
                        msnomorpajak.c_current = structure.Fields.current;
                    }

                    hasAnyChanges = true;

                    #endregion
                }

                if (hasAnyChanges)
                {
                    db.SubmitChanges();

                    db.Transaction.Commit();

                    if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
                    {
                        msnomorpajak = new LG_MsNomorPajak();
                        msnomorpajak = (from q in db.LG_MsNomorPajaks
                                        select q).OrderByDescending(x => x.IDX).Take(1).SingleOrDefault();

                        idx = msnomorpajak.IDX.ToString();

                        dic.Add("sub", idx);
                    }

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Master:MasterNomorPajak - {0}", ex.Message);

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
