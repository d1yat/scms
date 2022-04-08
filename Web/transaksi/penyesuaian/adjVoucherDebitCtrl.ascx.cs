using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_penyesuaian_adjVoucherDebitCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    hfADJDebit.Clear();

    txKeteranganHdrDR.Clear();
    txKeteranganHdrDR.Disabled = false;

    cbTipeDR.Clear();
    cbTipeDR.Disabled = false;

    cbNoRefDtlDR.Clear();
    cbNoRefDtlDR.Disabled = false;

    txValueDR.Clear();
    txValueDR.Disabled = false;

    txKetDetDR.Clear();
    txKetDetDR.Disabled = false;

    btnPrint.Hidden = true;

    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
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

    //string tmp = null;

    #region Parser Header

    ClearEntrys();

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0120", paramX);

    try
    {
      dicADJ = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicADJ.ContainsKey("records") && (dicADJ.ContainsKey("totalRows") && (((long)dicADJ["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicADJ["records"]);

        dicADJInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        txKeteranganHdrDR.Text = (dicADJInfo.ContainsKey("v_ket") ? dicADJInfo["v_ket"] : string.Empty);
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

      hfADJDebit.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_sales_AdjustSTTDonasi:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    cbTipeDR.Disabled = true;
    cbNoRefDtlDR.Disabled = true;
    txKetDetDR.Disabled = true;
    txValueDR.Disabled = true;
    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();

  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string adjNumber, string Keterangan, Dictionary<string, string>[] dics)
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
      type = null,
      noRef = null,
      ket = null,
      ketD = null;
    bool isVoid = false,
      isNew = false, isModify = false;
    string varData = null;

    decimal value = 0;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Type", "12");
    pair.DicAttributeValues.Add("Keterangan", Keterangan);

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
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        noRef = dicData.GetValueParser<string>("c_noref");
        type = dicData.GetValueParser<string>("c_type");
        value = dicData.GetValueParser<decimal>("n_value", 0);
        ket = dicData.GetValueParser<string>("v_ket");



        if ((!string.IsNullOrEmpty(noRef)))
        {

          dicAttr.Add("noRef", noRef);
          dicAttr.Add("TypeDet", type);
          dicAttr.Add("Value", value.ToString());
          dicAttr.Add("KetDet", ket);

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
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        noRef = dicData.GetValueParser<string>("c_noref");
        type = dicData.GetValueParser<string>("c_type");
        value = dicData.GetValueParser<decimal>("n_value", 0);
        ket = dicData.GetValueParser<string>("v_ket");
        ketD = dicData.GetValueParser<string>("v_ketD");

        if ((!string.IsNullOrEmpty(noRef)))
        {
          dicAttr.Add("noRef", noRef);
          dicAttr.Add("TypeDet", type);
          dicAttr.Add("Value", value.ToString());
          dicAttr.Add("KetDet", ket);

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr,
            Value = (string.IsNullOrEmpty(ketD) ? "Human error" : ketD),
          });
        }
      }

      dicData.Clear();
    }
    try
    {
      varData = parser.ParserData("ADJVOUCHER", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penerimaan_ADJVoucherDebitCtrl SaveParser : {0} ", ex.Message);
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
    hfType.Text = "01";
  }

  public void CommandPopulate(bool isAdd, string pID, string gdgID, string Type)
  {

    if (isAdd)
    {
      ClearEntrys();

      winDetail.Title = "Adjustment Voucher Debit";

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      winDetail.Hidden = false;
      winDetail.Title = "Adjustment " + pID;
      PopulateDetail("c_adjno", pID, Type);
    }
  }

  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string Keterangan = (e.ExtraParams["Keterangan"] ?? string.Empty);
    string StoreID = (e.ExtraParams["StoreID"] ?? string.Empty);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    Dictionary<string, string>[] gridDataAdjTrans = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, Keterangan, gridDataAdjTrans);

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

            if (!string.IsNullOrEmpty(StoreID))
            {
              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_adjno': '{1}',
                'd_adjdate': {2},
                'v_ketTran': 'Transaksi VC Kredit',
                'v_ket': '{3}'
              }}));{0}.commitChanges();", StoreID,
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
}
