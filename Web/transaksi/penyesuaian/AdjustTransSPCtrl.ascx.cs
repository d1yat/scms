using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
public partial class transaksi_penyesuaian_AdjustTransSP : System.Web.UI.UserControl
{
  private void ClearEntrys()
  {

    hfADJTrans.Clear();
    hfAdjType.Clear();

    cbGudangHdr.Clear();
    cbGudangHdr.Disabled = false;

    txKeteranganHdr.Clear();
    txKeteranganHdr.Disabled = false;

    cbItemDtl.Clear();
    cbItemDtl.Disabled = false;

    cbPODtl.Clear();
    cbPODtl.Disabled = false;

    txQtyDtl.Clear();
    txQtyDtl.Disabled = false;

    btnPrint.Hidden = true;

    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  public void Initialize(string storeIDGridMain)
  {
    hfStoreID.Text = storeIDGridMain;
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  private void PopulateDetail(string pName, string pID, string type)
  {
    Dictionary<string, object> dicADJ = null;
    Dictionary<string, string> dicADJInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"},
        new string[] { string.Format("c_type = @0"), type, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string tmp = null;

    #region Parser Header

    ClearEntrys();

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0125", paramX);

    try
    {
      dicADJ = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicADJ.ContainsKey("records") && (dicADJ.ContainsKey("totalRows") && (((long)dicADJ["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicADJ["records"]);

        dicADJInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        Functional.SetComboData(cbGudangHdr, "c_gdg", dicADJInfo.GetValueParser<string>("v_gdgdesc", string.Empty), dicADJInfo.GetValueParser<string>("c_gdg", string.Empty));
        cbGudangHdr.Disabled = true;

        txKeteranganHdr.Text = (dicADJInfo.ContainsKey("v_ket") ? dicADJInfo["v_ket"] : string.Empty);
      }
    }

    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_sales_AdjustFB:PopulateDetail Header - ", ex.Message));
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

      hfADJTrans.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_sales_AdjustSTTDonasi:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    cbPODtl.Disabled = true;
    cbItemDtl.Disabled = true;
    
    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();

  }

  public void CommandPopulate(bool isAdd, string pID, string gdgID, string Type)
  {
    if (isAdd)
    {
      hfAdjType.Text = Type;
      ClearEntrys();

      switch (Type)
      {
        case "08":
          winDetail.Title = "Adjustment PO";
          break;
      }
      

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      winDetail.Title = "Adjustment PO " + pID ;
      PopulateDetail("c_adjno", pID, gdgID);
    }
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string adjNumber, Dictionary<string, string>[] dics, string gudangID, string keterangan)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = adjNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string tmp = null,
      item = null,
      noRef = null,
      batchExpired = null,
      ket = null;
    bool isVoid = false,
      isNew = false;
    string varData = null;

    decimal nQty = 0;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Gudang", gudangID);
    pair.DicAttributeValues.Add("Type", "08");
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
        noRef = dicData.GetValueParser<string>("c_noref");
        nQty = dicData.GetValueParser<decimal>("n_qty", 0);

        if ((!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(noRef)))
        {
          Functional.DateParser(batchExpired, "yyyy-MM-ddTHH:mm:ss", out date);

          dicAttr.Add("NoRef", noRef);
          dicAttr.Add("Item", item);
          dicAttr.Add("Qty", nQty.ToString());

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

        noRef = dicData.GetValueParser<string>("c_noref");
        item = dicData.GetValueParser<string>("c_iteno");

        if ((!string.IsNullOrEmpty(noRef)) &&
          (!string.IsNullOrEmpty(item)))
        {
          dicAttr.Add("NoRef", noRef);
          dicAttr.Add("Item", item);

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
      varData = parser.ParserData("ADJTRANS", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penerimaan_ADJTRANSCtrl SaveParser : {0} ", ex.Message);
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

  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string strGdgID = (e.ExtraParams["GudangID"] ?? string.Empty);
    string GudangDesc = (e.ExtraParams["GudangDesc"] ?? string.Empty);
    string Keterangan = (e.ExtraParams["Keterangan"] ?? string.Empty);
    string strStoreID = (e.ExtraParams["StoreID"] ?? string.Empty);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    Dictionary<string, string>[] gridDataAdjTrans = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataAdjTrans, strGdgID, Keterangan);

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

            if (!string.IsNullOrEmpty(strStoreID))
            {
              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_gdg': '{1}',
                'v_gdgdesc': '{2}',
                'c_adjno': '{3}',
                'd_adjdate': {4},
                'v_ket': '{5}',
                'l_print': false,
                'l_status': false
              }}));{0}.commitChanges();", strStoreID,
                    strGdgID, GudangDesc,
                    respon.Values.GetValueParser<string>("ADJ", string.Empty),
                    dateJs, Keterangan);

              X.AddScript(scrpt);
            }
          }
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
