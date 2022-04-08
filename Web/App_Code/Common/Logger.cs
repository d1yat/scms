using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scms.Web.Common
{
  /// <summary>
  /// Summary description for Logger
  /// </summary>
  public static class Logger
  {
    public static void WriteLine(string message)
    {
      System.Diagnostics.Debug.WriteLine(message);
    }

    public static void WriteLine(string format, params object[] args)
    {
      string result = string.Format(format, args);

      System.Diagnostics.Debug.WriteLine(result);
    }
  }
}