using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class master_expedisi_MasterExpedisiCtrl : System.Web.UI.UserControl
{
  private void ClearEntrys()
  {
    winDetail.Title = "Master Expedisi";
    
    hfNoExp.Clear();

    txName.Clear();
    txName.Disabled = false;

    chkAktif.Clear();
    chkAktif.Disabled = false;

    chkDarat.Clear();
    chkDarat.Disabled = false;

    chkImport.Clear();
    chkImport.Disabled = false;

    chkLaut.Clear();
    chkLaut.Disabled = false;

    chkUdara.Clear();
    chkUdara.Disabled = false;
      
    chkNpwp.Clear();
    chkNpwp.Disabled = false;

    txNpwp.Clear();
    txNpwp.Disabled = false;
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  public void Initialize(string storeIDGridMain)
  {
    hfStoreID.Text = storeIDGridMain;
  }

  private PostDataParser.StructureResponse SaveParser(string sDivSupID, bool isAdd)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();


    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = sDivSupID;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("ExpID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Nama", txName.Value.ToString());
    pair.DicAttributeValues.Add("isAktif", chkAktif.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("isDarat", chkDarat.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("isUdara", chkUdara.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("isLaut", chkLaut.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("isImport", chkImport.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("isNpwp", chkNpwp.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("Npwp", txNpwp.Value.ToString());

    try
    {
      varData = parser.ParserData("MasterExpedisi", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("Master_ExpedisiCtrl SaveParser : {0} ", ex.Message);
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

  private void PopulateDetail(string pID)
  {
    ClearEntrys();

    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "c_exp = @0", pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0149", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        txName.Text = dicResultInfo.GetValueParser<string>("v_ket");
        chkAktif.Checked = dicResultInfo.GetValueParser<bool>("l_aktif", false);
        chkDarat.Checked = dicResultInfo.GetValueParser<bool>("l_darat", false);
        chkImport.Checked = dicResultInfo.GetValueParser<bool>("l_import", false);
        chkLaut.Checked = dicResultInfo.GetValueParser<bool>("l_laut", false);
        chkUdara.Checked = dicResultInfo.GetValueParser<bool>("l_udara", false);
        chkNpwp.Checked = dicResultInfo.GetValueParser<bool>("l_npwp", false);
        if (chkNpwp.Checked)
        {
            txNpwp.Hidden = false;
            txNpwp.Text = dicResultInfo.GetValueParser<string>("c_npwp");
        }

        winDetail.Title = string.Format("Master Expedisi - {0}", txName.Text);

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("MasterCustomerCtrl:PopulateDetail Header - ", ex.Message));
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

    hfNoExp.Text = pID;
    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  public void CommandPopulate(bool isAdd, string pID)
  {
    if (isAdd)
    {
      ClearEntrys();
      winDetail.Hidden = false;
    }
    else
    {
      winDetail.Hidden = false;

      PopulateDetail(pID);
    }
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
        string scrpt = null;

        string Nama = (txName.Text == null ? string.Empty : txName.Text);

        bool Aktip = bool.Parse((chkAktif.Checked ? chkAktif.Checked : false).ToString());
        bool Darat = bool.Parse((chkDarat.Checked ? chkDarat.Checked : false).ToString());
        bool Import = bool.Parse((chkImport.Checked ? chkImport.Checked : false).ToString());
        bool Laut = bool.Parse((chkLaut.Checked ? chkLaut.Checked : false).ToString());
        bool Udara = bool.Parse((chkUdara.Checked ? chkUdara.Checked : false).ToString());

        if (isAdd)
        {

          if (!string.IsNullOrEmpty(storeId))
          {
            scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_exp': '{1}',
                'v_ket': '{2}',
                'l_udara': {3},
                'l_darat': {4},
                'l_laut': {5},
                'l_import': {6},
                'l_aktif': true
              }}));{0}.commitChanges();", storeId, respon.Values.GetValueParser<string>("ExpID", string.Empty),
                     Nama, Udara.ToString().ToLower(), Darat.ToString().ToLower(), Laut.ToString().ToLower(), Import.ToString().ToLower(), Aktip.ToString().ToLower());

            X.AddScript(scrpt);
          }
        }
        else
        {
          scrpt = string.Format(@"var rec = {0}.getById('{1}');
                                  if(!Ext.isEmpty(rec)) {{
                                    rec.set('v_ket', '{2}');
                                    rec.set('l_udara', {3});
                                    rec.set('l_darat', {4});
                                    rec.set('l_laut', {5});
                                    rec.set('l_import', {6});
                                    rec.set('l_aktif', {7});
                                  }};{0}.commitChanges();",
                                storeId, numberId, Nama, Udara.ToString().ToLower(),
                                Darat.ToString().ToLower(),
                                Laut.ToString().ToLower(),
                                Import.ToString().ToLower(),
                                Aktip.ToString().ToLower());

          X.AddScript(scrpt);
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
