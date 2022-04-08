using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_wp_SerahTerimaCtrl : System.Web.UI.UserControl
{
  protected void Page_Load(object sender, EventArgs e)
  {

  }

  public void CommandPopulate(bool isAdd, string pID)
  {
    if (isAdd)
    {
      //ClearEntrys();

      wndIDUser.Hidden = false;
      wndIDUser.ShowModal();

      pnlGridDetail.Hidden = true;

      wndIDUser.Height = 190;
    }
    else
    {
      //PopulateDetail("c_spno", pID);
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void text_onchange(object sender, DirectEventArgs e)
  {
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void btn_onclick(object sender, DirectEventArgs e)
  {
    pnlGridDetail.Hidden = false;

    wndIDUser.Height = 480;
    txNipPenyerah.Disabled = true;


  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void ReloadBtn_Click(object sender, DirectEventArgs e)
  {
    txNamePenyerah.Clear();
    txNipPenyerah.Clear();
    txNipPenyerah.Disabled = false;
    pnlGridDetail.Hidden = true;

    wndIDUser.Height = 190;
  }
}
