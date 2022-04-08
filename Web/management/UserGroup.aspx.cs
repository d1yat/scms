using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using System.Text;

public partial class management_UserGroup : Scms.Web.Core.PageHandler
{
  private const string BY_USER = "byUser";
  private const string BY_GROUP = "byGroup";
  
  private PostDataParser.StructureResponse SaveParser(string cNip, bool isByuser, Dictionary<string, string>[] dics)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = cNip;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string tmp = null;
    bool isNew = false,
      isDelete = false;
    string varData = null;

    if (isByuser)
    {
      #region By User

      dic.Add("Nip", pair);
      pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);

      for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
      {
        tmp = nLoop.ToString();

        dicData = dics[nLoop];

        if (dicData.ContainsKey("l_new"))
        {
          isNew = dicData.GetValueParser<bool>("l_new");
        }
        if (dicData.ContainsKey("l_delete"))
        {
          isDelete = dicData.GetValueParser<bool>("l_delete");
        }

        if ((dicData.ContainsKey("c_group") && (!pair.DicValues.ContainsKey(tmp)))
          && ((isNew && (!isDelete)) || ((!isNew) && isDelete)))
        {
          dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

          dicAttr.Add("New", isNew.ToString().ToLower());
          dicAttr.Add("Delete", isDelete.ToString().ToLower());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = dicData["c_group"],
            DicAttributeValues = dicAttr
          });
        }

        dicData.Clear();
      }

      try
      {
        varData = parser.ParserData("UserGroupAccess", "Modify", dic);
      }
      catch (Exception ex)
      {
        Scms.Web.Common.Logger.WriteLine("management_Groups SaveParser_UserGroupAccess : {0} ", ex.Message);
      }

      #endregion
    }
    else
    {
      #region By Group

      dic.Add("Group", pair);
      pair.DicAttributeValues.Add("Entry", this.Nip);

      for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
      {
        tmp = nLoop.ToString();

        dicData = dics[nLoop];

        if (dicData.ContainsKey("l_new"))
        {
          isNew = dicData.GetValueParser<bool>("l_new");
        }
        if (dicData.ContainsKey("l_delete"))
        {
          isDelete = dicData.GetValueParser<bool>("l_delete");
        }

        if ((dicData.ContainsKey("c_nip") && (!pair.DicValues.ContainsKey(tmp)))
          && ((isNew && (!isDelete)) || ((!isNew) && isDelete)))
        {
          dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

          dicAttr.Add("New", isNew.ToString().ToLower());
          dicAttr.Add("Delete", isDelete.ToString().ToLower());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = dicData["c_nip"],
            DicAttributeValues = dicAttr
          });
        }

        dicData.Clear();
      }

      try
      {
        varData = parser.ParserData("GroupUserAccess", "Modify", dic);
      }
      catch (Exception ex)
      {
        Scms.Web.Common.Logger.WriteLine("management_Groups SaveParser_GroupUserAccess : {0} ", ex.Message);
      }

      #endregion
    }

    string result = null;

    if (!string.IsNullOrEmpty(varData))
    {
      Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

      result = soa.PostData(varData);

      responseResult = parser.ResponseParser(result);
    }

    return responseResult;
  }

  //protected void Page_Init(object sender, EventArgs e)
  //{
  //  if (!this.IsPostBack)
  //  {
  //    Ext.Net.Store store = gridUserMain.GetStore();
  //    if (store != null)
  //    {
  //      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
  //    }
  //  }
  //}

  protected void Page_Load(object sender, EventArgs e)
  {

  }
  
  protected void saveGridEntry(object sender, DirectEventArgs e)
  {
    string jsonGridValues = (e.ExtraParams["GridValues"] ?? string.Empty);
    string activeId = (e.ExtraParams["ActiveID"] ?? string.Empty);
    string cData = null;
    bool isGroup = false;

    PostDataParser.StructureResponse respon = default(PostDataParser.StructureResponse);

    Dictionary<string, string>[] gridDataAccess = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    if (activeId.Equals(pnlByUser.ClientID, StringComparison.OrdinalIgnoreCase))
    {
      cData = hfUserID.Text;

      respon = SaveParser(cData, true, gridDataAccess);
    }
    else if (activeId.Equals(pnlByGroup.ClientID, StringComparison.OrdinalIgnoreCase))
    {
      cData = hfGroupID.Text;

      respon = SaveParser(cData, false, gridDataAccess);

      isGroup = true;
    }
    else
    {
      e.Success = false;
      e.ErrorMessage = "Unknown command id";
    }

    for (int nLoop = 0, nLen = gridDataAccess.Length; nLoop < nLen; nLoop++)
    {
      gridDataAccess[nLoop].Clear();
    }

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;

        if (isGroup)
        {
          scrpt = string.Format("{0}.removeAll();{0}.reload();{1}.removeAll();{1}.reload();",
           cbUser.GetStore().ClientID, gridGroupDetail.GetStore().ClientID);

          X.AddScript(scrpt);
        }
        else
        {
          scrpt = string.Format("{0}.removeAll();{0}.reload();{1}.removeAll();{1}.reload();",
           cbGroup.GetStore().ClientID, gridUserDetail.GetStore().ClientID);

          X.AddScript(scrpt);
        }

        Functional.ShowMsgInformation("Data berhasil tersimpan.");
      }
      else
      {
        e.ErrorMessage = respon.Message;

        e.Success = false;
      }
    }
    else
    {
      e.ErrorMessage = "Unknown response";

      e.Success = false;
    }
  }
}
