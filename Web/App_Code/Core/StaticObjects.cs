using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Scms.Web.Common;
using System.Configuration;

namespace Scms.Web.Core
{
  /// <summary>
  /// Summary description for StaticObjects
  /// </summary>
  public class StaticObjects
  {
    public StaticObjects(HttpApplication app)
    {
      //
      // TODO: Add constructor logic here
      //
      ReadAll(app);
    }

    private void ReadAll(HttpApplication app)
    {
      string tmp = null;
      string result = null;
      int iTmp = 0;

      #region Read Menu Configuration

      tmp = app.Server.MapPath("~/App_Data/Menu/Menu.xml");

      result = this.ReadFile(tmp);

      MenuConfiguration = (string.IsNullOrEmpty(result) ? string.Empty : result);

      #endregion

      #region Read Right Configuration

      tmp = app.Server.MapPath("~/App_Data/Menu/Management.xml");

      result = this.ReadFile(tmp);

      RightConfiguration = (string.IsNullOrEmpty(result) ? string.Empty : result);

      #endregion

      #region Read All App Settings

      SoaAddress = (ConfigurationManager.AppSettings["soaAddress"] ?? string.Empty).Trim();

      tmp = (ConfigurationManager.AppSettings["soaPort"] ?? string.Empty);
      if (int.TryParse(tmp, out iTmp))
      {
        SoaPort = (iTmp < 1 ? 1234 : iTmp);
      }
      else
      {
        SoaPort = 1234;
      }

      #endregion
    }

    #region Private

    private string ReadFile(string filePath)
    {
      if (string.IsNullOrEmpty(filePath))
        return null;

      string result = null;

      FileInfo fi = new FileInfo(filePath);

      StreamReader swRead = null;

      try
      {
        swRead = fi.OpenText();

        result = swRead.ReadToEnd();
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          "Scms.Web.Core.StaticObjects:ReadFile - {0}", ex.Message);
      }
      finally
      {
        if (swRead != null)
        {
          try
          {
            swRead.Close();
            swRead.Dispose();
          }
          catch (Exception ex)
          {
            Logger.WriteLine(
              "Scms.Web.Core.StaticObjects:ReadFile SubClose - {0}", ex.Message);
          }
        }
      }

      return result;
    }

    private static string ReadFileStatic(string filePath)
    {
      if (string.IsNullOrEmpty(filePath))
        return null;

      string result = null;

      FileInfo fi = new FileInfo(filePath);

      StreamReader swRead = null;

      try
      {
        swRead = fi.OpenText();

        result = swRead.ReadToEnd();
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          "Scms.Web.Core.StaticObjects:ReadFile - {0}", ex.Message);
      }
      finally
      {
        if (swRead != null)
        {
          try
          {
            swRead.Close();
            swRead.Dispose();
          }
          catch (Exception ex)
          {
            Logger.WriteLine(
              "Scms.Web.Core.StaticObjects:ReadFile SubClose - {0}", ex.Message);
          }
        }
      }

      return result;
    }

    #endregion

    #region Static

    public static string MenuConfiguration
    {
      get
      {
        string tmp = HttpContext.Current.Server.MapPath("~/App_Data/Menu/Menu.xml");

        return ReadFileStatic(tmp);
      }
      private set
      {
        ;
      }
    }

    public static string RightConfiguration
    {
      get
      {
        string tmp = HttpContext.Current.Server.MapPath("~/App_Data/Menu/Management.xml");

        return ReadFileStatic(tmp);
      }
      private set
      {
        ;
      }
    }

    public static string SoaAddress
    { get; private set; }

    public static int SoaPort
    { get; private set; }

    #endregion
  }
}