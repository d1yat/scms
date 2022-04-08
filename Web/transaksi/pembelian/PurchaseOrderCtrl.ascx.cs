using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_pembelian_PurchaseOrderCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Purchase Order";

    hfPoNo.Clear();

    lbGudangHdr.Text = string.Empty;
    lbSuplierHdr.Text = string.Empty;
    lbNomorORHdr.Text = string.Empty;
    lbTglPOHdr.Text = string.Empty;
    lbImport.Text = string.Empty;

    cbKursHdr.Clear();
    cbKursHdr.Disabled = false;
    txKursValue.Clear();
    txKursValue.Disabled = false;

    txKeteranganHdr.Clear();
    txKeteranganHdr.Disabled = false;

    lbBrutoBtm.Text = string.Empty;
    lbDiscountBtm.Text = string.Empty;

    txDiscPercNewBtm.Clear();
    txDiscPercNewBtm.MinValue = 0;
    txDiscPercNewBtm.MaxValue = 100;
    txDiscPercNewBtm.Disabled = false;

    lbXDiscPercBtm.Text = string.Empty;
    lbPPNBtm.Text = string.Empty;
    lbNetBtm.Text = string.Empty;
    lbImport.Text = string.Empty;

    btnPrint.Hidden = true;
    btnSend.Hidden = true;

    btnSave.Hidden = false;

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private void PopulateDetail(string pName, string pID)
  {
    ClearEntrys();

    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0017", paramX);
    bool isSend = false,
      isProcess = false;

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        isSend = dicResultInfo.GetValueParser<bool>("l_send");
        isProcess = dicResultInfo.GetValueParser<bool>("isProcess");

        txDiscPercNewBtm.Disabled = btnSave.Hidden = isProcess;

        winDetail.Title = string.Format("Purchase Order - {0}", pID);

        lbGudangHdr.Text = string.Format("{0} - {1}", dicResultInfo.GetValueParser<string>("c_gdg"), dicResultInfo.GetValueParser<string>("v_gdgdesc"));
        lbSuplierHdr.Text = string.Format("{0} - {1}", dicResultInfo.GetValueParser<string>("c_nosup"), dicResultInfo.GetValueParser<string>("v_nama"));
        txKeteranganHdr.Text = dicResultInfo.GetValueParser<string>("v_ket");
        
        lbNomorORHdr.Text = dicResultInfo.GetValueParser<string>("c_orno");
        lbTglPOHdr.Text = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_podate")).ToString("dd-MM-yyyy");
        lbImport.Text = (dicResultInfo.GetValueParser<bool>("l_import") ? "Ya" : "Tidak");
        
        //cbKursHdr.ToBuilder().AddItem(
        //  (dicResultInfo.ContainsKey("v_nama") ? dicSPInfo["v_nama"] : string.Empty),
        //  (dicResultInfo.ContainsKey("c_nosup") ? dicSPInfo["c_nosup"] : string.Empty)
        //);
        //if (cbKursHdr.GetStore() != null)
        //{
        //  cbKursHdr.GetStore().CommitChanges();
        //}
        //cbKursHdr.SetValueAndFireSelect((dicResultInfo.ContainsKey("c_nosup") ? dicResultInfo["c_nosup"] : string.Empty));

        Functional.SetComboData(cbKursHdr, "c_kurs", dicResultInfo.GetValueParser<string>("v_desc_kurs", string.Empty), dicResultInfo.GetValueParser<string>("c_kurs", string.Empty));

        txKursValue.Text = dicResultInfo.GetValueParser<decimal>("n_kurs", 0).ToString();

        lbBrutoBtm.Text = dicResultInfo.GetValueParser<decimal>("n_bruto").ToString("N2");
        lbDiscountBtm.Text = dicResultInfo.GetValueParser<decimal>("n_disc").ToString("N2");

        //txDiscPercNewBtm.SetRawValue(dicResultInfo.GetValueParser<decimal>("n_pdisc").ToString("N2"));
        txDiscPercNewBtm.SetRawValue(dicResultInfo.GetValueParser<decimal>("n_pdisc"));

        lbXDiscPercBtm.Text = dicResultInfo.GetValueParser<decimal>("n_xdisc").ToString("N2");

        lbPPNBtm.Text = dicResultInfo.GetValueParser<decimal>("n_ppn").ToString("N2");
        lbNetBtm.Text = dicResultInfo.GetValueParser<decimal>("n_bilva").ToString("N2");

        btnPrint.Hidden = false;
        btnSend.Hidden = false;

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_pembelian_PurchaseOrderCtrl:PopulateDetail Header - ", ex.Message));
    }
    finally
    {
      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicResultInfo != null)
      {
        dicResultInfo.Clear();
      }
      if (dicResult != null)
      {
        dicResult.Clear();
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
            store.BaseParams["parameters"] = string.Format("[['orno', '{1}', 'System.String']]", pName, pID);
          }
        }
      }

      hfPoNo.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_pembelian_PurchaseOrderCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string poNumber, Dictionary<string, string>[] dics, decimal xtraDisc, decimal xtraDiscOri)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = poNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string tmp = null,
      item = null,
      ket = null;
    decimal nDisc = 0,
      nPrice = 0;
    bool isVoid = false,
      isModify = false;
    string varData = null;
    int nCount = 0;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Gdg", "1");
    pair.DicAttributeValues.Add("XDisc", xtraDisc.ToString());
    pair.DicAttributeValues.Add("Keterangan", txKeteranganHdr.Text.Trim());

    if (cbKursHdr.SelectedItem != null)
    {
      tmp = cbKursHdr.SelectedItem.Value;
      pair.DicAttributeValues.Add("Kurs", tmp);

      decimal.TryParse(txKursValue.Text, out nPrice);
      pair.DicAttributeValues.Add("KursValue", nPrice.ToString());
    }

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      isVoid = dicData.GetValueParser<bool>("l_void");
      isModify = dicData.GetValueParser<bool>("l_modified");

      if (isVoid)
      {
        nCount++;

        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", "false");
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        ket = dicData.GetValueParser<string>("v_ket");

        if (!string.IsNullOrEmpty(item))
        {
          dicAttr.Add("Item", item);

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
            DicAttributeValues = dicAttr
          });
        }
      }
      else if (isModify)
      {
        nCount++;

        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", "false");
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        ket = dicData.GetValueParser<string>("v_ket");
        nDisc = dicData.GetValueParser<decimal>("n_disc");
        nPrice = dicData.GetValueParser<decimal>("n_salpri");

        if (!string.IsNullOrEmpty(item))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Disc", nDisc.ToString());
          dicAttr.Add("Salpri", nPrice.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
            DicAttributeValues = dicAttr
          });
        }
      }

      dicData.Clear();
    }

    //if ((nCount < 1) && (xtraDisc == xtraDiscOri))
    //{
    //  dic.Clear();

    //  return new PostDataParser.StructureResponse()
    //  {
    //    IsSet = true,
    //    Message = "Tidak ada data yang berubah.",
    //    Response = PostDataParser.ResponseStatus.Failed
    //  };
    //}

    try
    {
      varData = parser.ParserData("PurchaseOrder", "Modify", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_PurchaseOrderCtrl SaveParser : {0} ", ex.Message);
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

  private PostDataParser.StructureResponse SendParser(string poNumber)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = poNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Gdg", "1");
    pair.DicAttributeValues.Add("HasSend", "true");

    try
    {
      varData = parser.ParserData("PurchaseOrder", "SendProcess", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_PurchaseOrderCtrl SendParser : {0} ", ex.Message);
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

  public void Initialize()
  {
    ;
  }

  public void CommandPopulate(bool isAdd, string pID)
  {
    //if (isAdd)
    //{
    //  SetEditor(false);

    //  ClearEntrys();

    //  winDetail.Title = "Order Request Pemasok";

    //  winDetail.Hidden = false;
    //  winDetail.ShowModal();
    //}
    //else
    //{
    //  PopulateDetail("c_orno", pID);
    //}

    PopulateDetail("c_pono", pID);
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowEdit)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menyimpan data.");
      return;
    }

    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string strXDisc = (e.ExtraParams["ExtraDisc"] ?? string.Empty);
    string strXDiscOri = (e.ExtraParams["ExtraDiscOri"] ?? string.Empty);

    decimal xtraDisc = 0,
      xtraDiscOri = 0;

    decimal.TryParse(strXDisc, out xtraDisc);
    decimal.TryParse(strXDiscOri, out xtraDiscOri);

    Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    PostDataParser.StructureResponse respon = SaveParser(false, numberId, gridDataPL, xtraDisc, xtraDiscOri);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        this.ClearEntrys();

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

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    if (!pag.IsAllowPrinting)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
      return;
    }

    string gdgId = pag.ActiveGudang;
    //string custId = (e.ExtraParams["CustomeID"] ?? string.Empty);
    string numberID = (e.ExtraParams["NumberID"] ?? string.Empty);
    //string pl2 = (e.ExtraParams["PLID2"] ?? string.Empty);
    //string tmp = (e.ExtraParams["Async"] ?? string.Empty);

    if (string.IsNullOrEmpty(numberID))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");
      return;
    }

    ReportParser rptParse = new ReportParser();
    
    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();

    rptParse.ReportingID = "10106";

    #region Linq Filter Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "c_pono = @0",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID),
      IsLinqFilterParameter = true
    });

    #endregion

    #region Report Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "lg_vwPOCusno.c_pono",
      ParameterValue = numberID
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
        string tmpUri = Functional.UriDownloadGenerator(pag,
          rptResult.OutputFile, "Purchase Order ", rptResult.Extension);

        wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
      }
      else
      {
        Functional.ShowMsgWarning(rptResult.MessageResponse);
      }
    }

    GC.Collect();
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Mail_OnSend(object sender, DirectEventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowEdit)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menyimpan data.");
      return;
    }

    string poID = (e.ExtraParams["NumberID"] ?? string.Empty);

    PostDataParser.StructureResponse respon = SendParser(poID);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        Functional.ShowMsgInformation("Data berhasil terkirim.");
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
}