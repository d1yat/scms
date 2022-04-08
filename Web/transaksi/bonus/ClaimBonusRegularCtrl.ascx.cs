using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_bonus_ClaimBonusRegularCtrl : System.Web.UI.UserControl
{
  private void ClearEntrys()
  {
    hfClaimNo.Clear();

    cbPrincipalHdr.Clear();
    cbPrincipalHdr.Disabled = false;

    txKeterangan.Clear();
    txKeterangan.Disabled = false;

    cbKurs.Clear();
    cbKurs.Disabled = false;

    txKeterangan.Clear();

    txYear.Clear();
    txYear.Disabled = false;

    cbMonthHdr.Clear();
    cbMonthHdr.Disabled = false;

    txTop.Clear();
    txTop.Disabled = false;

    txKurs.Clear();
    txKurs.Disabled = false;
    

    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));

    cbItemDtl.Disabled = false;
    cbItemDtl.Clear();
    txQtyDtl.Disabled = false;
    txQtyDtl.Clear();

    winDetail.Title = "Claim regular";

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
    if (!this.IsPostBack)
    {
      DateTime date = DateTime.Now;

      txYear.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
      date = date.AddYears(-1);
      txYear.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
      date = date.AddYears(-1);
      txYear.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
      txYear.SelectedIndex = 0;

      Functional.PopulateBulan(cbMonthHdr, date.Month);
    }
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

    string MontNames = null;
    int month = 0;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0061", paramX);

    try
    {
      dicClaim = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicClaim.ContainsKey("records") && (dicClaim.ContainsKey("totalRows") && (((long)dicClaim["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicClaim["records"]);

        dicClaimInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

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

        Functional.SetComboData(cbKurs, "c_kurs", dicClaimInfo.GetValueParser<string>("c_desc", string.Empty), dicClaimInfo.GetValueParser<string>("c_kurs", string.Empty));
        cbKurs.Disabled = true;

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

        Functional.SetComboData(cbPrincipalHdr, "c_nosup", dicClaimInfo.GetValueParser<string>("v_nama", string.Empty), dicClaimInfo.GetValueParser<string>("c_nosup", string.Empty));
        cbPrincipalHdr.Disabled = true;

        txKeterangan.Text = (dicClaimInfo.ContainsKey("v_ket") ? dicClaimInfo["v_ket"] : string.Empty);

        txYear.Text = (dicClaimInfo.ContainsKey("s_tahun") ? dicClaimInfo["s_tahun"] : string.Empty);
        txYear.Disabled = true;

        month = Convert.ToInt16((dicClaimInfo.ContainsKey("t_bulan") ? dicClaimInfo["t_bulan"] : string.Empty));
        IFormatProvider provider = null;
        System.Globalization.DateTimeFormatInfo info = System.Globalization.DateTimeFormatInfo.GetInstance(provider);
        info.GetAbbreviatedMonthName(month);
        MontNames = info.GetMonthName(month);

        cbMonthHdr.Text = MontNames;
        cbMonthHdr.Disabled = true;

        txTop.Number = Convert.ToDouble(dicClaimInfo.ContainsKey("n_top") ? Convert.ToDouble(dicClaimInfo["n_top"]) : double.MinValue);
        txTop.Disabled = true;

        txKurs.Text = (dicClaimInfo.ContainsKey("n_kurs") ? dicClaimInfo["n_kurs"] : string.Empty);
        txKurs.Disabled = true;

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

      winDetail.Title = "Claim regular";

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

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string claimNumber, Dictionary<string, string>[] dics, string SuplierId, string Top, string KursTipe, string KursVal, string Keterangan, string Year, string Month)
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
    decimal nQty = 0, nDisc = 0, nSalpri = 0;
    bool isNew = false,
      isVoid = false,
      isModify = false;
    string varData = null;


    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);

    pair.DicAttributeValues.Add("Tahun", Year);
    pair.DicAttributeValues.Add("Bulan", Month);
    pair.DicAttributeValues.Add("Suplier", SuplierId);
    pair.DicAttributeValues.Add("Top", Top);
    pair.DicAttributeValues.Add("KursDesc", KursTipe);
    pair.DicAttributeValues.Add("KursVal", KursVal);
    pair.DicAttributeValues.Add("Keterangan", Keterangan);
    pair.DicAttributeValues.Add("Type","01");

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
        nQty = dicData.GetValueParser<decimal>("n_qty", 0);
        nDisc = dicData.GetValueParser<decimal>("n_disc", 0);
        nSalpri = dicData.GetValueParser<decimal>("n_salpri", 0);

        if ((!string.IsNullOrEmpty(item)) &&
          (nQty != 0))
          {
            dicAttr.Add("Item", item);
            dicAttr.Add("Quantity", nQty.ToString());
            dicAttr.Add("Disc", nDisc.ToString());
            dicAttr.Add("Salpri", nSalpri.ToString());
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
        nQty = dicData.GetValueParser<decimal>("n_qty", 0);
        nDisc = dicData.GetValueParser<decimal>("n_disc", 0);
        nSalpri = dicData.GetValueParser<decimal>("n_salpri", 0);

        if ((!string.IsNullOrEmpty(item)) &&
          (nQty != 0))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Quantity", nQty.ToString());
          dicAttr.Add("Disc", nDisc.ToString());
          dicAttr.Add("Salpri", nSalpri.ToString());
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
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        nQty = dicData.GetValueParser<decimal>("n_qty", 0);
        nDisc = dicData.GetValueParser<decimal>("n_disc", 0);
        nSalpri = dicData.GetValueParser<decimal>("n_salpri", 0);

        if ((!string.IsNullOrEmpty(item)) &&
          (nQty != 0))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Quantity", nQty.ToString());
          dicAttr.Add("Disc", nDisc.ToString());
          dicAttr.Add("Salpri", nSalpri.ToString());
          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Human Error" : ket),
            DicAttributeValues = dicAttr
          });
        }
      }
      dicData.Clear();
    }
    try
    {
      varData = parser.ParserData("CLAIM", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_retur_ClaimBonusRegulerCtrl SaveParser : {0} ", ex.Message);
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
    string strSuplDesc = (e.ExtraParams["SuplierDesc"] ?? string.Empty);
    string SuplierId = (e.ExtraParams["SuplierId"] ?? string.Empty);
    string Top = (e.ExtraParams["Top"] ?? string.Empty);
    string KursTipe = (e.ExtraParams["KursTipe"] ?? string.Empty);
    string KursVal = (e.ExtraParams["KursVal"] ?? string.Empty);
    string Keterangan = (e.ExtraParams["Keterangan"] ?? string.Empty);
    string Year = (e.ExtraParams["Year"] ?? string.Empty);
    string Month = (e.ExtraParams["Month"] ?? string.Empty);
    string strStoreID = (e.ExtraParams["StoreID"] ?? string.Empty);

    Dictionary<string, string>[] gridDataClaim = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    string ClaimID = null;

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataClaim, SuplierId, Top, KursTipe, KursVal, Keterangan, Year, Month);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null, dateJs = null;


        DateTime date = DateTime.Today;

        if (isAdd)
        {
          if (respon.Values != null)
          {
            if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
            {
              dateJs = Functional.DateToJson(date).ToString();
            }
            if (!string.IsNullOrEmpty(strStoreID))
            {
              ClaimID = respon.Values.GetValueParser<string>("CLAIM", string.Empty);

              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_claimno': '{1}',
                'd_claimdate': {2},
                'v_nama': '{3}',
                's_tahun': {4},
                't_bulan': '{5}'
              }}));{0}.commitChanges();", strStoreID,
                    ClaimID,
                    dateJs, strSuplDesc,
                    Year, Month);

              
            }
          }
        }

        if (string.IsNullOrEmpty(ClaimID))
        {
          ClaimID = numberId;
        }

        PopulateDetail("c_claimno", ClaimID);

        X.AddScript(scrpt);
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
