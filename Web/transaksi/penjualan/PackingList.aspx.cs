using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaction_sales_PackingList : Scms.Web.Core.PageHandler
{
  private const string CONFIRM_MODE = "CF";
  private const string STANDARD_MODE = "STD";
  private const string AUTO_MODE = "AUTO";
  private const string BOX_MODE = "BOX";
  private const string AUTOGEN_MODE = "AUTOGEN";

  #region Private

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
      varData = parser.ParserData("PackingList", "Delete", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_PackingListCtrl DeleteParser : {0} ", ex.Message);
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

  #endregion

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  public void DeleteMethod(string plNumber, string keterangan)
  {
    if (!this.IsAllowDelete)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menghapus data.");
      return;
    }

    if (string.IsNullOrEmpty(plNumber))
    {
      Functional.ShowMsgWarning("Nomor PL tidak terbaca.");
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
    else if (hfMode.Text.Trim().Equals(CONFIRM_MODE, StringComparison.OrdinalIgnoreCase))
    {
      Functional.ShowMsgError("Maaf, anda tidak dapat menghapus data.");
      return;
    }

    PostDataParser.StructureResponse respon = DeleteParser(plNumber, keterangan);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r);{0}.commitChanges(); }}",
        storeGridPL.ClientID, plNumber));

      Functional.ShowMsgInformation(string.Format("Nomor PL '{0}' telah terhapus.", plNumber));      
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = respon.Message;
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  public void ShowSavedData(string plNumber, string msg)
  {
    if (!this.IsAllowView)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk melihat data.");
      return;
    }

    if (string.IsNullOrEmpty(plNumber))
    {
      Functional.ShowMsgWarning("Nomor PL tidak terbaca.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Nomor PL tidak terbaca.";

      return;
    }

    PackingListCtrl1.CommandPopulate(false, plNumber);

    if (!string.IsNullOrEmpty(msg))
    {
      Functional.ShowMsgInformation(msg);
    }
  }

  protected void Page_Init(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

      if (qryString.Equals("confirm", StringComparison.OrdinalIgnoreCase))
      {
        PackingListAutoCtrl1.Visible = false;
        PackingListPrintCtrl1.Visible = false;
        PackingListConfirmCtrl1.Visible = true;

        hfMode.Text = CONFIRM_MODE;

        btnAddNew.Hidden = true;
        btnPrintPL.Hidden = true;
      }
      else if (qryString.Equals("auto", StringComparison.OrdinalIgnoreCase))
      {
        PackingListConfirmCtrl1.Visible = false;
        PackingListAutoCtrl1.Visible = true;

        hfMode.Text = AUTO_MODE;
        hfType.Text = "02";
      }
      else if (qryString.Equals("box", StringComparison.OrdinalIgnoreCase))
      {
        PackingListConfirmCtrl1.Visible = false;
        PackingListAutoCtrl1.Visible = true;

        hfMode.Text = BOX_MODE;
        hfType.Text = "05";
      }
      else if (qryString.Equals("autogen", StringComparison.OrdinalIgnoreCase))
      {
        PackingListAutoGeneratorCtrl.Visible = true;
        PackingListConfirmCtrl1.Visible = false;
        PackingListAutoCtrl1.Visible = false;

        hfMode.Text = AUTOGEN_MODE;
        hfType.Text = "01";
      }
      else
      {
        PackingListAutoCtrl1.Visible = false;
        PackingListConfirmCtrl1.Visible = false;
        PackingListCtrl1.Visible = true;

        hfMode.Text = STANDARD_MODE;
      }
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      hfGdg.Text = this.ActiveGudang;

      if (hfMode.Text.Equals(CONFIRM_MODE))
      {
        Ext.Net.Toolbar tb = pnlMainControl.TopBar[0] as Ext.Net.Toolbar;
        if (tb != null)
        {
          //tb.Hidden = true;
        }

        //PackingListCtrl1.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridPL.ClientID, true);
        PackingListConfirmCtrl1.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridPL.ClientID);
      }
      else if (hfMode.Text.Equals(AUTO_MODE))
      {
        PackingListCtrl1.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridPL.ClientID);
        PackingListAutoCtrl1.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridPL.ClientID);
      }
      else if (hfMode.Text.Equals(BOX_MODE))
      {
        PackingListCtrl1.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridPL.ClientID);
        PackingListMasterBox.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridPL.ClientID);
      }
      else if (hfMode.Text.Equals(AUTOGEN_MODE))
      {
        PackingListCtrl1.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridPL.ClientID);
        PackingListAutoGeneratorCtrl.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridPL.ClientID);
      }
      else
      {
        PackingListCtrl1.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridPL.ClientID);
      }
    }
  }

  protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
  {
    if (!this.IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    if (hfMode.Text.Equals(CONFIRM_MODE))
    {
      Functional.ShowMsgError("Maaf, fungsi ini tidak dapat dipergunakan.");
      return;
    }
    else if (hfMode.Text.Equals(AUTO_MODE))
    {
      //PackingListAutoCtrl1.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridPL.ClientID);
      PackingListAutoCtrl1.CommandPopulate();
    }
    else if (hfMode.Text.Equals(BOX_MODE))
    {
      //PackingListAutoCtrl1.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridPL.ClientID);
      PackingListMasterBox.CommandPopulate(true, null);
    }
    else if (hfMode.Text.Equals(AUTOGEN_MODE))
    {
      PackingListAutoGeneratorCtrl.CommandPopulate(true, null);
    }
    else
    {
      PackingListCtrl1.CommandPopulate(true, null);
    }
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      if (hfMode.Text.Equals(CONFIRM_MODE))
      {
        PackingListConfirmCtrl1.CommandPopulate(false, pID);
      }
      else
      {
        PackingListCtrl1.CommandPopulate(false, pID);
      }
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

    PackingListPrintCtrl1.ShowPrintPage();
  }
}
