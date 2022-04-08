//Created By Indra Monitoring Process 20180523FM

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_wp_MonitoringPLEkspedisi : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Surat Pesanan";    

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private void PopulateDetail(string pName, string pID)
  {
    ClearEntrys();

    Dictionary<string, object> dicEP = null;
    Dictionary<string, string> dicEPInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string tmp = null;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0380-a", paramX);

    try
    {
      dicEP = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicEP.ContainsKey("records") && (dicEP.ContainsKey("totalRows") && (((long)dicEP["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicEP["records"]);

        dicEPInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("No.EP - {0}", pID);

        txGudang.Text           = dicEPInfo.GetValueParser<string>("v_gdgdesc", string.Empty);

        tmp                     = dicEPInfo.GetValueParser<string>("d_expdate", string.Empty);
        DateTime date1          = Functional.JsonDateToDate(tmp);
        txTglEkspedisi.Text     = date1.ToString("dd-MM-yyyy");       

        txNamaEkspedisi.Text    = dicEPInfo.GetValueParser<string>("v_ket", string.Empty);

        txCabang.Text           = dicEPInfo.GetValueParser<string>("c_cab_dcore", string.Empty);       

        txNoResi.Text           = dicEPInfo.GetValueParser<string>("c_resi", string.Empty);

        tmp                     = dicEPInfo.GetValueParser<string>("d_resi", string.Empty);
        DateTime date2          = Functional.JsonDateToDate(tmp);
        txTglResi.Text          = date2.ToString("dd-MM-yyyy");

        txKoli.Text             = dicEPInfo.GetValueParser<string>("n_koli", string.Empty);

        txBerat.Text            = dicEPInfo.GetValueParser<string>("n_berat", string.Empty);

        txVolume.Text           = dicEPInfo.GetValueParser<string>("n_vol", string.Empty);

        txReceh.Text            = dicEPInfo.GetValueParser<string>("n_receh", string.Empty);
  
        jarr.Clear();
       
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_pembelian_SuratPesananCtrl:PopulateDetail Header - ", ex.Message));
    }
    finally
    {

      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicEPInfo != null)
      {
          dicEPInfo.Clear();
      }
      if (dicEP != null)
      {
        dicEP.Clear();
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

      hfEPNo.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_pembelian_SuratPesananCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  #endregion

  public void Initialize(string storeIDGridMain, string typeName)
  {
    hfStoreID.Text = storeIDGridMain;
    hfTypeNameCtrl.Text = typeName;
  }

  public void CommandPopulate(bool isAdd, string pID)
  {
    if (isAdd)
    {
      ClearEntrys();

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      PopulateDetail("c_expno", pID);
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    if (!pag.IsAllowPrinting)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
      return;
    }

    string custId = (e.ExtraParams["CustomeID"] ?? string.Empty);
    string numberID = (e.ExtraParams["NumberID"] ?? string.Empty);

    if (string.IsNullOrEmpty(custId) && string.IsNullOrEmpty(numberID))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");
      return;
    }

    ReportParser rptParse = new ReportParser();
    
    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();

    rptParse.ReportingID = "10113";
    
    #region Report Parameter
    
    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_SPH.c_cusno",
      ParameterValue = (string.IsNullOrEmpty(custId) ? string.Empty : custId)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_SPH.c_spno",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID)
    });

    #endregion

    #region Linq Filter Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(char).FullName,
      ParameterName = "c_cusno = @0",
      ParameterValue = (string.IsNullOrEmpty(custId) ? string.Empty : custId),
      IsLinqFilterParameter = true
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "c_spno = @0",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID),
      IsLinqFilterParameter = true
    });

    #endregion

    rptParse.PaperID = "Letter";
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;
    rptParse.UserDefinedName = numberID;

    string xmlFiles = ReportParser.Deserialize(rptParse);

    SoaReportCaller soa = new SoaReportCaller();

    string result = soa.GeneratorReport(xmlFiles);

    ReportingResult rptResult = ReportingResult.Serialize(result);

    if (rptResult == null)
    {
      Functional.ShowMsgError("Pembuatan report gagal.");
    }
    else
    {
      if (rptResult.IsSuccess)
      {
        string tmpUri = Functional.UriDownloadGenerator(pag,
          rptResult.OutputFile, "Surat Pesanan", rptResult.Extension);

        wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
      }
      else
      {
        Functional.ShowMsgWarning(rptResult.MessageResponse);
      }
    }

    GC.Collect();
  }

  protected void OnSelectGrid(object sender, DirectEventArgs e)
  {
      string pName = (e.ExtraParams["c_noTrans"] ?? string.Empty);
      string tmp = null;
      Ext.Net.Store store = GridDetail2.GetStore();

      if (pName.StartsWith("DO"))
      {
          tmp = string.Format(@"var xOpts = {{
                                  params: {{
                                      start: '0',
                                      limit: '10',
                                      allQuery: 'true',
                                      model: '2162',
                                      sort: '',
                                      dir: '',
                                      parameters: [['c_no', '{0}', 'System.String']]
                                    }}
                                  }};", pName.ToString());
      }
      else
      {
          tmp = string.Format(@"var xOpts = {{
                                  params: {{
                                      start: '0',
                                      limit: '10',
                                      allQuery: 'true',
                                      model: '2161',
                                      sort: '',
                                      dir: '',
                                      parameters: [['c_no', '{0}', 'System.String']]
                                    }}
                                  }};", pName.ToString());
      }
      X.AddScript(tmp);
      X.AddScript(string.Format("{0}.removeAll();{0}.reload(xOpts);", Sx.ClientID));

      GC.Collect();
  }

}