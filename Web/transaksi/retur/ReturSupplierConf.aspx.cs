using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_retur_ReturSupplierConf : Scms.Web.Core.PageHandler
{
  //private const string RS_PEMBELIAN = "PEMBELIAN";
  //private const string RN_REPACK = "REPACK";
  //private const string RN_CONF = "CONF";

  //private const string TYPE_RS_PEMBELIAN = "01";
  //private const string TYPE_RN_REPACK = "02";
  //private const string TYPE_RN_CONF = "01";

  //private Control MyLoadControl()
  //{
  //  Control c = null;

  //  const string CONT_NORMAL = "x1x1x1";
  //  const string CONT_NORMAL_DATA = "x1x1x1x1";

  //  c = phCtrl.FindControl(CONT_NORMAL);
  //  if (c == null)
  //  {
  //    c = this.LoadControl("ReturSupplierConfCtrl.ascx");

  //    c.ID = CONT_NORMAL;

  //    phCtrl.Controls.Add(c);
  //  }

  //  c = phCtrl.FindControl(CONT_NORMAL_DATA);
  //  if (c == null)
  //  {
  //    c = this.LoadControl("ReturSupplierPembelianConfirmCtrl.ascx");

  //    c.ID = CONT_NORMAL_DATA;

  //    phCtrl.Controls.Add(c);
  //  }

  //  return c;
  //}

  //protected void Page_Init(object sender, EventArgs e)
  //{
  //  if (!this.IsPostBack)
  //  {
  //    //string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

  //    //if (qryString.Equals("conf", StringComparison.OrdinalIgnoreCase))
  //    //{
  //    //  hfMode.Text = RN_CONF;
  //    //  hfType.Text = TYPE_RN_CONF;
  //    //}
  //    //else
  //    //{
  //    //  hfMode.Text = RN_CONF;
  //    //  hfType.Text = TYPE_RN_CONF;
  //    //}
  //  }
  //}
  
  private void GetTypeName()
  {
    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    Dictionary<string, object> dicSP = null;
    Dictionary<string, string> dicSPInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    string[][] paramX = new string[][]{
        new string[] { "c_notrans = @0", "43", "System.String"},
        new string[] { "c_portal = @0", "3", "System.Char"},
        new string[] { "c_type = @0", "04", "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "2001", paramX);

    try
    {
      dicSP = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicSP.ContainsKey("records") && (dicSP.ContainsKey("totalRows") && (((long)dicSP["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicSP["records"]);

        dicSPInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        hfTypeName.Text = dicSPInfo.GetValueParser<string>("v_ket");
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_retur_ReturSupplierConf:GetTypeName - ", ex.Message));
    }
    finally
    {
      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicSPInfo != null)
      {
        dicSPInfo.Clear();
      }
      if (dicSP != null)
      {
        dicSP.Clear();
      }
    }

    #endregion

    GC.Collect();
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    //transaksi_retur_ReturSupplierConfCtrl opr = MyLoadControl() as transaksi_retur_ReturSupplierConfCtrl;

    //if (opr == null)
    //{
    //  Functional.ShowMsgError("Objek control Retur supplier Pembelian.");
    //}
    //else
    //{
    //  opr.Initialize(storeGridRS.ClientID);
    //}

    //transaksi_retur_ReturSupplierConfCtrl opr = MyLoadControl() as transaksi_retur_ReturSupplierConfCtrl;

    //if (opr == null)
    //{
    //  Functional.ShowMsgError("Objek control Retur supplier Pembelian.");
    //}
    //else
    //{
    //  opr.Initialize(storeGridRS.ClientID);
    //}

    if (!this.IsPostBack)
    {
      GetTypeName();

      ReturSupplierConfCtrl1.Initialize(storeGridRS.ClientID);
      ReturSupplierPembelianConfirmCtrl1.Initialize(storeGridRS.ClientID, hfTypeName.Text);
    }
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);
    string gdgID = (e.ExtraParams["GudangID"] ?? string.Empty);

    ReturSupplierPembelianConfirmCtrl1.CommandPopulate(false, pID);

    //string modeValue = hfMode.Text.Trim().ToUpper();

    //if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    //{
    //  //PurchaseOrderCtrl1.CommandPopulate(false, pID);

    //  switch (modeValue)
    //  {
        
    //    case RN_CONF:
    //      {
    //        transaksi_retur_ReturSupplierConfCtrl opr = MyLoadControl() as transaksi_retur_ReturSupplierConfCtrl;

    //        if (opr == null)
    //        {
    //          Functional.ShowMsgError("Objek kontrol Retur Pembelian tidak dapat dibuka.");
    //        }
    //        else
    //        {
    //          opr.CommandPopulate(false, pID);
    //        }
    //      }
    //      break;
        
    //  }
    //}

    GC.Collect();
  }

  protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
  {
    if (!this.IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    ReturSupplierConfCtrl1.CommandPopulate(true, null);

    //string modeValue = hfMode.Text.Trim().ToUpper();

    //switch (modeValue)
    //{
     
    //  case RN_CONF:
    //    {
    //      transaksi_retur_ReturSupplierConfCtrl opr = MyLoadControl() as transaksi_retur_ReturSupplierConfCtrl;

    //      if (opr == null)
    //      {
    //        Functional.ShowMsgError("Objek kontrol Retur Pembelian tidak dapat dibuka.");
    //      }
    //      else
    //      {
    //        opr.CommandPopulate(true, null);
    //      }
    //    }
    //    break;
    //}
  }
  
  protected void btnPrintRS_OnClick(object sender, DirectEventArgs e)
  {
    if (!this.IsAllowPrinting)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
      return;
    }

    ReturSupplierPrintCtrl1.ShowPrintPage("03");
  }
}
