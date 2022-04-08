using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Core;
using Ext.Net;
using System.Web.UI.HtmlControls;
using System.Xml;

public partial class Default4 : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!X.IsAjaxRequest)
    {
      Button_Click(null, null);
    }
  }

  protected void Button_Click(object sender, EventArgs e)
  {
    RightBuilder rb = new RightBuilder();
    rb.Populate();

    Scms.Web.Core.RightBuilder.RightApplication[] rights = rb.Rights;

    //string result = JSON.Serialize(rights);

    Ext.Net.TreePanel tree = new Ext.Net.TreePanel();

    RightBuilder.RightApplication rapp = null;
    Ext.Net.TreeNode nodeL1 = null;
    Ext.Net.TreeNode nodeL2 = null;
    Ext.Net.TreeNode nodeL3 = null;
    //Ext.Net.TreeNode nodeCA = null;

    List<Ext.Net.TreeNode> lstNodes = new List<Ext.Net.TreeNode>();

    Ext.Net.TreeNode rootNodes = new Ext.Net.TreeNode();
    rootNodes.Text = "Management";
    rootNodes.Icon = Icon.Briefcase;

    string tmp1, tmp2, tmp3;

    //RightBuilder.Page[] pages = null;

    for (int nLoop = 0, nLen = rights.Length; nLoop < nLen; nLoop++)
    {
      rapp = rights[nLoop];

      nodeL1 = new Ext.Net.TreeNode(rapp.ID, rapp.Name, Icon.Application);

      //nodeL1.CustomAttributes.Add(new ConfigItem("NodeID", rapp.ID, ParameterMode.Value));

      if ((rapp.Modules != null) && (rapp.Modules.Count > 0))
      {
        foreach (KeyValuePair<string, RightBuilder.Module> kvpMods in rapp.Modules)
        {
          tmp1 = string.Concat(rapp.ID, "_", kvpMods.Value.ID);

          nodeL2 = new Ext.Net.TreeNode(tmp1, kvpMods.Value.Name, Icon.Package);

          //nodeL2.CustomAttributes.Add(new ConfigItem("NodeID", kvpMods.Value.ID, ParameterMode.Value));

          if ((kvpMods.Value.Items != null) && (kvpMods.Value.Items.Count > 0))
          {
            foreach (KeyValuePair<string, RightBuilder.Item> kvpItems in kvpMods.Value.Items)
            {
              tmp2 = string.Concat(tmp1, "_", kvpItems.Value.ID);

              tmp3 = kvpItems.Value.Node;

              //pages = rb.PageAccess(tmp3);

              nodeL3 = new Ext.Net.TreeNode(tmp2, kvpItems.Value.Name, Icon.Page);

              nodeL3.Leaf = true;

              //if (pages != null)
              //{
              //  nodeCA = null;

              //  foreach (RightBuilder.Page pg in pages)
              //  {
              //    nodeCA = new Ext.Net.TreeNode(pg.Name);

              //    nodeCA.Leaf = true;

              //    nodeCA.CustomAttributes.Add(new ConfigItem("Nama", pg.Name, ParameterMode.Value));
              //    nodeCA.CustomAttributes.Add(new ConfigItem("isView", pg.IsView.ToString(), ParameterMode.Value));
              //    nodeCA.CustomAttributes.Add(new ConfigItem("isPrint", pg.IsPrint.ToString(), ParameterMode.Value));
              //    nodeCA.CustomAttributes.Add(new ConfigItem("isAdd", pg.IsAdd.ToString(), ParameterMode.Value));
              //    nodeCA.CustomAttributes.Add(new ConfigItem("isEdit", pg.IsEdit.ToString(), ParameterMode.Value));
              //    nodeCA.CustomAttributes.Add(new ConfigItem("isDelete", pg.IsDelete.ToString(), ParameterMode.Value));

              //    nodeL3.Nodes.Add(nodeCA);
              //  }
              //}

              nodeL3.CustomAttributes.Add(new ConfigItem("nodeX", tmp3, ParameterMode.Value));

              nodeL3.CustomAttributes.Add(new ConfigItem("NodeID", kvpItems.Value.ID, ParameterMode.Value));

              nodeL2.Nodes.Add(nodeL3);
            }
          }

          nodeL1.Nodes.Add(nodeL2);
        }
      }

      rootNodes.Nodes.Add(nodeL1);
      //lstNodes.Add(nodeL1);
    }

    rootNodes.Expanded = true;

    treeApp.Root.Clear();

    treeApp.Root.Add(rootNodes);
  }

  [DirectMethod]
  public void NodeClick(string id, string nodeX, string nodeId)
  {
    RightBuilder rb = new RightBuilder();

    Ext.Net.Store store = gridListAccess.GetStore();

    if (store != null)
    {
      store.RemoveAll();

      RightBuilder.Page[] pages = rb.PageAccess(nodeX);
      if ((pages != null) && (pages.Length > 0))
      {
        RightBuilder.Page pg = null;
        List<object[]> list = new List<object[]>();
        Ext.Net.ColumnModel colModel = gridListAccess.ColumnModel;
        
        //List<IDictionary<string, string>> listData = new List<IDictionary<string, string>>();
        
        //IDictionary<string, string> dic = null;

        for(int nLoop = 0, nLen = pages.Length;nLoop <nLen ;nLoop++)
        {
          pg = pages[nLoop];

          //dic = new Dictionary<string, string>();

          //dic.Add("Nama", pg.Name);
          //dic.Add("isOkView", pg.IsView.ToString());
          //dic.Add("isOkPrint", pg.IsPrint.ToString());
          //dic.Add("isOkAdd", pg.IsAdd.ToString());
          //dic.Add("isOkEdit", pg.IsEdit.ToString());
          //dic.Add("isOkDelete", pg.IsDelete.ToString());
          //dic.Add("isView", "false");
          //dic.Add("isPrint", "false");
          //dic.Add("isAdd", "false");
          //dic.Add("isEdit", "false");
          //dic.Add("isDelete", "false");

          //listData.Add(dic);

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

          //colBase = colModel.Columns[

          //colModel.Columns.Add(new Ext.Net.Column(new Column.Config()
          //{
          //  DataIndex = "Nama",
          //  Header = "Halaman",
          //  Sortable = false,
          //  Editable = false
          //}));

          //if (pg.HasAccess)
          //{
          //  if (pg.IsView)
          //  {
          //    colModel.Columns.Add(new Ext.Net.Column(new Column.Config()
          //    {
          //      DataIndex = "isView",
          //      Header = "Lihat",
          //      Editable = true
          //    }));
          //  }
          //  if (pg.IsPrint)
          //  {
          //    colModel.Columns.Add(new Ext.Net.Column(new Column.Config()
          //    {
          //      DataIndex = "isPrint",
          //      Header = "Cetak",
          //      Editable = true
          //    }));
          //  }
          //  if (pg.IsAdd)
          //  {
          //    colModel.Columns.Add(new Ext.Net.Column(new Column.Config()
          //    {
          //      DataIndex = "isAdd",
          //      Header = "Tambah",
          //      Editable = true
          //    }));
          //  }
          //  if (pg.IsEdit)
          //  {
          //    colModel.Columns.Add(new Ext.Net.Column(new Column.Config()
          //    {
          //      DataIndex = "isUbah",
          //      Header = "Ubah",
          //      Editable = true
          //    }));
          //  }
          //  if (pg.IsDelete)
          //  {
          //    colModel.Columns.Add(new Ext.Net.Column(new Column.Config()
          //    {
          //      DataIndex = "isDelete",
          //      Header = "Hapus",
          //      Editable = true
          //    }));
          //  }
          //}
        }

        store.DataSource = list.ToArray();
        store.DataBind();

        list.Clear();
      }
    }
  }

  private void ColumnManipulate(Ext.Net.ColumnModel colModel, string colName, bool showColumn)
  {
    for (int nLoop = 0, nLen = colModel.Columns.Count; nLoop < nLen; nLoop++)
    {
      if (colModel.Columns[nLoop].ColumnID.Equals(colName, StringComparison.OrdinalIgnoreCase))
      {
        colModel.SetHidden(nLoop, (!showColumn));
        
        break;
      }
    }
  }

  protected void btnApply_Click(object sender, DirectEventArgs e)
  {
    string dataAccess = (e.ExtraParams["DataAccess"] ?? string.Empty);
    string dataNodeID = (e.ExtraParams["DataNodeID"] ?? string.Empty);
    string dataNode = (e.ExtraParams["DataNode"] ?? string.Empty);

    List<Dictionary<string, object>> list = JSON.Deserialize<List<Dictionary<string, object>>>(dataAccess);

    for (int nLoop = 0, nLen = list.Count; nLoop < nLen; nLoop++)
    {
      list[nLoop].Clear();
    }

    list.Clear();

    GC.Collect();
  }

  protected void btnPostBack_Click(object sender, EventArgs e)
  {
    SoaCaller soa = new SoaCaller();

    string str = soa.TestPostData("Data output");
  }
}
