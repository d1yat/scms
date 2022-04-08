using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_penyesuaian_AdjustSTTDonasiCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Adjustment STT Donasi";

    hfADJSTTDonasi.Clear();

    cbGudangHdrDonasi.Clear();
    cbGudangHdrDonasi.Disabled = false;
    Ext.Net.Store cbGudangHdrDonasistr = cbGudangHdrDonasi.GetStore();
    if (cbGudangHdrDonasistr != null)
    {
        cbGudangHdrDonasistr.RemoveAll();
    }

    txKeteranganHdrDonasi.Clear();
    txKeteranganHdrDonasi.Disabled = false;

    cbItemDtlDonasi.Clear();
    cbItemDtlDonasi.Disabled = false;
    Ext.Net.Store cbItemDtlDonasistr = cbItemDtlDonasi.GetStore();
    if (cbItemDtlDonasistr != null)
    {
        cbItemDtlDonasistr.RemoveAll();
    }

    cbTipeHdrDonasi.Clear();
    cbTipeHdrDonasi.Disabled = false;
    Ext.Net.Store cbTipeHdrDonasistr = cbTipeHdrDonasi.GetStore();
    if (cbTipeHdrDonasistr != null)
    {
        cbTipeHdrDonasistr.RemoveAll();
    }

    cbBatDtlDonasi.Clear();
    cbBatDtlDonasi.Disabled = false;
    Ext.Net.Store cbBatDtlDonasistr = cbBatDtlDonasi.GetStore();
    if (cbBatDtlDonasistr != null)
    {
        cbBatDtlDonasistr.RemoveAll();
    }

    cbNoRefDtlDonasi.Clear();
    cbNoRefDtlDonasi.Disabled = false;
    Ext.Net.Store cbNoRefDtlDonasistr = cbNoRefDtlDonasi.GetStore();
    if (cbNoRefDtlDonasistr != null)
    {
        cbNoRefDtlDonasistr.RemoveAll();
    }

    txQtyDtlDonasi.Clear();

    btnPrint.Hidden = true;

    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
    Functional.SetComboData(cbGudangHdrDonasi, "c_gdg", pag.ActiveGudangDescription, pag.ActiveGudang);
  }

  private void PopulateDetail(string pName, string pID, string type)
  {
    Dictionary<string, object> dicADJ = null;
    Dictionary<string, string> dicADJInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    //string tmp = null;

    #region Parser Header

    ClearEntrys();

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0097", paramX);

    try
    {
      dicADJ = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicADJ.ContainsKey("records") && (dicADJ.ContainsKey("totalRows") && (((long)dicADJ["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicADJ["records"]);

        dicADJInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        hfADJSTTDonasi.Text = pID;
        winDetail.Title = string.Concat("Adjustment STT Donasi - ", pID);

        Functional.SetComboData(cbGudangHdrDonasi, "c_gdg", dicADJInfo.GetValueParser<string>("v_gdgdesc", string.Empty), dicADJInfo.GetValueParser<string>("c_gdg", string.Empty));
        cbGudangHdrDonasi.Disabled = true;

        Functional.SetComboData(cbTipeHdrDonasi, "c_beban", dicADJInfo.GetValueParser<string>("v_jenis", string.Empty), dicADJInfo.GetValueParser<string>("c_beban", string.Empty));
        cbTipeHdrDonasi.Disabled = true;

        txKeteranganHdrDonasi.Text = (dicADJInfo.ContainsKey("v_ket") ? dicADJInfo["v_ket"] : string.Empty);

        btnPrint.Hidden = false;
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_penyesuaian_AdjustSTTDonasiCtrl:PopulateDetail Header - ", ex.Message));
    }
    finally
    {
      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicADJInfo != null)
      {
        dicADJInfo.Clear();
      }
      if (dicADJ != null)
      {
        dicADJ.Clear();
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

      hfADJSTTDonasi.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_penyesuaian_AdjustSTTDonasiCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    //cbNoRefDtlDonasi.Disabled = true;
    //cbItemDtlDonasi.Disabled = true;
    //cbBatDtlDonasi.Disabled = true;

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();

  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string adjNumber, string Type, string TypeDesc, string Gudang, string GudangDesc, string Keterangan, Dictionary<string, string>[] dics)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = adjNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string tmp = null,
      item = null,
      noRef = null,
      batch = null,
      ket = null;
    bool isVoid = false,
      isNew = false, isModify = false;
    string varData = null;

    decimal Qty = 0;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Gudang", Gudang);
    pair.DicAttributeValues.Add("Beban", Type);
    pair.DicAttributeValues.Add("Type", "01");
    pair.DicAttributeValues.Add("Keterangan", Keterangan);

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      isNew = dicData.GetValueParser<bool>("l_new");
      isVoid = dicData.GetValueParser<bool>("l_void");

      if (isNew && (!isVoid))
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        noRef = dicData.GetValueParser<string>("c_noref");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        Qty = dicData.GetValueParser<decimal>("n_qty", 0);


        if ((!string.IsNullOrEmpty(noRef)))
        {

          dicAttr.Add("NoRef", noRef);
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("Qty", Qty.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr
          });
        }
      }
      else if ((!isNew) && isVoid)
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        noRef = dicData.GetValueParser<string>("c_noref");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        Qty = dicData.GetValueParser<decimal>("n_qty", 0);

        if ((!string.IsNullOrEmpty(noRef)))
        {
          dicAttr.Add("NoRef", noRef);
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("Qty", Qty.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr,
            Value = (string.IsNullOrEmpty(Keterangan) ? "Human error" : Keterangan),
          });
        }
      }

      dicData.Clear();
    }
    try
    {
      varData = parser.ParserData("ADJSTTDONASI", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penyesuaian_AdjustSTTDonasiCtrl SaveParser : {0} ", ex.Message);
    }

    string result = null;

    if (!string.IsNullOrEmpty(varData))
    {
      Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

      result = soa.PostData(varData);

      responseResult = parser.ResponseParser(result);
    }

    return responseResult;
  }

  #endregion

  protected void Page_Load(object sender, EventArgs e)
  {
    
  }

  public void Initialize(string storeIDGridMain)
  {
    hfStoreIDDonasi.Text = storeIDGridMain;
    hfTypeDonasi.Text = "01";
  }

  public void CommandPopulate(bool isAdd, string pID, string gdgID, string Type)
  {    
    if (isAdd)
    {
      ClearEntrys();

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      PopulateDetail("c_adjno", pID, Type);
    }
  }
  
  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string typeCode = (e.ExtraParams["Type"] ?? string.Empty);
    string TypeDesc = (e.ExtraParams["TypeDesc"] ?? string.Empty);
    string Gudang = (e.ExtraParams["Gudang"] ?? string.Empty);
    string GudangDesc = (e.ExtraParams["GudangDesc"] ?? string.Empty);
    string Keterangan = (e.ExtraParams["Keterangan"] ?? string.Empty);
    string StoreID = (e.ExtraParams["StoreID"] ?? string.Empty);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    Dictionary<string, string>[] gridDataAdjTrans = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, typeCode, TypeDesc, Gudang, GudangDesc, Keterangan, gridDataAdjTrans);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null,
          dateJs = null;

        DateTime date = DateTime.Now;

        if (isAdd)
        {
          if (respon.Values != null)
          {
            if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
            {
              dateJs = Functional.DateToJson(date);
            }

            numberId = respon.Values.GetValueParser<string>("ADJ", string.Empty);

            if (!string.IsNullOrEmpty(StoreID))
            {
              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_gdg': '{6}',
                'v_gdgdesc': '{3}',
                'c_adjno': '{1}',
                'd_adjdate': {2},
                'v_jenis': '{4}',
                'v_ket': '{5}'
              }}));{0}.commitChanges();", StoreID,
                    numberId,
                    dateJs,
                    GudangDesc, TypeDesc, Keterangan, Gudang);

              X.AddScript(scrpt);
            }
          }
        }
        //this.ClearEntrys();
        if (!string.IsNullOrEmpty(numberId))
        {
          PopulateDetail("c_adjno", numberId, hfTypeDonasi.Text);
        }

        Functional.ShowMsgInformation("Data berhasil tersimpan.");
      }
      else
      {
        e.ErrorMessage = respon.Message;

        e.Success = false;
      }
    }
    else
    {
      e.ErrorMessage = "Unknown response";

      e.Success = false;
    }
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

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    if (!pag.IsAllowPrinting)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
      return;
    }

    string gdgId = pag.ActiveGudang;
    string numberID = (e.ExtraParams["NumberID"] ?? string.Empty);

    if (string.IsNullOrEmpty(numberID))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");

      return;
    }

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();

    rptParse.ReportingID = "10112";

    #region Report Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(char).FullName,
      ParameterName = "LG_AdjSTH.c_gdg",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_AdjSTH.c_adjno",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_AdjSTH.c_type",
      ParameterValue = "01"
    });

    #endregion

    rptParse.PaperID = "A4";
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
        //string rptName = string.Concat("Packing_List_", pag.Nip, ".", rptResult.Extension);

        //string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
        //tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
        //  tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

        string tmpUri = Functional.UriDownloadGenerator(pag,
          rptResult.OutputFile, "Adjustment STT (Donasi) ", rptResult.Extension);

        wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
      }
      else
      {
        Functional.ShowMsgWarning(rptResult.MessageResponse);
      }
    }

    GC.Collect();
  }
}
