using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_pm_PMCtrl : System.Web.UI.UserControl
{
  //private const string DONASI_MODE = "01";

  #region Private 

  private void ClearEntrys()
  {
    hfPMNo.Clear();

    winDetail.Title = "Pemusnahan";
    
    lbGudang.Text = hfGudangDesc.Text;

    txMemo.Clear();
    txMemo.Disabled = false;

    txKeterangan.Clear();
    txKeterangan.Disabled = false;

    cbBatDtl.Clear();
    cbBatDtl.Disabled = false;
    Ext.Net.Store cbBatDtlstr = cbBatDtl.GetStore();
    if (cbBatDtlstr != null)
    {
        cbBatDtlstr.RemoveAll();
    }

    cbItemDtl.Clear();
    cbItemDtl.Disabled = false;
    Ext.Net.Store cbItemDtlstr = cbItemDtl.GetStore();
    if (cbItemDtlstr != null)
    {
        cbItemDtlstr.RemoveAll();
    }

    txQtyDtl.Clear();
    txQtyDtl.Disabled = false;

    btnPrint.Hidden = true;

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private void PopulateDetail(string sttName, string pmID)
  {
    ClearEntrys();

    Dictionary<string, object> dicSTT = null;
    Dictionary<string, string> dicSTTInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", sttName), pmID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0320", paramX);

    bool isDo = false;
    bool isPrint = false;
    
    try
    {
      dicSTT = JSON.Deserialize<Dictionary<string, object>>(res);

      if (dicSTT.ContainsKey("records") && (dicSTT.ContainsKey("totalRows") && (((long)dicSTT["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicSTT["records"]);

        dicSTTInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("Pemusnahan - {0}", pmID);

        lbGudang.Text = dicSTTInfo.GetValueParser<string>("v_gdgdesc", string.Empty);
                  
        txMemo.Text = dicSTTInfo.GetValueParser<string>("c_memo", string.Empty);
        
        txKeterangan.Text = dicSTTInfo.GetValueParser<string>("v_ket", string.Empty);

        if ((isDo || isPrint) && (!pag.IsSpecialGroup))
        {
          //frmpnlDetailEntry.Disabled = true;
          cbItemDtl.Disabled = true;
          cbBatDtl.Disabled = true;
          txQtyDtl.Disabled = true;
        }

        X.AddScript(string.Format("clearRelatedComboRecursive(true, {0});", cbItemDtl.ClientID));

        jarr.Clear();

        btnPrint.Hidden = false;
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_pemusnahan_PM:PopulateDetail Header - ", ex.Message));
    }
    finally
    {
      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicSTTInfo != null)
      {
        dicSTTInfo.Clear();
      }
      if (dicSTT != null)
      {
        dicSTT.Clear();
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
            store.BaseParams.Add(new Ext.Net.Parameter("parameters", string.Format("[['{0} = @0', '{1}', 'System.String']]", sttName, pmID), ParameterMode.Raw));
          }
          else
          {
            store.BaseParams["parameters"] = string.Format("[['{0} = @0', '{1}', 'System.String']]", sttName, pmID);
          }
        }
      }

      hfPMNo.Text = pmID;

      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_pemusnahan_PM:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string doNumberID, Dictionary<string, string>[] dics)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = doNumberID;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string tmp = null,
      item = null, ket = null,
      batch = null;

    decimal nQty = 0;
    bool isNew = false,
      isVoid = false, isModify = false;
    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);

    //pair.DicAttributeValues.Add("From", pag.ActiveGudang);
    pair.DicAttributeValues.Add("Gudang", hfGudang.Text);
    pair.DicAttributeValues.Add("Memo", txMemo.Text);
    pair.DicAttributeValues.Add("Keterangan", txKeterangan.Text);

    //if (isConfirm)
    //{
    //  pair.DicAttributeValues.Add("Confirm", isConfimed.ToString().ToLower());
    //}
    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      isNew = dicData.GetValueParser<bool>("l_new");
      isVoid = dicData.GetValueParser<bool>("l_void");
      isModify = dicData.GetValueParser<bool>("l_modified");

      if (isNew && (!isVoid) && (!isModify))
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");

        nQty = dicData.GetValueParser<decimal>("n_gqty", 0);

        if ((!string.IsNullOrEmpty(item)) &&
          (nQty > 0))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("Qty", nQty.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr
          });
        }
      }
      else if ((!isNew) && isVoid && (!isModify))
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());
        
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        ket = dicData.GetValueParser<string>("v_ket");

        nQty = dicData.GetValueParser<decimal>("n_gqty", 0);

        if ((!string.IsNullOrEmpty(item)) &&
          (nQty > 0))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("Qty", nQty.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
            DicAttributeValues = dicAttr
          });
        }
      }

      #region Old Coded
      //else if ((!isNew) && (!isVoid) && (isModify))
      //{
      //  dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      //  dicAttr.Add("New", isNew.ToString().ToLower());
      //  dicAttr.Add("Delete", isVoid.ToString().ToLower());
      //  dicAttr.Add("Modified", isModify.ToString().ToLower());

      //  item = dicData.GetValueParser<string>("c_iteno");
      //  ket = dicData.GetValueParser<string>("v_ket");
      //  batch = dicData.GetValueParser<string>("c_batch");

      //  nQty = dicData.GetValueParser<decimal>("n_gqty", 0);

      //  if ((!string.IsNullOrEmpty(item)) &&
      //    (nQty > 0))
      //  {
      //    dicAttr.Add("Item", item);
      //    dicAttr.Add("Batch", batch);
      //    dicAttr.Add("Qty", nQty.ToString());

      //    pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
      //    {
      //      IsSet = true,
      //      Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
      //      DicAttributeValues = dicAttr
      //    });
      //  }
      //}
      #endregion

      dicData.Clear();
    }

    try
    {
        varData = parser.ParserData("Pemusnahan", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_pemusnahan_PMCtrl SaveParser : {0} ", ex.Message);
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

  public void Initialize(string gudang, string gudangDesc, string storeIDGridMain, string typeTransaksi)
  {
    //hfTypeTrx.Text = (string.IsNullOrEmpty(typeTransaksi) ? DONASI_MODE : typeTransaksi);
    hfGudang.Text = gudang;
    hfGudangDesc.Text = gudangDesc;
    hfStoreID.Text = storeIDGridMain;
  }

  public void CommandPopulate(bool isAdd, string pID)
  {
    if (isAdd)
    {
      ClearEntrys();

      winDetail.Hidden = false;
    }
    else
    {
      PopulateDetail("c_pmno", pID);
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jSonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
   
    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    if (isAdd)
    {
      if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
      {
        Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.") ;
        return;
      }
    }
    else if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowEdit)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menyimpan data.");
      return;
    }
    
    Dictionary<string, string>[] gridDataDO = JSON.Deserialize<Dictionary<string, string>[]>(jSonGridValues);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataDO);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;
        string storeId = hfStoreID.Text;

        string gdgDesc = (hfGudangDesc.Text != null ? hfGudangDesc.Text : string.Empty);
        string gdgId = (hfGudang.Text != null ? hfGudang.Text : string.Empty);

        string dateJs = null;
        DateTime date = DateTime.Today;

        if (isAdd)
        {
          if (respon.Values != null)
          {
            if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
            {
              dateJs = Functional.DateToJson(date);
            }

            numberId = respon.Values.GetValueParser<string>("PM", string.Empty);

            if (!string.IsNullOrEmpty(storeId))
            {
              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_pmno': '{1}',
                'd_pmdate': {2},
                'v_gdgdesc': '{3}',
                'v_ket': '{4}',
                'c_memo': '{5}',
                'c_gdg':'{6}'
              }}));{0}.commitChanges();", storeId, numberId,
                       dateJs, gdgDesc, txKeterangan.Text, txMemo.Text, gdgId);

              X.AddScript(scrpt);
            }
          }
        }

        //this.ClearEntrys();
        if (!string.IsNullOrEmpty(numberId))
        {
          this.PopulateDetail("c_pmno", numberId);
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

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    if (!pag.IsAllowPrinting)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
      return;
    }

    string gdgId = "5";
    string numberID = (e.ExtraParams["NumberID"] ?? string.Empty);

    if (string.IsNullOrEmpty(numberID))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");

      return;
    }

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();
    rptParse.ReportingID = "10114";

    #region Report Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(char).FullName,
      ParameterName = "LG_PMH.c_gdg",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_PMH.c_pmno",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID)
    });

    #endregion

    #region Linq Filter Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(char).FullName,
      ParameterName = "c_gdg = @0",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId),
      IsLinqFilterParameter = true
    });
    
    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "c_pmno = @0",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID),
      IsLinqFilterParameter = true
    });

    #endregion

    rptParse.PaperID = "8.5x5.5";
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;

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
        string rptName = "Pemusnahan Barang";

        string tmpUri = Functional.UriDownloadGenerator(pag,
          rptResult.OutputFile, rptName, rptResult.Extension);

        wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
      }
      else
      {
        Functional.ShowMsgWarning(rptResult.MessageResponse);
      }
    }

    GC.Collect();
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
