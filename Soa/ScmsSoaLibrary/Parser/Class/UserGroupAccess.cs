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
  [XmlRoot(ElementName="Structure")]
  public class UserGroupAccessStructure
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name
    { get; set; }

    [XmlAttribute(AttributeName = "method")]
    public string Method
    { get; set; }
    
    [XmlIgnore]
    public bool IsGroupMode
    { get; private set; }

    public static UserGroupAccessStructure Serialize(string rawData)
    {
      UserGroupAccessStructure strt = StructureBase<UserGroupAccessStructure>.Serialize(rawData);

      if (strt != null)
      {
        strt.IsGroupMode = ((strt.Fields != null) &&
          (!string.IsNullOrEmpty(strt.Fields.Group)) ? true : false);
      }

      return strt;
    }

    public static string Deserialize(UserGroupAccessStructure strt)
    {
      return StructureBase<UserGroupAccessStructure>.Deserialize(strt);
    }

    [XmlElement(ElementName = "Fields")]
    public UserGroupAccessStructureFields Fields
    { get; set; }
  }

  [Serializable]
  public class UserGroupAccessStructureFields
  {
    [XmlAttribute(AttributeName = "Nip")]
    public string Nip
    { get; set; }

    [XmlAttribute(AttributeName = "Group")]
    public string Group
    { get; set; }

    [XmlAttribute(AttributeName = "Entry")]
    public string Entry
    { get; set; }

    [XmlElement(ElementName = "Field")]
    public UserGroupAccessStructureField[] Field
    { get; set; }
  }

  [Serializable]
  public class UserGroupAccessStructureField
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

    [XmlText]
    public string Value
    { get; set; }
  }
}
