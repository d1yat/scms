using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_pengiriman_EkspedisiGudang : Scms.Web.Core.PageHandler
{
  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      EkspedisiGudangViewCtrl.CommandPopulate(false, pID);
    }

    GC.Collect();
  }

  protected void Page_Init(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      hfType.Text = "02";
      //hfNip.Text = this.Nip;
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      hfGdg.Text = this.ActiveGudang;
      EkspedisiGudangViewCtrl.Initialize(this.ActiveGudang, this.ActiveGudangDescription, strGridMain.ClientID);
      
    }
  }

}
