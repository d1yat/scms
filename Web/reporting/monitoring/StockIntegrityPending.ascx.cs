using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Common;
using Ext.Net;

public partial class reporting_Monitoring_StockIntegrityPending : System.Web.UI.UserControl
{
    public void Initialize(string wndDownload,string gudang)
    {
        hidWndDown.Text = wndDownload;
        hfGdg.Text = gudang;
    }
}
