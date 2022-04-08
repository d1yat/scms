#define DUMMY_DATA


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ScmsModel;
using ScmsModel.Core;
using ScmsSoaLibrary.Commons;
using ScmsMailLibrary.Global;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsMailLibrary
{
  public class SendPOAuto
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

    internal class POHeaderSenderFormat
    {
      public string C_CORNO { get; set; }
      public DateTime D_CORDA { get; set; }
      public string C_KOMEN1 { get; set; }
      public string C_KOMEN2 { get; set; }
      public char L_LOAD { get; set; }
      public string C_KDDEPO { get; set; }
      public string C_KDCAB { get; set; }
      public string C_NMCAB { get; set; }
    }

    internal class PODetailSenderFormat
    {
      public string c_corno { get; set; }
      public string c_iteno { get; set; }
      public string c_itenopri { get; set; }
      public string c_itnam { get; set; }
      public string c_undes { get; set; }
      public decimal n_qty { get; set; }
      public decimal n_salpri { get; set; }
      public string c_nosp { get; set; }
      public string c_via { get; set; }
    }

    internal class SuplierInformation
    {
      public string KodeSuplier { get; set; }
      public string NamaSuplier { get; set; }
      public string Alamat1 { get; set; }
      public string Alamat2 { get; set; }
      public string[] Emails { get; set; }
    }

    #endregion

    private const string SMTP_IP = "10.100.10.9";
    private const int SMTP_PORT = 25;

    private const string DEFAULT_NAME_TABEL_PO_PRINCIPAL = "POPRINCIPAL";
    private const string DEFAULT_NAME_TABEL_PO_LIST = "POLIST";
    private const string DEFAULT_NAME_TABEL_PO_HEADER = "POHEADER";
    private const string DEFAULT_NAME_TABEL_PO_DETAIL = "PODETAIL";
    
    private const string MAIL_HEADER_NAME = "header.dbf";
    private const string MAIL_DETAIL_NAME = "detail.dbf";

    private AutoResetEvent areEvent;
    private AutoResetEvent areThreadStop;

    private ScmsSoaLibrary.Commons.Config config;

    private bool isStart;

    private volatile bool isRunning;

    public SendPOAuto(ScmsSoaLibrary.Commons.Config config)
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

        tmp = System.IO.Path.Combine(config.PathTempExtractMail, "POSend");

        tmp = (tmp.EndsWith("\\") ? tmp : string.Concat(tmp, "\\"));

        dir = tmp;
      }
      else
      {
        tmp = System.IO.Path.Combine(config.PathTempExtractMail, "POSend");

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

      this.IsActiveBackupSendPO = config.IsActiveBackupPOSend;
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
          keyName = row.GetValue<string>("C_CORNO", string.Empty);

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
            view = new System.Data.DataView(table, string.Format("C_CORNO = '{0}'", keyName), "c_itnam", System.Data.DataViewRowState.CurrentRows);

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
         string.Format("ScmsMailLibrary.SendPOAuto:BackupLogicFolder - {0}", ex.Message));

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
        tableHeader = null, tableDetail = null;
      System.IO.MemoryStream msHeader = null,
        msDetail = null;
      Dictionary<string, System.IO.MemoryStream> dicMsHeader = null,
        dicMsDetail = null;

      List<string> lstKeyHeader = null;

      System.Data.DataSet dataset = null;

      System.Data.DataRow row = null;

      int nLoop = 0,
        nLen = 0;

      string kodeSupl = null;

      Dictionary<string, string> dicPrinting = null;

      System.Data.DataTable tableMailSuplier = ReadAllActivePrincipal(db);

      System.Data.DataView view = new System.Data.DataView(tableMailSuplier, null, "KodeSuplier", System.Data.DataViewRowState.CurrentRows);

      //this.ClassNotify

      for (nLoop = 0, nLen = view.Count; nLoop < nLen; nLoop++)
      {
        row = view[nLoop].Row;

        kodeSupl = row.GetValue<string>("KodeSuplier", string.Empty);

        if (!string.IsNullOrEmpty(kodeSupl))
        {
          dataset = CreateReaderSetPOMultiple(db, kodeSupl, null);

          if (dataset != null)
          {
            if (dataset.Tables.Count == 3)
            {
              #region DEFAULT_NAME_TABEL_PO_HEADER
              
              if (dataset.Tables.Contains(DEFAULT_NAME_TABEL_PO_HEADER))
              {
                tableHeader = dataset.Tables[DEFAULT_NAME_TABEL_PO_HEADER];
                dicMsHeader = this.CreateDBFStream(tableHeader, pathFolder);
                //msHeader = ScmsMailLibrary.Core.Commons.CreateDBFStream(tableHeader, pathFolder);

                if (dicMsHeader.Count > 0)
                {
                  #region DEFAULT_NAME_TABEL_PO_DETAIL

                  if (dataset.Tables.Contains(DEFAULT_NAME_TABEL_PO_DETAIL))
                  {
                    lstKeyHeader = dicMsHeader.Keys.CollectionToList();

                    tableDetail = dataset.Tables[DEFAULT_NAME_TABEL_PO_DETAIL];
                    //msDetail = ScmsMailLibrary.Core.Commons.CreateDBFStream(tableDetail, pathFolder);
                    dicMsDetail = this.CreateDBFStreamDetail(lstKeyHeader, tableDetail, pathFolder);

                    dicPrinting = PrintPageMultiple(lstKeyHeader, row, tableHeader, tableDetail);

                    if (SendMailMultiple(row, dicMsHeader, dicMsDetail, dicPrinting))
                    {
                      #region DEFAULT_NAME_TABEL_PO_LIST

                      if (dataset.Tables.Contains(DEFAULT_NAME_TABEL_PO_LIST))
                      {
                        table = dataset.Tables[DEFAULT_NAME_TABEL_PO_LIST];

                        UpdateAllToTableMultiple(db, lstKeyHeader);

                        if (table != null)
                        {
                          table.Clear();
                          table.Dispose();
                        }
                      }

                      #endregion
                    }
                    
                    if (this.IsActiveBackupSendPO)
                    {
                      BackupLogicFolder(row, dicMsHeader, dicMsDetail, dicPrinting);
                    }

                    if (dicMsHeader.Count > 0)
                    {
                      foreach (KeyValuePair<string, System.IO.MemoryStream> kvp in dicMsHeader)
                      {
                        kvp.Value.Close();
                        kvp.Value.Dispose();
                      }

                      dicMsHeader.Clear();
                    }

                    if (dicMsDetail.Count > 0)
                    {
                      foreach (KeyValuePair<string, System.IO.MemoryStream> kvp in dicMsDetail)
                      {
                        kvp.Value.Close();
                        kvp.Value.Dispose();
                      }
                      
                      dicMsDetail.Clear();
                    }

                    dicPrinting.Clear();

                    if (msDetail != null)
                    {
                      msDetail.Close();
                      msDetail.Dispose();
                    }

                    if (tableDetail != null)
                    {
                      tableDetail.Clear();
                      tableDetail.Dispose();
                    }
                  }

                  #endregion
                }

                if (msHeader != null)
                {
                  msHeader.Close();
                  msHeader.Dispose();
                }

                if (tableHeader != null)
                {
                  tableHeader.Clear();
                  tableHeader.Dispose();
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

      if (tableMailSuplier != null)
      {
        tableMailSuplier.Clear();
        tableMailSuplier.Dispose();
      }

      if (lstKeyHeader != null)
      {
        lstKeyHeader.Clear();
      }

      db.Dispose();

      GC.Collect();
    }

    #region Runner

    private System.Data.DataTable ReadAllActivePrincipal(ORMDataContext db)
    {
      System.Data.DataTable table = null;

      var qry = (from q in db.LG_DatsupEmails
                 join q1 in db.LG_DatSups on q.c_nosup equals q1.c_nosup
                 where (q.c_form == "02")
                 group new { q, q1 } by new { q.c_nosup, q1.v_nama, q1.v_alamat1, q1.v_alamat2 } into g
                 select new SuplierInformation()
                 {
                   KodeSuplier = g.Key.c_nosup,
                   NamaSuplier = g.Key.v_nama,
                   Alamat1 = g.Key.v_alamat1,
                   Alamat2 = g.Key.v_alamat2,
                   Emails = (from sq in db.LG_DatsupEmails
                             where (sq.c_nosup == g.Key.c_nosup)
                              && (sq.c_form == "02")
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

    private System.Data.DataSet CreateReaderSetPOMultiple(ORMDataContext db, string noSup, string PoNo)
    {
      System.Data.DataSet dataset = new System.Data.DataSet();
      System.Data.DataTable table = null;

      DateTime date = DateTime.Now;

      string groupPo = date.ToString("yyMMddHHmm");

      List<POHeaderSenderFormat> lstHdr = null;
      List<PODetailSenderFormat> lstDtl = null;

      #region Header

      var qry = (from q in db.LG_POHs
                 join q1 in db.LG_POD1s on new { q.c_gdg, q.c_pono } equals new { q1.c_gdg, q1.c_pono }
                 join q2 in db.LG_POD2s on new { q.c_gdg, q.c_pono } equals new { q2.c_gdg, q2.c_pono }
                 where (q.c_nosup == noSup)
                   && ((q.l_send.HasValue ? q.l_send.Value : false) == true)
                   && ((q.l_print.HasValue ? q.l_print.Value : false) == true)
                   && ((q.l_sent.HasValue ? q.l_sent.Value : false) == false)
                   && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                   && ((string.IsNullOrEmpty(PoNo) ? q.c_pono : PoNo) == q.c_pono)
                   && (from sq in db.LG_ORHs
                       where (sq.c_gdg == q.c_gdg)
                          && ((sq.l_delete.HasValue ? sq.l_delete.Value : false) == false)
                       select sq.c_orno).Contains(q2.c_orno)
                 select new
                 {
                   q,
                   q1
                 }).Distinct().AsQueryable();

      lstHdr = (from q in qry
                group q by new { q.q.c_pono, q.q.d_podate, q.q.v_ket } into g
                select new POHeaderSenderFormat()
                {
                  C_CORNO = (g.Key.c_pono == null ? string.Empty : g.Key.c_pono.Substring(2)),
                  D_CORDA = (g.Key.d_podate.HasValue ? g.Key.d_podate.Value : Functionals.StandardSqlDateTime),
                  C_KOMEN1 = "Head Office",
                  C_KOMEN2 = string.Empty,
                  L_LOAD = 'T',
                  C_KDDEPO = string.Empty,
                  C_KDCAB = "HO",
                  C_NMCAB = g.Key.v_ket,
                }).ToList();

      table = lstHdr.CopyToDataTableObject();
      table.TableName = DEFAULT_NAME_TABEL_PO_HEADER;

      if (noSup.Equals("00019", StringComparison.OrdinalIgnoreCase))
      {
        table.Columns.Remove("C_NMCAB");
      }

      dataset.Tables.Add(table);

      #endregion

      if (lstHdr.Count > 0)
      {
        #region Detail

        //Production
        //var qryDtlSub = (from q in qry
        //                 join q7 in db.FA_MasItms on q.q1.c_iteno equals q7.c_iteno into q_7
        //                 from qItm in q_7.DefaultIfEmpty()
        //                 select new
        //                 {
        //                   q,
        //                   q.q1,
        //                   qItm
        //                 }).Distinct().AsQueryable();

        // Will Be Deleted (A)
        var qryDtlSub = (from q in qry
                         join q7 in db.FA_MasItms on q.q1.c_iteno equals q7.c_iteno into q_7
                         from qItm in q_7.DefaultIfEmpty()
                         select new
                         {
                           q,
                           q.q1,
                           qItm.c_iteno,
                           qItm.c_itenopri,
                           qItm.v_itnam,
                           qItm.v_undes,
                           qItm.c_via,
                           qItm.n_salpri
                         }).Distinct().AsQueryable();
        //(A)

        //Production
        //lstDtl = (from q in qryDtlSub
        //          join q10 in db.MsTransDs on new { c_portal = '3', c_notrans = "02", c_type = q.qItm.c_via } equals new { q10.c_portal, q10.c_notrans, q10.c_type } into q_10
        //          from qViaItmDesc in q_10.DefaultIfEmpty()
        //          group q by new { q.q.q.c_pono, q.q1.c_iteno, q.qItm.c_itenopri, q.qItm.v_itnam, q.qItm.v_undes, q.qItm.n_salpri, qViaItmDesc.v_ket } into g
        //          orderby g.Key.v_itnam ascending
        //          select new PODetailSenderFormat()
        //          {
        //            c_corno = (g.Key.c_pono == null ? string.Empty : g.Key.c_pono.Substring(2)),
        //            c_iteno = g.Key.c_iteno,
        //            c_itenopri = g.Key.c_itenopri,
        //            c_itnam = g.Key.v_itnam,
        //            c_undes = g.Key.v_undes,
        //            n_qty = g.Sum(t => (t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0)),
        //            n_salpri = (g.Key.n_salpri.HasValue ? g.Key.n_salpri.Value : 0),
        //            c_nosp = string.Empty,
        //            c_via = (g.Key.v_ket == null ? "Darat" : g.Key.v_ket).Substring(0, 1).ToUpper(),
        //          }).Distinct().ToList();

        lstDtl = (from q in qryDtlSub
                  join q10 in db.MsTransDs on new { c_portal = '3', c_notrans = "02", c_type = q.c_via } equals new { q10.c_portal, q10.c_notrans, q10.c_type } into q_10
                  from qViaItmDesc in q_10.DefaultIfEmpty()
                  group q by new { q.q.q.c_pono, q.q1.c_iteno, q.c_itenopri, q.v_itnam, q.v_undes, q.n_salpri, qViaItmDesc.v_ket } into g
                  orderby g.Key.v_itnam ascending
                  select new PODetailSenderFormat()
                  {
                    c_corno = (g.Key.c_pono == null ? string.Empty : g.Key.c_pono.Substring(2)),
                    c_iteno = g.Key.c_iteno,
                    c_itenopri = g.Key.c_itenopri,
                    c_itnam = g.Key.v_itnam,
                    c_undes = g.Key.v_undes,
                    n_qty = g.Sum(t => (t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0)),
                    n_salpri = (g.Key.n_salpri.HasValue ? g.Key.n_salpri.Value : 0),
                    c_nosp = string.Empty,
                    c_via = (g.Key.v_ket == null ? "Darat" : g.Key.v_ket).Substring(0, 1).ToUpper(),
                  }).Distinct().ToList();

        table = lstDtl.CopyToDataTableObject();
        table.TableName = DEFAULT_NAME_TABEL_PO_DETAIL;

        dataset.Tables.Add(table);

        #endregion

        #region All PO Sended

        table = new System.Data.DataTable(DEFAULT_NAME_TABEL_PO_LIST);

        table.Columns.Add("PO", typeof(string));

        table.BeginLoadData();

        for (int nLoop = 0, nLen = lstHdr.Count; nLoop < nLen; nLoop++)
        {
          table.LoadDataRow(new object[] { string.Concat("PO", lstHdr[nLoop].C_CORNO) }, true);
        }

        table.EndLoadData();

        dataset.Tables.Add(table);

        #endregion

        lstDtl.Clear();
        lstHdr.Clear();
      }

      return dataset;
    }

    private System.Data.DataSet CreateReaderSetPO(ORMDataContext db, string noSup, string PoNo)
    {
      System.Data.DataSet dataset = new System.Data.DataSet();
      System.Data.DataTable table = null;

      DateTime date = DateTime.Now;

      string groupPo = date.ToString("yyMMddHHmm");

      List<string> lstSentDO = null;
      List<POHeaderSenderFormat> lstHdr = null;
      List<PODetailSenderFormat> lstDtl = null;

      #region Header

      var qry = (from q in db.LG_POHs
                 join q1 in db.LG_POD1s on new { q.c_gdg, q.c_pono } equals new { q1.c_gdg, q1.c_pono }
                 join q2 in db.LG_POD2s on new { q.c_gdg, q.c_pono } equals new { q2.c_gdg, q2.c_pono }
                 where (q.c_nosup == noSup)
                   && ((q.l_send.HasValue ? q.l_send.Value : false) == true)
                   && ((q.l_print.HasValue ? q.l_print.Value : false) == true)
                   && ((q.l_sent.HasValue ? q.l_sent.Value : false) == false)
                   && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                   && ((string.IsNullOrEmpty(PoNo) ? q.c_pono : PoNo) == q.c_pono)
                   && (from sq in db.LG_ORHs
                       where (sq.c_gdg == q.c_gdg)
                          && ((sq.l_delete.HasValue ? sq.l_delete.Value : false) == false)
                       select sq.c_orno).Contains(q2.c_orno)
                 select new
                 {
                   q,
                   q1
                 }).Distinct().AsQueryable();

      lstSentDO = (from q in qry
                   group q by new { q.q.c_pono } into g
                   select (g.Key.c_pono == null ? string.Empty : g.Key.c_pono)).Distinct().ToList();

      lstHdr = new List<POHeaderSenderFormat>();
      lstHdr.Add(new POHeaderSenderFormat()
      {
        C_CORNO = groupPo,
        D_CORDA = date,
        C_KOMEN1 = "Group PO",
        C_KOMEN2 = string.Empty,
        C_KDDEPO = string.Empty,
        L_LOAD = 'T',
        C_KDCAB = "HO",
        C_NMCAB = "Head Office"
      });

      table = lstHdr.CopyToDataTableObject();
      table.TableName = DEFAULT_NAME_TABEL_PO_HEADER;

      dataset.Tables.Add(table);

      #endregion

      if (lstSentDO.Count > 0)
      {
        #region Detail

        var qryDtlSub = (from q in qry
                         join q7 in db.FA_MasItms on q.q1.c_iteno equals q7.c_iteno into q_7
                         from qItm in q_7.DefaultIfEmpty()
                         select new
                         {
                           q,
                           q.q1,
                           qItm
                         }).Distinct().AsQueryable();

        lstDtl = (from q in qryDtlSub
                  join q10 in db.MsTransDs on new { c_portal = '3', c_notrans = "02", c_type = q.qItm.c_via } equals new { q10.c_portal, q10.c_notrans, q10.c_type } into q_10
                  from qViaItmDesc in q_10.DefaultIfEmpty()
                  group q by new { q.q1.c_iteno, q.qItm.c_itenopri, q.qItm.v_itnam, q.qItm.v_undes, q.qItm.n_salpri, qViaItmDesc.v_ket } into g
                  orderby g.Key.v_itnam ascending
                  select new PODetailSenderFormat()
                  {
                    c_corno = groupPo,
                    c_iteno = g.Key.c_iteno,
                    c_itenopri = g.Key.c_itenopri,
                    c_itnam = g.Key.v_itnam,
                    c_nosp = string.Empty,
                    c_undes = g.Key.v_undes,
                    c_via = (g.Key.v_ket == null ? "Darat" : g.Key.v_ket).Substring(0, 1).ToUpper(),
                    n_qty = g.Sum(t => (t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0)),
                    n_salpri = (g.Key.n_salpri.HasValue ? g.Key.n_salpri.Value : 0),
                  }).Distinct().ToList();

        table = lstDtl.CopyToDataTableObject();
        table.TableName = DEFAULT_NAME_TABEL_PO_DETAIL;

        dataset.Tables.Add(table);

        #endregion

        #region All PO Sended

        if (lstSentDO.Count > 0)
        {
          table = new System.Data.DataTable(DEFAULT_NAME_TABEL_PO_LIST);

          table.Columns.Add("PO", typeof(string));

          table.BeginLoadData();

          for (int nLoop = 0, nLen = lstSentDO.Count; nLoop < nLen; nLoop++)
          {
            table.LoadDataRow(new object[] { lstSentDO[nLoop] }, true);
          }

          table.EndLoadData();

          dataset.Tables.Add(table);
        }

        #endregion

        lstSentDO.Clear();
        lstDtl.Clear();
        lstHdr.Clear();
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

    private bool SendMailMultiple(System.Data.DataRow rowSuplInfo, Dictionary<string, System.IO.MemoryStream> dicMsHeader, Dictionary<string, System.IO.MemoryStream> dicMsDetail, Dictionary<string, string> dicPrinting)
    {
      if ((rowSuplInfo == null) || (dicMsHeader == null) || (dicMsHeader.Count < 1) || (dicMsDetail == null) || (dicMsDetail.Count < 1) || (dicPrinting == null))
      {
        return false;
      }

      bool bOk = false;

      System.Net.Mail.MailMessage mail = null;
      System.Net.Mail.SmtpClient smtp = null;

      Encoding utf8 = Encoding.UTF8;

      int nLoop = 0, nLen = 0;

      string namaSupl = rowSuplInfo.GetValue<string>("NamaSuplier", string.Empty),
        tmp = null;
      string[] emailUsers = rowSuplInfo.GetValue<string[]>("Emails", new string[0]);

      StringBuilder sb = new StringBuilder();

      //System.IO.MemoryStream msHeader,
      //  msDetail = null;

      if ((emailUsers != null) && (emailUsers.Length > 0))
      {
        mail = new System.Net.Mail.MailMessage();
        mail.From = new System.Net.Mail.MailAddress("scms@ams.co.id", "Supply Chain Management System");

        mail.Subject = string.Concat("Permintaan Untuk - ", namaSupl);

        for (nLoop = 0, nLen = emailUsers.Length; nLoop < nLen; nLoop++)
        {
          mail.To.Add(emailUsers[nLoop]);
        }

#if DUMMY_DATA

        for (nLoop = 0, nLen = 10; nLoop < nLen; nLoop++)
        {
          sb.Append("Dummy ");
        }

#endif

        sb.AppendLine();
        sb.AppendLine();

        sb.AppendLine("Pengiriman PO :");

        foreach (KeyValuePair<string, System.IO.MemoryStream> kvp in dicMsHeader)
        {
          mail.Attachments.Add(new System.Net.Mail.Attachment(kvp.Value, string.Concat(kvp.Key, "H.dbf"),
            System.Net.Mime.MediaTypeNames.Application.Octet));
        }
        foreach (KeyValuePair<string, System.IO.MemoryStream> kvp in dicMsDetail)
        {
          mail.Attachments.Add(new System.Net.Mail.Attachment(kvp.Value, string.Concat(kvp.Key, "D.dbf"),
            System.Net.Mime.MediaTypeNames.Application.Octet));
        }

        //mail.Attachments.Add(new System.Net.Mail.Attachment(msHeader, MAIL_HEADER_NAME,
        //  System.Net.Mime.MediaTypeNames.Application.Octet));
        //mail.Attachments.Add(new System.Net.Mail.Attachment(msDetail, MAIL_DETAIL_NAME,
        //  System.Net.Mime.MediaTypeNames.Application.Octet));

        nLoop = 1;

        foreach (KeyValuePair<string, string> kvp in dicPrinting)
        {
          tmp = kvp.Key;
          sb.AppendLine(string.Concat(nLoop, ". ", tmp));

          mail.Attachments.Add(System.Net.Mail.Attachment.CreateAttachmentFromString(kvp.Value, string.Concat(tmp, ".txt")));

          nLoop++;
        }

        sb.AppendLine();
        sb.AppendLine();

        sb.AppendLine("Mohon untuk diperiksa dan diproses.");

        mail.Body = sb.ToString();

        smtp = new System.Net.Mail.SmtpClient(SMTP_IP, SMTP_PORT);

        try
        {
          smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
          smtp.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
          smtp.Send(mail);

          bOk = true;
        }
        catch (Exception ex)
        {
          Logger.WriteLine(ex.Message);
          Logger.WriteLine(ex.StackTrace);
        }
      }

      return bOk;
    }

    private bool SendMail(System.Data.DataRow rowSuplInfo, System.IO.MemoryStream msHeader, System.IO.MemoryStream msDetail, Dictionary<string, string> dicPrinting)
    {
      if ((rowSuplInfo == null) || (msHeader == null) || (msHeader == null) || (dicPrinting == null))
      {
        return false;
      }

      bool bOk = false;

      System.Net.Mail.MailMessage mail = null;
      System.Net.Mail.SmtpClient smtp = null;

      Encoding utf8 = Encoding.UTF8;

      int nLoop = 0, nLen = 0;

      string namaSupl = rowSuplInfo.GetValue<string>("NamaSuplier", string.Empty),
        tmp = null;
      string[] emailUsers = rowSuplInfo.GetValue<string[]>("Emails", new string[0]);
      StringBuilder sb = new StringBuilder();

      if ((emailUsers != null) && (emailUsers.Length > 0))
      {
        mail = new System.Net.Mail.MailMessage();
        mail.From = new System.Net.Mail.MailAddress("scms@ams.co.id", "Supply Chain Management System");

        mail.Subject = string.Concat("Permintaan Untuk - ", namaSupl);

        for (nLoop = 0, nLen = emailUsers.Length; nLoop < nLen; nLoop++)
        {
          mail.To.Add(emailUsers[nLoop]);
        }

        for (nLoop = 0, nLen = 10; nLoop < nLen; nLoop++)
        {
          sb.Append("Dummy ");
        }

        sb.AppendLine();
        sb.AppendLine();

        sb.AppendLine("Pengiriman PO :");

        mail.Attachments.Add(new System.Net.Mail.Attachment(msHeader, MAIL_HEADER_NAME,
          System.Net.Mime.MediaTypeNames.Application.Octet));
        mail.Attachments.Add(new System.Net.Mail.Attachment(msDetail, MAIL_DETAIL_NAME,
          System.Net.Mime.MediaTypeNames.Application.Octet));

        nLoop = 1;

        foreach (KeyValuePair<string, string> kvp in dicPrinting)
        {
          tmp = kvp.Key;
          sb.AppendLine(string.Concat(nLoop, ". ", tmp));

          mail.Attachments.Add(System.Net.Mail.Attachment.CreateAttachmentFromString(kvp.Value, string.Concat(tmp, ".txt")));

          nLoop++;
        }

        sb.AppendLine();
        sb.AppendLine();

        sb.AppendLine("Mohon untuk diperiksa dan diproses.");

        mail.Body = sb.ToString();

        smtp = new System.Net.Mail.SmtpClient(SMTP_IP, SMTP_PORT);

        try
        {
          smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
          smtp.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
          smtp.Send(mail);

          bOk = true;
        }
        catch (Exception ex)
        {
          Logger.WriteLine(ex.Message);
          Logger.WriteLine(ex.StackTrace);
        }
      }

      return bOk;
    }

    private void UpdateAllToTableMultiple(ORMDataContext db, List<string> listParentKeys)
    {
      int nLoop = 0,
        nLen = 0;

      string poNumber = null;

      List<string> lstPO = new List<string>();

      List<LG_POH> lstPOH = null;

      for (nLoop = 0, nLen = listParentKeys.Count; nLoop < nLen; nLoop++)
      {
        poNumber = string.Concat("PO", listParentKeys[nLoop]);

        if (!string.IsNullOrEmpty(poNumber))
        {
          if (!lstPO.Contains(poNumber))
          {
            lstPO.Add(poNumber);
          }
        }
      }

      if (lstPO.Count > 0)
      {
        lstPOH = (from q in db.LG_POHs
                  where lstPO.Contains(q.c_pono)
                    && ((q.l_sent.HasValue ? q.l_sent.Value : false) == false)
                  select q).Distinct().ToList();

        if (lstPOH != null)
        {
          try
          {
            for (nLoop = 0, nLen = lstPOH.Count; nLoop < nLen; nLoop++)
            {
              lstPOH[nLoop].l_sent = true;
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
            lstPOH.Clear();
          }
        }

        lstPO.Clear();
      }
    }

    private void UpdateAllToTable(ORMDataContext db, System.Data.DataTable table)
    {
      int nLoop = 0,
        nLen = 0;

      System.Data.DataRow row = null;

      string poNumber = null;

      List<string> lstPO = new List<string>();

      List<LG_POH> lstPOH = null;

      for (nLoop = 0, nLen = table.Rows.Count; nLoop < nLen; nLoop++)
      {
        row = table.Rows[nLoop];

        poNumber = row.GetValue<string>("PO", string.Empty);

        if (!string.IsNullOrEmpty(poNumber))
        {
          if (!lstPO.Contains(poNumber))
          {
            lstPO.Add(poNumber);
          }
        }
      }

      if (lstPO.Count > 0)
      {
        lstPOH = (from q in db.LG_POHs
                  where lstPO.Contains(q.c_pono)
                    && ((q.l_sent.HasValue ? q.l_sent.Value : false) == false)
                  select q).Distinct().ToList();

        if (lstPOH != null)
        {
          try
          {
            for (nLoop = 0, nLen = lstPOH.Count; nLoop < nLen; nLoop++)
            {
              lstPOH[nLoop].l_sent = true;
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
            lstPOH.Clear();
          }
        }

        lstPO.Clear();
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
      catch (Exception ex)
      {
        this.TriggerClassNotify(
          string.Format("SendPOAuto.WoTCallback - {0}", ex.Message),
          ClassNotifyEventArgs.TypeEnum.IsError);
        
        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }

      isRunning = false;
    }

    #endregion
    
    public string TempPath
    { get; private set; }
    
    public bool Start()
    {
      if (isStart)
      {
        this.TriggerClassNotify("PO Sender Auto sudah aktif.", ClassNotifyEventArgs.TypeEnum.IsNotify);

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
          string.Format("SendPOAuto.Start - {0}", ex.Message),
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
        this.TriggerClassNotify("PO Sender Auto belum aktif.", ClassNotifyEventArgs.TypeEnum.IsNotify);

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
          string.Format("SendPOAuto.Stop - {0}", ex.Message),
          ClassNotifyEventArgs.TypeEnum.IsError);

        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }
    }

    public TimeSpan TimePeriodic
    { get; private set; }

    public string MonitoringFolder
    { get; private set; }

    public bool IsActiveBackupSendPO
    { get; private set; }

    public void Testing()
    {
      RunningLogicSender(this.TempPath);
    }
  }
}
