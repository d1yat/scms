/*
 * Created By Indra
 * 20171231FM
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
  public class StockOpnameStructure
  {
      [XmlAttribute(AttributeName = "name")]
      public string Name
      { get; set; }

      [XmlAttribute(AttributeName = "method")]
      public string Method
      { get; set; }

      public static StockOpnameStructure Serialize(string rawData)
      {
          StockOpnameStructure sps = StructureBase<StockOpnameStructure>.Serialize(rawData);

          DateTime date = DateTime.Now;

          if (sps != null)
          {
              if ((sps.Fields != null) && (sps.Fields.Field != null) && (sps.Fields.Field.Length > 0))
              {
                  StockOpnameStructureField field = null;

                  for (int nLoop = 0; nLoop < sps.Fields.Field.Length; nLoop++)
                  {
                      field = sps.Fields.Field[nLoop];

                      if (field != null)
                      {
                          if (!string.IsNullOrEmpty(field.Expired))
                          {
                              if (Functionals.DateParser(field.Expired, "yyyyMMdd", out date))
                              {
                                  field.ExpiredDateFormated = date;
                              }
                              else
                              {
                                  field.ExpiredDateFormated = DateTime.MinValue;
                              }
                          }
                      }
                  }
              }
          }
          return sps;
      }

      public static string Deserialize(StockOpnameStructure strt)
      {
          return StructureBase<StockOpnameStructure>.Deserialize(strt);
      }
       
      [XmlElement(ElementName = "Fields")]
      public StockOpnameStructureFields Fields
      { get; set; }

  }

  [Serializable]
  public class StockOpnameStructureFields
  {
      [XmlElement(ElementName = "Field")]
      public StockOpnameStructureField[] Field
      { get; set; }

      [XmlAttribute(AttributeName = "Entry")]
      public string Entry
      { get; set; }

      [XmlAttribute(AttributeName = "Gudang")]
      public string Gudang
      { get; set; }

      [XmlAttribute(AttributeName = "BuatForm")]
      public string BuatForm
      { get; set; }

      [XmlAttribute(AttributeName = "Principal")]
      public string Principal
      { get; set; }

      [XmlAttribute(AttributeName = "DivPrincipal")]
      public string DivPrincipal
      { get; set; }

      [XmlAttribute(AttributeName = "Kategori")]
      public string Kategori
      { get; set; }

      [XmlAttribute(AttributeName = "Item")]
      public string Item
      { get; set; }

      [XmlAttribute(AttributeName = "Status")]
      public string Status
      { get; set; }

      [XmlAttribute(AttributeName = "Batch")]
      public string Batch
      { get; set; }

      [XmlAttribute(AttributeName = "Expired")]
      public string Expired
      { get; set; }

      [XmlIgnore]
      public DateTime ExpiredDateFormated
      { get; set; }

      [XmlAttribute(AttributeName = "Noform")]
      public string Noform
      { get; set; }
  }

  [Serializable]
  public class StockOpnameStructureField
  {
      [XmlAttribute(AttributeName = "Entry")]
      public string Entry
      { get; set; }

      [XmlAttribute(AttributeName = "Gudang")]
      public string Gudang
      { get; set; }

      [XmlAttribute(AttributeName = "KdPrincipal")]
      public string KdPrincipal
      { get; set; }

      [XmlAttribute(AttributeName = "Principal")]
      public string Principal
      { get; set; }

      [XmlAttribute(AttributeName = "KdDivPrincipal")]
      public string KdDivPrincipal
      { get; set; }

      [XmlAttribute(AttributeName = "DivPrincipal")]
      public string DivPrincipal
      { get; set; }

      [XmlAttribute(AttributeName = "Location")]
      public string Location
      { get; set; }

      [XmlAttribute(AttributeName = "KdBarang")]
      public string KdBarang
      { get; set; }

      [XmlAttribute(AttributeName = "NmBarang")]
      public string NmBarang
      { get; set; }

      [XmlAttribute(AttributeName = "StBarang")]
      public string StBarang
      { get; set; }

      [XmlAttribute(AttributeName = "Batch")]
      public string Batch
      { get; set; }

      [XmlAttribute(AttributeName = "QtySys")]
      public decimal QtySys
      { get; set; }

      [XmlAttribute(AttributeName = "SOQty")]
      public decimal SOQty
      { get; set; }

      [XmlAttribute(AttributeName = "Recount1")]
      public decimal Recount1
      { get; set; }

      [XmlAttribute(AttributeName = "Recount2")]
      public decimal Recount2
      { get; set; }

      [XmlAttribute(AttributeName = "Selisih")]
      public decimal Selisih
      { get; set; }

      [XmlAttribute(AttributeName = "Expired")]
      public string Expired
      { get; set; }

      [XmlIgnore]
      public DateTime ExpiredDateFormated
      { get; set; }

      [XmlAttribute(AttributeName = "Box")]
      public decimal Box
      { get; set; }

      [XmlAttribute(AttributeName = "Stage")]
      public string Stage
      { get; set; }
  }
}
