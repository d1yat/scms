using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Channels;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Xml;
using System.ComponentModel;

namespace ScmsSoaLibraryInterface.Core.CustomMessageEncoder
{
  public class CustomMessageProperty : IMessageProperty
  {
    public enum MethodLogical
    {
      Unknown,
      JsonPadding,
      StringResultFormatter
    }

    public const string Name = "ScmsSoaLibraryInterface.Core.CustomMessageEncoder.CustomMessageEncoderProperty";

    public IMessageProperty CreateCopy()
    {
      return new CustomMessageProperty(this);
    }

    public CustomMessageProperty()
    {
    }

    internal CustomMessageProperty(CustomMessageProperty other)
    {
      this.MethodLogic = other.MethodLogic;
      this.MethodName = other.MethodName;
      this.ReturnJsonArray = other.ReturnJsonArray;
      this.ReformatStringResult = other.ReformatStringResult;
    }

    [DefaultValue(MethodLogical.Unknown)]
    public MethodLogical MethodLogic { get; set; }

    public string MethodName { get; set; }

    public bool ReturnJsonArray { get; set; }

    public bool ReformatStringResult { get; set; }
  }

  public class CustomMessageEncoderFactory : MessageEncoderFactory
  {
    CustomMessageEncoderEncoder encoder;

    public CustomMessageEncoderFactory()
    {
      encoder = new CustomMessageEncoderEncoder();
    }

    public override MessageEncoder Encoder
    {
      get
      {
        return encoder;
      }
    }

    public override MessageVersion MessageVersion
    {
      get { return encoder.MessageVersion; }
    }

    //Custom Message encoder
    class CustomMessageEncoderEncoder : MessageEncoder
    {
      private MessageEncoder encoder;

      public CustomMessageEncoderEncoder()
      {
        WebMessageEncodingBindingElement element = new WebMessageEncodingBindingElement();
        encoder = element.CreateMessageEncoderFactory().Encoder;
      }

      public override string ContentType
      {
        get
        {
          return encoder.ContentType;
        }
      }

      public override string MediaType
      {
        get
        {
          return encoder.MediaType;
        }
      }

      public override MessageVersion MessageVersion
      {
        get
        {
          return encoder.MessageVersion;
        }
      }

      public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
      {
        return encoder.ReadMessage(buffer, bufferManager, contentType);
      }

      public override Message ReadMessage(System.IO.Stream stream, int maxSizeOfHeaders, string contentType)
      {
        return encoder.ReadMessage(stream, maxSizeOfHeaders, contentType);
      }

      public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
      {
        MemoryStream dataStream = new MemoryStream();
        XmlWriter writer = null;

        CustomMessageProperty.MethodLogical methodLogic = CustomMessageProperty.MethodLogical.Unknown;
        string methodName = null;
        bool returnJsonArray = false;
        bool reformatStringResult = false;

        if (message.Properties.ContainsKey(CustomMessageProperty.Name))
        {
          CustomMessageProperty custMsgProp = (CustomMessageProperty)(message.Properties[CustomMessageProperty.Name]) as CustomMessageProperty;

          methodLogic = custMsgProp.MethodLogic;
          methodName = custMsgProp.MethodName;
          returnJsonArray = custMsgProp.ReturnJsonArray;
          reformatStringResult = custMsgProp.ReformatStringResult;
        }

        StringBuilder sbJson = new StringBuilder();

        try
        {
          writer = JsonReaderWriterFactory.CreateJsonWriter(dataStream);

          message.WriteMessage(writer);
          writer.Flush();
        }
        catch (Exception ex)
        {
          System.Diagnostics.Debug.WriteLine(
            string.Format("ScmsSoaLibraryInterface.Core.CustomMessageEncoder.CustomMessageEncoderEncoderFactory:CustomMessageEncoderEncoder:WriteMessage - {0}", ex.Message));

          methodName = string.Empty;

          dataStream.Position = 0;
        }
        finally
        {
          string tmp = null;
          string tmpErrorMsg = null;
          Dictionary<string, object> outputDict = null;

          if (methodLogic == CustomMessageProperty.MethodLogical.JsonPadding)
          {
            if ((!string.IsNullOrEmpty(methodName)) && ((dataStream != null) && (dataStream.Length > 0)))
            {
              if (returnJsonArray)
              {
                tmp = Encoding.Default.GetString(dataStream.ToArray());

                if (tmp.IndexOf("\"") == 0)
                {
                  tmp = tmp.Remove(0, 1);
                  if (tmp.LastIndexOf("\"") == (tmp.Length - 1))
                  {
                    tmp = tmp.Remove((tmp.Length - 1), 1);
                  }
                }

                tmp = tmp.Replace("\\\\", "").Replace("\\\"", "\"");

                outputDict = Ext.Net.JSON.Deserialize<Dictionary<string, object>>(tmp);

                if (outputDict.ContainsKey("exception"))
                {
                  tmpErrorMsg = (outputDict["exception"]).ToString();

                  sbJson.AppendFormat("{0}( {{ \"d\":{1}, \"responseText\":\"{2}\" }} )", methodName, tmp, tmpErrorMsg);
                }
                else
                {
                  sbJson.AppendFormat("{0}( {{ \"d\":{1} }} )", methodName, tmp);
                }

                //sbJson.AppendFormat("{0}( {{ \"d\":{1}, \"responseText\":{2} }} )", methodName, tmp, tmpErrorMsg);
              }
              else
              {
                tmp = Encoding.UTF8.GetString(dataStream.ToArray());
                
                outputDict = Ext.Net.JSON.Deserialize<Dictionary<string, object>>(tmp);

                if (outputDict.ContainsKey("exception"))
                {
                  tmpErrorMsg = (outputDict["exception"]).ToString();

                  sbJson.AppendFormat("{0}( {{ \"d\":{1}, \"responseText\":\"{2}\" }} )", methodName, tmp, tmpErrorMsg);
                }
                else
                {
                  sbJson.AppendFormat("{0}( {{ \"d\":{1} }} )", methodName, tmp);
                }

                //sbJson.AppendFormat("{0}( {{ \"d\":{1} }} )", methodName, tmp);

                //sbJson.AppendFormat("{0}( {{ \"d\":{1}, \"responseText\":{2} }} )", methodName, Encoding.UTF8.GetString(dataStream.ToArray()), tmpErrorMsg);
              }
            }
          }
          else if( methodLogic == CustomMessageProperty.MethodLogical.StringResultFormatter)
          {
            tmp = Encoding.Default.GetString(dataStream.ToArray());

            if (tmp.IndexOf("\"") == 0)
            {
              tmp = tmp.Remove(0, 1);
              if (tmp.LastIndexOf("\"") == (tmp.Length - 1))
              {
                tmp = tmp.Remove((tmp.Length - 1), 1);
              }
            }

            sbJson.Append(tmp.Replace("\\\\", "").Replace("\\\"", "\""));
          }

          if (writer != null)
          {
            writer.Close();
          }
        }

        if (dataStream != null)
        {
          dataStream.Close();
          dataStream.Dispose();
        }

        ArraySegment<byte> byteArray = new ArraySegment<byte>(Encoding.UTF8.GetBytes(sbJson.ToString()), 0, sbJson.Length);

        sbJson.Remove(0, sbJson.Length);

        return byteArray;
      }

      public override void WriteMessage(Message message, System.IO.Stream stream)
      {
        ArraySegment<byte> arrseg = this.WriteMessage(message, -1, null, -1);

        stream.Write(arrseg.Array, arrseg.Offset, arrseg.Count);
      }

      public void WriteToStream(Stream stream, string content)
      {
        using (StreamWriter sw = new StreamWriter(stream))
        {
          sw.Write(content);
        }
      }

      public override bool IsContentTypeSupported(string contentType)
      {
        return encoder.IsContentTypeSupported(contentType);
      }
    }
  }
}
