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

public partial class transaksi_InvoiceShipmentCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {

      DateTime date = DateTime.Now;

      cbEksHdr.Clear();
      cbEksHdr.Disabled = false;

      txFakturHdr.Clear();
      txFakturHdr.Disabled = false;

      txFisikFaktur.Clear();
      txFisikFaktur.Disabled = false;

      txTglFaktur.Clear();
      txTglFaktur.Text = date.ToString("dd-MM-yyyy");
      txTglFaktur.Disabled = false;

      txTOP.Clear();
      txTOP.Disabled = false;

      txPajak.Clear();
      txPajak.Text = "0";
      txPajak.Disabled = false;

      txKet.Clear();
      txKet.Disabled = false;

      txKM.Clear();
      txKM.Disabled = false;

      txMaterai.Clear();
      txMaterai.Text = "0";
      txMaterai.Disabled = false;

      txPotongan.Clear();
      txPotongan.Text = "0";
      txPotongan.Disabled = false;

      txClaimNo.Clear();
      txClaimNo.Disabled = false;

      //rdBerat.Checked = true;
      cbTipeBiaya.SelectedIndex = 0;

      cbResi.Clear();
      cbResi.Disabled = false;
      Ext.Net.Store cbResiStr = cbResi.GetStore();
      if (cbResiStr != null)
      {
          cbResiStr.RemoveAll();
      }

      Ext.Net.Store store = gridDetail.GetStore();
      if (store != null)
      {
          store.RemoveAll();
      }

      lbGrossBtm.Text = "";
      lbDiscBtm.Text = "";
      lbTaxBtm.Text = "";
      lbNetBtm.Text = "";
      lbLainBtm.Text = "";

      hfGdg.Clear();
      hfFeno.Clear();
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string numberID, string gudang, Dictionary<string, string>[] dics, string gross, string totalPajak, string totalBiayaLain, string netBerat, string netVol)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    //pair.TagExtraName = "FieldBea";
    pair.Value = numberID;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicValuesExtra = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    //decimal jml = 0,
    //  bea = 0,
    //  disc = 0,
    //  price = 0,
    //  ppph = 0;
    //string tmp = null,
    //  itemId = null,
    //  itemType = null,
    //  varData = null,
    //  ket = null;

    double grossP = double.Parse(gross, CultureInfo.CreateSpecificCulture("id-ID"));
    double totalPajakP = double.Parse(totalPajak, CultureInfo.CreateSpecificCulture("id-ID"));
    double totalBiayaLainP = double.Parse(totalBiayaLain, CultureInfo.CreateSpecificCulture("id-ID"));
    double netBeratP = double.Parse(netBerat, CultureInfo.CreateSpecificCulture("id-ID"));
    double netVolP = double.Parse(netVol, CultureInfo.CreateSpecificCulture("id-ID"));

    decimal koli = 0,
        berat = 0,
        volume = 0,
        jumlah = 0,
        biayalain = 0,
        totalcost = 0,
        biaya = 0;
        

    string tmp = null,
        varData = null,
        ketVoid = null,
        resi = null,
        epno = null,
        cabang = null,
        tipebiaya = null,
        exptype = null,
        via = null;

    bool isNew = false,
      isModify = false,
      isVoid = false;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Gudang", (string.IsNullOrEmpty(gudang) ? "1" : gudang.Trim()));
    pair.DicAttributeValues.Add("Ekspedisi", cbEksHdr.SelectedItem.Value);
    pair.DicAttributeValues.Add("Faktur", txFakturHdr.Text);
    pair.DicAttributeValues.Add("FisikFaktur", txFisikFaktur.Text);
    if (!Functional.DateParser(txTglFaktur.RawText, "dd-MM-yyyy", out date))
    {
        date = DateTime.MinValue;
    }
    pair.DicAttributeValues.Add("FakturDate", date.ToString("yyyyMMddHHmmssfff"));
    pair.DicAttributeValues.Add("TOP", txTOP.Text);
    pair.DicAttributeValues.Add("Pajak", txPajak.Text);
    pair.DicAttributeValues.Add("Ket", txKet.Text);
    pair.DicAttributeValues.Add("KM", txKM.Text);
    pair.DicAttributeValues.Add("Materai", txMaterai.Text);
    pair.DicAttributeValues.Add("Potongan", txPotongan.Text);
    pair.DicAttributeValues.Add("ClaimNo", txClaimNo.Text);

    pair.DicAttributeValues.Add("Gross", grossP.ToString());
    pair.DicAttributeValues.Add("TotalTax", totalPajakP.ToString());
    pair.DicAttributeValues.Add("TotalBiayaLain", totalBiayaLainP.ToString());
    pair.DicAttributeValues.Add("NetBerat", netBeratP.ToString());
    pair.DicAttributeValues.Add("NetVol", netVolP.ToString());

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

        resi = dicData.GetValueParser<string>("c_resi");
        epno = dicData.GetValueParser<string>("c_expno");
        cabang = dicData.GetValueParser<string>("c_cusno");
        koli = dicData.GetValueParser<decimal>("n_koli", 0);
        berat = dicData.GetValueParser<decimal>("n_berat", 0);
        volume = dicData.GetValueParser<decimal>("n_vol", 0);
        biaya = dicData.GetValueParser<decimal>("n_biaya", 0);
        exptype = dicData.GetValueParser<string>("c_exptype");
        via = dicData.GetValueParser<string>("c_via");
        //tipebiaya = dicData.GetValueParser<string>("c_tipebiaya");
        biayalain = dicData.GetValueParser<decimal>("n_biayalain", 0);
        totalcost = dicData.GetValueParser<decimal>("n_totalcost", 0);

            if (!string.IsNullOrEmpty(resi))
        {
          dicAttr.Add("resiNo", resi);
          dicAttr.Add("epNo", epno);
          dicAttr.Add("Cusno", cabang);
          dicAttr.Add("Koli", koli.ToString());
          dicAttr.Add("Berat", berat.ToString());
          dicAttr.Add("Volume", volume.ToString());
          dicAttr.Add("Biaya", biaya.ToString());
          dicAttr.Add("Via", via.ToString());
          dicAttr.Add("expType", exptype.ToString());
          //dicAttr.Add("tipeBiaya", tipebiaya.ToString());
          dicAttr.Add("biayaLain", biayalain.ToString());
          dicAttr.Add("totalCost", totalcost.ToString());

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

        resi = dicData.GetValueParser<string>("c_resi");
        epno = dicData.GetValueParser<string>("c_expno");
        cabang = dicData.GetValueParser<string>("c_cusno");
        koli = dicData.GetValueParser<decimal>("n_koli", 0);
        berat = dicData.GetValueParser<decimal>("n_berat", 0);
        volume = dicData.GetValueParser<decimal>("n_vol", 0);
        biaya = dicData.GetValueParser<decimal>("n_biaya", 0);
        exptype = dicData.GetValueParser<string>("c_exptype");
        via = dicData.GetValueParser<string>("c_via");
        //tipebiaya = dicData.GetValueParser<string>("c_tipebiaya");
        biayalain = dicData.GetValueParser<decimal>("n_biayalain", 0);
        totalcost = dicData.GetValueParser<decimal>("n_totalcost", 0);

        if (!string.IsNullOrEmpty(resi))
        {
            dicAttr.Add("resiNo", resi);
            dicAttr.Add("epNo", epno);
            dicAttr.Add("Cusno", cabang);
            dicAttr.Add("Koli", koli.ToString());
            dicAttr.Add("Berat", berat.ToString());
            dicAttr.Add("Volume", volume.ToString());
            dicAttr.Add("Biaya", biaya.ToString());
            dicAttr.Add("Via", via.ToString());
            dicAttr.Add("expType", exptype.ToString());
            //dicAttr.Add("tipeBiaya", tipebiaya.ToString());
            dicAttr.Add("biayaLain", totalcost.ToString());
            dicAttr.Add("totalCost", totalcost.ToString());

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

        resi = dicData.GetValueParser<string>("c_resi");
        epno = dicData.GetValueParser<string>("c_expno");
        cabang = dicData.GetValueParser<string>("c_cusno");
        koli = dicData.GetValueParser<decimal>("n_koli", 0);
        berat = dicData.GetValueParser<decimal>("n_berat", 0);
        volume = dicData.GetValueParser<decimal>("n_vol", 0);
        biaya = dicData.GetValueParser<decimal>("n_biaya", 0);
        exptype = dicData.GetValueParser<string>("c_exptype");
        via = dicData.GetValueParser<string>("c_via");
        //tipebiaya = dicData.GetValueParser<string>("c_tipebiaya");
        biayalain = dicData.GetValueParser<decimal>("n_biayalain", 0);
        totalcost = dicData.GetValueParser<decimal>("n_totalcost", 0);

        if (!string.IsNullOrEmpty(resi))
        {
            dicAttr.Add("resiNo", resi);
            dicAttr.Add("epNo", epno);
            dicAttr.Add("Cusno", cabang);
            dicAttr.Add("Koli", koli.ToString());
            dicAttr.Add("Berat", berat.ToString());
            dicAttr.Add("Volume", volume.ToString());
            dicAttr.Add("Biaya", biaya.ToString());
            dicAttr.Add("Via", via.ToString());
            dicAttr.Add("expType", exptype.ToString());
            //dicAttr.Add("tipeBiaya", tipebiaya.ToString());
            dicAttr.Add("biayaLain", totalcost.ToString());
            dicAttr.Add("totalCost", totalcost.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ketVoid) ? "Human error" : ketVoid),
            DicAttributeValues = dicAttr
          });
        }
      }

      dicData.Clear();
    }

    #endregion

    try
    {
        varData = parser.ParserData("InvoiceShipment", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
        Scms.Web.Common.Logger.WriteLine("transaksi_InvoiceShipmentCtrl SaveParser : {0} ", ex.Message);
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

    string[][] paramX = new string[][]{
        new string[] { "c_feno = @0", pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string tmp = null;
    DateTime date = DateTime.Today;

    char gdg = char.MinValue;

    CultureInfo culture = new CultureInfo("id-ID");

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "04001", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Concat("Faktur Ekspedisi - ", pID);

        fakturId = dicResultInfo.GetValueParser<string>("c_feno", string.Empty);

        Functional.SetComboData(cbEksHdr, "c_exp", dicResultInfo.GetValueParser<string>("v_nama_exp", string.Empty), dicResultInfo.GetValueParser<string>("c_exp", string.Empty));
        cbEksHdr.Disabled = true;

        txFakturHdr.Text = dicResultInfo.GetValueParser<string>("c_fe");
        //txFakturHdr.Disabled = true;

        txFisikFaktur.Text = dicResultInfo.GetValueParser<string>("n_bilva_faktur").ToString();
        //txFisikFaktur.Disabled = true;

        date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_fedate"));
        txTglFaktur.Text = date.ToString("dd-MM-yyyy");
        //txTglFaktur.Disabled = true;

        txTOP.Text = dicResultInfo.GetValueParser<decimal>("n_top", 0).ToString();
        //txTOP.Disabled = true;

        txPajak.Text = dicResultInfo.GetValueParser<decimal>("n_tax", 0).ToString();
        //txPajak.Disabled = true;

        txKM.Text = dicResultInfo.GetValueParser<decimal>("n_kilometer", 0).ToString();
        //txKM.Disabled = true;

        txMaterai.Text = dicResultInfo.GetValueParser<decimal>("n_materai", 0).ToString();
        //txMaterai.Disabled = true;

        txPotongan.Text = dicResultInfo.GetValueParser<decimal>("n_disc", 0).ToString();
        //txPotongan.Disabled = true;

        txClaimNo.Text = dicResultInfo.GetValueParser<string>("c_claimno");
        //txClaimNo.Disabled = true;

        txKet.Text = dicResultInfo.GetValueParser<string>("v_ket");
        //txKet.Disabled = true;

        lbGrossBtm.Text = dicResultInfo.GetValueParser<decimal>("n_bruto").ToString("N2", culture);
        lbDiscBtm.Text = dicResultInfo.GetValueParser<decimal>("n_disc").ToString("N2", culture);
        lbTaxBtm.Text = dicResultInfo.GetValueParser<decimal>("n_totaltax").ToString("N2", culture);

        lbLainBtm.Text = dicResultInfo.GetValueParser<decimal>("n_totalbiayalain").ToString("N2", culture);
        lbMaterai.Text = dicResultInfo.GetValueParser<decimal>("n_materai").ToString("N2", culture);
        lbNetBtm.Text = dicResultInfo.GetValueParser<decimal>("n_net").ToString("N2", culture);

        lbNetVolBtm.Text = dicResultInfo.GetValueParser<decimal>("n_netvol").ToString("N2", culture);


        //lbDiscBtm.Text = dicResultInfo.GetValueParser<decimal>("n_disc").ToString("N2", culture);
        //lbNetBtm.Text = dicResultInfo.GetValueParser<decimal>("n_bilva").ToString("N2", culture);

        //lbGrossBtm.Text = dicResultInfo.GetValueParser<decimal>("n_bruto").ToString("N2", culture);
        //lbTaxBtm.Text = dicResultInfo.GetValueParser<decimal>("n_ppn").ToString("N2", culture);

        //lbDiscBtm.Text = dicResultInfo.GetValueParser<decimal>("n_disc").ToString("N2", culture);
        //lbNetBtm.Text = dicResultInfo.GetValueParser<decimal>("n_bilva").ToString("N2", culture);

        //date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_fbdate"));
        //txTanggalHdr.Text = date.ToString("dd-MM-yyyy");
        ////txTanggalHdr.Disabled = true;

        //Functional.SetComboData(cbPemasokHdr, "c_nosup", dicResultInfo.GetValueParser<string>("v_nama_supl", string.Empty), dicResultInfo.GetValueParser<string>("c_nosup", string.Empty));
        //cbPemasokHdr.Disabled = true;

        //tmp = dicResultInfo.GetValueParser<string>("c_rnno", string.Empty);
        //Functional.SetComboData(cbNoDOHdr, "c_rnno", dicResultInfo.GetValueParser<string>("c_dono", string.Empty), tmp);
        //cbNoDOHdr.Disabled = true;

        //txTaxNoHdr.Text = dicResultInfo.GetValueParser<string>("c_taxno", string.Empty);
        //date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_taxdate"));
        //txTaxDateHdr.Text = date.ToString("dd-MM-yyyy");

        //kursId = dicResultInfo.GetValueParser<string>("c_kurs", string.Empty);
        //Functional.SetComboData(cbKursHdr, "c_kurs", dicResultInfo.GetValueParser<string>("v_desc_kurs", string.Empty), kursId);
        //txKursValueHdr.Text = dicResultInfo.GetValueParser<decimal>("n_kurs").ToString();

        //txTopHdr.Text = dicResultInfo.GetValueParser<decimal>("n_top", 0).ToString();
        //date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_top"));
        //lbDateTopHdr.Text = date.ToString("dd-MM-yyyy");
        
        //txTopPjgHdr.Text = dicResultInfo.GetValueParser<decimal>("n_toppjg", 0).ToString();
        //date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_toppjg"));
        //lbDateTopPjgHdr.Text = date.ToString("dd-MM-yyyy");

        //txExtDiscHdr.Text = dicResultInfo.GetValueParser<decimal>("n_pdisc", 0).ToString();
        //lbExtDiscHdr.Text = dicResultInfo.GetValueParser<decimal>("n_xdisc").ToString("N2", culture);

        //txNPph22.Text = dicResultInfo.GetValueParser<decimal>("n_ppph", 0).ToString();
        //lbNPph22.Text = dicResultInfo.GetValueParser<decimal>("n_xpph", 0).ToString("N2", culture);

        //lbGrossBtm.Text = dicResultInfo.GetValueParser<decimal>("n_bruto").ToString("N2", culture);
        //lbTaxBtm.Text = dicResultInfo.GetValueParser<decimal>("n_ppn").ToString("N2", culture);

        //lbDiscBtm.Text = dicResultInfo.GetValueParser<decimal>("n_disc").ToString("N2", culture);
        //lbNetBtm.Text = dicResultInfo.GetValueParser<decimal>("n_bilva").ToString("N2", culture);

        //txKeteranganHdr.Text = dicResultInfo.GetValueParser<string>("v_ket", string.Empty);

        //gdg = dicResultInfo.GetValueParser<char>("c_gdg", '1');
        //hfGdg.Text = (gdg.Equals(char.MinValue) ? "1"  : gdg.ToString());
        //hfFaktur.Text = fakturId;

        //txTotalFaktur.Text = dicResultInfo.GetValueParser<decimal>("n_bilva_faktur").ToString();

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_InvoiceShipmentCtrl:PopulateDetail Header - ", ex.Message));
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
            store.BaseParams.Add(new Ext.Net.Parameter("parameters", string.Format("[['{0} = @0', '{1}', 'System.String']]", pName, pID), ParameterMode.Raw));
          }
          else
          {
              store.BaseParams["parameters"] = string.Format("[['{0} = @0', '{1}', 'System.String']]", pName, pID);
          }
        }
      }

      hfFeno.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_InvoiceShipmentCtrl:PopulateDetail Detail - ", ex.Message));
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

      //txTotalFaktur.Text = "0";

      //txExtDiscHdr.Text = "0";

      //txTanggalHdr.Text = DateTime.Now.ToString("dd-MM-yyyy");

      winDetail.Title = "Faktur Ekspedisi";

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      PopulateDetail("c_feno", pID);
    }
  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string gdg = (e.ExtraParams["Gudang"] ?? string.Empty);
    //string ekspedisi = (e.ExtraParams["Ekspedisi"] ?? string.Empty);
    //string faktur = (e.ExtraParams["Faktur"] ?? string.Empty);
    //string fisikfaktur = (e.ExtraParams["FisikFaktur"] ?? string.Empty);
    //string tgltop = (e.ExtraParams["TanggalTOP"] ?? string.Empty);
    //string top = (e.ExtraParams["TOP"] ?? string.Empty);
    //string tax = (e.ExtraParams["Pajak"] ?? string.Empty);
    //string ketheader = (e.ExtraParams["Ket"] ?? string.Empty);
    //string km = (e.ExtraParams["KM"] ?? string.Empty);
    string gross = (e.ExtraParams["Gross"] ?? string.Empty);
    string totalPajak = (e.ExtraParams["Pajak"] ?? string.Empty);
    string totalBiayaLain = (e.ExtraParams["TotalBiayaLain"] ?? string.Empty);
    string netBerat = (e.ExtraParams["NetBerat"] ?? string.Empty);
    string netVol = (e.ExtraParams["NetVol"] ?? string.Empty);



    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);
    //bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    Dictionary<string, string>[] gridData = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);
    //Dictionary<string, string>[] gridDataBea = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValuesBea);
    
    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gdg, gridData, gross, totalPajak, totalBiayaLain, netBerat, netVol);

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

              if (Functional.DateParser(respon.Values.GetValueParser<string>("FakturDate", string.Empty), "yyyyMMdd", out date))
              {
                dateJs = Functional.DateToJson(date);
              }

              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                                                        'c_feno': '{1}',
                                                        'c_fe': '{2}',
                                                        'd_fedate': {3},
                                                        'v_nama_exp': '{4}',
                                                        'n_bilva_faktur': {5},
                                                        'n_netvol': {6},
                                                        'v_ket': '{7}'
                                        }}));{0}.commitChanges();", storeId,
                                        respon.Values.GetValueParser<string>("feno"),
                                        txFakturHdr.Text,
                                        dateJs,
                                        cbEksHdr.SelectedItem.Text,
                                        respon.Values.GetValueParser<decimal>("FisikFaktur", -1),
                                        respon.Values.GetValueParser<decimal>("NetVol", -1),
                                        txKet.Text
                                        );
            }
            else
            {
              scrpt = string.Format(@"var idx = {0}.findExact('c_feno', '{1}');
                      if(idx != -1) {{
                        var r = {0}.getAt(idx);
                        r.set('n_bilva_faktur', {2});
                        r.set('n_netvol', {3});
                        r.set('v_ket', '{4}');
                        {0}.commitChanges();
                      }}", storeId,
                          respon.Values.GetValueParser<string>("feno"),
                          respon.Values.GetValueParser<decimal>("FisikFaktur", -1),
                          respon.Values.GetValueParser<decimal>("NetVol", -1),
                          txKet.Text
                          );
            }

            X.AddScript(scrpt);
          }
        }

        //this.ClearEntrys();

        PopulateDetail("c_feno", respon.Values.GetValueParser<string>("feno", string.Empty));

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
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        lstResultInfo = JSON.Deserialize<List<Dictionary<string, object>>>(jarr.First.ToString());

        dicResult.Clear();

        tmp = gridDetail.GetStore().ClientID;

        sb.AppendFormat("{0}.removeAll();", tmp);

        for (int nLoop = 0, nLen = lstResultInfo.Count; nLoop < nLen; nLoop++)
        {
          dicResult = lstResultInfo[nLoop];

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

          dicResult.Clear();
        }

        sb.AppendFormat("{0}.commitChanges();", tmp);

        sb.AppendFormat("recalculateFaktur({0});", tmp);

        X.AddScript(sb.ToString());

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_InvoiceShipmentCtrl:NoDOHdr_Change PopulateRNItems - ", ex.Message));
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

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void AddBtnResi_Click(object sebder, DirectEventArgs e)
  {
      string exp = e.ExtraParams["exp"].ToString();
      string resi = e.ExtraParams["resi"].ToString();
      string gdg = "1";
      string jsonGridValues = (e.ExtraParams["Grid"] ?? string.Empty);
      bool isNotDouble = false;


      if (!string.IsNullOrEmpty(resi))
      {
          isNotDouble = jsonGridValues.Contains(resi);
      }

      if (!isNotDouble)
      {
          InsertDetail(exp, resi, gdg, true, jsonGridValues);
      }
      else
      {
          Functional.ShowMsgInformation("No. " + resi + ", Data telah ada.");
      }
      cbResi.Clear();
  }

  private void InsertDetail(string exp, string resi, string gdg, bool add, string gridValues)
  {
      Dictionary<string, object> dicEXP = null;

      Dictionary<string, string> dicEXPInfo = null;
      Newtonsoft.Json.Linq.JArray jarr = null;
      System.Text.StringBuilder sb = new System.Text.StringBuilder();
      Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();


      string tmp = null;
      tmp = gridDetail.GetStore().ClientID;

      string sResi = null,
       sExpno = null,
       sCunam = null,
       sCusno = null,
       sViaKet = null,
       sVia = null,
       sExptype = null,
       sExptypeKet = null;

      decimal nKoli = 0,
      nBerat = 0,
      nVolume = 0,
      nJumlah = 0,
      nBiayaLain = 0,
      nBiayaKirim = 0,
      divider = 0.5m,
      costBerat = 0,
      totalCostBerat = 0,
      beratRound = 0,
      totalCostVolume = 0,
      volumeRound = 0,
      sumTaxBerat = 0,
      sumTaxVolume = 0,
      Tax = Int16.Parse(txPajak.Text),
      Potongan = Int64.Parse(txPotongan.Text),
      bruto = 0,
      nDaratLaut = 0,
      nUdara = 0,
      nIcePack = 0,
      nCDD = 0,
      nTronton = 0,
      nFuso = 0;


      string typebiaya = null;

      typebiaya = cbTipeBiaya.SelectedItem.Value == null ? "01" : cbTipeBiaya.SelectedItem.Value;

      string[][] paramX = new string[][]{
              new string[] { string.Format("{0}", "c_exp"), exp, "System.String"},
              new string[] { string.Format("{0}", "c_resi"), resi, "System.String"},
              new string[] { string.Format("{0}", "c_gdg"), gdg, "System.String"},
              new string[] { string.Format("{0}", "tipebiaya"), typebiaya, "System.String"}
            };

      string res = soa.GlobalQueryService(0, -1, false, string.Empty, string.Empty, "0312", paramX);
      dicEXP = JSON.Deserialize<Dictionary<string, object>>(res);


      #region Parser Detail

      try
      {
          dicEXP = JSON.Deserialize<Dictionary<string, object>>(res);

          foreach (KeyValuePair<string, object> kvp in dicEXP)
          {
              if (kvp.Key == "records")
              {
                  object s = kvp.Value;

                  Dictionary<string, string>[] dicEXP1 = JSON.Deserialize<Dictionary<string, string>[]>(s.ToString());

                  foreach (Dictionary<string, string> dics in dicEXP1)
                  {
                      foreach (KeyValuePair<string, string> kvps in dics)
                      {
                          if (kvps.Key.Equals("c_resi"))
                          {
                              sResi = (string)kvps.Value;
                          }

                          if (kvps.Key.Equals("c_expno"))
                          {
                              sExpno = (string)kvps.Value;
                          }

                          if (kvps.Key.Equals("v_cunam"))
                          {
                              sCunam = (string)kvps.Value;
                          }

                          if (kvps.Key.Equals("c_cusno"))
                          {
                             sCusno  = (string)kvps.Value;
                          }

                          if (kvps.Key.Equals("n_koli"))
                          {
                              nKoli = decimal.Parse(kvps.Value);
                          }

                          if (kvps.Key.Equals("n_berat"))
                          {
                              nBerat = decimal.Parse(kvps.Value);
                          }

                          if (kvps.Key.Equals("n_vol"))
                          {
                              nVolume = decimal.Parse(kvps.Value);
                          }

                          if (kvps.Key.Equals("n_biayalain"))
                          {
                              nBiayaLain = decimal.Parse(kvps.Value);
                          }

                          if (kvps.Key.Equals("c_exptype"))
                          {
                              sExptype = (string)kvps.Value;
                          }

                          if (kvps.Key.Equals("v_ket_exptype"))
                          {
                              sExptypeKet = (string)kvps.Value;
                          }

                          if (kvps.Key.Equals("n_udara"))
                          {
                              nUdara = decimal.Parse(kvps.Value);
                          }

                          if (kvps.Key.Equals("n_daratlaut"))
                          {
                              nDaratLaut = decimal.Parse(kvps.Value);
                          }

                          if (kvps.Key.Equals("n_icepack"))
                          {
                              nIcePack = decimal.Parse(kvps.Value);
                          }

                          if (kvps.Key.Equals("n_cdd"))
                          {
                              nCDD = decimal.Parse(kvps.Value);
                          }

                          if (kvps.Key.Equals("n_fuso"))
                          {
                              nFuso = decimal.Parse(kvps.Value);
                          }

                          if (kvps.Key.Equals("n_tronton"))
                          {
                              nTronton = decimal.Parse(kvps.Value);
                          }

                          if (kvps.Key.Equals("c_via"))
                          {
                              sVia = (string)kvps.Value;
                          }

                          if (kvps.Key.Equals("v_ket_via"))
                          {
                              sViaKet = (string)kvps.Value;
                          }
                      }

                      //set price
                      switch (sVia)
                      {
                          case "01":
                              nBiayaKirim = nUdara;
                              break;
                          case "02":
                              nBiayaKirim = nDaratLaut;
                              break;
                          case "03":
                              nBiayaKirim = nIcePack;
                              break;
                          case "04":
                              nBiayaKirim = nCDD;
                              break;
                          case "05":
                              nBiayaKirim = nFuso;
                              break;
                          case "06":
                              nBiayaKirim = nTronton;
                              break;
                      }

                      //all berat
                      if(nBerat % 1 == divider)
                      {
                          beratRound = nBerat;
                      }
                      else 
                      {
                          beratRound = Math.Round(nBerat, MidpointRounding.AwayFromZero);
                      }

                      costBerat = beratRound * nBiayaKirim;
                      bruto = costBerat + nBiayaLain;
                      totalCostBerat += bruto;

                      //volume
                      if (nVolume > 0)
                      {
                          if (nVolume % 1 == divider)
                          {
                              volumeRound = nVolume;
                          }
                          else
                          {
                              volumeRound = Math.Round(nVolume, MidpointRounding.AwayFromZero);
                          }
                          totalCostVolume += volumeRound * nBiayaKirim + nBiayaLain;
                      }
                      else
                      {
                          totalCostVolume += costBerat + nBiayaLain;
                      }
                      
                      sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                              'c_resi': '{1}',
                              'c_expno': '{2}',
                              'v_cunam': '{3}',
                              'c_cusno': '{4}',
                              'n_koli': {5},
                              'n_berat': {6},
                              'n_vol': {7},
                              'n_biaya' : {8},
                              'c_exptype' : '{9}',
                              'v_ket_exptype' : '{10}',
                              'c_via' : '{11}',
                              'v_ket_via' : '{12}',
                              'n_biayalain' : {13},
                              'n_totalcost': {14},
                              'l_new': true,
                              'l_modified': false,
                              'l_void': false,
                            }})); 
                            ", gridDetail.GetStore().ClientID,
                             sResi, sExpno, sCunam, sCusno, nKoli, nBerat, nVolume, nBiayaKirim, sExptype, sExptypeKet, sVia, sViaKet, nBiayaLain, bruto);
                  }
              }
          }

          sumTaxBerat = (totalCostBerat * (Tax / 100));
          totalCostBerat = totalCostBerat - sumTaxBerat - Potongan;
          lbNetBtm.Text = totalCostBerat.ToString();

          sumTaxVolume = (totalCostVolume * (Tax / 100));
          totalCostVolume = totalCostVolume - sumTaxVolume - Potongan;
          lbNetVolBtm.Text = totalCostVolume.ToString();
          sb.AppendFormat("recalculateFaktur({0});", tmp);
          

          X.AddScript(sb.ToString());

      }
      catch (Exception ex)
      {
          System.Diagnostics.Debug.WriteLine(
            string.Concat("transaksi_InvoiceShipment:PopulateDetail Detail - ", ex.Message));
      }
      finally
      {
          if (jarr != null)
          {
              jarr.Clear();
          }
          if (dicEXPInfo != null)
          {
              dicEXPInfo.Clear();
          }
          if (dicEXP != null)
          {
              dicEXP.Clear();
          }
          sb.Length = 0;
      }

      #endregion
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