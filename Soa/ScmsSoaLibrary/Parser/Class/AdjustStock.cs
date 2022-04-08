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
  public class AdjustStockStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static AdjustStockStructure Serialize(string rawData)
    {
      return StructureBase<AdjustStockStructure>.Serialize(rawData);
    }

    public static string Deserialize(AdjustStockStructure strt)
    {
      return StructureBase<AdjustStockStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public AdjustStockStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class AdjustStockStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string AdjustStockID
    { get; set; }

    [XmlAttribute(AttributeName = "Gudang")]
    public string Gudang
    { get; set; }

    [XmlAttribute(AttributeName = "Type")]
    public string Type
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public AdjustStockStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class AdjustStockStructureField
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

    [XmlAttribute(AttributeName = "Batch")]
    public string Batch
    { get; set; }

    [XmlAttribute(AttributeName = "GQty")]
    public decimal GQty
    { get; set; }

    [XmlAttribute(AttributeName = "BQty")]
    public decimal BQty
    { get; set; }

    [XmlAttribute(AttributeName = "KetDet")]
    public string KetDet
    { get; set; }

    [XmlText]
    public string Keterangan
    { get; set; }
  }
}
