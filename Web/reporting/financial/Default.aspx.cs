using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Common;

public partial class reporting_financial_Default : Scms.Web.Core.PageHandler
{
  private const string AP_FAKTUR_PENDING = "apfakturpending";
  private const string AP_GL = "apgl";
  private const string AP_LIST = "aplist";
  private const string AP_LIST_BAYAR = "aplistbayar";
  private const string AR_FAKTUR_PENDING = "arfakturpending";
  private const string AR_GL = "argl";
  private const string AR_LIST = "arlist";
  private const string AR_LIST_BAYAR = "arlistbayar";
  private const string HPP_DIV_AMS = "hppdivams";
  private const string HPP_DIV_AMS_EXT_DISC = "hppdivamsextdisc";
  private const string HPP_DIV_PRIN = "hppdivprins";
  private const string BEA_KIRIM = "beakirim";
  private const string BAYAR_JTEMPO = "jtempo";
  private const string BAYAR_BAYAR = "pembayaran";
  private const string CLAIM = "claim";
  private const string CLAIM_ACC = "claimacc";
  private const string EFAKTUR = "efaktur";
  private const string FAKTUR_MANUAL = "fakturmanual";


  protected void Page_Init(object sender, EventArgs e)
  {
    string qryString = null;

    if ((!this.IsPostBack) && (!Ext.Net.X.IsAjaxRequest))
    {
      qryString = (this.Request.QueryString.IsContainKey("mode") ? (this.Request.QueryString["mode"] ?? string.Empty).Trim() : string.Empty).ToLower();

      switch (qryString)
      {
        case AP_FAKTUR_PENDING:
          APFakturPending.InitializePage(wndDown.ClientID);
          APFakturPending.Visible = true;
          break;
        case AP_GL:
          APGL.InitializePage(wndDown.ClientID);
          APGL.Visible = true;
          break;
        case AP_LIST:
          APList.InitializePage(wndDown.ClientID);
          APList.Visible = true;
          break;
        case AP_LIST_BAYAR:
          APListBayar.InitializePage(wndDown.ClientID);
          APListBayar.Visible = true;
          break;
        case AR_FAKTUR_PENDING:
          ARFakturPending.InitializePage(wndDown.ClientID);
          ARFakturPending.Visible = true;
          break;
        case AR_GL:
          ARGL.InitializePage(wndDown.ClientID);
          ARGL.Visible = true;
          break;
        case AR_LIST:
          ARList.InitializePage(wndDown.ClientID);
          ARList.Visible = true;
          break;
        case AR_LIST_BAYAR:
          ARListBayar.InitializePage(wndDown.ClientID);
          ARListBayar.Visible = true;
          break;
        case HPP_DIV_AMS:
          HPPDivAMS.InitializePage(wndDown.ClientID);
          HPPDivAMS.Visible = true;
          break;
        case HPP_DIV_AMS_EXT_DISC:
          //HPPDivAMSExtDisc.InitializePage(wndDown.ClientID);
          //HPPDivAMSExtDisc.Visible = true;
          break;
        case HPP_DIV_PRIN:
          HPPDivPrinsipal.InitializePage(wndDown.ClientID);
          HPPDivPrinsipal.Visible = true;
          break;
        case BEA_KIRIM:
          BeaKirim.InitializePage(wndDown.ClientID);
          BeaKirim.Visible = true;
          break;
        case BAYAR_JTEMPO:
          JatuhTempo.InitializePage(wndDown.ClientID);
          JatuhTempo.Visible = true;
          break;
        case BAYAR_BAYAR:
          Pembayaran.InitializePage(wndDown.ClientID);
          Pembayaran.Visible = true;
          break;
        case CLAIM:
          Claim.InitializePage(wndDown.ClientID);
          Claim.Visible = true;
          break;
        case CLAIM_ACC:
          ClaimAcc.InitializePage(wndDown.ClientID);
          ClaimAcc.Visible = true;
          break;
        case EFAKTUR:
          Efaktur.InitializePage(wndDown.ClientID);
          Efaktur.Visible = true;
          break;
        case FAKTUR_MANUAL:
          FakturManual.InitializePage(wndDown.ClientID);
          FakturManual.Visible = true;
          break;
      }
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }
}
