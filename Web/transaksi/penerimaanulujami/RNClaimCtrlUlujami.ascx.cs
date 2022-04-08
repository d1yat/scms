using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_penerimaan_RNClaimCtrlUlujami : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    hfRnNo.Clear();

    cbGudangHdr.Clear();
    cbGudangHdr.Disabled = false;
    Ext.Net.Store cbGudangHdrrstr = cbGudangHdr.GetStore();
    if (cbGudangHdrrstr != null)
    {
        cbGudangHdrrstr.RemoveAll();
    }

    cbSuplierHdr.Clear();
    cbSuplierHdr.Disabled = false;
    Ext.Net.Store cbSuplierHdrstr = cbSuplierHdr.GetStore();
    if (cbSuplierHdrstr != null)
    {
        cbSuplierHdrstr.RemoveAll();
    }

    txDoHdr.Clear();
    txDoHdr.Disabled = false;

    txDateDoHdr.Clear();
    txDateDoHdr.Disabled = false;

    txBeaHdr.SetRawValue(0);
    txBeaHdr.Disabled = false;

    chkFloatingHdr.Clear();
    chkFloatingHdr.Disabled = false;

    txKeteranganHdr.Clear();
    txKeteranganHdr.Disabled = false;

    btnPrint.Hidden = true;

    btnSave.Hidden = false;

    txDateDtl.MinDate = DateTime.Now;

    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private const string RN_CLAIM_KET = "RelokasiUJClaim03";
  private void PopulateDetail(string pName, string pID, string gdgID)
  {
    ClearEntrys();

    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"},
        new string[] { "Ket", RN_CLAIM_KET,  "System.String"},
        new string[] { "c_gdg = @0", gdgID, "System.Char"},
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0050-a", paramX);

    DateTime date = DateTime.Today;

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("Relokasi Claim Ulujami - {0}", pID);

        Functional.SetComboData(cbGudangHdr, "c_gdg", dicResultInfo.GetValueParser<string>("v_gdgdesc", string.Empty), dicResultInfo.GetValueParser<string>("c_gdg", string.Empty));
        cbGudangHdr.Disabled = true;

        Functional.SetComboData(cbSuplierHdr, "c_from", dicResultInfo.GetValueParser<string>("v_nama", string.Empty), dicResultInfo.GetValueParser<string>("c_from", string.Empty));
        cbSuplierHdr.Disabled = true;
        
        txDoHdr.Text = dicResultInfo.GetValueParser<string>("c_dono", string.Empty);
        txDoHdr.Disabled = true;

        txBeaHdr.SetRawValue(dicResultInfo.GetValueParser<decimal>("n_bea", 0));

        chkFloatingHdr.Checked = dicResultInfo.GetValueParser<bool>("l_float");

        txKeteranganHdr.Text = dicResultInfo.GetValueParser<string>("v_ket");

        date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_dodate", string.Empty));

        txDateDoHdr.SetValue(date.ToString("dd-MM-yyyy"));

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_penerimaan_RNPembelianCtrl:PopulateDetail Header - ", ex.Message));
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
              string.Format(@"[['gudang', '{0}', 'System.Char'],
                                ['{1} = @0', '{2}', 'System.String']]", gdgID, pName, pID),
                                                                      ParameterMode.Raw));
          }
          else
          {
            store.BaseParams["parameters"] = string.Format(@"[['gudang', '{0}', 'System.Char'],
                                                              ['{1} = @0', '{2}', 'System.String']]", gdgID, pName, pID);
          }
        }
      }

      hfRnNo.Text = pID;
      hfKet.Text = RN_CLAIM_KET;
      btnSave.Hidden = true;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_penerimaan_RNPembelianCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string rnNumber, Dictionary<string, string>[] dics, string gudangID, string suplierID, string nomorDO, DateTime tglDO, float bea, bool isFloating, string keterangan)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = rnNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string tmp = null,
      item = null,
      po = null,
      batch = null,
      batchExpired = null,
      ket = null;
    bool isVoid = false,
      isNew = false;
    string varData = null;

    decimal nQty = 0;

    DateTime date = DateTime.Today;
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("TypeRN", "03Ulujami");
    //pair.DicAttributeValues.Add("Gdg", gudangID);
    pair.DicAttributeValues.Add("Gdg", pag.ActiveGudang);
    pair.DicAttributeValues.Add("Suplier", suplierID);
    pair.DicAttributeValues.Add("SuplierDO", nomorDO.Trim());
    pair.DicAttributeValues.Add("TanggalDO", tglDO.ToString("yyyyMMddHHmmssfff"));
    pair.DicAttributeValues.Add("Bea", bea.ToString());
    pair.DicAttributeValues.Add("Floating", isFloating.ToString().ToLower());
    pair.DicAttributeValues.Add("Keterangan", keterangan);

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
        dicAttr.Add("Modified", "false");

        po = dicData.GetValueParser<string>("c_refno");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        batchExpired = dicData.GetValueParser<string>("d_batchexpired");
        nQty = dicData.GetValueParser<decimal>("n_gqty", 0);

        if ((!string.IsNullOrEmpty(po)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)) &&
          (!string.IsNullOrEmpty(batchExpired)) &&
          (nQty > 0))
        {
          Functional.DateParser(batchExpired, "yyyy-MM-ddTHH:mm:ss", out date);

          dicAttr.Add("Item", item);
          dicAttr.Add("Type", "01");
          dicAttr.Add("Batch", batch);
          dicAttr.Add("RefID", po);
          dicAttr.Add("Expired", date.ToString("yyyyMMdd"));
          dicAttr.Add("Qty", nQty.ToString());

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
        dicAttr.Add("Modified", "false");

        po = dicData.GetValueParser<string>("c_refno");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");

        if ((!string.IsNullOrEmpty(po)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Type", "01");
          dicAttr.Add("Batch", batch);
          dicAttr.Add("RefID", po);

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr,
            Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
          });
        }
      }

      dicData.Clear();
    }

    try
    {
      varData = parser.ParserData("RNClaim", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penerimaan_RNClaimCtrl SaveParser : {0} ", ex.Message);
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

  public void Initialize(string storeIDGridMain)
  {
    hfStoreID.Text = storeIDGridMain;
  }

  public void CommandPopulate(bool isAdd, string pID, string gdgID)
  {
    if (isAdd)
    {
      //SetEditor(false);

      ClearEntrys();
      Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
      //Functional.SetComboData(cbGudangHdr, "c_gdg", pag.ActiveGudangDescription, pag.ActiveGudang);
      Functional.SetComboData(cbGudangHdr, "c_gdg", "Gudang Ulujami", "6");
      cbGudangHdr.Disabled = true;

      winDetail.Title = "Receive Notes Claim";

      //X.AddScript(string.Format("{0}.removeAll();{0}.reload();", cbTipeDtl.GetStore().ClientID));
      //X.AddScript(string.Format("{0}.removeAll();{0}.reload();", cbTipeDtl.GetStore().ClientID));
      //cbTipeDtl.SelectByValue("01", true);
      //Functional.SetComboData(cbTipeDtl, "c_type", "Beli", "03");

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      PopulateDetail("c_rnno", pID, gdgID);
    }
  }

  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string strGdgID = (e.ExtraParams["GudangID"] ?? string.Empty);
    string strGdgDesc = (e.ExtraParams["GudangDesc"] ?? string.Empty);
    string strSuplID = (e.ExtraParams["SuplierID"] ?? string.Empty);
    string strSuplDesc = (e.ExtraParams["SuplierDesc"] ?? string.Empty);
    string strNumberDO = (e.ExtraParams["NumberDO"] ?? string.Empty);
    string strKeterangan = (e.ExtraParams["Keterangan"] ?? string.Empty);
    string strStoreID = (e.ExtraParams["StoreID"] ?? string.Empty);

    string strDateDO = (e.ExtraParams["DateDO"] ?? string.Empty);
    string strBea = (e.ExtraParams["Bea"] ?? string.Empty);
    string strFloating = (e.ExtraParams["Floating"] ?? string.Empty);

    bool isFloating = false;
    float fBea = 0;
    DateTime dateDO = DateTime.Today;

    bool.TryParse(strFloating, out isFloating);
    float.TryParse(strBea, out fBea);
    Functional.DateParser(strDateDO, "yyyy-MM-ddTHH:mm:ss", out dateDO);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    if (isAdd)
    {
      if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowAdd)
      {
        Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
        return;
      }
    }
    else if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowEdit)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menyimpan data.");
      return;
    }

    Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataPL, strGdgID, strSuplID, strNumberDO, dateDO, fBea, isFloating, strKeterangan);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null,
          dateJs = null,
          dateDoJs = null;

        DateTime date = DateTime.Today;

        if (isAdd)
        {
          if (respon.Values != null)
          {
            if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
            {
              dateJs = Functional.DateToJson(date);
            }

            dateDoJs = Functional.DateToJson(dateDO);
            numberId = respon.Values.GetValueParser<string>("RN", string.Empty);

            if (!string.IsNullOrEmpty(strStoreID))
            {
              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_gdg': '{1}',
                'v_gdgdesc': '{2}',
                'c_rnno': '{3}',
                'd_rndate': {4},
                'c_dono': '{5}',
                'd_dodate': {6},
                'v_nama': '{7}',
                'l_float': {8},
                'l_print': false,
                'l_status': false
              }}));{0}.commitChanges();", strStoreID,
                    strGdgID, strGdgDesc,
                    respon.Values.GetValueParser<string>("RN", string.Empty),
                    dateJs, strNumberDO, dateDoJs, strSuplDesc, 
                    isFloating.ToString().ToLower());

              X.AddScript(scrpt);
            }
          }
        }

        //this.ClearEntrys();        
        if (!string.IsNullOrEmpty(numberId))
        {
            this.PopulateDetail("c_rnno", numberId, strGdgID);
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
