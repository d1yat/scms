<%@ Page Language="C#" EnableViewState="false" %>

<%@ Import Namespace="System.IO" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">  
  private string getQueryString(string name)
  {
    foreach (string s in Request.QueryString.Keys)
    {
      //if (string.Compare(name, s, StringComparison.OrdinalIgnoreCase) == 0)
      if (s.Equals(name, StringComparison.OrdinalIgnoreCase))
      {
        return Request[name];
      }
    }

    return string.Empty;
  }

  private void RenderImage(string mode, string file)
  {
    if (string.IsNullOrEmpty(mode) || string.IsNullOrEmpty(file))
    {
      this.Response.End();

      return;
    }

    string fullTemp = null;

    if (mode.Equals("User", StringComparison.OrdinalIgnoreCase))
    {
      fullTemp = this.Server.MapPath(Scms.Web.Common.Constant.PATH_LOCATION_USER_IMAGE);
    }
    else
    {
      fullTemp = string.Empty;
    }

    FileInfo fi = new FileInfo(Path.Combine(fullTemp, file));

    if (!fi.Exists)
    {
      this.Response.End();
      
      return;
    }
    
    HttpResponse resp = this.Response;

    resp.Cache.SetAllowResponseInBrowserHistory(false);
    resp.Cache.SetCacheability(HttpCacheability.NoCache);
    resp.Cache.SetExpires(DateTime.Now.AddHours(-3));
    resp.Cache.SetNoServerCaching();
    resp.BufferOutput = false;
    resp.Buffer = false;
    resp.Cache.SetExpires(DateTime.Now.AddDays(-1));
    resp.Cache.SetNoStore();

    switch (fi.Extension.ToLower())
    {
      case ".gif":
        resp.ContentType = "image/gif";
        break;
      case ".jpg":
        resp.ContentType = "image/jpeg";
        break;
      case ".png":
        resp.ContentType = "image/png";
        break;
      case ".bmp":
        resp.ContentType = "image/bmp";
        break;
      default:
        resp.ContentType = "octet-stream";
        break;
    }

    resp.WriteFile(fi.FullName, true);
  }
  
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!Page.IsPostBack)
    {
      string file = null,
        mode = null;

      mode = getQueryString("m");
      file = getQueryString("f");

      RenderImage(mode, file);
    }
  }
  
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title></title>
</head>
<body>
  <form id="form1" runat="server">
  <div>
  </div>
  </form>
</body>
</html>
