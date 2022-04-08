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
  public class ClaimAccStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static ClaimAccStructure Serialize(string rawData)
    {
      return StructureBase<ClaimAccStructure>.Serialize(rawData); ;
    }

    public static string Deserialize(ClaimAccStructure strt)
    {
      return StructureBase<ClaimAccStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public ClaimAccStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class ClaimAccStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string ID
    { get; set; }

    [XmlAttribute(AttributeName = "NoPrinsipal")]
    public string NoPrinsipal
    { get; set; }

    [XmlAttribute(AttributeName = "TglPrinsipal")]
    public string TglPrinsipal
    { get; set; }

    [XmlAttribute(AttributeName = "Suplier")]
    public string Suplier
    { get; set; }

    [XmlAttribute(AttributeName = "claimno")]
    public string claimno
    { get; set; }

    [XmlAttribute(AttributeName = "Type")]
    public string Type
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public ClaimAccStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class ClaimAccStructureField
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

    [XmlAttribute(AttributeName = "qtyAcc")]
    public decimal qtyAcc
    { get; set; }

    [XmlAttribute(AttributeName = "qtyTolak")]
    public decimal qtyTolak
    { get; set; }

    [XmlText]
    public string KeteranganMod
    { get; set; }
  }
}
