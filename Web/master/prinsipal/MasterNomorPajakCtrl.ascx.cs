using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class master_nomorpajakCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Tambah Master Nomor Pajak";

    hfIDXno.Clear();

    txTahun.Clear();
    txTahun.Disabled = false;

    txDigit1.Clear();
    txDigit1.Disabled = false;

    txDigit2.Clear();
    txDigit2.Disabled = false;

    txAwal.Clear();
    txAwal.Disabled = false;

    txAkhir.Clear();
    txAkhir.Disabled = false;

    txCurrent.Clear();
    txCurrent.Disabled = false;
  }

  private void PopulateDetail(string pID)
  {
    ClearEntrys();

    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "IDX = @0", pID, "System.String"}
      };


    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0215", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Concat("", dicResultInfo.GetValueParser<string>("IDX", string.Empty));

        txTahun.Text = dicResultInfo.GetValueParser<string>("s_tahun", string.Empty);
        txDigit1.Text = dicResultInfo.GetValueParser<string>("c_digit1", string.Empty);
        txDigit2.Text = dicResultInfo.GetValueParser<string>("c_digit2", string.Empty);
        txAwal.Text = dicResultInfo.GetValueParser<string>("c_awal", string.Empty);
        txAkhir.Text = dicResultInfo.GetValueParser<string>("c_akhir", string.Empty);
        txCurrent.Text = dicResultInfo.GetValueParser<string>("c_current", string.Empty);

        txCurrent.Disabled = true;

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("master_nomorpajakCtrl:PopulateDetail Header - ", ex.Message));
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

    hfIDXno.Text = pID;
    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string numberID)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);


    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();


    pair.IsSet = true;
    pair.IsList = true;
    //pair.Value = numberId;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    DateTime date = DateTime.Today;

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    dic.Add("ID", pair);


    pair.DicAttributeValues.Add("idx", numberID);
    pair.DicAttributeValues.Add("tahun", txTahun.Text);
    pair.DicAttributeValues.Add("digit1", txDigit1.Text);
    pair.DicAttributeValues.Add("digit2", txDigit2.Text);
    pair.DicAttributeValues.Add("awal", txAwal.Text);
    pair.DicAttributeValues.Add("akhir", txAkhir.Text);
    pair.DicAttributeValues.Add("current", txCurrent.Text);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);

    try
    {
      varData = parser.ParserData("MasterNomorPajak", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("master_nomorpajakCtrl SaveParser : {0} ", ex.Message);
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

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        if (isAdd)
        {
          string sd = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                     'IDX' : '{1}',  
                     's_tahun' : '{2}',  
                     'c_awal': '{3}',
                     'c_akhir': '{4}',
                     'c_current': '{5}',
                     'c_digit1': '{6}',
                     'c_digit2': '{7}'
                     }}));{0}.commitChanges();", storeId,
(respon.Values.GetValueParser<string>("sub", string.Empty) == null ? "" : respon.Values.GetValueParser<string>("sub", string.Empty)),
                    txTahun.Text, txAwal.Text, txAkhir.Text, txAwal.Text, txDigit1.Text, txDigit2.Text
                    );

          X.AddScript(sd);
          this.ClearEntrys();

          Functional.ShowMsgInformation("Data berhasil tersimpan.");
        }
        else
        {
          string sd = string.Format(@"var c_nip = {0}.findExact('IDX', '{1}');
                                if(c_nip != -1) {{
                                  var r = {0}.getAt(c_nip);
                                  r.set('s_tahun', '{2}');
                                  r.set('c_awal', '{3}');
                                  r.set('c_akhir', '{4}');
                                  r.set('c_digit1', '{5}');
                                  r.set('c_digit2', '{6}');
                                  {0}.commitChanges();
                                }}", storeId, numberId,
                    txTahun.Text, txAwal.Text, txAkhir.Text, txDigit1.Text, txDigit2.Text);

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
