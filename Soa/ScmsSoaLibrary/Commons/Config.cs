using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Diagnostics;

namespace ScmsSoaLibrary.Commons
{
  public class Config
  {
    private void InitConfig()
    {
      if (ConfigurationManager.AppSettings.Count > 0)
      {
        bool bParse = false;
        string tmp = null;
        TimeSpan ts = TimeSpan.MinValue;
        int nTmp = 0;

        try
        {
          this.SqlServer = (ConfigurationManager.AppSettings["server"] == null ? string.Empty : ConfigurationManager.AppSettings["server"]);
          this.SqlServerBackup = (ConfigurationManager.AppSettings["serverFailover"] == null ? string.Empty : ConfigurationManager.AppSettings["serverFailover"]);
          this.Database = (ConfigurationManager.AppSettings["contextData"] == null ? string.Empty : ConfigurationManager.AppSettings["contextData"]);
          this.DatabaseHistory = (ConfigurationManager.AppSettings["contextDataHistory"] == null ? string.Empty : ConfigurationManager.AppSettings["contextDataHistory"]);
          this.SqlServerReporting = (ConfigurationManager.AppSettings["serverReporting"] == null ? string.Empty : ConfigurationManager.AppSettings["serverReporting"]);

          if (ConfigurationManager.AppSettings["integrated"] != null)
          {
            bool.TryParse(ConfigurationManager.AppSettings["integrated"], out bParse);
          }
          this.Integrated = bParse;//( == null ? false : bool.Parse(ConfigurationManager.AppSettings["server"]));

          if (ConfigurationManager.AppSettings["activeAsHistory"] != null)
          {
            bool.TryParse(ConfigurationManager.AppSettings["activeAsHistory"], out bParse);
          }
          this.IsHistoryData = bParse;

          if (ConfigurationManager.AppSettings["activeBackupPOSend"] != null)
          {
            bool.TryParse(ConfigurationManager.AppSettings["activeBackupPOSend"], out bParse);
          }
          this.IsActiveBackupPOSend = bParse;

          if (ConfigurationManager.AppSettings["activeBackupRCSend"] != null)
          {
              bool.TryParse(ConfigurationManager.AppSettings["activeBackupRCSend"], out bParse);
          }
          this.IsActiveBackupRCSend = bParse;

          if (ConfigurationManager.AppSettings["activeAutoMonitoring"] != null)
          {
            bool.TryParse(ConfigurationManager.AppSettings["activeAutoMonitoring"], out bParse);
          }
          this.IsActiveMonitoring = bParse;

          if (ConfigurationManager.AppSettings["activeSendRCAuto"] != null)
          {
              bool.TryParse(ConfigurationManager.AppSettings["activeSendRCAuto"], out bParse);
          }
          this.IsActiveSendRCAuto = bParse;

          if (ConfigurationManager.AppSettings["activeSPMail"] != null)
          {
              bool.TryParse(ConfigurationManager.AppSettings["activeSPMail"], out bParse);
          }
          this.IsActiveSPMail = bParse;

          this.User = (ConfigurationManager.AppSettings["user"] == null ? string.Empty : ConfigurationManager.AppSettings["user"]);
          this.Password = (ConfigurationManager.AppSettings["password"] == null ? string.Empty : ConfigurationManager.AppSettings["password"]);
          this.BaseUri = (ConfigurationManager.AppSettings["ClientSettingsProvider.ServiceUri"] == null ? string.Empty : ConfigurationManager.AppSettings["ClientSettingsProvider.ServiceUri"]);
          this.BaseUriReporting = (ConfigurationManager.AppSettings["ClientSettingsProvider.ServiceUri.Reporting"] == null ? string.Empty : ConfigurationManager.AppSettings["ClientSettingsProvider.ServiceUri.Reporting"]);
          this.PathReportStorage = (ConfigurationManager.AppSettings["pathReportStorage"] == null ? string.Empty : ConfigurationManager.AppSettings["pathReportStorage"]);
          this.PathPrintStorageLat1 = (ConfigurationManager.AppSettings["PrinterLT1"] == null ? string.Empty : ConfigurationManager.AppSettings["PrinterLT1"]);
          this.PathPrintStorageLat2 = (ConfigurationManager.AppSettings["PrinterLT2"] == null ? string.Empty : ConfigurationManager.AppSettings["PrinterLT2"]);
          this.PathPrintStorageLat3 = (ConfigurationManager.AppSettings["PrinterLT3"] == null ? string.Empty : ConfigurationManager.AppSettings["PrinterLT3"]);
          this.PathPrintStorageLat4 = (ConfigurationManager.AppSettings["PrinterLT4"] == null ? string.Empty : ConfigurationManager.AppSettings["PrinterLT4"]);
          this.PathReport = (ConfigurationManager.AppSettings["pathReport"] == null ? string.Empty : ConfigurationManager.AppSettings["pathReport"]);
          this.PathTempExtract = (ConfigurationManager.AppSettings["pathTempExtract"] == null ? string.Empty : ConfigurationManager.AppSettings["pathTempExtract"]);
          this.PathTempLog = (ConfigurationManager.AppSettings["pathTempLog"] == null ? string.Empty : ConfigurationManager.AppSettings["pathTempLog"]);
          this.PathTempExtractMail = (ConfigurationManager.AppSettings["pathTempExtractMail"] == null ? string.Empty : ConfigurationManager.AppSettings["pathTempExtractMail"]);
          this.PathRar = (ConfigurationManager.AppSettings["pathRar"] == null ? string.Empty : ConfigurationManager.AppSettings["pathRar"]);

          tmp = (ConfigurationManager.AppSettings["timerPeriodicMailer"] == null ? string.Empty : ConfigurationManager.AppSettings["timerPeriodicMailer"]);
          if (TimeSpan.TryParse(tmp, out ts))
          {
            this.TimerPeriodicMailer = ts;
          }
          else
          {
            this.TimerPeriodicMailer = new TimeSpan(0, 15, 0);
          }

          tmp = (ConfigurationManager.AppSettings["timerPeriodicMonitoring"] == null ? string.Empty : ConfigurationManager.AppSettings["timerPeriodicMonitoring"]);
          if (TimeSpan.TryParse(tmp, out ts))
          {
            this.TimerPeriodicMonitoring = ts;
          }
          else
          {
            this.TimerPeriodicMonitoring = new TimeSpan(0, 5, 0);
          }

          tmp = (ConfigurationManager.AppSettings["pop3DOPharosPort"] == null ? string.Empty : ConfigurationManager.AppSettings["pop3DOPharosPort"]);
          if(!int.TryParse(tmp, out nTmp))
          {
            nTmp = 110;
          }
          tmp = (ConfigurationManager.AppSettings["pop3DOPharosHost"] == null ? string.Empty : ConfigurationManager.AppSettings["pop3DOPharosHost"]);
          this.POP3DOPharosEP = (System.Net.EndPoint)new System.Net.IPEndPoint(System.Net.IPAddress.Parse(tmp), nTmp);

          tmp = (ConfigurationManager.AppSettings["discorePort"] == null ? string.Empty : ConfigurationManager.AppSettings["discorePort"]);
          if(!int.TryParse(tmp, out nTmp))
          {
            nTmp = 80;
          }
          tmp = (ConfigurationManager.AppSettings["discoreHost"] == null ? string.Empty : ConfigurationManager.AppSettings["discoreHost"]);
          this.DiscoreEP = (System.Net.EndPoint)new System.Net.IPEndPoint(System.Net.IPAddress.Parse(tmp), nTmp);

          this.IsConfigurated = true;
        }
        catch (Exception ex)
        {
          ScmsSoaLibraryInterface.Commons.Logger.WriteLine(ex.Message);
          ScmsSoaLibraryInterface.Commons.Logger.WriteLine(ex.StackTrace);
        }
      }

      System.Data.SqlClient.SqlConnectionStringBuilder sqlConnBuild = new System.Data.SqlClient.SqlConnectionStringBuilder()
      {
        AsynchronousProcessing = true,
        DataSource = this.SqlServer,
        ConnectTimeout = 1000,
        FailoverPartner = this.SqlServerBackup,
        InitialCatalog = this.Database,
        IntegratedSecurity = this.Integrated,
        UserID = this.User,
        MultipleActiveResultSets = true,
        MaxPoolSize = 1000,
        MinPoolSize = 100,
        Password = this.Password,
        PersistSecurityInfo = true,
        Pooling = true,
        Replication = true
      };
      this.ConnectionString = sqlConnBuild.ConnectionString;

      sqlConnBuild.InitialCatalog = this.DatabaseHistory;
      this.ConnectionStringHistory = sqlConnBuild.ConnectionString;
    }

    public Config()
    {
      //ConfigurationManager.AppSettings
      InitConfig();
    }

    public bool IsConfigurated
    { get; private set; }

    public bool IsHistoryData
    { get; private set; }

    public bool IsActiveBackupPOSend
    { get; private set; }

    public bool IsActiveBackupRCSend
    { get; private set; }

    public bool IsActiveMonitoring
    { get; private set; }

    public bool IsActiveSendRCAuto
    { get; private set; }

    public bool IsActiveSPMail
    { get; private set; }
        
    public string BaseUri
    { get; private set; }

    public string BaseUriReporting
    { get; private set; }

    public string SqlServer
    { get; private set; }

    public string SqlServerBackup
    { get; private set; }

    public string Database
    { get; private set; }

    public string DatabaseHistory
    { get; private set; }

    public bool Integrated
    { get; private set; }

    public string User
    { get; private set; }

    public string Password
    { get; private set; }

    public string ConnectionString
    { get; private set; }

    public string ConnectionStringHistory
    { get; private set; }

    #region Old Coded

    //public string ConnectionString
    //{
    //  get
    //  {
    //    System.Data.SqlClient.SqlConnectionStringBuilder sqlConnBuild = new System.Data.SqlClient.SqlConnectionStringBuilder()
    //    {
    //      AsynchronousProcessing = true,
    //      DataSource = this.SqlServer,
    //      ConnectTimeout = 1000,
    //      FailoverPartner = this.SqlServerBackup,
    //      InitialCatalog = this.Database,
    //      IntegratedSecurity = this.Integrated,
    //      UserID = this.User,
    //      MultipleActiveResultSets = true,
    //      MaxPoolSize = 1000,
    //      MinPoolSize = 100,
    //      Password = this.Password,
    //      PersistSecurityInfo = true,
    //      Pooling = true,
    //      Replication = true
    //    };

    //    return sqlConnBuild.ConnectionString;
    //  }
    //}

    //public string ConnectionStringHistory
    //{
    //  get
    //  {
    //    System.Data.SqlClient.SqlConnectionStringBuilder sqlConnBuild = new System.Data.SqlClient.SqlConnectionStringBuilder()
    //    {
    //      AsynchronousProcessing = true,
    //      DataSource = this.SqlServer,
    //      ConnectTimeout = 1000,
    //      FailoverPartner = this.SqlServerBackup,
    //      InitialCatalog = this.DatabaseHistory,
    //      IntegratedSecurity = this.Integrated,
    //      UserID = this.User,
    //      MultipleActiveResultSets = true,
    //      MaxPoolSize = 1000,
    //      MinPoolSize = 100,
    //      Password = this.Password,
    //      PersistSecurityInfo = true,
    //      Pooling = true,
    //      Replication = true
    //    };

    //    return sqlConnBuild.ConnectionString;
    //  }
    //}

    #endregion

    public string ConnectionStringReporting
    {
      get
      {
        System.Data.SqlClient.SqlConnectionStringBuilder sqlConnBuild = new System.Data.SqlClient.SqlConnectionStringBuilder()
        {
          AsynchronousProcessing = true,
          DataSource = this.SqlServerReporting,
          ConnectTimeout = 1000,
          InitialCatalog = this.Database,
          IntegratedSecurity = this.Integrated,
          UserID = this.User,
          MultipleActiveResultSets = true,
          MaxPoolSize = 1000,
          MinPoolSize = 100,
          Password = this.Password,
          PersistSecurityInfo = true,
          Pooling = true,
          Replication = true
        };

        return sqlConnBuild.ConnectionString;
      }
    }

    public string PathReportStorage
    { get; private set; }

    public string PathPrintStorageLat1
    { get; private set; }

    public string PathPrintStorageLat2
    { get; private set; }

    public string PathPrintStorageLat3
    { get; private set; }

    public string PathPrintStorageLat4
    { get; private set; }

    public string PathReport
    { get; private set; }

    public string PathTempExtract
    { get; private set; }

    public string PathTempExtractMail
    { get; private set; }

    public string PathTempLog
    { get; private set; }

    public string PathRar
    { get; private set; }

    public TimeSpan TimerPeriodicMailer
    { get; private set; }

    public TimeSpan TimerPeriodicMonitoring
    { get; private set; }

    public System.Net.EndPoint POP3DOPharosEP
    { get; private set; }

    public string SqlServerReporting
    { get; set; }

    public System.Net.EndPoint DiscoreEP
    { get; private set; }

    public override string ToString()
    {
      //return base.ToString();
      StringBuilder strBuild = new StringBuilder();

      strBuild.AppendFormat("BaseUri : {0}", this.BaseUri);
      strBuild.AppendLine();
      strBuild.AppendFormat("SqlServer : {0}", this.SqlServer);
      strBuild.AppendLine();
      strBuild.AppendFormat("Database : {0}", this.Database);
      strBuild.AppendLine();
      strBuild.AppendFormat("Integrated : {0}", this.Integrated);
      strBuild.AppendLine();
      strBuild.AppendFormat("User : {0}", this.User);
      strBuild.AppendLine();
      strBuild.AppendFormat("Password : {0}", this.Password);
      strBuild.AppendLine();

      return strBuild.ToString();
    }
  }
}
