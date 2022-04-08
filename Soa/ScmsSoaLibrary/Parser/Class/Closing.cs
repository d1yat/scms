using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Components;

namespace ScmsSoaLibrary.Parser.Class
{
  #region Closing

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class ClosingStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static ClosingStructure Serialize(string rawData)
    {
      return StructureBase<ClosingStructure>.Serialize(rawData); ;
    }

    public static string Deserialize(ClosingStructure strt)
    {
      return StructureBase<ClosingStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public ClosingStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class ClosingStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "Tahun")]
    public string Tahun
    { get; set; }

    [XmlAttribute(AttributeName = "Bulan")]
    public bool Bulan
    { get; set; }
  }

  #endregion
}
