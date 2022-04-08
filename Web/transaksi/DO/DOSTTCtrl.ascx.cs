﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_DO_DOSTTCtrl : System.Web.UI.UserControl
{
  public void ClearEntry()
  {
    hfDONo.Clear();
    hfPlNo.Clear();

    cbGudang.Clear();
    cbGudang.Disabled = false;

    cbSTTSample.Clear();
    cbSTTSample.Disabled = false;

    cbViaHdr.Clear();
    cbViaHdr.Disabled = false;

    txKeterangan.Clear();
    txKeterangan.Disabled = false;

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  public void PopulateDetail(string pName, string pID, string Case)
  {
    Dictionary<string, object> dicDO = null;
    Dictionary<string, string> dicDOInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string tmp = null;
    try
    {
      switch (Case)
      {
        #region Add

        case "Add":
          {

            Ext.Net.Store store = gridDetail.GetStore();

            tmp = string.Format(@"var xOpts = {{
                                  params: {{
                                      start: '0',
                                      limit: '-1',
                                      allQuery: 'true',
                                      model: '0010a',
                                      sort: '',
                                      dir: '',
                                      parameters: [['c_stno = @0', paramValueGetter({0}), 'System.String']]
                                    }}
                                  }};", cbSTTSample.ClientID);

            X.AddScript(tmp);

            X.AddScript(string.Format("{0}.removeAll();{0}.reload(xOpts);", store.ClientID));

          }
          break;
        #endregion

        #region View

        case "View":
          {

            winDetil.Hidden = false;

            string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0007", paramX);

            dicDO = JSON.Deserialize<Dictionary<string, object>>(res);
            if (dicDO.ContainsKey("records") && (dicDO.ContainsKey("totalRows") && (((long)dicDO["totalRows"]) > 0)))
            {

              jarr = new Newtonsoft.Json.Linq.JArray(dicDO["records"]);

              dicDOInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());


              txCustomerHeader.ToBuilder().AddItem(
                      dicDOInfo.ContainsKey("v_cunam") ? dicDOInfo["v_cunam"] : string.Empty,
                      dicDOInfo.ContainsKey("c_cusno") ? dicDOInfo["c_cusno"] : string.Empty
                      );
              if (txCustomerHeader.GetStore() != null)
              {
                txCustomerHeader.GetStore().CommitChanges();
              }
              txCustomerHeader.SetValueAndFireSelect((dicDOInfo.ContainsKey("c_cusno") ? dicDOInfo["c_cusno"] : string.Empty));
              txCustomerHeader.Disabled = true;

              cbGudang.ToBuilder().AddItem(
                      dicDOInfo.ContainsKey("v_gdgdesc") ? dicDOInfo["v_gdgdesc"] : string.Empty,
                      dicDOInfo.ContainsKey("c_gdg") ? dicDOInfo["c_gdg"] : string.Empty
                      );
              if (cbGudang.GetStore() != null)
              {
                cbGudang.GetStore().CommitChanges();
              }
              cbGudang.SetValueAndFireSelect((dicDOInfo.ContainsKey("c_gdg") ? dicDOInfo["c_gdg"] : string.Empty));
              cbGudang.Disabled = true;

              cbSTTSample.SetValueAndFireSelect((dicDOInfo.ContainsKey("c_plno") ? dicDOInfo["c_plno"] : string.Empty));
              cbSTTSample.Disabled = true;

              cbViaHdr.SetValueAndFireSelect((dicDOInfo.ContainsKey("v_ket") ? dicDOInfo["v_ket"] : string.Empty));
              cbViaHdr.Disabled = true;

              txKeterangan.Text = ((dicDOInfo.ContainsKey("v_ketdo") ? dicDOInfo["v_ketdo"] : string.Empty));
              txKeterangan.Disabled = false;

              #region Parser Detail

              Ext.Net.Store store = gridDetail.GetStore();

              tmp = string.Format(@"var xOpts = {{
                                  params: {{
                                      start: '0',
                                      limit: '-1',
                                      allQuery: 'true',
                                      model: '0010',
                                      sort: '',
                                      dir: '',
                                      parameters: [['{0} = @0', '{1}', 'System.String']]
                                    }}
                                  }};", pName, pID);

              X.AddScript(tmp);


              hfDONo.Text = pID;

              winDetil.ShowModal();
              winDetil.Title = string.Format("Delivery Order - {0}", pID);
              X.AddScript(string.Format("{0}.removeAll();{0}.reload(xOpts);", store.ClientID));

              GC.Collect();

              #endregion

            }

          }
          break;

        #endregion

      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
      string.Concat("transaction_sales_PackingList:PopulateDetail Detail - ", ex.Message));
    }
    GC.Collect();
  }

  public void Initialize(string storeIDGridMain)
  {
    hfStoreID.Text = storeIDGridMain;
  }

  public void CommandPopulate(bool isAdd, string pName, string pID, string Type)
  {
    ClearEntry();

    if (isAdd == true)
    {
      winDetil.Visible = true;
      winDetil.Hidden = false;

      winDetil.Title = "DO STT";
    }
    else
    {
      PopulateDetail(pName, pID, "View");
    }
  }

  protected void OnEvenAddGrid(object sender, DirectEventArgs e)
  {
    //string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    PopulateDetail(pName, pID, "Add");

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string doNumberID, Dictionary<string, string>[] dics)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = doNumberID;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);


    string tmp = null,
      item = null, ket = null;

    int nQty = 0;
    bool isNew = false,
      isVoid = false, isModify = false;
    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);

    pair.DicAttributeValues.Add("Via", cbViaHdr.Text);
    pair.DicAttributeValues.Add("Customer", txCustomerHeader.Text);
    pair.DicAttributeValues.Add("sttNO", cbSTTSample.Text);
    pair.DicAttributeValues.Add("Keterangan", txKeterangan.Text);

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      isNew = dicData.GetValueParser<bool>("l_new");
      isVoid = dicData.GetValueParser<bool>("l_void");
      isModify = dicData.GetValueParser<bool>("l_modified");

      if (isVoid && (!isModify) && (!isNew))
      {

        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        nQty = dicData.GetValueParser<int>("n_qty", 0);
        ket = dicData.GetValueParser<string>("v_ket");

        if (
          (!string.IsNullOrEmpty(item)) &&
          (nQty > 0))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Qty", nQty.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
            DicAttributeValues = dicAttr
          });
        }
      }
      if ((!isVoid) && (!isModify) && (isNew))
      {

        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", "true");
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        nQty = dicData.GetValueParser<int>("n_qty", 0);

        if ((!string.IsNullOrEmpty(item)) &&
          (nQty > 0))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Qty", nQty.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr
          });
        }
      }
    }

    dicData.Clear();

    try
    {
      varData = parser.ParserData("DOSTT", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_STTCtrl SaveParser : {0} ", ex.Message);
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

  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string NumberID = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jSonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);

    Dictionary<string, string>[] gridDataDO = JSON.Deserialize<Dictionary<string, string>[]>(jSonGridValues);

    bool isAdd = (string.IsNullOrEmpty(NumberID) ? true : false);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, NumberID, gridDataDO);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;
        string storeId = hfStoreID.Text;
        
        string cust = (txCustomerHeader.SelectedIndex != -1 ? txCustomerHeader.SelectedItem.Text : string.Empty);
        string Via = (cbViaHdr.SelectedIndex != -1 ? cbViaHdr.SelectedItem.Text : string.Empty);
        string noPl = (cbSTTSample.SelectedIndex != -1 ? cbSTTSample.SelectedItem.Text : string.Empty);

        string dateJs = null;
        DateTime date = DateTime.Today;

        if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
        {
          dateJs = Functional.DateToJson(date);
        }
        if (!string.IsNullOrEmpty(storeId))
        {
          scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                                    'c_dono': '{1}'
                                    ,'d_dodate': {2}
                                    ,'v_gdgdesc': '{3}'
                                    ,'v_cunam': '{4}'
                                    ,'v_ket': '{5}'
                                    ,'c_plno': '{6}'
                                    ,'c_pin': '{7}'
                                    ,'c_expno': '{8}'
                                    ,'l_confirm': false }})",
                                      storeId, respon.Values.GetValueParser<string>("DO", string.Empty),
                                      dateJs,
                                      respon.Values.GetValueParser<string>("Gudang", string.Empty),
                                      cust, Via, noPl,
                                      respon.Values.GetValueParser<string>("Pin", string.Empty),
                                      string.Empty);

          X.AddScript(scrpt);
        }


        ClearEntry();

        Functional.ShowMsgInformation("Data berhasil tersimpan.");
      }
    }
  }

  protected void btnPrintPL_OnClick(object sender, DirectEventArgs e)
  {
    //Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    //if (!pag.IsAllowPrinting)
    //{
    //  Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
    //  return;
    //}

    DOSTTCtrlPrint.ShowPrintPage();
  }
}
