using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class master_driver_DriverCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Tambah Driver";

    hfGdgId.Clear();

    txNip.Clear();

    txNama.Clear();
    txNama.Disabled = false;

    txNopol.Clear();
    txNopol.Disabled = false;

    chkAktif.Clear();
    chkAktif.Disabled = false;
  }

  private void PopulateDetail(string pID)
  {
    ClearEntrys();

    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "c_nip = @0", pID, "System.String"}
      };

    string tmp = null;

    bool isAktif = false;

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0210", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Concat("", dicResultInfo.GetValueParser<string>("v_nama", string.Empty));
      
        txNip.Text = dicResultInfo.GetValueParser<string>("c_nip", string.Empty);
        txNama.Text = dicResultInfo.GetValueParser<string>("v_nama", string.Empty);
        txNopol.Text = dicResultInfo.GetValueParser<string>("c_nopol", string.Empty);


        Functional.SetComboData(cbTipeJenis, "c_type", dicResultInfo.GetValueParser<string>("v_ket", string.Empty), dicResultInfo.GetValueParser<string>("c_type", string.Empty));


        tmp = dicResultInfo.GetValueParser<string>("l_aktif", string.Empty);  //cek lagi

        bool.TryParse(tmp, out isAktif);

        chkAktif.Checked = isAktif;

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("master_driver_DriverCtrl:PopulateDetail Header - ", ex.Message));
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

    hfGdgId.Text = pID;
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

    pair.DicAttributeValues.Add("Nip", txNip.Text);
    pair.DicAttributeValues.Add("Nama", txNama.Text);
    pair.DicAttributeValues.Add("Tipe", cbTipeJenis.Value.ToString());
    pair.DicAttributeValues.Add("Nopol", txNopol.Text.ToUpper());
    pair.DicAttributeValues.Add("Aktif", chkAktif.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);



    try
    {
      varData = parser.ParserData("MasterDriver", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("master_driver_DriverCtrl SaveParser : {0} ", ex.Message);
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
      cbTipeJenis.Disabled = false;
      Functional.SetComboData(cbTipeJenis, "c_type", "Internal", "01");
      txNip.Disabled = false;
    }
    else
    {
      PopulateDetail(pID);
      cbTipeJenis.Disabled = true;
      txNip.Disabled = true;
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
        if (isAdd)
        {
          string sd = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                     'c_nip' : '{1}',  
                     'v_nama': '{2}',
                     'c_nopol': '{3}',
                     'v_ket': '{4}',
                     'l_aktif': {5}
                     }}));{0}.commitChanges();", storeId,
(respon.Values.GetValueParser<string>("sub", string.Empty) == null ? "" : respon.Values.GetValueParser<string>("sub", string.Empty)),
                    txNama.Text, txNopol.Text.ToUpper(), cbTipeJenis.SelectedItem.Text,
                    chkAktif.Checked.ToString().ToLower()
                    );

          X.AddScript(sd);
          this.ClearEntrys();

          Functional.ShowMsgInformation("Data berhasil tersimpan.");
        }
        else
        {
          string sd = string.Format(@"var c_nip = {0}.findExact('c_nip', '{1}');
                                if(c_nip != -1) {{
                                  var r = {0}.getAt(c_nip);
                                  r.set('v_nama', '{2}');
                                  r.set('c_nopol', '{3}');
                                  r.set('v_ket', '{4}');
                                  r.set('l_aktif', {5});
                                  {0}.commitChanges();
                                }}", storeId, numberId,
                    txNama.Text, txNopol.Text.ToUpper(), cbTipeJenis.SelectedItem.Text,
                    chkAktif.Checked.ToString().ToLower());

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
