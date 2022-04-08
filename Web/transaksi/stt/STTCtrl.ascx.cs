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

public partial class transaksi_stt_STTCtrl : System.Web.UI.UserControl
{
  private const string DONASI_MODE = "01";

  #region Private 

  private void ClearEntrys()
  {
    hfSTTNo.Clear();

    if (hfTypeTrx.Text.Equals("02"))
    {
      winDetail.Title = "STT Sample";
    }
    else
    {
      winDetail.Title = "STT Donasi";
    }

    //cbGudangHdr.Clear();
    //cbGudangHdr.Disabled = false;
    lbGudang.Text = hfGudangDesc.Text;

    cbMemoHdr.Clear();
    cbMemoHdr.Disabled = false;

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

  private void PopulateDetail(string sttName, string sttID, string typeTransaksi)
  {
    ClearEntrys();

    Dictionary<string, object> dicSTT = null;
    Dictionary<string, string> dicSTTInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", sttName), sttID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0021", paramX);

    bool isDo = false;
    bool isPrint = false;
    
    try
    {
      dicSTT = JSON.Deserialize<Dictionary<string, object>>(res);

      if (dicSTT.ContainsKey("records") && (dicSTT.ContainsKey("totalRows") && (((long)dicSTT["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicSTT["records"]);

        dicSTTInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        if (typeTransaksi.Equals("02"))
        {
          winDetail.Title = string.Format("STT Sample - {0}", sttID);
        }
        else
        {
          winDetail.Title = string.Format("STT Donasi - {0}", sttID);
        }

        //cbGdg.SetValueAndFireSelect((dicSTTInfo.ContainsKey("v_gdgdesc") ? dicSTTInfo["v_gdgdesc"] : string.Empty));
        //cbGdg.Disabled = (pag.IsSpecialGroup ? false : true);

        //Functional.SetComboData(cbGudangHdr, "c_gdg", dicSTTInfo.GetValueParser<string>("v_gdgdesc", string.Empty), dicSTTInfo.GetValueParser<string>("c_gdg", string.Empty));
        //cbGudangHdr.Disabled = true;
        lbGudang.Text = dicSTTInfo.GetValueParser<string>("v_gdgdesc", string.Empty);

        Functional.SetComboData(cbMemoHdr, "c_mtno", dicSTTInfo.GetValueParser<string>("c_memo", string.Empty), dicSTTInfo.GetValueParser<string>("c_mtno", string.Empty));
        cbMemoHdr.Disabled = true;

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
        string.Concat("transaction_sales_PackingList:PopulateDetail Header - ", ex.Message));
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
            store.BaseParams.Add(new Ext.Net.Parameter("parameters", string.Format("[['{0} = @0', '{1}', 'System.String']]", sttName, sttID), ParameterMode.Raw));
          }
          else
          {
            store.BaseParams["parameters"] = string.Format("[['{0} = @0', '{1}', 'System.String']]", sttName, sttID);
          }
        }
      }

      hfSTTNo.Text = sttID;

      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_sales_STT:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string doNumberID, Dictionary<string, string>[] dics, string Mode)
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
    pair.DicAttributeValues.Add("Memo", cbMemoHdr.Text);
    pair.DicAttributeValues.Add("Type", hfTypeTrx.Text);
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
      varData = parser.ParserData("STT", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_stt_STTCtrl SaveParser : {0} ", ex.Message);
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
    hfTypeTrx.Text = (string.IsNullOrEmpty(typeTransaksi) ? DONASI_MODE : typeTransaksi);
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
      PopulateDetail("c_stno", pID, hfTypeTrx.Text);
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
    string typeCode = (e.ExtraParams["TypeCode"] ?? string.Empty);

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

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataDO, hfTypeTrx.Text);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;
        string storeId = hfStoreID.Text;

        string gdgDesc = (hfGudangDesc.Text != null ? hfGudangDesc.Text : string.Empty);
        string gdgId = (hfGudang.Text != null ? hfGudang.Text : string.Empty);
        string memoNo = (cbMemoHdr.SelectedItem != null ? cbMemoHdr.SelectedItem.Text : string.Empty);

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

            numberId = respon.Values.GetValueParser<string>("STT", string.Empty);

            if (!string.IsNullOrEmpty(storeId))
            {
              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_stno': '{1}',
                'd_stdate': {2},
                'v_gdgdesc': '{3}',
                'ket_ds': '{4}',
                'c_memo': '{5}',
                'c_gdg':'{6}'
              }}));{0}.commitChanges();", storeId, numberId,
                       dateJs, gdgDesc, txKeterangan.Text, memoNo, gdgId);

              X.AddScript(scrpt);
            }
          }
        }

        //this.ClearEntrys();
        if (!string.IsNullOrEmpty(numberId))
        {
          this.PopulateDetail("c_stno", numberId, typeCode);
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

    string gdgId = pag.ActiveGudang;
    string numberID = (e.ExtraParams["NumberID"] ?? string.Empty);
    string typeCode = (e.ExtraParams["TypeCode"] ?? string.Empty);

    if (string.IsNullOrEmpty(numberID))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");

      return;
    }

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();

    if (typeCode.Equals("01", StringComparison.OrdinalIgnoreCase))
    {
      rptParse.ReportingID = "10104";
    }
    else if (typeCode.Equals("02", StringComparison.OrdinalIgnoreCase))
    {
      rptParse.ReportingID = "10104-a";
    }

    #region Report Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(char).FullName,
      ParameterName = "LG_STH.c_gdg",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_STH.c_stno",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_STH.c_type",
      ParameterValue = (string.IsNullOrEmpty(typeCode) ? string.Empty : typeCode)
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
      DataType = typeof(char).FullName,
      ParameterName = "c_type = @0",
      ParameterValue = (string.IsNullOrEmpty(typeCode) ? string.Empty : typeCode),
      IsLinqFilterParameter = true
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "c_stno = @0",
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
        string rptName = null;

        if (typeCode.Equals("01", StringComparison.OrdinalIgnoreCase))
        {
          rptName = "Surat Tanda Terima - Donasi ";
        }
        else if (typeCode.Equals("02", StringComparison.OrdinalIgnoreCase))
        {
          rptName = "Surat Tanda Terima - Sample ";
        }

        //string rptName = string.Concat("Packing_List_", pag.Nip, ".", rptResult.Extension);

        //string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
        //tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
        //  tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

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
