using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using Scms.Web.Core;
using System.Globalization;
using ScmsSoaLibraryInterface.Core.Crypto;

namespace Scms.Web.Common
{
  /// <summary>
  /// Summary description for Functional
  /// </summary>
  public class Functional
  {
    #region Helper Class 

    [Serializable]
    public class ScmsHttpCookie
    {
      public string Domain { get; set; }
      public DateTime Expires { get; set; }
      public bool HttpOnly { get; set; }
      public string Name { get; set; }
      public string Path { get; set; }
      public bool Secure { get; set; }
      public string Value { get; set; }
      public ScmsHttpCookieValue[] Values { get; set; }
    }

    [Serializable]
    public class ScmsHttpCookieValue
    {
      public string Name { get; set; }
      public string Value { get; set; }
    }

    #endregion

    private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    private static readonly Random randRandom = new Random((int)DateTime.Now.Ticks);

    public Functional()
    {
      //
      // TODO: Add constructor logic here
      //
    }

    public void RecursiveReplaceStoreUrl(ControlCollection cc)
    {
      const string CALLBACK_NAME = "soaScmsCallback";

      string addr = StaticObjects.SoaAddress;
      int port = StaticObjects.SoaPort;

      int nLen = cc.Count;
      Ext.Net.Store store = null;
      Ext.Net.ScriptTagProxy stp = null;
      UriBuilder uriB = null;

      for (int nLoop = 0; nLoop < nLen; nLoop++)
      {
        if(cc[nLoop].Controls.Count > 0)
        {
          RecursiveReplaceStoreUrl(cc[nLoop].Controls);
        }
        if (cc[nLoop].GetType().Equals(typeof(Ext.Net.Store)))
        {
          store = cc[nLoop] as Ext.Net.Store;
          if ((store != null) && (store.Proxy.Count > 0))
          {
            stp = store.Proxy[0] as Ext.Net.ScriptTagProxy;
            if ((stp != null) && (stp.CallbackParam.Equals(CALLBACK_NAME, StringComparison.OrdinalIgnoreCase)))
            {
              uriB = new UriBuilder(stp.Url);

              uriB.Host = addr;
              uriB.Port = port;

              stp.Url = uriB.ToString();
            }
          }
        }
      }
    }

    public static CultureInfo ActiveCulture
    { get { return new CultureInfo("id-ID"); } }

    public static bool DateParser(string raw, string format, out DateTime date)
    {
      CultureInfo culture = ActiveCulture;

      DateTime dat = DateTime.Today;

      bool isOk = false;

      if (DateTime.TryParseExact(raw, format, culture, DateTimeStyles.AssumeLocal, out dat))
      {
        date = dat;

        isOk = true;
      }
      else
      {
        date = DateTime.MinValue;
      }

      return isOk;
    }

    public static bool DateParser(string raw, string format, CultureInfo culture, out DateTime date)
    {
      DateTime dat = DateTime.Today;

      bool isOk = false;

      if (DateTime.TryParseExact(raw, format, culture, DateTimeStyles.AssumeLocal, out dat))
      {
        date = dat;

        isOk = true;
      }
      else
      {
        date = DateTime.MinValue;
      }

      return isOk;
    }

    public static string DateToJson(DateTime date)
    {
      string result = null;

      try
      {
        result = string.Format("Date({0})", (long)((DateTime)date).ToUniversalTime().Subtract(unixEpoch).TotalMilliseconds);
      }
      catch (Exception ex)
      {
        Logger.WriteLine("Scms.Web.Common.Functional:DateToJson - {0}", ex.Message);
      }

      return result;
    }

    public static DateTime JSDateToModern(string jsDate)
    {
      if (string.IsNullOrEmpty(jsDate))
      {
        return DateTime.MinValue;
      }

      const string DEFAULT_FORMAT_JS = "ddd MMM d yyyy HH:mm:ss GMTzzzzz";

      DateTime result = DateTime.MinValue;
      string tmp = result.ToString(DEFAULT_FORMAT_JS);
      int nLen = tmp.Length;
      double dbl = 0;

      try
      {
        tmp = ((jsDate.Length > nLen) ? jsDate.Substring(0, nLen).Trim() : jsDate);

        if (!Functional.DateParser(tmp, DEFAULT_FORMAT_JS, CultureInfo.InvariantCulture, out result))
        {
          dbl = Microsoft.JScript.DateConstructor.parse(jsDate);

          result = Functional.JsonDateToDate(dbl);
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine("Scms.Web.Common.Functional:JSDateToModern - {0}", ex.Message);
      }

      return result;
    }

    public static DateTime JsonDateToDate(string jsonDate)
    {
      if (string.IsNullOrEmpty(jsonDate))
      {
        return DateTime.MinValue;
      }

      DateTime result = DateTime.MinValue;
      string tmp = null;

      try
      {
        int i1 = jsonDate.IndexOf("(", StringComparison.OrdinalIgnoreCase) + 1;
        int i2 = jsonDate.IndexOf(")", StringComparison.OrdinalIgnoreCase);

        tmp = jsonDate.Substring(i1, (i2 - i1));

        result = unixEpoch.ToLocalTime().AddMilliseconds((double)Convert.ToInt64(tmp));
      }
      catch (Exception ex)
      {
        Logger.WriteLine("Scms.Web.Common.Functional:JsonToDate - {0}", ex.Message);
      }

      return result;
    }

    public static DateTime JsonDateToDate(double jsonDate)
    {
      if (jsonDate == 0)
      {
        return DateTime.MinValue;
      }

      DateTime result = DateTime.MinValue;

      try
      {
        result = unixEpoch.ToLocalTime().AddMilliseconds((double)jsonDate);
      }
      catch (Exception ex)
      {
        Logger.WriteLine("Scms.Web.Common.Functional:JsonToDate - {0}", ex.Message);
      }

      return result;
    }

    public static void SetComboData(Ext.Net.ComboBox cb, string valueField, string textData, string valueData)
    {
      SetComboData(cb, valueField, textData, valueData, false);
    }

    public static void SetComboData(Ext.Net.ComboBox cb, string valueField, string textData, string valueData, bool clearFilter)
    {
      Ext.Net.Store store = cb.GetStore();

      if (store != null)
      {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        int rndX = randRandom.Next(0, int.MaxValue);

        if (clearFilter)
        {
          sb.AppendFormat("{0}.clearFilter(true);", store.ClientID);
        }
        sb.AppendFormat("var idx{0} = {1}.findExact('{2}', '{3}'); ", rndX, store.ClientID, valueField, valueData);
        sb.AppendFormat("if(idx{0} == -1) {{ ", rndX);
        sb.AppendFormat("{0}.addItem('{1}','{2}');{3}.commitChanges(); ", cb.ClientID, textData, valueData, store.ClientID);
        sb.Append("} ");
        sb.AppendFormat("{0}.setValueAndFireSelect('{1}'); ", cb.ClientID, valueData);

        Ext.Net.X.AddScript(sb.ToString());

        sb.Remove(0, sb.Length);
      }
    }

    public static void SetSelectBoxData(Ext.Net.SelectBox sbox, string textData, string valueData)
    {
      SetSelectBoxData(sbox, textData, valueData, false);
    }

    public static void SetSelectBoxData(Ext.Net.SelectBox sbox, string textData, string valueData, bool clearFilter)
    {
      System.Text.StringBuilder sb = new System.Text.StringBuilder();

      int rndX = randRandom.Next(0, int.MaxValue);

      if (clearFilter)
      {
        sb.AppendFormat("{0}.getStore().clearFilter(true); ", sbox.ClientID);
      }
      sb.AppendFormat("var rec{0} = {1}.findRecord('value', '{2}'); ", rndX, sbox.ClientID, valueData);
      sb.AppendFormat("if(Ext.isEmpty(rec{0})) {{ ", rndX);
      sb.AppendFormat("{0}.addItem('{1}', '{2}'); ", sbox.ClientID, textData, valueData);
      sb.Append("} ");
      sb.AppendFormat("{0}.setValueAndFireSelect('{1}'); ", sbox.ClientID, valueData);

      Ext.Net.X.AddScript(sb.ToString());

      sb.Remove(0, sb.Length);
    }

    public static void SetNumberField(Ext.Net.NumberField nf, decimal value)
    {
      SetNumberField(nf, value, "0.000,00/i");
    }

    public static void SetNumberField(Ext.Net.NumberField nf, decimal value, string format)
    {
      if (string.IsNullOrEmpty(format))
      {
        Ext.Net.X.AddScript(string.Format("{0}.setRawValue(Ext.util.Format.number({1}));",
          nf.ClientID, value));
      }
      else
      {
        Ext.Net.X.AddScript(string.Format("{0}.setRawValue(Ext.util.Format.number({1}, '{2}'));",
          nf.ClientID, value, format));
      }
    }

    public static void SetDateField(Ext.Net.DateField df, string tmp)
    {
      SetDateField(df, tmp, "dd-MM-yyyy");
    }

    public static void SetDateField(Ext.Net.DateField df, string tmp, string format)
    {
      DateTime date = Functional.JsonDateToDate(tmp);
      
      df.Text = date.ToString(format);
    }
    
    public static void PopulateBulan(Ext.Net.SelectBox ddl)
    {
      PopulateBulan(ddl, -1);
    }

    public static void PopulateBulan(Ext.Net.SelectBox ddl, int selectedMonth)
    {
      DateTime date = new DateTime(DateTime.Now.Year, 1, 1);

      //List<string[,]> listData = new List<string[,]>();

      ddl.Items.Clear();

      CultureInfo culture = ActiveCulture;

      for (int n = 1; n < 13; n++)
      {
        ddl.Items.Add(new Ext.Net.ListItem(date.ToString("MMMM", culture), date.Month.ToString()));

        date = date.AddMonths(1);
      }

      if (ddl.Items.Count > 0)
      {
        selectedMonth -= 1;

        ddl.SelectedIndex = ((selectedMonth >= 0) && (selectedMonth <= 11) ? ddl.SelectedIndex = selectedMonth : -1);
      }
    }

    public static void PopulateTahun(Ext.Net.SelectBox ddl)
    {
      PopulateBulan(ddl, -1);
    }

    public static void PopulateTahun(Ext.Net.SelectBox ddl, int selectedYear, int preYear, int sufYear)
    {
      DateTime date = new DateTime(DateTime.Now.Year, 1, 1);

      ddl.Items.Clear();

      int nYear = 0;

      if (preYear > 0)
      {
        date = date.AddYears((-preYear));

        preYear = (-preYear);
      }
      else
      {
        preYear = 0;
      }

      if (sufYear > 0)
      {
        for (int n = preYear; n <= sufYear; n++)
        {
          nYear = date.Year;

          ddl.Items.Add(new Ext.Net.ListItem(nYear.ToString(), date.Year.ToString()));

          if (nYear.Equals(selectedYear))
          {
            ddl.SelectedIndex = ((ddl.Items.Count > 0) ? (ddl.Items.Count - 1) : -1);
          }

          date = date.AddYears(1);
        }
      }
      else
      {
        ddl.Items.Add(new Ext.Net.ListItem(DateTime.Today.Year.ToString(), DateTime.Today.Year.ToString()));
        ddl.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));

        ddl.SelectedIndex = 0;
      }
    }

    public static void GeneratorLoadedWindow(string windowName, string url, Ext.Net.LoadMode loadMode)
    {
      GeneratorLoadedWindow(windowName, url, Ext.Net.LoadMode.IFrame, false);
    }

    public static void GeneratorLoadedWindow(string windowName, string url, Ext.Net.LoadMode loadMode, bool isCache)
    {
      //{script:"ctl00_cphContent_Asuransi1_wndDown.load({url:\"../../Viewer.aspx?o=History_Asuransi_105839H.pdf&f=93611B87&p=Reports&c=pdf&dwnl=1\",nocache:true,scripts:true,mode:\"iframe\"});"}

      string reslt =  string.Format(@"{0}.load(
              {{
                url: ""{1}"",
                nocache: {2},
                scripts: true,
                showMask: true,
                mode: ""{3}""
              }});", windowName, url.Replace("\"", "\\\""), (!isCache).ToString().ToLower(), (loadMode == Ext.Net.LoadMode.IFrame ? "iframe" : "merge"));

      Ext.Net.X.AddScript(reslt);
    }
    
    public static string CrystalReportDateString(DateTime date)
    {
      string result = null;

      try
      {
        //result = string.Format("CDate('{0:yyyy-MM-dd}')", date);
        //result = string.Format("CDate({0:yyyy}, {0:MM}, {0:dd})", date);
        result = string.Concat("'", date.ToString("yyyy-MM-dd"), "'");
      }
      catch (Exception ex)
      {
        Logger.WriteLine("Scms.Web.Common.Functional:CrystalReportDateString - {0}", ex.Message);
      }

      return result;
    }

    public static string CrystalReportDateCDate(DateTime date)
    {
      string result = null;

      try
      {
        //result = string.Format("CDate('{0:yyyy-MM-dd}')", date);
        result = string.Format("CDate({0:yyyy}, {0:MM}, {0:dd})", date);
        //result = string.Concat("'", date.ToString("yyyy-MM-dd"), "'");
      }
      catch (Exception ex)
      {
        Logger.WriteLine("Scms.Web.Common.Functional:CrystalReportDateString - {0}", ex.Message);
      }

      return result;
    }

    public static string UriDownloadGenerator(Scms.Web.Core.PageHandler page, string reportPhysic, string reportName, string fileType)
    {
      string sPath = page.Server.MapPath("~/App_Data/Reports");
      string reportFilePath = System.IO.Path.Combine(sPath, reportPhysic);

      string resl = string.Empty;
      string userEntry = null;      
      reportName = (reportName ?? string.Empty).Trim();

      if (System.IO.File.Exists(reportFilePath))
      {
        //Scms.Web.Core.PageHandler

        userEntry = (string.IsNullOrEmpty(page.Nip) ? "ANONYM" : page.Nip);

        reportName = string.Format("{0}_{1}.{2}", userEntry, reportName.Replace(' ', '_'), fileType);

        resl = page.ResolveClientUrl("~/Viewer.aspx");
        resl = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
          resl, reportName, reportPhysic, "Reports", fileType);
      }

      return resl;
    }

    public static UserControl LoadDynamicControl(Scms.Web.Core.PageHandler page, string controlName, string controlId, Ext.Net.Panel panel)
    {
      UserControl uc = null;

      int rndX = randRandom.Next(0, int.MaxValue);

      try
      {
        uc = (UserControl)page.LoadControl(controlName);
        uc.ID = (controlId ?? string.Concat("LDC_", rndX));

        panel.ContentControls.Add(uc);

        Ext.Net.X.Js.Call("destroyCtrlFromCache",
          new Ext.Net.JRawValue(panel.ClientID));

        Ext.Net.X.Js.Call("putCtrlToCache", new Ext.Net.JRawValue(panel.ClientID));
        panel.UpdateContent();
      }
      catch (Exception ex)
      {
        Logger.WriteLine("Scms.Web.Common.Functional:LoadDynamicControl - {0}", ex.Message);

        if (uc != null)
        {
          uc.Dispose();
        }

        uc = null;
      }

      return uc;
    }

    public static bool CanCreateGenerateReport(Scms.Web.Core.PageHandler page)
    {
      if (page.IsAllowAdd)
      {
        return true;
      }

      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk membuat laporan.");

      return false;
    }

    public static bool CanPrintGenerateReport(Scms.Web.Core.PageHandler page)
    {
      if (page.IsAllowPrinting)
      {
        return false;
      }

      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak laporan.");

      return true;
    }

    public static string GetCheckedRadioData(Ext.Net.RadioGroup rg)
    {
      return (rg.CheckedItems.Count < 1 ? string.Empty : rg.CheckedItems[0].InputValue); ;
    }

    #region Cookies

    public static HttpCookie WriteCookies(Page page, string nipUser, string passWord, UserInformation userInfo)
    {
      System.Text.Encoding utf8 = System.Text.Encoding.UTF8;

      HttpCookie cook = new HttpCookie(Constant.COOKIES_NAME);

      //cook.Domain = Constant.DOMAIN_NAME;
      cook.Expires = DateTime.Now.AddDays(Constant.COOKIES_EXPIRED_DAYS);
      //cook.Path = Constant.COOKIES_PATH;
      cook.Secure = false;
      //cook.Path = "/";
      cook.Domain = page.Request.Url.DnsSafeHost;

      string userIP = page.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
      userIP = (string.IsNullOrEmpty(userIP) ? (page.Request.ServerVariables["REMOTE_ADDR"] ?? page.Request.UserHostAddress) : userIP);

      string cryptPwd = GlobalCrypto.ToBase64(utf8.GetBytes(passWord)),
        packedData = null,
        signBrowser = string.Concat(page.Request.UserAgent,
          page.Request.UserHostAddress, page.Request.UserHostName,
          page.Request.Browser.Id, page.Request.Browser.Browser, page.Request.Browser.Platform,
          page.Request.Browser.Type, page.Request.Browser.Version, userIP);

      packedData = PackDataAndCrypt(nipUser, passWord, signBrowser, userInfo);

      cook.Values.Add("NIP", nipUser);
      cook.Values.Add("XXX", cryptPwd);
      cook.Values.Add("DATA", packedData);
      cook.Values.Add("SIGN", Constant.SIGN_ID);
      cook.Values.Add("CPR", Constant.SIGN_COPYRIGHT);
      
      SaveCookies(page, cook);

      GC.Collect();

      return cook;
    }

    public static UserInformation ReadCookies(Page page)
    {
      HttpCookie cook = LoadCookies(page);

      if (cook == null)
      {
        return null;
      }
      else if(DateTime.Now.CompareTo(cook.Expires) > 0)
      {
        DeleteCookies(page);

        return null;
      }

      UserInformation userInfo = null;

      System.Text.Encoding utf8 = System.Text.Encoding.UTF8;

      string userIP = page.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
      userIP = (string.IsNullOrEmpty(userIP) ? (page.Request.ServerVariables["REMOTE_ADDR"] ?? page.Request.UserHostAddress) : userIP);

      string nipUser = (cook.Values["NIP"] ?? string.Empty),
        cryptPwd = (cook.Values["XXX"] ?? string.Empty),
        packedData = (cook.Values["DATA"] ?? string.Empty),
        signBrowser = string.Concat(page.Request.UserAgent,
          page.Request.UserHostAddress, page.Request.UserHostName,
          page.Request.Browser.Id, page.Request.Browser.Browser, page.Request.Browser.Platform,
          page.Request.Browser.Type, page.Request.Browser.Version, userIP),
        copyRight = (cook.Values["CPR"] ?? string.Empty),
        originPwd = null;

      try
      {
        originPwd = utf8.GetString(GlobalCrypto.FromBase64(cryptPwd));
      }
      catch 
      {
        return null;
      }

      userInfo = UnpackDataAndDecrypt(nipUser, originPwd, signBrowser, packedData);

      GC.Collect();

      return userInfo;
    }

    public static void DeleteCookies(Page page)
    {
      string pathFile = null,
        ipUser = null;

      System.IO.FileInfo fi = null;

      try
      {
        pathFile = page.Server.MapPath(Constant.PATH_LOCATION_USER_COOKIES);

        ipUser = page.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        ipUser = (string.IsNullOrEmpty(ipUser) ? (page.Request.ServerVariables["REMOTE_ADDR"] ?? page.Request.UserHostAddress) : ipUser);

        pathFile = System.IO.Path.Combine(pathFile, string.Concat(ipUser, ".txt"));

        fi = new System.IO.FileInfo(pathFile);

        if (fi.Exists)
        {
          fi.Delete();
        }
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine(
          string.Concat("Scms.Web.Common.Functional:DeleteCookies - ", ex.Message));
      }
    }

    private static bool SaveCookies(Page page, HttpCookie cook)
    {
      bool bOk = false;

      string pathFile = null,
        ipUser = null,
        rawData = null;

      System.IO.FileInfo fi = null;
      System.IO.StreamWriter sw = null;
      
      ScmsHttpCookie scook = FromHttpCookies(cook);

      try
      {
        pathFile = page.Server.MapPath(Constant.PATH_LOCATION_USER_COOKIES);

        ipUser = page.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        ipUser = (string.IsNullOrEmpty(ipUser) ? (page.Request.ServerVariables["REMOTE_ADDR"] ?? page.Request.UserHostAddress) : ipUser);

        pathFile = System.IO.Path.Combine(pathFile, string.Concat(ipUser, ".txt"));

        fi = new System.IO.FileInfo(pathFile);

        if (fi.Exists)
        {
          fi.Delete();
        }

        rawData = ScmsSoaLibraryInterface.Components.StructureBase<ScmsHttpCookie>.DeserializeBinary(scook);
        
        sw = fi.CreateText();

        sw.Write(rawData);

        bOk = true;
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine(
          string.Concat("Scms.Web.Common.Functional:SaveCookies - ", ex.Message));
      }
      finally
      {
        if (sw != null)
        {
          sw.Close();
          sw.Dispose();
        }
      }

      return bOk;
    }

    private static HttpCookie LoadCookies(Page page)
    {
      HttpCookie cook = null;

      string pathFile = null,
         ipUser = null,
         rawData = null;

      System.IO.FileInfo fi = null;
      System.IO.StreamReader sr = null;
      ScmsHttpCookie scook = null;

      try
      {
        pathFile = page.Server.MapPath(Constant.PATH_LOCATION_USER_COOKIES);

        ipUser = page.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        ipUser = (string.IsNullOrEmpty(ipUser) ? (page.Request.ServerVariables["REMOTE_ADDR"] ?? page.Request.UserHostAddress) : ipUser);

        pathFile = System.IO.Path.Combine(pathFile, string.Concat(ipUser, ".txt"));

        fi = new System.IO.FileInfo(pathFile);

        if (fi.Exists)
        {
          sr = fi.OpenText();

          rawData = sr.ReadToEnd();

          scook = ScmsSoaLibraryInterface.Components.StructureBase<ScmsHttpCookie>.SerializeBinary(rawData);
          if (scook != null)
          {
            cook = ToHttpCookies(scook);
          }
        }
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine(
          string.Concat("Scms.Web.Common.Functional:LoadCookies - ", ex.Message));
      }
      finally
      {
        if (sr != null)
        {
          sr.Close();
          sr.Dispose();
        }
      }
      
      return cook;
    }

    #region Converter

    private static ScmsHttpCookie FromHttpCookies(HttpCookie cook)
    {
      ScmsHttpCookie scook = new ScmsHttpCookie();

      string sKey = null,
        sValue = null;

      List<ScmsHttpCookieValue> list = null;

      if (cook != null)
      {
        scook.Domain = cook.Domain;
        scook.Expires = cook.Expires;
        scook.HttpOnly = cook.HttpOnly;
        scook.Path = cook.Path;
        scook.Secure = cook.Secure;
        //scook.Value = cook.Value;
        scook.Name = cook.Name;        

        if ((cook.Values != null) && (cook.Values.Count > 0))
        {
          list = new List<ScmsHttpCookieValue>();

          for (int nLoop = 0, nLen = cook.Values.AllKeys.Length; nLoop < nLen; nLoop++)
          {
            sKey = cook.Values.AllKeys[nLoop];
            if (!string.IsNullOrEmpty(sKey))
            {
              sValue = cook.Values[sKey];

              list.Add(new ScmsHttpCookieValue()
              {
                Name = sKey,
                Value = sValue
              });
            }
          }

          scook.Values = list.ToArray();

          list.Clear();
        }
      }

      return scook;
    }

    private static HttpCookie ToHttpCookies(ScmsHttpCookie scook)
    {
      if (scook == null)
      {
        return null;
      }

      HttpCookie cook = new HttpCookie(Constant.COOKIES_NAME);
      ScmsHttpCookieValue scookv = null;

      if (scook != null)
      {
        cook.Domain = scook.Domain;
        cook.Expires = scook.Expires;
        cook.HttpOnly = scook.HttpOnly;
        cook.Path = scook.Path;
        cook.Secure = scook.Secure;
        //cook.Value = scook.Value;
        cook.Name = scook.Name;

        if ((scook.Values != null) && (scook.Values.Length > 0))
        {
          for (int nLoop = 0, nLen = scook.Values.Length; nLoop < nLen; nLoop++)
          {
            //cook.Values.Add(kvp.Key, kvp.Value);
            scookv = scook.Values[nLoop];

            cook.Values.Add(scookv.Name, scookv.Value);
          }
        }
      }

      return cook;
    }

    #endregion

    #region Pack data

    private static string PackDataAndCrypt(string nipUser, string password, string signBrowser, UserInformation userInfo)
    {
      string serialize = null,
        packed = null,
        strongKeys = null,
        crypted = null;

      System.IO.MemoryStream msData = null,
        msComp = null;
      System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
      byte[] byts = null;
      ScmsSoaLibraryInterface.Components.Package pack = null;
      ScmsSoaLibraryInterface.Components.StructureBase<UserInformation> strt = null;
      int lenData = 0;
      CryptorRijndael rc5 = null;

      try
      {
        strt = new ScmsSoaLibraryInterface.Components.StructureBase<UserInformation>();
        serialize = strt.DeserializeIt(userInfo);

        if (!string.IsNullOrEmpty(serialize))
        {
          msData = new System.IO.MemoryStream();

          byts = utf8.GetBytes(serialize);

          msData.Write(byts, 0, byts.Length);

          pack = new ScmsSoaLibraryInterface.Components.Package();

          msComp = pack.CompresDeflate(msData);

          if (msComp != null)
          {
            Array.Clear(byts, 0, byts.Length);

            lenData = (int)msComp.Length;

            byts = new byte[lenData];

            msComp.Read(byts, 0, lenData);

            packed = GlobalCrypto.ToBase64(byts);

            strongKeys = GlobalCrypto.Crypt1WayMD5String(string.Concat(Constant.SIGN_ID, " - ", nipUser, password));

            rc5 = new CryptorRijndael(signBrowser, strongKeys);

            crypted = rc5.Crypt(packed);

            if (rc5.IsError)
            {
              crypted = null;
            }
          }
        }
      }
      catch (Exception ex)
      {
        crypted = null;

        System.Diagnostics.Debug.WriteLine(
          string.Concat("Scms.Web.Common.Functional:PackData - ", ex.Message));
      }
      finally
      {
        if (msData != null)
        {
          msData.Close();
          msData.Dispose();
        }

        if (msComp != null)
        {
          msComp.Close();
          msComp.Dispose();
        }
      }

      return crypted;
    }

    private static UserInformation UnpackDataAndDecrypt(string nipUser, string password, string signBrowser, string packedData)
    {
      string serialize = null,
        packed = null,
        strongKeys = null;

      UserInformation userInfo = null;

      System.IO.MemoryStream msData = null,
        msComp = null;
      System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
      byte[] byts = null,
        bytsDecrypt = null;
      ScmsSoaLibraryInterface.Components.Package pack = null;
      ScmsSoaLibraryInterface.Components.StructureBase<UserInformation> strt = null;
      int lenData = 0;
      CryptorRijndael rc5 = null;

      try
      {
        //byts = GlobalCrypto.FromBase64(packedData);

        strongKeys = GlobalCrypto.Crypt1WayMD5String(string.Concat(Constant.SIGN_ID, " - ", nipUser, password));

        rc5 = new CryptorRijndael(signBrowser, strongKeys);

        //bytsDecrypt = rc5.DeCryptArray(byts);
        packed = rc5.DeCrypt(packedData);

        //Array.Clear(byts, 0, byts.Length);

        bytsDecrypt = (string.IsNullOrEmpty(packed) ? null : GlobalCrypto.FromBase64(packed));

        if ((!rc5.IsError) && (bytsDecrypt != null))
        {
          pack = new ScmsSoaLibraryInterface.Components.Package();

          msComp = new System.IO.MemoryStream();

          msComp.Write(bytsDecrypt, 0, bytsDecrypt.Length);

          msData = pack.DecompresDeflate(msComp);

          Array.Clear(bytsDecrypt, 0, bytsDecrypt.Length);

          if (msData != null)
          {
            lenData = (int)msData.Length;

            byts = new byte[lenData];

            msData.Read(byts, 0, lenData);

            serialize = utf8.GetString(byts);

            Array.Clear(byts, 0, byts.Length);

            if (!string.IsNullOrEmpty(serialize))
            {
              strt = new ScmsSoaLibraryInterface.Components.StructureBase<UserInformation>();

              userInfo = strt.SerializeIt(serialize);
            }
          }
        }
      }
      catch (Exception ex)
      {
        userInfo = null;

        System.Diagnostics.Debug.WriteLine(
          string.Concat("Scms.Web.Common.Functional:PackData - ", ex.Message));
      }
      finally
      {
        if (msData != null)
        {
          msData.Close();
          msData.Dispose();
        }

        if (msComp != null)
        {
          msComp.Close();
          msComp.Dispose();
        }
      }

      return userInfo;
    }

    #endregion

    #endregion

    #region Message

    public static void ShowMsgError(string msg)
    {
      //ShowMsgError(null, msg);
      Ext.Net.X.AddScript(" ShowError(\"{0}\"); ",
        (string.IsNullOrEmpty(msg) ? string.Empty :
               msg.Replace("\\", "\\\\").Replace("\r\n", "\\n").Replace("\r", "\\n").Replace("\n", "\\n").Replace("\'", "\\'").Replace("\"", "\\\"")));
    }

    public static void ShowMsgError(string title, string msg)
    {
      Ext.Net.X.Msg.Show(new Ext.Net.MessageBoxConfig()
      {
        Title = (string.IsNullOrEmpty(title) ? "Error" : title.Trim()),
        Icon = Ext.Net.MessageBox.Icon.ERROR,
        Modal = true,
        Buttons = Ext.Net.MessageBox.Button.OK,
        Message = (string.IsNullOrEmpty(msg) ? string.Empty :
               msg.Replace("\\", "\\\\").Replace("\r\n", "\\n").Replace("\r", "\\n").Replace("\n", "\\n").Replace("\'", "\\'").Replace("\"", "\\\""))
      });
    }

    public static void ShowMsgWarning(string msg)
    {
      //ShowMsgWarning(null, msg);
      Ext.Net.X.AddScript(" ShowWarning(\"{0}\"); ",
      (string.IsNullOrEmpty(msg) ? string.Empty :
             msg.Replace("\\", "\\\\").Replace("\r\n", "\\n").Replace("\r", "\\n").Replace("\n", "\\n").Replace("\'", "\\'").Replace("\"", "\\\"")));
    }

    public static void ShowMsgWarning(string title, string msg)
    {
      Ext.Net.X.Msg.Show(new Ext.Net.MessageBoxConfig()
      {
        Title = (string.IsNullOrEmpty(title) ? "Warning" : title.Trim()),
        Icon = Ext.Net.MessageBox.Icon.WARNING,
        Modal = true,
        Buttons = Ext.Net.MessageBox.Button.OK,
        Message = (string.IsNullOrEmpty(msg) ? string.Empty :
               msg.Replace("\\", "\\\\").Replace("\r\n", "\\n").Replace("\r", "\\n").Replace("\n", "\\n").Replace("\'", "\\'").Replace("\"", "\\\""))
      });
    }

    public static void ShowMsgInformation(string msg)
    {
      //ShowMsgInformation(null, msg);
      Ext.Net.X.AddScript(" ShowInformasi(\"{0}\"); ",
        (string.IsNullOrEmpty(msg) ? string.Empty :
               msg.Replace("\\", "\\\\").Replace("\r\n", "\\n").Replace("\r", "\\n").Replace("\n", "\\n").Replace("\'", "\\'").Replace("\"", "\\\"")));
    }

    public static void ShowMsgInformation(string title, string msg)
    {
      Ext.Net.X.Msg.Show(new Ext.Net.MessageBoxConfig()
      {
        Title = (string.IsNullOrEmpty(title) ? "Informasi" : title.Trim()),
        Icon = Ext.Net.MessageBox.Icon.INFO,
        Modal = true,
        Buttons = Ext.Net.MessageBox.Button.OK,
        Message = (string.IsNullOrEmpty(msg) ? string.Empty :
               msg.Replace("\\", "\\\\").Replace("\r\n", "\\n").Replace("\r", "\\n").Replace("\n", "\\n").Replace("\'", "\\'").Replace("\"", "\\\""))
      });
    }

    public static void ShowNotInformation(string title, string msg)
    {
      Ext.Net.Notification.Show(new Ext.Net.NotificationConfig()
      {
        Title = (string.IsNullOrEmpty(title) ? "Informasi" : title.Trim()),
        Icon = Ext.Net.Icon.Information,
        Modal = true,
        Html = (string.IsNullOrEmpty(msg) ? string.Empty :
               msg),
      });
      
    }

    public static void ShowMsg(string msg)
    {
      //ShowMsg(null, msg);
      Ext.Net.X.AddScript(" ShowMessage('{0}'); ",
        (string.IsNullOrEmpty(msg) ? string.Empty :
               msg.Replace("\\n", "\\\n").Replace("\'", "\\\'").Replace("\"", "\\\"")));
    }

    public static void ShowMsg(string title, string msg)
    {
      Ext.Net.X.Msg.Show(new Ext.Net.MessageBoxConfig()
      {
        Title = (string.IsNullOrEmpty(title) ? "Pesan" : title.Trim()),
        Icon = Ext.Net.MessageBox.Icon.NONE,
        Modal = true,
        Buttons = Ext.Net.MessageBox.Button.OK,
        Message = (string.IsNullOrEmpty(msg) ? string.Empty :
               msg.Replace("\\", "\\\\").Replace("\r\n", "\\n").Replace("\r", "\\n").Replace("\n", "\\n").Replace("\'", "\\'").Replace("\"", "\\\""))
      });
    }

    public static void ShowAlert(string msg)
    {
        Ext.Net.X.AddScript("alert( \"{0}\"); ",
        (string.IsNullOrEmpty(msg) ? string.Empty :
               msg.Replace("\\", "\\\\").Replace("\r\n", "\\n").Replace("\r", "\\n").Replace("\n", "\\n").Replace("\'", "\\'").Replace("\"", "\\\"")));
    }

    #endregion

    #region Constant

    public const string NAME_SOA_SCMS_CALLBACK = "soaScmsCallback";

    #endregion

    #region Crypto

    public static string DecryptHashRjdnl(string key, string val)
    {
      CryptorDES des = new CryptorDES(key, string.Empty);

      byte[] byt = GlobalCrypto.FromBase64(val);
      string dat = des.DeCrypt(byt);
      Array.Clear(byt, 0, byt.Length);
      byt = null;

      return dat;
    }

    public static string CryptHashRjdnl(string key, string val)
    {
      CryptorDES des = new CryptorDES(key, string.Empty);

      byte[] byt = des.CryptArray(val);
      string dat = GlobalCrypto.ToBase64(byt);
      Array.Clear(byt, 0, byt.Length);
      byt = null;

      return dat;
    }

    #endregion
  }
}