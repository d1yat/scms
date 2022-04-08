using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_pengiriman_Ekspedisi : Scms.Web.Core.PageHandler
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Ekspedisi";

    DateTime date = DateTime.Now;

    cbCustomerHdr.Clear();
    cbCustomerHdr.Disabled = false;

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
    winDetail.Title = "";
    Dictionary<string, object> dicEXP = null;
    Dictionary<string, string> dicEXPInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
              new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
            };

    //string tmp = null;

    DateTime date = DateTime.MinValue;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0005", paramX);

    try
    {
      dicEXP = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicEXP.ContainsKey("records") && (dicEXP.ContainsKey("totalRows") && (((long)dicEXP["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicEXP["records"]);

        dicEXPInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Concat("Ekspedisi - ", pID);

        //cbByHdr.ToBuilder().AddItem(
        //    dicEXPInfo.ContainsKey("v_ketBy") ? dicEXPInfo["v_ketBy"] : string.Empty,
        //    dicEXPInfo.ContainsKey("c_typeby") ? dicEXPInfo["c_typeby"] : string.Empty
        //    );
        //if (cbByHdr.GetStore() != null)
        //{
        //  cbByHdr.GetStore().CommitChanges();
        //}
        //cbByHdr.SetValueAndFireSelect((dicEXPInfo.ContainsKey("c_typeby") ? dicEXPInfo["c_typeby"] : string.Empty));
        //cbByHdr.Disabled = true;
        Functional.SetComboData(cbViaHdr, "c_typeby", dicEXPInfo.GetValueParser<string>("v_ketBy", string.Empty), dicEXPInfo.GetValueParser<string>("c_typeby", string.Empty));
        cbViaHdr.Disabled = true;

        //cbCustomerHdr.ToBuilder().AddItem(
        //    dicEXPInfo.ContainsKey("v_cunam") ? dicEXPInfo["v_cunam"] : string.Empty,
        //    dicEXPInfo.ContainsKey("c_cusno") ? dicEXPInfo["c_cusno"] : string.Empty
        //    );
        //if (cbCustomerHdr.GetStore() != null)
        //{
        //  cbCustomerHdr.GetStore().CommitChanges();
        //}
        //cbCustomerHdr.SetValueAndFireSelect((dicEXPInfo.ContainsKey("c_cusno") ? dicEXPInfo["c_cusno"] : string.Empty));
        //cbCustomerHdr.Disabled = true;
        Functional.SetComboData(cbCustomerHdr, "c_cusno", dicEXPInfo.GetValueParser<string>("v_cunam", string.Empty), dicEXPInfo.GetValueParser<string>("c_cusno", string.Empty));
        cbCustomerHdr.Disabled = true;

        Functional.SetComboData(cbByHdr, "c_typeby", dicEXPInfo.GetValueParser<string>("v_by_desc", string.Empty), dicEXPInfo.GetValueParser<string>("c_typeby", string.Empty));
        cbByHdr.Disabled = true;

        //cbEksHdr.ToBuilder().AddItem(
        //    dicEXPInfo.ContainsKey("v_ketMsek") ? dicEXPInfo["v_ketMsek"] : string.Empty,
        //    dicEXPInfo.ContainsKey("c_exp") ? dicEXPInfo["c_exp"] : string.Empty
        //    );
        //if (cbEksHdr.GetStore() != null)
        //{
        //  cbEksHdr.GetStore().CommitChanges();
        //}
        //cbEksHdr.SetValueAndFireSelect((dicEXPInfo.ContainsKey("c_exp") ? dicEXPInfo["c_exp"] : string.Empty));
        Functional.SetComboData(cbEksHdr, "c_exp", dicEXPInfo.GetValueParser<string>("v_ketMsek", string.Empty), dicEXPInfo.GetValueParser<string>("c_exp", string.Empty));
        cbEksHdr.Disabled = true;
        
        //cbViaHdr.ToBuilder().AddItem(
        //    dicEXPInfo.ContainsKey("v_ketTran") ? dicEXPInfo["v_ketTran"] : string.Empty,
        //    dicEXPInfo.ContainsKey("c_via") ? dicEXPInfo["c_via"] : string.Empty
        //    );
        //if (cbViaHdr.GetStore() != null)
        //{
        //  cbViaHdr.GetStore().CommitChanges();
        //}
        //cbViaHdr.SetValueAndFireSelect((dicEXPInfo.ContainsKey("c_via") ? dicEXPInfo["c_via"] : string.Empty));
        //cbViaHdr.Disabled = true;
        Functional.SetComboData(cbViaHdr, "c_via", dicEXPInfo.GetValueParser<string>("v_ketTran", string.Empty), dicEXPInfo.GetValueParser<string>("c_via", string.Empty));
        cbViaHdr.Disabled = true;

        txKetHdr.Text = ((dicEXPInfo.ContainsKey("v_ket") ? dicEXPInfo["v_ket"] : string.Empty));

        txNoResiHdr.Text = ((dicEXPInfo.ContainsKey("c_resi") ? dicEXPInfo["c_resi"] : string.Empty));

        date = Functional.JsonDateToDate(dicEXPInfo.GetValueParser<string>("d_resi"));

        txDayResiHdr.Text = date.ToString("dd-MM-yyyy");
        txTimeResiHdr.Text = date.ToString("HH:mm:ss"); //dicEXPInfo.GetValueParser<string>("t_resi");

        //tmp = (dicEXPInfo.ContainsKey("d_resi") ? dicEXPInfo["d_resi"] : string.Empty);
        //tmp = tmp.Substring(tmp.IndexOf("(") + 1, tmp.IndexOf(")") - tmp.IndexOf("(") - 1);

        //string tmp1 = dicEXPInfo["t_resi"];

        //long e = long.Parse(tmp);

        //tmp = date.ToString();

        //txDayResiHdr.Text = ((tmp));
        //txTimeResiHdr.Text = ((tmp1));

        #region tanggal dan waktu
        //DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        //tmp = dicEXPInfo["d_resi"];
        //tmp = tmp.Substring(tmp.IndexOf("(") + 1, tmp.IndexOf(")") - tmp.IndexOf("(") - 1 );
        //DateTime de = new DateTime(Convert.ToInt64(tmp));
        //DateTime de1 = new DateTime().ToUniversalTime();

        //TimeSpan ts = new TimeSpan(de.Ticks - de1.Ticks);

        //CultureInfo provider = CultureInfo.InvariantCulture;
        //string format = "dd/MM/yyyy";

        //DateTime dt = DateTime.ParseExact(tmp, format, provider);

        //var utcDate = DateTime.Now.ToUniversalTime();
        //var Uda = Convert.ToInt64(tmp);
        //DateTime de = new DateTime(Convert.ToInt64(tmp)).ToLocalTime();
        //de.Subtract(UnicodeCategory)
        //long baseTicks = 621355968000000000;
        //long tickResolution = 10000000;
        //long epoch = (de.Ticks - baseTicks) / tickResolution;
        //long epochTicks = (epoch * tickResolution) + baseTicks;
        //string date = new DateTime(epochTicks, DateTimeKind.Utc).ToString();
        //string tbEpoch = epoch.ToString();
        //string tbDate1 = de.ToString();
        //string tbDate2 = date.ToString();


        //string date = new DateTime(epochTicks, DateTimeKind.Utc).ToString();

        #endregion

        txKoli.Text = dicEXPInfo.GetValueParser<decimal>("n_koli").ToString(); //((dicEXPInfo.ContainsKey("n_koli") ? dicEXPInfo["n_koli"] : string.Empty));

        txBerat.Text = dicEXPInfo.GetValueParser<decimal>("n_berat").ToString(); ////((dicEXPInfo.ContainsKey("n_berat") ? dicEXPInfo["n_berat"] : string.Empty));

        //X.AddScript(string.Format("clearRelatedComboRecursive(true, {0});", cbDODtl.ClientID));

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_sales_Expedisi:PopulateDetail Header - ", ex.Message));
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

      hfExpNo.Text = pID;
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
  
  #endregion

  [DirectMethod(ShowMask = true)]
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      hfGdg.Text = this.ActiveGudang;
    }
  }

  protected void GridMainCommand(object sender, DirectEventArgs e)
  {
    ClearEntrys();
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      PopulateDetail(pName, pID);

    }
    else
    {
      Functional.ShowMsgError("Perintah tidak dikenal.");
    }

    GC.Collect();
  }  
}