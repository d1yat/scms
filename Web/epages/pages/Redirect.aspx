<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
  private const string REDIRECT_PARENT = "parentRedir";
  private const string REDIRECT_URL_PARENT = "~/";
  
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!Page.IsPostBack)
    {
      string fpath = this.Request.FilePath.ToLower();
      ClientScriptManager csm = null;
      string scrpt = null;

      if (fpath.Contains("default.aspx") ||
        fpath.Contains("main.aspx"))
      {
        this.Response.Redirect(REDIRECT_URL_PARENT, true);
      }
      else
      {

        fpath = this.ResolveClientUrl(REDIRECT_URL_PARENT);

        scrpt = string.Format("window.top.location.replace('{0}');", fpath);
        
        //Ext.Net.X.AddScript(string.Format("alert(window.top.location.href);", fpath));
        //Ext.Net.X.AddScript(string.Format("alert(window.parent.location.href);", fpath));
        //Ext.Net.X.AddScript(string.Format("alert(window.opener.location.href);", fpath));

        //Ext.Net.X.AddScript(scrpt);

        //this.Response.Redirect(REDIRECT_URL_PARENT);

        csm = this.ClientScript;

        if (!csm.IsClientScriptBlockRegistered(this.GetType(), REDIRECT_PARENT))
        {
          csm.RegisterClientScriptBlock(this.GetType(), REDIRECT_PARENT, scrpt, true);
        }
      }
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
    <h2>Session timeout..</h2>
  </div>
  </form>
</body>
</html>
