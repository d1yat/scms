using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class master_blokitem_MasterItem : Scms.Web.Core.PageHandler
{
  private PostDataParser.StructureResponse SaveParser(Dictionary<string, string>[] dics)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string itemCode = null;
    bool isBlock = false;
    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      dicData = dics[nLoop];

      isBlock = dicData.GetValueParser<bool>("l_block");
      itemCode = dicData.GetValueParser<string>("c_iteno", string.Empty);

      if (!string.IsNullOrEmpty(itemCode))
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("Item", itemCode);
        dicAttr.Add("Blocked", isBlock.ToString().ToLower());

        pair.DicValues.Add(nLoop.ToString(), new PostDataParser.StructurePair()
        {
          IsSet = true,
          DicAttributeValues = dicAttr
        });
      }

      dicData.Clear();
    }

    try
    {
      varData = parser.ParserData("MasterBlockItem", "Modify", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("master_blokitem_MasterItem SaveParser : {0} ", ex.Message);
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

  [DirectMethod(ShowMask = true,
    Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  public void SaveBlockItemMethod(string jsData, string storeId)
  {
    Dictionary<string, string>[] gridDataItemBlok = JSON.Deserialize<Dictionary<string, string>[]>(jsData);

    if ((gridDataItemBlok != null) && (gridDataItemBlok.Length > 0))
    {
      PostDataParser.StructureResponse respon = SaveParser(gridDataItemBlok);
      
      if (respon.IsSet)
      {
        if (respon.Response == PostDataParser.ResponseStatus.Success)
        {
          X.AddScript(string.Concat(storeId, ".commitChanges();"));

          Functional.ShowMsgInformation("Data berhasil tersimpan.");
        }
        else
        {
          Functional.ShowMsgWarning(respon.Message);
        }
      }
      else
      {
        Functional.ShowMsgError("Unknown response");
      }
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      //MasterItemCtrl.Initialize(storeGridItem.ClientID);
    }
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);
    string pidName = (e.ExtraParams["PrimaryNameID"] ?? string.Empty);

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      BlockItemCtrl1.CommandPopulate(pID, pidName);
    }

    GC.Collect();
  }
}
