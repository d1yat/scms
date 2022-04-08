using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_memo_MemoSTTDonasiCtrl : System.Web.UI.UserControl
{

  public void CommandPopulate(bool isAdd, string pID, string gdgID, string Type)
  {
    if (isAdd)
    {
      //hfAdjType.Text = Type;
      //ClearEntrys();

      switch (Type)
      {
        case "04":
          //winDetail.Title = "Adjustment FB";
          break;
      }


      //winDetail.Hidden = false;
      //winDetail.ShowModal();
    }
    else
    {
      //winDetail.Title = "Adjustment " + pID;
      //PopulateDetail("c_adjno", pID, gdgID);
    }
  }
}
