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

public partial class transaksi_memo_MemoComboCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Combo";

    hfComboNo.Clear();

    cbGudangHdr.Clear();
    cbGudangHdr.Disabled = false;

    cbMemoHdr.Clear();
    cbMemoHdr.Disabled = false;

    cbItemHdr.Clear();
    cbItemHdr.Disabled = false;
    Ext.Net.Store cbItemHdrstr = cbItemHdr.GetStore();
    if (cbItemHdrstr != null)
    {
        cbItemHdrstr.RemoveAll();
    }

    cbBatchHdr.Clear();
    cbBatchHdr.Disabled = false;
    Ext.Net.Store cbBatchHdrstr = cbBatchHdr.GetStore();
    if (cbBatchHdrstr != null)
    {
        cbBatchHdrstr.RemoveAll();
    }

    cbItemDtl.Clear();
    cbItemDtl.Disabled = false;
    Ext.Net.Store cbItemDtlstr = cbItemDtl.GetStore();
    if (cbItemDtlstr != null)
    {
        cbItemDtlstr.RemoveAll();
    }

    cbBatchDtl.Clear();
    cbBatchDtl.Disabled = false;
    Ext.Net.Store cbBatchDtlstr = cbBatchDtl.GetStore();
    if (cbBatchDtlstr != null)
    {
        cbBatchDtlstr.RemoveAll();
    }

    txQtyHdr.Clear();
    txQtyHdr.Disabled = false;

    txKeteranganHdr.Clear();
    txKeteranganHdr.Disabled = false;

    //X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));

    //cbItemDtl.Disabled = false;
    //txQtyDtl.Disabled = false;
    //btnAdd.Disabled = false;
    //btnClear.Disabled = false;

    btnProses.Hidden = false;
    btnPrint.Hidden = true;

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private void PopulateDetail(string pName, string gdgID, string pID)
  {
    ClearEntrys();

    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "c_gdg = @0", gdgID, "System.Char"},
        new string[] { "c_combono = @0", pID, "System.String"},
        new string[] { "typeCode", "01", "System.String"}        
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0046", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("Combo - {0}", pID);

        Functional.SetComboData(cbGudangHdr, "c_gdg", dicResultInfo.GetValueParser<string>("v_gdgdesc", string.Empty), dicResultInfo.GetValueParser<string>("c_gdg", string.Empty));
        cbGudangHdr.Disabled = true;

        Functional.SetComboData(cbMemoHdr, "c_memono", dicResultInfo.GetValueParser<string>("c_memo", string.Empty), dicResultInfo.GetValueParser<string>("c_memono", string.Empty));
        cbMemoHdr.Disabled = true;

        Functional.SetComboData(cbItemHdr, "c_iteno", dicResultInfo.GetValueParser<string>("v_itnam", string.Empty), dicResultInfo.GetValueParser<string>("c_iteno", string.Empty));
        cbItemHdr.Disabled = true;

        //txBatchHdr.Text = dicResultInfo.GetValueParser<string>("c_batch", string.Empty);
        //txBatchHdr.Disabled = true;

        Functional.SetComboData(cbBatchHdr, "c_batch", dicResultInfo.GetValueParser<string>("c_batch", string.Empty), dicResultInfo.GetValueParser<string>("c_batch", string.Empty));
        cbBatchHdr.Disabled = true;

        //txQtyHdr.SetRawValue(dicResultInfo.GetValueParser<decimal>("n_acc", 0));

        txQtyHdr.Text = dicResultInfo.GetValueParser<decimal>("n_acc", 0).ToString();
        txQtyHdr.Disabled = true;

        txKeteranganHdr.Text = dicResultInfo.GetValueParser<string>("v_ket", string.Empty);

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_memo_MemoComboCtrl:PopulateDetail Header - ", ex.Message));
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
              string.Format("[['gudang', '{0}', 'System.Char'],['comboID', '{0}', 'System.String']]", pName, gdgID, pID)
              , ParameterMode.Raw));
          }
          else
          {
            store.BaseParams["parameters"] = string.Format("[['gudang', '{0}', 'System.Char'],['memoID', '{0}', 'System.String']]", pName, gdgID, pID);
          }
        }
      }

      hfComboNo.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_memo_MemoComboCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    btnProses.Hidden = true;
    btnPrint.Hidden = false;

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string memoNumber, string gudangId, string memoId, string comboItem, string batchCode, decimal qtyNumber, string ketHeader, Dictionary<string, string>[] dics)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = memoNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string tmp = null,
      item = null,
      batch = null,
      ket = null;
    decimal nQty = 0,
      nKomposisi = 0;
    bool isNew = false,
      isVoid = false,
      isModify = false;
    string varData = null;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Gudang", gudangId);
    pair.DicAttributeValues.Add("MemoID", memoId);
    pair.DicAttributeValues.Add("ComboItem", comboItem);
    pair.DicAttributeValues.Add("Batch", batchCode);
    pair.DicAttributeValues.Add("Qty", qtyNumber.ToString());
    pair.DicAttributeValues.Add("Keterangan", ketHeader);
    //pair.DicAttributeValues.Add("Keterangan2", txKeterangan2Hdr.Text.Trim());

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      isNew = dicData.GetValueParser<bool>("l_new");
      isVoid = dicData.GetValueParser<bool>("l_void");
      isModify = false;

      if (isNew && (!isVoid) && (!isModify))
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        nQty = dicData.GetValueParser<decimal>("n_qty", 0);
        //tmp1 = dicData.GetValueParser<string>("d_expired");
        nKomposisi = dicData.GetValueParser<decimal>("n_pack", 0);
        ket = dicData.GetValueParser<string>("v_ket");

        if ((!string.IsNullOrEmpty(item)) &&
          (nQty > 0) && (nKomposisi > 0) && (!string.IsNullOrEmpty(batch)))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("Qty", nQty.ToString());
          dicAttr.Add("Komposisi", nKomposisi.ToString());

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
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        ket = dicData.GetValueParser<string>("v_ket");
        
        if (!string.IsNullOrEmpty(item))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);

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
      varData = parser.ParserData("LGMemoCombo", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_memo_MemoComboCtrl SaveParser : {0} ", ex.Message);
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

  public void CommandPopulate(bool isAdd, string gudangID, string pID)
  {
    if (isAdd)
    {
      this.ClearEntrys();

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      PopulateDetail("c_memono", gudangID, pID);
    }
  }

  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string gdgId = (e.ExtraParams["GudangID"] ?? string.Empty);
    string gdgDesc = (e.ExtraParams["GudangDesc"] ?? string.Empty);
    string memoId = (e.ExtraParams["MemoID"] ?? string.Empty);
    string comboItem = (e.ExtraParams["ComboItem"] ?? string.Empty);
    string comboItemDesc = (e.ExtraParams["ComboItemDesc"] ?? string.Empty);
    string batchCode = (e.ExtraParams["Batch"] ?? string.Empty);
    string qtyNumber = (e.ExtraParams["Qty"] ?? string.Empty);
    string ketHeader = (e.ExtraParams["Keterangan"] ?? string.Empty);

    decimal dQtyNumber = 0;
    decimal.TryParse(qtyNumber, out dQtyNumber);

    Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gdgId, memoId, comboItem, batchCode, dQtyNumber, ketHeader, gridDataPL);

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

            numberId = respon.Values.GetValueParser<string>("COMBO", string.Empty);

            if (!string.IsNullOrEmpty(storeId))
            {
              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                              'c_combono': '{1}',
                              'c_gdg': '{2}',
                              'v_gdgdesc': '{3}',
                              'd_combodate': {4},
                              'c_memono': '{5}',
                              'c_batch': '{6}',
                              'c_iteno': '{7}',
                              'n_acc': {8},
                              'n_bqty': 0,
                              'n_bsisa': 0,
                              'n_gqty': {8},
                              'n_gsisa': {8},
                              'l_confirm': false,
                              'v_ket': '{9}',
                              'v_itnam': '{10}'
              }}));{0}.commitChanges();", storeId,
                                        numberId,
                                        gdgId,
                                        gdgDesc,
                                        dateJs,
                                        memoId,
                                        batchCode,
                                        comboItem,
                                        qtyNumber,
                                        ketHeader,
                                        comboItemDesc);

              X.AddScript(scrpt);
            }
          }
        }

        //this.ClearEntrys();
        if (!string.IsNullOrEmpty(numberId))
        {
            PopulateDetail("c_combono", gdgId, numberId);
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

  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string gdgId = (e.ExtraParams["GudangID"] ?? string.Empty);
    string numberID = (e.ExtraParams["NumberID"] ?? string.Empty);

    if (string.IsNullOrEmpty(numberID))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");

      return;
    }

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();

    rptParse.ReportingID = "10109";

    #region Report Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(char).FullName,
      ParameterName = "LG_COMBOH.c_gdg",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_COMBOH.c_type",
      ParameterValue = "01"
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_COMBOH.c_combono",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "({LG_COMBOD1.N_QTY} <> 0)",
      IsReportDirectValue = true
    });
    
    #endregion

    #region Linq Filter Parameter

    //lstRptParam.Add(new ReportParameter()
    //{
    //  DataType = typeof(char).FullName,
    //  ParameterName = "c_gdg = @0",
    //  ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId),
    //  IsLinqFilterParameter = true
    //});

    //lstRptParam.Add(new ReportParameter()
    //{
    //  DataType = typeof(string).FullName,
    //  ParameterName = "c_combono = @0",
    //  //ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID),
    //  ParameterValue = "??????????",
    //  IsLinqFilterParameter = true
    //});

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
          rptResult.OutputFile, "Combo ", rptResult.Extension);

        wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
      }
      else
      {
        Functional.ShowMsgWarning(rptResult.MessageResponse);
      }
    }

    GC.Collect();
  }

  protected void ProcessCombo_Click(object sender, DirectEventArgs e)
  {
    string gudangId = (e.ExtraParams["gudangId"] ?? string.Empty);
    string comboItem = (e.ExtraParams["comboItem"] ?? string.Empty);
    string tmp = (e.ExtraParams["qty"] ?? string.Empty);

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "gudang", gudangId, "System.Char"},
        new string[] { "itemCombo", comboItem, "System.String"},
        new string[] { "Quantity", tmp, "System.Decimal"}
      };

    string result = soa.GlobalQueryService(0, -1, true, string.Empty, string.Empty, "12201", paramX);

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

          DateTime dTmp = DateTime.MinValue;
          string storeId = gridDetail.GetStore().ClientID;

          sb.AppendFormat("{0}.removeAll();", storeId);

          for (int nLoop = 0, nLen = lstResultInfo.Count; nLoop < nLen; nLoop++)
          {
            dicResultInfo = lstResultInfo[nLoop];

            sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{", storeId);

            sb.AppendFormat("'l_new': true,");

            foreach (KeyValuePair<string, string> kvp in dicResultInfo)
            {
              if (kvp.Key.Equals("d_expired", StringComparison.OrdinalIgnoreCase))
              {
                if (string.IsNullOrEmpty(kvp.Value))
                {
                  sb.AppendFormat("'{0}': '',", kvp.Key);
                }
                else
                {
                  sb.AppendFormat("'{0}': new {1},", kvp.Key, kvp.Value.Replace("\\/", ""));
                }
              }
              else
              {
                sb.AppendFormat("'{0}': '{1}',", kvp.Key, kvp.Value);
              }
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
      else
      {
        Functional.ShowMsgWarning("Stok tidak mencukupi atau combo tidak ditemukan.");
      }

      dicResult.Clear();
    }

  }
}