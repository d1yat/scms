using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_bonus_ClaimBonusAccRegularCtrl : System.Web.UI.UserControl
{
  protected void Page_Load(object sender, EventArgs e)
  {

  }

  private void ClearEntrys()
  {
    hfClaimAccNo.Clear();

    cbPrincipalHdr.Clear();
    cbPrincipalHdr.Disabled = false;

    txNoClaimAcc.Clear();
    txNoClaimAcc.Disabled = false;

    txDayClaimAcc.Clear();
    txDayClaimAcc.Disabled = false;

    txKeterangan.Clear();
    txKeterangan.Disabled = false;

    cbNoClaim.Clear();
    cbNoClaim.Disabled = false;

    txKeterangan.Clear();

    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));

    cbItemDtl.Disabled = false;
    cbItemDtl.Clear();
    
    txQtyDtlAcc.Disabled = false;
    txQtyDtlAcc.Clear();

    txQtyDtlTolak.Disabled = false;
    txQtyDtlTolak.Clear();

    winDetail.Title = "Claim ACC regular";

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

  private void PopulateDetail(string pName, string pID)
  {
    ClearEntrys();

    Dictionary<string, object> dicClaimAcc = null;
    Dictionary<string, string> dicClaimAccInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;
    DateTime date = DateTime.Today;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0070", paramX);

    try
    {
      dicClaimAcc = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicClaimAcc.ContainsKey("records") && (dicClaimAcc.ContainsKey("totalRows") && (((long)dicClaimAcc["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicClaimAcc["records"]);

        dicClaimAccInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        //cbKurs.ToBuilder().AddItem(
        //  (dicClaimInfo.ContainsKey("c_desc") ? dicClaimInfo["c_desc"] : string.Empty),
        //  (dicClaimInfo.ContainsKey("c_kurs") ? dicClaimInfo["c_kurs"] : string.Empty)
        //);
        //if (cbKurs.GetStore() != null)
        //{
        //  cbKurs.GetStore().CommitChanges();
        //}
        //cbKurs.SetValueAndFireSelect((dicClaimInfo.ContainsKey("c_kurs") ? dicClaimInfo["c_kurs"] : string.Empty));
        //cbKurs.Disabled = true;

        date = Functional.JsonDateToDate(dicClaimAccInfo.GetValueParser<string>("d_claimaccdate"));

        txDayClaimAcc.Text = date.ToString("dd-MM-yyyy");
        txDayClaimAcc.Disabled = true;

        Functional.SetComboData(cbNoClaim, "c_claimno", dicClaimAccInfo.GetValueParser<string>("c_claimno", string.Empty), dicClaimAccInfo.GetValueParser<string>("c_claimno", string.Empty));
        cbNoClaim.Disabled = true;

        //cbPrincipalHdr.ToBuilder().AddItem(
        //  (dicClaimInfo.ContainsKey("v_nama") ? dicClaimInfo["v_nama"] : string.Empty),
        //  (dicClaimInfo.ContainsKey("c_nosup") ? dicClaimInfo["c_nosup"] : string.Empty)
        //);
        //if (cbPrincipalHdr.GetStore() != null)
        //{
        //  cbPrincipalHdr.GetStore().CommitChanges();
        //}
        //cbPrincipalHdr.SetValueAndFireSelect((dicClaimInfo.ContainsKey("c_nosup") ? dicClaimInfo["c_nosup"] : string.Empty));
        //cbPrincipalHdr.Disabled = true;

        Functional.SetComboData(cbPrincipalHdr, "c_nosup", dicClaimAccInfo.GetValueParser<string>("v_nama", string.Empty), dicClaimAccInfo.GetValueParser<string>("c_nosup", string.Empty));
        cbPrincipalHdr.Disabled = true;

        txKeterangan.Text = (dicClaimAccInfo.ContainsKey("v_ket") ? dicClaimAccInfo["v_ket"] : string.Empty);

        txNoClaimAcc.Text = (dicClaimAccInfo.ContainsKey("c_claimnoprinc") ? dicClaimAccInfo["c_claimnoprinc"] : string.Empty);
        txNoClaimAcc.Disabled = true;

        winDetail.Title = string.Format("Retur Claim Acc Regular List - {0}", pID);
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_sales_ClaimRegular:PopulateDetail Header - ", ex.Message));
    }
    finally
    {

      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicClaimAccInfo != null)
      {
        dicClaimAccInfo.Clear();
      }
      if (dicClaimAcc != null)
      {
        dicClaimAcc.Clear();
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

      hfClaimAccNo.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_sales_ClaimRegular:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
    
  }

  public void CommandPopulate(bool isAdd, string pID)
  {
    if (isAdd)
    {
      ClearEntrys();

      winDetail.Title = "Claim ACC regular";

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      winDetail.Hidden = false;
      winDetail.ShowModal();
      PopulateDetail("c_claimaccno", pID);
      hfClaimAccNo.Text = pID;
    }
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string claimNumber, Dictionary<string, string>[] dics, string NoClaimAcc, string DayClaimAcc, string PrincipalId, string NoClaim, string Keterangan)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = claimNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string tmp = null,
      item = null, ket = null;
    decimal nQtyAcc = 0, nQtyTolak = 0;
    bool isNew = false,
      isVoid = false,
      isModify = false;
    string varData = null;


    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);

    pair.DicAttributeValues.Add("NoPrinsipal", NoClaimAcc);
    pair.DicAttributeValues.Add("TglPrinsipal", DayClaimAcc);
    pair.DicAttributeValues.Add("Suplier", PrincipalId);
    pair.DicAttributeValues.Add("claimno", NoClaim);
    pair.DicAttributeValues.Add("Keterangan", Keterangan);
    pair.DicAttributeValues.Add("Type", "01");

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

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        nQtyAcc = dicData.GetValueParser<decimal>("n_qtyacc", 0);
        nQtyTolak = dicData.GetValueParser<decimal>("n_qtytolak", 0);

        if ((!string.IsNullOrEmpty(item)))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("qtyAcc", nQtyAcc.ToString());
          dicAttr.Add("qtyTolak", nQtyTolak.ToString());
          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
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
        nQtyAcc = dicData.GetValueParser<decimal>("n_qtyacc", 0);
        nQtyTolak = dicData.GetValueParser<decimal>("n_qtytolak", 0);

        if ((!string.IsNullOrEmpty(item)))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("qtyAcc", nQtyAcc.ToString());
          dicAttr.Add("qtyTolak", nQtyTolak.ToString());
          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Human Error" : ket),
            DicAttributeValues = dicAttr
          });
        }
      }
      else if ((!isNew) && (!isVoid) && isModify)
      {

      }
      dicData.Clear();
    }
    try
    {
      varData = parser.ParserData("CLAIMACC", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_retur_ClaimBonusRegulerAccCtrl SaveParser : {0} ", ex.Message);
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
    string NoClaimAcc = (e.ExtraParams["NoClaimAcc"] ?? string.Empty);
    string DayClaimAcc = (e.ExtraParams["DayClaimAcc"] ?? string.Empty);
    string PrincipalId = (e.ExtraParams["PrincipalId"] ?? string.Empty);
    string PrincipalDesc = (e.ExtraParams["PrincipalDesc"] ?? string.Empty);
    string NoClaim = (e.ExtraParams["NoClaim"] ?? string.Empty);
    string Keterangan = (e.ExtraParams["Keterangan"] ?? string.Empty);
    string strStoreID = (e.ExtraParams["hfStoreID"] ?? string.Empty);

    Dictionary<string, string>[] gridDataClaim = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    DateTime dateClaimAccPrin = DateTime.Today;

    Functional.DateParser(DayClaimAcc, "yyyy-MM-ddTHH:mm:ss", out dateClaimAccPrin);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataClaim, NoClaimAcc, DayClaimAcc, PrincipalId, NoClaim, Keterangan);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null,
          dateJs = null, datePrin = null,
          ClaimAccID = null;


        DateTime date = DateTime.Today;
        DateTime date1 = DateTime.Today;

        if (!string.IsNullOrEmpty(DayClaimAcc))
        {
          date1 = Convert.ToDateTime(DayClaimAcc);
        }

       // datePrin = Functional.DateToJson(date1);

        if (isAdd)
        {
          if (respon.Values != null)
          {
            if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
            {
              dateJs = Functional.DateToJson(date);
            }
            if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal_Prinsipal", string.Empty), "yyyyMMdd", out date))
            {
              datePrin = Functional.DateToJson(date);
            }
            if (!string.IsNullOrEmpty(strStoreID))
            {
              ClaimAccID = respon.Values.GetValueParser<string>("CLAIMACC", string.Empty);

              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_claimaccno': '{1}',
                'd_claimaccdate': {2},
                'c_claimnoprinc': '{3}',
                'd_claimdateprinc': {4},
                'c_claimno': '{5}',
                'v_nama': '{6}',
              }}));{0}.commitChanges();", strStoreID,
                    ClaimAccID,
                    dateJs, NoClaimAcc,
                    datePrin, NoClaim, PrincipalDesc);
 
            }
          }
        }

        //if (string.IsNullOrEmpty(ClaimAccID))
        //{
        //  ClaimAccID = hfClaimAccNo.Text;
        //}

        X.AddScript(scrpt);

        PopulateDetail("c_claimaccno", (string.IsNullOrEmpty(numberId) ? NoClaimAcc : numberId));

        //this.ClearEntrys();

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
