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
using System.Collections.Generic;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_pembelian_HistorySP : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    private void PopulateDetail(string pName, string pID)
    {
        #region Parser Detail

        try
        {
            Ext.Net.Store store = gridDetailHistory.GetStore();
            if (store.Proxy.Count > 0)
            {
                Ext.Net.ScriptTagProxy stp = store.Proxy[0] as Ext.Net.ScriptTagProxy;
                if ((stp != null) && stp.CallbackParam.Equals(Functional.NAME_SOA_SCMS_CALLBACK, StringComparison.OrdinalIgnoreCase))
                {
                    string param = (store.BaseParams["parameters"] ?? string.Empty);
                    if (string.IsNullOrEmpty(param))
                    {
                        store.BaseParams.Add(new Ext.Net.Parameter("parameters", string.Format("[['{0} = @0', '{1}', 'System.String']]", pName, pID), ParameterMode.Raw));
                    }
                    else
                    {
                        store.BaseParams["parameters"] = string.Format("[['{0} = @0', '{1}', 'System.String']]", pName, pID);
                    }
                }
            }
          txtspno.Text = pID;
          X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
        }
        catch (Exception ex)
        {
          System.Diagnostics.Debug.WriteLine(
            string.Concat("transaksi_pembelian_HistorySP:PopulateDetail Detail - ", ex.Message));
        }        
        #endregion

        winDetail.Hidden = false;
        winDetail.ShowModal();

        GC.Collect();
    }

    public void CommandPopulate(bool isAdd, string pID)
    {
        if (isAdd)
        {

        }
        else
        {
            PopulateDetail("c_spno",pID);
        }
    }
}
