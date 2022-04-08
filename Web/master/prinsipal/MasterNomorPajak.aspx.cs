using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class master_nomorpajak_MasterNomorPajak : Scms.Web.Core.PageHandler
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      MasterNomorPajakCtrl1.Initialize(storeGrid.ClientID);
    }
  }

  protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
  {
    if (!this.IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    MasterNomorPajakCtrl1.CommandPopulate(true, null);
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      MasterNomorPajakCtrl1.CommandPopulate(false, pID);
    }

    GC.Collect();
  }
}
