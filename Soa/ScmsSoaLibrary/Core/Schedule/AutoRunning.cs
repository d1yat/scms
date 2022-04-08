using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsModel;
using ScmsSoaLibraryInterface.Commons;
using System.IO;
using System.Threading;
using System.Data.Linq.SqlClient;

namespace ScmsSoaLibrary.Core.Schedule
{
  public class AutoRunning
  {
    private volatile bool isStart;
    private volatile System.Timers.Timer timerX;
    private volatile System.Timers.Timer timerMon;
    private volatile Config cfg;

    private bool isReady;

    private TimeSpan AUTO_RUNTIME;

    private const int AUTO_DATEDAYHISTORY = 1;
    private const int MAX_INTERVAL = 3;

    private const int MAX_DATA_GET = 150;
    
    private volatile StringBuilder sbLogHistory;

    private volatile ManualResetEvent treSP;
    private volatile ManualResetEvent treOR;
    private volatile ManualResetEvent trePO;
    private volatile ManualResetEvent trePL;
    private volatile ManualResetEvent treSJ;
    private volatile ManualResetEvent treSTT;
    private volatile ManualResetEvent treSG;

    private volatile ManualResetEvent treMonSP;
    private volatile ManualResetEvent treMonPL;
    private volatile ManualResetEvent treMonDO;
    private volatile ManualResetEvent treMonSG;
    private volatile ManualResetEvent treMonSJ;

    #region Helper

    enum HistoryThreadingProsesEnum
    {
      IsSP = 1,
      IsOR,
      IsPO,
      IsPL,
      IsSJ,
      IsSTT,
      IsSG
    }

    struct HistoryThreadingProses
    {
      public bool IsSet;
      public ORMDataContext dbCurrent;
      public ORMDataContext dbHistory;
      public DateTime dateNow;
      public HistoryThreadingProsesEnum TipeProses;
    }

    enum MonitoringThreadingProsesEnum
    {
      IsPLSnap = 1
    }

    struct MonitoringThreadingProses
    {
      public bool IsSet;
      public ORMDataContext dbCurrent;
      public DateTime dateNow;
      public MonitoringThreadingProsesEnum TipeProses;
    }

    #endregion

    #region Class Helper

    public class MonitoringSnapShot
    {
      public string Cusno { get; set; }
      public DateTime TrxDate { get; set; }
      public string Iteno { get; set; }
      public decimal Qty { get; set; }
      public decimal Sisa { get; set; }
    }

    #endregion

    public AutoRunning(Config cfg)
    {
      DirectoryInfo di = null;

      this.cfg = cfg;
      this.AUTO_RUNTIME = new TimeSpan(0, 3, 0);

      try
      {
        di = new DirectoryInfo(cfg.PathTempLog);

        if (!di.Exists)
        {
          di.Create();
        }

        isReady = true;
      }
      catch (Exception ex)
      {
        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }
    }

    public void Start()
    {
      if (!isReady)
      {
        Logger.WriteLine("Auto schedule it's not ready yet, check log folder.");
        return;
      }
      else if (isStart)
      {
        return;
      }

      isStart = true;

      #region Auto Monitoring

      try
      {
        timerMon = new System.Timers.Timer((cfg.TimerPeriodicMonitoring.TotalSeconds * 1000));
        timerMon.AutoReset = false;
        timerMon.Enabled = true;

        timerMon.Elapsed += new System.Timers.ElapsedEventHandler(timerMon_Elapsed);

        timerMon.Start();
      }
      catch (Exception ex)
      {
        Logger.WriteLine(string.Concat("Starting AutoMonitoring : ", ex.Message));
        Logger.WriteLine(ex.StackTrace);
      }

      #endregion
    }

    public void Stop()
    {
      if (!isReady)
      {
        Logger.WriteLine("Auto schedule it's not ready yet, check log folder.");
        return;
      }
      else if (!isStart)
      {
        return;
      }

      isStart = false;

      if (timerX != null)
      {
        timerX.Enabled = false;

        timerX.Stop();

        timerX.Close();
        timerX.Dispose();
      }
    }

    public bool IsStart
    {
      get
      {
        if (timerX == null)
        {
          isStart = false;
        }

        return isStart;
      }
    }

    public void Testing()
    {
      ORMDataContext db = new ORMDataContext();

      DateTime date = DateTime.Now;

      /*
       * t_c_type :
       *            01 = SP
       *            02 = PL
       *            03 = DO
       *            04 = Exp
       *            05 = SG
       *            06 = SJ
       *            
       * v_c_type :
       *            01 = Done
       *            02 = Pending
       *            03 = Done CITO
       *            04 = Pending CITO
       */

      int curPos = 0,
        totalRows = 0,
        nLoop = 0,
        nLen = 0;
      //string sqlQuery = null;

      IQueryable<MonitoringSnapShot> qrySnapSP = null;
      //IQueryable<SCMS_MONITORING_SNAPSHOT> qrySnapShot = null;

      List<MonitoringSnapShot> listSnapShotSP = null;
      //List<SCMS_MONITORING_SNAPSHOT> listSnapShot = null;

      MonitoringSnapShot mss = null;
      SCMS_MONITORING_SNAPSHOT sms = null;

      totalRows = db.ExecuteCommand("DELETE FROM SCMS_MONITORING_SNAPSHOT WHERE t_c_type = {0}", "01");
        
      qrySnapSP = (from q in db.LG_SPHs
                   join q1 in db.LG_SPD1s on q.c_spno equals q1.c_spno
                   where (SqlMethods.DateDiffMonth(q.d_spdate, date) < 2)
                     && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                   orderby new { q.d_spdate, q1.c_iteno } ascending
                   select new MonitoringSnapShot()
                   {
                     Cusno = q.c_cusno,
                     TrxDate = (q.d_spdate.HasValue ? q.d_spdate.Value : Functionals.StandardSqlDateTime),
                     Iteno = q1.c_iteno,
                     Qty = (q1.n_acc.HasValue ? q1.n_acc.Value : 0),
                     Sisa = (q1.n_sisa.HasValue ? q1.n_sisa.Value : 0)
                   }).Distinct().AsQueryable();

      totalRows = qrySnapSP.Count();

      while (totalRows > curPos)
      {
        listSnapShotSP = qrySnapSP.Skip(curPos).Take(MAX_DATA_GET).ToList();

        for (nLoop = 0, nLen = listSnapShotSP.Count; nLoop < nLen; nLoop++)
        {
          mss = listSnapShotSP[nLoop];

          sms = (from q in db.SCMS_MONITORING_SNAPSHOTs
                 where (q.c_cusno == mss.Cusno) && (q.v_c_type == "01")
                 select q).Take(1).SingleOrDefault();

          //sms.
        }
      }
    }

    #region Event
    
    private void timerMon_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      DateTime date = DateTime.Now;
      StringBuilder sb = null;
      StreamWriter sw = null;
      string fName = null;
      DirectoryInfo di = new DirectoryInfo(cfg.PathTempLog);
      FileInfo fi = null;

      if (this.cfg.IsActiveMonitoring && ((date.Hour >= 7) && (date.Hour <= 22)))
      {
        try
        {
          if (!di.Exists)
          {
            di.Create();
          }

          fName = string.Concat("MON_", date.ToString("yyyyMMdd"), ".log");
          fi = new FileInfo(Path.Combine(di.FullName, fName));
          if (fi.Exists)
          {
            sw = fi.AppendText();
          }
          else
          {
            sw = fi.CreateText();
          }

          sb = new StringBuilder();

          sb.AppendFormat("Run @ {0}", date.ToString("dd-MM-yyyy HH:mm:ss"));
          sb.AppendLine();

          sb.AppendLine("Start -> AutoMonitoring");
          sb.AppendLine(this.AutoMonitoring(this.cfg, date));
          sb.AppendLine("End -> AutoMonitoring");

          sw.WriteLine(sb.ToString());
        }
        catch (Exception ex)
        {
          Logger.WriteLine(ex.Message);
          Logger.WriteLine(ex.StackTrace);
        }
        finally
        {
          if (sw != null)
          {
            sw.Close();
            sw.Dispose();
          }

          GC.Collect();
        }
      }

      timerMon.Start();
    }
    
    #endregion

    #region Private

    #region Monitoring

    private void AutoMonitor_PLSnap(ORMDataContext db, DateTime dateNow)
    {
      string queryX = null;
      int nCount = 0;

      try
      {
        #region SP

        queryX = @"
        insert into SCMS_BI_SNAPSHOOT (n_pendingpl, n_pendingdo, n_pendingexp, n_pendingsj, c_gdg, d_snaps)
          select COUNT(0),0,0,0,c_gdg,GETDATE() from LG_PLH where 
l_delete = 0 and l_confirm = 0 
group by c_gdg
        ";

        lock (db)
        {
          nCount = db.ExecuteCommand(queryX);
        }

        #endregion
      }
      catch (Exception ex)
      {
        sbLogHistory.AppendLine(string.Format("ScmsSoaLibrary.Core.Schedule.AutoMonitor:AutoMonitor_PLSnap Insert_Delete - {0}", ex.Message));
        sbLogHistory.AppendLine(ex.StackTrace);

        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }

      if (treMonSP != null)
      {
        treMonSP.Set();
      }
    }

    #region Inner Code

    #endregion

    private void RunAutoMonitoring(object state)
    {
      MonitoringThreadingProses htp = (MonitoringThreadingProses)state;

      if (htp.IsSet)
      {
        switch (htp.TipeProses)
        {
          case MonitoringThreadingProsesEnum.IsPLSnap:
            AutoMonitor_PLSnap(htp.dbCurrent, htp.dateNow);
            break;
        }
      }
    }

    private string AutoMonitoring(Config cfg, DateTime date)
    {
      ORMDataContext db = new ORMDataContext(cfg.ConnectionString);

      StringBuilder sbLog = new StringBuilder();

      bool isOk = false;

      #region Transaksi

      try
      {
        db.Connection.Open();
        db.Transaction = db.Connection.BeginTransaction();
        db.CommandTimeout *= 20;

        isOk = true;
      }
      catch (Exception ex)
      {
        isOk = false;

        sbLog.AppendLine(string.Format("ScmsSoaLibrary.Core.Schedule.AutoRunning:AutoHistory BeginTransaction - {0}", ex.Message));
        sbLog.AppendLine(ex.StackTrace);

        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }

      #endregion

      WaitHandle[] waits = new WaitHandle[1];

      if (isOk)
      {
        #region Moveover

        sbLogHistory = new StringBuilder();

        try
        {
          

          #region Old Coded

          //#region SP

          //sbLog.AppendLine("Running Threading Monitoring SP");

          //treMonSP = new ManualResetEvent(false);
          //waits[0] = treMonSP;
          //ThreadPool.QueueUserWorkItem(RunAutoMonitoring,
          //  new MonitoringThreadingProses()
          //  {
          //    IsSet = true,
          //    dateNow = date,
          //    dbCurrent = db,
          //    TipeProses = MonitoringThreadingProsesEnum.IsSP
          //  });

          //#endregion

          //#region PL

          //sbLog.AppendLine("Running Threading Monitoring PL");

          //treMonPL = new ManualResetEvent(false);
          //waits[1] = treMonPL;
          //ThreadPool.QueueUserWorkItem(RunAutoMonitoring,
          //  new MonitoringThreadingProses()
          //  {
          //    IsSet = true,
          //    dateNow = date,
          //    dbCurrent = db,
          //    TipeProses = MonitoringThreadingProsesEnum.IsPL
          //  });

          //#endregion

          //#region DO

          //sbLog.AppendLine("Running Threading Monitoring DO");

          //treMonDO = new ManualResetEvent(false);
          //waits[2] = treMonDO;
          //ThreadPool.QueueUserWorkItem(RunAutoMonitoring,
          //  new MonitoringThreadingProses()
          //  {
          //    IsSet = true,
          //    dateNow = date,
          //    dbCurrent = db,
          //    TipeProses = MonitoringThreadingProsesEnum.IsDO
          //  });

          //#endregion

          //#region SG

          //sbLog.AppendLine("Running Threading Monitoring SG");

          //treMonSG = new ManualResetEvent(false);
          //waits[3] = treMonSG;
          //ThreadPool.QueueUserWorkItem(RunAutoMonitoring,
          //  new MonitoringThreadingProses()
          //  {
          //    IsSet = true,
          //    dateNow = date,
          //    dbCurrent = db,
          //    TipeProses = MonitoringThreadingProsesEnum.IsSG
          //  });

          //#endregion

          //#region SJ

          //sbLog.AppendLine("Running Threading Monitoring SJ");

          //treMonSJ = new ManualResetEvent(false);
          //waits[4] = treMonSJ;
          //ThreadPool.QueueUserWorkItem(RunAutoMonitoring,
          //  new MonitoringThreadingProses()
          //  {
          //    IsSet = true,
          //    dateNow = date,
          //    dbCurrent = db,
          //    TipeProses = MonitoringThreadingProsesEnum.IsSJ
          //  });

          //#endregion


          #endregion

          #region PL Pending Snapshoot

          sbLog.AppendLine("Running Threading Monitoring PL Pending Snap");

          treMonSP = new ManualResetEvent(false);
          waits[0] = treMonSP;
          ThreadPool.QueueUserWorkItem(RunAutoMonitoring,
            new MonitoringThreadingProses()
            {
              IsSet = true,
              dateNow = date,
              dbCurrent = db,
              TipeProses = MonitoringThreadingProsesEnum.IsPLSnap
            });

          #endregion

          isOk = WaitHandle.WaitAll(waits);

          //isOk = true;
        }
        catch (Exception ex)
        {
          sbLog.AppendLine(string.Format("ScmsSoaLibrary.Core.Schedule.AutoRunning:AutoHistory Insert_Delete - {0}", ex.Message));
          sbLog.AppendLine(ex.StackTrace);

          Logger.WriteLine(ex.Message);
          Logger.WriteLine(ex.StackTrace);
        }
        finally
        {
          #region Close

          Array.Clear(waits, 0, waits.Length);

          if (treMonSP != null)
          {
            treMonSP.Close();
          }
          if (treMonPL != null)
          {
            treMonPL.Close();
          }
          if (treMonDO != null)
          {
            treMonDO.Close();
          }
          if (treMonSG != null)
          {
            treMonSG.Close();
          }
          if (treMonSJ != null)
          {
            treMonSJ.Close();
          }

          #endregion

          sbLog.AppendLine();
          sbLog.AppendLine(sbLogHistory.ToString());

          sbLogHistory.Remove(0, sbLogHistory.Length);
        }

        #endregion
      }

      #region Transaksi

      try
      {
        if (isOk)
        {
          db.SubmitChanges();
          db.Transaction.Commit();
          //db.Transaction.Rollback();
          sbLog.AppendLine("Done...");
        }
        else
        {
          db.Transaction.Rollback();
        }

        isOk = true;
      }
      catch (Exception ex)
      {
        db.Transaction.Rollback();

        isOk = false;

        sbLog.AppendLine(string.Format("ScmsSoaLibrary.Core.Schedule.AutoRunning:AutoHistory Rollback - {0}", ex.Message));
        sbLog.AppendLine(ex.StackTrace);

        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }

      #endregion

      db.Dispose();

      return sbLog.ToString();
    }

    #endregion

    #endregion
  }
}