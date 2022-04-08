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
using System.IO;
using System.Text;

public partial class proses_stock_opname_StockOpname : Scms.Web.Core.PageHandler
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            StockOpnameNewBatch1.Initialize(gridMain.ClientID, TxNoForm.Text);
            StockOpnamePerProduk1.Initialize(storeGridSP.ClientID, TxNoForm.ClientID);
        }

    }

    #region Load Data

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

        string gdgAsal      = pag.ActiveGudang;
        string DivAMS       = (e.ExtraParams["cbDivAms"] ?? string.Empty);
        string Suplier      = (e.ExtraParams["cbSuplier"] ?? string.Empty);
        string DivSuplier   = (e.ExtraParams["cbDivSuplier"] ?? string.Empty);
        string Kategori     = (e.ExtraParams["cbKategori"] ?? string.Empty);
        string Items        = (e.ExtraParams["cbItems"] ?? string.Empty);
        string Status       = (e.ExtraParams["cbStatus"] ?? string.Empty);

        if (Kategori == "")
        {
            e.ErrorMessage = "Kategori produk Harus diisi. Tidak dapat menampilkan data";

            e.Success = false;

            return;
        }

        PopulateData(gdgAsal, DivAMS, Suplier, DivSuplier, Kategori, Items, Status, "1");

    }

    [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void OnEvenAddGrid(object sender, DirectEventArgs e)
    {

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        string NoForm = (e.ExtraParams["PrimaryID"] ?? string.Empty);
        string gdgAsal = pag.ActiveGudang;

        PopulateData(gdgAsal, NoForm, "", "", "", "", "", "2");
        cbBentukForm.Text = "";

        GC.Collect();
    }

    private void PopulateData(string gdgAsal, string DivAMS, string Suplier, string DivSuplier, string Kategori, string Items, string Status, string Tipe)
    {
        Dictionary<string, object> dicResult = null;
        Dictionary<string, string> dicResultInfo = null;
        List<Dictionary<string, string>> lstResultInfo = null;
        Newtonsoft.Json.Linq.JArray jarr = null;

        Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

        string[][] paramX = new string[][]{
        new string[] { "gdgAsal", gdgAsal, "System.String"},
        new string[] { "DivAMS", DivAMS,  "System.String"},
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

        Ext.Net.Store store = gridMain.GetStore();
        if (store != null)
        {
            store.RemoveAll();
        }

        sb.AppendFormat("{0}.removeAll(); {0}.commitChanges(); ", gridMain.GetStore().ClientID);

        string stage, NoForm, KeepNoForm = null;

        bool NoFormSaved = true;

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

                    NoForm = dicResultInfo.GetValueParser<string>("noform", string.Empty);

                    if (NoFormSaved == false)
                    {
                        goto Done;
                    }

                    if (NoForm != null)
                    {
                        TxNoForm.Text = NoForm;
                    }
                    else if (NoForm != KeepNoForm)
                    {
                        TxNoForm.Text = "";
                        NoFormSaved = false;
                    }
                    if (KeepNoForm == null)
                    {
                        KeepNoForm = NoForm;
                    }
                    else if (NoForm != KeepNoForm)
                    {
                        TxNoForm.Text = "";
                        NoFormSaved = false;
                    }

                Done:                    

                    stage = dicResultInfo.GetValueParser<string>("stage", string.Empty);
                    if (stage == "0")
                    {
                        txKondisi.Text = "Buat Form";
                    }                    
                    else if (stage == "1")
                    {
                        txKondisi.Text = "SOQty";
                    }
                    else if (stage == "2")
                    {
                        txKondisi.Text = "Recount-1";
                    }
                    else if (stage == "3")
                    {
                        txKondisi.Text = "Recount-2";
                    }
                    else if (stage == "4")
                    {
                        txKondisi.Text = "Adjustment";
                    }
                    else
                    {
                        txKondisi.Text = "Disable";
                    }
                    
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
                    }})); ", gridMain.GetStore().ClientID,
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
                TxNoForm.Text = "";

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

    #endregion

    #region New Batch

    protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
    {
        if (!this.IsAllowAdd)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
            return;
        }

        if (TxNoForm.Text == "")
        {
            Functional.ShowMsgError("Masukan nomor form terlebih dahulu. Tidak dapat melelakukan penambahan batch.");
            return;
        }

        if (TxNoForm.Text == "Recount-2")
        {
            Functional.ShowMsgError("Sudah confirm Recount-2. Tidak dapat melelakukan penambahan batch.");
            return;
        }

        if (TxNoForm.Text == "Adjustment")
        {
            Functional.ShowMsgError("Sudah adjusment. Tidak dapat melelakukan penambahan batch.");
            return;
        }

        if (cbSuplier.Text == "")
        {
            Functional.ShowMsgError("Principle Kosong. Tidak dapat melelakukan penambahan batch.");
            return;
        }

        if (cbDivSuplier.Text == "")
        {
            Functional.ShowMsgError("Divisi Principle Kosong. Tidak dapat melelakukan penambahan batch.");
            return;
        }

        StockOpnameNewBatch1.CommandPopulate(true, null, TxNoForm.Text, cbSuplier.SelectedItem.Value, cbSuplier.SelectedItem.Text, cbDivSuplier.SelectedItem.Value, cbDivSuplier.SelectedItem.Text);
    }

    #endregion

    #region Buat Form SO

    protected void btnBuatFormSO_OnClick(object sender, DirectEventArgs e)
    {
        string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
        string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);

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

        if (cbBentukForm.Text == "")
        {
            Functional.ShowMsgError("Pilih bentuk form terlebih dahulu. Buat form dibatalkan");
            return;
        }

        Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

        PostDataParser.StructureResponse respon = SaveParserBuatFormSO(isAdd, numberId, gridDataPL);

        if (respon.IsSet)
        {
            if (respon.Response == PostDataParser.ResponseStatus.Success)
            {
                Functional.ShowMsgInformation("Penyimpanan berhasil. Form hasil SO tersimpan.");

                Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

                string gdgAsal = pag.ActiveGudang;
                string NoForm = TxNoForm.Text;

                PopulateData(gdgAsal, NoForm, "", "", "", "", "", "2");
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
               KdPrincipal = null,
               Principal = null,
               KdDivPrincipal = null,
               DivPrincipal = null,
               Location = null,
               Kategori = null,
               KdBarang = null,
               NmBarang = null,
               StBarang = null,
               Batch = null,
               QtySys = null,
               Status = null;

        decimal SOQty = 0,
                Recount1 = 0,
                Recount2 = 0,
                Selisih = 0,
                Box = 0;

        DateTime Expired;

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        DateTime date = DateTime.Today;

        if (cbSuplier.Text == "")
        {
            TxNoForm.Text = "";
        }

        else if (cbBentukForm.Text == "Per Div. Principal")
        {
            TxNoForm.Text = "";
        }

        else if (cbSuplier.Text != "")
        {
            TxNoForm.Text = pag.ActiveGudang +
                            cbSuplier.SelectedItem.Value +
                            cbKategori.SelectedItem.Value.Substring(1, 1) +
                            cbItems.SelectedItem.Value +
                            cbStatus.SelectedItem.Value.Substring(6,1);
        }

        dic.Add("ID", pair);
        pair.DicAttributeValues.Add("Gudang", pag.ActiveGudang);
        pair.DicAttributeValues.Add("BuatForm", (cbBentukForm.Text == "Per Principal" ? "0" : "1"));
        pair.DicAttributeValues.Add("Principal", cbSuplier.SelectedItem.Value);
        pair.DicAttributeValues.Add("DivPrincipal", cbDivSuplier.SelectedItem.Value);
        pair.DicAttributeValues.Add("Kategori", cbKategori.SelectedItem.Value.Substring(1,1));
        pair.DicAttributeValues.Add("Item", cbItems.SelectedItem.Value);
        

        if (cbStatus.SelectedItem.Value == null)
        {
            Status = "";
        }
        else if (cbStatus.SelectedItem.Value == "Stock Good")
        {
            Status = "G";
        }
        else
        {
            Status = "B";
        }
        pair.DicAttributeValues.Add("Status", Status);        

        for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
        {
            tmp = nLoop.ToString();

            dicData = dics[nLoop];

            dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            KdPrincipal = dicData.GetValueParser<string>("kdprincipal");
            Principal = dicData.GetValueParser<string>("principal");
            KdDivPrincipal = dicData.GetValueParser<string>("kddivprincipal");
            DivPrincipal = dicData.GetValueParser<string>("divprincipal");
            Location = "KOSONG";//dicData.GetValueParser<string>("location");
            KdBarang = dicData.GetValueParser<string>("kdbarang");
            NmBarang = dicData.GetValueParser<string>("nmbarang");
            StBarang = dicData.GetValueParser<string>("stbarang");
            Batch = dicData.GetValueParser<string>("batch");
            QtySys = dicData.GetValueParser<string>("qtysys");
            SOQty = dicData.GetValueParser<decimal>("SOQty");
            Recount1 = dicData.GetValueParser<decimal>("Recount-1");
            Recount2 = dicData.GetValueParser<decimal>("Recount-2");
            Selisih = dicData.GetValueParser<decimal>("selisih");
            Expired = dicData.GetValueParser<DateTime>("expired");
            Box = dicData.GetValueParser<decimal>("box");

            if ((!string.IsNullOrEmpty(QtySys)))
            {
                dicAttr.Add("Entry", pag.Nip);
                dicAttr.Add("Gudang", pag.ActiveGudang);
                dicAttr.Add("KdPrincipal", KdPrincipal);
                dicAttr.Add("Principal", Principal);
                dicAttr.Add("KdDivPrincipal", KdDivPrincipal);
                dicAttr.Add("DivPrincipal", DivPrincipal);
                dicAttr.Add("Location", Location);
                dicAttr.Add("KdBarang", KdBarang);
                dicAttr.Add("NmBarang", NmBarang);
                dicAttr.Add("StBarang", StBarang);
                dicAttr.Add("Batch", Batch);
                dicAttr.Add("QtySys", QtySys.ToString());
                dicAttr.Add("SOQty", SOQty.ToString());
                dicAttr.Add("Recount1", Recount1.ToString());
                dicAttr.Add("Recount2", Recount2.ToString());
                dicAttr.Add("Selisih", Selisih.ToString());
                dicAttr.Add("Expired", Expired.ToString("yyyyMMdd"));
                dicAttr.Add("Box", Box.ToString());

                pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
                {
                    IsSet = true,
                    DicAttributeValues = dicAttr
                });


            }
            dicData.Clear();
        }

        try
        {
            varData = parser.ParserData("StockOpnameBuatFormSO", (isAdd ? "Add" : "Modify"), dic);
        }
        catch (Exception ex)
        {
            Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_SuratPesananCtrl SaveParser : {0} ", ex.Message);
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

    #region Form SO dibatalkan

    protected void btnFormSOBatal_OnClick(object sender, DirectEventArgs e)
    {
        string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
        string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);

        bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;


        //if (pag.Nip != "001491E" && pag.Nip != "970922D" && pag.Nip != "170007H" && pag.Nip != "170007" && pag.Nip != "0000")
        //{
        //    Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menghapus form.");
        //    return;
        //}

        if (TxNoForm.Text == "")
        {
            Functional.ShowMsgError("Masukan nomor form terlebih dahulu. Tidak dapat melelakukan pembatalan form.");
            return;
        }
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

        if (txKondisi.Text == "Adjustment")
        {
            Functional.ShowMsgError("Tidak bisa membatalkan Form yang sudah Adjustment.");
            return;
        }

        Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

        PostDataParser.StructureResponse respon = SaveParserFormSOBatal(isAdd, numberId, gridDataPL);

        if (respon.IsSet)
        {
            if (respon.Response == PostDataParser.ResponseStatus.Success)
            {
                Functional.ShowMsgInformation("Pembatalan berhasil. Form hasil SO dibatlkan.");

                TxNoForm.Text = "";
                PopulateData("", "", "", "", "", "", "", "2");
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

    private PostDataParser.StructureResponse SaveParserFormSOBatal(bool isAdd, string plNumber, Dictionary<string, string>[] dics)
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

        string varData = null;            

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        DateTime date = DateTime.Today;

        dic.Add("ID", pair);
        pair.DicAttributeValues.Add("Noform", TxNoForm.Text);

        try
        {
            varData = parser.ParserData("StockOpnameFormSOBatal", (isAdd ? "Add" : "Modify"), dic);
        }
        catch (Exception ex)
        {
            Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_SuratPesananCtrl SaveParser : {0} ", ex.Message);
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

    #region Printing Stock Opname

    [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void btnPrintingSO_OnClick(object sender, DirectEventArgs e)
    {
        string tipereport = (e.ExtraParams["NumberID"] ?? string.Empty);
        string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
        Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        PostDataParser parser = new PostDataParser();
        IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

        Dictionary<string, string> dicData = null;

        PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

        Dictionary<string, string> dicAttr = null;

        Dictionary<string, string>[] dics = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

        pair.IsSet = true;
        pair.IsList = true;
        pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
        pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        string tmp = null,
               KdBarang = "",
               Batch = "";

        bool Print = false;

        //if (tipereport == "3" && txKondisi.Text != "Adjustment")
        //{
        //    Functional.ShowMsgError("No. from " + TxNoForm.Text + " Belum melakukan Adjusment. Cetak laporan dibatalkan.");
        //    return;
        //}

        if (cbTipeReport.Text == "")
        {
            Functional.ShowMsgError("Pilih dahulu tipe report. Cetak laporan dibatalkan.");
            return;
        }

        if (cbKateReport.Text == "")
        {
            Functional.ShowMsgError("Pilih dahulu kategori report. Cetak laporan dibatalkan.");
            return;
        }

        if (cbKateReport.Text == "Per No. Form" && TxNoForm.Text == "")
        {
            Functional.ShowMsgError("Masukan no. form jika kategori report adalah per no. from. Cetak laporan dibatalkan.");
            return;
        }

        for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
        {
            tmp = nLoop.ToString();

            dicData = dics[nLoop];

            dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            Print = dicData.GetValueParser<bool>("print");

            if (Print == true)
            {
                /*
                if (KdBarang == "")
                {
                    KdBarang = dicData.GetValueParser<string>("kdbarang");
                }
                else
                {
                    KdBarang = KdBarang + "," + dicData.GetValueParser<string>("kdbarang");

                }
                if (Batch == "")
                {
                    Batch = dicData.GetValueParser<string>("batch");
                }
                else
                {
                    Batch = Batch + "," + dicData.GetValueParser<string>("batch");
                }
                */

                if (KdBarang == "")
                {
                    KdBarang = dicData.GetValueParser<string>("kdbarang") + dicData.GetValueParser<string>("batch");
                }
                else
                {
                    KdBarang = KdBarang + "," + dicData.GetValueParser<string>("kdbarang") + dicData.GetValueParser<string>("batch");

                }                
            }


            dicData.Clear();
        }

        if (!Functional.CanCreateGenerateReport(pag))
        {
            return;
        }

        ReportParser rptParse = new ReportParser();

        List<ReportParameter> lstRptParam = new List<ReportParameter>();
        List<ReportCustomizeText> lstCustTxt = new List<ReportCustomizeText>();

        DateTime date1 = DateTime.Today,
          date2 = DateTime.Today;
        List<string> lstData = new List<string>();
        //string tmp = null;
        bool isAsync = false;

        string Hitung = "";

        if (txKondisi.Text == "Buat Form")
        {
            Hitung = "Hitung Awal";
        }
        else if (txKondisi.Text == "SOQty")
        {
            Hitung = "Recount Ke-1";
        }
        else if (txKondisi.Text == "Recount-1")
        {
            Hitung = "Recount Ke-2";
        }
        else if (txKondisi.Text == "Adjustment")
        {

        }
        rptParse.ReportingID = "0258";

        isAsync = false;

        #region Sql Parameter

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(DateTime).FullName,
            ParameterName = "TipeReport",
            IsSqlParameter = true,
            ParameterValue = tipereport
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(DateTime).FullName,
            ParameterName = "Gudang",
            IsSqlParameter = true,
            ParameterValue = pag.ActiveGudang
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "NoForm",
            IsSqlParameter = true,
            ParameterValue = cbKateReport.Text == "Semua Form" ? "00000" : TxNoForm.Text
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "Item",
            IsSqlParameter = true,
            ParameterValue = KdBarang == "" ? "0000" : KdBarang
        });
        /*
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "Batch",
            IsSqlParameter = true,
            ParameterValue = Batch == "" ? "0000" : Batch
        });
        */
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "Status",
            IsSqlParameter = true,
            ParameterValue = cbStatus.SelectedItem.Value
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "Cetak",
            IsSqlParameter = true,
            ParameterValue = cbTipeReport.Text == "PDF" ? "1" : "3"
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "Entry",
            IsSqlParameter = true,
            ParameterValue = pag.Nip
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "RPT_ReportSO.c_entry",
            ParameterValue = pag.Nip
        });

        #endregion

        #region Report parameter

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "TipeReport",
            IsSqlParameter = true,
            ParameterValue = tipereport
        });
        
        lstCustTxt.Add(new ReportCustomizeText()
        {
            SectionName = "Section1",
            ControlName = "PrintedBy",
            Value = ": " + pag.Nip + " - " + pag.Username
        });

        #endregion

        rptParse.PaperID = "A3";
        rptParse.IsLandscape = false;        
        rptParse.ReportCustomizeText = lstCustTxt.ToArray();
        rptParse.ReportParameter = lstRptParam.ToArray();
        rptParse.User = pag.Nip;

        rptParse.OutputReport = ReportParser.ParsingOutputReport(cbTipeReport.Text == "PDF" ? "01" : "03");

        string xmlFiles = ReportParser.Deserialize(rptParse);

        SoaReportCaller soa = new SoaReportCaller();

        string result = soa.GeneratorReport(isAsync, xmlFiles);

        ReportingResult rptResult = ReportingResult.Serialize(result);

        if (rptResult.IsSuccess)
        {
            string rptName = string.Concat(Hitung, "_", cbStatus.SelectedItem.Value, "_", pag.Nip, ".", rptResult.Extension);

            //string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
            //tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
            //  tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

            ////wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
            //Functional.GeneratorLoadedWindow("ctl00_cphContent_wndDown", tmpUri, LoadMode.IFrame);

            string tmpUri = Functional.UriDownloadGenerator(pag, rptResult.OutputFile, rptName, rptResult.Extension);

            wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
        }
        else
        {
            Functional.ShowMsgWarning(rptResult.MessageResponse);
        }

        GC.Collect();

    }

    #endregion

    #region Get Data First

    [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void btnGetData_OnClick(object sender, DirectEventArgs e)
    {
        string tipereport = (e.ExtraParams["NumberID"] ?? string.Empty);

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        if (!Functional.CanCreateGenerateReport(pag))
        {
            return;
        }

        ReportParser rptParse = new ReportParser();

        List<ReportParameter> lstRptParam = new List<ReportParameter>();
        List<ReportCustomizeText> lstCustTxt = new List<ReportCustomizeText>();

        DateTime date1 = DateTime.Today,
          date2 = DateTime.Today;
        List<string> lstData = new List<string>();
        //string tmp = null;
        bool isAsync = false;

        rptParse.ReportingID = "0258";

        isAsync = true;

        #region Sql Parameter

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(DateTime).FullName,
            ParameterName = "TipeReport",
            IsSqlParameter = true,
            ParameterValue = tipereport
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(DateTime).FullName,
            ParameterName = "Gudang",
            IsSqlParameter = true,
            ParameterValue = pag.ActiveGudang
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "NoForm",
            IsSqlParameter = true,
            ParameterValue = cbKateReport.Text == "Semua Form" ? "00000" : TxNoForm.Text
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "Item",
            IsSqlParameter = true,
            ParameterValue = "0000"
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "Batch",
            IsSqlParameter = true,
            ParameterValue = "00000"
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "Status",
            IsSqlParameter = true,
            ParameterValue = cbStatus.SelectedItem.Value
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "Cetak",
            IsSqlParameter = true,
            ParameterValue = cbTipeReport.Text == "PDF" ? "1" : "3"
        });

        #endregion

        #region Report parameter

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "TipeReport",
            IsSqlParameter = true,
            ParameterValue = tipereport
        });

        lstCustTxt.Add(new ReportCustomizeText()
        {
            SectionName = "Section1",
            ControlName = "PrintedBy",
            Value = ": " + pag.Nip + " - " + pag.Username
        });

        #endregion

        rptParse.PaperID = "A3";
        rptParse.ReportCustomizeText = lstCustTxt.ToArray();
        rptParse.ReportParameter = lstRptParam.ToArray();
        rptParse.IsLandscape = false;
        rptParse.User = pag.Nip;

        rptParse.OutputReport = ReportParser.ParsingOutputReport(cbTipeReport.Text == "PDF" ? "01" : "03");
        rptParse.IsShared = true;

        string xmlFiles = ReportParser.Deserialize(rptParse);

        SoaReportCaller soa = new SoaReportCaller();

        string result = soa.GeneratorReport(isAsync, xmlFiles);

        ReportingResult rptResult = ReportingResult.Serialize(result);

        if (rptResult == null)
        {
            Functional.ShowMsgError("Pembuatan report gagal.");
        }
        else
        {
            if (rptResult.IsSuccess)
            {
                if (isAsync)
                {
                    Functional.ShowMsgInformation("Proses sedang berjalan, mohon tunggu 1 menit.");
                }                
            }
            else
            {
                Functional.ShowMsgWarning(rptResult.MessageResponse);
            }
        }

        GC.Collect();
    }

    #endregion

    #region Input Hasil SO

    [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]    
    protected void BtnSaveData_Click(object sender, DirectEventArgs e)
    {
        string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
        string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
        
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

        if (txKondisi.Text == "Disable")
        {
            return;
        }

        Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

        PostDataParser.StructureResponse respon = SaveParserSaveData(isAdd, numberId, gridDataPL);

        if (respon.IsSet)
        {
            if (respon.Response == PostDataParser.ResponseStatus.Success)
            {
                
            }
            else
            {
                //e.ErrorMessage = respon.Message;

                //e.Success = false;
            }
        }
        else
        {
            e.ErrorMessage = "Unknown response";

            e.Success = false;
        }
    }

    private PostDataParser.StructureResponse SaveParserSaveData(bool isAdd, string plNumber, Dictionary<string, string>[] dics)
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
               Principal = null,
               DivPrincipal = null,
               Location = null,
               KdBarang = null,
               NmBarang = null,
               StBarang = null,
               Batch = null,
               Stage = null;

        decimal QtySys = 0,
                SOQty = 0,
                Recount1 = 0,
                Recount2 = 0,
                Selisih = 0,
                Box = 0;

        bool EditData = false;

        DateTime Expired;

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        DateTime date = DateTime.Today;

        dic.Add("ID", pair);

        for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
        {
            tmp = nLoop.ToString();

            dicData = dics[nLoop];

            dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            Principal = dicData.GetValueParser<string>("principal");
            DivPrincipal = dicData.GetValueParser<string>("divprincipal");
            Location = "KOSONG";//dicData.GetValueParser<string>("location");
            KdBarang = dicData.GetValueParser<string>("kdbarang");
            NmBarang = dicData.GetValueParser<string>("nmbarang");
            StBarang = dicData.GetValueParser<string>("stbarang");
            Batch = dicData.GetValueParser<string>("batch");
            QtySys = dicData.GetValueParser<decimal>("qtysys");
            SOQty = dicData.GetValueParser<decimal>("SOQty");
            Recount1 = dicData.GetValueParser<decimal>("Recount-1");
            Recount2 = dicData.GetValueParser<decimal>("Recount-2");
            Selisih = dicData.GetValueParser<decimal>("selisih");
            Expired = dicData.GetValueParser<DateTime>("expired");
            Box = dicData.GetValueParser<decimal>("box");
            Stage = dicData.GetValueParser<string>("stage");
            EditData = dicData.GetValueParser<Boolean>("saving");

            if (EditData)
            {
                dicAttr.Add("Entry", pag.Nip);
                dicAttr.Add("Gudang", pag.ActiveGudang);
                dicAttr.Add("Principal", Principal);
                dicAttr.Add("DivPrincipal", DivPrincipal);
                dicAttr.Add("Location", Location);
                dicAttr.Add("KdBarang", KdBarang);
                dicAttr.Add("NmBarang", NmBarang);
                dicAttr.Add("StBarang", StBarang);
                dicAttr.Add("Batch", Batch);
                dicAttr.Add("QtySys", QtySys.ToString());
                dicAttr.Add("SOQty", SOQty.ToString());
                dicAttr.Add("Recount1", Recount1.ToString());
                dicAttr.Add("Recount2", Recount2.ToString());
                dicAttr.Add("Selisih", Selisih.ToString());
                dicAttr.Add("Expired", Expired.ToString("yyyyMMdd"));
                dicAttr.Add("Box", Box.ToString());
                dicAttr.Add("Stage", Stage);

                pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
                {
                    IsSet = true,
                    DicAttributeValues = dicAttr
                });

            }
            dicData.Clear();
        }

        try
        {
            varData = parser.ParserData("StockOpname", (isAdd ? "Add" : "Modify"), dic);
        }
        catch (Exception ex)
        {
            Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_SuratPesananCtrl SaveParser : {0} ", ex.Message);
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

    #region SO Per Produk

    protected void btnPerProduk_OnClick(object sender, DirectEventArgs e)
    {
        if (!this.IsAllowAdd)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
            return;
        }

        StockOpnamePerProduk1.CommandPopulate(true, null, TxNoForm.Text);
    }

    #endregion

    #region Refresh Form

    protected void btnRefresh_OnClick(object sender, DirectEventArgs e)
    {
        if (TxNoForm.Text == "")
        {
            Functional.ShowMsgError("Masukan No. form terlebuh dahulu, refresh form dibatalkan.");
            return;
        }
        else
        {
            Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

            string gdgAsal = pag.ActiveGudang;

            PopulateData(gdgAsal, TxNoForm.Text, "", "", "", "", "", "2");
        }
    }

    #endregion

    #region Monitoring SO

    protected void btnMonitor_OnClick(object sender, DirectEventArgs e)
    {
        if (!this.IsAllowAdd)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
            return;
        }

        StockOpnameMonitoring1.CommandPopulate(true, null, TxNoForm.Text);
    }

    #endregion

    #region Confirm Hasil SO

    protected void btnConfirm_OnClick(object sender, DirectEventArgs e)
    {
        string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
        string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
        string Message = "";

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

        if (TxNoForm.Text == "")
        {
            Functional.ShowMsgError("Masukan nomor form terlebih dahulu. Tidak dapat melelakukan confirm hasil SO.");
            return;
        }

        Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

        PostDataParser.StructureResponse respon = SaveParserConfirm(isAdd, numberId, gridDataPL);

        if (respon.IsSet)
        {
            if (respon.Response == PostDataParser.ResponseStatus.Success)
            {
                if (txKondisi.Text == "Buat Form")
                {
                    Message = "Hitung Awal";
                }
                else if (txKondisi.Text == "SOQty")
                {
                    Message = "Recount-1";
                }
                else
                {
                    Message = "Recount-2";
                }

                Functional.ShowMsgInformation("Penyimpanan berhasil. Confirm " + Message + " tersimpan");

                Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

                string gdgAsal = pag.ActiveGudang;
                string NoForm = TxNoForm.Text;
                ChPrint.Checked = false;

                PopulateData(gdgAsal, NoForm, "", "", "", "", "", "2");
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

    private PostDataParser.StructureResponse SaveParserConfirm(bool isAdd, string plNumber, Dictionary<string, string>[] dics)
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
               Principal = null,
               DivPrincipal = null,
               Location = null,
               KdBarang = null,
               NmBarang = null,
               StBarang = null,
               Batch = null,
               QtySys = null;

        decimal SOQty = 0,
                Recount1 = 0,
                Recount2 = 0,
                Selisih = 0,
                Box = 0;

        DateTime Expired;

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        DateTime date = DateTime.Today;

        dic.Add("ID", pair);

        for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
        {
            tmp = nLoop.ToString();

            dicData = dics[nLoop];

            dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            Principal = dicData.GetValueParser<string>("principal");
            DivPrincipal = dicData.GetValueParser<string>("divprincipal");
            Location = "KOSONG";//dicData.GetValueParser<string>("location");
            KdBarang = dicData.GetValueParser<string>("kdbarang");
            NmBarang = dicData.GetValueParser<string>("nmbarang");
            StBarang = dicData.GetValueParser<string>("stbarang");
            Batch = dicData.GetValueParser<string>("batch");
            QtySys = dicData.GetValueParser<string>("qtysys");
            SOQty = dicData.GetValueParser<decimal>("SOQty");
            Recount1 = dicData.GetValueParser<decimal>("Recount-1");
            Recount2 = dicData.GetValueParser<decimal>("Recount-2");
            Selisih = dicData.GetValueParser<decimal>("selisih");
            Expired = dicData.GetValueParser<DateTime>("expired");
            Box = dicData.GetValueParser<decimal>("box");

            if ((!string.IsNullOrEmpty(QtySys)))
            {
                dicAttr.Add("Entry", pag.Nip);
                dicAttr.Add("Gudang", pag.ActiveGudang);
                dicAttr.Add("Principal", Principal);
                dicAttr.Add("DivPrincipal", DivPrincipal);
                dicAttr.Add("Location", Location);
                dicAttr.Add("KdBarang", KdBarang);
                dicAttr.Add("NmBarang", NmBarang);
                dicAttr.Add("StBarang", StBarang);
                dicAttr.Add("Batch", Batch);
                dicAttr.Add("QtySys", QtySys.ToString());
                dicAttr.Add("SOQty", SOQty.ToString());
                dicAttr.Add("Recount1", Recount1.ToString());
                dicAttr.Add("Recount2", Recount2.ToString());
                dicAttr.Add("Selisih", Selisih.ToString());
                dicAttr.Add("Expired", Expired.ToString("yyyyMMdd"));
                dicAttr.Add("Box", Box.ToString());

                pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
                {
                    IsSet = true,
                    DicAttributeValues = dicAttr
                });


            }
            dicData.Clear();
        }

        try
        {
            varData = parser.ParserData("StockOpnameConfirmSO", (isAdd ? "Add" : "Modify"), dic);
        }
        catch (Exception ex)
        {
            Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_SuratPesananCtrl SaveParser : {0} ", ex.Message);
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

    #region Adjusment SO

    protected void btnAdjust_OnClick(object sender, DirectEventArgs e)
    {
        string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
        string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);

        bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

        if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowDelete)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk adjusment stock.");
            return;
        }

        if (TxNoForm.Text == "")
        {
            Functional.ShowMsgError("Masukan nomor form terlebih dahulu. Tidak dapat melelakukan adjustment SO.");
            return;
        }
        if (txKondisi.Text == "Adjustment")
        {
            Functional.ShowMsgError("No. from " + TxNoForm.Text + " Sudah dilakukan adjustment. Adjustment stok dibatalkan.");
            return;
        }
        else if (txKondisi.Text != "Recount-2")
        {
            Functional.ShowMsgError("No. from " + TxNoForm.Text + " Belum confirm Recount-2. Adjustment stok dibatalkan.");
            return;
        }
        
        Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

        PostDataParser.StructureResponse respon = SaveParserAdjust(isAdd, numberId, gridDataPL);

        if (respon.IsSet)
        {
            if (respon.Response == PostDataParser.ResponseStatus.Success)
            {
                Functional.ShowMsgInformation("Penyimpanan berhasil. Proses adjustment stock selesai.");

                Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

                string gdgAsal = pag.ActiveGudang;
                string NoForm = TxNoForm.Text;

                PopulateData(gdgAsal, NoForm, "", "", "", "", "", "2");
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

    private PostDataParser.StructureResponse SaveParserAdjust(bool isAdd, string plNumber, Dictionary<string, string>[] dics)
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
               KdPrincipal = null,
               Principal = null,
               KdDivPrincipal = null,
               DivPrincipal = null,
               Location = null,
               KdBarang = null,
               NmBarang = null,
               StBarang = null,
               Batch = null,
               QtySys = null;

        decimal SOQty = 0,
                Recount1 = 0,
                Recount2 = 0,
                Selisih = 0,
                Box = 0;

        DateTime Expired;

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        DateTime date = DateTime.Today;

        dic.Add("ID", pair);
        pair.DicAttributeValues.Add("Entry", pag.Nip);
        pair.DicAttributeValues.Add("Gudang", pag.ActiveGudang);
        pair.DicAttributeValues.Add("Principal", TxNoForm.Text.Substring(1,5));

        for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
        {
            tmp = nLoop.ToString();

            dicData = dics[nLoop];

            dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            KdPrincipal = dicData.GetValueParser<string>("kdprincipal");
            Principal = dicData.GetValueParser<string>("principal");
            KdDivPrincipal = dicData.GetValueParser<string>("kddivprincipal");
            DivPrincipal = dicData.GetValueParser<string>("divprincipal");
            Location = "KOSONG";//dicData.GetValueParser<string>("location");
            KdBarang = dicData.GetValueParser<string>("kdbarang");
            NmBarang = dicData.GetValueParser<string>("nmbarang");
            StBarang = dicData.GetValueParser<string>("stbarang");
            Batch = dicData.GetValueParser<string>("batch");
            QtySys = dicData.GetValueParser<string>("qtysys");
            SOQty = dicData.GetValueParser<decimal>("SOQty");
            Recount1 = dicData.GetValueParser<decimal>("Recount-1");
            Recount2 = dicData.GetValueParser<decimal>("Recount-2");
            Selisih = dicData.GetValueParser<decimal>("selisih");
            Expired = dicData.GetValueParser<DateTime>("expired");
            Box = dicData.GetValueParser<decimal>("box");

            if ((!string.IsNullOrEmpty(QtySys)))
            {
                dicAttr.Add("Entry", pag.Nip);
                dicAttr.Add("Gudang", pag.ActiveGudang);
                dicAttr.Add("KdPrincipal", KdPrincipal);
                dicAttr.Add("Principal", Principal);
                dicAttr.Add("KdDivPrincipal", KdDivPrincipal);
                dicAttr.Add("DivPrincipal", DivPrincipal);
                dicAttr.Add("Location", Location);
                dicAttr.Add("KdBarang", KdBarang);
                dicAttr.Add("NmBarang", NmBarang);
                dicAttr.Add("StBarang", StBarang);
                dicAttr.Add("Batch", Batch);
                dicAttr.Add("QtySys", QtySys.ToString());
                dicAttr.Add("SOQty", SOQty.ToString());
                dicAttr.Add("Recount1", Recount1.ToString());
                dicAttr.Add("Recount2", Recount2.ToString());
                dicAttr.Add("Selisih", Selisih.ToString());
                dicAttr.Add("Expired", Expired.ToString("yyyyMMdd"));
                dicAttr.Add("Box", Box.ToString());

                pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
                {
                    IsSet = true,
                    DicAttributeValues = dicAttr
                });

            }
            dicData.Clear();
        }

        try
        {
            varData = parser.ParserData("StockOpnameAdjust", (isAdd ? "Add" : "Modify"), dic);
        }
        catch (Exception ex)
        {
            Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_SuratPesananCtrl SaveParser : {0} ", ex.Message);
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

    #region SO Ulang

    protected void btnSOUlang_OnClick(object sender, DirectEventArgs e)
    {
        string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
        string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);

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
        if (TxNoForm.Text == "")
        {
            Functional.ShowMsgError("Masukan nomor form terlebih dahulu. Tidak dapat melelakukan SO ulang.");
            return;
        }
        if (txKondisi.Text != "Adjustment")
        {
            Functional.ShowMsgError("No. from " + TxNoForm.Text + " belum melakukan adjusment. SO ulang dibatalkan");
            return;
        }

        Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

        PostDataParser.StructureResponse respon = SaveParserSOUlang(isAdd, numberId, gridDataPL);

        if (respon.IsSet)
        {
            if (respon.Response == PostDataParser.ResponseStatus.Success)
            {
                Functional.ShowMsgInformation("Penyimpanan berhasil. Proses SO ULang selesai.");

                Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

                string gdgAsal      = pag.ActiveGudang;
                string DivAMS       = cbDivAms.SelectedItem.Value;
                string Suplier      = cbSuplier.SelectedItem.Value;
                string DivSuplier   = cbDivSuplier.SelectedItem.Value;
                string Kategori     = cbKategori.SelectedItem.Value;
                string Items        = cbItems.SelectedItem.Value;
                string Status       = cbStatus.SelectedItem.Value;

                txKondisi.Text = "";
                TxNoForm.Text = "";
                PopulateData(gdgAsal, DivAMS, Suplier, DivSuplier, Kategori, Items, Status, "2");

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

    private PostDataParser.StructureResponse SaveParserSOUlang(bool isAdd, string plNumber, Dictionary<string, string>[] dics)
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
               KdPrincipal = null,
               Principal = null,
               KdDivPrincipal = null,
               DivPrincipal = null,
               Location = null,
               KdBarang = null,
               NmBarang = null,
               StBarang = null,
               Batch = null,
               QtySys = null,
               Status = null;

        decimal SOQty = 0,
                Recount1 = 0,
                Recount2 = 0,
                Selisih = 0,
                Box = 0;

        DateTime Expired;
              
        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        DateTime date = DateTime.Today;

        dic.Add("ID", pair);
        pair.DicAttributeValues.Add("Noform", TxNoForm.Text);
        pair.DicAttributeValues.Add("Entry", pag.Nip);

        for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
        {
            tmp = nLoop.ToString();

            dicData = dics[nLoop];

            dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            KdPrincipal = dicData.GetValueParser<string>("kdprincipal");
            Principal = dicData.GetValueParser<string>("principal");
            KdDivPrincipal = dicData.GetValueParser<string>("kddivprincipal");
            DivPrincipal = dicData.GetValueParser<string>("divprincipal");
            Location = "KOSONG";//dicData.GetValueParser<string>("location");
            KdBarang = dicData.GetValueParser<string>("kdbarang");
            NmBarang = dicData.GetValueParser<string>("nmbarang");
            StBarang = dicData.GetValueParser<string>("stbarang");
            Batch = dicData.GetValueParser<string>("batch");
            QtySys = dicData.GetValueParser<string>("qtysys");
            SOQty = dicData.GetValueParser<decimal>("SOQty");
            Recount1 = dicData.GetValueParser<decimal>("Recount-1");
            Recount2 = dicData.GetValueParser<decimal>("Recount-2");
            Selisih = dicData.GetValueParser<decimal>("selisih");
            Expired = dicData.GetValueParser<DateTime>("expired");
            Box = dicData.GetValueParser<decimal>("box");

            if ((!string.IsNullOrEmpty(QtySys)))
            {
                dicAttr.Add("Entry", pag.Nip);
                dicAttr.Add("Gudang", pag.ActiveGudang);
                dicAttr.Add("KdPrincipal", KdPrincipal);
                dicAttr.Add("Principal", Principal);
                dicAttr.Add("KdDivPrincipal", KdDivPrincipal);
                dicAttr.Add("DivPrincipal", DivPrincipal);
                dicAttr.Add("Location", Location);
                dicAttr.Add("KdBarang", KdBarang);
                dicAttr.Add("NmBarang", NmBarang);
                dicAttr.Add("StBarang", StBarang);
                dicAttr.Add("Batch", Batch);
                dicAttr.Add("QtySys", QtySys.ToString());
                dicAttr.Add("SOQty", SOQty.ToString());
                dicAttr.Add("Recount1", Recount1.ToString());
                dicAttr.Add("Recount2", Recount2.ToString());
                dicAttr.Add("Selisih", Selisih.ToString());
                dicAttr.Add("Expired", Expired.ToString("yyyyMMdd"));
                dicAttr.Add("Box", Box.ToString());

                pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
                {
                    IsSet = true,
                    DicAttributeValues = dicAttr
                });


            }
            dicData.Clear();
        }

        try
        {
            varData = parser.ParserData("StockOpnameSOUlang", (isAdd ? "Add" : "Modify"), dic);
        }
        catch (Exception ex)
        {
            Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_SuratPesananCtrl SaveParser : {0} ", ex.Message);
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
}
