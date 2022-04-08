using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Common;

public partial class reporting_Inventory_Default : Scms.Web.Core.PageHandler
{

    private const string RPT_Recall = "recall";
    

    protected void Page_Init(object sender, EventArgs e)
    {
        string qryString = null;

        if ((!this.IsPostBack) && (!Ext.Net.X.IsAjaxRequest))
        {
            qryString = (this.Request.QueryString.IsContainKey("mode") ? (this.Request.QueryString["mode"] ?? string.Empty).Trim() : string.Empty).ToLower();

            switch (qryString)
            {

                case RPT_Recall:
                    {
                        Recall.InitializePage(wndDown.ClientID);
                        Recall.Visible = true;
                    }
                    break;

               





            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
    }
}