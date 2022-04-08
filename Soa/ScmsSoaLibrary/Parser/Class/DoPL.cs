using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Components;

namespace ScmsSoaLibrary.Parser.Class
{
  #region DO PL Transaksi

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class DoPLStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static DoPLStructure Serialize(string rawData)
    {
      return StructureBase<DoPLStructure>.Serialize(rawData); ;
    }

    public static string Deserialize(DoPLStructure strt)
    {
      return StructureBase<DoPLStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public DoPLStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class DoPLStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string DOid
    { get; set; }

    [XmlAttribute(AttributeName = "Via")]
    public string Via
    { get; set; }

    [XmlAttribute(AttributeName = "ConfirmSent")]
    public bool ConfirmedSent
    { get; set; }

    [XmlAttribute(AttributeName = "Customer")]
    public string Customer
    { get; set; }

    [XmlAttribute(AttributeName = "Gudang")]
    public string Gudang
    { get; set; }

    [XmlAttribute(AttributeName = "Plno")]
    public string nopl
    { get; set; }

    [XmlAttribute(AttributeName = "Type")]
    public string TypePackingList
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public DoPLStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class DoPLStructureField
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

    [XmlAttribute(AttributeName = "Item")]
    public string Item
    { get; set; }

    [XmlAttribute(AttributeName = "Qty")]
    public decimal Quantity
    { get; set; }

    [XmlText]
    public string KeteranganMod
    { get; set; }
  }

  #endregion
  
  #region DO STT Transaksi

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class DOSTTStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static DOSTTStructure Serialize(string rawData)
    {
      return StructureBase<DOSTTStructure>.Serialize(rawData); ;
    }

    public static string Deserialize(DOSTTStructure strt)
    {
      return StructureBase<DOSTTStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public DOSTTStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class DOSTTStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string DOid
    { get; set; }

    [XmlAttribute(AttributeName = "Via")]
    public string Via
    { get; set; }

    [XmlAttribute(AttributeName = "ConfirmSent")]
    public bool ConfirmedSent
    { get; set; }

    [XmlAttribute(AttributeName = "Customer")]
    public string Customer
    { get; set; }

    [XmlAttribute(AttributeName = "Gudang")]
    public string Gudang
    { get; set; }

    [XmlAttribute(AttributeName = "sttNO")]
    public string nopl
    { get; set; }

    [XmlAttribute(AttributeName = "Type")]
    public string TypePackingList
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public DOSTTStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class DOSTTStructureField
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

    [XmlAttribute(AttributeName = "Item")]
    public string Item
    { get; set; }

    [XmlAttribute(AttributeName = "Qty")]
    public decimal Quantity
    { get; set; }

    [XmlText]
    public string KeteranganMod
    { get; set; }
  }

  #endregion

  #region DO To DistCore
  
  [Serializable]
  public class DeliveryOrderPosting
  {
    public string PIN { get; set; }

    public string DO { get; set; }

    public string Cabang { get; set; }

    public DateTime TanggalDO { get; set; }

    public string TanggalDO_Str { get; set; }

    public string TypeCode { get; set; }

    public string FakturID { get; set; }

    public string ReferenceID { get; set; }

    public DateTime TanggalFJ { get; set; }

    public string TanggalFJ_Str { get; set; }

    public char Gudang { get; set; }

    public string GudangDesc { get; set; }

    public string Via { get; set; }

    public string user { get; set; }

    public string PoOutlet { get; set; }

    public string Outlet { get; set; }

    public string PLPHAR { get; set; }

    public string SPPs { get; set; }//suwandi 21 juni 2017

    public DeliveryOrderDetailPostings[] Fields { get; set; }

    public static DeliveryOrderPosting Deserialize(string rawData)
    {
      return StructureBase<DeliveryOrderPosting>.SerializeJson(rawData);
    }

    public static string Serialize(DeliveryOrderPosting strt)
    {
      return StructureBase<DeliveryOrderPosting>.DeserializeJson(strt);
    }
  }
  
  [Serializable]
  public class DeliveryOrderDetailPostings
  {
    public string Item { get; set; }

    public string NamaItem { get; set; }

    public string Batch { get; set; }

    public DateTime Expired { get; set; }

    public string Expired_Str { get; set; }

    public decimal Jumlah { get; set; }

    public decimal Harga { get; set; }

    public decimal Diskon { get; set; }

    public string SPHs { get; set; }

    public DeliveryOrderSPDetailPostings[] SPs { get; set; }

  }

  [Serializable]
  public class DeliveryOrderSPDetailPostings
  {
    public string SP { get; set; }

    public decimal Jumlah { get; set; }
  }

  #endregion

  #region DO PL SEND

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class DoPLSendStructure
  {
      [XmlAttribute(AttributeName = "name")]
      public string Name
      { get; set; }

      [XmlAttribute(AttributeName = "method")]
      public string Method
      { get; set; }

      public static DoPLSendStructure Serialize(string rawData)
      {
          return StructureBase<DoPLSendStructure>.Serialize(rawData); ;
      }

      public static string Deserialize(DoPLSendStructure strt)
      {
          return StructureBase<DoPLSendStructure>.Deserialize(strt);
      }

      [XmlElement(ElementName = "Fields")]
      public DoPLSendStructureFields Fields
      { get; set; }
  }

  [Serializable]
  public class DoPLSendStructureFields
  {
      [XmlAttribute(AttributeName = "Entry")]
      public string Entry
      { get; set; }

      [XmlElement(ElementName = "Field")]
      public DoPLSendStructureField[] Field
      { get; set; }
  }

  [Serializable]
  public class DoPLSendStructureField
  {
      [XmlAttribute(AttributeName = "ID")]
      public string DOid
      { get; set; }

      [XmlAttribute(AttributeName = "gudang")]
      public string Gudang
      { get; set; }

      [XmlAttribute(AttributeName = "send")]
      public bool Send
      { get; set; }

      [XmlText]
      public string KeteranganMod
      { get; set; }
  }

  #endregion
}
