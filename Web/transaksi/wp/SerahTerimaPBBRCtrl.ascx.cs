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

public partial class transaksi_wp_SerahTerimaPBBRCtrl : System.Web.UI.UserControl
{
    #region Private

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
                        new string[] { "c_grouped", "ST_PBBR", "System.String"}
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

                wndIDUser.Title = string.Concat("Serah Terima Admin PBB/R - ", pID);

                txNipPenerima.Text = ((dicResultInfo.ContainsKey("c_entry") ? dicResultInfo["c_entry"] : string.Empty));
                txNipPenerima.Disabled = true;

                txNamePenerima.Text = ((dicResultInfo.ContainsKey("v_entry") ? dicResultInfo["v_entry"] : string.Empty));
                txNamePenerima.Disabled = true;
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
        btnSave.Disabled = true;
        btnReload.Disabled = true;

        GC.Collect();
    }

    private void ClearEntrys()
    {
        wndIDUser.Title = "Serah Terima Admin PBB/R ";
        hfSTNo.Clear();

        frmpnlDetailEntry.Hidden = false;

        txNipPenerima.Clear();
        txNipPenerima.Disabled = false;
        txNamePenerima.Clear();

        btnOk.Disabled = false;
        btnSave.Disabled = false;
        btnReload.Disabled = false;

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
        pair.DicAttributeValues.Add("Give", "XXXXXXX");
        pair.DicAttributeValues.Add("GiveName", "Team Gudang Retur");
        pair.DicAttributeValues.Add("Tipe", "06");

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

                if (!string.IsNullOrEmpty(noTrans))
                {
                    dicAttr.Add("TransNo", noTrans);
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
            Scms.Web.Common.Logger.WriteLine("transaksi_wp_SerahTerimaPBBRCtrl SaveParser : {0} ", ex.Message);
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
        string s = e.ExtraParams["NO"].ToString().ToUpper();

        if (!string.IsNullOrEmpty(s) && (s.Length == 16))
        {

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            string jsonGridValues = (e.ExtraParams["Grid"] ?? string.Empty);

            bool isExist = jsonGridValues.Contains(s);

            if (!isExist)
            {
                sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                'c_no': '{1}',
                'l_new': true,
              }})); ", gridDetail.GetStore().ClientID,
                                    s);

                X.AddScript(sb.ToString());
            }
            else
            {
                Functional.ShowMsgInformation("Data telah ada.");
            }
        }

        txPBBR.Clear();
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
                                        hfGudangDesc.Text, dateJs, txNamePenerima.Text, "Team Gudang Retur");

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
