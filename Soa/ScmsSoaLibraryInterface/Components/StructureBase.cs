using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using ScmsSoaLibraryInterface.Commons;
using System.Runtime.Serialization.Formatters.Binary;

namespace ScmsSoaLibraryInterface.Components
{
  public class StructureBase<T>
  {
    public static T Serialize(string rawData)
    {
      return new StructureBase<T>().SerializeIt(rawData);
    }

    public static string Deserialize(T strt)
    {
      return new StructureBase<T>().DeserializeIt(strt);
    }

    public static T SerializeBinary(string rawData)
    {
      return new StructureBase<T>().SerializeBinaryIt(rawData);
    }

    public static string DeserializeBinary(T strt)
    {
      return new StructureBase<T>().DeserializeBinaryIt(strt);
    }

    public T SerializeIt(string rawData)
    {
      if (string.IsNullOrEmpty(rawData))
      {
        return default(T);
      }

      T strt = default(T);

      MemoryStream memory = new MemoryStream();
      //SoapFormatter formatter = new SoapFormatter();
      XmlSerializer formatter = new XmlSerializer(typeof(T));

      try
      {
        memory.Write(Encoding.UTF8.GetBytes(rawData), 0, rawData.Length);

        memory.Position = 0;

        strt = (T)formatter.Deserialize(memory);

        //if (strt != null)
        //{
        //  strt.IsGroupMode = ((strt.Fields != null) &&
        //    (!string.IsNullOrEmpty(strt.Fields.Group)) ? true : false);
        //}
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibraryInterface.Components.StructureBase:SerializeIt - {0}", ex.Message);
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

    public string DeserializeBinaryIt(T strt)
    {
      if (strt == null)
      {
        return null;
      }

      MemoryStream memory = new MemoryStream();
      BinaryFormatter formatter = new BinaryFormatter();
      string result = null;

      try
      {
        formatter.Serialize(memory, strt);

        result = Convert.ToBase64String(memory.ToArray());
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibraryInterface.Components.StructureBase:Deserialize - {0}", ex.Message);
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

    public T SerializeBinaryIt(string rawData)
    {
      if (string.IsNullOrEmpty(rawData))
      {
        return default(T);
      }

      T strt = default(T);

      MemoryStream memory = new MemoryStream();
      //SoapFormatter formatter = new SoapFormatter();
      BinaryFormatter formatter = new BinaryFormatter();
      byte[] byts = null;

      try
      {
        byts = Convert.FromBase64String(rawData);

        memory.Write(byts, 0, byts.Length);

        memory.Position = 0;

        strt = (T)formatter.Deserialize(memory);

        Array.Clear(byts, 0, byts.Length);
        //if (strt != null)
        //{
        //  strt.IsGroupMode = ((strt.Fields != null) &&
        //    (!string.IsNullOrEmpty(strt.Fields.Group)) ? true : false);
        //}
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibraryInterface.Components.StructureBase:SerializeIt - {0}", ex.Message);
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

    public string DeserializeIt(T strt)
    {
      if (strt == null)
      {
        return null;
      }

      MemoryStream memory = new MemoryStream();
      //SoapFormatter formatter = new SoapFormatter();
      XmlSerializer formatter = new XmlSerializer(typeof(T));
      string result = null;

      try
      {
        formatter.Serialize(memory, strt);

        result = Encoding.UTF8.GetString(memory.ToArray());
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibraryInterface.Components.StructureBase:Deserialize - {0}", ex.Message);
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

    public static T SerializeJson(string rawData)
    {
      return SerializeJson(rawData, null);
    }

    public static T SerializeJson(string rawData, params Newtonsoft.Json.JsonConverter[] converters)
    {
      if (string.IsNullOrEmpty(rawData))
      {
        return default(T);
      }

      T strt = default(T);

      try
      {
        Newtonsoft.Json.JsonConvert.DeserializeObject<T>(rawData, converters);
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibraryInterface.Components.StructureBase:SerializeJson - {0}", ex.Message);
      }

      return strt;
    }

    public static string DeserializeJson(T strt)
    {
      return DeserializeJson(strt, false, null);
    }

    public static string DeserializeJson(T strt, bool isIndend, params Newtonsoft.Json.JsonConverter[] converters)
    {
      if (strt == null)
      {
        return null;
      }

      string result = null;
      
      try
      {
        result = Newtonsoft.Json.JsonConvert.SerializeObject((T)strt, (isIndend ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None), converters);
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibraryInterface.Components.StructureBase:SerializeJson - {0}", ex.Message);
      }

      return result;
    }
  }
}
