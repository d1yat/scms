using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_retur_SerahTerimaGoodCtrl : System.Web.UI.UserControl
{
    #region Private

    private void PopulateDetail(string pName, string pID)
    {
        Dictionary<string, object> dicResult = null;
        Dictionary<string, string> dicResultInfo = null;
        Newtonsoft.Json.Linq.JArray jarr = null;

        Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

        string[][] paramX = new string[][]{
        new string[] { "v_stno = @0", pID, "System.String"}
      };

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0362", paramX);

        System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("id-ID");

        #region Parser Header
        try
        {
            dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
            if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
            {
                jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

                string tgl = null;

                dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

                winDetail.Title = string.Format("Master Prinsipal - {0}", pID);

                DateTime date = Functional.JsonDateToDate(tgl);
                txSTno.Text = ((dicResultInfo.ContainsKey("v_stno") ? dicResultInfo["v_stno"] : string.Empty));
                txRCno.Text = ((dicResultInfo.ContainsKey("c_rcno") ? dicResultInfo["c_rcno"] : string.Empty));
                txPBBRno.Text = ((dicResultInfo.ContainsKey("v_pbbrno") ? dicResultInfo["v_pbbrno"] : string.Empty));
                txCab.Text = ((dicResultInfo.ContainsKey("v_cunam") ? dicResultInfo["v_cunam"] : string.Empty));
                tgl = ((dicResultInfo.ContainsKey("d_entry") ? dicResultInfo["d_entry"] : string.Empty));

                date = Functional.JsonDateToDate(tgl);
                dfTanggal.Text = date.ToString("dd-MM-yyyy");

            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(string.Concat("SerahTerima:PopulateDetail Header - ", ex.Message));
        }
        #endregion

        #region Parser Detail

        try
        {
            Ext.Net.Store store = griddetail.GetStore();
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

            hfSTno.Text = pID;
            X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
              string.Concat("Serahterima:PopulateDetail Detail - ", ex.Message));
        }
        #endregion

        hfSTno.Text = pID;
    }

    private PostDataParser.StructureResponse SaveParser(bool isAdd, string plNumber, Dictionary<string, string>[] dics, string confirm)
    {
        PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

        PostDataParser parser = new PostDataParser();
        IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

        Dictionary<string, string> dicData = null;

        PostDataParser.StructurePair pair = new PostDataParser.StructurePair();
        var connectionStringSql = "Driver={SQL Server};Server=10.100.11.25;Database=AMS;Uid=sa;Pwd=itadmin";
        //var connectionStringSql = "Driver={SQL Server};Server=10.100.11.25;Database=AMS;Uid=sa;Pwd=itadmin";

        Dictionary<string, string> dicAttr = null;

        pair.IsSet = true;
        pair.IsList = true;
        pair.Value = plNumber;
        pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
        pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        string result = null;

        string tmp = null,
          item = null,
          ket = null,
          ketEdit = null,
          nbatch = null,
          nbatchditerima = null;
        decimal nQty = 0, nJml = 0,
          nAcc = 0;
        bool isNew = false,
          isVoid = false,
          isModify = false;
        string varData = null;
        string email = null, vitnam = null;
        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        DateTime date = DateTime.Today;
        DateTime dateawal = DateTime.Today;

        dic.Add("ID", pair);
        pair.DicAttributeValues.Add("Entry", pag.Nip);
        pair.DicAttributeValues.Add("Gudang", pag.Cabang);
        pair.DicAttributeValues.Add("Confirm", confirm);
        //pair.DicAttributeValues.Add("Keterangan", txKeterangan.Text.Trim());
        //pair.DicAttributeValues.Add("Tipe", "03");
        //pair.DicAttributeValues.Add("Cek", chkCheck.Checked.ToString().ToLower());
        try
        {
            for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
            {
                tmp = nLoop.ToString();
                dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                dicData = dics[nLoop];
                string c_iteno = dicData.GetValueParser<string>("c_iteno");
                string c_batch = dicData.GetValueParser<string>("c_batch");
                string n_qtyterima = dicData.GetValueParser<string>("n_qtyterima");
                string n_qtyreject = dicData.GetValueParser<string>("n_qtyreject");
                dicAttr.Add("c_iteno", dicData.GetValueParser<string>("c_iteno"));
                dicAttr.Add("c_batch", dicData.GetValueParser<string>("c_batch"));
                dicAttr.Add("n_qtyterima", dicData.GetValueParser<string>("n_qtyterima"));
                dicAttr.Add("n_qtyreject", dicData.GetValueParser<string>("n_qtyreject"));

                pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
                {
                    IsSet = true,
                    Value = (string.IsNullOrEmpty(ket) ? "Human error" : ""),
                    DicAttributeValues = dicAttr
                });
            }
            varData = parser.ParserData("UPDATEST", ("Modify"), dic);
        }
        catch (Exception ex)
        {
            responseResult = new PostDataParser.StructureResponse()
            {
                IsSet = true,
                Message = "Ada Kesalahan pada batch mohon di cek kembali.",
                Response = PostDataParser.ResponseStatus.Failed
            };
            goto endlogic;
        }

        
    endlogic:
        if (!string.IsNullOrEmpty(varData))
        {
            Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

            result = soa.PostData(varData);

            responseResult = parser.ResponseParser(result);
        }

        return responseResult;
    }

    #endregion

    public void CommandPopulate(bool isNew, string pID, string sTipe)
    {
        winDetail.Title = "Serah Terima GOOD";
        PopulateDetail("v_stno", pID);
        winDetail.Hidden = false;
        winDetail.ShowModal();
        GC.Collect();
    }

    protected void SaveBtn_Click(object sender, DirectEventArgs e)
    {
        string numberId = (e.ExtraParams["stno"] ?? string.Empty);
        string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
        string confirm = (e.ExtraParams["confirm"] ?? string.Empty);
        bool isAdd = (string.IsNullOrEmpty(numberId) ? false : true);

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


        Dictionary<string, string>[] gridDataLim = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);


        PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataLim, confirm);
        if (respon.IsSet)
        {
            if (respon.Response == PostDataParser.ResponseStatus.Success)
            {
                string storeId = hfStoreID.Text;

                PopulateDetail("c_po_outlet", numberId);

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
    
}
