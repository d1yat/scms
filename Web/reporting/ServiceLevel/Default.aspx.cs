using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Common;

public partial class reporting_Inventory_Default : Scms.Web.Core.PageHandler
{

    private const string RPT_CURRENTSTOK = "servicelevelpo";
    private const string RPT_SERVICELEVELCABANG = "servicelevelcbg";

    protected void Page_Init(object sender, EventArgs e)
    {
        string qryString = null;

        if ((!this.IsPostBack) && (!Ext.Net.X.IsAjaxRequest))
        {
            qryString = (this.Request.QueryString.IsContainKey("mode") ? (this.Request.QueryString["mode"] ?? string.Empty).Trim() : string.Empty).ToLower();

            switch (qryString)
            {

                case RPT_CURRENTSTOK:
                    {
                        ServiceLevelPO.InitializePage(wndDown.ClientID);
                        ServiceLevelPO.Visible = true;
                    }
                    break;

                case RPT_SERVICELEVELCABANG:
                    {
                        ServiceLevelCabang.InitializePage(wndDown.ClientID);
                        ServiceLevelCabang.Visible = true;
                    }
                    break;





            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
    }
}