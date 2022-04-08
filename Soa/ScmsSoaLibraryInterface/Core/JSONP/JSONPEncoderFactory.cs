using System;
using System.Text;
using System.ServiceModel.Channels;
using System.IO;
using System.Xml;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Diagnostics;

namespace ScmsSoaLibraryInterface.Core.JSONP
{
  class JSONPMessageProperty : IMessageProperty
  {
    public const string Name = "ScmsSoaLibraryInterface.Core.JSONP.JSONPMessageProperty";

    public IMessageProperty CreateCopy()
    {
      return new JSONPMessageProperty(this);
    }

    public JSONPMessageProperty()
    {
    }

    internal JSONPMessageProperty(JSONPMessageProperty other)
    {
      this.MethodName = other.MethodName;
      this.ReturnJsonArray = other.ReturnJsonArray;
    }

    public string MethodName { get; set; }

    public bool ReturnJsonArray { get; set; }
  }

  class JSONPEncoderFactory : MessageEncoderFactory
  {
    JSONPEncoder encoder;

    public JSONPEncoderFactory()
    {
      encoder = new JSONPEncoder();
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

    //JSONP encoder
    class JSONPEncoder : MessageEncoder
    {
      private MessageEncoder encoder;

      public JSONPEncoder()
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

        string methodName = null;
        bool returnJsonArray = false;
        
        if (message.Properties.ContainsKey(JSONPMessageProperty.Name))
        {
          methodName = ((JSONPMessageProperty)(message.Properties[JSONPMessageProperty.Name])).MethodName;
          returnJsonArray = ((JSONPMessageProperty)(message.Properties[JSONPMessageProperty.Name])).ReturnJsonArray;
        }

        StringBuilder sbJson = new StringBuilder();

        try
        {
          writer = JsonReaderWriterFactory.CreateJsonWriter(dataStream);

          message.WriteMessage(writer);
          writer.Flush();

          //stream.Write(dataStream.ToArray(), 0, (int)dataStream.Length);
          //dataStream.WriteTo(stream);
        }
        catch (Exception ex)
        {
          Debug.WriteLine(
            string.Format("ScmsSoaLibraryInterface.Core.JSONP.JSONPEncoderFactory:JSONPEncoder:WriteMessage - {0}", ex.Message));

          methodName = string.Empty;

          dataStream.Position = 0;
        }
        finally
        {
          if ((!string.IsNullOrEmpty(methodName)) && ((dataStream != null) && (dataStream.Length > 0)))
          {
            string tmp = null;
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

              //if (tmp.Contains("/Date("))
              //{
              //  tmp = tmp.Replace("/Date(", "\\/Date(").Replace(")/", ")\\/");
              //}

              #region Old Coded

              //Debug.Write(tmp);

              ////sbJson.Append(string.Concat(methodName, "({\"d\":"));
              //sbJson.AppendFormat(string.Concat(methodName, "( "));

              ////stream.Write(Encoding.UTF8.GetBytes(methodName), 0, methodName.Length);

              //sbJson.Append(tmp);

              //sbJson.Append(")");
              ////sbJson.Append("})");

              #endregion

              sbJson.AppendFormat("{0}( {{\"d\":{1}}} )", methodName, tmp);

              //Debug.WriteLine(sbJson.ToString());
            }
            else
            {
              sbJson.AppendFormat("{0}( {{\"d\":{1}}} )", methodName, Encoding.UTF8.GetString(dataStream.ToArray()));
            }
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

        //byte[] messageBytes = stream.ToArray();
        //int messageLength = (int)stream.Length;

        ArraySegment<byte> byteArray = new ArraySegment<byte>(Encoding.UTF8.GetBytes(sbJson.ToString()), 0, sbJson.Length);

        sbJson.Remove(0, sbJson.Length);

        return byteArray;
      }

      //public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
      //{
      //  MemoryStream stream = new MemoryStream();
      //  StreamWriter sw = new StreamWriter(stream);

      //  string methodName = null;
      //  if (message.Properties.ContainsKey(JSONPMessageProperty.Name))
      //    methodName = ((JSONPMessageProperty)(message.Properties[JSONPMessageProperty.Name])).MethodName;

      //  if (methodName != null)
      //  {
      //    sw.Write(methodName + "( ");
      //    //sw.Flush();
      //  }

      //  XmlWriter writer = JsonReaderWriterFactory.CreateJsonWriter(stream);
      //  message.WriteMessage(writer);
      //  //writer.Flush();

      //  if (methodName != null)
      //  {
      //    sw.Write(" );");
      //    //sw.Flush();
      //  }

      //  byte[] messageBytes = stream.GetBuffer();
      //  int messageLength = (int)stream.Position;
      //  int totalLength = messageLength + messageOffset;
      //  byte[] totalBytes = bufferManager.TakeBuffer(totalLength);
      //  Array.Copy(messageBytes, 0, totalBytes, messageOffset, messageLength);

      //  ArraySegment<byte> byteArray = new ArraySegment<byte>(totalBytes, messageOffset, messageLength);
        
      //  writer.Close();

      //  sw.Close();
      //  sw.Dispose();

      //  stream.Close();
      //  stream.Dispose();

      //  return byteArray;
      //}

      public override void WriteMessage(Message message, System.IO.Stream stream)
      {
        #region Old Coded

        //string methodName = null;
        //if (message.Properties.ContainsKey(JSONPMessageProperty.Name))
        //  methodName = ((JSONPMessageProperty)(message.Properties[JSONPMessageProperty.Name])).MethodName;

        //if (methodName == null)
        //{
        //  encoder.WriteMessage(message, stream);
        //  return;
        //}

        //WriteToStream(stream, methodName + "( ");
        //encoder.WriteMessage(message, stream);
        //WriteToStream(stream, " );");

        #endregion
        
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
