using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Ext.Net;

public partial class transaksi_retur_SerahTerimaGood : Scms.Web.Core.PageHandler
{
    protected void Page_Load(object sender, EventArgs e)
    {
        hfGudang.Text = this.Cabang;
    }

    protected void gridMainCommand(object sender, DirectEventArgs e)
    {
        string cmd = (e.ExtraParams["Command"] ?? string.Empty);
        string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
        string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

        if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
        {
            SerahTerimaCtrl2.CommandPopulate(false, pID, hfMode.Text);
            //ReturCustomerProcessCtrl.CommandPopulate(false, pID);
        }

        GC.Collect();
    }

    protected void btnPrint_OnClick(object sender, DirectEventArgs e)
    {
        SerahTerimaPrint1.CommandPopulate();
    }
}
