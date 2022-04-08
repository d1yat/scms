using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_bonus_ClaimBonusAcc : Scms.Web.Core.PageHandler
{
  private const string CLAIM_ACC_REGULAR = "REGULAR";
  private const string CLAIM_ACC_STT = "STT";

  private const string TYPE_CLAIM_ACC_REGULAR = "01";
  private const string TYPE_CLAIM_ACC_STT = "02";

  private Control MyLoadControl()
  {
    Control c = null;

    const string CONT_NORMAL = "x1x1x1";

    c = phCtrl.FindControl(CONT_NORMAL);

    string modeValue = hfMode.Text.Trim().ToUpper();

    switch (modeValue)
    {
      case CLAIM_ACC_STT:

        if (c == null)
        {
          c = this.LoadControl("ClaimBonusAccSTTCtrl.ascx");

          c.ID = CONT_NORMAL;

          phCtrl.Controls.Add(c);
        }

        break;
      default:

        if (c == null)
        {
          c = this.LoadControl("ClaimBonusAccRegularCtrl.ascx");

          c.ID = CONT_NORMAL;

          phCtrl.Controls.Add(c);
        }

        break;
    }

    return c;
  }

  private PostDataParser.StructureResponse DeleteParser(string ClaimAccNo, string tipeClaim, string ket)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = ClaimAccNo;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", this.Nip);
    pair.DicAttributeValues.Add("Type", tipeClaim);
    pair.DicAttributeValues.Add("Keterangan", ket.Trim());

    try
    {
      varData = parser.ParserData("CLAIMACC", "Delete", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penerimaan_ClaimAcc DeleteParser : {0} ", ex.Message);
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
  public void DeleteMethod(string ClaimAccNo, string tipeClaim, string keterangan)
  {
    if (!this.IsAllowDelete)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menghapus data.");
      return;
    }

    if (string.IsNullOrEmpty(ClaimAccNo))
    {
      Functional.ShowMsgWarning("Nomor Claim tidak terbaca.");
      return;
    }
    else if (string.IsNullOrEmpty(tipeClaim))
    {
      Functional.ShowMsgWarning("Tipe Claim tidak terbaca.");
      return;
    }
    else if (string.IsNullOrEmpty(keterangan))
    {
      Functional.ShowMsgWarning("Keterangan tidak boleh kosong");
      return;
    }

    PostDataParser.StructureResponse respon = DeleteParser(ClaimAccNo, tipeClaim, keterangan);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r);{0}.commitChanges(); }}",
        storeGridClaimAcc.ClientID, ClaimAccNo));

      Functional.ShowMsgInformation(string.Format("Nomor RN '{0}' telah terhapus.", ClaimAccNo));
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
    }
  }

  protected void Page_Init(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

      if (qryString.Equals("stt", StringComparison.OrdinalIgnoreCase))
      {
        hfMode.Text = CLAIM_ACC_STT;
        hfType.Text = TYPE_CLAIM_ACC_STT;
      }
      else
      {
        hfMode.Text = CLAIM_ACC_REGULAR;
        hfType.Text = TYPE_CLAIM_ACC_REGULAR;
      }
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    string modeValue = hfMode.Text.Trim().ToUpper();

    switch (modeValue)
    {
      case CLAIM_ACC_STT:
        {
          transaksi_bonus_ClaimBonusAccSTTCtrl opr = MyLoadControl() as transaksi_bonus_ClaimBonusAccSTTCtrl;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol ACC Claim STT tidak dapat dibuka.");
          }
          else
          {
            opr.Initialize(storeGridClaimAcc.ClientID);
          }
        }
        break;
      default:
        {
          transaksi_bonus_ClaimBonusAccRegularCtrl opr = MyLoadControl() as transaksi_bonus_ClaimBonusAccRegularCtrl;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol ACC Claim Regular tidak dapat dibuka.");
          }
          else
          {
            opr.Initialize(storeGridClaimAcc.ClientID);
          }
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

      switch (modeValue)
      {
        case CLAIM_ACC_STT:
          {
            transaksi_bonus_ClaimBonusAccSTTCtrl opr = MyLoadControl() as transaksi_bonus_ClaimBonusAccSTTCtrl;

            if (opr == null)
            {
              Functional.ShowMsgError("Objek kontrol Claim ACC STT tidak dapat dibuka.");
            }
            else
            {
              opr.CommandPopulate(false, pID);
            }
          }
          break;
        default:
          {
            transaksi_bonus_ClaimBonusAccRegularCtrl opr = MyLoadControl() as transaksi_bonus_ClaimBonusAccRegularCtrl;

            if (opr == null)
            {
              Functional.ShowMsgError("Objek kontrol Claim ACC Regular tidak dapat dibuka.");
            }
            else
            {
              opr.CommandPopulate(false, pID);
            }
          }
          break;
      }
    }

    GC.Collect();
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
      case CLAIM_ACC_STT:
        {
          transaksi_bonus_ClaimBonusAccSTTCtrl opr = MyLoadControl() as transaksi_bonus_ClaimBonusAccSTTCtrl;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Claim ACC STT tidak dapat dibuka.");
          }
          else
          {
            opr.CommandPopulate(true, null);
          }
        }
        break;
      default:
        {
          transaksi_bonus_ClaimBonusAccRegularCtrl opr = MyLoadControl() as transaksi_bonus_ClaimBonusAccRegularCtrl;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Claim ACC Regular tidak dapat dibuka.");
          }
          else
          {
            opr.CommandPopulate(true, null);
          }
        }
        break;
    }
  }
}
