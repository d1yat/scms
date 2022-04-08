using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_pembelian_SuratPesananCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Surat Pesanan";

    hfSpNo.Clear();

    cbCustomerHdr.Clear();
    cbCustomerHdr.Disabled = false;

    txKeterangan.Clear();
    txKeterangan.Disabled = false;

    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));

    txTanggal.Clear();
    txTanggal.Disabled = false;

    txSpCabang.Clear();
    txSpCabang.Disabled = false;

    //cbItemDtl.Disabled = false;
    //txQtyDtl.Disabled = false;
    //txAccDtl.Disabled = false;
    //txKetDtl.Disabled = false;

    chkCheck.Clear();
    chkCheck.Disabled = false;

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

    Dictionary<string, object> dicSP = null;
    Dictionary<string, string> dicSPInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string tmp = null;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0011", paramX);
    bool isCheck = false;
    bool isComplete = false;

    try
    {
      dicSP = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicSP.ContainsKey("records") && (dicSP.ContainsKey("totalRows") && (((long)dicSP["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicSP["records"]);

        dicSPInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("Surat Pesanan - {0}", pID);

        //cbCustomerHdr.ToBuilder().AddItem(
        //  (dicSPInfo.ContainsKey("v_cunam") ? dicSPInfo["v_cunam"] : string.Empty),
        //  (dicSPInfo.ContainsKey("c_cusno") ? dicSPInfo["c_cusno"] : string.Empty)
        //);
        //if (cbCustomerHdr.GetStore() != null)
        //{
        //  cbCustomerHdr.GetStore().CommitChanges();
        //}
        //cbCustomerHdr.SetValueAndFireSelect((dicSPInfo.ContainsKey("c_cusno") ? dicSPInfo["c_cusno"] : string.Empty));

        Functional.SetComboData(cbCustomerHdr, "c_cusno", dicSPInfo.GetValueParser<string>("v_cunam", string.Empty), dicSPInfo.GetValueParser<string>("c_cusno", string.Empty));
        cbCustomerHdr.Disabled = true;

        tmp = dicSPInfo.GetValueParser<string>("d_spdate", string.Empty);

        isCheck = dicSPInfo.GetValueParser<bool>("l_cek");
        isComplete = dicSPInfo.GetValueParser<bool>("spComplete");

        chkCheck.Checked = isCheck;

        DateTime date = Functional.JsonDateToDate(tmp);

        txTanggal.Text = date.ToString("dd-MM-yyyy");
        txTanggal.Disabled = true;

        txSpCabang.Text = dicSPInfo.GetValueParser<string>("c_sp", string.Empty);
        txSpCabang.Disabled = true;

        txKeterangan.Text = dicSPInfo.GetValueParser<string>("v_ket", string.Empty);

        tmp = dicSPInfo.GetValueParser<string>("d_etdsp", string.Empty);
        date = Functional.JsonDateToDate(tmp);
        dtEtdCab.Text = date.ToString("dd-MM-yyyy");

        tmp = dicSPInfo.GetValueParser<string>("d_etasp", string.Empty);
        date = Functional.JsonDateToDate(tmp);
        dtEtaCab.Text = date.ToString("dd-MM-yyyy");

        jarr.Clear();

        btnPrint.Hidden = false;
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_pembelian_SuratPesananCtrl:PopulateDetail Header - ", ex.Message));
    }
    finally
    {
      SetEditor(!isComplete);

      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicSPInfo != null)
      {
        dicSPInfo.Clear();
      }
      if (dicSP != null)
      {
        dicSP.Clear();
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

      hfSpNo.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_pembelian_SuratPesananCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string plNumber, Dictionary<string, string>[] dics)
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
      ket = null,
      ketEdit = null;
    decimal nQty = 0,
      nAcc = 0;
    bool isNew = false,
      isVoid = false,
      isModify = false;
    string varData = null;

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    DateTime date = DateTime.Today;

    if (!Functional.DateParser(txTanggal.RawText, "dd-MM-yyyy", out date))
    {
      responseResult = new PostDataParser.StructureResponse()
      {
        IsSet = true,
        Message = "Format penulisan tanggal salah.",
        Response = PostDataParser.ResponseStatus.Failed
      };

      return responseResult;
    }

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", pag.Nip);
    pair.DicAttributeValues.Add("Customer", cbCustomerHdr.Text);
    pair.DicAttributeValues.Add("Tanggal", date.ToString("yyyyMMddHHmmssfff"));
    pair.DicAttributeValues.Add("SPCabang", txSpCabang.Text.Trim());
    pair.DicAttributeValues.Add("Keterangan", txKeterangan.Text.Trim());
    pair.DicAttributeValues.Add("Tipe", "03");
    pair.DicAttributeValues.Add("Cek", chkCheck.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("D_ETD", dtEtdCab.Text.ToString());
    pair.DicAttributeValues.Add("D_ETA", dtEtaCab.Text.ToString());

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      isNew = dicData.GetValueParser<bool>("l_new");
      isVoid = dicData.GetValueParser<bool>("l_void");
      isModify = dicData.GetValueParser<bool>("l_modified");

      if (isNew && (!isVoid) && (!isModify))
      {
        if(!pag.IsAllowAdd)
        {
          continue;
        }

        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        nQty = dicData.GetValueParser<decimal>("n_QtyRequest", 0);
        nAcc = dicData.GetValueParser<decimal>("n_QtyApprove", 0);
        ket = dicData.GetValueParser<string>("v_keterangan");

        if ((!string.IsNullOrEmpty(item)) &&
          (nQty > 0) &&
          (nAcc > 0))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Qty", nQty.ToString());
          dicAttr.Add("Acc", nAcc.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? string.Empty : ket.Trim()),
            DicAttributeValues = dicAttr
          });
        }
      }
      else if ((!isNew) && isVoid && (!isModify))
      {
        if(!pag.IsAllowDelete)
        {
          continue;
        }

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
        if(!pag.IsAllowEdit)
        {
          continue;
        }

        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        ket = dicData.GetValueParser<string>("v_ket");
        ketEdit = dicData.GetValueParser<string>("v_keterangan");
        nAcc = dicData.GetValueParser<decimal>("n_QtyApprove", 0);

        if (!string.IsNullOrEmpty(item))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Acc", nAcc.ToString());
          dicAttr.Add("Keterangan", ketEdit);

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
      varData = parser.ParserData("SuratPesanan", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_SuratPesananCtrl SaveParser : {0} ", ex.Message);
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
                      var qtyReq = e.record.get('n_QtyRequest');
                      if((!lMod) && ((accQty == 0) || (sisa <= 0))) {
                        e.cancel = true;
                      }
                      else if (e.value > qtyReq) {
                        e.cancel = true;
                        ShowWarning('Maaf, jumlah acc tidak boleh lebih dari quantity.');
                      }
                      else if (accQty != sisa) {
                        e.cancel = true;
                        ShowWarning('Maaf, barang yang telah terproses tidak dapat diubah.');
                      }
                      else if((e.value == oAccQty) && (e.value != 0)) {
                        e.record.reject();
                      }
                      else {
                        e.record.set('n_QtyApprove', e.value);
                        e.record.set('n_sisa', e.value);
                        e.record.set('l_modified', true);
                        if(e.value == 0) {
                          e.record.set('n_totalAcc', 0);
                        }
                        else {
                          e.record.set('n_totalAcc', ((sisa > e.value) ? (totAccQty - (sisa - e.value)) : (totAccQty + (e.value - sisa))));
                        }
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

      txTanggal.Text = DateTime.Now.ToString("dd-MM-yyyy");
      dtEtaCab.MinDate = DateTime.Now;
      dtEtdCab.MinDate = DateTime.Now;
      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      PopulateDetail("c_spno", pID);
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);

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

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataPL);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;
        string storeId = hfStoreID.Text;

        string cust = (cbCustomerHdr.SelectedIndex != -1 ? cbCustomerHdr.SelectedItem.Text : string.Empty);

        string dateJs = null;
        DateTime date = DateTime.Today;

        if (isAdd)
        {
          if (respon.Values != null)
          {
            numberId = respon.Values.GetValueParser<string>("SP", string.Empty);

            if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
            {
              dateJs = Functional.DateToJson(date);
            }

            if (!string.IsNullOrEmpty(storeId))
            {
              if (!Functional.DateParser(txTanggal.RawText, "dd-MM-yyyy", out date))
              {
                date = DateTime.Now;
              }

              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_spno': '{1}',
                'd_spdate': {2},
                'c_sp': '{3}',
                'v_type_desc': '{4}',
                'v_cunam': '{5}',
                'spComplete': false,
                'l_print': false,
                'l_confirm': false,
                'd_spinsert': {6}
              }}));{0}.commitChanges();", storeId, numberId,
                        Functional.DateToJson(date), txSpCabang.Text,
                        hfTypeNameCtrl.Text, (cbCustomerHdr.SelectedItem == null ? cbCustomerHdr.Text : cbCustomerHdr.SelectedItem.Text), dateJs);

              X.AddScript(scrpt);
            }
          }
        }

        //this.ClearEntrys();        
        PopulateDetail("c_spno", numberId);

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

    string custId = (e.ExtraParams["CustomeID"] ?? string.Empty);
    string numberID = (e.ExtraParams["NumberID"] ?? string.Empty);

    if (string.IsNullOrEmpty(custId) && string.IsNullOrEmpty(numberID))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");
      return;
    }

    ReportParser rptParse = new ReportParser();
    
    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();

    rptParse.ReportingID = "10113";
    
    #region Report Parameter
    
    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_SPH.c_cusno",
      ParameterValue = (string.IsNullOrEmpty(custId) ? string.Empty : custId)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_SPH.c_spno",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID)
    });

    #endregion
	
    #region TipeReport

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(DateTime).FullName,
      ParameterName = "tipereport",
      IsSqlParameter = true,
      ParameterValue = "01"
    });

    #endregion
    #region Linq Filter Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(char).FullName,
      ParameterName = "c_cusno = @0",
      ParameterValue = (string.IsNullOrEmpty(custId) ? string.Empty : custId),
      IsLinqFilterParameter = true
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "c_spno = @0",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID),
      IsLinqFilterParameter = true
    });

    #endregion

    rptParse.PaperID = "Letter";
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
        string tmpUri = Functional.UriDownloadGenerator(pag,
          rptResult.OutputFile, "Surat Pesanan", rptResult.Extension);

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