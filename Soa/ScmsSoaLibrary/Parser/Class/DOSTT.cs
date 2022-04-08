using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using ScmsSoaLibrary.Commons;

namespace ScmsSoaLibrary.Parser.Class
{
  //[Serializable]
  //[XmlRoot(ElementName = "Structure")]
  //public class DOSTTStructure
  //{
  //  [XmlAttribute(AttributeName = "name")]
  //  public string Name
  //  { get; set; }

  //  [XmlAttribute(AttributeName = "method")]
  //  public string Method
  //  { get; set; }

  //  public bool IsGroupMode
  //  { get; private set; }

  //  public static DOSTTStructure Serialize(string rawData)
  //  {
  //    return StructureBase<DOSTTStructure>.Serialize(rawData); ;
  //  }

  //  public static string Deserialize(DOSTTStructure strt)
  //  {
  //    return StructureBase<DOSTTStructure>.Deserialize(strt);
  //  }

  //  [XmlElement(ElementName = "Fields")]
  //  public DOSTTStructureFields Fields
  //  { get; set; }
  //}

  //[Serializable]
  //public class DOSTTStructureFields
  //{
  //  [XmlAttribute(AttributeName = "Entry")]
  //  public string Entry
  //  { get; set; }

  //  [XmlAttribute(AttributeName = "ID")]
  //  public string DOid
  //  { get; set; }

  //  [XmlAttribute(AttributeName = "Via")]
  //  public string Via
  //  { get; set; }

  //  [XmlAttribute(AttributeName = "Customer")]
  //  public string Customer
  //  { get; set; }

  //  [XmlAttribute(AttributeName = "Gudang")]
  //  public string Gudang
  //  { get; set; }

  //  [XmlAttribute(AttributeName = "sttNO")]
  //  public string nopl
  //  { get; set; }

  //  [XmlAttribute(AttributeName = "Type")]
  //  public string TypePackingList
  //  { get; set; }

  //  [XmlAttribute(AttributeName = "Keterangan")]
  //  public string Keterangan
  //  { get; set; }

  //  [XmlElement(ElementName = "Field")]
  //  public DOSTTStructureField[] Field
  //  { get; set; }
  //}

  //[Serializable]
  //public class DOSTTStructureField
  //{
  //  [XmlAttribute(AttributeName = "name")]
  //  public string Name
  //  { get; set; }

  //  [XmlAttribute(AttributeName = "New")]
  //  public bool IsNew
  //  { get; set; }

  //  [XmlAttribute(AttributeName = "Delete")]
  //  public bool IsDelete
  //  { get; set; }

  //  [XmlAttribute(AttributeName = "Item")]
  //  public string Item
  //  { get; set; }

  //  [XmlAttribute(AttributeName = "Qty")]
  //  public decimal Quantity
  //  { get; set; }

  //  [XmlText]
  //  public string KeteranganMod
  //  { get; set; }
  //}
}
