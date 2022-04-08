using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Common;
using Ext.Net;

public partial class transaksi_wp_SerahTerimaSearchCtrl : System.Web.UI.UserControl
{
    public void Initialize(string gudang)
    {
        hfGdg.Text = gudang;
    }

    public void CommandPopulate(bool isView, string pID)
    {
        if (isView)
        {
            winHeader.Hidden = false;
            winHeader.ShowModal();
        }
    }
}
