using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Components;

namespace ScmsSoaLibrary.Parser.Class
{
  #region MK Memo Combo

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class MKMemoComboStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static MKMemoComboStructure Serialize(string rawData)
    {
      return StructureBase<MKMemoComboStructure>.Serialize(rawData);
    }

    public static string Deserialize(MKMemoComboStructure strt)
    {
      return StructureBase<MKMemoComboStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public MKMemoComboStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class MKMemoComboStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string MemoID
    { get; set; }

    [XmlAttribute(AttributeName = "Memo")]
    public string MemoRequest
    { get; set; }

    [XmlAttribute(AttributeName = "Gudang")]
    public string Gudang
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan1")]
    public string Keterangan1
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan2")]
    public string Keterangan2
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public MKMemoComboStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class MKMemoComboStructureField
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

    [XmlAttribute(AttributeName = "Box")]
    public decimal BoxQuantity
    { get; set; }

    [XmlText]
    public string Keterangan
    { get; set; }
  }

  #endregion

  #region LG Combo

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class MemoComboStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static MemoComboStructure Serialize(string rawData)
    {
      MemoComboStructure mcs = StructureBase<MemoComboStructure>.Serialize(rawData);

      if (mcs.Fields != null)
      {
        mcs.Fields.Batch = (string.IsNullOrEmpty(mcs.Fields.Batch) ? string.Empty : mcs.Fields.Batch.Trim());

        if ((mcs.Fields.Field != null) && (mcs.Fields.Field.Length > 0))
        {
          MemoComboStructureField field = null;

          for (int nLoop = 0; nLoop < mcs.Fields.Field.Length; nLoop++)
          {
            field = mcs.Fields.Field[nLoop];

            if (field != null)
            {
              field.Batch = (string.IsNullOrEmpty(field.Batch) ? string.Empty : field.Batch.Trim());
            }
          }
        }
      }

      return mcs;
    }

    public static string Deserialize(MemoComboStructure strt)
    {
      return StructureBase<MemoComboStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public MemoComboStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class MemoComboStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string ComboID
    { get; set; }

    [XmlAttribute(AttributeName = "MemoID")]
    public string MemoID
    { get; set; }

    [XmlAttribute(AttributeName = "Gudang")]
    public string Gudang
    { get; set; }

    [XmlAttribute(AttributeName = "ComboItem")]
    public string ComboItem
    { get; set; }

    [XmlAttribute(AttributeName = "Batch")]
    public string Batch
    { get; set; }

    [XmlAttribute(AttributeName = "Qty")]
    public decimal Quantity
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlAttribute(AttributeName = "IsConfirm")]
    public bool IsConfirm
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public MemoComboStructureField[] Field
    { get; set; }
  }
  
  [Serializable]
  public class MemoComboStructureField
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

    [XmlAttribute(AttributeName = "Komposisi")]
    public decimal Komposisi
    { get; set; }

    [XmlAttribute(AttributeName = "Batch")]
    public string Batch
    { get; set; }
  
    //[XmlAttribute(AttributeName = "RN")]
    //public string ReceiveNote
    //{ get; set; }

    [XmlText]
    public string Keterangan
    { get; set; }
  }

  #endregion

  #region MK Memo STT

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class MKMemoSTTStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static MKMemoSTTStructure Serialize(string rawData)
    {
      return StructureBase<MKMemoSTTStructure>.Serialize(rawData);
    }

    public static string Deserialize(MKMemoSTTStructure strt)
    {
      return StructureBase<MKMemoSTTStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public MKMemoSTTStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class MKMemoSTTStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string MemoID
    { get; set; }

    [XmlAttribute(AttributeName = "MemoID")]
    public string MemoRequest
    { get; set; }

    [XmlAttribute(AttributeName = "Gudang")]
    public string Gudang
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlAttribute(AttributeName = "Type")]
    public string Type
    { get; set; }

    [XmlAttribute(AttributeName = "Nip")]
    public string Nip
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public MKMemoSTTStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class MKMemoSTTStructureField
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

    [XmlText]
    public string Keterangan
    { get; set; }
  }

  #endregion

  #region MK Memo Pemusnahan

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class MKMemoPemusnahanStructure
  {
      [XmlAttribute(AttributeName = "name")]
      public string Name
      { get; set; }

      [XmlAttribute(AttributeName = "method")]
      public string Method
      { get; set; }

      public static MKMemoPemusnahanStructure Serialize(string rawData)
      {
          return StructureBase<MKMemoPemusnahanStructure>.Serialize(rawData);
      }

      public static string Deserialize(MKMemoPemusnahanStructure strt)
      {
          return StructureBase<MKMemoPemusnahanStructure>.Deserialize(strt);
      }

      [XmlElement(ElementName = "Fields")]
      public MKMemoPemusnahanStructureFields Fields
      { get; set; }
  }

  [Serializable]
  public class MKMemoPemusnahanStructureFields
  {
      [XmlAttribute(AttributeName = "Entry")]
      public string Entry
      { get; set; }

      [XmlAttribute(AttributeName = "ID")]
      public string MemoID
      { get; set; }

      [XmlAttribute(AttributeName = "MemoID")]
      public string MemoRequest
      { get; set; }

      [XmlAttribute(AttributeName = "Gudang")]
      public string Gudang
      { get; set; }

      [XmlAttribute(AttributeName = "Keterangan")]
      public string Keterangan
      { get; set; }

      [XmlElement(ElementName = "Field")]
      public MKMemoPemusnahanStructureField[] Field
      { get; set; }
  }

  [Serializable]
  public class MKMemoPemusnahanStructureField
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

      [XmlText]
      public string Keterangan
      { get; set; }
  }

  #endregion

  #region MK Memo BASPB SJ

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class MKMemoBASPBSJStructure
  {
      [XmlAttribute(AttributeName = "name")]
      public string Name
      { get; set; }

      [XmlAttribute(AttributeName = "method")]
      public string Method
      { get; set; }

      public static MKMemoBASPBSJStructure Serialize(string rawData)
      {
          return StructureBase<MKMemoBASPBSJStructure>.Serialize(rawData);
      }

      public static string Deserialize(MKMemoBASPBSJStructure strt)
      {
          return StructureBase<MKMemoBASPBSJStructure>.Deserialize(strt);
      }

      [XmlElement(ElementName = "Fields")]
      public MKMemoBASPBSJStructureFields Fields
      { get; set; }
  }

  [Serializable]
  public class MKMemoBASPBSJStructureFields
  {
      [XmlAttribute(AttributeName = "Entry")]
      public string Entry
      { get; set; }

      [XmlAttribute(AttributeName = "ID")]
      public string MemoID
      { get; set; }

      [XmlAttribute(AttributeName = "gdgasal")]
      public string gdgasal
      { get; set; }

      [XmlAttribute(AttributeName = "gdgtujuan")]
      public string gdgtujuan
      { get; set; }

      [XmlAttribute(AttributeName = "sjno")]
      public string sjno
      { get; set; }

      [XmlAttribute(AttributeName = "Keterangan")]
      public string Keterangan
      { get; set; }

      [XmlElement(ElementName = "Field")]
      public MKMemoBASPBSJStructureField[] Field
      { get; set; }
  }

  [Serializable]
  public class MKMemoBASPBSJStructureField
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

      [XmlAttribute(AttributeName = "QtyDO")]
      public decimal QuantityDO
      { get; set; }

      [XmlAttribute(AttributeName = "QtyDiff")]
      public decimal QuantityDiff
      { get; set; }

      [XmlAttribute(AttributeName = "claimType")]
      public string claimType
      { get; set; }

      [XmlText]
      public string Keterangan
      { get; set; }
  }

  #endregion
}
