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

public partial class transaksi_retur_ReturSupplierConfCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Retur Suplier Konfirmasi";

    hfRSNo.Clear();

    cbGudangHdr.Clear();
    cbGudangHdr.ClearValue();
    cbGudangHdr.Disabled = false;

    cbPrincipalHdr.Clear();
    cbPrincipalHdr.ClearValue();
    cbPrincipalHdr.Disabled = false;

    txKeterangan.Clear();
    txKeterangan.Disabled = false;

    //cbNoRefDtl.Clear();
    //cbRSList.ClearValue();
    //cbRSList.Disabled = false;

    txCPRNo.Clear();
    txCPRNo.Disabled = false;

    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));

    cbItemDtl.Clear();
    cbItemDtl.ClearValue();
    cbItemDtl.Disabled = false;

    cbBatDtl.Clear();
    cbBatDtl.ClearValue();
    cbBatDtl.Disabled = false;

    txGQtyDtl.Clear();
    txGQtyDtl.Disabled = false;

    txBQtyDtl.Clear();
    txBQtyDtl.Disabled = false;

    txQRedressDtl.Clear();
    txQRedressDtl.Disabled = false;

    txQRejectDtl.Clear();
    txQRejectDtl.Disabled = false;

    txQReworkDtl.Clear();
    txQReworkDtl.Disabled = false;

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private void PopulateDetail(string pName, string pID, string mode)
  {
    Dictionary<string, object> dicRS = null;
    Dictionary<string, string> dicRSInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string tmp = null;

    switch (mode)
    {
      case "view":
        {
          ClearEntrys();

          #region Parser Header

          string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0052", paramX);

          try
          {
            dicRS = JSON.Deserialize<Dictionary<string, object>>(res);
            if (dicRS.ContainsKey("records") && (dicRS.ContainsKey("totalRows") && (((long)dicRS["totalRows"]) > 0)))
            {
              jarr = new Newtonsoft.Json.Linq.JArray(dicRS["records"]);

              dicRSInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

              winDetail.Title = string.Format("Retur Suplier Confirm - {0}", pID);

              cbGudangHdr.SetValueAndFireSelect((dicRSInfo.ContainsKey("v_gdgdesc") ? dicRSInfo["v_gdgdesc"] : string.Empty));
              cbGudangHdr.Disabled = true;

              cbPrincipalHdr.SetValueAndFireSelect((dicRSInfo.ContainsKey("v_nama") ? dicRSInfo["v_nama"] : string.Empty));
              cbPrincipalHdr.Disabled = true;

              txKeterangan.Text = ((dicRSInfo.ContainsKey("v_ket") ? dicRSInfo["v_ket"] : string.Empty));

              txCPRNo.Text = ((dicRSInfo.ContainsKey("c_cprno") ? dicRSInfo["c_cprno"] : string.Empty));

              cbItemDtl.Disabled = true;
              cbBatDtl.Disabled = true;

              //X.AddScript(string.Format("clearRelatedComboRecursive(true, {0});", cbItemDtl.ClientID));

              jarr.Clear();

            }
          }
          catch (Exception ex)
          {
            System.Diagnostics.Debug.WriteLine(
              string.Concat("transaction_sales_ReturSupplier:PopulateDetail Header - ", ex.Message));
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
            hfRSNo.Text = pID;
            Ext.Net.Store store = gridDetail.GetStore();
            tmp = string.Format(@"var xOpts = {{
                                  params: {{
                                      start: '10',
                                      limit: '-1',
                                      allQuery: 'true',
                                      model: '0137',
                                      sort: '',
                                      dir: '',
                                      parameters: [['c_rsno = @0', paramValueGetter({0}), 'System.String']]
                                    }}
                                  }};", hfRSNo.ClientID);

            X.AddScript(tmp);
            X.AddScript(string.Format("{0}.removeAll();{0}.reload(xOpts);", store.ClientID));

            //Ext.Net.Store store = gridDetail.GetStore();
            //if (store.Proxy.Count > 0)
            //{
            //  Ext.Net.ScriptTagProxy stp = store.Proxy[0] as Ext.Net.ScriptTagProxy;
            //  if ((stp != null) && stp.CallbackParam.Equals(Functional.NAME_SOA_SCMS_CALLBACK, StringComparison.OrdinalIgnoreCase))
            //  {
            //    string param = (store.BaseParams["parameters"] ?? string.Empty);
            //    if (string.IsNullOrEmpty(param))
            //    {
            //      store.BaseParams.Add(new Ext.Net.Parameter("parameters", string.Format("[['{0} = @0', '{1}', 'System.String']]", pName, pID), ParameterMode.Raw));
            //    }
            //    else
            //    {
            //      store.BaseParams["parameters"] = string.Format("[['{0} = @0', '{1}', 'System.String']]", pName, pID);
            //    }
            //  }
            //}

            //hfRSNo.Text = pID;
            //X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
          }
          catch (Exception ex)
          {
            System.Diagnostics.Debug.WriteLine(
              string.Concat("transaction_sales_PackingList:PopulateDetail Detail - ", ex.Message));
          }

          #endregion
        }
        break;
      case "add":
        {
          #region Add

          Ext.Net.Store store = gridDetail.GetStore();
          tmp = string.Format(@"var xOpts = {{
                                  params: {{
                                      start: '10',
                                      limit: '10',
                                      allQuery: 'true',
                                      model: '0133',
                                      sort: '',
                                      dir: '',
                                      parameters: [['rsno', paramValueGetter({0}), 'System.String'],
                                                   ['supl', paramValueGetter({1}), 'System.String'],
                                                   ['gdg', paramValueGetter({2}), 'System.Char']]
                                    }}
                                  }};", cbNoRefDtl.ClientID, cbPrincipalHdr.ClientID, cbGudangHdr.ClientID);

          X.AddScript(tmp);
          X.AddScript(string.Format("{0}.removeAll();{0}.reload(xOpts);", store.ClientID));

          #endregion
        }
        break;
    }
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string plNumber, Dictionary<string, string>[] dics, string Gudangid, string Prinsipal, string keterangan, string NoCPR, string NoRS1)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    bool isNew = false,
      isVoid = false,
      isModify = false, isAlternate = false,
      isValid = false;
    string tmp = null,
      item = null, batch = null, rsDet = null,
      varData = null, ketDel = null;
    decimal nGQty = 0, nBQty = 0, nQty = 0, nQtyRej = 0,
      nQtyRew = 0, nQtyRed = 0;


    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = plNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Supplier", Prinsipal);
    pair.DicAttributeValues.Add("Keterangan", keterangan);
    pair.DicAttributeValues.Add("Gudang", Gudangid);
    pair.DicAttributeValues.Add("NoCPR", NoCPR);
    pair.DicAttributeValues.Add("NoRS1", NoRS1);
    //pair.DicAttributeValues.Add("NoRS2", NoRS2);

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {

      tmp = nLoop.ToString();
      Ext.Net.NumberColumn num = new NumberColumn();

      dicData = dics[nLoop];

      isNew = dicData.GetValueParser<bool>("l_new");
      isVoid = dicData.GetValueParser<bool>("l_void");
      isModify = dicData.GetValueParser<bool>("l_modified");

      nQtyRej = dicData.GetValueParser<decimal>("nReject");
      nQtyRew = dicData.GetValueParser<decimal>("nRework");
      nQtyRed = dicData.GetValueParser<decimal>("nRedress");

      isValid = ((nQtyRej + nQtyRew + nQtyRed) > 0) ? true : false;

      if (isNew && (!isVoid) && (!isModify) && (isValid))
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        decimal nGalloc = 0, nBalloc = 0, nRetTotal = 0, Good = 0; ;

        rsDet = dicData.GetValueParser<string>("c_rsno");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        nGQty = dicData.GetValueParser<decimal>("good");
        nBQty = dicData.GetValueParser<decimal>("bad");
        nQty = dicData.GetValueParser<decimal>("nQty");
        nQtyRej = dicData.GetValueParser<decimal>("nReject");
        nQtyRew = dicData.GetValueParser<decimal>("nRework");
        nQtyRed = dicData.GetValueParser<decimal>("nRedress");
        isAlternate = dicData.GetValueParser<bool>("l_alternate");

        nRetTotal = nQtyRej + nQtyRew + nQtyRed;

        //if (nRetTotal > 0)
        //{
        //  nBalloc = nBQty;
        //  nBalloc -= nRetTotal;
        //  if (nBalloc < 0)
        //  {
        //    Good = nGQty;
        //    nGalloc = nGQty;
        //    Good += nBalloc;
        //    nGalloc -= Good;
        //    nBalloc = nBQty;
        //  }
        //}

        if (nRetTotal != nQty)
        {
          nBalloc = nBQty;
          nBalloc -= nRetTotal;
          if (nBalloc < 0)
          {
            Good = nGQty;
            nGalloc = nGQty;
            Good += nBalloc;
            nGalloc -= Good;
            nBalloc = nBQty;
          }
        }
        else
        {
          nBalloc = nBQty;
          nGalloc = nGQty;
        }

        if ((!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)))
        {
          dicAttr.Add("NORS", rsDet);
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("nGQty", nGalloc.ToString());
          dicAttr.Add("nBQty", nBalloc.ToString());
          dicAttr.Add("nQty", nQty.ToString());
          dicAttr.Add("nQtyRej", nQtyRej.ToString());
          dicAttr.Add("nQtyRew", nQtyRew.ToString());
          dicAttr.Add("nQtyRed", nQtyRed.ToString());
          dicAttr.Add("isAlternate", isAlternate.ToString().ToLower());
          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            DicAttributeValues = dicAttr
          });
        }
      }
      if ((!isNew) && isVoid && (!isModify))
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        rsDet = dicData.GetValueParser<string>("c_rsno");
        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");
        nGQty = dicData.GetValueParser<decimal>("good");
        nBQty = dicData.GetValueParser<decimal>("bad");
        nQty = dicData.GetValueParser<decimal>("nQty");
        nQtyRej = dicData.GetValueParser<decimal>("nReject");
        nQtyRew = dicData.GetValueParser<decimal>("nRework");
        nQtyRed = dicData.GetValueParser<decimal>("nRedress");
        isAlternate = dicData.GetValueParser<bool>("l_alternate");

        if ((!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)))
        {
          dicAttr.Add("NORS", rsDet);
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
          dicAttr.Add("nGQty", nGQty.ToString());
          dicAttr.Add("nBQty", nBQty.ToString());
          dicAttr.Add("nQty", nQty.ToString());
          dicAttr.Add("nQtyRej", nQtyRej.ToString());
          dicAttr.Add("nQtyRew", nQtyRew.ToString());
          dicAttr.Add("nQtyRed", nQtyRed.ToString());
          dicAttr.Add("isAlternate", isAlternate.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ketDel) ? "Human error" : ketDel),
            DicAttributeValues = dicAttr
          });
        }
      }
      if ((!isNew) && (!isVoid) && isModify)
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        batch = dicData.GetValueParser<string>("c_batch");

        if ((!string.IsNullOrEmpty(item)) &&
          (!string.IsNullOrEmpty(batch)))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);
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
      varData = parser.ParserData("RSCONF", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_penjualan_ReturSupplierConfCtrl SaveParser : {0} ", ex.Message);
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
      winDetail.Hidden = false;
      winDetail.ShowModal();
      PopulateDetail("c_rsno", pID, "view");
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void OnEvenAddGrid(object sender, DirectEventArgs e)
  {
    //string pName = (e.ExtraParams["Parameter"] ?? string.Empty);

    //string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    System.Threading.Thread.Sleep(5000);

    PopulateDetail(null, null, "add");

    GC.Collect();
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string GudangDesc = (e.ExtraParams["GudangDesc"] ?? string.Empty);
    string GudangId = (e.ExtraParams["GudangID"] ?? string.Empty);
    string PrinsipalDesc = (e.ExtraParams["PemasokDesc"] ?? string.Empty);
    string PrinsipalID = (e.ExtraParams["PemasokID"] ?? string.Empty);
    string Keterangan = (e.ExtraParams["Keterangan"] ?? string.Empty);
    string NoCPR = (e.ExtraParams["NoCPR"] ?? string.Empty);
    string NoRS1 = (e.ExtraParams["NoRS1"] ?? string.Empty);
    //string NoRS2 = (e.ExtraParams["NoRS2"] ?? string.Empty);

    Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataPL, GudangId, PrinsipalID, Keterangan, NoCPR, NoRS1);

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
                              'v_ket': '{5}',
                              'c_cprno': '{6}'
              }}));{0}.commitChanges();", storeId,
                                        respon.Values.GetValueParser<string>("rsID", string.Empty),
                                        dateJs, GudangDesc, PrinsipalDesc, Keterangan, NoCPR);

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
}
