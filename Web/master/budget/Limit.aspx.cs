using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;

public partial class master_budget_Limit : Scms.Web.Core.PageHandler
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      hfTahun.Text = DateTime.Now.Year.ToString();
      hfBulan.Text = DateTime.Now.Month.ToString();
    }
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);
    string pIdName = (e.ExtraParams["PrimaryNameID"] ?? string.Empty);
    //string tahunStr = (e.ExtraParams["Tahun"] ?? string.Empty);
    //string bulanStr = (e.ExtraParams["Bulan"] ?? string.Empty);

    //decimal tahun = 0,
    //  bulan = 0;

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      //decimal.TryParse(tahunStr, out tahun);
      //decimal.TryParse(bulanStr, out bulan);

      LimitCtrl1.CommandPopulate(pID, pIdName);
    }

    GC.Collect();
  }
}
