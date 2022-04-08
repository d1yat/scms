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
  public class PemusnahanStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static PemusnahanStructure Serialize(string rawData)
    {
      return StructureBase<PemusnahanStructure>.Serialize(rawData); ;
    }

    public static string Deserialize(PemusnahanStructure strt)
    {
      return StructureBase<PemusnahanStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public PemusnahanStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class PemusnahanStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string PMid
    { get; set; }

    [XmlAttribute(AttributeName = "Gudang")]
    public string Gudang
    { get; set; }

    [XmlAttribute(AttributeName = "Memo")]
    public string Memo
    { get; set; }

    [XmlAttribute(AttributeName = "Type")]
    public string TipeTransaksi
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public PemusnahanStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class PemusnahanStructureField
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

    [XmlAttribute(AttributeName = "Item")]
    public string Item
    { get; set; }

    [XmlAttribute(AttributeName = "Qty")]
    public decimal Quantity
    { get; set; }

    [XmlAttribute(AttributeName = "Batch")]
    public string Batch
    { get; set; }

    [XmlText]
    public string KeteranganMod
    { get; set; }
  }
}
