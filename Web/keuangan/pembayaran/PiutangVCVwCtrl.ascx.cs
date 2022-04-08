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

public partial class keuangan_pembayaran_PiutangVCVwCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    txTanggalHdr.Clear();
    txTanggalHdr.Disabled = false;

    cbTipeBayarHdr.Clear();
    cbTipeBayarHdr.Disabled = false;

    cbCustomerHdr.Clear();
    cbCustomerHdr.Disabled = false;

    cbTipeHdr.Clear();
    cbTipeHdr.Disabled = false;

    cbBankHdr.Clear();
    cbBankHdr.Disabled = false;

    cbRekeningHdr.Clear();
    txTanggalHdr.Disabled = false;

    txGiroHdr.Clear();
    txGiroHdr.Disabled = false;

    txTempoGiroHdr.Clear();
    txTempoGiroHdr.Disabled = false;

    cbKursHdr.Clear();
    cbKursHdr.Disabled = false;
    txKursValueHdr.Clear();
    txKursValueHdr.Disabled = false;

    txJumlahHdr.Clear();
    txJumlahHdr.Disabled = false;

    lbSisaHdr.Text = "";

    txKeteranganHdr.Clear();
    txKeteranganHdr.Disabled = false;

    Ext.Net.Store store = gridDetailBayarVC.GetStore();
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
    string custId = null,
      kursId = null,
      noteId = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "c_noteno = @0", pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string tmp = null;
    DateTime date = DateTime.Today;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0085", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("Credit - {0}", pID);

        noteId = dicResultInfo.GetValueParser<string>("c_noteno", string.Empty);

        date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_notedate"));
        txTanggalHdr.Text = date.ToString("dd-MM-yyyy");
        txTanggalHdr.Disabled = true;

        tmp = dicResultInfo.GetValueParser<string>("c_type", string.Empty);

        Functional.SetComboData(cbTipeHdr, "c_type", dicResultInfo.GetValueParser<string>("v_ket_jenis_bayar", string.Empty), dicResultInfo.GetValueParser<string>("c_type", string.Empty));
        cbTipeHdr.Disabled = true;

        custId = dicResultInfo.GetValueParser<string>("c_cusno", string.Empty);
        Functional.SetComboData(cbCustomerHdr, "c_cusno", dicResultInfo.GetValueParser<string>("v_nama_customer", string.Empty), custId);
        cbCustomerHdr.Disabled = true;

        Functional.SetComboData(cbTipeBayarHdr, "c_type_note", dicResultInfo.GetValueParser<string>("v_ket_tipe_bayar", string.Empty), dicResultInfo.GetValueParser<string>("c_type_note", string.Empty));
        cbTipeBayarHdr.Disabled = true;

        Functional.SetComboData(cbBankHdr, "c_bank", dicResultInfo.GetValueParser<string>("v_bank", string.Empty), dicResultInfo.GetValueParser<string>("c_bank", string.Empty));
        cbBankHdr.Disabled = true;

        Functional.SetComboData(cbRekeningHdr, "c_rekNo", dicResultInfo.GetValueParser<string>("c_rekNo", string.Empty), dicResultInfo.GetValueParser<string>("c_rekNo", string.Empty));
        cbRekeningHdr.Disabled = true;

        if (tmp.Equals("01", StringComparison.OrdinalIgnoreCase))
        {
          txGiroHdr.Clear();
          txGiroHdr.Disabled = true;
          
          txTempoGiroHdr.Clear();
          txTempoGiroHdr.Disabled = true;
        }
        else if(tmp.Equals("02", StringComparison.OrdinalIgnoreCase))
        {
          txGiroHdr.Text = dicResultInfo.GetValueParser<string>("v_no", string.Empty);
          txGiroHdr.Disabled = true;

          date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("D_tempo"));
          txTempoGiroHdr.RawText = date.ToString("dd-MM-yyyy");
          txTempoGiroHdr.Disabled = true;
        }

        kursId = dicResultInfo.GetValueParser<string>("c_kurs", string.Empty);
        Functional.SetComboData(cbKursHdr, "c_kurs", dicResultInfo.GetValueParser<string>("v_desc_kurs", string.Empty), kursId);
        cbKursHdr.Disabled = true;

        txKursValueHdr.Text = dicResultInfo.GetValueParser<decimal>("n_kurs").ToString();
        txKursValueHdr.Disabled = true;

        txJumlahHdr.Text = dicResultInfo.GetValueParser<decimal>("n_bilva").ToString();
        txJumlahHdr.Disabled = true;

        lbSisaHdr.Text = dicResultInfo.GetValueParser<decimal>("n_sisa").ToString("N2", new System.Globalization.CultureInfo("id-ID"));

        txKeteranganHdr.Text = dicResultInfo.GetValueParser<string>("v_ket", string.Empty);
        txKeteranganHdr.Disabled = true;

        hfDebit.Text = noteId;
        hfVoucher.Text = pID;

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("keuangan_pembayaran_PiutangVCVwCtrl:PopulateDetail Header - ", ex.Message));
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
      Ext.Net.Store store = gridDetailBayarVC.GetStore();
      if (store.Proxy.Count > 0)
      {
        Ext.Net.ScriptTagProxy stp = store.Proxy[0] as Ext.Net.ScriptTagProxy;
        if ((stp != null) && stp.CallbackParam.Equals(Functional.NAME_SOA_SCMS_CALLBACK, StringComparison.OrdinalIgnoreCase))
        {
          string param = (store.BaseParams["parameters"] ?? string.Empty);
          if (string.IsNullOrEmpty(param))
          {
            store.BaseParams.Add(new Ext.Net.Parameter("parameters",
              string.Format("[['c_noteno = @0', '{0}', 'System.String']]", noteId)
              , ParameterMode.Raw));
          }
          else
          {
            store.BaseParams["parameters"] = string.Format("[['c_noteno = @0', '{0}', 'System.String']]", noteId);
          }
        }
      }

      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("keuangan_pembayaran_PiutangVCVwCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  //private PostDataParser.StructureResponse SaveParser(string numberID, Dictionary<string, string>[] dics)
  //{
  //  PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

  //  PostDataParser parser = new PostDataParser();
  //  IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

  //  Dictionary<string, string> dicData = null;

  //  PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

  //  Dictionary<string, string> dicAttr = null;

  //  pair.IsSet = true;
  //  pair.IsList = true;
  //  pair.Value = numberID;
  //  pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
  //  pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

  //  decimal jmlBayar = 0;
  //  string tmp = null,
  //    fakturID = null,
  //    tipeId = null;
  //  string varData = null;

  //  DateTime date = DateTime.Today;

  //  dic.Add("ID", pair);
  //  pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
  //  pair.DicAttributeValues.Add("PaymentType", cbTipeHdr.SelectedItem.Value);
  //  pair.DicAttributeValues.Add("Customer", cbCustomerHdr.SelectedItem.Value);
  //  pair.DicAttributeValues.Add("PaymentModeType", cbTipeBayarHdr.SelectedItem.Value);
  //  pair.DicAttributeValues.Add("Bank", cbBankHdr.SelectedItem.Value);
  //  pair.DicAttributeValues.Add("Account", cbRekeningHdr.SelectedItem.Value);
  //  pair.DicAttributeValues.Add("GiroNumber", txGiroHdr.Text);

  //  if (!Functional.DateParser(txTanggalHdr.RawText, "dd-MM-yyyy", out date))
  //  {
  //    date = DateTime.MinValue;
  //  }
  //  pair.DicAttributeValues.Add("VoucherDate", date.ToString("yyyyMMddHHmmssfff"));
    
  //  if (!Functional.DateParser(txTempoGiroHdr.RawText, "dd-MM-yyyy", out date))
  //  {
  //    date = DateTime.MinValue;
  //  }
  //  pair.DicAttributeValues.Add("GiroDate", date.ToString("yyyyMMddHHmmssfff"));

  //  pair.DicAttributeValues.Add("Kurs", cbKursHdr.SelectedItem.Value);
  //  pair.DicAttributeValues.Add("KursValue", txKursValue.Text);
  //  pair.DicAttributeValues.Add("ValuePay", txJumlahHdr.Text);
  //  pair.DicAttributeValues.Add("Keterangan", txKeteranganHdr.Text);
    
  //  for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
  //  {
  //    tmp = nLoop.ToString();

  //    dicData = dics[nLoop];

  //    dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

  //    dicAttr.Add("New", "true");
  //    dicAttr.Add("Delete", "false");
  //    dicAttr.Add("Modified", "false");

  //    fakturID = dicData.GetValueParser<string>("Faktur");
  //    tipeId = dicData.GetValueParser<string>("v_type");
  //    jmlBayar = dicData.GetValueParser<decimal>("Pembayaran", 0);

  //    if ((!string.IsNullOrEmpty(fakturID)) &&
  //      ((tipeId.Equals("01") && jmlBayar > 0) ||
  //        (tipeId.Equals("02") && jmlBayar < 0)))
  //    {
  //      dicAttr.Add("Faktur", fakturID);
  //      dicAttr.Add("TipeFaktur", tipeId);
  //      dicAttr.Add("Jumlah", jmlBayar.ToString());

  //      pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
  //      {
  //        IsSet = true,
  //        DicAttributeValues = dicAttr
  //      });
  //    }

  //    dicData.Clear();
  //  }

  //  try
  //  {
  //    varData = parser.ParserData("VoucherDebit", "Add", dic);
  //  }
  //  catch (Exception ex)
  //  {
  //    Scms.Web.Common.Logger.WriteLine("keuangan_pembayaran_PiutangVCVwCtrl SaveParser : {0} ", ex.Message);
  //  }

  //  string result = null;

  //  if (!string.IsNullOrEmpty(varData))
  //  {
  //    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

  //    result = soa.PostData(varData);

  //    responseResult = parser.ResponseParser(result);
  //  }

  //  return responseResult;
  //}

  #endregion

  public void Initialize()
  {
    ;
  }

  public void CommandPopulate(string pID)
  {
    PopulateDetail("c_noteno", pID);
  }

  //protected void SaveBtn_Click(object sender, DirectEventArgs e)
  //{
  //  string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
  //  string jsonStoreValues = (e.ExtraParams["storeValues"] ?? string.Empty);

  //  Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonStoreValues);

  //  //PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gdgId, memoId, comboItem, batchCode, dQtyNumber, ketHeader, gridDataPL);

  //  PostDataParser.StructureResponse respon = SaveParser(numberId, gridDataPL);

  //  //PostDataParser.StructureResponse respon = new PostDataParser.StructureResponse()
  //  //{
  //  //  IsSet = true,
  //  //  Message = "Testing",
  //  //  Response = PostDataParser.ResponseStatus.Error      
  //  //};

  //  respon.Response = PostDataParser.ResponseStatus.Failed;

  //  if (respon.IsSet)
  //  {
  //    if (respon.Response == PostDataParser.ResponseStatus.Success)
  //    {
  //      string scrpt = null;
  //      //string storeId = hfStoreID.Text;
  //      string storeId = "";

  //      string dateJs = null;
  //      DateTime date = DateTime.Today;
        
  //      if (respon.Values != null)
  //      {
  //        if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
  //        {
  //          dateJs = Functional.DateToJson(date);
  //        }

  //        if (!string.IsNullOrEmpty(storeId))
  //        {
  //          //              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
  //          //                              'c_combono': '{1}',
  //          //                              'c_gdg': '{2}',
  //          //                              'v_gdgdesc': '{3}',
  //          //                              'd_combodate': {4},
  //          //                              'c_memono': '{5}',
  //          //                              'c_batch': '{6}',
  //          //                              'c_iteno': '{7}',
  //          //                              'n_acc': {8},
  //          //                              'n_bqty': 0,
  //          //                              'n_bsisa': 0,
  //          //                              'n_gqty': {8},
  //          //                              'n_gsisa': {8},
  //          //                              'l_confirm': false,
  //          //                              'v_ket': '{9}',
  //          //                              'v_itnam': '{10}'
  //          //              }}));{0}.commitChanges();", storeId,
  //          //                                        respon.Values.GetValueParser<string>("COMBO", string.Empty),
  //          //                                        gdgId,
  //          //                                        gdgDesc,
  //          //                                        dateJs,
  //          //                                        memoId,
  //          //                                        batchCode,
  //          //                                        comboItem,
  //          //                                        qtyNumber,
  //          //                                        ketHeader,
  //          //                                        comboItemDesc);

  //          X.AddScript(scrpt);
  //        }
  //      }

  //      this.ClearEntrys();

  //      Functional.ShowMsgInformation("Data berhasil tersimpan.");
  //    }
  //    else
  //    {
  //      e.ErrorMessage = respon.Message;

  //      e.Success = false;
  //    }
  //  }
  //  else
  //  {
  //    e.ErrorMessage = "Unknown response";

  //    e.Success = false;
  //  }
  //}

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

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
}