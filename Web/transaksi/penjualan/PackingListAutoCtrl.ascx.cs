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

public partial class transaksi_penjualan_PackingListAutoCtrl : System.Web.UI.UserControl
{
  #region Private 
  
  private void ClearEntrys()
  {
    winDetail.Title = "Packing List Auto";

    hfTypDtl.Clear();

    cbViaHdr.Clear();
    cbViaHdr.Disabled = false;

    cbNoDoKhususHdr.Clear();
    cbNoDoKhususHdr.Disabled = false;

    cbCustomerHdr.Clear();
    cbCustomerHdr.Disabled = false;

    cbSuplierHdr.Clear();
    cbSuplierHdr.Disabled = false;

    txKeterangan.Clear();
    txKeterangan.Disabled = false;

    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));
    //frmpnlDetailEntry.Disabled = false;

    cbItemDtl.Disabled = false;
    cbSpcDtl.Disabled = false;
    cbBatDtl.Disabled = false;
    txQtyDtl.Disabled = false;
    btnAdd.Disabled = false;
    btnClear.Disabled = false;

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private PostDataParser.StructureResponse SaveParser(Dictionary<string, string>[] dics)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    Scms.Web.Core.PageHandler page = this.Page as Scms.Web.Core.PageHandler;

    string tmp = null,
      sp = null,
      item = null,
      batch = null,
      rn = null;
    decimal nQty = 0, nQtyRn = 0, nQtyAdj = 0;
    bool isNew = false,
      isVoid = false,
      isModify = false;
    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", page.Nip);
    pair.DicAttributeValues.Add("Via", cbViaHdr.Text);
    pair.DicAttributeValues.Add("Customer", cbCustomerHdr.Text);
    pair.DicAttributeValues.Add("Gudang", hfGudangAuto.Text);
    pair.DicAttributeValues.Add("NoRN", cbNoDoKhususHdr.Text);
    pair.DicAttributeValues.Add("Suplier", cbSuplierHdr.Text);
    pair.DicAttributeValues.Add("Tipe", cbTipe.Text);
    pair.DicAttributeValues.Add("Keterangan", txKeterangan.Text);

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

        dicAttr.Add("New", "true");
        dicAttr.Add("Delete", "false");
        dicAttr.Add("Modified", "false");

        rn = dicData.GetValueParser<string>("c_rnno");
        sp = dicData.GetValueParser<string>("c_spno");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        nQty = dicData.GetValueParser<decimal>("n_qtysp", 0);
        nQtyRn = dicData.GetValueParser<decimal>("n_qtyrn", 0);
        nQtyAdj = dicData.GetValueParser<decimal>("n_qtysp_adj", 0);

        if ((!string.IsNullOrEmpty(sp)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)) &&
          ((nQty > 0) || (nQtyAdj > 0)))
        {
          dicAttr.Add("Rn", rn);
          dicAttr.Add("Sp", sp);
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("qtySP", nQty.ToString());
          dicAttr.Add("qtyRN", nQtyRn.ToString());
          dicAttr.Add("qtySPAdj", nQtyAdj.ToString());

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
      varData = parser.ParserData("PackingListAuto", "Add", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_PackingListCtrlAuto SaveParser : {0} ", ex.Message);
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

  #endregion
  
  public void Initialize(string gudang, string gudangDesc, string storeIDGridMain)
  {
    hfGudangAuto.Text = gudang;
    hfGudangDescAuto.Text = gudangDesc;
    hfStoreIDAuto.Text = storeIDGridMain;
  }
  
  public void CommandPopulate()
  {
    //if (isAdd)
    //{
    //  this.ClearEntrys();

    //  winDetail.Hidden = false;
    //  winDetail.ShowModal();
    //}

    this.ClearEntrys();

    winDetail.Hidden = false;
    winDetail.ShowModal();
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      ;
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void ProcessPL_Click(object sender, DirectEventArgs e)
  {
    string Gudang = (e.ExtraParams["Gudang"] ?? string.Empty);
    string Customer = (e.ExtraParams["Customer"] ?? string.Empty);
    string Via = (e.ExtraParams["Via"] ?? string.Empty);
    string noRN = (e.ExtraParams["noRN"] ?? string.Empty);
    //string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "Gudang", Gudang, "System.String"},
        new string[] { "Customer", Customer, "System.String"},
        new string[] { "Via", Via, "System.String"},
        new string[] { "noRN", noRN, "System.String"},
    };

    string result = soa.GlobalQueryService(0, -1, true, string.Empty, string.Empty, "0128", paramX);

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
            }

            if (sb.ToString().EndsWith(",", StringComparison.OrdinalIgnoreCase))
            {
              sb.Remove(sb.Length - 1, 1);
            }

            sb.Append("}));");

            dicResultInfo.Clear();
          }

          sb.AppendLine(string.Format("{0}.commitChanges()", storeId));

          Ext.Net.X.AddScript(sb.ToString());

          sb.Remove(0, sb.Length);
        }

        #endregion

        jarr.Clear();
      }
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowEdit)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }
    else if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menyimpan data.");
      return;
    }

    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    //string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);

    //transaksi_penjualan_PackingListAutoCtrl
    string plID = null;
    string storeId = hfStoreIDAuto.Text;

    Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    PostDataParser.StructureResponse respon = SaveParser(gridDataPL);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;

        string cust = (cbCustomerHdr.SelectedItem != null ? cbCustomerHdr.SelectedItem.Text : string.Empty);
        string supl = (cbSuplierHdr.SelectedItem != null ? cbSuplierHdr.SelectedItem.Text : string.Empty);

        string dateJs = null;
        DateTime date = DateTime.Today;
        
        if (respon.Values != null)
        {
          if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
          {
            dateJs = Functional.DateToJson(date);
          }

          plID = respon.Values.GetValueParser<string>("PL", string.Empty);

          if (!string.IsNullOrEmpty(storeId))
          {
            scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_plno': '{1}',
                'd_pldate': {2},
                'v_gdgdesc': '{3}',
                'v_cunam': '{4}',
                'v_nama': '{5}',
                'l_print': true,
                'l_confirm': true,
                'l_do': true
              }}));{0}.commitChanges();", storeId,
                                      plID,
                     dateJs, hfGudangDescAuto.Text, cust,
                     supl);

            X.AddScript(scrpt);
          }
        }

        this.ClearEntrys();

        //Functional.ShowMsgInformation("Data berhasil tersimpan.");

        scrpt = string.Format(@"if(Ext.isFunction(Ext.net.DirectMethods.ShowSavedData)) {{
                            {0}.hide();
                            Ext.net.DirectMethods.ShowSavedData('{1}', 'Data berhasil tersimpan.');
                          }}
else {{
  ShowInformasi('Data berhasil tersimpan.');
}}", winDetail.ClientID, plID);

        X.AddScript(scrpt);
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
}
