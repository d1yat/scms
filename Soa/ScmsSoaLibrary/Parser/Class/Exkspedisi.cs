using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using ScmsSoaLibrary.Commons;


namespace ScmsSoaLibrary.Parser.Class
{
  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class ExpedisiStructure
  {
    [XmlAttribute(AttributeName="nama")]
    public string Name
    { get; set; } 

    [XmlAttribute(AttributeName="method")]
    public string Method
    { get; set; }

    public bool IsGroupMode
    { get; private set; }

    public static ExpedisiStructure Serialize(string rawData)
    {
      ExpedisiStructure  es = StructureBase<ExpedisiStructure>.Serialize(rawData);
      if (es != null)
      {
        if (!string.IsNullOrEmpty(es.Fields.DResi))
        {
          DateTime date = DateTime.MinValue;

          if (DateTime.TryParseExact(es.Fields.DResi, "yyyyMMddHHmmss", new System.Globalization.CultureInfo("en-US"), System.Globalization.DateTimeStyles.AssumeLocal,
            out date))
          {
            es.Fields.DateResi = date;
          }
        }
      }
      return es;
    }

    public static string Deserialize(ExpedisiStructure strt)
    {
      return StructureBase<ExpedisiStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public ExpedisiStructureFields Fields
    { get; set; }

  }

  [Serializable]
  public class ExpedisiStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string EkspedisiID
    { get; set; }

    [XmlAttribute(AttributeName = "Via")]
    public string Via
    { get; set; }

    [XmlAttribute(AttributeName = "Gudang")]
    public string Gudang
    { get; set; }

    [XmlAttribute(AttributeName = "Customer")]
    public string Customer
    { get; set; }

    [XmlAttribute(AttributeName = "By")]
    public string By
    { get; set; }

    [XmlAttribute(AttributeName = "Eks")]
    public string Eks
    { get; set; }

    [XmlAttribute(AttributeName = "Ket")]
    public string Ket
    { get; set; }

    [XmlAttribute(AttributeName = "Resi")]
    public string Resi
    { get; set; }

    [XmlAttribute(AttributeName = "koli")]
    public string koli
    { get; set; }

    [XmlAttribute(AttributeName = "berat")]
    public string berat
    { get; set; }

    [XmlAttribute(AttributeName = "DResi")]
    public string DResi
    { get; set; }

    //[XmlAttribute(AttributeName = "DResi")]
    //public string DResi
    //{ get; set; }
    
    public DateTime DateResi
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public ExpedisiStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class ExpedisiStructureField
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

    [XmlAttribute(AttributeName = "dono")]
    public string DO
    { get; set; }

    [XmlText]
    public string Keterangan
    { get; set; }
  }
}
