using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_wp_MonitoringPO : Scms.Web.Core.PageHandler
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            hfGdg.Text = this.ActiveGudang;
            MonitoringPOCtrl.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridWP.ClientID);
        }
    }

    protected void gridMainCommand(object sender, DirectEventArgs e)
    {
        string cmd = (e.ExtraParams["Command"] ?? string.Empty);
        string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
        string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);
        string pStatus = (e.ExtraParams["Status"] ?? string.Empty);


        if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
        {
            if (pStatus.ToUpper() == "WAITING")
            {
                e.ErrorMessage = "Dokumen belum di receiving.";
                e.Success = false;
            }
            else
            {
                MonitoringPOCtrl.CommandPopulate(pID);
            }
        }

        GC.Collect();
    }
}
