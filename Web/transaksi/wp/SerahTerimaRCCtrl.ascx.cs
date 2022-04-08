using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using ScmsSoaLibraryInterface.Core.Crypto;
using System.Text;

public partial class transaksi_wp_SerahTerimaRCCtrl : System.Web.UI.UserControl
{
    #region Private
    string bar = null;
    string barcode = null;
    private static string passPenyerah = null;

    private void PopulateUserAccess(List<Dictionary<string, string>> listUserInfo)
    {
        Dictionary<string, string> dicUserInfo = null;
        bool isFilled = false;
        List<string> listAkses = new List<string>();

        try
        {
            for (int nLoop = 0, nLen = listUserInfo.Count; nLoop < nLen; nLoop++)
            {
                dicUserInfo = listUserInfo[nLoop];

                if (!isFilled)
                {
                    txNipPenerima.Text = dicUserInfo.GetValueParser<string>("c_nip", string.Empty);
                    txNamePenerima.Text = dicUserInfo.GetValueParser<string>("v_username", string.Empty);
                    passPenyerah = dicUserInfo.GetValueParser<string>("v_password", string.Empty);

                    isFilled = true;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
              string.Concat("_Default:PopulateUserAccess - ", ex.Message));
        }
    }

    private void PopulateUser(string Case, string pNip)
    {
        switch (Case)
        {
            case "View":
                {
                }
                break;
            case "Add":
                {
                    #region Add
                    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

                    string[][] paramX = new string[][]
                    {
                        new string[] { "c_nip", pNip, "System.String"},
                        new string[] { "c_grouped", "ST_RC", "System.String"}
                    };

                    string res = soa.GlobalQueryService(0, 0, true, string.Empty, string.Empty, "2151", paramX);

                    Dictionary<string, object> dicUser = null;
                    List<Dictionary<string, string>> listUserInfo = null;
                    Newtonsoft.Json.Linq.JArray jarr = null;
                    try
                    {
                        dicUser = JSON.Deserialize<Dictionary<string, object>>(res);
                        if (dicUser.ContainsKey("records"))
                        {
                            jarr = new Newtonsoft.Json.Linq.JArray(dicUser["records"]);

                            listUserInfo = JSON.Deserialize<List<Dictionary<string, string>>>(jarr.First.ToString());

                            this.PopulateUserAccess(listUserInfo);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(
                          string.Concat("_Default:PopulateDetail - ", ex.Message));
                    }
                    finally
                    {
                        if (jarr != null)
                            jarr.Clear();

                        if (listUserInfo != null)
                            listUserInfo.Clear();

                        if (dicUser != null)
                            dicUser.Clear();
                    }
                    #endregion
                }
                break;
        }
    }

    private void PopulateDetail(string pName, string pID)
    {
        ClearEntrys();

        Dictionary<string, object> dicResult = null;
        Dictionary<string, string> dicResultInfo = null;
        Newtonsoft.Json.Linq.JArray jarr = null;

        Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

        string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
      };

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        #region Parser Header

        string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0307", paramX);

        try
        {
            dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
            if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
            {
                jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

                dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

                wndIDUser.Title = string.Concat("Serah Terima Gudang PBB/R - ", pID);

                txNipPenerima.Text = ((dicResultInfo.ContainsKey("c_entry") ? dicResultInfo["c_entry"] : string.Empty));
                txNipPenerima.Disabled = true;

                txNamePenerima.Text = ((dicResultInfo.ContainsKey("v_entry") ? dicResultInfo["v_entry"] : string.Empty));
                txNamePenerima.Disabled = true;

                Functional.SetComboData(cbEksHdr, "c_exp", dicResultInfo.GetValueParser<string>("v_give", string.Empty), dicResultInfo.GetValueParser<string>("c_give", string.Empty));
                cbEksHdr.Hidden = false;

                txNoResiHdr.Text = ((dicResultInfo.ContainsKey("c_resi") ? dicResultInfo["c_resi"] : string.Empty));
                txNoResiHdr.Hidden = false;

                txKoli.Text = (dicResultInfo.GetValueParser<decimal>("n_koli").ToString());
                txKoli.Hidden = false;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
              string.Concat("transaction_wp_SerahTerima_Transportasi:PopulateDetail Header - ", ex.Message));
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

            hfSTNo.Text = pID;
            X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
              string.Concat("transaction_wp_SerahTerima_Transportasi:PopulateDetail Detail - ", ex.Message));
        }

        #endregion

        //frmpnlDetailEntry.Hidden = true;
        pnlGridDetail.Hidden = false;
        wndIDUser.Height = 480;

        wndIDUser.Hidden = false;
        wndIDUser.ShowModal();

        btnOk.Disabled = true;
        //btnSave.Disabled = true;
        btnReload.Disabled = true;
        btnOk.Disabled = true;

        GC.Collect();
    }

    private void ClearEntrys()
    {
        wndIDUser.Title = "Serah Terima Gudang PBB/R";
        hfSTNo.Clear();

        frmpnlDetailEntry.Hidden = false;

        txNipPenerima.Clear();
        txNipPenerima.Disabled = false;
        txNamePenerima.Clear();

        cbEksHdr.Clear();
        cbEksHdr.Disabled = false;
        Ext.Net.Store cbEksHdrstr = cbEksHdr.GetStore();
        if (cbEksHdrstr != null)
        {
            cbEksHdrstr.RemoveAll();
        }
        cbEksHdr.Hidden = true;

        txNoResiHdr.Clear();
        txNoResiHdr.Hidden = true;

        txKoli.Clear();
        txKoli.Hidden = true;

        btnOk.Disabled = false;
        btnSave.Disabled = false;
        btnReload.Disabled = false;
        btnOk.Disabled = false;

        X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));
        pnlGridDetail.Hidden = true;

        wndIDUser.Height = 190;

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
    private void BacaBarcode()
    {
        string a1 = null;
        string b1 = null;
        string a2 = null;
        a1 = barcode.Substring(0, 1);
        byte[] aa = ASCIIEncoding.UTF32.GetBytes("a");

        //int a = BitConverter.GetBytes(barcode.Substring(0, 1));
        byte[] a = ASCIIEncoding.UTF32.GetBytes(barcode.Substring(0, 1));
        byte[] b = ASCIIEncoding.UTF32.GetBytes(barcode.Substring(1, 1));
        byte[] c = ASCIIEncoding.UTF32.GetBytes(barcode.Substring(2, 1));
        string d = barcode.Substring(3, 4);
        int i = 65;
        int ab = 2017;
        string c1 = null;
        if (b[0] >= 65 && b[0] <= 76)
        {
            a1 = "R";
            if (b[0] == 65) { a2 = "01"; }
            if (b[0] == 66) { a2 = "02"; }
            if (b[0] == 67) { a2 = "03"; }
            if (b[0] == 68) { a2 = "04"; }
            if (b[0] == 69) { a2 = "05"; }
            if (b[0] == 70) { a2 = "06"; }
            if (b[0] == 71) { a2 = "07"; }
            if (b[0] == 72) { a2 = "08"; }
            if (b[0] == 73) { a2 = "09"; }
            if (b[0] == 74) { a2 = "10"; }
            if (b[0] == 75) { a2 = "11"; }
            if (b[0] == 76) { a2 = "12"; }
        }
        else if (b[0] >= 77 && b[0] <= 88)
        {
            a1 = "B";
            if (b[0] == 77) { a2 = "01"; }
            if (b[0] == 78) { a2 = "02"; }
            if (b[0] == 79) { a2 = "03"; }
            if (b[0] == 80) { a2 = "04"; }
            if (b[0] == 81) { a2 = "05"; }
            if (b[0] == 82) { a2 = "06"; }
            if (b[0] == 83) { a2 = "07"; }
            if (b[0] == 84) { a2 = "08"; }
            if (b[0] == 85) { a2 = "09"; }
            if (b[0] == 86) { a2 = "10"; }
            if (b[0] == 87) { a2 = "11"; }
            if (b[0] == 88) { a2 = "12"; }
        }
        
        if (a[0] == 65){ b1 = "MDN"; }
        if (a[0] == 66){ b1 = "PKB"; }
        if (a[0] == 67){ b1 = "BTM"; }
        if (a[0] == 68){ b1 = "PDG"; }
        if (a[0] == 69){ b1 = "PLB"; }
        if (a[0] == 70){ b1 = "TGR"; }
        if (a[0] == 71){ b1 = "JK3"; }
        if (a[0] == 72){ b1 = "BKS"; }
        if (a[0] == 73){ b1 = "BGR"; }
        if (a[0] == 74){ b1 = "BDG"; }
        if (a[0] == 75){ b1 = "CRB"; }
        if (a[0] == 76){ b1 = "PWK"; }
        if (a[0] == 77){ b1 = "SMG"; }
        if (a[0] == 78){ b1 = "YOG"; }
        if (a[0] == 79){ b1 = "SLO"; }
        if (a[0] == 80){ b1 = "MLG"; }
        if (a[0] == 81){ b1 = "SB1"; }
        if (a[0] == 82){ b1 = "SB2"; }
        if (a[0] == 83){ b1 = "JBR"; }
        if (a[0] == 84){ b1 = "DPS"; }
        if (a[0] == 85){ b1 = "MKS"; }
        if (a[0] == 86){ b1 = "SMD"; }
        if (a[0] == 87){ b1 = "JK1"; }
        if (a[0] == 88){ b1 = "PTK"; }
        if (a[0] == 89){ b1 = "BJM"; }
        if (a[0] == 90){ b1 = "JK2"; }
        if (a[0] == 97){ b1 = "BLG"; }
        if (a[0] == 98){ b1 = "PST"; }
	if (a[0] == 99){ b1 = "JMB"; }

        for (i = 65;i<= 200;i++)
        {
            if (i == c[0]){ c1 = ab.ToString(); break; }
            if (i != c[0]) { ab = ab + 1; }
        }
        bar = "PB" + a1 + b1 + c1 + a2 + d;

    }
    private PostDataParser.StructureResponse SaveParser(bool isAdd, bool isConfirm, string plNumber, Dictionary<string, string>[] dics)
    {
        PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

        PostDataParser parser = new PostDataParser();
        IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

        Dictionary<string, string> dicData = null;

        PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

        Dictionary<string, string> dicAttr = null;

        string tmp = null,
        noTrans = null,
        ket = null;
        bool isNew = false,
          isVoid = false,
          isModify = false;
        string varData = null;

        DateTime tanggal = DateTime.MinValue;

        pair.IsSet = true;
        pair.IsList = true;
        pair.Value = plNumber;
        pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
        pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dic.Add("ID", pair);
        pair.DicAttributeValues.Add("Entry", txNipPenerima.Text.Trim());
        pair.DicAttributeValues.Add("EntryName", txNamePenerima.Text.Trim());
        pair.DicAttributeValues.Add("Gudang", hfGudang.Text.Trim());
        pair.DicAttributeValues.Add("Give", cbEksHdr.Text);
        pair.DicAttributeValues.Add("GiveName", cbEksHdr.SelectedItem.Text);
        pair.DicAttributeValues.Add("Koli", txKoli.Text.Trim());
        pair.DicAttributeValues.Add("Resi", txNoResiHdr.Text.Trim());
        pair.DicAttributeValues.Add("Tipe", "05");

        if (!isAdd)
            pair.DicAttributeValues.Add("NoDoc", hfSTNo.Text);

        System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("id-ID");

        for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
        {
            tmp = nLoop.ToString();

            dicData = dics[nLoop];

            isNew = dicData.GetValueParser<bool>("l_new");
            isVoid = dicData.GetValueParser<bool>("l_void");
            isModify = dicData.GetValueParser<bool>("l_modified");

            if ((!isVoid))
            {
                dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                dicAttr.Add("New", isNew.ToString().ToLower());
                dicAttr.Add("Delete", isVoid.ToString().ToLower());
                dicAttr.Add("Modified", isModify.ToString().ToLower());

                noTrans = dicData.GetValueParser<string>("c_no");
                //n_koli = dicData.GetValueParser<decimal>("n_koli", 0);
                //n_berat = dicData.GetValueParser<decimal>("n_berat", 0);

                if (!string.IsNullOrEmpty(noTrans))
                {
                    dicAttr.Add("TransNo", noTrans);
                    //dicAttr.Add("Koli", n_koli.ToString());
                    //dicAttr.Add("Berat", n_berat.ToString());

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
            varData = parser.ParserData("SerahTerima", (isAdd ? "Add" : "Modify"), dic);
        }
        catch (Exception ex)
        {
            Scms.Web.Common.Logger.WriteLine("transaksi_wp_SerahTerimaRCCtrl SaveParser : {0} ", ex.Message);
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

    public void Initialize(string gudang, string gudangDesc, string storeIDGridMain)
    {
        hfGudang.Text = gudang;
        hfStoreID.Text = storeIDGridMain;
        hfGudangDesc.Text = gudangDesc;
    }

    public void CommandPopulate(bool isAdd, string pID)
    {
        if (isAdd)
        {
            ClearEntrys();

            Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
            //txNipPenerima.Text = pag.Nip;
            //txNamePenerima.Text = pag.Username;


            wndIDUser.Hidden = false;
            wndIDUser.ShowModal();
            wndIDUser.Height = 190;
        }
        else
        {
            PopulateDetail("c_nodoc", pID);
        }
    }

    [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void btn_onclick(object sender, DirectEventArgs e)
    {
        bool isOk = false;

        if (string.IsNullOrEmpty(txNipPenerima.Text))
            e.ErrorMessage = "Nip Penerima belum diisi.";
        else if (string.IsNullOrEmpty(txNamePenerima.Text))
            e.ErrorMessage = "NIP Penerima " + txNipPenerima.Text + " tidak ada akses.";
        else
            isOk = true;

        if (isOk)
        {
            pnlGridDetail.Hidden = false;
            wndIDUser.Height = 480;
            txNipPenerima.Disabled = true;
            btnOk.Disabled = true;
            cbEksHdr.Hidden = false;
            txNoResiHdr.Hidden = false;
            txKoli.Hidden = false;
        }
    }

    [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void ReloadBtn_Click(object sender, DirectEventArgs e)
    {
        ClearEntrys();
    }

    [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void OnEvenCheckUser(object sender, DirectEventArgs e)
    {
        string pNipPenerima = (e.ExtraParams["NipPenerima"] ?? string.Empty);

        PopulateUser("Add", pNipPenerima);

        GC.Collect();

        btn_onclick(sender, e);
    }

    [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void Submit_scane(object sender, DirectEventArgs e)
    {
        if (string.IsNullOrEmpty(hfSTNo.Text))
        {
            string s = e.ExtraParams["NO"].ToString().ToUpper();

            if (!string.IsNullOrEmpty(s) && (s.Length == 7))
            {
                barcode = s;
                BacaBarcode();

                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                string jsonGridValues = (e.ExtraParams["Grid"] ?? string.Empty);

                bool isExist = jsonGridValues.Contains(bar);

                if (!isExist)
                {
                    sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                'c_no': '{1}',
                'l_new': true,
              }})); ", gridDetail.GetStore().ClientID,
                                        bar);

                    X.AddScript(sb.ToString());
                }
                else
                {
                    Functional.ShowMsgInformation("Data telah ada.");
                }
            }

            txPBBR.Clear();
        }
    }

    [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void SaveBtn_Click(object sender, DirectEventArgs e)
    {
        string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
        string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
        bool isConfirm = false;

        Dictionary<string, string>[] gridDataPicker = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

        bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

        PostDataParser.StructureResponse respon = SaveParser(isAdd, isConfirm, numberId, gridDataPicker);

        if (respon.IsSet)
        {
            if (respon.Response == PostDataParser.ResponseStatus.Success)
            {
                string scrpt = null,
                    sTransSalah = null;
                string storeId = hfStoreID.Text;

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
                                                        'v_gdgdesc': '{2}',
                                                        'c_nodoc': '{1}',
                                                        'd_entry': {3},
                                                        'v_entry': '{4}',
                                                        'v_give': '{5}'
                                                      }}));{0}.commitChanges();", storeId,
                                        respon.Values.GetValueParser<string>("WP", string.Empty),
                                        hfGudangDesc.Text, dateJs, txNamePenerima.Text, cbEksHdr.SelectedItem.Text);

                            X.AddScript(scrpt);
                        }
                    }
                }

                this.ClearEntrys();

                sTransSalah = respon.Values.GetValueParser<string>("Transaksi_Salah", string.Empty);

                if (!string.IsNullOrEmpty(sTransSalah))
                {
                    sTransSalah = "terdapat No.Transaksi yang salah : " + sTransSalah;
                    Functional.ShowAlert("Data sebagian berhasil tersimpan, " + sTransSalah + " " + Environment.NewLine + "Mohon dicek kembali... !!!");
                }
                else
                {
                    Functional.ShowMsgInformation("Data berhasil tersimpan.");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(respon.Message))
                {
                    e.ErrorMessage = "PBB/R tidak ada data di DistCore";
                }
                else
                {
                    e.ErrorMessage = respon.Message;
                }
                e.Success = false;
            }
        }
        else
        {
            e.ErrorMessage = "Unknown response";
            e.Success = false;
        }
    }

    protected void OnSelectGrid(object sender, DirectEventArgs e)
    {
        string pName = (e.ExtraParams["c_noTrans"] ?? string.Empty);
        string tmp = null;
        
        Ext.Net.Store store = gridDetail2.GetStore();

        tmp = string.Format(@"var xOpts = {{
                                  params: {{
                                      start: '0',
                                      limit: '-1',
                                      allQuery: 'true',
                                      model: 'a-15001',
                                      sort: '',
                                      dir: '',
                                      parameters: [['C_PBNO', '{0}', 'System.String']]
                                    }}
                                  }};", pName.ToString());
        X.AddScript(tmp);
        X.AddScript(string.Format("{0}.removeAll();{0}.reload(xOpts);", store.ClientID));

        GC.Collect();
    }
}
