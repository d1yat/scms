using System;
using System.Collections.Generic;
using System.Data;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaLibrary.Commons
{
  public static class ForDataRow
  {
    public static T GetValue<T>(this DataRow row, string fieldName)
    {
      return GetValue<T>(row, fieldName, default(T));
    }

    public static T GetValue<T>(this DataRow row, string fieldName, T defaultValue)
    {
      T result = default(T);

      try
      {
        if ((defaultValue != null) && defaultValue.Equals(default(T)))
        {
          //result = (row.IsNull(fieldName) ? default(T) : (T)row[fieldName]);
          result = (row.IsNull(fieldName) ? default(T) : (T)Convert.ChangeType(row[fieldName], typeof(T)));
        }
        else
        {
          result = (row.IsNull(fieldName) ? defaultValue : (T)Convert.ChangeType(row[fieldName], typeof(T)));
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          string.Format("ScmsSoaLibrary.Commons.Extensions:ForDataRow:GetValue<T>(row, FieldName, defaultValue) - {0}", ex.Message));
        Logger.WriteLine(ex.StackTrace);

        if ((defaultValue != null) && (!defaultValue.Equals(default(T))))
        {
          result = defaultValue;
        }
      }

      return result;
    }
    
    public static T GetValue<T>(this DataRow row, DataColumn column)
    {
      return GetValue<T>(row, column, default(T));
    }

    public static T GetValue<T>(this DataRow row, DataColumn column, T defaultValue)
    {
      T result = default(T);

      try
      {
        if ((defaultValue != null) && defaultValue.Equals(default(T)))
        {
          //result = (row.IsNull(column) ? default(T) : (T)row[column]);
          result = (row.IsNull(column) ? default(T) : (T)Convert.ChangeType(row[column], typeof(T)));
        }
        else
        {
          //result = (row.IsNull(column) ? defaultValue : (T)row[column]);
          result = (row.IsNull(column) ? defaultValue : (T)Convert.ChangeType(row[column], typeof(T)));
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          string.Format("ScmsSoaLibrary.Commons.Extensions:ForDataRow:GetValue<T>(row, FieldName, defaultValue) - {0}", ex.Message));
        Logger.WriteLine(ex.StackTrace);

        if ((defaultValue != null) && (!defaultValue.Equals(default(T))))
        {
          result = defaultValue;
        }
      }

      return result;
    }
  }

  public static class ForICollection
  {
    public static List<T> CollectionToList<T>(this System.Collections.Generic.ICollection<T> icol)
    {
      //List<T> list = new List<T>(icol.Count);
      //list.AddRange(icol.Cast<T>());

      List<T> list = new List<T>(icol);

      return list;
    }
  }

  public static class ForIDictionary
  {
    public static TResult GetValueParser<TKey, TValue, TResult>(this IDictionary<TKey, TValue> dic, TKey key)
    {
      return GetValueParser<TKey, TValue, TResult>(dic, key, default(TResult));
    }

    public static TResult GetValueParser<TKey, TValue, TResult>(this IDictionary<TKey, TValue> dic, TKey key, TResult defaultValue)
    {
      TResult result = default(TResult);
      TValue val = default(TValue);

      try
      {
        if (dic.ContainsKey(key))
        {
          val = dic[key];


          if (EqualityComparer<TValue>.Default.Equals(val, default(TValue)))
          {
            if (EqualityComparer<TResult>.Default.Equals(defaultValue, default(TResult)))
            {
              result = defaultValue;
            }
            else
            {
              result = default(TResult);
            }
          }
          else
          {
            result = (TResult)Convert.ChangeType(val, typeof(TResult));
          }
        }
        else
        {
          if ((defaultValue != null) && (!defaultValue.Equals(default(TResult))))
          {
            result = defaultValue;
          }
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          string.Format("ScmsSoaLibrary.Commons.Extensions:ForIDictionary:GetValueParser<T>(dic, key, defaultValue) - {0}", ex.Message));
        Logger.WriteLine(ex.StackTrace);

        if ((defaultValue != null) && (!defaultValue.Equals(default(TResult))))
        {
          result = defaultValue;
        }
      }

      return result;
    }
  }
  
  public static class ForKeyValuePair
  {
    public static TResult GetValueParser<TKey, TValue, TResult>(this KeyValuePair<TKey, TValue> kvp)
    {
      return GetValueParser<TKey, TValue, TResult>(kvp, default(TResult));
    }

    public static TResult GetValueParser<TKey, TValue, TResult>(this KeyValuePair<TKey, TValue> kvp, TResult defaultValue)
    {
      TResult result = default(TResult);
      TValue val = default(TValue);

      try
      {
        val = kvp.Value;

        if ((val != null) && (val.Equals(default(TValue))))
        {
          if ((defaultValue != null) && (!defaultValue.Equals(result)))
          {
            result = (TResult)Convert.ChangeType(kvp.Value, typeof(TResult));
          }
        }
        else
        {
          result = (TResult)Convert.ChangeType(kvp.Value, typeof(TResult));
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          string.Format("ScmsSoaLibrary.Commons.Extensions:ForIDictionary:GetValueParser<T>(dic, key, defaultValue) - {0}", ex.Message));
        Logger.WriteLine(ex.StackTrace);

        if ((defaultValue != null) && (!defaultValue.Equals(default(TResult))))
        {
          result = defaultValue;
        }
      }

      return result;
    }
  }
}
