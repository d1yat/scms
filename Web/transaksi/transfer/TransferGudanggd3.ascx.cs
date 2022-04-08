using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_transfer_TransferGudang : System.Web.UI.UserControl
{
  private const string CONFIRM_MODE = "CF";
  private const string STANDARD_MODE = "STD";
  private const string AUTO_MODE = "OTO";
  
  #region Private

  private PostDataParser.StructureResponse DeleteParser(string sjNumber, string ket)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = sjNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("From", pag.ActiveGudang);
    pair.DicAttributeValues.Add("Keterangan", ket.Trim());

    try
    {
      varData = parser.ParserData("TransferGudang", "Delete", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_Transfer_Gudang SaveParser : {0} ", ex.Message);
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

  protected void Page_Init(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

      if (qryString.Equals("confirm", StringComparison.OrdinalIgnoreCase))
      {
        hfMode.Text = CONFIRM_MODE;
      }
      else if (qryString.Equals("auto", StringComparison.OrdinalIgnoreCase))
      {
        hfMode.Text = AUTO_MODE;
      }
      else
      {
        hfMode.Text = STANDARD_MODE;
      }
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
    if (!this.IsPostBack)
    {
      //hfGudang.Text = pag.ActiveGudang;
      hfGudangDesc.Text = pag.ActiveGudangDescription;

      if (hfMode.Text.Equals(CONFIRM_MODE))
      {
        Ext.Net.Toolbar tb = pnlMainControl.TopBar[0] as Ext.Net.Toolbar;
        if (tb != null)
        {
        //  tb.Hidden = true;
          tb.Controls[0].Visible = false;
          tb.Controls[1].Visible = false;
          tb.Controls[2].Visible = false;
          tb.Controls[3].Visible = false;
        }

        TransferGudangCtrl.Initialize(pag.ActiveGudang, pag.ActiveGudangDescription, storeGridSJ.ClientID, true);
      }
      else if (hfMode.Text.Equals(AUTO_MODE))
      {
//        TransferGudangAuto.Initialize(pag.ActiveGudang, pag.ActiveGudangDescription, storeGridSJ.ClientID, true);
      }
      else
      {
        TransferGudangCtrl.Initialize(pag.ActiveGudang, pag.ActiveGudangDescription, storeGridSJ.ClientID, false);
      }
    }
  }

  protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
    if (!pag.IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    if (hfMode.Text.Equals(CONFIRM_MODE))
    {
      Functional.ShowMsgError("Maaf, fungsi ini tidak dapat dipergunakan.");
      return;
    }

    if (hfMode.Text.Equals(AUTO_MODE))
    {
      //TransferGudangAuto.CommandPopulate(true, null);
    }
    else
    {
      TransferGudangCtrl.CommandPopulate(true, null);
    }
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      TransferGudangCtrl.CommandPopulate(false, pID);
    }

    GC.Collect();
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  public void DeleteMethod(string sjNumber, string keterangan)
  {
    if (string.IsNullOrEmpty(sjNumber))
    {
      Functional.ShowMsgWarning("Nomor SJ tidak terbaca.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Nomor SJ tidak terbaca.";

      return;
    }
    else if (string.IsNullOrEmpty(keterangan))
    {
      Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Keterangan tidak boleh kosong.";

      return;
    }
    
    PostDataParser.StructureResponse respon = DeleteParser(sjNumber, keterangan);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r);{0}.commitChanges(); }}",
        storeGridSJ.ClientID, sjNumber));

      Functional.ShowMsgInformation(string.Format("Nomor SJ '{0}' telah terhapus.", sjNumber));
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = respon.Message;
    }
  }

  protected void btnPrintSJ_OnClick(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
    if (!pag.IsAllowPrinting)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
      return;
    }

    //string typeCode = null;

    //if (hfMode.Text.Equals(CONFIRM_MODE, StringComparison.OrdinalIgnoreCase))
    //{
    //  typeCode = "01";
    //}
    //else if (hfMode.Text.Equals(STANDARD_MODE, StringComparison.OrdinalIgnoreCase))
    //{
    //  typeCode = "02";
    //}

    //TransferGudangCtrlPrint.ShowPrintPage("01");
  }
}
