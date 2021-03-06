using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_pembelian_PO_ApotekerCtrl : System.Web.UI.UserControl
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

    btnSave.Hidden = false;

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private void PopulateDetail(string pName, string pID, string typePO)
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

        winDetail.Title = string.Format("Purchase Order - {0}", pID);

        lbGudangHdr.Text = string.Format("{0} - {1}", dicResultInfo.GetValueParser<string>("c_gdg"), dicResultInfo.GetValueParser<string>("v_gdgdesc"));
        lbSuplierHdr.Text = string.Format("{0} - {1}", dicResultInfo.GetValueParser<string>("c_nosup"), dicResultInfo.GetValueParser<string>("v_nama"));
        txKeteranganHdr.Text = dicResultInfo.GetValueParser<string>("v_ket");
        txKeteranganHdr.Disabled = true;
        
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
        cbKursHdr.Disabled = true;

        txKursValue.Text = dicResultInfo.GetValueParser<decimal>("n_kurs", 0).ToString();
        txKursValue.Disabled = true;

        lbBrutoBtm.Text = dicResultInfo.GetValueParser<decimal>("n_bruto").ToString("N2");
        lbDiscountBtm.Text = dicResultInfo.GetValueParser<decimal>("n_disc").ToString("N2");

        //txDiscPercNewBtm.SetRawValue(dicResultInfo.GetValueParser<decimal>("n_pdisc").ToString("N2"));
        txDiscPercNewBtm.SetRawValue(dicResultInfo.GetValueParser<decimal>("n_pdisc"));
        txDiscPercNewBtm.Disabled = true;

        lbXDiscPercBtm.Text = dicResultInfo.GetValueParser<decimal>("n_xdisc").ToString("N2");

        lbPPNBtm.Text = dicResultInfo.GetValueParser<decimal>("n_ppn").ToString("N2");
        lbNetBtm.Text = dicResultInfo.GetValueParser<decimal>("n_bilva").ToString("N2");

        chkConfirm.Checked = false;

        btnPrint.Hidden = false;

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
      hfTypePO.Text = typePO;
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

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string poNumber, bool isConfimed, Dictionary<string, string>[] dics, decimal xtraDisc, decimal xtraDiscOri)
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
    pair.DicAttributeValues.Add("isConfirm", isConfimed.ToString().ToLower());
    pair.DicAttributeValues.Add("typeApoteker", hfTypePO.Text);

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

    try
    {
      varData = parser.ParserData("PurchaseOrderApoteker", "Modify", dic);
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
  
  #endregion

  public void Initialize(string storeIDGridMain, string typePO)
  {
      hfStoreID.Text = storeIDGridMain;
      hfTypePO.Text = typePO;
  }

  public void CommandPopulate(bool isAdd, string pID, string typePO)
  {
      PopulateDetail("c_pono", pID, typePO);
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
    string tipePO = (e.ExtraParams["typePO"] ?? string.Empty);
    string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);

    decimal xtraDisc = 0,
      xtraDiscOri = 0;

    decimal.TryParse(strXDisc, out xtraDisc);
    decimal.TryParse(strXDiscOri, out xtraDiscOri);

    bool isConfimed = chkConfirm.Checked;

    Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    PostDataParser.StructureResponse respon = SaveParser(false, numberId, isConfimed, gridDataPL, xtraDisc, xtraDiscOri);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
          PopulateDetail("c_pono", numberId, tipePO);

          numberId = respon.Values.GetValueParser<string>("pono", string.Empty);
          string poktnoParse = respon.Values.GetValueParser<string>("poktno", string.Empty);

          if (!string.IsNullOrEmpty(storeId))
          {
              string sd = string.Format(@"var c_pono = {0}.findExact('c_pono', '{1}');
                                if(c_pono != -1) {{
                                  var r = {0}.getAt(c_pono);
                                  r.set('c_poktno', '{2}');
                                  r.set('l_status_pok', 'true');
                                  {0}.commitChanges();
                                }}", storeId,
                                   numberId,
                                   poktnoParse);

              X.AddScript(sd);
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
    List<ReportCustomizeText> lstCustTxt = new List<ReportCustomizeText>();

    List<string> lstData = new List<string>();

    rptParse.ReportingID = "10202";

    #region Report Parameter



    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "LG_POH.c_gdg",
        ParameterValue = gdgId
    });

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "LG_POH.c_pono",
        ParameterValue = numberID
    });

    #region SQL Parameter

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "tipereport",
        ParameterValue = hfTypePO.Text,
        IsSqlParameter = true
    });

    #endregion

    string reportTitle = null;
    switch (hfTypePO.Text)
    {
        case "02": reportTitle = "SURAT PESANAN PSIKOTROPIKA"; break;
        case "07": reportTitle = "SURAT PESANAN PREKURSOR"; break;
        case "09": reportTitle = "SURAT PESANAN OBAT-OBAT TERTENTU"; break;

    }


    lstCustTxt.Add(new ReportCustomizeText()
    {
        SectionName = "Section2",
        ControlName = "txTitle",
        Value = reportTitle
    });

    #endregion

    rptParse.PaperID = "Letter";
    rptParse.ReportCustomizeText = lstCustTxt.ToArray();
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