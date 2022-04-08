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
    public class ReturCustomerStructureRS
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static ReturCustomerStructureRS Serialize(string rawData)
        {
            ReturCustomerStructureRS rcs = StructureBase<ReturCustomerStructureRS>.Serialize(rawData);

            if ((rcs.Fields != null) && (rcs.Fields.Field != null) && (rcs.Fields.Field.Length > 0))
            {
                ReturCustomerStructureRSField field = null;

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

        public static string Deserialize(ReturCustomerStructureRS strt)
        {
            return StructureBase<ReturCustomerStructureRS>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public ReturCustomerStructureRSFields Fields
        { get; set; }
    }

    [Serializable]
    public class ReturCustomerStructureRSFields
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public string ReturCustomerID
        { get; set; }

        [XmlAttribute(AttributeName = "Gudang")]
        public string Gudang
        { get; set; }

        [XmlAttribute(AttributeName = "Customer")]
        public string Customer
        { get; set; }

        [XmlAttribute(AttributeName = "PBBR")]
        public string PBBR
        { get; set; }

        [XmlAttribute(AttributeName = "Keterangan")]
        public string Keterangan
        { get; set; }

        [XmlAttribute(AttributeName = "ConfirmSent")]
        public bool ConfirmedSent
        { get; set; }

        [XmlAttribute(AttributeName = "BaspbNo")]
        public string BaspbNo
        { get; set; }

        [XmlElement(ElementName = "Field")]
        public ReturCustomerStructureRSField[] Field
        { get; set; }
    }

    [Serializable]
    public class ReturCustomerStructureRSField
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

        [XmlAttribute(AttributeName = "NoRN")]
        public string NoRN
        { get; set; }

        [XmlAttribute(AttributeName = "TypeItem")]
        public string TypeItem
        { get; set; }

        [XmlAttribute(AttributeName = "Qty")]
        public decimal Quantity
        { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public string RCid
        { get; set; }

        [XmlAttribute(AttributeName = "gudang")]
        public string Gudang
        { get; set; }

        [XmlText]
        public string Keterangan
        { get; set; }

        [XmlAttribute(AttributeName = "DcoreSend")]
        public bool IsDcoreSend
        { get; set; }

        [XmlAttribute(AttributeName = "PBId")]
        public string PBId
        { get; set; }

    }

    #region JSON Convert

    [Serializable]
    public class ReturCustomerRSResponse
    {
        public string ID
        { get; set; }

        public string RC
        { get; set; }

        public string USER
        { get; set; }

        public DateTime TanggalRC { get; set; }

        public string TanggalRC_Str { get; set; }

        public ReturCustomerStructureRSField[] Fields
        { get; set; }

        public static ReturCustomerRSResponse Deserialize(string rawData)
        {
            return StructureBase<ReturCustomerRSResponse>.SerializeJson(rawData);
        }

        public static string Serialize(ReturCustomerRSResponse strt)
        {
            return StructureBase<ReturCustomerRSResponse>.DeserializeJson(strt);
        }
    }

    [Serializable]
    public class ReturCustomerRSJSONStructure
    {
        public string ID
        { get; set; }

        public string RC
        { get; set; }

        public static ReturCustomerRSJSONStructure Deserialize(string rawData)
        {
            return StructureBase<ReturCustomerRSJSONStructure>.SerializeJson(rawData);
        }

        public static string Serialize(ReturCustomerRSJSONStructure strt)
        {
            return StructureBase<ReturCustomerRSJSONStructure>.DeserializeJson(strt);
        }

        public ReturCustomerRSJSONStructureField[] Fields
        { get; set; }
    }

    [Serializable]
    public class ReturCustomerRSJSONStructureField
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
