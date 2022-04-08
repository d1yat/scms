using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_memo_MemoSTT : Scms.Web.Core.PageHandler
{
  private const string ADJ_DONASI = "ADJDONASI";
  private const string ADJ_SAMPLE = "ADJSAMPLE";

  private const string TYPE_ADJ_DONASI = "01";
  private const string TYPE_ADJ_SAMPLE = "02";

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  protected void Page_Init(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

      if (qryString.Equals("sample", StringComparison.OrdinalIgnoreCase))
      {
        hfMode.Text = ADJ_SAMPLE;
        hfType.Text = TYPE_ADJ_SAMPLE;
      }
      else
      {
        hfMode.Text = ADJ_DONASI;
        hfType.Text = TYPE_ADJ_DONASI;
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
      case TYPE_ADJ_SAMPLE:
        {
          MemoSTTSampleCtrl.CommandPopulate(true, null, null, TYPE_ADJ_SAMPLE);
        }
        break;
      default:
        {
          MemoSTTDonasiCtrl.CommandPopulate(true, null, null, TYPE_ADJ_DONASI);
        }
        break;
    }
  }
}
