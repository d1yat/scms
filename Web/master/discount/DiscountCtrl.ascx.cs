using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class keuangan_discount_DiscountCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    hfDiscNo.Clear();
    hfIdCode.Clear();

    lbItem.Text = "";

    txDiscOnHdr.Clear();
    txDiscOnHdr.Disabled = false;

    txDiscOffHdr.Clear();
    txDiscOffHdr.Disabled = false;

    txTglAwalHdr.Clear();
    txTglAwalHdr.Disabled = false;

    txTglAkhirHdr.Clear();
    txTglAkhirHdr.Disabled = false;

    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private void PopulateDetail(string pName, string itemField, string pID, string itemId)
  {
    this.ClearEntrys();

    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0}", pName), pID, "System.String"},
        new string[] { string.Format("{0}", itemField), itemId, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string tmp = null;
    DateTime date = DateTime.Today;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0112", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        hfDiscNo.Text = pID;
        hfItemNo.Text = itemId;

        tmp = dicResultInfo.GetValueParser<string>("v_itnam");
        winDetail.Title = string.Format("Discount - {0} ({1})", pID, tmp);

        lbItem.Text = string.Format("{0} - {1}", itemId, tmp);

        txDiscOnHdr.Text = dicResultInfo.GetValueParser<decimal>("n_discon").ToString();
        txDiscOffHdr.Text = dicResultInfo.GetValueParser<decimal>("n_discoff").ToString();

        date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_start"));
        txTglAwalHdr.Text = date.ToString("dd-MM-yyyy");
        //txTglAwalHdr.Disabled = true;

        date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_finish"));
        txTglAkhirHdr.Text = date.ToString("dd-MM-yyyy");
        //txTglAkhirHdr.Disabled = true;

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("keuangan_discount_DiscountCtrl:PopulateDetail Header - ", ex.Message));
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
            store.BaseParams.Add(new Ext.Net.Parameter("parameters", string.Format("[['{0}', '{1}', 'System.String'], ['{2}', '{3}', 'System.String']]", pName, pID, itemField, itemId), ParameterMode.Raw));
          }
          else
          {
            store.BaseParams["parameters"] = string.Format("[['{0}', '{1}', 'System.String'], ['{2}', '{3}', 'System.String']]", pName, pID, itemField, itemId);
          }
        }
      }

      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("keuangan_discount_DiscountCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  //private PostDataParser.StructureResponse SaveParser(bool isAdd, string memoNumber, Dictionary<string, string>[] dics)
  private PostDataParser.StructureResponse SaveParser(string discId, string itemId, decimal discOn, decimal discOff, string dateAwal, string dateAkhir, Dictionary<string, string>[] dics)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);
    
    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = discId;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string tmp = null,
      ket = null;
    bool isNew = false,
      isVoid = false,
      isModify = false,
      isAktif = false;
    string varData = null;

    decimal nIdx = 0;

    DateTime date = DateTime.Today;
    
    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Item", itemId);
    pair.DicAttributeValues.Add("DiscOn", discOn.ToString());
    pair.DicAttributeValues.Add("DiscOff", discOff.ToString());

    if (!Functional.DateParser(dateAwal, "yyyy-MM-ddTHH:mm:ss", out date))
    {
      date = DateTime.MinValue;
    }
    pair.DicAttributeValues.Add("DateStart", date.ToString("yyyyMMddHHmmssfff"));

    if (!Functional.DateParser(dateAkhir, "yyyy-MM-ddTHH:mm:ss", out date))
    {
      date = DateTime.MinValue;
    }
    pair.DicAttributeValues.Add("DateEnd", date.ToString("yyyyMMddHHmmssfff"));
    
    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      isNew = dicData.GetValueParser<bool>("l_new", false);
      isVoid = dicData.GetValueParser<bool>("l_void", false);
      isModify = dicData.GetValueParser<bool>("l_modified", false);
      isAktif = dicData.GetValueParser<bool>("l_aktif", false);

      ket = dicData.GetValueParser<string>("v_keterangan", string.Empty);

      dicAttr.Add("New", isNew.ToString().ToLower());
      dicAttr.Add("Delete", isVoid.ToString().ToLower());
      dicAttr.Add("Modified", isModify.ToString().ToLower());
      dicAttr.Add("Active", isAktif.ToString().ToLower());

      nIdx = dicData.GetValueParser<decimal>("IDX", 0);

      dateAwal = dicData.GetValueParser<string>("d_start");
      dateAkhir = dicData.GetValueParser<string>("d_finish");

      if (isNew && (!isVoid) && (!isModify) && isAktif)
      {
        dicAttr.Add("DiscOn", dicData.GetValueParser<decimal> ("n_discon", 0).ToString());
        dicAttr.Add("DiscOff", dicData.GetValueParser<decimal>("n_discoff", 0).ToString());

        dicAttr.Add("ID", nIdx.ToString());

        if (!Functional.DateParser(dateAwal, "yyyy-MM-ddTHH:mm:ss", out date))
        {
          date = DateTime.MinValue;
        }
        dicAttr.Add("DateStart", date.ToString("yyyyMMddHHmmssfff"));

        if (!Functional.DateParser(dateAkhir, "yyyy-MM-ddTHH:mm:ss", out date))
        {
          date = DateTime.MinValue;
        }
        dicAttr.Add("DateEnd", date.ToString("yyyyMMddHHmmssfff"));

        pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
        {
          IsSet = true,
          DicAttributeValues = dicAttr
        });
      }
      else if ((!isNew) && isModify && (!isVoid))
      {
        dicAttr.Add("DiscOn", dicData.GetValueParser<decimal>("n_discon", 0).ToString());
        dicAttr.Add("DiscOff", dicData.GetValueParser<decimal>("n_discoff", 0).ToString());

        dicAttr.Add("ID", nIdx.ToString());

        if (!Functional.DateParser(dateAwal, "yyyy-MM-ddTHH:mm:ss", out date))
        {
          date = DateTime.MinValue;
        }
        dicAttr.Add("DateStart", date.ToString("yyyyMMddHHmmssfff"));

        if (!Functional.DateParser(dateAkhir, "yyyy-MM-ddTHH:mm:ss", out date))
        {
          date = DateTime.MinValue;
        }
        dicAttr.Add("DateEnd", date.ToString("yyyyMMddHHmmssfff"));

        pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
        {
          IsSet = true,
          DicAttributeValues = dicAttr
        });
      }
      else if ((!isNew) && (!isModify) && isVoid)
      {
        ket = dicData.GetValueParser<string>("v_ket");

        dicAttr.Add("ID", nIdx.ToString());

        pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
        {
          IsSet = true,
          Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
          DicAttributeValues = dicAttr
        });
      }
      else
      {
        dicAttr.Clear();
      }

      dicData.Clear();
    }

    try
    {
      varData = parser.ParserData("MasterDiscount", "Modify", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("keuangan_discount_DiscountCtrl SaveParser : {0} ", ex.Message);
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

  public void CommandPopulate(string pID, string itemId)
  {
    PopulateDetail("noDiscount", "noItem", pID, itemId);
  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Page_Load(object sender, EventArgs e)
  {

  }
  
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string discId = (e.ExtraParams["DiscountID"] ?? string.Empty);
    string itemId = (e.ExtraParams["ItemID"] ?? string.Empty);
    string jsonStoreValues = (e.ExtraParams["storeValues"] ?? string.Empty);
    string discOn = (e.ExtraParams["DiscOn"] ?? string.Empty);
    string discOff = (e.ExtraParams["DiscOff"] ?? string.Empty);
    string tglAwal = (e.ExtraParams["TanggalAwal"] ?? string.Empty);
    string tglAkhir = (e.ExtraParams["TanggalAkhir"] ?? string.Empty);

    decimal decDiscOn = 0,
      decDiscOff = 0;

    decimal.TryParse(discOn, out decDiscOn);
    decimal.TryParse(discOff, out decDiscOff);

    Dictionary<string, string>[] gridData = JSON.Deserialize<Dictionary<string, string>[]>(jsonStoreValues);
    
    PostDataParser.StructureResponse respon = SaveParser(discId, itemId, decDiscOn, decDiscOff, tglAwal, tglAkhir, gridData);
    
    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        this.ClearEntrys();

        Functional.ShowMsgInformation("Data berhasil tersimpan.");

        winDetail.Hide();
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
