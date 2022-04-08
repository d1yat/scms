using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_wp_SerahTerima : Scms.Web.Core.PageHandler
{
    private const string ST_PICKER = "SerahTerimaPicker";
    private const string ST_INKJET = "SerahTerimaInkJet";
    private const string ST_CHECKER = "SerahTerimaChecker";
    private const string ST_PACKER = "SerahTerimaPAcker";
    private const string ST_TRANSPORTASI = "SerahTerimaTransportasi";
    
    private const string ST_RC = "SerahTerimaRC";
    private const string ST_PBBR = "SerahTerimaPBBR";
    private const string ST_BASPBGdg = "SerahTerimaBASPBGdg";
    private const string ST_BASPBAdm = "SerahTerimaBASPBAdm";
    private const string ST_CPR = "SerahTerimaCPR";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            hfGdg.Text = this.ActiveGudang;

            if (hfMode.Text.Equals(ST_PICKER))
            {
                SerahTerimaPickerCtrl.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridWP.ClientID);
            }
            else if (hfMode.Text.Equals(ST_INKJET))
            {
                SerahTerimaInkJetCtrl.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridWP.ClientID);
            }
            else if (hfMode.Text.Equals(ST_CHECKER))
            {
                SerahTerimaCheckerCtrl.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridWP.ClientID);
            }
            else if (hfMode.Text.Equals(ST_PACKER))
            {
                SerahTerimaPackerCtrl.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridWP.ClientID);
            }
            else if(hfMode.Text.Equals(ST_TRANSPORTASI))
            {
                SerahTerimaTransportasiCtrl.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridWP.ClientID);
            }
            else if (hfMode.Text.Equals(ST_RC))
            {
                SerahTerimaRCCtrl.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridWP.ClientID);
            }
            else if (hfMode.Text.Equals(ST_PBBR))
            {
                SerahTerimaPBBRCtrl.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridWP.ClientID);
            }
            else if (hfMode.Text.Equals(ST_BASPBGdg))
            {
                SerahTerimaBASPBGdgCtrl.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridWP.ClientID);
            }
            else if (hfMode.Text.Equals(ST_BASPBAdm))
            {
                SerahTerimaBASPBAdmCtrl.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridWP.ClientID);
            }
            else if (hfMode.Text.Equals(ST_CPR))
            {
                SerahTerimaCPRCtrl.Initialize(this.ActiveGudang, this.ActiveGudangDescription, storeGridWP.ClientID);
            }
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            string qryString = (this.Request.QueryString["mode"] ?? string.Empty);
            btnSearch.Visible = false;

            if (qryString.Equals("picker", StringComparison.OrdinalIgnoreCase))
            {
                hfMode.Text = ST_PICKER;
                hfType.Text = "01";
                btnPrintPL.Visible = false;
            }
            else if (qryString.Equals("inkjet", StringComparison.OrdinalIgnoreCase))
            {
                hfMode.Text = ST_INKJET;
                hfType.Text = "1A";
                btnPrintPL.Visible = false;
            }
            else if (qryString.Equals("checker", StringComparison.OrdinalIgnoreCase))
            {
                hfMode.Text = ST_CHECKER;
                hfType.Text = "02";
                btnPrintPL.Visible = false;
            }
            else if (qryString.Equals("packer", StringComparison.OrdinalIgnoreCase))
            {
                hfMode.Text = ST_PACKER;
                hfType.Text = "03";
                btnPrintPL.Visible = false;
            }
            else if (qryString.Equals("transportasi", StringComparison.OrdinalIgnoreCase))
            {
                hfMode.Text = ST_TRANSPORTASI;
                hfType.Text = "04";
                btnSearch.Visible = true;
                btnPrintPL.Visible = false;
            }
            else if (qryString.Equals("transportasi", StringComparison.OrdinalIgnoreCase))
            {
                hfMode.Text = ST_TRANSPORTASI;
                hfType.Text = "04";
                btnSearch.Visible = true;
                btnPrintPL.Visible = false;
            }
            else if (qryString.Equals("rc", StringComparison.OrdinalIgnoreCase))
            {
                hfMode.Text = ST_RC;
                hfType.Text = "05";
                btnView.Visible = false;

            }
            else if (qryString.Equals("pbbr", StringComparison.OrdinalIgnoreCase))
            {
                hfMode.Text = ST_PBBR;
                hfType.Text = "06";
                btnView.Visible = false;
                btnPrintPL.Visible = true;
            }
            else if (qryString.Equals("baspbGdg", StringComparison.OrdinalIgnoreCase))
            {
                hfMode.Text = ST_BASPBGdg;
                hfType.Text = "07";
                btnView.Visible = false;
                btnPrintPL.Visible = false;
            }
            else if (qryString.Equals("baspbAdm", StringComparison.OrdinalIgnoreCase))
            {
                hfMode.Text = ST_BASPBAdm;
                hfType.Text = "08";
                btnView.Visible = false;
                btnPrintPL.Visible = false;
            }
            else 
            {
                hfMode.Text = ST_CPR;
                hfType.Text = "09";
                btnView.Visible = false;
                btnPrintPL.Visible = false;
            }
        }
    }

    [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
    {
        if (!this.IsAllowAdd)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
            return;
        }

        if (hfMode.Text.Equals(ST_PICKER))
        {
            SerahTerimaPickerCtrl.CommandPopulate(true, null);
        }
        else if (hfMode.Text.Equals(ST_INKJET))
        {
            SerahTerimaInkJetCtrl.CommandPopulate(true, null);
        }
        else if (hfMode.Text.Equals(ST_CHECKER))
        {
            SerahTerimaCheckerCtrl.CommandPopulate(true, null);
        }
        else if (hfMode.Text.Equals(ST_PACKER))
        {
            SerahTerimaPackerCtrl.CommandPopulate(true, null);
        }
        else if (hfMode.Text.Equals(ST_TRANSPORTASI))
        {
            SerahTerimaTransportasiCtrl.CommandPopulate(true, null);
        }
        else if (hfMode.Text.Equals(ST_RC))
        {
            SerahTerimaRCCtrl.CommandPopulate(true, null); 
        }
        else if (hfMode.Text.Equals(ST_PBBR))
        {
            SerahTerimaPBBRCtrl.CommandPopulate(true, null);
        }
        else if (hfMode.Text.Equals(ST_BASPBGdg))
        {
            SerahTerimaBASPBGdgCtrl.CommandPopulate(true, null);
        }
        else if (hfMode.Text.Equals(ST_BASPBAdm))
        {
            SerahTerimaBASPBAdmCtrl.CommandPopulate(true, null);
        }
        else if (hfMode.Text.Equals(ST_CPR))
        {
            SerahTerimaCPRCtrl.CommandPopulate(true, null);
        }
    }

    [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void btnViewPending_OnClick(object sender, DirectEventArgs e)
    {
        string type = null;
        if (hfMode.Text.Equals(ST_PICKER))
        {
            type = "0306";
        }
        else if (hfMode.Text.Equals(ST_INKJET))
        {
            type = "0306x";
        }
        else if (hfMode.Text.Equals(ST_CHECKER))
        {
            type = "0306a";            
        }
        else if (hfMode.Text.Equals(ST_PACKER))
        {
            type = "0306b";
        }
        else if (hfMode.Text.Equals(ST_TRANSPORTASI))
        {
            type = "0306c";
        }

        SerahTerimaPendingCtrl.Initialize(this.ActiveGudang, type);
        SerahTerimaPendingCtrl.CommandPopulate(true, null);
    }

    [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void btnSearch_OnClick(object sender, DirectEventArgs e)
    {
        //string type = null;
        //if (hfMode.Text.Equals(ST_PICKER))
        //{
        //    type = "0306";
        //}
        //else if (hfMode.Text.Equals(ST_INKJET))
        //{
        //    type = "0306x";
        //}
        //else if (hfMode.Text.Equals(ST_CHECKER))
        //{
        //    type = "0306a";
        //}
        //else if (hfMode.Text.Equals(ST_PACKER))
        //{
        //    type = "0306b";
        //}
        //else if (hfMode.Text.Equals(ST_TRANSPORTASI))
        //{
        //    type = "0306c";
        //}
        
        SerahTerimaSearchCtrl.Initialize(this.ActiveGudang);
        SerahTerimaSearchCtrl.CommandPopulate(true, null);
    }

    protected void gridMainCommand(object sender, DirectEventArgs e)
    {
        string cmd = (e.ExtraParams["Command"] ?? string.Empty);
        string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
        string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

        if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
        {
            if (hfMode.Text.Equals(ST_PICKER))
            {
                SerahTerimaPickerCtrl.CommandPopulate(false, pID);
            }
            else if (hfMode.Text.Equals(ST_INKJET))
            {
                SerahTerimaInkJetCtrl.CommandPopulate(false, pID);
            }
            else if (hfMode.Text.Equals(ST_CHECKER))
            {
                SerahTerimaCheckerCtrl.CommandPopulate(false, pID);
            }
            else if (hfMode.Text.Equals(ST_PACKER))
            {
                SerahTerimaPackerCtrl.CommandPopulate(false, pID);
            }
            else if (hfMode.Text.Equals(ST_TRANSPORTASI))
            {
                SerahTerimaTransportasiCtrl.CommandPopulate(false, pID);
            }
            else if (hfMode.Text.Equals(ST_RC))
            {
                SerahTerimaRCCtrl.CommandPopulate(false, pID); 
            }
            
            else if (hfMode.Text.Equals(ST_PBBR))
            {
                SerahTerimaPBBRCtrl.CommandPopulate(false, pID);
            }
            
            else if (hfMode.Text.Equals(ST_BASPBGdg))
            {
                SerahTerimaBASPBGdgCtrl.CommandPopulate(false, pID);
            }
            else if (hfMode.Text.Equals(ST_BASPBAdm))
            {
                SerahTerimaBASPBAdmCtrl.CommandPopulate(false, pID);
            }
            else if (hfMode.Text.Equals(ST_CPR))
            {
                SerahTerimaCPRCtrl.CommandPopulate(false, pID);
            }
        }

        GC.Collect();
    }

    protected void btnPrintST_OnClick(object sender, DirectEventArgs e)
    {
        if (!this.IsAllowPrinting)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
            return;
        }


        var tipe = string.Empty;
        if (hfMode.Text.Equals(ST_RC))
        {
            tipe = "05";
        }
        else if (hfMode.Text.Equals(ST_PBBR))
        {
            tipe = "06";
        }

        
        SERAHTERIMAPRINTCTRL.InitializePage(tipe);

        SERAHTERIMAPRINTCTRL.ShowPrintPage();
    }

    
}
