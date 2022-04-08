/*
 * Created By Indra
 * 20190411FM
 * 
*/

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
    public class ProdukKosongStructure
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name
        { get; set; }

        [XmlAttribute(AttributeName = "method")]
        public string Method
        { get; set; }

        public static ProdukKosongStructure Serialize(string rawData)
        {
            ProdukKosongStructure discStrt = StructureBase<ProdukKosongStructure>.Serialize(rawData);

            DateTime date = DateTime.Now;

            if (discStrt != null)
            {
                if (discStrt.Fields != null)
                {
                    if (!string.IsNullOrEmpty(discStrt.Fields.strABE))
                    {
                        if (Functionals.DateParser(discStrt.Fields.strABE, "yyyyMMdd", out date))
                        {
                            discStrt.Fields.ABE = date;
                        }
                        else
                        {
                            discStrt.Fields.ABE = DateTime.MinValue;
                        }
                    }                    
                }
            }

            return discStrt;
        }

        public static string Deserialize(ProdukKosongStructure strt)
        {
            return StructureBase<ProdukKosongStructure>.Deserialize(strt);
        }

        [XmlElement(ElementName = "Fields")]
        public ProdukKosongStructureFields Fields
        { get; set; }
    }

  [Serializable]
    public class ProdukKosongStructureFields
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
        

        #region Produk Kosong Indra 20180815FM

        [XmlAttribute(AttributeName = "Pkno")]
        public string Pkno
        { get; set; }

        [XmlAttribute(AttributeName = "Produk")]
        public string Produk
        { get; set; }

        [XmlAttribute(AttributeName = "SendEmail")]
        public bool SendEmail
        { get; set; }

        [XmlAttribute(AttributeName = "txAlasanTolakSetuju")]
        public string AlasanTolakSetuju
        { get; set; }

        [XmlAttribute(AttributeName = "strABE")]
        public string strABE
        { get; set; }

        [XmlIgnore]
        public DateTime ABE
        { get; set; }

        [XmlAttribute(AttributeName = "Aktif")]
        public bool Aktif
        { get; set; }

        [XmlAttribute(AttributeName = "Keterangan")]
        public string Keterangan
        { get; set; }

        [XmlAttribute(AttributeName = "Tipeinput")]
        public string Tipeinput
        { get; set; }

        #endregion
    }
}
