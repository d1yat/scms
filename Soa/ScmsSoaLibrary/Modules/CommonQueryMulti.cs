using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsModel.Core;
using ScmsModel;
using Ext.Net;
using System.Globalization;
using System.Data.Linq.SqlClient;
using ScmsSoaLibraryInterface.Core.Crypto;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaLibrary.Modules
{
    partial class CommonQueryMulti
    {
        #region Internal Class

        //internal class LG_PLFormView3
        //{
        //  public string c_iteno { get; set; }
        //  public string c_batch { get; set; }
        //  public decimal? n_gsisa { get; set; }
        //  public decimal? n_gsisatotal { get; set; }
        //  public string c_type { get; set; }
        //  public decimal? n_sisaType { get; set; }
        //  public decimal? n_sisaSPTotal { get; set; }
        //}

        internal class Total_Stock
        {
            public string tipe_tot { get; set; }
            public string gudang_asal_tot { get; set; }
            public string gudang_tujuan_tot { get; set; }
            public string c_iteno_Tot { get; set; }
            public string v_itnam_Tot { get; set; }
            public string c_batch_Tot { get; set; }
            public string d_expired_Tot { get; set; }
            public decimal n_gsisa_Tot { get; set; }
            public decimal n_bsisa_Tot { get; set; }
           
            
        }
        internal class SJAuto
        {
            public string c_iteno { get; set; }
            public string c_no { get; set; }
            public decimal n_gqty { get; set; }
            public decimal n_gsisa { get; set; }
            public string c_orno { get; set; }
            public string c_spno { get; set; }
            public string c_batch { get; set; }
            public string v_itnam { get; set; }
        }

        internal class SGList
        {
            public string c_iteno { get; set; }
            public string c_spgno { get; set; }
            public decimal nQty { get; set; }
        }

        internal class SJDetailAutoGenerator
        {
            public string c_iteno { get; set; }
            public bool l_new { get; set; }
            public decimal n_gqty { get; set; }
            public decimal n_booked { get; set; }
            public string c_spgno { get; set; }
            public string c_batch { get; set; }
            public string v_itnam { get; set; }
            public decimal n_adjust { get; set; }
        }

        internal class SJDetailGenerator
        {
            public string c_spgno { get; set; }
            public string c_iteno { get; set; }
            public char gudang { get; set; }
            public string v_itemdesc { get; set; }
            public decimal n_sisa { get; set; }
            public string c_batch { get; set; }
            public bool l_expired { get; set; }
            public DateTime d_expired { get; set; }
            public decimal n_box { get; set; }
        }

        internal class ItemStockAvaible
        {
            public string c_iteno { get; set; }
            public string c_batch { get; set; }
            public decimal n_gsisa { get; set; }
            public decimal n_bsisa { get; set; }
            public DateTime d_expired { get; set; }
        }

        internal class SCMS_USER_CLASS
        {
            public string c_nip { get; set; }
            public string c_gdg { get; set; }
            public string v_gdgdesc { get; set; }
            public bool l_aktif { get; set; }
            public string v_username { get; set; }
            public string v_password { get; set; }
            public string c_entry { get; set; }
            public DateTime d_entry { get; set; }
            public string v_entry { get; set; }
            public string c_update { get; set; }
            public DateTime d_update { get; set; }
            public string v_update { get; set; }
            public string x_hash { get; set; }
            public string v_imgpic { get; set; }
            public string v_imgwall { get; set; }
            public string c_nosup { get; set; }
            public string v_supdesc { get; set; }
            public string c_kddivpri { get; set; }
            public string v_divpridesc { get; set; }
            public bool IsUserPrinsipal { get; set; }
            public bool IsUserCabang { get; set; }
        }

        internal class SCMS_GROUP_CLASS
        {
            public string c_group { get; set; }
            public bool l_aktif { get; set; }
            public string v_akses { get; set; }
            public string v_group_desc { get; set; }
            public int totalList { get; set; }
        }

        internal class SCMS_USERGROUP_CLASS
        {
            public string c_nip { get; set; }
            public string c_gdg { get; set; }
            public string v_gdgdesc { get; set; }
            public bool? l_aktif { get; set; }
            public string v_username { get; set; }
            public string v_password { get; set; }
            public bool l_aktif_group { get; set; }
            public string v_akses { get; set; }
            public string c_group { get; set; }
            public string v_group_desc { get; set; }
            public string x_hash { get; set; }
            public string v_imgpic { get; set; }
            public string v_imgwall { get; set; }
            public string c_nosup { get; set; }
            public string v_supdesc { get; set; }
            public string c_kddivpri { get; set; }
            public string v_divpridesc { get; set; }
            public bool IsUserPrinsipal { get; set; }
            public bool IsUserCabang { get; set; }
        }

        internal class SCMS_UNION_SP_SPG
        {
            public string c_cusno { get; set; }
            public string c_spno { get; set; }
            public string c_sp { get; set; }
            public DateTime? d_spdate { get; set; }
            public decimal? n_acc { get; set; }
            public decimal? n_qty { get; set; }
            public decimal? n_sisa { get; set; }
            public bool? l_aktif { get; set; }
        }

        internal class SCMS_RNKHUSUS_ITEM
        {
            public string v_itnam { get; set; }
            public string c_iteno { get; set; }
            public decimal n_qty { get; set; }
            public decimal n_sisa { get; set; }
        }

        internal class SCMS_UNION_FB_FBR
        {
            public string FakturBeli { get; set; }
            public string Faktur { get; set; }
            public string Kurs { get; set; }
            public string SuplierID { get; set; }
            public DateTime? FakturDate { get; set; }
            public DateTime FakturTopDate { get; set; }
            public decimal Value { get; set; }
            public decimal SisaTagihan { get; set; }
            public string Tipe { get; set; }
            public bool isDeleted { get; set; }
        }

        internal class SCMS_UNION_FJ_FJR
        {
            public string FakturEx { get; set; }
            public string Faktur { get; set; }
            public string Kurs { get; set; }
            public string CustomerID { get; set; }
            public DateTime? FakturDate { get; set; }
            public DateTime FakturTopDate { get; set; }
            public decimal Value { get; set; }
            public decimal SisaTagihan { get; set; }
            public string Tipe { get; set; }
            public bool isCabang { get; set; }
            public bool isDeleted { get; set; }
        }

        internal class SCMS_RNFaktur_UNION
        {
            public char Gudang { get; set; }
            public string GudangDesc { get; set; }
            public string ReceiveNote { get; set; }
            public DateTime? TanggalRN { get; set; }
            public string NoSupl { get; set; }
            public string DeliveryNo { get; set; }
            public DateTime? TanggalDO { get; set; }
        }

        internal class SCMS_RNFaktur_ITEM_UNION
        {
            public string c_iteno { get; set; }
            public string v_itnam { get; set; }
            public string c_type { get; set; }
            public string v_type_desc { get; set; }
            public decimal n_bea { get; set; }
            public decimal n_disc { get; set; }
            public decimal n_qty { get; set; }
            public decimal n_salpri { get; set; }
            public decimal n_ppph { get; set; }
        }

        internal class UnionPLSTT
        {
            public string NoRef { get; set; }
            public DateTime TglRef { get; set; }
        }

        internal class TempUnionDOJoin
        {
            public char c_gdg { get; set; }
            public string c_dono { get; set; }
            public DateTime d_dodate { get; set; }
            public decimal n_qty { get; set; }
            public string c_rnno { get; set; }
        }

        internal class PLSupplierClass
        {
            public string c_nosup { get; set; }
            public string v_nama { get; set; }
        }

        internal class PLItemClass
        {
            public string c_iteno { get; set; }
            public string v_itnam { get; set; }
            public string v_undes { get; set; }
        }

        internal class PLItemCategory
        {
            public string c_iteno { get; set; }
        }

        internal class PLCustomerCategory
        {
            public string c_nosup { get; set; }
        }

        internal class STDetail
        {
            public string noTrans { get; set; }
            public DateTime? dTrans { get; set; }
            public string c_type_lat { get; set; }
        }

        internal class SJDetailBatch
        {
            public string c_iteno { get; set; }
            public string c_batch { get; set; }
            public DateTime? d_expired { get; set; }
            public decimal n_soh { get; set; }
            public decimal? n_box { get; set; }
        }

        internal class SJDetailBatch2
        {
            public string c_iteno { get; set; }
            public string c_batch { get; set; }
            public DateTime? d_expired { get; set; }
            public decimal n_soh { get; set; }
            public decimal? n_box { get; set; }
        }

        #region StockOpname Indra 20171231FM
        internal class StockOpname
        {
            public string gudang { get; set; }
            public string kdprincipal { get; set; }
            public string principal { get; set; }
            public string kddivprincipal { get; set; }
            public string divprincipal { get; set; }
            public string location { get; set; }
            public string kdbarang { get; set; }
            public string nmbarang { get; set; }
            public string stbarang { get; set; }
            public string batch { get; set; }
            public decimal qtysys { get; set; }
            public decimal SOQty { get; set; }
            public decimal recount1 { get; set; }
            public decimal recount2 { get; set; }
            public decimal selisih { get; set; }
            public DateTime expired { get; set; }
            public decimal box { get; set; }
            public string stage { get; set; }
            public string noform { get; set; }
            public string noadj { get; set; }
            public bool hitawal { get; set; }
            public bool rec1 { get; set; }
            public bool rec2 { get; set; }
            public bool adjust { get; set; }
        }

        #endregion

        #region Monitoring PL Indra 20171231FM

        internal class MonitoringPL
        {
            public string GUDANG { get; set; }
            public int SPRECEIVED { get; set; }
            public int CREATEPL { get; set; }
            public int PICKING { get; set; }
            public int CREATEDO { get; set; }
            public int CHECKING { get; set; }
            public int PACKING { get; set; }
            public int READY { get; set; }
        }

        #endregion

        #endregion

        public static IDictionary<string, object> ModelGridQueryRightManagement(string connectionString, int start, int limit, bool allQuery, string sort, string dir, string model, IDictionary<string, Functionals.ParameterParser> parameters)
        {
            IDictionary<string, object> dic = new Dictionary<string, object>();

            ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);
            //db.CommandTimeout = 1000;

            bool decryptedString = false;
            int nCount = 0;
            string paternLike = null;

            try
            {
                switch (model)
                {
                    #region Right Management

                    #region MODEL_COMMON_QUERY_MULTIPLE_USERRIGHT

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_USERRIGHT:
                        {
                            string strongKey = string.Empty;

                            bool.TryParse((parameters.ContainsKey("decrypted") ?
                              (parameters["decrypted"].Value == null ? "false" : parameters["decrypted"].Value.ToString()) : "false"), out decryptedString);

                            var qryCus = (from q in db.SCMS_USERs
                                          join q1 in db.LG_Cusmas on q.c_gdg equals q1.c_cusno into q_1
                                          from qCus in q_1.DefaultIfEmpty()
                                          join q2 in db.SCMS_USERs on q.c_entry equals q2.c_nip into q_2
                                          from qEntry in q_2.DefaultIfEmpty()
                                          join q3 in db.SCMS_USERs on q.c_update equals q3.c_nip into q_3
                                          from qUpdate in q_3.DefaultIfEmpty()
                                          join q4 in db.LG_DatSups on q.c_nosup equals q4.c_nosup into q_4
                                          from qSup in q_4.DefaultIfEmpty()
                                          join q5 in db.FA_MsDivPris on q.c_kddivpri equals q5.c_kddivpri into q_5
                                          from qDivPri in q_5.DefaultIfEmpty()
                                          where (qCus.c_cusno != null)
                                          select new SCMS_USER_CLASS()
                                          {
                                              c_nip = q.c_nip,
                                              c_gdg = q.c_gdg.ToString(),
                                              v_gdgdesc = qCus.v_cunam,
                                              l_aktif = (q.l_aktif.HasValue ? q.l_aktif.Value : false),
                                              v_username = q.v_username,
                                              v_password = q.v_password,
                                              c_entry = q.c_entry,
                                              d_entry = (q.d_entry.HasValue ? q.d_entry.Value : Functionals.StandardSqlDateTime),
                                              v_entry = qEntry.v_username,
                                              c_update = q.c_update,
                                              d_update = (q.d_update.HasValue ? q.d_update.Value : Functionals.StandardSqlDateTime),
                                              v_update = qUpdate.v_username,
                                              x_hash = q.x_hash,
                                              v_imgpic = q.v_imgfile,
                                              v_imgwall = q.v_wallpaper,
                                              c_nosup = q.c_nosup,
                                              v_supdesc = qSup.v_nama,
                                              c_kddivpri = q.c_kddivpri,
                                              v_divpridesc = qDivPri.v_nmdivpri,
                                              IsUserPrinsipal = (((q.c_nosup == null) || (q.c_kddivpri == null)) || ((q.c_nosup == "") || (q.c_kddivpri == "")) ?
                                                false : true),
                                              IsUserCabang = true
                                          });

                            var qry = (from q in db.SCMS_USERs
                                       join q1 in db.LG_MsGudangs on q.c_gdg equals q1.c_gdg.ToString() into q_1
                                       from qGdg in q_1.DefaultIfEmpty()
                                       join q2 in db.SCMS_USERs on q.c_entry equals q2.c_nip into q_2
                                       from qEntry in q_2.DefaultIfEmpty()
                                       join q3 in db.SCMS_USERs on q.c_update equals q3.c_nip into q_3
                                       from qUpdate in q_3.DefaultIfEmpty()
                                       join q4 in db.LG_DatSups on q.c_nosup equals q4.c_nosup into q_4
                                       from qSup in q_4.DefaultIfEmpty()
                                       join q5 in db.FA_MsDivPris on q.c_kddivpri equals q5.c_kddivpri into q_5
                                       from qDivPri in q_5.DefaultIfEmpty()
                                       where (qGdg.c_gdg != null)
                                       select new SCMS_USER_CLASS()
                                       {
                                           c_nip = q.c_nip,
                                           c_gdg = q.c_gdg.ToString(),
                                           v_gdgdesc = qGdg.v_gdgdesc,
                                           l_aktif = (q.l_aktif.HasValue ? q.l_aktif.Value : false),
                                           v_username = q.v_username,
                                           v_password = q.v_password,
                                           c_entry = q.c_entry,
                                           d_entry = (q.d_entry.HasValue ? q.d_entry.Value : Functionals.StandardSqlDateTime),
                                           v_entry = qEntry.v_username,
                                           c_update = q.c_update,
                                           d_update = (q.d_update.HasValue ? q.d_update.Value : Functionals.StandardSqlDateTime),
                                           v_update = qUpdate.v_username,
                                           x_hash = q.x_hash,
                                           v_imgpic = q.v_imgfile,
                                           v_imgwall = q.v_wallpaper,
                                           c_nosup = q.c_nosup,
                                           v_supdesc = qSup.v_nama,
                                           c_kddivpri = q.c_kddivpri,
                                           v_divpridesc = qDivPri.v_nmdivpri,
                                           IsUserPrinsipal = (((q.c_nosup == null) || (q.c_kddivpri == null)) || ((q.c_nosup == "") || (q.c_kddivpri == "")) ?
                                             false : true),
                                           IsUserCabang = false
                                       }).Union(qryCus).AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                List<SCMS_USER_CLASS> lists = null;

                                if ((limit == -1) || allQuery)
                                {
                                    lists = qry.ToList();

                                    if (decryptedString)
                                    {
                                        for (int nLoop = 0; nLoop < lists.Count; nLoop++)
                                        {
                                            lists[nLoop].v_password = (string.IsNullOrEmpty(lists[nLoop].v_password) ? "" : Functionals.DecryptHashRjdnl(lists[nLoop].x_hash, lists[nLoop].v_password));
                                        }
                                    }

                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, lists);
                                }
                                else
                                {
                                    lists = qry.Skip(start).Take(limit).ToList();

                                    if (decryptedString)
                                    {
                                        for (int nLoop = 0; nLoop < lists.Count; nLoop++)
                                        {
                                            lists[nLoop].v_password = (string.IsNullOrEmpty(lists[nLoop].v_password) ? "" : Functionals.DecryptHashRjdnl(lists[nLoop].x_hash, lists[nLoop].v_password));
                                        }
                                    }

                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, lists);
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_GROUPRIGHT

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_GROUPRIGHT:
                        {
                            string strongKey = GlobalCrypto.Crypt1WayMD5String("Mochamad Rudi @ 2011");

                            bool.TryParse((parameters.ContainsKey("decrypted") ?
                              (parameters["decrypted"].Value == null ? "false" : parameters["decrypted"].Value.ToString()) : "false"), out decryptedString);

                            var qryGrp = (from q in db.SCMS_GROUPs
                                          where q.c_group == "Admin"
                                          select q).Union
                                      (from q in db.SCMS_GROUPs
                                       where q.c_group != "Admin"
                                       select q).AsQueryable();

                            var qryGrpUsr = (from q in db.SCMS_GROUPEDs
                                             group q by q.c_grouped into g
                                             select new
                                             {
                                                 c_grouped = g.Key,
                                                 total = g.Count()
                                             });

                            var qry = (from q in qryGrp
                                       join q1 in qryGrpUsr on q.c_group equals q1.c_grouped into q_1
                                       from qGU in q_1.DefaultIfEmpty()
                                       select new SCMS_GROUP_CLASS()
                                       {
                                           c_group = q.c_group,
                                           l_aktif = (q.l_aktif.HasValue ? q.l_aktif.Value : false),
                                           v_akses = (decryptedString ? q.v_akses : null),
                                           v_group_desc = q.v_group_desc,
                                           totalList = (qGU.c_grouped == null ? 0 : qGU.total)
                                       }).AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                List<SCMS_GROUP_CLASS> lists = null;

                                if ((limit == -1) || allQuery)
                                {
                                    lists = qry.ToList();

                                    if (decryptedString)
                                    {
                                        for (int nLoop = 0; nLoop < lists.Count; nLoop++)
                                        {
                                            lists[nLoop].v_akses = (string.IsNullOrEmpty(lists[nLoop].v_akses) ? "" : Functionals.DecryptHashRjdnl(strongKey, lists[nLoop].v_akses));
                                        }
                                    }

                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, lists);
                                }
                                else
                                {
                                    lists = qry.Skip(start).Take(limit).ToList();

                                    if (decryptedString)
                                    {
                                        for (int nLoop = 0; nLoop < lists.Count; nLoop++)
                                        {
                                            lists[nLoop].v_akses = (string.IsNullOrEmpty(lists[nLoop].v_akses) ? "" : Functionals.DecryptHashRjdnl(strongKey, lists[nLoop].v_akses));
                                        }
                                    }

                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, lists);
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_USERGROUPRIGHT

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_USERGROUPRIGHT:
                        {
                            string strongKey = GlobalCrypto.Crypt1WayMD5String("Mochamad Rudi @ 2011");
                            bool.TryParse((parameters.ContainsKey("decrypted") ?
                              (parameters["decrypted"].Value == null ? "false" : parameters["decrypted"].Value.ToString()) : "false"), out decryptedString);

                            var qryCus = (from q in db.SCMS_USERs
                                          join q1 in db.LG_Cusmas on q.c_gdg equals q1.c_cusno into q_1
                                          from qCus in q_1.DefaultIfEmpty()
                                          join q2 in db.SCMS_USERs on q.c_entry equals q2.c_nip into q_2
                                          from qEntry in q_2.DefaultIfEmpty()
                                          join q3 in db.SCMS_USERs on q.c_update equals q3.c_nip into q_3
                                          from qUpdate in q_3.DefaultIfEmpty()
                                          join q4 in db.LG_DatSups on q.c_nosup equals q4.c_nosup into q_4
                                          from qSup in q_4.DefaultIfEmpty()
                                          join q5 in db.FA_MsDivPris on q.c_kddivpri equals q5.c_kddivpri into q_5
                                          from qDivPri in q_5.DefaultIfEmpty()
                                          where (qCus.c_cusno != null)
                                          select new SCMS_USER_CLASS()
                                          {
                                              c_nip = q.c_nip,
                                              c_gdg = q.c_gdg.ToString(),
                                              v_gdgdesc = qCus.v_cunam,
                                              l_aktif = (q.l_aktif.HasValue ? q.l_aktif.Value : false),
                                              v_username = q.v_username,
                                              v_password = q.v_password,
                                              c_entry = q.c_entry,
                                              d_entry = (q.d_entry.HasValue ? q.d_entry.Value : Functionals.StandardSqlDateTime),
                                              v_entry = qEntry.v_username,
                                              c_update = q.c_update,
                                              d_update = (q.d_update.HasValue ? q.d_update.Value : Functionals.StandardSqlDateTime),
                                              v_update = qUpdate.v_username,
                                              x_hash = q.x_hash,
                                              v_imgpic = q.v_imgfile,
                                              v_imgwall = q.v_wallpaper,
                                              c_nosup = q.c_nosup,
                                              v_supdesc = qSup.v_nama,
                                              c_kddivpri = q.c_kddivpri,
                                              v_divpridesc = qDivPri.v_nmdivpri,
                                              IsUserPrinsipal = (((q.c_nosup == null) || (q.c_kddivpri == null)) || ((q.c_nosup == "") || (q.c_kddivpri == "")) ?
                                                false : true),
                                              IsUserCabang = true
                                          });

                            var qryGrpUnion = (from q in db.SCMS_USERs
                                               join q1 in db.LG_MsGudangs on q.c_gdg equals q1.c_gdg.ToString() into q_1
                                               from qGdg in q_1.DefaultIfEmpty()
                                               join q2 in db.SCMS_USERs on q.c_entry equals q2.c_nip into q_2
                                               from qEntry in q_2.DefaultIfEmpty()
                                               join q3 in db.SCMS_USERs on q.c_update equals q3.c_nip into q_3
                                               from qUpdate in q_3.DefaultIfEmpty()
                                               join q4 in db.LG_DatSups on q.c_nosup equals q4.c_nosup into q_4
                                               from qSup in q_4.DefaultIfEmpty()
                                               join q5 in db.FA_MsDivPris on q.c_kddivpri equals q5.c_kddivpri into q_5
                                               from qDivPri in q_5.DefaultIfEmpty()
                                               where (qGdg.c_gdg != null)
                                               select new SCMS_USER_CLASS()
                                               {
                                                   c_nip = q.c_nip,
                                                   c_gdg = q.c_gdg.ToString(),
                                                   v_gdgdesc = qGdg.v_gdgdesc,
                                                   l_aktif = (q.l_aktif.HasValue ? q.l_aktif.Value : false),
                                                   v_username = q.v_username,
                                                   v_password = q.v_password,
                                                   c_entry = q.c_entry,
                                                   d_entry = (q.d_entry.HasValue ? q.d_entry.Value : Functionals.StandardSqlDateTime),
                                                   v_entry = qEntry.v_username,
                                                   c_update = q.c_update,
                                                   d_update = (q.d_update.HasValue ? q.d_update.Value : Functionals.StandardSqlDateTime),
                                                   v_update = qUpdate.v_username,
                                                   x_hash = q.x_hash,
                                                   v_imgpic = q.v_imgfile,
                                                   v_imgwall = q.v_wallpaper,
                                                   c_nosup = q.c_nosup,
                                                   v_supdesc = qSup.v_nama,
                                                   c_kddivpri = q.c_kddivpri,
                                                   v_divpridesc = qDivPri.v_nmdivpri,
                                                   IsUserPrinsipal = (((q.c_nosup == null) || (q.c_kddivpri == null)) || ((q.c_nosup == "") || (q.c_kddivpri == "")) ?
                                                     false : true),
                                                   IsUserCabang = false
                                               }).Union(qryCus).AsQueryable();

                            var qry = (from q in qryGrpUnion
                                       join q1 in db.SCMS_GROUPEDs on q.c_nip equals q1.c_nip into q_1
                                       from qGrpd in q_1.DefaultIfEmpty()
                                       join q2 in db.SCMS_GROUPs on qGrpd.c_grouped equals q2.c_group into q_2
                                       from qGrp in q_2.DefaultIfEmpty()
                                       select new SCMS_USERGROUP_CLASS()
                                       {
                                           c_nip = q.c_nip,
                                           c_gdg = q.c_gdg.ToString(),
                                           v_gdgdesc = q.v_gdgdesc,
                                           l_aktif = q.l_aktif,
                                           v_username = q.v_username,
                                           v_password = q.v_password,
                                           x_hash = q.x_hash,
                                           l_aktif_group = (qGrp.l_aktif.HasValue ? qGrp.l_aktif.Value : false),
                                           v_akses = qGrp.v_akses,
                                           v_group_desc = qGrp.v_group_desc,
                                           c_group = qGrp.c_group,
                                           v_imgpic = q.v_imgpic,
                                           v_imgwall = q.v_imgwall,
                                           c_nosup = q.c_nosup,
                                           v_supdesc = q.v_supdesc,
                                           c_kddivpri = q.c_kddivpri,
                                           v_divpridesc = q.v_divpridesc,
                                           IsUserPrinsipal = q.IsUserPrinsipal,
                                           IsUserCabang = q.IsUserCabang
                                       });

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                List<SCMS_USERGROUP_CLASS> lists = null;

                                if ((limit == -1) || allQuery)
                                {
                                    lists = qry.ToList();

                                    if (decryptedString)
                                    {
                                        for (int nLoop = 0; nLoop < lists.Count; nLoop++)
                                        {
                                            lists[nLoop].v_password = GlobalCrypto.Crypt1WayMD5String(string.IsNullOrEmpty(lists[nLoop].v_password) ? "" : Functionals.DecryptHashRjdnl(lists[nLoop].x_hash, lists[nLoop].v_password));
                                            lists[nLoop].v_akses = (string.IsNullOrEmpty(lists[nLoop].v_akses) ? "" : Functionals.DecryptHashRjdnl(strongKey, lists[nLoop].v_akses));
                                        }
                                    }

                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, lists);
                                }
                                else
                                {
                                    lists = qry.Skip(start).Take(limit).ToList();

                                    if (decryptedString)
                                    {
                                        for (int nLoop = 0; nLoop < lists.Count; nLoop++)
                                        {
                                            lists[nLoop].v_password = GlobalCrypto.Crypt1WayMD5String(string.IsNullOrEmpty(lists[nLoop].v_password) ? "" : Functionals.DecryptHashRjdnl(lists[nLoop].x_hash, lists[nLoop].v_password));
                                        }
                                    }

                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, lists);
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_USERGROUPRIGHT_USER

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_USERGROUPRIGHT_USER:
                        {
                            var qry = (from q in db.SCMS_GROUPEDs
                                       join q1 in db.SCMS_GROUPs on q.c_grouped equals q1.c_group into q_1
                                       from qGrp in q_1.DefaultIfEmpty()
                                       join q2 in db.SCMS_USERs on q.c_nip equals q2.c_nip into q_2
                                       from qUsr in q_2.DefaultIfEmpty()
                                       select new
                                       {
                                           qUsr.c_nip,
                                           qUsr.v_username,
                                           qGrp.c_group,
                                           qGrp.v_group_desc,
                                           qGrp.v_akses
                                       });

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_USERGROUPRIGHT_USER_AVAIBLE

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_USERGROUPRIGHT_USER_AVAIBLE:
                        {
                            string uNip = (parameters.ContainsKey("c_nip") ? (string)((Functionals.ParameterParser)parameters["c_nip"]).Value : string.Empty);

                            var qryX = (from q in db.SCMS_GROUPEDs
                                        join q1 in db.SCMS_GROUPs on q.c_grouped equals q1.c_group into q_1
                                        from qGrp in q_1.DefaultIfEmpty()
                                        join q2 in db.SCMS_USERs on q.c_nip equals q2.c_nip into q_2
                                        from qUsr in q_2.DefaultIfEmpty()
                                        select new
                                        {
                                            qUsr.c_nip,
                                            qUsr.v_username,
                                            qGrp.c_group,
                                            qGrp.v_group_desc,
                                            qGrp.v_akses
                                        });

                            var qry = (from q in db.SCMS_GROUPs
                                       where !(from sq1 in qryX
                                               where sq1.c_nip == uNip
                                               select sq1.c_group).Contains(q.c_group)
                                       select new
                                       {
                                           q.l_aktif,
                                           q.c_group,
                                           q.v_group_desc
                                       });

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_USERGROUPRIGHT_GROUP

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_USERGROUPRIGHT_GROUP:
                        {
                            string cGrup = (parameters.ContainsKey("c_group") ? (string)((Functionals.ParameterParser)parameters["c_group"]).Value : string.Empty);

                            var qry = (from q in db.SCMS_GROUPEDs
                                       join q1 in db.SCMS_USERs on q.c_nip equals q1.c_nip into q_1
                                       from qUsr in q_1.DefaultIfEmpty()
                                       join q2 in db.SCMS_GROUPs on q.c_grouped equals q2.c_group into q_2
                                       from qGrp in q_2.DefaultIfEmpty()
                                       where qGrp.c_group == cGrup
                                       select new
                                       {
                                           qUsr.c_nip,
                                           qUsr.v_username,
                                           qUsr.l_aktif
                                       }).AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_USERGROUPRIGHT_GROUP_AVAIBLE

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_USERGROUPRIGHT_GROUP_AVAIBLE:
                        {
                            string cGrup = (parameters.ContainsKey("c_group") ? (string)((Functionals.ParameterParser)parameters["c_group"]).Value : string.Empty);

                            var qryX = (from q in db.SCMS_GROUPEDs
                                        join q1 in db.SCMS_USERs on q.c_nip equals q1.c_nip into q_1
                                        from qUsr in q_1.DefaultIfEmpty()
                                        join q2 in db.SCMS_GROUPs on q.c_grouped equals q2.c_group into q_2
                                        from qGrp in q_2.DefaultIfEmpty()
                                        where qGrp.c_group == cGrup
                                        select new
                                        {
                                            qUsr.c_nip,
                                            qUsr.v_username,
                                            qUsr.l_aktif
                                        }).AsQueryable();

                            var qry = (from q in db.SCMS_USERs
                                       where !(from sq1 in qryX
                                               select sq1.c_nip).Contains(q.c_nip)
                                       select new
                                       {
                                           q.l_aktif,
                                           q.c_nip,
                                           q.v_username
                                       });

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(
                  "ScmsSoaLibrary.Modules.CommonQueryMulti:ModelGridQueryRightManagement (First) <-> Switch {0} - {1}", model, ex.Message);
                Logger.WriteLine(ex.StackTrace);

                dic.Add(Constant.DEFAULT_NAMING_EXCEPTION, ex.Message);

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);
            }

            db.Dispose();

            return dic;
        }

        public static IDictionary<string, object> ModelGridQuery(string connectionString, int start, int limit, bool allQuery, string sort, string dir, string model, IDictionary<string, Functionals.ParameterParser> parameters)
        {
            IDictionary<string, object> dic = new Dictionary<string, object>();

            ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);
            //db.CommandTimeout = 1000;

            int nCount = 0;
            string paternLike = null;

            try
            {
                switch (model)
                {
                    #region SJ

                    #region MODEL_COMMON_QUERY_MULTIPLE_SUPLIER_SJ

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_SUPLIER_SJ:
                        {
                            DateTime date = DateTime.Today;

                            char gdg = (parameters.ContainsKey("gdgFrom") ? (char)((Functionals.ParameterParser)parameters["gdgFrom"]).Value : '?');

                            var qrySG = (from spgh in db.LG_SPGHs
                                         join spgd1 in db.LG_SPGD1s on spgh.c_spgno equals spgd1.c_spgno
                                         join sup in db.LG_DatSups on spgh.c_nosup equals sup.c_nosup
                                         where spgh.l_status == true && spgd1.n_sisa > 0
                                         && (SqlMethods.DateDiffMonth(spgh.d_spgdate, date) < 2)
                                         select new
                                         {
                                             c_nosup = sup.c_nosup,
                                             v_nama = sup.v_nama,
                                             spgd1.c_iteno,
                                             spgh.c_gdg2,
                                             sup.l_aktif,
                                             sup.l_hide
                                         }).Distinct().AsQueryable();
                            nCount = qrySG.Count();
                            var qry = (from q in qrySG
                                       join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                       join q4 in
                                           GlobalQuery.ViewStockLite(db, gdg) on new { c_gdg = q.c_gdg2.HasValue ? q.c_gdg2.Value : '1', q.c_iteno } equals new { q4.c_gdg, q4.c_iteno }
                                       where q1.l_aktif == true
                                        && q4.c_gdg == gdg && q4.n_gsisa != 0 //&& q2.c_nosup == "00046"
                                       select new
                                       {
                                           q.c_nosup,
                                           q.v_nama,
                                           q.l_aktif,
                                           q.l_hide,
                                       }).Distinct().AsQueryable();


                            //var qry1 = (from spgh in db.LG_SPGHs
                            //           join spgd1 in db.LG_SPGD1s on spgh.c_spgno equals spgd1.c_spgno
                            //           join sup in db.LG_DatSups on spgh.c_nosup equals sup.c_nosup
                            //           where spgh.l_status == true && spgd1.n_sisa > 0
                            //           && (SqlMethods.DateDiffMonth(spgh.d_spgdate, date) < 2)
                            //           select new
                            //           {
                            //             c_nosup = sup.c_nosup,
                            //             v_nama = sup.v_nama,
                            //             sup.l_aktif,
                            //             sup.l_hide
                            //           }).Distinct().ToList();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_SUPLIER_SJ_GUDANG_3

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_SUPLIER_SJ_GUDANG_3:
                        {
                            DateTime date = DateTime.Today;

                            char gdg = (parameters.ContainsKey("gdgFrom") ? (char)((Functionals.ParameterParser)parameters["gdgFrom"]).Value : '?');

                            var qry = (from spgh in db.LG_SPGHs
                                         join spgd1 in db.LG_SPGD1s on spgh.c_spgno equals spgd1.c_spgno
                                         join sup in db.LG_DatSups on spgh.c_nosup equals sup.c_nosup
                                         where spgh.l_status == true && spgd1.n_sisa > 0
                                         && (SqlMethods.DateDiffMonth(spgh.d_spgdate, date) < 2)
                                         select new
                                         {
                                             c_nosup = sup.c_nosup,
                                             v_nama = sup.v_nama,
                                             spgd1.c_iteno,
                                             spgh.c_gdg2,
                                             sup.l_aktif,
                                             sup.l_hide
                                         }).Distinct().AsQueryable();
                           nCount = qry.Count();

                            //var qry1 = (from spgh in db.LG_SPGHs
                            //           join spgd1 in db.LG_SPGD1s on spgh.c_spgno equals spgd1.c_spgno
                            //           join sup in db.LG_DatSups on spgh.c_nosup equals sup.c_nosup
                            //           where spgh.l_status == true && spgd1.n_sisa > 0
                            //           && (SqlMethods.DateDiffMonth(spgh.d_spgdate, date) < 2)
                            //           select new
                            //           {
                            //             c_nosup = sup.c_nosup,
                            //             v_nama = sup.v_nama,
                            //             sup.l_aktif,
                            //             sup.l_hide
                            //           }).Distinct().ToList();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            if (kvp.Key == "c_nosup.Contains(@0) || v_nama.Contains(@0)")
                                            { }
                                            else
                                            {
                                                qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                            }
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_SJ

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_SJ:
                        {
                            string gdgTo = (parameters.ContainsKey("gdgTo") ? (string)((Functionals.ParameterParser)parameters["gdgTo"]).Value : string.Empty);
                            char gdgFrom = (parameters.ContainsKey("gdgFrom") ? (char)((Functionals.ParameterParser)parameters["gdgFrom"]).Value : '?');
                            string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);
                            string itemCat = (parameters.ContainsKey("itemCat") ? (string)((Functionals.ParameterParser)parameters["itemCat"]).Value : string.Empty);
                            string itemLat = (parameters.ContainsKey("itemLat") ? (string)((Functionals.ParameterParser)parameters["itemLat"]).Value : string.Empty);

                            DateTime date = DateTime.Today;

                            #region Old Coded

                            //var ItemSJ = (from itm in db.FA_MasItms
                            //             join sjd1 in db.LG_SJD1s on  itm.c_iteno equals sjd1.c_iteno into sjd
                            //             from sjd1 in sjd.DefaultIfEmpty()
                            //              where sjd1.c_gdg == gdg
                            //             select new
                            //             {
                            //                 c_iteno = itm.c_iteno,
                            //                 v_itnam = itm.v_itnam
                            //             }).Distinct().AsQueryable();

                            //var itemqry = (from spgh in db.LG_SPGHs
                            //               join spgd1 in db.LG_SPGD1s on spgh.c_spgno equals spgd1.c_spgno
                            //               join itmsj in ItemSJ on spgd1.c_iteno equals itmsj.c_iteno
                            //               join vwstock in
                            //                 (from vwstock1 in GlobalQuery.ViewStockLite(db)
                            //                  group vwstock1 by new { vwstock1.c_gdg, vwstock1.c_iteno } into g
                            //                  select new
                            //                  {
                            //                    g.Key.c_gdg,
                            //                    g.Key.c_iteno,
                            //                    n_soh = g.Sum(x => x.n_gsisa)
                            //                  }) on
                            //                 new { c_itenosg = spgd1.c_iteno, c_itenosj = itmsj.c_iteno, c_gdgsp = spgh.c_gdg2.Value } equals
                            //                 new { c_itenosg = vwstock.c_iteno, c_itenosj = vwstock.c_iteno, c_gdgsp = vwstock.c_gdg }
                            //               where spgh.l_status == true && spgd1.n_sisa > 0 && spgh.c_nosup == nosup && vwstock.n_soh > 0
                            //               select new
                            //               {
                            //                 c_iteno = itmsj.c_iteno,
                            //                 v_itnam = itmsj.v_itnam,
                            //                 n_soh = vwstock.n_soh
                            //               }).Distinct().AsQueryable();

                            //var qry = (from sgpend in
                            //             (from sgpend1 in GlobalQuery.ViewStockLiteSPPendingNew(db)
                            //              where sgpend1.c_cusno == gdg && sgpend1.c_type == "SG"
                            //              group sgpend1 by sgpend1.c_iteno into g
                            //              select new
                            //              {
                            //                c_iteno = g.Key,
                            //                n_sgpending = g.Sum(x => x.n_pending)
                            //              })
                            //           join vwstock in
                            //             (from vwstock1 in GlobalQuery.ViewStockLite(db)
                            //              group vwstock1 by new { vwstock1.c_gdg, vwstock1.c_iteno } into g
                            //              where g.Key.c_gdg == gdgFrom
                            //              select new
                            //              {
                            //                g.Key.c_gdg,
                            //                g.Key.c_iteno,
                            //                n_soh = g.Sum(x => x.n_gsisa)
                            //              }) on sgpend.c_iteno equals vwstock.c_iteno
                            //           join item in db.FA_MasItms on vwstock.c_iteno equals item.c_iteno
                            //           where item.c_nosup == nosup
                            //           select new
                            //           {
                            //             c_iteno = sgpend.c_iteno,
                            //             v_itnam = item.v_itnam,
                            //             n_sgpending = sgpend.n_sgpending,
                            //             n_soh = vwstock.n_soh
                            //           }).Distinct().AsQueryable();

                            #endregion
                            var qry2 = (from q in db.FA_MasItms
                                       where q.l_aktif == true && q.c_nosup == nosup
                                       select new
                                       {
                                           c_iteno = q.c_iteno,
                                           v_itnam = q.v_itnam,
                                           n_box = q.n_box
                                       }).Distinct().AsQueryable();
                            //if (gdgTo == "6")
                            //{ 
                            //    var qry = (from q in db.FA_MasItms where q.l_aktif == true
                            //           select new
                            //           {
                            //               c_iteno = q.c_iteno,
                            //               v_itnam = q.v_itnam,
                            //               n_box = q.n_box
                            //           }).Distinct().AsQueryable();
                            //}
                            //else
                            //{
                                var qry = (from sgh in
                                           (from sgpend1 in GlobalQuery.ViewStockSPPendingNew(db)
                                            where sgpend1.c_cusno == gdgTo && sgpend1.c_type == "SG"
                                            group sgpend1 by sgpend1.c_iteno into g
                                            select new
                                            {
                                                c_iteno = g.Key,
                                                n_sgpending = g.Sum(x => x.n_pending)
                                            })
                                       join item in db.FA_MasItms on sgh.c_iteno equals item.c_iteno
                                       join q4 in
                                           (from sq in GlobalQuery.ViewStockLite(db, gdgFrom)
                                            group sq by sq.c_iteno into g
                                            where (g.Sum(y => y.n_gsisa) > 0)
                                            select new { c_iteno = g.Key }) on item.c_iteno equals q4.c_iteno
                                       where //(item.c_nosup == nosup) && 
                                        (string.IsNullOrEmpty(nosup) ? true : (item.c_nosup == nosup)) &&
                                       (sgh.n_sgpending > 0)
                                       && (from sq in db.SCMS_MSITEM_CATs
                                           where (sq.c_type == (string.IsNullOrEmpty(itemCat) || (itemCat == "00") ? sq.c_type : itemCat))
                                           select new PLItemCategory()
                                           {
                                               c_iteno = (string.IsNullOrEmpty(itemCat) || (itemCat == "00") ? sgh.c_iteno : sq.c_iteno)
                                           }).Contains(new PLItemCategory() { c_iteno = sgh.c_iteno })
                                       && (string.IsNullOrEmpty(itemLat) ? true : (from sq in db.SCMS_MSITEM_LATs
                                                                                   where (sq.c_type_lat == (string.IsNullOrEmpty(itemLat) || (itemLat == "00") ? sq.c_type_lat : itemLat))
                                                                                        && sq.c_gdg == gdgFrom
                                                                                   select new PLItemCategory()
                                                                                   {
                                                                                       c_iteno = (string.IsNullOrEmpty(itemLat) || (itemCat == "00") ? sgh.c_iteno : sq.c_iteno)
                                                                                   }).Contains(new PLItemCategory() { c_iteno = sgh.c_iteno }))
                                       select new
                                       {
                                           c_iteno = sgh.c_iteno,
                                           v_itnam = item.v_itnam,
                                           n_box = item.n_box
                                       }).Distinct().AsQueryable();
                            //}


                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());
                            
                            if (gdgTo == "6")
                            {
                                qry = qry2;
                            }

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_SGDETAIL_SJ

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_SGDETAIL_SJ:
                        {
                            char gdgTo = (parameters.ContainsKey("gdgTo") ? (char)((Functionals.ParameterParser)parameters["gdgTo"]).Value : '?');
                            string iteno = (parameters.ContainsKey("iteno") ? (string)((Functionals.ParameterParser)parameters["iteno"]).Value : string.Empty);

                            DateTime date = DateTime.Today;

                            var qry = (from sgh in db.LG_SPGHs
                                       join sgd1 in db.LG_SPGD1s on new { c_gdg = sgh.c_gdg1, c_spgno = sgh.c_spgno } equals new { c_gdg = sgd1.c_gdg, c_spgno = sgd1.c_spgno }
                                       join sg in db.VW_MAXETDSGs on sgh.c_spgno equals sg.C_SPGNO
                                       where (sgh.c_gdg1 == gdgTo) && (sgh.l_status == true)
                                        && (sgd1.c_iteno == iteno) && (sgd1.n_sisa > 0)
                                        && ((sgh.l_delete.HasValue ? sgh.l_delete.Value : false) == false)
                                        && (SqlMethods.DateDiffMonth(sg.ETA, date) < 2)
                                       select new
                                       {
                                           c_spgno = sgh.c_spgno,
                                           d_spgdate = sgh.d_spgdate,
                                           n_sisa = sgd1.n_sisa
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }
                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }

                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_SJ_AUTOGEN

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_SJ_AUTOGEN:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            char gdgTo = (parameters.ContainsKey("gdgTo") ? (char)((Functionals.ParameterParser)parameters["gdgTo"]).Value : '2');
                            string supl = (parameters.ContainsKey("supl") ? (string)((Functionals.ParameterParser)parameters["supl"]).Value : string.Empty);
                            string itemCat = (parameters.ContainsKey("itemCat") ? (string)((Functionals.ParameterParser)parameters["itemCat"]).Value : string.Empty);
                            string itemLat = (parameters.ContainsKey("itemLat") ? (string)((Functionals.ParameterParser)parameters["itemLat"]).Value : string.Empty);
                            string tipeSJ = (parameters.ContainsKey("tipeSJ") ? (string)((Functionals.ParameterParser)parameters["tipeSJ"]).Value : string.Empty);
                            //string tipeSJ = "BOX";

                            List<string> lstItems = null;
                            List<ItemStockAvaible> listStock = null,
                              listStockCount = null;
                            ItemStockAvaible listStockAlloc = null;
                            List<SJDetailGenerator> listSPTemp = null,
                              listSP = null;
                            int nLoop = 0,
                              nLoopC = 0;
                            decimal nAvaible = 0,
                               qtyAlloc = 0,
                               qtyRelloc = 0,
                               qtyTotal = 0,
                               cekTotal = 0;

                            DateTime date = DateTime.Now;

                            ItemStockAvaible isa = null;

                            listSPTemp = (from q in db.LG_SPGHs
                                          join q1 in db.LG_SPGD1s on q.c_spgno equals q1.c_spgno
                                          join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
                                          where (q1.n_sisa > 0) && (q.c_gdg1 == gdgTo) && (q.l_status == true)
                                          && q.c_nosup == supl
                                           && (from sq in db.SCMS_MSITEM_CATs
                                               where (sq.c_type == (string.IsNullOrEmpty(itemCat) || (itemCat == "00") ? sq.c_type : itemCat))
                                               select new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory()
                                               {
                                                   c_iteno = (string.IsNullOrEmpty(itemCat) || (itemCat == "00") ? q1.c_iteno : sq.c_iteno)
                                               }).Contains(new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory() { c_iteno = q1.c_iteno })

                                           && (string.IsNullOrEmpty(itemLat) ? true : (from sq in db.SCMS_MSITEM_LATs
                                                                                       where (sq.c_type_lat == (string.IsNullOrEmpty(itemLat) || (itemLat == "00") ? sq.c_type_lat : itemLat))
                                                                                            && sq.c_gdg ==gdg
                                                                                       select new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory()
                                                                                       {
                                                                                           c_iteno = (string.IsNullOrEmpty(itemLat) || (itemCat == "00") ? q1.c_iteno : sq.c_iteno)
                                                                                       }).Contains(new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory() { c_iteno = q1.c_iteno }))
                                           && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                           && (SqlMethods.DateDiffMonth(q.d_spgdate, DateTime.Now.Date) < 2)
                                          select new SJDetailGenerator()
                                          {
                                              c_spgno = q.c_spgno,
                                              c_iteno = q1.c_iteno,
                                              gudang = q.c_gdg2.HasValue ? q.c_gdg2.Value : '1',
                                              v_itemdesc = q2.v_itnam,
                                              n_sisa = (q1.n_sisa.HasValue ? q1.n_sisa.Value : 0),
                                              n_box = (q2.n_box.HasValue ? q2.n_box.Value : 0),
                                          }).Distinct().ToList();

                            if (listSPTemp.Count > 0)
                            {
                                lstItems = listSPTemp.GroupBy(t => new { t.gudang, t.c_iteno }).Select(t => t.Key.c_iteno).ToList();

                                listStock = (from q in GlobalQuery.ViewStockLiteContains(db, gdg, lstItems.ToArray())
                                             join q1 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q1.c_iteno, q1.c_batch }
                                             where (q.n_gsisa != 0)
                                             group q by new { q.c_iteno, q.c_batch, q1.d_expired } into g
                                             select new ItemStockAvaible()
                                             {
                                                 c_iteno = g.Key.c_iteno,
                                                 c_batch = (g.Key.c_batch == null ? string.Empty : g.Key.c_batch.Trim()),
                                                 n_gsisa = g.Sum(t => t.n_gsisa),
                                                 d_expired = (g.Key.d_expired.HasValue ? g.Key.d_expired.Value : Functionals.StandardSqlDateTime)
                                             }).Distinct().ToList();

                                lstItems.Clear();

                                listSP = new List<SJDetailGenerator>();

                                for (nLoopC = 0; nLoopC < listSPTemp.Count; nLoopC++)
                                {
                                    SJDetailGenerator sjdg = listSPTemp[nLoopC];

                                    nAvaible = listStock.Where(t => t.c_iteno == sjdg.c_iteno)
                                      .GroupBy(t => t.c_iteno).Sum(x => x.Sum(y => y.n_gsisa));

                                    if (nAvaible <= 0)
                                    {
                                        continue;
                                    }

                                    if (listSP.Count > 0)
                                    {
                                        qtyTotal = listSP.Where(t => t.c_iteno == sjdg.c_iteno)
                                        .GroupBy(t => t.c_iteno).Sum(x => x.Sum(y => y.n_sisa));

                                        if (qtyTotal >= nAvaible)
                                        {
                                            continue;
                                        }
                                        if (listSP.Count >= 8)
                                        {
                                            continue;
                                        }
                                    }

                                    nAvaible = (sjdg.n_sisa > nAvaible ? nAvaible : sjdg.n_sisa);

                                    listStockCount = listStock.Where(t => t.c_iteno == sjdg.c_iteno).ToList();

                                    for (nLoop = 0; nLoop < listStockCount.Count(); nLoop++)
                                    {
                                        listStockAlloc = listStockCount[nLoop];

                                        qtyAlloc = listStockAlloc.n_gsisa;

                                        if (qtyAlloc <= 0)
                                        {
                                            continue;
                                        }
                                        if (nAvaible <= 0)
                                        {
                                            break;
                                        }

                                        if (listSP.Count > 0)
                                        {
                                            qtyTotal = listSP.Where(t => t.c_iteno == sjdg.c_iteno
                                              && t.c_batch == listStockAlloc.c_batch)
                                            .GroupBy(t => new { t.c_iteno, t.c_batch }).Sum(x => x.Sum(y => y.n_sisa));

                                            if (qtyTotal >= listStockAlloc.n_gsisa)
                                            {
                                                continue;
                                            }
                                            if (listSP.Count >= 8)
                                            {
                                                continue;
                                            }
                                        }
                                        if (nAvaible >= sjdg.n_sisa)
                                        {
                                            cekTotal = qtyTotal + sjdg.n_sisa;
                                        }

                                        if (cekTotal >= qtyAlloc)
                                        {
                                            cekTotal -= qtyAlloc;
                                        }
                                        else
                                        {
                                            cekTotal = nAvaible;
                                        }

                                        qtyRelloc = (qtyAlloc > nAvaible ? cekTotal : qtyAlloc);

                                        if (tipeSJ == "01")
                                        {
                                            var xKali = Math.Floor(qtyRelloc / sjdg.n_box);
                                            if (xKali <= 0)
                                            {
                                                continue;
                                            }

                                            qtyRelloc = xKali * sjdg.n_box;
                                        }

                                        listSP.Add(new SJDetailGenerator()
                                        {
                                            c_spgno = sjdg.c_spgno,
                                            c_iteno = sjdg.c_iteno,
                                            v_itemdesc = sjdg.v_itemdesc,
                                            c_batch = listStockAlloc.c_batch,
                                            n_sisa = qtyRelloc,
                                            d_expired = listStockAlloc.d_expired,
                                            l_expired = listStockAlloc.d_expired < date.AddYears(1) ? true : false
                                        });

                                        nAvaible -= qtyRelloc;

                                    }

                                }

                                #region Original

                                //listSPTemp.ForEach(delegate(SJDetailGenerator sjdg)
                                //{
                                //  while (sjdg.n_sisa > 0)
                                //  {
                                //    nAvaible = listStock.Where(t => t.c_iteno == sjdg.c_iteno)
                                //      .GroupBy(t => t.c_iteno).Sum(x => x.Sum(y => y.n_gsisa));
                                //    if (nAvaible <= 0)
                                //    {
                                //      break;
                                //    }

                                //    isa = listStock.Where(x => sjdg.c_iteno.Equals(x.c_iteno, StringComparison.OrdinalIgnoreCase)).Take(1).SingleOrDefault();
                                //    if (isa == null)
                                //    {
                                //      break;
                                //    }
                                //    else if (isa.n_gsisa <= 0)
                                //    {
                                //      listStock.Remove(isa);
                                //      break;
                                //    }

                                //    nAvaible = (sjdg.n_sisa > isa.n_gsisa ? isa.n_gsisa : sjdg.n_sisa);

                                //    if (nAvaible <= 0)
                                //    {
                                //      break;
                                //    }

                                //    listSP.Add(new SJDetailGenerator()
                                //    {
                                //      c_spgno = sjdg.c_spgno,
                                //      c_iteno = sjdg.c_iteno,
                                //      v_itemdesc = sjdg.v_itemdesc,
                                //      c_batch = isa.c_batch,
                                //      n_sisa = nAvaible
                                //    });

                                //    sjdg.n_sisa -= nAvaible;
                                //    isa.n_gsisa -= nAvaible;

                                //    if (isa.n_gsisa <= 0)
                                //    {
                                //      listStock.Remove(isa);
                                //    }

                                //    if (sjdg.n_sisa < 0)
                                //    {
                                //      break;
                                //    }
                                //  }  
                                //});

                                #endregion

                                nCount = listSP.Count;

                                if (nCount > 0)
                                {
                                    if (limit == -1)
                                    {
                                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, listSP.ToArray());
                                    }
                                    else
                                    {
                                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, listSP.Take(limit).ToArray());
                                    }
                                }

                                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                                listStock.Clear();
                                listSP.Clear();
                            }
                            else
                            {
                                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            listSPTemp.Clear();
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_BATCHDETAIL_SJ

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_BATCHDETAIL_SJ:
                        {
                            char gdgTo = (parameters.ContainsKey("gdgTo") ? (char)((Functionals.ParameterParser)parameters["gdgTo"]).Value : '?');
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string spgno = (parameters.ContainsKey("spgno") ? (string)((Functionals.ParameterParser)parameters["spgno"]).Value : string.Empty);
                            string iteno = (parameters.ContainsKey("iteno") ? (string)((Functionals.ParameterParser)parameters["iteno"]).Value : string.Empty);
                            string tipeSJ = (parameters.ContainsKey("tipeSJ") ? (string)((Functionals.ParameterParser)parameters["tipeSJ"]).Value : string.Empty);
                            var qry2 = (from q in db.LG_RND1s
                                        join q1 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q1.c_iteno, q1.c_batch }
                                        where q.c_iteno == iteno && q.n_gsisa != 0 && q.c_gdg.ToString() == gdg.ToString()
                                        select new 
                                        {
                                            q.c_iteno,
                                            q.c_batch,
                                            q1.d_expired,
                                            q.n_gsisa,
                                        });
                            var qry3 = (from qs in
                                            (from s in qry2
                                             group s by new { s.c_iteno, s.c_batch } into g
                                             select new
                                             {
                                                 g.Key.c_batch,
                                                 g.Key.c_iteno,
                                                 n_soh = g.Sum(x => x.n_gsisa)
                                             })
                                             join qm in db.LG_MsBatches on new {qs.c_batch, qs.c_iteno} equals new {qm.c_batch,qm.c_iteno}
                                             where (qs.n_soh != 0)
                                        select new SJDetailBatch2{ 
                                        c_iteno = qs.c_iteno,
                                        c_batch = qs.c_batch,
                                        d_expired = qm.d_expired,
                                        n_soh = qs.n_soh.Value,
                                        n_box = 0
                                        });
                            var qry = (from qVWS in
                                           (from sq in GlobalQuery.ViewStockLiteContains(db, gdg, iteno)
                                            group sq by new { sq.c_gdg, sq.c_iteno, sq.c_batch } into g
                                            select new
                                            {
                                                g.Key.c_gdg,
                                                g.Key.c_iteno,
                                                g.Key.c_batch,
                                                n_soh = g.Sum(x => x.n_gsisa)
                                            })
                                       join q1 in db.LG_MsBatches on new { qVWS.c_iteno, qVWS.c_batch } equals new { q1.c_iteno, q1.c_batch }
                                       where (qVWS.n_soh != 0)
                                       select new SJDetailBatch
                                       {
                                           c_iteno = q1.c_iteno,
                                           c_batch = q1.c_batch,
                                           d_expired = q1.d_expired,
                                           n_soh = qVWS.n_soh,
                                           n_box = 0
                                       });

                            if (tipeSJ == "01")
                            {
                                qry = (from q1 in qry
                                       join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
                                       where q1.n_soh >= q2.n_box
                                       select new SJDetailBatch
                                        {
                                            c_iteno = q1.c_iteno,
                                            c_batch = q1.c_batch,
                                            d_expired = q1.d_expired,
                                            n_soh = q1.n_soh,
                                            n_box = q2.n_box
                                        });
                            }
                            #region Old Coded

                            //var sj = from sjd1 in db.LG_SJD1s
                            //         where sjd1.c_spgno == spgno
                            //         select new
                            //         {
                            //           c_gdg = sjd1.c_gdg,
                            //           c_iteno = sjd1.c_iteno,
                            //           c_batch = sjd1.c_batch
                            //         };

                            //var qrySJ = (from vwstock in
                            //               (from vwstock1 in GlobalQuery.ViewStockLite(db)
                            //                group vwstock1 by new { vwstock1.c_gdg, vwstock1.c_iteno, vwstock1.c_batch } into g
                            //                select new
                            //                {
                            //                  c_gdg = g.Key.c_gdg,
                            //                  c_iteno = g.Key.c_iteno,
                            //                  c_batch = g.Key.c_batch,
                            //                  n_soh = g.Sum(x => x.n_gsisa)
                            //                })
                            //             join batch in db.LG_MsBatches on new { c_iteno = vwstock.c_iteno, c_batch = vwstock.c_batch } equals new { c_iteno = batch.c_iteno, c_batch = batch.c_batch }
                            //             join sjdq in sj on new
                            //                                {
                            //                                  vwstock.c_gdg,
                            //                                  batch.c_iteno,
                            //                                  batch.c_batch
                            //                                }
                            //                                equals new
                            //                                {
                            //                                  sjdq.c_gdg,
                            //                                  sjdq.c_iteno,
                            //                                  sjdq.c_batch
                            //                                } into sjdet
                            //             from sjd in sjdet.DefaultIfEmpty()
                            //             where vwstock.n_soh != 0 && vwstock.c_iteno == iteno && vwstock.c_gdg == gdg
                            //             select new
                            //             {
                            //               vwstock.c_gdg,
                            //               vwstock.c_iteno,
                            //               batch.c_batch,
                            //               batch.d_expired,
                            //               n_soh = vwstock.n_soh
                            //             }
                            //            ).Distinct().AsQueryable();

                            //var qry = (from pendSG in GlobalQuery.ViewStockSPPendingNew(db)
                            //           join qryS in qrySJ on pendSG.c_iteno equals qryS.c_iteno
                            //           join vwstock in
                            //             (from vwstock1 in GlobalQuery.ViewStockLite(db)
                            //              where vwstock1.c_gdg == gdg
                            //              group vwstock1 by new
                            //              {
                            //                vwstock1.c_gdg,
                            //                vwstock1.c_iteno

                            //              } into g
                            //              select new
                            //              {
                            //                c_gdg = g.Key.c_gdg,
                            //                c_iteno = g.Key.c_iteno,
                            //                n_sohTot = g.Sum(x => x.n_gsisa)
                            //              }
                            //              ) on pendSG.c_iteno equals vwstock.c_iteno
                            //           where pendSG.c_spno == spgno
                            //           select new
                            //           {
                            //             c_gdg = qryS.c_gdg,
                            //             c_iteno = qryS.c_iteno,
                            //             c_batch = qryS.c_batch,
                            //             d_expired = qryS.d_expired,
                            //             n_soh = qryS.n_soh,
                            //             n_pending = pendSG.n_pending,
                            //             n_sohTot = vwstock.n_sohTot
                            //           }).Distinct().AsQueryable();

                            #endregion

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            if (gdgTo.ToString() != "6")
                                            {
                                                qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                            }
                                            else if (gdgTo.ToString() == "6")
                                            {
                                                qry3 = qry3.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                            }
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());
                            if (gdgTo.ToString() == "6")
                            {
                                nCount = qry3.Count();
                            }
                            else
                            {
                                nCount = qry.Count();
                            }

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    if (gdgTo.ToString() != "6")
                                    {
                                        qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                    }
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    if (gdgTo.ToString() != "6")
                                    {
                                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                    }
                                    else if (gdgTo.ToString() == "6")
                                    {
                                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry3.ToList());
                                    }
                                }
                                else
                                {
                                    if (gdgTo.ToString() != "6")
                                    {
                                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                    }
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_BATCHDETAIL_SJ_REPACK

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_BATCHDETAIL_SJ_REPACK:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string item = (parameters.ContainsKey("iteno") ? (string)((Functionals.ParameterParser)parameters["iteno"]).Value : string.Empty);

                            //var qry = (from q in GlobalQuery.ViewStockBatch(db, gdg, item)
                            //           join q1 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q1.c_iteno, q1.c_batch }
                            //           //where q.c_gdg == gdg && q.c_iteno == iteno
                            //           select new
                            //           {
                            //             q.c_gdg,
                            //             q.c_iteno,
                            //             q.c_batch,
                            //             q1.d_expired,
                            //             q.N_BSISA,
                            //             q.N_GSISA
                            //           }).Distinct().AsQueryable();

                            var qry = (from qVWS in
                                           (from sq in GlobalQuery.ViewStockLite(db, gdg, item)
                                            group sq by new { sq.c_gdg, sq.c_iteno, sq.c_batch } into g
                                            select new
                                            {
                                                g.Key.c_gdg,
                                                g.Key.c_iteno,
                                                g.Key.c_batch,
                                                N_GSISA = g.Sum(x => x.n_gsisa),
                                                N_BSISA = g.Sum(x => x.n_bsisa)
                                            })
                                       join q1 in db.LG_MsBatches on new { qVWS.c_iteno, qVWS.c_batch } equals new { q1.c_iteno, q1.c_batch }
                                       where (qVWS.N_GSISA != 0 || qVWS.N_BSISA != 0) && (qVWS.c_iteno == item)
                                       select new
                                       {
                                           q1.c_batch,
                                           q1.d_expired,
                                           qVWS.N_GSISA,
                                           qVWS.N_BSISA
                                       });

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_BATCHDETAIL_BAD

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_BATCHDETAIL_BAD:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string item = (parameters.ContainsKey("iteno") ? (string)((Functionals.ParameterParser)parameters["iteno"]).Value : string.Empty);

                            var qry = (from qVWS in
                                           (from sq in GlobalQuery.ViewStockLite(db, gdg, item)
                                            group sq by new { sq.c_gdg, sq.c_iteno, sq.c_batch } into g
                                            select new
                                            {
                                                g.Key.c_gdg,
                                                g.Key.c_iteno,
                                                g.Key.c_batch,
                                                N_BSISA = g.Sum(x => x.n_bsisa)
                                            })
                                       join q1 in db.LG_MsBatches on new { qVWS.c_iteno, qVWS.c_batch } equals new { q1.c_iteno, q1.c_batch }
                                       where qVWS.N_BSISA != 0 && (qVWS.c_iteno == item)
                                       select new
                                       {
                                           q1.c_batch,
                                           q1.d_expired,
                                           qVWS.N_BSISA
                                       });

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_SJ_REPACK

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_SJ_REPACK:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');

                            var qry = (from qVWS in
                                           (from sq in GlobalQuery.ViewStockLite(db, gdg)
                                            group sq by new { sq.c_gdg, sq.c_iteno } into g
                                            select new
                                            {
                                                g.Key.c_gdg,
                                                g.Key.c_iteno,
                                                N_GSISA = g.Sum(x => x.n_gsisa),
                                                N_BSISA = g.Sum(x => x.n_bsisa)
                                            })
                                       join q2 in db.FA_MasItms on new { qVWS.c_iteno } equals new { q2.c_iteno }
                                       where (qVWS.N_GSISA != 0 || qVWS.N_BSISA != 0)
                                       select new
                                       {
                                           qVWS.c_iteno,
                                           qVWS.c_gdg,
                                           q2.c_nosup,
                                           q2.v_itnam,
                                           q2.l_aktif,
                                           q2.l_hide,
                                       });

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_TF_PEMUSNAHAN

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_TF_PEMUSNAHAN:
                        {


                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string memo = (parameters.ContainsKey("memo") ? (string)((Functionals.ParameterParser)parameters["memo"]).Value : string.Empty);
                            string type = (parameters.ContainsKey("type") ? (string)((Functionals.ParameterParser)parameters["type"]).Value : string.Empty);

                            if (type == "regular")
                            {
                                var qry = (from qVWS in
                                               (from sq in GlobalQuery.ViewStockLite(db, gdg)
                                                group sq by new { sq.c_gdg, sq.c_iteno } into g
                                                select new
                                                {
                                                    g.Key.c_gdg,
                                                    g.Key.c_iteno,
                                                    N_GSISA = g.Sum(x => x.n_gsisa),
                                                    N_BSISA = g.Sum(x => x.n_bsisa)
                                                })
                                           join q2 in db.FA_MasItms on new { qVWS.c_iteno } equals new { q2.c_iteno }
                                           where (qVWS.N_BSISA != 0)
                                           select new
                                           {
                                               qVWS.c_iteno,
                                               qVWS.c_gdg,
                                               q2.c_nosup,
                                               q2.v_itnam,
                                               q2.l_aktif,
                                               q2.l_hide,
                                               n_sisa = qVWS.N_BSISA
                                           });

                                if ((parameters != null) && (parameters.Count > 0))
                                {
                                    foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                    {
                                        if (kvp.Value.IsCondition)
                                        {
                                            if (kvp.Value.IsLike)
                                            {
                                                paternLike = kvp.Value.Value.ToString();
                                                qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                            }
                                            else if (kvp.Value.IsBetween)
                                            {

                                            }
                                            else
                                            {
                                                qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                            }
                                        }
                                    }
                                }

                                Logger.WriteLine(qry.Provider.ToString());

                                nCount = qry.Count();

                                if (nCount > 0)
                                {
                                    if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                    {
                                        qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                    }

                                    if ((limit == -1) || allQuery)
                                    {
                                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                    }
                                    else
                                    {
                                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                    }
                                }

                                dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                            }
                            else if (type == "memo")
                            {
                                var qry = (from itm in db.FA_MasItms
                                           join mpd in db.MK_MPDs on itm.c_iteno equals mpd.c_iteno
                                           where (mpd.c_mpno == memo) && (mpd.n_sisa > 0)
                                            && (mpd.c_gdg == gdg)
                                           select new
                                           {
                                               itm.v_itnam,
                                               itm.c_iteno,
                                               mpd.n_sisa,
                                               itm.c_nosup,
                                               itm.l_aktif,
                                               itm.l_hide
                                           }).Distinct().AsQueryable();

                                if ((parameters != null) && (parameters.Count > 0))
                                {
                                    foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                    {
                                        if (kvp.Value.IsCondition)
                                        {
                                            if (kvp.Value.IsLike)
                                            {
                                                paternLike = kvp.Value.Value.ToString();
                                                qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                            }
                                            else if (kvp.Value.IsBetween)
                                            {

                                            }
                                            else
                                            {
                                                qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                            }
                                        }
                                    }
                                }

                                Logger.WriteLine(qry.Provider.ToString());

                                nCount = qry.Count();

                                if (nCount > 0)
                                {
                                    if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                    {
                                        qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                    }

                                    if ((limit == -1) || allQuery)
                                    {
                                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                    }
                                    else
                                    {
                                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                    }
                                }

                                dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                            }
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_SUPPLIER_SJ

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_SUPPLIER_SJ:
                        {

                            var qry = (from q in db.LG_DatSups
                                       join q1 in db.LG_SPGHs on q.c_nosup equals q1.c_nosup
                                       join q2 in db.LG_SPGD1s on new { c_gdg = q1.c_gdg1, q1.c_spgno } equals new { q2.c_gdg, q2.c_spgno }
                                       where q2.n_sisa > 0
                                       && SqlMethods.DateDiffMonth(q1.d_spgdate, DateTime.Now) < 2
                                       && q1.l_status == true
                                       select new
                                       {
                                           q.c_nosup,
                                           q.v_nama,
                                           q.l_aktif,
                                           q.l_hide
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_SURAT_JALAN_AUTO

                    case Constant.MODEL_COMMON_QUERY_SURAT_JALAN_AUTO:
                        {
                            string rnno = (parameters.ContainsKey("c_rnno") ? (string)((Functionals.ParameterParser)parameters["c_rnno"]).Value : string.Empty);
                            char gdg = (parameters.ContainsKey("c_gdg") ? (char)((Functionals.ParameterParser)parameters["c_gdg"]).Value : '?');
                            //string supplier = (parameters.ContainsKey("supplier") ? (string)((Functionals.ParameterParser)parameters["supplier"]).Value : string.Empty);

                            List<SJAuto> lstSjAuto = new List<SJAuto>();
                            List<SJDetailAutoGenerator> lstSjAutoF = new List<SJDetailAutoGenerator>();
                            List<SGList> lstSG = new List<SGList>();
                            SJDetailAutoGenerator sjSingleF = new SJDetailAutoGenerator();
                            SGList sgSingle = new SGList();
                            SJAuto iSjAuto = new SJAuto();

                            var qsd = (from q in db.LG_RNHs
                                       join q1 in db.LG_RND2s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                       join q2 in db.LG_RND1s on new { q1.c_gdg, q1.c_rnno, q1.c_iteno, q1.c_batch }
                                       equals new { q2.c_gdg, q2.c_rnno, q2.c_iteno, q2.c_batch }
                                       join q3 in db.FA_MasItms on q2.c_iteno equals q3.c_iteno
                                       where q.c_rnno == rnno && q.c_gdg == gdg //&& q.c_from == supplier
                                       group new { q1, q2 } by new { q1.c_gdg, q1.c_no, q1.c_iteno, q1.c_batch, q3.v_itnam } into gSum
                                       select new
                                       {
                                           c_gdg = gSum.Key.c_gdg,
                                           c_iteno = gSum.Key.c_iteno,
                                           c_no = gSum.Key.c_no,
                                           c_batch = gSum.Key.c_batch,
                                           nQty = gSum.Sum(x => x.q2.n_gqty),
                                           nSisa = gSum.Sum(x => x.q2.n_gsisa),
                                           v_itnam = gSum.Key.v_itnam,
                                       }).AsQueryable();

                            var sf = qsd.ToList();

                            lstSjAuto = (from q in qsd
                                         join q1 in db.LG_POD2s on new { q.c_gdg, c_pono = q.c_no } equals new { q1.c_gdg, q1.c_pono }
                                         join q2 in db.LG_ORD2s on new { q1.c_gdg, q1.c_orno, q.c_iteno } equals new { q2.c_gdg, q2.c_orno, q2.c_iteno }
                                         where q2.c_spno.Contains("SG")
                                         select new SJAuto
                                         {
                                             c_iteno = q.c_iteno,
                                             c_no = q.c_no,
                                             n_gqty = q.nQty.HasValue ? q.nQty.Value : 0,
                                             c_orno = q1.c_orno,
                                             c_spno = q2.c_spno,
                                             c_batch = q.c_batch,
                                             n_gsisa = q.nSisa.HasValue ? q.nSisa.Value : 0,
                                             v_itnam = q.v_itnam,
                                         }).Distinct().ToList();

                            var fg = (from q in qsd
                                      join q1 in db.LG_POD2s on new { q.c_gdg, c_pono = q.c_no } equals new { q1.c_gdg, q1.c_pono }
                                      join q2 in db.LG_ORD2s on new { q1.c_gdg, q1.c_orno, q.c_iteno } equals new { q2.c_gdg, q2.c_orno, q2.c_iteno }
                                      where q2.c_spno.Contains("SG")
                                      select new SJAuto
                                      {
                                          c_iteno = q.c_iteno,
                                          c_no = q.c_no,
                                          n_gqty = q.nQty.HasValue ? q.nQty.Value : 0,
                                          c_orno = q1.c_orno,
                                          c_spno = q2.c_spno,
                                          c_batch = q.c_batch,
                                          n_gsisa = q.nSisa.HasValue ? q.nSisa.Value : 0,
                                      }).ToList();

                            lstSG = (from q in qsd
                                     join q1 in db.LG_POD2s on new { q.c_gdg, c_pono = q.c_no } equals new { q1.c_gdg, q1.c_pono }
                                     join q2 in db.LG_ORD2s on new { q.c_gdg, q1.c_orno, q.c_iteno } equals new { q2.c_gdg, q2.c_orno, q2.c_iteno }
                                     where q2.c_spno.Contains("SG")
                                     group new { q, q2 } by new { q.c_iteno, q2.c_spno } into gSum
                                     select new SGList
                                     {
                                         c_iteno = gSum.Key.c_iteno,
                                         c_spgno = gSum.Key.c_spno,
                                         nQty = gSum.Sum(x => x.q2.n_sisa.HasValue ? x.q2.n_sisa.Value : 0),

                                     }).ToList();

                            int i = 0;

                            decimal nQtyBatch = 0,
                              nQtySG = 0, nAllocated = 0;

                            string sItemSJ = null, sItemSG = null,
                              sBatchEnding = null, sSGEnding = null;

                            List<string> lstItem = new List<string>();

                            if (lstSjAuto.Sum(x => x.n_gqty) == lstSjAuto.Sum(x => x.n_gsisa))
                            {
                                for (i = 0; i < lstSjAuto.Count; i++)
                                {
                                    iSjAuto = lstSjAuto[i];

                                    sItemSJ = iSjAuto.c_iteno;


                                    if (lstItem.Count > 0)
                                    {
                                        sgSingle = (from q in lstSG
                                                    where q.c_iteno == sItemSJ && q.c_spgno == iSjAuto.c_spno
                                                    select q).Take(1).SingleOrDefault();

                                        if (sItemSG == sItemSJ)
                                        {
                                            nQtySG = sgSingle.nQty;
                                        }
                                        else
                                        {
                                            nQtySG = sgSingle.nQty;
                                        }
                                    }

                                    var sSisaSG = (from q in db.LG_SPGD1s
                                                   where q.c_spgno == iSjAuto.c_spno && q.c_iteno == sItemSJ
                                                   select q.n_sisa).Take(1).SingleOrDefault();

                                    if (sSisaSG <= 0)
                                        continue;

                                    //if (sItemSG == null)
                                    //    sItemSG = "";

                                    //if (sItemSJ.ToString() == sItemSG.ToString())
                                    //{
                                    //    if (iSjAuto.n_gqty > sSisaSG)
                                    //        continue;
                                    //}

                                    if (lstItem == null || lstItem.Count == 0)
                                    {
                                        sItemSG = sItemSJ;
                                        lstItem.Add(sItemSG);
                                        nQtyBatch = iSjAuto.n_gqty;
                                        sgSingle = (from q in lstSG
                                                    where q.c_iteno == sItemSJ && q.c_spgno == iSjAuto.c_spno
                                                    select q).Take(1).SingleOrDefault();

                                        nQtySG = sgSingle.nQty;

                                    }
                                    else if ((sItemSG != sItemSJ) || sBatchEnding != iSjAuto.c_batch)
                                    {
                                        if (nQtyBatch > 0 && iSjAuto.n_gqty != nQtyBatch)
                                        {
                                            sjSingleF = lstSjAutoF.Find(delegate(SJDetailAutoGenerator sjd)
                                            {
                                                return sItemSG.Trim().Equals((string.IsNullOrEmpty(sjd.c_iteno.Trim()) ? string.Empty : sjd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                  sSGEnding.Trim().Equals((string.IsNullOrEmpty(sjd.c_spgno.Trim()) ? string.Empty : sjd.c_spgno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                    sBatchEnding.Trim().Equals((string.IsNullOrEmpty(sjd.c_batch.Trim()) ? string.Empty : sjd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                            });

                                            sjSingleF.n_adjust += Math.Abs(nQtyBatch);
                                        }

                                        sgSingle = (from q in lstSG
                                                    where q.c_iteno == sItemSJ && q.c_spgno == iSjAuto.c_spno
                                                    select q).Take(1).SingleOrDefault();

                                        nQtyBatch = iSjAuto.n_gqty;

                                        if (sgSingle != null)
                                        {
                                            nQtySG = sgSingle.nQty;
                                            lstItem.Add(sgSingle.c_iteno);
                                            sItemSG = sItemSJ;
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }

                                    nAllocated = nQtySG > nQtyBatch ? nQtyBatch : nQtySG;

                                    if (nAllocated > 0)
                                    {
                                        lstSjAutoF.Add(new SJDetailAutoGenerator()
                                        {
                                            c_iteno = iSjAuto.c_iteno,
                                            c_batch = iSjAuto.c_batch,
                                            n_gqty = nAllocated,
                                            v_itnam = iSjAuto.v_itnam,
                                            c_spgno = iSjAuto.c_spno,
                                            n_booked = nAllocated,
                                            l_new = true
                                        });
                                    }

                                    sBatchEnding = iSjAuto.c_batch;
                                    sSGEnding = iSjAuto.c_spno;

                                    nQtyBatch -= nAllocated;
                                }
                            }

                            if (nQtyBatch > 0 && iSjAuto.n_gqty != nQtyBatch)
                            {
                                sjSingleF = lstSjAutoF.Find(delegate(SJDetailAutoGenerator sjd)
                                {
                                    return sItemSG.Trim().Equals((string.IsNullOrEmpty(sjd.c_iteno.Trim()) ? string.Empty : sjd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                      sSGEnding.Trim().Equals((string.IsNullOrEmpty(sjd.c_spgno.Trim()) ? string.Empty : sjd.c_spgno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                        sBatchEnding.Trim().Equals((string.IsNullOrEmpty(sjd.c_batch.Trim()) ? string.Empty : sjd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                });

                                sjSingleF.n_adjust += Math.Abs(nQtyBatch);
                            }



                            nCount = lstSjAutoF.Count();

                            if ((limit == -1) || allQuery)
                            {
                                dic.Add(Constant.DEFAULT_NAMING_RECORDS, lstSjAutoF.ToList());
                            }
                            else
                            {
                                dic.Add(Constant.DEFAULT_NAMING_RECORDS, lstSjAutoF.Skip(start).Take(limit).ToList());
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #endregion

                    #region STT

                    #region MODEL_COMMON_QUERY_MULTIPLE_STT_MEMO

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_STT_MEMO:
                        {
                            var qry = (from q in db.MK_MTHs
                                       join q1 in db.MK_MTDs on q.c_mtno equals q1.c_mtno
                                       where (q1.n_sisa > 0)
                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       select new
                                       {
                                           q.c_mtno,
                                           q.d_mtdate,
                                           q.c_memo,
                                           q.c_type
                                       }).Distinct().AsQueryable();


                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_STT_ITEM

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_STT_ITEM:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string memo = (parameters.ContainsKey("memo") ? (string)((Functionals.ParameterParser)parameters["memo"]).Value : string.Empty);

                            var qry = (from itm in db.FA_MasItms
                                       join mkd in db.MK_MTDs on itm.c_iteno equals mkd.c_iteno
                                       //join sth in db.LG_STHs on mkd.c_mtno equals sth.c_mtno
                                       //join stock in GlobalQuery.ViewStockLite(db) on mkd.c_iteno equals stock.c_iteno
                                       //where stock.n_gsisa > 0 && sth.c_mtno == memo && mkd.n_sisa > 0
                                       where (mkd.c_mtno == memo) && (mkd.n_sisa > 0)
                                        && (mkd.c_gdg == gdg)
                                       select new
                                       {
                                           itm.v_itnam,
                                           itm.c_iteno,
                                           mkd.n_qty,
                                           mkd.n_sisa
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_STT_BATCH

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_STT_BATCH:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);

                            var qry = (from q in
                                           (from sq in GlobalQuery.ViewStockLite(db, gdg)
                                            where (sq.c_gdg == gdg) && (sq.c_iteno == item)
                                             && (sq.n_gsisa != 0)
                                            group sq by new { sq.c_gdg, sq.c_iteno, sq.c_batch } into g
                                            select new
                                            {
                                                g.Key.c_gdg,
                                                g.Key.c_iteno,
                                                g.Key.c_batch,
                                                n_gsisa = g.Sum(x => x.n_gsisa)
                                            })
                                       join q1 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q1.c_iteno, q1.c_batch } into q_1
                                       from qBat in q_1.DefaultIfEmpty()
                                       select new
                                       {
                                           c_gdg = gdg,
                                           q.c_iteno,
                                           q.c_batch,
                                           n_gsisa = q.n_gsisa,
                                           qBat.d_expired
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #endregion

                    #region Pemusnahan

                    #region MODEL_COMMON_QUERY_MULTIPLE_PM_ITEM

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_PM_ITEM:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');

                            var qry = (from q in db.FA_MasItms
                                       join q1 in
                                           ((from sq in GlobalQuery.ViewStockLite(db, gdg)
                                             group sq by sq.c_iteno into g
                                             where (g.Sum(x => x.n_bsisa) != 0)
                                             select new
                                             {
                                                 c_iteno = g.Key
                                             })) on q.c_iteno equals q1.c_iteno
                                       select new
                                       {
                                           q.v_itnam,
                                           q.c_iteno,
                                           q.v_undes,
                                       }).Distinct().AsQueryable();
                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }
                            Logger.WriteLine(qry.Provider.ToString());
                            nCount = qry.Count();
                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }
                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }
                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);
                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;
                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_PM_BATCH
                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_PM_BATCH:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);
                            var qry = (from q in
                                           (from sq in GlobalQuery.ViewStockLite(db, gdg)
                                            where (sq.c_gdg == gdg) && (sq.c_iteno == item)
                                             && (sq.n_bsisa != 0)
                                            group sq by new { sq.c_gdg, sq.c_iteno, sq.c_batch } into g
                                            select new
                                            {
                                                g.Key.c_gdg,
                                                g.Key.c_iteno,
                                                g.Key.c_batch,
                                                n_bsisa = g.Sum(x => x.n_bsisa)
                                            })
                                       join q1 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q1.c_iteno, q1.c_batch } into q_1
                                       from qBat in q_1.DefaultIfEmpty()
                                       select new
                                       {
                                           c_gdg = gdg,
                                           q.c_iteno,
                                           q.c_batch,
                                           n_bsisa = q.n_bsisa,
                                           qBat.d_expired
                                       }).Distinct().AsQueryable();
                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }
                            Logger.WriteLine(qry.Provider.ToString());
                            nCount = qry.Count();
                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }
                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }
                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);
                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;
                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_MEMO_PEMUSNAHAN

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_MEMO_PEMUSNAHAN:
                        {
                            var qry = (from q in db.MK_MPHs
                                       join q1 in db.MK_MPDs on q.c_mpno equals q1.c_mpno
                                       where (q1.n_sisa > 0)
                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       select new
                                       {
                                           q.c_mpno,
                                           q.d_mpdate,
                                           q.c_memo
                                       }).Distinct().AsQueryable();


                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;
                    #endregion

                    #endregion

                    #region Ekspedisi

                    #region MODEL_COMMON_QUERY_MULTIPLE_CUSTOMER_EKSPEDISI

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_CUSTOMER_EKSPEDISI:
                        {
                            string via = (parameters.ContainsKey("c_via") ? (string)((Functionals.ParameterParser)parameters["c_via"]).Value : string.Empty);

                            if (via != "01" && via != "03")
                            {
                                via = "02";
                            }

                            var qry1 = (from cab in db.LG_Cusmas
                                       join doh in db.LG_DOHs on cab.c_cusno equals doh.c_cusno
                                       join dod1 in db.LG_DOD1s on doh.c_dono equals dod1.c_dono
                                       where cab.l_stscus == true && dod1.n_qty >= 0 && doh.c_via == via
                                       select new
                                       {
                                           cab.c_cusno,
                                           cab.v_cunam,
                                           cab.c_cab,
                                           doh.c_via
                                       }).Distinct().AsQueryable();

                            var qry2 = (from q in db.LG_MsGudangs
                                        where q.l_aktif == true
                                        select new
                                        {
                                            c_cusno = q.c_gdg.ToString(),
                                            v_cunam = q.v_gdgdesc,
                                            c_cab = "HO" + q.c_gdg.ToString(),
                                            c_via = "01"
                                        }).AsQueryable();

                            var qry = qry1.Union(qry2).AsQueryable(); //penambahan by suwandi 08 oktober 2018 untuk penggabungan pengiriman DC dan cabang

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_MASTER_EKSPEDISI

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_MASTER_EKSPEDISI:
                        {
                            var qry = (from mseks in db.LG_MsExps
                                       where mseks.l_aktif == true
                                       select new
                                       {
                                           mseks.c_exp,
                                           mseks.v_ket
                                       }).AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_SUPPLIER_EKSPEDISI

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_SUPPLIER_EKSPEDISI:
                        {
                            var qry = (from supauto in db.LG_DatSupAutos
                                       join sup in db.LG_DatSups on supauto.c_nosup equals sup.c_nosup
                                       where supauto.l_exp == true
                                       select new
                                       {
                                           c_nosup = supauto.c_nosup,
                                           v_nama = sup.v_nama
                                       }).AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                        }
                        break;
                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_DO_EKSPEDISI

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_DO_EKSPEDISI:
                        {
                            string noSup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);
                            string cusno = (parameters.ContainsKey("cusno") ? (string)((Functionals.ParameterParser)parameters["cusno"]).Value : string.Empty);
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');

                            // Production
                            //var qry = (from q in db.LG_DOHs
                            //           join q1 in
                            //             (from sq in db.LG_DOD1s
                            //              join sq1 in db.FA_MasItms on sq.c_iteno equals sq1.c_iteno
                            //              select new
                            //              {
                            //                sq.c_dono,
                            //                sq1.c_nosup
                            //              }) on q.c_dono equals q1.c_dono
                            //           where (q.l_status == false)
                            //            && (q.c_cusno == cusno) && ((q.l_auto.HasValue ? q.l_auto.Value : false) == false)
                            //             //&& ((string.IsNullOrEmpty(noSup) ? q1.c_nosup : noSup) == q1.c_nosup) 
                            //            && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                            //           select new
                            //           {
                            //             q.c_dono,
                            //             q.d_dodate,
                            //             q1.c_nosup
                            //           }).Distinct().AsQueryable();

                            var listType = new string[] { "02", "06" };

                            var qry = (from q in db.LG_DOHs
                                       join q1 in
                                           (from sq in db.LG_DOD1s
                                            join sq1 in db.FA_MasItms on sq.c_iteno equals sq1.c_iteno
                                            where (string.IsNullOrEmpty(noSup) ? true : (sq1.c_nosup == noSup))
                                            select new
                                            {
                                                sq.c_dono,
                                                sq1.c_nosup
                                            }) on q.c_dono equals q1.c_dono
                                       join q2 in db.LG_PLHs on q.c_plno equals q2.c_plno
                                       where (q.l_status == false)
                                        && (q.c_cusno == cusno)
                                        && (q.c_gdg == gdg)
                                        && listType.Contains(q2.c_type)

                                       //&& ((string.IsNullOrEmpty(noSup) ? q1.c_nosup : noSup) == q1.c_nosup) 
                                       //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       select new
                                       {
                                           q.c_dono,
                                           q.c_type,
                                           q.d_dodate,
                                           q1.c_nosup,
                                           q.n_karton,
                                           q.n_receh
                                       }).Distinct().AsQueryable();
                            //(A)

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsIn)
                                        {
                                            #region In Clause

                                            qry = qry.In(kvp.Key, (object[])kvp.Value.Value).AsQueryable();

                                            #endregion
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                            #region Old Coded

                            //switch (nosup.Length > 0)
                            //{
                            //  case true:
                            //    {
                            //      var qry = (from doh in db.LG_DOHs
                            //                 join plh in db.LG_PLHs on doh.c_plno equals plh.c_plno
                            //                 join auto in db.LG_DatSupAutos on plh.c_nosup equals auto.c_nosup
                            //                 where doh.l_status == false && auto.l_exp == true
                            //                 && doh.c_cusno == cusno
                            //                 && plh.c_type != "03"
                            //                 select new
                            //                 {
                            //                   doh.c_dono
                            //                 }).AsQueryable();

                            //      if ((parameters != null) && (parameters.Count > 0))
                            //      {
                            //        

                            //        foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                            //        {
                            //          if (kvp.Value.IsCondition)
                            //          {
                            //            if (kvp.Value.IsLike)
                            //            {
                            //              paternLike = kvp.Value.Value.ToString();
                            //              qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                            //            }
                            //            else if (kvp.Value.IsBetween)
                            //            {

                            //            }
                            //            else
                            //            {
                            //              qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                            //            }
                            //          }
                            //        }
                            //      }

                            //      Logger.WriteLine(qry.Provider.ToString());

                            //      nCount = qry.Count();

                            //      if (nCount > 0)
                            //      {
                            //        if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                            //        {
                            //          qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                            //        }

                            //        if ((limit == -1) || allQuery)
                            //        {
                            //          dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                            //        }
                            //        else
                            //        {
                            //          dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                            //        }
                            //      }

                            //      dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            //      dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                            //    }
                            //    break;

                            //  case false:
                            //    {


                            //      var qry = (from doh in db.LG_DOHs
                            //                 join plh in db.LG_PLHs on doh.c_plno equals plh.c_plno
                            //                 join sup in db.LG_DatSups on plh.c_nosup equals sup.c_nosup
                            //                 where doh.l_status == false
                            //                 && doh.c_cusno == cusno && (sup.c_nosup != "00001" && sup.c_nosup != "00112" && sup.c_nosup != "00113" && sup.c_nosup != "00117" && sup.c_nosup != "00120")
                            //                 && plh.c_type != "03"
                            //                 select new
                            //                 {
                            //                   doh.c_dono,
                            //                   sup.v_nama
                            //                 }).AsQueryable();

                            //      if ((parameters != null) && (parameters.Count > 0))
                            //      {
                            //        

                            //        foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                            //        {
                            //          if (kvp.Value.IsCondition)
                            //          {
                            //            if (kvp.Value.IsLike)
                            //            {
                            //              paternLike = kvp.Value.Value.ToString();
                            //              qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                            //            }
                            //            else if (kvp.Value.IsBetween)
                            //            {

                            //            }
                            //            else
                            //            {
                            //              qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                            //            }
                            //          }
                            //        }
                            //      }

                            //      Logger.WriteLine(qry.Provider.ToString());

                            //      nCount = qry.Count();

                            //      if (nCount > 0)
                            //      {
                            //        if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                            //        {
                            //          qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                            //        }

                            //        if ((limit == -1) || allQuery)
                            //        {
                            //          dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                            //        }
                            //        else
                            //        {
                            //          dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                            //        }
                            //      }

                            //      dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            //      dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                            //    }

                            //    break;
                            //}

                            #endregion
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RS_EKSPEDISI
                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RS_EKSPEDISI:
                        {
                            string noSup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            var qry = (from q in db.LG_RSHes
                                       where q.c_nosup == noSup
                                       && q.c_gdg == gdg
                                       && (q.l_delete == false || q.l_delete == null)
                                       && (q.l_print.HasValue ? q.l_print.Value : false)
                                       && (q.l_exp == false || q.l_exp == null)
                                       select q).AsQueryable();
                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsIn)
                                        {
                                            #region In Clause
                                            qry = qry.In(kvp.Key, (object[])kvp.Value.Value).AsQueryable();
                                            #endregion
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }
                            Logger.WriteLine(qry.Provider.ToString());
                            nCount = qry.Count();
                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }
                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }
                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);
                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;
                    #endregion
                    #region MODEL_COMMON_QUERY_MULTIPLE_DO_EKSPEDISI_SJ

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_DO_EKSPEDISI_SJ:
                        {
                            //string noSup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);
                            char gdgTo = (parameters.ContainsKey("gdgTo") ? (char)((Functionals.ParameterParser)parameters["gdgTo"]).Value : '?');
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');

                            // Prodoction
                            //var qry = (from q in db.LG_SJHs
                            //           where (q.l_status == false)
                            //            && (q.c_gdg == gdg) && (q.l_confirm == true) && (q.l_print == true)
                            //             && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                            //            && (q.c_gdg2 == gdgTo)
                            //           select new
                            //           {
                            //             q.c_sjno,
                            //             q.d_sjdate,
                            //             q.c_nosup
                            //           }).Distinct().AsQueryable();


                            var qry = (from q in db.LG_SJHs
                                       where (q.l_exp == false)
                                        && (q.c_gdg == gdg) && (q.l_confirm == true) && (q.l_print == true)
                                           //&& (string.IsNullOrEmpty(noSup) ? false : (q.c_nosup == noSup))
                                        && (q.c_gdg2 == gdgTo)
                                       //&& q.l_auto == true
                                       select new
                                       {
                                           q.c_sjno,
                                           q.d_sjdate,
                                           q.n_karton,
                                           q.n_receh
                                           //q.c_nosup
                                       }).Distinct().AsQueryable();
                            //(A)

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsIn)
                                        {
                                            #region In Clause

                                            qry = qry.In(kvp.Key, (object[])kvp.Value.Value).AsQueryable();

                                            #endregion
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_DRIVER_EKSPEDISI

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_DRIVER_EKSPEDISI:
                        {
                            string c_type = (parameters.ContainsKey("c_type") ? (string)((Functionals.ParameterParser)parameters["c_type"]).Value : string.Empty);

                            var qry = (from q in db.SCMS_DRIVERs
                                       where q.l_aktif == true && q.c_type == c_type
                                       select new
                                       {
                                           q.c_nip,
                                           q.v_nama,
                                           q.c_nopol
                                       }).Distinct().AsQueryable();

                            var s = qry.ToList();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_WP_EKSPEDISI

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_WP_EKSPEDISI:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string cusno = (parameters.ContainsKey("cusno") ? (string)((Functionals.ParameterParser)parameters["cusno"]).Value : string.Empty);


                            var qry = (from q in db.SCMS_STHs
                                       join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                       where q.c_type == "04"
                                        && ((q1.l_exp.HasValue ? q1.l_exp.Value : false) == false)
                                        && (string.IsNullOrEmpty(cusno) ? true : (q1.c_cusno == cusno))
                                       select new
                                       {
                                           q.c_nodoc,
                                           q.d_date
                                       }).Distinct().AsQueryable();

                            var qerw = qry.ToList();
                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsIn)
                                        {
                                            #region In Clause

                                            qry = qry.In(kvp.Key, (object[])kvp.Value.Value).AsQueryable();

                                            #endregion
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RESI_EKSPEDISI

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RESI_EKSPEDISI:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string exp = (parameters.ContainsKey("exp") ? (string)((Functionals.ParameterParser)parameters["exp"]).Value : string.Empty);

                            var qry = (from q in db.LG_ExpHs
                                       where q.c_exp == exp
                                       && q.c_gdg == gdg
                                       && (q.l_ie == false || q.l_ie == null)
                                       && (q.l_delete == false || q.l_delete == null)
                                       && SqlMethods.DateDiffMonth(q.d_expdate, DateTime.Now.Date) < 12
                                       select new
                                       {
                                           q.c_exp,
                                           q.c_expno,
                                           q.c_resi
                                       }).Distinct().AsQueryable();


                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }

                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_NO_EKSPEDISI

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_NO_EKSPEDISI:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string except = (parameters.ContainsKey("EPexcept") ? (string)((Functionals.ParameterParser)parameters["EPexcept"]).Value : "?");


                            //var qryexcept = (from q in db.LG_ExpHs
                            //                 where (q.c_ref != null || q.c_ref != "")
                            //                 && (q.l_delete == false || q.l_delete == null)
                            //                 && q.c_gdg == gdg
                            //                 && (q.l_ie == false || q.l_ie == null)
                            //                 && SqlMethods.DateDiffMonth(q.d_expdate, DateTime.Now.Date) < 7
                            //                 select new
                            //                 {
                            //                     c_expno = q.c_ref
                            //                 }).Distinct().AsQueryable();

                            var qry = (from q in db.LG_ExpHs
                                       where (q.l_delete == false || q.l_delete == null)
                                       && q.c_gdg == gdg
                                       && (q.l_ie == false || q.l_ie == null)
                                       && (q.c_ref == null || q.c_ref == "")
                                       && q.c_expno != except
                                       && SqlMethods.DateDiffMonth(q.d_expdate, DateTime.Now.Date) < 12
                                       select new
                                       {
                                           q.c_expno
                                       }).Distinct().AsQueryable();

                            //var qry = qryep.Except(qryexcept).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }

                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_LIST_DO_RETURN

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_LIST_DO_RETURN:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');

                            DateTime fromDate = Convert.ToDateTime("2014-11-01");

                            var qryexcept = (from q in db.LG_ReturnDOs
                                             where q.c_gdg == gdg
                                             select new
                                             {
                                                 q.c_dono
                                             }).Distinct().AsQueryable();

                            var qrydo = (from q in db.LG_ExpDs
                                         join q1 in db.LG_ExpHs on q.c_expno equals q1.c_expno
                                         where (q1.l_delete == false || q1.l_delete == null)
                                         && q1.c_gdg == gdg
                                         && q1.d_expdate > fromDate
                                         && q.c_dono.StartsWith("DO")
                                         select new
                                         {
                                             q.c_dono
                                         }).Distinct().AsQueryable();

                            var qry = qrydo.Except(qryexcept).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }

                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_LIST_EP_EKSTERNAL

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_LIST_EP_EKSTERNAL:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string exp = (parameters.ContainsKey("exp") ? (string)((Functionals.ParameterParser)parameters["exp"]).Value : "?");

                            var qry = (from q in db.LG_ExpHs
                                       where q.c_exp == exp
                                       && q.c_gdg == gdg
                                       && (q.l_ie == false || q.l_ie == null)
                                       && (q.l_delete == false || q.l_delete == null)
                                       && SqlMethods.DateDiffMonth(q.d_expdate, DateTime.Now.Date) < 12
                                       select new
                                       {
                                           q.c_exp,
                                           q.c_expno
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }

                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_LIST_EP_INTERNAL

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_LIST_EP_INTERNAL:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');

                            var qry = (from q in db.LG_ExpHs
                                       where q.c_gdg == gdg
                                       && q.c_exp == "00"
                                       && (q.l_ie == false || q.l_ie == null)
                                       && (q.l_delete == false || q.l_delete == null)
                                       && SqlMethods.DateDiffMonth(q.d_expdate, DateTime.Now.Date) < 12
                                       select new
                                       {
                                           q.c_exp,
                                           q.c_expno
                                       }).Distinct().AsQueryable();


                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }

                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_FAKTUR_EKSPEDISI

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_FAKTUR_EKSPEDISI:
                        {
                            string exp = (parameters.ContainsKey("exp") ? (string)((Functionals.ParameterParser)parameters["exp"]).Value : string.Empty);

                            var qry = (from q in db.LG_IEHs
                                       join q1 in
                                           (from sq in db.LG_ExpHs
                                            join sq1 in db.LG_IED1s on sq.c_expno equals sq1.c_expno
                                            where sq.c_exp == exp
                                            group sq by sq1.c_ieno into g
                                            select new
                                            {
                                                c_ieno = g.Key,
                                                n_totalbiaya = g.Sum(x => x.n_totalbiaya),
                                            }) on q.c_ieno equals q1.c_ieno
                                       where q.c_exp == exp && q.n_netsisa > 0
                                       select new
                                       {
                                           q.c_exp,
                                           q1.c_ieno,
                                           q.n_netsisa,
                                           q.n_bilva_faktur,
                                           q.c_ie,
                                           q.n_disc,
                                           q1.n_totalbiaya
                                       }).Distinct().AsQueryable();

                            //var qry = (from q in db.LG_IEHs
                            //                 join q1 in db.LG_IED1s on q.c_ieno equals q1.c_ieno
                            //                 join q2 in 
                            //                    (from sq in db.LG_ExpHs
                            //                     where sq.c_exp == exp
                            //                     group sq by sq.c_expno into g
                            //                     select new 
                            //                     {
                            //                         c_expno = g.Key,
                            //                         n_totalbiaya = g.Sum(x => x.n_totalbiaya),
                            //                     }) on q1.c_expno equals q2.c_expno
                            //                 where q.c_exp == exp && q.n_netsisa > 0
                            //                 select new
                            //                 {
                            //                     q.c_exp,
                            //                     q1.c_expno,
                            //                     q1.c_ieno,
                            //                     q.n_netsisa,
                            //                     q.n_bilva_faktur,
                            //                     q.c_ie,
                            //                     q.n_disc,
                            //                     q2.n_totalbiaya
                            //                 }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }

                        break;

                    #endregion


                    #endregion

                    #region DO

                    #region MODEL_COMMON_QUERY_MULTIPLE_DOPL_NOPL

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_DOPL_NOPL:
                        {
                            string cusno = (parameters.ContainsKey("cusno") ? (string)((Functionals.ParameterParser)parameters["cusno"]).Value : string.Empty);

                            //string[] NoSup = {"00001","00112","00113","00117","00120"};

                            //var Stat = (from itm in db.FA_MasItms
                            //           where NoSup.Contains(itm.c_nosup) && itm.c_type == "02"
                            //           select new
                            //           {
                            //             itm.c_nosup,
                            //             itm.c_iteno
                            //           }).Distinct().AsQueryable();

                            var qty = (from Tot in db.LG_PLD1s
                                       group Tot by new { Tot.c_plno } into pl1
                                       select new
                                       {
                                           c_plno = pl1.Key.c_plno,
                                           n_sisa = pl1.Sum(x => x.n_sisa)
                                       }).Distinct().AsQueryable();

                            var Total = qty.Sum(x => x.n_sisa).Value > 0;

                            var qry = (from plh in db.LG_PLHs
                                       join pld1 in db.LG_PLD1s on plh.c_plno equals pld1.c_plno
                                       join sisa in qty on pld1.c_plno equals sisa.c_plno
                                       join ket in
                                           (from q in db.LG_DatSupAutos
                                            where q.l_exp == true
                                            select new { q.c_nosup }) on plh.c_nosup equals
                                                            ket.c_nosup into ketStr
                                       from ket1 in ketStr.DefaultIfEmpty()
                                       join mstran in db.MsTransDs on plh.c_type equals mstran.c_type
                                       where pld1.n_sisa > 0 && plh.l_confirm == true &&
                                             plh.l_print == true && sisa.n_sisa > 0 && plh.c_cusno == cusno
                                             && (mstran.c_notrans == "15") && (mstran.c_portal == '3')
                                       select new
                                       {

                                           mstran.v_ket,
                                           plh.c_plno,
                                           ket = ket1.c_nosup == null ? 0 : 1,
                                           plh.c_nosup,
                                           plh.c_baspbno
                                       }).Distinct().AsQueryable();




                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_DOSTT_NOSTT_SAMPLE

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_DOSTT_NOSTT_SAMPLE:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');

                            var qry = (from q in db.LG_STHs
                                       join q1 in db.LG_STD1s on new { q.c_gdg, q.c_stno } equals new { q1.c_gdg, q1.c_stno }
                                       where q.c_type == "02" && q1.n_sisa > 0 && q1.c_gdg == gdg
                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       select new
                                       {
                                           q.c_gdg,
                                           q.c_stno,
                                           q.c_mtno,
                                           q.d_stdate
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;
                    #endregion

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_GUDANGCABANG

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_GUDANGCABANG:
                        {
                            var qry = (from q in db.LG_MsGudangs
                                       select new
                                       {
                                           v_kode = q.c_gdg.ToString(),
                                           v_desc = q.v_gdgdesc,
                                           l_aktif = true
                                       })
                                       .Union(from q in db.LG_Cusmas
                                              select new
                                              {
                                                  v_kode = q.c_cusno,
                                                  v_desc = q.v_cunam,
                                                  l_aktif = (q.l_stscus.HasValue ? q.l_stscus.Value : false)
                                              }).AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region SP

                    #region MODEL_COMMON_QUERY_MULTIPLE_ITEM_SP

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_SP:
                        {
                            bool activateSuplier = (parameters.ContainsKey("activateSuplier") ? (bool)((Functionals.ParameterParser)parameters["activateSuplier"]).Value : false);
                            string spNo = (parameters.ContainsKey("spno") ? (string)((Functionals.ParameterParser)parameters["spno"]).Value : string.Empty);
                            string custNo = (parameters.ContainsKey("custno") ? (string)((Functionals.ParameterParser)parameters["custno"]).Value : string.Empty);

                            DateTime datePrev = DateTime.Now.AddMonths(-1);

                            var qry1 = (from q in db.LG_AvgSales
                                        where (q.c_cusno == custNo) && (q.s_tahun == datePrev.Year)
                                        select new
                                        {
                                            q.c_iteno,
                                            n_avgsales = (
                                              (datePrev.Month == 1 ? ((q.n_sls01.HasValue ? q.n_sls01.Value : 0) - (q.n_rtr01.HasValue ? q.n_rtr01.Value : 0)) :
                                              (datePrev.Month == 2 ? ((q.n_sls02.HasValue ? q.n_sls02.Value : 0) - (q.n_rtr02.HasValue ? q.n_rtr02.Value : 0)) :
                                              (datePrev.Month == 3 ? ((q.n_sls03.HasValue ? q.n_sls03.Value : 0) - (q.n_rtr03.HasValue ? q.n_rtr03.Value : 0)) :
                                              (datePrev.Month == 4 ? ((q.n_sls04.HasValue ? q.n_sls04.Value : 0) - (q.n_rtr04.HasValue ? q.n_rtr04.Value : 0)) :
                                              (datePrev.Month == 5 ? ((q.n_sls05.HasValue ? q.n_sls05.Value : 0) - (q.n_rtr05.HasValue ? q.n_rtr05.Value : 0)) :
                                              (datePrev.Month == 6 ? ((q.n_sls06.HasValue ? q.n_sls06.Value : 0) - (q.n_rtr06.HasValue ? q.n_rtr06.Value : 0)) :
                                              (datePrev.Month == 7 ? ((q.n_sls07.HasValue ? q.n_sls07.Value : 0) - (q.n_rtr07.HasValue ? q.n_rtr07.Value : 0)) :
                                              (datePrev.Month == 8 ? ((q.n_sls08.HasValue ? q.n_sls08.Value : 0) - (q.n_rtr08.HasValue ? q.n_rtr08.Value : 0)) :
                                              (datePrev.Month == 9 ? ((q.n_sls09.HasValue ? q.n_sls09.Value : 0) - (q.n_rtr09.HasValue ? q.n_rtr09.Value : 0)) :
                                              (datePrev.Month == 10 ? ((q.n_sls10.HasValue ? q.n_sls10.Value : 0) - (q.n_rtr10.HasValue ? q.n_rtr10.Value : 0)) :
                                              (datePrev.Month == 11 ? ((q.n_sls11.HasValue ? q.n_sls11.Value : 0) - (q.n_rtr11.HasValue ? q.n_rtr11.Value : 0)) :
                                              (datePrev.Month == 12 ? ((q.n_sls12.HasValue ? q.n_sls12.Value : 0) - (q.n_rtr12.HasValue ? q.n_rtr12.Value : 0)) :
                                              0))))))))))))
                                            )
                                        });

                            var qry = (from q in db.FA_MasItms
                                       join q1 in db.LG_DatSups on q.c_nosup equals q1.c_nosup into q_1
                                       from qDS in q_1.DefaultIfEmpty()
                                       join q2 in GlobalQuery.ViewAverageSales(db, custNo, datePrev) on q.c_iteno equals q2.Item into q_2
                                       from qAVGS in q_2.DefaultIfEmpty()
                                       where (!(from sq in db.LG_SPD1s
                                                where sq.c_spno == spNo
                                                select sq.c_iteno).Contains(q.c_iteno))
                                       select new
                                       {
                                           q.c_alkes,
                                           //q.c_entry,
                                           q.c_iteno,
                                           //q.c_itenopri,
                                           q.c_nosup,
                                           q.c_type,
                                           //q.c_update,
                                           q.c_via,
                                           //q.d_entry,
                                           //q.d_update,
                                           q.l_aktif,
                                           q.l_berat,
                                           q.l_combo,
                                           q.l_dinkes,
                                           q.l_gdg,
                                           q.l_hide,
                                           q.l_import,
                                           q.l_mprice,
                                           q.l_opname,
                                           q.n_beli,
                                           q.n_bonus,
                                           q.n_box,
                                           q.n_disc,
                                           q.n_estimasi,
                                           q.n_PBF,
                                           q.n_pminord,
                                           q.n_qminord,
                                           q.n_qtykons,
                                           q.n_salpri,
                                           q.v_acronim,
                                           q.v_itnam,
                                           q.v_undes,
                                           n_avgSales = (qAVGS == null ? 0 : (qAVGS.Sales - qAVGS.Retur)),
                                           v_nama_suplier = (activateSuplier ? qDS.v_nama : null)
                                       });

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #endregion

                    #region OR

                    #region MODEL_COMMON_QUERY_MULTIPLE_ITEM_OR

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_OR:
                        {
                            var qry = (from q in db.FA_MasItms
                                       join q1 in db.LG_DatSups on q.c_nosup equals q1.c_nosup
                                       join q2 in db.FA_Divpris on q.c_iteno equals q2.c_iteno
                                       join q3 in db.FA_MsDivPris on q2.c_kddivpri equals q3.c_kddivpri into q_3
                                       from qMDP in q_3.DefaultIfEmpty()
                                       //where q.V_itnam.Contains("Albo")
                                       select new
                                       {
                                           q.c_iteno,
                                           q.v_itnam,
                                           q.n_pminord,
                                           q.n_qminord,
                                           q.n_salpri,
                                           q.l_aktif,
                                           q.l_hide,
                                           q.l_combo,
                                           q1.n_index,
                                           q1.c_nosup,
                                           q1.v_nama,
                                           q2.c_kddivpri,
                                           qMDP.v_nmdivpri
                                       }).AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_ITEM_OR_SP

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_OR_SP:
                        {
                            string cust = (parameters.ContainsKey("customer") ? (string)((Functionals.ParameterParser)parameters["customer"]).Value : string.Empty);
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);
                            string tipeProcess = (parameters.ContainsKey("tipeProcess") ? (string)((Functionals.ParameterParser)parameters["tipeProcess"]).Value : string.Empty);
                            string gdg = (parameters.ContainsKey("gudang") ? (string)((Functionals.ParameterParser)parameters["gudang"]).Value : string.Empty);

                            #region Old Coded

                            //var qry = (from q in db.LG_SPHs
                            //           join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
                            //           join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
                            //           where SqlMethods.DateDiffMonth(q.d_spdate, DateTime.Now.Date) < 2
                            //             && q1.n_acc > 0 && q1.n_sisa > 0
                            //             && q1.c_iteno == item
                            //           select new SCMS_UNION_SP_SPG()
                            //           {
                            //             c_cusno = q.c_cusno,
                            //             c_spno = q.c_spno,
                            //             c_sp = q.c_sp.Trim(),
                            //             d_spdate = q.d_spdate,
                            //             n_acc = q1.n_acc,
                            //             n_qty = q1.n_qty,
                            //             n_sisa = q1.n_sisa,
                            //             l_aktif = q2.l_aktif
                            //           }).AsQueryable();

                            //if ((!string.IsNullOrEmpty(tipeProcess)) && (tipeProcess == "04"))
                            //{
                            //  qry = qry.Where(x => x.c_cusno == cust).AsQueryable();
                            //}

                            #endregion

                            var qry = (from q in db.LG_SPHs
                                       join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
                                       join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
                                       join q3 in db.LG_Cusmas on q.c_cusno equals q3.c_cusno
                                       where (SqlMethods.DateDiffMonth(q.d_etasp, DateTime.Now.Date) < 2)
                                        && (q.c_cusno == cust)
                                        && (q1.n_acc > 0) && (q1.n_sisa > 0)
                                        && (q1.c_iteno == item)
                                       select new SCMS_UNION_SP_SPG()
                                       {
                                           c_cusno = q.c_cusno,
                                           c_spno = q.c_spno,
                                           c_sp = q.c_sp.Trim(),
                                           d_spdate = q.d_spdate,
                                           n_acc = (q1.n_acc.HasValue ? q1.n_acc.Value : 0),
                                           n_qty = (q1.n_qty.HasValue ? q1.n_qty.Value : 0),
                                           n_sisa = (q1.n_sisa.HasValue ? q1.n_sisa.Value : 0),
                                           l_aktif = (q2.l_aktif.HasValue ? q2.l_aktif.Value : false)
                                       }).AsQueryable();

                            if (string.IsNullOrEmpty(gdg) && (!tipeProcess.Equals("02")))
                            {
                                if ((!string.IsNullOrEmpty(tipeProcess)) && (tipeProcess == "04"))
                                {
                                    qry = qry.Where(x => x.c_cusno == cust).AsQueryable();
                                }

                                var qry1 = (from q in db.LG_SPGHs
                                            join q1 in db.LG_SPGD1s on new { c_gdg = q.c_gdg1, q.c_spgno } equals new { q1.c_gdg, q1.c_spgno }
                                            join q2 in db.LG_ORD2s on new { c_spno = q.c_spgno, q1.c_iteno } equals new { q2.c_spno, q2.c_iteno } into q_2
                                            from qORD2 in q_2.DefaultIfEmpty()
                                            where (q.l_status == true) && (q1.n_sisa > 0)
                                              && (q1.c_gdg != '1') && (q1.c_iteno == item)
                                              && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                              && (!(from q_sq1 in db.LG_ORHs
                                                    join q_sq2 in db.LG_ORD2s on new { q_sq1.c_gdg, q_sq1.c_orno } equals new { q_sq2.c_gdg, q_sq2.c_orno }
                                                    where (q_sq2.c_spno == q1.c_spgno)
                                                     && ((q_sq2.c_itemcombo == null) || (q_sq2.c_itemcombo == "0000") || (q_sq2.c_itemcombo == ""))
                                                     && ((q_sq1.l_delete.HasValue ? q_sq1.l_delete.Value : false) == false)
                                                    select q_sq2.c_iteno).Contains(q1.c_iteno))
                                            select new SCMS_UNION_SP_SPG()
                                            {
                                                c_cusno = "0000",
                                                c_spno = q.c_spgno,
                                                c_sp = q.c_spgno.Trim(),
                                                d_spdate = q.d_spgdate,
                                                n_acc = (q1.n_spacc.HasValue ? q1.n_spacc.Value : 0),
                                                n_qty = (q1.n_qty.HasValue ? q1.n_qty.Value : 0),
                                                n_sisa = (q1.n_sisa.HasValue ? q1.n_sisa.Value : 0),
                                                l_aktif = (q1.n_sisa.HasValue && (q1.n_sisa.Value > 0))
                                            }).AsQueryable();

                                qry = qry.Union(qry1).AsQueryable();
                            }
                            else if (gdg.Equals("2", StringComparison.OrdinalIgnoreCase) && (!tipeProcess.Equals("02")))
                            {
                                qry = (from q in db.LG_SPGHs
                                       join q1 in db.LG_SPGD1s on new { c_gdg = q.c_gdg1, q.c_spgno } equals new { q1.c_gdg, q1.c_spgno }
                                       //join q2 in db.LG_ORD2s on new { c_spno = q.c_spgno, q1.c_iteno } equals new { q2.c_spno, q2.c_iteno } into q_2
                                       //from qORD2 in q_2.DefaultIfEmpty()
                                       where (q.l_status == true) && (q1.n_sisa > 0)
                                        && (q1.c_gdg != '1') && (q1.c_iteno == item)
                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                        && (!(from q_sq1 in db.LG_ORHs
                                              join q_sq2 in db.LG_ORD2s on new { q_sq1.c_gdg, q_sq1.c_orno } equals new { q_sq2.c_gdg, q_sq2.c_orno }
                                              where (q_sq2.c_spno == q1.c_spgno)
                                                && ((q_sq2.c_itemcombo == null) || (q_sq2.c_itemcombo == "0000") || (q_sq2.c_itemcombo == ""))
                                                && ((q_sq1.l_delete.HasValue ? q_sq1.l_delete.Value : false) == false)
                                              select q_sq2.c_iteno).Contains(q1.c_iteno))
                                       select new SCMS_UNION_SP_SPG()
                                       {
                                           c_cusno = "0000",
                                           c_spno = q.c_spgno,
                                           c_sp = q.c_spgno.Trim(),
                                           d_spdate = q.d_spgdate,
                                           n_acc = (q1.n_spacc.HasValue ? q1.n_spacc.Value : 0),
                                           n_qty = (q1.n_qty.HasValue ? q1.n_qty.Value : 0),
                                           n_sisa = (q1.n_sisa.HasValue ? q1.n_sisa.Value : 0),
                                           l_aktif = (q1.n_sisa.HasValue && (q1.n_sisa.Value > 0))
                                       }).AsQueryable();
                            }
                            else
                            {
                                if ((!string.IsNullOrEmpty(tipeProcess)) && (tipeProcess == "04"))
                                {
                                    qry = qry.Where(x => x.c_cusno == cust).AsQueryable();
                                }
                            }

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_ITEM_ORG_SPG

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_ORG_SPG:
                        {
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);
                            char gdgFrom = '2';

                            var qry = (from q in db.LG_SPHs
                                       join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
                                       join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
                                       join q3 in db.LG_Cusmas on q.c_cusno equals q3.c_cusno
                                       join q4 in db.LG_ORD2s on new { q.c_spno, q1.c_iteno } equals new { q4.c_spno, q4.c_iteno } into q_4
                                       from qORD2 in q_4.DefaultIfEmpty()
                                       where qORD2.c_iteno == null && q3.c_gdg == gdgFrom
                                         && (SqlMethods.DateDiffMonth(q.d_etasp, DateTime.Now.Date) < 2)
                                         && q1.n_acc > 0 && q1.n_sisa > 0
                                         && q1.c_iteno == item
                                       select new
                                       {
                                           q.c_cusno,
                                           q.c_spno,
                                           c_sp = q.c_sp.Trim(),
                                           q.d_spdate,
                                           q1.n_acc,
                                           q1.n_qty,
                                           q1.n_sisa,
                                           q2.l_aktif
                                       }).AsQueryable();

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_PROCESS_QUERY_MULTIPLE_DETIL_INFO_ORP

                    case Constant.MODEL_PROCESS_QUERY_MULTIPLE_DETIL_INFO_ORP:
                        {
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '0');

                            var qrySP = (from q in GlobalQuery.ViewStockSPPendingNew(db)
                                         join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                         where q.c_iteno == item && q.c_type != "SG"
                                         && q.c_gdg == gdg
                                         select new
                                         {
                                             q1.c_iteno,
                                             q1.v_itnam,
                                             q.n_pending,
                                             NoTrans = q.c_spno,
                                             q.v_cunam
                                         }).AsQueryable();

                            var qrySG = (from q in GlobalQuery.ViewStockSPPendingNew(db)
                                         join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                         where q.c_iteno == item && q.c_gdg != gdg && q.c_type == "SG"
                                         select new
                                         {
                                             q1.c_iteno,
                                             q1.v_itnam,
                                             q.n_pending,
                                             NoTrans = q.c_spno,
                                             q.v_cunam
                                         }).AsQueryable();

                            var qry = qrySP.Union(qrySG).AsQueryable();

                            var s = qry.ToList();

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #endregion

                    #region RN

                    #region Pembelian

                    #region MODEL_COMMON_QUERY_MULTIPLE_RN_BELI_ITEM

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_BELI_ITEM:
                        {
                            string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);
                            string tipe = (parameters.ContainsKey("tipe") ? (string)((Functionals.ParameterParser)parameters["tipe"]).Value : string.Empty);

                            var qry = (from q in db.FA_MasItms
                                       where (q.c_nosup == nosup)
                                       select new
                                       {
                                           q.v_itnam,
                                           q.c_iteno
                                       }).AsQueryable();

                            if (tipe.Equals("01", StringComparison.OrdinalIgnoreCase))
                            {
                                DateTime date = new DateTime(2014, 1, 1);

                                qry = (from q in qry
                                       join q1 in db.LG_POD1s on q.c_iteno equals q1.c_iteno
                                       join q2 in db.LG_POHs on new { q1.c_gdg, q1.c_pono } equals new { q2.c_gdg, q2.c_pono }
                                       where ((q2.l_delete == null) || (q2.l_delete == false)) && (q1.n_sisa > 0)
                                           //&& (SqlMethods.DateDiffMonth(q2.d_podate, DateTime.Now) < ((q2.l_import.HasValue && q2.l_import.Value) ? 6 : 2))
                                         && (SqlMethods.DateDiffMonth(q2.d_podate, DateTime.Now) < ((q2.l_import.HasValue && q2.l_import.Value) ? 7 : (q2.d_podate >= date ? 9999 : 7)))
                                       select q).Distinct().AsQueryable();

                            }
                            else
                            {
                                qry = qry.Distinct().AsQueryable();
                            }

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RN_BELI_PO

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_BELI_PO:
                        {
                            string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);
                            string tipe = (parameters.ContainsKey("tipe") ? (string)((Functionals.ParameterParser)parameters["tipe"]).Value : string.Empty);
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);
                            //20170116 Indra.D
                            string cabang = (parameters.ContainsKey("cabang") ? (string)((Functionals.ParameterParser)parameters["cabang"]).Value : string.Empty);
                            DateTime date = new DateTime(2014, 1, 1);

                            var qry = (from q in db.LG_POHs
                                       join q1 in db.LG_POD1s on new { q.c_gdg, q.c_pono } equals new { q1.c_gdg, q1.c_pono }
                                       //20170116 Indra.D
                                       join q2 in db.LG_CusmasCabs on q.c_cab equals q2.c_cab into q_2
                                       from qItm in q_2.DefaultIfEmpty()
                                       //20170116 Indra.D
                                       where ((q.l_delete == null) || (q.l_delete == false)) && (q.c_nosup == nosup)
                                           //&& (SqlMethods.DateDiffMonth(q.d_podate, DateTime.Now) < ((q.l_import.HasValue && q.l_import.Value) ? 6 : 2))
                                         //&& (SqlMethods.DateDiffMonth(q.d_podate, DateTime.Now) < ((q.l_import.HasValue && q.l_import.Value) ? 6 : (q.d_podate >= date ? 9999 : 2)))//remark by suwandi 06 Nov 2018
                                         && (SqlMethods.DateDiffMonth(q.d_podate, DateTime.Now) < ((q.l_import.HasValue && q.l_import.Value) ? 7 : (q.d_podate >= date ? 9999 : 7)))
                                       select new
                                       {
                                           q.c_pono,
                                           q.d_podate,
                                           q1.n_sisa,
                                           q1.c_iteno,
                                           //20170116
                                           //v_cunam = qItm.v_cunam == null ? "-" : qItm.v_cunam 
                                           qItm.v_cunam
                                       }).AsQueryable();

                            if (tipe.Equals("01", StringComparison.OrdinalIgnoreCase))
                            {
                                qry = qry.Where(x => x.c_iteno == item && (x.n_sisa > 0)).Distinct().AsQueryable();
                            }
                            else
                            {
                                qry = qry.Distinct().AsQueryable();
                            }

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }

                        

                        break;

                    #endregion


                    // ulujami

                    #region MODEL_COMMON_QUERY_MULTIPLE_RN_BELI_PO_ULUJAMI

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_BELI_PO_ULUJAMI:
                        {
                            string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);
                            string tipe = (parameters.ContainsKey("tipe") ? (string)((Functionals.ParameterParser)parameters["tipe"]).Value : string.Empty);
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);
                            //20170116 Indra.D
                            string cabang = (parameters.ContainsKey("cabang") ? (string)((Functionals.ParameterParser)parameters["cabang"]).Value : string.Empty);
                            DateTime date = new DateTime(2014, 1, 1);

                            var qry = (from q in db.LG_POHs
                                       join q1 in db.LG_POD1s on new { q.c_gdg, q.c_pono } equals new { q1.c_gdg, q1.c_pono }
                                       //20170116 Indra.D
                                       join q2 in db.LG_CusmasCabs on q.c_cab equals q2.c_cab into q_2
                                       from qItm in q_2.DefaultIfEmpty()
                                       //20170116 Indra.D
                                       where ((q.l_delete == null) || (q.l_delete == false)) && (q.c_nosup == nosup)
                                           //&& (SqlMethods.DateDiffMonth(q.d_podate, DateTime.Now) < ((q.l_import.HasValue && q.l_import.Value) ? 6 : 2))
                                         && (SqlMethods.DateDiffMonth(q.d_podate, DateTime.Now) < ((q.l_import.HasValue && q.l_import.Value) ? 7 : (q.d_podate >= date ? 9999 : 7)))
                                       select new
                                       {
                                           q.c_pono,
                                           q.d_podate,
                                           q1.n_sisa,
                                           q1.c_iteno,
                                           //20170116
                                           //v_cunam = qItm.v_cunam == null ? "-" : qItm.v_cunam 
                                           qItm.v_cunam
                                       }).AsQueryable();

                            if (tipe.Equals("01", StringComparison.OrdinalIgnoreCase))
                            {
                                qry = qry.Where(x => x.c_iteno == item && (x.n_sisa > 0)).Distinct().AsQueryable();
                            }
                            else
                            {
                                qry = qry.Distinct().AsQueryable();
                            }

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }



                        break;

                    #endregion

                    #endregion

                    #region Khusus

                    #region MODEL_COMMON_QUERY_MULTIPLE_RN_KHUSUS_PRINCIPAL

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_KHUSUS_PRINCIPAL:
                        {
                            var qry = (from q in db.LG_DatSups
                                       join q1 in db.LG_DatSupAutos on q.c_nosup equals q1.c_nosup
                                       where q1.l_rn == true
                                       select new
                                       {
                                           q.c_nosup,
                                           q.v_nama,
                                           q.l_hide,
                                           q.l_aktif
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RN_KHUSUS_DO

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_KHUSUS_DO:
                        {
                            string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);

                            var qry = (from q in db.LG_DOPHs
                                       join q1 in db.LG_DOPDs on new { q.c_nosup, q.c_dono } equals new { q1.c_nosup, q1.c_dono }
                                       where (q.c_nosup == nosup)
                                        && (SqlMethods.DateDiffMonth(q.d_dodate, DateTime.Now) < 2)
                                        && (q1.n_qty_sisa > 0)
                                       select new
                                       {
                                           q.c_dono,
                                           q.d_dodate
                                       }).Distinct().AsQueryable();

                            #region Old Coded

                            //var qry = (from q in db.LG_DOPHs
                            //           join q1 in db.LG_DOPDs on new { q.c_nosup, q.c_dono } equals new { q1.c_nosup, q1.c_dono }
                            //           join q2 in
                            //             (from sq in db.LG_RNHs
                            //              join sq1 in db.LG_RND2s on new { sq.c_gdg, sq.c_rnno } equals new { sq1.c_gdg, sq1.c_rnno }
                            //              where sq.c_type == "01"
                            //              select new
                            //              {
                            //                sq.c_dono,
                            //                sq1.c_iteno,
                            //                sq1.c_batch
                            //              }) on new { q1.c_dono, q1.c_iteno, q1.c_batch } equals new { q2.c_dono, q2.c_iteno, q2.c_batch } into q_2
                            //           from qRN in q_2.DefaultIfEmpty()
                            //           where (qRN.c_batch == null) && (q.c_nosup == nosup) 
                            //            && (SqlMethods.DateDiffMonth(q.d_dodate, DateTime.Now) < 2)
                            //           select new
                            //           {
                            //             q.c_dono,
                            //             q.d_dodate
                            //           }).Distinct().AsQueryable();

                            #endregion

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RN_KHUSUS_ITEM

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_KHUSUS_ITEM:
                        {
                            string tipeInput = (parameters.ContainsKey("tipe") ? (string)((Functionals.ParameterParser)parameters["tipe"]).Value : string.Empty);
                            string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);
                            string doKhusus = (parameters.ContainsKey("doKhusus") ? (string)((Functionals.ParameterParser)parameters["doKhusus"]).Value : string.Empty);

                            var qry = (from q in db.LG_DOPDs
                                       join q2 in db.FA_MasItms on q.c_iteno equals q2.c_iteno into q_2
                                       from qItm in q_2.DefaultIfEmpty()
                                       where (q.c_nosup == nosup) && (q.c_dono == doKhusus)
                                        && (q.c_type == tipeInput)
                                           //&& (((q.n_qty.HasValue ? q.n_qty.Value : 0) - (qSQRN.n_gqty.HasValue ? qSQRN.n_gqty.Value : 0)) > 0)
                                        && (q.n_qty_sisa > 0)
                                       select new SCMS_RNKHUSUS_ITEM()
                                       {
                                           v_itnam = qItm.v_itnam,
                                           c_iteno = q.c_iteno,
                                           n_qty = (q.n_qty.HasValue ? q.n_qty.Value : 0),
                                           n_sisa = (q.n_qty_sisa.HasValue ? q.n_qty_sisa.Value : 0)
                                       }).Distinct().AsQueryable();

                            #region Old Coded

                            //var qry = (from q in db.LG_DOPDs
                            //           join qSQ in
                            //             (from sq1 in db.LG_RNHs
                            //              join sq2 in db.LG_RND1s on new { sq1.c_gdg, sq1.c_rnno } equals new { sq2.c_gdg, sq2.c_rnno }
                            //              where (sq1.l_delete == null || sq1.l_delete == false)
                            //              group new { sq1, sq2 } by new { sq1.c_gdg, sq1.c_from, sq1.c_rnno, sq1.c_dono, sq2.c_iteno } into g
                            //              select new
                            //              {
                            //                g.Key.c_gdg,
                            //                c_nosup = g.Key.c_from,
                            //                g.Key.c_rnno,
                            //                g.Key.c_dono,
                            //                g.Key.c_iteno,
                            //                n_gqty = g.Sum(x => x.sq2.n_gqty),
                            //                n_bqty = g.Sum(x => x.sq2.n_bqty)
                            //              }) on new { q.c_nosup, q.c_dono, q.c_iteno } equals new { qSQ.c_nosup, qSQ.c_dono, qSQ.c_iteno } into q_SQ
                            //           from qSQRN in q_SQ.DefaultIfEmpty()
                            //           join q2 in db.FA_MasItms on q.c_iteno equals q2.c_iteno into q_2
                            //           from qItm in q_2.DefaultIfEmpty()
                            //           where q.c_nosup == nosup && q.c_dono == doKhusus
                            //             && (((q.n_qty.HasValue ? q.n_qty.Value : 0) - (qSQRN.n_gqty.HasValue ? qSQRN.n_gqty.Value : 0)) > 0)
                            //           select new SCMS_RNKHUSUS_ITEM()
                            //           {
                            //             v_itnam = qItm.v_itnam,
                            //             c_iteno = q.c_iteno
                            //           }).Distinct().AsQueryable();

                            //if (tipeInput.Equals("02", StringComparison.OrdinalIgnoreCase))
                            //{
                            //  qry = (from q in db.FA_MasItms
                            //         where q.c_nosup == nosup
                            //         select new SCMS_RNKHUSUS_ITEM()
                            //         {
                            //           v_itnam = q.v_itnam,
                            //           c_iteno = q.c_iteno,
                            //           n_qty = 0,
                            //           n_sisa = 0
                            //         }).Distinct().AsQueryable();
                            //}

                            #endregion

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RN_KHUSUS_PO

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_KHUSUS_PO:
                        {
                            string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);
                            string doKhusus = (parameters.ContainsKey("doKhusus") ? (string)((Functionals.ParameterParser)parameters["doKhusus"]).Value : string.Empty);
                            string itemNo = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);

                            var qry = (from q in db.LG_DOPDs
                                       join q1 in
                                           (from sq in db.LG_POHs
                                            join sq1 in db.LG_POD1s on new { sq.c_gdg, sq.c_pono } equals new { sq1.c_gdg, sq1.c_pono }
                                            where (sq.c_nosup == nosup)
                                             && (sq1.n_sisa > 0)
                                             && ((sq.l_delete.HasValue ? sq.l_delete.Value : false) == false)
                                             && (sq1.c_iteno == itemNo)
                                            select new
                                            {
                                                sq.c_pono,
                                                sq.d_podate,
                                                sq1.c_iteno,
                                                sq1.n_qty,
                                                sq1.n_sisa
                                            }) on new { q.c_pono, q.c_iteno } equals new { q1.c_pono, q1.c_iteno } into q_1
                                       from qPO in q_1.DefaultIfEmpty()
                                       where (q.c_nosup == nosup) && (q.c_dono == doKhusus)
                                        && (q.c_iteno == itemNo)
                                           //&& (((q.n_qty.HasValue ? q.n_qty.Value : 0) - (qSQRN.n_gqty.HasValue ? qSQRN.n_gqty.Value : 0)) > 0)
                                        && (q.n_qty_sisa > 0)
                                        && (qPO != null)
                                       select new
                                       {
                                           q.c_pono,
                                           qPO.d_podate,
                                           q.c_batch,
                                           q.d_expired,
                                           n_qty = (q.n_qty.HasValue ? q.n_qty.Value : 0),
                                           //n_sisa = ((q.n_qty.HasValue ? q.n_qty.Value : 0) - (qSQRN.n_gqty.HasValue ? qSQRN.n_gqty.Value : 0)),
                                           n_sisa = (q.n_qty_sisa.HasValue ? q.n_qty_sisa.Value : 0),
                                           n_qty_po = (qPO != null ? (qPO.n_qty.HasValue ? qPO.n_qty.Value : 0) : 0),
                                           n_sisa_po = (qPO != null ? (qPO.n_sisa.HasValue ? qPO.n_sisa.Value : 0) : 0),
                                       }).Distinct().AsQueryable();

                            #region Old Coded

                            //var qry = (from q in db.LG_DOPDs
                            //           join q1 in db.LG_POHs on q.c_pono equals q1.c_pono into q_1
                            //           from qPO in q_1.DefaultIfEmpty()
                            //           join qSQ in
                            //             (from sq1 in db.LG_RNHs
                            //              join sq2 in db.LG_RND1s on new { sq1.c_gdg, sq1.c_rnno } equals new { sq2.c_gdg, sq2.c_rnno }
                            //              where (sq1.l_delete == null || sq1.l_delete == false)
                            //              group new { sq1, sq2 } by new { sq1.c_gdg, sq1.c_from, sq1.c_rnno, sq1.c_dono, sq2.c_iteno } into g
                            //              select new
                            //              {
                            //                g.Key.c_gdg,
                            //                c_nosup = g.Key.c_from,
                            //                g.Key.c_rnno,
                            //                g.Key.c_dono,
                            //                g.Key.c_iteno,
                            //                n_gqty = g.Sum(x => x.sq2.n_gqty),
                            //                n_bqty = g.Sum(x => x.sq2.n_bqty)
                            //              }) on new { q.c_nosup, q.c_dono, q.c_iteno } equals new { qSQ.c_nosup, qSQ.c_dono, qSQ.c_iteno } into q_SQ
                            //           from qSQRN in q_SQ.DefaultIfEmpty()
                            //           where q.c_nosup == nosup && q.c_dono == doKhusus
                            //            && q.c_iteno == itemNo
                            //            && (qPO.l_delete == null || qPO.l_delete == false)
                            //            && (((q.n_qty.HasValue ? q.n_qty.Value : 0) - (qSQRN.n_gqty.HasValue ? qSQRN.n_gqty.Value : 0)) > 0)
                            //           select new
                            //           {
                            //             q.c_pono,
                            //             qPO.d_podate,
                            //             q.c_batch,
                            //             q.d_expired,
                            //             n_qty = (q.n_qty.HasValue ? q.n_qty.Value : 0),
                            //             n_sisa = ((q.n_qty.HasValue ? q.n_qty.Value : 0) - (qSQRN.n_gqty.HasValue ? qSQRN.n_gqty.Value : 0)),
                            //           }).Distinct().AsQueryable();

                            #endregion

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    // ulujami

                    #region MODEL_COMMON_QUERY_MULTIPLE_RN_KHUSUS_PO_ULUJAMI

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_KHUSUS_PO_ULUJAMI:
                        {
                            string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);
                            string doKhusus = (parameters.ContainsKey("doKhusus") ? (string)((Functionals.ParameterParser)parameters["doKhusus"]).Value : string.Empty);
                            string itemNo = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);

                            var qry = (from q in db.LG_DOPDs
                                       join q1 in
                                           (from sq in db.LG_POHs
                                            join sq1 in db.LG_POD1s on new { sq.c_gdg, sq.c_pono } equals new { sq1.c_gdg, sq1.c_pono }
                                            where sq.c_gdg == '6' && (sq.c_nosup == nosup)
                                             && (sq1.n_sisa > 0)
                                             && ((sq.l_delete.HasValue ? sq.l_delete.Value : false) == false)
                                             && (sq1.c_iteno == itemNo)
                                            select new
                                            {
                                                sq.c_pono,
                                                sq.d_podate,
                                                sq1.c_iteno,
                                                sq1.n_qty,
                                                sq1.n_sisa
                                            }) on new { q.c_pono, q.c_iteno } equals new { q1.c_pono, q1.c_iteno } into q_1
                                       from qPO in q_1.DefaultIfEmpty()
                                       where  (q.c_nosup == nosup) && (q.c_dono == doKhusus)
                                        && (q.c_iteno == itemNo)
                                           //&& (((q.n_qty.HasValue ? q.n_qty.Value : 0) - (qSQRN.n_gqty.HasValue ? qSQRN.n_gqty.Value : 0)) > 0)
                                        && (q.n_qty_sisa > 0)
                                        && (qPO != null)
                                       select new
                                       {
                                           q.c_pono,
                                           qPO.d_podate,
                                           q.c_batch,
                                           q.d_expired,
                                           n_qty = (q.n_qty.HasValue ? q.n_qty.Value : 0),
                                           //n_sisa = ((q.n_qty.HasValue ? q.n_qty.Value : 0) - (qSQRN.n_gqty.HasValue ? qSQRN.n_gqty.Value : 0)),
                                           n_sisa = (q.n_qty_sisa.HasValue ? q.n_qty_sisa.Value : 0),
                                           n_qty_po = (qPO != null ? (qPO.n_qty.HasValue ? qPO.n_qty.Value : 0) : 0),
                                           n_sisa_po = (qPO != null ? (qPO.n_sisa.HasValue ? qPO.n_sisa.Value : 0) : 0),
                                       }).Distinct().AsQueryable();

                            var www = qry.ToList();

                            #region Old Coded

                            //var qry = (from q in db.LG_DOPDs
                            //           join q1 in db.LG_POHs on q.c_pono equals q1.c_pono into q_1
                            //           from qPO in q_1.DefaultIfEmpty()
                            //           join qSQ in
                            //             (from sq1 in db.LG_RNHs
                            //              join sq2 in db.LG_RND1s on new { sq1.c_gdg, sq1.c_rnno } equals new { sq2.c_gdg, sq2.c_rnno }
                            //              where (sq1.l_delete == null || sq1.l_delete == false)
                            //              group new { sq1, sq2 } by new { sq1.c_gdg, sq1.c_from, sq1.c_rnno, sq1.c_dono, sq2.c_iteno } into g
                            //              select new
                            //              {
                            //                g.Key.c_gdg,
                            //                c_nosup = g.Key.c_from,
                            //                g.Key.c_rnno,
                            //                g.Key.c_dono,
                            //                g.Key.c_iteno,
                            //                n_gqty = g.Sum(x => x.sq2.n_gqty),
                            //                n_bqty = g.Sum(x => x.sq2.n_bqty)
                            //              }) on new { q.c_nosup, q.c_dono, q.c_iteno } equals new { qSQ.c_nosup, qSQ.c_dono, qSQ.c_iteno } into q_SQ
                            //           from qSQRN in q_SQ.DefaultIfEmpty()
                            //           where q.c_nosup == nosup && q.c_dono == doKhusus
                            //            && q.c_iteno == itemNo
                            //            && (qPO.l_delete == null || qPO.l_delete == false)
                            //            && (((q.n_qty.HasValue ? q.n_qty.Value : 0) - (qSQRN.n_gqty.HasValue ? qSQRN.n_gqty.Value : 0)) > 0)
                            //           select new
                            //           {
                            //             q.c_pono,
                            //             qPO.d_podate,
                            //             q.c_batch,
                            //             q.d_expired,
                            //             n_qty = (q.n_qty.HasValue ? q.n_qty.Value : 0),
                            //             n_sisa = ((q.n_qty.HasValue ? q.n_qty.Value : 0) - (qSQRN.n_gqty.HasValue ? qSQRN.n_gqty.Value : 0)),
                            //           }).Distinct().AsQueryable();

                            #endregion

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #endregion

                    #region Transfer

                    #region MODEL_COMMON_QUERY_MULTIPLE_RN_TRANSFER_SURATJALAN

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_TRANSFER_SURATJALAN:
                        {
                            char gdg = (parameters.ContainsKey("gudang") ? (char)((Functionals.ParameterParser)parameters["gudang"]).Value : char.MinValue);

                            var qry = (from q in db.LG_SJHs
                                       join q1 in db.LG_MsGudangs on q.c_gdg equals q1.c_gdg
                                       where q.l_confirm == true && (q.l_delete == null || q.l_delete == false) &&
                                         q.l_print == true && q.l_status == false && q.c_gdg2 == gdg
                                       select new
                                       {
                                           q.c_sjno,
                                           q.d_sjdate,
                                           q1.v_gdgdesc,
                                           q.c_pin
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #endregion

                    #region Claim

                    #region MODEL_COMMON_QUERY_MULTIPLE_RN_ClAIM_ITEM

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_ClAIM_ITEM:
                        {
                            string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);
                            string tipe = (parameters.ContainsKey("tipe") ? (string)((Functionals.ParameterParser)parameters["tipe"]).Value : string.Empty);
                            char gdg = (parameters.ContainsKey("gudang") ? (char)((Functionals.ParameterParser)parameters["gudang"]).Value : char.MinValue);

                            //var qry = (from q in db.FA_MasItms
                            //           where (q.c_nosup == nosup)
                            //           select new
                            //           {
                            //             q.v_itnam,
                            //             q.c_iteno
                            //           }).AsQueryable();


                            //qry = (from q in qry
                            //       join q1 in db.LG_ClaimAccDs on q.c_iteno equals q1.c_iteno
                            //       join q2 in db.LG_ClaimAccHes on new { q1.c_claimaccno } equals new { q2.c_claimaccno }
                            //       where ((q2.l_delete == null) || (q2.l_delete == false)) && (q1.n_sisa > 0)
                            //       && q2.c_nosup == nosup
                            //       select q).Distinct().AsQueryable();

                            var qry = (from q in db.FA_MasItms
                                       join q1 in db.LG_ClaimAccDs on q.c_iteno equals q1.c_iteno
                                       join q2 in db.LG_ClaimAccHes on new { q1.c_claimaccno } equals new { q2.c_claimaccno }
                                       where ((q2.l_delete == null) || (q2.l_delete == false)) && (q1.n_sisa > 0)
                                       && q2.c_nosup == nosup
                                       select q).Distinct().AsQueryable();


                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RN_ClAIM_NOCLAIM

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_ClAIM_NOCLAIM:
                        {
                            string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);
                            string tipe = (parameters.ContainsKey("tipe") ? (string)((Functionals.ParameterParser)parameters["tipe"]).Value : string.Empty);
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);

                            var qry = (from q in db.LG_ClaimAccHes
                                       join q1 in db.LG_ClaimAccDs on new { q.c_claimaccno } equals new { q1.c_claimaccno }
                                       where ((q.l_delete == null) || (q.l_delete == false))
                                       && q.c_nosup == nosup && q1.c_iteno == item
                                       select new
                                       {
                                           q.c_claimaccno,
                                           q.d_claimaccdate,
                                           q1.n_sisa,
                                           q1.c_iteno,
                                           q.c_claimnoprinc
                                       }).AsQueryable();

                            qry = qry.Where(x => x.c_iteno == item && (x.n_sisa > 0)).AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion


                    // Ulujami Claim item

                    #region MODEL_COMMON_QUERY_MULTIPLE_RN_ClAIM_ITEM_ULUJAMI

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_ClAIM_ITEM_ULUJAMI:
                        {
                            string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);
                            string tipe = (parameters.ContainsKey("tipe") ? (string)((Functionals.ParameterParser)parameters["tipe"]).Value : string.Empty);
                            char gdg = (parameters.ContainsKey("gudang") ? (char)((Functionals.ParameterParser)parameters["gudang"]).Value : char.MinValue);

                            //var qry = (from q in db.FA_MasItms
                            //           where (q.c_nosup == nosup)
                            //           select new
                            //           {
                            //             q.v_itnam,
                            //             q.c_iteno
                            //           }).AsQueryable();


                            //qry = (from q in qry
                            //       join q1 in db.LG_ClaimAccDs on q.c_iteno equals q1.c_iteno
                            //       join q2 in db.LG_ClaimAccHes on new { q1.c_claimaccno } equals new { q2.c_claimaccno }
                            //       where ((q2.l_delete == null) || (q2.l_delete == false)) && (q1.n_sisa > 0)
                            //       && q2.c_nosup == nosup
                            //       select q).Distinct().AsQueryable();

                            var qry = (from q in db.FA_MasItms
                                       join q1 in db.LG_ClaimAccDs on q.c_iteno equals q1.c_iteno
                                       join q2 in db.LG_ClaimAccHes on new { q1.c_claimaccno } equals new { q2.c_claimaccno }
                                       where ((q2.l_delete == null) || (q2.l_delete == false)) && (q1.n_sisa > 0)
                                       && q2.c_nosup == nosup
                                       select q).Distinct().AsQueryable();


                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    // Ulujami Calim Nomor CA

                    #region MODEL_COMMON_QUERY_MULTIPLE_RN_ClAIM_NOCLAIM_ULUJAMI

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_ClAIM_NOCLAIM_ULUJAMI:
                        {
                            string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);
                            string tipe = (parameters.ContainsKey("tipe") ? (string)((Functionals.ParameterParser)parameters["tipe"]).Value : string.Empty);
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);

                            var qry = (from q in db.LG_ClaimAccHes
                                       join q1 in db.LG_ClaimAccDs on new { q.c_claimaccno } equals new { q1.c_claimaccno }
                                       where ((q.l_delete == null) || (q.l_delete == false))
                                       && q.c_nosup == nosup && q1.c_iteno == item
                                       select new
                                       {
                                           q.c_claimaccno,
                                           q.d_claimaccdate,
                                           q1.n_sisa,
                                           q1.c_iteno,
                                           q.c_claimnoprinc
                                       }).AsQueryable();

                            qry = qry.Where(x => x.c_iteno == item && (x.n_sisa > 0)).AsQueryable();

                            var xxx = qry.ToList();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #endregion

                    #region Retur

                    #region MODEL_COMMON_QUERY_MULTIPLE_RN_RETUR_ITEM

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_RETUR_ITEM:
                        {
                            string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);
                            char gdg = (parameters.ContainsKey("gudang") ? (char)((Functionals.ParameterParser)parameters["gudang"]).Value : char.MinValue);

                            var qry = (from q in db.FA_MasItms
                                       where (q.c_nosup == nosup)
                                       select new
                                       {
                                           q.v_itnam,
                                           q.c_iteno
                                       }).AsQueryable();

                            qry = (from q in qry
                                   join q1 in db.LG_RSD2s on q.c_iteno equals q1.c_iteno
                                   join q2 in db.LG_RSHes on new { q1.c_gdg, q1.c_rsno } equals new { q2.c_gdg, q2.c_rsno }
                                   where ((q2.l_delete == null) || (q2.l_delete == false)) && ((q1.n_bsisa > 0) || (q1.n_gsisa > 0))
                                   && q2.c_nosup == nosup && q1.c_gdg == gdg && q2.c_type != "03"
                                   select q).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RN_RETUR_RS

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_RETUR_RS:
                        {
                            string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);
                            char gdg = (parameters.ContainsKey("gudang") ? (char)((Functionals.ParameterParser)parameters["gudang"]).Value : char.MinValue);
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);

                            var qry = (from q in db.LG_RSHes
                                       join q1 in db.LG_RSD2s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                                       join q2 in db.LG_RSD3s on new { q1.c_gdg, q.c_rsno, q1.c_iteno, q1.c_batch } equals new { c_gdg = q2.c_gdg.Value, q2.c_rsno, q2.c_iteno, q2.c_batch } into g
                                       from qO2 in g.DefaultIfEmpty()
                                       where ((q.l_delete == null) || (q.l_delete == false)) && (q.c_nosup == nosup)
                                         && (q.c_gdg == gdg) && q1.c_iteno == item
                                         && (q.c_type != "03")
                                         && qO2.c_rsno == null && (q1.n_bsisa > 0 || q1.n_gsisa > 0)
                                       select new
                                       {
                                           q.c_rsno,
                                           q.d_rsdate,
                                           q.c_cprno,
                                           //q1.n_bsisa,
                                           //q1.n_gsisa,
                                           q1.c_iteno
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RN_RETUR_BATCH

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_RETUR_BATCH:
                        {
                            string rsno = (parameters.ContainsKey("rsno") ? (string)((Functionals.ParameterParser)parameters["rsno"]).Value : string.Empty);
                            string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);
                            char gdg = (parameters.ContainsKey("gudang") ? (char)((Functionals.ParameterParser)parameters["gudang"]).Value : char.MinValue);
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);

                            var qry = (from q in db.LG_RSHes
                                       join q1 in db.LG_RSD2s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                                       join q2 in db.LG_RSD3s on new { q1.c_gdg, q.c_rsno, q1.c_iteno, q1.c_batch } equals new { c_gdg = q2.c_gdg.Value, q2.c_rsno, q2.c_iteno, q2.c_batch } into g
                                       from qO2 in g.DefaultIfEmpty()
                                       where ((q.l_delete == null) || (q.l_delete == false)) && (q.c_nosup == nosup)
                                         && (q.c_gdg == gdg) && q1.c_iteno == item
                                         && (q.c_type != "03") && (q.c_rsno == rsno)
                                         && qO2.c_rsno == null && (q1.n_bsisa > 0 || q1.n_gsisa > 0)
                                       group q1 by q1.c_batch into gSum
                                       select new
                                       {
                                           n_bsisa = gSum.Sum(x => x.n_bsisa),
                                           c_batch = gSum.Key,
                                           n_gsisa = gSum.Sum(x => x.n_gsisa),
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #endregion

                    #region Repack

                    #region MODEL_COMMON_QUERY_MULTIPLE_RN_REPACK_ITEM

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_REPACK_ITEM:
                        {
                            string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);
                            char gdg = (parameters.ContainsKey("gudang") ? (char)((Functionals.ParameterParser)parameters["gudang"]).Value : char.MinValue);

                            var listType = new string[] { "02", "03" };

                            var qry = (from q in db.FA_MasItms
                                       where (q.c_nosup == nosup)
                                       select new
                                       {
                                           q.v_itnam,
                                           q.c_iteno
                                       }).AsQueryable();


                            qry = (from q in qry
                                   join q1 in db.LG_RSD2s on q.c_iteno equals q1.c_iteno
                                   join q2 in db.LG_RSHes on new { q1.c_gdg, q1.c_rsno } equals new { q2.c_gdg, q2.c_rsno }
                                   where ((q2.l_delete == null) || (q2.l_delete == false)) && ((q1.n_bsisa > 0) || (q1.n_gsisa > 0))
                                   && q2.c_nosup == nosup && q1.c_gdg == gdg && listType.Contains(q2.c_type)
                                   select q).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RN_REPACK_RS

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_REPACK_RS:
                        {
                            string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);
                            char gdg = (parameters.ContainsKey("gudang") ? (char)((Functionals.ParameterParser)parameters["gudang"]).Value : char.MinValue);
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);

                            string[] typeAvaible = new string[] { "02", "03" };

                            var qry = (from q in db.LG_RSHes
                                       join q1 in db.LG_RSD2s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                                       join q2 in db.LG_RSD3s on new { q1.c_gdg, q.c_rsno, q1.c_iteno, q1.c_batch } equals new { c_gdg = q2.c_gdg.Value, q2.c_rsno, q2.c_iteno, q2.c_batch } into g
                                       from qO2 in g.DefaultIfEmpty()
                                       where ((q.l_delete == null) || (q.l_delete == false)) && (q.c_nosup == nosup)
                                         && (q.c_gdg == gdg) && (q1.c_iteno == item)
                                         && ((q1.n_bsisa > 0) || (q1.n_gsisa > 0))
                                         && typeAvaible.Contains(q.c_type) && (qO2.c_rsno == null)
                                       select new
                                       {
                                           q.c_rsno,
                                           q.d_rsdate,
                                           q.c_cprno,
                                           q.l_confirm,
                                           q.d_confirm,
                                           q1.c_iteno
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RN_REPACK_BATCH

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RN_REPACK_BATCH:
                        {
                            string rsno = (parameters.ContainsKey("rsno") ? (string)((Functionals.ParameterParser)parameters["rsno"]).Value : string.Empty);
                            string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);
                            char gdg = (parameters.ContainsKey("gudang") ? (char)((Functionals.ParameterParser)parameters["gudang"]).Value : char.MinValue);
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);

                            string[] typeAvaible = new string[] { "02", "03" };

                            var qry = (from q in db.LG_RSHes
                                       join q1 in db.LG_RSD2s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                                       join q2 in db.LG_RSD3s on new { q1.c_gdg, q.c_rsno, q1.c_iteno, q1.c_batch } equals new { c_gdg = q2.c_gdg.Value, q2.c_rsno, q2.c_iteno, q2.c_batch } into g
                                       from qO2 in g.DefaultIfEmpty()
                                       where ((q.l_delete == null) || (q.l_delete == false)) && (q.c_nosup == nosup)
                                         && (q.c_gdg == gdg) && (q1.c_iteno == item)
                                         && ((q1.n_bsisa > 0) || (q1.n_gsisa > 0))
                                           //&& (q.c_type == "02") && (q.c_rsno == rsno)
                                         && typeAvaible.Contains(q.c_type) && (q.c_rsno == rsno)
                                         && ((q1.n_bsisa > 0 || q1.n_gsisa > 0))
                                       group q1 by new { q1.c_iteno, q1.c_batch } into g
                                       select new
                                       {
                                           g.Key.c_iteno,
                                           g.Key.c_batch,
                                           n_gsisa = g.Sum(x => (x.n_gsisa.HasValue ? x.n_gsisa.Value : 0)),
                                           n_bsisa = g.Sum(x => (x.n_bsisa.HasValue ? x.n_bsisa.Value : 0))
                                           //q1.n_bsisa,
                                           //q1.c_batch,
                                           //q1.n_gsisa,
                                           //q1.c_iteno
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #endregion

                    #region MODEL_COMMON_QUERY_OUTSTANDPO_RN

                    case Constant.MODEL_COMMON_QUERY_OUTSTANDPO_RN:
                        {
                            string PONO = (parameters.ContainsKey("pono") ? (string)((Functionals.ParameterParser)parameters["pono"]).Value : string.Empty);

                            string[] typeAvaible = new string[] { "02", "03" };

                            var qry = (from q in db.LG_POD1s
                                   join q1 in db.LG_RND2s on new { q.c_pono, q.c_iteno }equals new { c_pono = q1.c_no, q1.c_iteno } into g
                                   from q2 in g.DefaultIfEmpty()
                                   where q.c_pono == PONO
                                   select new
                                   {
                                       q.c_pono,q.c_iteno,q.n_qty,q.n_sisa,q2.c_rnno,q2.c_batch,q2.n_gqty
                                   }).Distinct().AsQueryable();

                            //var qry = (from q in db.LG_RSHes
                            //           join q1 in db.LG_RSD2s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                            //           join q2 in db.LG_RSD3s on new { q1.c_gdg, q.c_rsno, q1.c_iteno, q1.c_batch } equals new { c_gdg = q2.c_gdg.Value, q2.c_rsno, q2.c_iteno, q2.c_batch } into g
                            //           from qO2 in g.DefaultIfEmpty()
                            //           where ((q.l_delete == null) || (q.l_delete == false)) && (q.c_nosup == nosup)
                            //             && (q.c_gdg == gdg) && (q1.c_iteno == item)
                            //             && ((q1.n_bsisa > 0) || (q1.n_gsisa > 0))
                            //               //&& (q.c_type == "02") && (q.c_rsno == rsno)
                            //             && typeAvaible.Contains(q.c_type) && (q.c_rsno == rsno)
                            //             && ((q1.n_bsisa > 0 || q1.n_gsisa > 0))
                            //           group q1 by new { q1.c_iteno, q1.c_batch } into g
                            //           select new
                            //           {
                            //               g.Key.c_iteno,
                            //               g.Key.c_batch,
                            //               n_gsisa = g.Sum(x => (x.n_gsisa.HasValue ? x.n_gsisa.Value : 0)),
                            //               n_bsisa = g.Sum(x => (x.n_bsisa.HasValue ? x.n_bsisa.Value : 0))
                            //               //q1.n_bsisa,
                            //               //q1.c_batch,
                            //               //q1.n_gsisa,
                            //               //q1.c_iteno
                            //           }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMO_QUERY_PO_SP

                    case Constant.MODEL_COMMO_QUERY_PO_SP:
                        {
                            string PONO = (parameters.ContainsKey("pono") ? (string)((Functionals.ParameterParser)parameters["pono"]).Value : string.Empty);

                            string[] typeAvaible = new string[] { "02", "03" };

                            var qry = (from q in db.LG_POD2s
                                       join q1 in db.LG_POD1s on q.c_pono equals q1.c_pono
                                       join q2 in db.LG_ORD2s on new { q.c_orno, q1.c_iteno } equals new { q2.c_orno, q2.c_iteno }
                                       where q.c_pono == PONO
                                       select new
                                       {
                                           q.c_pono,
                                           q1.c_iteno,
                                           q1.n_qty,
                                           q2.c_spno,
                                           q2.n_sisa,
                                       }).Distinct().AsQueryable();

                            //var qry = (from q in db.LG_RSHes
                            //           join q1 in db.LG_RSD2s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                            //           join q2 in db.LG_RSD3s on new { q1.c_gdg, q.c_rsno, q1.c_iteno, q1.c_batch } equals new { c_gdg = q2.c_gdg.Value, q2.c_rsno, q2.c_iteno, q2.c_batch } into g
                            //           from qO2 in g.DefaultIfEmpty()
                            //           where ((q.l_delete == null) || (q.l_delete == false)) && (q.c_nosup == nosup)
                            //             && (q.c_gdg == gdg) && (q1.c_iteno == item)
                            //             && ((q1.n_bsisa > 0) || (q1.n_gsisa > 0))
                            //               //&& (q.c_type == "02") && (q.c_rsno == rsno)
                            //             && typeAvaible.Contains(q.c_type) && (q.c_rsno == rsno)
                            //             && ((q1.n_bsisa > 0 || q1.n_gsisa > 0))
                            //           group q1 by new { q1.c_iteno, q1.c_batch } into g
                            //           select new
                            //           {
                            //               g.Key.c_iteno,
                            //               g.Key.c_batch,
                            //               n_gsisa = g.Sum(x => (x.n_gsisa.HasValue ? x.n_gsisa.Value : 0)),
                            //               n_bsisa = g.Sum(x => (x.n_bsisa.HasValue ? x.n_bsisa.Value : 0))
                            //               //q1.n_bsisa,
                            //               //q1.c_batch,
                            //               //q1.n_gsisa,
                            //               //q1.c_iteno
                            //           }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #endregion

                    #region MK

                    #region Memo Combo

                    case Constant.MODEL_PROCESS_QUERY_MULTIPLE_ITEM_MKMEMOCOMBO:
                        {
                            var qry = (from q in db.FA_MasItms
                                       //join q1 in db.MK_MemoDs on new { q.C_iteno, C_gdg = gdg, C_memono = memoId } equals new { q1.C_iteno, q1.C_gdg, q1.C_memono } into q_1
                                       //join q1 in db.MK_MemoDs on q.c_iteno equals q1.c_iteno into q_1
                                       //from qMeD in q_1.DefaultIfEmpty()
                                       where (q.l_aktif == true) && (q.l_combo == true) //&& (qMeD.c_iteno == null)
                                       select new
                                       {
                                           q.c_iteno,
                                           q.v_itnam,
                                           q.n_box
                                       }).AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #endregion

                    #region Memo

                    #region Memo Combo

                    #region MODEL_PROCESS_QUERY_MULTIPLE_MEMO_MEMO_MEMOCOMBO

                    case Constant.MODEL_PROCESS_QUERY_MULTIPLE_MEMO_MEMO_MEMOCOMBO:
                        {
                            var qry = (from q in db.MK_MemoHs
                                       join q1 in db.MK_MemoDs on new { q.c_gdg, q.c_memono } equals new { q1.c_gdg, q1.c_memono }
                                       where q1.n_sisa > 0
                                       select new
                                       {
                                           q.c_memono,
                                           q.c_memo,
                                           q.d_memodate,
                                           q.c_gdg
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_PROCESS_QUERY_MULTIPLE_MEMO_ITEM_MEMOCOMBO

                    case Constant.MODEL_PROCESS_QUERY_MULTIPLE_MEMO_ITEM_MEMOCOMBO:
                        {
                            var qry = (from q in db.FA_MasItms
                                       join q1 in db.MK_MemoDs on q.c_iteno equals q1.c_iteno
                                       where q.l_aktif == true && q.l_combo == true && q1.n_sisa > 0
                                       select new
                                       {
                                           q.v_itnam,
                                           q.c_iteno,
                                           q1.c_gdg,
                                           q1.c_memono,
                                           q1.n_qty,
                                           q1.n_sisa
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_PROCESS_QUERY_MULTIPLE_MEMO_COMBO_ITEM_MEMOCOMBO

                    case Constant.MODEL_PROCESS_QUERY_MULTIPLE_MEMO_COMBO_ITEM_MEMOCOMBO:
                        {
                            char gdg = (parameters.ContainsKey("gudang") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gudang"]).Value) : char.MinValue);
                            string itemCombo = (parameters.ContainsKey("itemCombo") ? (string)((Functionals.ParameterParser)parameters["itemCombo"]).Value : string.Empty);
                            decimal comboQty = (parameters.ContainsKey("comboQty") ? (decimal)((Functionals.ParameterParser)parameters["comboQty"]).Value : 0);

                            var qry = (from q in db.FA_Combos
                                       join q1 in
                                           (from sq in GlobalQuery.ViewStockLite(db, gdg)
                                            where sq.c_gdg == gdg
                                            group sq by sq.c_iteno into g
                                            select new
                                            {
                                                c_iteno = g.Key,
                                                n_qty = g.Sum(x => x.n_gsisa),
                                            }) on q.c_iteno equals q1.c_iteno
                                       join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
                                       where (q.c_combo == itemCombo) && (q1.n_qty > 0)
                                       select new
                                       {
                                           c_iteno = q.c_iteno,
                                           v_itnam = q2.v_itnam,
                                           n_qty = q1.n_qty,
                                           n_qty_request = (comboQty * q.n_qty),
                                           n_qty_komposisi = q.n_qty
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_PROCESS_QUERY_MULTIPLE_MEMO_COMBO_BATCH_MEMOCOMBO

                    case Constant.MODEL_PROCESS_QUERY_MULTIPLE_MEMO_COMBO_BATCH_MEMOCOMBO:
                        {
                            char gdg = (parameters.ContainsKey("gudang") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gudang"]).Value) : char.MinValue);
                            string itemCode = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);

                            var qry = (from q in
                                           (from sq in GlobalQuery.ViewStockLite(db, gdg)
                                            where sq.c_iteno == itemCode && sq.c_gdg == gdg
                                            group sq by new { sq.c_iteno, sq.c_batch } into g
                                            select new
                                            {
                                                g.Key.c_batch,
                                                g.Key.c_iteno,
                                                n_qty = g.Sum(x => x.n_gsisa)
                                            })
                                       join q1 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q1.c_iteno, q1.c_batch } into q_1
                                       from qBat in q_1.DefaultIfEmpty()
                                       where (q.n_qty > 0)
                                       select new
                                       {
                                           q.c_iteno,
                                           q.c_batch,
                                           q.n_qty,
                                           qBat.d_expired
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #endregion

                    #endregion

                    #region RC

                    #region MODEL_COMMON_QUERY_MULTIPLE_RC_BATCH

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RC_BATCH:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);

                            //var qry = (from q in
                            //             (from vw in GlobalQuery.ViewStockLite(db)
                            //              where vw.c_gdg == gdg
                            //              group vw by new { vw.c_gdg, vw.c_iteno, vw.c_batch } into g
                            //              select new
                            //              {
                            //                c_gdg = g.Key.c_gdg,
                            //                c_iteno = g.Key.c_iteno,
                            //                c_batch = g.Key.c_batch,
                            //                n_gsisa = g.Sum(x => x.n_gsisa)
                            //              })
                            //           join bat in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { bat.c_iteno, bat.c_batch }
                            //           where q.c_gdg == gdg && q.c_iteno == item && q.n_gsisa != 0
                            //           select new
                            //           {
                            //             bat.c_iteno,
                            //             bat.c_batch,
                            //             bat.d_expired,
                            //             n_qtybatch = q.n_gsisa
                            //           }).Distinct().AsQueryable();

                            var qry = (from q in db.LG_MsBatches
                                       where (q.c_iteno == item)
                                       orderby q.d_expired
                                       select new
                                       {
                                           q.c_iteno,
                                           q.c_batch,
                                           q.d_expired
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RC_ITEM

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RC_ITEM:
                        {
                            string supl = (parameters.ContainsKey("supl") ? (string)((Functionals.ParameterParser)parameters["supl"]).Value : string.Empty);

                            var qry = (from q in db.FA_MasItms
                                       where q.l_aktif == true && q.c_nosup == supl
                                       select new
                                       {
                                           q.c_iteno,
                                           q.v_itnam,
                                           q.c_nosup
                                       });

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RC_DO

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RC_DO:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);
                            string cusno = (parameters.ContainsKey("cusno") ? (string)((Functionals.ParameterParser)parameters["cusno"]).Value : string.Empty);
                            string batch = (parameters.ContainsKey("batch") ? (string)((Functionals.ParameterParser)parameters["batch"]).Value : string.Empty);

                            #region PL

                            var qryPL = (from q in db.LG_DOHs
                                         join q1 in db.LG_DOD1s on q.c_dono equals q1.c_dono
                                         //join q2 in db.LG_PLD2s on new { q.c_plno, q1.c_iteno } equals new { q2.c_plno, q2.c_iteno }
                                         join q2 in
                                             (from q4 in db.LG_PLD2s
                                              group q4 by new { q4.c_plno, q4.c_iteno, q4.c_rnno, q4.c_batch } into qsum
                                              select new
                                              {
                                                  c_plno = qsum.Key.c_plno,
                                                  c_rnno = qsum.Key.c_rnno,
                                                  c_iteno = qsum.Key.c_iteno,
                                                  c_batch = qsum.Key.c_batch,
                                                  n_qty = qsum.Sum(x => x.n_qty)
                                              }) on new { q.c_plno, q1.c_iteno } equals new { q2.c_plno, q2.c_iteno }
                                         where (q.c_gdg == gdg) && (q.c_cusno == cusno)
                                          && (q1.n_qty > 0) && (q2.c_batch == batch) && (q1.c_iteno == item)
                                         select new TempUnionDOJoin()
                                         {
                                             c_gdg = (q.c_gdg.HasValue ? q.c_gdg.Value : char.MinValue),
                                             c_dono = (q.c_dono ?? string.Empty),
                                             d_dodate = (q.d_dodate.HasValue ? q.d_dodate.Value : Functionals.StandardSqlDateTime),
                                             n_qty = (q2.n_qty.HasValue ? q2.n_qty.Value : 0),
                                             c_rnno = (q2.c_rnno ?? string.Empty)
                                         }).Distinct().AsQueryable();

                            #region Old Coded

                            //var qry = (from q in db.LG_DOHs
                            //           join q1 in db.LG_DOD2s on new { q.c_dono, q.c_via } equals new { q1.c_dono, q1.c_via }
                            //           where (q.c_gdg == gdg) && (q.c_cusno == cusno) && (q1.c_batch == batch) && (q1.c_iteno == item)
                            //            && (q1.n_sisa > 0)
                            //           select new
                            //           {
                            //             q.c_gdg,
                            //             q.c_dono,
                            //             q.d_dodate,
                            //             q1.c_iteno,
                            //             q1.c_batch,
                            //             q1.n_qty
                            //           }).Distinct().AsQueryable();

                            #endregion

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qryPL = qryPL.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qryPL = qryPL.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            //if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                            //{
                            //  qryPL = qryPL.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                            //}

                            #endregion

                            #region STT

                            var qrySTT = (from q in db.LG_DOHs
                                          join q1 in db.LG_DOD1s on q.c_dono equals q1.c_dono
                                          join q2 in db.LG_STD2s on new { q.c_plno, q1.c_iteno } equals new { c_plno = q2.c_stno, q2.c_iteno }
                                          where (q.c_gdg == gdg) && (q.c_cusno == cusno)
                                           && (q1.n_qty > 0) && (q2.c_batch == batch) && (q1.c_iteno == item)
                                          select new TempUnionDOJoin()
                                          {
                                              c_gdg = (q.c_gdg.HasValue ? q.c_gdg.Value : char.MinValue),
                                              c_dono = (q.c_dono ?? string.Empty),
                                              d_dodate = (q.d_dodate.HasValue ? q.d_dodate.Value : Functionals.StandardSqlDateTime),
                                              n_qty = (q2.n_qty.HasValue ? q2.n_qty.Value : 0),
                                              c_rnno = (q2.c_no ?? string.Empty)
                                          }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qrySTT = qrySTT.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qrySTT = qrySTT.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            //if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                            //{
                            //  qrySTT = qrySTT.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                            //}

                            #endregion

                            var qry = qryPL.ToList().Union(qrySTT.ToList()).AsQueryable();

                            nCount = qry.Count();

                            Logger.WriteLine(qry.Provider.ToString());

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;
                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RC_RN

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RC_RN:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);
                            string cusno = (parameters.ContainsKey("cusno") ? (string)((Functionals.ParameterParser)parameters["cusno"]).Value : string.Empty);
                            string batch = (parameters.ContainsKey("batch") ? (string)((Functionals.ParameterParser)parameters["batch"]).Value : string.Empty);
                            string dono = (parameters.ContainsKey("dono") ? (string)((Functionals.ParameterParser)parameters["dono"]).Value : string.Empty);

                            var qry = (from q in db.LG_DOHs
                                       join q1 in db.LG_DOD2s on new { q.c_dono, q.c_via } equals new { q1.c_dono, q1.c_via }
                                       join q2 in db.LG_PLD2s on new { q.c_plno, q1.c_iteno, q1.c_batch } equals new { q2.c_plno, q2.c_iteno, q2.c_batch }
                                       join q3 in db.LG_PLHs on q2.c_plno equals q3.c_plno
                                       join q4 in db.LG_RNHs on new { c_gdg = q3.c_gdg.Value, q2.c_rnno } equals new { c_gdg = q4.c_gdg, q4.c_rnno }
                                       where q.c_dono == dono && q.c_cusno == cusno && q1.c_iteno == item && q2.c_batch == batch
                                       && q1.n_sisa > 0
                                       group new { q, q2 } by new { q.c_gdg, q.c_dono, q.c_cusno, q.c_plno, q2.c_rnno } into g
                                       select new
                                       {
                                           c_gdg = g.Key.c_gdg,
                                           c_dono = g.Key.c_dono,
                                           c_cusno = g.Key.c_cusno,
                                           c_plno = g.Key.c_plno,
                                           c_rnno = g.Key.c_rnno,
                                           n_qty = g.Sum(x => x.q2.n_qty)
                                       }).Distinct().AsQueryable();

                            //var QtyDO = (from q in db.LG_DOD2s
                            //             join q1 in db.LG_DOHs on q.c_dono equals q1.c_dono
                            //             where q.c_dono == dono && q.c_batch == batch
                            //             && q.c_iteno == item && q1.c_gdg == gdg && q1.c_cusno == cusno
                            //             select new
                            //             {
                            //               q1.c_gdg,
                            //               q1.c_dono,
                            //               q1.c_plno,
                            //               q.c_iteno,
                            //               q.c_batch,
                            //               q.n_qty
                            //             }).Distinct().AsQueryable();

                            //var qry = (from q in QtyDO
                            //           join q1 in db.LG_PLD2s on 
                            //             new {q.c_plno, q.c_iteno, q.c_batch } 
                            //             equals new {q1.c_plno, q1.c_iteno, q1.c_batch} 



                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                        }
                        break;
                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RC_Type

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RC_Type:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);
                            string cusno = (parameters.ContainsKey("cusno") ? (string)((Functionals.ParameterParser)parameters["cusno"]).Value : string.Empty);
                            string batch = (parameters.ContainsKey("batch") ? (string)((Functionals.ParameterParser)parameters["batch"]).Value : string.Empty);
                            string dono = (parameters.ContainsKey("dono") ? (string)((Functionals.ParameterParser)parameters["dono"]).Value : string.Empty);
                            string rnno = (parameters.ContainsKey("rnno") ? (string)((Functionals.ParameterParser)parameters["rnno"]).Value : string.Empty);

                            var qry = (from q in db.LG_DOHs
                                       join q1 in db.LG_DOD2s on new { q.c_dono, q.c_via } equals new { q1.c_dono, q1.c_via }
                                       join q2 in
                                           (from q4 in db.LG_PLD2s
                                            group q4 by new { q4.c_plno, q4.c_iteno, q4.c_rnno, q4.c_batch } into qsum
                                            select new
                                            {
                                                c_plno = qsum.Key.c_plno,
                                                c_rnno = qsum.Key.c_rnno,
                                                c_iteno = qsum.Key.c_iteno,
                                                c_batch = qsum.Key.c_batch,
                                                n_qty = qsum.Sum(x => x.n_qty)
                                            }) on new { q.c_plno, q1.c_iteno } equals new { q2.c_plno, q2.c_iteno }
                                       where q.c_dono == dono && q.c_cusno == cusno && q1.c_iteno == item && q2.c_batch == batch
                                       && q1.n_sisa > 0 && q2.c_rnno == rnno
                                       select q2.n_qty).AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                        }
                        break;
                    #endregion

                    #endregion

                    #region RS

                    #region MODEL_COMMON_QUERY_MULTIPLE_RS_PEMBELIAN_ITEM

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RS_PEMBELIAN_ITEM:
                        {
                            string supl = (parameters.ContainsKey("supl") ? (string)((Functionals.ParameterParser)parameters["supl"]).Value : string.Empty);
                            char gudang = (parameters.ContainsKey("gudang") ? (char)((Functionals.ParameterParser)parameters["gudang"]).Value : '?');

                            //List<string> lstTipe = new List<string>();
                            //lstTipe.AddRange(new[] { "01", "04", "05" });

                            //var qry = (from q in db.FA_MasItms
                            //           join q1 in GlobalQuery.ViewStockLite(db) on q.c_iteno equals q1.c_iteno
                            //           join q2 in db.LG_MsBatches on new { q1.c_iteno, q1.c_batch } equals new { q2.c_iteno, q2.c_batch }
                            //           where lstTipe.Contains(q1.c_type) && q1.c_gdg == gudang
                            //           && q.c_nosup == supl
                            //           select q).Distinct().AsQueryable();

                            var qry = (from q in GlobalQuery.ViewStockLite(db, gudang)
                                       join q2 in db.FA_MasItms on q.c_iteno equals q2.c_iteno
                                       where q2.c_nosup == supl
                                       select new
                                       {
                                           c_nosup = q2.c_nosup,
                                           q.c_iteno,
                                           q2.v_itnam
                                       }).Distinct().AsQueryable();


                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RS_PEMBELIAN_BATCH

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RS_PEMBELIAN_BATCH:
                        {
                            char gudang = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);

                            //List<string> lstTipe = new List<string>();
                            //lstTipe.AddRange(new[] { "01", "04", "05" });

                            var qry = (from q in
                                           (from sq in GlobalQuery.ViewStockLiteContains(db, gudang, item)
                                            group sq by new { sq.c_iteno, sq.c_batch, sq.d_date, sq.c_gdg } into g
                                            select new { c_iteno = g.Key.c_iteno, c_batch = g.Key.c_batch, d_date = g.Key.d_date, n_gsisa = g.Sum(x => x.n_gsisa), n_bsisa = g.Sum(x => x.n_bsisa), c_gdg = g.Key.c_gdg })

                                       where (q.n_gsisa != 0 || q.n_bsisa != 0)
                                        && (q.c_iteno == item)
                                       select new
                                       {
                                           q.c_iteno,
                                           q.c_batch,
                                           d_expired = q.d_date,
                                           N_BSISA = q.n_bsisa,
                                           N_GSISA = q.n_gsisa
                                       }).AsQueryable();

                            //var qry = (from q in GlobalQuery.ViewStockLiteContains(db, gudang, item)
                            //           join q1 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q1.c_iteno, q1.c_batch }
                            //           where (q.n_bsisa > 0 || q.n_gsisa > 0)
                            //           select new
                            //           {
                            //             q.c_iteno,
                            //             q1.c_batch,
                            //             q1.d_expired,
                            //             N_BSISA = q.n_bsisa,
                            //             N_GSISA = q.n_gsisa
                            //           }).Distinct().AsQueryable();

                            //var qry = (from q in GlobalQuery.ViewStockBatch(db)
                            //           join q1 in db.LG_MsBatches on new  { q.c_iteno, q.c_batch } equals new {q1.c_iteno, q1.c_batch} 
                            //           where q.c_iteno == item
                            //           && q.c_gdg == gudang //&& ( q.N_GSISATOTAL > 0)
                            //           select new
                            //           {
                            //             q.c_iteno,
                            //             q.N_BSISA,
                            //             q.N_GSISA
                            //           }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;
                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RS_REPACK_ITEM

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RS_REPACK_ITEM:
                        {
                            string supl = (parameters.ContainsKey("supl") ? (string)((Functionals.ParameterParser)parameters["supl"]).Value : string.Empty);
                            char gudang = (parameters.ContainsKey("gudang") ? (char)((Functionals.ParameterParser)parameters["gudang"]).Value : '?');

                            //List<string> lstTipe = new List<string>();
                            //lstTipe.AddRange(new[] { "01", "04", "05" });

                            var qry = (from q in GlobalQuery.ViewStockLite(db, gudang)
                                       join q2 in db.FA_MasItms on q.c_iteno equals q2.c_iteno
                                       where q2.c_nosup == supl
                                       select new
                                       {
                                           c_nosup = q2.c_nosup,
                                           q.c_iteno,
                                           q2.v_itnam
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RS_REPACK_BATCH

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RS_REPACK_BATCH:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);

                            //List<string> lstTipe = new List<string>();
                            //lstTipe.AddRange(new[] { "01", "04", "05" });

                            var qry = (from q in
                                           (from sq in GlobalQuery.ViewStockLiteContains(db, gdg, item)
                                            group sq by new { sq.c_iteno, sq.c_batch, sq.d_date, sq.c_gdg } into g
                                            select new { c_iteno = g.Key.c_iteno, c_batch = g.Key.c_batch, d_date = g.Key.d_date, n_gsisa = g.Sum(x => x.n_gsisa), n_bsisa = g.Sum(x => x.n_bsisa), c_gdg = g.Key.c_gdg })

                                       where (q.n_gsisa != 0 || q.n_bsisa != 0)
                                        && (q.c_iteno == item)
                                       select new
                                       {
                                           q.c_iteno,
                                           q.c_batch,
                                           d_expired = q.d_date,
                                           N_BSISA = q.n_bsisa,
                                           N_GSISA = q.n_gsisa
                                       }).AsQueryable();

                            //var qry = (from q in GlobalQuery.ViewStockBatch(db)
                            //           join q1 in db.LG_MsBatches on new  { q.c_iteno, q.c_batch } equals new {q1.c_iteno, q1.c_batch} 
                            //           where q.c_iteno == item
                            //           && q.c_gdg == gudang //&& ( q.N_GSISATOTAL > 0)
                            //           select new
                            //           {
                            //             q.c_iteno,
                            //             q.N_BSISA,
                            //             q.N_GSISA
                            //           }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;
                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RS_CONF_PEMBELIAN_NORS

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RS_CONF_PEMBELIAN_NORS:
                        {
                            char gudang = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            //string NoRs = (parameters.ContainsKey("NoRs") ? (string)((Functionals.ParameterParser)parameters["NoRs"]).Value : string.Empty);
                            string noSupl = (parameters.ContainsKey("supl") ? (string)((Functionals.ParameterParser)parameters["supl"]).Value : string.Empty);

                            //var qry = (from q in db.LG_RSD2s
                            //           join q1 in db.LG_RSHes on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                            //           where (q.n_gsisa > 0 || q.n_bsisa > 0)
                            //           && q.c_gdg == gudang && q.c_rsno != NoRs && q1.c_nosup == supl
                            //           select new
                            //           {
                            //             c_noref = q.c_rsno,
                            //             q1.d_rsdate
                            //           }).Distinct().AsQueryable();

                            var qry = (from q in db.LG_RSHes
                                       join q1 in db.LG_RSD2s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                                       where (q.c_gdg == gudang) && (q.c_nosup == noSupl) && (q.c_type == "01")
                                        && ((q1.n_gsisa > 0) || (q1.n_bsisa > 0))
                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       select new
                                       {
                                           c_noref = q.c_rsno,
                                           d_rsdate = (q.d_rsdate.HasValue ? q.d_rsdate.Value : Functionals.StandardSqlDateTime)
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RS_CONF_PEMBELIAN_NORS_HDR

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RS_CONF_PEMBELIAN_NORS_HDR:
                        {
                            char gudang = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string rsno = (parameters.ContainsKey("rsno") ? (string)((Functionals.ParameterParser)parameters["rsno"]).Value : string.Empty);
                            string noSupl = (parameters.ContainsKey("supl") ? (string)((Functionals.ParameterParser)parameters["supl"]).Value : string.Empty);

                            var qry = (from q0 in db.LG_RSHes
                                       join q in db.LG_RSD2s on new { q0.c_gdg, q0.c_rsno } equals new { q.c_gdg, q.c_rsno }
                                       join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                       where (q0.c_nosup == noSupl) && (q.n_gsisa > 0 || q.n_bsisa > 0)
                                        && (q0.c_gdg == gudang) && (q0.c_rsno == rsno)
                                        && ((q.n_bsisa > 0) || (q.n_gsisa > 0))
                                        && ((q.l_confirm.HasValue ? q.l_confirm.Value : false) == false)
                                        && ((q0.l_delete.HasValue ? q0.l_delete.Value : false) == false)
                                       group q by new { q.c_gdg, q.c_rsno, q.c_iteno, q1.v_itnam, q.c_batch } into g
                                       select new
                                       {
                                           g.Key.c_gdg,
                                           g.Key.c_rsno,
                                           g.Key.c_iteno,
                                           g.Key.v_itnam,
                                           g.Key.c_batch,
                                           good = g.Sum(x => x.n_gsisa),
                                           bad = g.Sum(x => x.n_bsisa),
                                           nQty = g.Sum(x => x.n_gsisa) + g.Sum(x => x.n_bsisa),
                                           nReject = 0,
                                           nRework = 0,
                                           nRedress = 0,
                                           l_alternate = false,
                                           l_new = true
                                       }).AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RS_CONF_PEMBELIAN_ITEM_RS

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RS_CONF_PEMBELIAN_ITEM_RS:
                        {
                            char gudang = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string rsno = (parameters.ContainsKey("rsno") ? (string)((Functionals.ParameterParser)parameters["rsno"]).Value : string.Empty);

                            var qry = (from q in db.LG_RSD2s
                                       join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                       where (q.n_gsisa > 0 || q.n_bsisa > 0)
                                        && (q.c_gdg == gudang) && (q.c_rsno == rsno)
                                        && ((q.l_confirm.HasValue ? q.l_confirm.Value : false) == false)
                                       group q by new { q.c_gdg, q.c_rsno, q.c_iteno, q1.v_itnam, q.c_batch } into g
                                       select new
                                       {
                                           c_gdg = g.Key.c_gdg,
                                           c_rsno = g.Key.c_rsno,
                                           c_iteno = g.Key.c_iteno,
                                           v_itnam = g.Key.v_itnam,
                                           c_batch = g.Key.c_batch,
                                           good = g.Sum(x => x.n_gsisa),
                                           bad = g.Sum(x => x.n_bsisa)
                                       }).AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_RS_CONF_PEMBELIAN_BATCH_RS

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RS_CONF_PEMBELIAN_BATCH_RS:
                        {
                            char gudang = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string rsno = (parameters.ContainsKey("rsno") ? (string)((Functionals.ParameterParser)parameters["rsno"]).Value : string.Empty);
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);

                            var qry = (from q in db.LG_RSD2s
                                       join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                       where (q.n_gsisa > 0 || q.n_bsisa > 0)
                                       && q.c_gdg == gudang && q.c_rsno == rsno && q.c_iteno == item
                                       group q by new { q.c_gdg, q.c_rsno, q.c_iteno, q1.v_itnam, q.c_batch } into g
                                       select new
                                       {
                                           c_gdg = g.Key.c_gdg,
                                           c_rsno = g.Key.c_rsno,
                                           c_iteno = g.Key.c_iteno,
                                           v_itnam = g.Key.v_itnam,
                                           c_batch = g.Key.c_batch,
                                           good = g.Sum(x => x.n_gsisa),
                                           bad = g.Sum(x => x.n_bsisa)
                                       }).AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                        }
                        break;

                    #endregion


                    #endregion

                    #region WP

                    #region MODEL_COMMON_QUERY_MULTIPLE_WP_TRANSAKSI
                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_WP_TRANSAKSI:
                        {
                            string sTipe = (parameters.ContainsKey("c_type") ? (string)((Functionals.ParameterParser)parameters["c_type"]).Value : string.Empty);
                            char gdg = (parameters.ContainsKey("c_gdg") ? (char)((Functionals.ParameterParser)parameters["c_gdg"]).Value : '?');
                            string sModeDay = (parameters.ContainsKey("c_modeDay") ? (string)((Functionals.ParameterParser)parameters["c_modeDay"]).Value : string.Empty);
                            DateTime date = DateTime.Today;

                            List<string> sDataWP = new List<string>();

                            if (sTipe.Equals("01"))
                            {
                                #region 01

                                switch (sModeDay)
                                {
                                    case "01":
                                        {
                                            #region 01

                                            var qry = (from q in db.LG_PLHs
                                                       where SqlMethods.DateDiffMonth(q.d_pldate, date) > 1
                                                       && (q.l_wpppic == false || q.l_wpppic == null)
                                                       select new
                                                       {
                                                           c_dono = q.c_plno,
                                                           d_dodate = q.d_pldate,
                                                       }).AsQueryable();

                                            if ((parameters != null) && (parameters.Count > 0))
                                            {
                                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                                {
                                                    if (kvp.Value.IsCondition)
                                                    {
                                                        if (kvp.Value.IsLike)
                                                        {
                                                            paternLike = kvp.Value.Value.ToString();
                                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                                        }
                                                        else
                                                        {
                                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                                        }
                                                    }
                                                }
                                            }

                                            Logger.WriteLine(qry.Provider.ToString());

                                            nCount = qry.Count();

                                            if (nCount > 0)
                                            {
                                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                                {
                                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                                }

                                                if ((limit == -1) || allQuery)
                                                {
                                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                                }
                                                else
                                                {
                                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                                }
                                            }

                                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                                            #endregion
                                        }
                                        break;

                                    case "02":
                                        {
                                            #region 02

                                            var qry = (from q in db.LG_PLHs
                                                       where SqlMethods.DateDiffMonth(q.d_pldate, date) > 1
                                                       && (q.l_wpppic == false || q.l_wpppic == null)
                                                       select new
                                                       {
                                                           c_dono = q.c_plno,
                                                           d_dodate = q.d_pldate,
                                                       }).AsQueryable();

                                            if ((parameters != null) && (parameters.Count > 0))
                                            {
                                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                                {
                                                    if (kvp.Value.IsCondition)
                                                    {
                                                        if (kvp.Value.IsLike)
                                                        {
                                                            paternLike = kvp.Value.Value.ToString();
                                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                                        }
                                                        else
                                                        {
                                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                                        }
                                                    }
                                                }
                                            }

                                            Logger.WriteLine(qry.Provider.ToString());

                                            nCount = qry.Count();

                                            if (nCount > 0)
                                            {
                                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                                {
                                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                                }

                                                if ((limit == -1) || allQuery)
                                                {
                                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                                }
                                                else
                                                {
                                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                                }
                                            }

                                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                                            #endregion
                                        }
                                        break;

                                    default:
                                        {
                                            #region default

                                            var qry = (from q in db.LG_PLHs
                                                       where SqlMethods.DateDiffMonth(q.d_pldate, date) > 0
                                                       && (q.l_wpppic == false || q.l_wpppic == null)
                                                       select new
                                                       {
                                                           c_dono = q.c_plno,
                                                           d_dodate = q.d_pldate,
                                                       }).AsQueryable();

                                            if ((parameters != null) && (parameters.Count > 0))
                                            {
                                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                                {
                                                    if (kvp.Value.IsCondition)
                                                    {
                                                        if (kvp.Value.IsLike)
                                                        {
                                                            paternLike = kvp.Value.Value.ToString();
                                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                                        }
                                                        else
                                                        {
                                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                                        }
                                                    }
                                                }
                                            }

                                            Logger.WriteLine(qry.Provider.ToString());

                                            nCount = qry.Count();

                                            if (nCount > 0)
                                            {
                                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                                {
                                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                                }

                                                if ((limit == -1) || allQuery)
                                                {
                                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                                }
                                                else
                                                {
                                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                                }
                                            }

                                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                                            #endregion
                                        }
                                        break;
                                }
                                #endregion
                            }
                            else if (sTipe.Equals("02"))
                            {
                                #region 02

                                switch (sModeDay)
                                {
                                    case "01":
                                        {
                                            #region 01

                                            var qry = (from q in db.LG_DOHs
                                                       where SqlMethods.DateDiffMonth(q.d_dodate, date) > 1
                                                       && (q.l_wpdc == false || q.l_wpdc == null)
                                                       select new
                                                       {
                                                           c_dono = q.c_dono,
                                                           d_dodate = q.d_dodate,
                                                       }).AsQueryable();

                                            if ((parameters != null) && (parameters.Count > 0))
                                            {
                                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                                {
                                                    if (kvp.Value.IsCondition)
                                                    {
                                                        if (kvp.Value.IsLike)
                                                        {
                                                            paternLike = kvp.Value.Value.ToString();
                                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                                        }
                                                        else
                                                        {
                                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                                        }
                                                    }
                                                }
                                            }

                                            Logger.WriteLine(qry.Provider.ToString());

                                            nCount = qry.Count();

                                            if (nCount > 0)
                                            {
                                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                                {
                                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                                }

                                                if ((limit == -1) || allQuery)
                                                {
                                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                                }
                                                else
                                                {
                                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                                }
                                            }

                                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                                            #endregion
                                        }
                                        break;

                                    case "02":
                                        {
                                            #region 02

                                            var qry = (from q in db.LG_DOHs
                                                       where SqlMethods.DateDiffMonth(q.d_dodate, date) > 1
                                                       && (q.l_wpdc == false || q.l_wpdc == null)
                                                       select new
                                                       {
                                                           c_dono = q.c_dono,
                                                           d_dodate = q.d_dodate,
                                                       }).AsQueryable();

                                            if ((parameters != null) && (parameters.Count > 0))
                                            {
                                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                                {
                                                    if (kvp.Value.IsCondition)
                                                    {
                                                        if (kvp.Value.IsLike)
                                                        {
                                                            paternLike = kvp.Value.Value.ToString();
                                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                                        }
                                                        else
                                                        {
                                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                                        }
                                                    }
                                                }
                                            }

                                            Logger.WriteLine(qry.Provider.ToString());

                                            nCount = qry.Count();

                                            if (nCount > 0)
                                            {
                                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                                {
                                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                                }

                                                if ((limit == -1) || allQuery)
                                                {
                                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                                }
                                                else
                                                {
                                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                                }
                                            }

                                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                                            #endregion
                                        }
                                        break;

                                    default:
                                        {
                                            #region default

                                            var qry = (from q in db.LG_DOHs
                                                       where SqlMethods.DateDiffMonth(q.d_dodate, date) > 0
                                                       && (q.l_wpdc == false || q.l_wpdc == null)
                                                       select new
                                                       {
                                                           c_dono = q.c_dono,
                                                           d_dodate = q.d_dodate,
                                                       }).AsQueryable();

                                            if ((parameters != null) && (parameters.Count > 0))
                                            {
                                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                                {
                                                    if (kvp.Value.IsCondition)
                                                    {
                                                        if (kvp.Value.IsLike)
                                                        {
                                                            paternLike = kvp.Value.Value.ToString();
                                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                                        }
                                                        else
                                                        {
                                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                                        }
                                                    }
                                                }
                                            }

                                            Logger.WriteLine(qry.Provider.ToString());

                                            nCount = qry.Count();

                                            if (nCount > 0)
                                            {
                                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                                {
                                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                                }

                                                if ((limit == -1) || allQuery)
                                                {
                                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                                }
                                                else
                                                {
                                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                                }
                                            }

                                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                                            #endregion
                                        }
                                        break;
                                }
                                #endregion
                            }
                        }
                        break;
                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_WP_PICKER

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_WP_PICKER:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string type = (parameters.ContainsKey("tipe") ? (string)((Functionals.ParameterParser)parameters["tipe"]).Value : string.Empty);

                            var qryPL = (from q in db.LG_PLHs
                                         join q1 in db.LG_DOHs on q.c_plno equals q1.c_plno into qDO
                                         from qAll in qDO.DefaultIfEmpty()
                                         join q2 in db.LG_PLD2s on q.c_plno equals q2.c_plno
                                         where q.l_print == true && q.c_gdg == gdg
                                         && (q.l_delete == false || q.l_delete == null)
                                         && qAll.c_plno == null
                                         && SqlMethods.DateDiffMonth(q.d_pldate.Value, DateTime.Now) < 1
                                         select new STDetail
                                         {
                                             noTrans = q.c_plno,
                                             dTrans = q.d_pldate,
                                             c_type_lat = q.c_type_lat
                                         }).Distinct().AsQueryable();

                            //var x = qryPL.ToList();

                            var qrySJ = (from q in db.LG_SJHs
                                         join q1 in db.LG_SJD2s on q.c_sjno equals q1.c_sjno
                                         where q.l_print == true && q.l_confirm == false && q.c_gdg == gdg
                                         && (q.l_delete == false || q.l_delete == null)
                                         && SqlMethods.DateDiffMonth(q.d_sjdate.Value, DateTime.Now) < 1
                                         select new STDetail
                                         {
                                             noTrans = q.c_sjno,
                                             dTrans = q.d_sjdate,
                                             c_type_lat = q.c_type_lat
                                         }).Distinct().AsQueryable();

                            //var y = qrySJ.ToList();

                            var qryPLSJ = qrySJ.Union(qryPL).Distinct().AsQueryable();

                            //var s = qryPLSJ.ToList();

                            var qry = qryPLSJ;
                            if (type.ToString() == "0306")
                            {
                                var qryExcept = (from q in db.SCMS_STDs
                                                 join q1 in qryPLSJ on q.c_no equals q1.noTrans
                                                 join q2 in db.SCMS_STHs on q.c_nodoc equals q2.c_nodoc
                                                 where q2.c_type == "01" && q2.c_gdg == gdg
                                                 && (q2.l_delete == false || q2.l_delete == null)
                                                 select new STDetail
                                                 {
                                                     noTrans = q1.noTrans,
                                                     dTrans = q1.dTrans,
                                                     c_type_lat = q1.c_type_lat
                                                 }).Distinct().AsQueryable();

                                //var t = qryExcept.ToList();
                                qry = qryPLSJ.Except(qryExcept).Distinct().AsQueryable();
                            }
                            //var z = qry.ToList();
                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsIn)
                                        {
                                            #region In Clause

                                            qry = qry.In(kvp.Key, (object[])kvp.Value.Value).AsQueryable();

                                            #endregion
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_WP_CHECKER

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_WP_CHECKER:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string type = (parameters.ContainsKey("tipe") ? (string)((Functionals.ParameterParser)parameters["tipe"]).Value : string.Empty);

                            var qryPicker = (from q in db.SCMS_STHs
                                             join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                             where q.c_gdg == gdg
                                             && (q.l_delete == false || q.l_delete == null)
                                             && q.c_type == "01"
                                             && SqlMethods.DateDiffMonth(q.d_date.Value, DateTime.Now) < 1
                                             select new STDetail
                                             {
                                                 noTrans = q1.c_no,
                                                 dTrans = q.d_entry,
                                                 c_type_lat = q1.c_type_lat
                                             }).Distinct().AsQueryable();

                            //var x = qryPicker.ToList();

                            var qryChecker = (from q in db.SCMS_STHs
                                              join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                              where q.c_gdg == gdg
                                              && (q.l_delete == false || q.l_delete == null)
                                              && q.c_type == "02"
                                              && SqlMethods.DateDiffMonth(q.d_date.Value, DateTime.Now) < 1
                                              select new STDetail
                                              {
                                                  noTrans = q1.c_no,
                                                  dTrans = q.d_entry,
                                                  c_type_lat = q1.c_type_lat
                                              }).Distinct().AsQueryable();

                            //var y = qryChecker.ToList();

                            var qry = qryPicker.Except(qryChecker).Distinct().AsQueryable();

                            //var s = qryAll.ToList();

                            //var qry = (from q in qryAll
                            //           join q1 in db.SCMS_STDs on q.c_no equals q1.c_no
                            //           join q2 in db.SCMS_STHs on q1.c_nodoc equals q2.c_nodoc
                            //           select new STDetail
                            //           {
                            //               noTrans = q.c_no,
                            //               dTrans = q2.d_date,
                            //               c_type_lat = q.c_type_lat
                            //           }).AsQueryable();

                            if (type.ToString() != "0306a")
                            {
                                qry = (from q in db.SCMS_STHs
                                       join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                       where q.c_gdg == gdg
                                       && (q.l_delete == false || q.l_delete == null)
                                       && q.c_type == "01"
                                       && SqlMethods.DateDiffMonth(q.d_date.Value, DateTime.Now) < 1
                                       group new { q, q1 } by new { q.d_date, q1.c_no, q1.c_type_lat } into gGroup
                                       select new STDetail
                                       {
                                           noTrans = gGroup.Key.c_no,
                                           dTrans = gGroup.Max(a => a.q.d_date),
                                           c_type_lat = gGroup.Key.c_type_lat
                                       }).Distinct().AsQueryable();
                            }

                            //var z = qry.ToList();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsIn)
                                        {
                                            #region In Clause

                                            qry = qry.In(kvp.Key, (object[])kvp.Value.Value).AsQueryable();

                                            #endregion
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_WP_PACKER

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_WP_PACKER:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string type = (parameters.ContainsKey("tipe") ? (string)((Functionals.ParameterParser)parameters["tipe"]).Value : string.Empty);

                            var qryChecker = (from q in db.SCMS_STHs
                                              join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                              where q.c_gdg == gdg
                                              && (q.l_delete == false || q.l_delete == null)
                                              && q.c_type == "02"
                                              && SqlMethods.DateDiffMonth(q.d_date.Value, DateTime.Now) < 1
                                              select new STDetail
                                              {
                                                  noTrans = q1.c_no,
                                                  dTrans = q.d_entry,
                                                  c_type_lat = q1.c_type_lat
                                              }).Distinct().AsQueryable();

                            //var x = qryPicker.ToList();

                            var qryPacker = (from q in db.SCMS_STHs
                                             join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                             where q.c_gdg == gdg
                                             && (q.l_delete == false || q.l_delete == null)
                                             && q.c_type == "03"
                                             && SqlMethods.DateDiffMonth(q.d_date.Value, DateTime.Now) < 1
                                             select new STDetail
                                             {
                                                 noTrans = q1.c_no,
                                                 dTrans = q.d_entry,
                                                 c_type_lat = q1.c_type_lat
                                             }).Distinct().AsQueryable();

                            //var y = qryChecker.ToList();

                            var qry = qryChecker.Except(qryPacker).Distinct().AsQueryable();

                            //var z = qry.ToList();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsIn)
                                        {
                                            #region In Clause

                                            qry = qry.In(kvp.Key, (object[])kvp.Value.Value).AsQueryable();

                                            #endregion
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_WP_TRANSPORTASI

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_WP_TRANSPORTASI:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string type = (parameters.ContainsKey("tipe") ? (string)((Functionals.ParameterParser)parameters["tipe"]).Value : string.Empty);

                            var qryP1 = (from q in db.SCMS_STHs
                                         join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                         join q2 in db.LG_DOHs on q1.c_no equals q2.c_plno
                                         where q.c_gdg == gdg
                                         && (q.l_delete == false || q.l_delete == null)
                                         && q.c_type == "03"
                                         && SqlMethods.DateDiffMonth(q.d_date.Value, DateTime.Now) < 1
                                         select new STDetail
                                         {
                                             noTrans = q2.c_dono,
                                             dTrans = q.d_entry,
                                             c_type_lat = q1.c_type_lat
                                         }).Distinct().AsQueryable();

                            //var a = qryP1.ToList();

                            var qryP2 = (from q in db.SCMS_STHs
                                         join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                         where q.c_gdg == gdg
                                         && (q.l_delete == false || q.l_delete == null)
                                         && q.c_type == "03"
                                         && SqlMethods.DateDiffMonth(q.d_date.Value, DateTime.Now) < 1
                                         && q1.c_no.Substring(0, 2).ToUpper() == "SJ"
                                         select new STDetail
                                         {
                                             noTrans = q1.c_no,
                                             dTrans = q.d_entry,
                                             c_type_lat = q1.c_type_lat
                                         }).Distinct().AsQueryable();

                            //var b = qryP2.ToList();

                            var qryPacker = qryP1.Union(qryP2).Distinct().AsQueryable();
                            //var x = qryPacker.ToList();

                            var qryTransport = (from q in db.SCMS_STHs
                                                join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                                where q.c_gdg == gdg
                                                && (q.l_delete == false || q.l_delete == null)
                                                && q.c_type == "04"
                                                && SqlMethods.DateDiffMonth(q.d_date.Value, DateTime.Now) < 1

                                                select new STDetail
                                                {
                                                    noTrans = q1.c_no,
                                                    dTrans = q.d_entry,
                                                    c_type_lat = q1.c_type_lat
                                                }).Distinct().AsQueryable();

                            //var y = qryTransport.ToList();

                            var qry = qryPacker.Except(qryTransport).Distinct().AsQueryable();

                            //var z = qry.ToList();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsIn)
                                        {
                                            #region In Clause

                                            qry = qry.In(kvp.Key, (object[])kvp.Value.Value).AsQueryable();

                                            #endregion
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_WP_INKJET

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_WP_INKJET:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string type = (parameters.ContainsKey("tipe") ? (string)((Functionals.ParameterParser)parameters["tipe"]).Value : string.Empty);

                            List<string> lstTypeRNFB = new List<string>();
                            lstTypeRNFB.Add("04");
                            lstTypeRNFB.Add("05");

                            var qryPicker = (from q in db.SCMS_STHs
                                             join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                             join q2 in db.LG_PLD1s on q1.c_no equals q2.c_plno
                                             join q3 in db.SCMS_MSITEM_CATs on q2.c_iteno equals q3.c_iteno
                                             where q.c_gdg == gdg
                                             && (q.l_delete == false || q.l_delete == null)
                                             && q.c_type == "01"
                                             && SqlMethods.DateDiffMonth(q.d_date.Value, DateTime.Now) < 1
                                             && lstTypeRNFB.Contains(q3.c_type)
                                             select new STDetail
                                             {
                                                 noTrans = q1.c_no,
                                                 dTrans = q.d_entry,
                                                 c_type_lat = q1.c_type_lat
                                             }).Distinct().AsQueryable();

                            //var x = qryPicker.ToList();

                            var qryInkjet = (from q in db.SCMS_STHs
                                             join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                             where q.c_gdg == gdg
                                             && (q.l_delete == false || q.l_delete == null)
                                             && q.c_type == "1A"
                                             && SqlMethods.DateDiffMonth(q.d_date.Value, DateTime.Now) < 1
                                             select new STDetail
                                             {
                                                 noTrans = q1.c_no,
                                                 dTrans = q.d_entry,
                                                 c_type_lat = q1.c_type_lat
                                             }).Distinct().AsQueryable();

                            //var y = qryChecker.ToList();

                            var qry = qryPicker.Except(qryInkjet).Distinct().AsQueryable();

                            //var s = qryAll.ToList();

                            //var qry = (from q in qryAll
                            //           join q1 in db.SCMS_STDs on q.c_no equals q1.c_no
                            //           join q2 in db.SCMS_STHs on q1.c_nodoc equals q2.c_nodoc
                            //           select new STDetail
                            //           {
                            //               noTrans = q.c_no,
                            //               dTrans = q2.d_date,
                            //               c_type_lat = q.c_type_lat
                            //           }).AsQueryable();

                            if (type.ToString() != "0306x")
                            {
                                qry = (from q in db.SCMS_STHs
                                       join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                       where q.c_gdg == gdg
                                       && (q.l_delete == false || q.l_delete == null)
                                       && q.c_type == "01"
                                       && SqlMethods.DateDiffMonth(q.d_date.Value, DateTime.Now) < 1
                                       group new { q, q1 } by new { q.d_date, q1.c_no, q1.c_type_lat } into gGroup
                                       select new STDetail
                                       {
                                           noTrans = gGroup.Key.c_no,
                                           dTrans = gGroup.Max(a => a.q.d_date),
                                           c_type_lat = gGroup.Key.c_type_lat
                                       }).Distinct().AsQueryable();
                            }

                            //var z = qry.ToList();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsIn)
                                        {
                                            #region In Clause

                                            qry = qry.In(kvp.Key, (object[])kvp.Value.Value).AsQueryable();

                                            #endregion
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_WP_CHECKER_CekInkJet

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_WP_CHECKER_CekInkJet:
                        {
                            string plno = (parameters.ContainsKey("c_no") ? (string)((Functionals.ParameterParser)parameters["c_no"]).Value : string.Empty);
                            string inkjet = (parameters.ContainsKey("c_inkjet") ? (string)((Functionals.ParameterParser)parameters["c_inkjet"]).Value : string.Empty);

                            List<string> lstTypeRNFB = new List<string>();
                            lstTypeRNFB.Add("04");
                            lstTypeRNFB.Add("05");

                            var qry = (from q in db.LG_PLD1s
                                       join q1 in db.SCMS_MSITEM_CATs on q.c_iteno equals q1.c_iteno
                                       where q.c_plno == plno
                                       && lstTypeRNFB.Contains(q1.c_type)
                                       select new STDetail
                                       {
                                           noTrans = q.c_plno
                                       }).Distinct().AsQueryable();

                            if (!string.IsNullOrEmpty(inkjet))
                            {
                                qry = (from q in db.SCMS_STHs
                                       join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                                       where q1.c_no == plno
                                       && q.c_type == "1A"
                                       select new STDetail
                                       {
                                           noTrans = q1.c_no
                                       }).Distinct().AsQueryable();

                                //var listIn = qry.ToList();
                            }

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsIn)
                                        {
                                            #region In Clause

                                            qry = qry.In(kvp.Key, (object[])kvp.Value.Value).AsQueryable();

                                            #endregion
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_WP_PO

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_WP_PO:
                    {

                        string noDoc = (parameters.ContainsKey("c_nodoc") ? (string)((Functionals.ParameterParser)parameters["c_nodoc"]).Value : string.Empty);
                        //Indra 20180920FM
                        //SerahTerimaTransportasi
                        bool tipeDoc = (parameters.ContainsKey("TipeDoc") ? (bool)((Functionals.ParameterParser)parameters["TipeDoc"]).Value : false);

                        var noSup = (from q in db.SCMS_WPHs
                                     where q.c_nodoc == noDoc
                                     select q.c_nosup).Take(1).SingleOrDefault();

                        if (tipeDoc)
                        {
                            //Indra 20181011FM
                            //PO Never die
                            int iDate = 7;
                            //switch (noSup)
                            //{
                            //    case "00001":
                            //    case "00112":
                            //    case "00113":
                            //    case "00117":
                            //    case "00120":
                            //    case "00019":
                            //    case "00085":
                            //    //Indra 20181011FM
                            //    //PO Import Never die
                            //    case "00121":
                            //    case "00062":
                            //    case "00098":
                            //    case "00170":
                            //    case "00047":
                            //    case "00076":
                            //    case "00179":
                            //    case "00054":
                            //        iDate = 9999;
                            //        break;
                            //}
                            
                            var STT = (from q in db.LG_MsSTT_Principals
                                       where q.c_nosup == noSup && q.l_status == true
                                       select q).Take(1).SingleOrDefault();
                            if (STT == null)
                            {
                                iDate = 7;
                            }
                            else
                            {
                                iDate = 9999;
                            }

                            var qry = (from q in db.LG_POHs
                                       join q1 in db.LG_POD1s on q.c_pono equals q1.c_pono
                                       where q.c_nosup == noSup
                                       && (SqlMethods.DateDiffMonth(q.d_podate, DateTime.Now.Date) < iDate)
                                       && q1.n_sisa > 0
                                       select new
                                       {
                                           q.c_pono,
                                           q.d_podate,
                                           l_new = true
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsIn)
                                        {
                                            #region In Clause

                                            qry = qry.In(kvp.Key, (object[])kvp.Value.Value).AsQueryable();

                                            #endregion
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }
                        }
                        else
                        {
                            var qry = (from q in db.LG_SJHs
                                        join q1 in db.LG_SJD1s on q.c_sjno equals q1.c_sjno
                                        where q.l_status == false && q.c_nosup == noSup && q.d_sjdate > DateTime.Now.AddDays(-60) ||
                                            ((q.l_status == true) && (SqlMethods.DateDiffDay(q.d_update, DateTime.Now.Date) < 1))
                                        select new
                                        {
                                            c_pono = q.c_sjno,
                                            d_podate = q.d_sjdate,
                                            l_new = true
                                        }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsIn)
                                        {
                                            #region In Clause

                                            qry = qry.In(kvp.Key, (object[])kvp.Value.Value).AsQueryable();

                                            #endregion
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }
                        }
                        dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                        dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                    }
                    break;

                    #endregion

                    #region old
                    #region MODEL_COMMON_QUERY_MULTIPLE_WP_PACKER

                    //case Constant.MODEL_COMMON_QUERY_MULTIPLE_WP_PACKER:
                    //    {
                    //        char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                    //        string type = (parameters.ContainsKey("tipe") ? (string)((Functionals.ParameterParser)parameters["tipe"]).Value : string.Empty);

                    //        var qryDO = (from q in db.LG_DOHs
                    //                     where q.c_gdg == gdg
                    //                     && (q.l_delete == false || q.l_delete == null)
                    //                     && SqlMethods.DateDiffMonth(q.d_dodate.Value, DateTime.Now) < 2
                    //                     select new
                    //                     {
                    //                         q.c_dono,
                    //                     }).Distinct().AsQueryable();

                    //        //var x = qryDO.ToList();

                    //        var qryEP = (from q in db.LG_ExpHs
                    //                     join q1 in db.LG_ExpDs on q.c_expno equals q1.c_expno
                    //                     where q.c_gdg == gdg
                    //                     && (q.l_delete == false || q.l_delete == null)
                    //                     && SqlMethods.DateDiffMonth(q.d_expdate.Value, DateTime.Now) < 2
                    //                     select new
                    //                     {
                    //                         q1.c_dono,
                    //                     }).Distinct().AsQueryable();

                    //        //var x1 = qryEP.ToList();

                    //        var qryDoEp = qryDO.Except(qryEP).Distinct().AsQueryable();

                    //        //var x2 = qryDoEp.ToList();

                    //        var qryListDO = (from q in qryDoEp
                    //                         join q1 in db.LG_DOHs on q.c_dono equals q1.c_dono
                    //                         join q2 in db.LG_PLHs on q1.c_plno equals q2.c_plno
                    //                         select new STDetail
                    //                         {
                    //                             noTrans = q1.c_dono,
                    //                             dTrans = q1.d_dodate,
                    //                             c_type_lat = q2.c_type_lat
                    //                         }).Distinct().AsQueryable();

                    //        var qryListSJ = (from q in db.LG_SJHs
                    //                         join q1 in db.LG_SJD2s on q.c_sjno equals q1.c_sjno
                    //                         where q.l_print == true && q.l_confirm == true && q.c_gdg == gdg
                    //                         && (q.l_delete == false || q.l_delete == null)
                    //                         && SqlMethods.DateDiffMonth(q.d_sjdate.Value, DateTime.Now) < 2
                    //                         select new STDetail
                    //                         {
                    //                             noTrans = q.c_sjno,
                    //                             dTrans = q.d_sjdate,
                    //                             c_type_lat = q.c_type_lat
                    //                         }).Distinct().AsQueryable();

                    //        //var y = qryListSJ.ToList();

                    //        var qryAll = qryListDO.Union(qryListSJ).Distinct().AsQueryable();

                    //        //var s = qryAll.ToList();

                    //        var qry = qryAll;

                    //        if (type.ToString() == "0306b")
                    //        {
                    //            var qryExcept = (from q in db.SCMS_STDs
                    //                             join q1 in qryAll on q.c_no equals q1.noTrans
                    //                             join q2 in db.SCMS_STHs on q.c_nodoc equals q2.c_nodoc
                    //                             where q2.c_type == "03" && q2.c_gdg == gdg
                    //                             && (q2.l_delete == false || q2.l_delete == null)
                    //                             select new STDetail
                    //                             {
                    //                                 noTrans = q1.noTrans,
                    //                                 dTrans = q1.dTrans,
                    //                                 c_type_lat = q1.c_type_lat
                    //                             }).Distinct().AsQueryable();

                    //            //var s1 = qryExcept.ToList();

                    //            qry = qryAll.Except(qryExcept).Distinct().AsQueryable();
                    //        }

                    //        //var z = qry.ToList();

                    //        if ((parameters != null) && (parameters.Count > 0))
                    //        {
                    //            foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                    //            {
                    //                if (kvp.Value.IsCondition)
                    //                {
                    //                    if (kvp.Value.IsLike)
                    //                    {
                    //                        paternLike = kvp.Value.Value.ToString();
                    //                        qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                    //                    }
                    //                    else if (kvp.Value.IsIn)
                    //                    {
                    //                        #region In Clause

                    //                        qry = qry.In(kvp.Key, (object[])kvp.Value.Value).AsQueryable();

                    //                        #endregion
                    //                    }
                    //                    else
                    //                    {
                    //                        qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                    //                    }
                    //                }
                    //            }
                    //        }

                    //        Logger.WriteLine(qry.Provider.ToString());

                    //        nCount = qry.Count();

                    //        if (nCount > 0)
                    //        {
                    //            if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                    //            {
                    //                qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                    //            }

                    //            if ((limit == -1) || allQuery)
                    //            {
                    //                dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                    //            }
                    //            else
                    //            {
                    //                dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                    //            }
                    //        }

                    //        dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                    //        dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                    //    }
                    //    break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_WP_TRANSPORTASI

                    //case Constant.MODEL_COMMON_QUERY_MULTIPLE_WP_TRANSPORTASI:
                    //    {
                    //        char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                    //        string type = (parameters.ContainsKey("tipe") ? (string)((Functionals.ParameterParser)parameters["tipe"]).Value : string.Empty);

                    //        var qryPicker = (from q in db.SCMS_STHs
                    //                         join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                    //                         where q.c_gdg == gdg
                    //                         && (q.l_delete == false || q.l_delete == null)
                    //                         && q.c_type == "03"
                    //                         && SqlMethods.DateDiffMonth(q.d_date.Value, DateTime.Now) < 2
                    //                         select new
                    //                         {
                    //                             q1.c_no,
                    //                             q1.c_type_lat
                    //                         }).Distinct().AsQueryable();

                    //        //var x = qryPicker.ToList();

                    //        var qryChecker = (from q in db.SCMS_STHs
                    //                          join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                    //                          where q.c_gdg == gdg
                    //                          && (q.l_delete == false || q.l_delete == null)
                    //                          && q.c_type == "04"
                    //                          && SqlMethods.DateDiffMonth(q.d_date.Value, DateTime.Now) < 2
                    //                          select new
                    //                          {
                    //                              q1.c_no,
                    //                              q1.c_type_lat
                    //                          }).Distinct().AsQueryable();

                    //        //var y = qryChecker.ToList();

                    //        var qryAll = qryPicker.Except(qryChecker).Distinct().AsQueryable();

                    //        //var s = qryAll.ToList();

                    //        var qry = (from q in qryAll
                    //                   join q1 in db.SCMS_STDs on q.c_no equals q1.c_no
                    //                   join q2 in db.SCMS_STHs on q1.c_nodoc equals q2.c_nodoc
                    //                   select new STDetail
                    //                   {
                    //                       noTrans = q.c_no,
                    //                       dTrans = q2.d_date,
                    //                       c_type_lat = q.c_type_lat
                    //                   }).AsQueryable();

                    //        if (type.ToString() != "0306c")
                    //        {
                    //            qry = (from q in db.SCMS_STHs
                    //                   join q1 in db.SCMS_STDs on q.c_nodoc equals q1.c_nodoc
                    //                   where q.c_gdg == gdg
                    //                   && (q.l_delete == false || q.l_delete == null)
                    //                   && q.c_type == "03"
                    //                   && SqlMethods.DateDiffMonth(q.d_date.Value, DateTime.Now) < 2
                    //                   group new { q, q1 } by new { q.d_date, q1.c_no, q1.c_type_lat } into gGroup
                    //                   select new STDetail
                    //                   {
                    //                       noTrans = gGroup.Key.c_no,
                    //                       dTrans = gGroup.Max(a => a.q.d_date),
                    //                       c_type_lat = gGroup.Key.c_type_lat
                    //                   }).Distinct().AsQueryable();
                    //        }

                    //        //var z = qry.ToList();

                    //        if ((parameters != null) && (parameters.Count > 0))
                    //        {
                    //            foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                    //            {
                    //                if (kvp.Value.IsCondition)
                    //                {
                    //                    if (kvp.Value.IsLike)
                    //                    {
                    //                        paternLike = kvp.Value.Value.ToString();
                    //                        qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                    //                    }
                    //                    else if (kvp.Value.IsIn)
                    //                    {
                    //                        #region In Clause

                    //                        qry = qry.In(kvp.Key, (object[])kvp.Value.Value).AsQueryable();

                    //                        #endregion
                    //                    }
                    //                    else
                    //                    {
                    //                        qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                    //                    }
                    //                }
                    //            }
                    //        }

                    //        Logger.WriteLine(qry.Provider.ToString());

                    //        nCount = qry.Count();

                    //        if (nCount > 0)
                    //        {
                    //            if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                    //            {
                    //                qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                    //            }

                    //            if ((limit == -1) || allQuery)
                    //            {
                    //                dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                    //            }
                    //            else
                    //            {
                    //                dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                    //            }
                    //        }

                    //        dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                    //        dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                    //    }
                    //    break;

                    #endregion

                    #endregion

                    #endregion

                    #region Adjust

                    #region Stock

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ADJ_GOODBAD_BATCH:
                        {
                            char gudang = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);

                            #region Old
                            //List<string> lstTipe = new List<string>();
                            //lstTipe.AddRange(new[] { "01", "03", "04", "05" });

                            //var qryRN = (from q in db.LG_RNHs
                            //             join q1 in
                            //               (
                            //                from qd1 in db.LG_RND1s
                            //                group qd1 by new { qd1.c_gdg, qd1.c_iteno, qd1.c_batch } into g
                            //                select new
                            //                {
                            //                  c_gdg = g.Key.c_gdg,
                            //                  c_iteno = g.Key.c_iteno,
                            //                  c_batch = g.Key.c_batch,
                            //                  n_bsisa = g.Sum(x => x.n_bsisa),
                            //                  n_gsisa = g.Sum(x => x.n_gsisa)
                            //                }
                            //               ) on q.c_gdg equals q1.c_gdg
                            //             join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
                            //             where q2.l_aktif == true && q2.l_hide == false
                            //             && (q.l_delete == false || q.l_delete == null)
                            //             && lstTipe.Contains(q.c_type)
                            //             select new
                            //             {
                            //               q.c_gdg,
                            //               q1.c_iteno,
                            //               q2.v_itnam,
                            //               q1.c_batch,
                            //               q1.n_bsisa,
                            //               q1.n_gsisa
                            //             }).Distinct().AsQueryable();

                            //var qryCombo = (from q in
                            //                  (
                            //                    from q in db.LG_ComboHs
                            //                    where (q.l_delete == null || q.l_delete == false) && q.l_confirm == true
                            //                    group q by new { q.c_gdg, q.c_iteno, q.c_batch } into g
                            //                    select new
                            //                    {
                            //                      c_gdg = g.Key.c_gdg,
                            //                      c_iteno = g.Key.c_iteno,
                            //                      c_batch = g.Key.c_batch,
                            //                      n_bsisa = g.Sum(x => x.n_bsisa),
                            //                      n_gsisa = g.Sum(x => x.n_gsisa)
                            //                    }
                            //                  )
                            //                join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                            //                where q1.l_aktif == true && q1.l_hide == false
                            //                select new
                            //                {
                            //                  q.c_gdg,
                            //                  q1.c_iteno,
                            //                  q1.v_itnam,
                            //                  q.c_batch,
                            //                  q.n_bsisa,
                            //                  q.n_gsisa
                            //                }).Distinct().AsQueryable();

                            //var qry = qryCombo.Union(qryRN).Distinct().Where(x => x.c_gdg == gudang && x.c_iteno == item);
                            #endregion

                            var qry = (from qVWS in
                                           (from sq in GlobalQuery.ViewStockLiteContains(db, gudang, item)
                                            group sq by new { sq.c_gdg, sq.c_iteno, sq.c_batch } into g
                                            select new
                                            {
                                                g.Key.c_gdg,
                                                g.Key.c_iteno,
                                                g.Key.c_batch,
                                                n_bsisa = g.Sum(x => x.n_bsisa),
                                                n_gsisa = g.Sum(x => x.n_gsisa)
                                            })
                                       join q1 in db.FA_MasItms on qVWS.c_iteno equals q1.c_iteno
                                       where (qVWS.n_bsisa != 0 || qVWS.n_gsisa != 0)
                                       select new
                                       {
                                           qVWS.c_gdg,
                                           qVWS.c_iteno,
                                           q1.v_itnam,
                                           qVWS.c_batch,
                                           qVWS.n_bsisa,
                                           qVWS.n_gsisa
                                       });
                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region Batch Plus

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ADJ_BATCH_ALL:
                        {
                            char gudang = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);


                            var qry1 = (from q in db.LG_MsBatches
                                        join q1 in db.LG_vwStockAdjs on new { q.c_iteno, q.c_batch, c_gdg = gudang } equals new { q1.c_iteno, q1.c_batch, q1.c_gdg } into q1_1
                                        from qqqqq in q1_1.DefaultIfEmpty()
                                        join q2 in db.FA_MasItms on q.c_iteno equals q2.c_iteno
                                        where q.c_iteno == item
                                        select new
                                        {
                                            c_gdg = qqqqq.c_gdg == null ? gudang : qqqqq.c_gdg,
                                            c_iteno = q.c_iteno,
                                            v_itnam = q2.v_itnam,
                                            c_batch = q.c_batch,
                                            n_gsisa = qqqqq.n_gsisa.HasValue ? qqqqq.n_gsisa : 0,
                                            n_bsisa = qqqqq.n_bsisa.HasValue ? qqqqq.n_bsisa : 0
                                        }).AsQueryable();

                            var qry = (from q in qry1
                                       group q by new
                                       {
                                           q.c_gdg,
                                           q.c_iteno,
                                           q.v_itnam,
                                           q.c_batch
                                       } into g
                                       select new
                                       {
                                           g.Key.c_gdg,
                                           g.Key.c_iteno,
                                           g.Key.v_itnam,
                                           g.Key.c_batch,
                                           n_gsisa = g.Sum(t => t.n_gsisa),
                                           n_bsisa = g.Sum(t => t.n_bsisa)
                                       }).AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #endregion

                    #region Claim

                    #region MODEL_COMMON_QUERY_MULTIPLE_CLAIM_ITEM

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_CLAIM_ITEM:
                        {
                            string supl = (parameters.ContainsKey("supl") ? (string)((Functionals.ParameterParser)parameters["supl"]).Value : string.Empty);

                            if (supl == "00117" || supl == "00112" || supl == "00113" || supl == "00001" || supl == "00120")
                            {
                                string[] rSupplier = new string[] { "00117", "00112", "00113", "00001", "00120" };

                                var qry = (from q in db.FA_MasItms
                                           join q1 in db.LG_DatSups on q.c_nosup equals q1.c_nosup
                                           join q2 in db.FA_DiscDs on q.c_iteno equals q2.c_iteno
                                           where rSupplier.Contains(q.c_nosup)
                                           select new
                                           {
                                               q.c_alkes,
                                               q.c_iteno,
                                               q.c_nosup,
                                               q.c_type,
                                               q.c_via,
                                               q.l_aktif,
                                               q.l_berat,
                                               q.l_combo,
                                               q.l_dinkes,
                                               q.l_gdg,
                                               q.l_hide,
                                               q.l_import,
                                               q.l_mprice,
                                               q.l_opname,
                                               q.n_beli,
                                               q.n_bonus,
                                               q.n_box,
                                               q2.n_discon,
                                               q.n_estimasi,
                                               q.n_PBF,
                                               q.n_pminord,
                                               q.n_qminord,
                                               q.n_qtykons,
                                               q.n_salpri,
                                               q.v_acronim,
                                               q.v_itnam,
                                               q.v_undes
                                           });



                                if ((parameters != null) && (parameters.Count > 0))
                                {


                                    foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                    {
                                        if (kvp.Value.IsCondition)
                                        {
                                            if (kvp.Value.IsLike)
                                            {
                                                paternLike = kvp.Value.Value.ToString();
                                                qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                            }
                                            else if (kvp.Value.IsBetween)
                                            {

                                            }
                                            else
                                            {
                                                qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                            }
                                        }
                                    }
                                }

                                Logger.WriteLine(qry.Provider.ToString());

                                nCount = qry.Count();

                                if (nCount > 0)
                                {
                                    if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                    {
                                        qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                    }

                                    if ((limit == -1) || allQuery)
                                    {
                                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                    }
                                    else
                                    {
                                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                    }
                                }

                                dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                            }
                            else
                            {
                                var qry = (from q in db.FA_MasItms
                                           join q1 in db.LG_DatSups on q.c_nosup equals q1.c_nosup
                                           where q.c_nosup == supl
                                           select new
                                           {
                                               q.c_alkes,
                                               q.c_iteno,
                                               q.c_nosup,
                                               q.c_type,
                                               q.c_via,
                                               q.l_aktif,
                                               q.l_berat,
                                               q.l_combo,
                                               q.l_dinkes,
                                               q.l_gdg,
                                               q.l_hide,
                                               q.l_import,
                                               q.l_mprice,
                                               q.l_opname,
                                               q.n_beli,
                                               q.n_bonus,
                                               q.n_box,
                                               q.n_disc,
                                               q.n_estimasi,
                                               q.n_PBF,
                                               q.n_pminord,
                                               q.n_qminord,
                                               q.n_qtykons,
                                               q.n_salpri,
                                               q.v_acronim,
                                               q.v_itnam,
                                               q.v_undes
                                           });



                                if ((parameters != null) && (parameters.Count > 0))
                                {


                                    foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                    {
                                        if (kvp.Value.IsCondition)
                                        {
                                            if (kvp.Value.IsLike)
                                            {
                                                paternLike = kvp.Value.Value.ToString();
                                                qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                            }
                                            else if (kvp.Value.IsBetween)
                                            {

                                            }
                                            else
                                            {
                                                qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                            }
                                        }
                                    }
                                }

                                Logger.WriteLine(qry.Provider.ToString());

                                nCount = qry.Count();

                                if (nCount > 0)
                                {
                                    if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                    {
                                        qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                    }

                                    if ((limit == -1) || allQuery)
                                    {
                                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                    }
                                    else
                                    {
                                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                    }
                                }

                                dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                            }


                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_CLAIM_MSDIVPRI

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_CLAIM_MSDIVPRI:
                        {
                            var qry = (from q in db.FA_MsDivPris
                                       select new
                                       {
                                           q.c_kddivpri,
                                           q.c_nosup,
                                           q.v_nmdivpri,
                                           q.l_aktif,
                                           q.l_hide
                                       });

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;
                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_CLAIMACC_CURRENT_ITEM

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_CLAIMACC_CURRENT_ITEM:
                        {
                            var qry = (from q1 in db.LG_ClaimAccDs
                                       join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
                                       join q3 in db.LG_DatSups on new { q2.c_nosup } equals new { q3.c_nosup }
                                       where q1.n_sisa > 0
                                       group new { q1, q2, q3 } by new { q2.c_iteno, q2.v_itnam, q3.v_nama } into gSum
                                       select new
                                       {
                                           c_iteno = gSum.Key.c_iteno,
                                           v_itnam = gSum.Key.v_itnam,
                                           v_nama = gSum.Key.v_nama,
                                           n_sisa = gSum.Sum(x => (x.q1.n_sisa.HasValue ? x.q1.n_sisa.Value : 0)),
                                       }).AsQueryable().Distinct();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_CLAIMACC_CURRENT_ITEM_DETIL:
                        {
                            var qry = (from q in db.LG_ClaimAccHes
                                       join q1 in db.LG_ClaimAccDs on q.c_claimaccno equals q1.c_claimaccno
                                       join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
                                       where q1.n_sisa > 0
                                       select new
                                       {
                                           q.c_claimaccno,
                                           q.c_claimnoprinc,
                                           q.d_claimaccdate,
                                           q.d_claimdateprinc,
                                           q1.c_iteno,
                                           q1.n_qtyacc,
                                           q1.n_qtytolak,
                                           q1.n_sisa,
                                           q2.v_itnam
                                       }).AsQueryable().Distinct();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #endregion

                    #region ClaimACC

                    #region MODEL_COMMON_QUERY_MULTIPLE_CLAIMACC_NOCLAIM

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_CLAIMACC_NOCLAIM:
                        {
                            var qry = (from q in db.LG_ClaimD1s
                                       join q1 in db.LG_ClaimHs on q.c_claimno equals q1.c_claimno
                                       where q.n_sisa != 0
                                       select new
                                       {
                                           q1.c_claimno,
                                           q1.c_nosup,
                                           q1.c_type
                                       }).AsQueryable().Distinct();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_CLAIMACC_ITEM

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_CLAIMACC_ITEM:
                        {
                            string claimno = (parameters.ContainsKey("claimno") ? (string)((Functionals.ParameterParser)parameters["claimno"]).Value : string.Empty);

                            var qry = (from q in db.LG_ClaimD1s
                                       join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                       join q2 in db.LG_ClaimHs on q.c_claimno equals q2.c_claimno
                                       where q.n_sisa != 0 && q.c_claimno == claimno
                                       select new
                                       {
                                           q.c_iteno,
                                           q1.v_itnam,
                                           q.c_claimno,
                                           q2.c_type,
                                           q.n_qty,
                                           q.n_sisa,
                                           q.n_salpri,
                                           q.n_disc
                                       }).AsQueryable().Distinct();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #endregion

                    #region Static data Proses RC

                    case Constant.TEST_DATA:
                        {

                            var data = new string[][] {
                  new string[] { "PBB/01", "Item 1","0047", "1234", "Good", "0045", "DO11203456", "RN11045090", "test_ket", "10" },
                  new string[] { "PBR/02", "Item 2","3629", "2134", "Bad", "0059", "DO11203456", "RN11045092", "test_ket", "10"},
                  new string[] { "PBB/03", "Item 3","4443", "2234", "Good", "0065", "DO11203436", "RN11045094", "test_ket", "10"},
                  new string[] { "PBB/04", "Item 4","0363", "2222", "Good", "0080", "DO11203466", "RN11045091", "test_ket", "10" }
              };

                            var qry = (from q in data
                                       select new
                                       {
                                           c_no = q[0],
                                           v_itnam = q[1],
                                           c_iteno = q[2],
                                           c_batch = q[3],
                                           c_itenoType = q[4],
                                           c_dono = q[6],
                                           c_rnno = q[7],
                                           v_ket = q[8],
                                           n_qty = q[9],
                                           n_qtyAcc = q[9]
                                       }).AsQueryable();

                            #region Gak kePake

                            //string[] c_no = { "PBB123", "PBB2222", "PBB333", "PBB4467" };
                            //object[] c_type = { "P", "R", "P", "R" };
                            //object[] c_no = { "PBB123", "PBB2222", "PBB333", "PBB4467" };
                            //object[] c_itemno = { "5059", "5111", "4044", "4443" };
                            //object[] n_qty = { "5", "6", "7", "8" };
                            //object[] d_pbbr = { "2011-10-10", "2011-10-31", "2011-10-12", "2011-10-14" };
                            //object[] c_cusno = { "0105", "0049", "0054", "0066" };
                            //object[] c_gdg = { "1", "1", "2", "1" };
                            //object[] v_ket = { "test 1", "test 2", "test 3", "test 4" };

                            //List<string> c_no = new List<string>();

                            //Object data = (new[] {c_type});

                            //dicTest.Add("c_no", c_no);
                            //dicTest.Add("c_no", c_no);
                            //dicTest.Add("c_itemno", c_itemno);
                            //dicTest.Add("n_qty", n_qty);
                            //dicTest.Add("d_pbbr", d_pbbr);
                            //dicTest.Add("c_cusno", c_cusno);
                            //dicTest.Add("c_gdg", c_gdg);
                            //dicTest.Add("v_ket", v_ket);

                            //string[] words = { "PBB123", "PBB2222", "PBB333", "PBB4467" };
                            //string[] words_1 = { "P", "P", "P", "R" };
                            //string[] words_2 = { "1", "1", "2","1" };

                            //dicy.Add("P", words);

                            //var qry = words.Select((words_1, c_no) => new {}

                            //var data =
                            //    (from c_no in words_1
                            //     from c_type in words
                            //     from c_gdg in words_2
                            //     select new
                            //     {
                            //       c_no,
                            //       c_type,
                            //       c_gdg
                            //     }).ToList().AsQueryable();

                            //var qry = data;

                            #endregion

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region Categori Item

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_CATEGORY:
                        {
                            string tipe = (parameters.ContainsKey("tipe") ? (string)((Functionals.ParameterParser)parameters["tipe"]).Value : string.Empty);

                            var qry = (from q in db.FA_MasItms
                                       where q.l_aktif == true
                                       && (q.l_delete == false || q.l_delete == null) &&
                                       (!(from q1 in db.SCMS_MSITEM_CATs
                                          where q1.l_delete == false
                                          select q1.c_iteno).Contains(q.c_iteno))
                                       select new
                                       {
                                           q.c_iteno,
                                           q.v_itnam,
                                           q.v_undes
                                       }).AsQueryable().Distinct();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region Lantai Kategori Item

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_LANTAI:
                        {
                            string tipe = (parameters.ContainsKey("tipe") ? (string)((Functionals.ParameterParser)parameters["tipe"]).Value : string.Empty);
                            char gudang = (parameters.ContainsKey("gudang") ? (char)((Functionals.ParameterParser)parameters["gudang"]).Value : char.MinValue);

                            var qry = (from q in db.FA_MasItms
                                       where q.l_aktif == true
                                       && (q.l_delete == false || q.l_delete == null) &&
                                       (!(from q1 in db.SCMS_MSITEM_LATs
                                          where (q1.l_delete == false || q1.l_delete == null) && q1.c_gdg == gudang
                                          select q1.c_iteno).Contains(q.c_iteno))
                                       select new
                                       {
                                           q.c_iteno,
                                           q.v_itnam,
                                           q.v_undes
                                       }).AsQueryable().Distinct();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region Via Item

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_VIA:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '0');

                            if (!gdg.Equals("0"))
                            {
                                var qry = (from q in db.FA_MasItms
                                           where q.l_aktif == true
                                           && (q.l_delete == false || q.l_delete == null) &&
                                           (!(from q1 in db.SCMS_MSITEM_VIAs
                                              select q1.c_iteno).Contains(q.c_iteno))
                                           select new
                                           {
                                               q.c_iteno,
                                               q.v_itnam,
                                               q.l_aktif,
                                               q.l_hide,
                                               q.c_nosup,
                                           }).AsQueryable().Distinct();

                                if ((parameters != null) && (parameters.Count > 0))
                                {


                                    foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                    {
                                        if (kvp.Value.IsCondition)
                                        {
                                            if (kvp.Value.IsLike)
                                            {
                                                paternLike = kvp.Value.Value.ToString();
                                                qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                            }
                                            else if (kvp.Value.IsBetween)
                                            {

                                            }
                                            else
                                            {
                                                qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                            }
                                        }
                                    }
                                }

                                Logger.WriteLine(qry.Provider.ToString());

                                nCount = qry.Count();

                                if (nCount > 0)
                                {
                                    if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                    {
                                        qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                    }

                                    if ((limit == -1) || allQuery)
                                    {
                                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                    }
                                    else
                                    {
                                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                    }
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region Tujuan Transfer Gudang Regular
                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_TFGUDANG_REGULAR:
                        {
                            var listgdg = new char[] { '1', '2', '6' };

                            var qry = (from Gud in db.LG_MsGudangs
                                       where listgdg.Contains(Gud.c_gdg)
                                       select Gud);

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region Tujuan Transfer Gudang Karantina
                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_TFGUDANG_KARANTINA:
                        {
                            var listgdg = new char[] { '1', '4' };

                            var qry = (from Gud in db.LG_MsGudangs
                                       where listgdg.Contains(Gud.c_gdg)
                                       select Gud);

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region Tujuan Transfer Gudang Pemusnahan
                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_TFGUDANG_PEMUSNAHAN:
                        {
                            var listgdg = new char[] { '1', '5' };

                            var qry = (from Gud in db.LG_MsGudangs
                                       where listgdg.Contains(Gud.c_gdg)
                                       select Gud);

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else if (kvp.Value.IsIn)
                                        {
                                            #region In Clause

                                            qry = qry.In(kvp.Key, (object[])kvp.Value.Value).AsQueryable();

                                            #endregion
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region List SJ BASPB
                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_LIST_SJ_BASPB:
                        {
                            var qry = (from q in db.LG_SJHs
                                       where q.c_gdg == '1'
                                       && q.c_gdg2 == '2'
                                       && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       && q.l_status == true
                                       && q.l_exp == true
                                       && (SqlMethods.DateDiffYear(q.d_sjdate, DateTime.Now.Date) < 2)
                                       select new
                                       {
                                           q.c_gdg,
                                           q.c_gdg2,
                                           q.c_nosup,
                                           q.c_sjno,
                                           q.d_sjdate
                                       }).AsQueryable();
                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {
                                        }
                                        else if (kvp.Value.IsIn)
                                        {
                                            #region In Clause
                                            qry = qry.In(kvp.Key, (object[])kvp.Value.Value).AsQueryable();
                                            #endregion
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }
                            Logger.WriteLine(qry.Provider.ToString());
                            nCount = qry.Count();
                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }
                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }
                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);
                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;
                    #endregion
                    #region List SJ BASPB
                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_LIST_ITEM_SJ_BASPB:
                        {
                            string sjno = (parameters.ContainsKey("sjno") ? (string)((Functionals.ParameterParser)parameters["sjno"]).Value : string.Empty);
                            var qry = (from q in db.LG_SJHs
                                       join q1 in db.LG_SJD1s on q.c_sjno equals q1.c_sjno
                                       join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
                                       where q.c_gdg == '1'
                                       && q.c_gdg2 == '2'
                                       && q.c_sjno == sjno
                                       && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       && q.l_status == true
                                       group q1 by new { q1.c_iteno, q2.v_itnam } into g
                                       select new
                                       {
                                           c_iteno = g.Key.c_iteno,
                                           v_itnam = g.Key.v_itnam,
                                           n_qty = g.Sum(t => t.n_gqty)
                                       }).AsQueryable();
                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {
                                        }
                                        else if (kvp.Value.IsIn)
                                        {
                                            #region In Clause
                                            qry = qry.In(kvp.Key, (object[])kvp.Value.Value).AsQueryable();
                                            #endregion
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }
                            Logger.WriteLine(qry.Provider.ToString());
                            nCount = qry.Count();
                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }
                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }
                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);
                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;
                    #endregion

                    #region Detail Good All Gdg

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_DETAIL_GOOD_ALLGDG:
                        {
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);

                            var listgdg = new char[] { '1', '2' };

                            var qry = (from q in db.LG_vwStocks
                                       join q1 in db.FA_MasItms on new { q.c_iteno } equals new { q1.c_iteno }
                                       where (q.c_iteno == item)
                                              && listgdg.Contains(q.c_gdg)
                                              && (q.n_gsisa != 0)
                                       group q by new { q.c_gdg, q.c_iteno, q1.v_itnam, q.c_batch } into gsumm
                                       select new
                                       {
                                           c_gdg = gsumm.Key.c_gdg,
                                           c_iteno = gsumm.Key.c_iteno,
                                           v_itnam = gsumm.Key.v_itnam,
                                           c_batch = gsumm.Key.c_batch,
                                           n_gsisa = gsumm.Sum(y => (y.n_gsisa.HasValue ? y.n_gsisa.Value : 0))
                                       }).AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region StockOpname Indra 20171231FM

                    #region GetData

                    case Constant.MODEL_COMMON_QUERY_STOCK_OPNAME_GETDATA:
                        {
                            #region Set Variable

                            string gdgAsal      = (parameters.ContainsKey("gdgAsal") ? (string)((Functionals.ParameterParser)parameters["gdgAsal"]).Value : string.Empty);
                            string DivAMS       = (parameters.ContainsKey("DivAMS") ? (string)((Functionals.ParameterParser)parameters["DivAMS"]).Value : string.Empty);
                            string Suplier      = (parameters.ContainsKey("Suplier") ? (string)((Functionals.ParameterParser)parameters["Suplier"]).Value : string.Empty);
                            string DivSuplier   = (parameters.ContainsKey("DivSuplier") ? (string)((Functionals.ParameterParser)parameters["DivSuplier"]).Value : string.Empty);
                            string Kategori     = (parameters.ContainsKey("Kategori") ? (string)((Functionals.ParameterParser)parameters["Kategori"]).Value : string.Empty);
                            string Items        = (parameters.ContainsKey("Items") ? (string)((Functionals.ParameterParser)parameters["Items"]).Value : string.Empty);
                            string Status       = (parameters.ContainsKey("Status") ? (string)((Functionals.ParameterParser)parameters["Status"]).Value : string.Empty);
                            string Tipe         = (parameters.ContainsKey("Tipe") ? (string)((Functionals.ParameterParser)parameters["Tipe"]).Value : string.Empty);

                            List<StockOpname> listStockOpname = null;

                            #endregion

                            if (Tipe == "1")
                            {
                                #region New Load

                                listStockOpname = (from q in db.Vw_StockOpnames
                                                   where q.Gudang == Convert.ToChar(gdgAsal) &&
                                                       //((DivAMS == "") || q.DivAMS == DivAMS && DivAMS != "") &&
                                                       q.DivAMS.Contains(DivAMS) &&
                                                       //((Suplier == "") || q.KdPrincipal == Suplier && Suplier != "") &&
                                                       q.KdPrincipal.Contains(Suplier) &&
                                                         q.Reff == null &&
                                                       //((DivSuplier == "") || q.KdDivPrincipal == DivSuplier && DivSuplier != "") &&
                                                       q.KdDivPrincipal.Contains(DivSuplier) &&
                                                       //((Items == "") || q.KdBarang == Items && Items != "") &&
                                                       q.KdBarang.Contains(Items) &&
                                                       //((Status == "") || q.StBarang == Status && Status != "") &&
                                                       q.StBarang.Contains(Status) &&
                                                       //((Kategori == "01" && q.KategoriBarang == "01") || (Kategori == "03" && q.KategoriBarang == "03") || (Kategori == "02" && q.KategoriBarang != "01" && q.KategoriBarang != "03"))
                                                       q.KategoriBarang == Kategori

                                                   select new StockOpname()
                                                   {
                                                       kdprincipal      = q.KdPrincipal,
                                                       principal        = q.Principal,
                                                       kddivprincipal   = q.KdDivPrincipal,
                                                       divprincipal     = q.DivPrincipal,
                                                       location         = q.Location.ToString(),
                                                       kdbarang         = q.KdBarang,
                                                       nmbarang         = q.NmBarang,
                                                       stbarang         = q.StBarang,
                                                       batch            = q.Batch,
                                                       qtysys           = q.QtySys.Value,
                                                       SOQty            = q.SOQty,
                                                       recount1         = q.Recount1,
                                                       recount2         = q.Recount2,
                                                       selisih          = q.Selisih,
                                                       expired          =(q.Expired.HasValue ? q.Expired.Value : Functionals.StandardSqlDateTime),
                                                       box              = q.Box,
                                                       stage            = q.Stage.ToString(),
                                                       noform           = q.Reff
                                                   }).OrderByDescending(w => w.batch).OrderByDescending(x => x.nmbarang).OrderByDescending(y => y.divprincipal).OrderByDescending(z => z.principal).ToList();
                                #endregion
                            }
                            else if(Tipe=="2")
                            {
                                #region Load Freeze

                                listStockOpname = (from q in db.LG_StockSavings
                                                   where q.C_GDG == Convert.ToChar(gdgAsal) &&
                                                         q.C_REFF == DivAMS
 
                                                   select new StockOpname()
                                                   {
                                                       kdprincipal      = q.C_NOSUP,
                                                       principal        = q.V_NAMA,
                                                       kddivprincipal   = q.C_KDDIVPRI,
                                                       divprincipal     = q.V_NMDIVPRI,
                                                       location         = q.LOCATION,
                                                       kdbarang         = q.C_ITENO,
                                                       nmbarang         = q.V_ITNAM,
                                                       stbarang         = q.STBARANG,
                                                       batch            = q.C_BATCH,
                                                       qtysys           = q.QTYSYS.Value,
                                                       SOQty            = q.SOQTY.Value,
                                                       recount1         = q.RECOUNT1.Value,
                                                       recount2         = q.RECOUNT2.Value,
                                                       selisih          = q.SELISIH.Value,
                                                       expired          = (q.D_UPDATEEXPIRED.HasValue ? q.D_UPDATEEXPIRED.Value : q.D_EXPIRED),
                                                       box              = q.Box.Value,
                                                       stage            = q.C_STAGE.ToString(),
                                                       noform           = q.C_REFF
                                                   }).OrderByDescending(w => w.batch).OrderByDescending(x => x.nmbarang).OrderByDescending(y => y.divprincipal).OrderByDescending(z => z.principal).ToList();

                                #endregion
                            }
                            else if (Tipe == "3")
                            {
                                #region New Load Per Produk

                                listStockOpname = (from q in db.Vw_StockOpname_PerProduks
                                                       where q.Gudang == Convert.ToChar(gdgAsal) &&
                                                           ((Suplier == "") || q.KdPrincipal == Suplier && Suplier != "") &&
                                                             q.Reff == null &&
                                                           ((DivSuplier == "") || q.KdDivPrincipal == DivSuplier && DivSuplier != "") &&
                                                           ((Items == "") || q.KdBarang == Items && Items != "") &&
                                                           ((Status == "") || q.StBarang == Status && Status != "") &&
                                                           ((Kategori == "01" && q.KategoriBarang == "01") || (Kategori == "03" && q.KategoriBarang == "03") || (Kategori == "02" && q.KategoriBarang != "01" && q.KategoriBarang != "03"))

                                                           select new StockOpname()
                                                           {
                                                               kdprincipal      = q.KdPrincipal,
                                                               principal        = q.Principal,
                                                               kddivprincipal   = q.KdDivPrincipal,
                                                               divprincipal     = q.DivPrincipal,
                                                               location         = q.Location.ToString(),
                                                               kdbarang         = q.KdBarang,
                                                               nmbarang         = q.NmBarang,
                                                               stbarang         = q.StBarang
                                                           }).OrderByDescending(x => x.nmbarang).OrderByDescending(y => y.divprincipal).OrderByDescending(z => z.principal).ToList();
                                #endregion
                            }
                            else if (Tipe == "4")
                            {
                                #region Load Item Select Per Produk

                                int LengthKodeItem;
                                string AmbilKodeItem;

                                List<string> KodeItem = new List<string>();

                                LengthKodeItem = Items.Length;

                                for (int nLoopC = 0; nLoopC < LengthKodeItem; nLoopC++)
                                {
                                    AmbilKodeItem = Items.Substring(nLoopC, 4);

                                    KodeItem.Add(AmbilKodeItem);

                                    nLoopC = nLoopC + 3;
                                }

                                listStockOpname = (from q in db.Vw_StockOpnames
                                                   where q.Gudang == Convert.ToChar(gdgAsal) &&
                                                       ((Suplier == "") || q.KdPrincipal == Suplier && Suplier != "") &&
                                                         q.Reff == null &&
                                                       ((DivSuplier == "") || q.KdDivPrincipal == DivSuplier && DivSuplier != "") &&
                                                       ((Status == "") || q.StBarang == Status && Status != "") &&
                                                       ((Kategori == "01" && q.KategoriBarang == "01") || (Kategori == "03" && q.KategoriBarang == "03") || (Kategori == "02" && q.KategoriBarang != "01" && q.KategoriBarang != "03")) &&
                                                       KodeItem.Contains(q.KdBarang)

                                                   select new StockOpname()
                                                   {
                                                       kdprincipal      = q.KdPrincipal,
                                                       principal        = q.Principal,
                                                       kddivprincipal   = q.KdDivPrincipal,
                                                       divprincipal     = q.DivPrincipal,
                                                       location         = q.Location.ToString(),
                                                       kdbarang         = q.KdBarang,
                                                       nmbarang         = q.NmBarang,
                                                       stbarang         = q.StBarang,
                                                       batch            = q.Batch,
                                                       qtysys           = q.QtySys.Value,
                                                       SOQty            = q.SOQty,
                                                       recount1         = q.Recount1,
                                                       recount2         = q.Recount2,
                                                       selisih          = q.Selisih,
                                                       expired          =(q.Expired.HasValue ? q.Expired.Value : Functionals.StandardSqlDateTime),
                                                       box              = q.Box,
                                                       stage            = q.Stage.ToString(),
                                                       noform           = q.Reff
                                                   }).OrderByDescending(w => w.batch).OrderByDescending(x => x.nmbarang).OrderByDescending(y => y.divprincipal).OrderByDescending(z => z.principal).ToList();
                                #endregion
                            }

                            nCount = listStockOpname.Count();

                            if (nCount > 0)
                            {
                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, listStockOpname.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, listStockOpname.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                           
                        }

                        break;

                    #endregion

                    #region Monitoring

                    case Constant.MODEL_COMMON_QUERY_STOCK_OPNAME_MONITORING:
                        {
                            string gdgAsal = (parameters.ContainsKey("gdgAsal") ? (string)((Functionals.ParameterParser)parameters["gdgAsal"]).Value : string.Empty);

                            List<StockOpname> listStockOpname = null;

                            listStockOpname = (from q in db.LG_StockSavings
                                               where q.C_GDG == Convert.ToChar(gdgAsal)
                                               group new { q } by new { q.C_GDG, q.C_NOSUP, q.V_NAMA, q.C_KDDIVPRI, q.V_NMDIVPRI, q.C_REFF, q.C_STAGE, q.NOADJ } into g                      
                                               select new StockOpname()
                                               {
                                                   gudang           = g.Key.C_GDG.ToString(),
                                                   kdprincipal      = g.Key.C_NOSUP,
                                                   principal        = g.Key.V_NAMA,
                                                   kddivprincipal   = g.Key.C_KDDIVPRI,
                                                   divprincipal     = g.Key.V_NMDIVPRI,
                                                   noform           = g.Key.C_REFF,
                                                   stage            = g.Key.C_STAGE.ToString(),
                                                   noadj            = g.Key.NOADJ
                                               }).OrderByDescending(x => x.divprincipal).OrderByDescending(y => y.principal).ToList();

                            nCount = listStockOpname.Count();

                            if (nCount > 0)
                            {
                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, listStockOpname.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, listStockOpname.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #endregion

                    #region Monitoring PL Indra 20180511FM

                    #region GetData Indra 20180523FM

                    case Constant.MODEL_COMMON_QUERY_MONITORINGPL_GETDATA:
                        {
                            #region Set Variable

                            string gdgAsal      = (parameters.ContainsKey("gdgAsal") ? (string)((Functionals.ParameterParser)parameters["gdgAsal"]).Value : string.Empty);
                            string cabang       = (parameters.ContainsKey("cabang") ? (string)((Functionals.ParameterParser)parameters["cabang"]).Value : string.Empty);

                            List<MonitoringPL> listMonitoringPL = null;

                            #endregion



                            #region New Load

                            if (cabang == "")
                            {
                                listMonitoringPL    = (from q in db.VW_REKAPMONITORING_CSLs
                                                       where q.GUDANG == Convert.ToChar(gdgAsal)
                                                             
                                                       select new MonitoringPL()
                                                       {
                                                           SPRECEIVED   = q.SPRECEIVED.Value,
                                                           CREATEPL     = q.CREATEPL.Value,
                                                           PICKING      = q.PICKING.Value,
                                                           CREATEDO     = q.CREATEDO.Value,
                                                           CHECKING     = q.CHECKING.Value,
                                                           PACKING      = q.PACKING.Value,
                                                           READY        = q.READY.Value                                                       
                                                       }).ToList();
                            }
                            else
                            {
                                listMonitoringPL    = (from q in db.VW_REKAPMONITORINGCAB_CSLs
                                                       where q.GUDANG == Convert.ToChar(gdgAsal) &&
                                                             q.CABANG == cabang
                                                             
                                                       select new MonitoringPL()
                                                       {
                                                           SPRECEIVED   = q.SPRECEIVED.Value,
                                                           CREATEPL     = q.CREATEPL.Value,
                                                           PICKING      = q.PICKING.Value,
                                                           CREATEDO     = q.CREATEDO.Value,
                                                           CHECKING     = q.CHECKING.Value,
                                                           PACKING      = q.PACKING.Value,
                                                           READY        = q.READY.Value                                                       
                                                       }).ToList();
                            }
                            #endregion

                            nCount = listMonitoringPL.Count();

                            if (nCount > 0)
                            {
                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, listMonitoringPL.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, listMonitoringPL.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                           
                        }

                        break;

                    #endregion

                    #endregion

                    #region Email Produk Kosong 20190411FM

                    #region Load data main grid

                    case Constant.MODEL_COMMON_QUERY_EMAIL_PRODUKKOSONG:
                        {                           
                            var qry = (

                                from q in db.SCMS_ProdukKosongs
                                join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                join q2 in db.LG_DatSups on q1.c_nosup equals q2.c_nosup
                                join q3 in db.FA_Divpris on q1.c_iteno equals q3.c_iteno
                                join q4 in db.FA_MsDivPris on q3.c_kddivpri equals q4.c_kddivpri
                                join q5 in db.SCMS_USERs on q.c_entry equals q5.c_nip
                                where q.l_aktif == true &&
                                      q.l_delete == false
                                select new
                                {
                                    q.c_pkno,
                                    q.d_pkdate,
                                    q2.v_nama,
                                    q4.v_nmdivpri,
                                    q1.c_iteno,
                                    q1.v_itnam,
                                    pkdt = q.c_tipe == "01" ? q.d_abe : null,
                                    nedt = q.c_tipe == "01" ? null : q.d_abe,
                                    q.l_sent,
                                    q.d_sent,
                                    q.l_aktif,
                                    q5.v_username,
                                    q.l_delete,
                                    c_tipe = q.c_tipe == "01" ? "PK" : "NE"
                                }).AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                        }

                        break;

                    #endregion

                    #region History Produk Kosong

                    case Constant.MODEL_COMMON_QUERY_EMAIL_HISTORYPRODUKKOSONG:
                        {                           
                            var qry = (
                                from q in db.SCMS_ProdukKosongs
                                join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                join q2 in db.LG_DatSups on q1.c_nosup equals q2.c_nosup
                                join q3 in db.FA_Divpris on q1.c_iteno equals q3.c_iteno
                                join q4 in db.FA_MsDivPris on q3.c_kddivpri equals q4.c_kddivpri
                                join q5 in db.SCMS_USERs on q.c_entry equals q5.c_nip
                                where q.l_aktif == false ||
                                      q.l_delete == true
                                select new
                                {
                                    q.c_pkno,
                                    q.d_pkdate,
                                    q2.v_nama,
                                    q4.v_nmdivpri,
                                    q1.c_iteno,
                                    q1.v_itnam,
                                    pkdt = q.c_tipe == "01" ? q.d_abe : null,
                                    nedt = q.c_tipe == "01" ? null : q.d_abe,
                                    q.l_sent,
                                    q.d_sent,
                                    q.l_delete,
                                    q5.v_username,
                                    q.c_keterangan,
                                    c_tipe = q.c_tipe == "01" ? "PK" : "NE"
                                }).AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                        }

                        break;
                        
                    #endregion

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(
                  "ScmsSoaLibrary.Modules.CommonQueryMulti:ModelGridQuery (First) <-> Switch {0} - {1}", model, ex.Message);
                Logger.WriteLine(ex.StackTrace);

                dic.Add(Constant.DEFAULT_NAMING_EXCEPTION, ex.Message);

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);
            }

            db.Dispose();

            return dic;
        }

        public static IDictionary<string, object> ModelGridQueryCore(string connectionString, int start, int limit, bool allQuery, string sort, string dir, string model, IDictionary<string, Functionals.ParameterParser> parameters)
        {
            IDictionary<string, object> dic = new Dictionary<string, object>();

            ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);
            //db.CommandTimeout = 1000;

            int nCount = 0;
            string paternLike = null;

            try
            {
                switch (model)
                {
                    #region PL

                    #region MODEL_COMMON_QUERY_MULTIPLE_CUSTOMER_PL

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_CUSTOMER_PL:
                        {
                            //var qry = (from q in db.LG_SPHs
                            //           join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno into q_1
                            //           from qSpd1 in q_1.Where(t => t.n_sisa > 0)
                            //           join q2 in db.LG_Cusmas on q.c_cusno equals q2.c_cusno into q_2
                            //           from qCus in q_2.Where(t => t.l_stscus == true && t.l_hide == false && t.l_cabang == true)
                            //           select new { qCus.c_cab, qCus.v_cunam, qCus.c_cusno, qCus.l_cabang }).Distinct().AsQueryable();
                            var qry = (from q in db.LG_Cusmas
                                       where ((q.l_stscus.HasValue ? q.l_stscus.Value : false) == true)
                                        && ((q.l_hide.HasValue ? q.l_hide.Value : false) == false)
                                       select new
                                       {
                                           q.c_cab,
                                           q.v_cunam,
                                           q.c_cusno,
                                           q.l_cabang,
                                           c_gdg_cab = q.c_gdg
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_SUPLIER_PL

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_SUPLIER_PL:
                        {
                            string cusno = (parameters.ContainsKey("cusno") ? (string)((Functionals.ParameterParser)parameters["cusno"]).Value : string.Empty);
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            //Indra 20181226FM Penambahan filter ETD
                            DateTime dtPeriode1 = (parameters.ContainsKey("Periode1") ? (DateTime)((Functionals.ParameterParser)parameters["Periode1"]).Value : DateTime.Now.AddMonths(-1).AddDays(-DateTime.Now.Day + 1).AddHours(-DateTime.Now.Hour).AddMinutes(-DateTime.Now.Minute));
                            DateTime dtPeriode2 = (parameters.ContainsKey("Periode2") ? (DateTime)((Functionals.ParameterParser)parameters["Periode2"]).Value : DateTime.Now);
                            int tgl1 = 0, tgl2 = 0;
                            tgl1 = SqlMethods.DateDiffDay(dtPeriode1, DateTime.Now);
                            tgl2 = SqlMethods.DateDiffDay(dtPeriode2, DateTime.Now);
                            var qry = (from q in db.VW_SPHPLs
                                       where SqlMethods.DateDiffDay(q.d_etdsp,DateTime.Now) <= tgl1 && SqlMethods.DateDiffDay(q.d_etdsp,DateTime.Now) >= tgl2 &&
                                             q.c_cusno == cusno && q.c_gdg == gdg
                                       select new
                                       {
                                           q.c_nosup,
                                           q.v_nama,
                                           q.c_kddivpri,
                                           q.v_nmdivpri
                                       }).Distinct().AsQueryable();

                            #region OLD

                            //List<string> lstItems = null;

                            //var qrySP = (from sq in db.LG_SPHs
                            //             join sqSPD1 in db.LG_SPD1s on sq.c_spno equals sqSPD1.c_spno
                            //             where sqSPD1.n_sisa > 0 && sq.c_cusno == cusno
                            //              && (sq.c_type != "04")
                            //              && ((sq.l_delete.HasValue ? sq.l_delete.Value : false) == false)
                            //              && (SqlMethods.DateDiffMonth(sq.d_spdate, DateTime.Now.Date) < 2)
                            //             select new { sqSPD1.c_iteno, sq.c_spno }).AsQueryable();

                            //Indra 20181226FM Penambahan filter ETD
                            //var qrySP = (from q in db.LG_SPHs
                            //var qrySP = (from q in db.VW_SPHPLs
                            //             where q.D_ETDSP >= dtPeriode1 && q.D_ETDSP <= dtPeriode2 &&
                            //                  (q.C_CUSNO == cusno)
                            //             join q1 in db.LG_SPD1s on q.C_SPNO equals q1.c_spno
                            //             //where q1.n_sisa > 0 && q.d_spdate > DateTime.Now.AddMonths(-1).AddDays(-DateTime.Now.Day + 1).AddHours(-DateTime.Now.Hour).AddMinutes(-DateTime.Now.Minute)
                            //             join q2 in db.LG_Cusmas on q.C_CUSNO equals q2.c_cusno
                                         /*
                                         where (q2.n_days.HasValue ? (SqlMethods.DateDiffDay(q.d_spdate, DateTime.Now.Date) <= q2.n_days) :
                                                                     (SqlMethods.DateDiffMonth(q.d_spdate, DateTime.Now.Date) < 2))
                                         where (q1.n_sisa > 0) && (q.c_cusno == cusno)
                                            && (q.c_type != "06")  //exclude sp pharmanet

                                           & (q.c_type != "04")
                                             //&&   (!(from sq in db.LG_ORHs
                                             //      join sq1 in db.LG_ORD2s on new { sq.c_gdg, sq.c_orno } equals new { sq1.c_gdg, sq1.c_orno }
                                             //      where (sq.c_type == "02") && (sq.c_gdg == gdg)
                                             //       && (sq1.c_spno == q.c_spno) && sq1.c_iteno == q1.c_iteno
                                             //       && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                             //select sq1.c_iteno).Contains(q1.c_iteno))
                                           //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                           //&& (q2.n_days.HasValue ? (SqlMethods.DateDiffDay(q.d_spdate, DateTime.Now.Date) <= q2.n_days) :
                                                                 //(SqlMethods.DateDiffMonth(q.d_spdate, DateTime.Now.Date) < 2))
                                           */
                                          //select new { q1.c_iteno, q.C_SPNO }).AsQueryable();
                            
                            #region Old Coded

                            //Logger.WriteLine(qrySP.Provider.ToString());

                            //var qryOR = (from sq_q in db.LG_ORHs
                            //             join sq_q1 in db.LG_ORD2s on new { sq_q.c_gdg, sq_q.c_orno } equals new { sq_q1.c_gdg, sq_q1.c_orno }//into sq_q1_1
                            //             //from sq_qORD2 in sq_q1_1.Where(t => sq_q.c_gdg == t.c_gdg)
                            //             where sq_q.c_type == "02" && sq_q.c_gdg == gdg
                            //             select new { sq_q1.c_iteno, sq_q1.c_spno }).AsQueryable();

                            //Logger.WriteLine(qryOR.Provider.ToString());

                            //var qrySP_OR = (from q in qrySP
                            //                join q1 in qryOR on new { q.c_spno, q.c_iteno } equals new { q1.c_spno, q1.c_iteno } into q_1
                            //                from qOR in q_1.DefaultIfEmpty()
                            //                where qOR.c_iteno == null
                            //                select new { q.c_iteno }).Distinct().AsQueryable();

                            //lstItems = (from q in qrySP
                            //            join q1 in qryOR on new { q.c_spno, q.c_iteno } equals new { q1.c_spno, q1.c_iteno } into q_1
                            //            from qOR in q_1.DefaultIfEmpty()
                            //            where qOR.c_iteno == null
                            //            group q by q.c_iteno into g
                            //            select g.Key).Distinct().ToList();

                            //Logger.WriteLine(qrySP_OR.Provider.ToString());

                            //var qry = (from q in db.LG_DatSups
                            //           join qItm in db.FA_MasItms on q.c_nosup equals qItm.c_nosup
                            //           join qSPOR in qrySP_OR on qItm.c_iteno equals qSPOR.c_iteno
                            //           join qVWS in GlobalQuery.ViewStockLite(db, gdg) on qSPOR.c_iteno equals qVWS.c_iteno
                            //           where qVWS.c_gdg == gdg && q.l_aktif == true && qVWS.n_gsisa > 0
                            //           select new { q.c_nosup, q.v_nama }).Distinct().AsQueryable();

                            //var qry = (from q in db.LG_DatSups
                            //           join qItm in db.FA_MasItms on q.c_nosup equals qItm.c_nosup
                            //           //join qSPOR in qrySP_OR on qItm.c_iteno equals qSPOR.c_iteno
                            //           //join qVWS in GlobalQuery.ViewStockLite(db, gdg) on qItm.c_iteno equals qVWS.c_iteno
                            //           where (q.l_aktif == true) //&& (qVWS.n_gsisa > 0)
                            //            //&& lstItems.Contains(qItm.c_iteno)
                            //           select new
                            //           {
                            //             q.c_nosup,
                            //             q.v_nama
                            //           }).Distinct().AsQueryable();

                            #endregion

                            //var qrySP1 = (from q in db.LG_SPHs
                            //             join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
                            //             join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
                            //             where (q1.n_sisa > 0) && (q.c_cusno == cusno)
                            //               //&& (q.c_type != "04")
                            //              && (!(from sq in db.LG_ORHs
                            //                    join sq1 in db.LG_ORD2s on new { sq.c_gdg, sq.c_orno } equals new { sq1.c_gdg, sq1.c_orno }
                            //                    where (sq.c_type == "02") && (sq.c_gdg == gdg)
                            //                     && (sq1.c_spno == q.c_spno) && sq1.c_iteno == q1.c_iteno
                            //                     && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                            //                    select sq1.c_iteno).Contains(q1.c_iteno))
                            //              && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                            //              && (SqlMethods.DateDiffMonth(q.d_spdate, DateTime.Now.Date) < 2)
                            //              && q2.c_nosup == "00046"
                            //             select new { q1.c_iteno, q.c_spno }).AsQueryable();

                            //string[] sd = qrySP1.Select(x=>x.c_iteno).ToArray();

                            //string sdo = null;
                            //foreach (string di in sd)
                            //{
                            //  sdo = sdo +"," + di; 
                            //}

                            //var qry1 = (from q1 in db.FA_MasItms
                            //           join q2 in db.LG_DatSups on q1.c_nosup equals q2.c_nosup
                            //           join q4 in
                            //             GlobalQuery.ViewStockLite(db, gdg) on q1.c_iteno equals q4.c_iteno
                            //           where q1.l_aktif == true && q2.l_aktif == true
                            //            && q4.c_gdg == gdg && q4.n_gsisa != 0 && q2.c_nosup == "00046"
                            //           select new
                            //           {
                            //             q1.c_nosup,
                            //             q2.v_nama,
                            //             q1.c_iteno
                            //           }).Distinct().AsQueryable();

                            //string[] sd1 = qry1.Select(x => x.c_iteno).ToArray();

                            //sdo = null;

                            //foreach (string di in sd1)
                            //{
                            //  sdo = sdo + "," + di;
                            //}
                            #endregion

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            //nCount = qry.Count();

                            //if (nCount > 0)
                            //{
                            //    if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                            //    {
                            //        qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                            //    }

                            //    if ((limit == -1) || allQuery)
                            //    {
                            //        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                            //    }
                            //    else
                            //    {
                            //        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                            //    }
                            //}

                            //dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            //dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                            dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_PL

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEMDETAIL_PL:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string cusno = (parameters.ContainsKey("cusno") ? (string)((Functionals.ParameterParser)parameters["cusno"]).Value : string.Empty);
                            string supl = (parameters.ContainsKey("supl") ? (string)((Functionals.ParameterParser)parameters["supl"]).Value : string.Empty);
                            string itemCat = (parameters.ContainsKey("itemCat") ? (string)((Functionals.ParameterParser)parameters["itemCat"]).Value : string.Empty);
                            bool isConf = (parameters.ContainsKey("isconf") ? (bool)((Functionals.ParameterParser)parameters["isconf"]).Value : false);
                            string itemLat = (parameters.ContainsKey("itemLat") ? (string)((Functionals.ParameterParser)parameters["itemLat"]).Value : string.Empty);
                            string divPri = (parameters.ContainsKey("divPri") ? (string)((Functionals.ParameterParser)parameters["divPri"]).Value : string.Empty);
                            //Indra 20181226FM Penambahan filter ETD
                            DateTime dtPeriode1 = (parameters.ContainsKey("Periode1") ? (DateTime)((Functionals.ParameterParser)parameters["Periode1"]).Value : DateTime.Now.AddMonths(-1).AddDays(-DateTime.Now.Day + 1).AddHours(-DateTime.Now.Hour).AddMinutes(-DateTime.Now.Minute));
                            DateTime dtPeriode2 = (parameters.ContainsKey("Periode2") ? (DateTime)((Functionals.ParameterParser)parameters["Periode2"]).Value : DateTime.Now);
                            
                            int tgl1 = 0, tgl2 = 0;
                            tgl1 = SqlMethods.DateDiffDay(dtPeriode1, DateTime.Now);
                            tgl2 = SqlMethods.DateDiffDay(dtPeriode2, DateTime.Now);

                            List<string> lstItems = null;

                            //IQueryable qrySP = null;
                            var qrySP = (from q in db.FA_MasItms
                                         select new
                                         {
                                             c_iteno = q.c_iteno,
                                             v_undes = q.v_undes,
                                             v_itnam = q.v_itnam,
                                             q.n_box
                                         }).AsQueryable();

                            if (isConf)
                            {
                                if (string.IsNullOrEmpty(divPri))
                                {
                                    qrySP = (from q in db.VW_SPHPL_ITEMs
                                             where SqlMethods.DateDiffDay(q.d_etdsp, DateTime.Now) <= tgl1 && SqlMethods.DateDiffDay(q.d_etdsp, DateTime.Now) >= tgl2 &&
                                                   q.c_cusno == cusno &&
                                                   q.c_gdg == gdg &&
                                                   q.c_nosup == supl &&
                                                   ((itemCat == "") || q.c_type == itemCat && itemCat != "")
                                             select new
                                             {
                                                 q.c_iteno,
                                                 q.v_undes,
                                                 q.v_itnam,
                                                 q.n_box
                                             }).AsQueryable();

                                    #region OLD
                                    /*
                                    qrySP = (from q in db.LG_SPHs
                                             where q.d_etdsp >= dtPeriode1 && q.d_etdsp <= dtPeriode2 &&
                                                  (q.c_cusno == cusno) && (q.c_type != "06") && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                             join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno where q1.n_sisa > 0
                                             join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
                                             join q4 in db.LG_Cusmas on q.c_cusno equals q4.c_cusno
                                             join q3 in
                                                 (from sq in db.LG_ORHs
                                                  join sq1 in db.LG_ORD2s on new { sq.c_gdg, sq.c_orno } equals new { sq1.c_gdg, sq1.c_orno }
                                                  where (sq.c_type != "02") && (sq.c_gdg == gdg)
                                                  select new
                                                  {
                                                      sq1.c_spno,
                                                      sq1.c_iteno
                                                  }) on new { q1.c_iteno, q1.c_spno } equals new { q3.c_iteno, q3.c_spno } into gLeft
                                             from q4Lef in gLeft.DefaultIfEmpty()
                                             where //(q1.n_sisa > 0) && (q.c_cusno == cusno)
                                             //&& (q.c_type != "06")  //exclude sp pharmanet
                                                  (!(from sq in db.LG_ORHs
                                                       join sq1 in db.LG_ORD2s on new { sq.c_gdg, sq.c_orno } equals new { sq1.c_gdg, sq1.c_orno }
                                                       where (sq.c_type == "02") && (sq.c_gdg == gdg)
                                                        && (sq1.c_spno == q.c_spno) && (sq1.c_iteno == q1.c_iteno)
                                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                                       select sq1.c_iteno).Contains(q1.c_iteno))
                                             && (string.IsNullOrEmpty(itemCat) ? true : (from sq in db.SCMS_MSITEM_CATs
                                                                                         where (sq.c_type == (string.IsNullOrEmpty(itemCat) || (itemCat == "00") ? sq.c_type : itemCat))
                                                                                         select new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory()
                                                                                         {
                                                                                             c_iteno = (string.IsNullOrEmpty(itemCat) || (itemCat == "00") ? q1.c_iteno : sq.c_iteno)
                                                                                         }).Contains(new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory() { c_iteno = q1.c_iteno }))
                                              //&& ((q.l_delete.Value ? q.l_delete : false) == false)
                                              && (q2.c_nosup == supl) && q2.l_aktif == true
                                              //&& (q4.n_days.HasValue ? (SqlMethods.DateDiffDay(q.d_spdate, DateTime.Now.Date) <= q4.n_days) :
                                                                      //(SqlMethods.DateDiffMonth(q.d_spdate, DateTime.Now.Date) < 2))
                                             select new
                                             {
                                                 c_iteno = q1.c_iteno,
                                                 v_undes = q2.v_undes,
                                                 v_itnam = q2.v_itnam,
                                                 q2.n_box
                                             }).AsQueryable();
                                     */
                                    #endregion
                                }
                                else
                                {
                                    qrySP = (from q in db.VW_SPHPL_ITEM_DIVs
                                             where SqlMethods.DateDiffDay(q.d_etdsp, DateTime.Now) <= tgl1 && SqlMethods.DateDiffDay(q.d_etdsp, DateTime.Now) >= tgl2 &&
                                                   q.c_cusno == cusno &&
                                                   q.c_gdg == gdg &&
                                                   q.c_nosup == supl &&
                                                   ((itemCat == "") || q.c_type == itemCat && itemCat != "") &&
                                                   q.c_kddivpri == divPri
                                             select new
                                             {
                                                 q.c_iteno,
                                                 q.v_undes,
                                                 q.v_itnam,
                                                 q.n_box
                                             }).AsQueryable();

                                    #region OLD
                                    /*
                                    qrySP = (from q in db.LG_SPHs
                                             where q.d_etdsp >= dtPeriode1 && q.d_etdsp <= dtPeriode2 &&
                                              (q.c_cusno == cusno) && (q.c_type != "06") && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                             join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno where q1.n_sisa > 0
                                             join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
                                             join q4 in db.LG_Cusmas on q.c_cusno equals q4.c_cusno
                                             join q3 in
                                                 (from sq in db.LG_ORHs
                                                  join sq1 in db.LG_ORD2s on new { sq.c_gdg, sq.c_orno } equals new { sq1.c_gdg, sq1.c_orno }
                                                  where (sq.c_type != "02") && (sq.c_gdg == gdg)
                                                  select new
                                                  {
                                                      sq1.c_spno,
                                                      sq1.c_iteno
                                                  }) on new { q1.c_iteno, q1.c_spno } equals new { q3.c_iteno, q3.c_spno } into gLeft
                                             from q4Lef in gLeft.DefaultIfEmpty()
                                             join q5 in db.FA_Divpris on q1.c_iteno equals q5.c_iteno
                                             where //(q1.n_sisa > 0) && (q.c_cusno == cusno)
                                             //&& (q.c_type != "06")  //exclude sp pharmanet
                                                 //&& (!(from sq in db.LG_ORHs
                                                 //      join sq1 in db.LG_ORD2s on new { sq.c_gdg, sq.c_orno } equals new { sq1.c_gdg, sq1.c_orno }
                                                 //      where (sq.c_type == "02") && (sq.c_gdg == gdg)
                                                 //       && (sq1.c_spno == q.c_spno) && (sq1.c_iteno == q1.c_iteno)
                                                 //       && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                                 //      select sq1.c_iteno).Contains(q1.c_iteno))
                                             //&& 
                                             (string.IsNullOrEmpty(itemCat) ? true : (from sq in db.SCMS_MSITEM_CATs
                                                                                         where (sq.c_type == (string.IsNullOrEmpty(itemCat) || (itemCat == "00") ? sq.c_type : itemCat))
                                                                                         select new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory()
                                                                                         {
                                                                                             c_iteno = (string.IsNullOrEmpty(itemCat) || (itemCat == "00") ? q1.c_iteno : sq.c_iteno)
                                                                                         }).Contains(new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory() { c_iteno = q1.c_iteno }))
                                              //&& ((q.l_delete.Value ? q.l_delete : false) == false)
                                              && (q2.c_nosup == supl) && q2.l_aktif == true
                                              //&& (q4.n_days.HasValue ? (SqlMethods.DateDiffDay(q.d_spdate, DateTime.Now.Date) <= q4.n_days) :
                                                                      //(SqlMethods.DateDiffMonth(q.d_spdate, DateTime.Now.Date) < 2))
                                              && (string.IsNullOrEmpty(divPri) ? true : q5.c_kddivpri == divPri)
                                              //&& q.d_etdsp >= dtPeriode1 && q.d_etdsp <= dtPeriode2 //Indra 20181226FM Penambahan filter ETD
                                             select new
                                             {
                                                 c_iteno = q1.c_iteno,
                                                 v_undes = q2.v_undes,
                                                 v_itnam = q2.v_itnam,
                                                 q2.n_box
                                             }).AsQueryable();
                                     */
                                    #endregion
                                }

                                #region OLD
                                /*
                                var qry = (from q in qrySP
                                           join q4 in
                                               ((from sq in GlobalQuery.ViewStockLite(db, gdg)
                                                 group sq by sq.c_iteno into g
                                                 where (g.Sum(x => x.n_gsisa) != 0)
                                                 select new
                                                 {
                                                     c_iteno = g.Key
                                                 })) on q.c_iteno equals q4.c_iteno

                                           select new
                                           {
                                               q.v_itnam,
                                               q.c_iteno,
                                               q.v_undes,
                                               q.n_box
                                           }).Distinct().AsQueryable();
                                */
                                #endregion

                                var qry = (from q in qrySP
                                           select new
                                           {
                                               q.v_itnam,
                                               q.c_iteno,
                                               q.v_undes,
                                               q.n_box
                                           }).Distinct().AsQueryable();

                                if ((parameters != null) && (parameters.Count > 0))
                                {
                                    foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                    {
                                        if (kvp.Value.IsCondition)
                                        {
                                            if (kvp.Value.IsLike)
                                            {
                                                paternLike = kvp.Value.Value.ToString();
                                                qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                            }
                                            else if (kvp.Value.IsBetween)
                                            {

                                            }
                                            else
                                            {
                                                qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                            }
                                        }
                                    }
                                }

                                Logger.WriteLine(qry.Provider.ToString());

                                nCount = qry.Count();

                                if (nCount > 0)
                                {
                                    if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                    {
                                        qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                    }

                                    if ((limit == -1) || allQuery)
                                    {
                                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                    }
                                    else
                                    {
                                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                    }
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(divPri))
                                {

                                    qrySP = (from q in db.VW_SPHPL_ITEMs
                                             where SqlMethods.DateDiffDay(q.d_etdsp, DateTime.Now) <= tgl1 && SqlMethods.DateDiffDay(q.d_etdsp, DateTime.Now) >= tgl2 &&
                                                   q.c_cusno == cusno &&
                                                   q.c_gdg == gdg &&
                                                   q.c_nosup == supl &&
                                                   ((itemCat == "") || q.c_type == itemCat && itemCat != "")
                                             select new
                                             {
                                                 q.c_iteno,
                                                 q.v_undes,
                                                 q.v_itnam,
                                                 q.n_box
                                             }).AsQueryable();

                                    #region OLD
                                    /*
                                    qrySP = (from q in db.LG_SPHs
                                             where q.d_etdsp >= dtPeriode1 && q.d_etdsp <= dtPeriode2 &&
                                              (q.c_cusno == cusno) && (q.c_type != "06") && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                             join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
                                             where q1.n_sisa > 0
                                             join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
                                             join q4 in db.LG_Cusmas on q.c_cusno equals q4.c_cusno
                                             join q3 in
                                                 (from sq in db.LG_ORHs
                                                  join sq1 in db.LG_ORD2s on new { sq.c_gdg, sq.c_orno } equals new { sq1.c_gdg, sq1.c_orno }
                                                  where (sq.c_type != "02") && (sq.c_gdg == gdg)
                                                  select new
                                                  {
                                                      sq1.c_spno,
                                                      sq1.c_iteno
                                                  }) on new { q1.c_iteno, q1.c_spno } equals new { q3.c_iteno, q3.c_spno } into gLeft
                                             from q4Lef in gLeft.DefaultIfEmpty()
                                             where //(q1.n_sisa > 0) && (q.c_cusno == cusno)
                                                 //&& q.c_type != "06"
                                                 //&& (!(from sq in db.LG_ORHs
                                                 //      join sq1 in db.LG_ORD2s on new { sq.c_gdg, sq.c_orno } equals new { sq1.c_gdg, sq1.c_orno }
                                                 //      where (sq.c_type == "02") && (sq.c_gdg == gdg)
                                                 //       && (sq1.c_spno == q.c_spno) && (sq1.c_iteno == q1.c_iteno)
                                                 //       && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                                 //      select sq1.c_iteno).Contains(q1.c_iteno))

                                             //&& 
                                             (string.IsNullOrEmpty(itemCat) ? true : (from sq in db.SCMS_MSITEM_CATs
                                                                                      where (sq.c_type == (string.IsNullOrEmpty(itemCat) || (itemCat == "00") ? sq.c_type : itemCat))
                                                                                      select new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory()
                                                                                      {
                                                                                          c_iteno = (string.IsNullOrEmpty(itemCat) || (itemCat == "00") ? q1.c_iteno : sq.c_iteno)
                                                                                      }).Contains(new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory() { c_iteno = q1.c_iteno }))
                                              && (string.IsNullOrEmpty(itemLat) ? true : (from sqs in db.SCMS_MSITEM_LATs
                                                                                          where (sqs.c_type_lat == (string.IsNullOrEmpty(itemLat) || (itemLat == "00") ? sqs.c_type_lat : itemLat))
                                                                                            && sqs.c_gdg == gdg
                                                                                          select new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory()
                                                                                          {
                                                                                              c_iteno = (string.IsNullOrEmpty(itemLat) || (itemLat == "00") ? q1.c_iteno : sqs.c_iteno)
                                                                                          }).Contains(new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory() { c_iteno = q1.c_iteno }))
                                                 //&& ((q.l_delete.Value ? q.l_delete : false) == false)
                                              && (q2.c_nosup == supl) && q2.l_aktif == true
                                             //&& (q4.n_days.HasValue ? (SqlMethods.DateDiffDay(q.d_spdate, DateTime.Now.Date) <= q4.n_days) :
                                             //(SqlMethods.DateDiffMonth(q.d_spdate, DateTime.Now.Date) < 2))
                                             select new
                                             {
                                                 q1.c_iteno,
                                                 q2.v_undes,
                                                 q2.v_itnam,
                                                 q2.n_box
                                             }).AsQueryable();
                                    */
                                    #endregion
                                }
                                else
                                {
                                    qrySP = (from q in db.VW_SPHPL_ITEM_DIVs
                                             where SqlMethods.DateDiffDay(q.d_etdsp, DateTime.Now) <= tgl1 && SqlMethods.DateDiffDay(q.d_etdsp, DateTime.Now) >= tgl2 &&
                                                   q.c_cusno == cusno &&
                                                   q.c_gdg == gdg &&
                                                   q.c_nosup == supl &&
                                                   ((itemCat == "") || q.c_type == itemCat && itemCat != "") &&
                                                   q.c_kddivpri == divPri
                                             select new
                                             {
                                                 q.c_iteno,
                                                 q.v_undes,
                                                 q.v_itnam,
                                                 q.n_box
                                             }).AsQueryable();

                                    #region OLD
                                    /*
                                    qrySP = (from q in db.LG_SPHs
                                             where q.d_etdsp >= dtPeriode1 && q.d_etdsp <= dtPeriode2 &&
                                              (q.c_cusno == cusno) && (q.c_type != "06") && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                             join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno where q1.n_sisa > 0
                                             join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
                                             join q4 in db.LG_Cusmas on q.c_cusno equals q4.c_cusno
                                             join q3 in
                                                 (from sq in db.LG_ORHs
                                                  join sq1 in db.LG_ORD2s on new { sq.c_gdg, sq.c_orno } equals new { sq1.c_gdg, sq1.c_orno }
                                                  where (sq.c_type != "02") && (sq.c_gdg == gdg)
                                                  select new
                                                  {
                                                      sq1.c_spno,
                                                      sq1.c_iteno
                                                  }) on new { q1.c_iteno, q1.c_spno } equals new { q3.c_iteno, q3.c_spno } into gLeft
                                             from q4Lef in gLeft.DefaultIfEmpty()
                                             join q5 in db.FA_Divpris on q1.c_iteno equals q5.c_iteno
                                             where //(q1.n_sisa > 0) && (q.c_cusno == cusno)
                                             //&& q.c_type != "06"
                                                 //&& (!(from sq in db.LG_ORHs
                                                 //      join sq1 in db.LG_ORD2s on new { sq.c_gdg, sq.c_orno } equals new { sq1.c_gdg, sq1.c_orno }
                                                 //      where (sq.c_type == "02") && (sq.c_gdg == gdg)
                                                 //       && (sq1.c_spno == q.c_spno) && (sq1.c_iteno == q1.c_iteno)
                                                 //       && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                                 //      select sq1.c_iteno).Contains(q1.c_iteno))
                                             //&& 
                                             (string.IsNullOrEmpty(itemCat) ? true : (from sq in db.SCMS_MSITEM_CATs
                                                                                         where (sq.c_type == (string.IsNullOrEmpty(itemCat) || (itemCat == "00") ? sq.c_type : itemCat))
                                                                                         select new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory()
                                                                                         {
                                                                                             c_iteno = (string.IsNullOrEmpty(itemCat) || (itemCat == "00") ? q1.c_iteno : sq.c_iteno)
                                                                                         }).Contains(new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory() { c_iteno = q1.c_iteno }))
                                              && (string.IsNullOrEmpty(itemLat) ? true : (from sqs in db.SCMS_MSITEM_LATs
                                                                                          where (sqs.c_type_lat == (string.IsNullOrEmpty(itemLat) || (itemLat == "00") ? sqs.c_type_lat : itemLat))
                                                                                            && sqs.c_gdg == gdg
                                                                                          select new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory()
                                                                                          {
                                                                                              c_iteno = (string.IsNullOrEmpty(itemLat) || (itemLat == "00") ? q1.c_iteno : sqs.c_iteno)
                                                                                          }).Contains(new ScmsSoaLibrary.Modules.CommonQueryMulti.PLItemCategory() { c_iteno = q1.c_iteno }))
                                              //&& ((q.l_delete.Value ? q.l_delete : false) == false)
                                              && (q2.c_nosup == supl) && q2.l_aktif == true
                                              //&& (q4.n_days.HasValue ? (SqlMethods.DateDiffDay(q.d_spdate, DateTime.Now.Date) <= q4.n_days) :
                                                                        //(SqlMethods.DateDiffMonth(q.d_spdate, DateTime.Now.Date) < 2))
                                              && (string.IsNullOrEmpty(divPri) ? true : q5.c_kddivpri == divPri)
                                              //&& q.d_etdsp >= dtPeriode1 && q.d_etdsp <= dtPeriode2 //Indra 20181226FM Penambahan filter ETD
                                             select new
                                             {
                                                 q1.c_iteno,
                                                 q2.v_undes,
                                                 q2.v_itnam,
                                                 q2.n_box
                                             }).AsQueryable();
                                    */
                                    #endregion
                                }

                                #region OLD
                                /*
                                var qry = (from q in qrySP
                                           join q4 in
                                               ((from sq in GlobalQuery.ViewStockLite(db, gdg)
                                                 group sq by sq.c_iteno into g
                                                 where (g.Sum(x => x.n_gsisa) != 0)

                                                 select new
                                                 {
                                                     c_iteno = g.Key
                                                 })) on q.c_iteno equals q4.c_iteno

                                           select new
                                           {
                                               q.v_itnam,
                                               q.c_iteno,
                                               q.v_undes,
                                               q.n_box
                                           }).Distinct().AsQueryable();
                                */
                                #endregion

                                var qry = (from q in qrySP                                           
                                           select new
                                           {
                                               q.v_itnam,
                                               q.c_iteno,
                                               q.v_undes,
                                               q.n_box
                                           }).Distinct().AsQueryable();

                                //var sd = qry.ToList();

                                if ((parameters != null) && (parameters.Count > 0))
                                {
                                    foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                    {
                                        if (kvp.Value.IsCondition)
                                        {
                                            if (kvp.Value.IsLike)
                                            {
                                                paternLike = kvp.Value.Value.ToString();
                                                qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                            }
                                            else if (kvp.Value.IsBetween)
                                            {

                                            }
                                            else
                                            {
                                                qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                            }
                                        }
                                    }
                                }

                                Logger.WriteLine(qry.Provider.ToString());

                                nCount = qry.Count();

                                if (nCount > 0)
                                {
                                    if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                    {
                                        qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                    }

                                    if ((limit == -1) || allQuery)
                                    {
                                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                    }
                                    else
                                    {
                                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                    }
                                }
                            }


                            //lstItems = qrySP.GroupBy(t => t.c_iteno).Select(t => t.Key.Trim()).ToList();

                            //var ii = qrySP.ToList();



                            #region Old Coded

                            //var qrySP = (from sq in db.LG_SPHs
                            //             join sqSPD1 in db.LG_SPD1s on sq.c_spno equals sqSPD1.c_spno
                            //             where sqSPD1.n_sisa > 0 && sq.c_cusno == cusno
                            //             select new { sqSPD1.c_iteno, sq.c_spno }).AsQueryable();

                            //var qrySPItm = (from q in qrySP
                            //                join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                            //                where q1.c_nosup == supl
                            //                select new { q.c_iteno, q.c_spno, q1.c_nosup, q1.v_itnam }).AsQueryable();

                            //var qryOR = (from sq_q in db.LG_ORHs
                            //             join sq_q1 in db.LG_ORD2s on sq_q.c_orno equals sq_q1.c_orno //into sq_q1_1
                            //             //from sq_qORD2 in sq_q1_1.Where(t => sq_q.c_gdg == t.c_gdg)
                            //             where sq_q.c_type == "02" && sq_q.c_gdg == sq_q1.c_gdg
                            //             select new { sq_q1.c_iteno, sq_q1.c_spno, sq_q.c_gdg }).AsQueryable();

                            //var qry = (from q in qrySPItm
                            //           join qVWS in
                            //             (from sq in GlobalQuery.ViewStockLite(db)
                            //              where sq.n_gsisa != 0 && sq.c_gdg == gdg
                            //              group sq by sq.c_iteno into g
                            //              select new { c_iteno = g.Key, n_soh = g.Sum(x => x.n_gsisa) }) on q.c_iteno equals qVWS.c_iteno
                            //           join q1 in qryOR on new { q.c_spno, q.c_iteno } equals new { q1.c_spno, q1.c_iteno } into q_1
                            //           from qOR in q_1.Where(x => x.c_gdg == null).DefaultIfEmpty()
                            //           join q2 in db.LG_DatSups on q.c_nosup equals q2.c_nosup into q_2
                            //           from qSup in q_2.DefaultIfEmpty()
                            //           join q3 in
                            //             (from sq in GlobalQuery.ViewStockLiteSPPendingNew(db)
                            //              where (sq.c_gdg == gdg)
                            //              group sq by sq.c_iteno into g
                            //              select new { c_iteno = g.Key, n_SumPending = g.Sum(x => x.n_pending) }) on q.c_iteno equals q3.c_iteno into q_3
                            //           from qVWSP in q_3.DefaultIfEmpty()
                            //           where qVWS.n_soh > 0
                            //           select new
                            //           {
                            //             q.v_itnam,
                            //             q.c_iteno,
                            //             qSup.v_nama,
                            //             qVWSP.n_SumPending,
                            //             qVWS.n_soh
                            //           }).Distinct().AsQueryable();

                            #endregion



                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_SPDETAIL_PL

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_SPDETAIL_PL:
                        {
                            string cusno = (parameters.ContainsKey("cusno") ? (string)((Functionals.ParameterParser)parameters["cusno"]).Value : string.Empty);
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);
                            //Indra 20181226FM Penambahan filter ETD
                            DateTime dtPeriode1 = (parameters.ContainsKey("Periode1") ? (DateTime)((Functionals.ParameterParser)parameters["Periode1"]).Value : DateTime.Now.AddMonths(-1).AddDays(-DateTime.Now.Day + 1).AddHours(-DateTime.Now.Hour).AddMinutes(-DateTime.Now.Minute));
                            DateTime dtPeriode2 = (parameters.ContainsKey("Periode2") ? (DateTime)((Functionals.ParameterParser)parameters["Periode2"]).Value : DateTime.Now);                            

                            var qrySP = (from sq in db.VW_SP_SISAs
                                         where sq.c_cusno == cusno
                                          && (sq.c_iteno == item)
                                          && sq.d_etdsp >= dtPeriode1 && sq.d_etdsp <= dtPeriode2 //Indra 20181226FM Penambahan filter ETD
                                          //Indra 20181115FM ETD First
                                          //select new { sqSPD1.c_iteno, sq.c_spno, sq.c_sp, sq.d_spdate, sq.c_type, sq.l_cek, sqSPD1.n_sisa, sqSPD1.n_acc }).AsQueryable();
                                          select new { sq.c_iteno, sq.c_spno, sq.c_sp, sq.d_spdate, sq.c_type, sq.l_cek, sq.n_sisa, sq.n_acc, sq.d_etdsp }).AsQueryable();

                            //var qryOR = (from sq_q in db.LG_ORHs
                            //             join sq_q1 in db.LG_ORD2s on sq_q.c_orno equals sq_q1.c_orno //into sq_q1_1
                            //             //from sq_qORD2 in sq_q1_1.Where(t => sq_q.c_gdg == t.c_gdg)
                            //             where sq_q.c_type == "02" && sq_q.c_gdg == sq_q1.c_gdg
                            //             select new { sq_q1.c_iteno, sq_q1.c_spno, sq_q.c_gdg }).AsQueryable();

                            //var qryOR = (from sq_q in db.LG_ORD2s
                            //             join sq_q1 in db.LG_ORHs on sq_q.c_orno equals sq_q1.c_orno //into sq_q1_1
                            //             //from sq_qORD2 in sq_q1_1.Where(t => sq_q.c_gdg == t.c_gdg)
                            //             where sq_q1.c_type == "02" && sq_q.c_gdg == sq_q1.c_gdg
                            //              && (sq_q.c_iteno == item)
                            //              && (((sq_q1 != null) ? (sq_q1.l_delete.HasValue ? sq_q1.l_delete.Value : false) : true) == false)
                            //             //select new { sq_q.c_iteno, sq_q.c_spno }).AsQueryable();
                            //             select sq_q.c_spno).AsQueryable();

                            //var qry = (from sq in db.LG_SPHs
                            //            join sqSPD1 in db.LG_SPD1s on sq.c_spno equals sqSPD1.c_spno
                            //            join q1 in db.LG_ORD2s on new { sq.c_spno, sqSPD1.c_iteno } equals new { q1.c_spno, q1.c_iteno } into q_1
                            //            from qORD2 in q_1.DefaultIfEmpty()
                            //            join qORH in db.LG_ORHs on qORD2.c_orno equals qORH.c_orno
                            //            where sqSPD1.n_sisa > 0 && sq.c_cusno == cusno && qORH.c_type == "02"
                            //            select new { sqSPD1.c_iteno, sq.c_spno, sq.c_sp }).AsQueryable();

                            //var qry = (from q in qrySP
                            //           //join q1 in qryOR on new { q.c_spno, q.c_iteno } equals new { q1.c_spno, q1.c_iteno } into q_1
                            //           //from qOR in q_1.DefaultIfEmpty()
                            //           //where qOR.c_iteno == null && q.c_iteno == item
                            //           where (!qryOR.Contains(q.c_spno))
                            //           select new
                            //           {
                            //             q.c_spno,
                            //             c_sp = q.c_sp.Trim(),
                            //             q.d_spdate,
                            //             v_SpType = (q.c_type == "01" ? "O" :
                            //                         (q.c_type == "02" && ((!q.l_cek.Value) || (q.l_cek == null)) ? "MX" :
                            //                         (q.c_type == "02" && q.l_cek.Value ? "M" :
                            //                         "E"))),
                            //             n_spsisa = q.n_sisa,
                            //             n_spqty = q.n_acc
                            //           }).Distinct().AsQueryable();

                            var qry = (from q in qrySP
                                       //join q1 in qryOR on new { q.c_spno, q.c_iteno } equals new { q1.c_spno, q1.c_iteno } into q_1
                                       //from qOR in q_1.DefaultIfEmpty()
                                       //where qOR.c_iteno == null && q.c_iteno == item
                                       //where (!qryOR.Contains(q.c_spno))
                                       select new
                                       {
                                           q.c_spno,
                                           c_sp = q.c_sp.Trim(),
                                           q.d_spdate,
                                           v_SpType = (q.c_type == "01" ? "O" :
                                                       (q.c_type == "02" && ((!q.l_cek.Value) || (q.l_cek == null)) ? "MX" :
                                                       (q.c_type == "02" && q.l_cek.Value ? "M" :
                                                       "E"))),
                                           n_spsisa = q.n_sisa,
                                           n_spqty = q.n_acc,
                                           d_etdsp = q.d_etdsp.Value.Date //Indra 20181115FM ETD Firsts
                                          
                                       }).Distinct().AsQueryable();

                            //var qry = (from q in qrySPItm
                            //           join qVW in
                            //             (from sq in GlobalQuery.ViewStockLite(db)
                            //              where sq.n_gsisa > 0 && sq.c_gdg == gdg
                            //              select new { sq.c_iteno, sq.c_gdg }) on q.c_iteno equals qVW.c_iteno
                            //           join q1 in qryOR on new { q.c_spno, q.c_iteno } equals new { q1.c_spno, q1.c_iteno } into q_1
                            //           from qOR in q_1.Where(x => x.c_gdg == null).DefaultIfEmpty()
                            //           join q2 in db.LG_DatSups on q.c_nosup equals q2.c_nosup into q_2
                            //           from qSup in q_2.DefaultIfEmpty()
                            //           select new { q.v_itnam, q.c_iteno, qSup.v_nama }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_BATCHDETAIL_PL

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_BATCHDETAIL_PL:
                        {
                            string cusno = (parameters.ContainsKey("cusno") ? (string)((Functionals.ParameterParser)parameters["cusno"]).Value : "?");
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : "?");
                            //string nosp = (parameters.ContainsKey("nosp") ? (string)((Functionals.ParameterParser)parameters["nosp"]).Value : string.Empty);
                            //string tipeBeli = (parameters.ContainsKey("type") ? (string)((Functionals.ParameterParser)parameters["type"]).Value : string.Empty);
                            
                            #region Old Coded

                            //var qryStokBatchSOH = (from q in
                            //                         (from sq in GlobalQuery.ViewStockLite(db)
                            //                          group sq by new { sq.c_iteno, sq.c_batch, sq.c_gdg } into g
                            //                          select new { c_iteno = g.Key.c_iteno, c_batch = g.Key.c_batch, n_gsisa = g.Sum(x => x.n_gsisa), c_gdg = g.Key.c_gdg })
                            //                       join q1 in
                            //                         (from sq in GlobalQuery.ViewStockLite(db)
                            //                          group sq by new { sq.c_iteno, sq.c_gdg } into g
                            //                          select new { g.Key.c_iteno, g.Key.c_gdg, n_sisa = g.Sum(x => x.n_gsisa) }
                            //                          ) on q.c_gdg equals q1.c_gdg into q_1
                            //                       from qVW1 in q_1.Where(x => q.c_iteno == x.c_iteno).DefaultIfEmpty()
                            //                       where q.c_iteno == item && q.c_gdg == gdg && q.n_gsisa != 0
                            //                       select new
                            //                       {
                            //                         q.c_iteno,
                            //                         q.c_batch,
                            //                         n_qtybatch = q.n_gsisa,
                            //                         n_soh = qVW1.n_sisa,
                            //                       }).AsQueryable();

                            #endregion

                            var qry = (from q in
                                           (from sq in GlobalQuery.ViewStockLiteContains(db, gdg, item)
                                            group sq by new { sq.c_iteno, sq.c_batch, sq.d_date, sq.c_gdg } into g
                                            select new { c_iteno = g.Key.c_iteno, c_batch = g.Key.c_batch, d_date = g.Key.d_date, n_gsisa = g.Sum(x => x.n_gsisa), c_gdg = g.Key.c_gdg })

                                       where q.n_gsisa != 0
                                        && (q.c_iteno == item)
                                       select new
                                       {
                                           q.c_iteno,
                                           q.c_batch,
                                           d_expired = q.d_date,
                                           q.d_date,
                                           n_qtybatch = q.n_gsisa,
                                       }).AsQueryable();
                            var sss = qry.ToList();

                            //var qryStokBatchSOH = (from q in
                            //                         db.LG_vwStocks
                            //                       where q.n_gsisa != 0
                            //                        && (q.c_iteno == item) && q.c_gdg == gdg
                            //                       select new
                            //                       {
                            //                         q.c_iteno,
                            //                         q.c_batch,
                            //                         n_qtybatch = q.n_gsisa,
                            //                       }).AsQueryable();

                            #region Old Coded

                            //var qry = (from q in GlobalQuery.ViewStockLite(db)
                            //           join q1 in
                            //             (from sq in GlobalQuery.ViewStockLite(db)
                            //              group sq by new { sq.c_iteno, sq.c_gdg } into g1
                            //              select new
                            //              {
                            //                c_iteno = g1.Key.c_iteno,
                            //                c_gdg = g1.Key.c_gdg,
                            //                n_sisaTotal = g1.Sum(x => x.n_gsisa)
                            //              }) on q.c_iteno equals q1.c_iteno into q_1
                            //           from qNsisa in q_1.Where(x => q.c_gdg == x.c_gdg).DefaultIfEmpty()
                            //           join q2 in db.LG_MsBatches on q.c_iteno equals q2.c_iteno into q_2
                            //           from qMBat in q_2.Where(x => q.c_batch == x.c_batch).DefaultIfEmpty()
                            //           join q3 in
                            //             (from sq in GlobalQuery.ViewStockLiteSPPending(db)
                            //              where sq.c_cusno == cusno && sq.c_iteno == item &&
                            //               sq.c_spno == nosp
                            //              select new
                            //              {
                            //                sq.c_iteno,
                            //                sq.n_pending,
                            //                sq.c_gdg,
                            //                sq.c_cusno
                            //              }) on q.c_iteno equals q3.c_iteno into q_3
                            //           from qNpending in q_3.Where(x => q.c_gdg == x.c_gdg && x.c_cusno == cusno).DefaultIfEmpty()
                            //           where q.n_gsisa > 0 && q.c_gdg == gdg && q.c_iteno == item
                            //           select new
                            //           {
                            //             c_batch = q.c_batch.Trim(),
                            //             qMBat.d_expired,
                            //             q.c_iteno,
                            //             qNsisa.n_sisaTotal,
                            //             q.n_gsisa,
                            //             n_pending = (qNpending.n_pending.HasValue ? qNpending.n_pending.Value : 0)
                            //           });

                            //var qryLeft = (from q in
                            //                 (from sq in GlobalQuery.ViewStockBatch(db)
                            //                  where (sq.c_iteno == item) && (sq.c_gdg == gdg) && (sq.N_GSISA > 0)
                            //                  select new
                            //                  {
                            //                    sq.c_iteno,
                            //                    sq.c_batch,
                            //                    sq.N_GSISA,
                            //                    sq.N_GSISATOTAL
                            //                  })
                            //               join q1 in
                            //                 (from sq in GlobalQuery.ViewStockLiteSPPending(db)
                            //                  where (sq.c_cusno == cusno) && (sq.c_iteno == item) && (sq.c_type == tipeBeli)
                            //                  select new
                            //                  {
                            //                    sq.c_iteno,
                            //                    sq.c_type,
                            //                    sq.n_sisaType,
                            //                    sq.n_sisaGudang
                            //                  }) on q.c_iteno equals q1.c_iteno into q_1
                            //               from qX in q_1.DefaultIfEmpty()
                            //               where (qX == null)
                            //               select new LG_PLFormView3()
                            //               {
                            //                 c_iteno = q.c_iteno,
                            //                 c_batch = q.c_batch,
                            //                 n_gsisa = (q.N_GSISA.HasValue ? q.N_GSISA.Value : 0),
                            //                 n_gsisatotal = (q.N_GSISATOTAL.HasValue ? q.N_GSISATOTAL.Value : 0),
                            //                 c_type = qX.c_type,
                            //                 n_sisaType = (qX.n_sisaType.HasValue ? qX.n_sisaType.Value : 0),
                            //                 n_sisaSPTotal = (qX.n_sisaGudang.HasValue ? qX.n_sisaGudang : 0)
                            //               });

                            ////Logger.WriteLine(qryLeft.Provider.ToString());

                            //var qryRight = (from q in
                            //                  (from sq in GlobalQuery.ViewStockLiteSPPending(db)
                            //                   where (sq.c_cusno == cusno) && (sq.c_iteno == item) && (sq.c_type == tipeBeli)
                            //                   select new
                            //                   {
                            //                     sq.c_iteno,
                            //                     sq.c_type,
                            //                     sq.n_sisaType,
                            //                     sq.n_sisaGudang
                            //                   })
                            //                join q1 in
                            //                  (from sq in GlobalQuery.ViewStockBatch(db)
                            //                   where (sq.c_iteno == item) && (sq.c_gdg == gdg) && (sq.N_GSISA > 0)
                            //                   select new
                            //                   {
                            //                     sq.c_iteno,
                            //                     sq.c_batch,
                            //                     sq.N_GSISA,
                            //                     sq.N_GSISATOTAL
                            //                   }) on q.c_iteno equals q1.c_iteno into q_1
                            //                from qX in q_1.DefaultIfEmpty()
                            //                where (qX == null)
                            //                select new LG_PLFormView3()
                            //               {
                            //                 c_iteno = q.c_iteno,
                            //                 c_batch = qX.c_batch,
                            //                 n_gsisa = (qX.N_GSISA.HasValue ? qX.N_GSISA.Value : 0),
                            //                 n_gsisatotal = (qX.N_GSISATOTAL.HasValue ? qX.N_GSISATOTAL.Value : 0),
                            //                 c_type = q.c_type,
                            //                 n_sisaType = (q.n_sisaType.HasValue ? q.n_sisaType.Value : 0),
                            //                 n_sisaSPTotal = (q.n_sisaGudang.HasValue ? q.n_sisaGudang : 0)
                            //               });

                            ////Logger.WriteLine(qryRight.Provider.ToString());

                            //var qryUni = qryLeft.Union(qryRight).AsQueryable();

                            //var qry = (from q in qryVWSBat
                            //           //join qT in db.LG_MsBatches on new { q.c_batch } equals new { qT.c_batch }
                            //           join q1 in qryUni on q.c_iteno equals q1.c_iteno into q_1
                            //           from qUni in q_1.Where(x => q.c_batch == x.c_batch).DefaultIfEmpty()
                            //           //join q1 in qryUni on new { q.c_iteno, q.c_batch } equals {q1.c_iteno, q1.c_batch} into q_1
                            //           //from qUni in q_1.DefaultIfEmpty()
                            //           select new
                            //           {
                            //             c_batch = q.c_batch.Trim(),
                            //             q.d_expired,
                            //             qUni.n_sisaType,
                            //             qUni.n_sisaSPTotal,
                            //             qUni.n_gsisa,
                            //             qUni.n_gsisatotal
                            //           });

                            #endregion

                            //var qry = (from q in qryStokBatchSOH
                            //           join q1 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q1.c_iteno, q1.c_batch } into q_1
                            //           from qBat in q_1.DefaultIfEmpty()
                            //           //join q2 in db.LG_SPD1s on q.c_iteno equals q2.c_iteno
                            //           //where q2.c_spno == nosp
                            //           //group new { q, qBat } by new { q.c_batch, q.c_iteno, qBat.d_expired } into g
                            //           select new
                            //           {
                            //             c_iteno = g.Key.c_iteno,
                            //             c_batch = g.Key.c_batch.Trim(),
                            //             d_expired = g.Key.d_expired,
                            //             n_qtybatch = g.Sum(x=>x.q.n_qtybatch.HasValue ? x.q.n_qtybatch.Value : 0),
                            //           }).Distinct().AsQueryable();

                            //var qry = (from q in qryStokBatchSOH
                            //           join q1 in db.LG_MsBatches on q.c_batch equals q1.c_batch into q_1
                            //           from qBat in q_1.DefaultIfEmpty()
                            //           join q2 in db.LG_SPD1s on q.c_iteno equals q2.c_iteno
                            //           where q2.c_spno == nosp
                            //           group q by new { q.c_iteno, q.c_batch, qBat.d_expired, q.n_soh, q2.n_sisa } into g
                            //           select new
                            //           {
                            //             c_iteno = g.Key.c_iteno,
                            //             c_batch = g.Key.c_batch.Trim(),
                            //             d_expired = g.Key.d_expired,
                            //             n_qtybatch = g.Sum(x => x.n_qtybatch),
                            //             n_soh = g.Key.n_soh,
                            //             n_spsisa = g.Key.n_sisa
                            //           }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_VIA_PL

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_VIA_PL:
                        {
                            string cusno = (parameters.ContainsKey("cusno") ? (string)((Functionals.ParameterParser)parameters["cusno"]).Value : "?");

                            var qry = (from q in db.LG_Vias
                                       join q1 in db.MsTransDs on q.c_type equals q1.c_type
                                       where q1.c_portal == '3' && q1.c_notrans == "02"
                                       && q.c_cusno == cusno
                                       select new
                                       {
                                           q.c_type,
                                           q1.v_ket,
                                           q.c_cusno,
                                           q1.c_portal,
                                           q1.c_notrans,
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_CATEGORI_PL

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_CATEGORI_PL:
                        {
                            string lantai = (parameters.ContainsKey("lantai") ? (string)((Functionals.ParameterParser)parameters["lantai"]).Value : string.Empty);

                            var qry = (from q in db.SCMS_MSITEM_CATs
                                       join q1 in db.MsTransDs on q.c_type equals q1.c_type
                                       where q1.c_portal == '9' && q1.c_notrans == "001"
                                       && q.c_type_lat == (string.IsNullOrEmpty(lantai) ? q.c_type_lat : lantai)
                                       select new
                                       {
                                           q.c_type,
                                           q1.v_ket,
                                           q.c_type_lat,
                                           q1.c_portal,
                                           q1.c_notrans,
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {
                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region PL Auto

                    #region MODEL_COMMON_QUERY_MULTIPLE_RNKHUSUS_PL_AUTO

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_RNKHUSUS_PL_AUTO:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string cusno = (parameters.ContainsKey("cusno") ? (string)((Functionals.ParameterParser)parameters["cusno"]).Value : string.Empty);
                            string nosup = (parameters.ContainsKey("nosup") ? (string)((Functionals.ParameterParser)parameters["nosup"]).Value : string.Empty);

                            var qry = (from q in db.LG_RNHs
                                       join q1 in db.LG_RND2s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                       join q2 in db.LG_POD2s on new { q1.c_gdg, c_pono = q1.c_no } equals new { q2.c_gdg, q2.c_pono }
                                       //join q3 in db.LG_ORHs on new { q2.c_gdg, q2.c_orno } equals new { q3.c_gdg, q3.c_orno }

                                       where (q.c_gdg.ToString() == gdg.ToString())
                                         && ((q.l_khusus.HasValue ? q.l_khusus.Value : false) == true)
                                         && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false) && q.c_from == nosup
                                         && q.c_gdg.ToString() == gdg.ToString()
                                         && ((from sq in db.LG_RND1s
                                              where (sq.c_gdg == q.c_gdg) && (sq.c_rnno == q.c_rnno)
                                              group sq by new { sq.c_iteno } into g
                                              select new
                                              {
                                                  n_sisa = g.Sum(x => (x.n_gsisa.HasValue ? x.n_gsisa.Value : 0)),
                                                  n_gqty = g.Sum(x => (x.n_gqty.HasValue ? x.n_gqty.Value : 0))
                                              }).All(x => x.n_sisa == x.n_gqty))
                                       group new { q } by new { q.c_gdg, q.c_rnno, q.c_from, q.d_rndate, q.c_dono, q.d_dodate } into gGroup
                                       //group new { q, q3 } by new { q.c_gdg, q.c_rnno, q.c_from, q.d_rndate, q.c_dono, q.d_dodate } into gGroup
                                       select new
                                       {
                                           c_gdg = gGroup.Key.c_gdg,
                                           c_rnno = gGroup.Key.c_rnno,
                                           c_nosup = gGroup.Key.c_from,
                                           d_rndate = gGroup.Key.d_rndate,
                                           c_dono = gGroup.Key.c_dono,
                                           d_dodate = gGroup.Key.d_dodate,
                                           //q3.c_cusno
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_ITEM_PL_AUTO

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_PL_AUTO:
                        {
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            //string cusno = (parameters.ContainsKey("cusno") ? (string)((Functionals.ParameterParser)parameters["cusno"]).Value : string.Empty);
                            //string supl = (parameters.ContainsKey("supl") ? (string)((Functionals.ParameterParser)parameters["supl"]).Value : string.Empty);
                            string noRn = (parameters.ContainsKey("NoRN") ? (string)((Functionals.ParameterParser)parameters["NoRN"]).Value : string.Empty);

                            if (gdg.ToString() == "6")
                            {
                                noRn = (from q in db.LG_RNHs
                                        where q.c_rnno == noRn && q.c_gdg == gdg
                                        select q.c_rnno.Substring(0,10)).SingleOrDefault();
                                gdg = '1';
                            }
                            
                            var qry = (from q in db.LG_RNHs
                                       join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                       join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
                                       where (q.c_gdg.ToString() == gdg.ToString()) && (q.c_type == "06") && ((q.l_khusus.HasValue ? q.l_khusus.Value : false) == true) && (q.c_rnno == noRn)
                                          && ((q2.l_aktif.HasValue ? q2.l_aktif.Value : false) == true) && ((q2.l_hide.HasValue ? q2.l_hide.Value : false) == false)
                                          && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       group new { q1, q2 } by new { q1.c_iteno, q2.v_itnam, q2.v_undes } into g
                                       select new
                                       {
                                           g.Key.c_iteno,
                                           g.Key.v_itnam,
                                           g.Key.v_undes
                                       }).Distinct().AsQueryable();



                            #region Old Coded

                            //var qrySP = (from q in db.LG_SPHs
                            //             join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
                            //             where q1.n_sisa > 0 && q.c_cusno == cusno
                            //              && ((q.l_delete == null) || (q.l_delete == false))
                            //             select new { q1.c_iteno, q.c_spno }).AsQueryable();

                            //var qryOR = (from q in db.LG_ORHs
                            //             join q1 in db.LG_ORD2s on q.c_orno equals q1.c_orno
                            //             where q.c_type == "02" && q.c_gdg == q1.c_gdg
                            //              && ((q.l_delete == null) || (q.l_delete == false))
                            //             select new { q1.c_iteno, q1.c_spno, q.c_gdg }).AsQueryable();

                            //var qry1 = (from q in qrySP
                            //            join q1 in qryOR on new { q.c_spno, q.c_iteno } equals new { q1.c_spno, q1.c_iteno } into q_1
                            //            from qOR in q_1.Where(x => x.c_gdg == null).DefaultIfEmpty()
                            //            join q2 in db.FA_MasItms on q.c_iteno equals q2.c_iteno
                            //            join q3 in db.LG_DatSups on q2.c_nosup equals q3.c_nosup
                            //            select new
                            //            {
                            //              q2.v_itnam,
                            //              q.c_iteno,
                            //              q3.v_nama
                            //            }).Distinct().AsQueryable();

                            //var qry = (from q in qry1
                            //           join q1 in db.LG_RND1s on q.c_iteno equals q1.c_iteno
                            //           where q1.c_gdg == gdg && q1.c_rnno == NoRN
                            //           select q).Distinct().AsQueryable();

                            #endregion

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_SPDETAIL_PL_AUTO

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_SPDETAIL_PL_AUTO:
                        {
                            string cusno = (parameters.ContainsKey("cusno") ? (string)((Functionals.ParameterParser)parameters["cusno"]).Value : string.Empty);
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);

                            var qrySP = (from sq in db.LG_SPHs
                                         join sqSPD1 in db.LG_SPD1s on sq.c_spno equals sqSPD1.c_spno
                                         where sqSPD1.n_sisa > 0 && sq.c_cusno == cusno
                                         select new { sqSPD1.c_iteno, sq.c_spno, sq.c_sp, sq.d_spdate, sq.c_type, sq.l_cek, sqSPD1.n_sisa, sqSPD1.n_qty }).AsQueryable();

                            var qryOR = (from sq_q in db.LG_ORD2s
                                         join sq_q1 in db.LG_ORHs on sq_q.c_orno equals sq_q1.c_orno //into sq_q1_1
                                         where sq_q.c_type == "02" && sq_q.c_gdg == sq_q1.c_gdg
                                         select new { sq_q.c_iteno, sq_q.c_spno }).AsQueryable();

                            var qry = (from q in qrySP
                                       join q1 in qryOR on q.c_iteno equals q1.c_iteno into q_1
                                       from qOR in q_1.Where(t => t.c_spno == q.c_spno).DefaultIfEmpty()
                                       where qOR.c_iteno == null && q.c_iteno == item
                                       select new
                                       {
                                           q.c_spno,
                                           c_sp = q.c_sp.Trim(),
                                           q.d_spdate,
                                           v_SpType = (q.c_type == "01" ? "O" :
                                                       (q.c_type == "02" && ((!q.l_cek.Value) || (q.l_cek == null)) ? "MX" :
                                                       (q.c_type == "02" && q.l_cek.Value ? "M" :
                                                       "E"))),
                                           n_spsisa = q.n_sisa,
                                           n_spqty = q.n_qty
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_BATCH_PL_AUTO

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_BATCH_PL_AUTO:
                        {
                            //string cusno = (parameters.ContainsKey("cusno") ? (string)((Functionals.ParameterParser)parameters["cusno"]).Value : string.Empty);
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : '?');
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);
                            //string nosp = (parameters.ContainsKey("nosp") ? (string)((Functionals.ParameterParser)parameters["nosp"]).Value : string.Empty);
                            string noRn = (parameters.ContainsKey("NoRN") ? (string)((Functionals.ParameterParser)parameters["NoRN"]).Value : string.Empty);

                            var qry = (from q in db.LG_RNHs
                                       join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                       join q2 in db.LG_MsBatches on new { q1.c_iteno, q1.c_batch } equals new { q2.c_iteno, q2.c_batch } into q_2
                                       from qBat in q_2.DefaultIfEmpty()
                                       where (q.c_gdg == gdg) && (q.c_type == "06") && ((q.l_khusus.HasValue ? q.l_khusus.Value : false) == true) && (q.c_rnno == noRn)
                                         && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       group new { q1, qBat } by new { q1.c_iteno, q1.c_batch, qBat.d_expired } into g
                                       select new
                                       {
                                           g.Key.c_iteno,
                                           g.Key.c_batch,
                                           d_expired = (g.Key.d_expired.HasValue ? g.Key.d_expired.Value : new DateTime(1900, 1, 1)),
                                           n_qtybatch = g.Sum(x => (x.q1.n_gsisa.HasValue ? x.q1.n_gsisa.Value : 0))
                                       }).Distinct().AsQueryable();

                            #region Old Coded

                            //var qryStokBatchSOH = (from q in
                            //                         (from sq in GlobalQuery.ViewStockLite(db)
                            //                          group sq by new { sq.c_iteno, sq.c_batch, sq.c_gdg } into g
                            //                          select new { c_iteno = g.Key.c_iteno, c_batch = g.Key.c_batch, n_gsisa = g.Sum(x => x.n_gsisa), c_gdg = g.Key.c_gdg })
                            //                       where q.c_iteno == item && q.c_gdg == gdg && q.n_gsisa != 0
                            //                       select new
                            //                       {
                            //                         q.c_iteno,
                            //                         q.c_batch,
                            //                         n_qtybatch = q.n_gsisa,
                            //                       }).AsQueryable();


                            //var qry1 = (from q in qryStokBatchSOH
                            //            join q1 in db.LG_MsBatches on q.c_batch equals q1.c_batch into q_1
                            //            from qBat in q_1.DefaultIfEmpty()
                            //            select new
                            //            {
                            //              q.c_iteno,
                            //              c_batch = q.c_batch.Trim(),
                            //              qBat.d_expired,
                            //              q.n_qtybatch
                            //            }).Distinct().AsQueryable();

                            //var qry = (from q in qry1
                            //           join q1 in db.LG_RND1s on q.c_iteno equals q1.c_iteno
                            //           where q1.c_gdg == gdg && q1.c_rnno == NoRN
                            //           && q.c_iteno == item
                            //           select q).Distinct().AsQueryable();

                            #endregion

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_BASPB

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_BASPB:
                        {
                            string cusno = (parameters.ContainsKey("cusno") ? (string)((Functionals.ParameterParser)parameters["cusno"]).Value : string.Empty);

                            var qry = (from q in db.SCMS_BASPBHs
                                       join q1 in db.SCMS_BASPBDs on q.c_baspbno equals q1.c_baspbno
                                       where (q.l_delete == false || q.l_delete == null)
                                        && (q1.n_gsisa > 0)
                                        && q.c_cusno == cusno
                                       select new
                                       {
                                           q.c_baspbno,
                                           q.c_cusno
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_NO_PL_PHARMANET

                    case Constant.MODEL_COMMON_QUERY_NO_PL_PHARMANET:
                        {
                            string cusno = (parameters.ContainsKey("cusno") ? (string)((Functionals.ParameterParser)parameters["cusno"]).Value : string.Empty);
                            //string PL = (parameters.ContainsKey(""));
                            //var qry = (from q in db.LG_DOPHs
                            //           join q1 in db.SCMS_BASPBDs on q.c_baspbno equals q1.c_baspbno
                            //           where (q.l_delete == false || q.l_delete == null)
                            //            && (q1.n_gsisa > 0)
                            //            && q.c_cusno == cusno
                            //           select new
                            //           {
                            //               q.c_baspbno,
                            //               q.c_cusno
                            //           }).Distinct().AsQueryable();
                            var qry = (from q in db.LG_DOPHs
                                       join q1 in db.LG_DOPDs on q.c_po_outlet equals q1.c_po_outlet
                                       join q2 in db.LG_CusmasCabs on q.c_cab equals q2.c_cab
                                       join q3 in db.LG_vwOutlets on q1.c_outlet equals q3.c_outlet2
                                       where q.c_plphar != null && q2.c_cusno == cusno
                                       select new 
                                       { 
                                        q.c_plphar,
                                        q2.v_cunam,
                                        q3.v_outlet,
                                        q.c_po_outlet,
                                        q2.c_cusno,
                                       }).AsQueryable();

                            var xxx = qry.ToList();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #endregion

                    #region Current Stok Detail

                    #region SP

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_SP_SPG_PENDING:
                        {
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);
                            //char gdg = (parameters.ContainsKey("gdgTrx") ? (char)((Functionals.ParameterParser)parameters["gdgTrx"]).Value : char.MinValue);
                            char gdgPosStok = (parameters.ContainsKey("gdgStok") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgStok"]).Value) : '0');

                            char[] avaibleGudangOkt = new char[] { '0', '1' };

                            //var qry = (from q in GlobalQuery.ViewStockSPPendingNew(db)
                            //           join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                            //           where (q.c_iteno == item)
                            //             //&& (q.c_gdg == (q1.c_type == "02" ? q.c_gdg : gdg))
                            //             //&& (q.c_gdg == 
                            //             //    (q1.c_type == "02" ? gdg : q.c_gdg))
                            //            && (q.c_gdg ==
                            //                (q1.c_type == "02" ?
                            //                  (avaibleGudangOkt.Contains(gdg) ? q.c_gdg : '?') :
                            //                  (gdg == '0' ? q.c_gdg : gdg)))
                            //           select q).Distinct().AsQueryable();

                            //var qry = (from q in GlobalQuery.ViewStockSPPendingNew(db)
                            //           join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                            //           where (q.c_iteno == item)
                            //              && (q.n_pending > 0)
                            //             && (q.c_gdg ==
                            //                  (q1.c_type == "02" ?
                            //                    (avaibleGudangOkt.Contains(gdg) ? q.c_gdg : '?') :
                            //                    (gdg == '0' ? q.c_gdg : gdg)))
                            //           select q).Distinct().AsQueryable();

                            var qry = (from q in db.LG_vwSPPending_PSPs
                                       //join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
                                       //join q2 in db.LG_Cusmas on q.c_cusno equals q2.c_cusno
                                       join q3 in db.FA_MasItms on q.c_iteno equals q3.c_iteno
                                       join q4 in db.SCMS_MSITEM_CATs on q.c_iteno equals q4.c_iteno
                                       //join qItm in db.FA_MasItms on q1.c_iteno equals qItm.c_iteno
                                       where (q.c_iteno == item)
                                              && q.c_gdg == gdgPosStok
                                              //(q3.c_type == "02" ? (avaibleGudangOkt.Contains(gdgPosStok) ? q.c_gdg : '0') :
                                              //    (q4.c_type == "07" ? (avaibleGudangOkt.Contains(gdgPosStok) ? q.c_gdg : '0') : (gdgPosStok == '0' ? q.c_gdg : gdgPosStok))))
                                       //(gdg == '0' ? q.c_gdg : gdg)))
                                       //(q3.c_type == "02" ? q.c_gdg : (gdg == '0' ? q.c_gdg : gdg)))
                                       //&& (itemName.Length > 0 ? qItm.v_itnam.Contains(itemName) : true)
                                       //&& (itemUndes.Length > 0 ? qItm.v_undes.Contains(itemUndes) : true)
                                       //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       //&& ((q3.c_type == "02" ? q.c_gdg : gdgPosTrx) == q.c_gdg)
                                       select new //q).Distinct().AsQueryable();
                              {
                                  c_spno = q.c_spno,
                                  c_sp = q.c_sp,
                                  d_spdate = q.d_spdate,
                                  v_cunam = q.v_cunam,
                                  n_pending = q.n_sisasp,
                                  ETD = q.d_etdsp,
                                  ETA = q.d_etasp
                              }).Distinct().AsQueryable();

                            if (gdgPosStok == '7')
                            {
                                qry = (from q in db.LG_vwSPPending_PSPs
                                       //join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
                                       //join q2 in db.LG_Cusmas on q.c_cusno equals q2.c_cusno
                                       join q3 in db.FA_MasItms on q.c_iteno equals q3.c_iteno
                                       join q4 in db.SCMS_MSITEM_CATs on q.c_iteno equals q4.c_iteno
                                       //join qItm in db.FA_MasItms on q1.c_iteno equals qItm.c_iteno
                                       where (q.c_iteno == item)
                                              && q.c_gdg == '1'
                                              //(q3.c_type == "02" ? (avaibleGudangOkt.Contains(gdgPosStok) ? q.c_gdg : '0') :
                                              //    (q4.c_type == "07" ? (avaibleGudangOkt.Contains(gdgPosStok) ? q.c_gdg : '0') : (gdgPosStok == '0' ? q.c_gdg : gdgPosStok))))
                                       //(gdg == '0' ? q.c_gdg : gdg)))
                                       //(q3.c_type == "02" ? q.c_gdg : (gdg == '0' ? q.c_gdg : gdg)))
                                       //&& (itemName.Length > 0 ? qItm.v_itnam.Contains(itemName) : true)
                                       //&& (itemUndes.Length > 0 ? qItm.v_undes.Contains(itemUndes) : true)
                                       //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       //&& ((q3.c_type == "02" ? q.c_gdg : gdgPosTrx) == q.c_gdg)
                                       select new //q).Distinct().AsQueryable();
                                       {
                                           c_spno = q.c_spno,
                                           c_sp = q.c_sp,
                                           d_spdate = q.d_spdate,
                                           v_cunam = q.v_cunam,
                                           n_pending = q.n_sisasp,
                                           ETD = q.d_etdsp,
                                           ETA = q.d_etasp
                                       }).Distinct().AsQueryable();
                            }

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region Detail Good

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_DETAIL_GOOD:
                        {
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);
                            char gdgPosStok = (parameters.ContainsKey("gdgStok") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgStok"]).Value) : '0');

                            var qry = (from q in db.LG_vwStocks
                                       join q1 in db.FA_MasItms on new { q.c_iteno } equals new { q1.c_iteno }
                                       join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch }
                                       where (q.c_iteno == item)
                                              //&& (q.c_gdg == gdgPosStok)
                                              && ((gdgPosStok == '0' ? q.c_gdg : gdgPosStok) == q.c_gdg)
                                              && (q.n_gsisa != 0)
                                       group q by new { q.c_gdg, q.c_iteno, q1.v_itnam, q.c_batch, q2.d_expired } into gsumm
                                       select new
                                       {
                                           c_gdg = gsumm.Key.c_gdg,
                                           c_iteno = gsumm.Key.c_iteno,
                                           v_itnam = gsumm.Key.v_itnam,
                                           c_batch = gsumm.Key.c_batch,
                                           d_expired = gsumm.Key.d_expired,
                                           n_gsisa = gsumm.Sum(y => (y.n_gsisa.HasValue ? y.n_gsisa.Value : 0))
                                       }).AsQueryable();
                            if (gdgPosStok == '7')
                            {
                                qry = (from q in db.LG_vwStocks
                                       join q1 in db.FA_MasItms on new { q.c_iteno } equals new { q1.c_iteno }
                                       join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch }
                                       where (q.c_iteno == item)
                                           //&& (q.c_gdg == gdgPosStok)
                                              && (q.c_gdg == '1' || q.c_gdg == '6')
                                              && (q.n_gsisa != 0)
                                       group q by new { q.c_gdg, q.c_iteno, q1.v_itnam, q.c_batch, q2.d_expired } into gsumm
                                       select new
                                       {
                                           c_gdg = gsumm.Key.c_gdg,
                                           c_iteno = gsumm.Key.c_iteno,
                                           v_itnam = gsumm.Key.v_itnam,
                                           c_batch = gsumm.Key.c_batch,
                                           d_expired = gsumm.Key.d_expired,
                                           n_gsisa = gsumm.Sum(y => (y.n_gsisa.HasValue ? y.n_gsisa.Value : 0))
                                       }).AsQueryable();
                            }

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region Detail Bad

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_DETAIL_BAD:
                        {
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);
                            char gdgPosStok = (parameters.ContainsKey("gdgStok") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgStok"]).Value) : '0');

                            var qry = (from q in db.LG_vwStocks
                                       join q1 in db.FA_MasItms on new { q.c_iteno } equals new { q1.c_iteno }
                                       join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch }
                                       where (q.c_iteno == item)

                                              //&& (q.c_gdg == gdgPosStok)
                                              && ((gdgPosStok == '0' ? q.c_gdg : gdgPosStok) == q.c_gdg)
                                              && (q.n_bsisa != 0)
                                       group q by new { q.c_gdg, q.c_iteno, q1.v_itnam, q.c_batch, q2.d_expired } into gsumm
                                       select new
                                       {
                                           c_gdg = gsumm.Key.c_gdg,
                                           c_iteno = gsumm.Key.c_iteno,
                                           v_itnam = gsumm.Key.v_itnam,
                                           c_batch = gsumm.Key.c_batch,
                                           d_expired = gsumm.Key.d_expired,
                                           n_bsisa = gsumm.Sum(y => (y.n_bsisa.HasValue ? y.n_bsisa.Value : 0))
                                       }).AsQueryable();

                            if (gdgPosStok == '7')
                            {
                                qry = (from q in db.LG_vwStocks
                                       join q1 in db.FA_MasItms on new { q.c_iteno } equals new { q1.c_iteno }
                                       join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch }
                                       where (q.c_iteno == item)

                                              //&& (q.c_gdg == gdgPosStok)
                                              && (q.c_gdg == '1' || q.c_gdg == '6')
                                              && (q.n_bsisa != 0)
                                       group q by new { q.c_gdg, q.c_iteno, q1.v_itnam, q.c_batch, q2.d_expired } into gsumm
                                       select new
                                       {
                                           c_gdg = gsumm.Key.c_gdg,
                                           c_iteno = gsumm.Key.c_iteno,
                                           v_itnam = gsumm.Key.v_itnam,
                                           c_batch = gsumm.Key.c_batch,
                                           d_expired = gsumm.Key.d_expired,
                                           n_bsisa = gsumm.Sum(y => (y.n_bsisa.HasValue ? y.n_bsisa.Value : 0))
                                       }).AsQueryable();
                            }

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                        ///
                    #region Tot

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_Tot_PENDING:
                        {
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);
                            char gdgPosStok = (parameters.ContainsKey("gdgStok") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgStok"]).Value) : '0');
                            //char gdgPosTrx = (parameters.ContainsKey("gdgTrx") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgTrx"]).Value) : '0');
                            //string gdgPosStok = (parameters.ContainsKey("gdgStok") ? (string)((Functionals.ParameterParser)parameters["gdgStok"]).Value : string.Empty);

                            //DateTime fromDate = Convert.ToDateTime("2014-11-01");


                            //if (gdg_sot == '1')
                            //{
                            //    gdg_sot = '1';
                            //}
                            //else if (gdg_sot == '2')
                            //{
                            //    gdg_sot = '2';
                            //}

                            if (gdgPosStok == '0')
                            {
                                gdgPosStok = '0';
                            }
                            
                            var qry = (from q in db.vwStockGBGITs
                                       //join q1 in db.FA_MasItms on new { q.c_iteno } equals new { q1.c_iteno }
                                       //join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch }
                                       where (q.c_iteno == item)
                                           //&& (q.c_gdg == gdgPosStok)
                                              && ((gdgPosStok == '0' ? q.Kode_gdg_Asal : gdgPosStok) == q.Kode_gdg_Asal)
                                       //&& (q.n_gsisa != 0)
                                       group q by new { q.gudang_asal, q.gudang_tujuan, q.c_iteno, q.v_itnam, q.c_batch, q.d_expired, q.tipe } into gsumm



                                       select new
                                       {
                                           tipe_tot = gsumm.Key.tipe,
                                           gudang_asal_tot = gsumm.Key.gudang_asal,
                                           gudang_tujuan_tot = gsumm.Key.gudang_tujuan,
                                           c_iteno_Tot = gsumm.Key.c_iteno,
                                           v_itnam_Tot = gsumm.Key.v_itnam,
                                           c_batch_Tot = gsumm.Key.c_batch,
                                           d_expired_Tot = gsumm.Key.d_expired,
                                           n_gsisa_Tot = gsumm.Sum(y => (y.n_gsisa.HasValue ? y.n_gsisa.Value : 0)),
                                           n_bsisa_Tot = gsumm.Sum(y => (y.n_bsisa.HasValue ? y.n_bsisa.Value : 0))
                                           #region old
                                           //tipe_tot = gsumm.Key.tipe,
                                           //gudang_asal_tot = gsumm.Key.gudang_asal.ToString(),
                                           //gudang_tujuan_tot = gsumm.Key.gudang_tujuan.ToString(),
                                           //c_iteno_Tot = gsumm.Key.c_iteno,
                                           //v_itnam_Tot = gsumm.Key.v_itnam,
                                           //c_batch_Tot = gsumm.Key.c_batch,
                                           //d_expired_Tot = gsumm.Key.d_expired.ToString(),
                                           //n_gsisa_Tot = gsumm.Sum(y => (y.n_gsisa.HasValue ? y.n_gsisa.Value : 0)),
                                           //n_bsisa_Tot = gsumm.Sum(y => (y.n_bsisa.HasValue ? y.n_bsisa.Value : 0))
                                           #endregion
                                       }).AsQueryable();
                            if (gdgPosStok == '7')
                            {
                                qry = (from q in db.vwStockGBGITs
                                       //join q1 in db.FA_MasItms on new { q.c_iteno } equals new { q1.c_iteno }
                                       //join q2 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q2.c_iteno, q2.c_batch }
                                       where (q.c_iteno == item)
                                           //&& (q.c_gdg == gdgPosStok)
                                              && (q.Kode_gdg_Asal == '1' || q.Kode_gdg_Asal == '6')
                                       //&& (q.n_gsisa != 0)
                                       group q by new { q.gudang_asal, q.gudang_tujuan, q.c_iteno, q.v_itnam, q.c_batch, q.d_expired, q.tipe } into gsumm
                                       select new
                                       {
                                           tipe_tot = gsumm.Key.tipe,
                                           gudang_asal_tot = gsumm.Key.gudang_asal,
                                           gudang_tujuan_tot = gsumm.Key.gudang_tujuan,
                                           c_iteno_Tot = gsumm.Key.c_iteno,
                                           v_itnam_Tot = gsumm.Key.v_itnam,
                                           c_batch_Tot = gsumm.Key.c_batch,
                                           d_expired_Tot = gsumm.Key.d_expired,
                                           n_gsisa_Tot = gsumm.Sum(y => (y.n_gsisa.HasValue ? y.n_gsisa.Value : 0)),
                                           n_bsisa_Tot = gsumm.Sum(y => (y.n_bsisa.HasValue ? y.n_bsisa.Value : 0))
                                       }).AsQueryable();
                            }

                            var pp = qry.ToList();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion
                        ///

                    #region SiT

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_SiT_PENDING:
                        {
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);
                            //char gdg = (parameters.ContainsKey("gdgTrx") ? (char)((Functionals.ParameterParser)parameters["gdgTrx"]).Value : char.MinValue);
                            char gdgPosStok = (parameters.ContainsKey("gdgStok") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgStok"]).Value) : '0');

                            if (gdgPosStok == '0')
                            {
                                gdgPosStok = '9';
                            }

                            var qry = (from q in db.LG_SJHs
                                       join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                                       join q2 in db.LG_MsGudangs on q.c_gdg equals q2.c_gdg
                                       join q3 in db.LG_MsGudangs on q.c_gdg2 equals q3.c_gdg
                                                                            
                                       where (q.l_status == false) && (q1.c_iteno == item)
                                        && ((q1.n_gqty.Value > 0) || (q1.n_bqty.Value > 0))
                                        && ((gdgPosStok == '0' ? q.c_gdg2 : gdgPosStok) == q.c_gdg2)
                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       group q1 by new { q.c_sjno, q.d_sjdate, q1.c_iteno, gdgAsal = q2.v_gdgdesc, gdgTujuan =q3.v_gdgdesc } into g
                                       select new
                                       {
                                           gdgAsal = g.Key.gdgAsal,
                                           gdgTujuan = g.Key.gdgTujuan,

                                           RefNo = g.Key.c_sjno,
                                           DateRef = g.Key.d_sjdate,
                                           Item = g.Key.c_iteno,
                                           GQty = g.Sum(y => (y.n_gqty.HasValue ? y.n_gqty.Value : 0)),  // (q1.n_gqty.HasValue ? q1.n_gqty.Value : 0),
                                           BQty = g.Sum(y => (y.n_bqty.HasValue ? y.n_bqty.Value : 0)), //(q1.n_bqty.HasValue ? q1.n_bqty.Value : 0)
                                       }).AsQueryable();
                            if (gdgPosStok == '7')
                            {
                                qry = (from q in db.LG_SJHs
                                       join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                                       join q2 in db.LG_MsGudangs on q.c_gdg equals q2.c_gdg
                                       join q3 in db.LG_MsGudangs on q.c_gdg2 equals q3.c_gdg
                                       where (q.l_status == false) && (q1.c_iteno == item)
                                        && ((q1.n_gqty.Value > 0) || (q1.n_bqty.Value > 0))
                                        && (q.c_gdg2 == '1' || q.c_gdg2 == '6')
                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       group q1 by new { q.c_sjno, q.d_sjdate, q1.c_iteno, gdgAsal = q2.v_gdgdesc, gdgTujuan = q3.v_gdgdesc } into g
                                       select new
                                       {
                                           gdgAsal = g.Key.gdgAsal,
                                           gdgTujuan = g.Key.gdgTujuan,

                                           RefNo = g.Key.c_sjno,
                                           DateRef = g.Key.d_sjdate,
                                           Item = g.Key.c_iteno,
                                           GQty = g.Sum(y => (y.n_gqty.HasValue ? y.n_gqty.Value : 0)),  // (q1.n_gqty.HasValue ? q1.n_gqty.Value : 0),
                                           BQty = g.Sum(y => (y.n_bqty.HasValue ? y.n_bqty.Value : 0)), //(q1.n_bqty.HasValue ? q1.n_bqty.Value : 0)
                                       }).AsQueryable();
                            }
                            var gg = qry.ToList();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

					//hafizh awal //PL Boking Hafizh 08 maret 2018
                    #region PL BOKING 

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_PLBOKING_PENDING:
                        {
                           // string item = (parameters.ContainsKey("item_sot") ? (string)((Functionals.ParameterParser)parameters["item_sot"]).Value : string.Empty);
                              string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);
                            //char gdg_sot = (parameters.ContainsKey("gdgTrx") ? (char)((Functionals.ParameterParser)parameters["gdgTrx"]).Value : char.MinValue);
                            char gdgPosStok = (parameters.ContainsKey("gdgStok") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgStok"]).Value) : '0');

                            if (gdgPosStok == '1')
                            {
                                gdgPosStok = '1';
                            }
                            else if (gdgPosStok == '2')
                            {
                                gdgPosStok = '2';
                            }
                            else if (gdgPosStok == '0')
                            {
                                gdgPosStok = '0';
                            }

                            var qry = (from q in db.vwPLBokings
                                       where 
                                        (q.c_iteno == item)
                                        && ((q.n_qty.Value > 0) || (q.n_qty.Value > 0))
                                        && ((gdgPosStok == '0' ? q.c_gdg : gdgPosStok) == q.c_gdg)
                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       group q by new { q.c_gdg,q.c_plno,q.d_pldate,q.c_iteno,q.v_cunam,q.v_itnam } into g
                                       select new
                                       {
                                           #region old
                                           //Gudang = q.c_gdg,
                                           //NamaCabang = q.v_cunam,
                                           //NomorPL = q.c_plno,
                                           //DatePL = q.d_pldate,
                                           //Item = q.c_iteno,
                                           //NamaItem = q.v_itnam,
                                           //Batch = q.c_batch,
                                           //Qty = (q.n_qty.HasValue ? q.n_qty.Value : 0),
                                           //Sisa = (q.n_sisa.HasValue ? q.n_sisa.Value : 0),
                                           #endregion
                                           Gudang = g.Key.c_gdg,
                                           NamaCabang = g.Key.v_cunam,
                                           NomorPL = g.Key.c_plno,
                                           DatePL = g.Key.d_pldate,
                                           Item = g.Key.c_iteno,
                                           NamaItem = g.Key.v_itnam,
                                           Qty = g.Sum(y => (y.n_qty.HasValue ? y.n_qty.Value : 0)),
                                           Sisa = g.Sum(y => (y.n_sisa.HasValue ? y.n_sisa.Value : 0)),
                                       }).AsQueryable();

                            if (gdgPosStok == '7')
                            {
                                qry = (from q in db.vwPLBokings
                                       where
                                        (q.c_iteno == item)
                                        && ((q.n_qty.Value > 0) || (q.n_qty.Value > 0))
                                        && (q.c_gdg == '1' || q.c_gdg == '6')
                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       group q by new { q.c_gdg, q.c_plno, q.d_pldate, q.c_iteno, q.v_cunam, q.v_itnam } into g
                                       select new
                                       {
                                           #region old
                                           //Gudang = q.c_gdg,
                                           //NamaCabang = q.v_cunam,
                                           //NomorPL = q.c_plno,
                                           //DatePL = q.d_pldate,
                                           //Item = q.c_iteno,
                                           //NamaItem = q.v_itnam,
                                           //Batch = q.c_batch,
                                           //Qty = (q.n_qty.HasValue ? q.n_qty.Value : 0),
                                           //Sisa = (q.n_sisa.HasValue ? q.n_sisa.Value : 0),
                                           #endregion
                                           Gudang = g.Key.c_gdg,
                                           NamaCabang = g.Key.v_cunam,
                                           NomorPL = g.Key.c_plno,
                                           DatePL = g.Key.d_pldate,
                                           Item = g.Key.c_iteno,
                                           NamaItem = g.Key.v_itnam,
                                           Qty = g.Sum(y => (y.n_qty.HasValue ? y.n_qty.Value : 0)),
                                           Sisa = g.Sum(y => (y.n_sisa.HasValue ? y.n_sisa.Value : 0)),
                                       }).AsQueryable();
                            }

                            var gg = qry.ToList();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion
                    //hafih akhir

                        //hafizh awal
                    #region GOT

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_SOT_PENDING:
                        {
                            string item = (parameters.ContainsKey("item_sot") ? (string)((Functionals.ParameterParser)parameters["item_sot"]).Value : string.Empty);
                            //char gdg_sot = (parameters.ContainsKey("gdgTrx") ? (char)((Functionals.ParameterParser)parameters["gdgTrx"]).Value : char.MinValue);
                            char gdgPosStok = (parameters.ContainsKey("gdgStok") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgStok"]).Value) : '0');

                            if (gdgPosStok == '1')
                            {
                                gdgPosStok = '1';
                            }
                            else if (gdgPosStok == '2')
                            {
                                gdgPosStok = '2';
                            }

                            else if (gdgPosStok == '0')
                            {
                                gdgPosStok = '0';
                            }
                                                        
                            var qry = (from q in db.vwSJdanDO_GOTs
                                       //join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                                       //join q2 in db.LG_MsGudangs on q.c_gdg equals q2.c_gdg
                                       //join q3 in db.LG_MsGudangs on q.c_gdg2 equals q3.c_gdg
                                       where (q.l_status == false) && (q.c_iteno == item)
                                        && ((q.n_gqty.Value > 0) || (q.n_bqty.Value > 0))
                                        //&& ((gdg_sot == '0' ? q.c_gdg2 : gdg_sot) == q.c_gdg2)
                                        && ((gdgPosStok == '0' ? q.gudang_asal : gdgPosStok) == q.gudang_asal)
                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       group q by new { q.c_sjno, q.d_sjdate, q.c_iteno,q.gudang_asal,q.gudang_tujuan } into g
                                       select new
                                       {
                                           gdgAsal_sot = g.Key.gudang_asal,
                                           gdgTujuan_sot = g.Key.gudang_tujuan,
                                           RefNo_sot = g.Key.c_sjno,
                                           DateRef_sot = g.Key.d_sjdate,
                                           Item = g.Key.c_iteno,
                                           GQty_sot = g.Sum(y => (y.n_gqty.HasValue ? y.n_gqty.Value : 0)),  // (q1.n_gqty.HasValue ? q1.n_gqty.Value : 0),
                                           BQty_sot = g.Sum(y => (y.n_bqty.HasValue ? y.n_bqty.Value : 0)), //(q1.n_bqty.HasValue ? q1.n_bqty.Value : 0)
                                       }).AsQueryable();

                            if (gdgPosStok == '7')
                            {
                                qry = (from q in db.vwSJdanDO_GOTs
                                       //join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                                       //join q2 in db.LG_MsGudangs on q.c_gdg equals q2.c_gdg
                                       //join q3 in db.LG_MsGudangs on q.c_gdg2 equals q3.c_gdg
                                       where (q.l_status == false) && (q.c_iteno == item)
                                        && ((q.n_gqty.Value > 0) || (q.n_bqty.Value > 0))
                                           //&& ((gdg_sot == '0' ? q.c_gdg2 : gdg_sot) == q.c_gdg2)
                                        && (q.gudang_asal == '1' | q.gudang_asal == '6')
                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       group q by new { q.c_sjno, q.d_sjdate, q.c_iteno, q.gudang_asal, q.gudang_tujuan } into g
                                       select new
                                       {
                                           gdgAsal_sot = g.Key.gudang_asal,
                                           gdgTujuan_sot = g.Key.gudang_tujuan,
                                           RefNo_sot = g.Key.c_sjno,
                                           DateRef_sot = g.Key.d_sjdate,
                                           Item = g.Key.c_iteno,
                                           GQty_sot = g.Sum(y => (y.n_gqty.HasValue ? y.n_gqty.Value : 0)),  // (q1.n_gqty.HasValue ? q1.n_gqty.Value : 0),
                                           BQty_sot = g.Sum(y => (y.n_bqty.HasValue ? y.n_bqty.Value : 0)), //(q1.n_bqty.HasValue ? q1.n_bqty.Value : 0)
                                       }).AsQueryable();
                            }

                            #region old
                            //
                            //var qry = (from q in db.LG_SJHs
                            //           join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                            //           //join q2 in db.vw_SJHdanGudangs on new { q.c_gdg, q.c_sjno } equals new { q2.c_gdg, q2.c_sjno }
                            //           //join q3 in db.LG_MsGudangs on q.c_gdg equals q3.v_gdgdesc




                            //           where (q.l_status == false) && (q1.c_iteno == item)
                            //            && ((q1.n_gqty.Value > 0) || (q1.n_bqty.Value > 0))
                            //            && ((gdg == '0' ? q.c_gdg2 : gdg) == q.c_gdg2)
                            //            && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                            //           group q1 by new { q.c_sjno, q.d_sjdate, q1.c_iteno } into g
                            //           select new
                            //           {
                            //               //gdgAsal = g.Key.gdgAsal,
                            //               //gdgTujuan = g.Key.gdgTujuan,
                            //               RefNo = g.Key.c_sjno,
                            //               DateRef = g.Key.d_sjdate,
                            //               Item = g.Key.c_iteno,
                            //               GQty = g.Sum(y => (y.n_gqty.HasValue ? y.n_gqty.Value : 0)),  // (q1.n_gqty.HasValue ? q1.n_gqty.Value : 0),
                            //               BQty = g.Sum(y => (y.n_bqty.HasValue ? y.n_bqty.Value : 0)), //(q1.n_bqty.HasValue ? q1.n_bqty.Value : 0)

                            //           }).AsQueryable();
                            //

                            //var gg = sot.ToList();
                            #endregion

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion
                        //hafih akhir

                    #region PO

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_PO_PENDING:
                        {
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);
                            //char gdg = (parameters.ContainsKey("gdgTrx") ? (char)((Functionals.ParameterParser)parameters["gdgTrx"]).Value : char.MinValue);
                            char gdgPosStok = (parameters.ContainsKey("gdgStok") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgStok"]).Value) : '0');

                            if (gdgPosStok == '2')
                            {
                                gdgPosStok = '9';
                            }
                            else if (gdgPosStok == '0')
                            {
                                gdgPosStok = '1';
                            }
                            else if (gdgPosStok == '7')
                            {
                                gdgPosStok = '1';
                            }

                            //var qry = (from q in GlobalQuery.ViewStockPOPendingNew(db)
                            //           join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                            //           where (q.c_iteno == item)
                            //            && (q.n_sisa > 0)
                            //             && (q.c_gdg == (gdg == '0' ? q.c_gdg : gdg))
                            //           select q).Distinct().AsQueryable();

                            var qry = (from q in db.LG_vwPOPending_news
                                       join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                       where (q.c_iteno == item) 
                                        && (q.n_sisa > 0)
                                         //&& (q.c_gdg == (gdg == '0' ? q.c_gdg : gdg))

                                         && ((gdgPosStok == '0' ? q.c_gdg : gdgPosStok) == q.c_gdg)
                                       select q).Distinct().AsQueryable();

                            //var qry = (from q in db.LG_vwPOPending_news
                            //             //join q1 in db.LG_POD1s on new { q.c_gdg, q.c_pono } equals new { q1.c_gdg, q1.c_pono }
                            //             join qItm in db.FA_MasItms on q.c_iteno equals qItm.c_iteno
                            //             where (q.n_sisa > 0)
                            //                 //&& (SqlMethods.DateDiffMonth(q.d_podate, DateTime.Now) <
                            //                 //   ((q.l_import.HasValue ? q.l_import.Value : false) ? int.MaxValue : 2))
                            //              && ((gdgPosTrx == '0' ? q.c_gdg : gdgPosTrx) == q.c_gdg)
                            //              && (listNoSupl.Length == 0 ? true : listNoSupl.Contains(qItm.c_nosup))
                            //              && (itemCode.Length > 0 ? qItm.c_iteno.StartsWith(itemCode) : true)
                            //              && (itemName.Length > 0 ? qItm.v_itnam.Contains(itemName) : true)
                            //              && (itemUndes.Length > 0 ? qItm.v_undes.Contains(itemUndes) : true)
                            //             //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                            //             select new TempStokLogic()
                            //             {
                            //                 RefNo = q.c_pono,
                            //                 Item = q.c_iteno,
                            //                 GQty = (q.n_sisa.HasValue ? q.n_sisa.Value : 0),
                            //                 BQty = 0
                            //             }).AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                        //hafizh
                    #region SPG

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_ITEM_SPG_PENDING:
                        {
                            string item = (parameters.ContainsKey("item") ? (string)((Functionals.ParameterParser)parameters["item"]).Value : string.Empty);
                            //char gdgPosTrx = (parameters.ContainsKey("gdgTrx") ? (char)((Functionals.ParameterParser)parameters["gdgTrx"]).Value : char.MinValue);
                            char gdgPosStok = (parameters.ContainsKey("gdgStok") ? Convert.ToChar(((Functionals.ParameterParser)parameters["gdgStok"]).Value) : '0');


                            if (gdgPosStok == '1')
                            {
                                gdgPosStok = '9';
                            }
                            else if (gdgPosStok == '2')
                            {
                                gdgPosStok = '2';
                            }
                            else if (gdgPosStok == '0')
                            {
                                gdgPosStok = '2';
                            }
                            else if (gdgPosStok == '7')
                            {
                                gdgPosStok = '9';
                            }

                            //var qry = (from q in GlobalQuery.ViewStockPOPendingNew(db)
                            //           join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                            //           where (q.c_iteno == item)
                            //            && (q.n_sisa > 0)
                            //             && (q.c_gdg == (gdg == '0' ? q.c_gdg : gdg))
                            //           select q).Distinct().AsQueryable();

                            var qry = (from q in db.LG_vwPOPending_news
                                       join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                       where (q.c_iteno == item) 
                                        && (q.n_sisa > 0)
                                        //&& (q.c_gdg == (gdg == '0' ? q.c_gdg : gdg))


                                        && ((gdgPosStok == '0' ? q.c_gdg : gdgPosStok) == q.c_gdg)
                                       select q).Distinct().AsQueryable();

                            //var ll= qry.ToList();

                            //var qry = (from q in db.LG_vwPOPending_news
                            //             //join q1 in db.LG_POD1s on new { q.c_gdg, q.c_pono } equals new { q1.c_gdg, q1.c_pono }
                            //             join qItm in db.FA_MasItms on q.c_iteno equals qItm.c_iteno
                            //             where (q.n_sisa > 0)
                            //                 //&& (SqlMethods.DateDiffMonth(q.d_podate, DateTime.Now) <
                            //                 //   ((q.l_import.HasValue ? q.l_import.Value : false) ? int.MaxValue : 2))
                            //              && ((gdgPosTrx == '0' ? q.c_gdg : gdgPosTrx) == q.c_gdg)
                            //              && (listNoSupl.Length == 0 ? true : listNoSupl.Contains(qItm.c_nosup))
                            //              && (itemCode.Length > 0 ? qItm.c_iteno.StartsWith(itemCode) : true)
                            //              && (itemName.Length > 0 ? qItm.v_itnam.Contains(itemName) : true)
                            //              && (itemUndes.Length > 0 ? qItm.v_undes.Contains(itemUndes) : true)
                            //             //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                            //             select new TempStokLogic()
                            //             {
                            //                 RefNo = q.c_pono,
                            //                 Item = q.c_iteno,
                            //                 GQty = (q.n_sisa.HasValue ? q.n_sisa.Value : 0),
                            //                 BQty = 0
                            //             }).AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion
                        //hafizh

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(
                  "ScmsSoaLibrary.Modules.CommonQueryMulti:ModelGridQueryCore (First) <-> Switch {0} - {1}", model, ex.Message);
                Logger.WriteLine(ex.StackTrace);

                dic.Add(Constant.DEFAULT_NAMING_EXCEPTION, ex.Message);

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);
            }

            db.Dispose();

            return dic;
        }

        public static IDictionary<string, object> ModelGridQueryFinance(string connectionString, int start, int limit, bool allQuery, string sort, string dir, string model, IDictionary<string, Functionals.ParameterParser> parameters)
        {
            IDictionary<string, object> dic = new Dictionary<string, object>();

            ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);
            //db.CommandTimeout = 1000;

            string paternLike = null;
            int nCount = 0;

            try
            {
                switch (model)
                {
                    #region Pembayaran

                    #region MODEL_COMMON_QUERY_MULTIPLE_FAKTUR_SUPLIER
                    //
                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_FAKTUR_SUPLIER:
                        {
                            string supl = (parameters.ContainsKey("supl") ? (string)((Functionals.ParameterParser)parameters["supl"]).Value : string.Empty);

                            var qry = (from q in db.LG_FBHs
                                       where q.c_nosup == supl && q.n_sisa > 0
                                       select new SCMS_UNION_FB_FBR()
                                       {
                                           FakturBeli = q.c_fb,
                                           Faktur = q.c_fbno,
                                           Kurs = q.c_kurs,
                                           SuplierID = q.c_nosup,
                                           FakturDate = q.d_fbdate,
                                           FakturTopDate = (q.d_top.HasValue ? q.d_top.Value : Functionals.StandardSqlDateTime),
                                           Value = (q.n_bilva.HasValue ? q.n_bilva.Value : 0),
                                           SisaTagihan = (q.n_sisa.HasValue ? q.n_sisa.Value : 0),
                                           Tipe = "01",
                                           isDeleted = (q.l_delete.HasValue ? q.l_delete.Value : false)
                                       }).Distinct().Union(from q in db.LG_FBRHs
                                                           where q.c_nosup == supl && q.n_sisa > 0
                                                           select new SCMS_UNION_FB_FBR()
                                                           {
                                                               FakturBeli = q.c_exno,
                                                               Faktur = q.c_fbno,
                                                               Kurs = q.c_kurs,
                                                               SuplierID = q.c_nosup,
                                                               FakturDate = q.d_fbdate,
                                                               FakturTopDate = (q.d_fbdate.HasValue ? q.d_fbdate.Value : Functionals.StandardSqlDateTime),
                                                               Value = (q.n_bilva.HasValue ? q.n_bilva.Value : 0),
                                                               SisaTagihan = (q.n_sisa.HasValue ? q.n_sisa.Value : 0),
                                                               Tipe = "02",
                                                               isDeleted = (q.l_delete.HasValue ? q.l_delete.Value : false)
                                                           }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_FAKTUR_CUSTOMER

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_FAKTUR_CUSTOMER:
                        {
                            string cust = (parameters.ContainsKey("cust") ? (string)((Functionals.ParameterParser)parameters["cust"]).Value : string.Empty);

                            var qry = (from q in db.LG_FJHs
                                       join q1 in db.LG_Cusmas on q.c_cusno equals q1.c_cusno into q_1
                                       from qCus in q_1.DefaultIfEmpty()
                                       where q.c_cusno == cust && q.n_sisa > 0
                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       select new SCMS_UNION_FJ_FJR()
                                       {
                                           FakturEx = "-",
                                           Faktur = q.c_fjno.Trim(),
                                           Kurs = q.c_kurs,
                                           CustomerID = q.c_cusno,
                                           FakturDate = q.d_fjdate,
                                           FakturTopDate = (q.d_top.HasValue ? q.d_top.Value : Functionals.StandardSqlDateTime),
                                           Value = (q.n_net.HasValue ? q.n_net.Value : 0),
                                           SisaTagihan = (q.n_sisa.HasValue ? q.n_sisa.Value : 0),
                                           Tipe = "01",
                                           isCabang = (qCus.l_cabang.HasValue ? qCus.l_cabang.Value : false),
                                           isDeleted = (q.l_delete.HasValue ? q.l_delete.Value : false)
                                       }).Distinct().Union(from q in db.LG_FJRHs
                                                           join q1 in db.LG_Cusmas on q.c_cusno equals q1.c_cusno into q_1
                                                           from qCus in q_1.DefaultIfEmpty()
                                                           where q.c_cusno == cust && q.n_sisa > 0
                                                            && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                                           select new SCMS_UNION_FJ_FJR()
                                                           {
                                                               FakturEx = q.c_exno.Trim(),
                                                               Faktur = q.c_fjno.Trim(),
                                                               Kurs = q.c_kurs,
                                                               CustomerID = q.c_cusno,
                                                               FakturDate = q.d_fjdate,
                                                               FakturTopDate = (q.d_fjdate.HasValue ? q.d_fjdate.Value : Functionals.StandardSqlDateTime),
                                                               Value = (q.n_net.HasValue ? q.n_net.Value : 0),
                                                               SisaTagihan = (q.n_sisa.HasValue ? q.n_sisa.Value : 0),
                                                               Tipe = "02",
                                                               isCabang = (qCus.l_cabang.HasValue ? qCus.l_cabang.Value : false),
                                                               isDeleted = (q.l_delete.HasValue ? q.l_delete.Value : false)
                                                           }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_FB_RN

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_FB_RN:
                        {
                            string noSup = (parameters.ContainsKey("suplierId") ? (string)((Functionals.ParameterParser)parameters["suplierId"]).Value : string.Empty);

                            List<string> listTipe = new List<string>();
                            listTipe.Add("01");
                            listTipe.Add("06");

                            var qry = (from q in db.LG_RNHs
                                       join q2 in db.LG_RND2s on new { q.c_gdg, q.c_rnno } equals new { q2.c_gdg, q2.c_rnno }
                                       join q1 in db.LG_MsGudangs on q.c_gdg equals q1.c_gdg into q_1
                                       from qGdg in q_1.DefaultIfEmpty()
                                       where q.l_float == false && q.l_status == false && q.c_rnno.Substring(0, 3) != "RNX"
                                         && q.c_from == noSup && listTipe.Contains(q.c_type) && SqlMethods.DateDiffMonth(q.d_rndate, DateTime.Today) < 3
                                       select new SCMS_RNFaktur_UNION()
                                       {
                                           Gudang = q.c_gdg,
                                           GudangDesc = (qGdg == null ? string.Empty : (qGdg.v_gdgdesc == null ? string.Empty : qGdg.v_gdgdesc.Trim())),
                                           DeliveryNo = q.c_dono,
                                           NoSupl = q.c_from,
                                           ReceiveNote = q.c_rnno,
                                           TanggalRN = q.d_rndate,
                                           TanggalDO = q.d_dodate
                                       }).Distinct().AsQueryable();

                            //var qryRNF = (from q in db.LG_RNHs
                            //              join q1 in db.LG_RND4s on new { q.c_gdg, q.c_rnno } equals new { c_gdg = (char)q1.c_gdg, q1.c_rnno }
                            //              join q2 in db.LG_MsGudangs on q.c_gdg equals q2.c_gdg into q_2
                            //              from qGdg in q_2.DefaultIfEmpty()
                            //              where q.l_float == true && q.l_status == false && q1.c_fb == ""
                            //                && q1.c_fb.Substring(0, 2) != "RS" && q.c_from == noSup
                            //                && q.c_type == "01"
                            //              select new SCMS_RNFaktur_UNION()
                            //              {
                            //                Gudang = q.c_gdg,
                            //                GudangDesc = (qGdg == null ? string.Empty : (qGdg.v_gdgdesc == null ? string.Empty : qGdg.v_gdgdesc.Trim())),
                            //                DeliveryNo = q1.c_fb,
                            //                NoSupl = q.c_from,
                            //                ReceiveNote = string.Concat("RNFLOATING | ", q1.c_fb),
                            //                TanggalRN = q.d_rndate,
                            //                TanggalDO = q.d_dodate
                            //              }).Distinct().AsQueryable();

                            //var qry = qryRN.Union(qryRNF).AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_FB_RN_ITEM

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_FB_RN_ITEM:
                        {
                            string noSup = (parameters.ContainsKey("suplierId") ? (string)((Functionals.ParameterParser)parameters["suplierId"]).Value : string.Empty);
                            string noRcv = (parameters.ContainsKey("receiveId") ? (string)((Functionals.ParameterParser)parameters["receiveId"]).Value : string.Empty);
                            char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : char.MinValue);
                            bool isFloat = (parameters.ContainsKey("isFloating") ? (bool)((Functionals.ParameterParser)parameters["isFloating"]).Value : false);
                            //bool isRn = (string.IsNullOrEmpty(noRcv) || noRcv.Length < 2 ? false
                            //  : (noRcv.Substring(0, 2).Equals("RN", StringComparison.OrdinalIgnoreCase) ? true : false));

                            //string[] inTypeRN = new string[] { "01", "06" };

                            List<string> lstTypeRNFB = new List<string>();
                            lstTypeRNFB.Add("01");
                            lstTypeRNFB.Add("06");

                            IQueryable<SCMS_RNFaktur_ITEM_UNION> qry = default(IQueryable<SCMS_RNFaktur_ITEM_UNION>);

                            if (isFloat)
                            {
                                qry = (from q in db.LG_RNHs
                                       join q1 in db.LG_RND4s on new { q.c_gdg, q.c_rnno } equals new { c_gdg = (char)q1.c_gdg, q1.c_rnno }
                                       join q2 in db.FA_MasItms on new { c_nosup = q.c_from, q1.c_iteno } equals new { q2.c_nosup, q2.c_iteno } into q_2
                                       from qItm in q_2.DefaultIfEmpty()
                                       join q3 in db.MsTransDs on new { q1.c_type, c_portal = '3', c_notrans = "06" } equals new { q3.c_type, q3.c_portal, q3.c_notrans } into q_3
                                       from qTypFBD in q_3.DefaultIfEmpty()
                                       where lstTypeRNFB.Contains(q.c_type) && q.c_from == noSup
                                        && (q.c_gdg == gdg)
                                        && q1.c_fb == (string.IsNullOrEmpty(noRcv) ? "XxXxXx" : noRcv)
                                        && q1.l_status == false && q.l_float == true
                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       group new { q1, qItm, qTypFBD } by new { q1.c_iteno, q1.c_type, q1.n_disc, qItm.n_salpri, qItm.v_itnam, qTypFBD.v_ket } into g
                                       select new SCMS_RNFaktur_ITEM_UNION()
                                       {
                                           c_iteno = g.Key.c_iteno,
                                           v_itnam = g.Key.v_itnam,
                                           c_type = g.Key.c_type,
                                           v_type_desc = g.Key.v_ket,
                                           n_bea = 0,
                                           n_disc = (g.Key.n_disc.HasValue ? g.Key.n_disc.Value : 0),
                                           n_qty = g.Sum(x => (x.q1.n_qty.HasValue ? x.q1.n_qty.Value : 0)),
                                           n_salpri = (g.Key.n_salpri.HasValue ? g.Key.n_salpri.Value : 0)
                                       }).Distinct().AsQueryable();
                            }
                            else
                            {
                                qry = (from q in db.LG_RNHs
                                       join q1 in db.LG_RND2s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                       join q2 in db.FA_MasItms on new { c_nosup = q.c_from, q1.c_iteno } equals new { q2.c_nosup, q2.c_iteno } into q_2
                                       from qItm in q_2.DefaultIfEmpty()
                                       join q3 in db.MsTransDs on new { q1.c_type, c_portal = '3', c_notrans = "06" } equals new { q3.c_type, q3.c_portal, q3.c_notrans } into q_3
                                       from qTypFBD in q_3.DefaultIfEmpty()
                                       join q4 in
                                           (from sq in db.FA_DiscHes
                                            join sq1 in db.FA_DiscDs on sq.c_nodisc equals sq1.c_nodisc
                                            where (sq.c_type == "01") && (sq1.l_aktif == true) && (sq1.l_status == true)
                                            select new
                                            {
                                                sq1.c_iteno,
                                                sq1.n_discon
                                            }).Distinct() on q1.c_iteno equals q4.c_iteno into sq_4
                                       from qDH in sq_4.DefaultIfEmpty()
                                       join q6 in
                                           (from sq1 in db.LG_FBD1s
                                            join sq2 in db.LG_FBD2s on sq1.c_fbno equals sq2.c_fbno
                                            group new { sq1, sq2 } by new { sq1.c_iteno, sq1.c_type, sq2.c_rnno } into g
                                            select new
                                            {
                                                g.Key.c_rnno,
                                                g.Key.c_iteno,
                                                g.Key.c_type
                                            }).Distinct() on new { q.c_rnno, q1.c_iteno, q1.c_type } equals new { q6.c_rnno, q6.c_iteno, q6.c_type } into sq_6
                                       from qFB in sq_6.DefaultIfEmpty()
                                       where lstTypeRNFB.Contains(q.c_type) && q.c_from == noSup && q.c_rnno == noRcv
                                        && (q.c_gdg == gdg) && (q.l_float == false) && (q.l_status == false)
                                           //&& (qDisD.d_start >= structure.Fields.TanggalSP) || (qDisD.d_finish <= structure.Fields.TanggalSP)
                                         && ((qFB == null) || (qFB.c_iteno == null))
                                       // On Production && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                       group new { q1, qItm, qDH, qTypFBD } by new { q1.c_iteno, q1.c_type, qItm.n_disc, qItm.n_salpri, qItm.v_itnam, qDH.n_discon, qTypFBD.v_ket } into g
                                       select new SCMS_RNFaktur_ITEM_UNION()
                                       {
                                           c_iteno = g.Key.c_iteno,
                                           v_itnam = g.Key.v_itnam,
                                           c_type = g.Key.c_type,
                                           v_type_desc = g.Key.v_ket,
                                           n_bea = 0,
                                           n_disc = (g.Key.c_type == "02" ? 0 : (g.Key.n_discon.HasValue ? g.Key.n_discon.Value :
                                               (g.Key.n_disc.HasValue ? g.Key.n_disc.Value : 0))),
                                           n_qty = g.Sum(x => (x.q1.n_gqty.HasValue ? x.q1.n_gqty.Value : 0)),
                                           n_salpri = (g.Key.c_type == "02" ? 0 : g.Key.n_salpri.Value),
                                           n_ppph = 0,
                                       }).Distinct().AsQueryable();
                            }

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                            lstTypeRNFB.Clear();
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_FB_RN_ITEM_FLOATING

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_FB_RN_ITEM_FLOATING:
                        {
                            string noSup = (parameters.ContainsKey("suplierId") ? (string)((Functionals.ParameterParser)parameters["suplierId"]).Value : string.Empty);

                            #region Old Coded

                            //var qry = (from q in db.LG_RNHs
                            //           join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { c_gdg = (char)q1.c_gdg, q1.c_rnno }
                            //           join q2 in db.LG_RND2s on new { q.c_gdg, q.c_rnno, q1.c_iteno, q1.c_batch } equals new { c_gdg = (char)q2.c_gdg, q2.c_rnno, q2.c_iteno, q2.c_batch }
                            //           join q3 in db.LG_RND4s on new { q.c_gdg, q.c_rnno, q1.c_iteno, q1.c_batch } equals new { c_gdg = (char)q3.c_gdg, q3.c_rnno, q3.c_iteno, q3.c_batch } into q_2
                            //           from qRND4 in q_2.DefaultIfEmpty()
                            //           join q4 in db.FA_MasItms on q1.c_iteno equals q4.c_iteno
                            //           where q.l_float == true && q.c_from == noSup
                            //             && ((qRND4.l_status.HasValue ? qRND4.l_status.Value : false) == false)
                            //             && (q2.c_type == "01")
                            //           group new { q, q1, q2, qRND4, q4 } by new { qRND4.i_urut, q.c_gdg, q.c_rnno, q1.c_iteno, q2.c_type, qRND4.n_salpri, n_salpri_item = q4.n_salpri, qRND4.n_disc, n_disc_item = q4.n_disc, q4.v_itnam, qRND4.n_qty } into g
                            //           select new
                            //           {
                            //             g.Key.c_gdg,
                            //             g.Key.c_rnno,
                            //             g.Key.c_iteno,
                            //             g.Key.c_type,
                            //             n_salpri = (g.Key.n_salpri.HasValue ? g.Key.n_salpri.Value : g.Key.n_salpri_item.Value),
                            //             n_disc = (g.Key.n_disc.HasValue ? g.Key.n_disc.Value : g.Key.n_disc_item),
                            //             n_qty = (g.Key.n_qty.HasValue ? g.Key.n_qty.Value : (g.Sum(x => x.q1.n_gqty) + g.Sum(x => x.q1.n_bqty))),
                            //             g.Key.v_itnam
                            //           }).Distinct().AsQueryable();

                            #endregion

                            var qry = (from q in db.LG_RNHs
                                       join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { c_gdg = (char)q1.c_gdg, q1.c_rnno }
                                       join q2 in db.LG_RND2s on new { q.c_gdg, q.c_rnno, q1.c_iteno, q1.c_batch } equals new { c_gdg = (char)q2.c_gdg, q2.c_rnno, q2.c_iteno, q2.c_batch }
                                       join q4 in db.FA_MasItms on q1.c_iteno equals q4.c_iteno
                                       /*join q5 in db.FA_DiscDs on q1.C_iteno equals q5.C_iteno into q_5
                                       from qDD in q_5.DefaultIfEmpty()
                                       join q6 in db.FA_DiscHes on new { qDD.C_nodisc, C_type = "03" } equals new { q6.C_nodisc, q6.C_type } into q_6
                                       from qDH in q_6.DefaultIfEmpty()
                                       */
                                       join q5 in
                                           (
                                             from sq in db.FA_DiscHes
                                             join sq1 in db.FA_DiscDs on sq.c_nodisc equals sq1.c_nodisc
                                             where (sq.c_type == "01") && (sq1.l_aktif == true) && (sq1.l_status == true)
                                             select new
                                             {
                                                 sq.c_type,
                                                 sq1.c_iteno,
                                                 sq1.n_discon,
                                                 sq1.n_discoff
                                             }
                                             ) on new { q1.c_iteno, c_type = "03" } equals new { q5.c_iteno, q5.c_type } into q_5
                                       from qSQDisD in q_5.DefaultIfEmpty()
                                       where q.l_float == true && q.c_from == noSup
                                           //&& (q2.n_floqty > 0) 
                                         && (q2.c_type == "01")
                                       //group new { q, q1, q2, q4 } by new { q.c_gdg, q.c_rnno, q1.c_iteno, q1.c_batch, q2.c_type, q4.n_salpri, q4.n_disc, q4.v_itnam, qSQDisD.n_discon } into g
                                       group new { q, q2, q4 } by new { q.c_gdg, q2.c_iteno, q4.n_salpri, q4.n_disc, q4.v_itnam, qSQDisD.n_discon } into g
                                       select new
                                       {
                                           g.Key.c_gdg,
                                           //g.Key.c_rnno,
                                           g.Key.c_iteno,
                                           //g.Key.c_batch,
                                           //g.Key.c_type,
                                           n_salpri = (g.Key.n_salpri.HasValue ? g.Key.n_salpri.Value : 0),
                                           n_disc = (g.Key.n_disc.HasValue ? g.Key.n_disc.Value : (g.Key.n_discon.HasValue ? g.Key.n_discon.Value : 0)),
                                           n_qty = g.Sum(x => (x.q2.n_floqty.HasValue ? x.q2.n_floqty.Value : 0)),

                                           //n_qty = g.Sum(x => (x.q1.n_floqty.HasValue ? x.q1.n_floqty.Value : 0)),
                                           g.Key.v_itnam
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_FB_SUPPLIER

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_FB_SUPPLIER:
                        {

                            List<string> lstTipe = new List<string>();
                            lstTipe.Add("01");
                            lstTipe.Add("06");

                            var qry = (from q in db.LG_DatSups
                                       join q1 in db.LG_RNHs on q.c_nosup equals q1.c_from
                                       where lstTipe.Contains(q1.c_type) && q1.l_status == false
                                       && q1.c_rnno.Substring(0, 4) != "RNXX"
                                       select new
                                       {
                                           q.c_nosup,
                                           q.l_aktif,
                                           q.v_nama,
                                           q.l_hide
                                       }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion

                    #region MODEL_COMMON_QUERY_MULTIPLE_SUPLIER_FAKTURMANUAL

                    case Constant.MODEL_COMMON_QUERY_MULTIPLE_SUPLIER_FAKTURMANUAL:
                        {
                            var qry = (from q in db.LG_DatSups
                                       where q.l_aktif == true
                                       select new
                                       {
                                           c_nosup = q.c_nosup,
                                           v_nama = q.v_nama
                                       }).Union
                                        (from q in db.LG_NonDatsups
                                         where q.l_aktif == true
                                         select new
                                         {
                                             c_nosup = q.c_pkpno,
                                             v_nama = q.v_nama
                                         }).Distinct().AsQueryable();

                            if ((parameters != null) && (parameters.Count > 0))
                            {


                                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                                {
                                    if (kvp.Value.IsCondition)
                                    {
                                        if (kvp.Value.IsLike)
                                        {
                                            paternLike = kvp.Value.Value.ToString();
                                            qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                                        }
                                        else if (kvp.Value.IsBetween)
                                        {

                                        }
                                        else
                                        {
                                            qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                                        }
                                    }
                                }
                            }

                            Logger.WriteLine(qry.Provider.ToString());

                            nCount = qry.Count();

                            if (nCount > 0)
                            {
                                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                                {
                                    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                                }

                                if ((limit == -1) || allQuery)
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                                }
                                else
                                {
                                    dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                                }
                            }

                            dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                            dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
                        }
                        break;

                    #endregion
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(
                  "ScmsSoaLibrary.Modules.CommonQueryMulti:ModelGridQueryFinance (First) <-> Switch {0} - {1}", model, ex.Message);
                Logger.WriteLine(ex.StackTrace);

                dic.Add(Constant.DEFAULT_NAMING_EXCEPTION, ex.Message);

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);
            }

            db.Dispose();

            return dic;
        }
    }
}