using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_penyesuaian_AdjustVoucher : Scms.Web.Core.PageHandler
{
  private const string ADJ_VOUCHER_DEBIT = "ADJVOUCHERDEBIT";
  private const string ADJ_VOUCHER_CREDIT = "ADJVOUCHERCREDIT";

  private const string TYPE_ADJ_VOUCHER_DEBIT = "12";
  private const string TYPE_ADJ_VOUCHER_CREDIT = "17";

  protected void Page_Init(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

      if (qryString.Equals("adjvouchercredit", StringComparison.OrdinalIgnoreCase))
      {
        hfMode.Text = ADJ_VOUCHER_CREDIT;
        hfType.Text = TYPE_ADJ_VOUCHER_CREDIT;
      }
      else
      {
        hfMode.Text = ADJ_VOUCHER_DEBIT;
        hfType.Text = TYPE_ADJ_VOUCHER_DEBIT;
      }
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    string modeValue = hfMode.Text.Trim().ToUpper();

    switch (modeValue)
    {
      case ADJ_VOUCHER_CREDIT:
        {
          AdjVoucherCreditCtrl.Initialize(storeGridADJ.ClientID);
        }
        break;
      default:
        {
          adjVoucherDebitCtrl.Initialize(storeGridADJ.ClientID);
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
      case ADJ_VOUCHER_CREDIT:
        {
          AdjVoucherCreditCtrl.CommandPopulate(true, null, null, TYPE_ADJ_VOUCHER_CREDIT);
        }
        break;
      default:
        {
          adjVoucherDebitCtrl.CommandPopulate(true, null, null, TYPE_ADJ_VOUCHER_DEBIT);
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

      if (qryString.Equals("adjvouchercredit", StringComparison.OrdinalIgnoreCase))
      {
        AdjVoucherCreditCtrl.CommandPopulate(false, pID, null, TYPE_ADJ_VOUCHER_CREDIT);
      }
      else
      {
        adjVoucherDebitCtrl.CommandPopulate(false, pID, null, TYPE_ADJ_VOUCHER_DEBIT);
      }

    }
  }

  private PostDataParser.StructureResponse DeleteParser(string adjNumber, string ket)
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
    pair.DicAttributeValues.Add("Keterangan", ket.Trim());

    try
    {
      varData = parser.ParserData("ADJVOUCHER", "Delete", dic);
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
  public void DeleteMethod(string adjNumber, string keterangan)
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
    PostDataParser.StructureResponse respon = DeleteParser(adjNumber, keterangan);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r); }}",
        storeGridADJ.ClientID, adjNumber));

      Functional.ShowMsgInformation(string.Format("Nomor Adjust '{0}' telah terhapus.", adjNumber));
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
    }
  }
}
