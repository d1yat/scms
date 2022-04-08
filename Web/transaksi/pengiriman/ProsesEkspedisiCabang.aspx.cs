using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_pengiriman_ProsesEkspedisiCabang : Scms.Web.Core.PageHandler
{
  private string UploadData(bool isValidateOnly, string fileName, byte[] byts)
  {
    string result = null;

    Scms.Web.Core.SoaCaller soa = null;

    string[][] paramX = new string[][]{
        new string[] { "nipEntry", this.Nip, "System.String"},
        new string[] { "isValidate", isValidateOnly.ToString().ToLower(), "System.Boolean" }
      };

    if ((!string.IsNullOrEmpty(fileName)) && (byts != null) && (byts.Length > 0))
    {
      soa = new Scms.Web.Core.SoaCaller();

      result = soa.GlobalUploadData("100010101", fileName, byts, paramX);
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
    System.Text.StringBuilder sb = new System.Text.StringBuilder();

    List<object[]> lstLists = null;
    List<object> lst = null;

    List<Dictionary<string, object>> lstDicResult = null;
    Dictionary<string, object> dataDicResult = null;


    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(jsonData);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (dicResult.GetValueParser<long>("totalRows") > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        listDicResultInfo = JSON.Deserialize<List<Dictionary<string, string>>>(jarr.First.ToString());

        lstLists = new List<object[]>();
        lst = new List<object>();
        lstDicResult = new List<Dictionary<string, object>>();

        for (int nLoop = 0, nLen = listDicResultInfo.Count; nLoop < nLen; nLoop++)
        {
          dicResultInfo = listDicResultInfo[nLoop];

          if ((arr == null) || (arr.Length != dicResultInfo.Count))
          {
            arr = new string[dicResultInfo.Count];

            dicResultInfo.Keys.CopyTo(arr, 0);
          }

          sb.AppendFormat("{0}.insert(0, new Ext.data.Record({{", store.ClientID);

          lst.Add(dicResultInfo.GetValueParser<string>("expno", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<string>("resino", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<string>("tglresi", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<string>("tglexpcab", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<string>("wktexpcab", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<string>("Dtglresi", string.Empty).Replace("\\", ""));
          lst.Add(dicResultInfo.GetValueParser<string>("Dtglexpcab", string.Empty).Replace("\\", ""));
          lst.Add(dicResultInfo.GetValueParser<string>("Twktexpcab", string.Empty).Replace("\\", ""));
          lst.Add(dicResultInfo.GetValueParser<string>("v_ket", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<string>("c_type", string.Empty));
          lst.Add(true.ToString().ToLower());

          lstLists.Add(lst.ToArray());

          lst.Clear();

          sb.AppendFormat("}}));", store.ClientID);
        }

        dataDicResult = new Dictionary<string, object>();

        dataDicResult.Add("d", listDicResultInfo.ToArray());
        dataDicResult.Add("totalRows", listDicResultInfo.Count);
        dataDicResult.Add("success", true);

        store.DataSource = lstLists.ToArray();
        store.DataBind();

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
        string.Concat("Pengiriman_Proses_Ekspedisi_CabangProses:PopulateDetail Header - ", ex.Message));
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

  //  string tmp = null,
  //    customer = null,
  //    customerDesc = null,
  //    expno = null,
  //    expedisi = null,
  //    expedisiDesc = null,
  //    resino = null,
  //    varData = null;
  //  bool isNew = false;

  //  DateTime date = DateTime.Today,
  //    Dtglresi = DateTime.Today,
  //    Dtglexpcab = DateTime.Today;

  //  TimeSpan Twktexpcab = TimeSpan.MinValue, wktexpcab = TimeSpan.MinValue;

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

  //      expno = dicData.GetValueParser<string>("expno");
  //      resino = dicData.GetValueParser<string>("resino");
  //      //Dtglresi = dicData.GetValueParser<string>("Dtglresi");
  //      //Dtglexpcab = dicData.GetValueParser<string>("Dtglexpcab");
  //      //Twktexpcab = dicData.GetValueParser<string>("Twktexpcab");

  //      if (!Functional.DateParser(dicData.GetValueParser<string>("Dtglresi"), "yyyy-MM-ddTHH:mm:ss", out date))
  //      {
  //        Dtglresi = DateTime.MinValue;
  //      }
  //      Dtglresi = date;

  //      if (!Functional.DateParser(dicData.GetValueParser<string>("Dtglexpcab"), "yyyy-MM-ddTHH:mm:ss", out date))
  //      {
  //        Dtglexpcab = DateTime.MinValue;
  //      }
  //      Dtglexpcab = date;

  //      if (!TimeSpan.TryParse(dicData.GetValueParser<string>("wktexpcab"), out wktexpcab))
  //      {
  //        wktexpcab = TimeSpan.MinValue;
  //      }
  //      Twktexpcab = wktexpcab;

  //      if ((!string.IsNullOrEmpty(expno)) || (!string.IsNullOrEmpty(Dtglexpcab.ToString()))
  //        || (!string.IsNullOrEmpty(Dtglexpcab.ToString())) || (!string.IsNullOrEmpty(wktexpcab.ToString())))
  //      {
  //        dicAttr.Add("ExpId", expno);
  //        dicAttr.Add("resino", resino);
  //        dicAttr.Add("DateResi", Dtglresi.ToString("yyyyMMddHHmmssfff"));
  //        dicAttr.Add("Day", Dtglexpcab.ToString("yyyyMMddHHmmssfff"));
  //        dicAttr.Add("Time", Twktexpcab.ToString());

  //        pair.DicValuesExtra.Add(tmp, new PostDataParser.StructurePair()
  //        {
  //          IsSet = true,
  //          DicAttributeValues = dicAttr
  //        });

  //        if (dicData.GetValueParser<string>("c_type") == "02")
  //        {
            
  //        }
  //      }
  //    }

  //    dicData.Clear();
  //  }

  //  #endregion

  //  try
  //  {
  //    varData = parser.ParserData("EkspedisiCabang", "Process", dic);
  //  }
  //  catch (Exception ex)
  //  {
  //    Scms.Web.Common.Logger.WriteLine("Pengiriman_Proses_Ekspedisi_CabangProses SaveParser : {0} ", ex.Message);
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

  //protected void SaveBtn_Click(object sender, DirectEventArgs e)
  //{
  //  string jsonStoreValues = (e.ExtraParams["gridValues"] ?? string.Empty);

  //  Dictionary<string, string>[] gridData = JSON.Deserialize<Dictionary<string, string>[]>(jsonStoreValues);

  //  PostDataParser.StructureResponse respon = SaveParser(gridData);

  //  if (respon.IsSet)
  //  {
  //    if (respon.Response == PostDataParser.ResponseStatus.Success)
  //    {
  //      Functional.ShowMsgInformation("Data berhasil tersimpan.");

  //      Ext.Net.Store store = gridDetail.GetStore();
  //      store = gridDetail.GetStore();
  //      if (store != null)
  //      {
  //        store.RemoveAll();
  //      }
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

  protected void Proses_OnClick(object sender, DirectEventArgs e)
  {
    if (fuImportExp.HasFile)
    {
      string result = UploadData(chkVerify.Checked, fuImportExp.FileName, fuImportExp.FileBytes);

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
  }
}
