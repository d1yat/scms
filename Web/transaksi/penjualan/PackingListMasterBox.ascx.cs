using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using System.Data;
using System.Linq;
using Newtonsoft.Json;

public partial class transaksi_penjualan_PackingListMasterBox : System.Web.UI.UserControl
{
  private void ClearEntrys()
  {
    winDetail.Title = "Packing List";

    hfPlNoMaster.Clear();
    //hfTypDtl.Clear();
    hfSPDate.Clear();
    hfExpire.Clear();

    cbViaHdr.Clear();
    cbViaHdr.ClearValue();
    cbViaHdr.Disabled = false;

    cbCustomerHdr.Clear();
    cbCustomerHdr.ClearValue();
    cbCustomerHdr.Disabled = false;

    cbPrincipalHdr.Clear();
    cbPrincipalHdr.ClearValue();
    cbPrincipalHdr.Disabled = false;

    cbDivPrinsipal.Clear();
    cbDivPrinsipal.ClearValue();
    cbDivPrinsipal.Disabled = false;
    cbDivPrinsipal.Hidden = true;

    cbKategori.Clear();
    cbKategori.ClearValue();
    cbKategori.Disabled = false;

    txKeterangan.Clear();
    txKeterangan.Disabled = false;

    cbTipeHdr.Clear();
    cbTipeHdr.ClearValue();
    cbTipeHdr.Disabled = false;

    //chkConfirm.Clear();
    //chkConfirm.Disabled = true;
    lbChkConfirm.Text = string.Empty;
    lbChkConfirm.Icon = Icon.Delete;

    //X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));
    //frmpnlDetailEntry.Disabled = false;

    //cbItemDtl.Disabled = false;
    //cbSpcDtl.Disabled = false;
    //cbBatDtl.Disabled = false;
    //txQtyDtl.Disabled = false;
    //btnAdd.Disabled = false;
    //btnClear.Disabled = false;

    btnAutoGen.Hidden =
      btnAutoGen.Disabled = true;
    frmHeaders.Height = new Unit(180);

    btnSave.Hidden = false;

    btnPrint.Hidden = true;

    Ext.Net.Store store = gridDetailMaster.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }

    Ext.Net.Store store1 = gridDetailReceh.GetStore();
    if (store1 != null)
    {
      store1.RemoveAll();
    }

    Ext.Net.Store store2 = gridDetailMaster2.GetStore();
    if (store != null)
    {
      store2.RemoveAll();
    }

    Ext.Net.Store store3 = gridDetailReceh2.GetStore();
    if (store1 != null)
    {
      store3.RemoveAll();
    }

    Ext.Net.Store store4 = strMasterBox;
    if (store1 != null)
    {
      store4.RemoveAll();
    }

    Ext.Net.Store store5 = strMasterReceh;
    if (store1 != null)
    {
      store5.RemoveAll();
    }
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string plNumber, Dictionary<string, string>[] dicsMater, Dictionary<string, string>[] dicsReceh, Dictionary<string, string>[] dicsOriMater, Dictionary<string, string>[] dicsOriReceh)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null,
      dicDataTmp = null;

    Dictionary<string, string> dicMaster = null,
      dicReceh = null;
    Dictionary<int, Dictionary<string, string>> dicMasterToSave = new Dictionary<int, Dictionary<string, string>>(),
      dicRecehToSave = new Dictionary<int, Dictionary<string, string>>() ;

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
      isBox = false,
      isED = false;
    string varData = null;

    int iVal = 0;

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
    pair.DicAttributeValues.Add("Lantai", cbLantai.Text);
    pair.DicAttributeValues.Add("DivPriID", cbDivPrinsipal.Text);

    for (int nLoop = 0, nLen = dicsOriMater.Length; nLoop < nLen; nLoop++)
    {
      dicDataTmp = dicsOriMater[nLoop];

      sp = dicDataTmp.GetValueParser<string>("c_sp");
      item = dicDataTmp.GetValueParser<string>("c_iteno");
      batch = dicDataTmp.GetValueParser<string>("c_batch");

      List<string[]> keysToAdd = new List<string[]>();

      for (int nLoops = 0; nLoops < dicsMater.Length; nLoops++)
      {
        dicData = dicsMater[nLoops];

        if ((item.Equals(dicData.GetValueParser<string>("c_iteno"))) && ((batch.Equals(dicData.GetValueParser<string>("c_batch")))))
        {

          dicMaster = new Dictionary<string, string>();

          keysToAdd.Add(dicDataTmp.Keys.ToArray());
          foreach (string[] keys in keysToAdd)
          {
            foreach (string key in keys)
            {
              dicMaster.Add(key, dicDataTmp.GetValueParser<string>(key));
            }
          }
          //dicMaster.Add("c_sp", dicDataTmp.GetValueParser<string>("c_sp"));
          dicMaster.Add("l_expired", dicData.GetValueParser<bool>("l_expired").ToString());
          dicMaster.Add("v_ket_ed", dicData.GetValueParser<string>("v_ket_ed"));
          dicMasterToSave.Add(iVal, dicMaster);
          iVal++;
        }
      }
      
    }

    dicDataTmp = new Dictionary<string, string>();
    dicData = new Dictionary<string, string>();

    iVal = 0;

    for (int nLoop = 0, nLen = dicsOriReceh.Length; nLoop < nLen; nLoop++)
    {
      dicDataTmp = dicsOriReceh[nLoop];

      sp = dicDataTmp.GetValueParser<string>("c_sp");
      item = dicDataTmp.GetValueParser<string>("c_iteno");
      batch = dicDataTmp.GetValueParser<string>("c_batch");

      List<string[]> keysToAdd = new List<string[]>();

      for (int nLoops = 0; nLoops < dicsReceh.Length; nLoops++)
      {
        dicData = dicsReceh[nLoops];

        if ((item.Equals(dicData.GetValueParser<string>("c_iteno"))) && ((batch.Equals(dicData.GetValueParser<string>("c_batch")))))
        {

          dicReceh = new Dictionary<string, string>();

          keysToAdd.Add(dicDataTmp.Keys.ToArray());
          foreach (string[] keys in keysToAdd)
          {
            foreach (string key in keys)
            {
              dicReceh.Add(key, dicDataTmp.GetValueParser<string>(key));
            }
          }
          //dicReceh.Add("c_sp", dicDataTmp.GetValueParser<string>("c_sp"));
          dicReceh.Add("l_expired", dicData.GetValueParser<bool>("l_expired").ToString());
          dicReceh.Add("v_ket_ed", dicData.GetValueParser<string>("v_ket_ed"));
          dicRecehToSave.Add(iVal, dicReceh);
          iVal++;
        }
      }
    }

    dicData = new Dictionary<string, string>();

    for (int nLoop = 0, nLen = dicMasterToSave.Count; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dicMasterToSave[nLoop];

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

        sp = dicData.GetValueParser<string>("c_sp");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        isBox = dicData.GetValueParser<bool>("l_box", false);

        nQty = dicData.GetValueParser<decimal>("n_booked", 0);
        isED = dicData.GetValueParser<bool>("l_expired");
        accket = dicData.GetValueParser<string>("v_ket_ed");

        //Verifikasi keterangan ED
        if (isED && string.IsNullOrEmpty(accket) && !isVoid)
        {
            responseResult.Message = "KeteranganED";
            responseResult.Response = ScmsSoaLibraryInterface.Components.PostDataParser.ResponseStatus.Failed;
            return responseResult;
        }

        if ((!string.IsNullOrEmpty(sp)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)) &&
          (nQty > 0))
        {
          dicAttr.Add("Sp", sp);
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("isBox", isBox.ToString().ToLower());
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

      dicData.Clear();
    }


    for (int nLoop = 0, nLen = dicRecehToSave.Count; nLoop < nLen; nLoop++)
    {
      tmp = ((int.Parse(tmp) == 0 ? 1 : (int.Parse(tmp) + 1))).ToString();

      dicData = dicRecehToSave[nLoop];

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

        sp = dicData.GetValueParser<string>("c_sp");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        isBox = dicData.GetValueParser<bool>("l_box", false);

        nQty = dicData.GetValueParser<decimal>("n_booked", 0);
        isED = dicData.GetValueParser<bool>("l_expired");
        accket = dicData.GetValueParser<string>("v_ket_ed");

        //Verifikasi keterangan ED
        if (isED && string.IsNullOrEmpty(accket) && !isVoid)
        {
            responseResult.Message = "KeteranganED";
            responseResult.Response = ScmsSoaLibraryInterface.Components.PostDataParser.ResponseStatus.Failed;
            return responseResult;
        }

        if ((!string.IsNullOrEmpty(sp)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)) &&
          (nQty > 0))
        {
          dicAttr.Add("Sp", sp);
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("isBox", isBox.ToString().ToLower());
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

      dicData.Clear();
    }

    try
    {
      varData = parser.ParserData("PackingListMasterBox", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_PackingListMasterBoxCtrl SaveParser : {0} ", ex.Message);
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

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  private void PopulateAutoGenerate(string gdg, string cabang, string prinsipal, string kategory, string lantai, string divpri)
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
        new string[] { "itemCat", kategory,  "System.String"},
        new string[] { "itemLat", lantai,  "System.String"},
        new string[] { "isbox", "true",  "System.String"},
        new string[] { "divPri", divpri,  "System.String"},
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string res = soa.GlobalQueryService(0, 10, false, string.Empty, string.Empty, "3202", paramX);
    System.Text.StringBuilder sb = new System.Text.StringBuilder();

    decimal nSisa = 0;
    bool isMaster = false;
    DateTime date = DateTime.MinValue;
    DateTime dateSP = DateTime.MinValue;

    //sb.AppendFormat("{0}.removeAll(); {0}.commitChanges(); ", gridDetail.GetStore().ClientID);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        lstResultInfo = JSON.Deserialize<List<Dictionary<string, string>>>(jarr.First.ToString());

        DataTable dt = new DataTable();

        foreach (string column in lstResultInfo[0].Keys)
        {
          dt.Columns.Add(column);
        }

        foreach (Dictionary<string, string> dictionary in lstResultInfo)
        {
          DataRow dataRow = dt.NewRow();

          foreach (string column in dictionary.Keys)
          {
            dataRow[column] = dictionary[column];
          }

          dt.Rows.Add(dataRow);
        }

        List<string[]> lstdata = new List<string[]>();

        var grouped = (from table in dt.AsEnumerable()
                       group table by new
                       {
                         c_iteno = table["c_iteno"],
                         v_itemdesc = table["v_itemdesc"],
                         c_batch = table["c_batch"],
                         v_undes = table["v_undes"],
                         n_box = table["n_box"],
                         d_expired = table["d_expired"],
                         l_expired = table["l_expired"]
                       } into groupby
                      select new
                      {
                        c_iteno = groupby.Key.c_iteno,
                        v_itemdesc = groupby.Key.v_itemdesc,
                        c_batch = groupby.Key.c_batch,
                        v_undes = groupby.Key.v_undes,
                        n_box = groupby.Key.n_box,
                        d_expired = groupby.Key.d_expired,
                        l_expired = groupby.Key.l_expired,
                        SumSisa = groupby.Sum((r) => decimal.Parse(r["n_sisa"].ToString()))
                      }).ToList();

        dt = new DataTable();

        dt.Columns.Add("isMaster");
        dt.Columns.Add("c_iteno");
        dt.Columns.Add("c_batch");
        dt.Columns.Add("SumSisa");

        int iCountM = 0,
          iCountR = 0,
          iMaxItem = 10;

        foreach (var i in grouped)
        {
          DataRow dataRow = dt.NewRow();

          dt.Rows.Add(dataRow);

          decimal SumSisa = i.SumSisa,
            boxes = decimal.Parse(i.n_box.ToString());

          date = Functional.JsonDateToDate(i.d_expired.ToString());

          if ((SumSisa / boxes) >= 1)
          {
            if (iCountM < iMaxItem)
            {
              decimal flor = Math.Floor((SumSisa / boxes));

              sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                'c_iteno': '{1}',
                'v_itemdesc': '{2}',
                'c_batch': '{3}',
                'n_booked': {4},
                'n_QtyRequest': {4},
                'v_undes': '{5}',
                'n_box': '{6}',
                'l_box': true,
                'd_expired' : '{7}',
                'l_expired' : {8},
                'l_new': true
              }})); ", gridDetailMaster.GetStore().ClientID,
                        i.c_iteno,
                        i.v_itemdesc,
                        i.c_batch,
                        (flor * boxes),
                        i.v_undes,
                        i.n_box,
                        date,
                        i.l_expired.ToString().ToLower());

              iCountM++;
            }

            if ((SumSisa % boxes) > 0)
            {
              if (iCountR < iMaxItem)
              {
                decimal receh = (SumSisa % boxes);

                sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                'c_iteno': '{1}',
                'v_itemdesc': '{2}',
                'c_batch': '{3}',
                'n_booked': {4},
                'n_QtyRequest': {4},
                'v_undes': '{5}',
                'n_box': '{6}',
                'l_box': false,
                'd_expired' : '{7}',
                'l_expired' : {8},
                'l_new': true
              }})); ", gridDetailReceh.GetStore().ClientID,
                        i.c_iteno,
                        i.v_itemdesc,
                        i.c_batch,
                        receh,
                        i.v_undes,
                        i.n_box,
                        date,
                        i.l_expired.ToString().ToLower());
                iCountR++;
              }
            }
          }
          else
          {
            if (iCountR < iMaxItem)
            {

              sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                'c_iteno': '{1}',
                'v_itemdesc': '{2}',
                'c_batch': '{3}',
                'n_booked': {4},
                'n_QtyRequest': {4},
                'v_undes': '{5}',
                'n_box': '{6}',
                'l_box': false,
                'd_expired' : '{7}',
                'l_expired' : {8},
                'l_new': true
              }})); ", gridDetailReceh.GetStore().ClientID,
                        i.c_iteno,
                        i.v_itemdesc,
                        i.c_batch,
                        SumSisa,
                        i.v_undes,
                        i.n_box,
                        date,
                        i.l_expired.ToString().ToLower());

              iCountR++;
            }
          }
        }

        X.AddScript(sb.ToString());

        sb = new System.Text.StringBuilder();

        for (int nLoop = 0; nLoop < lstResultInfo.Count; nLoop++)
        {
          dicResultInfo = lstResultInfo[nLoop];

          nSisa = dicResultInfo.GetValueParser<decimal>("n_sisa", 0);
          isMaster = dicResultInfo.GetValueParser<bool>("isMaster", false);

          dateSP = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_spdate"));

          if (nSisa > 0)
          {
            if (isMaster)
            {
              sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                            'c_iteno': '{1}',
                            'v_itemdesc': '{2}',
                            'c_sp': '{3}',
                            'c_spc': '{4}',
                            'd_spdate': '{5}',
                            'c_batch': '{6}',
                            'n_booked': {7},
                            'n_QtyRequest': {7},
                            'v_undes': '{8}',
                            'n_box': '{9}',
                            'l_box': '{10}',
                            'l_new': true
                          }})); ", gridDetailMaster2.GetStore().ClientID,
                        dicResultInfo.GetValueParser<string>("c_iteno", string.Empty),
                        dicResultInfo.GetValueParser<string>("v_itemdesc", string.Empty),
                        dicResultInfo.GetValueParser<string>("c_spno", string.Empty),
                        dicResultInfo.GetValueParser<string>("c_sp", string.Empty),
                        dateSP,
                        dicResultInfo.GetValueParser<string>("c_batch", string.Empty),
                        nSisa,
                        dicResultInfo.GetValueParser<string>("v_undes", string.Empty),
                        dicResultInfo.GetValueParser<decimal>("n_box", 0),
                        dicResultInfo.GetValueParser<bool>("isMaster", false));

              sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                            'c_iteno': '{1}',
                            'v_itemdesc': '{2}',
                            'c_sp': '{3}',
                            'c_spc': '{4}',
                            'd_spdate': '{5}',
                            'c_batch': '{6}',
                            'n_booked': {7},
                            'n_QtyRequest': {7},
                            'v_undes': '{8}',
                            'n_box': '{9}',
                            'l_box': '{10}',
                            'l_new': true
                          }})); ", strMasterBox.ClientID,
                        dicResultInfo.GetValueParser<string>("c_iteno", string.Empty),
                        dicResultInfo.GetValueParser<string>("v_itemdesc", string.Empty),
                        dicResultInfo.GetValueParser<string>("c_spno", string.Empty),
                        dicResultInfo.GetValueParser<string>("c_sp", string.Empty),
                        dateSP,
                        dicResultInfo.GetValueParser<string>("c_batch", string.Empty),
                        nSisa,
                        dicResultInfo.GetValueParser<string>("v_undes", string.Empty),
                        dicResultInfo.GetValueParser<decimal>("n_box", 0),
                        dicResultInfo.GetValueParser<bool>("isMaster", false));


            }
            else
            {
              sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                            'c_iteno': '{1}',
                            'v_itemdesc': '{2}',
                            'c_sp': '{3}',
                            'c_spc': '{4}',
                            'd_spdate': '{5}',
                            'c_batch': '{6}',
                            'n_booked': {7},
                            'n_QtyRequest': {7},
                            'v_undes': '{8}',
                            'n_box': '{9}',
                            'l_box': '{10}',
                            'l_new': true
                          }})); ", gridDetailReceh2.GetStore().ClientID,
                        dicResultInfo.GetValueParser<string>("c_iteno", string.Empty),
                        dicResultInfo.GetValueParser<string>("v_itemdesc", string.Empty),
                        dicResultInfo.GetValueParser<string>("c_spno", string.Empty),
                        dicResultInfo.GetValueParser<string>("c_sp", string.Empty),
                        dateSP,
                        dicResultInfo.GetValueParser<string>("c_batch", string.Empty),
                        nSisa,
                        dicResultInfo.GetValueParser<string>("v_undes", string.Empty),
                        dicResultInfo.GetValueParser<decimal>("n_box", 0),
                        dicResultInfo.GetValueParser<bool>("isMaster", false));

              sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                            'c_iteno': '{1}',
                            'v_itemdesc': '{2}',
                            'c_sp': '{3}',
                            'c_spc': '{4}',
                            'd_spdate': '{5}',
                            'c_batch': '{6}',
                            'n_booked': {7},
                            'n_QtyRequest': {7},
                            'v_undes': '{8}',
                            'n_box': '{9}',
                            'l_box': '{10}',
                            'l_new': true
                          }})); ", strMasterReceh.ClientID,
                          dicResultInfo.GetValueParser<string>("c_iteno", string.Empty),
                          dicResultInfo.GetValueParser<string>("v_itemdesc", string.Empty),
                          dicResultInfo.GetValueParser<string>("c_spno", string.Empty),
                          dicResultInfo.GetValueParser<string>("c_sp", string.Empty),
                          dateSP,
                          dicResultInfo.GetValueParser<string>("c_batch", string.Empty),
                          nSisa,
                          dicResultInfo.GetValueParser<string>("v_undes", string.Empty),
                          dicResultInfo.GetValueParser<decimal>("n_box", 0),
                          dicResultInfo.GetValueParser<bool>("isMaster", false));
            }
          }

          dicResultInfo.Clear();
        }

        X.AddScript(sb.ToString());
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_penjualan_PackingListMasterBox:PopulateAutoGenerate - ", ex.Message));
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

  public void Initialize(string gudang, string gudangDesc, string storeIDGridMain)
  {
    hfGudang.Text = gudang;
    hfStoreID.Text = storeIDGridMain;
    hfGudangDesc.Text = gudangDesc;
  }

  private void SetEditor(bool isAdd)
  {
    string scrp = null;
//    string gridId = gridDetail.ColumnModel.ClientID;

//    if (isAdd)
//    {
//      scrp = string.Format(@"idx = {0}.findColumnIndex('n_QtyRequest');
//                          if(idx != -1) {{
//                            {0}.setEditable(idx, true);
//                          }}", gridId);
//    }
//    else
//    {
//      scrp = string.Format(@"idx = {0}.findColumnIndex('n_QtyRequest');
//                          if(idx != -1) {{
//                            {0}.setEditable(idx, false);
//                          }}", gridId);
//    }

    X.AddScript(scrp);
  }

  public void CommandPopulate(bool isAdd, string pID)
  {
    //bool isConfirm = false;

    //bool.TryParse(hfConfMode.Text, out isConfirm);

    if (isAdd)
    {
      ClearEntrys();

      Functional.SetComboData(cbViaHdr, "c_type", "Darat/Laut", "02");

      Functional.SetComboData(cbTipeHdr, "c_type", "Master Box", "05");

      btnAutoGen.Hidden = false;
      btnAutoGen.Disabled = false;
      frmHeaders.Height = new Unit(200);

      winDetail.Hidden = false;
      winDetail.ShowModal();

      SetEditor(true);
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
          rptResult.OutputFile, "Packing List Master Box", rptResult.Extension);

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
    if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowAdd)
    {
      //Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      e.ErrorMessage = "Maaf, anda tidak mempunyai hak akses untuk menambah data.";

      e.Success = false;

      return;
    }

    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string custId = (e.ExtraParams["CustomerID"] ?? string.Empty);
    string suplId = (e.ExtraParams["PrinsipalID"] ?? string.Empty);
    string katId = (e.ExtraParams["KategoriID"] ?? string.Empty);
    string latId = (e.ExtraParams["LantaiID"] ?? string.Empty);
    string divPriId = (e.ExtraParams["DivPriID"] ?? string.Empty);

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
    else if (string.IsNullOrEmpty(suplId))
    {
      e.ErrorMessage = "Nama pemasok tidak terbaca.";
      e.Success = false;
      return;
    }
    //else if (string.IsNullOrEmpty(katId))
    //{
    //  e.ErrorMessage = "Kategori tidak boleh kosong.";
    //  e.Success = false;
    //  return;
    //}

    PopulateAutoGenerate(hfGudang.Text.Trim(), custId, suplId, katId, latId, divPriId);
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
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValuesReceh = (e.ExtraParams["gridValuesReceh"] ?? string.Empty);
    string strMasterBox = (e.ExtraParams["strMasterBox"] ?? string.Empty);
    string jsonGridValuesMaster = (e.ExtraParams["gridValuesMaster"] ?? string.Empty);
    string strMasterReceh = (e.ExtraParams["strMasterReceh"] ?? string.Empty);
    string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);

    string NumberID2 = null;

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

    Dictionary<string, string>[] gridDataMaster = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValuesMaster);
    Dictionary<string, string>[] gridDataReceh = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValuesReceh);
    Dictionary<string, string>[] gridMasterBox = JSON.Deserialize<Dictionary<string, string>[]>(strMasterBox);
    Dictionary<string, string>[] gridMasterReceh = JSON.Deserialize<Dictionary<string, string>[]>(strMasterReceh);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataMaster, gridDataReceh, gridMasterBox, gridMasterReceh);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null,
          scrpt2 = null,
          NIP = null;

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

            numberId = respon.Values.GetValueParser<string>("PL1", string.Empty);
            NumberID2 = respon.Values.GetValueParser<string>("PL2", string.Empty);
            NIP = respon.Values.GetValueParser<string>("NIP", string.Empty);

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
                'L_DO': false,
                'c_entry' : '{6}'
              }}));{0}.commitChanges();", storeId, numberId,
                       dateJs, hfGudangDesc.Text, cust, supl, NIP);

              if (!string.IsNullOrEmpty(NumberID2))
              {
                scrpt2 = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_plno': '{1}',
                'd_pldate': {2},
                'v_gdgdesc': '{3}',
                'v_cunam': '{4}',
                'v_nama': '{5}',
                'l_print': false,
                'l_confirm': false,
                'L_DO': false,
                'c_entry' : '{6}'
              }}));{0}.commitChanges();", storeId, NumberID2,
                       dateJs, hfGudangDesc.Text, cust, supl, NIP);

                X.AddScript(scrpt2);
              }

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

        this.ClearEntrys();


        if (!string.IsNullOrEmpty(numberId))
        {
          //this.PopulateDetail("c_plno", numberId);
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
}
