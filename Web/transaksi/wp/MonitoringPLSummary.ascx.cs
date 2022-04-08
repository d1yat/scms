//Created By Indra Monitoring Process 20180523FM

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

public partial class transaksi_wp_MonitoringPLSummary : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void Initialize(string storeIDGridMain, string NoForm)
    {
        //hfStoreID.Text = storeIDGridMain;
        //txExpired.Text = DateTime.Now.ToString("dd-MM-yyyy");
    }

    public void CommandPopulate(bool isAdd, string pID, string CABANG, string SPRECEIVED, string CREATEPL, string PICKING, string CREATEDO, string CHECKING, string PACKING, string READY)
    {
        if (isAdd)
        {
            winDetail2.Hidden = false;
            //winDetail2.ShowModal();
            lblCABANG.Text      = CABANG;
            lblSPRECEIVED.Text  = SPRECEIVED;
            lblCREATEPL.Text    = CREATEPL;
            lblPICKING.Text     = PICKING;
            lblCREATEDO.Text    = CREATEDO;
            lblCHECKING.Text    = CHECKING;
            lblPACKING.Text     = PACKING;
            lblREADY.Text       = READY;

        }
        else
        {

        }
    }
}
