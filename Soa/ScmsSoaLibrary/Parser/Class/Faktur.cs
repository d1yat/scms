using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Components;

namespace ScmsSoaLibrary.Parser.Class
{
  #region Jual

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class FakturJualStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static FakturJualStructure Serialize(string rawData)
    {
      FakturJualStructure fjs = StructureBase<FakturJualStructure>.Serialize(rawData);

      if (fjs != null)
      {
        if (fjs.Fields != null)
        {
          DateTime date = DateTime.Now;

          if (!string.IsNullOrEmpty(fjs.Fields.TanggalFaktur))
          {
            if (Functionals.DateParser(fjs.Fields.TanggalFaktur, "yyyyMMddHHmmssfff", out date))
            {
              fjs.Fields.TanggalFakturDate = date;
            }
            else
            {
              fjs.Fields.TanggalFakturDate = Functionals.StandardSqlDateTime;
            }
          }
          if (!string.IsNullOrEmpty(fjs.Fields.TanggalTax))
          {
            if (Functionals.DateParser(fjs.Fields.TanggalTax, "yyyyMMddHHmmssfff", out date))
            {
              fjs.Fields.TanggalTaxDate = date;
            }
            else
            {
              fjs.Fields.TanggalTaxDate = Functionals.StandardSqlDateTime;
            }
          }
        }
      }

      return fjs;
    }

    public static string Deserialize(FakturJualStructure strt)
    {
      return StructureBase<FakturJualStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public FakturJualStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class FakturJualStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string FakturID
    { get; set; }

    [XmlAttribute(AttributeName = "FakturDate")]
    public string TanggalFaktur
    { get; set; }
    
    [XmlIgnore]
    public DateTime TanggalFakturDate
    { get; set; }

    [XmlAttribute(AttributeName = "TaxNo")]
    public string NoTax
    { get; set; }

    [XmlAttribute(AttributeName = "TaxDate")]
    public string TanggalTax
    { get; set; }

    [XmlIgnore]
    public DateTime TanggalTaxDate
    { get; set; }

    [XmlAttribute(AttributeName = "Kurs")]
    public string Kurs
    { get; set; }

    [XmlAttribute(AttributeName = "KursValue")]
    public decimal KursValue
    { get; set; }

    [XmlAttribute(AttributeName = "Top")]
    public int Top
    { get; set; }

    [XmlAttribute(AttributeName = "TopPjg")]
    public int TopPjg
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public FakturJualStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class FakturJualStructureField
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

    [XmlAttribute(AttributeName = "Bonus")]
    public decimal Bonus
    { get; set; }

    [XmlAttribute(AttributeName = "Disc")]
    public decimal Discount
    { get; set; }

    [XmlAttribute(AttributeName = "Price")]
    public decimal Harga
    { get; set; }

    [XmlText]
    public string KeteranganMod
    { get; set; }
  }

  #endregion
  
  #region Beli

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class FakturBeliStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static FakturBeliStructure Serialize(string rawData)
    {
      FakturBeliStructure fbs = StructureBase<FakturBeliStructure>.Serialize(rawData);

      if (fbs != null)
      {
        DateTime date = DateTime.Now;

        if (fbs.Fields != null)
        {

          if (!string.IsNullOrEmpty(fbs.Fields.TanggalFaktur))
          {
            if (Functionals.DateParser(fbs.Fields.TanggalFaktur, "yyyyMMddHHmmssfff", out date))
            {
              fbs.Fields.TanggalFakturDate = date;
            }
            else
            {
              fbs.Fields.TanggalFakturDate = Functionals.StandardSqlDateTime;
            }
          }

          if (!string.IsNullOrEmpty(fbs.Fields.TanggalTax))
          {
            if (Functionals.DateParser(fbs.Fields.TanggalTax, "yyyyMMddHHmmssfff", out date))
            {
              fbs.Fields.TanggalTaxDate = date;
            }
            else
            {
              fbs.Fields.TanggalTaxDate = Functionals.StandardSqlDateTime;
            }
          }

          if ((fbs.ExtraFields != null) && (fbs.ExtraFields.FieldBea != null) && (fbs.ExtraFields.FieldBea.Length > 0))
          {
            for (int nLoop = 0, nLen = fbs.ExtraFields.FieldBea.Length; nLoop < nLen; nLoop++)
            {
              if (Functionals.DateParser(fbs.ExtraFields.FieldBea[nLoop].Tanggal, "yyyyMMddHHmmssfff", out date))
              {
                fbs.ExtraFields.FieldBea[nLoop].TanggalDate = date;
              }
              else
              {
                fbs.ExtraFields.FieldBea[nLoop].TanggalDate = Functionals.StandardSqlDateTime;
              }
            }

            //int nLoopC = 0,
            //  nLenC = 0;
            //for (int nLoop = 0, nLen = fbs.ExtraFields.Length; nLoop < nLen; nLoop++)
            //{
            //  if (fbs.ExtraFields[nLoop].Name.Equals("FieldBea", StringComparison.OrdinalIgnoreCase))
            //  {
            //    if ((fbs.ExtraFields[nLoop].FieldBea != null) && (fbs.ExtraFields[nLoop].FieldBea.Length > 0))
            //    {
            //      for (nLoopC = 0, nLen = fbs.ExtraFields[nLoop].FieldBea.Length; nLoopC < nLenC; nLoopC++)
            //      {
            //        if (Functionals.DateParser(fbs.ExtraFields[nLoop].FieldBea[nLoopC].Tanggal, "yyyyMMddHHmmssfff", out date))
            //        {
            //          fbs.ExtraFields[nLoop].FieldBea[nLoopC].TanggalDate = date;
            //        }
            //        else
            //        {
            //          fbs.ExtraFields[nLoop].FieldBea[nLoopC].TanggalDate = Functionals.StandardSqlDateTime;
            //        }
            //      }
            //    }
            //  }
            //}
          }
        }
      }

      return fbs;
    }

    public static string Deserialize(FakturBeliStructure strt)
    {
      return StructureBase<FakturBeliStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public FakturBeliStructureFields Fields
    { get; set; }

    [XmlElement(ElementName = "ExtraFields")]
    public FakturBeliStructureExtraField ExtraFields
    { get; set; }
  }

  [Serializable]
  public class FakturBeliStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string FakturID
    { get; set; }

    [XmlAttribute(AttributeName = "Faktur")]
    public string FakturPrincipal
    { get; set; }

    [XmlAttribute(AttributeName = "FakturDate")]
    public string TanggalFaktur
    { get; set; }

    [XmlIgnore]
    public DateTime TanggalFakturDate
    { get; set; }

    [XmlAttribute(AttributeName = "Suplier")]
    public string Suplier
    { get; set; }

    [XmlAttribute(AttributeName = "NoReceive")]
    public string NoReceive
    { get; set; }

    [XmlAttribute(AttributeName = "TaxNo")]
    public string NoTax
    { get; set; }

    [XmlAttribute(AttributeName = "TaxDate")]
    public string TanggalTax
    { get; set; }

    [XmlIgnore]
    public DateTime TanggalTaxDate
    { get; set; }

    [XmlAttribute(AttributeName = "Kurs")]
    public string Kurs
    { get; set; }

    [XmlAttribute(AttributeName = "KursValue")]
    public decimal KursValue
    { get; set; }

    [XmlAttribute(AttributeName = "ExtraDiscount")]
    public decimal XDisc
    { get; set; }

    [XmlAttribute(AttributeName = "ExtraDiscountVal")]
    public decimal XDiscVal
    { get; set; }

    [XmlAttribute(AttributeName = "Top")]
    public int Top
    { get; set; }

    [XmlAttribute(AttributeName = "TopPjg")]
    public int TopPjg
    { get; set; }

    [XmlAttribute(AttributeName = "ValueFaktur")]
    [System.ComponentModel.DefaultValue(0)]
    public decimal ValueFaktur
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlAttribute(AttributeName = "Gudang")]
    public string Gudang
    { get; set; }

    [XmlAttribute(AttributeName = "n_ppph")]
    public decimal Npph
    { get; set; }

    [XmlAttribute(AttributeName = "x_pph")]
    public decimal Xpph
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public FakturBeliStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class FakturBeliStructureField
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
    public string TipeBarang
    { get; set; }

    [XmlAttribute(AttributeName = "Qty")]
    public decimal Quantity
    { get; set; }

    [XmlAttribute(AttributeName = "Disc")]
    public decimal Discount
    { get; set; }

    [XmlAttribute(AttributeName = "Price")]
    public decimal Harga
    { get; set; }

    [XmlAttribute(AttributeName = "Bea")]
    public decimal Bea
    { get; set; }

    [XmlAttribute(AttributeName = "n_ppph")]
    public decimal n_ppph
    { get; set; }

    [XmlAttribute(AttributeName = "n_discextra")]
    public decimal n_discextra
    { get; set; }

    [XmlText]
    public string KeteranganMod
    { get; set; }
  }

  [Serializable]
  public class FakturBeliStructureExtraField
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlElement(ElementName = "FieldBea")]
    public FakturBeliBeaStructureField[] FieldBea
    { get; set; }
  }
  
  [Serializable]
  public class FakturBeliBeaStructureField
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

    [XmlAttribute(AttributeName = "Tipe")]
    public string TypeBea
    { get; set; }

    [XmlAttribute(AttributeName = "Expeditur")]
    public string Expeditur
    { get; set; }

    [XmlAttribute(AttributeName = "Tanggal")]
    public string Tanggal
    { get; set; }

    [XmlAttribute(AttributeName = "n_ppph")]
    public decimal n_ppph
    { get; set; }

    [XmlIgnore]
    public DateTime TanggalDate
    { get; set; }

    [XmlAttribute(AttributeName = "Value")]
    public decimal Value
    { get; set; }

    [XmlText]
    public string KeteranganMod
    { get; set; }
  }

  #endregion

  #region Jual Retur

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class FakturJualReturStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static FakturJualReturStructure Serialize(string rawData)
    {
      FakturJualReturStructure fjs = StructureBase<FakturJualReturStructure>.Serialize(rawData);

      if (fjs != null)
      {
        DateTime date = DateTime.Now;

        if (fjs.Fields != null)
        {
          if (!string.IsNullOrEmpty(fjs.Fields.TanggalFaktur))
          {
            if (Functionals.DateParser(fjs.Fields.TanggalFaktur, "yyyyMMddHHmmssfff", out date))
            {
              fjs.Fields.TanggalFakturDate = date;
            }
            else
            {
              fjs.Fields.TanggalFakturDate = Functionals.StandardSqlDateTime;
            }
          }
          if (!string.IsNullOrEmpty(fjs.Fields.TanggalTax))
          {
            if (Functionals.DateParser(fjs.Fields.TanggalTax, "yyyyMMddHHmmssfff", out date))
            {
              fjs.Fields.TanggalTaxDate = date;
            }
            else
            {
              fjs.Fields.TanggalTaxDate = Functionals.StandardSqlDateTime;
            }
          }
        }

        if ((fjs.ExtraFields != null) && (fjs.ExtraFields.ProcessField != null) && (fjs.ExtraFields.ProcessField.Length > 0))
        {
          for (int nLoop = 0, nLen = fjs.ExtraFields.ProcessField.Length; nLoop < nLen; nLoop++)
          {
            if (Functionals.DateParser(fjs.ExtraFields.ProcessField[nLoop].TanggalRetur, "yyyyMMddHHmmssfff", out date))
            {
              fjs.ExtraFields.ProcessField[nLoop].TanggalReturDate = date;
            }
            else
            {
              fjs.ExtraFields.ProcessField[nLoop].TanggalReturDate = Functionals.StandardSqlDateTime;
            }
          }
        }
      }

      return fjs;
    }

    public static string Deserialize(FakturJualReturStructure strt)
    {
      return StructureBase<FakturJualReturStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public FakturJualReturStructureFields Fields
    { get; set; }

    [XmlElement(ElementName = "ExtraFields")]
    public FakturJualReturStructureExtraField ExtraFields
    { get; set; }
  }

  [Serializable]
  public class FakturJualReturStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string FakturID
    { get; set; }

    [XmlAttribute(AttributeName = "FakturDate")]
    public string TanggalFaktur
    { get; set; }

    [XmlIgnore]
    public DateTime TanggalFakturDate
    { get; set; }

    [XmlAttribute(AttributeName = "Customer")]
    public string Customer
    { get; set; }

    [XmlAttribute(AttributeName = "XFaktur")]
    public string ExFaktur
    { get; set; }

    [XmlAttribute(AttributeName = "TaxNo")]
    public string NoTax
    { get; set; }

    [XmlAttribute(AttributeName = "TaxDate")]
    public string TanggalTax
    { get; set; }

    [XmlIgnore]
    public DateTime TanggalTaxDate
    { get; set; }

    [XmlAttribute(AttributeName = "Kurs")]
    public string Kurs
    { get; set; }

    [XmlAttribute(AttributeName = "KursValue")]
    public decimal KursValue
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlAttribute(AttributeName = "IsCabang")]
    public bool IsCabang
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public FakturJualReturStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class FakturJualReturStructureField
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

    [XmlAttribute(AttributeName = "Disc")]
    public decimal Discount
    { get; set; }

    [XmlAttribute(AttributeName = "Price")]
    public decimal Harga
    { get; set; }

    [XmlText]
    public string KeteranganMod
    { get; set; }
  }
  
  [Serializable]
  public class FakturJualReturStructureExtraField
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlElement(ElementName = "ProcessField")]
    public FakturJualReturProcessStructureField[] ProcessField
    { get; set; }
  }
  
  [Serializable]
  public class FakturJualReturProcessStructureField
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "New")]
    public bool IsNew
    { get; set; }

    [XmlAttribute(AttributeName = "Customer")]
    public string Customer
    { get; set; }

    [XmlAttribute(AttributeName = "XFaktur")]
    public string ExFaktur
    { get; set; }

    [XmlAttribute(AttributeName = "RC")]
    public string ReturID
    { get; set; }

    [XmlAttribute(AttributeName = "ReturDate")]
    public string TanggalRetur
    { get; set; }

    [XmlIgnore]
    public DateTime TanggalReturDate
    { get; set; }

    [XmlAttribute(AttributeName = "DO")]
    public string DeliveryID
    { get; set; }

    [XmlAttribute(AttributeName = "Tipe")]
    public string TypeItem
    { get; set; }

    [XmlAttribute(AttributeName = "Item")]
    public string Item
    { get; set; }

    [XmlAttribute(AttributeName = "IsCabang")]
    public bool IsCabang
    { get; set; }

    [XmlAttribute(AttributeName = "Qty")]
    public decimal Quantity
    { get; set; }

    [XmlAttribute(AttributeName = "Disc")]
    public decimal Discount
    { get; set; }

    [XmlAttribute(AttributeName = "Price")]
    public decimal Harga
    { get; set; }

    [XmlText]
    public string KeteranganMod
    { get; set; }
  }

  #endregion

  #region Beli Retur

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class FakturBeliReturStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static FakturBeliReturStructure Serialize(string rawData)
    {
      FakturBeliReturStructure fbs = StructureBase<FakturBeliReturStructure>.Serialize(rawData);

      if (fbs != null)
      {
        DateTime date = DateTime.Now;

        if (fbs.Fields != null)
        {
          if (!string.IsNullOrEmpty(fbs.Fields.TanggalFaktur))
          {
            if (Functionals.DateParser(fbs.Fields.TanggalFaktur, "yyyyMMddHHmmssfff", out date))
            {
              fbs.Fields.TanggalFakturDate = date;
            }
            else
            {
              fbs.Fields.TanggalFakturDate = Functionals.StandardSqlDateTime;
            }
          }
          if (!string.IsNullOrEmpty(fbs.Fields.TanggalTax))
          {
            if (Functionals.DateParser(fbs.Fields.TanggalTax, "yyyyMMddHHmmssfff", out date))
            {
              fbs.Fields.TanggalTaxDate = date;
            }
            else
            {
              fbs.Fields.TanggalTaxDate = Functionals.StandardSqlDateTime;
            }
          }
        }

        if ((fbs.ExtraFields != null) && (fbs.ExtraFields.ProcessField != null) && (fbs.ExtraFields.ProcessField.Length > 0))
        {
          for (int nLoop = 0, nLen = fbs.ExtraFields.ProcessField.Length; nLoop < nLen; nLoop++)
          {
            if (Functionals.DateParser(fbs.ExtraFields.ProcessField[nLoop].TanggalTax, "yyyyMMddHHmmssfff", out date))
            {
              fbs.ExtraFields.ProcessField[nLoop].TanggalTaxDate = date;
            }
            else
            {
              fbs.ExtraFields.ProcessField[nLoop].TanggalTaxDate = Functionals.StandardSqlDateTime;
            }
          }
        }
      }

      return fbs;
    }

    public static string Deserialize(FakturBeliReturStructure strt)
    {
      return StructureBase<FakturBeliReturStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public FakturBeliReturStructureFields Fields
    { get; set; }

    [XmlElement(ElementName = "ExtraFields")]
    public FakturBeliReturStructureExtraField ExtraFields
    { get; set; }
  }

  [Serializable]
  public class FakturBeliReturStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string FakturID
    { get; set; }

    [XmlAttribute(AttributeName = "FakturDate")]
    public string TanggalFaktur
    { get; set; }

    [XmlIgnore]
    public DateTime TanggalFakturDate
    { get; set; }

    [XmlAttribute(AttributeName = "Suplier")]
    public string Suplier
    { get; set; }

    [XmlAttribute(AttributeName = "XFaktur")]
    public string ExFaktur
    { get; set; }

    [XmlAttribute(AttributeName = "TaxNo")]
    public string NoTax
    { get; set; }

    [XmlAttribute(AttributeName = "TaxDate")]
    public string TanggalTax
    { get; set; }

    [XmlIgnore]
    public DateTime TanggalTaxDate
    { get; set; }

    [XmlAttribute(AttributeName = "Kurs")]
    public string Kurs
    { get; set; }

    [XmlAttribute(AttributeName = "KursValue")]
    public decimal KursValue
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public FakturBeliReturStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class FakturBeliReturStructureField
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

    [XmlAttribute(AttributeName = "GQty")]
    public decimal GoodQuantity
    { get; set; }

    [XmlAttribute(AttributeName = "BQty")]
    public decimal BadQuantity
    { get; set; }

    [XmlAttribute(AttributeName = "Disc")]
    public decimal Discount
    { get; set; }

    [XmlAttribute(AttributeName = "Price")]
    public decimal Harga
    { get; set; }

    [XmlText]
    public string KeteranganMod
    { get; set; }
  }

  [Serializable]
  public class FakturBeliReturStructureExtraField
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlElement(ElementName = "ProcessField")]
    public FakturBeliReturProcessStructureField[] ProcessField
    { get; set; }
  }

  [Serializable]
  public class FakturBeliReturProcessStructureField
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "New")]
    public bool IsNew
    { get; set; }

    [XmlAttribute(AttributeName = "RS")]
    public string ReturID
    { get; set; }

    [XmlAttribute(AttributeName = "XFaktur")]
    public string ExFaktur
    { get; set; }

    [XmlAttribute(AttributeName = "TaxNo")]
    public string NoTax
    { get; set; }

    [XmlAttribute(AttributeName = "TaxDate")]
    public string TanggalTax
    { get; set; }

    [XmlIgnore]
    public DateTime TanggalTaxDate
    { get; set; }

    [XmlAttribute(AttributeName = "Item")]
    public string Item
    { get; set; }

    [XmlAttribute(AttributeName = "GQty")]
    public decimal GoodQuantity
    { get; set; }

    [XmlAttribute(AttributeName = "BQty")]
    public decimal BadQuantity
    { get; set; }

    [XmlAttribute(AttributeName = "Disc")]
    public decimal Discount
    { get; set; }

    [XmlAttribute(AttributeName = "Price")]
    public decimal Harga
    { get; set; }

    [XmlText]
    public string KeteranganMod
    { get; set; }
  }

  #endregion

  #region Faktur Manual

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class FakturManualStructure
  {
      [XmlAttribute(AttributeName = "name")]
      public string Name
      { get; set; }

      [XmlAttribute(AttributeName = "method")]
      public string Method
      { get; set; }

      public static FakturManualStructure Serialize(string rawData)
      {
          //return StructureBase<FakturManualStructure>.Serialize(rawData);

          FakturManualStructure fm = StructureBase<FakturManualStructure>.Serialize(rawData);

          if (fm != null)
          {
              if (fm.Fields != null)
              {
                  DateTime date = DateTime.Now;

                  if (!string.IsNullOrEmpty(fm.Fields.taxdate))
                  {
                      if (Functionals.DateParser(fm.Fields.taxdate, "yyyyMMddHHmmssfff", out date))
                      {
                          fm.Fields.taxdatefaktur = date;
                      }
                      else
                      {
                          fm.Fields.taxdatefaktur = Functionals.StandardSqlDateTime;
                      }
                  }
              }
          }

          return fm;
      }

      public static string Deserialize(FakturManualStructure strt)
      {
          return StructureBase<FakturManualStructure>.Deserialize(strt);
      }

      [XmlElement(ElementName = "Fields")]
      public FakturManualStructureFields Fields
      { get; set; }
  }

  [Serializable]
  public class FakturManualStructureFields
  {
      [XmlAttribute(AttributeName = "Entry")]
      public string Entry
      { get; set; }

      [XmlAttribute(AttributeName = "nosup")]
      public string nosup
      { get; set; }

      [XmlAttribute(AttributeName = "taxno")]
      public string taxno
      { get; set; }

      [XmlAttribute(AttributeName = "taxdate")]
      public string taxdate
      { get; set; }

      [XmlIgnore]
      public DateTime taxdatefaktur
      { get; set; }

      [XmlAttribute(AttributeName = "dpp")]
      public decimal dpp
      { get; set; }

      [XmlAttribute(AttributeName = "ppn")]
      public decimal ppn
      { get; set; }

      [XmlAttribute(AttributeName = "total")]
      public decimal total
      { get; set; }

      [XmlAttribute(AttributeName = "ket")]
      public string ket
      { get; set; }

      [XmlAttribute(AttributeName = "referensi")]
      public string referensi
      { get; set; }

      //[XmlElement(ElementName = "Field")]
      //public FakturManualStructureField[] Field
      //{ get; set; }

      [XmlAttribute(AttributeName = "ID")]
      public string ID
      { get; set; }

      [XmlAttribute(AttributeName = "fmno")]
      public string fakturID
      { get; set; }

      [XmlAttribute(AttributeName = "KeteranganDel")]
      public string KeteranganDel
      { get; set; }
  }

  //[Serializable]
  //public class FakturManualStructureField
  //{
  //}
  #endregion
}
