using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_penyesuaian_AdjustStockBatch : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    hfADJBatchNo.Clear();

    cbGudangHdr.Clear();
    cbGudangHdr.Disabled = false;
    Ext.Net.Store cbGudangHdrstr = cbGudangHdr.GetStore();
    if (cbGudangHdrstr != null)
    {
        cbGudangHdrstr.RemoveAll();
    }

    txKeteranganHdr.Clear();
    txKeteranganHdr.Disabled = false;

    cbItemDtl.Clear();
    cbItemDtl.Disabled = false;
    Ext.Net.Store cbItemDtlstr = cbItemDtl.GetStore();
    if (cbItemDtlstr != null)
    {
        cbItemDtlstr.RemoveAll();
    }

    cbBatDtlMin.Clear();
    cbBatDtlMin.Disabled = false;
    Ext.Net.Store cbBatDtlMinstr = cbBatDtlMin.GetStore();
    if (cbBatDtlMinstr != null)
    {
        cbBatDtlMinstr.RemoveAll();
    }

    cbBatDtlPlus.Clear();
    cbBatDtlPlus.Disabled = false;
    Ext.Net.Store cbBatDtlPlusstr = cbBatDtlPlus.GetStore();
    if (cbBatDtlPlusstr != null)
    {
        cbBatDtlPlusstr.RemoveAll();
    }

    txGQtyDtl.Clear();
    txGQtyDtl.Disabled = false;

    txBQtyDtl.Clear();
    txBQtyDtl.Disabled = false;

    btnPrint.Hidden = true;

    txKetDtl.Clear();
    txKetDtl.Disabled = false;

    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private void PopulateDetail(string pName, string pID, string Case)
  {
    Dictionary<string, object> dicADJ = null;
    Dictionary<string, string> dicADJInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string tmp = null;

    #region Parser Header

    ClearEntrys();

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0074", paramX);

    try
    {
      dicADJ = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicADJ.ContainsKey("records") && (dicADJ.ContainsKey("totalRows") && (((long)dicADJ["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicADJ["records"]);

        dicADJInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());


        cbGudangHdr.ToBuilder().AddItem(
          (dicADJInfo.ContainsKey("v_gdgdesc") ? dicADJInfo["v_gdgdesc"] : string.Empty),
          (dicADJInfo.ContainsKey("c_gdg") ? dicADJInfo["c_gdg"] : string.Empty)
        );
        if (cbGudangHdr.GetStore() != null)
        {
          cbGudangHdr.GetStore().CommitChanges();
        }
        cbGudangHdr.SetValueAndFireSelect((dicADJInfo.ContainsKey("c_gdg") ? dicADJInfo["c_gdg"] : string.Empty));
        cbGudangHdr.Disabled = true;

        txKeteranganHdr.Text = (dicADJInfo.ContainsKey("v_ket") ? dicADJInfo["v_ket"] : string.Empty);

        winDetail.Title = string.Format("Adjust Batch List - {0}", pID);

        //cbBatDtlMin.Disabled = false;
        //cbBatDtlPlus.Disabled = false;
        //cbItemDtl.Disabled = false;
        //txBQtyDtl.Disabled = false;
        //txGQtyDtl.Disabled = false;
        //txKetDtl.Disabled = false;
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_sales_AdjustBatch:PopulateDetail Header - ", ex.Message));
    }
    finally
    {


      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicADJInfo != null)
      {
        dicADJInfo.Clear();
      }
      if (dicADJ != null)
      {
        dicADJ.Clear();
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

      hfADJBatchNo.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_sales_AdjustBatch:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();

  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string rnNumber, Dictionary<string, string>[] dics, string gudangID, string keterangan)
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
      batch = null,
      batchExpired = null,
      ket = null, KetDet = null;
    bool isVoid = false,
      isNew = false;
    string varData = null;

    decimal nGQty = 0, nBQty = 0;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Gudang", gudangID);
    pair.DicAttributeValues.Add("Type", "02");
    pair.DicAttributeValues.Add("Keterangan", keterangan);

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
        batch = dicData.GetValueParser<string>("c_batch");
        nGQty = dicData.GetValueParser<decimal>("n_gqty", 0);
        nBQty = dicData.GetValueParser<decimal>("n_bqty", 0);
        KetDet = dicData.GetValueParser<string>("v_ket");

        if ((!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)))
        {
          Functional.DateParser(batchExpired, "yyyy-MM-ddTHH:mm:ss", out date);

          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("GQty", nGQty.ToString());
          dicAttr.Add("BQty", nBQty.ToString());
          dicAttr.Add("KetDet", KetDet.ToString());

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

        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");

        if ((!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);

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
      varData = parser.ParserData("ADJBATCH", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penerimaan_ADJBATCHCtrl SaveParser : {0} ", ex.Message);
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

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  public void Initialize(string storeIDGridMain)
  {
    hfStoreID.Text = storeIDGridMain;
  }

  public void CommandPopulate(bool isAdd, string pID, string gdgID)
  {
    if (isAdd)
    {

      ClearEntrys();

      winDetail.Title = "Adjustment Batch";

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      PopulateDetail("c_adjno", pID, gdgID);
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string strGdgID = (e.ExtraParams["GudangID"] ?? string.Empty);
    string GudangDesc = (e.ExtraParams["GudangDesc"] ?? string.Empty);
    string Keterangan = (e.ExtraParams["Keterangan"] ?? string.Empty);
    string strStoreID = (e.ExtraParams["StoreID"] ?? string.Empty);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    Dictionary<string, string>[] gridDataAdjGoodBad = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataAdjGoodBad, strGdgID, Keterangan);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null,
          dateJs = null;

        DateTime date = DateTime.Today;

        if (isAdd)
        {
          if (respon.Values != null)
          {
            if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
            {
              dateJs = Functional.DateToJson(date);
            }

            numberId = respon.Values.GetValueParser<string>("adj", string.Empty); 

            if (!string.IsNullOrEmpty(strStoreID))
            {
              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'v_gdgdesc': '{1}',
                'c_gdg': '{2}',
                'c_adjno': '{3}',
                'd_adjdate': {4},
                'ketType': 'Perpindahan Batch',
                'v_ket': '{5}'
              }}));{0}.commitChanges();", strStoreID,
                    GudangDesc,
                    strGdgID,
                    respon.Values.GetValueParser<string>("Adj", string.Empty),
                    dateJs, Keterangan);

              X.AddScript(scrpt);
            }
          }
        }

        //this.ClearEntrys();
        this.PopulateDetail("c_adjno", numberId, strGdgID);

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
