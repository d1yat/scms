using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScmsSoaLibraryInterface.Commons
{
  /// <summary>
  /// Summary description for Logger
  /// </summary>
  public static class Logger
  {
    public static void WriteLine(string message)
    {
      //Console.WriteLine(message);
    }

    public static void WriteLine(string format, params object[] args)
    {
      string result = string.Format(format, args);

      //Console.WriteLine(result);
    }

    public static void WriteLine(string message, bool isStatus)
    {
      //string path = AppDomain.CurrentDomain.BaseDirectory;

      //string spath = string.Concat(path, "\\Temp\\log\\Error Log\\");

      //string sFileName = string.Concat(DateTime.Today.ToShortDateString(), ".txt");

      //sFileName = sFileName.Replace("/", "");

      //string sLocat = string.Concat(spath, sFileName);

      //System.IO.FileStream fs = null;
      //if (!System.IO.File.Exists(sLocat))
      //{
      //  fs = System.IO.File.Create(sLocat);

      //  fs.Dispose();
      //}

      

      //if (isStatus)
      //{
      //  Console.WriteLine(message);

      //  if (System.IO.File.Exists(sLocat))
      //  {
      //    System.IO.FileStream fsi = new System.IO.FileStream(sLocat, System.IO.FileMode.Append, System.IO.FileAccess.Write);
      //    System.IO.StreamWriter sw = new System.IO.StreamWriter(fsi);
      //    //sw.WriteLine(Environment.NewLine);
      //    sw.Flush();
      //    sw.WriteLine(message + " Tanngal : {0}." , DateTime.Now);
      //    sw.Close();
      //  }
      //}
    }
  }
}