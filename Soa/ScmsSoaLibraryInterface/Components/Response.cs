using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaLibraryInterface.Components
{
  [Serializable]
  public class ReportingResult
  {
    public string MessageResponse
    { get; set; }

    public bool IsSuccess
    { get; set; }

    public string OutputFile
    { get; set; }

    public string Extension
    { get; set; }

    public static ReportingResult Serialize(string rawData)
    {
      if (string.IsNullOrEmpty(rawData))
      {
        return null;
      }

      ReportingResult responRptParser = null;

      MemoryStream memory = new MemoryStream();
      //SoapFormatter formatter = new SoapFormatter();
      XmlSerializer formatter = new XmlSerializer(typeof(ReportingResult));

      try
      {
        memory.Write(Encoding.UTF8.GetBytes(rawData), 0, rawData.Length);

        memory.Position = 0;

        responRptParser = formatter.Deserialize(memory) as ReportingResult;
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          string.Format("ScmsSoaLibraryInterface.Components.ReportingResult:Serialize - {0}", ex.Message));
      }
      finally
      {
        if (memory != null)
        {
          memory.Close();
          memory.Dispose();
        }
      }

      return responRptParser;
    }

    public static string Deserialize(ReportingResult responRptParser)
    {
      if (responRptParser == null)
      {
        return null;
      }

      MemoryStream memory = new MemoryStream();
      XmlSerializer formatter = new XmlSerializer(typeof(ReportingResult));
      string result = null;

      try
      {
        formatter.Serialize(memory, responRptParser);

        result = Encoding.UTF8.GetString(memory.ToArray());
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          string.Format("ScmsSoaLibraryInterface.Components.ReportingResult:Serialize - {0}", ex.Message));
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
  }
}
