using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class keuangan_faktur_FakturJual : Scms.Web.Core.PageHandler
{
  private PostDataParser.StructureResponse DeleteParser(string numberId, string ket)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = numberId;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", this.Nip);

    pair.DicAttributeValues.Add("Keterangan", ket.Trim());

    try
    {
      varData = parser.ParserData("FakturJual", "Delete", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("keuangan_faktur_FakturJual DeleteParser : {0} ", ex.Message);
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
  public void DeleteMethod(string noteNumber, string keterangan)
  {
    if (string.IsNullOrEmpty(noteNumber))
    {
      Functional.ShowMsgWarning("Nomor Faktur tidak terbaca.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Nomor SP tidak terbaca.";

      return;
    }
    else if (string.IsNullOrEmpty(keterangan))
    {
      Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Keterangan tidak boleh kosong.";

      return;
    }

    PostDataParser.StructureResponse respon = DeleteParser(noteNumber, keterangan);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r);{0}.commitChanges(); }}",
          storeGridMain.ClientID, noteNumber));

      Functional.ShowMsgInformation(string.Format("Nomor Faktur '{0}' telah terhapus.", noteNumber));
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
      FakturJualCtrl1.Initialize(storeGridMain.ClientID);
    }
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      FakturJualCtrl1.CommandPopulate(false, pID);
    }

    GC.Collect();
  }

  protected void btnPrintFaktur_OnClick(object sender, DirectEventArgs e)
  {
    if (!this.IsAllowPrinting)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
      return;
    }

    FakturJualPrintCtrl1.ShowPrintPage();
  }

  protected void btnGenerate_OnClick(object sender, DirectEventArgs e)
  {
      if (!this.IsAllowAdd)
      {
          Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
          return;
      }

      PostDataParser.StructureResponse respon = GenerateTaxNo();

      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {

          Functional.ShowMsgInformation(string.Format("Penomoran berhasil"));

          gridMain.Reload();
      }
      else
      {
          Functional.ShowMsgWarning(respon.Message);
      }
  }

  private PostDataParser.StructureResponse GenerateTaxNo()
  {
      PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

      PostDataParser parser = new PostDataParser();
      IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

      PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

      pair.IsSet = true;
      pair.IsList = true;
      //pair.Value = numberId;
      pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
      pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      string varData = null;

      dic.Add("ID", pair);
      pair.DicAttributeValues.Add("Entry", this.Nip);

      //pair.DicAttributeValues.Add("Keterangan", ket.Trim());

      try
      {
          varData = parser.ParserData("FakturJual", "GenerateTaxNo", dic);
      }
      catch (Exception ex)
      {
          Scms.Web.Common.Logger.WriteLine("keuangan_faktur_FakturJual GenerateTaxNo : {0} ", ex.Message);
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
