using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using Scms.Web.Core;
using ScmsSoaLibraryInterface.Components;

public partial class master_batch_BatchCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Master Batch";

    hfItemNo.Clear();

    X.AddScript(string.Format("{0}.getForm().reset();", frmpnlDetailEntry.ClientID));

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private void PopulateDetail(string pID, string pName)
  {
    this.ClearEntrys();

    hfItemNo.Text = pID;

    winDetail.Title = string.Format("Master Batch - ({0}) {1}", pID, pName);

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
            store.BaseParams.Add(new Ext.Net.Parameter("parameters", string.Format("[['noItem', '{0}', 'System.String']]", pID), ParameterMode.Raw));
          }
          else
          {
            store.BaseParams["parameters"] = string.Format("[['noItem', '{0}', 'System.String']]", pID);
          }
        }
      }

      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_pembelian_SuratPesananCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(string itemId, string batchId, DateTime tanggalExpired)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = itemId;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    //DateTime date = DateTime.Today;

    //if (!Functional.DateParser(tanggalExpired, "yyyy-MM-ddTHH:mm:ss", out date))
    //{
    //  date = DateTime.MinValue;
    //}

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Batch", batchId);
    pair.DicAttributeValues.Add("DateExpiration", tanggalExpired.ToString("yyyyMMddHHmmssfff"));
    pair.DicAttributeValues.Add("Gudang", ((Scms.Web.Core.PageHandler)this.Page).ActiveGudang);

    try
    {
      varData = parser.ParserData("MasterBatch", "AddModify", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("master_batch_BatchCtrl SaveParser : {0} ", ex.Message);
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

  public void CommandPopulate(string pID, string pidName)
  {
    PopulateDetail(pID, pidName);
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string itemId = (e.ExtraParams["ItemID"] ?? string.Empty);
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty).Trim();
    string exprDate = (e.ExtraParams["ExpiredDate"] ?? string.Empty);

    DateTime dateExpr = DateTime.MinValue;

    bool isAdd = false;

    if (!Functional.DateParser(exprDate, "yyyy-MM-ddTHH:mm:ss", out dateExpr))
    {
      dateExpr = DateTime.MinValue;
    }

    PostDataParser.StructureResponse respon = SaveParser(itemId, numberId, dateExpr);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;

        string dateJs = Functional.DateToJson(dateExpr),
          storeId = gridDetail.GetStore().ClientID;

        //isAdd = (respon.Values == null ? respon.Values.GetValueParser<bool>(
        if (respon.Values == null)
        {
          scrpt = string.Format(@"{0}.reload();", storeId,
                                                numberId, exprDate);
        }
        else
        {
          isAdd = respon.Values.GetValueParser<bool>("Add", false);

          if (isAdd)
          {
            scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                        'c_batch': '{1}',
                        'd_expired': {2},
                      }}));{0}.commitChanges();", storeId,
                                                  numberId, exprDate);
          }
          else
          {
            scrpt = string.Format(@"var idx = {0}.findExact('c_batch', '{1}');
                        if(idx != -1) {{
                          var rec = {0}.getAt(idx);
                          rec.set('d_expired', new {2});
                          rec.commit();
                        }}", storeId, numberId,
                           dateJs);
          }
        }

        X.AddScript(scrpt);

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
}
