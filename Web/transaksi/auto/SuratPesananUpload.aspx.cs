﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_auto_SuratPesananUpload : Scms.Web.Core.PageHandler
{
  protected void Page_Load(object sender, EventArgs e)
  {

  }

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

      result = soa.GlobalUploadData("up111", fileName, byts, paramX);
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

        for (nLoop = 0, nLen = listDicResultInfo.Count; nLoop < nLen; nLoop++)
        {
          dicResultInfo = listDicResultInfo[nLoop];

          if ((arr == null) || (arr.Length != dicResultInfo.Count))
          {
            arr = new string[dicResultInfo.Count];

            dicResultInfo.Keys.CopyTo(arr, 0);
          }
          DateTime date = DateTime.MinValue;
          bool isDate = Functional.DateParser(dicResultInfo.GetValueParser<string>("TglSP", string.Empty), "yyyyMMdd", out date);

          sb.AppendFormat("{0}.insert(0, new Ext.data.Record({{", store.ClientID);

          lst.Add(dicResultInfo.GetValueParser<string>("c_sp", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<string>("TipeSP", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<string>("kdCab", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<string>("kdCabSCMS", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<string>("Cabang", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<string>("kodeitem", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<string>("namaItem", string.Empty));
          lst.Add(dicResultInfo.GetValueParser<DateTime>("TglSP", DateTime.MinValue));
          lst.Add(dicResultInfo.GetValueParser<decimal>("Qty", 0));

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

  protected void Proses_OnClick(object sender, DirectEventArgs e)
  {
    if (fuImportFB.HasFile)
    {
      string result = UploadData(chkVerify.Checked, fuImportFB.FileName, fuImportFB.FileBytes);

      if (string.IsNullOrEmpty(result))
      {
        e.ErrorMessage = "Unknown response";

        e.Success = false;
      }
      else
      {
        string resp = PopulateData(result, gridDetail.GetStore());
      }
    }
    else
    {
      Functional.ShowMsgWarning("File tidak terbaca atau tidak ada.");
    }
  }
}
