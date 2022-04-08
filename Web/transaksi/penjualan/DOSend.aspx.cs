using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_penjualan_DOSend : Scms.Web.Core.PageHandler
{
    private void ClearEntrys()
    {
        hfDONo.Clear();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            hfGudang.Text = this.ActiveGudang;
            hfGdgDesc.Value = this.ActiveGudangDescription;
            
            hfStoreID.Text = storeGridDOSend.ClientID;

            Functional.SetComboData(cbGudang3, "c_gdg", (string.IsNullOrEmpty(this.ActiveGudangDescription) ? string.Empty : this.ActiveGudangDescription), (string.IsNullOrEmpty(this.ActiveGudang) ? string.Empty : this.ActiveGudang));
            cbGudang3.Disabled = true;

            //this.storeGridPL.DataBind();
            //RowSelectionModel sm = this.gridMain.SelectionModel.Primary as RowSelectionModel;
            //sm.SelectedRows.Add(new SelectedRow(1));
        }
    }

    private PostDataParser.StructureResponse SaveParser(Dictionary<string, string>[] listNum, string sGudang, bool isAdd)
    {
        PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

        PostDataParser parser = new PostDataParser();
        IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);


        PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

        Dictionary<string, string> dicAttr = null;
        Dictionary<string, string> dicData = null;

        string varData = null,
          tmp = null,
          noTrans = null;

        DateTime tanggal = DateTime.MinValue;

        pair.IsSet = true;
        pair.IsList = true;
        pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
        pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dic.Add("ID", pair);
        pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
        //pair.DicAttributeValues.Add("Gudang", sGudang);


        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        if ((listNum.Length > 0) || !string.IsNullOrEmpty(sGudang) || !string.IsNullOrEmpty(hfGudang.Text))
        {

            for (int nLoop = 0, nLen = listNum.Length; nLoop < nLen; nLoop++)
            {
                tmp = nLoop.ToString();

                dicData = listNum[nLoop];

                //isNew = dicData.GetValueParser<bool>("l_send");
                dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                

                noTrans = dicData.GetValueParser<string>("c_dono");

                if ((!string.IsNullOrEmpty(noTrans)))
                {
                    dicAttr.Add("ID", noTrans);
                    dicAttr.Add("gudang", sGudang);
                    
                    dicAttr.Add("send", true.ToString().ToLower());

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
            varData = parser.ParserData("DOSEND", (isAdd ? "ConfirmSent" : "Delete"), dic);
        }
        catch (Exception ex)
        {
            Scms.Web.Common.Logger.WriteLine("DO Send SaveParser : {0} ", ex.Message);
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

    [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void SubmitSelection(object sender, DirectEventArgs e)
    {
        string json = e.ExtraParams["Values"];
        string tipeBtn = e.ExtraParams["BtnTipe"];  
        string katcol = e.ExtraParams["katcol"];

        Dictionary<string, string>[] lstNTrans = JSON.Deserialize<Dictionary<string, string>[]>(json);

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        bool isAdd = (tipeBtn == "1" ? true : false);


        if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
            return;
        }

        PostDataParser.StructureResponse respon = SaveParser(lstNTrans, hfGudang.Text, isAdd);

        if (respon.IsSet)
        {
            Functional.ShowMsgInformation(respon.Message);
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
