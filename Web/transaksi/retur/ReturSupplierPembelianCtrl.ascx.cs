using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;


public partial class transaksi_retur_ReturSupplierPembelianCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    hfRSNo.Clear();

    winDetail.Title = "Retur Supplier Pembelian";

    //cbGudangHdr.Clear();
    //cbGudangHdr.Disabled = false;
    lbGudang.Text = hfGudangDesc.Text;

    cbPrincipalHdr.Clear();
    cbPrincipalHdr.Disabled = false;

    txKeterangan.Clear();
    txKeterangan.Disabled = false;


    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));

    cbRptTypeOutput.Hidden = false;
    btnPrint.Hidden = false;
    btnPrintUpload.Hidden = false;

    cbItemDtl.Disabled = false;
    cbItemDtl.Clear();

    cbBatDtl.Disabled = false;
    cbBatDtl.Clear();

    txGQtyDtl.Disabled = false;
    txGQtyDtl.Clear();

    txBQtyDtl.Disabled = false;
    txBQtyDtl.Clear();

    txKetDtl.Disabled = false;
    txKetDtl.Clear();

    txKPRDtl.Disabled = false;
    txKPRDtl.Clear();

    //cbBatDtl.Disabled = false;
    //txGQtyDtl.Disabled = false;
    //txBQtyDtl.Disabled = false;
    //txKetDtl.Disabled = false;
    //txBQtyDtl.Disabled = false;
    //txKPRDtl.Disabled = false;
    tbPnlGridDetail.Disabled = false;

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }

  }

  private void PopulateDetail(string pName, string pID)
  {
    ClearEntrys();

    Dictionary<string, object> dicRS = null;
    Dictionary<string, string> dicRSInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
      
    //tbPnlGridDetail.Disabled = true;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0051", paramX);

    try
    {
      dicRS = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicRS.ContainsKey("records") && (dicRS.ContainsKey("totalRows") && (((long)dicRS["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicRS["records"]);

        dicRSInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("Retur Supplier Pembelian - {0}", pID);

        Functional.SetComboData(cbPrincipalHdr, "c_nosup", dicRSInfo.GetValueParser<string>("v_nama", string.Empty), dicRSInfo.GetValueParser<string>("c_nosup", string.Empty));
        cbPrincipalHdr.Disabled = true;

        string noSup = dicRSInfo.GetValueParser<string>("c_nosup", string.Empty);
        if (noSup == "00085" || noSup == "00019")
        {
            btnPrintUpload.Text = "Send Upload";
        }
        else
        {
            btnPrintUpload.Text = "Cetak Upload";
        }

        txKeterangan.Text = ((dicRSInfo.ContainsKey("v_ket") ? dicRSInfo["v_ket"] : string.Empty));

        X.AddScript(string.Format("clearRelatedComboRecursive(true, {0});", cbItemDtl.ClientID));

        jarr.Clear();

      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_retur_ReturSupplierPembelianCtrl:PopulateDetail Header - ", ex.Message));
    }
    finally
    {

      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicRSInfo != null)
      {
        dicRSInfo.Clear();
      }
      if (dicRS != null)
      {
        dicRS.Clear();
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

      hfRSNo.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_sales_PackingList:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    btnPrint.Hidden = false;
    btnPrintUpload.Hidden = false;

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();

  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string plNumber, Dictionary<string, string>[] dics, string Gudangid, string Prinsipal, string keterangan)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    bool isNew = false,
      isVoid = false,
      isModify = false;
    string tmp = null,
      item = null, batch = null,
      ket = null, cprno = null, varData = null, ketDel = null,
      cabang = null, outlet = null, reason = null;
    decimal nGQty = 0, nBQty = 0, nGQtyNew = 0, nBQtyNew = 0
      , nGQtyFinal = 0, nBQtyFinal = 0;


    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = plNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);

    pair.DicAttributeValues.Add("Supplier", Prinsipal);
    pair.DicAttributeValues.Add("Keterangan", keterangan);
    pair.DicAttributeValues.Add("TipeRS", "01");
    pair.DicAttributeValues.Add("Gudang", Gudangid);

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {

      tmp = nLoop.ToString();
      Ext.Net.NumberColumn num = new NumberColumn();

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
        ket = dicData.GetValueParser<string>("v_ket");
        cprno = dicData.GetValueParser<string>("c_cprno");
        cabang = dicData.GetValueParser<string>("c_cusno");
        outlet = dicData.GetValueParser<string>("c_outlet");
        reason = dicData.GetValueParser<string>("c_reason");

        nGQty = dicData.GetValueParser<decimal>("n_gqty", 0);
        nBQty = dicData.GetValueParser<decimal>("n_bqty", 0);

        if ((!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)) &&
          ((nBQty > 0) || (nGQty > 0)))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("GQty", nGQty.ToString());
          dicAttr.Add("BQty", nBQty.ToString());
          dicAttr.Add("ketD", ket);
          dicAttr.Add("CprNo", cprno);
          dicAttr.Add("Cabang", cabang);
          dicAttr.Add("Outlet", outlet);
          dicAttr.Add("Reason", reason);
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
        cprno = dicData.GetValueParser<string>("c_cprno");
        cabang = dicData.GetValueParser<string>("c_cusno");
        outlet = dicData.GetValueParser<string>("c_outlet");
        reason = dicData.GetValueParser<string>("c_reason");
        ketDel = dicData.GetValueParser<string>("ketDel");

        nGQty = dicData.GetValueParser<decimal>("n_gqty", 0);
        nBQty = dicData.GetValueParser<decimal>("n_bqty", 0);

        if ((!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)) &&
          ((nBQty > 0) || (nGQty > 0)))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("GQty", nGQty.ToString());
          dicAttr.Add("BQty", nBQty.ToString());
          dicAttr.Add("ketD", ket);
          dicAttr.Add("CprNo", cprno);
          dicAttr.Add("Cabang", cabang);
          dicAttr.Add("Outlet", outlet);
          dicAttr.Add("Reason", reason);

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ketDel) ? "Human error" : ketDel),
            DicAttributeValues = dicAttr
          });
        }
      }
      else if ((!isNew) && (!isVoid) && isModify)
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        ket = dicData.GetValueParser<string>("v_ket");
        cprno = dicData.GetValueParser<string>("c_cprno");
        cabang = dicData.GetValueParser<string>("c_cusno");
        outlet = dicData.GetValueParser<string>("c_outlet");
        reason = dicData.GetValueParser<string>("c_reason");

        nGQty = dicData.GetValueParser<decimal>("n_gqtyH", 0);
        nGQtyNew = dicData.GetValueParser<decimal>("n_gqty", 0);
        nBQty = dicData.GetValueParser<decimal>("n_bqtyH", 0);
        nBQtyNew = dicData.GetValueParser<decimal>("n_bqty", 0);

        nGQtyFinal = nGQtyNew - nGQty;
        nBQtyFinal = nBQtyNew - nBQty;

        if ((!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("GQty", nGQtyFinal.ToString());
          dicAttr.Add("BQty", nBQtyFinal.ToString());
          dicAttr.Add("ketD", ket);
          dicAttr.Add("CprNo", cprno);
          dicAttr.Add("Cabang", cabang);
          dicAttr.Add("Outlet", outlet);
          dicAttr.Add("Reason", reason);
          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr
          });
        }
      }
      dicData.Clear();
    }

    try
    {
      varData = parser.ParserData("RSBELI", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_ReturSupplierPembelianCtrl SaveParser : {0} ", ex.Message);
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

  public void Initialize(string storeIDGridMain, string typeName, string ActiveGudang, string ActiveGudangDescription)
  {
    hfStoreID.Text = storeIDGridMain;
    hfTypeName.Text = typeName;
    hfGudang.Text = ActiveGudang;
    hfGudangDesc.Text = ActiveGudangDescription;
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  public void CommandPopulate(bool isAdd, string pID)
  {

    if (isAdd)
    {
      ClearEntrys();

      btnPrint.Hidden = true;
      btnPrintUpload.Hidden = true;
      cbRptTypeOutput.Hidden = true;


      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      PopulateDetail("c_rsno", pID);
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string GudangDesc = (e.ExtraParams["GudangDesc"] ?? string.Empty);
    string GudangId = (e.ExtraParams["GudangId"] ?? string.Empty);
    string PrinsipalDesc = (e.ExtraParams["PrinsipalDesc"] ?? string.Empty);
    string PrinsipalID = (e.ExtraParams["PrinsipalID"] ?? string.Empty);
    string Keterangan = (e.ExtraParams["Keterangan"] ?? string.Empty);
    string typeName = (e.ExtraParams["TypeName"] ?? string.Empty);

    Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataPL, GudangId, PrinsipalID, Keterangan);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;
        string storeId = hfStoreID.Text;

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
              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                              'c_rsno': '{1}',
                              'd_rsdate': {2},
                              'v_gdgdesc': '{3}',
                              'v_nama': '{4}',
                              'v_descTrans': '{5}',
                              'v_ket': '{6}',
                              'c_gdg': '{7}'
              }}));{0}.commitChanges();", storeId,
                                        respon.Values.GetValueParser<string>("RSID", string.Empty),
                                        dateJs, GudangDesc, PrinsipalDesc,
                                        typeName, Keterangan, GudangId);

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

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string gdgId = (e.ExtraParams["GudangId"] ?? string.Empty); ;
    string numberID = (e.ExtraParams["NumberID"] ?? string.Empty);
    string outputRpt = (e.ExtraParams["OutputRpt"] ?? string.Empty);
      
    if (string.IsNullOrEmpty(numberID))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");

      return;
    }

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();

    rptParse.ReportingID = "10107";

    #region Report Parameter

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(char).FullName,
      ParameterName = "LG_RSH.c_gdg",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_RSH.c_rsno",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(char).FullName,
      ParameterName = "({LG_RSH.c_type} = '01')",
      IsReportDirectValue = true
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
      ParameterName = "c_rsno = @0",
      ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID),
      IsLinqFilterParameter = true
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "c_type = @0",
      ParameterValue = "01",
      IsLinqFilterParameter = true
    });

    #endregion

    rptParse.IsLandscape = false;
    rptParse.PaperID = "14*8.5";
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;
    rptParse.OutputReport = ReportParser.ParsingOutputReport(outputRpt);
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
          rptResult.OutputFile, "Retur Suplier Pembelian ", rptResult.Extension);

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
  protected void Report_Upload_OnGenerate(object sender, DirectEventArgs e)
  {
      Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

      string gdgId = (e.ExtraParams["GudangId"] ?? string.Empty); ;
      string numberID = (e.ExtraParams["NumberID"] ?? string.Empty);
      string supplierID = (e.ExtraParams["supplierID"] ?? string.Empty);

      if (string.IsNullOrEmpty(numberID))
      {
          Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");

          return;
      }

      ReportParser rptParse = new ReportParser();

      List<ReportParameter> lstRptParam = new List<ReportParameter>();

      List<string> lstData = new List<string>();

      rptParse.ReportingID = "10107-u";

      #region Report Parameter

      lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(char).FullName,
          ParameterName = "LG_RSH.c_gdg",
          ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
      });

      lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(string).FullName,
          ParameterName = "LG_RSH.c_rsno",
          ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID)
      });

      lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(char).FullName,
          ParameterName = "({LG_RSH.c_type} = '01')",
          IsReportDirectValue = true
      });

      #endregion

      #region Sql Parameter

      lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(char).FullName,
          ParameterName = "rsno",
          IsSqlParameter = true,
          ParameterValue = numberID
      });

      if (supplierID == "00019" || supplierID == "00085")
      {
          lstRptParam.Add(new ReportParameter()
          {
              DataType = typeof(char).FullName,
              ParameterName = "nosup",
              IsSqlParameter = true,
              ParameterValue = supplierID
          });
      }

      lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(char).FullName,
          ParameterName = "tipeRS",
          IsSqlParameter = true,
          ParameterValue = "01"
      });
      #endregion

      rptParse.PaperID = "A4";
      rptParse.ReportParameter = lstRptParam.ToArray();
      rptParse.User = pag.Nip;
      rptParse.OutputReport = ReportParser.ParsingOutputReport("02");

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
                rptResult.OutputFile, "Retur Suplier Pembelian Upload", rptResult.Extension);

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
