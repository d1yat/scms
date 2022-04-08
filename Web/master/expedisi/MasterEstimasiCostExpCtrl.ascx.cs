using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class master_expedisi_MasterEstimasiCostExpCtrl : System.Web.UI.UserControl
{
  private static bool isAddNew;

  private void ClearEntrys()
    {
        winDetail.Title = "Master Estimasi Biaya Expedisi";

        hfNoExpEst.Clear();

        cbGudang.Clear();  

        cbCustomer.Clear();
        cbCustomer.Disabled = false;
        Ext.Net.Store cbCustomerStr = cbCustomer.GetStore();
        if (cbCustomerStr != null)
        {
            cbCustomerStr.RemoveAll();
        }

        cbEks.Clear();
        cbEks.Disabled = false;
        Ext.Net.Store cbEksStr = cbEks.GetStore();
        if (cbEksStr != null)
        {
            cbEksStr.RemoveAll();
        }

        txUdara.Clear();
        txUdara.Disabled = false;

        txDarat.Clear();
        txDarat.Disabled = false;

        txIcePack.Clear();
        txIcePack.Disabled = false;

        txCdd.Clear();
        txCdd.Disabled = false;

        txFuso.Clear();
        txFuso.Disabled = false;

        txTronton.Clear();
        txTronton.Disabled = false;

        txContainer.Clear();
        txContainer.Disabled = false;

        txCde.Clear();
        txCde.Disabled = false;

        txL300.Clear();
        txL300.Disabled = false;

        txMin.Clear();
        txMin.Disabled = false;

        //Ext.Net.Store store = gridDetail.GetStore();
        //if (store != null)
        //{
        //    store.RemoveAll();
        //}
    }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string plNumber, Dictionary<string, string>[] dics, string ExpID)
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
        cus = null,
        tipeKirim = null,
        effectiveDate = null,
        ketVoid = null, 
        varData = null,
        gdg = null;
      decimal nUdara = 0,
        nDarat = 0,
        nIce = 0,
        nCdd = 0,
        nFuso = 0,
        nTronton = 0,
        nContainer = 0,
        nCde = 0,
        nL300 = 0,
        nMin = 0;
      bool isNew = false,
        isVoid = false,
        isModify = false;

      DateTime date = DateTime.Today;

      dic.Add("ID", pair);
      pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
      pair.DicAttributeValues.Add("ExpId", ExpID);
      pair.DicAttributeValues.Add("Gudang", hfGdg.Text);

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

              gdg = dicData.GetValueParser<string>("c_gdg");
              cus = dicData.GetValueParser<string>("c_cusno");
              tipeKirim = dicData.GetValueParser<string>("c_typekrm");
              
              nUdara = dicData.GetValueParser<decimal>("n_udara", 0);
              nDarat = dicData.GetValueParser<decimal>("n_daratlaut", 0);
              nIce = dicData.GetValueParser<decimal>("n_icepack", 0);
              nCdd = dicData.GetValueParser<decimal>("n_cdd", 0);
              nFuso = dicData.GetValueParser<decimal>("n_fuso", 0);
              nTronton = dicData.GetValueParser<decimal>("n_tronton", 0);
              nContainer = dicData.GetValueParser<decimal>("n_container", 0);
              nCde = dicData.GetValueParser<decimal>("n_cde", 0);
              nL300 = dicData.GetValueParser<decimal>("n_l300", 0);
              nMin = dicData.GetValueParser<decimal>("n_expmin", 0);
              effectiveDate = dicData.GetValueParser<string>("d_effective");

              if ((!string.IsNullOrEmpty(cus)))
              {
                  dicAttr.Add("Gudang", gdg);
                  dicAttr.Add("Customer", cus);
                  dicAttr.Add("TipeKirim", tipeKirim);
                  dicAttr.Add("nUdara", nUdara.ToString());
                  dicAttr.Add("nDarat", nDarat.ToString());
                  dicAttr.Add("nIce", nIce.ToString());
                  dicAttr.Add("nCdd", nCdd.ToString());
                  dicAttr.Add("nFuso", nFuso.ToString());
                  dicAttr.Add("nTronton", nTronton.ToString());
                  dicAttr.Add("nContainer", nContainer.ToString());
                  dicAttr.Add("nCde", nCde.ToString());
                  dicAttr.Add("nL300", nL300.ToString());
                  dicAttr.Add("nExpMin", nMin.ToString());

                  Functional.DateParser(effectiveDate, "yyyy-MM-ddTHH:mm:ss", out date);
                  dicAttr.Add("EffectiveDate", date.ToString("yyyyMMdd"));

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

              gdg = dicData.GetValueParser<string>("c_gdg");
              cus = dicData.GetValueParser<string>("c_cusno");
              tipeKirim = dicData.GetValueParser<string>("c_typekrm");
              nMin = dicData.GetValueParser<decimal>("n_expmin", 0);
              effectiveDate = dicData.GetValueParser<string>("d_effective");
              ketVoid = dicData.GetValueParser<string>("v_ket");

              if ((!string.IsNullOrEmpty(cus)))
              {
                  dicAttr.Add("Gudang", gdg);
                  dicAttr.Add("Customer", cus);
                  dicAttr.Add("TipeKirim", tipeKirim);
                  dicAttr.Add("nExpMin", nMin.ToString());

                  Functional.DateParser(effectiveDate, "yyyy-MM-ddTHH:mm:ss", out date);
                  dicAttr.Add("EffectiveDate", date.ToString("yyyyMMdd"));

                  pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
                  {
                      IsSet = true,
                      Value = (string.IsNullOrEmpty(ketVoid) ? "Human error" : ketVoid),
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

              gdg = dicData.GetValueParser<string>("c_gdg");
              cus = dicData.GetValueParser<string>("c_cusno");
              tipeKirim = dicData.GetValueParser<string>("c_typekrm");

              nUdara = dicData.GetValueParser<decimal>("n_udara", 0);
              nDarat = dicData.GetValueParser<decimal>("n_daratlaut", 0);
              nIce = dicData.GetValueParser<decimal>("n_icepack", 0);
              nCdd = dicData.GetValueParser<decimal>("n_cdd", 0);
              nFuso = dicData.GetValueParser<decimal>("n_fuso", 0);
              nTronton = dicData.GetValueParser<decimal>("n_tronton", 0);
              nContainer = dicData.GetValueParser<decimal>("n_container", 0);
              nCde = dicData.GetValueParser<decimal>("n_cde", 0);
              nL300 = dicData.GetValueParser<decimal>("n_l300", 0);
              nMin = dicData.GetValueParser<decimal>("n_expmin", 0);
              effectiveDate = dicData.GetValueParser<string>("d_effective");
              ketVoid = dicData.GetValueParser<string>("v_ket");

              if ((!string.IsNullOrEmpty(cus)))
              {
                  dicAttr.Add("Gudang", gdg);
                  dicAttr.Add("Customer", cus);
                  dicAttr.Add("TipeKirim", tipeKirim);
                  dicAttr.Add("nUdara", nUdara.ToString());
                  dicAttr.Add("nDarat", nDarat.ToString());
                  dicAttr.Add("nIce", nIce.ToString());
                  dicAttr.Add("nCdd", nCdd.ToString());
                  dicAttr.Add("nFuso", nFuso.ToString());
                  dicAttr.Add("nTronton", nTronton.ToString());
                  dicAttr.Add("nContainer", nContainer.ToString());
                  dicAttr.Add("nCde", nCde.ToString());
                  dicAttr.Add("nL300", nL300.ToString());
                  dicAttr.Add("nExpMin", nMin.ToString());

                  Functional.DateParser(effectiveDate, "yyyy-MM-ddTHH:mm:ss", out date);
                  dicAttr.Add("EffectiveDate", date.ToString("yyyyMMdd"));

                  pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
                  {
                      IsSet = true,
                      Value = (string.IsNullOrEmpty(ketVoid) ? "Human error" : ketVoid),
                      DicAttributeValues = dicAttr
                  });
              }
          }
          dicData.Clear();
      }

      try
      {
          varData = parser.ParserData("MasterExpedisiBiaya", (isAdd ? "Add" : "Modify"), dic);
      }
      catch (Exception ex)
      {
          Scms.Web.Common.Logger.WriteLine("MasterEstimasiCostExpCtrl SaveParser : {0} ", ex.Message);
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

  protected void Page_Load(object sender, EventArgs e)
  {
      if (!this.IsPostBack)
      {
          hfGdg.Text = ((Scms.Web.Core.PageHandler)this.Page).ActiveGudang;
          hfGdgdesc.Text = ((Scms.Web.Core.PageHandler)this.Page).ActiveGudangDescription;
      }
  }

  private void PopulateDetail(string pID, string Mode)
  {
    ClearEntrys();
      
    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "c_exp = @0", pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
    string res = null;

    if (Mode.Equals("select"))
    {
      #region Parser Header

      res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0149", paramX);

      cbEks.Disabled = true;

      try
      {
        dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
        if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
        {
          jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

          dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());
          Functional.SetComboData(cbEks, "c_exp", dicResultInfo.GetValueParser<string>("v_ket", string.Empty), dicResultInfo.GetValueParser<string>("c_exp", string.Empty));

          winDetail.Title = string.Format("Master Estimasi Cost Expedisi - ({0} - {1})", pID, dicResultInfo["v_ket"]);

          jarr.Clear();
        }
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine(
          string.Concat("MasterEstimasiCostExpCtrl:PopulateDetail Header - ", ex.Message));
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
              store.BaseParams.Add(new Ext.Net.Parameter("parameters", string.Format("[['c_exp = @0', '{0}', 'System.String']]", pID), ParameterMode.Raw));
            }
            else
            {
              store.BaseParams["parameters"] = string.Format("[['c_exp = @0', '{0}', 'System.String']]", pID);
            }
          }
        }

        hfNoExpEst.Text = pID;
        X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine(
          string.Concat("MasterEstimasiCostExpCtrl:PopulateDetail Detail - ", ex.Message));
      }

      #endregion
    }

    hfNoExpEst.Text = pID;
    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  public void CommandPopulate(bool isAdd, string pID)
  {
    winDetail.Hidden = false;
    PopulateDetail(pID, "select");
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
      string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
      string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
      string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);
      string expId = (e.ExtraParams["expId"] ?? string.Empty);
      string expDesc = (e.ExtraParams["expDesc"] ?? string.Empty);

      Dictionary<string, string>[] gridData = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

      //bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);
      bool isAdd = isAddNew;

      PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridData, expId);

      if (respon.IsSet)
      {
          if (respon.Response == PostDataParser.ResponseStatus.Success)
          {
              if (isAdd)
              {
                  string scrpt = null;

                  if (!string.IsNullOrEmpty(storeId))
                  {
                      scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_exp': '{1}',
                'v_ket': '{2}'
              }}));{0}.commitChanges();", storeId, expId,
                               expDesc);

                      X.AddScript(scrpt);
                  }
              }

              PopulateDetail(expId, "select");

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
