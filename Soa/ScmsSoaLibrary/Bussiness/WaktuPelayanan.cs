using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsModel;
using ScmsSoaLibrary.Parser;
using ScmsSoaLibrary.Commons;
using System.Data.Common;
using ScmsSoaLibraryInterface.Commons;
using ScmsSoaLibrary.Modules;
using Newtonsoft.Json;
using Ext.Net;
using System.Data.Linq.SqlClient;

namespace ScmsSoaLibrary.Bussiness
{
    class WaktuPelayanan
    {
        public string WaktuPelayananLogistik(ScmsSoaLibrary.Parser.Class.WaktuPelayananStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false;
            string result = null,
              NoTrans = null,
              sTipe = null;

            int totalDetails = 0,
              nLoop = 0;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            LG_WPLogistik wplogistik = null,
              wpLogData = null;

            List<LG_WPLogistik> lstWpLog = null;

            LG_WPLogistik1 wpBackup = null;

            List<LG_WPLogistik1> lstWpLog1 = null;

            LG_PLH plh = null;
            LG_DOH doh = null;

            ScmsSoaLibrary.Parser.Class.WaktuPelayananStructure field = new ScmsSoaLibrary.Parser.Class.WaktuPelayananStructure();

            ScmsSoaLibrary.Parser.Class.WaktuPelayananStructureField fields = null;

            char GudangLog = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '0' : structure.Fields.Gudang[0]);

            string nipEntry = (string.IsNullOrEmpty(structure.Fields.Entry) || (structure.Fields.Entry.Length < 1) ? string.Empty : structure.Fields.Entry);

            string nipGive = (string.IsNullOrEmpty(structure.Fields.Give) || (structure.Fields.Give.Length < 1) ? string.Empty : structure.Fields.Give);


            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now;

            IDictionary<string, string> dic = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);
            sTipe = (structure.Fields.Tipe ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip Penerima dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            if (string.IsNullOrEmpty(nipGive))
            {
                result = "Nip penyerah dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            if (GudangLog.Equals(char.MinValue))
            {
                result = "Gudang tidak dapat dibaca.";

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

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        lstWpLog = new List<LG_WPLogistik>();

                        plh = new LG_PLH();
                        doh = new LG_DOH();

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            fields = structure.Fields.Field[nLoop];

                            wpLogData = new LG_WPLogistik();

                            wpLogData = (from q in db.LG_WPLogistiks
                                         where q.c_notrans == fields.TransNo && q.c_gdg == GudangLog
                                         select q).Take(1).SingleOrDefault();

                            if (structure.Fields.Tipe.Equals("01"))
                            {
                                plh = (from q in db.LG_PLHs
                                       where q.c_plno == fields.TransNo && (q.l_wpppic == false || q.l_wpppic == null)
                                       select q).Take(1).SingleOrDefault();

                                if (plh == null)
                                {
                                    continue;
                                }

                            }
                            else if (structure.Fields.Tipe.Equals("02"))
                            {
                                doh = (from q in db.LG_DOHs
                                       where q.c_dono == fields.TransNo && (q.l_wpdc == false || q.l_wpdc == null)
                                       select q).Take(1).SingleOrDefault();

                                if (doh == null)
                                {
                                    continue;
                                }
                            }
                            else
                            {

                            }

                            if (wpLogData == null)
                            {

                                lstWpLog.Add(new LG_WPLogistik()
                                {
                                    c_entry = nipEntry,
                                    c_gdg = GudangLog,
                                    c_notrans = fields.TransNo,
                                    c_update = nipEntry,
                                    c_type = sTipe,
                                    d_entry = date,
                                    d_update = date,
                                    c_serah = nipGive,
                                });

                                if (structure.Fields.Tipe.Equals("01"))
                                {
                                    plh.l_wpppic = true;
                                }
                                else if (structure.Fields.Tipe.Equals("02"))
                                {
                                    doh.l_wpdc = true;
                                }
                                else
                                {

                                }

                                totalDetails++;
                            }
                            else
                            {
                                result = "Nomor transaksi sudah exist.";

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
                        db.LG_WPLogistiks.InsertAllOnSubmit(lstWpLog.ToArray());

                        dic = new Dictionary<string, string>();

                        dic.Add("WP", NoTrans);
                        dic.Add("Tanggal", date.ToString("yyyyMMdd HH:mm:ss"));

                        result = string.Format("Total {0} detail(s)", totalDetails);

                        hasAnyChanges = true;
                    }

                    #endregion
                }
                else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        lstWpLog1 = new List<LG_WPLogistik1>();

                        plh = new LG_PLH();
                        doh = new LG_DOH();

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {

                            fields = structure.Fields.Field[nLoop];

                            wpLogData = new LG_WPLogistik();

                            wpLogData = (from q in db.LG_WPLogistiks
                                         where q.c_notrans == fields.TransNo && q.c_gdg == GudangLog
                                         select q).Take(1).SingleOrDefault();

                            if (structure.Fields.Tipe.Equals("01"))
                            {
                                plh = (from q in db.LG_PLHs
                                       where q.c_plno == fields.TransNo && (q.l_wpppic == true || q.l_wpppic == true)
                                       select q).Take(1).SingleOrDefault();

                                if (plh == null)
                                {
                                    continue;
                                }

                            }
                            else if (structure.Fields.Tipe.Equals("02"))
                            {
                                doh = (from q in db.LG_DOHs
                                       where q.c_dono == fields.TransNo && (q.l_wpdc == true || q.l_wpdc == true)
                                       select q).Take(1).SingleOrDefault();

                                if (doh == null)
                                {
                                    continue;
                                }
                            }
                            else
                            {

                            }

                            if (wpLogData != null)
                            {

                                lstWpLog1.Add(new LG_WPLogistik1()
                                {
                                    c_entry = nipEntry,
                                    c_gdg = GudangLog,
                                    c_notrans = fields.TransNo,
                                    c_update = nipEntry,
                                    c_type = sTipe,
                                    d_entry = date,
                                    d_update = date,
                                    c_serah = nipGive,
                                });

                                db.LG_WPLogistiks.DeleteOnSubmit(wpLogData);

                                if (structure.Fields.Tipe.Equals("01"))
                                {
                                    plh.l_wpppic = false;
                                }
                                else if (structure.Fields.Tipe.Equals("02"))
                                {
                                    doh.l_wpdc = false;
                                }
                                else
                                {

                                }

                                totalDetails++;
                            }
                        }
                    }

                    if (totalDetails > 0)
                    {
                        db.LG_WPLogistik1s.InsertAllOnSubmit(lstWpLog1.ToArray());

                        dic = new Dictionary<string, string>();

                        dic.Add("WP", NoTrans);
                        dic.Add("Tanggal", date.ToString("yyyyMMdd HH:mm:ss"));

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

                result = string.Format("ScmsSoaLibrary.Bussiness.WaktuPelayanan:WaktuPelayananLogistik - {0}", ex.Message);

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

        public string SerahTerima(ScmsSoaLibrary.Parser.Class.WaktuPelayananStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false;
            string result = null,
              NoTrans = null,
              sTipe = null,
              stID = null,
              sLat = null,
              sCus = null,
              sTempTransCancel = null;

            int totalDetails = 0,
              nLoop = 0,
              nLoop2 = 0;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            SCMS_STH sth = null;
            SCMS_STD std = null;
            LG_PLH plh = null;
            LG_SJH sjh = null;
            LG_DOH doh = null;
            LG_RCH rch = null;
            LG_RSH rsh = null;

            List<SCMS_STD> listStd = null;
            List<string> listNO = null;

            List<LG_PLH> listResPLH = null;
            List<LG_SJH> listResSJH = null;
            List<LG_DOH> listResDOH = null;
            List<LG_RCH> listResRCH = null;
            List<LG_RSH> listResRSH = null;
            List<SCMS_STD> listResSTD = null;

            List<SCMS_BASPBH> listBASPBH = null;
            List<SCMS_BASPBD> listBASPBD = null;
            List<SCMS_BASPBD> listBASPBDSum = null;

            ScmsSoaLibrary.Parser.Class.WaktuPelayananStructureField fields = null;

            char GudangLog = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '0' : structure.Fields.Gudang[0]);

            string nipEntry = (string.IsNullOrEmpty(structure.Fields.Entry) || (structure.Fields.Entry.Length < 1) ? string.Empty : structure.Fields.Entry);

            string nipGive = (string.IsNullOrEmpty(structure.Fields.Give) || (structure.Fields.Give.Length < 1) ? string.Empty : structure.Fields.Give);


            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now;

            IDictionary<string, string> dic = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);
            sTipe = (structure.Fields.Tipe ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip Penerima dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            if (string.IsNullOrEmpty(nipGive))
            {
                result = "Nip penyerah dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            if (GudangLog == '0')
            {
                result = "Gudang tidak dapat dibaca.";

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

                    stID = Commons.GenerateNumbering<SCMS_STH>(db, "WP", '3', "38", date, "c_nodoc");

                    sth = new SCMS_STH()
                    {
                        c_nodoc = stID,
                        c_gdg = GudangLog,
                        d_date = date,
                        c_type = structure.Fields.Tipe,
                        c_entry = structure.Fields.Entry,
                        v_entry = structure.Fields.EntryName,
                        d_entry = date.Date,
                        c_give = structure.Fields.Give,
                        v_give = structure.Fields.GiveName,
                        c_update = structure.Fields.Entry,
                        d_update = date,
                        n_koli = structure.Fields.Koli,
                        n_berat = structure.Fields.Berat,
                        n_vol = structure.Fields.Volume,
                        c_resi = structure.Fields.Resi,
                        n_receh = structure.Fields.Receh,
                        n_kolireceh = structure.Fields.KoliReceh
                    };

                    db.SCMS_STHs.InsertOnSubmit(sth);

                    #region Insert Detail
                    #region Picker
                    if (structure.Fields.Tipe.Equals("01"))
                    {
                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            listStd = new List<SCMS_STD>();

                            listNO = structure.Fields.Field.GroupBy(x => x.TransNo).Select(y => y.Key).ToList();

                            listResPLH = (from q in db.LG_PLHs
                                          where listNO.Contains(q.c_plno)
                                          select q).Distinct().ToList();

                            listResSJH = (from q in db.LG_SJHs
                                          where listNO.Contains(q.c_sjno)
                                          select q).Distinct().ToList();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                fields = structure.Fields.Field[nLoop];
                                if ((fields != null) && fields.IsNew && (!fields.IsDelete))
                                {
                                    var CekPL = (from q in db.SCMS_STDs
                                                     join q1 in db.SCMS_STHs on new {q.c_nodoc} equals new {q1.c_nodoc}
                                                     where q.c_no == fields.TransNo && q1.c_type == "01"
                                                     select q).ToList();
                                    if (CekPL.Count != 0)
                                    {
                                        result = string.Format(fields.TransNo + " Sudah pernah di scan");
                                        rpe = ResponseParser.ResponseParserEnum.IsError;
                                        db.Transaction.Rollback();
                                        goto endLogic;
                                    }

                                    plh = listResPLH.Find(delegate(LG_PLH plhdr)
                                    {
                                        return fields.TransNo.Equals((string.IsNullOrEmpty(plhdr.c_plno) ? string.Empty : plhdr.c_plno), StringComparison.OrdinalIgnoreCase)
                                            && plhdr.l_confirm == true;
                                    });

                                    sjh = listResSJH.Find(delegate(LG_SJH sjhdr)
                                    {
                                        return fields.TransNo.Equals((string.IsNullOrEmpty(sjhdr.c_sjno) ? string.Empty : sjhdr.c_sjno), StringComparison.OrdinalIgnoreCase)
                                            && sjhdr.l_confirm == false;
                                    });

                                    if ((plh != null) || (sjh != null))
                                    {
                                        if (plh != null)
                                        {
                                            //var CheckDOH = (from q in db.LG_DOHs
                                            //                where q.c_plno == plh.c_plno
                                            //                select q).Distinct().ToList();

                                            //if (CheckDOH.Count > 0)
                                            //{
                                            //    sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo;
                                            //    continue;
                                            //}

                                            plh.l_wpppic = true;
                                            plh.c_wpppic = structure.Fields.Entry;
                                            plh.d_wpppic = date;
                                            sLat = plh.c_type_lat;
                                            sCus = plh.c_cusno;
                                        }
                                        else
                                        {
                                            sLat = sjh.c_type_lat;
                                            sCus = sjh.c_gdg2.Value.ToString();
                                        }

                                        listStd.Add(new SCMS_STD()
                                        {
                                            c_nodoc = stID,
                                            c_no = fields.TransNo,
                                            c_type_lat = sLat,
                                            c_cusno = sCus
                                        });

                                        totalDetails++;
                                    }
                                    else
                                    {
                                        sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo;
                                    }
                                }
                            }

                            listResPLH.Clear();
                            listResSJH.Clear();

                            listNO.Clear();

                            if (listStd.Count > 0)
                            {
                                db.SCMS_STDs.InsertAllOnSubmit(listStd.ToArray());
                                listStd.Clear();
                            }
                        }
                    }

                    #endregion

                    #region InkJet & Checker
                    if (structure.Fields.Tipe.Equals("1A") || structure.Fields.Tipe.Equals("02"))
                    {
                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            listStd = new List<SCMS_STD>();

                            listNO = structure.Fields.Field.GroupBy(x => x.TransNo).Select(y => y.Key).ToList();

                            listResSTD = (from q in db.SCMS_STDs
                                          join q1 in db.SCMS_STHs on q.c_nodoc equals q1.c_nodoc
                                          where listNO.Contains(q.c_no) && q1.c_type == "01"
                                          select q).Distinct().ToList();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                fields = structure.Fields.Field[nLoop];
                                if ((fields != null) && fields.IsNew && (!fields.IsDelete))
                                {
                                    var CekPL = (from q in db.SCMS_STDs
                                                 join q1 in db.SCMS_STHs on new { q.c_nodoc } equals new { q1.c_nodoc }
                                                 where q.c_no == fields.TransNo && (q1.c_type == structure.Fields.Tipe)
                                                 select q).ToList();
                                    if (CekPL.Count != 0)
                                    {
                                        result = string.Format(fields.TransNo + " Sudah pernah di scan");
                                        rpe = ResponseParser.ResponseParserEnum.IsError;
                                        db.Transaction.Rollback();
                                        goto endLogic;
                                    }

                                    std = listResSTD.Find(delegate(SCMS_STD sthdr)
                                    {
                                        return fields.TransNo.Equals((string.IsNullOrEmpty(sthdr.c_no) ? string.Empty : sthdr.c_no), StringComparison.OrdinalIgnoreCase);
                                    });

                                    if ((std != null))
                                    {
                                        //var CheckDOH = (from q in db.LG_DOHs
                                        //                where q.c_plno == std.c_no
                                        //                select q).Distinct().ToList();

                                        //if (CheckDOH.Count > 0)
                                        //{
                                        //    sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo;
                                        //    continue;
                                        //}

                                        listStd.Add(new SCMS_STD()
                                        {
                                            c_nodoc = stID,
                                            c_no = fields.TransNo,
                                            c_type_lat = std.c_type_lat,
                                            c_cusno = std.c_cusno
                                        });

                                        totalDetails++;
                                    }
                                    else
                                    {
                                        sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo;
                                    }
                                }
                            }

                            listResSTD.Clear();
                            listNO.Clear();

                            if (listStd.Count > 0)
                            {
                                db.SCMS_STDs.InsertAllOnSubmit(listStd.ToArray());
                                listStd.Clear();
                            }
                        }
                    }

                    #endregion

                    #region Packer
                    if (structure.Fields.Tipe.Equals("03"))
                    {
                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            listStd = new List<SCMS_STD>();

                            listNO = structure.Fields.Field.GroupBy(x => x.TransNo).Select(y => y.Key).ToList();

                            listResSTD = (from q in db.SCMS_STDs
                                          join q1 in db.SCMS_STHs on q.c_nodoc equals q1.c_nodoc
                                          where listNO.Contains(q.c_no) && q1.c_type == "02"
                                          select q).Distinct().ToList();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                fields = structure.Fields.Field[nLoop];
                                if ((fields != null) && fields.IsNew && (!fields.IsDelete))
                                {
                                    var CekPL = (from q in db.SCMS_STDs
                                                 join q1 in db.SCMS_STHs on new { q.c_nodoc } equals new { q1.c_nodoc }
                                                 where q.c_no == fields.TransNo && q1.c_type == "03"
                                                 select q).ToList();
                                    if (CekPL.Count != 0)
                                    {
                                        result = string.Format(fields.TransNo + " Sudah pernah di scan");
                                        rpe = ResponseParser.ResponseParserEnum.IsError;
                                        db.Transaction.Rollback();
                                        goto endLogic;
                                    }

                                    std = listResSTD.Find(delegate(SCMS_STD sthdr)
                                    {
                                        return fields.TransNo.Equals((string.IsNullOrEmpty(sthdr.c_no) ? string.Empty : sthdr.c_no), StringComparison.OrdinalIgnoreCase);
                                    });

                                    if ((std != null))
                                    {
                                        //var CheckDOH = (from q in db.LG_DOHs
                                        //                where q.c_plno == std.c_no
                                        //                select q).Distinct().ToList();

                                        //if (CheckDOH.Count > 0)
                                        //{
                                        //    sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo;
                                        //    continue;
                                        //}

                                        listStd.Add(new SCMS_STD()
                                        {
                                            c_nodoc = stID,
                                            c_no = fields.TransNo,
                                            c_type_lat = std.c_type_lat,
                                            c_cusno = std.c_cusno
                                        });

                                        totalDetails++;
                                    }
                                    else
                                    {
                                        sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo;
                                    }
                                }
                            }

                            listResSTD.Clear();
                            listNO.Clear();

                            if (listStd.Count > 0)
                            {
                                db.SCMS_STDs.InsertAllOnSubmit(listStd.ToArray());
                                listStd.Clear();
                            }
                        }
                    }

                    #endregion

                    #region Transportasi
                    if (structure.Fields.Tipe.Equals("04"))
                    {
                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            string[] typePL = new string[] { "02", "06" };

                            listStd = new List<SCMS_STD>();

                            listNO = structure.Fields.Field.GroupBy(x => x.TransNo).Select(y => y.Key).ToList();

                            listResDOH = (from q in db.LG_DOHs
                                          where listNO.Contains(q.c_dono)
                                          select q).Distinct().ToList();

                            listResSJH = (from q in db.LG_SJHs
                                          where listNO.Contains(q.c_sjno)
                                          select q).Distinct().ToList();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                fields = structure.Fields.Field[nLoop];
                                if ((fields != null) && fields.IsNew && (!fields.IsDelete))
                                {
                                    var CekPL = (from q in db.SCMS_STDs
                                                 join q1 in db.SCMS_STHs on new { q.c_nodoc } equals new { q1.c_nodoc }
                                                 where q.c_no == fields.TransNo && q1.c_type == "04"
                                                 select q).ToList();
                                    if (CekPL.Count != 0)
                                    {
                                        result = string.Format(fields.TransNo + " Sudah pernah di scan");
                                        rpe = ResponseParser.ResponseParserEnum.IsError;
                                        db.Transaction.Rollback();
                                        goto endLogic;
                                    }

                                    doh = listResDOH.Find(delegate(LG_DOH dohdr)
                                    {
                                        return fields.TransNo.Equals((string.IsNullOrEmpty(dohdr.c_dono) ? string.Empty : dohdr.c_dono), StringComparison.OrdinalIgnoreCase);
                                    });

                                    sjh = listResSJH.Find(delegate(LG_SJH sjhdr)
                                    {
                                        return fields.TransNo.Equals((string.IsNullOrEmpty(sjhdr.c_sjno) ? string.Empty : sjhdr.c_sjno), StringComparison.OrdinalIgnoreCase)
                                            && sjhdr.l_confirm == true;
                                    });

                                    if ((doh != null) || (sjh != null))
                                    {
                                        if (doh != null)
                                        {
                                            listResSTD = (from q in db.SCMS_STDs
                                                          join q1 in db.SCMS_STHs on q.c_nodoc equals q1.c_nodoc
                                                          where q.c_no == doh.c_plno && q1.c_type == "03"
                                                          select q).Distinct().ToList();

                                            listResPLH = (from q in db.LG_PLHs
                                                          where q.c_plno == doh.c_plno
                                                          select q).Distinct().ToList();

                                            if(!string.IsNullOrEmpty(fields.editkoli) && fields.IsModifiedKoli)
                                            {
                                                doh.n_karton = fields.karton;
                                                doh.c_editkoli = fields.editkoli;
                                            }
                                            doh.n_receh = fields.receh;
                                        }
                                        else
                                        {
                                            listResSTD = (from q in db.SCMS_STDs
                                                          join q1 in db.SCMS_STHs on q.c_nodoc equals q1.c_nodoc
                                                          where q.c_no == sjh.c_sjno && q1.c_type == "03"
                                                          select q).Distinct().ToList();

                                            if (!string.IsNullOrEmpty(fields.editkoli) && fields.IsModifiedKoli)
                                            {
                                                sjh.n_karton = fields.karton;
                                                sjh.c_editkoli = fields.editkoli;
                                            }

                                            sjh.n_receh = fields.receh;
                                        }
                                        if (listResSTD.Count > 0)
                                        {
                                            var CheckEPD = (from q in db.LG_ExpDs
                                                            where q.c_dono == listResSTD[0].c_no.ToString()
                                                            select q).Distinct().ToList();

                                            if (CheckEPD.Count > 0)
                                            {
                                                sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo;
                                                continue;
                                            }

                                            if (listResPLH != null)
                                            {
                                                sLat = (string.IsNullOrEmpty(listResPLH[0].c_type_lat) ? string.Empty : listResPLH[0].c_type_lat);
                                                sCus = listResPLH[0].c_cusno.ToString();
                                                listResPLH.Clear();
                                            }
                                            else
                                            {
                                                sLat = (string.IsNullOrEmpty(sjh.c_type_lat) ? string.Empty : sjh.c_type_lat);
                                                sCus = sjh.c_gdg2.Value.ToString();
                                            }

                                            if (sCus != structure.Fields.Cusno)
                                            {
                                                result = fields.TransNo + " ini salah cabang/gudang. Mohon direvisi...";

                                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                                goto endLogic;
                                            }

                                            listStd.Add(new SCMS_STD()
                                            {
                                                c_nodoc = stID,
                                                c_no = fields.TransNo,
                                                //n_koli = fields.Koli,
                                                //n_berat = fields.Berat,
                                                c_type_lat = sLat,
                                                c_cusno = sCus
                                            });

                                            totalDetails++;
                                        }
                                        else
                                        {
                                            if (listResPLH != null)
                                            {
                                                if (typePL.Contains(listResPLH[0].c_type.ToString()))
                                                {
                                                    sLat = (string.IsNullOrEmpty(listResPLH[0].c_type_lat) ? string.Empty : listResPLH[0].c_type_lat);
                                                    sCus = listResPLH[0].c_cusno.ToString();
                                                    listResPLH.Clear();

                                                    if (sCus != structure.Fields.Cusno)
                                                    {
                                                        result = fields.TransNo + " ini salah cabang/gudang. Mohon direvisi...";

                                                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                                        goto endLogic;
                                                    }

                                                    listStd.Add(new SCMS_STD()
                                                    {
                                                        c_nodoc = stID,
                                                        c_no = fields.TransNo,
                                                        //n_koli = fields.Koli,
                                                        //n_berat = fields.Berat,
                                                        c_type_lat = sLat,
                                                        c_cusno = sCus
                                                    });

                                                    totalDetails++;
                                                    listResSTD.Clear();

                                                    continue;
                                                }
                                            }
                                            if (sjh != null)
                                            {
                                                if (sjh.l_auto == true)
                                                {
                                                    sLat = (string.IsNullOrEmpty(sjh.c_type_lat) ? string.Empty : sjh.c_type_lat);
                                                    sCus = sjh.c_gdg2.Value.ToString();

                                                    if (sCus != structure.Fields.Cusno)
                                                    {
                                                        result = fields.TransNo + " ini salah cabang/gudang. Mohon direvisi...";

                                                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                                        goto endLogic;
                                                    }

                                                    listStd.Add(new SCMS_STD()
                                                    {
                                                        c_nodoc = stID,
                                                        c_no = fields.TransNo,
                                                        //n_koli = fields.Koli,
                                                        //n_berat = fields.Berat,
                                                        c_type_lat = sLat,
                                                        c_cusno = sCus
                                                    });

                                                    totalDetails++;
                                                    listResSTD.Clear();

                                                    continue;
                                                }
                                            }
                                            sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo;
                                        }
                                        listResSTD.Clear();
                                    }
                                    else
                                    {
                                        sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo;
                                    }
                                }
                            }

                            listResDOH.Clear();
                            listResSJH.Clear();

                            listNO.Clear();

                            if (listStd.Count > 0)
                            {
                                db.SCMS_STDs.InsertAllOnSubmit(listStd.ToArray());
                                listStd.Clear();
                            }
                        }
                    }

                    #endregion

                    #region Gudang PBB/R
                    if (structure.Fields.Tipe.Equals("05"))
                    {
                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            listStd = new List<SCMS_STD>();

                            listNO = structure.Fields.Field.GroupBy(x => x.TransNo).Select(y => y.Key).ToList();

                            listResRCH = (from q in db.LG_RCHes
                                          where listNO.Contains(q.v_pbbrno)
                                          select q).Distinct().ToList();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                fields = structure.Fields.Field[nLoop];

                                if ((fields != null) && fields.IsNew && (!fields.IsDelete))
                                {
                                    NoTrans = (from q in db.SCMS_STHs
                                               join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                               where q1.c_no == fields.TransNo && q.c_type == "05"
                                               select q1.c_no).Take(1).SingleOrDefault();

                                    if (!string.IsNullOrEmpty(NoTrans))
                                    {
                                        sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo + " - No.PBB/R ini sudah diproses serah terima Gudang PBB/R";
                                        continue;
                                    }

                                    rch = listResRCH.Find(delegate(LG_RCH rchdr)
                                    {
                                        return fields.TransNo.Equals((string.IsNullOrEmpty(rchdr.v_pbbrno) ? string.Empty : rchdr.v_pbbrno), StringComparison.OrdinalIgnoreCase);
                                    });

                                    if ((rch != null))
                                    {
                                        sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo + " - No.PBB/R ini sudah diproses RC";
                                        continue;
                                    }
                                    else
                                    {

                                        string cabDcore = fields.TransNo.Substring(3, 3);
                                        sCus = (from q in db.LG_CusmasCabs
                                                where q.c_cab_dcore == cabDcore
                                                select q.c_cusno).Take(1).SingleOrDefault();

                                        listStd.Add(new SCMS_STD()
                                        {
                                            c_nodoc = stID,
                                            c_no = fields.TransNo,
                                            c_cusno = sCus
                                        });

                                        totalDetails++;
                                    }
                                }
                            }

                            listResRCH.Clear();
                            listNO.Clear();

                            if (listStd.Count > 0)
                            {
                                db.SCMS_STDs.InsertAllOnSubmit(listStd.ToArray());
                                listStd.Clear();
                            }
                        }
                    }

                    #endregion

                    #region Admin PBB/R
                    if (structure.Fields.Tipe.Equals("06"))
                    {
                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            listStd = new List<SCMS_STD>();

                            listNO = structure.Fields.Field.GroupBy(x => x.TransNo).Select(y => y.Key).ToList();

                            listResRCH = (from q in db.LG_RCHes
                                          where listNO.Contains(q.v_pbbrno)
                                          select q).Distinct().ToList();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                fields = structure.Fields.Field[nLoop];

                                if ((fields != null) && fields.IsNew && (!fields.IsDelete))
                                {
                                    string TransSTRC = (from q in db.SCMS_STHs
                                                        join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                                        where q1.c_no == fields.TransNo && q.c_type == "05"
                                                        select q1.c_no).Take(1).SingleOrDefault();

                                    if (string.IsNullOrEmpty(TransSTRC))
                                    {
                                        sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo + " - No.PBB/R ini belum di proses serah terima Gudang PBB/R";
                                        continue;
                                    }

                                    NoTrans = (from q in db.SCMS_STHs
                                               join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                               where q1.c_no == fields.TransNo && q.c_type == "06"
                                               select q1.c_no).Take(1).SingleOrDefault();

                                    if (!string.IsNullOrEmpty(NoTrans))
                                    {
                                        sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo + " - No.PBB/R ini sudah diproses serah terima Admin PBB/R";
                                        continue;
                                    }

                                    rch = listResRCH.Find(delegate(LG_RCH rchdr)
                                    {
                                        return fields.TransNo.Equals((string.IsNullOrEmpty(rchdr.v_pbbrno) ? string.Empty : rchdr.v_pbbrno), StringComparison.OrdinalIgnoreCase);
                                    });

                                    if ((rch != null))
                                    {
                                        sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo + " - No.PBB/R ini sudah diproses RC";
                                        continue;
                                    }
                                    else
                                    {

                                        string cabDcore = fields.TransNo.Substring(3, 3);
                                        sCus = (from q in db.LG_CusmasCabs
                                                where q.c_cab_dcore == cabDcore
                                                select q.c_cusno).Take(1).SingleOrDefault();

                                        listStd.Add(new SCMS_STD()
                                        {
                                            c_nodoc = stID,
                                            c_no = fields.TransNo,
                                            c_cusno = sCus
                                        });

                                        totalDetails++;
                                    }
                                }
                            }

                            listResRCH.Clear();
                            listNO.Clear();

                            if (listStd.Count > 0)
                            {
                                db.SCMS_STDs.InsertAllOnSubmit(listStd.ToArray());
                                listStd.Clear();
                            }
                        }
                    }

                    #endregion

                    #region Gudang BASPB
                    if (structure.Fields.Tipe.Equals("07"))
                    {
                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            Encoding utf8 = Encoding.UTF8;
                            Config cfg = Functionals.Configuration;

                            listStd = new List<SCMS_STD>();
                            listBASPBH = new List<SCMS_BASPBH>();
                            listBASPBD = new List<SCMS_BASPBD>();

                            //listNO = structure.Fields.Field.GroupBy(x => x.TransNo).Select(y => y.Key).ToList();

                            //listResRCH = (from q in db.LG_RCHes
                            //              where listNO.Contains(q.v_pbbrno)
                            //              select q).Distinct().ToList();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                fields = structure.Fields.Field[nLoop];

                                if ((fields != null) && fields.IsNew && (!fields.IsDelete))
                                {
                                    NoTrans = (from q in db.SCMS_STHs
                                               join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                               where q1.c_no == fields.TransNo && q.c_type == "07"
                                               select q1.c_no).Take(1).SingleOrDefault();

                                    if (!string.IsNullOrEmpty(NoTrans))
                                    {
                                        sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo + " - No.BASPB ini sudah diproses serah terima gudang BASPB";
                                        continue;
                                    }
                                    else
                                    {

                                        string cabDcore = fields.TransNo.Substring(4, 3);
                                        sCus = (from q in db.LG_CusmasCabs
                                                where q.c_cab_dcore == cabDcore
                                                select q.c_cusno).Take(1).SingleOrDefault();

                                        #region Header BASPB
                                        Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.40/dcore/?m=com.ams.json.ds&action=form&f=Business&open=trx_baspb_hd");

                                        Dictionary<string, string> param = new Dictionary<string, string>();

                                        param.Add("C_BASPBNO", fields.TransNo);
                                        param.Add("C_KODECAB", cabDcore);

                                        Dictionary<string, string> header = new Dictionary<string, string>();
                                        header.Add("X-Requested-With", "XMLHttpRequest");

                                        string data = null;
                                        Dictionary<string, object> dicHeader = null;
                                        Dictionary<string, object> dicDetail = null;
                                        List<Dictionary<string, string>> list = null;
                                        Dictionary<string, string> dataRow = null;

                                        ScmsSoaLibrary.Parser.ParserDisCore pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

                                        pdc.Referer = Functionals.DistCoreUrlBuilderString(cfg, "http://10.100.10.40/dcore/?m=com.ams.trx.pbbpbr");
                                        pdc.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                                        if (pdc.PostGetData(uri, param, header))
                                        {
                                            data = utf8.GetString(pdc.Result);

                                            dicHeader = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCore(data);

                                            list = dicHeader[Constant.DEFAULT_NAMING_RECORDS] as List<Dictionary<string, string>>;

                                            if (list == null)
                                            {
                                                sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo + " - No.BASPB ini tidak pernah ada di cabang.";
                                                continue;
                                            }
                                            else
                                            {
                                                dataRow = list[0];

                                                listBASPBH.Add(new SCMS_BASPBH()
                                                {
                                                    c_baspbno = dataRow["C_BASPBNO"].ToString(),
                                                    c_cusno = sCus,
                                                    d_baspb = date,
                                                    c_dono = dataRow["C_DONO"].ToString(),
                                                    c_entry = "system",
                                                    d_entry = date,
                                                    c_update = "system",
                                                    d_update = date
                                                });
                                            }
                                        }
                                        #endregion

                                        #region Detail BASPB
                                        param.Add("C_BASPBNO", fields.TransNo);
                                        param.Add("C_KODECAB", cabDcore);

                                        uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.40/dcore/?m=com.ams.json.ds&action=form&f=Business&open=trx_baspb_dt");
                                        if (pdc.PostGetData(uri, param, header))
                                        {
                                            data = utf8.GetString(pdc.Result);

                                            dicDetail = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCore(data);

                                            list = dicDetail[Constant.DEFAULT_NAMING_RECORDS] as List<Dictionary<string, string>>;

                                            if (list != null)
                                            {
                                                for (nLoop2 = 0; nLoop2 < list.Count; nLoop2++)
                                                {
                                                    dataRow = list[nLoop2];
                                                    
                                                    listBASPBD.Add(new SCMS_BASPBD()
                                                    {
                                                        c_baspbno = dataRow["C_BASPBNO"].ToString(),
                                                        c_iteno = dataRow["C_ITENO"].ToString(),
                                                        c_claimtype = string.IsNullOrEmpty(dataRow["C_CLAIMTYPE"].ToString()) ? 'E' : char.Parse(dataRow["C_CLAIMTYPE"].ToString()),
                                                        n_qtydo = decimal.Parse(dataRow["N_QTYDO"].ToString()),
                                                        n_qtyrn = decimal.Parse(dataRow["N_QTYRN"].ToString()),
                                                        n_gqty = decimal.Parse(dataRow["N_QTYG"].ToString()),
                                                        n_bqty = decimal.Parse(dataRow["N_QTYB"].ToString()),
                                                        n_qtydiff = decimal.Parse(dataRow["N_DIFFDORN"].ToString()),
                                                        n_gsisa = decimal.Parse(dataRow["N_DIFFDORN"].ToString())
                                                    });
                                                }
                                            }
                                        }
                                        #endregion

                                        listStd.Add(new SCMS_STD()
                                        {
                                            c_nodoc = stID,
                                            c_no = fields.TransNo,
                                            c_cusno = sCus
                                        });

                                        totalDetails++;
                                    }
                                }
                            }

                            if (listStd.Count > 0)
                            {
                                db.SCMS_STDs.InsertAllOnSubmit(listStd.ToArray());
                                listStd.Clear();
                            }

                            if (listBASPBH.Count > 0)
                            {
                                db.SCMS_BASPBHs.InsertAllOnSubmit(listBASPBH.ToArray());
                                listBASPBH.Clear();
                            }

                            if (listBASPBD.Count > 0)
                            {
                                listBASPBDSum = listBASPBD.GroupBy(x => new { x.c_baspbno, x.c_iteno, })
                                        .Select(x => new SCMS_BASPBD()
                                        {
                                            c_baspbno = x.Key.c_baspbno,
                                            c_iteno = x.Key.c_iteno,
                                            n_qtydo = x.Sum(y => (y.n_qtydo.HasValue ? y.n_qtydo.Value : 0)),
                                            n_qtyrn = x.Sum(y => (y.n_qtyrn.HasValue ? y.n_qtyrn.Value : 0)),
                                            n_gqty = x.Sum(y => (y.n_gqty.HasValue ? y.n_gqty.Value : 0)),
                                            n_bqty = x.Sum(y => (y.n_bqty.HasValue ? y.n_bqty.Value : 0)),
                                            n_qtydiff = x.Sum(y => (y.n_qtydiff.HasValue ? y.n_qtydiff.Value : 0))
                                        }).ToList();

                                db.SCMS_BASPBDs.InsertAllOnSubmit(listBASPBDSum.ToArray());
                                listBASPBD.Clear();
                                listBASPBDSum.Clear();
                            }
                        }
                    }

                    #endregion

                    #region Admin BASPB
                    if (structure.Fields.Tipe.Equals("08"))
                    {
                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            listStd = new List<SCMS_STD>();

                            //listNO = structure.Fields.Field.GroupBy(x => x.TransNo).Select(y => y.Key).ToList();

                            //listResRCH = (from q in db.LG_RCHes
                            //              where listNO.Contains(q.v_pbbrno)
                            //              select q).Distinct().ToList();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                fields = structure.Fields.Field[nLoop];

                                if ((fields != null) && fields.IsNew && (!fields.IsDelete))
                                {
                                    string TransSTRC = (from q in db.SCMS_STHs
                                                        join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                                        where q1.c_no == fields.TransNo && q.c_type == "07"
                                                        select q1.c_no).Take(1).SingleOrDefault();

                                    if (string.IsNullOrEmpty(TransSTRC))
                                    {
                                        sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo + " - No.BASPB ini belum di proses serah terima Gudang BASPB";
                                        continue;
                                    }

                                    NoTrans = (from q in db.SCMS_STHs
                                               join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                               where q1.c_no == fields.TransNo && q.c_type == "08"
                                               select q1.c_no).Take(1).SingleOrDefault();

                                    if (!string.IsNullOrEmpty(NoTrans))
                                    {
                                        sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo + " - No.BASPB ini sudah diproses serah terima Admin BASPB";
                                        continue;
                                    }

                                    //rch = listResRCH.Find(delegate(LG_RCH rchdr)
                                    //{
                                    //    return fields.TransNo.Equals((string.IsNullOrEmpty(rchdr.v_pbbrno) ? string.Empty : rchdr.v_pbbrno), StringComparison.OrdinalIgnoreCase);
                                    //});

                                    //if ((rch != null))
                                    //{
                                    //    sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo + " - No.PBB/R ini sudah diproses RC";
                                    //    continue;
                                    //}
                                    else
                                    {

                                        string cabDcore = fields.TransNo.Substring(3, 3);
                                        sCus = (from q in db.LG_CusmasCabs
                                                where q.c_cab_dcore == cabDcore
                                                select q.c_cusno).Take(1).SingleOrDefault();

                                        listStd.Add(new SCMS_STD()
                                        {
                                            c_nodoc = stID,
                                            c_no = fields.TransNo,
                                            c_cusno = sCus
                                        });

                                        totalDetails++;
                                    }
                                }
                            }

                            //listResRCH.Clear();
                            //listNO.Clear();

                            if (listStd.Count > 0)
                            {
                                db.SCMS_STDs.InsertAllOnSubmit(listStd.ToArray());
                                listStd.Clear();
                            }
                        }
                    }

                    #endregion

                    #region CPR
                    if (structure.Fields.Tipe.Equals("09"))
                    {
                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            listStd = new List<SCMS_STD>();

                            listNO = structure.Fields.Field.GroupBy(x => x.TransNo).Select(y => y.Key).ToList();

                            listResRSH = (from q in db.LG_RSHes
                                          where listNO.Contains(q.c_rsno)
                                          select q).Distinct().ToList();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                fields = structure.Fields.Field[nLoop];

                                if ((fields != null) && fields.IsNew && (!fields.IsDelete))
                                {
                                    NoTrans = (from q in db.SCMS_STHs
                                               join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                               where q1.c_no == fields.TransNo && q.c_type == "09"
                                               select q1.c_no).Take(1).SingleOrDefault();

                                    if (!string.IsNullOrEmpty(NoTrans))
                                    {
                                        sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo + " - No.RS ini sudah diproses serah terima CPR";
                                        continue;
                                    }

                                    rsh = listResRSH.Find(delegate(LG_RSH rshdr)
                                    {
                                        return fields.TransNo.Equals((string.IsNullOrEmpty(rshdr.c_rsno) ? string.Empty : rshdr.c_rsno), StringComparison.OrdinalIgnoreCase);
                                    });

                                    if ((rsh == null))
                                    {
                                        sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo + " - No.RS ini belum diproses RS";
                                        continue;
                                    }
                                    else
                                    {

                                        //string cabDcore = fields.TransNo.Substring(3, 3);
                                        //sCus = (from q in db.LG_CusmasCabs
                                        //        where q.c_cab_dcore == cabDcore
                                        //        select q.c_cusno).Take(1).SingleOrDefault();

                                        listStd.Add(new SCMS_STD()
                                        {
                                            c_nodoc = stID,
                                            c_no = fields.TransNo,
                                            //c_cusno = sCus
                                        });

                                        totalDetails++;
                                    }
                                }
                            }

                            listResRSH.Clear();
                            listNO.Clear();

                            if (listStd.Count > 0)
                            {
                                db.SCMS_STDs.InsertAllOnSubmit(listStd.ToArray());
                                listStd.Clear();
                            }
                        }
                    }

                    #endregion

                    #region old
                    //if (structure.Fields.Tipe.Equals("04"))
                    //{                    
                    //    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    //    {
                    //        listStd = new List<SCMS_STD>();

                    //        listNO = structure.Fields.Field.GroupBy(x => x.TransNo).Select(y => y.Key).ToList();

                    //        listResSTD = (from q in db.SCMS_STDs
                    //                      join q1 in db.SCMS_STHs on q.c_nodoc equals q1.c_nodoc
                    //                      where listNO.Contains(q.c_no) && q1.c_type == "03"
                    //                      select q).Distinct().ToList();

                    //        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    //        {
                    //            fields = structure.Fields.Field[nLoop];
                    //            if ((fields != null) && fields.IsNew && (!fields.IsDelete))
                    //            {
                    //                std = listResSTD.Find(delegate(SCMS_STD sthdr)
                    //                {
                    //                    return fields.TransNo.Equals((string.IsNullOrEmpty(sthdr.c_no) ? string.Empty : sthdr.c_no), StringComparison.OrdinalIgnoreCase);
                    //                });

                    //                if ((std != null))
                    //                {
                    //                    var CheckEPD = (from q in db.LG_ExpDs
                    //                                    where q.c_dono == std.c_no
                    //                                    select q).Distinct().ToList();

                    //                    if (CheckEPD.Count > 0)
                    //                    {
                    //                        sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo;
                    //                        continue;
                    //                    }

                    //                    listStd.Add(new SCMS_STD()
                    //                    {
                    //                        c_nodoc = stID,
                    //                        c_no = fields.TransNo,
                    //                        c_type_lat = std.c_type_lat
                    //                    });

                    //                    totalDetails++;
                    //                }
                    //                else
                    //                {
                    //                    sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo;
                    //                }
                    //            }
                    //        }

                    //        listResSTD.Clear();
                    //        listNO.Clear();

                    //        if (listStd.Count > 0)
                    //        {
                    //            db.SCMS_STDs.InsertAllOnSubmit(listStd.ToArray());
                    //            listStd.Clear();
                    //        }
                    //    }
                    //}

                    #endregion

                    #endregion

                    dic = new Dictionary<string, string>();

                    if (totalDetails > 0)
                    {
                        dic.Add("WP", stID);
                        dic.Add("Tanggal", date.ToString("yyyyMMdd"));
                        dic.Add("Transaksi_Salah", sTempTransCancel);

                        result = string.Format("Total {0} detail(s)", totalDetails);

                        hasAnyChanges = true;
                    }
                    else
                    {
                        hasAnyChanges = false;
                    }

                    #endregion
                }
                else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    #region Transportasi
                    if (structure.Fields.Tipe.Equals("04"))
                    {
                        string[] typePL = new string[] { "02", "06" };

                        stID = structure.Fields.NoDoc;
                        sth = (from q in db.SCMS_STHs
                               where q.c_nodoc == stID
                               select q).Take(1).SingleOrDefault();

                        if (sth == null)
                        {
                            result = "Nomor dokumen Serah Terima tidak ditemukan.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                        else
                        {
                            sth.d_update = date;

                            if (structure.Fields.Koli >= 0)
                            {
                                sth.n_koli = structure.Fields.Koli;
                            }

                            if (structure.Fields.Receh >= 0)
                            {
                                sth.n_receh = structure.Fields.Receh;
                            }

                            if (structure.Fields.KoliReceh >= 0)
                            {
                                sth.n_kolireceh = structure.Fields.KoliReceh;
                            }

                            if (structure.Fields.Berat >= 0)
                            {
                                sth.n_berat = structure.Fields.Berat;
                            }

                            if (structure.Fields.Volume >= 0)
                            {
                                sth.n_vol = structure.Fields.Volume;
                            }

                            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                            {
                                listStd = new List<SCMS_STD>();

                                listNO = structure.Fields.Field.GroupBy(x => x.TransNo).Select(y => y.Key).ToList();

                                listResDOH = (from q in db.LG_DOHs
                                              where listNO.Contains(q.c_dono)
                                              select q).Distinct().ToList();

                                listResSJH = (from q in db.LG_SJHs
                                              where listNO.Contains(q.c_sjno)
                                              select q).Distinct().ToList();

                                for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                                {
                                    fields = structure.Fields.Field[nLoop];
                                    if ((fields != null) && fields.IsNew && (!fields.IsDelete))
                                    {
                                        doh = listResDOH.Find(delegate(LG_DOH dohdr)
                                        {
                                            return fields.TransNo.Equals((string.IsNullOrEmpty(dohdr.c_dono) ? string.Empty : dohdr.c_dono), StringComparison.OrdinalIgnoreCase);
                                        });

                                        sjh = listResSJH.Find(delegate(LG_SJH sjhdr)
                                        {
                                            return fields.TransNo.Equals((string.IsNullOrEmpty(sjhdr.c_sjno) ? string.Empty : sjhdr.c_sjno), StringComparison.OrdinalIgnoreCase)
                                                && sjhdr.l_confirm == true;
                                        });

                                        if ((doh != null) || (sjh != null))
                                        {
                                            if (doh != null)
                                            {
                                                listResSTD = (from q in db.SCMS_STDs
                                                              join q1 in db.SCMS_STHs on q.c_nodoc equals q1.c_nodoc
                                                              where q.c_no == doh.c_plno && q1.c_type == "03"
                                                              select q).Distinct().ToList();

                                                listResPLH = (from q in db.LG_PLHs
                                                              where q.c_plno == doh.c_plno
                                                              select q).Distinct().ToList();

                                                if (!string.IsNullOrEmpty(fields.editkoli) && fields.IsModifiedKoli)
                                                {
                                                    doh.n_karton = fields.karton;
                                                    doh.c_editkoli = fields.editkoli;
                                                }
                                                doh.n_receh = fields.receh;
                                            }
                                            else
                                            {
                                                listResSTD = (from q in db.SCMS_STDs
                                                              join q1 in db.SCMS_STHs on q.c_nodoc equals q1.c_nodoc
                                                              where q.c_no == sjh.c_sjno && q1.c_type == "03"
                                                              select q).Distinct().ToList();

                                                if (!string.IsNullOrEmpty(fields.editkoli) && fields.IsModifiedKoli)
                                                {
                                                    sjh.n_karton = fields.karton;
                                                    sjh.c_editkoli = fields.editkoli;
                                                }
                                                sjh.n_receh = fields.receh;
                                            }
                                            if (listResSTD.Count > 0)
                                            {
                                                var CheckEPD = (from q in db.LG_ExpDs
                                                                where q.c_dono == listResSTD[0].c_no.ToString()
                                                                select q).Distinct().ToList();

                                                if (CheckEPD.Count > 0)
                                                {
                                                    sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo;
                                                    continue;
                                                }

                                                if (listResPLH != null)
                                                {
                                                    sLat = listResPLH[0].c_type_lat;
                                                    sCus = listResPLH[0].c_cusno;
                                                    listResPLH.Clear();
                                                }
                                                else
                                                {
                                                    sLat = sjh.c_type_lat;
                                                    sCus = sjh.c_gdg2.Value.ToString();
                                                }

                                                if (sCus != structure.Fields.Cusno)
                                                {
                                                    result = fields.TransNo + " ini salah cabang/gudang. Mohon direvisi...";

                                                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                                    goto endLogic;
                                                }

                                                listStd.Add(new SCMS_STD()
                                                {
                                                    c_nodoc = stID,
                                                    c_no = fields.TransNo,
                                                    c_type_lat = sLat,
                                                    c_cusno = sCus
                                                });

                                                totalDetails++;
                                            }
                                            else
                                            {
                                                if (listResPLH != null)
                                                {
                                                    if (typePL.Contains(listResPLH[0].c_type.ToString()))
                                                    {
                                                        sLat = (string.IsNullOrEmpty(listResPLH[0].c_type_lat) ? string.Empty : listResPLH[0].c_type_lat);
                                                        sCus = listResPLH[0].c_cusno.ToString();
                                                        listResPLH.Clear();

                                                        if (sCus != structure.Fields.Cusno)
                                                        {
                                                            result = fields.TransNo + " ini salah cabang/gudang. Mohon direvisi...";

                                                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                                            goto endLogic;
                                                        }

                                                        listStd.Add(new SCMS_STD()
                                                        {
                                                            c_nodoc = stID,
                                                            c_no = fields.TransNo,
                                                            //n_koli = fields.Koli,
                                                            //n_berat = fields.Berat,
                                                            c_type_lat = sLat,
                                                            c_cusno = sCus
                                                        });

                                                        totalDetails++;
                                                        listResSTD.Clear();

                                                        continue;
                                                    }
                                                }
                                                if (sjh != null)
                                                {
                                                    if (sjh.l_auto == true)
                                                    {
                                                        sLat = (string.IsNullOrEmpty(sjh.c_type_lat) ? string.Empty : sjh.c_type_lat);
                                                        sCus = sjh.c_gdg2.Value.ToString();

                                                        if (sCus != structure.Fields.Cusno)
                                                        {
                                                            result = fields.TransNo + " ini salah cabang/gudang. Mohon direvisi...";

                                                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                                            goto endLogic;
                                                        }

                                                        listStd.Add(new SCMS_STD()
                                                        {
                                                            c_nodoc = stID,
                                                            c_no = fields.TransNo,
                                                            //n_koli = fields.Koli,
                                                            //n_berat = fields.Berat,
                                                            c_type_lat = sLat,
                                                            c_cusno = sCus
                                                        });

                                                        totalDetails++;
                                                        listResSTD.Clear();

                                                        continue;
                                                    }
                                                }
                                                sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo;
                                            }
                                            listResSTD.Clear();
                                        }
                                        else
                                        {
                                            sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo;
                                        }
                                    }
                                    else if ((fields != null) && fields.IsModifiedKoli && (!fields.IsDelete))
                                    {
                                         doh = listResDOH.Find(delegate(LG_DOH dohdr)
                                        {
                                            return fields.TransNo.Equals((string.IsNullOrEmpty(dohdr.c_dono) ? string.Empty : dohdr.c_dono), StringComparison.OrdinalIgnoreCase);
                                        });

                                        sjh = listResSJH.Find(delegate(LG_SJH sjhdr)
                                        {
                                            return fields.TransNo.Equals((string.IsNullOrEmpty(sjhdr.c_sjno) ? string.Empty : sjhdr.c_sjno), StringComparison.OrdinalIgnoreCase)
                                                && sjhdr.l_confirm == true;
                                        });

                                        if ((doh != null) || (sjh != null))
                                        {
                                            if (doh != null)
                                            {
                                                if (!string.IsNullOrEmpty(fields.editkoli) && fields.IsModifiedKoli)
                                                {
                                                    doh.n_karton = fields.karton;
                                                    doh.c_editkoli = fields.editkoli;
                                                }
                                                doh.n_receh = fields.receh;
                                            }
                                            else
                                            {
                                                if (!string.IsNullOrEmpty(fields.editkoli) && fields.IsModifiedKoli)
                                                {
                                                    sjh.n_karton = fields.karton;
                                                    sjh.c_editkoli = fields.editkoli;
                                                }
                                                sjh.n_receh = fields.receh;
                                            }
                                        }
                                    }
                                }

                                listResDOH.Clear();
                                listResSJH.Clear();

                                listNO.Clear();

                                if (listStd.Count > 0)
                                {
                                    db.SCMS_STDs.InsertAllOnSubmit(listStd.ToArray());
                                    listStd.Clear();
                                }
                            }

                            hasAnyChanges = true;
                        }
                    }
                    #endregion

                    #region Gudang PBB/R
                    if (structure.Fields.Tipe.Equals("05"))
                    {
                        stID = structure.Fields.NoDoc;
                        sth = (from q in db.SCMS_STHs
                               where q.c_nodoc == stID
                               select q).Take(1).SingleOrDefault();

                        if (sth == null)
                        {
                            result = "Nomor dokumen Serah Terima tidak ditemukan.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                        else
                        {
                            sth.c_update = structure.Fields.Entry;
                            sth.d_update = date;

                            if (structure.Fields.Koli >= 0)
                            {
                                sth.n_koli = structure.Fields.Koli;
                            }

                            if (!string.IsNullOrEmpty(structure.Fields.Resi))
                            {
                                sth.c_resi = structure.Fields.Resi;
                            }

                            if (!string.IsNullOrEmpty(structure.Fields.Give))
                            {
                                sth.c_give = structure.Fields.Give;
                            }

                            if (!string.IsNullOrEmpty(structure.Fields.GiveName))
                            {
                                sth.v_give = structure.Fields.GiveName;
                            }

                            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                            {
                                listStd = new List<SCMS_STD>();

                                listNO = structure.Fields.Field.GroupBy(x => x.TransNo).Select(y => y.Key).ToList();

                                listResRCH = (from q in db.LG_RCHes
                                              where listNO.Contains(q.v_pbbrno)
                                              select q).Distinct().ToList();

                                for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                                {
                                    fields = structure.Fields.Field[nLoop];
                                    if ((fields != null) && fields.IsNew && (!fields.IsDelete))
                                    {
                                        NoTrans = (from q in db.SCMS_STHs
                                                   join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                                   where q1.c_no == fields.TransNo && q.c_type == "05"
                                                   select q1.c_no).Take(1).SingleOrDefault();

                                        if (!string.IsNullOrEmpty(NoTrans))
                                        {
                                            sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo + " - No.PBB/R ini sudah diproses serah terima PBB/R";
                                            continue;
                                        }

                                        rch = listResRCH.Find(delegate(LG_RCH rchdr)
                                        {
                                            return fields.TransNo.Equals((string.IsNullOrEmpty(rchdr.v_pbbrno) ? string.Empty : rchdr.v_pbbrno), StringComparison.OrdinalIgnoreCase);
                                        });

                                        if ((rch != null))
                                        {
                                            sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo + " - No.PBB/R ini sudah diproses RC";
                                            continue;
                                        }
                                        else
                                        {
                                            string cabDcore = fields.TransNo.Substring(3, 3);
                                            sCus = (from q in db.LG_CusmasCabs
                                                    where q.c_cab_dcore == cabDcore
                                                    select q.c_cab).Take(1).SingleOrDefault();

                                            listStd.Add(new SCMS_STD()
                                            {
                                                c_nodoc = stID,
                                                c_no = fields.TransNo,
                                                c_cusno = sCus
                                            });
                                        }
                                    }
                                }

                                listResRCH.Clear();
                                listNO.Clear();

                                if (listStd.Count > 0)
                                {
                                    db.SCMS_STDs.InsertAllOnSubmit(listStd.ToArray());
                                    listStd.Clear();
                                }
                            }

                            hasAnyChanges = true;
                        }
                    }
                    #endregion

                    #region Admin PBB/R
                    if (structure.Fields.Tipe.Equals("06"))
                    {
                        stID = structure.Fields.NoDoc;
                        sth = (from q in db.SCMS_STHs
                               where q.c_nodoc == stID
                               select q).Take(1).SingleOrDefault();

                        if (sth == null)
                        {
                            result = "Nomor dokumen Serah Terima tidak ditemukan.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                        else
                        {
                            sth.c_update = structure.Fields.Entry;
                            sth.d_update = date;

                            if (structure.Fields.Koli >= 0)
                            {
                                sth.n_koli = structure.Fields.Koli;
                            }

                            if (!string.IsNullOrEmpty(structure.Fields.Resi))
                            {
                                sth.c_resi = structure.Fields.Resi;
                            }

                            if (!string.IsNullOrEmpty(structure.Fields.Give))
                            {
                                sth.c_give = structure.Fields.Give;
                            }

                            if (!string.IsNullOrEmpty(structure.Fields.GiveName))
                            {
                                sth.v_give = structure.Fields.GiveName;
                            }

                            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                            {
                                listStd = new List<SCMS_STD>();

                                listNO = structure.Fields.Field.GroupBy(x => x.TransNo).Select(y => y.Key).ToList();

                                listResRCH = (from q in db.LG_RCHes
                                              where listNO.Contains(q.v_pbbrno)
                                              select q).Distinct().ToList();

                                for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                                {
                                    fields = structure.Fields.Field[nLoop];
                                    if ((fields != null) && fields.IsNew && (!fields.IsDelete))
                                    {
                                        string TransSTRC = (from q in db.SCMS_STHs
                                                            join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                                            where q1.c_no == fields.TransNo && q.c_type == "05"
                                                            select q1.c_no).Take(1).SingleOrDefault();

                                        if (string.IsNullOrEmpty(TransSTRC))
                                        {
                                            sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo + " - No.PBB/R ini belum di proses serah terima Gudang PBB/R";
                                            continue;
                                        }

                                        NoTrans = (from q in db.SCMS_STHs
                                                   join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                                   where q1.c_no == fields.TransNo && q.c_type == "06"
                                                   select q1.c_no).Take(1).SingleOrDefault();

                                        if (!string.IsNullOrEmpty(NoTrans))
                                        {
                                            sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo + " - No.PBB/R ini sudah diproses serah terima Admin PBB/R";
                                            continue;
                                        }

                                        rch = listResRCH.Find(delegate(LG_RCH rchdr)
                                        {
                                            return fields.TransNo.Equals((string.IsNullOrEmpty(rchdr.v_pbbrno) ? string.Empty : rchdr.v_pbbrno), StringComparison.OrdinalIgnoreCase);
                                        });

                                        if ((rch != null))
                                        {
                                            sTempTransCancel = sTempTransCancel + Environment.NewLine + fields.TransNo + " - No.PBB/R ini sudah diproses RC";
                                            continue;
                                        }
                                        else
                                        {
                                            string cabDcore = fields.TransNo.Substring(3, 3);
                                            sCus = (from q in db.LG_CusmasCabs
                                                    where q.c_cab_dcore == cabDcore
                                                    select q.c_cab).Take(1).SingleOrDefault();

                                            listStd.Add(new SCMS_STD()
                                            {
                                                c_nodoc = stID,
                                                c_no = fields.TransNo,
                                                c_cusno = sCus
                                            });
                                        }
                                    }
                                }

                                listResRCH.Clear();
                                listNO.Clear();

                                if (listStd.Count > 0)
                                {
                                    db.SCMS_STDs.InsertAllOnSubmit(listStd.ToArray());
                                    listStd.Clear();
                                }
                            }

                            hasAnyChanges = true;
                        }
                    }
                    #endregion

                    #region Gudang BASPB
                    if (structure.Fields.Tipe.Equals("07"))
                    {
                        stID = structure.Fields.NoDoc;
                        sth = (from q in db.SCMS_STHs
                               where q.c_nodoc == stID
                               select q).Take(1).SingleOrDefault();

                        if (sth == null)
                        {
                            result = "Nomor dokumen Serah Terima tidak ditemukan.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                        else
                        {
                            sth.c_update = structure.Fields.Entry;
                            sth.d_update = date;

                            if (structure.Fields.Koli >= 0)
                            {
                                sth.n_koli = structure.Fields.Koli;
                            }

                            if (!string.IsNullOrEmpty(structure.Fields.Resi))
                            {
                                sth.c_resi = structure.Fields.Resi;
                            }

                            if (!string.IsNullOrEmpty(structure.Fields.Give))
                            {
                                sth.c_give = structure.Fields.Give;
                            }

                            if (!string.IsNullOrEmpty(structure.Fields.GiveName))
                            {
                                sth.v_give = structure.Fields.GiveName;
                            }

                            hasAnyChanges = true;
                        }
                    }
                    #endregion
                    
                    #endregion

                    dic = new Dictionary<string, string>();

                    if (hasAnyChanges)
                    {
                        dic.Add("WP", stID);
                        dic.Add("Tanggal", date.ToString("yyyyMMdd"));
                        dic.Add("Transaksi_Salah", sTempTransCancel);

                        result = string.Format("Total {0} detail(s)", totalDetails);
                    }
                }
                else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete



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

                result = string.Format("ScmsSoaLibrary.Bussiness.WaktuPelayanan:WaktuPelayananLogistik - {0}", ex.Message);

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

        public string SerahTerimaTiket(ScmsSoaLibrary.Parser.Class.WaktuPelayananStructure structure)
        {
            if ((structure == null)) // || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;

            string stID = null,
                numbstr = null;

            int tmpnumb = 0;

            SCMS_WPH wph = null;
            SCMS_WPD wpd = null;

            DateTime date = DateTime.Now;

            ScmsSoaLibrary.Parser.Class.WaktuPelayananStructureField field = null;

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

                    wph = new SCMS_WPH();
                    dic = new Dictionary<string, string>();

                    stID = Commons.GenerateNumbering<SCMS_WPH>(db, "ST", '7', "07", date, "c_nodoc");

                    SCMS_WPH wpnumb = (from q in db.SCMS_WPHs
                                       //where q.d_wpdate == date
                                       where (SqlMethods.DateDiffDay(q.d_wpdate, date) == 0) && q.c_urut != "AUTO"
                                       select q).OrderByDescending(x => x.c_nodoc).Take(1).SingleOrDefault();

                    if (wpnumb != null)
                    {
                        tmpnumb = int.Parse(wpnumb.c_urut);
                        numbstr = (tmpnumb + 1).ToString().PadLeft(4, '0');
                    }
                    else
                    {
                        numbstr = "0001";
                    }

                    string v_nama = (from q in db.HR_MsKries
                                     where q.c_nip == nipEntry
                                     select q.v_nama).Take(1).SingleOrDefault();

                    wph = new SCMS_WPH()
                    {
                        //Indra 20180920FM
                        //SerahTerimaTransportasi
                        //c_gdg = '1',
                        c_gdg = Convert.ToChar(structure.Fields.Gudang),
                        c_nodoc = stID,
                        d_wpdate = date,
                        c_urut = numbstr,
                        c_nosup = structure.Fields.Nosup,
                        c_plat = structure.Fields.Nopol,
                        c_type = "01",
                        c_entry = nipEntry,
                        v_entry = v_nama,
                        d_entry = date,
                        c_update = nipEntry,
                        d_update = date
                    };

                    dic.Add("nodoc", stID);

                    db.SCMS_WPHs.InsertOnSubmit(wph);
                    hasAnyChanges = true;
                }

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

        public string SerahTerimaTiketPO(ScmsSoaLibrary.Parser.Class.WaktuPelayananStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false;
            string result = null,
              sTipe = null,
              wpID = null,
              sPemasok = null,
              sPlat = null,
              sAntrian = null,
              sTempTransCancel = null;

            int totalDetails = 0,
              nLoop = 0;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            SCMS_WPH wph = null;
            SCMS_WPD wpd = null;
            
            List<SCMS_WPD> listWpd = null;

            ScmsSoaLibrary.Parser.Class.WaktuPelayananStructureField fields = null;

            char GudangLog = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '0' : structure.Fields.Gudang[0]);

            string nipScan = (string.IsNullOrEmpty(structure.Fields.Scan) || (structure.Fields.Scan.Length < 1) ? string.Empty : structure.Fields.Scan);


            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now;

            IDictionary<string, string> dic = null;

            sTipe = (structure.Fields.Tipe ?? string.Empty);

            if (string.IsNullOrEmpty(nipScan))
            {
                result = "Nip Petugas Receiver dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            if (GudangLog.Equals(char.MinValue))
            {
                result = "Gudang tidak dapat dibaca.";

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

                    if (structure.Fields.Tipe.Equals("01"))
                    {
                        wpID = structure.Fields.NoDoc;

                        wpd = (from q in db.SCMS_WPDs
                               where q.c_nodoc == wpID
                               select q).Take(1).SingleOrDefault();

                        if (wpd != null)
                        {
                            result = "Nomor Barcode Antrian sudah pernah discan.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }

                        wph = (from q in db.SCMS_WPHs
                               where q.c_nodoc == wpID
                               select q).Take(1).SingleOrDefault();

                        if (wph == null)
                        {
                            result = "Nomor dokumen Serah Terima Tiket PO tidak ditemukan.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                        else
                        {
                            #region Update Header

                            wph.c_scan = nipScan;
                            wph.v_scan = structure.Fields.ScanName;
                            wph.d_scan = date;
                            wph.l_scan = true;
                            
                            sPlat = wph.c_plat;
                            sAntrian = wph.c_urut;
  
                            sPemasok = (from q in db.LG_DatSups
                                         where q.c_nosup == wph.c_nosup
                                         select q.v_nama).Take(1).SingleOrDefault();
                            #endregion

                            #region Insert Detail

                            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                            {
                                listWpd = new List<SCMS_WPD>();

                                for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                                {
                                    fields = structure.Fields.Field[nLoop];
                                    if ((fields != null) && fields.IsNew && (!fields.IsDelete))
                                    {
                                        listWpd.Add(new SCMS_WPD()
                                        {
                                            c_nodoc = wpID,
                                            c_no = fields.TransNo
                                        });

                                        totalDetails++;
                                    }
                                }

                                if (listWpd.Count > 0)
                                {
                                    db.SCMS_WPDs.InsertAllOnSubmit(listWpd.ToArray());
                                    listWpd.Clear();
                                }
                            }
                        }
                    }

                    #endregion

                    dic = new Dictionary<string, string>();

                    if (totalDetails > 0)
                    {
                        dic.Add("ST", wpID);
                        dic.Add("Tanggal", date.ToString("yyyyMMdd"));
                        dic.Add("Transaksi_Salah", sTempTransCancel);
                        dic.Add("Pemasok", sPemasok);
                        dic.Add("Plat", sPlat);
                        dic.Add("Antrian", sAntrian);

                        result = string.Format("Total {0} detail(s)", totalDetails);

                        hasAnyChanges = true;
                    }
                    else
                    {
                        hasAnyChanges = false;
                    }


                    #endregion
                }
                else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    #endregion

                    dic = new Dictionary<string, string>();

                    if (hasAnyChanges)
                    {
                        dic.Add("ST", wpID);
                        dic.Add("Tanggal", date.ToString("yyyyMMdd"));
                        dic.Add("Transaksi_Salah", sTempTransCancel);

                        result = string.Format("Total {0} detail(s)", totalDetails);
                    }
                }
                else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

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

                result = string.Format("ScmsSoaLibrary.Bussiness.WaktuPelayanan:SerahTerimaTiketPO - {0}", ex.Message);

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
