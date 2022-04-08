using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_bonus_ClaimBonus : Scms.Web.Core.PageHandler
{
  private const string CLAIM_REGULAR = "REGULAR";
  private const string CLAIM_STT = "STT";
  private const string CLAIM_PROCESS = "PROCESS";

  private const string TYPE_CLAIM_REGULAR = "01";
  private const string TYPE_CLAIM_STT = "02";

  private Control MyLoadControl(bool Process)
  {
    Control c = null;

    const string CONT_NORMAL = "x1x1x1";
    const string CONT_PROCESS = "x1x1x1x1";

    if (Process == false)
    {
      c = phCtrl.FindControl(CONT_NORMAL);

      string modeValue = hfMode.Text.Trim().ToUpper();

      switch (modeValue)
      {
        case CLAIM_STT:

          if (c == null)
          {
            c = this.LoadControl("ClaimBonusSTTCtrl.ascx");

            c.ID = CONT_NORMAL;

            phCtrl.Controls.Add(c);
          }

          break;
        case CLAIM_PROCESS:

          if (c == null)
          {
            c = this.LoadControl("ClaimBonusProcessCtrl.ascx");

            c.ID = CONT_NORMAL;

            phCtrl.Controls.Add(c);
          }

          break;
        default:

          if (c == null)
          {
            c = this.LoadControl("ClaimBonusRegularCtrl.ascx");

            c.ID = CONT_NORMAL;

            phCtrl.Controls.Add(c);
          }

          break;
      }
    }
    else
    {
      c = phCtrl.FindControl(CONT_PROCESS);

      c = this.LoadControl("ClaimBonusRegularCtrl.ascx");

      c.ID = CONT_PROCESS;

      phCtrl.Controls.Add(c);
    }

    return c;
  }

  protected void Page_Init(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

      if (qryString.Equals("stt", StringComparison.OrdinalIgnoreCase))
      {
        hfMode.Text = CLAIM_STT;
        hfType.Text = TYPE_CLAIM_STT;
      }
      else if (qryString.Equals("process", StringComparison.OrdinalIgnoreCase))
      {
        hfMode.Text = CLAIM_PROCESS;
        hfType.Text = TYPE_CLAIM_REGULAR;
      }
      else
      {
        hfMode.Text = CLAIM_REGULAR;
        hfType.Text = TYPE_CLAIM_REGULAR;
      }
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    string modeValue = hfMode.Text.Trim().ToUpper();

    switch (modeValue)
    {
      case CLAIM_STT:
        {
          transaksi_bonus_ClaimBonusSTTCtrl opr = MyLoadControl(false) as transaksi_bonus_ClaimBonusSTTCtrl;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Claim STT tidak dapat dibuka.");
          }
          else
          {
            opr.Initialize(storeGridClaim.ClientID);
          }
        }
        break;
      case CLAIM_PROCESS:
        {
          transaksi_bonus_ClaimBonusProcessCtrl opr = MyLoadControl(false) as transaksi_bonus_ClaimBonusProcessCtrl;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Claim Proses tidak dapat dibuka.");
          }
          else
          {
            opr.Initialize(storeGridClaim.ClientID);
          }
        }
        break;
      default:
        {
          transaksi_bonus_ClaimBonusRegularCtrl opr = MyLoadControl(false) as transaksi_bonus_ClaimBonusRegularCtrl;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Claim Regular tidak dapat dibuka.");
          }
          else
          {
            opr.Initialize(storeGridClaim.ClientID);
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
    string gdgID = (e.ExtraParams["GudangID"] ?? string.Empty);

    string modeValue = hfMode.Text.Trim().ToUpper();

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {

      switch (modeValue)
      {
        case CLAIM_STT:
          {
            transaksi_bonus_ClaimBonusSTTCtrl opr = MyLoadControl(false) as transaksi_bonus_ClaimBonusSTTCtrl;

            if (opr == null)
            {
              Functional.ShowMsgError("Objek Claim STT Control tidak dapat dibuka.");
            }
            else
            {
              opr.CommandPopulate(false, pID);
            }
          }
          break;
        case CLAIM_PROCESS:
          {
            transaksi_bonus_ClaimBonusProcessCtrl opr = MyLoadControl(true) as transaksi_bonus_ClaimBonusProcessCtrl;

            if (opr == null)
            {
              ClaimBonusRegularCtrl.CommandPopulate(false, pID);
              //Functional.ShowMsgError("Objek Claim Process Control tidak dapat dibuka.");
            }
            else
            {
              
              opr.CommandPopulate(false, pID);
            }
          }
          break;
        default:
          {
            transaksi_bonus_ClaimBonusRegularCtrl opr = MyLoadControl(false) as transaksi_bonus_ClaimBonusRegularCtrl;

            if (opr == null)
            {
              Functional.ShowMsgError("Objek Claim Regular Control tidak dapat dibuka.");
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
      case CLAIM_STT:
        {
          transaksi_bonus_ClaimBonusSTTCtrl opr = MyLoadControl(false) as transaksi_bonus_ClaimBonusSTTCtrl;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek Claim STT Control tidak dapat dibuka.");
          }
          else
          {
            opr.CommandPopulate(true, null);
          }
        }
        break;
      case CLAIM_PROCESS:
        {
          transaksi_bonus_ClaimBonusProcessCtrl opr = MyLoadControl(false) as transaksi_bonus_ClaimBonusProcessCtrl;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek Claim STT Control tidak dapat dibuka.");
          }
          else
          {
            opr.CommandPopulate(true, null);
          }
        }
        break;
      default:
        {
          transaksi_bonus_ClaimBonusRegularCtrl opr = MyLoadControl(false) as transaksi_bonus_ClaimBonusRegularCtrl;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek Claim Regular Control tidak dapat dibuka.");
          }
          else
          {
            opr.CommandPopulate(true, null);
          }
        }
        break;
    }
  }

  private PostDataParser.StructureResponse DeleteParser(string orNumber, string ket)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = orNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", this.Nip);
    pair.DicAttributeValues.Add("Keterangan", ket.Trim());

    try
    {
      varData = parser.ParserData("CLAIM", "Delete", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_bonus_ClaimBonus DeleteParser : {0} ", ex.Message);
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
  public void DeleteMethod(string clNumber, string keterangan)
  {
    if (!this.IsAllowDelete)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menghapus data.");
      return;
    }

    if (string.IsNullOrEmpty(clNumber))
    {
      Functional.ShowMsgWarning("Nomor Claim tidak terbaca.");

      return;
    }
    else if (string.IsNullOrEmpty(keterangan))
    {
      Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");

      return;
    }

    PostDataParser.StructureResponse respon = DeleteParser(clNumber, keterangan);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r);{0}.commitChanges(); }}",
        storeGridClaim.ClientID, clNumber));

      Functional.ShowMsgInformation(string.Format("Nomor Claim '{0}' telah terhapus.", clNumber));
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
    }
  }
}
