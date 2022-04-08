using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_transfer_TransferGudangAuto : System.Web.UI.UserControl
{
  protected void Page_Load(object sender, EventArgs e)
  {

  }

  private void PopulateDetailRN(string pName, string pID)
  {
    //ClearEntrys();

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string[][] paramX = new string[][]{
            new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
        };

    string tmp = null;

    #region Parser Detail

    try
    {
      tmp = string.Format(@"var xOpts = {{
                                  params: {{
                                      start: '0',
                                      limit: '-1',
                                      allQuery: 'true',
                                      model: '100003',
                                      sort: '',
                                      dir: '',
                                      parameters: [['{0}', '{1}', 'System.String'],
                                                    ['{2}', '{3}', 'System.Char']]
                                    }}
                                  }};", pName, pID, "c_gdg", hfGudang.Text);

      X.AddScript(tmp);


      //hfSJNo.Text = pID;
      Ext.Net.Store store = gridDetail.GetStore();
      winDetail.Hidden = false;
      winDetail.ShowModal();
      //winDetail.Title = string.Format("Delivery Order - {0}", pID);
      X.AddScript(string.Format("{0}.removeAll();{0}.reload(xOpts);", store.ClientID));

      GC.Collect();
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_transfer_TransferGudang:PopulateDetail Detail - ", ex.Message));
    }


    #endregion

  }

  public void CommandPopulate(bool isAdd, string pID)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    if (isAdd)
    {

      winDetail.Hidden = false;
      winDetail.ShowModal();

      hfGudang.Text = pag.ActiveGudang;

      ClearEntrys();
    }
    else
    {
      TransferGudangCtrl.CommandPopulate(false, pID);
    }
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string gdgAsal, bool isConfirm, string doNumberID, Dictionary<string, string>[] dics)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = doNumberID;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string tmp = null,
    item = null, ket = null, spgno = null, batch = null,
    type_desc = null;

    decimal nQty = 0,
      nAdjust = 0;
    bool isNew = false,
      isVoid = false, isModify = false;
    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", pag.Nip);

    pair.DicAttributeValues.Add("From", gdgAsal);
    pair.DicAttributeValues.Add("To", cbToHdrAuto.Text);
    pair.DicAttributeValues.Add("Keterangan", txKeteranganAuto.Text);
    pair.DicAttributeValues.Add("NoRN", txRnNoAuto.Text);
    pair.DicAttributeValues.Add("StatusSJ", "true");

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      isNew = dicData.GetValueParser<bool>("l_new");
      isVoid = dicData.GetValueParser<bool>("l_void");
      isModify = dicData.GetValueParser<bool>("l_modified");

      if (isNew && (!isVoid) && (!isModify))
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        spgno = dicData.GetValueParser<string>("c_spgno");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        nQty = dicData.GetValueParser<decimal>("n_gqty", 0);
        nAdjust = dicData.GetValueParser<decimal>("n_adjust", 0);

        if ((!string.IsNullOrEmpty(spgno)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)) &&
          (nQty > 0))
        {
          dicAttr.Add("spgno", spgno);
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("Qty", nQty.ToString());
          dicAttr.Add("nQtyAdj", nAdjust.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr
          });
        }
      }

      dicData.Clear();

    }

    try
    {
      varData = parser.ParserData("TransferGudang", "Auto", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_TransferGudangCtrlAuto SaveParser : {0} ", ex.Message);
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

  public void Initialize(string gudang, string gudangDesc, string storeIDGridMain, bool isConfirm)
  {
    //hfGudang.Text = gudang;
    hfStoreID.Text = storeIDGridMain;
    hfGudangDesc.Text = gudangDesc;

    if (isConfirm)
    {
      hfConfMode.Text = bool.TrueString.ToLower();
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void btnSimpan_OnClick(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string NumberID = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jSonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);
    //string gdgId = (e.ExtraParams["GudangID"] ?? string.Empty);
    string gdgId = hfGudang.Text;
    //string gdgDesc = (e.ExtraParams["GudangDesc"] ?? string.Empty);
    string gdgId2 = (e.ExtraParams["GudangID2"] ?? string.Empty);
    string gdgDesc2 = (e.ExtraParams["GudangDesc2"] ?? string.Empty);
    string ket = (e.ExtraParams["Keterangan"] ?? string.Empty);
    string supl = (e.ExtraParams["Supplier"] ?? string.Empty);
    string TypeCategory = (e.ExtraParams["TypeCategory"] ?? string.Empty);

    bool isConfirm = false,
    isConfirmed = false;

    Dictionary<string, string>[] gridDataDO = JSON.Deserialize<Dictionary<string, string>[]>(jSonGridValues);

    bool isAdd = (string.IsNullOrEmpty(NumberID) ? true : false);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, gdgId, isConfirmed, NumberID, gridDataDO);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;

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

            NumberID = respon.Values.GetValueParser<string>("SJ", string.Empty);

            if (!string.IsNullOrEmpty(storeId))
            {
              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_gdg': '{1}',
                'c_sjno': '{2}',
                'd_sjdate': {3},
                'v_to': '{4}',
                'v_ket': '{5}',
                'l_print': false,
                'l_status': false,
                'l_confirm': false,
                'c_expno': '',
                'v_nama': '{6}'
              }}));{0}.commitChanges();", storeId,
                                        gdgId2,
                                        NumberID,
                                        dateJs, gdgDesc2, ket, supl);

              X.AddScript(scrpt);
            }
          }
        }

        //this.ClearEntrys();
        if (isAdd && (!string.IsNullOrEmpty(NumberID)))
        {
          //this.PopulateDetailRN("c_sjno", NumberID);

          winDetail.Hidden = true;

          TransferGudangCtrl.CommandPopulate(false, NumberID);
        }

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

  private void ClearEntrys()
  {
    hfSJNo.Clear();

    winDetail.Title = "Pesanan Gudang Auto";


    lbGudangFrom.Text = hfGudangDesc.Text;
    btnSave.Hidden = false;

    frmHeaders.Height = new Unit(175);

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void OnEvenAddGrid(object sender, DirectEventArgs e)
  {
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    PopulateDetailRN(pName, pID);

    GC.Collect();
  }
}
