using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_penjualan_PackingListCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Packing List";

    hfPlNo.Clear();
    hfTypDtl.Clear();
    hfExpire.Clear();
    hfSPDate.Clear();
    hfETDDate.Clear(); //Indra 20181115FM ETD First
    //Indra 20181226FM Penambahan filter ETD
    txPeriode1.Text = DateTime.Now.AddMonths(-2).AddDays(-DateTime.Now.Day + 1).ToString("dd-MM-yyyy");
    txPeriode2.Text = DateTime.Now.ToString("dd-MM-yyyy");
    txPeriode1.Disabled = txPeriode2.Disabled = false;

    cbViaHdr.Clear();
    cbViaHdr.Disabled = false;
    Ext.Net.Store cbViaHdrstr = cbViaHdr.GetStore();
    if (cbViaHdrstr != null)
    {
        cbViaHdrstr.RemoveAll();
    }

    cbCustomerHdr.Clear();
    cbCustomerHdr.Disabled = false;
    Ext.Net.Store cbCustomerHdrstr = cbCustomerHdr.GetStore();
    if (cbCustomerHdrstr != null)
    {
        cbCustomerHdrstr.RemoveAll();
    }

    cbPrincipalHdr.Clear();
    cbPrincipalHdr.Disabled = false;
    Ext.Net.Store cbPrincipalHdrstr = cbPrincipalHdr.GetStore();
    if (cbPrincipalHdrstr != null)
    {
        cbPrincipalHdrstr.RemoveAll();
    }
    cbDivPrinsipal.AllowBlank = true;
    cbDivPrinsipal.Clear();
    cbDivPrinsipal.Disabled = false;
    Ext.Net.Store cbDivPrinsipalstr = cbDivPrinsipal.GetStore();
    if (cbDivPrinsipalstr != null)
    {
        cbDivPrinsipalstr.RemoveAll();
    }
    cbDivPrinsipal.Hidden = true;

    cbKategori.Clear();
    cbKategori.Disabled = false;
    Ext.Net.Store cbKategoristr = cbKategori.GetStore();
    if (cbKategoristr != null)
    {
        cbKategoristr.RemoveAll();
    }

    txKeterangan.Clear();
    txKeterangan.Disabled = false;

    cbTipeHdr.Clear();
    cbTipeHdr.Disabled = false;
    Ext.Net.Store cbTipeHdrstr = cbTipeHdr.GetStore();
    if (cbTipeHdrstr != null)
    {
        cbTipeHdrstr.RemoveAll();
    }

    //cbLantai.Clear();
    //cbLantai.Disabled = false;
    //Ext.Net.Store cbLantaistr = cbLantai.GetStore();
    //if (cbLantaistr != null)
    //{
    //    cbLantaistr.RemoveAll();
    //}

    //chkConfirm.Clear();
    //chkConfirm.Disabled = true;
    lbChkConfirm.Text = string.Empty;
    lbChkConfirm.Icon = Icon.Delete;
    
    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));
    //frmpnlDetailEntry.Disabled = false;

    cbItemDtl.Disabled = false;
    cbSpcDtl.Disabled = false;
    cbBatDtl.Disabled = false;
    txQtyDtl.Disabled = false;
    btnDelete.Hidden = false;
    btnAdd.Disabled = false;
    btnClear.Disabled = false;

    btnAutoGen.Hidden = 
      btnAutoGen.Disabled = true;
    frmHeaders.Height = new Unit(180);

    btnSave.Hidden = false;

    btnPrint.Hidden = true;

    cbBaspbNo.Hidden = true;
    
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

        winDetail.Title = string.Concat("Packing List - ", pID);

        tmp = (dicResultInfo.ContainsKey("l_do") ? dicResultInfo["l_do"] : string.Empty);
        if (!bool.TryParse(tmp, out isDo))
        {
          isDo = (tmp.Equals("1", StringComparison.OrdinalIgnoreCase) ? true : false);
        }

        tmp = dicResultInfo.GetValueParser<string>("l_confirm", string.Empty); //tmp = (dicResultInfo.ContainsKey("l_confirm") ? dicResultInfo["l_confirm"] : string.Empty);
        if (!bool.TryParse(tmp, out isConfirmed))
        {
          isConfirmed = (tmp.Equals("1", StringComparison.OrdinalIgnoreCase) ? true : false);
        }

        //cbViaHdr.SetValueAndFireSelect((dicResultInfo.ContainsKey("c_via") ? dicResultInfo["c_via"] : string.Empty));
        //cbViaHdr.Disabled = true;
        Functional.SetComboData(cbViaHdr, "c_via", dicResultInfo.GetValueParser<string>("v_ket_via", string.Empty), dicResultInfo.GetValueParser<string>("c_via", string.Empty));
        cbViaHdr.Disabled = (isConfirmed || isDo); ;

        //cbCustomerHdr.ToBuilder().AddItem(
        //  (dicResultInfo.ContainsKey("V_CUNAM") ? dicResultInfo["V_CUNAM"] : string.Empty),
        //  (dicResultInfo.ContainsKey("c_cusno") ? dicResultInfo["c_cusno"] : string.Empty)
        //);
        //if (cbCustomerHdr.GetStore() != null)
        //{
        //  cbCustomerHdr.GetStore().CommitChanges();
        //}
        //cbCustomerHdr.SetValueAndFireSelect((dicResultInfo.ContainsKey("c_cusno") ? dicResultInfo["c_cusno"] : string.Empty));
        Functional.SetComboData(cbCustomerHdr, "c_cusno", dicResultInfo.GetValueParser<string>("v_cunam", string.Empty), dicResultInfo.GetValueParser<string>("c_cusno", string.Empty));
        cbCustomerHdr.Disabled = true;

        //cbPrincipalHdr.ToBuilder().AddItem(
        //  (dicResultInfo.ContainsKey("V_NAMA") ? dicResultInfo["V_NAMA"] : string.Empty),
        //  (dicResultInfo.ContainsKey("c_nosup") ? dicResultInfo["c_nosup"] : string.Empty)
        //);
        //if (cbPrincipalHdr.GetStore() != null)
        //{
        //  cbPrincipalHdr.GetStore().CommitChanges();
        //}
        //cbPrincipalHdr.SetValueAndFireSelect((dicResultInfo.ContainsKey("c_nosup") ? dicResultInfo["c_nosup"] : string.Empty));
        Functional.SetComboData(cbPrincipalHdr, "c_nosup", dicResultInfo.GetValueParser<string>("v_nama", string.Empty), dicResultInfo.GetValueParser<string>("c_nosup", string.Empty)); 
        cbPrincipalHdr.Disabled = true;

        Functional.SetComboData(cbKategori, "c_type_cat", dicResultInfo.GetValueParser<string>("v_ket_itemcat", string.Empty), dicResultInfo.GetValueParser<string>("c_type_cat", string.Empty));
        cbKategori.Disabled = true;

        //Functional.SetComboData(cbLantai, "c_type_lat", dicResultInfo.GetValueParser<string>("v_ket_itemlat", string.Empty), dicResultInfo.GetValueParser<string>("c_type_lat", string.Empty));
        //cbLantai.Disabled = true;

        tmp = (dicResultInfo.ContainsKey("l_print") ? dicResultInfo["l_print"] : string.Empty);
        if (!bool.TryParse(tmp, out isPrint))
        {
          isPrint = (tmp.Equals("1", StringComparison.OrdinalIgnoreCase) ? true : false);
        }

        txKeterangan.Text = ((dicResultInfo.ContainsKey("v_ket") ? dicResultInfo["v_ket"] : string.Empty));
        txKeterangan.Disabled = (isConfirmed || isDo);

        //cbTipeHdr.SetValueAndFireSelect((dicResultInfo.ContainsKey("c_type") ? dicResultInfo["c_type"] : string.Empty));
        Functional.SetComboData(cbTipeHdr, "c_type", dicResultInfo.GetValueParser<string>("v_ket_type", string.Empty), dicResultInfo.GetValueParser<string>("c_type", string.Empty));
        cbTipeHdr.Disabled = (isConfirmed || isDo);

        if (dicResultInfo.GetValueParser<string>("c_type", string.Empty) == "03")
        {
            cbBaspbNo.Hidden = false;
            Functional.SetComboData(cbBaspbNo, "c_baspbno", dicResultInfo.GetValueParser<string>("c_baspbno", string.Empty), dicResultInfo.GetValueParser<string>("c_baspbno", string.Empty));
            cbBaspbNo.Disabled = (isConfirmed || isDo);

        }

        if (dicResultInfo.GetValueParser<string>("c_nosup", string.Empty) == "00019")
        {
            cbDivPrinsipal.Hidden = false;
            Functional.SetComboData(cbDivPrinsipal, "c_kddivpri", dicResultInfo.GetValueParser<string>("v_nmdivpri", string.Empty), dicResultInfo.GetValueParser<string>("c_kddivpri", string.Empty));
            cbDivPrinsipal.Disabled = true;
        }
        else
        {
            cbDivPrinsipal.Text = " "; 
        }

        //chkConfirm.Checked = isConfirmed;
        //chkConfirm.Disabled = true;

        lbChkConfirm.Text = string.Empty;
        lbChkConfirm.Icon = (isConfirmed ? Icon.Accept : Icon.Delete);

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

        X.AddScript(string.Format("clearRelatedComboRecursive(true, {0});", cbItemDtl.ClientID));

        jarr.Clear();

        btnSave.Hidden = (isConfirmed || isDo);

        //btnPrint.Hidden = ((!isPrint) || pag.IsSpecialGroup ? false : true);
        btnPrint.Hidden = false;
        btnDelete.Hidden = true;
        
        //Indra 20181226FM Penambahan filter ETD
        txPeriode1.Text = DateTime.Now.AddMonths(-1).AddDays(-DateTime.Now.Day + 1).ToString("dd-MM-yyyy");
        txPeriode2.Text = DateTime.Now.AddDays(28).ToString("dd-MM-yyyy");
        txPeriode1.Disabled = txPeriode2.Disabled = true;
      
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_penjualan_PackingList:PopulateDetail Header - ", ex.Message));
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

      hfPlNo.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_penjualan_PackingList:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    //btnAutoGen.Hidden = true;
    //btnAutoGen.Disabled = true;
    //frmHeaders.Height = new Unit(160);

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
      sp = null,
      item = null,
      batch = null,
      ket = null,
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
    pair.DicAttributeValues.Add("Customer", cbCustomerHdr.Text);
    pair.DicAttributeValues.Add("Suplier", cbPrincipalHdr.Text);
    pair.DicAttributeValues.Add("Type", cbTipeHdr.Text);
    pair.DicAttributeValues.Add("Keterangan", txKeterangan.Text.Trim());
    pair.DicAttributeValues.Add("Gudang", hfGudang.Text.Trim());
    pair.DicAttributeValues.Add("TypeCategory", cbKategori.Text);
    pair.DicAttributeValues.Add("Confirm", "false");
    //pair.DicAttributeValues.Add("Lantai", cbLantai.Text);
    pair.DicAttributeValues.Add("BaspbNo", cbBaspbNo.Text);
    pair.DicAttributeValues.Add("DivPriID", cbDivPrinsipal.Text);

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
        if(!pag.IsAllowAdd)
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

        sp = dicData.GetValueParser<string>("c_sp");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        ket = dicData.GetValueParser<string>("v_ket");

        nQty = dicData.GetValueParser<decimal>("n_QtyRequest", 0);

        if ((!string.IsNullOrEmpty(sp)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)))
        {
          dicAttr.Add("Sp", sp);
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

        sp = dicData.GetValueParser<string>("c_sp");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        ket = dicData.GetValueParser<string>("v_ket");

        nQty = dicData.GetValueParser<decimal>("n_QtyRequest", 0);
        accket = dicData.GetValueParser<string>("v_ket_ed");

        if ((!string.IsNullOrEmpty(sp)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)))
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

          nQty = dicData.GetValueParser<decimal>("n_QtyRequest", 0);
          accket = dicData.GetValueParser<string>("v_ket_ed");

          if ((!string.IsNullOrEmpty(sp)) &&
            (!string.IsNullOrEmpty(item)) &&
            (!string.IsNullOrEmpty(batch)))
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
                  Value = (string.IsNullOrEmpty(ket) ? "Modify at Confirm" : ket),
                  DicAttributeValues = dicAttr
              });
          }
      }

      dicData.Clear();
    }

    try
    {
        varData = parser.ParserData("PackingListAutoGenerator", (isAdd ? "Add" : "Modify"), dic, true);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_PackingListCtrl SaveParser : {0} ", ex.Message);
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

  //Indra 20181226FM Penambahan filter ETD
  //private void PopulateAutoGenerate(string gdg, string cabang, string prinsipal, string kategory, string lantai, string divpri)
  private void PopulateAutoGenerate(string gdg, string cabang, string prinsipal, string kategory, string lantai, string divpri, string txPeriode1, string txPeriode2)
  {
    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    List<Dictionary<string, string>> lstResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "gdg", gdg, "System.Char"},
        new string[] { "cusno", cabang,  "System.String"},
        new string[] { "supl", prinsipal,  "System.String"},
        new string[] { "cat", kategory,  "System.String"},
        new string[] { "itemLat", lantai,  "System.String"},
        new string[] { "isbox", "false",  "System.String"},
        new string[] { "divPri", divpri,  "System.String"},
        //Indra 20181226FM Penambahan filter ETD
        new string[] { "txPeriode1", txPeriode1, "System.String"},
        new string[] { "txPeriode2", txPeriode2, "System.String"},
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string res = soa.GlobalQueryService(0, -1, false, string.Empty, string.Empty, "3204", paramX);
    System.Text.StringBuilder sb = new System.Text.StringBuilder();

    decimal nSisa = 0;

    //sb.AppendFormat("{0}.removeAll(); {0}.commitChanges(); ", gridDetail.GetStore().ClientID);
    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
        store.RemoveAll();
    }

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
          DateTime dateSP = DateTime.MinValue;
          DateTime dateETD = DateTime.MinValue; //Indra 20181115FM ETD First

          date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_expired"));
          dateSP = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_spdate"));
          dateETD = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_etdsp")); //Indra 20181115FM ETD First

          if (nSisa > 0)
          {
            sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
            'c_iteno': '{1}',
            'v_itemdesc': '{2}',
            'c_sp': '{3}',
            'c_spc': '{4}',
            'c_batch': '{5}',
            'n_booked': {6},
            'n_QtyRequest': {6},
            'v_undes': '{7}',
            'd_expired' : '{8}',
            'l_expired' : {9},
            'd_spdate' : '{10}',
            'd_etdsp' : '{11}', 
            'l_new': true
          }})); ", gridDetail.GetStore().ClientID,
                    dicResultInfo.GetValueParser<string>("c_iteno", string.Empty),
                    dicResultInfo.GetValueParser<string>("v_itemdesc", string.Empty),
                    dicResultInfo.GetValueParser<string>("c_spno", string.Empty),
                    dicResultInfo.GetValueParser<string>("c_sp", string.Empty),
                    dicResultInfo.GetValueParser<string>("c_batch", string.Empty),
                    nSisa,
                    dicResultInfo.GetValueParser<string>("v_undes", string.Empty),
                    date,
                    dicResultInfo.GetValueParser<bool>("l_expired", false ).ToString().ToLower(),
                    dateSP,
                    dateETD); //Indra 20181115FM ETD First
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

  private void SetEditor(bool isAdd)
  {
    string scrp = null;
    string gridId = gridDetail.ColumnModel.ClientID;

    if (isAdd)
    {
      scrp = string.Format(@"idx = {0}.findColumnIndex('n_QtyRequest');
                          if(idx != -1) {{
                            {0}.setEditable(idx, true);
                          }}", gridId);
    }
    else
    {
      scrp = string.Format(@"idx = {0}.findColumnIndex('n_QtyRequest');
                          if(idx != -1) {{
                            {0}.setEditable(idx, false);
                          }}", gridId);
    }

    X.AddScript(scrp);
  }

  #endregion

  public void Initialize(string gudang, string gudangDesc, string storeIDGridMain)
  {
    hfGudang.Text = gudang;
    hfStoreID.Text = storeIDGridMain;
    hfGudangDesc.Text = gudangDesc;

    //Indra 20181226FM Penambahan filter ETD
    txPeriode1.Text = DateTime.Now.AddMonths(-1).AddDays(-DateTime.Now.Day + 1).ToString("dd-MM-yyyy");
    txPeriode2.Text = DateTime.Now.ToString("dd-MM-yyyy");
    txPeriode1.Disabled = txPeriode2.Disabled = false;
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
      //frmHeaders.Height = new Unit(160);
      //chkConfirm.Hidden = true;

      ClearEntrys();

      //cbViaHdr.SetValueAndFireSelect("02");
      Functional.SetComboData(cbViaHdr, "c_type", "Darat/Laut", "02");

      //cbTipeHdr.SetValueAndFireSelect("01");
      Functional.SetComboData(cbTipeHdr, "c_type", "Reguler", "01");

      btnAutoGen.Hidden = false;
      btnAutoGen.Disabled = false;
      frmHeaders.Height = new Unit(200);

      winDetail.Hidden = false;
      winDetail.ShowModal();

      SetEditor(true);
    }
    else
    {
      SetEditor(false);

      PopulateDetail("c_plno", pID);

      frmHeaders.Height = new Unit(200);
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);

    //bool isConfirm = false,
    //  isConfirmed = false;

    //if (bool.TryParse(hfConfMode.Text, out isConfirm))
    //{
    //  isConfirmed = (chkConfirm.Checked ? true : false);
    //}

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

        string cust = (cbCustomerHdr.SelectedItem != null ? cbCustomerHdr.SelectedItem.Text : string.Empty);
        string supl = (cbPrincipalHdr.SelectedItem != null ? cbPrincipalHdr.SelectedItem.Text : string.Empty);

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

            numberId = respon.Values.GetValueParser<string>("PL", string.Empty);

            if (!string.IsNullOrEmpty(storeId))
            {
              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_plno': '{1}',
                'd_pldate': {2},
                'v_gdgdesc': '{3}',
                'v_cunam': '{4}',
                'v_nama': '{5}',
                'l_print': false,
                'l_confirm': false,
                'L_DO': false
              }}));{0}.commitChanges();", storeId, numberId,
                       dateJs, hfGudangDesc.Text, cust, supl);

              X.AddScript(scrpt);
            }
          }
        }
//        else if (isConfirmed)
//        {
//          scrpt = string.Format(@"var rec = {0}.getById('{1}');
//                                  if(!Ext.isEmpty(rec)) {{
//                                    rec.set('l_confirm', true);
//                                  }}", storeId, numberId);
//        }

        //this.ClearEntrys();
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
          rptResult.OutputFile, "Packing List", rptResult.Extension);

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

    btnAutoGen.Hidden = false;
    btnAutoGen.Disabled = false;
    frmHeaders.Height = new Unit(200);
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void AutoGenBtn_Click(object sender, DirectEventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowAdd)
    {
      //Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      e.ErrorMessage = "Maaf, anda tidak mempunyai hak akses untuk menambah data.";

      e.Success=false;

      return;
    }

    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string custId = (e.ExtraParams["CustomerID"] ?? string.Empty);
    string suplId = (e.ExtraParams["PrinsipalID"] ?? string.Empty);
    string katId = (e.ExtraParams["KategoriID"] ?? string.Empty);
    string latId = (e.ExtraParams["LantaiID"] ?? string.Empty);
    string divPriId = (e.ExtraParams["DivPriID"] ?? string.Empty);
    //Indra 20181226FM Penambahan filter ETD
    string txPeriode1 = (e.ExtraParams["txPeriode1"].Substring(0, 10) ?? string.Empty);
    string txPeriode2 = (e.ExtraParams["txPeriode2"].Substring(0, 10) ?? string.Empty);
      
    
    if (!string.IsNullOrEmpty(numberId))
    {
      e.ErrorMessage = "Generator hanya dapat di jalankan untuk pembuatan data baru.";
      e.Success = false;
      return;
    }
    else if (string.IsNullOrEmpty(custId))
    {
      e.ErrorMessage = "Nama cabang tidak terbaca.";
      e.Success = false;
      return;
    }
    //else if (string.IsNullOrEmpty(katId))
    //{
    //  e.ErrorMessage = "Kategori tidak boleh kosong.";
    //  e.Success = false;
    //  return;
    //}

    //Indra 20181226FM Penambahan filter ETD
    //PopulateAutoGenerate(hfGudang.Text.Trim(), custId, suplId, katId, latId, divPriId);
    PopulateAutoGenerate(hfGudang.Text.Trim(), custId, suplId, katId, latId, divPriId, txPeriode1, txPeriode2);
  }
}