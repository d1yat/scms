using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_retur_ReturCustomer : Scms.Web.Core.PageHandler
{
  private const string AUTO_MODE = "AUTO";
  private const string STANDARD_MODE_GOOD = "STDGOOD";
  private const string STANDARD_MODE_BAD = "STDBAD";

  #region Private
  
  private PostDataParser.StructureResponse DeleteParser(string rcNumber, string gudangId, string ket)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = rcNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Gudang", gudangId);
    pair.DicAttributeValues.Add("Keterangan", ket.Trim());

    try
    {
      varData = parser.ParserData("RCIN", "Delete", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_retur_ReturCustomer DeleteParser : {0} ", ex.Message);
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
  
  private PostDataParser.StructureResponse SubmitParser(string doNumberID)
  {
    Scms.Web.Core.PageHandler page = this.Page as Scms.Web.Core.PageHandler;

    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = doNumberID;
    //pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    DateTime date = DateTime.Today;

    string GudangId = ((Scms.Web.Core.PageHandler)this.Page).ActiveGudang;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", page.Nip);
    pair.DicAttributeValues.Add("ConfirmSent", "true");
    pair.DicAttributeValues.Add("Gudang", GudangId);

    try
    {
      varData = parser.ParserData("RCIN", "ConfirmSent", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_Retur_Customer SubmitParser : {0} ", ex.Message);
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

  protected void Page_Init(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

      if (qryString.Equals("auto", StringComparison.OrdinalIgnoreCase))
      {
        ReturCustomerCtrl.Visible = true;
        ReturCustomerProcessCtrl.Visible = true;

        hfMode.Text = AUTO_MODE;
      }
      else
      {
        ReturCustomerProcessCtrl.Visible = false;
        ReturCustomerCtrl.Visible = true;
        if (qryString.Equals("stdbad", StringComparison.OrdinalIgnoreCase))
        {
          hfMode.Text = STANDARD_MODE_BAD;
          hfType.Text = "02";
        }
        else
        {
          hfMode.Text = STANDARD_MODE_GOOD;
          hfType.Text = "01";
        }
      }
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      if (hfMode.Text.Equals(AUTO_MODE))
      {
        ReturCustomerProcessCtrl.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridRC.ClientID);
      }
      else
      {
        ReturCustomerCtrl.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridRC.ClientID, hfType.Text);
      }
	  hfGudang.Text = this.ActiveGudang;
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  public void DeleteMethod(string rcNumber, string gudangId, string keterangan)
  {
    if (!this.IsAllowDelete)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menghapus data.");
      return;
    }

    if (string.IsNullOrEmpty(rcNumber))
    {
      Functional.ShowMsgWarning("Nomor RC tidak terbaca.");

      return;
    }
    else if (string.IsNullOrEmpty(gudangId))
    {
      Functional.ShowMsgWarning("Gudang tidak terbaca.");

      return;
    }
    else if (string.IsNullOrEmpty(keterangan))
    {
      Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");

      return;
    }

    PostDataParser.StructureResponse respon = DeleteParser(rcNumber, gudangId, keterangan);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r); {0}.commitChanges(); }}",
        storeGridRC.ClientID, rcNumber));

      Functional.ShowMsgInformation(string.Format("Nomor RC '{0}' telah terhapus.", rcNumber));
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  public void SubmitMethod(string rcNumber)
  {
    if (!this.IsAllowEdit)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menyimpan data.");
      return;
    }

    if (string.IsNullOrEmpty(rcNumber))
    {
      Functional.ShowMsgWarning("Nomor RC tidak terbaca.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Nomor DO tidak terbaca.";

      return;
    }

    PostDataParser.StructureResponse respon = SubmitParser(rcNumber);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;

        scrpt = string.Format(@"var vIdx = {0}.findExact('c_rcno', '{1}'); 
                if(vIdx != -1) {{
                  var r = {0}.getAt(vIdx);
                  if(!Ext.isEmpty(r)) {{
                    r.set('l_sent', true);
                    r.commit();
                  }}
                }}", gridMain.GetStore().ClientID, rcNumber);

        X.AddScript(scrpt);

        Functional.ShowMsgInformation("Data RC berhasil terproses.");
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

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  public void SelectedSavedMethod(string rcNumber)
  {
    if (!this.IsAllowView)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk melihat data.");
      return;
    }

    if (string.IsNullOrEmpty(rcNumber))
    {
      Functional.ShowMsgWarning("Nomor RC tidak terbaca.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Nomor DO tidak terbaca.";

      return;
    }

    ReturCustomerCtrl.CommandPopulate(false, rcNumber, hfType.Text);

    Functional.ShowMsgInformation("Data berhasil tersimpan.");
  }

  protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
  {
    if (!this.IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    if (hfMode.Text.Equals(AUTO_MODE))
    {
      ReturCustomerProcessCtrl.CommandPopulate(true, null);
    }
    else
    {
      ReturCustomerCtrl.CommandPopulate(true, null, hfType.Text);
    }
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      ReturCustomerCtrl.CommandPopulate(false, pID, hfMode.Text);
      //ReturCustomerProcessCtrl.CommandPopulate(false, pID);
    }

    GC.Collect();
  }
}
