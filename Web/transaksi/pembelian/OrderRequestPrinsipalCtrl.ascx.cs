using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_pembelian_OrderRequestPrinsipalCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Order Request Pemasok";

    hfOrNo.Clear();

    cbSuplierHdr.Clear();
    cbSuplierHdr.Disabled = false;

    txKeterangan.Clear();
    txKeterangan.Disabled = false;

    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));

    cbItemDtl.Disabled = false;
    txQtyDtl.Disabled = false;
    btnAdd.Disabled = false;
    btnClear.Disabled = false;

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

    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    Scms.Web.Core.PageHandler page = this.Page as Scms.Web.Core.PageHandler;

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"},
        new string[] { "gudang", page.ActiveGudang, "System.Char"}
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

        winDetail.Title = string.Format("Order Request Pemasok - {0}", pID);

        //cbSuplierHdr.ToBuilder().AddItem(
        //  (dicSPInfo.ContainsKey("v_nama") ? dicSPInfo["v_nama"] : string.Empty),
        //  (dicSPInfo.ContainsKey("c_nosup") ? dicSPInfo["c_nosup"] : string.Empty)
        //);
        //if (cbSuplierHdr.GetStore() != null)
        //{
        //  cbSuplierHdr.GetStore().CommitChanges();
        //}
        //cbSuplierHdr.SetValueAndFireSelect((dicSPInfo.ContainsKey("c_nosup") ? dicSPInfo["c_nosup"] : string.Empty));
        Functional.SetComboData(cbSuplierHdr, "c_nosup", dicResultInfo.GetValueParser<string>("v_nama", string.Empty), dicResultInfo.GetValueParser<string>("c_nosup", string.Empty));
        cbSuplierHdr.Disabled = true;

        isComplete = dicResultInfo.GetValueParser<bool>("orProses");

        txKeterangan.Text = dicResultInfo.GetValueParser<string>("v_ket", string.Empty);
        
        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_pembelian_OrderRequestPrinsipalCtrl:PopulateDetail Header - ", ex.Message));
    }
    finally
    {
      SetEditor(!isComplete);

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
            store.BaseParams["parameters"] = string.Format("[['orno', '{1}', 'System.String']]", pName, pID);
          }
        }
      }

      hfOrNo.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_pembelian_OrderRequestPrinsipalCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string plNumber, Dictionary<string, string>[] dics, string suplierId, string keteranganHeader)
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
      item = null,
      ket = null;
    decimal nQty = 0,
      nAcc = 0;
    bool isNew = false,
      isVoid = false,
      isModify = false;
    string varData = null;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Suplier", suplierId);
    //pair.DicAttributeValues.Add("Tanggal", date.ToString("yyyyMMddHHmmssfff"));
    //pair.DicAttributeValues.Add("SPCabang", txSpCabang.Text.Trim());
    pair.DicAttributeValues.Add("Keterangan", keteranganHeader);
    pair.DicAttributeValues.Add("Tipe", "05");
    //pair.DicAttributeValues.Add("Cek", chkCheck.Checked.ToString().ToLower());

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
        nQty = dicData.GetValueParser<decimal>("n_Qty", 0);
        nAcc = dicData.GetValueParser<decimal>("n_QtyOrd", 0);

        if ((!string.IsNullOrEmpty(item)) &&
          (nQty > 0) &&
          (nAcc > 0))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Qty", nQty.ToString());
          dicAttr.Add("QtyOrd", nAcc.ToString());

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
        
        if (!string.IsNullOrEmpty(item))
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
      else if ((!isNew) && (!isVoid) && isModify)
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        nQty = dicData.GetValueParser<decimal>("n_Qty", 0);
        ket = dicData.GetValueParser<string>("v_ket");

        if ((!string.IsNullOrEmpty(item)) &&
          (nQty > 0))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Qty", nQty.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Modify" : ket),
            DicAttributeValues = dicAttr
          });
        }
      }

      dicData.Clear();
    }

    try
    {
      varData = parser.ParserData("OrderRequestPrincipal", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_OrderRequestPrinsipalCtrl SaveParser : {0} ", ex.Message);
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
  
  private void SetEditor(bool isEditing)
  {
    if (isEditing)
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
                      var accQty = e.record.get('n_QtyApprove');
                      var sisa = e.record.get('n_sisa');
                      var totAccQty = e.record.get('n_totalAcc');
                      var lMod = e.record.get('l_modified');
                      var oAccQty = e.record.get('n_QtyOriginal');
                      if (accQty != sisa) {
                        e.cancel = true;
                        ShowWarning('Maaf, barang yang telah terproses tidak dapat diubah.');
                      }
                      else if(e.value == oAccQty) {
                        e.record.reject();
                      }
                      else {
                        e.record.set('n_sisa', e.value);
                        e.record.set('n_QtyApprove', e.value);
                        e.record.set('n_totalAcc', ((sisa > e.value) ? (totAccQty - (sisa - e.value)) : (totAccQty + (e.value - sisa))));
                        e.record.set('l_modified', true);
                        if(!lMod) {
                          e.record.set('n_QtyOriginal', accQty);
                        }
                      }
                    };");

      X.AddScript(
        string.Format(@"var tryFindColIndex = function () {{
                          var idx = {0}.findColumnIndex('n_QtyApprove');
                          if(idx != -1) {{
                            {0}.setEditable(idx, true);
                            {0}.setEditor(idx, new Ext.form.NumberField({{
                              allowBlank: false,
                              allowDecimals: true,
                              allowNegative: false,
                              minValue: 0.00
                            }}));
                          }}
                        }};
                        tryFindColIndex();", gridDetail.ColumnModel.ClientID));

      gridDetail.AddListener("ValidateEdit", "verifyRowEditConfirm");
    }
    else
    {
      X.AddScript(
        string.Format(@"var idx = {0}.findColumnIndex('n_QtyRequest');
              if(idx != -1) {{
                {0}.setEditable(idx, false);
              }}", gridDetail.ColumnModel.ClientID));
    }
  }

  #endregion

  public void Initialize(string storeIDGridMain, string typeName)
  {
    hfStoreID.Text = storeIDGridMain;
    hfTypeNameCtrl.Text = typeName;
  }

  public void CommandPopulate(bool isAdd, string pID)
  {
    if (isAdd)
    {
      SetEditor(false);

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
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string suplierId = (e.ExtraParams["SuplierID"] ?? string.Empty);
    string ketHeader = (e.ExtraParams["Keterangan"] ?? string.Empty);
    string typeName = (e.ExtraParams["TypeName"] ?? string.Empty);
    string suplierName = (e.ExtraParams["SuplierName"] ?? string.Empty);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

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

    Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataPL, suplierId, ketHeader);

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
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    if (!pag.IsAllowPrinting)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
      return;
    }

    string gdgId = pag.ActiveGudang;
    string custId = (e.ExtraParams["CustomeID"] ?? string.Empty);
    string pl1 = (e.ExtraParams["PLID1"] ?? string.Empty);
    string pl2 = (e.ExtraParams["PLID2"] ?? string.Empty);
    string tmp = (e.ExtraParams["Async"] ?? string.Empty);

    if (string.IsNullOrEmpty(gdgId) && string.IsNullOrEmpty(custId) &&
      string.IsNullOrEmpty(pl1) && string.IsNullOrEmpty(pl2))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");
      return;
    }

    ReportParser rptParse = new ReportParser();
    
    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();
    bool isAsync = false;

    bool.TryParse(tmp, out isAsync);

    rptParse.ReportingID = "10101";
    
    #region Report Parameter
    
    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_PLH.c_gdg",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_PLH.c_cusno",
      ParameterValue = (string.IsNullOrEmpty(custId) ? string.Empty : custId)
    });

    if (!string.IsNullOrEmpty(pl1))
    {
      if (string.IsNullOrEmpty(pl2))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_PLH.c_plno",
          ParameterValue = (string.IsNullOrEmpty(pl1) ? string.Empty : pl1)
        });
      }
      else
      {
        if (pl1.CompareTo(pl2) <= 0)
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "LG_PLH.c_plno",
            ParameterValue = (string.IsNullOrEmpty(pl1) ? string.Empty : pl1)

          });
        }
        else
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{LG_PLH.c_plno}} IN ({0} TO {1}))", pl1, pl2),
            IsReportDirectValue = true
          });
        }
      }
    }

    #endregion

    rptParse.PaperID = "8.5x5.5";
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;

    string xmlFiles = ReportParser.Deserialize(rptParse);

    SoaReportCaller soa = new SoaReportCaller();

    //string result = soa.GeneratorReport(isAsync, xmlFiles);
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