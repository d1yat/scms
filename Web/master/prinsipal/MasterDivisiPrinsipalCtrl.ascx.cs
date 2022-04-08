using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class master_prinsipal_MasterDivisiPrinsipalCtrl : System.Web.UI.UserControl
{
  private void ClearEntrys()
  {
    winDetail.Title = "Master Divisi Prinsipal";

    hfNoDivSup.Clear();

    cbPrincipalHdr.Disabled = false;
    cbPrincipalHdr.Clear();

    txHet.Disabled = false;
    txHet.Clear();

    txIndexNonPareto.Clear();
    txIndexNonPareto.Disabled = false;

    txIndexPareto.Clear();
    txIndexPareto.Disabled = false;

    txName.Clear();
    txName.Disabled = false;

    chkAktif.Clear();
    chkAktif.Disabled = false;

    chkHide.Clear();
    chkHide.Disabled = false;
  }

  public void Initialize(string storeIDGridMain)
  {
    hfStoreID.Text = storeIDGridMain;
  }

  protected void Page_Load(object sender, EventArgs e)
  {

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

    dic.Add("DivSupplierID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("SupplierId", cbPrincipalHdr.Value.ToString());
    pair.DicAttributeValues.Add("Nama", txName.Text);
    pair.DicAttributeValues.Add("isAktif", chkAktif.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("isHide", chkHide.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("idxp", txIndexPareto.Text);
    pair.DicAttributeValues.Add("idxnp", txIndexNonPareto.Text);
    pair.DicAttributeValues.Add("het", txHet.Text);

    try
    {
      varData = parser.ParserData("MasterDivisiPrisipal", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("Master_Divisi_PrinsipalCtrl SaveParser : {0} ", ex.Message);
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
        string scrpt = null;

        string Nama = (txName.Text == null ? string.Empty : txName.Text);
        string Supplier = (cbPrincipalHdr.SelectedItem != null ? cbPrincipalHdr.SelectedItem.Text : string.Empty);
        string pareto = (txIndexPareto.Text == null ? "0" : txIndexPareto.Text);
        string noNpareto = (txIndexNonPareto.Text == null ? "0" : txIndexNonPareto.Text);
        string Het = (txHet.Text == null ? "0" : txHet.Text);
        bool Aktip = chkAktif.Checked;

        if (isAdd)
        {
          if (!string.IsNullOrEmpty(storeId))
          {
            scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_kddivpri': '{1}',
                'v_nmdivpri': '{2}',
                'v_nama': '{3}',
                'n_idxp': {4},
                'n_idxnp': {5},
                'n_het': {6},
                'l_aktif': true
              }}));{0}.commitChanges();", storeId, respon.Values.GetValueParser<string>("DivSupplierID", string.Empty),
                     Nama, Supplier, pareto, noNpareto, Het, Aktip);

            X.AddScript(scrpt);
          }
        }
        else
        {
          scrpt = string.Format(@"var rec = {0}.getById('{1}');
                                  if(!Ext.isEmpty(rec)) {{
                                    rec.set('v_nmdivpri', '{1}');
                                    rec.set('v_nama', '{2}');
                                    rec.set('n_idxp', '{3}');
                                    rec.set('n_idxnp', '{4}');
                                    rec.set('n_het', '{5}');
                                    rec.set('l_aktif', {6});
                                  }};{0}.commitChanges()", storeId, Nama, Supplier, pareto, noNpareto, Het, Aktip.ToString().ToLower());
          
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

  private void PopulateDetail(string pName, string pID)
  {
    ClearEntrys();

    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { pName +" = @0", pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0141", paramX);

    System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("id-ID");

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("Master Divisi Prinsipal - {0}", pID);

        Functional.SetComboData(cbPrincipalHdr, "c_nosup", dicResultInfo.GetValueParser<string>("v_nama", string.Empty), dicResultInfo.GetValueParser<string>("c_nosup", string.Empty));
        cbPrincipalHdr.Disabled = true;

        txName.Text = (dicResultInfo.ContainsKey("v_nmdivpri") ? dicResultInfo["v_nmdivpri"] : string.Empty);
        chkAktif.Checked = dicResultInfo.GetValueParser<bool>("l_aktif", false);
        chkHide.Checked = dicResultInfo.GetValueParser<bool>("l_hide", false);
        txHet.Text = dicResultInfo.GetValueParser<decimal>("n_het", 0).ToString(ci);
        txIndexPareto.Text = dicResultInfo.GetValueParser<decimal>("n_idxp", 0).ToString(ci);
        txIndexNonPareto.Text = dicResultInfo.GetValueParser<decimal>("n_idxnp", 0).ToString(ci);

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("MasterDivisiPrinsipalCtrl:PopulateDetail Header - ", ex.Message));
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

    hfNoDivSup.Text = pID;
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

      PopulateDetail("c_kddivpri", pID);
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
