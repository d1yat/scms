using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace ScmsSoaLibrary.Core.Converter
{
  public class JsonDisCoreDateTime : DateTimeConverterBase
  {
    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
    {
      if (value.GetType().Equals(typeof(DateTime)))
      {
        //writer.WriteValue(string.Format("\\/Date({0})\\/", (long)((DateTime)value).ToUniversalTime().Subtract(unixEpoch).TotalMilliseconds));
        writer.WriteValue(((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"));
      }
    }

    public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
    {
      throw new NotImplementedException("ScmsSoaLibrary.Core.Converter.JsonDateTime:ReadJson - Belum di implementasikan (Rudi)");
    }
  }

  public class JsonDateTime : DateTimeConverterBase
  {
    #region Sample Code

    //private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    //[IgnoreDataMember]
    //public DateTime MyDateTime { get; set; }

    //[DataMember(Name = "MyDateTime")]
    //private int MyDateTimeTicks
    //{
    //  get { return (int)(this.MyDateTime - unixEpoch).TotalSeconds; }
    //  set { this.MyDateTime = unixEpoch.AddSeconds(Convert.ToInt32(value)); }
    //}
    
    #endregion

    private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
    {
      if (value.GetType().Equals(typeof(DateTime)))
      {
        //\/Date(1140584400000)\/
        writer.WriteValue(string.Format("\\/Date({0})\\/", (long)((DateTime)value).ToUniversalTime().Subtract(unixEpoch).TotalMilliseconds));
      }
    }

    public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
    {
      throw new NotImplementedException("ScmsSoaLibrary.Core.Converter.JsonDateTime:ReadJson - Belum di implementasikan (Rudi)");
    }
  }

  public class JsonDateTimeFromJS : DateTimeConverterBase
  {
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException("ScmsSoaLibrary.Core.Converter.JsonDateTimeFromJS:WriteJson - Belum di implementasikan (Rudi)");
    }
  }

  public class JsonStringBase : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return (objectType.Equals(typeof(string)) ? true : false);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      return null;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      return;
    }
  }

  public class JsonString : JsonStringBase
  {
    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
    {
      //base.WriteJson(writer, value, serializer);
      if (value.GetType().Equals(typeof(string)))
      {
        string tmp = value.ToString();

        if (!string.IsNullOrEmpty(tmp))
        {
          writer.WriteValue(tmp.Replace("\"", "\'").Replace("'", "\\\'").Trim());
        }
        else
        {
          writer.WriteValue(string.Empty);
        }
      }
    }

    public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
    {
      return base.ReadJson(reader, objectType, existingValue, serializer);
    }
  }
}
