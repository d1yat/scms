using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Common;
using Ext.Net;

public partial class transaksi_wp_SerahTerimaPendingCtrl : System.Web.UI.UserControl
{
    public void Initialize(string gudang, string type)
    {
        hfGdg.Text = gudang;
        hfType.Text = type;

        Ext.Net.Store store = gridMain.GetStore();

        string tmp = string.Format(@"var xOpts = {{
                                  params: {{
                                      start: 0,
                                      limit: 20,
                                      model: '{0}',
                                      sort: 'noTrans',
                                      dir: 'ASC',
                                      parameters: [['gdg', '{1}', 'System.Char'],['tipe', '{0}', 'System.String']]
                                    }}
                                  }};", type.ToString(), gudang.ToString());

        X.AddScript(tmp);
        X.AddScript(string.Format("{0}.removeAll();{0}.reload(xOpts);", store.ClientID));

        GC.Collect();
    }

    public void CommandPopulate(bool isView, string pID)
    {
        if (isView)
        {
            winHeader.Hidden = false;
            winHeader.ShowModal();
        }
    }
}
