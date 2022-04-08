using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Components;

namespace ScmsSoaLibrary.Parser.Class
{
  #region RS

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class ReturSupplierStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static ReturSupplierStructure Serialize(string rawData)
    {
      return StructureBase<ReturSupplierStructure>.Serialize(rawData);
    }

    public static string Deserialize(ReturSupplierStructure strt)
    {
      return StructureBase<ReturSupplierStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public ReturSupplierStructureFields Fields
    { get; set; }

  }

  [Serializable]
  public class ReturSupplierStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string ReturSupplierID
    { get; set; }

    [XmlAttribute(AttributeName = "Gudang")]
    public string Gudang
    { get; set; }

    [XmlAttribute(AttributeName = "Supplier")]
    public string Supplier
    { get; set; }

    [XmlAttribute(AttributeName = "TipeRS")]
    public string TipeRetur
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public ReturSupplierStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class ReturSupplierStructureField
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

    [XmlAttribute(AttributeName = "ketD")]
    public string ketD
    { get; set; }

    [XmlAttribute(AttributeName = "CprNo")]
    public string CprNo
    { get; set; }

    [XmlAttribute(AttributeName = "Cabang")]
    public string Cabang
    { get; set; }

    [XmlAttribute(AttributeName = "Outlet")]
    public string Outlet
    { get; set; }

    [XmlAttribute(AttributeName = "Reason")]
    public string Reason
    { get; set; }

    [XmlAttribute(AttributeName = "RSId")]
    public string RsId
    { get; set; }

    [XmlAttribute(AttributeName = "ConfirmDisp")]
    public bool ConfirmDisp
    { get; set; }

    [XmlText]
    public string Keterangan
    { get; set; }
  }

  #endregion

  #region RS Conf

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class ReturSupplierConfStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static ReturSupplierConfStructure Serialize(string rawData)
    {
      return StructureBase<ReturSupplierConfStructure>.Serialize(rawData);
    }

    public static string Deserialize(ReturSupplierConfStructure strt)
    {
      return StructureBase<ReturSupplierConfStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public ReturSupplierConfStructureFields Fields
    { get; set; }

  }

  [Serializable]
  public class ReturSupplierConfStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string ReturSupplierID
    { get; set; }

    [XmlAttribute(AttributeName = "Gudang")]
    public string Gudang
    { get; set; }

    [XmlAttribute(AttributeName = "Supplier")]
    public string Supplier
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlAttribute(AttributeName = "NoCPR")]
    public string NoCPR
    { get; set; }

    [XmlAttribute(AttributeName = "NoRS1")]
    public string NoRS1
    { get; set; }

    [XmlAttribute(AttributeName = "NoRS2")]
    public string NoRS2
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public ReturSupplierConfStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class ReturSupplierConfStructureField
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

    [XmlAttribute(AttributeName = "NORS")]
    public string NORS
    { get; set; }

    [XmlAttribute(AttributeName = "Item")]
    public string Item
    { get; set; }

    [XmlAttribute(AttributeName = "Batch")]
    public string Batch
    { get; set; }

    [XmlAttribute(AttributeName = "nGQty")]
    public decimal nGQty
    { get; set; }

    [XmlAttribute(AttributeName = "nBQty")]
    public decimal nBQty
    { get; set; }

    [XmlAttribute(AttributeName = "nQty")]
    public decimal nQty
    { get; set; }

    [XmlAttribute(AttributeName = "nQtyRej")]
    public decimal nQtyRej
    { get; set; }

    [XmlAttribute(AttributeName = "nQtyRew")]
    public decimal nQtyRew
    { get; set; }

    [XmlAttribute(AttributeName = "nQtyRed")]
    public decimal nQtyRed
    { get; set; }

    [XmlAttribute(AttributeName = "isAlternate")]
    public bool isAlternate
    { get; set; }

    [XmlText]
    public string Keterangan
    { get; set; }
  }

  #endregion
}
