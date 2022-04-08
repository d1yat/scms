using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using ScmsSoaLibrary.Commons;
using System.ComponentModel;
using ScmsSoaLibraryInterface.Components;

namespace ScmsSoaLibrary.Parser.Class
{
  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class ReturCustomerStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static ReturCustomerStructure Serialize(string rawData)
    {
      ReturCustomerStructure rcs = StructureBase<ReturCustomerStructure>.Serialize(rawData);

      if ((rcs.Fields != null) && (rcs.Fields.Field != null) && (rcs.Fields.Field.Length > 0))
      {
        ReturCustomerStructureField field = null;

        for (int nLoop = 0; nLoop < rcs.Fields.Field.Length; nLoop++)
        {
          field = rcs.Fields.Field[nLoop];
          
          if (field != null)
          {
            field.Batch = (string.IsNullOrEmpty(field.Batch) ? string.Empty : field.Batch.Trim());
          }
        }
      }

      return rcs;
    }

    public static string Deserialize(ReturCustomerStructure strt)
    {
      return StructureBase<ReturCustomerStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public ReturCustomerStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class ReturCustomerStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string ReturCustomerID
    { get; set; }
    
    [XmlAttribute(AttributeName = "PB")]
    public string PBID
    { get; set; }

    [XmlAttribute(AttributeName = "PBOLD")]
    public string PBIDOLD
    { get; set; }

    [XmlAttribute(AttributeName = "Gudang")]
    public string Gudang
    { get; set; }

    [XmlAttribute(AttributeName = "Customer")]
    public string Customer
    { get; set; }

    [XmlAttribute(AttributeName = "Supplier")]
    public string Supplier
    { get; set; }

    [XmlAttribute(AttributeName = "USER")]
    public string USER
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public ReturCustomerStructureField[] Field
    { get; set; }
      
  }

  [Serializable]
  public class ReturCustomerStructureField
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

    [XmlAttribute(AttributeName = "Batch")]
    public string Batch
    { get; set; }

    [XmlAttribute(AttributeName = "NoDO")]
    public string NoDO
    { get; set; }

    [XmlAttribute(AttributeName = "TypeItem")]
    public string Type
    { get; set; }

    [XmlAttribute(AttributeName = "Qty")]
    public decimal QuantityReceive
    { get; set; }

    [XmlAttribute(AttributeName = "n_disc")]
    public decimal n_disc
    { get; set; }

    [XmlAttribute(AttributeName = "n_salpri")]
    public decimal n_salpri
    { get; set; }

    [XmlAttribute(AttributeName = "Acceptance")]
    public decimal Acceptance
    { get; set; }

    [XmlAttribute(AttributeName = "Destroy")]
    public decimal Destroy
    { get; set; }

    [XmlAttribute(AttributeName = "Outlet")]
    public string Outlet
    { get; set; }

    [XmlAttribute(AttributeName = "TipeOutlet")]
    public string TipeOutlet
    { get; set; }

    public decimal Quantity
    { get; set; }

    [XmlAttribute(AttributeName = "C_REASON")]
    public string Keterangan
    { get; set; }
    
    [XmlAttribute(AttributeName = "batchterima")]
    public string batchterima
    { get; set; }
  }

  #region JSON Convert

  [Serializable]
  public class ReturCustomerResponse
  {
    public string ID
    { get; set; }

    public string RC
    { get; set; }

    public string USER
    { get; set; }

    public DateTime TanggalRC { get; set; }

    public string TanggalRC_Str { get; set; }

    public ReturCustomerStructureField[] Fields
    { get; set; }

    public static ReturCustomerResponse Deserialize(string rawData)
    {
      return StructureBase<ReturCustomerResponse>.SerializeJson(rawData);
    }

    public static string Serialize(ReturCustomerResponse strt)
    {
      return StructureBase<ReturCustomerResponse>.DeserializeJson(strt);
    }
  }

  [Serializable]
  public class ReturCustomerJSONStructure
  {
    public string ID
    { get; set; }

    public string RC
    { get; set; }

    public static ReturCustomerJSONStructure Deserialize(string rawData)
    {
      return StructureBase<ReturCustomerJSONStructure>.SerializeJson(rawData);
    }

    public static string Serialize(ReturCustomerJSONStructure strt)
    {
      return StructureBase<ReturCustomerJSONStructure>.DeserializeJson(strt);
    }

    public ReturCustomerJSONStructureField[] Fields
    { get; set; }
  }

  [Serializable]
  public class ReturCustomerJSONStructureField
  {
    public string Item
    { get; set; }

    public string Batch
    { get; set; }

    public string DO
    { get; set; }

    public decimal Qty
    { get; set; }

    public decimal Acc
    { get; set; }

    public decimal Dtry
    { get; set; }

    public string Type
    { get; set; }
  }

  #endregion
}
