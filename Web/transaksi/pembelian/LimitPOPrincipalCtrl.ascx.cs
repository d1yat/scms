using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using System.Globalization;

public partial class transaksi_pembelian_LimitPOPrincipalCtrl : System.Web.UI.UserControl
{
    #region Private

    private void ClearEntrys()
    {
        winDetail.Title = "Limit Principal";

        hfDivPriNo.Clear();

        Ext.Net.Store store = gridDetail.GetStore();
        if (store != null)
        {
            store.RemoveAll();
        }
    }

    private void PopulateDetail(string pName, string pID, string tahun, string bulan)
    {
        ClearEntrys();

        Dictionary<string, object> dicLim = null;
        Dictionary<string, string> dicLimInfo = null;
        Newtonsoft.Json.Linq.JArray jarr = null;

        CultureInfo culture = new CultureInfo("id-ID");

        Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

        string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"},
        new string[] { string.Format("{0} = @0", "n_tahun"), tahun, "System.Decimal"},
        new string[] { string.Format("{0} = @0", "n_bulan"), bulan, "System.Decimal"}
      };

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        #region Parser Header

        string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "05006", paramX);

        try
        {
            dicLim = JSON.Deserialize<Dictionary<string, object>>(res);
            if (dicLim.ContainsKey("records") && (dicLim.ContainsKey("totalRows") && (((long)dicLim["totalRows"]) > 0)))
            {
                jarr = new Newtonsoft.Json.Linq.JArray(dicLim["records"]);

                dicLimInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

                winDetail.Title = string.Format("Limit PO Principal - {0} - {1}", pID, dicLimInfo.GetValueParser<string>("v_nama", string.Empty));

                //lbLimitPrinc.Text = dicLimInfo.GetValueParser<decimal>("limit", 0).ToString("N2", culture);
                //lbLimitSisaPrinc.Text = dicLimInfo.GetValueParser<decimal>("avaiblelimit", 0).ToString("N2", culture);
                //lbLimitDivPri.Text = dicLimInfo.GetValueParser<decimal>("n_budget", 0).ToString("N2", culture);
                //lbLimitAlokasi.Text = dicLimInfo.GetValueParser<decimal>("n_balance", 0).ToString("N2", culture);

                hfTahun.Text = tahun;
                hfBulan.Text = bulan;

                chkSuggest.Checked = false;

                jarr.Clear();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
              string.Concat("transaksi_pembelian_Limit_PO_Principal_Ctrl:PopulateDetail Header - ", ex.Message));
        }
        finally
        {
            if (jarr != null)
            {
                jarr.Clear();
            }
            if (dicLimInfo != null)
            {
                dicLimInfo.Clear();
            }
            if (dicLim != null)
            {
                dicLim.Clear();
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

            hfDivPriNo.Text = pID;
            X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
              string.Concat("transaksi_pembelian_LimitPOCtrl:PopulateDetail Detail - ", ex.Message));
        }

        #endregion

        winDetail.Hidden = false;
        winDetail.ShowModal();

        GC.Collect();
    }

    private PostDataParser.StructureResponse SaveParser(bool isAdd, string idNumber, Dictionary<string, string>[] dics, string nTahun, string nBulan, decimal limitdiv, decimal limitalokasi)
    {
        PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

        PostDataParser parser = new PostDataParser();
        IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

        Dictionary<string, string> dicData = null;

        PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

        Dictionary<string, string> dicAttr = null;

        pair.IsSet = true;
        pair.IsList = true;
        pair.Value = idNumber;
        pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
        pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        string tmp = null,
          item = null,
          ket = null;

        bool isModify = false,
            isTotal = false;
        string varData = null;

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        DateTime date = DateTime.Today;

        if (limitalokasi > limitdiv)
        {
            responseResult = new PostDataParser.StructureResponse()
            {
                IsSet = true,
                Message = "Melebihi limit",
                Response = PostDataParser.ResponseStatus.Failed
            };

            return responseResult;
        }

        dic.Add("ID", pair);
        pair.DicAttributeValues.Add("Entry", pag.Nip);
        pair.DicAttributeValues.Add("kddivprihdr", idNumber);
        pair.DicAttributeValues.Add("nTahun", nTahun);
        pair.DicAttributeValues.Add("nBulan", nBulan);
        pair.DicAttributeValues.Add("limitdiv", limitdiv.ToString());
        pair.DicAttributeValues.Add("limitdivsisa", limitalokasi.ToString());
        //pair.DicAttributeValues.Add("limitused", limitused.ToString());

        for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
        {
            tmp = nLoop.ToString();

            dicData = dics[nLoop];

            isModify = dicData.GetValueParser<bool>("l_modified");

            if (isModify && !isTotal)
            {
                if (!pag.IsAllowEdit)
                {
                    continue;
                }

                dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                dicAttr.Add("Modified", isModify.ToString().ToLower());

                item = dicData.GetValueParser<string>("c_iteno");

                if (!string.IsNullOrEmpty(item))
                {
                    dicAttr.Add("Item", item);
                    dicAttr.Add("Budget", dicData.GetValueParser<string>("n_budget"));
                    dicAttr.Add("Balance", dicData.GetValueParser<string>("n_balance"));
                    dicAttr.Add("availablebudget", dicData.GetValueParser<string>("n_availablebudget"));

                    pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
                    {
                        IsSet = true,
                        Value = (string.IsNullOrEmpty(ket) ? "Modify" : ket),
                        DicAttributeValues = dicAttr
                    });
                }
            }

            dicData.Clear();
        }

        try
        {
            varData = parser.ParserData("LimitPOPrincipal", (isAdd ? "Add" : "Modify"), dic);
        }
        catch (Exception ex)
        {
            Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_LimitPOItemCtrl SaveParser : {0} ", ex.Message);
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

    public void Initialize(string storeIDGridMain)
    {
        hfStoreID.Text = storeIDGridMain;
    }

    public void CommandPopulate(string pID, string tahun, string bulan)
    {
        PopulateDetail("c_nosup", pID, tahun, bulan);
    }

    [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void SaveBtn_Click(object sender, DirectEventArgs e)
    {
        string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
        string limitdiv = (e.ExtraParams["limitdiv"] ?? string.Empty);
        string limitalokasi = (e.ExtraParams["limitalokasi"] ?? string.Empty);
        //string limitused = (e.ExtraParams["limitused"] ?? string.Empty);

        string nTahun = (e.ExtraParams["tahun"] ?? string.Empty);
        string nBulan = (e.ExtraParams["bulan"] ?? string.Empty);
        string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);

        decimal limalokasi = 0,
              limbudget = 0;

        bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

        if (isAdd)
        {
            if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
            {
                Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
                return;
            }
        }
        else if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowEdit)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menyimpan data.");
            return;
        }

        limbudget = decimal.Parse(limitdiv.Replace(".", "").Replace(",", "."));
        limalokasi = decimal.Parse(limitalokasi.Replace(".", "").Replace(",", "."));
        //limused = decimal.Parse(limitused.Replace(".", "").Replace(",", "."));

        Dictionary<string, string>[] gridDataLim = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

        PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataLim, nTahun, nBulan, limbudget, limalokasi);

        if (respon.IsSet)
        {
            if (respon.Response == PostDataParser.ResponseStatus.Success)
            {
                string storeId = hfStoreID.Text;

                //this.ClearEntrys();        
                PopulateDetail("c_nosup", numberId, nTahun, nBulan);

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