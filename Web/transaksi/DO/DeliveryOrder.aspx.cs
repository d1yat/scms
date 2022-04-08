using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_DO_DeliveryOrder : Scms.Web.Core.PageHandler
{
  private const string DO_PL = "DOPL";
  private const string DO_STT = "DOSTT";

  private const string TYPE_DO_PL = "01";
  private const string TYPE_DO_STT = "02";

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

      if (qryString.Equals("dostt", StringComparison.OrdinalIgnoreCase))
      {
        DOSTTCtrl.Initialize(storeGridPL.ClientID);
      }
      else
      {
        DOPLCtrl.Initialize(storeGridPL.ClientID);
      }
    }
  }

  protected void Page_Init(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

      if (qryString.Equals("dostt", StringComparison.OrdinalIgnoreCase))
      {
        hfMode.Text = DO_STT;
        hfType.Text = TYPE_DO_STT;
      }
      else
      {
        hfMode.Text = DO_PL;
        hfType.Text = TYPE_DO_PL;
      }
    }
  }

  protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
  {
    if (!this.IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    string modeValue = hfMode.Text.Trim().ToUpper();

    switch (modeValue)
    {
      case DO_STT:
        {
          DOSTTCtrl.CommandPopulate(true, null, null, TYPE_DO_STT);
        }
        break;
      default:
        {
          DOPLCtrl.CommandPopulate(true, null, null, TYPE_DO_PL);
        }
        break;
    }
  }

  protected void btnPrintDO_OnClick(object sender, DirectEventArgs e)
  {
    //Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    //if (!pag.IsAllowPrinting)
    //{
    //  Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
    //  return;
    //}

    //DOPLPrint.ShowPrintPage();
    //DO_PL_Print.ShowPrintPage();
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    if (!this.IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    string modeValue = hfMode.Text.Trim().ToUpper();

    switch (modeValue)
    {
      case DO_STT:
        {

          DOSTTCtrl.CommandPopulate(false, pName, pID, TYPE_DO_STT);
        }
        break;
      default:
        {
          DOPLCtrl.CommandPopulate(false, pName, pID, TYPE_DO_PL);
        }
        break;
    }
  }
}
