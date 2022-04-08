using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_wp_SerahTerimaPO : Scms.Web.Core.PageHandler
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            hfGdg.Text = this.ActiveGudang;
            SerahTerimaPOCtrl.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridWP.ClientID);
        }
    }

    [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
    {
        if (!this.IsAllowAdd)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
            return;
        }
        SerahTerimaPOCtrl.CommandPopulate(true, null);
    }

    [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void btnViewPending_OnClick(object sender, DirectEventArgs e)
    {
        SerahTerimaPOPendingCtrl.Initialize(this.ActiveGudang);
        SerahTerimaPOPendingCtrl.CommandPopulate(true, null);
    }

    protected void gridMainCommand(object sender, DirectEventArgs e)
    {
        string cmd = (e.ExtraParams["Command"] ?? string.Empty);
        string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
        string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);


        if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
        {
            SerahTerimaPOCtrl.CommandPopulate(false, pID);            
        }

        GC.Collect();
    }
}
