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
using System.IO;
using Excel;
using System.Data;
using ScmsSoaLibraryInterface.Commons;
using System.Data.OleDb;

namespace ScmsSoaLibrary.Modules
{
  partial class CommonUploadedQuery
  {
    #region Internal Class

    //Gudang	RsNo	Item	Batch	Salpri	Disc	Gqty	Bqty	Ex.Faktur	Tax	TaxDate
    internal class UploadImportRS
    {
      public char Gudang { get; set; }
      public string RsNo { get; set; }
      public string Item { get; set; }
      public string Batch { get; set; }
      public decimal Salpri { get; set; }
      public decimal Disc { get; set; }
      public decimal Gqty { get; set; }
      public decimal Bqty { get; set; }
      public string Ex_Faktur { get; set; }
      public string Tax { get; set; }
      public DateTime TaxDate { get; set; }
    }

    internal class UploadImportExp
    {
      public string customer { get; set; }
      public string customerDesc { get; set; }
      public string expno { get; set; }
      public string expedisi { get; set; }
      public string expedisiDesc { get; set; }
      public string resino { get; set; }
      public string tglresi { get; set; }
      public string tglexpcab { get; set; }
      public string wktexpcab { get; set; }
      public DateTime Dtglresi { get; set; }
      public DateTime Dtglexpcab { get; set; }
      public TimeSpan Twktexpcab { get; set; }
    }

    internal class Temporary_ImportRS
    {
      public char c_gdg { get; set; }
      public string c_rsno { get; set; }
      public string c_iteno { get; set; }
      public string v_itnam { get; set; }
      public string c_batch { get; set; }
      public string c_fb { get; set; }
      public string c_taxno { get; set; }
      public DateTime d_taxdate { get; set; }
      public decimal n_salpri { get; set; }
      public decimal n_disc { get; set; }
      public decimal n_gsisa { get; set; }
      public decimal n_bsisa { get; set; }
      public string c_up_exfaktur { get; set; }
      public string c_up_taxno { get; set; }
      public DateTime d_up_taxdate { get; set; }
      public decimal n_up_salpri { get; set; }
      public decimal n_up_disc { get; set; }
      public decimal n_up_gsisa { get; set; }
      public decimal n_up_bsisa { get; set; }
      public string v_ket { get; set; }
      public string c_rnno { get; set; }
    }

    internal class TempItemList
    {
      public string Item { get; set; }
      public string ItemName { get; set; }
    }

    internal class UploadImportFB
    {
      public char Gudang { get; set; }
      public string Supplier { get; set; }
      public string RnNo { get; set; }
      public string DoNo { get; set; }
      public string Faktur { get; set; }
      public DateTime FbDate { get; set; }
      public int Top { get; set; }
      public int TopPjg { get; set; }
      public string Tax { get; set; }
      public DateTime TaxDate { get; set; }
      public decimal Bea { get; set; }
      public decimal XDisc { get; set; }
      public decimal ValueFaktur { get; set; }
      public decimal n_ppph { get; set; }
    }

    internal class Temporary_ImportFB
    {
      public char Gudang { get; set; }
      public string Supplier { get; set; }
      public string RnNo { get; set; }
      public string DoNo { get; set; }
      public string Keterangan { get; set; }
    }

    internal class Temporary_ImportExp
    {
      public string customer { get; set; }
      public string customerDesc { get; set; }
      public string expno { get; set; }
      public string expedisi { get; set; }
      public string expedisiDesc { get; set; }
      public string resino { get; set; }
      public string tglresi { get; set; }
      public string tglexpcab { get; set; }
      public string wktexpcab { get; set; }
      public DateTime Dtglresi { get; set; }
      public DateTime Dtglexpcab { get; set; }
      public TimeSpan Twktexpcab { get; set; }
      public string c_type { get; set; }
      public string v_ket { get; set; }
      public string l_new { get; set; }
    }

    internal class UploadSP
    {
      public string c_sp { get; set; }
      public string TipeSP { get; set; }
      public string kdCab { get; set; }
      public string kdCabSCMS { get; set; }
      public string Cabang { get; set; }
      public string kodeitem { get; set; }
      public string namaItem { get; set; }
      public string TglSP { get; set; }
      public decimal Qty { get; set; }
      
    }

    internal class Temporary_SP
    {
      public string c_sp { get; set; }
      public string TipeSP { get; set; }
      public string kdCab { get; set; }
      public string kdCabSCMS { get; set; }
      public string Cabang { get; set; }
      public string kodeitem { get; set; }
      public string namaItem { get; set; }
      public string TglSP { get; set; }
      public decimal Qty { get; set; }
    }

    internal class Temporary_Voucher
    {
        public string Principal { get; set; }
        public string c_fbno { get; set; }
        public decimal n_value { get; set; }
        public string c_vdno { get; set; }
        public string c_noteno { get; set; }
        public string d_date { get; set; }
        public string c_bank { get; set; }
    }

    internal class Temporary_User
    {
        public decimal No { get; set; }    
        public string Nip { get; set; }
        public string Nama { get; set; }
        public string Tgl_Lahir { get; set; }
        public string Gudang { get; set; }
        public string Aktif { get; set; }
        public string nipEntry { get; set; }
    }
    //Indra 20170714
    internal class Temporary_ResendDOBatch
    {
        public decimal No { get; set; }
        public string NO_DO { get; set; }
    }

    internal class Temporary_SO
    {
        public string Team { get; set; }
        public string C_nosup { get; set; }
        public string Principal { get; set; }
        public string C_iteno { get; set; }
        public string C_itnam { get; set; }
        public decimal Qty { get; set; }
        public string Batch { get; set; }
        public string Ed { get; set; }
    }

    internal class cabang
    {
      public string NamaCabang { get; set; }
      public string KdCabang { get; set; } 
    }

    #endregion

    #region Structures

    private struct StructureReturValidate
    {
      public string TaxNo;
      public string Item;
      public string ExFaktur;
      public decimal Price;
      public decimal Disc;
    }

    #endregion

    public static IDictionary<string, object> ModelUploadedQuery(string connectionString, string model, string originalName, byte[] data, IDictionary<string, Functionals.ParameterParser> parameters)
    {
      IDictionary<string, object> dic = new Dictionary<string, object>();

      ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);
      //db.CommandTimeout = 1000;
      MemoryStream memStream = null;

      IExcelDataReader excelReader = null;
      DataSet dataSet = null;
      DataTable table = null;
      int nLoop = 0, nLen = 0, 
        nLoopC = 0, nLenC = 0;
      DataRow row = null;
      List<string> listTmp = null,
        listTmp1 = null;
      string tmp = null;
      DateTime dateTmp = DateTime.MinValue;
      TimeSpan timeTmp = TimeSpan.MinValue;
      int nCount = 0;
      string nipEntry = null;
      string NIPEntry = null;

      bool isValidate = false;

      CommonQuerySP cqSP = null;

      #region Sample Code

      //FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);

      ////1. Reading from a binary Excel file ('97-2003 format; *.xls)
      //IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
      ////...
      ////2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
      //IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
      ////...
      ////3. DataSet - The result of each spreadsheet will be created in the result.Tables
      //DataSet result = excelReader.AsDataSet();
      ////...
      ////4. DataSet - Create column names from first row
      //excelReader.IsFirstRowAsColumnNames = true;
      //DataSet result = excelReader.AsDataSet();

      ////5. Data Reader methods
      //while (excelReader.Read())
      //{
      //  //excelReader.GetInt32(0);
      //}

      ////6. Free resources (IExcelDataReader is IDisposable)
      //excelReader.Close();

      #endregion

      try
      {
        switch (model)
        {
          #region MODEL_COMMON_UPLOADED_QUERY_IMPORTRS

          case Constant.MODEL_COMMON_UPLOADED_QUERY_IMPORTRS:
            {
              string tipeRetur = (parameters.ContainsKey("tipeRetur") ? (string)((Functionals.ParameterParser)parameters["tipeRetur"]).Value : string.Empty);
              nipEntry = (parameters.ContainsKey("nipEntry") ? (string)((Functionals.ParameterParser)parameters["nipEntry"]).Value : string.Empty);
              isValidate = (parameters.ContainsKey("isValidate") ? Convert.ToBoolean(((Functionals.ParameterParser)parameters["isValidate"]).Value) : false);

              List<UploadImportRS> lstUIRS = null;
              List<UploadImportRS> lstUIRSInvalid = null;
              List<Temporary_ImportRS> lstResult = null;
              List<Temporary_ImportRS> lstResultNew = null;
              List<StructureReturValidate> lstValRetur = null;
              IDictionary<string, TempItemList> dicItemList = new Dictionary<string, TempItemList>(StringComparer.OrdinalIgnoreCase);

              List<Temporary_ImportRS> lstResultNotMatch = null;

              StructureReturValidate srv = default(StructureReturValidate);
              Temporary_ImportRS tirs = null;
              UploadImportRS uirs = null;

              memStream = new MemoryStream(data);

              excelReader = ExcelReaderFactory.CreateBinaryReader(memStream);
              excelReader.IsFirstRowAsColumnNames = true;
              dataSet = excelReader.AsDataSet();

              table = (dataSet.Tables.Contains("ImportRS") ? dataSet.Tables["ImportRS"] : null);

              // Gudang	RsNo	Item	Batch	Salpri	Disc	Gqty	Bqty	Ex.Faktur	Tax	TaxDate

              if (table == null)
              {
                dic.Add(Constant.DEFAULT_NAMING_EXCEPTION, "Sheet 'ImportRS' tidak ditemukan.");

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);

                break;
              }

              lstUIRS = new List<UploadImportRS>();
              listTmp = new List<string>();
              lstValRetur = new List<StructureReturValidate>();
              lstResultNotMatch = new List<Temporary_ImportRS>();

              for (nLoop = 0, nLen = table.Rows.Count; nLoop < nLen; nLoop++)
              {
                row = table.Rows[nLoop];

                uirs = new UploadImportRS()
                {
                  RsNo = (row.IsNull("RsNo") ? string.Empty : row.GetValue<string>("RsNo", string.Empty).Trim()),
                  Item = (row.IsNull("Item") ? string.Empty : row.GetValue<string>("Item", string.Empty).Trim()),
                  Batch = (row.IsNull("Batch") ? string.Empty : row.GetValue<string>("Batch", string.Empty).Trim()),
                  Salpri = (row.IsNull("Salpri") ? 0 : row.GetValue<decimal>("Salpri")),
                  Disc = (row.IsNull("Disc") ? 0 : row.GetValue<decimal>("Disc")),
                  Gqty = (row.IsNull("Gqty") ? 0 : row.GetValue<decimal>("Gqty")),
                  Bqty = (row.IsNull("Bqty") ? 0 : row.GetValue<decimal>("Bqty")),
                  Ex_Faktur = (row.IsNull("Ex.Faktur") ? string.Empty : row.GetValue<string>("Ex.Faktur", string.Empty).Trim()),
                  Tax = (row.IsNull("Tax") ? string.Empty : row.GetValue<string>("Tax", string.Empty).Trim()),
                };

                tmp = (row.IsNull("TaxDate") ? string.Empty : row.GetValue<string>("TaxDate", string.Empty));
                if (!Functionals.DateParser(tmp, "yyyy-M-d", out dateTmp))
                {
                  dateTmp = DateTime.MinValue;
                }
                uirs.TaxDate = dateTmp;

                tmp = (row.IsNull("Gudang") ? string.Empty : row.GetValue<string>("Gudang", string.Empty));
                uirs.Gudang = (string.IsNullOrEmpty(tmp) ? char.MinValue : tmp[0]);

                if (!string.IsNullOrEmpty(uirs.RsNo))
                {
                  if (!listTmp.Contains(uirs.RsNo))
                  {
                    listTmp.Add(uirs.RsNo);
                  }

                  lstUIRS.Add(uirs);
                }
              }

              #region Check Invalid Hna, Disc, Ex.Faktur

              lstValRetur = lstUIRS.GroupBy(x => new
              {
                x.Tax,
                x.Item,
                x.Ex_Faktur,
                x.Salpri,
                x.Disc
              }).Select(
                y => new StructureReturValidate()
                {
                  TaxNo = y.Key.Tax,
                  Item = y.Key.Item,
                  ExFaktur = y.Key.Ex_Faktur,
                  Price = y.Key.Salpri,
                  Disc = y.Key.Disc
                }).ToList();

              for (nLoop = 0, nLen = lstValRetur.Count; nLoop < nLen; nLoop++)
              {
                srv = lstValRetur[nLoop];

                nCount = lstValRetur.Where(x => x.TaxNo == srv.TaxNo && x.Item == srv.Item).Count();
                
                if (nCount > 1)
                {
                  lstUIRSInvalid = lstUIRS.Where(x => x.Tax == srv.TaxNo && x.Item == srv.Item).ToList();

                  if ((lstUIRSInvalid != null) && (lstUIRSInvalid.Count > 0))
                  {
                    for (nLoopC = 0, nLenC = lstUIRSInvalid.Count; nLoopC < nLenC; nLoopC++)
                    {
                      uirs = lstUIRSInvalid[nLoopC];
                      
                      if (!dicItemList.ContainsKey(uirs.Item))
                      {
                        tmp = (from q in db.FA_MasItms where q.c_iteno == uirs.Item select q.v_itnam).SingleOrDefault();

                        dicItemList.Add(uirs.Item, new TempItemList()
                        {
                          Item = uirs.Item,
                          ItemName = tmp
                        });
                      }
                      else
                      {
                        tmp = dicItemList[uirs.Item].ItemName;
                      }

                      lstResultNotMatch.Add(new Temporary_ImportRS()
                      {
                        c_rsno = uirs.RsNo,
                        c_iteno = uirs.Item,
                        c_batch = uirs.Batch,
                        c_up_exfaktur = uirs.Ex_Faktur,
                        c_up_taxno = srv.TaxNo,
                        d_up_taxdate = uirs.TaxDate,
                        n_up_disc = uirs.Disc,
                        n_up_salpri = uirs.Salpri,
                        n_up_bsisa = uirs.Bqty,
                        n_up_gsisa = uirs.Gqty,
                        v_itnam = tmp,
                        v_ket = "Exclude : Terjadi multi data pada ex.faktur atau hna atau discount."
                      });

                      lstUIRS.Remove(uirs);
                    }

                    lstUIRSInvalid.Clear();
                  }
                }
              }

              if (lstUIRS.Count < 1)
              {
                goto justOutResult;
              }

              #endregion

              #region Old Coded

              //listTmp = lstUIRS.GroupBy(x => x.Tax).Select(y => y.Key).ToList();

              //if ((listTmp != null) && (listTmp.Count > 0))
              //{
              //  //nTmp1 = lstUIRS.GroupBy(x => 
              //  //  new { x.Item, 
              //  //    x.Batch,                    
              //  //    x.Ex_Faktur 
              //  //  }).Where(y => listTmp.Contains(y.Key.

              //  //lstUIRS.GroupBy(x => new
              //  //{
              //  //  x.Tax,
              //  //  x.Item,
              //  //  x.Batch,
              //  //  x.Ex_Faktur
              //  //}).Where(y => listTmp.Contains(y.Key.Tax)).Select(z =>
              //}

              //listTmp = lstUIRS.GroupBy(x => x.RsNo).Select(y => y.Key).ToList();

              #endregion

              #region Old Coded

              //lstResult = (from q in db.LG_RSHes
              //             join q1 in db.LG_RSD1s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
              //             join q2 in db.LG_RSD2s on new { q.c_gdg, q.c_rsno, q1.c_iteno, q1.c_batch } equals new { q2.c_gdg, q2.c_rsno, q2.c_iteno, q2.c_batch }
              //             join q3 in db.LG_DatSups on q.c_nosup equals q3.c_nosup
              //             join q4 in db.FA_MasItms on q1.c_iteno equals q4.c_iteno
              //             join q5 in db.LG_RSD3s on new { q.c_gdg, q.c_rsno, q1.c_iteno, q1.c_batch } equals new { c_gdg = (char)q5.c_gdg, q5.c_rsno, q5.c_iteno, q5.c_batch } into q_5
              //             from qRSD3 in q_5.DefaultIfEmpty()
              //             join q6 in db.LG_FBD2s on q2.c_rnno equals q6.c_rnno into q_6
              //             from qFBD2 in q_6.DefaultIfEmpty()
              //             join q7 in db.LG_FBHs on qFBD2.c_fbno equals q7.c_fbno into q_7
              //             from qFBH in q_7.DefaultIfEmpty()
              //             join q8 in db.LG_FBD1s on new { q1.c_iteno, qFBD2.c_fbno } equals new { q8.c_iteno, q8.c_fbno } into q_8
              //             from qFBD1 in q_8.DefaultIfEmpty()
              //             where listTmp.Contains(q.c_rsno) && (q.c_type == tipeRetur)
              //               && (qRSD3.c_rsno == null)
              //             group new { q, q1, q2, qFBH, q4 } by
              //               new
              //               {
              //                 q.c_gdg,
              //                 q.c_rsno,
              //                 q1.c_iteno,
              //                 q1.c_batch,
              //                 qFBH.c_fb,
              //                 qFBH.c_taxno,
              //                 qFBH.d_taxdate,
              //                 qFBD1.n_salpri,
              //                 n_salpri_Itm = q4.n_salpri,
              //                 qFBD1.n_disc,
              //                 n_disc_Itm = q4.n_disc,
              //                 qFBD1.n_bea,
              //                 q4.v_itnam
              //               } into g
              //             select new Temporary_ImportRS()
              //             {
              //               c_gdg = g.Key.c_gdg,
              //               c_rsno = g.Key.c_rsno.Trim(),
              //               c_iteno = g.Key.c_iteno.Trim(),
              //               v_itnam = g.Key.v_itnam.Trim(),
              //               c_batch = g.Key.c_batch.Trim(),
              //               c_fb = (string.IsNullOrEmpty(g.Key.c_fb) ? "" : g.Key.c_fb.Trim()),
              //               c_taxno = (string.IsNullOrEmpty(g.Key.c_taxno) ? "" : g.Key.c_taxno.Trim()),
              //               d_taxdate = (g.Key.d_taxdate.HasValue ? g.Key.d_taxdate.Value : Functionals.StandardSqlDateTime),
              //               n_salpri = (g.Key.n_salpri.HasValue ?
              //                     g.Key.n_salpri.Value :
              //                     (g.Key.n_salpri_Itm.HasValue ?
              //                      g.Key.n_salpri_Itm.Value : 0)),
              //               n_disc = (g.Key.n_disc.HasValue ?
              //                     g.Key.n_disc.Value :
              //                     (g.Key.n_disc_Itm.HasValue ?
              //                      g.Key.n_disc_Itm.Value : 0)),
              //               n_gsisa = (g.Sum(x => x.q2.n_gsisa).HasValue ? g.Sum(x => x.q2.n_gsisa).Value : 0),
              //               n_bsisa = (g.Sum(x => x.q2.n_bsisa).HasValue ? g.Sum(x => x.q2.n_bsisa).Value : 0),
              //             }).Distinct().ToList();

              #endregion

              #region Old Coded

              //lstResult = (from q in db.LG_RSHes
              //             join q1 in db.LG_RSD2s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
              //             join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
              //             join q3 in db.LG_FBD2s on q1.c_rnno equals q3.c_rnno into q_3
              //             from qFBD2 in q_3.DefaultIfEmpty()
              //             join q4 in db.LG_FBD1s on new { qFBD2.c_fbno, q1.c_iteno, c_type = "01" } equals new { q4.c_fbno, q4.c_iteno, q4.c_type } into q_4
              //             from qFBD1 in q_4.DefaultIfEmpty()
              //             join q7 in db.LG_FBHs on qFBD2.c_fbno equals q7.c_fbno into q_7
              //             from qFBH in q_7.DefaultIfEmpty()
              //             where listTmp.Contains(q.c_rsno) && (q.c_type == tipeRetur)
              //              && (q1.l_status == false)
              //             group new { q1 } by new { q1.c_gdg, q1.c_rsno, q1.c_iteno, q1.c_batch, q2.v_itnam, qFBH.c_fb, qFBH.c_taxno, qFBH.d_taxdate, qFBD1.n_salpri, qFBD1.n_disc } into g
              //             select new Temporary_ImportRS()
              //             {
              //               c_gdg = g.Key.c_gdg,
              //               c_rsno = g.Key.c_rsno.Trim(),
              //               c_iteno = g.Key.c_iteno.Trim(),
              //               c_batch = g.Key.c_batch.Trim(),
              //               n_bsisa = g.Sum(x => (x.q1.n_bsisa.HasValue ? x.q1.n_bsisa.Value : 0)),
              //               n_gsisa = g.Sum(x => (x.q1.n_gsisa.HasValue ? x.q1.n_gsisa.Value : 0)),
              //               v_itnam = g.Key.v_itnam.Trim(),
              //               c_fb = g.Key.c_fb,
              //               c_taxno = g.Key.c_taxno,
              //               d_taxdate = (g.Key.d_taxdate.HasValue ? g.Key.d_taxdate.Value : new DateTime(1900, 1, 1)),
              //               n_salpri = (g.Key.n_salpri.HasValue ? g.Key.n_salpri.Value : 0),
              //               n_disc = (g.Key.n_disc.HasValue ? g.Key.n_disc.Value : 0)
              //             }).Distinct().ToList();

              #endregion

              if (listTmp.Count > Constant.MAX_QUERY_LIST_SIZE)
              {
                listTmp.RemoveRange(0, Constant.MAX_QUERY_LIST_SIZE);
              }

              lstResult = (from q in db.LG_RSHes
                           join q1 in db.LG_RSD2s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                           join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
                           where listTmp.Contains(q.c_rsno) && (q.c_type == tipeRetur)
                            && (q1.l_status == false)
                           group new { q1 } by new { q1.c_gdg, q1.c_rsno, q1.c_iteno, q1.c_batch, q2.v_itnam} into g
                           select new Temporary_ImportRS()
                           {
                             c_gdg = g.Key.c_gdg,
                             c_rsno = g.Key.c_rsno.Trim(),
                             c_iteno = g.Key.c_iteno.Trim(),
                             c_batch = g.Key.c_batch.Trim(),
                             n_bsisa = g.Sum(x => (x.q1.n_bsisa.HasValue ? x.q1.n_bsisa.Value : 0)),
                             n_gsisa = g.Sum(x => (x.q1.n_gsisa.HasValue ? x.q1.n_gsisa.Value : 0)),
                             v_itnam = g.Key.v_itnam.Trim()
                           }).Distinct().ToList();

              listTmp.Clear();

              #region Old Coded

              //for (nLoop = 0, nLen = lstUIRS.Count;nLoop < nLen ;nLoop++)
              //{
              //  uirs = lstUIRS[nLoop];

              //  tirs = lstResult.Find(delegate(Temporary_ImportRS t)
              //  {
              //    return (
              //      t.c_gdg.Equals(uirs.Gudang) &&
              //      t.c_rsno.Equals(uirs.RsNo) &&
              //      t.c_iteno.Equals(uirs.Item) &&
              //      t.c_batch.Equals(uirs.Batch) &&
              //      t.n_bsisa == uirs.Bqty &&
              //      t.n_gsisa == uirs.Gqty
              //      );
              //  });

              //  if (tirs == null)
              //  {
              //    if (!dicItemList.ContainsKey(uirs.Item))
              //    {
              //      tmp = (from q in db.FA_MasItms where q.c_iteno == uirs.Item select q.v_itnam).SingleOrDefault();

              //      dicItemList.Add(uirs.Item, new TempItemList()
              //      {
              //        Item = uirs.Item,
              //        ItemName = tmp
              //      });
              //    }
              //    else
              //    {
              //      tmp = dicItemList[uirs.Item].ItemName;
              //    }

              //    tirs = new Temporary_ImportRS()
              //    {
              //      c_gdg = uirs.Gudang,
              //      c_rsno = uirs.RsNo,
              //      c_iteno = uirs.Item,
              //      v_itnam = tmp,
              //      c_batch = uirs.Batch,
              //      c_up_exfaktur = uirs.Ex_Faktur,
              //      c_up_taxno = uirs.Tax,
              //      d_up_taxdate = uirs.TaxDate,
              //      n_up_bsisa = uirs.Bqty,
              //      n_up_gsisa = uirs.Gqty,
              //      n_up_disc = uirs.Disc,
              //      n_up_salpri = uirs.Salpri
              //    };
                  
              //    lstResult.Add(tirs);
              //  }
              //  else
              //  {
              //    tirs.c_up_exfaktur = uirs.Ex_Faktur;
              //    tirs.c_up_taxno = uirs.Tax;
              //    tirs.d_up_taxdate = uirs.TaxDate;
              //    tirs.n_up_bsisa = uirs.Bqty;
              //    tirs.n_up_disc = uirs.Disc;
              //    tirs.n_up_gsisa = uirs.Gqty;
              //    tirs.n_up_salpri = uirs.Salpri;

              //    //lstResult[nLoop] = tirs;
              //    lstResult.Remove(tirs);

              //    lstResult.Add(tirs);
              //  }
              //}

              #endregion

              lstResultNew = new List<Temporary_ImportRS>();

              #region Populate Result

              for (nLoop = 0, nLen = lstUIRS.Count; nLoop < nLen; nLoop++)
              {
                uirs = lstUIRS[nLoop];

                tirs = lstResult.Find(delegate(Temporary_ImportRS t)
                {
                  return (
                    t.c_gdg.Equals(uirs.Gudang) &&
                    t.c_rsno.Equals(uirs.RsNo.Trim(), StringComparison.OrdinalIgnoreCase) &&
                    t.c_iteno.Equals(uirs.Item.Trim(), StringComparison.OrdinalIgnoreCase) &&
                    t.c_batch.Equals(uirs.Batch.Trim(), StringComparison.OrdinalIgnoreCase) //&& t.c_taxno.Equals(uirs.Ex_Faktur.Trim(), StringComparison.OrdinalIgnoreCase)
                    //(t.n_bsisa == uirs.Bqty) &&
                    //(t.n_gsisa == uirs.Gqty)
                    );
                });
                
                if (tirs == null)
                {
                  if (!dicItemList.ContainsKey(uirs.Item))
                  {
                    tmp = (from q in db.FA_MasItms where q.c_iteno == uirs.Item select q.v_itnam).SingleOrDefault();

                    dicItemList.Add(uirs.Item, new TempItemList()
                    {
                      Item = uirs.Item,
                      ItemName = tmp
                    });
                  }
                  else
                  {
                    tmp = dicItemList[uirs.Item].ItemName;
                  }

                  lstResultNotMatch.Add(new Temporary_ImportRS()
                  {
                    c_gdg = uirs.Gudang,
                    c_rsno = uirs.RsNo,
                    c_iteno = uirs.Item,
                    v_itnam = tmp,
                    c_batch = uirs.Batch,
                    n_up_bsisa = uirs.Bqty,
                    n_up_gsisa = uirs.Gqty,
                    c_up_exfaktur = uirs.Ex_Faktur,
                    c_up_taxno = uirs.Tax,
                    d_up_taxdate = uirs.TaxDate,
                    n_up_salpri = uirs.Salpri,
                    v_ket = "Exclude : Data tidak ditemukan."
                  });
                }
                else
                {
                  if ((!dicItemList.ContainsKey(tirs.c_iteno)) && (!string.IsNullOrEmpty(tirs.v_itnam)))
                  {
                    dicItemList.Add(tirs.c_iteno, new TempItemList()
                    {
                      Item = tirs.c_iteno,
                      ItemName = tirs.v_itnam
                    });
                  }

                  lstResult.Remove(tirs);

                  #region Old Coded

                  //if ((tirs.n_bsisa != uirs.Bqty) &&
                  //  (tirs.n_gsisa != uirs.Gqty))
                  //{
                  //  if (!dicItemList.ContainsKey(uirs.Item))
                  //  {
                  //    tmp = (from q in db.FA_MasItms where q.c_iteno == uirs.Item select q.v_itnam).SingleOrDefault();

                  //    dicItemList.Add(uirs.Item, new TempItemList()
                  //    {
                  //      Item = uirs.Item,
                  //      ItemName = tmp
                  //    });
                  //  }
                  //  else
                  //  {
                  //    tmp = dicItemList[uirs.Item].ItemName;
                  //  }

                  //  lstResultNotMatch.Add(new Temporary_ImportRS()
                  //  {
                  //    c_gdg = uirs.Gudang,
                  //    c_rsno = uirs.RsNo,
                  //    c_iteno = uirs.Item,
                  //    v_itnam = tmp,
                  //    c_batch = uirs.Batch,
                  //    n_up_bsisa = uirs.Bqty,
                  //    n_up_gsisa = uirs.Gqty,
                  //    c_up_exfaktur = uirs.Ex_Faktur,
                  //    c_up_taxno = uirs.Tax,
                  //    d_up_taxdate = uirs.TaxDate,
                  //    n_up_salpri = uirs.Salpri,
                  //    v_ket = "Quantity Good/Bad tidak sama."
                  //  });
                  //}

                  #endregion

                  tirs.n_bsisa -= uirs.Bqty;
                  tirs.n_gsisa -= uirs.Gqty;

                  if ((tirs.n_bsisa < 0) || (tirs.n_gsisa < 0))
                  {
                    if (!dicItemList.ContainsKey(uirs.Item))
                    {
                      tmp = (from q in db.FA_MasItms where q.c_iteno == uirs.Item select q.v_itnam).SingleOrDefault();

                      dicItemList.Add(uirs.Item, new TempItemList()
                      {
                        Item = uirs.Item,
                        ItemName = tmp
                      });
                    }
                    else
                    {
                      tmp = dicItemList[uirs.Item].ItemName;
                    }

                    lstResultNotMatch.Add(new Temporary_ImportRS()
                    {
                      c_gdg = uirs.Gudang,
                      c_rsno = uirs.RsNo,
                      c_iteno = uirs.Item,
                      v_itnam = tmp,
                      c_batch = uirs.Batch,
                      n_bsisa = tirs.n_bsisa,
                      n_gsisa = tirs.n_gsisa,
                      n_up_bsisa = uirs.Bqty,
                      n_up_gsisa = uirs.Gqty,
                      c_up_exfaktur = uirs.Ex_Faktur,
                      c_up_taxno = uirs.Tax,
                      d_up_taxdate = uirs.TaxDate,
                      n_up_salpri = uirs.Salpri,
                      v_ket = "Quantity Good/Bad tidak sama."
                    });
                  }

                  if ((tirs.n_bsisa > 0) || (tirs.n_gsisa > 0))
                  {
                    lstResult.Add(tirs);
                  }

                  tirs.c_up_exfaktur = uirs.Ex_Faktur;
                  tirs.c_up_taxno = uirs.Tax;
                  tirs.d_up_taxdate = uirs.TaxDate;
                  tirs.n_up_bsisa = uirs.Bqty;
                  tirs.n_up_disc = uirs.Disc;
                  tirs.n_up_gsisa = uirs.Gqty;
                  tirs.n_up_salpri = uirs.Salpri;

                  lstResultNew.Add(tirs);
                }
              }

              #endregion

              #region Populate Missing Result

              if ((lstResult != null) && (lstResult.Count > 0))
              {
                for (nLoop = 0, nLen = lstResult.Count; nLoop < nLen; nLoop++)
                {
                  tirs = lstResult[nLoop];

                  if (string.IsNullOrEmpty(tirs.v_itnam))
                  {
                    if (!dicItemList.ContainsKey(tirs.c_iteno))
                    {
                      tmp = (from q in db.FA_MasItms where q.c_iteno == tirs.c_iteno select q.v_itnam).SingleOrDefault();

                      dicItemList.Add(uirs.Item, new TempItemList()
                      {
                        Item = uirs.Item,
                        ItemName = tmp
                      });
                    }
                    else
                    {
                      tmp = dicItemList[tirs.c_iteno].ItemName;
                    }

                    tirs.v_itnam = tmp;                    
                  }
                  
                  tirs.v_ket = "Exclude : Quantity sisa.";
                }

                lstResultNotMatch.AddRange(lstResult.ToArray());
              }

              #endregion

              dicItemList.Clear();
              lstResult.Clear();

              if ((lstResultNew.Count > 0) && (!isValidate))
              {
                //ScmsSoaLibrary.Bussiness.Commons.RunningGenerateRS(db, nipEntry, true,
                //  lstResultNew.ToArray());

                cqSP = new CommonQuerySP();

                cqSP.SP_LG_CalcRS(db.Connection.ConnectionString, nipEntry, true, lstResultNew.ToArray());

                lstResultNew.Clear();
              }

            justOutResult:
              nCount = lstResultNotMatch.Count;

              if (nCount > 0)
              {
                dic.Add(Constant.DEFAULT_NAMING_RECORDS, lstResultNotMatch.ToArray());

                lstResultNotMatch.Clear();
              }

              dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

              dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
            }
            break;

          #endregion

          #region MODEL_COMMON_UPLOADED_QUERY_IMPORTFB

          case Constant.MODEL_COMMON_UPLOADED_QUERY_IMPORTFB:
            {
              nipEntry = (parameters.ContainsKey("nipEntry") ? (string)((Functionals.ParameterParser)parameters["nipEntry"]).Value : string.Empty);
              isValidate = (parameters.ContainsKey("isValidate") ? Convert.ToBoolean(((Functionals.ParameterParser)parameters["isValidate"]).Value) : false);

              List<UploadImportFB> lstUIFB = null;
              List<Temporary_ImportFB> lstResultNotMatch = null;
              listTmp = new List<string>();
              listTmp1 = new List<string>();
              lstResultNotMatch = new List<Temporary_ImportFB>();
              UploadImportFB uifb = null;

              memStream = new MemoryStream(data);

              excelReader = ExcelReaderFactory.CreateBinaryReader(memStream);
              excelReader.IsFirstRowAsColumnNames = true;
              dataSet = excelReader.AsDataSet();

              table = (dataSet.Tables.Contains("ImportFB") ? dataSet.Tables["ImportFB"] : null);

              if (table == null)
              {
                dic.Add(Constant.DEFAULT_NAMING_EXCEPTION, "Sheet 'ImportFB' tidak ditemukan.");

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);

                break;
              }

              lstUIFB = new List<UploadImportFB>();

              for (nLoop = 0, nLen = table.Rows.Count; nLoop < nLen; nLoop++)
              {
                row = table.Rows[nLoop];

                uifb = new UploadImportFB()
                {
                  Gudang = char.MinValue,
                  Supplier = row.GetValue<string>("Supplier", string.Empty),
                  RnNo = row.GetValue<string>("RnNo", string.Empty),
                  DoNo = row.GetValue<string>("DoNo", string.Empty),
                  Faktur = row.GetValue<string>("Faktur", string.Empty),
                  FbDate = Functionals.StandardSqlDateTime,
                  Top = row.GetValue<int>("Top"),
                  TopPjg = row.GetValue<int>("TopPjg"),
                  Tax = row.GetValue<string>("Tax", string.Empty),
                  TaxDate = Functionals.StandardSqlDateTime,
                  Bea = row.GetValue<decimal>("Bea"),
                  XDisc = row.GetValue<decimal>("XDisc"),
                  ValueFaktur = row.GetValue<decimal>("ValueFaktur"),
                  n_ppph = row.GetValue<decimal>("n_ppph"),
                };
                
                tmp = row.GetValue<string>("FbDate", string.Empty);
                if (Functionals.DateParser(tmp, "yyyy-M-d", out dateTmp))
                {
                  uifb.FbDate = dateTmp;
                }
                tmp = row.GetValue<string>("TaxDate", string.Empty);
                if (Functionals.DateParser(tmp, "yyyy-M-d", out dateTmp))
                {
                  uifb.TaxDate = dateTmp;
                }
                tmp = row.GetValue<string>("Gudang", string.Empty);
                uifb.Gudang = (string.IsNullOrEmpty(tmp) ? char.MinValue : tmp[0]);

                if ((!string.IsNullOrEmpty(uifb.RnNo)) && (!string.IsNullOrEmpty(uifb.DoNo)))
                {
                  if(listTmp.Contains(uifb.RnNo) && listTmp1.Contains(uifb.DoNo))
                  {
                    lstResultNotMatch.Add(new Temporary_ImportFB()
                    {
                      Gudang = uifb.Gudang,
                      RnNo = uifb.RnNo,
                      DoNo = uifb.DoNo,
                      Supplier = uifb.Supplier,
                      Keterangan = "Exclude : Duplikat Rn dan Do"
                    });
                  }
                  else if (listTmp.Contains(uifb.RnNo))
                  {
                    lstResultNotMatch.Add(new Temporary_ImportFB()
                    {
                      Gudang = uifb.Gudang,
                      RnNo = uifb.RnNo,
                      DoNo = uifb.DoNo,
                      Supplier = uifb.Supplier,
                      Keterangan = "Exclude : Duplikat Rn"
                    });
                  }
                  else if (string.IsNullOrEmpty(uifb.RnNo) && listTmp1.Contains(uifb.DoNo))
                  {
                    lstResultNotMatch.Add(new Temporary_ImportFB()
                    {
                      Gudang = uifb.Gudang,
                      RnNo = uifb.RnNo,
                      DoNo = uifb.DoNo,
                      Supplier = uifb.Supplier,
                      Keterangan = "Exclude : Duplikat Do"
                    });
                  }

                  if ((!listTmp.Contains(uifb.RnNo)) || (!listTmp1.Contains(uifb.DoNo)))
                  {
                    if (!string.IsNullOrEmpty(uifb.RnNo))
                    {
                      listTmp.Add(uifb.RnNo);
                    }
                    if (!string.IsNullOrEmpty(uifb.DoNo))
                    {
                      listTmp1.Add(uifb.DoNo);
                    }

                    lstUIFB.Add(uifb);
                  }
                  else
                  {
                    lstResultNotMatch.Add(new Temporary_ImportFB()
                    {
                      Gudang = uifb.Gudang,
                      RnNo = uifb.RnNo,
                      DoNo = uifb.DoNo,
                      Supplier = uifb.Supplier,
                      Keterangan = "Exclude : Duplikat Rn atau Do"
                    });
                  }
                }
                else
                {
                  lstResultNotMatch.Add(new Temporary_ImportFB()
                  {
                    Gudang = uifb.Gudang,
                    RnNo = uifb.RnNo,
                    DoNo = uifb.DoNo,
                    Supplier = uifb.Supplier,
                    Keterangan = "Exclude : Rn atau Do kosong"
                  });
                }
              }

              if (lstUIFB.Count < 1)
              {
                goto justOutResult;
              }

              if ((lstUIFB.Count > 0) && (!isValidate))
              {
                cqSP = new CommonQuerySP();

                cqSP.SP_LG_CalcFB(db.Connection.ConnectionString, nipEntry, true, lstUIFB.ToArray());

                lstUIFB.Clear();
              }

            justOutResult:
              nCount = lstResultNotMatch.Count;

              if (nCount > 0)
              {
                dic.Add(Constant.DEFAULT_NAMING_RECORDS, lstResultNotMatch.ToArray());

                lstResultNotMatch.Clear();
              }

              dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

              dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
            }
            break;

          #endregion

          #region MODEL_COMMON_UPLOADED_QUERY_IMPORTRS_EXPEDISI

          case Constant.MODEL_COMMON_UPLOADED_QUERY_IMPORTRS_EXPEDISI:
            {
              nipEntry = (parameters.ContainsKey("nipEntry") ? (string)((Functionals.ParameterParser)parameters["nipEntry"]).Value : string.Empty);
              isValidate = (parameters.ContainsKey("isValidate") ? Convert.ToBoolean(((Functionals.ParameterParser)parameters["isValidate"]).Value) : false);

              List<UploadImportExp> lstUIExp = null;

              //StructureReturValidate srv = default(StructureReturValidate);
              Temporary_ImportExp tiexp = null;
              UploadImportExp uiexp = null;

              //List<Temporary_ImportExp> lstResult = null;
              List<Temporary_ImportExp> lstResultNew = null;
              //List<StructureReturValidate> lstValRetur = null;

              List<Temporary_ImportExp> lstResultNotMatch = null;


              memStream = new MemoryStream(data);

              excelReader = ExcelReaderFactory.CreateBinaryReader(memStream);
              excelReader.IsFirstRowAsColumnNames = true;
              dataSet = excelReader.AsDataSet();

              table = (dataSet.Tables.Contains("ImportExp") ? dataSet.Tables["ImportExp"] : null);

              if (table == null)
              {
                dic.Add(Constant.DEFAULT_NAMING_EXCEPTION, "Sheet 'ImportRS' tidak ditemukan.");

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);

                break;
              }

              listTmp = new List<string>();
              lstUIExp = new List<UploadImportExp>();

              for (nLoop = 0, nLen = table.Rows.Count; nLoop < nLen; nLoop++)
              {
                row = table.Rows[nLoop];

                uiexp = new UploadImportExp()
                {
                  expno = (row.IsNull("expno") ? string.Empty : row.GetValue<string>("expno", string.Empty).Trim()),
                  resino = (row.IsNull("resino") ? string.Empty : row.GetValue<string>("resino", string.Empty).Trim()),
                  tglresi = (row.IsNull("tglresi") ? string.Empty : row.GetValue<string>("tglresi", string.Empty).Trim()),
                  tglexpcab = (row.IsNull("tglexpcab") ? string.Empty : row.GetValue<string>("tglexpcab", string.Empty).Trim()),
                  wktexpcab = (row.IsNull("wktexpcab") ? string.Empty : row.GetValue<string>("wktexpcab", string.Empty).Trim())
                };

                //tmp = (row.IsNull("TaxDate") ? string.Empty : row.GetValue<string>("TaxDate", string.Empty));
                CultureInfo culture = new CultureInfo("id-ID");
                
                //if (!Functionals.DateParser(tmp, "yyyy-MM-dd", out dateTmp))

                tmp = (row.IsNull("tglresi") ? string.Empty : row.GetValue<string>("tglresi", string.Empty));
                if (!Functionals.DateParser(DateTime.Parse(tmp).ToString("yyyy-MM-dd"), "yyyy-MM-dd", out dateTmp))
                {
                  dateTmp = DateTime.MinValue;
                }
                uiexp.Dtglresi = dateTmp;

                tmp = null;
                tmp = (row.IsNull("tglexpcab") ? string.Empty : row.GetValue<string>("tglexpcab", string.Empty));
                if (!DateTime.TryParse(tmp, out dateTmp))
                {
                  dateTmp = DateTime.MinValue;
                }
                uiexp.Dtglexpcab = dateTmp;

                tmp = null;
                tmp = (row.IsNull("wktexpcab") ? string.Empty : row.GetValue<string>("wktexpcab", string.Empty));
                if (!TimeSpan.TryParse(tmp, out timeTmp))
                {
                  timeTmp = TimeSpan.MinValue;
                }
                uiexp.Twktexpcab = timeTmp;

                
                if (!string.IsNullOrEmpty(uiexp.expno))
                {
                  if (!listTmp.Contains(uiexp.expno))
                  {
                    listTmp.Add(uiexp.expno);
                  }

                  lstUIExp.Add(uiexp);
                }
              }

              listTmp.Clear();

              lstResultNew = new List<Temporary_ImportExp>();
              lstResultNotMatch = new List<Temporary_ImportExp>();

              #region Cek

              for (nLoop = 0, nLen = lstUIExp.Count; nLoop < nLen; nLoop++)
              {
                uiexp = lstUIExp[nLoop];

                tiexp = (from q in db.LG_ExpHs
                         where q.c_expno == uiexp.expno && q.c_resi == uiexp.resino
                         select new Temporary_ImportExp()
                         {
                            customer = q.c_cusno,
                            expedisi = q.c_exp,
                            resino = q.c_resi,
                            expno = q.c_expno
                         }).SingleOrDefault();

                if ((tiexp == null))
                {
                  lstResultNotMatch.Add(new Temporary_ImportExp()
                  {
                    expno = uiexp.expno,
                    resino = uiexp.resino,
                    tglresi = uiexp.tglresi,
                    tglexpcab = uiexp.tglexpcab,
                    wktexpcab = uiexp.wktexpcab,
                    Dtglresi = uiexp.Dtglresi,
                    Dtglexpcab = uiexp.Dtglexpcab,
                    Twktexpcab = uiexp.Twktexpcab,
                    c_type = "02",
                    v_ket = "Data Tidak Di Temukan"
                  });
                }
                else
                {
                    tiexp.expno = uiexp.expno;
                    tiexp.resino = uiexp.resino;
                    tiexp.tglresi = uiexp.tglresi;
                    tiexp.tglexpcab = uiexp.tglexpcab;
                    tiexp.wktexpcab = uiexp.wktexpcab;
                    tiexp.Dtglresi = uiexp.Dtglresi;
                    tiexp.Dtglexpcab = uiexp.Dtglexpcab;
                    tiexp.c_type = "01";
                    tiexp.Twktexpcab = uiexp.Twktexpcab;

                    lstResultNew.Add(tiexp);
                }
              }

              #endregion

              nCount = lstResultNotMatch.Count;

              if (lstResultNew.Count > 0 && (!isValidate))
              {
                cqSP = new CommonQuerySP();
                cqSP.ExpedisiUpload(db.Connection.ConnectionString, nipEntry, true, lstResultNew.ToArray());

                lstResultNew.Clear();
              }

              if (nCount > 0)
              {
                dic.Add(Constant.DEFAULT_NAMING_RECORDS, lstResultNotMatch.ToArray());

                lstResultNew.Clear();
              }

              dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

              dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
            }
            break;

          #endregion

          #region MODEL_COMMON_UPLOADED_QUERY_ZIPDBF

          case Constant.MODEL_COMMON_UPLOADED_QUERY_ZIPDBF:
            {

              isValidate = (parameters.ContainsKey("isValidate") ? Convert.ToBoolean(((Functionals.ParameterParser)parameters["isValidate"]).Value) : false);
              List<UploadSP> lstSP = null;

              Temporary_SP tisp = null;
              UploadSP uiesp = null;

              Config cf = new Config();
               

              List<Temporary_SP> lstResultNew = null;

              List<Temporary_SP> lstResultNotMatch = null;
                
              string path = string.Concat(cf.PathTempExtractMail,"Upload\\");
              //FileStream fileStream = System.IO.File.Create(path, (int)data.Length);
              memStream = new MemoryStream(data);
              //memStream.Read(data, 0, (int)data.Length);
              //fileStream.Write(data, 0, data.Length);

              ICSharpCode.SharpZipLib.Zip.ZipFile zf = new ICSharpCode.SharpZipLib.Zip.ZipFile(memStream);
              String fullZipToPath = null;
              string directoryName = null;
              foreach (ICSharpCode.SharpZipLib.Zip.ZipEntry ze in zf)
              {
                String entryFileName = ze.Name;

                byte[] buffer = new byte[4096];
                Stream zipStream = zf.GetInputStream(ze);

                fullZipToPath = Path.Combine(path, entryFileName);
                directoryName = Path.GetDirectoryName(fullZipToPath);
                if (directoryName.Length > 0)
                  Directory.CreateDirectory(directoryName);
                string result = Path.GetFileName(fullZipToPath);
                using (FileStream streamWriter = File.Create(fullZipToPath))
                {
                  ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(zipStream, streamWriter, buffer);
                }
                
              }

              zf.IsStreamOwner = true;
              zf.Close();
              DirectoryInfo di = new DirectoryInfo(directoryName);
              FileInfo[] fo = di.GetFiles("*", SearchOption.AllDirectories);
              dataSet = new DataSet();
              DataTable dt = null;
              foreach (FileInfo d in fo)
              {
                dt = new DataTable();
                dt = ReadDbfDatabase(d.FullName, d.Name);
                dt.TableName = d.Name;
                dataSet.Tables.Add(dt);
              }
              
              cqSP = new CommonQuerySP();

              if (!isValidate)
              {
                bool isSave = cqSP.PostingSPZip(db.Connection.ConnectionString, dataSet, false);
              }
              else
              {
                lstResultNew = new List<Temporary_SP>();
                for (nLoopC = 0; nLoopC < dataSet.Tables.Count; nLoopC++)
                {
                  table = new DataTable();
                  table = dataSet.Tables[nLoopC];

                  for (nLoop = 0; nLoop < table.Rows.Count; nLoop++)
                  {
                    tisp = new Temporary_SP();
                    row = table.Rows[nLoop];

                    decimal nQty = 0;
                    string sTipe = null,
                      sCabCabang = null,
                      Item = null;

                    if (row.GetValue<string>("c_pareto").ToString().Substring(1, 1).Equals("M"))
                    {
                      nQty = row.GetValue<int>("n_adjust", 0);
                      sTipe = "02";
                    }
                    else
                    {
                      if (row.GetValue<int>("n_adjust", 0) < 0)
                        nQty = row.GetValue<int>("n_order", 0) + row.GetValue<int>("n_adjust", 0);
                      else
                        nQty = row.GetValue<int>("n_order", 0);

                      sTipe = "01";
                    }

                    sTipe = (sTipe == "01" ? "SP AUTO" : "SP MANUAL");

                    sCabCabang = row.GetValue<string>("c_kdcab");
                    Item = row.GetValue<string>("c_iteno", string.Empty);
                    cabang NamaCab = (from q in db.LG_Cusmas
                                      where q.c_cab == sCabCabang
                                      select new cabang()
                                      {
                                        NamaCabang = q.v_cunam,
                                        KdCabang = q.c_cusno
                                      }).Take(1).SingleOrDefault();

                    string sNamItm = (from q in db.FA_MasItms
                                      where q.c_iteno == Item
                                      select q.v_itnam).Take(1).SingleOrDefault();

                    tisp = new Temporary_SP()
                    {
                      c_sp = row.GetValue<string>("c_nosp"),
                      kdCab = row.GetValue<string>("c_kdcab"),
                      Cabang = NamaCab.NamaCabang,
                      kdCabSCMS = NamaCab.KdCabang,
                      TipeSP = sTipe,
                      kodeitem = Item,
                      namaItem = sNamItm,
                      Qty = nQty,
                      TglSP = row.GetValue<string>("d_tglsp", string.Empty)
                    };

                    lstResultNew.Add(tisp);
                  }

                }
              }

              di = new DirectoryInfo(path);

              nCount = lstResultNew.Count;

              if ((lstResultNew.Count > 0) && isValidate)
              {
                dic.Add(Constant.DEFAULT_NAMING_RECORDS, lstResultNew.ToArray());

                lstResultNew.Clear();
              }

              dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

              dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

            }
            break;

          #endregion

          #region MODEL_COMMON_UPLOADED_QUERY_VOUCHER_EXCELL

          case Constant.MODEL_COMMON_UPLOADED_QUERY_VOUCHER_EXCELL:
            {
                isValidate = (parameters.ContainsKey("isValidate") ? Convert.ToBoolean(((Functionals.ParameterParser)parameters["isValidate"]).Value) : false);
                Temporary_Voucher tiVoucher = null;
                
                Config cf = new Config();
                List<Temporary_Voucher> lstResultNew = null;
                string isSave = string.Empty;

                string path = string.Concat(cf.PathTempExtractMail, "Upload\\"+originalName);
                string directoryName = string.Concat(cf.PathTempExtractMail, "Upload\\");
 
                memStream = new MemoryStream(data);
                FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write);
                memStream.WriteTo(file);
                file.Close();
                file.Dispose();
                memStream.Close();
                memStream.Dispose();

                DirectoryInfo di = new DirectoryInfo(directoryName);
                FileInfo[] fo = di.GetFiles("*", SearchOption.AllDirectories);
                dataSet = new DataSet();
                DataTable dt = null;
                foreach (FileInfo d in fo)
                {
                    dt = new DataTable();
                    dt = ReadFileExcell(d.FullName, "Sheet1$");
                    dt.TableName = d.Name;
                    dataSet.Tables.Add(dt);
                }

                cqSP = new CommonQuerySP();

                if (!isValidate)
                {
                    isSave = cqSP.PostingExcellVH(db.Connection.ConnectionString, dataSet, false);
                }
                else
                {
                    lstResultNew = new List<Temporary_Voucher>();
                    for (nLoopC = 0; nLoopC < dataSet.Tables.Count; nLoopC++)
                    {
                        table = new DataTable();
                        table = dataSet.Tables[nLoopC];

                        for (nLoop = 0; nLoop < table.Rows.Count; nLoop++)
                        {
                            tiVoucher = new Temporary_Voucher();
                            row = table.Rows[nLoop];

                            DateTime date = row.GetValue<DateTime>("d_date", DateTime.MinValue);
                            if (date.Equals(DateTime.MinValue))
                            {
                                tmp = row.GetValue<string>("d_date", string.Empty);
                                if (!Functionals.DateParser(tmp, "M/d/yyyy HH:mm:ss", out date))
                                {
                                    date = Functionals.StandardSqlDateTime;
                                }
                            }

                            var bank = (from q in db.FA_MsBanks 
                                        join q1 in db.FA_MsBankReks on q.c_bank equals q1.c_bank
                                          where q.c_bank == row.GetValue<string>("c_bank", string.Empty).Trim()
                                          && q1.c_type == "02"
                                          select q.v_bank).Take(1).SingleOrDefault();

                            tiVoucher = new Temporary_Voucher()
                            {
                                Principal = row.GetValue<string>("Principal", string.Empty).Trim(),
                                c_fbno = row.GetValue<string>("c_fbno", string.Empty).Trim(),                                
                                n_value = row.GetValue<decimal>("n_value",0),
                                c_vdno = row.GetValue<string>("c_vdno", string.Empty).Trim(),
                                c_noteno = row.GetValue<string>("c_noteno", string.Empty).Trim(),
                                d_date = date.ToString("yyyyMMdd"),
                                c_bank = bank
                            };

                            lstResultNew.Add(tiVoucher);
                        }

                    }
                }

                di = new DirectoryInfo(path);

                if (lstResultNew != null)
                {
                    nCount = lstResultNew.Count;
                    
                    if ((lstResultNew.Count > 0) && isValidate)
                    {
                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, lstResultNew.ToArray());

                        lstResultNew.Clear();
                    }
                }               

                dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                dic.Add(Constant.DEFAULT_NAMING_EXCEPTION, isSave);
            }
            break;

          #endregion

          //Indra
          #region MODEL_COMMON_UPLOADED_QUERY_VOUCHER_EXCELL_USER

          case Constant.MODEL_COMMON_UPLOADED_QUERY_VOUCHER_EXCELL_USER:
            {
                isValidate = (parameters.ContainsKey("isValidate") ? Convert.ToBoolean(((Functionals.ParameterParser)parameters["isValidate"]).Value) : false);
                NIPEntry = (parameters.ContainsKey("nipEntry") ? (string)((Functionals.ParameterParser)parameters["nipEntry"]).Value : string.Empty);

                Temporary_User tiUser = null;

                Config cf = new Config();
                List<Temporary_User> lstResultNew = null;
                string isSave = string.Empty;

                string path = string.Concat(cf.PathTempExtractMail, "Upload\\" + originalName);
                string directoryName = string.Concat(cf.PathTempExtractMail, "Upload\\");

                memStream = new MemoryStream(data);
                FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write);
                memStream.WriteTo(file);
                file.Close();
                file.Dispose();
                memStream.Close();
                memStream.Dispose();

                DirectoryInfo di = new DirectoryInfo(directoryName);
                FileInfo[] fo = di.GetFiles("*", SearchOption.AllDirectories);
                dataSet = new DataSet();
                DataTable dt = null;
                foreach (FileInfo d in fo)
                {
                    dt = new DataTable();
                    dt = ReadFileExcell(d.FullName, "Sheet1$");
                    dt.TableName = d.Name;
                    dataSet.Tables.Add(dt);
                }

                cqSP = new CommonQuerySP();

                if (!isValidate)
                {
                    isSave = cqSP.PostingExcellUSER(db.Connection.ConnectionString, dataSet, false);

                }
                else
                {
                    lstResultNew = new List<Temporary_User>();
                    for (nLoopC = 0; nLoopC < dataSet.Tables.Count; nLoopC++)
                    {
                        table = new DataTable();
                        table = dataSet.Tables[nLoopC];

                        for (nLoop = 0; nLoop < table.Rows.Count; nLoop++)
                        {
                            tiUser = new Temporary_User();
                            row = table.Rows[nLoop];

                            tiUser = new Temporary_User()
                            {
                                nipEntry = NIPEntry,
                                No = nLoop + 1,
                                Nip = row.GetValue<string>("Nip", string.Empty).Trim(),
                                Nama = row.GetValue<string>("Nama", string.Empty).Trim(),
                                Tgl_Lahir = row.GetValue<string>("Tgl_Lahir", string.Empty).Trim(),
                                Gudang = row.GetValue<string>("Gudang", string.Empty).Trim(),
                                Aktif = row.GetValue<string>("Aktif", string.Empty).Trim()
                            };

                            lstResultNew.Add(tiUser);
                        }

                    }
                }

                di = new DirectoryInfo(path);

                if (lstResultNew != null)
                {
                    nCount = lstResultNew.Count;

                    if ((lstResultNew.Count > 0) && isValidate)
                    {
                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, lstResultNew.ToArray());

                        lstResultNew.Clear();
                    }
                }

                dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                dic.Add(Constant.DEFAULT_NAMING_EXCEPTION, isSave);
            }
            break;
          #endregion

          //Indra 20170714
          #region MODEL_COMMON_UPLOADED_QUERY_SEND_BATCH

          case Constant.MODEL_COMMON_UPLOADED_QUERY_SEND_BATCH:
            {
                isValidate = (parameters.ContainsKey("isValidate") ? Convert.ToBoolean(((Functionals.ParameterParser)parameters["isValidate"]).Value) : false);
                NIPEntry = (parameters.ContainsKey("nipEntry") ? (string)((Functionals.ParameterParser)parameters["nipEntry"]).Value : string.Empty);

                Temporary_ResendDOBatch tiDO = null;

                Config cf = new Config();
                List<Temporary_ResendDOBatch> lstResultNew = null;
                string isSave = string.Empty;

                string path = string.Concat(cf.PathTempExtractMail, "Upload\\" + originalName);
                string directoryName = string.Concat(cf.PathTempExtractMail, "Upload\\");

                memStream = new MemoryStream(data);
                FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write);
                memStream.WriteTo(file);
                file.Close();
                file.Dispose();
                memStream.Close();
                memStream.Dispose();

                DirectoryInfo di = new DirectoryInfo(directoryName);
                FileInfo[] fo = di.GetFiles("*", SearchOption.AllDirectories);
                dataSet = new DataSet();
                DataTable dt = null;
                foreach (FileInfo d in fo)
                {
                    dt = new DataTable();
                    dt = ReadFileExcell(d.FullName, "Sheet1$");
                    dt.TableName = d.Name;
                    dataSet.Tables.Add(dt);
                }

                cqSP = new CommonQuerySP();

                if (!isValidate)
                {
                    isSave = cqSP.PostingExcellResendDOBatch(db.Connection.ConnectionString, dataSet, false);

                }
                else
                {
                    lstResultNew = new List<Temporary_ResendDOBatch>();
                    for (nLoopC = 0; nLoopC < dataSet.Tables.Count; nLoopC++)
                    {
                        table = new DataTable();
                        table = dataSet.Tables[nLoopC];

                        for (nLoop = 0; nLoop < table.Rows.Count; nLoop++)
                        {
                            tiDO = new Temporary_ResendDOBatch();
                            row = table.Rows[nLoop];

                            tiDO = new Temporary_ResendDOBatch()
                            {
                                No = nLoop + 1,
                                NO_DO = row.GetValue<string>("NO_DO", string.Empty).Trim()
                            };

                            lstResultNew.Add(tiDO);
                        }

                    }
                }

                di = new DirectoryInfo(path);

                if (lstResultNew != null)
                {
                    nCount = lstResultNew.Count;

                    if ((lstResultNew.Count > 0) && isValidate)
                    {
                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, lstResultNew.ToArray());

                        lstResultNew.Clear();
                    }
                }

                dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                dic.Add(Constant.DEFAULT_NAMING_EXCEPTION, isSave);
            }
            break;
          #endregion

          #region MODEL_COMMON_UPLOADED_QUERY_SO

          case Constant.MODEL_COMMON_UPLOADED_QUERY_SO :
            {
                isValidate = (parameters.ContainsKey("isValidate") ? Convert.ToBoolean(((Functionals.ParameterParser)parameters["isValidate"]).Value) : false);
                nipEntry = (parameters.ContainsKey("nipEntry") ? (string)((Functionals.ParameterParser)parameters["nipEntry"]).Value : string.Empty);
                string tipe = (parameters.ContainsKey("tipe") ? (string)((Functionals.ParameterParser)parameters["tipe"]).Value : string.Empty);
                string tahun = (parameters.ContainsKey("tahun") ? (string)((Functionals.ParameterParser)parameters["tahun"]).Value : string.Empty);
                string bulan = (parameters.ContainsKey("bulan") ? (string)((Functionals.ParameterParser)parameters["bulan"]).Value : string.Empty);
                string sheet = (parameters.ContainsKey("sheet") ? (string)((Functionals.ParameterParser)parameters["sheet"]).Value : string.Empty);
                
                Temporary_SO tiSO = null;

                Config cf = new Config();
                List<Temporary_SO> lstResultNew = null;
                string isSave = string.Empty;

                string path = string.Concat(cf.PathTempExtractMail, "Upload\\SO\\" + originalName);
                string directoryName = string.Concat(cf.PathTempExtractMail, "Upload\\SO\\");

                memStream = new MemoryStream(data);
                FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write);
                memStream.WriteTo(file);
                file.Close();
                file.Dispose();
                memStream.Close();
                memStream.Dispose();

                DirectoryInfo di = new DirectoryInfo(directoryName);
                FileInfo[] fo = di.GetFiles("*", SearchOption.AllDirectories);
                dataSet = new DataSet();
                DataTable dt = null;
                foreach (FileInfo d in fo)
                {
                    dt = new DataTable();
                    dt = ReadFileExcell(d.FullName, sheet);
                    dt.TableName = d.Name;
                    dataSet.Tables.Add(dt);
                }

                cqSP = new CommonQuerySP();

                if (!isValidate)
                {
                    isSave = cqSP.PostingExcellSO(db.Connection.ConnectionString, dataSet, false, int.Parse(tahun), int.Parse(bulan), tipe, nipEntry);
                }
                else
                {
                    lstResultNew = new List<Temporary_SO>();
                    for (nLoopC = 0; nLoopC < dataSet.Tables.Count; nLoopC++)
                    {
                        table = new DataTable();
                        table = dataSet.Tables[nLoopC];

                        for (nLoop = 0; nLoop < table.Rows.Count; nLoop++)
                        {
                            tiSO = new Temporary_SO();
                            row = table.Rows[nLoop];

                            tiSO = new Temporary_SO()
                            {
                                Team = row.GetValue<string>("TEAM", string.Empty).Trim(),
                                C_nosup = row.GetValue<string>("C_NOSUP", string.Empty).Trim(),
                                Principal = row.GetValue<string>("PRINCIPAL", string.Empty).Trim(),
                                C_iteno = row.GetValue<string>("C_ITENO", string.Empty).Trim(),
                                C_itnam = row.GetValue<string>("C_ITNAM", string.Empty).Trim(),
                                Qty = row.GetValue<decimal>("QTY", 0),
                                Batch = row.GetValue<string>("BATCH", string.Empty).Trim(),
                                Ed = row.GetValue<string>("ED", string.Empty).Trim(),
                            };
                            lstResultNew.Add(tiSO);
                        }
                    }
                }

                di = new DirectoryInfo(path);

                if (lstResultNew != null)
                {
                    nCount = lstResultNew.Count;

                    if ((lstResultNew.Count > 0) && isValidate)
                    {
                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, lstResultNew.ToArray());

                        lstResultNew.Clear();
                    }
                }

                dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

                dic.Add(Constant.DEFAULT_NAMING_EXCEPTION, isSave);
            }
            break;

          #endregion
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          "ScmsSoaLibrary.Modules.CommonUploadedQuery:ModelUploadedQuery (First) <-> Switch {0} - {1}", model, ex.Message);
        Logger.WriteLine(ex.StackTrace);

        dic.Add(Constant.DEFAULT_NAMING_EXCEPTION, ex.Message);

        dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);
      }
      finally
      {
        if (listTmp1 != null)
        {
          listTmp1.Clear();
        }

        if (listTmp != null)
        {
          listTmp.Clear();
        }

        if (table != null)
        {
          table.Clear();
          table.Dispose();
        }

        if (dataSet != null)
        {
          dataSet.Clear();
          dataSet.Dispose();
        }

        if (memStream != null)
        {
          memStream.Close();
          memStream.Dispose();
        }

        if (excelReader != null)
        {
          excelReader.Close();
          excelReader.Dispose();
        }
      }

      db.Dispose();

      return dic;
    }

    public static System.Data.DataTable ReadDbfDatabase(string dbfFile, string tableName)
    {
      if (!System.IO.File.Exists(dbfFile))
      {
        return null;
      }

      System.Data.DataTable table = null;

      string fileName = System.IO.Path.GetFileName(dbfFile);
      string pathName = System.IO.Path.GetDirectoryName(dbfFile);
      string pathNameFull = (pathName.EndsWith("\\") ? pathName : string.Concat(pathName, "\\"));

      System.Data.OleDb.OleDbConnection con = null;
      System.Data.OleDb.OleDbCommand cmd = null;
      System.Data.OleDb.OleDbDataReader odbReader = null;

      try
      {
        //con = new System.Data.OleDb.OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source='{0}';Extended Properties=dBASE IV;", pathName));
        con = new System.Data.OleDb.OleDbConnection(string.Format("Provider=vfpoledb;Data Source='{0}';Collating Sequence=general;", pathName));
        con.Open();

        if (con.State == System.Data.ConnectionState.Open)
        {
          cmd = con.CreateCommand();
          cmd.CommandText = string.Format("Select * From [{0}]", fileName);

          odbReader = cmd.ExecuteReader();

          if ((odbReader != null) && (odbReader.HasRows) && (!odbReader.IsClosed))
          {
            table = new System.Data.DataTable(tableName);
            table.Load(odbReader);
          }
        }
      }
      catch (Exception ex)
      {
        table = null;

        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }
      finally
      {
        if (odbReader != null)
        {
          odbReader.Close();
          odbReader.Dispose();
        }

        if (con != null)
        {
          if (con.State == System.Data.ConnectionState.Open)
          {
            con.Close();
          }
          con.Dispose();
        }
      }

      try
      {
        System.IO.File.Delete(dbfFile);
      }
      catch (Exception ex)
      {
        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }

      return table;
    }

    public static System.Data.DataTable ReadFileExcell(string dbfFile, string tableName)
    {
        if (!System.IO.File.Exists(dbfFile))
        {
            return null;
        }

        DataTable table = new DataTable();
        System.Data.OleDb.OleDbConnection con = null;
        System.Data.OleDb.OleDbCommand cmd = null;

        try
        {
            con = new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source='{0}';Extended Properties=Excel 8.0;Persist Security Info=False;", dbfFile));
            con.Open();

            if (con.State == System.Data.ConnectionState.Open)
            {
                string strSQL = string.Format("SELECT * FROM [{0}]", tableName);
                
                cmd = new OleDbCommand(strSQL, con);

                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                adapter.Fill(table);
            }
        }
        catch (Exception ex)
        {
            table = null;

            Logger.WriteLine(ex.Message);
            Logger.WriteLine(ex.StackTrace);
        }
        finally
        {
            if (con != null)
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
                con.Dispose();
            }
        }

        try
        {
            System.IO.File.Delete(dbfFile);
        }
        catch (Exception ex)
        {
            Logger.WriteLine(ex.Message);
            Logger.WriteLine(ex.StackTrace);
        }

        return table;
    }
  }
}
