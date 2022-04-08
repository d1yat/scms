using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ScmsModel;
using ScmsModel.Core;
using ScmsSoaLibrary.Commons;
using ScmsMailLibrary.Global;
using ScmsSoaLibraryInterface.Commons;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;

namespace ScmsMailLibrary
{
    public class SendRCAuto
    {
        #region Internal Class

        //internal class TaskInfo
        //{
        //  public TaskInfo()
        //  {
        //    this.OtherInfo = "default";
        //    this.Handle = null;
        //  }

        //  public RegisteredWaitHandle Handle { get; set; }
        //  public string OtherInfo { get; set; }
        //}

        internal class RCAremasSenderFormat
        {
            public string c_jnstrans { get; set; }
            public DateTime d_datent { get; set; }
            public string c_timent { get; set; }
            public string c_invno { get; set; }
            public string c_exfak { get; set; }
            public string c_cusno { get; set; }
            public string c_corty { get; set; }
            public string c_srano { get; set; }
            public DateTime d_invdate { get; set; }
            public string c_payway { get; set; }
            public DateTime d_paydate { get; set; }
            public DateTime d_taxdate { get; set; }
            public decimal? n_grosva { get; set; }
            public decimal? n_disvsls { get; set; }
            public decimal n_disvund { get; set; }
            public decimal? n_netva { get; set; }
            public decimal n_taxva { get; set; }
            public decimal n_meterai { get; set; }
            public decimal? n_bilva { get; set; }
            public string c_print { get; set; }
            public string c_notax { get; set; }
            public string c_cancel { get; set; }
            public string c_expdisi { get; set; }
            public string c_userid { get; set; }
            public string c_areacode { get; set; }
            public string c_rayon { get; set; }
            public string c_stt { get; set; }
            public DateTime d_extgl { get; set; }
            public string c_exnotax { get; set; }
            public string c_cusno2 { get; set; }
            public string c_corty2 { get; set; }
            public string c_colno { get; set; }
            public string c_rynco { get; set; }
            public string c_nosp { get; set; }
            public string n_cetak { get; set; }
            public string c_inkano { get; set; }
            public string c_rynka { get; set; }
            public string c_pesan { get; set; }
            public decimal n_fee { get; set; }
            public decimal n_topcus { get; set; }
            public string c_nmjbt { get; set; }
            public string c_nmpsn { get; set; }
            public string m_catatan { get; set; }
            public string c_nodo { get; set; }
            public string c_oldinv { get; set; }
            public string c_pin { get; set; }
            public decimal n_exdisc { get; set; }
            public string c_from { get; set; }
        }

        internal class RCRetcusdSenderFormat
        {
            public string c_invno { get; set; }
            public string c_kddivams { get; set; }
            public string c_iteno { get; set; }
            public decimal? n_qtysalg { get; set; }
            public decimal? n_qtysalb { get; set; }
            public decimal? n_qtybong { get; set; }
            public decimal? n_qtybonb { get; set; }
            public decimal? n_salpri { get; set; }
            public decimal? n_discams { get; set; }
            public decimal? n_discpri { get; set; }
            public string c_kddivpri { get; set; }
            public decimal? n_discpst { get; set; }
            public decimal? n_ldiscpst { get; set; }
            public decimal? n_ldiscams { get; set; }
            public decimal? n_ldiscpri { get; set; }
            public decimal? n_meterai { get; set; }
        }

        internal class RCRetcusbcSenderFormat
        {
            public string c_invno { get; set; }
            public string c_iteno { get; set; }
            public string c_batch { get; set; }
            public decimal? n_qty { get; set; }
            public decimal? n_qtyr { get; set; }
            public DateTime? d_expired { get; set; }
        }

        internal class CabangInfo
        {
            public string KodeRC { get; set; }
            public string KodePIN { get; set; }
            public string KodeCabang { get; set; }
            public string NamaCabang { get; set; }
            public string Alamat1 { get; set; }
            public string Alamat2 { get; set; }
            public string[] Emails { get; set; }
        }

        #endregion

        private const string SMTP_IP = "10.100.10.9";
        private const int SMTP_PORT = 25;

        private const string DEFAULT_NAME_TABEL_RC_AREMAS = "RCAREMAS";
        private const string DEFAULT_NAME_TABEL_RC_RETCUSBC = "RCRETCUSBC";
        private const string DEFAULT_NAME_TABEL_RC_RETCUSD = "RCRETCUSD";
        private const string DEFAULT_NAME_TABEL_RC_LIST = "RCLIST";

        private const string MAIL_HEADER_NAME = "header.dbf";
        private const string MAIL_DETAIL_NAME = "detail.dbf";

        private AutoResetEvent areEvent;
        private AutoResetEvent areThreadStop;

        private ScmsSoaLibrary.Commons.Config config;

        private bool isStart;

        private volatile bool isRunning;

        public SendRCAuto(ScmsSoaLibrary.Commons.Config config)
        {
            this.config = config;

            string tmp = null,
              dir = null;

            if ((config == null) || string.IsNullOrEmpty(config.PathTempExtract))
            {
                tmp = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                tmp = (tmp.EndsWith("\\") ? tmp : string.Concat(tmp, "\\"));

                this.TempPath = tmp;
            }
            else
            {
                this.TempPath = config.PathTempExtract;
            }

            if ((config == null) || config.TimerPeriodicMailer.Equals(TimeSpan.MinValue))
            {
                this.TimePeriodic = new TimeSpan(0, 5, 0);
            }
            else
            {
                this.TimePeriodic = config.TimerPeriodicMailer;
            }

            if ((config == null) || string.IsNullOrEmpty(config.PathTempExtractMail))
            {
                tmp = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                tmp = System.IO.Path.Combine(config.PathTempExtractMail, "RCSend");

                tmp = (tmp.EndsWith("\\") ? tmp : string.Concat(tmp, "\\"));

                dir = tmp;
            }
            else
            {
                tmp = System.IO.Path.Combine(config.PathTempExtractMail, "RCSend");

                dir = (tmp.EndsWith("\\") ? tmp : string.Concat(tmp, "\\"));
            }

            try
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(dir);

                if (!di.Exists)
                {
                    di.Create();
                }

                this.MonitoringFolder = dir;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(
                  string.Format("ScmsMailLibrary.DOPharosReader:DOPharosReader - {0}", ex.Message));
                Logger.WriteLine(ex.StackTrace);
            }

            areEvent = new AutoResetEvent(false);

            this.IsActiveBackupSendRC = config.IsActiveBackupRCSend;
        }

        #region Delegate & Event

        public delegate void ClassNotifyEventHandler(object sender, ClassNotifyEventArgs e);

        public event ClassNotifyEventHandler ClassNotify;

        #endregion

        #region Private

        private Dictionary<string, System.IO.MemoryStream> CreateDBFStream(System.Data.DataTable table, string pathFolder)
        {
            Dictionary<string, System.IO.MemoryStream> dicMemStream = new Dictionary<string, System.IO.MemoryStream>(StringComparer.OrdinalIgnoreCase);

            System.Data.DataTable tableClone = table.Clone();

            System.Data.DataRow row = null;

            string keyName = null;
            System.IO.MemoryStream memStream = null;

            for (int nLoop = 0, nLen = table.Rows.Count; nLoop < nLen; nLoop++)
            {
                row = table.Rows[nLoop];

                try
                {
                    keyName = row.GetValue<string>("c_invno", string.Empty);

                    if ((!string.IsNullOrEmpty(keyName)) && (!dicMemStream.ContainsKey(keyName)))
                    {
                        tableClone.ImportRow(row);

                        memStream = ScmsMailLibrary.Core.Commons.CreateDBFStream(tableClone, pathFolder);

                        if (memStream != null)
                        {
                            dicMemStream.Add(keyName, memStream);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(ex.Message);
                    Logger.WriteLine(ex.StackTrace);
                }
                finally
                {
                    if (tableClone.Rows.Count > 0)
                    {
                        tableClone.Clear();
                    }
                }
            }

            tableClone.Dispose();

            return dicMemStream;
        }

        private Dictionary<string, System.IO.MemoryStream> CreateDBFStreamDetail(List<string> listParentKeys, System.Data.DataTable table, string pathFolder)
        {
            Dictionary<string, System.IO.MemoryStream> dicMemStream = new Dictionary<string, System.IO.MemoryStream>(StringComparer.OrdinalIgnoreCase);

            System.Data.DataTable tableClone = table.Clone();

            System.Data.DataView view = null;
            System.Data.DataRow row = null;

            string keyName = null;
            System.IO.MemoryStream memStream = null;

            int nLoop = 0, nLen = 0,
              nLoopC = 0, nLenC = 0;

            for (nLoop = 0, nLen = listParentKeys.Count; nLoop < nLen; nLoop++)
            {
                keyName = listParentKeys[nLoop];

                try
                {
                    if ((!string.IsNullOrEmpty(keyName)) && (!dicMemStream.ContainsKey(keyName)))
                    {
                        view = new System.Data.DataView(table, string.Format("c_invno = '{0}'", keyName), "c_iteno", System.Data.DataViewRowState.CurrentRows);

                        if (view.Count > 0)
                        {
                            for (nLoopC = 0, nLenC = view.Count; nLoopC < nLenC; nLoopC++)
                            {
                                row = view[nLoopC].Row;

                                tableClone.ImportRow(row);
                            }


                            memStream = ScmsMailLibrary.Core.Commons.CreateDBFStream(tableClone, pathFolder);

                            if (memStream != null)
                            {
                                dicMemStream.Add(keyName, memStream);
                            }
                        }

                        view.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(ex.Message);
                    Logger.WriteLine(ex.StackTrace);
                }
                finally
                {
                    if (tableClone.Rows.Count > 0)
                    {
                        tableClone.Clear();
                    }
                }
            }

            tableClone.Dispose();

            return dicMemStream;
        }

        private void BackupLogicFolder(System.Data.DataRow rowSuplInfo, Dictionary<string, System.IO.MemoryStream> dicMsHeader, Dictionary<string, System.IO.MemoryStream> dicMsDetail, Dictionary<string, string> dicPrinting)
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(this.MonitoringFolder);

            if (!di.Exists)
            {
                return;
            }

            System.IO.DirectoryInfo diSub = null;

            System.IO.FileStream fs = null;

            System.IO.StreamWriter sw = null;

            System.Data.DataSet dataset = null;

            string pathFile = null,
              pathFolder = System.IO.Path.Combine(this.MonitoringFolder, DateTime.Now.ToString("yyyyMMdd"));

            di = new System.IO.DirectoryInfo(pathFolder);

            string namaSupl = rowSuplInfo.GetValue<string>("NamaSuplier", string.Empty);
            string[] emailUsers = rowSuplInfo.GetValue<string[]>("Emails", new string[0]);


            try
            {
                if (!di.Exists)
                {
                    di.Create();
                }

                #region Pack Header

                foreach (KeyValuePair<string, System.IO.MemoryStream> kvp in dicMsHeader)
                {
                    diSub = di.CreateSubdirectory(kvp.Key);

                    if (diSub.Exists)
                    {
                        pathFile = System.IO.Path.Combine(diSub.FullName, string.Concat(kvp.Key, "H.dbf"));

                        fs = new System.IO.FileStream(pathFile, System.IO.FileMode.Create);
                        kvp.Value.WriteTo(fs);
                        fs.Close();
                        fs.Dispose();
                    }
                }

                #endregion

                #region Pack Detail

                foreach (KeyValuePair<string, System.IO.MemoryStream> kvp in dicMsDetail)
                {
                    diSub = di.CreateSubdirectory(kvp.Key);

                    if (diSub.Exists)
                    {
                        pathFile = System.IO.Path.Combine(diSub.FullName, string.Concat(kvp.Key, "D.dbf"));

                        fs = new System.IO.FileStream(pathFile, System.IO.FileMode.Create);
                        kvp.Value.WriteTo(fs);
                        fs.Close();
                        fs.Dispose();
                    }
                }

                #endregion

                #region Pack Printing

                foreach (KeyValuePair<string, string> kvp in dicPrinting)
                {
                    diSub = di.CreateSubdirectory(kvp.Key);

                    if (diSub.Exists)
                    {
                        pathFile = System.IO.Path.Combine(diSub.FullName, string.Concat(kvp.Key, ".txt"));

                        sw = new System.IO.StreamWriter(pathFile, false);
                        sw.Write(kvp.Value);
                        sw.Close();
                        sw.Dispose();
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                Logger.WriteLine(
                 string.Format("ScmsMailLibrary.SendRCAuto:BackupLogicFolder - {0}", ex.Message));

                Logger.WriteLine(ex.StackTrace);
            }
        }

        private void RunningLogicSender(string pathFolder)
        {
            ORMDataContext db = new ORMDataContext(this.config.ConnectionString);

            if (!System.IO.Directory.Exists(pathFolder))
            {
                System.IO.Directory.CreateDirectory(pathFolder);
            }

            System.Data.DataTable table = null,
              tableAremas = null, tableRCRetcusd = null, tableRCRetcusbc = null;
            System.IO.MemoryStream msAremas = null,
              msRCRetcusd = null,
              msRCRetcusbc = null;
            Dictionary<string, System.IO.MemoryStream> dicMsAremas = null,
              dicMsRCRetcusd = null,
              dicMsRCRetcusbc = null;

            List<string> lstKeyAremas = null;

            System.Data.DataSet dataset = null;

            System.Data.DataRow row = null;

            int nLoop = 0,
              nLen = 0;

            string kodeCab = null, rcNo = null, pin = null;

            Dictionary<string, string> dicPrinting = null;

            System.Data.DataTable tableMailCabang = ReadAllActiveCabang(db);

            System.Data.DataView view = new System.Data.DataView(tableMailCabang, null, "KodeRC", System.Data.DataViewRowState.CurrentRows);

            //this.ClassNotify

            for (nLoop = 0, nLen = view.Count; nLoop < nLen; nLoop++)
            {
                row = view[nLoop].Row;

                kodeCab = row.GetValue<string>("KodeCabang", string.Empty);
                rcNo = row.GetValue<string>("KodeRC", string.Empty);
                pin = row.GetValue<string>("KodePIN", string.Empty);


                if (!string.IsNullOrEmpty(kodeCab))
                {
                    dataset = CreateReaderSetRCMultiple(db, kodeCab, rcNo);

                    if (dataset != null)
                    {
                        if (dataset.Tables.Count == 4)
                        {
                            #region DEFAULT_NAME_TABEL_RC_AREMAS

                            if (dataset.Tables.Contains(DEFAULT_NAME_TABEL_RC_AREMAS))
                            {
                                tableAremas = dataset.Tables[DEFAULT_NAME_TABEL_RC_AREMAS];
                                dicMsAremas = this.CreateDBFStream(tableAremas, pathFolder);
                                //msHeader = ScmsMailLibrary.Core.Commons.CreateDBFStream(tableHeader, pathFolder);

                                if (dicMsAremas.Count > 0)
                                {
                                    #region DEFAULT_NAME_TABEL_RC_RETCUSD & DEFAULT_NAME_TABEL_RC_RETCUSBC

                                    if ((dataset.Tables.Contains(DEFAULT_NAME_TABEL_RC_RETCUSD)) && (dataset.Tables.Contains(DEFAULT_NAME_TABEL_RC_RETCUSBC)))
                                    {
                                        lstKeyAremas = dicMsAremas.Keys.CollectionToList();

                                        tableRCRetcusd = dataset.Tables[DEFAULT_NAME_TABEL_RC_RETCUSD];
                                        //msDetail = ScmsMailLibrary.Core.Commons.CreateDBFStream(tableDetail, pathFolder);
                                        dicMsRCRetcusd = this.CreateDBFStreamDetail(lstKeyAremas, tableRCRetcusd, pathFolder);


                                        tableRCRetcusbc = dataset.Tables[DEFAULT_NAME_TABEL_RC_RETCUSBC];
                                        //msDetail = ScmsMailLibrary.Core.Commons.CreateDBFStream(tableDetail, pathFolder);
                                        dicMsRCRetcusbc = this.CreateDBFStreamDetail(lstKeyAremas, tableRCRetcusbc, pathFolder);

                                        //dicPrinting = PrintPageMultiple(lstKeyAremas, row, tableAremas, tableRCRetcusd);

                                        if (SendMailMultiple(row, dicMsAremas, dicMsRCRetcusd, dicMsRCRetcusbc, pin))
                                        {
                                            #region DEFAULT_NAME_TABEL_RC_LIST

                                            if (dataset.Tables.Contains(DEFAULT_NAME_TABEL_RC_LIST))
                                            {
                                                table = dataset.Tables[DEFAULT_NAME_TABEL_RC_LIST];

                                                UpdateAllToTableMultiple(db, lstKeyAremas);

                                                if (table != null)
                                                {
                                                    table.Clear();
                                                    table.Dispose();
                                                }
                                            }

                                            #endregion
                                        }

                                        if (this.IsActiveBackupSendRC)
                                        {
                                            BackupLogicFolder(row, dicMsAremas, dicMsRCRetcusd, dicPrinting);
                                        }

                                        if (dicMsAremas.Count > 0)
                                        {
                                            foreach (KeyValuePair<string, System.IO.MemoryStream> kvp in dicMsAremas)
                                            {
                                                kvp.Value.Close();
                                                kvp.Value.Dispose();
                                            }

                                            dicMsAremas.Clear();
                                        }

                                        if (dicMsRCRetcusd.Count > 0)
                                        {
                                            foreach (KeyValuePair<string, System.IO.MemoryStream> kvp in dicMsRCRetcusd)
                                            {
                                                kvp.Value.Close();
                                                kvp.Value.Dispose();
                                            }

                                            dicMsRCRetcusd.Clear();
                                        }

                                        if (dicMsRCRetcusbc.Count > 0)
                                        {
                                            foreach (KeyValuePair<string, System.IO.MemoryStream> kvp in dicMsRCRetcusbc)
                                            {
                                                kvp.Value.Close();
                                                kvp.Value.Dispose();
                                            }

                                            dicMsRCRetcusd.Clear();
                                        }

                                        //dicPrinting.Clear();

                                        if (msRCRetcusd != null)
                                        {
                                            msRCRetcusd.Close();
                                            msRCRetcusd.Dispose();
                                        }

                                        if (tableRCRetcusd != null)
                                        {
                                            tableRCRetcusd.Clear();
                                            tableRCRetcusd.Dispose();
                                        }

                                        if (tableRCRetcusbc != null)
                                        {
                                            tableRCRetcusbc.Clear();
                                            tableRCRetcusbc.Dispose();
                                        }
                                    }

                                    #endregion
                                }

                                if (msAremas != null)
                                {
                                    msAremas.Close();
                                    msAremas.Dispose();
                                }

                                if (tableAremas != null)
                                {
                                    tableAremas.Clear();
                                    tableAremas.Dispose();
                                }
                            }

                            #endregion
                        }

                        dataset.Tables.Clear();
                        dataset.Clear();
                        dataset.Dispose();
                    }
                }
            }

            view.Dispose();

            if (tableMailCabang != null)
            {
                tableMailCabang.Clear();
                tableMailCabang.Dispose();
            }

            if (lstKeyAremas != null)
            {
                lstKeyAremas.Clear();
            }

            db.Dispose();

            GC.Collect();
        }

        #region Runner

        private System.Data.DataTable ReadAllActiveCabang(ORMDataContext db)
        {
            System.Data.DataTable table = null;

            var qry = (from q in db.LG_CusmasEmails
                       join q1 in db.LG_Cusmas on q.c_cusno equals q1.c_cusno
                       join q2 in db.LG_RCHes on q.c_cusno equals q2.c_cusno
                       where (q.c_form == "18")
                           //&& q2.c_rcno == "RC15010629"
                       && ((q2.l_send.HasValue ? q2.l_send.Value : false) == true)
                       && ((q2.l_sent.HasValue ? q2.l_sent.Value : false) == true)
                       && ((q1.l_cabang.HasValue ? q1.l_cabang.Value : false) == true)
                       && ((q1.l_rcdcore.HasValue ? q1.l_rcdcore.Value : false) == false)
                       //group new { q, q1, q2 } by new { q.c_cusno, q1.v_cunam, q1.v_adrtax1, q1.v_adrtax2 } into g
                       select new CabangInfo()
                       {
                           KodeRC = q2.c_rcno,
                           KodePIN = q2.c_pin,
                           KodeCabang = q2.c_cusno,
                           NamaCabang = q1.v_cunam,
                           Alamat1 = q1.v_adrtax1,
                           Alamat2 = q1.v_adrtax2,
                           Emails = (from sq in db.LG_CusmasEmails
                                     where (sq.c_cusno == q2.c_cusno)
                                      && (sq.c_form == "18")
                                     select sq.v_email).Distinct().ToArray()
                       }).Distinct().AsQueryable();

            table = qry.CopyToDataTableObject();

            return table;
        }

        #region Old Coded

        //private System.Data.DataSet CreateReaderSetPO(ORMDataContext db, string noSup, string PoNo)
        //{
        //  System.Data.DataSet dataset = new System.Data.DataSet();
        //  System.Data.DataTable table = null;

        //  #region Header

        //  var qry = (from q in db.LG_POHs
        //             join q1 in db.LG_POD1s on new { q.c_gdg, q.c_pono } equals new { q1.c_gdg, q1.c_pono }
        //             join q2 in db.LG_POD2s on new { q.c_gdg, q.c_pono } equals new { q2.c_gdg, q2.c_pono } into q_2
        //             from qPOD2 in q_2.DefaultIfEmpty()
        //             join q3 in db.LG_ORD2s on new { q.c_gdg, qPOD2.c_orno } equals new { q3.c_gdg, q3.c_orno } into q_3
        //             from qORD2 in q_3.DefaultIfEmpty()
        //             join q4 in db.LG_ORHs on new { qORD2.c_gdg, qORD2.c_orno } equals new { q4.c_gdg, q4.c_orno } into q_4
        //             from qORH in q_4.DefaultIfEmpty()
        //             join q5 in db.LG_SPHs on qORD2.c_spno equals q5.c_spno into q_5
        //             from qSPH in q_5.DefaultIfEmpty()
        //             join q6 in db.LG_Cusmas on qSPH.c_cusno equals q6.c_cusno into q_6
        //             from qCus in q_6.DefaultIfEmpty()
        //             where (q.c_nosup == noSup)
        //              && ((q.l_send.HasValue ? q.l_send.Value : false) == true)
        //              && ((q.l_print.HasValue ? q.l_print.Value : false) == true) 
        //              && ((q.l_sent.HasValue ? q.l_sent.Value : false) == false)
        //              && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
        //              && ((string.IsNullOrEmpty(PoNo) ? q.c_pono : PoNo) == q.c_pono)
        //              && (((qORH != null) ?
        //              (qORH.l_delete.HasValue ? qORH.l_delete.Value : false) : false) == false)
        //                && (((qSPH != null) ?
        //                (qSPH.l_delete.HasValue ? qSPH.l_delete.Value : false) : false) == false)
        //                  && (((qCus != null) ?
        //                  (qCus.l_stscus.HasValue ? qCus.l_stscus.Value : false) : true) == true)
        //             select new
        //             {
        //               q,
        //               q1,
        //               qCus
        //             }).Distinct().AsQueryable();

        //  List<POHeaderSenderFormat> lstHdr = (from q in qry
        //                                       select new POHeaderSenderFormat()
        //                                       {
        //                                         C_CORNO = (string.IsNullOrEmpty(q.q.c_pono) ? "" : q.q.c_pono.Substring(2, 8)),
        //                                         D_CORDA = (q.q.d_podate.HasValue ? q.q.d_podate.Value : ScmsSoaLibrary.Commons.Functionals.StandardSqlDateTime),
        //                                         C_KOMEN1 = q.q.v_ket,
        //                                         C_KOMEN2 = null,
        //                                         C_KDDEPO = "",
        //                                         L_LOAD = true,
        //                                         C_KDCAB = q.qCus.c_cusno,
        //                                         C_NMCAB = q.qCus.v_cunam
        //                                       }).Distinct().ToList();

        //  table = lstHdr.CopyToDataTableObject();
        //  table.TableName = DEFAULT_NAME_TABEL_PO_HEADER;

        //  dataset.Tables.Add(table);

        //  #endregion

        //  #region Detail

        //  var qryDtlSub = (from q in qry
        //                   join q7 in db.FA_MasItms on q.q1.c_iteno equals q7.c_iteno into q_7
        //                   from qItm in q_7.DefaultIfEmpty()
        //                   join q8 in db.LG_Vias on new { q.qCus.c_cusno, qItm.c_iteno } equals new { q8.c_cusno, q8.c_iteno } into q_8
        //                   from qViaCus in q_8.DefaultIfEmpty()
        //                   join q9 in db.MsTransDs on new { c_portal = '3', c_notrans = "02", c_type = qViaCus.c_via } equals new { q9.c_portal, q9.c_notrans, q9.c_type } into q_9
        //                   from qViaCusDesc in q_9.DefaultIfEmpty()
        //                   select new
        //                   {
        //                     q,
        //                     q.q1,
        //                     //q.qCus,
        //                     qItm,
        //                     //qViaCus,
        //                     qViaCusDesc
        //                   }).Distinct().AsQueryable();

        //  List<PODetailSenderFormat> lstDtl = (from q in qryDtlSub
        //                                       join q10 in db.MsTransDs on new { c_portal = '3', c_notrans = "02", c_type = q.qItm.c_via } equals new { q10.c_portal, q10.c_notrans, q10.c_type } into q_10
        //                                       from qViaItmDesc in q_10.DefaultIfEmpty()
        //                                       select new PODetailSenderFormat()
        //                                       {
        //                                         c_corno = (string.IsNullOrEmpty(q.q.q.c_pono) ? "" : q.q.q.c_pono.Substring(2, 8)),
        //                                         c_iteno = q.q1.c_iteno,
        //                                         c_itenopri = q.qItm.c_itenopri,
        //                                         c_itnam = q.qItm.v_itnam,
        //                                         c_nosp = "",
        //                                         c_undes = q.qItm.v_undes,
        //                                         c_via = ((q.qViaCusDesc != null) ? (string.IsNullOrEmpty(q.qViaCusDesc.v_ket) ? "Darat" : q.qViaCusDesc.v_ket) :
        //                                                   ((qViaItmDesc != null) ? (string.IsNullOrEmpty(qViaItmDesc.v_ket) ? "Darat" : qViaItmDesc.v_ket) :
        //                                                     "Darat")).Substring(0, 1).ToUpper(),
        //                                         n_qty = (q.q1.n_qty.HasValue ? q.q1.n_qty.Value : 0),
        //                                         n_salpri = (q.qItm.n_salpri.HasValue ? q.qItm.n_salpri.Value : 0)
        //                                       }).Distinct().ToList();

        //  table = lstDtl.CopyToDataTableObject();
        //  table.TableName = DEFAULT_NAME_TABEL_PO_DETAIL;

        //  dataset.Tables.Add(table);

        //  #endregion

        //  #region All PO Sended

        //  if ((lstHdr != null) && (lstHdr.Count > 0))
        //  {
        //    List<string> lstTotalPO = lstHdr.GroupBy(x => x.C_CORNO).Select(y => string.Concat("PO", y.Key)).ToList();

        //    if ((lstTotalPO != null) && (lstTotalPO.Count > 0))
        //    {
        //      table = new System.Data.DataTable(DEFAULT_NAME_TABEL_PO_LIST);

        //      table.Columns.Add("PO", typeof(string));

        //      table.BeginLoadData();

        //      for (int nLoop = 0, nLen = lstTotalPO.Count; nLoop < nLen; nLoop++)
        //      {
        //        table.LoadDataRow(new object[] { lstTotalPO[nLoop] }, true);
        //      }

        //      table.EndLoadData();

        //      dataset.Tables.Add(table);

        //      lstTotalPO.Clear();
        //    }
        //  }

        //  #endregion

        //  lstDtl.Clear();
        //  lstHdr.Clear();

        //  return dataset;
        //}

        #endregion

        private string PadLeftRight(string dataString, int lenPad, char leftChar, char rightChar)
        {
            string result = null;
            int lenData = dataString.Length,
              subData = (lenData >= lenPad ? 0 : ((lenPad - lenData) / 2));

            result = string.Concat("".PadLeft(subData, leftChar), dataString, "".PadLeft(subData, rightChar));

            return result;
        }

        private System.Data.DataSet CreateReaderSetRCMultiple(ORMDataContext db, string noCab, string RcNo)
        {
            System.Data.DataSet dataset = new System.Data.DataSet();
            System.Data.DataTable table = null;

            DateTime date = DateTime.Now;

            string groupPo = date.ToString("yyMMddHHmm");

            List<RCAremasSenderFormat> lstAremas = null;
            List<RCRetcusdSenderFormat> lstRCRetcusd = null;
            List<RCRetcusbcSenderFormat> lstRCRetcusbc = null;

            //temp
            ///////////////////

            #region AREMAS

            var qrytes = (from q in db.LG_RCHes
                          join q1 in db.LG_RCD1s on new { q.c_rcno, q.c_gdg } equals new { q1.c_rcno, q1.c_gdg }
                          join q2 in db.LG_RCD3s on new { q.c_rcno, q1.c_iteno } equals new { q2.c_rcno, q2.c_iteno }
                          where q.c_cusno == noCab
                          && q.c_rcno == RcNo
                          && ((q.l_send.HasValue ? q.l_send.Value : false) == true)
                          && ((q.l_sent.HasValue ? q.l_sent.Value : false) == true)
                          && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                          //&& q.c_rcno == ""
                          select new
                          {
                              q.c_rcno,
                              q.c_pin,
                              q.d_rcdate,
                              q.d_entry,
                              q.c_gdg,
                              q1.n_qtyrcv,
                              q1.c_iteno,
                              q2.n_salpri,
                              q2.n_disc
                          }).AsQueryable();

            lstAremas = (from q in qrytes
                         group q by new { q.c_gdg, q.c_rcno, q.d_rcdate, q.d_entry, q.c_pin, } into g
                         select new RCAremasSenderFormat()
                         {
                             c_jnstrans = "C",
                             d_datent = (g.Key.d_rcdate.HasValue ? g.Key.d_rcdate.Value : Functionals.StandardSqlDateTime), //convert
                             c_timent = g.Key.d_entry.Value.Hour.ToString().PadLeft(2, '0') + ":" + g.Key.d_entry.Value.Minute.ToString().PadLeft(2, '0') + ":" + g.Key.d_entry.Value.Second.ToString().PadLeft(2, '0'),
                             c_invno = g.Key.c_rcno.Substring(3, Math.Min(7, g.Key.c_rcno.Length)),
                             c_exfak = g.Key.c_rcno.Substring(3, Math.Min(7, g.Key.c_rcno.Length)),
                             c_cusno = string.Empty,
                             c_corty = string.Empty,
                             c_srano = string.Empty,
                             d_invdate = (g.Key.d_rcdate.HasValue ? g.Key.d_rcdate.Value : Functionals.StandardSqlDateTime), //convert
                             c_payway = string.Empty,
                             d_paydate = date,
                             d_taxdate = date,
                             n_grosva = g.Sum(x => (x.n_qtyrcv * x.n_salpri)),   //((q.n_qty * q.n_salpri) - ((q.n_qty * q.n_salpri) * (q.n_disc / 100))),
                             n_disvsls = g.Sum(x => ((x.n_qtyrcv * x.n_salpri) * (x.n_disc / 100))),
                             n_disvund = 0,
                             n_netva = g.Sum(x => (x.n_qtyrcv * x.n_salpri) - ((x.n_qtyrcv * x.n_salpri) * (x.n_disc / 100))),
                             n_taxva = 0,
                             n_meterai = 0,
                             n_bilva = g.Sum(x => (x.n_qtyrcv * x.n_salpri) - ((x.n_qtyrcv * x.n_salpri) * (x.n_disc / 100))),
                             c_print = string.Empty,
                             c_notax = string.Empty,
                             c_cancel = string.Empty,
                             c_expdisi = string.Empty,
                             c_userid = string.Empty,
                             c_areacode = string.Empty,
                             c_rayon = string.Empty,
                             c_stt = string.Empty,
                             d_extgl = (g.Key.d_rcdate.HasValue ? g.Key.d_rcdate.Value : Functionals.StandardSqlDateTime),   //d_datent
                             c_exnotax = string.Empty,
                             c_cusno2 = string.Empty,
                             c_corty2 = string.Empty,
                             c_colno = string.Empty,
                             c_rynco = string.Empty,
                             c_nosp = string.Empty,
                             n_cetak = string.Empty,
                             c_inkano = string.Empty,
                             c_rynka = string.Empty,
                             c_pesan = string.Empty,
                             n_fee = 0,
                             n_topcus = 0,
                             c_nmjbt = string.Empty,
                             c_nmpsn = string.Empty,
                             m_catatan = string.Empty,
                             c_nodo = string.Empty,
                             c_oldinv = string.Empty,
                             c_pin = g.Key.c_pin,
                             n_exdisc = 0,
                             c_from = g.Key.c_gdg.ToString().Trim().Equals("1") ? "X9" : "X8"
                         }).ToList();

            table = lstAremas.CopyToDataTableObject();
            table.TableName = DEFAULT_NAME_TABEL_RC_AREMAS;



            //set column format
            table.Columns["c_jnstrans"].MaxLength = 1;
            table.Columns["d_datent"].ExtendedProperties.Add("datetype", "1");
            table.Columns["c_timent"].MaxLength = 8;
            table.Columns["c_invno"].MaxLength = 7;
            table.Columns["c_exfak"].MaxLength = 7;
            table.Columns["c_cusno"].MaxLength = 1;
            table.Columns["c_corty"].MaxLength = 1;
            table.Columns["c_srano"].MaxLength = 1;
            table.Columns["d_invdate"].ExtendedProperties.Add("datetype", "1");
            table.Columns["c_payway"].MaxLength = 1;
            table.Columns["d_paydate"].ExtendedProperties.Add("datetype", "1");
            table.Columns["d_taxdate"].ExtendedProperties.Add("datetype", "1");
            table.Columns["n_grosva"].ExtendedProperties.Add("precision", "20,4");
            table.Columns["n_disvsls"].ExtendedProperties.Add("precision", "20,9");
            table.Columns["n_disvund"].ExtendedProperties.Add("precision", "1,0");
            table.Columns["n_netva"].ExtendedProperties.Add("precision", "20,4");
            table.Columns["n_taxva"].ExtendedProperties.Add("precision", "1,0");
            table.Columns["n_meterai"].ExtendedProperties.Add("precision", "1,0");
            table.Columns["n_bilva"].ExtendedProperties.Add("precision", "20,4");
            table.Columns["c_print"].MaxLength = 1;
            table.Columns["c_notax"].MaxLength = 1;
            table.Columns["c_cancel"].MaxLength = 1;
            table.Columns["c_expdisi"].MaxLength = 1;
            table.Columns["c_userid"].MaxLength = 1;
            table.Columns["c_areacode"].MaxLength = 1;
            table.Columns["c_rayon"].MaxLength = 1;
            table.Columns["c_stt"].MaxLength = 1;
            table.Columns["d_extgl"].ExtendedProperties.Add("datetype", "1");
            table.Columns["c_exnotax"].MaxLength = 1;
            table.Columns["c_cusno2"].MaxLength = 1;
            table.Columns["c_corty2"].MaxLength = 1;
            table.Columns["c_colno"].MaxLength = 1;
            table.Columns["c_rynco"].MaxLength = 1;
            table.Columns["c_nosp"].MaxLength = 1;
            table.Columns["n_cetak"].MaxLength = 1;
            table.Columns["c_inkano"].MaxLength = 1;
            table.Columns["c_rynka"].MaxLength = 1;
            table.Columns["c_pesan"].MaxLength = 1;
            table.Columns["n_fee"].ExtendedProperties.Add("precision", "1,0");
            table.Columns["n_topcus"].ExtendedProperties.Add("precision", "1,0");
            table.Columns["c_nmjbt"].MaxLength = 1;
            table.Columns["c_nmpsn"].MaxLength = 1;
            table.Columns["m_catatan"].MaxLength = 1;
            table.Columns["c_nodo"].MaxLength = 1;
            table.Columns["c_oldinv"].MaxLength = 1;
            table.Columns["c_pin"].MaxLength = 10;
            table.Columns["n_exdisc"].ExtendedProperties.Add("precision", "1,0");
            table.Columns["c_from"].MaxLength = 2;


            dataset.Tables.Add(table);

            #endregion

            if (lstAremas.Count > 0)
            {
                #region RETCUSD

                var qryrccusd = (from q in db.LG_RCHes
                                 join q1 in db.LG_RCD1s on new { q.c_rcno, q.c_gdg } equals new { q1.c_rcno, q1.c_gdg }
                                 join q2 in db.LG_RCD3s on new { q1.c_rcno, q1.c_iteno } equals new { q2.c_rcno, q2.c_iteno }
                                 where q.c_cusno == noCab
                                 && q.c_rcno == RcNo
                                 && ((q.l_send.HasValue ? q.l_send.Value : false) == true)
                                 && ((q.l_sent.HasValue ? q.l_sent.Value : false) == true)
                                 && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                 select new
                                 {
                                     q.c_rcno,
                                     q1.c_iteno,
                                     q2.n_salpri,
                                     q2.n_disc,
                                     q1.c_type,
                                     q1.n_qtyrcv,
                                     q.c_gdg
                                 }).AsQueryable();

                lstRCRetcusd = (from q in qryrccusd
                                group q by new { q.c_gdg, q.c_rcno, q.c_iteno, q.n_salpri, q.n_disc, q.c_type } into g
                                select new RCRetcusdSenderFormat
                                {
                                    c_invno = g.Key.c_rcno.Substring(3, Math.Min(7, g.Key.c_rcno.Length)),
                                    c_kddivams = string.Empty,
                                    c_iteno = g.Key.c_iteno,
                                    n_qtysalg = g.Key.c_type == "01" ? g.Sum(d => d.n_qtyrcv) : 0,
                                    n_qtysalb = g.Key.c_type == "02" ? g.Sum(d => d.n_qtyrcv) : 0,
                                    n_qtybong = 0,
                                    n_qtybonb = 0,
                                    n_salpri = g.Key.n_salpri,
                                    n_discams = g.Key.n_disc,
                                    n_discpri = 0,
                                    c_kddivpri = string.Empty,
                                    n_discpst = 0,
                                    n_ldiscpst = 0,
                                    n_ldiscams = 0,
                                    n_ldiscpri = 0,
                                    n_meterai = 0,
                                }).ToList();


                table = lstRCRetcusd.CopyToDataTableObject();
                table.TableName = DEFAULT_NAME_TABEL_RC_RETCUSD;

                //set column format
                table.Columns["c_invno"].MaxLength = 7;
                table.Columns["c_kddivams"].MaxLength = 1;
                table.Columns["c_iteno"].MaxLength = 4;
                table.Columns["n_qtysalg"].ExtendedProperties.Add("precision", "20,2");
                table.Columns["n_qtysalb"].ExtendedProperties.Add("precision", "20,2");
                table.Columns["n_qtybong"].ExtendedProperties.Add("precision", "1,0");
                table.Columns["n_qtybonb"].ExtendedProperties.Add("precision", "1,0");
                table.Columns["n_salpri"].ExtendedProperties.Add("precision", "20,2");
                table.Columns["n_discams"].ExtendedProperties.Add("precision", "7,2");
                table.Columns["n_discpri"].ExtendedProperties.Add("precision", "1,0");
                table.Columns["c_kddivpri"].MaxLength = 1;
                table.Columns["n_discpst"].ExtendedProperties.Add("precision", "1,0");
                table.Columns["n_ldiscpst"].ExtendedProperties.Add("precision", "1,0");
                table.Columns["n_ldiscams"].ExtendedProperties.Add("precision", "1,0");
                table.Columns["n_ldiscpri"].ExtendedProperties.Add("precision", "1,0");
                table.Columns["n_meterai"].ExtendedProperties.Add("precision", "1,0");


                dataset.Tables.Add(table);

                #endregion

                #region RETCUSBC

                var qryrccusbc = (from q in db.LG_RCHes
                                  join q1 in db.LG_RCD1s on new { q.c_rcno, q.c_gdg } equals new { q1.c_rcno, q1.c_gdg }
                                  join q2 in db.LG_MsBatches on new { q1.c_iteno, q1.c_batch } equals new { q2.c_iteno, q2.c_batch }
                                  where q.c_cusno == noCab
                                     && q.c_rcno == RcNo
                                     && ((q.l_send.HasValue ? q.l_send.Value : false) == true)
                                     && ((q.l_sent.HasValue ? q.l_sent.Value : false) == true)
                                     && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                  select new
                                  {
                                      q.c_rcno,
                                      q1.c_iteno,
                                      q1.c_batch,
                                      q2.d_expired,
                                      q1.c_type,
                                      q1.n_qtyrcv,
                                      q.c_gdg
                                  }).AsQueryable();

                lstRCRetcusbc = (from q in qryrccusbc
                                 group q by new { q.c_gdg, q.c_rcno, q.c_iteno, q.c_batch, q.d_expired, q.c_type } into g
                                 select new RCRetcusbcSenderFormat
                                 {
                                     c_invno = g.Key.c_rcno.Substring(3, Math.Min(7, g.Key.c_rcno.Length)),
                                     c_iteno = g.Key.c_iteno,
                                     c_batch = g.Key.c_batch,
                                     n_qty = g.Key.c_type == "01" ? g.Sum(x => x.n_qtyrcv) : 0,
                                     n_qtyr = g.Key.c_type == "02" ? g.Sum(x => x.n_qtyrcv) : 0,
                                     d_expired = g.Key.d_expired
                                 }).ToList();


                table = lstRCRetcusbc.CopyToDataTableObject();
                table.TableName = DEFAULT_NAME_TABEL_RC_RETCUSBC;

                //set column format
                table.Columns["c_invno"].MaxLength = 7;
                table.Columns["c_iteno"].MaxLength = 4;
                table.Columns["c_batch"].MaxLength = 15;
                table.Columns["n_qty"].ExtendedProperties.Add("precision", "20,2");
                table.Columns["n_qtyr"].ExtendedProperties.Add("precision", "20,2");
                table.Columns["d_expired"].ExtendedProperties.Add("datetype", "1");

                dataset.Tables.Add(table);



                #endregion

                #region All RC Sended

                table = new System.Data.DataTable(DEFAULT_NAME_TABEL_RC_LIST);

                table.Columns.Add("RC", typeof(string));

                table.BeginLoadData();

                for (int nLoop = 0, nLen = lstAremas.Count; nLoop < nLen; nLoop++)
                {
                    table.LoadDataRow(new object[] { string.Concat("RC", lstAremas[nLoop].c_invno) }, true);
                }

                table.EndLoadData();

                dataset.Tables.Add(table);

                #endregion

                lstAremas.Clear();
                lstRCRetcusd.Clear();
                lstRCRetcusbc.Clear();
            }

            return dataset;
        }

        private Dictionary<string, string> PrintPageMultiple(List<string> listParentKeys, System.Data.DataRow rowSupInfo, System.Data.DataTable tableHeader, System.Data.DataTable tableDetail)
        {
            if ((rowSupInfo == null) || (tableHeader == null) || (tableDetail == null))
            {
                return null;
            }

            StringBuilder sbHeader = new StringBuilder();
            StringBuilder sbHeaderData = new StringBuilder();
            StringBuilder sbHeaderColumn = new StringBuilder();
            StringBuilder sbListData = new StringBuilder();
            StringBuilder sbFooter = new StringBuilder();
            StringBuilder sbFooterData = new StringBuilder();
            StringBuilder sb = new StringBuilder();

            Dictionary<string, string> dataPrinting = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            int widthPage = 80;

            int hdrPading = 40,
              colProd = 36,
              colKemasan = 13,
              colQty = 10,
              colTotal = 21,
              footApj = 28,
              footAcc = 26,
              footLog = 26;

            int nLoop = 0,
              nLen = 0,
              nLoopC = 0,
              nLenC = 0,
              nIdx = 0;

            System.Data.DataRow rowHdr = null,
              rowDtl = null;

            System.Data.DataView viewDtl = null,
              viewHdr = null;

            string tmp = null,
              primaryId = null;

            decimal decQty = 0, decHna = 0,
              calcQtyHna = 0,
              sumCalcQtyHna = 0;

            #region Header

            tmp = "PT ANTARMITRA SEMBADA (PUSAT)";
            sbHeader.AppendLine(string.Concat(tmp.PadRight(hdrPading), "Kepada Yth,"));
            tmp = "JL. POS PENGUMBEN RAYA NO.8";
            sbHeader.AppendLine(string.Concat(tmp.PadRight(hdrPading), "NUTRINDO JAYA"));
            tmp = "KEBON JERUK, JAKARTA 11560";
            sbHeader.AppendLine(string.Concat(tmp.PadRight(hdrPading), "Ruko Taman Kebon Jeruk"));
            tmp = "JAKARTA";
            sbHeader.AppendLine(string.Concat(tmp.PadRight(hdrPading), "Meruya Utara,Kembangan"));

            sbHeader.AppendLine();

            sbHeader.AppendLine(PadLeftRight("P U R C H A S E  O R D E R", widthPage, ' ', ' '));

            #endregion

            #region Column Header

            tmp = "=";
            sbHeaderColumn.AppendLine(tmp.PadRight(widthPage, '='));
            tmp = "Product";
            sbHeaderColumn.Append(PadLeftRight(tmp, colProd, ' ', ' '));
            tmp = "Kemasan";
            sbHeaderColumn.Append(PadLeftRight(tmp, colKemasan, ' ', ' '));
            tmp = "Jumlah";
            sbHeaderColumn.Append(PadLeftRight(tmp, colQty, ' ', ' '));
            tmp = "Rp.";
            sbHeaderColumn.AppendLine(PadLeftRight(tmp, colTotal, ' ', ' '));
            tmp = "=";
            sbHeaderColumn.AppendLine(tmp.PadRight(widthPage, '='));

            #endregion

            #region Footer

            tmp = "Penerima Pesanan";
            sbFooter.Append(PadLeftRight(tmp, footApj, ' ', ' '));
            tmp = "Penanggung Jawab";
            sbFooter.Append(PadLeftRight(tmp, footAcc, ' ', ' '));
            tmp = "Logistik";
            sbFooter.AppendLine(PadLeftRight(tmp, footLog, ' ', ' '));

            sbFooter.AppendLine();
            sbFooter.AppendLine();
            sbFooter.AppendLine();

            tmp = "(Nama & Stempel)";
            sbFooter.Append(PadLeftRight(tmp, footAcc, ' ', ' '));
            tmp = "M. Fathan Nugraha";
            sbFooter.Append(PadLeftRight(tmp, footApj, ' ', ' '));
            tmp = "Niken Priscilia";
            sbFooter.AppendLine(PadLeftRight(tmp, footLog, ' ', ' '));

            tmp = "";
            sbFooter.Append(PadLeftRight(tmp, footAcc, ' ', ' '));
            tmp = "SP.KP.01.03.1.3.1191";
            sbFooter.Append(PadLeftRight(tmp, footApj, ' ', ' '));

            #endregion

            viewHdr = tableHeader.DefaultView;
            viewHdr.Sort = "C_CORNO";

            //for (nLoop = 0, nLen = tableHeader.Rows.Count; nLoop < nLen; nLoop++)
            for (nLoop = 0, nLen = listParentKeys.Count; nLoop < nLen; nLoop++)
            {
                primaryId = listParentKeys[nLoop];

                nIdx = viewHdr.Find(primaryId);

                if (nIdx != -1)
                {
                    rowHdr = viewHdr[nIdx].Row;

                    #region Header Data

                    tmp = primaryId;
                    sbHeaderData.Append(string.Format("NO : {0}", tmp.PadRight(16)));
                    tmp = rowHdr.GetValue<DateTime>("D_CORDA", ScmsSoaLibrary.Commons.Functionals.StandardSqlDateTime).ToString("dd-MM-yyyy");
                    sbHeaderData.AppendLine(string.Format("TGL : {0}", tmp));
                    tmp = rowHdr.GetValue<string>("C_NMCAB", string.Empty);
                    sbHeaderData.AppendLine((string.IsNullOrEmpty(tmp) ? null : string.Concat("PESANAN DI KIRIM KE ", tmp)));

                    #endregion

                    #region Populate Data

                    sumCalcQtyHna = 0;

                    viewDtl = new System.Data.DataView(tableDetail, string.Format("c_corno = '{0}'", primaryId), null, System.Data.DataViewRowState.CurrentRows);

                    for (nLoopC = 0, nLenC = viewDtl.Count; nLoopC < nLenC; nLoopC++)
                    {
                        rowDtl = viewDtl[nLoopC].Row;

                        tmp = rowDtl.GetValue<string>("c_itnam", string.Empty);
                        sbListData.Append(tmp.PadRight(colProd));
                        tmp = rowDtl.GetValue<string>("c_undes", string.Empty);
                        sbListData.Append(tmp.PadRight(colKemasan));

                        decQty = rowDtl.GetValue<decimal>("n_qty", 0);
                        sbListData.Append(decQty.ToString("N2").PadLeft(colQty));

                        decHna = rowDtl.GetValue<decimal>("n_salpri", 0);
                        calcQtyHna = (decQty * decHna);
                        sbListData.AppendLine(calcQtyHna.ToString("N2").PadLeft(colTotal));

                        sumCalcQtyHna += calcQtyHna;
                    }

                    viewDtl.Dispose();

                    #endregion

                    #region Footer Data

                    tmp = "=";
                    sbFooterData.AppendLine(tmp.PadRight(widthPage, '='));
                    tmp = "TOTAL : ";
                    sbFooterData.Append(tmp.PadLeft(60));
                    //tmp = "119,316,000";
                    tmp = sumCalcQtyHna.ToString("N2");
                    sbFooterData.AppendLine(tmp.PadLeft(20));
                    tmp = "=";
                    sbFooterData.AppendLine(tmp.PadRight(widthPage, '='));

                    #endregion

                    #region Combine

                    sb.Append(sbHeader.ToString());

                    sb.AppendLine();

                    sb.Append(sbHeaderData.ToString());
                    sb.Append(sbHeaderColumn.ToString());

                    sb.AppendLine();

                    sb.Append(sbListData.ToString());

                    sb.AppendLine();

                    sb.Append(sbFooterData.ToString());
                    sb.Append(sbFooter.ToString());

                    #endregion

                    if (!dataPrinting.ContainsKey(primaryId))
                    {
                        dataPrinting.Add(primaryId, sb.ToString());
                    }

                    #region Clear

                    sbHeaderData.Remove(0, sbHeaderData.Length);
                    sbListData.Remove(0, sbListData.Length);
                    sbFooterData.Remove(0, sbFooterData.Length);
                    sb.Remove(0, sb.Length);

                    #endregion
                }
            }

            sbHeaderColumn.Remove(0, sbHeaderColumn.Length);
            sbHeader.Remove(0, sbHeader.Length);
            sbFooter.Remove(0, sbFooter.Length);

            return dataPrinting;
        }

        private Dictionary<string, string> PrintPage(System.Data.DataRow rowSupInfo, System.Data.DataTable tableHeader, System.Data.DataTable tableDetail)
        {
            if ((rowSupInfo == null) || (tableHeader == null) || (tableDetail == null))
            {
                return null;
            }

            StringBuilder sbHeader = new StringBuilder();
            StringBuilder sbHeaderData = new StringBuilder();
            StringBuilder sbHeaderColumn = new StringBuilder();
            StringBuilder sbListData = new StringBuilder();
            StringBuilder sbFooter = new StringBuilder();
            StringBuilder sbFooterData = new StringBuilder();
            StringBuilder sb = new StringBuilder();

            Dictionary<string, string> dataPrinting = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            int widthPage = 80;

            int hdrPading = 40,
              colProd = 36,
              colKemasan = 13,
              colQty = 10,
              colTotal = 21,
              footApj = 28,
              footAcc = 26,
              footLog = 26;

            int nLoop = 0,
              nLen = 0,
              nLoopC = 0,
              nLenC = 0;

            System.Data.DataRow rowHdr = null,
              rowDtl = null;

            System.Data.DataView viewDtl = null;

            string tmp = null,
              primaryId = null;

            decimal decQty = 0, decHna = 0,
              calcQtyHna = 0,
              sumCalcQtyHna = 0;

            #region Header

            tmp = "PT ANTARMITRA SEMBADA (PUSAT)";
            sbHeader.AppendLine(string.Concat(tmp.PadRight(hdrPading), "Kepada Yth,"));
            tmp = "JL. POS PENGUMBEN RAYA NO.8";
            sbHeader.AppendLine(string.Concat(tmp.PadRight(hdrPading), "NUTRINDO JAYA"));
            tmp = "KEBON JERUK, JAKARTA 11560";
            sbHeader.AppendLine(string.Concat(tmp.PadRight(hdrPading), "Ruko Taman Kebon Jeruk"));
            tmp = "JAKARTA";
            sbHeader.AppendLine(string.Concat(tmp.PadRight(hdrPading), "Meruya Utara,Kembangan"));

            sbHeader.AppendLine();

            sbHeader.AppendLine(PadLeftRight("P U R C H A S E  O R D E R", widthPage, ' ', ' '));

            #endregion

            #region Column Header

            tmp = "=";
            sbHeaderColumn.AppendLine(tmp.PadRight(widthPage, '='));
            tmp = "Product";
            sbHeaderColumn.Append(PadLeftRight(tmp, colProd, ' ', ' '));
            tmp = "Kemasan";
            sbHeaderColumn.Append(PadLeftRight(tmp, colKemasan, ' ', ' '));
            tmp = "Jumlah";
            sbHeaderColumn.Append(PadLeftRight(tmp, colQty, ' ', ' '));
            tmp = "Rp.";
            sbHeaderColumn.AppendLine(PadLeftRight(tmp, colTotal, ' ', ' '));
            tmp = "=";
            sbHeaderColumn.AppendLine(tmp.PadRight(widthPage, '='));

            #endregion

            #region Footer

            tmp = "Penerima Pesanan";
            sbFooter.Append(PadLeftRight(tmp, footApj, ' ', ' '));
            tmp = "Penanggung Jawab";
            sbFooter.Append(PadLeftRight(tmp, footAcc, ' ', ' '));
            tmp = "Logistik";
            sbFooter.AppendLine(PadLeftRight(tmp, footLog, ' ', ' '));

            sbFooter.AppendLine();
            sbFooter.AppendLine();
            sbFooter.AppendLine();

            tmp = "(Nama & Stempel)";
            sbFooter.Append(PadLeftRight(tmp, footAcc, ' ', ' '));
            tmp = "M. Fathan Nugraha";
            sbFooter.Append(PadLeftRight(tmp, footApj, ' ', ' '));
            tmp = "Niken Priscilia";
            sbFooter.AppendLine(PadLeftRight(tmp, footLog, ' ', ' '));

            tmp = "";
            sbFooter.Append(PadLeftRight(tmp, footAcc, ' ', ' '));
            tmp = "SP.KP.01.03.1.3.1191";
            sbFooter.Append(PadLeftRight(tmp, footApj, ' ', ' '));

            #endregion

            for (nLoop = 0, nLen = tableHeader.Rows.Count; nLoop < nLen; nLoop++)
            {
                rowHdr = tableHeader.Rows[nLoop];

                #region Header Data

                primaryId = tmp = rowHdr.GetValue<string>("C_CORNO", string.Empty);
                sbHeaderData.Append(string.Format("NO : {0}", tmp.PadRight(16)));
                tmp = rowHdr.GetValue<DateTime>("D_CORDA", ScmsSoaLibrary.Commons.Functionals.StandardSqlDateTime).ToString("dd-MM-yyyy");
                sbHeaderData.AppendLine(string.Format("TGL : {0}", tmp));
                tmp = rowHdr.GetValue<string>("C_NMCAB", string.Empty);
                sbHeaderData.AppendLine((string.IsNullOrEmpty(tmp) ? null : string.Concat("PESANAN DI KIRIM KE ", tmp)));

                #endregion

                #region Populate Data

                sumCalcQtyHna = 0;

                viewDtl = new System.Data.DataView(tableDetail, string.Format("c_corno = '{0}'", primaryId), null, System.Data.DataViewRowState.CurrentRows);

                for (nLoopC = 0, nLenC = viewDtl.Count; nLoopC < nLenC; nLoopC++)
                {
                    rowDtl = viewDtl[nLoopC].Row;

                    tmp = rowDtl.GetValue<string>("c_itnam", string.Empty);
                    sbListData.Append(tmp.PadRight(colProd));
                    tmp = rowDtl.GetValue<string>("c_undes", string.Empty);
                    sbListData.Append(tmp.PadRight(colKemasan));

                    decQty = rowDtl.GetValue<decimal>("n_qty", 0);
                    sbListData.Append(decQty.ToString("N2").PadLeft(colQty));

                    decHna = rowDtl.GetValue<decimal>("n_salpri", 0);
                    calcQtyHna = (decQty * decHna);
                    sbListData.AppendLine(calcQtyHna.ToString("N2").PadLeft(colTotal));

                    sumCalcQtyHna += calcQtyHna;
                }

                viewDtl.Dispose();

                #endregion

                #region Footer Data

                tmp = "=";
                sbFooterData.AppendLine(tmp.PadRight(widthPage, '='));
                tmp = "TOTAL : ";
                sbFooterData.Append(tmp.PadLeft(60));
                //tmp = "119,316,000";
                tmp = sumCalcQtyHna.ToString("N2");
                sbFooterData.AppendLine(tmp.PadLeft(20));
                tmp = "=";
                sbFooterData.AppendLine(tmp.PadRight(widthPage, '='));

                #endregion

                #region Combine

                sb.Append(sbHeader.ToString());

                sb.AppendLine();

                sb.Append(sbHeaderData.ToString());
                sb.Append(sbHeaderColumn.ToString());

                sb.AppendLine();

                sb.Append(sbListData.ToString());

                sb.AppendLine();

                sb.Append(sbFooterData.ToString());
                sb.Append(sbFooter.ToString());

                #endregion

                //listDataPrinted.Add(sb.ToString());
                if (!dataPrinting.ContainsKey(primaryId))
                {
                    dataPrinting.Add(primaryId, sb.ToString());
                }

                #region Clear

                sbHeader.Remove(0, sbHeader.Length);
                sbHeaderColumn.Remove(0, sbHeaderColumn.Length);
                sbListData.Remove(0, sbListData.Length);
                sbFooter.Remove(0, sbFooter.Length);
                sbFooterData.Remove(0, sbFooterData.Length);
                sb.Remove(0, sb.Length);

                #endregion
            }

            return dataPrinting;
        }

        private bool SendMailMultiple(System.Data.DataRow rowSuplInfo, Dictionary<string, System.IO.MemoryStream> dicMsAremas, Dictionary<string, System.IO.MemoryStream> dicMsRCRetcusd, Dictionary<string, System.IO.MemoryStream> dicMsRCRetcusbc, string pin)
        {
            if ((rowSuplInfo == null) || (dicMsAremas == null) || (dicMsAremas.Count < 1) || (dicMsRCRetcusd == null) || (dicMsRCRetcusd.Count < 1) || (dicMsRCRetcusbc == null) || (dicMsRCRetcusbc.Count < 1))
            {
                return false;
            }

            bool bOk = false;

            System.Net.Mail.MailMessage mail = null;
            System.Net.Mail.SmtpClient smtp = null;

            Encoding utf8 = Encoding.UTF8;

            int nLoop = 0, nLen = 0;
            int nLoop2 = 0, nLen2 = 0;

            string namaSupl = rowSuplInfo.GetValue<string>("NamaCabang", string.Empty),
              tmp = null, noRCcab = null;
            string[] emailUsers = rowSuplInfo.GetValue<string[]>("Emails", new string[0]);

            List<string> lstRCno = null;

            StringBuilder sb = new StringBuilder();

            //System.IO.MemoryStream msHeader,
            //  msDetail = null;


            //tes

            //System.IO.DirectoryInfo diSub = null;

            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(config.PathTempExtractMail + "RCSend");

            System.IO.FileStream fs = null;

            System.IO.StreamWriter sw = null;

            //System.Data.DataSet dataset = null;


            string fileloc = null,
              folderloc = null, folderlocdetail = null;

            folderloc = config.PathTempExtractMail; //System.IO.Path.Combine("temp\\", "RC");

            foreach (KeyValuePair<string, System.IO.MemoryStream> kvp in dicMsAremas)
            {
                folderlocdetail = System.IO.Path.Combine(config.PathTempExtractMail + "RCSend\\", kvp.Key);
                System.IO.Directory.CreateDirectory(folderlocdetail);

                fileloc = System.IO.Path.Combine(di.FullName, string.Concat(kvp.Key, "\\AREMAS.DBF"));

                fs = new System.IO.FileStream(fileloc, System.IO.FileMode.Create);
                kvp.Value.WriteTo(fs);
                fs.Close();
                fs.Dispose();
            }

            foreach (KeyValuePair<string, System.IO.MemoryStream> kvp in dicMsRCRetcusd)
            {
                System.IO.Directory.CreateDirectory(folderloc);

                fileloc = System.IO.Path.Combine(di.FullName, string.Concat(kvp.Key, "\\RETCUSD.DBF"));

                fs = new System.IO.FileStream(fileloc, System.IO.FileMode.Create);
                kvp.Value.WriteTo(fs);
                fs.Close();
                fs.Dispose();
            }

            foreach (KeyValuePair<string, System.IO.MemoryStream> kvp in dicMsRCRetcusbc)
            {
                System.IO.Directory.CreateDirectory(folderloc);

                fileloc = System.IO.Path.Combine(di.FullName, string.Concat(kvp.Key, "\\RETCUSBC.DBF"));

                fs = new System.IO.FileStream(fileloc, System.IO.FileMode.Create);
                kvp.Value.WriteTo(fs);
                fs.Close();
                fs.Dispose();
            }

            //foreach (KeyValuePair<string, System.IO.MemoryStream> kvp in dicMsRCRetcusd)
            //{
            //    if (di.Exists)
            //    {
            //        fileloc = System.IO.Path.Combine(di.FullName, string.Concat("RETCUSD", ".dbf"));

            //        fs = new System.IO.FileStream(fileloc, System.IO.FileMode.Create);
            //        kvp.Value.WriteTo(fs);
            //        fs.Close();
            //        fs.Dispose();
            //    }
            //}

            //foreach (KeyValuePair<string, System.IO.MemoryStream> kvp in dicMsRCRetcusbc)
            //{
            //    if (di.Exists)
            //    {
            //        fileloc = System.IO.Path.Combine(di.FullName, string.Concat("RETCUSBC", ".dbf"));

            //        fs = new System.IO.FileStream(fileloc, System.IO.FileMode.Create);
            //        kvp.Value.WriteTo(fs);
            //        fs.Close();
            //        fs.Dispose();
            //    }
            //}


            string RAR_PATH = config.PathRar;

            lstRCno = dicMsAremas.Keys.CollectionToList();

            for (int j = 0; j < lstRCno.Count; j++)
            {
                if (lstRCno.Count > 0)
                {
                    noRCcab = lstRCno[j];
                }

                //string cmdArgs = string.Format("A {0} {1}",
                //    "temp\\RC\\" + rarName[0] + ".rar", "temp\\RC\\" + rarName[0] + ".dbf" + " -ep");

                string cmdArgs = string.Format("a -Y -RR -S -EP -CFG -M5 {0} {1}",
                    di + "\\" + noRCcab + "\\" + noRCcab + ".rar", di + "\\" + noRCcab + "\\*.DBF");

                Process prs = new Process();
                prs.StartInfo.Arguments = cmdArgs;
                prs.StartInfo.FileName = RAR_PATH;
                //prs.StartInfo.FileName = "C:\RAR\RAR.exe";
                //prs.StartInfo.CreateNoWindow = false;
                prs.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //prs.StartInfo.UseShellExecute = false; 


                prs.Start();
                prs.WaitForExit();
            }

            for (int mailloop = 0; mailloop < lstRCno.Count; mailloop++)
            {

                if ((emailUsers != null) && (emailUsers.Length > 0))
                {
                    mail = new System.Net.Mail.MailMessage();
                    mail.From = new System.Net.Mail.MailAddress("scms@ams.co.id", "Supply Chain Management System");

                    mail.Subject = string.Concat("RC Cabang - ", namaSupl);

                    for (nLoop = 0, nLen = emailUsers.Length; nLoop < nLen; nLoop++)
                    {
                        string[] emailTo = emailUsers[nLoop].Split(';');
                        for (nLoop2 = 0, nLen2 = emailTo.Length; nLoop2 < nLen2; nLoop2++)
                        {
                            mail.To.Add(emailTo[nLoop2]);
                        }
                    }

                    sb.AppendLine();
                    sb.AppendLine();

                    sb.AppendLine("Pengiriman RC :" + noRCcab);
                    sb.AppendLine("PIN :" + pin);

                    System.Net.Mail.Attachment attachment;
                    attachment = new System.Net.Mail.Attachment(config.PathTempExtractMail + "RCSend\\" + lstRCno[mailloop] + "\\" + lstRCno[mailloop] + ".RAR");
                    mail.Attachments.Add(attachment);

                    sb.AppendLine();
                    sb.AppendLine();

                    sb.AppendLine("Mohon untuk diperiksa dan diproses.");

                    mail.Body = sb.ToString();

                    smtp = new System.Net.Mail.SmtpClient(SMTP_IP, SMTP_PORT);

                    try
                    {
                        //    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                        //    smtp.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                        smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new System.Net.NetworkCredential("scms@ams.co.id", "scms");

                        smtp.Send(mail);

                        mail.Dispose();
                        sb.Length = 0;

                        bOk = true;
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine(ex.Message);
                        Logger.WriteLine(ex.StackTrace);
                    }
                }
            }



            try
            {
                for (int j = 0; j < lstRCno.Count; j++)
                {
                    if (lstRCno.Count > 0)
                    {
                        noRCcab = lstRCno[j];
                        Directory.Delete(config.PathTempExtractMail + "RCSend\\" + noRCcab, true);
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
                Logger.WriteLine(ex.StackTrace);
            }

            if (lstRCno.Count > 0)
            {
                lstRCno.Clear();
            }

            return bOk;
        }

        private void UpdateAllToTableMultiple(ORMDataContext db, List<string> listParentKeys)
        {
            int nLoop = 0,
              nLen = 0;

            string rcNumber = null;

            List<string> lstRC = new List<string>();

            List<LG_RCH> lstRCH = null;

            for (nLoop = 0, nLen = listParentKeys.Count; nLoop < nLen; nLoop++)
            {
                rcNumber = string.Concat("RC1", listParentKeys[nLoop]);

                if (!string.IsNullOrEmpty(rcNumber))
                {
                    if (!lstRC.Contains(rcNumber))
                    {
                        lstRC.Add(rcNumber);
                    }
                }
            }

            if (lstRC.Count > 0)
            {
                lstRCH = (from q in db.LG_RCHes
                          where lstRC.Contains(q.c_rcno)
                            && ((q.l_send.HasValue ? q.l_send.Value : false) == true)
                          select q).Distinct().ToList();

                if (lstRCH != null)
                {
                    try
                    {
                        for (nLoop = 0, nLen = lstRCH.Count; nLoop < nLen; nLoop++)
                        {
                            lstRCH[nLoop].l_send = false;
                            lstRCH[nLoop].d_update = DateTime.Now;
                        }

                        db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine(ex.Message);
                        Logger.WriteLine(ex.StackTrace);
                    }
                    finally
                    {
                        lstRCH.Clear();
                    }
                }

                lstRC.Clear();
            }
        }

        #endregion

        #region Trigger Event

        private void TriggerClassNotify(string msg, ClassNotifyEventArgs.TypeEnum type)
        {
            if (ClassNotify != null)
            {
                ClassNotify(this, new ClassNotifyEventArgs(msg, type));
            }
        }

        #endregion

        #endregion

        #region Callback

        private void WoTCallback(object state, bool timeOut)
        {
            if (isRunning)
            {
                return;
            }

            isRunning = true;

            ScmsMailLibrary.Pop3MailerReader.TaskInfo ti = state as ScmsMailLibrary.Pop3MailerReader.TaskInfo;

            try
            {
                if (ti != null)
                {
                    if (isStart)
                    {
                        if (timeOut)
                        {
                            this.RunningLogicSender(this.TempPath);
                        }
                    }
                    else
                    {
                        if (ti.Handle != null)
                        {
                            //ti.Handle.Unregister(areEvent);
                            ti.Handle.Unregister(null);

                            if (areThreadStop != null)
                            {
                                areThreadStop.Set();
                            }
                        }
                    }
                }
            }
            catch (Win32Exception w)
            {
                Console.WriteLine(w.Message);
                Console.WriteLine(w.ErrorCode.ToString());
                Console.WriteLine(w.NativeErrorCode.ToString());
                Console.WriteLine(w.StackTrace);
                Console.WriteLine(w.Source);
                Exception e = w.GetBaseException();
                Console.WriteLine(e.Message);
            }

            //catch (Exception ex)
            //{
            //  this.TriggerClassNotify(
            //    string.Format("SendRCAuto.WoTCallback - {0}", ex.Message),
            //    ClassNotifyEventArgs.TypeEnum.IsError);

            //  Logger.WriteLine(ex.Message);
            //  Logger.WriteLine(ex.StackTrace);
            //}

            isRunning = false;
        }

        #endregion

        public string TempPath
        { get; private set; }

        public bool Start()
        {
            if (isStart)
            {
                this.TriggerClassNotify("RC Sender Auto sudah aktif.", ClassNotifyEventArgs.TypeEnum.IsNotify);

                return false;
            }

            try
            {
                ScmsMailLibrary.Pop3MailerReader.TaskInfo ti = new ScmsMailLibrary.Pop3MailerReader.TaskInfo();

                ti.Handle = System.Threading.ThreadPool.RegisterWaitForSingleObject(areEvent,
                  new WaitOrTimerCallback(WoTCallback), ti,
                  this.TimePeriodic, false);

                isStart = true;
            }
            catch (Exception ex)
            {
                this.TriggerClassNotify(
                  string.Format("SendRCAuto.Start - {0}", ex.Message),
                  ClassNotifyEventArgs.TypeEnum.IsError);

                Logger.WriteLine(ex.Message);
                Logger.WriteLine(ex.StackTrace);
            }

            return isStart;
        }

        public void Stop()
        {
            if (!isStart)
            {
                this.TriggerClassNotify("RC Sender Auto belum aktif.", ClassNotifyEventArgs.TypeEnum.IsNotify);

                return;
            }

            isStart = false;

            try
            {
                areThreadStop = new AutoResetEvent(false);

                if (areEvent != null)
                {
                    areEvent.Set();
                }

                areThreadStop.WaitOne();

                areThreadStop.Close();

                areThreadStop = null;

                areEvent.Reset();
            }
            catch (Exception ex)
            {
                this.TriggerClassNotify(
                  string.Format("SendRCAuto.Stop - {0}", ex.Message),
                  ClassNotifyEventArgs.TypeEnum.IsError);

                Logger.WriteLine(ex.Message);
                Logger.WriteLine(ex.StackTrace);
            }
        }

        public TimeSpan TimePeriodic
        { get; private set; }

        public string MonitoringFolder
        { get; private set; }

        public bool IsActiveBackupSendRC
        { get; private set; }

        public void Testing()
        {
            RunningLogicSender(this.TempPath);
        }
    }
}
