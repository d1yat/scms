using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_pemusnahan_PM : Scms.Web.Core.PageHandler
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

        dic.Add("ID", pair);
        pair.DicAttributeValues.Add("Entry", this.Nip);

        pair.DicAttributeValues.Add("Keterangan", ket.Trim());

        try
        {
            varData = parser.ParserData("Pemusnahan", "Delete", dic);
        }
        catch (Exception ex)
        {
            Scms.Web.Common.Logger.WriteLine("transaksi_pemusnahan_PM DeleteParser : {0} ", ex.Message);
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
    public void DeleteMethod(string sttNumber, string keterangan)
    {
        if (!this.IsAllowDelete)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menghapus data.");
            return;
        }

        if (string.IsNullOrEmpty(sttNumber))
        {
            Functional.ShowMsgWarning("Nomor Transaksi Pemusnahan tidak terbaca.");
            //Ext.Net.ResourceManager.AjaxSuccess = false;
            //Ext.Net.ResourceManager.AjaxErrorMessage = "Nomor PL tidak terbaca.";

            return;
        }
        else if (string.IsNullOrEmpty(keterangan))
        {
            Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");
            //Ext.Net.ResourceManager.AjaxSuccess = false;
            //Ext.Net.ResourceManager.AjaxErrorMessage = "Keterangan tidak boleh kosong.";

            return;
        }

        PostDataParser.StructureResponse respon = DeleteParser(sttNumber, keterangan);

        if (respon.Response == PostDataParser.ResponseStatus.Success)
        {
            X.AddScript(
              string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r);{0}.commitChanges(); }}",
              storeGridSTT.ClientID, sttNumber));

            Functional.ShowMsgInformation(string.Format("Nomor Transaksi Pemusnahan '{0}' telah terhapus.", sttNumber));
        }
        else
        {
            Functional.ShowMsgWarning(respon.Message);
            //Ext.Net.ResourceManager.AjaxSuccess = false;
            //Ext.Net.ResourceManager.AjaxErrorMessage = respon.Message;
        }

    }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            PMCtrl1.Initialize("5", "Gudang Pemusnahan", storeGridSTT.ClientID, hfMode.Text);
            //STTCtrlPrint1.Initialize(this.ActiveGudang, this.ActiveGudangDescription, hfMode.Text);
        }
    }

    protected void btnAddNew_OnClick(object sender, EventArgs e)
    {
        if (!this.IsAllowAdd)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
            return;
        }

        PMCtrl1.CommandPopulate(true, null);
    }

    protected void gridMainCommand(object sender, DirectEventArgs e)
    {
        string cmd = (e.ExtraParams["Command"] ?? string.Empty);
        string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
        string sttID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

        if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
        {
            PMCtrl1.CommandPopulate(false, sttID);
        }

        GC.Collect();
    }

    protected void btnPrintPL_OnClick(object sender, DirectEventArgs e)
    {
        if (!this.IsAllowPrinting)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
            return;
        }

        PMCtrlPrint1.ShowPrintPage();
    }
}
