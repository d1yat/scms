using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsModel;
using ScmsSoaLibrary.Parser;
//using ScmsSoaLibrary.Core.Crypto;
using ScmsSoaLibrary.Commons;
using System.Data.Common;
using ScmsSoaLibrary.Core.Response;
using ScmsSoaLibraryInterface.Commons;
using ScmsSoaLibrary.Modules;

namespace ScmsSoaLibrary.Bussiness
{
    //internal class RS_ConfirmUpdate
    //{
    //  public char Gudang { get; set; }
    //  public string RsId { get; set; }
    //  public DateTime RsDate { get; set; }
    //  public string NipCreateEntry { get; set; }
    //  public DateTime CreateEntryDate { get; set; }
    //  public string NipUpdateEntry { get; set; }
    //  public DateTime UpdateEntryDate { get; set; }
    //  public string Keterangan { get; set; }
    //}

    internal class ReturClassComponent
    {
        public string RefID { get; set; }
        public string SignID { get; set; }
        public string Item { get; set; }
        public decimal good { get; set; }
        public decimal bad { get; set; }
        public string Supplier { get; set; }
        public decimal Box { get; set; }
    }

    internal class ReturCustomerInfo
    {
        public string Ref1 { get; set; }
        public string Ref2 { get; set; }
        public string Item { get; set; }
        public string Batch { get; set; }
        public decimal Dec1 { get; set; }
        public decimal Dec2 { get; set; }
    }

    class ReturPusat
    {
        private string ResponToDisCorePB(ORMDataContext db, ScmsSoaLibrary.Parser.Class.ReturCustomerStructure structure, string returId, DateTime date)
        {
            bool isSuccess = false;

            return ResponToDisCorePB(db, structure, returId, date, true, out isSuccess);
        }

        private string ResponToDisCorePBResend(ORMDataContext db, ScmsSoaLibrary.Parser.Class.ReturCustomerStructureIn structure, string returId, DateTime date, bool directSubmit, bool isSuccess, int loop)
        {
            string rest = null;

            isSuccess = false;

            ScmsSoaLibrary.Parser.Class.ReturCustomerInResponse strt = new ScmsSoaLibrary.Parser.Class.ReturCustomerInResponse();

            strt.ID = structure.Fields.Field[loop].PBId;
            strt.Fields = structure.Fields.Field;
            strt.USER = structure.Fields.Entry;
            if (!string.IsNullOrEmpty(returId))
            {
                strt.RC = returId;
                strt.TanggalRC = date;
                strt.TanggalRC_Str = date.ToString("yyyy-MM-dd");

            }

            Config cfg = Functionals.Configuration;

            string val = ScmsSoaLibrary.Parser.Class.ReturCustomerInResponse.Serialize(strt);

            Dictionary<string, string> dicParam = new Dictionary<string, string>();
            dicParam.Add("params", val);

            Dictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add("X-Requested-With", "XMLHttpRequest");

            SCMS_RESPONSE_OBJECT sro = new SCMS_RESPONSE_OBJECT()
            {
                l_status = false,
                v_contentType = "application/x-www-form-urlencoded; charset=UTF-8",
                v_header = ParserDisCore.HeaderParserEnc(dicHeader),
                v_param = ParserDisCore.ParameterParser(dicParam),
                v_referer = "http://10.100.10.28/dcore/?m=com.ams.trx.pbbpbr",
                v_url = "http://10.100.10.28/dcore/?m=com.ams.json.ds&action=form&f=Submit&_q=trx_rc_trigger"
            };

            db.SCMS_RESPONSE_OBJECTs.InsertOnSubmit(sro);

            ScmsSoaLibrary.Parser.ResponseParser res = new ScmsSoaLibrary.Parser.ResponseParser();

            ScmsSoaLibrary.Core.Response.DiscoreResponse ds = new ScmsSoaLibrary.Core.Response.DiscoreResponse();

            List<SCMS_RESPONSE_OBJECT> lisRes = new List<SCMS_RESPONSE_OBJECT>();

            IDictionary<string, string> dic = new Dictionary<string, string>();

            ScmsSoaLibrary.Parser.ParserDisCore pdc;

            string result = null;
            Uri uri = null;

            Dictionary<string, string> dHeader = ParserDisCore.HeaderParserDec(sro.v_header);
            Dictionary<string, string> dParam = ParserDisCore.ParameterParserDec(sro.v_param);
            Encoding utf8 = Encoding.UTF8;

            uri = Functionals.DistCoreUrlBuilder(cfg, sro.v_url);

            pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

            pdc.Referer = Functionals.DistCoreUrlBuilderString(cfg, sro.v_referer);
            pdc.ContentType = sro.v_contentType;

            dHeader = ParserDisCore.HeaderParserDec(sro.v_header);
            dParam = ParserDisCore.ParameterParserDec(sro.v_param);

            if (pdc.PostGetData(uri, dParam, dHeader))
            {
                result = utf8.GetString(pdc.Result);

                Logger.WriteLine(result, true);

                dic = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCoreSwitch(result);

                if (dic.GetValueParser<string, string, string>("void", string.Empty).Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    rest = dic.GetValueParser<string, string, string>("NewID", string.Empty);// dic["NewID"];

                    if (pdc.PostGetData(uri, dParam, dHeader))
                    {
                        result = utf8.GetString(pdc.Result);

                        Logger.WriteLine(result, true);
                    }
                    else
                    {
                        Logger.WriteLine(pdc.ErrorMessage);
                    }
                }

                sro.l_status = (dic.ContainsKey("success") && (dic["success"] == "1") ? true : false);
            }
            else
            {
                Logger.WriteLine(pdc.ErrorMessage);
            }

            dHeader.Clear();
            dParam.Clear();

            if (directSubmit)
            {
                db.SubmitChanges();
            }

            return rest;
        }

        private string ResponToDisCorePB(ORMDataContext db, ScmsSoaLibrary.Parser.Class.ReturCustomerStructure structure, string returId, DateTime date, bool directSubmit, out bool isSuccess)
        {
            string rest = null;

            isSuccess = false;

            ScmsSoaLibrary.Parser.Class.ReturCustomerResponse strt = new ScmsSoaLibrary.Parser.Class.ReturCustomerResponse();

            strt.ID = structure.Fields.PBID;
            strt.Fields = structure.Fields.Field;
            strt.USER = structure.Fields.USER;
            if (!string.IsNullOrEmpty(returId))
            {
                strt.RC = returId;
                strt.TanggalRC = date;
                strt.TanggalRC_Str = date.ToString("yyyy-MM-dd");

            }

            Config cfg = Functionals.Configuration;

            string val = ScmsSoaLibrary.Parser.Class.ReturCustomerResponse.Serialize(strt);

            Dictionary<string, string> dicParam = new Dictionary<string, string>();
            dicParam.Add("params", val);

            Dictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add("X-Requested-With", "XMLHttpRequest");

            SCMS_RESPONSE_OBJECT sro = new SCMS_RESPONSE_OBJECT()
            {
                l_status = false,
                v_contentType = "application/x-www-form-urlencoded; charset=UTF-8",
                v_header = ParserDisCore.HeaderParserEnc(dicHeader),
                v_param = ParserDisCore.ParameterParser(dicParam),
                v_referer = "http://10.100.10.28/dcore/?m=com.ams.trx.pbbpbr",
                v_url = "http://10.100.10.28/dcore/?m=com.ams.json.ds&action=form&f=Submit&_q=trx_rc_trigger"
            };

            db.SCMS_RESPONSE_OBJECTs.InsertOnSubmit(sro);

            ScmsSoaLibrary.Parser.ResponseParser res = new ScmsSoaLibrary.Parser.ResponseParser();

            ScmsSoaLibrary.Core.Response.DiscoreResponse ds = new ScmsSoaLibrary.Core.Response.DiscoreResponse();

            List<SCMS_RESPONSE_OBJECT> lisRes = new List<SCMS_RESPONSE_OBJECT>();

            IDictionary<string, string> dic = new Dictionary<string, string>();

            ScmsSoaLibrary.Parser.ParserDisCore pdc;

            string result = null;
            Uri uri = null;

            Dictionary<string, string> dHeader = ParserDisCore.HeaderParserDec(sro.v_header);
            Dictionary<string, string> dParam = ParserDisCore.ParameterParserDec(sro.v_param);
            Encoding utf8 = Encoding.UTF8;

            uri = Functionals.DistCoreUrlBuilder(cfg, sro.v_url);

            pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

            pdc.Referer = Functionals.DistCoreUrlBuilderString(cfg, sro.v_referer);
            pdc.ContentType = sro.v_contentType;

            dHeader = ParserDisCore.HeaderParserDec(sro.v_header);
            dParam = ParserDisCore.ParameterParserDec(sro.v_param.Replace("'",""));
            dParam = ParserDisCore.ParameterParserDec(sro.v_param.Replace("&","_"));

            if (pdc.PostGetData(uri, dParam, dHeader))
            {
                result = utf8.GetString(pdc.Result);

                Logger.WriteLine(result, true);

                dic = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCoreSwitch(result);

                if (dic.GetValueParser<string, string, string>("success", string.Empty).Equals("true", StringComparison.OrdinalIgnoreCase)) //suwandi 22 mei 2018
                //if (dic.GetValueParser<string, string, string>("void", string.Empty).Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    rest = dic.GetValueParser<string, string, string>("RSID", string.Empty);// dic["NewID"]; // new by suwandi 20 juni 2018
                    //rest = dic.GetValueParser<string, string, string>("NewID", string.Empty); //old by suwandi 20 juni 2018
                    if (pdc.PostGetData(uri, dParam, dHeader))
                    {
                        result = utf8.GetString(pdc.Result);

                        Logger.WriteLine(result, true);
                    }
                    else
                    {
                        Logger.WriteLine(pdc.ErrorMessage);
                    }
                }
                else if (dic.GetValueParser<string, string, string>("success", string.Empty).Equals("false", StringComparison.OrdinalIgnoreCase)) //suwandi 22 mei 2018
                //else if (dic.GetValueParser<string, string, string>("void", string.Empty).Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    rest = dic.GetValueParser<string, string, string>("errors", string.Empty);
                    //rest = dic.GetValueParser<string, string, string>("RSID", string.Empty);
                }

                sro.l_status = (dic.ContainsKey("success") && (dic["success"] == "1") ? true : false);

                //if (sro.l_status.HasValue && sro.l_status.Value)
                //{
                //  rest = "success";
                //}
            }
            else
            {
                Logger.WriteLine(pdc.ErrorMessage);
            }

            dHeader.Clear();
            dParam.Clear();
            db.SubmitChanges();


            #region Old Coded

            //db.SubmitChanges();

            //ScmsSoaLibrary.Parser.ResponseParser res = new ScmsSoaLibrary.Parser.ResponseParser();

            //ScmsSoaLibrary.Core.Response.DiscoreResponse ds = new ScmsSoaLibrary.Core.Response.DiscoreResponse();

            //List<SCMS_RESPONSE_OBJECT> lisRes = new List<SCMS_RESPONSE_OBJECT>();

            //IDictionary<string, string> dic = new Dictionary<string, string>();

            //lisRes = (from q in db.SCMS_RESPONSE_OBJECTs
            //          where q.l_status == false
            //          select q).Distinct().ToList();

            //int nLoop = 0;

            //Uri uri = null;

            //ScmsSoaLibrary.Parser.ParserDisCore pdc;

            //string result = null;

            //Dictionary<string, string> dHeader = ParserDisCore.HeaderParserDec(lisRes[nLoop].v_header);
            //Dictionary<string, string> dParam = ParserDisCore.ParameterParserDec(lisRes[nLoop].v_param);
            //Encoding utf8 = Encoding.UTF8;
            ////ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.IsSuccess;
            ////ScmsSoaLibraryInterface.Components.PostDataParser.StructureResponseNext test = default(ScmsSoaLibraryInterface.Components.PostDataParser.StructureResponseNext);

            //for (nLoop = 0; nLoop < lisRes.Count; nLoop++)
            //{
            //  //uri = new Uri(lisRes[nLoop].v_url.ToString());
            //  uri = Functionals.DistCoreUrlBuilder(cfg, lisRes[nLoop].v_url);

            //  pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

            //  pdc.Referer = Functionals.DistCoreUrlBuilderString(cfg, lisRes[nLoop].v_referer);
            //  pdc.ContentType = lisRes[nLoop].v_contentType;

            //  dHeader = ParserDisCore.HeaderParserDec(lisRes[nLoop].v_header);
            //  dParam = ParserDisCore.ParameterParserDec(lisRes[nLoop].v_param);
            //  //rpe = ResponseParser.ResponseParserEnum.IsSuccess;

            //  if (pdc.PostGetData(uri, dParam, dHeader))
            //  {
            //    result = utf8.GetString(pdc.Result);

            //    Logger.WriteLine(result, true);

            //    dic = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCoreSwitch(result);

            //    //test = new ScmsSoaLibraryInterface.Components.PostDataParser.StructureResponseNext(); ;

            //    if (dic["void"] == "true")
            //    {
            //      rest = dic["NewID"];

            //      //Dictionary<string, string> NewParam = new Dictionary<string, string>();

            //      if (pdc.PostGetData(uri, dParam, dHeader))
            //      {
            //        result = utf8.GetString(pdc.Result);

            //        Logger.WriteLine(result, true);
            //      }
            //      else
            //      {
            //        Logger.WriteLine(pdc.ErrorMessage);
            //      }
            //    }
            //    if (dic["success"] == "1")
            //    {
            //      lisRes[nLoop].l_status = true;
            //    }
            //  }
            //  else
            //  {
            //    Logger.WriteLine(pdc.ErrorMessage);
            //  }

            //  dHeader.Clear();
            //  dParam.Clear();
            //}

            #endregion


            //if (directSubmit)
            //{
            //    db.SubmitChanges();
            //}

            return rest;
        }

        private bool ResponToDisCorePBReturn(ORMDataContext db, ScmsSoaLibrary.Parser.Class.ReturCustomerStructure structure, string returId, DateTime date)
        {
            string resultData = null;
            bool bResult = false;

            try
            {
                resultData = ResponToDisCorePB(db, structure, returId, date, false, out bResult);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
                Logger.WriteLine(ex.StackTrace);
            }

            return bResult;
        }

        private string ResponToDisCoreTipePB(ORMDataContext db, ScmsSoaLibrary.Parser.Class.ReturCustomerStructure structure)
        {
            Encoding utf8 = Encoding.UTF8;
            Config cfg = Functionals.Configuration;

            string rest = null;

            Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.40/dcore/?m=com.ams.json.ds&action=form&f=Business&open=trx_pb");

            Dictionary<string, string> param = new Dictionary<string, string>();

            param.Add("C_PBNO", structure.Fields.PBID);
            param.Add("C_KODECABOLD", structure.Fields.Customer);
            param.Add("C_STATUS", "P");

            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("X-Requested-With", "XMLHttpRequest");

            bool getSuccess = false;

            string result = null;
            Dictionary<string, object> dicHeader = null;
            List<Dictionary<string, string>> list = null;
            Dictionary<string, string> dataRow = null;

            ScmsSoaLibrary.Parser.ParserDisCore pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

            pdc.Referer = Functionals.DistCoreUrlBuilderString(cfg, "http://10.100.10.40/dcore/?m=com.ams.trx.pbbpbr");
            pdc.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            if (pdc.PostGetData(uri, param, header))
            {
                result = utf8.GetString(pdc.Result);

                Logger.WriteLine(result);

                dicHeader = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCore(result);

                if ((dicHeader != null) && dicHeader.ContainsKey(Constant.DEFAULT_NAMING_SUCCESS))
                {
                    getSuccess = (bool)dicHeader[Constant.DEFAULT_NAMING_SUCCESS];

                    if (getSuccess)
                    {
                        list = dicHeader[Constant.DEFAULT_NAMING_RECORDS] as List<Dictionary<string, string>>;

                        if (list.Count > 0)
                        {
                            dataRow = list[0];
                            rest = dataRow["C_EXSTOCK"].ToString();
                        }
                    }
                }
            }
            else
            {
                result = pdc.ErrorMessage;

                Logger.WriteLine(result);
            }

            return rest;
        }

        public string ReturCustomer(ScmsSoaLibrary.Parser.Class.ReturCustomerStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            ScmsSoaLibrary.Parser.Class.ReturCustomerStructureField field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now;

            //decimal nQtyAlocate = 0;

            IDictionary<string, string> dic = null;

            char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '1' : structure.Fields.Gudang[0]);
            string emailksl = null, emailksa = null, emailkca = null;
            string namabarang = null;

            string result = null,
              OldPBBR = null,
              TypePB = null;
            string nipEntry = null,
              RnTmp = null;
            string rcID = null,
                //tmpNumbering = null,
              pinCode = null,
              cusNam = null,
              suplId = null,
              spID = null,
              memoID = null;
            int nLoop,
              nLoopC;
            decimal nQtyAlloc = 0,
              nQtyReceived = 0;
            bool isResponse = false,
              isGood = false,
              isReTry = false,
              isSent = false;

            string NewNoPBB = null,ket = null, cusid = null, bodymail = null, error = null;

            LG_RCD1 rcd1 = null;
            LG_RCD2 rcd2 = null;
            LG_RCD3 rcd3 = null;
            LG_RCD5 rcd5 = null;
            LG_MEMO_RCH memoh = null; //suwandi 16 mei 2018
            LG_MEMO_RCD memod = null;
            List<LG_MEMO_RCD> listmemod = null;
            List<LG_RCD1> listRCD1 = null;
            List<LG_RCD2> listRCD2 = null;
            List<LG_RCD3> listRCD3 = null;
            List<LG_RCD5> listRCD5 = null;
            //List<LG_RCD4> listRCD4 = null;
            //List<string> ListStringRCD3 = null;
            LG_RND1 rnd1 = null;
            List<LG_RND1> listRND1 = null;
            List<LG_RND1> listSumRND1 = null;
            //List<LG_RND1> UListRND1 = null;
            //LG_RND1 listDataRND1 = null;
            //List<object> listDataO = null;
            LG_RCH rch = null;
            LG_RCH rchCheck = null;
            List<LG_RNH> listRNH = null;
            //FA_MasItm masitm = null;
            LG_RNH rnh = null;
            LG_ComboH comboh = null;
            LG_DOD1 dod1 = null;

            FA_MasItm msitm = null;

            List<ReturCustomerInfo> lstDOPLInfo = null;
            List<ReturCustomerInfo> lstRCItemDiscInfo = null;
            ReturCustomerInfo rciDOPL = null;
            //List<LG_PLD2> listPLD2 = null;
            LG_PLD2 pld2 = null;
            LG_STD2 std2 = null;
            
            int totalDetails = 0;

            Dictionary<string, string> dicItemSupl = null;

            List<string> lstGudang = new List<string>();
            lstGudang.Add("1");
            lstGudang.Add("2");

            #region Authorize Nip Entrys

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            #endregion

            rcID = (structure.Fields.ReturCustomerID ?? string.Empty);

            const string SPDefault = "SPXXXXXXXX";
            const string DODefault = "DOXXXXXXXX";

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
                {
                    #region Add

                    #region otorisasi Add

                    if (!string.IsNullOrEmpty(rcID))
                    {
                        result = "Nomor Retur Customer harus kosong.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    //else if (string.IsNullOrEmpty(structure.Fields.Customer))
                    //{
                    //    result = "Nama cabang dibutuhkan.";

                    //    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //    if (db.Transaction != null)
                    //    {
                    //        db.Transaction.Rollback();
                    //    }

                    //    goto endLogic;
                    //}
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
                    else if (!string.IsNullOrEmpty(structure.Fields.PBID))
                    {
                        rchCheck = new LG_RCH();
                        rchCheck = (from q in db.LG_RCHes
                                    where q.v_pbbrno == structure.Fields.PBID
                                    select q).Take(1).SingleOrDefault();

                        if (rchCheck != null)
                        {
                            result = "No.PBB/PBR sudah ada.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                    }
                    //Cek Void
                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];
                            if (field.QuantityReceive != (field.Acceptance + field.Destroy))
                            {
                                //rpe = ResponseParser.ResponseParserEnum.IsFailed;
                                namabarang = (from q in db.FA_MasItms 
                                                  where q.c_iteno == field.Item && q.l_aktif == true
                                                  select q.v_itnam).SingleOrDefault();
                                isResponse = true;
                                bodymail = bodymail + field.Item + " - " + namabarang + " Qty Barang di dokumen = " + field.QuantityReceive + " , Qty Barang diterima = " + (field.Acceptance + field.Destroy);
                            }
                        }
                    }

                    #region Check Response

                Reponse:

                    if (structure.Fields.PBID.ToUpper().Substring(0, 3).ToString() == "PBB" || structure.Fields.PBID.ToUpper().Substring(0, 3).ToString() == "TBB")
                    {
                        TypePB = "01";
                        isGood = true;
                    }
                    else
                    {
                        TypePB = "02";
                    }

                    #endregion

                    #endregion

                    //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "RC");
                    rcID = Commons.GenerateNumbering<LG_RCH>(db, "RC", '3', "11", date, "c_rcno");
                    pinCode = Functionals.GeneratedRandomPinId(4, string.Empty);

                    #region Insert Memo
                    //Suwandi 16 Mei 2018
                    //if (isResponse == true)
                    //{
                    //    cusid = (from q in db.LG_CusmasCabs
                    //             where q.c_cab == structure.Fields.Customer
                    //             select q.c_cusno).SingleOrDefault();
                    //    memoID = "MR" + rcID.Substring(2, 8);
                    //    memoh = new LG_MEMO_RCH()
                    //    {
                    //        id_memono = memoID,
                    //        c_cusno = cusid,
                    //        c_entry = structure.Fields.Entry,
                    //        c_update = structure.Fields.Entry,
                    //        d_entry = DateTime.Now,
                    //        d_update = DateTime.Now,
                    //        attribute1 = null
                    //    };
                    //    db.LG_MEMO_RCHes.InsertOnSubmit(memoh);
                    //    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    //    {
                    //        field = structure.Fields.Field[nLoop];
                    //        if (field.QuantityReceive != (field.Acceptance + field.Destroy))
                    //        {

                    //            if (field.QuantityReceive < (field.Acceptance + field.Destroy))
                    //            {
                    //                ket = "Barang diterima Lebih";
                    //            }
                    //            else if (field.QuantityReceive > (field.Acceptance + field.Destroy))
                    //            {
                    //                ket = "Barang diterima Kurang";
                    //            }
                    //            //rpe = ResponseParser.ResponseParserEnum.IsFailed;
                    //            memod = new LG_MEMO_RCD()
                    //            {
                    //                id_memono = memoID,
                    //                c_iteno = field.Item,
                    //                c_batch = field.Batch,
                    //                n_qtycabang = field.QuantityReceive,
                    //                n_qtyterima = field.Acceptance,
                    //                attribute1 = structure.Fields.PBID,
                    //                v_ket = ket
                    //            };
                    //            db.LG_MEMO_RCDs.InsertOnSubmit(memod);

                    //        }
                    //    }
                    //}

                    //Random rand = new Random();
                    //double doub = rand.NextDouble();
                    //double tes = (1 - 9999999999 - 1) * doub + 9999999999;
                    //pinCode = Math.Round(tes, 0).ToString();
                    #endregion

                    #region Insert Header

                    var varCus = (from q in db.LG_CusmasCabs
                                  where q.c_cab_dcore == structure.Fields.PBID.Substring(3,3)
                                  select q).Take(1).SingleOrDefault();

                    cusNam = varCus.v_cunam;

                    rch = new LG_RCH()
                    {
                        c_cusno = varCus.c_cusno,
                        c_entry = nipEntry,
                        c_update = nipEntry,
                        d_entry = DateTime.Now,
                        d_update = DateTime.Now,
                        d_rcdate = date,
                        c_gdg = gudang,
                        c_pin = pinCode,
                        c_rcno = rcID,
                        l_delete = false,
                        l_send = false,
                        l_status = false,
                        v_ket = structure.Fields.Keterangan,
                        d_send = DateTime.Now,
                        v_ket_mark = null,
                        c_pbbrno = TypePB,
                        v_pbbrno = structure.Fields.PBID,
                        v_oldpbbrno = structure.Fields.PBIDOLD ?? OldPBBR
                    };

                    db.LG_RCHes.InsertOnSubmit(rch);

                    #region Old Coded

                    //db.SubmitChanges();

                    //rch = (from q in db.LG_RCHes
                    //       where q.v_ket == tmpNumbering
                    //       select q).Take(1).SingleOrDefault();

                    //if (!string.IsNullOrEmpty(rcID))
                    //{
                    //  result = "Nomor Retur Customer tidak dapat di raih.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}
                    //else if (rch.c_rcno.Equals("XXXXXXXXXX"))
                    //{
                    //  result = "Trigger Retur Customer tidak aktif.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}

                    //rch.v_ket = structure.Fields.Keterangan;

                    //rcID = rch.c_rcno;

                    #endregion

                    #endregion

                    #region Insert Detail

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        listRCD1 = new List<LG_RCD1>();
                        listRCD2 = new List<LG_RCD2>();
                        listRCD3 = new List<LG_RCD3>();
                        listRCD5 = new List<LG_RCD5>();

                        listRNH = new List<LG_RNH>();
                        listRND1 = new List<LG_RND1>();
                        listSumRND1 = new List<LG_RND1>();

                        listRND1 = new List<LG_RND1>();
                        dicItemSupl = new Dictionary<string, string>();
                        lstRCItemDiscInfo = new List<ReturCustomerInfo>();

                        ReturCustomerInfo rcDiscInf = new ReturCustomerInfo();

                        msitm = new FA_MasItm();

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            field.Outlet = field.Outlet.Replace('&', '-');
                            field.Batch = field.Batch.Trim();

                            rcDiscInf = (from q in db.FA_MasItms
                                         join q1 in
                                             (from sq in db.FA_DiscHes
                                              join sq1 in db.FA_DiscDs on sq.c_nodisc equals sq1.c_nodisc
                                              where (sq.c_type == "03") && (sq1.c_iteno == field.Item)
                                              select sq1) on q.c_iteno equals q1.c_iteno into q_1
                                         from qDisc in q_1.DefaultIfEmpty()
                                         where (q.c_iteno == field.Item)
                                         select new ReturCustomerInfo()
                                         {
                                             Item = field.Item,
                                             Dec1 = (q.n_salpri.HasValue ? q.n_salpri.Value : 0),
                                             Dec2 = (qDisc == null ? 0 : (qDisc.n_discon.HasValue ? qDisc.n_discon.Value : 0))
                                         }).Take(1).SingleOrDefault();


                            field.n_disc = rcDiscInf.Dec2;
                            field.n_salpri = rcDiscInf.Dec1;

                            field.Type = TypePB;

                            nQtyAlloc = (field.Acceptance + field.Destroy);

                            field.Quantity = nQtyAlloc;

                            if (nQtyAlloc <= 0)
                            {
                                continue;
                            }

                            nQtyReceived = nQtyAlloc;

                            //validasi DO tembakan dari dcore  
                            dod1 = (from q in db.LG_DOD1s
                                    where q.c_dono == field.NoDO && q.c_iteno == field.Item
                                    select q).SingleOrDefault();

                            if (dod1 == null || field.NoDO.Length < 10)
                            {
                                field.NoDO = DODefault;
                            }

                        doNotExists:
                            if (string.IsNullOrEmpty(field.NoDO) || field.NoDO.StartsWith("DOX", StringComparison.OrdinalIgnoreCase) || isReTry)
                            {
                                isReTry = false;

                                field.NoDO = (string.IsNullOrEmpty(field.NoDO) ? DODefault : field.NoDO);

                                spID = SPDefault;

                                #region RN

                                #region Cek RN Header

                                if (dicItemSupl.ContainsKey(field.Item))
                                {
                                    suplId = dicItemSupl[field.Item];
                                }
                                else
                                {
                                    suplId = db.FA_MasItms.Where(x => x.c_iteno == field.Item).Select(y => y.c_nosup).Take(1).SingleOrDefault();

                                    if (string.IsNullOrEmpty(suplId))
                                    {
                                        suplId = "00000";
                                    }
                                }

                                RnTmp = string.Concat("RNXX", gudang, suplId);

                                if (!listRNH.Exists(delegate(LG_RNH rn)
                                {
                                    return rn.c_gdg.Equals(gudang) &&
                                      RnTmp.Equals((string.IsNullOrEmpty(rn.c_rnno) ? string.Empty : rn.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                      suplId.Equals((string.IsNullOrEmpty(rn.c_from) ? string.Empty : rn.c_from.Trim()), StringComparison.OrdinalIgnoreCase);
                                }))
                                {
                                    rnh = (from q in db.LG_RNHs
                                           where (q.c_gdg == gudang) && (q.c_rnno == RnTmp) &&
                                            (q.c_from == suplId)
                                           select q).Take(1).SingleOrDefault();

                                    if (rnh == null)
                                    {
                                        rnh = new LG_RNH()
                                        {
                                            c_gdg = gudang,
                                            c_rnno = RnTmp,
                                            d_rndate = date,
                                            c_type = "01",
                                            l_float = false,
                                            c_dono = "DOXXXXXXXX",
                                            d_dodate = date,
                                            v_ket = Constant.SYSTEM_USERNAME,
                                            c_from = suplId,
                                            n_bea = 0,
                                            l_print = true,
                                            l_status = true,
                                            c_entry = Constant.SYSTEM_USERNAME,
                                            d_entry = date,
                                            c_update = Constant.SYSTEM_USERNAME,
                                            d_update = date,
                                            l_delete = false,
                                            l_khusus = false,
                                        };

                                        db.LG_RNHs.InsertOnSubmit(rnh);
                                    }

                                    listRNH.Add(rnh);
                                }

                                #endregion

                                #region RND1

                                rnd1 = listRND1.Find(delegate(LG_RND1 rnd)
                                {
                                    return gudang.Equals(rnd.c_gdg) &&
                                      RnTmp.Equals((string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                      field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                      field.batchterima.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                });

                                if (rnd1 == null)
                                {
                                    rnd1 = (from q in db.LG_RND1s
                                            where (q.c_gdg == gudang) && (q.c_rnno == RnTmp)
                                              && (q.c_iteno == field.Item) && (q.c_batch == field.batchterima)
                                            select q).Take(1).SingleOrDefault();
                                }

                                if (rnd1 == null)
                                {
                                    rnd1 = new LG_RND1()
                                    {
                                        c_gdg = gudang,
                                        c_rnno = RnTmp,
                                        c_iteno = field.Item,
                                        c_batch = field.batchterima,
                                        //n_floqty = 0,
                                        //n_gsisa = 0,
                                        //n_bsisa = 0,
                                        n_gsisa = (isGood ? field.Acceptance : 0),
                                        n_bsisa = (isGood ? 0 : field.Acceptance),
                                        n_gqty = 0,
                                        n_bqty = 0,
                                    };

                                    //db.LG_RND1s.InsertOnSubmit(rnd1);

                                    Commons.CheckAndProcessBatch(db, field.Item, field.batchterima, date, nipEntry);

                                    listRND1.Add(rnd1);
                                }
                                else
                                {
                                    rnd1.n_gsisa += (isGood ? field.Acceptance : 0);
                                    rnd1.n_bsisa += (isGood ? 0 : field.Acceptance);
                                }
                                #endregion

                                #endregion

                                #region RCD1

                                rcd1 = listRCD1.Find(delegate(LG_RCD1 rcd)
                                {
                                    return gudang.Equals(rcd.c_gdg) &&
                                      RnTmp.Equals(rcd.c_rnno, StringComparison.OrdinalIgnoreCase) &&
                                      field.Item.Equals(rcd.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                                      field.Batch.Equals(rcd.c_batch, StringComparison.OrdinalIgnoreCase) &&
                                      field.NoDO.Equals(rcd.c_dono, StringComparison.OrdinalIgnoreCase);
                                });

                                if (rcd1 == null)
                                {
                                    rcd1 = new LG_RCD1()
                                    {
                                        c_gdg = gudang,
                                        c_rcno = rcID,
                                        c_iteno = field.Item,
                                        c_batch = field.batchterima,
                                        c_type = TypePB,
                                        c_dono = field.NoDO,
                                        c_rnno = RnTmp,
                                        n_qty = 0,
                                        n_qtyrcv = 0,
                                        n_qtydestroy = 0,
                                        n_sisa = 0,
                                        v_outlet = field.Outlet,
                                        c_typeoutlet = field.TipeOutlet,
                                        c_batchterima = field.Batch,
                                    };

                                    listRCD1.Add(rcd1);
                                }
                                
                                rcd1.n_qty = rcd1.n_sisa += field.Acceptance;
                                rcd1.n_qtyrcv += nQtyReceived;
                                rcd1.n_qtydestroy += field.Destroy;
                                
                                #endregion

                                #region RCD2

                                rcd2 = listRCD2.Find(delegate(LG_RCD2 rcd)
                                {
                                    return gudang.Equals(rcd.c_gdg) &&
                                      RnTmp.Equals(rcd.c_rnno, StringComparison.OrdinalIgnoreCase) &&
                                      field.Item.Equals(rcd.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                                      field.batchterima.Equals(rcd.c_batch, StringComparison.OrdinalIgnoreCase) &&
                                      field.NoDO.Equals(rcd.c_dono, StringComparison.OrdinalIgnoreCase) &&
                                      spID.Equals(rcd.c_spno, StringComparison.OrdinalIgnoreCase);
                                });

                                if (rcd2 == null)
                                {
                                    rcd2 = new LG_RCD2()
                                    {
                                        c_gdg = gudang,
                                        c_rcno = rcID,
                                        c_iteno = field.Item,
                                        c_batch = field.batchterima,
                                        c_type = TypePB,
                                        c_dono = field.NoDO,
                                        c_rnno = RnTmp,
                                        n_qty = 0,
                                        n_sisa = 0,
                                        c_spno = spID
                                    };

                                    listRCD2.Add(rcd2);
                                }

                                rcd2.n_qty = rcd2.n_sisa += field.Acceptance;

                                #endregion

                                #region RCD3

                                rcd3 = listRCD3.Find(delegate(LG_RCD3 rcd)
                                {
                                    return gudang.Equals(rcd.c_gdg) &&
                                      field.Item.Equals(rcd.c_iteno, StringComparison.OrdinalIgnoreCase);
                                });

                                if (rcd3 == null)
                                {
                                    rciDOPL = lstRCItemDiscInfo.Find(delegate(ReturCustomerInfo rci)
                                    {
                                        return field.Item.Equals((string.IsNullOrEmpty(rci.Item) ? string.Empty : rci.Item.Trim()), StringComparison.OrdinalIgnoreCase);
                                    });

                                    if (rciDOPL == null)
                                    {
                                        rciDOPL = (from q in db.FA_MasItms
                                                   join q1 in
                                                       (from sq in db.FA_DiscHes
                                                        join sq1 in db.FA_DiscDs on sq.c_nodisc equals sq1.c_nodisc
                                                        where (sq.c_type == "03") && (sq1.c_iteno == field.Item)
                                                        select sq1) on q.c_iteno equals q1.c_iteno into q_1
                                                   from qDisc in q_1.DefaultIfEmpty()
                                                   where (q.c_iteno == field.Item)
                                                   select new ReturCustomerInfo()
                                                   {
                                                       Item = field.Item,
                                                       Dec1 = (q.n_salpri.HasValue ? q.n_salpri.Value : 0),
                                                       Dec2 = (qDisc == null ? 0 : (qDisc.n_discon.HasValue ? qDisc.n_discon.Value : 0))
                                                   }).Take(1).SingleOrDefault();

                                        lstRCItemDiscInfo.Add(rciDOPL);


                                    }

                                    if (rciDOPL == null)
                                    {
                                        rcd3 = new LG_RCD3()
                                        {
                                            c_gdg = gudang,
                                            c_rcno = rcID,
                                            c_iteno = field.Item,
                                            n_salpri = 0,
                                            n_disc = 0
                                        };
                                    }
                                    else
                                    {
                                        rcd3 = new LG_RCD3()
                                        {
                                            c_gdg = gudang,
                                            c_rcno = rcID,
                                            c_iteno = field.Item,
                                            n_salpri = rciDOPL.Dec1,
                                            n_disc = rciDOPL.Dec2
                                        };
                                    }

                                    listRCD3.Add(rcd3);
                                }

                                #endregion

                                totalDetails++;
                            }
                            else
                            {
                                lstDOPLInfo = (from q in db.LG_DOD1s
                                               join q1 in db.LG_DOHs on q.c_dono equals q1.c_dono
                                               where (q.c_dono == field.NoDO)
                                                && (q.c_iteno == field.Item)
                                                && ((q1.l_delete.HasValue ? q1.l_delete.Value : false) == false)
                                               group q by new { q1.c_dono, q1.c_plno, q.c_iteno } into g
                                               select new ReturCustomerInfo()
                                               {
                                                   Ref1 = g.Key.c_dono,
                                                   Ref2 = g.Key.c_plno,
                                                   Item = g.Key.c_iteno,
                                               }).ToList();

                                if ((lstDOPLInfo != null) && (lstDOPLInfo.Count > 0))
                                {
                                    #region DO Exists

                                    for (nLoopC = 0; nLoopC < lstDOPLInfo.Count; nLoopC++)
                                    {
                                        rciDOPL = lstDOPLInfo[nLoopC];

                                        pld2 = (from q in db.LG_PLHs
                                                join q1 in db.LG_PLD2s on q.c_plno equals q1.c_plno
                                                where (q.c_plno == rciDOPL.Ref2) && (q1.c_iteno == rciDOPL.Item)
                                                  && (q1.c_batch == field.batchterima)
                                                  && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                                select q1).Take(1).SingleOrDefault();

                                        std2 = (from q in db.LG_STHs
                                                join q1 in db.LG_STD2s on q.c_stno equals q1.c_stno
                                                where (q.c_stno == rciDOPL.Ref2) && (q1.c_iteno == rciDOPL.Item)
                                                  && (q1.c_batch == field.batchterima)
                                                  && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                                select q1).Take(1).SingleOrDefault();

                                        //  std2 = ListSTD2.Find(delegate(LG_STD2 std)
                                        //{
                                        //    return field.NoRN.Equals((string.IsNullOrEmpty(std.c_no) ? string.Empty : std.c_no.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                        //      field.Batch.Equals((string.IsNullOrEmpty(std.c_batch) ? string.Empty : std.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                        //      field.Item.Equals((string.IsNullOrEmpty(std.c_iteno) ? string.Empty : std.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase);
                                        //});

                                        if (pld2 != null || std2 != null)
                                        {
                                            if (pld2 != null)
                                            {
                                                spID = (string.IsNullOrEmpty(pld2.c_spno) ? SPDefault : pld2.c_spno.Trim());
                                            }
                                            else
                                            {
                                                spID = "SPXXXXXXXX";
                                            }


                                            if (pld2 != null)
                                            {
                                                RnTmp = (string.IsNullOrEmpty(pld2.c_rnno) ? string.Empty : pld2.c_rnno.Trim());
                                            }
                                            else
                                            {
                                                RnTmp = (string.IsNullOrEmpty(std2.c_no) ? string.Empty : std2.c_no.Trim());
                                            }

                                            if (RnTmp.StartsWith("RN", StringComparison.OrdinalIgnoreCase) || RnTmp.StartsWith("RR", StringComparison.OrdinalIgnoreCase))
                                            {
                                                #region RN

                                                rnd1 = listRND1.Find(delegate(LG_RND1 rnd)
                                                {
                                                    return gudang.Equals(rnd.c_gdg) &&
                                                      RnTmp.Equals((string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                      field.batchterima.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                                });

                                                if (rnd1 == null)
                                                {
                                                    rnd1 = (from q in db.LG_RND1s
                                                            where (q.c_gdg == gudang) && (q.c_rnno == RnTmp)
                                                              && (q.c_iteno == field.Item) && (q.c_batch == field.batchterima)
                                                            select q).Take(1).SingleOrDefault();
                                                }

                                                if (rnd1 != null)
                                                {
                                                    //listRND1.Add(rnd1);

                                                    rnd1.n_gsisa += (isGood ? field.Acceptance : 0);
                                                    rnd1.n_bsisa += (isGood ? 0 : field.Acceptance);
                                                }
                                                else
                                                {
                                                    #region Cek RN Header
                                                    if (dicItemSupl.ContainsKey(field.Item))
                                                    {
                                                        suplId = dicItemSupl[field.Item];
                                                    }
                                                    else
                                                    {
                                                        suplId = db.FA_MasItms.Where(x => x.c_iteno == field.Item).Select(y => y.c_nosup).Take(1).SingleOrDefault();

                                                        if (string.IsNullOrEmpty(suplId))
                                                        {
                                                            suplId = "00000";
                                                        }
                                                    }

                                                    if (RnTmp.Contains("RNXX") && lstGudang.Contains(gudang.ToString()))
                                                    {
                                                        RnTmp = string.Concat("RNXX", gudang, suplId);
                                                    }

                                                    if (!listRNH.Exists(delegate(LG_RNH rn)
                                                    {
                                                        return rn.c_gdg.Equals(gudang) &&
                                                          RnTmp.Equals((string.IsNullOrEmpty(rn.c_rnno) ? string.Empty : rn.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                                          suplId.Equals((string.IsNullOrEmpty(rn.c_from) ? string.Empty : rn.c_from.Trim()), StringComparison.OrdinalIgnoreCase);
                                                    }))
                                                    {
                                                        //if (dicItemSupl.ContainsKey(field.Item))
                                                        //{
                                                        //    suplId = dicItemSupl[field.Item];
                                                        //}
                                                        //else
                                                        //{
                                                        //    suplId = db.FA_MasItms.Where(x => x.c_iteno == field.Item).Select(y => y.c_nosup).Take(1).SingleOrDefault();

                                                        //    if (string.IsNullOrEmpty(suplId))
                                                        //    {
                                                        //        suplId = "00000";
                                                        //    }
                                                        //}
                                                        //Lupaaa 10 April 2015
                                                        RnTmp = string.Concat("RNXX", gudang, suplId);

                                                        rnh = (from q in db.LG_RNHs
                                                               where (q.c_gdg == gudang) && (q.c_rnno == RnTmp) &&
                                                                (q.c_from == suplId)
                                                               select q).Take(1).SingleOrDefault();

                                                        if (rnh == null)
                                                        {
                                                            rnh = new LG_RNH()
                                                            {
                                                                c_gdg = gudang,
                                                                c_rnno = RnTmp,
                                                                d_rndate = date,
                                                                c_type = "01",
                                                                l_float = false,
                                                                c_dono = DODefault,
                                                                d_dodate = date,
                                                                v_ket = Constant.SYSTEM_USERNAME,
                                                                c_from = suplId,
                                                                n_bea = 0,
                                                                l_print = true,
                                                                l_status = true,
                                                                c_entry = Constant.SYSTEM_USERNAME,
                                                                d_entry = date,
                                                                c_update = Constant.SYSTEM_USERNAME,
                                                                d_update = date,
                                                                l_delete = false,
                                                                l_khusus = false,
                                                            };

                                                            db.LG_RNHs.InsertOnSubmit(rnh);
                                                        }
                                                        listRNH.Add(rnh);
                                                    }

                                                    #endregion

                                                    #region RND1
                                                    rnd1 = (from q in db.LG_RND1s
                                                            where (q.c_gdg == gudang) && (q.c_rnno == RnTmp)
                                                              && (q.c_iteno == field.Item) && (q.c_batch == field.batchterima)
                                                            select q).Take(1).SingleOrDefault();

                                                    if (rnd1 == null)
                                                    {
                                                        rnd1 = new LG_RND1()
                                                        {
                                                            c_gdg = gudang,
                                                            c_rnno = RnTmp,
                                                            c_iteno = field.Item,
                                                            c_batch = field.batchterima,
                                                            //n_floqty = 0,
                                                            n_gsisa = (isGood ? field.Acceptance : 0),
                                                            n_bsisa = (isGood ? 0 : field.Acceptance),
                                                            n_gqty = 0,
                                                            n_bqty = 0,
                                                        };

                                                        Commons.CheckAndProcessBatch(db, field.Item, field.Batch, date, nipEntry);

                                                        listRND1.Add(rnd1);

                                                        //db.LG_RND1s.InsertOnSubmit(rnd1);
                                                    }
                                                    else
                                                    {
                                                        rnd1.n_gsisa += (isGood ? field.Acceptance : 0);
                                                        rnd1.n_bsisa += (isGood ? 0 : field.Acceptance);
                                                    }

                                                    #endregion
                                                }

                                                #endregion
                                            }
                                            else
                                            {
                                                comboh = (from q in db.LG_ComboHs
                                                          where (q.c_gdg == gudang) && (q.c_combono == RnTmp)
                                                            && (q.c_iteno == field.Item) && (q.c_batch == field.batchterima)
                                                          select q).Take(1).SingleOrDefault();

                                                if (comboh != null)
                                                {
                                                    comboh.n_gsisa += (isGood ? field.Acceptance : 0);
                                                    comboh.n_bsisa += (isGood ? 0 : field.Acceptance);
                                                }
                                                //Case jika DO adalah transaksi combo tetapi tidak ada di gudang yg bersangkutan maka dimasukan ke RN
                                                else
                                                {
                                                    if (dicItemSupl.ContainsKey(field.Item))
                                                    {
                                                        suplId = dicItemSupl[field.Item];
                                                    }
                                                    else
                                                    {
                                                        suplId = db.FA_MasItms.Where(x => x.c_iteno == field.Item).Select(y => y.c_nosup).Take(1).SingleOrDefault();

                                                        if (string.IsNullOrEmpty(suplId))
                                                        {
                                                            suplId = "00000";
                                                        }
                                                    }

                                                    RnTmp = string.Concat("RNXX", gudang, suplId);

                                                    rnh = (from q in db.LG_RNHs
                                                           where (q.c_gdg == gudang) && (q.c_rnno == RnTmp) &&
                                                            (q.c_from == suplId)
                                                           select q).Take(1).SingleOrDefault();

                                                    if (rnh == null)
                                                    {
                                                        rnh = new LG_RNH()
                                                        {
                                                            c_gdg = gudang,
                                                            c_rnno = RnTmp,
                                                            d_rndate = date,
                                                            c_type = "01",
                                                            l_float = false,
                                                            c_dono = DODefault,
                                                            d_dodate = date,
                                                            v_ket = Constant.SYSTEM_USERNAME,
                                                            c_from = suplId,
                                                            n_bea = 0,
                                                            l_print = true,
                                                            l_status = true,
                                                            c_entry = Constant.SYSTEM_USERNAME,
                                                            d_entry = date,
                                                            c_update = Constant.SYSTEM_USERNAME,
                                                            d_update = date,
                                                            l_delete = false,
                                                            l_khusus = false,
                                                        };

                                                        db.LG_RNHs.InsertOnSubmit(rnh);
                                                    }

                                                    rnd1 = (from q in db.LG_RND1s
                                                            where (q.c_gdg == gudang) && (q.c_rnno == RnTmp)
                                                              && (q.c_iteno == field.Item) && (q.c_batch == field.batchterima)
                                                            select q).Take(1).SingleOrDefault();
                                                    if (rnd1 == null)
                                                    {
                                                        rnd1 = new LG_RND1()
                                                        {
                                                            c_gdg = gudang,
                                                            c_rnno = RnTmp,
                                                            c_iteno = field.Item,
                                                            c_batch = field.batchterima,
                                                            n_gsisa = (isGood ? field.Acceptance : 0),
                                                            n_bsisa = (isGood ? 0 : field.Acceptance),
                                                            n_gqty = 0,
                                                            n_bqty = 0,
                                                        };

                                                        //db.LG_RND1s.InsertOnSubmit(rnd1);

                                                        Commons.CheckAndProcessBatch(db, field.Item, field.batchterima, date, nipEntry);

                                                        listRND1.Add(rnd1);
                                                    }
                                                    else
                                                    {
                                                        rnd1.n_gsisa += (isGood ? field.Acceptance : 0);
                                                        rnd1.n_bsisa += (isGood ? 0 : field.Acceptance);
                                                    }
                                                }
                                            }

                                            #region RCD1

                                            rcd1 = listRCD1.Find(delegate(LG_RCD1 rcd)
                                            {
                                                return gudang.Equals(rcd.c_gdg) &&
                                                  RnTmp.Equals(rcd.c_rnno, StringComparison.OrdinalIgnoreCase) &&
                                                  field.Item.Equals(rcd.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                                                  field.batchterima.Equals(rcd.c_batch, StringComparison.OrdinalIgnoreCase) &&
                                                  field.NoDO.Equals(rcd.c_dono, StringComparison.OrdinalIgnoreCase);
                                            });

                                            if (rcd1 == null)
                                            {
                                                rcd1 = new LG_RCD1()
                                                {
                                                    c_gdg = gudang,
                                                    c_rcno = rcID,
                                                    c_iteno = field.Item,
                                                    c_batch = field.batchterima,
                                                    c_type = TypePB,
                                                    c_dono = field.NoDO,
                                                    c_rnno = RnTmp,
                                                    n_qty = 0,
                                                    n_qtyrcv = 0,
                                                    n_qtydestroy = 0,
                                                    n_sisa = 0,
                                                    v_outlet = field.Outlet,
                                                    c_typeoutlet = field.TipeOutlet,
                                                    c_batchterima = field.Batch
                                                };

                                                listRCD1.Add(rcd1);
                                            }

                                            rcd1.n_qty = rcd1.n_sisa += field.Acceptance;
                                            rcd1.n_qtyrcv = nQtyReceived;
                                            rcd1.n_qtydestroy = field.Destroy;

                                            #endregion

                                            #region RCD2

                                            rcd2 = listRCD2.Find(delegate(LG_RCD2 rcd)
                                            {
                                                return gudang.Equals(rcd.c_gdg) &&
                                                  RnTmp.Equals(rcd.c_rnno, StringComparison.OrdinalIgnoreCase) &&
                                                  field.Item.Equals(rcd.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                                                  field.batchterima.Equals(rcd.c_batch, StringComparison.OrdinalIgnoreCase) &&
                                                  field.NoDO.Equals(rcd.c_dono, StringComparison.OrdinalIgnoreCase) &&
                                                  spID.Equals(rcd.c_spno, StringComparison.OrdinalIgnoreCase);
                                            });

                                            if (rcd2 == null)
                                            {
                                                rcd2 = new LG_RCD2()
                                                {
                                                    c_gdg = gudang,
                                                    c_rcno = rcID,
                                                    c_iteno = field.Item,
                                                    c_batch = field.batchterima,
                                                    c_type = TypePB,
                                                    c_dono = field.NoDO,
                                                    c_rnno = RnTmp,
                                                    n_qty = 0,
                                                    n_sisa = 0,
                                                    c_spno = spID
                                                };

                                                listRCD2.Add(rcd2);
                                            }

                                            rcd2.n_qty = rcd2.n_sisa += field.Acceptance;

                                            #endregion

                                            #region RCD3

                                            rcd3 = listRCD3.Find(delegate(LG_RCD3 rcd)
                                            {
                                                return gudang.Equals(rcd.c_gdg) &&
                                                  field.Item.Equals(rcd.c_iteno, StringComparison.OrdinalIgnoreCase);
                                            });

                                            if (rcd3 == null)
                                            {
                                                rciDOPL = lstRCItemDiscInfo.Find(delegate(ReturCustomerInfo rci)
                                                {
                                                    return field.Item.Equals((string.IsNullOrEmpty(rci.Item) ? string.Empty : rci.Item.Trim()), StringComparison.OrdinalIgnoreCase);
                                                });

                                                if (rciDOPL == null)
                                                {
                                                    rciDOPL = (from q in db.FA_MasItms
                                                               join q1 in
                                                                   (from sq in db.FA_DiscHes
                                                                    join sq1 in db.FA_DiscDs on sq.c_nodisc equals sq1.c_nodisc
                                                                    where (sq.c_type == "03") && (sq1.c_iteno == field.Item)
                                                                    select sq1) on q.c_iteno equals q1.c_iteno into q_1
                                                               from qDisc in q_1.DefaultIfEmpty()
                                                               where (q.c_iteno == field.Item)
                                                               select new ReturCustomerInfo()
                                                               {
                                                                   Item = field.Item,
                                                                   Dec1 = (q.n_salpri.HasValue ? q.n_salpri.Value : 0),
                                                                   Dec2 = (qDisc == null ? 0 : (qDisc.n_discon.HasValue ? qDisc.n_discon.Value : 0))
                                                               }).Take(1).SingleOrDefault();

                                                    field.n_disc = rciDOPL.Dec2;
                                                    field.n_salpri = rciDOPL.Dec1;

                                                    lstRCItemDiscInfo.Add(rciDOPL);
                                                }

                                                if (rciDOPL == null)
                                                {
                                                    rcd3 = new LG_RCD3()
                                                    {
                                                        c_gdg = gudang,
                                                        c_rcno = rcID,
                                                        c_iteno = field.Item,
                                                        n_salpri = 0,
                                                        n_disc = 0
                                                    };
                                                }
                                                else
                                                {
                                                    rcd3 = new LG_RCD3()
                                                    {
                                                        c_gdg = gudang,
                                                        c_rcno = rcID,
                                                        c_iteno = field.Item,
                                                        n_salpri = rciDOPL.Dec1,
                                                        n_disc = rciDOPL.Dec2
                                                    };
                                                }

                                                listRCD3.Add(rcd3);
                                            }

                                            #endregion

                                            totalDetails++;
                                        }
                                        else
                                        {
                                            isReTry = true;

                                            goto doNotExists;
                                        }

                                        #region Old Coded

                                        //listPLD2 = (from q in db.LG_PLHs
                                        //            join q1 in db.LG_PLD2s on q.c_plno equals q1.c_plno
                                        //            where (q.c_plno == rciDOPL.Ref2) && (q1.c_iteno == rciDOPL.Item)
                                        //              && (q1.c_batch == field.Batch)
                                        //              && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                        //            select q1).ToList();

                                        //if ((listPLD2 != null) && (listPLD2.Count > 0))
                                        //{
                                        //  for (nLoopD = 0; nLoopD < listPLD2.Count; nLoopD++)
                                        //  {
                                        //    pld2 = listPLD2[nLoopD];

                                        //    spID = (string.IsNullOrEmpty(pld2.c_spno) ? SPDefault : pld2.c_spno.Trim());

                                        //    #region RN

                                        //    RnTmp = (string.IsNullOrEmpty(pld2.c_rnno) ? string.Empty : pld2.c_rnno.Trim());

                                        //    rnd1 = listRND1.Find(delegate(LG_RND1 rnd)
                                        //    {
                                        //      return gudang.Equals(rnd.c_gdg) &&
                                        //        RnTmp.Equals((string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                        //        field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                        //        field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                        //    });

                                        //    if (rnd1 == null)
                                        //    {
                                        //      rnd1 = (from q in db.LG_RND1s
                                        //              where (q.c_gdg == gudang) && (q.c_rnno == RnTmp)
                                        //                && (q.c_iteno == field.Item) && (q.c_batch == field.Batch)
                                        //              select q).Take(1).SingleOrDefault();
                                        //    }

                                        //    if (rnd1 != null)
                                        //    {
                                        //      listRND1.Add(rnd1);

                                        //      rnd1.n_gsisa += (isGood ? field.Acceptance : 0);
                                        //      rnd1.n_bsisa += (isGood ? 0 : field.Acceptance);
                                        //    }
                                        //    else
                                        //    {
                                        //      #region Cek RN Header

                                        //      if (!listRNH.Exists(delegate(LG_RNH rn)
                                        //      {
                                        //        return rn.c_gdg.Equals(gudang) &&
                                        //          RnTmp.Equals((string.IsNullOrEmpty(rn.c_rnno) ? string.Empty : rn.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                        //          suplId.Equals((string.IsNullOrEmpty(rn.c_from) ? string.Empty : rn.c_from.Trim()), StringComparison.OrdinalIgnoreCase);
                                        //      }))
                                        //      {
                                        //        if (dicItemSupl.ContainsKey(field.Item))
                                        //        {
                                        //          suplId = dicItemSupl[field.Item];
                                        //        }
                                        //        else
                                        //        {
                                        //          suplId = db.FA_MasItms.Where(x => x.c_iteno == field.Item).Select(y => y.c_nosup).Take(1).SingleOrDefault();

                                        //          if (string.IsNullOrEmpty(suplId))
                                        //          {
                                        //            suplId = "00000";
                                        //          }
                                        //        }

                                        //        RnTmp = string.Concat("RNXX", gudang, suplId);

                                        //        rnh = (from q in db.LG_RNHs
                                        //               where (q.c_gdg == gudang) && (q.c_rnno == RnTmp) &&
                                        //                (q.c_from == suplId)
                                        //               select q).Take(1).SingleOrDefault();

                                        //        if (rnh == null)
                                        //        {
                                        //          rnh = new LG_RNH()
                                        //          {
                                        //            c_gdg = gudang,
                                        //            c_rnno = RnTmp,
                                        //            d_rndate = date,
                                        //            c_type = "01",
                                        //            l_float = false,
                                        //            c_dono = DODefault,
                                        //            d_dodate = date,
                                        //            v_ket = Constant.SYSTEM_USERNAME,
                                        //            c_from = suplId,
                                        //            n_bea = 0,
                                        //            l_print = true,
                                        //            l_status = true,
                                        //            c_entry = Constant.SYSTEM_USERNAME,
                                        //            d_entry = date,
                                        //            c_update = Constant.SYSTEM_USERNAME,
                                        //            d_update = date,
                                        //            l_delete = false,
                                        //            l_khusus = false,
                                        //          };

                                        //          db.LG_RNHs.InsertOnSubmit(rnh);
                                        //        }

                                        //        listRNH.Add(rnh);
                                        //      }

                                        //      #endregion

                                        //      #region RND1

                                        //      rnd1 = new LG_RND1()
                                        //      {
                                        //        c_gdg = gudang,
                                        //        c_rnno = RnTmp,
                                        //        c_iteno = field.Item,
                                        //        c_batch = field.Batch,
                                        //        n_floqty = 0,
                                        //        n_gsisa = (isGood ? field.Acceptance : 0),
                                        //        n_bsisa = (isGood ? 0 : field.Acceptance),
                                        //        n_gqty = 0,
                                        //        n_bqty = 0,
                                        //      };

                                        //      db.LG_RND1s.InsertOnSubmit(rnd1);

                                        //      Commons.CheckAndProcessBatch(db, field.Item, field.Batch, date, nipEntry);

                                        //      listRND1.Add(rnd1);

                                        //      #endregion
                                        //    }

                                        //    #endregion

                                        //    #region RCD1

                                        //    rcd1 = listRCD1.Find(delegate(LG_RCD1 rcd)
                                        //    {
                                        //      return gudang.Equals(rcd.c_gdg) &&
                                        //        RnTmp.Equals(rcd.c_rnno, StringComparison.OrdinalIgnoreCase) &&
                                        //        field.Item.Equals(rcd.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                                        //        field.Batch.Equals(rcd.c_batch, StringComparison.OrdinalIgnoreCase) &&
                                        //        field.NoDO.Equals(rcd.c_dono, StringComparison.OrdinalIgnoreCase);
                                        //    });

                                        //    if (rcd1 == null)
                                        //    {
                                        //      rcd1 = new LG_RCD1()
                                        //      {
                                        //        c_gdg = gudang,
                                        //        c_rcno = rcID,
                                        //        c_iteno = field.Item,
                                        //        c_batch = field.Batch,
                                        //        c_type = TypePB,
                                        //        c_dono = field.NoDO,
                                        //        c_rnno = RnTmp,
                                        //        n_qty = 0,
                                        //        n_qtyrcv = 0,
                                        //        n_qtydestroy = 0,
                                        //        n_sisa = 0
                                        //      };

                                        //      listRCD1.Add(rcd1);
                                        //    }

                                        //    rcd1.n_qty = rcd1.n_sisa += field.Acceptance;
                                        //    rcd1.n_qtyrcv = field.QuantityReceive;
                                        //    rcd1.n_qtydestroy = field.Destroy;

                                        //    #endregion

                                        //    #region RCD2

                                        //    rcd2 = listRCD2.Find(delegate(LG_RCD2 rcd)
                                        //    {
                                        //      return gudang.Equals(rcd.c_gdg) &&
                                        //        RnTmp.Equals(rcd.c_rnno, StringComparison.OrdinalIgnoreCase) &&
                                        //        field.Item.Equals(rcd.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                                        //        field.Batch.Equals(rcd.c_batch, StringComparison.OrdinalIgnoreCase) &&
                                        //        field.NoDO.Equals(rcd.c_dono, StringComparison.OrdinalIgnoreCase) &&
                                        //        spID.Equals(rcd.c_spno, StringComparison.OrdinalIgnoreCase);
                                        //    });

                                        //    if (rcd2 == null)
                                        //    {
                                        //      rcd2 = new LG_RCD2()
                                        //      {
                                        //        c_gdg = gudang,
                                        //        c_rcno = rcID,
                                        //        c_iteno = field.Item,
                                        //        c_batch = field.Batch,
                                        //        c_type = TypePB,
                                        //        c_dono = field.NoDO,
                                        //        c_rnno = RnTmp,
                                        //        n_qty = 0,
                                        //        n_sisa = 0,
                                        //        c_spno = spID
                                        //      };

                                        //      listRCD2.Add(rcd2);
                                        //    }

                                        //    rcd2.n_qty = rcd2.n_sisa += field.Acceptance;

                                        //    #endregion

                                        //    #region RCD3

                                        //    rcd3 = listRCD3.Find(delegate(LG_RCD3 rcd)
                                        //    {
                                        //      return gudang.Equals(rcd.c_gdg) &&
                                        //        field.Item.Equals(rcd.c_iteno, StringComparison.OrdinalIgnoreCase);
                                        //    });

                                        //    if (rcd3 == null)
                                        //    {
                                        //      rciDOPL = lstRCItemDiscInfo.Find(delegate(ReturCustomerInfo rci)
                                        //      {
                                        //        return field.Item.Equals((string.IsNullOrEmpty(rci.Item) ? string.Empty : rci.Item.Trim()), StringComparison.OrdinalIgnoreCase);
                                        //      });

                                        //      if (rciDOPL == null)
                                        //      {
                                        //        rciDOPL = (from q in db.FA_MasItms
                                        //                   join q1 in
                                        //                     (from sq in db.FA_DiscHes
                                        //                      join sq1 in db.FA_DiscDs on sq.c_nodisc equals sq1.c_nodisc
                                        //                      where (sq.c_type == "03") && (sq1.c_iteno == field.Item)
                                        //                      select sq1) on q.c_iteno equals q1.c_iteno into q_1
                                        //                   from qDisc in q_1.DefaultIfEmpty()
                                        //                   where (q.c_iteno == field.Item)
                                        //                   select new ReturCustomerInfo()
                                        //                   {
                                        //                     Item = field.Item,
                                        //                     Dec1 = (q.n_salpri.HasValue ? q.n_salpri.Value : 0),
                                        //                     Dec2 = (qDisc == null ? 0 : (qDisc.n_discon.HasValue ? qDisc.n_discon.Value : 0))
                                        //                   }).Take(1).SingleOrDefault();

                                        //        lstRCItemDiscInfo.Add(rciDOPL);
                                        //      }

                                        //      if (rciDOPL == null)
                                        //      {
                                        //        rcd3 = new LG_RCD3()
                                        //        {
                                        //          c_gdg = gudang,
                                        //          c_rcno = rcID,
                                        //          c_iteno = field.Item,
                                        //          n_salpri = 0,
                                        //          n_disc = 0
                                        //        };
                                        //      }
                                        //      else
                                        //      {
                                        //        rcd3 = new LG_RCD3()
                                        //        {
                                        //          c_gdg = gudang,
                                        //          c_rcno = rcID,
                                        //          c_iteno = field.Item,
                                        //          n_salpri = rciDOPL.Dec1,
                                        //          n_disc = rciDOPL.Dec2
                                        //        };
                                        //      }

                                        //      listRCD3.Add(rcd3);
                                        //    }

                                        //    #endregion

                                        //    totalDetails++;
                                        //  }
                                        //}
                                        //else
                                        //{
                                        //  isReTry = true;

                                        //  goto doNotExists;
                                        //}

                                        #endregion
                                    }

                                    lstDOPLInfo.Clear();

                                    #endregion
                                }
                                else
                                {
                                    isReTry = true;

                                    //field.NoDO = DODefault;

                                    goto doNotExists;
                                }
                            }

                            #region "Stok Pemusnahan"
                            if (field.Destroy > 0)
                            {
                                #region RN

                                #region Cek RN Header

                                if (dicItemSupl.ContainsKey(field.Item))
                                {
                                    suplId = dicItemSupl[field.Item];
                                }
                                else
                                {
                                    suplId = db.FA_MasItms.Where(x => x.c_iteno == field.Item).Select(y => y.c_nosup).Take(1).SingleOrDefault();

                                    if (string.IsNullOrEmpty(suplId))
                                    {
                                        suplId = "00000";
                                    }
                                }

                                RnTmp = string.Concat("RNXX5", suplId);

                                if (!listRNH.Exists(delegate(LG_RNH rn)
                                {
                                    return rn.c_gdg == '5' &&
                                      RnTmp.Equals((string.IsNullOrEmpty(rn.c_rnno) ? string.Empty : rn.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                      suplId.Equals((string.IsNullOrEmpty(rn.c_from) ? string.Empty : rn.c_from.Trim()), StringComparison.OrdinalIgnoreCase);
                                }))
                                {
                                    rnh = (from q in db.LG_RNHs
                                           where (q.c_gdg == '5') && (q.c_rnno == RnTmp) &&
                                            (q.c_from == suplId)
                                           select q).Take(1).SingleOrDefault();

                                    if (rnh == null)
                                    {
                                        rnh = new LG_RNH()
                                        {
                                            c_gdg = '5',
                                            c_rnno = RnTmp,
                                            d_rndate = date,
                                            c_type = "01",
                                            l_float = false,
                                            c_dono = "RCXXXXXXXX",
                                            d_dodate = date,
                                            v_ket = Constant.SYSTEM_USERNAME,
                                            c_from = suplId,
                                            n_bea = 0,
                                            l_print = true,
                                            l_status = true,
                                            c_entry = Constant.SYSTEM_USERNAME,
                                            d_entry = date,
                                            c_update = Constant.SYSTEM_USERNAME,
                                            d_update = date,
                                            l_delete = false,
                                            l_khusus = false,
                                        };

                                        db.LG_RNHs.InsertOnSubmit(rnh);
                                    }

                                    listRNH.Add(rnh);
                                }

                                #endregion

                                #region RND1

                                rnd1 = listRND1.Find(delegate(LG_RND1 rnd)
                                {
                                    return rnd.c_gdg == '5' &&
                                      RnTmp.Equals((string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                      field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                      field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                });

                                if (rnd1 == null)
                                {
                                    rnd1 = (from q in db.LG_RND1s
                                            where (q.c_gdg == '5') && (q.c_rnno == RnTmp)
                                              && (q.c_iteno == field.Item) && (q.c_batch == field.Batch)
                                            select q).Take(1).SingleOrDefault();
                                }

                                if (rnd1 == null)
                                {
                                    rnd1 = new LG_RND1()
                                    {
                                        c_gdg = '5',
                                        c_rnno = RnTmp,
                                        c_iteno = field.Item,
                                        c_batch = field.Batch,
                                        n_gsisa = 0,
                                        n_bsisa = field.Destroy,
                                        n_gqty = 0,
                                        n_bqty = 0,
                                    };

                                    listRND1.Add(rnd1);
                                }
                                else
                                {
                                    rnd1.n_bsisa += field.Destroy;
                                }
                                #endregion

                                #endregion

                                #region RCD5

                                rcd5 = listRCD5.Find(delegate(LG_RCD5 rcd)
                                {
                                    return gudang.Equals(rcd.c_gdg) &&
                                      RnTmp.Equals(rcd.c_rnno, StringComparison.OrdinalIgnoreCase) &&
                                      field.Item.Equals(rcd.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                                      field.Batch.Equals(rcd.c_batch, StringComparison.OrdinalIgnoreCase) &&
                                      rcd.c_gdg2 == '5';
                                });

                                if (rcd5 == null)
                                {
                                    rcd5 = new LG_RCD5()
                                    {
                                        c_gdg = gudang,
                                        c_rcno = rcID,
                                        c_iteno = field.Item,
                                        c_batch = field.Batch,
                                        c_gdg2 = '5',
                                        c_rnno = RnTmp,
                                        n_qtydestroy = 0,
                                        n_sisa = 0,
                                    };

                                    listRCD5.Add(rcd5);
                                }

                                rcd5.n_qtydestroy = rcd5.n_sisa += field.Destroy;

                                #endregion
                            }
                            #endregion
                        }
                    }

                    if ((listRND1 != null) && (listRND1.Count > 0))
                    {
                        listSumRND1 = listRND1.GroupBy(x => new { x.c_gdg, x.c_rnno, x.c_iteno, x.c_batch })
                                        .Select(x => new LG_RND1()
                                        {
                                            c_gdg = x.Key.c_gdg,
                                            c_rnno = x.Key.c_rnno,
                                            c_iteno = x.Key.c_iteno,
                                            c_batch = x.Key.c_batch,
                                            n_gqty = 0,
                                            n_bqty = 0,
                                            n_gsisa = x.Sum(y => (y.n_gsisa.HasValue ? y.n_gsisa.Value : 0)),
                                            n_bsisa = x.Sum(y => (y.n_bsisa.HasValue ? y.n_bsisa.Value : 0)),
                                        }).ToList();

                        db.LG_RND1s.InsertAllOnSubmit(listSumRND1.ToArray());
                        listRND1.Clear();
                        listSumRND1.Clear();
                    }

                    if ((listRCD1 != null) && (listRCD1.Count > 0))
                    {
                        db.LG_RCD1s.InsertAllOnSubmit(listRCD1.ToArray());
                        listRCD1.Clear();
                    }

                    if ((listRCD2 != null) && (listRCD2.Count > 0))
                    {
                        db.LG_RCD2s.InsertAllOnSubmit(listRCD2.ToArray());
                        listRCD2.Clear();
                    }

                    if ((listRCD3 != null) && (listRCD3.Count > 0))
                    {
                        db.LG_RCD3s.InsertAllOnSubmit(listRCD3.ToArray());
                        listRCD3.Clear();
                    }

                    if ((listRCD5 != null) && (listRCD5.Count > 0))
                    {
                        db.LG_RCD5s.InsertAllOnSubmit(listRCD5.ToArray());
                        listRCD5.Clear();
                    }

                    #endregion

                    dic = new Dictionary<string, string>();

                    if (totalDetails > 0)
                    {

                        rch.l_send = isSent;
                        rch.l_sent = isSent;

                        dic.Add("RC", rcID);
                        dic.Add("Tanggal", date.ToString("yyyyMMdd"));
                        dic.Add("Pin", pinCode);
                        dic.Add("Cabang", cusNam);
                        dic.Add("Sent", isSent.ToString().ToLower());
                        dic.Add("Keterangan", structure.Fields.Keterangan);
                        dic.Add("PBBR", structure.Fields.PBID);
                        dic.Add("Res", isResponse.ToString());

                        result = string.Format("Total {0} detail(s)", totalDetails);

                        hasAnyChanges = true;
                    }
                    else if (totalDetails == 0)
                    {
                        result = "Tidak boleh terima 0";
                        rpe = ResponseParser.ResponseParserEnum.IsError;
                        error = "benar";
                        goto endLogic;
                    }

                    #endregion
                }

                if (hasAnyChanges)
                {
                    db.SubmitChanges();
                    #region Old code 
                    //suwandi 16 Mei 2018
                    //db.Transaction.Commit();
                    //if (isResponse)
                    //{
                    //    //if (isResponse)
                    //    //{
                    //    NewNoPBB = ResponToDisCorePB(db, structure, null, date);
                    //}
                    //if (string.IsNullOrEmpty(NewNoPBB))
                    //{
                    //    NewNoPBB = OldPBBR = structure.Fields.PBID;
                    //    structure.Fields.PBID = NewNoPBB;
                    //}
                    //else
                    //{
                    //    structure.Fields.PBIDOLD = structure.Fields.PBID;
                    //    structure.Fields.PBID = NewNoPBB;

                    //    //}
                    //}
                    #endregion
                    rch = new LG_RCH();

                    rch = (from q in db.LG_RCHes
                           where q.c_rcno == rcID
                           select q).Take(1).SingleOrDefault();

                    //db.Dispose();
                    ////db.Connection.Close();
                    //db = new ORMDataContext(Functionals.ActiveConnectionString);
                    //db.Connection.Open();

                    //db.Transaction = db.Connection.BeginTransaction();
                    emailksl = (from q in db.Temp_emails
                                where q.c_cab_dcore == structure.Fields.PBID.Substring(3, 3) && q.group_mail == "ksl"
                                select q.nv_email_cbg).SingleOrDefault();
                    emailksa = (from q in db.Temp_emails
                                where q.c_cab_dcore == structure.Fields.PBID.Substring(3, 3) && q.group_mail == "ksa"
                                select q.nv_email_cbg).SingleOrDefault();
                    emailkca = (from q in db.Temp_emails
                                where q.c_cab_dcore == structure.Fields.PBID.Substring(3, 3) && q.group_mail == "kacab"
                                select q.nv_email_cbg).SingleOrDefault();

                    rch.l_send = isSent;
                    rch.l_sent = isSent;
                    //rch.v_pbbrno = structure.Fields.PBID;
                    //rch.v_oldpbbrno = structure.Fields.PBIDOLD ?? OldPBBR;
                    //rch.c_type = ResponToDisCoreTipePB(db, structure);

                    //isSent = ResponToDisCorePBReturn(db, structure, rcID, date); //old code by suwandi 16 Mei 2018
                    NewNoPBB = ResponToDisCorePB(db, structure, rcID, date); //new code by Suwandi 16 Mei 2018
                    if (NewNoPBB.Substring(0, 2) != "RS")
                    {
                        rpe = ResponseParser.ResponseParserEnum.IsFailed;
                        result = NewNoPBB;
                        error = "benar";
                        goto endLogic;
                    }
                    db.SubmitChanges();
                    db.Transaction.Commit();

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
                rpe = ResponseParser.ResponseParserEnum.IsError;

                result = string.Format("ScmsSoaLibrary.Bussiness.Retur:ReturCustomer - {0}", ex.Message);

                Logger.WriteLine(result, true);
                Logger.WriteLine(ex.StackTrace);
            }

        endLogic:
            result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);
            if (rpe == ResponseParser.ResponseParserEnum.IsSuccess & isResponse == true)
            {
                System.Net.Mail.SmtpClient smtp = null;
                StringBuilder sb = new StringBuilder();
                try
                {
                    using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                    {
                        // send mail containing the file here

                        mail.From = new System.Net.Mail.MailAddress("scms.sph@ams.co.id", "Supply Chain Management System");

                        mail.Subject = "Laporan Data PBB/R Terima Selisih No. " + structure.Fields.PBID;
                        
                        //mail.To.Add("suwandi@ams.co.id");
                        mail.To.Add(emailksl);
                        mail.To.Add(emailksa);
                        mail.To.Add(emailkca);
                        mail.CC.Add("denny.ams.co.id");
                        mail.CC.Add("movi@log.ams.co.id");
                        sb.AppendLine("Dear KSL, KSA dan BM,");
                        sb.AppendLine();
                        sb.AppendLine("Mohon di periksa kembali PBB no " + structure.Fields.PBID + " karena ada selisih terima barang");
                        sb.AppendLine();
                        sb.AppendLine();
                        sb.AppendLine(bodymail);
                        sb.AppendLine();
                        sb.AppendLine("Terima Kasih,");
                        sb.AppendLine(structure.Fields.USER + " - Gd. Retur");

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
            if (dic != null)
            {
                dic.Clear();
            }
            //add by suwandi 21 Mei 2018
            if (error == "benar")
            {
                db.Transaction.Rollback();
            }
            db.Dispose();

            return result;
        }

        public string ReturCustomerIn(ScmsSoaLibrary.Parser.Class.ReturCustomerStructureIn structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            ScmsSoaLibrary.Parser.Class.ReturCustomerStructureInField field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now;

            IDictionary<string, string> dic = null;

            char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '1' : structure.Fields.Gudang[0]);

            string result = null,
              nipEntry = null,
              rnTempId = null;
            string rcID = null,
              pinCode = null,
              typeItem = null;
            int nLoop = 0;
            decimal nQtyAlloc = 0,
              nQtyAlocate = 0,
              totalDetails = 0;
            bool isSent = false;

            List<LG_RCD1> ListRCD1 = null;
            List<LG_RCD2> ListRCD2 = null;
            List<LG_RCD3> ListRCD3 = null;
            List<LG_RCD4> ListRCD4 = null;

            //List<string> ListStringRCD3 = null;

            List<LG_RNH> ListRNH = null;
            List<LG_RND1> ListRND1 = null;

            List<LG_ComboH> ListComboH = null;

            //List<LG_RNH> ListRNHGet = null;
            //List<LG_RND1> ListRND1Get = null;

            List<LG_PLD2> ListPLD2 = null;
            List<LG_STD2> ListSTD2 = null;

            List<string> lstTemp = null;
            List<string> lstTemp1 = null;

            List<FA_MasItm> listMsItem = null;
            List<FA_DiscD> listMsDisc = null;

            LG_RCD1 rcd1 = null;
            LG_RCD2 rcd2 = null;
            LG_RCD3 rcd3 = null;

            LG_PLD2 pld2 = null;
            LG_STD2 std2 = null;

            LG_RNH rnh = null;
            LG_RND1 rnd1 = null;

            LG_ComboH comboh = null;

            FA_MasItm masitm = null;
            FA_DiscD masdisc = null;

            LG_RCH rch = null;
            SCMS_BASPBD baspbd = null;

            //List<LG_RND1> UListRND1 = null;
            //LG_RND1 listDataRND1 = null;
            //List<object> listDataO = null;
            //LG_RCH rch = null;
            //LG_RNH rnhSingle = null;

            //List<string> listTypeRN = new List<string>();
            //listTypeRN.Add("01");
            //listTypeRN.Add("03");
            //listTypeRN.Add("04");
            //listTypeRN.Add("05");

            #region Authorize Nip Entrys

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            #endregion

            rcID = (structure.Fields.ReturCustomerID ?? string.Empty);

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
                {
                    #region Add

                    if (!string.IsNullOrEmpty(rcID))
                    {
                        result = "Nomor Retur Customer harus kosong.";

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
                        result = "Retur Customer tidak dapat disimpan, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    rcID = Commons.GenerateNumbering<LG_RCH>(db, "RC", '3', "11", date, "c_rcno");
                    pinCode = Functionals.GeneratedRandomPinId(4, string.Empty);

                    rch = new LG_RCH()
                    {
                        c_cusno = structure.Fields.Customer,
                        c_entry = nipEntry,
                        c_update = nipEntry,
                        d_entry = date,
                        d_update = date,
                        d_rcdate = date,
                        c_gdg = gudang,
                        c_pin = pinCode,
                        c_rcno = rcID,
                        l_delete = false,
                        l_send = false,
                        l_status = false,
                        v_ket = structure.Fields.Keterangan,
                        d_send = date,
                        v_ket_mark = null,
                        c_pbbrno = null,
                        v_oldpbbrno = null,
                        v_pbbrno = structure.Fields.PBBR,
                        l_sent = false,
                        c_baspbno = structure.Fields.BaspbNo,
                    };

                    db.LG_RCHes.InsertOnSubmit(rch);

                    #region Old Coded

                    //db.SubmitChanges();

                    //rch = (from q in db.LG_RCHes
                    //       where q.v_ket == tmpNumbering
                    //       select q).Take(1).SingleOrDefault();

                    //if (!string.IsNullOrEmpty(rcID))
                    //{
                    //  result = "Nomor Retur Customer tidak dapat di raih.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}

                    //if (rch.c_rcno.Equals("XXXXXXXXXX"))
                    //{
                    //  result = "Trigger Retur Customer tidak aktif.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}

                    //rch.v_ket = structure.Fields.Keterangan;

                    //rcID = rch.c_rcno;

                    #endregion

                    #region Insert Detail

                    ListRCD1 = new List<LG_RCD1>();
                    ListRCD2 = new List<LG_RCD2>();
                    ListRCD3 = new List<LG_RCD3>();

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        //ListRND1 = new List<LG_RND1>();
                        //ListRNH = new List<LG_RNH>();

                        #region Prepare Data

                        lstTemp = structure.Fields.Field.GroupBy(x => x.Item).Select(y => y.Key).ToList();

                        listMsItem = (from q in db.FA_MasItms
                                      where lstTemp.Contains(q.c_iteno)
                                      select q).Distinct().ToList();

                        listMsDisc = (from q in db.FA_DiscDs
                                      where lstTemp.Contains(q.c_iteno)
                                      && q.c_nodisc == "DSXXXXXX03"
                                      select q).Distinct().ToList();

                        lstTemp.Clear();

                        lstTemp = structure.Fields.Field.GroupBy(x => x.NoDO).Select(y => y.Key).ToList();
                        lstTemp1 = structure.Fields.Field.GroupBy(x => x.NoRN).Select(y => y.Key).ToList();

                        ListComboH = (from q in db.LG_ComboHs
                                      where lstTemp1.Contains(q.c_combono) && (q.c_gdg == gudang)
                                       && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                      select q).Distinct().ToList();

                        ListRNH = (from q in db.LG_RNHs
                                   where lstTemp1.Contains(q.c_rnno) && (q.c_gdg == gudang)
                                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                   select q).Distinct().ToList();

                        ListRND1 = (from q in db.LG_RNHs
                                    join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                    where lstTemp1.Contains(q.c_rnno) && (q.c_gdg == gudang)
                                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                    select q1).Distinct().ToList();

                        ListPLD2 = (from q in db.LG_DOHs
                                    join q1 in db.LG_PLHs on q.c_plno equals q1.c_plno
                                    join q2 in db.LG_PLD2s on q.c_plno equals q2.c_plno
                                    where lstTemp.Contains(q.c_dono) && lstTemp1.Contains(q2.c_rnno)
                                      && (q.c_gdg == gudang) && (q.c_cusno == structure.Fields.Customer)
                                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                      && ((q1.l_delete.HasValue ? q1.l_delete.Value : false) == false)
                                    select q2).Distinct().ToList();

                        ListSTD2 = (from q in db.LG_DOHs
                                    join q1 in db.LG_STHs on q.c_plno equals q1.c_stno
                                    join q2 in db.LG_STD2s on q.c_plno equals q2.c_stno
                                    where lstTemp.Contains(q.c_dono) && lstTemp1.Contains(q2.c_no)
                                      && (q.c_gdg == gudang) && (q.c_cusno == structure.Fields.Customer)
                                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                      && ((q1.l_delete.HasValue ? q1.l_delete.Value : false) == false)
                                    select q2).Distinct().ToList();
                        lstTemp1.Clear();
                        lstTemp.Clear();

                        #endregion

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if ((field != null) && field.IsNew && (field.Quantity > 0) && (!string.IsNullOrEmpty(field.Batch)) && (field.Batch.Trim().Length > 0))
                            {
                                if (Commons.CheckAndProcessBatch(db, field.Item, field.Batch, date, nipEntry) > 0)
                                {
                                    rcd1 = new LG_RCD1()
                                    {
                                        c_gdg = gudang,
                                        c_rcno = rcID,
                                        c_iteno = field.Item,
                                        c_batch = field.Batch,
                                        c_type = field.TypeItem,
                                        c_dono = field.NoDO,
                                        c_rnno = field.NoRN,
                                        n_qty = field.Quantity,
                                        n_sisa = field.Quantity,
                                        n_qtyrcv = field.Quantity,
                                        n_qtydestroy = 0
                                    };

                                    rcd2 = new LG_RCD2()
                                    {
                                        c_gdg = gudang,
                                        c_rcno = rcID,
                                        c_iteno = field.Item,
                                        c_batch = field.Batch,
                                        c_type = field.TypeItem,
                                        c_dono = field.NoDO,
                                        c_rnno = field.NoRN,
                                        c_spno = "SPXXXXXXXX",
                                        n_qty = field.Quantity,
                                        n_sisa = field.Quantity
                                    };

                                    masitm = listMsItem.Find(delegate(FA_MasItm itm)
                                    {
                                        return field.Item.Equals((string.IsNullOrEmpty(itm.c_iteno) ? string.Empty : itm.c_iteno), StringComparison.OrdinalIgnoreCase);
                                    });

                                    masdisc = listMsDisc.Find(delegate(FA_DiscD itm)
                                    {
                                        return field.Item.Equals((string.IsNullOrEmpty(itm.c_iteno) ? string.Empty : itm.c_iteno), StringComparison.OrdinalIgnoreCase);
                                    });

                                    if (field.NoDO.Equals("DOXXXXXXXX", StringComparison.OrdinalIgnoreCase))
                                    {
                                        #region DO Tidak Di isi

                                        //pld2 = ListPLD2.Find(delegate(LG_PLD2 pld)
                                        //{
                                        //  return field.NoRN.Equals((string.IsNullOrEmpty(pld.c_rnno) ? string.Empty : pld.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                        //    field.Batch.Equals((string.IsNullOrEmpty(pld.c_batch) ? string.Empty : pld.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                        //    field.Item.Equals((string.IsNullOrEmpty(pld.c_iteno) ? string.Empty : pld.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase);
                                        //});

                                        if (masitm != null)
                                        {
                                            rnTempId = string.Concat("RNXX", gudang, masitm.c_nosup);

                                            rnh = ListRNH.Find(delegate(LG_RNH rn)
                                            {
                                                return rnTempId.Equals((string.IsNullOrEmpty(rn.c_rnno) ? string.Empty : rn.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
                                            });

                                            rnd1 = ListRND1.Find(delegate(LG_RND1 rnd)
                                            {
                                                return rnTempId.Equals((string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno), StringComparison.OrdinalIgnoreCase) &&
                                                  field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno), StringComparison.OrdinalIgnoreCase) &&
                                                  field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch), StringComparison.OrdinalIgnoreCase);
                                            });

                                            if (!ListRCD3.Exists(delegate(LG_RCD3 rcd)
                                            {
                                                return field.Item.Equals((string.IsNullOrEmpty(rcd.c_iteno) ? string.Empty : rcd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase);
                                            }))
                                            {
                                                rcd3 = new LG_RCD3()
                                                {
                                                    c_gdg = gudang,
                                                    c_rcno = rcID,
                                                    c_iteno = field.Item,
                                                    n_disc = masdisc.n_discon,
                                                    n_salpri = masitm.n_salpri
                                                };
                                            }
                                            else
                                            {
                                                rcd3 = null;
                                            }

                                            #region RN Header

                                            if (rnh == null)
                                            {
                                                rnh = (from q in db.LG_RNHs
                                                       where (q.c_gdg == gudang) && (q.c_rnno == rnTempId)
                                                           //&& (q.c_dono == field.NoDO)
                                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                                       select q).Take(1).SingleOrDefault();
                                            }

                                            if (rnh == null)
                                            {
                                                rnh = new LG_RNH()
                                                {
                                                    c_gdg = gudang,
                                                    c_rnno = rnTempId,
                                                    d_rndate = date,
                                                    c_type = "07",
                                                    l_float = false,
                                                    c_dono = field.NoDO,
                                                    d_dodate = date,
                                                    v_ket = "Auto Generate",
                                                    c_from = masitm.c_nosup,
                                                    n_bea = 0,
                                                    l_print = false,
                                                    l_status = false,
                                                    c_entry = "auto",
                                                    d_entry = date,
                                                    c_update = "auto",
                                                    d_update = date
                                                };

                                                db.LG_RNHs.InsertOnSubmit(rnh);

                                                ListRNH.Add(rnh);
                                            }

                                            #endregion

                                            #region RN Detail

                                            if (rnd1 == null)
                                            {
                                                rnd1 = (from q in db.LG_RND1s
                                                        where (q.c_gdg == gudang) && (q.c_rnno == rnTempId)
                                                         && (q.c_iteno == field.Item) && (q.c_batch == field.Batch)
                                                        select q).Take(1).SingleOrDefault();
                                            }

                                            if (rnd1 == null)
                                            {
                                                rnd1 = new LG_RND1()
                                                {
                                                    c_gdg = gudang,
                                                    c_rnno = rnTempId,
                                                    c_iteno = field.Item,
                                                    c_batch = field.Batch,
                                                    n_gqty = 0,
                                                    n_bqty = 0,
                                                    n_gsisa = 0,
                                                    n_bsisa = 0
                                                };

                                                db.LG_RND1s.InsertOnSubmit(rnd1);

                                                ListRND1.Add(rnd1);
                                            }

                                            #endregion

                                            if ((rnh != null) && (rnd1 != null))
                                            {
                                                if (field.TypeItem.Equals("01", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    rnd1.n_gsisa += field.Quantity;
                                                }
                                                else
                                                {
                                                    rnd1.n_bsisa += field.Quantity;
                                                }

                                                rcd1.c_rnno = rcd2.c_rnno = rnTempId;

                                                ListRCD1.Add(rcd1);
                                                ListRCD2.Add(rcd2);
                                                if (rcd3 != null)
                                                {
                                                    ListRCD3.Add(rcd3);
                                                }

                                                totalDetails++;
                                            }
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        #region DO Diisi

                                        //rnh = ListRNH.Find(delegate(LG_RNH rn)
                                        //{
                                        //  return field.NoRN.Equals((string.IsNullOrEmpty(rn.c_rnno) ? string.Empty : rn.c_rnno), StringComparison.OrdinalIgnoreCase);
                                        //});

                                        comboh = ListComboH.Find(delegate(LG_ComboH cb)
                                        {
                                            return field.NoRN.Equals((string.IsNullOrEmpty(cb.c_combono) ? string.Empty : cb.c_combono.Trim()), StringComparison.OrdinalIgnoreCase);
                                        });

                                        rnh = ListRNH.Find(delegate(LG_RNH rn)
                                        {
                                            return field.NoRN.Equals((string.IsNullOrEmpty(rn.c_rnno) ? string.Empty : rn.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
                                        });

                                        rnd1 = ListRND1.Find(delegate(LG_RND1 rnd)
                                        {
                                            return field.NoRN.Equals((string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                        });

                                        pld2 = ListPLD2.Find(delegate(LG_PLD2 pld)
                                        {
                                            return field.NoRN.Equals((string.IsNullOrEmpty(pld.c_rnno) ? string.Empty : pld.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              field.Batch.Equals((string.IsNullOrEmpty(pld.c_batch) ? string.Empty : pld.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              field.Item.Equals((string.IsNullOrEmpty(pld.c_iteno) ? string.Empty : pld.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase);
                                        });

                                        std2 = ListSTD2.Find(delegate(LG_STD2 std)
                                        {
                                            return field.NoRN.Equals((string.IsNullOrEmpty(std.c_no) ? string.Empty : std.c_no.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              field.Batch.Equals((string.IsNullOrEmpty(std.c_batch) ? string.Empty : std.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              field.Item.Equals((string.IsNullOrEmpty(std.c_iteno) ? string.Empty : std.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase);
                                        });

                                        if (!ListRCD3.Exists(delegate(LG_RCD3 rcd)
                                        {
                                            return field.Item.Equals((string.IsNullOrEmpty(rcd.c_iteno) ? string.Empty : rcd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase);
                                        }))
                                        {
                                            rcd3 = new LG_RCD3()
                                            {
                                                c_gdg = gudang,
                                                c_rcno = rcID,
                                                c_iteno = field.Item,
                                                n_disc = masdisc.n_discon,
                                                n_salpri = masitm.n_salpri
                                            };
                                        }
                                        else
                                        {
                                            rcd3 = null;
                                        }

                                        if ((masitm != null) && ((pld2 != null || std2 != null)))
                                        {
                                            rcd2.c_spno = pld2 == null ? "SPXXXXXXXX" : pld2.c_spno;

                                            if ((rnh != null) && (rnd1 != null))
                                            {
                                                if (field.TypeItem.Equals("01", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    rnd1.n_gsisa += field.Quantity;
                                                }
                                                else
                                                {
                                                    rnd1.n_bsisa += field.Quantity;
                                                }

                                                ListRCD1.Add(rcd1);
                                                ListRCD2.Add(rcd2);
                                                if (rcd3 != null)
                                                {
                                                    ListRCD3.Add(rcd3);
                                                }

                                                totalDetails++;
                                            }
                                            else
                                            {
                                                if (comboh != null)
                                                {
                                                    if (field.TypeItem.Equals("01", StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        comboh.n_gsisa += field.Quantity;
                                                    }
                                                    else
                                                    {
                                                        comboh.n_bsisa += field.Quantity;
                                                    }

                                                    ListRCD1.Add(rcd1);
                                                    ListRCD2.Add(rcd2);
                                                    if (rcd3 != null)
                                                    {
                                                        ListRCD3.Add(rcd3);
                                                    }

                                                    totalDetails++;
                                                }
                                            }
                                        }

                                        #endregion
                                    }
                                }
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

                            #region Old Coded

                            //var ListDO = (from q in db.LG_DOD2s
                            //              join q1 in db.LG_DOHs on q.c_dono equals q1.c_dono
                            //              where q.c_dono == field.NoDO
                            //              && q.c_iteno == field.Item
                            //              && q.c_batch == field.Batch
                            //              select new
                            //              {
                            //                q.c_dono,
                            //                q1.c_plno,
                            //                q.c_iteno,
                            //                q.c_batch
                            //              }).ToList();

                            //// Insert RCD1
                            //#region RCD1

                            //if (ListDO.Count > 0 && ListDO != null)
                            //{

                            //  var ListPL = (from q in db.LG_PLD2s
                            //                where q.c_plno == ListDO[0].c_plno
                            //                && q.c_iteno == ListDO[0].c_iteno
                            //                && q.c_batch == ListDO[0].c_batch
                            //                group q by new { q.c_plno, q.c_iteno, q.c_batch, q.c_rnno } into g
                            //                select new
                            //                {
                            //                  c_plno = g.Key.c_plno,
                            //                  c_iteno = g.Key.c_iteno,
                            //                  c_batch = g.Key.c_batch,
                            //                  c_rnno = g.Key.c_rnno,
                            //                  n_qty = g.Sum(x => x.n_qty)
                            //                }).ToList();

                            //  var ListPL2 = (from q in db.LG_PLD2s
                            //                 where q.c_plno == ListDO[0].c_plno
                            //                 && q.c_iteno == ListDO[0].c_iteno
                            //                 && q.c_batch == ListDO[0].c_batch
                            //                 group q by new { q.c_plno, q.c_iteno, q.c_batch, q.c_rnno, q.c_spno } into g
                            //                 select new
                            //                 {
                            //                   c_plno = g.Key.c_plno,
                            //                   c_iteno = g.Key.c_iteno,
                            //                   c_batch = g.Key.c_batch,
                            //                   c_rnno = g.Key.c_rnno,
                            //                   c_spno = g.Key.c_spno,
                            //                   n_qty = g.Sum(x => x.n_qty)
                            //                 }).ToList();

                            //  if (ListPL.Count > 0 && ListPL != null)
                            //  {
                            //    for (nLoopC = 0; nLoopC < ListPL.Count; nLoopC++)
                            //    {
                            //      ListRCD1.Add(new LG_RCD1()
                            //      {
                            //        c_batch = field.Batch,
                            //        c_dono = ListDO[0].c_dono,
                            //        c_gdg = gudang,
                            //        c_iteno = field.Item,
                            //        c_rcno = rcID,
                            //        c_rnno = ListPL[nLoopC].c_rnno,
                            //        n_qty = field.Quantity,
                            //        n_sisa = field.Quantity,
                            //        c_type = field.TypeItem
                            //      });
                            //    }

                            //    #region Old Code

                            //    //nQtyAlloc = field.Quantity;
                            //    //decimal? qAqum = 0, qE = 0, qI = 0;
                            //    //bool QLop = true;

                            //    //for (nLoopC = 0; nLoopC < ListPL.Count; nLoopC++)
                            //    //{
                            //    //  qI = nQtyAlloc - ListPL[nLoopC].n_qty.Value;

                            //    //  if (QLop == true)
                            //    //  {
                            //    //    qE = nQtyAlloc - qI;
                            //    //    qAqum = qAqum + qE;
                            //    //  }
                            //    //  if (qAqum > nQtyAlloc)
                            //    //  {
                            //    //    qE = nQtyAlloc - (qAqum - qE);
                            //    //    qAqum = qAqum + qE;
                            //    //  }

                            //    //  if (QLop == false)
                            //    //  {
                            //    //    qE = 0;
                            //    //  }

                            //    //  // Insert RC with PL And DO
                            //    //  if (qE != 0)
                            //    //  {

                            //    //  }
                            //    //  if (qAqum > nQtyAlloc)
                            //    //  {
                            //    //    QLop = false;
                            //    //  }
                            //    //}

                            //    #endregion
                            //  }
                            //  else if (ListPL.Count <= 0)
                            //  {
                            //    // Insert RC without NO PL
                            //    listDataRND1 = new LG_RND1();
                            //    masitm = new FA_MasItm();

                            //    listDataRND1 = (from q in db.LG_RND1s
                            //                    where q.c_gdg == gudang
                            //                    && q.c_iteno == field.Item
                            //                    && q.c_batch == field.Batch
                            //                    && q.c_rnno.Substring(0, 4) == "RNXX"
                            //                    select q).Take(1).SingleOrDefault();

                            //    if (listDataRND1 != null)
                            //    {
                            //      ListRCD1.Add(new LG_RCD1()
                            //      {
                            //        c_batch = ListDO[0].c_batch,
                            //        c_dono = ListDO[0].c_dono,
                            //        c_gdg = gudang,
                            //        c_iteno = field.Item,
                            //        c_rcno = rcID,
                            //        c_rnno = listDataRND1.c_rnno,
                            //        n_qty = field.Quantity,
                            //        n_sisa = field.Quantity,
                            //        c_type = field.TypeItem
                            //      });
                            //    }
                            //    else
                            //    {
                            //      masitm = (from q in db.FA_MasItms
                            //                where q.c_iteno == field.Item
                            //                select q).Take(1).SingleOrDefault();

                            //      RNX = "RNXX" + gudang.ToString() + masitm.c_nosup;

                            //      ListRCD1.Add(new LG_RCD1()
                            //      {
                            //        c_batch = ListDO[0].c_batch,
                            //        c_dono = ListDO[0].c_dono,
                            //        c_gdg = gudang,
                            //        c_iteno = field.Item,
                            //        c_rcno = rcID,
                            //        c_rnno = RNX,
                            //        n_qty = field.Quantity,
                            //        n_sisa = field.Quantity,
                            //        c_type = field.TypeItem
                            //      });
                            //    }
                            //  }
                            //}
                            //else if (ListDO.Count <= 0)
                            //{
                            //  // Insert RC without NO DO

                            //  listDataRND1 = new LG_RND1();
                            //  masitm = new FA_MasItm();

                            //  listDataRND1 = (from q in db.LG_RND1s
                            //                  where q.c_gdg == gudang
                            //                  && q.c_iteno == field.Item
                            //                  && q.c_batch == field.Batch
                            //                  && q.c_rnno.Substring(0, 4) == "RNXX"
                            //                  select q).Take(1).SingleOrDefault();

                            //  if (listDataRND1 != null)
                            //  {
                            //    ListRCD1.Add(new LG_RCD1()
                            //    {
                            //      c_batch = field.Batch,
                            //      c_dono = "DOXXXXXXXX",
                            //      c_gdg = gudang,
                            //      c_iteno = field.Item,
                            //      c_rcno = rcID,
                            //      c_rnno = listDataRND1.c_rnno,
                            //      n_qty = field.Quantity,
                            //      n_sisa = field.Quantity,
                            //      c_type = field.TypeItem
                            //    });
                            //  }
                            //  else
                            //  {
                            //    masitm = (from q in db.FA_MasItms
                            //              where q.c_iteno == field.Item
                            //              select q).Take(1).SingleOrDefault();

                            //    RNX = "RNXX" + gudang.ToString() + masitm.c_nosup;

                            //    ListRCD1.Add(new LG_RCD1()
                            //    {
                            //      c_batch = field.Batch,
                            //      c_dono = "DOXXXXXXXX",
                            //      c_gdg = gudang,
                            //      c_iteno = field.Item,
                            //      c_rcno = rcID,
                            //      c_rnno = RNX,
                            //      c_type = field.TypeItem,
                            //      n_qty = field.Quantity,
                            //      n_sisa = field.Quantity
                            //    });
                            //  }
                            //}

                            //#region Update Pin

                            //LG_RCD1 rcd1 = null;

                            //rcd1 = (from q in db.LG_RCD1s
                            //        where q.c_rcno == rcID
                            //        select q).Take(1).SingleOrDefault();

                            //double Pinh = Convert.ToDouble(rch.c_pin);

                            //int resultC = 0;

                            //foreach (double c in field.Batch.Trim())
                            //{
                            //  resultC++;
                            //}

                            //int Ref = resultC - 1;

                            //for (int nLoopPin = 0; nLoopPin < resultC; nLoopPin++)
                            //{
                            //  char S = Convert.ToChar(field.Batch.Trim().Substring(Ref, 1));
                            //  int test = Convert.ToInt32(S);
                            //  Pin = Pin + test;
                            //  Ref = Ref - 1;
                            //}

                            //#endregion

                            //#endregion

                            ////Insert RCD2
                            //#region RCD2

                            //if (ListDO.Count > 0 && ListDO != null)
                            //{
                            //  ListRCD2 = new List<LG_RCD2>();
                            //  var ListPL = (from q in db.LG_PLD2s
                            //                where q.c_plno == ListDO[0].c_plno
                            //                && q.c_iteno == ListDO[0].c_iteno
                            //                && q.c_batch == ListDO[0].c_batch
                            //                group q by new { q.c_plno, q.c_iteno, q.c_batch, q.c_rnno } into g
                            //                select new
                            //                {
                            //                  c_plno = g.Key.c_plno,
                            //                  c_iteno = g.Key.c_iteno,
                            //                  c_batch = g.Key.c_batch,
                            //                  c_rnno = g.Key.c_rnno,
                            //                  n_qty = g.Sum(x => x.n_qty)
                            //                }).ToList();

                            //  var ListPL2 = (from q in db.LG_PLD2s
                            //                 where q.c_plno == ListDO[0].c_plno
                            //                 && q.c_iteno == ListDO[0].c_iteno
                            //                 && q.c_batch == ListDO[0].c_batch
                            //                 group q by new { q.c_plno, q.c_iteno, q.c_batch, q.c_rnno, q.c_spno } into g
                            //                 select new
                            //                 {
                            //                   c_plno = g.Key.c_plno,
                            //                   c_iteno = g.Key.c_iteno,
                            //                   c_batch = g.Key.c_batch,
                            //                   c_rnno = g.Key.c_rnno,
                            //                   c_spno = g.Key.c_spno,
                            //                   n_qty = g.Sum(x => x.n_qty)
                            //                 }).ToList();

                            //  if (ListPL2.Count > 0 && ListPL2 != null)
                            //  {
                            //    #region Old Code

                            //    //nQtyAlloc = field.Quantity;

                            //    //decimal Aqum = 0, E = 0, I = 0;
                            //    //bool Lop = true;

                            //    //for (nLoopC = 0; nLoopC < ListPL2.Count; nLoopC++)
                            //    //{
                            //    //  I = nQtyAlloc - field.Quantity;

                            //    //  if (Lop == true)
                            //    //  {
                            //    //    E = nQtyAlloc - I;
                            //    //    Aqum = Aqum + E;
                            //    //  }
                            //    //  if (Aqum > nQtyAlloc)
                            //    //  {
                            //    //    E = nQtyAlloc - (Aqum - E);
                            //    //    Aqum = Aqum + E;
                            //    //  }
                            //    //  if (Lop == false)
                            //    //  {
                            //    //    E = 0;
                            //    //  }

                            //    //  ListRCD2.Add(new LG_RCD2()
                            //    //  {
                            //    //    c_batch = ListDO[0].c_batch,
                            //    //    c_dono = ListDO[0].c_dono,
                            //    //    c_gdg = gudang,
                            //    //    c_iteno = field.Item,
                            //    //    c_rcno = rcID,
                            //    //    c_rnno = ListPL2[nLoopC].c_rnno,
                            //    //    c_spno = ListPL2[nLoopC].c_spno,
                            //    //    n_qty = E,
                            //    //    n_sisa = E,
                            //    //    c_type = field.TypeItem
                            //    //  });
                            //    //}
                            //    //if (Aqum > nQtyAlloc)
                            //    //{
                            //    //  Lop = false;
                            //    //}

                            //    #endregion

                            //    ListRCD2.Add(new LG_RCD2()
                            //    {
                            //      c_batch = field.Batch,
                            //      c_dono = ListDO[0].c_dono,
                            //      c_gdg = gudang,
                            //      c_iteno = field.Item,
                            //      c_rcno = rcID,
                            //      c_rnno = field.NoRN,
                            //      c_type = field.TypeItem,
                            //      c_spno = "SPXXXXXXXX",
                            //      n_qty = field.Quantity,
                            //      n_sisa = field.Quantity
                            //    });

                            //  }
                            //  else if (ListPL2.Count <= 0)
                            //  {
                            //    listDataRND1 = new LG_RND1();
                            //    masitm = new FA_MasItm();

                            //    listDataRND1 = (from q in db.LG_RND1s
                            //                    where q.c_gdg == gudang
                            //                    && q.c_iteno == field.Item
                            //                    && q.c_batch == field.Batch
                            //                    && q.c_rnno.Substring(0, 4) == "RNXX"
                            //                    select q).Take(1).SingleOrDefault();

                            //    if (listDataRND1 != null)
                            //    {
                            //      ListRCD2.Add(new LG_RCD2()
                            //      {
                            //        c_batch = ListDO[0].c_batch,
                            //        c_dono = ListDO[0].c_dono,
                            //        c_gdg = gudang,
                            //        c_iteno = field.Item,
                            //        c_rcno = rcID,
                            //        c_rnno = listDataRND1.c_rnno,
                            //        c_type = field.TypeItem,
                            //        c_spno = "SPXXXXXXXX",
                            //        n_qty = field.Quantity,
                            //        n_sisa = field.Quantity
                            //      });
                            //    }
                            //    else
                            //    {
                            //      masitm = (from q in db.FA_MasItms
                            //                where q.c_iteno == field.Item
                            //                select q).Take(1).SingleOrDefault();

                            //      RNX = "RNXX" + gudang.ToString() + masitm.c_nosup;

                            //      ListRCD2.Add(new LG_RCD2()
                            //      {
                            //        c_batch = ListDO[0].c_batch,
                            //        c_dono = ListDO[0].c_dono,
                            //        c_gdg = gudang,
                            //        c_iteno = field.Item,
                            //        c_rcno = rcID,
                            //        c_rnno = RNX,
                            //        c_type = field.TypeItem,
                            //        c_spno = "SPXXXXXXXX",
                            //        n_qty = field.Quantity,
                            //        n_sisa = field.Quantity
                            //      });
                            //    }
                            //  }
                            //}
                            //else if (ListDO.Count <= 0)
                            //{
                            //  listDataRND1 = new LG_RND1();
                            //  masitm = new FA_MasItm();

                            //  listDataRND1 = (from q in db.LG_RND1s
                            //                  where q.c_gdg == gudang
                            //                  && q.c_iteno == field.Item
                            //                  && q.c_batch == field.Batch
                            //                  && q.c_rnno.Substring(0, 4) == "RNXX"
                            //                  select q).Take(1).SingleOrDefault();

                            //  if (listDataRND1 != null)
                            //  {
                            //    ListRCD2.Add(new LG_RCD2()
                            //    {
                            //      c_batch = field.Batch,
                            //      c_dono = field.NoDO,
                            //      c_gdg = gudang,
                            //      c_iteno = field.Item,
                            //      c_rcno = rcID,
                            //      c_rnno = listDataRND1.c_rnno,
                            //      c_type = field.TypeItem,
                            //      c_spno = "SPXXXXXXXX",
                            //      n_qty = field.Quantity,
                            //      n_sisa = field.Quantity
                            //    });
                            //  }
                            //  else
                            //  {
                            //    masitm = (from q in db.FA_MasItms
                            //              where q.c_iteno == field.Item
                            //              select q).Take(1).SingleOrDefault();

                            //    RNX = "RNXX" + gudang.ToString() + masitm.c_nosup;

                            //    ListRCD2.Add(new LG_RCD2()
                            //    {
                            //      c_batch = field.Batch,
                            //      c_dono = field.NoDO,
                            //      c_gdg = gudang,
                            //      c_iteno = field.Item,
                            //      c_rcno = rcID,
                            //      c_rnno = RNX,
                            //      c_type = field.TypeItem,
                            //      c_spno = "SPXXXXXXXX",
                            //      n_qty = field.Quantity,
                            //      n_sisa = field.Quantity
                            //    });
                            //  }
                            //}

                            //#endregion

                            #endregion
                        }

                        ListRNH.Clear();
                        ListRND1.Clear();
                        ListPLD2.Clear();
                        listMsItem.Clear();
                        listMsDisc.Clear();

                        #region Old Coded

                        //rch.c_pin = Pin.ToString();
                        ////db.SubmitChanges();

                        ////insert RCD3
                        //#region RCD3

                        //ListRCD3 = new List<LG_RCD3>();

                        //var RCD1 = ListRCD1.GroupBy(x => new { x.c_iteno }).Select(x => new { x.Key.c_iteno}).ToList();

                        //for (int LopRCD1 = 0; LopRCD1 < RCD1.Count; LopRCD1++)
                        //{
                        //  var varRCD3 = (from q1 in db.FA_MasItms
                        //                 join q3 in db.FA_DiscDs on q1.c_iteno equals q3.c_iteno
                        //                 where q3.c_nodisc == "DSXXXXXX03" &&
                        //                 q1.c_iteno == RCD1[LopRCD1].c_iteno
                        //                 select new
                        //                 {
                        //                   q1.c_iteno,
                        //                   n_disc = q3.n_discon,
                        //                   q1.n_salpri
                        //                 }).ToList();

                        //  for (int Rcd3 = 0; Rcd3 < varRCD3.Count; Rcd3++)
                        //  {
                        //    ListRCD3.Add(new LG_RCD3()
                        //    {
                        //      c_gdg = gudang,
                        //      c_rcno = rcID,
                        //      c_iteno = varRCD3[Rcd3].c_iteno,
                        //      n_disc = varRCD3[Rcd3].n_disc,
                        //      n_salpri = varRCD3[Rcd3].n_salpri
                        //    });
                        //  }
                        //  #region Old Code

                        //  //if (RCD1[LopRCD1].c_dono.Substring(0, 4).ToString() == "DOXX")
                        //  //{
                        //  //  var varRCD3 = (from q1 in db.FA_MasItms
                        //  //                 join q3 in db.FA_DiscDs on q1.c_iteno equals q3.c_iteno
                        //  //                 where q3.c_nodisc == "DSXXXXXX03" &&
                        //  //                 q1.c_iteno == RCD1[LopRCD1].c_iteno
                        //  //                 select new
                        //  //                 {
                        //  //                   q1.c_iteno,
                        //  //                   n_disc = q3.n_discon,
                        //  //                   q1.n_salpri
                        //  //                 }).ToList();

                        //  //  for (int Rcd3 = 0; Rcd3 < varRCD3.Count; Rcd3++)
                        //  //  {
                        //  //    ListRCD3.Add(new LG_RCD3()
                        //  //    {
                        //  //      c_gdg = gudang,
                        //  //      c_rcno = rcID,
                        //  //      c_iteno = varRCD3[Rcd3].c_iteno,
                        //  //      n_disc = varRCD3[Rcd3].n_disc,
                        //  //      n_salpri = varRCD3[Rcd3].n_salpri
                        //  //    });
                        //  //  }
                        //  //}
                        //  //else
                        //  //{
                        //  //  var varRCD3 = (from q in db.LG_FJD3s
                        //  //                 join q1 in db.LG_FJD2s on q.c_fjno equals q1.c_fjno
                        //  //                 join q2 in db.LG_FJD1s on new { q1.c_fjno, q1.c_iteno } equals new { q2.c_fjno, q2.c_iteno }
                        //  //                 where q.c_dono == RCD1[LopRCD1].c_dono &&
                        //  //                 q1.c_iteno == RCD1[LopRCD1].c_iteno
                        //  //                 select new
                        //  //                 {
                        //  //                   q1.c_iteno,
                        //  //                   n_disc = q1.n_discon,
                        //  //                   q2.n_salpri
                        //  //                 }).ToList();

                        //  //  for (int Rcd3 = 0; Rcd3 < varRCD3.Count; Rcd3++)
                        //  //  {
                        //  //    ListRCD3.Add(new LG_RCD3()
                        //  //    {
                        //  //      c_gdg = gudang,
                        //  //      c_rcno = rcID,
                        //  //      c_iteno = varRCD3[Rcd3].c_iteno,
                        //  //      n_disc = varRCD3[Rcd3].n_disc,
                        //  //      n_salpri = varRCD3[Rcd3].n_salpri
                        //  //    });
                        //  //  }
                        //  //}

                        //  #endregion
                        //}

                        //#endregion

                        //#region Update RN

                        //var varRCD2 = ListRCD2.ToList();
                        //int rnLoop = 0;

                        //if (varRCD2.Count > 0)
                        //{
                        //  rnhSingle = new LG_RNH();
                        //  rnd1 = new LG_RND1();
                        //  UListRND1 = new List<LG_RND1>();

                        //  for (rnLoop = 0; rnLoop < varRCD2.Count; rnLoop++)
                        //  {
                        //    var varListRNH = (from q in db.LG_RNHs
                        //                      where q.c_rnno == varRCD2[rnLoop].c_rnno
                        //                      && q.c_gdg == gudang
                        //                      && q.c_type == "01"
                        //                      select q).ToList();

                        //    if (varListRNH.Count > 0)
                        //    {
                        //      rnd1 = (from q in db.LG_RND1s
                        //              join q1 in db.LG_RNHs on new {q.c_gdg, q.c_rnno} equals new {q1.c_gdg, q1.c_rnno}
                        //              where q.c_rnno == varRCD2[rnLoop].c_rnno
                        //              && q.c_gdg == gudang
                        //              && q.c_iteno == varRCD2[rnLoop].c_iteno
                        //              && q.c_batch == varRCD2[rnLoop].c_batch
                        //              && q1.c_type == "01"
                        //              select q).Take(1).SingleOrDefault();

                        //      if (rnd1 != null)
                        //      {
                        //        if (field.TypeItem == "01")
                        //        {
                        //          rnd1.n_gsisa += varRCD2[rnLoop].n_qty;
                        //        }
                        //        else
                        //        {
                        //          rnd1.n_bsisa += varRCD2[rnLoop].n_qty;
                        //        }
                        //      }
                        //      else
                        //      {
                        //        if (field.TypeItem == "01")
                        //        {
                        //          UListRND1.Add(new LG_RND1()
                        //          {
                        //            c_batch = varRCD2[rnLoop].c_batch,
                        //            c_gdg = gudang,
                        //            c_iteno = varRCD2[rnLoop].c_iteno,
                        //            c_rnno = varRCD2[rnLoop].c_rnno,
                        //            n_bqty = 0,
                        //            n_bsisa = 0,
                        //            n_gqty = 0,
                        //            n_gsisa = varRCD2[rnLoop].n_qty
                        //          });
                        //        }
                        //        else
                        //        {
                        //          UListRND1.Add(new LG_RND1()
                        //          {
                        //            c_batch = varRCD2[rnLoop].c_batch,
                        //            c_gdg = gudang,
                        //            c_iteno = varRCD2[rnLoop].c_iteno,
                        //            c_rnno = varRCD2[rnLoop].c_rnno,
                        //            n_bqty = 0,
                        //            n_bsisa = varRCD2[rnLoop].n_qty,
                        //            n_gqty = 0,
                        //            n_gsisa = 0
                        //          });
                        //        }
                        //      }
                        //    }
                        //    else
                        //    {
                        //      var sup = (from q in db.FA_MasItms
                        //                 where q.c_iteno == varRCD2[rnLoop].c_iteno
                        //                 select q).Take(1).SingleOrDefault();

                        //      string rnno = "RNXX" + gudang.ToString() + sup.c_nosup.ToString();

                        //      var a = (from q in db.LG_RNHs
                        //               where q.c_rnno == rnno && q.c_type == "01"
                        //               && q.c_gdg == gudang
                        //               select q).Take(1).SingleOrDefault();

                        //      var CekRN1 = (from q in db.LG_RNHs
                        //                    where q.c_rnno == rnno && q.c_type == "01"
                        //                    && q.c_gdg == gudang
                        //                    select q).Take(1).SingleOrDefault();

                        //      if (CekRN1 != null)
                        //      {
                        //        rnd1 = (from q in db.LG_RND1s
                        //                where q.c_rnno == CekRN1.c_rnno
                        //               && q.c_gdg == gudang
                        //               && q.c_iteno == varRCD2[rnLoop].c_iteno
                        //               && q.c_batch == varRCD2[rnLoop].c_batch
                        //                select q).Take(1).SingleOrDefault();



                        //        if (rnd1 != null)
                        //        {
                        //          if (field.TypeItem == "01")
                        //          {
                        //            rnd1.n_gsisa += varRCD2[rnLoop].n_qty;
                        //          }
                        //          else
                        //          {
                        //            rnd1.n_bsisa += varRCD2[rnLoop].n_qty;
                        //          }
                        //        }
                        //        else
                        //        {
                        //          if (field.TypeItem == "01")
                        //          {
                        //            UListRND1.Add(new LG_RND1()
                        //            {
                        //              c_batch = varRCD2[rnLoop].c_batch,
                        //              c_gdg = gudang,
                        //              c_iteno = varRCD2[rnLoop].c_iteno,
                        //              c_rnno = CekRN1.c_rnno,
                        //              n_bqty = 0,
                        //              n_bsisa = 0,
                        //              n_gqty = 0,
                        //              n_gsisa = varRCD2[rnLoop].n_qty
                        //            });
                        //          }
                        //          else
                        //          {
                        //            UListRND1.Add(new LG_RND1()
                        //            {
                        //              c_batch = varRCD2[rnLoop].c_batch,
                        //              c_gdg = gudang,
                        //              c_iteno = varRCD2[rnLoop].c_iteno,
                        //              c_rnno = CekRN1.c_rnno,
                        //              n_bqty = 0,
                        //              n_bsisa = varRCD2[rnLoop].n_qty,
                        //              n_gqty = 0,
                        //              n_gsisa = 0
                        //            });
                        //          }
                        //        }

                        //      }
                        //      else
                        //      {
                        //        rnhSingle = new LG_RNH()
                        //        {
                        //          c_dono = varRCD2[rnLoop].c_dono,
                        //          c_from = sup.c_nosup,
                        //          c_gdg = gudang,
                        //          c_entry = structure.Fields.Entry,
                        //          c_rnno = rnno,
                        //          c_type = "01",
                        //          c_update = structure.Fields.Entry,
                        //          d_dodate = date,
                        //          d_entry = date,
                        //          d_rndate = date,
                        //          d_update = date,
                        //          l_delete = false,
                        //          l_float = false,
                        //          l_print = false,
                        //          l_status = false,
                        //          n_bea = 0,
                        //          v_ket = "RN RC"
                        //        };

                        //        //for (int nLoopRCD1 = 0; nLoopRCD1 < ListRCD1.Count; nLoopRCD1++)
                        //        //{
                        //        //  if (ListRCD1[nLoopRCD1].c_batch == varRCD2[rnLoop].c_batch
                        //        //    && ListRCD1[nLoopRCD1].c_gdg == varRCD2[rnLoop].c_gdg
                        //        //    && ListRCD1[nLoopRCD1].c_iteno == varRCD2[rnLoop].c_iteno)
                        //        //  {
                        //        //    ListRCD1[nLoopRCD1].c_rnno.Replace(ListRCD1[nLoopRCD1].c_rnno.ToString(), rnno);
                        //        //  }
                        //        //}

                        //        if (field.TypeItem == "01")
                        //        {
                        //          UListRND1.Add(new LG_RND1()
                        //          {
                        //            c_batch = varRCD2[rnLoop].c_batch,
                        //            c_gdg = gudang,
                        //            c_iteno = varRCD2[rnLoop].c_iteno,
                        //            c_rnno = rnno,
                        //            n_bqty = 0,
                        //            n_bsisa = 0,
                        //            n_gqty = 0,
                        //            n_gsisa = varRCD2[rnLoop].n_qty
                        //          });
                        //        }
                        //        else
                        //        {
                        //          UListRND1.Add(new LG_RND1()
                        //          {
                        //            c_batch = varRCD2[rnLoop].c_batch,
                        //            c_gdg = gudang,
                        //            c_iteno = varRCD2[rnLoop].c_iteno,
                        //            c_rnno = rnno,
                        //            n_bqty = 0,
                        //            n_bsisa = varRCD2[rnLoop].n_qty,
                        //            n_gqty = 0,
                        //            n_gsisa = 0
                        //          });
                        //        }
                        //      }
                        //    }
                        //  }
                        //}

                        //#endregion

                        //if (rnhSingle.c_rnno != null)
                        //{
                        //  db.LG_RNHs.InsertOnSubmit(rnhSingle);
                        //}
                        //if (UListRND1.Count > 0)
                        //{
                        //  db.LG_RND1s.InsertAllOnSubmit(UListRND1.ToArray());
                        //  UListRND1.Clear();
                        //}

                        #endregion

                        if ((ListRCD1 != null) && (ListRCD1.Count > 0))
                        {
                            db.LG_RCD1s.InsertAllOnSubmit(ListRCD1.ToArray());
                            ListRCD1.Clear();
                        }

                        if ((ListRCD2 != null) && (ListRCD2.Count > 0))
                        {
                            db.LG_RCD2s.InsertAllOnSubmit(ListRCD2.ToArray());
                            ListRCD2.Clear();
                        }

                        if ((ListRCD3 != null) && (ListRCD3.Count > 0))
                        {
                            db.LG_RCD3s.InsertAllOnSubmit(ListRCD3.ToArray());
                            ListRCD3.Clear();
                        }
                    }

                    #endregion

                    dic = new Dictionary<string, string>();

                    if (totalDetails > 0)
                    {
                        dic.Add("rcID", rcID);
                        //dic.Add("Pin", Pin.ToString());
                        dic.Add("Pin", pinCode);
                        dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                        result = string.Format("Total {0} detail(s)", totalDetails);

                        hasAnyChanges = true;
                    }

                    #endregion
                }
                else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    if (string.IsNullOrEmpty(rcID))
                    {
                        result = "Nomor RC dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    rch = (from q in db.LG_RCHes
                           where q.c_rcno == rcID
                           select q).Take(1).SingleOrDefault();

                    if (rch == null)
                    {
                        result = "Nomor RC tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (rch.l_delete.HasValue && rch.l_delete.Value)
                    {
                        result = "Tidak dapat menghapus nomor RC yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.HasFJR(db, rcID))
                    {
                        result = "RC yang sudah terdapat Faktur tidak dapat diubah.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, rch.d_rcdate))
                    {
                        result = "Retur Customer tidak dapat diubah, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (rch.l_sent.HasValue && rch.l_sent.Value)
                    {
                        result = "Retur Customer tidak dapat diubah, karena sudah di kirim.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
                    {
                        rch.v_ket = structure.Fields.Keterangan;
                    }

                    rch.c_update = nipEntry;
                    rch.d_update = DateTime.Now;

                    #region Detil

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        ListRCD4 = new List<LG_RCD4>();

                        #region Prepare Data

                        lstTemp = structure.Fields.Field.GroupBy(x => x.Item).Select(y => y.Key).ToList();

                        listMsItem = (from q in db.FA_MasItms
                                      where lstTemp.Contains(q.c_iteno)
                                      select q).Distinct().ToList();

                        listMsDisc = (from q in db.FA_DiscDs
                                      where lstTemp.Contains(q.c_iteno)
                                      && q.c_nodisc == "DSXXXXXX03"
                                      select q).Distinct().ToList();

                        lstTemp.Clear();

                        lstTemp = structure.Fields.Field.GroupBy(x => x.NoDO).Select(y => y.Key).ToList();
                        lstTemp1 = structure.Fields.Field.GroupBy(x => x.NoRN).Select(y => y.Key).ToList();

                        ListRNH = (from q in db.LG_RNHs
                                   where lstTemp1.Contains(q.c_rnno) && (q.c_gdg == gudang)
                                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                   select q).Distinct().ToList();

                        ListRND1 = (from q in db.LG_RNHs
                                    join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                    where lstTemp1.Contains(q.c_rnno) && (q.c_gdg == gudang)
                                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                    select q1).Distinct().ToList();

                        ListPLD2 = (from q in db.LG_DOHs
                                    join q1 in db.LG_PLHs on q.c_plno equals q1.c_plno
                                    join q2 in db.LG_PLD2s on q.c_plno equals q2.c_plno
                                    where lstTemp.Contains(q.c_dono) && lstTemp1.Contains(q2.c_rnno)
                                      && (q.c_gdg == gudang) && (q.c_cusno == structure.Fields.Customer)
                                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                      && ((q1.l_delete.HasValue ? q1.l_delete.Value : false) == false)
                                    select q2).Distinct().ToList();

                        lstTemp1.Clear();
                        lstTemp.Clear();

                        ListRCD1 = (from q in db.LG_RCD1s
                                    where (q.c_gdg == gudang) && (q.c_rcno == structure.Fields.ReturCustomerID)
                                    select q).ToList();

                        ListRCD2 = (from q in db.LG_RCD2s
                                    where (q.c_gdg == gudang) && (q.c_rcno == structure.Fields.ReturCustomerID)
                                    select q).ToList();

                        ListRCD3 = (from q in db.LG_RCD3s
                                    where (q.c_gdg == gudang) && (q.c_rcno == structure.Fields.ReturCustomerID)
                                    select q).ToList();

                        #endregion

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if ((field != null) && field.IsNew && (field.Quantity > 0) && (!string.IsNullOrEmpty(field.Batch)) && (field.Batch.Trim().Length > 0))
                            {
                                #region New

                                if (Commons.CheckAndProcessBatch(db, field.Item, field.Batch, date, nipEntry) > 0)
                                {
                                    rcd1 = new LG_RCD1()
                                    {
                                        c_gdg = gudang,
                                        c_rcno = rcID,
                                        c_iteno = field.Item,
                                        c_batch = field.Batch,
                                        c_type = field.TypeItem,
                                        c_dono = field.NoDO,
                                        c_rnno = field.NoRN,
                                        n_qty = field.Quantity,
                                        n_sisa = field.Quantity,
                                        n_qtyrcv = field.Quantity,
                                        n_qtydestroy = 0
                                    };

                                    rcd2 = new LG_RCD2()
                                    {
                                        c_gdg = gudang,
                                        c_rcno = rcID,
                                        c_iteno = field.Item,
                                        c_batch = field.Batch,
                                        c_type = field.TypeItem,
                                        c_dono = field.NoDO,
                                        c_rnno = field.NoRN,
                                        c_spno = "SPXXXXXXXX",
                                        n_qty = field.Quantity,
                                        n_sisa = field.Quantity
                                    };

                                    masitm = listMsItem.Find(delegate(FA_MasItm itm)
                                    {
                                        return field.Item.Equals((string.IsNullOrEmpty(itm.c_iteno) ? string.Empty : itm.c_iteno), StringComparison.OrdinalIgnoreCase);
                                    });

                                    masdisc = listMsDisc.Find(delegate(FA_DiscD itm)
                                    {
                                        return field.Item.Equals((string.IsNullOrEmpty(itm.c_iteno) ? string.Empty : itm.c_iteno), StringComparison.OrdinalIgnoreCase);
                                    });

                                    if (field.NoDO.Equals("DOXXXXXXXX", StringComparison.OrdinalIgnoreCase))
                                    {
                                        #region DO Tidak Di isi

                                        //pld2 = ListPLD2.Find(delegate(LG_PLD2 pld)
                                        //{
                                        //  return field.NoRN.Equals((string.IsNullOrEmpty(pld.c_rnno) ? string.Empty : pld.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                        //    field.Batch.Equals((string.IsNullOrEmpty(pld.c_batch) ? string.Empty : pld.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                        //    field.Item.Equals((string.IsNullOrEmpty(pld.c_iteno) ? string.Empty : pld.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase);
                                        //});

                                        if (masitm != null)
                                        {
                                            rnTempId = string.Concat("RNXX", gudang, masitm.c_nosup);

                                            rnh = ListRNH.Find(delegate(LG_RNH rn)
                                            {
                                                return rnTempId.Equals((string.IsNullOrEmpty(rn.c_rnno) ? string.Empty : rn.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
                                            });

                                            rnd1 = ListRND1.Find(delegate(LG_RND1 rnd)
                                            {
                                                return rnTempId.Equals((string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno), StringComparison.OrdinalIgnoreCase) &&
                                                  field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno), StringComparison.OrdinalIgnoreCase) &&
                                                  field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch), StringComparison.OrdinalIgnoreCase);
                                            });

                                            if (!ListRCD3.Exists(delegate(LG_RCD3 rcd)
                                            {
                                                return field.Item.Equals((string.IsNullOrEmpty(rcd.c_iteno) ? string.Empty : rcd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase);
                                            }))
                                            {
                                                rcd3 = new LG_RCD3()
                                                {
                                                    c_gdg = gudang,
                                                    c_rcno = rcID,
                                                    c_iteno = field.Item,
                                                    n_disc = masdisc.n_discon,
                                                    n_salpri = masitm.n_salpri
                                                };
                                            }
                                            else
                                            {
                                                rcd3 = null;
                                            }

                                            #region RN Header

                                            if (rnh == null)
                                            {
                                                rnh = (from q in db.LG_RNHs
                                                       where (q.c_gdg == gudang) && (q.c_rnno == rnTempId)
                                                        && (q.c_dono == field.NoDO)
                                                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                                       select q).Take(1).SingleOrDefault();
                                            }

                                            if (rnh == null)
                                            {
                                                rnh = new LG_RNH()
                                                {
                                                    c_gdg = gudang,
                                                    c_rnno = rnTempId,
                                                    d_rndate = date,
                                                    c_type = "07",
                                                    l_float = false,
                                                    c_dono = field.NoDO,
                                                    d_dodate = date,
                                                    v_ket = "Auto Generate",
                                                    c_from = masitm.c_nosup,
                                                    n_bea = 0,
                                                    l_print = false,
                                                    l_status = false,
                                                    c_entry = "auto",
                                                    d_entry = date,
                                                    c_update = "auto",
                                                    d_update = date
                                                };

                                                db.LG_RNHs.InsertOnSubmit(rnh);

                                                ListRNH.Add(rnh);
                                            }

                                            #endregion

                                            #region RN Detail

                                            if (rnd1 == null)
                                            {
                                                rnd1 = (from q in db.LG_RND1s
                                                        where (q.c_gdg == gudang) && (q.c_rnno == rnTempId)
                                                         && (q.c_iteno == field.Item) && (q.c_batch == field.Batch)
                                                        select q).Take(1).SingleOrDefault();
                                            }

                                            if (rnd1 == null)
                                            {
                                                rnd1 = new LG_RND1()
                                                {
                                                    c_gdg = gudang,
                                                    c_rnno = rnTempId,
                                                    c_iteno = field.Item,
                                                    c_batch = field.Batch,
                                                    n_gqty = 0,
                                                    n_bqty = 0,
                                                    n_gsisa = 0,
                                                    n_bsisa = 0
                                                };

                                                db.LG_RND1s.InsertOnSubmit(rnd1);

                                                ListRND1.Add(rnd1);
                                            }

                                            #endregion

                                            if ((rnh != null) && (rnd1 != null))
                                            {
                                                if (field.TypeItem.Equals("01", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    rnd1.n_gsisa += field.Quantity;
                                                }
                                                else
                                                {
                                                    rnd1.n_bsisa += field.Quantity;
                                                }

                                                rcd1.c_rnno = rcd2.c_rnno = rnTempId;

                                                ListRCD4.Add(new LG_RCD4()
                                                {
                                                    c_rcno = rcID,
                                                    c_iteno = field.Item,
                                                    c_batch = field.Batch,
                                                    c_type = field.TypeItem,
                                                    c_dono = field.NoDO,
                                                    c_rnno = rnTempId,
                                                    c_spno = "SPXXXXXXXX",
                                                    n_qty = field.Quantity,
                                                    n_sisa = field.Quantity,
                                                    c_entry = nipEntry,
                                                    d_entry = date,
                                                    v_type = "01"
                                                });

                                                ListRCD1.Add(rcd1);
                                                ListRCD2.Add(rcd2);

                                                db.LG_RCD1s.InsertOnSubmit(rcd1);
                                                db.LG_RCD2s.InsertOnSubmit(rcd2);
                                                if (rcd3 != null)
                                                {
                                                    db.LG_RCD3s.InsertOnSubmit(rcd3);
                                                    ListRCD3.Add(rcd3);
                                                }

                                                totalDetails++;
                                            }
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        #region DO Diisi

                                        //rnh = ListRNH.Find(delegate(LG_RNH rn)
                                        //{
                                        //  return field.NoRN.Equals((string.IsNullOrEmpty(rn.c_rnno) ? string.Empty : rn.c_rnno), StringComparison.OrdinalIgnoreCase);
                                        //});

                                        rnh = ListRNH.Find(delegate(LG_RNH rn)
                                        {
                                            return field.NoRN.Equals((string.IsNullOrEmpty(rn.c_rnno) ? string.Empty : rn.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
                                        });

                                        rnd1 = ListRND1.Find(delegate(LG_RND1 rnd)
                                        {
                                            return field.NoRN.Equals((string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              field.Item.Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              field.Batch.Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                                        });

                                        pld2 = ListPLD2.Find(delegate(LG_PLD2 pld)
                                        {
                                            return field.NoRN.Equals((string.IsNullOrEmpty(pld.c_rnno) ? string.Empty : pld.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              field.Batch.Equals((string.IsNullOrEmpty(pld.c_batch) ? string.Empty : pld.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              field.Item.Equals((string.IsNullOrEmpty(pld.c_iteno) ? string.Empty : pld.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase);
                                        });

                                        if ((masitm != null) && (pld2 != null))
                                        {
                                            rcd2.c_spno = pld2.c_spno;

                                            if (!ListRCD3.Exists(delegate(LG_RCD3 rcd)
                                            {
                                                return field.Item.Equals((string.IsNullOrEmpty(rcd.c_iteno) ? string.Empty : rcd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase);
                                            }))
                                            {
                                                rcd3 = new LG_RCD3()
                                                {
                                                    c_gdg = gudang,
                                                    c_rcno = rcID,
                                                    c_iteno = field.Item,
                                                    n_disc = masdisc.n_discon,
                                                    n_salpri = masitm.n_salpri
                                                };
                                            }
                                            else
                                            {
                                                rcd3 = null;
                                            }

                                            if ((rnh != null) && (rnd1 != null))
                                            {
                                                if (field.TypeItem.Equals("01", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    rnd1.n_gsisa += field.Quantity;
                                                }
                                                else
                                                {
                                                    rnd1.n_bsisa += field.Quantity;
                                                }

                                                ListRCD4.Add(new LG_RCD4()
                                                {
                                                    c_rcno = rcID,
                                                    c_iteno = field.Item,
                                                    c_batch = field.Batch,
                                                    c_type = field.TypeItem,
                                                    c_dono = field.NoDO,
                                                    c_rnno = field.NoRN,
                                                    c_spno = rcd2.c_spno,
                                                    n_qty = field.Quantity,
                                                    n_sisa = field.Quantity,
                                                    c_entry = nipEntry,
                                                    d_entry = date,
                                                    v_type = "01"
                                                });

                                                ListRCD1.Add(rcd1);
                                                ListRCD2.Add(rcd2);

                                                db.LG_RCD1s.InsertOnSubmit(rcd1);
                                                db.LG_RCD2s.InsertOnSubmit(rcd2);
                                                if (rcd3 != null)
                                                {
                                                    db.LG_RCD3s.InsertOnSubmit(rcd3);
                                                    ListRCD3.Add(rcd3);
                                                }

                                                totalDetails++;
                                            }
                                        }

                                        #endregion
                                    }
                                }

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

                                #endregion
                            }
                            else if ((field != null) && field.IsDelete && (field.Quantity > 0) && (!string.IsNullOrEmpty(field.Batch)) && (field.Batch.Trim().Length > 0))
                            {
                                #region Delete

                                //rcd1 = ListRCD1[nLoop];

                                rcd1 = ListRCD1.Find(delegate(LG_RCD1 rcd)
                                {
                                    return field.Item.Equals((string.IsNullOrEmpty(rcd.c_iteno) ? string.Empty : rcd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                      field.Batch.Equals((string.IsNullOrEmpty(rcd.c_batch) ? string.Empty : rcd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                      field.NoDO.Equals((string.IsNullOrEmpty(rcd.c_dono) ? string.Empty : rcd.c_dono.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                      field.NoRN.Equals((string.IsNullOrEmpty(rcd.c_rnno) ? string.Empty : rcd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
                                });

                                if (rcd1 != null)
                                {
                                    typeItem = (string.IsNullOrEmpty(rcd1.c_type) ? "02" : rcd1.c_type);

                                    nQtyAlloc = (rcd1.n_sisa.HasValue ? rcd1.n_sisa.Value : 0);
                                    nQtyAlocate = (rcd1.n_qty.HasValue ? rcd1.n_qty.Value : 0);

                                    if (nQtyAlloc != nQtyAlocate)
                                    {
                                        result = "Retur Customer tidak dapat dihapus, karena salah satu sisa detail telah dipergunakan.";

                                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                        if (db.Transaction != null)
                                        {
                                            db.Transaction.Rollback();
                                        }

                                        goto endLogic;
                                    }

                                    rcd2 = ListRCD2.Find(delegate(LG_RCD2 rcd)
                                    {
                                        //return (string.IsNullOrEmpty(rcd1.c_iteno) ? string.Empty : rcd1.c_iteno.Trim()).Equals((string.IsNullOrEmpty(rcd.c_iteno) ? string.Empty : rcd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                        //  (string.IsNullOrEmpty(rcd1.c_batch) ? string.Empty : rcd1.c_batch.Trim()).Equals((string.IsNullOrEmpty(rcd.c_batch) ? string.Empty : rcd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                        //  (string.IsNullOrEmpty(rcd1.c_dono) ? string.Empty : rcd1.c_dono.Trim()).Equals((string.IsNullOrEmpty(rcd.c_dono) ? string.Empty : rcd.c_dono.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                        //  (string.IsNullOrEmpty(rcd1.c_rnno) ? string.Empty : rcd1.c_rnno.Trim()).Equals((string.IsNullOrEmpty(rcd.c_rnno) ? string.Empty : rcd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);

                                        return field.Item.Equals((string.IsNullOrEmpty(rcd.c_iteno) ? string.Empty : rcd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                          field.Batch.Equals((string.IsNullOrEmpty(rcd.c_batch) ? string.Empty : rcd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                          field.NoDO.Equals((string.IsNullOrEmpty(rcd.c_dono) ? string.Empty : rcd.c_dono.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                          field.NoRN.Equals((string.IsNullOrEmpty(rcd.c_rnno) ? string.Empty : rcd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
                                    });

                                    if (rcd2 != null)
                                    {
                                        rnd1 = ListRND1.Find(delegate(LG_RND1 rnd)
                                        {
                                            return (string.IsNullOrEmpty(rcd1.c_iteno) ? string.Empty : rcd1.c_iteno.Trim()).Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              (string.IsNullOrEmpty(rcd1.c_batch) ? string.Empty : rcd1.c_batch.Trim()).Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                              (string.IsNullOrEmpty(rcd1.c_rnno) ? string.Empty : rcd1.c_rnno.Trim()).Equals((string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
                                        });

                                        if (rnd1 != null)
                                        {
                                            if (typeItem.Equals("01", StringComparison.OrdinalIgnoreCase))
                                            {
                                                rnd1.n_gsisa -= nQtyAlloc;
                                            }
                                            else
                                            {
                                                rnd1.n_bsisa -= nQtyAlloc;
                                            }
                                        }

                                        ListRCD4.Add(new LG_RCD4()
                                        {
                                            c_rcno = rcID,
                                            c_iteno = rcd1.c_iteno,
                                            c_batch = rcd1.c_batch,
                                            c_dono = rcd1.c_dono,
                                            c_rnno = rcd1.c_rnno,
                                            c_type = rcd1.c_type,
                                            n_qty = rcd1.n_qty,
                                            n_sisa = rcd1.n_sisa,
                                            c_spno = (rcd2 != null ? rcd2.c_spno : string.Empty),
                                            c_entry = nipEntry,
                                            d_entry = date,
                                            v_ket_del = field.Keterangan,
                                            v_type = "03"
                                        });

                                        db.LG_RCD1s.DeleteOnSubmit(rcd1);
                                        db.LG_RCD2s.DeleteOnSubmit(rcd2);

                                        ListRCD1.Remove(rcd1);
                                        ListRCD2.Remove(rcd2);

                                        if (!ListRCD1.Exists(delegate(LG_RCD1 rcd)
                                        {
                                            return field.Item.Equals((string.IsNullOrEmpty(rcd.c_iteno) ? string.Empty : rcd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase);
                                        }))
                                        {
                                            //rcd3 = 

                                            rcd3 = ListRCD3.Find(delegate(LG_RCD3 rcd)
                                            {
                                                return (string.IsNullOrEmpty(rcd1.c_iteno) ? string.Empty : rcd1.c_iteno.Trim()).Equals((string.IsNullOrEmpty(rcd.c_iteno) ? string.Empty : rcd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase);
                                            });

                                            if (rcd3 != null)
                                            {
                                                db.LG_RCD3s.DeleteOnSubmit(rcd3);
                                            }
                                        }
                                    }

                                    #region BASPB
                                    if (!string.IsNullOrEmpty(structure.Fields.BaspbNo))
                                    {
                                        baspbd = (from q in db.SCMS_BASPBDs
                                                  where q.c_baspbno == structure.Fields.BaspbNo
                                                        && q.c_iteno == field.Item
                                                  select q).Take(1).SingleOrDefault();

                                        if (baspbd != null)
                                        {
                                            baspbd.n_gsisa += rcd1.n_qty;
                                        }
                                    }
                                    #endregion
                                }

                                #endregion
                            }
                        }

                        if (ListRCD4.Count > 0)
                        {
                            db.LG_RCD4s.InsertAllOnSubmit(ListRCD4.ToArray());
                            ListRCD4.Clear();
                        }
                    }

                    #endregion

                    hasAnyChanges = true;

                    #endregion
                }
                else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    if (string.IsNullOrEmpty(rcID))
                    {
                        result = "Nomor RC dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        goto endLogic;
                    }

                    rch = (from q in db.LG_RCHes
                           where (q.c_rcno == rcID) && (q.c_gdg == gudang)
                           select q).Take(1).SingleOrDefault();

                    if (rcID == null)
                    {
                        result = "Nomor RC tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (rch.l_delete.HasValue && rch.l_delete.Value)
                    {
                        result = "Tidak dapat menghapus nomor RC yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.HasFJR(db, rcID))
                    {
                        result = "RC yang sudah terdapat Faktur tidak dapat dihapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, rch.d_rcdate))
                    {
                        result = "Retur Customer tidak dapat dihapus, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (rch.l_sent.HasValue && rch.l_sent.Value)
                    {
                        result = "Retur Customer tidak dapat di hapus, karena sudah di kirim.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    rch.c_update = nipEntry;
                    rch.d_update = DateTime.Now;

                    rch.l_delete = true;
                    rch.v_ket_mark = structure.Fields.Keterangan;

                    #region Detil

                    ListRCD4 = new List<LG_RCD4>();

                    ListRCD1 = (from q in db.LG_RCD1s
                                where (q.c_gdg == gudang) && (q.c_rcno == structure.Fields.ReturCustomerID)
                                select q).ToList();

                    ListRCD2 = (from q in db.LG_RCD2s
                                where (q.c_gdg == gudang) && (q.c_rcno == structure.Fields.ReturCustomerID)
                                select q).ToList();

                    ListRCD3 = (from q in db.LG_RCD3s
                                where (q.c_gdg == gudang) && (q.c_rcno == structure.Fields.ReturCustomerID)
                                select q).ToList();

                    lstTemp = ListRCD1.GroupBy(x => x.c_rnno).Select(y => (string.IsNullOrEmpty(y.Key) ? string.Empty : y.Key.Trim())).ToList();

                    ListRND1 = (from q in db.LG_RNHs
                                join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                where (q.c_gdg == gudang) && lstTemp.Contains(q.c_rnno)
                                select q1).ToList();

                    for (nLoop = 0; nLoop < ListRCD1.Count; nLoop++)
                    {
                        rcd1 = ListRCD1[nLoop];

                        typeItem = (string.IsNullOrEmpty(rcd1.c_type) ? "02" : rcd1.c_type);

                        nQtyAlloc = (rcd1.n_sisa.HasValue ? rcd1.n_sisa.Value : 0);
                        nQtyAlocate = (rcd1.n_qty.HasValue ? rcd1.n_qty.Value : 0);

                        if (nQtyAlloc != nQtyAlocate)
                        {
                            result = "Retur Customer tidak dapat dihapus, karena salah satu sisa detail telah dipergunakan.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }

                        rcd2 = ListRCD2.Find(delegate(LG_RCD2 rcd)
                        {
                            return (string.IsNullOrEmpty(rcd1.c_iteno) ? string.Empty : rcd1.c_iteno.Trim()).Equals((string.IsNullOrEmpty(rcd.c_iteno) ? string.Empty : rcd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                              (string.IsNullOrEmpty(rcd1.c_batch) ? string.Empty : rcd1.c_batch.Trim()).Equals((string.IsNullOrEmpty(rcd.c_batch) ? string.Empty : rcd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                              (string.IsNullOrEmpty(rcd1.c_dono) ? string.Empty : rcd1.c_dono.Trim()).Equals((string.IsNullOrEmpty(rcd.c_dono) ? string.Empty : rcd.c_dono.Trim()), StringComparison.OrdinalIgnoreCase) &&
                              (string.IsNullOrEmpty(rcd1.c_rnno) ? string.Empty : rcd1.c_rnno.Trim()).Equals((string.IsNullOrEmpty(rcd.c_rnno) ? string.Empty : rcd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
                        });

                        if (rcd2 != null)
                        {
                            rnd1 = ListRND1.Find(delegate(LG_RND1 rnd)
                            {
                                return (string.IsNullOrEmpty(rcd1.c_iteno) ? string.Empty : rcd1.c_iteno.Trim()).Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                  (string.IsNullOrEmpty(rcd1.c_batch) ? string.Empty : rcd1.c_batch.Trim()).Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                                  (string.IsNullOrEmpty(rcd1.c_rnno) ? string.Empty : rcd1.c_rnno.Trim()).Equals((string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase);
                            });

                            if (rnd1 != null)
                            {
                                if (typeItem.Equals("01", StringComparison.OrdinalIgnoreCase))
                                {
                                    rnd1.n_gsisa -= nQtyAlloc;
                                }
                                else
                                {
                                    rnd1.n_bsisa -= nQtyAlloc;
                                }
                            }
                        }

                        ListRCD4.Add(new LG_RCD4()
                        {
                            c_rcno = rcID,
                            c_iteno = rcd1.c_iteno,
                            c_batch = rcd1.c_batch,
                            c_dono = rcd1.c_dono,
                            c_rnno = rcd1.c_rnno,
                            c_type = rcd1.c_type,
                            n_qty = rcd1.n_qty,
                            n_sisa = rcd1.n_sisa,
                            c_spno = (rcd2 != null ? rcd2.c_spno : string.Empty),
                            c_entry = nipEntry,
                            d_entry = date,
                            v_ket_del = structure.Fields.Keterangan,
                            v_type = "03"
                        });

                        #region BASPB
                        if (!string.IsNullOrEmpty(rch.c_baspbno))
                        {
                            baspbd = (from q in db.SCMS_BASPBDs
                                      where q.c_baspbno == rch.c_baspbno
                                            && q.c_iteno == rcd1.c_iteno
                                      select q).Take(1).SingleOrDefault();

                            if (baspbd != null)
                            {
                                baspbd.n_gsisa += rcd1.n_qty;
                            }
                        }
                        #endregion
                    }

                    db.LG_RCD1s.DeleteAllOnSubmit(ListRCD1.ToArray());
                    db.LG_RCD2s.DeleteAllOnSubmit(ListRCD2.ToArray());
                    db.LG_RCD3s.DeleteAllOnSubmit(ListRCD3.ToArray());

                    db.LG_RCD4s.InsertAllOnSubmit(ListRCD4.ToArray());

                    #region Old Coded

                    //var RCCount = (from q in db.LG_RCD1s
                    //               where q.c_gdg == gudang && q.c_rcno == rcID
                    //               select q).AsQueryable().ToList();

                    //for (nLoop = 0; nLoop < RCCount.Count; nLoop++)
                    //{
                    //  #region update RN

                    //  var RCQty = (from q in db.LG_RCD1s
                    //               where q.c_gdg == gudang && q.c_rcno == rcID
                    //               && q.c_rnno == RCCount[nLoop].c_rnno
                    //               && q.c_dono == RCCount[nLoop].c_dono
                    //               && q.c_batch == RCCount[nLoop].c_batch
                    //               && q.c_iteno == RCCount[nLoop].c_iteno
                    //               && q.c_type == RCCount[nLoop].c_type
                    //               select q).ToList();

                    //  var updateRN = (from q in db.LG_RND1s
                    //                  where q.c_gdg == gudang &&
                    //                  q.c_rnno == RCCount[nLoop].c_rnno
                    //                  && q.c_batch == RCCount[nLoop].c_batch
                    //                  && q.c_iteno == RCCount[nLoop].c_iteno
                    //                  select q).Take(1).SingleOrDefault();

                    //  if (updateRN.c_iteno.Length > 0)
                    //  {
                    //    if (RCQty[nLoop].c_type.Equals("01"))
                    //    {
                    //      updateRN.n_gsisa += RCQty[nLoop].n_qty;
                    //    }
                    //    else
                    //    {
                    //      updateRN.n_bsisa += RCQty[nLoop].n_qty;
                    //    }
                    //  }

                    //  #endregion

                    //  #region insert RCD4

                    //  var RC2Count = (from q in db.LG_RCD2s
                    //                  where q.c_gdg == gudang && q.c_rcno == rcID
                    //                  && q.c_rnno == RCCount[nLoop].c_rnno
                    //                  && q.c_iteno == RCCount[nLoop].c_iteno
                    //                  && q.c_dono == RCCount[nLoop].c_dono
                    //                  && q.c_batch == RCCount[nLoop].c_batch
                    //                  && q.c_type == RCCount[nLoop].c_type
                    //                  select q).AsQueryable().ToList();

                    //  for (nLoopC = 0; nLoopC < RC2Count.Count; nLoopC++)
                    //  {
                    //    ListRCD4.Add(new LG_RCD4()
                    //    {
                    //      c_batch = RC2Count[nLoop].c_batch,
                    //      c_dono = RC2Count[nLoop].c_dono,
                    //      c_entry = structure.Fields.Entry,
                    //      c_type = RC2Count[nLoop].c_type,
                    //      c_spno = RC2Count[nLoop].c_spno,
                    //      c_rnno = RC2Count[nLoop].c_rnno,
                    //      c_iteno = RC2Count[nLoop].c_iteno,
                    //      c_rcno = RC2Count[nLoop].c_rcno,
                    //      d_entry = DateTime.Now,
                    //      n_qty = RC2Count[nLoop].n_qty,
                    //      n_sisa = RC2Count[nLoop].n_sisa,
                    //      v_ket_del = structure.Fields.Keterangan,
                    //      v_type = "03"
                    //    });

                    //    #region delete RC2

                    //    ListRCD2 = (from q in db.LG_RCD2s
                    //                where q.c_gdg == gudang && q.c_rcno == rcID
                    //                && q.c_rnno == RC2Count[nLoopC].c_rnno
                    //                && q.c_iteno == RC2Count[nLoopC].c_iteno
                    //                && q.c_dono == RC2Count[nLoopC].c_dono
                    //                && q.c_batch == RC2Count[nLoopC].c_batch
                    //                && q.c_spno == RC2Count[nLoopC].c_spno
                    //                && q.c_type == RC2Count[nLoopC].c_type
                    //                select q).AsQueryable().ToList();

                    //    db.LG_RCD4s.InsertAllOnSubmit(ListRCD4.ToArray());

                    //    db.LG_RCD2s.DeleteAllOnSubmit(ListRCD2.ToArray());

                    //    #endregion

                    //  }

                    //  #endregion

                    //  #region Delete RCD1, 3

                    //  ListRCD1 = (from q in db.LG_RCD1s
                    //              where q.c_gdg == gudang && q.c_rcno == rcID
                    //              && q.c_batch == RCCount[nLoop].c_batch
                    //              && q.c_dono == RCCount[nLoop].c_dono
                    //              && q.c_iteno == RCCount[nLoop].c_iteno
                    //              && q.c_rnno == RCCount[nLoop].c_rnno
                    //              select q).AsQueryable().ToList();


                    //  ListRCD3 = (from q in db.LG_RCD3s
                    //              where q.c_gdg == gudang && q.c_rcno == rcID
                    //              && q.c_iteno == RCCount[nLoop].c_iteno
                    //              select q).AsQueryable().ToList();

                    //  db.LG_RCD1s.DeleteAllOnSubmit(ListRCD1.ToArray());

                    //  db.LG_RCD3s.DeleteAllOnSubmit(ListRCD3.ToArray());

                    //  #endregion
                    //}

                    #endregion

                    ListRCD1.Clear();
                    ListRCD2.Clear();
                    ListRCD3.Clear();
                    ListRCD4.Clear();
                    lstTemp.Clear();

                    #endregion

                    hasAnyChanges = true;

                    #endregion
                }
                else if (structure.Method.Equals("ConfirmSent", StringComparison.OrdinalIgnoreCase))
                {
                    #region ConfirmSent

                    if (string.IsNullOrEmpty(rcID))
                    {
                        result = "Nomor RC dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        goto endLogic;
                    }

                    rch = (from q in db.LG_RCHes
                           where (q.c_rcno == rcID) && (q.c_gdg == gudang)
                           select q).Take(1).SingleOrDefault();

                    if (rcID == null)
                    {
                        result = "Nomor RC tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (rch.l_delete.HasValue && rch.l_delete.Value)
                    {
                        result = "Tidak dapat mengubah nomor RC yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    //else if (Commons.HasFJR(db, rcID))
                    //{
                    //  result = "RC yang sudah terdapat Faktur tidak dapat dikirim.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}
                    else if (rch.l_sent == true)
                    {
                        result = "Retur Customer tidak dapat dikirim, sudah di kirim.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    //else if (Commons.IsClosingLogistik(db, rch.d_rcdate))
                    //{
                    //    result = "Retur Customer tidak dapat dikirim, karena sudah closing.";

                    //    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //    if (db.Transaction != null)
                    //    {
                    //        db.Transaction.Rollback();
                    //    }

                    //    goto endLogic;
                    //}

                    isSent = (rch.l_send.HasValue ? rch.l_send.Value : false);

                    if (structure.Fields.ConfirmedSent && (!isSent))
                    {
                        rch.l_send = structure.Fields.ConfirmedSent;
                        rch.l_sent = structure.Fields.ConfirmedSent;

                        rch.c_update = nipEntry;
                        rch.d_update = date;

                        hasAnyChanges = true;
                    }
                    else
                    {
                        result = "Status kirim RC terkonfirmasi.";
                    }

                    #endregion
                }
                else if (structure.Method.Equals("ConfirmReSent", StringComparison.OrdinalIgnoreCase))
                {
                    #region ConfirmReSent
                    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    {
                        field = structure.Fields.Field[nLoop];

                        gudang = (string.IsNullOrEmpty(field.Gudang) || (field.Gudang.Length < 1) ? '1' : field.Gudang[0]);
                        //    field = structure.Fields.Field[nLoop];

                        rcID = field.RCid;
                        if (string.IsNullOrEmpty(rcID))
                        {
                            result = "Nomor RC dibutuhkan.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            goto endLogic;
                        }

                        rch = (from q in db.LG_RCHes
                               where (q.c_rcno == rcID) //&& (q.c_gdg == gudang)
                               select q).Take(1).SingleOrDefault();

                        if (rcID == null)
                        {
                            result = "Nomor RC tidak ditemukan.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }
                        else if (rch.l_delete.HasValue && rch.l_delete.Value)
                        {
                            result = "Tidak dapat mengubah nomor RC yang sudah terhapus.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }

                        if (field.IsDcoreSend)
                        {
                            //var NewNoPBB = ResponToDisCorePBResend(db, structure, null, date, true, false, nLoop);

                            //if (string.IsNullOrEmpty(NewNoPBB))
                            //{
                            //    rch.v_pbbrno = field.PBId;
                            //    rch.v_oldpbbrno = field.PBId;
                            //}
                            //else
                            //{
                            //    rch.v_pbbrno = NewNoPBB;
                            //    rch.v_oldpbbrno = field.PBId;
                            //}

                            //ListRCD1 = new List<LG_RCD1>();

                            //ListRCD1 = (from q in db.LG_RCD1s
                            //            where q.c_rcno == rcID
                            //              select q).Distinct().ToList();

                            //for (int nLoop1 = 0; nLoop < ListRCD1.Count; nLoop1++)
                            //{
                            //    field.Item = ListRCD1[nLoop1].c_iteno;
                            //    field.Batch = ListRCD1[nLoop1].c_batch;
                            //    nQtyAlloc = ((decimal)ListRCD1[nLoop1].n_qtyrcv + (decimal)ListRCD1[nLoop1].n_qtydestroy);
                            //    field.Quantity = nQtyAlloc;
                            //    field.NoDO = ListRCD1[nLoop1].c_dono;
                            //    field.NoRN = ListRCD1[nLoop1].c_rnno;
                            //}

                            var proses = ResponToDisCorePBResend(db, structure, rcID, date, false, false, nLoop);
                        }

                        isSent = (rch.l_sent.HasValue ? rch.l_sent.Value : false);

                        if (isSent)
                        {
                            rch.l_send = isSent;
                            rch.l_sent = isSent;

                            rch.c_update = nipEntry;
                            rch.d_update = date;

                            hasAnyChanges = true;
                            result = "Status kirim RC terkonfirmasi.";
                        }
                        else
                        {
                            result = "Status kirim RC gagal.";
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

                result = string.Format("ScmsSoaLibrary.Bussiness.Retur:ReturCustomerIn - {0}", ex.Message);

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

        public string ReturRS(ScmsSoaLibrary.Parser.Class.ReturCustomerStructureIn structure)
        {
            //if ((structure == null) || (structure.Fields == null))
            //{
            //    return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            //}

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            ScmsSoaLibrary.Parser.Class.ReturCustomerStructureInField field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now;

            IDictionary<string, string> dic = null;

            char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '1' : structure.Fields.Gudang[0]);

            string result = null,
              nipEntry = null,
              rnTempId = null;
            string rcID = null,
              pinCode = null,
              typeItem = null;
            int nLoop = 0;
            decimal nQtyAlloc = 0,
              nQtyAlocate = 0,
              totalDetails = 0,GqtyRN = 0,GqtyRc = 0;
            bool isSent = false;
            LG_RSH rsh = null;
            nipEntry = structure.Fields.Entry;
            rcID = structure.Fields.ReturCustomerID;
            string rsID = Commons.GenerateNumbering<LG_RSH>(db, "RS", '3', "04", date, "c_rsno");
            string PLNO = structure.Fields.PBBR;
            db.Connection.Open();
            db.Transaction = db.Connection.BeginTransaction();
            string outlet = (from q in db.LG_DOPHs where q.c_plphar == PLNO
                                 select q.c_po_outlet).SingleOrDefault();
            var STAT = (from q in db.LG_RCHes where q.c_rcno == rcID
                          select q.l_status).SingleOrDefault();
            if (STAT == true)
            {
                result = "RC ini sudah dibuat menjadi RS.";
                rpe = ResponseParser.ResponseParserEnum.IsError;
                goto endlogic;
            }
            var RN = (from q in db.LG_RCD1s
                      where q.c_rcno == rcID
                      select q
                      ).ToList();
            int nloop = 0;
            for (nloop = 0; nloop < RN.Count(); nloop++)
            {
                var SRN = RN[nloop];
                GqtyRc = SRN.n_qtyrcv ?? 0;
                
                var QTYRN = (from q in db.LG_RND1s
                              where q.c_iteno == SRN.c_iteno && q.c_batch == SRN.c_batch
                              select q.n_gsisa
                              ).ToList();
                GqtyRN = QTYRN.Sum() ?? 0;
                if (GqtyRN < GqtyRc)
                {
                    result = "Qty RS item " + SRN.c_iteno + " tidak mencukupi";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    goto endlogic;
                }

            }
                try
                {
                    if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
                    {
                        db.SP_RSAUTO_EXEC(rcID, rsID, PLNO, nipEntry);
                        db.Transaction.Commit();
                        result = "DATA TELAH DI SIMPAN. Nomor RS =" + rsID ;

                        rpe = ResponseParser.ResponseParserEnum.IsSuccess;

                    }
                }
                catch (Exception ex)
                {
                    result = "DATA GAGAL DI SIMPAN.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    goto endlogic;
                }
            endlogic:
                result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

                if (dic != null)
                {
                    dic.Clear();
                }

                db.Dispose();
            return result;
        }

        public string ProsesST(ScmsSoaLibrary.Parser.Class.ReturCustomerStructureIn structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            ScmsSoaLibrary.Parser.Class.ReturCustomerStructureInField field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now;

            IDictionary<string, string> dic = null;

            char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '1' : structure.Fields.Gudang[0]);

            string result = null,
              nipEntry = null,
              rnTempId = null;
            string stID = null,
              pinCode = null,
              typeItem = null;
            int nLoop = 0;
            decimal nQtyAlloc = 0,
              nQtyAlocate = 0,
              totalDetails = 0;
            bool isSent = false;
            string iteno = null, batch = null;

            LG_SerahTerimaH STH = null;
            LG_SerahTerimaD STD = null;
            List<LG_SerahTerimaD> listSTD = null;
            listSTD = new List<LG_SerahTerimaD>();
            LG_RCH RCH = null;
            LG_RCD1 RCD1 = null;
            List<LG_RCD1> listRCD1 = null;
            if (!string.IsNullOrEmpty(stID))
            {
                result = "Nomor Serah Terima harus kosong.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                if (db.Transaction != null)
                {
                    db.Transaction.Rollback();
                }

                goto endLogic;
            }
            else if (string.IsNullOrEmpty(structure.Fields.Entry))
            {
                result = "NIP Penginput dibutuhkan";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                if (db.Transaction != null)
                {
                    db.Transaction.Rollback();
                }

                goto endLogic;
            }
            else if (string.IsNullOrEmpty(structure.Fields.ReturCustomerID))
            {
                result = "Nomor Retur Customer harus ada";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                if (db.Transaction != null)
                {
                    db.Transaction.Rollback();
                }

                goto endLogic;
            }
            db.Connection.Open();

            db.Transaction = db.Connection.BeginTransaction();

            stID = Commons.GenerateNumbering<LG_SerahTerimaH>(db, "ST", '3', "47", date, "v_stno");
            RCH = (from q in db.LG_RCHes where q.c_rcno == structure.Fields.ReturCustomerID select q).SingleOrDefault();
            listRCD1 = (from q in db.LG_RCD1s where q.c_rcno == structure.Fields.ReturCustomerID select q).ToList();
            int cek = (from q in db.LG_SerahTerimaHs where q.c_rcno == structure.Fields.ReturCustomerID select q).Count();
            if (cek > 0)
            {
                result = "Nomor RC ini sudah pernah di buat dokumen serah terima nya.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                if (db.Transaction != null)
                {
                    db.Transaction.Rollback();
                }

                goto endLogic;
            }
            STH = new LG_SerahTerimaH
            {
                c_gdg = structure.Fields.Gudang,
                c_cusno = RCH.c_cusno,
                c_entry = structure.Fields.Entry,
                c_rcno = structure.Fields.ReturCustomerID,
                d_entry = DateTime.Now,
                v_pbbrno = RCH.v_pbbrno,
                v_stno = stID
            };
            db.LG_SerahTerimaHs.InsertOnSubmit(STH);
            listRCD1 = listRCD1.OrderBy(x => x.c_iteno).ThenBy(y => y.c_batch).ToList();
            for (nLoop = 0; nLoop < listRCD1.Count(); nLoop++)
            {
                if (iteno == listRCD1[nLoop].c_iteno && batch == listRCD1[nLoop].c_batch)
                {
                    STD.n_qty = STD.n_qty + listRCD1[nLoop].n_qtyrcv;
                }
                else
                {
                    STD = new LG_SerahTerimaD
                    {
                        v_stno = stID,
                        c_batch = listRCD1[nLoop].c_batch,
                        c_iteno = listRCD1[nLoop].c_iteno,
                        n_qty = listRCD1[nLoop].n_qtyrcv,
                        n_qtyreject = 0,
                        n_qtyterima = 0
                    };
                    listSTD.Add(STD);                    
                }
                iteno = listRCD1[nLoop].c_iteno;
                batch = listRCD1[nLoop].c_batch;

            }
            if (listSTD.Count > 0)
            {
                db.LG_SerahTerimaDs.InsertAllOnSubmit(listSTD);
                listSTD.Clear();
            }
            db.SubmitChanges();
            db.Transaction.Commit();
            result = "Data sudah selesai di proses";

            rpe = ResponseParser.ResponseParserEnum.IsSuccess;

        endLogic:
            result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

            if (dic != null)
            {
                dic.Clear();
            }

            db.Dispose();

            return result;
        }

        public string UpdateST(ScmsSoaLibrary.Parser.Class.MovementStockStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure Oject null/invalid");
            }
            bool hasAnyChanges = false,
                confirmDisp = false,
                isConfirm = true;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            ScmsSoaLibrary.Parser.Class.ReturSupplierStructureField field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now;
            string result = null;
            IDictionary<string, string> dic = null;
            LG_SerahTerimaD std = null;
            LG_SerahTerimaH STH = null;

            int i = 0;

            rpe = ResponseParser.ResponseParserEnum.IsSuccess;
            try
            {
                db.Connection.Open();
                db.Transaction = db.Connection.BeginTransaction();

                STH = (from q in db.LG_SerahTerimaHs where q.v_stno == structure.Fields.STID select q).SingleOrDefault();

                STH.l_confirm = Convert.ToBoolean(structure.Fields.Confirm);
                STH.d_confirm = Convert.ToDateTime(DateTime.Now);

                for (i = 0; i < structure.Fields.Field.Count(); i++)
                {
                    std = null;
                    std = (from q in db.LG_SerahTerimaDs where q.v_stno == structure.Fields.STID && q.c_iteno == structure.Fields.Field[i].Iteno && q.c_batch == structure.Fields.Field[i].Batch select q).SingleOrDefault();
                    if (std.n_qtyterima > 0)
                    {
                        result = "Nomor serah terima ini sudah pernah di update, tidak boleh di update lagi";
                        rpe = ResponseParser.ResponseParserEnum.IsFailed;
                        db.Transaction.Rollback();
                        goto endLogic;
                    }
                    std.n_qtyreject = std.n_qty - Convert.ToDecimal(structure.Fields.Field[i].QtyTerima);
                    if (std.n_qtyreject < 0 || std.n_qtyterima < 0)
                    {
                        result = "Qty yang diterima tidak boleh lebih kecil dari 0.";
                        rpe = ResponseParser.ResponseParserEnum.IsFailed;
                        db.Transaction.Rollback();
                        goto endLogic;
                    }
                    std.n_qtyterima = Convert.ToDecimal(structure.Fields.Field[i].QtyTerima);
                    if (std.n_qtyterima + std.n_qtyreject > std.n_qty)
                    {
                        result = "Qty yang diterima tidak boleh lebih besar dari pada qty awal serah terima";
                        rpe = ResponseParser.ResponseParserEnum.IsFailed;
                        db.Transaction.Rollback();
                        goto endLogic;                        
                    }
                }
                db.SubmitChanges();
                db.Transaction.Commit();
                result = "Data berhasil diinput";
                rpe = ResponseParser.ResponseParserEnum.IsSuccess;
            }
            catch (Exception ex)
            {
                result = "Terjadi kesalahan, " + ex.Message;
                rpe = ResponseParser.ResponseParserEnum.IsFailed;
                db.Transaction.Rollback();
            }
            endLogic:
            result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

            return result;
        }

        public string ReturSuplier(ScmsSoaLibrary.Parser.Class.ReturSupplierStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false,
                confirmDisp = false,
                isConfirm = true;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            ScmsSoaLibrary.Parser.Class.ReturSupplierStructureField field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now;

            IDictionary<string, string> dic = null;

            char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '1' : structure.Fields.Gudang[0]);

            string result = null;
            string nipEntry = null;
            string rsID = null;
            int nLoop, nLoopC;
            decimal GQty = 0, BQty = 0, GQtyAlocate = 0, BQtyAlocate = 0,
              nQtyAllocGood = 0, nQtyAllocBad = 0;

            int totalDetails = 0;

            List<LG_RSD1> ListRSD1 = null;
            List<LG_RSD2> ListRSD2 = null;
            List<LG_RSD2> ListRSD2Copy = null;
            List<LG_RSD5> ListRSD5 = null;

            List<LG_RND1> ListRND1 = null;
            ReturClassComponent retAcc = null;

            LG_RND1 rnd1 = null;
            LG_RSD2 rsd2 = null;

            LG_ComboH combo = null;

            LG_RSD1 rsd1 = null;
            LG_RSH rsh = null;
            LG_FBRD2 fbrd2 = null;

            List<string> lstTemp = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            rsID = (structure.Fields.ReturSupplierID ?? string.Empty);

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
                {
                    #region Add

                    #region Cek Data

                    if (!string.IsNullOrEmpty(rsID))
                    {
                        result = "Nomor Retur harus kosong.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (string.IsNullOrEmpty(structure.Fields.Supplier))
                    {
                        result = "Nama pemasok dibutuhkan.";

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
                        result = "Retur Supplier tidak dapat disimpan, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    #endregion

                    #region Insert Header RS

                    //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "RSREPACK");

                    rsID = Commons.GenerateNumbering<LG_RSH>(db, "RS", '3', "04", date, "c_rsno");

                    rsh = new LG_RSH()
                    {
                        c_gdg = gudang,
                        c_nosup = structure.Fields.Supplier,
                        c_rsno = rsID,
                        c_type = structure.Fields.TipeRetur,
                        c_update = nipEntry,
                        c_entry = nipEntry,
                        d_entry = DateTime.Now,
                        d_update = DateTime.Now,
                        d_rsdate = DateTime.Now,
                        l_delete = false,
                        l_print = false,
                        v_ket = structure.Fields.Keterangan
                    };

                    db.LG_RSHes.InsertOnSubmit(rsh);

                    #region Old Code

                    //db.SubmitChanges();

                    //rsh = (from q in db.LG_RSHes
                    //       where q.v_ket == tmpNumbering
                    //       select q).Take(1).SingleOrDefault();

                    //if (!string.IsNullOrEmpty(rsID))
                    //{
                    //  result = "Nomor Retur Supplier tidak dapat di raih.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}

                    //if (rsh.c_rsno.Equals("XXXXXXXXXX"))
                    //{
                    //  result = "Trigger Retur Supplier tidak aktif.";

                    //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    //  if (db.Transaction != null)
                    //  {
                    //    db.Transaction.Rollback();
                    //  }

                    //  goto endLogic;
                    //}

                    //rsh.v_ket = structure.Fields.Keterangan;

                    //rsID = rsh.c_rsno;

                    #endregion

                    #endregion

                    #region Insert Detil

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        ListRSD1 = new List<LG_RSD1>();
                        ListRSD2 = new List<LG_RSD2>();
                        ListRSD2Copy = new List<LG_RSD2>();
                        ListRND1 = new List<LG_RND1>();
                        //rnd1 = new LG_RND1();

                        //decimal? BAqum = 0, GAqum = 0, bE = 0, gE = 0, bI = 0, gI = 0;
                        //bool GLop = true, BLop = true;

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if (field.IsNew && ((field.GQty > 0) || (field.BQty > 0)))
                            {
                                #region Populate Data

                                #region Cek RN

                                var LstRN = (from q in GlobalQuery.ViewStockLiteContains(db, gudang, field.Item)
                                             where q.c_batch == field.Batch
                                             //&& q.c_iteno == field.Item
                                             select q).ToList();

                                //ListRND1 = (from q in db.LG_RNHs
                                //            join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                //            where (q.c_gdg == gudang) && (q1.c_iteno == field.Item)
                                //              && (q1.c_batch == field.Batch)
                                //              && ((q1.n_gsisa > 0) || (q1.n_bsisa > 0))
                                //              && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                //            select q1).Distinct().ToList();

                                GQty = field.GQty;
                                BQty = field.BQty;

                                for (nLoopC = 0; nLoopC < LstRN.Count; nLoopC++)
                                {
                                    var sgnlRN = LstRN[nLoopC];
                                    bool isOk = false;

                                    GQtyAlocate = sgnlRN.n_gsisa;
                                    BQtyAlocate = sgnlRN.n_bsisa;

                                    if (sgnlRN.c_table.Equals("CB"))
                                    {
                                        combo = (from q in db.LG_ComboHs
                                                 where q.c_gdg == gudang &&
                                                 q.c_iteno == sgnlRN.c_iteno
                                                 && q.c_batch == sgnlRN.c_batch
                                                 && q.c_combono == sgnlRN.c_no
                                                 select q).Take(1).SingleOrDefault();

                                        #region Good

                                        nQtyAllocGood = ((GQty > GQtyAlocate) ? GQtyAlocate : GQty);

                                        if (nQtyAllocGood > 0)
                                        {
                                            if (combo.n_gsisa >= nQtyAllocGood)
                                            {
                                                combo.n_gsisa -= nQtyAllocGood;

                                                GQty -= nQtyAllocGood;

                                                isOk = true;
                                            }
                                        }

                                        #endregion

                                        #region Bad

                                        nQtyAllocBad = ((BQty > BQtyAlocate) ? BQtyAlocate : BQty);

                                        if (nQtyAllocBad > 0)
                                        {
                                            if (combo.n_bsisa >= nQtyAllocBad)
                                            {
                                                combo.n_bsisa -= nQtyAllocBad;

                                                BQty -= nQtyAllocBad;

                                                isOk = true;
                                            }
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        rnd1 = (from q in db.LG_RND1s
                                                where q.c_gdg == gudang &&
                                                q.c_iteno == sgnlRN.c_iteno
                                                && q.c_batch == sgnlRN.c_batch
                                                && q.c_rnno == sgnlRN.c_no
                                                select q).Take(1).SingleOrDefault();

                                        #region Good

                                        nQtyAllocGood = ((GQty > GQtyAlocate) ? GQtyAlocate : GQty);

                                        if (nQtyAllocGood > 0)
                                        {
                                            if (rnd1.n_gsisa >= nQtyAllocGood)
                                            {
                                                rnd1.n_gsisa -= nQtyAllocGood;

                                                GQty -= nQtyAllocGood;
                                                isOk = true;

                                            }
                                        }

                                        #endregion

                                        #region Bad

                                        nQtyAllocBad = ((BQty > BQtyAlocate) ? BQtyAlocate : BQty);

                                        if (nQtyAllocBad > 0)
                                        {
                                            if (rnd1.n_bsisa >= nQtyAllocBad)
                                            {
                                                rnd1.n_bsisa -= nQtyAllocBad;

                                                BQty -= nQtyAllocBad;

                                                isOk = true;
                                            }
                                        }

                                        #endregion
                                    }

                                    if (isOk)
                                    {
                                        rsd2 = new LG_RSD2()
                                        {
                                            c_gdg = gudang,
                                            c_rsno = rsID,
                                            c_iteno = field.Item,
                                            c_batch = field.Batch,
                                            c_rnno = sgnlRN.c_no,
                                            l_status = false,
                                            n_gqty = nQtyAllocGood,
                                            n_gsisa = nQtyAllocGood,
                                            n_bqty = nQtyAllocBad,
                                            n_bsisa = nQtyAllocBad,
                                            c_cprno = field.CprNo,
                                            l_confirm = false,
                                            n_bqtyAcc = 0,
                                            n_gqtyAcc = 0
                                        };

                                        ListRSD2Copy.Add(rsd2);
                                    }

                                    if ((GQty <= 0) && (BQty <= 0))
                                    {
                                        break;
                                    }
                                }

                                ListRND1.Clear();

                                #endregion

                                //if ((GQty == 0) && (BQty == 0) && (ListRSD2Copy.Count > 0))
                                if ((ListRSD2Copy.Count > 0))
                                {
                                    ListRSD1.Add(new LG_RSD1()
                                    {
                                        c_gdg = gudang,
                                        c_rsno = rsID,
                                        c_iteno = field.Item,
                                        c_batch = field.Batch,
                                        c_cprno = field.CprNo,
                                        v_ket = field.ketD,
                                        n_bqty = field.BQty,
                                        n_gqty = field.GQty,
                                        c_cusno = field.Cabang,
                                        c_outlet = field.Outlet,
                                        c_reason = field.Reason
                                    });

                                    ListRSD2.AddRange(ListRSD2Copy.ToArray());

                                    ListRSD2Copy.Clear();

                                    totalDetails++;
                                }

                                #endregion
                            }

                            #region Old Coded

                            //for (nLoopC = 0; nLoopC < varLisRnd1.Count; nLoopC++)
                            //{
                            //  if (varLisRnd1[nLoopC].n_bsisa > 0 || varLisRnd1[nLoopC].n_gsisa > 0)
                            //  {
                            //    bI = nQtyAllocBad - varLisRnd1[nLoopC].n_bsisa.Value;
                            //    gI = nQtyAllocGood - varLisRnd1[nLoopC].n_gsisa.Value;

                            //    if (BLop == true)
                            //    {
                            //      bE = nQtyAllocBad - bI;
                            //      BAqum = BAqum + bE;
                            //    }
                            //    if (GLop == true)
                            //    {
                            //      gE = nQtyAllocGood - gI;
                            //      GAqum = GAqum + gE;
                            //    }
                            //    if (BAqum > nQtyAllocBad)
                            //    {
                            //      bE = nQtyAllocBad - (BAqum - bE);
                            //      BAqum = BAqum + bE;
                            //    }
                            //    if (GAqum > nQtyAllocGood)
                            //    {
                            //      gE = nQtyAllocGood - (GAqum - gE);
                            //      GAqum = GAqum + gE;
                            //    }
                            //    if (BLop == false)
                            //    {
                            //      bE = 0;
                            //    }
                            //    if (GLop == false)
                            //    {
                            //      gE = 0;
                            //    }

                            //    if (bE != 0 || gE != 0)
                            //    {
                            //      ListRSD2.Add(new LG_RSD2()
                            //      {
                            //        c_batch = field.Batch,
                            //        c_gdg = gudang,
                            //        c_iteno = field.Item,
                            //        c_rsno = rsID,
                            //        n_bqty = bE,
                            //        n_gqty = gE,
                            //        c_rnno = varLisRnd1[nLoopC].c_rnno,
                            //        n_bsisa = bE,
                            //        n_gsisa = gE,
                            //        l_status = false
                            //      });
                            //    }

                            //    rnd1 = (from q in db.LG_RND1s
                            //            where q.c_gdg == gudang &&
                            //            q.c_iteno == varLisRnd1[nLoopC].c_iteno
                            //            && q.c_batch == varLisRnd1[nLoopC].c_batch
                            //            && q.c_rnno == varLisRnd1[nLoopC].c_rnno
                            //            select q).Take(1).SingleOrDefault();

                            //    rnd1.n_gsisa -= gE;
                            //    rnd1.n_bsisa -= bE;

                            //    if (BAqum > nQtyAllocBad)
                            //    {
                            //      BLop = false;
                            //    }
                            //    if (GAqum > nQtyAllocGood)
                            //    {
                            //      GLop = false;
                            //    }
                            //  }
                            //}

                            //#region Insert RSD1

                            //ListRSD1.Add(new LG_RSD1()
                            //{
                            //  c_batch = field.Batch,
                            //  c_cprno = field.CprNo,
                            //  c_gdg = gudang,
                            //  c_iteno = field.Item,
                            //  c_rsno = rsID,
                            //  n_bqty = field.BQty,
                            //  n_gqty = field.GQty,
                            //  v_ket = field.ketD
                            //});

                            //#endregion

                            #endregion
                        }
                        if (ListRSD1.Count > 0 && ListRSD1 != null)
                        {
                            db.LG_RSD1s.InsertAllOnSubmit(ListRSD1.ToArray());
                            ListRSD1.Clear();
                        }
                        if (ListRSD2 != null && ListRSD2.Count > 0)
                        {
                            db.LG_RSD2s.InsertAllOnSubmit(ListRSD2.ToArray());
                            ListRSD2.Clear();
                        }
                    }

                    #endregion

                    if (totalDetails > 0)
                    {
                        dic = new Dictionary<string, string>();

                        dic.Add("RSID", rsID);
                        dic.Add("Tanggal", date.ToString("yyyyMMdd"));

                        hasAnyChanges = true;
                    }

                    #endregion
                }
                else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
                {
                    #region Modify

                    if (string.IsNullOrEmpty(rsID))
                    {
                        result = "Nomor Retur Supplier dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    rsh = (from q in db.LG_RSHes
                           where q.c_rsno == rsID
                           select q).Take(1).SingleOrDefault();

                    if (rsh == null)
                    {
                        result = "Nomor Retur Supplier tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (rsh.l_delete.HasValue && rsh.l_delete.Value)
                    {
                        result = "Tidak dapat menghapus nomor RS yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, rsh.d_rsdate))
                    {
                        result = "Retur Supplier tidak dapat diubah, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.HasFBR(db, rsID))
                    {
                        result = "Retur Supplier yang sudah terdapat Faktur Beli Retur tidak dapat diubah.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
                    {
                        rsh.v_ket = structure.Fields.Keterangan;
                    }

                    rsh.c_update = nipEntry;
                    rsh.d_update = DateTime.Now;

                    #region Populate Detail

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {
                        ListRSD1 = new List<LG_RSD1>();
                        ListRSD2 = new List<LG_RSD2>();
                        ListRND1 = new List<LG_RND1>();
                        ListRSD5 = new List<LG_RSD5>();
                        rsd2 = new LG_RSD2();
                        combo = new LG_ComboH();
                        rnd1 = new LG_RND1();

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified) && ((field.BQty > 0) || (field.GQty > 0)))
                            {
                                #region Add Detil

                                #region Insert into RSD2

                                //var varLisRnd1 = (from q in db.LG_RND1s
                                //                  where q.c_batch == field.Batch
                                //                  && q.c_gdg == gudang &&
                                //                  q.c_iteno == field.Item
                                //                  select q).ToList();

                                var varLisRnd1 = (from q in GlobalQuery.ViewStockLiteContains(db, gudang, field.Item)
                                                  where q.c_batch == field.Batch
                                                  select q).ToList();



                                nQtyAllocGood = field.GQty;
                                nQtyAllocBad = field.BQty;
                                decimal? BAqum = 0, GAqum = 0, bE = 0, gE = 0, bI = 0, gI = 0;
                                bool GLop = true, BLop = true;

                                for (nLoopC = 0; nLoopC < varLisRnd1.Count; nLoopC++)
                                {
                                    if (varLisRnd1[nLoopC].n_bsisa > 0 || varLisRnd1[nLoopC].n_gsisa > 0)
                                    {
                                        bI = nQtyAllocBad - varLisRnd1[nLoopC].n_bsisa;
                                        gI = nQtyAllocGood - varLisRnd1[nLoopC].n_gsisa;

                                        if (BLop == true)
                                        {
                                            bE = nQtyAllocBad - bI;
                                            BAqum = BAqum + bE;
                                        }
                                        if (GLop == true)
                                        {
                                            gE = nQtyAllocGood - gI;
                                            GAqum = GAqum + gE;
                                        }
                                        if (BAqum > nQtyAllocBad)
                                        {
                                            bE = nQtyAllocBad - (BAqum - bE);
                                            BAqum = BAqum + bE;
                                        }
                                        if (GAqum > nQtyAllocGood)
                                        {
                                            gE = nQtyAllocGood - (GAqum - gE);
                                            GAqum = GAqum + gE;
                                        }
                                        if (BLop == false)
                                        {
                                            bE = 0;
                                        }
                                        if (GLop == false)
                                        {
                                            gE = 0;
                                        }

                                        if (bE != 0 || gE != 0)
                                        {
                                            rsd2 = new LG_RSD2()
                                            {
                                                c_batch = field.Batch,
                                                c_gdg = gudang,
                                                c_iteno = field.Item,
                                                c_rsno = rsID,
                                                n_bqty = bE,
                                                n_gqty = gE,
                                                c_rnno = varLisRnd1[nLoopC].c_no,
                                                n_bsisa = bE,
                                                n_gsisa = gE,
                                                l_status = false
                                            };

                                            db.LG_RSD2s.InsertOnSubmit(rsd2);

                                            #region Old Code

                                            //ListRSD2.Add(new LG_RSD2()
                                            //{
                                            //  c_batch = field.Batch,
                                            //  c_gdg = gudang,
                                            //  c_iteno = field.Item,
                                            //  c_rsno = rsID,
                                            //  n_bqty = bE,
                                            //  n_gqty = gE,
                                            //  c_rnno = varLisRnd1[nLoopC].c_rnno,
                                            //  n_bsisa = bE,
                                            //  n_gsisa = gE,
                                            //  l_status = false
                                            //});

                                            #endregion
                                        }

                                        if (varLisRnd1[nLoopC].c_table.Equals("CB"))
                                        {
                                            combo = (from q in db.LG_ComboHs
                                                     where q.c_gdg == gudang &&
                                                     q.c_iteno == varLisRnd1[nLoopC].c_iteno
                                                     && q.c_batch == varLisRnd1[nLoopC].c_batch
                                                     && q.c_combono == varLisRnd1[nLoopC].c_no
                                                     select q).Take(1).SingleOrDefault();

                                            combo.n_gsisa -= gE;
                                            combo.n_bsisa -= bE;
                                        }
                                        else
                                        {
                                            rnd1 = (from q in db.LG_RND1s
                                                    where q.c_gdg == gudang &&
                                                    q.c_iteno == varLisRnd1[nLoopC].c_iteno
                                                    && q.c_batch == varLisRnd1[nLoopC].c_batch
                                                    && q.c_rnno == varLisRnd1[nLoopC].c_no
                                                    select q).Take(1).SingleOrDefault();

                                            rnd1.n_gsisa -= gE;
                                            rnd1.n_bsisa -= bE;
                                        }

                                        if (BAqum > nQtyAllocBad)
                                        {
                                            BLop = false;
                                        }
                                        if (GAqum > nQtyAllocGood)
                                        {
                                            GLop = false;
                                        }
                                    }
                                }
                                #endregion

                                #region Insert RSD1

                                rsd1 = new LG_RSD1()
                                {
                                    c_batch = field.Batch,
                                    c_cprno = field.CprNo,
                                    c_gdg = gudang,
                                    c_iteno = field.Item,
                                    c_rsno = rsID,
                                    n_bqty = field.BQty,
                                    n_gqty = field.GQty,
                                    v_ket = field.ketD,
                                    c_cusno = field.Cabang,
                                    c_outlet = field.Outlet,
                                    c_reason = field.Reason
                                };

                                db.LG_RSD1s.InsertOnSubmit(rsd1);

                                #region Old

                                //ListRSD1.Add(new LG_RSD1()
                                //{
                                //  c_batch = field.Batch,
                                //  c_cprno = field.CprNo,
                                //  c_gdg = gudang,
                                //  c_iteno = field.Item,
                                //  c_rsno = rsID,
                                //  n_bqty = field.BQty,
                                //  n_gqty = field.GQty,
                                //  v_ket = field.ketD
                                //});

                                #endregion

                                #endregion

                                #endregion
                            }
                            else if ((field != null) && (!field.IsNew) && field.IsDelete && (!field.IsModified))
                            {
                                #region Delete Detil

                                ListRSD2 = (from q in db.LG_RSD2s
                                            where q.c_rsno == rsID && q.c_gdg == gudang &&
                                           q.c_iteno == field.Item && q.c_batch == field.Batch
                                           && (q.n_bsisa == q.n_bqty) && (q.n_gsisa == q.n_gqty)
                                            select q).ToList();

                                #region Insert Into RSD5 & Delete RSD2 dan update RN

                                for (nLoopC = 0; nLoopC < ListRSD2.Count; nLoopC++)
                                {

                                    rsd2 = ListRSD2[nLoopC];

                                    db.LG_RSD2s.DeleteOnSubmit(rsd2);

                                    ListRSD5.Add(new LG_RSD5()
                                    {
                                        c_batch = rsd2.c_batch,
                                        c_iteno = rsd2.c_iteno,
                                        c_rsno = rsID,
                                        n_bqty = rsd2.n_bqty,
                                        n_gqty = rsd2.n_bsisa,
                                        c_rnno = rsd2.c_rnno,
                                        l_status = rsd2.l_status,
                                        n_bsisa = rsd2.n_bsisa,
                                        n_gsisa = rsd2.n_gsisa,
                                        c_entry = nipEntry,
                                        d_entry = DateTime.Now,
                                        v_ket_del = field.Keterangan,
                                        v_type = "02"
                                    });


                                    if (rsd2.c_rnno.Substring(0, 2).Equals("CB"))
                                    {
                                        combo = (from q in db.LG_ComboHs
                                                 where q.c_gdg == gudang &&
                                                 q.c_iteno == rsd2.c_iteno
                                                 && q.c_batch == rsd2.c_batch
                                                 && q.c_combono == rsd2.c_rnno
                                                 select q).Take(1).SingleOrDefault();

                                        combo.n_gsisa -= rsd2.n_gsisa;
                                        combo.n_bsisa -= rsd2.n_bsisa;
                                    }
                                    else
                                    {
                                        rnd1 = (from q in db.LG_RNHs
                                                join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                                where q.c_rnno == rsd2.c_rnno &&
                                                q.c_gdg == rsd2.c_gdg && q1.c_iteno == rsd2.c_iteno
                                                && q1.c_batch == rsd2.c_batch
                                                select q1).Take(1).SingleOrDefault();

                                        if (rnd1 != null)
                                        {
                                            rnd1.n_bsisa += rsd2.n_bsisa;
                                            rnd1.n_gsisa += rsd2.n_gsisa;
                                        }
                                    }

                                    #region Old COde

                                    //if (retAcc != null)
                                    //{
                                    //  if (retAcc.SignID.Equals("RN", StringComparison.OrdinalIgnoreCase))
                                    //  {
                                    //    rnd1 = (from q in db.LG_RND1s
                                    //            where (q.c_gdg == gudang) && (q.c_rnno == retAcc.RefID)
                                    //              && (q.c_iteno == field.Item) && (q.c_batch == field.Batch)
                                    //            select q).Take(1).SingleOrDefault();

                                    //    if (rnd1 != null)
                                    //    {
                                    //      rnd1.n_gsisa += rsd2.n_gsisa;
                                    //      rsd2.n_bsisa += rsd2.n_bsisa;
                                    //    }
                                    //  }
                                    //  else
                                    //  {
                                    //    combo = (from q in db.LG_ComboHs
                                    //             where (q.c_gdg == gudang) && (q.c_combono == retAcc.RefID)
                                    //              && (q.c_iteno == field.Item) && (q.c_batch == field.Batch)
                                    //              && (q.n_gsisa > 0)
                                    //             select q).Take(1).SingleOrDefault();

                                    //    if (combo != null)
                                    //    {
                                    //      combo.n_gsisa += rsd2.n_gsisa;
                                    //      combo.n_bsisa += rsd2.n_bsisa;
                                    //    }
                                    //  }
                                    //}

                                    #endregion
                                }

                                #endregion

                                rsd1 = (from q in db.LG_RSD1s
                                        where q.c_rsno == rsID && q.c_gdg == gudang &&
                                         q.c_iteno == field.Item && q.c_batch == field.Batch
                                        select q).Take(1).SingleOrDefault();

                                if (rsd1 != null)
                                {
                                    db.LG_RSD1s.DeleteOnSubmit(rsd1);
                                }

                                #endregion
                            }
                            else if ((field != null) && (!field.IsNew) && (!field.IsDelete) && field.IsModified)
                            {
                                #region Old Coded

                                #region Old Codes 1

                                //ListRSD2 = (from q in db.LG_RSD2s
                                //                   where q.c_rsno == rsID && q.c_gdg == gudang &&
                                //                   q.c_iteno == field.Item && q.c_batch == field.Batch
                                //                   select q).ToList();

                                //decimal? qtyGRN = 0, qtyGRND1 = 0;
                                //decimal? qtyBRN = 0, qtyBRND1 = 0;
                                //bool minG = false;
                                //bool minB = false;

                                //qtyGRND1 = field.GQty;
                                //qtyBRND1 = field.BQty;

                                //if (field.GQty >= 0)
                                //{
                                //  qtyGRN = field.GQty;
                                //}
                                //else if (field.GQty < 0)
                                //{
                                //  qtyGRN = field.GQty * -1;
                                //  minG = true;
                                //}

                                //if (field.BQty >= 0)
                                //{
                                //  qtyBRN = field.BQty;
                                //}
                                //else if (field.GQty < 0)
                                //{
                                //  qtyBRN = field.BQty * -1;
                                //  minB = true;
                                //}

                                //if (ListRSD2.GroupBy(x => x.c_rnno).Sum(y => y.Sum(x => x.n_gsisa)) >= field.GQty &&
                                //    ListRSD2.GroupBy(x => x.c_rnno).Sum(y => y.Sum(x => x.n_bsisa)) >= field.BQty)
                                //{

                                //  #region Check Data RSD2 GOOD

                                //  for (nLoop = 0; nLoop < ListRSD2.Count; nLoop++)
                                //  {
                                //    var VarTakeRSD2 = (from q in db.LG_RSD2s
                                //                       where q.c_rsno == rsID &&
                                //                        q.c_iteno == field.Item && q.c_batch == field.Batch
                                //                        && q.c_rnno == ListRSD2[nLoop].c_rnno
                                //                       select q).Take(1).SingleOrDefault();


                                //    if (qtyGRN > VarTakeRSD2.n_gqty && qtyGRN != 0)
                                //    {
                                //      ListRSD2.Add(new LG_RSD2()
                                //      {
                                //        c_gdg = gudang,
                                //        c_iteno = VarTakeRSD2.c_iteno,
                                //        c_rsno = rsID,
                                //        n_gqty = VarTakeRSD2.n_gqty,
                                //        c_rnno = VarTakeRSD2.c_rnno
                                //      });
                                //      qtyGRN -= VarTakeRSD2.n_gqty;
                                //    }
                                //    else if (qtyGRN < VarTakeRSD2.n_gqty && qtyGRN != 0)
                                //    {
                                //      ListRSD2.Add(new LG_RSD2()
                                //      {
                                //        c_gdg = gudang,
                                //        c_iteno = VarTakeRSD2.c_iteno,
                                //        c_rsno = rsID,
                                //        n_gqty = qtyGRN,
                                //        c_rnno = VarTakeRSD2.c_rnno
                                //      });

                                //      qtyGRN -= qtyGRN;
                                //    }

                                //  }

                                //  #endregion

                                //  #region Exekusi RSD2 Good Stock

                                //  for (nLoop = 0; nLoop < ListRSD2.Count; nLoop++)
                                //  {

                                //    rnd1 = (from q in db.LG_RND1s
                                //            where q.c_gdg == gudang && q.c_rnno == ListRSD2[nLoop].c_rnno
                                //            && q.c_iteno == field.Item && q.c_batch == field.Batch
                                //            select q).Take(1).SingleOrDefault();

                                //    rsd2 = (from q in db.LG_RSD2s
                                //            where q.c_rsno == rsID && q.c_gdg == gudang &&
                                //             q.c_iteno == field.Item && q.c_batch == field.Batch
                                //             && q.c_rnno == ListRSD2[nLoop].c_rnno
                                //            select q).Take(1).SingleOrDefault();


                                //    ListRSD5.Add(new LG_RSD5()
                                //    {
                                //      c_batch = ListRSD2[nLoop].c_batch,
                                //      c_iteno = ListRSD2[nLoop].c_iteno,
                                //      c_rsno = rsID,
                                //      n_bqty = 0,
                                //      n_gqty = ListRSD2[nLoop].n_gqty,
                                //      c_rnno = ListRSD2[nLoop].c_rnno,
                                //      l_status = ListRSD2[nLoop].l_status,
                                //      n_bsisa = 0,
                                //      n_gsisa = ListRSD2[nLoop].n_gsisa,
                                //      c_entry = nipEntry,
                                //      d_entry = DateTime.Now,
                                //      v_ket_del = field.Keterangan,
                                //      v_type = "02"
                                //    });

                                //    if (minG == true)
                                //    {
                                //      rnd1.n_gsisa += ListRSD2[nLoop].n_gqty;
                                //      rsd2.n_gqty -= ListRSD2[nLoop].n_gqty;
                                //      rsd2.n_gsisa -= ListRSD2[nLoop].n_gqty;
                                //    }
                                //    else if (minG == false)
                                //    {
                                //      rnd1.n_gsisa -= ListRSD2[nLoop].n_gqty;
                                //      rsd2.n_gqty += ListRSD2[nLoop].n_gqty;
                                //      rsd2.n_gsisa += ListRSD2[nLoop].n_gqty;
                                //    }

                                //  }

                                //  #endregion

                                //  #region eksekusi RSD1 GOOD

                                //  rsd1 = (from q in db.LG_RSD1s
                                //          where q.c_rsno == rsID && q.c_gdg == gudang &&
                                //          q.c_iteno == field.Item && q.c_batch == field.Batch
                                //          select q).Take(1).SingleOrDefault();


                                //  rsd1.n_gqty += qtyGRND1;

                                //  ListRSD2.Clear();

                                //  #endregion

                                //  db.LG_RSD5s.InsertAllOnSubmit(ListRSD5.ToArray());

                                //  #region Check Data RSD2 BAD

                                //  for (nLoop = 0; nLoop < VarListRSD2.Count; nLoop++)
                                //  {
                                //    var VarTakeRSD2 = (from q in db.LG_RSD2s
                                //                       where q.c_rsno == rsID &&
                                //                        q.c_iteno == field.Item && q.c_batch == field.Batch
                                //                        && q.c_rnno == VarListRSD2[nLoop].c_rnno
                                //                       select q).Take(1).SingleOrDefault();


                                //    if (qtyBRN > VarTakeRSD2.n_bqty && qtyBRN != 0)
                                //    {
                                //      ListRSD2.Add(new LG_RSD2()
                                //      {
                                //        c_gdg = gudang,
                                //        c_iteno = VarTakeRSD2.c_iteno,
                                //        c_rsno = rsID,
                                //        n_bqty = VarTakeRSD2.n_bqty,
                                //        c_rnno = VarTakeRSD2.c_rnno
                                //      });
                                //      qtyBRN -= VarTakeRSD2.n_bqty;
                                //    }
                                //    else if (qtyBRN < VarTakeRSD2.n_bqty && qtyBRN != 0)
                                //    {
                                //      ListRSD2.Add(new LG_RSD2()
                                //      {
                                //        c_gdg = gudang,
                                //        c_iteno = VarTakeRSD2.c_iteno,
                                //        c_rsno = rsID,
                                //        n_bqty = qtyBRN,
                                //        c_rnno = VarTakeRSD2.c_rnno
                                //      });

                                //      qtyBRN -= qtyBRN;
                                //    }

                                //  }

                                //  #endregion

                                //  #region Exekusi RSD2 Bad Stock

                                //  for (nLoop = 0; nLoop < ListRSD2.Count; nLoop++)
                                //  {

                                //    rnd1 = (from q in db.LG_RND1s
                                //            where q.c_gdg == gudang && q.c_rnno == ListRSD2[nLoop].c_rnno
                                //            && q.c_iteno == field.Item && q.c_batch == field.Batch
                                //            select q).Take(1).SingleOrDefault();

                                //    rsd2 = (from q in db.LG_RSD2s
                                //            where q.c_rsno == rsID && q.c_gdg == gudang &&
                                //             q.c_iteno == field.Item && q.c_batch == field.Batch
                                //             && q.c_rnno == ListRSD2[nLoop].c_rnno
                                //            select q).Take(1).SingleOrDefault();


                                //    ListRSD5.Add(new LG_RSD5()
                                //    {
                                //      c_batch = ListRSD2[nLoop].c_batch,
                                //      c_iteno = ListRSD2[nLoop].c_iteno,
                                //      c_rsno = rsID,
                                //      n_bqty = ListRSD2[nLoop].n_bqty,
                                //      n_gqty = 0,
                                //      c_rnno = ListRSD2[nLoop].c_rnno,
                                //      l_status = ListRSD2[nLoop].l_status,
                                //      n_bsisa = ListRSD2[nLoop].n_bsisa,
                                //      n_gsisa = 0,
                                //      c_entry = nipEntry,
                                //      d_entry = DateTime.Now,
                                //      v_ket_del = field.Keterangan,
                                //      v_type = "02"
                                //    });

                                //    if (minB == true)
                                //    {
                                //      rnd1.n_bsisa += ListRSD2[nLoop].n_bqty;
                                //      rsd2.n_bqty -= ListRSD2[nLoop].n_bqty;
                                //      rsd2.n_bsisa -= ListRSD2[nLoop].n_bqty;
                                //    }
                                //    else if (minB == false)
                                //    {
                                //      rnd1.n_bsisa -= ListRSD2[nLoop].n_bqty;
                                //      rsd2.n_bqty += ListRSD2[nLoop].n_bqty;
                                //      rsd2.n_bsisa += ListRSD2[nLoop].n_bqty;
                                //    }

                                //  }

                                //  #endregion

                                //  #region eksekusi RSD1 BAD

                                //  rsd1 = (from q in db.LG_RSD1s
                                //          where q.c_rsno == rsID && q.c_gdg == gudang &&
                                //          q.c_iteno == field.Item && q.c_batch == field.Batch
                                //          select q).Take(1).SingleOrDefault();


                                //  rsd1.n_bqty += qtyBRND1;

                                //  ListRSD2.Clear();

                                //  db.LG_RSD5s.InsertAllOnSubmit(ListRSD5.ToArray());

                                #endregion

                                #region Old Code

                                //  #region Insert Into RSD5

                                //  //ListRSD2 = (from q in db.LG_RSD2s
                                //  //            where q.c_gdg == gudang && q.c_iteno == field.Item
                                //  //            && q.c_rsno == rsID && q.c_batch == field.Batch
                                //  //            select q).ToList();

                                //  //decimal ? RNQtyGAloc = field.GQty;
                                //  //decimal ? RNBtyGAloc = field.BQty;

                                //  //for (nLoopC = 0; nLoopC < ListRSD2.Count; nLoopC++)
                                //  //{

                                //  //  var VarTakeRND2 = (from q in db.LG_RND1s
                                //  //                     where q.c_iteno == field.Item
                                //  //                     && q.c_batch == field.Batch
                                //  //                     && q.c_rnno == ListRSD2[nLoopC].c_rnno
                                //  //                     select q).Take(1).SingleOrDefault();


                                //  //  rsd2 = (from q in db.LG_RSD2s
                                //  //          where q.c_gdg == gudang && q.c_iteno == field.Item
                                //  //            && q.c_rsno == rsID && q.c_batch == field.Batch
                                //  //            && q.c_rnno == ListRSD2[nLoopC].c_rnno
                                //  //            select q).Take(1).SingleOrDefault();

                                //  //  ListRSD5.Add(new LG_RSD5()
                                //  //  {
                                //  //    c_batch = ListRSD2[nLoopC].c_batch,
                                //  //    c_iteno = ListRSD2[nLoopC].c_iteno,
                                //  //    c_rsno = rsID,
                                //  //    n_bqty = ListRSD2[nLoopC].n_bqty,
                                //  //    n_gqty = ListRSD2[nLoopC].n_gqty,
                                //  //    c_rnno = ListRSD2[nLoopC].c_rnno,
                                //  //    l_status = ListRSD2[nLoopC].l_status,
                                //  //    n_bsisa = ListRSD2[nLoopC].n_bsisa,
                                //  //    n_gsisa = ListRSD2[nLoopC].n_gsisa,
                                //  //    c_entry = nipEntry,
                                //  //    d_entry = DateTime.Now,
                                //  //    v_ket_del = field.Keterangan,
                                //  //    v_type = "02"
                                //  //  });

                                //  //  db.LG_RSD5s.InsertAllOnSubmit(ListRSD5.ToArray());

                                //  //  for (int RNQtyG = 0; RNQtyG < field.GQty; RNQtyG++)
                                //  //  {
                                //  //    VarTakeRND2.n_gsisa += rsd2.n_gsisa;

                                //  //  }

                                //  //  VarTakeRND2.n_bsisa += rsd2.n_bsisa;

                                //  //}
                                //  #endregion

                                //  #region EDit RN & RS

                                //  //  for (nLoop = 0; nLoop < VarListRSD2.Count; nLoop++)
                                //  //  {
                                //  //    var VarListRSD2RN = (from q in db.LG_RSD2s
                                //  //                 where q.c_rsno == rsID && q.c_gdg == gudang &&
                                //  //                 q.c_iteno == field.Item && q.c_batch == field.Batch
                                //  //                 && q.c_rnno == VarListRSD2[nLoop].c_rnno
                                //  //                 select q).ToList();

                                //  //    //var VarTakeRSD2RN = (from q in db.LG_RSD2s
                                //  //    //                     where q.c_rsno == rsID &&
                                //  //    //                     q.c_iteno == field.Item && q.c_batch == field.Batch
                                //  //    //                     && q.c_rnno == VarListRSD2[nLoop].c_rnno
                                //  //    //                     select q).Take(1).SingleOrDefault();


                                //  //    for (nLoopC = 0; nLoopC < VarListRSD2RN.Count; nLoopC++)
                                //  //    {
                                //  //      rnd1 = (from q in db.LG_RND1s
                                //  //              where q.c_batch == field.Batch
                                //  //              && q.c_gdg == gudang && q.c_iteno == field.Item
                                //  //              && q.c_rnno == VarListRSD2RN[nLoopC].c_rnno
                                //  //              select q).Take(1).SingleOrDefault();

                                //  //      rsd2 = (from q in db.LG_RSD2s 
                                //  //             where q.c_batch == field.Batch &&
                                //  //             q.c_iteno == field.Item &&
                                //  //             q.c_gdg == gudang && q.c_rsno == rsID &&
                                //  //              q.c_rnno == VarListRSD2RN[nLoopC].c_rnno
                                //  //              select q).Take(1).SingleOrDefault();

                                //  //      #region Old School

                                //  //      //if (field.GQty == 0)
                                //  //      //{
                                //  //      //  for (int qtyloop = 0; qtyloop < qtyBRN; qtyloop++)
                                //  //      //  {
                                //  //      //    if (VarListRSD2RN[nLoopC].n_bsisa >= field.BQty)
                                //  //      //    {
                                //  //      //      rnd1.n_bsisa += VarListRSD2RN[nLoopC].n_bsisa;
                                //  //      //    }
                                //  //      //    else if (VarListRSD2RN[nLoopC].n_bsisa < field.BQty)
                                //  //      //    {
                                //  //      //      rnd1.n_bsisa += rnd1.n_bqty;
                                //  //      //      qtyBRN -= rnd1.n_bqty;
                                //  //      //    }
                                //  //      //  }
                                //  //      //}
                                //  //      //else if (field.BQty == 0)
                                //  //      //{
                                //  //      //  for (int qtyloop = 0; qtyloop < qtyGRN; qtyloop++)
                                //  //      //  {
                                //  //      //    if (VarListRSD2RN[nLoopC].n_gsisa >= field.GQty)
                                //  //      //    {
                                //  //      //      rnd1.n_gsisa += VarListRSD2RN[nLoopC].n_gsisa;
                                //  //      //    }
                                //  //      //    else if (VarListRSD2RN[nLoopC].n_gsisa < field.GQty)
                                //  //      //    {
                                //  //      //      rnd1.n_gsisa += rnd1.n_gqty;
                                //  //      //      qtyGRN -= rnd1.n_gqty;
                                //  //      //    }
                                //  //      //  }
                                //  //      //}
                                //  //      //else if (field.BQty != 0 && field.GQty != 0)
                                //  //      //{
                                //  //      //  for (int qtyloop = 0; qtyloop < qtyGRN; qtyloop++)
                                //  //      //  {
                                //  //      //    if (VarListRSD2RN[nLoopC].n_gsisa >= field.GQty)
                                //  //      //    {
                                //  //      //      rnd1.n_gsisa += VarListRSD2RN[nLoopC].n_gsisa;
                                //  //      //    }
                                //  //      //    else if (VarListRSD2RN[nLoopC].n_gsisa < field.GQty)
                                //  //      //    {
                                //  //      //      rnd1.n_gsisa += rnd1.n_gqty;
                                //  //      //      qtyGRN -= rnd1.n_gqty;
                                //  //      //    }
                                //  //      //  }
                                //  //      //  for (int qtyloop = 0; qtyloop < qtyBRN; qtyloop++)
                                //  //      //  {
                                //  //      //    if (VarListRSD2RN[nLoopC].n_bsisa >= field.BQty)
                                //  //      //    {
                                //  //      //      rnd1.n_bsisa += VarListRSD2RN[nLoopC].n_bsisa;
                                //  //      //    }
                                //  //      //    else if (VarListRSD2RN[nLoopC].n_bsisa < field.BQty)
                                //  //      //    {
                                //  //      //      rnd1.n_bsisa += rnd1.n_bqty;
                                //  //      //      qtyBRN -= rnd1.n_bqty;
                                //  //      //    }
                                //  //      //  }
                                //  //      //}

                                //  //      #endregion

                                //  //      if (VarListRSD2RN[nLoopC].n_bqty >= field.BQty || VarListRSD2RN[nLoopC].n_gqty >= field.GQty)
                                //  //      {
                                //  //        VarListRSD2RN[nLoopC].n_bsisa += field.BQty;
                                //  //        VarListRSD2RN[nLoopC].n_gsisa += field.GQty;
                                //  //        rsd2.n_bqty -= field.BQty;
                                //  //        rsd2.n_bsisa -= field.BQty;
                                //  //        rsd2.n_gqty -= field.GQty;
                                //  //        rsd2.n_gsisa -= field.GQty;
                                //  //      }
                                //  //      else if (VarListRSD2RN[nLoopC].n_bqty < field.BQty || VarListRSD2RN[nLoopC].n_gqty < field.GQty)
                                //  //      {
                                //  //        qtyBRN = field.BQty;
                                //  //        qtyGRN = field.GQty;

                                //  //        for (int QtyB = 0; QtyB < qtyBRN; QtyB++)
                                //  //        {
                                //  //          VarListRSD2RN[nLoopC].n_bsisa += VarListRSD2RN[nLoopC].n_bqty;
                                //  //          rsd2.n_bqty -= rsd2.n_bqty;
                                //  //          rsd2.n_bsisa -= rsd2.n_bqty;
                                //  //          qtyBRN -= VarListRSD2RN[nLoopC].n_bqty;
                                //  //        }
                                //  //        for (int QtyG = 0; QtyG < qtyGRN; QtyG++)
                                //  //        {
                                //  //          VarListRSD2RN[nLoopC].n_gsisa += VarListRSD2RN[nLoopC].n_gqty;
                                //  //          rsd2.n_gqty -= rsd2.n_gqty;
                                //  //          rsd2.n_gsisa -= rsd2.n_gqty;
                                //  //          qtyGRN -= VarListRSD2RN[nLoopC].n_gqty;
                                //  //        }

                                //  //        VarListRSD2RN[nLoopC].n_bsisa += field.BQty;
                                //  //        VarListRSD2RN[nLoopC].n_gsisa += field.GQty;
                                //  //      }

                                //  //      db.SubmitChanges();
                                //  //    }


                                //  //    VarListRSD2RN.Clear();
                                //  //  }

                                //  //  VarTakeRSD2.n_bqty -= field.BQty;
                                //  //  VarTakeRSD2.n_bsisa -= field.BQty;
                                //  //  VarTakeRSD2.n_gqty -= field.GQty;
                                //  //  VarTakeRSD2.n_gsisa -= field.GQty;

                                //  #endregion

                                //  #endregion
                                // }

                                #endregion

                                #endregion
                            }
                        }

                        if (ListRSD5.Count > 0 && ListRSD5 != null)
                        {
                            db.LG_RSD5s.InsertAllOnSubmit(ListRSD5.ToArray());
                            ListRSD5.Clear();
                        }
                    }

                    #endregion

                    hasAnyChanges = true;

                    #endregion
                }
                else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    #region Delete

                    if (string.IsNullOrEmpty(rsID))
                    {
                        result = "Nomor Retur Supplier dibutuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    rsh = (from q in db.LG_RSHes
                           where q.c_rsno == rsID
                           select q).Take(1).SingleOrDefault();

                    if (rsh == null)
                    {
                        result = "Nomor Retur Supplier tidak ditemukan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (rsh.l_delete.HasValue && rsh.l_delete.Value)
                    {
                        result = "Tidak dapat menghapus nomor RS yang sudah terhapus.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, rsh.d_rsdate))
                    {
                        result = "Retur Supplier tidak dapat dihapus, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.HasFBR(db, rsID))
                    {
                        result = "Retur Supplier yang sudah terdapat Faktur Beli Retur tidak dapat diubah.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
                    {
                        rsh.v_ket = structure.Fields.Keterangan;
                    }

                    rsh.c_update = nipEntry;
                    rsh.d_update = DateTime.Now;
                    rsh.l_delete = true;

                    #region Populate Detail

                    ListRSD5 = new List<LG_RSD5>();

                    ListRSD1 = (from q in db.LG_RSD1s
                                where (q.c_gdg == gudang) && (q.c_rsno == rsID)
                                select q).Distinct().ToList();
                    ListRSD2 = (from q in db.LG_RSD2s
                                where (q.c_gdg == gudang) && (q.c_rsno == rsID)
                                select q).Distinct().ToList();

                    lstTemp = ListRSD2.GroupBy(x => x.c_rnno).Select(y => y.Key).ToList();

                    ListRND1 = (from q in db.LG_RNHs
                                join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                                where (q.c_gdg == gudang) && lstTemp.Contains(q.c_rnno)
                                  && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                select q1).Distinct().ToList();

                    for (nLoop = 0; nLoop < ListRSD2.Count; nLoop++)
                    {
                        rsd2 = ListRSD2[nLoop];

                        rnd1 = ListRND1.Find(delegate(LG_RND1 rnd)
                        {
                            return (string.IsNullOrEmpty(rsd2.c_rnno) ? string.Empty : rsd2.c_rnno.Trim()).Equals((string.IsNullOrEmpty(rnd.c_rnno) ? string.Empty : rnd.c_rnno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                              (string.IsNullOrEmpty(rsd2.c_iteno) ? string.Empty : rsd2.c_iteno.Trim()).Equals((string.IsNullOrEmpty(rnd.c_iteno) ? string.Empty : rnd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                              (string.IsNullOrEmpty(rsd2.c_batch) ? string.Empty : rsd2.c_batch.Trim()).Equals((string.IsNullOrEmpty(rnd.c_batch) ? string.Empty : rnd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase);
                        });

                        GQty = (rsd2.n_gqty.HasValue ? rsd2.n_gqty.Value : 0);
                        BQty = (rsd2.n_bqty.HasValue ? rsd2.n_bqty.Value : 0);
                        GQtyAlocate = (rsd2.n_gsisa.HasValue ? rsd2.n_gsisa.Value : 0);
                        BQtyAlocate = (rsd2.n_bsisa.HasValue ? rsd2.n_bsisa.Value : 0);

                        if ((GQty != GQtyAlocate) || (BQty != BQtyAlocate))
                        {
                            result = "Retur Supplier tidak dapat dihapus karena quantity tidak sama.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }

                        if (rnd1 != null)
                        {
                            if (GQty > 0)
                            {
                                rnd1.n_gsisa += GQty;
                            }
                            if (BQty > 0)
                            {
                                rnd1.n_bsisa += BQty;
                            }
                        }

                        ListRSD5.Add(new LG_RSD5()
                        {
                            c_rsno = rsID,
                            c_iteno = rsd2.c_iteno,
                            c_batch = rsd2.c_batch,
                            c_rnno = rsd2.c_rnno,
                            n_gqty = GQty,
                            n_gsisa = GQtyAlocate,
                            n_bqty = BQty,
                            n_bsisa = BQtyAlocate,
                            v_type = "03",
                            v_ket_del = structure.Fields.Keterangan,
                            l_status = rsd2.l_status,
                            c_entry = nipEntry,
                            d_entry = date
                        });
                    }

                    if (ListRSD1 != null && ListRSD1.Count > 0
                      && ListRSD2 != null && ListRSD2.Count > 0)
                    {
                        db.LG_RSD1s.DeleteAllOnSubmit(ListRSD1.ToArray());
                        ListRSD1.Clear();
                        db.LG_RSD2s.DeleteAllOnSubmit(ListRSD2.ToArray());
                        ListRSD2.Clear();
                    }

                    if (ListRSD5 != null && ListRSD5.Count > 0)
                    {
                        db.LG_RSD5s.InsertAllOnSubmit(ListRSD5.ToArray());
                        ListRSD5.Clear();
                    }
                    lstTemp.Clear();
                    ListRND1.Clear();

                    #endregion

                    hasAnyChanges = true;

                    #endregion
                }
                else if (structure.Method.Equals("ConfirmDisposisi", StringComparison.OrdinalIgnoreCase))
                {
                    #region Confirm Disposisi
                    nipEntry = (structure.Fields.Entry ?? string.Empty);

                    for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                    {
                        field = structure.Fields.Field[nLoop];
                        rsID = field.RsId;
                        confirmDisp = field.ConfirmDisp;

                        bool isRS = (rsID.Substring(0, 2).Equals("RS"));

                        if (string.IsNullOrEmpty(rsID))
                        {
                            result = "Nomor RS dibutuhkan.";

                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                            if (db.Transaction != null)
                            {
                                db.Transaction.Rollback();
                            }

                            goto endLogic;
                        }

                        if (isRS)
                        {
                            #region RS

                            rsh = (from q in db.LG_RSHes
                                   where q.c_rsno == rsID
                                   select q).Take(1).SingleOrDefault();

                            if (rsh == null)
                            {
                                result = "Nomor RS tidak ditemukan.";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }

                                goto endLogic;
                            }
                            else if (rsh.c_type == "02")
                            {
                                result = "RS Repack tidak bisa untuk proses disposisi full";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }

                                goto endLogic;
                            }
                            else if (rsh.l_delete.HasValue && rsh.l_delete.Value)
                            {
                                result = "Tidak dapat proses disposisi full yang terhapus.";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                if (db.Transaction != null)
                                {
                                    db.Transaction.Rollback();
                                }

                                goto endLogic;
                            }

                            isConfirm = (rsh.l_confirm.HasValue ? rsh.l_confirm.Value : false);

                            if (!isConfirm)
                            {
                                rsh.l_confirm = confirmDisp;
                                rsh.d_confirm = date;
                                rsh.c_confirm = nipEntry;

                                hasAnyChanges = true;
                                result = "RS berhasil confirm disposisi full";
                            }
                            else
                            {
                                fbrd2 = (from q in db.LG_FBRD2s
                                         where q.c_rsno == rsID
                                         select q).Take(1).SingleOrDefault();
                                if (fbrd2 == null)
                                {
                                    rsh.l_confirm = false;
                                    rsh.c_confirm = nipEntry;

                                    hasAnyChanges = true;
                                    result = "RS berhasil unconfirm disposisi full";
                                }
                                else
                                {
                                    result = "Nomor RS sudah jadi faktur retur.";

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

                result = string.Format("ScmsSoaLibrary.Bussiness.Retur:ReturSuplier - {0}", ex.Message);

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

        public string ReturSuplierConfirm(ScmsSoaLibrary.Parser.Class.ReturSupplierConfStructure structure)
        {
            if ((structure == null) || (structure.Fields == null))
            {
                return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
            }

            bool hasAnyChanges = false;

            ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

            ScmsSoaLibrary.Parser.Class.ReturSupplierConfStructureField field = null;
            ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
            DateTime date = DateTime.Now;

            IDictionary<string, string> dic = null;

            char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '1' : structure.Fields.Gudang[0]);

            string result = null;
            string nipEntry = null;
            string rsID = null;
            int nLoop = 0,
              nLoopC = 0,
                //nLenC = 0,
              totalDetails = 0,
              nLoopRS = 1;
            decimal nQtyBad = 0,
              nQtyGood = 0,
              nTotal = 0,
              nTotalPerItem = 0,
              nQtyAllocGood = 0,
              nQtyAllocBad = 0;
            string tipeRSConf = null;

            List<LG_RSD1> ListRSD1 = null;
            List<LG_RSD2> ListRSD2 = null;
            List<LG_RSD2> ListRSD2Pre = null;
            List<LG_RSD6> ListRSD6 = null;
            LG_ComboH combo = null;
            LG_RND1 rnd1 = null;

            List<string> lstRSID = null;

            List<LG_RSH> ListRSHOriginal = null;
            List<LG_RSD2> ListRSD2Original = null;
            //List<LG_RSD2> ListRSD2OriginalCopy = null;

            //LG_RSH rshOriginal = null;
            //LG_RSD2 rsd2Original = null;

            LG_RSH rsh = null;
            LG_RSD1 rsd1 = null;
            LG_RSD2 rsd2 = null;

            //List<RS_ConfirmUpdate> lstRsConfirm = null;
            //RS_ConfirmUpdate rscu = null;

            nipEntry = (structure.Fields.Entry ?? string.Empty);

            if (string.IsNullOrEmpty(nipEntry))
            {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                goto endLogic;
            }

            rsID = (structure.Fields.ReturSupplierID ?? string.Empty);

            try
            {
                db.Connection.Open();

                db.Transaction = db.Connection.BeginTransaction();

                if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
                {
                    #region Add

                    if (!string.IsNullOrEmpty(rsID))
                    {
                        result = "Nomor Retur harus kosong.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (string.IsNullOrEmpty(structure.Fields.Supplier))
                    {
                        result = "Nama pemasok dibutuhkan.";

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
                    else if (string.IsNullOrEmpty(structure.Fields.NoCPR))
                    {
                        result = "No CPR Di Butuhkan.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }
                    else if (Commons.IsClosingLogistik(db, date))
                    {
                        result = "Retur Supplier tidak dapat disimpan, karena sudah closing.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }

                        goto endLogic;
                    }

                    //rshOriginal = (from q in db.LG_RSHes
                    //               where (q.c_gdg 
                    rsID = Commons.GenerateNumbering<LG_RSH>(db, "RS", '3', "04", date, "c_rsno");

                    rsh = new LG_RSH()
                    {
                        c_gdg = gudang,
                        c_nosup = structure.Fields.Supplier,
                        c_rsno = rsID,
                        c_type = "03",
                        c_update = nipEntry,
                        c_entry = nipEntry,
                        d_entry = date,
                        d_update = date,
                        d_rsdate = date,
                        l_delete = false,
                        l_print = false,
                        v_ket = structure.Fields.Keterangan,
                        c_cprno = structure.Fields.NoCPR
                    };

                    db.LG_RSHes.InsertOnSubmit(rsh);

                    if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
                    {

                        ListRSD1 = new List<LG_RSD1>();
                        ListRSD2 = new List<LG_RSD2>();
                        ListRSD2Pre = new List<LG_RSD2>();
                        ListRSD6 = new List<LG_RSD6>();
                        rnd1 = new LG_RND1();
                        combo = new LG_ComboH();

                        #region Ekstrak RSD2

                        lstRSID = structure.Fields.Field.GroupBy(x => x.NORS).Select(y => y.Key).ToList();

                        ListRSHOriginal = (from q in db.LG_RSHes
                                           where (q.c_gdg == gudang) && lstRSID.Contains(q.c_rsno)
                                            && (q.c_type == "01")
                                            && ((q.l_confirm.HasValue ? q.l_confirm.Value : false) == false)
                                            && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                           select q).Distinct().ToList();

                        for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
                        {
                            field = structure.Fields.Field[nLoop];

                            tipeRSConf = field.nBQty <= 0 ? "01" : "02";

                            nTotal = field.nQtyRed + field.nQtyRej + field.nQtyRew;
                            nTotalPerItem = field.nQty;

                            if ((nTotal > nTotalPerItem) || (nTotal <= 0))
                            {
                                continue;
                            }

                            rsh = (from q in db.LG_RSHes
                                   where (q.c_gdg == gudang) && q.c_rsno == structure.Fields.NoRS1
                                    && (q.c_type == "01")
                                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                   select q).Take(1).SingleOrDefault();

                            if (rsh != null)
                            {
                                ListRSD2Original = (from q in db.LG_RSD2s
                                                    where (q.c_gdg == gudang) && q.c_rsno == structure.Fields.NoRS1
                                                     && ((q.n_gsisa > 0) || (q.n_bsisa > 0))
                                                     && ((q.l_confirm.HasValue ? q.l_confirm.Value : false) == false)
                                                     && (q.c_iteno == field.Item) && (q.c_batch == field.Batch)
                                                    select q).Distinct().ToList();

                                if (ListRSD2Original.Count < 1)
                                {
                                    continue;
                                }

                                nTotalPerItem = ListRSD2Original.GroupBy(t => new { t.c_iteno, t.c_batch })
                                  .Select(x => x.Sum(y => ((y.n_gsisa.HasValue ? y.n_gsisa.Value : 0) + (y.n_bsisa.HasValue ? y.n_bsisa.Value : 0)))).Take(1).Single();

                                if (nTotal > nTotalPerItem)
                                {
                                    continue;
                                }

                                rsd1 = new LG_RSD1()
                                {
                                    c_gdg = gudang,
                                    c_rsno = rsID,
                                    c_iteno = field.Item,
                                    c_batch = field.Batch,
                                    c_cprno = structure.Fields.NoCPR,
                                    v_ket = "Auto",
                                    n_gqty = 0,
                                    n_bqty = nTotal
                                };

                                ListRSD1.Add(rsd1);



                                ListRSD6.Add(new LG_RSD6()
                                {
                                    n_count = nLoop,
                                    c_gdg = gudang,
                                    c_rsno = rsID,
                                    c_nosup = structure.Fields.Supplier,
                                    c_noref = rsh.c_rsno,
                                    d_rsdate = rsh.d_rsdate,
                                    c_entry = rsh.c_entry,
                                    d_entry = rsh.d_entry,
                                    c_update = rsh.c_update,
                                    d_update = rsh.d_update,
                                    v_ket = rsh.v_ket,
                                    c_iteno = field.Item,
                                    c_batch = field.Batch,
                                    n_bqty = nTotal,
                                    n_gqty = 0,
                                    n_redress = field.nQtyRed,
                                    n_reject = field.nQtyRej,
                                    n_rework = field.nQtyRew,
                                    c_type = tipeRSConf
                                });



                                for (nLoopC = 0; nLoopC < nLoopRS; nLoopC++)
                                {
                                    rsd2 = ListRSD2Original[nLoopC];

                                    nQtyBad = (rsd2.n_bsisa.HasValue ? rsd2.n_bsisa.Value : 0);
                                    nQtyGood = (rsd2.n_gsisa.HasValue ? rsd2.n_gsisa.Value : 0);

                                    rsd2.l_confirm = true;

                                    nQtyAllocBad = (nQtyBad > nTotal ? nTotal : nQtyBad);
                                    nTotal -= nQtyAllocBad;
                                    nQtyBad -= nQtyAllocBad;
                                    nQtyAllocGood = (nQtyGood > nTotal ? nTotal : nQtyGood);
                                    nTotal -= nQtyAllocGood;
                                    nQtyGood -= nQtyAllocGood;

                                    rsd2.n_bsisa = nQtyBad;
                                    rsd2.n_gsisa = nQtyGood;

                                    totalDetails++;

                                    if (field.nQtyRed > 0 || field.nQtyRew > 0)
                                    {

                                        decimal dRepc = field.nQtyRed > 0 ? field.nQtyRed : field.nQtyRew;
                                        decimal dRepcGood = 0,
                                          dRepcBad = 0;
                                        if (tipeRSConf.Equals("01"))
                                        {
                                            dRepcGood = dRepc;
                                        }
                                        else
                                        {
                                            dRepcBad = dRepc;
                                        }

                                        ListRSD2.Add(new LG_RSD2()
                                        {
                                            c_gdg = gudang,
                                            c_rsno = rsID,
                                            c_iteno = field.Item,
                                            c_batch = field.Batch,
                                            c_rnno = (string.IsNullOrEmpty(rsd2.c_rnno) ? string.Empty : rsd2.c_rnno),
                                            l_status = false,
                                            n_gqtyAcc = 0,
                                            n_gqty = 0,
                                            n_gsisa = dRepcGood,
                                            n_bqty = 0,
                                            n_bsisa = dRepcBad,
                                            n_bqtyAcc = 0,
                                            c_cprno = structure.Fields.NoCPR,
                                            l_confirm = false
                                        });
                                    }

                                    if (rsd2.c_rnno.Substring(0, 2).Equals("CB"))
                                    {
                                        combo = (from q in db.LG_ComboHs
                                                 where q.c_gdg == gudang
                                                 && q.c_batch == field.Batch
                                                 && q.c_iteno == field.Item
                                                 && q.c_combono == rsd2.c_rnno
                                                 select q).Take(1).SingleOrDefault();

                                        if (rnd1 != null)
                                        {
                                            if (field.nGQty > 0 && field.nBQty <= 0)
                                            {
                                                combo.n_gsisa += field.nQtyRej;
                                            }
                                            else
                                            {
                                                combo.n_bsisa += field.nQtyRej;
                                            }

                                            //totalDetails++;
                                        }
                                        else
                                        {
                                            result = "Tidak Ada di stock untuk batch tersebut.";

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
                                        rnd1 = (from q in db.LG_RND1s
                                                where q.c_gdg == gudang
                                                && q.c_batch == field.Batch
                                                && q.c_iteno == field.Item
                                                && q.c_rnno == rsd2.c_rnno
                                                select q).Take(1).SingleOrDefault();

                                        if (rnd1 != null)
                                        {
                                            if (field.nGQty > 0 && field.nBQty <= 0)
                                            {
                                                rnd1.n_gsisa += field.nQtyRej;
                                            }
                                            else
                                            {
                                                rnd1.n_bsisa += field.nQtyRej;
                                            }

                                            //totalDetails++;
                                        }
                                        else
                                        {
                                            result = "Tidak Ada di stock untuk batch tersebut.";

                                            rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                            if (db.Transaction != null)
                                            {
                                                db.Transaction.Rollback();
                                            }

                                            goto endLogic;
                                        }
                                    }

                                    if (nTotal <= 0)
                                    {
                                        break;
                                    }
                                }

                                //for (nLoopC = 0; nLoopC < ListRSD2Original.Count; nLoopC++)
                                //{
                                //  rsd2 = ListRSD2Original[nLoopC];

                                //  //nQtyAllocBad = nQtyBad = (rsd2.n_bsisa.HasValue ? rsd2.n_bsisa.Value : 0);
                                //  //nQtyAllocGood = nQtyGood = (rsd2.n_gsisa.HasValue ? rsd2.n_gsisa.Value : 0);
                                //  nQtyBad = (rsd2.n_bsisa.HasValue ? rsd2.n_bsisa.Value : 0);
                                //  nQtyGood = (rsd2.n_gsisa.HasValue ? rsd2.n_gsisa.Value : 0);
                                //  //nQty = (nQtyAllocBad.HasValue ? nQtyAllocBad.Value : 0) +
                                //  //  (nQtyAllocGood.HasValue ? nQtyAllocGood.Value : 0);

                                //  //nTotalPerItem = (rsd2.n_bqty.HasValue ? rsd2.n_bqty.Value : 0) +
                                //  //  (rsd2.n_gqty.HasValue ? rsd2.n_gqty.Value : 0);

                                //  rsd2.l_confirm = true;

                                //  nQtyAllocBad = (nQtyBad > nTotal ? nTotal : nQtyBad);
                                //  nTotal -= nQtyAllocBad;
                                //  nQtyBad -= nQtyAllocBad;
                                //  nQtyAllocGood = (nQtyGood > nTotal ? nTotal : nQtyGood);
                                //  nTotal -= nQtyAllocGood;
                                //  nQtyGood -= nQtyAllocGood;

                                //  ListRSD2.Add(new LG_RSD2()
                                //  {
                                //    c_gdg = gudang,
                                //    c_rsno = rsID,
                                //    c_iteno = field.Item,
                                //    c_batch = field.Batch,
                                //    c_rnno = (string.IsNullOrEmpty(rsd2.c_rnno) ? string.Empty : rsd2.c_rnno),
                                //    l_status = false,
                                //    n_gqtyAcc = nQtyAllocGood,
                                //    n_gqty = nQtyAllocGood,
                                //    n_gsisa = nQtyAllocGood,
                                //    n_bqty = nQtyAllocBad,
                                //    n_bsisa = nQtyAllocBad,
                                //    n_bqtyAcc = nQtyAllocBad,
                                //    c_cprno = structure.Fields.NoCPR,
                                //    l_confirm = false
                                //  });



                                //  rsd2.n_bsisa = nQtyBad;
                                //  rsd2.n_gsisa = nQtyGood;

                                //  totalDetails++;

                                //  if (nTotal <= 0)
                                //  {
                                //    break;
                                //  }

                                //  //if (nTotalPerItem == nQty)
                                //  //{
                                //  //  for (nLen = 0; nLen < nTotal; nLen++)
                                //  //  {
                                //  //    if (nQtyAllocBad > 0 && nTotal > 0)
                                //  //    {
                                //  //      rsd2.n_bsisa -= ((nTotal < rsd2.n_bsisa) ? nTotal : rsd2.n_bsisa);
                                //  //      Bad += ((nTotal < rsd2.n_bsisa) ? nTotal : rsd2.n_bsisa);
                                //  //      nQtyAllocBad -= ((nTotal < nQtyBad) ? nTotal : nQtyBad);
                                //  //      nTotal -= ((nTotal < nQtyBad) ? nTotal : nQtyBad);
                                //  //    }
                                //  //    if (nQtyAllocGood > 0 && nTotal > 0)
                                //  //    {
                                //  //      rsd2.n_gsisa -= ((nTotal < rsd2.n_gsisa) ? nTotal : rsd2.n_gsisa);
                                //  //      Good += ((nTotal < rsd2.n_gsisa) ? nTotal : rsd2.n_gsisa);
                                //  //      nQtyAllocGood -= ((nTotal < nQtyGood) ? nTotal : nQtyGood);
                                //  //      nTotal -= ((nTotal < nQtyGood) ? nTotal : nQtyGood);
                                //  //    }
                                //  //    totalDetails++;
                                //  //  }
                                //  //}
                                //}
                            }

                            //rshOriginal = ListRSHOriginal.Find(delegate(LG_RSH rs)
                            //{
                            //  return field.NORS.Equals((string.IsNullOrEmpty(rs.c_rsno) ? string.Empty : rs.c_rsno.Trim()), StringComparison.OrdinalIgnoreCase);
                            //});
                        }


                        #endregion

                        #region Add To Rsd2

                        //if ((field.nQtyRed == 0) && (field.nQtyRew == 0))
                        //{
                        //  for (nLoopC = 0, nLenC = ListRSD2Pre.Count; nLoopC < nLenC; nLoopC++)
                        //  {
                        //    rsd2Original = ListRSD2Pre[nLoopC];

                        //    rsd2 = ListRSD2.Find(delegate(LG_RSD2 rsd)
                        //    {
                        //      return rsd2Original.c_iteno.Equals(rsd.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                        //        rsd2Original.c_batch.Equals(rsd.c_batch, StringComparison.OrdinalIgnoreCase) &&
                        //        rsd2Original.c_rnno.Equals(rsd.c_rnno, StringComparison.OrdinalIgnoreCase);
                        //    });

                        //    if (rsd2 == null)
                        //    {
                        //      ListRSD2.Add(rsd2Original);

                        //      rsd2 = rsd2Original;
                        //    }

                        //    rsd1 = ListRSD1.Find(delegate(LG_RSD1 rsd)
                        //    {
                        //      return field.Item.Equals(rsd.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                        //        field.Batch.Equals(rsd.c_batch, StringComparison.OrdinalIgnoreCase);
                        //    });

                        //    nTotal = (rsd2.n_gsisa.HasValue ? rsd2.n_gsisa.Value : 0) +
                        //      (rsd2.n_bsisa.HasValue ? rsd2.n_bsisa.Value : 0);

                        //    if (rsd1 == null)
                        //    {
                        //      rsd1 = new LG_RSD1()
                        //      {
                        //        c_gdg = gudang,
                        //        c_rsno = rsID,
                        //        c_iteno = field.Item,
                        //        c_batch = field.Batch,
                        //        c_cprno = structure.Fields.NoCPR,
                        //        v_ket = "Auto",
                        //        n_gqty = 0,
                        //        n_bqty = nTotal
                        //      };

                        //      ListRSD1.Add(rsd1);
                        //    }
                        //    else
                        //    {
                        //      //rsd1.n_gqty += rsd2.n_gqty;
                        //      rsd1.n_bqty += nTotal;
                        //    }

                        //    totalDetails++;
                        //  }
                        //}

                        #endregion

                        #region Original Code

                        //lstRSID = structure.Fields.Field.GroupBy(x => x.NORS).Select(y => y.Key).ToList();

                        //ListRSHOriginal = (from q in db.LG_RSHes
                        //                   where (q.c_gdg == gudang) && lstRSID.Contains(q.c_rsno)
                        //                    && (q.c_type == "01") 
                        //                    && ((q.l_confirm.HasValue ? q.l_confirm.Value : false) == false)
                        //                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                        //                   select q).Distinct().ToList();

                        //ListRSD2Original = (from q in db.LG_RSHes
                        //                    join q1 in db.LG_RSD2s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                        //                    where (q.c_gdg == gudang) && lstRSID.Contains(q.c_rsno)
                        //                     && (q.c_type == "01") && ((q1.n_gsisa > 0) || (q1.n_bsisa > 0))
                        //                     && ((q.l_confirm.HasValue ? q.l_confirm.Value : false) == false)
                        //                     && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                        //                    select q1).Distinct().ToList();

                        //nLenData = 0;

                        //for (nLoop = 0, nLen = structure.Fields.Field.Length; nLoop < nLen; nLoop++)
                        //{
                        //  field = structure.Fields.Field[nLoop];

                        //  rshOriginal = ListRSHOriginal.Find(delegate(LG_RSH rs)
                        //  {
                        //    return field.NORS.Equals((string.IsNullOrEmpty(rs.c_rsno) ? string.Empty : rs.c_rsno.Trim()), StringComparison.OrdinalIgnoreCase);
                        //  });

                        //  if (rshOriginal != null)
                        //  {
                        //    if ((rshOriginal.l_confirm.HasValue ? rshOriginal.l_confirm.Value : false) == false)
                        //    {
                        //      rshOriginal.l_confirm = true;
                        //      rshOriginal.d_confirm = date;
                        //    }

                        //    rsIDOriginal = (string.IsNullOrEmpty(rshOriginal.c_rsno) ? string.Empty : rshOriginal.c_rsno.Trim());

                        //    ListRSD2OriginalCopy = ListRSD2Original.FindAll(delegate(LG_RSD2 rsd)
                        //    {
                        //      return field.NORS.Equals((string.IsNullOrEmpty(rsd.c_rsno) ? string.Empty : rsd.c_rsno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                        //        field.Item.Equals((string.IsNullOrEmpty(rsd.c_iteno) ? string.Empty : rsd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
                        //        field.Batch.Equals((string.IsNullOrEmpty(rsd.c_batch) ? string.Empty : rsd.c_batch.Trim()), StringComparison.OrdinalIgnoreCase) &&
                        //        (((rsd.n_gsisa.HasValue ? rsd.n_gsisa.Value : 0) > 0) || ((rsd.n_bsisa.HasValue ? rsd.n_bsisa.Value : 0) > 0));
                        //    });

                        //    nTotal = (field.nQtyRed + field.nQtyRew);

                        //    #region Ekstrak RSD2

                        //    for (nLoopC = 0, nLenC = ListRSD2OriginalCopy.Count, nLenData = 1; nLoopC < nLenC; nLoopC++,nLenData++)
                        //    {
                        //      rsd2 = null;

                        //      rsd2Original = ListRSD2OriginalCopy[nLoopC];

                        //      rnId = (string.IsNullOrEmpty(rsd2Original.c_rnno) ? string.Empty : rsd2Original.c_rnno.Trim());

                        //      nQtyGood = (rsd2Original.n_gsisa.HasValue ? rsd2Original.n_gsisa.Value : 0);
                        //      nQtyBad = (rsd2Original.n_bsisa.HasValue ? rsd2Original.n_bsisa.Value : 0);

                        //      if (nTotal > 0)
                        //      {
                        //        nBQtyAlloc = (((nQtyBad > 0) && (nTotal > 0)) ? ((nQtyBad > nTotal) ? nTotal : nQtyBad) : 0);
                        //        //field.nQtyRed = (field.nQtyRed > nBQtyAlloc ? nBQtyAlloc : field.nQtyRed);
                        //        nTotal -= nBQtyAlloc;
                        //        nGQtyAlloc = (((nQtyGood > 0) && (nTotal > 0))? ((nQtyGood > nTotal) ? nTotal : nQtyGood) : 0);
                        //        //field.nQtyRed = (field.nQtyRed > nGQtyAlloc ? nGQtyAlloc : field.nQtyRed);
                        //        nTotal -= nGQtyAlloc;

                        //        nQty = (nBQtyAlloc + nGQtyAlloc);

                        //        //field.nQtyRed = (((nQty > 0) && (field.nQtyRed > 0)) ?
                        //        //  ((nQty > field.nQtyRed) ? (nQty - field.nQtyRed) : ((field.nQtyRed - nQty))) : 0);
                        //        //field.nQtyRew = (((nQty > 0) && (field.nQtyRew > 0)) ?
                        //        //  ((nQty > field.nQtyRew) ? (nQty - field.nQtyRew) : ((field.nQtyRew - nQty))) : 0);

                        //        nRedSub = (nQty > field.nQtyRed ? field.nQtyRed : nQty);
                        //        nQty -= nRedSub;
                        //        nRewSub = (nQty > field.nQtyRew ? field.nQtyRew : nQty);
                        //        nQty -= nRewSub;

                        //        field.nQtyRed -= nRedSub;
                        //        field.nQtyRew -= nRewSub;

                        //        rsd2Original.n_bsisa -= nBQtyAlloc;
                        //        rsd2Original.n_gsisa -= nGQtyAlloc;

                        //        rsd2 = ListRSD2Pre.Find(delegate(LG_RSD2 rsd)
                        //        {
                        //          return field.Item.Equals(rsd.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                        //            field.Batch.Equals(rsd.c_batch, StringComparison.OrdinalIgnoreCase) &&
                        //            rnId.Equals(rsd.c_rnno, StringComparison.OrdinalIgnoreCase);
                        //        });

                        //        #region Pindah data menjadi Bat

                        //        nTotal = (nGQtyAlloc + nBQtyAlloc);

                        //        if (rsd2 == null)
                        //        {
                        //          rsd2 = new LG_RSD2()
                        //          {
                        //            c_gdg = gudang,
                        //            c_rsno = rsID,
                        //            c_iteno = field.Item,
                        //            c_batch = field.Batch,
                        //            c_rnno = (string.IsNullOrEmpty(rsd2Original.c_rnno) ? string.Empty : rsd2Original.c_rnno),
                        //            l_status = false,
                        //            n_gqtyAcc = 0,
                        //            n_gqty = 0,
                        //            n_gsisa = 0,
                        //            n_bqty = nTotal,
                        //            n_bsisa = nTotal,
                        //            n_bqtyAcc = nTotal,
                        //            c_cprno = structure.Fields.NoCPR,
                        //            l_confirm = true
                        //          };

                        //          ListRSD2Pre.Add(rsd2);
                        //        }
                        //        else
                        //        {
                        //          //rsd2.n_gqtyAcc += nGQtyAlloc;
                        //          //rsd2.n_gqty += nGQtyAlloc;
                        //          //rsd2.n_gsisa += nGQtyAlloc;
                        //          rsd2.n_bqtyAcc += nTotal;
                        //          rsd2.n_bqty += nTotal;
                        //          rsd2.n_bsisa += nTotal;
                        //        }

                        //        #endregion

                        //        #region Old Coded

                        //        //if (rsd2 == null)
                        //        //{
                        //        //  rsd2 = new LG_RSD2()
                        //        //  {
                        //        //    c_gdg = gudang,
                        //        //    c_rsno = rsID,
                        //        //    c_iteno = field.Item,
                        //        //    c_batch = field.Batch,
                        //        //    c_rnno = (string.IsNullOrEmpty(rsd2Original.c_rnno) ? string.Empty : rsd2Original.c_rnno),
                        //        //    l_status = false,
                        //        //    n_gqty = nGQtyAlloc,
                        //        //    n_gsisa = nGQtyAlloc,
                        //        //    n_bqty = nBQtyAlloc,
                        //        //    n_bsisa = nBQtyAlloc,
                        //        //    n_gqtyAcc = nGQtyAlloc,
                        //        //    n_bqtyAcc = nBQtyAlloc,
                        //        //    c_cprno = structure.Fields.NoCPR,
                        //        //    l_confirm = true
                        //        //  };

                        //        //  ListRSD2Pre.Add(rsd2);
                        //        //}
                        //        //else
                        //        //{
                        //        //  rsd2.n_gqty += nGQtyAlloc;
                        //        //  rsd2.n_gsisa += nGQtyAlloc;
                        //        //  rsd2.n_bqty += nBQtyAlloc;
                        //        //  rsd2.n_bsisa += nBQtyAlloc;
                        //        //  rsd2.n_gqtyAcc += nGQtyAlloc;
                        //        //  rsd2.n_bqtyAcc += nBQtyAlloc;
                        //        //}

                        //        #endregion

                        //        //rscu = lstRsConfirm.Find(delegate(RS_ConfirmUpdate cu)
                        //        //{
                        //        //  return (string.IsNullOrEmpty(rshOriginal.c_rsno) ? string.Empty : rshOriginal.c_rsno.Trim()).Equals((string.IsNullOrEmpty(cu.RsId) ? string.Empty : cu.RsId.Trim()), StringComparison.OrdinalIgnoreCase);
                        //        //});

                        //        //if (rscu == null)
                        //        //{
                        //        //  lstRsConfirm.Add(new RS_ConfirmUpdate()
                        //        //  {
                        //        //    Gudang = gudang,
                        //        //    RsId = (string.IsNullOrEmpty(rshOriginal.c_rsno) ? string.Empty : rshOriginal.c_rsno.Trim()),
                        //        //    RsDate = (rshOriginal.d_rsdate.HasValue ? rshOriginal.d_rsdate.Value : Functionals.StandardSqlDateTime),
                        //        //    NipCreateEntry = (string.IsNullOrEmpty(rshOriginal.c_entry) ? string.Empty : rshOriginal.c_entry),
                        //        //    CreateEntryDate = (rshOriginal.d_entry.HasValue ? rshOriginal.d_entry.Value : Functionals.StandardSqlDateTime),
                        //        //    NipUpdateEntry = (string.IsNullOrEmpty(rshOriginal.c_update) ? string.Empty : rshOriginal.c_update),
                        //        //    UpdateEntryDate = (rshOriginal.d_rsdate.HasValue ? rshOriginal.d_rsdate.Value : Functionals.StandardSqlDateTime),
                        //        //    Keterangan = (string.IsNullOrEmpty(rshOriginal.v_ket) ? string.Empty : rshOriginal.v_ket.Trim())
                        //        //  });
                        //        //}

                        //        ListRSD6.Add(new LG_RSD6()
                        //        {
                        //          n_count = nLenData,
                        //          c_gdg = gudang,
                        //          c_rsno = rsID,
                        //          c_nosup = structure.Fields.Supplier,
                        //          c_noref = rshOriginal.c_rsno,
                        //          d_rsdate = rshOriginal.d_rsdate,
                        //          c_entry = rshOriginal.c_entry,
                        //          d_entry = rshOriginal.d_entry,
                        //          c_update = rshOriginal.c_update,
                        //          d_update = rshOriginal.d_update,
                        //          v_ket = rshOriginal.v_ket,
                        //          c_iteno = rsd2Original.c_iteno,
                        //          c_batch = rsd2Original.c_batch,
                        //          n_bqty = rsd2Original.n_bqty,
                        //          n_gqty = rsd2Original.n_gqty,
                        //          n_redress = field.nQtyRed,
                        //          n_reject = field.nQtyRej,
                        //          n_rework = field.nQtyRew
                        //        });
                        //      }

                        //      if (nTotal == 0)
                        //      {
                        //        break;
                        //      }
                        //    }

                        //    #endregion

                        //    #region Add To Rsd2

                        //    if ((field.nQtyRed  == 0) && (field.nQtyRew  == 0))
                        //    {
                        //      for (nLoopC = 0, nLenC = ListRSD2Pre.Count; nLoopC < nLenC; nLoopC++)
                        //      {
                        //        rsd2Original = ListRSD2Pre[nLoopC];

                        //        rsd2 = ListRSD2.Find(delegate(LG_RSD2 rsd)
                        //        {
                        //          return rsd2Original.c_iteno.Equals(rsd.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                        //            rsd2Original.c_batch.Equals(rsd.c_batch, StringComparison.OrdinalIgnoreCase) &&
                        //            rsd2Original.c_rnno.Equals(rsd.c_rnno, StringComparison.OrdinalIgnoreCase);
                        //        });

                        //        if (rsd2 == null)
                        //        {
                        //          ListRSD2.Add(rsd2Original);

                        //          rsd2 = rsd2Original;
                        //        }

                        //        rsd1 = ListRSD1.Find(delegate(LG_RSD1 rsd)
                        //        {
                        //          return field.Item.Equals(rsd.c_iteno, StringComparison.OrdinalIgnoreCase) &&
                        //            field.Batch.Equals(rsd.c_batch, StringComparison.OrdinalIgnoreCase);
                        //        });

                        //        nTotal = (rsd2.n_gsisa.HasValue ? rsd2.n_gsisa.Value : 0) +
                        //          (rsd2.n_bsisa.HasValue ? rsd2.n_bsisa.Value : 0);

                        //        if (rsd1 == null)
                        //        {
                        //          rsd1 = new LG_RSD1()
                        //          {
                        //            c_gdg = gudang,
                        //            c_rsno = rsID,
                        //            c_iteno = field.Item,
                        //            c_batch = field.Batch,
                        //            c_cprno = structure.Fields.NoCPR,
                        //            v_ket = "Auto",
                        //            n_gqty = 0,
                        //            n_bqty = nTotal
                        //          };

                        //          ListRSD1.Add(rsd1);
                        //        }
                        //        else
                        //        {
                        //          //rsd1.n_gqty += rsd2.n_gqty;
                        //          rsd1.n_bqty += nTotal;
                        //        }

                        //        totalDetails++;
                        //      }
                        //    }

                        //    #endregion

                        //    ListRSD2OriginalCopy.Clear();
                        //    ListRSD2Pre.Clear();
                        //  }
                        //}

                        #endregion

                        #region Update Original RSH

                        if (totalDetails > 0)
                        {
                            if ((ListRSD1 != null) && (ListRSD1.Count > 0))
                            {
                                db.LG_RSD1s.InsertAllOnSubmit(ListRSD1.ToArray());
                                ListRSD1.Clear();
                            }

                            if ((ListRSD2 != null) && (ListRSD2.Count > 0))
                            {
                                db.LG_RSD2s.InsertAllOnSubmit(ListRSD2.ToArray());
                                ListRSD2.Clear();
                            }

                            if ((ListRSD6 != null) && (ListRSD6.Count > 0))
                            {
                                db.LG_RSD6s.InsertAllOnSubmit(ListRSD6.ToArray());
                                ListRSD6.Clear();
                            }
                        }
                        #endregion
                    }

                    dic = new Dictionary<string, string>();

                    if (totalDetails > 0)
                    {
                        dic.Add("rsID", rsID);
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
                rpe = ResponseParser.ResponseParserEnum.IsError;

                result = string.Format("ScmsSoaLibrary.Bussiness.ReturSuplierConf:ReturSuplierConfirm - {0}", ex.Message);

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

    #region Old Coded

    //class ReturSuplierPembelian
    //{
    //  public string ReturSupplierBeli(ScmsSoaLibrary.Parser.Class.ReturSupplierStructure structure)
    //  {

    //    if ((structure == null) || (structure.Fields == null))
    //    {
    //      return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
    //    }

    //    bool hasAnyChanges = false;

    //    #region univ_data

    //    ORMDataContext db = new ORMDataContext();

    //    ScmsSoaLibrary.Parser.Class.ReturSupplierStructureField field = null;
    //    ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
    //    DateTime date = DateTime.Now;

    //    IDictionary<string, string> dic = null;

    //    char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) || (structure.Fields.Gudang.Length < 1) ? '1' : structure.Fields.Gudang[0]);

    //    #endregion

    //    #region Declare

    //    string result = null;
    //    string nipEntry = null, RnTmp = null;
    //    string rsID = null, tmpNumbering = null;
    //    int nLoop, nLoopC;
    //    decimal? GQty = 0, BQty = 0, GQtyAlocate = 0, BQtyAlocate = 0,
    //      nQtyAllocGood = 0, nQtyAllocBad = 0;

    //    List<LG_RSD1> ListRSD1 = null;
    //    List<LG_RSD2> ListRSD2 = null;
    //    List<LG_RSD3> ListRSD3 = null;
    //    List<LG_RSD5> ListRSD5 = null;
    //    List<LG_RND1> ListRND1 = null;
    //    LG_RND1 rnd1 = null;
    //    LG_RSD2 rsd2 = null;
    //    LG_RSD1 rsd1 = null;
    //    LG_RSH rsh = null;
    //    List<LG_RNH> rnh = null;


    //    #endregion

    //    #region Authorize Nip Entrys

    //    nipEntry = (structure.Fields.Entry ?? string.Empty);

    //    if (string.IsNullOrEmpty(nipEntry))
    //    {
    //      result = "Nip penanggung jawab dibutuhkan.";

    //      rpe = ResponseParser.ResponseParserEnum.IsFailed;

    //      goto endLogic;
    //    }

    //    #endregion

    //    rsID = (structure.Fields.ReturSupplierID ?? string.Empty);

    //    try
    //    {
    //      db.Connection.Open();

    //      db.Transaction = db.Connection.BeginTransaction();

    //      if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
    //      {
    //        #region Add

    //        #region Cek Data

    //        if (!string.IsNullOrEmpty(rsID))
    //        {
    //          result = "Nomor Packing List harus kosong.";

    //          rpe = ResponseParser.ResponseParserEnum.IsFailed;

    //          goto endLogic;
    //        }

    //        if (string.IsNullOrEmpty(structure.Fields.Supplier))
    //        {
    //          result = "Nama pemasok dibutuhkan.";

    //          rpe = ResponseParser.ResponseParserEnum.IsFailed;

    //          goto endLogic;
    //        }

    //        if (string.IsNullOrEmpty(structure.Fields.Gudang))
    //        {
    //          result = "Gudang dibutuhkan.";

    //          rpe = ResponseParser.ResponseParserEnum.IsFailed;

    //          goto endLogic;
    //        }
    //        else if (Commons.IsClosingLogistik(db, date))
    //        {
    //          result = "Retur Supplier tidak dapat disimpan, karena sudah closing.";

    //          rpe = ResponseParser.ResponseParserEnum.IsFailed;

    //          if (db.Transaction != null)
    //          {
    //            db.Transaction.Rollback();
    //          }

    //          goto endLogic;
    //        }

    //        //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "RSBELI");

    //        rsID = Commons.GenerateNumbering<LG_RSH>(db, "RS", '3', "04", date, "c_rsno");

    //        #endregion

    //        #region Insert Header RS

    //        rsh = new LG_RSH()
    //        {
    //          c_gdg = gudang,
    //          c_nosup = structure.Fields.Supplier,
    //          c_rsno = rsID,
    //          c_type = "01",
    //          c_update = nipEntry,
    //          c_entry = nipEntry,
    //          d_entry = DateTime.Now,
    //          d_update = DateTime.Now,
    //          d_rsdate = DateTime.Now,
    //          l_delete = false,
    //          l_print = false,
    //          v_ket = structure.Fields.Keterangan
    //        };

    //        db.LG_RSHes.InsertOnSubmit(rsh);

    //        #region Old Code

    //        //db.SubmitChanges();

    //        //rsh = (from q in db.LG_RSHes
    //        //       where q.v_ket == tmpNumbering
    //        //       select q).Take(1).SingleOrDefault();

    //        //if (!string.IsNullOrEmpty(rsID))
    //        //{
    //        //  result = "Nomor Retur Supplier tidak dapat di raih.";

    //        //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

    //        //  goto endLogic;
    //        //}

    //        //if (rsh.c_rsno.Equals("XXXXXXXXXX"))
    //        //{
    //        //  result = "Trigger Retur Supplier tidak aktif.";

    //        //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

    //        //  goto endLogic;
    //        //}

    //        //rsh.v_ket = structure.Fields.Keterangan;

    //        //rsID = rsh.c_rsno;

    //        #endregion

    //        #endregion

    //        #region Insert Detil

    //        if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
    //        {
    //          ListRSD1 = new List<LG_RSD1>();
    //          ListRSD2 = new List<LG_RSD2>();
    //          ListRND1 = new List<LG_RND1>();
    //          rnd1 = new LG_RND1();

    //          for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
    //          {
    //            field = structure.Fields.Field[nLoop];

    //            #region Insert into RSD2

    //            var varLisRnd1 = (from q in db.LG_RND1s
    //                              where q.c_batch == field.Batch
    //                              && q.c_gdg == gudang &&
    //                              q.c_iteno == field.Item
    //                              select q).ToList();

    //            nQtyAllocGood = field.GQty;
    //            nQtyAllocBad = field.BQty;
    //            decimal? BAqum = 0, GAqum = 0, bE = 0, gE = 0, bI = 0, gI = 0;
    //            bool GLop = true, BLop = true;

    //            for (nLoopC = 0; nLoopC < varLisRnd1.Count; nLoopC++)
    //            {
    //              if (varLisRnd1[nLoopC].n_bsisa > 0 || varLisRnd1[nLoopC].n_gsisa > 0)
    //              {
    //                bI = nQtyAllocBad - varLisRnd1[nLoopC].n_bsisa.Value;
    //                gI = nQtyAllocGood - varLisRnd1[nLoopC].n_gsisa.Value;

    //                if (BLop == true)
    //                {
    //                  bE = nQtyAllocBad - bI;
    //                  BAqum = BAqum + bE;
    //                }
    //                if (GLop == true)
    //                {
    //                  gE = nQtyAllocGood - gI;
    //                  GAqum = GAqum + gE;
    //                }
    //                if (BAqum > nQtyAllocBad)
    //                {
    //                  bE = nQtyAllocBad - (BAqum - bE);
    //                  BAqum = BAqum + bE;
    //                }
    //                if (GAqum > nQtyAllocGood)
    //                {
    //                  gE = nQtyAllocGood - (GAqum - gE);
    //                  GAqum = GAqum + gE;
    //                }
    //                if (BLop == false)
    //                {
    //                  bE = 0;
    //                }
    //                if (GLop == false)
    //                {
    //                  gE = 0;
    //                }

    //                if (bE != 0 || gE != 0)
    //                {
    //                  ListRSD2.Add(new LG_RSD2()
    //                  {
    //                    c_batch = field.Batch,
    //                    c_gdg = gudang,
    //                    c_iteno = field.Item,
    //                    c_rsno = rsID,
    //                    n_bqty = bE,
    //                    n_gqty = gE,
    //                    c_rnno = varLisRnd1[nLoopC].c_rnno,
    //                    n_bsisa = bE,
    //                    n_gsisa = gE,
    //                    l_status = false
    //                  });
    //                }

    //                rnd1 = (from q in db.LG_RND1s
    //                        where q.c_gdg == gudang &&
    //                        q.c_iteno == varLisRnd1[nLoopC].c_iteno
    //                        && q.c_batch == varLisRnd1[nLoopC].c_batch
    //                        && q.c_rnno == varLisRnd1[nLoopC].c_rnno
    //                        select q).Take(1).SingleOrDefault();

    //                rnd1.n_gsisa -= gE;
    //                rnd1.n_bsisa -= bE;

    //                if (BAqum > nQtyAllocBad)
    //                {
    //                  BLop = false;
    //                }
    //                if (GAqum > nQtyAllocGood)
    //                {
    //                  GLop = false;
    //                }
    //              }                
    //            }
    //            #endregion

    //            #region Insert RSD1

    //            ListRSD1.Add(new LG_RSD1()
    //            {
    //              c_batch = field.Batch,
    //              c_cprno = field.CprNo,
    //              c_gdg = gudang,
    //              c_iteno = field.Item,
    //              c_rsno = rsID,
    //              n_bqty = field.BQty,
    //              n_gqty = field.GQty,
    //              v_ket = field.ketD
    //            });

    //            #endregion   

    //          }
    //          db.LG_RSD2s.InsertAllOnSubmit(ListRSD2.ToArray());
    //          db.LG_RSD1s.InsertAllOnSubmit(ListRSD1.ToArray());

    //          ListRSD1.Clear();
    //          ListRSD2.Clear();
    //        }

    //        #endregion

    //        #endregion
    //      }
    //      else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
    //      {
    //        #region Modify

    //        if (string.IsNullOrEmpty(rsID))
    //        {
    //          result = "Nomor Retur Supplier dibutuhkan.";

    //          rpe = ResponseParser.ResponseParserEnum.IsFailed;

    //          goto endLogic;
    //        }

    //        rsh = (from q in db.LG_RSHes
    //               where q.c_rsno == rsID
    //               select q).Take(1).SingleOrDefault();

    //        if (rsh == null)
    //        {
    //          result = "Nomor Retur Supplier tidak ditemukan.";

    //          rpe = ResponseParser.ResponseParserEnum.IsFailed;

    //          goto endLogic;
    //        }
    //        else if (rsh.l_delete.HasValue && rsh.l_delete.Value)
    //        {
    //          result = "Tidak dapat menghapus nomor RS yang sudah terhapus.";

    //          rpe = ResponseParser.ResponseParserEnum.IsFailed;

    //          goto endLogic;
    //        }
    //        else if (Commons.IsClosingLogistik(db, rsh.d_rsdate))
    //        {
    //          result = "Retur Supplier tidak dapat diubah, karena sudah closing.";

    //          rpe = ResponseParser.ResponseParserEnum.IsFailed;

    //          if (db.Transaction != null)
    //          {
    //            db.Transaction.Rollback();
    //          }

    //          goto endLogic;
    //        }
    //        else if (Commons.HasFBR(db, rsID))
    //        {
    //          result = "Retur Supplier yang sudah terdapat Faktur Beli Retur tidak dapat diubah.";

    //          rpe = ResponseParser.ResponseParserEnum.IsFailed;

    //          if (db.Transaction != null)
    //          {
    //            db.Transaction.Rollback();
    //          }

    //          goto endLogic;
    //        }

    //        if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
    //        {
    //          rsh.v_ket = structure.Fields.Keterangan;
    //        }

    //        //rsh.c_update = nipEntry;
    //        //rsh.d_update = DateTime.Now;

    //        #region Old Coded

    //        //#region Populate Detail

    //        //if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
    //        //{
    //        //  ListRSD1 = new List<LG_RSD1>();
    //        //  ListRSD2 = new List<LG_RSD2>();
    //        //  ListRND1 = new List<LG_RND1>();
    //        //  ListRSD5 = new List<LG_RSD5>();

    //        //  for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
    //        //  {
    //        //    field = structure.Fields.Field[nLoop];

    //        //    if ((field != null) && field.IsNew && (!field.IsDelete) && (!field.IsModified) && ((field.BQty > 0) || (field.GQty > 0)))
    //        //    {
    //        //      #region Add Detil

    //        //      #region Insert into RSD2

    //        //      var varLisRnd1 = (from q in db.LG_RND1s
    //        //                        where q.c_batch == field.Batch
    //        //                        && q.c_gdg == gudang &&
    //        //                        q.c_iteno == field.Item
    //        //                        select q).ToList();

    //        //      nQtyAllocGood = field.GQty;
    //        //      nQtyAllocBad = field.BQty;
    //        //      decimal? BAqum = 0, GAqum = 0, bE = 0, gE = 0, bI = 0, gI = 0;
    //        //      bool GLop = true, BLop = true;

    //        //      for (nLoopC = 0; nLoopC < varLisRnd1.Count; nLoopC++)
    //        //      {
    //        //        if (varLisRnd1[nLoopC].n_bsisa > 0 || varLisRnd1[nLoopC].n_gsisa > 0)
    //        //        {
    //        //          bI = nQtyAllocBad - varLisRnd1[nLoopC].n_bsisa.Value;
    //        //          gI = nQtyAllocGood - varLisRnd1[nLoopC].n_gsisa.Value;

    //        //          if (BLop == true)
    //        //          {
    //        //            bE = nQtyAllocBad - bI;
    //        //            BAqum = BAqum + bE;
    //        //          }
    //        //          if (GLop == true)
    //        //          {
    //        //            gE = nQtyAllocGood - gI;
    //        //            GAqum = GAqum + gE;
    //        //          }
    //        //          if (BAqum > nQtyAllocBad)
    //        //          {
    //        //            bE = nQtyAllocBad - (BAqum - bE);
    //        //            BAqum = BAqum + bE;
    //        //          }
    //        //          if (GAqum > nQtyAllocGood)
    //        //          {
    //        //            gE = nQtyAllocGood - (GAqum - gE);
    //        //            GAqum = GAqum + gE;
    //        //          }
    //        //          if (BLop == false)
    //        //          {
    //        //            bE = 0;
    //        //          }
    //        //          if (GLop == false)
    //        //          {
    //        //            gE = 0;
    //        //          }

    //        //          if (bE != 0 || gE != 0)
    //        //          {
    //        //            ListRSD2.Add(new LG_RSD2()
    //        //            {
    //        //              c_batch = field.Batch,
    //        //              c_gdg = gudang,
    //        //              c_iteno = field.Item,
    //        //              c_rsno = rsID,
    //        //              n_bqty = bE,
    //        //              n_gqty = gE,
    //        //              c_rnno = varLisRnd1[nLoopC].c_rnno,
    //        //              n_bsisa = bE,
    //        //              n_gsisa = gE,
    //        //              l_status = false
    //        //            });
    //        //          }

    //        //          rnd1 = (from q in db.LG_RND1s
    //        //                  where q.c_gdg == gudang &&
    //        //                  q.c_iteno == varLisRnd1[nLoopC].c_iteno
    //        //                  && q.c_batch == varLisRnd1[nLoopC].c_batch
    //        //                  && q.c_rnno == varLisRnd1[nLoopC].c_rnno
    //        //                  select q).Take(1).SingleOrDefault();

    //        //          rnd1.n_gsisa -= gE;
    //        //          rnd1.n_bsisa -= bE;

    //        //          if (BAqum > nQtyAllocBad)
    //        //          {
    //        //            BLop = false;
    //        //          }
    //        //          if (GAqum > nQtyAllocGood)
    //        //          {
    //        //            GLop = false;
    //        //          }
    //        //        }
    //        //      }
    //        //      #endregion

    //        //      #region Insert RSD1

    //        //      ListRSD1.Add(new LG_RSD1()
    //        //      {
    //        //        c_batch = field.Batch,
    //        //        c_cprno = field.CprNo,
    //        //        c_gdg = gudang,
    //        //        c_iteno = field.Item,
    //        //        c_rsno = rsID,
    //        //        n_bqty = field.BQty,
    //        //        n_gqty = field.GQty,
    //        //        v_ket = field.ketD
    //        //      });

    //        //      #endregion

    //        //      db.LG_RSD2s.InsertAllOnSubmit(ListRSD2.ToArray());
    //        //      db.LG_RSD1s.InsertAllOnSubmit(ListRSD1.ToArray());

    //        //      ListRSD1.Clear();
    //        //      ListRSD2.Clear();

    //        //      #endregion
    //        //    }
    //        //    else if ((field != null) && (!field.IsNew) && field.IsDelete && (!field.IsModified))
    //        //    {
    //        //      #region Delete Detil

    //        //      var VarListRSD2 = (from q in db.LG_RSD2s
    //        //                         where q.c_rsno == rsID &&
    //        //                         q.c_iteno == field.Item && q.c_batch == field.Batch
    //        //                         select q).ToList();

    //        //      if (VarListRSD2.GroupBy(x => x.c_rnno).Sum(y => y.Sum(x => x.n_gsisa)) == field.GQty &&
    //        //          VarListRSD2.GroupBy(x => x.c_rnno).Sum(y => y.Sum(x => x.n_bsisa)) == field.BQty)
    //        //      {

    //        //        #region Insert Into RSD5

    //        //        ListRSD2 = (from q in db.LG_RSD2s
    //        //                    where q.c_gdg == gudang && q.c_iteno == field.Item
    //        //                    && q.c_rsno == rsID && q.c_batch == field.Batch
    //        //                    select q).ToList();

    //        //        for (nLoopC = 0; nLoopC < ListRSD2.Count; nLoopC++)
    //        //        {

    //        //          var VarTakeRND2 = (from q in db.LG_RND1s
    //        //                             where q.c_iteno == field.Item && q.c_gdg == gudang
    //        //                             && q.c_batch == field.Batch
    //        //                             && q.c_rnno == ListRSD2[nLoopC].c_rnno
    //        //                             select q).Take(1).SingleOrDefault();

    //        //          ListRSD5.Add(new LG_RSD5()
    //        //          {
    //        //            c_batch = ListRSD2[nLoopC].c_batch,
    //        //            c_iteno = ListRSD2[nLoopC].c_iteno,
    //        //            c_rsno = rsID,
    //        //            n_bqty = ListRSD2[nLoopC].n_bqty,
    //        //            n_gqty = ListRSD2[nLoopC].n_bsisa,
    //        //            c_rnno = ListRSD2[nLoopC].c_rnno,
    //        //            l_status = ListRSD2[nLoopC].l_status,
    //        //            n_bsisa = ListRSD2[nLoopC].n_bsisa,
    //        //            n_gsisa = ListRSD2[nLoopC].n_gsisa,
    //        //            c_entry = nipEntry,
    //        //            d_entry = DateTime.Now,
    //        //            v_ket_del = field.Keterangan,
    //        //            v_type = "02"
    //        //          });

    //        //          db.LG_RSD5s.InsertAllOnSubmit(ListRSD5.ToArray());

    //        //          if (VarTakeRND2.n_gsisa >= ListRSD2[nLoopC].n_gsisa)
    //        //          {
    //        //            VarTakeRND2.n_gsisa += ListRSD2[nLoopC].n_gsisa;
    //        //          }
    //        //          else
    //        //          {
    //        //            VarTakeRND2.n_gsisa = VarTakeRND2.n_gqty;
    //        //          }
    //        //        }

    //        //        #endregion

    //        //        #region delete RSD1 dan RSD2

    //        //        ListRSD1 = (from q in db.LG_RSD1s
    //        //                    where q.c_rsno == rsID && q.c_iteno == field.Item
    //        //                    && q.c_gdg == gudang && q.c_batch == field.Batch
    //        //                    select q).ToList();

    //        //        ListRSD3 = (from q in db.LG_RSD3s
    //        //                    where q.c_rsno == rsID && q.c_iteno == field.Item
    //        //                    && q.c_gdg == gudang && q.c_batch == field.Batch
    //        //                    select q).ToList();

    //        //        db.LG_RSD1s.DeleteAllOnSubmit(ListRSD1.ToArray());
    //        //        db.LG_RSD2s.DeleteAllOnSubmit(ListRSD2.ToArray());

    //        //        db.SubmitChanges();

    //        //        #endregion
    //        //      }

    //        //      #endregion
    //        //    }
    //        //    else if ((field != null) && (!field.IsNew) && (!field.IsDelete) && field.IsModified)
    //        //    {
    //        //      #region Update Detil

    //        //      var VarListRSD2 = (from q in db.LG_RSD2s
    //        //                         where q.c_rsno == rsID &&
    //        //                         q.c_iteno == field.Item && q.c_batch == field.Batch
    //        //                         select q).ToList();

    //        //      decimal? qtyGRN = 0, qtyGRND1 = 0;
    //        //      decimal? qtyBRN = 0, qtyBRND1 = 0;
    //        //      bool minG = false;
    //        //      bool minB = false;

    //        //      qtyGRND1 = field.GQty;
    //        //      qtyBRND1 = field.BQty;

    //        //      if (field.GQty >= 0)
    //        //      {
    //        //        qtyGRN = field.GQty;
    //        //      }
    //        //      else if (field.GQty < 0)
    //        //      {
    //        //        qtyGRN = field.GQty * -1;
    //        //        minG = true;
    //        //      }

    //        //      if (field.BQty >= 0)
    //        //      {
    //        //        qtyBRN = field.BQty;
    //        //      }
    //        //      else if (field.GQty < 0)
    //        //      {
    //        //        qtyBRN = field.BQty * -1;
    //        //        minB = true;
    //        //      }

    //        //      if (VarListRSD2.GroupBy(x => x.c_rnno).Sum(y => y.Sum(x => x.n_gsisa)) >= field.GQty &&
    //        //          VarListRSD2.GroupBy(x => x.c_rnno).Sum(y => y.Sum(x => x.n_bsisa)) >= field.BQty)
    //        //      {

    //        //        #region Check Data RSD2 GOOD

    //        //        for (nLoop = 0; nLoop < VarListRSD2.Count; nLoop++)
    //        //        {
    //        //          var VarTakeRSD2 = (from q in db.LG_RSD2s
    //        //                             where q.c_rsno == rsID &&
    //        //                              q.c_iteno == field.Item && q.c_batch == field.Batch
    //        //                              && q.c_rnno == VarListRSD2[nLoop].c_rnno
    //        //                             select q).Take(1).SingleOrDefault();


    //        //          if (qtyGRN > VarTakeRSD2.n_gqty && qtyGRN != 0)
    //        //          {
    //        //            ListRSD2.Add(new LG_RSD2()
    //        //            {
    //        //              c_gdg = gudang,
    //        //              c_iteno = VarTakeRSD2.c_iteno,
    //        //              c_rsno = rsID,
    //        //              n_gqty = VarTakeRSD2.n_gqty,
    //        //              c_rnno = VarTakeRSD2.c_rnno
    //        //            });
    //        //            qtyGRN -= VarTakeRSD2.n_gqty;
    //        //          }
    //        //          else if (qtyGRN < VarTakeRSD2.n_gqty && qtyGRN != 0)
    //        //          {
    //        //            ListRSD2.Add(new LG_RSD2()
    //        //            {
    //        //              c_gdg = gudang,
    //        //              c_iteno = VarTakeRSD2.c_iteno,
    //        //              c_rsno = rsID,
    //        //              n_gqty = qtyGRN,
    //        //              c_rnno = VarTakeRSD2.c_rnno
    //        //            });

    //        //            qtyGRN -= qtyGRN;
    //        //          }

    //        //        }

    //        //        #endregion

    //        //        #region Exekusi RSD2 Good Stock

    //        //        for (nLoop = 0; nLoop < ListRSD2.Count; nLoop++)
    //        //        {

    //        //          rnd1 = (from q in db.LG_RND1s
    //        //                  where q.c_gdg == gudang && q.c_rnno == ListRSD2[nLoop].c_rnno
    //        //                  && q.c_iteno == field.Item && q.c_batch == field.Batch
    //        //                  select q).Take(1).SingleOrDefault();

    //        //          rsd2 = (from q in db.LG_RSD2s
    //        //                  where q.c_rsno == rsID && q.c_gdg == gudang &&
    //        //                   q.c_iteno == field.Item && q.c_batch == field.Batch
    //        //                   && q.c_rnno == ListRSD2[nLoop].c_rnno
    //        //                  select q).Take(1).SingleOrDefault();


    //        //          ListRSD5.Add(new LG_RSD5()
    //        //          {
    //        //            c_batch = ListRSD2[nLoop].c_batch,
    //        //            c_iteno = ListRSD2[nLoop].c_iteno,
    //        //            c_rsno = rsID,
    //        //            n_bqty = 0,
    //        //            n_gqty = ListRSD2[nLoop].n_gqty,
    //        //            c_rnno = ListRSD2[nLoop].c_rnno,
    //        //            l_status = ListRSD2[nLoop].l_status,
    //        //            n_bsisa = 0,
    //        //            n_gsisa = ListRSD2[nLoop].n_gsisa,
    //        //            c_entry = nipEntry,
    //        //            d_entry = DateTime.Now,
    //        //            v_ket_del = field.Keterangan,
    //        //            v_type = "02"
    //        //          });

    //        //          if (minG == true)
    //        //          {
    //        //            rnd1.n_gsisa += ListRSD2[nLoop].n_gqty;
    //        //            rsd2.n_gqty -= ListRSD2[nLoop].n_gqty;
    //        //            rsd2.n_gsisa -= ListRSD2[nLoop].n_gqty;
    //        //          }
    //        //          else if (minG == false)
    //        //          {
    //        //            rnd1.n_gsisa -= ListRSD2[nLoop].n_gqty;
    //        //            rsd2.n_gqty += ListRSD2[nLoop].n_gqty;
    //        //            rsd2.n_gsisa += ListRSD2[nLoop].n_gqty;
    //        //          }

    //        //        }

    //        //        #endregion

    //        //        #region eksekusi RSD1 GOOD

    //        //        rsd1 = (from q in db.LG_RSD1s
    //        //                where q.c_rsno == rsID && q.c_gdg == gudang &&
    //        //                q.c_iteno == field.Item && q.c_batch == field.Batch
    //        //                select q).Take(1).SingleOrDefault();


    //        //        rsd1.n_gqty += qtyGRND1;

    //        //        ListRSD2.Clear();

    //        //        #endregion

    //        //        db.LG_RSD5s.InsertAllOnSubmit(ListRSD5.ToArray());

    //        //        db.SubmitChanges();

    //        //        #region Check Data RSD2 BAD

    //        //        for (nLoop = 0; nLoop < VarListRSD2.Count; nLoop++)
    //        //        {
    //        //          var VarTakeRSD2 = (from q in db.LG_RSD2s
    //        //                             where q.c_rsno == rsID &&
    //        //                              q.c_iteno == field.Item && q.c_batch == field.Batch
    //        //                              && q.c_rnno == VarListRSD2[nLoop].c_rnno
    //        //                             select q).Take(1).SingleOrDefault();


    //        //          if (qtyBRN > VarTakeRSD2.n_bqty && qtyBRN != 0)
    //        //          {
    //        //            ListRSD2.Add(new LG_RSD2()
    //        //            {
    //        //              c_gdg = gudang,
    //        //              c_iteno = VarTakeRSD2.c_iteno,
    //        //              c_rsno = rsID,
    //        //              n_bqty = VarTakeRSD2.n_bqty,
    //        //              c_rnno = VarTakeRSD2.c_rnno
    //        //            });
    //        //            qtyBRN -= VarTakeRSD2.n_bqty;
    //        //          }
    //        //          else if (qtyBRN < VarTakeRSD2.n_bqty && qtyBRN != 0)
    //        //          {
    //        //            ListRSD2.Add(new LG_RSD2()
    //        //            {
    //        //              c_gdg = gudang,
    //        //              c_iteno = VarTakeRSD2.c_iteno,
    //        //              c_rsno = rsID,
    //        //              n_bqty = qtyBRN,
    //        //              c_rnno = VarTakeRSD2.c_rnno
    //        //            });

    //        //            qtyBRN -= qtyBRN;
    //        //          }

    //        //        }

    //        //        #endregion

    //        //        #region Exekusi RSD2 Bad Stock

    //        //        for (nLoop = 0; nLoop < ListRSD2.Count; nLoop++)
    //        //        {

    //        //          rnd1 = (from q in db.LG_RND1s
    //        //                  where q.c_gdg == gudang && q.c_rnno == ListRSD2[nLoop].c_rnno
    //        //                  && q.c_iteno == field.Item && q.c_batch == field.Batch
    //        //                  select q).Take(1).SingleOrDefault();

    //        //          rsd2 = (from q in db.LG_RSD2s
    //        //                  where q.c_rsno == rsID && q.c_gdg == gudang &&
    //        //                   q.c_iteno == field.Item && q.c_batch == field.Batch
    //        //                   && q.c_rnno == ListRSD2[nLoop].c_rnno
    //        //                  select q).Take(1).SingleOrDefault();


    //        //          ListRSD5.Add(new LG_RSD5()
    //        //          {
    //        //            c_batch = ListRSD2[nLoop].c_batch,
    //        //            c_iteno = ListRSD2[nLoop].c_iteno,
    //        //            c_rsno = rsID,
    //        //            n_bqty = ListRSD2[nLoop].n_bqty,
    //        //            n_gqty = 0,
    //        //            c_rnno = ListRSD2[nLoop].c_rnno,
    //        //            l_status = ListRSD2[nLoop].l_status,
    //        //            n_bsisa = ListRSD2[nLoop].n_bsisa,
    //        //            n_gsisa = 0,
    //        //            c_entry = nipEntry,
    //        //            d_entry = DateTime.Now,
    //        //            v_ket_del = field.Keterangan,
    //        //            v_type = "02"
    //        //          });

    //        //          if (minB == true)
    //        //          {
    //        //            rnd1.n_bsisa += ListRSD2[nLoop].n_bqty;
    //        //            rsd2.n_bqty -= ListRSD2[nLoop].n_bqty;
    //        //            rsd2.n_bsisa -= ListRSD2[nLoop].n_bqty;
    //        //          }
    //        //          else if (minB == false)
    //        //          {
    //        //            rnd1.n_bsisa -= ListRSD2[nLoop].n_bqty;
    //        //            rsd2.n_bqty += ListRSD2[nLoop].n_bqty;
    //        //            rsd2.n_bsisa += ListRSD2[nLoop].n_bqty;
    //        //          }

    //        //        }

    //        //        #endregion

    //        //        #region eksekusi RSD1 BAD

    //        //        rsd1 = (from q in db.LG_RSD1s
    //        //                where q.c_rsno == rsID && q.c_gdg == gudang &&
    //        //                q.c_iteno == field.Item && q.c_batch == field.Batch
    //        //                select q).Take(1).SingleOrDefault();


    //        //        rsd1.n_bqty += qtyBRND1;

    //        //        ListRSD2.Clear();

    //        //        #endregion

    //        //        db.LG_RSD5s.InsertAllOnSubmit(ListRSD5.ToArray());

    //        //        db.SubmitChanges();

    //        //        #region Old Code

    //        //        #region Insert Into RSD5

    //        //        //ListRSD2 = (from q in db.LG_RSD2s
    //        //        //            where q.c_gdg == gudang && q.c_iteno == field.Item
    //        //        //            && q.c_rsno == rsID && q.c_batch == field.Batch
    //        //        //            select q).ToList();

    //        //        //decimal ? RNQtyGAloc = field.GQty;
    //        //        //decimal ? RNBtyGAloc = field.BQty;

    //        //        //for (nLoopC = 0; nLoopC < ListRSD2.Count; nLoopC++)
    //        //        //{

    //        //        //  var VarTakeRND2 = (from q in db.LG_RND1s
    //        //        //                     where q.c_iteno == field.Item
    //        //        //                     && q.c_batch == field.Batch
    //        //        //                     && q.c_rnno == ListRSD2[nLoopC].c_rnno
    //        //        //                     select q).Take(1).SingleOrDefault();


    //        //        //  rsd2 = (from q in db.LG_RSD2s
    //        //        //          where q.c_gdg == gudang && q.c_iteno == field.Item
    //        //        //            && q.c_rsno == rsID && q.c_batch == field.Batch
    //        //        //            && q.c_rnno == ListRSD2[nLoopC].c_rnno
    //        //        //            select q).Take(1).SingleOrDefault();

    //        //        //  ListRSD5.Add(new LG_RSD5()
    //        //        //  {
    //        //        //    c_batch = ListRSD2[nLoopC].c_batch,
    //        //        //    c_iteno = ListRSD2[nLoopC].c_iteno,
    //        //        //    c_rsno = rsID,
    //        //        //    n_bqty = ListRSD2[nLoopC].n_bqty,
    //        //        //    n_gqty = ListRSD2[nLoopC].n_gqty,
    //        //        //    c_rnno = ListRSD2[nLoopC].c_rnno,
    //        //        //    l_status = ListRSD2[nLoopC].l_status,
    //        //        //    n_bsisa = ListRSD2[nLoopC].n_bsisa,
    //        //        //    n_gsisa = ListRSD2[nLoopC].n_gsisa,
    //        //        //    c_entry = nipEntry,
    //        //        //    d_entry = DateTime.Now,
    //        //        //    v_ket_del = field.Keterangan,
    //        //        //    v_type = "02"
    //        //        //  });

    //        //        //  db.LG_RSD5s.InsertAllOnSubmit(ListRSD5.ToArray());

    //        //        //  for (int RNQtyG = 0; RNQtyG < field.GQty; RNQtyG++)
    //        //        //  {
    //        //        //    VarTakeRND2.n_gsisa += rsd2.n_gsisa;

    //        //        //  }

    //        //        //  VarTakeRND2.n_bsisa += rsd2.n_bsisa;

    //        //        //}
    //        //        #endregion

    //        //        #region EDit RN & RS

    //        //        //  for (nLoop = 0; nLoop < VarListRSD2.Count; nLoop++)
    //        //        //  {
    //        //        //    var VarListRSD2RN = (from q in db.LG_RSD2s
    //        //        //                 where q.c_rsno == rsID && q.c_gdg == gudang &&
    //        //        //                 q.c_iteno == field.Item && q.c_batch == field.Batch
    //        //        //                 && q.c_rnno == VarListRSD2[nLoop].c_rnno
    //        //        //                 select q).ToList();

    //        //        //    //var VarTakeRSD2RN = (from q in db.LG_RSD2s
    //        //        //    //                     where q.c_rsno == rsID &&
    //        //        //    //                     q.c_iteno == field.Item && q.c_batch == field.Batch
    //        //        //    //                     && q.c_rnno == VarListRSD2[nLoop].c_rnno
    //        //        //    //                     select q).Take(1).SingleOrDefault();


    //        //        //    for (nLoopC = 0; nLoopC < VarListRSD2RN.Count; nLoopC++)
    //        //        //    {
    //        //        //      rnd1 = (from q in db.LG_RND1s
    //        //        //              where q.c_batch == field.Batch
    //        //        //              && q.c_gdg == gudang && q.c_iteno == field.Item
    //        //        //              && q.c_rnno == VarListRSD2RN[nLoopC].c_rnno
    //        //        //              select q).Take(1).SingleOrDefault();

    //        //        //      rsd2 = (from q in db.LG_RSD2s 
    //        //        //             where q.c_batch == field.Batch &&
    //        //        //             q.c_iteno == field.Item &&
    //        //        //             q.c_gdg == gudang && q.c_rsno == rsID &&
    //        //        //              q.c_rnno == VarListRSD2RN[nLoopC].c_rnno
    //        //        //              select q).Take(1).SingleOrDefault();

    //        //        //      #region Old School

    //        //        //      //if (field.GQty == 0)
    //        //        //      //{
    //        //        //      //  for (int qtyloop = 0; qtyloop < qtyBRN; qtyloop++)
    //        //        //      //  {
    //        //        //      //    if (VarListRSD2RN[nLoopC].n_bsisa >= field.BQty)
    //        //        //      //    {
    //        //        //      //      rnd1.n_bsisa += VarListRSD2RN[nLoopC].n_bsisa;
    //        //        //      //    }
    //        //        //      //    else if (VarListRSD2RN[nLoopC].n_bsisa < field.BQty)
    //        //        //      //    {
    //        //        //      //      rnd1.n_bsisa += rnd1.n_bqty;
    //        //        //      //      qtyBRN -= rnd1.n_bqty;
    //        //        //      //    }
    //        //        //      //  }
    //        //        //      //}
    //        //        //      //else if (field.BQty == 0)
    //        //        //      //{
    //        //        //      //  for (int qtyloop = 0; qtyloop < qtyGRN; qtyloop++)
    //        //        //      //  {
    //        //        //      //    if (VarListRSD2RN[nLoopC].n_gsisa >= field.GQty)
    //        //        //      //    {
    //        //        //      //      rnd1.n_gsisa += VarListRSD2RN[nLoopC].n_gsisa;
    //        //        //      //    }
    //        //        //      //    else if (VarListRSD2RN[nLoopC].n_gsisa < field.GQty)
    //        //        //      //    {
    //        //        //      //      rnd1.n_gsisa += rnd1.n_gqty;
    //        //        //      //      qtyGRN -= rnd1.n_gqty;
    //        //        //      //    }
    //        //        //      //  }
    //        //        //      //}
    //        //        //      //else if (field.BQty != 0 && field.GQty != 0)
    //        //        //      //{
    //        //        //      //  for (int qtyloop = 0; qtyloop < qtyGRN; qtyloop++)
    //        //        //      //  {
    //        //        //      //    if (VarListRSD2RN[nLoopC].n_gsisa >= field.GQty)
    //        //        //      //    {
    //        //        //      //      rnd1.n_gsisa += VarListRSD2RN[nLoopC].n_gsisa;
    //        //        //      //    }
    //        //        //      //    else if (VarListRSD2RN[nLoopC].n_gsisa < field.GQty)
    //        //        //      //    {
    //        //        //      //      rnd1.n_gsisa += rnd1.n_gqty;
    //        //        //      //      qtyGRN -= rnd1.n_gqty;
    //        //        //      //    }
    //        //        //      //  }
    //        //        //      //  for (int qtyloop = 0; qtyloop < qtyBRN; qtyloop++)
    //        //        //      //  {
    //        //        //      //    if (VarListRSD2RN[nLoopC].n_bsisa >= field.BQty)
    //        //        //      //    {
    //        //        //      //      rnd1.n_bsisa += VarListRSD2RN[nLoopC].n_bsisa;
    //        //        //      //    }
    //        //        //      //    else if (VarListRSD2RN[nLoopC].n_bsisa < field.BQty)
    //        //        //      //    {
    //        //        //      //      rnd1.n_bsisa += rnd1.n_bqty;
    //        //        //      //      qtyBRN -= rnd1.n_bqty;
    //        //        //      //    }
    //        //        //      //  }
    //        //        //      //}

    //        //        //      #endregion

    //        //        //      if (VarListRSD2RN[nLoopC].n_bqty >= field.BQty || VarListRSD2RN[nLoopC].n_gqty >= field.GQty)
    //        //        //      {
    //        //        //        VarListRSD2RN[nLoopC].n_bsisa += field.BQty;
    //        //        //        VarListRSD2RN[nLoopC].n_gsisa += field.GQty;
    //        //        //        rsd2.n_bqty -= field.BQty;
    //        //        //        rsd2.n_bsisa -= field.BQty;
    //        //        //        rsd2.n_gqty -= field.GQty;
    //        //        //        rsd2.n_gsisa -= field.GQty;
    //        //        //      }
    //        //        //      else if (VarListRSD2RN[nLoopC].n_bqty < field.BQty || VarListRSD2RN[nLoopC].n_gqty < field.GQty)
    //        //        //      {
    //        //        //        qtyBRN = field.BQty;
    //        //        //        qtyGRN = field.GQty;

    //        //        //        for (int QtyB = 0; QtyB < qtyBRN; QtyB++)
    //        //        //        {
    //        //        //          VarListRSD2RN[nLoopC].n_bsisa += VarListRSD2RN[nLoopC].n_bqty;
    //        //        //          rsd2.n_bqty -= rsd2.n_bqty;
    //        //        //          rsd2.n_bsisa -= rsd2.n_bqty;
    //        //        //          qtyBRN -= VarListRSD2RN[nLoopC].n_bqty;
    //        //        //        }
    //        //        //        for (int QtyG = 0; QtyG < qtyGRN; QtyG++)
    //        //        //        {
    //        //        //          VarListRSD2RN[nLoopC].n_gsisa += VarListRSD2RN[nLoopC].n_gqty;
    //        //        //          rsd2.n_gqty -= rsd2.n_gqty;
    //        //        //          rsd2.n_gsisa -= rsd2.n_gqty;
    //        //        //          qtyGRN -= VarListRSD2RN[nLoopC].n_gqty;
    //        //        //        }

    //        //        //        VarListRSD2RN[nLoopC].n_bsisa += field.BQty;
    //        //        //        VarListRSD2RN[nLoopC].n_gsisa += field.GQty;
    //        //        //      }

    //        //        //      db.SubmitChanges();
    //        //        //    }


    //        //        //    VarListRSD2RN.Clear();
    //        //        //  }

    //        //        //  VarTakeRSD2.n_bqty -= field.BQty;
    //        //        //  VarTakeRSD2.n_bsisa -= field.BQty;
    //        //        //  VarTakeRSD2.n_gqty -= field.GQty;
    //        //        //  VarTakeRSD2.n_gsisa -= field.GQty;

    //        //        #endregion

    //        //        #endregion
    //        //      }

    //        //      db.SubmitChanges();
    //        //      #endregion
    //        //    }
    //        //  }
    //        //}

    //        //#endregion

    //        #endregion

    //        #endregion
    //      }
    //      else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
    //      {
    //        #region Delete

    //        if (string.IsNullOrEmpty(rsID))
    //        {
    //          result = "Nomor Retur Supplier dibutuhkan.";

    //          rpe = ResponseParser.ResponseParserEnum.IsFailed;

    //          goto endLogic;
    //        }

    //        rsh = (from q in db.LG_RSHes
    //               where q.c_rsno == rsID
    //               select q).Take(1).SingleOrDefault();

    //        if (rsh == null)
    //        {
    //          result = "Nomor Retur Supplier tidak ditemukan.";

    //          rpe = ResponseParser.ResponseParserEnum.IsFailed;

    //          goto endLogic;
    //        }
    //        else if (rsh.l_delete.HasValue && rsh.l_delete.Value)
    //        {
    //          result = "Tidak dapat menghapus nomor RS yang sudah terhapus.";

    //          rpe = ResponseParser.ResponseParserEnum.IsFailed;

    //          goto endLogic;
    //        }
    //        else if (Commons.IsClosingLogistik(db, rsh.d_rsdate))
    //        {
    //          result = "Retur Supplier tidak dapat dihapus, karena sudah closing.";

    //          rpe = ResponseParser.ResponseParserEnum.IsFailed;

    //          if (db.Transaction != null)
    //          {
    //            db.Transaction.Rollback();
    //          }

    //          goto endLogic;
    //        }
    //        else if (Commons.HasFBR(db, rsID))
    //        {
    //          result = "Retur Supplier yang sudah terdapat Faktur Beli Retur tidak dapat diubah.";

    //          rpe = ResponseParser.ResponseParserEnum.IsFailed;

    //          if (db.Transaction != null)
    //          {
    //            db.Transaction.Rollback();
    //          }

    //          goto endLogic;
    //        }

    //        if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
    //        {
    //          rsh.v_ket = structure.Fields.Keterangan;
    //        }

    //        rsh.c_update = nipEntry;
    //        rsh.d_update = DateTime.Now;
    //        rsh.l_delete = true;

    //        #region Populate Detail

    //        var VarList = (from q in db.LG_RSD1s
    //                       where q.c_rsno == rsID
    //                       select q).ToList();

    //        if (VarList.Count > 0)
    //        {
    //          ListRSD1 = new List<LG_RSD1>();
    //          ListRSD2 = new List<LG_RSD2>();
    //          ListRND1 = new List<LG_RND1>();
    //          ListRSD5 = new List<LG_RSD5>();


    //          for (nLoop = 0; nLoop < VarList.Count; nLoop++)
    //          {

    //            ScmsModel.LG_RSD1 lis = VarList[nLoop];

    //            var VarListRSD2 = (from q in db.LG_RSD2s
    //                               where q.c_rsno == rsID &&
    //                               q.c_iteno == lis.c_iteno && q.c_batch == lis.c_batch
    //                               select q).ToList();

    //            if (VarListRSD2.GroupBy(x => x.c_rnno).Sum(y => y.Sum(x => x.n_gsisa)) == lis.n_gqty &&
    //                VarListRSD2.GroupBy(x => x.c_rnno).Sum(y => y.Sum(x => x.n_bsisa)) == lis.n_bqty)
    //            {

    //              ListRSD2 = (from q in db.LG_RSD2s
    //                          where q.c_gdg == gudang && q.c_iteno == lis.c_iteno
    //                          && q.c_rsno == rsID && q.c_batch == lis.c_batch
    //                          select q).ToList();

    //              #region Insert Into RSD5 && update RND1

    //              for (nLoopC = 0; nLoopC < ListRSD2.Count; nLoopC++)
    //              {

    //                var VarTakeRND2 = (from q in db.LG_RND1s
    //                                   where q.c_iteno == lis.c_iteno
    //                                   && q.c_batch == lis.c_batch
    //                                   && q.c_rnno == ListRSD2[nLoopC].c_rnno
    //                                   select q).Take(1).SingleOrDefault();

    //                ListRSD5.Add(new LG_RSD5()
    //                {

    //                  c_batch = ListRSD2[nLoopC].c_batch,
    //                  c_iteno = ListRSD2[nLoopC].c_iteno,
    //                  c_rsno = rsID,
    //                  n_bqty = ListRSD2[nLoopC].n_bqty,
    //                  n_gqty = ListRSD2[nLoopC].n_bsisa,
    //                  c_rnno = ListRSD2[nLoopC].c_rnno,
    //                  l_status = ListRSD2[nLoopC].l_status,
    //                  n_bsisa = ListRSD2[nLoopC].n_gqty,
    //                  n_gsisa = ListRSD2[nLoopC].n_gsisa,
    //                  c_entry = nipEntry,
    //                  d_entry = DateTime.Now,
    //                  v_ket_del = structure.Fields.Keterangan,
    //                  v_type = "03"
    //                });

    //                VarTakeRND2.n_gsisa += ListRSD2[nLoopC].n_gsisa;

    //                db.LG_RSD5s.InsertAllOnSubmit(ListRSD5.ToArray());


    //              }
    //              db.SubmitChanges();



    //              #endregion

    //              #region delete RSD1 dan RSD2

    //              ListRSD1 = (from q in db.LG_RSD1s
    //                          where q.c_rsno == rsID && q.c_iteno == lis.c_iteno
    //                          && q.c_gdg == gudang && q.c_batch == lis.c_batch
    //                          select q).ToList();

    //              db.LG_RSD1s.DeleteAllOnSubmit(ListRSD1.ToArray());
    //              db.LG_RSD2s.DeleteAllOnSubmit(ListRSD2.ToArray());

    //              #endregion

    //              db.SubmitChanges();

    //              ListRSD1.Clear();
    //              ListRSD2.Clear();
    //              ListRSD5.Clear();
    //            }
    //          }
    //        }
    //        #endregion

    //        #endregion
    //      }

    //      if (hasAnyChanges)
    //      {
    //        db.SubmitChanges();

    //        db.Transaction.Commit();

    //        rpe = ResponseParser.ResponseParserEnum.IsSuccess;
    //      }
    //      else
    //      {
    //        db.Transaction.Rollback();

    //        rpe = ResponseParser.ResponseParserEnum.IsFailed;
    //      }
    //    }
    //    catch (Exception ex)
    //    {
    //      if (db.Transaction != null)
    //      {
    //        db.Transaction.Rollback();
    //      }

    //      result = string.Format("ScmsSoaLibrary.Bussiness.ReturSuplierPembelian:ReturSupplierPembelian - {0}", ex.Message);

    //      Logger.WriteLine(result, true);
    //    }

    //  endLogic:
    //    result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

    //    if (dic != null)
    //    {
    //      dic.Clear();
    //    }

    //    db.Dispose();

    //    return result;
    //  }
    //}

    #endregion
}