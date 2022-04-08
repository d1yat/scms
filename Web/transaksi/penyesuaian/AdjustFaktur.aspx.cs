using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_penyesuaian_AdjustFaktur : Scms.Web.Core.PageHandler
{
  private const string ADJ_FJ = "ADJFJ";
  private const string ADJ_FB = "ADJFB";

  private const string TYPE_ADJ_FJ = "05";
  private const string TYPE_ADJ_FB = "04";

  protected void Page_Init(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

      if (qryString.Equals("adjfj", StringComparison.OrdinalIgnoreCase))
      {
        hfMode.Text = ADJ_FJ;
        hfType.Text = TYPE_ADJ_FJ;
      }
      else
      {
        hfMode.Text = ADJ_FB;
        hfType.Text = TYPE_ADJ_FB;
      }
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    string modeValue = hfMode.Text.Trim().ToUpper();

    switch (modeValue)
    {
      case ADJ_FJ:
        {
          AdjustTransFJCtrl.Initialize(storeGridADJ.ClientID);
        }
        break;
      default:
        {
          AdjustTransFBCtrl.Initialize(storeGridADJ.ClientID);
        }
        break;
    }
  }

  protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
  {
    if (!this.IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    string modeValue = hfMode.Text.Trim().ToUpper();

    switch (modeValue)
    {
      case ADJ_FJ:
        {
          AdjustTransFJCtrl.CommandPopulate(true, null, null, TYPE_ADJ_FJ);
        }
        break;
      case ADJ_FB:
        {
          AdjustTransFBCtrl.CommandPopulate(true, null, null, TYPE_ADJ_FB);
        }
        break;
    }
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    string modeValue = hfMode.Text.Trim().ToUpper();

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

      if (qryString.Equals("adjfj", StringComparison.OrdinalIgnoreCase))
      {
        AdjustTransFJCtrl.CommandPopulate(false, pID, null, TYPE_ADJ_FJ);
      }
      else
      {
        AdjustTransFBCtrl.CommandPopulate(false, pID, null, TYPE_ADJ_FB);
      }
      
    }
  }

  private PostDataParser.StructureResponse DeleteParser(string rcNumber, string ket)
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
    pair.DicAttributeValues.Add("Entry", this.Nip);
    pair.DicAttributeValues.Add("Keterangan", ket.Trim());

    string modeValue = hfMode.Text.Trim().ToUpper();
    string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

    try
    {
      if (qryString.Equals("adjfj", StringComparison.OrdinalIgnoreCase))
      {
        varData = parser.ParserData("ADJFJ", "Delete", dic);
      }
      else
      {
        varData = parser.ParserData("ADJFB", "Delete", dic);
      }
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_Adjustment SaveParser : {0} ", ex.Message);
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
  public void DeleteMethod(string rcNumber, string keterangan)
  {
    if (string.IsNullOrEmpty(rcNumber))
    {
      Functional.ShowMsgWarning("Nomor Adjust tidak terbaca.");

      return;
    }
    if (string.IsNullOrEmpty(keterangan))
    {
      Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");

      return;
    }
    PostDataParser.StructureResponse respon = DeleteParser(rcNumber, keterangan);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r); }}",
        storeGridADJ.ClientID, rcNumber));

      Functional.ShowMsgInformation(string.Format("Nomor Adjust '{0}' telah terhapus.", rcNumber));
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
    }
  }
}
