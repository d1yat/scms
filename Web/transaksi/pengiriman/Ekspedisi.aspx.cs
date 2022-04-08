using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using Scms.Web.Core;
using System.Linq;
using ScmsSoaLibraryInterface.Components;
using System.Collections;
using System.Data.Odbc;

public partial class transaksi_pengiriman_Ekspedisi : Scms.Web.Core.PageHandler
{
    #region Private

    private void ClearEntrys()
    {
        winDetail.Title = "Ekspedisi";

        DateTime date = DateTime.Now;

        hfExpNo.Clear();
        cbasal.Clear();
        cbasal.Hidden = true;
        cbasal.Disabled = false;
        cbCustomerHdr.Clear();
        cbCustomerHdr.Disabled = false;
        Ext.Net.Store cbCustomerHdrstr = cbCustomerHdr.GetStore();
        if (cbCustomerHdrstr != null)
        {
            cbCustomerHdrstr.RemoveAll();
        }

        cbByHdr.Clear();
        cbByHdr.Disabled = false;
        Ext.Net.Store cbByHdrstr = cbByHdr.GetStore();
        if (cbByHdrstr != null)
        {
            cbByHdrstr.RemoveAll();
        }

        cbEksHdr.Clear();
        cbEksHdr.Disabled = true;
        Ext.Net.Store cbEksHdrstr = cbEksHdr.GetStore();
        if (cbEksHdrstr != null)
        {
            cbEksHdrstr.RemoveAll();
        }
        txExp.Clear();
        txExp.Hidden = true;

        cbViaHdr.Clear();
        cbViaHdr.Disabled = false;
        Ext.Net.Store cbViaHdrstr = cbViaHdr.GetStore();
        if (cbViaHdrstr != null)
        {
            cbViaHdrstr.RemoveAll();
        }

        cbDriver.Clear();
        cbDriver.Disabled = false;
        Ext.Net.Store cbDriverstr = cbDriver.GetStore();
        if (cbDriverstr != null)
        {
            cbDriverstr.RemoveAll();
        }
        txNoPol.Clear();
        txNoPol.Disabled = false;

        txNoResiHdr.Clear();
        txNoResiHdr.Disabled = false;

        txDayResiHdr.Clear();
        txDayResiHdr.Disabled = false;

        txBerat.Clear();
        txBerat.Disabled = true;

        txDayResiHdr.Clear();
        txDayResiHdr.Text = date.ToString("dd-MM-yyyy");
        txDayResiHdr.Disabled = false;

        txTimeResiHdr.Clear();
        txTimeResiHdr.Text = date.ToString("HH:mm:ss");
        txTimeResiHdr.Disabled = false;

        txKoli.Clear();
        txKoli.Disabled = true;

        txReceh.Clear();
        txReceh.Disabled = true;

        txVol.Clear();
        txVol.Disabled = true;

        txKetHdr.Clear();
        txKetHdr.Disabled = false;

        txReprint.Clear();
        txReprint.Hidden = true;

        cbDODtl.Clear();
        cbDODtl.Disabled = false;

        cbTipeKrmHdr.Clear();
        cbTipeKrmHdr.Disabled = false;
        Ext.Net.Store cbTipeKrmHdrstr = cbTipeKrmHdr.GetStore();
        if (cbTipeKrmHdrstr != null)
        {
            cbTipeKrmHdrstr.RemoveAll();
        }

        cbRefHo.Clear();
        cbRefHo.Disabled = false;

        txBeratVol.Clear();
        txBeratVol.Disabled = true;

        txBiayaLain.Text = "0";
        txBiayaLain.Disabled = false;

        lbTotalBiaya.Text = "0";

        txBiayaExp.Text = "0";
        txBiayaExp.Disabled = true;
        lbMinExp.Text = "0";

        Ext.Net.Store cbDODtlstr = cbDODtl.GetStore();
        if (cbDODtlstr != null)
        {
            cbDODtlstr.RemoveAll();
        }

        cbWPDtl.Clear();
        cbWPDtl.Disabled = false;
        Ext.Net.Store cbWPDtlstr = cbWPDtl.GetStore();
        if (cbWPDtlstr != null)
        {
            cbWPDtlstr.RemoveAll();
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
    }

    private void PopulateDetail(string pName, string pID)
    {
        winDetail.Title = "";
        Dictionary<string, object> dicEXP = null;
        Dictionary<string, string> dicEXPInfo = null;
        Newtonsoft.Json.Linq.JArray jarr = null;

        Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

        string[][] paramX = new string[][]{
              new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
            };

        //string tmp = null;

        DateTime date = DateTime.MinValue;

        //gridDetail2.ColumnModel.SetEditable(1, false);
        //gridDetail2.ColumnModel.SetEditable(2, false);
        //gridDetail2.ColumnModel.SetEditable(3, false);

        //gridDetail2.ColumnModel.SetEditable(1, true);
        //gridDetail2.ColumnModel.SetEditor(1, new Ext.Net.NumberField() { AllowDecimals = true, AllowBlank = false, AllowNegative = false, MinValue = 0.00 });
        gridDetail2.ColumnModel.SetEditable(2, true);
        gridDetail2.ColumnModel.SetEditor(2, new Ext.Net.NumberField() { AllowDecimals = true, AllowBlank = false, AllowNegative = false, MinValue = 0.00 });
        gridDetail2.ColumnModel.SetEditable(3, true);
        gridDetail2.ColumnModel.SetEditor(3, new Ext.Net.NumberField() { AllowDecimals = true, AllowBlank = false, AllowNegative = false, MinValue = 0.00 });

        #region Parser Header

        string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0005", paramX);

        try
        {
            dicEXP = JSON.Deserialize<Dictionary<string, object>>(res);
            if (dicEXP.ContainsKey("records") && (dicEXP.ContainsKey("totalRows") && (((long)dicEXP["totalRows"]) > 0)))
            {
                jarr = new Newtonsoft.Json.Linq.JArray(dicEXP["records"]);

                dicEXPInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

                winDetail.Title = string.Concat("Ekspedisi - ", pID);

                //cbByHdr.ToBuilder().AddItem(
                //    dicEXPInfo.ContainsKey("v_ketBy") ? dicEXPInfo["v_ketBy"] : string.Empty,
                //    dicEXPInfo.ContainsKey("c_typeby") ? dicEXPInfo["c_typeby"] : string.Empty
                //    );
                //if (cbByHdr.GetStore() != null)
                //{
                //  cbByHdr.GetStore().CommitChanges();
                //}
                //cbByHdr.SetValueAndFireSelect((dicEXPInfo.ContainsKey("c_typeby") ? dicEXPInfo["c_typeby"] : string.Empty));
                //cbByHdr.Disabled = true;
                Functional.SetComboData(cbViaHdr, "c_typeby", dicEXPInfo.GetValueParser<string>("v_ketBy", string.Empty), dicEXPInfo.GetValueParser<string>("c_typeby", string.Empty));
                cbViaHdr.Disabled = false;

                Functional.SetComboData(cbasal, "c_asal", dicEXPInfo.GetValueParser<string>("v_asal", string.Empty), dicEXPInfo.GetValueParser<string>("c_asal",string.Empty));
                cbasal.Disabled = true;

                Functional.SetComboData(cbCustomerHdr, "c_cusno", dicEXPInfo.GetValueParser<string>("v_cunam", string.Empty), dicEXPInfo.GetValueParser<string>("c_cusno", string.Empty));
                cbCustomerHdr.Disabled = true;

                Functional.SetComboData(cbByHdr, "c_typeby", dicEXPInfo.GetValueParser<string>("v_by_desc", string.Empty), dicEXPInfo.GetValueParser<string>("c_typeby", string.Empty));
                cbByHdr.Disabled = true;

                Functional.SetComboData(cbEksHdr, "c_exp", dicEXPInfo.GetValueParser<string>("v_ketMsek", string.Empty), dicEXPInfo.GetValueParser<string>("c_exp", string.Empty));
                cbEksHdr.Disabled = false;

                txExp.Text = ((dicEXPInfo.ContainsKey("v_exp") ? dicEXPInfo["v_exp"] : string.Empty));

                Functional.SetComboData(cbViaHdr, "c_via", dicEXPInfo.GetValueParser<string>("v_ketTran", string.Empty), dicEXPInfo.GetValueParser<string>("c_via", string.Empty));
                cbViaHdr.Disabled = false;

                Functional.SetComboData(cbDriver, "c_driver", dicEXPInfo.GetValueParser<string>("c_driver", string.Empty), dicEXPInfo.GetValueParser<string>("c_driver", string.Empty));

                txNoPol.Text = ((dicEXPInfo.ContainsKey("c_nopol") ? dicEXPInfo["c_nopol"] : string.Empty));

                txKetHdr.Text = ((dicEXPInfo.ContainsKey("v_ket") ? dicEXPInfo["v_ket"] : string.Empty));

                txNoResiHdr.Text = ((dicEXPInfo.ContainsKey("c_resi") ? dicEXPInfo["c_resi"] : string.Empty));

                date = Functional.JsonDateToDate(dicEXPInfo.GetValueParser<string>("d_resi"));

                txDayResiHdr.Text = date.ToString("dd-MM-yyyy");
                txTimeResiHdr.Text = date.ToString("HH:mm:ss"); //dicEXPInfo.GetValueParser<string>("t_resi");

                Functional.SetComboData(cbRefHo, "c_ref", dicEXPInfo.GetValueParser<string>("c_ref", string.Empty), dicEXPInfo.GetValueParser<string>("c_ref", string.Empty));
                cbRefHo.Disabled = false;
                
                Functional.SetComboData(cbTipeKrmHdr, "c_exptype", dicEXPInfo.GetValueParser<string>("v_ket_ref", string.Empty), dicEXPInfo.GetValueParser<string>("c_exptype", string.Empty));
                cbTipeKrmHdr.Disabled = false;

                txBiayaLain.Text = "0";
                txBiayaLain.Disabled = false;

                txBeratVol.Text = ((dicEXPInfo.ContainsKey("n_biayalain") ? dicEXPInfo["n_biayalain"] : string.Empty));
                txBeratVol.Disabled = true;

                lbTotalBiaya.Text = ((dicEXPInfo.ContainsKey("n_totalbiaya") ? dicEXPInfo["n_totalbiaya"] : string.Empty));

                txBiayaExp.Text = ((dicEXPInfo.ContainsKey("n_biayakg") ? dicEXPInfo["n_biayakg"] : string.Empty));

                lbMinExp.Text = ((dicEXPInfo.ContainsKey("n_expmin") ? dicEXPInfo["n_expmin"] : string.Empty));

                DateTime date1 = DateTime.Now.AddDays(-4);
                if (date1 > date)
                {
                    //cbRefHo.Disabled = true;
                    //cbViaHdr.Disabled = true;
                    //cbTipeKrmHdr.Disabled = true;
                    //cbEksHdr.Disabled = true;
                }

                //tmp = (dicEXPInfo.ContainsKey("d_resi") ? dicEXPInfo["d_resi"] : string.Empty);
                //tmp = tmp.Substring(tmp.IndexOf("(") + 1, tmp.IndexOf(")") - tmp.IndexOf("(") - 1);

                //string tmp1 = dicEXPInfo["t_resi"];

                //long e = long.Parse(tmp);

                //tmp = date.ToString();

                //txDayResiHdr.Text = ((tmp));
                //txTimeResiHdr.Text = ((tmp1));

                #region tanggal dan waktu
                //DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

                //tmp = dicEXPInfo["d_resi"];
                //tmp = tmp.Substring(tmp.IndexOf("(") + 1, tmp.IndexOf(")") - tmp.IndexOf("(") - 1 );
                //DateTime de = new DateTime(Convert.ToInt64(tmp));
                //DateTime de1 = new DateTime().ToUniversalTime();

                //TimeSpan ts = new TimeSpan(de.Ticks - de1.Ticks);

                //CultureInfo provider = CultureInfo.InvariantCulture;
                //string format = "dd/MM/yyyy";

                //DateTime dt = DateTime.ParseExact(tmp, format, provider);

                //var utcDate = DateTime.Now.ToUniversalTime();
                //var Uda = Convert.ToInt64(tmp);
                //DateTime de = new DateTime(Convert.ToInt64(tmp)).ToLocalTime();
                //de.Subtract(UnicodeCategory)
                //long baseTicks = 621355968000000000;
                //long tickResolution = 10000000;
                //long epoch = (de.Ticks - baseTicks) / tickResolution;
                //long epochTicks = (epoch * tickResolution) + baseTicks;
                //string date = new DateTime(epochTicks, DateTimeKind.Utc).ToString();
                //string tbEpoch = epoch.ToString();
                //string tbDate1 = de.ToString();
                //string tbDate2 = date.ToString();


                //string date = new DateTime(epochTicks, DateTimeKind.Utc).ToString();

                #endregion

                txKoli.Text = dicEXPInfo.GetValueParser<decimal>("n_koli").ToString(); //((dicEXPInfo.ContainsKey("n_koli") ? dicEXPInfo["n_koli"] : string.Empty));
                txReceh.Text = dicEXPInfo.GetValueParser<decimal>("n_receh").ToString();
                txBerat.Text = dicEXPInfo.GetValueParser<decimal>("n_berat").ToString(); ////((dicEXPInfo.ContainsKey("n_berat") ? dicEXPInfo["n_berat"] : string.Empty));
                txVol.Text = dicEXPInfo.GetValueParser<decimal>("n_vol").ToString(); ////((dicEXPInfo.ContainsKey("n_berat") ? dicEXPInfo["n_berat"] : string.Empty));

                hfPrint.Text = ((dicEXPInfo.ContainsKey("l_print") ? dicEXPInfo["l_print"] : string.Empty));
                
                //X.AddScript(string.Format("clearRelatedComboRecursive(true, {0});", cbDODtl.ClientID));

                jarr.Clear();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
              string.Concat("transaction_sales_Expedisi:PopulateDetail Header - ", ex.Message));
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
        }


        #endregion

        #region Parser Detail

        try
        {
            Ext.Net.Store store = gridDetail.GetStore();
            Ext.Net.Store store2 = gridDetail2.GetStore();
            if (store2 != null)
            {
                store2.RemoveAll();
            }

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

            hfExpNo.Text = pID;
            X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
            //X.AddScript(string.Format("{0}.removeAll();{0}.reload();", Store2.ClientID));

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
              string.Concat("transaction_sales_Expedisi:PopulateDetail Detail - ", ex.Message));
        }

        #endregion

        #region Detail 2

        string res1 = soa.GlobalQueryService(0, -1, false, string.Empty, string.Empty, "03106", paramX);


        try
        {
            dicEXP = JSON.Deserialize<Dictionary<string, object>>(res1);

            System.Text.StringBuilder sb = null;

            double dKoli = 0;
            double dReceh = 0;
            double dBerat = 0;
            double dVol = 0;

            foreach (KeyValuePair<string, object> kvp in dicEXP)
            {
                if (kvp.Key == "records")
                {
                    object s = kvp.Value;

                    Dictionary<string, string>[] dicEXP1 = JSON.Deserialize<Dictionary<string, string>[]>(s.ToString());

                    foreach (Dictionary<string, string> dics in dicEXP1)
                    {
                        string n_koli = (string)dics["n_koli"];
                        string n_receh = (string)dics["n_receh"];
                        string n_berat = (string)dics["n_berat"];
                        string n_vol = (string)dics["n_vol"];
                        string expNo = (string)dics["c_expno"];
                        string cPart = (string)dics["c_nopart"];

                        dKoli = double.Parse(n_koli);
                        dReceh = double.Parse(n_receh);
                        dBerat = double.Parse(n_berat);
                        dVol = string.IsNullOrEmpty(n_vol) ? 0 : double.Parse(n_vol);

                        sb = new System.Text.StringBuilder();
                        sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                              'n_koli': {1},
                              'n_receh': {6},
                              'n_berat': {2},
                              'n_vol': {5},
                              'c_expno': '{3}',
                              'c_nopart': '{4}'
                            }})); ", gridDetail2.GetStore().ClientID,
                                              dKoli, dBerat, expNo, cPart, dVol, dReceh);

                        X.AddScript(sb.ToString());
                    }
                }
            }

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
              string.Concat("transaction_sales_Expedisi:PopulateDetail Detail - ", ex.Message));
        }

        #endregion


        winDetail.Hidden = false;
        winDetail.ShowModal();

        GC.Collect();

    }

    private PostDataParser.StructureResponse DeleteParser(string eksNumber, string ket)
    {
        PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

        PostDataParser parser = new PostDataParser();
        IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

        PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

        pair.IsSet = true;
        pair.IsList = true;
        pair.Value = eksNumber;
        pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
        pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        string varData = null;

        dic.Add("ID", pair);
        pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);

        pair.DicAttributeValues.Add("Ket", ket.Trim());

        try
        {
            varData = parser.ParserData("Ekspedisi", "Delete", dic);
        }
        catch (Exception ex)
        {
            Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_ekspedisi SaveParser : {0} ", ex.Message);
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

    private PostDataParser.StructureResponse SaveParser(bool isAdd, bool isConfirm, string plNumber, Dictionary<string, string>[] dics, Dictionary<string, string>[] dics2)
    {
        PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

        PostDataParser parser = new PostDataParser();
        IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

        Dictionary<string, string> dicData = null;
        Dictionary<string, string> dicData2 = null;

        PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

        Dictionary<string, string> dicAttr = null;

        string tmp = null,
        dono = null,
        ket = null,
        tmp2 = null,
        partNo = null;
        bool isNew = false,
          isVoid = false,
          isModify = false;
        string varData = null,
          kodeExp = null;

        decimal nKoli = 0,
            nReceh = 0,
          nBerat = 0,
          nVol = 0;

        int iHit = 0;

        DateTime tanggal = DateTime.MinValue,
          tanggalResi = DateTime.MinValue;

        //if (txBeratVol.Text == "0")
        //{
            if (cbViaHdr.SelectedItem.Value == "02" || cbViaHdr.SelectedItem.Value == "03")
            {
                decimal beratkg = Convert.ToDecimal(txBerat.Text);
                decimal beratvol = Convert.ToDecimal(txVol.Text) * 250;
                decimal totbiayavol = beratvol * Convert.ToDecimal(txBiayaExp.Text);
                decimal totbiayakg = beratkg * Convert.ToDecimal(txBiayaExp.Text);
                txBeratVol.Text = beratvol.ToString();
                if (totbiayavol > totbiayakg)
                {
                    lbTotalBiaya.Text = totbiayavol.ToString();
                }
                else
                {
                    lbTotalBiaya.Text = totbiayakg.ToString();
                }
            }
            else if(cbViaHdr.SelectedItem.Value == "01" || cbViaHdr.SelectedItem.Value == "10")
            {
                double beratvol = Convert.ToDouble(txVol.Text) * 166.66667;
                double beratkg = Convert.ToDouble(txBerat.Text);
                double totbiayavol = beratvol * Convert.ToDouble(txBiayaExp.Text);
                double totbiayakg = beratkg * Convert.ToDouble(txBiayaExp.Text);
                txBeratVol.Text = beratvol.ToString();
                if (totbiayavol > totbiayakg)
                {
                    lbTotalBiaya.Text = totbiayavol.ToString();
                }
                else
                {
                    lbTotalBiaya.Text = totbiayakg.ToString();
                }
            }
        //}

        pair.IsSet = true;
        pair.IsList = true;
        pair.Value = plNumber;
        pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
        pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dic.Add("ID", pair);
        pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
        if (cbByHdr.SelectedItem.Value == "04" || cbByHdr.SelectedItem.Value == "4")
        {
            kodeExp = cbEksHdr.SelectedItem.Value;
        }
        else
        {
            kodeExp = (cbByHdr.Text.Equals("01", StringComparison.OrdinalIgnoreCase) ? cbEksHdr.Text : "00");
        }
        pair.DicAttributeValues.Add("Via", cbViaHdr.Text);
        pair.DicAttributeValues.Add("Customer", cbCustomerHdr.Text);
        pair.DicAttributeValues.Add("By", cbByHdr.Text);
        pair.DicAttributeValues.Add("Eks", kodeExp);
        pair.DicAttributeValues.Add("EksPlus", txExp.Text.Trim());
        pair.DicAttributeValues.Add("Ket", txKetHdr.Text.Trim());
        pair.DicAttributeValues.Add("Resi", txNoResiHdr.Text.Trim());
        pair.DicAttributeValues.Add("Koli", txKoli.Text.Trim());
        pair.DicAttributeValues.Add("Receh", txReceh.Text.Trim());
        pair.DicAttributeValues.Add("Berat", txBerat.Text.Trim());
        pair.DicAttributeValues.Add("Volume", txVol.Text.Trim());
        pair.DicAttributeValues.Add("Gudang", hfGdg.Text.Trim());
        pair.DicAttributeValues.Add("Tipe", "01");
        pair.DicAttributeValues.Add("Driver", cbDriver.SelectedItem.Text);
        pair.DicAttributeValues.Add("Nopol", txNoPol.Text.Trim());
        pair.DicAttributeValues.Add("AsalKirim", cbasal.SelectedItem.Value); //penambahan untuk direct shipment 8 Juni 2018 by Suwandi
        pair.DicAttributeValues.Add("Ref", cbRefHo.SelectedItem.Value == null ? "" : cbRefHo.SelectedItem.Value);
        pair.DicAttributeValues.Add("TipeExp", cbTipeKrmHdr.SelectedItem.Value);
        pair.DicAttributeValues.Add("BiayaLain", txBeratVol.Text.Trim());
        pair.DicAttributeValues.Add("TotalBiaya", lbTotalBiaya.Text.Trim());
        pair.DicAttributeValues.Add("BiayaKg", txBiayaExp.Text.Trim());
        pair.DicAttributeValues.Add("expMin", lbMinExp.Text.Trim());

        System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("id-ID");

        string tglResi = string.Concat(txDayResiHdr.RawValue, " ", txTimeResiHdr.RawText);

        bool isValid = DateTime.TryParse(tglResi, ci, System.Globalization.DateTimeStyles.AssumeLocal, out tanggal);
        if (!isValid)
        {
            tanggal = DateTime.MinValue;
        }

        tanggalResi = tanggal;

        //if (!Functional.DateParser(string.Concat(txDayResiHdr.Format.ToString(), " ", txTimeResiHdr.RawText),
        //  "d-M-y H:m:s", out tanggal))
        //{
        //  tanggalResi = tanggal;
        //}

        pair.DicAttributeValues.Add("DResi", tanggal.ToString("yyyyMMddHHmmss"));

        for (int nLoop = 0, nLen = dics2.Length; nLoop < nLen; nLoop++)
        {
            dicData2 = dics2[nLoop];
            tmp = nLoop.ToString();

            isNew = dicData2.GetValueParser<bool>("l_new");
            isVoid = dicData2.GetValueParser<bool>("l_void");
            isModify = dicData2.GetValueParser<bool>("l_modified");

            if ((!isNew) && (!isVoid) && isModify)
            {
                nKoli = dicData2.GetValueParser<decimal>("n_koli", 0);
                nReceh = dicData2.GetValueParser<decimal>("n_receh", 0);
                nBerat = dicData2.GetValueParser<decimal>("n_berat", 0);
                nVol = dicData2.GetValueParser<decimal>("n_vol", 0);
                partNo = dicData2.GetValueParser<string>("c_nopart", string.Empty);

                dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                dicAttr.Add("Modified", isModify.ToString().ToLower());
                dicAttr.Add("koli", nKoli.ToString());
                dicAttr.Add("receh", nReceh.ToString());
                dicAttr.Add("berat", nBerat.ToString());
                dicAttr.Add("volume", nVol.ToString());
                dicAttr.Add("partno", partNo);

                pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
                {
                    IsSet = true,
                    DicAttributeValues = dicAttr
                });
            }

            if ((!isNew) && isVoid && (!isModify))
            {
                ket = dicData2.GetValueParser<string>("v_ket");
                nKoli = dicData2.GetValueParser<decimal>("n_koli", 0);
                nReceh = dicData2.GetValueParser<decimal>("n_receh", 0);
                nBerat = dicData2.GetValueParser<decimal>("n_berat", 0);
                nVol = dicData2.GetValueParser<decimal>("n_vol", 0);
                partNo = dicData2.GetValueParser<string>("c_nopart", string.Empty);

                foreach (Dictionary<string, string> dicStr in dics)
                {
                    string part = (string)dicStr["c_nopart"];

                    if (part.Equals(partNo))
                    {
                        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        dicAttr.Add("New", isNew.ToString().ToLower());
                        dicAttr.Add("Delete", isVoid.ToString().ToLower());
                        dicAttr.Add("Modified", isModify.ToString().ToLower());

                        tmp2 = iHit.ToString();
                        dicAttr.Add("dono", dicStr.GetValueParser<string>("c_dono"));
                        dicAttr.Add("koli", nKoli.ToString());
                        dicAttr.Add("receh", nReceh.ToString());
                        dicAttr.Add("berat", nBerat.ToString());
                        dicAttr.Add("volume", nVol.ToString());
                        dicAttr.Add("partno", partNo);
                        pair.DicValues.Add(tmp2, new PostDataParser.StructurePair()
                        {
                            IsSet = true,
                            Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
                            DicAttributeValues = dicAttr
                        });
                        iHit++;
                    }
                }
            }
        }

        for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
        {
            if (string.IsNullOrEmpty(tmp))
            {
                tmp = nLoop.ToString();
            }
            else
            {
                int i = int.Parse(tmp.ToString()) + 1;
                tmp = i.ToString();
            }

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

                dono = dicData.GetValueParser<string>("c_dono");

                partNo = dicData.GetValueParser<string>("c_nopart", string.Empty);

                foreach (Dictionary<string, string> dicStr in dics2)
                {
                    string part = (string)dicStr["c_nopart"];

                    if (part.Equals(partNo))
                    {
                        nKoli = dicStr.GetValueParser<decimal>("n_koli", 0);
                        nReceh = dicStr.GetValueParser<decimal>("n_receh", 0);
                        nBerat = dicStr.GetValueParser<decimal>("n_berat", 0);
                        nVol = dicStr.GetValueParser<decimal>("n_vol", 0);
                        break;
                    }
                }


                if (!string.IsNullOrEmpty(dono))
                {
                    dicAttr.Add("dono", dono);
                    dicAttr.Add("koli", nKoli.ToString());
                    dicAttr.Add("receh", nReceh.ToString());
                    dicAttr.Add("berat", nBerat.ToString());
                    dicAttr.Add("volume", nVol.ToString());
                    dicAttr.Add("partno", partNo);

                    pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
                    {
                        IsSet = true,
                        DicAttributeValues = dicAttr
                    });
                }
            }

        }

        try
        {
            if (isConfirm)
            {
                varData = parser.ParserData("Ekspedisi", "ModifyConfirm", dic);
            }
            else
            {
                varData = parser.ParserData("Ekspedisi", (isAdd ? "Add" : "Modify"), dic);
            }
        }
        catch (Exception ex)
        {
            Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_EkspedisiCtrl SaveParser : {0} ", ex.Message);
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

    private void CalculateKoliBerat(string sNo, bool add, string gridValues, string cusno, string gridValues2)
    {
        Dictionary<string, object> dicEXP = null;

        Dictionary<string, string> dicEXPInfo = null;
        Newtonsoft.Json.Linq.JArray jarr = null;
        bool isNotDouble = false,
          isNotDouble2 = false;
        //bool isValid = string.IsNullOrEmpty(gridValues);
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

        decimal dKoli = 0;
        decimal dReceh = 0;
        decimal dBerat = 0;
        decimal dVol = 0;
        decimal decKoli = 0;
        decimal decReceh = 0;
        decimal decBerat = 0;
        decimal decVol = 0;

        string sValue = null;

        string[][] paramX = new string[][]{
              new string[] { string.Format("{0}", "c_no"), sNo, "System.String"},
              new string[] { string.Format("{0}", "cusno"), cusno, "System.String"}
            };

        string res = soa.GlobalQueryService(0, -1, false, string.Empty, string.Empty, "0309", paramX);
        dicEXP = JSON.Deserialize<Dictionary<string, object>>(res);


        #region Parser Detail

        try
        {
            dicEXP = JSON.Deserialize<Dictionary<string, object>>(res);
            Dictionary<string, string>[] gridDataExp2 = JSON.Deserialize<Dictionary<string, string>[]>(gridValues2);

            int i = 0;

            if (gridDataExp2.Length > 0)
            {
                foreach (KeyValuePair<string, string> kvp in gridDataExp2[0])
                {
                    if (kvp.Key == "c_nopart")
                    {
                        i = int.Parse(kvp.Value) + 1;
                        hfHitPart.Text = i.ToString();
                    }
                }
            }
            else
            {
                i = gridDataExp2.Length + 1;
                hfHitPart.Text = i.ToString();
            }
            
            foreach (KeyValuePair<string, object> kvp in dicEXP)
            {
                if (kvp.Key == "records")
                {
                    object s = kvp.Value;

                    Dictionary<string, string>[] dicEXP1 = JSON.Deserialize<Dictionary<string, string>[]>(s.ToString());

                    foreach (Dictionary<string, string> dics in dicEXP1)
                    {
                        string part = null;


                        foreach (KeyValuePair<string, string> kvps in dics)
                        {
                            if (kvps.Key.Equals("c_dono"))
                            {
                                sValue = (string)kvps.Value;
                            }

                            if (kvps.Key.Equals("n_berat"))
                            {
                                dBerat = decimal.Parse(kvps.Value);
                            }

                            if (kvps.Key.Equals("n_koli"))
                            {
                                dKoli = decimal.Parse(kvps.Value);
                            }

                            if (kvps.Key.Equals("n_receh"))
                            {
                                dReceh = decimal.Parse(kvps.Value);
                            }

                            if (kvps.Key.Equals("n_vol"))
                            {
                                dVol = decimal.Parse(kvps.Value);
                            }
                        }

                        if (sNo.Substring(0, 2) == "WP")
                        {
                            isNotDouble = gridValues.Contains(sValue);
                        }

                        if (!isNotDouble)
                        {
                            sb = new System.Text.StringBuilder();
                            sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                              'c_dono': '{1}',
                              'c_nopart': '{2}',
                              'n_berat': {3},
                              'n_koli': {4},
                              'n_vol': {5},
                              'n_receh': {6},
                              'l_new': true,
                            }})); ", gridDetail.GetStore().ClientID,
                                                  sValue, i.ToString("000"), dBerat, dKoli, dVol, dReceh);
                            
                            X.AddScript(sb.ToString());
                        }
                        else
                        {
                            Functional.ShowMsgInformation("No. " + sValue + ", Data telah ada.");
                        }
                    }
                }
            }

            //isNotDouble2 = gridValues2.Contains(dKoli.ToString());
            Dictionary<string, string>[] dicEXP2 = null;
            dicEXP2 = JSON.Deserialize<Dictionary<string, string>[]>(gridValues2);

            //foreach (Dictionary<string, string> dics in dicEXP2)
            //{
            //  string n_koli = (string)dics["n_koli"];

            //  if (n_koli.Equals(dKoli.ToString()))
            //  {
            //    isNotDouble = true;
            //  }
            //}

            if (!string.IsNullOrEmpty(dKoli.ToString()) && !string.IsNullOrEmpty(dReceh.ToString()) && !(string.IsNullOrEmpty(dBerat.ToString())) && !(string.IsNullOrEmpty(dVol.ToString())) && !isNotDouble)
            {

                sb = new System.Text.StringBuilder();
                sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                              'n_koli': {1},
                              'n_berat': {2},
                              'n_vol': {4},
                              'c_nopart': '{3}',
                              'n_receh': {5},
                            }})); ", gridDetail2.GetStore().ClientID,
                                      dKoli, dBerat, i.ToString("000"),dVol, dReceh);



                X.AddScript(sb.ToString());
            }
            else
            {
                Functional.ShowMsgInformation("Data telah ada.");
            }

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
              string.Concat("transaction_sales_Expedisi:PopulateDetail Detail - ", ex.Message));
        }

        #endregion

        #region Parser Header
        if (!isNotDouble)
        {
            try
            {

                if (dicEXP.ContainsKey("records") && (dicEXP.ContainsKey("totalRows") && (((long)dicEXP["totalRows"]) > 0)))
                {
                    jarr = new Newtonsoft.Json.Linq.JArray(dicEXP["records"]);


                    decimal totKoli = string.IsNullOrEmpty(txKoli.Text) ? 0 : decimal.Parse(txKoli.Text);
                    decimal totReceh = string.IsNullOrEmpty(txReceh.Text) ? 0 : decimal.Parse(txReceh.Text);
                    decimal totBerat = string.IsNullOrEmpty(txBerat.Text) ? 0 : decimal.Parse(txBerat.Text);
                    decimal totVol = string.IsNullOrEmpty(txVol.Text) ? 0 : decimal.Parse(txVol.Text);
                    decimal dec = 0;
                    decimal udara = 166.66667m;
                    decimal darat = 250;

                    dicEXPInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

                    dKoli = dicEXPInfo.GetValueParser<decimal>("n_koli", 0);
                    dReceh = dicEXPInfo.GetValueParser<decimal>("n_receh", 0);
                    dBerat = dicEXPInfo.GetValueParser<decimal>("n_berat", 0);
                    dVol = dicEXPInfo.GetValueParser<decimal>("n_vol", 0);

                    decKoli = dKoli;
                    decReceh = dReceh;
                    decBerat = dBerat;
                    decVol = dVol;

                    totKoli += dKoli;
                    totReceh += dReceh;
                    totBerat += dBerat;
                    totVol += dVol;
                    if (cbViaHdr.SelectedItem.Value == "10" || cbViaHdr.SelectedItem.Value == "01" )
                    {
                        dec = totVol * udara;
                    }
                    else
                    {
                        dec = totVol * darat;
                    }
                                        
                    txKoli.Text = totKoli.ToString();
                    txReceh.Text = totReceh.ToString();
                    txBerat.Text = totBerat.ToString();
                    txVol.Text = totVol.ToString();
                    txBeratVol.Text = dec.ToString();

                    jarr.Clear();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                  string.Concat("transaction_sales_Expedisi:PopulateDetail Header - ", ex.Message));
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
            }
        }

        #endregion
    }

    private void CalculateTotalBiaya(string gridValues, string exp, string cusno, string tipebiaya)
    {
        Dictionary<string, object> dicEXP = null;
        Dictionary<string, string> dicEXPInfo = null;
        var connectionStringSql = "Driver={SQL Server};Server=10.100.41.29;Database=AMS;Uid=sa;Pwd=4M5M1s2015";
        PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);
        Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();
        Newtonsoft.Json.Linq.JArray jarr = null;

        string[][] paramX = new string[][]{
              new string[] { string.Format("{0}", "exp"), exp, "System.String"},
              new string[] { string.Format("{0}", "cusno"), cusno, "System.String"},
              new string[] { string.Format("{0}", "tipebiaya"), tipebiaya, "System.String"},
              new string[] { string.Format("{0}", "via"), cbViaHdr.SelectedItem.Value.ToString(), "System.String"},
              new string[] { string.Format("{0}", "gdg"), hfGdg.Text, "System.Char"},
              new string[] { string.Format("{0}", "date"), txDayResiHdr.Text.Substring(0,10), "System.String"},
              new string[] { string.Format("{0}", "carakirim"), cbByHdr.SelectedItem.Value.ToString(), "System.String"}
            };

        string res = soa.GlobalQueryService(0, -1, false, string.Empty, string.Empty, "0313", paramX);
        dicEXP = JSON.Deserialize<Dictionary<string, object>>(res);

        decimal nTotalBiaya = 0,
            biayakg = 0,
            beratAktual = 0,
            nBerat = decimal.Parse(txBerat.Text == null ? "0" : txBerat.Text);

        if (Convert.ToDouble(txBerat.Text) < Convert.ToDouble(txBeratVol.Text))
        {
            nBerat = decimal.Parse(txBeratVol.Text == null ? "0" : txBeratVol.Text);
        }

        string nBiayaLain = txBiayaLain.Text == null ? "0" : txBiayaLain.Text;

        #region Parser Detail

        try
        {
            dicEXP = JSON.Deserialize<Dictionary<string, object>>(res);
            Dictionary<string, string>[] gridDataExp = JSON.Deserialize<Dictionary<string, string>[]>(gridValues);

            if (dicEXP.ContainsKey("records") && (dicEXP.ContainsKey("totalRows") && (((long)dicEXP["totalRows"]) > 0)))
            {
                jarr = new Newtonsoft.Json.Linq.JArray(dicEXP["records"]);

                dicEXPInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());
            }
            else
            {
                Functional.ShowMsgInformation("Biaya tidak diketahui, periksa master biaya ekspedisi");
                lbTotalBiaya.Text = "0";
                txBiayaExp.Text = "0";
                return;
            }
            

            decimal expmin = decimal.Parse(dicEXPInfo.GetValueParser<string>("n_expmin", "0"));


            if (expmin > 0 && nBerat < expmin)
            {
                beratAktual = expmin;
            }
            else
            {
                beratAktual = nBerat;
            }

            switch (cbViaHdr.SelectedItem.Value)
            {
                case "01":
                    biayakg = decimal.Parse(dicEXPInfo.GetValueParser<string>("n_udara", "0"));
                    nTotalBiaya = biayakg * beratAktual;
                    break;
                case "02":
                    biayakg = decimal.Parse(dicEXPInfo.GetValueParser<string>("n_daratlaut", "0"));
                    nTotalBiaya = biayakg * beratAktual;
                    break;
                case "03":
                    biayakg = decimal.Parse(dicEXPInfo.GetValueParser<string>("n_icepack", "0"));
                    nTotalBiaya = biayakg * beratAktual;
                    break;
                case "04":
                    biayakg = decimal.Parse(dicEXPInfo.GetValueParser<string>("n_cdd", "0"));
                    nTotalBiaya = biayakg;
                    break;
                case "05":
                    biayakg = decimal.Parse(dicEXPInfo.GetValueParser<string>("n_fuso", "0"));
                    nTotalBiaya = biayakg;
                    break;
                case "06":
                    biayakg = decimal.Parse(dicEXPInfo.GetValueParser<string>("n_tronton", "0"));
                    nTotalBiaya = biayakg;
                    break;
                case "07":
                    biayakg = decimal.Parse(dicEXPInfo.GetValueParser<string>("n_container", "0"));
                    nTotalBiaya = biayakg;
                    break;
                case "08":
                    biayakg = decimal.Parse(dicEXPInfo.GetValueParser<string>("n_cde", "0"));
                    nTotalBiaya = biayakg;
                    break;
                case "09":
                    biayakg = decimal.Parse(dicEXPInfo.GetValueParser<string>("n_l300", "0"));
                    nTotalBiaya = biayakg;
                    break;
                case "10":
                    biayakg = decimal.Parse(dicEXPInfo.GetValueParser<string>("n_icepack", "0"));
                    nTotalBiaya = biayakg * beratAktual;
                    break;
            }

            txBiayaExp.Text = biayakg.ToString();
            lbTotalBiaya.Text = nTotalBiaya.ToString();
            lbMinExp.Text = expmin.ToString();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
              string.Concat("transaction_sales_Expedisi:Calc Biaya - ", ex.Message));
        }

        #endregion
    }

    #endregion

    [DirectMethod(ShowMask = true)]
    public void DeleteMethod(string eksNumber, string keterangan)
    {
        if (string.IsNullOrEmpty(eksNumber))
        {
            Functional.ShowMsgWarning("Nomor Ekspedisi tidak terbaca.");
            //Ext.Net.ResourceManager.AjaxSuccess = false;
            //Ext.Net.ResourceManager.AjaxErrorMessage = "Nomor Ekspedisi tidak terbaca.";

            return;
        }
        else if (string.IsNullOrEmpty(keterangan))
        {
            Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");
            //Ext.Net.ResourceManager.AjaxSuccess = false;
            //Ext.Net.ResourceManager.AjaxErrorMessage = "Keterangan tidak boleh kosong.";

            return;
        }
        PostDataParser.StructureResponse respon = DeleteParser(eksNumber, keterangan);

        if (respon.Response == PostDataParser.ResponseStatus.Success)
        {
            X.AddScript(
              string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r); }}",
              strGridMain.ClientID, eksNumber));

            Functional.ShowMsgInformation(string.Format("Nomor Expedisi '{0}' telah terhapus.", eksNumber));
        }
        else
        {
            Functional.ShowMsgWarning(respon.Message);
            //Ext.Net.ResourceManager.AjaxSuccess = false;
            //Ext.Net.ResourceManager.AjaxErrorMessage = respon.Message;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            hfGdg.Text = this.ActiveGudang;
            hfNip.Text = this.Nip;
        }


    }

    protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
    {
        if (!this.IsAllowAdd)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
            return;
        }

        //gridDetail2.ColumnModel.SetEditable(1, true);
        //gridDetail2.ColumnModel.SetEditor(1, new Ext.Net.NumberField() { AllowDecimals = true, AllowBlank = false, AllowNegative = false, MinValue = 0.00 });
        gridDetail2.ColumnModel.SetEditable(2, true);
        gridDetail2.ColumnModel.SetEditor(2, new Ext.Net.NumberField() { AllowDecimals = true, AllowBlank = false, AllowNegative = false, MinValue = 0.00 });
        gridDetail2.ColumnModel.SetEditable(3, true);
        gridDetail2.ColumnModel.SetEditor(3, new Ext.Net.NumberField() { AllowDecimals = true, AllowBlank = false, AllowNegative = false, MinValue = 0.00 });

        ClearEntrys();

        winDetail.Hidden = false;
        winDetail.ShowModal();
    }

    protected void GridMainCommand(object sender, DirectEventArgs e)
    {
        ClearEntrys();
        string cmd = (e.ExtraParams["Command"] ?? string.Empty);
        string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
        string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

        if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
        {
            PopulateDetail(pName, pID);
        }
        else
        {
            Functional.ShowMsgError("Perintah tidak dikenal.");
        }

        GC.Collect();
    }

    protected void GridDetailCommand(object sender, DirectEventArgs e)
    {
        string cmd = (e.ExtraParams["Command"] ?? string.Empty);
        string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

        if (cmd.Equals("Delete", StringComparison.OrdinalIgnoreCase) || cmd.Equals("Void", StringComparison.OrdinalIgnoreCase))
        {
            CalculateKoliBerat(pID, false, null, null, null);
        }
        else
        {
            Functional.ShowMsgError("Perintah tidak dikenal.");
        }

        GC.Collect();
    }

    [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void SaveBtn_Click(object sender, DirectEventArgs e)
    {
        string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
        string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
        string jsonGridValues2 = (e.ExtraParams["gridValues2"] ?? string.Empty);

        bool isConfirm = false;

        if (cbByHdr.SelectedItem.Value == "01" && lbTotalBiaya.Text == "0" && cbRefHo.SelectedIndex < 0)
        {
            Functional.ShowMsgInformation("Total biaya tidak boleh 0, periksa master biaya ekspedisi");
            return;
        }

        if (cbByHdr.SelectedItem.Value == "02" && (string.IsNullOrEmpty(cbDriver.SelectedItem.Value) || string.IsNullOrEmpty(txNoPol.Text)))
        {
            Functional.ShowMsgInformation("Driver & No Polisi tidak boleh kosong");
            return;
        }

        Dictionary<string, string>[] gridDataExp = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);
        Dictionary<string, string>[] gridDataExp2 = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues2);


        bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

        PostDataParser.StructureResponse respon = SaveParser(isAdd, isConfirm, numberId, gridDataExp, gridDataExp2);

        if (respon.IsSet)
        {
            if (respon.Response == PostDataParser.ResponseStatus.Success)
            {
                string scrpt = null,
                  sDOSalah = null,
                  sDOErrWP = null;
                string storeId = strGridMain.ClientID;

                string cust = (cbCustomerHdr.SelectedItem != null ? cbCustomerHdr.SelectedItem.Text : string.Empty);
                string viaDesc = (cbViaHdr.SelectedItem != null ? cbViaHdr.SelectedItem.Text : string.Empty);

                string dateJs = null;
                DateTime date = DateTime.Today;

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
                'c_expno': '{1}',
                'd_expdate': {2},
                'v_gdgdesc': '{3}',
                'v_cunam': '{4}',
                'v_ketTran': '{5}'
              }}));{0}.commitChanges();", storeId,
                                                      respon.Values.GetValueParser<string>("EXP", string.Empty),
                                     dateJs, this.ActiveGudangDescription, cust, viaDesc);

                            X.AddScript(scrpt);
                        }
                    }
                }

                //this.ClearEntrys();

                PopulateDetail("c_expno", respon.Values.GetValueParser<string>("EXP", string.Empty));

                sDOSalah = respon.Values.GetValueParser<string>("DO_Salah", string.Empty);
                sDOErrWP = respon.Values.GetValueParser<string>("DO_Err_WP", string.Empty);

                if (!string.IsNullOrEmpty(sDOSalah) || !string.IsNullOrEmpty(sDOErrWP))
                {
                    if (!string.IsNullOrEmpty(sDOSalah))
                    {
                        sDOSalah = "terdapat DO yang salah : " + sDOSalah;
                        Functional.ShowMsgInformation("Data berhasil tersimpan, " + sDOSalah + ".");
                    }

                    if (!string.IsNullOrEmpty(sDOErrWP))
                    {
                        sDOErrWP = "terdapat DO yang tanggal entry nya > tanggal resi : " + sDOErrWP;
                        Functional.ShowAlert("Data berhasil tersimpan, " + sDOErrWP + " " + Environment.NewLine + "Mohon diubah Tanggal atau Jam Resi nya");
                    }
                }
                else
                {
                    Functional.ShowMsgInformation("Data berhasil tersimpan.");
                }
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
    protected void Submit_scane(object sender, DirectEventArgs e)
    {
        string sNopart = "0";
        string s = e.ExtraParams["DO"].ToString();
        string scusno = e.ExtraParams["Cusno"].ToString();

        if (!string.IsNullOrEmpty(s) && (s.Length == 10))
        {
            Dictionary<string, object> dicEXP = null;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.Text.StringBuilder sb1 = new System.Text.StringBuilder();

            Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

            Dictionary<string, object> dicResult = null;
            Dictionary<string, string> dicResultInfo = null;
            Newtonsoft.Json.Linq.JArray jarr = null;
            string res = null;

            decimal curKoli = 0,
                curReceh = 0,
                nKarton = 0,
                nReceh = 0,
                nKartonSum = 0,
                nRecehSum = 0;

            string jsonGridValues = (e.ExtraParams["Grid"] ?? string.Empty);
            bool isChecklist = false,
                isExist = jsonGridValues.Contains(s);

            string[][] paramX = new string[][]{
                new string[] { string.Format("{0} = @0", "c_dono"), s, "System.String"}
                };

            res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0317", paramX);

            try
            {
                dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
                if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
                {
                    jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

                    dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

                    nKarton = dicResultInfo.GetValueParser<decimal>("n_karton", 0);
                    nReceh = dicResultInfo.GetValueParser<decimal>("n_receh", 0);

                    jarr.Clear();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                  string.Concat("wp_SerahTerimaTransportasiCtrl:PopulateDetail Header - ", ex.Message));
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

            if (!isExist)
            {
                string grid1Json = e.ExtraParams["Grid1"];
                IList<Dictionary<string, object>> list1 = JSON.Deserialize<Dictionary<string, object>[]>(grid1Json);
                var sListNoPart = new ArrayList(); 

                for (int i = 0; i < list1.Count; i++)
                {
                    dicEXP = list1[i];
                    foreach (KeyValuePair<string, object> kvp in dicEXP)
                    {
                        if (isChecklist)
                        {
                            continue;
                        }
                        if (kvp.Key == "chk1")
                        {
                            isChecklist = (bool)kvp.Value;
                        }
                        if (kvp.Key == "n_koli")
                        {
                            curKoli = (long)kvp.Value;
                        }
                        if (kvp.Key == "n_receh")
                        {
                            curReceh = (long)kvp.Value;
                        }
                        if (kvp.Key == "c_nopart")
                        {
                            sNopart = (string)kvp.Value;
                            sListNoPart.Add(sNopart);
                        }
                    }
                }

                if (!isChecklist)
                {
                    var iPart = sListNoPart.Count > 0 ? int.Parse(sListNoPart[0].ToString()) + 1 : 1;
                    sNopart = iPart.ToString().PadLeft(3, '0');

                    sb1.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                                    'n_koli': {2},
                                    'n_receh': {3},
                                    'n_berat': 0,
                                    'n_vol': 0,
                                    'c_nopart': '{1}',
                                    'l_new': true,
                                  }})); ", gridDetail2.GetStore().ClientID, sNopart, nKarton, nReceh);

                    X.AddScript(sb1.ToString());
                }
                else
                {
                    nKartonSum = nKarton + curKoli;

                    sb1.AppendFormat(@"var idx = {0}.findExact('c_nopart', '{1}');
                      if(idx != -1) {{
                        var r = {0}.getAt(idx);
                        r.set('n_koli', {2});
                        r.set('n_receh', {3});
                        {0}.commitChanges();
                      }}", gridDetail2.GetStore().ClientID,
                          sNopart,
                          nKartonSum,
                          nRecehSum
                          );

                    X.AddScript(sb1.ToString());
                }

                    sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                                    'c_dono': '{1}',
                                    'n_koli': {3},
                                    'n_receh': {4},
                                    'n_berat': 0,
                                    'n_vol': 0,
                                    'c_nopart': '{2}',
                                    'l_new': true,
                                  }})); ", gridDetail.GetStore().ClientID, s, sNopart, nKarton, nReceh);

                X.AddScript(sb.ToString());

                txKoli.Clear();
            }
            else
            {
                Functional.ShowMsgInformation("Data telah ada.");
            }
        }

        cbDODtl.Clear();
    }

    [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void AddBtn_Click(object sebder, DirectEventArgs e)
    {
        string no = e.ExtraParams["NO"].ToString();
        string jsonGridValues = (e.ExtraParams["Grid"] ?? string.Empty);
        CalculateKoliBerat(no, true, jsonGridValues, null, null);
    }

    [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void RecalcBtn(object sebder, DirectEventArgs e)
    {
        if (cbRefHo.SelectedIndex < 0)
        {
            if (cbByHdr.SelectedItem.Value == "01" || cbByHdr.SelectedItem.Value == "4" || cbByHdr.SelectedItem.Value == "04")
            {
                string jsonGridValues = (e.ExtraParams["Grid"] ?? string.Empty);
                string exp = e.ExtraParams["exp"].ToString();
                string cusno = e.ExtraParams["cusno"].ToString();
                string tipebiaya = e.ExtraParams["tipebiaya"].ToString();
                CalculateTotalBiaya(jsonGridValues, exp, cusno, tipebiaya);
            }
            else
            {
                lbTotalBiaya.Text = "0";
                lbMinExp.Text = "0";
                txBiayaExp.Text = "0";
            }
        }
        else
        {
            txBiayaLain.Text = "0";
            lbTotalBiaya.Text = "0";
            lbMinExp.Text = "0";
            txBiayaExp.Text = "0";
        }
    }

    [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void AddBtnWP_Click(object sebder, DirectEventArgs e)
    {
        string no = e.ExtraParams["WP"].ToString();
        string jsonGridValues = (e.ExtraParams["Grid"] ?? string.Empty);
        string jsonGridValues2 = (e.ExtraParams["Grid2"] ?? string.Empty);
        string cusno = (e.ExtraParams["cusno"] ?? string.Empty);
        CalculateKoliBerat(no, true, jsonGridValues, cusno, jsonGridValues2);
        cbWPDtl.Clear();
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

        if (string.IsNullOrEmpty(numberID))
        {
            Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");
            return;
        }

        if (hfPrint.Text.ToUpper() == "TRUE" && string.IsNullOrEmpty(txReprint.Text))
        {
            txReprint.Hidden = false;
            Functional.ShowMsgError("Maaf, Harap berikan masukan alasan reprint terlebih dahulu!!!");
            txReprint.Focus();
            return;
        }

        ReportParser rptParse = new ReportParser();

        List<ReportParameter> lstRptParam = new List<ReportParameter>();

        List<string> lstData = new List<string>();

        rptParse.ReportingID = "10115";

        #region Report Parameter

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(char).FullName,
            ParameterName = "LG_ExpH.c_gdg",
            ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "LG_ExpH.c_expno",
            ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID)
        });

        #endregion

        rptParse.PaperID = "Letter";
        rptParse.ReportParameter = lstRptParam.ToArray();
        rptParse.User = pag.Nip;
        rptParse.Reason = txReprint.Text.Trim();
        rptParse.UserDefinedName = numberID;

        txReprint.Clear();
        txReprint.Hidden = true;

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
                //string rptName = string.Concat("Packing_List_", pag.Nip, ".", rptResult.Extension);

                //string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
                //tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
                //  tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

                string tmpUri = Functional.UriDownloadGenerator(pag,
                  rptResult.OutputFile, "Ekspedisi Shipment", rptResult.Extension);

                wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
                hfPrint.Text = "TRUE";
            }
            else
            {
                Functional.ShowMsgWarning(rptResult.MessageResponse);
            }
        }

        GC.Collect();
    }
}