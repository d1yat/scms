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
  public class PembayaranVchStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }
    
    public static PembayaranVchStructure Serialize(string rawData)
    {
      PembayaranVchStructure sps = StructureBase<PembayaranVchStructure>.Serialize(rawData);

      if (sps != null)
      {
        if (sps.Fields != null)
        {
          DateTime date = DateTime.Now;

          if (!string.IsNullOrEmpty(sps.Fields.TempoGiro))
          {
            if (Functionals.DateParser(sps.Fields.TempoGiro, "yyyyMMddHHmmssfff", out date))
            {
              sps.Fields.TempoGiroDate = date;
            }
            else
            {
              sps.Fields.TempoGiroDate = DateTime.MinValue;
            }
          }
          if (!string.IsNullOrEmpty(sps.Fields.TanggalDebit))
          {
            if (Functionals.DateParser(sps.Fields.TanggalDebit, "yyyyMMddHHmmssfff", out date))
            {
              sps.Fields.TanggalDebitDate = date;
            }
            else
            {
              sps.Fields.TanggalDebitDate = DateTime.MinValue;
            }
          }
        }
      }

      return sps;
    }

    public static string Deserialize(PembayaranVchStructure strt)
    {
      return StructureBase<PembayaranVchStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public PembayaranVchStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class PembayaranVchStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string PembayaranNoteID
    { get; set; }

    [XmlAttribute(AttributeName = "NoteDate")]
    public string TanggalDebit
    { get; set; }
    
    [XmlIgnore]
    public DateTime TanggalDebitDate
    { get; set; }

    [XmlAttribute(AttributeName = "PaymentType")]
    public string JenisPembayaran
    { get; set; }

    [XmlAttribute(AttributeName = "Suplier")]
    public string Suplier
    { get; set; }

    [XmlAttribute(AttributeName = "Customer")]
    public string Customer
    { get; set; }

    [XmlAttribute(AttributeName = "PaymentModeType")]
    public string TipePembayaran
    { get; set; }

    [XmlAttribute(AttributeName = "Bank")]
    public string Bank
    { get; set; }

    [XmlAttribute(AttributeName = "Account")]
    public string Rekening
    { get; set; }

    [XmlAttribute(AttributeName = "GiroNumber")]
    public string NoGiro
    { get; set; }

    [XmlAttribute(AttributeName = "GiroDate")]
    public string TempoGiro
    { get; set; }

    [XmlIgnore]
    public DateTime TempoGiroDate
    { get; set; }

    [XmlAttribute(AttributeName = "Kurs")]
    public string Kurs
    { get; set; }

    [XmlAttribute(AttributeName = "KursValue")]
    public decimal KursValue
    { get; set; }

    [XmlAttribute(AttributeName = "ValuePay")]
    public decimal JumlahPembayaran
    { get; set; }

    [XmlAttribute(AttributeName = "AdminTax")]
    public decimal BiayaAdmin
    { get; set; }

    [XmlAttribute(AttributeName = "IsDownPayment")]
    public bool IsDownPayment
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public PembayaranVchStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class PembayaranVchStructureField
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

    [XmlAttribute(AttributeName = "Faktur")]
    public string Faktur
    { get; set; }

    [XmlAttribute(AttributeName = "TipeFaktur")]
    public string TipeFaktur
    { get; set; }

    [XmlAttribute(AttributeName = "Jumlah")]
    public decimal Jumlah
    { get; set; }

    [XmlText]
    public string Keterangan
    { get; set; }
  }
}
