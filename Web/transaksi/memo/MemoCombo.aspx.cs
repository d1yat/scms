using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_memo_MemoCombo : Scms.Web.Core.PageHandler
{
  #region Private

  private PostDataParser.StructureResponse DeleteParser(string gudang, string comboNumber, string ket)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = comboNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", this.Nip);
    pair.DicAttributeValues.Add("Gudang", gudang);

    pair.DicAttributeValues.Add("Keterangan", ket.Trim());

    try
    {
      varData = parser.ParserData("LGMemoCombo", "Delete", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_memo_MemoCombo DeleteParser : {0} ", ex.Message);
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

  private PostDataParser.StructureResponse ConfirmParser(string gudang, string comboNumber)
  {
    Scms.Web.Core.PageHandler page = this.Page as Scms.Web.Core.PageHandler;

    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = comboNumber;
    //pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Gudang", gudang);
    pair.DicAttributeValues.Add("Entry", page.Nip);
    pair.DicAttributeValues.Add("IsConfirm", "true");

    try
    {
      varData = parser.ParserData("LGMemoCombo", "Confirm", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_memo_MemoCombo ConfirmParser : {0} ", ex.Message);
    }

    string result = null;

    if (!string.IsNullOrEmpty(varData))
    {
      Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

      result = soa.PostData(varData);
      //result = null;

      responseResult = parser.ResponseParser(result);
    }

    return responseResult;
  }

  #endregion

  [DirectMethod(ShowMask = true)]
  public void DeleteMethod(string gudang, string comboNumber, string keterangan)
  {
    if (!this.IsAllowDelete)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menghapus data.");
      return;
    }

    if (string.IsNullOrEmpty(comboNumber))
    {
      Functional.ShowMsgWarning("Nomor Combo tidak terbaca.");
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
    else if (string.IsNullOrEmpty(gudang))
    {
      Functional.ShowMsgWarning("Gudang tidak boleh kosong.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Keterangan tidak boleh kosong.";

      return;
    }

    PostDataParser.StructureResponse respon = DeleteParser(gudang, comboNumber, keterangan);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r);{0}.commitChanges(); }}",
        storeGridCB.ClientID, comboNumber));

      Functional.ShowMsgInformation(string.Format("Nomor MEMO '{0}' telah terhapus.", comboNumber));
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = respon.Message;
    }
  }

  [DirectMethod(ShowMask = true)]
  public void SubmitMethod(string gudang, string comboNumber)
  {
    if (string.IsNullOrEmpty(comboNumber))
    {
      Functional.ShowMsgWarning("Nomor Combo tidak terbaca.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Nomor SP tidak terbaca.";

      return;
    }
    else if (string.IsNullOrEmpty(gudang))
    {
      Functional.ShowMsgWarning("Gudang tidak boleh kosong.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Keterangan tidak boleh kosong.";

      return;
    }

    PostDataParser.StructureResponse respon = ConfirmParser(gudang, comboNumber);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;

        scrpt = string.Format(@"var vIdx = {0}.findExact('c_combono', '{1}'); 
                if(vIdx != -1) {{
                  var r = {0}.getAt(vIdx);
                  if(!Ext.isEmpty(r)) {{
                    r.set('l_confirm', true);
                    r.commit();
                  }}
                }}", gridMain.GetStore().ClientID, comboNumber);

        X.AddScript(scrpt);

        Functional.ShowMsgInformation("Data Combo berhasil terproses.");
      }
      else
      {
        Functional.ShowMsgWarning(respon.Message);
      }
    }
    else
    {
      Functional.ShowMsgWarning("Unknown response");
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      MemoComboCtrl1.Initialize(storeGridCB.ClientID);
    }
  }

  protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
  {
    if (!this.IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    MemoComboCtrl1.CommandPopulate(true, null, null);  
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);
    string gdgID = (e.ExtraParams["GudangID"] ?? string.Empty);

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      MemoComboCtrl1.CommandPopulate(false, gdgID, pID);
    }

    GC.Collect();
  }
  
  protected void btnPrintCombo_OnClick(object sender, DirectEventArgs e)
  {
    if (!this.IsAllowPrinting)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
      return;
    }

    MemoComboPrintCtrl1.ShowPrintPage();
  }
}
