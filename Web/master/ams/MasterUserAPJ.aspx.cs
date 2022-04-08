using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class master_ams_MasterUserAPJ : Scms.Web.Core.PageHandler
{
    private PostDataParser.StructureResponse DeleteParser(string Number, string ket)
    {
        PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

        PostDataParser parser = new PostDataParser();
        IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

        PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

        pair.IsSet = true;
        pair.IsList = true;
        pair.Value = Number;
        pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
        pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        string varData = null;

        dic.Add("NipID", pair);
        pair.DicAttributeValues.Add("Entry", this.Nip);
        pair.DicAttributeValues.Add("Keterangan", ket.Trim());

        try
        {
            varData = parser.ParserData("MasterUserApj", "Delete", dic);
        }
        catch (Exception ex)
        {
            Scms.Web.Common.Logger.WriteLine("Master_ams DeleteParser : {0} ", ex.Message);
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
            Functional.ShowMsgWarning("NIP tidak terbaca.");

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
              storeGridAPJ.ClientID, plNumber));

            Functional.ShowMsgInformation(string.Format("User APJ '{0}' telah terhapus.", plNumber));
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
            MasterUserAPJCtrl.Initialize(storeGridAPJ.ClientID);
        }
    }

    protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
    {
        if (!this.IsAllowAdd)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
            return;
        }

        MasterUserAPJCtrl.CommandPopulate(true, null);
    }

    protected void gridMainCommand(object sender, DirectEventArgs e)
    {
        string cmd = (e.ExtraParams["Command"] ?? string.Empty);
        string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
        string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

        if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
        {
            MasterUserAPJCtrl.CommandPopulate(false, pID);
        }

        GC.Collect();
    }
}
