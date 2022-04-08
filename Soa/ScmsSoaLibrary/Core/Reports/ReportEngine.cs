using System;
using System.Collections.Generic;
using System.Text;
using CrystalDecisions.CrystalReports.Engine;
using System.Data.SqlClient;
using System.IO;
using CrystalDecisions.Shared;
using System.Configuration;
using System.Diagnostics;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaLibrary.Core.Reports
{
  public class ReportEngine : ReportDocument
  {
    private string _reportPath;
    private string _errorResult;
    private bool isDisposed;
    private Config cfg = null;
    
    public struct CrystalReportStructureConfigure
    {
      public string ReportFile;
      public string RecordSelection;
      public string QueryToExecute;
      public ParameterQueryExecute[] ParametersQueryToExecute;
      public bool ReRunQuery;
      /// <summary>
      /// Each entry should be separator by ';'
      /// Ex. [Section Name];[Control Name];[Value]
      /// Each entry column is mandatory
      /// </summary>
      //public string[] CustomText;
      public ScmsSoaLibraryInterface.Components.ReportCustomizeText[] CustomizeTextData;
      public bool isLandscape;
      public string paperName;
      public string outputFolder;
      public string outputName;
      public string extReport;
      public string sizeOutput;
      public string resultMessage;
      public bool IsSet;
      public System.Data.DataSet dataSet;
    }

    public struct ParameterQueryExecute
    {
      public string ParameterName;
      public object Value;
      public System.Data.SqlDbType DataType;
      public int SizeValue;
    }

    public static ParameterQueryExecute[] ConvertToPQE(params SqlParameter[] parameters)
    {
      ParameterQueryExecute[] pqe = null;

      if ((parameters != null) && (parameters.Length > 0))
      {
        pqe = new ParameterQueryExecute[parameters.Length];

        for (int nLoop = 0; nLoop < parameters.Length; nLoop++)
        {
          pqe[nLoop].DataType = parameters[nLoop].SqlDbType;
          pqe[nLoop].ParameterName = parameters[nLoop].ParameterName;
          pqe[nLoop].SizeValue = parameters[nLoop].Size;
          pqe[nLoop].Value = parameters[nLoop].Value;
        }
      }

      return pqe;
    }

    public static SqlParameter[] ConvertToSqlParameter(params ParameterQueryExecute[] pqes)
    {
      SqlParameter[] sqlParams = null;

      if ((pqes != null) && (pqes.Length > 0))
      {
        sqlParams = new SqlParameter[pqes.Length];

        for (int nLoop = 0; nLoop < pqes.Length; nLoop++)
        {
          sqlParams[nLoop].SqlDbType = pqes[nLoop].DataType;
          sqlParams[nLoop].ParameterName = pqes[nLoop].ParameterName;
          sqlParams[nLoop].Size = pqes[nLoop].SizeValue;
          sqlParams[nLoop].Value = pqes[nLoop].Value;
        }
      }

      return sqlParams;
    }

    public ReportEngine(string reportPath ,Config config)
      : base()
    {
      this._reportPath = reportPath;

      this.cfg = config;

      //base.InitReport += new EventHandler(CrystalReportMapper_InitReport);
      //base.RefreshReport += new EventHandler(CrystalReportMapper_RefreshReport);
    }

    ~ReportEngine()
    {
      //base.RefreshReport -= new EventHandler(CrystalReportMapper_RefreshReport);
      //base.InitReport -= new EventHandler(CrystalReportMapper_InitReport);
    }

    private void FixReportOdbc(Database database)
    {
      Table table = null;
      TableLogOnInfo tli = null;
      try
      {
        for (int nLoop = 0; nLoop < database.Tables.Count; nLoop++)
        {
          table = database.Tables[nLoop];
          tli = table.LogOnInfo;

          tli.ConnectionInfo.AllowCustomConnection = true;
          tli.ConnectionInfo.DatabaseName = cfg.Database;
          tli.ConnectionInfo.ServerName = cfg.SqlServerReporting;
          if (cfg.Integrated)
          {
            tli.ConnectionInfo.IntegratedSecurity = true;
          }
          else
          {
            tli.ConnectionInfo.UserID = cfg.User;
            tli.ConnectionInfo.Password = cfg.Password;
          }
          tli.ConnectionInfo.Type = ConnectionInfoType.CRQE;

          table.ApplyLogOnInfo(tli);
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibrary.Core.Reports.ReportEngine:FixReportOdbc - {0}", ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }

      #region Old Coded

      //string cnString = null;
      //if (ConfigurationManager.ConnectionStrings["MyDbConnReport"] != null)
      //{
      //  cnString = ConfigurationManager.ConnectionStrings["MyDbConnReport"].ConnectionString;
      //}

      //if (!string.IsNullOrEmpty(cnString))
      //{
      //  SqlConnectionStringBuilder sqlBuild = new SqlConnectionStringBuilder(cnString);

      //  Table table = null;
      //  TableLogOnInfo tli = null;
      //  try
      //  {
      //    for (int nLoop = 0; nLoop < database.Tables.Count; nLoop++)
      //    {
      //      table = database.Tables[nLoop];
      //      tli = table.LogOnInfo;

      //      tli.ConnectionInfo.AllowCustomConnection = true;
      //      tli.ConnectionInfo.DatabaseName = sqlBuild.InitialCatalog;
      //      tli.ConnectionInfo.ServerName = sqlBuild.DataSource;
      //      if (!sqlBuild.IntegratedSecurity)
      //      {
      //        tli.ConnectionInfo.UserID = sqlBuild.UserID;
      //        tli.ConnectionInfo.Password = sqlBuild.Password;
      //      }
      //      else
      //      {
      //        tli.ConnectionInfo.IntegratedSecurity = sqlBuild.IntegratedSecurity;
      //      }
      //      tli.ConnectionInfo.Type = ConnectionInfoType.CRQE;

      //      table.ApplyLogOnInfo(tli);
      //    }
      //  }
      //  catch { ; }
      //}

      #endregion
    }

    new public void Dispose()
    {
      if (!isDisposed)
      {
        isDisposed = true;

        this.Close();

        base.Dispose();
      }
    }

    new protected void Clone()
    {
      return;
    }

    new public void Close()
    {
      if (base.IsLoaded)
      {
        base.Close();
      }
    }

    public bool IsError
    { get { return string.IsNullOrEmpty(_errorResult); } }

    public string ErrorMessage
    { get { return _errorResult; } }

    public bool LoadReport()
    {
      return LoadReport(null);
    }

    public bool LoadReport(System.Data.DataSet dataset)
    {
      bool ret = false;

      try
      {
        if (File.Exists(_reportPath))
        {
          base.Load(_reportPath, OpenReportMethod.OpenReportByTempCopy);

          if (dataset == null)
          {
            this.FixReportOdbc(base.Database);
          }
          else
          {
            this.SetDataSource(dataset);
          }

          ret = true;
        }
        else
        {
          throw new FileNotFoundException(
            string.Format("Report '{0}' not found", _reportPath));
        }
      }
      catch (Exception ex)
      {
        if (base.IsLoaded)
        {
          base.Close();
          base.Dispose();
        }

        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);

        _errorResult = ex.Message;
      }

      return ret;
    }

    public override void Refresh()
    {
      base.Refresh();
    }

    public MemoryStream PopulateReportToStream(ExportFormatType exportType)
    {
      if (!base.IsLoaded)
      {
        _errorResult = "Report not loaded yet.";

        return null;
      }

      _errorResult = string.Empty;

      MemoryStream memory = null;

      try
      {
        base.Refresh();

        memory = (MemoryStream)base.ExportToStream(exportType);
      }
      catch (Exception ex)
      {
        _errorResult = ex.Message;

        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }

      return memory;
    }

    public string PopulateReportToDisk(ExportFormatType exportType, string pathFile, string savedFile)
    {
      if (!base.IsLoaded)
      {
        _errorResult = "Report not loaded yet.";

        return string.Empty;
      }

      _errorResult = string.Empty;

      string ret = null;

      string sFile = Path.Combine(pathFile, savedFile);

      if (!string.IsNullOrEmpty(savedFile))
      {
        FileInfo fi = new FileInfo(sFile);

        ret = fi.Name;

        if (fi.Exists)
        {
          try
          {
            fi.Delete();
          }
          catch (Exception ex)
          {
            _errorResult = string.Format("ScmsSoaLibrary.Core.Reports.ReportEngine:PopulateReportToDisk - {0}", ex.Message);

            Logger.WriteLine("ScmsSoaLibrary.Core.Reports.ReportEngine:PopulateReportToDisk - {0}", ex.Message);
            Logger.WriteLine(ex.StackTrace);
          }
        }
      }
      else
      {
        Random rand = new Random((int)DateTime.Now.Ticks);

        do
        {
          ret = rand.Next(int.MinValue, int.MaxValue).ToString("X08");
          sFile = Path.Combine(pathFile, ret);
        } while (File.Exists(sFile));
      }

      try
      {
        base.Refresh();

        base.ExportToDisk(exportType, sFile);
      }
      catch (Exception ex)
      { 
        _errorResult = string.Format("ScmsSoaLibrary.Core.Reports.ReportEngine:PopulateReportToDisk Export - {0}", ex.Message);

        Logger.WriteLine(_errorResult);
        Logger.WriteLine(ex.StackTrace);

        ret = null;
      }

      return ret;
    }
  }
}