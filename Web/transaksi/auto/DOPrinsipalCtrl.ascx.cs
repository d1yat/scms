using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_auto_DOPrinsipalCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Monitoring DO Principal";

    hfSuplNo.Clear();
    hfDOID.Clear();

    cbSuplierHdr.Clear();
    cbSuplierHdr.Disabled = false;

    txDOHdr.Clear();
    txDOHdr.Disabled = false;

    txDateDoHdr.Clear();
    txDateDoHdr.Disabled = false;

    txFJHdr.Clear();
    txFJHdr.Disabled = false;

    txDateFJHdr.Clear();
    txDateFJHdr.Disabled = false;

    cbCustomerHdr.Clear();
    cbCustomerHdr.Disabled = false;

    cbViaHdr.Clear();
    cbViaHdr.Disabled = false;

    txDateDoHdr.Clear();
    txDateDoHdr.Disabled = false;

    txTaxHdr.Clear();
    txTaxHdr.Disabled = false;

    btnSave.Hidden = false;

    frmpnlDetailEntry.Hidden = false;
    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private void PopulateDetail(string suplID, string pID)
  {
    ClearEntrys();

    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "c_nosup = @0", suplID, "System.String"},
        new string[] { "c_dono = @0", pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string tmp = null;

    DateTime date = DateTime.MinValue;
    hfSuplNo.Text = suplID;
    hfDOID.Text = pID;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0300", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("Monitoring DO Principal - {0} ({1})", pID,
          dicResultInfo.GetValueParser<string>("v_sup_name", string.Empty).Trim());

        Functional.SetComboData(cbSuplierHdr, "c_nosup", dicResultInfo.GetValueParser<string>("v_sup_name", string.Empty), dicResultInfo.GetValueParser<string>("c_nosup", string.Empty));
        cbSuplierHdr.Disabled = true;

        txDOHdr.Text = dicResultInfo.GetValueParser<string>("c_dono", string.Empty);
        txDOHdr.Disabled = true;
        tmp = dicResultInfo.GetValueParser<string>("d_dodate", string.Empty);
        date = Functional.JsonDateToDate(tmp);
        txDateDoHdr.Text = date.ToString("dd-MM-yyyy");
        txDateDoHdr.Disabled = true;

        txFJHdr.Text = dicResultInfo.GetValueParser<string>("d_fjno", string.Empty);
        txFJHdr.Disabled = true;
        tmp = dicResultInfo.GetValueParser<string>("d_fjdate", string.Empty);
        date = Functional.JsonDateToDate(tmp);
        txDateFJHdr.Text = date.ToString("dd-MM-yyyy");
        txDateFJHdr.Disabled = true;

        Functional.SetComboData(cbCustomerHdr, "c_cusno", dicResultInfo.GetValueParser<string>("v_cunam", string.Empty), dicResultInfo.GetValueParser<string>("c_cusno", string.Empty));
        cbCustomerHdr.Disabled = true;

        Functional.SetComboData(cbViaHdr, "c_via", dicResultInfo.GetValueParser<string>("v_via_kirim", string.Empty), dicResultInfo.GetValueParser<string>("c_via", string.Empty));
        cbViaHdr.Disabled = true;

        txTaxHdr.Text = dicResultInfo.GetValueParser<string>("c_taxno", string.Empty);
        txTaxHdr.Disabled = true;

        //X.AddScript(string.Format("clearRelatedComboRecursive(true, {0});", cbItemDtl.ClientID));

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_auto_DOPrinsipalCtrl:PopulateDetail Header - ", ex.Message));
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
            store.BaseParams.Add(new Ext.Net.Parameter("parameters",
              string.Format("[['nosup', '{0}', 'System.String'], ['nodo', '{1}', 'System.String']]", suplID, pID), ParameterMode.Raw));
          }
          else
          {
            store.BaseParams["parameters"] = string.Format("[['nosup', '{0}', 'System.String'], ['nodo', '{1}', 'System.String']]", suplID, pID);
          }
        }
      }

      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_auto_DOPrinsipalCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    frmpnlDetailEntry.Hidden = true;

    btnSave.Hidden = true;

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(string doNumber, string supldId, Dictionary<string, string>[] dics)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = doNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string tmp = null,
      item = null,
      itemSupl = null,
      tipeTrx = null,
      po = null,
      batch = null,
      batchExpired = null,
      ket = null;
    bool isVoid = false,
      isNew = false,
      isClaim = false,
      isPending = false;
    string varData = null;

    decimal nQty = 0,
      nDisc = 0;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    
    Functional.DateParser(txDateDoHdr.RawText, "d-M-yyyy", out date);
    pair.DicAttributeValues.Add("TanggalDO", date.ToString("yyyyMMddHHmmssfff"));

    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Prinsipal", supldId);;

    pair.DicAttributeValues.Add("Faktur", txFJHdr.Text);
    Functional.DateParser(txDateFJHdr.RawText, "d-M-yyyy", out date);
    pair.DicAttributeValues.Add("TanggalFJ", date.ToString("yyyyMMddHHmmssfff"));

    pair.DicAttributeValues.Add("Customer", (cbCustomerHdr.SelectedItem == null ? "Z0" : cbCustomerHdr.SelectedItem.Value));
    pair.DicAttributeValues.Add("Via", cbViaHdr.SelectedItem.Value);
    pair.DicAttributeValues.Add("TaxNo", txTaxHdr.Text);

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      isNew = dicData.GetValueParser<bool>("l_new");
      isVoid = dicData.GetValueParser<bool>("l_void");

      if (isNew && (!isVoid))
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", "false");

        item = dicData.GetValueParser<string>("c_iteno");
        itemSupl = dicData.GetValueParser<string>("c_itenopri");
        tipeTrx = dicData.GetValueParser<string>("c_type");
        batch = dicData.GetValueParser<string>("c_batch");
        batchExpired = dicData.GetValueParser<string>("d_expired");
        po = dicData.GetValueParser<string>("c_pono");
        isClaim = dicData.GetValueParser<bool>("l_claim");
        isPending = dicData.GetValueParser<bool>("l_pending");
        nQty = dicData.GetValueParser<decimal>("n_qty", 0);
        nDisc = dicData.GetValueParser<decimal>("n_disc", 0);

        if ((!string.IsNullOrEmpty(po)) &&
          (!string.IsNullOrEmpty(tipeTrx)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)) &&
          (!string.IsNullOrEmpty(batchExpired)) &&
          (nQty > 0))
        {
          Functional.DateParser(batchExpired, "yyyy-MM-ddTHH:mm:ss", out date);

          dicAttr.Add("Item", item);
          dicAttr.Add("Type", tipeTrx);
          dicAttr.Add("Qty", nQty.ToString());
          dicAttr.Add("Disc", nDisc.ToString());
          dicAttr.Add("ItemSuplier", itemSupl);
          dicAttr.Add("PO", po);
          dicAttr.Add("Batch", batch.Trim());
          dicAttr.Add("Expired", date.ToString("yyyyMMdd"));
          dicAttr.Add("IsClaim", isClaim.ToString().ToLower());
          dicAttr.Add("IsPending", isPending.ToString().ToLower());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr
          });
        }
      }
      else if ((!isNew) && isVoid)
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", "false");

        po = dicData.GetValueParser<string>("c_refno");
        tipeTrx = dicData.GetValueParser<string>("c_type");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        ket = dicData.GetValueParser<string>("v_ket");

        if ((!string.IsNullOrEmpty(po)) &&
          (!string.IsNullOrEmpty(tipeTrx)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Type", tipeTrx);
          dicAttr.Add("Batch", batch.Trim());
          dicAttr.Add("PO", po);

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr,
            Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
          });
        }
      }

      dicData.Clear();
    }

    try
    {
      varData = parser.ParserData("DOPrinsipal", "Add", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penerimaan_RNPembelianCtrl SaveParser : {0} ", ex.Message);
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

  public void Initialize(string storeID)
  {
    hfStoreID.Text = storeID;
  }

  public void CommandPopulate(bool isAdd, string suplierId, string noDo)
  {
    if (isAdd)
    {
      this.ClearEntrys();

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      PopulateDetail(suplierId, noDo);
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string suplId = (e.ExtraParams["SuplierID"] ?? string.Empty);
    string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);
    
    numberId = (string.IsNullOrEmpty(numberId) ? txDOHdr.Text.Trim() : numberId);
    suplId = (string.IsNullOrEmpty(suplId) ? 
      (cbSuplierHdr.SelectedItem != null ? cbSuplierHdr.SelectedItem.Value : string.Empty) : suplId);

    if (string.IsNullOrEmpty(numberId) || string.IsNullOrEmpty(suplId))
    {
      e.ErrorMessage = "DO atau Pemasok tidak boleh kosong.";

      e.Success = false;

      return;
    }
    
    Dictionary<string, string>[] gridData = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    PostDataParser.StructureResponse respon = SaveParser(numberId, suplId, gridData);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null,
          dateFjJs = null,
          dateDoJs = null;

        DateTime date = DateTime.Today;

        Functional.DateParser(txDateDoHdr.RawText, "d-M-yyyy", out date);
        dateDoJs = Functional.DateToJson(date);

        Functional.DateParser(txFJHdr.RawText, "d-M-yyyy", out date);
        dateFjJs = Functional.DateToJson(date);

        if (!string.IsNullOrEmpty(storeId))
        {
          scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_nosup': '{1}',
                'v_sup_name': '{2}',
                'c_dono': '{3}',
                'd_dodate': {4},
                'd_fjno': '{5}',
                'd_fjdate': {6},
                'v_cunam': '{7}',
                'v_via_kirim': '{8}',
                'c_taxno': '{9}'
              }}));{0}.commitChanges();", storeId,
                (cbSuplierHdr.SelectedItem != null ? cbSuplierHdr.SelectedItem.Value : string.Empty),
                (cbSuplierHdr.SelectedItem != null ? cbSuplierHdr.SelectedItem.Text : string.Empty),
                txDOHdr.Text, dateDoJs, txFJHdr.Text, dateFjJs,
                (cbCustomerHdr.SelectedItem != null ? cbCustomerHdr.SelectedItem.Text : string.Empty),
                (cbViaHdr.SelectedItem != null ? cbViaHdr.SelectedItem.Text : string.Empty),
                txTaxHdr.Text);

          X.AddScript(scrpt);
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
}
