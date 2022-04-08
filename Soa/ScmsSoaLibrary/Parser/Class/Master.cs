using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Components;

namespace ScmsSoaLibrary.Parser.Class
{
    #region Master Approval
    [Serializable]
    [XmlRoot(ElementName = "Structure")]

    public class MasterApprovalStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "method")]
        public string Method { get; set; }

        public static MasterApprovalStructure Serialize(string rawData)
        {
            MasterApprovalStructure discStrt = StructureBase<MasterApprovalStructure>.Serialize(rawData);

            //if (discStrt != null)
            //{
            //  if (!string.IsNullOrEmpty(discStrt.Fields.DateNP))
            //  {
            //    DateTime date = DateTime.MinValue;

            //    if (DateTime.TryParseExact(discStrt.Fields.DateNP, "yyyyMMddHHmmss", new System.Globalization.CultureInfo("en-US"), System.Globalization.DateTimeStyles.AssumeLocal,
            //      out date))
            //    {
            //      discStrt.Fields.DateNP = date;
            //    }
            //  }
            //}

            return discStrt;
        }
        public static string Deserialize(MasterApprovalStructure strt)
        {
            return StructureBase<MasterApprovalStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterApprovalStructureFields Fields
        { get; set; }
    }
    [Serializable]
    public class MasterApprovalStructureFields
    {
        [XmlAttribute(AttributeName = "cusno")]
        public string cusno { get; set; }

        [XmlAttribute(AttributeName = "kdapproval")]
        public string kdapproval { get; set; }

        [XmlAttribute(AttributeName = "status")]
        public string status
        { get; set; }

        [XmlAttribute(AttributeName = "NIP")]
        public string NIP
        { get; set; }

        [XmlAttribute(AttributeName = "d_entry")]
        public string d_entry
        { get; set; }

        [XmlAttribute(AttributeName = "param")]
        public string param
        { get; set; }

        [XmlAttribute(AttributeName = "param2")]
        public string param2
        { get; set; }
    }
    #endregion

    #region Master Item

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class MasterItemStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MasterItemStructure Serialize(string rawData)
        {
            MasterItemStructure itm = StructureBase<MasterItemStructure>.Serialize(rawData);

            if (itm != null)
            {
                if ((itm.Fields != null) && (!string.IsNullOrEmpty(itm.Fields.Tanggal)))
                {
                    DateTime date = DateTime.Now;

                    if (Functionals.DateParser(itm.Fields.Tanggal, "yyyyMMdd", out date))
                    {
                        itm.Fields.TanggalNie = date;
                    }
                    else
                    {
                        itm.Fields.TanggalNie = DateTime.MinValue;
                    }
                }
            }

            return itm;
            //return StructureBase<MasterItemStructure>.Serialize(rawData);
        }

        public static string Deserialize(MasterItemStructure strt)
        {
            return StructureBase<MasterItemStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterItemStructureFields Fields
        { get; set; }


    }

    [Serializable]
    public class MasterItemStructureFields
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

        [XmlAttribute(AttributeName = "ItemID")]
        public string ItemID
        { get; set; }

        [XmlAttribute(AttributeName = "MinQ")]
        public decimal MinQ
        { get; set; }

        [XmlAttribute(AttributeName = "HET")]
        public decimal HET
        { get; set; }

        [XmlAttribute(AttributeName = "Alkes")]
        public string Alkes
        { get; set; }

        [XmlAttribute(AttributeName = "Dinkes")]
        public bool Dinkes
        { get; set; }

        [XmlAttribute(AttributeName = "nosup")]
        public string nosup
        { get; set; }

        [XmlAttribute(AttributeName = "undes")]
        public string undes
        { get; set; }

        [XmlAttribute(AttributeName = "salpri")]
        public string salpri
        { get; set; }

        [XmlAttribute(AttributeName = "status")]
        public string status
        { get; set; }

        [XmlAttribute(AttributeName = "Nie")]
        public string Nie
        { get; set; }

        [XmlAttribute(AttributeName = "Tanggal")]
        public string Tanggal
        { get; set; }

        [XmlIgnore]
        public DateTime TanggalNie
        { get; set; }

        [XmlAttribute(AttributeName = "Komposisi")]
        public string Komposisi
        { get; set; }

        [XmlAttribute(AttributeName = "Golongan")]
        public string Golongan
        { get; set; }

        [XmlAttribute(AttributeName = "Box")]
        public decimal Box
        { get; set; }
    }

    #region JSON Convert

    [Serializable]
    public class MasterItemResponse
    {
        public string ID { get; set; }

        public string Iteno { get; set; }

        public static MasterItemResponse Deserialize(string rawData)
        {
            return StructureBase<MasterItemResponse>.SerializeJson(rawData);
        }

        public static string Serialize(MasterItemResponse strt)
        {
            return StructureBase<MasterItemResponse>.DeserializeJson(strt);
        }

        public MasterItemResponseJSONStructureField[] Fields
        { get; set; }
    }

    [Serializable]
    public class MasterItemResponseJSONStructureField
    {
        public string C_ITENO
        { get; set; }

        public decimal N_BOX
        { get; set; }
    }

    #endregion

    #endregion

    #region Discount

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class DiscountStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static DiscountStructure Serialize(string rawData)
        {
            DiscountStructure discStrt = StructureBase<DiscountStructure>.Serialize(rawData);

            DateTime date = DateTime.Now;
            DiscountStructureField dsf = null;

            if (discStrt != null)
            {
                if (discStrt.Fields != null)
                {
                    if (!string.IsNullOrEmpty(discStrt.Fields.TanggalAwal))
                    {
                        if (Functionals.DateParser(discStrt.Fields.TanggalAwal, "yyyyMMddHHmmssfff", out date))
                        {
                            discStrt.Fields.TanggalAwalDate = date;
                        }
                        else
                        {
                            discStrt.Fields.TanggalAwalDate = DateTime.MinValue;
                        }
                    }
                    if (!string.IsNullOrEmpty(discStrt.Fields.TanggalAkhir))
                    {
                        if (Functionals.DateParser(discStrt.Fields.TanggalAkhir, "yyyyMMddHHmmssfff", out date))
                        {
                            discStrt.Fields.TanggalAkhirDate = date;
                        }
                        else
                        {
                            discStrt.Fields.TanggalAkhirDate = DateTime.MinValue;
                        }
                    }

                    if ((discStrt.Fields.Field != null) && (discStrt.Fields.Field.Length > 0))
                    {
                        for (int nLoop = 0; nLoop < discStrt.Fields.Field.Length; nLoop++)
                        {
                            dsf = discStrt.Fields.Field[nLoop];

                            if (!string.IsNullOrEmpty(dsf.TanggalAwal))
                            {
                                if (Functionals.DateParser(dsf.TanggalAwal, "yyyyMMddHHmmssfff", out date))
                                {
                                    dsf.TanggalAwalDate = date;
                                }
                                else
                                {
                                    dsf.TanggalAwalDate = DateTime.MinValue;
                                }
                            }
                            if (!string.IsNullOrEmpty(dsf.TanggalAkhir))
                            {
                                if (Functionals.DateParser(dsf.TanggalAkhir, "yyyyMMddHHmmssfff", out date))
                                {
                                    dsf.TanggalAkhirDate = date;
                                }
                                else
                                {
                                    dsf.TanggalAkhirDate = DateTime.MinValue;
                                }
                            }
                        }
                    }
                }
            }

            return discStrt;
        }

        public static string Deserialize(DiscountStructure strt)
        {
            return StructureBase<DiscountStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public DiscountStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class DiscountStructureFields
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public string DiscountID
        { get; set; }

        [XmlAttribute(AttributeName = "Item")]
        public string Item
        { get; set; }

        [XmlAttribute(AttributeName = "Active")]
        public bool Aktif
        { get; set; }

        [XmlAttribute(AttributeName = "DiscOn")]
        public decimal DiscountOn
        { get; set; }

        [XmlAttribute(AttributeName = "DiscOff")]
        public decimal DiscountOff
        { get; set; }

        [XmlAttribute(AttributeName = "DateStart")]
        public string TanggalAwal
        { get; set; }

        public DateTime TanggalAwalDate
        { get; set; }

        [XmlAttribute(AttributeName = "DateEnd")]
        public string TanggalAkhir
        { get; set; }

        public DateTime TanggalAkhirDate
        { get; set; }

        [XmlAttribute(AttributeName = "Keterangan")]
        public string Keterangan
        { get; set; }

        [XmlElement(ElementName = "Field")]
        public DiscountStructureField[] Field
        { get; set; }
    }

    [Serializable]
    public class DiscountStructureField
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

        [XmlAttribute(AttributeName = "ID")]
        public decimal IndexID
        { get; set; }

        [XmlAttribute(AttributeName = "Active")]
        public bool Aktif
        { get; set; }

        [XmlAttribute(AttributeName = "DiscOn")]
        public decimal DiscountOn
        { get; set; }

        [XmlAttribute(AttributeName = "DiscOff")]
        public decimal DiscountOff
        { get; set; }

        [XmlAttribute(AttributeName = "DateStart")]
        public string TanggalAwal
        { get; set; }

        [XmlIgnore]
        public DateTime TanggalAwalDate
        { get; set; }

        [XmlAttribute(AttributeName = "DateEnd")]
        public string TanggalAkhir
        { get; set; }

        [XmlIgnore]
        public DateTime TanggalAkhirDate
        { get; set; }

        [XmlText]
        public string KeteranganMod
        { get; set; }
    }

    #endregion

    #region Batch Item

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class BatchItemStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static BatchItemStructure Serialize(string rawData)
        {
            BatchItemStructure biStrt = StructureBase<BatchItemStructure>.Serialize(rawData);

            DateTime date = DateTime.Now;

            if (biStrt != null)
            {
                if (biStrt.Fields != null)
                {
                    if (!string.IsNullOrEmpty(biStrt.Fields.TanggalExpire))
                    {
                        if (Functionals.DateParser(biStrt.Fields.TanggalExpire, "yyyyMMddHHmmssfff", out date))
                        {
                            biStrt.Fields.TanggalExpireDate = date;
                        }
                        else
                        {
                            biStrt.Fields.TanggalExpireDate = DateTime.MinValue;
                        }
                    }
                }
            }

            return biStrt;
        }

        public static string Deserialize(BatchItemStructure strt)
        {
            return StructureBase<BatchItemStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public BatchItemStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class BatchItemStructureFields
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public string ItemID
        { get; set; }

        [XmlAttribute(AttributeName = "Batch")]
        public string BatchID
        { get; set; }

        [XmlAttribute(AttributeName = "DateExpiration")]
        public string TanggalExpire
        { get; set; }

        [XmlIgnore]
        public DateTime TanggalExpireDate
        { get; set; }

        [XmlAttribute(AttributeName = "Keterangan")]
        public string Keterangan
        { get; set; }

        [XmlAttribute(AttributeName = "Gudang")]
        public string Gudang
        { get; set; }
    }

    #endregion

    #region Budget Limit

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class BudgetLimitStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static BudgetLimitStructure Serialize(string rawData)
        {
            return StructureBase<BudgetLimitStructure>.Serialize(rawData);
        }

        public static string Deserialize(BudgetLimitStructure strt)
        {
            return StructureBase<BudgetLimitStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public BudgetLimitStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class BudgetLimitStructureFields
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlAttribute(AttributeName = "Delete")]
        public bool IsDelete
        { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public string SupplierID
        { get; set; }

        [XmlAttribute(AttributeName = "Tahun")]
        public int PeriodeTahun
        { get; set; }

        [XmlAttribute(AttributeName = "Bulan")]
        public int PeriodeBulan
        { get; set; }

        [XmlAttribute(AttributeName = "Limit")]
        public decimal Limit
        { get; set; }

        [XmlAttribute(AttributeName = "Persen")]
        public decimal Persentase
        { get; set; }

        [XmlAttribute(AttributeName = "Keterangan")]
        public string Keterangan
        { get; set; }
    }

    #endregion

    #region Prinsipal

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class MasterPrinsipalStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MasterPrinsipalStructure Serialize(string rawData)
        {
            MasterPrinsipalStructure discStrt = StructureBase<MasterPrinsipalStructure>.Serialize(rawData);

            DateTime date = DateTime.Now;

            if (discStrt != null)
            {
                if (discStrt.Fields != null)
                {
                    if (!string.IsNullOrEmpty(discStrt.Fields.Date))
                    {
                        if (Functionals.DateParser(discStrt.Fields.Date, "yyyyMMddHHmmssfff", out date))
                        {
                            discStrt.Fields.DateNpkp = date;
                        }
                        else
                        {
                            discStrt.Fields.DateNpkp = DateTime.MinValue;
                        }                        
                    }
                    //Indra 20180815FM
                    if (!string.IsNullOrEmpty(discStrt.Fields.EfektiveDate))
                    {

                        if (Functionals.DateParser(discStrt.Fields.EfektiveDate, "yyyyMMdd", out date))
                        {
                            discStrt.Fields.DateEfektiveDate = date;
                        }
                        else
                        {
                            discStrt.Fields.DateEfektiveDate = DateTime.MinValue;
                        } 
                    }
                }
            }

            return discStrt;
        }

        public static string Deserialize(MasterPrinsipalStructure strt)
        {
            return StructureBase<MasterPrinsipalStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterPrinsipalStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class MasterPrinsipalStructureFields
    {
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

        [XmlAttribute(AttributeName = "Keterangan")]
        public string Keterangan
        { get; set; }

        [XmlAttribute(AttributeName = "SupplierID")]
        public string SupplierID
        { get; set; }

        [XmlAttribute(AttributeName = "Acc")]
        public string Acc
        { get; set; }

        [XmlAttribute(AttributeName = "Acc1")]
        public string Acc1
        { get; set; }

        [XmlAttribute(AttributeName = "Acc2")]
        public string Acc2
        { get; set; }

        [XmlAttribute(AttributeName = "Alamat1")]
        public string Alamat1
        { get; set; }

        [XmlAttribute(AttributeName = "Alamat2")]
        public string Alamat2
        { get; set; }

        [XmlAttribute(AttributeName = "AlamatBank")]
        public string AlamatBank
        { get; set; }

        [XmlAttribute(AttributeName = "Area")]
        public string Area
        { get; set; }

        [XmlAttribute(AttributeName = "Bank")]
        public string Bank
        { get; set; }

        [XmlAttribute(AttributeName = "Contact")]
        public string Contact
        { get; set; }

        [XmlAttribute(AttributeName = "Disc")]
        public decimal Disc
        { get; set; }

        [XmlAttribute(AttributeName = "Fax1")]
        public string Fax1
        { get; set; }

        [XmlAttribute(AttributeName = "Fax2")]
        public string Fax2
        { get; set; }

        [XmlAttribute(AttributeName = "Fax3")]
        public string Fax3
        { get; set; }

        [XmlAttribute(AttributeName = "Index")]
        public decimal Index
        { get; set; }

        [XmlAttribute(AttributeName = "KodeGol")]
        public string KodeGol
        { get; set; }

        [XmlAttribute(AttributeName = "Lead")]
        public decimal Lead
        { get; set; }

        [XmlAttribute(AttributeName = "Nama")]
        public string Nama
        { get; set; }

        [XmlAttribute(AttributeName = "Nppkp")]
        public string Nppkp
        { get; set; }

        [XmlAttribute(AttributeName = "Npwp")]
        public string Npwp
        { get; set; }

        [XmlAttribute(AttributeName = "Owner")]
        public string Owner
        { get; set; }

        [XmlAttribute(AttributeName = "Phone1")]
        public string Phone1
        { get; set; }

        [XmlAttribute(AttributeName = "Phone2")]
        public string Phone2
        { get; set; }

        [XmlAttribute(AttributeName = "Phone3")]
        public string Phone3
        { get; set; }

        [XmlAttribute(AttributeName = "Tax")]
        public string Tax
        { get; set; }

        [XmlAttribute(AttributeName = "Top")]
        public decimal Top
        { get; set; }

        [XmlAttribute(AttributeName = "IsAktif")]
        public bool IsAktif
        { get; set; }

        [XmlAttribute(AttributeName = "IsFax")]
        public bool IsFax
        { get; set; }

        [XmlAttribute(AttributeName = "IsImport")]
        public bool IsImport
        { get; set; }

        [XmlAttribute(AttributeName = "IsKons")]
        public bool IsKons
        { get; set; }

        [XmlAttribute(AttributeName = "IsHide")]
        public bool IsHide
        { get; set; }

        [XmlAttribute(AttributeName = "Date")]
        public string Date
        { get; set; }

        public DateTime DateNpkp
        { get; set; }

        [XmlAttribute(AttributeName = "AlamatTax1")]
        public string AlamatTax1
        { get; set; }

        [XmlAttribute(AttributeName = "AlamatTax2")]
        public string AlamatTax2
        { get; set; }

        [XmlAttribute(AttributeName = "NamaTax")]
        public string NamaTax
        { get; set; }

        #region Leadtime Principal Indra 20180815FM

        [XmlAttribute(AttributeName = "txLeadtimeAwal")]
        public decimal LeadtimeAwal
        { get; set; }

        [XmlAttribute(AttributeName = "txLeadtimePerubahan")]
        public decimal LeadtimePerubahan
        { get; set; }

        [XmlAttribute(AttributeName = "dtEfektiveDate")]
        public string EfektiveDate
        { get; set; }

        [XmlAttribute(AttributeName = "txAlasanPerubahan")]
        public string AlasanPerubahan
        { get; set; }

        [XmlAttribute(AttributeName = "txRequestor")]
        public string Requestor
        { get; set; }

        [XmlAttribute(AttributeName = "txAlasanTolakSetuju")]
        public string AlasanTolakSetuju
        { get; set; }

        [XmlIgnore]
        public DateTime DateEfektiveDate
        { get; set; }

        [XmlAttribute(AttributeName = "txhftypedo")]
        public string TypeDo
        { get; set; }

        [XmlAttribute(AttributeName = "TipePerubahan")]
        public string TipePerubahan
        { get; set; }

        #endregion
    }

    #endregion

    #region Master Div Prinsipal

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class MasterDivisiPrinsipalStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MasterDivisiPrinsipalStructure Serialize(string rawData)
        {
            return StructureBase<MasterDivisiPrinsipalStructure>.Serialize(rawData);
        }

        public static string Deserialize(MasterDivisiPrinsipalStructure strt)
        {
            return StructureBase<MasterDivisiPrinsipalStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterDivisiPrinsipalStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class MasterDivisiPrinsipalStructureFields
    {
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

        [XmlAttribute(AttributeName = "DivSupplierID")]
        public string DivSupplierID
        { get; set; }

        [XmlAttribute(AttributeName = "SupplierId")]
        public string SupplierId
        { get; set; }

        [XmlAttribute(AttributeName = "Nama")]
        public string Nama
        { get; set; }

        [XmlAttribute(AttributeName = "isAktif")]
        public bool isAktif
        { get; set; }

        [XmlAttribute(AttributeName = "isHide")]
        public bool isHide
        { get; set; }

        [XmlAttribute(AttributeName = "idxp")]
        public decimal idxp
        { get; set; }

        [XmlAttribute(AttributeName = "idxnp")]
        public decimal idxnp
        { get; set; }

        [XmlAttribute(AttributeName = "het")]
        public decimal het
        { get; set; }
    }

    #endregion

    #region Master Div Prinsipal Item

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class MasterDivPrinsipalItemStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MasterDivPrinsipalItemStructure Serialize(string rawData)
        {
            return StructureBase<MasterDivPrinsipalItemStructure>.Serialize(rawData);
        }

        public static string Deserialize(MasterDivPrinsipalItemStructure strt)
        {
            return StructureBase<MasterDivPrinsipalItemStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterDivPrinsipalItemStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class MasterDivPrinsipalItemStructureFields
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlAttribute(AttributeName = "DivID")]
        public string DivID
        { get; set; }

        [XmlElement(ElementName = "Field")]
        public MasterDivPrinsipalItemStructureField[] Field
        { get; set; }
    }

    [Serializable]
    public class MasterDivPrinsipalItemStructureField
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

        [XmlText]
        public string KeteranganMod
        { get; set; }
    }

    #endregion

    #region Customer

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class MasterCustomerStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MasterCustomerStructure Serialize(string rawData)
        {
            MasterCustomerStructure discStrt = StructureBase<MasterCustomerStructure>.Serialize(rawData);

            //if (discStrt != null)
            //{
            //  if (!string.IsNullOrEmpty(discStrt.Fields.DateNP))
            //  {
            //    DateTime date = DateTime.MinValue;

            //    if (DateTime.TryParseExact(discStrt.Fields.DateNP, "yyyyMMddHHmmss", new System.Globalization.CultureInfo("en-US"), System.Globalization.DateTimeStyles.AssumeLocal,
            //      out date))
            //    {
            //      discStrt.Fields.DateNP = date;
            //    }
            //  }
            //}

            return discStrt;
        }

        public static string Deserialize(MasterCustomerStructure strt)
        {
            return StructureBase<MasterCustomerStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterCustomerStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class MasterCustomerStructureFields
    {
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

        [XmlAttribute(AttributeName = "Keterangan")]
        public string Keterangan
        { get; set; }

        [XmlAttribute(AttributeName = "CustomerID")]
        public string CustomerID
        { get; set; }

        [XmlAttribute(AttributeName = "Nama")]
        public string Nama
        { get; set; }

        [XmlAttribute(AttributeName = "Account")]
        public string Account
        { get; set; }

        [XmlAttribute(AttributeName = "Addr1")]
        public string Addr1
        { get; set; }

        [XmlAttribute(AttributeName = "Addr2")]
        public string Addr2
        { get; set; }

        [XmlAttribute(AttributeName = "AddrTax1")]
        public string AddrTax1
        { get; set; }

        [XmlAttribute(AttributeName = "AddrTax2")]
        public string AddrTax2
        { get; set; }

        [XmlAttribute(AttributeName = "Fax")]
        public string Fax
        { get; set; }

        [XmlAttribute(AttributeName = "Fee")]
        public decimal Fee
        { get; set; }

        [XmlAttribute(AttributeName = "Kodecab")]
        public string Kodecab
        { get; set; }

        [XmlAttribute(AttributeName = "Kota")]
        public string Kota
        { get; set; }

        [XmlAttribute(AttributeName = "KotaTax")]
        public string KotaTax
        { get; set; }

        [XmlAttribute(AttributeName = "Limit")]
        public decimal Limit
        { get; set; }

        [XmlAttribute(AttributeName = "NmBank")]
        public string NmBank
        { get; set; }

        [XmlAttribute(AttributeName = "SpDays")]
        public string SpDays
        { get; set; }

        [XmlAttribute(AttributeName = "SpDaysInternal")]
        public string SpDaysInternal
        { get; set; }

        [XmlAttribute(AttributeName = "SpDaysEksternal")]
        public string SpDaysEksternal
        { get; set; }

        [XmlAttribute(AttributeName = "NPKP")]
        public string NPKP
        { get; set; }

        [XmlAttribute(AttributeName = "NPWP")]
        public string NPWP
        { get; set; }

        [XmlAttribute(AttributeName = "Gudang")]
        public string Gudang
        { get; set; }

        [XmlAttribute(AttributeName = "Pemilik")]
        public string Pemilik
        { get; set; }

        [XmlAttribute(AttributeName = "KdPos")]
        public string KdPos
        { get; set; }

        [XmlAttribute(AttributeName = "KdPosTax")]
        public string KdPosTax
        { get; set; }

        [XmlAttribute(AttributeName = "Tagih")]
        public string Tagih
        { get; set; }

        [XmlAttribute(AttributeName = "TaxName")]
        public string TaxName
        { get; set; }

        [XmlAttribute(AttributeName = "Telp1")]
        public string Telp1
        { get; set; }

        [XmlAttribute(AttributeName = "Telp2")]
        public string Telp2
        { get; set; }

        [XmlAttribute(AttributeName = "TOP")]
        public byte TOP
        { get; set; }

        [XmlAttribute(AttributeName = "TOPPjg")]
        public byte TOPPjg
        { get; set; }

        [XmlAttribute(AttributeName = "Sektor")]
        public string Sektor
        { get; set; }

        [XmlAttribute(AttributeName = "isAkses")]
        public bool isAkses
        { get; set; }

        [XmlAttribute(AttributeName = "isCabang")]
        public bool isCabang
        { get; set; }

        [XmlAttribute(AttributeName = "isDispen")]
        public bool isDispen
        { get; set; }

        [XmlAttribute(AttributeName = "isHide")]
        public bool isHide
        { get; set; }

        [XmlAttribute(AttributeName = "isMaterai")]
        public bool isMaterai
        { get; set; }

        [XmlAttribute(AttributeName = "isStatus")]
        public bool isStatus
        { get; set; }

        [XmlAttribute(AttributeName = "DateNP")]
        public string DateNP
        { get; set; }

        [XmlAttribute(AttributeName = "DateOpen")]
        public string DateOpen
        { get; set; }

        [XmlAttribute(AttributeName = "DateClose")]
        public string DateClose
        { get; set; }
    }
    #endregion

    #region Expedisi

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class MasterExpedisiStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MasterExpedisiStructure Serialize(string rawData)
        {
            return StructureBase<MasterExpedisiStructure>.Serialize(rawData);
        }

        public static string Deserialize(MasterExpedisiStructure strt)
        {
            return StructureBase<MasterExpedisiStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterExpedisiStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class MasterExpedisiStructureFields
    {
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

        [XmlAttribute(AttributeName = "ExpID")]
        public string ExpID
        { get; set; }

        [XmlAttribute(AttributeName = "Nama")]
        public string Nama
        { get; set; }

        [XmlAttribute(AttributeName = "isAktif")]
        public bool isAktif
        { get; set; }

        [XmlAttribute(AttributeName = "isUdara")]
        public bool isUdara
        { get; set; }

        [XmlAttribute(AttributeName = "isDarat")]
        public bool isDarat
        { get; set; }

        [XmlAttribute(AttributeName = "isLaut")]
        public bool isLaut
        { get; set; }

        [XmlAttribute(AttributeName = "isImport")]
        public bool isImport
        { get; set; }

        [XmlAttribute(AttributeName = "isNpwp")]
        public bool isNpwp
        { get; set; }

        [XmlAttribute(AttributeName = "Npwp")]
        public string Npwp
        { get; set; }
    }

    #endregion
    //
    #region Master Cabang Hari

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class MasterCabangHariStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MasterCabangHariStructure Serialize(string rawData)
        {
            return StructureBase<MasterCabangHariStructure>.Serialize(rawData);
        }

        public static string Deserialize(MasterCabangHariStructure strt)
        {
            return StructureBase<MasterCabangHariStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterCabangHariStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class MasterCabangHariStructureFields
    {
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

        [XmlAttribute(AttributeName = "ExpID")]
        public string ExpID
        { get; set; }

        [XmlAttribute(AttributeName = "Nama")]
        public string Nama
        { get; set; }

        [XmlAttribute(AttributeName = "issenin")]
        public bool issenin
        { get; set; }

        [XmlAttribute(AttributeName = "isselasa")]
        public bool isselasa
        { get; set; }

        [XmlAttribute(AttributeName = "israbu")]
        public bool israbu
        { get; set; }

        [XmlAttribute(AttributeName = "iskamis")]
        public bool iskamis
        { get; set; }

        [XmlAttribute(AttributeName = "isjumat")]
        public bool isjumat
        { get; set; }

        [XmlAttribute(AttributeName = "issabtu")]
        public bool issabtu
        { get; set; }

        //[XmlAttribute(AttributeName = "Npwp")]
        //public string Npwp
        //{ get; set; }
    }

    #endregion
    //

    #region Estimasi Ekspedisi

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class MasterExpedisiEstimasiStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MasterExpedisiEstimasiStructure Serialize(string rawData)
        {
            return StructureBase<MasterExpedisiEstimasiStructure>.Serialize(rawData);
        }

        public static string Deserialize(MasterExpedisiEstimasiStructure strt)
        {
            return StructureBase<MasterExpedisiEstimasiStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterExpedisiEstimasiStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class MasterExpedisiEstimasiStructureFields
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlAttribute(AttributeName = "ExpId")]
        public string ExpId
        { get; set; }

        [XmlAttribute(AttributeName = "Keterangan")]
        public string Keterangan
        { get; set; }

        [XmlElement(ElementName = "Field")]
        public MasterExpedisiEstimasiStructureField[] Field
        { get; set; }
    }

    [Serializable]
    public class MasterExpedisiEstimasiStructureField
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

        [XmlAttribute(AttributeName = "Customer")]
        public string Customer
        { get; set; }

        [XmlAttribute(AttributeName = "nUdara")]
        public decimal nUdara
        { get; set; }

        [XmlAttribute(AttributeName = "nDarat")]
        public decimal nDarat
        { get; set; }

        [XmlAttribute(AttributeName = "nImport")]
        public decimal nImport
        { get; set; }

        [XmlAttribute(AttributeName = "nIce")]
        public decimal nIce
        { get; set; }

        [XmlText]
        public string KeteranganMod
        { get; set; }
    }

    #endregion

    #region Estimasi Biaya Ekspedisi

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class MasterExpedisiEstimasiBiayaStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MasterExpedisiEstimasiBiayaStructure Serialize(string rawData)
        {
            //return StructureBase<MasterExpedisiEstimasiBiayaStructure>.Serialize(rawData);
            MasterExpedisiEstimasiBiayaStructure meb = StructureBase<MasterExpedisiEstimasiBiayaStructure>.Serialize(rawData);

            DateTime date = DateTime.Now;

            if (meb != null)
            {
                if ((meb.Fields != null) && (meb.Fields.Field != null) && (meb.Fields.Field.Length > 0))
                {
                    MasterExpedisiEstimasiBiayaStructureField field = null;

                    for (int nLoop = 0; nLoop < meb.Fields.Field.Length; nLoop++)
                    {
                        field = meb.Fields.Field[nLoop];

                        if (field != null)
                        {
                            if (!string.IsNullOrEmpty(field.EffectiveDate))
                            {
                                if (Functionals.DateParser(field.EffectiveDate, "yyyyMMdd", out date))
                                {
                                    field.EffectiveDateFormated = date;
                                }
                                else
                                {
                                    field.EffectiveDateFormated = DateTime.MinValue;
                                }
                            }
                        }
                    }
                }
            }

            return meb;
        }

        public static string Deserialize(MasterExpedisiEstimasiBiayaStructure strt)
        {
            return StructureBase<MasterExpedisiEstimasiBiayaStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterExpedisiEstimasiBiayaStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class MasterExpedisiEstimasiBiayaStructureFields
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlAttribute(AttributeName = "ExpId")]
        public string ExpId
        { get; set; }

        [XmlAttribute(AttributeName = "Keterangan")]
        public string Keterangan
        { get; set; }

        //[XmlAttribute(AttributeName = "Gudang")]
        //public string Gudang
        //{ get; set; }

        [XmlElement(ElementName = "Field")]
        public MasterExpedisiEstimasiBiayaStructureField[] Field
        { get; set; }
    }

    [Serializable]
    public class MasterExpedisiEstimasiBiayaStructureField
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

        [XmlAttribute(AttributeName = "Gudang")]
        public string Gudang
        { get; set; }

        [XmlAttribute(AttributeName = "Customer")]
        public string Customer
        { get; set; }

        [XmlAttribute(AttributeName = "TipeKirim")]
        public string TipeKirim
        { get; set; }

        [XmlAttribute(AttributeName = "nUdara")]
        public decimal nUdara
        { get; set; }

        [XmlAttribute(AttributeName = "nDarat")]
        public decimal nDarat
        { get; set; }

        [XmlAttribute(AttributeName = "nIce")]
        public decimal nIce
        { get; set; }

        [XmlAttribute(AttributeName = "nCdd")]
        public decimal nCdd
        { get; set; }

        [XmlAttribute(AttributeName = "nFuso")]
        public decimal nFuso
        { get; set; }

        [XmlAttribute(AttributeName = "nTronton")]
        public decimal nTronton
        { get; set; }

        [XmlAttribute(AttributeName = "nContainer")]
        public decimal nContainer
        { get; set; }

        [XmlAttribute(AttributeName = "nCde")]
        public decimal nCde
        { get; set; }

        [XmlAttribute(AttributeName = "nL300")]
        public decimal nL300
        { get; set; }

        [XmlAttribute(AttributeName = "nExpMin")]
        public decimal nExpMin
        { get; set; }

        [XmlAttribute(AttributeName = "nBiayaLain")]
        public decimal nBiayaLain
        { get; set; }

        [XmlAttribute(AttributeName = "EffectiveDate")]
        public string EffectiveDate
        { get; set; }

        [XmlIgnore]
        public DateTime EffectiveDateFormated
        { get; set; }

        [XmlText]
        public string KeteranganMod
        { get; set; }
    }

    #endregion

    #region Master Block Item

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class MasterBlockItemStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MasterBlockItemStructure Serialize(string rawData)
        {
            return StructureBase<MasterBlockItemStructure>.Serialize(rawData);
        }

        public static string Deserialize(MasterBlockItemStructure strt)
        {
            return StructureBase<MasterBlockItemStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterBlockItemStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class MasterBlockItemStructureFields
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlElement(ElementName = "Field")]
        public MasterBlockItemStructureField[] Field
        { get; set; }
    }

    [Serializable]
    public class MasterBlockItemStructureField
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "Item")]
        public string Item
        { get; set; }

        [XmlAttribute(AttributeName = "Blocked")]
        public bool IsBlocked
        { get; set; }
    }

    #endregion

    #region Master Bank

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class MasterBankStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MasterBankStructure Serialize(string rawData)
        {
            return StructureBase<MasterBankStructure>.Serialize(rawData);
        }

        public static string Deserialize(MasterBankStructure strt)
        {
            return StructureBase<MasterBankStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterBankStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class MasterBankStructureFields
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlAttribute(AttributeName = "c_bank")]
        public string c_bank
        { get; set; }

        [XmlAttribute(AttributeName = "v_bank")]
        public string v_bank
        { get; set; }

        [XmlAttribute(AttributeName = "v_bankcab")]
        public string v_bankcab
        { get; set; }

        [XmlAttribute(AttributeName = "l_aktif")]
        public bool l_aktif
        { get; set; }

        [XmlElement(ElementName = "Field")]
        public MasterBankStructureField[] Field
        { get; set; }
    }

    [Serializable]
    public class MasterBankStructureField
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

        [XmlAttribute(AttributeName = "Rekening")]
        public string Rekening
        { get; set; }

        [XmlAttribute(AttributeName = "Pemilik")]
        public string Pemilik
        { get; set; }

        [XmlAttribute(AttributeName = "Glno")]
        public string Glno
        { get; set; }

        [XmlAttribute(AttributeName = "Tipe")]
        public string Tipe
        { get; set; }

        [XmlAttribute(AttributeName = "idx")]
        public decimal idx
        { get; set; }

        [XmlText]
        public string KeteranganMod
        { get; set; }
    }

    #endregion

    #region Master Combo

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class MasterComboStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MasterComboStructure Serialize(string rawData)
        {
            return StructureBase<MasterComboStructure>.Serialize(rawData);
        }

        public static string Deserialize(MasterComboStructure strt)
        {
            return StructureBase<MasterComboStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterComboStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class MasterComboStructureFields
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public string ComboID
        { get; set; }

        [XmlElement(ElementName = "Field")]
        public MasterComboStructureField[] Field
        { get; set; }
    }

    [Serializable]
    public class MasterComboStructureField
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

    #region Master Transaksi

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class MasterTransaksiStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MasterTransaksiStructure Serialize(string rawData)
        {
            return StructureBase<MasterTransaksiStructure>.Serialize(rawData);
        }

        public static string Deserialize(MasterTransaksiStructure strt)
        {
            return StructureBase<MasterTransaksiStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterTransaksiStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class MasterTransaksiStructureFields
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlAttribute(AttributeName = "PortalID")]
        public string PortalID
        { get; set; }

        [XmlAttribute(AttributeName = "TransaksiID")]
        public string TransaksiID
        { get; set; }

        [XmlAttribute(AttributeName = "Deskripsi")]
        public string Deskripsi
        { get; set; }

        [XmlElement(ElementName = "Field")]
        public MasterTransaksiStructureField[] Field
        { get; set; }
    }

    [Serializable]
    public class MasterTransaksiStructureField
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

        [XmlAttribute(AttributeName = "TipeID")]
        public string TipeID
        { get; set; }

        [XmlText]
        public string Deskripsi
        { get; set; }
    }

    #endregion

    #region Master Item Category

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class MasterItemCategoryStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MasterItemCategoryStructure Serialize(string rawData)
        {
            return StructureBase<MasterItemCategoryStructure>.Serialize(rawData);
        }

        public static string Deserialize(MasterItemCategoryStructure strt)
        {
            return StructureBase<MasterItemCategoryStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterItemCategoryStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class MasterItemCategoryStructureFields
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public string TipeID
        { get; set; }

        [XmlAttribute(AttributeName = "Item")]
        public string Item
        { get; set; }

        [XmlElement(ElementName = "Field")]
        public MasterItemCategoryStructureField[] Field
        { get; set; }
    }

    [Serializable]
    public class MasterItemCategoryStructureField
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
    }

    #endregion

    #region Master Item Lantai

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class MasterItemLantaiStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MasterItemLantaiStructure Serialize(string rawData)
        {
            return StructureBase<MasterItemLantaiStructure>.Serialize(rawData);
        }

        public static string Deserialize(MasterItemLantaiStructure strt)
        {
            return StructureBase<MasterItemLantaiStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterItemLantaiStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class MasterItemLantaiStructureFields
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlAttribute(AttributeName = "Gudang")]
        public string Gudang
        { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public string TipeID
        { get; set; }

        [XmlAttribute(AttributeName = "Item")]
        public string Item
        { get; set; }

        [XmlElement(ElementName = "Field")]
        public MasterItemLantaiStructureField[] Field
        { get; set; }
    }

    [Serializable]
    public class MasterItemLantaiStructureField
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
    }

    #endregion

    #region Master Div AMS

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class MasterDivAMSItemStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MasterDivAMSItemStructure Serialize(string rawData)
        {
            return StructureBase<MasterDivAMSItemStructure>.Serialize(rawData);
        }

        public static string Deserialize(MasterDivAMSItemStructure strt)
        {
            return StructureBase<MasterDivAMSItemStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterDivAMSItemStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class MasterDivAMSItemStructureFields
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlAttribute(AttributeName = "DivAMSID")]
        public string DivID
        { get; set; }

        [XmlAttribute(AttributeName = "NamaDiv")]
        public string NamaDiv
        { get; set; }

        [XmlAttribute(AttributeName = "Aktif")]
        public bool Aktif
        { get; set; }

        [XmlAttribute(AttributeName = "Hide")]
        public bool Hide
        { get; set; }

        [XmlAttribute(AttributeName = "Keterangan")]
        public string Keterangan
        { get; set; }

        [XmlElement(ElementName = "Field")]
        public MasterDivAMSItemStructureField[] Field
        { get; set; }
    }

    [Serializable]
    public class MasterDivAMSItemStructureField
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

        [XmlText]
        public string KeteranganMod
        { get; set; }
    }

    #endregion

    #region Master USER APJ

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class MasterUserApjStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MasterUserApjStructure Serialize(string rawData)
        {
            return StructureBase<MasterUserApjStructure>.Serialize(rawData);
        }

        public static string Deserialize(MasterUserApjStructure strt)
        {
            return StructureBase<MasterUserApjStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterUserApjStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class MasterUserApjStructureFields
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlAttribute(AttributeName = "NipID")]
        public string NipID
        { get; set; }

        [XmlAttribute(AttributeName = "Gudang")]
        public string Gudang
        { get; set; }

        [XmlAttribute(AttributeName = "Nip")]
        public string Nip
        { get; set; }

        [XmlAttribute(AttributeName = "CusNo")]
        public string CusNo
        { get; set; }

        [XmlAttribute(AttributeName = "Nama")]
        public string Nama
        { get; set; }

        [XmlAttribute(AttributeName = "NoPbf")]
        public string NoPbf
        { get; set; }

        [XmlAttribute(AttributeName = "NoSik")]
        public string NoSik
        { get; set; }

        [XmlAttribute(AttributeName = "KodeArea")]
        public string KodeArea
        { get; set; }

        [XmlAttribute(AttributeName = "ImagePath")]
        public string ImagePath
        { get; set; }
    }

    #endregion


    #region Master Item Via

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class MasterItemViaStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MasterItemViaStructure Serialize(string rawData)
        {
            return StructureBase<MasterItemViaStructure>.Serialize(rawData);
        }

        public static string Deserialize(MasterItemViaStructure strt)
        {
            return StructureBase<MasterItemViaStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterItemViaStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class MasterItemViaStructureFields
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public string TipeID
        { get; set; }

        [XmlAttribute(AttributeName = "idx")]
        public string idx
        { get; set; }

        [XmlAttribute(AttributeName = "Keterangan")]
        public string Keterangan
        { get; set; }

        [XmlAttribute(AttributeName = "Cusno")]
        public string Cusno
        { get; set; }

        [XmlAttribute(AttributeName = "Gdg")]
        public string Gdg
        { get; set; }



        [XmlElement(ElementName = "Field")]
        public MasterItemViaStructureField[] Field
        { get; set; }
    }

    [Serializable]
    public class MasterItemViaStructureField
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

        [XmlAttribute(AttributeName = "Item")]
        public string Item
        { get; set; }

        [XmlAttribute(AttributeName = "Via")]
        public string Via
        { get; set; }


    }

    #endregion

    #region Master Driver

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class MasterDriverStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MasterDriverStructure Serialize(string rawData)
        {
            return StructureBase<MasterDriverStructure>.Serialize(rawData);
        }

        public static string Deserialize(MasterDriverStructure strt)
        {
            return StructureBase<MasterDriverStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterDriverStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class MasterDriverStructureFields
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlAttribute(AttributeName = "Keterangan")]
        public string Keterangan
        { get; set; }

        [XmlAttribute(AttributeName = "Nip")]
        public string Nip
        { get; set; }

        [XmlAttribute(AttributeName = "Nama")]
        public string Nama
        { get; set; }

        [XmlAttribute(AttributeName = "Nopol")]
        public string Nopol
        { get; set; }

        [XmlAttribute(AttributeName = "Tipe")]
        public string Tipe
        { get; set; }

        [XmlAttribute(AttributeName = "Aktif")]
        public bool Aktif
        { get; set; }

        [XmlElement(ElementName = "Field")]
        public MasterDriverStructureField[] Field
        { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public string TipeID
        { get; set; }
    }

    [Serializable]
    public class MasterDriverStructureField
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlAttribute(AttributeName = "Keterangan")]
        public string Keterangan
        { get; set; }

        [XmlAttribute(AttributeName = "Nip")]
        public string Nip
        { get; set; }

        [XmlAttribute(AttributeName = "Nama")]
        public string Nama
        { get; set; }

        [XmlAttribute(AttributeName = "Nopol")]
        public string Nopol
        { get; set; }

        [XmlAttribute(AttributeName = "Tipe")]
        public string Tipe
        { get; set; }

        [XmlAttribute(AttributeName = "Aktif")]
        public bool Aktif
        { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public string TipeID
        { get; set; }
    }
    #endregion

    #region Master PKP

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class MasterPKPStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MasterPKPStructure Serialize(string rawData)
        {
            //return StructureBase<MasterPKPStructure>.Serialize(rawData);

            MasterPKPStructure fm = StructureBase<MasterPKPStructure>.Serialize(rawData);

            if (fm != null)
            {
                if (fm.Fields != null)
                {
                    DateTime date = DateTime.Now;

                    if (!string.IsNullOrEmpty(fm.Fields.nppkpdate))
                    {
                        if (Functionals.DateParser(fm.Fields.nppkpdate, "yyyyMMddHHmmssfff", out date))
                        {
                            fm.Fields.nppkpdateC = date;
                        }
                        else
                        {
                            fm.Fields.nppkpdateC = Functionals.StandardSqlDateTime;
                        }
                    }
                }
            }

            return fm;
        }

        public static string Deserialize(MasterPKPStructure strt)
        {
            return StructureBase<MasterPKPStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterPKPStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class MasterPKPStructureFields
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlAttribute(AttributeName = "pkpno")]
        public string pkpno
        { get; set; }

        [XmlAttribute(AttributeName = "pkpname")]
        public string pkpname
        { get; set; }

        [XmlAttribute(AttributeName = "npwp")]
        public string npwp
        { get; set; }

        [XmlAttribute(AttributeName = "nppkp")]
        public string nppkp
        { get; set; }

        [XmlAttribute(AttributeName = "nppkpdate")]
        public string nppkpdate
        { get; set; }

        [XmlIgnore]
        public DateTime nppkpdateC
        { get; set; }

        [XmlAttribute(AttributeName = "alamat1")]
        public string alamat1
        { get; set; }

        [XmlAttribute(AttributeName = "alamat2")]
        public string alamat2
        { get; set; }

        [XmlAttribute(AttributeName = "telepon1")]
        public string telepon1
        { get; set; }

        [XmlAttribute(AttributeName = "fax1")]
        public string fax1
        { get; set; }

        [XmlAttribute(AttributeName = "fax2")]
        public string fax2
        { get; set; }

        [XmlAttribute(AttributeName = "isAktif")]
        public bool isAktif
        { get; set; }
    }
    #endregion

    #region Master Nomor Pajak

    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class MasterNomorPajakStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MasterNomorPajakStructure Serialize(string rawData)
        {
            return StructureBase<MasterNomorPajakStructure>.Serialize(rawData);
        }

        public static string Deserialize(MasterNomorPajakStructure strt)
        {
            return StructureBase<MasterNomorPajakStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MasterNomorPajakStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class MasterNomorPajakStructureFields
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlAttribute(AttributeName = "idx")]
        public string idx
        { get; set; }

        [XmlAttribute(AttributeName = "tahun")]
        public string tahun
        { get; set; }

        [XmlAttribute(AttributeName = "digit1")]
        public string digit1
        { get; set; }

        [XmlAttribute(AttributeName = "digit2")]
        public string digit2
        { get; set; }

        [XmlAttribute(AttributeName = "awal")]
        public string awal
        { get; set; }

        [XmlAttribute(AttributeName = "akhir")]
        public string akhir
        { get; set; }

        [XmlAttribute(AttributeName = "current")]
        public string current
        { get; set; }

        [XmlElement(ElementName = "Field")]
        public MasterNomorPajakStructureField[] Field
        { get; set; }
    }

    [Serializable]
    public class MasterNomorPajakStructureField
    {

    }
    #endregion
}
