using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_retur_ReturCustomerCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    hfRCNo.Clear();

    winDetail.Title = "Retur Cabang";

    cbCustomerHdr.Clear();
    cbCustomerHdr.Disabled = false;

    cbBaspbNo.Clear();
    cbBaspbNo.Disabled = false;

    txKeterangan.Clear();
    txKeterangan.Disabled = false;

    cbItemDtl.Clear();
    cbItemDtl.Disabled = false;

    //cbGudangHdr.Clear();
    //cbGudangHdr.Disabled = false;
    lbGudang.Text = hfGudangDesc.Text;

    //cbTipeDtl.Clear();
    //cbTipeDtl.Disabled = false;

    txPBBRHdr.Clear();
    txPBBRHdr.Disabled = false;

    cbBatDtl.Clear();
    cbBatDtl.Disabled = false;

    cbDO.Clear();
    cbDO.Disabled = false;

    btnSimpan.Disabled = false;

    //cbRN.Clear();
    //cbRN.Disabled = false;

    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));

    txQtyDtl.Clear();
    txQtyDtl.Disabled = false;

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private PostDataParser.StructureResponse ProsesRS(bool isAdd, string rcNumber, Dictionary<string, string>[] dics, string GudangId, string CustomerID, string PLNO, string Keterangan)
  {
      PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

      PostDataParser parser = new PostDataParser();
      IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

      Dictionary<string, string> dicData = null;

      PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

      Dictionary<string, string> dicAttr = null;

      pair.IsSet = true;
      pair.IsList = true;
      pair.Value = rcNumber;
      pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
      pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      string tmp = null,
        noRN = null,
        noDO = null,
        typeItem = null,
        item = null,
        batch = null,
        ket = null;
      decimal nQty = 0;
      bool isNew = false,
        isVoid = false,
        isModify = false;
      string varData = null;

      dic.Add("ID", pair);
      pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
      pair.DicAttributeValues.Add("Keterangan", Keterangan);
      pair.DicAttributeValues.Add("Gudang", GudangId);
      pair.DicAttributeValues.Add("PBBR", PLNO);
      pair.DicAttributeValues.Add("BaspbNo", cbBaspbNo.Text);

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

              noRN = dicData.GetValueParser<string>("c_rnno");
              noDO = dicData.GetValueParser<string>("c_dono");
              item = dicData.GetValueParser<string>("c_iteno");
              batch = dicData.GetValueParser<string>("c_batch");
              typeItem = dicData.GetValueParser<string>("c_type");

              nQty = dicData.GetValueParser<decimal>("n_qty", 0);

              if ((!string.IsNullOrEmpty(typeItem)) &&
                (!string.IsNullOrEmpty(noDO)) &&
                (!string.IsNullOrEmpty(noRN)) &&
                (!string.IsNullOrEmpty(item)) &&
                (!string.IsNullOrEmpty(batch)) &&
                (nQty > 0))
              {
                  dicAttr.Add("NoDO", noDO);
                  dicAttr.Add("NoRN", noRN);
                  dicAttr.Add("TypeItem", typeItem);
                  dicAttr.Add("Item", item);
                  dicAttr.Add("Batch", batch.Trim());
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

              noRN = dicData.GetValueParser<string>("c_rnno");
              noDO = dicData.GetValueParser<string>("c_dono");
              item = dicData.GetValueParser<string>("c_iteno");
              batch = dicData.GetValueParser<string>("c_batch");
              typeItem = dicData.GetValueParser<string>("c_type");

              nQty = dicData.GetValueParser<decimal>("n_qty", 0);

              if ((!string.IsNullOrEmpty(typeItem)) &&
                (!string.IsNullOrEmpty(noDO)) &&
                (!string.IsNullOrEmpty(noRN)) &&
                (!string.IsNullOrEmpty(item)) &&
                (!string.IsNullOrEmpty(batch)) &&
                (nQty > 0))
              {
                  dicAttr.Add("NoDO", noDO);
                  dicAttr.Add("NoRN", noRN);
                  dicAttr.Add("TypeItem", typeItem);
                  dicAttr.Add("Item", item);
                  dicAttr.Add("Batch", batch.Trim());
                  dicAttr.Add("Qty", nQty.ToString());

                  pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
                  {
                      IsSet = true,
                      Value = (string.IsNullOrEmpty(ket) ? "Human error" : Keterangan),
                      DicAttributeValues = dicAttr
                  });
              }
          }

          dicData.Clear();
      }

      try
      {
          varData = parser.ParserData("RCRS", (isAdd ? "Add" : "Modify"), dic);
      }
      catch (Exception ex)
      {
          Scms.Web.Common.Logger.WriteLine("transaksi_retur_ReturCustomerCtrl SaveParser : {0} ", ex.Message);
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

  private PostDataParser.StructureResponse ProsesParser(bool isadd, string rcNumber, string GudangId)
  {
      PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

      PostDataParser parser = new PostDataParser();
      IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

      Dictionary<string, string> dicData = null;

      PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

      Dictionary<string, string> dicAttr = null;

      pair.IsSet = true;
      pair.IsList = true;
      pair.Value = rcNumber;
      pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
      pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      string tmp = null,
        noRN = null,
        noDO = null,
        typeItem = null,
        item = null,
        batch = null,
        ket = null;
      decimal nQty = 0;
      bool isNew = false,
        isVoid = false,
        isModify = false;
      string varData = null;

      dic.Add("ID", pair);
      pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
      pair.DicAttributeValues.Add("Gudang", ((Scms.Web.Core.PageHandler)this.Page).ActiveGudang );
      
      try
      {
          //varData = parser.ParserData("RCIN", ("Modify"), dic);
          varData = parser.ParserData("PROSESST", ("add"), dic);
      }
      catch (Exception ex)
      {
          Scms.Web.Common.Logger.WriteLine("transaksi_retur_ReturCustomerCtrl SaveParser : {0} ", ex.Message);
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

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string rcNumber, Dictionary<string, string>[] dics, string GudangId, string CustomerID, string PBBRNO, string Keterangan)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = rcNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string tmp = null,
      noRN = null,
      noDO = null,
      typeItem = null,
      item = null,
      batch = null,
      ket = null;
    decimal nQty = 0;
    bool isNew = false,
      isVoid = false,
      isModify = false;
    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Customer", CustomerID);
    pair.DicAttributeValues.Add("Keterangan", Keterangan);
    pair.DicAttributeValues.Add("Gudang", GudangId);
    pair.DicAttributeValues.Add("PBBR", PBBRNO);
    pair.DicAttributeValues.Add("PB", PBBRNO);
    pair.DicAttributeValues.Add("BaspbNo", cbBaspbNo.Text);

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

        noRN = dicData.GetValueParser<string>("c_rnno");
        noDO = dicData.GetValueParser<string>("c_dono");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        typeItem = dicData.GetValueParser<string>("c_type");

        nQty = dicData.GetValueParser<decimal>("n_qty", 0);

        if ((!string.IsNullOrEmpty(typeItem)) &&
          (!string.IsNullOrEmpty(noDO)) &&
          (!string.IsNullOrEmpty(noRN)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)) &&
          (nQty > 0))
        {
          dicAttr.Add("NoDO", noDO);
          dicAttr.Add("NoRN", noRN);
          dicAttr.Add("TypeItem", typeItem);
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch.Trim());
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

        noRN = dicData.GetValueParser<string>("c_rnno");
        noDO = dicData.GetValueParser<string>("c_dono");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        typeItem = dicData.GetValueParser<string>("c_type");

        nQty = dicData.GetValueParser<decimal>("n_qty", 0);

        if ((!string.IsNullOrEmpty(typeItem)) &&
          (!string.IsNullOrEmpty(noDO)) &&
          (!string.IsNullOrEmpty(noRN)) &&
          (!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)) &&
          (nQty > 0))
        {
          dicAttr.Add("NoDO", noDO);
          dicAttr.Add("NoRN", noRN);
          dicAttr.Add("TypeItem", typeItem);
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch.Trim());
          dicAttr.Add("Qty", nQty.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Human error" : Keterangan),
            DicAttributeValues = dicAttr
          });
        }
      }

      dicData.Clear();
    }

    try
    {
        //varData = parser.ParserData("RCIN", ("Modify"), dic);
        if (isAdd == false)
        {
            varData = parser.ParserData("RCIN", ("Modify"), dic);
        }
        else
        {
            varData = parser.ParserData("RCIN", ("Add"), dic);
        }
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_retur_ReturCustomerCtrl SaveParser : {0} ", ex.Message);
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

    Dictionary<string, object> dicRC = null;
    Dictionary<string, string> dicRCInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
    btnprosesrs.Visible = false;
    cbPLno.Visible = false;
    //string tmp = null;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0037", paramX);

    try
    {
      dicRC = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicRC.ContainsKey("records") && (dicRC.ContainsKey("totalRows") && (((long)dicRC["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicRC["records"]);

        dicRCInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        //cbGudangHdr.ToBuilder().AddItem(
        //  (dicRCInfo.ContainsKey("v_gdgdesc") ? dicRCInfo["v_gdgdesc"] : string.Empty),
        //  (dicRCInfo.ContainsKey("c_gdg") ? dicRCInfo["c_gdg"] : string.Empty)
        //);
        //if (cbGudangHdr.GetStore() != null)
        //{
        //  cbGudangHdr.GetStore().CommitChanges();
        //}
        //cbGudangHdr.SetValueAndFireSelect((dicRCInfo.ContainsKey("c_gdg") ? dicRCInfo["c_gdg"] : string.Empty));
        //Functional.SetComboData(cbGudangHdr, "c_gdg", dicRCInfo.GetValueParser<string>("v_gdgdesc", string.Empty), dicRCInfo.GetValueParser<string>("c_gdg", string.Empty)); 
        //cbGudangHdr.Disabled = true;
        lbGudang.Text = dicRCInfo.GetValueParser<string>("v_gdgdesc", string.Empty);

        //cbCustomerHdr.ToBuilder().AddItem(
        //  (dicRCInfo.ContainsKey("v_cunam") ? dicRCInfo["v_cunam"] : string.Empty),
        //  (dicRCInfo.ContainsKey("c_cusno") ? dicRCInfo["c_cusno"] : string.Empty)
        //);
        //if (cbCustomerHdr.GetStore() != null)
        //{
        //  cbCustomerHdr.GetStore().CommitChanges();
        //}
        //cbCustomerHdr.SetValueAndFireSelect((dicRCInfo.ContainsKey("c_cusno") ? dicRCInfo["c_cusno"] : string.Empty));
        Functional.SetComboData(cbCustomerHdr, "c_cusno", dicRCInfo.GetValueParser<string>("v_cunam", string.Empty), dicRCInfo.GetValueParser<string>("c_cusno", string.Empty)); 
        cbCustomerHdr.Disabled = true;

        Functional.SetComboData(cbBaspbNo, "c_baspbno", dicRCInfo.GetValueParser<string>("c_baspbno", string.Empty), dicRCInfo.GetValueParser<string>("c_baspbno", string.Empty));
        cbBaspbNo.Disabled = true;

        if (dicRCInfo.GetValueParser<bool>("l_rcdcore", false) == true)
        {
          btnSimpan.Disabled = false;
        }

        txPBBRHdr.Text = dicRCInfo.GetValueParser<string>("pbbrno", string.Empty);
        txPBBRHdr.Disabled = true;
        cbPLno.Visible = true;
        btnprosesrs.Visible = true;

        txKeterangan.Text = (dicRCInfo.ContainsKey("v_ket") ? dicRCInfo["v_ket"] : string.Empty);

        winDetail.Title = string.Format("Retur Cabang - {0}", pID);
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_sales_ReturCustomer:PopulateDetail Header - ", ex.Message));
    }
    finally
    {
      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicRCInfo != null)
      {
        dicRCInfo.Clear();
      }
      if (dicRC != null)
      {
        dicRC.Clear();
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

      hfRCNo.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_sales_ReturCustomer:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  #endregion

  public void Initialize(string gudang, string gudangDesc, string storeIDGridMain, string sTipe)
  {
    hfGudang.Text = gudang;
    hfStoreID.Text = storeIDGridMain;
    hfGudangDesc.Text = gudangDesc;
    hfTypeID.Text = sTipe;
  }

  public void CommandPopulate(bool isNew, string pID, string sTipe)
  {
    if (isNew)
    {
      ClearEntrys();
      winDetail.Title = "Retur Customer";
      if (sTipe.Equals("01"))
      {
        //cbTipeDtl.SetValue(sTipe);
        //cbTipeDtl.Text = "Good";

        Functional.SetComboData(cbTipeDtl, "c_type", "Good", "01");
      }
      else
      {
        Functional.SetComboData(cbTipeDtl, "c_type", "Bad", "02");
      }
      winDetail.Hidden = false;
      winDetail.ShowModal();

    }
    else
    { 
      PopulateDetail("c_rcno", pID); 
    }
  }

  protected void prosesST(object sender, DirectEventArgs e)
  {
      string numberID = (e.ExtraParams["NumberID"] ?? string.Empty);
      string GudangId = (e.ExtraParams["GudangId"] ?? string.Empty);

      PostDataParser.StructureResponse respon = ProsesParser(true,numberID,GudangId);
      if (respon.IsSet)
      {
          if (respon.Response == PostDataParser.ResponseStatus.Success)
          {
              Functional.ShowMsg(respon.Message);
          }
          else
          {
              e.ErrorMessage = respon.Message;
              e.Success = false;
          }
      }
      else
      {
          e.ErrorMessage = respon.Message;
          e.Success = false;
      }

  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string GudangDesc = (e.ExtraParams["GudangDesc"] ?? string.Empty);
    string GudangId = (e.ExtraParams["GudangId"] ?? string.Empty);
    string CustomerDesc = (e.ExtraParams["CustomerDesc"] ?? string.Empty);
    string CustomerID = (e.ExtraParams["CustomerID"] ?? string.Empty);
    string PBBRNO = (e.ExtraParams["PBBRNO"] ?? string.Empty);
    string Keterangan = (e.ExtraParams["Keterangan"] ?? string.Empty);

    Dictionary<string, string>[] gridDataRC = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataRC, GudangId, CustomerID, PBBRNO, Keterangan);

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

            numberId = respon.Values.GetValueParser<string>("rcID", string.Empty);

            if (!string.IsNullOrEmpty(storeId))
            {
              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                              'c_rcno': '{1}',
                              'd_rcdate': {2},
                              'v_gdgdesc': '{3}',
                              'v_cunam': '{4}',
                              'v_ket': '{5}',
                              'c_pin': '{6}',
                              'pbbrno': '{7}',
                              'c_gdg': '{8}'
              }}));{0}.commitChanges();", storeId,
                                        numberId,
                                        dateJs, GudangDesc, CustomerDesc, Keterangan,
                                        respon.Values.GetValueParser<string>("Pin", string.Empty), PBBRNO,
                                        GudangId);

              X.AddScript(scrpt);
            }
          }
        }

        //this.ClearEntrys();

        PopulateDetail("c_rcno", numberId);

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
  protected void SaveBtnrs_Click(object sender, DirectEventArgs e)
  {
      string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
      string keterangan = (e.ExtraParams["keterangan"] ?? string.Empty);
      string GudangId = (e.ExtraParams["GudangId"] ?? string.Empty);
      string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
      bool isAdd = (string.IsNullOrEmpty(numberId) ? false : true);
      string CustomerID = (e.ExtraParams["CustomerID"] ?? string.Empty);
      Dictionary<string, string>[] gridDataRC = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);
      string POoutlet = (e.ExtraParams["POOUTLET"] ?? string.Empty);
      if (POoutlet == "" || POoutlet == null)
      {
          e.ErrorMessage = "Nomor PL tidak boleh kosong";
          e.Success = false;
      }
      else
      {
          PostDataParser.StructureResponse respon = ProsesRS(isAdd,numberId, gridDataRC,GudangId, CustomerID, POoutlet,keterangan);
          if (respon.IsSet)
          {
              if (respon.Response == PostDataParser.ResponseStatus.Success)
              {
                  Functional.ShowMsg("Data berhasil disimpan." + respon.Message);
              }
              else
              {
                  e.ErrorMessage = respon.Message;
                  e.Success = false;
              }
          }
          else
          {
              e.ErrorMessage = respon.Message;
              e.Success = false;
          }
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
      string numberID = (e.ExtraParams["NumberID"] ?? string.Empty);

      if (string.IsNullOrEmpty(numberID))
      {
          Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");

          return;
      }

      ReportParser rptParse = new ReportParser();

      List<ReportParameter> lstRptParam = new List<ReportParameter>();

      List<string> lstData = new List<string>();

      rptParse.ReportingID = "10301";

      #region Report Parameter

      lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(char).FullName,
          ParameterName = "LG_SERAHTERIMAH.c_gdg",
          ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
      });

      lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(string).FullName,
          ParameterName = "LG_SERAHTERIMAH.c_rcno",
          ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID)
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
          ParameterName = "c_rcno = @0",
          ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID),
          IsLinqFilterParameter = true
      });

      #endregion

      rptParse.PaperID = "8.5x5.5";
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
                rptResult.OutputFile, "Movement Stock", rptResult.Extension);
          }
          else
          {
              Functional.ShowMsgWarning(rptResult.MessageResponse);
          }
      }

      GC.Collect();
  }


  




}
