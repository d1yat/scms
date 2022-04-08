using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_penyesuaian_AdjustTrans : Scms.Web.Core.PageHandler
{
  private const string ADJ_PO = "ADJPO";

  private const string TYPE_ADJ_PO = "08";

  protected void Page_Init(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

      if (qryString.Equals("adjpo", StringComparison.OrdinalIgnoreCase))
      {
        hfMode.Text = ADJ_PO;
        hfType.Text = TYPE_ADJ_PO;
      }
      else
      {
        hfMode.Text = ADJ_PO;
        hfType.Text = TYPE_ADJ_PO;
      }
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    string modeValue = hfMode.Text.Trim().ToUpper();

    switch (modeValue)
    {
      case ADJ_PO:
        {
          AdjustTransPOCtrl.Initialize(storeGridADJ.ClientID);
        }
        break;
      default:
        {
          AdjustTransPOCtrl.Initialize(storeGridADJ.ClientID);
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

      if (qryString.Equals("adjpo", StringComparison.OrdinalIgnoreCase))
      {
        AdjustTransPOCtrl.CommandPopulate(false, pID, null, TYPE_ADJ_PO);
      }
      else
      {
        AdjustTransPOCtrl.CommandPopulate(false, pID, null, TYPE_ADJ_PO);
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

    string modeValue = hfMode.Text.Trim().ToUpper();

    switch (modeValue)
    {
      case ADJ_PO:
        {
          AdjustTransPOCtrl.CommandPopulate(true, null, null, TYPE_ADJ_PO);
        }
        break;
     
    }
  }

  private PostDataParser.StructureResponse DeleteParser(string adjNumber, string ket, string gdg)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = adjNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", this.Nip);
    pair.DicAttributeValues.Add("KetDel", ket.Trim());
    pair.DicAttributeValues.Add("Gudang", gdg);

    try
    {
      varData = parser.ParserData("ADJTRANS", "Delete", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_Adjustment Delete : {0} ", ex.Message);
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

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  public void DeleteMethod(string adjNumber, string keterangan, string gdg)
  {
    if (string.IsNullOrEmpty(adjNumber))
    {
      Functional.ShowMsgWarning("Nomor Adjust tidak terbaca.");

      return;
    }
    if (string.IsNullOrEmpty(keterangan))
    {
      Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");

      return;
    }
    PostDataParser.StructureResponse respon = DeleteParser(adjNumber, keterangan, gdg);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r);{0}.commitChanges() }}",
        storeGridADJ.ClientID, adjNumber));

      Functional.ShowMsgInformation(string.Format("Nomor Adjust '{0}' telah terhapus.", adjNumber));
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
    }
  }
}
