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
using ScmsSoaLibraryInterface.Components;
using System.Data.SqlClient;

namespace ScmsSoaLibrary.Bussiness
{
    class Pembelian
    {
        #region Internal Class

        internal class SPAdminComponent
        {
            public string RefID { get; set; }
            public string SignID { get; set; }
            public string Item { get; set; }
            public string Batch { get; set; }
            public decimal Qty { get; set; }
            public string Supplier { get; set; }
            public decimal Box { get; set; }
        }

        internal class ORHelper
        {
            public string c_iteno { get; set; }
            public string v_itnam { get; set; }
            public decimal n_pminord { get; set; }
            public decimal n_qminord { get; set; }
            public string c_type { get; set; }
            public string c_via { get; set; }
            public decimal n_beli { get; set; }
            public decimal n_box { get; set; }
            public decimal n_salpri { get; set; }
            public decimal n_index { get; set; }
            public string c_nosup { get; set; }
            public string c_kddivpri { get; set; }
            public string v_nmdivpri { get; set; }
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

        #endregion

        #region Private

        //private void PostDataReplySP(string connectionString, ScmsSoaLibrary.Parser.Class.SuratPesananResponse strt, string spNo)
        //{
        //    string dataResult = ScmsSoaLibrary.Parser.Class.SuratPesananResponse.Serialize(strt);

        //    Commons.RunningSendingReplySP(connectionString, dataResult, spNo);
        //}

        //private void PostDataReplySPM(string connectionString, ScmsSoaLibrary.Parser.Class.SuratPesananResponse strt, string spNo)
        //{
        //    string dataResult = ScmsSoaLibrary.Parser.Class.SuratPesananResponse.Serialize(strt);

        //    Commons.RunningSendingReplySPM(connectionString, dataResult, spNo);
        //}

        public string PostDataReplySPM(string connectionString, ScmsSoaLibrary.Parser.Class.SuratPesananResponse strt, string spNo)
        {
            ORMDataContext db = new ORMDataContext(connectionString);
            string dataResult = ScmsSoaLibrary.Parser.Class.SuratPesananResponse.Serialize(strt);

            string bResult = null;

            Config cfg = Functionals.Configuration;

            Dictionary<string, string> dicParam = new Dictionary<string, string>();
            dicParam.Add("param", dataResult);

            Dictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add("X-Requested-With", "XMLHttpRequest");

            IDictionary<string, object> dic = new Dictionary<string, object>();

            ScmsSoaLibrary.Parser.ResponseParser res = new ScmsSoaLibrary.Parser.ResponseParser();

            ScmsSoaLibrary.Core.Response.DiscoreResponse ds = new ScmsSoaLibrary.Core.Response.DiscoreResponse();

            ScmsSoaLibrary.Parser.ParserDisCore pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

            pdc.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            pdc.Referer = Functionals.DistCoreUrlBuilderString(cfg, "http://10.100.10.28/dcore/?m=com.ams.welcome");

            Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.28/dcore/?m=com.ams.json.ds&action=form&f=Submit&_q=insert_spm");

            string result = null;

            bool isError = false;

            StringBuilder sb = new StringBuilder();

            Encoding utf8 = Encoding.UTF8;

            DateTime date = DateTime.MinValue;

            sb.AppendLine(uri.ToString());
            sb.AppendLine(dataResult);

            Logger.WriteLine(uri.ToString());

            if (pdc.PostGetData(uri, dicParam, dicHeader))
            {
                result = utf8.GetString(pdc.Result);
                if (result.Contains("success"))
                {
                    bResult = "success";
                }
                else
                {
                    bResult = "Error";
                }
            }
            else
            {
                result = pdc.ErrorMessage;
                isError = true;
            }

            Logger.WriteLine(result);

            //Bussiness.Commons.InsertReceivedRespose(db, sb.ToString(), result, isError, false, "SP", spNo);

            dic.Clear();

            dicHeader.Clear();
            dicParam.Clear();

            db.Dispose();

            return bResult;
        }
        //penambahan untuk SP reply dari dcore suwandi 14 November 2018
        private string PostDataReplySPETA(string connectionString, ScmsSoaLibrary.Parser.Class.SuratPesananResponse strt, string spNo)
        {
            string dataResult = ScmsSoaLibrary.Parser.Class.SuratPesananResponse.Serialize(strt);

            //Commons.RunningSendingReplySPETA(connectionString, dataResult, spNo);
            ORMDataContext db = new ORMDataContext(connectionString);

            bool bResult = false;
            string cek = null;

            Config cfg = Functionals.Configuration;

            Dictionary<string, string> dicParam = new Dictionary<string, string>();
            dicParam.Add("param", dataResult);

            Dictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add("X-Requested-With", "XMLHttpRequest");

            IDictionary<string, string> dic = new Dictionary<string, string>();

            ScmsSoaLibrary.Parser.ResponseParser res = new ScmsSoaLibrary.Parser.ResponseParser();

            ScmsSoaLibrary.Core.Response.DiscoreResponse ds = new ScmsSoaLibrary.Core.Response.DiscoreResponse();

            ScmsSoaLibrary.Parser.ParserDisCore pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

            pdc.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            pdc.Referer = Functionals.DistCoreUrlBuilderString(cfg, "http://10.100.10.28/dcore/?m=com.ams.welcome");

            //Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.28/dcore/index.php?m=com.ams.json.ds&f=Submit&_q=trx_spi_change_eta");
            Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.35/dcore/index.php?m=com.ams.json.ds&f=Submit&_q=trx_spi_change_eta");

            string result = null;

            bool isError = false;

            StringBuilder sb = new StringBuilder();

            Encoding utf8 = Encoding.UTF8;

            DateTime date = DateTime.MinValue;

            sb.AppendLine(uri.ToString());
            sb.AppendLine(dataResult);

            Logger.WriteLine(uri.ToString());

            if (pdc.PostGetData(uri, dicParam, dicHeader))
            {
                result = utf8.GetString(pdc.Result);

                Logger.WriteLine(result, true);

                dic = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCoreSwitch(result);

                if (result.Contains("success"))
                {
                    bResult = true;
                    cek = "success";
                    cek = cek + dic.GetValueParser<string, string, string>("D_ETA", string.Empty);
                }
                else if (!result.Contains("success"))
                {
                    bResult = false;
                    cek = "gagal";
                }
            }
            else
            {
                result = pdc.ErrorMessage;
                isError = true;
            }

            Logger.WriteLine(result);

            Bussiness.Commons.InsertReceivedRespose(db, sb.ToString(), result, isError, false, "SP", spNo);

            dic.Clear();

            dicHeader.Clear();
            dicParam.Clear();

            db.Dispose();

            return cek;

        }

        #endregion

        public string CancelPharmanet(ScmsSoaLibrary.Parser.Class.DOPharmanetStructure structure)
        {
            return CancelPharmanet(structure, null, false);
        }

        public string CancelPharmanet(ScmsSoaLibrary.Parser.Class.DOPharmanetStructure structure, ORMDataContext dbContext, bool isRemoveSisa)
        {
            if (structure == null || structure.Fields == null)
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }
            ScmsSoaLibrary.Parser.Class.DOPharmanetStructure field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            IDictionary<string, string> dic = null;
            string result = null;
            string nipEntry = null;
            string cabang = structure.Fields.Customer;
            string NoPL = null;
            string emailksa = null;
            string emailksl = null;
            string emailkacab = null;
            nipEntry = structure.Fields.Entry;
            ORMDataContext db = null;
            bool isContexted = false;
            string PO = structure.Fields.DOPharmanetID;
            if (dbContext == null)
            {
                db = new ORMDataContext(Functionals.ActiveConnectionString);
            }
            else
            {
                isContexted = true;
                db = dbContext;
            }
            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endlogic;
            }
            if (PO == null)
            {
                result = "Belum pilih nomor DO.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endlogic;
            }

            emailkacab = (from q in db.Temp_emails
                          where q.c_cusno == cabang && q.group_mail == "kacab"
                          select q.nv_email_cbg).SingleOrDefault();
            emailksa = (from q in db.Temp_emails
                        where q.c_cusno == cabang && q.group_mail == "ksa"
                        select q.nv_email_cbg).SingleOrDefault();
            emailksl = (from q in db.Temp_emails
                        where q.c_cusno == cabang && q.group_mail == "ksl"
                        select q.nv_email_cbg).SingleOrDefault();

            cabang = (from q in db.LG_Cusmas
                      where q.c_cusno == cabang
                      select q.v_cunam).SingleOrDefault();

            NoPL = (from q in db.Temp_LG_DOPHs
                    where q.c_po_outlet == PO
                    select q.c_plphar).SingleOrDefault();

            Temp_LG_DOPH TmpLGDOPH = null;

            List<Temp_LG_DOPH> LstTmpLGDOPH = null;

            try
            {
                db.Connection.Open();
                db.Transaction = db.Connection.BeginTransaction();

                LstTmpLGDOPH = (from q in db.Temp_LG_DOPHs
                                    where q.c_po_outlet == PO
                                    select q).ToList();
                if (LstTmpLGDOPH != null)
                {
                    db.ExecuteCommand("update temp_lg_doph set status = '30' where c_po_outlet = '" + PO + "' and status <> '20'");
                        //scms_limitpri = listLimitPri.Find(delegate(SCMS_LIMITPRI limitpri)
                        //            {
                        //                return field.nosup.Equals((string.IsNullOrEmpty(limitpri.c_nosup) ? string.Empty : limitpri.c_nosup.Trim()));
                        //            });
                    db.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                rpe = ResponseParser.ResponseParserEnum.IsFailed;
                goto endlogic;
            }

            #region Email ke pharmanet
            //System.Net.Mail.SmtpClient smtp = null;
            //StringBuilder sb = new StringBuilder();
            //try
            //{
            //    using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
            //    {
            //        // send mail containing the file here

            //        mail.From = new System.Net.Mail.MailAddress("scms.sph@ams.co.id", "Supply Chain Management System");

            //        mail.Subject = "Laporan Data Pharmanet PL di cancel";

            //        mail.To.Add("it-support@pharos.co.id");
            //        mail.To.Add("lianto@pharos.co.id");
            //        mail.To.Add("haes@pharos.co.id");
            //        mail.To.Add("betriani_s@pharos.co.id");
            //        mail.CC.Add("timbul@pharos.co.id");
            //        mail.CC.Add("jessy@pharos.co.id");
            //        mail.CC.Add("wahyuni@pharos.co.id");
            //        mail.CC.Add("agung_imawan@pharos.co.id");
            //        mail.CC.Add("thiojerry@ams.co.id");
            //        mail.CC.Add("ida.widyastuti@ams.co.id");
            //        mail.To.Add("hafizh.ahmad@ams.co.id");
            //        mail.To.Add("suwandi@ams.co.id");
            //        mail.To.Add("indra.dwi@ams.co.id");
            //        mail.To.Add(emailkacab);
            //        mail.To.Add(emailksa);
            //        mail.To.Add(emailksl);
            //        sb.AppendLine("Dear Team Pharmanet,");
            //        sb.AppendLine("");
            //        sb.AppendLine("Data PL " + NoPL + " untuk cabang " + cabang + " telah di cancel, karena: ");
            //        sb.AppendLine("");
            //        sb.AppendLine(structure.Fields.Keterangan);
            //        sb.AppendLine("");
            //        sb.AppendLine("Terima Kasih,");
            //        sb.AppendLine("AMS - MIS Team");

            //        mail.Body = sb.ToString();

            //        smtp = new System.Net.Mail.SmtpClient("10.100.10.9", 25);

            //        smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            //        smtp.UseDefaultCredentials = false;
            //        smtp.Credentials = new System.Net.NetworkCredential("scms.sph@ams.co.id", "scms");

            //        smtp.Send(mail);
            //        sb.Length = 0;
            //    }
            //    rpe = ResponseParser.ResponseParserEnum.IsSuccess;
            //    result = "Email verifikasi berhasil terkirim. Silakan cek email anda.";
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    Console.WriteLine(ex.StackTrace);
            //}
            #endregion

        endlogic:
            if (dic != null)
            {
                dic.Clear();
            }

            result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

            return result;
        }

        public string VerifikasiPharmanet(ScmsSoaLibrary.Parser.Class.DOPharmanetStructure structure)
        {
            return VerifikasiPharmanet(structure, null,false);
        }

        public string VerifikasiPharmanet(ScmsSoaLibrary.Parser.Class.DOPharmanetStructure structure, ORMDataContext dbContext, bool isRemoveSisa)
        {
            if (structure == null || structure.Fields == null)
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }
            ScmsSoaLibrary.Parser.Class.DOPharmanetStructure field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            IDictionary<string, string> dic = null;
            string result = null;
            string nipEntry = null;
            string cabang = structure.Fields.Customer;
            string NoPL = null;
            string emailksa = null;
            string emailksl = null;
            string emailkacab = null;
            nipEntry = structure.Fields.Entry;
            ORMDataContext db = null;
            bool isContexted = false;
            string PO = structure.Fields.DOPharmanetID;
            if (dbContext == null)
            {
                db = new ORMDataContext(Functionals.ActiveConnectionString);
            }
            else
            {
                isContexted = true;
                db = dbContext;
            }
            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endlogic;
            }

            emailkacab = (from q in db.Temp_emails
                          where q.c_cusno == cabang && q.group_mail == "kacab"
                          select q.nv_email_cbg).SingleOrDefault();
            emailksa = (from q in db.Temp_emails
                          where q.c_cusno == cabang && q.group_mail == "ksa"
                          select q.nv_email_cbg).SingleOrDefault();
            emailksl = (from q in db.Temp_emails
                          where q.c_cusno == cabang && q.group_mail == "ksl"
                          select q.nv_email_cbg).SingleOrDefault();

            cabang = (from q in db.LG_Cusmas
                      where q.c_cusno == cabang
                      select q.v_cunam).SingleOrDefault();

            NoPL = (from q in db.Temp_LG_DOPHs
                        where q.c_po_outlet == PO && q.Status == "10"
                        select q.c_plphar).SingleOrDefault();
            db.ExecuteCommand("update temp_lg_doph set status = '40' where c_po_outlet = '" + PO + "' and status = '10'");
            
            #region Email ke pharmanet
        //System.Net.Mail.SmtpClient smtp = null;
            //StringBuilder sb = new StringBuilder();
            //try
            //{
            //    using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
            //        {
            //            // send mail containing the file here

            //            mail.From = new System.Net.Mail.MailAddress("scms.sph@ams.co.id", "Supply Chain Management System");

            //            mail.Subject = "Laporan Data Pharmanet tidak sesuai";

            //            mail.To.Add("it-support@pharos.co.id");
            //            mail.To.Add("lianto@pharos.co.id");
            //            mail.To.Add("haes@pharos.co.id");
            //            mail.To.Add("betriani_s@pharos.co.id");
            //            mail.CC.Add("timbul@pharos.co.id");
            //            mail.CC.Add("jessy@pharos.co.id");
            //            mail.CC.Add("wahyuni@pharos.co.id");
            //            mail.CC.Add("agung_imawan@pharos.co.id");
            //            mail.CC.Add("thiojerry@ams.co.id");
            //            mail.CC.Add("ida.widyastuti@ams.co.id");
            //            mail.To.Add("hafizh.ahmad@ams.co.id");
            //            mail.To.Add("suwandi@ams.co.id");
            //            mail.To.Add("indra.dwi@ams.co.id");
            //            mail.To.Add(emailkacab);
            //            mail.To.Add(emailksa);
            //            mail.To.Add(emailksl);
            //            sb.AppendLine("Dear Team Pharmanet,");
            //            sb.AppendLine("");
            //            sb.AppendLine("Mohon di periksa kembali Data PL " + NoPL + " untuk cabang " + cabang + " karena: ");
            //            sb.AppendLine("");
            //            sb.AppendLine(structure.Fields.Keterangan);
            //            sb.AppendLine("");
            //            sb.AppendLine("Apabila dalam 2 hari kerja tidak ada perbaikan maka PL dicancel, dan barang dikembalikan ke Pharmanet melalui AMS HO.");
            //            sb.AppendLine("");
            //            sb.AppendLine("Terima Kasih,");
            //            sb.AppendLine("AMS - MIS Team");

            //            mail.Body = sb.ToString();

            //            smtp = new System.Net.Mail.SmtpClient("10.100.10.9", 25);

            //            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            //            smtp.UseDefaultCredentials = false;
            //            smtp.Credentials = new System.Net.NetworkCredential("scms.sph@ams.co.id", "scms");

            //            smtp.Send(mail);
            //            sb.Length = 0;
            //        }
            //    rpe = ResponseParser.ResponseParserEnum.IsSuccess;
            //    result = "Email verifikasi berhasil terkirim. Silakan cek email anda.";
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //        Console.WriteLine(ex.StackTrace);
        //    }
            #endregion
        endlogic:
            if (dic != null)
            {
                dic.Clear();
            }

            result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

            return result;
        }

        public string SaveBatch(ScmsSoaLibrary.Parser.Class.DOPharmanetStructure structure)
        {
            return SaveBatch(structure, null, false);
        }

        public string SaveBatch(ScmsSoaLibrary.Parser.Class.DOPharmanetStructure structure, ORMDataContext dbContext, bool isRemoveSisa)
        {
            if (structure == null || structure.Fields == null)
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }
            ScmsSoaLibrary.Parser.Class.DOPharmanetStructure field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            List<ScmsSoaLibrary.Parser.Class.DOPharmanetJSONStructureField> listSPJsonField = null;
            IDictionary<string, string> dic = null;
            string result = null;
            string nipEntry = null, batch = null, batchterima = null;
            nipEntry = structure.Fields.Entry;
            batch = structure.Fields.Customer;
            batchterima = structure.Fields.SPCabang;
            ORMDataContext db = null;
            ORMDataContext dbSP = null;
            bool isContexted = false;

            if (dbContext == null)
            {
                db = new ORMDataContext(Functionals.ActiveConnectionString);
            }
            else
            {
                isContexted = true;
                db = dbContext;
            }
            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";
                rpe = ResponseParser.ResponseParserEnum.IsFailed;
            }

            try
            {
                if (result == null)
                {
                    db.Connection.Open();
                    db.Transaction = db.Connection.BeginTransaction();
                    string tes = null;
                    tes = structure.Fields.DOPharmanetID;
                    var cek = (from q in db.LG_DOHs
                               where
                                   q.c_po_outlet == tes
                               select q.c_dono).SingleOrDefault();
                    if (cek == null)
                    {
                        if (tes != null)
                        {
                            try
                            {
                                db.ExecuteCommand("update temp_lg_dopd set nv_batchterima = '" + batchterima + "' where c_batch = '" + batch + "' and c_po_outlet = '" + tes + "'");
                                db.Transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                rpe = ResponseParser.ResponseParserEnum.IsFailed;
                                result = "Gagal update Batch." + batch + ex.Message;
                                db.Transaction.Rollback();
                                goto endlogic;
                            }
                        }
                        rpe = ResponseParser.ResponseParserEnum.IsSuccess;
                        result = "DO berhasil di bentuk";
                    }
                    else
                    {
                        result = "Data sudah pernah di proses. Tidak bisa di proses lagi.";
                        rpe = ResponseParser.ResponseParserEnum.IsFailed;
                        goto endlogic;
                    }
                }
            }
            catch (Exception Ex)
            {
                result = Ex.Message;
                rpe = ResponseParser.ResponseParserEnum.IsFailed;
            }

        endlogic:
            if (dic != null)
            {
                dic.Clear();
            }

            result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

            return result;
        }

        public string ProcessPharmanet(ScmsSoaLibrary.Parser.Class.DOPharmanetStructure structure)
        {
            return ProcessPharmanet(structure, null, false);
        }

        public string ProcessPharmanet(ScmsSoaLibrary.Parser.Class.DOPharmanetStructure structure, ORMDataContext dbContext, bool isRemoveSisa)
        {
            if (structure == null || structure.Fields == null)
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }
            ScmsSoaLibrary.Parser.Class.DOPharmanetStructure field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            List<ScmsSoaLibrary.Parser.Class.DOPharmanetJSONStructureField> listSPJsonField = null; 
            IDictionary<string, string> dic = null;
            string result = null;
            string nipEntry = null;
            nipEntry = structure.Fields.Entry;
            ORMDataContext db = null;
            ORMDataContext dbSP = null;
            bool isContexted = false;
            string emailksa = null;
            string emailksl = null;
            string emailkacab = null;
            string cabang = structure.Fields.Customer;
            string NoPL = null;
            string PO = structure.Fields.DOPharmanetID;

            if (dbContext == null)
            {
                db = new ORMDataContext(Functionals.ActiveConnectionString);
            }
            else
            {
                isContexted = true;
                db = dbContext;
            }
            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";
                rpe = ResponseParser.ResponseParserEnum.IsFailed;
            }

            listSPJsonField = new List<ScmsSoaLibrary.Parser.Class.DOPharmanetJSONStructureField>();



            try
            {
                if (result == null)
                {
                    if (db.Connection.State == System.Data.ConnectionState.Closed) db.Connection.Open();
                    db.Transaction = db.Connection.BeginTransaction();
                    string tes = null;
                    tes = structure.Fields.DOPharmanetID;
                    var cek = (from q in db.LG_DOHs where 
                                   q.c_po_outlet == tes
                                   select q.c_dono).SingleOrDefault();
                    if (cek == null)
                    {
                        if (tes != null)
                        {
                            try
                                {
                                    var res = db.SP_DOPHARMANET_EXEC(tes).SingleOrDefault();
                                    rpe = ResponseParser.ResponseParserEnum.IsSuccess;
                                    db.Transaction.Commit();
                                }
                                catch(Exception ex)
                                {
                                    rpe = ResponseParser.ResponseParserEnum.IsFailed;
                                    result = "DO Gagal dibentuk." + ex.Message;
                                    db.Transaction.Rollback();
                                    goto endlogic;
                                }
                                
                                //cn.Open();
                                //SqlCommand cmd = cn.CreateCommand();
                                //cmd.CommandText = "exec SP_DOPharmanet '" + tes + "'";
                                
                                //cmd.BeginExecuteNonQuery();
                                //cmd.ExecuteNonQuery();
                                //cmd.EndExecuteNonQuery();

                                //cn.Dispose();
                                var DO = (from q in db.LG_DOHs
                                          where q.c_po_outlet == tes
                                          select q.c_dono).SingleOrDefault();
                                var cusno = (from q in db.LG_DOHs where q.c_po_outlet == tes
                                                 select q.c_cusno).SingleOrDefault();
                                Commons2.PostDataDODirect(db, DO, true, true);
                            }
                        emailkacab = (from q in db.Temp_emails
                                      where q.c_cusno == cabang && q.group_mail == "kacab"
                                      select q.nv_email_cbg).SingleOrDefault();
                        emailksa = (from q in db.Temp_emails
                                    where q.c_cusno == cabang && q.group_mail == "ksa"
                                    select q.nv_email_cbg).SingleOrDefault();
                        emailksl = (from q in db.Temp_emails
                                    where q.c_cusno == cabang && q.group_mail == "ksl"
                                    select q.nv_email_cbg).SingleOrDefault();
                        cabang = (from q in db.LG_Cusmas
                                  where q.c_cusno == cabang
                                  select q.v_cunam).SingleOrDefault();
                        NoPL = (from q in db.Temp_LG_DOPHs
                                where q.c_po_outlet == PO && q.Status == "20"
                                select q.c_plphar).SingleOrDefault();

                        #region Email ke pharmanet

                        //System.Net.Mail.SmtpClient smtp = null;
                            //StringBuilder sb = new StringBuilder();
                            //try
                            //{
                            //    using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                            //    {
                            //        // send mail containing the file here

                            //        mail.From = new System.Net.Mail.MailAddress("scms.sph@ams.co.id", "Supply Chain Management System");

                            //        mail.Subject = "Laporan Data Pharmanet yang berhasil di proses cabang";

                            //        mail.To.Add("it-support@pharos.co.id");
                            //        mail.To.Add("lianto@pharos.co.id");
                            //        mail.To.Add("haes@pharos.co.id");
                            //        mail.To.Add("betriani_s@pharos.co.id");
                            //        mail.CC.Add("timbul@pharos.co.id");
                            //        mail.CC.Add("jessy@pharos.co.id");
                            //        mail.CC.Add("wahyuni@pharos.co.id");
                            //        mail.CC.Add("agung_imawan@pharos.co.id");
                            //        mail.CC.Add("thiojerry@ams.co.id");
                            //        mail.CC.Add("ida.widyastuti@ams.co.id");
                            //        mail.To.Add("hafizh.ahmad@ams.co.id");
                            //        mail.To.Add("suwandi@ams.co.id");
                            //        mail.To.Add("indra.dwi@ams.co.id");
                            //        mail.To.Add(emailkacab);
                            //        mail.To.Add(emailksa);
                            //        mail.To.Add(emailksl);
                            //        sb.AppendLine("Dear Team Pharmanet,");
                            //        sb.AppendLine("");
                            //        sb.AppendLine("Data PL " + NoPL + " untuk cabang " + cabang + " berhasil di proses dengan detail: ");
                            //        sb.AppendLine("");
                            //        sb.AppendLine(structure.Fields.Keterangan);
                            //        sb.AppendLine("");
                            //        sb.AppendLine("");
                            //        sb.AppendLine("Terima Kasih,");
                            //        sb.AppendLine("AMS - MIS Team");

                            //        mail.Body = sb.ToString();

                            //        smtp = new System.Net.Mail.SmtpClient("10.100.10.9", 25);

                            //        smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                            //        smtp.UseDefaultCredentials = false;
                            //        smtp.Credentials = new System.Net.NetworkCredential("scms.sph@ams.co.id", "scms");

                            //        smtp.Send(mail);
                            //        sb.Length = 0;
                            //    }
                            //    rpe = ResponseParser.ResponseParserEnum.IsSuccess;
                            //    result = "DO berhasil dibentuk. Silakan cek email anda.";
                            //}
                            //catch (Exception ex)
                            //{
                            //    Console.WriteLine(ex.Message);
                            //    Console.WriteLine(ex.StackTrace);
                        //}
                        #endregion
                    }
                    else
                    {
                        result = "Data sudah pernah di proses. Tidak bisa di proses lagi.";
                        rpe = ResponseParser.ResponseParserEnum.IsFailed;
                    }
                }
            }
            catch(Exception Ex)
            {
                result = Ex.Message;
                rpe = ResponseParser.ResponseParserEnum.IsFailed;
            }

            endlogic:
            if (dic != null)
            {
                dic.Clear();
            }

            result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

            return result;
        }

        public string SuratPesanan(ScmsSoaLibrary.Parser.Class.SuratPesananStructure structure)
        {
            return SuratPesanan(structure, null, false);
        }
        #region Old SPM
        //public string SuratPesananManual(ScmsSoaLibrary.Parser.Class.SuratPesananStructure structure)
        //{
        //    return SuratPesananManual(structure, null, false);
        //}

        //public string SuratPesananManual(ScmsSoaLibrary.Parser.Class.SuratPesananStructure structure, ORMDataContext dbContext, bool isRemoveSisa)
        //{
        //    if ((structure == null) || (structure.Fields == null))
        //    {
        //        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
        //    }

        //    string result = null;

        //    bool hasAnyChanges = false;

        //    bool isContexted = false;
        //    ORMDataContext db = null;

        //    if (dbContext == null)
        //    {
        //        db = new ORMDataContext(Functionals.ActiveConnectionString);
        //    }
        //    else
        //    {
        //        isContexted = true;
        //        db = dbContext;
        //    }

        //    LG_SPH sph = null;

        //    ScmsSoaLibrary.Parser.Class.SuratPesananResponse spResp = null;
        //    List<ScmsSoaLibrary.Parser.Class.SuratPesananJSONStructureField> listSPJsonField = null;

        //    ScmsSoaLibrary.Parser.Class.SuratPesananStructureField field = null;
        //    string nipEntry = null,
        //        spID = null,
        //        iteno = null,
        //        tipeItem = null,
        //        noSup = null,
        //        cabangDcore = null;

        //    ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
        //    DateTime date = DateTime.Now,
        //      dateSp = DateTime.MinValue;

        //    decimal spQty = 0,
        //      spSisa = 0,
        //      spAcc = 0,
        //      nPrice = 0;

        //    List<LG_SPD1> listSpd1 = null;
        //    List<LG_SPD2> listSpd2 = null;
        //    List<string> lstSp = new List<string>();

        //    LG_SPD1 spd1 = null;
        //    LG_SPD2 spd2 = null;

        //    int nLoop = 0;

        //    IDictionary<string, string> dic = null;

        //    SPAdminComponent spacDisc = null;

        //    nipEntry = (structure.Fields.Entry ?? string.Empty);

        //    if (string.IsNullOrEmpty(nipEntry))
        //    {
        //        result = "Nip penanggung jawab dibutuhkan.";

        //        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        //        goto endLogic;
        //    }
        //    int totalDetails = 0;

        //    spID = (structure.Fields.SuratPesananID ?? string.Empty);

        //    try
        //    {
        //        if (!isContexted)
        //        {
        //            db.Connection.Open();

        //            db.Transaction = db.Connection.BeginTransaction();
        //        }

        //        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        //        {
        //            #region Add

        //            if (!string.IsNullOrEmpty(spID))
        //            {
        //                result = "Nomor Surat Pesanan harus kosong.";

        //                rpe = ResponseParser.ResponseParserEnum.IsFailed;

        //                if (db.Transaction != null)
        //                {
        //                    db.Transaction.Rollback();
        //                }

        //                goto endLogic;
        //            }
        //            else if (structure.Fields.TanggalSP.Equals(DateTime.MinValue))
        //            {
        //                result = "Format tanggal tidak dapat terbaca.";

        //                rpe = ResponseParser.ResponseParserEnum.IsFailed;

        //                if (db.Transaction != null)
        //                {
        //                    db.Transaction.Rollback();
        //                }

        //                goto endLogic;
        //            }
        //            else if (string.IsNullOrEmpty(structure.Fields.Customer))
        //            {
        //                result = "Nama cabang dibutuhkan.";

        //                rpe = ResponseParser.ResponseParserEnum.IsFailed;

        //                if (db.Transaction != null)
        //                {
        //                    db.Transaction.Rollback();
        //                }

        //                goto endLogic;
        //            }

        //            Constant.TRANSID = (from q in db.LG_SPHs
        //                                where q.d_spdate.Value.Month == date.Date.Month
        //                                select q.c_spno).Max();

        //            var cabang = (from q in db.LG_CusmasCabs
        //                          where q.c_cusno == structure.Fields.Customer
        //                          select q.c_cab_dcore).Take(1).SingleOrDefault();

        //            Constant.Gudang = '1';

        //            spID = Commons.GenerateNumbering<LG_SPH>(db, "SP", '3', "07", date, "c_spno");

        //            Constant.NUMBERID_GUDANG = Constant.NUMBERID_GUDANG.Substring(0, 6) + cabang + Constant.NUMBERID_GUDANG.Substring(9, 5);

        //            sph = new LG_SPH()
        //            {
        //                c_cusno = structure.Fields.Customer,
        //                c_entry = nipEntry,
        //                c_sp = Constant.NUMBERID_GUDANG,
        //                c_spno = spID,
        //                c_type = structure.Fields.TipeSP,
        //                c_update = nipEntry,
        //                d_entry = date,
        //                d_spdate = structure.Fields.TanggalSP,
        //                d_spinsert = date,
        //                d_update = date,
        //                l_cek = structure.Fields.Cek,
        //                l_print = false,
        //                v_ket = structure.Fields.Keterangan,
        //                l_spm = true
        //            };

        //            db.LG_SPHs.InsertOnSubmit(sph);

        //            #region Insert Detail

        //            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
        //            {
        //                listSpd1 = new List<LG_SPD1>();
        //                listSpd2 = new List<LG_SPD2>();

        //                for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
        //                {
        //                    field = structure.Fields.Field[nLoop];

        //                    if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0))
        //                    {
        //                        iteno = (from q in db.FA_MasItms
        //                                 where q.c_iteno == field.Item && q.l_aktif == true && q.c_nosup == "00100"
        //                                 select (q.c_iteno)).Take(1).SingleOrDefault();

        //                        if (!string.IsNullOrEmpty(iteno))
        //                        {
        //                            if (date.DayOfWeek != DayOfWeek.Monday && date.DayOfWeek != DayOfWeek.Tuesday)
        //                            {
        //                                result = "Produk Prinicple Puspa Pharma (" + field.Item + ") hanya bisa diinput Senin dan Selasa. " +
        //                                            "Mohon dikeluarkan dari list pesanan.";

        //                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

        //                                if (db.Transaction != null)
        //                                {
        //                                    db.Transaction.Rollback();
        //                                }

        //                                goto endLogic;
        //                            }
        //                        }
        //                        nPrice = (from q in db.FA_MasItms
        //                                  where q.c_iteno == field.Item && q.l_aktif == true && q.l_hide == false
        //                                  select (q.n_salpri.HasValue ? q.n_salpri.Value : 0)).Take(1).SingleOrDefault();

        //                        listSpd1.Add(new LG_SPD1()
        //                        {
        //                            c_iteno = field.Item,
        //                            c_spno = spID,
        //                            c_type = "01",
        //                            n_qty = field.Quantity,
        //                            n_salpri = nPrice,
        //                            v_ket = field.Keterangan
        //                        });

        //                        #region Discount

        //                        spacDisc = (from q in db.FA_DiscHes
        //                                    join q1 in db.FA_DiscDs on q.c_nodisc equals q1.c_nodisc
        //                                    where q1.c_iteno == field.Item && q1.l_aktif == true
        //                                     && q1.l_status == true && q.c_type == "03"
        //                                    select new SPAdminComponent()
        //                                    {
        //                                        RefID = q.c_nodisc,
        //                                        SignID = q.c_type,
        //                                        Qty = (q1.n_discon.HasValue ? q1.n_discon.Value : 0)
        //                                    }).Take(1).SingleOrDefault();

        //                        if (spacDisc == null)
        //                        {
        //                            listSpd2.Add(new LG_SPD2()
        //                            {
        //                                c_iteno = field.Item,
        //                                c_no = "??????????",
        //                                c_spno = spID,
        //                                c_type = "??",
        //                                n_discoff = 0,
        //                                n_discon = 0
        //                            });
        //                        }
        //                        else
        //                        {
        //                            listSpd2.Add(new LG_SPD2()
        //                            {
        //                                c_iteno = field.Item,
        //                                c_no = spacDisc.RefID,
        //                                c_spno = spID,
        //                                c_type = spacDisc.SignID,
        //                                n_discoff = 0,
        //                                n_discon = spacDisc.Qty
        //                            });
        //                        }

        //                        #endregion
        //                    }

        //                    totalDetails++;
        //                }

        //                if ((listSpd1.Count > 0) && (listSpd2.Count > 0))
        //                {
        //                    db.LG_SPD1s.InsertAllOnSubmit(listSpd1.ToArray());
        //                    listSpd1.Clear();

        //                    db.LG_SPD2s.InsertAllOnSubmit(listSpd2.ToArray());
        //                    listSpd2.Clear();
        //                }
        //            }

        //            #endregion

        //            dic = new Dictionary<string, string>();

        //            if (totalDetails > 0)
        //            {
        //                dic.Add("SP", spID);
        //                dic.Add("Tanggal", date.ToString("yyyyMMdd"));

        //                result = string.Format("Total {0} detail(s)", totalDetails);

        //                hasAnyChanges = true;
        //            }

        //            #endregion
        //        }
        //        else if (structure.Method.Equals("Confirm", StringComparison.OrdinalIgnoreCase))
        //        {
        //            #region Confirm

        //            #region Populate Detail

        //            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
        //            {
        //                listSpd1 = new List<LG_SPD1>();

        //                for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
        //                {
        //                    field = structure.Fields.Field[nLoop];
        //                    lstSp.Add(field.NomorSP);

        //                    if ((field != null) && field.IsModified)
        //                    {
        //                        #region Modify
        //                        sph = (from q in db.LG_SPHs
        //                               where q.c_spno == field.NomorSP
        //                               select q).Take(1).SingleOrDefault();

        //                        if (sph == null)
        //                        {
        //                            result = "Nomor Surat Pesanan " + field.NomorSP + " tidak ditemukan.";

        //                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

        //                            if (db.Transaction != null)
        //                            {
        //                                db.Transaction.Rollback();
        //                            }

        //                            goto endLogic;
        //                        }
        //                        if (sph.l_delete.HasValue && sph.l_delete.Value)
        //                        {
        //                            result = "Surat Pesanan " + field.NomorSP + " sudah terhapus. Mohon diperiksa kembali.";

        //                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

        //                            if (db.Transaction != null)
        //                            {
        //                                db.Transaction.Rollback();
        //                            }

        //                            goto endLogic;
        //                        }

        //                        if (sph != null)
        //                        {
        //                            listSPJsonField = new List<ScmsSoaLibrary.Parser.Class.SuratPesananJSONStructureField>();

        //                            sph.c_type = "03";
        //                            sph.l_cek = true;
        //                            sph.c_spm = structure.Fields.Entry;
        //                            sph.d_spm = date;

        //                            spd1 = (from q in db.LG_SPD1s
        //                                    where q.c_spno == field.NomorSP && q.c_iteno == field.Item
        //                                    select q).Take(1).SingleOrDefault();

        //                            if (spd1 != null)
        //                            {
        //                                spAcc = field.Acceptance;

        //                                spd1.n_acc = spAcc;
        //                                spd1.n_sisa = spAcc;
        //                                spd1.c_type = field.StatusSP;
        //                            }
        //                        }

        //                        #endregion

        //                        hasAnyChanges = true;
        //                    }
        //                }
        //            }

        //            #endregion

        //            #endregion
        //        }
        //        if (!isContexted)
        //        {
        //            if (hasAnyChanges)
        //            {
        //                db.SubmitChanges();

        //                db.Transaction.Commit();
        //                //db.Transaction.Rollback();

        //                var dbSpPo = new ORMDataContext(Functionals.ActiveConnectionString);

        //                try
        //                {
        //                    if (dbSpPo.Transaction == null)
        //                    {
        //                        dbSpPo.Connection.Open();

        //                        dbSpPo.Transaction = dbSpPo.Connection.BeginTransaction();
        //                    }

        //                    if (lstSp.Count > 0)
        //                    {
        //                        var grouped = lstSp.GroupBy(s => s).Select(g => new { Symbol = g.Key, Count = g.Count() });

        //                        foreach (var item in grouped)
        //                        {
        //                            //var symbol = item.Symbol;
        //                            //var count = item.Count;

        //                            sph = (from q in dbSpPo.LG_SPHs
        //                                   where q.c_spno == item.Symbol
        //                                   select q).Take(1).SingleOrDefault();

        //                            if (sph != null)
        //                            {
        //                                listSpd1 = (from q in dbSpPo.LG_SPD1s
        //                                        where q.c_spno == item.Symbol && q.n_sisa > 0
        //                                        select q).ToList();

        //                                cabangDcore = (from q in dbSpPo.LG_Cusmas
        //                                               where q.c_cusno == sph.c_cusno && q.l_rcdcore == true
        //                                               select q.c_cusno).Take(1).SingleOrDefault();

        //                                dateSp = (DateTime)sph.d_spdate;
        //                                spResp = new ScmsSoaLibrary.Parser.Class.SuratPesananResponse()
        //                                {
        //                                    Cabang = sph.c_cusno,
        //                                    ID = sph.c_spno,
        //                                    C_SPNO = sph.c_sp,
        //                                    TanggalSP = dateSp,
        //                                    TanggalSP_Str = dateSp.ToString("yyyy-MM-dd HH:mm:ss.fff")
        //                                };

        //                                if (listSpd1.Count > 0)
        //                                {
        //                                    for (nLoop = 0; nLoop < listSpd1.Count; nLoop++)
        //                                    {
        //                                        tipeItem = (from q in dbSpPo.SCMS_MSITEM_CATs
        //                                                        where q.c_iteno == listSpd1[nLoop].c_iteno
        //                                                       select q.c_type).Take(1).SingleOrDefault();

        //                                        noSup = (from q in dbSpPo.FA_MasItms
        //                                                    where q.c_iteno == listSpd1[nLoop].c_iteno
        //                                                    select q.c_nosup).Take(1).SingleOrDefault();

        //                                        listSPJsonField.Add(new ScmsSoaLibrary.Parser.Class.SuratPesananJSONStructureField()
        //                                        {
        //                                            Acc = (listSpd1[nLoop].n_acc.HasValue ? listSpd1[nLoop].n_acc.Value : 0),
        //                                            C_ITENO = listSpd1[nLoop].c_iteno,
        //                                            Type = "02",
        //                                            C_NOSUP = noSup,
        //                                            Qty = (listSpd1[nLoop].n_qty.HasValue ? listSpd1[nLoop].n_qty.Value : 0),
        //                                            N_QTYSAL = (listSpd1[nLoop].n_sisa.HasValue ? listSpd1[nLoop].n_sisa.Value : 0)
        //                                        });
        //                                    }

        //                                    switch (tipeItem)
        //                                    {
        //                                        case "01":
        //                                            {
        //                                                spResp.TipeSP = "OKT";
        //                                            }
        //                                            break;
        //                                        case "07":
        //                                            {
        //                                                spResp.TipeSP = "PREKURSOR";
        //                                            }
        //                                            break;
        //                                        case "09":
        //                                            {
        //                                                spResp.TipeSP = "OOT";
        //                                            }
        //                                            break;
        //                                        default:
        //                                            {
        //                                                spResp.TipeSP = "REGULER"; 
        //                                            }
        //                                            break;
        //                                    }

        //                                    if (spResp != null && !string.IsNullOrEmpty(cabangDcore))
        //                                    {
        //                                        if ((listSPJsonField != null) && (listSPJsonField.Count > 0))
        //                                        {
        //                                            spResp.Fields = listSPJsonField.ToArray();

        //                                            listSPJsonField.Clear();
        //                                            listSpd1.Clear();

        //                                            PostDataReplySPM(dbSpPo.Connection.ConnectionString, spResp, sph.c_spno);
        //                                        }
        //                                    }
        //                                    //if (Constant.isDcoreError)
        //                                    //{
        //                                    //    dbSpPo.Transaction.Rollback();
        //                                    //    rpe = ResponseParser.ResponseParserEnum.IsFailed;
        //                                    //    Constant.isDcoreError = false;
        //                                    //}
        //                                }
        //                                else
        //                                {
        //                                    sph.l_delete = true;                                            
        //                                }
        //                            }
        //                        }
        //                        dbSpPo.SubmitChanges();

        //                        dbSpPo.Transaction.Commit();
        //                        //db.Transaction.Rollback();

        //                        rpe = ResponseParser.ResponseParserEnum.IsSuccess;
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

        //                    if (dbSpPo.Transaction != null)
        //                    {
        //                        dbSpPo.Transaction.Rollback();
        //                    }
        //                }
        //                rpe = ResponseParser.ResponseParserEnum.IsSuccess;
        //            }
        //            else
        //            {
        //                db.Transaction.Rollback();

        //                rpe = ResponseParser.ResponseParserEnum.IsFailed;
        //            }
        //        }
        //        else
        //        {
        //            if (hasAnyChanges)
        //            {
        //                rpe = ResponseParser.ResponseParserEnum.IsSuccess;
        //            }
        //            else
        //            {
        //                rpe = ResponseParser.ResponseParserEnum.IsFailed;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (db.Transaction != null)
        //        {
        //            db.Transaction.Rollback();
        //        }
        //        rpe = ResponseParser.ResponseParserEnum.IsError;

        //        result = string.Format("ScmsSoaLibrary.Bussiness.Pembelian:SuratPesananManual - {0}", ex.Message);

        //        Logger.WriteLine(result, true);
        //        Logger.WriteLine(ex.StackTrace);
        //    }

        //endLogic:
        //    result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

        //    if (dic != null)
        //    {
        //        dic.Clear();
        //    }

        //    if (!isContexted)
        //    {
        //        db.Dispose();
        //    }

        //    return result;
        //}
        #endregion

        public string SuratPesanan(ScmsSoaLibrary.Parser.Class.SuratPesananStructure structure, ORMDataContext dbContext, bool isRemoveSisa)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null;
            string SPPharID = null;

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

            LG_SPUPDATE SPUpdate = null;
            LG_SPH sph = null;
            LG_Cusma cusmas = null;

            ScmsSoaLibrary.Parser.Class.SuratPesananResponse spResp = null;
            List<ScmsSoaLibrary.Parser.Class.SuratPesananJSONStructureField> listSPJsonField = null;

            ScmsSoaLibrary.Parser.Class.SuratPesananStructureField field = null;
            string nipEntry = null;
            string spID = null;
            //string tmpNumbering = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now,
              dateSp = DateTime.MinValue,
              dateETA = DateTime.Now,
              dateETD = DateTime.Now,
              dateETAbefore = DateTime.Now,
              dateETDbefore = DateTime.Now;

            decimal spQty = 0,
              spSisa = 0,
              spAcc = 0,
              nPrice = 0;

            List<LG_SPD1> listSpd1 = null;
            List<LG_SPD2> listSpd2 = null;
            List<LG_SPD3> listSpd3 = null;

            LG_SPD1 spd1 = null;
            LG_SPD2 spd2 = null;
            LG_SPD3 spd3 = null;

            int nLoop = 0;

            IDictionary<string, string> dic = null;

            SPAdminComponent spacDisc = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }
            int totalDetails = 0;

            spID = (structure.Fields.SuratPesananID ?? string.Empty);

            #region insert
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

                    if (!string.IsNullOrEmpty(spID))
                    {
                        result = "Nomor Surat Pesanan harus kosong.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (string.IsNullOrEmpty(structure.Fields.SPCabang))
                    {
                        result = "Nomor Surat Pesanan Cabang harus terisi.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (structure.Fields.TanggalSP.Equals(DateTime.MinValue))
                    {
                        result = "Format tanggal tidak dapat terbaca.";

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
                    //else if (Commons.IsClosingLogistik(db, date))
                    //{
                    //  result = "Surat pesanan tidak dapat disimpan, karena sudah closing.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}

                    nLoop = (from q in db.LG_SPHs
                             where (q.c_cusno == structure.Fields.Customer)
                              && (q.c_sp == structure.Fields.SPCabang)
                              && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                             select q).Count();
                    if (nLoop > 0)
                    {
                        result = "Nomor surat pesanan telah ada.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    Constant.TRANSID = (from q in db.LG_SPHs
                                        where q.d_spdate.Value.Month == date.Date.Month
                                        select q.c_spno).Max();

                    spID = Commons.GenerateNumbering<LG_SPH>(db, "SP", '3', "07", date, "c_spno");
                    dateETA = Convert.ToDateTime(structure.Fields.D_ETA);
                    dateETD = Convert.ToDateTime(structure.Fields.D_ETD);
                    //Production
                    //spID = string.Concat(spID.Substring(0, 6), "P", spID.Substring(7, 3));

                    //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "SP");

                    sph = new LG_SPH()
                    {
                        c_cusno = structure.Fields.Customer,
                        c_entry = nipEntry,
                        c_sp = structure.Fields.SPCabang,
                        c_spno = spID,
                        c_type = structure.Fields.TipeSP,
                        c_update = nipEntry,
                        d_entry = date,
                        d_spdate = structure.Fields.TanggalSP,
                        d_spinsert = date,
                        d_update = date,
                        l_cek = structure.Fields.Cek,
                        l_print = false,
                        v_ket = structure.Fields.Keterangan,
                        d_etdsp = dateETD,
                        d_etasp = dateETA
                    };

                    //spResp = new ScmsSoaLibrary.Parser.Class.SuratPesananResponse()
                    //{
                    //  Customer = structure.Fields.Customer,
                    //  ID = spID,
                    //  ReferenceID = structure.Fields.SPCabang,
                    //  TanggalSP = date,
                    //  TanggalSP_Str = date.ToString("yyyy-MM-dd HH:mm:ss.fff")
                    //};

                    db.LG_SPHs.InsertOnSubmit(sph);

                    #region Old Coded

                    //db.SubmitChanges();

                    //sph = (from q in db.LG_SPHs
                    //       where q.v_ket == tmpNumbering
                    //       select q).Take(1).SingleOrDefault();

                    //if (sph == null)
                    //{
                    //  result = "Nomor Surat Pesanan tidak dapat di raih.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}
                    //else if (sph.c_spno.Equals("XXXXXXXXXX"))
                    //{
                    //  result = "Trigger Surat Pesanan tidak aktif.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}

                    //sph.v_ket = structure.Fields.Keterangan;

                    //spID = sph.c_spno;

                    #endregion

                    #region Insert Detail

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        listSpd1 = new List<LG_SPD1>();
                        listSpd2 = new List<LG_SPD2>();

                        listSPJsonField = new List<ScmsSoaLibrary.Parser.Class.SuratPesananJSONStructureField>();

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            listSPJsonField.Add(new ScmsSoaLibrary.Parser.Class.SuratPesananJSONStructureField()
                            {
                                Acc = field.Acceptance,
                                C_ITENO = field.Item,
                                Type = "01",
                                Qty = field.Quantity,
                                N_QTYSAL = field.Quantity,
                            });

                            if (field.Acceptance > field.Quantity)
                            {
                                field.Acceptance = field.Quantity;
                            }

                            if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Acceptance > 0))
                            {
                                nPrice = (from q in db.FA_MasItms
                                          where q.c_iteno == field.Item && q.l_aktif == true && q.l_hide == false
                                          select (q.n_salpri.HasValue ? q.n_salpri.Value : 0)).Take(1).SingleOrDefault();

                                listSpd1.Add(new LG_SPD1()
                                {
                                    c_iteno = field.Item,
                                    c_spno = spID,
                                    c_type = "01",
                                    n_acc = field.Acceptance,
                                    n_qty = field.Quantity,
                                    n_salpri = nPrice,
                                    n_sisa = (isRemoveSisa ? 0 : field.Acceptance),
                                    n_spds = 0,
                                    v_ket = field.Keterangan
                                });

                                #region Discount

                                spacDisc = (from q in db.FA_DiscHes
                                            join q1 in db.FA_DiscDs on q.c_nodisc equals q1.c_nodisc
                                            where q1.c_iteno == field.Item && q1.l_aktif == true
                                             && q1.l_status == true && q.c_type == "03"
                                            //&& (q1.d_start >= structure.Fields.TanggalSP) || (q1.d_finish <= structure.Fields.TanggalSP)
                                            select new SPAdminComponent()
                                               {
                                                   RefID = q.c_nodisc,
                                                   SignID = q.c_type,
                                                   Qty = (q1.n_discon.HasValue ? q1.n_discon.Value : 0)
                                               }).Take(1).SingleOrDefault();

                                if (spacDisc == null)
                                {
                                    listSpd2.Add(new LG_SPD2()
                                    {
                                        c_iteno = field.Item,
                                        c_no = "??????????",
                                        c_spno = spID,
                                        c_type = "??",
                                        n_discoff = 0,
                                        n_discon = 0
                                    });
                                }
                                else
                                {
                                    listSpd2.Add(new LG_SPD2()
                                    {
                                        c_iteno = field.Item,
                                        c_no = spacDisc.RefID,
                                        c_spno = spID,
                                        c_type = spacDisc.SignID,
                                        n_discoff = 0,
                                        n_discon = spacDisc.Qty
                                    });
                                }

                                #endregion
                            }

                            totalDetails++;
                        }

                        if ((listSpd1.Count > 0) && (listSpd2.Count > 0))
                        {
                            db.LG_SPD1s.InsertAllOnSubmit(listSpd1.ToArray());
                            listSpd1.Clear();

                            db.LG_SPD2s.InsertAllOnSubmit(listSpd2.ToArray());
                            listSpd2.Clear();
                        }
                    }

                    #endregion

                    dic = new Dictionary<string, string>();

                    if (totalDetails > 0)
                    {
                        dic.Add("SP", spID);
                        dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                        result = string.Format("Total {0} detail(s)", totalDetails);

                        hasAnyChanges = true;
                    }

                    #endregion
                }
                else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    if (string.IsNullOrEmpty(spID))
                    {
                        result = "Nomor Surat Pesanan dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    //else if (Commons.HasOrderOrPacking(db, spID))
                    //{
                    //  result = "Nomor Surat Pesanan tidak dapat di ubah, karena sedang dalam proses.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}

                    sph = (from q in db.LG_SPHs
                           where q.c_spno == spID
                           select q).Take(1).SingleOrDefault();

                    if (sph == null)
                    {
                        result = "Nomor Surat Pesanan tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (sph.l_delete.HasValue && sph.l_delete.Value)
                    {
                        result = "Tidak dapat mengubah Surat Pesanan yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    //else if (Commons.IsClosingLogistik(db, sph.d_spdate))
                    //{
                    //  result = "Surat pesanan tidak dapat diubah, karena sudah closing.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}

                    if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
                    {
                        sph.v_ket = structure.Fields.Keterangan;
                    }

                    dateSp = (sph.d_spinsert.HasValue ? sph.d_spinsert.Value : Functionals.StandardSqlDateTime);
                    dateETDbefore = Convert.ToDateTime(sph.d_etdsp);
                    dateETAbefore = Convert.ToDateTime(sph.d_etasp);

                    var cek = (from q in db.LG_SPD1s where q.c_spno == spID
                                   && q.n_acc != q.n_sisa
                                   select q).AsQueryable();
                    dateETDbefore = Convert.ToDateTime(sph.d_etdsp);
                    cusmas = (from q in db.LG_Cusmas where q.c_cusno == sph.c_cusno
                                        select q).SingleOrDefault();
                    if (structure.Fields.D_ETA != string.Empty)
                    {
                        dateETA = Convert.ToDateTime(structure.Fields.D_ETA);
                    }
                    else
                    {
                        dateETA = Functionals.StandardSqlDateTime;
                    }
                    if (structure.Fields.D_ETD != string.Empty)
                    {
                        dateETD = Convert.ToDateTime(structure.Fields.D_ETD);
                    }
                    else
                    {
                        dateETD = Functionals.StandardSqlDateTime;
                    }
                    if (cek.Count() != 0)
                    {
                        if (sph.d_etdsp != dateETD)
                        {
                            db.Transaction.Rollback();
                            result = "ETA SP tidak bisa di rubah karena sudah dilayani.";
                            rpe = ResponseParser.ResponseParserEnum.IsError;
                            goto endLogic;
                        }
                    }
                    ////sph.d_etasp = dateETA.AddDays(Convert.ToDouble(cusmas.n_days_ekspedisi)); //penambahan ETA SP by Suwandi 09 Nov 2018
                    if (dateETD >= DateTime.Now)
                    {
                        sph.d_etdsp = dateETD; //perubahan dari save ETA menjadi save ETD by Suwandi 21 Nov 2018
                    }
                    else if (dateETD == sph.d_etdsp)
                    {

                    }
                    else
                    {
                        db.Transaction.Rollback();
                        result = "ETA SP tidak bisa di rubah karena lebih kecil dari hari ini.";
                        rpe = ResponseParser.ResponseParserEnum.IsError;
                        goto endLogic;
                    }

                    spResp = new ScmsSoaLibrary.Parser.Class.SuratPesananResponse()
                    {
                        Cabang = sph.c_cusno,
                        ID = spID,
                        C_SPNO = sph.c_sp,
                        TanggalSP = dateSp,
                        TanggalSP_Str = dateSp.ToString("yyyy-MM-dd"),
                        D_ETA = dateETA,
                        D_ETA_str = dateETA.ToString("yyyy-MM-dd"),
                        D_ETD = dateETD.ToString("yyyy-MM-dd")
                    };

                    sph.l_cek = structure.Fields.Cek;
                    sph.c_update = nipEntry;
                    sph.d_update = DateTime.Now;

                    #region Populate Detail

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        listSpd3 = new List<LG_SPD3>();

                        listSPJsonField = new List<ScmsSoaLibrary.Parser.Class.SuratPesananJSONStructureField>();

                        listSpd1 = (from q in db.LG_SPD1s
                                    where q.c_spno == spID
                                    select q).ToList();

                        listSpd2 = (from q in db.LG_SPD2s
                                    where q.c_spno == spID
                                    select q).ToList();

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified) && (field.Quantity > 0))
                            {
                                #region New

                                //listSPJsonField.Add(new ScmsSoaLibrary.Parser.Class.SuratPesananJSONStructureField()
                                //{
                                //  Acc = field.Acceptance,
                                //  Item = field.Item,
                                //  Type = "01",
                                //  Qty = field.Quantity,
                                //  Sisa = field.Quantity,
                                //});

                                if (field.Acceptance > field.Quantity)
                                {
                                    field.Acceptance = field.Quantity;
                                }

                                nPrice = (from q in db.FA_MasItms
                                          where q.c_iteno == field.Item && q.l_aktif == true && q.l_hide == false
                                          select (q.n_salpri.HasValue ? q.n_salpri.Value : 0)).Take(1).SingleOrDefault();

                                spd1 = new LG_SPD1()
                                {
                                    c_iteno = field.Item,
                                    c_spno = spID,
                                    c_type = "01",
                                    n_acc = field.Acceptance,
                                    n_qty = field.Quantity,
                                    n_salpri = nPrice,
                                    n_sisa = field.Acceptance,
                                    v_ket = field.Keterangan
                                };

                                spd3 = new LG_SPD3()
                                {
                                    c_entry = nipEntry,
                                    c_iteno = field.Item,
                                    c_spno = spID,
                                    d_entry = date,
                                    n_acc = field.Acceptance,
                                    n_qty = field.Quantity,
                                    n_salpri = nPrice,
                                    n_sisa = field.Acceptance,
                                    v_ket = field.Keterangan,
                                    v_ket_del = "Append",
                                    v_type = "01",
                                };

                                #region Discount

                                spacDisc = (from q in db.FA_DiscHes
                                            join q1 in db.FA_DiscDs on q.c_nodisc equals q1.c_nodisc
                                            where q1.c_iteno == field.Item && q1.l_aktif == true
                                             && q1.l_status == true && q.c_type == "03"
                                            //&& (q1.d_start >= structure.Fields.TanggalSP) || (q1.d_finish <= structure.Fields.TanggalSP)
                                            select new SPAdminComponent()
                                            {
                                                RefID = q.c_nodisc,
                                                SignID = q.c_type,
                                                Qty = (q1.n_discon.HasValue ? q1.n_discon.Value : 0)
                                            }).Take(1).SingleOrDefault();

                                if (spacDisc == null)
                                {
                                    spd2 = new LG_SPD2()
                                    {
                                        c_iteno = field.Item,
                                        c_no = "??????????",
                                        c_spno = spID,
                                        c_type = "??",
                                        n_discoff = 0,
                                        n_discon = 0
                                    };

                                    #region SPD3

                                    spd3.c_no = "??????????";
                                    spd3.c_type = "??";
                                    spd3.n_discoff = 0;
                                    spd3.n_discon = 0;

                                    #endregion
                                }
                                else
                                {
                                    spd2 = new LG_SPD2()
                                    {
                                        c_iteno = field.Item,
                                        c_no = spacDisc.RefID,
                                        c_spno = spID,
                                        c_type = spacDisc.SignID,
                                        n_discoff = 0,
                                        n_discon = spacDisc.Qty
                                    };

                                    #region SPD3

                                    spd3.c_no = spacDisc.RefID;
                                    spd3.c_type = spacDisc.SignID;
                                    spd3.n_discoff = 0;
                                    spd3.n_discon = spacDisc.Qty;

                                    #endregion
                                }

                                #endregion

                                db.LG_SPD1s.InsertOnSubmit(spd1);
                                db.LG_SPD2s.InsertOnSubmit(spd2);

                                listSpd3.Add(spd3);

                                #endregion
                            }
                            else if ((field != null) && (!field.IsNew) && (!field.IsDelete) && field.IsModified)
                            {
                                #region Modify

                                if ((listSpd1 != null) && (listSpd1.Count > 0))
                                {
                                    //spd1 = (from q in listSpd1
                                    //        where q.c_iteno == field.Item
                                    //        select q).Take(1).SingleOrDefault();

                                    spd1 = listSpd1.Find(delegate(LG_SPD1 spd)
                                    {
                                        return field.Item.Equals((string.IsNullOrEmpty(spd.c_iteno) ? string.Empty : spd.c_iteno.Trim()));
                                    });

                                    if (spd1 != null)
                                    {
                                        spAcc = (spd1.n_acc.HasValue ? spd1.n_acc.Value : 0);

                                        if (spAcc <= 0)
                                        {
                                            continue;
                                        }

                                        spd3 = new LG_SPD3();

                                        spSisa = (spd1.n_sisa.HasValue ? spd1.n_sisa.Value : 0);

                                        spd3.c_entry = nipEntry;
                                        spd3.c_iteno = field.Item;
                                        spd3.c_spno = spID;
                                        spd3.d_entry = date;
                                        spd3.n_acc = spd1.n_acc;
                                        spd3.n_qty = spd1.n_qty;
                                        spd3.n_salpri = spd1.n_salpri;
                                        spd3.n_sisa = spSisa;
                                        spd3.v_ket = spd1.v_ket;
                                        spd3.v_ket_del = field.Keterangan;
                                        spd3.v_type = "02";

                                        //spd2 = (from q in listSpd2
                                        //        where q.c_iteno == field.Item
                                        //        select q).Take(1).SingleOrDefault();

                                        spd2 = listSpd2.Find(delegate(LG_SPD2 spd)
                                        {
                                            return field.Item.Equals((string.IsNullOrEmpty(spd.c_iteno) ? string.Empty : spd.c_iteno.Trim()));
                                        });

                                        if (spd2 != null)
                                        {
                                            spd3.c_no = spd2.c_no;
                                            spd3.c_type = spd2.c_type;
                                            spd3.n_discoff = spd2.n_discoff;
                                            spd3.n_discon = spd2.n_discon;
                                        }

                                        #region Old Coded

                                        //if (field.Acceptance > spd1.n_sisa)
                                        //{
                                        //  spQty = (field.Acceptance - spd1.n_acc);
                                        //  if (spQty < 0)
                                        //  {
                                        //    spQty = 0;
                                        //  }
                                        //}
                                        //else if (field.Acceptance < spd1.n_sisa)
                                        //{
                                        //  spQty = (field.Acceptance - spd1.n_acc);
                                        //  if (spQty > 0)
                                        //  {
                                        //    spQty = 0;
                                        //  }
                                        //}
                                        //else
                                        //{
                                        //  spQty = 0;
                                        //}

                                        #endregion

                                        spQty = (field.Acceptance >= spSisa ? spSisa : field.Acceptance);
                                        if (spAcc == spSisa)
                                        {
                                            spQty = field.Quantity;
                                            spAcc = (field.Acceptance > spAcc ? spAcc : field.Acceptance);
                                        }
                                        else
                                        {
                                            spQty = field.Quantity;
                                            spAcc = (field.Acceptance > spAcc ? spAcc : field.Acceptance);
                                        }

                                        spd1.n_acc = spAcc;
                                        spd1.n_sisa = spQty;
                                        spd1.n_relokasi = 0;
                                        spd1.v_ket = field.KeteranganEditing;

                                        listSpd3.Add(spd3);

                                        listSPJsonField.Add(new ScmsSoaLibrary.Parser.Class.SuratPesananJSONStructureField()
                                        {
                                            Acc = field.Acceptance,
                                            C_ITENO = spd1.c_iteno,
                                            Type = "02",
                                            Qty = (spd1.n_qty.HasValue ? spd1.n_qty.Value : 0),
                                            N_QTYSAL = spd1.n_sisa.Value + spd1.n_spds.Value + (spd1.n_relokasi.HasValue ? spd1.n_relokasi.Value : 0),
                                        });
                                    }
                                }

                                #endregion
                            }
                            else if ((field != null) && (!field.IsNew) && field.IsDelete && (!field.IsModified))
                            {
                                #region Delete

                                if ((listSpd1 != null) && (listSpd1.Count > 0))
                                {
                                    //spd1 = (from q in listSpd1
                                    //        where q.c_iteno == field.Item
                                    //        select q).Take(1).SingleOrDefault();

                                    spd1 = listSpd1.Find(delegate(LG_SPD1 spd)
                                    {
                                        return field.Item.Equals((string.IsNullOrEmpty(spd.c_iteno) ? string.Empty : spd.c_iteno.Trim()));
                                    });

                                    if (spd1 != null)
                                    {
                                        spQty = (spd1.n_acc.HasValue ? spd1.n_acc.Value : 0);
                                        spSisa = (spd1.n_sisa.HasValue ? spd1.n_sisa.Value : 0);

                                        if (spQty == spSisa)
                                        {
                                            listSPJsonField.Add(new ScmsSoaLibrary.Parser.Class.SuratPesananJSONStructureField()
                                            {
                                                Acc = spQty,
                                                C_ITENO = spd1.c_iteno,
                                                Type = "03",
                                                Qty = (spd1.n_qty.HasValue ? spd1.n_qty.Value : 0),
                                                N_QTYSAL = 0,
                                            });

                                            spd3 = new LG_SPD3();

                                            spd3.c_entry = nipEntry;
                                            spd3.c_iteno = field.Item;
                                            spd3.c_spno = spID;
                                            spd3.d_entry = date;
                                            spd3.n_acc = spQty;
                                            spd3.n_qty = spd1.n_qty;
                                            spd3.n_salpri = spd1.n_salpri;
                                            spd3.n_sisa = spSisa;
                                            spd3.v_ket = spd1.v_ket;
                                            spd3.v_ket_del = field.Keterangan;
                                            spd3.v_type = "03";
                                            spd3.c_type = (sph.c_type == null ? string.Empty : sph.c_type);

                                            //spd2 = (from q in listSpd2
                                            //        where q.c_iteno == field.Item
                                            //        select q).Take(1).SingleOrDefault();

                                            spd2 = listSpd2.Find(delegate(LG_SPD2 spd)
                                            {
                                                return field.Item.Equals((string.IsNullOrEmpty(spd.c_iteno) ? string.Empty : spd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase);
                                            });

                                            if (spd2 != null)
                                            {
                                                spd3.c_no = spd2.c_no;
                                                spd3.c_type = spd2.c_type;
                                                spd3.n_discoff = spd2.n_discoff;
                                                spd3.n_discon = spd2.n_discon;

                                                db.LG_SPD2s.DeleteOnSubmit(spd2);
                                            }

                                            listSpd3.Add(spd3);

                                            db.LG_SPD1s.DeleteOnSubmit(spd1);
                                        }
                                    }
                                }

                                #endregion
                            }
                        }

                        if (listSpd3.Count > 0)
                        {
                            db.LG_SPD3s.InsertAllOnSubmit(listSpd3.ToArray());

                            listSpd3.Clear();
                        }

                        if (listSpd1 != null)
                        {
                            listSpd1.Clear();
                        }
                        if (listSpd2 != null)
                        {
                            listSpd2.Clear();
                        }
                    }

                    #endregion

                    hasAnyChanges = true;

                    #endregion
                }
                else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    if (string.IsNullOrEmpty(spID))
                    {
                        result = "Nomor Surat Pesanan dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.HasOrderOrPacking(db, spID))
                    {
                        result = "Nomor Surat Pesanan tidak dapat di hapus, karena sedang dalam proses.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    sph = (from q in db.LG_SPHs
                           where q.c_spno == spID
                           select q).Take(1).SingleOrDefault();

                    if (sph == null)
                    {
                        result = "Nomor Surat Pesanan tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (sph.l_delete.HasValue && sph.l_delete.Value)
                    {
                        result = "Tidak dapat menghapus nomor Surat Pesanan yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, sph.d_entry))
                    {
                        result = "Surat pesanan tidak dapat dihapus, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    dateSp = (sph.d_spinsert.HasValue ? sph.d_spinsert.Value : Functionals.StandardSqlDateTime);

                    spResp = new ScmsSoaLibrary.Parser.Class.SuratPesananResponse()
                    {
                        Cabang = sph.c_cusno,
                        ID = spID,
                        C_SPNO = sph.c_sp,
                        TanggalSP = dateSp,
                        TanggalSP_Str = dateSp.ToString("yyyy-MM-dd HH:mm:ss.fff")
                    };

                    sph.c_update = nipEntry;
                    sph.d_update = DateTime.Now;

                    sph.l_delete = true;
                    sph.v_ket_mark = structure.Fields.Keterangan;

                    listSpd3 = new List<LG_SPD3>();

                    listSpd1 = (from q in db.LG_SPD1s
                                where q.c_spno == spID
                                select q).ToList();

                    listSpd2 = (from q in db.LG_SPD2s
                                where q.c_spno == spID
                                select q).ToList();

                    if ((listSpd1 != null) && (listSpd1.Count > 0))
                    {
                        listSPJsonField = new List<ScmsSoaLibrary.Parser.Class.SuratPesananJSONStructureField>();

                        for (nLoop = 0; nLoop < listSpd1.Count; nLoop++)
                        {
                            spd1 = listSpd1[nLoop];

                            if (spd1 != null && ((spd1.n_qty.HasValue ? spd1.n_qty.Value : 0).Equals(spd1.n_sisa.HasValue ? spd1.n_sisa.Value : 0)))
                            {
                                listSPJsonField.Add(new ScmsSoaLibrary.Parser.Class.SuratPesananJSONStructureField()
                                {
                                    Acc = (spd1.n_acc.HasValue ? spd1.n_acc.Value : 0),
                                    C_ITENO = spd1.c_iteno,
                                    Type = "03",
                                    Qty = (spd1.n_qty.HasValue ? spd1.n_qty.Value : 0),
                                    N_QTYSAL = spd1.n_sisa.Value + spd1.n_spds.Value + (spd1.n_relokasi.HasValue ? spd1.n_relokasi.Value : 0),
                                    //N_QTYSAL = (spd1.n_sisa.HasValue ? spd1.n_sisa.Value : 0),

                                });

                                spd3 = new LG_SPD3()
                                {
                                    c_entry = nipEntry,
                                    c_iteno = spd1.c_iteno,
                                    c_spno = spID,
                                    d_entry = DateTime.Now,
                                    n_acc = spd1.n_acc,
                                    n_qty = spd1.n_qty,
                                    n_salpri = spd1.n_salpri,
                                    n_sisa = spd1.n_sisa,
                                    v_ket = spd1.v_ket,
                                    v_ket_del = structure.Fields.Keterangan,
                                    v_type = "03"
                                };

                                if ((listSpd2 != null) && (listSpd2.Count > 0))
                                {
                                    spd2 = (from q in listSpd2
                                            where q.c_spno == spID && q.c_iteno == spd1.c_iteno
                                            select q).Take(1).SingleOrDefault();

                                    if (spd2 != null)
                                    {
                                        spd3.c_no = spd2.c_no;
                                        spd3.c_type = spd2.c_type;
                                        spd3.n_discoff = spd2.n_discoff;
                                        spd3.n_discon = spd2.n_discon;
                                    }
                                }

                                listSpd3.Add(spd3);
                            }
                            else
                            {
                                result = "Surat pesanan tidak dapat dihapus, karena sudah terpakai.";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }

                                goto endLogic;
                            }
                        }

                        if ((listSpd1 != null) && (listSpd1.Count > 0))
                        {
                            db.LG_SPD1s.DeleteAllOnSubmit(listSpd1.ToArray());
                            listSpd1.Clear();
                        }

                        if ((listSpd2 != null) && (listSpd2.Count > 0))
                        {
                            db.LG_SPD2s.DeleteAllOnSubmit(listSpd2.ToArray());
                            listSpd2.Clear();
                        }
                    }
                    else if ((listSpd2 != null) && (listSpd2.Count > 0))
                    {
                        for (nLoop = 0; nLoop < listSpd2.Count; nLoop++)
                        {
                            spd2 = listSpd2[nLoop];

                            if (spd2 != null)
                            {
                                listSpd3.Add(new LG_SPD3()
                                {
                                    c_no = spd2.c_no,
                                    c_type = spd2.c_type,
                                    n_discoff = spd2.n_discoff,
                                    n_discon = spd2.n_discon
                                });
                            }
                        }

                        if ((listSpd2 != null) && (listSpd2.Count > 0))
                        {
                            db.LG_SPD2s.DeleteAllOnSubmit(listSpd2.ToArray());
                            listSpd2.Clear();
                        }
                    }

                    if (listSpd3.Count > 0)
                    {
                        db.LG_SPD3s.InsertAllOnSubmit(listSpd3.ToArray());
                        listSpd3.Clear();
                    }

                    hasAnyChanges = true;

                    #endregion
                }

                if (!isContexted)
                {
                    if (hasAnyChanges)
                    {
                        if (spResp != null)
                        {
                            //spResp.Fields = 
                            if ((listSPJsonField != null) && (listSPJsonField.Count > 0))
                            {
                                string cek = null;
                                spResp.Fields = listSPJsonField.ToArray();

                                listSPJsonField.Clear();

                                cek = PostDataReplySPM(db.Connection.ConnectionString, spResp, spID);
                                if (!cek.Contains("success"))
                                {
                                    db.Transaction.Rollback();
                                    result = "Data gagal di simpan";
                                    goto endLogic;
                                }
                            }
                            if (spResp.D_ETD != null)
                            {
                                string test = null;

                                test = PostDataReplySPETA(db.Connection.ConnectionString, spResp, spID);
                                if (test.Contains("success"))
                                {
                                    dateETA = Convert.ToDateTime(test.Substring(7, 10));
                                    Constant.isDcoreError = false;
                                    sph.d_etasp = dateETA;
                                    if (dateETD != dateETDbefore)
                                    {
                                        SPUpdate = new LG_SPUPDATE
                                        {
                                            c_entry = structure.Fields.Entry,
                                            c_sp = structure.Fields.SPCabang,
                                            c_spno = structure.Fields.SuratPesananID,
                                            d_before = dateETDbefore,
                                            d_after = dateETD,
                                            d_entry = DateTime.Now
                                        };
                                        db.LG_SPUPDATEs.InsertOnSubmit(SPUpdate);
                                    }

                                }
                                else if (!test.Contains("success"))
                                {
                                    Constant.isDcoreError = true;
                                }

                            }
                        }

                        if (Constant.isDcoreError)
                        {
                            db.Transaction.Rollback();
                            rpe = ResponseParser.ResponseParserEnum.IsFailed;
                            result = "Data gagal di simpan";
                            Constant.isDcoreError = false;
                        }
                        else
                        {
                            db.SubmitChanges();

                            db.Transaction.Commit();
                            //db.Transaction.Rollback();

                            rpe = ResponseParser.ResponseParserEnum.IsSuccess;
                        }
                    }
                    else
                    {
                        db.Transaction.Rollback();

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        result = "Data gagal di simpan";
                    }
                }
                else
                {
                    if (hasAnyChanges)
                    {
                        rpe = ResponseParser.ResponseParserEnum.IsSuccess;
                    }
                    else
                    {
                        rpe = ResponseParser.ResponseParserEnum.IsFailed;
                        result = "Data gagal di simpan";
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

                result = string.Format("ScmsSoaLibrary.Bussiness.Pembelian:SuratPesanan - {0}", ex.Message);

                Logger.WriteLine(result, true);
                Logger.WriteLine(ex.StackTrace);
            }
            #endregion
        endLogic:
            //rpe = ResponseParser.ResponseParserEnum.IsError;
            //result = string.Format("ScmsSoaLibrary.Bussiness.Pembelian:SuratPesanan - {0} test");
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

        public string SuratPesananAdmin(ScmsSoaLibrary.Parser.Class.SuratPesananStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null,
              ret = null;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

            IDictionary<string, string> dic = null;

            Dictionary<string, LG_PLH> dicPLs = null;
            List<LG_PLD1> listPld1 = null;
            List<LG_PLD2> listPld2 = null;

            LG_PLD1 pld1 = null;
            LG_PLD2 pld2 = null;

            LG_ComboH combo = null;
            LG_RND1 rnd1 = null;

            char gdg = char.MinValue;
            char? gudang = null;

            DateTime date = DateTime.Now;

            Penjualan.PLClassComponent spac = null;

            List<Penjualan.PLClassComponent> listSPAC = null;

            Dictionary<string, List<Penjualan.PLClassComponent>> dicItemStock = null;

            ScmsSoaLibrary.Parser.Class.SuratPesananStructureField field = null;

            List<string> lstDODone = null;

            bool hasAnyChanges = false;

            int nLoop = 0,
              nLen = 0,
              nLoopC = 0,
              nLenC = 0,
              totalDetails = 0;

            string suplID = null,
              plID = null,
              custID = null,
              nipEntry = null,
              refID = null,
              spID = null,
              doID = null;

            decimal nQty = 0,
              totalCurrentStock = 0;

            gudang = (from q in db.LG_Cusmas
                      where (q.c_cusno == structure.Fields.Customer)
                      select q.c_gdg).Take(1).SingleOrDefault();

            gdg = (gudang.HasValue ? gudang.Value : char.MinValue);

            if (gdg.Equals(char.MinValue))
            {
                result = "Gudang cabang tidak dapat dibaca.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            ScmsSoaLibrary.Parser.Class.DoPLStructure strucDO = null;

            List<ScmsSoaLibrary.Parser.Class.DoPLStructureField> listDOPLSF = null;

            ScmsSoaLibrary.Parser.Class.DoPLStructureField doplsf = null;

            ScmsSoaLibraryInterface.Components.PostDataParser parser = new ScmsSoaLibraryInterface.Components.PostDataParser();

            ScmsSoaLibraryInterface.Components.PostDataParser.StructureResponse responseResult = default(ScmsSoaLibraryInterface.Components.PostDataParser.StructureResponse);

            Penjualan pnjl = null;

            custID = structure.Fields.Customer;

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                ret = SuratPesanan(structure, db, true);

                responseResult = parser.ResponseParser(ret);

                if (responseResult.IsSet && (responseResult.Response == ScmsSoaLibraryInterface.Components.PostDataParser.ResponseStatus.Success))
                {
                    spID = responseResult.Values.GetValueParser<string, string, string>("SP", string.Empty);

                    if (string.IsNullOrEmpty(spID))
                    {
                        result = "Surat pesanan admin tidak dapat dibuat.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    dicPLs = new Dictionary<string, LG_PLH>(StringComparer.OrdinalIgnoreCase);
                    listPld1 = new List<LG_PLD1>();
                    listPld2 = new List<LG_PLD2>();
                    dicItemStock = new Dictionary<string, List<Penjualan.PLClassComponent>>(StringComparer.OrdinalIgnoreCase);

                    #region Populate Data

                    for (nLoop = 0, nLen = structure.Fields.Field.Length; nLoop < nLen; nLoop++)
                    {
                        field = structure.Fields.Field[nLoop];

                        if (dicItemStock.ContainsKey(field.Item))
                        {
                            listSPAC = dicItemStock[field.Item];
                        }
                        else
                        {
                            listSPAC = (from q in ScmsSoaLibrary.Modules.GlobalQuery.ViewStockLite(db, gdg, field.Item)
                                        join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                        join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                        from qBat in q_2.DefaultIfEmpty()
                                        where (q.n_gsisa != 0)
                                        orderby qBat.d_expired ascending
                                        select new Penjualan.PLClassComponent()
                                        {
                                            RefID = q.c_no,
                                            SignID = (q.c_table == null ? string.Empty : q.c_table.Trim()),
                                            Item = q.c_iteno,
                                            BatchID = (q.c_batch == null ? string.Empty : q.c_batch.Trim()),
                                            Qty = q.n_gsisa,
                                            Supplier = q1.c_nosup,
                                            Box = (q1.n_box.HasValue ? q1.n_box.Value : 0)
                                        }).Distinct().ToList();

                            dicItemStock.Add(field.Item, listSPAC);
                        }

                        if (listSPAC.Count > 0)
                        {
                            totalCurrentStock = listSPAC.Sum(x => x.Qty);
                            if (totalCurrentStock <= 0)
                            {
                                continue;
                            }

                            suplID = listSPAC[nLoop].Supplier;

                            if (string.IsNullOrEmpty(suplID))
                            {
                                continue;
                            }

                            if (!dicPLs.ContainsKey(suplID))
                            {
                                plID = Commons.GenerateNumbering<LG_PLH>(db, "PL", '3', "08", date, "c_plno");

                                dicPLs.Add(suplID, new LG_PLH()
                                {
                                    c_gdg = gdg,
                                    c_plno = plID,
                                    d_pldate = date,
                                    c_cusno = custID,
                                    c_nosup = suplID,
                                    v_ket = "Sys: PL Admin",
                                    c_via = "02",
                                    c_type = "03",
                                    l_confirm = true,
                                    c_entry = nipEntry,
                                    d_entry = date,
                                    c_update = nipEntry,
                                    d_update = date,
                                    l_print = true
                                });
                            }
                            else
                            {
                                plID = dicPLs[suplID].c_plno;
                            }

                            for (nLoopC = 0, nLenC = listSPAC.Count; nLoopC < nLenC; nLoopC++)
                            {
                                spac = listSPAC[nLoopC];
                                nQty = 0;

                                #region Detail Insert

                                if (spac.SignID.Equals("CB", StringComparison.OrdinalIgnoreCase))
                                {
                                    combo = (from q in db.LG_ComboHs
                                             where (q.c_gdg == gdg) && (q.c_combono == spac.RefID)
                                              && (q.c_iteno == field.Item) && (q.c_batch == spac.BatchID)
                                              && (q.n_gsisa > 0)
                                             select q).Take(1).SingleOrDefault();

                                    if (combo != null)
                                    {
                                        nQty = (field.Quantity > spac.Qty ? spac.Qty : field.Quantity);
                                        nQty = (totalCurrentStock > nQty ? nQty : totalCurrentStock);

                                        refID = combo.c_combono;

                                        spac.Qty -= nQty;
                                        combo.n_gsisa -= nQty;
                                    }
                                    else
                                    {
                                        nQty = 0;
                                    }
                                }
                                else if (spac.SignID.Equals("RN", StringComparison.OrdinalIgnoreCase) || spac.SignID.Equals("RR", StringComparison.OrdinalIgnoreCase))
                                {
                                    rnd1 = (from q in db.LG_RND1s
                                            where (q.c_gdg == gdg) && (q.c_rnno == spac.RefID)
                                              && (q.c_iteno == field.Item) && (q.c_batch == spac.BatchID)
                                              && (q.n_gsisa > 0)
                                            select q).Take(1).SingleOrDefault();

                                    if (rnd1 != null)
                                    {
                                        nQty = (field.Quantity > spac.Qty ? spac.Qty : field.Quantity);
                                        nQty = (totalCurrentStock > nQty ? nQty : totalCurrentStock);

                                        refID = rnd1.c_rnno;

                                        spac.Qty -= nQty;
                                        rnd1.n_gsisa -= nQty;
                                    }
                                    else
                                    {
                                        nQty = 0;
                                    }
                                }
                                else
                                {
                                    nQty = 0;
                                }

                                if (nQty > 0)
                                {
                                    #region Populate PLD

                                    pld1 = listPld1.Find(delegate(LG_PLD1 pld)
                                    {
                                        return field.Item.Equals(pld.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                                          spac.BatchID.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
                                          spID.Equals(pld.c_spno, StringComparison.OrdinalIgnoreCase);
                                    });

                                    if (pld1 == null)
                                    {
                                        listPld1.Add(new LG_PLD1()
                                        {
                                            c_plno = plID,
                                            c_iteno = field.Item,
                                            c_batch = spac.BatchID,
                                            c_spno = spID,
                                            c_type = "01",
                                            n_booked = nQty,
                                            n_qty = nQty,
                                            n_sisa = 0
                                        });
                                    }
                                    else
                                    {
                                        pld1.n_booked = pld1.n_qty += nQty;
                                    }

                                    pld2 = listPld2.Find(delegate(LG_PLD2 pld)
                                    {
                                        return field.Item.Equals(pld.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                                          spac.BatchID.Equals(pld.c_batch, StringComparison.OrdinalIgnoreCase) &&
                                          spID.Equals(pld.c_spno, StringComparison.OrdinalIgnoreCase) &&
                                          refID.Equals(pld.c_rnno, StringComparison.OrdinalIgnoreCase);
                                    });

                                    if (pld2 == null)
                                    {
                                        listPld2.Add(new LG_PLD2()
                                        {
                                            c_plno = plID,
                                            c_iteno = field.Item,
                                            c_batch = spac.BatchID,
                                            c_spno = spID,
                                            c_type = "01",
                                            c_rnno = refID,
                                            n_qty = nQty,
                                            n_sisa = 0
                                        });
                                    }
                                    else
                                    {
                                        pld2.n_qty += nQty;
                                    }

                                    field.Quantity -= nQty;

                                    #endregion
                                }
                                if (field.Quantity <= 0)
                                {
                                    break;
                                }

                                #endregion
                            }

                            if (field.Quantity <= 0)
                            {
                                totalDetails++;
                            }

                            listSPAC.Clear();
                        }
                    }

                    #region Cleaning

                    foreach (KeyValuePair<string, List<Penjualan.PLClassComponent>> kvp in dicItemStock)
                    {
                        kvp.Value.Clear();
                    }

                    dicItemStock.Clear();

                    #endregion

                    #endregion

                    if (totalDetails > 0)
                    {
                        foreach (KeyValuePair<string, LG_PLH> kvp in dicPLs)
                        {
                            db.LG_PLHs.InsertOnSubmit(kvp.Value);
                        }

                        db.LG_PLD1s.InsertAllOnSubmit(listPld1.ToArray());

                        db.LG_PLD2s.InsertAllOnSubmit(listPld2.ToArray());

                        #region Buat DO

                        listDOPLSF = new List<ScmsSoaLibrary.Parser.Class.DoPLStructureField>();

                        lstDODone = new List<string>();

                        pnjl = new Penjualan();

                        foreach (KeyValuePair<string, LG_PLH> kvp in dicPLs)
                        {
                            plID = kvp.Value.c_plno;

                            strucDO = new ScmsSoaLibrary.Parser.Class.DoPLStructure()
                            {
                                Method = "Add",
                                Name = Constant.CLASS_NAME_DOPL,
                                Fields = new ScmsSoaLibrary.Parser.Class.DoPLStructureFields()
                                {
                                    ConfirmedSent = false,
                                    Customer = custID,
                                    Entry = nipEntry,
                                    Gudang = gdg.ToString(),
                                    DOid = doID,
                                    TypePackingList = "01",
                                    nopl = plID,
                                    Via = kvp.Value.c_via,
                                    Keterangan = "Sys: DO Admin"
                                }
                            };

                            //strucDO.Method = "Add";
                            //strucDO.Name = Constant.CLASS_NAME_DOPL;
                            //strucDO.Fields = new ScmsSoaLibrary.Parser.Class.DoPLStructureFields();

                            //strucDO.Fields.Customer = custID;
                            //strucDO.Fields.ConfirmedSent = false;
                            //strucDO.Fields.Entry = nipEntry;
                            //strucDO.Fields.Gudang = gdg.ToString();
                            //strucDO.Fields.nopl = plID;
                            //strucDO.Fields.Via = kvp.Value.c_via;
                            //strucDO.Fields.TypePackingList = "01";
                            //strucDO.Fields.Keterangan = "Sys: DO Admin";

                            for (nLoop = 0, nLen = listPld1.Count; nLoop < nLen; nLoop++)
                            {
                                pld1 = listPld1[nLoop];

                                if (pld1.c_plno.Equals(plID, StringComparison.OrdinalIgnoreCase))
                                {
                                    doplsf = listDOPLSF.Find(delegate(ScmsSoaLibrary.Parser.Class.DoPLStructureField sf)
                                    {
                                        return pld1.c_iteno.Equals(sf.Item, StringComparison.OrdinalIgnoreCase);
                                    });

                                    if (doplsf == null)
                                    {
                                        doplsf = new ScmsSoaLibrary.Parser.Class.DoPLStructureField()
                                        {
                                            IsNew = true,
                                            Item = pld1.c_iteno,
                                            Quantity = pld1.n_qty.Value
                                        };

                                        listDOPLSF.Add(doplsf);
                                    }
                                    else
                                    {
                                        doplsf.Quantity += pld1.n_qty.Value;
                                    }
                                }
                            }

                            strucDO.Fields.Field = listDOPLSF.ToArray();

                            ret = pnjl.DOPackingList(strucDO, db);

                            responseResult = parser.ResponseParser(ret);

                            if (responseResult.IsSet && (responseResult.Response == ScmsSoaLibraryInterface.Components.PostDataParser.ResponseStatus.Success))
                            {
                                doID = responseResult.Values.GetValueParser<string, string, string>("DO", string.Empty);

                                if (!string.IsNullOrEmpty(doID))
                                {
                                    lstDODone.Add(doID);
                                }
                            }

                            //lstDODone.Clear();

                            listDOPLSF.Clear();
                            listPld1.Clear();
                            listPld2.Clear();
                        }

                        #endregion
                    }

                    hasAnyChanges = true;

                    dicPLs.Clear();
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

                result = string.Format("ScmsSoaLibrary.Bussiness.Pembelian:SuratPesananAdmin - {0}", ex.Message);

                Logger.WriteLine(result, true);
                Logger.WriteLine(ex.StackTrace);

                lstDODone.Clear();
            }

            if ((lstDODone != null) && (lstDODone.Count > 0))
            {
                for (nLoop = 0, nLen = lstDODone.Count; nLoop < nLen; nLoop++)
                {
                    doID = lstDODone[nLoop];

                    if (!string.IsNullOrEmpty(doID))
                    {
                        strucDO = new ScmsSoaLibrary.Parser.Class.DoPLStructure()
                        {
                            Method = "ConfirmSent",
                            Name = Constant.CLASS_NAME_DOPL,
                            Fields = new ScmsSoaLibrary.Parser.Class.DoPLStructureFields()
                            {
                                ConfirmedSent = true,
                                Customer = custID,
                                Entry = nipEntry,
                                Gudang = gdg.ToString(),
                                DOid = doID,
                                TypePackingList = "01",
                                Keterangan = "Sys: DO Admin"
                            }
                        };

                        ret = pnjl.DOPackingList(strucDO);

                        //responseResult = parser.ResponseParser(ret);

                        //if (responseResult.IsSet && (responseResult.Response == ScmsSoaLibraryInterface.Components.PostDataParser.ResponseStatus.Success))
                        //{

                        //}
                    }
                }
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

        public string OrderRequest(ScmsSoaLibrary.Parser.Class.OrderRequestStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false;

            string result = null;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            LG_ORH orh = null;

            ScmsSoaLibrary.Parser.Class.OrderRequestStructureField field = null;
            string nipEntry = null;
            string orID = null;
            //string tmpNumbering = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now;

            //decimal? spQty = 0,
            //  nPrice = 0;
            //decimal spQtyReloc = 0,
            //  rnQty = 0,
            //  spAlloc = 0;

            ORHelper orhlp = null;

            List<LG_ORD1> listOrd1 = null;
            List<LG_ORD2> listOrd2 = null;
            //List<LG_ORD3> listOrd3 = null;
            List<LG_ORD4> listOrd4 = null;
            List<LG_ORD5> listOrd5 = null;

            LG_ORD1 ord1 = null;
            LG_ORD2 ord2 = null;
            //LG_ORD3 ord3 = null;
            //LG_ORD4 ord4 = null;
            //LG_ORD5 ord5 = null;

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

            orID = (structure.Fields.OrderRequestID ?? string.Empty);

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
                {
                    #region Add

                    if (!string.IsNullOrEmpty(orID))
                    {
                        result = "Nomor Order Request harus kosong.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (string.IsNullOrEmpty(structure.Fields.Suplier))
                    {
                        result = "Nama suplier dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, date))
                    {
                        result = "Order tidak dapat disimpan, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    orID = Commons.GenerateNumbering<LG_ORH>(db, "OR", '3', "01", date, "c_orno");

                    //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "OR");

                    orh = new LG_ORH()
                    {
                        c_gdg = '1',
                        c_nosup = structure.Fields.Suplier,
                        d_ordate = date,
                        c_orno = orID,
                        c_entry = nipEntry,
                        c_type = structure.Fields.TipeOR,
                        c_update = nipEntry,
                        d_entry = date,
                        d_update = date,
                        v_ket = structure.Fields.Keterangan
                    };

                    db.LG_ORHs.InsertOnSubmit(orh);

                    #region Old Coded

                    //db.SubmitChanges();

                    //orh = (from q in db.LG_ORHs
                    //       where q.v_ket == tmpNumbering
                    //       select q).Take(1).SingleOrDefault();

                    //if (orh == null)
                    //{
                    //  result = "Nomor Order Request tidak dapat di raih.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}
                    //else if (orh.c_orno.Equals("XXXXXXXXXX"))
                    //{
                    //  result = "Trigger Order Request tidak aktif.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}

                    //orh.v_ket = structure.Fields.Keterangan;

                    //orID = orh.c_orno;

                    #endregion

                    if ((!string.IsNullOrEmpty(orh.c_type)) && orh.c_type.Equals("05", StringComparison.OrdinalIgnoreCase))
                    {
                        #region Insert Detail (Manual)

                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            listOrd1 = new List<LG_ORD1>();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                field = structure.Fields.Field[nLoop];

                                if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0) && (field.QuantityOrder > 0))
                                {
                                    orhlp = (from q in db.FA_MasItms
                                             join q1 in db.LG_DatSups on q.c_nosup equals q1.c_nosup
                                             join q2 in db.FA_Divpris on q.c_iteno equals q2.c_iteno
                                             join q3 in db.FA_MsDivPris on q2.c_kddivpri equals q3.c_kddivpri into q_3
                                             from qMDP in q_3.DefaultIfEmpty()
                                             where q.c_iteno == field.Item
                                             select new ORHelper()
                                             {
                                                 c_iteno = q.c_iteno,
                                                 v_itnam = q.v_itnam,
                                                 n_pminord = (q.n_pminord.HasValue ? q.n_pminord.Value : 0),
                                                 n_qminord = (q.n_qminord.HasValue ? q.n_qminord.Value : 0),
                                                 c_type = q.c_type,
                                                 c_via = q.c_via,
                                                 n_beli = (q.n_beli.HasValue ? q.n_beli.Value : 0),
                                                 n_box = (q.n_box.HasValue ? q.n_box.Value : 0),
                                                 n_salpri = (q.n_salpri.HasValue ? q.n_salpri.Value : 0),
                                                 n_index = (q1.n_index.HasValue ? q1.n_index.Value : 0),
                                                 c_nosup = q1.c_nosup,
                                                 c_kddivpri = q2.c_kddivpri,
                                                 v_nmdivpri = qMDP.v_nmdivpri
                                             }).Take(1).SingleOrDefault();

                                    if (orhlp != null)
                                    {
                                        listOrd1.Add(new LG_ORD1()
                                        {
                                            c_gdg = orh.c_gdg,
                                            c_iteno = field.Item,
                                            c_kddivpri = orhlp.c_kddivpri,
                                            c_orno = orID,
                                            c_type = orhlp.c_type,
                                            c_via = orhlp.c_via,
                                            n_avgsls = 0,
                                            n_avgslsdivpri = 0,
                                            n_beli = orhlp.n_beli,
                                            n_bo = 0,
                                            n_bonus = 0,
                                            n_box = orhlp.n_box,
                                            n_deviasi = 0,
                                            n_ideal = 0,
                                            n_idxnp = 0,
                                            n_idxp = 0,
                                            n_index = orhlp.n_index,
                                            n_order = 0,
                                            n_pareto = 0,
                                            n_pminord = 0,
                                            n_qminord = 0,
                                            n_qty = field.Quantity,
                                            n_salpri = orhlp.n_salpri,
                                            n_sisa = field.QuantityOrder,
                                            n_sit = 0,
                                            n_soh = 0,
                                            n_spacc = field.Quantity,
                                            n_variabel = 0
                                        });
                                    }
                                }

                                totalDetails++;
                            }

                            if (listOrd1.Count > 0)
                            {
                                db.LG_ORD1s.InsertAllOnSubmit(listOrd1.ToArray());
                            }

                            listOrd1.Clear();
                        }

                        #endregion
                    }

                    dic = new Dictionary<string, string>();

                    if (totalDetails > 0)
                    {
                        dic.Add("OR", orID);
                        dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                        result = string.Format("Total {0} detail(s)", totalDetails);

                        hasAnyChanges = true;
                    }

                    #endregion
                }
                else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    if (string.IsNullOrEmpty(orID))
                    {
                        result = "Nomor Order Request dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    orh = (from q in db.LG_ORHs
                           where q.c_orno == orID
                           select q).Take(1).SingleOrDefault();

                    if (orh == null)
                    {
                        result = "Nomor Order Request tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (orh.l_delete.HasValue && orh.l_delete.Value)
                    {
                        result = "Tidak dapat mengubah Order Request yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.HasPO(db, orID))
                    {
                        result = "Tidak dapat mengubah Order Request yang sudah terkirim ke principal.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    //else if (Commons.IsClosingLogistik(db, orh.d_ordate))
                    //{
                    //  result = "Order request tidak dapat diubah, karena sudah closing.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}

                    if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
                    {
                        orh.v_ket = structure.Fields.Keterangan;
                    }

                    orh.c_update = nipEntry;
                    orh.d_update = DateTime.Now;

                    #region Populate Detail

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0) && (!string.IsNullOrEmpty(orh.c_type)))
                    {
                        listOrd1 = new List<LG_ORD1>();
                        listOrd5 = new List<LG_ORD5>();

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified) && (field.Quantity > 0) && (field.QuantityOrder > 0))
                            {
                                #region New

                                orhlp = (from q in db.FA_MasItms
                                         join q1 in db.LG_DatSups on q.c_nosup equals q1.c_nosup
                                         join q2 in db.FA_Divpris on q.c_iteno equals q2.c_iteno
                                         join q3 in db.FA_MsDivPris on q2.c_kddivpri equals q3.c_kddivpri into q_3
                                         from qMDP in q_3.DefaultIfEmpty()
                                         where q.c_iteno == field.Item
                                         select new ORHelper()
                                         {
                                             c_iteno = q.c_iteno,
                                             v_itnam = q.v_itnam,
                                             n_pminord = (q.n_pminord.HasValue ? q.n_pminord.Value : 0),
                                             n_qminord = (q.n_qminord.HasValue ? q.n_qminord.Value : 0),
                                             c_type = q.c_type,
                                             c_via = q.c_via,
                                             n_beli = (q.n_beli.HasValue ? q.n_beli.Value : 0),
                                             n_box = (q.n_box.HasValue ? q.n_box.Value : 0),
                                             n_salpri = (q.n_salpri.HasValue ? q.n_salpri.Value : 0),
                                             n_index = (q1.n_index.HasValue ? q1.n_index.Value : 0),
                                             c_nosup = q1.c_nosup,
                                             c_kddivpri = q2.c_kddivpri,
                                             v_nmdivpri = qMDP.v_nmdivpri
                                         }).Take(1).SingleOrDefault();

                                if (orhlp != null)
                                {
                                    listOrd1.Add(new LG_ORD1()
                                    {
                                        c_gdg = orh.c_gdg,
                                        c_iteno = field.Item,
                                        c_kddivpri = orhlp.c_kddivpri,
                                        c_orno = orID,
                                        c_type = orhlp.c_type,
                                        c_via = orhlp.c_via,
                                        n_avgsls = 0,
                                        n_avgslsdivpri = 0,
                                        n_beli = orhlp.n_beli,
                                        n_bo = 0,
                                        n_bonus = 0,
                                        n_box = orhlp.n_box,
                                        n_deviasi = 0,
                                        n_ideal = 0,
                                        n_idxnp = 0,
                                        n_idxp = 0,
                                        n_index = orhlp.n_index,
                                        n_order = 0,
                                        n_pareto = 0,
                                        n_pminord = 0,
                                        n_qminord = 0,
                                        n_qty = field.QuantityOrder,
                                        n_salpri = orhlp.n_salpri,
                                        n_sisa = field.Quantity,
                                        n_sit = 0,
                                        n_soh = 0,
                                        n_spacc = 0,
                                        n_variabel = 0
                                    });

                                    listOrd5.Add(new LG_ORD5()
                                    {
                                        c_entry = nipEntry,
                                        c_gdg = orh.c_gdg,
                                        c_iteno = field.Item,
                                        c_no = null,
                                        c_orno = orID,
                                        d_entry = date,
                                        n_qty = field.Quantity,
                                        n_sisa = field.QuantityOrder,
                                        v_type = "01"
                                    });
                                }

                                totalDetails++;

                                #endregion
                            }
                            else if ((field != null) && (!field.IsNew) && field.IsDelete && (!field.IsModified))
                            {
                                #region Delete

                                ord1 = (from q in db.LG_ORD1s
                                        where q.c_orno == orID && q.c_iteno == field.Item
                                        select q).Take(1).SingleOrDefault();

                                if (ord1 != null)
                                {
                                    switch (orh.c_type)
                                    {
                                        #region Khusus & Otomatis

                                        case "01":
                                        case "02":
                                        case "03":
                                        case "04":
                                            {
                                                listOrd2 = (from q in db.LG_ORD2s
                                                            where q.c_orno == orID && q.c_iteno == field.Item
                                                            select q).ToList();
                                                if (listOrd2.Count > 0)
                                                {
                                                    for (nLoopC = 0; nLoopC < listOrd2.Count; nLoopC++)
                                                    {
                                                        ord2 = listOrd2[nLoopC];
                                                        if (ord2 != null)
                                                        {
                                                            listOrd5.Add(new LG_ORD5()
                                                            {
                                                                c_entry = nipEntry,
                                                                c_gdg = ord1.c_gdg,
                                                                c_iteno = field.Item,
                                                                c_no = ord2.c_spno,
                                                                c_orno = orID,
                                                                d_entry = date,
                                                                n_qty = ord2.n_sisa,
                                                                n_sisa = ord2.n_sisa,
                                                                v_ket_del = field.Keterangan,
                                                                v_type = "03"
                                                            });
                                                        }
                                                        else
                                                        {
                                                            listOrd5.Add(new LG_ORD5()
                                                            {
                                                                c_entry = nipEntry,
                                                                c_gdg = ord1.c_gdg,
                                                                c_iteno = field.Item,
                                                                c_no = null,
                                                                c_orno = orID,
                                                                d_entry = date,
                                                                n_qty = ord1.n_qty,
                                                                n_sisa = ord1.n_sisa,
                                                                v_ket_del = field.Keterangan,
                                                                v_type = "03"
                                                            });
                                                        }
                                                    }
                                                    db.LG_ORD2s.DeleteAllOnSubmit(listOrd2.ToArray());

                                                    listOrd2.Clear();
                                                }
                                                else
                                                {
                                                    listOrd5.Add(new LG_ORD5()
                                                    {
                                                        c_entry = nipEntry,
                                                        c_gdg = ord1.c_gdg,
                                                        c_iteno = field.Item,
                                                        c_no = null,
                                                        c_orno = orID,
                                                        d_entry = date,
                                                        n_qty = ord1.n_qty,
                                                        n_sisa = ord1.n_sisa,
                                                        v_ket_del = field.Keterangan,
                                                        v_type = "03"
                                                    });
                                                }
                                            }
                                            break;

                                        #endregion

                                        #region Manual

                                        case "05":
                                            {
                                                listOrd5.Add(new LG_ORD5()
                                                {
                                                    c_entry = nipEntry,
                                                    c_gdg = ord1.c_gdg,
                                                    c_iteno = field.Item,
                                                    c_no = null,
                                                    c_orno = orID,
                                                    d_entry = date,
                                                    n_qty = ord1.n_qty,
                                                    n_sisa = ord1.n_sisa,
                                                    v_ket_del = field.Keterangan,
                                                    v_type = "03"
                                                });
                                            }
                                            break;

                                        #endregion
                                    }

                                    db.LG_ORD1s.DeleteOnSubmit(ord1);
                                }

                                #endregion
                            }
                        }

                        if (listOrd1.Count > 0)
                        {
                            db.LG_ORD1s.InsertAllOnSubmit(listOrd1.ToArray());
                        }

                        if (listOrd5.Count > 0)
                        {
                            db.LG_ORD5s.InsertAllOnSubmit(listOrd5.ToArray());
                        }

                        listOrd1.Clear();
                        listOrd5.Clear();

                    }

                    #endregion

                    hasAnyChanges = true;

                    #endregion
                }
                else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    if (string.IsNullOrEmpty(orID))
                    {
                        result = "Nomor Order Request dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    orh = (from q in db.LG_ORHs
                           where q.c_orno == orID
                           select q).Take(1).SingleOrDefault();

                    if (orh == null)
                    {
                        result = "Nomor Order Request tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (orh.l_delete.HasValue && orh.l_delete.Value)
                    {
                        result = "Tidak dapat mengubah Order Request yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.HasPO(db, orID))
                    {
                        result = "Tidak dapat menghapus Order Request yang sudah terkirim ke principal.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, orh.d_ordate))
                    {
                        result = "Order request tidak dapat dihapus, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    orh.c_update = nipEntry;
                    orh.d_update = DateTime.Now;

                    orh.l_delete = true;
                    orh.v_ket_mark = structure.Fields.Keterangan;

                    #region Deleting

                    listOrd1 = (from q in db.LG_ORD1s
                                where q.c_orno == orID
                                select q).ToList();

                    if (listOrd1 != null)
                    {
                        listOrd5 = new List<LG_ORD5>();

                        for (nLoop = 0; nLoop < listOrd1.Count; nLoop++)
                        {
                            ord1 = listOrd1[nLoop];

                            if (ord1 != null)
                            {
                                switch (orh.c_type)
                                {
                                    #region Khusus & Otomatis

                                    case "01":
                                    case "02":
                                    case "03":
                                    case "04":
                                        {
                                            listOrd2 = (from q in db.LG_ORD2s
                                                        where q.c_orno == orID && q.c_iteno == ord1.c_iteno
                                                        select q).ToList();

                                            if ((listOrd2 != null) && (listOrd2.Count > 0))
                                            {
                                                for (nLoopC = 0; nLoopC < listOrd2.Count; nLoopC++)
                                                {
                                                    ord2 = listOrd2[nLoopC];
                                                    if (ord2 != null)
                                                    {
                                                        listOrd5.Add(new LG_ORD5()
                                                        {
                                                            c_entry = nipEntry,
                                                            c_gdg = ord1.c_gdg,
                                                            c_iteno = ord1.c_iteno,
                                                            c_no = ord2.c_spno,
                                                            c_orno = orID,
                                                            d_entry = date,
                                                            n_qty = ord2.n_sisa,
                                                            n_sisa = ord2.n_sisa,
                                                            v_ket_del = structure.Fields.Keterangan,
                                                            v_type = "03"
                                                        });
                                                    }
                                                    else
                                                    {
                                                        listOrd5.Add(new LG_ORD5()
                                                        {
                                                            c_entry = nipEntry,
                                                            c_gdg = ord1.c_gdg,
                                                            c_iteno = ord1.c_iteno,
                                                            c_no = null,
                                                            c_orno = orID,
                                                            d_entry = date,
                                                            n_qty = ord1.n_qty,
                                                            n_sisa = ord1.n_sisa,
                                                            v_ket_del = structure.Fields.Keterangan,
                                                            v_type = "03"
                                                        });
                                                    }
                                                }
                                                db.LG_ORD2s.DeleteAllOnSubmit(listOrd2.ToArray());

                                                listOrd2.Clear();
                                            }
                                            else
                                            {
                                                listOrd5.Add(new LG_ORD5()
                                                {
                                                    c_entry = nipEntry,
                                                    c_gdg = ord1.c_gdg,
                                                    c_iteno = ord1.c_iteno,
                                                    c_no = null,
                                                    c_orno = orID,
                                                    d_entry = date,
                                                    n_qty = ord1.n_qty,
                                                    n_sisa = ord1.n_sisa,
                                                    v_ket_del = structure.Fields.Keterangan,
                                                    v_type = "03"
                                                });
                                            }
                                        }
                                        break;

                                    #endregion

                                    #region Manual

                                    case "05":
                                        {
                                            listOrd5.Add(new LG_ORD5()
                                            {
                                                c_entry = nipEntry,
                                                c_gdg = ord1.c_gdg,
                                                c_iteno = ord1.c_iteno,
                                                c_no = null,
                                                c_orno = orID,
                                                d_entry = date,
                                                n_qty = ord1.n_qty,
                                                n_sisa = ord1.n_sisa,
                                                v_ket_del = structure.Fields.Keterangan,
                                                v_type = "03"
                                            });
                                        }
                                        break;

                                    #endregion
                                }
                            }
                        }

                        listOrd4 = (from q in db.LG_ORD4s
                                    where q.c_orno == orID && q.c_iteno == ord1.c_iteno
                                    select q).ToList();

                        if (listOrd4.Count > 0)
                        {
                            db.LG_ORD4s.DeleteAllOnSubmit(listOrd4.ToArray());
                            listOrd4.Clear();
                        }

                        if (listOrd1.Count > 0)
                        {
                            db.LG_ORD1s.DeleteAllOnSubmit(listOrd1.ToArray());
                            listOrd1.Clear();
                        }

                        if (listOrd5.Count > 0)
                        {
                            db.LG_ORD5s.InsertAllOnSubmit(listOrd5.ToArray());
                            listOrd5.Clear();
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
                rpe = ResponseParser.ResponseParserEnum.IsError;

                result = string.Format("ScmsSoaLibrary.Bussiness.Pembelian:OrderRequest - {0}", ex.Message);

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

        public string OrderRequestProcess(ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false;

            string result = null;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            LG_ORH orh = null;

            ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructureField field = null;
            ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructureField field1 = null;
            string nipEntry = null;
            string orID = null;
            string tmpNumbering = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now;

            ScmsSoaLibrary.Parser.Class.SuratPesananStructure sps = null;

            decimal? spQty = 0;
            decimal spQtyReloc = 0,
              spDiv = 0,
              spAlloc = 0;

            List<LG_ORD1> listOrd1 = null;
            List<LG_ORD2> listOrd2 = null;
            List<LG_ORD2> listOrd2Adj = null;
            //List<LG_ORD3> listOrd3 = null;
            List<LG_ORD4> listOrd4 = null;
            //List<LG_ORD5> listOrd5 = null;

            LG_ORD1 ord1 = null;
            //LG_ORD2 ord2 = null;
            //LG_ORD3 ord3 = null;
            //LG_ORD4 ord4 = null;
            //LG_ORD5 ord5 = null;

            List<string> listItems = null;

            List<ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructureField> listFields = new List<ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructureField>();
            List<ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructureField> listItmFltr = null;
            List<ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructureField> listItmSiT = null;

            List<ScmsSoaLibrary.Parser.Class.SuratPesananStructureField> listAdjSp = new List<ScmsSoaLibrary.Parser.Class.SuratPesananStructureField>();

            int nLoop = 0,
              nLoopC = 0;

            bool isKhusus = false;

            IDictionary<string, string> dic = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }
            int totalDetails = 0;

            orID = (structure.Fields.OrderRequestID ?? string.Empty);

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
                {
                    #region Add

                    if (!string.IsNullOrEmpty(orID))
                    {
                        result = "Nomor Order Request harus kosong.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (string.IsNullOrEmpty(structure.Fields.Suplier))
                    {
                        result = "Nama suplier dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, date))
                    {
                        result = "Order request tidak dapat disimpan, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    isKhusus = (structure.Fields.TipeOR.Equals("02", StringComparison.OrdinalIgnoreCase) ? true : false);

                    orID = Commons.GenerateNumbering<LG_ORH>(db, "OR", '3', "01", date, "c_orno");

                    //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "ORP");

                    orh = new LG_ORH()
                    {
                        c_gdg = '1',
                        c_nosup = structure.Fields.Suplier,
                        d_ordate = date,
                        c_orno = orID,
                        c_entry = nipEntry,
                        c_type = structure.Fields.TipeOR,
                        c_update = nipEntry,
                        d_entry = date,
                        d_update = date,
                        v_ket = structure.Fields.Keterangan
                    };

                    db.LG_ORHs.InsertOnSubmit(orh);

                    #region Old Coded

                    //db.SubmitChanges();

                    //orh = (from q in db.LG_ORHs
                    //       where q.v_ket == tmpNumbering
                    //       select q).Take(1).SingleOrDefault();

                    //if (orh == null)
                    //{
                    //  result = "Nomor Order Request tidak dapat di raih.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}

                    //if (orh.c_orno.Equals("XXXXXXXXXX"))
                    //{
                    //  result = "Trigger Order Request tidak aktif.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}

                    //orh.v_ket = structure.Fields.Keterangan;

                    //orID = orh.c_orno;

                    #endregion

                    sps = new ScmsSoaLibrary.Parser.Class.SuratPesananStructure()
                    {
                        Method = "Add",
                        Fields = new ScmsSoaLibrary.Parser.Class.SuratPesananStructureFields()
                        {
                            Cek = false,
                            Customer = structure.Fields.Cabang,
                            Entry = nipEntry,
                            Keterangan = "SP Adjust",
                            SPCabang = "Auto Adj",
                            TanggalSP = date,
                            TipeSP = "04"
                        },
                        Name = "SuratPesanan"
                    };

                    #region Insert Detail (Manual)

                    listOrd1 = new List<LG_ORD1>();
                    listOrd2 = new List<LG_ORD2>();
                    listOrd2Adj = new List<LG_ORD2>();
                    listOrd4 = new List<LG_ORD4>();

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        listFields.AddRange(structure.Fields.Field);

                        #region Verify Fields

                        var listFieldsGrp = (from q in listFields
                                             where //q.NomorID != null &&
                                                 //q.SpAcc > 0 && 
                                              !q.Manual
                                             group q by new
                                             {
                                                 q.Item,
                                                 q.SpAcc,
                                                 q.AvgSales,
                                                 q.AvgSalesDivPri,
                                                 q.BackOrder,
                                                 q.Bonus,
                                                 q.Box,
                                                 q.Deviasi,
                                                 q.DivisiPrincipal,
                                                 q.HNA,
                                                 q.Ideal,
                                                 q.Idxnp,
                                                 q.Idxp,
                                                 q.Index,
                                                 q.MOQ,
                                                 q.MOP,
                                                 q.SiT,
                                                 q.SoH,
                                                 q.TypeItem,
                                                 q.Variable,
                                                 q.Pareto,
                                                 q.Via,
                                                 q.Order
                                             } into g
                                             select new ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructureField()
                                             {
                                                 Item = g.Key.Item,
                                                 SpAcc = g.Key.SpAcc,
                                                 AvgSales = g.Key.AvgSales,
                                                 AvgSalesDivPri = g.Key.AvgSalesDivPri,
                                                 BackOrder = g.Key.BackOrder,
                                                 Bonus = g.Key.Bonus,
                                                 Box = g.Key.Box,
                                                 Deviasi = g.Key.Deviasi,
                                                 DivisiPrincipal = g.Key.DivisiPrincipal,
                                                 HNA = g.Key.HNA,
                                                 Ideal = g.Key.Ideal,
                                                 Idxnp = g.Key.Idxnp,
                                                 Idxp = g.Key.Idxp,
                                                 Index = g.Key.Index,
                                                 MOQ = g.Key.MOQ,
                                                 MOP = g.Key.MOP,
                                                 SiT = g.Key.SiT,
                                                 SoH = g.Key.SoH,
                                                 TypeItem = g.Key.TypeItem,
                                                 Variable = g.Key.Variable,
                                                 Pareto = g.Key.Pareto,
                                                 Via = g.Key.Via,
                                                 Order = g.Key.Order
                                             }).ToList();

                        var listFieldsGrpManual = (from q in listFields
                                                   where //q.NomorID != null &&
                                                       //q.SpAcc > 0 && 
                                                    q.Manual
                                                   group q by new
                                                   {
                                                       q.Item,
                                                   } into g
                                                   select g.Key).ToList();

                        if (listFieldsGrpManual.Count > 0)
                        {
                            List<string> listManual = (from q in listFieldsGrpManual
                                                       join q1 in listFieldsGrp on q.Item equals q1.Item into q_1
                                                       from qLG in q_1.DefaultIfEmpty()
                                                       select q.Item).ToList();

                            listFieldsGrpManual.Clear();

                            var listFieldsGrpMan = (from q in listFields
                                                    where //q.NomorID != null &&
                                                        //q.SpAcc > 0 && 
                                                     listManual.Contains(q.Item)
                                                    group q by new
                                                    {
                                                        q.Item,
                                                        q.SpAcc,
                                                        q.AvgSales,
                                                        q.AvgSalesDivPri,
                                                        q.BackOrder,
                                                        q.Bonus,
                                                        q.Box,
                                                        q.Deviasi,
                                                        q.DivisiPrincipal,
                                                        q.HNA,
                                                        q.Ideal,
                                                        q.Idxnp,
                                                        q.Idxp,
                                                        q.Index,
                                                        q.MOQ,
                                                        q.MOP,
                                                        q.SiT,
                                                        q.SoH,
                                                        q.TypeItem,
                                                        q.Variable,
                                                        q.Pareto,
                                                        q.Via,
                                                        q.Order
                                                    } into g
                                                    select new ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructureField()
                                                    {
                                                        Item = g.Key.Item,
                                                        SpAcc = g.Key.SpAcc,
                                                        AvgSales = g.Key.AvgSales,
                                                        AvgSalesDivPri = g.Key.AvgSalesDivPri,
                                                        BackOrder = g.Key.BackOrder,
                                                        Bonus = g.Key.Bonus,
                                                        Box = g.Key.Box,
                                                        Deviasi = g.Key.Deviasi,
                                                        DivisiPrincipal = g.Key.DivisiPrincipal,
                                                        HNA = g.Key.HNA,
                                                        Ideal = g.Key.Ideal,
                                                        Idxnp = g.Key.Idxnp,
                                                        Idxp = g.Key.Idxp,
                                                        Index = g.Key.Index,
                                                        MOQ = g.Key.MOQ,
                                                        MOP = g.Key.MOP,
                                                        SiT = g.Key.SiT,
                                                        SoH = g.Key.SoH,
                                                        TypeItem = g.Key.TypeItem,
                                                        Variable = g.Key.Variable,
                                                        Pareto = g.Key.Pareto,
                                                        Via = g.Key.Via,
                                                        Order = g.Key.Order
                                                    }).ToList();

                            listFieldsGrp.AddRange(listFieldsGrpMan.ToArray());

                            listFieldsGrpMan.Clear();
                        }

                        #endregion

                        #region Verify SiT

                        listItems = listFieldsGrp.GroupBy(x => x.Item).Select(x => x.Key).ToList();

                        if ((listItems != null) && (listItems.Count > 0))
                        {
                            // Rewrite SiT
                            listItmSiT = (from q in db.LG_SJHs
                                          join q1 in db.LG_SJD2s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                                          where q.l_status == false && q1.n_gsisa != 0
                                           && q.c_gdg2 == '1' && listItems.Contains(q1.c_iteno)
                                          group new { q, q1 } by new { q.c_sjno, q1.c_iteno } into g
                                          select new ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructureField()
                                          {
                                              NomorID = g.Key.c_sjno,
                                              Item = g.Key.c_iteno,
                                              Quantity = (g.Sum(x => x.q1.n_gsisa).HasValue ? g.Sum(x => x.q1.n_gsisa).Value : 0)
                                          }).Distinct().ToList();

                        }

                        #endregion

                        totalDetails = 0;
                        for (nLoop = 0; nLoop < listFieldsGrp.Count; nLoop++)
                        {
                            field = listFieldsGrp[nLoop];

                            if ((field != null) && (!string.IsNullOrEmpty(field.Item)))
                            {
                                if (listOrd1.Where(x => x.c_iteno == field.Item).Count() < 1)
                                {
                                    #region Ord1

                                    ord1 = new LG_ORD1()
                                    {
                                        c_gdg = orh.c_gdg,
                                        c_iteno = field.Item,
                                        c_kddivpri = field.DivisiPrincipal,
                                        c_orno = orID,
                                        c_type = field.TypeItem,
                                        c_via = field.Via,
                                        n_avgsls = field.AvgSales,
                                        n_avgslsdivpri = field.AvgSalesDivPri,
                                        n_beli = field.Beli,
                                        n_bo = field.BackOrder,
                                        n_bonus = field.Bonus,
                                        n_box = field.Box,
                                        n_deviasi = field.Deviasi,
                                        n_ideal = field.Ideal,
                                        n_idxnp = field.Idxnp,
                                        n_idxp = field.Idxp,
                                        n_index = field.Index,
                                        n_order = field.Order,
                                        n_pareto = field.Pareto,
                                        n_pminord = field.MOP,
                                        n_qminord = field.MOQ,
                                        n_salpri = field.HNA,
                                        n_sisa = field.Box == 0 ? field.Deviasi : field.Deviasi * field.Box,
                                        n_qty = field.Order,
                                        //n_qty = field.Order,
                                        n_spacc = field.SpAcc,
                                        n_sit = field.SiT,
                                        n_soh = field.SoH,
                                        n_variabel = field.Variable
                                    };

                                    if (isKhusus)
                                    {
                                        ord1.n_qty = field.SpAcc;
                                        ord1.n_sisa = field.SpAcc;
                                    }

                                    #endregion

                                    #region Ord2

                                    listItmFltr = listFields.Where(x => x.Item == field.Item).ToList();
                                    if ((listItmFltr != null) && (listItmFltr.Count > 0))
                                    {
                                        spQtyReloc = field.SpAcc;
                                        spAlloc = 0;

                                        for (nLoopC = 0; nLoopC < listItmFltr.Count; nLoopC++)
                                        {
                                            field1 = listItmFltr[nLoopC];
                                            if ((field1 != null) && (!string.IsNullOrEmpty(field1.Item))
                                              && (!string.IsNullOrEmpty(field.NomorID)))
                                            {
                                                listOrd2.Add(new LG_ORD2()
                                                {
                                                    c_gdg = orh.c_gdg,
                                                    c_iteno = field.Item,
                                                    c_orno = orID,
                                                    c_spno = field1.NomorID,
                                                    c_type = field1.TypeItem,
                                                    n_sisa = field1.QtySisa,
                                                    c_itemcombo = (field1.IsCombo ? field1.ItemCombo : "0000")
                                                });

                                                spAlloc += field1.QtySisa;
                                            }
                                        }

                                        spDiv = (spQtyReloc - spAlloc);

                                        if ((spDiv > 0) && (field.MOQ > 0))
                                        {
                                            listAdjSp.Add(new ScmsSoaLibrary.Parser.Class.SuratPesananStructureField()
                                            {
                                                Acceptance = spDiv,
                                                IsNew = true,
                                                Item = field.Item,
                                                Keterangan = orh.c_orno,
                                                Quantity = spDiv
                                            });

                                            listOrd2Adj.Add(new LG_ORD2()
                                            {
                                                c_gdg = orh.c_gdg,
                                                c_iteno = field.Item,
                                                c_orno = orID,
                                                c_spno = "X",
                                                c_type = "01",
                                                n_sisa = spDiv
                                            });
                                        }

                                        //if (orh.c_type.Equals("01", StringComparison.OrdinalIgnoreCase) &&
                                        //  orh.c_type.Equals("02", StringComparison.OrdinalIgnoreCase))
                                        //{

                                        //}

                                        listItmFltr.Clear();
                                    }

                                    #endregion

                                    #region Ord4

                                    if ((listItmSiT != null) && (listItmSiT.Count > 0))
                                    {
                                        spQty = listItmSiT.GroupBy(x => x.Item).Sum(x => x.Sum(y => y.Quantity));

                                        ord1.n_sit = (spQty.HasValue ? spQty.Value : 0);

                                        for (nLoopC = 0; nLoopC < listItmSiT.Count; nLoopC++)
                                        {
                                            field1 = listItmSiT[nLoopC];

                                            if ((field1 != null) && (!string.IsNullOrEmpty(field1.Item)))
                                            {
                                                listOrd4.Add(new LG_ORD4()
                                                {
                                                    c_gdg = orh.c_gdg,
                                                    c_iteno = field.Item,
                                                    c_orno = orID,
                                                    c_sjno = field1.NomorID,
                                                    n_sisa = field1.Quantity
                                                });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ord1.n_sit = 0;
                                    }

                                    #endregion

                                    listOrd1.Add(ord1);
                                }
                            }

                            totalDetails++;
                        }

                        listFieldsGrp.Clear();

                        listFields.Clear();

                        listItmSiT.Clear();

                        if (listAdjSp.Count > 0)
                        {
                            sps.Fields.Field = listAdjSp.ToArray();

                            result = this.SuratPesanan(sps, db, false);

                            if (!string.IsNullOrEmpty(result))
                            {
                                ScmsSoaLibraryInterface.Components.PostDataParser pdp = new ScmsSoaLibraryInterface.Components.PostDataParser();

                                ScmsSoaLibraryInterface.Components.PostDataParser.StructureResponse resp = pdp.ResponseParser(result);

                                if (resp.IsSet && resp.Response == ScmsSoaLibraryInterface.Components.PostDataParser.ResponseStatus.Success)
                                {
                                    tmpNumbering = (resp.Values.ContainsKey("SP") ? resp.Values["SP"] ?? string.Empty : string.Empty);
                                    if (!string.IsNullOrEmpty(tmpNumbering))
                                    {
                                        listOrd2Adj.ForEach(delegate(LG_ORD2 t)
                                        {
                                            t.c_spno = tmpNumbering;
                                        });

                                        listOrd2.AddRange(listOrd2Adj.ToArray());
                                    }
                                }
                            }

                            listAdjSp.Clear();
                            listOrd2Adj.Clear();
                        }

                        if ((listOrd1 != null) && (listOrd1.Count > 0))
                        {
                            db.LG_ORD1s.InsertAllOnSubmit(listOrd1.ToArray());
                            listOrd1.Clear();
                        }

                        if ((listOrd2 != null) && (listOrd2.Count > 0))
                        {
                            db.LG_ORD2s.InsertAllOnSubmit(listOrd2.ToArray());
                            listOrd2.Clear();
                        }

                        if ((listOrd4 != null) && (listOrd4.Count > 0))
                        {
                            db.LG_ORD4s.InsertAllOnSubmit(listOrd4.ToArray());
                            listOrd4.Clear();
                        }
                    }

                    #endregion

                    dic = new Dictionary<string, string>();

                    dic.Add("OR", orID);
                    dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                    result = string.Format("Total {0} detail(s)", totalDetails);

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
                rpe = ResponseParser.ResponseParserEnum.IsError;

                result = string.Format("ScmsSoaLibrary.Bussiness.Pembelian:OrderRequestProcess - {0}", ex.Message);

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

        public string OrderRequestGudang(ScmsSoaLibrary.Parser.Class.OrderRequestGudangStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false;

            string result = null;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            LG_SPGH spgh = null;

            ScmsSoaLibrary.Parser.Class.OrderRequestGudangStructureField field = null;
            string nipEntry = null;
            string spgID = null;
            //string tmpNumbering = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now;

            //decimal? spQty = 0,
            //  nPrice = 0;
            //decimal spQtyReloc = 0,
            //  rnQty = 0,
            //  spAlloc = 0;

            List<LG_SPGD1> listSpg1 = null;
            List<LG_SPGD2> listSpg2 = null;
            //List<LG_SPGD3> listSpg3 = null;
            List<LG_SPGD4> listSpg4 = null;

            LG_SPGD1 spg1 = null;
            //LG_SPGD2 spg2 = null;
            //LG_SPGD3 spg3 = null;
            //LG_SPGD4 spg4 = null;

            ORHelper orhlp = null;

            bool isConfirm = false;

            int nLoop = 0;

            int totalDetails = 0;

            IDictionary<string, string> dic = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            spgID = (structure.Fields.OrderRequestID ?? string.Empty);

            char gdg1 = (string.IsNullOrEmpty(structure.Fields.GudangFrom) ? char.MinValue : structure.Fields.GudangFrom[0]),
              gdg2 = char.MaxValue;

            if (gdg1.Equals(char.MinValue))
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

                    gdg1 = (string.IsNullOrEmpty(structure.Fields.GudangFrom) ? char.MinValue : structure.Fields.GudangFrom[0]);
                    gdg2 = (string.IsNullOrEmpty(structure.Fields.GudangTo) ? char.MinValue : structure.Fields.GudangTo[0]);

                    if (!string.IsNullOrEmpty(spgID))
                    {
                        result = "Nomor Order Request harus kosong.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (string.IsNullOrEmpty(structure.Fields.Suplier))
                    {
                        result = "Nama suplier dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (gdg1 == char.MinValue)
                    {
                        result = "Gudang asal tidak terbaca.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (gdg2 == char.MinValue)
                    {
                        result = "Gudang tujuan tidak terbaca.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, date))
                    {
                        result = "Surat gudang tidak dapat disimpan, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    spgID = Commons.GenerateNumbering<LG_SPGH>(db, "SG", '3', "20", date, "c_spgno");

                    //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "SPG");

                    spgh = new LG_SPGH()
                    {
                        c_gdg1 = gdg1,
                        c_gdg2 = gdg2,
                        c_nosup = structure.Fields.Suplier,
                        d_spgdate = date,
                        c_spgno = spgID,
                        c_entry = nipEntry,
                        c_type = structure.Fields.TipeORG,
                        c_update = nipEntry,
                        d_entry = date,
                        d_update = date,
                        v_ket = structure.Fields.Keterangan,
                        l_print = false,
                        l_status = false,
                    };

                    db.LG_SPGHs.InsertOnSubmit(spgh);

                    if (!string.IsNullOrEmpty(structure.Fields.TanggalSend))
                    {
                        spgh.d_sgsender = structure.Fields.TanggalSendFormat;
                    }

                    #region Old Coded

                    //db.SubmitChanges();

                    //spgh = (from q in db.LG_SPGHs
                    //        where q.v_ket == tmpNumbering
                    //        select q).Take(1).SingleOrDefault();

                    //if (spgh == null)
                    //{
                    //  result = "Nomor Order Request tidak dapat di raih.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}
                    //else if (spgh.c_spgno.Equals("XXXXXXXXXX"))
                    //{
                    //  result = "Trigger Order Request tidak aktif.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}

                    //spgh.v_ket = structure.Fields.Keterangan;

                    //spgID = spgh.c_spgno;

                    #endregion

                    if ((!string.IsNullOrEmpty(spgh.c_type)) && spgh.c_type.Equals("01", StringComparison.OrdinalIgnoreCase))
                    {
                        #region Insert Detail (Manual)

                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            listSpg1 = new List<LG_SPGD1>();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                field = structure.Fields.Field[nLoop];

                                if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0) && (field.QuantityOrder > 0))
                                {
                                    orhlp = (from q in db.FA_MasItms
                                             join q1 in db.LG_DatSups on q.c_nosup equals q1.c_nosup
                                             join q2 in db.FA_Divpris on q.c_iteno equals q2.c_iteno
                                             join q3 in db.FA_MsDivPris on q2.c_kddivpri equals q3.c_kddivpri into q_3
                                             from qMDP in q_3.DefaultIfEmpty()
                                             where q.c_iteno == field.Item
                                             select new ORHelper()
                                             {
                                                 c_iteno = q.c_iteno,
                                                 v_itnam = q.v_itnam,
                                                 n_pminord = (q.n_pminord.HasValue ? q.n_pminord.Value : 0),
                                                 n_qminord = (q.n_qminord.HasValue ? q.n_qminord.Value : 0),
                                                 c_type = q.c_type,
                                                 c_via = q.c_via,
                                                 n_beli = (q.n_beli.HasValue ? q.n_beli.Value : 0),
                                                 n_box = (q.n_box.HasValue ? q.n_box.Value : 0),
                                                 n_salpri = (q.n_salpri.HasValue ? q.n_salpri.Value : 0),
                                                 n_index = (q1.n_index.HasValue ? q1.n_index.Value : 0),
                                                 c_nosup = q1.c_nosup,
                                                 c_kddivpri = q2.c_kddivpri,
                                                 v_nmdivpri = qMDP.v_nmdivpri
                                             }).Take(1).SingleOrDefault();

                                    if (orhlp != null)
                                    {
                                        listSpg1.Add(new LG_SPGD1()
                                        {
                                            c_gdg = gdg1,
                                            c_iteno = field.Item,
                                            c_kddivpri = orhlp.c_kddivpri,
                                            c_spgno = spgID,
                                            c_type = orhlp.c_type,
                                            c_via = orhlp.c_via,
                                            n_avgsls = 0,
                                            n_avgslsdivpri = 0,
                                            n_beli = orhlp.n_beli,
                                            n_bonus = 0,
                                            n_box = orhlp.n_box,
                                            n_deviasi = 0,
                                            n_ideal = 0,
                                            n_idxnp = 0,
                                            n_idxp = 0,
                                            n_index = orhlp.n_index,
                                            n_order = 0,
                                            n_pareto = 0,
                                            n_pminord = orhlp.n_pminord,
                                            n_qminord = orhlp.n_qminord,
                                            n_qty = field.Quantity,
                                            n_salpri = orhlp.n_salpri,
                                            n_sisa = field.QuantityOrder,
                                            n_sit = 0,
                                            n_soh = 0,
                                            n_spacc = field.Quantity,
                                            n_variabel = 0
                                        });
                                    }
                                }

                                totalDetails++;
                            }

                            if (listSpg1.Count > 0)
                            {
                                db.LG_SPGD1s.InsertAllOnSubmit(listSpg1.ToArray());
                                listSpg1.Clear();
                            }
                        }

                        #endregion
                    }

                    dic = new Dictionary<string, string>();

                    dic.Add("SPG", spgID);
                    dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                    result = string.Format("Total {0} detail(s)", totalDetails);

                    hasAnyChanges = true;

                    #endregion
                }
                else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    if (string.IsNullOrEmpty(spgID))
                    {
                        result = "Nomor Order Request dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    spgh = (from q in db.LG_SPGHs
                            where (q.c_gdg1 == gdg1) && (q.c_spgno == spgID)
                            select q).Take(1).SingleOrDefault();

                    if (spgh == null)
                    {
                        result = "Nomor Order Request tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (spgh.l_delete.HasValue && spgh.l_delete.Value)
                    {
                        result = "Tidak dapat mengubah Order Request yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    //else if (Commons.IsClosingLogistik(db, spgh.d_spgdate))
                    //{
                    //  result = "Surat gudang tidak dapat diubah, karena sudah closing.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}
                    else if (Commons.HasPO(db, spgID))
                    {
                        result = "Tidak dapat mengubah Order Request yang sudah terkirim ke principal.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        db.Transaction.Rollback();

                        goto endLogic;
                    }

                    if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
                    {
                        spgh.v_ket = structure.Fields.Keterangan;
                    }

                    if (!string.IsNullOrEmpty(structure.Fields.TanggalSend))
                    {
                        spgh.d_sgsender = structure.Fields.TanggalSendFormat;
                    }

                    spgh.c_update = nipEntry;
                    spgh.d_update = DateTime.Now;

                    #region Populate Detail

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        if ((!string.IsNullOrEmpty(spgh.c_type)))
                        {
                            #region Manual

                            listSpg1 = new List<LG_SPGD1>();
                            listSpg4 = new List<LG_SPGD4>();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                field = structure.Fields.Field[nLoop];

                                if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified) && (!field.isReset) && (field.Quantity > 0) && (field.QuantityOrder > 0))
                                {
                                    #region New

                                    var qryItemEntry = (from q in db.FA_MasItms
                                                        join q1 in db.LG_DatSups on q.c_nosup equals q1.c_nosup
                                                        join q2 in db.FA_Divpris on q.c_iteno equals q2.c_iteno
                                                        join q3 in db.FA_MsDivPris on q2.c_kddivpri equals q3.c_kddivpri into q_3
                                                        from qMDP in q_3.DefaultIfEmpty()
                                                        where q.c_iteno == field.Item
                                                        select new
                                                        {
                                                            q.c_iteno,
                                                            q.v_itnam,
                                                            q.n_pminord,
                                                            q.n_qminord,
                                                            q.c_type,
                                                            q.c_via,
                                                            q.n_beli,
                                                            q.n_box,
                                                            q.n_salpri,
                                                            q1.n_index,
                                                            q1.c_nosup,
                                                            q2.c_kddivpri,
                                                            qMDP.v_nmdivpri
                                                        }).Take(1).SingleOrDefault();

                                    if (qryItemEntry != null)
                                    {
                                        listSpg1.Add(new LG_SPGD1()
                                        {
                                            c_gdg = spgh.c_gdg1,
                                            c_iteno = field.Item,
                                            c_kddivpri = qryItemEntry.c_kddivpri,
                                            c_spgno = spgID,
                                            c_type = qryItemEntry.c_type,
                                            c_via = qryItemEntry.c_via,
                                            n_avgsls = 0,
                                            n_avgslsdivpri = 0,
                                            n_beli = qryItemEntry.n_beli,
                                            n_bonus = 0,
                                            n_box = qryItemEntry.n_box,
                                            n_deviasi = 0,
                                            n_ideal = 0,
                                            n_idxnp = 0,
                                            n_idxp = 0,
                                            n_index = qryItemEntry.n_index,
                                            n_order = 0,
                                            n_pareto = 0,
                                            n_pminord = qryItemEntry.n_pminord,
                                            n_qminord = qryItemEntry.n_qminord,
                                            n_qty = field.QuantityOrder,
                                            n_salpri = qryItemEntry.n_salpri,
                                            n_sisa = field.Quantity,
                                            n_sit = 0,
                                            n_soh = 0,
                                            n_spacc = 0,
                                            n_variabel = 0
                                        });

                                        listSpg4.Add(new LG_SPGD4()
                                        {
                                            c_entry = nipEntry,
                                            c_gdg = spgh.c_gdg1,
                                            c_gdg_to = (spgh.c_gdg2.HasValue ? spgh.c_gdg2.Value : '0'),
                                            c_iteno = field.Item,
                                            c_no = null,
                                            c_spgno = spgID,
                                            d_entry = date,
                                            n_qty = field.Quantity,
                                            n_sisa = field.QuantityOrder,
                                            v_type = "01"
                                        });
                                    }

                                    totalDetails++;

                                    #endregion
                                }
                                else if ((field != null) && (!field.IsNew) && field.IsDelete && (!field.IsModified) && (!field.isReset))
                                {
                                    #region Delete

                                    spg1 = (from q in db.LG_SPGD1s
                                            where q.c_spgno == spgID && q.c_iteno == field.Item
                                            && q.n_sisa == q.n_qty
                                            select q).Take(1).SingleOrDefault();

                                    if (spg1 != null)
                                    {
                                        listSpg4.Add(new LG_SPGD4()
                                        {
                                            c_entry = nipEntry,
                                            c_gdg = spg1.c_gdg,
                                            c_gdg_to = (spgh.c_gdg2.HasValue ? spgh.c_gdg2.Value : '0'),
                                            c_iteno = field.Item,
                                            c_no = null,
                                            c_spgno = spgID,
                                            d_entry = date,
                                            n_qty = spg1.n_qty,
                                            n_sisa = spg1.n_sisa,
                                            v_ket_del = field.Keterangan,
                                            v_type = "03"
                                        });

                                        db.LG_SPGD1s.DeleteOnSubmit(spg1);
                                    }

                                    #endregion
                                }
                                else if ((field != null) && (!field.IsNew) && (!field.IsDelete) && (field.IsModified) && (field.isReset))
                                {
                                    #region Reset

                                    spg1 = (from q in db.LG_SPGD1s
                                            where q.c_spgno == spgID && q.c_iteno == field.Item
                                            select q).Take(1).SingleOrDefault();

                                    if (spg1 != null)
                                    {
                                        listSpg4.Add(new LG_SPGD4()
                                        {
                                            c_entry = nipEntry,
                                            c_gdg = spg1.c_gdg,
                                            c_gdg_to = (spgh.c_gdg2.HasValue ? spgh.c_gdg2.Value : '0'),
                                            c_iteno = field.Item,
                                            c_no = null,
                                            c_spgno = spgID,
                                            d_entry = date,
                                            n_qty = spg1.n_qty,
                                            n_sisa = spg1.n_sisa,
                                            v_ket_del = field.Keterangan,
                                            v_type = "03"
                                        });

                                        spg1.n_sisa = 0;
                                    }

                                    #endregion
                                }
                            }

                            if (listSpg1.Count > 0)
                            {
                                db.LG_SPGD1s.InsertAllOnSubmit(listSpg1.ToArray());
                                listSpg1.Clear();
                            }

                            if (listSpg4.Count > 0)
                            {
                                db.LG_SPGD4s.InsertAllOnSubmit(listSpg4.ToArray());
                                listSpg4.Clear();
                            }

                            #endregion
                        }
                    }

                    #endregion

                    hasAnyChanges = true;

                    #endregion
                }
                else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    if (string.IsNullOrEmpty(spgID))
                    {
                        result = "Nomor Order Request dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        db.Transaction.Rollback();

                        goto endLogic;
                    }

                    spgh = (from q in db.LG_SPGHs
                            where (q.c_gdg1 == gdg1) && (q.c_spgno == spgID)
                            select q).Take(1).SingleOrDefault();

                    if (spgh == null)
                    {
                        result = "Nomor Order Request tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        db.Transaction.Rollback();

                        goto endLogic;
                    }
                    else if (spgh.l_delete.HasValue && spgh.l_delete.Value)
                    {
                        result = "Tidak dapat mengubah Order Request yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        db.Transaction.Rollback();

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, spgh.d_spgdate))
                    {
                        result = "Surat gudang tidak dapat dihapus, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    //else if (HasPO(db, spgID))
                    //{
                    //  result = "Tidak dapat menghapus Order Request yang sudah terkirim ke principal.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  db.Transaction.Rollback();

                    //  goto endLogic;
                    //}

                    //if ((!string.IsNullOrEmpty(spgh.c_type)) && spgh.c_type.Equals("01", StringComparison.OrdinalIgnoreCase))
                    //{

                    //}

                    spgh.c_update = nipEntry;
                    spgh.d_update = DateTime.Now;

                    spgh.l_delete = true;
                    spgh.v_ket_mark = structure.Fields.Keterangan;

                    #region Manual

                    listSpg1 = (from q in db.LG_SPGD1s
                                where q.c_spgno == spgID
                                select q).ToList();

                    if (listSpg1 != null)
                    {
                        listSpg4 = new List<LG_SPGD4>();

                        for (nLoop = 0; nLoop < listSpg1.Count; nLoop++)
                        {
                            spg1 = listSpg1[nLoop];

                            if (spg1 != null)
                            {
                                listSpg4.Add(new LG_SPGD4()
                                {
                                    c_entry = nipEntry,
                                    c_gdg = spg1.c_gdg,
                                    c_gdg_to = (spgh.c_gdg2.HasValue ? spgh.c_gdg2.Value : '0'),
                                    c_iteno = spg1.c_iteno,
                                    c_no = null,
                                    c_spgno = spgID,
                                    d_entry = date,
                                    n_qty = spg1.n_qty,
                                    n_sisa = spg1.n_sisa,
                                    v_ket_del = structure.Fields.Keterangan,
                                    v_type = "03"
                                });
                            }
                        }

                        if (listSpg1.Count > 0)
                        {
                            db.LG_SPGD1s.DeleteAllOnSubmit(listSpg1.ToArray());
                            listSpg1.Clear();
                        }

                        if (listSpg4.Count > 0)
                        {
                            db.LG_SPGD4s.InsertAllOnSubmit(listSpg4.ToArray());
                            listSpg4.Clear();
                        }
                    }

                    listSpg2 = (from q in db.LG_SPGD2s
                                where q.c_spgno == spgID
                                select q).ToList();

                    if ((listSpg2 != null) && (listSpg2.Count > 0))
                    {
                        db.LG_SPGD2s.DeleteAllOnSubmit(listSpg2.ToArray());
                        listSpg2.Clear();
                    }

                    hasAnyChanges = true;

                    #endregion

                    #endregion
                }
                else if (structure.Method.Equals("Submit", StringComparison.OrdinalIgnoreCase))
                {
                    #region Submit

                    if (string.IsNullOrEmpty(spgID))
                    {
                        result = "Nomor Order Request dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    spgh = (from q in db.LG_SPGHs
                            where (q.c_gdg1 == gdg1) && (q.c_spgno == spgID)
                            select q).Take(1).SingleOrDefault();

                    if (spgh == null)
                    {
                        result = "Nomor Order Request tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (spgh.l_delete.HasValue && spgh.l_delete.Value)
                    {
                        result = "Tidak dapat mengubah Order Request yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, spgh.d_spgdate))
                    {
                        result = "Surat gudang tidak dapat diubah, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    isConfirm = (spgh.l_status.HasValue ? spgh.l_status.Value : false);

                    if (structure.Fields.ConfirmedProcess && (!isConfirm))
                    {
                        spgh.l_status = structure.Fields.ConfirmedProcess;

                        spgh.c_update = nipEntry;
                        spgh.d_update = date;

                        hasAnyChanges = true;
                    }
                    else
                    {
                        result = "Status Pesanan Gudang terkonfirmasi.";
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

                result = string.Format("ScmsSoaLibrary.Bussiness.Pembelian:OrderRequestGudang - {0}", ex.Message);

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

        public string OrderRequestProcessGudang(ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false;

            string result = null;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            LG_SPGH spgh = null;

            ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructureField field = null;
            ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructureField field1 = null;
            string nipEntry = null;
            string spgID = null;
            //string tmpNumbering = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now;

            //decimal? spQty = 0,
            //  nPrice = 0;
            decimal spQtyReloc = 0,
                //spDiv = 0,
              spAlloc = 0;

            List<LG_SPGD1> listSpgd1 = null;
            List<LG_SPGD2> listSpgd2 = null;
            //List<LG_SPGD2> listSpgd2Adj = null;
            //List<LG_SPGD3> listSpgd3 = null;
            //List<LG_SPGD4> listSpgd4 = null;

            LG_SPGD1 spgd1 = null;
            //LG_SPGD2 spgd2 = null;
            //LG_SPGD3 spgd3 = null;
            //LG_SPGD4 spgd4 = null;

            //List<string> listItems = null;

            List<ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructureField> listFields = new List<ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructureField>();
            List<ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructureField> listItmFltr = null;
            //List<ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructureField> listItmSiT = null;

            List<ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructureField> listFieldsGrpMan = null,
              listFieldsGrp = null;

            List<string> listFieldsGrpItemManual = null;

            int nLoop = 0,
              nLoopC = 0;

            char gdgFrom = char.MinValue,
              gdgTo = char.MinValue;

            IDictionary<string, string> dic = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }
            int totalDetails = 0;

            spgID = (structure.Fields.OrderRequestID ?? string.Empty);

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
                {
                    #region Add

                    gdgFrom = (string.IsNullOrEmpty(structure.Fields.Gudang) ? char.MinValue : structure.Fields.Gudang[0]);
                    gdgTo = (string.IsNullOrEmpty(structure.Fields.GudangTo) ? char.MinValue : structure.Fields.GudangTo[0]);

                    if (gdgFrom.Equals(char.MinValue) || (gdgTo.Equals(char.MinValue)))
                    {
                        result = "Gudang tujuan dan asal harus terdefinisi.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (!string.IsNullOrEmpty(spgID))
                    {
                        result = "Nomor Order Request harus kosong.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (string.IsNullOrEmpty(structure.Fields.Suplier))
                    {
                        result = "Nama suplier dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (gdgFrom == char.MinValue)
                    {
                        result = "Gudang asal tidak terbaca.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (gdgTo == char.MinValue)
                    {
                        result = "Gudang tujuan tidak terbaca.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, date))
                    {
                        result = "Surat gudang tidak dapat disimpan, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    spgID = Commons.GenerateNumbering<LG_SPGH>(db, "SG", '3', "20", date, "c_spgno");

                    //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "SPGP");

                    spgh = new LG_SPGH()
                    {
                        c_entry = nipEntry,
                        c_gdg1 = gdgFrom,
                        c_gdg2 = gdgTo,
                        c_nosup = structure.Fields.Suplier,
                        c_spgno = spgID,
                        c_type = structure.Fields.TipeOR,
                        c_update = nipEntry,
                        d_entry = date,
                        d_spgdate = date.Date,
                        d_spsubmit = null,
                        d_update = date,
                        l_delete = false,
                        l_print = false,
                        l_status = false,
                        v_ket = structure.Fields.Keterangan,
                        v_ket_mark = string.Empty
                    };

                    db.LG_SPGHs.InsertOnSubmit(spgh);

                    #region Old Coded

                    //db.SubmitChanges();

                    //spgh = (from q in db.LG_SPGHs
                    //        where q.v_ket == tmpNumbering
                    //        select q).Take(1).SingleOrDefault();

                    //if (spgh == null)
                    //{
                    //  result = "Nomor Surat Pesanan Gudang tidak dapat di raih.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}

                    //if (spgh.c_spgno.Equals("XXXXXXXXXX"))
                    //{
                    //  result = "Trigger Surat Pesanan Gudang tidak aktif.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}

                    //spgh.v_ket = structure.Fields.Keterangan;

                    //spgID = spgh.c_spgno;

                    #endregion

                    #region Insert Detail (Manual)

                    listSpgd1 = new List<LG_SPGD1>();
                    listSpgd2 = new List<LG_SPGD2>();
                    //listSpgd2Adj = new List<LG_ORD2>();
                    //listSpgd3 = new List<LG_SPGD3>();

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        listFields.AddRange(structure.Fields.Field);

                        #region Verify Fields

                        //listFieldsGrp = (from q in listFields
                        //                 where q.NomorID != null &&
                        //                  q.SpAcc > 0 && !q.Manual
                        //                 group q by new
                        //                 {
                        //                   q.Item,
                        //                   q.SpAcc,
                        //                   q.AvgSales,
                        //                   q.AvgSalesDivPri,
                        //                   q.BackOrder,
                        //                   q.Bonus,
                        //                   q.Box,
                        //                   q.Deviasi,
                        //                   q.DivisiPrincipal,
                        //                   q.HNA,
                        //                   q.Ideal,
                        //                   q.Idxnp,
                        //                   q.Idxp,
                        //                   q.Index,
                        //                   q.MOQ,
                        //                   q.MOP,
                        //                   q.SiT,
                        //                   q.SoH,
                        //                   q.TypeItem,
                        //                   q.Variable,
                        //                   q.Pareto,
                        //                   q.Via
                        //                 } into g
                        //                 select new ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructureField()
                        //                 {
                        //                   Item = g.Key.Item,
                        //                   SpAcc = g.Key.SpAcc,
                        //                   AvgSales = g.Key.AvgSales,
                        //                   AvgSalesDivPri = g.Key.AvgSalesDivPri,
                        //                   BackOrder = g.Key.BackOrder,
                        //                   Bonus = g.Key.Bonus,
                        //                   Box = g.Key.Box,
                        //                   Deviasi = g.Key.Deviasi,
                        //                   DivisiPrincipal = g.Key.DivisiPrincipal,
                        //                   HNA = g.Key.HNA,
                        //                   Ideal = g.Key.Ideal,
                        //                   Idxnp = g.Key.Idxnp,
                        //                   Idxp = g.Key.Idxp,
                        //                   Index = g.Key.Index,
                        //                   MOQ = g.Key.MOQ,
                        //                   MOP = g.Key.MOP,
                        //                   SiT = g.Key.SiT,
                        //                   SoH = g.Key.SoH,
                        //                   TypeItem = g.Key.TypeItem,
                        //                   Variable = g.Key.Variable,
                        //                   Pareto = g.Key.Pareto,
                        //                   Via = g.Key.Via
                        //                 }).ToList();

                        //listFieldsGrpItemManual = (from q in listFields
                        //                           where (q.NomorID != null) && (q.SpAcc > 0) && q.Manual
                        //                           group q by q.Item into g
                        //                           select g.Key).ToList();

                        //if (listFieldsGrpItemManual.Count > 0)
                        //{
                        //  //listManual = (from q in listFieldsGrpItemManual
                        //  //              join q1 in listFieldsGrp on q.Item equals q1.Item into q_1
                        //  //              from qLG in q_1.DefaultIfEmpty()
                        //  //              select q.Item).ToList();

                        //  //listManual = (from q in listFieldsGrpItemManual
                        //  //              //join q1 in listFieldsGrp on q.Item equals q1.Item into q_1
                        //  //              //from qLG in q_1.DefaultIfEmpty()
                        //  //              where listFieldsGrp.Contains(q)
                        //  //              select q.Item).ToList();

                        //  //listFieldsGrpItemManual.Clear();

                        //  listFieldsGrpMan = (from q in listFields
                        //                      where (q.NomorID != null) &&
                        //                       q.SpAcc > 0 && listManual.Contains(q.Item)
                        //                      group q by new
                        //                      {
                        //                        q.Item,
                        //                        q.SpAcc,
                        //                        q.AvgSales,
                        //                        q.AvgSalesDivPri,
                        //                        q.BackOrder,
                        //                        q.Bonus,
                        //                        q.Box,
                        //                        q.Deviasi,
                        //                        q.DivisiPrincipal,
                        //                        q.HNA,
                        //                        q.Ideal,
                        //                        q.Idxnp,
                        //                        q.Idxp,
                        //                        q.Index,
                        //                        q.MOQ,
                        //                        q.MOP,
                        //                        q.SiT,
                        //                        q.SoH,
                        //                        q.TypeItem,
                        //                        q.Variable,
                        //                        q.Pareto,
                        //                        q.Via
                        //                      } into g
                        //                      select new ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructureField()
                        //                      {
                        //                        Item = g.Key.Item,
                        //                        SpAcc = g.Key.SpAcc,
                        //                        AvgSales = g.Key.AvgSales,
                        //                        AvgSalesDivPri = g.Key.AvgSalesDivPri,
                        //                        BackOrder = g.Key.BackOrder,
                        //                        Bonus = g.Key.Bonus,
                        //                        Box = g.Key.Box,
                        //                        Deviasi = g.Key.Deviasi,
                        //                        DivisiPrincipal = g.Key.DivisiPrincipal,
                        //                        HNA = g.Key.HNA,
                        //                        Ideal = g.Key.Ideal,
                        //                        Idxnp = g.Key.Idxnp,
                        //                        Idxp = g.Key.Idxp,
                        //                        Index = g.Key.Index,
                        //                        MOQ = g.Key.MOQ,
                        //                        MOP = g.Key.MOP,
                        //                        SiT = g.Key.SiT,
                        //                        SoH = g.Key.SoH,
                        //                        TypeItem = g.Key.TypeItem,
                        //                        Variable = g.Key.Variable,
                        //                        Pareto = g.Key.Pareto,
                        //                        Via = g.Key.Via
                        //                      }).ToList();

                        //  listFieldsGrp.AddRange(listFieldsGrpMan.ToArray());

                        //  listFieldsGrpMan.Clear();
                        //}

                        #endregion

                        #region Verify Fields

                        listFieldsGrp = (from q in listFields
                                         where //q.NomorID != null &&
                                             //q.SpAcc > 0 && 
                                          !q.Manual
                                         group q by new
                                         {
                                             q.Item,
                                             q.SpAcc,
                                             q.AvgSales,
                                             q.AvgSalesDivPri,
                                             q.BackOrder,
                                             q.Bonus,
                                             q.Box,
                                             q.Deviasi,
                                             q.DivisiPrincipal,
                                             q.HNA,
                                             q.Ideal,
                                             q.Idxnp,
                                             q.Idxp,
                                             q.Index,
                                             q.MOQ,
                                             q.MOP,
                                             q.SiT,
                                             q.SoH,
                                             q.TypeItem,
                                             q.Variable,
                                             q.Pareto,
                                             q.Via,
                                             q.QtyOR
                                         } into g
                                         select new ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructureField()
                                         {
                                             Item = g.Key.Item,
                                             SpAcc = g.Key.SpAcc,
                                             AvgSales = g.Key.AvgSales,
                                             AvgSalesDivPri = g.Key.AvgSalesDivPri,
                                             BackOrder = g.Key.BackOrder,
                                             Bonus = g.Key.Bonus,
                                             Box = g.Key.Box,
                                             Deviasi = g.Key.Deviasi,
                                             DivisiPrincipal = g.Key.DivisiPrincipal,
                                             HNA = g.Key.HNA,
                                             Ideal = g.Key.Ideal,
                                             Idxnp = g.Key.Idxnp,
                                             Idxp = g.Key.Idxp,
                                             Index = g.Key.Index,
                                             MOQ = g.Key.MOQ,
                                             MOP = g.Key.MOP,
                                             SiT = g.Key.SiT,
                                             SoH = g.Key.SoH,
                                             TypeItem = g.Key.TypeItem,
                                             Variable = g.Key.Variable,
                                             Pareto = g.Key.Pareto,
                                             Via = g.Key.Via,
                                             QtyOR = g.Key.QtyOR
                                         }).ToList();

                        //listFieldsGrpItemManual = (from q in listFields
                        //                           where q.NomorID != null &&
                        //                            q.SpAcc > 0 && q.Manual
                        //                           group q by new
                        //                           {
                        //                             q.Item,
                        //                           } into g
                        //                           select g.Key).ToList();

                        listFieldsGrpItemManual = (from q in listFields
                                                   where (q.NomorID != null) && (q.SpAcc > 0) && q.Manual
                                                   group q by q.Item into g
                                                   select g.Key).ToList();

                        if (listFieldsGrpItemManual.Count > 0)
                        {
                            //List<string> listManual = (from q in listFieldsGrpManual
                            //                           join q1 in listFieldsGrp on q.Item equals q1.Item into q_1
                            //                           from qLG in q_1.DefaultIfEmpty()
                            //                           select q.Item).ToList();

                            //listFieldsGrpManual.Clear();

                            listFieldsGrpMan = (from q in listFields
                                                where q.NomorID != null &&
                                                 q.SpAcc > 0 && listFieldsGrpItemManual.Contains(q.Item)
                                                group q by new
                                                {
                                                    q.Item,
                                                    q.SpAcc,
                                                    q.AvgSales,
                                                    q.AvgSalesDivPri,
                                                    q.BackOrder,
                                                    q.Bonus,
                                                    q.Box,
                                                    q.Deviasi,
                                                    q.DivisiPrincipal,
                                                    q.HNA,
                                                    q.Ideal,
                                                    q.Idxnp,
                                                    q.Idxp,
                                                    q.Index,
                                                    q.MOQ,
                                                    q.MOP,
                                                    q.SiT,
                                                    q.SoH,
                                                    q.TypeItem,
                                                    q.Variable,
                                                    q.Pareto,
                                                    q.Via,
                                                    q.QtyOR
                                                } into g
                                                select new ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructureField()
                                                {
                                                    Item = g.Key.Item,
                                                    SpAcc = g.Key.SpAcc,
                                                    AvgSales = g.Key.AvgSales,
                                                    AvgSalesDivPri = g.Key.AvgSalesDivPri,
                                                    BackOrder = g.Key.BackOrder,
                                                    Bonus = g.Key.Bonus,
                                                    Box = g.Key.Box,
                                                    Deviasi = g.Key.Deviasi,
                                                    DivisiPrincipal = g.Key.DivisiPrincipal,
                                                    HNA = g.Key.HNA,
                                                    Ideal = g.Key.Ideal,
                                                    Idxnp = g.Key.Idxnp,
                                                    Idxp = g.Key.Idxp,
                                                    Index = g.Key.Index,
                                                    MOQ = g.Key.MOQ,
                                                    MOP = g.Key.MOP,
                                                    SiT = g.Key.SiT,
                                                    SoH = g.Key.SoH,
                                                    TypeItem = g.Key.TypeItem,
                                                    Variable = g.Key.Variable,
                                                    Pareto = g.Key.Pareto,
                                                    Via = g.Key.Via,
                                                    QtyOR = g.Key.QtyOR
                                                }).ToList();

                            listFieldsGrp.AddRange(listFieldsGrpMan.ToArray());

                            listFieldsGrpMan.Clear();
                            listFieldsGrpItemManual.Clear();
                        }

                        #endregion

                        #region Verify SiT

                        //listItems = listFieldsGrp.GroupBy(x => x.Item).Select(x => x.Key).ToList();

                        //if ((listItems != null) && (listItems.Count > 0))
                        //{
                        //  // Rewrite SiT
                        //  listItmSiT = (from q in db.LG_SJHs
                        //                join q1 in db.LG_SJD2s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                        //                where q.l_status == false && q1.n_gsisa != 0
                        //                 && q.c_gdg2 == '1' && listItems.Contains(q1.c_iteno)
                        //                group new { q, q1 } by new { q.c_sjno, q1.c_iteno } into g
                        //                select new ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructureField()
                        //                {
                        //                  NomorID = g.Key.c_sjno,
                        //                  Item = g.Key.c_iteno,
                        //                  Quantity = (g.Sum(x => x.q1.n_gsisa).HasValue ? g.Sum(x => x.q1.n_gsisa).Value : 0)
                        //                }).Distinct().ToList();

                        //}

                        #endregion

                        totalDetails = 0;

                        for (nLoop = 0; nLoop < listFieldsGrp.Count; nLoop++)
                        {
                            field = listFieldsGrp[nLoop];

                            if ((field != null) && (!string.IsNullOrEmpty(field.Item)))
                            {
                                if (listSpgd1.Where(x => x.c_iteno == field.Item).Count() < 1)
                                {
                                    #region Ord1

                                    spgd1 = new LG_SPGD1()
                                    {
                                        c_gdg = gdgFrom,
                                        c_iteno = field.Item,
                                        c_kddivpri = field.DivisiPrincipal,
                                        c_spgno = spgID,
                                        c_type = field.TypeItem,
                                        c_via = field.Via,
                                        n_avgsls = field.AvgSales,
                                        n_avgslsdivpri = field.AvgSalesDivPri,
                                        n_beli = field.Beli,
                                        n_bonus = field.Bonus,
                                        n_box = field.Box,
                                        n_deviasi = field.Deviasi,
                                        n_ideal = field.Ideal,
                                        n_idxnp = field.Idxnp,
                                        n_idxp = field.Idxp,
                                        n_index = field.Index,
                                        n_order = field.Order,
                                        n_pareto = field.Pareto,
                                        n_pminord = field.MOP,
                                        n_qminord = field.MOQ,
                                        n_salpri = field.HNA,
                                        n_sisa = field.QtyOR,
                                        //n_qty = field.Order,
                                        n_qty = field.QtyOR,
                                        n_spacc = field.SpAcc,
                                        n_sit = field.SiT,
                                        n_soh = field.SoH,
                                        n_variabel = field.Variable
                                    };

                                    #endregion

                                    #region Ord2

                                    listItmFltr = listFields.Where(x => x.Item == field.Item).ToList();
                                    if ((listItmFltr != null) && (listItmFltr.Count > 0))
                                    {
                                        spQtyReloc = field.SpAcc;
                                        spAlloc = 0;

                                        for (nLoopC = 0; nLoopC < listItmFltr.Count; nLoopC++)
                                        {
                                            field1 = listItmFltr[nLoopC];
                                            if ((field1 != null) && (!string.IsNullOrEmpty(field1.Item))
                                              && (!string.IsNullOrEmpty(field.NomorID)))
                                            {
                                                listSpgd2.Add(new LG_SPGD2()
                                                {
                                                    c_gdg = gdgFrom,
                                                    c_iteno = field.Item,
                                                    c_spgno = spgID,
                                                    c_spno = field1.NomorID,
                                                    n_sisa = field1.QtySisa
                                                });

                                                spAlloc += field1.QtySisa;
                                            }
                                        }

                                        #region Old Coded

                                        //spDiv = (spQtyReloc - spAlloc);

                                        //if ((spDiv > 0) && (field.MOQ > 0))
                                        //{
                                        //  listAdjSp.Add(new ScmsSoaLibrary.Parser.Class.SuratPesananStructureField()
                                        //  {
                                        //    Acceptance = spDiv,
                                        //    IsNew = true,
                                        //    Item = field.Item,
                                        //    Keterangan = spgh.c_orno,
                                        //    Quantity = spDiv
                                        //  });

                                        //  listSpgd2Adj.Add(new LG_ORD2()
                                        //  {
                                        //    c_gdg = spgh.c_gdg,
                                        //    c_iteno = field.Item,
                                        //    c_orno = spgID,
                                        //    c_spno = "X",
                                        //    c_type = "01",
                                        //    n_sisa = spDiv
                                        //  });
                                        //}

                                        #endregion

                                        listItmFltr.Clear();
                                    }

                                    #endregion

                                    #region Ord4

                                    //if ((listItmSiT != null) && (listItmSiT.Count > 0))
                                    //{
                                    //  spQty = listItmSiT.GroupBy(x => x.Item).Sum(x => x.Sum(y => y.Quantity));

                                    //  spgd1.n_sit = (spQty.HasValue ? spQty.Value : 0);

                                    //  for (nLoopC = 0; nLoopC < listItmSiT.Count; nLoopC++)
                                    //  {
                                    //    field1 = listItmSiT[nLoopC];

                                    //    if ((field1 != null) && (!string.IsNullOrEmpty(field1.Item)))
                                    //    {
                                    //      listSpgd4.Add(new LG_ORD4()
                                    //      {
                                    //        c_gdg = spgh.c_gdg,
                                    //        c_iteno = field.Item,
                                    //        c_orno = spgID,
                                    //        c_sjno = field1.NomorID,
                                    //        n_sisa = field1.Quantity
                                    //      });
                                    //    }
                                    //  }
                                    //}
                                    //else
                                    //{
                                    //  spgd1.n_sit = 0;
                                    //}

                                    #endregion

                                    listSpgd1.Add(spgd1);
                                }
                            }

                            totalDetails++;
                        }

                        listFieldsGrp.Clear();

                        listFields.Clear();

                        if ((listSpgd1 != null) && (listSpgd1.Count > 0))
                        {
                            db.LG_SPGD1s.InsertAllOnSubmit(listSpgd1.ToArray());
                            listSpgd1.Clear();
                        }

                        if ((listSpgd2 != null) && (listSpgd2.Count > 0))
                        {
                            db.LG_SPGD2s.InsertAllOnSubmit(listSpgd2.ToArray());
                            listSpgd2.Clear();
                        }
                    }

                    #endregion

                    dic = new Dictionary<string, string>();

                    if (totalDetails > 0)
                    {
                        dic.Add("SPG", spgID);
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
                rpe = ResponseParser.ResponseParserEnum.IsError;

                result = string.Format("ScmsSoaLibrary.Bussiness.Pembelian:OrderRequestProcessGudang - {0}", ex.Message);

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

        public string PurchaseOrder(ScmsSoaLibrary.Parser.Class.PurchaseOrderStructure structure)
        {
            return PurchaseOrder(structure, null);
        }

        public string PurchaseOrder(ScmsSoaLibrary.Parser.Class.PurchaseOrderStructure structure, ORMDataContext dbContext)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false;

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

            LG_POH poh = null;

            ScmsSoaLibrary.Parser.Class.PurchaseOrderStructureField field = null;
            string nipEntry = null;
            string poID = null,
              noSupl = null;
            string tmpNumbering = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now,
              datePO = DateTime.Now;

            decimal? dTmp = 0;

            decimal nPrice = 0,
              itemPrice = 0,
              itemPriceSum = 0,
              itemPriceSumDisc = 0,
              qtyAcc = 0,
              discOn = 0,
              old_xDisc = 0;

            List<LG_POD1> listPod1 = null;
            List<LG_POD2> listPod2 = null;
            List<LG_POD3> listPod3 = null;

            List<ORHelper> listORHlp = null;
            ORHelper orhlp = null;

            LG_POD1 pod1 = null;
            LG_POD2 pod2 = null;
            //LG_POD3 pod3 = null;

            //List<LG_ORD1> listOrd1 = null;

            //LG_ORD1 ord1 = null;

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
            char gdg = (string.IsNullOrEmpty(structure.Fields.Gudang) ? char.MinValue : structure.Fields.Gudang[0]);

            if (gdg == char.MinValue)
            {
                result = "Gudang tidak boleh kosong.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            poID = (structure.Fields.PurchaseOrderID ?? string.Empty);

            try
            {
                if (!isContexted)
                {
                    db.Connection.Open();

                    db.Transaction = db.Connection.BeginTransaction();
                }

                if (structure.Method.Equals("Submit", StringComparison.OrdinalIgnoreCase))
                {
                    #region Add

                    if (!string.IsNullOrEmpty(poID))
                    {
                        result = "Nomor Purchase ID harus kosong.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.HasPO(db, structure.Fields.OrderRequestID))
                    {
                        result = "Order Request telah dibuat Purchasing Order.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    var qryOrSup = (from q in db.LG_ORHs
                                    join q1 in db.LG_DatSups on q.c_nosup equals q1.c_nosup into q_1
                                    from qDS in q_1.DefaultIfEmpty()
                                    where (q.c_orno == structure.Fields.OrderRequestID)
                                      && (q.c_gdg == gdg)
                                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false) //(q.l_delete == null || (q.l_delete.Value == false))
                                    select new
                                    {
                                        q,
                                        qDS
                                    }).Take(1).SingleOrDefault();

                    if (qryOrSup == null)
                    {
                        result = "Nomor Order tidak dapat dibaca.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, date))
                    {
                        result = "Purchase order tidak dapat disimpan, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    //gdg = qryOrSup.q.c_gdg;

                    poID = Commons.GenerateNumbering<LG_POH>(db, "PO", '3', "02", date, "c_pono");

                    //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "PO");

                    noSupl = (string.IsNullOrEmpty(qryOrSup.q.c_nosup) ? string.Empty : qryOrSup.q.c_nosup.Trim());

                    poh = new LG_POH()
                    {
                        c_entry = nipEntry,
                        c_gdg = gdg,
                        c_kurs = "01",
                        c_nosup = noSupl,
                        c_pono = poID,
                        c_update = nipEntry,
                        d_entry = date,
                        d_podate = date,
                        d_update = date,
                        l_import = (qryOrSup.qDS != null ? (qryOrSup.qDS.l_import.HasValue ? qryOrSup.qDS.l_import.Value : false) : false),
                        l_print = false,
                        l_revisi = false,
                        l_send = false,
                        n_bilva = 0,
                        n_bruto = 0,
                        n_disc = 0,
                        n_kurs = 1,
                        n_pdisc = 0,
                        n_ppn = 0,
                        n_xdisc = 0,
                        v_ket = structure.Fields.Keterangan
                    };

                    db.LG_POHs.InsertOnSubmit(poh);

                    #region Old Coded

                    //db.SubmitChanges();

                    //poh = (from q in db.LG_POHs
                    //       where q.v_ket == tmpNumbering
                    //       select q).Take(1).SingleOrDefault();

                    //if (poh == null)
                    //{
                    //  result = "Nomor Order Request tidak dapat di raih.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}

                    //if (poh.c_pono.Equals("XXXXXXXXXX"))
                    //{
                    //  result = "Trigger Order Request tidak aktif.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}

                    //poh.v_ket = structure.Fields.Keterangan;

                    //poID = poh.c_pono;

                    #endregion

                    #region Insert Detail

                    #region Old Coded

                    //var qryItemsOr = (from q in db.LG_ORD1s
                    //                  join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                    //                  join q2 in db.FA_DiscDs on q.c_iteno equals q2.c_iteno into q_2
                    //                  from qDD in q_2.DefaultIfEmpty()
                    //                  join q3 in db.FA_DiscHes on qDD.c_nodisc equals q3.c_nodisc into q_3
                    //                  from qDH in q_3.DefaultIfEmpty()
                    //                  where (q.c_orno == structure.Fields.OrderRequestID)
                    //                    && (q.c_gdg == gdg) && (qDH.c_type == "03")
                    //                    && (q1.c_nosup == noSupl)
                    //                    && (qDD.l_aktif == true) && (qDD.l_status == true)
                    //                  //&& (qDD.d_start >= date) || (qDD.d_finish <= date)
                    //                  select new { q, q1, qDD }).ToList();

                    #endregion

                    listORHlp = (from q in db.LG_ORD1s
                                 join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                 where (q.c_orno == structure.Fields.OrderRequestID)
                                   && (q.c_gdg == gdg) && (q1.c_nosup == noSupl)
                                   && (q.n_qty > 0)
                                 //select new { q, q1 }).ToList();
                                 select new ORHelper()
                                 {
                                     c_iteno = q.c_iteno,
                                     n_beli = (q.n_qty.HasValue ? q.n_qty.Value : 0),
                                     n_pminord = (q1.n_disc.HasValue ? q1.n_disc.Value : 0),
                                     n_salpri = (q1.n_salpri.HasValue ? q1.n_salpri.Value : 0),
                                 }).ToList();

                    if ((listORHlp == null) || (listORHlp.Count < 1))
                    {
                        result = "Items Order Request kosong atau tidak terbaca atau memiliki qty 0.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    listPod1 = new List<LG_POD1>();
                    listPod2 = new List<LG_POD2>();

                    totalDetails = 0;

                    itemPriceSum = 0;

                    //foreach (var qry in qryItemsOr)
                    for (nLoopC = 0; nLoopC < listORHlp.Count; nLoopC++)
                    {
                        orhlp = listORHlp[nLoopC];

                        qtyAcc = orhlp.n_beli; //(qry.q.n_qty.HasValue ? qry.q.n_qty.Value : 0);
                        discOn = orhlp.n_pminord; //(qry.q1.n_disc.HasValue ? qry.q1.n_disc.Value : 0);
                        nPrice = orhlp.n_salpri; //(qry.q1.n_salpri.HasValue ? qry.q1.n_salpri.Value : 0);

                        if (orhlp.n_beli > 0)
                        {
                            listPod1.Add(new LG_POD1()
                            {
                                c_gdg = gdg,
                                c_iteno = orhlp.c_iteno,
                                c_pono = poID,
                                n_disc = discOn,
                                n_qty = qtyAcc,
                                n_salpri = nPrice,
                                n_sisa = qtyAcc
                            });

                            itemPrice = (nPrice * qtyAcc);
                            itemPriceSumDisc += (itemPrice * (discOn / 100m));
                            itemPriceSum += itemPrice;

                            totalDetails++;
                        }
                    }

                    listORHlp.Clear();

                    if (listPod1.Count > 0)
                    {
                        db.LG_POD1s.InsertAllOnSubmit(listPod1.ToArray());

                        listPod2.Add(new LG_POD2()
                        {
                            c_gdg = gdg,
                            c_orno = structure.Fields.OrderRequestID,
                            c_pono = poID
                        });

                        db.LG_POD2s.InsertAllOnSubmit(listPod2.ToArray());
                    }

                    listPod2.Clear();
                    listPod1.Clear();

                    poh.n_bruto = itemPriceSum;
                    poh.n_disc = itemPriceSumDisc;

                    itemPrice = ((itemPriceSum - itemPriceSumDisc) * 0.1m);
                    poh.n_ppn = itemPrice;

                    poh.n_bilva = ((itemPriceSum - itemPriceSumDisc) + itemPrice);

                    if (!Commons.PO_BudgetLimit(db, noSupl, poID, date, itemPriceSum, nipEntry, false))
                    {
                        result = "Anggaran untuk pemasok ini tidak mencukupi.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    #endregion

                    dic = new Dictionary<string, string>();

                    if (totalDetails > 0)
                    {
                        dic.Add("PO", poID);
                        dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                        result = string.Format("Total {0} detail(s)", totalDetails);

                        hasAnyChanges = true;
                    }

                    #endregion
                }
                else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    if (string.IsNullOrEmpty(poID))
                    {
                        result = "Nomor Purchase Order dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    poh = (from q in db.LG_POHs
                           where q.c_pono == poID && q.c_gdg == gdg
                           select q).Take(1).SingleOrDefault();

                    if (poh == null)
                    {
                        result = "Nomor Purchase Order tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (poh.l_delete.HasValue && poh.l_delete.Value)
                    {
                        result = "Tidak dapat mengubah Purchase Order yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.HasPOSend(db, gdg, poID))
                    {
                        result = "Tidak dapat mengubah Purchasing Order yang sudah terkirim ke principal.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, poh.d_podate))
                    {
                        result = "Purchase order tidak dapat diubah, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    noSupl = (string.IsNullOrEmpty(poh.c_nosup) ? string.Empty : poh.c_nosup);

                    old_xDisc = (poh.n_xdisc.HasValue ? poh.n_xdisc.Value : 0);

                    poh.l_revisi = true;

                    if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
                    {
                        poh.v_ket = structure.Fields.Keterangan;
                    }

                    if ((!string.IsNullOrEmpty(structure.Fields.Kurs)) && (structure.Fields.KursValue > 0))
                    {
                        poh.c_kurs = structure.Fields.Kurs;
                        poh.n_kurs = structure.Fields.KursValue;
                    }

                    poh.n_xdisc = structure.Fields.ExtraDiscount;

                    poh.c_update = nipEntry;
                    poh.d_update = DateTime.Now;

                    datePO = (poh.d_podate.HasValue ? poh.d_podate.Value : DateTime.MinValue);

                    #region Populate Detail

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        listPod3 = new List<LG_POD3>();

                        listPod1 = (from q in db.LG_POD1s
                                    where q.c_pono == poID && q.c_gdg == gdg
                                    select q).ToList();

                        nLoopC = 0;

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if ((field != null) && (!field.IsNew) && (!field.IsDelete) && field.IsModified)
                            {
                                #region Modify

                                pod1 = (from q in listPod1
                                        where q.c_iteno == field.Item
                                        select q).Take(1).SingleOrDefault();

                                if (pod1 != null)
                                {
                                    if (((pod1.n_disc.HasValue ? pod1.n_disc.Value : 0) == field.Discount) ||
                                      ((pod1.n_salpri.HasValue ? pod1.n_salpri.Value : 0) == field.Price))
                                    {
                                        nLoopC++;

                                        pod1.n_disc = field.Discount;
                                        pod1.n_salpri = field.Price;

                                        listPod3.Add(new LG_POD3()
                                        {
                                            c_entry = nipEntry,
                                            c_gdg = gdg,
                                            c_iteno = field.Item,
                                            c_pono = poID,
                                            d_entry = DateTime.Now,
                                            n_disc = pod1.n_disc,
                                            n_qty = pod1.n_qty,
                                            n_salpri = pod1.n_salpri,
                                            n_sisa = pod1.n_sisa,
                                            n_xdisc = old_xDisc,
                                            v_ket_del = field.Keterangan,
                                            v_type = "02"
                                        });
                                    }
                                }

                                #endregion
                            }
                            else if ((field != null) && (!field.IsNew) && field.IsDelete && (!field.IsModified))
                            {
                                #region Delete

                                pod1 = (from q in listPod1
                                        where q.c_iteno == field.Item
                                        select q).Take(1).SingleOrDefault();

                                if (pod1 != null)
                                {
                                    nLoopC++;

                                    listPod3.Add(new LG_POD3()
                                    {
                                        c_entry = nipEntry,
                                        c_gdg = gdg,
                                        c_iteno = field.Item,
                                        c_pono = poID,
                                        d_entry = DateTime.Now,
                                        n_disc = pod1.n_disc,
                                        n_qty = pod1.n_qty,
                                        n_salpri = pod1.n_salpri,
                                        n_sisa = pod1.n_sisa,
                                        n_xdisc = old_xDisc,
                                        v_ket_del = (string.IsNullOrEmpty(field.Keterangan) ? "Human error" : field.Keterangan),
                                        v_type = "03"
                                    });

                                    listPod1.Remove(pod1);

                                    db.LG_POD1s.DeleteOnSubmit(pod1);
                                }

                                #endregion
                            }
                        }

                        #region Recalculate All

                        if (nLoopC > 0)
                        {
                            dTmp = listPod1.GroupBy(x => x.c_iteno).Sum(x => x.Sum(y => (y.n_salpri * y.n_qty)));

                            itemPriceSum = (dTmp.HasValue ? dTmp.Value : 0);

                            dTmp = listPod1.GroupBy(x => x.c_iteno).Sum(x => x.Sum(y => ((y.n_salpri * y.n_qty) * (y.n_disc / 100))));

                            itemPriceSumDisc = (dTmp.HasValue ? dTmp.Value : 0);

                            discOn = ((itemPriceSum - itemPriceSumDisc) * (structure.Fields.ExtraDiscount / 100));

                            nPrice = (itemPriceSum - itemPriceSumDisc - discOn);

                            poh.n_bruto = itemPriceSum;
                            poh.n_disc = itemPriceSumDisc;
                            poh.n_pdisc = structure.Fields.ExtraDiscount;
                            poh.n_xdisc = discOn;
                            poh.n_ppn = (nPrice * 0.1m);
                            poh.n_bilva = (nPrice + poh.n_ppn);

                            if (!Commons.PO_BudgetLimit(db, noSupl, poID, datePO, itemPriceSum, nipEntry, false))
                            {
                                result = "Anggaran untuk pemasok ini tidak mencukupi.";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }

                                goto endLogic;
                            }
                        }

                        #endregion
                    }

                    #endregion

                    hasAnyChanges = true;

                    #endregion
                }
                else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    if (string.IsNullOrEmpty(poID))
                    {
                        result = "Nomor Purchase Order dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        db.Transaction.Rollback();

                        goto endLogic;
                    }

                    poh = (from q in db.LG_POHs
                           where q.c_pono == poID
                           select q).Take(1).SingleOrDefault();

                    if (poh == null)
                    {
                        result = "Nomor Purchase Order tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        db.Transaction.Rollback();

                        goto endLogic;
                    }
                    else if (poh.l_delete.HasValue && poh.l_delete.Value)
                    {
                        result = "Tidak dapat menghapus nomor Purchase Order yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        db.Transaction.Rollback();

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, poh.d_podate))
                    {
                        result = "Purchase order tidak dapat dihapus, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (poh.l_sent.HasValue && poh.l_sent.Value)
                    {
                        result = "Tidak dapat menghapus nomor Purchase Order yang sudah terkirim.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    noSupl = (string.IsNullOrEmpty(poh.c_nosup) ? string.Empty : poh.c_nosup);

                    //itemPriceSum = (poh.n_bruto.HasValue ? poh.n_bruto.Value : 0);

                    poh.c_update = nipEntry;
                    poh.d_update = DateTime.Now;

                    poh.l_delete = true;
                    poh.v_ket_mark = structure.Fields.Keterangan;

                    datePO = (poh.d_podate.HasValue ? poh.d_podate.Value : DateTime.MinValue);

                    listPod3 = new List<LG_POD3>();

                    listPod1 = (from q in db.LG_POD1s
                                where q.c_gdg == gdg && q.c_pono == poID
                                select q).ToList();

                    listPod2 = (from q in db.LG_POD2s
                                where q.c_gdg == gdg && q.c_pono == poID
                                select q).Take(1).ToList();

                    itemPriceSum = 0;

                    if ((listPod2 != null) && (listPod2.Count > 0))
                    {
                        pod2 = listPod2[0];

                        tmpNumbering = pod2.c_orno;

                        db.LG_POD2s.DeleteOnSubmit(pod2);
                    }
                    else
                    {
                        tmpNumbering = "<unreaded>";
                    }

                    if ((listPod1 != null) && (listPod1.Count > 0))
                    {
                        itemPriceSum = listPod1.Sum(t => (t.n_qty.HasValue ? t.n_qty.Value : 0) * (t.n_salpri.HasValue ? t.n_salpri.Value : 0));

                        for (nLoop = 0; nLoop < listPod1.Count; nLoop++)
                        {
                            pod1 = listPod1[nLoop];
                            if (pod1 != null)
                            {
                                listPod3.Add(new LG_POD3()
                                {
                                    c_entry = nipEntry,
                                    c_gdg = gdg,
                                    c_iteno = pod1.c_iteno,
                                    c_no = tmpNumbering,
                                    c_pono = poID,
                                    d_entry = date,
                                    n_disc = pod1.n_disc,
                                    n_qty = pod1.n_qty,
                                    n_salpri = pod1.n_salpri,
                                    n_sisa = pod1.n_sisa,
                                    n_xdisc = poh.n_xdisc,
                                    v_ket_del = (string.IsNullOrEmpty(structure.Fields.Keterangan) ? "Human error" : structure.Fields.Keterangan),
                                    v_type = "03"
                                });
                            }
                        }

                        db.LG_POD1s.DeleteAllOnSubmit(listPod1.ToArray());

                        listPod1.Clear();
                    }

                    if (listPod3.Count > 0)
                    {
                        db.LG_POD3s.InsertAllOnSubmit(listPod3.ToArray());

                        listPod3.Clear();
                    }

                    Commons.PO_BudgetLimit(db, noSupl, poID, datePO, itemPriceSum, nipEntry, true, structure.Fields.Keterangan);

                    hasAnyChanges = true;

                    #endregion
                }
                else if (structure.Method.Equals("SendProcess", StringComparison.OrdinalIgnoreCase))
                {
                    #region SendProcess

                    if (string.IsNullOrEmpty(poID))
                    {
                        result = "Nomor Purchase Order dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    poh = (from q in db.LG_POHs
                           where q.c_pono == poID && q.c_gdg == gdg
                           select q).Take(1).SingleOrDefault();

                    if (poh == null)
                    {
                        result = "Nomor Purchase Order tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (poh.l_delete.HasValue && poh.l_delete.Value)
                    {
                        result = "Tidak dapat mengubah Purchase Order yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (poh.l_print.HasValue && (!poh.l_print.Value))
                    {
                        result = "Tidak dapat mengirim Purchase Order yang belum tercetak.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    //else if (Commons.HasPOSend(db, gdg, poID))
                    //{
                    //  result = "Purchasing Order sudah terkirim ke principal.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}
                    else if (Commons.IsClosingLogistik(db, poh.d_podate))
                    {
                        result = "Purchase order tidak dapat diubah, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    if ((!(poh.l_send.HasValue ? poh.l_send.Value : false)) && structure.Fields.HasSend)
                    {
                        poh.l_send = structure.Fields.HasSend;

                        poh.c_update = nipEntry;
                        poh.d_update = DateTime.Now;

                        hasAnyChanges = true;
                    }
                    else
                    {
                        result = "Status kirim PO terkonfirmasi.";
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

                result = string.Format("ScmsSoaLibrary.Bussiness.Pembelian:PurchaseOrder - {0}", ex.Message);

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

        public string ReceiveNote(ScmsSoaLibrary.Parser.Class.ReceiveNoteStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false;

            string result = null;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            ScmsSoaLibrary.Parser.Class.ReceiveNoteStructureField field = null;
            string nipEntry = null;
            string rnID = null;
            string rnIDUlujami = null;
            string sjID = null;
            string pinNumber = null;
            string poID = null;
            //string tmpNumbering = null;
            string typeTrx = null,
              typeDtl = null,
              noSup = null,
              noDo = null,
              noItem = null,
              noBatch = null;

            //Indra 20181001FM
            //STT Auto ketika RN Khusus
            string stID = null;
            List<SCMS_WPD> listWpd = null;

            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now;

            //decimal? dTmp = 0;

            //decimal nPrice = 0,
            //  itemPrice = 0,
            //  itemPriceSum = 0,
            //  itemPriceSumDisc = 0,
            //  qtyAcc = 0,
            //  discOn = 0,
            //  old_xDisc = 0;

            string[] typeList = null;
            List<string> lstTemp = null;

            bool isFloating = false;

            List<LG_RND1> listRnd1 = null;
            List<LG_RND2> listRnd2 = null;
            List<LG_RND2> listRnd2Copy = null;
            List<LG_RND3> listRnd3 = null;
            List<LG_RND3> listRnd3Copy = null;
            //List<LG_RND4> listRnd4 = null;
            List<LG_RND5> listRnd5 = null;

            List<LG_RSD2> listRSD2 = null;
            List<LG_RSD2> listRSD2Copy = null;

            LG_RNH rnh = null;
            LG_RNH rnh_6 = null;
            LG_RND1 rnd1 = null;
            LG_RND2 rnd2 = null;
            LG_RND3 rnd3 = null;
            LG_RSD2 rsd2 = null;
            LG_ClaimAccD claimaccd = null;
            SCMS_WPH wph = null;
            SCMS_WPD wpd = null;
       //hafizh
            LG_POH PO = null;
            string spgTempId = null;

            List<LG_SJD1> listSjd1 = null;
            List<LG_SJD2> listSjd2 = null;
            List<LG_SJD2> listSjd2Copy = null;
            List<LG_SJD3> listSjd3 = null;
            LG_SPGH spgh = null;
            List<LG_SPGD1> listSPGD1Auto = null;
            List<string> listRN = null;
            LG_ComboH comboh = null;
            Dictionary<string, List<SJClassComponent>> dicItemStock = null;

             LG_SPGD1 spgd1 = null;

             List<LG_RND1> listRnd1Ulujami = null;
             List<LG_RND2> listRnd2Ulujami = null;
             List<LG_RND3> listRnd3Ulujami = null;

            LG_SJH sjh = null;

            #region New Stock Indra 20180305FM

            LG_DAILY_STOCK_v2 daily2 = null; 
            LG_MOVEMENT_STOCK_v2 movement2 = null;

            #endregion

            int nLoop = 0,
              nLoopC = 0;

            decimal nQtyGood = 0,
              nQtyBad = 0,
              nQtyAllocGood = 0,
              nQtyAllocBad = 0,
              nQtyAlloc = 0;

            bool modifedPo = false;
            IDictionary<string, string> dic = null;
            IDictionary<string, string> dic2 = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            int totalDetails = 0;

            char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) ? char.MinValue : structure.Fields.Gudang[0]);

            if (gudang == char.MinValue)
            {
                result = "Gudang tidak boleh kosong.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            else if (gudang == '6')
            {
                gudang = '1';
            }
            rnID = (structure.Fields.ReceiveNoteID ?? string.Empty);

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
                {
                    #region Add

                    #region Validasi New Stock Indra 20180305FM

                    if (!string.IsNullOrEmpty(rnID))
                    {
                        result = "Nomor Receive harus kosong.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (string.IsNullOrEmpty(structure.Fields.TypeRN))
                    {
                        result = "Tipe Receive tidak boleh kosong.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, date))
                    {
                        result = "Receive note tidak dapat disimpan, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    #endregion

                    rnID = Commons.GenerateNumbering<LG_RNH>(db, "RN", '3', "03", date, "c_rnno");
                    
                    //rnIDUlujami = Commons.GenerateNumbering<LG_RNH>(db, "RN", '3', "03", date, "c_rnno");

                    //Indra 20181001FM
                    //STT Auto ketika RN Khusus
                    stID = Commons.GenerateNumbering<SCMS_WPH>(db, "ST", '7', "07", date, "c_nodoc");
                    PO = (from q in db.LG_POHs where q.c_pono == structure.Fields.Field[0].ReferenceID
                              select q).Distinct().SingleOrDefault();
                    #region Ulujami

                    //if (structure.Fields.TypeRN.Equals("Ulujami", StringComparison.OrdinalIgnoreCase) )
                      if (structure.Fields.TypeRN.Equals("Ulujami", StringComparison.OrdinalIgnoreCase) ||
                      structure.Fields.TypeRN.Equals("06Ulujami", StringComparison.OrdinalIgnoreCase))


                    {
                        if (string.IsNullOrEmpty(structure.Fields.DOPrincipal))
                        {
                            result = "Nomor DO Suplier harus terisi.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                        else if (structure.Fields.TanggalDOFormat.Equals(DateTime.MinValue))
                        {
                            result = "Format tanggal DO tidak dapat terbaca.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                        else if (string.IsNullOrEmpty(structure.Fields.Suplier))
                        {
                            result = "Suplier tidak boleh kosong.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            db.Transaction.Rollback();

                            goto endLogic;
                        }


                       // RN gudang 1

                        //Indra 20190312FM Pencegahan RN Double
                        LG_RNH CekRNDO = null;

                        CekRNDO = (from q in db.LG_RNHs
                                   where q.c_gdg == gudang &&
                                         q.c_dono == structure.Fields.DOPrincipal &&
                                         q.d_entry >= date.AddMinutes(-30)
                                   select q).Take(1).SingleOrDefault();

                        if (CekRNDO != null)
                        {
                            result = "DO nomor " + structure.Fields.DOPrincipal + " sudah tersimpan pada jam " + CekRNDO.d_entry + " harap refresh atau menunggu 1/2 jam untuk melakukan penyimpanan.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            db.Transaction.Rollback();

                            goto endLogic;
                        }

                        rnh = new LG_RNH()
                        {
                            c_dono = structure.Fields.DOPrincipal,
                            c_entry = nipEntry,
                            c_from = structure.Fields.Suplier,
                            c_gdg = gudang,
                            c_rnno = rnID,
                            c_type = "01",
                            c_update = nipEntry,
                            d_dodate = structure.Fields.TanggalDOFormat,
                            d_entry = date,
                            d_rndate = date,
                            d_update = date,
                            n_bea = structure.Fields.Bea,
                            l_float = structure.Fields.Floating,
                            l_print = true,
                            l_status = false,
                            //l_rnkhusus = true,
                            l_rnkhusus = (structure.Fields.TypeRN.Equals("06Ulujami", StringComparison.OrdinalIgnoreCase)),
                            //l_khusus = (structure.Fields.TypeRN.Equals("06Ulujami", StringComparison.OrdinalIgnoreCase) && structure.Fields.OrderKhusus ? true : false),
                            v_ket = "Ulujami" +","+ structure.Fields.Keterangan,
                        };

                        db.LG_RNHs.InsertOnSubmit(rnh);



                        sjID = Commons.GenerateNumbering<LG_SJH>(db, "SJ", '3', "06", date, "c_sjno");


                        pinNumber = Functionals.GeneratedRandomPinId(8, string.Empty);



                        sjh = new LG_SJH()
                        {
                            c_sjno = sjID,
                            c_entry = nipEntry,
                            c_gdg = '1',
                            c_nosup = structure.Fields.Suplier,
                            c_type = "01",
                            c_update = nipEntry,
                            d_entry = date,
                            d_update = date,
                            l_confirm = true,
                            l_print = false,
                            v_ket = "Ulujami" + "," + structure.Fields.Keterangan ,
                            c_gdg2 = '6',
                            c_pin = pinNumber,
                            d_sjdate = date.Date,
                            l_exp = false,
                            l_status = true,
                            c_confirm = nipEntry,
                            d_confirm = date,
                            //c_type_cat = structure.Fields.TypeKategori,
                            //c_type_lat = structure.Fields.TypeLantai,
                            //c_type_sj = structure.Fields.TypeSJ,


                            // harus di perbaiki 
                            //c_type_cat = "06",  // Reguler
                            //c_type_lat = "Lantai 1",
                            c_type_sj = "02", // Master Box

                            l_auto = false
                        };


                        db.LG_SJHs.InsertOnSubmit(sjh);



                       // Proses Gudang ulujami

                        rnh = new LG_RNH()
                        {
                            //c_dono = structure.Fields.DOPrincipal,
                            c_dono = sjID,
                            c_entry = nipEntry,
                            c_from = structure.Fields.Suplier,
                            c_gdg = '6',
                            c_rnno = rnID,
                            c_type = "05",
                            c_update = nipEntry,
                            d_dodate = structure.Fields.TanggalDOFormat,
                            d_entry = date,
                            d_rndate = date,
                            d_update = date,
                            n_bea = structure.Fields.Bea,
                            l_float = structure.Fields.Floating,
                            l_print = true,
                            l_status = true,
                            l_rnkhusus = (structure.Fields.TypeRN.Equals("06Ulujami", StringComparison.OrdinalIgnoreCase)),
                            v_ket = rnID +"," +"Ulujami" + "," + structure.Fields.Keterangan,
                        };

                        db.LG_RNHs.InsertOnSubmit(rnh);

                      

                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            listRnd1 = new List<LG_RND1>();
                            listRnd2 = new List<LG_RND2>();
                            listRnd3 = new List<LG_RND3>();

                            listRnd1Ulujami = new List<LG_RND1>();
                            listRnd2Ulujami = new List<LG_RND2>();


                            //

                            listSjd1 = new List<LG_SJD1>();
                            listSjd2 = new List<LG_SJD2>();
                            listRN = new List<string>();
                            comboh = new LG_ComboH();
                            rnd1 = new LG_RND1();
                            dicItemStock = new Dictionary<string, List<SJClassComponent>>(StringComparer.OrdinalIgnoreCase);
                            spgh = new LG_SPGH();
                            listSPGD1Auto = new List<LG_SPGD1>();

                            //

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                field = structure.Fields.Field[nLoop];


                                string CekBatch = (field.Batch ?? string.Empty.Trim());

                                if ((CekBatch.Contains("'")) || (CekBatch.Contains("*")) || (CekBatch.Contains("%")))
                                {
                                    result = "Tidak Boleh Karakter ' ( Kutip) atau  * (Bintang) atau %(Persentase) ";

                                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                    goto endLogic;

                                }


                                if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0) && (!string.IsNullOrEmpty(field.Batch)) && (field.Batch.Length > 0))
                                {

                                    

                                    if (field.TypeTrx.Equals("02", StringComparison.OrdinalIgnoreCase))
                                    {
                                        modifedPo = true;
                                    }
                                    else
                                    {
                                        modifedPo = Commons.PO_Modifikasi(db, gudang, field.ReferenceID, field.Item, false, field.Quantity);
                                    }

                                    if (modifedPo)
                                    {
                                        if (structure.Fields.TypeRN.Equals("06Ulujami", StringComparison.OrdinalIgnoreCase))
                                        {
                                            modifedPo = Commons.DOPharos_Modifikasi(db, structure.Fields.Suplier, structure.Fields.DOPrincipal, field.Item, field.Batch, false, field.Quantity);
                                        }
                                        else
                                        {
                                            modifedPo = true;
                                        }

                                        if (modifedPo)
                                        {
                                            #region Detail Data

                                            listRnd2.Add(new LG_RND2()
                                            {
                                                // PROJECT
                                                c_batch = field.Batch.ToUpper(),
                                                c_gdg = gudang,
                                                c_iteno = field.Item,
                                                c_no = field.ReferenceID,
                                                c_rnno = rnID,
                                                c_type = field.TypeTrx,
                                                n_bqty = 0,
                                                n_gqty = field.Quantity,
                                                n_floqty = structure.Fields.Floating ? field.Quantity : 0
                                            });



                                            spgTempId = string.Concat("SGXXXX05UJ");

                                            listSjd2.Add(new LG_SJD2()
                                            {
                                                c_batch = field.Batch,
                                                c_gdg = '1',
                                                c_iteno = field.Item,
                                                c_sjno = sjID,
                                                c_spgno = spgTempId,
                                                c_rnno = rnID,
                                                n_bqty = 0,
                                                n_gqty = field.Quantity,
                                                n_bsisa = 0,
                                                n_gsisa = 0
                                            });


                                            listRnd2Ulujami.Add(new LG_RND2()
                                            {
                                                // PROJECT
                                                c_batch = field.Batch.ToUpper(),
                                                c_gdg = '6',
                                                c_iteno = field.Item,
                                                c_no = sjID,
                                                c_rnno = rnID,
                                                c_type = field.TypeTrx,
                                                n_bqty = 0,
                                                n_gqty = field.Quantity,
                                                n_floqty = structure.Fields.Floating ? field.Quantity : 0
                                            });


                                            wpd = (from q in db.SCMS_WPDs
                                                   where q.c_no == field.ReferenceID
                                                      && (q.l_rn == false || q.l_rn == null)
                                                   select q).Take(1).SingleOrDefault();

                                            if (wpd != null)
                                            {
                                                wph = (from q in db.SCMS_WPHs
                                                       where q.c_nodoc == wpd.c_nodoc
                                                       select q).Take(1).SingleOrDefault();

                                                wph.c_rn = nipEntry;
                                                wph.d_rn = date;

                                                wpd.l_rn = true;
                                            }

                                            bool isOrKhusus = Commons.IsORKhusus(db, field.ReferenceID);

                                            if (isOrKhusus)
                                            {
                                                rnh.l_khusus = isOrKhusus;
                                            }

                                            rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
                                            {
                                                return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                  field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                            });

                                            if (rnd1 == null)
                                            {
                                                listRnd1.Add(new LG_RND1()
                                                {

                                                    //Project
                                                    c_batch = field.Batch.ToUpper(),
                                                    c_gdg = gudang,
                                                    c_iteno = field.Item,
                                                    c_rnno = rnID,
                                                    n_bqty = 0,
                                                    n_bsisa = 0,
                                                    n_gqty = field.Quantity,
                                                    n_gsisa = 0,
                                                });


                                                spgTempId = string.Concat("SGXXXX05UJ");

                                                listSjd1.Add(new LG_SJD1()
                                                {
                                                    c_batch = field.Batch,
                                                    c_gdg = '1',                                                  
                                                    c_iteno = field.Item,
                                                    c_sjno = sjID,
                                                    c_spgno = spgTempId,
                                                    //n_bqty = field.QuantityBad,
                                                    //n_booked_bad = field.QuantityBad,
                                                    n_bqty = 0,
                                                    n_booked_bad = 0,
                                                    n_booked = field.Quantity,
                                                    n_gqty = field.Quantity,
                                                    l_expired = field.ExpiredDateFormated < DateTime.Now ? true : false,
                                                    
                                                });



                                                listRnd1Ulujami.Add(new LG_RND1()
                                                {

                                                    //Project
                                                    c_batch = field.Batch.ToUpper(),
                                                    c_gdg = '6',
                                                    c_iteno = field.Item,
                                                    c_rnno = rnID,
                                                    n_bqty = 0,
                                                    n_bsisa = 0,
                                                    n_gqty = field.Quantity,
                                                    n_gsisa = field.Quantity,
                                                });


                                                // Check Batch
                                                Commons.CheckAndProcessBatchRN(db, field.Item, field.Batch, field.ExpiredDateFormated, nipEntry);
                                            }
                                            else
                                            {
                                                rnd1.n_gsisa = rnd1.n_gqty += field.Quantity;
                                            }


                                            if (structure.Fields.Floating)
                                            {
                                                rnd3 = listRnd3.Find(delegate(LG_RND3 rnd3s)
                                                {
                                                    return field.Item.Equals((string.IsNullOrEmpty(rnd3s.c_iteno) ? string.Empty : rnd3s.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.Batch.Equals((string.IsNullOrEmpty(rnd3s.c_batch) ? string.Empty : rnd3s.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                                });

                                                if (rnd3 == null)
                                                {
                                                    listRnd3.Add(new LG_RND3()
                                                    {
                                                        // project
                                                        c_batch = field.Batch.ToUpper(),
                                                        c_gdg = gudang,
                                                        c_iteno = field.Item,
                                                        c_rnno = rnID,
                                                        n_bqty = 0,
                                                        n_gqty = field.Quantity,
                                                        //n_floqty = (structure.Fields.Floating ? field.Quantity : 0)
                                                    });


                                                    //listRnd3Ulujami.Add(new LG_RND3()
                                                    //{
                                                    //    // project
                                                    //    c_batch = field.Batch.ToUpper(),
                                                    //    c_gdg = gudang,
                                                    //    c_iteno = field.Item,
                                                    //    c_rnno = rnIDUlujami,
                                                    //    n_bqty = 0,
                                                    //    n_gqty = field.Quantity,
                                                    //    //n_floqty = (structure.Fields.Floating ? field.Quantity : 0)
                                                    //});

                                                }
                                                else
                                                {
                                                    rnd3.n_gqty = rnd3.n_gqty += field.Quantity;
                                                }
                                            }

                                            #region New Stock Indra 20180305FM

                                            SavingStock.DailyStock(db, '1'.ToString(),
                                                                       rnID,
                                                                       "01",
                                                                       field.Item,
                                                                       field.Batch,
                                                                       field.Quantity,
                                                                       0,
                                                                       "KOSONG",
                                                                       "01",
                                                                       "01",
                                                                       nipEntry,
                                                                       "01");                                            

                                            if ((SavingStock.DailyStock(db, '1'.ToString(),
                                                                      sjID,
                                                                      "01",
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

                                            SavingStock.DailyStock(db, '6'.ToString(),
                                                                       rnID,
                                                                       "05",
                                                                       field.Item,
                                                                       field.Batch,
                                                                       field.Quantity,
                                                                       0,
                                                                       "KOSONG",
                                                                       "01",
                                                                       "01",
                                                                       nipEntry,
                                                                       "01");
                                            #endregion

                                            totalDetails++;

                                            #endregion
                                        }
                                    }
                                }
                            }

                            if ((listRnd1.Count > 0) && (listRnd2.Count > 0))
                            {
                                db.LG_RND1s.InsertAllOnSubmit(listRnd1.ToArray());
                                db.LG_RND2s.InsertAllOnSubmit(listRnd2.ToArray());
                            }

                            //db.SubmitChanges();

                            listRnd1.Clear();
                            listRnd2.Clear();

                            if ((listRnd1Ulujami.Count > 0) && (listRnd2Ulujami.Count > 0))
                            {
                                db.LG_RND1s.InsertAllOnSubmit(listRnd1Ulujami.ToArray());
                                db.LG_RND2s.InsertAllOnSubmit(listRnd2Ulujami.ToArray());
                            }

                            //db.SubmitChanges();

                            listRnd1.Clear();
                            listRnd2.Clear();


                            if ((listSjd1.Count > 0) && (listSjd2.Count > 0))
                            {
                                db.LG_SJD1s.InsertAllOnSubmit(listSjd1.ToArray());
                                db.LG_SJD2s.InsertAllOnSubmit(listSjd2.ToArray());
                            }


                            listSjd1.Clear();
                            listSjd2.Clear();
                           



                        }
                    }

                    #endregion

                    #region Ulujami rn claim
                     else if (structure.Fields.TypeRN.Equals("03Ulujami", StringComparison.OrdinalIgnoreCase))
                      {
                          if (string.IsNullOrEmpty(structure.Fields.DOPrincipal))
                          {
                              result = "Nomor DO Suplier harus terisi.";
                              rpe = ResponseParser.ResponseParserEnum.IsFailed;
                              if (db.Transaction != null)
                              {
                                  db.Transaction.Rollback();
                              }
                              goto endLogic;
                          }
                          else if (structure.Fields.TanggalDOFormat.Equals(DateTime.MinValue))
                          {
                              result = "Format tanggal DO tidak dapat terbaca.";
                              rpe = ResponseParser.ResponseParserEnum.IsFailed;
                              if (db.Transaction != null)
                              {
                                  db.Transaction.Rollback();
                              }
                              goto endLogic;
                          }
                          else if (string.IsNullOrEmpty(structure.Fields.Suplier))
                          {
                              result = "Suplier tidak boleh kosong.";
                              rpe = ResponseParser.ResponseParserEnum.IsFailed;
                              if (db.Transaction != null)
                              {
                                  db.Transaction.Rollback();
                              }
                              goto endLogic;
                          }
                          // RN gudang 1

                          //Indra 20190312FM Pencegahan RN Double
                          LG_RNH CekRNDO = null;

                          CekRNDO = (from q in db.LG_RNHs
                                     where q.c_gdg == gudang &&
                                           q.c_dono == structure.Fields.DOPrincipal &&
                                           q.d_entry >= date.AddMinutes(-30)
                                     select q).Take(1).SingleOrDefault();

                          if (CekRNDO != null)
                          {
                              result = "DO nomor " + structure.Fields.DOPrincipal + " sudah tersimpan pada jam " + CekRNDO.d_entry + " harap refresh atau menunggu 1/2 jam untuk melakukan penyimpanan.";

                              rpe = ResponseParser.ResponseParserEnum.IsFailed;

                              db.Transaction.Rollback();

                              goto endLogic;
                          }

                          rnh = new LG_RNH()
                          {
                              c_dono = structure.Fields.DOPrincipal,
                              c_entry = nipEntry,
                              c_from = structure.Fields.Suplier,
                              c_gdg = gudang,
                              c_rnno = rnID,
                              c_type = "03",
                              c_update = nipEntry,
                              d_dodate = structure.Fields.TanggalDOFormat,
                              d_entry = date,
                              d_rndate = date,
                              d_update = date,
                              n_bea = structure.Fields.Bea,
                              l_float = false,
                              l_print = false,
                              l_status = false,
                              v_ket = "RelokasiUJClaim03" + "," + structure.Fields.Keterangan,
                          };
                          db.LG_RNHs.InsertOnSubmit(rnh);
                          sjID = Commons.GenerateNumbering<LG_SJH>(db, "SJ", '3', "06", date, "c_sjno");
                          pinNumber = Functionals.GeneratedRandomPinId(8, string.Empty);
                            // Proses SJ
                          sjh = new LG_SJH()
                          {
                              c_sjno = sjID,
                              c_entry = nipEntry,
                              c_gdg = '1',
                              c_nosup = structure.Fields.Suplier,
                              c_type = "01",
                              c_update = nipEntry,
                              d_entry = date,
                              d_update = date,
                              l_confirm = true,
                              l_print = false,
                              v_ket = "Ulujami" + "," + structure.Fields.Keterangan,
                              c_gdg2 = '6',
                              c_pin = pinNumber,
                              d_sjdate = date.Date,
                              l_exp = false,
                              l_status = true,
                              c_confirm = nipEntry,
                              d_confirm = date,
                              c_type_sj = "02", // Master Box
                              l_auto = false
                          };
                          db.LG_SJHs.InsertOnSubmit(sjh);
                          rnh = new LG_RNH()
                          {
                              c_dono = sjID,
                              c_entry = nipEntry,
                              c_from = structure.Fields.Suplier,
                              c_gdg = '6',
                              c_rnno = rnID,
                              c_type = "05",
                              c_update = nipEntry,
                              d_dodate = structure.Fields.TanggalDOFormat,
                              d_entry = date,
                              d_rndate = date,
                              d_update = date,
                              n_bea = structure.Fields.Bea,
                              l_float = structure.Fields.Floating,
                              l_print = true,
                              l_status = true,
                              v_ket = rnID + "," + "RelokasiUJClaim03" + "," + structure.Fields.Keterangan,
                          };
                          db.LG_RNHs.InsertOnSubmit(rnh);
                          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                          {
                              listRnd1 = new List<LG_RND1>();
                              listRnd2 = new List<LG_RND2>();
                              listRnd3 = new List<LG_RND3>();
                              claimaccd = new LG_ClaimAccD();
                              listRnd1Ulujami = new List<LG_RND1>();
                              listRnd2Ulujami = new List<LG_RND2>();
                              listSjd1 = new List<LG_SJD1>();
                              listSjd2 = new List<LG_SJD2>();
                              listRN = new List<string>();
                              comboh = new LG_ComboH();
                              rnd1 = new LG_RND1();
                              dicItemStock = new Dictionary<string, List<SJClassComponent>>(StringComparer.OrdinalIgnoreCase);
                              spgh = new LG_SPGH();
                              listSPGD1Auto = new List<LG_SPGD1>();
                              for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                              {
                                  field = structure.Fields.Field[nLoop];


                                  string CekBatch = (field.Batch ?? string.Empty.Trim());

                                  if ((CekBatch.Contains("'")) || (CekBatch.Contains("*")) || (CekBatch.Contains("%")))
                                  {
                                      result = "Tidak Boleh Karakter ' ( Kutip) atau  * (Bintang) atau %(Persentase) ";

                                      rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                      goto endLogic;

                                  }


                                  if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0))
                                  {
                                      #region Detail Data
                                      listRnd2.Add(new LG_RND2()
                                      {
                                          c_batch = field.Batch.ToUpper(),
                                          c_gdg = gudang,
                                          c_iteno = field.Item,
                                          c_no = field.ReferenceID,
                                          c_rnno = rnID,
                                          c_type = field.TypeTrx,
                                          n_bqty = 0,
                                          n_gqty = field.Quantity
                                      });
                                      spgTempId = string.Concat("SGXXXX03UJ");
                                      listSjd2.Add(new LG_SJD2()
                                      {
                                          c_batch = field.Batch.ToUpper(),
                                          c_gdg = '1',
                                          c_iteno = field.Item,
                                          c_sjno = sjID,
                                          c_spgno = spgTempId,
                                          c_rnno = rnID,
                                          n_bqty = 0,
                                          n_gqty = field.Quantity,
                                          n_bsisa = 0,
                                          n_gsisa = 0
                                      });
                                      listRnd2Ulujami.Add(new LG_RND2()
                                      {
                                          c_batch = field.Batch.ToUpper(),
                                          c_gdg = '6',
                                          c_iteno = field.Item,
                                          c_no = sjID,
                                          c_rnno = rnID,
                                          c_type = field.TypeTrx,
                                          n_bqty = 0,
                                          n_gqty = field.Quantity,
                                      });
                                      rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
                                      {
                                          return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                            field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                      });
                                      if (rnd1 == null)
                                      {
                                          listRnd1.Add(new LG_RND1()
                                          {
                                              c_batch = field.Batch.ToUpper(),
                                              c_gdg = gudang,
                                              c_iteno = field.Item,
                                              c_rnno = rnID,
                                              n_bqty = 0,
                                              n_bsisa = 0,
                                              n_gqty = field.Quantity,
                                              n_gsisa = 0
                                          });
                                          spgTempId = string.Concat("SGXXXX03UJ");
                                          listSjd1.Add(new LG_SJD1()
                                          {
                                              c_batch = field.Batch.ToUpper(),
                                              c_gdg = '1',
                                              c_iteno = field.Item,
                                              c_sjno = sjID,
                                              c_spgno = spgTempId,
                                              n_bqty = 0,
                                              n_booked_bad = 0,
                                              n_booked = field.Quantity,
                                              n_gqty = field.Quantity,
                                              l_expired = field.ExpiredDateFormated < DateTime.Now ? true : false,
                                          });
                                          listRnd1Ulujami.Add(new LG_RND1()
                                          {
                                              c_batch = field.Batch.ToUpper(),
                                              c_gdg = '6',
                                              c_iteno = field.Item,
                                              c_rnno = rnID,
                                              n_bqty = 0,
                                              n_bsisa = 0,
                                              n_gqty = field.Quantity,
                                              n_gsisa = field.Quantity,
                                          });
                                          Commons.CheckAndProcessBatchRN(db, field.Item, field.Batch, field.ExpiredDateFormated, nipEntry);
                                          claimaccd = (from q in db.LG_ClaimAccDs
                                                       where q.c_claimaccno == field.ReferenceID
                                                       && q.c_iteno == field.Item
                                                       select q).Take(1).SingleOrDefault();
                                          claimaccd.n_sisa -= field.Quantity;
                                          totalDetails++;
                                      }
                                      else
                                      {
                                          rnd1.n_gqty += field.Quantity;
                                          rnd1.n_gsisa += field.Quantity;
                                          claimaccd = (from q in db.LG_ClaimAccDs
                                                       where q.c_claimaccno == field.ReferenceID
                                                       && q.c_iteno == field.Item
                                                       select q).Take(1).SingleOrDefault();
                                          claimaccd.n_sisa -= field.Quantity;
                                          totalDetails++;
                                      }
                                      #endregion

                                      #region New Stock Indra 20180305FM

                                      SavingStock.DailyStock(db, gudang.ToString(),
                                                                 rnID,
                                                                 "03".ToString(),
                                                                 field.Item,
                                                                 field.Batch,
                                                                 field.Quantity,
                                                                 0,
                                                                 "KOSONG",
                                                                 "01",
                                                                 "01",
                                                                 nipEntry,
                                                                 "01");
                                      
                                      if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                                      sjID,
                                                                      "01",
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

                                      SavingStock.DailyStock(db, '6'.ToString(),
                                                                 rnID,
                                                                 "03".ToString(),
                                                                 field.Item,
                                                                 field.Batch,
                                                                 field.Quantity,
                                                                 0,
                                                                 "KOSONG",
                                                                 "01",
                                                                 "01",
                                                                 nipEntry,
                                                                 "01");

                                      #endregion

                                  }
                              }
                              if ((listRnd1.Count > 0) && (listRnd2.Count > 0))
                              {
                                  db.LG_RND1s.InsertAllOnSubmit(listRnd1.ToArray());
                                  db.LG_RND2s.InsertAllOnSubmit(listRnd2.ToArray());
                              }
                              listRnd1.Clear();
                              listRnd2.Clear();
                              if ((listRnd1Ulujami.Count > 0) && (listRnd2Ulujami.Count > 0))
                              {
                                  db.LG_RND1s.InsertAllOnSubmit(listRnd1Ulujami.ToArray());
                                  db.LG_RND2s.InsertAllOnSubmit(listRnd2Ulujami.ToArray());
                              }
                              listRnd1.Clear();
                              listRnd2.Clear();
                              if ((listSjd1.Count > 0) && (listSjd2.Count > 0))
                              {
                                  db.LG_SJD1s.InsertAllOnSubmit(listSjd1.ToArray());
                                  db.LG_SJD2s.InsertAllOnSubmit(listSjd2.ToArray());
                              }
                              listSjd1.Clear();
                              listSjd2.Clear();
                          }
                      }
                    #endregion


                    #region Type 01 || 06

                    if (structure.Fields.TypeRN.Equals("01", StringComparison.OrdinalIgnoreCase) ||
                      structure.Fields.TypeRN.Equals("06", StringComparison.OrdinalIgnoreCase))
                    {

                        #region Validasi New Stock Indra 20180305FM

                        if (string.IsNullOrEmpty(structure.Fields.DOPrincipal))
                        {
                            result = "Nomor DO Suplier harus terisi.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                        else if (structure.Fields.TanggalDOFormat.Equals(DateTime.MinValue))
                        {
                            result = "Format tanggal DO tidak dapat terbaca.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                        else if (string.IsNullOrEmpty(structure.Fields.Suplier))
                        {
                            result = "Suplier tidak boleh kosong.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            db.Transaction.Rollback();

                            goto endLogic;
                        }

                        #endregion

                        //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "RN");
                        string cab = "";
                        if (PO.c_cab == null)
                        {
                            cab = "";
                        }
                        else
                        {
                            cab = PO.c_cab.Trim();
                        }
                        if (cab == "X8")
                        {
                            rnh = new LG_RNH()
                            {
                                c_dono = structure.Fields.DOPrincipal,
                                c_entry = nipEntry,
                                c_from = structure.Fields.Suplier,
                                c_gdg = gudang,
                                c_rnno = rnID,
                                c_type = "01",
                                c_update = nipEntry,
                                d_dodate = structure.Fields.TanggalDOFormat,
                                d_entry = date,
                                d_rndate = date,
                                d_update = date,
                                n_bea = structure.Fields.Bea,
                                l_float = structure.Fields.Floating,
                                l_print = false,
                                l_status = false,
                                //l_rnkhusus = true,
                                l_rnkhusus = true,
                                //l_khusus = (structure.Fields.TypeRN.Equals("06", StringComparison.OrdinalIgnoreCase) && structure.Fields.OrderKhusus ? true : false),
                                v_ket = structure.Fields.Keterangan
                            };
                        }
                        else
                        {
                            rnh = new LG_RNH()
                            {
                                c_dono = structure.Fields.DOPrincipal,
                                c_entry = nipEntry,
                                c_from = structure.Fields.Suplier,
                                c_gdg = gudang,
                                c_rnno = rnID,
                                c_type = "01",
                                c_update = nipEntry,
                                d_dodate = structure.Fields.TanggalDOFormat,
                                d_entry = date,
                                d_rndate = date,
                                d_update = date,
                                n_bea = structure.Fields.Bea,
                                l_float = structure.Fields.Floating,
                                l_print = false,
                                l_status = false,
                                //l_rnkhusus = true,
                                l_rnkhusus = (structure.Fields.TypeRN.Equals("06", StringComparison.OrdinalIgnoreCase)),
                                //l_khusus = (structure.Fields.TypeRN.Equals("06", StringComparison.OrdinalIgnoreCase) && structure.Fields.OrderKhusus ? true : false),
                                v_ket = structure.Fields.Keterangan
                            };
                        }
                        db.LG_RNHs.InsertOnSubmit(rnh);

                        //Indra 20181001FM
                        //STT Auto ketika RN Khusus
                        if ((structure.Fields.TipeSTT == "0"))
                        {
                            wph = new SCMS_WPH()
                            {
                                c_gdg = Convert.ToChar(structure.Fields.Gudang),
                                c_nodoc = stID,
                                d_wpdate = date,
                                c_urut = "AUTO",
                                c_nosup = structure.Fields.Suplier,
                                c_plat = "AUTO",
                                c_type = "01",
                                c_entry = nipEntry,
                                v_entry = nipEntry,
                                d_entry = date,
                                c_update = nipEntry,
                                d_update = date,
                                l_print = true,
                                l_scan = true
                            };

                            db.SCMS_WPHs.InsertOnSubmit(wph);
                        }                                 

                        #region Old Coded

                        //db.SubmitChanges();

                        //rnh = (from q in db.LG_RNHs
                        //       where q.v_ket == tmpNumbering && q.c_gdg == gdg
                        //       select q).Take(1).SingleOrDefault();

                        //if (rnh == null)
                        //{
                        //  result = "Nomor Receive tidak dapat di raih.";

                        //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        //  if (db.Transaction != null)
                        //  {
                        //    db.Transaction.Rollback();
                        //  }

                        //  goto endLogic;
                        //}
                        //else if (rnh.c_rnno.Equals("XXXXXXXXXX"))
                        //{
                        //  result = "Trigger Receive Note tidak aktif.";

                        //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        //  if (db.Transaction != null)
                        //  {
                        //    db.Transaction.Rollback();
                        //  }

                        //  goto endLogic;
                        //}

                        //rnh.v_ket = structure.Fields.Keterangan;

                        //rnID = rnh.c_rnno;

                        #endregion

                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            listRnd1 = new List<LG_RND1>();
                            listRnd2 = new List<LG_RND2>();
                            listRnd3 = new List<LG_RND3>();

                            //Indra 20181001FM
                            //STT Auto ketika RN Khusus
                            listWpd = new List<SCMS_WPD>();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                field = structure.Fields.Field[nLoop];


                                string CekBatch = (field.Batch ?? string.Empty.Trim());

                                if ((CekBatch.Contains("'")) || (CekBatch.Contains("*")) || (CekBatch.Contains("%")))
                                {
                                    result = "Tidak Boleh Karakter ' ( Kutip) atau  * (Bintang) atau %(Persentase) ";

                                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                    goto endLogic;

                                }


                                if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0) && (!string.IsNullOrEmpty(field.Batch)) && (field.Batch.Length > 0))
                                {
                                    if (field.TypeTrx.Equals("02", StringComparison.OrdinalIgnoreCase))
                                    {
                                        modifedPo = true;
                                    }
                                    else
                                    {
                                        modifedPo = Commons.PO_Modifikasi(db, gudang, field.ReferenceID, field.Item, false, field.Quantity);
                                    }

                                    if (modifedPo)
                                    {
                                        if (structure.Fields.TypeRN.Equals("06", StringComparison.OrdinalIgnoreCase))
                                        {
                                            modifedPo = Commons.DOPharos_Modifikasi(db, structure.Fields.Suplier, structure.Fields.DOPrincipal, field.Item, field.Batch, false, field.Quantity);
                                        }
                                        else
                                        {
                                            modifedPo = true;
                                        }

                                        if (modifedPo)
                                        {
                                            #region Detail Data

                                            listRnd2.Add(new LG_RND2()
                                            {
                                                // PROJECT
                                                c_batch = field.Batch.ToUpper(),
                                                c_gdg = gudang,
                                                c_iteno = field.Item,
                                                c_no = field.ReferenceID,
                                                c_rnno = rnID,
                                                c_type = field.TypeTrx,
                                                n_bqty = 0,
                                                n_gqty = field.Quantity,
                                                n_floqty = structure.Fields.Floating ? field.Quantity : 0
                                            });

                                            //Indra 20180927FM
                                            //ST Tiket RN Pembelian
                                            bool isOrKhusus = Commons.IsORKhusus(db, field.ReferenceID);

                                            if (isOrKhusus)
                                            {
                                                rnh.l_khusus = isOrKhusus;                                                
                                            }

                                            //Indra 20180927FM
                                            //ST Tiket RN Pembelian
                                            if ((!isOrKhusus) && (structure.Fields.TipeSTT == "0"))
                                            {
                                                result = "Tipe PO untuk Nomor PO " + field.ReferenceID + " adalah regular, silahkan ubah tipe RN menjadi Regular.";

                                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                                if (db.Transaction != null)
                                                {
                                                    db.Transaction.Rollback();
                                                }

                                                goto endLogic;
                                            }

                                            if ((isOrKhusus) && (structure.Fields.TipeSTT == "1"))
                                            {
                                                result = "Tipe PO untuk Nomor PO " + field.ReferenceID + " adalah khusus, silahkan ubah tipe RN menjadi Auto/ADM.";

                                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                                if (db.Transaction != null)
                                                {
                                                    db.Transaction.Rollback();
                                                }

                                                goto endLogic;
                                            }                                            

                                            if ((structure.Fields.TipeSTT == "0"))
                                            {

                                                wpd = (from q in db.SCMS_WPDs
                                                       where q.c_no == field.ReferenceID
                                                          && q.c_nodoc == stID
                                                       select q).Take(1).SingleOrDefault();
                                                if (wpd == null)
                                                {
                                                    listWpd.Add(new SCMS_WPD()
                                                    {
                                                        c_nodoc = stID,
                                                        c_no = field.ReferenceID,
                                                        l_rn = true
                                                    });

                                                    db.SCMS_WPDs.InsertAllOnSubmit(listWpd.ToArray());
                                                    db.SubmitChanges();
                                                }
                                            }
                                            else
                                            {
                                                wpd = (from q in db.SCMS_WPDs
                                                       where q.c_no == field.ReferenceID
                                                          && q.c_nodoc == structure.Fields.NoSerahTerimaTiket
                                                       select q).Take(1).SingleOrDefault();

                                                if (wpd != null)
                                                {
                                                    wph = (from q in db.SCMS_WPHs
                                                           where q.c_nodoc == wpd.c_nodoc
                                                           select q).Take(1).SingleOrDefault();

                                                    wph.c_rn = nipEntry;
                                                    wph.d_rn = date;

                                                    wpd.l_rn = true;
                                                }
                                                else
                                                {
                                                    result = "Nomor PO " + field.ReferenceID + " atas No. Serah Terima " + structure.Fields.NoSerahTerimaTiket + " belum di scan, silahkan scan terlebih dahulu.";

                                                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                                    if (db.Transaction != null)
                                                    {
                                                        db.Transaction.Rollback();
                                                    }

                                                    goto endLogic;
                                                }
                                            }                                           

                                            //rnd1 = listRnd1.Where(x => //(x.c_batch == field.Batch) &&
                                            //  ((string.IsNullOrEmpty(x.c_batch) ? string.Empty : x.c_batch.Trim()) == field.Batch) &&
                                            //  (x.c_gdg == gudang) && (x.c_iteno == field.Item) &&
                                            //  (x.c_rnno == rnID)).Take(1).SingleOrDefault();

                                            rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
                                            {
                                                return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                  field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                            });

                                            if (rnd1 == null)
                                            {
                                                listRnd1.Add(new LG_RND1()
                                                {
                                                    //Project
                                                    c_batch = field.Batch.ToUpper(),
                                                    c_gdg = gudang,
                                                    c_iteno = field.Item,
                                                    c_rnno = rnID,
                                                    n_bqty = 0,
                                                    n_bsisa = 0,
                                                    n_gqty = field.Quantity,
                                                    n_gsisa = field.Quantity,
                                                });

                                                // Check Batch
                                                Commons.CheckAndProcessBatchRN(db, field.Item, field.Batch, field.ExpiredDateFormated, nipEntry);                                                
                                            }
                                            else
                                            {
                                                rnd1.n_gsisa = rnd1.n_gqty += field.Quantity;
                                            }

                                            if (structure.Fields.Floating)
                                            {
                                                rnd3 = listRnd3.Find(delegate(LG_RND3 rnd3s)
                                                {
                                                    return field.Item.Equals((string.IsNullOrEmpty(rnd3s.c_iteno) ? string.Empty : rnd3s.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.Batch.Equals((string.IsNullOrEmpty(rnd3s.c_batch) ? string.Empty : rnd3s.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                                });

                                                if (rnd3 == null)
                                                {
                                                    listRnd3.Add(new LG_RND3()
                                                    {
                                                      // project
                                                        c_batch = field.Batch.ToUpper(),
                                                        c_gdg = gudang,
                                                        c_iteno = field.Item,
                                                        c_rnno = rnID,
                                                        n_bqty = 0,
                                                        n_gqty = field.Quantity,
                                                        //n_floqty = (structure.Fields.Floating ? field.Quantity : 0)
                                                    });
                                                }
                                                else
                                                {
                                                    rnd3.n_gqty = rnd3.n_gqty += field.Quantity;
                                                }
                                            }

                                            #region New Stock Indra 20180305FM                                            

                                            if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                                            rnID,
                                                                            "01",
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

                                            totalDetails++;

                                            #endregion
                                        }
                                    }
                                }
                            }

                            if ((listRnd1.Count > 0) && (listRnd2.Count > 0))
                            {
                                db.LG_RND1s.InsertAllOnSubmit(listRnd1.ToArray());
                                db.LG_RND2s.InsertAllOnSubmit(listRnd2.ToArray());
                            }

                            //db.SubmitChanges();

                            listRnd1.Clear();
                            listRnd2.Clear();

                            //Indra 20180927FM
                            //ST Tiket RN Pembelian
                            listWpd.Clear();
                        }
                    }

                    #endregion

                    #region Type 02

                    #region Validasi New Stock Indra 20180305FM

                    else if (structure.Fields.TypeRN.Equals("02", StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.IsNullOrEmpty(structure.Fields.DOPrincipal))
                        {
                            result = "Nomor DO Suplier harus terisi.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                        else if (structure.Fields.TanggalDOFormat.Equals(DateTime.MinValue))
                        {
                            result = "Format tanggal DO tidak dapat terbaca.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                        else if (string.IsNullOrEmpty(structure.Fields.Suplier))
                        {
                            result = "Suplier tidak boleh kosong.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }

                        #endregion

                        //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "RN");

                        rnh = new LG_RNH()
                        {
                            c_dono = structure.Fields.DOPrincipal,
                            c_entry = nipEntry,
                            c_from = structure.Fields.Suplier,
                            c_gdg = gudang,
                            c_rnno = rnID,
                            c_type = structure.Fields.TypeRN,
                            c_update = nipEntry,
                            d_dodate = structure.Fields.TanggalDOFormat,
                            d_entry = date,
                            d_rndate = date,
                            d_update = date,
                            n_bea = structure.Fields.Bea,
                            l_float = false,
                            l_print = false,
                            l_status = false,
                            v_ket = structure.Fields.Keterangan
                        };

                        db.LG_RNHs.InsertOnSubmit(rnh);

                        #region Old Coded

                        //db.SubmitChanges();

                        //rnh = (from q in db.LG_RNHs
                        //       where q.v_ket == tmpNumbering && q.c_gdg == gdg
                        //       select q).Take(1).SingleOrDefault();

                        //if (rnh == null)
                        //{
                        //  result = "Nomor Receive tidak dapat di raih.";

                        //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        //  if (db.Transaction != null)
                        //  {
                        //    db.Transaction.Rollback();
                        //  }

                        //  goto endLogic;
                        //}
                        //else if (rnh.c_rnno.Equals("XXXXXXXXXX"))
                        //{
                        //  result = "Trigger Receive Note tidak aktif.";

                        //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        //  if (db.Transaction != null)
                        //  {
                        //    db.Transaction.Rollback();
                        //  }

                        //  goto endLogic;
                        //}

                        //rnh.v_ket = structure.Fields.Keterangan;

                        //rnID = rnh.c_rnno;

                        #endregion

                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            listRnd1 = new List<LG_RND1>();
                            listRnd2 = new List<LG_RND2>();
                            listRnd3 = new List<LG_RND3>();

                            //lstTemp = structure.Fields.Field.Where(t => (!string.IsNullOrEmpty(t.ReferenceID))).GroupBy(x => x.ReferenceID).Select(y => y.Key).ToList();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                field = structure.Fields.Field[nLoop];


                                string CekBatch = (field.Batch ?? string.Empty.Trim());

                                if ((CekBatch.Contains("'")) || (CekBatch.Contains("*")) || (CekBatch.Contains("%")))
                                {
                                    result = "Tidak Boleh Karakter ' ( Kutip) atau  * (Bintang) atau %(Persentase) ";

                                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                    goto endLogic;

                                }


                                if ((field != null) && field.IsNew && (!field.IsDelete) && ((field.Quantity > 0) || (field.QuantityBad > 0)) && (!string.IsNullOrEmpty(field.Batch)) && (field.Batch.Length > 0))
                                {
                                    //rsd2 = (from q in db.LG_RSD2s
                                    //        join q1 in db.LG_RSHes on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                                    //        where q1.c_type == "02" && q1.c_rsno == field.ReferenceID
                                    //        && q.c_gdg == gudang && q.c_iteno == field.Item && q.c_batch == field.Batch
                                    //        select q).Take(1).SingleOrDefault();

                                    listRSD2 = (from q in db.LG_RSHes
                                                join q1 in db.LG_RSD2s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                                                where (q.c_gdg == gudang) && q.c_rsno == field.ReferenceID
                                                && (q.c_type != "03") && q1.c_batch == field.Batch
                                                && ((q1.n_gsisa > 0) || (q1.n_bsisa > 0))
                                                  && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                                select q1).Distinct().ToList();

                                    if ((listRSD2 != null) && (listRSD2.Count > 0))
                                    {   
                                        #region New Stock Indra 20180305FM

                                        if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                                        rnID,
                                                                        structure.Fields.TypeRN,
                                                                        field.Item,
                                                                        field.Batch,
                                                                        field.Quantity,
                                                                        field.QuantityBad,
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

                                        for (nLoopC = 0; nLoopC < listRSD2.Count; nLoopC++)
                                        {
                                            rsd2 = listRSD2.Find(delegate(LG_RSD2 rsd)
                                            {
                                                return field.ReferenceID.Equals((string.IsNullOrEmpty(rsd.c_rsno) ? string.Empty : rsd.c_rsno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                  field.Item.Equals((string.IsNullOrEmpty(rsd.c_iteno) ? string.Empty : rsd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                  field.Batch.Equals((string.IsNullOrEmpty(rsd.c_batch) ? string.Empty : rsd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                  listRSD2[nLoopC].c_rnno.Equals((string.IsNullOrEmpty(rsd.c_rnno) ? string.Empty : rsd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                  (((rsd.n_gsisa.HasValue ? rsd.n_gsisa.Value : 0) > 0) || ((rsd.n_bsisa.HasValue ? rsd.n_bsisa.Value : 0) > 0));
                                            });

                                            if (rsd2 != null)
                                            {
                                                #region Detail Data

                                                #region Old Code

                                                //rnd2 = listRnd2.Find(delegate(LG_RND2 rnd)
                                                //{
                                                //  return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                //    field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                //    field.ReferenceID.Equals((string.IsNullOrEmpty(rnd.c_no) ? string.Empty : rnd.c_no.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                //    field.TypeTrx.Equals((string.IsNullOrEmpty(rnd.c_type) ? string.Empty : rnd.c_type.Trim()), StringComparison.OrdinalIgnoreCase);
                                                //});

                                                //if (rnd2 == null)
                                                //{
                                                //  listRnd2.Add(new LG_RND2()
                                                //  {
                                                //    c_batch = field.Batch,
                                                //    c_gdg = gudang,
                                                //    c_iteno = field.Item,
                                                //    c_no = field.ReferenceID,
                                                //    c_rnno = rnID,
                                                //    c_type = field.TypeTrx,
                                                //    n_bqty = rsd2.n_bsisa,
                                                //    n_gqty = rsd2.n_gsisa
                                                //  });
                                                //}
                                                //else
                                                //{
                                                //  rnd2.n_gqty += rsd2.n_gsisa;
                                                //  rnd2.n_bqty += rsd2.n_bsisa;
                                                //}

                                                //rnd1 = listRnd1.Where(x => //(x.c_batch == field.Batch) &&
                                                //  ((string.IsNullOrEmpty(x.c_batch) ? string.Empty : x.c_batch.Trim()) == field.Batch) &&
                                                //  (x.c_gdg == gudang) && (x.c_iteno == field.Item) &&
                                                //  (x.c_rnno == rnID)).Take(1).SingleOrDefault();

                                                #endregion

                                                rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
                                                {
                                                    return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      rnID.Equals((string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
                                                });

                                                if (rnd1 == null)
                                                {
                                                    listRnd1.Add(new LG_RND1()
                                                    {
                                                        c_batch = field.Batch,
                                                        c_gdg = gudang,
                                                        c_iteno = field.Item,
                                                        c_rnno = rnID,
                                                        n_bqty = rsd2.n_bsisa,
                                                        n_bsisa = rsd2.n_bsisa,
                                                        n_gqty = rsd2.n_gsisa,
                                                        n_gsisa = rsd2.n_gsisa
                                                    });

                                                    // Check Batch
                                                    Commons.CheckAndProcessBatchRN(db, field.Item, field.Batch, field.ExpiredDateFormated, nipEntry);
                                                }
                                                else
                                                {
                                                    rnd1.n_bsisa = rnd1.n_bqty += rsd2.n_bsisa;
                                                    rnd1.n_gsisa = rnd1.n_gqty += rsd2.n_gsisa;
                                                }

                                                rnd3 = listRnd3.Find(delegate(LG_RND3 rnd)
                                                {
                                                    return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.ReferenceID.Equals((string.IsNullOrEmpty(rnd.c_no) ? string.Empty : rnd.c_no.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.TypeTrx.Equals((string.IsNullOrEmpty(rnd.c_type) ? string.Empty : rnd.c_type.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      (string.IsNullOrEmpty(rsd2.c_rnno) ? string.Empty : rsd2.c_rnno.Trim()).Equals((string.IsNullOrEmpty(rnd.c_rn) ? string.Empty : rnd.c_rn.Trim()), StringComparison.OrdinalIgnoreCase);
                                                });

                                                if (rnd3 == null)
                                                {
                                                    listRnd3.Add(new LG_RND3()
                                                    {
                                                        c_batch = field.Batch,
                                                        c_gdg = gudang,
                                                        c_iteno = field.Item,
                                                        c_no = rsd2.c_rsno,
                                                        c_rn = rsd2.c_rnno,
                                                        c_rnno = rnID,
                                                        c_type = field.TypeTrx,
                                                        n_bqty = rsd2.n_bsisa,
                                                        n_gqty = rsd2.n_gsisa
                                                    });
                                                }
                                                else
                                                {
                                                    rnd3.n_gqty += rsd2.n_gsisa;
                                                    rnd3.n_bqty += rsd2.n_bsisa;
                                                }

                                                rsd2.n_bsisa -= rsd2.n_bsisa;
                                                rsd2.n_gsisa -= rsd2.n_gsisa;

                                                totalDetails++;

                                                #endregion
                                            }
                                        }
                                    }
                                }
                            }

                            listRSD2.Clear();
                            //lstTemp.Clear();

                            if ((listRnd1 != null) && (listRnd1.Count > 0))
                            {
                                db.LG_RND1s.InsertAllOnSubmit(listRnd1.ToArray());
                                listRnd1.Clear();
                            }

                            if ((listRnd2 != null) && (listRnd2.Count > 0))
                            {
                                db.LG_RND2s.InsertAllOnSubmit(listRnd2.ToArray());
                                listRnd2.Clear();
                            }

                            if ((listRnd3 != null) && (listRnd3.Count > 0))
                            {
                                db.LG_RND3s.InsertAllOnSubmit(listRnd3.ToArray());
                                listRnd3.Clear();
                            }
                        }
                    }

                    #endregion

                    #region Type 03

                    else if (structure.Fields.TypeRN.Equals("03", StringComparison.OrdinalIgnoreCase))
                    {

                        #region Validasi Indra 20171120

                        if (string.IsNullOrEmpty(structure.Fields.DOPrincipal))
                        {
                            result = "Nomor DO Suplier harus terisi.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                        else if (structure.Fields.TanggalDOFormat.Equals(DateTime.MinValue))
                        {
                            result = "Format tanggal DO tidak dapat terbaca.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                        else if (string.IsNullOrEmpty(structure.Fields.Suplier))
                        {
                            result = "Suplier tidak boleh kosong.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }

                        #endregion

                        //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "RN");

                        rnh = new LG_RNH()
                        {
                            c_dono = structure.Fields.DOPrincipal,
                            c_entry = nipEntry,
                            c_from = structure.Fields.Suplier,
                            c_gdg = gudang,
                            c_rnno = rnID,
                            c_type = structure.Fields.TypeRN,
                            c_update = nipEntry,
                            d_dodate = structure.Fields.TanggalDOFormat,
                            d_entry = date,
                            d_rndate = date,
                            d_update = date,
                            n_bea = structure.Fields.Bea,
                            l_float = false,
                            l_print = false,
                            l_status = false,
                            v_ket = structure.Fields.Keterangan
                        };

                        db.LG_RNHs.InsertOnSubmit(rnh);

                        #region Old Coded

                        //db.SubmitChanges();

                        //rnh = (from q in db.LG_RNHs
                        //       where q.v_ket == tmpNumbering && q.c_gdg == gdg
                        //       select q).Take(1).SingleOrDefault();

                        //if (rnh == null)
                        //{
                        //  result = "Nomor Receive tidak dapat di raih.";

                        //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        //  if (db.Transaction != null)
                        //  {
                        //    db.Transaction.Rollback();
                        //  }

                        //  goto endLogic;
                        //}
                        //else if (rnh.c_rnno.Equals("XXXXXXXXXX"))
                        //{
                        //  result = "Trigger Receive Note tidak aktif.";

                        //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        //  if (db.Transaction != null)
                        //  {
                        //    db.Transaction.Rollback();
                        //  }

                        //  goto endLogic;
                        //}

                        //rnh.v_ket = structure.Fields.Keterangan;

                        //rnID = rnh.c_rnno;

                        #endregion

                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            listRnd1 = new List<LG_RND1>();
                            listRnd2 = new List<LG_RND2>();
                            claimaccd = new LG_ClaimAccD();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                field = structure.Fields.Field[nLoop];



                                string CekBatch = (field.Batch ?? string.Empty.Trim());

                                if ((CekBatch.Contains("'")) || (CekBatch.Contains("*")) || (CekBatch.Contains("%")))
                                {
                                    result = "Tidak Boleh Karakter ' ( Kutip) atau  * (Bintang) atau %(Persentase) ";

                                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                    goto endLogic;
                                    
                                }


                                if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0))
                                {
                                    #region Detail Data

                                    listRnd2.Add(new LG_RND2()
                                    {
                                        c_batch = field.Batch,
                                        c_gdg = gudang,
                                        c_iteno = field.Item,
                                        c_no = field.ReferenceID,
                                        c_rnno = rnID,
                                        c_type = field.TypeTrx,
                                        n_bqty = 0,
                                        n_gqty = field.Quantity
                                    });

                                    //rnd1 = listRnd1.Where(x => //(x.c_batch == field.Batch) &&
                                    //  ((string.IsNullOrEmpty(x.c_batch) ? string.Empty : x.c_batch.Trim()) == field.Batch) &&
                                    //  (x.c_gdg == gudang) && (x.c_iteno == field.Item) &&
                                    //  (x.c_rnno == rnID)).Take(1).SingleOrDefault();

                                    rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
                                    {
                                        return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                          field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                    });

                                    if (rnd1 == null)
                                    {
                                        listRnd1.Add(new LG_RND1()
                                        {
                                            c_batch = field.Batch,
                                            c_gdg = gudang,
                                            c_iteno = field.Item,
                                            c_rnno = rnID,
                                            n_bqty = 0,
                                            n_bsisa = 0,
                                            n_gqty = field.Quantity,
                                            n_gsisa = field.Quantity
                                        });

                                        // Check Batch
                                        Commons.CheckAndProcessBatchRN(db, field.Item, field.Batch, field.ExpiredDateFormated, nipEntry);

                                        claimaccd = (from q in db.LG_ClaimAccDs
                                                     where q.c_claimaccno == field.ReferenceID
                                                     && q.c_iteno == field.Item
                                                     select q).Take(1).SingleOrDefault();

                                        claimaccd.n_sisa -= field.Quantity;

                                        totalDetails++;
                                    }
                                    else
                                    {
                                        rnd1.n_gqty += field.Quantity;
                                        rnd1.n_gsisa += field.Quantity;

                                        claimaccd = (from q in db.LG_ClaimAccDs
                                                     where q.c_claimaccno == field.ReferenceID
                                                     && q.c_iteno == field.Item
                                                     select q).Take(1).SingleOrDefault();

                                        claimaccd.n_sisa -= field.Quantity;

                                        totalDetails++;
                                    }

                                    #region New Stock Indra 20180305FM

                                    if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                                    rnID,
                                                                    structure.Fields.TypeRN,
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

                                    #endregion
                                }
                            }

                            if ((listRnd1.Count > 0) && (listRnd2.Count > 0))
                            {
                                db.LG_RND1s.InsertAllOnSubmit(listRnd1.ToArray());
                                db.LG_RND2s.InsertAllOnSubmit(listRnd2.ToArray());
                            }

                            //db.SubmitChanges();

                            listRnd1.Clear();
                            listRnd2.Clear();
                        }
                    }
                    #endregion

                    #region Type 04

                    else if (structure.Fields.TypeRN.Equals("04", StringComparison.OrdinalIgnoreCase))
                    {

                        #region Validasi Indra 20171120

                        if (string.IsNullOrEmpty(structure.Fields.DOPrincipal))
                        {
                            result = "Nomor DO Suplier harus terisi.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                        else if (structure.Fields.TanggalDOFormat.Equals(DateTime.MinValue))
                        {
                            result = "Format tanggal DO tidak dapat terbaca.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                        else if (string.IsNullOrEmpty(structure.Fields.Suplier))
                        {
                            result = "Suplier tidak boleh kosong.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }

                        #endregion

                        //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "RN");

                        rnh = new LG_RNH()
                        {
                            c_dono = structure.Fields.DOPrincipal,
                            c_entry = nipEntry,
                            c_from = structure.Fields.Suplier,
                            c_gdg = gudang,
                            c_rnno = rnID,
                            c_type = structure.Fields.TypeRN,
                            c_update = nipEntry,
                            d_dodate = structure.Fields.TanggalDOFormat,
                            d_entry = date,
                            d_rndate = date,
                            d_update = date,
                            n_bea = structure.Fields.Bea,
                            l_float = false,
                            l_print = false,
                            l_status = false,
                            v_ket = structure.Fields.Keterangan
                        };

                        db.LG_RNHs.InsertOnSubmit(rnh);

                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            listRnd1 = new List<LG_RND1>();
                            listRnd2 = new List<LG_RND2>();
                            listRnd3 = new List<LG_RND3>();

                            lstTemp = structure.Fields.Field.Where(t => (!string.IsNullOrEmpty(t.ReferenceID))).GroupBy(x => x.ReferenceID).Select(y => y.Key).ToList();

                            typeList = new string[] { "02", "03" };

                            listRSD2 = (from q in db.LG_RSHes
                                        join q1 in db.LG_RSD2s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                                        where (q.c_gdg == gudang) && lstTemp.Contains(q.c_rsno)
                                        && typeList.Contains(q.c_type)
                                        && ((q1.n_gsisa > 0) || (q1.n_bsisa > 0))
                                          && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                        select q1).Distinct().ToList();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                field = structure.Fields.Field[nLoop];



                                string CekBatch = (field.Batch ?? string.Empty.Trim());

                                if ((CekBatch.Contains("'")) || (CekBatch.Contains("*")) || (CekBatch.Contains("%")))
                                {
                                    result = "Tidak Boleh Karakter ' ( Kutip) atau  * (Bintang) atau %(Persentase) ";

                                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                    goto endLogic;

                                }


                                if ((field != null) && field.IsNew && (!field.IsDelete) && ((field.Quantity > 0) || (field.QuantityBad > 0)) && (!string.IsNullOrEmpty(field.Batch)) && (field.Batch.Length > 0))
                                {
                                    if (field.RepackNew)
                                    {
                                        #region New Items

                                        #region Detail Data

                                        rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
                                        {
                                            return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                        });

                                        if (rnd1 == null)
                                        {
                                            listRnd1.Add(new LG_RND1()
                                            {
                                                c_batch = field.Batch,
                                                c_gdg = gudang,
                                                c_iteno = field.Item,
                                                c_rnno = rnID,
                                                n_bqty = 0,
                                                n_bsisa = 0,
                                                n_gqty = field.Quantity,
                                                n_gsisa = field.Quantity
                                            });

                                            // Check Batch
                                            Commons.CheckAndProcessBatchRN(db, field.Item, field.Batch, field.ExpiredDateFormated, nipEntry);
                                        }
                                        else
                                        {
                                            //rnd1.n_bsisa = rnd1.n_bqty += field.QuantityBad;
                                            rnd1.n_gsisa = rnd1.n_gqty += field.Quantity;
                                        }

                                        #region New Stock Indra 20180305FM

                                        if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                                        rnID,
                                                                        structure.Fields.TypeRN,
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

                                        totalDetails++;

                                        #endregion

                                        #endregion
                                    }
                                    else
                                    {
                                        #region Original

                                        nQtyAlloc = (field.QuantityBad + field.Quantity);

                                        listRSD2Copy = listRSD2.FindAll(delegate(LG_RSD2 rsd)
                                        {
                                            return field.ReferenceID.Equals((string.IsNullOrEmpty(rsd.c_rsno) ? string.Empty : rsd.c_rsno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              field.Item.Equals((string.IsNullOrEmpty(rsd.c_iteno) ? string.Empty : rsd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              field.Batch.Equals((string.IsNullOrEmpty(rsd.c_batch) ? string.Empty : rsd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              (((rsd.n_gsisa.HasValue ? rsd.n_gsisa.Value : 0) > 0) || ((rsd.n_bsisa.HasValue ? rsd.n_bsisa.Value : 0) > 0));
                                        });

                                        for (nLoopC = 0; nLoopC < listRSD2Copy.Count; nLoopC++)
                                        {
                                            rsd2 = listRSD2Copy[nLoopC];

                                            if (rsd2 != null)
                                            {
                                                nQtyAllocBad = field.QuantityBad;
                                                nQtyAllocGood = field.Quantity;

                                                nQtyBad = ((nQtyAllocBad > 0) && (nQtyAlloc > 0) ?
                                                  (nQtyAllocBad > nQtyAlloc ? nQtyAlloc : nQtyAllocBad) : 0);
                                                rsd2.n_bsisa -= nQtyBad;
                                                nQtyAlloc -= nQtyAllocBad;
                                                nQtyBad -= nQtyAllocBad;

                                                nQtyGood = ((nQtyAllocGood > 0) && (nQtyAlloc > 0) ?
                                                  (nQtyAllocGood > nQtyAlloc ? nQtyAlloc : nQtyAllocGood) : 0);
                                                rsd2.n_gsisa -= nQtyGood;
                                                nQtyAlloc -= nQtyAllocGood;
                                                nQtyGood -= nQtyAllocGood;

                                                #region Detail Data

                                                rnd2 = listRnd2.Find(delegate(LG_RND2 rnd)
                                                {
                                                    return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.ReferenceID.Equals((string.IsNullOrEmpty(rnd.c_no) ? string.Empty : rnd.c_no.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.TypeTrx.Equals((string.IsNullOrEmpty(rnd.c_type) ? string.Empty : rnd.c_type.Trim()), StringComparison.OrdinalIgnoreCase);
                                                });

                                                if (rnd2 == null)
                                                {
                                                    listRnd2.Add(new LG_RND2()
                                                    {
                                                        c_gdg = gudang,
                                                        c_rnno = rnID,
                                                        c_batch = field.Batch,
                                                        c_iteno = field.Item,
                                                        c_no = field.ReferenceID,
                                                        c_type = field.TypeTrx,
                                                        n_gqty = nQtyAllocGood,
                                                        n_bqty = nQtyAllocBad
                                                    });
                                                }
                                                else
                                                {
                                                    rnd2.n_gqty += nQtyAllocGood;
                                                    rnd2.n_bqty += nQtyAllocBad;
                                                }

                                                rnd3 = listRnd3.Find(delegate(LG_RND3 rnd)
                                                {
                                                    return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.ReferenceID.Equals((string.IsNullOrEmpty(rnd.c_no) ? string.Empty : rnd.c_no.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.TypeTrx.Equals((string.IsNullOrEmpty(rnd.c_type) ? string.Empty : rnd.c_type.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      (string.IsNullOrEmpty(rsd2.c_rnno) ? string.Empty : rsd2.c_rnno.Trim()).Equals((string.IsNullOrEmpty(rnd.c_rn) ? string.Empty : rnd.c_rn.Trim()), StringComparison.OrdinalIgnoreCase);
                                                });

                                                if (rnd3 == null)
                                                {
                                                    listRnd3.Add(new LG_RND3()
                                                    {
                                                        c_batch = field.Batch,
                                                        c_gdg = gudang,
                                                        c_iteno = field.Item,
                                                        c_no = rsd2.c_rsno,
                                                        c_rn = rsd2.c_rnno,
                                                        c_rnno = rnID,
                                                        c_type = field.TypeTrx,
                                                        n_gqty = nQtyAllocGood,
                                                        n_bqty = nQtyAllocBad
                                                    });
                                                }
                                                else
                                                {
                                                    rnd3.n_gqty += nQtyAllocGood;
                                                    rnd3.n_bqty += nQtyAllocBad;
                                                }

                                                totalDetails++;

                                                #endregion

                                                if (nQtyAllocBad <= 0)
                                                {
                                                    listRSD2.Remove(rsd2);
                                                }
                                                if (nQtyAlloc <= 0)
                                                {
                                                    break;
                                                }
                                            }
                                        }

                                        listRSD2Copy.Clear();

                                        #endregion
                                    }

                                    #region Backup

                                    //if (field.RepackNew)
                                    //{
                                    //  #region New Items

                                    //  #region Detail Data

                                    //  listRnd2.Add(new LG_RND2()
                                    //  {
                                    //    c_batch = field.Batch,
                                    //    c_gdg = gudang,
                                    //    c_iteno = field.Item,
                                    //    c_no = field.ReferenceID,
                                    //    c_rnno = rnID,
                                    //    c_type = field.TypeTrx,
                                    //    n_bqty = field.QuantityBad,
                                    //    n_gqty = field.Quantity
                                    //  });

                                    //  //rnd1 = listRnd1.Where(x => //(x.c_batch == field.Batch) &&
                                    //  //  ((string.IsNullOrEmpty(x.c_batch) ? string.Empty : x.c_batch.Trim()) == field.Batch) &&
                                    //  //  (x.c_gdg == gudang) && (x.c_iteno == field.Item) &&
                                    //  //  (x.c_rnno == rnID)).Take(1).SingleOrDefault();

                                    //  rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
                                    //  {
                                    //    return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                    //      field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                    //  });

                                    //  if (rnd1 == null)
                                    //  {
                                    //    listRnd1.Add(new LG_RND1()
                                    //    {
                                    //      c_batch = field.Batch,
                                    //      c_gdg = gudang,
                                    //      c_iteno = field.Item,
                                    //      c_rnno = rnID,
                                    //      n_bqty = field.QuantityBad,
                                    //      n_bsisa = field.QuantityBad,
                                    //      n_gqty = field.Quantity,
                                    //      n_gsisa = field.Quantity
                                    //    });

                                    //    // Check Batch
                                    //    Commons.CheckAndProcessBatch(db, field.Item, field.Batch, field.ExpiredDateFormated, nipEntry);
                                    //  }
                                    //  else
                                    //  {
                                    //    rnd1.n_bsisa = rnd1.n_bqty += field.QuantityBad;
                                    //    rnd1.n_gsisa = rnd1.n_gqty += field.Quantity;
                                    //  }

                                    //  //listRnd3.Add(new LG_RND3()
                                    //  //{
                                    //  //  c_batch = field.Batch,
                                    //  //  c_gdg = gudang,
                                    //  //  c_iteno = field.Item,
                                    //  //  c_no = rsd2.c_rsno,
                                    //  //  c_rn = rsd2.c_rnno,
                                    //  //  c_rnno = rnID,
                                    //  //  c_type = field.TypeTrx,
                                    //  //  n_bqty = field.QuantityBad,
                                    //  //  n_gqty = field.Quantity
                                    //  //});

                                    //  totalDetails++;

                                    //  #endregion

                                    //  #endregion
                                    //}
                                    //else
                                    //{
                                    //  #region Original

                                    //  rsd2 = listRSD2.Find(delegate(LG_RSD2 rsd)
                                    //  {
                                    //    return field.ReferenceID.Equals((string.IsNullOrEmpty(rsd.c_rsno) ? string.Empty : rsd.c_rsno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                    //      field.Item.Equals((string.IsNullOrEmpty(rsd.c_iteno) ? string.Empty : rsd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                    //      field.Batch.Equals((string.IsNullOrEmpty(rsd.c_batch) ? string.Empty : rsd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                    //      (((rsd.n_gsisa.HasValue ? rsd.n_gsisa.Value : 0) > 0) || ((rsd.n_bsisa.HasValue ? rsd.n_bsisa.Value : 0) > 0));
                                    //  });

                                    //  if (rsd2 != null)
                                    //  {
                                    //    nQtyAllocBad = (rsd2.n_bsisa.HasValue ? rsd2.n_bsisa.Value : 0);
                                    //    nQtyAllocGood = (rsd2.n_gsisa.HasValue ? rsd2.n_gsisa.Value : 0);

                                    //    nQtyBad = (field.QuantityBad + field.Quantity);

                                    //    rsd2.n_bsisa -= ((nQtyAllocBad > 0) && (nQtyBad > 0) ? 
                                    //      (nQtyAllocBad > nQtyBad ? nQtyBad : nQtyAllocBad) : 0);
                                    //    nQtyBad -= nQtyAllocBad;

                                    //    rsd2.n_gsisa -= ((nQtyAllocGood > 0) && (nQtyBad > 0) ?
                                    //      (nQtyAllocGood > nQtyBad ? nQtyBad : nQtyAllocGood) : 0);
                                    //    nQtyBad -= nQtyAllocGood;

                                    //    #region Detail Data

                                    //    listRnd2.Add(new LG_RND2()
                                    //    {
                                    //      c_batch = field.Batch,
                                    //      c_gdg = gudang,
                                    //      c_iteno = field.Item,
                                    //      c_no = field.ReferenceID,
                                    //      c_rnno = rnID,
                                    //      c_type = field.TypeTrx,
                                    //      n_bqty = field.QuantityBad,
                                    //      n_gqty = field.Quantity
                                    //    });

                                    //    //rnd1 = listRnd1.Where(x => //(x.c_batch == field.Batch) &&
                                    //    //  ((string.IsNullOrEmpty(x.c_batch) ? string.Empty : x.c_batch.Trim()) == field.Batch) &&
                                    //    //  (x.c_gdg == gudang) && (x.c_iteno == field.Item) &&
                                    //    //  (x.c_rnno == rnID)).Take(1).SingleOrDefault();

                                    //    rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
                                    //    {
                                    //      return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                    //        field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                    //    });

                                    //    if (rnd1 == null)
                                    //    {
                                    //      listRnd1.Add(new LG_RND1()
                                    //      {
                                    //        c_batch = field.Batch,
                                    //        c_gdg = gudang,
                                    //        c_iteno = field.Item,
                                    //        c_rnno = rnID,
                                    //        n_bqty = field.QuantityBad,
                                    //        n_bsisa = field.QuantityBad,
                                    //        n_gqty = field.Quantity,
                                    //        n_gsisa = field.Quantity
                                    //      });

                                    //      // Check Batch
                                    //      Commons.CheckAndProcessBatch(db, field.Item, field.Batch, field.ExpiredDateFormated, nipEntry);
                                    //    }
                                    //    else
                                    //    {
                                    //      rnd1.n_bsisa = rnd1.n_bqty += field.QuantityBad;
                                    //      rnd1.n_gsisa = rnd1.n_gqty += field.Quantity;
                                    //    }

                                    //    listRnd3.Add(new LG_RND3()
                                    //    {
                                    //      c_batch = field.Batch,
                                    //      c_gdg = gudang,
                                    //      c_iteno = field.Item,
                                    //      c_no = rsd2.c_rsno,
                                    //      c_rn = rsd2.c_rnno,
                                    //      c_rnno = rnID,
                                    //      c_type = field.TypeTrx,
                                    //      n_bqty = field.QuantityBad,
                                    //      n_gqty = field.Quantity
                                    //    });

                                    //    totalDetails++;

                                    //    #endregion
                                    //  }

                                    //  #endregion
                                    //}

                                    #endregion
                                }
                            }

                            listRSD2.Clear();
                            lstTemp.Clear();

                            if ((listRnd1 != null) && (listRnd1.Count > 0))
                            {
                                db.LG_RND1s.InsertAllOnSubmit(listRnd1.ToArray());
                                listRnd1.Clear();
                            }

                            if ((listRnd2 != null) && (listRnd2.Count > 0))
                            {
                                db.LG_RND2s.InsertAllOnSubmit(listRnd2.ToArray());
                                listRnd2.Clear();
                            }

                            if ((listRnd3 != null) && (listRnd3.Count > 0))
                            {
                                db.LG_RND3s.InsertAllOnSubmit(listRnd3.ToArray());
                                listRnd3.Clear();
                            }
                        }
                    }

                    #endregion

                    #region Old Coded

                    //#region Type 04

                    //if (structure.Fields.TypeRN.Equals("04", StringComparison.OrdinalIgnoreCase))
                    //{
                    //  if (string.IsNullOrEmpty(structure.Fields.DOPrincipal))
                    //  {
                    //    result = "Nomor DO Suplier harus terisi.";

                    //    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //    if (db.Transaction != null)
                    //    {
                    //      db.Transaction.Rollback();
                    //    }

                    //    goto endLogic;
                    //  }
                    //  else if (structure.Fields.TanggalDOFormat.Equals(DateTime.MinValue))
                    //  {
                    //    result = "Format tanggal DO tidak dapat terbaca.";

                    //    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //    if (db.Transaction != null)
                    //    {
                    //      db.Transaction.Rollback();
                    //    }

                    //    goto endLogic;
                    //  }
                    //  else if (string.IsNullOrEmpty(structure.Fields.Suplier))
                    //  {
                    //    result = "Suplier tidak boleh kosong.";

                    //    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //    if (db.Transaction != null)
                    //    {
                    //      db.Transaction.Rollback();
                    //    }

                    //    goto endLogic;
                    //  }

                    //  //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "RN");

                    //  rnh = new LG_RNH()
                    //  {
                    //    c_dono = structure.Fields.DOPrincipal,
                    //    c_entry = nipEntry,
                    //    c_from = structure.Fields.Suplier,
                    //    c_gdg = gudang,
                    //    c_rnno = rnID,
                    //    c_type = structure.Fields.TypeRN,
                    //    c_update = nipEntry,
                    //    d_dodate = structure.Fields.TanggalDOFormat,
                    //    d_entry = date,
                    //    d_rndate = date,
                    //    d_update = date,
                    //    n_bea = structure.Fields.Bea,
                    //    l_float = false,
                    //    l_print = false,
                    //    l_status = false,
                    //    v_ket = tmpNumbering
                    //  };

                    //  db.LG_RNHs.InsertOnSubmit(rnh);

                    //  #region Old Coded

                    //  //db.SubmitChanges();

                    //  //rnh = (from q in db.LG_RNHs
                    //  //       where q.v_ket == tmpNumbering && q.c_gdg == gdg
                    //  //       select q).Take(1).SingleOrDefault();

                    //  //if (rnh == null)
                    //  //{
                    //  //  result = "Nomor Receive tidak dapat di raih.";

                    //  //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  //  if (db.Transaction != null)
                    //  //  {
                    //  //    db.Transaction.Rollback();
                    //  //  }

                    //  //  goto endLogic;
                    //  //}
                    //  //else if (rnh.c_rnno.Equals("XXXXXXXXXX"))
                    //  //{
                    //  //  result = "Trigger Receive Note tidak aktif.";

                    //  //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  //  if (db.Transaction != null)
                    //  //  {
                    //  //    db.Transaction.Rollback();
                    //  //  }

                    //  //  goto endLogic;
                    //  //}

                    //  //rnh.v_ket = structure.Fields.Keterangan;

                    //  //rnID = rnh.c_rnno;

                    //  #endregion

                    //  if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    //  {
                    //    listRnd1 = new List<LG_RND1>();
                    //    listRnd2 = new List<LG_RND2>();
                    //    listRnd3 = new List<LG_RND3>();

                    //    lstTemp = structure.Fields.Field.Where(t => (!string.IsNullOrEmpty(t.ReferenceID))).GroupBy(x => x.ReferenceID).Select(y => y.Key).ToList();

                    //    listRSD2 = (from q in db.LG_RSHes
                    //                join q1 in db.LG_RSD2s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                    //                where (q.c_gdg == gudang) && lstTemp.Contains(q.c_rsno)
                    //                  && ((q1.n_gsisa > 0) || (q1.n_bsisa > 0))
                    //                  && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                    //                select q1).Distinct().ToList();

                    //    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    //    {
                    //      field = structure.Fields.Field[nLoop];

                    //      if ((field != null) && field.IsNew && (!field.RepackNew) && (!field.IsDelete) && ((field.Quantity > 0) || (field.QuantityBad > 0)))
                    //      {
                    //        #region Old Coded

                    //        #region Detil

                    //        var ListRSD2 = (from q in db.LG_RSD2s
                    //                        where q.c_rsno == field.ReferenceID
                    //                        && q.c_gdg == gudang && q.c_iteno == field.Item
                    //                        && q.c_batch == field.Batch
                    //                        select q).ToList();

                    //        #region Insert RN 1 & 2

                    //        listRnd2.Add(new LG_RND2()
                    //        {
                    //          c_batch = field.Batch,
                    //          c_gdg = gudang,
                    //          c_iteno = field.Item,
                    //          c_no = field.ReferenceID,
                    //          c_rnno = rnID,
                    //          c_type = "01",
                    //          n_bqty = field.QuantityBad,
                    //          n_gqty = field.Quantity
                    //        });

                    //        listRnd1.Add(new LG_RND1()
                    //        {
                    //          c_batch = field.Batch,
                    //          c_gdg = gudang,
                    //          c_iteno = field.Item,
                    //          c_rnno = rnID,
                    //          n_bqty = field.QuantityBad,
                    //          n_bsisa = field.QuantityBad,
                    //          n_gqty = field.Quantity,
                    //          n_gsisa = field.Quantity
                    //        });

                    //        #endregion

                    //        var obj = ListRSD2.GroupBy(x => new { x.c_iteno, x.c_batch }).Select(y => new { y.Key.c_iteno, y.Key.c_batch }).ToList();

                    //        var listRNs = (from q in db.LG_RND1s
                    //                       where obj.Contains(new { q.c_iteno, q.c_batch })
                    //                        && ((q.n_gsisa > 0) || (q.n_bsisa > 0))
                    //                       select new
                    //                       {
                    //                         q.c_rnno,
                    //                         q.c_iteno,
                    //                         q.c_batch,
                    //                         q.n_bsisa,
                    //                         q.n_gsisa
                    //                       }).ToList();

                    //        if (field.Quantity > 0 && field.QuantityBad == 0)
                    //        {
                    //          #region QtyGood Only

                    //          for (nLoopC = 0; nLoopC < ListRSD2.Count; nLoopC++)
                    //          {
                    //            nQtyAllocGood = field.Quantity;

                    //            if (ListRSD2[nLoopC].n_gsisa < nQtyAllocGood)
                    //            {
                    //              listRnd3.Add(new LG_RND3()
                    //              {
                    //                c_batch = ListRSD2[nLoopC].c_batch,
                    //                c_gdg = gudang,
                    //                c_iteno = field.Item,
                    //                c_no = ListRSD2[nLoopC].c_rsno,
                    //                c_rn = ListRSD2[nLoopC].c_rnno,
                    //                c_rnno = rnID,
                    //                c_type = "01",
                    //                n_bqty = 0,
                    //                n_gqty = ListRSD2[nLoopC].n_gsisa
                    //              });

                    //              nQtyAllocGood -= ListRSD2[nLoopC].n_gsisa ?? 0;
                    //              ListRSD2[nLoopC].n_gsisa = 0;

                    //            }
                    //            else
                    //            {
                    //              listRnd3.Add(new LG_RND3()
                    //              {
                    //                c_batch = ListRSD2[nLoopC].c_batch,
                    //                c_gdg = gudang,
                    //                c_iteno = field.Item,
                    //                c_no = ListRSD2[nLoopC].c_rsno,
                    //                c_rn = ListRSD2[nLoopC].c_rnno,
                    //                c_rnno = rnID,
                    //                c_type = "01",
                    //                n_bqty = 0,
                    //                n_gqty = nQtyAllocGood
                    //              });
                    //              ListRSD2[nLoopC].n_gsisa -= nQtyAllocGood;
                    //            }
                    //          }

                    //          #endregion
                    //        }
                    //        else if (field.Quantity == 0 && field.QuantityBad > 0)
                    //        {
                    //          #region QtyBad Only

                    //          for (nLoopC = 0; nLoopC < ListRSD2.Count; nLoopC++)
                    //          {
                    //            nQtyAllocBad = field.QuantityBad;

                    //            if (ListRSD2[nLoopC].n_bsisa < nQtyAllocBad)
                    //            {
                    //              listRnd3.Add(new LG_RND3()
                    //              {
                    //                c_batch = ListRSD2[nLoopC].c_batch,
                    //                c_gdg = gudang,
                    //                c_iteno = field.Item,
                    //                c_no = ListRSD2[nLoopC].c_rsno,
                    //                c_rn = ListRSD2[nLoopC].c_rnno,
                    //                c_rnno = rnID,
                    //                c_type = "01",
                    //                n_bqty = ListRSD2[nLoopC].n_bsisa,
                    //                n_gqty = 0
                    //              });

                    //              nQtyAllocBad -= ListRSD2[nLoopC].n_bsisa ?? 0;
                    //              ListRSD2[nLoopC].n_bsisa = 0;

                    //            }
                    //            else
                    //            {
                    //              listRnd3.Add(new LG_RND3()
                    //              {
                    //                c_batch = ListRSD2[nLoopC].c_batch,
                    //                c_gdg = gudang,
                    //                c_iteno = field.Item,
                    //                c_no = ListRSD2[nLoopC].c_rsno,
                    //                c_rn = ListRSD2[nLoopC].c_rnno,
                    //                c_rnno = rnID,
                    //                c_type = "01",
                    //                n_bqty = nQtyAllocBad,
                    //                n_gqty = 0
                    //              });
                    //              ListRSD2[nLoopC].n_bsisa -= nQtyAllocBad;
                    //            }
                    //          }

                    //          #endregion
                    //        }
                    //        if (field.Quantity > 0 && field.QuantityBad > 0)
                    //        {

                    //        }

                    //        totalDetails++;

                    //        #endregion

                    //        #endregion
                    //      }
                    //    }

                    //    if ((field != null) && field.IsNew && field.RepackNew && (!field.IsDelete) && ((field.Quantity > 0) || (field.QuantityBad > 0)))
                    //    {
                    //      #region Insert RN 1

                    //      listRnd2.Add(new LG_RND2()
                    //      {
                    //        c_batch = field.Batch,
                    //        c_gdg = gudang,
                    //        c_iteno = field.Item,
                    //        c_no = field.ReferenceID,
                    //        c_rnno = rnID,
                    //        c_type = "01",
                    //        n_bqty = field.QuantityBad,
                    //        n_gqty = field.Quantity
                    //      });

                    //      #endregion
                    //    }
                    //  }

                    //  if ((listRnd1 != null) && (listRnd1.Count > 0))
                    //  {
                    //    db.LG_RND1s.InsertAllOnSubmit(listRnd1.ToArray());
                    //    listRnd1.Clear();
                    //  }

                    //  if ((listRnd2 != null) && (listRnd2.Count > 0))
                    //  {
                    //    db.LG_RND2s.InsertAllOnSubmit(listRnd2.ToArray());
                    //    listRnd2.Clear();
                    //  }

                    //  if ((listRnd3 != null) && (listRnd3.Count > 0))
                    //  {
                    //    db.LG_RND3s.InsertAllOnSubmit(listRnd3.ToArray());
                    //    listRnd3.Clear();
                    //  }
                    //}

                    //#endregion

                    #endregion

                    dic = new Dictionary<string, string>();

                    if (totalDetails > 0)
                    {
                        //dic.Add("RNU", rnID);
                        dic.Add("RN", rnID);
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

                    #region Validasi Indra 20171120

                    if (string.IsNullOrEmpty(rnID))
                    {
                        result = "Nomor Receive dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    rnh = (from q in db.LG_RNHs
                           where q.c_rnno == rnID && q.c_gdg == gudang
                           select q).Take(1).SingleOrDefault();

                    if (rnh == null)
                    {
                        result = "Nomor Receive tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (rnh.l_delete.HasValue && rnh.l_delete.Value)
                    {
                        result = "Tidak dapat mengubah Nomor Receive yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.HasFB(db, rnID))
                    {
                        result = "Receive note tidak dapat diubah, karena sudah dibuat faktur pembelian.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, rnh.d_rndate))
                    {
                        result = "Receive note tidak dapat diubah, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
                    {
                        rnh.v_ket = structure.Fields.Keterangan;
                    }

                    #endregion

                    rnh.c_update = nipEntry;
                    rnh.d_update = DateTime.Now;

                    typeTrx = (rnh.c_type ?? "").Trim();
                    noSup = (rnh.c_from ?? "").Trim();
                    noDo = (rnh.c_dono ?? "").Trim();

                    #region Type 01 || 06

                    if (string.IsNullOrEmpty(rnh.c_type) ||
                      (rnh.c_type.Equals("01", StringComparison.OrdinalIgnoreCase) ||
                      rnh.c_type.Equals("06", StringComparison.OrdinalIgnoreCase)))
                    {
                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            listRnd5 = new List<LG_RND5>();

                            listRnd1 = (from q in db.LG_RND1s
                                        where q.c_gdg == gudang && q.c_rnno == rnID
                                        select q).ToList();

                            listRnd2 = (from q in db.LG_RND2s
                                        where q.c_gdg == gudang && q.c_rnno == rnID
                                        select q).ToList();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                field = structure.Fields.Field[nLoop];

                                if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0) && (!string.IsNullOrEmpty(field.Batch)) && (field.Batch.Length > 0))
                                {
                                    #region New

                                    rnd2 = listRnd2.Where(x => (x.c_gdg == gudang) &&
                                            (x.c_rnno == rnID) && (x.c_iteno == field.Item) &&
                                            (x.c_no == field.ReferenceID) &&
                                        //(x.c_batch == field.Batch) &&
                                            ((string.IsNullOrEmpty(x.c_batch) ? string.Empty : x.c_batch.Trim()) == field.Batch) &&
                                            (x.c_type == field.TypeTrx)).Take(1).SingleOrDefault();

                                    if (rnd2 == null)
                                    {
                                        if (field.TypeTrx.Equals("02", StringComparison.OrdinalIgnoreCase))
                                        {
                                            modifedPo = true;
                                        }
                                        else
                                        {
                                            modifedPo = Commons.PO_Modifikasi(db, gudang, field.ReferenceID, field.Item, false, field.Quantity);
                                        }

                                        if (modifedPo)
                                        {
                                            if (typeTrx.Equals("06", StringComparison.OrdinalIgnoreCase))
                                            {
                                                modifedPo = Commons.DOPharos_Modifikasi(db, noSup, noDo, field.Item, field.Batch, false, field.Quantity);
                                            }
                                            else
                                            {
                                                modifedPo = true;
                                            }

                                            if (modifedPo)
                                            {
                                                #region Detail Data

                                                rnd2 = new LG_RND2()
                                                {
                                                    c_batch = field.Batch,
                                                    c_gdg = gudang,
                                                    c_iteno = field.Item,
                                                    c_no = field.ReferenceID,
                                                    c_rnno = rnID,
                                                    c_type = field.TypeTrx,
                                                    n_bqty = 0,
                                                    n_gqty = field.Quantity,
                                                    n_floqty = structure.Fields.Floating ? field.Quantity : 0,
                                                };

                                                db.LG_RND2s.InsertOnSubmit(rnd2);

                                                wpd = (from q in db.SCMS_WPDs
                                                       where q.c_no == field.ReferenceID
                                                          && (q.l_rn == false || q.l_rn == null)
                                                       select q).Take(1).SingleOrDefault();

                                                if (wpd != null)
                                                {
                                                    wph = (from q in db.SCMS_WPHs
                                                           where q.c_nodoc == wpd.c_nodoc
                                                           select q).Take(1).SingleOrDefault();

                                                    wph.c_rn = nipEntry;
                                                    wph.d_rn = date;

                                                    wpd.l_rn = true;
                                                }

                                                //rnd1 = listRnd1.Where(x =>
                                                //  (field.Batch.Equals((x.c_batch ?? string.Empty), StringComparison.OrdinalIgnoreCase)) &&
                                                //  (x.c_iteno == field.Item)).Take(1).SingleOrDefault();

                                                rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
                                                {
                                                    return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                                });

                                                if (rnd1 == null)
                                                {
                                                    rnd1 = new LG_RND1()
                                                    {
                                                        c_batch = field.Batch,
                                                        c_gdg = gudang,
                                                        c_iteno = field.Item,
                                                        c_rnno = rnID,
                                                        n_bqty = 0,
                                                        n_bsisa = 0,
                                                        n_gqty = field.Quantity,
                                                        n_gsisa = field.Quantity
                                                    };

                                                    db.LG_RND1s.InsertOnSubmit(rnd1);

                                                    listRnd1.Add(rnd1);

                                                    Commons.CheckAndProcessBatchRN(db, field.Item, field.Batch, field.ExpiredDateFormated, nipEntry);
                                                }
                                                else
                                                {
                                                    rnd1.n_gsisa = rnd1.n_gqty += field.Quantity;
                                                }

                                                if (structure.Fields.Floating)
                                                {
                                                    rnd3 = listRnd3.Find(delegate(LG_RND3 rnd3s)
                                                    {
                                                        return field.Item.Equals((string.IsNullOrEmpty(rnd3s.c_iteno) ? string.Empty : rnd3s.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                          field.Batch.Equals((string.IsNullOrEmpty(rnd3s.c_batch) ? string.Empty : rnd3s.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                                    });

                                                    if (rnd3 == null)
                                                    {
                                                        listRnd3.Add(new LG_RND3()
                                                        {
                                                            c_batch = field.Batch,
                                                            c_gdg = gudang,
                                                            c_iteno = field.Item,
                                                            c_rnno = rnID,
                                                            n_bqty = 0,
                                                            n_gqty = field.Quantity,
                                                            //n_floqty = (structure.Fields.Floating ? field.Quantity : 0)
                                                        });

                                                    }
                                                    else
                                                    {
                                                        rnd3.n_gqty = rnd3.n_gqty += field.Quantity;
                                                    }
                                                }


                                                listRnd2.Add(rnd2);

                                                listRnd5.Add(new LG_RND5()
                                                {
                                                    c_batch = rnd2.c_batch,
                                                    c_dono = rnh.c_dono,
                                                    c_entry = nipEntry,
                                                    c_fb = null,
                                                    c_gdg = gudang,
                                                    c_iteno = rnd2.c_iteno,
                                                    c_rnno = rnID,
                                                    c_type = rnd2.c_type,
                                                    d_entry = null,
                                                    d_entry_log = date,
                                                    i_urut = null,
                                                    l_status = rnh.l_status,
                                                    n_bsisa = rnd1.n_bsisa,
                                                    n_salpri = 0,
                                                    n_qty = 0,
                                                    n_gsisa = rnd1.n_gsisa,
                                                    v_ket_del = field.Keterangan,
                                                    v_type = "01"
                                                });

                                                #region New Stock Indra 20180305FM

                                                if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                                                rnID,
                                                                                typeTrx,
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

                                                totalDetails++;

                                                #endregion
                                            }
                                            else
                                            {
                                                result = "Tidak dapat memperbarui Nomor Receive jika ada salah satu data detil nomor DO Khusus tidak berhasil diubah.";

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
                                            result = "Tidak dapat memperbarui Nomor Receive jika ada salah satu data detil nomor PO tidak berhasil diubah.";

                                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                            if (db.Transaction != null)
                                            {
                                                db.Transaction.Rollback();
                                            }

                                            goto endLogic;
                                        }
                                    }

                                    #endregion
                                }
                                else if ((field != null) && (!field.IsNew) && field.IsDelete)
                                {
                                    #region Delete

                                    rnd2 = listRnd2.Where(x =>
                                            field.ReferenceID.Equals((x.c_no ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase) &&
                                            field.Item.Equals((x.c_iteno ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase) &&
                                            field.Batch.Equals((x.c_batch ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase) &&
                                            field.TypeTrx.Equals(x.c_type, StringComparison.OrdinalIgnoreCase)).Take(1).SingleOrDefault();

                                    if (rnd2 != null)
                                    {
                                        typeDtl = (rnd2.c_type ?? string.Empty).Trim();
                                        poID = (rnd2.c_no ?? string.Empty).Trim();

                                        nQtyGood = (rnd2.n_gqty.HasValue ? rnd2.n_gqty.Value : 0);
                                        nQtyBad = (rnd2.n_bqty.HasValue ? rnd2.n_bqty.Value : 0);

                                        //rnd1 = listRnd1.Where(x =>
                                        //    field.Item.Equals((x.c_iteno ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase) &&
                                        //    field.Batch.Equals((x.c_batch ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)).Take(1).SingleOrDefault();

                                        rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
                                        {
                                            return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                        });

                                        if (rnd1 != null)
                                        {
                                            nQtyAllocGood = ((rnd1.n_gsisa.HasValue ? rnd1.n_gsisa.Value : 0) - nQtyGood);
                                            nQtyAllocBad = ((rnd1.n_bsisa.HasValue ? rnd1.n_bsisa.Value : 0) - nQtyBad);

                                            if ((nQtyAllocBad == 0) && (nQtyAllocGood == 0))
                                            {
                                                if (typeDtl.Equals("02", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    modifedPo = true;
                                                }
                                                else
                                                {
                                                    modifedPo = Commons.PO_Modifikasi(db, gudang, poID, field.Item, true, nQtyGood);
                                                }

                                                if (modifedPo)
                                                {
                                                    if (typeTrx.Equals("06", StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        modifedPo = Commons.DOPharos_Modifikasi(db, noSup, noDo, poID, field.Item, field.Batch, true, nQtyGood);
                                                    }
                                                    else
                                                    {
                                                        modifedPo = true;
                                                    }

                                                    if (modifedPo)
                                                    {
                                                        #region Detail Data

                                                        rnd1.n_gqty -= nQtyGood;
                                                        rnd1.n_bqty -= nQtyBad;

                                                        rnd1.n_gsisa -= nQtyGood;
                                                        rnd1.n_bsisa -= nQtyBad;

                                                        if (((rnd1.n_gqty.HasValue ? rnd1.n_gqty.Value : 0) == 0) &&
                                                          ((rnd1.n_bqty.HasValue ? rnd1.n_bqty.Value : 0) == 0) &&
                                                          ((rnd1.n_gsisa.HasValue ? rnd1.n_gsisa.Value : 0) == 0) &&
                                                          ((rnd1.n_bsisa.HasValue ? rnd1.n_bsisa.Value : 0) == 0))
                                                        {
                                                            db.LG_RND1s.DeleteOnSubmit(rnd1);
                                                        }

                                                        db.LG_RND2s.DeleteOnSubmit(rnd2);

                                                        listRnd5.Add(new LG_RND5()
                                                        {
                                                            c_batch = field.Batch,
                                                            c_dono = noDo,
                                                            c_entry = nipEntry,
                                                            c_fb = null,
                                                            c_gdg = gudang,
                                                            c_iteno = field.Item,
                                                            c_rnno = rnID,
                                                            c_type = typeDtl,
                                                            d_entry = null,
                                                            d_entry_log = date,
                                                            i_urut = null,
                                                            l_status = rnh.l_status,
                                                            n_bsisa = nQtyBad,
                                                            n_salpri = 0,
                                                            n_qty = 0,
                                                            n_gsisa = nQtyGood,
                                                            v_ket_del = (string.IsNullOrEmpty(field.Keterangan) ? "Human error" : field.Keterangan),
                                                            v_type = "03"
                                                        });

                                                        #region New Stock Indra 20180305FM

                                                        if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                                                   rnID,
                                                                                   "01",
                                                                                   field.Item,
                                                                                   field.Batch,
                                                                                   nQtyGood * -1,
                                                                                   nQtyBad * -1,
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

                                                        totalDetails++;

                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        result = "Tidak dapat memperbarui Nomor Receive jika ada salah satu data detil nomor DO Khusus tidak berhasil diubah.";

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
                                                    result = "Tidak dapat memperbarui Nomor Receive jika ada salah satu data detil nomor PO tidak berhasil diubah.";

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

                                    #endregion
                                }
                            }

                            if (listRnd5.Count > 0)
                            {
                                db.LG_RND5s.InsertAllOnSubmit(listRnd5.ToArray());
                                listRnd5.Clear();
                            }

                            listRnd1.Clear();
                            listRnd2.Clear();
                        }
                    }

                    #endregion

                    #region Type 02

                    if (string.IsNullOrEmpty(rnh.c_type) ||
                      rnh.c_type.Equals("02", StringComparison.OrdinalIgnoreCase))
                    {

                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            listRnd5 = new List<LG_RND5>();

                            listRnd1 = (from q in db.LG_RND1s
                                        where q.c_gdg == gudang && q.c_rnno == rnID
                                        select q).ToList();

                            //listRnd2 = (from q in db.LG_RND2s
                            //            where q.c_gdg == gudang && q.c_rnno == rnID
                            //            select q).ToList();


                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                field = structure.Fields.Field[nLoop];

                                listRnd3 = (from q in db.LG_RND3s
                                            where q.c_gdg == gudang && q.c_rnno == rnID
                                            && q.c_iteno == field.Item && q.c_batch == field.Batch
                                            && q.c_no == field.ReferenceID
                                            select q).ToList();

                                if ((field != null) && field.IsNew && (!field.IsDelete) && ((field.Quantity > 0) || (field.QuantityBad > 0)) && (!string.IsNullOrEmpty(field.Batch)) && (field.Batch.Length > 0))
                                {
                                    #region New

                                    listRSD2 = (from q in db.LG_RSHes
                                                join q1 in db.LG_RSD2s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                                                where (q.c_gdg == gudang) && q.c_rsno == field.ReferenceID
                                                && (q.c_type != "03") && q1.c_batch == field.Batch
                                                && ((q1.n_gsisa > 0) || (q1.n_bsisa > 0))
                                                  && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                                select q1).Distinct().ToList();

                                    if ((listRSD2 != null) && (listRSD2.Count > 0))
                                    {
                                        for (nLoopC = 0; nLoopC < listRSD2.Count; nLoopC++)
                                        {
                                            rsd2 = listRSD2.Find(delegate(LG_RSD2 rsd)
                                            {
                                                return field.ReferenceID.Equals((string.IsNullOrEmpty(rsd.c_rsno) ? string.Empty : rsd.c_rsno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                  field.Item.Equals((string.IsNullOrEmpty(rsd.c_iteno) ? string.Empty : rsd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                  field.Batch.Equals((string.IsNullOrEmpty(rsd.c_batch) ? string.Empty : rsd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                  listRSD2[nLoopC].c_rnno.Equals((string.IsNullOrEmpty(rsd.c_rnno) ? string.Empty : rsd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                  (((rsd.n_gsisa.HasValue ? rsd.n_gsisa.Value : 0) > 0) || ((rsd.n_bsisa.HasValue ? rsd.n_bsisa.Value : 0) > 0));
                                            });

                                            if (rsd2 != null)
                                            {
                                                #region Detail Data

                                                rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
                                                {
                                                    return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      rnID.Equals((string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
                                                });

                                                #region New Stock Indra 20180305FM

                                                if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                                                rnID,
                                                                                typeTrx,
                                                                                field.Item,
                                                                                field.Batch,
                                                                                rsd2.n_gsisa.Value,
                                                                                rsd2.n_bsisa.Value,
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

                                                if (rnd1 == null)
                                                {
                                                    rnd1 = new LG_RND1()
                                                    {
                                                        c_batch = field.Batch,
                                                        c_gdg = gudang,
                                                        c_iteno = field.Item,
                                                        c_rnno = rnID,
                                                        n_bqty = rsd2.n_bsisa,
                                                        n_bsisa = rsd2.n_bsisa,
                                                        n_gqty = rsd2.n_gsisa,
                                                        n_gsisa = rsd2.n_gsisa
                                                    };

                                                    // Check Batch
                                                    Commons.CheckAndProcessBatchRN(db, field.Item, field.Batch, field.ExpiredDateFormated, nipEntry);

                                                    db.LG_RND1s.InsertOnSubmit(rnd1);

                                                    listRnd1.Add(rnd1);
                                                }
                                                else
                                                {
                                                    rnd1.n_bsisa = rnd1.n_bqty += rsd2.n_bsisa;
                                                    rnd1.n_gsisa = rnd1.n_gqty += rsd2.n_gsisa;
                                                }

                                                rnd3 = listRnd3.Find(delegate(LG_RND3 rnd)
                                                {
                                                    return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.ReferenceID.Equals((string.IsNullOrEmpty(rnd.c_no) ? string.Empty : rnd.c_no.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.TypeTrx.Equals((string.IsNullOrEmpty(rnd.c_type) ? string.Empty : rnd.c_type.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      (string.IsNullOrEmpty(rsd2.c_rnno) ? string.Empty : rsd2.c_rnno.Trim()).Equals((string.IsNullOrEmpty(rnd.c_rn) ? string.Empty : rnd.c_rn.Trim()), StringComparison.OrdinalIgnoreCase);
                                                });

                                                if (rnd3 == null)
                                                {
                                                    rnd3 = new LG_RND3()
                                                    {
                                                        c_batch = field.Batch,
                                                        c_gdg = gudang,
                                                        c_iteno = field.Item,
                                                        c_no = rsd2.c_rsno,
                                                        c_rn = rsd2.c_rnno,
                                                        c_rnno = rnID,
                                                        c_type = field.TypeTrx,
                                                        n_bqty = rsd2.n_bsisa,
                                                        n_gqty = rsd2.n_gsisa
                                                    };

                                                    db.LG_RND3s.InsertOnSubmit(rnd3);
                                                    listRnd3.Add(rnd3);
                                                }
                                                else
                                                {
                                                    rnd3.n_gqty += rsd2.n_gsisa;
                                                    rnd3.n_bqty += rsd2.n_bsisa;
                                                }

                                                rsd2.n_bsisa -= rsd2.n_bsisa;
                                                rsd2.n_gsisa -= rsd2.n_gsisa;

                                                totalDetails++;

                                                #endregion
                                            }
                                        }
                                    }

                                    #region Old Coded

                                    //rnd2 = listRnd2.Where(x => (x.c_gdg == gudang) &&
                                    //        (x.c_rnno == rnID) && (x.c_iteno == field.Item) &&
                                    //        (x.c_no == field.ReferenceID) &&
                                    //  //(x.c_batch == field.Batch) &&
                                    //        ((string.IsNullOrEmpty(x.c_batch) ? string.Empty : x.c_batch.Trim()) == field.Batch) &&
                                    //        (x.c_type == field.TypeTrx)).Take(1).SingleOrDefault();

                                    //if (rnd2 == null)
                                    //{
                                    //  rsd2 = new LG_RSD2();
                                    //  listRnd3 = new List<LG_RND3>();

                                    //  rsd2 = (from q in db.LG_RSD2s
                                    //          join q1 in db.LG_RSHes on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                                    //          where q1.c_type == "02" && q1.c_rsno == field.ReferenceID
                                    //          && q.c_gdg == gudang && q.c_iteno == field.Item && q.c_batch == field.Batch
                                    //          select q).Take(1).SingleOrDefault();

                                    //  rsd2.n_bsisa -= field.QuantityBad;
                                    //  rsd2.n_gsisa -= field.Quantity;

                                    //  #region Detail Data

                                    //  rnd2 = new LG_RND2()
                                    //  {
                                    //    c_batch = field.Batch,
                                    //    c_gdg = gudang,
                                    //    c_iteno = field.Item,
                                    //    c_no = field.ReferenceID,
                                    //    c_rnno = rnID,
                                    //    c_type = field.TypeTrx,
                                    //    n_bqty = field.QuantityBad,
                                    //    n_gqty = field.Quantity
                                    //  };

                                    //  db.LG_RND2s.InsertOnSubmit(rnd2);

                                    //  //rnd1 = listRnd1.Where(x => //(x.c_batch == field.Batch) &&
                                    //  //  ((string.IsNullOrEmpty(x.c_batch) ? string.Empty : x.c_batch.Trim()) == field.Batch) &&
                                    //  //  (x.c_gdg == gudang) && (x.c_iteno == field.Item) &&
                                    //  //  (x.c_rnno == rnID)).Take(1).SingleOrDefault();

                                    //  rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
                                    //  {
                                    //    return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                    //      field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                    //  });

                                    //  if (rnd1 == null)
                                    //  {
                                    //    rnd1 = new LG_RND1()
                                    //    {
                                    //      c_batch = field.Batch,
                                    //      c_gdg = gudang,
                                    //      c_iteno = field.Item,
                                    //      c_rnno = rnID,
                                    //      n_bqty = field.QuantityBad,
                                    //      n_bsisa = field.QuantityBad,
                                    //      n_gqty = field.Quantity,
                                    //      n_gsisa = field.Quantity
                                    //    };

                                    //    db.LG_RND1s.InsertOnSubmit(rnd1);

                                    //    listRnd1.Add(rnd1);

                                    //    Commons.CheckAndProcessBatch(db, field.Item, field.Batch, field.ExpiredDateFormated, nipEntry);
                                    //  }
                                    //  else
                                    //  {
                                    //    rnd1.n_gsisa = rnd1.n_gqty += field.Quantity;
                                    //  }

                                    //  listRnd2.Add(rnd2);

                                    //  listRnd5.Add(new LG_RND5()
                                    //  {
                                    //    c_batch = rnd2.c_batch,
                                    //    c_dono = rnh.c_dono,
                                    //    c_entry = nipEntry,
                                    //    c_fb = null,
                                    //    c_gdg = gudang,
                                    //    c_iteno = rnd2.c_iteno,
                                    //    c_rnno = rnID,
                                    //    c_type = rnd2.c_type,
                                    //    d_entry = null,
                                    //    d_entry_log = date,
                                    //    i_urut = null,
                                    //    l_status = rnh.l_status,
                                    //    n_bsisa = rnd1.n_bsisa,
                                    //    n_salpri = 0,
                                    //    n_qty = 0,
                                    //    n_gsisa = rnd1.n_gsisa,
                                    //    v_ket_del = field.Keterangan,
                                    //    v_type = "01"
                                    //  });

                                    //  rnd3 = new LG_RND3()
                                    //  {
                                    //    c_batch = field.Batch,
                                    //    c_gdg = gudang,
                                    //    c_iteno = field.Item,
                                    //    c_no = rsd2.c_rsno,
                                    //    c_rn = rsd2.c_rnno,
                                    //    c_rnno = rnID,
                                    //    c_type = field.TypeTrx,
                                    //    n_bqty = field.QuantityBad,
                                    //    n_gqty = field.Quantity
                                    //  };

                                    //  db.LG_RND3s.InsertOnSubmit(rnd3);

                                    //  totalDetails++;

                                    //  #endregion

                                    //}

                                    #endregion

                                    #endregion
                                }
                                else if ((field != null) && (!field.IsNew) && field.IsDelete)
                                {
                                    #region Delete

                                    rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
                                    {
                                        return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                          field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                          rnID.Equals((string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                          field.Quantity.Equals(rnd.n_gsisa.HasValue ? rnd.n_gsisa.Value : 0) &&
                                          field.QuantityBad.Equals(rnd.n_bsisa.HasValue ? rnd.n_bsisa.Value : 0);
                                    });

                                    if (rnd1 != null)
                                    {
                                        if (listRnd3 != null && listRnd3.Count > 0)
                                        {
                                            for (nLoopC = 0; nLoopC < listRnd3.Count; nLoopC++)
                                            {
                                                rnd3 = listRnd3[nLoopC];

                                                rsd2 = (from q in db.LG_RSD2s
                                                        join q1 in db.LG_RSHes on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                                                        where q1.c_type != "03" && q1.c_rsno == field.ReferenceID
                                                        && q.c_gdg == gudang && q.c_iteno == field.Item
                                                        && q.c_batch == field.Batch && q.c_rnno == rnd3.c_rn
                                                        select q).Take(1).SingleOrDefault();

                                                if (rsd2 != null)
                                                {
                                                    rsd2.n_bsisa += rnd3.n_bqty;
                                                    rsd2.n_gsisa += rnd3.n_gqty;

                                                    listRnd5.Add(new LG_RND5()
                                                    {
                                                        c_batch = rnd3.c_batch,
                                                        c_dono = rnh.c_dono,
                                                        c_entry = nipEntry,
                                                        c_fb = null,
                                                        c_gdg = gudang,
                                                        c_iteno = rnd3.c_iteno,
                                                        c_rnno = rnID,
                                                        c_type = rnd3.c_type,
                                                        d_entry = null,
                                                        d_entry_log = date,
                                                        i_urut = null,
                                                        l_status = rnh.l_status,
                                                        n_bsisa = rnd3.n_bqty,
                                                        n_salpri = 0,
                                                        n_qty = 0,
                                                        n_gsisa = rnd3.n_gqty,
                                                        v_ket_del = field.Keterangan,
                                                        c_no = rnd3.c_no,
                                                        c_rn = rnd3.c_rn,
                                                        v_type = "03"
                                                    });

                                                    db.LG_RND3s.DeleteOnSubmit(rnd3);

                                                    #region New Stock Indra 20180305FM

                                                    if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                                                    rnID,
                                                                                    typeTrx,
                                                                                    field.Item,
                                                                                    field.Batch,
                                                                                    rnd3.n_gqty.Value * -1,
                                                                                    rnd3.n_bqty.Value * -1,
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

                                            }
                                        }

                                        db.LG_RND1s.DeleteOnSubmit(rnd1);
                                    }

                                    #region Old Code

                                    //rnd2 = listRnd2.Where(x => (x.c_gdg == gudang) &&
                                    //        (x.c_rnno == rnID) && (x.c_iteno == field.Item) &&
                                    //        (x.c_no == field.ReferenceID) && //&& (x.c_batch == field.Batch) &&
                                    //        ((string.IsNullOrEmpty(x.c_batch) ? string.Empty : x.c_batch.Trim()) == field.Batch) &&
                                    //        (x.c_type == field.TypeTrx)).Take(1).SingleOrDefault();

                                    //if (rnd2 != null)
                                    //{
                                    //  nQtyGood = (rnd2.n_gqty.HasValue ? rnd2.n_gqty.Value : 0);
                                    //  nQtyBad = (rnd2.n_bqty.HasValue ? rnd2.n_bqty.Value : 0);

                                    //  //rnd1 = listRnd1.Where(x => (x.c_gdg == gudang) &&
                                    //  //      (x.c_rnno == rnID) && (x.c_iteno == field.Item) &&
                                    //  //  //(x.c_batch == field.Batch)
                                    //  //      ((string.IsNullOrEmpty(x.c_batch) ? string.Empty : x.c_batch.Trim()) == field.Batch)).Take(1).SingleOrDefault();

                                    //  rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
                                    //  {
                                    //    return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                    //      field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                    //  });

                                    //  rnd3 = listRnd3.Where(x => (x.c_gdg == gudang) &&
                                    //        (x.c_rnno == rnID) && (x.c_iteno == field.Item) &&
                                    //        ((string.IsNullOrEmpty(x.c_batch) ? string.Empty : x.c_batch.Trim()) == field.Batch)).Take(1).SingleOrDefault();

                                    //  if (rnd1 != null)
                                    //  {
                                    //    nQtyAllocGood = ((rnd1.n_gsisa.HasValue ? rnd1.n_gsisa.Value : 0) - nQtyGood);
                                    //    nQtyAllocBad = ((rnd1.n_bsisa.HasValue ? rnd1.n_bsisa.Value : 0) - nQtyBad);

                                    //    if ((nQtyAllocBad == 0) && (nQtyAllocGood == 0))
                                    //    {
                                    //      rsd2 = new LG_RSD2();
                                    //      listRnd3 = new List<LG_RND3>();

                                    //      rsd2 = (from q in db.LG_RSD2s
                                    //              join q1 in db.LG_RSHes on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                                    //              where q1.c_type == "02" && q1.c_rsno == field.ReferenceID
                                    //              && q.c_gdg == gudang && q.c_iteno == field.Item && q.c_batch == field.Batch
                                    //              select q).Take(1).SingleOrDefault();

                                    //      rsd2.n_bsisa += field.QuantityBad;
                                    //      rsd2.n_gsisa += field.Quantity;

                                    //      #region Detail Data

                                    //      rnd1.n_gqty -= nQtyGood;
                                    //      rnd1.n_bqty -= nQtyBad;

                                    //      rnd1.n_gsisa -= nQtyGood;
                                    //      rnd1.n_bsisa -= nQtyBad;

                                    //      if (((rnd1.n_gqty.HasValue ? rnd1.n_gqty.Value : 0) == 0) &&
                                    //        ((rnd1.n_bqty.HasValue ? rnd1.n_bqty.Value : 0) == 0) &&
                                    //        ((rnd1.n_gsisa.HasValue ? rnd1.n_gsisa.Value : 0) == 0) &&
                                    //        ((rnd1.n_bsisa.HasValue ? rnd1.n_bsisa.Value : 0) == 0))
                                    //      {
                                    //        db.LG_RND1s.DeleteOnSubmit(rnd1);
                                    //      }

                                    //      db.LG_RND2s.DeleteOnSubmit(rnd2);

                                    //      db.LG_RND3s.DeleteOnSubmit(rnd3);

                                    //      listRnd5.Add(new LG_RND5()
                                    //      {
                                    //        c_batch = rnd2.c_batch,
                                    //        c_dono = rnh.c_dono,
                                    //        c_entry = nipEntry,
                                    //        c_fb = null,
                                    //        c_gdg = gudang,
                                    //        c_iteno = rnd2.c_iteno,
                                    //        c_rnno = rnID,
                                    //        c_type = rnd2.c_type,
                                    //        d_entry = null,
                                    //        d_entry_log = date,
                                    //        i_urut = null,
                                    //        l_status = rnh.l_status,
                                    //        n_bsisa = rnd1.n_bsisa,
                                    //        n_salpri = 0,
                                    //        n_qty = 0,
                                    //        n_gsisa = rnd1.n_gsisa,
                                    //        v_ket_del = field.Keterangan,
                                    //        v_type = "03"
                                    //      });



                                    //      totalDetails++;

                                    //      #endregion
                                    //    }
                                    //  }


                                    //}
                                    #endregion

                                    #endregion
                                }
                            }

                            if (listRnd5.Count > 0)
                            {
                                db.LG_RND5s.InsertAllOnSubmit(listRnd5.ToArray());
                                listRnd5.Clear();
                            }
                        }
                    }

                    #endregion

                    #region Type 03

                    if (string.IsNullOrEmpty(rnh.c_type) ||
                      (rnh.c_type.Equals("03", StringComparison.OrdinalIgnoreCase)))
                    {
                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            listRnd5 = new List<LG_RND5>();

                            listRnd1 = (from q in db.LG_RND1s
                                        where q.c_gdg == gudang && q.c_rnno == rnID
                                        select q).ToList();

                            listRnd2 = (from q in db.LG_RND2s
                                        where q.c_gdg == gudang && q.c_rnno == rnID
                                        select q).ToList();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                field = structure.Fields.Field[nLoop];

                                if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Quantity > 0))
                                {
                                    claimaccd = new LG_ClaimAccD();

                                    #region New

                                    rnd2 = listRnd2.Where(x => (x.c_gdg == gudang) &&
                                            (x.c_rnno == rnID) && (x.c_iteno == field.Item) &&
                                            (x.c_no == field.ReferenceID) &&
                                        //(x.c_batch == field.Batch) &&
                                            ((string.IsNullOrEmpty(x.c_batch) ? string.Empty : x.c_batch.Trim()) == field.Batch) &&
                                            (x.c_type == field.TypeTrx)).Take(1).SingleOrDefault();

                                    if (rnd2 == null)
                                    {

                                        claimaccd = (from q in db.LG_ClaimAccDs
                                                     where q.c_claimaccno == field.ReferenceID
                                                     && q.c_iteno == field.Item
                                                     select q).Take(1).SingleOrDefault();

                                        claimaccd.n_sisa -= field.Quantity;

                                        #region Detail Data

                                        rnd2 = new LG_RND2()
                                        {
                                            c_batch = field.Batch,
                                            c_gdg = gudang,
                                            c_iteno = field.Item,
                                            c_no = field.ReferenceID,
                                            c_rnno = rnID,
                                            c_type = field.TypeTrx,
                                            n_bqty = 0,
                                            n_gqty = field.Quantity
                                        };

                                        db.LG_RND2s.InsertOnSubmit(rnd2);

                                        //rnd1 = listRnd1.Where(x => //(x.c_batch == field.Batch) &&
                                        //  ((string.IsNullOrEmpty(x.c_batch) ? string.Empty : x.c_batch.Trim()) == field.Batch) &&
                                        //  (x.c_gdg == gudang) && (x.c_iteno == field.Item) &&
                                        //  (x.c_rnno == rnID)).Take(1).SingleOrDefault();

                                        rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
                                        {
                                            return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                        });

                                        if (rnd1 == null)
                                        {
                                            rnd1 = new LG_RND1()
                                            {
                                                c_batch = field.Batch,
                                                c_gdg = gudang,
                                                c_iteno = field.Item,
                                                c_rnno = rnID,
                                                n_bqty = 0,
                                                n_bsisa = 0,
                                                n_gqty = field.Quantity,
                                                n_gsisa = field.Quantity
                                            };

                                            db.LG_RND1s.InsertOnSubmit(rnd1);

                                            listRnd1.Add(rnd1);

                                            Commons.CheckAndProcessBatchRN(db, field.Item, field.Batch, field.ExpiredDateFormated, nipEntry);
                                        }
                                        else
                                        {
                                            rnd1.n_gsisa = rnd1.n_gqty += field.Quantity;
                                        }

                                        listRnd2.Add(rnd2);

                                        listRnd5.Add(new LG_RND5()
                                        {
                                            c_batch = rnd2.c_batch,
                                            c_dono = rnh.c_dono,
                                            c_entry = nipEntry,
                                            c_fb = null,
                                            c_gdg = gudang,
                                            c_iteno = rnd2.c_iteno,
                                            c_rnno = rnID,
                                            c_type = rnd2.c_type,
                                            d_entry = null,
                                            d_entry_log = date,
                                            i_urut = null,
                                            l_status = rnh.l_status,
                                            n_bsisa = rnd1.n_bsisa,
                                            n_salpri = 0,
                                            n_qty = 0,
                                            n_gsisa = rnd1.n_gsisa,
                                            v_ket_del = field.Keterangan,
                                            v_type = "01"
                                        });

                                        #region New Stock Indra 20180305FM

                                        if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                                        rnID,
                                                                        typeTrx,
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

                                        totalDetails++;

                                        #endregion

                                    }
                                    #endregion
                                }
                                else if ((field != null) && (!field.IsNew) && field.IsDelete)
                                {
                                    #region Delete

                                    rnd2 = listRnd2.Where(x => (x.c_gdg == gudang) &&
                                            (x.c_rnno == rnID) && (x.c_iteno == field.Item) &&
                                            (x.c_no == field.ReferenceID) && //&& (x.c_batch == field.Batch) &&
                                            ((string.IsNullOrEmpty(x.c_batch) ? string.Empty : x.c_batch.Trim()) == field.Batch) &&
                                            (x.c_type == field.TypeTrx)).Take(1).SingleOrDefault();

                                    if (rnd2 != null)
                                    {
                                        nQtyGood = (rnd2.n_gqty.HasValue ? rnd2.n_gqty.Value : 0);
                                        nQtyBad = (rnd2.n_bqty.HasValue ? rnd2.n_bqty.Value : 0);

                                        //rnd1 = listRnd1.Where(x => (x.c_gdg == gudang) &&
                                        //      (x.c_rnno == rnID) && (x.c_iteno == field.Item) &&
                                        //  //(x.c_batch == field.Batch)
                                        //      ((string.IsNullOrEmpty(x.c_batch) ? string.Empty : x.c_batch.Trim()) == field.Batch)).Take(1).SingleOrDefault();

                                        rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
                                        {
                                            return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                        });

                                        if (rnd1 != null && (rnd1.n_gqty == rnd1.n_gsisa) && (rnd1.n_bqty == rnd1.n_bsisa))
                                        {
                                            nQtyAllocGood = ((rnd2.n_gqty.HasValue ? rnd1.n_gqty.Value : 0) - nQtyGood);
                                            nQtyAllocBad = ((rnd2.n_bqty.HasValue ? rnd1.n_bqty.Value : 0) - nQtyBad);

                                            if ((nQtyAllocBad == 0) && (nQtyAllocGood == 0))
                                            {
                                                claimaccd = new LG_ClaimAccD();

                                                claimaccd = (from q in db.LG_ClaimAccDs
                                                             where q.c_claimaccno == field.ReferenceID
                                                             && q.c_iteno == field.Item
                                                             select q).Take(1).SingleOrDefault();

                                                claimaccd.n_sisa += nQtyGood;

                                                #region Detail Data

                                                rnd1.n_gqty -= nQtyGood;
                                                rnd1.n_bqty -= nQtyBad;

                                                rnd1.n_gsisa -= nQtyGood;
                                                rnd1.n_bsisa -= nQtyBad;

                                                if (((rnd1.n_gqty.HasValue ? rnd1.n_gqty.Value : 0) == 0) &&
                                                  ((rnd1.n_bqty.HasValue ? rnd1.n_bqty.Value : 0) == 0) &&
                                                  ((rnd1.n_gsisa.HasValue ? rnd1.n_gsisa.Value : 0) == 0) &&
                                                  ((rnd1.n_bsisa.HasValue ? rnd1.n_bsisa.Value : 0) == 0))
                                                {
                                                    db.LG_RND1s.DeleteOnSubmit(rnd1);
                                                }

                                                db.LG_RND2s.DeleteOnSubmit(rnd2);

                                                listRnd5.Add(new LG_RND5()
                                                {
                                                    c_batch = rnd2.c_batch,
                                                    c_dono = rnh.c_dono,
                                                    c_entry = nipEntry,
                                                    c_fb = null,
                                                    c_gdg = gudang,
                                                    c_iteno = rnd2.c_iteno,
                                                    c_rnno = rnID,
                                                    c_type = rnd2.c_type,
                                                    d_entry = null,
                                                    d_entry_log = date,
                                                    i_urut = null,
                                                    l_status = rnh.l_status,
                                                    n_bsisa = rnd1.n_bsisa,
                                                    n_salpri = 0,
                                                    n_qty = 0,
                                                    n_gsisa = rnd1.n_gsisa,
                                                    v_ket_del = field.Keterangan,
                                                    v_type = "03"
                                                });

                                                #region New Stock Indra 20180305FM                                                

                                                if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                                                rnID,
                                                                                typeTrx,
                                                                                rnd2.c_iteno,
                                                                                rnd2.c_batch,
                                                                                nQtyGood * -1,
                                                                                nQtyBad * -1,
                                                                                "KOSONG",
                                                                                "02",
                                                                                "01",
                                                                                nipEntry,
                                                                                "01")) == 0)
                                                {
                                                    result = "Terdapat Kesalahan pada Item " + rnd2.c_iteno + " dengan Batch " + rnd2.c_batch + ". Harap Hubungi MIS.";

                                                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                                    if (db.Transaction != null)
                                                    {
                                                        db.Transaction.Rollback();
                                                    }

                                                    goto endLogic;
                                                }
												
												#endregion

                                                totalDetails++;

                                                #endregion

                                            }
                                        }
                                    }

                                    #endregion
                                }
                            }

                            if (listRnd5.Count > 0)
                            {
                                db.LG_RND5s.InsertAllOnSubmit(listRnd5.ToArray());
                                listRnd5.Clear();
                            }
                        }
                    }

                    #endregion

                    #region Type 04

                    if (string.IsNullOrEmpty(rnh.c_type) ||
                      (rnh.c_type.Equals("04", StringComparison.OrdinalIgnoreCase)))
                    {
                        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                        {
                            listRnd1 = new List<LG_RND1>();
                            listRnd2 = new List<LG_RND2>();
                            listRnd3 = new List<LG_RND3>();
                            listRnd5 = new List<LG_RND5>();

                            lstTemp = structure.Fields.Field.Where(t => (!string.IsNullOrEmpty(t.ReferenceID))).GroupBy(x => x.ReferenceID).Select(y => y.Key).ToList();

                            typeList = new string[] { "02", "03" };

                            listRSD2 = (from q in db.LG_RSHes
                                        join q1 in db.LG_RSD2s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                                        where (q.c_gdg == gudang) && lstTemp.Contains(q.c_rsno)
                                        && typeList.Contains(q.c_type)
                                        && ((q1.n_gsisa > 0) || (q1.n_bsisa > 0))
                                          && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                        select q1).Distinct().ToList();

                            //listRnd1 = (from q in db.LG_RND1s
                            //            where q.c_gdg == gudang && q.c_rnno == rnID
                            //            select q).ToList();

                            //listRnd2 = (from q in db.LG_RND2s
                            //            where q.c_gdg == gudang && q.c_rnno == rnID
                            //            select q).ToList();

                            //listRnd3 = (from q in db.LG_RND3s
                            //            where q.c_gdg == gudang && q.c_rnno == rnID
                            //            select q).ToList();

                            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            {
                                field = structure.Fields.Field[nLoop];

                                if ((field != null) && field.IsNew && (!field.IsDelete) && ((field.Quantity > 0) || (field.QuantityBad > 0)) && (!string.IsNullOrEmpty(field.Batch)) && (field.Batch.Length > 0))
                                {
                                    if (field.RepackNew)
                                    {
                                        #region New Items

                                        #region Detail Data

                                        //rnd2 = listRnd2.Find(delegate(LG_RND2 rnd)
                                        //{
                                        //  return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                        //    field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                        //});

                                        //if (rnd2 == null)
                                        //{
                                        //  listRnd2.Add(new LG_RND2()
                                        //  {
                                        //    c_gdg = gudang,
                                        //    c_rnno = rnID,
                                        //    c_batch = field.Batch,
                                        //    c_iteno = field.Item,
                                        //    c_no = "RSXXXXXXXX",
                                        //    c_type = field.TypeTrx,
                                        //    n_bqty = 0,
                                        //    n_gqty = field.Quantity
                                        //  });
                                        //}
                                        //else
                                        //{
                                        //  rnd2.n_gqty += field.Quantity;
                                        //}

                                        //rnd1 = listRnd1.Where(x => //(x.c_batch == field.Batch) &&
                                        //  ((string.IsNullOrEmpty(x.c_batch) ? string.Empty : x.c_batch.Trim()) == field.Batch) &&
                                        //  (x.c_gdg == gudang) && (x.c_iteno == field.Item) &&
                                        //  (x.c_rnno == rnID)).Take(1).SingleOrDefault();

                                        rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
                                        {
                                            return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                        });

                                        if (rnd1 == null)
                                        {
                                            rnd1 = new LG_RND1()
                                            {
                                                c_batch = field.Batch,
                                                c_gdg = gudang,
                                                c_iteno = field.Item,
                                                c_rnno = rnID,
                                                n_bqty = 0,
                                                n_bsisa = 0,
                                                n_gqty = field.Quantity,
                                                n_gsisa = field.Quantity
                                            };

                                            db.LG_RND1s.InsertOnSubmit(rnd1);

                                            listRnd1.Add(rnd1);

                                            // Check Batch
                                            Commons.CheckAndProcessBatchRN(db, field.Item, field.Batch, field.ExpiredDateFormated, nipEntry);
                                        }
                                        else
                                        {
                                            //rnd1.n_bsisa = rnd1.n_bqty += field.QuantityBad;
                                            rnd1.n_gsisa = rnd1.n_gqty += field.Quantity;
                                        }

                                        #region New Stock Indra 20180305FM                                        

                                        if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                                   rnID,
                                                                   typeTrx,
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

                                        totalDetails++;

                                        #endregion

                                        #endregion
                                    }
                                    else
                                    {
                                        #region Original

                                        nQtyAlloc = (field.QuantityBad + field.Quantity);

                                        //listRSD2Copy = listRSD2.FindAll(delegate(LG_RSD2 rsd)
                                        //{
                                        //  return field.ReferenceID.Equals((string.IsNullOrEmpty(rsd.c_rsno) ? string.Empty : rsd.c_rsno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                        //    field.Item.Equals((string.IsNullOrEmpty(rsd.c_iteno) ? string.Empty : rsd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                        //    field.Batch.Equals((string.IsNullOrEmpty(rsd.c_batch) ? string.Empty : rsd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                        //    (((rsd.n_gsisa.HasValue ? rsd.n_gsisa.Value : 0) > 0) || ((rsd.n_bsisa.HasValue ? rsd.n_bsisa.Value : 0) > 0));
                                        //});

                                        listRSD2Copy = listRSD2.FindAll(delegate(LG_RSD2 rsd)
                                        {
                                            return field.ReferenceID.Equals((string.IsNullOrEmpty(rsd.c_rsno) ? string.Empty : rsd.c_rsno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              field.Item.Equals((string.IsNullOrEmpty(rsd.c_iteno) ? string.Empty : rsd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              field.Batch.Equals((string.IsNullOrEmpty(rsd.c_batch) ? string.Empty : rsd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              (((rsd.n_gsisa.HasValue ? rsd.n_gsisa.Value : 0) > 0) || ((rsd.n_bsisa.HasValue ? rsd.n_bsisa.Value : 0) > 0));
                                        });

                                        for (nLoopC = 0; nLoopC < listRSD2Copy.Count; nLoopC++)
                                        {
                                            rsd2 = listRSD2Copy[nLoopC];

                                            if (rsd2 != null)
                                            {
                                                nQtyAllocBad = (rsd2.n_bsisa.HasValue ? rsd2.n_bsisa.Value : 0);
                                                nQtyAllocGood = (rsd2.n_gsisa.HasValue ? rsd2.n_gsisa.Value : 0);

                                                nQtyBad = ((nQtyAllocBad > 0) && (nQtyAlloc > 0) ?
                                                  (nQtyAllocBad > nQtyAlloc ? nQtyAlloc : nQtyAllocBad) : 0);
                                                rsd2.n_bsisa -= nQtyBad;
                                                nQtyAlloc -= nQtyAllocBad;
                                                nQtyBad -= nQtyAllocBad;

                                                nQtyGood = ((nQtyAllocGood > 0) && (nQtyAlloc > 0) ?
                                                  (nQtyAllocGood > nQtyAlloc ? nQtyAlloc : nQtyAllocGood) : 0);
                                                rsd2.n_gsisa -= nQtyGood;
                                                nQtyAlloc -= nQtyAllocGood;
                                                nQtyGood -= nQtyAllocGood;

                                                //nQtyAllocBad = (nQtyBad + nQtyGood);

                                                #region Detail Data

                                                rnd2 = listRnd2.Find(delegate(LG_RND2 rnd)
                                                {
                                                    return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.ReferenceID.Equals((string.IsNullOrEmpty(rnd.c_no) ? string.Empty : rnd.c_no.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.TypeTrx.Equals((string.IsNullOrEmpty(rnd.c_type) ? string.Empty : rnd.c_type.Trim()), StringComparison.OrdinalIgnoreCase);
                                                });

                                                if (rnd2 == null)
                                                {
                                                    listRnd2.Add(new LG_RND2()
                                                    {
                                                        c_gdg = gudang,
                                                        c_rnno = rnID,
                                                        c_batch = field.Batch,
                                                        c_iteno = field.Item,
                                                        c_no = field.ReferenceID,
                                                        c_type = field.TypeTrx,
                                                        n_gqty = nQtyAllocGood,
                                                        n_bqty = nQtyAllocBad
                                                    });
                                                }
                                                else
                                                {
                                                    rnd2.n_gqty += nQtyAllocGood;
                                                    rnd2.n_bqty += nQtyAllocBad;
                                                }

                                                //rnd1 = listRnd1.Find(delegate(LG_RND1 rnd)
                                                //{
                                                //  return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                //    field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                                //});

                                                //listRnd3.Add(new LG_RND3()
                                                //{
                                                //  c_batch = field.Batch,
                                                //  c_gdg = gudang,
                                                //  c_iteno = field.Item,
                                                //  c_no = rsd2.c_rsno,
                                                //  c_rn = rsd2.c_rnno,
                                                //  c_rnno = rnID,
                                                //  c_type = field.TypeTrx,
                                                //  n_bqty = field.QuantityBad,
                                                //  n_gqty = field.Quantity
                                                //});

                                                rnd3 = listRnd3.Find(delegate(LG_RND3 rnd)
                                                {
                                                    return field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.ReferenceID.Equals((string.IsNullOrEmpty(rnd.c_no) ? string.Empty : rnd.c_no.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.TypeTrx.Equals((string.IsNullOrEmpty(rnd.c_type) ? string.Empty : rnd.c_type.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      (string.IsNullOrEmpty(rsd2.c_rnno) ? string.Empty : rsd2.c_rnno.Trim()).Equals((string.IsNullOrEmpty(rnd.c_rn) ? string.Empty : rnd.c_rn.Trim()), StringComparison.OrdinalIgnoreCase);
                                                });

                                                if (rnd3 == null)
                                                {
                                                    listRnd3.Add(new LG_RND3()
                                                    {
                                                        c_batch = field.Batch,
                                                        c_gdg = gudang,
                                                        c_iteno = field.Item,
                                                        c_no = rsd2.c_rsno,
                                                        c_rn = rsd2.c_rnno,
                                                        c_rnno = rnID,
                                                        c_type = field.TypeTrx,
                                                        n_gqty = nQtyAllocGood,
                                                        n_bqty = nQtyAllocBad
                                                    });
                                                }
                                                else
                                                {
                                                    rnd3.n_gqty += nQtyAllocGood;
                                                    rnd3.n_bqty += nQtyAllocBad;
                                                }

                                                #region New Stock Indra 20180305FM

                                                if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                                                rnID,
                                                                                field.TypeTrx,
                                                                                field.Item,
                                                                                field.Batch,
                                                                                nQtyAllocGood,
                                                                                nQtyAllocBad,
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

                                                totalDetails++;

                                                #endregion

                                                if (nQtyAllocBad <= 0)
                                                {
                                                    listRSD2.Remove(rsd2);
                                                }
                                                if (nQtyAlloc <= 0)
                                                {
                                                    break;
                                                }
                                            }
                                        }

                                        listRSD2Copy.Clear();

                                        #endregion
                                    }
                                }
                            }

                            #region Old Coded

                            //for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                            //{
                            //  listRnd1 = (from q in db.LG_RND1s
                            //              where q.c_gdg == gudang && q.c_rnno == rnID
                            //              && q.c_iteno == field.Item &&
                            //              q.c_batch == field.Batch
                            //              && q.n_bsisa == q.n_bqty && q.n_gsisa == q.n_gqty
                            //              select q).ToList();
                            //  if (listRnd1.Count > 0)
                            //  {
                            //    listRnd2 = (from q in db.LG_RND2s
                            //                where q.c_gdg == gudang && q.c_rnno == rnID
                            //                && q.c_iteno == field.Item &&
                            //                q.c_batch == field.Batch
                            //                select q).ToList();
                            //    if (listRnd2.Count > 0)
                            //    {
                            //      for (nLoopC = 0; nLoopC < listRnd2.Count; nLoopC++)
                            //      {
                            //        listRnd3 = (from q in db.LG_RND3s
                            //                    where q.c_gdg == gudang && q.c_rnno == rnID
                            //                    && q.c_iteno == listRnd2[nLoopC].c_iteno &&
                            //                    q.c_batch == listRnd2[nLoopC].c_batch &&
                            //                    q.c_no == listRnd2[nLoopC].c_no
                            //                    select q).ToList();
                            //        for (int rs = 0; rs < listRnd3.Count; rs++)
                            //        {
                            //          rsd2 = (from q in db.LG_RSD2s
                            //                  where q.c_gdg == gudang && q.c_rsno == listRnd3[rs].c_no
                            //                  && q.c_iteno == listRnd3[rs].c_iteno
                            //                  && q.c_batch == listRnd3[rs].c_batch
                            //                  && q.c_rnno == listRnd3[rs].c_rn
                            //                  select q).Take(1).SingleOrDefault();

                            //          rsd2.n_bsisa += listRnd3[rs].n_bqty;
                            //          rsd2.n_gsisa += listRnd3[rs].n_gqty;
                            //        }
                            //        db.LG_RND3s.DeleteAllOnSubmit(listRnd3.ToArray());
                            //        listRnd3.Clear();
                            //      }
                            //    }
                            //  }
                            //  else
                            //  {

                            //  }
                            //}

                            #endregion
                        }
                    }

                    #endregion

                    hasAnyChanges = true;

                    #endregion
                }
                else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    #region Validasi

                    if (string.IsNullOrEmpty(rnID))
                    {
                        result = "Nomor Receive dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    rnh = (from q in db.LG_RNHs
                           where q.c_gdg == gudang && q.c_rnno == rnID && q.c_type == structure.Fields.TypeRN
                           select q).Take(1).SingleOrDefault();

                    if (rnh == null)
                    {
                        result = "Nomor Receive tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (rnh.l_delete.HasValue && rnh.l_delete.Value)
                    {
                        result = "Tidak dapat menghapus Nomor Receive yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.HasFB(db, rnID))
                    {
                        result = "Receive note tidak dapat hapus, karena sudah dibuat faktur pembelian.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, rnh.d_rndate))
                    {
                        result = "Receive note tidak dapat dihapus, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    #endregion

                    rnh.c_update = nipEntry;
                    rnh.d_update = DateTime.Now;

                    rnh.l_delete = true;
                    rnh.v_ket_mark = structure.Fields.Keterangan;

                    typeTrx = (rnh.c_type ?? "").Trim();
                    noSup = (rnh.c_from ?? "").Trim();
                    noDo = (rnh.c_dono ?? "").Trim();

                    listRnd5 = new List<LG_RND5>();

                    #region Tipe 01 || 06

                    if (structure.Fields.TypeRN.Equals("01", StringComparison.OrdinalIgnoreCase) ||
                      structure.Fields.TypeRN.Equals("06", StringComparison.OrdinalIgnoreCase))
                    {
                        listRnd1 = (from q in db.LG_RND1s
                                    where q.c_gdg == gudang && q.c_rnno == rnID
                                    select q).ToList();

                        listRnd2 = (from q in db.LG_RND2s
                                    where q.c_gdg == gudang && q.c_rnno == rnID
                                    select q).ToList();

                        if ((listRnd1 != null) && (listRnd1.Count > 0))
                        {
                            nLoop = listRnd1.Where(x => (x.n_bsisa != x.n_bqty) || (x.n_gsisa != x.n_gqty)).Count();

                            if (nLoop > 0)
                            {
                                result = "Tidak dapat menghapus Nomor Receive jika ada salah satu data detil ada yang berubah.";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }

                                goto endLogic;
                            }

                            listRnd5 = new List<LG_RND5>();

                            for (nLoop = 0; nLoop < listRnd1.Count; nLoop++)
                            {
                                rnd1 = listRnd1[nLoop];

                                if (rnd1 != null)
                                {
                                    noItem = (rnd1.c_iteno ?? string.Empty).Trim();
                                    noBatch = (rnd1.c_batch ?? string.Empty).Trim();

                                    listRnd2Copy = listRnd2.Where(x =>
                                        (noBatch.Equals((x.c_batch ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)) &&
                                        (noItem.Equals((x.c_iteno ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase))).ToList();

                                    for (nLoopC = 0; nLoopC < listRnd2Copy.Count; nLoopC++)
                                    {
                                        rnd2 = listRnd2Copy[nLoopC];

                                        if (rnd2 != null)
                                        {
                                            typeDtl = (rnd2.c_type ?? string.Empty).Trim();
                                            poID = (rnd2.c_no ?? string.Empty).Trim();
                                            nQtyGood = (rnd2.n_gqty.HasValue ? rnd2.n_gqty.Value : 0);

                                            #region Revert PO

                                            if (typeDtl.Equals("02", StringComparison.OrdinalIgnoreCase))
                                            {
                                                modifedPo = true;
                                            }
                                            else
                                            {
                                                modifedPo = Commons.PO_Modifikasi(db, gudang, poID, noItem, true, nQtyGood);
                                            }

                                            if (modifedPo)
                                            {
                                                if (typeTrx.Equals("06", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    modifedPo = Commons.DOPharos_Modifikasi(db, noSup, noDo, poID, noItem, noBatch, true, nQtyGood);
                                                }
                                                else
                                                {
                                                    modifedPo = true;
                                                }

                                                if (modifedPo)
                                                {
                                                    #region Detail Data

                                                    listRnd5.Add(new LG_RND5()
                                                    {
                                                        c_batch = noBatch,
                                                        c_dono = noDo,
                                                        c_entry = nipEntry,
                                                        c_fb = null,
                                                        c_gdg = gudang,
                                                        c_iteno = noItem,
                                                        c_rnno = rnID,
                                                        c_type = typeTrx,
                                                        d_entry = null,
                                                        d_entry_log = date,
                                                        i_urut = null,
                                                        l_status = rnh.l_status,
                                                        n_bsisa = rnd1.n_bsisa,
                                                        n_salpri = 0,
                                                        n_qty = 0,
                                                        n_gsisa = rnd1.n_gsisa,
                                                        v_ket_del = structure.Fields.Keterangan,
                                                        v_type = "03"
                                                    });

                                                    db.LG_RND1s.DeleteOnSubmit(rnd1);
                                                    db.LG_RND2s.DeleteOnSubmit(rnd2);

                                                    #region New Stock Indra 20180305FM

                                                    if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                                                rnID,
                                                                                typeTrx,
                                                                                noItem,
                                                                                noBatch,
                                                                                nQtyGood * -1,
                                                                                0,
                                                                                "KOSONG",
                                                                                "02",
                                                                                "01",
                                                                                nipEntry,
                                                                                "01")) == 0)
                                                    {
                                                        result = "Terdapat Kesalahan pada Item " + noItem + " dengan Batch " + noBatch + ". Harap Hubungi MIS.";

                                                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                                        if (db.Transaction != null)
                                                        {
                                                            db.Transaction.Rollback();
                                                        }

                                                        goto endLogic;
                                                    }
													
													#endregion
													
                                                    #endregion
                                                }
                                                else
                                                {
                                                    result = "Tidak dapat menghapus Nomor Receive jika ada salah satu data detil nomor DO Pharos tidak berhasil diubah.";

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
                                                result = "Tidak dapat menghapus Nomor Receive jika ada salah satu data detil nomor PO tidak berhasil diubah.";

                                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                                if (db.Transaction != null)
                                                {
                                                    db.Transaction.Rollback();
                                                }

                                                goto endLogic;
                                            }

                                            #endregion
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    listRnd2Copy.Clear();
                                }
                            }
                        }

                        listRnd1.Clear();
                        listRnd2.Clear();
                    }

                    #endregion

                    #region Tipe 02

                    if (structure.Fields.TypeRN.Equals("02", StringComparison.OrdinalIgnoreCase))
                    {
                        listRnd1 = (from q in db.LG_RND1s
                                    where q.c_gdg == gudang && q.c_rnno == rnID
                                    select q).ToList();

                        if (listRnd1 != null && listRnd1.Count > 0)
                        {
                            nLoop = listRnd1.Where(x => (x.n_bsisa != x.n_bqty) || (x.n_gsisa != x.n_gqty)).Count();

                            if (nLoop > 0)
                            {
                                result = "Tidak dapat menghapus Nomor Receive jika ada salah satu data detil ada yang berubah.";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }

                                goto endLogic;
                            }

                            listRnd3 = (from q in db.LG_RND3s
                                        where q.c_gdg == gudang && q.c_rnno == rnID
                                        select q).ToList();

                            for (nLoop = 0; nLoop < listRnd1.Count; nLoop++)
                            {
                                rnd1 = listRnd1[nLoop];
                                noItem = (rnd1.c_iteno ?? string.Empty).Trim();
                                noBatch = (rnd1.c_batch ?? string.Empty).Trim();

                                #region New Stock Indra 20180305FM

                                if ((SavingStock.DailyStock(db, rnd1.c_gdg.ToString(),
                                                                rnd1.c_rnno,
                                                                rnh.c_type,
                                                                rnd1.c_iteno,
                                                                rnd1.c_batch,
                                                                rnd1.n_gsisa.Value * -1,
                                                                rnd1.n_bsisa.Value * -1,
                                                                "KOSONG",
                                                                "02",
                                                                "01",
                                                                nipEntry,
                                                                "01")) == 0)
                                {
                                    result = "Terdapat Kesalahan pada Item " + rnd3.c_iteno + " dengan Batch " + rnd3.c_batch + ". Harap Hubungi MIS.";

                                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                    if (db.Transaction != null)
                                    {
                                        db.Transaction.Rollback();
                                    }

                                    goto endLogic;
                                }
								
								#endregion

                                if (listRnd3 != null && listRnd3.Count > 0)
                                {
                                    listRnd3Copy = listRnd3.Where(x =>
                                        (noBatch.Equals((x.c_batch ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)) &&
                                        (noItem.Equals((x.c_iteno ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase))).ToList();

                                    for (nLoopC = 0; nLoopC < listRnd3Copy.Count; nLoopC++)
                                    {
                                        rnd3 = listRnd3Copy[nLoopC];

                                        rsd2 = (from q in db.LG_RSD2s
                                                join q1 in db.LG_RSHes on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                                                where q1.c_type != "03" && q1.c_rsno == rnd3.c_no
                                                && q.c_gdg == gudang && q.c_iteno == rnd3.c_iteno
                                                && q.c_batch == rnd3.c_batch && q.c_rnno == rnd3.c_rn
                                                select q).Take(1).SingleOrDefault();

                                        if (rsd2 != null)
                                        {
                                            rsd2.n_bsisa += rnd3.n_bqty;
                                            rsd2.n_gsisa += rnd3.n_gqty;

                                            listRnd5.Add(new LG_RND5()
                                            {
                                                c_batch = rnd3.c_batch,
                                                c_dono = rnh.c_dono,
                                                c_entry = nipEntry,
                                                c_fb = null,
                                                c_gdg = gudang,
                                                c_iteno = rnd3.c_iteno,
                                                c_rnno = rnID,
                                                c_type = rnd3.c_type,
                                                d_entry = null,
                                                d_entry_log = date,
                                                i_urut = null,
                                                l_status = rnh.l_status,
                                                n_bsisa = rnd3.n_bqty,
                                                n_salpri = 0,
                                                n_qty = 0,
                                                n_gsisa = rnd3.n_gqty,
                                                v_ket_del = structure.Fields.Keterangan,
                                                c_no = rnd3.c_no,
                                                c_rn = rnd3.c_rn,
                                                v_type = "03"
                                            });
                                        }
                                    }
                                    if (listRnd3Copy.Count > 0)
                                    {
                                        listRnd3Copy.Clear();
                                    }
                                }

                            }
                            db.LG_RND1s.DeleteAllOnSubmit(listRnd1.ToArray());
                            db.LG_RND3s.DeleteAllOnSubmit(listRnd3.ToArray());
                        }

                        #region Old Coded

                        //listRnd1 = (from q in db.LG_RND1s
                        //            where q.c_gdg == gudang && q.c_rnno == rnID
                        //            select q).ToList();

                        //listRnd2 = (from q in db.LG_RND2s
                        //            where q.c_gdg == gudang && q.c_rnno == rnID
                        //            select q).ToList();

                        //listRnd3 = (from q in db.LG_RND3s
                        //            where q.c_gdg == gudang && q.c_rnno == rnID
                        //            && q.c_iteno == rnd1.c_iteno && q.c_batch == rnd1.c_batch
                        //            select q).ToList();

                        //if ((listRnd1 != null) && (listRnd1.Count > 0))
                        //{
                        //  nLoop = listRnd1.Where(x => (x.n_bsisa != x.n_bqty) || (x.n_gsisa != x.n_gqty)).Count();

                        //  if (nLoop > 0)
                        //  {
                        //    result = "Tidak dapat menghapus Nomor Receive jika ada salah satu data detil ada yang berubah.";

                        //    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        //    if (db.Transaction != null)
                        //    {
                        //      db.Transaction.Rollback();
                        //    }

                        //    goto endLogic;
                        //  }

                        //  listRnd5 = new List<LG_RND5>();

                        //  for (nLoop = 0; nLoop < listRnd1.Count; nLoop++)
                        //  {
                        //    rnd1 = listRnd1[nLoop];

                        //    listRnd3 = (from q in db.LG_RND3s
                        //                where q.c_gdg == gudang && q.c_rnno == rnID
                        //                && q.c_iteno == rnd1.c_iteno && q.c_batch == rnd1.c_batch
                        //                select q).ToList();

                        //    for (nLoopC = 0; nLoopC < listRnd3.Count; nLoopC++)
                        //    {
                        //      rnd3 = listRnd3[nLoopC];

                        //      rsd2 = (from q in db.LG_RSD2s
                        //              where q.c_gdg == gudang && q.c_batch == field.Batch
                        //              && q.c_iteno == field.Item && q.c_rnno == rnd3.c_no &&
                        //              q.c_rsno == rnd3.c_no
                        //              select q).Take(1).SingleOrDefault();

                        //      rsd2.n_bsisa += rnd3.n_bqty;
                        //      rsd2.n_gsisa += rnd3.n_gqty;
                        //    }

                        //    if (rnd1 != null)
                        //    {
                        //      do
                        //      {

                        //        if (rnd2 != null)
                        //        {
                        //          #region Revert PO

                        //          #region Detail Data

                        //          listRnd5.Add(new LG_RND5()
                        //          {
                        //            c_batch = rnd2.c_batch,
                        //            c_dono = rnh.c_dono,
                        //            c_entry = nipEntry,
                        //            c_fb = null,
                        //            c_gdg = gudang,
                        //            c_iteno = rnd2.c_iteno,
                        //            c_rnno = rnID,
                        //            c_type = rnd2.c_type,
                        //            d_entry = null,
                        //            d_entry_log = date,
                        //            i_urut = null,
                        //            l_status = rnh.l_status,
                        //            n_bsisa = rnd1.n_bsisa,
                        //            n_salpri = 0,
                        //            n_qty = 0,
                        //            n_gsisa = rnd1.n_gsisa,
                        //            v_ket_del = structure.Fields.Keterangan,
                        //            v_type = "03"
                        //          });

                        //          db.LG_RND1s.DeleteOnSubmit(rnd1);
                        //          db.LG_RND2s.DeleteOnSubmit(rnd2);

                        //          listRnd2.Remove(rnd2);

                        //          #endregion

                        //          #endregion
                        //        }
                        //        else
                        //        {
                        //          break;
                        //        }

                        //      } while (rnd2 != null);
                        //    }
                        //  }
                        //}

                        #endregion
                    }

                    #endregion

                    #region Type 03

                    if (string.IsNullOrEmpty(rnh.c_type) ||
                      (rnh.c_type.Equals("03", StringComparison.OrdinalIgnoreCase)))
                    {

                        listRnd1 = (from q in db.LG_RND1s
                                    where q.c_gdg == gudang && q.c_rnno == rnID
                                    select q).ToList();

                        listRnd2 = (from q in db.LG_RND2s
                                    where q.c_gdg == gudang && q.c_rnno == rnID
                                    select q).ToList();



                        if ((listRnd1 != null) && (listRnd1.Count > 0))
                        {
                            nLoop = listRnd1.Where(x => (x.n_bsisa != x.n_bqty) || (x.n_gsisa != x.n_gqty)).Count();

                            if (nLoop > 0)
                            {
                                result = "Tidak dapat menghapus Nomor Receive jika ada salah satu data detil ada yang berubah.";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }

                                goto endLogic;
                            }

                            listRnd5 = new List<LG_RND5>();

                            for (nLoop = 0; nLoop < listRnd1.Count; nLoop++)
                            {
                                rnd1 = listRnd1[nLoop];

                                #region Update Claim Acc Det

                                listRnd2 = (from q in db.LG_RND2s
                                            where q.c_gdg == rnd1.c_gdg &&
                                            q.c_iteno == rnd1.c_iteno &&
                                            q.c_batch == rnd1.c_batch &&
                                            q.c_rnno == rnd1.c_rnno
                                            select q).ToList();
                                nQtyGood = 0;
                                nQtyBad = 0;
                                for (nLoopC = 0; nLoopC < listRnd2.Count; nLoopC++)
                                {
                                    rnd2 = listRnd2[nLoopC];

                                    listRnd5.Add(new LG_RND5()
                                    {
                                        c_batch = rnd2.c_batch,
                                        c_gdg = rnd2.c_gdg,
                                        c_dono = rnd2.c_no,
                                        v_type = "03",
                                        c_iteno = rnd2.c_iteno,
                                        n_qty = rnd2.n_gqty,
                                        n_bsisa = rnd1.n_bsisa,
                                        n_gsisa = rnd1.n_gsisa
                                    });


                                    claimaccd = (from q in db.LG_ClaimAccDs
                                                 where q.c_claimaccno == rnd2.c_no
                                                 && q.c_iteno == rnd2.c_iteno
                                                 select q).Take(1).SingleOrDefault();

                                    claimaccd.n_sisa += rnd2.n_gqty;

                                    nQtyGood += rnd2.n_gqty.Value;
                                    nQtyBad += rnd2.n_bqty.Value;
                                }

                                #endregion

                                #region New Stock Indra 20180305FM

                                if ((SavingStock.DailyStock(db, rnd2.c_gdg.ToString(),
                                                                rnID,
                                                                typeTrx,
                                                                rnd1.c_iteno,
                                                                rnd1.c_batch,
                                                                nQtyGood * -1,
                                                                nQtyBad * -1,
                                                                "KOSONG",
                                                                "02",
                                                                "01",
                                                                nipEntry,
                                                                "01")) == 0)
                                {
                                    result = "Terdapat Kesalahan pada Item " + rnd1.c_iteno + " dengan Batch " + rnd1.c_batch + ". Harap Hubungi MIS.";

                                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                    if (db.Transaction != null)
                                    {
                                        db.Transaction.Rollback();
                                    }

                                    goto endLogic;
                                }
								
								#endregion

                                rnd1.n_gqty -= nQtyGood;
                                rnd1.n_bqty -= nQtyBad;

                                rnd1.n_gsisa -= nQtyGood;
                                rnd1.n_bsisa -= nQtyBad;

                                if (((rnd1.n_gqty.HasValue ? rnd1.n_gqty.Value : 0) == 0) &&
                                          ((rnd1.n_bqty.HasValue ? rnd1.n_bqty.Value : 0) == 0) &&
                                          ((rnd1.n_gsisa.HasValue ? rnd1.n_gsisa.Value : 0) == 0) &&
                                          ((rnd1.n_bsisa.HasValue ? rnd1.n_bsisa.Value : 0) == 0))
                                {

                                    db.LG_RND1s.DeleteOnSubmit(rnd1);
                                }
                                db.LG_RND2s.DeleteOnSubmit(rnd2);
                            }

                            if (listRnd5.Count > 0)
                            {
                                db.LG_RND5s.InsertAllOnSubmit(listRnd5.ToArray());
                            }

                            listRnd5.Clear();
                        }
                    }

                    #endregion

                    #region Type 04 || 02

                    if (string.IsNullOrEmpty(rnh.c_type) || (rnh.c_type.Equals("04", StringComparison.OrdinalIgnoreCase)))
                    {
                        listRnd1 = (from q in db.LG_RND1s
                                    where (q.c_gdg == gudang) && (q.c_rnno == structure.Fields.ReceiveNoteID)
                                    select q).Distinct().ToList();

                        listRnd2 = (from q in db.LG_RND2s
                                    where (q.c_gdg == gudang) && (q.c_rnno == structure.Fields.ReceiveNoteID)
                                    select q).Distinct().ToList();

                        listRnd3 = (from q in db.LG_RND3s
                                    where (q.c_gdg == gudang) && (q.c_rnno == structure.Fields.ReceiveNoteID)
                                    select q).Distinct().ToList();

                        lstTemp = listRnd3.GroupBy(x => x.c_no).Select(y => (string.IsNullOrEmpty(y.Key) ? string.Empty : y.Key.Trim())).ToList();

                        listRSD2 = (from q in db.LG_RSHes
                                    join q1 in db.LG_RSD2s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                                    where (q.c_gdg == gudang) && lstTemp.Contains(q.c_rsno)
                                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                    select q1).Distinct().ToList();

                        for (nLoop = 0; nLoop < listRnd1.Count; nLoop++)
                        {
                            rnd1 = listRnd1[nLoop];

                            nQtyAllocGood = (rnd1.n_gsisa.HasValue ? rnd1.n_gsisa.Value : 0);
                            nQtyAllocBad = (rnd1.n_bsisa.HasValue ? rnd1.n_bsisa.Value : 0);

                            nQtyGood = (rnd1.n_gqty.HasValue ? rnd1.n_gqty.Value : 0);
                            nQtyBad = (rnd1.n_bqty.HasValue ? rnd1.n_bqty.Value : 0);

                            if ((nQtyAllocGood != nQtyGood) || (nQtyAllocBad != nQtyBad))
                            {
                                result = "Receive note tidak dapat dihapus, karena quantity sudah dipergunakan.";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }

                                goto endLogic;
                            }

                            #region New Stock Indra 20180305FM

                            if ((SavingStock.DailyStock(db, gudang.ToString(),
                                                            structure.Fields.ReceiveNoteID,
                                                            typeTrx,
                                                            field.Item,
                                                            field.Batch,
                                                            nQtyGood * -1,
                                                            nQtyBad * -1,
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

                        for (nLoop = 0; nLoop < listRnd3.Count; nLoop++)
                        {
                            rnd3 = listRnd3[nLoop];

                            rsd2 = listRSD2.Find(delegate(LG_RSD2 rsd)
                            {
                                return (string.IsNullOrEmpty(rnd3.c_no) ? string.Empty : rnd3.c_no.Trim()).Equals((string.IsNullOrEmpty(rsd.c_rsno) ? string.Empty : rsd.c_rsno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                  (string.IsNullOrEmpty(rnd3.c_iteno) ? string.Empty : rnd3.c_iteno.Trim()).Equals((string.IsNullOrEmpty(rsd.c_iteno) ? string.Empty : rsd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                  (string.IsNullOrEmpty(rnd3.c_batch) ? string.Empty : rnd3.c_batch.Trim()).Equals((string.IsNullOrEmpty(rsd.c_batch) ? string.Empty : rsd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                  (string.IsNullOrEmpty(rnd3.c_rn) ? string.Empty : rnd3.c_rn.Trim()).Equals((string.IsNullOrEmpty(rsd.c_rnno) ? string.Empty : rsd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
                            });

                            if (rsd2 != null)
                            {
                                nQtyAllocGood = (rnd3.n_gqty.HasValue ? rnd3.n_gqty.Value : 0);
                                nQtyAllocBad = (rnd3.n_bqty.HasValue ? rnd3.n_bqty.Value : 0);

                                rsd2.n_gsisa += nQtyAllocGood;
                                rsd2.n_bsisa += nQtyAllocBad;
                            }

                            listRnd5.Add(new LG_RND5()
                            {
                                c_gdg = gudang,
                                c_rnno = rnID,
                                c_iteno = rnd3.c_iteno,
                                c_batch = rnd3.c_batch,
                                c_dono = rnh.c_dono,
                                c_entry = nipEntry,
                                c_fb = null,
                                c_no = rnd3.c_no,
                                c_type = rnd3.c_type,
                                d_entry = null,
                                d_entry_log = date,
                                i_urut = null,
                                l_status = rnh.l_status,
                                n_bsisa = rnd3.n_bqty,
                                n_gsisa = rnd3.n_gqty,
                                n_qty = null,
                                n_salpri = null,
                                v_ket_del = structure.Fields.Keterangan,
                                v_type = "03"
                            });
                        }

                        if ((listRnd1 != null) && (listRnd1.Count > 0))
                        {
                            db.LG_RND1s.DeleteAllOnSubmit(listRnd1.ToArray());
                            listRnd1.Clear();
                        }

                        if ((listRnd2 != null) && (listRnd2.Count > 0))
                        {
                            db.LG_RND2s.DeleteAllOnSubmit(listRnd2.ToArray());
                            listRnd2.Clear();
                        }

                        if ((listRnd3 != null) && (listRnd3.Count > 0))
                        {
                            db.LG_RND3s.DeleteAllOnSubmit(listRnd3.ToArray());
                            listRnd3.Clear();
                        }

                        if ((listRnd5 != null) && (listRnd5.Count > 0))
                        {
                            db.LG_RND5s.InsertAllOnSubmit(listRnd5.ToArray());
                            listRnd5.Clear();
                        }

                        #region Old Coded

                        ////Hitung RND 1

                        //var varListRnd1 = (from q in db.LG_RND1s
                        //                   where q.c_rnno == rnID && q.c_gdg == gudang
                        //                   select q).ToList();

                        //for (nLoop = 0; nLoop < varListRnd1.Count; nLoop++)
                        //{
                        //  //cek rnd1 sisa = qty
                        //  listRnd1 = (from q in db.LG_RND1s
                        //              where q.c_gdg == varListRnd1[nLoop].c_gdg &&
                        //              q.c_rnno == varListRnd1[nLoop].c_rnno &&
                        //              q.c_iteno == varListRnd1[nLoop].c_iteno &&
                        //              q.c_batch == varListRnd1[nLoop].c_batch &&
                        //              q.n_gqty == q.n_gsisa && q.n_bqty == q.n_bsisa
                        //              select q).ToList();

                        //  if (listRnd1.Count > 0)
                        //  {
                        //    //hitung rnd2
                        //    listRnd2 = (from q in db.LG_RND2s
                        //                where q.c_gdg == varListRnd1[nLoop].c_gdg &&
                        //                q.c_rnno == varListRnd1[nLoop].c_rnno &&
                        //                q.c_iteno == varListRnd1[nLoop].c_iteno &&
                        //                q.c_batch == varListRnd1[nLoop].c_batch
                        //                select q).ToList();

                        //    if (listRnd2.Count > 0)
                        //    {
                        //      for (int cRnd2 = 0; cRnd2 < listRnd2.Count; cRnd2++)
                        //      {
                        //        //hitung rnd3
                        //        listRnd3 = (from q in db.LG_RND3s
                        //                    where q.c_gdg == listRnd2[cRnd2].c_gdg &&
                        //                    q.c_rnno == listRnd2[cRnd2].c_rnno &&
                        //                    q.c_iteno == listRnd2[cRnd2].c_iteno &&
                        //                    q.c_batch == listRnd2[cRnd2].c_batch &&
                        //                    q.c_no == listRnd2[cRnd2].c_no
                        //                    select q).ToList();

                        //        for (int cRnd3 = 0; cRnd3 < listRnd3.Count; cRnd3++)
                        //        {
                        //          //cek rsd2
                        //          rsd2 = (from q in db.LG_RSD2s
                        //                  where q.c_gdg == listRnd3[cRnd3].c_gdg &&
                        //                  q.c_rsno == listRnd3[cRnd3].c_no &&
                        //                  q.c_iteno == listRnd3[cRnd3].c_iteno &&
                        //                  q.c_batch == listRnd3[cRnd3].c_batch &&
                        //                  q.c_rnno == listRnd3[cRnd3].c_rn
                        //                  select q).Take(1).SingleOrDefault();

                        //          listRnd5.Add(new LG_RND5()
                        //          {
                        //            c_batch = listRnd3[cRnd3].c_batch,
                        //            c_dono = listRnd3[cRnd3].c_no,
                        //            c_entry = structure.Fields.Entry,
                        //            c_gdg = listRnd3[cRnd3].c_gdg,
                        //            c_iteno = listRnd3[cRnd3].c_iteno,
                        //            c_rnno = listRnd3[cRnd3].c_rnno,
                        //            n_bsisa = varListRnd1[nLoop].n_bsisa,
                        //            n_gsisa = varListRnd1[nLoop].n_gsisa,
                        //            n_qty = listRnd3[cRnd3].n_gqty,
                        //            v_ket_del = structure.Fields.Keterangan,
                        //            v_type = "03"
                        //          });

                        //          //update sisa rsd2
                        //          rsd2.n_bsisa += listRnd3[cRnd3].n_bqty;
                        //          rsd2.n_gsisa += listRnd3[cRnd3].n_gqty;
                        //        }

                        //        if (listRnd3.Count > 0)
                        //        {
                        //          db.LG_RND3s.DeleteAllOnSubmit(listRnd3.ToArray());
                        //          db.LG_RND5s.InsertAllOnSubmit(listRnd5.ToArray());
                        //          listRnd3.Clear();
                        //          listRnd5.Clear();
                        //        }

                        //      }

                        //      db.LG_RND2s.DeleteAllOnSubmit(listRnd2.ToArray());
                        //      listRnd2.Clear();
                        //    }

                        //    db.LG_RND1s.DeleteAllOnSubmit(listRnd1.ToArray());
                        //    listRnd1.Clear();
                        //  }
                        //  else
                        //  {
                        //    result = "Tidak dapat menghapus Nomor Receive jika ada salah satu data detil ada yang berubah.";

                        //    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        //    if (db.Transaction != null)
                        //    {
                        //      db.Transaction.Rollback();
                        //    }

                        //    goto endLogic;
                        //  }

                        //}

                        #endregion
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
                rpe = ResponseParser.ResponseParserEnum.IsError;

                result = string.Format("ScmsSoaLibrary.Bussiness.Pembelian:ReceiveNote - {0}", ex.StackTrace);

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

        public string PurchaseOrderApoteker(ScmsSoaLibrary.Parser.Class.PurchaseOrderStructure structure)
        {
            if ((structure == null)) // || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            string result = null,
            poktno = null,
            mon = null,
            tipekode = null,
            typepo = null;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;

            LG_POH poh = null;
            LG_PO_OKTH okth = null;

            DateTime date = DateTime.Now;

            char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '1' : structure.Fields.Gudang[0]);
            typepo = structure.Fields.typeApoteker;


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

                if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    poh = (from q in db.LG_POHs
                           where (q.c_pono == structure.Fields.PurchaseOrderID)
                           select q).Take(1).SingleOrDefault();

                    if (poh != null)
                    {
                        switch (date.Month)
                        {
                            case 1: mon = "I"; break;
                            case 2: mon = "II"; break;
                            case 3: mon = "III"; break;
                            case 4: mon = "IV"; break;
                            case 5: mon = "V"; break;
                            case 6: mon = "VI"; break;
                            case 7: mon = "VII"; break;
                            case 8: mon = "VIII"; break;
                            case 9: mon = "IX"; break;
                            case 10: mon = "X"; break;
                            case 11: mon = "XI"; break;
                            case 12: mon = "XII"; break;
                        }

                        switch (typepo)
                        {
                            case "02": tipekode = "Psi"; break;
                            case "07": tipekode = "Pre"; break;
                            case "09": tipekode = "OOT"; break;
                        }

                        poktno = poh.c_pono + "/" + tipekode + "/HO/" + mon + "/" + date.Year;

                        okth = (from q in db.LG_PO_OKTHs
                                where q.c_pono == structure.Fields.PurchaseOrderID
                                select q).Take(1).SingleOrDefault();

                        if (okth == null)
                        {
                            okth = new LG_PO_OKTH()
                            {
                                c_gdg = gudang,
                                c_poktno = poktno,
                                c_pono = poh.c_pono,
                                d_poktdate = date,
                                c_nosup = poh.c_nosup,
                                v_ket = null,
                                l_print = false,
                                l_send = false,
                                l_status_pok = structure.Fields.isConfirm,
                                c_entry = nipEntry,
                                d_entry = date,
                                c_update = nipEntry,
                                d_update = date,
                                c_type = typepo
                            };

                            db.LG_PO_OKTHs.InsertOnSubmit(okth);

                            hasAnyChanges = true;

                            dic = new Dictionary<string, string>();
                            dic.Add("poktno", poktno);
                            dic.Add("pono", structure.Fields.PurchaseOrderID);
                        }
                    }
                    else
                    {
                        result = "Data PO tidak terbaca dari database.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    #endregion
                }
                else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    okth = (from q in db.LG_PO_OKTHs
                            where q.c_pono == structure.Fields.PurchaseOrderID
                            select q).Take(1).SingleOrDefault();

                    if (okth != null)
                    {
                        db.LG_PO_OKTHs.DeleteOnSubmit(okth);
                        hasAnyChanges = true;
                    }
                    else
                    {
                        result = "Data PO belum diconfirm.";

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Pembelian:PurchaseOrderApoteker - {0}", ex.Message);

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

        public string LimitPOItem(ScmsSoaLibrary.Parser.Class.LimitPOItemStructure structure)
        {
            if ((structure == null)) // || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            ScmsSoaLibrary.Parser.Class.LimitPOItemStructureField field = null;

            string result = null;

            int nLoop = 0;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;

            SCMS_LIMITPO scms_limitpo = null;
            SCMS_LIMITPO_DIV scms_limitpo_div = null;


            List<SCMS_LIMITPO> listLimitPO = null;


            DateTime date = DateTime.Now;

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

                if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    #region Populate Detail

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        listLimitPO = (from q in db.SCMS_LIMITPOs
                                       where q.n_tahun == structure.Fields.nTahun
                                       && q.n_bulan == structure.Fields.nBulan
                                       && q.c_kddivpri == structure.Fields.kddivprihdr
                                       select q).ToList();

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if (field.IsModified)
                            {
                                #region Modify

                                if ((listLimitPO != null) && (listLimitPO.Count > 0))
                                {
                                    scms_limitpo = listLimitPO.Find(delegate(SCMS_LIMITPO limitpo)
                                    {
                                        return field.Item.Equals((string.IsNullOrEmpty(limitpo.c_iteno) ? string.Empty : limitpo.c_iteno.Trim()));
                                    });

                                    if (scms_limitpo != null)
                                    {

                                        scms_limitpo.n_budget = field.Budget;
                                        scms_limitpo.n_balance = field.Balance;
                                        scms_limitpo.n_availablebudget = field.availablebudget;
                                    }
                                }


                                hasAnyChanges = true;
                                #endregion
                            }
                        }

                        if (listLimitPO != null)
                        {
                            listLimitPO.Clear();
                        }
                    }

                    #endregion

                    //dic = new Dictionary<string, string>();

                    //dic.Add("kddivpri", structure.Fields.kddivpri);
                    //dic.Add("tahun", structure.Fields.nTahun.ToString());
                    //dic.Add("bulan", structure.Fields.nBulan.ToString());

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Pembelian:PurchaseOrderApoteker - {0}", ex.Message);

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

        public string LimitPODivPri(ScmsSoaLibrary.Parser.Class.LimitPOItemStructure structure)
        {
            if ((structure == null)) // || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            ScmsSoaLibrary.Parser.Class.LimitPOItemStructureField field = null;

            string result = null;

            int nLoop = 0;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;

            SCMS_LIMITPO_DIV scms_limitpo_div = null;

            List<SCMS_LIMITPO_DIV> listLimitPODiv = null;


            DateTime date = DateTime.Now;

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

                if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    #region Populate Detail

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        listLimitPODiv = (from q in db.SCMS_LIMITPO_DIVs
                                          where q.n_tahun == structure.Fields.nTahun
                                          && q.n_bulan == structure.Fields.nBulan
                                          && q.c_nosup == structure.Fields.nosup
                                          select q).ToList();

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if (field.IsModified)
                            {
                                #region Modify

                                if ((listLimitPODiv != null) && (listLimitPODiv.Count > 0))
                                {
                                    scms_limitpo_div = listLimitPODiv.Find(delegate(SCMS_LIMITPO_DIV limitpodiv)
                                    {
                                        return field.kddivpridtl.Equals((string.IsNullOrEmpty(limitpodiv.c_kddivpri) ? string.Empty : limitpodiv.c_kddivpri.Trim()));
                                    });

                                    if (scms_limitpo_div != null)
                                    {

                                        scms_limitpo_div.n_budget = field.Budget;
                                        scms_limitpo_div.n_balance = field.Balance;
                                    }
                                }


                                hasAnyChanges = true;
                                #endregion
                            }
                        }

                        if (listLimitPODiv != null)
                        {
                            listLimitPODiv.Clear();
                        }
                    }

                    #endregion

                    //dic = new Dictionary<string, string>();

                    //dic.Add("kddivpri", structure.Fields.kddivpri);
                    //dic.Add("tahun", structure.Fields.nTahun.ToString());
                    //dic.Add("bulan", structure.Fields.nBulan.ToString());

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Pembelian:LimitPODivPri - {0}", ex.Message);

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

        public string LimitPOPrincipal(ScmsSoaLibrary.Parser.Class.LimitPOItemStructure structure)
        {
            if (structure.Fields == null)
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            ScmsSoaLibrary.Parser.Class.LimitPOItemStructureField field = null;

            string result = null;

            int nLoop = 0;

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            IDictionary<string, string> dic = null;

            SCMS_LIMITPRI scms_limitpri = null;

            List<SCMS_LIMITPRI> listLimitPri = null;

            DateTime date = DateTime.Now;

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

                if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    #region Populate Detail

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            listLimitPri = (from q in db.SCMS_LIMITPRIs
                                            where q.n_tahun == field.nTahun
                                            && q.n_bulan == field.nBulan
                                            && q.c_nosup == field.nosup
                                            select q).ToList();

                            if (field.IsModified)
                            {
                                #region Modify

                                if ((listLimitPri != null) && (listLimitPri.Count > 0))
                                {
                                    scms_limitpri = listLimitPri.Find(delegate(SCMS_LIMITPRI limitpri)
                                    {
                                        return field.nosup.Equals((string.IsNullOrEmpty(limitpri.c_nosup) ? string.Empty : limitpri.c_nosup.Trim()));
                                    });

                                    if (scms_limitpri != null)
                                    {
                                        scms_limitpri.n_besls = field.nBestSls;
                                        scms_limitpri.n_percentadj = field.nPercentAdj;
                                        scms_limitpri.n_qty = field.nQty;
                                        scms_limitpri.n_budget = field.Budget;
                                        scms_limitpri.n_availablebudget = field.availablebudget;


                                        using (SqlConnection cn = new SqlConnection(Functionals.ActiveConnectionString))
                                        {
                                            cn.Open();
                                            SqlCommand cmd = cn.CreateCommand();
                                            cmd.CommandText = "exec LG_POLimiter '" + field.nosup + "', " + field.nTahun + ", " + field.nBulan + ", " + field.Budget + ", '" + nipEntry + "', 1, 0, 55, 0, 0";
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                }

                                hasAnyChanges = true;
                                #endregion
                            }
                        }

                        if (listLimitPri != null)
                        {
                            listLimitPri.Clear();
                        }
                    }

                    #endregion

                    //dic = new Dictionary<string, string>();

                    //dic.Add("kddivpri", structure.Fields.kddivpri);
                    //dic.Add("tahun", structure.Fields.nTahun.ToString());
                    //dic.Add("bulan", structure.Fields.nBulan.ToString());

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Pembelian:LimitPOPrincipal - {0}", ex.Message);

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

        #region old limitpoprincipal
        //public string LimitPOPrincipalOld(ScmsSoaLibrary.Parser.Class.LimitPOItemStructure structure)
        //{
        //    if ((structure == null)) // || (structure.Fields == null))
        //    {
        //        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
        //    }

        //    ScmsSoaLibrary.Parser.Class.LimitPOItemStructureField field = null;

        //    string result = null;

        //    int nLoop = 0;

        //    bool hasAnyChanges = false;

        //    ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

        //    IDictionary<string, string> dic = null;

        //    DateTime date = DateTime.Now;

        //    List<SqlParameter> lstParams = new List<SqlParameter>();

        //    Config cfg = new Config();

        //    ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

        //    string nipEntry = null,
        //        tmpQuery = null;

        //    nipEntry = structure.Fields.Entry;

        //    if (string.IsNullOrEmpty(nipEntry))
        //    {
        //        result = "Nip penanggung jawab dibutuhkan.";

        //        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        //        goto endLogic;
        //    }

        //    try
        //    {
        //        db.Connection.Open();

        //        db.Transaction = db.Connection.BeginTransaction();

        //        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        //        {
        //            #region Modify

        //            lstParams.Add(new SqlParameter("@nosup", System.Data.SqlDbType.VarChar)
        //            {
        //                Size = 15,
        //                Value = structure.Fields.nosup
        //            });

        //            lstParams.Add(new SqlParameter("@tahun", System.Data.SqlDbType.VarChar)
        //            {
        //                Size = 15,
        //                Value = structure.Fields.nTahun
        //            });

        //            lstParams.Add(new SqlParameter("@bulan", System.Data.SqlDbType.VarChar)
        //            {
        //                Size = 15,
        //                Value = structure.Fields.nBulan
        //            });

        //            lstParams.Add(new SqlParameter("@limit", System.Data.SqlDbType.VarChar)
        //            {
        //                Size = 15,
        //                Value = structure.Fields.limit
        //            });

        //            lstParams.Add(new SqlParameter("@user", System.Data.SqlDbType.VarChar)
        //            {
        //                Size = 15,
        //                Value = nipEntry
        //            });

        //            lstParams.Add(new SqlParameter("@nextlimit", System.Data.SqlDbType.VarChar)
        //            {
        //                Size = 15,
        //                Value = structure.Fields.percentage
        //            });

        //            lstParams.Add(new SqlParameter("@param2", System.Data.SqlDbType.VarChar)
        //            {
        //                Size = 15,
        //                Value = "0"
        //            });

        //            tmpQuery = "Exec LG_POLimiter @nosup, @tahun, @bulan, @limit, @user, @param2, @param2, @nextlimit, @param2, @param2";

        //            Functionals.ExecProcedures(cfg, tmpQuery, lstParams.ToArray());

        //            lstParams.Clear();

        //            hasAnyChanges = true;

        //            //dic = new Dictionary<string, string>();

        //            //dic.Add("nosup", structure.Fields.kddivpri);
        //            //dic.Add("tahun", structure.Fields.nTahun.ToString());
        //            //dic.Add("bulan", structure.Fields.nBulan.ToString());

        //            #endregion
        //        }
        //        else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        //        {
        //            #region Modify

        //            #region Populate Detail

        //            if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
        //            {                    
        //                for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
        //                {
        //                    field = structure.Fields.Field[nLoop];

        //                    if (field.IsModified)
        //                    {
        //                        #region Modify

        //                        lstParams.Add(new SqlParameter("@nosup", System.Data.SqlDbType.VarChar)
        //                        {
        //                            Size = 15,
        //                            Value = field.nosup
        //                        });

        //                        lstParams.Add(new SqlParameter("@tahun", System.Data.SqlDbType.VarChar)
        //                        {
        //                            Size = 15,
        //                            Value = field.nTahun
        //                        });

        //                        lstParams.Add(new SqlParameter("@bulan", System.Data.SqlDbType.VarChar)
        //                        {
        //                            Size = 15,
        //                            Value = field.nBulan
        //                        });

        //                        lstParams.Add(new SqlParameter("@limit", System.Data.SqlDbType.VarChar)
        //                        {
        //                            Size = 15,
        //                            Value = field.Budget
        //                        });

        //                        lstParams.Add(new SqlParameter("@user", System.Data.SqlDbType.VarChar)
        //                        {
        //                            Size = 15,
        //                            Value = nipEntry
        //                        });

        //                        lstParams.Add(new SqlParameter("@nextlimit", System.Data.SqlDbType.VarChar)
        //                        {
        //                            Size = 15,
        //                            Value = field.nextlimit
        //                        });

        //                        lstParams.Add(new SqlParameter("@param1", System.Data.SqlDbType.VarChar)
        //                        {
        //                            Size = 15,
        //                            Value = "1"
        //                        });

        //                        lstParams.Add(new SqlParameter("@param2", System.Data.SqlDbType.VarChar)
        //                        {
        //                            Size = 15,
        //                            Value = "0"
        //                        });

        //                        tmpQuery = "Exec LG_POLimiter @nosup, @tahun, @bulan, @limit, @user, @param1, @param2, @nextlimit, @param2, @param2";

        //                        Functionals.ExecProcedures(cfg, tmpQuery, lstParams.ToArray());

        //                        lstParams.Clear();

        //                        hasAnyChanges = true;
        //                        #endregion
        //                    }
        //                }
        //            }

        //            #endregion
        //            #endregion
        //        }

        //        if (hasAnyChanges)
        //        {
        //            //db.SubmitChanges();

        //            //db.Transaction.Commit();

        //            //db.Transaction.Rollback();

        //            rpe = ResponseParser.ResponseParserEnum.IsSuccess;
        //        }
        //        else
        //        {
        //            db.Transaction.Rollback();

        //            rpe = ResponseParser.ResponseParserEnum.IsFailed;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (db.Transaction != null)
        //        {
        //            db.Transaction.Rollback();
        //        }

        //        result = string.Format("ScmsSoaLibrary.Bussiness.Pembelian:PurchaseOrderApoteker - {0}", ex.Message);

        //        Logger.WriteLine(result);
        //        Logger.WriteLine(ex.StackTrace);
        //    }

        //endLogic:
        //    result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

        //    if (dic != null)
        //    {
        //        dic.Clear();
        //    }

        //    db.Dispose();

        //    return result;
        //}

        #endregion old limitpoprincipal
    }
}