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
  public class PackingListAutoStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static PackingListAutoStructure Serialize(string rawData)
    {
      return StructureBase<PackingListAutoStructure>.Serialize(rawData);
    }

    public static string Deserialize(PackingListAutoStructure strt)
    {
      return StructureBase<PackingListAutoStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public PackingListAutoStructureAutoFields Fields
    { get; set; }
  }

  [Serializable]
  public class PackingListAutoStructureAutoFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string PackingListID
    { get; set; }

    [XmlAttribute(AttributeName = "Via")]
    public string Via
    { get; set; }

    [XmlAttribute(AttributeName = "Tipe")]
    public string Tipe
    { get; set; }

    [XmlAttribute(AttributeName = "Customer")]
    public string Customer
    { get; set; }

    [XmlAttribute(AttributeName = "Suplier")]
    public string Suplier
    { get; set; }

    [XmlAttribute(AttributeName = "Gudang")]
    public string Gudang
    { get; set; }

    [XmlAttribute(AttributeName = "NoRN")]
    public string NoRN
    { get; set; }

    [XmlAttribute(AttributeName = "Confirm")]
    public bool IsConfirm
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public PackingListAutoStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class PackingListAutoStructureField
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

    [XmlAttribute(AttributeName = "Rn")]
    public string NomorRN
    { get; set; }

    [XmlAttribute(AttributeName = "Sp")]
    public string NomorSP
    { get; set; }

    [XmlAttribute(AttributeName = "Item")]
    public string Item
    { get; set; }

    [XmlAttribute(AttributeName = "Batch")]
    public string Batch
    { get; set; }

    [XmlAttribute(AttributeName = "qtySP")]
    public decimal qtySP
    { get; set; }

    [XmlAttribute(AttributeName = "qtyRN")]
    public decimal qtyRN
    { get; set; }

    [XmlAttribute(AttributeName = "qtySPAdj")]
    public decimal qtySPAdj
    { get; set; }

    [XmlAttribute(AttributeName = "isED")]
    public bool isED
    { get; set; }

    [XmlAttribute(AttributeName = "accket")]
    public string accket
    { get; set; }

    [XmlText]
    public string Keterangan
    { get; set; }
  }
}

