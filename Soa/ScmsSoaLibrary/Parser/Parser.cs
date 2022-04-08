using System;
using System.Collections.Generic;
using System.Text;
using ScmsSoaLibrary.Commons;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Xml;
using ScmsSoaLibraryInterface.Commons;
using System.ComponentModel;

namespace ScmsSoaLibrary.Parser
{
  class Parser
  {
    internal enum InterpreterMethod
    {
      Unknown,
      IsAdd,
      IsUpdate,
      IsDelete,
      IsBarcode
    }

    #region Internal Class

    internal class StructureXmlHeaderParser
    {
      public bool IsSet { get; set; }
      public string ParserName { get; set; }
      public string ClassName { get; set; }
      public List<StructureXmlDetailParser> ListRelated { get; set; }
    }

    internal class StructureXmlDetailParser
    {
      public bool IsSet { get; set; }
      public string Name { get; set; }
      public string Property { get; set; }
      public Type Type { get; set; }
      public int Len { get; set; }
      public bool IsNotNull { get; set; }
      public bool IsAdd { get; set; }
      public bool IsUpdate { get; set; }
      public bool IsList { get; set; }
      public bool IsDynamic { get; set; }
      public bool IsRequired { get; set; }
      public StructureXmlHeaderParser HeaderParser { get; set; }
      public string Value { get; set; }
    }

    internal class StructureDataNamingHeader
    {
      public bool IsSet { get; set; }
      public string Class { get; set; }
      public InterpreterMethod Method { get; set; }
      public string CustomMethod { get; set; }
      public List<StructureDataInputDetail> List { get; set; }
    }

    internal class StructureDataInputDetail
    {
      public bool IsSet { get; set; }
      public string Name { get; set; }
      public string Value { get; set; }
      public bool IsList { get; set; }
      public StructureDataNamingHeader Naming { get; set; }
    }

    #endregion

    string _xmlData = null;

    public Parser() 
    {
      Assembly myAssembly = null;
      StreamReader reader = null;

      try
      {
        myAssembly = Assembly.GetExecutingAssembly();
        reader = new StreamReader(myAssembly.GetManifestResourceStream("ScmsSoaLibrary.Rsrc.Parser.xml"));

        this._xmlData = reader.ReadToEnd();
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibrary.Parser.Parser:Parser - {0}", ex.Message);
        Logger.WriteLine(ex.StackTrace);

        reader = null;
      }
      finally
      {
        if (reader != null)
        {
          reader.Close();
          reader.Dispose();
        }
      }
    }

    ~Parser()
    {
      _xmlData = null;

      GC.Collect();
    }

    public StructureDataNamingHeader DataParser { get; private set; }

    public StructureXmlHeaderParser XmlParser { get; private set; }

    public bool IsPopulated { get; private set; }

    #region Parser Data Input

    private List<StructureDataInputDetail> PopulateParserDataDetail(XmlNode node)
    {
      if ((node == null) || (node.NodeType != XmlNodeType.Element))
      {
        return new List<StructureDataInputDetail>();
      }

      List<StructureDataInputDetail> list = new List<StructureDataInputDetail>();

      StructureDataInputDetail input = null;

      while (node != null)
      {
        if (node.NodeType == XmlNodeType.Element)
        {
          if (node.HasChildNodes && (node.FirstChild.NodeType == XmlNodeType.Element))
          {
            input = new StructureDataInputDetail()
            {
              IsList = true,
              IsSet = true,
              Name = ReaderXml.ReadAttribute(node, "name"),
              Naming = PopulateParserDataHeader(node.FirstChild),
              Value = node.InnerText
            };
          }
          else
          {
            input = new StructureDataInputDetail()
            {
              IsList = false,
              IsSet = true,
              Name = ReaderXml.ReadAttribute(node, "name"),
              Naming = null,
              Value = node.InnerText
            };
          }

          list.Add(input);
        }

        node = node.NextSibling;
      }

      return list;
    }

    private StructureDataNamingHeader PopulateParserDataHeader(XmlNode node)
    {
      if ((node == null) || (node.NodeType != XmlNodeType.Element))
      {
        return new StructureDataNamingHeader();
      }

      StructureDataNamingHeader naming = new StructureDataNamingHeader();

      string tmp = ReaderXml.ReadAttribute(node, "method");

      if (tmp.Equals("add", StringComparison.OrdinalIgnoreCase))
      {
        naming.Method = InterpreterMethod.IsAdd;
      }
      else if (tmp.Equals("modify", StringComparison.OrdinalIgnoreCase))
      {
        naming.Method = InterpreterMethod.IsUpdate;
      }
      else if (tmp.Equals("delete", StringComparison.OrdinalIgnoreCase))
      {
        naming.Method = InterpreterMethod.IsDelete;
      }
      else
      {
        naming.Method = InterpreterMethod.Unknown;
        naming.CustomMethod = tmp;
      }

      naming.Class = ReaderXml.ReadAttribute(node, "name");
      naming.IsSet = true;
      naming.CustomMethod = string.IsNullOrEmpty(naming.CustomMethod) ? ReaderXml.ReadAttribute(node, "issync") : tmp;
      naming.List = PopulateParserDataDetail(node.FirstChild);

      return naming;
    }

    private StructureDataNamingHeader ParserData(string data)
    {
      if (string.IsNullOrEmpty(data))
      {
        return null;
      }

      XmlDocument doc = new XmlDocument();

      XmlNode node = null;

      StructureDataNamingHeader naming = null;

      try
      {
        doc.LoadXml(data);

        node = doc.SelectSingleNode("Structure");

        if (node != null)
        {
          naming = PopulateParserDataHeader(node);
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibrary.Parser.Parser:ParserData - {0}", ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }

      return naming;
    }

    #endregion

    #region Parser Xml

    private List<StructureXmlDetailParser> ParsingDetailData(XmlNode node)
    {
      if ((node == null) || (node.NodeType != XmlNodeType.Element))
      {
        return new List<StructureXmlDetailParser>();
      }

      List<StructureXmlDetailParser> list = new List<StructureXmlDetailParser>();

      StructureXmlDetailParser sdp = null;

      while (node != null)
      {
        if (node.NodeType == XmlNodeType.Element)
        {
          if (node.HasChildNodes && (node.FirstChild.NodeType == XmlNodeType.Element))
          {
            sdp = new StructureXmlDetailParser()
            {
              HeaderParser = new StructureXmlHeaderParser()
              {
                IsSet = true,
                ParserName = node.Name,
                ClassName = ReaderXml.ReadAttribute(node, "class"),
                ListRelated = this.ParsingDetailData(node.FirstChild)
              },
              IsAdd = ReaderXml.ReadAttributeBool(node, "IsAdd"),
              IsDynamic = (string.IsNullOrEmpty(ReaderXml.ReadAttribute(node, "name")) ? true : false),
              IsList = false,
              IsNotNull = ReaderXml.ReadAttributeBool(node, "IsNotNull"),
              IsSet = true,
              IsUpdate = ReaderXml.ReadAttributeBool(node, "IsUpdate"),
              Len = ReaderXml.ReadAttributeInt(node, "Len"),
              Name = ReaderXml.ReadAttribute(node, "name"),
              Property = ReaderXml.ReadAttribute(node, "parser"),
              Type = ReaderXml.ReadAttributeType(node, "type"),
            };
          }
          else
          {
            sdp = new StructureXmlDetailParser()
            {
              HeaderParser = null,
              IsAdd = ReaderXml.ReadAttributeBool(node, "IsAdd"),
              IsDynamic = (string.IsNullOrEmpty(ReaderXml.ReadAttribute(node, "name")) ? true : false),
              IsList = false,
              IsNotNull = ReaderXml.ReadAttributeBool(node, "IsNotNull"),
              IsSet = true,
              IsUpdate = ReaderXml.ReadAttributeBool(node, "IsUpdate"),
              Len = ReaderXml.ReadAttributeInt(node, "Len"),
              Name = ReaderXml.ReadAttribute(node, "name"),
              Property = ReaderXml.ReadAttribute(node, "parser"),
              Type = ReaderXml.ReadAttributeType(node, "type")
            };
          }

          sdp.IsRequired = ((!string.IsNullOrEmpty(sdp.Name)) || sdp.IsNotNull);

          list.Add(sdp);
        }

        node = node.NextSibling;
      }

      return list;
    }

    private StructureXmlHeaderParser ParsingHeaderData(XmlNode node)
    {
      if ((node == null) || (node.NodeType != XmlNodeType.Element))
      {
        return new StructureXmlHeaderParser();
      }

      StructureXmlHeaderParser shp = new StructureXmlHeaderParser();

      shp.IsSet = true;
      shp.ParserName = node.Name;
      
      shp.ClassName = ReaderXml.ReadAttribute(node, "class");
      if (node.HasChildNodes)
      {
        shp.ListRelated = ParsingDetailData(node.FirstChild);
      }

      return shp;
    }

    public void Populate(string data)
    {
      StructureDataNamingHeader naming = ParserData(data);

      if ((naming == null) || (!naming.IsSet))
      {
        return;
      }

      XmlDocument doc = new XmlDocument();
      this.IsPopulated = false;
      XmlNode node = null;

      string className = naming.Class;

      this.DataParser = naming;

      try
      {
        doc.LoadXml(this._xmlData);

        node = doc.SelectSingleNode("Structure/Config");

        if (node != null)
        {
          XmlNode child = null;
          node = node.FirstChild;

          while (node != null)
          {
            if (node.NodeType == XmlNodeType.Element)
            {
              if (ReaderXml.ReadAttribute(node, "name").Equals(className, StringComparison.OrdinalIgnoreCase))
              {
                child = doc.SelectSingleNode(string.Concat("Structure/List/", node.InnerText));

                this.XmlParser = ParsingHeaderData(child);

                break;
              }
              node = node.NextSibling;
            }
          }
        }

        this.IsPopulated = ((this.XmlParser != null) && this.XmlParser.IsSet && (this.XmlParser.ListRelated != null) && (this.XmlParser.ListRelated.Count > 0));
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibrary.Parser.Parser:Populate - {0}", ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }

      doc = null;
    }

    #endregion
  }

  class ReaderXml
  {
    public static string ReadAttribute(XmlNode node, string name)
    {
      string result = string.Empty;

      try
      {
        result = (node.Attributes[name] == null ? string.Empty : node.Attributes[name].Value);
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibrary.Parser.ReaderXml:ReadAttribute - {0}", ex.Message);
        Logger.WriteLine(ex.StackTrace);
        
        result = string.Empty;
      }

      return result;
    }

    public static bool ReadAttributeBool(XmlNode node, string name)
    {
      bool result;

      try
      {
        bool.TryParse((node.Attributes[name] == null ? string.Empty : node.Attributes[name].Value), out result);
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibrary.Parser.ReaderXml:ReadAttributeBool - {0}", ex.Message);
        Logger.WriteLine(ex.StackTrace);

        result = false;
      }

      return result;
    }

    public static System.Type ReadAttributeType(XmlNode node, string name)
    {
      System.Type result;

      try
      {
        result = System.Type.GetType((node.Attributes[name].Value ?? string.Empty));        
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibrary.Parser.ReaderXml:ReadAttributeType - {0}", ex.Message);
        Logger.WriteLine(ex.StackTrace);

        return null;
      }

      return result;
    }
    
    public static int ReadAttributeInt(XmlNode node, string name)
    {
      int result;

      try
      {
        int.TryParse((node.Attributes[name] == null ? string.Empty : node.Attributes[name].Value), out result);
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibrary.Parser.ReaderXml:ReadAttributeInt - {0}", ex.Message);
        Logger.WriteLine(ex.StackTrace);

        result = 0;
      }

      return result;
    }
  }

  class ResponseParser
  {
    internal enum ResponseParserEnum
    {
      Unknown,
      IsSuccess,
      IsFailed,
      IsError
    }

    public static string ResponseGenerator(ResponseParserEnum responseEnum, IDictionary<string, string> dicOutput, string message)
    {
      StringBuilder sbResult = new StringBuilder();

      XmlWriter writer = XmlWriter.Create(sbResult, new XmlWriterSettings()
      {
        CloseOutput = true,
        ConformanceLevel = ConformanceLevel.Fragment,
        Encoding = Encoding.UTF8,
        Indent = false,
        NewLineOnAttributes = false,
        OmitXmlDeclaration = false
      });

      try
      {
        writer.WriteComment("Generated by mpat_tra");

        writer.WriteStartElement("response");

          // 1
          writer.WriteStartElement("status");

        switch (responseEnum)
        {
          case ResponseParserEnum.IsSuccess:
            writer.WriteValue("success");
            break;
          case ResponseParserEnum.IsFailed:
            writer.WriteValue("failed");
            break;
          case ResponseParserEnum.IsError:
            writer.WriteValue("error");
            break;
          default:
            writer.WriteValue("unknown");
            break;
        }
        
        writer.WriteEndElement();
        // 1

        // 2
        writer.WriteStartElement("message");

        if (!string.IsNullOrEmpty(message))
        {
          writer.WriteCData(message);
        }

        writer.WriteEndElement();
        // 2

        if ((dicOutput != null) && (dicOutput.Count > 0))
        {
          // 3
          writer.WriteStartElement("result");

          foreach (KeyValuePair<string, string> kvp in dicOutput)
          {
            writer.WriteStartElement("data");

            writer.WriteAttributeString("name", (string.IsNullOrEmpty(kvp.Key) ? string.Empty : kvp.Key.Trim()));

            writer.WriteValue((string.IsNullOrEmpty(kvp.Value) ? string.Empty : kvp.Value));

            writer.WriteEndElement();
          }

          writer.WriteEndElement();
          // 3
        }

        writer.WriteEndElement();

        writer.Flush();
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibrary.Parser.ResponseParser:ResponseParser - {0}", ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }
      finally
      {
        writer.Close();
      }
        

      return sbResult.ToString();
    }

    public static string ResponseGeneratorMulti(ResponseParserEnum responseEnum, IDictionary<string, List<Functionals.PL1>> dicOutput, string message)
    {
      StringBuilder sbResult = new StringBuilder();

      XmlWriter writer = XmlWriter.Create(sbResult, new XmlWriterSettings()
      {
        CloseOutput = true,
        ConformanceLevel = ConformanceLevel.Fragment,
        Encoding = Encoding.UTF8,
        Indent = false,
        NewLineOnAttributes = false,
        OmitXmlDeclaration = false
      });

      try
      {
          writer.WriteComment(" Generated by mpat_tra ");

          writer.WriteStartElement("response");

        // 1
        writer.WriteStartElement("status");

        switch (responseEnum)
        {
          case ResponseParserEnum.IsSuccess:
            writer.WriteValue("success");
            break;
          case ResponseParserEnum.IsFailed:
            writer.WriteValue("failed");
            break;
          case ResponseParserEnum.IsError:
            writer.WriteValue("error");
            break;
          default:
            writer.WriteValue("unknown");
            break;
        }

        writer.WriteEndElement();
        // 1

        // 2
        writer.WriteStartElement("message");

        if (!string.IsNullOrEmpty(message))
        {
          writer.WriteCData(message);
        }

        writer.WriteEndElement();
        // 2

        if ((dicOutput != null) && (dicOutput.Count > 0))
        {
          // 3
          writer.WriteStartElement("result");



          foreach (KeyValuePair<string, List<Functionals.PL1>> kvp in dicOutput)
          {
            writer.WriteStartElement("data");
            writer.WriteAttributeString("name", (string.IsNullOrEmpty(kvp.Key) ? string.Empty : kvp.Key.Trim()));

            foreach (Functionals.PL1 pl in kvp.Value)
            {
              var sd = pl.GetType().GetProperties();
              

              PropertyInfo[] prop = pl.GetType().GetProperties();
              writer.WriteStartElement("Fields");
             foreach (PropertyInfo attribData in pl.GetType().GetProperties())
             {

               string value = attribData.GetGetMethod().Invoke(pl, null).ToString();
               string Names = attribData.Name;

               writer.WriteAttributeString(Names, value);
               //writer.WriteValue((string.IsNullOrEmpty(value) ? string.Empty : value));

               
             }

             writer.WriteEndElement();
            }
            writer.WriteEndElement();
          }

          writer.WriteEndElement();
          // 3
        }

        writer.WriteEndElement();

        writer.Flush();
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibrary.Parser.ResponseParser:ResponseParser - {0}", ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }
      finally
      {
        writer.Close();
      }


      return sbResult.ToString();
    }
  }
}
