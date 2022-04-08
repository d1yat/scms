#undef DEBUG_CODE

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using Scms.Web.Core;
using ScmsSoaLibraryInterface.Core.Crypto;

public partial class _Default : System.Web.UI.Page
{
  #region Private

  private bool VerifyPassword(string userName, string passWord, bool isRemember)
  {
    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "c_nip = @0", userName, "System.String"},
        new string[] { "decrypted", bool.TrueString, "System.Boolean"}
      };

    //passWord = GlobalCrypto.Crypt1WayMD5String(passWord);

    #region Parser Data

    string res = soa.GlobalQueryService(0, 0, true, string.Empty, string.Empty, "101101", paramX);
    if (string.IsNullOrEmpty(res))
    {
      return false;
    }

    bool result = false;

    Dictionary<string, object> dicUser = null;
    List<Dictionary<string, string>> listUserInfo = null;
    //Dictionary<string, string> dicUserInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;
    UserInformation userInfo = null;
    bool isLoginSuccess = false;

    try
    {
      dicUser = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicUser.ContainsKey("records") && (dicUser.ContainsKey("totalRows") && (((long)dicUser["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicUser["records"]);

        listUserInfo = JSON.Deserialize<List<Dictionary<string, string>>>(jarr.First.ToString());

        userInfo = PopulateUserAccess(passWord, listUserInfo, out isLoginSuccess);

        if ((userInfo != null) && isLoginSuccess)
        {
          this.Session[Constant.SESSION_LOGIN_INFORMATION] = userInfo;

          if (isRemember)
          {
            //this.WriteCookies(userName, passWord, userInfo);

            Functional.WriteCookies(this, userName, passWord, userInfo);
          }
          
          result = true;
        }

        #region Old Coded

        //bool isFilled = false;
        //List<string> listAkses = new List<string>();
        //string tmp = null;

        //for (int nLoop = 0, nLen = listUserInfo.Count; nLoop < nLen; nLoop++)
        //{
        //  dicUserInfo = listUserInfo[nLoop];
        //  if (!isFilled)
        //  {
        //    userInfo = new UserInformation()
        //    {
        //      IsAktif = dicUserInfo.GetValueParser<bool>("l_aktif"),
        //      Lokasi = dicUserInfo.GetValueParser<string>("c_gdg", string.Empty),
        //      LokasiDeskripsi = dicUserInfo.GetValueParser<string>("v_gdgdesc", string.Empty),
        //      NIP = dicUserInfo.GetValueParser<string>("c_nip", string.Empty),
        //      Password = dicUserInfo.GetValueParser<string>("v_password", string.Empty),
        //      UserName = dicUserInfo.GetValueParser<string>("v_username", string.Empty),
        //      ImagePicture = dicUserInfo.GetValueParser<string>("v_imgpic", string.Empty),
        //      ImageWallpaper = dicUserInfo.GetValueParser<string>("v_imgwall", string.Empty)
        //    };

        //    isFilled = true;
        //  }

        //  tmp = dicUserInfo.GetValueParser<string>("c_group");
        //  if (!string.IsNullOrEmpty(tmp))
        //  {
        //    if (tmp.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        //    {
        //      userInfo.HasAdminGroup = true;
        //    }

        //    tmp = dicUserInfo.GetValueParser<string>("v_akses");
        //    if (!string.IsNullOrEmpty(tmp))
        //    {
        //      listAkses.Add(tmp);
        //    }

        //    dicUserInfo.Clear();
        //  }

        //  if (dicUserInfo != null)
        //  {
        //    dicUserInfo.Clear();
        //  }
        //}

        //if (isFilled && (!string.IsNullOrEmpty(userInfo.Password)))
        //{
        //  result = userInfo.Password.Equals(passWord);
        //}
        //else
        //{
        //  result = false;
        //}

        //if (result)
        //{
        //  List<RightBuilder.GroupAccess> listGA = new List<RightBuilder.GroupAccess>();

        //  RightBuilder.GroupAccess ga = null;

        //  for (int nLoop = 0, nLen = listAkses.Count; nLoop < nLen; nLoop++)
        //  {
        //    tmp = listAkses[nLoop];

        //    ga = RightBuilder.GroupAccess.Serialize(tmp);

        //    if (ga != null)
        //    {
        //      listGA.Add(ga);
        //    }
        //  }

        //  userInfo.GroupAccess = listGA.ToArray();

        //  listGA.Clear();

        //  listAkses.Clear();

        //  this.Session[Constant.SESSION_LOGIN_INFORMATION] = userInfo;
        //}

        #endregion

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("_Default:PopulateDetail - ", ex.Message));
    }
    finally
    {
      if (jarr != null)
      {
        jarr.Clear();
      }
      if (listUserInfo != null)
      {
        listUserInfo.Clear();
      }
      if (dicUser != null)
      {
        dicUser.Clear();
      }
    }

    #endregion

    GC.Collect();

    return result;
  }

  private UserInformation PopulateUserAccess(string passwordOrigin, List<Dictionary<string, string>> listUserInfo, out bool isLoginSuccess)
  {
    isLoginSuccess = false;

    Dictionary<string, string> dicUserInfo = null;
    bool isFilled = false,
      result = false;
    UserInformation userInfo = null;
    string tmp = null;
    List<string> listAkses = new List<string>();
    List<RightBuilder.GroupAccess> listGA = null;
    RightBuilder.GroupAccess ga = null;

    string passWord = GlobalCrypto.Crypt1WayMD5String(passwordOrigin);

    try
    {
      for (int nLoop = 0, nLen = listUserInfo.Count; nLoop < nLen; nLoop++)
      {
        dicUserInfo = listUserInfo[nLoop];
        if (!isFilled)
        {
          userInfo = new UserInformation()
          {
            IsAktif = dicUserInfo.GetValueParser<bool>("l_aktif"),
            Lokasi = dicUserInfo.GetValueParser<string>("c_gdg", string.Empty),
            LokasiDeskripsi = dicUserInfo.GetValueParser<string>("v_gdgdesc", string.Empty),
            NIP = dicUserInfo.GetValueParser<string>("c_nip", string.Empty),
            Password = dicUserInfo.GetValueParser<string>("v_password", string.Empty),
            UserName = dicUserInfo.GetValueParser<string>("v_username", string.Empty),
            ImagePicture = dicUserInfo.GetValueParser<string>("v_imgpic", string.Empty),
            ImageWallpaper = dicUserInfo.GetValueParser<string>("v_imgwall", string.Empty),
            IsCabang = dicUserInfo.GetValueParser<bool>("IsUserCabang"),
            IsPrinsipal = dicUserInfo.GetValueParser<bool>("IsUserPrinsipal"),
            Prinsipal = dicUserInfo.GetValueParser<string>("c_nosup", string.Empty),
            PrinsipalDeskripsi = dicUserInfo.GetValueParser<string>("v_supdesc", string.Empty),
            DivisiPrinsipal = dicUserInfo.GetValueParser<string>("c_kddivpri", string.Empty),
            DivisiPrinsipalDeskripsi = dicUserInfo.GetValueParser<string>("v_divpridesc", string.Empty),
          };

          userInfo.IsPrinsipal =
            (userInfo.IsPrinsipal ? userInfo.IsPrinsipal
            : ((string.IsNullOrEmpty(userInfo.Prinsipal) && string.IsNullOrEmpty(userInfo.DivisiPrinsipal)) ? false : true));

          userInfo.IsCabang =
                      (userInfo.IsCabang ? userInfo.IsCabang
                      : ((userInfo.Lokasi.Trim().Length == 1) ? false : true));

          isFilled = true;
        }

        tmp = dicUserInfo.GetValueParser<string>("c_group");
        if (!string.IsNullOrEmpty(tmp))
        {
          if (tmp.Equals("Admin", StringComparison.OrdinalIgnoreCase))
          {
            userInfo.HasAdminGroup = true;
          }

          tmp = dicUserInfo.GetValueParser<string>("v_akses");
          if (!string.IsNullOrEmpty(tmp))
          {
            listAkses.Add(tmp);
          }
        }

        dicUserInfo.Clear();
      }

      if (isFilled && (!string.IsNullOrEmpty(userInfo.Password)))
      {
        result = userInfo.Password.Equals(passWord);
      }
      else
      {
        result = false;
      }

      if (result)
      {
        isLoginSuccess = true;

        listGA = new List<RightBuilder.GroupAccess>();

        for (int nLoop = 0, nLen = listAkses.Count; nLoop < nLen; nLoop++)
        {
          tmp = listAkses[nLoop];

          ga = RightBuilder.GroupAccess.Serialize(tmp);

          if (ga != null)
          {
            listGA.Add(ga);
          }
        }

        userInfo.GroupAccess = listGA.ToArray();
      }
    }
    catch (Exception ex)
    {
      userInfo = null;

      System.Diagnostics.Debug.WriteLine(
        string.Concat("_Default:PopulateUserAccess - ", ex.Message));
    }

    if (listGA != null)
    {
      listGA.Clear();
    }

    listAkses.Clear();

    return userInfo;
  }

  private void ReadCookies()
  {
    UserInformation userInfo = Functional.ReadCookies(this);

    if (userInfo != null)
    {
      this.Session[Constant.SESSION_LOGIN_INFORMATION] = userInfo;

      X.Redirect("Main.aspx", "(Cookies) Mohon tunggu, sedang membuat struktur menu...");
    }
  }

  #endregion

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      this.ReadCookies();
    }
  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void btnLogin_Click(object sender, DirectEventArgs e)
  {
    string sNip = txUsername.Text.Trim();
    string sPass = hidPwd.Text.Trim();
    bool isRemember = chkRemember.Checked;

    bool isOk = VerifyPassword(sNip, sPass, isRemember);

    e.Success = isOk;

    if (isOk)
    {
        UserInformation userInfo = this.Session[Constant.SESSION_LOGIN_INFORMATION] as UserInformation;
        if (userInfo.IsAktif == false)
        {
            e.Success = false;
            e.ErrorMessage = "User tidak aktif.";
        }
        else
        {
            X.Redirect("Main.aspx", "Mohon tunggu, sedang membuat struktur menu..."); 
        }
      //if (userInfo != null)
      //{
      //  //string pageFile = this.Request.Url.Segments[this.Request.Url.Segments.Length - 1];
      //  //string query = this.Request.Url.Query;
        
      //  //Populate(userInfo, pageFile, null);

      //  //Populate(userInfo, this.Request);
      //}
      //this.Response.Redirect("Main.aspx");
    }
    else
    {
      e.ErrorMessage = "NIP atau kata kunci tidak sama.";
    }
  }

  private static string[] PagesAvoid =
    new string[]
      {
        "/web/Main.aspx",
        "/web/reporting/Default.aspx"
      };

#if DEBUG_CODE

  bool IsAdministrator;
  bool IsAllowAdd;
  bool IsAllowEdit;
  bool IsAllowDelete;
  bool IsAllowView;
  bool IsAllowPrinting;
  bool IsGroupAdmin;
  string Nip;
  string Username;
  string ActiveGudang;
  string ActiveGudangDescription;

  private void Populate(UserInformation userInfo, HttpRequest req)
  {
    if ((userInfo == null) || (req == null))
    {
      this.Response.Redirect("~/");

      return;
    }

    string pageFile = req.Path;

    if (Array.Exists<string>(PagesAvoid, delegate(string pageUrl)
    {
      if (pageUrl.Equals(pageFile, StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }

      return false;
    }))
    {
      return;
    }

    List<string> lstQuery = new List<string>();
    
    string tmp = null;

    for (int nLoop = 0, nLen = req.QueryString.Keys.Count; nLoop < nLen; nLoop++)
    {
      tmp = req.QueryString.Keys[0];
      if ((tmp != null) && (!tmp.Equals("_dc", StringComparison.OrdinalIgnoreCase)))
      {
        lstQuery.Add(string.Concat(tmp, "=", req.QueryString[tmp]));
      }
    }

    string joinQuery = string.Join("&", lstQuery.ToArray());

    RightBuilder.GroupAccess[] grpAccs = userInfo.GroupAccess;
    RightBuilder.GroupAccess ga = null;
    RightBuilder.PageRightAccess pga = null;

    this.Nip = userInfo.NIP;
    this.Username = userInfo.UserName;
    this.ActiveGudang = userInfo.Lokasi;
    this.ActiveGudangDescription = userInfo.LokasiDeskripsi;

    if (this.Nip.Equals("Administrator", StringComparison.OrdinalIgnoreCase))
    {
      this.IsAdministrator = true;
    }
    else
    {
      this.IsAdministrator = false;
    }

    this.IsGroupAdmin = userInfo.HasAdminGroup;

    if (grpAccs != null)
    {
      if (grpAccs.Length == 1)
      {
        #region Single Group Access

        ga = grpAccs[0];

        if (ga != null)
        {
          pga = ga.FindPage(pageFile, joinQuery);

          if (pga != null)
          {
            this.IsAllowAdd = pga.IsAdd;
            this.IsAllowDelete = pga.IsDelete;
            this.IsAllowEdit = pga.IsEdit;
            this.IsAllowPrinting = pga.IsPrint;
            this.IsAllowView = pga.IsView;
          }
        }

        #endregion
      }
      else if (grpAccs.Length > 1)
      {
        #region Multiple Group Access

        for (int nLoop = 0, nLen = grpAccs.Length; nLoop < nLen; nLoop++)
        {
          ga = grpAccs[nLoop];

          if (ga != null)
          {
            pga = ga.FindPage(pageFile, joinQuery);

            if (pga != null)
            {
              if (pga.IsAdd && (!this.IsAllowAdd))
              {
                this.IsAllowAdd = true;
              }
              if (pga.IsDelete && (!this.IsAllowDelete))
              {
                this.IsAllowDelete = true;
              }
              if (pga.IsEdit && (!this.IsAllowEdit))
              {
                this.IsAllowEdit = true;
              }
              if (pga.IsPrint && (!this.IsAllowPrinting))
              {
                this.IsAllowPrinting = true;
              }
              if (pga.IsView && (!this.IsAllowView))
              {
                this.IsAllowView = true;
              }
            }
          }
        }

        #endregion
      }
    }

    if (!this.IsAllowView)
    {
      //this.Response.StatusCode = 403; // Forbidden
      //this.Response.SubStatusCode = 8; // Site access denied
      //this.Response.Status = "403;8";

      this.Response.Redirect("~/epages/pages/403.htm", true);
    }
  }

#endif
}
