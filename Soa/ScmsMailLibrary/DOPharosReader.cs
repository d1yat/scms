using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsSoaLibrary.Commons;
using System.ServiceModel.Channels;
using ScmsModel;
using ScmsSoaLibraryInterface.Commons;
using System.Threading;

namespace ScmsMailLibrary
{
  public class DOPharosReader
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

    public DOPharosReader(ScmsSoaLibrary.Commons.Config config)
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

        tmp = System.IO.Path.Combine(config.PathTempExtractMail, "DOGet");

        tmp = (tmp.EndsWith("\\") ? tmp : string.Concat(tmp, "\\"));

        dir = tmp;
      }
      else
      {
        tmp = System.IO.Path.Combine(config.PathTempExtractMail, "DOGet");

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

    ~DOPharosReader()
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
          string.Format("ScmsMailLibrary.DOPharosReader:Stop - {0}", ex.Message));
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

      e.Delete = true;

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
            nChanges += PopulateDoHeaderDetailPI(dataset, dicMappingPrinsipal, db);
          }

          if (dataset != null)
          {
            dataset.Clear();
            dataset.Dispose();
          }
        }

        if (nChanges > 0)
        {
          db.SubmitChanges();
        }
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

    private int PopulateDoHeaderDetailPI(System.Data.DataSet dataset, Dictionary<string, string> dicMappingPrinsipalGroup, ScmsModel.ORMDataContext db)
    {
      if ((dataset == null) || (dataset.Tables.Count != 2) || (dicMappingPrinsipalGroup == null))
      {
        return 0;
      }

      const string PENDING_PO_NAME = "(PENDING)";

      List<LG_DOPH> lstDOPH = null;
      LG_DOPH doph = null;
      List<LG_DOPD> lstDOPD = null;
      LG_DOPD dopd = null;
      List<LG_DOPH> lstDOPHRem = null;
      List<LG_DOPD> lstDOPDRem = null;

      FA_MasItm itm = null;

      int nChanges = 0;
      System.Data.DataTable table = null,
        tableDetail = null;
      string namaPrinsipal = null,
        tmp = null,
        kodePrinsipal = null,
        poData = null;
      System.Data.DataRow row = null;
      System.Data.DataRow[] rowCols = null;
      DateTime date = DateTime.MinValue;

      string[] poList = null;
      DateTime dateNow = DateTime.Now;
      bool isPending = false;

      int nLoop = 0,
        nLen = 0,
        nLoopC = 0,
        nLenC = 0,
        nLoopCi = 0,
        nLenCi = 0;

      decimal nPoQtySisa = 0,
        nQtyDO = 0;

      List<DO_PO_Link> lstPoLink = null;
      DO_PO_Link dol = default(DO_PO_Link);

      List<DO_PI_Header_Check> lstDOPHeaderCheck = null;
      List<DO_PI_Detail_Check> lstDOPDetailCheck = null;
      List<DO_PI_Result> lstDOPResult = null;
      //DO_PI_Result dopic = null;
      itm = new FA_MasItm();
      string kode_item = null;

      if (dataset.Tables.Contains(DEFAULT_NAME_TABEL_HEADER))
      {
        table = dataset.Tables[DEFAULT_NAME_TABEL_HEADER];

        if (dataset.Tables.Contains(DEFAULT_NAME_TABEL_DETAIL))
        {
          tableDetail = dataset.Tables[DEFAULT_NAME_TABEL_DETAIL];
        }

        lstDOPH = new List<LG_DOPH>();
        lstDOPD = new List<LG_DOPD>();

        #region Populate Data

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
            //kodePrinsipal = "00001";
            kodePrinsipal = "00000";
          }

          #region DOPH

          doph = new LG_DOPH()
          {
            c_nosup = kodePrinsipal,
            c_dono = row.GetValue<string>("C_NODO", string.Empty),
            d_dodate = null,
            d_fjno = row.GetValue<string>("C_EXNOINV", string.Empty),
            d_fjdate = null,
            l_status = false,
            c_cab = row.GetValue<string>("C_KDCAB", string.Empty),
            c_via = null,
            c_taxno = row.GetValue<string>("C_NOTAX", string.Empty),
            d_entry = dateNow
          };

          #endregion

          if (tableDetail != null)
          {
            rowCols = tableDetail.Select(string.Format("C_PT = '{0}' And C_NODO = '{1}'",
              namaPrinsipal, doph.c_dono), "C_ITNAM");

            if ((rowCols != null) && (rowCols.Length > 0))
            {
                

              #region Lanjutan DOPH

              date = row.GetValue<DateTime>("D_TGLDO", DateTime.MinValue);
              if (date.Equals(DateTime.MinValue))
              {
                tmp = row.GetValue<string>("D_TGLDO", string.Empty);
                if (!Functionals.DateParser(tmp, "M/d/yyyy HH:mm:ss", out date))
                {
                  date = Functionals.StandardSqlDateTime;
                }
              }
              doph.d_dodate = date;

              date = row.GetValue<DateTime>("D_JTH", DateTime.MinValue);
              if (date.Equals(DateTime.MinValue))
              {
                tmp = row.GetValue<string>("D_JTH", string.Empty);
                if (!Functionals.DateParser(tmp, "M/d/yyyy HH:mm:ss", out date))
                {
                  date = Functionals.StandardSqlDateTime;
                }
              }
              doph.d_fjdate = date;

              tmp = row.GetValue<string>("C_VIA", string.Empty).Trim();
              if (tmp == null)
              {
                tmp = "00";
              }
              else if (tmp.Equals("D", StringComparison.OrdinalIgnoreCase))
              {
                tmp = "02";
              }
              else if (tmp.Equals("U", StringComparison.OrdinalIgnoreCase))
              {
                tmp = "01";
              }
              else
              {
                tmp = "03";
              }
              doph.c_via = tmp;

              #endregion

              for (nLoopC = 0, nLenC = rowCols.Length; nLoopC < nLenC; nLoopC++)
              {
                row = rowCols[nLoopC];

                tmp = row.GetValue<string>("C_ITENO", string.Empty).Trim();

                if (string.IsNullOrEmpty(tmp))
                {

                    string noPric = row.GetValue<string>("C_ITENOPRI", string.Empty).Trim();
                     kode_item = (from q in db.FA_MasItms
                                        where q.c_itenopri == noPric
                                        select q.c_iteno).Take(1).SingleOrDefault();
                }

                poData = row.GetValue<string>("C_NOSP", string.Empty).Replace(" ", "").Trim();
                nQtyDO = row.GetValue<decimal>("N_QTYRCV");

                if (nQtyDO <= 0.00m)
                {
                  continue;
                }

                if (string.IsNullOrEmpty(kode_item) && string.IsNullOrEmpty(row.GetValue<string>("C_ITENO", string.Empty).Trim()))
                {
                    continue;
                }

                #region Default Detail

                dopd = new LG_DOPD()
                {
                  c_nosup = kodePrinsipal,
                  c_dono = row.GetValue<string>("C_NODO", string.Empty),
                  c_iteno = string.IsNullOrEmpty(row.GetValue<string>("C_ITENO", string.Empty).Trim()) ? kode_item : row.GetValue<string>("C_ITENO", string.Empty),
                  c_itenopri = row.GetValue<string>("C_ITENOPRI", string.Empty),
                  v_itnam = null,
                  v_undes = null,
                  n_qty = nQtyDO,
                  n_qty_sisa = nQtyDO,
                  c_batch = row.GetValue<string>("C_BATCH", string.Empty),
                  d_expired = null,
                  c_pono = null,
                  n_disc = row.GetValue<decimal>("N_DISC"),
                  l_claim = row.GetValue<bool>("L_CLAIMBNS"),
                  c_type = "01",
                  d_entry = dateNow,
                  l_done = false,
                  l_pending = false
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
                        l_pending = isPending
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

                  if ((!string.IsNullOrEmpty(poData)) && (poData.Length == 8))
                  {
                    dopd.c_pono = string.Concat("PO", poData);
                    dopd.l_pending = isPending;

                    lstDOPD.Add(dopd);
                  }

                  #endregion
                }
              }
            }

            lstDOPH.Add(doph);
          }
        }

        #endregion

        #region Check Header

        lstDOPHeaderCheck = lstDOPH.GroupBy(x => new { x.c_nosup, x.c_dono })
          .Select(y => new DO_PI_Header_Check()
          {
            Principal = y.Key.c_nosup.Trim(),
            DO = y.Key.c_dono.Trim()
          }).ToList();

        if ((lstDOPHeaderCheck != null) && (lstDOPHeaderCheck.Count > 1))
        {
          //tmp = (from q in db.LG_DOPHs
          //       where lstDOPHeaderCheck.Contains(new DO_PI_Header_Check()
          //       {
          //         Principal = q.c_nosup,
          //         DO = q.c_dono
          //       })
          //       select new DO_PI_Result()
          //       {
          //         Principal = q.c_nosup,
          //         DO = q.c_dono
          //       }).Distinct().Provider.ToString();

          lstDOPResult = (from q in db.LG_DOPHs
                          where lstDOPHeaderCheck.Contains(new DO_PI_Header_Check()
                          {
                            Principal = q.c_nosup,
                            DO = q.c_dono
                          })
                          select new DO_PI_Result()
                          {
                            Principal = q.c_nosup.Trim(),
                            DO = q.c_dono.Trim()
                          }).Distinct().ToList();

          if ((lstDOPResult != null) && (lstDOPResult.Count > 0))
          {
              lstDOPHRem = (from q in lstDOPH
                            join q1 in lstDOPResult on q.c_nosup equals q1.Principal
                            select q).ToList();

              if ((lstDOPHRem != null) && (lstDOPHRem.Count > 0))
              {
                  for (nLoop = 0, nLen = lstDOPHRem.Count; nLoop < nLen; nLoop++)
                  {
                      lstDOPH.Remove(lstDOPHRem[nLoop]);
                  }

                  lstDOPHRem.Clear();
              }

              //for (nLoop = 0, nLen = lstDOPResult.Count; nLoop < nLen; nLoop++)
              //{
              //  dopic = lstDOPResult[nLoop];

              //  doph = lstDOPH.Find(delegate(LG_DOPH dopoh)
              //  {
              //    return (dopoh.c_nosup.Equals(dopic.Principal, StringComparison.OrdinalIgnoreCase) &&
              //      dopoh.c_dono.Equals(dopic.DO, StringComparison.OrdinalIgnoreCase));
              //  });

              //  if (doph != null)
              //  {
              //    lstDOPH.Remove(doph);
              //  }
              //}

              lstDOPResult.Clear();
          }

          lstDOPHeaderCheck.Clear();
        }

        #endregion

        #region Check Detail


        lstDOPDetailCheck = lstDOPD.GroupBy(x => new { x.c_nosup, x.c_dono, x.c_iteno, x.c_type, x.c_pono, x.c_batch })
          .Select(y => new DO_PI_Detail_Check()
          {
            Principal = y.Key.c_nosup.Trim(),
            DO = y.Key.c_dono.Trim(),
            Item = y.Key.c_iteno.Trim(),
            PO = y.Key.c_pono.Trim(),
            Batch = y.Key.c_batch.Trim()
          }).ToList();

        if ((lstDOPDetailCheck != null) && (lstDOPDetailCheck.Count > 1))
        {
          //tmp = (from q in db.LG_DOPDs
          //       where lstDOPDetailCheck.Contains(new DO_PI_Detail_Check()
          //       {
          //         Principal = q.c_nosup,
          //         DO = q.c_dono,
          //         Item = q.c_iteno,
          //         PO = q.c_pono,
          //         Batch = q.c_batch
          //       })
          //       select new DO_PI_Result()
          //       {
          //         Principal = q.c_nosup,
          //         DO = q.c_dono,
          //         Item = q.c_iteno,
          //         PO = q.c_pono,
          //         Batch = q.c_batch
          //       }).Distinct().Provider.ToString();

          lstDOPResult = (from q in db.LG_DOPDs
                          where lstDOPDetailCheck.Contains(new DO_PI_Detail_Check()
                          {
                            Principal = q.c_nosup,
                            DO = q.c_dono,
                            Item = q.c_iteno,
                            PO = q.c_pono,
                            Batch = q.c_batch
                          })
                          select new DO_PI_Result()
                          {
                            Principal = q.c_nosup.Trim(),
                            DO = q.c_dono.Trim(),
                            Item = q.c_iteno.Trim(),
                            PO = q.c_pono.Trim(),
                            Batch = q.c_batch.Trim()
                          }).Distinct().ToList();

          if ((lstDOPResult != null) && (lstDOPResult.Count > 0))
          {
            lstDOPDRem = (from q in lstDOPD
                          join q1 in lstDOPResult on q.c_nosup equals q1.Principal
                          select q).Distinct().ToList();

            if ((lstDOPDRem != null) && (lstDOPDRem.Count > 0))
            {
              for (nLoop = 0, nLen = lstDOPDRem.Count; nLoop < nLen; nLoop++)
              {
                lstDOPD.Remove(lstDOPDRem[nLoop]);
              }

              lstDOPDRem.Clear();
            }

            //for (nLoop = 0, nLen = lstDOPResult.Count; nLoop < nLen; nLoop++)
            //{
            //  dopic = lstDOPResult[nLoop];

            //  doph = lstDOPH.Find(delegate(LG_DOPH dopoh)
            //  {
            //    return (dopoh.c_nosup.Equals(dopic.Principal, StringComparison.OrdinalIgnoreCase) &&
            //      dopoh.c_dono.Equals(dopic.DO, StringComparison.OrdinalIgnoreCase));
            //  });

            //  if (doph != null)
            //  {
            //    lstDOPH.Remove(doph);
            //  }
            //}

            lstDOPResult.Clear();
          }

          lstDOPDetailCheck.Clear();
        }

        #endregion

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
      }

      return nChanges;
    }

    #region Folder Monitor

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
              totalSaved = PopulateDoHeaderDetailPI(dataset, dicMappingPrinsipal, db);
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
              fi.Delete();
            }
            catch (Exception ex)
            {
              Logger.WriteLine(
                string.Format("ScmsMailLibrary.DOPharosReader:DOPharosReader DeleteFile - {0}", ex.Message));

              Logger.WriteLine(ex.StackTrace);
            }
          }
        }
        catch (Exception ex)
        {
          Logger.WriteLine(
           string.Format("ScmsMailLibrary.DOPharosReader:FolderReader - {0}", ex.Message));

          Logger.WriteLine(ex.StackTrace);
        }

        if (db != null)
        {
          if (nChanges > 0)
          {
            db.SubmitChanges();
          }

          db.Dispose();
        }
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
