using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class master_pkpCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
      winDetail.Title = "Master PKP";

      //hfGdgId.Clear();
      hfPKPno.Clear();

      txPKPNameHdr.Clear();
      txNPWP.Clear();
      txNPPKP.Clear();
      txNPPKPDateHdr.Clear();
      txAlamat1.Clear();
      txAlamat2.Clear();
      txPhone1.Clear();
      txFax1.Clear();
      txFax2.Clear();
      chkAktif.Clear();
  }

  private void PopulateDetail(string pID)
  {
    ClearEntrys();

    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "c_pkpno = @0", pID, "System.String"}
      };

    DateTime date = DateTime.Today;

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0214", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Concat("Master PKP - ", pID);

        hfPKPno.Text = pID;

        txPKPNameHdr.Text = dicResultInfo.GetValueParser<string>("v_nama", string.Empty);
        txNPWP.Text = dicResultInfo.GetValueParser<string>("v_npwp", string.Empty);
        txNPPKP.Text = dicResultInfo.GetValueParser<string>("v_nppkp", string.Empty);


        date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_tglpkp"));
        txNPPKPDateHdr.Text = date.ToString("dd-MM-yyyy");

        txAlamat1.Text = dicResultInfo.GetValueParser<string>("v_alamat1", string.Empty);
        txAlamat2.Text = dicResultInfo.GetValueParser<string>("v_alamat2", string.Empty);
        txPhone1.Text = dicResultInfo.GetValueParser<string>("v_telepon1", string.Empty);
        txFax1.Text = dicResultInfo.GetValueParser<string>("v_fax1", string.Empty);
        txFax2.Text = dicResultInfo.GetValueParser<string>("v_fax2", string.Empty);
        chkAktif.Checked = dicResultInfo.GetValueParser<bool>("l_aktif", false);

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("master_pkpCtrl:PopulateDetail Header - ", ex.Message));
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
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("pkpno", string.IsNullOrEmpty(hfPKPno.Text) ? "" : hfPKPno.Text);
    pair.DicAttributeValues.Add("pkpname", txPKPNameHdr.Text);
    pair.DicAttributeValues.Add("npwp", txNPWP.Text);
    pair.DicAttributeValues.Add("nppkp", txNPPKP.Text);

    if (!Functional.DateParser(txNPPKPDateHdr.RawText, "dd-MM-yyyy", out date))
    {
        date = DateTime.MinValue;
    }
    pair.DicAttributeValues.Add("nppkpdate", date.ToString("yyyyMMddHHmmssfff"));

    pair.DicAttributeValues.Add("alamat1", txAlamat1.Text);
    pair.DicAttributeValues.Add("alamat2", txAlamat2.Text);
    pair.DicAttributeValues.Add("telepon1", txPhone1.Text);
    pair.DicAttributeValues.Add("fax1", txFax1.Text);
    pair.DicAttributeValues.Add("fax2", txFax2.Text);
    pair.DicAttributeValues.Add("isAktif", chkAktif.Value.ToString().ToLower());

    try
    {
      varData = parser.ParserData("MasterPKP", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
        Scms.Web.Common.Logger.WriteLine("master_pkpCtrl SaveParser : {0} ", ex.Message);
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

         if (Functional.DateParser(respon.Values.GetValueParser<string>("tglpkp", string.Empty), "yyyyMMdd", out date))
         {
             dateJs = Functional.DateToJson(date);
         }

        if (isAdd)
        {
          string sd = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                     'c_pkpno' : '{1}',  
                     'v_nama': '{2}',
                     'v_telepon1': '{3}',
                     'v_fax1': '{4}',
                     'l_aktif': {5}
                     }}));{0}.commitChanges();", storeId,
(respon.Values.GetValueParser<string>("pkpno", string.Empty) == null ? "" : respon.Values.GetValueParser<string>("pkpno", string.Empty)),
                    txPKPNameHdr.Text, 
                    txPhone1.Text,
                    txFax1.Text,
                    chkAktif.Value.ToString().ToLower()
                    );

          X.AddScript(sd);
          this.ClearEntrys();

          Functional.ShowMsgInformation("Data berhasil tersimpan.");
        }
        else
        {
          string sd = string.Format(@"var c_pkpno = {0}.findExact('c_pkpno', '{1}');
                                if(c_pkpno != -1) {{
                                  var r = {0}.getAt(c_pkpno);
                                  r.set('v_nama', '{2}');
                                  r.set('v_telepon1', '{3}');
                                  r.set('v_fax1', '{4}');
                                  r.set('l_aktif', {5});
                                  {0}.commitChanges();
                                }}", storeId, 
                                   numberId,
                                   txPKPNameHdr.Text,
                                   txPhone1.Text,
                                   txFax1.Text,
                                   chkAktif.Value.ToString().ToLower());

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
