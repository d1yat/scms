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
  public class AdjustFakturStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static AdjustFakturStructure Serialize(string rawData)
    {
      return StructureBase<AdjustFakturStructure>.Serialize(rawData);
    }

    public static string Deserialize(AdjustFakturStructure strt)
    {
      return StructureBase<AdjustFakturStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public AdjustFakturStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class AdjustFakturStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string AdjustFakturID
    { get; set; }

    [XmlAttribute(AttributeName = "Beban")]
    public string Beban
    { get; set; }

    [XmlAttribute(AttributeName = "Type")]
    public string Type
    { get; set; }

    [XmlAttribute(AttributeName = "SubType")]
    public string SubType
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlAttribute(AttributeName = "KetDel")]
    public string KetDel
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public AdjustFakturStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class AdjustFakturStructureField
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

    [XmlAttribute(AttributeName = "noRef")]
    public string noRef
    { get; set; }

    [XmlAttribute(AttributeName = "TypeDet")]
    public string TypeDet
    { get; set; }

    [XmlAttribute(AttributeName = "Value")]
    public decimal Value
    { get; set; }

    [XmlAttribute(AttributeName = "Ket")]
    public string Ket
    { get; set; }

    [XmlAttribute(AttributeName = "KetDet")]
    public string KetDet
    { get; set; }

    [XmlText]
    public string Keterangan
    { get; set; }
  }
}
