using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_transfer_TransferGudangKarantina : Scms.Web.Core.PageHandler
{
  private const string CONFIRM_MODE = "CF";
  private const string STANDARD_MODE = "STD";

  protected void Page_Init(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

      if (qryString.Equals("confirm", StringComparison.OrdinalIgnoreCase))
      {
        hfMode.Text = CONFIRM_MODE;
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
      hfGdg.Text = pag.ActiveGudang;

      if (hfMode.Text.Equals(CONFIRM_MODE))
      {
        Ext.Net.Toolbar tb = pnlMainControl.TopBar[0] as Ext.Net.Toolbar;

        Ext.Net.Button BtnAdd = pnlMainControl.TopBar[0].Controls[0] as Ext.Net.Button;
        Ext.Net.Button BtnCetak = pnlMainControl.TopBar[0].Controls[2] as Ext.Net.Button;

        if (BtnAdd != null)
        {
          BtnAdd.Hidden = true;
        }
        if (BtnCetak != null)
        {
          BtnCetak.Hidden = true;
        }
        //if (tb != null)
        //{
        //  tb.Hidden = true;
        //}

        TransferGudangKarantinaConfirmCtrl.Initialize(pag.ActiveGudang, pag.ActiveGudangDescription, storeGridSJ.ClientID, true, hfMode.Text);
      }
      else
      {
          TransferGudangKarantinaCtrl.Initialize(pag.ActiveGudang, pag.ActiveGudangDescription, storeGridSJ.ClientID, false, hfMode.Text);
        
      }
    }
  }

  private PostDataParser.StructureResponse DeleteParser(string sjNumber, string ket)
  {
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
    pair.DicAttributeValues.Add("Entry", this.Nip);
    pair.DicAttributeValues.Add("From", this.ActiveGudang);
    pair.DicAttributeValues.Add("Keterangan", ket.Trim());

    try
    {
      varData = parser.ParserData("TransferGudangRepack", "Delete", dic);
    }
    catch (Exception ex)
    {
        Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_TransferGudangKarantina DeleteParser : {0} ", ex.Message);
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

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  public void DeleteMethod(string sjNumber, string keterangan)
  {
    if (string.IsNullOrEmpty(sjNumber))
    {
      Functional.ShowMsgWarning("Nomor RC tidak terbaca.");

      return;
    }
    if (string.IsNullOrEmpty(keterangan))
    {
      Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");

      return;
    }
    PostDataParser.StructureResponse respon = DeleteParser(sjNumber, keterangan);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r); }}",
        storeGridSJ.ClientID, sjNumber));

      Functional.ShowMsgInformation(string.Format("Nomor RC '{0}' telah terhapus.", sjNumber));
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
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

    TransferGudangKarantinaCtrl.CommandPopulate(true, null);

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
        TransferGudangKarantinaConfirmCtrl.CommandPopulate(false, pID);
      }
      else
      {
          TransferGudangKarantinaCtrl.CommandPopulate(false, pID);
      }
    }
    GC.Collect();
  }
}
