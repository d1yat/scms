/*
 * Created By Indra
 * 20171231FMs 
*/

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class master_prinsipal_MasterPrinsipalHistory : System.Web.UI.UserControl
{

    public void Initialize(string storeIDGridMain)
    {
        hfStoreID.Text = storeIDGridMain;
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void CommandPopulate()
    {
        PopulateDetail();
    }

    private void PopulateDetail()
    {        

        #region Parser Detail

        try
        {
            Ext.Net.Store store = gridDetailHistory.GetStore();            
            X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
              string.Concat("transaksi_pembelian_SuratPesananCtrl:PopulateDetail Detail - ", ex.Message));
        }

        #endregion

        winDetail.Hidden = false;
        winDetail.ShowModal();

        GC.Collect();
    }
}
