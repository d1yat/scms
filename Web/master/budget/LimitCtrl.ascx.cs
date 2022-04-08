using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;


public partial class master_budget_LimitCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Batas Anggaran";

    Ext.Net.Store store = gridHeaderList.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }

    store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private void PopulateDetail(string pName, string pID, string idName)
  {
    ClearEntrys();

    winDetail.Title = string.Concat("Batas Anggaran - ", idName);

    #region Parser Detail

    try
    {
      Ext.Net.Store store = gridHeaderList.GetStore();
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

      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("master_budget_LimitCtrl:PopulateDetail HeaderList - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  #endregion

  public void CommandPopulate(string pID, string pIdName)
  {
    hfSuplier.Text = pID;
    hfSuplierName.Text = pIdName;
    PopulateDetail("c_nosup", pID, pIdName);
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      LimitEntryCtrl1.Initialize(storeHdrList.ClientID, storeDtl.ClientID);
    }
  }

  //protected void ResetBtn_Click(object sender, DirectEventArgs e)
  //{
  //}

  protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler page = this.Page as Scms.Web.Core.PageHandler;

    if (!page.IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);
    string pIdName = (e.ExtraParams["PrimaryNameID"] ?? string.Empty);

    //master_budget_LimitEntryCtrl lec = (master_budget_LimitEntryCtrl)Functional.LoadDynamicControl(this.Page, "LimitEntryCtrl.ascx", "LimitEntryCtrl1", pnlDynCtrl);

    //lec.CommandPopulate(true, pID, pIdName, 0, 0);

    LimitEntryCtrl1.CommandPopulate(true, pID, pIdName, 0, 0, 0, 0);

    //X.Js.Call("destroyCtrlFromCache", new JRawValue(Panel1.ClientID));
    //BaseUserControl uc1 = (BaseUserControl)this.LoadControl("UserControl1.ascx");
    //uc1.ID = "UC1";
    //this.Panel1.ContentControls.Add(uc1);

    //X.Js.Call("putToCache", new JRawValue(Panel1.ClientID), uc1.ControlsToDestroy);
    //this.Panel1.UpdateContent();

    //this.Button1.Disabled = true;
    //this.Button2.Disabled = false;
  }

  protected void OnGridCommand_Click(object sender, DirectEventArgs e)
  {
    int thn,
      bln;
    decimal lmt,
      persen;

    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string suplId = (e.ExtraParams["SuplierID"] ?? string.Empty);
    string suplName = (e.ExtraParams["SuplierName"] ?? string.Empty);

    string tmp = (e.ExtraParams["Tahun"] ?? string.Empty);
    int.TryParse(tmp, out thn);

    tmp = (e.ExtraParams["Bulan"] ?? string.Empty);
    int.TryParse(tmp, out bln);

    tmp = (e.ExtraParams["Limit"] ?? string.Empty);
    decimal.TryParse(tmp, out lmt);

    tmp = (e.ExtraParams["Persentase"] ?? string.Empty);
    decimal.TryParse(tmp, out persen);

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      LimitEntryCtrl1.CommandPopulate(false, suplId, suplName, thn, bln, lmt, persen);
    }
  }
}
