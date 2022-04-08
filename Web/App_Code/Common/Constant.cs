using System;

namespace Scms.Web.Common
{
  /// <summary>
  /// Summary description for Constant
  /// </summary>
  public class Constant
  {
    public const string SESSION_LOGIN_INFORMATION = "SLI";

    public const string PATH_LOCATION_USER_IMAGE = "~/App_Data/Users";

    public const string PATH_LOCATION_USER_COOKIES = "~/App_Data/Cookies";

    public const string IMAGE_WINDOW_NAME = "WinImageShow";

    public const int DEFAULT_DIRECTEVENT_TIMEOUT = 990000000;

    public const string DOMAIN_NAME = "root://localhost";

    public const string COOKIES_NAME = "scms_next_g";

    public const string COOKIES_PATH = "/scms";

    public const int COOKIES_EXPIRED_DAYS = 5;

    public const string SIGN_ID = "SCMS";

    public const string SIGN_COPYRIGHT = "SCMS TEAM © 2012";
  }

  #region Class

  [Serializable]
  public class UserInformation
  {
    public string NIP { get; set; }
    public bool IsCabang { get; set; }
    public string LokasiDeskripsi { get; set; }
    public string Lokasi { get; set; }
    public bool IsAktif { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool HasAdminGroup { get; set; }
    public string ImagePicture { get; set; }
    public string ImageWallpaper { get; set; }
    public bool IsPrinsipal { get; set; }
    public string Prinsipal { get; set; }
    public string PrinsipalDeskripsi { get; set; }
    public string DivisiPrinsipal { get; set; }
    public string DivisiPrinsipalDeskripsi { get; set; }
    public Scms.Web.Core.RightBuilder.GroupAccess[] GroupAccess { get; set; }
  }

  #endregion
}
