using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_bonus_ClaimCurrentOutStanding : Scms.Web.Core.PageHandler
{
  protected void Page_Load(object sender, EventArgs e)
  {

  }
  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);
    string PrimaryName = (e.ExtraParams["PrimaryName"] ?? string.Empty);
    string Jml = (e.ExtraParams["Jml"] ?? string.Empty);

    ClaimCurrentOutStandingCtrl.CommandPopulate(pID, PrimaryName, Jml);
  }
}
