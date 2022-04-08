using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class Master : System.Web.UI.MasterPage
{
  private void AddCustomize()
  {
    if (this.Page != null)
    {
      HtmlHead head = this.Page.Header;

      HtmlLink link = new HtmlLink()
      {
        Href = this.ResolveUrl("~/styles/main.css"),        
      };
      link.Attributes.Add("type", "text/css");
      link.Attributes.Add("rel", "Stylesheet");
      head.Controls.Add(link);

      link = new HtmlLink()
      {
        Href = this.ResolveUrl("~/styles/desktop.css"),
      };
      link.Attributes.Add("type", "text/css");
      link.Attributes.Add("rel", "Stylesheet");
      head.Controls.Add(link);

      HtmlGenericControl gen = new HtmlGenericControl("script");
      gen.Attributes["src"] = this.ResolveUrl("~/scripts/simpleutils.js");
      gen.Attributes["type"] = "text/javascript";
      gen.Attributes["language"] = "javascript";
      head.Controls.Add(gen);
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    AddCustomize();
  //<link type="text/css" rel="Stylesheet" href='<%= this.ResolveUrl("~/styles/main.css") %>' />

  //<script src='<%= this.ResolveUrl("~/scripts/simpleutils.js") %>' type="text/javascript" language="javascript"></script>
  
    /*
     */
  }
}
