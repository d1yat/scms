using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaTester
{
  [Serializable]
  public class Structure
  {
    [XmlAttribute(AttributeName="name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static Structure Serialize(string rawData)
    {
      if (string.IsNullOrEmpty(rawData))
      {
        return null;
      }

      Structure strt = null;

      MemoryStream memory = new MemoryStream();
      //SoapFormatter formatter = new SoapFormatter();
      XmlSerializer formatter = new XmlSerializer(typeof(Structure));

      try
      {
        memory.Write(Encoding.UTF8.GetBytes(rawData), 0, rawData.Length);

        memory.Position = 0;

        strt = formatter.Deserialize(memory) as Structure;
      }
      catch (Exception ex)
      {
        Logger.WriteLine("Scms.Web.Core.RightBuilder.RightAccess:Serialize - {0}", ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }
      finally
      {
        if (memory != null)
        {
          memory.Close();
          memory.Dispose();
        }
      }

      return strt;
    }

    public static string Deserialize(Structure strt)
    {
      if (strt == null)
      {
        return null;
      }

      MemoryStream memory = new MemoryStream();
      //SoapFormatter formatter = new SoapFormatter();
      XmlSerializer formatter = new XmlSerializer(typeof(Structure));
      string result = null;

      try
      {
        formatter.Serialize(memory, strt);

        result = Encoding.UTF8.GetString(memory.ToArray());
      }
      catch (Exception ex)
      {
        Logger.WriteLine("Scms.Web.Core.RightBuilder.RightAccess:Serialize - {0}", ex.Message);
        Logger.WriteLine(ex.StackTrace);
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

    [XmlElement(ElementName = "Fields")]
    public StructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class StructureFields
  {
    [XmlAttribute(AttributeName = "Nip")]
    public string Nip
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public StructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class StructureField
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "New")]
    public bool IsNew
    { get; set; }

    [XmlAttribute(AttributeName = "Delete")]
    public bool IsDelete
    { get; set; }

    [XmlText]
    public string Value
    { get; set; }
  }
}
