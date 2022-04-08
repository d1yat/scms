using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class master_prinsipal_MasterPrinsipal : Scms.Web.Core.PageHandler
{
  private PostDataParser.StructureResponse DeleteParser(string plNumber, string ket)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = plNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("SupplierID", pair);
    pair.DicAttributeValues.Add("Entry", this.Nip);
    pair.DicAttributeValues.Add("Keterangan", ket.Trim());

    try
    {
      varData = parser.ParserData("MasterPrisipal", "Delete", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("Master_Prinsipal DeleteParser : {0} ", ex.Message);
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
  public void DeleteMethod(string plNumber, string keterangan)
  {
    if (string.IsNullOrEmpty(plNumber))
    {
      Functional.ShowMsgWarning("Nomor Supplier tidak terbaca.");

      return;
    }
    else if (string.IsNullOrEmpty(keterangan))
    {
      Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");

      return;
    }

    PostDataParser.StructureResponse respon = DeleteParser(plNumber, keterangan);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r);{0}.commitChanges(); }}",
        storeGridItem.ClientID, plNumber));

      Functional.ShowMsgInformation(string.Format("Nomor Supplier '{0}' telah terhapus.", plNumber));
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
    }

  }  

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
        MasterPrinsipalCtrl.Initialize(gridMain.ClientID);
        MasterPrinsipalLeadtime.Initialize(gridMain.ClientID);
        MasterPrinsipalHistory.Initialize(gridMain.ClientID);
    }
  }

  protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
  {
    if (!this.IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }
      
    MasterPrinsipalCtrl.CommandPopulate(true, null);
  }

  protected void btnHistory_OnClick(object sender, DirectEventArgs e)
  {
    if (!this.IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    MasterPrinsipalHistory.CommandPopulate();
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    //Indra 20180815FM
    //if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    if (cmd.Equals("EditLeadtime", StringComparison.OrdinalIgnoreCase))
    {

        if (!this.IsAllowEdit)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mengubah leadtime.");
            return;
        }

        MasterPrinsipalLeadtime.CommandPopulate(false, pID);
    }
    else
    {
        if (!this.IsAllowAdd)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mengubah data principal.");
            return;
        }

        MasterPrinsipalCtrl.CommandPopulate(false, pID);
    }
    GC.Collect();
  }
}
