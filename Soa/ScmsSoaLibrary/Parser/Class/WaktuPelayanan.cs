using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ScmsSoaLibraryInterface.Components;

namespace ScmsSoaLibrary.Parser.Class
{
  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class WaktuPelayananStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static WaktuPelayananStructure Serialize(string rawData)
    {
      return StructureBase<WaktuPelayananStructure>.Serialize(rawData);
    }

    public static string Deserialize(WaktuPelayananStructure strt)
    {
      return StructureBase<WaktuPelayananStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public WaktuPelayananStructureFields Fields
    { get; set; }
  }

    [Serializable]
    public class WaktuPelayananStructureFields
    {
      [XmlAttribute(AttributeName = "name")]
      public string Name
      { get; set; }

      [XmlAttribute(AttributeName = "ID")]
      public string WpID
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

      [XmlAttribute(AttributeName = "NoDoc")]
      public string NoDoc
      { get; set; }

      [XmlAttribute(AttributeName = "Entry")]
      public string Entry
      { get; set; }

      [XmlAttribute(AttributeName = "EntryName")]
      public string EntryName
      { get; set; }

      [XmlAttribute(AttributeName = "Give")]
      public string Give
      { get; set; }

      [XmlAttribute(AttributeName = "GiveName")]
      public string GiveName
      { get; set; }

      [XmlAttribute(AttributeName = "Gudang")]
      public string Gudang
      { get; set; }

      [XmlAttribute(AttributeName = "Tipe")]
      public string Tipe
      { get; set; }

      [XmlAttribute(AttributeName = "Koli")]
      public decimal Koli
      { get; set; }

      [XmlAttribute(AttributeName = "Receh")]
      public decimal Receh
      { get; set; }

      [XmlAttribute(AttributeName = "KoliReceh")]
      public decimal KoliReceh
      { get; set; }

      [XmlAttribute(AttributeName = "Berat")]
      public decimal Berat
      { get; set; }

      [XmlAttribute(AttributeName = "Volume")]
      public decimal Volume
      { get; set; }

      [XmlAttribute(AttributeName = "Resi")]
      public string Resi
      { get; set; }

      [XmlAttribute(AttributeName = "Nosup")]
      public string Nosup
      { get; set; }

      [XmlAttribute(AttributeName = "Nopol")]
      public string Nopol
      { get; set; }

      [XmlAttribute(AttributeName = "Cusno")]
      public string Cusno
      { get; set; }

      [XmlElement(ElementName = "Field")]
      public WaktuPelayananStructureField[] Field
      { get; set; }

      [XmlAttribute(AttributeName = "Scan")]
      public string Scan
      { get; set; }

      [XmlAttribute(AttributeName = "ScanName")]
      public string ScanName
      { get; set; }
    }

    [Serializable]
    public class WaktuPelayananStructureField
    {
        [XmlAttribute(AttributeName = "TransNo")]
        public string TransNo
        { get; set; }

        [XmlAttribute(AttributeName = "karton")]
        public decimal karton
        { get; set; }
        
        [XmlAttribute(AttributeName = "receh")]
        public decimal receh
        { get; set; }

        [XmlAttribute(AttributeName = "editkoli")]
        public string editkoli
        { get; set; }
  
      [XmlAttribute(AttributeName = "Keterangan")]
      public string Keterangan
      { get; set; }

      [XmlAttribute(AttributeName = "New")]
      public bool IsNew
      { get; set; }

      [XmlAttribute(AttributeName = "Delete")]
      public bool IsDelete
      { get; set; }

      [XmlAttribute(AttributeName = "ModifiedKoli")]
      public bool IsModifiedKoli
      { get; set; }

    }
}
