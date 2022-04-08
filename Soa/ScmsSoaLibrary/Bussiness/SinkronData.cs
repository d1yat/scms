using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsModel;
using ScmsSoaLibrary.Parser;
//using ScmsSoaLibrary.Core.Crypto;
using ScmsSoaLibrary.Commons;
using System.Data.Common;
using ScmsSoaLibrary.Core.Threading;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaLibrary.Bussiness
{
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

  internal class ReceiveRelocation
  {
      public string c_iteno { get; set; }
      public string c_batch { get; set; }
      public string c_spno { get; set; }
      public decimal qty { get; set; }
  }

  class SinkronData
  {
      #region responToDiscore

      private string ResponToDisCorePB(ORMDataContext db, ScmsSoaLibrary.Parser.Class.ReceiveRelokasiStructure structure, string returId, DateTime date)
      {
          string rest = null;

          ScmsSoaLibrary.Parser.Class.ReceiveRelokasiResponse strt = new ScmsSoaLibrary.Parser.Class.ReceiveRelokasiResponse();

          strt.ID = structure.Fields.C_PBNO;
          strt.Fields = structure.Fields.Field;

          if (!string.IsNullOrEmpty(returId))
          {
              strt.RC = returId;
              strt.USER = "SCMS";
              strt.TanggalRC = date;
              strt.TanggalRC_Str = date.ToString("yyyy-MM-dd");
          }

          Config cfg = Functionals.Configuration;

          string val = ScmsSoaLibrary.Parser.Class.ReceiveRelokasiResponse.Serialize(strt);

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
          dParam = ParserDisCore.ParameterParserDec(sro.v_param.Replace("'", "\\\""));

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

         return rest;
      }

      #endregion

    public string OrderCustomerReceived(ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false;
      string result = null,
        tmp = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructureField field = null;
      string nipEntry = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      List<LG_DOH> listSDOrderD = null;

      LG_DOH doh = null;
      LG_DOD1 dod1 = null;

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

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Submit", StringComparison.OrdinalIgnoreCase))
        {
          #region Submit

          #region Verify

          if (string.IsNullOrEmpty(structure.Fields.NoReceive))
          {
            result = "Nomor Receive tidak boleh kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Cabang))
          {
            result = "Cabang tidak boleh kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (structure.Fields.TanggalDODate.Equals(DateTime.MinValue))
          {
            result = "Tanggal DO tidak boleh kosong / invalid.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (structure.Fields.TanggalInvoiceDate.Equals(DateTime.MinValue))
          {
            result = "Tanggal Invoice tidak boleh kosong / invalid.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (structure.Fields.TanggalRNDate.Equals(DateTime.MinValue))
          {
            result = "Tanggal penerima dicabang tidak boleh kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          #endregion

          #region Old Code

          //orderh = (from q in db.SD_OrderHs
          //          where (q.c_cab == structure.Fields.Cabang)
          //            && (q.c_norcv == structure.Fields.NoReceive)
          //          select q).Take(1).SingleOrDefault();

          //if (orderh == null)
          //{
          //  orderh = new SD_OrderH()
          //  {
          //    c_cab = structure.Fields.Cabang,
          //    c_norcv = structure.Fields.NoReceive,
          //    d_order = structure.Fields.TanggalRNDate,
          //    c_nosup = structure.Fields.Supplier,
          //    c_exnoinv = "",
          //    d_exinv = structure.Fields.TanggalInvoiceDate,
          //    c_notax = "TAX",
          //    c_nodo = "",
          //    d_tgldo = structure.Fields.TanggalDODate,
          //    c_pbdono = "",
          //    n_top = structure.Fields.Top,
          //    d_jth = structure.Fields.TanggalInvoiceDate.AddDays((double)structure.Fields.Top),
          //    n_extdisc = 0,
          //    n_jum = 0,
          //    n_psened = 0,
          //    c_ket = structure.Fields.Keterangan
          //  };

          //  db.SD_OrderHs.InsertOnSubmit(orderh);
          //}

          //tmp = (string.IsNullOrEmpty(structure.Fields.NomorInvoice) ? string.Empty :
          //  (structure.Fields.NomorInvoice.Length > 18 ? structure.Fields.NomorInvoice.Substring(0, 18).Trim() : structure.Fields.NomorInvoice.Trim()));
          //orderh.c_exnoinv = tmp;
          //orderh.d_exinv = structure.Fields.TanggalInvoiceDate;
          
          //tmp = (string.IsNullOrEmpty(structure.Fields.NomorDO) ? string.Empty :
          //  (structure.Fields.NomorDO.Length > 15 ? structure.Fields.NomorDO.Substring(0, 18).Trim() : structure.Fields.NomorDO.Trim()));
          //orderh.c_nodo = tmp;
          //orderh.d_tgldo = structure.Fields.TanggalDODate;

          //orderh.n_top = structure.Fields.Top;
          //orderh.d_jth = structure.Fields.TanggalInvoiceDate.AddDays((double)structure.Fields.Top);
          
          //orderh.c_ket = structure.Fields.Keterangan;

          //if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          //{
          //  listSDOrderD = (from q in db.SD_OrderDs
          //                  where (q.c_cab == structure.Fields.Cabang)
          //                    && (q.c_norcv == structure.Fields.NoReceive)
          //                  select q).Distinct().ToList();
            
          //  for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
          //  {
          //    field = structure.Fields.Field[nLoop];

          //    orderd = listSDOrderD.Find(delegate(SD_OrderD ored)
          //    {
          //      return field.Item.Equals((string.IsNullOrEmpty(ored.c_iteno) ? string.Empty : ored.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
          //        field.NomorSP.Equals((string.IsNullOrEmpty(ored.c_exnosp) ? string.Empty : ored.c_exnosp.Trim()), StringComparison.OrdinalIgnoreCase);
          //    });

          //    if (orderd != null)
          //    {
          //      if (field.IsDelete)
          //      {
          //        db.SD_OrderDs.DeleteOnSubmit(orderd);

          //        listSDOrderD.Remove(orderd);
          //      }
          //      else
          //      {
          //        orderd.n_qtyrcv = field.Quantity;
          //        orderd.n_bonus = field.Bonus;
          //        orderd.n_disc = field.Discount;
          //        orderd.n_salpri = field.Salpri;
          //        orderd.c_kddivams = field.DivisiAMS;
          //        orderd.c_kddivpri = field.DivisiSupplier;
          //      }
          //    }
          //    else
          //    {
          //      orderd = new SD_OrderD()
          //      {
          //        c_cab = structure.Fields.Cabang,
          //        c_norcv = structure.Fields.NoReceive,
          //        c_iteno = field.Item,
          //        n_qtyrcv = field.Quantity,
          //        n_bonus = field.Bonus,
          //        n_disc = field.Discount,
          //        n_salpri = field.Salpri,
          //        c_exnosp = field.NomorSP,
          //        n_qtoutdev = 0,
          //        c_kddivams = field.DivisiAMS,
          //        c_kddivpri = field.DivisiSupplier,
          //        n_qbeli = 0,
          //        n_qbonus = 0,
          //        n_qcdisc = 0,
          //        n_qcbonus = 0,
          //        n_docost = 0,
          //        n_ppnbm = 0
          //      };

          //      db.SD_OrderDs.InsertOnSubmit(orderd);

          //      listSDOrderD.Add(orderd);
          //    }

          //    totalDetails++;
          //  }
          //}

          //listSDOrderD.Clear();

          #endregion

          doh = new LG_DOH();

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {

              doh = (from q in db.LG_DOHs
                     where q.c_dono == structure.Fields.NomorDO
                     && q.c_cusno == structure.Fields.Cabang
                     select q).SingleOrDefault();

              totalDetails++;
          }

         


          dic = new Dictionary<string, string>();

          if (totalDetails > 0)
          {
            dic.Add("Tanggal", date.ToString("yyyyMMdd"));
            dic.Add("NO_DO", doh.c_dono);

            result = string.Format("Total {0} detail(s)", totalDetails);

            hasAnyChanges = true;
          }
          else
          {
            result = "Tidak ada data yang disimpan.";
          }

          #endregion
        }
        else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete

          //#region Verify

          //if (!string.IsNullOrEmpty(structure.Fields.NoReceive))
          //{
          //  result = "Nomor Receive tidak boleh kosong.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}
          //else if (string.IsNullOrEmpty(structure.Fields.Cabang))
          //{
          //  result = "Cabang tidak boleh kosong.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}

          //#endregion

          //orderh = (from q in db.SD_OrderHs
          //          where (q.c_cab == structure.Fields.Cabang)
          //            && (q.c_norcv == structure.Fields.NoReceive)
          //          select q).Take(1).SingleOrDefault();

          //if (orderh != null)
          //{
          //  db.SD_OrderHs.DeleteOnSubmit(orderh);

          //  totalDetails++;
            
          //  listSDOrderD = (from q in db.SD_OrderDs
          //                  where (q.c_cab == structure.Fields.Cabang)
          //                    && (q.c_norcv == structure.Fields.NoReceive)
          //                  select q).Distinct().ToList();

          //  if (listSDOrderD.Count > 0)
          //  {
          //    totalDetails += listSDOrderD.Count;
              
          //    db.SD_OrderDs.DeleteAllOnSubmit(listSDOrderD.ToArray());

          //    listSDOrderD.Clear();
          //  }
          //}

          //if (totalDetails > 0)
          //{
          //  hasAnyChanges = true;
          //}
          //else
          //{
          //  result = "Tidak ada data yang dihapus.";
          //}

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

        result = string.Format("ScmsSoaLibrary.Bussiness.SinkronData:OrderCustomerReceived - {0}", ex.Message);

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

    public string ReturCustomerReceived(ScmsSoaLibrary.Parser.Class.ReturCustomerReceiveStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false;
      string result = null,
        tmp = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      ScmsSoaLibrary.Parser.Class.ReturCustomerReceiveStructureField field = null;
      string nipEntry = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      List<SD_RetsupD> listSDRetsupD = null;

      SD_InventH inventh = null;
      SD_RetsupD retsupd = null;

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

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Submit", StringComparison.OrdinalIgnoreCase))
        {
          #region Submit

          #region Verify

          if (!string.IsNullOrEmpty(structure.Fields.NomorRetur))
          {
            result = "Nomor retur cabang tidak boleh kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Cabang))
          {
            result = "Cabang tidak boleh kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (structure.Fields.TanggalReturDate.Equals(DateTime.MinValue))
          {
            result = "Tanggal DO tidak boleh  kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (structure.Fields.TanggalRCDate.Equals(DateTime.MinValue))
          {
            result = "Tanggal Invoice tidak boleh kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (structure.Fields.TanggalExFakturDate.Equals(DateTime.MinValue))
          {
            result = "Tanggal penerima dicabang tidak boleh kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          #endregion

          inventh = (from q in db.SD_InventHs
                     where (q.c_cab == structure.Fields.Cabang)
                       && (q.c_invno == structure.Fields.NomorRetur)
                     select q).Take(1).SingleOrDefault();

          if (inventh == null)
          {
            inventh = new SD_InventH()
            {
              c_cab = structure.Fields.Cabang,
              c_invno = structure.Fields.NomorRetur,
              d_invdate = structure.Fields.TanggalReturDate,
              c_rcno = structure.Fields.Supplier,
              d_rcdate = structure.Fields.TanggalRCDate,
              c_exfak = structure.Fields.ExFaktur,
              n_retva = structure.Fields.ReturValue,
              c_exnotax = structure.Fields.ExNoFaktur,
              d_extax = structure.Fields.TanggalExFakturDate,
              c_nosup = structure.Fields.Supplier,
              c_taxco = structure.Fields.ExFakturCo,
              c_exfak2 = structure.Fields.ExFaktur2,
              c_exnotax2 = structure.Fields.ExNoFaktur2,
              d_extax2 = structure.Fields.TanggalExFaktur2Date,
              n_ppnbm = structure.Fields.PpnBM
            };

            db.SD_InventHs.InsertOnSubmit(inventh);
          }

          tmp = (string.IsNullOrEmpty(structure.Fields.NomorRetur) ? string.Empty :
            (structure.Fields.NomorRetur.Length > 16 ? structure.Fields.NomorRetur.Substring(0, 16).Trim() : structure.Fields.NomorRetur.Trim()));
          inventh.c_invno = tmp;

          tmp = (string.IsNullOrEmpty(structure.Fields.NomorRC) ? string.Empty :
            (structure.Fields.NomorRC.Length > 10 ? structure.Fields.NomorRC.Substring(0, 10).Trim() : structure.Fields.NomorRC.Trim()));
          inventh.c_rcno = tmp;

          tmp = (string.IsNullOrEmpty(structure.Fields.ExFaktur) ? string.Empty :
            (structure.Fields.ExFaktur.Length > 10 ? structure.Fields.ExFaktur.Substring(0, 10).Trim() : structure.Fields.ExFaktur.Trim()));
          inventh.c_exfak = tmp;

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listSDRetsupD = (from q in db.SD_RetsupDs
                             where (q.c_cab == structure.Fields.Cabang)
                              && (q.c_invno == structure.Fields.NomorRetur)
                             select q).Distinct().ToList();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              retsupd = listSDRetsupD.Find(delegate(SD_RetsupD retd)
              {
                return field.Item.Equals((string.IsNullOrEmpty(retd.c_iteno) ? string.Empty : retd.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase);
              });

              if (retsupd != null)
              {
                if (field.IsDelete)
                {
                  db.SD_RetsupDs.DeleteOnSubmit(retsupd);

                  listSDRetsupD.Remove(retsupd);
                }
                else
                  retsupd.n_qtygood = field.QuantityGood;
                  retsupd.n_qtybad = field.QuantityBad;
                  retsupd.n_qtybong = field.BonusQuantityGood;
                  retsupd.n_qtybonb = field.BonusQuantityBad;
                  retsupd.n_disc = field.Discount;
                  retsupd.n_salpri = field.Salpri;
                  retsupd.c_kddivams = field.DivisiAMS;
                  retsupd.c_kddivpri = field.DivisiSupplier;
                }
              else 
              {
                retsupd = new SD_RetsupD()
                {
                  c_cab = structure.Fields.Cabang,
                  c_invno = structure.Fields.NomorRetur,
                  c_iteno = field.Item,
                  n_qtygood = field.QuantityGood,
                  n_qtybad = field.QuantityBad,
                  n_qtybong = field.BonusQuantityGood,
                  n_qtybonb = field.BonusQuantityBad,
                  n_disc = field.Discount,
                  n_salpri = field.Salpri,
                  c_kddivams = field.DivisiAMS,
                  c_kddivpri = field.DivisiSupplier
                };

                db.SD_RetsupDs.InsertOnSubmit(retsupd);

                listSDRetsupD.Add(retsupd);
              }

              totalDetails++;
            }
          }

          listSDRetsupD.Clear();

          dic = new Dictionary<string, string>();

          if (totalDetails > 0)
          {
            dic.Add("Tanggal", date.ToString("yyyyMMdd"));

            result = string.Format("Total {0} detail(s)", totalDetails);

            hasAnyChanges = true;
          }
          else
          {
            result = "Tidak ada data yang disimpan.";
          }

          #endregion
        }
        else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete

          #region Verify

          if (!string.IsNullOrEmpty(structure.Fields.NomorRetur))
          {
            result = "Nomor retur cabang tidak boleh kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Cabang))
          {
            result = "Cabang tidak boleh kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          #endregion

          inventh = (from q in db.SD_InventHs
                     where (q.c_cab == structure.Fields.Cabang)
                       && (q.c_invno == structure.Fields.NomorRetur)
                     select q).Take(1).SingleOrDefault();

          if (inventh != null)
          {
            db.SD_InventHs.DeleteOnSubmit(inventh);

            totalDetails++;

            listSDRetsupD = (from q in db.SD_RetsupDs
                             where (q.c_cab == structure.Fields.Cabang)
                              && (q.c_invno == structure.Fields.NomorRetur)
                             select q).Distinct().ToList();

            if (listSDRetsupD.Count > 0)
            {
              totalDetails += listSDRetsupD.Count;

              db.SD_RetsupDs.DeleteAllOnSubmit(listSDRetsupD.ToArray());

              listSDRetsupD.Clear();
            }
          }

          if (totalDetails > 0)
          {
            hasAnyChanges = true;
          }
          else
          {
            result = "Tidak ada data yang dihapus.";
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

        result = string.Format("ScmsSoaLibrary.Bussiness.SinkronData:OrderCustomerReceived - {0}", ex.Message);

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

    public string MasterItemReceived(ScmsSoaLibrary.Parser.Class.MasterItemReceiveStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false;
      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      ScmsSoaLibrary.Parser.Class.MasterItemReceiveStructureFields field = null;
      string nipEntry = null;
      //string memoID = null;
      //string tmpNumbering = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      FA_MasItm itm = null;

      IDictionary<string, string> dic = null;

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

        else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase)
          || structure.Method.Equals("Edit", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify

          field = structure.Fields;

          itm = (from q in db.FA_MasItms
                 where q.c_iteno == structure.Fields.ItemID
                 select q).Take(1).SingleOrDefault();

          //itm.c_entry = field.Entry;
          itm.c_nosup = field.C_NOSUP;
          itm.c_update = field.Entry;
          //itm.d_entry = date;
          itm.d_update = date;
          itm.c_itenopri = field.C_ITENOPRI.Trim();

          //Logger.WriteLine(field.C_ITENOPRI.Trim(), true);

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

    public string SuratPesananReceived(ScmsSoaLibrary.Parser.Class.SuratPesananReceiveStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      bool hasAnyChanges = false;
      string result = null;

      bool isContexted = false;
      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      if (db == null)
      {
        db = new ORMDataContext(Functionals.ActiveConnectionString);
      }
      string SPNO = null;
      int SPtriton = 0;
      string bodyemail = null;
      LG_SPH sph = null;
      LG_Cusma cusmas = null;

      ScmsSoaLibrary.Parser.Class.SuratPesananReceiveStructure spResp = null;
      //List<ScmsSoaLibrary.Parser.Class.SuratPesananJSONStructureField> listSPJsonField = null;

      ScmsSoaLibrary.Parser.Class.SuratPesananReceiveStructureField field = null;
      string nipEntry = null;
      string spID = null;
      //string tmpNumbering = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now,
        dateSp = DateTime.MinValue;

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

      spID = (structure.Fields.SpSCMS ?? string.Empty);


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

            rpe = ResponseParser.ResponseParserEnum.IsError;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.SPCabang))
          {
            result = "Nomor Surat Pesanan Cabang harus terisi.";

            rpe = ResponseParser.ResponseParserEnum.IsError;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (structure.Fields.TanggalSP.Equals(DateTime.MinValue))
          {
              result = "Format tanggal tidak dapat terbaca. " + structure.Fields.TanggalSP;

              rpe = ResponseParser.ResponseParserEnum.IsError;

              if (db.Transaction != null)
              {
                  db.Transaction.Rollback();
              }

              goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Customer))
          {
            result = "Nama cabang dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsError;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          cusmas = (from q in db.LG_Cusmas
                        where q.c_cusno == structure.Fields.Customer
                        select q).SingleOrDefault();
          double n_lead_ekspedisi = Convert.ToDouble(cusmas.n_days_ekspedisi) * -1;
          nLoop = (from q in db.LG_SPHs
                   where (q.c_sp == structure.Fields.SPCabang)
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

          spID = Commons.GenerateNumbering<LG_SPH>(db, "SP", '3', "07", date, "c_spno");

          string sTipeSP = null;

          switch (structure.Fields.SPCabang.Substring(0, 3))
          {
            case "SPM":
              sTipeSP = "02";
              break;
            case "SPO":
              sTipeSP = "01";
              break;
            case "SPD":
              sTipeSP = "05";
              break;
            case "SPI": //Indra 20180607FM Penambahan Tipe SP
              sTipeSP = "02";
              break;
            case "SPK":
              sTipeSP = "02";
              break;
          }
            DateTime DateETA = structure.Fields.D_ETA;
            DateTime spdate = structure.Fields.TanggalSP;

          sph = new LG_SPH()
          {
            c_cusno = structure.Fields.Customer,
            c_entry = nipEntry,
            c_sp = structure.Fields.SPCabang,
            c_spno = spID,
            c_type = sTipeSP,
            c_update = nipEntry,
            d_entry = date,
            d_spdate = structure.Fields.TanggalSP,
            d_spinsert = date,
            d_update = date,
            l_cek = false,
            l_print = false,
            v_ket = string.Empty,
            d_etasp = DateETA, //penambahan by suwandi 27 agustus 2018
            d_etdsp = DateETA.AddDays(n_lead_ekspedisi)
          };

          db.LG_SPHs.InsertOnSubmit(sph);
          
          #region Insert Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listSpd1 = new List<LG_SPD1>();
            listSpd2 = new List<LG_SPD2>();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              if ((field != null) && field.IsNew && (!field.IsDelete) && (field.Acc > 0))
              {
                nPrice = (from q in db.FA_MasItms
                          where q.c_iteno == field.Item && q.l_aktif == true && q.l_hide == false
                          select (q.n_salpri.HasValue ? q.n_salpri.Value : 0)).Take(1).SingleOrDefault();
                var SPSUPTRITON = (from q in db.FA_MasItms join q2 in db.FA_Divpris on q.c_iteno equals q2.c_iteno where q.c_iteno == field.Item && q.l_aktif == true && q.l_hide == false && (from q1 in db.scms_params where q1.v_var == "PL_AUTO" select q1.v_values).Contains(q2.c_kddivpri) select q).SingleOrDefault();

                if (SPSUPTRITON != null)
                {
                    var principal = (from q in db.LG_DatSups where q.c_nosup == SPSUPTRITON.c_nosup select q).SingleOrDefault();
                    var divpri = (from q in db.FA_MsDivPris 
                                      join q1 in db.FA_Divpris on q.c_kddivpri equals q1.c_kddivpri 
                                      where q1.c_iteno == field.Item
                                      select q).SingleOrDefault();
                    SPtriton = SPtriton + 1;
                    if (bodyemail == null)
                    {
                        bodyemail = "<tr><td>" + field.Item + "</td><td>" + SPSUPTRITON.v_itnam + "</td><td>" + principal.v_nama + "</td><td>" + divpri.v_nmdivpri + "</td><td>" + field.Qty + "</td><td>" + DateETA.AddDays(n_lead_ekspedisi).ToShortDateString() + "</td><td>" + DateETA.ToShortDateString() + "</td></tr>";
                    }
                    else
                    {
                        bodyemail = bodyemail + "<tr><td>" + field.Item + "</td><td>" + SPSUPTRITON.v_itnam + "</td><td>" + principal.v_nama + "</td><td>" + divpri.v_nmdivpri + "</td><td>" + field.Qty + "</td><td>" + DateETA.AddDays(n_lead_ekspedisi).ToShortDateString() + "</td><td>" + DateETA.ToShortDateString() + "</td></tr>";
                    }
                }
                listSpd1.Add(new LG_SPD1()
                {
                  c_iteno = field.Item,
                  c_spno = spID,
                  c_type = "01",
                  n_acc = (field.Acc == null ? 0 : field.Acc),
                  n_qty = (field.Qty == null ? 0 : field.Qty),
                  n_salpri = nPrice,
                  n_sisa = (field.Qty == null ? 0 : field.Qty),
                  n_spds = 0,
                  v_ket = string.Empty
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

              //db.LG_SPD2s.InsertAllOnSubmit(listSpd2.ToArray());
              listSpd2.Clear();
            }

          }

          #endregion

          //#region Test suwandi 24 oktober 2018
          //rpe = ResponseParser.ResponseParserEnum.IsError;

          //result = string.Format("Test Error");

          //goto endLogic;
          //#endregion

          dic = new Dictionary<string, string>();

          if (totalDetails > 0)
          {
            dic.Add("SP", spID);
            dic.Add("Tanggal", date.ToString("yyyyMMdd"));

            result = string.Format("Success:True");

            hasAnyChanges = true;
          }

          #endregion
        }
        else if (structure.Method.Equals("ModifyETA", StringComparison.OrdinalIgnoreCase))
        {
            #region Modify
            if (string.IsNullOrEmpty(structure.Fields.SPCabang))
            {
                result = "Nomor Surat Pesanan Cabang harus terisi.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                if (db.Transaction != null)
                {
                    db.Transaction.Rollback();
                }

                goto endLogic;
            }

            nLoop = (from q in db.LG_SPHs
                     where (q.c_sp == structure.Fields.SPCabang)
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                     select q).Count();
            cusmas = (from q in db.LG_Cusmas
                      join q1 in db.LG_CusmasCabs on q.c_cusno equals q1.c_cusno
                      where q1.c_cab_dcore == structure.Fields.SPCabang.Substring(3,3)
                      select q).SingleOrDefault();
            double n_lead_ekspedisi = Convert.ToDouble(cusmas.n_days_ekspedisi) * -1;
            DateTime DateETA = structure.Fields.D_ETA;
            if (nLoop == 0)
            {
                result = "Nomor surat pesanan belum ada.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                if (db.Transaction != null)
                {
                    db.Transaction.Rollback();
                }

                goto endLogic;
            }

            nLoop = (from q in db.LG_SPHs
                         join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
                         where q.c_sp == structure.Fields.SPCabang && q1.n_acc != q1.n_sisa
                         select q).Count();
            if (nLoop > 0)
            {
                result = "SP ini sudah di layani.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                if (db.Transaction != null)
                {
                    db.Transaction.Rollback();
                }

                goto endLogic;
            }

            sph = (from q in db.LG_SPHs
                       where q.c_sp == structure.Fields.SPCabang
                       select q).SingleOrDefault();
            sph.d_etasp = structure.Fields.D_ETA;
            sph.d_etdsp = DateETA.AddDays(n_lead_ekspedisi);
            hasAnyChanges = true;
            result = string.Format("Success:True");
            #endregion
        }
        else if (structure.Method.Equals("CancelSP", StringComparison.OrdinalIgnoreCase))
        {
            #region Cancel SP
            sph = (from q in db.LG_SPHs
                       where q.c_sp == structure.Fields.SPCabang
                       select q).SingleOrDefault();
            listSpd1 = (from q in db.LG_SPD1s 
                    where q.c_spno == sph.c_spno && q.n_acc != q.n_sisa
                        select q).ToList();
            if (sph.l_delete == false || sph.l_delete == null)
            {
                if (listSpd1.Count() > 0)
                {
                    result = "Tidak bisa di cancel karena sudah terbuat PL atau di buat relokasi.";
                    rpe = ResponseParser.ResponseParserEnum.IsError;
                    db.Transaction.Rollback();
                    goto endLogic;
                }
                else if (listSpd1.Count() == 0)
                {
                    listSpd1 = (from q in db.LG_SPD1s
                                where q.c_spno == sph.c_spno
                                select q).ToList();
                    sph.l_delete = true;
                    sph.v_ket = "Batal dari cabang.";
                    for (nLoop = 0; nLoop < listSpd1.Count(); nLoop++)
                    {
                        listSpd1[nLoop].n_sisa = 0;
                        listSpd1[nLoop].n_acc = 0;
                    }
                    hasAnyChanges = true;
                }
            }
            else if (sph.l_delete == true)
            {
                result = "Sudah pernah di cancel, tidak bisa di cancel lagi.";
                rpe = ResponseParser.ResponseParserEnum.IsError;
                hasAnyChanges = false;
            }
            #endregion
        }

        if (!isContexted)
        {
          if (hasAnyChanges)
          {
              if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
              {
                  nLoop = (from q in db.LG_SPHs
                           where (q.c_sp == structure.Fields.SPCabang)
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
              }
            if (spResp != null)
            {
              //spResp.Fields = 
              //if ((listSPJsonField != null) && (listSPJsonField.Count > 0))
              //{
              //  spResp.Fields = listSPJsonField.ToArray();

              //  listSPJsonField.Clear();
              //}

              //PostDataReplySP(db.Connection.ConnectionString, spResp); 
            }
            if (SPtriton != 0)
            {
                string Sender = "scms.dophar@ams.co.id";
                string TextSender = "Supply Chain Management System";
                string Received = "";
                if (cusmas.c_gdg == '1')
                {
                    Received = "ditto@ams.co.id;evi.fitriani@ams.co.id;admin.ppic-import@ams.co.id;Order.process-ppicmkt@ams.co.id";
                }
                else if (cusmas.c_gdg == '2')
                {
                    Received = "pic1@log2.ams.co.id;pic2@log2.ams.co.id";
                }
                string CarbonCopy = "suwandi@ams.co.id;fery@ams.co.id";
                string Subject = "";
                string EmailHeader = "Dear Bapak/Ibu,";
                string EmailContent = "";
                string EmailFooter = "<br/>Kepada personil terkait, harap pesanan dapat diproses di hari yang sama sejak pesanan diterima.<br/><br/>Terima kasih";
                string EmailTable = "";
                Subject = structure.Fields.SPCabang;
                EmailContent = "Berikut adalah daftar pesanan cabang " + cusmas.v_cunam + " untuk produk – produk Townsend dan Thuasne:<br/><br/>";
                EmailTable = "<tr><td>Kode Produk</td><td>Nama Produk</td><td>Principal</td><td>Divisi Principal</td><td>Jumlah Pesanan</td><td>ETD SP</td><td>ETA SP</td></tr>";
                EmailTable = EmailTable + bodyemail;
                SPNO = structure.Fields.SPCabang;
                EmailSender.EmailParameter(db,Sender,TextSender,Received + ";",CarbonCopy + ";",Subject,EmailHeader,EmailContent,EmailTable,EmailFooter);
            }
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
              if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
              {
                  nLoop = (from q in db.LG_SPHs
                           where (q.c_sp == structure.Fields.SPCabang)
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
              }
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

        result = string.Format("ScmsSoaLibrary.Bussiness.Pembelian:SuratPesanan - {0}", ex.Message);

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

    public string RecallBarang(ScmsSoaLibrary.Parser.Class.RecallReceiveStructure structure)
    {
        if ((structure == null) || (structure.Fields == null))
        {
            return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
        }

        bool hasAnyChanges = false;
        string result = null;
        string batchd = null, iteno = null;
        char[] gdg = {'1','2','6'};
        bool isContexted = false;
        ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);
        int i = 0, a = 0,idet = 0, gud = 0;
        if (db == null)
        {
            db = new ORMDataContext(Functionals.ActiveConnectionString);
        }

        LG_RND1 RND1 = null;
        LG_AdjustH AdjustH = null;
        LG_AdjustD1 AdjustD1 = null;
        LG_AdjustD2 AdjustD2 = null;
        LG_RECALLH recallh = null;
        LG_RECALLD recalld = null;
        string AJNO = null;
        ScmsSoaLibrary.Parser.Class.RecallReceiveStructure recall = null;
        ScmsSoaLibrary.Parser.Class.RecallReceiveStructureField field = null;

        ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
        List<LG_RND1> listrnd1 = null;
        List<LG_RND1> listgdg = null;
        List<LG_AdjustD1> listadjustd1 = null;
        List<LG_AdjustD2> listadjustd2 = null;
        List<LG_RECALLD> listrecalld = null;
        listadjustd1 = new List<LG_AdjustD1>();
        listadjustd2 = new List<LG_AdjustD2>();
        listrecalld = new List<LG_RECALLD>();
        IDictionary<string, string> dic = null;

        try
        {
            if (!isContexted)
            {
                db.Connection.Open();
                db.Transaction = db.Connection.BeginTransaction();
            }
            #region update
            if (structure.Method.Equals("update", StringComparison.OrdinalIgnoreCase))
            {
                #region input transaksi dari dcore
                recallh = new LG_RECALLH()
                {
                    c_recalno = structure.Fields.RecallNo,
                    d_recall_start = Convert.ToDateTime(structure.Fields.Tanggal),
                    d_recall_from = Convert.ToDateTime(structure.Fields.recallfrom),
                    d_recall_to = Convert.ToDateTime(structure.Fields.recallto),
                    c_ket = structure.Fields.Memo,
                    c_submit = structure.Fields.entry,
                    d_submit = DateTime.Now
                };
                db.LG_RECALLHs.InsertOnSubmit(recallh);

                for (i = 0; i < structure.Fields.Field.Length; i++)
                {
                    field = structure.Fields.Field[i];
                    string[] batch = field.Batches.Split(',');
                    for (a = 0; a < batch.Count(); a++)
                    {
                        recalld = new LG_RECALLD
                        {
                            c_recallno = structure.Fields.RecallNo,
                            c_iteno = field.Iteno,
                            c_batch = batch[a].ToString()
                        };
                        listrecalld.Add(recalld);
                    }
                }
                #endregion

                #region input transaksi adjustment
                for (gud = 0; gud < gdg.Count(); gud++)
                {
                    iteno = null;
                    batchd = null;
                    
                    AJNO = Commons.GenerateNumbering<LG_AdjustH>(db, "AJ", '3', "15", DateTime.Now, "c_adjno");
                    AdjustH = new LG_AdjustH()
                    {
                        c_gdg = gdg[gud],
                        c_adjno = AJNO,
                        d_adjdate = DateTime.Now,
                        c_type = "02",
                        v_ket = structure.Fields.Memo,
                        c_entry = "DCORE",
                        d_entry = DateTime.Now,
                        c_update = "DCORE",
                        d_update = DateTime.Now,
                        l_delete = null,
                        v_ket_mark = null
                    };
                    db.LG_AdjustHs.InsertOnSubmit(AdjustH);

                    for (i = 0; i < structure.Fields.Field.Length; i++)
                    {
                        field = structure.Fields.Field[i];
                        if (field.Batches == "")
                        {
                            //var qry = (from q in db.LG_RND1s
                            //           where q.c_iteno == field.Iteno && q.n_gsisa > 0
                            //           select new { q.c_rnno,q.c_batch,q.n_gsisa}
                            //           ).ToList();
                            listrnd1 = (from q in db.LG_RND1s
                                        where q.c_iteno == field.Iteno && (q.n_gsisa > 0) && q.c_gdg == gdg[gud]
                                        orderby q.c_batch
                                        select q).ToList();
                            if (listrnd1.Count() > 0)
                            {
                                for (a = 0; a < listrnd1.Count(); a++)
                                {
                                    if ((iteno == null && batchd == null && batchd != listrnd1[a].c_batch) || (iteno != null && batchd != null && batchd != listrnd1[a].c_batch))
                                    {
                                        AdjustD1 = new LG_AdjustD1
                                        {
                                            c_gdg = listrnd1[a].c_gdg,
                                            c_adjno = AJNO,
                                            c_batch = listrnd1[a].c_batch,
                                            c_iteno = listrnd1[a].c_iteno,
                                            n_bqty = listrnd1[a].n_gsisa,
                                            n_gqty = listrnd1[a].n_gsisa * -1
                                        };
                                        AdjustD2 = new LG_AdjustD2
                                        {
                                            c_gdg = listrnd1[a].c_gdg,
                                            c_adjno = AJNO,
                                            c_batch = listrnd1[a].c_batch,
                                            c_iteno = listrnd1[a].c_iteno,
                                            c_rnno = listrnd1[a].c_rnno,
                                            n_bqty = listrnd1[a].n_gsisa,
                                            n_gqty = listrnd1[a].n_gsisa * -1
                                        };
                                        listadjustd1.Add(AdjustD1);
                                        listadjustd2.Add(AdjustD2);
                                        batchd = listrnd1[a].c_batch;
                                        iteno = listrnd1[a].c_iteno;
                                    }
                                    else
                                    {
                                        AdjustD1.n_gqty += listrnd1[a].n_gsisa * -1;
                                        AdjustD1.n_bqty += listrnd1[a].n_gsisa;
                                        AdjustD2.n_gqty += listrnd1[a].n_gsisa * -1;
                                        AdjustD2.n_bqty += listrnd1[a].n_gsisa;
                                        batchd = listrnd1[a].c_batch;
                                        iteno = listrnd1[a].c_iteno;
                                    }
                                    listrnd1[a].n_bsisa += listrnd1[a].n_gsisa;
                                    listrnd1[a].n_gsisa -= listrnd1[a].n_gsisa;
                                }
                            }
                        }
                        else if (field.Batches != "")
                        {
                            string[] batch = field.Batches.Split(',');

                            for (a = 0; a < batch.Count(); a++)
                            {
                                listrnd1 = (from q in db.LG_RND1s
                                            where q.c_iteno == field.Iteno && q.c_batch == batch[a] && (q.n_gsisa > 0) && q.c_gdg == gdg[gud]
                                            select q).ToList();
                                if (listrnd1.Count > 0)
                                {
                                    for (idet = 0; idet < listrnd1.Count(); idet++)
                                    {
                                        if ((iteno == null && batchd == null && batchd != listrnd1[idet].c_batch) || (iteno != null && batchd != null && batchd != listrnd1[idet].c_batch))
                                        {
                                            AdjustD1 = new LG_AdjustD1
                                            {
                                                c_gdg = listrnd1[idet].c_gdg,
                                                c_adjno = AJNO,
                                                c_batch = listrnd1[idet].c_batch,
                                                c_iteno = listrnd1[idet].c_iteno,
                                                n_bqty = listrnd1[idet].n_gsisa,
                                                n_gqty = listrnd1[idet].n_gsisa * -1
                                            };
                                            AdjustD2 = new LG_AdjustD2
                                            {
                                                c_gdg = listrnd1[idet].c_gdg,
                                                c_adjno = AJNO,
                                                c_batch = listrnd1[idet].c_batch,
                                                c_iteno = listrnd1[idet].c_iteno,
                                                c_rnno = listrnd1[idet].c_rnno,
                                                n_bqty = listrnd1[idet].n_gsisa,
                                                n_gqty = listrnd1[idet].n_gsisa * -1
                                            };
                                            listadjustd1.Add(AdjustD1);
                                            listadjustd2.Add(AdjustD2);
                                            batchd = listrnd1[idet].c_batch;
                                            iteno = listrnd1[idet].c_iteno;
                                        }
                                        else
                                        {
                                            AdjustD1.n_gqty += listrnd1[idet].n_gsisa * -1;
                                            AdjustD1.n_bqty += listrnd1[idet].n_gsisa;
                                            AdjustD2.n_gqty += listrnd1[idet].n_gsisa * -1;
                                            AdjustD2.n_bqty += listrnd1[idet].n_gsisa;
                                            batchd = listrnd1[idet].c_batch;
                                            iteno = listrnd1[idet].c_iteno;
                                        }
                                        listrnd1[idet].n_bsisa += listrnd1[idet].n_gsisa;
                                        listrnd1[idet].n_gsisa -= listrnd1[idet].n_gsisa;
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                if (listadjustd1 != null)
                {
                    db.LG_RECALLDs.InsertAllOnSubmit(listrecalld.ToArray());
                    db.LG_AdjustD1s.InsertAllOnSubmit(listadjustd1.ToArray());
                    db.LG_AdjustD2s.InsertAllOnSubmit(listadjustd2.ToArray());
                    listadjustd1.Clear();
                    listadjustd2.Clear();
                    listrecalld.Clear();
                }
                db.SubmitChanges();
                db.Transaction.Commit();
                rpe = ResponseParser.ResponseParserEnum.IsSuccess;
                result = "Data telah di commit";
            }
            #endregion
            else
            {
                rpe = ResponseParser.ResponseParserEnum.IsError;
                result = "Metode tidak terdaftar";
                goto EndLogic;
            }
        }
        catch(Exception ex)
        {
            db.Transaction.Rollback();
            rpe = ResponseParser.ResponseParserEnum.IsError;
            result = "Data Gagal di commit " + ex.Message;
            goto EndLogic;
        }
        EndLogic:
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

    public string Relokasi(ScmsSoaLibrary.Parser.Class.RelokasiStructure structure)
    {
        if ((structure == null) || (structure.Fields == null))
        {
            return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
        }

        bool hasAnyChanges = false;
        string result = null;

        bool isContexted = false;
        ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);
        int i = 0, a = 0, idet = 0;
        if (db == null)
        {
            db = new ORMDataContext(Functionals.ActiveConnectionString);
        }
        LG_SPH sph = null;
        LG_SPD1 spd1 = null;

        ScmsSoaLibrary.Parser.Class.RelokasiStructure recall = null;
        ScmsSoaLibrary.Parser.Class.RelokasiStructureField field = null;

        ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
        IDictionary<string, string> dic = null;

        try
        {
            if (!isContexted)
            {
                db.Connection.Open();
                db.Transaction = db.Connection.BeginTransaction();
            }
            if (structure.Method.Equals("add", StringComparison.OrdinalIgnoreCase))
            {
                for (i = 0; i < structure.Fields.Field.Length; i++)
                {
                    if (structure.Fields.Field[i].C_SPNO != string.Empty)
                    {
                        sph = (from q in db.LG_SPHs
                               where q.c_sp == structure.Fields.Field[i].C_SPNO
                               select q).SingleOrDefault();
                        spd1 = (from q in db.LG_SPD1s
                                where q.c_spno == sph.c_spno && q.c_iteno == structure.Fields.C_ITENO
                                select q).SingleOrDefault();

                        if (spd1.n_sisa < Convert.ToInt32(structure.Fields.Field[i].N_QTYREL))
                        {
                            result = "Sisa " + sph.c_spno + " lebih sedikit dari pada yang mau di alokasi.";
                            rpe = ResponseParser.ResponseParserEnum.IsError;
                            db.Transaction.Rollback();
                            goto EndLogic;
                        }
                        else
                        {
                            spd1.n_sisa = spd1.n_sisa - Convert.ToInt32(structure.Fields.Field[i].N_QTYREL);
                            spd1.n_relokasi = Convert.ToInt32(spd1.n_relokasi) + Convert.ToInt32(structure.Fields.Field[i].N_QTYREL);
                        }
                    }
                }
            }
            else
            {
                result = "Metode yang anda masukkan tidak terdaftar.";
                rpe = ResponseParser.ResponseParserEnum.IsError;
                db.Transaction.Rollback();
                goto EndLogic;
            }
            db.SubmitChanges();
            db.Transaction.Commit();
            rpe = ResponseParser.ResponseParserEnum.IsSuccess;
            result = "Data telah di commit";
        }
        catch (Exception ex)
        {
            db.Transaction.Rollback();
            rpe = ResponseParser.ResponseParserEnum.IsError;
            result = "Data Gagal di commit " + ex.Message;
            goto EndLogic;
        }
    EndLogic:
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

    public string ReceiveRelokasi(ScmsSoaLibrary.Parser.Class.ReceiveRelokasiStructure structure)
    {

        if ((structure == null) || (structure.Fields == null))
        {
            return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
        }

        bool hasAnyChanges = false;
        string result = null, rcno = null, RSID = null, plno = null, dono = null, supplier = null, c_iteno = null, c_batch = null, donocab = null, itenocab = null, batchcab = null, spno = null, sprc = null;
        decimal ngross = 0, ndisc = 0, ntax = 0, nnet = 0, nsisa = 0;
        DateTime date = DateTime.Now;

        bool isContexted = false;
        ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);
        int i = 0, a = 0, idet = 0;
        decimal rel = 0;
        if (db == null)
        {
            db = new ORMDataContext(Functionals.ActiveConnectionString);
        }
        #region declare table
        LG_RCH rch = null;
        LG_RCD1 rcd1 = null;
        LG_RCD2 rcd2 = null;
        LG_RCD3 rcd3 = null;
        FA_DiscD discd = null;
        LG_PLH plh = null;
        LG_PLD1 pld1 = null;
        LG_PLD2 pld2 = null;
        LG_DOH doh = null;
        LG_DOD1 dod1 = null;
        LG_DOD2 dod2 = null;
        LG_FJH fjh = null;
        LG_FJD1 fjd1 = null;
        LG_FJD2 fjd2 = null;
        LG_FJD3 fjd3 = null;
        LG_SPD1 spd1 = null;
        List<LG_RCD1> listrcd1 = null;
        List<LG_RCD2> listrcd2 = null;
        List<LG_RCD3> listrcd3 = null;
        List<LG_PLD1> listpld1 = null;
        List<LG_PLD2> listpld2 = null;
        List<LG_DOD1> listdod1 = null;
        List<LG_DOD2> listdod2 = null;
        List<LG_FJD1> listfjd1 = null;
        List<LG_FJD2> listfjd2 = null;
        listrcd1 = new List<LG_RCD1>();
        listrcd2 = new List<LG_RCD2>();
        listrcd3 = new List<LG_RCD3>();
        listpld1 = new List<LG_PLD1>();
        listpld2 = new List<LG_PLD2>();
        listdod1 = new List<LG_DOD1>();
        listdod2 = new List<LG_DOD2>();
        listfjd1 = new List<LG_FJD1>();
        listfjd2 = new List<LG_FJD2>();
        LG_Cusma cusmas = null;
        LG_CusmasCab cusmascab = null;
        LG_CusmasCab cusmasasal = null;
        #endregion

        string RN = null;
        ScmsSoaLibrary.Parser.Class.ReceiveRelokasiStructure receivereloc = null;
        ScmsSoaLibrary.Parser.Class.ReceiveRelokasiStructureField field = null;
        
        ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
        IDictionary<string, string> dic = null;
        ReceiveRelocation recrelo = null;
        Dictionary<string, List<ReceiveRelocation>> recreloc = null;
        recreloc = new Dictionary<string, List<ReceiveRelocation>>(StringComparer.OrdinalIgnoreCase);
        try
        {
            if (!isContexted)
            {
                db.Connection.Open();
                db.Transaction = db.Connection.BeginTransaction();
            }
            if (structure.Method.Equals("add", StringComparison.OrdinalIgnoreCase))
            {
                var recrel = (from q in db.LG_RCHes where q.v_pbbrno == structure.Fields.C_PBNO select q).SingleOrDefault();
                cusmasasal = (from q in db.LG_CusmasCabs where q.c_cab_dcore == structure.Fields.CabangAsal select q).SingleOrDefault();
                cusmascab = (from q in db.LG_CusmasCabs where q.c_cab_dcore == structure.Fields.CabangTerima select q).SingleOrDefault();
                cusmas = (from q in db.LG_Cusmas where q.c_cusno == cusmascab.c_cusno select q).SingleOrDefault();
                if (recrel == null)
                {
                    #region RC
                    rcno = Commons.GenerateNumbering<LG_RCH>(db, "RC", '3', "11", date, "c_rcno");
                    rch = new LG_RCH
                    {
                        c_gdg = cusmas.c_gdg.Value,
                        c_rcno = rcno,
                        d_rcdate = DateTime.Now,
                        c_cusno = cusmasasal.c_cusno,
                        v_ket = structure.Fields.Keterangan,
                        c_pin = "0",
                        l_status = false,
                        l_send = false,
                        c_entry = "SYSTEM",
                        d_entry = DateTime.Now,
                        c_update = "SYSTEM",
                        d_update = DateTime.Now,
                        c_pbbrno = "01",
                        d_send = DateTime.Now,
                        l_delete = false,
                        v_ket_mark = null,
                        v_oldpbbrno = null,
                        v_pbbrno = structure.Fields.C_PBNO,
                        l_sent = false,
                        c_baspbno = null,
                        c_type = "L"
                    };
                    db.LG_RCHes.InsertOnSubmit(rch);
                    for (i = 0; i < structure.Fields.Field.Length; i++)
                    {
                        field = structure.Fields.Field[i];
                        field.Batch = field.BATCH;
                        field.Item = field.ITENO;
                        field.QuantityReceive = field.N_QTY_PBB;
                        field.Quantity = field.N_QTY_RCV;
                        field.Acceptance = field.N_QTY_RCV;
                        field.Destroy = "0";
                        field.NoDO = field.C_DONO;

                        if (field.C_DONO == string.Empty)
                        {
                            result = "Nomor DO nya kosong. Harap di isi nomor DO nya.";
                            rpe = ResponseParser.ResponseParserEnum.IsError;
                            db.Transaction.Rollback();
                            goto EndLogic;
                        }
                        var rn = (from q in db.LG_DOHs
                                  join q1 in db.LG_PLD2s on q.c_plno equals q1.c_plno
                                  where q.c_dono == field.C_DONO && q1.c_iteno == field.ITENO && q1.c_batch == field.BATCH
                                  select q1).Take(1).SingleOrDefault();
                        var iteno = (from q in db.FA_MasItms
                                     where q.c_iteno == field.ITENO
                                     select q).SingleOrDefault();
                        var sp = (from q in db.LG_SPHs where q.c_sp == field.C_SPNO select q).SingleOrDefault();
                        var disc = (from q in db.FA_DiscDs where q.c_iteno == field.ITENO && q.c_nodisc == "DSXXXXXX03" select q).SingleOrDefault();
                        field.n_disc = disc.n_discon.ToString();
                        field.n_salpri = iteno.n_salpri.ToString();

                        if (supplier == null)
                        {
                            supplier = iteno.c_nosup;
                        }
                        else
                        {
                            if (supplier.ToString() != iteno.c_nosup.ToString())
                            {
                                result = "Supplier nya tidak sama";
                                rpe = ResponseParser.ResponseParserEnum.IsError;
                                db.Transaction.Rollback();
                                goto EndLogic;
                            }
                        }
                        discd = (from q in db.FA_DiscDs where q.c_iteno == field.ITENO && q.c_nodisc == "DSXXXXXX03" select q).SingleOrDefault();
                        var harga = (from q in db.FA_MasItms where q.c_iteno == field.ITENO && q.l_aktif == true select q).SingleOrDefault();
                        if (itenocab != field.ITENO)
                        {
                            rcd3 = new LG_RCD3()
                            {
                                c_gdg = cusmas.c_gdg.Value,
                                c_iteno = field.ITENO,
                                c_rcno = rcno,
                                n_disc = discd.n_discon,
                                n_salpri = harga.n_salpri
                            };
                            listrcd3.Add(rcd3);
                        }
                        #region awal
                        if (rn == null)
                        {
                            //RN = "RNXXXXXXXX";
                            //LG_RND1 rnd1 = null;
                            //rnd1 = new LG_RND1
                            //{
                            //    c_gdg = cusmas.c_gdg.Value,
                            //    c_iteno = field.ITENO,
                            //    c_batch = field.BATCH,
                            //    c_rnno = "RNXXXXXXXX",
                            //    n_bqty = 0,
                            //    n_gqty = 0,
                            //    n_gsisa = 0,
                            //    n_bsisa = 0
                            //};
                            //db.LG_RND1s.InsertOnSubmit(rnd1);
                            decimal qty = Convert.ToDecimal(field.N_QTY_RCV);
                            if (donocab == null && itenocab == null && batchcab == null)
                            {
                                rcd1 = new LG_RCD1()
                                {
                                    c_gdg = cusmas.c_gdg.Value,
                                    c_rcno = rcno,
                                    c_iteno = field.ITENO,
                                    c_batch = field.BATCH,
                                    c_type = "01",
                                    c_dono = field.C_DONO,
                                    c_rnno = "RNXXXXXXXX",
                                    n_qty = qty,
                                    n_sisa = qty,
                                    n_qtydestroy = 0,
                                    n_qtyrcv = qty,
                                    v_outlet = "",
                                    c_typeoutlet = "",
                                    c_batchterima = field.BATCH
                                };
                                //db.LG_RCD1s.InsertOnSubmit(rcd1);
                                listrcd1.Add(rcd1);

                                rcd2 = new LG_RCD2()
                                {
                                    c_gdg = cusmas.c_gdg.Value,
                                    c_rcno = rcno,
                                    c_iteno = field.ITENO,
                                    c_batch = field.BATCH,
                                    c_type = "01",
                                    c_dono = field.C_DONO,
                                    c_rnno = "RNXXXXXXXX",
                                    c_spno = "SPXXXXXXXX",
                                    n_qty = qty,
                                    n_sisa = qty
                                };
                                //db.LG_RCD2s.InsertOnSubmit(rcd2);
                                listrcd2.Add(rcd2);

                                donocab = field.C_DONO;
                                itenocab = field.ITENO;
                                batchcab = field.BATCH;
                            }
                            else
                            {
                                if (donocab != field.C_DONO && itenocab != field.ITENO && batchcab != field.BATCH)
                                {
                                    rcd1 = new LG_RCD1()
                                    {
                                        c_gdg = cusmas.c_gdg.Value,
                                        c_rcno = rcno,
                                        c_iteno = field.ITENO,
                                        c_batch = field.BATCH,
                                        c_type = "01",
                                        c_dono = field.C_DONO,
                                        c_rnno = "RNXXXXXXXX",
                                        n_qty = qty,
                                        n_sisa = qty,
                                        n_qtydestroy = 0,
                                        n_qtyrcv = qty,
                                        v_outlet = "",
                                        c_typeoutlet = "",
                                        c_batchterima = field.BATCH
                                    };
                                    //db.LG_RCD1s.InsertOnSubmit(rcd1);
                                    listrcd1.Add(rcd1);

                                    rcd2 = new LG_RCD2()
                                    {
                                        c_gdg = cusmas.c_gdg.Value,
                                        c_rcno = rcno,
                                        c_iteno = field.ITENO,
                                        c_batch = field.BATCH,
                                        c_type = "01",
                                        c_dono = field.C_DONO,
                                        c_rnno = "RNXXXXXXXX",
                                        c_spno = sp.c_spno,
                                        n_qty = qty,
                                        n_sisa = qty
                                    };
                                    //db.LG_RCD2s.InsertOnSubmit(rcd2);
                                    listrcd2.Add(rcd2);
                                    donocab = field.C_DONO;
                                    itenocab = field.ITENO;
                                    batchcab = field.BATCH;

                                }
                                else
                                {
                                    rcd1.n_qty += qty;
                                    rcd1.n_qtyrcv += qty;
                                    rcd1.n_sisa += qty;
                                    rcd2.n_qty += qty;
                                    rcd2.n_sisa += qty;
                                    donocab = field.C_DONO;
                                    itenocab = field.ITENO;
                                    batchcab = field.BATCH;
                                }
                            }
                        }
                        #endregion
                        else
                        #region kedua
                        {
                            if (donocab == null && itenocab == null && batchcab == null)
                            {
                                rcd1 = new LG_RCD1()
                                {
                                    c_gdg = cusmas.c_gdg.Value,
                                    c_rcno = rcno,
                                    c_iteno = field.ITENO,
                                    c_batch = field.BATCH,
                                    c_type = "01",
                                    c_dono = field.C_DONO,
                                    c_rnno = rn.c_rnno,
                                    n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                    n_sisa = Convert.ToDecimal(field.N_QTY_RCV),
                                    n_qtydestroy = 0,
                                    n_qtyrcv = Convert.ToDecimal(field.N_QTY_RCV),
                                    v_outlet = null,
                                    c_typeoutlet = null,
                                    c_batchterima = field.BATCH
                                };
                                //db.LG_RCD1s.InsertOnSubmit(rcd1);
                                listrcd1.Add(rcd1);

                                rcd2 = new LG_RCD2()
                                {
                                    c_gdg = cusmas.c_gdg.Value,
                                    c_rcno = rcno,
                                    c_iteno = field.ITENO,
                                    c_batch = field.BATCH,
                                    c_type = "01",
                                    c_dono = field.C_DONO,
                                    c_rnno = rn.c_rnno,
                                    c_spno = rn.c_spno,
                                    n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                    n_sisa = Convert.ToDecimal(field.N_QTY_RCV)
                                };
                                //db.LG_RCD2s.InsertOnSubmit(rcd2);
                                listrcd2.Add(rcd2);
                                donocab = field.C_DONO;
                                itenocab = field.ITENO;
                                batchcab = field.BATCH;
                                sprc = rn.c_spno;
                            }
                            else
                            {
                                if (itenocab != field.ITENO || batchcab != field.BATCH)
                                {
                                    rcd1 = new LG_RCD1()
                                    {
                                        c_gdg = cusmas.c_gdg.Value,
                                        c_rcno = rcno,
                                        c_iteno = field.ITENO,
                                        c_batch = field.BATCH,
                                        c_type = "01",
                                        c_dono = field.C_DONO,
                                        c_rnno = rn.c_rnno,
                                        n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                        n_sisa = Convert.ToDecimal(field.N_QTY_RCV),
                                        n_qtydestroy = 0,
                                        n_qtyrcv = Convert.ToDecimal(field.N_QTY_RCV),
                                        v_outlet = "",
                                        c_typeoutlet = "",
                                        c_batchterima = field.BATCH
                                    };
                                    //db.LG_RCD1s.InsertOnSubmit(rcd1);
                                    listrcd1.Add(rcd1);

                                    rcd2 = new LG_RCD2()
                                    {
                                        c_gdg = cusmas.c_gdg.Value,
                                        c_rcno = rcno,
                                        c_iteno = field.ITENO,
                                        c_batch = field.BATCH,
                                        c_type = "01",
                                        c_dono = field.C_DONO,
                                        c_rnno = rn.c_rnno,
                                        c_spno = rn.c_spno,
                                        n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                        n_sisa = Convert.ToDecimal(field.N_QTY_RCV)
                                    };
                                    //db.LG_RCD2s.InsertOnSubmit(rcd2);
                                    listrcd2.Add(rcd2);
                                    donocab = field.C_DONO;
                                    itenocab = field.ITENO;
                                    batchcab = field.BATCH;
                                    sprc = rn.c_spno;
                                }
                                else if (itenocab == field.ITENO && batchcab == field.BATCH)
                                {
                                    if (donocab == field.C_DONO)
                                    {
                                        if (sprc == rn.c_spno)
                                        {
                                            rcd1.n_qty += Convert.ToDecimal(field.N_QTY_RCV);
                                            rcd1.n_qtyrcv += Convert.ToDecimal(field.N_QTY_RCV);
                                            rcd1.n_sisa += Convert.ToDecimal(field.N_QTY_RCV);
                                            rcd2.n_qty += Convert.ToDecimal(field.N_QTY_RCV);
                                            rcd2.n_sisa += Convert.ToDecimal(field.N_QTY_RCV);
                                            donocab = field.C_DONO;
                                            itenocab = field.ITENO;
                                            batchcab = field.BATCH;
                                            sprc = rn.c_spno;
                                        }
                                        else if (sprc != rn.c_spno)
                                        {
                                            rcd1.n_qty += Convert.ToDecimal(field.N_QTY_RCV);
                                            rcd1.n_qtyrcv += Convert.ToDecimal(field.N_QTY_RCV);
                                            rcd1.n_sisa += Convert.ToDecimal(field.N_QTY_RCV);
                                            rcd2 = new LG_RCD2()
                                            {
                                                c_gdg = cusmas.c_gdg.Value,
                                                c_rcno = rcno,
                                                c_iteno = field.ITENO,
                                                c_batch = field.BATCH,
                                                c_type = "01",
                                                c_dono = field.C_DONO,
                                                c_rnno = rn.c_rnno,
                                                c_spno = rn.c_spno,
                                                n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                                n_sisa = Convert.ToDecimal(field.N_QTY_RCV)
                                            };
                                            listrcd2.Add(rcd2);
                                            donocab = field.C_DONO;
                                            itenocab = field.ITENO;
                                            batchcab = field.BATCH;
                                            sprc = rn.c_spno;
                                        }
                                        else
                                        {
                                            db.Transaction.Rollback();
                                            rpe = ResponseParser.ResponseParserEnum.IsError;
                                            result = "Nomor SP tidak terdaftar";
                                            goto EndLogic;
                                        }
                                    }
                                    else if (donocab != field.C_DONO)
                                    {
                                        rcd1 = new LG_RCD1()
                                        {
                                            c_gdg = cusmas.c_gdg.Value,
                                            c_rcno = rcno,
                                            c_iteno = field.ITENO,
                                            c_batch = field.BATCH,
                                            c_type = "01",
                                            c_dono = field.C_DONO,
                                            c_rnno = rn.c_rnno,
                                            n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                            n_sisa = Convert.ToDecimal(field.N_QTY_RCV),
                                            n_qtydestroy = 0,
                                            n_qtyrcv = Convert.ToDecimal(field.N_QTY_RCV),
                                            v_outlet = "",
                                            c_typeoutlet = "",
                                            c_batchterima = field.BATCH
                                        };
                                        //db.LG_RCD1s.InsertOnSubmit(rcd1);
                                        listrcd1.Add(rcd1);

                                        rcd2 = new LG_RCD2()
                                        {
                                            c_gdg = cusmas.c_gdg.Value,
                                            c_rcno = rcno,
                                            c_iteno = field.ITENO,
                                            c_batch = field.BATCH,
                                            c_type = "01",
                                            c_dono = field.C_DONO,
                                            c_rnno = rn.c_rnno,
                                            c_spno = rn.c_spno,
                                            n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                            n_sisa = Convert.ToDecimal(field.N_QTY_RCV)
                                        };
                                        //db.LG_RCD2s.InsertOnSubmit(rcd2);
                                        listrcd2.Add(rcd2);
                                        donocab = field.C_DONO;
                                        itenocab = field.ITENO;
                                        batchcab = field.BATCH;
                                        sprc = rn.c_spno;
                                    }
                                    else
                                    {
                                        db.Transaction.Rollback();
                                        rpe = ResponseParser.ResponseParserEnum.IsError;
                                        result = "Nomor DO tidak terdaftar";
                                        goto EndLogic;
                                    }
                                }
                                else
                                {
                                    db.Transaction.Rollback();
                                    rpe = ResponseParser.ResponseParserEnum.IsError;
                                    result = "Ada transaksi yang tidak di kenal mohon hubungi MIS";
                                    goto EndLogic;
                                }

                                #region OldCode
                                /*
                            if (donocab != field.C_DONO && itenocab != field.ITENO && batchcab != field.BATCH)
                            {
                                rcd1 = new LG_RCD1()
                                {
                                    c_gdg = cusmas.c_gdg.Value,
                                    c_rcno = rcno,
                                    c_iteno = field.ITENO,
                                    c_batch = field.BATCH,
                                    c_type = "01",
                                    c_dono = field.C_DONO,
                                    c_rnno = rn.c_rnno,
                                    n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                    n_sisa = Convert.ToDecimal(field.N_QTY_RCV),
                                    n_qtydestroy = 0,
                                    n_qtyrcv = Convert.ToDecimal(field.N_QTY_RCV),
                                    v_outlet = "",
                                    c_typeoutlet = "",
                                    c_batchterima = field.BATCH
                                };
                                //db.LG_RCD1s.InsertOnSubmit(rcd1);
                                listrcd1.Add(rcd1);

                                rcd2 = new LG_RCD2()
                                {
                                    c_gdg = cusmas.c_gdg.Value,
                                    c_rcno = rcno,
                                    c_iteno = field.ITENO,
                                    c_batch = field.BATCH,
                                    c_type = "01",
                                    c_dono = field.C_DONO,
                                    c_rnno = rn.c_rnno,
                                    c_spno = rn.c_spno,
                                    n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                    n_sisa = Convert.ToDecimal(field.N_QTY_RCV)
                                };
                                //db.LG_RCD2s.InsertOnSubmit(rcd2);
                                listrcd2.Add(rcd2);
                                donocab = field.C_DONO;
                                itenocab = field.ITENO;
                                batchcab = field.BATCH;
                                sprc = rn.c_spno;
                            }
                            else if (donocab != field.C_DONO && itenocab == field.ITENO && batchcab == field.BATCH)
                            {
                                rcd1 = new LG_RCD1()
                                {
                                    c_gdg = cusmas.c_gdg.Value,
                                    c_rcno = rcno,
                                    c_iteno = field.ITENO,
                                    c_batch = field.BATCH,
                                    c_type = "01",
                                    c_dono = field.C_DONO,
                                    c_rnno = rn.c_rnno,
                                    n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                    n_sisa = Convert.ToDecimal(field.N_QTY_RCV),
                                    n_qtydestroy = 0,
                                    n_qtyrcv = Convert.ToDecimal(field.N_QTY_RCV),
                                    v_outlet = "",
                                    c_typeoutlet = "",
                                    c_batchterima = field.BATCH
                                };
                                //db.LG_RCD1s.InsertOnSubmit(rcd1);
                                listrcd1.Add(rcd1);

                                rcd2 = new LG_RCD2()
                                {
                                    c_gdg = cusmas.c_gdg.Value,
                                    c_rcno = rcno,
                                    c_iteno = field.ITENO,
                                    c_batch = field.BATCH,
                                    c_type = "01",
                                    c_dono = field.C_DONO,
                                    c_rnno = rn.c_rnno,
                                    c_spno = rn.c_spno,
                                    n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                    n_sisa = Convert.ToDecimal(field.N_QTY_RCV)
                                };
                                //db.LG_RCD2s.InsertOnSubmit(rcd2);
                                listrcd2.Add(rcd2);
                                donocab = field.C_DONO;
                                itenocab = field.ITENO;
                                batchcab = field.BATCH;
                                sprc = rn.c_spno;
                            }
                            else if (donocab == field.C_DONO && itenocab == field.ITENO && batchcab == field.BATCH)
                            {
                                rcd1.n_qty += Convert.ToDecimal(field.N_QTY_RCV);
                                rcd1.n_qtyrcv += Convert.ToDecimal(field.N_QTY_RCV);
                                rcd1.n_sisa += Convert.ToDecimal(field.N_QTY_RCV);

                                if (sprc != rn.c_spno)
                                {
                                    rcd2 = new LG_RCD2()
                                    {
                                        c_gdg = cusmas.c_gdg.Value,
                                        c_rcno = rcno,
                                        c_iteno = field.ITENO,
                                        c_batch = field.BATCH,
                                        c_type = "01",
                                        c_dono = field.C_DONO,
                                        c_rnno = rn.c_rnno,
                                        c_spno = rn.c_spno,
                                        n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                        n_sisa = Convert.ToDecimal(field.N_QTY_RCV)
                                    };
                                    //db.LG_RCD2s.InsertOnSubmit(rcd2);
                                    listrcd2.Add(rcd2);
                                }
                                else if (sprc == rn.c_spno)
                                {
                                    rcd2.n_qty += Convert.ToDecimal(field.N_QTY_RCV);
                                    rcd2.n_sisa += Convert.ToDecimal(field.N_QTY_RCV);
                                }
                                donocab = field.C_DONO;
                                itenocab = field.ITENO;
                                batchcab = field.BATCH;
                                sprc = rn.c_spno;
                            }
                            else
                            {
                                db.Transaction.Rollback();
                                rpe = ResponseParser.ResponseParserEnum.IsError;
                                result = "Ada transaksi yang tidak di kenal mohon hubungi MIS";
                                goto EndLogic;
                            }*/
                                #endregion
                            }
                        }
                        #endregion
                        var batch = (from q in db.LG_MsBatches where q.c_iteno == field.ITENO && q.c_batch == field.BATCH select q).SingleOrDefault();
                        if (batch == null)
                        {
                            LG_MsBatch batches = null;
                            batches = new LG_MsBatch
                            {
                                c_iteno = field.ITENO,
                                c_batch = field.BATCH,
                                d_entry = DateTime.Now,
                                c_entry = "SYSTEM",
                                c_update = "SYSTEM",
                                d_update = DateTime.Now,
                                d_expired = Convert.ToDateTime(field.d_expire)
                            };
                            db.LG_MsBatches.InsertOnSubmit(batches);
                        }
                    }

                    #endregion

                    #region PL & DO & FJ

                    plno = Commons.GenerateNumbering<LG_PLH>(db, "PL", '3', "08", date, "c_plno");
                    dono = Commons.GenerateNumbering<LG_DOH>(db, "DO", '3', "09", date, "c_dono");

                    #region PL Header
                    plh = new LG_PLH
                    {
                        c_plno = plno,
                        d_pldate = date,
                        c_gdg = cusmas.c_gdg,
                        c_cusno = cusmas.c_cusno,
                        c_nosup = supplier,
                        v_ket = "Relokasi dari Cabang " + structure.Fields.CabangAsal + " ke Cabang " + structure.Fields.CabangTerima,
                        c_via = "02",
                        c_type = "03",
                        l_confirm = true,
                        l_print = true,
                        c_entry = "SYSTEM",
                        d_entry = DateTime.Now,
                        c_update = "SYSTEM",
                        d_update = DateTime.Now,
                        l_delete = false,
                        v_ket_mark = structure.Fields.Keterangan,
                        c_type_cat = null,
                        l_wpppic = null,
                        d_print = null,
                        d_confirm = null,
                        c_print = null,
                        c_confirm = null,
                        c_wpppic = null,
                        d_wpppic = null,
                        c_type_lat = null,
                        l_cek = null,
                        l_box = null,
                        c_plnum = null,
                        c_baspbno = null,
                        c_kddivpri = null
                    };
                    db.LG_PLHs.InsertOnSubmit(plh);
                    #endregion

                    #region DO Header
                    doh = new LG_DOH
                    {
                        c_dono = dono,
                        d_dodate = DateTime.Now,
                        c_gdg = cusmas.c_gdg,
                        c_cusno = cusmascab.c_cusno,
                        c_type = "01",
                        c_plno = plno,
                        c_via = "02",
                        v_ket = "Relokasi dari Cabang " + structure.Fields.CabangAsal + " ke Cabang " + structure.Fields.CabangTerima + " No Relokasi " + structure.Fields.Keterangan,
                        c_pin = "1",
                        l_status = false,
                        l_print = false,
                        l_send = true,
                        c_entry = "SYSTEM",
                        d_entry = DateTime.Now,
                        c_update = "SYSTEM",
                        d_update = DateTime.Now,
                        l_auto = false,
                        l_delete = false,
                        l_sent = true,
                        v_ket_mark = null,
                        l_wpdc = false,
                        l_receipt = false,
                        d_receipt = null,
                        c_rnno = null,
                        c_outlet = null,
                        c_po_outlet = null,
                        c_plphar = null,
                        d_rncab = null,
                        n_karton = 0,
                        n_receh = 0,
                        c_editkoli = null
                    };
                    db.LG_DOHs.InsertOnSubmit(doh);
                    #endregion

                    #region FJ Header & FJD3
                    fjh = new LG_FJH
                    {
                        c_fjno = dono.Replace("DO", "FJ"),
                        d_fjdate = DateTime.Now,
                        c_cusno = cusmascab.c_cusno,
                        c_taxno = null,
                        d_taxdate = null,
                        n_top = cusmas.t_top,
                        d_top = DateTime.Now.AddDays(Convert.ToDouble(cusmas.t_top)),
                        n_toppjg = cusmas.t_toppjg,
                        d_toppjg = DateTime.Now.AddDays(Convert.ToDouble(cusmas.t_toppjg)),
                        c_kurs = "01",
                        n_kurs = 1,
                        v_ket = "Relokasi",
                        n_gross = 0,
                        n_disc = 0,
                        n_tax = 0,
                        n_net = 0,
                        n_sisa = 0,
                        l_cabang = true,
                        l_print = false,
                        c_entry = "SYSTEM",
                        d_entry = DateTime.Now,
                        c_update = "SYSTEM",
                        d_update = DateTime.Now,
                        l_delete = null,
                        v_ket_mark = null
                    };
                    db.LG_FJHs.InsertOnSubmit(fjh);

                    fjd3 = new LG_FJD3
                    {
                        c_fjno = dono.Replace("DO", "FJ"),
                        c_dono = dono
                    };
                    db.LG_FJD3s.InsertOnSubmit(fjd3);
                    #endregion


                    for (i = 0; i < structure.Fields.Field.Length; i++)
                    {
                        field = structure.Fields.Field[i];
                        field.ID = dono;
                        var rn = (from q in db.LG_DOHs
                                  join q1 in db.LG_PLD2s on q.c_plno equals q1.c_plno
                                  where q.c_dono == field.C_DONO && q1.c_iteno == field.ITENO && q1.c_batch == field.BATCH
                                  select q1).Take(1).SingleOrDefault();
                        var iteno = (from q in db.FA_MasItms where q.c_iteno == field.ITENO && q.l_aktif == true select q).SingleOrDefault();
                        var disc = (from q in db.FA_DiscDs where q.c_iteno == field.ITENO && q.c_nodisc == "DSXXXXXX03" select q).SingleOrDefault();
                        var sp = (from q in db.LG_SPHs where q.c_sp == field.C_SPNO select q).SingleOrDefault();
                        spd1 = (from q in db.LG_SPD1s where q.c_spno == sp.c_spno && q.c_iteno == field.ITENO select q).SingleOrDefault();

                        #region PL detail
                        if (c_batch == null && c_iteno == null)
                        {
                            pld1 = new LG_PLD1
                            {
                                c_plno = plno,
                                c_iteno = field.ITENO,
                                c_spno = sp.c_spno,
                                c_type = "01",
                                c_batch = field.BATCH,
                                n_booked = Convert.ToDecimal(field.N_QTY_RCV),
                                n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                n_sisa = 0,
                                c_type_dc = null,
                                l_expired = false,
                                v_ket_ed = null,
                                c_acc_ed = null,
                                c_po_outlet = null
                            };
                            listpld1.Add(pld1);

                            if (rn != null)
                            {
                                pld2 = new LG_PLD2
                                {
                                    c_plno = plno,
                                    c_iteno = field.ITENO,
                                    c_spno = sp.c_spno,
                                    c_rnno = rn.c_rnno,
                                    c_batch = field.BATCH,
                                    c_type = "01",
                                    n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                    n_sisa = 0
                                };
                                listpld2.Add(pld2);
                            }
                            else
                            {
                                pld2 = new LG_PLD2
                                {
                                    c_plno = plno,
                                    c_iteno = field.ITENO,
                                    c_spno = sp.c_spno,
                                    c_rnno = "RNXXXXXXXX",
                                    c_batch = field.BATCH,
                                    c_type = "01",
                                    n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                    n_sisa = 0
                                };
                                listpld2.Add(pld2);
                            }
                            if (Convert.ToDecimal(field.N_QTY_REL) != 0)
                            {
                                rel = Convert.ToDecimal(field.N_QTY_REL);
                            }
                            rel = rel - Convert.ToDecimal(field.N_QTY_RCV);
                            if (rel < 0)
                            {
                                spd1.n_sisa = spd1.n_sisa - rel;
                            }
                            spd1.n_relokasi -= Convert.ToDecimal(field.N_QTY_REL);
                            spd1.n_sisa = spd1.n_sisa + Convert.ToDecimal(field.N_QTY_REL) - Convert.ToDecimal(field.N_QTY_RCV);
                        }
                        else
                        {
                            if (c_iteno == field.ITENO && c_batch == field.BATCH)
                            {
                                if (spno != sp.c_spno)
                                {
                                    int cek = 0;
                                    for (cek = 0; cek < listpld1.Count(); cek++)
                                    {
                                        if (listpld1[cek].c_spno == sp.c_spno && listpld1[cek].c_iteno == field.ITENO && listpld1[cek].c_batch == field.BATCH)
                                        {
                                            listpld1[cek].n_booked += Convert.ToDecimal(field.N_QTY_RCV);
                                            listpld1[cek].n_qty += Convert.ToDecimal(field.N_QTY_RCV);
                                            listpld2[cek].n_qty += Convert.ToDecimal(field.N_QTY_RCV);
                                            #region Potong SP
                                            if (Convert.ToDecimal(field.N_QTY_REL) != 0)
                                            {
                                                rel = Convert.ToDecimal(field.N_QTY_REL);
                                            }
                                            rel = rel - Convert.ToDecimal(field.N_QTY_RCV);
                                            if (rel < 0)
                                            {
                                                spd1.n_sisa = spd1.n_sisa - rel;
                                            }

                                            spd1.n_relokasi -= Convert.ToDecimal(field.N_QTY_REL);
                                            spd1.n_sisa = spd1.n_sisa + Convert.ToDecimal(field.N_QTY_REL) - Convert.ToDecimal(field.N_QTY_RCV);
                                            #endregion

                                            goto NextLogic;
                                        }
                                    }
                                    pld1 = new LG_PLD1
                                    {
                                        c_plno = plno,
                                        c_iteno = field.ITENO,
                                        c_spno = sp.c_spno,
                                        c_type = "01",
                                        c_batch = field.BATCH,
                                        n_booked = Convert.ToDecimal(field.N_QTY_RCV),
                                        n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                        n_sisa = 0,
                                        c_type_dc = null,
                                        l_expired = false,
                                        v_ket_ed = null,
                                        c_acc_ed = null,
                                        c_po_outlet = null
                                    };
                                    listpld1.Add(pld1);

                                    if (rn != null)
                                    {
                                        pld2 = new LG_PLD2
                                        {
                                            c_plno = plno,
                                            c_iteno = field.ITENO,
                                            c_spno = sp.c_spno,
                                            c_rnno = rn.c_rnno,
                                            c_batch = field.BATCH,
                                            c_type = "01",
                                            n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                            n_sisa = 0
                                        };
                                        listpld2.Add(pld2);
                                    }
                                    else
                                    {
                                        pld2 = new LG_PLD2
                                        {
                                            c_plno = plno,
                                            c_iteno = field.ITENO,
                                            c_spno = sp.c_spno,
                                            c_rnno = "RNXXXXXXXX",
                                            c_batch = field.BATCH,
                                            c_type = "01",
                                            n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                            n_sisa = 0
                                        };
                                        listpld2.Add(pld2);
                                    }
                                    if (Convert.ToDecimal(field.N_QTY_REL) != 0)
                                    {
                                        rel = Convert.ToDecimal(field.N_QTY_REL);
                                    }
                                    rel = rel - Convert.ToDecimal(field.N_QTY_RCV);
                                    if (rel < 0)
                                    {
                                        spd1.n_sisa = spd1.n_sisa - rel;
                                    }
                                    spd1.n_relokasi -= Convert.ToDecimal(field.N_QTY_REL);
                                    spd1.n_sisa = spd1.n_sisa + Convert.ToDecimal(field.N_QTY_REL) - Convert.ToDecimal(field.N_QTY_RCV);
                                NextLogic:
                                    result = "cek";
                                }
                                else
                                {
                                    pld1.n_booked += Convert.ToDecimal(field.N_QTY_RCV);
                                    pld1.n_qty += Convert.ToDecimal(field.N_QTY_RCV);
                                    pld2.n_qty += Convert.ToDecimal(field.N_QTY_RCV);
                                    if (Convert.ToDecimal(field.N_QTY_REL) != 0)
                                    {
                                        rel = Convert.ToDecimal(field.N_QTY_REL);
                                    }
                                    rel = rel - Convert.ToDecimal(field.N_QTY_RCV);
                                    if (rel < 0)
                                    {
                                        spd1.n_sisa = spd1.n_sisa - rel;
                                    }
                                    spd1.n_relokasi -= Convert.ToDecimal(field.N_QTY_REL);
                                    spd1.n_sisa = spd1.n_sisa + Convert.ToDecimal(field.N_QTY_REL) - Convert.ToDecimal(field.N_QTY_RCV);
                                }
                            }
                            else if (c_iteno != field.ITENO || c_batch != field.BATCH)
                            {
                                pld1 = new LG_PLD1
                                {
                                    c_plno = plno,
                                    c_iteno = field.ITENO,
                                    c_spno = sp.c_spno,
                                    c_type = "01",
                                    c_batch = field.BATCH,
                                    n_booked = Convert.ToDecimal(field.N_QTY_RCV),
                                    n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                    n_sisa = 0,
                                    c_type_dc = null,
                                    l_expired = false,
                                    v_ket_ed = null,
                                    c_acc_ed = null,
                                    c_po_outlet = null
                                };
                                listpld1.Add(pld1);

                                if (rn != null)
                                {
                                    pld2 = new LG_PLD2
                                    {
                                        c_plno = plno,
                                        c_iteno = field.ITENO,
                                        c_spno = sp.c_spno,
                                        c_rnno = rn.c_rnno,
                                        c_batch = field.BATCH,
                                        c_type = "01",
                                        n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                        n_sisa = 0
                                    };
                                    listpld2.Add(pld2);
                                }
                                else
                                {
                                    pld2 = new LG_PLD2
                                    {
                                        c_plno = plno,
                                        c_iteno = field.ITENO,
                                        c_spno = sp.c_spno,
                                        c_rnno = "RNXXXXXXXX",
                                        c_batch = field.BATCH,
                                        c_type = "01",
                                        n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                        n_sisa = 0
                                    };
                                    listpld2.Add(pld2);
                                }
                                if (Convert.ToDecimal(field.N_QTY_REL) != 0)
                                {
                                    rel = Convert.ToDecimal(field.N_QTY_REL);
                                }
                                rel = rel - Convert.ToDecimal(field.N_QTY_RCV);
                                if (rel < 0)
                                {
                                    spd1.n_sisa = spd1.n_sisa - rel;
                                }
                                spd1.n_relokasi -= Convert.ToDecimal(field.N_QTY_REL);
                                spd1.n_sisa = spd1.n_sisa + Convert.ToDecimal(field.N_QTY_REL) - Convert.ToDecimal(field.N_QTY_RCV);

                            }
                        }
                        #endregion

                        #region DO detail
                        if (c_batch == null && c_iteno == null)
                        {
                            dod1 = new LG_DOD1
                            {
                                c_dono = dono,
                                c_via = "02",
                                c_iteno = field.ITENO,
                                n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                n_sisa = Convert.ToDecimal(field.N_QTY_RCV)
                            };
                            listdod1.Add(dod1);

                            dod2 = new LG_DOD2
                            {
                                c_dono = dono,
                                c_via = "02",
                                c_iteno = field.ITENO,
                                c_batch = field.BATCH,
                                n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                n_sisa = Convert.ToDecimal(field.N_QTY_RCV)
                            };
                            listdod2.Add(dod2);
                        }
                        else
                        {
                            if (c_iteno == field.ITENO && c_batch == field.BATCH)
                            {
                                dod1.n_qty += Convert.ToDecimal(field.N_QTY_RCV);
                                dod1.n_sisa += Convert.ToDecimal(field.N_QTY_RCV);
                                dod2.n_qty += Convert.ToDecimal(field.N_QTY_RCV);
                                dod2.n_sisa += Convert.ToDecimal(field.N_QTY_RCV);
                            }
                            else if (c_iteno != field.ITENO || c_batch != field.BATCH)
                            {
                                if (c_iteno == field.ITENO)
                                {
                                    dod1.n_qty += Convert.ToDecimal(field.N_QTY_RCV);
                                    dod1.n_sisa += Convert.ToDecimal(field.N_QTY_RCV);
                                }
                                else
                                {
                                    dod1 = new LG_DOD1
                                    {
                                        c_dono = dono,
                                        c_via = "02",
                                        c_iteno = field.ITENO,
                                        n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                        n_sisa = Convert.ToDecimal(field.N_QTY_RCV)
                                    };
                                    listdod1.Add(dod1);
                                }

                                dod2 = new LG_DOD2
                                {
                                    c_dono = dono,
                                    c_via = "02",
                                    c_iteno = field.ITENO,
                                    c_batch = field.BATCH,
                                    n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                    n_sisa = Convert.ToDecimal(field.N_QTY_RCV)
                                };
                                listdod2.Add(dod2);
                            }
                        }
                        #endregion

                        #region FJ Detail
                        if (c_batch == null && c_iteno == null)
                        {
                            fjd1 = new LG_FJD1
                            {
                                c_fjno = dono.Replace("DO", "FJ"),
                                c_iteno = field.ITENO,
                                n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                n_disc = 0,
                                n_salpri = iteno.n_salpri
                            };
                            listfjd1.Add(fjd1);

                            fjd2 = new LG_FJD2
                            {
                                c_fjno = dono.Replace("DO", "FJ"),
                                c_iteno = field.ITENO,
                                c_type = "03",
                                n_discon = disc.n_discon,
                                n_discoff = disc.n_discoff,
                                c_no = "DSXXXXXX03",
                                n_neton = Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri * disc.n_discon / 100,
                                n_netoff = Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri * disc.n_discoff / 100,
                                n_sisaon = (Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri) - (Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri * disc.n_discon / 100),
                                n_sisaoff = Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri
                            };
                            ngross = ngross + (Convert.ToDecimal(field.N_QTY_RCV) * Convert.ToDecimal(iteno.n_salpri));
                            ndisc = ndisc + Convert.ToDecimal(Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri * disc.n_discon / 100);
                            nnet = nnet + Convert.ToDecimal((Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri) - (Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri * disc.n_discon / 100));
                            listfjd2.Add(fjd2);
                            c_iteno = field.ITENO;
                            c_batch = field.BATCH;
                            spno = sp.c_spno;
                        }
                        else
                        {
                            if (c_iteno == field.ITENO)
                            {
                                fjd1.n_qty += Convert.ToDecimal(field.N_QTY_RCV);
                                fjd2.n_neton += Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri * disc.n_discon / 100;
                                fjd2.n_netoff += Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri * disc.n_discoff / 100;
                                fjd2.n_sisaon += (Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri) - (Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri * disc.n_discon / 100);
                                fjd2.n_sisaoff += Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri;
                                ngross = ngross + (Convert.ToDecimal(field.N_QTY_RCV) * Convert.ToDecimal(iteno.n_salpri));
                                ndisc = ndisc + Convert.ToDecimal(Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri * disc.n_discon / 100);
                                nnet = nnet + Convert.ToDecimal((Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri) - (Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri * disc.n_discon / 100));
                                c_iteno = field.ITENO;
                                c_batch = field.BATCH;
                                spno = sp.c_spno;
                            }
                            else if (c_iteno != field.ITENO)
                            {
                                fjd1 = new LG_FJD1
                                {
                                    c_fjno = dono.Replace("DO", "FJ"),
                                    c_iteno = field.ITENO,
                                    n_qty = Convert.ToDecimal(field.N_QTY_RCV),
                                    n_disc = 0,
                                    n_salpri = iteno.n_salpri
                                };
                                listfjd1.Add(fjd1);

                                fjd2 = new LG_FJD2
                                {
                                    c_fjno = dono.Replace("DO", "FJ"),
                                    c_iteno = field.ITENO,
                                    c_type = "03",
                                    n_discon = disc.n_discon,
                                    n_discoff = disc.n_discoff,
                                    c_no = "DSXXXXXX03",
                                    n_neton = Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri * disc.n_discon / 100,
                                    n_netoff = Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri * disc.n_discoff / 100,
                                    n_sisaon = (Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri) - (Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri * disc.n_discon / 100),
                                    n_sisaoff = Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri
                                };
                                ngross = ngross + (Convert.ToDecimal(field.N_QTY_RCV) * Convert.ToDecimal(iteno.n_salpri));
                                ndisc = ndisc + Convert.ToDecimal(Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri * disc.n_discon / 100);
                                nnet = nnet + Convert.ToDecimal((Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri) - (Convert.ToDecimal(field.N_QTY_RCV) * iteno.n_salpri * disc.n_discon / 100));
                                listfjd2.Add(fjd2);
                                c_iteno = field.ITENO;
                                c_batch = field.BATCH;
                                spno = sp.c_spno;
                            }
                            else
                            {
                                db.Transaction.Rollback();
                                rpe = ResponseParser.ResponseParserEnum.IsError;
                                result = "FJ tidak memenuhi syarat";
                                goto EndLogic;
                            }
                        }
                        #endregion


                    }
                    fjh.n_gross = ngross;
                    fjh.n_disc = ndisc;
                    fjh.n_net = nnet;
                    fjh.n_sisa = nnet;

                    #endregion

                }
                else
                {

                    var cek = (from q in db.LG_RCHes 
                                 join q1 in db.LG_DOHs on q.v_ket equals q1.v_ket.Substring(51,16)
                                 where q.v_pbbrno == structure.Fields.C_PBNO
                                select q1).SingleOrDefault();
                    dono = cek.c_dono.ToString();
                    goto DOResendLogic;
                }


            }
            else
            {
                result = "Metode tidak di kenal";
                rpe = ResponseParser.ResponseParserEnum.IsError;
                db.Transaction.Rollback();
                goto EndLogic;
            }

            if (listrcd1.Count > 0)
            {
                db.LG_RCD1s.InsertAllOnSubmit(listrcd1.ToArray());
                listrcd1.Clear();
            }
            if (listrcd2.Count > 0)
            {
                db.LG_RCD2s.InsertAllOnSubmit(listrcd2.ToArray());
                listrcd2.Clear();
            }
            if (listrcd3.Count > 0)
            {
                db.LG_RCD3s.InsertAllOnSubmit(listrcd3.ToArray());
                listrcd3.Clear();
            }
            if (listpld1.Count > 0)
            {
                db.LG_PLD1s.InsertAllOnSubmit(listpld1.ToArray());
                listpld1.Clear();
            }
            if (listpld2.Count > 0)
            {
                db.LG_PLD2s.InsertAllOnSubmit(listpld2.ToArray());
                listpld2.Clear();
            }
            if (listdod1.Count > 0)
            {
                db.LG_DOD1s.InsertAllOnSubmit(listdod1.ToArray());
                listdod1.Clear();
            }
            if (listdod2.Count > 0)
            {
                db.LG_DOD2s.InsertAllOnSubmit(listdod2.ToArray());
                listdod2.Clear();
            }
            if (listfjd1.Count > 0)
            {
                db.LG_FJD1s.InsertAllOnSubmit(listfjd1.ToArray());
                listfjd1.Clear();
            }
            if (listfjd2.Count > 0)
            {
                db.LG_FJD2s.InsertAllOnSubmit(listfjd2.ToArray());
                listfjd2.Clear();
            }

            RSID = ResponToDisCorePB(db, structure, rcno, date);
            if (RSID == null)
            {
                db.Transaction.Rollback();
                result = "Gagal Update";
                rpe = ResponseParser.ResponseParserEnum.IsError;
                goto EndLogic;
            }
            db.SubmitChanges();
            db.Transaction.Commit();
        DOResendLogic:
            rpe = ResponseParser.ResponseParserEnum.IsSuccess;
            result = dono;
            Modules.CommonQuerySP post = new ScmsSoaLibrary.Modules.CommonQuerySP();
            post.PostDataDO(db.Connection.ConnectionString, dono, false);
        }
        catch (Exception ex)
        {
            db.Transaction.Rollback();
            rpe = ResponseParser.ResponseParserEnum.IsError;
            result = "Data Gagal di commit. " + ex.Message;
            goto EndLogic;
        }
    EndLogic:
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

    public string CancelPB(ScmsSoaLibrary.Parser.Class.CancelPBStructure structure)
    {
        if ((structure == null) || (structure.Fields == null))
        {
            return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
        }

        bool hasAnyChanges = false;
        string result = null;

        bool isContexted = false;
        ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);
        int i = 0, a = 0, idet = 0;
        if (db == null)
        {
            db = new ORMDataContext(Functionals.ActiveConnectionString);
        }
        LG_SPH sph = null;
        LG_SPD1 spd1 = null;

        ScmsSoaLibrary.Parser.Class.RelokasiStructure recall = null;
        ScmsSoaLibrary.Parser.Class.RelokasiStructureField field = null;

        ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
        IDictionary<string, string> dic = null;

        try
        {
            if (!isContexted)
            {
                db.Connection.Open();
                db.Transaction = db.Connection.BeginTransaction();
            }
            if (structure.Method.Equals("del", StringComparison.OrdinalIgnoreCase))
            {
                for (i = 0; i < structure.Fields.Field.Length; i++)
                {
                    if (structure.Fields.Field[i].SPNO != string.Empty)
                    {
                        sph = (from q in db.LG_SPHs
                               where q.c_sp == structure.Fields.Field[i].SPNO
                               select q).SingleOrDefault();
                        spd1 = (from q in db.LG_SPD1s
                                where q.c_spno == sph.c_spno && q.c_iteno == structure.Fields.Field[i].ITENO
                                select q).SingleOrDefault();

                        if (spd1.n_relokasi < Convert.ToInt32(structure.Fields.Field[i].N_QTYREL))
                        {
                            result = "Sisa relokasi " + sph.c_spno + " lebih kecil dari pada yang mau di kembalikan.";
                            rpe = ResponseParser.ResponseParserEnum.IsError;
                            db.Transaction.Rollback();
                            goto EndLogic;
                        }
                        else
                        {
                            spd1.n_sisa = spd1.n_sisa + Convert.ToInt32(structure.Fields.Field[i].N_QTYREL);
                            spd1.n_relokasi = spd1.n_relokasi - Convert.ToInt32(structure.Fields.Field[i].N_QTYREL);
                        }
                    }
                }
            }
            db.SubmitChanges();
            db.Transaction.Commit();
            rpe = ResponseParser.ResponseParserEnum.IsSuccess;
            result = "Data telah di commit";
        }
        catch (Exception ex)
        {
            db.Transaction.Rollback();
            rpe = ResponseParser.ResponseParserEnum.IsError;
            result = "Data Gagal di commit " + ex.Message;
            goto EndLogic;
        }
    EndLogic:
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

    public string ReceivePO(ScmsSoaLibrary.Parser.Class.ReceivePOStructure structure)
    {
        if ((structure == null) || (structure.Fields == null))
        {
            return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
        }
        bool hasAnyChanges = false;
        string result = null;
        string iteno = null;
        string cab = null;
        bool isContexted = false;
        int i = 0;
        decimal n_bruto = 0, n_disc = 0, n_ppn = 0, n_bilva = 0, n_qty = 0, n_salpri = 0;
        ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);
        if (db == null)
        {
            db = new ORMDataContext(Functionals.ActiveConnectionString);
        }

        LG_CusmasCab cusmas = null;
        LG_POH POH = null;
        LG_POD1 POD1 = null;
        LG_POD2 POD2 = null;
        LG_ORH ORH = null;
        LG_ORD1 ORD1 = null;
        LG_ORD2 ORD2 = null;
        LG_ORD3 ORD3 = null;
        LG_SPD1 spd = null;
        LG_DatSup SUPPLIER = null;
        FA_MasItm msitm = null;
        List<LG_POD1> listPOD1 = null;
        List<LG_POD2> listPOD2 = null;
        List<LG_ORD1> listORD1 = null;
        List<LG_ORD2> listORD2 = null;
        List<LG_ORD3> listORD3 = null;
        listPOD1 = new List<LG_POD1>();
        listPOD2 = new List<LG_POD2>();
        listORD1 = new List<LG_ORD1>();
        listORD2 = new List<LG_ORD2>();
        listORD3 = new List<LG_ORD3>();
        ScmsSoaLibrary.Parser.Class.ReceivePOStructureField field = null;
        ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
        IDictionary<string, string> dic = null;

        try
        {
            if (!isContexted)
            {
                db.Connection.Open();
                db.Transaction = db.Connection.BeginTransaction();
            }
            #region add
            if (structure.Method.Equals("add", StringComparison.OrdinalIgnoreCase))
            {
                cusmas = (from q in db.LG_CusmasCabs where q.c_cab_dcore == structure.Fields.C_CAB select q).SingleOrDefault();
                var POHCHECK = (from q in db.LG_POHs where q.c_pono == structure.Fields.C_PONO select q).Count();
                if (POHCHECK != 0)
                {
                    rpe = ResponseParser.ResponseParserEnum.IsError;
                    result = "Data sudah ada";
                    goto EndLogic;
                }
                n_bruto = Convert.ToDecimal(structure.Fields.N_BRUTO);
                n_disc = Convert.ToDecimal(structure.Fields.N_DISC);
                n_ppn = Convert.ToDecimal(structure.Fields.N_PPN);
                n_bilva = Convert.ToDecimal(structure.Fields.N_BILVA);
                ORH = new LG_ORH
                {
                    c_gdg = '1',
                    c_orno = structure.Fields.C_ORNO,
                    d_ordate = Convert.ToDateTime(structure.Fields.D_PODATE),
                    c_nosup = structure.Fields.C_NOSUP,
                    v_ket = structure.Fields.V_KET,
                    c_type = structure.Fields.C_TYPE,
                    l_status = true,
                    l_print = false,
                    c_entry = structure.Fields.C_ENTRY,
                    d_entry = DateTime.Now,
                    c_update = structure.Fields.C_ENTRY,
                    d_update = DateTime.Now,
                    l_delete = null,
                    v_ket_mark = null
                };
                db.LG_ORHs.InsertOnSubmit(ORH);
                if (cusmas != null)
                {
                    cab = cusmas.c_cab;
                }
                else
                {
                    cab = "";
                }
                POH = new LG_POH
                {
                    c_gdg = '1',
                    c_pono = structure.Fields.C_PONO,
                    d_podate = Convert.ToDateTime(structure.Fields.D_PODATE),
                    c_nosup = structure.Fields.C_NOSUP,
                    l_import = false,
                    c_kurs = "01",
                    n_kurs = 1,
                    v_ket = structure.Fields.V_KET,
                    n_bruto = n_bruto,
                    n_disc = n_disc,
                    n_pdisc = 0,
                    n_xdisc= 0,
                    n_ppn = n_ppn,
                    n_bilva = n_bilva,
                    l_print = false,
                    l_send = false,
                    l_revisi = false,
                    c_entry = structure.Fields.C_ENTRY,
                    d_entry = DateTime.Now,
                    c_update = structure.Fields.C_ENTRY,
                    d_update = DateTime.Now,
                    l_delete = false,
                    l_sent = false,
                    v_ket_mark = null,
                    c_cab = cab,
                    c_type = null,
                    d_posender = null,
                    n_leadtime_datsup = 6,
                    l_cek_apj = true,
                    l_cek_ppic = true,
                };
                db.LG_POHs.InsertOnSubmit(POH);
                POD2 = new LG_POD2
                {
                    c_gdg = '1',
                    c_pono = structure.Fields.C_PONO,
                    c_orno = structure.Fields.C_ORNO
                };
                db.LG_POD2s.InsertOnSubmit(POD2);
                if (structure.Fields.C_TYPE == "05")
                {
                    #region PO Reguler
                    for (i = 0; i < structure.Fields.field.Length; i++)
                    {
                        field = structure.Fields.field[i];
                        n_qty = Convert.ToDecimal(field.N_QTY);
                        n_disc = Convert.ToDecimal(field.N_DISC);
                        n_salpri = Convert.ToDecimal(field.N_SALPRI); 
                        POD1 = new LG_POD1
                        {
                            c_gdg = '1',
                            c_pono = structure.Fields.C_PONO,
                            c_iteno = field.C_ITENO,
                            n_qty = n_qty,
                            n_disc = n_disc,
                            n_salpri = n_salpri,
                            n_sisa = n_qty
                        };
                        listPOD1.Add(POD1);
                        msitm = (from q in db.FA_MasItms where q.c_iteno == field.C_ITENO && q.l_aktif == true select q).SingleOrDefault();
                        ORD1 = new LG_ORD1
                        {
                            c_gdg = '1',
                            c_orno = structure.Fields.C_ORNO,
                            c_iteno = field.C_ITENO,
                            n_qty = n_qty,
                            n_sisa = n_qty,
                            n_avgsls = 0,
                            n_index = 0,
                            n_soh = 0,
                            n_sit = 0,
                            n_bo = 0,
                            n_spacc = 0,
                            n_box = msitm.n_box,
                            n_salpri = n_salpri,
                            n_pminord = 0,
                            n_qminord = 0,
                            n_bonus = 0,
                            n_beli = 0,
                            c_via = "01",
                            c_type = "01",
                            c_kddivpri = structure.Fields.C_KDDIVPRI,
                            n_avgslsdivpri = 0,
                            n_variabel = 0,
                            n_idxp = 0,
                            n_idxnp = 0,
                            n_pareto = 0,
                            n_ideal = 0,
                            n_order = 0,
                            n_deviasi = 0
                        };
                        listORD1.Add(ORD1);
                    }
                    #endregion
                }
                else if (structure.Fields.C_TYPE == "02")
                {
                    #region PO Direct Shipment dan Cross Docking
                    for (i = 0; i < structure.Fields.field.Length; i++)
                    {
                        field = structure.Fields.field[i];
                        n_qty = Convert.ToDecimal(field.N_QTY);
                        n_disc = Convert.ToDecimal(field.N_DISC);
                        n_salpri = Convert.ToDecimal(field.N_SALPRI);
                        if (iteno != field.C_ITENO || iteno == null)
                        {
                            POD1 = new LG_POD1
                            {
                                c_gdg = '1',
                                c_pono = structure.Fields.C_PONO,
                                c_iteno = field.C_ITENO,
                                n_qty = n_qty,
                                n_disc = n_disc,
                                n_salpri = n_salpri,
                                n_sisa = n_qty
                            };
                            listPOD1.Add(POD1);
                            msitm = (from q in db.FA_MasItms where q.c_iteno == field.C_ITENO && q.l_aktif == true select q).SingleOrDefault();
                            ORD1 = new LG_ORD1
                            {
                                c_gdg = '1',
                                c_orno = structure.Fields.C_ORNO,
                                c_iteno = field.C_ITENO,
                                n_qty = n_qty,
                                n_sisa = n_qty,
                                n_avgsls = 0,
                                n_index = 0,
                                n_soh = 0,
                                n_sit = 0,
                                n_bo = 0,
                                n_spacc = 0,
                                n_box = msitm.n_box,
                                n_salpri = n_salpri,
                                n_pminord = 0,
                                n_qminord = 0,
                                n_bonus = 0,
                                n_beli = 0,
                                c_via = "01",
                                c_type = "01",
                                c_kddivpri = structure.Fields.C_KDDIVPRI,
                                n_avgslsdivpri = 0,
                                n_variabel = 0,
                                n_idxp = 0,
                                n_idxnp = 0,
                                n_pareto = 0,
                                n_ideal = 0,
                                n_order = 0,
                                n_deviasi = 0
                            };
                            listORD1.Add(ORD1);
                            iteno = field.C_ITENO;
                        }
                        else
                        {
                            POD1.n_sisa = POD1.n_sisa + n_qty;
                            POD1.n_qty = POD1.n_qty + n_qty;
                            ORD1.n_qty = ORD1.n_qty + n_qty;
                            ORD1.n_sisa = ORD1.n_sisa + n_qty;
                        }
                        var spno = (from q in db.LG_SPHs where q.c_sp == field.C_SPNO select q).SingleOrDefault();
                        if (spno == null)
                        {
                            db.Transaction.Rollback();
                            rpe = ResponseParser.ResponseParserEnum.IsError;
                            result = "Data tidak bisa di save karena tidak ada sp nya.";
                            goto EndLogic;
                        }
                        spd = (from q in db.LG_SPD1s where q.c_spno == spno.c_spno && q.c_iteno == field.C_ITENO select q).SingleOrDefault();
                        spd.n_sisa = spd.n_sisa - n_qty;
                        if (spd.n_spds != null)
                        {
                            spd.n_spds = spd.n_spds + n_qty;
                        }
                        else
                        {
                            spd.n_spds = n_qty;
                        }
                        ORD2 = new LG_ORD2
                        {
                            c_gdg = '1',
                            c_orno = structure.Fields.C_ORNO,
                            c_spno = spno.c_spno,
                            c_iteno = field.C_ITENO,
                            c_type = "01",
                            n_sisa = n_qty,
                            c_itemcombo = null
                        };
                        listORD2.Add(ORD2);

                        ORD3 = new LG_ORD3
                        {
                            c_gdg = '1',
                            c_orno = structure.Fields.C_ORNO,
                            c_pono = structure.Fields.C_PONO,
                            c_iteno = field.C_ITENO,
                            n_sisa = n_qty
                        };
                    }
                    #endregion
                }
                if (listORD1.Count() > 0)
                {
                    db.LG_ORD1s.InsertAllOnSubmit(listORD1);
                    listORD1.Clear();
                }
                if (listORD2.Count() > 0)
                {
                    db.LG_ORD2s.InsertAllOnSubmit(listORD2);
                    listORD2.Clear();
                }
                if (listORD3.Count() > 0)
                {
                    db.LG_ORD3s.InsertAllOnSubmit(listORD3);
                    listORD3.Clear();
                }
                if (listPOD1.Count() > 0)
                {
                    db.LG_POD1s.InsertAllOnSubmit(listPOD1);
                    listPOD1.Clear();
                }
                db.SubmitChanges();
                db.Transaction.Commit();
                rpe = ResponseParser.ResponseParserEnum.IsSuccess;
                result = "Data Sukses di simpan.";
            }
            #endregion

        }
        catch (Exception ex)
        {
            db.Transaction.Rollback();
            rpe = ResponseParser.ResponseParserEnum.IsError;
            result = "Data Gagal di commit " + ex.Message;
            goto EndLogic;
        }
        EndLogic:
        result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

        return result;
    }

    public string CancelSP(ScmsSoaLibrary.Parser.Class.CancelSPStructure structure)
    {
        if ((structure == null) || (structure.Fields == null))
        {
            return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
        }

        bool hasAnyChanges = false;
        string result = null;

        bool isContexted = false;
        ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);
        int i = 0, a = 0, idet = 0;
        if (db == null)
        {
            db = new ORMDataContext(Functionals.ActiveConnectionString);
        }
        LG_SPH sph = null;
        LG_SPD1 spd1 = null;

        ScmsSoaLibrary.Parser.Class.RelokasiStructure recall = null;
        ScmsSoaLibrary.Parser.Class.RelokasiStructureField field = null;

        ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
        IDictionary<string, string> dic = null;

        try
        {
            if (!isContexted)
            {
                db.Connection.Open();
                db.Transaction = db.Connection.BeginTransaction();
            }
            if (structure.Method.Equals("del", StringComparison.OrdinalIgnoreCase))
            {
                sph = (from q in db.LG_SPHs where q.c_sp == structure.Fields.SPNO select q).SingleOrDefault();
                spd1 = (from q in db.LG_SPD1s where q.c_spno == sph.c_spno && q.c_iteno == structure.Fields.ITENO select q).SingleOrDefault();

                spd1.n_relokasi = spd1.n_relokasi - Convert.ToInt32(structure.Fields.QTYREL);
                spd1.n_sisa = spd1.n_sisa + Convert.ToInt32(structure.Fields.QTYREL);
            }
            db.SubmitChanges();
            db.Transaction.Commit();
            rpe = ResponseParser.ResponseParserEnum.IsSuccess;
            result = "Data telah di commit";
        }
        catch (Exception ex)
        {
            db.Transaction.Rollback();
            rpe = ResponseParser.ResponseParserEnum.IsError;
            result = "Data Gagal di commit " + ex.Message;
            goto EndLogic;
        }
    EndLogic:
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

    public string MasterRelokasi(ScmsSoaLibrary.Parser.Class.MasterRelokasiStructure structure)
    {
        if ((structure == null) || (structure.Fields == null))
        {
            return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
        }

        bool hasAnyChanges = false;
        string result = null;

        bool isContexted = false;
        ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);
        int i = 0, a = 0, idet = 0;
        if (db == null)
        {
            db = new ORMDataContext(Functionals.ActiveConnectionString);
        }

        lg_relo_cab relo = null;
        List<lg_relo_cab> listrelo = null;
        listrelo = new List<lg_relo_cab>();

        ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
        IDictionary<string, string> dic = null;

        try
        {
            if (!isContexted)
            {
                db.Connection.Open();
                db.Transaction = db.Connection.BeginTransaction();
            }
            if (structure.Method.Equals("add", StringComparison.OrdinalIgnoreCase))
            {
                string sSql = "delete from lg_relo_cab where kodecab = '" + structure.Fields.c_asal + "'";
                db.ExecuteCommand(sSql);
                
                for (i = 0; i < structure.Fields.Field.Length; i++)
                {
                    relo = new lg_relo_cab
                    {
                        KODECAB = structure.Fields.Field[i].c_asal,
                        JENIS = Convert.ToChar(structure.Fields.Field[i].c_jenis),
                        TUJUAN = structure.Fields.Field[i].c_tujuan,
                        LTO = Convert.ToInt32(structure.Fields.Field[i].c_lto)
                    };
                    listrelo.Add(relo);
                }
                if (listrelo.Count > 0)
                {
                    db.lg_relo_cabs.InsertAllOnSubmit(listrelo);
                    listrelo.Clear();
                }
            }
            db.SubmitChanges();
            db.Transaction.Commit();
            rpe = ResponseParser.ResponseParserEnum.IsSuccess;
            result = "Data telah di commit";
        }
        catch (Exception ex)
        {
            db.Transaction.Rollback();
            rpe = ResponseParser.ResponseParserEnum.IsError;
            result = "Data Gagal di commit " + ex.Message;
            goto EndLogic;
        }
    EndLogic:
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
  }
}
