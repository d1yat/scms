using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsSoaLibrary.Commons;
using System.ServiceModel.Channels;
using ScmsModel;
using ScmsSoaLibraryInterface.Commons;
using System.Threading;
using ScmsSoaLibrary.Bussiness;
using ScmsSoaLibrary.Modules;
using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb;
using System.IO;
using ClosedXML.Excel;
//using System.Web;

namespace ScmsMailLibrary
{
    public class DOPharmanetReader
    {
        internal class DO_PO_Link
        {
            public string PO { get; set; }
            public string Item { get; set; }
            public decimal Sisa { get; set; }
        }

        internal class DO_PI_Header_Check
        {
            public string Principal { get; set; }
            public string DO { get; set; }
        }

        internal class DO_PI_Detail_Check
        {
            public string Principal { get; set; }
            public string DO { get; set; }
            public string PO { get; set; }
            public string Item { get; set; }
            public string Batch { get; set; }
        }

        internal class DO_PI_Result
        {
            public string Principal { get; set; }
            public string DO { get; set; }
            public string PO { get; set; }
            public string Item { get; set; }
            public string Batch { get; set; }
        }

        internal class SP_Link
        {
            public string SP { get; set; }
            public string Cusno { get; set; }
            public string OutletPO { get; set; }
            public string Outlet { get; set; }
        }

        internal class DOPhar_Link
        {
            public string NoSup { get; set; }
            public string DoNo { get; set; }
            public string Cab { get; set; }
            public string RnNo { get; set; }
            public string OutletPO { get; set; }
            public string PLPhar { get; set; }
        }

        internal class LG_PLD1_SUM_BYBATCH
        {
            public string c_plno { get; set; }
            public string c_iteno { get; set; }
            public string c_batch { get; set; }
            public decimal n_qty { get; set; }
        }

        internal class AddReDO
        {
            public string c_dono { get; set; }
            public string c_cusno { get; set; }
        }

        internal class DOInformation
        {
            public string ID { get; set; }
            public bool IsStt { get; set; }
        }

        internal class PROCESS_DO_TO_FJ
        {
            public string DoNo { get; set; }
            public string Via { get; set; }
            public string Item { get; set; }
            public decimal Qty { get; set; }
            public decimal Sisa { get; set; }
            public decimal Disc { get; set; }
            public decimal DiscOff { get; set; }
            public decimal HNA { get; set; }
            public decimal Bonus { get; set; }
            public string DiscountCode { get; set; }
        }

        private const string DEFAULT_NAME_FILE_HEADER = "DOHEADER.DBF";
        private const string DEFAULT_NAME_FILE_DETAIL = "DODETAIL.DBF";

        private const string DEFAULT_NAME_TABEL_HEADER = "DOHEADER";
        private const string DEFAULT_NAME_TABEL_DETAIL = "DODETAIL";

        private Pop3MailerReader pop3;
        private ScmsSoaLibrary.Commons.Config config;

        private AutoResetEvent areEvent;
        private AutoResetEvent areThreadStop;

        private bool isStart;

        private volatile bool isRunning;

        private Dictionary<string, string> dicMappingPrinsipal;

        public DOPharmanetReader(ScmsSoaLibrary.Commons.Config config)
        {
            this.config = config;

            dicMappingPrinsipal = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            dicMappingPrinsipal.Add("PT. NUTRISAINS", "00113");
            dicMappingPrinsipal.Add("PT. PRIMA MEDIKA LABORATORIES", "00117");
            dicMappingPrinsipal.Add("PT. NUTRINDO JAYA ABADI", "00112");
            dicMappingPrinsipal.Add("PT. PHAROS INDONESIA", "00001");
            dicMappingPrinsipal.Add("PT. APEX PHARMA INDONESIA", "00120");
            dicMappingPrinsipal.Add("PT. PERINTIS PELAYANAN PARIPURNA", "00159");
            dicMappingPrinsipal.Add("PT. ETERCON PHARMA", "00085");
            dicMappingPrinsipal.Add("PT. NOVELL PHARMACEUTICAL LABORATORIES", "00019");
            dicMappingPrinsipal.Add("CHC", "00159");
            dicMappingPrinsipal.Add("CFU", "00165");
            dicMappingPrinsipal.Add("PT. SARUA SUBUR", "00171");

            if ((config == null) || config.TimerPeriodicMailer.Equals(TimeSpan.MinValue))
            {
                this.TimePeriodic = new TimeSpan(0, 3, 0);
            }
            else
            {
                this.TimePeriodic = config.TimerPeriodicMailer;
            }

            string tmp = null,
              dir = null;

            if ((config == null) || string.IsNullOrEmpty(config.PathTempExtractMail))
            {
                tmp = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                tmp = System.IO.Path.Combine(config.PathTempExtractMail, "DOPharGet");

                tmp = (tmp.EndsWith("\\") ? tmp : string.Concat(tmp, "\\"));

                dir = tmp;
            }
            else
            {
                tmp = System.IO.Path.Combine(config.PathTempExtractMail, "DOPharGet");

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
                  string.Format("ScmsMailLibrary.DOPharmanetReader:DOPharmanetReader - {0}", ex.Message));
                Logger.WriteLine(ex.StackTrace);
            }

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

            areEvent = new AutoResetEvent(false);
        }

        ~DOPharmanetReader()
        {
            if (dicMappingPrinsipal != null)
            {
                dicMappingPrinsipal.Clear();
            }
        }

        public bool Start(string email, string pwd)
        {
            System.Net.IPEndPoint ep = config.POP3DOPharosEP as System.Net.IPEndPoint;

            if (ep == null)
            {
                ep = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 110);
            }

            if (!isStart)
            {
                StartingFolderReader();
            }

            return Start(ep, email, pwd);
        }

        public bool Start(System.Net.IPEndPoint epHost, string email, string pwd)
        {
            bool bOk = false;

            pop3 = new Pop3MailerReader(epHost.Address.ToString(), epHost.Port, this.config);

            bOk = pop3.Start(email, pwd, false);

            pop3.ClassNotify += new Pop3MailerReader.ClassNotifyEventHandler(pop3_ClassNotify);
            pop3.Pop3MailMessage += new Pop3MailerReader.Pop3MailMessageEventHandler(pop3_Pop3MailMessage);

            if (!isStart)
            {
                StartingFolderReader();
            }

            return bOk;
        }

        public void Stop()
        {
            pop3.Stop();

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
                Logger.WriteLine(
                  string.Format("ScmsMailLibrary.DOPharmanetReader:Stop - {0}", ex.Message));
                Logger.WriteLine(ex.StackTrace);
            }
        }

        public TimeSpan TimePeriodic
        { get; private set; }

        public string MonitoringFolder
        { get; private set; }

        public string TempPath
        { get; private set; }

        #region Event

        private void pop3_Pop3MailMessage(object sender, Pop3MailMessageEventArgs e)
        {
            if (!e.HasAttachment)
            {
                return;
            }

            System.Data.DataSet dataset = null;
            Pop3MailerReader pop = sender as Pop3MailerReader;
            ORMDataContext db = null;
            int nChanges = 0;

            //e.Delete = true;
            DateTime dt = DateTime.Now;
            string jam = dt.ToString("HH");
            //string jam = dt.ToString("HH:mm");
            if (jam == "08" || jam == "12" || jam == "15" || jam == "18" || jam == "22")
            //if (jam == "08:00" || jam == "15:00" || jam == "09:35")
            { 
                try
                {
                    db = new ORMDataContext(this.config.ConnectionString);

                    //pop.TempPath
                    if (!System.IO.Directory.Exists(pop.TempPath))
                    {
                        System.IO.Directory.CreateDirectory(pop.TempPath);
                    }

                    //SaveToTemplateAndExtract(pop.TempPath, 
                    //pathFile = System.IO.Path.Combine(pop.TempPath, e.Attachments);
                    foreach (KeyValuePair<string, byte[]> kvp in e.Attachments)
                    {
                        dataset = SaveToTemplateAndExtract(pop.TempPath, kvp.Value);

                        if ((dataset != null) && (dataset.Tables.Count == 2))
                        {
                            nChanges += insertintotemp(dataset, dicMappingPrinsipal, db);
                            //nChanges += PopulateDoHeaderDetailPI(dataset, dicMappingPrinsipal, db);
                        }

                        if (dataset != null)
                        {
                            dataset.Clear();
                            //dataset.Dispose();
                        }
                    }

                    if (nChanges > 0)
                    {
                        e.Delete = true;
                    }
                    else
                    {
                        e.Delete = false;
                    }

                    //if (nChanges > 0)
                    //{
                    //    db.SubmitChanges();
                    //}
                }
                catch (Exception ex)
                {
                    dataset = null;

                    Logger.WriteLine(ex.Message);
                    Logger.WriteLine(ex.StackTrace);
                }
                finally
                {

                    if (db != null)
                    {
                        db.Dispose();
                    }
                }
            }
        }

        private void pop3_ClassNotify(object sender, ScmsMailLibrary.Global.ClassNotifyEventArgs e)
        {
            switch (e.TypeMessage)
            {
                case ScmsMailLibrary.Global.ClassNotifyEventArgs.TypeEnum.IsError:
                    Logger.WriteLine("Error : {0}", e.Message);
                    break;
                case ScmsMailLibrary.Global.ClassNotifyEventArgs.TypeEnum.IsInformation:
                    Logger.WriteLine("Info : {0}", e.Message);
                    break;
                case ScmsMailLibrary.Global.ClassNotifyEventArgs.TypeEnum.IsNotify:
                    Logger.WriteLine("Notify : {0}", e.Message);
                    break;
                default:
                    Logger.WriteLine("-> : {0}", e.Message);
                    break;
            }
        }

        #endregion

        #region Private
        private const string SMTP_IP = "10.100.10.9";
        private const int SMTP_PORT = 25;

        private System.Data.DataSet SaveToTemplateAndExtract(string pathFolder, byte[] datas)
        {
            if (string.IsNullOrEmpty(pathFolder) || (datas == null) || (!System.IO.Directory.Exists(pathFolder)))
            {
                return null;
            }

            const int MAXIMUM_BUFFER = 4096;

            System.Data.DataSet dataSet = new System.Data.DataSet();
            System.Data.DataTable tableHeader = null;
            System.Data.DataTable tableDetail = null;

            int nExtract = 0;

            #region Zip Operation

            System.IO.FileStream fs = null;
            System.IO.MemoryStream ms = null;
            ICSharpCode.SharpZipLib.Zip.ZipFile zip = null;
            System.IO.Stream zipStream = null;
            string fName = null;
            byte[] buff = null;
            BufferManager bufMan = null;
            bool isOkToExtract = false;

            ms = new System.IO.MemoryStream();

            ms.Write(datas, 0, datas.Length);

            zip = new ICSharpCode.SharpZipLib.Zip.ZipFile(ms);

            bufMan = BufferManager.CreateBufferManager(long.MaxValue, 1024);

            foreach (ICSharpCode.SharpZipLib.Zip.ZipEntry ze in zip)
            {
                if (ze.IsFile)
                {
                    try
                    {
                        isOkToExtract = false;

                        if (ze.Name.Equals(DEFAULT_NAME_FILE_DETAIL, StringComparison.OrdinalIgnoreCase))
                        {
                            nExtract++;

                            isOkToExtract = true;
                        }
                        else if (ze.Name.Equals(DEFAULT_NAME_FILE_HEADER, StringComparison.OrdinalIgnoreCase))
                        {
                            nExtract++;

                            isOkToExtract = true;
                        }

                        if (isOkToExtract)
                        {
                            //buff = new byte[MAXIMUM_BUFFER];

                            buff = bufMan.TakeBuffer(MAXIMUM_BUFFER);

                            zipStream = zip.GetInputStream(ze);

                            fName = System.IO.Path.Combine(pathFolder, ze.Name);

                            if (System.IO.File.Exists(fName))
                            {
                                System.IO.File.Delete(fName);
                            }

                            fs = new System.IO.FileStream(fName, System.IO.FileMode.Create);

                            ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(zipStream, fs, buff);
                        }
                    }
                    catch (Exception ex)
                    {
                        nExtract--;

                        Logger.WriteLine(ex.Message);
                        Logger.WriteLine(ex.StackTrace);
                    }
                    finally
                    {
                        if (fs != null)
                        {
                            fs.Close();
                            fs.Dispose();
                        }

                        if (zipStream != null)
                        {
                            zipStream.Close();
                            zipStream.Dispose();
                        }

                        if (buff != null)
                        {
                            //Array.Clear(buff, 0, MAXIMUM_BUFFER);
                            bufMan.ReturnBuffer(buff);
                            buff = null;
                        }
                    }
                }
            }

            bufMan.Clear();

            zip.Close();

            ms.Close();
            ms.Dispose();

            #endregion

            if (nExtract == 2)
            {
                fName = System.IO.Path.Combine(pathFolder, DEFAULT_NAME_FILE_HEADER);
                tableHeader = ScmsMailLibrary.Core.Commons.ReadDbfDatabase(fName, DEFAULT_NAME_TABEL_HEADER);

                fName = System.IO.Path.Combine(pathFolder, DEFAULT_NAME_FILE_DETAIL);
                tableDetail = ScmsMailLibrary.Core.Commons.ReadDbfDatabase(fName, DEFAULT_NAME_TABEL_DETAIL);

                dataSet.Tables.Add(tableHeader);
                dataSet.Tables.Add(tableDetail);
            }

            return dataSet;
        }

        //penambahan sistem pharmanet yang baru by suwandi 28 September 2017

        private int insertintotemp(System.Data.DataSet dataset, Dictionary<string, string> DicMappingPrinsipalGroup, ScmsModel.ORMDataContext db)
        {
            int nChanges = 0;
            if ((dataset == null) || (dataset.Tables.Count != 2) || (DicMappingPrinsipalGroup == null))
            {
                return 0;
            }

            #region declare
            Temp_LG_DOPH Temp_DOPH = null;
            List<Temp_LG_DOPH> List_Temp_DOPH = null;
            Temp_LG_DOPD Temp_DOPD = null;
            List<Temp_LG_DOPD> List_Temp_DOPD = null;
            string namaPrinsipal = null, kodePrinsipal = null, kdcab = null, dcorecab = null, POOUTLETDEL = null;

            #region declare header
            string sOutlet = null, sPoOutlet = null, sPlphar = null, NoDO = null, NoInv = null, tmp = null, 
                via = null, Tax = null, vOutlet = null, SPPharID = null, NoPLReject = null,cusno = null;
            DateTime TglDO = DateTime.Now, TglInv = DateTime.Now, SpDate = DateTime.Now, PoDate = DateTime.Now;
            #endregion

            #region declare detail
            string Iteno = null, cItenoPri = null, sItnam = null, sDesc = null, sBatch = null, sNosp = null, sNoDPL= null;
            DateTime dExp = DateTime.Now;
            decimal nDisc = 0, nClaim = 0, nSalpri = 0, nQty = 0, nQtybns = 0, nDpriOn = 0, nDpriOff = 0, nDamsOn = 0;
            #endregion

            #region "declare PO"
            
            string kodeSubPrincipal = null, namaSubPrincipal = null, alamatSubPrincipal = null, businessLicense = null, picApoteker = null, sik = null;
            DateTime? expBusinessLicense = null, expSik = null;

            Temp_LG_POPH Temp_POPH = null;
            List<Temp_LG_POPH> List_Temp_POPH = null;
            
            #endregion

            List_Temp_DOPH = new List<Temp_LG_DOPH>();
            List_Temp_DOPD = new List<Temp_LG_DOPD>();
            List_Temp_POPH = new List<Temp_LG_POPH>();

            System.Data.DataTable table = null, tableDetail = null;
            int nLoop = 0, nLen = 0;

            decimal nPoQtySisa = 0, nQtyDO = 0, nDamsOff = 0;
            System.Data.DataRow row = null;
            #endregion

            var dbDOPH = new ORMDataContext(this.config.ConnectionString);
            try
            {
                if (dbDOPH.Transaction == null)
                {
                    dbDOPH.Connection.Open();
                    dbDOPH.Transaction = dbDOPH.Connection.BeginTransaction();
                }
                if (dataset.Tables.Contains(DEFAULT_NAME_TABEL_HEADER))
                {
                    table = dataset.Tables[DEFAULT_NAME_TABEL_HEADER];
                    if (dataset.Tables.Contains(DEFAULT_NAME_TABEL_DETAIL))
                    {
                        tableDetail = dataset.Tables[DEFAULT_NAME_TABEL_DETAIL];
                    }

                    #region insert header
                    for (nLoop = 0, nLen = table.Rows.Count; nLoop < nLen; nLoop++)
                    {
                        row = table.Rows[nLoop];
                        
                        namaPrinsipal = row.GetValue<string>("C_PT", string.Empty).Trim();

                        if (DicMappingPrinsipalGroup.ContainsKey(namaPrinsipal))
                        {
                            kodePrinsipal = DicMappingPrinsipalGroup[namaPrinsipal];
                        }
                        else
                        {
                            kodePrinsipal = "00000";
                        }
                        tmp = row.GetValue<string>("C_VIA", string.Empty).Trim();
                        if (tmp == null)
                        {
                            via = "00";
                        }
                        else if (tmp.Equals("D", StringComparison.OrdinalIgnoreCase))
                        {
                            via = "02";
                        }
                        else if (tmp.Equals("U", StringComparison.OrdinalIgnoreCase))
                        {
                            via = "01";
                        }
                        else
                        {
                            via = "03";
                        }

                        sOutlet = row.GetValue<string>("C_OUTLET").Trim();
                        sPoOutlet = row.GetValue<string>("C_PO_OUTLE", string.Empty).Trim();
                        sPlphar = row.GetValue<string>("N_NOPL", string.Empty).Trim();
                        NoDO = row.GetValue<string>("C_NODO", string.Empty).Trim();
                        TglDO = row.GetValue<DateTime>("D_TGLDO", DateTime.Now);
                        PoDate = row.GetValue<DateTime>("D_TGLPO", DateTime.Now);
                        SpDate = TglDO.AddDays(-10);
                        NoInv = row.GetValue<string>("C_EXNOINV", string.Empty).Trim();
                        TglInv = row.GetValue<DateTime>("D_JTH", DateTime.Now);
                        Tax = row.GetValue<string>("C_NOTAX", string.Empty).Trim();
                        vOutlet = row.GetValue<string>("", string.Empty).Trim();
                        kdcab = row.GetValue<string>("c_kdcab", string.Empty);
                        /* Enhance PO */
                        kodeSubPrincipal = row.GetValue<string>("v_kdsub_pr", string.Empty);
                        namaSubPrincipal = row.GetValue<string>("v_nmsub_pr", string.Empty);
                        alamatSubPrincipal = row.GetValue<string>("t_alamt_pr", string.Empty);
                        businessLicense = row.GetValue<string>("v_bns_lcse", string.Empty);
                        expBusinessLicense = row.GetValue<DateTime?>("d_exp_bnsl", DateTime.Now);
                        picApoteker = row.GetValue<string>("v_pic_apot", string.Empty);
                        sik = row.GetValue<string>("v_sik", string.Empty);
                        expSik = row.GetValue<DateTime?>("d_exp_sik", DateTime.Now);
                        /* Enhance PO */
                        if (kdcab == "")
                        {
                            goto skip;
                        }
                        SPPharID = Commons2.GenerateNumbering<LG_SPH>(dbDOPH, "SPP", '4', row.GetValue<string>("c_kdcab", string.Empty), SpDate, "c_sp");
                        cusno = (from q in dbDOPH.LG_CusmasCabs
                                 where q.c_cab == row.GetValue<string>("c_kdcab", string.Empty)
                                 select q.c_cusno).SingleOrDefault();

                        dcorecab = (from q in dbDOPH.LG_CusmasCabs
                                 where q.c_cab == row.GetValue<string>("c_kdcab", string.Empty)
                                 select q.c_cab_dcore).Take(1).SingleOrDefault();
                        SPPharID = SPPharID.Substring(0, 3) + dcorecab + SPPharID.Substring(3, 10);

                        var cek = (from q in dbDOPH.Temp_LG_DOPHs
                                   where q.c_po_outlet == sPoOutlet
                                   select q.c_po_outlet).SingleOrDefault();
                        if (cek == null)
                        {
                            if (sPoOutlet != null)
                            {
                                if (cusno != null)
                                {
                                    Temp_DOPH = new Temp_LG_DOPH()
                                    {
                                        c_nosup = kodePrinsipal,
                                        c_dono = NoDO,
                                        d_dodate = TglDO,
                                        d_fjno = NoInv,
                                        d_fjdate = TglInv,
                                        l_status = "True",
                                        c_cab = kdcab,
                                        c_via = via,
                                        c_taxno = Tax,
                                        d_entry = DateTime.Now,
                                        c_po_outlet = sPoOutlet,
                                        d_tglpo = PoDate,
                                        c_outlet = sOutlet,
                                        v_outlet = vOutlet,
                                        c_plphar = sPlphar,
                                        c_type = "SPPHAR2",
                                        Status = "10",
                                        v_ket = "",
                                        c_spphar = SPPharID,
                                        c_cusno = cusno
                                    };
                                    List_Temp_DOPH.Add(Temp_DOPH);

                                    /* Enhance PO */
                                    Temp_POPH = new Temp_LG_POPH()
                                    {
                                        c_po_outlet = sPoOutlet,
                                        c_plphar = sPlphar,
                                        c_spphar = SPPharID,
                                        v_kdsub_prn = kodeSubPrincipal,
                                        v_nmsub_prn = namaSubPrincipal,
                                        t_alamatsub_prn = alamatSubPrincipal,
                                        v_bsn_license = businessLicense,
                                        d_exp_bsn_license = expBusinessLicense,
                                        v_pic_apoteker = picApoteker,
                                        v_sik = sik,
                                        d_exp_sik = expSik,
                                        d_entry = DateTime.Now
                                    };
                                    List_Temp_POPH.Add(Temp_POPH);
                                    /* Enhance PO */
                                }
                            }
                        }
                        else
                        {
                            if (NoPLReject == null)
                            {
                                NoPLReject = sPlphar;
                            }
                            else
                            {
                                NoPLReject = NoPLReject + "," + sPlphar;
                            }
                        }
                    skip:
                        nChanges += 1;

                    }
                    #endregion

                    #region insert detail
                    for (nLoop = 0, nLen = tableDetail.Rows.Count; nLoop < nLen; nLoop++)
                    {
                        row = tableDetail.Rows[nLoop];

                        namaPrinsipal = row.GetValue<string>("C_PT", string.Empty).Trim();

                        if (DicMappingPrinsipalGroup.ContainsKey(namaPrinsipal))
                        {
                            kodePrinsipal = DicMappingPrinsipalGroup[namaPrinsipal];
                        }
                        else
                        {
                            kodePrinsipal = "00000";
                        }
                        NoDO = row.GetValue<string>("C_NODO", string.Empty).Trim();
                        Iteno = row.GetValue<string>("C_ITENO", string.Empty).Trim();
                        cItenoPri = row.GetValue<string>("C_ITENOPRI", string.Empty).Trim();
                        var CekItenoPri = (from q in db.FA_MasItms where q.c_itenopri == cItenoPri && q.d_nie > DateTime.Now
                                               select q).AsQueryable();
                        if (CekItenoPri.Count() == 0)
                        {
                            if (POOUTLETDEL == string.Empty)
                            {
                                POOUTLETDEL = row.GetValue<string>("C_PO_OUTLE", string.Empty).Trim();
                            }
                            else
                            {
                                POOUTLETDEL = POOUTLETDEL + "," + row.GetValue<string>("C_PO_OUTLE", string.Empty).Trim();
                            }
                            goto Miss;
                        }
                        sItnam = row.GetValue<string>("C_ITNAM", string.Empty).Trim();
                        sDesc = row.GetValue<string>("C_UNDES", string.Empty).Trim();
                        nQty = row.GetValue<decimal>("N_QTYRCV", 0);
                        sBatch = row.GetValue<string>("C_BATCH", string.Empty).Trim();
                        dExp = row.GetValue<DateTime>("D_EXPIRED", DateTime.Now);
                        sNosp = row.GetValue<string>("C_NOSP", string.Empty).Trim();
                        nDisc = row.GetValue<decimal>("N_DISC", 0);
                        nClaim = row.GetValue<decimal>("L_CLAIMBNS", 0);
                        sPoOutlet = row.GetValue<string>("C_PO_OUTLE", string.Empty).Trim();
                        nSalpri = row.GetValue<decimal>("N_SALPRI", 0);
                        nQtybns = row.GetValue<decimal>("N_QTYBNS", 0);
                        nDpriOn = row.GetValue<decimal>("N_DPRION", 0);
                        nDpriOff = row.GetValue<decimal>("N_DPRIOFF", 0);
                        nDamsOn = row.GetValue<decimal>("N_DAMSON", 0);
                        nDamsOff = row.GetValue<decimal>("N_DAMSOFF", 0);
                        sNoDPL = row.GetValue<string>("C_NODPL", string.Empty).Trim();
                        var cek = (from q in dbDOPH.Temp_LG_DOPDs where q.c_po_outlet == sPoOutlet && q.c_itenopri == cItenoPri && q.c_batch == sBatch
                                   select q.c_po_outlet).SingleOrDefault();
                        if (cek == null)
                        {
                            Temp_DOPD = new Temp_LG_DOPD
                            {
                                c_nosup = kodePrinsipal,
                                c_dono = NoDO,
                                c_iteno = Iteno,
                                c_type = "01",
                                c_itenopri = cItenoPri,
                                v_itnam = sItnam,
                                v_undes = sDesc,
                                c_pono = "",
                                n_qty = nQty,
                                c_batch = sBatch,
                                d_expired = dExp,
                                c_nosp = sNosp,
                                n_disc = nDisc,
                                l_claim = null,
                                d_entry = DateTime.Now,
                                l_pending = null,
                                n_qty_sisa = 0,
                                l_done = false,
                                n_qtybns = nQtybns,
                                n_dprion = nDpriOn,
                                n_dprioff = nDpriOff,
                                n_damson = nDamsOn,
                                n_damsoff = nDamsOff,
                                c_nodpl = sNoDPL,
                                c_po_outlet = sPoOutlet,
                                c_outlet = "",
                                n_salpri = nSalpri,
                                n_qtyterima = 0,
                                v_ket = ""
                            };
                            List_Temp_DOPD.Add(Temp_DOPD);
                        }
                        Miss:
                            nChanges += 1;
                    }
                    #endregion

                    if (List_Temp_DOPH.Count > 0)
                    {
                        dbDOPH.Temp_LG_DOPHs.InsertAllOnSubmit(List_Temp_DOPH.ToArray());
                        List_Temp_DOPH.Clear();
                    }
                    if (List_Temp_DOPD.Count > 0)
                    {
                        dbDOPH.Temp_LG_DOPDs.InsertAllOnSubmit(List_Temp_DOPD.ToArray());
                        List_Temp_DOPD.Clear();
                    }
                    /* Enhance PO */
                    if (List_Temp_POPH.Count > 0)
                    {
                        dbDOPH.Temp_LG_POPHs.InsertAllOnSubmit(List_Temp_POPH.ToArray());
                        List_Temp_POPH.Clear();
                    }
                    /* Enhance PO */

                    dbDOPH.SubmitChanges();
                    dbDOPH.Transaction.Commit();
                    if (POOUTLETDEL != null)
                    {
                        string[] PODEL = POOUTLETDEL.Split(','); 

                    }

                    #region Cek Data PL
                    //for (nLoop = 0, nLen = table.Rows.Count; nLoop < nLen; nLoop++)
                    //{
                    //    row = table.Rows[nLoop];

                    //    sPoOutlet = row.GetValue<string>("C_PO_OUTLE", string.Empty).Trim();

                    //    decimal totalharga = 0;

                    //    var cek = (from q in dbDOPH.Temp_LG_DOPDs
                    //               where q.c_po_outlet == sPoOutlet
                    //               select q.n_qtyterima  ).SingleOrDefault();


                    //}
                    #endregion

                    dbDOPH.Dispose();

                    if (NoPLReject != null)
                    {
                        System.Net.Mail.SmtpClient smtp = null;
                        StringBuilder sb = new StringBuilder();
                        try
                        {
                            using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                            {
                                // send mail containing the file here

                                mail.From = new System.Net.Mail.MailAddress("scms.sph@ams.co.id", "Supply Chain Management System");

                                mail.Subject = "Laporan Data Pharmanet yang di reject";

                                //mail.To.Add("hafizh.ahmad@ams.co.id");
                                //mail.To.Add("suwandi@ams.co.id");
                                //mail.To.Add("indra.dwi@ams.co.id");
                                mail.To.Add("teddy@ams.co.id");
                                mail.To.Add("enik@ams.co.id");
                                mail.To.Add("noval@ams.co.id");
                                mail.CC.Add("dudy.budiman@ams.co.id");
                                mail.CC.Add("akhirudin.sudiyat@ams.co.id");
                                sb.AppendLine("Dear Team Pharmanet,");
                                sb.AppendLine("");
                                sb.AppendLine("Mohon di periksa kembali Data PL dibawah ini karena PL ini sudah pernah di terima di AMS");
                                sb.AppendLine(NoPLReject);
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
                    }
                    
                }
            }
            catch (Exception ex)
            {
                if (dbDOPH.Transaction != null)
                {
                    dbDOPH.Transaction.Rollback();
                    nChanges = 0;
                }
            }

            return nChanges;
        }

        //penambahan sistem pharmanet yang baru by suwandi 28 September 2017

        //pharmanet sistem lama - tidak di pakai lagi
        
        private int PopulateDoHeaderDetailPI(System.Data.DataSet dataset, Dictionary<string, string> dicMappingPrinsipalGroup, ScmsModel.ORMDataContext db)
        {
            if ((dataset == null) || (dataset.Tables.Count != 2) || (dicMappingPrinsipalGroup == null))
            {
                return 0;
            }
            #region declare
            const string PENDING_PO_NAME = "(PENDING)";

            LG_SPH sph = null;
            List<LG_SPH> ListSPH = null;
            LG_SPD1 spd1 = null;
            List<LG_SPD1> ListSPD1 = null;
            LG_SPD2 spd2 = null;
            List<LG_SPD2> ListSPD2 = null;

            LG_POH poh = null;
            List<LG_POH> ListPOH = null;
            LG_POD1 pod1 = new LG_POD1(); ;
            List<LG_POD1> ListPOD1 = null;
            LG_POD2 pod2 = null;
            List<LG_POD2> ListPOD2 = null;

            List<LG_DOPH> lstDOPH = null;
            LG_DOPH doph = null;
            List<LG_DOPD> lstDOPD = null;
            LG_DOPD dopd = null;
            //List<LG_DOPH> lstDOPHRem = null;
            //List<LG_DOPD> lstDOPDRem = null;

            List<LG_DOPHErr1> lstDOPHErrDel1 = null;
            List<LG_DOPHErr1> lstDOPHErr1 = null;
            LG_DOPHErr1 dophErr1 = null;

            List<LG_DOPDErr1> lstDOPDErrDel1 = null;
            List<LG_DOPDErr1> lstDOPDErr1 = null;
            LG_DOPDErr1 dopdErr1 = null;

            List<LG_DOPHErr2> lstDOPHErr2 = null;
            LG_DOPHErr2 dophErr2 = null;

            List<LG_DOPDErr2> lstDOPDErr2 = null;
            LG_DOPDErr2 dopdErr2 = null;

            List<LG_RNH> listRnh = null;
            List<LG_RND1> listRnd1 = null;
            List<LG_RND2> listRnd2 = null;

            List<LG_DOH> listDOH = null;
            List<AddReDO> listReDo = null;
            List<AddReDO> listReDoGroup = null;

            LG_DOH doh = null;
            LG_DOH doh2 = null;

            FA_MasItm itm = null;

            LG_FJH fjh = null;
            List<LG_FJD1> lstFjd1 = null;
            List<LG_FJD2> lstFjd2 = null;
            LG_FJD3 fjd3 = null;

            int nChanges = 0;
            System.Data.DataTable table = null,
              tableDetail = null;
            string namaPrinsipal = null,
              tmp = null,
              kodePrinsipal = null,
              poData = null,
              kode_item = null,
              nipEntry = null,
              via = null,
              tmpCustomer = null,
              plID = null,
              doID = null,
              noDpl = null,
              spID = null,
              poID = null,
              sOutlet = null,
              sPoOutlet = null,
              SingleCabang = null,
                //Nosup = null,
              sXdiscNo = null,
              rnIDTemp = null,
              plIDTemp = null,
              plIDAutoTemp = null,
              doIDTemp = null,
              SPPharID = null,
              kdcab = null,
                //poIDTemp = null,
              rnID = null;
            //rnIDTemp = null;
            var nomorDO = new List<string>();
            
            string strCommand = string.Empty;
            System.Data.DataRow row = null;
            System.Data.DataRow[] rowCols = null;
            DateTime date = DateTime.Today,
            spDate = DateTime.Now.AddDays(-10),
            dateToday = DateTime.Today,
            poDate = DateTime.Now.AddDays(-3);

            
            string itemlist = null;
            string itemPriceList = null;
            string itemPrice = null;
            string[] poList = null;
            DateTime dateNow = DateTime.Now,
                dateError = DateTime.Now;
            bool isPending = false;

            int nLoop = 0,
              nLen = 0,
              nLoopC = 0,
              nLenC = 0,
              nLoopCi = 0,
              nLenCi = 0,
              nLoopRh = 0,
              nLoopRd = 0,
              nLoopPh = 0,
              nLoopPd = 0,
              nLoopFj = 0;

            decimal nPoQtySisa = 0,
              nQtyDO = 0,
              nQtybns = 0,
              nDpriOn = 0,
              nDpriOff = 0,
              nDamsOn = 0,
              nDamsOff = 0,
              nSalpri = 0;

            List<DO_PO_Link> lstPoLink = null;
            DO_PO_Link dol = default(DO_PO_Link);

            List<SP_Link> lstSpLink = null;
            List<DOPhar_Link> lstDoPharLink = null;

            SP_Link spl = default(SP_Link);

            List<DO_PI_Header_Check> lstDOPHeaderCheck = null;
            List<DO_PI_Detail_Check> lstDOPDetailCheck = null;
            List<DO_PI_Result> lstDOPResult = null;

            itm = new FA_MasItm();
            #endregion
            try
            {
                if (db.Transaction == null)
                {
                    db.Connection.Open();

                    db.Transaction = db.Connection.BeginTransaction();
                }


                if (dataset.Tables.Contains(DEFAULT_NAME_TABEL_HEADER))
                {
                    table = dataset.Tables[DEFAULT_NAME_TABEL_HEADER];

                    if (dataset.Tables.Contains(DEFAULT_NAME_TABEL_DETAIL))
                    {
                        tableDetail = dataset.Tables[DEFAULT_NAME_TABEL_DETAIL];
                    }

                    lstDOPH = new List<LG_DOPH>();
                    lstDOPD = new List<LG_DOPD>();

                    lstDOPHErr1 = new List<LG_DOPHErr1>();
                    lstDOPHErr2 = new List<LG_DOPHErr2>();
                    lstDOPHErrDel1 = new List<LG_DOPHErr1>();

                    lstDOPDErr1 = new List<LG_DOPDErr1>();
                    lstDOPDErr2 = new List<LG_DOPDErr2>();
                    lstDOPDErrDel1 = new List<LG_DOPDErr1>();

                    #region Populate Data
                    lstDOPHErrDel1 = (from q in db.LG_DOPHErr1s
                                      select q).ToList();

                    lstDOPDErrDel1 = (from q in db.LG_DOPDErr1s
                                      select q).ToList();

                    for (nLoop = 0, nLen = table.Rows.Count; nLoop < nLen; nLoop++)
                    {
                        row = table.Rows[nLoop];

                        namaPrinsipal = row.GetValue<string>("C_PT", string.Empty).Trim();

                        if (dicMappingPrinsipalGroup.ContainsKey(namaPrinsipal))
                        {
                            kodePrinsipal = dicMappingPrinsipalGroup[namaPrinsipal];
                        }
                        else
                        {
                            kodePrinsipal = "00000";
                        }

                        if (kodePrinsipal == "00165" || kodePrinsipal == "00159")
                        {
                            nipEntry = "SPPHAR2";
                        }
                        else
                        {
                            nipEntry = "SPPHAR1";
                        }


                        var dbSpPo = new ORMDataContext(this.config.ConnectionString);

                        try
                        {
                            if (dbSpPo.Transaction == null)
                            {
                                dbSpPo.Connection.Open();

                                dbSpPo.Transaction = dbSpPo.Connection.BeginTransaction();
                            }

                            ListSPH = new List<LG_SPH>();
                            ListPOH = new List<LG_POH>();
                            ListPOD2 = new List<LG_POD2>();

                            #region "SP Header & Purchase Order"
                            //tableH = dataset.Tables["H"];

                            SysNo sysNum = (from q in dbSpPo.SysNos
                                            where q.c_portal == '3' && q.c_type == "43"
                                              && q.s_tahun == DateTime.Now.Year
                                            select q).SingleOrDefault();

                            spID = Commons2.GenerateNumbering<LG_SPH>(dbSpPo, "SF", '3', "43", spDate, "c_spno");

                            SysNo sysNumPO = (from q in dbSpPo.SysNos
                                              where q.c_portal == '3' && q.c_type == "44"
                                                && q.s_tahun == DateTime.Now.Year
                                              select q).SingleOrDefault();

                            poID = Commons2.GenerateNumbering<LG_POH>(dbSpPo, "PF", '3', "44", poDate, "c_pono");

                            //SysNo sysNumSPP = (from q in dbSpPo.SysNos
                            //                       where q.c_portal == '3' && q.c_type == "43" 
                            //                       && q.s_tahun == DateTime.Now.Year select q).SingleOrDefault();

                            SPPharID = Commons2.GenerateNumbering<LG_SPH>(dbSpPo, "SPP", '3', row.GetValue<string>("c_kdcab",string.Empty), spDate, "c_sp"); 

                            kdcab = (from q in dbSpPo.LG_CusmasCabs
                                         where q.c_cab == row.GetValue<string>("c_kdcab", string.Empty)
                                         select q.c_cab_dcore).Take(1).SingleOrDefault();
                            SPPharID = SPPharID.Substring(0, 3) + kdcab + SPPharID.Substring(3, 10);

                            //for (nLoop = 0; nLoop < tableH.Rows.Count; nLoop++)
                            //{
                            //row = tableH.Rows[nLoop];

                            //spCab = row.GetValue<string>("c_nosp").ToString();
                            //sOutlet = row.GetValue<string>("C_OUTLET").ToString().Substring(2, 4);
                            sOutlet = row.GetValue<string>("C_OUTLET").Trim();
                            sPoOutlet = row.GetValue<string>("C_PO_OUTLE", string.Empty).Trim();

                            //kd = row.GetValue<string>("c_kdcab", string.Empty);

                            SingleCabang = (from q in dbSpPo.LG_Cusmas
                                            where q.c_cab == row.GetValue<string>("c_kdcab", string.Empty)
                                            select q.c_cusno).Take(1).SingleOrDefault();

                            //spDate = DateTime.Parse(row.ItemArray[2].ToString());
                            //spDate = DateTime.Today;

                            sph = new LG_SPH()
                            {
                                c_sp = "SPPHAR",
                                c_type = "06",
                                c_cusno = SingleCabang,
                                c_entry = Constant.SYSTEM_USERNAME,
                                c_spno = spID,
                                c_update = Constant.SYSTEM_USERNAME,
                                d_entry = DateTime.Now,
                                d_spdate = spDate,
                                d_spinsert = DateTime.Now,
                                d_update = DateTime.Now,
                                l_cek = true,
                                l_delete = false,
                                l_print = false,
                                v_ket = "SP Pharmanet",
                                c_outlet = sOutlet,
                                c_po_outlet = sPoOutlet,
                                c_spphar = SPPharID
                            };

                            ListSPH.Add(sph);

                            //nm = row.GetValue<string>("c_supplier", string.Empty);

                            //Nosup = (from q in dbSpPo.LG_DatSups
                            //         where q.v_namatax == row.GetValue<string>("C_PT", string.Empty)
                            //         select q.c_nosup).Take(1).SingleOrDefault();

                            poh = new LG_POH()
                            {
                                c_gdg = '1',
                                c_pono = poID,
                                //d_podate = DateTime.Now.Date,
                                //d_podate = spDate,
                                d_podate = poDate,
                                c_nosup = kodePrinsipal,
                                l_import = false,
                                c_kurs = "01",
                                n_kurs = 1,
                                v_ket = "PHARMANET",
                                n_bruto = 0,
                                n_disc = 0,
                                n_pdisc = 0,
                                n_xdisc = 0,
                                n_ppn = 0,
                                n_bilva = 0,
                                l_print = false,
                                l_send = true,
                                l_revisi = true,
                                c_entry = Constant.SYSTEM_USERNAME,
                                d_entry = DateTime.Now,
                                c_update = Constant.SYSTEM_USERNAME,
                                d_update = DateTime.Now,
                                c_type = "01"
                            };

                            ListPOH.Add(poh);

                            pod2 = new LG_POD2()
                            {
                                c_gdg = '1',
                                c_pono = poID,
                                c_orno = spID
                            };
                             
                            ListPOD2.Add(pod2);
                            //}
                            #endregion

                            #region DOPH & Check Header

                            var isEmpty = (from q in dbSpPo.LG_DOPHs
                                           where q.c_dono == row.GetValue<string>("C_NODO", string.Empty)
                                                && q.c_nosup == kodePrinsipal
                                                && q.c_cab != "HO"
                                           select q.c_dono).Take(1).SingleOrDefault();

                            if (!string.IsNullOrEmpty(isEmpty))
                            {
                                dbSpPo.Transaction.Rollback();
                                continue;
                            }

                            date = row.GetValue<DateTime>("D_JTH", DateTime.MinValue);
                            if (date.Equals(DateTime.MinValue))
                            {
                                tmp = row.GetValue<string>("D_JTH", string.Empty);
                                if (!Functionals.DateParser(tmp, "M/d/yyyy HH:mm:ss", out date))
                                {
                                    date = Functionals.StandardSqlDateTime;
                                }
                            }

                            tmp = row.GetValue<string>("C_VIA", string.Empty).Trim();
                            if (tmp == null)
                            {
                                via = "00";
                            }
                            else if (tmp.Equals("D", StringComparison.OrdinalIgnoreCase))
                            {
                                via = "02";
                            }
                            else if (tmp.Equals("U", StringComparison.OrdinalIgnoreCase))
                            {
                                via = "01";
                            }
                            else
                            {
                                via = "03";
                            }

                            #region Insert DO not Valid

                            string c_outlet = row.GetValue<string>("C_OUTLET", string.Empty);
                            if (!string.IsNullOrEmpty(c_outlet))
                            {
                                var isError = (from q in dbSpPo.SCMS_MSOUTLET_CABANGs
                                               where q.c_cab == c_outlet.Substring(0, 2)
                                                    && q.c_outlet == c_outlet.Substring(2, 4)
                                               select q.c_cab).Take(1).SingleOrDefault();

                                if (string.IsNullOrEmpty(isError))
                                {
                                    var dateDO = row.GetValue<DateTime>("D_TGLDO", DateTime.MinValue);
                                    if (dateDO.Equals(DateTime.MinValue))
                                    {
                                        tmp = row.GetValue<string>("D_TGLDO", string.Empty);
                                        if (!Functionals.DateParser(tmp, "M/d/yyyy HH:mm:ss", out dateDO))
                                        {
                                            dateDO = Functionals.StandardSqlDateTime;
                                        }
                                    }

                                    dophErr1 = new LG_DOPHErr1()
                                    {
                                        c_nosup = kodePrinsipal,
                                        c_dono = row.GetValue<string>("C_NODO", string.Empty),
                                        d_dodate = dateDO,
                                        d_fjno = row.GetValue<string>("C_EXNOINV", string.Empty),
                                        d_fjdate = date,
                                        l_status = true,
                                        c_cab = row.GetValue<string>("C_KDCAB", string.Empty),
                                        c_via = row.GetValue<string>("C_VIA", string.Empty).Trim(),
                                        c_taxno = row.GetValue<string>("C_NOTAX", string.Empty),
                                        d_entry = dateNow,
                                        c_plphar = row.GetValue<string>("N_NOPL", string.Empty),
                                        c_po_outlet = sPoOutlet,
                                        c_ket = "Outlet Not Valid",
                                        c_outlet = c_outlet,
                                        v_outlet = row.GetValue<string>("V_OUTLET", string.Empty)
                                    };

                                    lstDOPHErr1.Add(dophErr1);

                                    dophErr2 = (from q in db.LG_DOPHErr2s
                                                where q.c_dono == row.GetValue<string>("C_NODO", string.Empty)
                                                     && q.c_nosup == kodePrinsipal
                                                     && q.c_po_outlet == sPoOutlet
                                                select q).SingleOrDefault();

                                    if (dophErr2 == null)
                                    {
                                        dophErr2 = new LG_DOPHErr2()
                                        {
                                            c_nosup = kodePrinsipal,
                                            c_dono = row.GetValue<string>("C_NODO", string.Empty),
                                            d_dodate = dateDO,
                                            d_fjno = row.GetValue<string>("C_EXNOINV", string.Empty),
                                            d_fjdate = date,
                                            l_status = true,
                                            c_cab = row.GetValue<string>("C_KDCAB", string.Empty),
                                            c_via = via,
                                            c_taxno = row.GetValue<string>("C_NOTAX", string.Empty),
                                            d_entry = dateNow,
                                            c_plphar = row.GetValue<string>("N_NOPL", string.Empty),
                                            c_po_outlet = sPoOutlet,
                                            c_ket = "Outlet Not Valid",
                                            c_outlet = c_outlet,
                                            v_outlet = row.GetValue<string>("V_OUTLET", string.Empty)
                                        };

                                        lstDOPHErr2.Add(dophErr2);
                                    }
                                    else
                                    {
                                        dophErr2.c_po_outlet = sPoOutlet;
                                        dophErr2.c_cab = row.GetValue<string>("C_KDCAB", string.Empty);
                                        dophErr2.c_outlet = c_outlet;
                                        dophErr2.v_outlet = row.GetValue<string>("V_OUTLET", string.Empty);
                                        dophErr2.d_update = dateNow;
                                    }

                                    dbSpPo.Transaction.Rollback();
                                    continue;
                                }
                            }
                            #endregion

                            doph = new LG_DOPH()
                            {
                                c_nosup = kodePrinsipal,
                                c_dono = row.GetValue<string>("C_NODO", string.Empty),
                                d_dodate = dateToday,
                                d_fjno = row.GetValue<string>("C_EXNOINV", string.Empty),
                                d_fjdate = date,
                                l_status = true,
                                c_cab = row.GetValue<string>("C_KDCAB", string.Empty),
                                c_via = via,
                                c_taxno = row.GetValue<string>("C_NOTAX", string.Empty),
                                d_entry = dateNow,
                                c_plphar = row.GetValue<string>("N_NOPL", string.Empty),
                                c_po_outlet = sPoOutlet
                            };

                            #endregion

                            if (tableDetail != null)
                            {
                                ListSPD1 = new List<LG_SPD1>();
                                ListSPD2 = new List<LG_SPD2>();
                                ListPOD1 = new List<LG_POD1>();

                                rowCols = tableDetail.Select(string.Format("C_PT = '{0}' And C_NODO = '{1}' And C_PO_OUTLE = '{2}' ",
                                  namaPrinsipal, doph.c_dono, sPoOutlet), "C_ITNAM");

                                if ((rowCols != null) && (rowCols.Length > 0))
                                {
                                    for (nLoopC = 0, nLenC = rowCols.Length; nLoopC < nLenC; nLoopC++)
                                    {
                                        row = rowCols[nLoopC];

                                        kode_item = row.GetValue<string>("C_ITENO", string.Empty).Trim();
                                        string noPric = row.GetValue<string>("C_ITENOPRI", string.Empty).Trim();
                                        string sItnam = row.GetValue<string>("C_ITNAM", string.Empty).Trim();
                                        string itemPricePri = row.GetValue<string>("N_SALPRI", string.Empty).Trim();
                                        //if (kode_item != "4706")
                                        //{
                                        if (string.IsNullOrEmpty(kode_item))
                                        {
                                            kode_item = (from q in db.FA_MasItms
                                                         where q.c_itenopri == noPric && q.l_aktif == true && q.c_nosup != "00001"
                                                         select q.c_iteno).Take(1).SingleOrDefault();
                                            //add by suwandi 03 oktober 2017
                                            if (kode_item == null)
                                            {
                                                
                                                if (itemlist == null)
                                                {
                                                    itemlist = noPric + " - " + sItnam + " - " + sPoOutlet;
                                                }
                                                else
                                                {
                                                    itemlist = itemlist + "\n" + noPric + " - " + sItnam + " - " + sPoOutlet;
                                                }

                                            }

                                            itemPrice = (from q in db.FA_MasItms
                                                             where q.c_itenopri == noPric && q.l_aktif == true && q.c_nosup == kodePrinsipal
                                                             select q.n_salpri).ToString();

                                            if (itemPrice != itemPricePri)
                                            {
                                                if (itemPriceList == null)
                                                {
                                                    itemPriceList = sPoOutlet + " - " + itemPricePri + " - " + itemPrice;
                                                }
                                                else
                                                { 
                                                    itemPriceList = itemPriceList + " - " + sPoOutlet + " - " + itemPricePri + " - " + itemPrice;
                                                }
                                            }

                                        }
                                        else
                                        {
                                            var sup = (from q in db.FA_MasItms
                                                       where q.c_iteno == kode_item && q.l_aktif == true
                                                       select q.c_nosup.Trim()).Take(1).SingleOrDefault();

                                            if (sup != kodePrinsipal)
                                            {
                                                kode_item = (from q in db.FA_MasItms
                                                             where q.c_itenopri == noPric
                                                                && q.c_nosup == kodePrinsipal && q.l_aktif == true
                                                             select q.c_iteno).Take(1).SingleOrDefault();
                                            }
                                        }
                                        //}

                                        //if (noPric == "013038" )
                                        //{
                                        //    kode_item = "6309";
                                        //}

                                        if (string.IsNullOrEmpty(kode_item))
                                        {
                                            var c_iteno = row.GetValue<string>("C_ITENO", string.Empty).Trim();
                                            if (string.IsNullOrEmpty(c_iteno))
                                            {
                                                kode_item = (from q in db.LG_MsItmPhars
                                                             where q.c_itenopri == noPric
                                                             select q.c_iteno).Take(1).SingleOrDefault();
                                            }
                                            else
                                            {
                                                kode_item = (from q in db.LG_MsItmPhars
                                                             where q.c_iteno == c_iteno
                                                             select q.c_iteno).Take(1).SingleOrDefault();
                                            }
                                        }
                                        //poData = row.GetValue<string>("C_NOSP", string.Empty).Replace(" ", "").Trim();
                                        poData = poID;
                                        nQtyDO = row.GetValue<decimal>("N_QTYRCV");
                                        nQtybns = row.GetValue<decimal>("N_QTYBNS");
                                        nDpriOn = row.GetValue<decimal>("N_DPRION");
                                        nDpriOff = row.GetValue<decimal>("N_DPRIOFF");
                                        nDamsOn = row.GetValue<decimal>("N_DAMSON");
                                        nDamsOff = row.GetValue<decimal>("N_DAMSOFF");
                                        noDpl = row.GetValue<string>("C_NODPF", string.Empty).Replace(" ", "").Trim();

                                        if (nQtyDO <= 0.00m)
                                        {
                                            continue;
                                        }

                                        //if (string.IsNullOrEmpty(kode_item) && string.IsNullOrEmpty(row.GetValue<string>("C_ITENO", string.Empty).Trim()))
                                        if (string.IsNullOrEmpty(kode_item))
                                        {
                                            continue;
                                        }

                                        #region "SP Detail & PO Detail"
                                        //tableD = dataset.Tables["D"];

                                        //for (nLoop = 0; nLoop < tableD.Rows.Count; nLoop++)
                                        //{
                                        //    row = tableD.Rows[nLoop];

                                        //nQty = row.GetValue<int>("n_qty", 0);

                                        if (nQtyDO > 0)
                                        {
                                            //kdItem = row.GetValue<string>("c_iteno", string.Empty).Trim();

                                            nSalpri = (from q in dbSpPo.FA_MasItms
                                                       where q.c_iteno == kode_item
                                                       select
                                                         q.n_salpri.HasValue ? q.n_salpri.Value : 0
                                                          ).Take(1).SingleOrDefault();


                                            var Disc = (from q in dbSpPo.FA_DiscDs
                                                        where q.c_iteno == kode_item && q.c_nodisc == "DSXXXXXX03"
                                                        select q).SingleOrDefault();

                                            if (Disc != null)
                                            {
                                                //if (nDpriOn <= 0)
                                                //{
                                                //    nDpriOn = (Disc.n_discon.HasValue ? Disc.n_discon.Value : 0);
                                                //}
                                                //if (nDpriOff <= 0)
                                                //{
                                                //    nDpriOff = (Disc.n_discoff.HasValue ? Disc.n_discoff.Value : 0);
                                                //}
                                                sXdiscNo = (Disc.c_nodisc == null ? string.Empty : Disc.c_nodisc);
                                            }
                                            else
                                            {
                                                sXdiscNo = string.Empty;
                                            }


                                            spd1 = ListSPD1.Find(delegate(LG_SPD1 sp)
                                            {
                                                return spID.Equals((string.IsNullOrEmpty(sp.c_spno) ? string.Empty : sp.c_spno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                  kode_item.Equals((string.IsNullOrEmpty(sp.c_iteno) ? string.Empty : sp.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase);
                                            });

                                            if (spd1 == null)
                                            {
                                                ListSPD1.Add(new LG_SPD1()
                                                {
                                                    c_iteno = kode_item,
                                                    c_spno = spID,
                                                    c_type = "01",
                                                    n_acc = nQtyDO,
                                                    n_qty = nQtyDO,
                                                    n_sisa = nQtyDO,
                                                    n_salpri = nSalpri,
                                                    v_ket = ""
                                                });

                                                ListSPD2.Add(new LG_SPD2()
                                                {
                                                    c_iteno = kode_item,
                                                    c_spno = spID,
                                                    c_type = "03",
                                                    c_no = sXdiscNo,
                                                    n_discoff = nDpriOff,
                                                    n_discon = nDpriOn
                                                });

                                                ListPOD1.Add(new LG_POD1()
                                                {
                                                    c_gdg = '1',
                                                    c_pono = poID,
                                                    c_iteno = kode_item,
                                                    n_qty = nQtyDO,
                                                    n_disc = nDpriOn,
                                                    n_salpri = nSalpri,
                                                    n_sisa = 0
                                                });
                                            }
                                            else
                                            {
                                                ListSPD1[ListSPD1.Count - 1].n_qty += nQtyDO;
                                                ListSPD1[ListSPD1.Count - 1].n_acc += nQtyDO;
                                                ListSPD1[ListSPD1.Count - 1].n_sisa += nQtyDO;
                                                ListPOD1[ListPOD1.Count - 1].n_qty += nQtyDO;
                                            }
                                        }

                                        //}
                                        #endregion

                                        #region Default Detail DOPD

                                        dopd = new LG_DOPD()
                                        {
                                            c_nosup = kodePrinsipal,
                                            c_dono = row.GetValue<string>("C_NODO", string.Empty),
                                            //c_iteno = string.IsNullOrEmpty(row.GetValue<string>("C_ITENO", string.Empty).Trim()) ? kode_item : row.GetValue<string>("C_ITENO", string.Empty),
                                            c_iteno = kode_item,
                                            c_itenopri = row.GetValue<string>("C_ITENOPRI", string.Empty),
                                            v_itnam = row.GetValue<string>("C_ITNAM", string.Empty),
                                            v_undes = row.GetValue<string>("C_UNDES", string.Empty),
                                            n_qty = nQtyDO,
                                            n_qty_sisa = 0,
                                            c_batch = row.GetValue<string>("C_BATCH", string.Empty),
                                            d_expired = null,
                                            c_pono = null,
                                            n_disc = row.GetValue<decimal>("N_DISC"),
                                            l_claim = row.GetValue<bool>("L_CLAIMBNS"),
                                            c_type = "01",
                                            d_entry = dateNow,
                                            l_done = false,
                                            l_pending = false,
                                            n_qtybns = nQtybns,
                                            n_dprion = nDpriOn,
                                            n_dprioff = nDpriOff,
                                            n_damson = nDamsOn,
                                            n_damsoff = nDamsOff,
                                            c_nodpl = noDpl,
                                            c_outlet = sOutlet,
                                            c_po_outlet = sPoOutlet
                                        };

                                        #endregion

                                        date = row.GetValue<DateTime>("D_EXPIRED", DateTime.MinValue);
                                        if (date.Equals(DateTime.MinValue))
                                        {
                                            tmp = row.GetValue<string>("D_EXPIRED", string.Empty);
                                            if (!Functionals.DateParser(tmp, "M/d/yyyy HH:mm:ss", out date))
                                            {
                                                date = Functionals.StandardSqlDateTime;
                                            }
                                        }
                                        dopd.d_expired = date;

                                        if (poData.Contains(","))
                                        {
                                            #region Multiple PO

                                            poList = poData.Split(',');

                                            if (poData.IndexOf(PENDING_PO_NAME, StringComparison.OrdinalIgnoreCase) == -1)
                                            {
                                                isPending = true;
                                            }
                                            else
                                            {
                                                isPending = false;
                                            }

                                            lstPoLink = (from q in db.LG_POD1s
                                                         where poList.Contains(q.c_pono) && (q.c_iteno == dopd.c_iteno)
                                                         select new DO_PO_Link()
                                                         {
                                                             PO = q.c_pono,
                                                             Item = dopd.c_iteno,
                                                             Sisa = (q.n_sisa.HasValue ? q.n_sisa.Value : 0)
                                                         }).Distinct().ToList();

                                            for (nLoopCi = 0, nLenCi = poList.Length; nLoopCi < nLenCi; nLoopCi++)
                                            {
                                                tmp = poList[nLoopCi].Trim();

                                                dol = lstPoLink.Find(delegate(DO_PO_Link dopol)
                                                {
                                                    return (string.IsNullOrEmpty(dopol.PO) ? false : true);
                                                });

                                                if (dol == null)
                                                {
                                                    nPoQtySisa = (nQtyDO > dol.Sisa ?
                                                      (dol.Sisa - nQtyDO) : nQtyDO);
                                                }
                                                else
                                                {
                                                    nPoQtySisa = 0;
                                                }

                                                if (tmp.IndexOf(PENDING_PO_NAME, StringComparison.OrdinalIgnoreCase) == -1)
                                                {
                                                    isPending = false;
                                                }
                                                else
                                                {
                                                    tmp = tmp.Replace(PENDING_PO_NAME, "").Trim();
                                                    isPending = true;
                                                }

                                                //nQtyDO = (lstPoLink.Count > nLoopCi

                                                if ((!string.IsNullOrEmpty(tmp)) && (tmp.Length == 10))
                                                {
                                                    lstDOPD.Add(new LG_DOPD()
                                                    {
                                                        c_nosup = kodePrinsipal,
                                                        c_dono = dopd.c_dono,
                                                        c_iteno = dopd.c_iteno,
                                                        c_itenopri = dopd.c_itenopri,
                                                        v_itnam = dopd.v_itnam,
                                                        v_undes = dopd.v_undes,
                                                        n_qty = nPoQtySisa,
                                                        n_qty_sisa = nPoQtySisa,
                                                        c_batch = dopd.c_batch,
                                                        d_expired = dopd.d_expired,
                                                        c_pono = tmp,
                                                        n_disc = dopd.n_disc,
                                                        l_claim = dopd.l_claim,
                                                        c_type = dopd.c_type,
                                                        d_entry = dateNow,
                                                        l_done = false,
                                                        l_pending = isPending,
                                                        n_qtybns = nQtybns,
                                                        n_dprion = nDpriOn,
                                                        n_dprioff = nDpriOff,
                                                        n_damson = nDamsOn,
                                                        n_damsoff = nDamsOff,
                                                        c_nodpl = noDpl,
                                                        c_outlet = sOutlet,
                                                        c_po_outlet = sPoOutlet
                                                    });
                                                }
                                            }

                                            lstPoLink.Clear();

                                            #endregion
                                        }
                                        else
                                        {
                                            #region Single PO

                                            if (poData.IndexOf(PENDING_PO_NAME, StringComparison.OrdinalIgnoreCase) == -1)
                                            {
                                                isPending = false;
                                            }
                                            else
                                            {
                                                poData = poData.Replace(PENDING_PO_NAME, "").Trim();
                                                isPending = true;
                                            }

                                            //if ((!string.IsNullOrEmpty(poData)) && (poData.Length == 8))
                                            //{
                                            dopd.c_pono = poData;
                                            dopd.l_pending = isPending;

                                            lstDOPD.Add(dopd);
                                            //}


                                            #endregion
                                        }

                                        #region Insert DO not Valid Price

                                        var price = row.GetValue<decimal>("N_SALPRI");
                                        //if (price != nSalpri)
                                        //{
                                        //    dopdErr1 = new LG_DOPDErr1()
                                        //    {
                                        //        c_nosup = kodePrinsipal,
                                        //        c_dono = row.GetValue<string>("C_NODO", string.Empty),
                                        //        c_iteno = string.IsNullOrEmpty(row.GetValue<string>("C_ITENO", string.Empty).Trim()) ? kode_item : row.GetValue<string>("C_ITENO", string.Empty),
                                        //        c_itenopri = row.GetValue<string>("C_ITENOPRI", string.Empty),
                                        //        v_itnam = row.GetValue<string>("C_ITNAM", string.Empty),
                                        //        v_undes = row.GetValue<string>("C_UNDES", string.Empty),
                                        //        n_qty = nQtyDO,
                                        //        c_batch = row.GetValue<string>("C_BATCH", string.Empty),
                                        //        d_expired = date,
                                        //        n_disc = row.GetValue<decimal>("N_DISC"),
                                        //        d_entry = dateNow,
                                        //        n_dprion = nDpriOn,
                                        //        n_dprioff = nDpriOff,
                                        //        n_damson = nDamsOn,
                                        //        n_damsoff = nDamsOff,
                                        //        c_nodpl = noDpl,
                                        //        c_outlet = sOutlet,
                                        //        c_po_outlet = sPoOutlet,
                                        //        n_salpri = nSalpri,
                                        //        n_salpri_phar = price
                                        //    };

                                        //    lstDOPDErr1.Add(dopdErr1);

                                        //    var noPhar = (from q in db.LG_DOPDErr2s
                                        //                where q.c_dono == row.GetValue<string>("C_NODO", string.Empty)
                                        //                     && q.c_nosup == kodePrinsipal
                                        //                     && q.c_po_outlet == sPoOutlet
                                        //                select q.c_dono).Take(1).SingleOrDefault();

                                        //    if (string.IsNullOrEmpty(noPhar))
                                        //    {
                                        //        dopdErr2 = new LG_DOPDErr2()
                                        //        {
                                        //            c_nosup = kodePrinsipal,
                                        //            c_dono = row.GetValue<string>("C_NODO", string.Empty),
                                        //            c_iteno = string.IsNullOrEmpty(row.GetValue<string>("C_ITENO", string.Empty).Trim()) ? kode_item : row.GetValue<string>("C_ITENO", string.Empty),
                                        //            c_itenopri = row.GetValue<string>("C_ITENOPRI", string.Empty),
                                        //            v_itnam = row.GetValue<string>("C_ITNAM", string.Empty),
                                        //            v_undes = row.GetValue<string>("C_UNDES", string.Empty),
                                        //            n_qty = nQtyDO,
                                        //            c_batch = row.GetValue<string>("C_BATCH", string.Empty),
                                        //            d_expired = date,
                                        //            n_disc = row.GetValue<decimal>("N_DISC"),
                                        //            d_entry = dateNow,
                                        //            n_dprion = nDpriOn,
                                        //            n_dprioff = nDpriOff,
                                        //            n_damson = nDamsOn,
                                        //            n_damsoff = nDamsOff,
                                        //            c_nodpl = noDpl,
                                        //            c_outlet = sOutlet,
                                        //            c_po_outlet = sPoOutlet,
                                        //            n_salpri = nSalpri,
                                        //            n_salpri_phar = price
                                        //        };

                                        //        lstDOPDErr2.Add(dopdErr2);
                                        //    }
                                        //}
                                        #endregion
                                    }
                                }

                                if (ListSPD1.Count > 0 && lstDOPDErr1.Count == 0)
                                {
                                    lstDOPH.Add(doph);
                                }
                                else
                                {
                                    //lstDOPD.Clear();
                                    dbSpPo.Transaction.Rollback();
                                    continue;
                                }
                            }

                            #region Insert to DB SP & PO

                            if (ListSPH.Count > 0)
                            {
                                dbSpPo.LG_SPHs.InsertAllOnSubmit(ListSPH.ToArray());
                                ListSPH.Clear();
                            }
                            if (ListPOH.Count > 0)
                            {
                                dbSpPo.LG_POHs.InsertAllOnSubmit(ListPOH.ToArray());
                                ListPOH.Clear();
                            }
                            if (ListSPD1.Count > 0)
                            {
                                dbSpPo.LG_SPD1s.InsertAllOnSubmit(ListSPD1.ToArray());
                                ListSPD1.Clear();
                            }
                            if (ListSPD2.Count > 0)
                            {
                                dbSpPo.LG_SPD2s.InsertAllOnSubmit(ListSPD2.ToArray());
                                ListSPD2.Clear();
                            }
                            if (ListPOD1.Count > 0)
                            {
                                dbSpPo.LG_POD1s.InsertAllOnSubmit(ListPOD1.ToArray());
                                ListPOD1.Clear();
                            }
                            if (ListPOD2.Count > 0)
                            {
                                dbSpPo.LG_POD2s.InsertAllOnSubmit(ListPOD2.ToArray());
                                ListPOD2.Clear();
                            }
                            #region dateSP
                            if (dbSpPo != null)
                            {
                                string tmpNum = spID.Substring(6, 4);
                                string tmpNumPO = poID.Substring(6, 4);
                                //switch (DateTime.Now.Month)
                                switch (spDate.Month)
                                {
                                    case 1:
                                        sysNum.c_bln01 = tmpNum;
                                        //sysNumPO.c_bln01 = tmpNumPO;
                                        break;
                                    case 2:
                                        sysNum.c_bln02 = tmpNum;
                                        //sysNumPO.c_bln02 = tmpNumPO;
                                        break;
                                    case 3:
                                        sysNum.c_bln03 = tmpNum;
                                        //sysNumPO.c_bln03 = tmpNumPO;
                                        break;
                                    case 4:
                                        sysNum.c_bln04 = tmpNum;
                                        //sysNumPO.c_bln04 = tmpNumPO;
                                        break;
                                    case 5:
                                        sysNum.c_bln05 = tmpNum;
                                        //sysNumPO.c_bln05 = tmpNumPO;
                                        break;
                                    case 6:
                                        sysNum.c_bln06 = tmpNum;
                                        //sysNumPO.c_bln06 = tmpNumPO;
                                        break;
                                    case 7:
                                        sysNum.c_bln07 = tmpNum;
                                        //sysNumPO.c_bln07 = tmpNumPO;
                                        break;
                                    case 8:
                                        sysNum.c_bln08 = tmpNum;
                                        //sysNumPO.c_bln08 = tmpNumPO;
                                        break;
                                    case 9:
                                        sysNum.c_bln09 = tmpNum;
                                        //sysNumPO.c_bln09 = tmpNumPO;
                                        break;
                                    case 10:
                                        sysNum.c_bln10 = tmpNum;
                                        //sysNumPO.c_bln10 = tmpNumPO;
                                        break;
                                    case 11:
                                        sysNum.c_bln11 = tmpNum;
                                        //sysNumPO.c_bln11 = tmpNumPO;
                                        break;
                                    case 12:
                                        sysNum.c_bln12 = tmpNum;
                                        //sysNumPO.c_bln12 = tmpNumPO;
                                        break;
                                }
                            #endregion
                            #region DatePO
                                //switch (DateTime.Now.Month)
                                switch(poDate.Month)
                                {
                                    case 1:
                                        //sysNum.c_bln01 = tmpNum;
                                        sysNumPO.c_bln01 = tmpNumPO;
                                        break;
                                    case 2:
                                        //sysNum.c_bln02 = tmpNum;
                                        sysNumPO.c_bln02 = tmpNumPO;
                                        break;
                                    case 3:
                                        //sysNum.c_bln03 = tmpNum;
                                        sysNumPO.c_bln03 = tmpNumPO;
                                        break;
                                    case 4:
                                        //sysNum.c_bln04 = tmpNum;
                                        sysNumPO.c_bln04 = tmpNumPO;
                                        break;
                                    case 5:
                                        //sysNum.c_bln05 = tmpNum;
                                        sysNumPO.c_bln05 = tmpNumPO;
                                        break;
                                    case 6:
                                        //sysNum.c_bln06 = tmpNum;
                                        sysNumPO.c_bln06 = tmpNumPO;
                                        break;
                                    case 7:
                                        //sysNum.c_bln07 = tmpNum;
                                        sysNumPO.c_bln07 = tmpNumPO;
                                        break;
                                    case 8:
                                        //sysNum.c_bln08 = tmpNum;
                                        sysNumPO.c_bln08 = tmpNumPO;
                                        break;
                                    case 9:
                                        //sysNum.c_bln09 = tmpNum;
                                        sysNumPO.c_bln09 = tmpNumPO;
                                        break;
                                    case 10:
                                        //sysNum.c_bln10 = tmpNum;
                                        sysNumPO.c_bln10 = tmpNumPO;
                                        break;
                                    case 11:
                                        //sysNum.c_bln11 = tmpNum;
                                        sysNumPO.c_bln11 = tmpNumPO;
                                        break;
                                    case 12:
                                        //sysNum.c_bln12 = tmpNum;
                                        sysNumPO.c_bln12 = tmpNumPO;
                                        break;
                                }
                                #endregion
                                dbSpPo.SubmitChanges();
                                dbSpPo.Transaction.Commit();
                                dbSpPo.Dispose();
                            }
                            else
                            {
                                dbSpPo.Transaction.Rollback();
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            if (dbSpPo.Transaction != null)
                            {
                                dbSpPo.Transaction.Rollback();
                            }
                        }
                    }

                    #region Insert RN

                    string typeTrx = null,
                      typeDtl = null,
                      noSup = null,
                      noDo = null,
                      noItem = null,
                      noBatch = null;

                    string[] typeList = null;
                    List<string> lstTemp = null;

                    bool isFloating = false;

                    List<LG_RND2> listRnd2Copy = null;
                    List<LG_RND3> listRnd3 = null;
                    List<LG_RND3> listRnd3Copy = null;
                    List<LG_RND5> listRnd5 = null;

                    List<LG_RSD2> listRSD2 = null;
                    List<LG_RSD2> listRSD2Copy = null;

                    List<LG_DOPD> lstReDOPD = null;

                    LG_RNH rnh = null;
                    LG_RND1 rnd1 = null;
                    LG_RND2 rnd2 = null;
                    LG_RND3 rnd3 = null;

                    LG_RSD2 rsd2 = null;

                    LG_ClaimAccD claimaccd = null;

                    decimal nQtyGood = 0,
                      nQtyBad = 0,
                      nQtyAllocGood = 0,
                      nQtyAllocBad = 0,
                      nQtyAlloc = 0;

                    bool modifedPo = false;
                    IDictionary<string, string> dic = null;

                    //nipEntry = "system";
                    Constant.Gudang = '1';

                    listRnh = new List<LG_RNH>();
                    listRnd1 = new List<LG_RND1>();
                    listRnd2 = new List<LG_RND2>();
                    listRnd3 = new List<LG_RND3>();

                    SysNo sysNumRN = (from q in db.SysNos
                                      where q.c_portal == '3' && q.c_type == "03"
                                        && q.s_tahun == DateTime.Now.Year
                                      select q).SingleOrDefault();

                    for (nLoopRh = 0, nLenC = lstDOPH.Count; nLoopRh < nLenC; nLoopRh++)
                    {
                        if (!string.IsNullOrEmpty(rnIDTemp))
                        {
                            int digit = 0;
                            if (IsNumeric(rnIDTemp.Substring(6, 1)))
                            {
                                digit = int.Parse(rnIDTemp.Substring(6, 4)) + 1;
                                if (digit >= 10000)
                                {
                                    rnID = rnIDTemp.Substring(0, 6) + "A001";
                                }
                                else
                                {
                                    rnID = rnIDTemp.Substring(0, 6) + digit.ToString().PadLeft(4, '0');
                                }
                            }
                            else
                            {
                                digit = int.Parse(rnIDTemp.Substring(7, 3)) + 1;
                                if (digit >= 1000)
                                {
                                    rnID = rnIDTemp.Substring(0, 6) + ChangeAlphabet(rnIDTemp.Substring(6, 1)).ToUpper() + "001";
                                }
                                else
                                {
                                    rnID = rnIDTemp.Substring(0, 7) + digit.ToString().PadLeft(3, '0');
                                }
                            }
                        }
                        else
                        {
                            rnID = Commons2.GenerateNumbering<LG_RNH>(db, "RN", '3', "03", dateNow, "c_rnno");
                        }

                        rnIDTemp = rnID;

                        listRnh.Add(new LG_RNH()
                        {
                            c_dono = lstDOPH[nLoopRh].c_dono,
                            c_entry = nipEntry,
                            c_from = lstDOPH[nLoopRh].c_nosup,
                            c_gdg = '1',
                            c_rnno = rnIDTemp,
                            c_type = "01",
                            c_update = nipEntry,
                            d_dodate = lstDOPH[nLoopRh].d_dodate,
                            d_entry = dateNow,
                            d_rndate = dateNow,
                            d_update = dateNow,
                            n_bea = 0,
                            l_float = false,
                            l_print = false,
                            l_status = false,
                            l_rnkhusus = true,
                            l_khusus = false,
                            v_ket = "RN PHARMANET",
                            c_po_outlet = lstDOPH[nLoopRh].c_po_outlet
                        });

                        if (lstDOPD.Count > 0)
                        {
                            lstReDOPD = new List<LG_DOPD>();

                            lstReDOPD = (from q in lstDOPH
                                         join q1 in lstDOPD on new { q.c_nosup, q.c_dono } equals new { q1.c_nosup, q1.c_dono }
                                         where q1.c_dono == lstDOPH[nLoopRh].c_dono
                                         && q1.c_nosup == lstDOPH[nLoopRh].c_nosup
                                         select q1).Distinct().ToList();

                            for (nLoopRd = 0, nLenCi = lstReDOPD.Count; nLoopRd < nLenCi; nLoopRd++)
                            {
                                #region Detail Data

                                listRnd2.Add(new LG_RND2()
                                {
                                    c_batch = lstReDOPD[nLoopRd].c_batch,
                                    c_gdg = '1',
                                    c_iteno = lstReDOPD[nLoopRd].c_iteno,
                                    c_no = lstReDOPD[nLoopRd].c_pono,
                                    c_rnno = rnIDTemp,
                                    c_type = "01",
                                    n_bqty = 0,
                                    n_gqty = lstReDOPD[nLoopRd].n_qty,
                                    n_floqty = 0
                                });

                                rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
                                {
                                    return lstReDOPD[nLoopRd].c_iteno.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                      lstReDOPD[nLoopRd].c_batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                      rnIDTemp.Equals((string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
                                });

                                if (rnd1 == null)
                                {
                                    listRnd1.Add(new LG_RND1()
                                    {
                                        c_batch = lstReDOPD[nLoopRd].c_batch,
                                        c_gdg = '1',
                                        c_iteno = lstReDOPD[nLoopRd].c_iteno,
                                        c_rnno = rnIDTemp,
                                        n_bqty = 0,
                                        n_bsisa = 0,
                                        n_gqty = lstReDOPD[nLoopRd].n_qty,
                                        n_gsisa = 0,
                                    });

                                    // Check Batch
                                    var dbBatch = new ORMDataContext(this.config.ConnectionString);
                                    Commons2.CheckAndProcessBatch(dbBatch, lstReDOPD[nLoopRd].c_iteno, lstReDOPD[nLoopRd].c_batch, lstReDOPD[nLoopRd].d_expired.Value, "system");
                                    dbBatch.SubmitChanges();
                                }
                                else
                                {
                                    rnd1.n_gsisa = rnd1.n_gqty += lstReDOPD[nLoopRd].n_qty;
                                }

                                #endregion
                            }
                        }

                        if (nLoopRh == 0)
                        {
                            strCommand = "select b.v_namatax [C_pt],a.c_dono [C_nodo],a.c_iteno,c.c_itenopri,c.v_itnam [C_itnam], c.v_undes [C_undes],a.n_qty [N_qtyrcv], " +
                                    "a.c_batch, a.d_expired,right(a.c_pono,8) [C_nosp], d.d_dodate [D_tglsp], a.n_disc, a.l_claim [L_claimbns], " +
                                    "d.c_po_outlet [C_po_outle],c.n_salpri, a.n_qtybns,a.n_dprion, a.n_dprioff,a.n_damson,a.n_damsoff,a.c_nodpl [c_nodpf] " +
                                    "from LG_DOPD a inner join LG_DatSup b on a.c_nosup = b.c_nosup " +
                                    "inner join FA_MasItm c on a.c_iteno = c.c_iteno " +
                                    "inner join LG_DOPH d on a.c_nosup = d.c_nosup and a.c_dono = d.c_dono " +
                                    "where a.c_nosup = '" + lstDOPH[nLoopRh].c_nosup + "' and a.c_dono = '" + lstDOPH[nLoopRh].c_dono + "' and a.c_po_outlet != '' ";
                        }
                        else
                        {
                            strCommand += " union all " +
                                "select b.v_namatax [C_pt],a.c_dono [C_nodo],a.c_iteno,c.c_itenopri,c.v_itnam [C_itnam], c.v_undes [C_undes],a.n_qty [N_qtyrcv], " +
                                    "a.c_batch, a.d_expired,right(a.c_pono,8) [C_nosp], d.d_dodate [D_tglsp], a.n_disc, a.l_claim [L_claimbns], " +
                                    "d.c_po_outlet [C_po_outle],c.n_salpri, a.n_qtybns,a.n_dprion, a.n_dprioff,a.n_damson,a.n_damsoff,a.c_nodpl [c_nodpf] " +
                                    "from LG_DOPD a inner join LG_DatSup b on a.c_nosup = b.c_nosup " +
                                    "inner join FA_MasItm c on a.c_iteno = c.c_iteno " +
                                    "inner join LG_DOPH d on a.c_nosup = d.c_nosup and a.c_dono = d.c_dono " +
                                    "where a.c_nosup = '" + lstDOPH[nLoopRh].c_nosup + "' and a.c_dono = '" + lstDOPH[nLoopRh].c_dono + "' and a.c_po_outlet != '' ";
                        }
                    }
                    #endregion

                    #region Insert PL

                    LG_PLH plh = null;
                    LG_PLD1 pld1 = null;
                    LG_PLD2 pld2 = null;

                    List<LG_PLH> listPlh = null;
                    List<LG_PLD1> listPld1 = null;
                    List<LG_PLD2> listPld2 = null;

                    List<LG_SPH> listSPH = null;
                    List<LG_SPD1> listSPD1 = null;
                    List<LG_SPD2> listSPD2 = null;

                    List<LG_DOD1> listDOD1 = null;
                    List<LG_DOD2> listDOD2 = null;

                    //List<FA_MasItm> lstFaItm = null;
                    List<LG_SPD1> lstSPD1 = null;
                    //List<LG_RND1> lstRND1 = null;
                    List<LG_RND2> lstRND2 = null;

                    List<LG_PLD1_SUM_BYBATCH> listPLSum = null;
                    LG_PLD1_SUM_BYBATCH pld1Sum = null;

                    LG_DOH checkDoh = null;

                    listPlh = new List<LG_PLH>();
                    listPld1 = new List<LG_PLD1>();
                    listPld2 = new List<LG_PLD2>();

                    listSPH = new List<LG_SPH>();
                    listSPD1 = new List<LG_SPD1>();
                    listSPD2 = new List<LG_SPD2>();

                    listDOH = new List<LG_DOH>();
                    listDOD1 = new List<LG_DOD1>();
                    listDOD2 = new List<LG_DOD2>();

                    listReDo = new List<AddReDO>();
                    listReDoGroup = new List<AddReDO>();

                    lstFjd1 = new List<LG_FJD1>();
                    lstFjd2 = new List<LG_FJD2>();

                    //lstSpLink = new List<SP_Link>();
                    lstSPD1 = new List<LG_SPD1>();
                    lstRND2 = new List<LG_RND2>();
                    lstReDOPD = new List<LG_DOPD>();


                    Constant.TRANSID = (from q in db.LG_PLHs
                                        where q.d_pldate.Value.Month == dateNow.Date.Month
                                        select q.c_plno).Max();

                    lstDoPharLink = (from q in lstDOPH
                                     join q1 in listRnh on q.c_dono equals q1.c_dono
                                     orderby q.c_po_outlet
                                     select new DOPhar_Link()
                                     {
                                         NoSup = q.c_nosup,
                                         DoNo = q.c_dono,
                                         Cab = q.c_cab,
                                         RnNo = q1.c_rnno,
                                         OutletPO = q1.c_po_outlet,
                                         PLPhar = q.c_plphar
                                     }).Distinct().ToList();

                    string PoOutlet = string.Empty;

                    SysNo sysNumPL = (from q in db.SysNos
                                      where q.c_portal == '3' && q.c_type == "08"
                                        && q.s_tahun == DateTime.Now.Year
                                      select q).SingleOrDefault();

                    SysNo sysNumDO = (from q in db.SysNos
                                      where q.c_portal == '3' && q.c_type == "09"
                                        && q.s_tahun == DateTime.Now.Year
                                      select q).SingleOrDefault();

                    sysnoNumber sysNumAuto = (from q in db.sysnoNumbers
                                              where q.c_portal == '3'
                                              && q.c_notrans == "08"
                                                && q.s_tahun == dateNow.Year
                                                && q.c_gdg == Constant.Gudang
                                              select q).SingleOrDefault();

                    for (nLoopPh = 0, nLoop = lstDoPharLink.Count; nLoopPh < nLoop; nLoopPh++)
                    {
                        var noPo = (from q in lstDOPH
                                    join q1 in lstDOPD on new { q.c_dono, q.c_nosup } equals new { q1.c_dono, q1.c_nosup }
                                    where q.c_dono == lstDoPharLink[nLoopPh].DoNo
                                        && q.c_nosup == lstDoPharLink[nLoopPh].NoSup
                                        && q.c_po_outlet == lstDoPharLink[nLoopPh].OutletPO
                                    select q1.c_pono).Take(1).SingleOrDefault();

                        lstSpLink = (from q in db.LG_SPHs
                                     join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
                                     join q2 in db.LG_POD2s on q.c_spno equals q2.c_orno
                                     join q3 in db.LG_POHs on q2.c_pono equals q3.c_pono
                                     where q.c_po_outlet == lstDoPharLink[nLoopPh].OutletPO
                                        && (q1.n_sisa > 0)
                                        && q3.c_nosup == lstDoPharLink[nLoopPh].NoSup
                                        && q3.c_pono == noPo
                                     select new SP_Link()
                                     {
                                         SP = q.c_spno,
                                         Cusno = q.c_cusno,
                                         OutletPO = q.c_po_outlet,
                                         Outlet = q.c_outlet
                                     }).Distinct().ToList();

                        spl = lstSpLink.Find(delegate(SP_Link sppol)
                        {
                            return (string.IsNullOrEmpty(sppol.SP) ? false : true);
                        });

                        tmpCustomer = spl.Cusno;

                        checkDoh = (from q in db.LG_DOHs
                                    where q.c_po_outlet == lstDoPharLink[nLoopPh].OutletPO
                                        && q.c_rnno == null
                                    select q).Take(1).SingleOrDefault();

                        if (checkDoh == null)
                        {
                            if (PoOutlet != lstDoPharLink[nLoopPh].OutletPO)
                            {
                                #region PL Header

                                if (!string.IsNullOrEmpty(plIDTemp))
                                {
                                    int digit = 0;
                                    if (IsNumeric(plIDTemp.Substring(6, 1)))
                                    {
                                        digit = int.Parse(plIDTemp.Substring(6, 4)) + 1;
                                        if (digit >= 10000)
                                        {
                                            plID = plIDTemp.Substring(0, 6) + "A001";
                                        }
                                        else
                                        {
                                            plID = plIDTemp.Substring(0, 6) + digit.ToString().PadLeft(4, '0');
                                        }
                                    }
                                    else
                                    {
                                        digit = int.Parse(plIDTemp.Substring(7, 3)) + 1;
                                        if (digit >= 1000)
                                        {
                                            plID = plIDTemp.Substring(0, 6) + ChangeAlphabet(plIDTemp.Substring(6, 1)).ToUpper() + "001";
                                        }
                                        else
                                        {
                                            plID = plIDTemp.Substring(0, 7) + digit.ToString().PadLeft(3, '0');
                                        }
                                    }
                                }
                                else
                                {
                                    plID = Commons2.GenerateNumbering<LG_PLH>(db, "PL", '3', "08", dateNow, "c_plno");
                                }

                                plIDTemp = plID;

                                if (!string.IsNullOrEmpty(plIDAutoTemp))
                                {
                                    //PL150601000022
                                    int digit2 = int.Parse(plIDAutoTemp.Substring(8, 6)) + 1;
                                    plIDAutoTemp = plIDAutoTemp.Substring(0, 8) + digit2.ToString().PadLeft(6, '0');
                                }
                                else
                                {
                                    plIDAutoTemp = Constant.NUMBERID_GUDANG;
                                }

                                listPlh.Add(new LG_PLH()
                                {
                                    c_plno = plID,
                                    c_cusno = tmpCustomer,
                                    c_entry = nipEntry,
                                    c_gdg = '1',
                                    c_nosup = kodePrinsipal,
                                    c_type = "02",
                                    c_update = nipEntry,
                                    c_via = via,
                                    d_entry = dateNow,
                                    d_pldate = dateNow.Date,
                                    d_update = dateNow,
                                    l_confirm = true,
                                    l_print = true,
                                    v_ket = "Sys: Auto Generate Pharmanet",
                                    c_type_cat = string.Empty,
                                    c_plnum = plIDAutoTemp,
                                });

                                #endregion

                                #region DO Header

                                if (!string.IsNullOrEmpty(doIDTemp))
                                {
                                    int digit = 0;
                                    if (IsNumeric(doIDTemp.Substring(6, 1)))
                                    {
                                        digit = int.Parse(doIDTemp.Substring(6, 4)) + 1;
                                        if (digit >= 10000)
                                        {
                                            doID = doIDTemp.Substring(0, 6) + "A001";
                                        }
                                        else
                                        {
                                            doID = doIDTemp.Substring(0, 6) + digit.ToString().PadLeft(4, '0');
                                        }
                                    }
                                    else
                                    {
                                        digit = int.Parse(doIDTemp.Substring(7, 3)) + 1;
                                        if (digit >= 1000)
                                        {
                                            doID = doIDTemp.Substring(0, 6) + ChangeAlphabet(doIDTemp.Substring(6, 1)).ToUpper() + "001";
                                        }
                                        else
                                        {
                                            doID = doIDTemp.Substring(0, 7) + digit.ToString().PadLeft(3, '0');
                                        }
                                    }
                                }
                                else
                                {
                                    doID = Commons2.GenerateNumbering<LG_DOH>(db, "DO", '3', "09", dateNow, "c_dono");
                                }

                                doIDTemp = doID;

                                doh = new LG_DOH()
                                {
                                    c_dono = doID,
                                    d_dodate = DateTime.Now,
                                    c_gdg = '1',
                                    c_type = "01",
                                    c_cusno = spl.Cusno,
                                    c_plno = plID,
                                    c_via = via,
                                    v_ket = string.Concat("Auto DO Pharmanet : ", plID, " - ", doID),
                                    //c_pin = valueS.ToString(),
                                    l_status = true,
                                    l_print = true,
                                    l_send = false,
                                    l_sent = true,
                                    c_entry = nipEntry,
                                    d_entry = dateNow,
                                    c_update = nipEntry,
                                    d_update = dateNow,
                                    l_delete = false,
                                    c_outlet = spl.Outlet,
                                    c_po_outlet = spl.OutletPO,
                                    c_plphar = lstDoPharLink[nLoopPh].PLPhar
                                };

                                listDOH.Add(doh);
                                nomorDO.Add(doID);
                                #endregion
                            }
                        }
                        else
                        {
                            plID = checkDoh.c_plno;
                            doID = checkDoh.c_dono;
                        }

                        #region Insert Detail

                        if (listRnd2.Count > 0)
                        {
                            lstSPD1 = (from q in db.LG_SPHs
                                       join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
                                       where q.c_spno == spl.SP
                                        && (q1.n_sisa > 0)
                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       select q1).Distinct().ToList();

                            lstRND2 = (from q in listRnh
                                       join q1 in listRnd2 on q.c_rnno equals q1.c_rnno
                                       where q1.c_rnno == lstDoPharLink[nLoopPh].RnNo
                                       select q1).Distinct().ToList();

                            for (nLoopPd = 0, nLen = lstRND2.Count; nLoopPd < nLen; nLoopPd++)
                            {
                                #region PL Detail

                                #region Sp Normal

                                spd1 = lstSPD1.Find(delegate(LG_SPD1 spd)
                                {
                                    return (string.IsNullOrEmpty(spd.c_spno) ? string.Empty : spd.c_spno.Trim()).Equals(spl.SP, StringComparison.OrdinalIgnoreCase) &&
                                      (string.IsNullOrEmpty(spd.c_iteno) ? string.Empty : spd.c_iteno.Trim()).Equals(lstRND2[nLoopPd].c_iteno.Trim(), StringComparison.OrdinalIgnoreCase) &&
                                      ((spd.n_sisa.HasValue ? spd.n_sisa.Value : 0) > 0);
                                });

                                if (spd1 != null)
                                {
                                    pld1 = new LG_PLD1()
                                    {
                                        c_batch = lstRND2[nLoopPd].c_batch,
                                        c_iteno = lstRND2[nLoopPd].c_iteno,
                                        c_plno = plID,
                                        c_spno = spl.SP,
                                        c_type = "01",
                                        n_booked = lstRND2[nLoopPd].n_gqty,
                                        n_qty = lstRND2[nLoopPd].n_gqty,
                                        n_sisa = 0
                                        //c_po_outlet = spl.OutletPO
                                    };

                                    spd1.n_sisa -= lstRND2[nLoopPd].n_gqty;
                                    #region PLD2

                                    pld2 = new LG_PLD2()
                                    {
                                        c_batch = lstRND2[nLoopPd].c_batch,
                                        c_iteno = lstRND2[nLoopPd].c_iteno,
                                        c_plno = plID,
                                        c_rnno = lstRND2[nLoopPd].c_rnno,
                                        c_spno = spl.SP,
                                        c_type = "01",
                                        n_qty = lstRND2[nLoopPd].n_gqty,
                                        n_sisa = lstRND2[nLoopPd].n_gqty
                                    };

                                    #endregion
                                }

                                #endregion

                                listPld1.Add(pld1);
                                listPld2.Add(pld2);
                                #endregion
                            }

                            lstSPD1.Clear();
                            lstRND2.Clear();
                        }

                        #endregion

                        PoOutlet = lstDoPharLink[nLoopPh].OutletPO;
                    }

                    #region Insert DO Detail

                    #region Pin

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

                        char[] item = listPld1[nHit].c_iteno.ToCharArray();
                        foreach (char letter in item)
                        {
                            value = Convert.ToInt64(letter);
                            valueS += value;
                        }
                        //valueS += decimal.Parse(listPld1[nHit].c_iteno);
                        Hit++;
                        nHit++;
                    }

                    valueS = Math.Round(valueS);

                    #endregion

                    listPLSum =
                      listPld1.GroupBy(x => new { x.c_plno, x.c_iteno })
                      .Select(x => new LG_PLD1_SUM_BYBATCH() { c_plno = x.Key.c_plno, c_iteno = x.Key.c_iteno, n_qty = x.Sum(y => (y.n_qty.HasValue ? y.n_qty.Value : 0)) }).ToList();

                    if (listPLSum.Count > 0)
                    {
                        #region DOD1
                        string cusNo = string.Empty;

                        for (nLoop = 0; nLoop < listPLSum.Count; nLoop++)
                        {
                            pld1Sum = listPLSum[nLoop];

                            if (pld1Sum != null)
                            {
                                doh = listDOH.Find(delegate(LG_DOH doo)
                                {
                                    return (string.IsNullOrEmpty(doo.c_plno) ? string.Empty : doo.c_plno.Trim()).Equals(pld1Sum.c_plno, StringComparison.OrdinalIgnoreCase);
                                });

                                if (doh != null)
                                {
                                    valueS += 3;
                                    doh.c_pin = valueS.ToString();
                                    doID = doh.c_dono;
                                }
                                else
                                {
                                    doh2 = (from q in db.LG_DOHs
                                            where q.c_plno == pld1Sum.c_plno
                                            select q).Take(1).SingleOrDefault();

                                    if (doh2 != null)
                                    {
                                        doID = doh2.c_dono;
                                        cusNo = doh2.c_cusno != null ? doh2.c_cusno : string.Empty;
                                    }

                                    listReDo.Add(new AddReDO()
                                         {
                                             c_dono = doID,
                                             c_cusno = cusNo
                                         });

                                }

                                listDOD1.Add(new LG_DOD1()
                                {
                                    c_dono = doID,
                                    c_iteno = pld1Sum.c_iteno,
                                    c_via = via,
                                    n_qty = pld1Sum.n_qty,
                                    n_sisa = pld1Sum.n_qty
                                });
                            }
                        }

                        if ((listReDo != null) && listReDo.Count > 1)
                        {
                            listReDoGroup = listReDo.GroupBy(x => new { x.c_dono, x.c_cusno })
                                            .Select(x => new AddReDO() { c_dono = x.Key.c_dono, c_cusno = x.Key.c_cusno }).ToList();

                            listReDo.Clear();
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

                            doh = listDOH.Find(delegate(LG_DOH doo)
                            {
                                return (string.IsNullOrEmpty(doo.c_plno) ? string.Empty : doo.c_plno.Trim()).Equals(pld1Sum.c_plno, StringComparison.OrdinalIgnoreCase);
                            });

                            if (doh != null)
                            {
                                doID = doh.c_dono;
                            }
                            else
                            {
                                doID = (from q in db.LG_DOHs
                                        where q.c_plno == pld1Sum.c_plno
                                        select q.c_dono).Take(1).SingleOrDefault();

                            }

                            if (pld1Sum != null)
                            {
                                listDOD2.Add(new LG_DOD2()
                                {
                                    c_dono = doID,
                                    c_iteno = pld1Sum.c_iteno,
                                    c_via = via,
                                    c_batch = pld1Sum.c_batch,
                                    n_qty = pld1Sum.n_qty,
                                    n_sisa = pld1Sum.n_qty
                                });
                            }
                        }

                        listPLSum.Clear();

                        #endregion
                    }

                    #endregion

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

                    if ((listDOH != null) && (listDOH.Count > 0))
                    {
                        db.LG_DOHs.InsertAllOnSubmit(listDOH.ToArray());
                        //listDOH.Clear();
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

                    #endregion

                    #endregion

                    #endregion

                    #region old
                    //#region Check Header

                    //lstDOPHeaderCheck = lstDOPH.GroupBy(x => new { x.c_nosup, x.c_dono })
                    //  .Select(y => new DO_PI_Header_Check()
                    //  {
                    //      Principal = y.Key.c_nosup.Trim(),
                    //      DO = y.Key.c_dono.Trim()
                    //  }).ToList();

                    //if ((lstDOPHeaderCheck != null) && (lstDOPHeaderCheck.Count > 1))
                    //{
                    //    lstDOPResult = (from q in db.LG_DOPHs
                    //                    where lstDOPHeaderCheck.Contains(new DO_PI_Header_Check()
                    //                    {
                    //                        Principal = q.c_nosup,
                    //                        DO = q.c_dono
                    //                    })
                    //                    select new DO_PI_Result()
                    //                    {
                    //                        Principal = q.c_nosup.Trim(),
                    //                        DO = q.c_dono.Trim()
                    //                    }).Distinct().ToList();

                    //    if ((lstDOPResult != null) && (lstDOPResult.Count > 0))
                    //    {
                    //        lstDOPHRem = (from q in lstDOPH
                    //                      join q1 in lstDOPResult on q.c_nosup equals q1.Principal
                    //                      select q).ToList();

                    //        if ((lstDOPHRem != null) && (lstDOPHRem.Count > 0))
                    //        {
                    //            for (nLoop = 0, nLen = lstDOPHRem.Count; nLoop < nLen; nLoop++)
                    //            {
                    //                lstDOPH.Remove(lstDOPHRem[nLoop]);
                    //            }

                    //            lstDOPHRem.Clear();
                    //        }

                    //        lstDOPResult.Clear();
                    //    }

                    //    lstDOPHeaderCheck.Clear();
                    //}

                    //#endregion

                    //#region Check Detail


                    //lstDOPDetailCheck = lstDOPD.GroupBy(x => new { x.c_nosup, x.c_dono, x.c_iteno, x.c_type, x.c_pono, x.c_batch })
                    //  .Select(y => new DO_PI_Detail_Check()
                    //  {
                    //      Principal = y.Key.c_nosup.Trim(),
                    //      DO = y.Key.c_dono.Trim(),
                    //      Item = y.Key.c_iteno.Trim(),
                    //      PO = y.Key.c_pono.Trim(),
                    //      Batch = y.Key.c_batch.Trim()
                    //  }).ToList();

                    //if ((lstDOPDetailCheck != null) && (lstDOPDetailCheck.Count > 1))
                    //{
                    //    lstDOPResult = (from q in db.LG_DOPDs
                    //                    where lstDOPDetailCheck.Contains(new DO_PI_Detail_Check()
                    //                    {
                    //                        Principal = q.c_nosup,
                    //                        DO = q.c_dono,
                    //                        Item = q.c_iteno,
                    //                        PO = q.c_pono,
                    //                        Batch = q.c_batch
                    //                    })
                    //                    select new DO_PI_Result()
                    //                    {
                    //                        Principal = q.c_nosup.Trim(),
                    //                        DO = q.c_dono.Trim(),
                    //                        Item = q.c_iteno.Trim(),
                    //                        PO = q.c_pono.Trim(),
                    //                        Batch = q.c_batch.Trim()
                    //                    }).Distinct().ToList();

                    //    if ((lstDOPResult != null) && (lstDOPResult.Count > 0))
                    //    {
                    //        lstDOPDRem = (from q in lstDOPD
                    //                      join q1 in lstDOPResult on q.c_nosup equals q1.Principal
                    //                      select q).Distinct().ToList();

                    //        if ((lstDOPDRem != null) && (lstDOPDRem.Count > 0))
                    //        {
                    //            for (nLoop = 0, nLen = lstDOPDRem.Count; nLoop < nLen; nLoop++)
                    //            {
                    //                lstDOPD.Remove(lstDOPDRem[nLoop]);
                    //            }

                    //            lstDOPDRem.Clear();
                    //        }

                    //        lstDOPResult.Clear();
                    //    }

                    //    lstDOPDetailCheck.Clear();
                    //}

                    //#endregion

                    #endregion
                    #region Insert Nomor RN, PL, DO 
                    if (lstDOPH.Count > 0)
                    {
                        nChanges += lstDOPH.Count();

                        db.LG_DOPHs.InsertAllOnSubmit(lstDOPH.ToArray());
                        lstDOPH.Clear();
                    }

                    if (lstDOPD.Count > 0)
                    {
                        nChanges += lstDOPD.Count();

                        db.LG_DOPDs.InsertAllOnSubmit(lstDOPD.ToArray());
                        lstDOPD.Clear();
                    }
                    if (lstDOPHErrDel1.Count > 0)
                    {
                        db.LG_DOPHErr1s.DeleteAllOnSubmit(lstDOPHErrDel1.ToArray());
                        lstDOPHErrDel1.Clear();
                    }

                    if (lstDOPHErr1.Count > 0)
                    {
                        db.LG_DOPHErr1s.InsertAllOnSubmit(lstDOPHErr1.ToArray());

                        dateError = (from q in lstDOPHErr1
                                     orderby q.d_dodate descending
                                     select q.d_dodate.Value).Take(1).SingleOrDefault();

                        //lstDOPHErr1.Clear();
                    }

                    if (lstDOPHErr2.Count > 0)
                    {
                        db.LG_DOPHErr2s.InsertAllOnSubmit(lstDOPHErr2.ToArray());
                        lstDOPHErr2.Clear();
                    }

                    if (lstDOPDErrDel1.Count > 0)
                    {
                        db.LG_DOPDErr1s.DeleteAllOnSubmit(lstDOPDErrDel1.ToArray());
                        lstDOPDErrDel1.Clear();
                    }

                    if (lstDOPDErr1.Count > 0)
                    {
                        db.LG_DOPDErr1s.InsertAllOnSubmit(lstDOPDErr1.ToArray());
                    }

                    if (lstDOPDErr2.Count > 0)
                    {
                        nChanges += lstDOPDErr2.Count();

                        db.LG_DOPDErr2s.InsertAllOnSubmit(lstDOPDErr2.ToArray());
                        lstDOPDErr2.Clear();
                    }

                    if ((listRnh.Count > 0) && (listRnd1.Count > 0) && (listRnd2.Count > 0))
                    {
                        db.LG_RNHs.InsertAllOnSubmit(listRnh.ToArray());
                        db.LG_RND1s.InsertAllOnSubmit(listRnd1.ToArray());
                        db.LG_RND2s.InsertAllOnSubmit(listRnd2.ToArray());

                        listRnh.Clear();
                        listRnd1.Clear();
                        listRnd2.Clear();

                        string tmpNumRN = rnIDTemp.Substring(6, 4);

                        if (!string.IsNullOrEmpty(plIDTemp) && !string.IsNullOrEmpty(doIDTemp) && !string.IsNullOrEmpty(plIDAutoTemp))
                        {
                            string tmpNumPL = plIDTemp.Substring(6, 4);
                            string tmpNumDO = doIDTemp.Substring(6, 4);
                            string tmpNumAuto = plIDAutoTemp.Substring(8, 6);
                            switch (DateTime.Now.Month)
                            {
                                case 1:
                                    sysNumRN.c_bln01 = tmpNumRN;
                                    sysNumPL.c_bln01 = tmpNumPL;
                                    sysNumDO.c_bln01 = tmpNumDO;
                                    sysNumAuto.c_bln01 = tmpNumAuto;
                                    break;
                                case 2:
                                    sysNumRN.c_bln02 = tmpNumRN;
                                    sysNumPL.c_bln02 = tmpNumPL;
                                    sysNumDO.c_bln02 = tmpNumDO;
                                    sysNumAuto.c_bln02 = tmpNumAuto;
                                    break;
                                case 3:
                                    sysNumRN.c_bln03 = tmpNumRN;
                                    sysNumPL.c_bln03 = tmpNumPL;
                                    sysNumDO.c_bln03 = tmpNumDO;
                                    sysNumAuto.c_bln03 = tmpNumAuto;
                                    break;
                                case 4:
                                    sysNumRN.c_bln04 = tmpNumRN;
                                    sysNumPL.c_bln04 = tmpNumPL;
                                    sysNumDO.c_bln04 = tmpNumDO;
                                    sysNumAuto.c_bln04 = tmpNumAuto;
                                    break;
                                case 5:
                                    sysNumRN.c_bln05 = tmpNumRN;
                                    sysNumPL.c_bln05 = tmpNumPL;
                                    sysNumDO.c_bln05 = tmpNumDO;
                                    sysNumAuto.c_bln05 = tmpNumAuto;
                                    break;
                                case 6:
                                    sysNumRN.c_bln06 = tmpNumRN;
                                    sysNumPL.c_bln06 = tmpNumPL;
                                    sysNumDO.c_bln06 = tmpNumDO;
                                    sysNumAuto.c_bln06 = tmpNumAuto;
                                    break;
                                case 7:
                                    sysNumRN.c_bln07 = tmpNumRN;
                                    sysNumPL.c_bln07 = tmpNumPL;
                                    sysNumDO.c_bln07 = tmpNumDO;
                                    sysNumAuto.c_bln07 = tmpNumAuto;
                                    break;
                                case 8:
                                    sysNumRN.c_bln08 = tmpNumRN;
                                    sysNumPL.c_bln08 = tmpNumPL;
                                    sysNumDO.c_bln08 = tmpNumDO;
                                    sysNumAuto.c_bln08 = tmpNumAuto;
                                    break;
                                case 9:
                                    sysNumRN.c_bln09 = tmpNumRN;
                                    sysNumPL.c_bln09 = tmpNumPL;
                                    sysNumDO.c_bln09 = tmpNumDO;
                                    sysNumAuto.c_bln09 = tmpNumAuto;
                                    break;
                                case 10:
                                    sysNumRN.c_bln10 = tmpNumRN;
                                    sysNumPL.c_bln10 = tmpNumPL;
                                    sysNumDO.c_bln10 = tmpNumDO;
                                    sysNumAuto.c_bln10 = tmpNumAuto;
                                    break;
                                case 11:
                                    sysNumRN.c_bln11 = tmpNumRN;
                                    sysNumPL.c_bln11 = tmpNumPL;
                                    sysNumDO.c_bln11 = tmpNumDO;
                                    sysNumAuto.c_bln11 = tmpNumAuto;
                                    break;
                                case 12:
                                    sysNumRN.c_bln12 = tmpNumRN;
                                    sysNumPL.c_bln12 = tmpNumPL;
                                    sysNumDO.c_bln12 = tmpNumDO;
                                    sysNumAuto.c_bln12 = tmpNumAuto;
                                    break;
                            }
                        }
                        else
                        {
                            switch (DateTime.Now.Month)
                            {
                                case 1:
                                    sysNumRN.c_bln01 = tmpNumRN;
                                    break;
                                case 2:
                                    sysNumRN.c_bln02 = tmpNumRN;
                                    break;
                                case 3:
                                    sysNumRN.c_bln03 = tmpNumRN;
                                    break;
                                case 4:
                                    sysNumRN.c_bln04 = tmpNumRN;
                                    break;
                                case 5:
                                    sysNumRN.c_bln05 = tmpNumRN;
                                    break;
                                case 6:
                                    sysNumRN.c_bln06 = tmpNumRN;
                                    break;
                                case 7:
                                    sysNumRN.c_bln07 = tmpNumRN;
                                    break;
                                case 8:
                                    sysNumRN.c_bln08 = tmpNumRN;
                                    break;
                                case 9:
                                    sysNumRN.c_bln09 = tmpNumRN;
                                    break;
                                case 10:
                                    sysNumRN.c_bln10 = tmpNumRN;
                                    break;
                                case 11:
                                    sysNumRN.c_bln11 = tmpNumRN;
                                    break;
                                case 12:
                                    sysNumRN.c_bln12 = tmpNumRN;
                                    break;
                            }
                        }
                    }
                    #endregion

                }

                #region send email item tidak terdaftar
                if (itemlist != null)
                { 
                    System.Net.Mail.SmtpClient smtpemail = null;
                    StringBuilder sbemail = new StringBuilder();

                    try
                    {
                        using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                        {
                            mail.From = new System.Net.Mail.MailAddress("scms.dophar@ams.co.id", "Supply Chain Management System");
                            mail.Subject = "Data DO Pharmanet yang gagal di create karena tidak ada item nya";
                            mail.To.Add("suwandi@ams.co.id");
                            mail.To.Add("indra.dwi@ams.co.id");
                            mail.To.Add("hafizh.ahmad@ams.co.id");
                            sbemail.AppendLine("Berikut dikirimkan kembali data ITEM yang tidak bisa dibuatkan pl karena item nya blm terdaftar");
                            sbemail.AppendLine("");
                            sbemail.AppendLine(itemlist);
                            mail.Body = sbemail.ToString();
                            smtpemail = new System.Net.Mail.SmtpClient(SMTP_IP, SMTP_PORT);
                            smtpemail.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                            smtpemail.UseDefaultCredentials = false;
                            smtpemail.Credentials = new System.Net.NetworkCredential("scms.dophar@ams.co.id", "scmsdophar");

                            smtpemail.Send(mail);
                            sbemail.Length = 0;
                        }
                    }
                    catch
                    { 
                    
                    }

                }

                #endregion

                if (db != null)
                {
                    if (nChanges > 0)
                    {
                        db.SubmitChanges();
                        db.Transaction.Commit();
                        if (listDOH.Count > 0)
                        {
                            for (nLoopFj = 0, nLoop = listDOH.Count; nLoopFj < nLoop; nLoopFj++)
                            {
                                //Commons2.RunningGenerateFJ(db, tmpCustomer, doID, doID, nipEntry);
                                Commons2.RunningGenerateFJ(db, listDOH[nLoopFj].c_cusno, listDOH[nLoopFj].c_dono, listDOH[nLoopFj].c_dono, nipEntry);
                            }
                            listDOH.Clear();
                        }

                        if (listReDoGroup.Count > 0)
                        {
                            for (nLoopFj = 0, nLoop = listReDoGroup.Count; nLoopFj < nLoop; nLoopFj++)
                            {
                                #region Faktur Jual
                                string noFj = string.Concat("FJ", listReDoGroup[nLoopFj].c_dono.Substring(2)).Trim();

                                fjh = (from q in db.LG_FJHs
                                       where q.c_fjno == noFj
                                       select q).SingleOrDefault();

                                if (fjh != null)
                                {
                                    db.LG_FJHs.DeleteOnSubmit(fjh);
                                }

                                lstFjd1 = (from q in db.LG_FJD1s
                                           where q.c_fjno == noFj
                                           select q).Distinct().ToList();

                                if (lstFjd1.Count > 0)
                                {
                                    db.LG_FJD1s.DeleteAllOnSubmit(lstFjd1.ToArray());
                                    lstFjd1.Clear();
                                }

                                lstFjd2 = (from q in db.LG_FJD2s
                                           where q.c_fjno == noFj
                                           select q).Distinct().ToList();

                                if (lstFjd2.Count > 0)
                                {
                                    db.LG_FJD2s.DeleteAllOnSubmit(lstFjd2.ToArray());
                                    lstFjd2.Clear();
                                }

                                fjd3 = (from q in db.LG_FJD3s
                                        where q.c_fjno == noFj
                                        select q).SingleOrDefault();

                                if (fjd3 != null)
                                {
                                    db.LG_FJD3s.DeleteOnSubmit(fjd3);
                                }

                                doh = (from q in db.LG_DOHs
                                       where q.c_dono == listReDoGroup[nLoopFj].c_dono
                                       select q).SingleOrDefault();

                                if (doh != null)
                                {
                                    doh.l_receipt = false;
                                }


                                db.SubmitChanges();


                                #endregion
                                //bool isSend = Commons2.PostDataDODirect(db, listReDoGroup[nLoopFj].c_dono, false, true);
                                Commons2.RunningGenerateFJ(db, listReDoGroup[nLoopFj].c_cusno, listReDoGroup[nLoopFj].c_dono, listReDoGroup[nLoopFj].c_dono, nipEntry);
                            }
                            listReDoGroup.Clear();
                        }

                        #region Send PO

                        SqlConnection cn = new SqlConnection(this.config.ConnectionString);
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = cn;

                        cn.Open();
                        cmd = new SqlCommand();
                        cmd.Connection = cn;
                        cmd.CommandText = strCommand;

                        DataTable dt = new DataTable(),
                          dt1 = new DataTable();
                        DataSet ds = new DataSet(),
                          ds1 = new DataSet();

                        string path = null,
                            supName = string.Empty,
                            supName2 = string.Empty;
                        if (kodePrinsipal.Equals("00001"))
                        {
                            path = @"D:\PHAROSGROUP\PHAROS\PHARMANET\";
                            supName = "PI";
                            supName2 = "PT. PHAROS INDONESIA";
                        }
                        else if (kodePrinsipal.Equals("00112"))
                        {
                            path = @"D:\PHAROSGROUP\NUTRINDO JAYA\PHARMANET\";
                            supName = "NJA";
                            supName2 = "PT. NUTRINDO JAYA ABADI";
                        }
                        else if (kodePrinsipal.Equals("00113"))
                        {
                            path = @"D:\PHAROSGROUP\NUTRISAINS\PHARMANET\";
                            supName = "NS";
                            supName2 = "PT. NUTRISAINS";
                        }
                        else if (kodePrinsipal.Equals("00117"))
                        {
                            path = @"D:\PHAROSGROUP\PRIMA MEDIKA\PHARMANET\";
                            supName = "PML";
                            supName2 = "PT. PRIMA MEDIKA LABORATORIES";
                        }
                        else if (kodePrinsipal.Equals("00120"))
                        {
                            path = @"D:\PHAROSGROUP\APEX\PHARMANET\";
                            supName = "APEX";
                            supName2 = "PT. APEX PHARMA INDONESIA";
                        }
                        else if (kodePrinsipal.Equals("00159") || kodePrinsipal.Equals("00165"))
                        {
                            path = @"D:\PHAROSGROUP\PERINTIS\PHARMANET\";
                            supName = "PERINTIS";
                            supName2 = "PT. PERINTIS PELAYANAN PARIPURNA";
                        }
                        else if (kodePrinsipal.Equals("00171"))
                        {
                            path = @"D:\PHAROSGROUP\SARUA SUBUR\PHARMANET\";
                            supName = "SSB";
                            supName2 = "PT. SARUA SUBUR";
                        }

                        if (!string.IsNullOrEmpty(path))
                        {
                            if (!string.IsNullOrEmpty(strCommand))
                            {
                                SqlDataReader reader1 = cmd.ExecuteReader();

                                if (reader1.HasRows)
                                {
                                    dt1.Load(reader1);

                                    ds1.Tables.Add(dt1);
                                }

                                reader1.Close();
                                reader1.Dispose();

                                bool isSukses = false;

                                if (ds1.Tables.Count > 0 && (!string.IsNullOrEmpty(path)) && (!string.IsNullOrEmpty(poID)))
                                {
                                    isSukses = ExportDBF(ds1, path, "dodetail_" + supName + dateNow.ToString("yyyMMdd") + "_" + dateNow.ToString("hhmmss"), false, false);
                                }
                            }
                        }

                        #endregion

                        #region Send DO Not Valid
                        if (lstDOPHErr1.Count > 0)
                        {
                            cmd = new SqlCommand();
                            cmd.Connection = cn;
                            cmd.CommandText = "select b.v_namatax [C_PT],a.c_dono [C_NODO],A.d_dodate [D_TGLDO],a.d_fjno [C_EXNOINV],a.d_fjdate [D_JTH], " +
                                "a.c_cab [C_KDCAB],a.c_via [C_VIA],a.c_taxno [C_NOTAX],a.c_po_outlet [C_PO_OUTLE],a.c_outlet [C_OUTLET], a.v_outlet [V_OUTLET], " +
                                "a.c_plphar [N_NOPL],a.C_KET from LG_DOPHErr1 a join LG_DatSup b on a.c_nosup = b.c_nosup";

                            SqlDataReader reader = cmd.ExecuteReader();

                            path = config.PathTempExtractMail + "DOPharErr\\";
                            string fileName = "DOHEADER_" + supName + dateError.ToString("yyyMMdd") + ".DBF";
                            if (!string.IsNullOrEmpty(path))
                            {
                                if (reader.HasRows)
                                {
                                    dt.Load(reader);

                                    ds.Tables.Add(dt);
                                }

                                reader.Close();
                                reader.Dispose();

                                bool isSukses = false;

                                if (ds.Tables.Count > 0 && (!string.IsNullOrEmpty(path)) && (!string.IsNullOrEmpty(poID)))
                                {
                                    isSukses = ExportDBF(ds, path, fileName, false, false);
                                }
                            }


                            #region Send Mail

                            System.Net.Mail.SmtpClient smtp = null;
                            StringBuilder sb = new StringBuilder();
                            try
                            {
                                using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                                {
                                    // send mail containing the file here

                                    mail.From = new System.Net.Mail.MailAddress("scms.dophar@ams.co.id", "Supply Chain Management System");

                                    mail.Subject = "Data DO Pharmanet (" + supName2 + ") Tanggal " + dateError.ToString("dd MMMM yyyy");

                                    //mail.To.Add("handry.wardoyo@ams.co.id");
                                    //mail.To.Add("akhmad.perdana@ams.co.id");
                                    //mail.To.Add("it-support@pharos.co.id");
                                    //mail.CC.Add("wahyuni@pharos.co.id");
                                    //mail.CC.Add("lianto@pharos.co.id");
                                    mail.CC.Add("jan@ams.co.id");
                                    mail.CC.Add("akhmad.perdana@ams.co.id");
                                    mail.CC.Add("suwandi@ams.co.id");

                                    //sb.AppendLine();
                                    //sb.AppendLine();

                                    sb.AppendLine("Berikut dikirimkan kembali data DO Pharmanet (" + supName2 + ") yang tidak valid.");
                                    sb.AppendLine();
                                    sb.AppendLine("Mohon untuk diperiksa dan dikirim kembali ke Kami dengan Outlet yang valid.");

                                    using (System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(path + fileName))
                                    {
                                        mail.Attachments.Add(attachment);

                                        sb.AppendLine();
                                        sb.AppendLine();
                                        sb.AppendLine("Terima Kasih,");
                                        sb.AppendLine("AMS - MIS Team");

                                        mail.Body = sb.ToString();

                                        smtp = new System.Net.Mail.SmtpClient(SMTP_IP, SMTP_PORT);

                                        smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                                        smtp.UseDefaultCredentials = false;
                                        smtp.Credentials = new System.Net.NetworkCredential("scms.dophar@ams.co.id", "scmsdophar");

                                        smtp.Send(mail);

                                        //mail.Dispose();
                                        sb.Length = 0;
                                    }
                                }

                                File.Delete(path + fileName);

                                //using (var stream = File.Open(path + fileName, FileMode.Open, FileAccess.Write, FileShare.Read))
                                //{
                                //    File.Delete(path + fileName);
                                //}

                                //if (Directory.Exists(path + fileName))
                                //{
                                //    Directory.Delete(path + fileName, true);
                                //}
                            }
                            catch (Exception ex)
                            {
                                Logger.WriteLine(ex.Message);
                                Logger.WriteLine(ex.StackTrace);
                            }
                            #endregion

                            lstDOPHErr1.Clear();
                        }
                        #endregion

                        #region Send DO Not Valid Price
                        if (lstDOPDErr1.Count > 0)
                        {
                            cmd = new SqlCommand();
                            cmd.Connection = cn;
                            cmd.CommandText = "select b.v_namatax [Principle],a.c_dono [No.DO],a.c_iteno [Kode Item AMS],a.c_itenopri[Kode Item Principle],a.v_itnam [Nama Item], " +
                                                "a.c_batch [Batch],a.d_expired [Expired],a.n_qty [Qty],a.n_disc,a.n_dprion,a.n_salpri [Harga AMS],a.n_salpri_phar [Harga Pharmanet], " +
                                                "a.c_po_outlet,a.c_outlet " +
                                                "from LG_DOPDErr1 a join LG_DatSup b on a.c_nosup = b.c_nosup";

                            DataTable tbl = new DataTable();
                            SqlDataAdapter adp = new SqlDataAdapter(cmd.CommandText, cn);
                            adp.Fill(tbl);

                            string fileName = "DOPHARMANET_" + supName + DateTime.Now.ToString("yyyMMdd") + ".xlsx";

                            path = config.PathTempExtractMail + "DOPharErr\\";

                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                wb.Worksheets.Add(tbl, "Beda Harga");
                                wb.SaveAs(path+fileName);
                            }


                            #region Send Mail

                            System.Net.Mail.SmtpClient smtp = null;
                            StringBuilder sb = new StringBuilder();
                            try
                            {
                                using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                                {
                                    // send mail containing the file here

                                    mail.From = new System.Net.Mail.MailAddress("scms.dophar@ams.co.id", "Supply Chain Management System");

                                    mail.Subject = "Data DO Pharmanet (" + supName2 + ") Tanggal " + DateTime.Now.ToString("dd MMMM yyyy");

                                    //mail.To.Add("handry.wardoyo@ams.co.id");
                                    //mail.To.Add("akhmad.perdana@ams.co.id");
                                    mail.To.Add("it-support@pharos.co.id");
                                    mail.To.Add("lianto@pharos.co.id");
                                    mail.CC.Add("timbul@pharos.co.id");
                                    mail.CC.Add("wahyuni@pharos.co.id");
                                    mail.CC.Add("jan@ams.co.id");
                                    mail.CC.Add("thiojerry@ams.co.id");
                                    mail.CC.Add("arfan@ams.co.id");
                                    mail.CC.Add("akhmad.perdana@ams.co.id");
                                    mail.CC.Add("suwandi@ams.co.id");

                                    sb.AppendLine("Berikut dikirimkan kembali data DO Pharmanet (" + supName2 + ") yang beda Harga antara PT. AMS dengan " + supName2 + ".");
                                    sb.AppendLine();
                                    sb.AppendLine("Mohon untuk diperiksa dan dikirim kembali ke Kami dengan Harga yang sesuai di PT.AMS (AntarMitra Sembada)");

                                    using (System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(path + fileName))
                                    {
                                        mail.Attachments.Add(attachment);

                                        sb.AppendLine();
                                        sb.AppendLine();
                                        sb.AppendLine("Terima Kasih,");
                                        sb.AppendLine("AMS - MIS Team");

                                        mail.Body = sb.ToString();

                                        smtp = new System.Net.Mail.SmtpClient(SMTP_IP, SMTP_PORT);

                                        smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                                        smtp.UseDefaultCredentials = false;
                                        smtp.Credentials = new System.Net.NetworkCredential("scms.dophar@ams.co.id", "scms");

                                        smtp.Send(mail);
                                        sb.Length = 0;
                                    }
                                }

                                File.Delete(path + fileName);
                            }
                            catch (Exception ex)
                            {
                                Logger.WriteLine(ex.Message);
                                Logger.WriteLine(ex.StackTrace);
                            }
                            #endregion

                            lstDOPHErr1.Clear();
                        }
                        #endregion

                        #region SENDDO
                        //int i = 0;
                        //for (i = 0; i < nomorDO.Count; i++)
                        //{
                        //    Commons2.PostDataDODirect(db, nomorDO[i], false, false);
                        //}
                        cn.Close();
                        #endregion
                        cn.Dispose();
                    }
                    else
                    {
                        nChanges += 1;
                        db.Transaction.Rollback();
                        System.Net.Mail.SmtpClient smtp = null;
                        StringBuilder sb = new StringBuilder();
                        using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                            {
                                // send mail containing the file here
                                mail.From = new System.Net.Mail.MailAddress("scms.dophar@ams.co.id", "Supply Chain Management System");
                                mail.Subject = "Data DO Pharmanet Tanggal " + dateError.ToString("dd MMMM yyyy") + " Putus pada saat pengiriman";
                                //mail.To.Add("handry.wardoyo@ams.co.id");
                                //mail.To.Add("akhmad.perdana@ams.co.id");
                                mail.To.Add("indra.dwi@ams.co.id");
                                mail.To.Add("hafizh.ahmad@ams.co.id");
                                mail.CC.Add("suwandi@ams.co.id");
                                //sb.AppendLine();
                                //sb.AppendLine();
                                sb.AppendLine("Data DO yang di kirim ke cabang putus pada saat pengiriman.");
                                sb.AppendLine();
                                sb.AppendLine("Mohon untuk cek DO yang belum di RN dan resend kembali");
                                sb.AppendLine();
                                sb.AppendLine();
                                sb.AppendLine("Terima Kasih,");
                                sb.AppendLine("AMS - MIS Team");
                                mail.Body = sb.ToString();
                                smtp = new System.Net.Mail.SmtpClient(SMTP_IP, SMTP_PORT);
                                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                                smtp.UseDefaultCredentials = false;
                                smtp.Credentials = new System.Net.NetworkCredential("scms.dophar@ams.co.id", "scmsdophar");
                                smtp.Send(mail);
                                //mail.Dispose();
                                sb.Length = 0;
                            }
                    }

                    //db.Dispose();
                }
            }
            catch (Exception ex)
            {
                nChanges = 1;
                if (db.Transaction != null)
                {
                    db.Transaction.Rollback();
                    System.Net.Mail.SmtpClient smtp = null;
                    StringBuilder sb = new StringBuilder();
                    using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                    {
                        // send mail containing the file here
                        mail.From = new System.Net.Mail.MailAddress("scms.dophar@ams.co.id", "Supply Chain Management System");
                        mail.Subject = "Data DO Pharmanet Tanggal " + dateError.ToString("dd MMMM yyyy") + " error pada saat input ke database";
                        //mail.To.Add("wahyuni@pharos.co.id");
                        mail.To.Add("indra.dwi@ams.co.id");
                        mail.To.Add("hafizh.ahmad@ams.co.id");
                        mail.CC.Add("suwandi@ams.co.id");
                        //sb.AppendLine();
                        //sb.AppendLine();
                        sb.AppendLine("Data PL yang di kirim oleh Pharos error pada saat input");
                        sb.AppendLine();
                        sb.AppendLine("Mohon untuk cek datanya dan kirim ke pharos");
                        sb.AppendLine();
                        sb.AppendLine(ex.Message.ToString());
                        sb.AppendLine();
                        sb.AppendLine("Terima Kasih,");
                        sb.AppendLine("AMS - MIS Team");
                        mail.Body = sb.ToString();
                        smtp = new System.Net.Mail.SmtpClient(SMTP_IP, SMTP_PORT);
                        smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new System.Net.NetworkCredential("scms.dophar@ams.co.id", "scmsdophar");
                        smtp.Send(mail);
                        //mail.Dispose();
                        sb.Length = 0;
                    }
                }
            }


            return nChanges;
        }
        
        //pharmanet sistem lama - tidak di pakai lagi

        private bool ExportDBF(System.Data.DataSet dsExport, string folderPath, string sNama, bool isHeader, bool isText)
        {
            string tableName = sNama;
            bool returnStatus = false;

            try
            {
                string createStatement = "Create Table " + tableName + " ( ";
                string insertStatement = "Insert Into " + tableName + " Values ( ";
                string insertTemp = string.Empty;
                OleDbCommand cmd = new OleDbCommand();
                OleDbConnection conn = null;
                if (dsExport.Tables[0].Columns.Count <= 0) { throw new Exception(); }

                StringBuilder sb = new StringBuilder();
                int nLoop = 0,
                  nLen = 0,
                  nLenC = 0,
                  nLoopC = 0;
                System.Data.DataColumn col = null;
                System.Data.DataRow row = null;
                string reslt = null,
                    dateStr = null;

                string sFile = folderPath + sNama + ".dbf";

                bool bData = false;
                DateTime d_corda;

                if (!isText)
                {
                    #region Create Table

                    conn = new System.Data.OleDb.OleDbConnection(string.Format("Provider=vfpoledb;Data Source='{0}';Collating Sequence=general;", folderPath));
                    conn.Open();

                    cmd = conn.CreateCommand();

                    DataTable table = dsExport.Tables[0];

                    sb.AppendFormat("CREATE TABLE {0} (", tableName);

                    for (nLoop = 0, nLen = table.Columns.Count; nLoop < nLen; nLoop++)
                    {
                        if ((nLoop + 1) >= nLen)
                        {
                            sb.AppendFormat(" {0}", DbfColumnParser(table.Columns[nLoop], table.Columns[nLoop].Caption));
                        }
                        else
                        {
                            sb.AppendFormat(" {0},", DbfColumnParser(table.Columns[nLoop], table.Columns[nLoop].Caption));
                        }
                    }
                    sb.Append(" )");

                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();

                    cmd.Dispose();

                    sb.Remove(0, sb.Length);

                    #endregion

                    #region Populate Data

                    cmd = conn.CreateCommand();

                    nLenC = table.Columns.Count;

                    for (nLoopC = 0; nLoopC < nLenC; nLoopC++)
                    {
                        col = table.Columns[nLoopC];

                        reslt = string.Concat(reslt, ",", col.ColumnName);
                    }

                    reslt = (reslt.StartsWith(",", StringComparison.OrdinalIgnoreCase) ? reslt.Remove(0, 1) : reslt);

                    for (nLoop = 0, nLen = table.Rows.Count; nLoop < nLen; nLoop++)
                    {
                        row = table.Rows[nLoop];

                        sb.AppendFormat("Insert Into {0} ({1}) Values (", tableName, reslt);

                        for (nLoopC = 0; nLoopC < nLenC; nLoopC++)
                        {
                            col = table.Columns[nLoopC];

                            if (col.DataType.Equals(typeof(float)) ||
                               col.DataType.Equals(typeof(double)) ||
                               col.DataType.Equals(typeof(decimal)))
                            {
                                sb.AppendFormat("{0} ,", decimal.Parse(row[col].ToString()));
                            }
                            else if (col.DataType.Equals(typeof(ushort)) ||
                               col.DataType.Equals(typeof(short)) ||
                               col.DataType.Equals(typeof(uint)) ||
                               col.DataType.Equals(typeof(int)) ||
                               col.DataType.Equals(typeof(ulong)) ||
                               col.DataType.Equals(typeof(long)))
                            {
                                sb.AppendFormat("{0} ,", int.Parse(row[col].ToString()));
                            }
                            else if (col.DataType.Equals(typeof(DateTime)))
                            {
                                d_corda = DateTime.Parse(row[col].ToString());
                                sb.AppendFormat("Date({0},{1},{2}) ,", d_corda.Year, d_corda.Month, d_corda.Day);
                            }
                            else if (col.DataType.Equals(typeof(bool)))
                            {
                                bData = bool.Parse(row[col].ToString());
                                sb.AppendFormat("{0} ,", (bData ? ".t." : ".f."));
                                //sb.AppendFormat("NULL ,", (bData ? 1 : 0));
                            }
                            else
                            {
                                sb.AppendFormat("'{0}' ,", row[col]);
                            }
                        }

                        sb.Remove(sb.Length - 1, 1);

                        sb.AppendLine(" ) ");

                        cmd.CommandText = sb.ToString();

                        cmd.ExecuteNonQuery();

                        sb.Remove(0, sb.Length);
                    }


                    #endregion
                    cmd.Dispose();
                    conn.Close();
                    conn.Dispose();
                }
                else
                {
                    DataTable dt = dsExport.Tables[0];

                    dt.Columns.Remove("c_corno");
                    dt.Columns.Remove("c_iteno");
                    dt.Columns.Remove("c_itenopri");
                    dt.Columns.Remove("c_nosp");
                    dt.Columns.Remove("c_via");

                    int[] maxLengths = new int[dt.Columns.Count];

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        maxLengths[i] = dt.Columns[i].ColumnName.Length;

                        foreach (DataRow rows in dt.Rows)
                        {
                            if (!rows.IsNull(i))
                            {
                                int length = rows[i].ToString().Length;

                                if (length > maxLengths[i])
                                {
                                    maxLengths[i] = length;
                                }
                            }
                        }
                    }

                    using (StreamWriter sw = new StreamWriter(folderPath + sNama, false))
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            sw.Write(dt.Columns[i].ColumnName.PadRight(maxLengths[i] + 2));
                        }

                        sw.WriteLine();

                        foreach (DataRow rows in dt.Rows)
                        {
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                if (!rows.IsNull(i))
                                {
                                    sw.Write(rows[i].ToString().PadRight(maxLengths[i] + 2));
                                }
                                else
                                {
                                    sw.Write(new string(' ', maxLengths[i] + 2));
                                }
                            }

                            sw.WriteLine();
                        }

                        sw.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                string ss = ex.Message;
            }
            return returnStatus = true;
        }

        private static string DbfColumnParser(System.Data.DataColumn column, string caption)
        {
            string rets = null;

            if (column.DataType.Equals(typeof(float)) ||
              column.DataType.Equals(typeof(double)) ||
              column.DataType.Equals(typeof(decimal)))
            {
                int i = 18;
                switch (caption.ToLower())
                {
                    case "n_qtyrcv":
                    case "n_disc":
                    case "n_salpri":
                    case "n_qtybns":
                    case "n_dprion":
                    case "n_dprioff":
                    case "n_damson":
                    case "n_damsoff":
                        i = 10;
                        break;
                }
                rets = string.Format("[{0}] numeric({1},2) {2}",
                  column.ColumnName, i,
                  (column.AllowDBNull ? "NULL" : "NOT NULL"));
            }
            else if (column.DataType.Equals(typeof(ushort)) ||
              column.DataType.Equals(typeof(short)) ||
              column.DataType.Equals(typeof(uint)) ||
              column.DataType.Equals(typeof(int)) ||
              column.DataType.Equals(typeof(ulong)) ||
              column.DataType.Equals(typeof(long)))
            {
                rets = string.Format("[{0}] int {1}",
                  column.ColumnName,
                  (column.AllowDBNull ? "NULL" : "NOT NULL"));
            }
            else if (column.DataType.Equals(typeof(bool)))
            {
                rets = string.Format("[{0}] logical {1}",
                  column.ColumnName,
                  (column.AllowDBNull ? "NULL" : "NOT NULL"));
            }
            else if (column.DataType.Equals(typeof(DateTime)))
            {
                rets = string.Format("[{0}] date {1}",
                  column.ColumnName,
                  (column.AllowDBNull ? "NULL" : "NOT NULL"));
            }
            else
            {
                int i = 1;
                switch (caption.ToLower())
                {
                    case "c_pt":
                    case "c_itnam":
                    case "v_outlet":
                        i = 50;
                        break;
                    case "c_undes":
                    case "c_ket":
                        i = 25;
                        break;
                    case "c_nodpf":
                    case "c_nosp":
                        i = 15;
                        break;
                    case "c_iteno":
                        i = 4;
                        break;
                    case "c_itenopri":
                    case "c_outlet":
                        i = 6;
                        break;
                    case "c_batch":
                        i = 8;
                        break;
                    case "c_po_outle":
                    case "c_notax":
                    case "n_nopl":
                        i = 10;
                        break;
                    case "c_nodo":
                    case "c_exnoinv":
                        i = 12;
                        break;
                    case "c_kdcab":
                    case "c_via":
                        i = 2;
                        break;
                }
                rets = string.Format("[{0}] char({1}) {2}",
                  column.ColumnName, i,
                  (column.AllowDBNull ? "NULL" : "NOT NULL"));
            }

            return rets;
        }

        private static bool IsNumeric(string s)
        {
            double Result;
            return double.TryParse(s, out Result);
        }

        private static string ChangeAlphabet(string alphabet)
        {
            string abjad = string.Empty;
            switch (alphabet)
            {
                case "A":
                    abjad = "B";
                    break;
                case "B":
                    abjad = "C";
                    break;
                case "C":
                    abjad = "D";
                    break;
                case "D":
                    abjad = "E";
                    break;
                case "E":
                    abjad = "F";
                    break;
                case "F":
                    abjad = "G";
                    break;
                case "G":
                    abjad = "H";
                    break;
                case "H":
                    abjad = "I";
                    break;
                case "I":
                    abjad = "J";
                    break;
                case "J":
                    abjad = "K";
                    break;
                case "K":
                    abjad = "L";
                    break;
                case "L":
                    abjad = "M";
                    break;
                case "M":
                    abjad = "N";
                    break;
                case "N":
                    abjad = "O";
                    break;
                case "O":
                    abjad = "P";
                    break;
                case "P":
                    abjad = "Q";
                    break;
                case "Q":
                    abjad = "R";
                    break;
                case "R":
                    abjad = "S";
                    break;
                case "S":
                    abjad = "T";
                    break;
                case "T":
                    abjad = "U";
                    break;
                case "U":
                    abjad = "V";
                    break;
                case "V":
                    abjad = "W";
                    break;
                case "W":
                    abjad = "X";
                    break;
                case "X":
                    abjad = "Y";
                    break;
                case "Y":
                    abjad = "Z";
                    break;
            }
            return abjad;
        }
        #region Folder Monitor
// tidak di pakai per november 2017
        private void StartingFolderReader()
        {
            if (string.IsNullOrEmpty(MonitoringFolder))
            {
                return;
            }

            isStart = true;

            ScmsMailLibrary.Pop3MailerReader.TaskInfo ti = new ScmsMailLibrary.Pop3MailerReader.TaskInfo();

            ti.Handle = System.Threading.ThreadPool.RegisterWaitForSingleObject(areEvent,
              new WaitOrTimerCallback(WoTCallback), ti,
              this.TimePeriodic, false);
        }

        private void FolderReader()
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(this.MonitoringFolder);
            System.IO.FileInfo[] fis = null;
            System.IO.FileInfo fi = null;
            System.IO.FileStream fs = null;
            byte[] byts = null;

            System.Data.DataSet dataset = null;

            ORMDataContext db = null;
            int nChanges = 0,
              lenData = 0,
              totalSaved = 0;

            DateTime dt = DateTime.Now;
            string jam = dt.ToString("HH");
            if (jam == "07" || jam == "10" || jam == "22")
            //if (jam == "07" || jam == "09" || jam == "14" || jam == "15")
            {
                if (di.Exists)
                {
                    if (!System.IO.Directory.Exists(this.TempPath))
                    {
                        System.IO.Directory.CreateDirectory(this.TempPath);
                    }

                    try
                    {
                        db = new ORMDataContext(this.config.ConnectionString);

                        fis = di.GetFiles("*.zip");

                        for (int nLoop = 0, nLen = fis.Length; nLoop < nLen; nLoop++)
                        {
                            totalSaved = 0;

                            fi = fis[nLoop];

                            #region Stream

                            fs = fi.OpenRead();

                            fs.Position = 0;

                            lenData = (int)fs.Length;

                            byts = new byte[lenData];

                            fs.Read(byts, 0, lenData);

                            fs.Close();

                            fs.Dispose();

                            #endregion

                            #region Extract Data

                            dataset = SaveToTemplateAndExtract(this.TempPath, byts);

                            if ((dataset != null) && (dataset.Tables.Count == 2))
                            {
                                totalSaved = insertintotemp(dataset, dicMappingPrinsipal, db);
                                //totalSaved = PopulateDoHeaderDetailPI(dataset, dicMappingPrinsipal, db);
                            }

                            Array.Clear(byts, 0, lenData);

                            if (dataset != null)
                            {
                                dataset.Clear();
                                dataset.Dispose();
                            }

                            #endregion

                            nChanges += totalSaved;

                            try
                            {
                                if (nChanges > 0)
                                {
                                    fi.Delete();
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.WriteLine(
                                  string.Format("ScmsMailLibrary.DOPharmanetReader:DOPharmanetReader DeleteFile - {0}", ex.Message));

                                Logger.WriteLine(ex.StackTrace);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine(
                         string.Format("ScmsMailLibrary.DOPharmanetReader:FolderReader - {0}", ex.Message));

                        Logger.WriteLine(ex.StackTrace);
                    }

                    //if (db != null)
                    //{
                    //    if (nChanges > 0)
                    //    {
                    //        db.SubmitChanges();
                    //    }

                    //    db.Dispose();
                    //}
                }
            }
        }
// tidak di pakai per november 2017
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
                            this.FolderReader();
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
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
                Logger.WriteLine(ex.StackTrace);
            }

            isRunning = false;
        }

        #endregion
    }
}
