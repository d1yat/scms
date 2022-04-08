<%@ Page Language="C#" EnableViewState="false" %>

<%@ Import Namespace="System.IO" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">
  private string getQueryString(string name)
  {
    foreach (string s in Request.QueryString.Keys)
    {
      //if (string.Compare(name, s, StringComparison.OrdinalIgnoreCase) == 0)
      if(s.Equals(name, StringComparison.OrdinalIgnoreCase))
      {
        return Request[name];
      }
    }

    return string.Empty;
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!Page.IsPostBack)
    {
      bool ret = false;
      bool isDownloadable = false;

      HttpResponse resp = this.Response;

      string qryFile = getQueryString("f");
      string qryPath = getQueryString("p");
      string qryFrmt = getQueryString("c");
      string origName = getQueryString("o");
      string qryDownload = getQueryString("dwnl");

      if (qryDownload.Equals("1"))
      {
        isDownloadable = true;
      }
      else
      {
        bool.TryParse(qryDownload, out isDownloadable);
      }

      if (string.IsNullOrEmpty(qryFile) ||
        string.IsNullOrEmpty(qryFrmt) ||
        string.IsNullOrEmpty(qryPath) ||
        string.IsNullOrEmpty(origName))
      {
        Functional.ShowMsgError("Parameter salah.");
        
        goto AkhirSub;
      }

      try { resp.Headers.Clear(); }
      catch { ;}
      finally { ;}
      
      string sFile = string.Concat(Server.MapPath(string.Format("~/App_Data/{0}/", qryPath)), qryFile);

      FileInfo fi = new FileInfo(sFile);

      if (fi.Exists)
      {
        resp.Cache.SetAllowResponseInBrowserHistory(false);
        resp.Cache.SetCacheability(HttpCacheability.NoCache);
        resp.Cache.SetExpires(DateTime.Now.AddHours(-3));
        resp.Cache.SetNoServerCaching();        

        if (isDownloadable)
        {
          resp.AppendHeader("Content-Disposition",
              string.Format("attachment; filename={0}", this.Server.UrlEncode(origName)));

          resp.ContentType = "octet-stream";
        }
        else
        {
          resp.BufferOutput = false;
          
          resp.Buffer = false;
          
          resp.Cache.SetExpires(DateTime.Now.AddDays(-1));
          
          resp.Cache.SetNoStore();
          
          switch (qryFrmt.ToLower())
          {
            case "pdf":
              resp.ContentType = "application/pdf";
              break;
            case "xls":
              resp.ContentType = "application/vnd.ms-excel";
              break;
          }
        }

        if (fi.Exists && resp.IsClientConnected)
        {
          try
          {
            resp.WriteFile(fi.FullName, true);
          }
          catch (Exception ex)
          {
            System.Diagnostics.Debug.WriteLine(
              string.Format("Viewer.aspx:Page_Load - {0}", ex.Message));
          }          

          ret = true;

          //if (fi != null)
          //{
          //  try
          //  {
          //    fi.Delete();
          //  }
          //  catch (Exception ex)
          //  {
          //    System.Diagnostics.Debug.WriteLine(
          //      string.Format("Viewer.aspx:Page_Load DeleteFile - {0}", ex.Message));
          //  }
          //}        
        }
      }
      else
      {
        resp.StatusCode = (int)System.Net.HttpStatusCode.PreconditionFailed;
      }

      this.Title = "View reports";

    AkhirSub:
      if (!ret)
      {
        Functional.ShowMsgError("Maaf, file tidak dapat di akses, kemungkinan file tidak ada atau terkunci.");
      }

      //Aig.Common.ClientBrowser.ShowScriptAtBlock(this,
      //  "closeWindow();", "closeWind");

      if (isDownloadable)
      {
        ClientScriptManager csm = this.ClientScript;
        
        if (!csm.IsClientScriptBlockRegistered("closeWind"))
        {
          csm.RegisterClientScriptBlock(this.GetType(), "closeWind", "window.close();", true);
        }
      }
           
      GC.Collect();

      if (isDownloadable && ret)
      {
        resp.Flush();

        resp.Close();
        
        resp.End();
      }
    }
  }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server" id="head1">
  <title>Viewer Page</title>

  <script language="javascript" type="text/javascript">
    function closeWindow() {
      window.close();

      return false;
    }
  </script>

</head>
<body>
  <form id="form1" runat="server" method="get" action="#" enableviewstate="false" >
    <br />
    <br />
    <center>
      <input type="button" id="btnClose" value="Close" onclick="return closeWindow()" />
    </center>
  </form>
</body>
</html>
