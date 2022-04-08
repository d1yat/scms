using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Channels;
using System.Xml;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Diagnostics;

namespace ScmsSoaLibraryInterface.Core.CustomEncoder
{
  class CustomMessageProperty : IMessageProperty
  {
    public const string Name = "ScmsSoaLibraryInterface.Core.CustomEncoder.CustomMessageProperty";

    public IMessageProperty CreateCopy()
    {
      return new CustomMessageProperty(this);
    }

    public CustomMessageProperty()
    {
    }

    internal CustomMessageProperty(CustomMessageProperty other)
    {
      this.ReformatStringResult = other.ReformatStringResult;
    }

    public bool ReformatStringResult { get; set; }
  }

  class CustomEncoderFactory : MessageEncoderFactory
  {
    CustomEncoder encoder;

    public CustomEncoderFactory()
    {
      encoder = new CustomEncoder();
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

    //Custom encoder
    class CustomEncoder : MessageEncoder
    {
      private MessageEncoder encoder;

      public CustomEncoder()
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

        bool reformatStringResult = false;
        
        if (message.Properties.ContainsKey(CustomMessageProperty.Name))
        {
          reformatStringResult = ((CustomMessageProperty)(message.Properties[CustomMessageProperty.Name])).ReformatStringResult;
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
            string.Format("ScmsSoaLibraryInterface.Core.Custom.CustomEncoderFactory:CustomEncoder:WriteMessage - {0}", ex.Message));

          dataStream.Position = 0;
        }
        finally
        {
          string tmp = null;
          if (reformatStringResult)
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

            //Debug.WriteLine(sbJson.ToString());
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