using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Components;

namespace ScmsSoaLibrary.Parser.Class
{
    [Serializable]
    [XmlRoot(ElementName = "Structure")]
    public class DOPharmanetStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static DOPharmanetStructure Serialize(string rawData)
        {
            DOPharmanetStructure DOPH = StructureBase<DOPharmanetStructure>.Serialize(rawData);

            if (DOPH != null)
            {
                if ((DOPH.Fields != null) && (!string.IsNullOrEmpty(DOPH.Fields.Tanggal)))
                {
                    DateTime date = DateTime.Now;

                    if (Functionals.DateParser(DOPH.Fields.Tanggal, "yyyyMMddHHmmssfff", out date))
                    {
                        DOPH.Fields.TanggalSP = date;
                    }
                    else
                    {
                        DOPH.Fields.TanggalSP = DateTime.MinValue;
                    }
                }
            }

            return DOPH;
        }

        public static string Deserialize(DOPharmanetStructure strt)
        {
            return StructureBase<DOPharmanetStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public DOPharmanetStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class DOPharmanetStructureFields
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public string DOPharmanetID
        { get; set; }

        [XmlAttribute(AttributeName = "Customer")]
        public string Customer
        { get; set; }

        [XmlAttribute(AttributeName = "Tanggal")]
        public string Tanggal
        { get; set; }

        [XmlIgnore]
        public DateTime TanggalSP
        { get; set; }

        [XmlAttribute(AttributeName = "SPCabang")]
        public string SPCabang
        { get; set; }

        [XmlAttribute(AttributeName = "Keterangan")]
        public string Keterangan
        { get; set; }

        [XmlAttribute(AttributeName = "Cek")]
        public bool Cek
        { get; set; }

        [XmlAttribute(AttributeName = "Tipe")]
        public string TipeSP
        { get; set; }

        [XmlElement(ElementName = "Field")]
        public DOPharmanetStructureField[] Field
        { get; set; }

    }

    [Serializable]
    public class DOPharmanetStructureField
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

        [XmlAttribute(AttributeName = "Keterangan")]
        public string KeteranganEditing
        { get; set; }

        [XmlText]
        public string Keterangan
        { get; set; }

        [XmlAttribute(AttributeName = "NomorSP")]
        public string NomorSP
        { get; set; }

        [XmlAttribute(AttributeName = "StatusSP")]
        public string StatusSP
        { get; set; }
    }

    #region JSON Convert

    [Serializable]
    public class DOPharmanetResponse
    {
        public string ID { get; set; }

        public DateTime TanggalSP { get; set; }

        public string TanggalSP_Str { get; set; }

        public string C_SPNO { get; set; }

        public string Cabang { get; set; }

        public string TipeSP { get; set; }

        public static DOPharmanetResponse Deserialize(string rawData)
        {
            return StructureBase<DOPharmanetResponse>.SerializeJson(rawData);
        }

        public static string Serialize(DOPharmanetResponse strt)
        {
            return StructureBase<DOPharmanetResponse>.DeserializeJson(strt);
        }

        public DOPharmanetJSONStructureField[] Fields
        { get; set; }
    }

    [Serializable]
    public class DOPharmanetJSONStructureField
    {
        public string C_ITENO
        { get; set; }

        public decimal Qty
        { get; set; }

        public decimal Acc
        { get; set; }

        public decimal Batch
        { get; set; }

        public string Bacthditerima
        { get; set; }

        public string PoOutlet
        { get; set; }
    }

    #endregion
}
