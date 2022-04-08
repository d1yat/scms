using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class memo_stt_MemoSTTSampleCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Memo Combo";

    hfMemoSample.Clear();

    cbGudangHdr.Clear();
    cbGudangHdr.Disabled = false;

    txMemoHdr.Clear();
    txMemoHdr.Disabled = false;
    
    cbItemDtl.Clear();
    cbItemDtl.Disabled = false;
    
    cbKryHdr.Clear();
    cbKryHdr.Disabled = false;

    txQtyDtl.Clear();
    txQtyDtl.Disabled = false;

    txKeteranganHdr.Clear();
    txKeteranganHdr.Disabled = false;

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
        new string[] { "c_mtno = @0", pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0048", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("Memo Combo - {0}", pID);

        Functional.SetComboData(cbGudangHdr, "c_gdg", dicResultInfo.GetValueParser<string>("v_gdgdesc", string.Empty), dicResultInfo.GetValueParser<string>("c_gdg", string.Empty));

        txMemoHdr.Text = dicResultInfo.GetValueParser<string>("c_memo", string.Empty);

        Functional.SetComboData(cbKryHdr, "c_nip", dicResultInfo.GetValueParser<string>("v_nama", string.Empty), dicResultInfo.GetValueParser<string>("c_nip", string.Empty));

        txKeteranganHdr.Text = dicResultInfo.GetValueParser<string>("v_ket", string.Empty);

        cbGudangHdr.Disabled = true;
        cbKryHdr.Disabled = true;
        txMemoHdr.Disabled = true;

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("memo_combo_MemoComboCtrl:PopulateDetail Header - ", ex.Message));
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
              string.Format("[['gudang', '{0}', 'System.Char'],['c_mtno', '{0}', 'System.String']]", pName, gdgID, pID)
              , ParameterMode.Raw));
          }
          else
          {
            store.BaseParams["parameters"] = string.Format("[['gudang', '{0}', 'System.Char'],['c_mtno', '{0}', 'System.String']]", pName, gdgID, pID);
          }
        }
      }

      hfMemoSample.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("memo_combo_MemoComboCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string memoNumber, string gudangId, string memoId, string NipKry, string Keterangan, Dictionary<string, string>[] dics)
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
      ket = null;
    decimal nQty = 0;
    bool isNew = false,
      isVoid = false,
      isModify = false;
    string varData = null;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Gudang", gudangId);
    pair.DicAttributeValues.Add("MemoID", memoId);
    pair.DicAttributeValues.Add("Nip", NipKry);
    pair.DicAttributeValues.Add("Type", "02");
    pair.DicAttributeValues.Add("Keterangan", Keterangan);

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
        nQty = dicData.GetValueParser<decimal>("n_qty", 0);

        if ((!string.IsNullOrEmpty(item)))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Qty", nQty.ToString());

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
        ket = dicData.GetValueParser<string>("v_ket", string.Empty);

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

      dicData.Clear();
    }

    try
    {
      varData = parser.ParserData("MKMemoSample", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_memo_MemoSampleCtrl SaveParser : {0} ", ex.Message);
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

  public void CommandPopulate(bool isAdd, string pID, string gdgID, string Type)
  {
    if (isAdd)
    {
      hfMemoSampleType.Text = Type;
      ClearEntrys();

      winDetail.Title = "Memo Sample";

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      winDetail.Title = "Adjustment " + pID;
      PopulateDetail("c_adjno", pID, gdgID);
    }
  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string gdgId = (e.ExtraParams["Gudang"] ?? string.Empty);
    string gdgDesc = (e.ExtraParams["GudangDesc"] ?? string.Empty);
    string NamaKry = (e.ExtraParams["NamaKry"] ?? string.Empty);
    string NipKry = (e.ExtraParams["NipKry"] ?? string.Empty);
    string memoId = (e.ExtraParams["Memo"] ?? string.Empty);
    string Keterangan = (e.ExtraParams["Keterangan"] ?? string.Empty);

    Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gdgId, memoId, NipKry, Keterangan, gridDataPL);

    string App = NipKry +" - " + NamaKry ;

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;
        string storeId = hfStoreIDSample.Text;

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
                              'c_mtno': '{1}',
                              'd_mtdate': {2},
                              'v_gdgdesc': '{3}',
                              'v_ketTrans': 'Memo Sample',
                              'c_memo': '{4}',
                              'Nama': '{5}'
              }}));{0}.commitChanges();", storeId,
                                        respon.Values.GetValueParser<string>("MEMO", string.Empty),
                                        dateJs,
                                        gdgDesc,
                                        memoId,
                                        App);

              X.AddScript(scrpt);
            }
          }
        }

        //this.ClearEntrys();
        if (!string.IsNullOrEmpty(numberId))
        {
            this.PopulateDetail("c_stno", gdgId, numberId);
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

  public void Initialize(string storeIDGridMain)
  {
    hfStoreIDSample.Text = storeIDGridMain;
  }
}
