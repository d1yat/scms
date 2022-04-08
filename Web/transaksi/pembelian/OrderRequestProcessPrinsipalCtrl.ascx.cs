using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using System.Text;

public partial class transaksi_pembelian_OrderRequestProcessPrinsipalCtrl : System.Web.UI.UserControl
{
  #region Private

  private static readonly Random randRandom = new Random((int)DateTime.Now.Ticks);

  private void ClearEntrys()
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    winDetail.Title = "Order Request Process Pemasok";

    hfContentID.Clear();

    cbSuplierHdr.Clear();
    cbSuplierHdr.Disabled = false;

    cbTipeHdr.Clear();
    cbTipeHdr.Disabled = false;

    cbGudangHdr.Clear();
    cbGudangHdr.Disabled = false;

    cbCustomerHdr.Clear();
    cbCustomerHdr.Disabled = false;

    cbDivPrincipalHdr.Clear();
    cbDivPrincipalHdr.Disabled = false;

    cbViaHdr.Clear();
    cbViaHdr.Disabled = false;
    
    cbTipeProdukHdr.Clear();
    cbViaHdr.Disabled = false;

    txKeterangan.Clear();
    txKeterangan.Disabled = false;

    X.AddScript(string.Format("{0}.getForm().reset();", frmHeaders.ClientID));

    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));

    cbItemDtl.Disabled = false;
    cbSPNoDtl.Disabled = false;
    txQtyDtl.Disabled = false;   
 
    btnAdd.Disabled = false;
    btnClear.Disabled = false;

    //btnPrint.Hidden = true;

    Functional.SetComboData(cbTipeHdr, "c_type", "Otomatis", "01", false);
    Functional.SetComboData(cbGudangHdr, "c_gdg", pag.ActiveGudangDescription, pag.ActiveGudang, false);
    Functional.SetComboData(cbViaHdr, "c_type", "Darat/Laut", "02", false);

    cbCustomerHdr.AllowBlank = true;

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
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
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0013", paramX);
    bool isComplete = false;

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("Order Request Process Pemasok - {0}", pID);

        cbSuplierHdr.ToBuilder().AddItem(
          (dicResultInfo.ContainsKey("v_nama") ? dicResultInfo["v_nama"] : string.Empty),
          (dicResultInfo.ContainsKey("c_nosup") ? dicResultInfo["c_nosup"] : string.Empty)
        );
        if (cbSuplierHdr.GetStore() != null)
        {
          cbSuplierHdr.GetStore().CommitChanges();
        }
        cbSuplierHdr.SetValueAndFireSelect((dicResultInfo.ContainsKey("c_nosup") ? dicResultInfo["c_nosup"] : string.Empty));
        
        isComplete = dicResultInfo.GetValueParser<bool>("orProses");

        txKeterangan.Text = dicResultInfo.GetValueParser<string>("v_ket", string.Empty);
        
        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_pembelian_transaksi_pembelian_OrderRequestProcessPrinsipalCtrl:PopulateDetail Header - ", ex.Message));
    }
    finally
    {
      //SetEditor(!isComplete);

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
        string.Concat("transaksi_pembelian_transaksi_pembelian_OrderRequestProcessPrinsipalCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(Dictionary<string, string>[] dics, string suplierId, string tipeTransaksi, string gudang, string cabang, string divPrinsipal, string via, string tipeProduk, string keteranganHeader)
  {
    if (dics == null)
    {
      return new PostDataParser.StructureResponse()
      {
        IsSet = true,
        Message = "Can't parsing session from grid.",
        Response = PostDataParser.ResponseStatus.Error,
      };
    }

    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = null;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string tmp = null;
    string varData = null;

    decimal oRder = dicData.GetValueParser<decimal>("n_order");

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Suplier", suplierId);
    pair.DicAttributeValues.Add("Tipe", tipeTransaksi);
    pair.DicAttributeValues.Add("Gudang", (string.IsNullOrEmpty(gudang) ? "1" : gudang));
    pair.DicAttributeValues.Add("Cabang", cabang);
    pair.DicAttributeValues.Add("DivPrincipal", divPrinsipal);
    pair.DicAttributeValues.Add("Via", via);
    pair.DicAttributeValues.Add("TipeItem", tipeProduk);
    pair.DicAttributeValues.Add("Keterangan", keteranganHeader);

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      dicAttr = new Dictionary<string, string>();

      dicAttr.Add("New", "true");
      dicAttr.Add("Delete", "false");
      dicAttr.Add("Modified", "false");

      dicAttr.Add("Item", dicData.GetValueParser<string>("c_iteno", string.Empty));
      dicAttr.Add("NoID", dicData.GetValueParser<string>("NoID", string.Empty));
      dicAttr.Add("NoRef", dicData.GetValueParser<string>("NoRef", string.Empty));
      dicAttr.Add("Type", dicData.GetValueParser<string>("c_type", string.Empty));
      dicAttr.Add("DivPri", dicData.GetValueParser<string>("c_kddivpri", string.Empty));
      dicAttr.Add("Via", dicData.GetValueParser<string>("c_via", string.Empty));
      dicAttr.Add("Quantity", dicData.GetValueParser<decimal>("Quantity").ToString());
      dicAttr.Add("MOQ", dicData.GetValueParser<decimal>("n_qminord").ToString());
      dicAttr.Add("AvgSales", dicData.GetValueParser<decimal>("n_avgsls").ToString());
      dicAttr.Add("Index", dicData.GetValueParser<decimal>("n_index").ToString());
      dicAttr.Add("SoH", dicData.GetValueParser<decimal>("n_soh").ToString());
      dicAttr.Add("SiT", dicData.GetValueParser<decimal>("n_sit").ToString());
      dicAttr.Add("BackOrder", dicData.GetValueParser<decimal>("n_bo").ToString());
      dicAttr.Add("Box", dicData.GetValueParser<decimal>("n_box").ToString());
      dicAttr.Add("HNA", dicData.GetValueParser<decimal>("n_salpri").ToString());
      dicAttr.Add("MOP", dicData.GetValueParser<decimal>("n_pminord").ToString());
      dicAttr.Add("SpAcc", dicData.GetValueParser<decimal>("n_spacc").ToString());
      dicAttr.Add("QtyOR", dicData.GetValueParser<decimal>("n_qty").ToString());
      dicAttr.Add("Bonus", dicData.GetValueParser<decimal>("n_bonus").ToString());
      dicAttr.Add("AvgSalesDivPri", dicData.GetValueParser<decimal>("n_avgslsdivpri").ToString());
      dicAttr.Add("Variable", dicData.GetValueParser<decimal>("n_variabel").ToString());
      dicAttr.Add("Idxp", dicData.GetValueParser<decimal>("n_idxp").ToString());
      dicAttr.Add("Idxnp", dicData.GetValueParser<decimal>("n_idxnp").ToString());
      dicAttr.Add("Pareto", dicData.GetValueParser<decimal>("n_pareto").ToString());
      dicAttr.Add("Ideal", dicData.GetValueParser<decimal>("n_ideal").ToString());
      dicAttr.Add("Order", dicData.GetValueParser<decimal>("n_order").ToString());
      dicAttr.Add("Deviasi", dicData.GetValueParser<decimal>("n_deviasi").ToString());
      dicAttr.Add("QtySisa", dicData.GetValueParser<decimal>("QtySisa").ToString());
      dicAttr.Add("Beli", dicData.GetValueParser<decimal>("n_beli").ToString());
      dicAttr.Add("Manual", dicData.GetValueParser<bool>("l_manual").ToString().ToLower());
      dicAttr.Add("IsCombo", dicData.GetValueParser<bool>("l_combo").ToString().ToLower());
      dicAttr.Add("ItemCombo", dicData.GetValueParser<string>("ItemCombo", string.Empty).ToString().ToLower());

      pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
      {
        IsSet = true,
        DicAttributeValues = dicAttr
      });        

      #region Old Coded

      //isNew = dicData.GetValueParser<bool>("l_new");
      //isVoid = dicData.GetValueParser<bool>("l_void");
      //isModify = dicData.GetValueParser<bool>("l_modified");

      //if (isNew && (!isVoid) && (!isModify))
      //{
      //  dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      //  dicAttr.Add("New", isNew.ToString().ToLower());
      //  dicAttr.Add("Delete", isVoid.ToString().ToLower());
      //  dicAttr.Add("Modified", isModify.ToString().ToLower());

      //  item = dicData.GetValueParser<string>("c_iteno");
      //  nQty = dicData.GetValueParser<decimal>("n_Qty", 0);
      //  nAcc = dicData.GetValueParser<decimal>("n_QtyOrd", 0);

      //  if ((!string.IsNullOrEmpty(item)) &&
      //    (nQty > 0) &&
      //    (nAcc > 0))
      //  {
      //    dicAttr.Add("Item", item);
      //    dicAttr.Add("Qty", nQty.ToString());
      //    dicAttr.Add("QtyOrd", nAcc.ToString());

      //    pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
      //    {
      //      IsSet = true,
      //      DicAttributeValues = dicAttr
      //    });
      //  }
      //}
      //else if ((!isNew) && isVoid && (!isModify))
      //{
      //  dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      //  dicAttr.Add("New", isNew.ToString().ToLower());
      //  dicAttr.Add("Delete", isVoid.ToString().ToLower());
      //  dicAttr.Add("Modified", isModify.ToString().ToLower());

      //  item = dicData.GetValueParser<string>("c_iteno");
      //  ket = dicData.GetValueParser<string>("v_ket");
        
      //  if (!string.IsNullOrEmpty(item))
      //  {
      //    dicAttr.Add("Item", item);

      //    pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
      //    {
      //      IsSet = true,
      //      Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
      //      DicAttributeValues = dicAttr
      //    });
      //  }
      //}
      //else if ((!isNew) && (!isVoid) && isModify)
      //{
      //  dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      //  dicAttr.Add("New", isNew.ToString().ToLower());
      //  dicAttr.Add("Delete", isVoid.ToString().ToLower());
      //  dicAttr.Add("Modified", isModify.ToString().ToLower());

      //  item = dicData.GetValueParser<string>("c_iteno");
      //  nQty = dicData.GetValueParser<decimal>("n_Qty", 0);
      //  ket = dicData.GetValueParser<string>("v_ket");

      //  if ((!string.IsNullOrEmpty(item)) &&
      //    (nQty > 0))
      //  {
      //    dicAttr.Add("Item", item);
      //    dicAttr.Add("Qty", nQty.ToString());

      //    pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
      //    {
      //      IsSet = true,
      //      Value = (string.IsNullOrEmpty(ket) ? "Modify" : ket),
      //      DicAttributeValues = dicAttr
      //    });
      //  }
      //}

      #endregion

      //dicData.Clear();
    }

    try
    {
      varData = parser.ParserData("OrderRequestProcessPrincipal", "Add", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_transaksi_pembelian_OrderRequestProcessPrinsipalCtrl SaveParser : {0} ", ex.Message);
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
  
//  private void SetEditor(bool isEditing)
//  {
//    if (isEditing)
//    {
//      /*
//      Properties of 'e' include:
//      e.grid - This grid
//      e.record - The record being edited
//      e.field - The field name being edited
//      e.value - The value being set
//      e.originalValue - The original value for the field, before the edit.
//      e.row - The grid row index
//      e.column - The grid column index
//      e.cancel - Cancel new data
//      */
//      X.AddScript(@"var verifyRowEditConfirm = function(e) {
//                      var accQty = e.record.get('n_QtyApprove');
//                      var sisa = e.record.get('n_sisa');
//                      var totAccQty = e.record.get('n_totalAcc');
//                      var lMod = e.record.get('l_modified');
//                      var oAccQty = e.record.get('n_QtyOriginal');
//                      if (accQty != sisa) {
//                        e.cancel = true;
//                        ShowWarning('Maaf, barang yang telah terproses tidak dapat diubah.');
//                      }
//                      else if(e.value == oAccQty) {
//                        e.record.reject();
//                      }
//                      else {
//                        e.record.set('n_sisa', e.value);
//                        e.record.set('n_QtyApprove', e.value);
//                        e.record.set('n_totalAcc', ((sisa > e.value) ? (totAccQty - (sisa - e.value)) : (totAccQty + (e.value - sisa))));
//                        e.record.set('l_modified', true);
//                        if(!lMod) {
//                          e.record.set('n_QtyOriginal', accQty);
//                        }
//                      }
//                    };");

//      X.AddScript(
//        string.Format(@"var tryFindColIndex = function () {{
//                          var idx = {0}.findColumnIndex('n_QtyApprove');
//                          if(idx != -1) {{
//                            {0}.setEditable(idx, true);
//                            {0}.setEditor(idx, new Ext.form.NumberField({{
//                              allowBlank: false,
//                              allowDecimals: true,
//                              allowNegative: false,
//                              minValue: 0.00
//                            }}));
//                          }}
//                        }};
//                        tryFindColIndex();", gridDetail.ColumnModel.ClientID));

//      gridDetail.AddListener("ValidateEdit", "verifyRowEditConfirm");
//    }
//    else
//    {
//      X.AddScript(
//        string.Format(@"var idx = {0}.findColumnIndex('n_QtyRequest');
//              if(idx != -1) {{
//                {0}.setEditable(idx, false);
//              }}", gridDetail.ColumnModel.ClientID));
//    }
//  }

  private List<Dictionary<string, string>> SummaryData(List<Dictionary<string, string>> listDic)
  {
    if (listDic == null)
    {
      return null;
    }

    List<Dictionary<string, string>> listD = null;
    Dictionary<string, string> dicRead = null,
      dicWrite = null;
    
    string item = null;
    //decimal decSPAcc = 0,
    //  decQty = 0,
    //  decAcc = 0,
    //  decQtySisa = 0,
    //  decOrder = 0;

    Dictionary<string, Dictionary<string, string>> dicData = new Dictionary<string,Dictionary<string,string>>();

    for (int nLoop = 0, nLen = listDic.Count; nLoop < nLen; nLoop++)
    {
      dicRead = listDic[nLoop];

      item = dicRead.GetValueParser<string>("c_iteno", string.Empty);

      if (!dicData.ContainsKey(item))
      {
        dicWrite = new Dictionary<string, string>();

        foreach (KeyValuePair<string, string> kvp in dicRead)
        {
          dicWrite.Add(kvp.Key, kvp.Value);
        }

        dicData.Add(item, dicWrite);
      }
      //else
      //{
      //  dicWrite = dicData[item];

      //  decSPAcc = dicWrite.GetValueParser<decimal>("n_spacc");
      //  decSPAcc += dicRead.GetValueParser<decimal>("n_spacc");
      //  dicWrite["n_spacc"] = decSPAcc.ToString();
        
      //  decQty = dicWrite.GetValueParser<decimal>("Quantity");
      //  decQty += dicRead.GetValueParser<decimal>("Quantity");
      //  dicWrite["Quantity"] = decQty.ToString();

      //  decAcc = dicWrite.GetValueParser<decimal>("Acceptance");
      //  decAcc += dicRead.GetValueParser<decimal>("Acceptance");
      //  dicWrite["Acceptance"] = decAcc.ToString();

      //  decQtySisa = dicWrite.GetValueParser<decimal>("QtySisa");
      //  decQtySisa += dicRead.GetValueParser<decimal>("QtySisa");
      //  dicWrite["QtySisa"] = decQtySisa.ToString();

      //  decOrder = dicWrite.GetValueParser<decimal>("n_order");
      //  decOrder += dicRead.GetValueParser<decimal>("n_order");
      //  dicWrite["n_order"] = decOrder.ToString();
      //}
    }

    if (dicData.Count > 0)
    {
      listD = new List<Dictionary<string, string>>();

      foreach (KeyValuePair<string, Dictionary<string, string>> kvp in dicData)
      {
        listD.Add(kvp.Value);
      }

      dicData.Clear();

      item = randRandom.Next(int.MinValue, int.MaxValue).ToString("x08");

      hfContentID.Text = item;

      this.Session[item] = listDic;
    }

    return listD;
  }

  private Dictionary<string, string>[] MergeData(string contentId , Dictionary<string, string>[] dicData)
  {
    if (string.IsNullOrEmpty(contentId))
    {
      return null;
    }

    List<Dictionary<string, string>> listDic = null;

    listDic = this.Session[contentId] as List<Dictionary<string, string>>;
    if (listDic == null)
    {
      return null;
    }

    Dictionary<string, string> dicRead = null,
      dicWrite = null;
    List<Dictionary<string, string>> listD = new List<Dictionary<string,string>>();

    int nLoop = 0,
      nLen = 0,
      nLoopC = 0,
      nLenC = 0;
    
    string item = null,
      curItem = null;
    bool isSpDtl = false;

    for (nLoop = 0, nLen = dicData.Length; nLoop < nLen; nLoop++)
    {
      dicRead = dicData[nLoop];

      item = dicRead.GetValueParser<string>("c_iteno", string.Empty);
      isSpDtl = dicRead.GetValueParser<bool>("l_spdtl");

      if (isSpDtl)
      {
        dicWrite = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicWrite.Add("c_iteno", dicRead.GetValueParser<string>("c_iteno", string.Empty));
        dicWrite.Add("n_spacc", dicRead.GetValueParser<decimal>("n_spacc").ToString());
        dicWrite.Add("NoID", dicRead.GetValueParser<string>("NoID", string.Empty));
        dicWrite.Add("NoRef", dicRead.GetValueParser<string>("NoRef", string.Empty));
        dicWrite.Add("n_qminord", dicRead.GetValueParser<decimal>("n_qminord").ToString());
        dicWrite.Add("Quantity", dicRead.GetValueParser<decimal>("Quantity").ToString());
        dicWrite.Add("l_manual", dicRead.GetValueParser<bool>("l_manual").ToString().ToLower());

        listD.Add(dicWrite);
      }
      else
      {
        for (nLoopC = 0, nLenC = listDic.Count; nLoopC < nLenC; nLoopC++)
        {
          dicWrite = listDic[nLoopC];

          curItem = dicWrite.GetValueParser<string>("c_iteno", string.Empty);

          if (curItem.Equals(item, StringComparison.OrdinalIgnoreCase))
          {
            listD.Add(dicWrite);
          }
        }
      }

      //dicRead.Clear();
    }

    return listD.ToArray();
  }

  private void ClearSessionObject(string contId)
  {
    List<Dictionary<string, string>> listDic = null;

    if (!string.IsNullOrEmpty(contId))
    {
      listDic = this.Session[contId] as List<Dictionary<string, string>>;

      if (listDic != null)
      {
        listDic.Clear();

        this.Session.Remove(contId);
      }
    }

    hfContentID.Text = string.Empty;
  }

  #endregion

  public void Initialize(string storeIDGridMain, string typeName)
  {
    hfStoreID.Text = storeIDGridMain;
  }

  public void CommandPopulate(bool isAdd, string pID)
  {
    if (isAdd)
    {
      //SetEditor(false);

      ClearEntrys();

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      PopulateDetail("c_orno", pID);
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string suplierId = (e.ExtraParams["SuplierID"] ?? string.Empty);
    string suplierName = (e.ExtraParams["SuplierName"] ?? string.Empty);
    string typeValue = (e.ExtraParams["TypeValue"] ?? string.Empty);
    string typeName = (e.ExtraParams["TypeName"] ?? string.Empty);
    string gudangValue = (e.ExtraParams["Gudang"] ?? string.Empty);
    string customerValue = (e.ExtraParams["Customer"] ?? string.Empty);
    string divPrincipalValue = (e.ExtraParams["DivPrincipal"] ?? string.Empty);
    string viaValue = (e.ExtraParams["Via"] ?? string.Empty);
    string tipeProdukValue = (e.ExtraParams["TipeProduk"] ?? string.Empty);
    string ketHeader = (e.ExtraParams["Keterangan"] ?? string.Empty);
    string contId = (e.ExtraParams["ContentID"] ?? string.Empty);

    Dictionary<string, string>[] gridDataDetails = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    bool isAdd = true;

    gridDataDetails = MergeData(contId, gridDataDetails);

    PostDataParser.StructureResponse respon = SaveParser(gridDataDetails, suplierId, typeValue, gudangValue, customerValue, divPrincipalValue, viaValue, tipeProdukValue, ketHeader);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;
        string storeId = hfStoreID.Text;

        string dateJs = null;
        DateTime date = DateTime.Today;

        if (isAdd)
        {
          if (respon.Values != null)
          {
            if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
            {
              dateJs = Functional.DateToJson(date);
            }

            if (!string.IsNullOrEmpty(storeId))
            {
              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_orno': '{1}',
                'd_ordate': {2},
                'v_ket': '{3}',
                'v_type_desc': '{4}',
                'v_nama': '{5}',
                'orProses': false,
                'c_pono': ''
              }}));{0}.commitChanges();", storeId,
                    respon.Values.GetValueParser<string>("OR", string.Empty),
                        dateJs, ketHeader, typeName, suplierName);

              X.AddScript(scrpt);
            }
          }
        }

        this.ClearSessionObject(contId);

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

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void ProcessORP_Click(object sender, DirectEventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    /*
     <ext:Parameter Name="parameters" Value="[['divSuplier', #{cbDivPrincipal}.getValue(), 'System.String'],
                      ['via', #{cbViaHdr}.getValue(), 'System.String'],
                      ['typeItem', #{cbTipeProdukHdr}.getValue(), 'System.String'],
                      ['suplier', #{cbSuplierHdr}.getValue(), 'System.String'],
                      ['tipeProcess', #{cbTipeHdr}.getValue(), 'System.String'],
                      ['gudang', #{cbGudangHdr}.getValue(), 'System.Char'],
                      ['customer', #{cbCustomerHdr}.getValue(), 'System.String']]" Mode="Raw" />
     */
    string divSuplier = (e.ExtraParams["divSuplier"] ?? string.Empty);
    string via = (e.ExtraParams["via"] ?? string.Empty);
    string typeItem = (e.ExtraParams["typeItem"] ?? string.Empty);
    string suplier = (e.ExtraParams["suplier"] ?? string.Empty);
    string tipeProcess = (e.ExtraParams["tipeProcess"] ?? string.Empty);
    string gudang = (e.ExtraParams["gudang"] ?? string.Empty);
    string customer = (e.ExtraParams["customer"] ?? string.Empty);
    string contId = (e.ExtraParams["ContentID"] ?? string.Empty);

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "divSuplier", divSuplier, "System.String"},
        new string[] { "via", via, "System.String"},
        new string[] { "typeItem", typeItem, "System.String"},
        new string[] { "suplier", suplier, "System.String"},
        new string[] { "tipeProcess", tipeProcess, "System.String"},
        new string[] { "gudang", gudang, "System.Char"},
        new string[] { "customer", customer, "System.String"}
      };

    string result = soa.GlobalQueryService(0, -1, true, string.Empty, string.Empty, "8091", paramX);

    this.ClearSessionObject(contId);

    if (!string.IsNullOrEmpty(result))
    {
      Dictionary<string, object> dicResult = null;
      Dictionary<string, string> dicResultInfo = null;
      List<Dictionary<string, string>> lstResultInfo = null;
      Newtonsoft.Json.Linq.JArray jarr = null;
      
      dicResult = JSON.Deserialize<Dictionary<string, object>>(result);

      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        lstResultInfo = JSON.Deserialize<List<Dictionary<string, string>>>(jarr.First.ToString());

        #region Populate Data

        lstResultInfo = SummaryData(lstResultInfo);

        if ((lstResultInfo != null) && (lstResultInfo.Count > 0))
        {
          dicResult.Clear();

          StringBuilder sb = new StringBuilder();

          string storeId = gridDetail.GetStore().ClientID;

          sb.AppendFormat("{0}.removeAll();", storeId);

          for (int nLoop = 0, nLen = lstResultInfo.Count; nLoop < nLen; nLoop++)
          {
            dicResultInfo = lstResultInfo[nLoop];

            sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{", storeId);

            foreach (KeyValuePair<string, string> kvp in dicResultInfo)
            {
              sb.AppendFormat("'{0}': '{1}',", kvp.Key, kvp.Value);

//              sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
//                'c_orno': '{1}',
//                'd_ordate': {2},
//                'v_ket': '{3}',
//                'v_type_desc': '{4}',
//                'v_nama': '{5}',
//                'orProses': false,
//                'c_pono': ''
//              }}));{0}.commitChanges();", storeId,
//                    respon.Values.GetValueParser<string>("OR", string.Empty),
//                        dateJs, ketHeader, typeName, suplierName);
            }

            if (sb.ToString().EndsWith(",", StringComparison.OrdinalIgnoreCase))
            {
              sb.Remove(sb.Length - 1, 1);
            }

            sb.Append("}));");

            dicResultInfo.Clear();
          }

          sb.AppendLine(string.Format("{0}.commitChanges();recalculateTotalOR({0}, {1});", 
            storeId, sbDtlPanel.ClientID));

          Ext.Net.X.AddScript(sb.ToString());

          sb.Remove(0, sb.Length);
        }

        #endregion

        jarr.Clear();
      }
    }
  }

  protected void ReloadBtn_Click(object sender, DirectEventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    string contId = (e.ExtraParams["ContentID"] ?? string.Empty);

    this.ClearSessionObject(contId);

    this.ClearEntrys();
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      OrderRequestDetilInfo.CommandPopulate("ORP", pag.ActiveGudang, pID);
    }
  }
}