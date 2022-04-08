using System;
using System.Collections.Generic;
using System.Text;
using ScmsSoaLibrary.Commons;
using System.Net;
using System.IO;


namespace ScmsSoaLibrary.Parser
{
  public class ParserDisCore
  {
    Uri _uri;
    string _error;
    byte[] _resultData;

    public ParserDisCore() : this(null) { }

    public ParserDisCore(Uri uri)
    {
      this._error = string.Empty;
      this._resultData = null;
      this._uri = uri;
      this.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
    }

    //public Dictionary<string, object> ParsingRawData(string data)
    //{
    //  const string DEFAULT_RECORDCOUNT_NAME = "RecordCount";
    //  const string DEFAULT_RECORDDATA_NAME = "DataRow";

    //  if (string.IsNullOrEmpty(data))
    //  {
    //    return null;
    //  }

    //  Dictionary<string, object> obj = null;
    //  Dictionary<string, object> objResult = null;
    //  List<Dictionary<string, string>> dataRow = null;
    //  Newtonsoft.Json.Linq.JArray jarr = null;
    //  int recCount = 0;
    //  string tmp = null;

    //  try
    //  {
    //    obj = Ext.Net.JSON.Deserialize<Dictionary<string, object>>(data);

    //    if (obj.ContainsKey(DEFAULT_RECORDCOUNT_NAME))
    //    {
    //      tmp = obj[DEFAULT_RECORDCOUNT_NAME] as string;

    //      if (int.TryParse(tmp, out recCount))
    //      {
    //        if (recCount > 0 && obj.ContainsKey(DEFAULT_RECORDDATA_NAME))
    //        {
    //          jarr = obj[DEFAULT_RECORDDATA_NAME] as Newtonsoft.Json.Linq.JArray;

    //          try
    //          {
    //            dataRow = Ext.Net.JSON.Deserialize<List<Dictionary<string, string>>>(jarr.ToString());

    //            objResult = DictResultOk(dataRow);
    //          }
    //          catch (Exception ex)
    //          {
    //            objResult = DictResultError(ex.Message);
    //          }
    //        }
    //        else
    //        {
    //          objResult = DictResultError("Row kosong");
    //        }
    //      }
    //      else
    //      {
    //        objResult = DictResultError("Gagal parsing field 'RecordCount'.");
    //      }
    //    }
    //    else
    //    {
    //      objResult = DictResultError("Field 'RecordCount' tidak ditemukan.");
    //    }
    //  }
    //  catch (Exception ex)
    //  {
    //    tmp = string.Concat(ex.Message, "\n\r", data);

    //    objResult = DictResultError(tmp);
    //  }

    //  return objResult;
    //}

    //public Dictionary<string, object> DictResultError(string errorMsg)
    //{
    //  Dictionary<string, object> objResult = new Dictionary<string, object>();

    //  objResult.Add(Constant.DEFAULT_NAMING_RECORDS, null);

    //  objResult.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, 0);

    //  objResult.Add(Constant.DEFAULT_NAMING_SUCCESS, false);

    //  objResult.Add(Constant.DEFAULT_NAMING_EXCEPTION, errorMsg);

    //  return objResult;
    //}

    //public Dictionary<string, object> DictResultOk(List<Dictionary<string, string>> dataContent)
    //{
    //  if ((dataContent == null) || (dataContent.Count < 1))
    //  {
    //    return DictResultError("Data content empty");
    //  }

    //  Dictionary<string, object> objResult = new Dictionary<string, object>();

    //  objResult.Add(Constant.DEFAULT_NAMING_RECORDS, dataContent);

    //  objResult.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, dataContent.Count);

    //  objResult.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

    //  return objResult;
    //}

    public string ContentType
    { get; set; }

    public string Referer
    { get; set; }

    public bool PostData(Dictionary<string, string> param)
    {
      return PostData(this._uri, param, null);
    }

    public bool PostData(Uri uri, Dictionary<string, string> param)
    {
      return PostData(uri, param, null);
    }

    public bool PostData(Uri uri, Dictionary<string, string> param, Dictionary<string, string> header)
    {
      if (uri == null)
      {
        this._error = "Uri need.";

        return false;
      }

      this._error = string.Empty;
      this._resultData = null;

      bool bRes = false;
      bool continued100 = false;
      StringBuilder sb = new StringBuilder();
      System.IO.StreamWriter sw = null;
      System.IO.BinaryReader br = null;

      if ((param != null) && (param.Count > 0))
      {
        foreach (KeyValuePair<string, string> kvp in param)
        {
          sb.AppendLine(string.Concat(kvp.Key, "=", kvp.Value.Trim()));
        }
      }

      continued100 = System.Net.ServicePointManager.Expect100Continue;

      System.Net.ServicePointManager.Expect100Continue = false;

      System.Net.HttpWebRequest request = null;
      System.Net.HttpWebResponse respon = null;

      try
      {
        request = System.Net.WebRequest.Create(uri) as System.Net.HttpWebRequest;

        if ((header != null) && (header.Count > 0))
        {
          foreach (KeyValuePair<string, string> kvp in header)
          {
            request.Headers.Add(kvp.Key, kvp.Value);
          }
        }

        request.UserAgent = "Mr.Coding";
        request.Referer = this.Referer;
        request.Method = "POST";
        request.ContentType = this.ContentType;
        //webReq.KeepAlive = true;
        request.ContentLength = sb.Length;

        sw = new System.IO.StreamWriter(request.GetRequestStream());

        sw.Write(sb.ToString());

        sw.Close();

        respon = request.GetResponse() as System.Net.HttpWebResponse;

        if (respon != null)
        {
          if (respon.StatusCode == System.Net.HttpStatusCode.OK)
          {
            br = new System.IO.BinaryReader(respon.GetResponseStream());

            if (br.BaseStream.CanRead)
            {
              _resultData = br.ReadBytes((int)respon.ContentLength);
            }
            else
            {
              this._error = "Failed to read response";
            }

            br.Close();

            bRes = true;
          }
        }
      }
      catch (Exception ex)
      {
        this._error = ex.Message;
      }
      finally
      {
        if (respon != null)
        {
          respon.Close();
        }

        if (request != null)
        {
          request.Abort();
        }
      }

      System.Net.ServicePointManager.Expect100Continue = continued100;

      return bRes;
    }

    public bool PostGetData(Uri uri, Dictionary<string, string> param, Dictionary<string, string> header)
    {
      if (uri == null)
      {
        this._error = "Uri need.";

        return false;
      }

      this._error = string.Empty;
      this._resultData = null;

      bool bRes = false;
      bool continued100 = false;
      System.IO.StreamWriter sw = null;
      System.IO.BinaryReader br = null;

      string paramString = ParameterParser(param);

      continued100 = System.Net.ServicePointManager.Expect100Continue;

      System.Net.ServicePointManager.Expect100Continue = false;

      System.Net.HttpWebRequest request = null;
      System.Net.HttpWebResponse respon = null;

      try
      {
        request = System.Net.WebRequest.Create(uri) as System.Net.HttpWebRequest;

        if ((header != null) && (header.Count > 0))
        {
          foreach (KeyValuePair<string, string> kvp in header)
          {
            request.Headers.Add(kvp.Key, kvp.Value);
          }
        }

        request.UserAgent = "SCMS";
        request.Referer = this.Referer;
        request.Method = "POST";
        request.ContentType = this.ContentType;
        //webReq.KeepAlive = true;
        request.ContentLength = paramString.Length;

        sw = new System.IO.StreamWriter(request.GetRequestStream());

        sw.Write(paramString);

        sw.Close();

        respon = request.GetResponse() as System.Net.HttpWebResponse;

        if (respon != null)
        {            
          if (respon.StatusCode == System.Net.HttpStatusCode.OK)
          {
              if (respon.ContentLength < 0)
              {
                  using (Stream responseStream = request.GetResponse().GetResponseStream())
                  {
                      int bytes;
                      MemoryStream memoryStream = new MemoryStream(0x40000);
                      byte[] buffer = new byte[0x4000];
                      while ((bytes = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                      {
                          memoryStream.Write(buffer, 0, bytes);
                      }
                      var tes = Encoding.UTF8.GetString(memoryStream.ToArray());

                      _resultData = memoryStream.ToArray();
                  }
              }
              else
              {
                  br = new System.IO.BinaryReader(respon.GetResponseStream());

                  if (br.BaseStream.CanRead)
                  {
                      _resultData = br.ReadBytes((int)respon.ContentLength);
                  }
                  else
                  {
                      this._error = "Failed to read response";
                  }

                  br.Close();
              }            

            bRes = true;
          }
        }
      }
      catch (Exception ex)
      {
        this._error = ex.Message;
      }
      finally
      {
        if (respon != null)
        {
          respon.Close();
        }

        if (request != null)
        {
          request.Abort();
        }
      }

      System.Net.ServicePointManager.Expect100Continue = continued100;

      return bRes;
    }

    public bool GetData(Uri uri, Dictionary<string, string> param, Dictionary<string, string> header)
    {
      if (uri == null)
      {
        this._error = "Uri need.";

        return false;
      }

      this._error = string.Empty;
      this._resultData = null;

      bool bRes = false;
      bool continued100 = false;
      //StringBuilder sb = new StringBuilder();
      System.IO.BinaryReader br = null;
      //List<string> list = new List<string>();

      UriBuilder uriB = new UriBuilder(uri);

      //if ((param != null) && (param.Count > 0))
      //{
      //  foreach (KeyValuePair<string, string> kvp in param)
      //  {
      //    list.Add(string.Concat(kvp.Key, "=", kvp.Value.Trim()));
      //  }

      //  sb.Append(string.Join("&", list.ToArray()));

      //  list.Clear();

      //  uriB.Query = (string.IsNullOrEmpty(uriB.Query) ? sb.ToString() : string.Concat(uriB.Query, "&", sb.ToString()));
      //}

      string paramString = ParameterParser(param);

      if (!string.IsNullOrEmpty(paramString))
      {
        uriB.Query = (string.IsNullOrEmpty(uriB.Query) ? paramString : string.Concat(uriB.Query, "&", paramString));
      }

      continued100 = System.Net.ServicePointManager.Expect100Continue;

      System.Net.ServicePointManager.Expect100Continue = false;

      System.Net.HttpWebRequest request = null;
      System.Net.HttpWebResponse respon = null;

      try
      {
        request = System.Net.WebRequest.Create(uriB.Uri) as System.Net.HttpWebRequest;

        if ((header != null) && (header.Count > 0))
        {
          foreach (KeyValuePair<string, string> kvp in header)
          {
            request.Headers.Add(kvp.Key, kvp.Value);
          }
        }

        request.UserAgent = "Mr.Coding";
        request.Referer = this.Referer;
        request.Method = "GET";
        request.ContentType = this.ContentType;
        //webReq.KeepAlive = true;
        //request.ContentLength = sb.Length;

        respon = request.GetResponse() as System.Net.HttpWebResponse;

        if (respon != null)
        {
          if (respon.StatusCode == System.Net.HttpStatusCode.OK)
          {
            br = new System.IO.BinaryReader(respon.GetResponseStream());

            if (br.BaseStream.CanRead)
            {
              _resultData = br.ReadBytes((int)respon.ContentLength);
            }
            else
            {
              this._error = "Failed to read response";
            }

            br.Close();

            bRes = true;
          }
        }
      }
      catch (Exception ex)
      {
        this._error = ex.Message;
      }
      finally
      {
        if (respon != null)
        {
          respon.Close();
        }

        if (request != null)
        {
          request.Abort();
        }
      }

      System.Net.ServicePointManager.Expect100Continue = continued100;

      return bRes;
    }

    public byte[] Result
    { get { return (_resultData == null ? new byte[0] : this._resultData); } }

    public string ErrorMessage
    { get { return _error; } }

    #region Static

    public static Dictionary<string, object> ParsingFromDisCore(string data)
    {
      const string DEFAULT_RECORDCOUNT_NAME = "RecordCount";
      const string DEFAULT_SUCCESS_NAME = "success";
      const string DEFAULT_RECORDDATAROW_NAME = "DataRow";
      const string DEFAULT_RECORDDATA_NAME = "data";

      if (string.IsNullOrEmpty(data))
      {
        return DictResultError("Data kosong.");
      }

      Dictionary<string, object> obj = null;
      Dictionary<string, object> objResult = null;
      List<Dictionary<string, string>> lstDataRow = null;
      Dictionary<string, string> dataRow = null;
      Newtonsoft.Json.Linq.JArray jarr = null;
      int recCount = 0;
      string tmp = null;
      Newtonsoft.Json.Linq.JObject jobj = null;

      try
      {
        obj = Ext.Net.JSON.Deserialize<Dictionary<string, object>>(data);

        if (obj.ContainsKey(DEFAULT_SUCCESS_NAME))
        {
          jobj = obj[DEFAULT_RECORDDATA_NAME] as Newtonsoft.Json.Linq.JObject;

          if (jobj == null)
          {
            objResult = DictResultError("Data field tidak ditemukan/kosong.");
          }
          else
          {
            try
            {
              dataRow = Ext.Net.JSON.Deserialize<Dictionary<string, string>>(jobj.ToString());

              objResult = DictResultOkSingle(dataRow);
            }
            catch (Exception ex)
            {
              objResult = DictResultError(ex.Message);
            }
          }
        }
        else if (obj.ContainsKey(DEFAULT_RECORDCOUNT_NAME))
        {
          tmp = obj[DEFAULT_RECORDCOUNT_NAME] as string;

          if (int.TryParse(tmp, out recCount))
          {
            if (recCount > 0 && obj.ContainsKey(DEFAULT_RECORDDATAROW_NAME))
            {
              jarr = obj[DEFAULT_RECORDDATAROW_NAME] as Newtonsoft.Json.Linq.JArray;

              try
              {
                lstDataRow = Ext.Net.JSON.Deserialize<List<Dictionary<string, string>>>(jarr.ToString());

                objResult = DictResultOk(lstDataRow);
              }
              catch (Exception ex)
              {
                objResult = DictResultError(ex.Message);
              }
            }
            else
            {
              objResult = DictResultError("Row kosong");
            }
          }
          else
          {
            objResult = DictResultError("Gagal parsing field 'RecordCount'.");
          }
        }
        else
        {
          objResult = DictResultError("Field 'RecordCount' tidak ditemukan.");
        }
      }
      catch (Exception ex)
      {
        tmp = string.Concat(ex.Message, "\n\r", data);

        objResult = DictResultError(tmp);
      }

      return objResult;
    }

    public static Dictionary<string, string> ParsingFromDisCoreSwitch(string data)
    {
      //const string DEFAULT_RECORDCOUNT_NAME = "RecordCount";
      //const string DEFAULT_RECORDDATA_NAME = "DataRow";

      Dictionary<string, string> obj = null;
      Dictionary<string, string> objResult = null;
      //List<Dictionary<string, string>> dataRow = null;
      //Newtonsoft.Json.Linq.JArray jarr = null;
      //int recCount = 0;
      //string tmp = null;

      obj = Ext.Net.JSON.Deserialize<Dictionary<string, string>>(data);

      //jarr = obj[DEFAULT_RECORDDATA_NAME] as Newtonsoft.Json.Linq.JArray;


      objResult = obj;
     
      return objResult;
     
    }

    private static Dictionary<string, object> DictResultError(string errorMsg)
    {
      Dictionary<string, object> objResult = new Dictionary<string, object>();

      objResult.Add(Constant.DEFAULT_NAMING_RECORDS, null);

      objResult.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, 0);

      objResult.Add(Constant.DEFAULT_NAMING_SUCCESS, false);

      objResult.Add(Constant.DEFAULT_NAMING_EXCEPTION, errorMsg);

      return objResult;
    }

    private static Dictionary<string, object> DictResultOkSingle(Dictionary<string, string> dataContent)
    {
      if ((dataContent == null) || (dataContent.Count < 1))
      {
        return DictResultError("Data content empty");
      }

      Dictionary<string, object> objResult = new Dictionary<string, object>();

      objResult.Add(Constant.DEFAULT_NAMING_RECORDS, dataContent);

      objResult.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, 1);

      objResult.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

      return objResult;
    }

    private static Dictionary<string, object> DictResultOk(List<Dictionary<string, string>> dataContent)
    {
      if ((dataContent == null) || (dataContent.Count < 1))
      {
        return DictResultError("Data content empty");
      }

      Dictionary<string, object> objResult = new Dictionary<string, object>();

      objResult.Add(Constant.DEFAULT_NAMING_RECORDS, dataContent);

      objResult.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, dataContent.Count);

      objResult.Add(Constant.DEFAULT_NAMING_SUCCESS, true);

      return objResult;
    }

    public static string ParameterParser(Dictionary<string, string> dicParam)
    {
      StringBuilder sb = new StringBuilder();

      List<string> list = new List<string>();

      if ((dicParam != null) && (dicParam.Count > 0))
      {
        foreach (KeyValuePair<string, string> kvp in dicParam)
        {
          if (string.IsNullOrEmpty(kvp.Key.Trim()))
          {
            list.Add(kvp.Value.Trim());
          }
          else
          {
            list.Add(string.Concat(kvp.Key, "=", kvp.Value.Trim()));
          }
        }

        sb.Append(string.Join("&", list.ToArray()));

        list.Clear();

        dicParam.Clear();
      }

      return sb.ToString();
    }

    public static Dictionary<string, string> ParameterParserDec(string raw)
    {
      Dictionary<string, string> ret = new Dictionary<string, string>();

      StringBuilder sb = new StringBuilder();

      List<string> list = new List<string>();

      string[] perLine = raw.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
      string[] perHeader = null;

      for (int nLoop = 0, nLen = perLine.Length; nLoop < nLen; nLoop++)
      {
        perHeader = perLine[nLoop].Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

        if ((perHeader != null) && (perHeader.Length == 2))
        {
          ret.Add(perHeader[0], perHeader[1]);
        }
      }

      return ret;
    }

    public static string HeaderParserEnc(Dictionary<string, string> dicParam)
    {
      StringBuilder sb = new StringBuilder();

      List<string> list = new List<string>();

      if ((dicParam != null) && (dicParam.Count > 0))
      {
        foreach (KeyValuePair<string, string> kvp in dicParam)
        {
          if ((!kvp.Key.Contains(":")) || (!kvp.Value.Contains("~")))
          {
            list.Add(string.Concat(kvp.Key, "=", kvp.Value.Trim()));
          }
        }

        sb.Append(string.Join("~", list.ToArray()));

        list.Clear();

        dicParam.Clear();
      }

      return sb.ToString();
    }

    public static Dictionary<string, string> HeaderParserDec(string raw)
    {
      Dictionary<string, string> ret = new Dictionary<string, string>();

      StringBuilder sb = new StringBuilder();

      List<string> list = new List<string>();

      string[] perLine = raw.Split(new string[] { "~" },  StringSplitOptions.RemoveEmptyEntries);
      string[] perHeader = null;

      for (int nLoop = 0, nLen = perLine.Length; nLoop < nLen; nLoop++)
      {
        perHeader = perLine[nLoop].Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

        if ((perHeader != null) && (perHeader.Length  == 2))
        {
          ret.Add(perHeader[0], perHeader[1]);
        }
      }

      return ret;
    }

    public static string GeneralizeObject(string sParams)
    {
      
      string sValue = null;
      string result = null;
      System.Xml.XmlNode node = null;
      
      try
      {

        string sUri = "xmlns=http://scms.ams.org",
          sSymbols_1 = "&lt;", sSymbols_2 = "&gt;";
        char cSymbols_2 = ('"');
        string sUris = "http://scms.ams.org";
        string sSymbols_3 = cSymbols_2.ToString();
        string sName = null;

        if (sParams.Contains(sSymbols_1))
        {
          sParams = sParams.Replace(sSymbols_1.ToString(), "<");
        }
        if (sParams.Contains("'"))
        {
          sParams = sParams.Replace("'", sSymbols_3.ToString());
        }
        if (sParams.Contains(sUri))
        {
          sParams = sParams.Replace(sUri.ToString(), "");
        }
        if (sParams.Contains(sUris))
        {
          sParams = sParams.Replace(sUris.ToString(), "");
        }
        if (sParams.Contains(sSymbols_2))
        {
          sParams = sParams.Replace(sSymbols_2.ToString(), ">");
        }

        
        System.Xml.XmlDocument xd = new System.Xml.XmlDocument();
        xd.LoadXml(sParams);

        System.Xml.XmlElement elem = (System.Xml.XmlElement)xd.DocumentElement.FirstChild;
        //elem.SetAttribute("MasterItem", "MasterItemB");
        //sValue = elem.InnerXml;
        
        System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
        doc.LoadXml(elem.InnerXml);
        node = doc.SelectSingleNode("Structure");
        sName = (node.Attributes["name"] == null ? string.Empty : node.Attributes["name"].Value);

        switch (sName)
        {
          case "MasterItem":
            {
              node.Attributes["name"].Value = "SyncMasterItem";
            }
            break;
          case "SuratPesanan":
            {
              node.Attributes["name"].Value = "SyncSuratPesanan";
            }
            break;
          case "Recall":
            {
                node.Attributes["name"].Value = "SyncRecall";
            }
            break;
          case "Relocation":
            { 
                node.Attributes["name"].Value = "SyncRelokasi";
            }
            break;
          case "ReceiveRelocation":
            {
                node.Attributes["name"].Value = "SyncReceiveRelokasi";
            }
            break;
          case "CancelPBRelocation":
            {
                node.Attributes["name"].Value = "SyncCancelPBRelocation";
            }
            break;
          case "CancelSPRelocation":
            { 
                node.Attributes["name"].Value = "SyncCancelSPRelocation";
            }
            break;
          case "MasterRelocation":
            {
                node.Attributes["name"].Value = "SyncMasterRelocation";
            }
            break;
          case "ReceivePO":
            {
                node.Attributes["name"].Value = "SyncReceivePO";
            }
            break;
        }

        sValue = node.OuterXml;
      }
      catch (Exception ex)
      {
        result = string.Format("ScmsSoaLibrary.Parser.ParserDisCore.GeneralizeObject - {0} - {1} ", ex.Message, sParams);

        ScmsSoaLibraryInterface.Commons.Logger.WriteLine(result, true);
        ScmsSoaLibraryInterface.Commons.Logger.WriteLine(ex.StackTrace);
      }

      return sValue;
    }

    #endregion
  }
}
