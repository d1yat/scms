using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class master_prinsipal_MasterPrinsipalCtrl : System.Web.UI.UserControl
{
  private void ClearEntrys()
  {
    winDetail.Title = "Master Prinsipal";

    hfNoSup.Clear();

    txAcc.Clear();
    txAcc.Disabled = false;

    txAcc1.Clear();
    txAcc1.Disabled = false;

    txAcc2.Clear();
    txAcc2.Disabled = false;

    chkAktif.Clear();
    chkAktif.Disabled = false;

    txAlamat1.Clear();
    txAlamat1.Disabled = false;

    txAlamat2.Clear();
    txAlamat2.Disabled = false;

    txAlamatBank.Clear();
    txAlamatBank.Disabled = false;

    txArea.Clear();
    txArea.Disabled = false;

    txBank.Clear();
    txBank.Disabled = false;

    txContact.Clear();
    txContact.Disabled = false;

    txDisc.Clear();
    txDisc.Disabled = false;

    chkFax.Clear();
    chkFax.Disabled = false;

    txFax1.Clear();
    txFax1.Disabled = false;

    txFax2.Clear();
    txFax2.Disabled = false;

    txFax3.Clear();
    txFax3.Disabled = false;

    chkImport.Clear();
    chkImport.Disabled = false;

    txNama.Clear();
    txNama.Disabled = false;

    txIndex.Clear();
    txIndex.Disabled = false;

    txKodeGol.Clear();
    txKodeGol.Disabled = false;

    chkKons.Clear();
    chkKons.Disabled = false;

    txLead.Clear();
    txLead.Disabled = false;

    txNamaTax.Clear();
    txNamaTax.Disabled = false;

    txNppkp.Clear();
    txNppkp.Disabled = false;

    txNpwp.Clear();
    txNpwp.Disabled = false;

    txOwner.Clear();
    txOwner.Disabled = false;

    txPhone1.Clear();
    txPhone1.Disabled = false;

    txPhone2.Clear();
    txPhone2.Disabled = false;

    txPhone3.Clear();
    txPhone3.Disabled = false;

    txTax.Clear();
    txTax.Disabled = false;

    txTop.Clear();
    txTop.Disabled = false;

    dtNppkp.Clear();
    dtNppkp.Disabled = false;

    txAlamatTax1.Clear();
    txAlamatTax1.Disabled = false;

    txAlamatTax2.Clear();
    txAlamatTax2.Disabled = false;

    txNamaTax.Clear();
    txNamaTax.Disabled = false;
  }

  public void Initialize(string storeIDGridMain)
  {
    hfStoreID.Text = storeIDGridMain;
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  private void PopulateDetail(string pName, string pID)
  {
    ClearEntrys();

    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "c_nosup = @0", pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0152", paramX);

    System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("id-ID");

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        DateTime date = DateTime.MinValue;        

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("Master Prinsipal - {0}", pID);
		
        date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_tglpkp"));
		
        txAcc.Text = ((dicResultInfo.ContainsKey("v_acccode") ? dicResultInfo["v_acccode"] : string.Empty));
        txAcc1.Text = ((dicResultInfo.ContainsKey("v_accno1") ? dicResultInfo["v_accno1"] : string.Empty));
        txAcc2.Text = ((dicResultInfo.ContainsKey("v_accno2") ? dicResultInfo["v_accno2"] : string.Empty));
        txAlamat1.Text = ((dicResultInfo.ContainsKey("v_alamat1") ? dicResultInfo["v_alamat1"] : string.Empty));
        txAlamat2.Text = ((dicResultInfo.ContainsKey("v_alamat2") ? dicResultInfo["v_alamat2"] : string.Empty));
        txAlamatBank.Text = ((dicResultInfo.ContainsKey("v_alamatbank") ? dicResultInfo["v_alamatbank"] : string.Empty));
        txArea.Text = ((dicResultInfo.ContainsKey("c_area") ? dicResultInfo["c_area"] : string.Empty));
        txBank.Text = ((dicResultInfo.ContainsKey("v_namabank") ? dicResultInfo["v_namabank"] : string.Empty));
        txContact.Text = ((dicResultInfo.ContainsKey("v_contact") ? dicResultInfo["v_contact"] : string.Empty));
        txDisc.Text = dicResultInfo.GetValueParser<decimal>("n_xdisc", 0).ToString();
        txFax1.Text = ((dicResultInfo.ContainsKey("v_fax1") ? dicResultInfo["v_fax1"] : string.Empty));
        txFax2.Text = ((dicResultInfo.ContainsKey("v_fax2") ? dicResultInfo["v_fax2"] : string.Empty));
        txFax3.Text = ((dicResultInfo.ContainsKey("v_fax3") ? dicResultInfo["v_fax3"] : string.Empty));
        txIndex.Text = dicResultInfo.GetValueParser<decimal>("n_index", decimal.Zero).ToString(ci);
        txKodeGol.Text = ((dicResultInfo.ContainsKey("c_kdgol") ? dicResultInfo["c_kdgol"] : string.Empty));
        txLead.Text = ((dicResultInfo.ContainsKey("n_leadtime") ? dicResultInfo["n_leadtime"] : string.Empty));
        txLead.ReadOnly = true;
        txNamaTax.Text = ((dicResultInfo.ContainsKey("v_namatax") ? dicResultInfo["v_namatax"] : string.Empty));
        txNppkp.Text = ((dicResultInfo.ContainsKey("v_nppkp") ? dicResultInfo["v_nppkp"] : string.Empty));
        txNpwp.Text = ((dicResultInfo.ContainsKey("v_npwp") ? dicResultInfo["v_npwp"] : string.Empty));
        txOwner.Text = ((dicResultInfo.ContainsKey("v_boss") ? dicResultInfo["v_boss"] : string.Empty));
        txPhone1.Text = ((dicResultInfo.ContainsKey("v_telepon1") ? dicResultInfo["v_telepon1"] : string.Empty));
        txPhone2.Text = ((dicResultInfo.ContainsKey("v_telepon2") ? dicResultInfo["v_telepon2"] : string.Empty));
        txPhone3.Text = ((dicResultInfo.ContainsKey("v_telepon3") ? dicResultInfo["v_telepon3"] : string.Empty));
        txTax.Text = ((dicResultInfo.ContainsKey("v_taxseri") ? dicResultInfo["v_taxseri"] : string.Empty));
        txTop.Text = ((dicResultInfo.ContainsKey("n_top") ? dicResultInfo["n_top"] : string.Empty));
        chkAktif.Checked = dicResultInfo.GetValueParser<bool>("l_aktif", false);
        chkFax.Checked = dicResultInfo.GetValueParser<bool>("l_fax", false);
        chkImport.Checked = dicResultInfo.GetValueParser<bool>("l_import", false);
        chkKons.Checked = dicResultInfo.GetValueParser<bool>("l_konsinyasi", false);
        chkisHide.Checked = dicResultInfo.GetValueParser<bool>("l_hide", false);
        txNama.Text = ((dicResultInfo.ContainsKey("v_nama") ? dicResultInfo["v_nama"] : string.Empty));
        txAlamatTax1.Text = ((dicResultInfo.ContainsKey("v_alamat1_tax") ? dicResultInfo["v_alamat1_tax"] : string.Empty));
        txAlamatTax2.Text = ((dicResultInfo.ContainsKey("v_alamat2_tax") ? dicResultInfo["v_alamat2_tax"] : string.Empty));
        dtNppkp.Text = date.ToString("dd-MM-yyyy");

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("MasterPrinsipalCtrl:PopulateDetail Header - ", ex.Message));
    }
    finally
    {
      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicResultInfo != null)
      {
        dicResultInfo.Clear();
      }
      if (dicResult != null)
      {
        dicResult.Clear();
      }
    }

    #endregion

    hfNoSup.Text = pID;
    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(string sSupID, bool isAdd)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();


    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = sSupID;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("SupplierID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Acc", txAcc.Text);
    pair.DicAttributeValues.Add("Acc1", txAcc1.Text);
    pair.DicAttributeValues.Add("Acc2", txAcc2.Text);
    pair.DicAttributeValues.Add("Alamat1", txAlamat1.Text);
    pair.DicAttributeValues.Add("Alamat2", txAlamat2.Text);
    pair.DicAttributeValues.Add("AlamatBank", txAlamatBank.Text);
    pair.DicAttributeValues.Add("Area", txArea.Text);
    pair.DicAttributeValues.Add("Bank", txBank.Text);
    pair.DicAttributeValues.Add("Contact", txContact.Text);
    pair.DicAttributeValues.Add("Disc", (string.IsNullOrEmpty(txDisc.Text) ? decimal.Zero.ToString() : txDisc.Text));
    pair.DicAttributeValues.Add("Fax1", txFax1.Text);
    pair.DicAttributeValues.Add("Fax2", txFax2.Text);
    pair.DicAttributeValues.Add("Fax3", txFax3.Text);
    pair.DicAttributeValues.Add("Index", (string.IsNullOrEmpty(txIndex.Text) ? decimal.Zero.ToString() : txIndex.Text));
    pair.DicAttributeValues.Add("KodeGol", txKodeGol.Text);
    pair.DicAttributeValues.Add("Lead", (string.IsNullOrEmpty(txLead.Text) ? decimal.Zero.ToString() : txLead.Text));
    pair.DicAttributeValues.Add("Nama", txNama.Text);
    pair.DicAttributeValues.Add("Nppkp", txNppkp.Text);
    pair.DicAttributeValues.Add("Npwp", txNpwp.Text);
    pair.DicAttributeValues.Add("Owner", txOwner.Text);
    pair.DicAttributeValues.Add("Phone1", txPhone1.Text);
    pair.DicAttributeValues.Add("Phone2", txPhone2.Text);
    pair.DicAttributeValues.Add("Phone3", txPhone3.Text);
    pair.DicAttributeValues.Add("Tax", txTax.Text);
    pair.DicAttributeValues.Add("Top", (string.IsNullOrEmpty(txTop.Text) ? decimal.Zero.ToString() : txTop.Text));
    pair.DicAttributeValues.Add("IsAktif", chkAktif.Value.ToString().ToLower());
    pair.DicAttributeValues.Add("IsFax", chkFax.Value.ToString().ToLower());
    pair.DicAttributeValues.Add("IsImport", chkImport.Value.ToString().ToLower());
    pair.DicAttributeValues.Add("IsKons", chkKons.Value.ToString().ToLower());
    pair.DicAttributeValues.Add("IsHide", chkisHide.Value.ToString().ToLower());
    pair.DicAttributeValues.Add("Date", dtNppkp.Value.ToString());
    pair.DicAttributeValues.Add("AlamatTax1", txAlamatTax1.Text);
    pair.DicAttributeValues.Add("AlamatTax2", txAlamatTax2.Text);
    pair.DicAttributeValues.Add("NamaTax", txNamaTax.Text);
    //Indra 20180815FM
    pair.DicAttributeValues.Add("TipePerubahan", "02");   
 

    try
    {
      varData = parser.ParserData("MasterPrisipal", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("Master_PrinsipalCtrl SaveParser : {0} ", ex.Message);
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
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    PostDataParser.StructureResponse respon = SaveParser(numberId, isAdd);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        if (isAdd)
        {
          string scrpt = null;

          string Nama = (txNama.Text == null ? string.Empty : txNama.Text);
          string Pemilik = (txOwner.Text == null ? string.Empty : txOwner.Text);
          string Cont = (txContact.Text == null ? string.Empty : txContact.Text);
          string Alamat = (txAlamat1.Text == null ? string.Empty : txAlamat1.Text);
          string Telp = (txPhone1.Text == null ? string.Empty : txPhone1.Text);
          string Fax = (txFax1 == null ? string.Empty : txFax1.Text);
          bool Aktip = (chkAktif.Checked ? chkAktif.Checked : false);

          bool dataAk = Convert.ToBoolean(Aktip.ToString().ToLower());

          if (!string.IsNullOrEmpty(storeId))
          {
            scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_nosup': '{1}',
                'v_nama': '{2}',
                'v_boss': '{3}',
                'v_contact': '{4}',
                'v_alamat1': '{5}',
                'v_telepon1': '{6}',
                'v_fax1': '{7}',
                'l_aktif': true
              }}));{0}.commitChanges();", storeId, respon.Values.GetValueParser<string>("SupplierID", string.Empty),
                     Nama, Pemilik, Cont, Alamat, Telp, Fax,
                     dataAk);

            X.AddScript(scrpt);
          }
        }

        this.ClearEntrys();

        Functional.ShowMsgInformation("Data berhasil tersimpan.");
      }
      else
      {
        e.ErrorMessage = respon.Message;

        e.Success = false;
      }
    }
    else
    {
      e.ErrorMessage = "Unknown response";

      e.Success = false;
    }
  }

  public void CommandPopulate(bool isAdd, string pID)
  {
    winDetail.Hidden = false;

    if (isAdd)
    {
      ClearEntrys();

      btnSimpan.Text = "Simpan";
    }
    else
    {      
      PopulateDetail("c_nosup", pID);
    }
  }

  protected void ReloadBtn_Click(object sender, DirectEventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    this.ClearEntrys();
  }
}
