using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_pembelian_LimitPOItem : Scms.Web.Core.PageHandler
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            DateTime date = DateTime.Now;

            cbPeriode1.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
            date = date.AddYears(-1);
            cbPeriode1.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
            date = date.AddYears(-1);
            cbPeriode1.Items.Add(new Ext.Net.ListItem(date.Year.ToString(), date.Year.ToString()));
            cbPeriode1.SelectedIndex = 0;

            Functional.PopulateBulan(cbPeriode2, date.Month);

            LimitPOItemCtrl.Initialize(storeGridItem.ClientID);
        }
    }

    protected void gridMainCommand(object sender, DirectEventArgs e)
    {
        string cmd = (e.ExtraParams["Command"] ?? string.Empty);
        string tahun = (e.ExtraParams["tahun"] ?? string.Empty);
        string bulan = (e.ExtraParams["bulan"] ?? string.Empty);
        string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

        if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
        {
            LimitPOItemCtrl.CommandPopulate(pID, tahun, bulan);
        }

        GC.Collect();
    }
}
