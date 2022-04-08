using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using System.Text;

public partial class transaksi_penerimaan_ReceiveNoteGudangCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Receive Notes  - Transfer Gudang";

    cbGudangHdr.Clear();
    cbGudangHdr.Disabled = false;

    cbNomorSJHdr.Clear();
    cbNomorSJHdr.Disabled = false;

    txPinCodeHdr.Clear();
    txPinCodeHdr.Disabled = false;

    X.AddScript(string.Format("{0}.getForm().reset();", frmHeaders.ClientID));

    //btnPrint.Hidden = true;

    frmHeaders.Hidden = false;

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }

    btnSimpan.Hidden = false;
  }

  private void PopulateDetail(string pName, string pID, string gudangId)
  {
    ClearEntrys();

    //Dictionary<string, object> dicResult = null;
    //Dictionary<string, string> dicResultInfo = null;
    //Newtonsoft.Json.Linq.JArray jarr = null;

    //Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    //string[][] paramX = new string[][]{
    //    new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
    //  };

    //Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    winDetail.Title = string.Format("Receive Notes - Transfer Gudang - {0}", pID);

    frmHeaders.Hidden = true;

    //string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0013", paramX);
    //bool isComplete = false;

    //try
    //{
    //  dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
    //  if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
    //  {
    //    jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

    //    dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

    //    winDetail.Title = string.Format("Receive Notes Process Transfer Gudang - {0}", pID);

    //    isComplete = dicResultInfo.GetValueParser<bool>("orProses");

    //    txPinCodeHdr.Text = dicResultInfo.GetValueParser<string>("v_ket", string.Empty);
        
    //    jarr.Clear();
    //  }
    //}
    //catch (Exception ex)
    //{
    //  System.Diagnostics.Debug.WriteLine(
    //    string.Concat("transaksi_pembelian_transaksi_penerimaan_ReceiveNoteGudangCtrl:PopulateDetail Header - ", ex.Message));
    //}
    //finally
    //{
    //  //SetEditor(!isComplete);

    //  if (jarr != null)
    //  {
    //    jarr.Clear();
    //  }
    //  if (dicResultInfo != null)
    //  {
    //    dicResultInfo.Clear();
    //  }
    //  if (dicResult != null)
    //  {
    //    dicResult.Clear();
    //  }
    //}

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
          //string param = (store.BaseParams["parameters"] ?? string.Empty);
          //if (string.IsNullOrEmpty(param))
          //{
          //  store.BaseParams.Add(new Ext.Net.Parameter("parameters", string.Format("[['{0} = @0', '{1}', 'System.String']]", pName, pID), ParameterMode.Raw));
          //}
          //else
          //{
          //  store.BaseParams["parameters"] = string.Format("[['{0} = @0', '{1}', 'System.String']]", pName, pID);
          //}
          //store.BaseParams["parameters"] = string.Format("[['suratID', '{0}', 'System.String'], ['gudang', '{1}', 'System.Char']]", pID, gudangId);

          StringBuilder sb = new StringBuilder();

//          sb.AppendFormat("var myLastOptions = {0}.lastOptions;", store.ClientID);
//          sb.AppendFormat(@"Ext.apply(myLastOptions.params, {{
//                              parameters: [['suratID', '{0}', 'System.String'], ['gudang', '{1}', 'System.Char']]
//                            }});", pID, gudangId);

          sb.AppendFormat("{0}.setBaseParam('parameters', [['suratID', '{1}', 'System.String'], ['gudang', '{2}', 'System.Char']]);",
            store.ClientID, pID, gudangId);

          sb.AppendFormat("{0}.removeAll();{0}.reload();", store.ClientID);

          X.AddScript(sb.ToString());
        }
      }

      //sbScript.AppendFormat("{0}.removeAll();{0}.reload();", store.ClientID);
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_pembelian_transaksi_penerimaan_ReceiveNoteGudangCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    btnSimpan.Hidden = true;

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(Dictionary<string, string>[] dics, string gudang, string suratId, string pinCode)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = null;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string tmp = null;
    string varData = null;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("TypeRN", "05");
    pair.DicAttributeValues.Add("Gdg", (string.IsNullOrEmpty(gudang) ? "?" : gudang));
    pair.DicAttributeValues.Add("SuratID", suratId);
    pair.DicAttributeValues.Add("PIN", (pinCode ?? string.Empty).Trim());

    //date = Functional.JsonToDate("Date(1295370000000)");

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      dicAttr = new Dictionary<string, string>();

      dicAttr.Add("New", "true");
      dicAttr.Add("Delete", "false");
      dicAttr.Add("Modified", "false");

      dicAttr.Add("Item", dicData.GetValueParser<string>("c_iteno").ToString());
      dicAttr.Add("Batch", dicData.GetValueParser<string>("c_batch").ToString());

      dicAttr.Add("Qty", dicData.GetValueParser<decimal>("n_gqty").ToString());
      dicAttr.Add("QtyBad", dicData.GetValueParser<decimal>("n_bqty").ToString());

      dicAttr.Add("ReferenceID", dicData.GetValueParser<string>("c_refno"));

      //varData = dicData.GetValueParser<string>("d_ref", string.Empty);
      //if (Functional.DateParser(varData, "dd-MM-yyyy", out date))
      //{
      //  dicAttr.Add("ReferenceDate", date.ToString("yyyyMMdd"));
      //}
      //dicAttr.Add("ReferenceDate", Functional.JSDateToModern(varData).ToString("yyyyMMdd"));

      dicAttr.Add("AdditionalID", dicData.GetValueParser<string>("c_addtno"));

      //varData = dicData.GetValueParser<string>("d_addtdate", string.Empty);
      //if (Functional.DateParser(varData, "dd-MM-yyyy", out date))
      //{
      //  dicAttr.Add("AdditionalDate", date.ToString("yyyyMMdd"));
      //}
      //dicAttr.Add("AdditionalDate", Functional.JSDateToModern(varData).ToString("yyyyMMdd"));

      //dicAttr.Add("Keterangan", dicData.GetValueParser<string>("v_ket").ToString());
      //dicAttr.Add("IsFloat", dicData.GetValueParser<bool>("l_float").ToString().ToLower());
      //dicAttr.Add("Bea", dicData.GetValueParser<decimal>("n_bea").ToString());
      ////dicAttr.Add("Gudang", dicData.GetValueParser<string>("c_gdg").ToString());
      //dicAttr.Add("Suplier", dicData.GetValueParser<string>("c_nosup").ToString());
      //dicAttr.Add("IsPrint", dicData.GetValueParser<bool>("l_print").ToString().ToLower());
      //dicAttr.Add("IsStatus", dicData.GetValueParser<bool>("l_status").ToString().ToLower());
      
      pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
      {
        IsSet = true,
        DicAttributeValues = dicAttr
      });        

      dicData.Clear();
    }

    try
    {
      varData = parser.ParserData("RNTransferGudang", "Processing", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_transaksi_penerimaan_ReceiveNoteGudangCtrl SaveParser : {0} ", ex.Message);
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
  
//  private void SetEditor(bool isEditing)
//  {
//    if (isEditing)
//    {
//      /*
//      Properties of 'e' include:
//      e.grid - This grid
//      e.record - The record being edited
//      e.field - The field name being edited
//      e.value - The value being set
//      e.originalValue - The original value for the field, before the edit.
//      e.row - The grid row index
//      e.column - The grid column index
//      e.cancel - Cancel new data
//      */
//      X.AddScript(@"var verifyRowEditConfirm = function(e) {
//                      var accQty = e.record.get('n_QtyApprove');
//                      var sisa = e.record.get('n_sisa');
//                      var totAccQty = e.record.get('n_totalAcc');
//                      var lMod = e.record.get('l_modified');
//                      var oAccQty = e.record.get('n_QtyOriginal');
//                      if (accQty != sisa) {
//                        e.cancel = true;
//                        ShowWarning('Maaf, barang yang telah terproses tidak dapat diubah.');
//                      }
//                      else if(e.value == oAccQty) {
//                        e.record.reject();
//                      }
//                      else {
//                        e.record.set('n_sisa', e.value);
//                        e.record.set('n_QtyApprove', e.value);
//                        e.record.set('n_totalAcc', ((sisa > e.value) ? (totAccQty - (sisa - e.value)) : (totAccQty + (e.value - sisa))));
//                        e.record.set('l_modified', true);
//                        if(!lMod) {
//                          e.record.set('n_QtyOriginal', accQty);
//                        }
//                      }
//                    };");

//      X.AddScript(
//        string.Format(@"var tryFindColIndex = function () {{
//                          var idx = {0}.findColumnIndex('n_QtyApprove');
//                          if(idx != -1) {{
//                            {0}.setEditable(idx, true);
//                            {0}.setEditor(idx, new Ext.form.NumberField({{
//                              allowBlank: false,
//                              allowDecimals: true,
//                              allowNegative: false,
//                              minValue: 0.00
//                            }}));
//                          }}
//                        }};
//                        tryFindColIndex();", gridDetail.ColumnModel.ClientID));

//      gridDetail.AddListener("ValidateEdit", "verifyRowEditConfirm");
//    }
//    else
//    {
//      X.AddScript(
//        string.Format(@"var idx = {0}.findColumnIndex('n_QtyRequest');
//              if(idx != -1) {{
//                {0}.setEditable(idx, false);
//              }}", gridDetail.ColumnModel.ClientID));
//    }
//  }

  #endregion

  public void Initialize(string storeIDGridMain)
  {
    hfStoreID.Text = storeIDGridMain;
  }

  public void CommandPopulate(bool isAdd, string pID, string gudangId)
  {
    if (isAdd)
    {
      //SetEditor(false);

      ClearEntrys();

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      PopulateDetail("c_sjno", pID, gudangId);
    }
  }

  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string gudangValue = (e.ExtraParams["Gudang"] ?? string.Empty);
    string suratValue = (e.ExtraParams["SuratID"] ?? string.Empty);
    string pinValue = (e.ExtraParams["PIN"] ?? string.Empty);

    Dictionary<string, string>[] gridDataDetails = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    bool isAdd = true;

    PostDataParser.StructureResponse respon = SaveParser(gridDataDetails,  gudangValue, suratValue, pinValue);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        //string scrpt = null;

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

            if (!string.IsNullOrEmpty(storeId))
            {
              string scrpt = string.Format(@"{0}.reload();", storeId);

              X.AddScript(scrpt);
            }
          }
        }

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
  
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
    
    if (!pag.IsAllowPrinting)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
      return;
    }

    string gdgId = pag.ActiveGudang;
    string custId = (e.ExtraParams["CustomeID"] ?? string.Empty);
    string pl1 = (e.ExtraParams["PLID1"] ?? string.Empty);
    string pl2 = (e.ExtraParams["PLID2"] ?? string.Empty);
    string tmp = (e.ExtraParams["Async"] ?? string.Empty);

    if (string.IsNullOrEmpty(gdgId) && string.IsNullOrEmpty(custId) &&
      string.IsNullOrEmpty(pl1) && string.IsNullOrEmpty(pl2))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");
      return;
    }

    ReportParser rptParse = new ReportParser();
    
    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();
    bool isAsync = false;

    bool.TryParse(tmp, out isAsync);

    rptParse.ReportingID = "10101";
    
    #region Report Parameter
    
    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_PLH.c_gdg",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_PLH.c_cusno",
      ParameterValue = (string.IsNullOrEmpty(custId) ? string.Empty : custId)
    });

    if (!string.IsNullOrEmpty(pl1))
    {
      if (string.IsNullOrEmpty(pl2))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_PLH.c_plno",
          ParameterValue = (string.IsNullOrEmpty(pl1) ? string.Empty : pl1)
        });
      }
      else
      {
        if (pl1.CompareTo(pl2) <= 0)
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "LG_PLH.c_plno",
            ParameterValue = (string.IsNullOrEmpty(pl1) ? string.Empty : pl1)

          });
        }
        else
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{LG_PLH.c_plno}} IN ({0} TO {1}))", pl1, pl2),
            IsReportDirectValue = true
          });
        }
      }
    }

    #endregion

    rptParse.PaperID = "8.5x5.5";
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;

    string xmlFiles = ReportParser.Deserialize(rptParse);

    SoaReportCaller soa = new SoaReportCaller();

    //string result = soa.GeneratorReport(isAsync, xmlFiles);
  }
  
  protected void ProcessRNG_Click(object sender, DirectEventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    string gudang = (e.ExtraParams["gudang"] ?? string.Empty);
    string suratId = (e.ExtraParams["suratID"] ?? string.Empty);
    string pinCode = (e.ExtraParams["pinCode"] ?? string.Empty);

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "gudang", gudang, "System.Char"},
        new string[] { "suratID", suratId, "System.String"},
        new string[] { "pinCode", pinCode, "System.String"}
      };

    string result = soa.GlobalQueryService(0, -1, true, string.Empty, string.Empty, "10061", paramX);

    if (!string.IsNullOrEmpty(result))
    {
      Dictionary<string, object> dicResult = null;
      Dictionary<string, string> dicResultInfo = null;
      List<Dictionary<string, string>> lstResultInfo = null;
      Newtonsoft.Json.Linq.JArray jarr = null;
      
      dicResult = JSON.Deserialize<Dictionary<string, object>>(result);

      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        lstResultInfo = JSON.Deserialize<List<Dictionary<string, string>>>(jarr.First.ToString());

        #region Populate Data

        if ((lstResultInfo != null) && (lstResultInfo.Count > 0))
        {
          dicResult.Clear();

          StringBuilder sb = new StringBuilder();

          string storeId = gridDetail.GetStore().ClientID;

          sb.AppendFormat("{0}.removeAll();", storeId);
          DateTime dTmp = DateTime.Today;

          for (int nLoop = 0, nLen = lstResultInfo.Count; nLoop < nLen; nLoop++)
          {
            dicResultInfo = lstResultInfo[nLoop];

            sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{", storeId);

            foreach (KeyValuePair<string, string> kvp in dicResultInfo)
            {
              sb.AppendFormat("'{0}': '{1}',", kvp.Key, kvp.Value);

              //if (kvp.Key.Equals("d_ref", StringComparison.OrdinalIgnoreCase) ||
              //  kvp.Key.Equals("d_addtdate", StringComparison.OrdinalIgnoreCase))
              //{
              //  if (string.IsNullOrEmpty(kvp.Value))
              //  {
              //    sb.AppendFormat("'{0}': '',", kvp.Key);
              //  }
              //  else
              //  {
              //    dTmp = Functional.JsonDateToDate(kvp.Value);

              //    //sb.AppendFormat("'{0}': {1},", kvp.Key, Functional.DateToJson(dTmp));

              //    sb.AppendFormat("'{0}': '{1}',", kvp.Key, dTmp.ToString("dd-MM-yyyy"));

              //    //sb.AppendFormat("'{0}': {1},", kvp.Key, kvp.Value.Replace("\\/", ""));
              //  }
              //}
              //else
              //{
              //  sb.AppendFormat("'{0}': '{1}',", kvp.Key, kvp.Value);
              //}
            }

            if (sb.ToString().EndsWith(",", StringComparison.OrdinalIgnoreCase))
            {
              sb.Remove(sb.Length - 1, 1);
            }

            sb.Append("}));");

            dicResultInfo.Clear();
          }

          sb.AppendLine(string.Format("{0}.commitChanges()", storeId));

          Ext.Net.X.AddScript(sb.ToString());

          sb.Remove(0, sb.Length);
        }

        #endregion

        jarr.Clear();
      }
    }
  }

  protected void ReloadBtn_Click(object sender, DirectEventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    this.ClearEntrys();
  }
}