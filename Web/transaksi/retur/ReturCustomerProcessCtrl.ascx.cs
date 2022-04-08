using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_retur_ReturCustomerProcessCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Retur Customer Proses";

    hfRCNoProcess.Clear();

    cbCustomerHdr.Clear();
    cbCustomerHdr.Disabled = true;

    txKeterangan.Clear();
    txKeterangan.Disabled = false;

    txPBBR.Clear();
    txPBBR.Disabled = false;

    //cbGudangHdr.Clear();
    //cbGudangHdr.Disabled = false;
    lbGudang.Text = hfGudangProcessDesc.Text;

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private void PopulateDetail(string pName, string pID, string Case)
  {
    //Dictionary<string, object> dicRC = null;
    //Dictionary<string, string> dicRCInfo = null;
    //Newtonsoft.Json.Linq.JArray jarr = null;

    //Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    //string[][] paramX = new string[][]{
    //    new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
    //  };

    //Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string tmp = null;

    switch (Case)
    {
      case "View":
        {
          #region Parser Header

          //string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0032", paramX);

          //try
          //{
          //  dicRC = JSON.Deserialize<Dictionary<string, object>>(res);
          //  if (dicRC.ContainsKey("records") && (dicRC.ContainsKey("totalRows") && (((long)dicRC["totalRows"]) > 0)))
          //  {
          //    jarr = new Newtonsoft.Json.Linq.JArray(dicRC["records"]);

          //    dicRCInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

          //    cbCustomerHdr.ToBuilder().AddItem(
          //      (dicRCInfo.ContainsKey("v_cunam") ? dicRCInfo["v_cunam"] : string.Empty),
          //      (dicRCInfo.ContainsKey("c_cusno") ? dicRCInfo["c_cusno"] : string.Empty)
          //    );
          //    if (cbCustomerHdr.GetStore() != null)
          //    {
          //      cbCustomerHdr.GetStore().CommitChanges();
          //    }
          //    cbCustomerHdr.SetValueAndFireSelect((dicRCInfo.ContainsKey("c_cusno") ? dicRCInfo["c_cusno"] : string.Empty));
          //    cbCustomerHdr.Disabled = true;

          //    //cbGudangHdr.ToBuilder().AddItem(
          //    //  (dicRCInfo.ContainsKey("v_gdgdesc") ? dicRCInfo["v_gdgdesc"] : string.Empty),
          //    //  (dicRCInfo.ContainsKey("c_gdg") ? dicRCInfo["c_gdg"] : string.Empty)
          //    //);
          //    //if (cbGudangHdr.GetStore() != null)
          //    //{
          //    //  cbGudangHdr.GetStore().CommitChanges();
          //    //}
          //    //cbGudangHdr.SetValueAndFireSelect((dicRCInfo.ContainsKey("c_gdg") ? dicRCInfo["c_gdg"] : string.Empty));
          //    //cbGudangHdr.Disabled = true;

          //    txKeterangan.Text = (dicRCInfo.ContainsKey("v_ket") ? dicRCInfo["v_ket"] : string.Empty);

          //    txPBBR.Text = (dicRCInfo.ContainsKey("v_pbbrno") ? dicRCInfo["v_pbbrno"] : string.Empty);
          //    txPBBR.Disabled = true;
          //    //txPBBR.Text = (dicRCInfo.ContainsKey("v_ket") ? dicRCInfo["v_ket"] : string.Empty);

          //    winDetail.Title = string.Format("Retur Customer Proses - {0}", pID);
          //  }
          //}
          //catch (Exception ex)
          //{
          //  System.Diagnostics.Debug.WriteLine(
          //    string.Concat("transaction_sales_ReturCustomer:PopulateDetail Header - ", ex.Message));
          //}
          //finally
          //{


          //  if (jarr != null)
          //  {
          //    jarr.Clear();
          //  }
          //  if (dicRCInfo != null)
          //  {
          //    dicRCInfo.Clear();
          //  }
          //  if (dicRC != null)
          //  {
          //    dicRC.Clear();
          //  }
          //}

          //#region Parser Detail

          //try
          //{

          //  Ext.Net.Store store = gridDetail.GetStore();
          //  if (store.Proxy.Count > 0)
          //  {
          //    Ext.Net.ScriptTagProxy stp = store.Proxy[0] as Ext.Net.ScriptTagProxy;
          //    if ((stp != null) && stp.CallbackParam.Equals(Functional.NAME_SOA_SCMS_CALLBACK, StringComparison.OrdinalIgnoreCase))
          //    {
          //      string param = (store.BaseParams["parameters"] ?? string.Empty);
          //      if (string.IsNullOrEmpty(param))
          //      {
          //        store.BaseParams.Add(new Ext.Net.Parameter("parameters", string.Format("[['{0} = @0', '{1}', 'System.String']]", pName, pID), ParameterMode.Raw));
          //      }
          //      else
          //      {
          //        store.BaseParams["parameters"] = string.Format("[['{0} = @0', '{1}', 'System.String']]", pName, pID);
          //      }
          //    }
          //  }

          //  hfRCNoProcess.Text = pID;
          //  X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
          //}
          //catch (Exception ex)
          //{
          //  System.Diagnostics.Debug.WriteLine(
          //    string.Concat("transaction_sales_ReturCustomer:PopulateDetail Detail - ", ex.Message));
          //}

          //#endregion

          //winDetail.Hidden = false;
          //winDetail.ShowModal();

          #endregion
        }
        break;
      case "Add":
        {
          #region Add

          Ext.Net.Store store = gridDetail.GetStore();

          tmp = string.Format(@"var xOpts = {{
                                  params: {{
                                      start: '0',
                                      limit: '-1',
                                      allQuery: 'true',
                                      model: 'a-15001',
                                      sort: '',
                                      dir: '',
                                      parameters: [['C_PBNO', paramValueGetter({0}), 'System.String'],
                                                  ['cusmas', paramValueGetter({1}), 'System.String']]
                                    }}
                                  }};", txPBBR.ClientID, cbCustomerHdr.ClientID);

          X.AddScript(tmp);
          X.AddScript(string.Format("{0}.removeAll();{0}.reload(xOpts);", store.ClientID));

          txPBBR.Text = txPBBR.Text.ToUpper();
//          string cab = txPBBR.Text.Substring(3, 3);
//            tmp = string.Format(@"var xOpts = {{
//                                  params: {{
//                                      start: '0',
//                                      limit: '-1',
//                                      allQuery: 'true',
//                                      model: '2193',
//                                      sort: '',
//                                      dir: '',
//                                      parameters: [['c_cab_dcore', paramValueGetter({0}), 'System.String']
//                                    }}
//                                  }};", cab);
//            X.AddScript(tmp);


          #endregion
        }
        break;
    }
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string rcNumber, Dictionary<string, string>[] dics)
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
      noDO = null,
      item = null,
      batch = null,
      outlet = null,
      tipeOutlet = null,
      ket = null, Keterangan = null, batchterima = null, reason = null;
    decimal nQty = 0, 
      nQtyAcc = 0, 
      nQtyDestroy = 0;
    bool isNew = false,
      isVoid = false,
      isModify = false;
    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);

    pair.DicAttributeValues.Add("Customer", cbCustomerHdr.Text);
    pair.DicAttributeValues.Add("Keterangan", txKeterangan.Text.Trim());
    pair.DicAttributeValues.Add("Gudang", hfGudangProcess.Text);
    pair.DicAttributeValues.Add("PB", txPBBR.Text.Trim());
    pair.DicAttributeValues.Add("USER", Constant.SIGN_ID);

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      isNew = dicData.GetValueParser<bool>("l_new");
      isVoid = dicData.GetValueParser<bool>("l_void");
      isModify = dicData.GetValueParser<bool>("l_modified");
      
      if ((!isNew) && isVoid && (!isModify))
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        noDO = dicData.GetValueParser<string>("c_dono");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        batchterima = dicData.GetValueParser<string>("c_batchterima");
        nQty = dicData.GetValueParser<decimal>("n_qty", 0);
        reason = dicData.GetValueParser<string>("C_REASON");

          if (batchterima.ToString() == "" )
          {
              responseResult.Message = "Batch tidak boleh kosong";
              goto endlogic;
          }

        if ((!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)) &&
          (nQty > 0))
        {

          dicAttr.Add("NoDO", noDO);
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("Qty", nQty.ToString());
          dicAttr.Add("batchterima", batchterima.ToString());
          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Human error" : Keterangan),
            DicAttributeValues = dicAttr
          });
        }
      }
      else 
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", "true");
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        noDO = dicData.GetValueParser<string>("C_NOREF");
        item = dicData.GetValueParser<string>("C_ITENO");
        batch = dicData.GetValueParser<string>("C_BATCH");
        batchterima = dicData.GetValueParser<string>("c_batchterima"); //suwandi
        nQty = dicData.GetValueParser<decimal>("N_QTY", 0);
        nQtyAcc = dicData.GetValueParser<decimal>("n_qtyAcc", 0);
        nQtyDestroy = dicData.GetValueParser<decimal>("n_destroy", 0);
        reason = dicData.GetValueParser<string>("C_REASON");

        if (batchterima.ToString() == "")
        {
            responseResult.Message = "Batch tidak boleh kosong";
            goto endlogic;
        }
        outlet = dicData.GetValueParser<string>("C_CUNAM", string.Empty);
        if (outlet == "&nbsp" || outlet == "&nbsp;")
        {
            outlet = string.Empty;
        }
        tipeOutlet = dicData.GetValueParser<string>("C_CUSTYPE", string.Empty);

        if ((!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)) && (nQty > 0))
        {
          dicAttr.Add("NoDO", noDO);
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("Qty", nQty.ToString());
          dicAttr.Add("Acceptance", nQtyAcc.ToString());
          dicAttr.Add("Destroy", nQtyDestroy.ToString());
          dicAttr.Add("Outlet", outlet.ToString());
          dicAttr.Add("TipeOutlet", tipeOutlet.ToString());
          dicAttr.Add("batchterima", batchterima.ToString());
          dicAttr.Add("C_REASON", reason.ToString());
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
      varData = parser.ParserData("RC", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_retur_ReturCustomerCtrl SaveParser : {0} ", ex.Message);
    }

    string result = null;
     endlogic:
    if (!string.IsNullOrEmpty(varData))
    {
      Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

      result = soa.PostData(varData);

      responseResult = parser.ResponseParser(result);
    }


    return responseResult;

  }

  #endregion

  public void Initialize(string gudang, string gudangDesc, string storeIDGridMain)
  {
    hfStoreID.Text = storeIDGridMain;
    hfGudangProcess.Text = gudang;
    hfStoreIDProcess.Text = storeIDGridMain;
    hfGudangProcessDesc.Text = gudangDesc;
    //btnSavePharmanet.Hidden = true;

  }

  public void CommandPopulate(bool isNew, string pID)
  {
    if (isNew)
    {
      ClearEntrys();

      winDetail.Hidden = false;
      winDetail.ShowModal();

    }
    else
    {
      PopulateDetail("c_rcno", pID, "View");
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string strStoreID = (e.ExtraParams["StoreID"] ?? string.Empty);
    string strGdgDesc = (e.ExtraParams["GudangDesc"] ?? string.Empty);
    string strGdgID = (e.ExtraParams["GudangID"] ?? string.Empty);

    Dictionary<string, string>[] gridDataRC = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);
    bool isSent = false;

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataRC);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null,
          dateJs = null,
          NoPBB = null;

        DateTime date = DateTime.Today;

        if (isAdd)
        {
          if (respon.Values != null)
          {
            NoPBB = respon.Values.GetValueParser<string>("PBBR", string.Empty);
            numberId = respon.Values.GetValueParser<string>("RC", string.Empty);
            if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
            {
              dateJs = Functional.DateToJson(date);
            }
            if (!string.IsNullOrEmpty(strStoreID))
            {
              isSent = respon.Values.GetValueParser<bool>("Sent");

              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_rcno': '{1}',
                'd_rcdate': {2},                
                'v_gdgdesc': '{3}',
                'v_cunam': '{4}',
                'v_ket': '{5}',
                'c_pin': {6},
                'pbbrno': '{7}',
                'l_sent': {8},
                'l_send': {8},
                'c_gdg': '{9}'
              }}));{0}.commitChanges();", strStoreID, numberId, dateJs, strGdgDesc,
                      respon.Values.GetValueParser<string>("Cabang", string.Empty),
                      respon.Values.GetValueParser<string>("Keterangan", string.Empty),
                      respon.Values.GetValueParser<string>("Pin", string.Empty),
                      NoPBB,
                      isSent.ToString().ToLower(), strGdgID);

              X.AddScript(scrpt);

              //if (respon.Values.GetValueParser<string>("RC", string.Empty) != "-")
              //{
              //}
              //else
              //{
              //  txPBBR.Clear();
              //  txPBBR.Text = respon.Values.GetValueParser<string>("NewPBBR", string.Empty);
              //  NewID = true;
              //  goto End;

              //}              
            }
          }
        }

        this.ClearEntrys();

        scrpt = string.Format(@"if(Ext.isFunction(selectedSavedRCData)) {{
                      {0}.hide();
                      selectedSavedRCData('{1}');
                    }}
                    else {{
                      ShowInformasi('Data berhasil tersimpan.');
                    }}", winDetail.ClientID, numberId);

        X.AddScript(scrpt);

        //Functional.ShowMsgInformation("Data berhasil tersimpan.");
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
    ////End:
    //if (NewID)
    //{
    //  Functional.ShowMsgInformation("Data Di Ganti Baru.");
    //  PopulateDetail("c_rcno", null, "Add");
    //}
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void OnEvenAddGrid(object sender, DirectEventArgs e)
  {
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);
    
    PopulateDetail(pName, pID, "Add");
    
    GC.Collect();
  }



    // hafizh awal



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





  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click_Pharmanet(object sender, DirectEventArgs e)
  {
      //string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
      //string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
      //string GudangDesc = (e.ExtraParams["GudangDesc"] ?? string.Empty);
      //string GudangId = (e.ExtraParams["GudangId"] ?? string.Empty);
      //string PrinsipalDesc = (e.ExtraParams["PrinsipalDesc"] ?? string.Empty);
      //string PrinsipalID = (e.ExtraParams["PrinsipalID"] ?? string.Empty);
      //string Keterangan = (e.ExtraParams["Keterangan"] ?? string.Empty);
      //string typeName = (e.ExtraParams["TypeName"] ?? string.Empty);


      string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
      string StoreID = (e.ExtraParams["gridValues"] ?? string.Empty);
      string GudangId = (e.ExtraParams["GudangDesc"] ?? string.Empty);
      string PrinsipalID = (e.ExtraParams["GudangId"] ?? string.Empty);
      string Keterangan = (e.ExtraParams["PrinsipalDesc"] ?? string.Empty);
      string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);



      Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

      bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

      //PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataPL, GudangId, PrinsipalID, Keterangan);
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
                                                    //respon.Values.GetValueParser<string>("RSID", string.Empty),
                                                    //dateJs, GudangDesc, PrinsipalDesc,
                                                    //typeName, Keterangan, GudangId);

                                                    respon.Values.GetValueParser<string>("RSID", string.Empty),
                                                    dateJs, Keterangan, GudangId);

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

    // hafizh akhir
}
