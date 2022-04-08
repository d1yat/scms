using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Components;

namespace ScmsSoaLibrary.Parser.Class
{
  #region Ekspedisi

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class ExpedisiStructure
  {
    [XmlAttribute(AttributeName="nama")]
    public string Name
    { get; set; } 

    [XmlAttribute(AttributeName="method")]
    public string Method
    { get; set; }

    public static ExpedisiStructure Serialize(string rawData)
    {
      ExpedisiStructure  es = StructureBase<ExpedisiStructure>.Serialize(rawData);
      if (es != null)
      {
        if (!string.IsNullOrEmpty(es.Fields.DResi))
        {
          DateTime date = DateTime.MinValue;

          if (DateTime.TryParseExact(es.Fields.DResi, "yyyyMMddHHmmss", new System.Globalization.CultureInfo("en-US"), System.Globalization.DateTimeStyles.AssumeLocal,
            out date))
          {
            es.Fields.DateResi = date;
          }
        }
      }
      return es;
    }

    public static string Deserialize(ExpedisiStructure strt)
    {
      return StructureBase<ExpedisiStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public ExpedisiStructureFields Fields
    { get; set; }

  }

  [Serializable]
  public class ExpedisiStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string EkspedisiID
    { get; set; }

    [XmlAttribute(AttributeName = "Via")]
    public string Via
    { get; set; }

    [XmlAttribute(AttributeName = "Gudang")]
    public string Gudang
    { get; set; }

    [XmlAttribute(AttributeName = "Customer")]
    public string Customer
    { get; set; }

    [XmlAttribute(AttributeName = "By")]
    public string By
    { get; set; }

    [XmlAttribute(AttributeName = "Eks")]
    public string Expedisi
    { get; set; }

    [XmlAttribute(AttributeName = "EksPlus")]
    public string ExpedisiPlus
    { get; set; }

    [XmlAttribute(AttributeName = "Tipe")]
    public string TipeExpedisi
    { get; set; }

    [XmlAttribute(AttributeName = "Ket")]
    public string Ket
    { get; set; }

    [XmlAttribute(AttributeName = "Resi")]
    public string Resi
    { get; set; }

    [XmlAttribute(AttributeName = "Koli")]
    public decimal Koli
    { get; set; }

    [XmlAttribute(AttributeName = "Berat")]
    public decimal Berat
    { get; set; }

    [XmlAttribute(AttributeName = "Receh")]
    public decimal Receh
    { get; set; }

    [XmlAttribute(AttributeName = "Volume")]
    public decimal Volume
    { get; set; }

    [XmlAttribute(AttributeName = "DResi")]
    public string DResi
    { get; set; }

    [XmlAttribute(AttributeName = "Driver")]
    public string Driver
    { get; set; }

    [XmlAttribute(AttributeName = "Nopol")]
    public string Nopol
    { get; set; }
    //[XmlAttribute(AttributeName = "DResi")]
    //public string DResi
    //{ get; set; }

    [XmlAttribute(AttributeName = "Ref")]
    public string Ref
    { get; set; }

    [XmlAttribute(AttributeName = "TipeExp")]
    public string TipeExp
    { get; set; }

    [XmlAttribute(AttributeName = "BiayaLain")]
    public decimal BiayaLain
    { get; set; }

    [XmlAttribute(AttributeName = "TotalBiaya")]
    public decimal TotalBiaya
    { get; set; }

    [XmlAttribute(AttributeName = "BiayaKg")]
    public decimal BiayaKg
    { get; set; }

    [XmlAttribute(AttributeName = "expMin")]
    public decimal expMin
    { get; set; }

    [XmlAttribute(AttributeName = "isCabang")]
    public bool isCabang
    { get; set; }

    [XmlAttribute(AttributeName = "Customer2")]
    public string Customer2
    { get; set; }

    [XmlAttribute(AttributeName = "AsalKirim")]
    public string AsalKirim
    { get; set; }

    [XmlIgnore]
    public DateTime DateResi
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public ExpedisiStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class ExpedisiStructureField
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "New")]
    public bool IsNew
    { get; set; }

    [XmlAttribute(AttributeName = "Modified")]
    public bool IsModify
    { get; set; }

    [XmlAttribute(AttributeName = "Delete")]
    public bool IsDelete
    { get; set; }

    [XmlAttribute(AttributeName = "dono")]
    public string DO
    { get; set; }

    [XmlAttribute(AttributeName = "koli")]
    public decimal koli
    { get; set; }

    [XmlAttribute(AttributeName = "berat")]
    public decimal berat
    { get; set; }

    [XmlAttribute(AttributeName = "volume")]
    public decimal volume
    { get; set; }


    [XmlAttribute(AttributeName = "receh")]
    public decimal receh
    { get; set; }

    [XmlAttribute(AttributeName = "partno")]
    public string partno
    { get; set; }

    [XmlText]
    public string Keterangan
    { get; set; }
  }

  #endregion

  #region Ekspedisi Cabang

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class ExpedisiCabangStructure
  {
    [XmlAttribute(AttributeName = "nama")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static ExpedisiCabangStructure Serialize(string rawData)
    {
      //return StructureBase<ExpedisiCabangStructure>.Serialize(rawData);

      int nLoop = 0;

      ExpedisiCabangStructure esc = StructureBase<ExpedisiCabangStructure>.Serialize(rawData);
      if (esc != null)
      {
        if (!string.IsNullOrEmpty(esc.Fields.DateResi))
        {
          DateTime date = DateTime.MinValue;
          if (DateTime.TryParseExact(esc.Fields.DateResi, "yyyyMMddHHmmss", new System.Globalization.CultureInfo("en-US"), System.Globalization.DateTimeStyles.AssumeLocal,
            out date))
          {
            esc.Fields.DateResiXml = date;
          }
        }
        if (!string.IsNullOrEmpty(esc.Fields.Day))
        {
          DateTime date = DateTime.MinValue;
          if (DateTime.TryParseExact(esc.Fields.Day, "yyyyMMdd", new System.Globalization.CultureInfo("en-US"), System.Globalization.DateTimeStyles.AssumeLocal,
            out date))
          {
            esc.Fields.DayXml = date;
          }
        }
        if (!string.IsNullOrEmpty(esc.Fields.Time))
        {
          DateTime date = DateTime.MinValue;
          if (DateTime.TryParseExact(esc.Fields.Time, "HHmmss", new System.Globalization.CultureInfo("en-US"), System.Globalization.DateTimeStyles.AssumeLocal,
            out date))
          {
            esc.Fields.TimeXml = date;
          }
        }
        if (esc.ExtraFields != null)
        {
          if (esc.ExtraFields.ProcessField.Length > 0)
          {
            ExpedisiCabangProcessStructureField pField = null;
            for (nLoop = 0; nLoop < esc.ExtraFields.ProcessField.Length; nLoop++)
            {
              pField = esc.ExtraFields.ProcessField[nLoop];

              if (!string.IsNullOrEmpty(pField.DateResi))
              {
                DateTime date = DateTime.MinValue;
                if (DateTime.TryParseExact(pField.DateResi, "yyyyMMddHHmmss", new System.Globalization.CultureInfo("en-US"), System.Globalization.DateTimeStyles.AssumeLocal,
                  out date))
                {
                  pField.DateResiXml = date;
                }
              }
              if (!string.IsNullOrEmpty(pField.Day))
              {
                DateTime date = DateTime.MinValue;
                if (DateTime.TryParseExact(pField.Day, "yyyyMMdd", new System.Globalization.CultureInfo("en-US"), System.Globalization.DateTimeStyles.AssumeLocal,
                  out date))
                {
                  pField.DayXml = date;
                }
              }
              if (!string.IsNullOrEmpty(pField.Time))
              {
                DateTime date = DateTime.MinValue;
                if (DateTime.TryParseExact(pField.Time, "HHmmss", new System.Globalization.CultureInfo("en-US"), System.Globalization.DateTimeStyles.AssumeLocal,
                  out date))
                {
                  pField.TimeXml = date;
                }
              }
            }
          }
        }
      }
      return esc;
    }

    public static string Deserialize(ExpedisiCabangStructure strt)
    {
      return StructureBase<ExpedisiCabangStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public ExpedisiCabangStructureFields Fields
    { get; set; }

    [XmlElement(ElementName = "ExtraFields")]
    public ExpedisiCabangStructureExtraField ExtraFields
    { get; set; }

  }

  [Serializable]
  public class ExpedisiCabangStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string ID
    { get; set; }

    [XmlAttribute(AttributeName = "Day")]
    public string Day
    { get; set; }

    [XmlAttribute(AttributeName = "Time")]
    public string Time
    { get; set; }

    [XmlAttribute(AttributeName = "ExpId")]
    public string ExpId
    { get; set; }

    [XmlAttribute(AttributeName = "DateResi")]
    public string DateResi
    { get; set; }

    [XmlIgnore]
    public DateTime DateResiXml
    { get; set; }

    [XmlIgnore]
    public DateTime DayXml
    { get; set; }

    [XmlIgnore]
    public DateTime TimeXml
    { get; set; }
  }

  [Serializable]
  public class ExpedisiCabangStructureExtraField
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlElement(ElementName = "ProcessField")]
    public ExpedisiCabangProcessStructureField[] ProcessField
    { get; set; }
  }

  [Serializable]
  public class ExpedisiCabangProcessStructureField
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "New")]
    public bool IsNew
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string ID
    { get; set; }

    [XmlAttribute(AttributeName = "Day")]
    public string Day
    { get; set; }

    [XmlAttribute(AttributeName = "Time")]
    public string Time
    { get; set; }

    [XmlAttribute(AttributeName = "ExpId")]
    public string ExpId
    { get; set; }

    [XmlAttribute(AttributeName = "DateResi")]
    public string DateResi
    { get; set; }

    [XmlAttribute(AttributeName = "customer")]
    public string customer
    { get; set; }

    [XmlAttribute(AttributeName = "expedisi")]
    public string expedisi
    { get; set; }

    [XmlIgnore]
    public DateTime DateResiXml
    { get; set; }

    [XmlIgnore]
    public DateTime DayXml
    { get; set; }

    [XmlIgnore]
    public DateTime TimeXml
    { get; set; }
  }

  #endregion

  #region Invoice Ekspedisi Eksternal

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class InvoiceEkspedisiEksternalStructure
  {
      [XmlAttribute(AttributeName = "name")]
      public string Name
      { get; set; }

      [XmlAttribute(AttributeName = "method")]
      public string Method
      { get; set; }

      //public static InvoiceEkspedisiStructure Serialize(string rawData)
      //{
      //    return StructureBase<InvoiceEkspedisiStructure>.Serialize(rawData);
      //}

      public static InvoiceEkspedisiEksternalStructure Serialize(string rawData)
      {
          InvoiceEkspedisiEksternalStructure fes = StructureBase<InvoiceEkspedisiEksternalStructure>.Serialize(rawData);

          if (fes != null)
          {
              if (fes.Fields != null)
              {
                  DateTime date = DateTime.Now;

                  if (!string.IsNullOrEmpty(fes.Fields.TanggalFaktur))
                  {
                      if (Functionals.DateParser(fes.Fields.TanggalFaktur, "yyyyMMddHHmmssfff", out date))
                      {
                          fes.Fields.TanggalFakturDate = date;
                      }
                      else
                      {
                          fes.Fields.TanggalFakturDate = Functionals.StandardSqlDateTime;
                      }
                  }
              }
          }

          return fes;
      }

      public static string Deserialize(InvoiceEkspedisiEksternalStructure strt)
      {
          return StructureBase<InvoiceEkspedisiEksternalStructure>.Deserialize(strt);
      }

      [XmlElement(ElementName = "Fields")]
      public InvoiceEkspedisiEksternalStructureFields Fields
      { get; set; }

      [XmlElement(ElementName = "ExtraFields")]
      public InvoiceEkspedisiEksternalStructureExtraField ExtraFields
      { get; set; }
  }

  [Serializable]
  public class InvoiceEkspedisiEksternalStructureFields
  {
      [XmlAttribute(AttributeName = "Entry")]
      public string Entry
      { get; set; }

      [XmlAttribute(AttributeName = "ID")]
      public string FakturID
      { get; set; }

      [XmlAttribute(AttributeName = "Gudang")]
      public string Gudang
      { get; set; }

      [XmlAttribute(AttributeName = "Ekspedisi")]
      public string Ekspedisi
      { get; set; }

      [XmlAttribute(AttributeName = "Faktur")]
      public string Faktur
      { get; set; }

      [XmlAttribute(AttributeName = "FisikFaktur")]
      [System.ComponentModel.DefaultValue(0)]
      public decimal FisikFaktur
      { get; set; }

      [XmlAttribute(AttributeName = "FakturDate")]
      public string TanggalFaktur
      { get; set; }

      [XmlIgnore]
      public DateTime TanggalFakturDate
      { get; set; }

      [XmlAttribute(AttributeName = "TOP")]
      public decimal TOP
      { get; set; }

      [XmlAttribute(AttributeName = "Pajak")]
      public decimal Pajak
      { get; set; }

      [XmlAttribute(AttributeName = "Ket")]
      public string Ket
      { get; set; }

      [XmlAttribute(AttributeName = "KM")]
      public decimal KM
      { get; set; }

      [XmlAttribute(AttributeName = "Materai")]
      public decimal Materai
      { get; set; }

      [XmlAttribute(AttributeName = "totalPotongan")]
      public decimal totalPotongan
      { get; set; }

      [XmlAttribute(AttributeName = "ClaimNo")]
      public string ClaimNo
      { get; set; }

      [XmlAttribute(AttributeName = "Gross")]
      public decimal Gross
      { get; set; }

      [XmlAttribute(AttributeName = "TotalTax")]
      public decimal TotalTax
      { get; set; }

      [XmlAttribute(AttributeName = "TotalBiayaLain")]
      public decimal TotalBiayaLain
      { get; set; }

      [XmlAttribute(AttributeName = "NetBerat")]
      public decimal NetBerat
      { get; set; }

      [XmlAttribute(AttributeName = "NetVol")]
      public decimal NetVol
      { get; set; }

      [XmlElement(ElementName = "Field")]
      public InvoiceEkspedisiEksternalStructureField[] Field
      { get; set; }


  }

  [Serializable]
  public class InvoiceEkspedisiEksternalStructureField
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

      [XmlAttribute(AttributeName = "resiNo")]
      public string resiNo
      { get; set; }

      [XmlAttribute(AttributeName = "epNo")]
      public string epNo
      { get; set; }

      [XmlAttribute(AttributeName = "Cusno")]
      public string Cusno
      { get; set; }

      [XmlAttribute(AttributeName = "Koli")]
      public decimal Koli
      { get; set; }

      [XmlAttribute(AttributeName = "Berat")]
      public decimal Berat
      { get; set; }

      [XmlAttribute(AttributeName = "Volume")]
      public decimal Volume
      { get; set; }

      [XmlAttribute(AttributeName = "Tonase")]
      public decimal Tonase
      { get; set; }

      [XmlAttribute(AttributeName = "Biaya")]
      public decimal Biaya
      { get; set; }

      [XmlAttribute(AttributeName = "Via")]
      public string Via
      { get; set; }

      [XmlAttribute(AttributeName = "expType")]
      public string expType
      { get; set; }

      [XmlAttribute(AttributeName = "tipeBiaya")]
      public string tipeBiaya
      { get; set; }

      [XmlAttribute(AttributeName = "biayaLain")]
      public decimal biayaLain
      { get; set; }

      [XmlAttribute(AttributeName = "totalCost")]
      public decimal totalCost
      { get; set; }

      [XmlText]
      public string KeteranganMod
      { get; set; }

      [XmlAttribute(AttributeName = "expMin")]
      public decimal expMin
      { get; set; }

      [XmlAttribute(AttributeName = "urut")]
      public int urut
      { get; set; }
  }

      [Serializable]
      public class InvoiceEkspedisiEksternalStructureExtraField
      {
          [XmlAttribute(AttributeName = "name")]
          public string Name
          { get; set; }

          [XmlElement(ElementName = "FieldClaim")]
          public InvoiceEkspedisiEksternalClaimStructureField[] FieldClaim
          { get; set; }
      }

      
  [Serializable]
  public class InvoiceEkspedisiEksternalClaimStructureField
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

    [XmlAttribute(AttributeName = "claimNo")]
    public string claimNo
    { get; set; }

    [XmlAttribute(AttributeName = "Potongan")]
    public decimal Potongan
    { get; set; }

    [XmlText]
    public string KeteranganMod
    { get; set; }
  }
  #endregion

  #region Invoice Ekspedisi Internal

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class InvoiceEkspedisiInternalStructure
  {
      [XmlAttribute(AttributeName = "name")]
      public string Name
      { get; set; }

      [XmlAttribute(AttributeName = "method")]
      public string Method
      { get; set; }

      //public static InvoiceEkspedisiInternalStructure Serialize(string rawData)
      //{
      //    return StructureBase<InvoiceEkspedisiInternalStructure>.Serialize(rawData);
      //}

      public static InvoiceEkspedisiInternalStructure Serialize(string rawData)
      {
          InvoiceEkspedisiInternalStructure fes = StructureBase<InvoiceEkspedisiInternalStructure>.Serialize(rawData);

          if (fes != null)
          {
              if (fes.Fields != null)
              {
                  DateTime date = DateTime.Now;

                  if (!string.IsNullOrEmpty(fes.Fields.TanggalFaktur))
                  {
                      if (Functionals.DateParser(fes.Fields.TanggalFaktur, "yyyyMMddHHmmssfff", out date))
                      {
                          fes.Fields.TanggalFakturDate = date;
                      }
                      else
                      {
                          fes.Fields.TanggalFakturDate = Functionals.StandardSqlDateTime;
                      }
                  }
              }
          }

          return fes;
      }

      public static string Deserialize(InvoiceEkspedisiInternalStructure strt)
      {
          return StructureBase<InvoiceEkspedisiInternalStructure>.Deserialize(strt);
      }

      [XmlElement(ElementName = "Fields")]
      public InvoiceEkspedisiInternalStructureFields Fields
      { get; set; }

      [XmlElement(ElementName = "ExtraFields")]
      public InvoiceEkspedisiInternalStructureExtraField ExtraFields
      { get; set; }
  }

  [Serializable]
  public class InvoiceEkspedisiInternalStructureFields
  {
      [XmlAttribute(AttributeName = "Entry")]
      public string Entry
      { get; set; }

      [XmlAttribute(AttributeName = "ID")]
      public string FakturID
      { get; set; }

      [XmlAttribute(AttributeName = "Gudang")]
      public string Gudang
      { get; set; }

      //[XmlAttribute(AttributeName = "Ekspedisi")]
      //public string Ekspedisi
      //{ get; set; }

      [XmlAttribute(AttributeName = "Faktur")]
      public string Faktur
      { get; set; }

      [XmlAttribute(AttributeName = "FisikFaktur")]
      [System.ComponentModel.DefaultValue(0)]
      public decimal FisikFaktur
      { get; set; }

      [XmlAttribute(AttributeName = "FakturDate")]
      public string TanggalFaktur
      { get; set; }

      [XmlIgnore]
      public DateTime TanggalFakturDate
      { get; set; }

      [XmlAttribute(AttributeName = "Ket")]
      public string Ket
      { get; set; }

      [XmlAttribute(AttributeName = "BiayaTol")]
      public decimal BiayaTol
      { get; set; }

      [XmlAttribute(AttributeName = "BBMLiter")]
      public decimal BBMLiter
      { get; set; }

      [XmlAttribute(AttributeName = "BBMType")]
      public bool BBMType
      { get; set; }

      [XmlAttribute(AttributeName = "BiayaBBM")]
      public decimal BiayaBBM
      { get; set; }

      [XmlAttribute(AttributeName = "awalKM")]
      public decimal awalKM
      { get; set; }

      [XmlAttribute(AttributeName = "akhirKM")]
      public decimal akhirKM
      { get; set; }

      [XmlAttribute(AttributeName = "Materai")]
      public decimal Materai
      { get; set; }

      [XmlAttribute(AttributeName = "TotalBiayaLain")]
      public decimal TotalBiayaLain
      { get; set; }

      [XmlAttribute(AttributeName = "Net")]
      public decimal Net
      { get; set; }

      [XmlElement(ElementName = "Field")]
      public InvoiceEkspedisiInternalStructureField[] Field
      { get; set; }

      //Indra 20170426
      [XmlAttribute(AttributeName = "TipeBBM")]
      public string TipeBBM
      { get; set; }
  }

  [Serializable]
  public class InvoiceEkspedisiInternalStructureField
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

      [XmlAttribute(AttributeName = "resiNo")]
      public string resiNo
      { get; set; }

      [XmlAttribute(AttributeName = "epNo")]
      public string epNo
      { get; set; }

      [XmlAttribute(AttributeName = "Cusno")]
      public string Cusno
      { get; set; }

      [XmlAttribute(AttributeName = "Koli")]
      public decimal Koli
      { get; set; }

      [XmlAttribute(AttributeName = "Berat")]
      public decimal Berat
      { get; set; }

      [XmlAttribute(AttributeName = "Volume")]
      public decimal Volume
      { get; set; }

      [XmlAttribute(AttributeName = "Via")]
      public string Via
      { get; set; }

      [XmlAttribute(AttributeName = "biayaLain")]
      public decimal biayaLain
      { get; set; }

      [XmlText]
      public string KeteranganMod
      { get; set; }

      [XmlAttribute(AttributeName = "urut")]
      public int urut
      { get; set; }
  }

  [Serializable]
  public class InvoiceEkspedisiInternalStructureExtraField
  {
      [XmlAttribute(AttributeName = "name")]
      public string Name
      { get; set; }

      [XmlElement(ElementName = "FieldTol")]
      public InvoiceEkspedisiInternalTolStructureField[] FieldTol
      { get; set; }
  }


  [Serializable]
  public class InvoiceEkspedisiInternalTolStructureField
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

      [XmlAttribute(AttributeName = "idx")]
      public string idx
      { get; set; }

      [XmlAttribute(AttributeName = "detailtol")]
      public decimal detailtol
      { get; set; }

      [XmlText]
      public string KeteranganMod
      { get; set; }
  }
  #endregion

  #region Faktur Ekspedisi

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class FakturEkspedisiStructure
  {
      [XmlAttribute(AttributeName = "name")]
      public string Name
      { get; set; }

      [XmlAttribute(AttributeName = "method")]
      public string Method
      { get; set; }

      public static FakturEkspedisiStructure Serialize(string rawData)
      {
          FakturEkspedisiStructure fes = StructureBase<FakturEkspedisiStructure>.Serialize(rawData);

          if (fes != null)
          {
              if (fes.Fields != null)
              {
                  DateTime date = DateTime.Now;

                  if (!string.IsNullOrEmpty(fes.Fields.TanggalFaktur))
                  {
                      if (Functionals.DateParser(fes.Fields.TanggalFaktur, "yyyyMMddHHmmssfff", out date))
                      {
                          fes.Fields.TanggalFakturDate = date;
                      }
                      else
                      {
                          fes.Fields.TanggalFakturDate = Functionals.StandardSqlDateTime;
                      }
                  }
              }
          }

          return fes;
      }

      public static string Deserialize(FakturEkspedisiStructure strt)
      {
          return StructureBase<FakturEkspedisiStructure>.Deserialize(strt);
      }

      [XmlElement(ElementName = "Fields")]
      public FakturEkspedisiStructureFields Fields
      { get; set; }
  }

  [Serializable]
  public class FakturEkspedisiStructureFields
  {
      [XmlAttribute(AttributeName = "Entry")]
      public string Entry
      { get; set; }

      [XmlAttribute(AttributeName = "ID")]
      public string FakturID
      { get; set; }

      [XmlAttribute(AttributeName = "Gudang")]
      public string Gudang
      { get; set; }

      [XmlAttribute(AttributeName = "Ekspedisi")]
      public string Ekspedisi
      { get; set; }

      [XmlAttribute(AttributeName = "Faktur")]
      public string Faktur
      { get; set; }

      [XmlAttribute(AttributeName = "FisikFaktur")]
      [System.ComponentModel.DefaultValue(0)]
      public decimal FisikFaktur
      { get; set; }

      [XmlAttribute(AttributeName = "FakturDate")]
      public string TanggalFaktur
      { get; set; }

      [XmlIgnore]
      public DateTime TanggalFakturDate
      { get; set; }

      [XmlAttribute(AttributeName = "Ket")]
      public string Ket
      { get; set; }

      [XmlAttribute(AttributeName = "BilvaFaktur")]
      public decimal BilvaFaktur
      { get; set; }

      [XmlAttribute(AttributeName = "Net")]
      public decimal Net
      { get; set; }

      [XmlAttribute(AttributeName = "Selisih")]
      public decimal Selisih
      { get; set; }

      [XmlAttribute(AttributeName = "Claim")]
      public decimal Claim
      { get; set; }

      [XmlAttribute(AttributeName = "Pinalty")]
      public decimal Pinalty
      { get; set; }

      [XmlAttribute(AttributeName = "Lain2")]
      public decimal Lain2
      { get; set; }

      [XmlAttribute(AttributeName = "Alasan")]
      public string Alasan
      { get; set; }

      [XmlAttribute(AttributeName = "TotalNet")]
      public decimal TotalNet
      { get; set; }

      [XmlAttribute(AttributeName = "Pph")]
      public decimal Pph
      { get; set; }

      [XmlElement(ElementName = "Field")]
      public FakturEkspedisiStructureField[] Field
      { get; set; }
  }

  [Serializable]
  public class FakturEkspedisiStructureField
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

      [XmlAttribute(AttributeName = "InvNo")]
      public string InvNo
      { get; set; }

      [XmlAttribute(AttributeName = "Resi")]
      public string Resi
      { get; set; }

      [XmlAttribute(AttributeName = "BilvaD")]
      public decimal BilvaD
      { get; set; }

      [XmlAttribute(AttributeName = "NetD")]
      public decimal NetD
      { get; set; }

      [XmlAttribute(AttributeName = "SelisihD")]
      public decimal SelisihD
      { get; set; }

      [XmlText]
      public string KeteranganMod
      { get; set; }
  }
  #endregion

  #region Return DO

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class ReturnDOStructure
  {
      [XmlAttribute(AttributeName = "name")]
      public string Name
      { get; set; }

      [XmlAttribute(AttributeName = "method")]
      public string Method
      { get; set; }

      public static ReturnDOStructure Serialize(string rawData)
      {
         ReturnDOStructure meb = StructureBase<ReturnDOStructure>.Serialize(rawData);

          DateTime date = DateTime.Now;

          if (meb != null)
          {
              if ((meb.Fields != null) && (meb.Fields.Field != null) && (meb.Fields.Field.Length > 0))
              {
                  ReturnDOStructureField field = null;

                  for (int nLoop = 0; nLoop < meb.Fields.Field.Length; nLoop++)
                  {
                      field = meb.Fields.Field[nLoop];

                      if (field != null)
                      {
                          if (!string.IsNullOrEmpty(field.tglBalik))
                          {
                              if (Functionals.DateParser(field.tglBalik, "yyyyMMdd", out date))
                              {
                                  field.tglBalikFormated = date;
                              }
                              else
                              {
                                  field.tglBalikFormated = DateTime.MinValue;
                              }
                          }
                          
                          if (!string.IsNullOrEmpty(field.tglTerima))
                          {
                              if (Functionals.DateParser(field.tglTerima, "yyyyMMdd", out date))
                              {
                                  field.tglTerimaFormated = date;
                              }
                              else
                              {
                                  field.tglTerimaFormated = DateTime.MinValue;
                              }
                          }
                      }
                  }
              }
          }

          return meb;
      }

      public static string Deserialize(ReturnDOStructure strt)
      {
          return StructureBase<ReturnDOStructure>.Deserialize(strt);
      }

      [XmlElement(ElementName = "Fields")]
      public ReturnDOStructureFields Fields
      { get; set; }
  }

  [Serializable]
  public class ReturnDOStructureFields
  {
      [XmlAttribute(AttributeName = "Entry")]
      public string Entry
      { get; set; }

      [XmlAttribute(AttributeName = "ID")]
      public string ID
      { get; set; }

      [XmlAttribute(AttributeName = "Keterangan")]
      public string Keterangan
      { get; set; }

      [XmlAttribute(AttributeName = "Gudang")]
      public string Gudang
      { get; set; }

      [XmlElement(ElementName = "Field")]
      public ReturnDOStructureField[] Field
      { get; set; }
  }

  [Serializable]
  public class ReturnDOStructureField
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

      [XmlAttribute(AttributeName = "dono")]
      public string dono
      { get; set; }

      [XmlAttribute(AttributeName = "tglTerima")]
      public string tglTerima
      { get; set; }

      [XmlIgnore]
      public DateTime tglTerimaFormated
      { get; set; }

      [XmlAttribute(AttributeName = "tglBalik")]
      public string tglBalik
      { get; set; }

      [XmlIgnore]
      public DateTime tglBalikFormated
      { get; set; }

      [XmlText]
      public string KeteranganMod
      { get; set; }
  }

  #endregion
}
