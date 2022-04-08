using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;


public partial class transaksi_penerimaan_RNRepack : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    hfRnNo.Clear();

    winDetail.Title = "Receive Notes Repack";

    cbGudangHdr.Clear();
    cbGudangHdr.Disabled = false;
    Ext.Net.Store cbGudangHdrrstr = cbGudangHdr.GetStore();
    if (cbGudangHdrrstr != null)
    {
        cbGudangHdrrstr.RemoveAll();
    }

    cbSuplierHdr.Clear();
    cbSuplierHdr.Disabled = false;
    Ext.Net.Store cbSuplierHdrstr = cbSuplierHdr.GetStore();
    if (cbSuplierHdrstr != null)
    {
        cbSuplierHdrstr.RemoveAll();
    }

    txDoHdr.Clear();
    txDoHdr.Disabled = false;

    txDateDoHdr.Clear();
    txDateDoHdr.Disabled = false;

    txBeaHdr.SetRawValue(0);
    txBeaHdr.Disabled = false;

    txKeteranganHdr.Clear();
    txKeteranganHdr.Disabled = false;

    //btnPrint.Hidden = true;

    btnSave.Hidden = false;

    //txDateDtl.MinDate = DateTime.Now;

    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
    Ext.Net.Store storeNew = gridDetailNewItem.GetStore();
    if (storeNew != null)
    {
      storeNew.RemoveAll();
    }
  }

  private void PopulateDetail(string pName, string pID, string gdgID)
  {
    ClearEntrys();

    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"},
        new string[] { "c_gdg = @0", gdgID, "System.Char"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0026", paramX);

    DateTime date = DateTime.Today;

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("Receive Notes Pembelian - {0}", pID);

        Functional.SetComboData(cbGudangHdr, "c_gdg", dicResultInfo.GetValueParser<string>("v_gdgdesc", string.Empty), dicResultInfo.GetValueParser<string>("c_gdg", string.Empty));
        cbGudangHdr.Disabled = true;

        Functional.SetComboData(cbSuplierHdr, "c_from", dicResultInfo.GetValueParser<string>("v_nama", string.Empty), dicResultInfo.GetValueParser<string>("c_from", string.Empty));
        cbSuplierHdr.Disabled = true;

        txDoHdr.Text = dicResultInfo.GetValueParser<string>("c_dono", string.Empty);
        txDoHdr.Disabled = true;

        txBeaHdr.SetRawValue(dicResultInfo.GetValueParser<decimal>("n_bea", 0));
        txBeaHdr.Disabled = true;

        txKeteranganHdr.Text = dicResultInfo.GetValueParser<string>("v_ket");

        date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_dodate", string.Empty));

        txDateDoHdr.SetValue(date.ToString("dd-MM-yyyy"));
        txDateDoHdr.Disabled = true;

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_penerimaan_RNPembelianCtrl:PopulateDetail Header - ", ex.Message));
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
      Ext.Net.Store storeNew = gridDetailNewItem.GetStore();
      if ((store.Proxy.Count > 0) || (storeNew.Proxy.Count > 0))
      {
        Ext.Net.ScriptTagProxy stp = store.Proxy[0] as Ext.Net.ScriptTagProxy;
        Ext.Net.ScriptTagProxy stpNew = storeNew.Proxy[0] as Ext.Net.ScriptTagProxy;
        if (((stp != null) || ((stpNew != null))) && stp.CallbackParam.Equals(Functional.NAME_SOA_SCMS_CALLBACK, StringComparison.OrdinalIgnoreCase))
        {
          string param = (store.BaseParams["parameters"] ?? string.Empty);
          string paramNew = (storeNew.BaseParams["parameters"] ?? string.Empty);
          if ((string.IsNullOrEmpty(param)) || (string.IsNullOrEmpty(paramNew)))
          {
            store.BaseParams.Add(new Ext.Net.Parameter("parameters",
              string.Format(@"[['gudang', '{0}', 'System.Char'],
                                ['{1} = @0', '{2}', 'System.String']]", gdgID, pName, pID),
                                                                      ParameterMode.Raw));
            storeNew.BaseParams.Add(new Ext.Net.Parameter("parameters",
              string.Format(@"[['gudang', '{0}', 'System.Char'],
                                ['{1} = @0', '{2}', 'System.String']]", gdgID, pName, pID),
                                                                      ParameterMode.Raw));
          }
          else
          {
            store.BaseParams["parameters"] = string.Format(@"[['gudang', '{0}', 'System.Char'],
                                                              ['{1} = @0', '{2}', 'System.String']]", gdgID, pName, pID);

            storeNew.BaseParams["parameters"] = string.Format(@"[['gudang', '{0}', 'System.Char'],
                                                              ['{1} = @0', '{2}', 'System.String']]", gdgID, pName, pID);
          }
        }
      }

      hfRnNo.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", storeNew.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_penerimaan_RNPembelianCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string rnNumber, Dictionary<string, string>[] dics, Dictionary<string, string>[] dicsNew, string gudangID, string suplierID, string nomorDO, DateTime tglDO, float bea, string keterangan)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = rnNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string tmp = null,
      item = null,
      rs = null,
      batch = null,
      batchExpired = null,
      ket = null;
    bool isVoid = false,
      isNew = false;
    string varData = null;

    decimal nGQty = 0, nBQty = 0;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry",((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("TypeRN", "04");
    pair.DicAttributeValues.Add("Gdg", gudangID);
    pair.DicAttributeValues.Add("Suplier", suplierID);
    pair.DicAttributeValues.Add("SuplierDO", nomorDO.Trim());
    pair.DicAttributeValues.Add("TanggalDO", tglDO.ToString("yyyyMMddHHmmssfff"));
    pair.DicAttributeValues.Add("Bea", bea.ToString());
    pair.DicAttributeValues.Add("Floating", "false");
    pair.DicAttributeValues.Add("Keterangan", keterangan);

    #region Item RS

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

        rs = dicData.GetValueParser<string>("c_refno");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        nGQty = dicData.GetValueParser<decimal>("n_gqty", 0);
        nBQty = dicData.GetValueParser<decimal>("n_bqty", 0);

        if ((!string.IsNullOrEmpty(rs)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)) &&
          ((nGQty > 0) || (nBQty > 0)))
        {
          Functional.DateParser(batchExpired, "yyyy-MM-ddTHH:mm:ss", out date);

          dicAttr.Add("Item", item);
          dicAttr.Add("Type", "01");
          dicAttr.Add("Batch", batch);
          dicAttr.Add("RefID", rs);
          dicAttr.Add("Expired", date.ToString("yyyyMMdd"));
          dicAttr.Add("Qty", nGQty.ToString());
          dicAttr.Add("QtyBad", nBQty.ToString());

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

        rs = dicData.GetValueParser<string>("c_refno");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");

        if ((!string.IsNullOrEmpty(rs)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("RefID", rs);

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

    #endregion

    #region Item New

    for (int nLoop = 0, nLen = dicsNew.Length; nLoop < nLen; nLoop++)
    {
      int c = nLoop + dics.Length;

      tmp = c.ToString();

      dicData = dicsNew[nLoop];

      isNew = dicData.GetValueParser<bool>("l_new");
      isVoid = dicData.GetValueParser<bool>("l_void");

      if (isNew && (!isVoid))
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", "false");

        rs = dicData.GetValueParser<string>("c_refno");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        nGQty = dicData.GetValueParser<decimal>("n_gqty", 0);
        batchExpired = dicData.GetValueParser<string>("d_batchexpired");

        if ((!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)) &&
          (nGQty > 0))
        {
          Functional.DateParser(batchExpired, "yyyy-MM-ddTHH:mm:ss", out date);

          dicAttr.Add("Item", item);
          dicAttr.Add("Type", "01");
          dicAttr.Add("Batch", batch);
          dicAttr.Add("Expired", date.ToString("yyyyMMdd"));
          dicAttr.Add("Qty", nGQty.ToString());
          dicAttr.Add("RepackNew", "true");

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr
          });
        }

      }
    }

    #endregion

    try
    {
      varData = parser.ParserData("RNRepack", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penerimaan_RNRepackCtrl SaveParser : {0} ", ex.Message);
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

  public void Initialize(string storeIDGridMain)
  {
    hfStoreID.Text = storeIDGridMain;
  }

  public void CommandPopulate(bool isAdd, string pID, string gdgID)
  {
    if (isAdd)
    {
      //SetEditor(false);

      ClearEntrys();
      Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
      Functional.SetComboData(cbGudangHdr, "c_gdg", pag.ActiveGudangDescription, pag.ActiveGudang);

      //X.AddScript(string.Format("{0}.removeAll();{0}.reload();", cbTipeDtl.GetStore().ClientID));
      //X.AddScript(string.Format("{0}.removeAll();{0}.reload();", cbTipeDtl.GetStore().ClientID));
      //cbTipeDtl.SelectByValue("01", true);
      //Functional.SetComboData(cbTipeDtl, "c_type", "Beli", "01");

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      PopulateDetail("c_rnno", pID, gdgID);
    }
  }

  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string jsonGridValuesNew = (e.ExtraParams["gridValuesNew"] ?? string.Empty);
    string strGdgID = (e.ExtraParams["GudangID"] ?? string.Empty);
    string strGdgDesc = (e.ExtraParams["GudangDesc"] ?? string.Empty);
    string strSuplID = (e.ExtraParams["SuplierID"] ?? string.Empty);
    string strSuplDesc = (e.ExtraParams["SuplierDesc"] ?? string.Empty);
    string strNumberDO = (e.ExtraParams["NumberDO"] ?? string.Empty);
    string strKeterangan = (e.ExtraParams["Keterangan"] ?? string.Empty);
    string strStoreID = (e.ExtraParams["StoreID"] ?? string.Empty);

    string strDateDO = (e.ExtraParams["DateDO"] ?? string.Empty);
    string strBea = (e.ExtraParams["Bea"] ?? string.Empty);

    float fBea = 0;
    DateTime dateDO = DateTime.Today;

    float.TryParse(strBea, out fBea);
    Functional.DateParser(strDateDO, "yyyy-MM-ddTHH:mm:ss", out dateDO);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    if (isAdd)
    {
      if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowAdd)
      {
        Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
        return;
      }
    }
    else if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowEdit)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menyimpan data.");
      return;
    }

    Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    Dictionary<string, string>[] gridDataPLNew = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValuesNew);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataPL, gridDataPLNew, strGdgID, strSuplID, strNumberDO, dateDO, fBea, strKeterangan);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null,
          dateJs = null,
          dateDoJs = null;

        DateTime date = DateTime.Today;

        if (isAdd)
        {
          if (respon.Values != null)
          {
            if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
            {
              dateJs = Functional.DateToJson(date);
            }

            dateDoJs = Functional.DateToJson(dateDO);
            numberId = respon.Values.GetValueParser<string>("RN", string.Empty);

            if (!string.IsNullOrEmpty(strStoreID))
            {
              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_gdg': '{1}',
                'v_gdgdesc': '{2}',
                'c_rnno': '{3}',
                'd_rndate': {4},
                'c_dono': '{5}',
                'd_dodate': {6},
                'v_nama': '{7}',
                'l_float': false,
                'l_print': false,
                'l_status': false
              }}));{0}.commitChanges();", strStoreID,
                    strGdgID, strGdgDesc,
                    numberId,
                    dateJs, strNumberDO, dateDoJs, strSuplDesc);

              X.AddScript(scrpt);
            }
          }
        }

        //this.ClearEntrys();
        if (!string.IsNullOrEmpty(numberId))
        {
          PopulateDetail("c_rnno", numberId, strGdgID);
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

  protected void ReloadBtn_Click(object sender, DirectEventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    this.ClearEntrys();
  }
}
