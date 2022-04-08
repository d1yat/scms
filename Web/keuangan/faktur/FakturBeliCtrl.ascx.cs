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
using System.Globalization;
using System.Data.Odbc;

public partial class keuangan_pembayaran_FakturBeliCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    hfFaktur.Clear();

    txFakturHdr.Clear();
    txFakturHdr.Disabled = false;

    txTanggalHdr.Clear();
    txTanggalHdr.Disabled = false;

    cbPemasokHdr.Clear();
    cbPemasokHdr.Disabled = false;

    cbNoDOHdr.Clear();
    cbNoDOHdr.Disabled = false;

    txTaxNoHdr.Clear();
    txTaxNoHdr.Disabled = false;
    txTaxDateHdr.Clear();
    txTaxDateHdr.Disabled = false;
    
    //cbKursHdr.Clear();
    cbKursHdr.Disabled = false;
    //txKursValueHdr.Clear();
    txKursValueHdr.Disabled = false;
    Functional.SetComboData(cbKursHdr, "c_kurs", "Rupiah", "01");
    txKursValueHdr.Text = "1";

    txExtDiscHdr.Clear();
    //txExtDiscHdr.Disabled = true;
    //lbExtDiscHdr.Text = string.Empty;    
    lbExtDiscHdr.Disabled = true;

    txTopHdr.Clear();
    txTopHdr.Disabled = false;
    lbDateTopHdr.Text = DateTime.Now.ToString("dd-MM-yyyy");
    txTopHdr.Text = "0";

    txTopPjgHdr.Clear();
    txTopPjgHdr.Disabled = false;
    lbDateTopPjgHdr.Text = DateTime.Now.ToString("dd-MM-yyyy");
    txTopPjgHdr.Text = "0";

    txKeteranganHdr.Clear();
    txKeteranganHdr.Disabled = false;

    txTotalFaktur.Clear();
    txTotalFaktur.Disabled = false;

    lbGrossBtm.Text = "";
    lbTaxBtm.Text = "";

    lbDiscBtm.Text = "";
    lbNetBtm.Text = "";

    txNPph22.Text = "0";
    lbNPph22.Text = "0,00";

    hfGdg.Clear();
    
    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntryGridDtlBea.ClientID));

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }

    store = gridDetailBea.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string numberID, string gudang, Dictionary<string, string>[] dics, Dictionary<string, string>[] dicsBea)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.TagExtraName = "FieldBea";
    pair.Value = numberID;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicValuesExtra = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    decimal jml = 0,
      bea = 0,
      disc = 0,
      price = 0,
      ppph = 0;
      //discExtra = 0;
    string tmp = null,
      itemId = null,
      itemType = null,
      varData = null,
      ket = null;
    bool isNew = false,
      isModify = false,
      isVoid = false;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);

    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);

    pair.DicAttributeValues.Add("Faktur", txFakturHdr.Text);

    if (!Functional.DateParser(txTanggalHdr.RawText, "dd-MM-yyyy", out date))
    {
      date = DateTime.MinValue;
    }
    pair.DicAttributeValues.Add("FakturDate", date.ToString("yyyyMMddHHmmssfff"));
        
    pair.DicAttributeValues.Add("Suplier", cbPemasokHdr.SelectedItem.Value);

    pair.DicAttributeValues.Add("NoReceive", cbNoDOHdr.SelectedItem.Value);

    pair.DicAttributeValues.Add("TaxNo", txTaxNoHdr.Text);

    if (!Functional.DateParser(txTaxDateHdr.RawText, "dd-MM-yyyy", out date))
    {
      date = DateTime.MinValue;
    }
    pair.DicAttributeValues.Add("TaxDate", date.ToString("yyyyMMddHHmmssfff"));

    pair.DicAttributeValues.Add("Kurs", cbKursHdr.SelectedItem.Value);
    pair.DicAttributeValues.Add("KursValue", txKursValueHdr.Text);

    pair.DicAttributeValues.Add("ExtraDiscount", txExtDiscHdr.Text);
    pair.DicAttributeValues.Add("ExtraDiscountVal", lbExtDiscHdr.Text);
    
    pair.DicAttributeValues.Add("Top", txTopHdr.Text);
    pair.DicAttributeValues.Add("TopPjg", txTopPjgHdr.Text);

    pair.DicAttributeValues.Add("ValueFaktur", txTotalFaktur.Text);

    pair.DicAttributeValues.Add("Keterangan", txKeteranganHdr.Text);

    pair.DicAttributeValues.Add("n_ppph", txNPph22.Text);
    pair.DicAttributeValues.Add("x_pph", lbNPph22.Text);

    pair.DicAttributeValues.Add("Gudang", (string.IsNullOrEmpty(gudang) ? "1" : gudang.Trim()));

    int nLoop = 0,
      nLen = 0;

    #region Grid Details

    for (nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      isNew = dicData.GetValueParser<bool>("l_new");
      isModify = dicData.GetValueParser<bool>("l_modified");
      isVoid = dicData.GetValueParser<bool>("l_void");

      if (isNew && (!isVoid) && (!isModify))
      {
        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        itemId = dicData.GetValueParser<string>("c_iteno");
        itemType = dicData.GetValueParser<string>("c_type");
        jml = dicData.GetValueParser<decimal>("n_qty", 0);
        disc = dicData.GetValueParser<decimal>("n_disc", 0);
        bea = dicData.GetValueParser<decimal>("n_bea", 0);
        price = dicData.GetValueParser<decimal>("n_salpri", 0);
        ppph = dicData.GetValueParser<decimal>("n_ppph", 0);
        //discExtra = dicData.GetValueParser<decimal>("n_discextra", 0);

        if (!string.IsNullOrEmpty(itemId))
        {
          dicAttr.Add("Item", itemId);
          dicAttr.Add("Type", itemType);
          dicAttr.Add("Qty", jml.ToString());
          dicAttr.Add("Disc", disc.ToString());
          dicAttr.Add("Price", price.ToString());
          dicAttr.Add("Bea", bea.ToString());
          dicAttr.Add("n_ppph", ppph.ToString());
          //dicAttr.Add("n_discextra", discExtra.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr
          });
        }
      }
      else if (isModify && (!isVoid) && (!isNew))
      {
        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        itemId = dicData.GetValueParser<string>("c_iteno");
        jml = dicData.GetValueParser<decimal>("n_qty", 0);
        disc = dicData.GetValueParser<decimal>("n_disc", 0);
        bea = dicData.GetValueParser<decimal>("n_bea", 0);
        price = dicData.GetValueParser<decimal>("n_salpri", 0);
        ppph = dicData.GetValueParser<decimal>("n_ppph", 0);
        itemType = dicData.GetValueParser<string>("c_type");
        //discExtra = dicData.GetValueParser<decimal>("n_discextra", 0);

        if (!string.IsNullOrEmpty(itemId))
        {
          dicAttr.Add("Item", itemId);
          dicAttr.Add("Qty", jml.ToString());
          dicAttr.Add("Disc", disc.ToString());
          dicAttr.Add("Price", price.ToString());
          dicAttr.Add("Bea", bea.ToString());
          dicAttr.Add("n_ppph", ppph.ToString());
          dicAttr.Add("Type", itemType);
          //dicAttr.Add("n_discextra", discExtra.ToString());
          
          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr
          });
        }
      }
      else if ((!isModify) && isVoid && (!isNew))
      {
        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        itemId = dicData.GetValueParser<string>("c_iteno");
        jml = dicData.GetValueParser<decimal>("n_qty", 0);
        disc = dicData.GetValueParser<decimal>("n_disc", 0);
        bea = dicData.GetValueParser<decimal>("n_bea", 0);
        price = dicData.GetValueParser<decimal>("n_salpri", 0);
        ppph = dicData.GetValueParser<decimal>("n_ppph", 0);
        itemType = dicData.GetValueParser<string>("c_type");
        //discExtra = dicData.GetValueParser<decimal>("n_discextra", 0);

        if (!string.IsNullOrEmpty(itemId))
        {
          dicAttr.Add("Item", itemId);
          dicAttr.Add("Qty", jml.ToString());
          dicAttr.Add("Disc", disc.ToString());
          dicAttr.Add("Price", price.ToString());
          dicAttr.Add("Bea", bea.ToString());
          dicAttr.Add("n_ppph", ppph.ToString());
          dicAttr.Add("Type", itemType);
          //dicAttr.Add("n_discextra", discExtra.ToString());

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

    #endregion

    #region Bea Details
    
    for (nLoop = 0, nLen = dicsBea.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dicsBea[nLoop];

      dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      isNew = dicData.GetValueParser<bool>("l_new");
      isModify = dicData.GetValueParser<bool>("l_modified");
      isVoid = dicData.GetValueParser<bool>("l_void");

      if (isNew && (!isVoid) && (!isModify))
      {
        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        itemType = dicData.GetValueParser<string>("c_type");
        itemId = dicData.GetValueParser<string>("c_exp", "00");

        if (!Functional.DateParser(dicData.GetValueParser<string>("d_top"), "yyyy-MM-ddTHH:mm:ss", out date))
        {
          date = DateTime.MinValue;
        }

        jml = dicData.GetValueParser<decimal>("n_value", 0);

        if ((!string.IsNullOrEmpty(itemType)) || (!string.IsNullOrEmpty(itemId)))
        {
          dicAttr.Add("Tipe", itemType);
          dicAttr.Add("Expeditur", itemId);
          dicAttr.Add("Tanggal", date.ToString("yyyyMMddHHmmssfff"));
          dicAttr.Add("Value", jml.ToString());

          pair.DicValuesExtra.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr
          });
        }
      }
      else if (isModify && (!isVoid) && (!isNew))
      {
        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        itemType = dicData.GetValueParser<string>("c_type");
        itemId = dicData.GetValueParser<string>("c_exp", "00");

        if (!Functional.DateParser(dicData.GetValueParser<string>("d_top"), "dd-MM-yyyyTHH:mm:ss", out date))
        {
          date = DateTime.MinValue;
        }

        jml = dicData.GetValueParser<decimal>("n_value", 0);

        if ((!string.IsNullOrEmpty(itemType)) || (!string.IsNullOrEmpty(itemId)))
        {
          dicAttr.Add("Tipe", itemType);
          dicAttr.Add("Expeditur", itemId);
          dicAttr.Add("Tanggal", date.ToString("yyyyMMddHHmmssfff"));
          dicAttr.Add("Value", jml.ToString());

          pair.DicValuesExtra.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr
          });
        }
      }
      else if ((!isModify) && isVoid && (!isNew))
      {
        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        itemType = dicData.GetValueParser<string>("c_type");
        itemId = dicData.GetValueParser<string>("c_exp", "00");
        ket = dicData.GetValueParser<string>("v_ket");

        if (!string.IsNullOrEmpty(itemId))
        {
          dicAttr.Add("Tipe", itemType);
          dicAttr.Add("Expeditur", itemId);

          pair.DicValuesExtra.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
            DicAttributeValues = dicAttr
          });
        }
      }

      dicData.Clear();
    }

    #endregion

    try
    {
      varData = parser.ParserData("FakturBeli", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("keuangan_pembayaran_FakturBeliCtrl SaveParser : {0} ", ex.Message);
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

  private void PopulateDetail(string pName, string pID)
  {
    ClearEntrys();

    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;
    string kursId = null,
      fakturId = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();
    //Indra 20181219FM 
    string[][] paramX = new string[][]{
        new string[] { "c_fbno = @0", pID, "System.String"},
        new string[] { "GetData", "1", "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string tmp = null;
    DateTime date = DateTime.Today;

    char gdg = char.MinValue;

    CultureInfo culture = new CultureInfo("id-ID");

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0102", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Concat("Faktur Beli - ", pID);

        fakturId = dicResultInfo.GetValueParser<string>("c_fbno", string.Empty);

        txFakturHdr.Text = dicResultInfo.GetValueParser<string>("c_fb");
        //txFakturHdr.Disabled = true;

        date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_fbdate"));
        txTanggalHdr.Text = date.ToString("dd-MM-yyyy");
        //txTanggalHdr.Disabled = true;

        Functional.SetComboData(cbPemasokHdr, "c_nosup", dicResultInfo.GetValueParser<string>("v_nama_supl", string.Empty), dicResultInfo.GetValueParser<string>("c_nosup", string.Empty));
        cbPemasokHdr.Disabled = true;

        tmp = dicResultInfo.GetValueParser<string>("c_rnno", string.Empty);
        Functional.SetComboData(cbNoDOHdr, "c_rnno", dicResultInfo.GetValueParser<string>("c_dono", string.Empty), tmp);
        cbNoDOHdr.Disabled = true;

        txTaxNoHdr.Text = dicResultInfo.GetValueParser<string>("c_taxno", string.Empty);
        date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_taxdate"));
        txTaxDateHdr.Text = date.ToString("dd-MM-yyyy");

        kursId = dicResultInfo.GetValueParser<string>("c_kurs", string.Empty);
        Functional.SetComboData(cbKursHdr, "c_kurs", dicResultInfo.GetValueParser<string>("v_desc_kurs", string.Empty), kursId);
        txKursValueHdr.Text = dicResultInfo.GetValueParser<decimal>("n_kurs").ToString();

        txTopHdr.Text = dicResultInfo.GetValueParser<decimal>("n_top", 0).ToString();
        date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_top"));
        lbDateTopHdr.Text = date.ToString("dd-MM-yyyy");
        
        txTopPjgHdr.Text = dicResultInfo.GetValueParser<decimal>("n_toppjg", 0).ToString();
        date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_toppjg"));
        lbDateTopPjgHdr.Text = date.ToString("dd-MM-yyyy");

        txExtDiscHdr.Text = dicResultInfo.GetValueParser<decimal>("n_pdisc", 0).ToString();
        lbExtDiscHdr.Text = dicResultInfo.GetValueParser<decimal>("n_xdisc").ToString("N2", culture);

        txNPph22.Text = dicResultInfo.GetValueParser<decimal>("n_ppph", 0).ToString();
        lbNPph22.Text = dicResultInfo.GetValueParser<decimal>("n_xpph", 0).ToString("N2", culture);

        lbGrossBtm.Text = dicResultInfo.GetValueParser<decimal>("n_bruto").ToString("N2", culture);
        lbTaxBtm.Text = dicResultInfo.GetValueParser<decimal>("n_ppn").ToString("N2", culture);

        lbDiscBtm.Text = dicResultInfo.GetValueParser<decimal>("n_disc").ToString("N2", culture);
        lbNetBtm.Text = dicResultInfo.GetValueParser<decimal>("n_bilva").ToString("N2", culture);

        txKeteranganHdr.Text = dicResultInfo.GetValueParser<string>("v_ket", string.Empty);

        gdg = dicResultInfo.GetValueParser<char>("c_gdg", '1');
        hfGdg.Text = (gdg.Equals(char.MinValue) ? "1"  : gdg.ToString());
        hfFaktur.Text = fakturId;

        txTotalFaktur.Text = dicResultInfo.GetValueParser<decimal>("n_bilva_faktur").ToString();

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("keuangan_pembayaran_FakturBeliCtrl:PopulateDetail Header - ", ex.Message));
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
              string.Format("[['fakturNo', '{0}', 'System.String']]", fakturId)
              , ParameterMode.Raw));
          }
          else
          {
            store.BaseParams["parameters"] = string.Format("[['fakturNo', '{0}', 'System.String']]", fakturId);
          }
        }
      }

      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("keuangan_pembayaran_FakturBeliCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion
    
    #region Parser Detail Bea

    try
    {
      Ext.Net.Store store = gridDetailBea.GetStore();
      if (store.Proxy.Count > 0)
      {
        Ext.Net.ScriptTagProxy stp = store.Proxy[0] as Ext.Net.ScriptTagProxy;
        if ((stp != null) && stp.CallbackParam.Equals(Functional.NAME_SOA_SCMS_CALLBACK, StringComparison.OrdinalIgnoreCase))
        {
          string param = (store.BaseParams["parameters"] ?? string.Empty);
          if (string.IsNullOrEmpty(param))
          {
            store.BaseParams.Add(new Ext.Net.Parameter("parameters",
              string.Format("[['fakturNo', '{0}', 'System.String']]", fakturId)
              , ParameterMode.Raw));
          }
          else
          {
            store.BaseParams["parameters"] = string.Format("[['fakturNo', '{0}', 'System.String']]", fakturId);
          }
        }
      }

      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("keuangan_pembayaran_FakturBeliCtrl:PopulateDetail DetailBea - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  #endregion

  public void Initialize(string storeIDGridMain)
  {
    hfStoreID.Text = storeIDGridMain;
  }

  public void CommandPopulate(bool isAdd, string pID)
  {
    hfGdg.Text = "1";

    if (isAdd)
    {
      ClearEntrys();

      txTotalFaktur.Text = "0";

      txExtDiscHdr.Text = "0";

      txTanggalHdr.Text = DateTime.Now.ToString("dd-MM-yyyy");

      winDetail.Title = "Faktur Beli";

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      PopulateDetail("c_fbno", pID);
    }
  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string jsonGridValuesBea = (e.ExtraParams["gridValuesBea"] ?? string.Empty);
    //string tanggalNote = (e.ExtraParams["TanggalNote"] ?? string.Empty);
    //string jenisBayar = (e.ExtraParams["JenisBayar"] ?? string.Empty);
    //string customerId = (e.ExtraParams["CustomerID"] ?? string.Empty);
    //string suplierName = (e.ExtraParams["CustomerName"] ?? string.Empty);
    //string tipeBayar = (e.ExtraParams["TipeBayar"] ?? string.Empty);
    //string bankID = (e.ExtraParams["BankID"] ?? string.Empty);
    //string rekNo = (e.ExtraParams["RekNo"] ?? string.Empty);
    //string giroID = (e.ExtraParams["GiroID"] ?? string.Empty);
    //string giroDate = (e.ExtraParams["GiroDate"] ?? string.Empty);
    //string kursID = (e.ExtraParams["KursID"] ?? string.Empty);
    //string kursValue = (e.ExtraParams["KursValue"] ?? string.Empty);
    //string jumlahTransaksi = (e.ExtraParams["JumlahTransaksi"] ?? string.Empty);
    //string sisaTransaksi = (e.ExtraParams["SisaTransaksi"] ?? string.Empty);
    //string ketHeader = (e.ExtraParams["Keterangan"] ?? string.Empty);
    string gdg = (e.ExtraParams["Gudang"] ?? string.Empty);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    Dictionary<string, string>[] gridData = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);
    Dictionary<string, string>[] gridDataBea = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValuesBea);
    
    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gdg, gridData, gridDataBea);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;
        string storeId = hfStoreID.Text;

        DateTime date = DateTime.Today;
        
        if (respon.Values != null)
        {
          if (!string.IsNullOrEmpty(storeId))
          {
            if (isAdd)
            {
              string dateJs = null;

              if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
              {
                dateJs = Functional.DateToJson(date);
              }

              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                                                        'c_fb': '{1}',
                                                        'c_fbno': '{2}',
                                                        'd_fbdate': {3},
                                                        'c_nosup': '{4}',
                                                        'v_nama_supl': '{5}',
                                                        'n_bilva': {6},
                                                        'n_sisa': {6},
                                                        'c_rnno': '{7}',
                                                        'v_ket': '{8}',
                                                        'c_dono': '{9}'
                                        }}));{0}.commitChanges();", storeId,
                                        txFakturHdr.Text,
                                        respon.Values.GetValueParser<string>("Faktur", string.Empty),
                                        dateJs,
                                        cbPemasokHdr.SelectedItem.Value,
                                        cbPemasokHdr.SelectedItem.Text,
                                        respon.Values.GetValueParser<decimal>("Net", -1),
                                        cbNoDOHdr.SelectedItem.Value,
                                        txKeteranganHdr.Text,
                                        cbNoDOHdr.SelectedItem.Text);
            }
            else
            {
              scrpt = string.Format(@"var idx = {0}.findExact('c_fbno', '{1}');
                      if(idx != -1) {{
                        var r = {0}.getAt(idx);
                        r.set('n_bilva', {2});
                        r.set('n_sisa', {3});
                        {0}.commitChanges();
                      }}", storeId, numberId,
                          respon.Values.GetValueParser<decimal>("Net", -1),
                          respon.Values.GetValueParser<decimal>("Sisa", -1));
            }

            X.AddScript(scrpt);
          }
        }

        //this.ClearEntrys();

        PopulateDetail("c_fbno", respon.Values.GetValueParser<string>("Faktur", string.Empty));

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

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string gdgId = pag.ActiveGudang;
    string custId = (e.ExtraParams["CustomeID"] ?? string.Empty);
    string pl1 = (e.ExtraParams["PLID1"] ?? string.Empty);
    string pl2 = (e.ExtraParams["PLID2"] ?? string.Empty);
    string tmp = (e.ExtraParams["Async"] ?? string.Empty);

    if (string.IsNullOrEmpty(gdgId) && string.IsNullOrEmpty(custId) &&
      string.IsNullOrEmpty(pl1) && string.IsNullOrEmpty(pl2))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");
      return;
    }

    ReportParser rptParse = new ReportParser();
    
    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();
    bool isAsync = false;

    bool.TryParse(tmp, out isAsync);

    rptParse.ReportingID = "10101";
    
    #region Report Parameter
    
    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_PLH.c_gdg",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_PLH.c_cusno",
      ParameterValue = (string.IsNullOrEmpty(custId) ? string.Empty : custId)
    });

    if (!string.IsNullOrEmpty(pl1))
    {
      if (string.IsNullOrEmpty(pl2))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_PLH.c_plno",
          ParameterValue = (string.IsNullOrEmpty(pl1) ? string.Empty : pl1)
        });
      }
      else
      {
        if (pl1.CompareTo(pl2) <= 0)
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "LG_PLH.c_plno",
            ParameterValue = (string.IsNullOrEmpty(pl1) ? string.Empty : pl1)

          });
        }
        else
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{LG_PLH.c_plno}} IN ({0} TO {1}))", pl1, pl2),
            IsReportDirectValue = true
          });
        }
      }
    }

    #endregion

    rptParse.PaperID = "8.5x5.5";
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;

    string xmlFiles = ReportParser.Deserialize(rptParse);

    SoaReportCaller soa = new SoaReportCaller();

    //string result = soa.GeneratorReport(isAsync, xmlFiles);
  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void TaxDateHdr_Change(object sender, DirectEventArgs e)
  {
      string handler = "PPN";
      string tglBerlaku = txTaxDateHdr.RawText.Trim();
      
      if (string.IsNullOrEmpty(tglBerlaku))
      {
          return;
      }

      DateTime date = DateTime.Now;
      if (!Functional.DateParser(tglBerlaku, "dd-MM-yyyy", out date))
      {
          date = DateTime.MinValue;
      }
      string[][] paramX = new string[][] {
        new string[] { "handler", handler, "System.String" },
        new string[] { "tglBerlaku", date.ToString("yyyy-MM-dd"), "System.String" }
      };

      Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();
      string res2 = soa.GlobalQueryService(0, -1, true, string.Empty, string.Empty, "2197", paramX);
      try
      {

          Dictionary<string, object> dicResult2 = JSON.Deserialize<Dictionary<string, object>>(res2);
          List<Dictionary<string, object>> lstResultInfo = new List<Dictionary<string,object>>();

          if (dicResult2.ContainsKey("records") && (dicResult2.ContainsKey("totalRows") && (((long)dicResult2["totalRows"]) > 0)))
          {
              Newtonsoft.Json.Linq.JArray jarr2 = new Newtonsoft.Json.Linq.JArray(dicResult2["records"]);
              lstResultInfo = JSON.Deserialize<List<Dictionary<string, object>>>(jarr2.First.ToString());
              this.hfTax.Text = lstResultInfo[0].GetValueParser<string>("value", string.Empty);
          }
      }
      catch (Exception)
      {

          throw;
      }
  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void NoDOHdr_Change(object sender, DirectEventArgs e)
  {
    string noSupl = (e.ExtraParams["NoSupl"] ?? string.Empty);
    string noDO = (e.ExtraParams["NoDO"] ?? string.Empty);
    string gdg = (e.ExtraParams["Gudang"] ?? string.Empty);

    if (string.IsNullOrEmpty(noSupl) || string.IsNullOrEmpty(noDO) || string.IsNullOrEmpty(gdg))
    {
      return;
    }

    Dictionary<string, object> dicResult = null;
    List<Dictionary<string, object>> lstResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;
    //string kursId = null,
    //  fakturId = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "suplierId", noSupl, "System.String"},
        new string[] { "receiveId", noDO, "System.String"},
        new string[] { "gdg", gdg, "System.Char"},
        new string[] { "isFloating", "false", "System.Boolean"},
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string tmp = null;
    DateTime date = DateTime.Today;

    CultureInfo culture = new CultureInfo("id-ID");

    string res = soa.GlobalQueryService(0, -1, true, string.Empty, string.Empty, "14011", paramX);

    StringBuilder sb = new StringBuilder();

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
          //paramX = new string[][] {
          //    new string[] { "gdg", gdg, "System.Char"},
          //    new string[] { "receiveId", noDO, "System.String"}
          //};
          
          //res = soa.GlobalQueryService(0, -1, true, string.Empty, string.Empty, "2196", paramX);
          //var dicResult2 = JSON.Deserialize<Dictionary<string, object>>(res);
          
          jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);
          //var jarr2 = new Newtonsoft.Json.Linq.JArray(dicResult2["records"]);

          lstResultInfo = JSON.Deserialize<List<Dictionary<string, object>>>(jarr.First.ToString());
          //var lstResultInfo2 = JSON.Deserialize<List<Dictionary<string, object>>>(jarr2.First.ToString());

          //string tanggalDO = lstResultInfo2[0].GetValueParser<string>("d_dodate", string.Empty);
          //tanggalDO = @"""" + tanggalDO + @"""";
          //tanggalDO = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(tanggalDO);
          
          dicResult.Clear();
          //dicResult2.Clear();

        tmp = gridDetail.GetStore().ClientID;

        sb.AppendFormat("{0}.removeAll();", tmp);

        for (int nLoop = 0, nLen = lstResultInfo.Count; nLoop < nLen; nLoop++)
        {
          dicResult = lstResultInfo[nLoop];

          //string itemNo = dicResult.GetValueParser<string>("c_iteno", string.Empty);
          //string tglBerlaku = !string.IsNullOrEmpty(tanggalDO) ? DateTime.Parse(tanggalDO).ToString("yyyy-MM-dd") : string.Empty;
          //decimal d = decimal.Zero;
          //var connectionStringSql = "Driver={SQL Server};Server=10.100.41.29;Database=AMS;Uid=sa;Pwd=4M5M1s2015";
          //using (OdbcConnection con = new OdbcConnection(connectionStringSql))
          //{
          //    con.Open();
          //    OdbcCommand cmd = new OdbcCommand();
          //    cmd.Connection = con;
          //    string sSql = "select * from openquery(dcoreprod,'SELECT N_DISCEXTRA FROM `tbl_discountextra_periode` WHERE D_BERLAKU <= ''" + tglBerlaku + "'' AND c_iteno=''" + itemNo + "'' ORDER BY N_ID DESC LIMIT 1;')";
          //    cmd.CommandText = sSql;
          //    OdbcDataReader reader = cmd.ExecuteReader();
          //    while (reader.Read())
          //    {
          //        d = decimal.Parse(reader["N_DISCEXTRA"].ToString());
          //    }
          //    cmd.Dispose();
          //}

        //'n_discextra': {10},
          sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                  'c_iteno': '{1}',
                  'v_itnam': '{2}',
                  'c_type': '{3}',
                  'v_type_desc': '{4}',
                  'n_bea': {5},
                  'n_disc': {6},
                  'n_qty': {7},
                  'n_salpri': {8},
                  'l_new': true,
                  'l_modified': false,
                  'l_void': false,
                  'v_ket': '',
                  'n_ppph': {9},
                  'l_pph': false,
                }}));", tmp,
                      dicResult.GetValueParser<string>("c_iteno", string.Empty),
                      dicResult.GetValueParser<string>("v_itnam", string.Empty),
                      dicResult.GetValueParser<string>("c_type", string.Empty),
                      dicResult.GetValueParser<string>("v_type_desc", string.Empty),
                      dicResult.GetValueParser<decimal>("n_bea", 0),
                      dicResult.GetValueParser<decimal>("n_disc", 0),
                      dicResult.GetValueParser<decimal>("n_qty", 0),
                      dicResult.GetValueParser<decimal>("n_salpri", 0),
                      dicResult.GetValueParser<decimal>("n_ppph", 0));
                      //d.ToString() ?? "0");

          dicResult.Clear();
        }

        sb.AppendFormat("{0}.commitChanges();", tmp);

        sb.AppendFormat("recalculateFaktur({0});", tmp);

        X.AddScript(sb.ToString());

        jarr.Clear();
        //jarr2.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("keuangan_pembayaran_FakturBeliCtrl:NoDOHdr_Change PopulateRNItems - ", ex.Message));
    }
    finally
    {
      if (jarr != null)
      {
        jarr.Clear();
      }
      if (lstResultInfo != null)
      {
        lstResultInfo.Clear();
      }
      if (dicResult != null)
      {
        dicResult.Clear();
      }
    }
  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void onchange(object sender, DirectEventArgs e)
  {
    string noSupl = (e.ExtraParams["NoSupl"] ?? string.Empty);
    string noDO = (e.ExtraParams["NoDO"] ?? string.Empty);

    System.Text.StringBuilder sb = new System.Text.StringBuilder();

    //Ext.Net.SelectionModelCollection = gridDetail.GetSelectionModel();

    var ss = gridDetail.GetSelectionModel();

    
  }
}