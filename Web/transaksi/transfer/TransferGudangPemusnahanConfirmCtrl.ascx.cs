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

public partial class transaksi_transfer_TransferGudangPemusnahanConfirmCtrl : System.Web.UI.UserControl
{
  private void ClearEntrys()
  {
    winDetail.Title = "Transfer Gudang Pemusnahan Confirm";

    hfSJNo.Clear();

    //cbFromHdr.Clear();
    //cbFromHdr.Disabled = false;

    lbGudangFrom.Text = hfGudangDesc.Text;

    txNoDok.Clear();
    txNoDok.Disabled = false;

    chkConfirm.Checked = false;

    btnSave.Disabled = false;

    cbToHdr.Clear();
    cbToHdr.Disabled = false;
    Ext.Net.Store cbToHdrStr = cbToHdr.GetStore();
    if (cbToHdrStr != null)
    {
        cbToHdrStr.RemoveAll();
    }

    cbMemoHdr.Clear();
    cbMemoHdr.Disabled = false;
    Ext.Net.Store cbMemoHdrStr = cbMemoHdr.GetStore();
    if (cbMemoHdrStr != null)
    {
        cbMemoHdrStr.RemoveAll();
    }

    cbAsalProduk.Clear();
    cbAsalProduk.Disabled = false;
    Ext.Net.Store cbAsalProdukStr = cbAsalProduk.GetStore();
    if (cbAsalProdukStr != null)
    {
        cbAsalProdukStr.RemoveAll();
    }

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

  private void SetEditoryConfirm(bool isRN)
  {
    if (!isRN)
    {
      X.AddScript(@"var verifyRowEditConfirmPemusnahan = function(e) {
                     var bqty = e.record.get('n_QtyRequestBad');
                     
                    if (e.field == 'n_bqty')
                    {
                      if (e.value > bqty) {
                        e.cancel = true;
                        ShowWarning('Jumlah tidak boleh melebihi batas.');
                      }
                      else if(e.value == bqty) {
                        e.record.reject();
                      }
                      else {
                        e.record.set('l_modified', true);
                      }
                    }
                    };");

      X.AddScript(
        string.Format(@"var tryFindColIndexPemusnahan = function () {{
                          var idxb = {0}.findColumnIndex('n_bqty');
                          if(idxb != -1) {{
                            {0}.setEditable(idxb, true);
                            {0}.setEditor(idxb, new Ext.form.NumberField({{
                              allowBlank: false,
                              allowDecimals: true,
                              allowNegative: false,
                              minValue: 0.00
                            }}));
                          }}
                          idxb = {0}.findColumnIndex('v_ket_type_dc');
                            if(idxb != -1) {{
                              {0}.setEditable(idxb, true);
                            }}
                        }};
                        tryFindColIndexPemusnahan();", gridDetail.ColumnModel.ClientID));

      gridDetail.AddListener("ValidateEdit", "verifyRowEditConfirmPemusnahan");

      frmHeaders.Height = new Unit(230);
      //chkConfirm.Hidden = false;
      //btnSave.Hidden = false;
    }
    else
    {
      X.AddScript(
        string.Format(@"var idxb = {0}.findColumnIndex('n_bqty');
              if(idxb != -1) {{
                {0}.setEditable(idxb, false);
              }};"
        , gridDetail.ColumnModel.ClientID));

      frmHeaders.Height = new Unit(230);
      //chkConfirm.Hidden = true;
      //btnSave.Hidden = true;
    }
  }

  public void Initialize(string gudang, string gudangDesc, string storeIDGridMain, bool isConfirm, string mode)
  {
    hfGudang.Text = gudang;
    hfStoreID.Text = storeIDGridMain;
    hfGudangDesc.Text = gudangDesc;
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, bool isConfirm, string gdgAsal, string doNumberID, Dictionary<string, string>[] dics)
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
    pair.DicAttributeValues.Add("Confirm", chkConfirm.Checked.ToString().ToLower());

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
      else if ((!isNew) && (!isVoid) && isModify)
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        nQty = 0;
        nBQty = dicData.GetValueParser<decimal>("n_bqty", 0);

        if (nQty <= 0 && nBQty <= 0)
        {
          dicAttr.Add("New", "false");
          dicAttr.Add("Delete", "true");
          dicAttr.Add("Modified", "false");
        }
        else
        {
          dicAttr.Add("New", isNew.ToString().ToLower());
          dicAttr.Add("Delete", isVoid.ToString().ToLower());
          dicAttr.Add("Modified", isModify.ToString().ToLower());
        }

        item = dicData.GetValueParser<string>("c_iteno");
        ket = dicData.GetValueParser<string>("c_type_dc");
        batch = dicData.GetValueParser<string>("c_batch");
        //nQty = dicData.GetValueParser<decimal>("n_gqty", 0);
        //nBQty = dicData.GetValueParser<decimal>("n_bqty", 0);
        nBqtyH = dicData.GetValueParser<decimal>("n_bqtyH", 0);
        nGqtyH = 0;
        sGqty = 0;
        sBqty = nBQty - nBqtyH;

        if ((!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)) &&
          ((!string.IsNullOrEmpty(ket)) || (!ket.Equals("00", StringComparison.OrdinalIgnoreCase))))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("Qty", nQty.ToString());
          dicAttr.Add("BQty", nBQty.ToString());
          dicAttr.Add("sBQty", sBqty.ToString());
          dicAttr.Add("sGQty", sGqty.ToString());
          dicAttr.Add("tipe_dc", ket.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
            DicAttributeValues = dicAttr
          });
        }
      }
      #region Old Coded
      //else if ((!isNew) && isVoid && (!isModify))
      //{
      //  dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      //  dicAttr.Add("New", isNew.ToString().ToLower());
      //  dicAttr.Add("Delete", isVoid.ToString().ToLower());
      //  dicAttr.Add("Modified", isModify.ToString().ToLower());

      //  item = dicData.GetValueParser<string>("c_iteno");
      //  batch = dicData.GetValueParser<string>("c_batch");
      //  ket = dicData.GetValueParser<string>("v_ket");
      //  nQty = dicData.GetValueParser<decimal>("n_gqty", 0);
      //  nBQty = dicData.GetValueParser<decimal>("n_bqty", 0);

      //  if ((!string.IsNullOrEmpty(item)) &&
      //    (!string.IsNullOrEmpty(batch)))
      //  {
      //    dicAttr.Add("Item", item);
      //    dicAttr.Add("Batch", batch);
      //    dicAttr.Add("Qty", nQty.ToString());
      //    dicAttr.Add("BQty", nBQty.ToString());

      //    pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
      //    {
      //      IsSet = true,
      //      Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
      //      DicAttributeValues = dicAttr
      //    });
      //  }
      //}

      #endregion

      dicData.Clear();
    }

    try
    {
      varData = parser.ParserData("TransferGudangRepack", "ModifyConfirm", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_TransferGudangPemusnahanConfirmCtrl SaveParser : {0} ", ex.Message);
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

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void btnSimpan_OnClick(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string gdgId = pag.ActiveGudang;
    string NumberID = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jSonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);

    bool isConfirm = false,
    isConfirmed = false;

    isConfirmed = chkConfirm.Checked;
    bool isAdd = (string.IsNullOrEmpty(NumberID) ? true : false);

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

    //if (bool.TryParse(hfConfMode.Text, out isConfirm))
    //{
    //  isConfirmed = (chkConfirm.Checked ? true : false);
    //}

    Dictionary<string, string>[] gridDataDO = JSON.Deserialize<Dictionary<string, string>[]>(jSonGridValues);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, isConfirm, gdgId, NumberID, gridDataDO);
    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;
        string storeId = hfStoreID.Text;

        //string fromHdr = hfGudangDesc.Text; //(cbFromHdr.SelectedIndex != -1 ? cbFromHdr.SelectedItem.Text : string.Empty);
        //string toHdr = (cbToHdr.SelectedIndex != -1 ? cbToHdr.SelectedItem.Text : string.Empty);
        //string Ket = txKeterangan.Text;
        //string supl = (cbPrincipalHdr.SelectedIndex != -1 ? cbPrincipalHdr.SelectedItem.Text : string.Empty);

        //string dateJs = null;
        DateTime date = DateTime.Today;

        if (isConfirmed)
        {
          if (!string.IsNullOrEmpty(storeId))
          {
            scrpt = string.Format(@"var rec = {0}.getById('{1}');
                                  if(!Ext.isEmpty(rec)) {{
                                    rec.set('l_confirm', true);{0}.commitChanges();
                                  }}", storeId, NumberID);

            X.AddScript(scrpt);
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

    //bool isConfirm = false;
    bool isConfirmed = false;
    bool isRn = false;

    //bool.TryParse(hfConfMode.Text, out isConfirm);

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0020", paramX);

    try
    {
      dicSJ = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicSJ.ContainsKey("records") && (dicSJ.ContainsKey("totalRows") && (((long)dicSJ["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicSJ["records"]);

        dicSJInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("Transfer Gudang Pemusnahan Confirm - {0}", pID);

        tmp = (dicSJInfo.ContainsKey("l_status") ? dicSJInfo["l_status"] : string.Empty);
        if (!bool.TryParse(tmp, out isRn))
        {
          isRn = (tmp.Equals("1", StringComparison.OrdinalIgnoreCase) ? true : false);
        }

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

        if (tmp2 == "01")
        {
            Functional.SetComboData(cbMemoHdr, "c_mpno", dicSJInfo.GetValueParser<string>("c_mpno", string.Empty), dicSJInfo.GetValueParser<string>("c_mpno", string.Empty));
            cbMemoHdr.Disabled = false;

            txCabang.Text = " ";
            txCabang.Hidden = true;
            //txCabang.Visible = false;

            txNoDok.Disabled = true;

            //cbMemoHdr.Visible = true;
            cbMemoHdr.Hidden = false;

        }
        else if (tmp2 == "02")
        {
            txCabang.Text = " ";
            //txCabang.Visible = false;
            txCabang.Hidden = true;
            //cbMemoHdr.Visible = false;
            cbMemoHdr.Hidden = true;
            txNoDok.Disabled = false;
        }
        else
        {
            //txCabang.Visible = true;
            txCabang.Hidden = false;
            txCabang.Text = dicSJInfo.GetValueParser<string>("v_product_origin", string.Empty); ;
            //cbMemoHdr.Visible = false;
            cbMemoHdr.Hidden = true;
        }

        txNoDok.Text = ((dicSJInfo.ContainsKey("v_nodok")) ? dicSJInfo["v_nodok"] : string.Empty);
        txNoDok.Disabled = false;

        txKet.Text = dicSJInfo.GetValueParser<string>("v_ket", string.Empty);
        txKet.Disabled = false;

        chkConfirm.Checked = isConfirmed;
        chkConfirm.Disabled = false;

        btnSave.Disabled = false;

        cbItemDtl.Disabled = true;
        cbBatDtl.Disabled = true;
        txBQtyDtl.Disabled = true;
        btnAdd.Disabled = true;
        btnClear.Disabled = true;
        //txCabang.Disabled = true;


        cbAsalProduk.Disabled = true;
        txNoDok.Disabled = true;
        txKet.Disabled = true;
        txCabang.Disabled = true;
        cbMemoHdr.Disabled = true;

        if (isConfirmed)
        {
            chkConfirm.Disabled = true;
            btnSave.Disabled = true;
        }
        else
        {
            chkConfirm.Disabled = false;
            btnSave.Disabled = false;
        }

        //X.AddScript(string.Format("clearRelatedComboRecursive(true, {0});", cbItemDtl.ClientID));

        chkConfirm.Hidden = false;

        //btnPrint.Hidden = (isConfirmed ? false : true);
        btnPrint.Hidden = false;
        btnSave.Hidden = isRn;
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
      //SetEditoryConfirm(isRn);

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

  protected void Page_Load(object sender, EventArgs e)
  {

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
      PopulateDetail("c_sjno", pID);
    }
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
    List<ReportCustomizeText> lstCustTxt = new List<ReportCustomizeText>();

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

    lstCustTxt.Add(new ReportCustomizeText()
    {
        SectionName = "Section2",
        ControlName = "txtField",
        Value = "Supplier"
    });

    //lstCustTxt.Add(new ReportCustomizeText()
    //{
    //    SectionName = "Section2",
    //    ControlName = "txtSupplier",
    //    Value = ": " + cbPrincipalHdr.SelectedItem.Text
    //});

    rptParse.PaperID = "8.5x5.5";
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.ReportCustomizeText = lstCustTxt.ToArray();
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
