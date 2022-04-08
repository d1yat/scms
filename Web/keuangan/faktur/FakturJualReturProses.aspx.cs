using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class keuangan_faktur_FakturJualReturProses : Scms.Web.Core.PageHandler
{
  #region Private

  private PostDataParser.StructureResponse SaveParser(string customerId, Dictionary<string, string>[] dicsProses)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.TagExtraName = "ProcessField";
    pair.DicValuesExtra = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    //decimal jml = 0,
    //  disc = 0,
    //  price = 0;
    string tmp = null,
      //itemId = null,
      noRetur = null,
      noDelivery = null,
      //typeId = null,
      exFaktur = null,
      cust = null,
      varData = null;
    bool isNew = false,
      isCabang = false;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);

    pair.DicAttributeValues.Add("Entry", this.Nip);

    pair.DicAttributeValues.Add("Customer", customerId);

    int nLoop = 0,
      nLen = 0;

    #region Process Details

    for (nLoop = 0, nLen = dicsProses.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dicsProses[nLoop];

      dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      isNew = dicData.GetValueParser<bool>("l_new");

      if (isNew)
      {
        dicAttr.Add("New", "true");

        noRetur = dicData.GetValueParser<string>("c_norc");
        noDelivery = dicData.GetValueParser<string>("c_nodo");
        //typeId = dicData.GetValueParser<string>("c_type");
        //itemId = dicData.GetValueParser<string>("c_iteno");
        //price = dicData.GetValueParser<decimal>("n_salpri", 0);
        //disc = dicData.GetValueParser<decimal>("n_disc", 0);
        //jml = dicData.GetValueParser<decimal>("n_qty", 0);
        cust = dicData.GetValueParser<string>("c_cusno");
        exFaktur = dicData.GetValueParser<string>("c_exno");
        isCabang = dicData.GetValueParser<bool>("l_cab");

        if (!Functional.DateParser(dicData.GetValueParser<string>("d_rcdate"), "yyyy-MM-ddTHH:mm:ss", out date))
        {
          date = DateTime.MinValue;
        }

        if ((!string.IsNullOrEmpty(noDelivery)) || (!string.IsNullOrEmpty(noRetur)))
        {
          dicAttr.Add("XFaktur", exFaktur);
          dicAttr.Add("ReturDate", date.ToString("yyyyMMddHHmmssfff"));
          dicAttr.Add("RC", noRetur);
          dicAttr.Add("DO", noDelivery);
          dicAttr.Add("Customer", cust);
          dicAttr.Add("IsCabang", isCabang.ToString().ToLower());
          //dicAttr.Add("Tipe", typeId);
          //dicAttr.Add("Item", itemId);
          //dicAttr.Add("Qty", jml.ToString());
          //dicAttr.Add("Disc", disc.ToString());
          //dicAttr.Add("Price", price.ToString());

          pair.DicValuesExtra.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr
          });
        }
      }

      dicData.Clear();
    }

    #endregion

    try
    {
      varData = parser.ParserData("FakturJualRetur", "Process", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("keuangan_faktur_FakturJualReturProses SaveParser : {0} ", ex.Message);
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

  private void ProsesLoadData(string customerNo, string rcFrom, string rcTo)
  {
    Dictionary<string, object> dicResult = null;
    List<Dictionary<string, string>> dicResultInfo = null;
    //Dictionary<string, string> dicData = null;
    Newtonsoft.Json.Linq.JArray jarr = null;
    //string kursId = null,
    //  fakturId = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    rcTo = (string.IsNullOrEmpty(rcTo) ? rcFrom : rcTo);

    string[][] paramX = new string[][]{
        new string[] { "customerNo", customerNo, "System.String"},
        new string[] { "rcFrom", rcFrom, "System.String"},
        new string[] { "rcTo", rcTo, "System.String"}
      };

    string res = soa.GlobalQueryService(0, -1, true, string.Empty, string.Empty, "14101", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<List<Dictionary<string, string>>>(jarr.First.First.ToString());
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("keuangan_faktur_FakturJualReturProses:ProsesLoadData Header - ", ex.Message));
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

    dicResult.Clear();
  }

  #endregion

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      DateTime date = DateTime.Now;

      Functional.PopulateBulan(cbBulan, date.Month);

      Functional.PopulateTahun(cbTahun, date.Year, 1, 0);
    }
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {

  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void ProcessFJR_Click(object sender, DirectEventArgs e)
  {
    //string customerNo = (e.ExtraParams["customerNo"] ?? string.Empty);
    //string rcFrom = (e.ExtraParams["rcFrom"] ?? string.Empty);
    //string rcTo = (e.ExtraParams["rcTo"] ?? string.Empty);

    //ProsesLoadData(customerNo, rcFrom, rcTo);

    string customerNo = (e.ExtraParams["customerNo"] ?? string.Empty);
    string blnAktif = (e.ExtraParams["bulan"] ?? string.Empty);
    string thnAktif = (e.ExtraParams["tahun"] ?? string.Empty);
  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string jsonStoreValues = (e.ExtraParams["storeValues"] ?? string.Empty);
    string customerId = (e.ExtraParams["customerId"] ?? string.Empty);

    Dictionary<string, string>[] gridData = JSON.Deserialize<Dictionary<string, string>[]>(jsonStoreValues);

    PostDataParser.StructureResponse respon = SaveParser(customerId, gridData);
    
    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
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
