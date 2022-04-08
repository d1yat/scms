using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class master_ams_MasterDivisiAMSCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Master Divisi Prinsipal";

    hfDivAMSId.Clear();
    cbItemDtl.Clear();
    txNamaDivAMS.Clear();
    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private PostDataParser.StructureResponse SaveParser(string sDivSupID, bool isAdd, string Nama, string Aktif, string Hide, Dictionary<string, string>[] dics)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();
    Dictionary<string, string> dicData = null;
    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = sDivSupID;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string tmp = null,
      item = null,
      ket = null;
    //decimal nQty = 0;
    bool isNew = false,
      isVoid = false,
      isModify = false;
    string varData = null;

    dic.Add("DivAMSID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("NamaDiv", txNamaDivAMS.Text);
    pair.DicAttributeValues.Add("Aktif", chkAktif.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("Hide", chkHide.Checked.ToString().ToLower());

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      isNew = dicData.GetValueParser<bool>("l_new");
      isVoid = dicData.GetValueParser<bool>("l_void");
      isModify = dicData.GetValueParser<bool>("l_modified");

      if (isNew && (!isVoid) && (!isModify))
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");

        if ((!string.IsNullOrEmpty(item)))
        {
          dicAttr.Add("Item", item);
          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr
          });
        }
      }
      else if ((!isNew) && isVoid && (!isModify))
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        ket = dicData.GetValueParser<string>("v_ket");

        if ((!string.IsNullOrEmpty(item)))
        {
          dicAttr.Add("Item", item);

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
            DicAttributeValues = dicAttr
          });
        }
      }
    }

    try
    {
      varData = parser.ParserData("MasterDivAMS", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("master_ams_MasterDivisiAMSCtrl SaveParser : {0} ", ex.Message);
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

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0201", paramX);

    System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("id-ID");

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("Master Divisi AMS - {0}", pID);

        chkAktif.Checked = dicResultInfo.GetValueParser<bool>("l_aktif", false);
        chkHide.Checked = dicResultInfo.GetValueParser<bool>("l_hide", false);
        txNamaDivAMS.Text = (dicResultInfo.ContainsKey("v_nmdivams") ? dicResultInfo["v_nmdivams"] : string.Empty);

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
      hfDivAMSId.Text = pID;
    }

    #endregion

    #region Parser Detail

    try
    {
      Ext.Net.Store store = gridDetail.GetStore();
      if (store.Proxy.Count > 0)
      {
        Ext.Net.ScriptTagProxy stp = store.Proxy[0] as Ext.Net.ScriptTagProxy;
        if ((stp != null) && stp.CallbackParam.Equals(Functional.NAME_SOA_SCMS_CALLBACK, StringComparison.OrdinalIgnoreCase))
        {
          string param = (store.BaseParams["parameters"] ?? string.Empty);
          if (string.IsNullOrEmpty(param))
          {
            store.BaseParams.Add(new Ext.Net.Parameter("parameters", string.Format("[['{0} = @0', '{1}', 'System.String']]", pName, pID), ParameterMode.Raw));
          }
          else
          {
            store.BaseParams["parameters"] = string.Format("[['{0} = @0', '{1}', 'System.String']]", pName, pID);
          }
        }
      }

      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("master_ams_MasterDivisiAMSCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  #endregion

  public void Initialize(string storeIDGridMain)
  {
    hfStoreID.Text = storeIDGridMain;
  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);
    string Nama = (e.ExtraParams["Nama"] ?? string.Empty);
    string Aktif = (e.ExtraParams["Aktif"] ?? string.Empty);
    string Hide = (e.ExtraParams["Hide"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    Dictionary<string, string>[] gridDataDiv = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    PostDataParser.StructureResponse respon = SaveParser(numberId, isAdd, Nama, Aktif, Hide, gridDataDiv);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        if (isAdd)
        {
          string scrpt = null;

          scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_kddivams': '{1}',
                'v_nmdivams': '{2}',
                'l_aktif': {3},
                'l_hide': {4}
              }}));{0}.commitChanges();", storeId, respon.Values.GetValueParser<string>("DIVID", string.Empty),
                       Nama, Aktif.ToLower(), Hide.ToLower());

              X.AddScript(scrpt);

              this.ClearEntrys();
          }
          else
          {
              PopulateDetail("c_kddivams", numberId);
          }
        //PopulateDetail("c_kddivams", numberId);
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

  protected void Page_Load(object sender, EventArgs e)
  {

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

      PopulateDetail("c_kddivams", pID);
    }
  }
}
