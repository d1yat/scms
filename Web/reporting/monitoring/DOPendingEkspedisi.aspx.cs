using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_pengiriman_Ekspedisi : Scms.Web.Core.PageHandler
{
  #region Private
  
  #endregion

  [DirectMethod(ShowMask = true)]
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      hfGdg.Text = this.ActiveGudang;
    }
  }

  protected void GridMainCommand(object sender, DirectEventArgs e)
  {
  }  
}