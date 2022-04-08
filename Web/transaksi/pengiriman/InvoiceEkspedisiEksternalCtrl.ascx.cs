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

public partial class transaksi_InvoiceEkspedisiEksternalCtrl : System.Web.UI.UserControl
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

      txTOP.Text = "30";
      txTOP.Disabled = false;

      txPajak.Clear();
      txPajak.Text = "0";
      txPajak.Disabled = false;

      txKet.Clear();
      txKet.Disabled = false;


      txMaterai.Clear();
      txMaterai.Text = "0";
      txMaterai.Disabled = false;

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

      Ext.Net.Store store2 = gridDetail2.GetStore();
      if (store2 != null)
      {
          store2.RemoveAll();
      }

      lbGrossBtm.Text = "0";
      lbDiscBtm.Text = "0";
      lbTaxBtm.Text = "0";
      lbNetBtm.Text = "0";
      lbLainBtm.Text = "0";
      lbNetVolBtm.Text = "0";
      lbMaterai.Text = "0";

      btnPrint.Hidden = true;

      hfIeno.Clear();
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string numberID, string gudang, Dictionary<string, string>[] dics, Dictionary<string, string>[] dics2, string gross, string totalPajak, string totalBiayaLain, string netBerat, string netVol, string totalPotongan)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.TagExtraName = "FieldClaim";
    pair.Value = numberID;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicValuesExtra = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    double grossP = double.Parse(gross, CultureInfo.CreateSpecificCulture("id-ID"));
    double totalPajakP = double.Parse(totalPajak, CultureInfo.CreateSpecificCulture("id-ID"));
    double totalBiayaLainP = double.Parse(totalBiayaLain, CultureInfo.CreateSpecificCulture("id-ID"));
    double netBeratP = double.Parse(netBerat, CultureInfo.CreateSpecificCulture("id-ID"));
    double netVolP = double.Parse(netVol, CultureInfo.CreateSpecificCulture("id-ID"));
    double totalPotonganP = double.Parse(totalPotongan, CultureInfo.CreateSpecificCulture("id-ID"));

    decimal koli = 0,
        berat = 0,
        volume = 0,
        biayalain = 0,
        totalcost = 0,
        biaya = 0,
        expmin = 0,
        tonase = 0,
        potongan = 0;

    int urut = 0;

    string tmp = null,
        varData = null,
        ketVoid = null,
        resi = null,
        epno = null,
        cabang = null,
        exptype = null,
        via = null,
        claimno = null;

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
    pair.DicAttributeValues.Add("Materai", txMaterai.Text);
    pair.DicAttributeValues.Add("totalPotongan", totalPotonganP.ToString());
    //pair.DicAttributeValues.Add("ClaimNo", txClaimNo.Text);

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
        tonase = dicData.GetValueParser<decimal>("n_tonase", 0);
        biaya = dicData.GetValueParser<decimal>("n_biaya", 0);
        expmin = dicData.GetValueParser<decimal>("n_expmin", 0);
        exptype = dicData.GetValueParser<string>("c_exptype");
        via = dicData.GetValueParser<string>("c_via");
        //tipebiaya = dicData.GetValueParser<string>("c_tipebiaya");
        biayalain = dicData.GetValueParser<decimal>("n_biayalain", 0);
        totalcost = dicData.GetValueParser<decimal>("n_totalbiaya", 0);
        urut = dicData.GetValueParser<int>("i_urut", 0);

            if (!string.IsNullOrEmpty(resi))
        {
          dicAttr.Add("resiNo", resi);
          dicAttr.Add("epNo", epno);
          dicAttr.Add("Cusno", cabang);
          dicAttr.Add("Koli", koli.ToString());
          dicAttr.Add("Berat", berat.ToString());
          dicAttr.Add("Volume", volume.ToString());
          dicAttr.Add("Tonase", tonase.ToString());
          dicAttr.Add("Biaya", biaya.ToString());
          dicAttr.Add("Via", via.ToString());
          dicAttr.Add("expType", exptype.ToString());
          dicAttr.Add("expMin", expmin.ToString());
          //dicAttr.Add("tipeBiaya", tipebiaya.ToString());
          dicAttr.Add("biayaLain", biayalain.ToString());
          dicAttr.Add("totalCost", totalcost.ToString());
          dicAttr.Add("urut", urut.ToString());

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
        tonase = dicData.GetValueParser<decimal>("n_tonase", 0);
        biaya = dicData.GetValueParser<decimal>("n_biaya", 0);
        expmin = dicData.GetValueParser<decimal>("n_expmin", 0);
        exptype = dicData.GetValueParser<string>("c_exptype");
        via = dicData.GetValueParser<string>("c_via");
        //tipebiaya = dicData.GetValueParser<string>("c_tipebiaya");
        biayalain = dicData.GetValueParser<decimal>("n_biayalain", 0);
        totalcost = dicData.GetValueParser<decimal>("n_totalbiaya", 0);

        if (!string.IsNullOrEmpty(resi))
        {
            dicAttr.Add("resiNo", resi);
            dicAttr.Add("epNo", epno);
            dicAttr.Add("Cusno", cabang);
            dicAttr.Add("Koli", koli.ToString());
            dicAttr.Add("Berat", berat.ToString());
            dicAttr.Add("Volume", volume.ToString());
            dicAttr.Add("Tonase", tonase.ToString());
            dicAttr.Add("Biaya", biaya.ToString());
            dicAttr.Add("Via", via.ToString());
            dicAttr.Add("expType", exptype.ToString());
            dicAttr.Add("expMin", expmin.ToString());
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
      else if ((!isModify) && isVoid && (!isNew))
      {
        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        ketVoid = dicData.GetValueParser<string>("v_ket");
        resi = dicData.GetValueParser<string>("c_resi");
        epno = dicData.GetValueParser<string>("c_expno");
        cabang = dicData.GetValueParser<string>("c_cusno");
        koli = dicData.GetValueParser<decimal>("n_koli", 0);
        berat = dicData.GetValueParser<decimal>("n_berat", 0);
        volume = dicData.GetValueParser<decimal>("n_vol", 0);
        tonase = dicData.GetValueParser<decimal>("n_tonase", 0);
        biaya = dicData.GetValueParser<decimal>("n_biaya", 0);
        expmin = dicData.GetValueParser<decimal>("n_expmin", 0);
        exptype = dicData.GetValueParser<string>("c_exptype");
        via = dicData.GetValueParser<string>("c_via");
        //tipebiaya = dicData.GetValueParser<string>("c_tipebiaya");
        biayalain = dicData.GetValueParser<decimal>("n_biayalain", 0);
        totalcost = dicData.GetValueParser<decimal>("n_totalbiaya", 0);

        if (!string.IsNullOrEmpty(resi))
        {
            dicAttr.Add("resiNo", resi);
            dicAttr.Add("epNo", epno);
            dicAttr.Add("Cusno", cabang);
            dicAttr.Add("Koli", koli.ToString());
            dicAttr.Add("Berat", berat.ToString());
            dicAttr.Add("Volume", volume.ToString());
            dicAttr.Add("Tonase", tonase.ToString());
            dicAttr.Add("Biaya", biaya.ToString());
            dicAttr.Add("Via", via.ToString());
            dicAttr.Add("expType", exptype.ToString());
            dicAttr.Add("expMin", expmin.ToString());
            //dicAttr.Add("tipeBiaya", tipebiaya.ToString());
            dicAttr.Add("biayaLain", biayalain.ToString());
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

    #region Grid Details Claim

    for (nLoop = 0, nLen = dics2.Length; nLoop < nLen; nLoop++)
    {
        tmp = nLoop.ToString();

        dicData = dics2[nLoop];

        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        isNew = dicData.GetValueParser<bool>("l_new");
        isModify = dicData.GetValueParser<bool>("l_modified");
        isVoid = dicData.GetValueParser<bool>("l_void");

        if (isNew && (!isVoid) && (!isModify))
        {
            dicAttr.Add("New", isNew.ToString().ToLower());
            dicAttr.Add("Delete", isVoid.ToString().ToLower());
            dicAttr.Add("Modified", isModify.ToString().ToLower());

            claimno = dicData.GetValueParser<string>("c_claimno");
            potongan = dicData.GetValueParser<decimal>("n_disc", 0);

            if (!string.IsNullOrEmpty(claimno))
            {
                dicAttr.Add("claimNo", claimno);
                dicAttr.Add("Potongan", potongan.ToString());

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

            claimno = dicData.GetValueParser<string>("c_claimno");
            potongan = dicData.GetValueParser<decimal>("n_disc", 0);

            if (!string.IsNullOrEmpty(claimno))
            {
                dicAttr.Add("claimNo", claimno);
                dicAttr.Add("Potongan", potongan.ToString());

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

            claimno = dicData.GetValueParser<string>("c_claimno");
            potongan = dicData.GetValueParser<decimal>("n_disc", 0);

            if (!string.IsNullOrEmpty(claimno))
            {
                dicAttr.Add("claimNo", claimno);
                dicAttr.Add("Potongan", potongan.ToString());

                pair.DicValuesExtra.Add(tmp, new PostDataParser.StructurePair()
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
        varData = parser.ParserData("InvoiceEkspedisiEksternal", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
        Scms.Web.Common.Logger.WriteLine("transaksi_InvoiceEkspedisiCtrl SaveParser : {0} ", ex.Message);
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
    string fakturId = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "c_ieno = @0", pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    DateTime date = DateTime.Today;

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

        winDetail.Title = string.Concat("Invoice Ekspedisi Eksternal Detail - ", pID);

        fakturId = dicResultInfo.GetValueParser<string>("c_ieno", string.Empty);

        Functional.SetComboData(cbEksHdr, "c_exp", dicResultInfo.GetValueParser<string>("v_nama_exp", string.Empty), dicResultInfo.GetValueParser<string>("c_exp", string.Empty));
        cbEksHdr.Disabled = true;

        txFakturHdr.Text = dicResultInfo.GetValueParser<string>("c_ie");
        //txFakturHdr.Disabled = true;

        txFisikFaktur.Text = dicResultInfo.GetValueParser<string>("n_bilva_faktur").ToString();
        //txFisikFaktur.Disabled = true;

        date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_iedate"));
        txTglFaktur.Text = date.ToString("dd-MM-yyyy");
        //txTglFaktur.Disabled = true;

        txTOP.Text = dicResultInfo.GetValueParser<decimal>("n_top", 0).ToString();
        //txTOP.Disabled = true;

        txPajak.Text = dicResultInfo.GetValueParser<decimal>("n_tax", 0).ToString();
        //txPajak.Disabled = true;

        txMaterai.Text = dicResultInfo.GetValueParser<decimal>("n_materai", 0).ToString();
        //txMaterai.Disabled = true;

        //txPotongan.Text = dicResultInfo.GetValueParser<decimal>("n_disc", 0).ToString();
        //txPotongan.Disabled = true;

        //txClaimNo.Text = dicResultInfo.GetValueParser<string>("c_claimno");
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

        btnPrint.Hidden = false;

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_InvoiceEkspedisiCtrl:PopulateDetail Header - ", ex.Message));
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

      hfIeno.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_InvoiceEkspedisiCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    #region Parser Detail Claim

    try
    {
        Ext.Net.Store store2 = gridDetail2.GetStore();
        if (store2.Proxy.Count > 0)
        {
            Ext.Net.ScriptTagProxy stp = store2.Proxy[0] as Ext.Net.ScriptTagProxy;
            if ((stp != null) && stp.CallbackParam.Equals(Functional.NAME_SOA_SCMS_CALLBACK, StringComparison.OrdinalIgnoreCase))
            {
                string param = (store2.BaseParams["parameters"] ?? string.Empty);
                if (string.IsNullOrEmpty(param))
                {
                    store2.BaseParams.Add(new Ext.Net.Parameter("parameters", string.Format("[['{0} = @0', '{1}', 'System.String']]", pName, pID), ParameterMode.Raw));
                }
                else
                {
                    store2.BaseParams["parameters"] = string.Format("[['{0} = @0', '{1}', 'System.String']]", pName, pID);
                }
            }
        }

        hfIeno.Text = pID;
        X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store2.ClientID));
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine(
          string.Concat("transaksi_InvoiceEkspedisiCtrl:PopulateDetail Detail - ", ex.Message));
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

      Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
      hfGdg.Text = pag.ActiveGudang;

    if (isAdd)
    {
      ClearEntrys();

      winDetail.Title = "Invoice Ekspedisi Eksternal";

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      PopulateDetail("c_ieno", pID);
    }
  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string jsonGridValues2 = (e.ExtraParams["gridValues2"] ?? string.Empty);
    string gdg = (e.ExtraParams["Gudang"] ?? string.Empty);
    string gross = (e.ExtraParams["Gross"] ?? string.Empty);
    string totalPajak = (e.ExtraParams["Pajak"] ?? string.Empty);
    string totalBiayaLain = (e.ExtraParams["TotalBiayaLain"] ?? string.Empty);
    string netBerat = (e.ExtraParams["NetBerat"] ?? string.Empty);
    string netVol = (e.ExtraParams["NetVol"] ?? string.Empty);
    string totalPotongan = (e.ExtraParams["totalPotongan"] ?? string.Empty);


    decimal netVolParse = decimal.Parse(netVol, CultureInfo.CreateSpecificCulture("id-ID"));

    if (netVolParse != decimal.Parse(txFisikFaktur.Text))
    {
        Functional.ShowMsgInformation("Net dengan fisik berbeda!");
        return;
    }

    
    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    Dictionary<string, string>[] gridData = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);
    Dictionary<string, string>[] gridData2 = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues2);

    
    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gdg, gridData, gridData2, gross, totalPajak, totalBiayaLain, netBerat, netVol, totalPotongan);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;
        string storeId = hfStoreID.Text;
        string gdgdesc = "Gudang " + hfGdg.Text;

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
                                                        'v_gdgdesc': '{1}',
                                                        'c_ieno': '{2}',
                                                        'c_ie': '{3}',
                                                        'd_iedate': {4},
                                                        'v_nama_exp': '{5}',
                                                        'n_bilva_faktur': {6},
                                                        'n_netvol': {7},
                                                        'v_ket': '{8}'
                                        }}));{0}.commitChanges();", storeId,
                                        gdgdesc,
                                        respon.Values.GetValueParser<string>("ieno"),
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
              scrpt = string.Format(@"var idx = {0}.findExact('c_ieno', '{1}');
                      if(idx != -1) {{
                        var r = {0}.getAt(idx);
                        r.set('n_bilva_faktur', {2});
                        r.set('n_netvol', {3});
                        r.set('v_ket', '{4}');
                        {0}.commitChanges();
                      }}", storeId,
                          respon.Values.GetValueParser<string>("ieno"),
                          respon.Values.GetValueParser<decimal>("FisikFaktur", -1),
                          respon.Values.GetValueParser<decimal>("NetVol", -1),
                          txKet.Text
                          );
            }

            X.AddScript(scrpt);
          }
        }

        //this.ClearEntrys();

        PopulateDetail("c_ieno", respon.Values.GetValueParser<string>("ieno", string.Empty));

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
  protected void AddBtnResi_Click(object sender, DirectEventArgs e)
  {
      string exp = e.ExtraParams["exp"].ToString();
      string resi = e.ExtraParams["resi"].ToString();
      string expno = null;
      string gdg = hfGdg.Text;
      string jsonGridValues = (e.ExtraParams["Grid"] ?? string.Empty);
      bool isNotDouble = false;
      
      if (!string.IsNullOrEmpty(resi))
      {
          isNotDouble = jsonGridValues.Contains(resi);
      }

      if (cbResi.SelectedIndex < 0)
      {
          return;
      }

      if (!isNotDouble)
      {
          InsertDetailResi(exp, resi, gdg, expno, true, jsonGridValues);
      }
      else
      {
          Functional.ShowMsgInformation("No. " + resi + ", Data telah ada.");
      }
      cbResi.Clear();
      cbResi.Focus();
  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void AddBtnEP_Click(object sender, DirectEventArgs e)
  {
      string exp = e.ExtraParams["exp"].ToString();
      string resi = null;
      string expno = e.ExtraParams["expno"].ToString();
      string gdg = hfGdg.Text;
      string jsonGridValues = (e.ExtraParams["Grid"] ?? string.Empty);
      bool isNotDouble = false;

      if (cbEP.SelectedIndex < 0)
      {
          return;
      }

      if (!string.IsNullOrEmpty(expno))
      {
          isNotDouble = jsonGridValues.Contains(expno);
      }

      if (!isNotDouble)
      {
          InsertDetailResi(exp, resi, gdg, expno, true, jsonGridValues);
      }
      else
      {
          Functional.ShowMsgInformation("No. " + resi + ", Data telah ada.");
      }
      cbEP.Clear();
      cbEP.Focus();
  }

  private void InsertDetailResi(string exp, string resi, string gdg, string expno, bool add, string gridValues)
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
       sExptypeKet = null,
       res = null;

      decimal nKoli = 0,
      nBerat = 0,
      nVolume = 0,
      nBiayaLain = 0,
      nTotalBiaya = 0,
      nBiayaKirim = 0,
      divider = 0.5m,
      costBerat = 0,
      totalCostBerat = 0,
      beratAktual = 0,
      totalCostVolume = 0,
      volumeAktual = 0,
      tonase = 0,
      Tax = Int16.Parse(txPajak.Text),
          //Potongan = Int64.Parse(txPotongan.Text),
      nExpMin = 0;
      //tonase = 0;

      if (expno == null)
      { //add resi
          string[][] paramX = new string[][]{
              new string[] { string.Format("{0}", "c_exp"), exp, "System.String"},
              new string[] { string.Format("{0}", "c_resi"), resi, "System.String"},
              new string[] { string.Format("{0}", "c_gdg"), gdg, "System.String"},
            };

          res = soa.GlobalQueryService(0, -1, false, string.Empty, string.Empty, "0312", paramX);
      }
      else //add exp
      {
          string[][] paramX = new string[][]{
          new string[] { string.Format("{0}", "c_exp"), exp, "System.String"},
          new string[] { string.Format("{0}", "c_expno"), expno, "System.String"},
          new string[] { string.Format("{0}", "c_gdg"), gdg, "System.String"},
            };

          res = soa.GlobalQueryService(0, -1, false, string.Empty, string.Empty, "0315", paramX);
      }
          dicEXP = JSON.Deserialize<Dictionary<string, object>>(res);


      #region Parser Detail

      try
      {
          dicEXP = JSON.Deserialize<Dictionary<string, object>>(res);
          Dictionary<string, string>[] gridDataExp = JSON.Deserialize<Dictionary<string, string>[]>(gridValues);

          int ord = 0;

          if (gridDataExp.Length > 0)
          {
              foreach (KeyValuePair<string, string> kvp in gridDataExp[0])
              {
                  if (kvp.Key == "i_urut")
                  {
                      ord = string.IsNullOrEmpty(kvp.Value) ? 1 : int.Parse(kvp.Value) + 1;
                  }
              }
          }
          else
          {
              ord = gridDataExp.Length + 1;
          }

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
                              nBiayaLain = 0;
                          }

                          if (kvps.Key.Equals("n_totalbiaya"))
                          {
                              nTotalBiaya = decimal.Parse(kvps.Value);
                          }

                          if (kvps.Key.Equals("c_exptype"))
                          {
                              sExptype = (string)kvps.Value;
                          }

                          if (kvps.Key.Equals("v_ket_exptype"))
                          {
                              sExptypeKet = (string)kvps.Value;
                          }

                          if (kvps.Key.Equals("n_biayakg"))
                          {
                              nBiayaKirim = decimal.Parse(kvps.Value);
                          }

                          if (kvps.Key.Equals("c_via"))
                          {
                              sVia = (string)kvps.Value;
                          }

                          if (kvps.Key.Equals("v_ket_via"))
                          {
                              sViaKet = (string)kvps.Value;
                          }

                          if (kvps.Key.Equals("n_expmin"))
                          {
                              nExpMin = decimal.Parse(kvps.Value);
                          }
                          if (kvps.Key.Equals("n_biayalain"))
                          {
                              tonase = decimal.Parse(kvps.Value);
                          }
                      }

                      if (nTotalBiaya > 0)
                      {                          
                          ////all berat
                          if (sVia == "04" || sVia == "05" || sVia == "06" || sVia == "07" || sVia == "08" || sVia == "09")
                          {
                              totalCostBerat = nBiayaKirim;
                              totalCostVolume = nBiayaKirim;
                          }
                          else
                          {
                              if (nExpMin > 0 && nBerat < nExpMin)
                              {
                                  beratAktual = nExpMin;
                              }
                              else
                              {
                                  beratAktual = nBerat;
                              }

                              //bruto = costBerat + nBiayaLain;
                              totalCostBerat = beratAktual * nBiayaKirim;

                              //volume
                              if (nVolume > 0)
                              {
                                  //tonase = nVolume / 6000 * 1000000;
                                  if (nExpMin > 0 && nVolume < nExpMin)
                                  {
                                      volumeAktual = nExpMin;
                                  }
                                  else
                                  {
                                      volumeAktual = nVolume;
                                  }
                                  totalCostVolume += volumeAktual * nBiayaKirim;
                              }
                              else
                              {
                                  totalCostVolume = costBerat;
                              }
                          }
                      }
                      else
                      {
                          nBiayaKirim = 0;
                          totalCostBerat = 0;
                          totalCostVolume = 0;
                      }
                      
                      sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                              'c_resi': '{1}',
                              'c_expno': '{2}',
                              'v_cunam': '{3}',
                              'c_cusno': '{4}',
                              'n_koli': {5},
                              'n_berat': {6},
                              'n_tonase': {7},
                              'n_vol': {8},
                              'n_biaya' : {9},
                              'n_expmin' : {10},
                              'c_exptype' : '{11}',
                              'v_ket_exptype' : '{12}',
                              'c_via' : '{13}',
                              'v_ket_via' : '{14}',
                              'n_biayalain' : {15},
                              'n_totalbiaya': {16},
                              'l_new': true,
                              'l_modified': false,
                              'l_void': false,
                              'i_urut' : {17}
                            }})); 
                            ", gridDetail.GetStore().ClientID,
                             sResi, sExpno, sCunam, sCusno, nKoli, nBerat, nVolume, tonase, nBiayaKirim, nExpMin,sExptype, sExptypeKet, sVia, sViaKet, nBiayaLain, totalCostVolume > 0 ? totalCostVolume : totalCostBerat, ord);

                      ord++;
                  }
              }
          }

          
          //sumTaxBerat = (totalCostBerat * (Tax / 100));
          //totalCostBerat = totalCostBerat + sumTaxBerat - Potongan;
          //lbNetBtm.Text = totalCostBerat.ToString();

          //sumTaxVolume = (totalCostVolume * (Tax / 100));
          //totalCostVolume = totalCostVolume + sumTaxVolume - Potongan;
          //lbNetVolBtm.Text = totalCostVolume.ToString();
          sb.AppendFormat("recalculateFaktur({0});", tmp);
          

          X.AddScript(sb.ToString());

      }
      catch (Exception ex)
      {
          System.Diagnostics.Debug.WriteLine(
            string.Concat("transaksi_InvoiceEkspedisi:PopulateDetail Detail - ", ex.Message));
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

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
      Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

      if (!pag.IsAllowPrinting)
      {
          Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
          return;
      }

      string gdgId = pag.ActiveGudang;
      string numberID = (e.ExtraParams["NumberID"] ?? string.Empty);
      string outputRpt = (e.ExtraParams["OutputRpt"] ?? string.Empty);

      if (string.IsNullOrEmpty(numberID))
      {
          Functional.ShowMsgError("Maaf, No IE tidak dapat dibaca.");

          return;
      }

      ReportParser rptParse = new ReportParser();

      List<ReportParameter> lstRptParam = new List<ReportParameter>();

      List<string> lstData = new List<string>();

      rptParse.ReportingID = "10116";

      #region Report Parameter

      lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(char).FullName,
          ParameterName = "LG_IEH.c_gdg",
          ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
      });

      lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(string).FullName,
          ParameterName = "LG_IEH.c_ieno",
          ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID)
      });

      #endregion

      rptParse.IsLandscape = true;
      rptParse.PaperID = "Letter";
      rptParse.ReportParameter = lstRptParam.ToArray();
      rptParse.User = pag.Nip;
      rptParse.UserDefinedName = numberID;
      rptParse.OutputReport = ReportParser.ParsingOutputReport(outputRpt);

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
              string tmpUri = Functional.UriDownloadGenerator(pag,
                rptResult.OutputFile, "Invoice Ekspedisi Eksternal", rptResult.Extension);

              wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
          }
          else
          {
              Functional.ShowMsgWarning(rptResult.MessageResponse);
          }
      }

      GC.Collect();
  }
}