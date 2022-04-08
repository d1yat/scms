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

public partial class transaksi_bonus_ClaimBonusProcessCtrl : System.Web.UI.UserControl
{
  private void ClearEntrys()
  {
    hfClaimNo.Clear();

    cbPrincipalHdr.Clear();
    cbPrincipalHdr.Disabled = false;

    cbDivPrinsipalHdr.Clear();
    cbDivPrinsipalHdr.Disabled = false;

    txKeteranganHdr.Clear();
    txKeteranganHdr.Disabled = false;

    cbMonthHdr.Clear();
    cbMonthHdr.Disabled = false;

    txYearHdr.Clear();
    txYearHdr.Disabled = false;

    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      DateTime date = DateTime.Now;

      txYearHdr.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
      date = date.AddYears(-1);
      txYearHdr.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
      date = date.AddYears(-1);
      txYearHdr.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
      txYearHdr.SelectedIndex = 0;

      Functional.PopulateBulan(cbMonthHdr, date.Month);
    }
  }

  public void Initialize(string storeIDGridMain)
  {
    hfStoreID.Text = storeIDGridMain;
  }

  private void PopulateDetail(string pName, string pID)
  {
    ClearEntrys();

    Dictionary<string, object> dicClaim = null;
    Dictionary<string, string> dicClaimInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0061", paramX);

    try
    {
      dicClaim = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicClaim.ContainsKey("records") && (dicClaim.ContainsKey("totalRows") && (((long)dicClaim["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicClaim["records"]);

        dicClaimInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        cbPrincipalHdr.ToBuilder().AddItem(
          (dicClaimInfo.ContainsKey("v_nama") ? dicClaimInfo["v_nama"] : string.Empty),
          (dicClaimInfo.ContainsKey("c_nosup") ? dicClaimInfo["c_nosup"] : string.Empty)
        );
        if (cbPrincipalHdr.GetStore() != null)
        {
          cbPrincipalHdr.GetStore().CommitChanges();
        }
        cbPrincipalHdr.SetValueAndFireSelect((dicClaimInfo.ContainsKey("c_nosup") ? dicClaimInfo["c_nosup"] : string.Empty));
        cbPrincipalHdr.Disabled = true;

        txKeteranganHdr.Text = (dicClaimInfo.ContainsKey("v_ket") ? dicClaimInfo["v_ket"] : string.Empty);

        txYearHdr.Text = (dicClaimInfo.ContainsKey("s_tahun") ? dicClaimInfo["s_tahun"] : string.Empty);
        txYearHdr.Disabled = true;

        cbMonthHdr.ToBuilder().AddItem(
          (dicClaimInfo.ContainsKey("v_nama") ? dicClaimInfo["v_nama"] : string.Empty),
          (dicClaimInfo.ContainsKey("t_bulan") ? dicClaimInfo["t_bulan"] : string.Empty)
        );
        if (cbMonthHdr.GetStore() != null)
        {
          cbMonthHdr.GetStore().CommitChanges();
        }
        cbMonthHdr.SetValueAndFireSelect((dicClaimInfo.ContainsKey("t_bulan") ? dicClaimInfo["t_bulan"] : string.Empty));
        cbMonthHdr.Disabled = true;

        winDetail.Title = string.Format("Retur Claim Regular List - {0}", pID);
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
      if (dicClaimInfo != null)
      {
        dicClaimInfo.Clear();
      }
      if (dicClaim != null)
      {
        dicClaim.Clear();
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

      hfClaimNo.Text = pID;
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

      winDetail.Title = "Claim Process";

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      winDetail.Hidden = false;
      winDetail.ShowModal();
      PopulateDetail("c_claimno", pID);
    }

  }

  protected void ProcessORP_Click(object sender, DirectEventArgs e)
  {
    string Tahun = (e.ExtraParams["Tahun"] ?? string.Empty);
    string Bulan = (e.ExtraParams["Bulan"] ?? string.Empty);
    string Supplier = (e.ExtraParams["Supplier"] ?? string.Empty);
    string divSupplier = (e.ExtraParams["divSupplier"] ?? string.Empty);

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "Tahun", Tahun, "System.String"},
        new string[] { "Bulan", Bulan, "System.String"},
        new string[] { "Supplier", Supplier, "System.String"},
        new string[] { "divSupplier", divSupplier, "System.String"},
      };

    string result = soa.GlobalQueryService(0, -1, true, string.Empty, string.Empty, "0069", paramX);

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

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string claimNumber, Dictionary<string, string>[] dics, string Year,string Month, string Prinsipal, string Keterangan)
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
      item = null, cusno = null;
    decimal nSales = 0, nGret = 0, nBret = 0, nQty = 0, nDisc = 0, nSalpri = 0;
    bool isNew = false,
      isVoid = false,
      isModify = false;
    string varData = null;


    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    
    pair.DicAttributeValues.Add("Tahun", Year);
    pair.DicAttributeValues.Add("Suplier", Prinsipal);
    pair.DicAttributeValues.Add("Bulan", Month);
    pair.DicAttributeValues.Add("Keterangan", Keterangan);
    pair.DicAttributeValues.Add("Type", "01");

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      isNew = dicData.GetValueParser<bool>("l_new");
      isVoid = dicData.GetValueParser<bool>("l_void");
      isModify = dicData.GetValueParser<bool>("l_modified");

      dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      dicAttr.Add("New", "true");
      dicAttr.Add("Delete", isVoid.ToString().ToLower());
      dicAttr.Add("Modified", isModify.ToString().ToLower());

      cusno = dicData.GetValueParser<string>("c_cusno");
      item = dicData.GetValueParser<string>("c_iteno");
      nSales = dicData.GetValueParser<decimal>("qty", 0);
      nGret = dicData.GetValueParser<decimal>("gret", 0);
      nBret = dicData.GetValueParser<decimal>("bret", 0);

      nDisc = dicData.GetValueParser<decimal>("n_disc", 0);
      nSalpri = dicData.GetValueParser<decimal>("n_salpri", 0);

      if ((!string.IsNullOrEmpty(item)))
      {
        dicAttr.Add("Item", item);
        dicAttr.Add("cusno", cusno);
        dicAttr.Add("Quantity", nSales.ToString());
        dicAttr.Add("nGret", nGret.ToString());
        dicAttr.Add("nBret", nBret.ToString());
        dicAttr.Add("Disc", nDisc.ToString());
        dicAttr.Add("Salpri", nSalpri.ToString());

        pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
        {
          IsSet = true,
          DicAttributeValues = dicAttr
        });
      }
      dicData.Clear();
    }
    try
    {
      varData = parser.ParserData("CLAIM_PROCCESS", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_retur_ClaimBonusProcessCtrl SaveParser : {0} ", ex.Message);
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
    string Year = (e.ExtraParams["Year"] ?? string.Empty);
    string Month = (e.ExtraParams["Month"] ?? string.Empty);
    string Prinsipal = (e.ExtraParams["Prinsipal"] ?? string.Empty);
    string Keterangan = (e.ExtraParams["Keterangan"] ?? string.Empty);
    string strSuplDesc = (e.ExtraParams["SuplierDesc"] ?? string.Empty);
    string strStoreID = (e.ExtraParams["StoreID"] ?? string.Empty);

    Dictionary<string, string>[] gridDataClaim = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataClaim, Year, Month, Prinsipal, Keterangan);

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
                'c_claimno': '{1}',
                'd_claimdate': {2},
                'v_nama': '{3}',
                's_tahun': {4},
                't_bulan': {5}
              }}));{0}.commitChanges();", strStoreID,
                    respon.Values.GetValueParser<string>("CLAIM", string.Empty),
                    dateJs, strSuplDesc,
                    Year, Month);

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
