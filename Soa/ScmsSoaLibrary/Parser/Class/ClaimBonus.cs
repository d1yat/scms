using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Components;

namespace ScmsSoaLibrary.Parser.Class
{
  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class ClaimStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static ClaimStructure Serialize(string rawData)
    {
      return StructureBase<ClaimStructure>.Serialize(rawData); ;
    }

    public static string Deserialize(ClaimStructure strt)
    {
      return StructureBase<ClaimStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public ClaimStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class ClaimStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string ClaimID
    { get; set; }

    [XmlAttribute(AttributeName = "Tahun")]
    public string Tahun
    { get; set; }

    [XmlAttribute(AttributeName = "Bulan")]
    public string Bulan
    { get; set; }

    [XmlAttribute(AttributeName = "Suplier")]
    public string Suplier
    { get; set; }

    [XmlAttribute(AttributeName = "Top")]
    public string Top
    { get; set; }

    [XmlAttribute(AttributeName = "KursDesc")]
    public string KursDesc
    { get; set; }

    [XmlAttribute(AttributeName = "KursVal")]
    public string KursVal
    { get; set; }

    [XmlAttribute(AttributeName = "Type")]
    public string Type
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public ClaimStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class ClaimStructureField
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

    [XmlAttribute(AttributeName = "Modified")]
    public bool IsModified
    { get; set; }

    [XmlAttribute(AttributeName = "Item")]
    public string Item
    { get; set; }

    [XmlAttribute(AttributeName = "Quantity")]
    public decimal Quantity
    { get; set; }

    [XmlAttribute(AttributeName = "Disc")]
    public decimal Disc
    { get; set; }

    [XmlAttribute(AttributeName = "Salpri")]
    public decimal Salpri
    { get; set; }

    [XmlText]
    public string KeteranganMod
    { get; set; }
  }

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class ClaimStructureProcess
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static ClaimStructureProcess Serialize(string rawData)
    {
      return StructureBase<ClaimStructureProcess>.Serialize(rawData); ;
    }

    public static string Deserialize(ClaimStructureProcess strt)
    {
      return StructureBase<ClaimStructureProcess>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public ClaimStructureProcessFields Fields
    { get; set; }
  }

  [Serializable]
  public class ClaimStructureProcessFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string ClaimID
    { get; set; }

    [XmlAttribute(AttributeName = "Tahun")]
    public string Tahun
    { get; set; }

    [XmlAttribute(AttributeName = "Bulan")]
    public string Bulan
    { get; set; }

    [XmlAttribute(AttributeName = "Suplier")]
    public string Suplier
    { get; set; }

    [XmlAttribute(AttributeName = "Top")]
    public string Top
    { get; set; }

    [XmlAttribute(AttributeName = "KursDesc")]
    public string KursDesc
    { get; set; }

    [XmlAttribute(AttributeName = "KursVal")]
    public string KursVal
    { get; set; }

    [XmlAttribute(AttributeName = "Type")]
    public string Type
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public ClaimStructureProcessField[] Field
    { get; set; }
  }

  [Serializable]
  public class ClaimStructureProcessField
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

    [XmlAttribute(AttributeName = "Modified")]
    public bool IsModified
    { get; set; }

    [XmlAttribute(AttributeName = "Item")]
    public string Item
    { get; set; }

    [XmlAttribute(AttributeName = "cusno")]
    public string cusno
    { get; set; }

    [XmlAttribute(AttributeName = "Quantity")]
    public decimal Quantity
    { get; set; }

    [XmlAttribute(AttributeName = "nGret")]
    public decimal nGret
    { get; set; }

    [XmlAttribute(AttributeName = "nBret")]
    public decimal nBret
    { get; set; }

    [XmlAttribute(AttributeName = "Salpri")]
    public decimal Salpri
    { get; set; }

    [XmlAttribute(AttributeName = "Disc")]
    public decimal Disc
    { get; set; }

    [XmlText]
    public string KeteranganMod
    { get; set; }
  }
}
