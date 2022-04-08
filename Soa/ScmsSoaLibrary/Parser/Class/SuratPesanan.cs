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
  public class SuratPesananStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static SuratPesananStructure Serialize(string rawData)
    {
      SuratPesananStructure sps = StructureBase<SuratPesananStructure>.Serialize(rawData);

      if (sps != null)
      {
        if ((sps.Fields != null) && (!string.IsNullOrEmpty(sps.Fields.Tanggal)))
        {
          DateTime date = DateTime.Now;

          if (Functionals.DateParser(sps.Fields.Tanggal, "yyyyMMddHHmmssfff", out date))
          {
            sps.Fields.TanggalSP = date;
          }
          else
          {
            sps.Fields.TanggalSP = DateTime.MinValue;
          }
        }
      }

      return sps;
    }

    public static string Deserialize(SuratPesananStructure strt)
    {
      return StructureBase<SuratPesananStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public SuratPesananStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class SuratPesananStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string SuratPesananID
    { get; set; }

    [XmlAttribute(AttributeName = "Customer")]
    public string Customer
    { get; set; }

    [XmlAttribute(AttributeName = "Tanggal")]
    public string Tanggal
    { get; set; }

    [XmlIgnore]
    public DateTime TanggalSP
    { get; set; }

    [XmlAttribute(AttributeName = "SPCabang")]
    public string SPCabang
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlAttribute(AttributeName = "Cek")]
    public bool Cek
    { get; set; }

    [XmlAttribute(AttributeName = "Tipe")]
    public string TipeSP
    { get; set; }

    [XmlAttribute(AttributeName = "D_ETA")]
    public string D_ETA
    { get; set; }

    [XmlAttribute(AttributeName = "D_ETD")]
    public string D_ETD
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public SuratPesananStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class SuratPesananStructureField
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

    [XmlAttribute(AttributeName = "Qty")]
    public decimal Quantity
    { get; set; }

    [XmlAttribute(AttributeName = "Acc")]
    public decimal Acceptance
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string KeteranganEditing
    { get; set; }

    [XmlText]
    public string Keterangan
    { get; set; }

    [XmlAttribute(AttributeName = "NomorSP")]
    public string NomorSP
    { get; set; }

    [XmlAttribute(AttributeName = "StatusSP")]
    public string StatusSP
    { get; set; }
  }
  
  #region JSON Convert

  [Serializable]
  public class SuratPesananResponse
  {
    public string ID { get; set; }

    public DateTime TanggalSP { get; set; }

    public string TanggalSP_Str { get; set; }

    public string C_SPNO { get; set; }

    public string Cabang { get; set; }

    public string TipeSP { get; set; }

    public DateTime D_ETA { get; set; }

    public string D_ETA_str { get; set; }

    public string D_ETD { get; set; }

    public static SuratPesananResponse Deserialize(string rawData)
    {
      return StructureBase<SuratPesananResponse>.SerializeJson(rawData);
    }

    public static string Serialize(SuratPesananResponse strt)
    {
      return StructureBase<SuratPesananResponse>.DeserializeJson(strt);
    }

    public SuratPesananJSONStructureField[] Fields
    { get; set; }
  }

  [Serializable]
  public class SuratPesananJSONStructureField
  {
    public string C_ITENO
    { get; set; }

    public decimal Qty
    { get; set; }

    public decimal Acc
    { get; set; }

    public decimal N_QTYSAL
    { get; set; }

    public string Type
    { get; set; }

    public string C_NOSUP
    { get; set; }

    public string D_ETA
    { get; set; }

  }
  
  #endregion
}
