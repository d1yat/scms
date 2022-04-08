using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class master_pkp : Scms.Web.Core.PageHandler
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
        MasterPKPCtrl1.Initialize(storeGridFaktur.ClientID);
    }
  }

  protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
  {
    if (!this.IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    MasterPKPCtrl1.CommandPopulate(true, null);
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
        MasterPKPCtrl1.CommandPopulate(false, pID);
    }

    GC.Collect();
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  public void DeleteMethod(string fmNumber, string keterangan)
  {
      if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowDelete)
      {
          Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menghapus data.");
          return;
      }

      if (string.IsNullOrEmpty(fmNumber))
      {
          Functional.ShowMsgWarning("Nomor FM tidak terbaca.");
          //Ext.Net.ResourceManager.AjaxSuccess = false;
          //Ext.Net.ResourceManager.AjaxErrorMessage = "Nomor DO tidak terbaca.";

          return;
      }
      else if (string.IsNullOrEmpty(keterangan))
      {
          Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");
          //Ext.Net.ResourceManager.AjaxSuccess = false;
          //Ext.Net.ResourceManager.AjaxErrorMessage = "Keterangan tidak boleh kosong.";

          return;
      }
      PostDataParser.StructureResponse respon = DeleteParser(fmNumber, keterangan);

      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
          X.AddScript(
            string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r); {0}.commitChanges(); }}",
            storeGridFaktur.ClientID, fmNumber));

          Functional.ShowMsgInformation(string.Format("Nomor '{0}' telah terhapus.", fmNumber));
      }
      else
      {
          Functional.ShowMsgWarning(respon.Message);
          //Ext.Net.ResourceManager.AjaxSuccess = false;
          //Ext.Net.ResourceManager.AjaxErrorMessage = respon.Message;
      }
  }

  private PostDataParser.StructureResponse DeleteParser(string fmNumber, string ket)
  {
      PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

      PostDataParser parser = new PostDataParser();
      IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

      PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

      pair.IsSet = true;
      pair.IsList = true;
      pair.Value = fmNumber;
      pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
      pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      string varData = null;

      dic.Add("ID", pair);
      pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
      pair.DicAttributeValues.Add("pkpno", fmNumber);
      pair.DicAttributeValues.Add("KeteranganDel", ket.Trim());

      try
      {
          varData = parser.ParserData("MasterPKP", "Delete", dic);
      }
      catch (Exception ex)
      {
          Scms.Web.Common.Logger.WriteLine("faktur_manual SaveParser : {0} ", ex.Message);
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
}
