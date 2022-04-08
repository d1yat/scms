using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class faktur_manualCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Faktur Manual";

    //hfGdgId.Clear();
    hfFMno.Clear();

    cbPrincipalHdr.Clear();

    Ext.Net.Store cbPrincipalStr = cbPrincipalHdr.GetStore();
    if (cbPrincipalStr != null)
    {
        cbPrincipalStr.RemoveAll();
    }

    txTaxNoHdr.Clear();
    txTaxNoHdr.Disabled = true;
    
    //txTaxDateHdr.Clear();
   
    txDPP.Clear();

    txPPN.Clear();
    txPPN.Disabled = true;

    txTotal.Clear();
    txTotal.Disabled = true;

    txKet.Clear();

    txRef.Clear();
  }

  private void PopulateDetail(string pID)
  {
    ClearEntrys();

    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "c_fmno = @0", pID, "System.String"}
      };

    string tmp = null;

    bool isAktif = false;

    DateTime date = DateTime.Today;

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0200", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Concat("Faktur Manual - ", pID);

        hfFMno.Text = pID;

        Functional.SetComboData(cbPrincipalHdr, "c_nosup", dicResultInfo.GetValueParser<string>("v_nama", string.Empty), dicResultInfo.GetValueParser<string>("c_nosup", string.Empty));

        txTaxNoHdr.Text = dicResultInfo.GetValueParser<string>("c_taxno", string.Empty);
        txTaxNoHdr.Disabled = false;

        date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_taxdate"));
        txTaxDateHdr.Text = date.ToString("dd-MM-yyyy");

        txDPP.Text = dicResultInfo.GetValueParser<decimal>("n_dpp", 0).ToString();
        txPPN.Text = dicResultInfo.GetValueParser<decimal>("n_ppn", 0).ToString();
        txTotal.Text = dicResultInfo.GetValueParser<decimal>("n_total", 0).ToString();
        txKet.Text = dicResultInfo.GetValueParser<string>("v_ket", string.Empty);
        txRef.Text = dicResultInfo.GetValueParser<string>("v_ref", string.Empty);

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("faktur_manualCtrl:PopulateDetail Header - ", ex.Message));
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

    //hfGdgId.Text = pID;
    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    //pair.Value = numberId;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    DateTime date = DateTime.Today;

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("fmno", string.IsNullOrEmpty(hfFMno.Text) ? "" : hfFMno.Text);
    pair.DicAttributeValues.Add("nosup", cbPrincipalHdr.SelectedItem.Value);
    pair.DicAttributeValues.Add("taxno", txTaxNoHdr.Text);
    if (!Functional.DateParser(txTaxDateHdr.RawText, "dd-MM-yyyy", out date))
    {
        date = DateTime.MinValue;
    }
    pair.DicAttributeValues.Add("taxdate", date.ToString("yyyyMMddHHmmssfff"));
    pair.DicAttributeValues.Add("dpp", txDPP.Text);
    pair.DicAttributeValues.Add("ppn", txPPN.Text);
    pair.DicAttributeValues.Add("total", txTotal.Text);
    pair.DicAttributeValues.Add("ket", txKet.Text);
    pair.DicAttributeValues.Add("referensi", txRef.Text);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    


    try
    {
      varData = parser.ParserData("FakturManual", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
        Scms.Web.Common.Logger.WriteLine("faktur_manualCtrl SaveParser : {0} ", ex.Message);
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
    string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    PostDataParser.StructureResponse respon = SaveParser(isAdd);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {

         DateTime date = DateTime.Today;

         string dateJs = null;

         if (Functional.DateParser(respon.Values.GetValueParser<string>("taxdate", string.Empty), "yyyyMMdd", out date))
         {
             dateJs = Functional.DateToJson(date);
         }

        if (isAdd)
        {
          string sd = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                     'c_fmno' : '{1}',  
                     'v_nama': '{2}',
                     'c_taxno': '{3}',
                     'd_taxdate': {4},
                     'n_dpp': {5},
                     'n_ppn': {6},
                     'n_total': {7},
                     'v_ref': '{8}',
                     'v_ket': '{9}'
                     }}));{0}.commitChanges();", storeId,
                    (respon.Values.GetValueParser<string>("fmno", string.Empty) == null ? "" : respon.Values.GetValueParser<string>("fmno", string.Empty)),
                    cbPrincipalHdr.SelectedItem.Text,
                    (respon.Values.GetValueParser<string>("taxno", string.Empty) == null ? "" : respon.Values.GetValueParser<string>("taxno", string.Empty)),
                    dateJs, 
                    txDPP.Text, 
                    txPPN.Text, 
                    txTotal.Text, 
                    txRef.Text, 
                    txKet.Text
                    );

          X.AddScript(sd);
          this.ClearEntrys();

          Functional.ShowMsgInformation("Data berhasil tersimpan.");
        }
        else
        {
          string sd = string.Format(@"var c_fmno = {0}.findExact('c_fmno', '{1}');
                                if(c_fmno != -1) {{
                                  var r = {0}.getAt(c_fmno);
                                  r.set('v_nama', '{2}');
                                  r.set('c_taxno', '{3}');
                                  r.set('d_taxdate', {4});
                                  r.set('n_dpp', {5});
                                  r.set('n_ppn', {6});
                                  r.set('n_total', {7});
                                  r.set('v_ref', '{8}');
                                  r.set('v_ket', '{9}');
                                  {0}.commitChanges();
                                }}", storeId, 
                                   numberId,
                                   cbPrincipalHdr.SelectedItem.Text, 
                                   txTaxNoHdr.Text,
                                   dateJs, 
                                   txDPP.Text, 
                                   txPPN.Text, 
                                   txTotal.Text, 
                                   txRef.Text, 
                                   txKet.Text);

          X.AddScript(sd);

          Functional.ShowMsgInformation("Data berhasil diubah.");
        }
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
