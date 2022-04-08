using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class master_ams_MasterUserAPJCtrl : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void Initialize(string storeIDGridMain)
    {
        hfStoreID.Text = storeIDGridMain;
    }

    private void PopulateDetail(string pName, string pID)
    {
        ClearEntrys();

        Dictionary<string, object> dicResult = null;
        Dictionary<string, string> dicResultInfo = null;
        Newtonsoft.Json.Linq.JArray jarr = null;

        Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

        string[][] paramX = new string[][]{
        new string[] { pName +" = @0", pID, "System.String"}
      };

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        #region Parser Header

        string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0302", paramX);

        try
        {
            dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
            if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
            {
                jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

                dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

                DateTime date = DateTime.MinValue;
                date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_npkp"));

                winDetail.Title = string.Format("Master Cabang - {0}", pID);

                Functional.SetComboData(cbCustomer, "c_cusno", dicResultInfo.GetValueParser<string>("v_cunam", string.Empty), dicResultInfo.GetValueParser<string>("c_cusno", string.Empty));
                cbCustomer.Disabled = true;

                hfCabang.Text = dicResultInfo.GetValueParser<string>("c_cusno", string.Empty);

                Functional.SetComboData(cbGudang, "c_gdg", dicResultInfo.GetValueParser<string>("v_gdgdesc", string.Empty), dicResultInfo.GetValueParser<string>("c_gdg", string.Empty));
                cbGudang.Disabled = false;

                txNip.Text = (dicResultInfo.ContainsKey("c_nip") ? dicResultInfo["c_nip"] : string.Empty);
                txNip.ReadOnly = true;

                txNama.Text = (dicResultInfo.ContainsKey("v_nama") ? dicResultInfo["v_nama"] : string.Empty);
                //txCusNo.Text = (dicResultInfo.ContainsKey("c_cusno") ? dicResultInfo["c_cusno"] : string.Empty);

                txNoSik.Text = (dicResultInfo.ContainsKey("c_nosik") ? dicResultInfo["c_nosik"] : string.Empty);
                txNoPbf.Text = (dicResultInfo.ContainsKey("c_nopbf") ? dicResultInfo["c_nopbf"] : string.Empty);
                txKodeArea.Text = (dicResultInfo.ContainsKey("c_kodearea") ? dicResultInfo["c_kodearea"] : string.Empty);

                jarr.Clear();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
              string.Concat("MasterCustomerCtrl:PopulateDetail Header - ", ex.Message));
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

        hfNip.Text = pID;
        winDetail.Hidden = false;
        winDetail.ShowModal();

        GC.Collect();
    }

    public void CommandPopulate(bool isAdd, string pID)
    {
        if (isAdd)
        {
            ClearEntrys();

            winDetail.Hidden = false;
        }
        else
        {
            ClearEntrys();

            winDetail.Hidden = false;

            PopulateDetail("c_nip", pID);
        }
    }

    private string UploadData(string fileName, byte[] byts)
    {
        string result = null;

        Scms.Web.Core.SoaCaller soa = null;

        string[][] paramX = new string[][] { };

        if ((!string.IsNullOrEmpty(fileName)) && (byts != null) && (byts.Length > 0))
        {
            soa = new Scms.Web.Core.SoaCaller();

            result = soa.GlobalUploadData("up110", fileName, byts, paramX);
        }

        return result;
    }

    private PostDataParser.StructureResponse SaveParser(string sDivSupID, bool isAdd)
    {
        PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

        PostDataParser parser = new PostDataParser();
        IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

        PostDataParser.StructurePair pair = new PostDataParser.StructurePair();


        pair.IsSet = true;
        pair.IsList = true;
        pair.Value = sDivSupID;
        pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
        pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        string varData = null;
        DateTime date = DateTime.MinValue;

        //if (!Functional.DateParser(dateNPKP, "yyyy-MM-ddTHH:mm:ss", out date))
        //{
        //  date = DateTime.MinValue;
        //}
        //pair.DicAttributeValues.Add("Date", date.ToString("yyyyMMddHHmmssfff"));


        string cusNo = cbCustomer.Value == null ? string.Empty : cbCustomer.Value.ToString();

        dic.Add("NipID", pair);
        pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
        pair.DicAttributeValues.Add("Nip", txNip.Text);
        pair.DicAttributeValues.Add("Nama", txNama.Text);
        pair.DicAttributeValues.Add("CusNo", cusNo);
        pair.DicAttributeValues.Add("NoSik", txNoSik.Text);
        //pair.DicAttributeValues.Add("Images", txImage.Text);
        pair.DicAttributeValues.Add("NoPbf", txNoPbf.Text);
        pair.DicAttributeValues.Add("KodeArea", txKodeArea.Text);
        pair.DicAttributeValues.Add("Gudang", cbGudang.Value.ToString());
        try
        {
            varData = parser.ParserData("MasterUserApj", (isAdd ? "Add" : "Modify"), dic);
        }
        catch (Exception ex)
        {
            Scms.Web.Common.Logger.WriteLine("Master_User_ApjCtrl SaveParser : {0} ", ex.Message);
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

        PostDataParser.StructureResponse respon = SaveParser(numberId, isAdd);

        if (respon.IsSet)
        {
            if (respon.Response == PostDataParser.ResponseStatus.Success)
            {
                string Nip = (txNip.Text == null ? string.Empty : txNip.Text);
                string Nama = (txNama.Text == null ? string.Empty : txNama.Text);
                string KodeCab = (cbCustomer.Value == null ? (hfCabang.Text) : cbCustomer.Value.ToString());
                string NoSik = (txNoSik.Text == null ? string.Empty : txNoSik.Text);
                string NoPbf = (txNoPbf.Text == null ? string.Empty : txNoPbf.Text);
                string kodeArea = (txKodeArea.Text == null ? string.Empty : txNoPbf.Text);
                string Gudang = (cbGudang.SelectedItem == null ? cbGudang.Text : cbGudang.SelectedItem.Text);

                if (isAdd)
                {
                    if (fuImage.HasFile)
                    {
                        string result = UploadData(fuImage.FileName, fuImage.FileBytes);

                        if (string.IsNullOrEmpty(result))
                        {
                            e.ErrorMessage = "Unknown response";

                            e.Success = false;
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        Functional.ShowMsgWarning("File tidak terbaca atau tidak ada.");
                    }

                    string scrpt = null;

                    
                    if (!string.IsNullOrEmpty(storeId))
                    {
                        scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_nip': '{1}',
                'v_nama': '{2}',
                'c_cusno': '{3}',
                'c_nosik': '{4}',
                'c_nopbf': '{5}',
                'c_kodearea': '{6}',
                'v_gdgdesc': '{7}'
              }}));{0}.commitChanges();", storeId, Nip, Nama, KodeCab, NoSik, NoPbf, kodeArea, Gudang);

                        X.AddScript(scrpt);
                    }
                }
                else
                {
                     string scrpt = string.Format(@"var rec = {0}.getById('{1}');
                                  if(!Ext.isEmpty(rec)) {{
                                    rec.set('c_nip', '{2}');
                                    rec.set('v_nama', {3});
                                    rec.set('c_cusno', {4});
                                    rec.set('c_nosik', {5});
                                    rec.set('c_nopbf', {6});
                                    rec.set('c_kodearea', {7});
                                    rec.set('v_gdgdesc', {8});
                                  }};{0}.commitChanges();",
                                          storeId, numberId, Nip, Nama,
                                          hfCabang.Text,
                                          NoSik,
                                          NoPbf,
                                          kodeArea,
                                          Gudang);

                    X.AddScript(scrpt);
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

    private void ClearEntrys()
    {
        winDetail.Title = "Master User APJ";

        hfNip.Clear();

        txNip.Clear();
        txNip.Disabled = false;
        txNip.ReadOnly = false;

        txNama.Clear();
        txNama.Disabled = false;
       

        cbCustomer.Clear();
        cbCustomer.Disabled = false;

        //txCusNo.Clear();
        //txCusNo.Disabled = false;

        txNoSik.Clear();
        txNoSik.Disabled = false;

        hfCabang.Clear();

        txNoPbf.Clear();
        txNoPbf.Disabled = false;

        txKodeArea.Clear();
        txKodeArea.Disabled = false;

        cbGudang.Clear();
        cbGudang.Disabled = false;

        X.AddScript(string.Format("{0}.getForm().reset();", frmHeaders.ClientID));
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
