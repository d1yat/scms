using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_pembelian_LimitPOPrincipal : Scms.Web.Core.PageHandler
{
    private void ClearEntrys()
    {
    }

    [DirectMethod(ShowMask = true)]
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            DateTime date = DateTime.Now;

            cbPeriode1.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
            date = date.AddYears(-1);
            cbPeriode1.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
            date = date.AddYears(-1);
            cbPeriode1.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
            cbPeriode1.SelectedIndex = 0;

            Functional.PopulateBulan(cbPeriode2, date.Month);

            X.AddScript(string.Format("reloadFilterGrid({0});", gridMain.ClientID));
        }
    }


    [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void SaveBtn_Click(object sender, DirectEventArgs e)
    {
        string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);

        Dictionary<string, string>[] gridData = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

        bool isAdd = bool.Parse((e.ExtraParams["isAdd"] ?? "false"));

        PostDataParser.StructureResponse respon = SaveParser(isAdd, gridData);

        if (respon.IsSet)
        {
            Ext.Net.Store store = gridMain.GetStore();
            if (respon.Response == PostDataParser.ResponseStatus.Success)
            {
                if (isAdd)
                {
                    this.ClearEntrys();

                    X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
                    Functional.ShowMsgInformation("Data berhasil tersimpan.");
                }
                else
                {
                    
                    X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
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

    private PostDataParser.StructureResponse SaveParser(bool isAdd, Dictionary<string, string>[] dics)
    {
        PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

        PostDataParser parser = new PostDataParser();
        IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

        Dictionary<string, string> dicData = null;

        PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

        Dictionary<string, string> dicAttr = null;

        pair.IsSet = true;
        pair.IsList = true;
        pair.Value = "limitPO";
        pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
        pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        string tmp = null,
            varData = null,
            ketVoid = null;

        bool isNew = false,
          isModify = false;

        DateTime date = DateTime.Today;

        dic.Add("ID", pair);
        pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);

        int nLoop = 0,
          nLen = 0;

        #region Grid Details

        if (isAdd)
        {
            //pair.DicAttributeValues.Add("nTahun", cbPeriodeAdd1.SelectedItem.Value);
            //pair.DicAttributeValues.Add("nBulan", cbPeriodeAdd2.SelectedItem.Value);
            //pair.DicAttributeValues.Add("nosup", cbSuplierAdd.SelectedItem.Value);
            //pair.DicAttributeValues.Add("limit", txLimit.Text.Trim());
            //pair.DicAttributeValues.Add("percentage", txPercentage.Text.Trim());
        }
        else
        {
            for (nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
            {
                tmp = nLoop.ToString();

                dicData = dics[nLoop];

                dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                isNew = dicData.GetValueParser<bool>("l_new");
                isModify = dicData.GetValueParser<bool>("l_modified");

                if (isModify)
                {
                    dicAttr.Add("New", isNew.ToString().ToLower());
                    dicAttr.Add("Modified", isModify.ToString().ToLower());

                    dicAttr.Add("nosup", dicData.GetValueParser<string>("c_nosup"));
                    dicAttr.Add("nTahun", dicData.GetValueParser<string>("n_tahun"));
                    dicAttr.Add("nBulan", dicData.GetValueParser<string>("n_bulan"));
                    dicAttr.Add("nBestSls", dicData.GetValueParser<string>("n_besls"));
                    dicAttr.Add("nPercentAdj", dicData.GetValueParser<string>("n_percentadj"));
                    dicAttr.Add("nQty", dicData.GetValueParser<string>("n_qty"));
                    dicAttr.Add("Budget", dicData.GetValueParser<string>("n_budget"));
                    dicAttr.Add("availablebudget", dicData.GetValueParser<string>("n_availablebudget"));

                    pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
                    {
                        IsSet = true,
                        Value = (string.IsNullOrEmpty(ketVoid) ? "Human error" : ketVoid),
                        DicAttributeValues = dicAttr
                    });
                }

                dicData.Clear();
            }
        }

        #endregion

        try
        {
            varData = parser.ParserData("LimitPOPrincipal", (isAdd ? "Add" : "Modify"), dic);
        }
        catch (Exception ex)
        {
            Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_LimitPOPrincipalCtrl SaveParser : {0} ", ex.Message);
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
