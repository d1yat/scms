using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class master_bank_MasterBankCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Bank";

    txNamaBank.Clear();
    txNamaBank.Disabled = false;

    txNamaCabang.Clear();
    txNamaCabang.Disabled = false;

    cbTipe.Clear();
    cbTipe.Disabled = false;

    txNoGL.Clear();
    txNoGL.Disabled = false;

    txNoRek.Clear();
    txNoRek.Disabled = false;

    txPemilik.Clear();
    txPemilik.Disabled = false;

    chkAktif.Clear();
    chkAktif.Disabled = false;

    hfBankId.Clear();

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private void PopulateDetail(string pID)
  {
    ClearEntrys();

    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "c_bank = @0", pID, "System.string"}
      };

    string tmp = null;

    bool isAktif = false;

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty , "0150", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Concat("Bank - ", dicResultInfo.GetValueParser<string>("v_bank", string.Empty));

        txNamaBank.Text = dicResultInfo.GetValueParser<string>("v_bank", string.Empty);
        txNamaCabang.Text = dicResultInfo.GetValueParser<string>("v_bankcab", string.Empty);

        tmp = dicResultInfo.GetValueParser<string>("l_aktif", string.Empty);

        bool.TryParse(tmp, out isAktif);

        chkAktif.Checked = isAktif;

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("master_gudang_GudangCtrl:PopulateDetail Header - ", ex.Message));
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
            store.BaseParams.Add(new Ext.Net.Parameter("parameters", string.Format("[['c_bank = @0', '{0}', 'System.String']]", pID), ParameterMode.Raw));
          }
          else
          {
            store.BaseParams["parameters"] = string.Format("[['c_bank = @0', '{0}', 'System.String']]", pID);
          }
        }
      }

      hfBankId.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("master_bank_MasterBankCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    hfBankId.Text = pID;
    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string bankId, Dictionary<string, string>[] dics)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    Dictionary<string, string> dicAttr = null;

    Dictionary<string, string> dicData = null;

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();
    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = bankId;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string tmp = null,
      rek = null,
      pemilik = null,
      glno = null,
      tipe = null,
      ket = null;
    decimal idx = 0;
    bool isNew = false,
      isVoid = false,
      isModify = false;
    string varData = null;

    dic.Add("c_bank", pair);
    pair.DicAttributeValues.Add("v_bank", txNamaBank.Text.Trim());
    pair.DicAttributeValues.Add("v_bankcab", txNamaCabang.Text.Trim());
    pair.DicAttributeValues.Add("l_aktif", chkAktif.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);

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

        rek = dicData.GetValueParser<string>("c_rekno");
        pemilik = dicData.GetValueParser<string>("v_pemilk");
        glno = dicData.GetValueParser<string>("c_glno");
        tipe = dicData.GetValueParser<string>("c_type");

        if ((!string.IsNullOrEmpty(rek)) &&
          (!string.IsNullOrEmpty(tipe)))
        {
          dicAttr.Add("Rekening", rek);
          dicAttr.Add("Pemilik", pemilik);
          dicAttr.Add("Glno", glno);
          dicAttr.Add("Tipe", tipe);

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

        rek = dicData.GetValueParser<string>("c_rekno");
        pemilik = dicData.GetValueParser<string>("v_pemilk");
        glno = dicData.GetValueParser<string>("c_glno");
        tipe = dicData.GetValueParser<string>("c_type");
        ket = dicData.GetValueParser<string>("v_ketBatal");
        idx = dicData.GetValueParser<decimal>("IDX");

        if ((!string.IsNullOrEmpty(rek)) &&
          (!string.IsNullOrEmpty(tipe)))
        {
          dicAttr.Add("Rekening", rek);
          dicAttr.Add("Pemilik", pemilik);
          dicAttr.Add("Glno", glno);
          dicAttr.Add("Tipe", tipe);
          dicAttr.Add("idx", idx.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
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

        rek = dicData.GetValueParser<string>("c_rekno");
        pemilik = dicData.GetValueParser<string>("v_pemilk");
        glno = dicData.GetValueParser<string>("c_glno");
        tipe = dicData.GetValueParser<string>("c_type");
        idx = dicData.GetValueParser<decimal>("IDX");
        ket = dicData.GetValueParser<string>("v_ketBatal");

        if ((!string.IsNullOrEmpty(rek)) &&
          (!string.IsNullOrEmpty(tipe)))
        {
          dicAttr.Add("Rekening", rek);
          dicAttr.Add("Pemilik", pemilik);
          dicAttr.Add("Glno", glno);
          dicAttr.Add("Tipe", tipe);
          dicAttr.Add("idx", idx.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? "Modify " : ket),
            DicAttributeValues = dicAttr
          });
        }
      }

      dicData.Clear();
    }

    try
    {
      varData = parser.ParserData("MasterBank", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("master_bank_MasterBankCtrl SaveParser : {0} ", ex.Message);
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
      this.ClearEntrys();

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      PopulateDetail(pID);
    }
  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);

    Dictionary<string, string>[] gridData = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridData);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;

        if (isAdd)
        {
          numberId = respon.Values.GetValueParser<string>("BankId", string.Empty);

          scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                          'c_bank': '{1}',
                          'v_bank': '{2}',
                          'v_bankcab': '{3}',
                          'l_aktif': {4}
                        }}));{0}.commitChanges();", storeId, numberId,
                   txNamaBank.Text.Trim(), txNamaCabang.Text.Trim(), 
                   chkAktif.Checked.ToString().ToLower());

          hfBankId.Text = numberId;

          winDetail.Title = string.Concat("Bank - ", txNamaBank.Text.Trim());
        }
        else
        {
          scrpt = string.Format(@"var idx = {0}.findExact('c_bank', '{1}');
                      if(idx != -1) {{
                        var r = {0}.getAt(idx);
                        r.set('v_bank', '{2}');
                        r.set('v_bankcab', '{3}');
                        r.set('l_aktif', {4});
                        {0}.commitChanges();
                      }}", storeId, numberId,
                      txNamaBank.Text.Trim(), txNamaCabang.Text.Trim(),
                      chkAktif.Checked.ToString().ToLower());
        }

        X.AddScript(scrpt);

        this.ClearEntrys();
        PopulateDetail(numberId);
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
