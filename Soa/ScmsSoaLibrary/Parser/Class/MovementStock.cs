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
    public class MovementStockStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static MovementStockStructure Serialize(string rawData)
        {
            MovementStockStructure rcs = StructureBase<MovementStockStructure>.Serialize(rawData);

            if ((rcs.Fields != null) && (rcs.Fields.Field != null) && (rcs.Fields.Field.Length > 0))
            {
                MovementStockStructureField field = null;

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

        public static string Deserialize(MovementStockStructure strt)
        {
            return StructureBase<MovementStockStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public MovementStockStructureFields Fields
        { get; set; }
    }

    [Serializable]
    public class MovementStockStructureFields
    {
        [XmlAttribute(AttributeName = "Entry")]
        public string Entry
        { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public string STID
        { get; set; }

        [XmlAttribute(AttributeName = "Gudang")]
        public string Gudang
        { get; set; }

        [XmlAttribute(AttributeName = "Confirm")]
        public string Confirm
        { get; set; }

        [XmlElement(ElementName = "Field")]
        public MovementStockStructureField[] Field
        { get; set; }

    }

    [Serializable]
    public class MovementStockStructureField
    {
        [XmlAttribute(AttributeName = "c_iteno")]
        public string Iteno
        { get; set; }

        [XmlAttribute(AttributeName = "c_batch")]
        public string Batch
        { get; set; }

        [XmlAttribute(AttributeName = "n_qtyterima")]
        public string QtyTerima
        { get; set; }

        [XmlAttribute(AttributeName = "n_qtyreject")]
        public string QtyReject
        { get; set; }
    }

}
