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
  public class TranStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static TranStructure Serialize(string rawData)
    {
      return StructureBase<TranStructure>.Serialize(rawData); ;
    }

    public static string Deserialize(TranStructure strt)
    {
      return StructureBase<TranStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public TranStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class TranStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string sjID
    { get; set; }

    [XmlAttribute(AttributeName = "Via")]
    public string Via
    { get; set; }

    [XmlAttribute(AttributeName = "Supplier")]
    public string Supplier
    { get; set; }

    [XmlAttribute(AttributeName = "From")]
    public string From
    { get; set; }

    [XmlAttribute(AttributeName = "To")]
    public string To
    { get; set; }

    [XmlAttribute(AttributeName = "SJno")]
    public string sjno
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlAttribute(AttributeName = "NoRN")]
    public string NoRN
    { get; set; }

    [XmlAttribute(AttributeName = "Confirm")]
    public bool IsConfirm
    { get; set; }

    [XmlAttribute(AttributeName = "TypeCategory")]
    public string TypeKategori
    { get; set; }

    [XmlAttribute(AttributeName = "TypeSJ")]
    public string TypeSJ
    { get; set; }

    [XmlAttribute(AttributeName = "TypeLantai")]
    public string TypeLantai
    { get; set; }

    [XmlAttribute(AttributeName = "noDok")]
    public string noDok
    { get; set; }

    [XmlAttribute(AttributeName = "asalProduk")]
    public string asalProduk
    { get; set; }

    [XmlAttribute(AttributeName = "cabangExp")]
    public string cabangExp
    { get; set; }

    [XmlAttribute(AttributeName = "memo")]
    public string memo
    { get; set; }

    [XmlAttribute(AttributeName = "StatusSJ")]
    public bool StatusSJ
    { get; set; }

    [XmlAttribute(AttributeName = "isDisposal")]
    public bool isDisposal
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public TranStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class TranStructureField
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

    [XmlAttribute(AttributeName = "spgno")]
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

    [XmlAttribute(AttributeName = "nQtyAdj")]
    public decimal nQtyAdj
    { get; set; }

    [XmlAttribute(AttributeName = "BQty")]
    public decimal BadQuantity
    { get; set; }

    [XmlAttribute(AttributeName = "sGQty")]
    public decimal sGQty
    { get; set; }

    [XmlAttribute(AttributeName = "sBQty")]
    public decimal sBQty
    { get; set; }

    [XmlAttribute(AttributeName = "tipe_dc")]
    public string tipe_dc
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

    [XmlText]
    public string KeteranganMod
    { get; set; }
  }
}
