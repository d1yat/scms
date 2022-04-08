using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Common;
using Ext.Net;

public partial class transaksi_wp_SerahTerimaPOPendingCtrl : System.Web.UI.UserControl
{
    public void Initialize(string gudang)
    {
        hfGdg.Text = gudang;
        
        Ext.Net.Store store = gridMain.GetStore();

        string tmp = string.Format(@"var xOpts = {{
                                  params: {{
                                      start: 0,
                                      limit: 20,
                                      allQuery: 'true',
                                      model: '0340',
                                      sort: '',
                                      dir: '',
                                      parameters: [['gdg', '{0}', 'System.Char'],
                                                    ['c_type = @0', '01', ''],
                                                    ['d_wpdate = @0', '{1}', 'System.DateTime'],
                                                    ['(l_scan == null ? false : l_scan) = @0', 'false' , 'System.Boolean']]
                                    }}
                                  }};", gudang.ToString(), DateTime.Now.ToString("dd-MM-yyyy"));

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
