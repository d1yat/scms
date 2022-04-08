using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_penjualan_Legend : System.Web.UI.UserControl
{  
  public void Initialize(string gudang, string gudangDesc, string storeIDGridMain)
  {
    
  }

  public void CommandPopulate(bool isAdd, string pID)
  {

    if (isAdd)
    {

      winDetail.Hidden = false;
      winDetail.ShowModal();


    }
    else
    {
      
    }
  }
}