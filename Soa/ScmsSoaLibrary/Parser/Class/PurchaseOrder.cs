using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Components;

namespace ScmsSoaLibrary.Parser.Class
{

    #region Purchase Order
    [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class PurchaseOrderStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static PurchaseOrderStructure Serialize(string rawData)
    {
      return StructureBase<PurchaseOrderStructure>.Serialize(rawData);
    }

    public static string Deserialize(PurchaseOrderStructure strt)
    {
      return StructureBase<PurchaseOrderStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public PurchaseOrderStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class PurchaseOrderStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "OrderID")]
    public string OrderRequestID
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string PurchaseOrderID
    { get; set; }

    [XmlAttribute(AttributeName = "Gdg")]
    public string Gudang
    { get; set; }

    [XmlAttribute(AttributeName = "Kurs")]
    public string Kurs
    { get; set; }

    [XmlAttribute(AttributeName = "KursValue")]
    public decimal KursValue
    { get; set; }

    [XmlAttribute(AttributeName = "XDisc")]
    public decimal ExtraDiscount
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlAttribute(AttributeName = "HasSend")]
    public bool HasSend
    { get; set; }

    [XmlAttribute(AttributeName = "typeApoteker")]
    public string typeApoteker
    { get; set; }

    [XmlAttribute(AttributeName = "poktno")]
    public string poktno
    { get; set; }

    [XmlAttribute(AttributeName = "isConfirm")]
    public bool isConfirm
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public PurchaseOrderStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class PurchaseOrderStructureField
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

    [XmlAttribute(AttributeName = "Disc")]
    public decimal Discount
    { get; set; }

    [XmlAttribute(AttributeName = "Salpri")]
    public decimal Price
    { get; set; }

    [XmlText]
    public string Keterangan
    { get; set; }
  }

#endregion Purchase Order

    #region PO Limit
  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class LimitPOItemStructure
  {
      [XmlAttribute(AttributeName = "name")]
      public string Name
      { get; set; }

      [XmlAttribute(AttributeName = "method")]
      public string Method
      { get; set; }

      public static LimitPOItemStructure Serialize(string rawData)
      {
          return StructureBase<LimitPOItemStructure>.Serialize(rawData);
      }

      public static string Deserialize(LimitPOItemStructure strt)
      {
          return StructureBase<LimitPOItemStructure>.Deserialize(strt);
      }

      [XmlElement(ElementName = "Fields")]
      public LimitPOItemStructureFields Fields
      { get; set; }
  }

  [Serializable]
  public class LimitPOItemStructureFields
  {
      [XmlAttribute(AttributeName = "Entry")]
      public string Entry
      { get; set; }

      [XmlAttribute(AttributeName = "ID")]
      public string ID
      { get; set; }

      [XmlAttribute(AttributeName = "kddivprihdr")]
      public string kddivprihdr
      { get; set; }

      [XmlAttribute(AttributeName = "nosup")]
      public string nosup
      { get; set; }

      [XmlAttribute(AttributeName = "nTahun")]
      public short nTahun
      { get; set; }

      [XmlAttribute(AttributeName = "nBulan")]
      public short nBulan
      { get; set; }

      [XmlAttribute(AttributeName = "limit")]
      public string limit
      { get; set; }

      [XmlAttribute(AttributeName = "limitalokasi")]
      public string limitalokasi
      { get; set; }

      [XmlAttribute(AttributeName = "percentage")]
      public string percentage
      { get; set; }

      [XmlAttribute(AttributeName = "limitused")]
      public string limitused
      { get; set; }

      [XmlElement(ElementName = "Field")]
      public LimitPOItemStructureField[] Field
      { get; set; }
  }

  [Serializable]
  public class LimitPOItemStructureField
  {
      [XmlAttribute(AttributeName = "Modified")]
      public bool IsModified
      { get; set; }

      [XmlAttribute(AttributeName = "nosup")]
      public string nosup
      { get; set; }

      [XmlAttribute(AttributeName = "nTahun")]
      public short nTahun
      { get; set; }

      [XmlAttribute(AttributeName = "nBulan")]
      public short nBulan
      { get; set; }

      [XmlAttribute(AttributeName = "Item")]
      public string Item
      { get; set; }

      [XmlAttribute(AttributeName = "kddivpridtl")]
      public string kddivpridtl
      { get; set; }

      [XmlAttribute(AttributeName = "Budget")]
      public decimal Budget
      { get; set; }

      [XmlAttribute(AttributeName = "nextlimit")]
      public decimal nextlimit
      { get; set; }

      [XmlAttribute(AttributeName = "Balance")]
      public decimal Balance
      { get; set; }

      [XmlAttribute(AttributeName = "availablebudget")]
      public decimal availablebudget
      { get; set; }

      [XmlAttribute(AttributeName = "nBestSls")]
      public decimal nBestSls
      { get; set; }

      [XmlAttribute(AttributeName = "nPercentAdj")]
      public decimal nPercentAdj
      { get; set; }

      [XmlAttribute(AttributeName = "nQty")]
      public decimal nQty
      { get; set; }

  }
    #endregion PO Limit
}
