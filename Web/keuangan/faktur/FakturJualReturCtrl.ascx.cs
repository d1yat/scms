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
using System.Globalization;

public partial class keuangan_pembayaran_FakturJualReturCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    hfFaktur.Clear();
    hfCabang.Clear();

    txTanggalHdr.Clear();
    txTanggalHdr.Disabled = false;

    txExFakturHdr.Clear();
    txExFakturHdr.Disabled = false;

    lbCustomerHdr.Text = string.Empty;

    txTaxNoHdr.Clear();
    txTaxDateHdr.Clear();

    cbKursHdr.Clear();
    cbKursHdr.Disabled = false;
    txKursValueHdr.Clear();
    cbKursHdr.Disabled = false;

    txKeteranganHdr.Clear();

    lbGrossBtm.Text = "";
    lbTaxBtm.Text = "";

    lbDiscBtm.Text = "";
    lbNetBtm.Text = "";

    btnPrint.Hidden = true;

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private PostDataParser.StructureResponse SaveParser(string numberID, Dictionary<string, string>[] dics)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = numberID;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    decimal jml = 0,
      bonus = 0,
      disc = 0,
      price = 0;
    string tmp = null,
      itemId = null,
      varData = null,
      ket = null;
    bool isModify = false,
      isVoid = false;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);

    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);

    if (!Functional.DateParser(txTanggalHdr.RawText, "dd-MM-yyyy", out date))
    {
      date = DateTime.MinValue;
    }
    pair.DicAttributeValues.Add("FakturDate", date.ToString("yyyyMMddHHmmssfff"));
    
    pair.DicAttributeValues.Add("TaxNo", txTaxNoHdr.Text);

    if (!Functional.DateParser(txTaxDateHdr.RawText, "dd-MM-yyyy", out date))
    {
      date = DateTime.MinValue;
    }
    pair.DicAttributeValues.Add("TaxDate", date.ToString("yyyyMMddHHmmssfff"));

    pair.DicAttributeValues.Add("Kurs", cbKursHdr.SelectedItem.Value);
    pair.DicAttributeValues.Add("KursValue", txKursValueHdr.Text);

    pair.DicAttributeValues.Add("Keterangan", txKeteranganHdr.Text);
    
    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
      
      isModify = dicData.GetValueParser<bool>("l_modified");
      isVoid = dicData.GetValueParser<bool>("l_void");

      if (isModify && (!isVoid))
      {
        dicAttr.Add("New", "false");
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        itemId = dicData.GetValueParser<string>("c_iteno");
        jml = dicData.GetValueParser<decimal>("n_qty", 0);
        bonus = dicData.GetValueParser<decimal>("n_disc", 0);
        disc = dicData.GetValueParser<decimal>("n_discon", 0);
        price = dicData.GetValueParser<decimal>("n_salpri", 0);

        if (!string.IsNullOrEmpty(itemId))
        {
          dicAttr.Add("Item", itemId);
          dicAttr.Add("Qty", jml.ToString());
          dicAttr.Add("Bonus", bonus.ToString());
          dicAttr.Add("Disc", disc.ToString());
          dicAttr.Add("Price", price.ToString());
          
          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr
          });
        }
      }
      else if ((!isModify) && isVoid)
      {
        dicAttr.Add("New", "false");
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        itemId = dicData.GetValueParser<string>("c_iteno");
        ket = dicData.GetValueParser<string>("v_ket");

        if (!string.IsNullOrEmpty(itemId))
        {
          dicAttr.Add("Item", itemId);

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
      varData = parser.ParserData("FakturJualRetur", "Modify", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("keuangan_pembayaran_FakturJualReturCtrl SaveParser : {0} ", ex.Message);
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

  private void PopulateDetail(string pName, string pID)
  {
    ClearEntrys();

    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;
    string kursId = null,
      fakturId = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "c_fjno = @0", pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    DateTime date = DateTime.Today;

    CultureInfo culture = new CultureInfo("id-ID");

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0105", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("Faktur Jual Retur - {0}", pID);

        fakturId = dicResultInfo.GetValueParser<string>("c_fjno", string.Empty);

        date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_fjdate"));
        txTanggalHdr.Text = date.ToString("dd-MM-yyyy");

        txExFakturHdr.Text = dicResultInfo.GetValueParser<string>("c_exno", string.Empty);

        lbCustomerHdr.Text = dicResultInfo.GetValueParser<string>("v_cunam", string.Empty);

        txTaxNoHdr.Text = dicResultInfo.GetValueParser<string>("c_taxno", string.Empty);
        date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_taxdate"));
        txTaxDateHdr.Text = date.ToString("dd-MM-yyyy");

        kursId = dicResultInfo.GetValueParser<string>("c_kurs", string.Empty);
        Functional.SetComboData(cbKursHdr, "c_kurs", dicResultInfo.GetValueParser<string>("v_desc_kurs", string.Empty), kursId);
        cbKursHdr.Disabled = true;
        txKursValueHdr.Text = dicResultInfo.GetValueParser<decimal>("n_kurs").ToString();

        lbGrossBtm.Text = dicResultInfo.GetValueParser<decimal>("n_gross").ToString("N2", culture);
        lbTaxBtm.Text = dicResultInfo.GetValueParser<decimal>("n_tax").ToString("N2", culture);

        lbDiscBtm.Text = dicResultInfo.GetValueParser<decimal>("n_disc").ToString("N2", culture);
        lbNetBtm.Text = dicResultInfo.GetValueParser<decimal>("n_net").ToString("N2", culture);

        txKeteranganHdr.Text = dicResultInfo.GetValueParser<string>("v_ket", string.Empty);

        hfCabang.Text = dicResultInfo.GetValueParser<bool>("l_cabang", false).ToString().ToLower();

        hfFaktur.Text = fakturId;

        jarr.Clear();

        btnPrint.Hidden = false;
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("keuangan_pembayaran_FakturJualReturCtrl:PopulateDetail Header - ", ex.Message));
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
            store.BaseParams.Add(new Ext.Net.Parameter("parameters",
              string.Format("[['fakturNo', '{0}', 'System.String']]", fakturId)
              , ParameterMode.Raw));
          }
          else
          {
            //store.BaseParams["parameters"] = string.Format("[['c_noteno = @0', '{0}', 'System.String']]", fakturId);
            store.BaseParams["parameters"] = string.Format("[['fakturNo', '{0}', 'System.String']]", fakturId);
          }
        }
      }

      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("keuangan_pembayaran_FakturJualReturCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  #endregion

  public void Initialize(string storeIDGridMain)
  {
    hfStoreID.Text = storeIDGridMain;
  }

  public void CommandPopulate(bool isAdd, string pID)
  {
    PopulateDetail("c_fjno", pID);
  }

  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    //string tanggalNote = (e.ExtraParams["TanggalNote"] ?? string.Empty);
    //string jenisBayar = (e.ExtraParams["JenisBayar"] ?? string.Empty);
    //string customerId = (e.ExtraParams["CustomerID"] ?? string.Empty);
    //string suplierName = (e.ExtraParams["CustomerName"] ?? string.Empty);
    //string tipeBayar = (e.ExtraParams["TipeBayar"] ?? string.Empty);
    //string bankID = (e.ExtraParams["BankID"] ?? string.Empty);
    //string rekNo = (e.ExtraParams["RekNo"] ?? string.Empty);
    //string giroID = (e.ExtraParams["GiroID"] ?? string.Empty);
    //string giroDate = (e.ExtraParams["GiroDate"] ?? string.Empty);
    //string kursID = (e.ExtraParams["KursID"] ?? string.Empty);
    //string kursValue = (e.ExtraParams["KursValue"] ?? string.Empty);
    //string jumlahTransaksi = (e.ExtraParams["JumlahTransaksi"] ?? string.Empty);
    //string sisaTransaksi = (e.ExtraParams["SisaTransaksi"] ?? string.Empty);
    //string ketHeader = (e.ExtraParams["Keterangan"] ?? string.Empty);

    Dictionary<string, string>[] gridData = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);
    
    PostDataParser.StructureResponse respon = SaveParser(numberId, gridData);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;
        string storeId = hfStoreID.Text;

        DateTime date = DateTime.Today;
        //respon.Values.GetValueParser<decimal>("Sisa", string.Empty), "yyyyMMdd", out date)
        if (respon.Values != null)
        {
          if (!string.IsNullOrEmpty(storeId))
          {
            scrpt = string.Format(@"var idx = {0}.findExact('c_fjno', '{1}');
                      if(idx != -1) {{
                        var r = {0}.getAt(idx);
                        r.set('n_net', {2});
                        r.set('n_sisa', {3});
                        {0}.commitChanges();
                      }}", storeId, numberId,
                        respon.Values.GetValueParser<decimal>("Net", -1),
                        respon.Values.GetValueParser<decimal>("Sisa", -1));

//            scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
//                                          'c_noteno': '{1}',
//                                          'd_notedate': {2},
//                                          'c_refcode': '{3}',
//                                          'v_refcode_desc': '{4}',
//                                          'n_bilva': {5},
//                                          'n_sisa': 0,
//                                          'c_vcno': '{6}',
//                                          'v_ket': '{7}'
//                          }}));{0}.commitChanges();", storeId,
//                                      respon.Values.GetValueParser<string>("Note", string.Empty),
//                                      dateJs,
//                                      customerId,
//                                      suplierName,
//                                      jumlahTransaksiOut,
//                                      respon.Values.GetValueParser<string>("Voucher", string.Empty),
//                                      ketHeader);

            X.AddScript(scrpt);
          }
        }

        //this.ClearEntrys();

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
    string numberID = (e.ExtraParams["NumberID"] ?? string.Empty);

    if (string.IsNullOrEmpty(numberID))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");

      return;
    }

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();

    rptParse.ReportingID = "10111-b";

    #region Report Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_FJRH.c_fjno",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID)
    });

    #endregion

    #region Linq Filter Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "c_fjno = @0",
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
        //string rptName = string.Concat("Packing_List_", pag.Nip, ".", rptResult.Extension);

        //string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
        //tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
        //  tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

        string tmpUri = Functional.UriDownloadGenerator(pag,
          rptResult.OutputFile, "Faktur Jual Retur ", rptResult.Extension);

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