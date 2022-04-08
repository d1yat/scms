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

public partial class keuangan_pembayaran_FakturBeliReturCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    hfFaktur.Clear();

    txTanggalHdr.Clear();
    txTanggalHdr.Disabled = false;

    lbSuplierHdr.Text = string.Empty;

    lbExFakturHdr.Text = string.Empty;

    lbTaxNoHdr.Text = string.Empty;
    lbTaxDateHdr.Text = string.Empty;

    cbKursHdr.Clear();
    cbKursHdr.Disabled = true;
    txKursValueHdr.Clear();

    txKeteranganHdr.Clear();

    lbGrossBtm.Text = "";
    lbTaxBtm.Text = "";

    lbDiscBtm.Text = "";
    lbNetBtm.Text = "";

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private PostDataParser.StructureResponse SaveParser(string numberID, Dictionary<string, string>[] dics, Dictionary<string, string>[] dicsBea)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.TagExtraName = "FieldBea";
    pair.Value = numberID;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicValuesExtra = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    //decimal jml = 0,
    //  bea = 0,
    //  disc = 0,
    //  price = 0;
    string tmp = null,
      itemId = null,
      //itemType = null,
      varData = null,
      ket = null;
    bool isNew = false,
      isModify = false,
      isVoid = false;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);

    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);

    if (!Functional.DateParser(txTanggalHdr.RawText, "dd-MM-yyyy", out date))
    {
      date = DateTime.MinValue;
    }
    pair.DicAttributeValues.Add("FakturDate", date.ToString("yyyyMMddHHmmssfff"));

    pair.DicAttributeValues.Add("Kurs", cbKursHdr.SelectedItem.Value);
    pair.DicAttributeValues.Add("KursValue", txKursValueHdr.Text);

    pair.DicAttributeValues.Add("Keterangan", txKeteranganHdr.Text);

    int nLoop = 0,
      nLen = 0;

    #region Grid Details

    for (nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      isNew = false;
      isModify = dicData.GetValueParser<bool>("l_modified");
      isVoid = dicData.GetValueParser<bool>("l_void");

      //if (isNew && (!isVoid) && (!isModify))
      //{
      //  dicAttr.Add("New", isNew.ToString().ToLower());
      //  dicAttr.Add("Delete", isVoid.ToString().ToLower());
      //  dicAttr.Add("Modified", isModify.ToString().ToLower());

      //  itemId = dicData.GetValueParser<string>("c_iteno");
      //  itemType = dicData.GetValueParser<string>("c_type");
      //  jml = dicData.GetValueParser<decimal>("n_qty", 0);
      //  disc = dicData.GetValueParser<decimal>("n_disc", 0);
      //  bea = dicData.GetValueParser<decimal>("n_bea", 0);
      //  price = dicData.GetValueParser<decimal>("n_salpri", 0);

      //  if (!string.IsNullOrEmpty(itemId))
      //  {
      //    dicAttr.Add("Item", itemId);
      //    dicAttr.Add("Type", itemType);
      //    dicAttr.Add("Qty", jml.ToString());
      //    dicAttr.Add("Disc", disc.ToString());
      //    dicAttr.Add("Price", price.ToString());
      //    dicAttr.Add("Bea", bea.ToString());

      //    pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
      //    {
      //      IsSet = true,
      //      DicAttributeValues = dicAttr
      //    });
      //  }
      //}
      //else if (isModify && (!isVoid) && (!isNew))
      //{
      //  dicAttr.Add("New", isNew.ToString().ToLower());
      //  dicAttr.Add("Delete", isVoid.ToString().ToLower());
      //  dicAttr.Add("Modified", isModify.ToString().ToLower());

      //  itemId = dicData.GetValueParser<string>("c_iteno");
      //  jml = dicData.GetValueParser<decimal>("n_qty", 0);
      //  disc = dicData.GetValueParser<decimal>("n_disc", 0);
      //  bea = dicData.GetValueParser<decimal>("n_bea", 0);
      //  price = dicData.GetValueParser<decimal>("n_salpri", 0);

      //  if (!string.IsNullOrEmpty(itemId))
      //  {
      //    dicAttr.Add("Item", itemId);
      //    dicAttr.Add("Qty", jml.ToString());
      //    dicAttr.Add("Disc", disc.ToString());
      //    dicAttr.Add("Price", price.ToString());
      //    dicAttr.Add("Bea", bea.ToString());
          
      //    pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
      //    {
      //      IsSet = true,
      //      DicAttributeValues = dicAttr
      //    });
      //  }
      //}
      if ((!isModify) && isVoid && (!isNew))
      {
        dicAttr.Add("New", isNew.ToString().ToLower());
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

    #endregion

    try
    {
      varData = parser.ParserData("FakturBeliRetur", "Modify", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("keuangan_pembayaran_FakturBeliReturCtrl SaveParser : {0} ", ex.Message);
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
        new string[] { "c_fbno = @0", pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    //string tmp = null;
    DateTime date = DateTime.Today;
    
    CultureInfo culture = new CultureInfo("id-ID");

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0107", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("Faktur Beli - {0}", pID);

        fakturId = dicResultInfo.GetValueParser<string>("c_fbno", string.Empty);

        date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_fbdate"));
        txTanggalHdr.Text = date.ToString("dd-MM-yyyy");
        txTanggalHdr.Disabled = true;

        lbSuplierHdr.Text = dicResultInfo.GetValueParser<string>("v_nama_supl", string.Empty);

        lbExFakturHdr.Text = dicResultInfo.GetValueParser<string>("c_exno", string.Empty);

        lbTaxNoHdr.Text = dicResultInfo.GetValueParser<string>("c_taxno", "-");
        date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_taxdate"));
        lbTaxDateHdr.Text = date.ToString("dd-MM-yyyy");

        kursId = dicResultInfo.GetValueParser<string>("c_kurs", string.Empty);
        Functional.SetComboData(cbKursHdr, "c_kurs", dicResultInfo.GetValueParser<string>("v_kurs_desc", string.Empty), kursId);
        txKursValueHdr.Text = dicResultInfo.GetValueParser<decimal>("n_kurs").ToString();

        lbGrossBtm.Text = dicResultInfo.GetValueParser<decimal>("n_bruto").ToString("N2", culture);
        lbTaxBtm.Text = dicResultInfo.GetValueParser<decimal>("n_ppn").ToString("N2", culture);

        lbDiscBtm.Text = dicResultInfo.GetValueParser<decimal>("n_disc").ToString("N2", culture);
        lbNetBtm.Text = dicResultInfo.GetValueParser<decimal>("n_bilva").ToString("N2", culture);

        txKeteranganHdr.Text = dicResultInfo.GetValueParser<string>("v_ket", string.Empty);

        hfFaktur.Text = fakturId;
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("keuangan_pembayaran_FakturBeliReturCtrl:PopulateDetail Header - ", ex.Message));
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
            store.BaseParams["parameters"] = string.Format("[['fakturNo', '{0}', 'System.String']]", fakturId);
          }
        }
      }

      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("keuangan_pembayaran_FakturBeliReturCtrl:PopulateDetail Detail - ", ex.Message));
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
    PopulateDetail("c_fbno", pID);
  }

  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string jsonGridValuesBea = (e.ExtraParams["gridValuesBea"] ?? string.Empty);

    Dictionary<string, string>[] gridData = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);
    Dictionary<string, string>[] gridDataBea = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValuesBea);

    PostDataParser.StructureResponse respon = SaveParser(numberId, gridData, gridDataBea);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;
        string storeId = hfStoreID.Text;

        DateTime date = DateTime.Today;
        
        if (respon.Values != null)
        {
          if (!string.IsNullOrEmpty(storeId))
          {
            scrpt = string.Format(@"var idx = {0}.findExact('c_fbno', '{1}');
                      if(idx != -1) {{
                        var r = {0}.getAt(idx);
                        r.set('n_bilva', {2});
                        r.set('n_sisa', {3});
                        {0}.commitChanges();
                      }}", storeId, numberId,
                        respon.Values.GetValueParser<decimal>("Net", -1),
                        respon.Values.GetValueParser<decimal>("Sisa", -1));

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

  protected void NoDOHdr_Change(object sender, DirectEventArgs e)
  {
    string noSupl = (e.ExtraParams["NoSupl"] ?? string.Empty);
    string noDO = (e.ExtraParams["NoDO"] ?? string.Empty);
    
    Dictionary<string, object> dicResult = null;
    List<Dictionary<string, object>> lstResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;
    //string kursId = null,
    //  fakturId = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "suplierId", noSupl, "System.String"},
        new string[] { "receiveId", noDO, "System.String"},
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string tmp = null;
    DateTime date = DateTime.Today;

    CultureInfo culture = new CultureInfo("id-ID");

    string res = soa.GlobalQueryService(0, -1, true, string.Empty, string.Empty, "14011", paramX);
    StringBuilder sb = new StringBuilder();

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        lstResultInfo = JSON.Deserialize<List<Dictionary<string, object>>>(jarr.First.ToString());

        dicResult.Clear();

        tmp = gridDetail.GetStore().ClientID;

        sb.AppendFormat("{0}.removeAll();", tmp);

        for (int nLoop = 0, nLen = lstResultInfo.Count; nLoop < nLen; nLoop++)
        {
          dicResult = lstResultInfo[nLoop];

          sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                  'c_iteno': '{1}',
                  'v_itnam': '{2}',
                  'c_type': '{3}',
                  'v_type_desc': '{4}',
                  'n_bea': {5},
                  'n_disc': {6},
                  'n_qty': {7},
                  'n_salpri': {8},
                  'l_new': true,
                  'l_modified': false,
                  'l_void': false,
                  'v_ket': ''
                }}));", tmp,
                      dicResult.GetValueParser<string>("c_iteno", string.Empty),
                      dicResult.GetValueParser<string>("v_itnam", string.Empty),
                      dicResult.GetValueParser<string>("c_type", string.Empty),
                      dicResult.GetValueParser<string>("v_type_desc", string.Empty),
                      dicResult.GetValueParser<decimal>("n_bea", 0),
                      dicResult.GetValueParser<decimal>("n_disc", 0),
                      dicResult.GetValueParser<decimal>("n_qty", 0),
                      dicResult.GetValueParser<decimal>("n_salpri", 0));

          dicResult.Clear();
        }

        sb.AppendFormat("{0}.commitChanges();", tmp);

        sb.AppendFormat("recalculateFaktur({0});", tmp);

        X.AddScript(sb.ToString());

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("keuangan_pembayaran_FakturBeliReturCtrl:NoDOHdr_Change PopulateRNItems - ", ex.Message));
    }
    finally
    {
      if (jarr != null)
      {
        jarr.Clear();
      }
      if (lstResultInfo != null)
      {
        lstResultInfo.Clear();
      }
      if (dicResult != null)
      {
        dicResult.Clear();
      }
    }
  }
}