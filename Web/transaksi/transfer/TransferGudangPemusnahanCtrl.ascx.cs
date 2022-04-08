using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_transfer_TransferGudangPemusnahanCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Transfer Gudang Pemusnahan";

    hfSJNo.Clear();

    lbGudangFrom.Text = hfGudangDesc.Text;

    txNoDok.Clear();
    txNoDok.Disabled = false;

    cbToHdr.Clear();
    cbToHdr.Disabled = false;
    Ext.Net.Store cbToHdrStr = cbToHdr.GetStore();
    if (cbToHdrStr != null)
    {
        cbToHdrStr.RemoveAll();
    }
    Functional.SetComboData(cbToHdr, "c_gdg", "Gudang Pemusnahan", "5");

    cbAsalProduk.Clear();
    cbAsalProduk.Disabled = false;
    Ext.Net.Store cbAsalProdukStr = cbAsalProduk.GetStore();
    if (cbAsalProdukStr != null)
    {
        cbAsalProdukStr.RemoveAll();
    }

    cbMemoHdr.Clear();
    cbMemoHdr.Disabled = false;
    Ext.Net.Store cbMemoHdrStr = cbMemoHdr.GetStore();
    if (cbMemoHdrStr != null)
    {
        cbMemoHdrStr.RemoveAll();
    }

    txCabang.Clear();

    txKet.Clear();
    txKet.Disabled = false;

    cbItemDtl.Clear();
    cbItemDtl.Disabled = false;
    Ext.Net.Store cbItemDtlStr = cbItemDtl.GetStore();
    if (cbItemDtlStr != null)
    {
        cbItemDtlStr.RemoveAll();
    }

    cbBatDtl.Clear();
    cbBatDtl.Disabled = false;
    Ext.Net.Store cbBatDtlStr = cbBatDtl.GetStore();
    if (cbBatDtlStr != null)
    {
        cbBatDtlStr.RemoveAll();
    }

    btnPrint.Hidden = true;

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private void PopulateDetail(string pName, string pID)
  {
    ClearEntrys();
    Dictionary<string, object> dicSJ = null;
    Dictionary<string, string> dicSJInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string[][] paramX = new string[][]{
            new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
        };

    string tmp = null,
        tmp2 = null;

    bool isConfirm = false;
    bool isConfirmed = false;

    bool.TryParse(hfConfMode.Text, out isConfirm);

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0020", paramX);

    try
    {
      dicSJ = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicSJ.ContainsKey("records") && (dicSJ.ContainsKey("totalRows") && (((long)dicSJ["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicSJ["records"]);

        dicSJInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("Transfer Gudang - {0}", pID);

        Functional.SetComboData(cbToHdr, "c_gdg2", dicSJInfo.GetValueParser<string>("v_to", string.Empty), dicSJInfo.GetValueParser<string>("c_gdg2", string.Empty));
        cbToHdr.Disabled = true;

        tmp = (dicSJInfo.ContainsKey("l_confirm") ? dicSJInfo["l_confirm"] : string.Empty);
        if (!bool.TryParse(tmp, out isConfirmed))
        {
          isConfirmed = (tmp.Equals("1", StringComparison.OrdinalIgnoreCase) ? true : false);
        }

        Functional.SetComboData(cbAsalProduk, "c_product_origin", dicSJInfo.GetValueParser<string>("v_ket_product_origin", string.Empty), dicSJInfo.GetValueParser<string>("c_product_origin", string.Empty));
        cbAsalProduk.Disabled = false;

        tmp2 = dicSJInfo.GetValueParser<string>("c_product_origin", string.Empty);
            //((dicSJInfo.ContainsKey("v_product_origin")) ? dicSJInfo["v_product_origin"] : string.Empty);

        txNoDok.Text = ((dicSJInfo.ContainsKey("v_nodok")) ? dicSJInfo["v_nodok"] : string.Empty);
        txNoDok.Disabled = false;

        if (tmp2 == "01")
        {
            Functional.SetComboData(cbMemoHdr, "c_mpno", dicSJInfo.GetValueParser<string>("c_mpno", string.Empty), dicSJInfo.GetValueParser<string>("c_mpno", string.Empty));
            cbMemoHdr.Disabled = false;

            txCabang.Text = " ";
            txCabang.Hidden = true;

            txNoDok.Disabled = true;

            cbMemoHdr.Hidden = false;

        }
        else if(tmp2== "02")
        {
            txCabang.Text = " ";
            txCabang.Hidden = true;
            cbMemoHdr.Hidden = true;
            txNoDok.Disabled = false;
        }
        else
        {
            txCabang.Hidden = false;
            txCabang.Text = dicSJInfo.GetValueParser<string>("v_product_origin", string.Empty); 
            cbMemoHdr.Hidden = true;
        }

        txKet.Text = dicSJInfo.GetValueParser<string>("v_ket", string.Empty);
        txKet.Disabled = false;

        btnSave.Disabled = false;

        chkConfirm.Checked = isConfirmed;

        if (isConfirmed)
        {
            cbAsalProduk.Disabled = true;
            txNoDok.Disabled = true;
            txKet.Disabled = true;
            chkConfirm.Disabled = true;
            btnSave.Disabled = true;
            txCabang.Disabled = true;
            cbMemoHdr.Disabled = true;
        }

        btnPrint.Hidden = (isConfirmed ? true : false);
        chkConfirm.Hidden = true;

        jarr.Clear();

        hfSJNo.Text = pID;
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_transfer_TransferGudang:PopulateDetail Header - ", ex.Message));
    }
    finally
    {
      //SetEditoryConfirm(isConfirmed, isConfirm);

      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicSJInfo != null)
      {
        dicSJInfo.Clear();
      }
      if (dicSJ != null)
      {
        dicSJ.Clear();
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
        string.Concat("transaksi_transfer_TransferGudang:PopulateDetail Detail - ", ex.Message));
    }


    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string gdgAsal, bool isConfirm, string doNumberID, Dictionary<string, string>[] dics)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
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
    item = null, ket = null,
    batch = null;

    decimal nQty = 0, nBQty = 0, 
      nGqtyH = 0, nBqtyH = 0,
      sGqty = 0, sBqty = 0;
    bool isNew = false,
      isVoid = false, isModify = false;
    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", pag.Nip);
    pair.DicAttributeValues.Add("From", gdgAsal);
    pair.DicAttributeValues.Add("To", cbToHdr.SelectedItem.Value);
    pair.DicAttributeValues.Add("asalProduk", cbAsalProduk.SelectedItem.Value);
    pair.DicAttributeValues.Add("cabangExp", string.IsNullOrEmpty(txCabang.Text.Trim()) ? string.Empty : txCabang.Text.Trim());
    pair.DicAttributeValues.Add("memo", string.IsNullOrEmpty(cbMemoHdr.SelectedItem.Value) ? string.Empty : cbMemoHdr.SelectedItem.Value);
    pair.DicAttributeValues.Add("Keterangan", txKet.Text);
    pair.DicAttributeValues.Add("noDok", txNoDok.Text.Trim());
    pair.DicAttributeValues.Add("Supplier", string.Empty);
    pair.DicAttributeValues.Add("isDisposal", "true");

    if (isConfirm)
    {
      pair.DicAttributeValues.Add("Confirm", isConfirm.ToString().ToLower());
    }

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
        batch = dicData.GetValueParser<string>("c_batch");
        nQty = 0;
        nBQty = dicData.GetValueParser<decimal>("n_bqty", 0);

        if ((!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("Qty", nQty.ToString());
          dicAttr.Add("BQty", nBQty.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr
          });
        }
      }
      #region Modify
      else if ((!isNew) && (!isVoid) && isModify)
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        ket = dicData.GetValueParser<string>("v_ket");
        batch = dicData.GetValueParser<string>("c_batch");
        nQty = 0;
        nBQty = dicData.GetValueParser<decimal>("n_bqty", 0);
        nBqtyH = dicData.GetValueParser<decimal>("n_bqtyH", 0);
        nGqtyH = 0;
        sGqty = 0;
        sBqty = nBQty - nBqtyH;

        if ((!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("Qty", nQty.ToString());
          dicAttr.Add("BQty", nBQty.ToString());
          dicAttr.Add("sBQty", sBqty.ToString());
          dicAttr.Add("sGQty", sGqty.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
            DicAttributeValues = dicAttr
          });
        }
      }
      #endregion
      else if ((!isNew) && isVoid && (!isModify))
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        ket = dicData.GetValueParser<string>("v_ket");
        nQty = 0;
        nBQty = dicData.GetValueParser<decimal>("n_bqty", 0);

        if ((!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("Qty", nQty.ToString());
          dicAttr.Add("BQty", nBQty.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
            DicAttributeValues = dicAttr
          });
        }
      }

      dicData.Clear();
    }

    try
    {
      if (isConfirm)
      {
        varData = parser.ParserData("TransferGudangRepack", "ModifyConfirm", dic);
      }
      else
      {
        varData = parser.ParserData("TransferGudangRepack", (isAdd ? "Add" : "Modify"), dic);
      }
    }
    catch (Exception ex)
    {
        Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_TransferGudangPemusnahanCtrl SaveParser : {0} ", ex.Message);
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

  private void SetEditoryConfirm(bool hasConfirm, bool isConfirm)
  {
    if (isConfirm)
    {

      X.AddScript(@"var verifyRowEditConfirmPemusnahan = function(e) {
                    if (e.field == 'n_gqty')
                    {
                      var n_gqty = e.record.get('n_gqty');                 
                      if (e.value > n_gqty) {
                        e.cancel = true;
                        ShowWarning('Jumlah tidak boleh melebihi batas.');
                      }
                      else if(e.value == n_gqty) {
                        e.record.reject();
                      }
                      else {
                        e.record.set('l_modified', true);
                      }
                    }else if (e.field == 'n_bqty'){
                      var n_bqty = e.record.get('n_bqty');                 
                      if (e.value > n_bqty) {
                        e.cancel = true;
                        ShowWarning('Jumlah tidak boleh melebihi batas.');
                      }
                      else if(e.value == n_bqty) {
                        e.record.reject();
                      }
                      else {
                        e.record.set('l_modified', true);
                      }
                    }
                    };");

      X.AddScript(
        string.Format(@"var tryFindColIndexPemusnahan = function () {{
                          var idx = {0}.findColumnIndex('n_gqty');
                          var idxb = {0}.findColumnIndex('n_bqty');
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
                          if(idxb != -1) {{
                            {0}.setEditable(idxb, true);
                            {0}.setEditor(idx, new Ext.form.NumberField({{
                              allowBlank: false,
                              allowDecimals: true,
                              allowNegative: false,
                              minValue: 0.00
                            }}));
                          }}
                          idx = {0}.findColumnIndex('v_ket_type_dc');
                            if(idx != -1) {{
                              {0}.setEditable(idxb, true);
                            }}
                        }};
                        idx = {0}.findColumnIndex('v_ket_type_dc');
                        if(idx != -1) {{
                          {0}.setHidden(idx, false);
                        }}
                        tryFindColIndexPemusnahan();", gridDetail.ColumnModel.ClientID));

      gridDetail.AddListener("ValidateEdit", "verifyRowEditConfirmPemusnahan");

      frmHeaders.Height = new Unit(225);
      chkConfirm.Hidden = false;
    }
    else
    {
      X.AddScript(
        string.Format(@"var idx = {0}.findColumnIndex('n_gqty');
                        var idxb = {0}.findColumnIndex('n_bqty');
              if(idx != -1 || idxb != -1) {{
                {0}.setEditable(idx, false);
                {0}.setEditable(idxb, false);
              }}
              idx = {0}.findColumnIndex('v_ket_type_dc');
              if(idx != -1) {{
                {0}.setHidden(idx, true);
              }}", gridDetail.ColumnModel.ClientID));

      frmHeaders.Height = new Unit(195);
      chkConfirm.Hidden = true;
    }
  }

  #endregion

  public void Initialize(string gudang, string gudangDesc, string storeIDGridMain, bool isConfirm, string mode)
  {
    hfGudang.Text = gudang;
    hfStoreID.Text = storeIDGridMain;
    hfGudangDesc.Text = gudangDesc;
    hfmode.Text = mode;

    //if (gudang == "1")
    //{
    //    Ext.Net.Store storeT = cbToHdr.GetStore();
    //    string pNames = "c_gdg";
    //    string pIDs = "4";
    //    storeT.BaseParams.Add(new Ext.Net.Parameter("parameters", string.Format("[['{0} = @0', {1}, 'System.Char']]", pNames, pIDs), ParameterMode.Raw));
    //}
    //else if (gudang == "4")
    //{
    //    Ext.Net.Store storeT = cbToHdr.GetStore();
    //    string pNames = "c_gdg";
    //    string pIDs = "1";
    //    storeT.BaseParams.Add(new Ext.Net.Parameter("parameters", string.Format("[['{0} = @0', {1}, 'System.Char']]", pNames, pIDs), ParameterMode.Raw));
    //}

    if (isConfirm)
    {
      hfConfMode.Text = bool.TrueString.ToLower();
    }
  }

  public void CommandPopulate(bool isAdd, string pID)
  {
    bool isConfirm = false;

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    bool.TryParse(hfConfMode.Text, out isConfirm);

    ClearEntrys();

    if (isAdd && isConfirm)
    {
      Functional.ShowMsgError("(UC) Perintah tidak dikenal.");
    }
    else if (isAdd)
    {
      //SetEditoryConfirm(false, false);
      //frmHeaders.Height = new Unit(200);

      chkConfirm.Hidden = true;
      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      chkConfirm.Hidden = false;
      PopulateDetail("c_sjno", pID);
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void btnSimpan_OnClick(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string gdgId = pag.ActiveGudang;
    string NumberID = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jSonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);

    bool isConfirm = false,
    isConfirmed = false;

    if (bool.TryParse(hfConfMode.Text, out isConfirm))
    {
      isConfirmed = (chkConfirm.Checked ? true : false);
    }

    Dictionary<string, string>[] gridDataDO = JSON.Deserialize<Dictionary<string, string>[]>(jSonGridValues);

    bool isAdd = (string.IsNullOrEmpty(NumberID) ? true : false);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, gdgId, isConfirm, NumberID, gridDataDO);
    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;
        string storeId = hfStoreID.Text;

        string fromHdr = hfGudangDesc.Text; //(cbFromHdr.SelectedIndex != -1 ? cbFromHdr.SelectedItem.Text : string.Empty);
        string toHdr = (cbToHdr.SelectedIndex != -1 ? cbToHdr.SelectedItem.Text : string.Empty);
        string Ket = txKet.Text;
        string gdgTo = (cbToHdr.Text != string.Empty ? cbToHdr.Value.ToString() : string.Empty);

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

            NumberID = respon.Values.GetValueParser<string>("SJ", string.Empty);

            if (!string.IsNullOrEmpty(storeId))
            {
              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'v_from': '{1}',
                'c_sjno': '{2}',
                'd_sjdate': {3},
                'v_to': '{4}',
                'v_ket': '{5}',
                'l_print': false,
                'l_status': false,
                'l_confirm': false,
                'c_expno':''
              }}));{0}.commitChanges();", storeId, fromHdr,
                    NumberID,
                       dateJs, toHdr, Ket);

              X.AddScript(scrpt);
            }
          }
        }

        //ClearEntrys();

        if (!string.IsNullOrEmpty(NumberID))
        {
          PopulateDetail("c_sjno", NumberID);
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
      e.ErrorMessage = "Unknown response";

      e.Success = false;
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string gdgId = pag.ActiveGudang;
    string numberID = (e.ExtraParams["NumberID"] ?? string.Empty);

    if (string.IsNullOrEmpty(numberID))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");

      return;
    }

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();

    rptParse.ReportingID = "10110-b";

    #region Report Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(char).FullName,
      ParameterName = "LG_SJH.c_gdg",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_SJH.c_sjno",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_SJH.c_type",
      ParameterValue = "02"
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
      ParameterName = "c_sjno = @0",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID),
      IsLinqFilterParameter = true
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "c_type = @0",
      ParameterValue = "02",
      IsLinqFilterParameter = true
    });

    #endregion

    rptParse.PaperID = "8.5x5.5";
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;

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
          rptResult.OutputFile, "Pesanan Gudang ", rptResult.Extension);

        wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
      }
      else
      {
        Functional.ShowMsgWarning(rptResult.MessageResponse);
      }
    }

    GC.Collect();
  }
}
