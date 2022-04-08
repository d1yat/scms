using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_penerimaan_ReceiveNote_Ulujami : Scms.Web.Core.PageHandler
{
  private const string RN_PEMBELIAN = "PEMBELIANULUJAMI";
  private const string RN_KHUSUS = "KHUSUSULUJAMI";
  private const string RN_RETUR = "RETURULUJAMI";
  private const string RN_CLAIM = "CLAIMULUJAMI";
  private const string RN_REPACK = "REPACKULUJAMI";
  private const string RN_TRANSFER = "TRANSFERULUJAMI";

  private const string RN_CLAIM_KET = "RelokasiUJClaim03";
  private const string TYPE_RN_PEMBELIAN = "05";
  private const string TYPE_RN_RETUR = "02";
  private const string TYPE_RN_CLAIM = "05";
  private const string TYPE_RN_REPACK = "04";
  private const string TYPE_RN_TRANSFER = "05";
  private const string TYPE_RN_KHUSUS = "05";

  private const string TYPE_RN_KHUSUS_STATUS = "true";

  private PostDataParser.StructureResponse DeleteParser(string gdg, string rnNumber, string tipeRn, string ket)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = rnNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", this.Nip);
    pair.DicAttributeValues.Add("Gdg", gdg);
    pair.DicAttributeValues.Add("TypeRN", tipeRn);
    pair.DicAttributeValues.Add("Keterangan", ket.Trim());

    try
    {
      varData = parser.ParserData("RNPembelian", "Delete", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penerimaan_ReceiveNote DeleteParser : {0} ", ex.Message);
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

  private Control MyLoadControl()
  {
    Control c = null;

    const string CONT_NORMAL = "x1x1x1";

    c = phCtrl.FindControl(CONT_NORMAL);

    string modeValue = hfMode.Text.Trim().ToUpper();

    switch (modeValue)
    {
      case RN_KHUSUS:

        if (c == null)
        {
          c = this.LoadControl("RNKhususCtrlUlujami.ascx");  

          c.ID = CONT_NORMAL;

          phCtrl.Controls.Add(c);
        }

        break;
      case RN_CLAIM:

        if (c == null)
        {
          c = this.LoadControl("RNClaimCtrlUlujami.ascx"); 

          c.ID = CONT_NORMAL;

          phCtrl.Controls.Add(c);
        }

        break;
      case RN_RETUR:

        if (c == null)
        {
          c = this.LoadControl("RNRetur.ascx");

          c.ID = CONT_NORMAL;

          phCtrl.Controls.Add(c);
        }

        break;
      case RN_REPACK:

        if (c == null)
        {
          c = this.LoadControl("RNRepack.ascx");

          c.ID = CONT_NORMAL;

          phCtrl.Controls.Add(c);
        }

        break;
      default:

        if (c == null)
        {
          c = this.LoadControl("RNPembelianCtrlUlujami.ascx");

          c.ID = CONT_NORMAL;

          phCtrl.Controls.Add(c);
        }

        break;
    }

    return c;
  }

  [DirectMethod(ShowMask = true)]
  public void DeleteMethod(string gudang, string rnNumber, string tipeRN, string keterangan)
  {
    if (!this.IsAllowDelete)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menghapus data.");
      return;
    }

    if (string.IsNullOrEmpty(gudang))
    {
      Functional.ShowMsgWarning("Gudang tidak terbaca.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Gudang tidak terbaca.";

      return;
    }
    else if (string.IsNullOrEmpty(rnNumber))
    {
      Functional.ShowMsgWarning("Nomor Receive tidak terbaca.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Nomor PO tidak terbaca.";

      return;
    }
    else if (string.IsNullOrEmpty(rnNumber))
    {
      Functional.ShowMsgWarning("Tipe Receive tidak terbaca.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Nomor PO tidak terbaca.";

      return;
    }
    else if (string.IsNullOrEmpty(keterangan))
    {
      Functional.ShowMsgWarning("Keterangan tidak boleh kosong");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Keterangan tidak boleh kosong.";

      return;
    }

    PostDataParser.StructureResponse respon = DeleteParser(gudang, rnNumber, tipeRN, keterangan);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r);{0}.commitChanges(); }}",
        storeGridRN.ClientID, rnNumber));

      Functional.ShowMsgInformation(string.Format("Nomor RN '{0}' telah terhapus.", rnNumber));
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = respon.Message;
    }
  }

  protected void Page_Init(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
   {
      string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

      if (qryString.Equals("khususulujami", StringComparison.OrdinalIgnoreCase))
      {
        hfMode.Text = RN_KHUSUS;
        hfType.Text = TYPE_RN_KHUSUS;
        hfRNKhusus.Text = TYPE_RN_KHUSUS_STATUS;
      }
      else if (qryString.Equals("retur", StringComparison.OrdinalIgnoreCase))
      {
        hfMode.Text = RN_RETUR;
        hfType.Text = TYPE_RN_RETUR;
        hfRNKhusus.Text = "false";
      }
      else if (qryString.Equals("claimulujami", StringComparison.OrdinalIgnoreCase))
      {
        hfMode.Text = RN_CLAIM;
        hfType.Text = TYPE_RN_CLAIM;
        hfRNKhusus.Text = "false";
        hfKet.Text = RN_CLAIM_KET;
      }
      else if (qryString.Equals("repack", StringComparison.OrdinalIgnoreCase))
      {
        hfMode.Text = RN_REPACK;
        hfType.Text = TYPE_RN_REPACK;
        hfRNKhusus.Text = "false";
      }
      else
      {
        hfMode.Text = RN_PEMBELIAN;
        hfType.Text = TYPE_RN_PEMBELIAN;
        hfRNKhusus.Text = "false";
      }
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    string modeValue = hfMode.Text.Trim().ToUpper();

   
    switch (modeValue)
    {
      case RN_KHUSUS:
        {
          #region Khusus

            transaksi_penerimaan_RNKhususCtrl_Ulujami opr = MyLoadControl() as transaksi_penerimaan_RNKhususCtrl_Ulujami;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Receive Note Khusus tidak dapat dibuka.");
          }
          else
          {
            opr.Initialize(storeGridRN.ClientID);
          }

          #endregion
        }
        break;
      case RN_RETUR:
        {
          #region Retur

          transaksi_penerimaan_RNRetur opr = MyLoadControl() as transaksi_penerimaan_RNRetur;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Receive Note Retur tidak dapat dibuka.");
          }
          else
          {
            opr.Initialize(storeGridRN.ClientID);
          }

          #endregion
        }
        break;
      case RN_CLAIM:
        {
          #region Claim

            transaksi_penerimaan_RNClaimCtrlUlujami opr = MyLoadControl() as transaksi_penerimaan_RNClaimCtrlUlujami;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Receive Note Claim tidak dapat dibuka.");
          }
          else
          {
            opr.Initialize(storeGridRN.ClientID);
          }

          #endregion
        }
        break;
      case RN_REPACK:
        {
          #region Repack

          transaksi_penerimaan_RNRepack opr = MyLoadControl() as transaksi_penerimaan_RNRepack;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Receive Note Repack tidak dapat dibuka.");
          }
          else
          {
            opr.Initialize(storeGridRN.ClientID);
          }

          #endregion
        }
        break;
      default:
        {
          #region Beli

          transaksi_penerimaan_RNPembelianCtrl_Ulujami opr = MyLoadControl() as transaksi_penerimaan_RNPembelianCtrl_Ulujami;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Receive Note Pembelian tidak dapat dibuka.");
          }
          else
          {
            opr.Initialize(storeGridRN.ClientID);
          }

          #endregion
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
      //PurchaseOrderCtrl1.CommandPopulate(false, pID);

      switch (modeValue)
      {
        case RN_KHUSUS:
          {
              transaksi_penerimaan_RNKhususCtrl_Ulujami opr = MyLoadControl() as transaksi_penerimaan_RNKhususCtrl_Ulujami;

            if (opr == null)
            {
              Functional.ShowMsgError("Objek kontrol Receive Note Khusus tidak dapat dibuka.");
            }
            else
            {
              opr.CommandPopulate(false, pID, gdgID);
            }
          }
          break;
        case RN_RETUR:
          {
            transaksi_penerimaan_RNRetur opr = MyLoadControl() as transaksi_penerimaan_RNRetur;

            if (opr == null)
            {
              Functional.ShowMsgError("Objek kontrol Receive Note Retur tidak dapat dibuka.");
            }
            else
            {
              opr.CommandPopulate(false, pID, gdgID);
            }
          }
          break;
        case RN_CLAIM:
          {
              transaksi_penerimaan_RNClaimCtrlUlujami opr = MyLoadControl() as transaksi_penerimaan_RNClaimCtrlUlujami;

            if (opr == null)
            {
              Functional.ShowMsgError("Objek kontrol Receive Note Claim tidak dapat dibuka.");
            }
            else
            {
              opr.CommandPopulate(false, pID, gdgID);
            }
          }
          break;
        case RN_REPACK:
          {
            transaksi_penerimaan_RNRepack opr = MyLoadControl() as transaksi_penerimaan_RNRepack;

            if (opr == null)
            {
              Functional.ShowMsgError("Objek kontrol Receive Note Repack tidak dapat dibuka.");
            }
            else
            {
              opr.CommandPopulate(false, pID, gdgID);
            }
          }
          break;
        default:
          {
              transaksi_penerimaan_RNPembelianCtrl_Ulujami opr = MyLoadControl() as transaksi_penerimaan_RNPembelianCtrl_Ulujami;

            if (opr == null)
            {
              Functional.ShowMsgError("Objek kontrol Receive Note Pembelian tidak dapat dibuka.");
            }
            else
            {
              opr.CommandPopulate(false, pID, gdgID);
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
      case RN_KHUSUS:
        {
            transaksi_penerimaan_RNKhususCtrl_Ulujami opr = MyLoadControl() as transaksi_penerimaan_RNKhususCtrl_Ulujami;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Receive Note Khusus tidak dapat dibuka.");
          }
          else
          {
            opr.CommandPopulate(true, null, null);
          }
        }
        break;
      case RN_RETUR:
        {
          transaksi_penerimaan_RNRetur opr = MyLoadControl() as transaksi_penerimaan_RNRetur;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Receive Note Retur tidak dapat dibuka.");
          }
          else
          {
            opr.CommandPopulate(true, null, null);
          }
        }
        break;
      case RN_CLAIM:
        {
            transaksi_penerimaan_RNClaimCtrlUlujami opr = MyLoadControl() as transaksi_penerimaan_RNClaimCtrlUlujami;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Receive Note Claim tidak dapat dibuka.");
          }
          else
          {
            opr.CommandPopulate(true, null, null);
          }
        }
        break;
      case RN_REPACK:
        {
          transaksi_penerimaan_RNRepack opr = MyLoadControl() as transaksi_penerimaan_RNRepack;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Receive Note Repack tidak dapat dibuka.");
          }
          else
          {
            opr.CommandPopulate(true, null, null);
          }
        }
        break;
      default:
        {
            transaksi_penerimaan_RNPembelianCtrl_Ulujami opr = MyLoadControl() as transaksi_penerimaan_RNPembelianCtrl_Ulujami;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Receive Note Pembelian tidak dapat dibuka.");
          }
          else
          {
            opr.CommandPopulate(true, null, null);
          }
        }
        break;
    }
  }
}
