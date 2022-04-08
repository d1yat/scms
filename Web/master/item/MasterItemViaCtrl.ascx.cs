using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;


public partial class master_item_MasterItemViaCtrl : System.Web.UI.UserControl
{
  #region Private

  
  private void ClearEntrys()
  {
    winDetail.Title = "Master Item Via";

    hfType.Clear();

    cbGudang.Clear();
    cbGudang.Disabled = false;

    cbCabang.Clear();
    cbCabang.Disabled = false;

    cbSuplier.Clear();
    cbSuplier.Disabled = false;

    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }

     store = cbItemDtl.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private void PopulateDetail(string pID, string pName, bool isAdd)
  {
    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "c_type = @0", pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    if (!isAdd)
    {
      ClearEntrys();
      winDetail.Title = string.Concat("Master Item Category - ", pName);

      #region Parser Header

      string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0170", paramX);

      try
      {
        dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
        if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
        {
          jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

          dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

          jarr.Clear();
        }
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine(
          string.Concat("master_item_MasterItemCategoryCtrl:PopulateDetail Header - ", ex.Message));
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
    }

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
      hfType.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_penjualan_PackingList:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    hfType.Text = pID;
    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  public void initialize(string sID)
  {
    hfStoreID.Text = sID;
  }


  private PostDataParser.StructureResponse SaveParser(bool isAdd, Dictionary<string, string>[] dics)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    //pair.Value = numberId;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null,
      tmp = null,
      item = null,
      via = null,
      ket = null;

    bool isNew = false,
      isVoid = false,
      isModify = false;

    DateTime date = DateTime.Today;

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Gdg", cbGudang.Text);
    pair.DicAttributeValues.Add("Cusno", cbCabang.Text);


    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      isNew = dicData.GetValueParser<bool>("l_new");
      isVoid = dicData.GetValueParser<bool>("l_void");
      isModify = dicData.GetValueParser<bool>("l_modified");

      if (isNew && (!isVoid) && (!isModify))
      {
        if (!pag.IsAllowAdd)
        {
          continue;
        }

        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        via = dicData.GetValueParser<string>("c_via");

        if ((!string.IsNullOrEmpty(item)))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Via", via);


          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr
          });
        }
      }
      else if ((!isNew) && (isVoid))
      {
        if (!pag.IsAllowDelete)
        {
          continue;
        }

        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

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
      varData = parser.ParserData("MasterItemVia", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("master_item_MasterItemViaCtrl SaveParser : {0} ", ex.Message);
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

  #endregion

  public void CommandPopulate(string pID, string pidName, bool isAdd)
  {
    if (isAdd)
    {
      winDetail.Show();
      winDetail.Hidden = false;

      this.ClearEntrys();
    }
    else
    {
      PopulateDetail(pID, pidName, false);
    }
  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);

    bool isAdd = true;

    if (isAdd)
    {
      if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
      {
        Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
        return;
      }
    }
    else if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowEdit)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menyimpan data.");
      return;
    }

    Dictionary<string, string>[] gridDataCat = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, gridDataCat);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        //for (int loop = 0; loop < gridDataCat.Length; loop++)
        //{
          //numberId = respon.Values.GetValueParser<string>("idx" + loop, string.Empty);
          //decimal sum = decimal.Parse(numberId) + 1;

          string scrpt = null;

          string dateJs = null;
          DateTime date = DateTime.Today;

//          string sd = (string.Format(@"{0}.insert(0, new Ext.data.Record({{
//                                 'v_cunam': '{1}',
//                                 'c_iteno': '{2}',
//                                 'v_itnam': '{3}',
//                                 'v_nama': '{4}',
//                                 'v_ket': '{5}',
//                                 'idx': {6}
//
//                                 
//                               }}));{0}.commitChanges();", storeId,
//                  (cbCabang.SelectedItem == null ? cbCabang.Text : cbCabang.SelectedItem.Text),
//                  (respon.Values.GetValueParser<string>("c_iteno" + loop, string.Empty) == null ? "" : respon.Values.GetValueParser<string>("c_iteno" + loop, string.Empty)),
//                  (respon.Values.GetValueParser<string>("v_itnam" + loop, string.Empty) == null ? "" : respon.Values.GetValueParser<string>("v_itnam" + loop, string.Empty)),
//                  (respon.Values.GetValueParser<string>("v_nama" + loop, string.Empty) == null ? "" : respon.Values.GetValueParser<string>("v_nama" + loop, string.Empty)),
//                  (respon.Values.GetValueParser<string>("v_ket" + loop, string.Empty) == null ? "" : respon.Values.GetValueParser<string>("v_ket" + loop, string.Empty)),
//                  sum
//                 )
            //(cbTipe.SelectedItem == null ? cbTipe.Text : cbTipe.SelectedItem.Text))


       //);

          string sd = (string.Format(@"{0}.reload();", storeId));
          X.AddScript(sd);
        //}

        Functional.ShowMsgInformation("Data berhasil tersimpan.");
        this.ClearEntrys();
        
        
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


  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void onchangeBtn(object sender, DirectEventArgs e)
  {
    string Tipe = (e.ExtraParams["Tipe"] ?? string.Empty);

    PopulateDetail(Tipe, "c_type", true);
  }

  protected void ReloadBtn_Click(object sender, DirectEventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }
    this.ClearEntrys();
  }


}
