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

public partial class transaksi_InvoiceEkspedisiInternalCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {

      DateTime date = DateTime.Now;

      txFakturHdr.Clear();
      txFakturHdr.Disabled = true;

      txFisikFaktur.Clear();
      txFisikFaktur.Disabled = true;

      txTglFaktur.Clear();
      txTglFaktur.Text = date.ToString("dd-MM-yyyy");
      txTglFaktur.Disabled = false;

      txKet.Clear();
      txKet.Disabled = false;

      txawalKM.Clear();
      txawalKM.Disabled = false;
      txakhirKM.Clear();
      txakhirKM.Disabled = false;
      lbJarak.Text = "0";

      txBBM.Clear();
      txBBM.Disabled = false;

      txTolDtl.Clear();

      //Indra D. 20170426
      //rdgSolar.Checked = true;
      lbBiayaBBM.Text = "0";

      cbEP.Clear();
      cbEP.Disabled = false;
      Ext.Net.Store cbEPStr = cbEP.GetStore();
      if (cbEPStr != null)
      {
          cbEPStr.RemoveAll();
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

      lbTolBtm.Text = "0";
      lbNetBtm.Text = "0";

      btnPrint.Hidden = true;

      hfIeno.Clear();
  }

  //Indra 20170426 Penambahan TipeBBM
  private PostDataParser.StructureResponse SaveParser(bool isAdd, string numberID, string gudang, Dictionary<string, string>[] dics, Dictionary<string, string>[] dics2, string net, string biayaBBM, string tol, string TipeBBM)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.TagExtraName = "FieldTol";
    pair.Value = numberID;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicValuesExtra = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    double totalBiayaLainP = 0;         //double.Parse(totalBiayaLain, CultureInfo.CreateSpecificCulture("id-ID"));
    double netP = double.Parse(net, CultureInfo.CreateSpecificCulture("id-ID"));
    double tolP = double.Parse(tol, CultureInfo.CreateSpecificCulture("id-ID"));

    decimal koli = 0,
        berat = 0,
        volume = 0,
        biayalain = 0,
        biaya = 0,
        detailTol = 0;

    int urut = 0;
        
    string tmp = null,
        varData = null,
        ketVoid = null,
        resi = null,
        epno = null,
        cabang = null,
        via = null,
        idx = null;

    bool isNew = false,
      isModify = false,
      isVoid = false,
      isSolar = false;


    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Gudang", (string.IsNullOrEmpty(gudang) ? "1" : gudang.Trim()));
    pair.DicAttributeValues.Add("Faktur", txFakturHdr.Text);
    pair.DicAttributeValues.Add("FisikFaktur", txFisikFaktur.Text);
    if (!Functional.DateParser(txTglFaktur.RawText, "dd-MM-yyyy", out date))
    {
        date = DateTime.MinValue;
    }
    pair.DicAttributeValues.Add("FakturDate", date.ToString("yyyyMMddHHmmssfff"));
    pair.DicAttributeValues.Add("Ket", txKet.Text);
    pair.DicAttributeValues.Add("BiayaTol", tolP.ToString());
    pair.DicAttributeValues.Add("BBMLiter", txBBM.Text);
    //isSolar = rdgBensin.Checked == true ? false : true;
    pair.DicAttributeValues.Add("BBMType", isSolar.ToString().ToLower());
    pair.DicAttributeValues.Add("BiayaBBM", biayaBBM);
    pair.DicAttributeValues.Add("awalKM", txawalKM.Text);
    pair.DicAttributeValues.Add("akhirKM", txakhirKM.Text);
    //pair.DicAttributeValues.Add("Materai", txMaterai.Text);
    pair.DicAttributeValues.Add("TotalBiayaLain", totalBiayaLainP.ToString());
    pair.DicAttributeValues.Add("Net", netP.ToString());
    if (TipeBBM.Trim() == "Solar")
    {
        TipeBBM = "02";
    }
    else if (TipeBBM.Trim() == "Bensin")
    {
        TipeBBM = "01";
    }
    else if (TipeBBM.Trim() == "Dexlite")
    {
        TipeBBM = "03";
    }
    pair.DicAttributeValues.Add("TipeBBM", TipeBBM.Trim()); //Indra D 20170426

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
        //biaya = dicData.GetValueParser<decimal>("n_biaya", 0);
        //exptype = dicData.GetValueParser<string>("c_exptype");
        via = dicData.GetValueParser<string>("c_via");
        //tipebiaya = dicData.GetValueParser<string>("c_tipebiaya");
        biayalain = dicData.GetValueParser<decimal>("n_biayalain", 0);
        //totalcost = dicData.GetValueParser<decimal>("n_totalbiaya", 0);
        urut = dicData.GetValueParser<int>("i_urut", 0);

            if (!string.IsNullOrEmpty(resi))
        {
          dicAttr.Add("resiNo", resi);
          dicAttr.Add("epNo", epno);
          dicAttr.Add("Cusno", cabang);
          dicAttr.Add("Koli", koli.ToString());
          dicAttr.Add("Berat", berat.ToString());
          dicAttr.Add("Volume", volume.ToString());
          //dicAttr.Add("Biaya", biaya.ToString());
          dicAttr.Add("Via", via.ToString());
          //dicAttr.Add("expType", exptype.ToString());
          //dicAttr.Add("tipeBiaya", tipebiaya.ToString());
          dicAttr.Add("biayaLain", biayalain.ToString());
          //dicAttr.Add("totalCost", totalcost.ToString());
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
        //biaya = dicData.GetValueParser<decimal>("n_biaya", 0);
        //exptype = dicData.GetValueParser<string>("c_exptype");
        via = dicData.GetValueParser<string>("c_via");
        //tipebiaya = dicData.GetValueParser<string>("c_tipebiaya");
        biayalain = dicData.GetValueParser<decimal>("n_biayalain", 0);
        //totalcost = dicData.GetValueParser<decimal>("n_totalbiaya", 0);

        if (!string.IsNullOrEmpty(resi))
        {
            dicAttr.Add("resiNo", resi);
            dicAttr.Add("epNo", epno);
            dicAttr.Add("Cusno", cabang);
            dicAttr.Add("Koli", koli.ToString());
            dicAttr.Add("Berat", berat.ToString());
            dicAttr.Add("Volume", volume.ToString());
            //dicAttr.Add("Biaya", biaya.ToString());
            dicAttr.Add("Via", via.ToString());
            //dicAttr.Add("expType", exptype.ToString());
            //dicAttr.Add("tipeBiaya", tipebiaya.ToString());
            dicAttr.Add("biayaLain", biayalain.ToString());
            //dicAttr.Add("totalCost", totalcost.ToString());

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
        //biaya = dicData.GetValueParser<decimal>("n_biaya", 0);
        //exptype = dicData.GetValueParser<string>("c_exptype");
        via = dicData.GetValueParser<string>("c_via");
        //tipebiaya = dicData.GetValueParser<string>("c_tipebiaya");
        biayalain = dicData.GetValueParser<decimal>("n_biayalain", 0);
        //totalcost = dicData.GetValueParser<decimal>("n_totalbiaya", 0);

        if (!string.IsNullOrEmpty(resi))
        {
            dicAttr.Add("resiNo", resi);
            dicAttr.Add("epNo", epno);
            dicAttr.Add("Cusno", cabang);
            dicAttr.Add("Koli", koli.ToString());
            dicAttr.Add("Berat", berat.ToString());
            dicAttr.Add("Volume", volume.ToString());
            //dicAttr.Add("Biaya", biaya.ToString());
            dicAttr.Add("Via", via.ToString());
            //dicAttr.Add("expType", exptype.ToString());
            //dicAttr.Add("tipeBiaya", tipebiaya.ToString());
            dicAttr.Add("biayaLain", biayalain.ToString());
            //dicAttr.Add("totalCost", totalcost.ToString());

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

    #region Grid Details Tol

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

            detailTol = dicData.GetValueParser<decimal>("n_detailtol", 0);

            if (!string.IsNullOrEmpty(detailTol.ToString()))
            {
                dicAttr.Add("detailtol", detailTol.ToString());

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

            idx = dicData.GetValueParser<string>("idx");
            detailTol = dicData.GetValueParser<decimal>("n_detailtol", 0);

            if (!string.IsNullOrEmpty(idx))
            {
                dicAttr.Add("idx", idx);
                dicAttr.Add("detailtol", detailTol.ToString());

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

            idx = dicData.GetValueParser<string>("IDX");

            if (!string.IsNullOrEmpty(idx))
            {
                dicAttr.Add("idx", idx);

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
        varData = parser.ParserData("InvoiceEkspedisiInternal", (isAdd ? "Add" : "Modify"), dic);
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

    bool isSolar = false;
    int jarak = 0;

    CultureInfo culture = new CultureInfo("id-ID");

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "04003", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Concat("Invoice Ekspedisi Internal Detail - ", pID);

        fakturId = dicResultInfo.GetValueParser<string>("c_ieno", string.Empty);
        txFakturHdr.Text = dicResultInfo.GetValueParser<string>("c_ie");
        txFakturHdr.Disabled = true;

        txFisikFaktur.Text = dicResultInfo.GetValueParser<string>("n_bilva_faktur").ToString();
        txFisikFaktur.Disabled = true;

        date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_iedate"));

        txawalKM.Text = dicResultInfo.GetValueParser<decimal>("n_awalkm", 0).ToString();
        txakhirKM.Text = dicResultInfo.GetValueParser<decimal>("n_akhirkm", 0).ToString();
        jarak = Int32.Parse(txakhirKM.Text) - Int32.Parse(txawalKM.Text);
        lbJarak.Text = jarak.ToString();

        txBBM.Text = dicResultInfo.GetValueParser<decimal>("n_bbmliter", 0).ToString();
        lbBiayaBBM.Text = dicResultInfo.GetValueParser<decimal>("n_bbmprice", 0).ToString();
        lbTolBtm.Text = dicResultInfo.GetValueParser<decimal>("n_tol", 0).ToString("N2", culture);
        isSolar = dicResultInfo.GetValueParser<bool>("l_solar");
        
        //Indra D. 20170426
        //if (isSolar)
        //{
        //    rdgSolar.Checked = true;
        //}
        //else
        //{
        //    rdgBensin.Checked = true;
        //}
        rdgBBMType.Text = dicResultInfo.GetValueParser<string>("v_tipe_bbm").ToString();
        
        //txMaterai.Text = dicResultInfo.GetValueParser<decimal>("n_materai", 0).ToString();

        txKet.Text = dicResultInfo.GetValueParser<string>("v_ket");

        //lbLainBtm.Text = dicResultInfo.GetValueParser<decimal>("n_totalbiayalain").ToString("N2", culture);
        //lbMaterai.Text = dicResultInfo.GetValueParser<decimal>("n_materai").ToString("N2", culture);
        lbNetBtm.Text = dicResultInfo.GetValueParser<decimal>("n_net").ToString("N2", culture);

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

      winDetail.Title = "Invoice Ekspedisi Internal";

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
    string biayaBBM = (e.ExtraParams["BBM"] ?? string.Empty);
    string net = (e.ExtraParams["Net"] ?? string.Empty);
    string tol = (e.ExtraParams["Tol"] ?? string.Empty);
    string TipeBBM = (e.ExtraParams["TipeBBM"] ?? string.Empty); //Indra 20170426


    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    Dictionary<string, string>[] gridData = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);
    Dictionary<string, string>[] gridData2 = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues2);

    //Indra 20170426 Penambahan TipeBBM
    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gdg, gridData, gridData2, net, biayaBBM, tol, TipeBBM);

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
                                                        'n_bilva_faktur': {5},
                                                        'n_netvol': {6},
                                                        'v_ket': '{7}'
                                        }}));{0}.commitChanges();", storeId,
                                        gdgdesc,
                                        respon.Values.GetValueParser<string>("ieno"),
                                        txFakturHdr.Text,
                                        dateJs,
                                        respon.Values.GetValueParser<decimal>("FisikFaktur", -1),
                                        respon.Values.GetValueParser<decimal>("Net", -1),
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
                          respon.Values.GetValueParser<decimal>("Net", -1),
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
  protected void addBtnEP_click(object sender, DirectEventArgs e)
  {
      string epno = e.ExtraParams["epno"].ToString();
      string jsonGridValues = (e.ExtraParams["Grid"] ?? string.Empty);
      string gdg = hfGdg.Text;
      bool isNotDouble = false;


      if (!string.IsNullOrEmpty(epno))
      {
          isNotDouble = jsonGridValues.Contains(epno);
      }

      if (!isNotDouble)
      {
          InsertDetail(epno, gdg, jsonGridValues);
      }
      else
      {
          Functional.ShowMsgInformation("No. " + epno + ", Data telah ada.");
      }
      cbEP.Clear();
      cbEP.Focus();
  }

  private void InsertDetail(string epno, string gdg, string gridValues)
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
      nBiayaLain = 0,
      nBiayaKirim = 0,
      divider = 0.5m,
      costBerat = 0,
      totalCostBerat = 0,
      beratRound = 0,
      totalCostVolume = 0,
      volumeRound = 0,
      bruto = 0;


      string[][] paramX = new string[][]{
              new string[] { string.Format("{0}", "c_expno"), epno, "System.String"},
              new string[] { string.Format("{0}", "c_gdg"), gdg, "System.String"},
            };

      string res = soa.GlobalQueryService(0, -1, false, string.Empty, string.Empty, "0316", paramX);
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

                          if (kvps.Key.Equals("c_via"))
                          {
                              sVia = (string)kvps.Value;
                          }

                          if (kvps.Key.Equals("v_ket_via"))
                          {
                              sViaKet = (string)kvps.Value;
                          }
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
                              'n_totalbiaya': {14},
                              'l_new': true,
                              'l_modified': false,
                              'l_void': false,
                              'i_urut': {15},
                            }})); 
                            ", gridDetail.GetStore().ClientID,
                             sResi, sExpno, sCunam, sCusno, nKoli, nBerat, nVolume, nBiayaKirim, sExptype, sExptypeKet, sVia, sViaKet, nBiayaLain, bruto, ord);

                      ord++;
                  }
              }
          }

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

      rptParse.ReportingID = "10117";

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

      rptParse.IsLandscape = false;
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
                rptResult.OutputFile, "Invoice Ekspedisi Internal", rptResult.Extension);

              wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
          }
          else
          {
              Functional.ShowMsgWarning(rptResult.MessageResponse);
          }
      }

      GC.Collect();
  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void BtnTipeBBM(object sebder, DirectEventArgs e)
  {

      if (rdgBBMType.Text == "Bensin")
      {
          lbBiayaBBM.Text = "1000";
      }
      
  }
}