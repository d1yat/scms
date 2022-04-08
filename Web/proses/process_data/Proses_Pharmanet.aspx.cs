using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_proses_pharmanet : Scms.Web.Core.PageHandler
{
    #region Private

    private  void ClearEntry()
    {
        

        if (this.ActiveGudang.Length < 2)
        {
            
            cbCustomerHdr.Clear();
            cbStatusHdr.Clear();
        }
        

        X.AddScript(string.Format("clearFilterGridHeader({0}, {1}, {2});reloadFilterGrid({0});", gridMain.ClientID, txSPFltr.ClientID, txPLFltr.ClientID, cbPrincipalFltr.ClientID));
    }

   
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (!this.IsPostBack)
        {
            //GetTypeName();

            Proses_PharmanetCtrl2.Initialize(storeGridSP.ClientID, hfTypeName.Text);
        }

    }



    

    protected void btnRefresh_OnClick(object sender, DirectEventArgs e)
    {
        ClearEntry();
    }

      

    protected void gridMainCommand(object sender, DirectEventArgs e)
    {
        string cmd = (e.ExtraParams["Command"] ?? string.Empty);
        string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
        string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

        if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
        {
            Proses_PharmanetCtrl2.CommandPopulate(false, pID);
        }

        GC.Collect();
    }

    



    

}
