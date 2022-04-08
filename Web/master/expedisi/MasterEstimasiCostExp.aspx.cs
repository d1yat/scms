using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class master_expedisi_MasterEstimasiCostExp : Scms.Web.Core.PageHandler
{
  protected void Page_Load(object sender, EventArgs e)
  {

  }

  [DirectMethod(ShowMask = true)]
  public void DeleteMethod(string plNumber, string keterangan)
  {
  }

  protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
  {
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      MasterEstimasiCostExpCtrl.CommandPopulate(false, pID);
    }
  }
}
