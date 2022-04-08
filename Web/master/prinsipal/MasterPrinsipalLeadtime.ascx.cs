/*
 * Created By Indra
 * 20171231FM
 * 
*/

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class master_prinsipal_MasterPrinsipalLeadtime : System.Web.UI.UserControl
{
  private void ClearEntrys()
  {
    winDetail.Title = "Master Prinsipal";

    hfNoSup.Clear();    

    txKodePemasok.Text = "";
    txNamaPemasok.Text = "";
    nmLeadtimeAwal.Text = "";
    nmLeadtimePerubahan.Text = "0";
    dtEfektiveDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
    txAlasanPerubahan.Text = "";
    txRequestor.Text = "";
    dtTglRequest.Text = DateTime.Now.ToString("dd-MM-yyyy");    
    txAlasanTolakSetuju.Text = "";

    txKodePemasok.ReadOnly = true;
    txNamaPemasok.ReadOnly = true;
    nmLeadtimeAwal.ReadOnly = true;
    dtTglRequest.ReadOnly = true;
    txAlasanTolakSetuju.ReadOnly = true;

    nmLeadtimePerubahan.ReadOnly = false;
    dtEfektiveDate.ReadOnly = false;
    txAlasanPerubahan.ReadOnly = false;

    btnSimpan.Hidden = true;
    btnReject.Hidden = true;
    btnCancel.Hidden = true;
    btnApprove.Hidden = true;
    txAlasanTolakSetuju.Hidden = true;
    

  }

  public void Initialize(string storeIDGridMain)
  {
    hfStoreID.Text = storeIDGridMain;
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  private void PopulateDetail(string pName, string pID)
  {

    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "c_nosup = @0", pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0152", paramX);

    System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("id-ID");

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        string status, nipatasan, tgl = null;

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("Master Prinsipal - {0}", pID);			        

        ClearEntrys();

        DateTime date = Functional.JsonDateToDate(tgl);

        txKodePemasok.Text = ((dicResultInfo.ContainsKey("c_nosup") ? dicResultInfo["c_nosup"] : string.Empty));

        txNamaPemasok.Text = ((dicResultInfo.ContainsKey("v_nama") ? dicResultInfo["v_nama"] : string.Empty));

        nmLeadtimeAwal.Text = ((dicResultInfo.ContainsKey("n_leadtime") ? dicResultInfo["n_leadtime"] : string.Empty));

        status = ((dicResultInfo.ContainsKey("c_status") ? dicResultInfo["c_status"] : string.Empty));

        nipatasan = ((dicResultInfo.ContainsKey("c_nipatsn") ? dicResultInfo["c_nipatsn"] : string.Empty));

        if ((status == null) || (status != "02"))
        {
            hfTypeDO.Text = "01";
            txRequestor.Text = pag.Username;
            btnSimpan.Hidden = false;
            dtEfektiveDate.MinDate = DateTime.Now;
            dtEfektiveDate.Value = DateTime.Now;
        }
        else
        {
            hfTypeDO.Text = "00";
            txRequestor.Text = ((dicResultInfo.ContainsKey("requestor") ? dicResultInfo["requestor"] : string.Empty));
            
            dtEfektiveDate.MinDate = DateTime.Now.AddDays(-100);

            dtEfektiveDate.ReadOnly = true;
            txAlasanTolakSetuju.Hidden = false;
            txAlasanTolakSetuju.ReadOnly = false;

            if (pag.Nip == nipatasan)
            {
                nmLeadtimePerubahan.Text = ((dicResultInfo.ContainsKey("n_leadtime_akhir") ? dicResultInfo["n_leadtime_akhir"] : string.Empty));
                nmLeadtimePerubahan.ReadOnly = true;

                tgl = ((dicResultInfo.ContainsKey("d_efectivedate") ? dicResultInfo["d_efectivedate"] : string.Empty));
                date = Functional.JsonDateToDate(tgl);
                dtEfektiveDate.Text = date.ToString("dd-MM-yyyy");

                txAlasanPerubahan.Text = ((dicResultInfo.ContainsKey("c_alasan_perubahan") ? dicResultInfo["c_alasan_perubahan"] : string.Empty)); ;
                txAlasanPerubahan.ReadOnly = true;

                tgl = ((dicResultInfo.ContainsKey("d_requestor") ? dicResultInfo["d_requestor"] : string.Empty));
                date = Functional.JsonDateToDate(tgl);
                dtTglRequest.Text = date.ToString("dd-MM-yyyy");

                btnApprove.Hidden = false;
                btnReject.Hidden = false;
            }

            else
            {
                nmLeadtimePerubahan.Text = ((dicResultInfo.ContainsKey("n_leadtime_akhir") ? dicResultInfo["n_leadtime_akhir"] : string.Empty));
                nmLeadtimePerubahan.ReadOnly = true;

                tgl = ((dicResultInfo.ContainsKey("d_efectivedate") ? dicResultInfo["d_efectivedate"] : string.Empty));
                date = Functional.JsonDateToDate(tgl);
                dtEfektiveDate.Text = date.ToString("dd-MM-yyyy");

                txAlasanPerubahan.Text = ((dicResultInfo.ContainsKey("c_alasan_perubahan") ? dicResultInfo["c_alasan_perubahan"] : string.Empty)); ;
                txAlasanPerubahan.ReadOnly = true;

                tgl = ((dicResultInfo.ContainsKey("d_requestor") ? dicResultInfo["d_requestor"] : string.Empty));
                date = Functional.JsonDateToDate(tgl);
                dtTglRequest.Text = date.ToString("dd-MM-yyyy");

                btnCancel.Hidden = false;
            }

            
        }
        txRequestor.ReadOnly = true;
        txRequestor.Disabled = true;

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("MasterPrinsipalCtrl:PopulateDetail Header - ", ex.Message));
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

    hfNoSup.Text = pID;
    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(string sSupID, bool isAdd, string sId)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();
    
    System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("id-ID");


    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = sSupID;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;
    
    DateTime tanggal = DateTime.MinValue;

    dic.Add("SupplierID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);    

    pair.DicAttributeValues.Add("txLeadtimeAwal", nmLeadtimeAwal.Text);

    pair.DicAttributeValues.Add("txLeadtimePerubahan", nmLeadtimePerubahan.Text);

    tanggal = DateTime.Parse(dtEfektiveDate.Text);

    pair.DicAttributeValues.Add("dtEfektiveDate", tanggal.ToString("yyyyMMdd"));   

    pair.DicAttributeValues.Add("txAlasanPerubahan", txAlasanPerubahan.Text);

    pair.DicAttributeValues.Add("txAlasanTolakSetuju", txAlasanTolakSetuju.Text);

    sId = "0" + sId;
    pair.DicAttributeValues.Add("txhftypedo", sId);

    pair.DicAttributeValues.Add("TipePerubahan", "01");   

    try
    {
      varData = parser.ParserData("MasterPrisipal", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("Master_PrinsipalCtrl SaveParser : {0} ", ex.Message);
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

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);


    #region validasi

    if ((nmLeadtimePerubahan.Text == "") || (nmLeadtimePerubahan.Text == "0"))
    {
        Functional.ShowMsgInformation("Kolom leadtime perubahan masih kosong atau 0. Ubah data dibatalkan");
        return;
    }

    if (nmLeadtimeAwal.Text == nmLeadtimePerubahan.Text)
    {
        Functional.ShowMsgInformation("Kolom leadtime awal sama dengan leadtime perubahan. Ubah data dibatalkan");
        return;
    }

    if (txAlasanPerubahan.Text.Trim() == "")
    {
        Functional.ShowMsgInformation("Kolom alasan perubahan masih kosong. Ubah data dibatalkan");
        return;
    }

    if ((txAlasanTolakSetuju.Text.Trim() == "") && (hfTypeDO.Text != "01"))
    {
        Functional.ShowMsgInformation("Kolom alasan batal/tolak/setuju masih kosong. Ubah data dibatalkan");
        return;
    }

    #endregion


    PostDataParser.StructureResponse respon = SaveParser(numberId, isAdd, storeId);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        if (isAdd)
        {
          string scrpt = null;          

          if (!string.IsNullOrEmpty(storeId))
          {
            X.AddScript(scrpt); 
          }
        }

        //this.ClearEntrys();

        Functional.ShowMsgInformation("Data berhasil tersimpan.");

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        btnSimpan.Hidden = true;
        btnCancel.Hidden = true;
        btnReject.Hidden = true;
        btnApprove.Hidden = true;

        sb.AppendFormat(@"{0}.reload(0, new Ext.data.Record({{
                    }})); ", hfStoreID.Text                             
                            );

        X.AddScript(sb.ToString());
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

  public void CommandPopulate(bool isAdd, string pID)
  {
    winDetail.Hidden = false;

    if (isAdd)
    {

    }
    else
    {      
      PopulateDetail("c_nosup", pID);
    }
  }

  protected void ReloadBtn_Click(object sender, DirectEventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

  }
}
