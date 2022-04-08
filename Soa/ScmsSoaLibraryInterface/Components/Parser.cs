using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaLibraryInterface.Components
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

  public enum PopulateMode
  {
    pmToPdf,
    pmToExcelDataOnly,
    pmToExcel,
    pmToWord,
  }

  [Serializable]
  public class ReportParser
  {
    public string User
    { get; set; }

    public string ReportingID
    { get; set; }

    public string PaperID
    { get; set; }

    public bool IsLandscape
    { get; set; }

    public string Reason
    { get; set; }

    public PopulateMode OutputReport
    { get; set; }

    public static PopulateMode ParsingOutputReport(string outputName)
    {
      PopulateMode pm = PopulateMode.pmToPdf;

      try
      {
        switch (outputName)
        {
          case "02":
            pm = PopulateMode.pmToExcelDataOnly;
            break;
          case "03":
            pm = PopulateMode.pmToExcel;
            break;
          case "04":
            pm = PopulateMode.pmToWord;
            break;
          default:
            pm = PopulateMode.pmToPdf;
            break;
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibraryInterface.Components.ReportParser:<static>ParsingOutputReport - {0}", ex.Message);
      }

      return pm;
    }

    public static ReportParser Serialize(string rawData)
    {
      if (string.IsNullOrEmpty(rawData))
      {
        return null;
      }

      ReportParser rptParser = null;

      MemoryStream memory = new MemoryStream();
      //SoapFormatter formatter = new SoapFormatter();
      XmlSerializer formatter = null;

      try
      {
        formatter = new XmlSerializer(typeof(ReportParser));

        memory.Write(Encoding.UTF8.GetBytes(rawData), 0, rawData.Length);

        memory.Position = 0;

        rptParser = formatter.Deserialize(memory) as ReportParser;

        if ((rptParser != null) && (rptParser.ReportParameter != null) && (rptParser.ReportParameter.Length > 0))
        {
          List<ReportParameter> lstRptParam = new List<ReportParameter>();
          ReportParameter rptParam = null;

          for (int nLoop = 0, nLen = rptParser.ReportParameter.Length; nLoop < nLen; nLoop++)
          {
            rptParam = rptParser.ReportParameter[nLoop];

            if ((!rptParam.IsReportDirectValue) || rptParam.IsSqlParameter )
            {
              if (rptParam.IsInValue && ((rptParam.ParameterValueArray != null) && (rptParam.ParameterValueArray.Length > 0)))
              {
                lstRptParam.Add(rptParam);
              }
              if (!string.IsNullOrEmpty(rptParam.ParameterValue))
              {
                lstRptParam.Add(rptParam);
              }
            }
            else if (rptParam.IsReportDirectValue && (!string.IsNullOrEmpty(rptParam.ParameterName)))
            {
              lstRptParam.Add(rptParam);              
            }
          }

          rptParser.ReportParameter = lstRptParam.ToArray();

          lstRptParam.Clear();
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          string.Format("ScmsSoaLibraryInterface.Components.ReportParser:Serialize - {0}", ex.Message));
      }
      finally
      {
        if (memory != null)
        {
          memory.Close();
          memory.Dispose();
        }
      }

      return rptParser;
    }

    public static string Deserialize(ReportParser reportParser)
    {
      if (reportParser == null)
      {
        return null;
      }

      MemoryStream memory = new MemoryStream();
      //SoapFormatter formatter = new SoapFormatter();
      XmlSerializer formatter = null;
      string result = null;

      try
      {
        formatter = new XmlSerializer(typeof(ReportParser)); 

        formatter.Serialize(memory, reportParser);

        result = Encoding.UTF8.GetString(memory.ToArray());
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          string.Format("ScmsSoaLibraryInterface.Components.ReportParser:Serialize - {0}", ex.Message));
      }
      finally
      {
        if (memory != null)
        {
          memory.Close();
          memory.Dispose();
        }
      }

      return result;
    }

    public ReportParameter[] ReportParameter
    { get; set; }

    public ReportCustomizeText[] ReportCustomizeText
    { get; set; }

    public string UserDefinedName
    { get; set; }

    public bool IsShared
    { get; set; }

    public bool IsBarcode
    { get; set; }

    public bool IsAutoPrint
    { get; set; }
  }

  [Serializable]
  public class ReportParameter
  {
    public string ParameterName
    { get; set; }

    public string ParameterValue
    { get; set; }
    
    public T ParameterRawValue<T>(T defaultValue)
    {
      return ParameterRawValue<T>(defaultValue, new CultureInfo("id-ID"));
    }

    public T ParameterRawValue<T>(T defaultValue, CultureInfo culture)
    {
      T result = default(T);
      
      try
      {
        if (string.IsNullOrEmpty(this.ParameterValue))
        {
          if (!default(T).Equals(defaultValue))
          {
            result = defaultValue;
          }
        }
        else
        {
          if (culture == null)
          {
            result = (T)Convert.ChangeType(this.ParameterValue, typeof(T));
          }
          else
          {
            result = (T)Convert.ChangeType(this.ParameterValue, typeof(T), culture);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          string.Concat("ScmsSoaLibraryInterface.Components.ReportParameter:ParameterRawValue - ", ex.Message));

        if ((defaultValue != null) && (!defaultValue.Equals(default(T))))
        {
          result = defaultValue;
        }
      }

      return result;
    }

    public T ParameterBetweenRawValue<T>(T defaultValue)
    {
      return ParameterBetweenRawValue<T>(defaultValue, new CultureInfo("id-ID"));
    }

    public T ParameterBetweenRawValue<T>(T defaultValue, CultureInfo culture)
    {
      T result = default(T);

      try
      {
        if (string.IsNullOrEmpty(this.BetweenValue))
        {
          if (!default(T).Equals(defaultValue))
          {
            result = defaultValue;
          }
        }
        else
        {
          if (culture == null)
          {
            result = (T)Convert.ChangeType(this.BetweenValue, typeof(T));
          }
          else
          {
            result = (T)Convert.ChangeType(this.BetweenValue, typeof(T), culture);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          string.Concat("ScmsSoaLibraryInterface.Components.ReportParameter:ParameterBetweenRawValue - ", ex.Message));

        if ((defaultValue != null) && (!defaultValue.Equals(default(T))))
        {
          result = defaultValue;
        }
      }

      return result;
    }

    public T ParameterRawValueArray<T>(T defaultValue)
    {
      T result = default(T);

      try
      {
        result = ChangeTypeData<T>(ParameterValueArray, defaultValue);
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          string.Concat("ScmsSoaLibraryInterface.Components.ReportParameter:ParameterRawValueArray - ", ex.Message));

        if ((defaultValue != null) && (!defaultValue.Equals(default(T))))
        {
          result = defaultValue;
        }
      }

      return result;
    }

    public string DataType
    { get; set; }

    [XmlIgnore]
    public Type DataTypeRaw
    {
      get
      {
        Type type = null;

        try
        {
          type = Type.GetType(this.DataType);
        }
        catch (Exception ex)
        {
          Logger.WriteLine(
            string.Concat("ScmsSoaLibraryInterface.Components.ReportParameter:DataTypeRaw - ", ex.Message));
        }

        return type;
      }
    }

    public bool IsSqlParameter
    { get; set; }

    public bool IsDatasetParameter
    { get; set; }

    public bool IsLinqFilterParameter
    { get; set; }

    public bool IsReportDirectValue
    { get; set; }
    
    public bool IsInValue
    { get; set; }

    public bool IsBetweenValue
    { get; set; }

    public bool OrMethod
    { get; set; }

    public string BetweenValue
    { get; set; }

    [XmlArray]
    public string[] ParameterValueArray
    { get; set; }

    [XmlIgnore]
    public object ParameterValueObject
    {
      get
      {
        object o = null;
        System.Type typ = this.DataTypeRaw;

        if (typ.Equals(typeof(int)))
        {
          o = ChangeTypeData<int>(this.ParameterValue, 0);
        }
        else if (typ.Equals(typeof(long)))
        {
          o = ChangeTypeData<long>(this.ParameterValue, 0);
        }
        else if (typ.Equals(typeof(decimal)))
        {
          o = ChangeTypeData<decimal>(this.ParameterValue, 0);
        }
        else if (typ.Equals(typeof(float)))
        {
          o = ChangeTypeData<float>(this.ParameterValue, 0);
        }
        else if (typ.Equals(typeof(bool)))
        {
          o = ChangeTypeData<bool>(this.ParameterValue, false);
        }
        else if (typ.Equals(typeof(DateTime)))
        {
          o = ChangeTypeData<DateTime>(this.ParameterValue, new DateTime(1900, 1, 1));
        }
        else if (typ.Equals(typeof(char)))
        {
          o = ChangeTypeData<char>(this.ParameterValue, char.MinValue);
        }
        else if (typ.Equals(typeof(byte)))
        {
          o = ChangeTypeData<byte>(this.ParameterValue, 0);
        }
        else
        {
          o = ChangeTypeData<string>(this.ParameterValue, string.Empty);
        }

        return o;
      }
    }

    [XmlIgnore]
    public object BetweenValueObject
    {
      get
      {
        object o = null;
        System.Type typ = this.DataTypeRaw;

        if (typ.Equals(typeof(int)))
        {
          o = ChangeTypeData<int>(this.BetweenValue, 0);
        }
        else if (typ.Equals(typeof(long)))
        {
          o = ChangeTypeData<long>(this.BetweenValue, 0);
        }
        else if (typ.Equals(typeof(decimal)))
        {
          o = ChangeTypeData<decimal>(this.BetweenValue, 0);
        }
        else if (typ.Equals(typeof(float)))
        {
          o = ChangeTypeData<float>(this.BetweenValue, 0);
        }
        else if (typ.Equals(typeof(bool)))
        {
          o = ChangeTypeData<bool>(this.ParameterValue, false);
        }
        else if (typ.Equals(typeof(DateTime)))
        {
          o = ChangeTypeData<DateTime>(this.BetweenValue, new DateTime(1900, 1, 1));
        }
        else
        {
          o = ChangeTypeData<string>(this.BetweenValue, string.Empty);
        }

        return o;
      }
    }

    private T ChangeTypeData<T>(object valueData, T defaultValue)
    {
      T result = default(T);

      try
      {
        result = (T)Convert.ChangeType(valueData, typeof(T));

        if ((!defaultValue.Equals(default(T))) && result.Equals(default(T)))
        {
          result = defaultValue;
        }

        //if (((defaultValue != null) && defaultValue.Equals(default(T))) && (result != defaultValue))
        //{
        //  result = (T)Convert.ChangeType(valueData, typeof(T));
        //}

        //if ((defaultValue == null) || defaultValue.Equals(default(T)))
        //{
        //  //result = (row.IsNull(fieldName) ? default(T) : (T)row[fieldName]);
        //  result = (T)Convert.ChangeType(valueData, typeof(T));
        //}
        //else
        //{
        //}
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          string.Format("ScmsSoaLibraryInterface.Components.ReportParameter:ChangeTypeData - {0}", ex.Message));

        if ((defaultValue != null) && (!defaultValue.Equals(default(T))))
        {
          result = defaultValue;
        }
      }

      return result;
    }
  }

  [Serializable]
  public class ReportCustomizeText
  {
    public string SectionName
    { get; set; }

    public string ControlName
    { get; set; }

    public string FieldObjName
    { get; set; }

    public string Value
    { get; set; }
  }
}
