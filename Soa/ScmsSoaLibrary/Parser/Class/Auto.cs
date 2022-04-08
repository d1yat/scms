using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Components;

namespace ScmsSoaLibrary.Parser.Class
{
  #region DO Prinsipal

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class DOPrinsipalStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static DOPrinsipalStructure Serialize(string rawData)
    {
      DOPrinsipalStructure dops = StructureBase<DOPrinsipalStructure>.Serialize(rawData);

      if (dops != null)
      {
        DateTime date = DateTime.Now;

        if (dops.Fields != null)
        {
          if (!string.IsNullOrEmpty(dops.Fields.TanggalDO))
          {
            if (Functionals.DateParser(dops.Fields.TanggalDO, "yyyyMMddHHmmssfff", out date))
            {
              dops.Fields.TanggalDODate = date;
            }
            else
            {
              dops.Fields.TanggalDODate = Functionals.StandardSqlDateTime;
            }
          }

          if (!string.IsNullOrEmpty(dops.Fields.TanggalFaktur))
          {
            if (Functionals.DateParser(dops.Fields.TanggalFaktur, "yyyyMMddHHmmssfff", out date))
            {
              dops.Fields.TanggalFakturDate = date;
            }
            else
            {
              dops.Fields.TanggalFakturDate = Functionals.StandardSqlDateTime;
            }
          }

          if ((dops.Fields != null) && (dops.Fields.Field != null) && (dops.Fields.Field.Length > 0))
          {
            for (int nLoop = 0, nLen = dops.Fields.Field.Length; nLoop < nLen; nLoop++)
            {
              if (Functionals.DateParser(dops.Fields.Field[nLoop].BatchExpired, "yyyyMMdd", out date))
              {
                dops.Fields.Field[nLoop].BatchExpiredDate = date;
              }
              else
              {
                dops.Fields.Field[nLoop].BatchExpiredDate = Functionals.StandardSqlDateTime;
              }
            }
          }
        }
      }

      return dops;
    }

    public static string Deserialize(DOPrinsipalStructure strt)
    {
      return StructureBase<DOPrinsipalStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public DOPrinsipalStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class DOPrinsipalStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string DOPrinsipalID
    { get; set; }

    [XmlAttribute(AttributeName = "Prinsipal")]
    public string Prinsipal
    { get; set; }

    [XmlAttribute(AttributeName = "TanggalDO")]
    public string TanggalDO
    { get; set; }

    [XmlIgnore]
    public DateTime TanggalDODate
    { get; set; }

    [XmlAttribute(AttributeName = "Faktur")]
    public string Faktur
    { get; set; }

    [XmlAttribute(AttributeName = "TanggalFJ")]
    public string TanggalFaktur
    { get; set; }

    [XmlIgnore]
    public DateTime TanggalFakturDate
    { get; set; }

    [XmlAttribute(AttributeName = "Customer")]
    public string Customer
    { get; set; }

    [XmlAttribute(AttributeName = "Via")]
    public string Via
    { get; set; }

    [XmlAttribute(AttributeName = "TaxNo")]
    public string Pajak
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public DOPrinsipalStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class DOPrinsipalStructureField
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
    public string TipeTransaksi
    { get; set; }

    [XmlAttribute(AttributeName = "Qty")]
    public decimal Quantity
    { get; set; }

    [XmlAttribute(AttributeName = "Disc")]
    public decimal Disc
    { get; set; }

    [XmlAttribute(AttributeName = "ItemSuplier")]
    public string ItemSuplier
    { get; set; }

    [XmlAttribute(AttributeName = "PO")]
    public string NomorPO 
    { get; set; }

    [XmlAttribute(AttributeName = "Batch")]
    public string Batch
    { get; set; }

    [XmlAttribute(AttributeName = "Expired")]
    public string BatchExpired
    { get; set; }

    [XmlAttribute(AttributeName = "IsClaim")]
    public bool IsClaim
    { get; set; }

    [XmlAttribute(AttributeName = "IsPending")]
    public bool IsPending
    { get; set; }
    
    [XmlIgnore]
    public DateTime BatchExpiredDate
    { get; set; }

    [XmlText]
    public string KeteranganMod
    { get; set; }
  }

  #endregion
}
