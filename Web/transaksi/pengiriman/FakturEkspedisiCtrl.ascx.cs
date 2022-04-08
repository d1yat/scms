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

public partial class transaksi_FakturEkspedisiCtrl : System.Web.UI.UserControl
{
    #region Private

    private void ClearEntrys()
    {

        DateTime date = DateTime.Now;

        cbEksHdr.Clear();
        cbEksHdr.Disabled = false;

        txInvFisik.Clear();
        txInvFisik.Disabled = true;
        txInvFisik.Text = "0";

        txInvAMS.Clear();
        txInvAMS.Disabled = true;

        txSelisih.Clear();
        txSelisih.Disabled = true;

        txClaim.Clear();
        txClaim.Disabled = false;
        txClaim.Text = "0";

        txPinalty.Clear();
        txPinalty.Disabled = false;
        txPinalty.Text = "0";

        txLain.Clear();
        txLain.Disabled = false;
        txLain.Text = "0";

        txAlasan.Clear();
        txAlasan.Disabled = false;

        txSisa.Clear();
        txSisa.Disabled = true;

        txPph.Clear();
        txPph.Disabled = false;
        txPph.Text = "0";

        txTotBE.Clear();
        txTotBE.Disabled = false;

        txDateBE.Clear();
        txDateBE.Text = date.ToString("dd-MM-yyyy");
        txDateBE.Disabled = false;

        txKet.Clear();
        txKet.Disabled = false;

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

        //hfGdg.Clear();
        hfFaktur.Text = string.Empty;
        hfMaxValue.Text = "0";
        hfIsNpwp.Text = string.Empty;

        btnPrint.Hidden = true;
    }

    private PostDataParser.StructureResponse SaveParser(bool isAdd, string numberID, string gudang, Dictionary<string, string>[] dics)
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

        decimal nilaiFisik = 0,
            nilaiAms = 0,
            selisih = 0;

        string tmp = null,
            varData = null,
            ketVoid = null,
            resi = null,
            ieno = null;

        bool isNew = false,
          isModify = false,
          isVoid = false;

        DateTime date = DateTime.Today;

        dic.Add("ID", pair);

        pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
        pair.DicAttributeValues.Add("Gudang", (string.IsNullOrEmpty(gudang) ? "1" : gudang.Trim()));
        pair.DicAttributeValues.Add("Ekspedisi", cbEksHdr.SelectedItem.Value);
        pair.DicAttributeValues.Add("BilvaFaktur", txInvFisik.Text.Trim());
        pair.DicAttributeValues.Add("Net", txInvAMS.Text.Trim());
        pair.DicAttributeValues.Add("Selisih", txSelisih.Text.Trim());
        pair.DicAttributeValues.Add("Claim", txClaim.Text.Trim());
        pair.DicAttributeValues.Add("Pinalty", txPinalty.Text.Trim());
        pair.DicAttributeValues.Add("Lain2", txLain.Text.Trim());
        pair.DicAttributeValues.Add("Alasan", txAlasan.Text.Trim());
        pair.DicAttributeValues.Add("Pph", txPph.Text.Trim());
        pair.DicAttributeValues.Add("TotalNet", txTotBE.Text.Trim());
        if (!Functional.DateParser(txDateBE.RawText, "dd-MM-yyyy", out date))
        {
            date = DateTime.MinValue;
        }
        pair.DicAttributeValues.Add("FakturDate", date.ToString("yyyyMMddHHmmssfff"));
        pair.DicAttributeValues.Add("Ket", txKet.Text);

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

                ieno = dicData.GetValueParser<string>("c_ieno");
                resi = dicData.GetValueParser<string>("c_ie");
                nilaiFisik = dicData.GetValueParser<decimal>("n_bilvafaktur", 0);
                nilaiAms = dicData.GetValueParser<decimal>("n_bed", 0);
                selisih = dicData.GetValueParser<decimal>("n_bedselisih", 0);
                ketVoid = dicData.GetValueParser<string>("v_ket");

                if (!string.IsNullOrEmpty(ieno))
                {
                    dicAttr.Add("InvNo", ieno);
                    dicAttr.Add("Resi", resi);
                    dicAttr.Add("BilvaD", nilaiFisik.ToString());
                    dicAttr.Add("NetD", nilaiAms.ToString());
                    dicAttr.Add("SelisihD", selisih.ToString());

                    pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
                    {
                        IsSet = true,
                        Value = (string.IsNullOrEmpty(ketVoid) ? "Human error" : ketVoid),
                        DicAttributeValues = dicAttr
                    });
                }
            }
            else if (isModify && (!isVoid) && (!isNew))
            {
                dicAttr.Add("New", isNew.ToString().ToLower());
                dicAttr.Add("Delete", isVoid.ToString().ToLower());
                dicAttr.Add("Modified", isModify.ToString().ToLower());

                ieno = dicData.GetValueParser<string>("c_ieno");
                resi = dicData.GetValueParser<string>("c_ie");
                nilaiFisik = dicData.GetValueParser<decimal>("n_bilvafaktur", 0);
                nilaiAms = dicData.GetValueParser<decimal>("n_bed", 0);
                selisih = dicData.GetValueParser<decimal>("n_bedselisih", 0);
                ketVoid = dicData.GetValueParser<string>("v_ket");

                if (!string.IsNullOrEmpty(ieno))
                {
                    dicAttr.Add("InvNo", ieno);
                    dicAttr.Add("Resi", resi);
                    dicAttr.Add("BilvaD", nilaiFisik.ToString());
                    dicAttr.Add("NetD", nilaiAms.ToString());
                    dicAttr.Add("SelisihD", selisih.ToString());

                    pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
                    {
                        IsSet = true,
                        Value = (string.IsNullOrEmpty(ketVoid) ? "Human error" : ketVoid),
                        DicAttributeValues = dicAttr
                    });
                }
            }
            else if ((!isModify) && isVoid && (!isNew))
            {
                dicAttr.Add("New", isNew.ToString().ToLower());
                dicAttr.Add("Delete", isVoid.ToString().ToLower());
                dicAttr.Add("Modified", isModify.ToString().ToLower());

                ieno = dicData.GetValueParser<string>("c_ieno");
                resi = dicData.GetValueParser<string>("c_ie");
                nilaiFisik = dicData.GetValueParser<decimal>("n_bilvafaktur", 0);
                nilaiAms = dicData.GetValueParser<decimal>("n_bed", 0);
                selisih = dicData.GetValueParser<decimal>("n_bedselisih", 0);
                ketVoid = dicData.GetValueParser<string>("v_ket");

                if (!string.IsNullOrEmpty(ieno))
                {
                    dicAttr.Add("InvNo", ieno);
                    dicAttr.Add("Resi", resi);
                    dicAttr.Add("BilvaD", nilaiFisik.ToString());
                    dicAttr.Add("NetD", nilaiAms.ToString());
                    dicAttr.Add("SelisihD", selisih.ToString());

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
            varData = parser.ParserData("FakturEkspedisi", (isAdd ? "Add" : "Modify"), dic);
        }
        catch (Exception ex)
        {
            Scms.Web.Common.Logger.WriteLine("transaksi_pengiriman_FakturEkspedisiCtrl SaveParser : {0} ", ex.Message);
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
        new string[] { "c_beno = @0", pID, "System.String"}
      };

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        string tmp = null;
        DateTime date = DateTime.Today;

        char gdg = char.MinValue;

        CultureInfo culture = new CultureInfo("id-ID");

        #region Parser Header

        string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "05001", paramX);

        try
        {
            dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
            if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
            {
                jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

                dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

                winDetail.Title = string.Concat("Faktur Ekspedisi - ", pID);

                fakturId = dicResultInfo.GetValueParser<string>("c_beno", string.Empty);

                date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_bedate"));
                txDateBE.Text = date.ToString("dd-MM-yyyy");
                //txTanggalHdr.Disabled = true;

                Functional.SetComboData(cbEksHdr, "c_exp", dicResultInfo.GetValueParser<string>("v_nama_exp", string.Empty), dicResultInfo.GetValueParser<string>("c_exp", string.Empty));
                cbEksHdr.Disabled = true;

                txInvFisik.Text = dicResultInfo.GetValueParser<decimal>("n_bilvafaktur", 0).ToString();
                txInvAMS.Text = dicResultInfo.GetValueParser<decimal>("n_net", 0).ToString();
                txSelisih.Text = dicResultInfo.GetValueParser<decimal>("n_selisihbe", 0).ToString();
                txClaim.Text = dicResultInfo.GetValueParser<decimal>("n_claim", 0).ToString();
                txPinalty.Text = dicResultInfo.GetValueParser<decimal>("n_pinalty", 0).ToString();
                txLain.Text = dicResultInfo.GetValueParser<decimal>("n_lain", 0).ToString();
                txAlasan.Text = dicResultInfo.GetValueParser<string>("v_alasan", string.Empty);
                txPph.Text = dicResultInfo.GetValueParser<decimal>("n_pph", 0).ToString();
                txTotBE.Text = dicResultInfo.GetValueParser<decimal>("n_be", 0).ToString();
                txKet.Text = dicResultInfo.GetValueParser<string>("v_ket", string.Empty);
                hfIsNpwp.Text = dicResultInfo.GetValueParser<string>("l_npwp", string.Empty).ToLower();

                jarr.Clear();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
              string.Concat("transaksi_pengiriman__FakturEkspedisiCtrl:PopulateDetail Header - ", ex.Message));
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

            hfFaktur.Text = pID;
            X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
              string.Concat("transaksi_pengiriman__FakturEkspedisiCtrl:PopulateDetail Detail - ", ex.Message));
        }

        #endregion

        winDetail.Hidden = false;
        winDetail.ShowModal();

        btnPrint.Hidden = false;

        GC.Collect();
    }

//    private void InsertDetail(string exp, string resi, string gdg, bool add, string gridValues)
//    {
//        Dictionary<string, object> dicEXP = null;

//        Dictionary<string, string> dicEXPInfo = null;
//        Newtonsoft.Json.Linq.JArray jarr = null;
//        bool isNotDouble = false,
//          isNotDouble2 = false;
//        System.Text.StringBuilder sb = new System.Text.StringBuilder();
//        Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();


//        string tmp = null;
//        tmp = gridDetail.GetStore().ClientID;

//        string sResi = null;
//        string sFaktur = null;
//        string sCunam = null;
//        string sCusno = null;

//        decimal nBilva = 0;
//        decimal n_Net = 0;
//        decimal nSelisih = 0;
//        decimal nJumlah = 0;
//        decimal nDisc = 0;

//        string typebiaya = null;

//        //typebiaya = cbTipeBiaya.SelectedItem.Value == null ? "01" : cbTipeBiaya.SelectedItem.Value;

//        string[][] paramX = new string[][]{
//              new string[] { string.Format("{0}", "c_exp"), exp, "System.String"},
//              new string[] { string.Format("{0}", "c_resi"), resi, "System.String"},
//              new string[] { string.Format("{0}", "c_gdg"), gdg, "System.String"},
//              //new string[] { string.Format("{0}", "tipebiaya"), typebiaya, "System.String"}
//            };

//        string res = soa.GlobalQueryService(0, -1, false, string.Empty, string.Empty, "0314", paramX);
//        dicEXP = JSON.Deserialize<Dictionary<string, object>>(res);


//        #region Parser Detail

//        try
//        {
//            dicEXP = JSON.Deserialize<Dictionary<string, object>>(res);

//            foreach (KeyValuePair<string, object> kvp in dicEXP)
//            {
//                if (kvp.Key == "records")
//                {
//                    object s = kvp.Value;

//                    Dictionary<string, string>[] dicEXP1 = JSON.Deserialize<Dictionary<string, string>[]>(s.ToString());

//                    foreach (Dictionary<string, string> dics in dicEXP1)
//                    {
//                        foreach (KeyValuePair<string, string> kvps in dics)
//                        {
//                            if (kvps.Key.Equals("c_ieno"))
//                            {
//                                sResi = (string)kvps.Value;
//                            }

//                            if (kvps.Key.Equals("c_ie"))
//                            {
//                                sFaktur = (string)kvps.Value;
//                            }

//                            if (kvps.Key.Equals("n_koli"))
//                            {
//                                nBilva = decimal.Parse(kvps.Value);
//                            }

//                            if (kvps.Key.Equals("n_berat"))
//                            {
//                                n_Net = decimal.Parse(kvps.Value);
//                            }

//                            if (kvps.Key.Equals("n_vol"))
//                            {
//                                nSelisih = decimal.Parse(kvps.Value);
//                            }

//                            if (kvps.Key.Equals("n_totalcost"))
//                            {
//                                nJumlah = decimal.Parse(kvps.Value);
//                            }

//                            if (kvps.Key.Equals("n_disc"))
//                            {
//                                nDisc = decimal.Parse(kvps.Value);
//                            }
//                        }


//                        sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
//                              'c_resi': '{1}',
//                              'c_expno': '{2}',
//                              'v_cunam': '{3}',
//                              'c_cusno': '{4}',
//                              'n_koli': {5},
//                              'n_berat': {6},
//                              'n_vol': {7},
//                              'c_tipebiaya' : '{8}',
//                              'n_totalcost': {9},
//                              'n_disc': {10},
//                              'l_new': true,
//                              'l_modified': false,
//                              'l_void': false,
//                            }})); 
//                            ", gridDetail.GetStore().ClientID,
//                               sResi, sFaktur, sCunam, sCusno, nBilva, n_Net, nSelisih, nJumlah, nDisc);

//                    }
//                }
//            }

//            sb.AppendFormat("recalculateFaktur({0});", tmp);

//            X.AddScript(sb.ToString());
//        }
//        catch (Exception ex)
//        {
//            System.Diagnostics.Debug.WriteLine(
//              string.Concat("transaksi_FakturEkspedisi:PopulateDetail Detail - ", ex.Message));
//        }
//        finally
//        {
//            if (jarr != null)
//            {
//                jarr.Clear();
//            }
//            if (dicEXPInfo != null)
//            {
//                dicEXPInfo.Clear();
//            }
//            if (dicEXP != null)
//            {
//                dicEXP.Clear();
//            }
//            sb.Length = 0;
//        }

//        #endregion
//    }

    #endregion

    public void Initialize(string storeIDGridMain)
    {
        hfStoreID.Text = storeIDGridMain;
    }

    public void CommandPopulate(bool isAdd, string pID)
    {
        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
        hfGdg.Text = pag.ActiveGudang;
        hfGdgDesc.Text = pag.ActiveGudangDescription;

        if (isAdd)
        {
            ClearEntrys();

            winDetail.Title = "Faktur Ekspedisi";

            winDetail.Hidden = false;
            winDetail.ShowModal();
        }
        else
        {
            PopulateDetail("c_beno", pID);
        }
    }

    [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void SaveBtn_Click(object sender, DirectEventArgs e)
    {
        string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
        string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
        string gdg = (e.ExtraParams["Gudang"] ?? string.Empty);
        string ekspedisi = (e.ExtraParams["Ekspedisi"] ?? string.Empty);

        bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

        Dictionary<string, string>[] gridData = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

        PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gdg, gridData);

        if (respon.IsSet)
        {
            if (respon.Response == PostDataParser.ResponseStatus.Success)
            {
                string scrpt = null;
                string storeId = hfStoreID.Text;

                string dateJs = null;
                DateTime date = DateTime.Today;

                string eksp = (cbEksHdr.SelectedItem != null ? cbEksHdr.SelectedItem.Text : string.Empty);
                string nip = ((Scms.Web.Core.PageHandler)this.Page).Nip;

                if (respon.Values != null)
                {
                    if (!string.IsNullOrEmpty(storeId))
                    {
                        if (isAdd)
                        {
                            if (respon.Values != null)
                            {
                                if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
                                {
                                    dateJs = Functional.DateToJson(date);
                                }

                                if (!string.IsNullOrEmpty(storeId))
                                {
                                    scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                                            'v_gdgdesc': '{7}',
                                            'c_beno': '{1}',
                                            'd_bedate': {2},
                                            'v_nama_exp': '{3}',
                                            'n_be': '{4}',
                                            'v_ket': '{5}',
                                            'c_entry': '{6}'
                                          }}));{0}.commitChanges();", storeId,
                                             respon.Values.GetValueParser<string>("Faktur", string.Empty),
                                             dateJs, eksp, txTotBE.Text, txKet.Text, nip, hfGdgDesc.Text);

                                    X.AddScript(scrpt);
                                }
                            }
                        }
                    }
                }

                PopulateDetail("c_beno", respon.Values.GetValueParser<string>("Faktur", string.Empty));

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

        if (!pag.IsAllowPrinting)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
            return;
        }

        string gdgId = pag.ActiveGudang;
        string numberID = (e.ExtraParams["NumberID"] ?? string.Empty);

        if (string.IsNullOrEmpty(numberID))
        {
            Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");

            return;
        }

        ReportParser rptParse = new ReportParser();

        List<ReportParameter> lstRptParam = new List<ReportParameter>();

        List<string> lstData = new List<string>();

        rptParse.ReportingID = "10118";
        rptParse.IsLandscape = true;

        #region Report Parameter

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "LG_BEH.c_beno",
            ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID)
        });

        #endregion

        #region Linq Filter Parameter

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "c_beno = @0",
            ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID),
            IsLinqFilterParameter = true
        });

        #endregion

        rptParse.PaperID = "Letter";
        rptParse.ReportParameter = lstRptParam.ToArray();
        rptParse.User = pag.Nip;
        rptParse.UserDefinedName = numberID;

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
                  rptResult.OutputFile, "Faktur Biaya Ekspedisi ", rptResult.Extension);

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
    protected void BtnCalcBayar(object sebder, DirectEventArgs e)
    {
        decimal result = 0;
        result = decimal.Parse(txInvFisik.Text);

        if (decimal.Parse(txClaim.Text) > 0)
            result -= decimal.Parse(txClaim.Text);
        if (decimal.Parse(txPinalty.Text) > 0)
            result -= decimal.Parse(txPinalty.Text);

        //if (decimal.Parse(txClaim.Text) > 0 && decimal.Parse(txPinalty.Text) > 0)
        //{
        //    result = decimal.Parse(txInvFisik.Text) - (decimal.Parse(txClaim.Text) + decimal.Parse(txPinalty.Text));
        //}

        if (decimal.Parse(txLain.Text) > 0)
            result += decimal.Parse(txLain.Text);

        if (decimal.Parse(txPph.Text) > 0)
            result -= decimal.Parse(txPph.Text);
        txTotBE.Text = result.ToString();
    }

    [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void BtnCalcSisa(object sebder, DirectEventArgs e)
    {
        double maxValue = double.Parse(hfMaxValue.Text),
            result = 0;

        if (maxValue > 0)
        {
            result = maxValue - double.Parse(txNilai.Text);
            txSisa.Text = result.ToString();
        }
        else
            txSisa.Text = "0"; ;
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