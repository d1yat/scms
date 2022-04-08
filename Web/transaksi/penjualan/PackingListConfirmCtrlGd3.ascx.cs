using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_penjualan_PackingListConfirmCtrlGd3 : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Packing List Confirm";

    hfPlNoConfirm.Clear();
    hfTypDtl.Clear();
    hfExpire.Clear();
    hfSPDate.Clear();

    cbViaHdr.Clear();
    cbViaHdr.Disabled = false;

    //cbCustomerHdr.Clear();
    //cbCustomerHdr.Disabled = false;

    //cbPrincipalHdr.Clear();
    //cbPrincipalHdr.Disabled = false;

    txKeterangan.Clear();
    txKeterangan.Disabled = false;

    cbTipeHdr.Clear();
    cbTipeHdr.Disabled = false;

    chkConfirm.Clear();
    chkConfirm.Disabled = false;
    
    hfItemCatHdr.Clear();
    lbItemCatHdr.Text = string.Empty;

    hfPrinsipalHdr.Clear();
    lbPrinsipalHdr.Text = string.Empty;

    hfCustomerHdr.Clear();
    lbCustomerHdr.Text = string.Empty;

    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));
    //frmpnlDetailEntry.Disabled = false;

    cbItemDtl.Disabled = false;
    cbSpcDtl.Disabled = false;
    cbBatDtl.Disabled = false;
    txQtyDtl.Disabled = false;

    btnAdd.Disabled = false;
    btnClear.Disabled = false;

    btnPrint.Hidden = true;

    btnSave.Hidden = false;

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

    string tmp = null;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0001", paramX);

    //bool isConfirm = false;
    bool isDo = false;
    bool isConfirmed = false;
    bool isPrint = false;

    //bool.TryParse(hfConfMode.Text, out isConfirm);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Concat("Packing List Confirm - ", pID);
        
        tmp = (dicResultInfo.ContainsKey("l_do") ? dicResultInfo["l_do"] : string.Empty);
        if (!bool.TryParse(tmp, out isDo))
        {
          isDo = (tmp.Equals("1", StringComparison.OrdinalIgnoreCase) ? true : false);
        }

        tmp = (dicResultInfo.ContainsKey("l_confirm") ? dicResultInfo["l_confirm"] : string.Empty);
        if (!bool.TryParse(tmp, out isConfirmed))
        {
          isConfirmed = (tmp.Equals("1", StringComparison.OrdinalIgnoreCase) ? true : false);
        }

        //cbViaHdr.SetValueAndFireSelect((dicResultInfo.ContainsKey("c_via") ? dicResultInfo["c_via"] : string.Empty));
        //cbViaHdr.Disabled = true;
        Functional.SetComboData(cbViaHdr, "c_via", dicResultInfo.GetValueParser<string>("v_ket_via", string.Empty), dicResultInfo.GetValueParser<string>("c_via", string.Empty));
        cbViaHdr.Disabled = true;

        ////cbCustomerHdr.ToBuilder().AddItem(
        ////  (dicResultInfo.ContainsKey("V_CUNAM") ? dicResultInfo["V_CUNAM"] : string.Empty),
        ////  (dicResultInfo.ContainsKey("c_cusno") ? dicResultInfo["c_cusno"] : string.Empty)
        ////);
        ////if (cbCustomerHdr.GetStore() != null)
        ////{
        ////  cbCustomerHdr.GetStore().CommitChanges();
        ////}
        ////cbCustomerHdr.SetValueAndFireSelect((dicResultInfo.ContainsKey("c_cusno") ? dicResultInfo["c_cusno"] : string.Empty));
        //Functional.SetComboData(cbCustomerHdr, "c_cusno", dicResultInfo.GetValueParser<string>("v_cunam", string.Empty), dicResultInfo.GetValueParser<string>("c_cusno", string.Empty));
        //cbCustomerHdr.Disabled = true;
        lbCustomerHdr.Text = dicResultInfo.GetValueParser<string>("v_cunam", string.Empty);
        hfCustomerHdr.Text = dicResultInfo.GetValueParser<string>("c_cusno", string.Empty);

        ////cbPrincipalHdr.ToBuilder().AddItem(
        ////  (dicResultInfo.ContainsKey("V_NAMA") ? dicResultInfo["V_NAMA"] : string.Empty),
        ////  (dicResultInfo.ContainsKey("c_nosup") ? dicResultInfo["c_nosup"] : string.Empty)
        ////);
        ////if (cbPrincipalHdr.GetStore() != null)
        ////{
        ////  cbPrincipalHdr.GetStore().CommitChanges();
        ////}
        ////cbPrincipalHdr.SetValueAndFireSelect((dicResultInfo.ContainsKey("c_nosup") ? dicResultInfo["c_nosup"] : string.Empty));
        //Functional.SetComboData(cbPrincipalHdr, "c_nosup", dicResultInfo.GetValueParser<string>("v_nama", string.Empty), dicResultInfo.GetValueParser<string>("c_nosup", string.Empty)); 
        //cbPrincipalHdr.Disabled = true;
        lbPrinsipalHdr.Text = dicResultInfo.GetValueParser<string>("v_nama", string.Empty);
        hfPrinsipalHdr.Text = dicResultInfo.GetValueParser<string>("c_nosup", string.Empty);

        tmp = (dicResultInfo.ContainsKey("l_print") ? dicResultInfo["l_print"] : string.Empty);
        if (!bool.TryParse(tmp, out isPrint))
        {
          isPrint = (tmp.Equals("1", StringComparison.OrdinalIgnoreCase) ? true : false);
        }

        hfItemCatHdr.Text = dicResultInfo.GetValueParser<string>("c_type_cat", string.Empty);
        lbItemCatHdr.Text = dicResultInfo.GetValueParser<string>("v_ket_itemcat", string.Empty);

        txKeterangan.Text = ((dicResultInfo.ContainsKey("v_ket") ? dicResultInfo["v_ket"] : string.Empty));
        
        //cbTipeHdr.SetValueAndFireSelect((dicResultInfo.ContainsKey("c_type") ? dicResultInfo["c_type"] : string.Empty));
        Functional.SetComboData(cbTipeHdr, "c_type", dicResultInfo.GetValueParser<string>("v_ket_type", string.Empty), dicResultInfo.GetValueParser<string>("c_type", string.Empty));
        cbTipeHdr.Disabled = true;

        chkConfirm.Checked = isConfirmed;

        //if ((isDo || isConfirmed) && (!pag.IsSpecialGroup))
        //{
        //  txKeterangan.Disabled = true;
        //  cbTipeHdr.Disabled = true;
        //}

        //if ((isDo || isConfirmed || isPrint) && (!pag.IsSpecialGroup))
        //{
        //  //frmpnlDetailEntry.Disabled = true;
        //  cbItemDtl.Disabled = true;
        //  cbSpcDtl.Disabled = true;
        //  cbBatDtl.Disabled = true;
        //  txQtyDtl.Disabled = true;
        //  chkConfirm.Disabled = true;
        //  btnAdd.Disabled = true;
        //  btnClear.Disabled = true;
        //}

        btnSave.Hidden = isDo;

        X.AddScript(string.Format("clearRelatedComboRecursive(true, {0});", cbItemDtl.ClientID));

        jarr.Clear();

        //btnPrint.Hidden = ((!isPrint) || pag.IsSpecialGroup ? false : true);
        btnPrint.Hidden = false;
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_sales_PackingList:PopulateDetail Header - ", ex.Message));
    }
    finally
    {
      SetEditoryConfirm((!isDo));

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

      hfPlNoConfirm.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_sales_PackingList:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, bool isConfimed, string plNumber, Dictionary<string, string>[] dics)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = plNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string tmp = null,
      sp = null,
      item = null,
      batch = null,
      ket = null,
      typeDC = null,
      accket = null;
    decimal nQty = 0;
    bool isNew = false,
      isVoid = false,
      isModify = false,
      isED = false,
      isAccModify = false;
    string varData = null;

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", pag.Nip);
    pair.DicAttributeValues.Add("Via", cbViaHdr.Text);
    pair.DicAttributeValues.Add("Customer", hfCustomerHdr.Text);
    pair.DicAttributeValues.Add("Suplier", hfPrinsipalHdr.Text);
    pair.DicAttributeValues.Add("Type", cbTipeHdr.Text);
    pair.DicAttributeValues.Add("Keterangan", txKeterangan.Text.Trim());
    pair.DicAttributeValues.Add("Gudang", "6");
    //pair.DicAttributeValues.Add("TypeCategory", string.Empty);
    pair.DicAttributeValues.Add("Confirm", isConfimed.ToString().ToLower());

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      isNew = dicData.GetValueParser<bool>("l_new");
      isVoid = dicData.GetValueParser<bool>("l_void");
      isModify = dicData.GetValueParser<bool>("l_modified");
      isED = dicData.GetValueParser<bool>("l_expired");
      isAccModify = dicData.GetValueParser<bool>("l_accmodify");

      //Verifikasi keterangan ED
      if (isED && string.IsNullOrEmpty(dicData.GetValueParser<string>("v_ket_ed")) && !isVoid)
      {
          responseResult.Message = "KeteranganED";
          responseResult.Response = ScmsSoaLibraryInterface.Components.PostDataParser.ResponseStatus.Failed;
          return responseResult;
      }

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

        sp = dicData.GetValueParser<string>("c_sp");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");

        nQty = dicData.GetValueParser<decimal>("n_booked", 0);
        accket = dicData.GetValueParser<string>("v_ket_ed");

        if ((!string.IsNullOrEmpty(sp)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)) &&
          (nQty > 0))
        {
          dicAttr.Add("Sp", sp);
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("Qty", nQty.ToString());
          dicAttr.Add("isED", isED.ToString().ToLower());
          dicAttr.Add("accket", accket);

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr
          });
        }
      }
      #region Old Coded
      //else if ((!isNew) && isVoid && (!isModify))
      //{
      //  if(!pag.IsAllowDelete)
      //  {
      //    continue;
      //  }

      //  dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      //  dicAttr.Add("New", isNew.ToString().ToLower());
      //  dicAttr.Add("Delete", isVoid.ToString().ToLower());
      //  dicAttr.Add("Modified", isModify.ToString().ToLower());

      //  sp = dicData.GetValueParser<string>("c_sp");
      //  item = dicData.GetValueParser<string>("c_iteno");
      //  batch = dicData.GetValueParser<string>("c_batch");
      //  ket = dicData.GetValueParser<string>("v_ket");

      //  nQty = dicData.GetValueParser<decimal>("n_QtyRequest", 0);

      //  if ((!string.IsNullOrEmpty(sp)) &&
      //    (!string.IsNullOrEmpty(item)) &&
      //    (!string.IsNullOrEmpty(batch)))
      //  {
      //    dicAttr.Add("Sp", sp);
      //    dicAttr.Add("Item", item);
      //    dicAttr.Add("Batch", batch);
      //    dicAttr.Add("Qty", nQty.ToString());

      //    pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
      //    {
      //      IsSet = true,
      //      Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
      //      DicAttributeValues = dicAttr
      //    });
      //  }
      //}
      #endregion
      else if ((!isNew) && (!isVoid) && isModify)
      {
        if(!pag.IsAllowEdit)
        {
          continue;
        }

        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        nQty = dicData.GetValueParser<decimal>("n_QtyRequest", 0);

        if (nQty > 0)
        {
          dicAttr.Add("New", isNew.ToString().ToLower());
          dicAttr.Add("Delete", isVoid.ToString().ToLower());
          dicAttr.Add("Modified", isModify.ToString().ToLower());
        }
        else
        {
          dicAttr.Add("New", "false");
          dicAttr.Add("Delete", "true");
          dicAttr.Add("Modified", "false");
          nQty = dicData.GetValueParser<decimal>("n_sisa", 0);
        }

        sp = dicData.GetValueParser<string>("c_sp");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        ket = dicData.GetValueParser<string>("v_ket");
        typeDC = dicData.GetValueParser<string>("c_type_dc");
        accket = dicData.GetValueParser<string>("v_ket_ed");

         //&& 
         // ((!string.IsNullOrEmpty(typeDC)) || (!typeDC.Equals("00", StringComparison.OrdinalIgnoreCase)))

        if ((!string.IsNullOrEmpty(sp)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)))
        {
          dicAttr.Add("Sp", sp);
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("Qty", nQty.ToString());
          dicAttr.Add("TipeKonfirmasi", typeDC);
          dicAttr.Add("isED", isED.ToString().ToLower());
          dicAttr.Add("accket", accket);

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Modify at Confirm" : ket),
            DicAttributeValues = dicAttr
          });
        }
      }
      else if ((!isNew) && (!isVoid) && !isModify && isAccModify)
      {
          if (!pag.IsAllowEdit)
          {
              continue;
          }

          dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

          dicAttr.Add("New", "false");
          dicAttr.Add("Delete", "false");
          dicAttr.Add("Modified", "false");
          dicAttr.Add("isAccModify", "true");

          sp = dicData.GetValueParser<string>("c_sp");
          item = dicData.GetValueParser<string>("c_iteno");
          batch = dicData.GetValueParser<string>("c_batch");
          ket = dicData.GetValueParser<string>("v_ket");
          typeDC = dicData.GetValueParser<string>("c_type_dc");
          accket = dicData.GetValueParser<string>("v_ket_ed");

          if ((!string.IsNullOrEmpty(sp)) &&
            (!string.IsNullOrEmpty(item)) &&
            (!string.IsNullOrEmpty(batch)))
          {
              dicAttr.Add("Sp", sp);
              dicAttr.Add("Item", item);
              dicAttr.Add("Batch", batch);
              dicAttr.Add("Qty", nQty.ToString());
              dicAttr.Add("TipeKonfirmasi", typeDC);
              dicAttr.Add("isED", isED.ToString().ToLower());
              dicAttr.Add("accket", accket);

              pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
              {
                  IsSet = true,
                  Value = (string.IsNullOrEmpty(ket) ? "Modify at Confirm" : ket),
                  DicAttributeValues = dicAttr
              });
          }
      }

      dicData.Clear();
    }

    try
    {
      varData = parser.ParserData("PackingList", "ModifyConfirm", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_PackingListConfirmCtrl SaveParser : {0} ", ex.Message);
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

  private void SetEditoryConfirm(bool isConfirm)
  {
    if (isConfirm)
    {
      /*
      Properties of 'e' include:
      e.grid - This grid
      e.record - The record being edited
      e.field - The field name being edited
      e.value - The value being set
      e.originalValue - The original value for the field, before the edit.
      e.row - The grid row index
      e.column - The grid column index
      e.cancel - Cancel new data
      */
      X.AddScript(@"var verifyRowEditConfirm = function(e) {
                      if(e.field == 'n_QtyRequest') {
                        var qty = e.record.get('n_qty');
                        if (e.value > qty) {
                          e.cancel = true;
                          ShowWarning('Jumlah tidak boleh melebihi batas.');
                        }
                        else if(e.value == qty) {
                          e.record.reject();
                        }
                        else {
                          e.record.set('l_modified', true);
                        }
                      }
                    };");

      X.AddScript(
        string.Format(@"var tryFindColIndex = function () {{
                          var idx = {0}.findColumnIndex('n_QtyRequest');
                          if(idx != -1) {{
                            {0}.setEditable(idx, true);
                            {0}.setEditor(idx, new Ext.form.NumberField({{
                              allowBlank: false,
                              allowDecimals: true,
                              allowNegative: false,
                              minValue: 0.00
                            }}));
                          }}
                          idx = {0}.findColumnIndex('v_ket_type_dc');
                          if(idx != -1) {{
                            {0}.setEditable(idx, true);
                          }}
                        }};
                        tryFindColIndex();", gridDetail.ColumnModel.ClientID));

      gridDetail.AddListener("ValidateEdit", "verifyRowEditConfirm");

      frmHeaders.Height = new Unit(225);
      chkConfirm.Hidden = false;
      btnSave.Hidden = false;
    }
    else
    {
      X.AddScript(
        string.Format(@"var idx = {0}.findColumnIndex('n_QtyRequest');
              if(idx != -1) {{
                {0}.setEditable(idx, false);
              }}
              idx = {0}.findColumnIndex('v_ket_type_dc');
              if(idx != -1) {{
                {0}.setEditable(idx, false);
              }}", gridDetail.ColumnModel.ClientID));
      
      //gridDetail.AddListener("ValidateEdit", "verifyRowEditConfirm");

      frmHeaders.Height = new Unit(195);
      chkConfirm.Hidden = true;
      btnSave.Hidden = true;
    }
  }

  #endregion

  public void Initialize(string gudang, string gudangDesc, string storeIDGridMain)
  {
    hfGudang.Text = "6";
    hfStoreID.Text = storeIDGridMain;
    hfGudangDesc.Text = gudangDesc;

    //if (isConfirm)
    //{
    //  hfConfMode.Text = bool.TrueString.ToLower();
    //}

    //if (!this.IsPostBack)
    //{
    //  Ext.Net.Store store = cbViaHdr.GetStore();
    //  if (store != null)
    //  {
    //    store.RemoveAll();
    //    X.AddScript(string.Format("{0}.load();", store.ClientID));
    //  }

    //  store = cbTipeHdr.GetStore();
    //  if (store != null)
    //  {
    //    store.RemoveAll();
    //    X.AddScript(string.Format("{0}.load();", store.ClientID));
    //  }
    //}
  }

  public void CommandPopulate(bool isAdd, string pID)
  {
    //bool isConfirm = false;

    //bool.TryParse(hfConfMode.Text, out isConfirm);

    if (isAdd)
    {
      Functional.ShowMsgError("(UC) Perintah tidak dikenal.");
    }
    else
    {
      PopulateDetail("c_plno", pID);
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);

    bool isConfirmed = chkConfirm.Checked;

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);
    
    if (isAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }
    else if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowEdit)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menyimpan data.");
      return;
    }
    
    Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, isConfirmed, numberId, gridDataPL);

    //Dictionary<string, string> dic = new Dictionary<string, string>();
    //dic.Add("Tanggal", "20120120");
    //dic.Add("PL", "PL12010002");
    //PostDataParser.StructureResponse respon = new PostDataParser.StructureResponse()
    //{
    //  IsSet = true,
    //  Response = PostDataParser.ResponseStatus.Success,
    //  Values = dic
    //};

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;

        //string cust = (cbCustomerHdr.SelectedIndex != -1 ? cbCustomerHdr.SelectedItem.Text : string.Empty);
        //string supl = (cbPrincipalHdr.SelectedIndex != -1 ? cbPrincipalHdr.SelectedItem.Text : string.Empty);

        //string cust = (cbCustomerHdr.SelectedItem != null ? cbCustomerHdr.SelectedItem.Text : string.Empty);
        //string supl = (cbPrincipalHdr.SelectedItem != null ? cbPrincipalHdr.SelectedItem.Text : string.Empty);

        //string dateJs = null;
        //DateTime date = DateTime.Today;

        if (isConfirmed)
        {
          scrpt = string.Format(@"var rec = {0}.getById('{1}');
                                  if(!Ext.isEmpty(rec)) {{
                                    rec.set('l_confirm', true);
                                  }}", storeId, numberId);
        }

        if (!string.IsNullOrEmpty(numberId))
        {
          this.PopulateDetail("c_plno", numberId);
        }

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
        if (respon.Message == "KeteranganED")
        {
            Functional.ShowMsgInformation("Periksa keterangan Acc ED");
            e.Success = true;
        }
        else
        {
            e.ErrorMessage = "Unknown response";
            e.Success = false;
        }
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
    
    if (!pag.IsAllowPrinting)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
      return;
    }

    string gdgId = "6";
    string numberID = (e.ExtraParams["NumberID"] ?? string.Empty);

    if (string.IsNullOrEmpty(numberID))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");

      return;
    }

    ReportParser rptParse = new ReportParser();
    
    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();

    rptParse.ReportingID = "10101";
    
    #region Report Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(char).FullName,
      ParameterName = "LG_PLH.c_gdg",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_PLH.c_plno",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "({LG_PLD1.n_qty} <> 0)",
      IsReportDirectValue = true
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "({MsTransD.c_portal} = '9')",
        IsReportDirectValue = true
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "({MsTransD.c_notrans} = '001')",
        IsReportDirectValue = true
    });

    #endregion

    #region Linq Filter Parameter
    
    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(char).FullName,
      ParameterName = "c_gdg = @0",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId),
      IsLinqFilterParameter = true
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "c_plno = @0",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID),
      IsLinqFilterParameter = true
    });

    #endregion

    rptParse.PaperID = "8.5x5.5";
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;
    rptParse.UserDefinedName = numberID;

    string xmlFiles = ReportParser.Deserialize(rptParse);

    SoaReportCaller soa = new SoaReportCaller();

    string result = soa.GeneratorReport(xmlFiles);
    
    ReportingResult rptResult = ReportingResult.Serialize(result);

    if (rptResult == null)
    {
      Functional.ShowMsgError("Pembuatan report gagal.");
    }
    else
    {
      if (rptResult.IsSuccess)
      {
        //string rptName = string.Concat("Packing_List_", pag.Nip, ".", rptResult.Extension);

        //string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
        //tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
        //  tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

        string tmpUri = Functional.UriDownloadGenerator(pag,
          rptResult.OutputFile, "Packing List ", rptResult.Extension);

        wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
      }
      else
      {
        Functional.ShowMsgWarning(rptResult.MessageResponse);
      }
    }

    GC.Collect();
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