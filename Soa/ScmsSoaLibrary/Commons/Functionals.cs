using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using ScmsSoaLibraryInterface.Core.Crypto;
using System.Data.SqlClient;
using ScmsSoaLibrary.Core.Reports;
using System.IO;
using ScmsSoaLibraryInterface.Components;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaLibrary.Commons
{
  public static class Functionals
  {
    internal class PL1
    {
      public string c_plno { get; set; }
      public string c_iteno { get; set; }
      public string c_spno { get; set; }
      public string c_type { get; set; }
      public string c_batch { get; set; }
      public decimal n_booked { get; set; }
      public decimal n_qty { get; set; }
      public decimal n_sisa { get; set; }
    }

    private static IDictionary<string, int> dicPaperKind;

    private static Random randNumber;

    private static Config cfgConfig;

    public static Config Configuration
    {
      get
      {
        if (cfgConfig == null)
        {
          cfgConfig = new Config();
        }

        return cfgConfig;
      }
    }

    public static bool DateParser(string raw, string format, out DateTime date)
    {
      CultureInfo culture = new CultureInfo("id-ID");

      DateTime dat = DateTime.Today;

      bool isOk = false;

      if (DateTime.TryParseExact(raw, format, culture, DateTimeStyles.AssumeLocal, out dat))
      {
        date = dat;

        isOk = true;
      }
      else
      {
        date = DateTime.MinValue;
      }

      return isOk;
    }

    public static string ActiveConnectionString
    {
      get
      {
        return (Configuration.IsHistoryData ? Configuration.ConnectionStringHistory : Configuration.ConnectionString);
      }
    }

    public struct ParameterParser
    {
      public bool IsSet;
      public object Value;
      public object Value_Next;
      public bool IsLike;
      public bool IsLikeOrNull;
      public bool IsLikeRight;
      public bool IsLikeLeft;
      public bool IsLikeBoth;
      public bool IsBetween;
      public bool IsCondition;
      public bool IsIn;
      public Type TypeOf;
    }

    public struct ReportingGeneratorResult
    {
      public string OutputPath;
      public string OutputFile;
      public string Extension;
      public string Size;
      public bool IsSuccess;
      public string Messages;
      public bool IsSet;
    }

    public static IDictionary<string, ParameterParser> ParserArrayParameter(string[][] parameters)
    {
      const string SYMBOLIC_SPLITER = "@";

      IDictionary<string, ParameterParser> dic = new Dictionary<string, ParameterParser>(StringComparer.OrdinalIgnoreCase);

      const string LIKE_OPERATOR = "%";
      const string NULL_OPERATOR = "!|";

      const string IN_OPERATOR = "@in.";
      const string CONTAINS_OPERATOR = "@contains.";
      const string STARTWITH_OPERATOR = "@startwith.";
      const string ENDWITH_OPERATOR = "@endwith.";

      if ((parameters != null) && (parameters.Length > 0))
      {
        string[] param = null;
        Type typ = null;
        string[] splt = null;
        bool isCondition = false,
          isInCodition = false;
        string nullLike = null;
        string tmpX = null;
        Newtonsoft.Json.Linq.JArray jarr = null;

        for (int nLoop = 0; nLoop < parameters.Length; nLoop++)
        {
          param = parameters[nLoop];

          if ((param != null) && (param.Length > 2) && (!string.IsNullOrEmpty(param[0])) && (!string.IsNullOrEmpty(param[1])))
          {
            #region Parser
            
            isInCodition =
              isCondition = false;

            try
            {
              // Comparasion
              // =, ==, !=, <>, >, >=, <, <=
              isCondition = (!((!param[0].Contains("=")) && (!param[0].Contains("==")) && 
                (!param[0].Contains("!=")) && (!param[0].Contains("<>")) && 
                (!param[0].Contains(">")) && (!param[0].Contains(">=")) &&
                (!param[0].Contains("<")) && (!param[0].Contains("<=")) &&
                (!param[1].Contains(LIKE_OPERATOR))));

              tmpX = (param[0] ?? string.Empty);

              if (!string.IsNullOrEmpty(tmpX))
              {
                if (tmpX.StartsWith(IN_OPERATOR, StringComparison.OrdinalIgnoreCase))
                {
                  isInCodition = true;

                  param[0] = tmpX.Replace(IN_OPERATOR, string.Empty).Trim();
                }
                else if (tmpX.StartsWith(STARTWITH_OPERATOR, StringComparison.OrdinalIgnoreCase))
                {
                  isCondition = true;

                  param[0] = tmpX.Replace(STARTWITH_OPERATOR, string.Empty).Trim();
                }
                else if (tmpX.StartsWith(ENDWITH_OPERATOR, StringComparison.OrdinalIgnoreCase))
                {
                  isCondition = true;

                  param[0] = tmpX.Replace(ENDWITH_OPERATOR, string.Empty).Trim();
                }
                else if (tmpX.StartsWith(CONTAINS_OPERATOR, StringComparison.OrdinalIgnoreCase))
                {
                  isCondition = true;

                  param[0] = tmpX.Replace(CONTAINS_OPERATOR, string.Empty).Trim();
                }
              }

              typ = Type.GetType(param[2]);
              if (((typ == null) || (typ.Equals(typeof(string)))) && (!isInCodition))
              {
                if (param[1].Contains(SYMBOLIC_SPLITER))
                {
                  splt = param[1].Split(new[] { SYMBOLIC_SPLITER }, StringSplitOptions.RemoveEmptyEntries);
                  if (splt.Length == 2)
                  {
                    dic.Add(param[0].Trim(), new ParameterParser()
                    {
                      IsSet = true,
                      IsBetween = true,
                      IsCondition = true,
                      Value = (splt[0]).ToString().Trim(),
                      Value_Next = (splt[1]).ToString().Trim(),
                      TypeOf = typeof(string)
                    });
                  }
                }
                else
                {
                  nullLike = (param[0].Length >= 2 ?
                    (param[0].Substring(0, 2).Equals(NULL_OPERATOR, StringComparison.OrdinalIgnoreCase) ? param[0].Substring(0, 2) : string.Empty) : string.Empty);
                  
                  if (!string.IsNullOrEmpty(nullLike))
                  {
                    param[0] = param[0].Substring(2);
                  }

                  dic.Add(param[0].Trim(), new ParameterParser()
                  {
                    IsSet = true,
                    IsCondition = isCondition,
                    IsLike = (param[1].Contains(LIKE_OPERATOR) ? true : false),
                    IsLikeOrNull = (param[1].Contains(LIKE_OPERATOR) && nullLike.Equals("!|",  StringComparison.OrdinalIgnoreCase) ? true : false),
                    //IsLikeRight = (param[1].EndsWith(LIKE_OPERATOR, StringComparison.OrdinalIgnoreCase) && (!param[1].StartsWith(LIKE_OPERATOR, StringComparison.OrdinalIgnoreCase)) ? true : false),
                    //IsLikeLeft = (param[1].StartsWith(LIKE_OPERATOR, StringComparison.OrdinalIgnoreCase) && (!param[1].EndsWith(LIKE_OPERATOR, StringComparison.OrdinalIgnoreCase)) ? true : false),
                    //IsLikeBoth = ((param[1].StartsWith(LIKE_OPERATOR, StringComparison.OrdinalIgnoreCase) && param[1].EndsWith(LIKE_OPERATOR, StringComparison.OrdinalIgnoreCase)) ||
                    //              param[1].Contains(LIKE_OPERATOR) ? true : false),
                    Value = (param[1]).ToString().Trim(),
                    TypeOf = typeof(string)
                  });
                }
              }
              else if (typ.Equals(typeof(decimal)))
              {
                if (param[1].Contains(SYMBOLIC_SPLITER))
                {
                  splt = param[1].Split(new[] { SYMBOLIC_SPLITER }, StringSplitOptions.RemoveEmptyEntries);
                  if (splt.Length == 2)
                  {
                    dic.Add(param[0].Trim(), new ParameterParser()
                    {
                      IsSet = true,
                      IsBetween = true,
                      IsCondition = true,
                      Value = decimal.Parse(splt[0]),
                      Value_Next = decimal.Parse(splt[1]),
                      TypeOf = typ
                    });
                  }
                }
                else
                {
                  dic.Add(param[0].Trim(), new ParameterParser()
                  {
                    IsSet = true,
                    IsCondition = isCondition,
                    Value = decimal.Parse(param[1]),
                    TypeOf = typ
                  });
                }
              }
              else if (typ.Equals(typeof(int)))
              {
                if (param[1].Contains(SYMBOLIC_SPLITER))
                {
                  splt = param[1].Split(new[] { SYMBOLIC_SPLITER }, StringSplitOptions.RemoveEmptyEntries);
                  if (splt.Length == 2)
                  {
                    dic.Add(param[0].Trim(), new ParameterParser()
                    {
                      IsSet = true,
                      IsBetween = true,
                      IsCondition = true,
                      Value = int.Parse(splt[0]),
                      Value_Next = int.Parse(splt[1]),
                      TypeOf = typ
                    });
                  }
                }
                else
                {
                  dic.Add(param[0].Trim(), new ParameterParser()
                  {
                    IsSet = true,
                    IsCondition = isCondition,
                    Value = int.Parse(param[1]),
                    TypeOf = typ
                  });
                }
              }
              else if (typ.Equals(typeof(short)))
              {
                if (param[1].Contains(SYMBOLIC_SPLITER))
                {
                  splt = param[1].Split(new[] { SYMBOLIC_SPLITER }, StringSplitOptions.RemoveEmptyEntries);
                  if (splt.Length == 2)
                  {
                    dic.Add(param[0].Trim(), new ParameterParser()
                    {
                      IsSet = true,
                      IsBetween = true,
                      IsCondition = true,
                      Value = short.Parse(splt[1]),
                      Value_Next = short.Parse(splt[1]),
                      TypeOf = typ
                    });
                  }
                }
                else
                {
                  dic.Add(param[0].Trim(), new ParameterParser()
                  {
                    IsSet = true,
                    IsCondition = isCondition,
                    Value = short.Parse(param[1]),
                    TypeOf = typ
                  });
                }
              }
              else if (typ.Equals(typeof(long)))
              {
                if (param[1].Contains(SYMBOLIC_SPLITER))
                {
                  splt = param[1].Split(new[] { SYMBOLIC_SPLITER }, StringSplitOptions.RemoveEmptyEntries);
                  if (splt.Length == 2)
                  {
                    dic.Add(param[0].Trim(), new ParameterParser()
                    {
                      IsSet = true,
                      IsBetween = true,
                      IsCondition = true,
                      Value = long.Parse(splt[1]),
                      Value_Next = long.Parse(splt[1]),
                      TypeOf = typ
                    });
                  }
                }
                else
                {
                  dic.Add(param[0].Trim(), new ParameterParser()
                  {
                    IsSet = true,
                    IsCondition = isCondition,
                    Value = long.Parse(param[1]),
                    TypeOf = typ
                  });
                }
              }
              else if (typ.Equals(typeof(byte)))
              {
                if (param[1].Contains(SYMBOLIC_SPLITER))
                {
                  splt = param[1].Split(new[] { SYMBOLIC_SPLITER }, StringSplitOptions.RemoveEmptyEntries);
                  if (splt.Length == 2)
                  {
                    dic.Add(param[0].Trim(), new ParameterParser()
                    {
                      IsSet = true,
                      IsBetween = true,
                      IsCondition = true,
                      Value = byte.Parse(splt[1]),
                      Value_Next = byte.Parse(splt[1]),
                      TypeOf = typ
                    });
                  }
                }
                else
                {
                  dic.Add(param[0].Trim(), new ParameterParser()
                  {
                    IsSet = true,
                    IsCondition = isCondition,
                    Value = byte.Parse(param[1]),
                    TypeOf = typ
                  });
                }
              }
              else if (typ.Equals(typeof(bool)))
              {
                dic.Add(param[0].Trim(), new ParameterParser()
                {
                  IsSet = true,
                  IsCondition = isCondition,
                  Value = bool.Parse(param[1]),
                  TypeOf = typ
                });
              }
              else if (typ.Equals(typeof(DateTime)))
              {
                if (param[1].Contains(SYMBOLIC_SPLITER))
                {
                  splt = param[1].Split(new[] { SYMBOLIC_SPLITER }, StringSplitOptions.RemoveEmptyEntries);
                  if (splt.Length == 2)
                  {
                    dic.Add(param[0].Trim(), new ParameterParser()
                    {
                      IsSet = true,
                      IsBetween = true,
                      IsCondition = true,
                      Value = DateTime.ParseExact(splt[0].ToString().Trim(), "d-M-yyyy", new CultureInfo("id-ID")),
                      Value_Next = DateTime.ParseExact(splt[1].ToString().Trim(), "d-M-yyyy", new CultureInfo("id-ID")),
                      TypeOf = typ
                    });
                  }
                }
                else
                {
                  dic.Add(param[0].Trim(), new ParameterParser()
                  {
                    IsSet = true,
                    IsCondition = isCondition,
                    Value = DateTime.ParseExact(param[1], "d-M-yyyy", new CultureInfo("id-ID")),
                    TypeOf = typ
                  });
                }
              }
              else if (typ.Equals(typeof(char)))
              {
                if (param[1].Contains(SYMBOLIC_SPLITER))
                {
                  splt = param[1].Split(new[] { SYMBOLIC_SPLITER }, StringSplitOptions.RemoveEmptyEntries);
                  if (splt.Length == 2)
                  {
                    dic.Add(param[0].Trim(), new ParameterParser()
                    {
                      IsSet = true,
                      IsBetween = true,
                      IsCondition = true,
                      Value = char.Parse(splt[0].ToString().Trim()),
                      Value_Next = char.Parse(splt[1].ToString().Trim()),
                      TypeOf = typeof(char)
                    });
                  }
                }
                else
                {
                  dic.Add(param[0].Trim(), new ParameterParser()
                  {
                    IsSet = true,
                    IsCondition = isCondition,
                    Value = char.Parse(param[1]),
                    TypeOf = typ
                  });
                }
              }
              else if (typ.IsArray || isInCodition)
              {
                tmpX = param[1].Replace(";", ",");

                jarr = Newtonsoft.Json.Linq.JArray.Parse(tmpX);

                if (jarr.Count < 1)
                {
                  jarr.Clear();

                  continue;
                }

                #region Array Parser

                if (typ.GetElementType().Equals(typeof(string)))
                {
                  dic.Add(param[0].Trim(), new ParameterParser()
                  {
                    IsSet = true,
                    IsCondition = true,
                    IsIn = isInCodition,
                    Value = Ext.Net.JSON.Deserialize<string[]>(jarr.ToString()),
                    TypeOf = typ
                  });
                }
                else if (typ.GetElementType().Equals(typeof(DateTime)))
                {
                  dic.Add(param[0].Trim(), new ParameterParser()
                  {
                    IsSet = true,
                    IsCondition = true,
                    IsIn = isInCodition,
                    Value = Ext.Net.JSON.Deserialize<DateTime[]>(jarr.ToString()),
                    TypeOf = typ
                  });
                }
                else if (typ.GetElementType().Equals(typeof(byte)))
                {
                  dic.Add(param[0].Trim(), new ParameterParser()
                  {
                    IsSet = true,
                    IsCondition = true,
                    IsIn = isInCodition,
                    Value = Ext.Net.JSON.Deserialize<byte[]>(jarr.ToString()),
                    TypeOf = typ
                  });
                }
                else if (typ.GetElementType().Equals(typeof(short)))
                {
                  dic.Add(param[0].Trim(), new ParameterParser()
                  {
                    IsSet = true,
                    IsCondition = true,
                    IsIn = isInCodition,
                    Value = Ext.Net.JSON.Deserialize<short[]>(jarr.ToString()),
                    TypeOf = typ
                  });
                }
                else if (typ.GetElementType().Equals(typeof(int)))
                {
                  dic.Add(param[0].Trim(), new ParameterParser()
                  {
                    IsSet = true,
                    IsCondition = true,
                    IsIn = isInCodition,
                    Value = Ext.Net.JSON.Deserialize<int[]>(jarr.ToString()),
                    TypeOf = typ
                  });
                }
                else if (typ.GetElementType().Equals(typeof(long)))
                {
                  dic.Add(param[0].Trim(), new ParameterParser()
                  {
                    IsSet = true,
                    IsCondition = true,
                    IsIn = isInCodition,
                    Value = Ext.Net.JSON.Deserialize<long[]>(jarr.ToString()),
                    TypeOf = typ
                  });
                }
                else if (typ.GetElementType().Equals(typeof(decimal)))
                {
                  dic.Add(param[0].Trim(), new ParameterParser()
                  {
                    IsSet = true,
                    IsCondition = true,
                    IsIn = isInCodition,
                    Value = Ext.Net.JSON.Deserialize<long[]>(jarr.ToString()),
                    TypeOf = typ
                  });
                }

                #endregion
              }
            }
            catch (Exception ex)
            {
              Logger.WriteLine("ScmsSoaLibrary.Commons.Functionals:ParserArrayParameter - {0}", ex.Message);
              Logger.WriteLine(ex.StackTrace);
            }

            #endregion
          }
        }
      }

      return dic;
    }

    public static string DecimalToString(decimal value)
    {
      return DecimalToString(value, null);
    }

    public static string DecimalToString(decimal value, string format)
    {
      string rest = null;
      System.Globalization.CultureInfo ci = new CultureInfo("id-ID");

      try
      {
        if (string.IsNullOrEmpty(format))
        {
          rest = value.ToString();
        }
        else
        {
          rest = value.ToString(format, ci);
        }
      }
      catch (Exception ex)
      {
        rest = string.Empty;

        Logger.WriteLine("ScmsSoaLibrary.Commons.Functionals:NumberToString - {0}", ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }

      return rest;
    }

    public static DateTime StandardSqlDateTime
    { get { return new DateTime(1900, 1, 1); } }
    
    public static string DistCoreUrlBuilderString(Config cfg, string url)
    {
      return DistCoreUrlBuilder(cfg, url).ToString();
    }

    public static Uri DistCoreUrlBuilder(Config cfg, string url)
    {
      if (string.IsNullOrEmpty(url))
      {
        return null;
      }

      System.Net.IPEndPoint ep = (System.Net.IPEndPoint)cfg.DiscoreEP;

      UriBuilder uri = new UriBuilder(url);

      uri.Host = ep.Address.ToString();
      uri.Port = ep.Port;

      return uri.Uri;
    }

    #region Crypto

    public static string DecryptHashRjdnl(string key, string val)
    {
      CryptorDES des = new CryptorDES(key, string.Empty);

      byte[] byt = GlobalCrypto.FromBase64(val);
      string dat = des.DeCrypt(byt);
      Array.Clear(byt, 0, byt.Length);
      byt = null;

      return dat;
    }

    public static string CryptHashRjdnl(string key, string val)
    {
      CryptorDES des = new CryptorDES(key, string.Empty);

      byte[] byt = des.CryptArray(val);
      string dat = GlobalCrypto.ToBase64(byt);
      Array.Clear(byt, 0, byt.Length);
      byt = null;

      return dat;
    }

    public static string GeneratedRandomUniqueId(int MaximalSize, string HeaderSignature, params string[] NextCodes)
    {
      if (MaximalSize < 1)
      {
        return HeaderSignature;
      }

      if (randNumber == null)
      {
        randNumber = new Random((int)DateTime.Now.Ticks);
      }

      int genSize = (MaximalSize - (string.IsNullOrEmpty(HeaderSignature) ? 0 : HeaderSignature.Length));

      if (genSize < 1)
      {
        return HeaderSignature;
      }

      string codes = string.Empty;
      char[] chars = ("1234567890abcdef").ToCharArray();
      byte[] datas = null;

      if ((NextCodes != null) && (NextCodes.Length > 0))
      {
        codes = string.Join(string.Empty, NextCodes);
        datas = Encoding.UTF8.GetBytes(codes);
      }
      else
      {
        datas = new byte[4];
        randNumber.NextBytes(datas);
      }

      System.Security.Cryptography.RNGCryptoServiceProvider crypto = new System.Security.Cryptography.RNGCryptoServiceProvider();

      crypto.GetNonZeroBytes(datas);
      datas = new byte[genSize];
      crypto.GetNonZeroBytes(datas);

      StringBuilder result = new StringBuilder(MaximalSize);

      result.Append(HeaderSignature);
      //foreach (byte b in data)
      for (int nLoop = 0; nLoop < datas.Length; nLoop++)
      {
        result.Append(chars[datas[nLoop] % (chars.Length - 1)]);
      }

      if (result.Length > MaximalSize)
      {
        result.Remove(MaximalSize, (result.Length - MaximalSize));
      }
      else if (result.Length < MaximalSize)
      {
        while (result.Length > MaximalSize)
        {
          result.Append(chars[randNumber.Next(0, (chars.Length - 1))]);
        }
      }

      return result.ToString();
    }

    public static string GeneratedRandomPinId(int MaximalSize, string HeaderSignature, params string[] NextCodes)
    {
      if (MaximalSize < 1)
      {
        return HeaderSignature;
      }

      if (randNumber == null)
      {
        randNumber = new Random((int)DateTime.Now.Ticks);
      }

      int genSize = (MaximalSize - (string.IsNullOrEmpty(HeaderSignature) ? 0 : HeaderSignature.Length));

      if (genSize < 1)
      {
        return HeaderSignature;
      }

      string codes = string.Empty;
      char[] chars = ("1234567890").ToCharArray();
      byte[] datas = null;

      if ((NextCodes != null) && (NextCodes.Length > 0))
      {
        codes = string.Join(string.Empty, NextCodes);
        datas = Encoding.UTF8.GetBytes(codes);
      }
      else
      {
        datas = new byte[4];
        randNumber.NextBytes(datas);
      }

      System.Security.Cryptography.RNGCryptoServiceProvider crypto = new System.Security.Cryptography.RNGCryptoServiceProvider();

      crypto.GetNonZeroBytes(datas);
      datas = new byte[genSize];
      crypto.GetNonZeroBytes(datas);

      StringBuilder result = new StringBuilder(MaximalSize);

      result.Append(HeaderSignature);
      //foreach (byte b in data)
      for (int nLoop = 0; nLoop < datas.Length; nLoop++)
      {
        result.Append(chars[datas[nLoop] % (chars.Length - 1)]);
      }

      if (result.Length > MaximalSize)
      {
        result.Remove(MaximalSize, (result.Length - MaximalSize));
      }
      else if (result.Length < MaximalSize)
      {
        while (result.Length > MaximalSize)
        {
          result.Append(chars[randNumber.Next(0, (chars.Length - 1))]);
        }
      }

      return result.ToString();
    }

    #endregion

    #region Class Extension



    #endregion

    #region Report

    public static int PaperKind(string paperName)
    {
      int rawKind = 0;
      System.Drawing.Printing.PrintDocument printDoc = null;
      System.Drawing.Printing.PrinterSettings printSet = null;

      try
      {
        //if (Aig.Core.GlobalAsax.PrinterPageSource != null)
        //{
        //  if (Aig.Core.GlobalAsax.PrinterPageSource.ContainsKey(paperName))
        //  {
        //    rawKind = Aig.Core.GlobalAsax.PrinterPageSource[paperName];
        //  }
        //}

        if (dicPaperKind == null)
        {
          string tmp = null;

          dicPaperKind = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

          printDoc = new System.Drawing.Printing.PrintDocument();

          printSet = printDoc.PrinterSettings;
          
          //printDoc.PrinterSettings
          //System.Drawing.Printing.PrinterSettings.PaperSizeCollection

          if (!dicPaperKind.ContainsKey("Default"))
          {
            dicPaperKind.Add("Default", 0);
          }

          for (int n = 0, nLen = printSet.PaperSizes.Count; n < nLen; n++)
          {
            //if (printDoc.PrinterSettings.PaperSizes[n].PaperName.Equals(paperName, StringComparison.OrdinalIgnoreCase))
            //{
            //  rawKind = printDoc.PrinterSettings.PaperSizes[n].RawKind;
            //  break;
            //}

            tmp = printSet.PaperSizes[n].PaperName;
            if (!dicPaperKind.ContainsKey(tmp))
            {
              dicPaperKind.Add(tmp, printSet.PaperSizes[n].RawKind);
            }
          }
        }

        if (dicPaperKind.ContainsKey(paperName))
        {
          rawKind = dicPaperKind[paperName];
        }
        else
        {
          rawKind = -2;
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          string.Format("ScmsSoaLibrary.Commons.Functionals:PaperKind - {0}", ex.Message));
        Logger.WriteLine(ex.StackTrace);

        rawKind = -1;
      }
      finally
      {
        if (printDoc != null)
        {
          printDoc.Dispose();
        }
      }

      return rawKind;
    }

    public static int ExecProcedures(Config cfg, string procedures, params SqlParameter[] parameters)
    {
      return ExecProcedures(cfg, procedures, false, parameters);
    }

    public static int ExecProcedures(Config cfg, string procedures, bool scalar, params SqlParameter[] parameters)
    {
      if (string.IsNullOrEmpty(procedures))
      {
        return 0;
      }

      int nResult = 0;

      //SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDbConn"].ToString());
      //SqlConnection cn = new SqlConnection();
      SqlConnection cn = new SqlConnection(cfg.ConnectionString);

      SqlCommand cmd = cn.CreateCommand();

      try
      {
        cn.Open();

        cmd.CommandTimeout = 3000;

        cmd.CommandText = procedures;

        if ((parameters != null) && (parameters.Length > 0))
        {
          cmd.Parameters.AddRange(parameters);
        }

        if (scalar)
        {
          object o = cmd.ExecuteScalar();

          if (o.GetType().Equals(typeof(byte)) ||
            o.GetType().Equals(typeof(sbyte)) ||
            o.GetType().Equals(typeof(short)) ||
            o.GetType().Equals(typeof(ushort)) ||
            o.GetType().Equals(typeof(int)) ||
            o.GetType().Equals(typeof(uint)) ||
            o.GetType().Equals(typeof(long)) ||
            o.GetType().Equals(typeof(ulong)) ||
            o.GetType().Equals(typeof(float)) ||
            o.GetType().Equals(typeof(decimal)) ||
            o.GetType().Equals(typeof(string)))
          {
            int.TryParse(o.ToString(), out nResult);
          }
        }
        else
        {
          nResult = cmd.ExecuteNonQuery();
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);

        nResult = -1;
      }
      finally
      {
        if (cmd != null)
        {
          if (cmd.Parameters.Count > 0)
          {
            cmd.Parameters.Clear();
          }

          cmd.Dispose();
        }

        if (cn != null)
        {
          cn.Close();
          cn.Dispose();
        }
      }

      return nResult;
    }

    public static bool RebindReport(Config cfg, string userName, ref ReportEngine.CrystalReportStructureConfigure reportStructure, PopulateMode popTo)
    {
      bool bOk = false;
      int paperKind = 0;

      if (reportStructure.IsSet)
      {
        ReportEngine crm = null;

        try
        {
          #region Dynamic Data

          crm = new ReportEngine(reportStructure.ReportFile, cfg);

          if (crm != null)
          {
            #region Load Report

            if (crm.LoadReport(reportStructure.dataSet))
            {
              #region Loaded Report

              if (crm.IsLoaded)
              {
                crm.PrintOptions.PaperOrientation = (reportStructure.isLandscape ? CrystalDecisions.Shared.PaperOrientation.Landscape : CrystalDecisions.Shared.PaperOrientation.Portrait);
                if (!string.IsNullOrEmpty(reportStructure.paperName))
                {
                  paperKind = PaperKind(reportStructure.paperName);
                  if (paperKind == -1)
                  {
                    reportStructure.resultMessage = "Invalid to read paper id.";

                    return false;
                  }
                  else if (paperKind == -2)
                  {
                    // -- No Code ?
                  }
                  else if (((CrystalDecisions.Shared.PaperSize)paperKind) != CrystalDecisions.Shared.PaperSize.DefaultPaperSize)
                  {
                    crm.PrintOptions.PaperSize = (CrystalDecisions.Shared.PaperSize)paperKind;
                  }
                }

                crm.DataDefinition.RecordSelectionFormula = reportStructure.RecordSelection;

                if ((reportStructure.CustomizeTextData != null) && (reportStructure.CustomizeTextData.Length > 0))
                {
                  CrystalDecisions.CrystalReports.Engine.Section section;
                  CrystalDecisions.CrystalReports.Engine.TextObject to = null;
                  CrystalDecisions.CrystalReports.Engine.FieldObject fo = null;
                  CrystalDecisions.CrystalReports.Engine.DataDefinition dd = null; 
                  ScmsSoaLibraryInterface.Components.ReportCustomizeText ctd = null;

                  for (int nLoop = 0; nLoop < reportStructure.CustomizeTextData.Length; nLoop++)
                  {
                    ctd = reportStructure.CustomizeTextData[nLoop];

                    try { section = crm.ReportDefinition.Sections[ctd.SectionName]; }
                    catch { section = null; }

                    if (section != null)
                    {
                      try { to = (CrystalDecisions.CrystalReports.Engine.TextObject)section.ReportObjects[ctd.ControlName]; }
                      catch { to = null; }

                      try { fo = (CrystalDecisions.CrystalReports.Engine.FieldObject)section.ReportObjects[ctd.FieldObjName]; }
                      catch { fo = null; }

                      if (to != null)
                      {
                        to.Text = (string.IsNullOrEmpty(ctd.ControlName) ? string.Empty : ctd.Value.Trim());
                      }

                      if (fo != null)
                      {
                        crm.DataDefinition.FormulaFields[ctd.FieldObjName].Text = (string.IsNullOrEmpty(ctd.ControlName) ? string.Empty : ctd.Value.Trim());
                      }
                    }
                  }
                }

                FileInfo fi = new FileInfo(reportStructure.ReportFile);

                string fileName = null;
                string savedFile = null;

                switch (popTo)
                {
                  case PopulateMode.pmToExcelDataOnly:

                    fileName = string.Format("{0}_{1}.xls", Path.GetFileNameWithoutExtension(fi.Name), userName);

                    savedFile = crm.PopulateReportToDisk(CrystalDecisions.Shared.ExportFormatType.ExcelRecord,
                      reportStructure.outputFolder, string.Empty);

                    reportStructure.outputName = savedFile;
                    reportStructure.extReport = "xls";

                    crm.Dispose();

                    break;

                  case PopulateMode.pmToExcel:

                    fileName = string.Format("{0}_{1}.xls", Path.GetFileNameWithoutExtension(fi.Name), userName);

                    savedFile = crm.PopulateReportToDisk(CrystalDecisions.Shared.ExportFormatType.Excel,
                      reportStructure.outputFolder, string.Empty);

                    reportStructure.outputName = savedFile;
                    reportStructure.extReport = "xls";

                    crm.Dispose();

                    break;

                  case PopulateMode.pmToPdf:

                    fileName = string.Format("{0}_{1}.pdf", Path.GetFileNameWithoutExtension(fi.Name), userName);

                    savedFile = crm.PopulateReportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat,
                      reportStructure.outputFolder, string.Empty);

                    reportStructure.outputName = savedFile;
                    reportStructure.extReport = "pdf";

                    crm.Dispose();

                    break;

                  case PopulateMode.pmToWord:

                    fileName = string.Format("{0}_{1}.doc", Path.GetFileNameWithoutExtension(fi.Name), userName);

                    savedFile = crm.PopulateReportToDisk(CrystalDecisions.Shared.ExportFormatType.WordForWindows,
                      reportStructure.outputFolder, string.Empty);

                    reportStructure.outputName = savedFile;
                    reportStructure.extReport = "doc";

                    crm.Dispose();

                    break;
                }

                if (!string.IsNullOrEmpty(savedFile))
                {
                  fileName = Path.Combine(reportStructure.outputFolder, savedFile);
                  fi = new FileInfo(fileName);

                  decimal calcSize = 0;
                  if (fi.Exists)
                  {
                    calcSize = (fi.Length / 1024);

                    if (fi.Length < 1024)
                    {
                      savedFile = string.Format("{0:N2} bytes", calcSize);
                    }
                    else if (calcSize >= 1024m)
                    {
                      calcSize /= 1024;

                      savedFile = string.Format("{0:N2} MB", calcSize);
                    }
                    else
                    {
                      savedFile = string.Format("{0:N2} KB", calcSize);
                    }

                    reportStructure.sizeOutput = savedFile;
                  }
                  else
                  {
                    reportStructure.outputName = null;
                    reportStructure.resultMessage = "Generate report done, but physical file not found.";
                  }

                  bOk = true;
                }
                else
                {
                  reportStructure.resultMessage = crm.ErrorMessage;
                }
              }
              else
              {
                reportStructure.resultMessage = crm.ErrorMessage;
              }

              #endregion
            }
            else
            {
              reportStructure.resultMessage = crm.ErrorMessage;
            }

            #endregion
          }

          if (reportStructure.dataSet != null)
          {
            reportStructure.dataSet.Clear();
            reportStructure.dataSet.Dispose();
          }

          #endregion
        }
        catch (Exception ex)
        {
          reportStructure.resultMessage = ex.Message;

          Logger.WriteLine(ex.Message);
          Logger.WriteLine(ex.StackTrace);
        }
      }
      else
      {
        reportStructure.resultMessage = "Report structure is not set.";
      }

      return bOk;
    }

    public static string ReportParameterBuilder(ReportParameter[] rptParams)
    {
      StringBuilder sbRptRecordSel = new StringBuilder();

      #region Report

      System.Type typ = null;
      ReportParameter rptParam = null;
      DateTime date = DateTime.MinValue;

      for (int nLoop = 0, nSize = 0, nLen = rptParams.Length; nLoop < nLen; nLoop++)
      {
        rptParam = rptParams[nLoop];
        if ((!rptParam.IsSqlParameter) && (!rptParam.IsLinqFilterParameter) && (!rptParam.IsDatasetParameter))
        {
          if (nSize > 0)
          {
            if (rptParam.OrMethod)
            {
              sbRptRecordSel.Append(" OR ");
            }
            else
            {
              sbRptRecordSel.Append(" AND ");
            }
          }

          if (rptParam.IsReportDirectValue)
          {
            sbRptRecordSel.Append(rptParam.ParameterName);
          }
          else
          {
            typ = rptParam.DataTypeRaw;            

            if (typ.Equals(typeof(DateTime)))
            {
              date = rptParam.ParameterRawValue<DateTime>(DateTime.MinValue);

              sbRptRecordSel.AppendFormat("({{{0}}} = Date({1}, {2}, {3}))", rptParam.ParameterName, date.Year, date.Month, date.Day);
            }
            else if (typ.Equals(typeof(decimal)))
            {
              sbRptRecordSel.AppendFormat("({{{0}}} = {1})", rptParam.ParameterName,
                rptParam.ParameterRawValue<decimal>(decimal.MinValue));
            }
            else if (typ.Equals(typeof(int)))
            {
              sbRptRecordSel.AppendFormat("({{{0}}} = {1})", rptParam.ParameterName,
                rptParam.ParameterRawValue<int>(int.MinValue));
            }
            else if (typ.Equals(typeof(long)))
            {
              sbRptRecordSel.AppendFormat("({{{0}}} = {1})", rptParam.ParameterName,
                rptParam.ParameterRawValue<long>(long.MinValue));
            }
            else if (typ.Equals(typeof(bool)))
            {
              sbRptRecordSel.AppendFormat("({{{0}}} = {1})", rptParam.ParameterName,
                rptParam.ParameterRawValue<bool>(false));
            }
            else
            {
              if (rptParam.IsBetweenValue)
              {
                sbRptRecordSel.AppendFormat("({{{0}}} in [{1}])", rptParam.ParameterName, rptParam.ParameterValue);
              }
              else
              {
                sbRptRecordSel.AppendFormat("({{{0}}} = '{1}')", rptParam.ParameterName, rptParam.ParameterValue);
              }
            }
          }

          nSize++;
        }
      }

      #endregion

      return sbRptRecordSel.ToString();
    }

    public static Dictionary<string, ParameterParser> ReportParameterDatasetBuilder(ReportParameter[] rptParams)
    {
      Dictionary<string, ParameterParser> dic = new Dictionary<string,ParameterParser>(StringComparer.OrdinalIgnoreCase);
      ReportParameter rp = null;
      ParameterParser pp = default(ParameterParser);

      for (int nLoop = 0, nLen = rptParams.Length; nLoop < nLen; nLoop++)
      {
        rp = rptParams[nLoop];

        if (rp.IsInValue && ((rp.ParameterValueArray == null) || (rp.ParameterValueArray.Length < 1))) 
        {
          continue;
        }
        else if ((!rp.IsInValue)&& string.IsNullOrEmpty(rp.ParameterValue) || (!rp.IsDatasetParameter))
        {
          continue;
        }

        pp = new ParameterParser()
        {
          IsSet=true,
          TypeOf = rp.DataTypeRaw,
        };

        #region Extract Data

        if (rp.IsBetweenValue)
        {
          #region Between Value

          if (rp.DataTypeRaw.Equals(typeof(DateTime)))
          {
            pp.Value = rp.ParameterRawValue<DateTime>(StandardSqlDateTime);
            pp.Value_Next = rp.ParameterBetweenRawValue<DateTime>(StandardSqlDateTime);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(bool)))
          {
            pp.Value = rp.ParameterRawValue<bool>(false);
            pp.Value_Next = rp.ParameterBetweenRawValue<DateTime>(StandardSqlDateTime);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(float)))
          {
            pp.Value = rp.ParameterRawValue<float>(0);
            pp.Value_Next = rp.ParameterBetweenRawValue<float>(0);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(decimal)))
          {
            pp.Value = rp.ParameterRawValue<decimal>(0);
            pp.Value_Next = rp.ParameterBetweenRawValue<decimal>(0);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(long)))
          {
            pp.Value = rp.ParameterRawValue<long>(0);
            pp.Value_Next = rp.ParameterBetweenRawValue<long>(0);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(int)))
          {
            pp.Value = rp.ParameterRawValue<int>(0);
            pp.Value_Next = rp.ParameterBetweenRawValue<int>(0);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(short)))
          {
            pp.Value = rp.ParameterRawValue<short>(0);
            pp.Value_Next = rp.ParameterBetweenRawValue<int>(0);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(char)))
          {
            pp.Value = rp.ParameterRawValue<char>(char.MinValue);
            pp.Value_Next = rp.ParameterBetweenRawValue<char>(char.MinValue);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(byte)))
          {
            pp.Value = rp.ParameterRawValue<byte>(0);
            pp.Value_Next = rp.ParameterBetweenRawValue<byte>(0);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }          
          else
          {
            pp.Value = rp.ParameterRawValue<string>(string.Empty);
            pp.Value_Next = rp.ParameterBetweenRawValue<string>(string.Empty);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }

          #endregion
        }
        else if (rp.IsInValue)
        {
          #region In Value

          pp.IsIn = true;

          if (rp.DataTypeRaw.Equals(typeof(string[])))
          {
            pp.Value = rp.ParameterRawValueArray<string[]>(new string[0]);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(DateTime[])))
          {
            pp.Value = rp.ParameterRawValueArray<DateTime[]>(new DateTime[0]);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(float[])))
          {
            pp.Value = rp.ParameterRawValueArray<float[]>(new float[0]);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(decimal[])))
          {
            pp.Value = rp.ParameterRawValueArray<decimal[]>(new decimal[0]);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(long[])))
          {
            pp.Value = rp.ParameterRawValueArray<long[]>(new long[0]);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(int[])))
          {
            pp.Value = rp.ParameterRawValueArray<int[]>(new int[0]);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(short[])))
          {
            pp.Value = rp.ParameterRawValueArray<short[]>(new short[0]);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(byte[])))
          {
            pp.Value = rp.ParameterRawValueArray<byte[]>(new byte[0]);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(char[])))
          {
            pp.Value = rp.ParameterRawValueArray<char[]>(new char[0]);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(bool[])))
          {
            pp.Value = rp.ParameterRawValueArray<bool[]>(new bool[0]);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }

          #endregion
        }
        else
        {
          #region Standard

          if (rp.DataTypeRaw.Equals(typeof(DateTime)))
          {
            pp.Value = rp.ParameterRawValue<DateTime>(StandardSqlDateTime);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(bool)))
          {
            pp.Value = rp.ParameterRawValue<bool>(false);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(float)))
          {
            pp.Value = rp.ParameterRawValue<float>(0);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(decimal)))
          {
            pp.Value = rp.ParameterRawValue<decimal>(0);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(long)))
          {
            pp.Value = rp.ParameterRawValue<long>(0);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(int)))
          {
            pp.Value = rp.ParameterRawValue<int>(0);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(short)))
          {
            pp.Value = rp.ParameterRawValue<short>(0);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(char)))
          {
            pp.Value = rp.ParameterRawValue<char>(char.MinValue);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else if (rp.DataTypeRaw.Equals(typeof(byte)))
          {
            pp.Value = rp.ParameterRawValue<byte>(0);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }
          else
          {
            pp.Value = rp.ParameterRawValue<string>(string.Empty);
            pp.IsCondition = true;

            dic.Add(rp.ParameterName, pp);
          }

          #endregion
        }
                
        #endregion
      }

      return dic;
    }

    public static bool ReportSaveParser(string reportName, string tipeReport, string rptFileStorage, string rptFile, string vTipe, string vUser, string vSize, bool isAsync, string userDefineName, bool isShared, out string outputMesasge)
    {
      bool result = false;
      PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

      PostDataParser parser = new PostDataParser();
      IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

      dic.Add("Idx", new PostDataParser.StructurePair()
      {
        IsSet = true,
        Value = "0"
      });
      dic.Add("Type", new PostDataParser.StructurePair()
      {
        IsSet = true,
        Value = tipeReport
      });
      dic.Add("Name", new PostDataParser.StructurePair()
      {
        IsSet = true,
        Value = reportName
      });
      dic.Add("Report", new PostDataParser.StructurePair()
      {
        IsSet = true,
        Value = rptFile
      });
      dic.Add("FileType", new PostDataParser.StructurePair()
      {
        IsSet = true,
        Value = vTipe
      });
      dic.Add("Size", new PostDataParser.StructurePair()
      {
        IsSet = true,
        Value = vSize
      });
      dic.Add("Downloaded", new PostDataParser.StructurePair()
      {
        IsSet = true,
        Value = (isAsync ? "0" : "1")
      });
      dic.Add("User", new PostDataParser.StructurePair()
      {
        IsSet = true,
        Value = vUser
      });
      dic.Add("UserDefinedName", new PostDataParser.StructurePair()
      {
        IsSet = true,
        Value = userDefineName
      });
      dic.Add("Shared", new PostDataParser.StructurePair()
      {
        IsSet = true,
        Value = isShared.ToString().ToLower()
      });

      string varData = null;

      try
      {
        varData = parser.ParserData("Reporting", "Add", dic);
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibrary.Commons.Functionals:ReportSaveParser - {0} ", ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }

      outputMesasge = null;

      if (varData != null)
      {
        string resultSrvc = null;

        Services.Service srvc = new ScmsSoaLibrary.Services.Service();

        resultSrvc = srvc.PostData(varData);

        responseResult = parser.ResponseParser(resultSrvc);

        outputMesasge = responseResult.Message;

        result = (responseResult.Response == PostDataParser.ResponseStatus.Success);
      }

      return result;
    }

    #endregion
  }
}
