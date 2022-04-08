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

public partial class transaksi_transfer_TransferGudangCtrlGd3 : System.Web.UI.UserControl
{
  #region Private

  private void SetEditoryConfirm(bool hasRn, bool isConfirm)
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
    if (isConfirm)
    {
      if (hasRn)
      {
        X.AddScript(
        string.Format(@"var idx = {0}.findColumnIndex('n_gqty');
              if(idx != -1) {{
                {0}.setEditable(idx, false);
              }};", gridDetail.ColumnModel.ClientID));

        frmHeaders.Height = new Unit(150);
        chkConfirm.Disabled = true;
        btnSave.Hidden = true;
      }
      else
      {
        X.AddScript(@"var verifyRowEditConfirm = function(e) {
                      var qty = e.record.get('n_QtyRequest');
                      if (e.field == 'n_gqty')
                      {
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
                          var idx = {0}.findColumnIndex('n_gqty');
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
                        idx = {0}.findColumnIndex('v_ket_type_dc');
                        if(idx != -1) {{
                          {0}.setHidden(idx, false);
                        }}
                        idx = {0}.findColumnIndex('l_void');
                        if(idx != -1) {{
                          {0}.setHidden(idx, true);
                        }}
                        tryFindColIndex();", gridDetail.ColumnModel.ClientID));

        gridDetail.AddListener("ValidateEdit", "verifyRowEditConfirm");

        frmHeaders.Height = new Unit(175);
        chkConfirm.Hidden = false;
      }
    }
    else
    {
      X.AddScript(
        string.Format(@"var idx = {0}.findColumnIndex('n_gqty');
              if(idx != -1) {{
                {0}.setEditable(idx, false);
              }}
              idx = {0}.findColumnIndex('v_ket_type_dc');
              if(idx != -1) {{
                {0}.setHidden(idx, true);
              }}
              idx = {0}.findColumnIndex('l_void');
              if(idx != -1) {{
                {0}.setHidden(idx, false);
              }}", gridDetail.ColumnModel.ClientID));

      frmHeaders.Height = new Unit(150);
      chkConfirm.Hidden = true;
    }
  }

  private void ClearEntrys(bool isClear)
  {
    hfSJNo.Clear();

    winDetail.Title = "Pesanan Gudang";

    //cbFromHdr.Clear();
    //cbFromHdr.Disabled = false;

    lbGudangFrom.Text = hfGudangDesc.Text;

    chkConfirm.Disabled = false;    
    btnSave.Hidden = false;

    cbToHdr.Clear();
    cbToHdr.Disabled = false;
    Ext.Net.Store cbToHdrstr = cbToHdr.GetStore();
    if (cbToHdrstr != null)
    {
        cbToHdrstr.RemoveAll();
    }

    if (isClear)
    {
        cbTipeSJ.Clear();
        cbTipeSJ.Disabled = false;
        Ext.Net.Store cbTipeSJstr = cbTipeSJ.GetStore();
        if (cbTipeSJstr != null)
        {
            cbTipeSJstr.RemoveAll();
        }
    }

    cbPrincipalHdr.Clear();
    cbPrincipalHdr.Items.Clear();
    cbPrincipalHdr.Disabled = false;

    txKeterangan.Clear();
    txKeterangan.Disabled = false;

    cbItemDtl.Clear();
    cbItemDtl.Disabled = false;

    cbSpgDtl.Clear();
    cbSpgDtl.Disabled = false;

    cbBatDtl.Clear();
    cbBatDtl.Disabled = false;

    txQtyDtl.Clear();
    txQtyDtl.Disabled = false;

    chkConfirm.Hidden = true;
    chkConfirm.Checked = false;

    cbKategori.Clear();
    cbKategori.Disabled = false;
    Ext.Net.Store cbKategoristr = cbKategori.GetStore();
    if (cbKategoristr != null)
    {
        cbKategoristr.RemoveAll();
    }

    cbLantai.Clear();
    cbLantai.Disabled = false;
    Ext.Net.Store cbLantaistr = cbLantai.GetStore();
    if (cbLantaistr != null)
    {
        cbLantaistr.RemoveAll();
    }

    btnAutoGen.Hidden =
    btnAutoGen.Disabled = true;

    btnPrint.Hidden = true;
    
    btnAutoGen.Hidden = false;
    btnAutoGen.Disabled = false;
    frmHeaders.Height = new Unit(175);

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }

    gridDetail.ColumnModel.SetEditable(10, true);
    gridDetail.ColumnModel.SetEditor(10, new Ext.Net.TextField() { AllowBlank = true });
  }

  private void PopulateDetail(string pName, string pID)
  {
    ClearEntrys(true);

    Dictionary<string, object> dicSJ = null;
    Dictionary<string, string> dicSJInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string[][] paramX = new string[][]{
            new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
        };

    string tmp = null;

    bool isConfirm = false;
    bool isConfirmed = false,
      isRn = false;

    bool.TryParse(hfConfMode.Text, out isConfirm);

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0003", paramX);

    try
    {
      dicSJ = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicSJ.ContainsKey("records") && (dicSJ.ContainsKey("totalRows") && (((long)dicSJ["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicSJ["records"]);

        dicSJInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("Pesanan Gudang - {0}", pID);
        lbGudangFrom.Text = dicSJInfo.GetValueParser<string>("v_from", string.Empty);

        hfGudang.Text = dicSJInfo.GetValueParser<string>("c_gdg", string.Empty);

        Functional.SetComboData(cbToHdr, "c_gdg2", dicSJInfo.GetValueParser<string>("v_to", string.Empty), dicSJInfo.GetValueParser<string>("c_gdg2", string.Empty));
        cbToHdr.Disabled = true;

        Functional.SetComboData(cbPrincipalHdr, "c_nosup", dicSJInfo.GetValueParser<string>("v_nama", string.Empty), dicSJInfo.GetValueParser<string>("c_nosup", string.Empty));
        cbPrincipalHdr.Disabled = true;

        if (string.IsNullOrEmpty(dicSJInfo.GetValueParser<string>("c_nosup", string.Empty)))
        {
            gridDetail.ColumnModel.SetEditable(10, false);
        }
        else
        {
            gridDetail.ColumnModel.SetEditable(10, true);
            gridDetail.ColumnModel.SetEditor(10, new Ext.Net.TextField() { AllowBlank = true });
        }

        Functional.SetComboData(cbLantai, "c_type_lat", dicSJInfo.GetValueParser<string>("v_ket_lat", string.Empty), dicSJInfo.GetValueParser<string>("c_type_lat", string.Empty));
        cbLantai.Disabled = true;

        Functional.SetComboData(cbKategori, "c_type_cat", dicSJInfo.GetValueParser<string>("v_ket_itemcat", string.Empty), dicSJInfo.GetValueParser<string>("c_type_cat", string.Empty));
        cbKategori.Disabled = true;

        Functional.SetComboData(cbTipeSJ, "c_type_sj", dicSJInfo.GetValueParser<string>("v_ket_sj", string.Empty), dicSJInfo.GetValueParser<string>("c_type_sj", string.Empty));
        cbTipeSJ.Disabled = true;

        tmp = (dicSJInfo.ContainsKey("l_confirm") ? dicSJInfo["l_confirm"] : string.Empty);
        if (!bool.TryParse(tmp, out isConfirmed))
        {
          isConfirmed = (tmp.Equals("1", StringComparison.OrdinalIgnoreCase) ? true : false);
        }

        tmp = (dicSJInfo.ContainsKey("l_status") ? dicSJInfo["l_status"] : string.Empty);
        if (!bool.TryParse(tmp, out isRn))
        {
          isRn = (tmp.Equals("1", StringComparison.OrdinalIgnoreCase) ? true : false);
        }

        txKeterangan.Text = ((dicSJInfo.ContainsKey("v_ket")) ? dicSJInfo["v_ket"] : string.Empty);
        chkConfirm.Checked = isConfirmed;

        if (isConfirmed)
        {
          txKeterangan.Disabled = true;
        }

        //cbItemDtl.Disabled = true;
        //cbSpgDtl.Disabled = true;
        //cbBatDtl.Disabled = true;

        //X.AddScript(string.Format("clearRelatedComboRecursive(true, {0});", cbItemDtl.ClientID));

        //btnPrint.Hidden = (isConfirm && (!isConfirmed) ? false : true);
        if (isConfirm)
        {
          chkConfirm.Hidden = false;

          //btnPrint.Hidden = (isConfirmed ? false : true);
        }
        else
        {
          //btnPrint.Hidden = (isConfirmed ? true : false);
        }

        btnPrint.Hidden = false;

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
      SetEditoryConfirm(isRn, isConfirm);

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

    btnAutoGen.Hidden = true;
    btnAutoGen.Disabled = true;
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
    item = null, ket = null, spgno = null, batch = null,
    type_desc = null,
    accket = null;

    decimal nQty = 0;
    bool isNew = false,
      isVoid = false,
      isModify = false,
      isED = false,
      isAccModify = false;
    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", pag.Nip);

    pair.DicAttributeValues.Add("From", "6");
    pair.DicAttributeValues.Add("To", cbToHdr.Text);
    pair.DicAttributeValues.Add("Keterangan", txKeterangan.Text);
    pair.DicAttributeValues.Add("Supplier", cbPrincipalHdr.Text);
    pair.DicAttributeValues.Add("TypeCategory", cbKategori.Text);
    pair.DicAttributeValues.Add("TypeLantai", cbLantai.Text);
    pair.DicAttributeValues.Add("TypeSJ", cbTipeSJ.Text);
    pair.DicAttributeValues.Add("Confirm", chkConfirm.Checked.ToString().ToLower());

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      isNew = dicData.GetValueParser<bool>("l_new");
      isVoid = dicData.GetValueParser<bool>("l_void");
      isModify = dicData.GetValueParser<bool>("l_modified");
      isAccModify = dicData.GetValueParser<bool>("l_accmodify");
      isED = dicData.GetValueParser<bool>("l_expired");

      //Verifikasi keterangan ED
      if (isED && string.IsNullOrEmpty(dicData.GetValueParser<string>("v_ket_ed")) && !isVoid)
      {
          responseResult.Message = "KeteranganED";
          responseResult.Response = ScmsSoaLibraryInterface.Components.PostDataParser.ResponseStatus.Failed;
          return responseResult;
      }
      
      if (isNew && (!isVoid) && (!isModify))
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        spgno = dicData.GetValueParser<string>("c_spgno");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        nQty = dicData.GetValueParser<decimal>("n_gqty", 0);
        accket = dicData.GetValueParser<string>("v_ket_ed");

        if ((!string.IsNullOrEmpty(spgno)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)) &&
          (nQty > 0))
        {
          dicAttr.Add("spgno", spgno);
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
      else if ((!isNew) && (!isVoid) && isModify)
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        nQty = dicData.GetValueParser<decimal>("n_gqty", 0);

        if (nQty == 0)
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

        spgno = dicData.GetValueParser<string>("c_spgno");
        item = dicData.GetValueParser<string>("c_iteno");
        ket = dicData.GetValueParser<string>("v_ket");
        batch = dicData.GetValueParser<string>("c_batch");
        type_desc = dicData.GetValueParser<string>("c_type_dc");
        isED = dicData.GetValueParser<bool>("l_expired");
        accket = dicData.GetValueParser<string>("v_ket_ed");

          //&&
          //((!string.IsNullOrEmpty(type_desc)))

        if ((!string.IsNullOrEmpty(spgno)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch))&&
          ((!string.IsNullOrEmpty(type_desc)) || (!string.IsNullOrEmpty(accket))))
        {
          dicAttr.Add("spgno", spgno);
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("Qty", nQty.ToString());
          dicAttr.Add("sGQty", nQty.ToString());
          dicAttr.Add("tipe_dc", type_desc);
          dicAttr.Add("isED", isED.ToString().ToLower());
          dicAttr.Add("accket", accket);

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
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

        spgno = dicData.GetValueParser<string>("c_spgno");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        ket = dicData.GetValueParser<string>("v_ket");

        nQty = dicData.GetValueParser<decimal>("n_gqty", 0);

        if ((!string.IsNullOrEmpty(spgno)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)))
        {
          dicAttr.Add("spgno", spgno);
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("Qty", nQty.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
            DicAttributeValues = dicAttr
          });
        }
      }
      else if ((!isNew) && (!isVoid) && (!isModify) && (isAccModify))
      {
          dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
      
          dicAttr.Add("New", "false");
          dicAttr.Add("Delete", "false");
          dicAttr.Add("Modified", "false");
          dicAttr.Add("isAccModify", "true");

          spgno = dicData.GetValueParser<string>("c_spgno");
          item = dicData.GetValueParser<string>("c_iteno");
          ket = dicData.GetValueParser<string>("v_ket");
          batch = dicData.GetValueParser<string>("c_batch");
          type_desc = dicData.GetValueParser<string>("c_type_dc");
          isED = dicData.GetValueParser<bool>("l_expired");
          accket = dicData.GetValueParser<string>("v_ket_ed");

          if ((!string.IsNullOrEmpty(spgno)) &&
            (!string.IsNullOrEmpty(item)) &&
            (!string.IsNullOrEmpty(batch)) &&
            ((!string.IsNullOrEmpty(type_desc)) || (!string.IsNullOrEmpty(accket))))
          {
              dicAttr.Add("spgno", spgno);
              dicAttr.Add("Item", item);
              dicAttr.Add("Batch", batch);
              dicAttr.Add("Qty", nQty.ToString());
              dicAttr.Add("sGQty", nQty.ToString());
              dicAttr.Add("tipe_dc", type_desc);
              dicAttr.Add("isED", isED.ToString().ToLower());
              dicAttr.Add("accket", accket);

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
        varData = parser.ParserData("TransferGudang", "ModifyConfirm", dic);
      }
      else
      {
        varData = parser.ParserData("TransferGudang", (isAdd ? "Add" : "Modify"), dic);
      }
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_TransferGudangCtrl SaveParser : {0} ", ex.Message);
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

  private void PopulateAutoGenerate(string gdg, string gdgToID, string prinsipal, string kategory, string lantai, string tipeSJ)
  {
    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    List<Dictionary<string, string>> lstResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "gdg", gdg, "System.Char"},
        new string[] { "gdgTo", gdgToID,  "System.Char"},
        new string[] { "supl", prinsipal,  "System.String"},
        new string[] { "itemCat", kategory,  "System.String"},
        new string[] { "itemLat", lantai,  "System.String"},
        new string[] { "tipeSJ", tipeSJ,  "System.String"},
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string res = soa.GlobalQueryService(0, -1, false, string.Empty, string.Empty, "3203", paramX);
    System.Text.StringBuilder sb = new System.Text.StringBuilder();

    decimal nSisa = 0;

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }

    sb.AppendFormat("{0}.removeAll(); {0}.commitChanges(); ", gridDetail.GetStore().ClientID);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        lstResultInfo = JSON.Deserialize<List<Dictionary<string, string>>>(jarr.First.ToString());

        for (int nLoop = 0; nLoop < lstResultInfo.Count; nLoop++)
        {
          dicResultInfo = lstResultInfo[nLoop];

          nSisa = dicResultInfo.GetValueParser<decimal>("n_sisa", 0);

          DateTime date = DateTime.MinValue;
          date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_expired"));

          if (nSisa > 0)
          {
            sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
            'c_iteno': '{1}',
            'c_batch': '{2}',
            'v_itnam': '{3}',
            'n_booked': {4},
            'n_gqty': {4},
            'c_spgno': '{5}',
            'd_expired' : '{6}',
            'l_expired' : {7},
            'l_new': true
          }})); ", gridDetail.GetStore().ClientID,
                    dicResultInfo.GetValueParser<string>("c_iteno", string.Empty),
                    dicResultInfo.GetValueParser<string>("c_batch", string.Empty),
                    dicResultInfo.GetValueParser<string>("v_itemdesc", string.Empty),
                    nSisa,
                    dicResultInfo.GetValueParser<string>("c_spgno", string.Empty),
                    date,
                    dicResultInfo.GetValueParser<bool>("l_expired", false ).ToString().ToLower()
                    );
          }

          dicResultInfo.Clear();
        }

        X.AddScript(sb.ToString());
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_penjualan_PackingList:PopulateAutoGenerate - ", ex.Message));
    }

    sb.Remove(0, sb.Length);

    if (lstResultInfo != null)
    {
      lstResultInfo.Clear();
    }
    if (dicResult != null)
    {
      dicResult.Clear();
    }
    if (jarr != null)
    {
      jarr.Clear();
    }
  }

  #endregion

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  public void Initialize(string gudang, string gudangDesc, string storeIDGridMain, bool isConfirm)
  {
    //hfGudang.Text = gudang;
    hfStoreID.Text = storeIDGridMain;
    hfGudangDesc.Text = "Gudang Ulujami";

    if (isConfirm)
    {
      hfConfMode.Text = bool.TrueString.ToLower();
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void btnSimpan_OnClick(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string NumberID = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jSonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);
    //string gdgId = (e.ExtraParams["GudangID"] ?? string.Empty);
    string gdgId = hfGudang.Text;
    //string gdgDesc = (e.ExtraParams["GudangDesc"] ?? string.Empty);
    string gdgId2 = (e.ExtraParams["GudangID2"] ?? string.Empty);
    string gdgDesc2 = (e.ExtraParams["GudangDesc2"] ?? string.Empty);
    string ket = (e.ExtraParams["Keterangan"] ?? string.Empty);
    string supl = (e.ExtraParams["Supplier"] ?? string.Empty);
    string TypeCategory = (e.ExtraParams["TypeCategory"] ?? string.Empty);

    bool isConfirm = false,
    isConfirmed = false;

    if (bool.TryParse(hfConfMode.Text, out isConfirm))
    {
      isConfirmed = (chkConfirm.Checked ? true : false);
    }

    Dictionary<string, string>[] gridDataDO = JSON.Deserialize<Dictionary<string, string>[]>(jSonGridValues);

    bool isAdd = (string.IsNullOrEmpty(NumberID) ? true : false);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, gdgId, isConfirmed, NumberID, gridDataDO);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;

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
                'c_gdg': '{1}',
                'c_sjno': '{2}',
                'd_sjdate': {3},
                'v_to': '{4}',
                'v_ket': '{5}',
                'l_print': false,
                'l_status': false,
                'l_confirm': false,
                'c_expno': '',
                'v_nama': '{6}'
              }}));{0}.commitChanges();", storeId,
                                        gdgId2,
                                        NumberID,
                                        dateJs, gdgDesc2, ket, supl);

              X.AddScript(scrpt);
            }
          }
        }

        //this.ClearEntrys();
        if ((!string.IsNullOrEmpty(NumberID)))
        {
          this.PopulateDetail("c_sjno", NumberID);
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

  public void CommandPopulate(bool isAdd, string pID)
  {
    bool isConfirm = false;

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    bool.TryParse(hfConfMode.Text, out isConfirm);

    if (isAdd && isConfirm)
    {
      Functional.ShowMsgError("(UC) Perintah tidak dikenal.");
    }
    else if (isAdd)
    {
      this.ClearEntrys(true);

      SetEditoryConfirm(false, false);

      frmHeaders.Height = new Unit(150);

      btnAutoGen.Hidden = false;
      btnAutoGen.Disabled = false;
      chkConfirm.Hidden = true;
      winDetail.Hidden = false;
      winDetail.ShowModal();

      hfGudang.Text = pag.ActiveGudang;
    }
    else
    {
      chkConfirm.Hidden = (!isConfirm);
      Ext.Net.GridPanel grid = gridDetail;
      if (!isConfirm)
      {
        //gridDetail.ColumnModel.Columns[7].Hidden = false;
        X.AddScript(
        string.Format(@"{0} = true;", gridDetail.ColumnModel.Columns[7].Hidden)
        );
      }
      PopulateDetail("c_sjno", pID);
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string gdgId = hfGudang.Text;
    string numberID = (e.ExtraParams["NumberID"] ?? string.Empty);

    if (string.IsNullOrEmpty(numberID))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");

      return;
    }

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();

    rptParse.ReportingID = "10110-a";

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
      ParameterValue = "01"
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
      ParameterValue = "01",
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

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void AutoGenBtn_Click(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    if (!pag.IsAllowAdd)
    {
      //Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      e.ErrorMessage = "Maaf, anda tidak mempunyai hak akses untuk menambah data.";

      e.Success = false;

      return;
    }

    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string gdgToID = (e.ExtraParams["gdgToID"] ?? string.Empty);
    string suplId = (e.ExtraParams["PrinsipalID"] ?? string.Empty);
    string katId = (e.ExtraParams["KategoriID"] ?? string.Empty);
    string latId = (e.ExtraParams["LantaiID"] ?? string.Empty);
    string tipeSJ = (e.ExtraParams["TipeSJ"] ?? string.Empty);

    if (!string.IsNullOrEmpty(numberId))
    {
      e.ErrorMessage = "Generator hanya dapat di jalankan untuk pembuatan data baru.";
      e.Success = false;
      return;
    }
    else if (string.IsNullOrEmpty(gdgToID))
    {
      e.ErrorMessage = "Nama Gudang Tujuan tidak terbaca.";
      e.Success = false;
      return;
    }
    else if (string.IsNullOrEmpty(suplId))
    {
      e.ErrorMessage = "Nama pemasok tidak terbaca.";
      e.Success = false;
      return;
    }
    else if (string.IsNullOrEmpty(tipeSJ))
    {
        e.ErrorMessage = "Tipe SJ tidak boleh kosong.";
        e.Success = false;
        return;
    }
    //else if (string.IsNullOrEmpty(katId))
    //{
    //  e.ErrorMessage = "Kategori tidak boleh kosong.";
    //  e.Success = false;
    //  return;
    //}

    string gdgAsal = pag.ActiveGudang;

    PopulateAutoGenerate(gdgAsal, gdgToID, suplId, katId, latId, tipeSJ);
  }

  protected void ReloadBtn_Click(object sender, DirectEventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    this.ClearEntrys(true);
  }

  protected void SelectedTipeSJ_Change(object sender, DirectEventArgs e)
  {
      if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
      {
          Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
          return;
      }

      this.ClearEntrys(false);
      if (cbTipeSJ.SelectedItem.Value == "01")
      {
          txQtyDtl.ReadOnly = false;
      }
      else
      {
          txQtyDtl.ReadOnly = false;
      }
  }
}
