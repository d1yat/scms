#define DEBUG_CODE

using System;
using System.Collections.Generic;
using System.Web;
using Scms.Web.Common;

namespace Scms.Web.Core
{
  /// <summary>
  /// Summary description for PageHandler
  /// </summary>
  public class PageHandler : System.Web.UI.Page
  {
    private static string[] PagesAvoid = 
      new string[]
      {
        "/web/Main.aspx",
        "/web/reporting/Default.aspx",
        "/Web/management/UserPanel.aspx",
      };

    private static string[] PagesAvoidAdmin =
      new string[]
      {
        "/Web/management/Groups.aspx",
        "/Web/management/UserGroup.aspx",
        "/Web/management/Users.aspx"
      };

    //private const string REDIRECT_PARENT = "parentRedir";
    //private const string REDIRECT_URL_PARENT = "~/";

    #region Private

    private void Populate(UserInformation userInfo, HttpRequest req)
    {
      if ((userInfo == null) || (req == null))
      {
        this.Response.Redirect("~/", true);

        return;
      }

      string pageFile = req.Path;

      List<string> lstQuery = new List<string>();
      List<string> lstDataQuery = new List<string>();

      string tmp = null;

      for (int nLoop = 0, nLen = req.QueryString.Keys.Count; nLoop < nLen; nLoop++)
      {
        tmp = req.QueryString.Keys[nLoop];
        if ((tmp != null) && (!tmp.Equals("_dc", StringComparison.OrdinalIgnoreCase)))
        {
          if (!lstDataQuery.Contains(tmp))
          {
            lstDataQuery.Add(tmp);

            lstQuery.Add(string.Concat(tmp, "=", req.QueryString[tmp]));
          }
        }
      }

      string joinQuery = string.Join("&", lstQuery.ToArray());

      RightBuilder.GroupAccess[] grpAccs = userInfo.GroupAccess;
      RightBuilder.GroupAccess ga = null;
      RightBuilder.PageRightAccess pga = null;

      this.Nip = userInfo.NIP;
      this.Username = userInfo.UserName;
      this.ActiveGudang = (userInfo.Lokasi ?? "") ;
      this.ActiveGudangDescription = userInfo.LokasiDeskripsi;
      this.ImagePicName = userInfo.ImagePicture;
      this.ImageWallpaperName = userInfo.ImageWallpaper;

      this.IsCabang = userInfo.IsCabang;
      this.IsSupplier = userInfo.IsPrinsipal;
      this.Supplier = userInfo.Prinsipal;
      this.SupplierDescription = userInfo.PrinsipalDeskripsi;
      this.DivisiSupplier = userInfo.DivisiPrinsipal;
      this.DivisiSupplierDescription = userInfo.DivisiPrinsipalDeskripsi;

      if (this.IsCabang)
      {
        this.Cabang = this.ActiveGudang;
        this.CabangDescription = this.ActiveGudangDescription;
      }

      if (this.Nip.Equals("Administrator", StringComparison.OrdinalIgnoreCase))
      {
        this.IsAdministrator = true;
        this.IsGroupAdmin = true;
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

              //if (pga.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase))
              //{
              //  this.IsGroupAdmin = true;
              //}
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

                //if ((!this.IsGroupAdmin) && pga.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                //{
                //  this.IsGroupAdmin = true;
                //}
              }
            }
          }

          #endregion
        }
      }

      if (Array.Exists<string>(PagesAvoid, delegate(string pageUrl)
      {
        if (pageUrl.Equals(pageFile, StringComparison.OrdinalIgnoreCase))
        {
          return true;
        }

        return false;
      }) ||
        Array.Exists<string>(PagesAvoidAdmin, delegate(string pageUrl)
      {
        if (pageUrl.Equals(pageFile, StringComparison.OrdinalIgnoreCase) && (this.IsGroupAdmin || this.IsAdministrator))
        {
          return true;
        }

        return false;
      }))
      {
        this.IsAllowView = true;

        return;
      }

      if (!this.IsAllowView)
      {
        //this.Response.StatusCode = 403; // Forbidden
        //this.Response.SubStatusCode = 8; // Site access denied
        //this.Response.Status = "403;8";

        this.Response.Redirect("~/epages/pages/403.htm", true);
      }
    }

    #endregion

    public PageHandler()
    {
      //
      // TODO: Add constructor logic here
      //
    }

    #region Page Handler

    protected override void OnInit(System.EventArgs e)
    {
      //string curPageName = (this.Request.Url.Segments.Length > 0 ? this.Request.Url.Segments[this.Request.Url.Segments.Length - 1] : string.Empty);
      //string curQueryString = (string.IsNullOrEmpty(this.Request.Url.Query) ? string.Empty : this.Request.Url.Query);
      
      UserInformation userInfo = this.Session[Constant.SESSION_LOGIN_INFORMATION] as UserInformation;
      if (userInfo == null)
      {
#if !DEBUG_CODE
                
        this.Response.Redirect("~/epages/pages/Redirect.aspx", true);

        //string fpath =  this.Request.FilePath.ToLower();

        //if (fpath.Contains("default.aspx") ||
        //  fpath.Contains("main.aspx"))
        //{
        //  this.Response.Redirect(REDIRECT_URL_PARENT, true);
        //}
        //else
        //{

        //  fpath = this.ResolveClientUrl(REDIRECT_URL_PARENT);

        //  //Ext.Net.X.AddScript(string.Format("alert(window.top.location.href);", fpath));
        //  //Ext.Net.X.AddScript(string.Format("alert(window.parent.location.href);", fpath));
        //  //Ext.Net.X.AddScript(string.Format("alert(window.opener.location.href);", fpath));

        //  Ext.Net.X.AddScript(string.Format("window.top.location.replace('{0}');", fpath));

        //  //this.Response.Redirect(REDIRECT_URL_PARENT);
        //}

        //base.OnInit(e);

        //return;

#endif

      }
      else
      {
        //string pageFile = this.Request.Url.Segments[this.Request.Url.Segments.Length - 1];
        //string query = this.Request.Url.Query;

        //Populate(userInfo, pageFile, query);

        Populate(userInfo, this.Request);
      }



      base.OnInit(e);
    }

    protected override void OnPreRender(EventArgs e)
    {
      if (!this.IsPostBack)
      {
        Functional fn = new Functional();
        fn.RecursiveReplaceStoreUrl(this.Controls);
      }

      base.OnPreRender(e);
    }

    #endregion

    public bool IsAdministrator
    { get; private set; }

    public bool IsAllowAdd
    { get; private set; }

    public bool IsAllowEdit
    { get; private set; }

    public bool IsAllowDelete
    { get; private set; }

    public bool IsAllowView
    { get; private set; }

    public bool IsAllowPrinting
    { get; private set; }

    public bool IsGroupAdmin
    { get; private set; }

    public bool IsSpecialGroup
    { get; private set; }

    public bool IsCabang
    { get; private set; }

    public string Nip
    { get; private set; }

    public string Username
    { get; private set; }

    public string ActiveGudang
    { get; private set; }

    public string ActiveGudangDescription
    { get; private set; }
    
    public string Cabang
    { get; private set; }

    public string CabangDescription
    { get; private set; }

    public string ImagePicName
    { get; private set; }

    public string ImageWallpaperName
    { get; private set; }
    
    public bool IsSupplier
    { get; private set; }
    
    public string Supplier
    { get; private set; }

    public string SupplierDescription
    { get; private set; }

    public string DivisiSupplier
    { get; private set; }

    public string DivisiSupplierDescription
    { get; private set; }
  }
}