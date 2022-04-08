/*
 * Created By Indra
 * 20171231FM
 * 
*/

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class proses_stock_opname_StockOpnamePerProduk : System.Web.UI.UserControl
{
    public void Initialize(string storeIDGridMain, string NoForm)
    {
        hfStoreID.Text = storeIDGridMain;
        //TextForm.Text = NoForm;
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            //GetTypeName();
        }
    }

    public void CommandPopulate(bool isAdd, string pID, string NoForm)
    {
        if (isAdd)
        {
            winDetail.Hidden = false;
            winDetail.ShowModal();
            cbSuplier.Text = "";
            cbDivSuplier.Text = "";
            cbKategori.Text = "";
            cbStatus.Text = "";
            
            Ext.Net.Store storein = gridDetail.GetStore();
            if (storein != null)
            {
                storein.RemoveAll();
            }

            Ext.Net.Store storeout = gridDetailNewItem.GetStore();
            if (storeout != null)
            {
                storeout.RemoveAll();
            }
        }
        else
        {

        }
    }

    protected void GetBtn_Click(object sender, DirectEventArgs e)
    {
        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        if (!pag.IsAllowAdd)
        {
            //Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
            e.ErrorMessage = "Maaf, anda tidak mempunyai hak akses untuk menambah data.";

            e.Success = false;

            return;
        }

        string gdgAsal = pag.ActiveGudang;
        string Suplier = (e.ExtraParams["cbSuplier"] ?? string.Empty);
        string DivSuplier = (e.ExtraParams["cbDivSuplier"] ?? string.Empty);
        string Kategori = (e.ExtraParams["cbKategori"] ?? string.Empty);
        string Items = (e.ExtraParams["cbItems"] ?? string.Empty);
        string Status = (e.ExtraParams["cbStatus"] ?? string.Empty);

        if (Kategori == "")
        {
            e.ErrorMessage = "Kategori produk Harus diisi. Tidak dapat menampilkan data";

            e.Success = false;

            return;
        }

        if (Status == "")
        {
            e.ErrorMessage = "Status produk Harus diisi. Tidak dapat menampilkan data";

            e.Success = false;

            return;
        }


        PopulateData(gdgAsal, Suplier, DivSuplier, Kategori, Items, Status, "3");
    }

    private void PopulateData(string gdgAsal, string Suplier, string DivSuplier, string Kategori, string Items, string Status, string Tipe)
    {
        Dictionary<string, object> dicResult = null;
        Dictionary<string, string> dicResultInfo = null;
        List<Dictionary<string, string>> lstResultInfo = null;
        Newtonsoft.Json.Linq.JArray jarr = null;

        Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

        string[][] paramX = new string[][]{
        new string[] { "gdgAsal", gdgAsal, "System.String"},
        new string[] { "Suplier", Suplier,  "System.String"},
        new string[] { "DivSuplier", DivSuplier,  "System.String"},
        new string[] { "Kategori", Kategori,  "System.String"},
        new string[] { "Items", Items,  "System.String"},
        new string[] { "Status", Status,  "System.String"},
        new string[] { "Tipe", Tipe,  "System.String"},
      };

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        string res = soa.GlobalQueryService(0, -1, false, string.Empty, string.Empty, "0257", paramX);
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        Ext.Net.Store store = gridDetail.GetStore();
        if (store != null)
        {
            store.RemoveAll();
        }

        sb.AppendFormat("{0}.removeAll(); {0}.commitChanges(); ", gridDetail.GetStore().ClientID);

        int Nomor = 0;
        try
        {
            dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
            if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
            {
                jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

                lstResultInfo = JSON.Deserialize<List<Dictionary<string, string>>>(jarr.First.ToString());

                Nomor = lstResultInfo.Count;

                for (int nLoop = 0; nLoop < lstResultInfo.Count; nLoop++)
                {
                    dicResultInfo = lstResultInfo[nLoop];

                    DateTime Expired = DateTime.MinValue;
                    Expired = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("expired"));
                
                    
                    sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                    'nomor': {1},
                    'kdprincipal': '{2}',
                    'principal': '{3}',
                    'kddivprincipal': '{4}',
                    'divprincipal': '{5}',
                    'location': '{6}',
                    'kdbarang' : '{7}',
                    'nmbarang' : '{8}',
                    'stbarang': '{9}'
                    }})); ", gridDetail.GetStore().ClientID,
                           Nomor,
                           dicResultInfo.GetValueParser<string>("kdprincipal", string.Empty),
                           dicResultInfo.GetValueParser<string>("principal", string.Empty),
                           dicResultInfo.GetValueParser<string>("kddivprincipal", string.Empty),
                           dicResultInfo.GetValueParser<string>("divprincipal", string.Empty),
                           dicResultInfo.GetValueParser<string>("location", string.Empty),
                           dicResultInfo.GetValueParser<string>("kdbarang", string.Empty),
                           dicResultInfo.GetValueParser<string>("nmbarang", string.Empty),
                           dicResultInfo.GetValueParser<string>("stbarang", string.Empty)
                            );
                    

                    dicResultInfo.Clear();

                    Nomor--;

                }

                X.AddScript(sb.ToString());
            }
            else
            {
                Functional.ShowMsgError("Data atau No. Form Tidak ditemukan.");
                
                return;
            }
        }
        
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
              string.Concat("transaction_penjualan_PackingList:PopulateAutoGenerate - ", ex.Message));
        }

        sb.Remove(0, sb.Length);

        if (lstResultInfo != null)
        {
            lstResultInfo.Clear();
        }
        if (dicResult != null)
        {
            dicResult.Clear();
        }
        if (jarr != null)
        {
            jarr.Clear();
        }
    }

    protected void btnPilihPerProduk_OnClick(object sender, DirectEventArgs e)
    {
        string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
        string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
        
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

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

        Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

        PostDataParser.StructureResponse respon = SaveParserBuatFormSO(isAdd, numberId, gridDataPL);

        Functional.ShowMsgInformation("Pemilihan item berhasil. Form hasil SO tersimpan.");

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    }

    private PostDataParser.StructureResponse SaveParserBuatFormSO(bool isAdd, string plNumber, Dictionary<string, string>[] dics)
    {
        PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

        PostDataParser parser = new PostDataParser();
        IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

        Dictionary<string, string> dicData = null;

        PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

        Dictionary<string, string> dicAttr = null;

        pair.IsSet = true;
        pair.IsList = true;
        pair.Value = plNumber;
        pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
        pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        string tmp = null,
               varData = null,
               KdBarang = null,
               Status = null;

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        DateTime date = DateTime.Today;

        dic.Add("ID", pair);

        for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
        {
            tmp = nLoop.ToString();

            dicData = dics[nLoop];

            dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            KdBarang = KdBarang + dicData.GetValueParser<string>("kdbarang");
        }

        pair.DicAttributeValues.Add("Item", KdBarang);

        if (cbStatus.SelectedItem.Value == null)
        {
            Status = "";
        }
        else if (cbStatus.SelectedItem.Value == "Stock Good")
        {
            Status = "Stock Good";
        }
        else
        {
            Status = "Stock Bad";
        }
        pair.DicAttributeValues.Add("Status", Status);

        GetPopulateData(pag.ActiveGudang, cbSuplier.SelectedItem.Value, cbDivSuplier.SelectedItem.Value, cbKategori.SelectedItem.Value, KdBarang, Status, "4");

        return responseResult;
    }

    private void GetPopulateData(string gdgAsal, string Suplier, string DivSuplier, string Kategori, string Items, string Status, string Tipe)
    {
        Dictionary<string, object> dicResult = null;
        Dictionary<string, string> dicResultInfo = null;
        List<Dictionary<string, string>> lstResultInfo = null;
        Newtonsoft.Json.Linq.JArray jarr = null;

        Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

        string[][] paramX = new string[][]{
        new string[] { "gdgAsal", gdgAsal, "System.String"},
        new string[] { "Suplier", Suplier,  "System.String"},
        new string[] { "DivSuplier", DivSuplier,  "System.String"},
        new string[] { "Kategori", Kategori,  "System.String"},
        new string[] { "Items", Items,  "System.String"},
        new string[] { "Status", Status,  "System.String"},
        new string[] { "Tipe", Tipe,  "System.String"},
      };

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        string res = soa.GlobalQueryService(0, -1, false, string.Empty, string.Empty, "0257", paramX);
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        //sb.AppendFormat("{0}.removeAll(); {0}.commitChanges(); ", gridDetail.GetStore().ClientID);

        int Nomor = 0;
        try
        {
            dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
            if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
            {
                jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

                lstResultInfo = JSON.Deserialize<List<Dictionary<string, string>>>(jarr.First.ToString());

                Nomor = lstResultInfo.Count;

                for (int nLoop = 0; nLoop < lstResultInfo.Count; nLoop++)
                {
                    dicResultInfo = lstResultInfo[nLoop];

                    DateTime Expired = DateTime.MinValue;
                    Expired = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("expired"));

                    sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                    'nomor': {1},
                    'kdprincipal': '{2}',
                    'principal': '{3}',
                    'kddivprincipal': '{4}',
                    'divprincipal': '{5}',
                    'location': '{6}',
                    'kdbarang' : '{7}',
                    'nmbarang' : '{8}',
                    'stbarang': '{9}',
                    'batch': '{10}',
                    'qtysys': {11},
                    'SOQty' : {12},
                    'Recount-1' : {13},
                    'Recount-2' : {14},
                    'selisih' : {15},
                    'expired': '{16}',
                    'box': {17},
                    'stage' : '{18}'
                    }})); ", hfStoreID.Text,
                             Nomor,
                             dicResultInfo.GetValueParser<string>("kdprincipal", string.Empty),
                             dicResultInfo.GetValueParser<string>("principal", string.Empty),
                             dicResultInfo.GetValueParser<string>("kddivprincipal", string.Empty),
                             dicResultInfo.GetValueParser<string>("divprincipal", string.Empty),
                             dicResultInfo.GetValueParser<string>("location", string.Empty),
                             dicResultInfo.GetValueParser<string>("kdbarang", string.Empty),
                             dicResultInfo.GetValueParser<string>("nmbarang", string.Empty),
                             dicResultInfo.GetValueParser<string>("stbarang", string.Empty),
                             dicResultInfo.GetValueParser<string>("batch", string.Empty),
                             dicResultInfo.GetValueParser<string>("qtysys", string.Empty),
                             dicResultInfo.GetValueParser<string>("SOQty", string.Empty),
                             dicResultInfo.GetValueParser<string>("recount1", string.Empty),
                             dicResultInfo.GetValueParser<string>("recount2", string.Empty),
                             dicResultInfo.GetValueParser<string>("selisih", string.Empty),
                             Expired,
                             dicResultInfo.GetValueParser<string>("box", string.Empty),
                             dicResultInfo.GetValueParser<string>("stage", string.Empty)
                            );
                    

                    dicResultInfo.Clear();

                    Nomor--;

                }

                X.AddScript(sb.ToString());
            }
            else
            {
                Functional.ShowMsgError("Data atau No. Form Tidak ditemukan.");

                return;
            }
        }

        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
              string.Concat("transaction_penjualan_PackingList:PopulateAutoGenerate - ", ex.Message));
        }

        sb.Remove(0, sb.Length);

        if (lstResultInfo != null)
        {
            lstResultInfo.Clear();
        }
        if (dicResult != null)
        {
            dicResult.Clear();
        }
        if (jarr != null)
        {
            jarr.Clear();
        }
    }
}
