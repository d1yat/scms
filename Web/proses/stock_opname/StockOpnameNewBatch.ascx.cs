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

public partial class proses_stock_opname_StockOpnameNewBatch : System.Web.UI.UserControl
{
    public void Initialize(string storeIDGridMain, string NoForm)
    {
        hfStoreID.Text = storeIDGridMain;
        //txExpired.Text = DateTime.Now.ToString("dd-MM-yyyy");
    }

    public void CommandPopulate(bool isAdd, string pID, string NoForm, string kodepemasok, string pemasok, string kodedivpemasok, string divpemasok)
    {
        if (isAdd)
        {
            winDetail2.Hidden = false;
            winDetail2.ShowModal();
            hfNomorForm.Text = NoForm;
            hfSupllier.Text = kodepemasok;
            hfDivSupllier.Text = kodedivpemasok;
            cbItems.Text = "";
            txBatch.Text = "";
            txExpired.Text = "";
            lbPemasok.Text = pemasok;
            lbDivPemasok.Text = divpemasok;
        }
        else
        {
            
        }
    }

    protected void SaveBtn_Click(object sender, DirectEventArgs e)
    {

        string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);

        PostDataParser.StructureResponse respon = SaveParser();

        if (respon.IsSet)
        {
            if (respon.Response == PostDataParser.ResponseStatus.Success)
            {
                Functional.ShowMsgInformation("Data berhasil tersimpan. Silahkan refresh data");

                cbItems.Text = "";
                txBatch.Text = "";
                txExpired.Text = "";

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

    private PostDataParser.StructureResponse SaveParser()
    {
        PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

        PostDataParser parser = new PostDataParser();
        IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

        Dictionary<string, string> dicData = null;

        PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

        Dictionary<string, string> dicAttr = null;

        pair.IsSet = true;
        pair.IsList = true;
        pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
        pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        string varData = null;

        DateTime Expired = DateTime.Today;

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        dic.Add("ID", pair);
        pair.DicAttributeValues.Add("Entry", pag.Nip);
        pair.DicAttributeValues.Add("Gudang", pag.ActiveGudang);
        pair.DicAttributeValues.Add("Item", cbItems.Text);
        pair.DicAttributeValues.Add("Batch", txBatch.Text);
        pair.DicAttributeValues.Add("Noform", hfNomorForm.Text);

        if (Scms.Web.Common.Functional.DateParser(txExpired.RawText.Trim(), "d-M-yyyy", out Expired))
        {
            dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            dicAttr.Add("Expired", Expired.ToString("yyyyMMdd"));

            pair.DicValues.Add("0", new PostDataParser.StructurePair()
            {
                IsSet = true,
                DicAttributeValues = dicAttr
            });
        }

        try
        {
            varData = parser.ParserData("StockOpnameCreateNewBatch", ("Add"), dic);
        }
        catch (Exception ex)
        {
            Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_PackingListCtrl SaveParser : {0} ", ex.Message);
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
}
