using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class memo_pemusnahan_MemoPemusnahan : Scms.Web.Core.PageHandler
{
  protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            MemoPemusnahanCtrl.Initialize(storeGridMemo.ClientID);
        }
    }

  protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
  {
    if (!this.IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    MemoPemusnahanCtrl.CommandPopulate(true, null, null);
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);
    string gdgID = (e.ExtraParams["GudangID"] ?? string.Empty);

    MemoPemusnahanCtrl.CommandPopulate(false, pID, gdgID);

    GC.Collect();
  }

  private PostDataParser.StructureResponse DeleteParser(string gudang, string memoNumber, string ket)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = memoNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", this.Nip);
    pair.DicAttributeValues.Add("Gudang", gudang);

    pair.DicAttributeValues.Add("Keterangan", ket.Trim());

    try
    {
      varData = parser.ParserData("MKMemoPemusnahan", "Delete", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("memo_pemusnahan_MemoPemusnahan DeleteParser : {0} ", ex.Message);
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
  public void DeleteMethod(string gudang, string memoNumber, string keterangan)
  {
    if (!this.IsAllowDelete)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menghapus data.");
      return;
    }

    if (string.IsNullOrEmpty(memoNumber))
    {
      Functional.ShowMsgWarning("Nomor Pemusnahan Memo tidak terbaca.");

      return;
    }
    else if (string.IsNullOrEmpty(keterangan))
    {
      Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");

      return;
    }
    else if (string.IsNullOrEmpty(gudang))
    {
      Functional.ShowMsgWarning("Gudang tidak boleh kosong.");

      return;
    }

    PostDataParser.StructureResponse respon = DeleteParser(gudang, memoNumber, keterangan);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r);{0}.commitChanges(); }}",
        storeGridMemo.ClientID, memoNumber));

      Functional.ShowMsgInformation(string.Format("Nomor MEMO Pemusnahan '{0}' telah terhapus.", memoNumber));
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
    }
  }

  protected void btnPrintMP_OnClick(object sender, DirectEventArgs e)
  {
      if (!this.IsAllowPrinting)
      {
          Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
          return;
      }

      MemoPemusnahanCtrlPrint.ShowPrintPage();
  }
}
