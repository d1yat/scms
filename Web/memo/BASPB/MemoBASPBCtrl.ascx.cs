using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_memo_BASPBCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Surat Pesanan";

    hfBASPNo.Clear();

    cbGudangFromHdr.Clear();
    Ext.Net.Store cbGudangFromHdrStr = cbGudangFromHdr.GetStore();
    if (cbGudangFromHdrStr != null)
    {
        cbGudangFromHdrStr.RemoveAll();
    }
    cbGudangFromHdr.Disabled = false;

    cbGudangToHdr.Clear();
    Ext.Net.Store cbGudangToHdrStr = cbGudangToHdr.GetStore();
    if (cbGudangToHdrStr != null)
    {
        cbGudangToHdrStr.RemoveAll();
    }
    cbGudangToHdr.Disabled = false;

    cbSJHdr.Clear();
    Ext.Net.Store cbSJHdrStr = cbSJHdr.GetStore();
    if (cbSJHdrStr != null)
    {
        cbSJHdrStr.RemoveAll();
    }
    cbSJHdr.Disabled = false;

    cbItemDtl.Clear();
    Ext.Net.Store cbItemDtlStr = cbItemDtl.GetStore();
    if (cbItemDtlStr != null)
    {
        cbItemDtlStr.RemoveAll();
    }
    cbItemDtl.Disabled = false;

    cbTipeDtl.Clear();
    Ext.Net.Store cbTipeDtlStr = cbTipeDtl.GetStore();
    if (cbTipeDtlStr != null)
    {
        cbTipeDtlStr.RemoveAll();
    }
    cbTipeDtl.Disabled = false;

    txKeterangan.Clear();
    txKeterangan.Disabled = false;

    txQtyDtl.Clear();
    txQtyDtl.Disabled = false;

    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));

    btnAdd.Disabled = false;
    btnClear.Disabled = false;

    btnPrint.Hidden = true;

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

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "05004", paramX);

    try
    {
        dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
        if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
          jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("Memo BASPB - {0}", pID);

        Functional.SetComboData(cbGudangFromHdr, "c_cusno", dicResultInfo.GetValueParser<string>("v_gdgdesc_asal", string.Empty), dicResultInfo.GetValueParser<string>("c_cusno", string.Empty));
        cbGudangFromHdr.Disabled = true;

        Functional.SetComboData(cbGudangToHdr, "c_gdg", dicResultInfo.GetValueParser<string>("v_gdgdesc_tujuan", string.Empty), dicResultInfo.GetValueParser<string>("c_gdg", string.Empty), true);
        cbGudangToHdr.Disabled = true;


        Functional.SetComboData(cbSJHdr, "c_dono", dicResultInfo.GetValueParser<string>("c_dono", string.Empty), dicResultInfo.GetValueParser<string>("c_dono", string.Empty), true);
        cbSJHdr.Disabled = true;

        txKeterangan.Text = dicResultInfo.GetValueParser<string>("v_ket", string.Empty);
        
        jarr.Clear();

        btnPrint.Hidden = false;
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_pembelian_MemoBASPBCtrl:PopulateDetail Header - ", ex.Message));
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
            store.BaseParams["parameters"] = string.Format("[['{0} = @0', '{1}', 'System.String']]", pName, pID);
          }
        }
      }

      hfBASPNo.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_pembelian_MemoBASPBCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string plNumber, Dictionary<string, string>[] dics)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = plNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string tmp = null,
      item = null,
      ket = null,
      claimType = null;
    decimal nQty = 0,
        nQtyDO = 0,
        nQtyDiff = 0;
    bool isNew = false,
      isVoid = false,
      isModify = false;
    string varData = null;

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", pag.Nip);
    pair.DicAttributeValues.Add("gdgasal", cbGudangFromHdr.SelectedItem.Value);
    pair.DicAttributeValues.Add("gdgtujuan", cbGudangToHdr.SelectedItem.Value);
    pair.DicAttributeValues.Add("sjno", cbSJHdr.SelectedItem.Value);
    pair.DicAttributeValues.Add("Keterangan", txKeterangan.Text.Trim());

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      isNew = dicData.GetValueParser<bool>("l_new");
      isVoid = dicData.GetValueParser<bool>("l_void");
      isModify = dicData.GetValueParser<bool>("l_modified");

      if (isNew && (!isVoid) && (!isModify))
      {
        if(!pag.IsAllowAdd)
        {
          continue;
        }

        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        nQty = dicData.GetValueParser<decimal>("n_gqty", 0);
        nQtyDO = dicData.GetValueParser<decimal>("n_qtydo", 0);
        nQtyDiff = dicData.GetValueParser<decimal>("n_qtydiff", 0);
        claimType = dicData.GetValueParser<string>("c_claimtype");

        if (!string.IsNullOrEmpty(item))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Qty", nQty.ToString());
          dicAttr.Add("QtyDo", nQty.ToString());
          dicAttr.Add("QtyDiff", nQty.ToString());
          dicAttr.Add("claimType", claimType.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? string.Empty : ket.Trim()),
            DicAttributeValues = dicAttr
          });
        }
      }
      else if ((!isNew) && isVoid && (!isModify))
      {
        if(!pag.IsAllowDelete)
        {
          continue;
        }

        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
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
      else if ((!isNew) && (!isVoid) && isModify)
      {
        if(!pag.IsAllowEdit)
        {
          continue;
        }

        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());


        item = dicData.GetValueParser<string>("c_iteno");
        nQty = dicData.GetValueParser<decimal>("n_gqty", 0);
        nQtyDO = dicData.GetValueParser<decimal>("n_qtydo", 0);
        nQtyDiff = dicData.GetValueParser<decimal>("n_qtydiff", 0);
        claimType = dicData.GetValueParser<string>("c_claimtype");

        if (!string.IsNullOrEmpty(item))
        {
            dicAttr.Add("Item", item);
            dicAttr.Add("Qty", nQty.ToString());
            dicAttr.Add("QtyDo", nQty.ToString());
            dicAttr.Add("QtyDiff", nQty.ToString());
            dicAttr.Add("claimType", claimType.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Modify" : ket),
            DicAttributeValues = dicAttr
          });
        }
      }
      else if ((!isNew) && isVoid && (!isModify))
      {
          if (!pag.IsAllowDelete)
          {
              continue;
          }

          dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

          dicAttr.Add("New", isNew.ToString().ToLower());
          dicAttr.Add("Delete", isVoid.ToString().ToLower());
          dicAttr.Add("Modified", isModify.ToString().ToLower());

          item = dicData.GetValueParser<string>("c_iteno");
          nQty = dicData.GetValueParser<decimal>("n_gqty", 0);
          nQtyDO = dicData.GetValueParser<decimal>("n_qtydo", 0);
          nQtyDiff = dicData.GetValueParser<decimal>("n_qtydiff", 0);
          claimType = dicData.GetValueParser<string>("c_claimtype");

          if (!string.IsNullOrEmpty(item))
          {
              dicAttr.Add("Item", item);
              dicAttr.Add("Qty", nQty.ToString());
              dicAttr.Add("QtyDo", nQty.ToString());
              dicAttr.Add("QtyDiff", nQty.ToString());
              dicAttr.Add("claimType", claimType.ToString());

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
        varData = parser.ParserData("MemoBASPBSJ", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_MemoBASPBCtrl SaveParser : {0} ", ex.Message);
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
        PopulateDetail("c_baspbno", pID);
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    if (isAdd)
    {
      if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
      {
        Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
        return;
      }
    }
    else if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowEdit)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menyimpan data.");
      return;
    }

    Dictionary<string, string>[] gridData = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridData);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;
        string storeId = hfStoreID.Text;

        //string cust = (cbCustomerHdr.SelectedIndex != -1 ? cbCustomerHdr.SelectedItem.Text : string.Empty);

        string dateJs = null;
        DateTime date = DateTime.Today;

        if (isAdd)
        {
          if (respon.Values != null)
          {
            numberId = respon.Values.GetValueParser<string>("MEMO", string.Empty);

            if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
            {
              dateJs = Functional.DateToJson(date);
            }

            if (!string.IsNullOrEmpty(storeId))
            {
                scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                              'c_baspbno': '{1}',
                              'd_baspb': {2},
                              'c_dono': '{3}',
                              'v_ket': '{4}',
                              'v_gdgdesc_asal': '{5}',
                              'v_gdgdesc_tujuan': '{6}'
                              }}));{0}.commitChanges();", storeId,
                                        numberId,
                                        Functional.DateToJson(date),
                                        cbSJHdr.SelectedItem.Text,
                                        txKeterangan.Text,
                                        cbGudangFromHdr.SelectedItem.Text,
                                        cbGudangToHdr.SelectedItem.Text);

              X.AddScript(scrpt);
            }
          }
        }

        //this.ClearEntrys();        
        PopulateDetail("c_baspbno", numberId);

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

    string numberID = (e.ExtraParams["NumberID"] ?? string.Empty);

    if (string.IsNullOrEmpty(numberID))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");
      return;
    }

    ReportParser rptParse = new ReportParser();
    
    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();

    rptParse.ReportingID = "10120";
    
    #region Report Parameter
    
    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "SCMS_BASPBH.c_baspbno",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID)
    });

    #endregion

    #region Linq Filter Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "c_baspbno = @0",
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
          rptResult.OutputFile, "Memo BASPB", rptResult.Extension);

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