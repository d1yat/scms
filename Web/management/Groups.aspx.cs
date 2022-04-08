using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Core;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class management_Groups : Scms.Web.Core.PageHandler
{
  private const string VIEW_STATE_VASKSES = "VAKSES";

  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Group Detail";

    hfGroup.Clear();

    txGrup.Clear();
    txGrup.Disabled = false;

    txDesc.Clear();
        
    chkAktif.Checked = false;
  }

  private void PopulateDetail(string pName, string pID)
  {
    ClearEntrys();
    
    Dictionary<string, string> dicUserInfo = null;

    string tmp = null;

    #region Parser Header

    try
    {
      dicUserInfo = ReadGroupDetail(pName, pID);

      if ((dicUserInfo != null) && (dicUserInfo.Count > 0))
      {
        winDetail.Title = "Group Detail";

        hfGroup.Text = txGrup.Text = pID;
        txGrup.Disabled = true;

        txDesc.Text = (dicUserInfo.ContainsKey("v_group_desc") ? dicUserInfo["v_group_desc"] : string.Empty);

        bool isAktif = false;

        tmp = (dicUserInfo.ContainsKey("l_aktif") ? dicUserInfo["l_aktif"] : string.Empty);
        if (!bool.TryParse(tmp, out isAktif))
        {
          isAktif = (tmp.Equals("1", StringComparison.OrdinalIgnoreCase) ? true : false);
        }

        tmp = (dicUserInfo.ContainsKey("v_akses") ? dicUserInfo["v_akses"] : string.Empty);

        this.Cache[VIEW_STATE_VASKSES] = tmp;

        chkAktif.Checked = isAktif;
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("management_Groups:PopulateDetail - ", ex.Message));
    }
    finally
    {
      if (dicUserInfo != null)
      {
        dicUserInfo.Clear();
      }
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    #region Old Coded

    //Dictionary<string, object> dicUser = null;
    //Dictionary<string, string> dicUserInfo = null;
    //Newtonsoft.Json.Linq.JArray jarr = null;

    //Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    //string[][] paramX = new string[][]{
    //    new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
    //  };

    //string tmp = null;

    //#region Parser Header

    //string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "101001", paramX);

    //try
    //{
    //  dicUser = JSON.Deserialize<Dictionary<string, object>>(res);
    //  if (dicUser.ContainsKey("records") && (dicUser.ContainsKey("totalRows") && (((long)dicUser["totalRows"]) > 0)))
    //  {
    //    jarr = new Newtonsoft.Json.Linq.JArray(dicUser["records"]);

    //    dicUserInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

    //    winDetail.Title = "Group Detail";

    //    hfGroup.Text = txGrup.Text = pID;
    //    txGrup.Disabled = true;

    //    txDesc.Text = (dicUserInfo.ContainsKey("v_group_desc") ? dicUserInfo["v_group_desc"] : string.Empty);

    //    bool isAktif = false;

    //    tmp = (dicUserInfo.ContainsKey("l_aktif") ? dicUserInfo["l_aktif"] : string.Empty);
    //    if (!bool.TryParse(tmp, out isAktif))
    //    {
    //      isAktif = (tmp.Equals("1", StringComparison.OrdinalIgnoreCase) ? true : false);
    //    }

    //    chkAktif.Checked = isAktif;

    //    jarr.Clear();
    //  }
    //}
    //catch (Exception ex)
    //{
    //  System.Diagnostics.Debug.WriteLine(
    //    string.Concat("management_Groups:PopulateDetail - ", ex.Message));
    //}
    //finally
    //{
    //  if (jarr != null)
    //  {
    //    jarr.Clear();
    //  }
    //  if (dicUserInfo != null)
    //  {
    //    dicUserInfo.Clear();
    //  }
    //  if (dicUser != null)
    //  {
    //    dicUser.Clear();
    //  }
    //}

    //#endregion

    //winDetail.Hidden = false;
    //winDetail.ShowModal();

    #endregion

    GC.Collect();
  }

  private Dictionary<string, string> ReadGroupDetail(string pName, string sGroup)
  {
    Dictionary<string, object> dicUser = null;
    Dictionary<string, string> dicUserInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), sGroup, "System.String"},
        new string[] { "decrypted", bool.TrueString, "System.Boolean"}
      };

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "101001", paramX);

    try
    {
      dicUser = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicUser.ContainsKey("records") && (dicUser.ContainsKey("totalRows") && (((long)dicUser["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicUser["records"]);

        dicUserInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("management_Groups:ReadGroupDetail - ", ex.Message));
    }
    finally
    {
      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicUser != null)
      {
        dicUser.Clear();
      }
    }

    #endregion

    return dicUserInfo;
  }

  private PostDataParser.StructureResponse SaveParser(bool isAddNew, string vAkses)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    dic.Add("Group", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = txGrup.Text.Trim()
    });
    dic.Add("Description", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = txDesc.Text.Trim()
    });
    dic.Add("Akses", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = (string.IsNullOrEmpty(vAkses) ? null : vAkses)
    });
    dic.Add("Aktif", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = chkAktif.Checked.ToString()
    });
    dic.Add("User", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = "105839H"
    });

    string varData = null;

    try
    {
      varData = parser.ParserData("Group", (isAddNew ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("management_Groups SaveParser : {0} ", ex.Message);
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

  private PostDataParser.StructureResponse DeleteParser(string sGroup)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    dic.Add("Group", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = sGroup.Trim()
    });
    dic.Add("User", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = "105839H"
    });

    string varData = null;

    try
    {
      varData = parser.ParserData("Group", "Delete", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("management_Groups DeleteParser : {0} ", ex.Message);
    }

    string result = null;

    if (!string.IsNullOrEmpty(varData))
    {
      Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

      result = soa.PostData(varData);

      responseResult = parser.ResponseParser(result);
    }

    responseResult.IsSet = true;

    return responseResult;
  }

  private void RebuildAccessMenu()
  {
    RightBuilder rb = new RightBuilder();
    rb.Populate();

    Scms.Web.Core.RightBuilder.RightApplication[] rights = rb.Rights;

    Ext.Net.TreePanel tree = new Ext.Net.TreePanel();

    RightBuilder.RightApplication rapp = null;
    Ext.Net.TreeNode nodeL1 = null;
    Ext.Net.TreeNode nodeL2 = null;
    Ext.Net.TreeNode nodeL3 = null;

    List<Ext.Net.TreeNode> lstNodes = new List<Ext.Net.TreeNode>();

    Ext.Net.TreeNode rootNodes = new Ext.Net.TreeNode();
    rootNodes.Text = "Management";
    rootNodes.Icon = Icon.Briefcase;

    string tmp1, tmp2, tmp3;

    for (int nLoop = 0, nLen = rights.Length; nLoop < nLen; nLoop++)
    {
      rapp = rights[nLoop];

      nodeL1 = new Ext.Net.TreeNode(rapp.ID, rapp.Name, Icon.Application);

      if ((rapp.Modules != null) && (rapp.Modules.Count > 0))
      {
        foreach (KeyValuePair<string, RightBuilder.Module> kvpMods in rapp.Modules)
        {
          tmp1 = string.Concat(rapp.ID, "_", kvpMods.Value.ID);

          nodeL2 = new Ext.Net.TreeNode(tmp1, kvpMods.Value.Name, Icon.Package);

          if ((kvpMods.Value.Items != null) && (kvpMods.Value.Items.Count > 0))
          {
            foreach (KeyValuePair<string, RightBuilder.Item> kvpItems in kvpMods.Value.Items)
            {
              tmp2 = string.Concat(tmp1, "_", kvpItems.Value.ID);

              tmp3 = kvpItems.Value.Node;

              nodeL3 = new Ext.Net.TreeNode(tmp2, kvpItems.Value.Name, Icon.Page);

              nodeL3.Leaf = true;

              nodeL3.CustomAttributes.Add(new ConfigItem("nodeID", kvpItems.Value.ID, ParameterMode.Value));

              nodeL3.CustomAttributes.Add(new ConfigItem("subNode", tmp3, ParameterMode.Value));

              nodeL2.Nodes.Add(nodeL3);
            }
          }

          nodeL1.Nodes.Add(nodeL2);
        }
      }

      rootNodes.Nodes.Add(nodeL1);
    }

    rootNodes.Expanded = true;

    treeApp.Root.Clear();

    treeApp.Root.Add(rootNodes);
  }

  private PostDataParser.StructureResponse SaveParserGroupAkses(string sGroup, string dataNodeID, string dataNode, List<Dictionary<string, object>> list)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    Dictionary<string, string> dicGroupInfo = ReadGroupDetail("c_group", sGroup);
    RightBuilder.GroupAccess ga = null;

    if ((dicGroupInfo != null) && (dicGroupInfo.Count > 0))
    {
      string vAkses = (dicGroupInfo.ContainsKey("v_akses") ? dicGroupInfo["v_akses"] : string.Empty);
      
      RightBuilder rb = new RightBuilder();
      rb.Populate();

      ga = rb.RebuildGroupAccess(vAkses, dataNodeID, dataNode, list);

      if (ga == null)
      {
        responseResult.IsSet = true;
        responseResult.Message = string.Format("Grup '{0}' tidak dapat di update.", sGroup);
        responseResult.Response = PostDataParser.ResponseStatus.Failed;
      }
      else
      {
        vAkses = RightBuilder.GroupAccess.Deserialize(ga);

        responseResult = SaveParser(false, vAkses);

        if (responseResult.Response == PostDataParser.ResponseStatus.Success)
        {
          this.Cache[VIEW_STATE_VASKSES] = vAkses;
        }
      }
    }
    else
    {
      responseResult.IsSet = true;
      responseResult.Message = string.Format("Grup '{0}' tidak ditemukan.", sGroup);
      responseResult.Response = PostDataParser.ResponseStatus.Failed;
    }

    return responseResult;
  }

  #endregion

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!X.IsAjaxRequest)
    {
      RebuildAccessMenu();
    }
  }

  protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
  {
    ClearEntrys();

    btnShowDetailAccess.Hidden = true;

    winDetail.Hidden = false;
    winDetail.ShowModal();
  }
  
  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      btnShowDetailAccess.Hidden = false;

      PopulateDetail(pName, pID);
    }
    else if (cmd.Equals("Delete", StringComparison.OrdinalIgnoreCase))
    {
      PostDataParser.StructureResponse result = DeleteParser(pID);

      Ext.Net.Store store = gridMain.GetStore();

      switch (result.Response)
      {
        case PostDataParser.ResponseStatus.Success:
          e.Success = true;

          if (store != null)
          {

            X.AddScript(string.Format(@"var idx = {0}.findExact('c_group', '{1}');
                                    if(idx != -1) {{                                                                            
                                      {0}.removeAt(idx);
                                      {0}.commitChanges();
                                    }};", store.ClientID, pID));

          }

          Functional.ShowMsgInformation("Data berhasil terhapus.");

          break;
        case PostDataParser.ResponseStatus.Error:
          e.Success = false;
          e.ErrorMessage = result.Message;
          break;
        case PostDataParser.ResponseStatus.Failed:
          e.Success = false;
          e.ErrorMessage = result.Message;
          break;
        case PostDataParser.ResponseStatus.Unknown:
          e.Success = false;
          e.ErrorMessage = "Unknown result";
          break;
      }
    }
    else
    {
      Functional.ShowMsgError("Perintah tidak dikenal.");
    }

    GC.Collect();
  }

  protected void btnSave_OnClick(object sender, DirectEventArgs e)
  {
    string sNip = (e.ExtraParams["GROUP"] ?? string.Empty);

    bool isAddNew = (string.IsNullOrEmpty(sNip) ? true : false);

    PostDataParser.StructureResponse result = SaveParser(isAddNew, null);

    Ext.Net.Store store = gridMain.GetStore();

    switch (result.Response)
    {
      case PostDataParser.ResponseStatus.Success:
        e.Success = true;

        if (store != null)
        {

          if (isAddNew)
          {
            IDictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("c_group", txGrup.Text.Trim());
            dic.Add("v_group_desc", txDesc.Text.Trim());
            dic.Add("l_aktif", chkAktif.Checked.ToString().ToLower());
            dic.Add("totalList", "0");            

            //store.InsertRecord(-1, dic, true);
            store.AddRecord(dic, true);

            dic.Clear();
          }
          else
          {
            X.AddScript(string.Format(@"var idx = {0}.findExact('c_group', '{1}');
                                    if(idx != -1) {{
                                      var rec = {0}.getAt(idx);
                                      
                                      rec.set('l_aktif', {2});
                                      rec.set('v_group_desc', '{3}');
                                      
                                      rec.commit();
                                    }};", store.ClientID, txGrup.Text.Trim(),
                           chkAktif.Checked.ToString().ToLower(), txDesc.Text.Trim()));
          }

        }

        Functional.ShowMsgInformation("Data berhasil tersimpan.");
        winDetail.Hide();

        break;
      case PostDataParser.ResponseStatus.Error:
        e.Success = false;
        e.ErrorMessage = result.Message;
        break;
      case PostDataParser.ResponseStatus.Failed:
        e.Success = false;
        e.ErrorMessage = result.Message;
        break;
      case PostDataParser.ResponseStatus.Unknown:
        e.Success = false;
        e.ErrorMessage = "Unknown result";
        break;
    }
  }
  
  [DirectMethod]
  public void NodeClick(string id, string nodeId, string subNode)
  {
    RightBuilder rb = new RightBuilder();

    Ext.Net.Store store = gridListAccess.GetStore();

    string tmp = (this.Cache[VIEW_STATE_VASKSES] == null ? string.Empty : (string)this.Cache[VIEW_STATE_VASKSES]);

    if (store != null)
    {
      store.RemoveAll();

      RightBuilder.RightAccess right = rb.RebuildRightAccess(tmp, nodeId, subNode);

      RightBuilder.Page[] pages = rb.PageAccess(subNode);
      if ((pages != null) && (pages.Length > 0))
      {
        RightBuilder.Page pg = null;
        List<object[]> list = new List<object[]>();
        Ext.Net.ColumnModel colModel = gridListAccess.ColumnModel;
        RightBuilder.PageRightAccess pra = null;

        #region Populate

        if (right == null)
        {
          for (int nLoop = 0, nLen = pages.Length; nLoop < nLen; nLoop++)
          {
            pg = pages[nLoop];

            list.Add(new object[]
              {
                pg.Name,
                pg.IsView,
                pg.IsPrint,
                pg.IsAdd,
                pg.IsEdit,
                pg.IsDelete,
                false,
                false,
                false,
                false,
                false,
              });
          }
        }
        else
        {
          for (int nLoop = 0, nLen = pages.Length; nLoop < nLen; nLoop++)
          {
            pg = pages[nLoop];

            for (int nLoopC = 0; nLoopC < right.Pages.Length; nLoopC++, pra = null)
            {
              pra = right.Pages[nLoopC];
              if ((!string.IsNullOrEmpty(pra.Name)) && pra.Name.Equals(pg.Name, StringComparison.OrdinalIgnoreCase))
              {
                break;
              }
            }

            if (pra == null)
            {
              list.Add(new object[]
              {
                pg.Name,
                pg.IsView,
                pg.IsPrint,
                pg.IsAdd,
                pg.IsEdit,
                pg.IsDelete,
                false,
                false,
                false,
                false,
                false,
              });
            }
            else
            {
              list.Add(new object[]
              {
                pg.Name,
                pg.IsView,
                pg.IsPrint,
                pg.IsAdd,
                pg.IsEdit,
                pg.IsDelete,
                (pg.IsView ? pra.IsView : false),
                (pg.IsPrint ? pra.IsPrint : false),
                (pg.IsAdd ? pra.IsAdd : false),
                (pg.IsEdit ? pra.IsEdit : false),
                (pg.IsDelete ? pra.IsDelete : false),
              });
            }
          }
        }

        #endregion

        store.DataSource = list.ToArray();
        store.DataBind();

        list.Clear();
      }
    }
  }
  
  protected void btnApply_Click(object sender, DirectEventArgs e)
  {
    string dataAccess = (e.ExtraParams["DataAccess"] ?? string.Empty);
    string dataNodeID = (e.ExtraParams["DataNodeID"] ?? string.Empty);
    string dataNode = (e.ExtraParams["DataNode"] ?? string.Empty);

    string groupId = hfGroup.Text;

    List<Dictionary<string, object>> list = JSON.Deserialize<List<Dictionary<string, object>>>(dataAccess);

    PostDataParser.StructureResponse result = SaveParserGroupAkses(groupId, dataNodeID, dataNode, list);

    switch (result.Response)
    {
      case PostDataParser.ResponseStatus.Success:
        e.Success = true;
        
        Functional.ShowMsgInformation("Data hak akses berhasil tersimpan.");
        winDetail.Hide();

        X.AddScript("{0}.getStore().commitChanges();", gridListAccess.ClientID);

        break;
      case PostDataParser.ResponseStatus.Error:
        e.Success = false;
        e.ErrorMessage = result.Message;
        break;
      case PostDataParser.ResponseStatus.Failed:
        e.Success = false;
        e.ErrorMessage = result.Message;
        break;
      case PostDataParser.ResponseStatus.Unknown:
        e.Success = false;
        e.ErrorMessage = "Unknown result";
        break;
    }

    for (int nLoop = 0, nLen = list.Count; nLoop < nLen; nLoop++)
    {
      list[nLoop].Clear();
    }

    list.Clear();

    GC.Collect();
  }

  protected void btnShowDetailAccess_OnClick(object sender, DirectEventArgs e)
  {
    if (string.IsNullOrEmpty(hfGroup.Text))
    {
      return;
    }

    treeApp.CollapseAll();
    X.AddScript(string.Format("{0}.getSelectionModel().clearSelections();", treeApp.ClientID));

    gridListAccess.GetStore().RemoveAll();

    winAkses.Hidden = false;
    winAkses.ShowModal();
  }
}
