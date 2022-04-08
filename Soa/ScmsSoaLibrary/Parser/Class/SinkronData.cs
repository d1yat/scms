using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Components;

namespace ScmsSoaLibrary.Parser.Class
{
  #region Order Customer Receive Transaksi

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class OrderCustomerReceiveStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static OrderCustomerReceiveStructure Serialize(string rawData)
    {
      OrderCustomerReceiveStructure ocs = StructureBase<OrderCustomerReceiveStructure>.Serialize(rawData);

      if (ocs != null)
      {
        if (ocs.Fields != null)
        {
          DateTime date = DateTime.Now;

          if (!string.IsNullOrEmpty(ocs.Fields.TanggalDO))
          {
            if (Functionals.DateParser(ocs.Fields.TanggalDO, "yyyyMMddHHmmssfff", out date))
            {
              ocs.Fields.TanggalDODate = date;
            }
            else
            {
              ocs.Fields.TanggalDODate = DateTime.MinValue;
            }
          }
          if (!string.IsNullOrEmpty(ocs.Fields.TanggalInvoice))
          {
            if (Functionals.DateParser(ocs.Fields.TanggalInvoice, "yyyyMMddHHmmssfff", out date))
            {
              ocs.Fields.TanggalInvoiceDate = date;
            }
            else
            {
              ocs.Fields.TanggalInvoiceDate = DateTime.MinValue;
            }
          }
          if (!string.IsNullOrEmpty(ocs.Fields.TanggalRN))
          {
            if (Functionals.DateParser(ocs.Fields.TanggalRN, "yyyyMMddHHmmssfff", out date))
            {
              ocs.Fields.TanggalRNDate = date;
            }
            else
            {
              ocs.Fields.TanggalRNDate = DateTime.MinValue;
            }
          }
        }
      }

      return ocs;
    }

    public static string Deserialize(OrderCustomerReceiveStructure strt)
    {
      return StructureBase<OrderCustomerReceiveStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public OrderCustomerReceiveStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class OrderCustomerReceiveStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string NoReceive
    { get; set; }

    [XmlAttribute(AttributeName = "Cabang")]
    public string Cabang
    { get; set; }

    [XmlAttribute(AttributeName = "Supplier")]
    public string Supplier
    { get; set; }

    [XmlAttribute(AttributeName = "RNDate")]
    public string TanggalRN
    { get; set; }
    
    [XmlIgnore]
    public DateTime TanggalRNDate
    { get; set; }

    [XmlAttribute(AttributeName = "NomorDO")]
    public string NomorDO
    { get; set; }

    [XmlAttribute(AttributeName = "DODate")]
    public string TanggalDO
    { get; set; }
    
    [XmlIgnore]
    public DateTime TanggalDODate
    { get; set; }

    [XmlAttribute(AttributeName = "NomorInvoice")]
    public string NomorInvoice
    { get; set; }

    [XmlAttribute(AttributeName = "InvoiceDate")]
    public string TanggalInvoice
    { get; set; }

    [XmlIgnore]
    public DateTime TanggalInvoiceDate
    { get; set; }

    [XmlAttribute(AttributeName = "TopValue")]
    public decimal Top
    { get; set; }

    [XmlAttribute(AttributeName = "Keterangan")]
    public string Keterangan
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public OrderCustomerReceiveStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class OrderCustomerReceiveStructureField
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
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

    [XmlAttribute(AttributeName = "Bonus")]
    public decimal Bonus
    { get; set; }

    [XmlAttribute(AttributeName = "Discount")]
    public decimal Discount
    { get; set; }

    [XmlAttribute(AttributeName = "Salpri")]
    public decimal Salpri
    { get; set; }

    [XmlAttribute(AttributeName = "SPNo")]
    public string NomorSP
    { get; set; }

    [XmlAttribute(AttributeName = "DivAMS")]
    public string DivisiAMS
    { get; set; }

    [XmlAttribute(AttributeName = "DivSupplier")]
    public string DivisiSupplier
    { get; set; }
  }

  #endregion
  
  #region Retur Customer Receive Transaksi

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class ReturCustomerReceiveStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static ReturCustomerReceiveStructure Serialize(string rawData)
    {
      ReturCustomerReceiveStructure ocs = StructureBase<ReturCustomerReceiveStructure>.Serialize(rawData);

      if (ocs != null)
      {
        if (ocs.Fields != null)
        {
          DateTime date = DateTime.Now;

          if (!string.IsNullOrEmpty(ocs.Fields.TanggalRetur))
          {
            if (Functionals.DateParser(ocs.Fields.TanggalRetur, "yyyyMMddHHmmssfff", out date))
            {
              ocs.Fields.TanggalReturDate = date;
            }
            else
            {
              ocs.Fields.TanggalReturDate = DateTime.MinValue;
            }
          }
          if (!string.IsNullOrEmpty(ocs.Fields.TanggalRC))
          {
            if (Functionals.DateParser(ocs.Fields.TanggalRC, "yyyyMMddHHmmssfff", out date))
            {
              ocs.Fields.TanggalRCDate = date;
            }
            else
            {
              ocs.Fields.TanggalRCDate = DateTime.MinValue;
            }
          }
          if (!string.IsNullOrEmpty(ocs.Fields.TanggalExFaktur))
          {
            if (Functionals.DateParser(ocs.Fields.TanggalExFaktur, "yyyyMMddHHmmssfff", out date))
            {
              ocs.Fields.TanggalExFakturDate = date;
            }
            else
            {
              ocs.Fields.TanggalExFakturDate = DateTime.MinValue;
            }
          }
          if (!string.IsNullOrEmpty(ocs.Fields.TanggalExFaktur2))
          {
            if (Functionals.DateParser(ocs.Fields.TanggalExFaktur2, "yyyyMMddHHmmssfff", out date))
            {
              ocs.Fields.TanggalExFaktur2Date = date;
            }
            else
            {
              ocs.Fields.TanggalExFaktur2Date = DateTime.MinValue;
            }
          }
        }
      }

      return ocs;
    }

    public static string Deserialize(ReturCustomerReceiveStructure strt)
    {
      return StructureBase<ReturCustomerReceiveStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public ReturCustomerReceiveStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class ReturCustomerReceiveStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "Cabang")]
    public string Cabang
    { get; set; }

    [XmlAttribute(AttributeName = "Supplier")]
    public string Supplier
    { get; set; }

    [XmlAttribute(AttributeName = "ID")]
    public string NomorRetur
    { get; set; }

    [XmlAttribute(AttributeName = "ReturDate")]
    public string TanggalRetur
    { get; set; }

    [XmlIgnore]
    public DateTime TanggalReturDate
    { get; set; }

    [XmlAttribute(AttributeName = "NomorRC")]
    public string NomorRC
    { get; set; }

    [XmlAttribute(AttributeName = "RCDate")]
    public string TanggalRC
    { get; set; }

    [XmlIgnore]
    public DateTime TanggalRCDate
    { get; set; }

    [XmlAttribute(AttributeName = "XFaktur")]
    public string ExFaktur
    { get; set; }

    [XmlAttribute(AttributeName = "XNoFaktur")]
    public string ExNoFaktur
    { get; set; }

    [XmlAttribute(AttributeName = "XNoFakturDate")]
    public string TanggalExFaktur
    { get; set; }

    [XmlIgnore]
    public DateTime TanggalExFakturDate
    { get; set; }

    [XmlAttribute(AttributeName = "XFakturCo")]
    public string ExFakturCo
    { get; set; }

    [XmlAttribute(AttributeName = "XFaktur2")]
    public string ExFaktur2
    { get; set; }

    [XmlAttribute(AttributeName = "XNoFaktur2")]
    public string ExNoFaktur2
    { get; set; }

    [XmlAttribute(AttributeName = "XNoFakturDate2")]
    public string TanggalExFaktur2
    { get; set; }

    [XmlIgnore]
    public DateTime TanggalExFaktur2Date
    { get; set; }

    [XmlAttribute(AttributeName = "ReturValue")]
    public decimal ReturValue
    { get; set; }

    [XmlAttribute(AttributeName = "PPNBM")]
    public decimal PpnBM
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public ReturCustomerReceiveStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class ReturCustomerReceiveStructureField
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "Delete")]
    public bool IsDelete
    { get; set; }

    [XmlAttribute(AttributeName = "Item")]
    public string Item
    { get; set; }

    [XmlAttribute(AttributeName = "QtyGood")]
    public decimal QuantityGood
    { get; set; }

    [XmlAttribute(AttributeName = "QtyBad")]
    public decimal QuantityBad
    { get; set; }

    [XmlAttribute(AttributeName = "BonusQtyGood")]
    public decimal BonusQuantityGood
    { get; set; }

    [XmlAttribute(AttributeName = "BonusQtyBad")]
    public decimal BonusQuantityBad
    { get; set; }

    [XmlAttribute(AttributeName = "Discount")]
    public decimal Discount
    { get; set; }

    [XmlAttribute(AttributeName = "Salpri")]
    public decimal Salpri
    { get; set; }

    [XmlAttribute(AttributeName = "DivAMS")]
    public string DivisiAMS
    { get; set; }

    [XmlAttribute(AttributeName = "DivSupplier")]
    public string DivisiSupplier
    { get; set; }
  }

  #endregion

  #region Recall Receive

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class RecallReceiveStructure
  {
      [XmlAttribute(AttributeName = "name")]
      public string Name
      { get; set; }

      [XmlAttribute(AttributeName = "method")]
      public string Method
      { get; set; }

      public static RecallReceiveStructure Serialize(string rawData)
      {
          return StructureBase<RecallReceiveStructure>.Serialize(rawData);
      }

      public static string Deserialize(RecallReceiveStructure strt)
      {
          return StructureBase<RecallReceiveStructure>.Deserialize(strt);
      }

      [XmlElement(ElementName = "Fields")]
      public RecallReceiveStructureFields Fields
      { get; set; }

  }

  [Serializable]
  public class RecallReceiveStructureFields
  {
      [XmlAttribute(AttributeName = "RecallNo")]
      public string RecallNo
      { get; set; }

      [XmlAttribute(AttributeName = "Tanggal")]
      public string Tanggal
      { get; set; }
      
      [XmlAttribute(AttributeName = "Memo")]
      public string Memo
      { get; set; }

      [XmlAttribute(AttributeName = "D_DISTFROM")]
      public string recallfrom
      { get; set; }

      [XmlAttribute(AttributeName = "D_DISTEND")]
      public string recallto
      { get; set; }

      [XmlAttribute(AttributeName = "C_SUBMIT")]
      public string entry
      { get; set; }

      [XmlElement(ElementName = "Field")]
      public RecallReceiveStructureField[] Field
      { get; set; }
  }

  [Serializable]
  public class RecallReceiveStructureField
  {
      [XmlAttribute(AttributeName = "Iteno")]
      public string Iteno
      { get; set; }

      [XmlAttribute(AttributeName = "Batches")]
      public string Batches
      { get; set; }
  }
  #endregion

  #region Relokasi
  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class RelokasiStructure
  {
      [XmlAttribute(AttributeName = "name")]
      public string Name
      { get; set; }

      [XmlAttribute(AttributeName = "method")]
      public string Method
      { get; set; }

      public static RelokasiStructure Serialize(string rawdata)
      {
          return StructureBase<RelokasiStructure>.Serialize(rawdata);
      }

      public static string Deserialize(RelokasiStructure strt)
      {
          return StructureBase<RelokasiStructure>.Deserialize(strt);
      }

      [XmlElement(ElementName = "Fields")]
      public RelokasiStructureFields Fields
      { get; set; }   

  }

  public class RelokasiStructureFields
  {
      [XmlAttribute(AttributeName = "C_ITENO")]
      public string C_ITENO
      { get; set; }

      [XmlElement(ElementName = "Field")]
      public RelokasiStructureField[] Field
      { get; set; }
      //[XmlAttribute(AttributeName = "")]
  }
    
  public class RelokasiStructureField
  {
      [XmlAttribute(AttributeName = "C_SPNO")]
      public string C_SPNO
      { get; set; }

      [XmlAttribute(AttributeName = "N_QTYREL")]
      public string N_QTYREL
      { get; set; }

  }


  #endregion

  #region Receive Relokasi

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class ReceiveRelokasiStructure
  {
      [XmlAttribute(AttributeName = "name")]
      public string Name
      { get; set; }

      [XmlAttribute(AttributeName = "method")]
      public string Method
      { get; set; }

      public static ReceiveRelokasiStructure Serialize(string rawdata)
      {
          //ReceiveRelokasiStructure rcs = StructureBase<ReceiveRelokasiStructure>.Serialize(rawdata);

          //if ((rcs.Fields != null) && (rcs.Fields.Field != null) && (rcs.Fields.Field.Length > 0))
          //{
          //    ReceiveRelokasiStructureField field = null;

          //    for (int nLoop = 0; nLoop < rcs.Fields.Field.Length; nLoop++)
          //    {
          //        field = rcs.Fields.Field[nLoop];

          //    }
          //}
          //return rcs;
          return StructureBase<ReceiveRelokasiStructure>.Serialize(rawdata);
      }

      public static string Deserialize(ReceiveRelokasiStructure strt)
      {
          return StructureBase<ReceiveRelokasiStructure>.Deserialize(strt);
      }

      [XmlElement(ElementName = "Fields")]
      public ReceiveRelokasiStructureFields Fields
      { get; set; }   

  }

  [Serializable]
  public class ReceiveRelokasiStructureFields
  {
      [XmlAttribute(AttributeName = "C_PBNO")]
      public string C_PBNO
      { get; set; }

      [XmlAttribute(AttributeName = "D_TRC")]
      public string Tanggal
      { get; set; }

      [XmlAttribute(AttributeName = "C_ORIGIN")]
      public string CabangAsal
      { get; set; }

      [XmlAttribute(AttributeName = "C_KODECAB")]
      public string CabangTerima
      { get; set; }

      [XmlAttribute(AttributeName = "C_TRCNO")]
      public string Keterangan
      { get; set; }

      [XmlAttribute(AttributeName = "RCID")]
      public string RCID
      { get; set; }

      [XmlElement(ElementName = "Field")]
      public ReceiveRelokasiStructureField[] Field
      { get; set; }
  }

  [Serializable]
  public class ReceiveRelokasiStructureField
  {
      [XmlAttribute(AttributeName = "C_ITENO")]
      public string ITENO
      { get; set; }
      public string Item
      { get; set; }

      [XmlAttribute(AttributeName = "N_QTYPB")]
      public string N_QTY_PBB
      { get; set; }
      public string Quantity
      { get; set; }

      [XmlAttribute(AttributeName = "N_QTYRCV")]
      public string N_QTY_RCV
      { get; set; }
      public string QuantityReceive
      { get; set; }

      [XmlAttribute(AttributeName = "N_QTYREL")]
      public string N_QTY_REL
      { get; set; }

      [XmlAttribute(AttributeName = "C_SPNO")]
      public string C_SPNO
      { get; set; }

      [XmlAttribute(AttributeName = "C_BATCH")]
      public string BATCH
      { get; set; }
      public string Batch
      { get; set; }

      [XmlAttribute(AttributeName = "C_BATCHRCV")]
      public string BATCH_TERIMA
      { get; set; }

      [XmlAttribute(AttributeName = "C_NOREF")]
      public string C_DONO
      { get; set; }

      public string NoDO
      { get; set; }

      public string n_salpri
      { get; set; }

      public string n_disc
      { get; set; }

      public string Acceptance
      { get; set; }

      public string Destroy
      { get; set; }

      public string ID
      { get; set; }

      [XmlAttribute(AttributeName = "D_EXPIRE")]
      public string d_expire
      { get; set; }
  }

  [Serializable]
  public class ReceiveRelokasiResponse
  {
      public string ID
      { get; set; }

      public string RC
      { get; set; }

      public string USER
      { get; set; }

      public DateTime TanggalRC { get; set; }

      public string TanggalRC_Str { get; set; }

      public ReceiveRelokasiStructureField[] Fields
      { get; set; }

      public static ReceiveRelokasiResponse Deserialize(string RawData)
      {
          return StructureBase<ReceiveRelokasiResponse>.SerializeJson(RawData);
      }

      public static string Serialize(ReceiveRelokasiResponse strt)
      {
          return StructureBase<ReceiveRelokasiResponse>.DeserializeJson(strt);
      }

  }

  [Serializable]
  public class ReceiveRelokasiJSONStructure
  {
      public string ID
      { get; set; }

      public string RC
      { get; set; }

      public static ReceiveRelokasiJSONStructure Deserialize(string rawData)
      {
          return StructureBase<ReceiveRelokasiJSONStructure>.SerializeJson(rawData);
      }

      public static string Serialize(ReceiveRelokasiJSONStructure strt)
      {
          return StructureBase<ReceiveRelokasiJSONStructure>.DeserializeJson(strt);
      }

      public ReceiveRelokasiJSONStructureField[] Fields
      { get; set; }
  }

  [Serializable]
  public class ReceiveRelokasiJSONStructureField
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

  #region receive master relokasi

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class MasterRelokasiStructure
  {
      [XmlAttribute(AttributeName = "name")]
      public string Name
      { get; set; }

      [XmlAttribute(AttributeName = "method")]
      public string Method
      { get; set; }

      public static MasterRelokasiStructure Serialize(string rawdata)
      {
          //ReceiveRelokasiStructure rcs = StructureBase<ReceiveRelokasiStructure>.Serialize(rawdata);

          //if ((rcs.Fields != null) && (rcs.Fields.Field != null) && (rcs.Fields.Field.Length > 0))
          //{
          //    ReceiveRelokasiStructureField field = null;

          //    for (int nLoop = 0; nLoop < rcs.Fields.Field.Length; nLoop++)
          //    {
          //        field = rcs.Fields.Field[nLoop];

          //    }
          //}
          //return rcs;
          return StructureBase<MasterRelokasiStructure>.Serialize(rawdata);
      }

      public static string Deserialize(MasterRelokasiStructure strt)
      {
          return StructureBase<MasterRelokasiStructure>.Deserialize(strt);
      }

      [XmlElement(ElementName = "Fields")]
      public MasterRelokasiStructureFields Fields
      { get; set; }

  }

  [Serializable]
  public class MasterRelokasiStructureFields
  {
      [XmlAttribute(AttributeName = "Cabang")]
      public string c_asal
      { get; set; }

      [XmlElement(ElementName = "Field")]
      public MasterRelokasiStructureField[] Field
      { get; set; }
  }

  [Serializable]
  public class MasterRelokasiStructureField
  {
      [XmlAttribute(AttributeName = "KODECAB")]
      public string c_asal
      { get; set; }

      [XmlAttribute(AttributeName = "JENIS")]
      public string c_jenis
      { get; set; }

      [XmlAttribute(AttributeName = "TUJUAN")]
      public string c_tujuan
      { get; set; }

      [XmlAttribute(AttributeName = "LTO")]
      public string c_lto
      { get; set; }

  }

  #endregion

  #region Cancel PB

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class CancelPBStructure
  {
      [XmlAttribute(AttributeName = "name")]
      public string Name
      { get; set; }

      [XmlAttribute(AttributeName = "method")]
      public string Method
      { get; set; }

      public static CancelPBStructure Serialize(string rawData)
      {
          return StructureBase<CancelPBStructure>.Serialize(rawData);
      }

      public static string Deserialize(CancelPBStructure strt)
      {
          return StructureBase<CancelPBStructure>.Deserialize(strt);
      }

      [XmlElement(ElementName = "Fields")]
      public CancelPBStructureFields Fields
      { get; set; }
  }

  [Serializable]
  public class CancelPBStructureFields
  {
      [XmlAttribute(AttributeName = "ID")]
      public string PBNO
      { get; set; }

      [XmlElement(ElementName = "Field")]
      public CancelPBStructureField[] Field
      { get; set; }
  }

  [Serializable]
  public class CancelPBStructureField
  {
      [XmlAttribute(AttributeName = "C_SPNO")]
      public string SPNO
      { get; set; }

      [XmlAttribute(AttributeName = "C_ITENO")]
      public string ITENO
      { get; set; }

      [XmlAttribute(AttributeName = "N_QTYREL")]
      public string N_QTYREL
      { get; set; }

  }

  #endregion

  #region Cancel SP

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class CancelSPStructure
  {
      [XmlAttribute(AttributeName = "name")]
      public string Name
      { get; set; }

      [XmlAttribute(AttributeName = "method")]
      public string Method
      { get; set; }

      public static CancelSPStructure Serialize(string rawData)
      {
          return StructureBase<CancelSPStructure>.Serialize(rawData);
      }

      public static string Deserialize(CancelSPStructure strt)
      {
          return StructureBase<CancelSPStructure>.Deserialize(strt);
      }

      [XmlElement(ElementName = "Fields")]
      public CancelSPStructureFields Fields
      { get; set; }
  }

  [Serializable]
  public class CancelSPStructureFields
  {
      [XmlAttribute(AttributeName = "C_SPNO")]
      public string SPNO
      { get; set; }

      [XmlAttribute(AttributeName = "C_ITENO")]
      public string ITENO
      { get; set; }

      [XmlAttribute(AttributeName = "N_QTYREL")]
      public string QTYREL
      { get; set; }

  }

  #endregion

  #region Surat Pesanan Receive Transaksi

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class SuratPesananReceiveStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static SuratPesananReceiveStructure Serialize(string rawData)
    {
      SuratPesananReceiveStructure sp = StructureBase<SuratPesananReceiveStructure>.Serialize(rawData);
      
      if (sp != null)
      {
        if (sp.Fields != null)
        {
          DateTime date = DateTime.Now;

          if (!string.IsNullOrEmpty(sp.Fields.Tanggal))
          {
            if (Functionals.DateParser(sp.Fields.Tanggal, "yyyyMMddHHmmssfff", out date))
            {
              sp.Fields.TanggalSP = date;
            }
            else
            {
              sp.Fields.TanggalSP = DateTime.MinValue;
            }
          }
        }
      }

      return sp;
    }

    public static string Deserialize(SuratPesananReceiveStructure strt)
    {
      return StructureBase<SuratPesananReceiveStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public SuratPesananReceiveStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class SuratPesananReceiveStructureFields
  {
    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "Customer")]
    public string Customer
    { get; set; }

    [XmlAttribute(AttributeName = "SpID")]
    public string SpSCMS
    { get; set; }

    [XmlAttribute(AttributeName = "Tanggal")]
    public string Tanggal
    { get; set; }

    [XmlAttribute(AttributeName = "TanggalSP")]
    public DateTime TanggalSP
    { get; set; }

    [XmlAttribute(AttributeName = "SPCabang")]
    public string SPCabang
    { get; set; }

    [XmlAttribute(AttributeName = "Tipe")]
    public string Tipe
    { get; set; }

    [XmlAttribute(AttributeName = "D_ETA")]
    public DateTime D_ETA //penambahan by suwandi 27 agustus 2018
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public SuratPesananReceiveStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class SuratPesananReceiveStructureField
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "Delete")]
    public bool IsDelete
    { get; set; }

    [XmlAttribute(AttributeName = "New")]
    public bool IsNew
    { get; set; }

    [XmlAttribute(AttributeName = "Item")]
    public string Item
    { get; set; }

    [XmlAttribute(AttributeName = "Qty")]
    public decimal Qty
    { get; set; }

    [XmlAttribute(AttributeName = "Acc")]
    public decimal Acc
    { get; set; }
  }

  #endregion

  #region Master Item Receive Transaksi

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class MasterItemReceiveStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }

    public static MasterItemReceiveStructure Serialize(string rawData)
    {
      return StructureBase<MasterItemReceiveStructure>.Serialize(rawData);
    }

    public static string Deserialize(MasterItemReceiveStructure strt)
    {
      return StructureBase<MasterItemReceiveStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public MasterItemReceiveStructureFields Fields
    { get; set; }


  }

  [Serializable]
  public class MasterItemReceiveStructureFields
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

    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlAttribute(AttributeName = "C_ACRONIM")]
    public string C_ACRONIM
    { get; set; }

    [XmlAttribute(AttributeName = "C_UNDES")]
    public string C_UNDES
    { get; set; }

    [XmlAttribute(AttributeName = "C_ITENOPRI")]
    public string C_ITENOPRI
    { get; set; }

    [XmlAttribute(AttributeName = "C_BARCODECD")]
    public string C_BARCODECD
    { get; set; }

    [XmlAttribute(AttributeName = "C_ALKES")]
    public string C_ALKES
    { get; set; }

    [XmlAttribute(AttributeName = "C_LOBCD")]
    public string C_LOBCD
    { get; set; }

    [XmlAttribute(AttributeName = "C_CLASS")]
    public string C_CLASS
    { get; set; }

    [XmlAttribute(AttributeName = "L_TYPEASKES")]
    public string L_TYPEASKES
    { get; set; }

    [XmlAttribute(AttributeName = "Tanggal")]
    public string Tanggal
    { get; set; }

    //[XmlAttribute(AttributeName = "New")]
    //public object New
    //{ get; set; }

    [XmlAttribute(AttributeName = "ItemID")]
    public string ItemID
    { get; set; }

    [XmlAttribute(AttributeName = "C_ITENO")]
    public string C_ITENO
    { get; set; }

    [XmlAttribute(AttributeName = "L_RETURNABLE")]
    public string L_RETURNABLE
    { get; set; }

    [XmlAttribute(AttributeName = "C_DELIVERY")]
    public string C_DELIVERY
    { get; set; }

    [XmlAttribute(AttributeName = "C_NOSUP")]
    public string C_NOSUP
    { get; set; }

    [XmlAttribute(AttributeName = "C_ITNAM")]
    public string C_ITNAM
    { get; set; }

    [XmlAttribute(AttributeName = "N_WEIGHT")]
    public string N_WEIGHT
    { get; set; }

    [XmlAttribute(AttributeName = "C_NOMEMO")]
    public string C_NOMEMO
    { get; set; }

    [XmlAttribute(AttributeName = "C_FARMALKES")]
    public string C_FARMALKES
    { get; set; }

    [XmlAttribute(AttributeName = "N_DIMENTION")]
    public string N_DIMENTION
    { get; set; }

    [XmlAttribute(AttributeName = "C_PACKTYPECD")]
    public string C_PACKTYPECD
    { get; set; }

    [XmlAttribute(AttributeName = "C_COMPOSITION")]
    public string C_COMPOSITION
    { get; set; }

    [XmlAttribute(AttributeName = "C_REGISTRATION")]
    public string C_REGISTRATION
    { get; set; }

    [XmlAttribute(AttributeName = "C_PACKTYPE_VALUE")]
    public string C_PACKTYPE_VALUE
    { get; set; }

    [XmlAttribute(AttributeName = "C_PACKUNITCD")]
    public string C_PACKUNITCD
    { get; set; }

    [XmlAttribute(AttributeName = "C_PACKFORM_VALUE")]
    public string C_PACKFORM_VALUE
    { get; set; }

    [XmlAttribute(AttributeName = "C_ARTIKELCD")]
    public string C_ARTIKELCD
    { get; set; }

    [XmlAttribute(AttributeName = "C_VIA")]
    public string C_VIA
    { get; set; }

    [XmlAttribute(AttributeName = "C_PACKFORMCD")]
    public string C_PACKFORMCD
    { get; set; }

    [XmlAttribute(AttributeName = "C_CHANNELCD_STR")]
    public string C_CHANNELCD_STR
    { get; set; }

    [XmlAttribute(AttributeName = "C_REPDEPKES")]
    public string C_REPDEPKES
    { get; set; }

    [XmlAttribute(AttributeName = "C_KDDIVPRI")]
    public string C_KDDIVPRI
    { get; set; }
  }

  #endregion

  #region Receive PO

  [Serializable]
  [XmlRoot(ElementName = "Structure")]
  public class ReceivePOStructure
  {
      [XmlAttribute(AttributeName = "name")]
      public string Name
      { get; set; }

      [XmlAttribute(AttributeName = "method")]
      public string Method
      { get; set; }

      public static ReceivePOStructure Serialize(string rawData)
      {
          return StructureBase<ReceivePOStructure>.Serialize(rawData);
      }

      public static string Deserialize(ReceivePOStructure strt)
      {
          return StructureBase<ReceivePOStructure>.Deserialize(strt);
      }

      [XmlElement(ElementName = "Fields")]
      public ReceivePOStructureFields Fields
      { get; set; }
  }

  [Serializable]
  public class ReceivePOStructureFields
  {
      [XmlAttribute(AttributeName = "C_PONO")]
      public string C_PONO
      { get; set; }

      [XmlAttribute(AttributeName = "D_PODATE")]
      public string D_PODATE
      { get; set; }

      [XmlAttribute(AttributeName = "C_NOSUP")]
      public string C_NOSUP
      { get; set; }

      [XmlAttribute(AttributeName = "L_IMPORT")]
      public string L_IMPORT
      { get; set; }

      [XmlAttribute(AttributeName = "V_KET")]
      public string V_KET
      { get; set; }

      [XmlAttribute(AttributeName = "N_BRUTO")]
      public string N_BRUTO
      { get; set; }

      [XmlAttribute(AttributeName = "N_DISC")]
      public string N_DISC
      { get; set; }

      [XmlAttribute(AttributeName = "N_PPN")]
      public string N_PPN
      { get; set; }

      [XmlAttribute(AttributeName = "N_BILVA")]
      public string N_BILVA
      { get; set; }

      [XmlAttribute(AttributeName = "C_ORNO")]
      public string C_ORNO
      { get; set; }

      [XmlAttribute(AttributeName = "C_TYPE")]
      public string C_TYPE
      { get; set; }

      [XmlAttribute(AttributeName = "C_ENTRY")]
      public string C_ENTRY
      { get; set; }

      [XmlAttribute(AttributeName = "C_CAB")]
      public string C_CAB
      { get; set; }

      [XmlAttribute(AttributeName = "N_LEADTIME")]
      public string N_LEADTIME
      { get; set; }

      [XmlAttribute(AttributeName = "C_KDDIVPRI")]
      public string C_KDDIVPRI
      { get; set; }

      [XmlElement(ElementName = "Field")]
      public ReceivePOStructureField[] field
      { get; set; }
  }

  [Serializable]
  public class ReceivePOStructureField
  {
      [XmlAttribute(AttributeName = "C_ITENO")]
      public string C_ITENO
      { get; set; }

      [XmlAttribute(AttributeName = "N_QTY")]
      public string N_QTY
      { get; set; }

      [XmlAttribute(AttributeName = "N_DISC")]
      public string N_DISC
      { get; set; }

      [XmlAttribute(AttributeName = "N_SALPRI")]
      public string N_SALPRI
      { get; set; }

      [XmlAttribute(AttributeName = "N_SISA")]
      public string N_SISA
      { get; set; }

      [XmlAttribute(AttributeName = "C_SPNO")]
      public string C_SPNO
      { get; set; }

  }


    #endregion
}
