using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Components;

namespace ScmsSoaLibrary.Parser.Class
{
  #region STD

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class OrderRequestStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static OrderRequestStructure Serialize(string rawData)
    {
      return StructureBase<OrderRequestStructure>.Serialize(rawData);
    }

    public static string Deserialize(OrderRequestStructure strt)
    {
      return StructureBase<OrderRequestStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public OrderRequestStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class OrderRequestStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string OrderRequestID
    { get; set; }

    [XmlAttribute(AttributeName = "Suplier")]
    public string Suplier
    { get; set; }

    [XmlAttribute(AttributeName = "Tipe")]
    public string TipeOR
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public OrderRequestStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class OrderRequestStructureField
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

    [XmlAttribute(AttributeName = "QtyOrd")]
    public decimal QuantityOrder
    { get; set; }

    [XmlText]
    public string Keterangan
    { get; set; }
  }

  #endregion

  #region Process
  
  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class OrderRequestProcessStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static OrderRequestProcessStructure Serialize(string rawData)
    {
      return StructureBase<OrderRequestProcessStructure>.Serialize(rawData);
    }

    public static string Deserialize(OrderRequestProcessStructure strt)
    {
      return StructureBase<OrderRequestProcessStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public OrderRequestProcessStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class OrderRequestProcessStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string OrderRequestID
    { get; set; }

    [XmlAttribute(AttributeName = "Suplier")]
    public string Suplier
    { get; set; }

    [XmlAttribute(AttributeName = "Tipe")]
    public string TipeOR
    { get; set; }

    [XmlAttribute(AttributeName = "Gudang")]
    public string Gudang
    { get; set; }

    [XmlAttribute(AttributeName = "GudangTo")]
    public string GudangTo
    { get; set; }

    [XmlAttribute(AttributeName = "Cabang")]
    public string Cabang
    { get; set; }

    [XmlAttribute(AttributeName = "DivPrincipal")]
    public string DivisiSuplier
    { get; set; }
    
    [XmlAttribute(AttributeName = "Via")]
    public string Via
    { get; set; }

    [XmlAttribute(AttributeName = "TipeItem")]
    public string TypeProduct
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public OrderRequestProcessStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class OrderRequestProcessStructureField
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

    [XmlAttribute(AttributeName = "NoID")]
    public string NomorID
    { get; set; }

    [XmlAttribute(AttributeName = "NoRef")]
    public string NomorReferensi
    { get; set; }

    [XmlAttribute(AttributeName = "Via")]
    public string Via
    { get; set; }

    [XmlAttribute(AttributeName = "Type")]
    public string TypeItem
    { get; set; }

    [XmlAttribute(AttributeName = "DivPri")]
    public string DivisiPrincipal
    { get; set; }

    [XmlAttribute(AttributeName = "Quantity")]
    public decimal Quantity
    { get; set; }

    [XmlAttribute(AttributeName = "MOQ")]
    public decimal MOQ
    { get; set; }

    [XmlAttribute(AttributeName = "AvgSales")]
    public decimal AvgSales
    { get; set; }

    [XmlAttribute(AttributeName = "Index")]
    public decimal Index
    { get; set; }

    [XmlAttribute(AttributeName = "SoH")]
    public decimal SoH
    { get; set; }

    [XmlAttribute(AttributeName = "SiT")]
    public decimal SiT
    { get; set; }

    [XmlAttribute(AttributeName = "BackOrder")]
    public decimal BackOrder
    { get; set; }

    [XmlAttribute(AttributeName = "Box")]
    public decimal Box
    { get; set; }

    [XmlAttribute(AttributeName = "HNA")]
    public decimal HNA
    { get; set; }

    [XmlAttribute(AttributeName = "MOP")]
    public decimal MOP
    { get; set; }

    [XmlAttribute(AttributeName = "QtyOR")]
    public decimal QtyOR
    { get; set; }

    [XmlAttribute(AttributeName = "SpAcc")]
    public decimal SpAcc
    { get; set; }

    [XmlAttribute(AttributeName = "Bonus")]
    public decimal Bonus
    { get; set; }

    [XmlAttribute(AttributeName = "AvgSalesDivPri")]
    public decimal AvgSalesDivPri
    { get; set; }

    [XmlAttribute(AttributeName = "Variable")]
    public decimal Variable
    { get; set; }

    [XmlAttribute(AttributeName = "Idxp")]
    public decimal Idxp
    { get; set; }

    [XmlAttribute(AttributeName = "Idxnp")]
    public decimal Idxnp
    { get; set; }

    [XmlAttribute(AttributeName = "Pareto")]
    public decimal Pareto
    { get; set; }

    [XmlAttribute(AttributeName = "Ideal")]
    public decimal Ideal
    { get; set; }

    [XmlAttribute(AttributeName = "Order")]
    public decimal Order
    { get; set; }

    [XmlAttribute(AttributeName = "Deviasi")]
    public decimal Deviasi
    { get; set; }

    [XmlAttribute(AttributeName = "QtySisa")]
    public decimal QtySisa
    { get; set; }

    [XmlAttribute(AttributeName = "Beli")]
    public decimal Beli
    { get; set; }

    [XmlAttribute(AttributeName = "Manual")]
    public bool Manual
    { get; set; }

    [XmlAttribute(AttributeName = "IsCombo")]
    public bool IsCombo
    { get; set; }

    [XmlAttribute(AttributeName = "ItemCombo")]
    public string ItemCombo
    { get; set; }
  }

  #endregion

  #region STD GUDANG

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class OrderRequestGudangStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static OrderRequestGudangStructure Serialize(string rawData)
    {
      //return StructureBase<OrderRequestGudangStructure>.Serialize(rawData);
        OrderRequestGudangStructure orgs = StructureBase<OrderRequestGudangStructure>.Serialize(rawData);

        DateTime date = DateTime.Now;
        if (orgs != null)
        {
            if ((orgs.Fields != null) && (!string.IsNullOrEmpty(orgs.Fields.TanggalSend)))
            {
                if (Functionals.DateParser(orgs.Fields.TanggalSend, "yyyyMMddHHmmssfff", out date))
                {
                    orgs.Fields.TanggalSendFormat = date;
                }
                else
                {
                    orgs.Fields.TanggalSendFormat = DateTime.MinValue;
                }
            }
        }
        return orgs;

    }

    public static string Deserialize(OrderRequestGudangStructure strt)
    {
      return StructureBase<OrderRequestGudangStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public OrderRequestGudangStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class OrderRequestGudangStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string OrderRequestID
    { get; set; }

    [XmlAttribute(AttributeName = "From")]
    public string GudangFrom
    { get; set; }

    [XmlAttribute(AttributeName = "To")]
    public string GudangTo
    { get; set; }

    [XmlAttribute(AttributeName = "Suplier")]
    public string Suplier
    { get; set; }

    [XmlAttribute(AttributeName = "Tipe")]
    public string TipeORG
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlAttribute(AttributeName = "Confirm")]
    public bool ConfirmedProcess
    { get; set; }

    [XmlAttribute(AttributeName = "TanggalSend")]
    public string TanggalSend
    { get; set; }

    public DateTime TanggalSendFormat
    { get; set; }


    [XmlElement(ElementName = "Field")]
    public OrderRequestGudangStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class OrderRequestGudangStructureField
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

    [XmlAttribute(AttributeName = "Reset")]
    public bool isReset
    { get; set; }

    [XmlAttribute(AttributeName = "Item")]
    public string Item
    { get; set; }

    [XmlAttribute(AttributeName = "Qty")]
    public decimal Quantity
    { get; set; }

    [XmlAttribute(AttributeName = "QtyOrd")]
    public decimal QuantityOrder
    { get; set; }

    [XmlText]
    public string Keterangan
    { get; set; }
  }

  #endregion
}
