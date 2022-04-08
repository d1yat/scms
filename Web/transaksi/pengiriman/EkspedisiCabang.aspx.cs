using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_pengiriman_EkspedisiCabang : Scms.Web.Core.PageHandler
{
  protected void Proses_OnClick(object sender, DirectEventArgs e)
  {
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      EkspedisiCabangCtrl.Initialize(strGridMain.ClientID);
    }
  }

  protected void GridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);
    string secID = (e.ExtraParams["SecondaryID"] ?? string.Empty);

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      EkspedisiCabangCtrl.CommandPopulate(pID, secID);
    }

    GC.Collect();
  }
}
