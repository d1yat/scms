using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using ScmsSoaLibraryInterface.Commons;
using System.Xml;

namespace ScmsSoaLibraryInterface.Components
{
  public class PostDataParser
  {
    public enum ResponseStatus
    {
      Unknown,
      Success,
      Failed,
      Error
    }

    public struct StructureResponse
    {
      public bool IsSet;
      public ResponseStatus Response;
      public Dictionary<string, string> Values;
      public string Message;
    }

    public struct StructurePair
    {
      public bool IsSet;
      public bool IsList;
      public string TagExtraName;
      public string Value;
      public Dictionary<string, string> DicAttributeValues;
      public Dictionary<string, StructurePair> DicValues;
      public Dictionary<string, StructurePair> DicValuesExtra;
    }

    //private void ParsingData(XmlWriter writer, IDictionary<string, StructurePair> dic)
    //{
    //  ParsingData(
    //}

    private void ParsingData(XmlWriter writer, IDictionary<string, StructurePair> dic)
    {
      if ((dic == null) || (dic.Count < 1))
      {
        return;
      }

      foreach (KeyValuePair<string, StructurePair> kvp in dic)
      {
        if (kvp.Value.IsSet)
        {
          if (kvp.Value.IsList)
          {
            writer.WriteStartElement("Fields");

            writer.WriteAttributeString(kvp.Key,
              ((kvp.Value.Value == null) ? string.Empty : kvp.Value.Value));

            if ((kvp.Value.DicAttributeValues != null) && (kvp.Value.DicAttributeValues.Count > 0))
            {
              foreach (KeyValuePair<string, string> kvpAttr in kvp.Value.DicAttributeValues)
              {
                writer.WriteAttributeString(kvpAttr.Key, kvpAttr.Value);
              }
            }

            this.ParsingData(writer, kvp.Value.DicValues);

            writer.WriteEndElement();
          }
          else
          {
            writer.WriteStartElement("Field");

            writer.WriteAttributeString("name", kvp.Key);

            if ((kvp.Value.DicAttributeValues != null) && (kvp.Value.DicAttributeValues.Count > 0))
            {
              foreach (KeyValuePair<string, string> kvpAttr in kvp.Value.DicAttributeValues)
              {
                writer.WriteAttributeString(kvpAttr.Key, kvpAttr.Value);
              }
            }

            writer.WriteValue(((kvp.Value.Value == null) ? string.Empty : kvp.Value.Value));
            
            writer.WriteEndElement();
          }
        }
      }
    }

    private void ParsingDataExtra(string listTagName, string tagName, XmlWriter writer, IDictionary<string, StructurePair> dic)
    {
      if ((dic == null) || (dic.Count < 1))
      {
        return;
      }

      writer.WriteStartElement(listTagName);

      writer.WriteAttributeString("name", tagName);

      foreach (KeyValuePair<string, StructurePair> kvp in dic)
      {
        writer.WriteStartElement(tagName);

        writer.WriteAttributeString("name", kvp.Key);

        if ((kvp.Value.DicAttributeValues != null) && (kvp.Value.DicAttributeValues.Count > 0))
        {
          foreach (KeyValuePair<string, string> kvpAttr in kvp.Value.DicAttributeValues)
          {
            writer.WriteAttributeString(kvpAttr.Key, kvpAttr.Value);
          }
        }

        writer.WriteValue(((kvp.Value.Value == null) ? string.Empty : kvp.Value.Value));

        writer.WriteEndElement();
      }

      writer.WriteEndElement();
    }

    public string ParserData(string Name, string Method, IDictionary<string, StructurePair> dic, bool isSync)
    {
      StringBuilder sbBuild = new StringBuilder();

      XmlWriter writer = XmlWriter.Create(sbBuild, new XmlWriterSettings()
      {
        CheckCharacters = true,
        CloseOutput = true,
        ConformanceLevel = ConformanceLevel.Fragment,
        Encoding = Encoding.UTF8,
        Indent = false,
        NewLineOnAttributes = false,
        OmitXmlDeclaration = false
      });

      writer.WriteStartElement("Structure");

      writer.WriteAttributeString("name", Name);

      writer.WriteAttributeString("issync", isSync.ToString());

      writer.WriteAttributeString("method", Method);

      ParsingData(writer, dic);

      foreach (KeyValuePair<string, StructurePair> kvp in dic)
      {
        if ((kvp.Value.DicValuesExtra != null) && (kvp.Value.DicValuesExtra.Count > 0) && (!string.IsNullOrEmpty(kvp.Value.TagExtraName)))
        {
          ParsingDataExtra("ExtraFields", kvp.Value.TagExtraName, writer, kvp.Value.DicValuesExtra);
        }
      }

      writer.WriteEndElement();

      writer.Flush();

      writer.Close();

      return sbBuild.ToString();
    }

    public string ParserData(string Name, string Method, IDictionary<string, StructurePair> dic)
    {
      StringBuilder sbBuild = new StringBuilder();

      XmlWriter writer = XmlWriter.Create(sbBuild, new XmlWriterSettings()
      {
        CheckCharacters = true,
        CloseOutput = true,
        ConformanceLevel = ConformanceLevel.Fragment,
        Encoding = Encoding.UTF8,
        Indent = false,
        NewLineOnAttributes = false,
        OmitXmlDeclaration = false
      });

      writer.WriteStartElement("Structure");

      writer.WriteAttributeString("name", Name);

      writer.WriteAttributeString("method", Method);

      ParsingData(writer, dic);

      foreach (KeyValuePair<string, StructurePair> kvp in dic)
      {
        if ((kvp.Value.DicValuesExtra != null) && (kvp.Value.DicValuesExtra.Count > 0) && (!string.IsNullOrEmpty(kvp.Value.TagExtraName)))
        {
          ParsingDataExtra("ExtraFields", kvp.Value.TagExtraName, writer, kvp.Value.DicValuesExtra);
        }
      }

      writer.WriteEndElement();

      writer.Flush();

      writer.Close();

      return sbBuild.ToString();
    }

    public StructureResponse ResponseParser(string result)
    {
      if (string.IsNullOrEmpty(result))
      {
        return default(StructureResponse);
      }

      StructureResponse respon = new StructureResponse();

      XmlDocument doc = new XmlDocument();
      XmlNode node = null;
      string tmp = null;

      try
      {
        doc.LoadXml(result);

        node = doc.SelectSingleNode("response/status");

        if ((node != null) && (node.NodeType == XmlNodeType.Element))
        {
          tmp = node.InnerText;

          if (tmp.Equals("success", StringComparison.OrdinalIgnoreCase))
          {
            respon.Response = ResponseStatus.Success;
          }
          else if (tmp.Equals("failed", StringComparison.OrdinalIgnoreCase))
          {
            respon.Response = ResponseStatus.Failed;
          }
          else if (tmp.Equals("error", StringComparison.OrdinalIgnoreCase))
          {
            respon.Response = ResponseStatus.Error;
          }
          else
          {
            respon.Response = ResponseStatus.Unknown;
          }
        }

        node = doc.SelectSingleNode("response/message");
        if (node != null)
        {
          node = node.FirstChild;
          if ((node != null) && (node.NodeType == XmlNodeType.CDATA))
          {
            respon.Message = (node.Value ?? (node.InnerText ?? string.Empty));
          }
        }

        node = doc.SelectSingleNode("response/result/data");
        if ((node != null) && (node.NodeType == XmlNodeType.Element))
        {
          Dictionary<string, string> dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

          while (node != null)
          {
            if (node.NodeType == XmlNodeType.Element)
            {
              tmp = XmlReadAttributeValue(node, "name");
              if (dic.ContainsKey(tmp))
              {
                dic[tmp] = node.InnerText;
              }
              else
              {
                dic.Add(tmp, node.InnerText);
              }
            }
            node = node.NextSibling;
          }

          respon.Values = dic;
        }

        respon.IsSet = true;
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibraryInterface.Components.PostDataParser:ResponseParser - {0}", ex.Message);
      }

      return respon;
    }

    private string XmlReadAttributeValue(XmlNode node, string attrName)
    {
      string result = null;

      try
      {
        if (node.Attributes.Count > 0)
        {
          result = (node.Attributes[attrName] == null ? string.Empty : node.Attributes[attrName].Value);
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibraryInterface.Components.PostDataParser:XmlReadAttributeValue - {0}", ex.Message);

        result = string.Empty;
      }

      return result;
    }

    public static string ParserDataNext(string attrName)
    {
      string result = null;

      try
      {
        result = attrName;
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibraryInterface.Components.PostDataParser:XmlReadAttributeValue - {0}", ex.Message);

        result = string.Empty;
      }

      return null;
    }

    public struct StructureResponseNext
    {
      public bool IsSet;
      public ResponseStatus Response;
      public Dictionary<string, string> Values;
      public string Teset;
    }

  }
}
