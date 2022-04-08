using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class keuangan_faktur_FakturBeliReturProses : Scms.Web.Core.PageHandler
{
  #region Private

  //private PostDataParser.StructureResponse SaveParser(Dictionary<string, string>[] dicsProses)
  //{
  //  PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

  //  PostDataParser parser = new PostDataParser();
  //  IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

  //  Dictionary<string, string> dicData = null;

  //  PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

  //  Dictionary<string, string> dicAttr = null;

  //  pair.IsSet = true;
  //  pair.IsList = true;
  //  pair.TagExtraName = "ProcessField";
  //  pair.DicValuesExtra = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
  //  pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

  //  decimal bJml = 0,
  //    gJml = 0,
  //    disc = 0,
  //    price = 0;
  //  string tmp = null,
  //    itemId = null,
  //    noRetur = null,
  //    noPajak = null,
  //    exFaktur = null,
  //    varData = null;
  //  bool isNew = false;

  //  DateTime date = DateTime.Today;

  //  dic.Add("ID", pair);

  //  pair.DicAttributeValues.Add("Entry", this.Nip);

  //  int nLoop = 0,
  //    nLen = 0;

  //  #region Process Details

  //  for (nLoop = 0, nLen = dicsProses.Length; nLoop < nLen; nLoop++)
  //  {
  //    tmp = nLoop.ToString();

  //    dicData = dicsProses[nLoop];

  //    dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

  //    isNew = dicData.GetValueParser<bool>("l_new");

  //    if (isNew)
  //    {
  //      dicAttr.Add("New", "true");

  //      noRetur = dicData.GetValueParser<string>("c_norc");
  //      itemId = dicData.GetValueParser<string>("c_iteno");
  //      price = dicData.GetValueParser<decimal>("n_up_salpri", 0);
  //      disc = dicData.GetValueParser<decimal>("n_up_disc", 0);
  //      gJml = dicData.GetValueParser<decimal>("n_up_gsisa", 0);
  //      bJml = dicData.GetValueParser<decimal>("n_up_bsisa", 0);
  //      exFaktur = dicData.GetValueParser<string>("c_up_exfaktur");
  //      noPajak = dicData.GetValueParser<string>("c_up_taxno");

  //      if (!Functional.DateParser(dicData.GetValueParser<string>("d_up_taxdate"), "yyyy-MM-ddTHH:mm:ss", out date))
  //      {
  //        date = DateTime.MinValue;
  //      }

  //      if ((!string.IsNullOrEmpty(noRetur)) || (!string.IsNullOrEmpty(itemId)))
  //      {
  //        dicAttr.Add("RS", noRetur);
  //        dicAttr.Add("XFaktur", exFaktur);
  //        dicAttr.Add("TaxNo", noPajak);
  //        dicAttr.Add("TaxDate", date.ToString("yyyyMMddHHmmssfff"));
  //        dicAttr.Add("Item", itemId);
  //        dicAttr.Add("GQty", gJml.ToString());
  //        dicAttr.Add("BQty", bJml.ToString());
  //        dicAttr.Add("Disc", disc.ToString());
  //        dicAttr.Add("Price", price.ToString());

  //        pair.DicValuesExtra.Add(tmp, new PostDataParser.StructurePair()
  //        {
  //          IsSet = true,
  //          DicAttributeValues = dicAttr
  //        });
  //      }
  //    }

  //    dicData.Clear();
  //  }

  //  #endregion

  //  try
  //  {
  //    varData = parser.ParserData("FakturBeliRetur", "Process", dic);
  //  }
  //  catch (Exception ex)
  //  {
  //    Scms.Web.Common.Logger.WriteLine("keuangan_faktur_FakturBeliReturProses SaveParser : {0} ", ex.Message);
  //  }

  //  string result = null;

  //  if (!string.IsNullOrEmpty(varData))
  //  {
  //    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

  //    result = soa.PostData(varData);

  //    responseResult = parser.ResponseParser(result);
  //  }

  //  return responseResult;
  //}

  private string UploadData(bool isValidateOnly, string fileName, byte[] byts)
  {
    string result = null;

    Scms.Web.Core.SoaCaller soa = null;

    string[][] paramX = new string[][]{
        new string[] { "tipeRetur", "01", "System.String"},
        new string[] { "nipEntry", this.Nip, "System.String"},
        new string[] { "isValidate", isValidateOnly.ToString().ToLower(), "System.Boolean" }
      };

    if ((!string.IsNullOrEmpty(fileName)) && (byts != null) && (byts.Length > 0))
    {
      soa = new Scms.Web.Core.SoaCaller();

      result = soa.GlobalUploadData("UP100", fileName, byts, paramX);
    }

    return result;
  }

  private string PopulateData(string jsonData, Ext.Net.Store store)
  {
    string resp = null;
    
    Dictionary<string, object> dicResult = null;
    List<Dictionary<string, string>> listDicResultInfo = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;
    string[] arr = null;
    int nLoop = 0, 
      nLen = 0;
    System.Text.StringBuilder sb = new System.Text.StringBuilder();
    //string tmp = null;

    List<object[]> lstLists = null;
    List<object> lst = null;

    //Dictionary<string, object> dicData = null;
    List<Dictionary<string, object>> lstDicResult = null;
    Dictionary<string, object> dataDicResult = null;

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(jsonData);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (dicResult.GetValueParser<long>("totalRows") > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        listDicResultInfo = JSON.Deserialize<List<Dictionary<string, string>>>(jarr.First.ToString());

        //sb.AppendFormat("{0}.removeAll();", store.ClientID);

        lstLists = new List<object[]>();
        lst = new List<object>();
        lstDicResult = new List<Dictionary<string, object>>();

        for (nLoop = 0, nLen = listDicResultInfo.Count; nLoop < nLen; nLoop++)
        {
          dicResultInfo = listDicResultInfo[nLoop];

          if ((arr == null) || (arr.Length != dicResultInfo.Count))
          {
            arr = new string[dicResultInfo.Count];

            dicResultInfo.Keys.CopyTo(arr, 0);
          }

          sb.AppendFormat("{0}.insert(0, new Ext.data.Record({{", store.ClientID);

          lst.Add(dicResultInfo.GetValueParser<string>("c_gdg", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<string>("c_rsno", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<string>("c_iteno", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<string>("v_itnam", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<string>("c_batch", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<string>("c_fb", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<string>("c_taxno", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<string>("d_taxdate", string.Empty).Replace("\\", ""));
          lst.Add(dicResultInfo.GetValueParser<decimal>("n_disc", 0));
          lst.Add(dicResultInfo.GetValueParser<decimal>("n_salpri", 0));
          lst.Add(dicResultInfo.GetValueParser<decimal>("n_gsisa", 0));
          lst.Add(dicResultInfo.GetValueParser<decimal>("n_bsisa", 0));
          lst.Add(dicResultInfo.GetValueParser<string>("c_up_exfaktur", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<string>("c_up_taxno", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<string>("d_up_taxdate", string.Empty).Replace("\\", ""));
          lst.Add(dicResultInfo.GetValueParser<decimal>("n_up_salpri", 0));
          lst.Add(dicResultInfo.GetValueParser<decimal>("n_up_disc", 0));
          lst.Add(dicResultInfo.GetValueParser<decimal>("n_up_gsisa", 0));
          lst.Add(dicResultInfo.GetValueParser<decimal>("n_up_bsisa", 0));
          lst.Add(false);
          lst.Add(dicResultInfo.GetValueParser<string>("v_ket", string.Empty));

          lstLists.Add(lst.ToArray());

          lst.Clear();

          #region Old Coded

          //dicData = new Dictionary<string, object>();

          //for (nLoopC = 0, nLenC = arr.Length; nLoopC < nLenC; nLoopC++)
          //{
          //  tmp = arr[nLoop];

          //  if (dicData.ContainsKey(tmp))
          //  {
          //    continue;
          //  }

          //  switch (tmp)
          //  {
          //    case "c_gdg":
          //    case "c_rsno":
          //    case "c_iteno":
          //    case "v_itnam":
          //    case "c_batch":
          //    case "c_fb":
          //    case "c_taxno":
          //    case "c_up_exfaktur":
          //    case "c_up_taxno":
          //      //sb.AppendFormat("'{0}':'{1}'", tmp, dicResultInfo.GetValueParser<string>(tmp));

          //      //if ((nLoopC + 1) < nLenC)
          //      //{
          //      //  sb.Append(",");
          //      //}

          //      dicData.Add(tmp, dicResultInfo.GetValueParser<string>(tmp, string.Empty));

          //      break;
          //    case "d_taxdate":
          //    case "d_up_taxdate":
          //      //sb.AppendFormat("'{0}':new {1}", tmp, dicResultInfo.GetValueParser<string>(tmp).Replace("\\", "").Replace("/", ""));

          //      //if ((nLoopC + 1) < nLenC)
          //      //{
          //      //  sb.Append(",");
          //      //}

          //      dicData.Add(tmp, dicResultInfo.GetValueParser<string>(tmp, string.Empty).Replace("\\", ""));

          //      break;
          //    case "n_salpri":
          //    case "n_disc":
          //    case "n_gsisa":
          //    case "n_bsisa":
          //    case "n_up_salpri":
          //    case "n_up_disc":
          //    case "n_up_gsisa":
          //    case "n_up_bsisa":
          //      //sb.AppendFormat("'{0}':{1}", tmp, dicResultInfo.GetValueParser<decimal>(tmp));

          //      //if ((nLoopC + 1) < nLenC)
          //      //{
          //      //  sb.Append(",");
          //      //}

          //      dicData.Add(tmp, dicResultInfo.GetValueParser<decimal>(tmp, 0));

          //      break;
          //  }

          //}

          //lstDicResult.Add(dicData);

          #endregion

          sb.AppendFormat("}}));", store.ClientID);
        }

        dataDicResult = new Dictionary<string, object>();

        dataDicResult.Add("d", listDicResultInfo.ToArray());
        dataDicResult.Add("totalRows", listDicResultInfo.Count);
        dataDicResult.Add("success", true);

        //sb.AppendFormat("{0}.commitChanges();", store.ClientID);

        //X.AddScript(sb.ToString());

        //store.LoadData(sb, false);
        //store.LoadData(lstLists.ToArray(), false);
        //store.LoadData(dataDicResult, false);

        store.DataSource = lstLists.ToArray();
        store.DataBind();
        //store.AddScript("{0}.load( {{ params:{{ start:0, limit:parseInt({1}.getValue()) }} }} );", store.ClientID, cbGmPagingBB.ClientID);

        dataDicResult.Clear();
        listDicResultInfo.Clear();

        lstLists.Clear();

        sb.Remove(0, sb.Length);

        resp = null;
      }
      else if (dicResult.ContainsKey("exception") && (!string.IsNullOrEmpty(dicResult.GetValueParser<string>("exception"))))
      {
        resp = (string)dicResult.GetValueParser<string>("exception");
      }
    }
    catch (Exception ex)
    {
      resp = ex.Message;

      System.Diagnostics.Debug.WriteLine(
        string.Concat("keuangan_faktur_FakturBeliReturProses:PopulateDetail Header - ", ex.Message));
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

    return resp;
  }

  #endregion

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  protected void Proses_OnClick(object sender, DirectEventArgs e)
  {
    if (fuImportRS.HasFile)
    {
      string result = UploadData(chkVerify.Checked, fuImportRS.FileName, fuImportRS.FileBytes);

      if (string.IsNullOrEmpty(result))
      {
        e.ErrorMessage = "Unknown response";

        e.Success = false;
      }
      else
      {
        string resp = PopulateData(result, gridDetail.GetStore());

        if (!string.IsNullOrEmpty(resp))
        {
          e.ErrorMessage = resp;

          e.Success = false;
        }
      }
    }
    else
    {
      Functional.ShowMsgWarning("File tidak terbaca atau tidak ada.");
    }
  }

  //protected void SaveBtn_Click(object sender, DirectEventArgs e)
  //{
  //  string jsonStoreValues = (e.ExtraParams["storeValues"] ?? string.Empty);

  //  Dictionary<string, string>[] gridData = JSON.Deserialize<Dictionary<string, string>[]>(jsonStoreValues);

  //  PostDataParser.StructureResponse respon = SaveParser(gridData);

  //  if (respon.IsSet)
  //  {
  //    if (respon.Response == PostDataParser.ResponseStatus.Success)
  //    {
  //      Functional.ShowMsgInformation("Data berhasil tersimpan.");
  //    }
  //    else
  //    {
  //      e.ErrorMessage = respon.Message;

  //      e.Success = false;
  //    }
  //  }
  //  else
  //  {
  //    e.ErrorMessage = "Unknown response";

  //    e.Success = false;
  //  }
  //}
}
