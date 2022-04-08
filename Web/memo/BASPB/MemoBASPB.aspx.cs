using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_memo_BASPB : Scms.Web.Core.PageHandler
{
  private void GetTypeName()
  {
    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    Dictionary<string, object> dic = null;
    Dictionary<string, string> dicInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    string[][] paramX = new string[][]{
        new string[] { "c_notrans = @0", "47", "System.String"},
        new string[] { "c_portal = @0", "3", "System.Char"},
        new string[] { "c_type = @0", "03", "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "2001", paramX);

    try
    {
      dic = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dic.ContainsKey("records") && (dic.ContainsKey("totalRows") && (((long)dic["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dic["records"]);

        dicInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        hfTypeName.Text = dicInfo.GetValueParser<string>("v_ket");
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_pembelian_MemoBASPB:GetTypeName - ", ex.Message));
    }
    finally
    {
      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicInfo != null)
      {
        dicInfo.Clear();
      }
      if (dic != null)
      {
        dic.Clear();
      }
    }

    #endregion

    GC.Collect();
  }

  private PostDataParser.StructureResponse DeleteParser(string baspbNumber, string ket, string sjno)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = baspbNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", this.Nip);

    pair.DicAttributeValues.Add("Keterangan", ket.Trim());
    pair.DicAttributeValues.Add("sjno", sjno.Trim());

    try
    {
        varData = parser.ParserData("MemoBASPBSJ", "Delete", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_MemoBASPB DeleteParser : {0} ", ex.Message);
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

  [DirectMethod(ShowMask = true)]
  public void DeleteMethod(string baspbNumber, string keterangan, string sjno)
  {
    if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowDelete)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menghapus data.");
      return;
    }

    if (string.IsNullOrEmpty(baspbNumber))
    {
      Functional.ShowMsgWarning("Nomor BASPB tidak terbaca.");

      return;
    }
    else if (string.IsNullOrEmpty(keterangan))
    {
      Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");

      return;
    }

    PostDataParser.StructureResponse respon = DeleteParser(baspbNumber, keterangan, sjno);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r);{0}.commitChanges(); }}",
        storeGridSP.ClientID, baspbNumber));

      Functional.ShowMsgInformation(string.Format("Nomor BASPB '{0}' telah terhapus.", baspbNumber));
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = respon.Message;
    }
  }  

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      GetTypeName();

      MemoBASPBCtrl1.Initialize(storeGridSP.ClientID, hfTypeName.Text);
    }
  }

  protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
  {
    if (!this.IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    MemoBASPBCtrl1.CommandPopulate(true, null);  
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      MemoBASPBCtrl1.CommandPopulate(false, pID);
    }

    GC.Collect();
  }
}
