using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_pembelian_SuratPesananManual : Scms.Web.Core.PageHandler
{
    #region Private

    private  void ClearEntry()
    {
        cbTahun.SelectedIndex = 0;
        cbBulan.SelectedIndex = DateTime.Now.Date.Month - 1;

        if (this.ActiveGudang.Length < 2)
        {
            cbStatus.SelectedIndex = 0;
            cbCustomerHdr.Clear();
        }
        btnSave.Hidden = false;

        X.AddScript(string.Format("clearFilterGridHeader({0}, {1}, {2});reloadFilterGrid({0});", gridMain.ClientID, txSPFltr.ClientID, txDateFltr.ClientID));
    }

    private PostDataParser.StructureResponse SaveParser(Dictionary<string, string>[] dics)
    {
        PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

        PostDataParser parser = new PostDataParser();
        IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

        Dictionary<string, string> dicData = null;

        PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

        Dictionary<string, string> dicAttr = null;

        pair.IsSet = true;
        pair.IsList = true;
        //pair.Value = idNumber;
        pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
        pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        string tmp = null,
          item = null,
          tipe = null,
          ket = null;

        bool isModify = false,
            isAccepted = false,
            isDeclined = false;
        string varData = null;

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        DateTime date = DateTime.Today;

        dic.Add("ID", pair);
        pair.DicAttributeValues.Add("Entry", pag.Nip);
        
        for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
        {
            tmp = nLoop.ToString();

            dicData = dics[nLoop];

            isModify = dicData.GetValueParser<bool>("l_modified");

            if (isModify)
            {
                dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                item = dicData.GetValueParser<string>("c_iteno");
                tipe = dicData.GetValueParser<string>("c_type");
                if (tipe != "01")
                {
                    dicAttr.Add("NomorSP", dicData.GetValueParser<string>("c_spno"));
                    dicAttr.Add("Item", item);
                    dicAttr.Add("StatusSP", tipe);
                    dicAttr.Add("Acc", dicData.GetValueParser<string>("n_acc"));
                    dicAttr.Add("Modified", isModify.ToString().ToLower());

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
            varData = parser.ParserData("SuratPesananManual", "Confirm", dic);
        }
        catch (Exception ex)
        {
            Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_SuratPesananManual SaveParser : {0} ", ex.Message);
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

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            if (this.IsCabang)
            {
                Functional.SetComboData(cbCustomerHdr, "c_cusno", this.ActiveGudangDescription, this.ActiveGudang);
                cbCustomerHdr.Disabled = true;
            }
            SuratPesananManualCtrl1.Initialize(storeGridSP.ClientID, hfTypeName.Text);

            DateTime date = DateTime.Now;

            cbTahun.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
            date = date.AddYears(-1);
            cbTahun.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
            date = date.AddYears(-1);
            cbTahun.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
            cbTahun.SelectedIndex = 0;

            Functional.PopulateBulan(cbBulan, date.Month);

            X.AddScript(string.Format("reloadFilterGrid({0});", gridMain.ClientID));
        }
    }

    protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
    {
        if (!this.IsAllowAdd)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
            return;
        }

        SuratPesananManualCtrl1.CommandPopulate(true, null,this.ActiveGudang,this.ActiveGudangDescription);
    }

    [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void SaveBtn_Click(object sender, DirectEventArgs e)
    {
        string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);

        if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowEdit)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menyimpan data.");
            return;
        }

        Dictionary<string, string>[] gridDataSPM = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

        PostDataParser.StructureResponse respon = SaveParser(gridDataSPM);

        if (respon.IsSet)
        {
            if (respon.Response == PostDataParser.ResponseStatus.Success)
            {
                ClearEntry();
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

    protected void btnRefresh_OnClick(object sender, DirectEventArgs e)
    {
        ClearEntry();
    }

    protected void btnFilter_OnClick(object sender, DirectEventArgs e)
    {
        btnSave.Hidden = false;
        
        if (cbStatus.SelectedIndex > 0)
        {
            btnSave.Hidden = true;
        }
        X.AddScript(string.Format("clearFilterGridHeader({0}, {1}, {2});reloadFilterGrid({0});", gridMain.ClientID, txSPFltr.ClientID, txDateFltr.ClientID));
    }

    protected void btnPrintSP_OnClick(object sender, DirectEventArgs e)
    {
        if (!this.IsAllowPrinting)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
            return;
        }

        SuratPesananPrintCtrl1.ShowPrintPage();
    }

}
