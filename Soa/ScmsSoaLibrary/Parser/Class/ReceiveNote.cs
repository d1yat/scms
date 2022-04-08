using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Components;

namespace ScmsSoaLibrary.Parser.Class
{
  #region Pembelian

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class ReceiveNoteStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static ReceiveNoteStructure Serialize(string rawData)
    {
      ReceiveNoteStructure rns = StructureBase<ReceiveNoteStructure>.Serialize(rawData);

      DateTime date = DateTime.Now;

      if (rns != null)
      {
        if ((rns.Fields != null) && (!string.IsNullOrEmpty(rns.Fields.TanggalDO)))
        {
          if (Functionals.DateParser(rns.Fields.TanggalDO, "yyyyMMddHHmmssfff", out date))
          {
            rns.Fields.TanggalDOFormat = date;
          }
          else
          {
            rns.Fields.TanggalDOFormat = DateTime.MinValue;
          }
        }

        if ((rns.Fields != null) && (rns.Fields.Field != null) && (rns.Fields.Field.Length > 0))
        {
          ReceiveNoteStructureField field = null;

          for (int nLoop = 0; nLoop < rns.Fields.Field.Length; nLoop++)
          {
            field = rns.Fields.Field[nLoop];

            if (field != null)
            {
              if (!string.IsNullOrEmpty(field.ExpiredDate))
              {
                if (Functionals.DateParser(field.ExpiredDate, "yyyyMMdd", out date))
                {
                  field.ExpiredDateFormated = date;
                }
                else
                {
                  field.ExpiredDateFormated = DateTime.MinValue;
                }
              }

              field.Batch = (string.IsNullOrEmpty(field.Batch) ? string.Empty : field.Batch.Trim());
            }
          }
        }
      }

      return rns;
    }

    public static string Deserialize(ReceiveNoteStructure strt)
    {
      return StructureBase<ReceiveNoteStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public ReceiveNoteStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class ReceiveNoteStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string ReceiveNoteID
    { get; set; }

    [XmlAttribute(AttributeName = "TypeRN")]
    public string TypeRN
    { get; set; }

    [XmlAttribute(AttributeName = "Gdg")]
    public string Gudang
    { get; set; }

    [XmlAttribute(AttributeName = "Suplier")]
    public string Suplier
    { get; set; }

    [XmlAttribute(AttributeName = "SuplierDO")]
    public string DOPrincipal
    { get; set; }

    [XmlAttribute(AttributeName = "TanggalDO")]
    public string TanggalDO
    { get; set; }

    public DateTime TanggalDOFormat
    { get; set; }

    [XmlAttribute(AttributeName = "Bea")]
    public decimal Bea
    { get; set; }

    [XmlAttribute(AttributeName = "Floating")]
    public bool Floating
    { get; set; }

    [XmlAttribute(AttributeName = "Khusus")]
    public bool OrderKhusus
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    //Indra 20180927FM
    //ST Tiket RN Pembelian
    [XmlAttribute(AttributeName = "NoSerahTerimaTiket")]
    public string NoSerahTerimaTiket
    { get; set; }

    [XmlAttribute(AttributeName = "TipeSTT")]
    public string TipeSTT
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public ReceiveNoteStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class ReceiveNoteStructureField
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

    [XmlAttribute(AttributeName = "Type")]
    public string TypeTrx
    { get; set; }

    [XmlAttribute(AttributeName = "RefID")]
    public string ReferenceID
    { get; set; }

    [XmlAttribute(AttributeName = "Batch")]
    public string Batch
    { get; set; }

    [XmlAttribute(AttributeName = "Expired")]
    public string ExpiredDate
    { get; set; }

    [XmlIgnore]
    public DateTime ExpiredDateFormated
    { get; set; }

    [XmlAttribute(AttributeName = "Qty")]
    public decimal Quantity
    { get; set; }

    [XmlAttribute(AttributeName = "QtyBad")]
    public decimal QuantityBad
    { get; set; }

    [XmlAttribute(AttributeName = "RepackNew")]
    public bool RepackNew
    { get; set; }

    [XmlText]
    public string Keterangan
    { get; set; }
  }

  #endregion

  #region Transfer Gudang

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class ReceiveNoteGudangStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static ReceiveNoteGudangStructure Serialize(string rawData)
    {
      ReceiveNoteGudangStructure rns = StructureBase<ReceiveNoteGudangStructure>.Serialize(rawData);

      DateTime date = DateTime.Now;

      if (rns != null)
      {
        if ((rns.Fields != null) && (rns.Fields.Field != null) && (rns.Fields.Field.Length > 0))
        {
          ReceiveNoteGudangStructureField field = null;

          for (int nLoop = 0; nLoop < rns.Fields.Field.Length; nLoop++)
          {
            field = rns.Fields.Field[nLoop];

            if (field != null)
            {
              if (!string.IsNullOrEmpty(field.AdditionalDate))
              {
                if (Functionals.DateParser(field.AdditionalDate, "yyyyMMdd", out date))
                {
                  field.AdditionalDateFormated = date;
                }
                else
                {
                  field.AdditionalDateFormated = DateTime.MinValue;
                }
              }

              if (!string.IsNullOrEmpty(field.ReferenceDate))
              {
                if (Functionals.DateParser(field.ReferenceDate, "yyyyMMdd", out date))
                {
                  field.ReferenceDateFormated = date;
                }
                else
                {
                  field.ReferenceDateFormated = DateTime.MinValue;
                }
              }

              field.Batch = (string.IsNullOrEmpty(field.Batch) ? string.Empty : field.Batch.Trim());
            }
          }
        }
      }

      return rns;
    }

    public static string Deserialize(ReceiveNoteGudangStructure strt)
    {
      return StructureBase<ReceiveNoteGudangStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public ReceiveNoteGudangStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class ReceiveNoteGudangStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "SuratID")]
    public string SuratID
    { get; set; }

    [XmlAttribute(AttributeName = "TypeRN")]
    public string TypeRN
    { get; set; }

    [XmlAttribute(AttributeName = "Gdg")]
    public string Gudang
    { get; set; }

    [XmlAttribute(AttributeName = "PIN")]
    public string PIN
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public ReceiveNoteGudangStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class ReceiveNoteGudangStructureField
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

    //[XmlAttribute(AttributeName = "Gudang")]
    //public string GudangID
    //{ get; set; }

    [XmlAttribute(AttributeName = "Suplier")]
    public string SuplierID
    { get; set; }

    [XmlAttribute(AttributeName = "ReferenceID")]
    public string ReferenceID
    { get; set; }

    [XmlAttribute(AttributeName = "ReferenceDate")]
    public string ReferenceDate
    { get; set; }

    public DateTime ReferenceDateFormated
    { get; set; }

    [XmlAttribute(AttributeName = "AdditionalID")]
    public string AdditionalID
    { get; set; }

    [XmlAttribute(AttributeName = "AdditionalDate")]
    public string AdditionalDate
    { get; set; }

    public DateTime AdditionalDateFormated
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlAttribute(AttributeName = "IsFloat")]
    public bool IsFloat
    { get; set; }

    [XmlAttribute(AttributeName = "Bea")]
    public decimal Bea
    { get; set; }

    [XmlAttribute(AttributeName = "IsPrint")]
    public bool IsPrint
    { get; set; }

    [XmlAttribute(AttributeName = "IsStatus")]
    public bool IsStatus
    { get; set; }

    [XmlAttribute(AttributeName = "Item")]
    public string Item
    { get; set; }

    [XmlAttribute(AttributeName = "Batch")]
    public string Batch
    { get; set; }

    [XmlAttribute(AttributeName = "Acc")]
    public decimal Acceptance
    { get; set; }

    [XmlAttribute(AttributeName = "Qty")]
    public decimal Quantity
    { get; set; }

    [XmlAttribute(AttributeName = "QtyBad")]
    public decimal QuantityBad
    { get; set; }
  }

  #endregion
}
