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
  public class PackingListStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static PackingListStructure Serialize(string rawData)
    {
      return StructureBase<PackingListStructure>.Serialize(rawData);
    }

    public static string Deserialize(PackingListStructure strt)
    {
      return StructureBase<PackingListStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public PackingListStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class PackingListStructureFields
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

    [XmlAttribute(AttributeName = "Customer")]
    public string Customer
    { get; set; }

    [XmlAttribute(AttributeName = "Gudang")]
    public string Gudang
    { get; set; }

    [XmlAttribute(AttributeName = "Suplier")]
    public string Suplier
    { get; set; }

    [XmlAttribute(AttributeName = "Type")]
    public string TypePackingList
    { get; set; }

    [XmlAttribute(AttributeName = "TypeCategory")]
    public string TypeKategori
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlAttribute(AttributeName = "Confirm")]
    public bool IsConfirm
    { get; set; }

    [XmlAttribute(AttributeName = "Lantai")]
    public string Lantai
    { get; set; }

    [XmlAttribute(AttributeName = "BaspbNo")]
    public string BaspbNo
    { get; set; }

    [XmlAttribute(AttributeName = "DivPriID")]
    public string DivPriID
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public PackingListStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class PackingListStructureField
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

    [XmlAttribute(AttributeName = "Sp")]
    public string NomorSP
    { get; set; }

    [XmlAttribute(AttributeName = "Item")]
    public string Item
    { get; set; }

    [XmlAttribute(AttributeName = "Batch")]
    public string Batch
    { get; set; }

    [XmlAttribute(AttributeName = "Qty")]
    public decimal Quantity
    { get; set; }

    [XmlAttribute(AttributeName = "isBox")]
    public bool isBox
    { get; set; }

    [XmlAttribute(AttributeName = "isED")]
    public bool isED
    { get; set; }

    [XmlAttribute(AttributeName = "isAccModify")]
    public bool isAccModify
    { get; set; }

    [XmlAttribute(AttributeName = "accket")]
    public string accket
    { get; set; }

    [XmlAttribute(AttributeName = "TipeKonfirmasi")]
    public string ConfirmType
    { get; set; }

    [XmlText]
    public string Keterangan
    { get; set; }
  }
}
