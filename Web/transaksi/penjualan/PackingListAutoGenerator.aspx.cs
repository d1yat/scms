using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_penjualan_PackingListAutoGenerator : Scms.Web.Core.PageHandler
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      hfGdg.Text = this.ActiveGudang;

      taskMgr.StartTask("servertime");
    }
  }

  protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
  {
    
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    
  }

  protected void Page_Init(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      hfType.Text = "01";
    }
  }

  protected void btnPrintPL_OnClick(object sender, DirectEventArgs e)
  {
    if (!this.IsAllowPrinting)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
      return;
    }

  }

  protected void RefreshTime(object sender, DirectEventArgs e)
  {
    Store eStr = gridMain.GetStore();
    gridMain.Reload();
    //eStr.DataBind();
  }
}
