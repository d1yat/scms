using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class master_cabang_hari_ctrl : System.Web.UI.UserControl
{
  private void ClearEntrys()
  {
    winDetail.Title = "Master Cabang Hari";
    
    hfNoExp.Clear();

  

    txName.Clear();
    txName.Disabled = false;

    chksenin.Clear();
    chksenin.Disabled = false;

    chkselasa.Clear();
    chkselasa.Disabled = false;

    chkrabu.Clear();
    chkrabu.Disabled = false;

    chkkamis.Clear();
    chkkamis.Disabled = false;

    chkjumat.Clear();
    chkjumat.Disabled = false;

    chksabtu.Clear();
    chksabtu.Disabled = false;


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
    pair.DicAttributeValues.Add("issenin", chksenin.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("isselasa", chkselasa.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("israbu", chkrabu.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("iskamis", chkkamis.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("isjumat", chkjumat.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("issabtu", chksabtu.Checked.ToString().ToLower());
      
    //pair.DicAttributeValues.Add("Npwp", txNpwp.Value.ToString());

    try
    {
        varData = parser.ParserData("MasterCabangHari", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("Master_CabangHari SaveParser : {0} ", ex.Message);
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
        new string[] { "c_cusno = @0", pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0149-a", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

      
        txName.Text = dicResultInfo.GetValueParser<string>("v_cunam");
        chksenin.Checked = dicResultInfo.GetValueParser<bool>("l_senin", false);
        chkselasa.Checked = dicResultInfo.GetValueParser<bool>("l_selasa", false);
        chkrabu.Checked = dicResultInfo.GetValueParser<bool>("l_rabu", false);
        chkkamis.Checked = dicResultInfo.GetValueParser<bool>("l_kamis", false);
        chkjumat.Checked = dicResultInfo.GetValueParser<bool>("l_jumat", false);
        chksabtu.Checked = dicResultInfo.GetValueParser<bool>("l_sabtu", false);

        //if (chkNpwp.Checked)
        //{
        //    txNpwp.Hidden = false;
        //    txNpwp.Text = dicResultInfo.GetValueParser<string>("c_npwp");
        //}

        winDetail.Title = string.Format("Master  - {0}", txName.Text);

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

        bool senin = bool.Parse((chksenin.Checked ? chksenin.Checked : false).ToString());
        bool selasa = bool.Parse((chkselasa.Checked ? chkselasa.Checked : false).ToString());
        bool rabu = bool.Parse((chkrabu.Checked ? chkrabu.Checked : false).ToString());
        bool kamis = bool.Parse((chkkamis.Checked ? chkkamis.Checked : false).ToString());
        bool jumat = bool.Parse((chkjumat.Checked ? chkjumat.Checked : false).ToString());
        bool sabtu = bool.Parse((chksabtu.Checked ? chksabtu.Checked : false).ToString());

        if (isAdd)
        {

          if (!string.IsNullOrEmpty(storeId))
          {
            scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_cusno': '{1}',
                'v_cunam': '{2}',
                'l_senin': {3},
                'l_selasa': {4},
                'l_rabu': {5},
                'l_kamis': {6},
                'l_jumat': (7),
                'l_sabtu': (8)

              }}));{0}.commitChanges();", storeId, respon.Values.GetValueParser<string>("ExpID", string.Empty),
                     Nama, 
                     senin.ToString().ToLower(), 
                     selasa.ToString().ToLower(), 
                     rabu.ToString().ToLower(), 
                     kamis.ToString().ToLower(), 
                     jumat.ToString().ToLower(),
                     sabtu.ToString().ToLower());

            X.AddScript(scrpt);
          }
        }
        else
        {
          scrpt = string.Format(@"var rec = {0}.getById('{1}');
                                  if(!Ext.isEmpty(rec)) {{
                                    rec.set('v_cunam', '{2}');
                                    rec.set('l_senin', {3});
                                    rec.set('l_selasa', {4});
                                    rec.set('l_rabu', {5});
                                    rec.set('l_kamis', {6});
                                    rec.set('l_jumat', {7});
                                    rec.set('l_sabtu', {8});

                                  }};{0}.commitChanges();",
                                storeId, numberId, 
                                Nama, 
                                senin.ToString().ToLower(),
                                selasa.ToString().ToLower(),
                                rabu.ToString().ToLower(),
                                kamis.ToString().ToLower(),
                                jumat.ToString().ToLower(),
                                sabtu.ToString().ToLower());

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
