using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_pengiriman_EkspedisiGudangCtrl : System.Web.UI.UserControl
{
  private void ClearEntrys()
  {
    winDetail.Title = "Ekspedisi";

    DateTime date = DateTime.Now;

    hfExpNo.Clear();

    cbGudangHdr.Clear();
    cbGudangHdr.Disabled = false;

    lbGudangFrom.Text = hfGudangDesc.Text;

    cbByHdr.Clear();
    cbByHdr.Disabled = false;

    cbEksHdr.Clear();
    cbEksHdr.Disabled = true;

    cbViaHdr.Clear();
    cbViaHdr.Disabled = false;

    txNoResiHdr.Clear();
    txNoResiHdr.Disabled = false;

    txDayResiHdr.Clear();
    txDayResiHdr.Disabled = false;

    txBerat.Clear();
    txBerat.Disabled = false;

    txDayResiHdr.Clear();
    txDayResiHdr.Text = date.ToString("d-M-yyyy");
    txDayResiHdr.Disabled = false;

    txTimeResiHdr.Clear();
    txTimeResiHdr.Text = date.ToString("HH:mm:ss");
    txTimeResiHdr.Disabled = false;

    txKoli.Clear();
    txKoli.Disabled = false;

    txKetHdr.Clear();
    txKetHdr.Disabled = false;

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private void PopulateDetail(string pName, string pID)
  {
    ClearEntrys();
    
    Dictionary<string, object> dicEXP = null;
    Dictionary<string, string> dicEXPInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    DateTime date = DateTime.MinValue;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0211", paramX);

    try
    {
      dicEXP = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicEXP.ContainsKey("records") && (dicEXP.ContainsKey("totalRows") && (((long)dicEXP["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicEXP["records"]);

        dicEXPInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Concat("Ekspedisi - ", pID);

        Functional.SetComboData(cbViaHdr, "c_typeby", dicEXPInfo.GetValueParser<string>("v_ketBy", string.Empty), dicEXPInfo.GetValueParser<string>("c_typeby", string.Empty));
        cbViaHdr.Disabled = true;

        Functional.SetComboData(cbGudangHdr, "c_cusno", dicEXPInfo.GetValueParser<string>("v_cunam", string.Empty), dicEXPInfo.GetValueParser<string>("c_cusno", string.Empty));
        cbGudangHdr.Disabled = true;

        Functional.SetComboData(cbByHdr, "c_typeby", dicEXPInfo.GetValueParser<string>("v_by_desc", string.Empty), dicEXPInfo.GetValueParser<string>("c_typeby", string.Empty));
        cbByHdr.Disabled = true;

        Functional.SetComboData(cbEksHdr, "c_exp", dicEXPInfo.GetValueParser<string>("v_ketMsek", string.Empty), dicEXPInfo.GetValueParser<string>("c_exp", string.Empty));
        cbEksHdr.Disabled = true;

        Functional.SetComboData(cbViaHdr, "c_via", dicEXPInfo.GetValueParser<string>("v_ketTran", string.Empty), dicEXPInfo.GetValueParser<string>("c_via", string.Empty));
        cbViaHdr.Disabled = true;

        txKetHdr.Text = ((dicEXPInfo.ContainsKey("v_ket") ? dicEXPInfo["v_ket"] : string.Empty));

        txNoResiHdr.Text = ((dicEXPInfo.ContainsKey("c_resi") ? dicEXPInfo["c_resi"] : string.Empty));

        date = Functional.JsonDateToDate(dicEXPInfo.GetValueParser<string>("d_resi"));

        txDayResiHdr.Text = date.ToString("dd-MM-yyyy");
        txTimeResiHdr.Text = date.ToString("HH:mm:ss"); 

        txKoli.Text = dicEXPInfo.GetValueParser<decimal>("n_koli").ToString(); //((dicEXPInfo.ContainsKey("n_koli") ? dicEXPInfo["n_koli"] : string.Empty));

        txBerat.Text = dicEXPInfo.GetValueParser<decimal>("n_berat").ToString(); ////((dicEXPInfo.ContainsKey("n_berat") ? dicEXPInfo["n_berat"] : string.Empty));


        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_pengiriman_EkspedisiGudangCtrl:PopulateDetail Header - ", ex.Message));
    }
    finally
    {
      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicEXPInfo != null)
      {
        dicEXPInfo.Clear();
      }
      if (dicEXP != null)
      {
        dicEXP.Clear();
      }
    }


    #endregion

    #region Parser Detail

    try
    {
      Ext.Net.Store store = gridDetail.GetStore();
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

      hfExpNo.Value = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_sales_Expedisi:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  public void CommandPopulate(bool isAdd, string pID)
  {
    //bool isConfirm = false;

    //bool.TryParse(hfConfMode.Text, out isConfirm);

    if (isAdd)
    {
      //frmHeaders.Height = new Unit(175);
      //chkConfirm.Hidden = true;

      ClearEntrys();

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      PopulateDetail("c_expno", pID);
    }
  }

  public void Initialize(string gudang, string gudangDesc, string storeIDGridMain)
  {
    hfGudang.Text = gudang;
    hfStoreID.Text = storeIDGridMain;
    hfGudangDesc.Text = gudangDesc;
  }

  protected void ReloadBtn_Click(object sender, DirectEventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    this.ClearEntrys();
  }
}
