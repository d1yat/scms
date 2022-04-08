using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Scms.Web.Common;
using System.Collections.Specialized;

namespace Scms.Web.Common
{
  /// <summary>
  /// Summary description for GlobalParser
  /// </summary>
  public static class GlobalParser
  {
    public static T GetValueParser<T>(this Dictionary<string, object> dictionary, string key)
    {
      return GetValueParser(dictionary, key, default(T));
    }

    public static T GetValueParser<T>(this IDictionary<string, object> dic, string Key, T defaultValue)
    {
      if (dic == null)
      {
        return default(T);
      }

      T val = default(T);

      if (dic.ContainsKey(Key))
      {
        try
        {
          if (dic[Key] == null)
          {
            if (!EqualityComparer<T>.Default.Equals(val, defaultValue))
            {
              val = defaultValue;
            }
          }
          else
          {
            val = (T)Convert.ChangeType(dic[Key], typeof(T));
          }
        }
        catch (Exception ex)
        {
          Logger.WriteLine("Scms.Web.Common.Extension:GetValue<T>(this IDictionary<string, object> dic, string Key, T defaultValue) - {0}", ex.Message);
        }
      }
      else
      {
        val = (EqualityComparer<T>.Default.Equals(val, defaultValue) ?
              default(T) : defaultValue);
      }

      return val;
    }

    public static T GetValueParser<T>(this Dictionary<string, string> dictionary, string key)
    {
      return GetValueParser(dictionary, key, default(T));
    }

    public static T GetValueParser<T>(this IDictionary<string, string> dic, string Key, T defaultValue)
    {
      if (dic == null)
      {
        return default(T);
      }

      T val = default(T);

      if (dic.ContainsKey(Key))
      {
        try
        {
          if (dic[Key] == null)
          {
            if (!EqualityComparer<T>.Default.Equals(val, defaultValue))
            {
              val = defaultValue;
            }
          }
          else
          {
            val = (T)Convert.ChangeType(dic[Key], typeof(T));
          }
        }
        catch (Exception ex)
        {
          Logger.WriteLine("Scms.Web.Common.Extension:GetValue<T>(this IDictionary<string, string> dic, string Key, T defaultValue) - {0}", ex.Message);
        }
      }
      else
      {
        val = (EqualityComparer<T>.Default.Equals(val, defaultValue) ?
              default(T) : defaultValue);
      }

      return val;
    }

    public static bool IsContainKey(this NameValueCollection nvc, string Key)
    {
      if ((nvc == null) || string.IsNullOrEmpty(Key))
      {
        return false;
      }

      return (nvc.Get(Key) == null ? false : true);
    }
  }
}