using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class master_pelanggan_MasterPelangganCtrl : System.Web.UI.UserControl
{
  private void ClearEntrys()
  {
    winDetail.Title = "Master Cabang";

    hfNoCus.Clear();

    txAccount.Clear();
    txAccount.Disabled = false;

    txAddr1.Clear();
    txAddr1.Disabled = false;

    txAddr2.Clear();
    txAddr2.Disabled = false;

    txAddrTax1.Clear();
    txAddrTax1.Disabled = false;

    txAddrTax2.Clear();
    txAddrTax2.Disabled = false;

    txFax.Clear();
    txFax.Disabled = false;

    txFee.Clear();
    txFee.Disabled = false;

    txKodeCab.Clear();
    txKodeCab.Disabled = false;

    txKota.Clear();
    txKota.Disabled = false;

    txKotaTax.Clear();
    txKotaTax.Disabled = false;

    txLimit.Clear();
    txLimit.Disabled = false;

    txName.Clear();
    txName.Disabled = false;

    txNmBank.Clear();
    txNmBank.Disabled = false;

    txDaysInt.Clear();
    txDaysInt.Disabled = false;

    txDaysEksp.Clear();
    txDaysEksp.Disabled = false;

    txNPKP.Clear();
    txNPKP.Disabled = false;

    txNPWP.Clear();
    txNPWP.Disabled = false;

    txPemilik.Clear();
    txPemilik.Disabled = false;

    txPos.Clear();
    txPos.Disabled = false;

    txPosTax.Clear();
    txPosTax.Disabled = false;

    txTagih.Clear();
    txTagih.Disabled = false;

    txTaxName.Clear();
    txTaxName.Disabled = false;

    txTelp1.Clear();
    txTelp1.Disabled = false;

    txTelp2.Clear();
    txTelp2.Disabled = false;

    txTOP.Clear();
    txTOP.Disabled = false;

    txTOPPjg.Clear();
    txTOPPjg.Disabled = false;

    chkAskes.Clear();
    chkAskes.Disabled = false;

    //chkCabang.Clear();
    //chkCabang.Disabled = false;

    chkDispen.Clear();
    chkDispen.Disabled = false;

    chkHide.Clear();
    chkHide.Disabled = false;

    chkMaterai.Clear();
    chkMaterai.Disabled = false;

    chkStatus.Clear();
    chkStatus.Disabled = false;

    cbGudang.Clear();
    cbGudang.Disabled = false;

    cbSektor.Clear();
    cbSektor.Disabled = false;

    dtNPKP.Clear();
    dtNPKP.Disabled = false;
    
    btnApprove.Hidden = true;
    btnCancel.Hidden = true;

    X.AddScript(string.Format("{0}.getForm().reset();", frmHeaders.ClientID));
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  private void PopulateDetail(string pName, string pID, string PageCode)
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

      string[][] paramX2 = new string[][] { 
        new string[] { "kd_approval + NIP = @0", PageCode + pag.Nip, "System.String"}
    };

    Scms.Web.Core.PageHandler pag2 = this.Page as Scms.Web.Core.PageHandler;

      string[][] paramX3 = new string[][] { 
        new string[] { "kd_approval + c_cusno = @0", PageCode + pID, "System.String"}
    };


    #region Parser Header

      #region cek hak approval
      string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0155", paramX2);
      string lvl_approval = null;
        try
        {
            dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
            if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
            {
                jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);
                dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());
                lvl_approval = (dicResultInfo.ContainsKey("lv_approval") ? dicResultInfo["lv_approval"] : string.Empty);
            }
            else
            {
                btnApprove.Hidden = true;
                btnCancel.Hidden = true;
                btnSimpan.Hidden = true;
                btnReload.Hidden = true;

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

      #region cek status approval
        res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0156", paramX3);
        try
        {
            dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
            if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
            {
                jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);
                dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());
                string cek = (dicResultInfo.ContainsKey("status") ? dicResultInfo["status"] : string.Empty);

                if (lvl_approval != null)
                {
                    if (lvl_approval == "1")
                    {
                        if (cek == "waiting")
                        {
                            btnApprove.Hidden = false;
                            btnCancel.Hidden = false;
                            btnSimpan.Hidden = true;
                            btnReload.Hidden = true;
                        }
                        else if (cek == "approve")
                        {
                            btnApprove.Hidden = true;
                            btnCancel.Hidden = true;
                            btnSimpan.Hidden = true;
                            btnReload.Hidden = true;
                        }
                    }
                    else if (lvl_approval == "2")
                    {
                        if (cek == "waiting")
                        {
                            btnApprove.Hidden = true;
                            btnCancel.Hidden = true;
                            btnSimpan.Hidden = true;
                            btnReload.Hidden = true;
                        }
                        else if (cek == "approve")
                        {
                            btnApprove.Hidden = true;
                            btnCancel.Hidden = true;
                            btnSimpan.Hidden = false;
                            btnReload.Hidden = false;
                        }
                    }
                }
                else
                {
                    btnApprove.Hidden = true;
                    btnCancel.Hidden = true;
                    btnSimpan.Hidden = true;
                    btnReload.Hidden = true;
                }
            }
            else
            {
                if (lvl_approval == "2")
                {
                    btnApprove.Hidden = true;
                    btnCancel.Hidden = true;
                    btnSimpan.Hidden = false;
                    btnReload.Hidden = false;
                }
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

      #region load data
        res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0142", paramX);

        try
        {
          dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
          if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
          {
            jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

            dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

            DateTime date = DateTime.MinValue;
            date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_npkp"));

            DateTime dateOpen = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_buka"));
            DateTime dateClose = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_tutup"));

            winDetail.Title = string.Format("Master Cabang - {0}", pID);

            Functional.SetComboData(cbGudang, "c_gdg", dicResultInfo.GetValueParser<string>("v_gdgdesc", string.Empty), dicResultInfo.GetValueParser<string>("c_gdg", string.Empty));
            cbGudang.Disabled = false;

            Functional.SetComboData(cbSektor, "c_type", dicResultInfo.GetValueParser<string>("v_ket", string.Empty), dicResultInfo.GetValueParser<string>("c_sektor", string.Empty));
            cbSektor.Disabled = false;
            cbSektor.Value = "01";
            cbSektor.Text = "Cabang";

            txName.Text = (dicResultInfo.ContainsKey("v_cunam") ? dicResultInfo["v_cunam"] : string.Empty);
            txAccount.Text = (dicResultInfo.ContainsKey("v_accno") ? dicResultInfo["v_accno"] : string.Empty);
            txAddr1.Text = (dicResultInfo.ContainsKey("v_adrbill1") ? dicResultInfo["v_adrbill1"] : string.Empty);
            txAddr2.Text = (dicResultInfo.ContainsKey("v_adrbill2") ? dicResultInfo["v_adrbill2"] : string.Empty);
            txAddrTax1.Text = (dicResultInfo.ContainsKey("v_adrtax1") ? dicResultInfo["v_adrtax1"] : string.Empty);
            txAddrTax2.Text = (dicResultInfo.ContainsKey("v_adrtax2") ? dicResultInfo["v_adrtax2"] : string.Empty);
            txFax.Text = (dicResultInfo.ContainsKey("v_fax") ? dicResultInfo["v_fax"] : string.Empty);
            txFee.Text = dicResultInfo.GetValueParser<decimal>("n_fee").ToString(); //(dicResultInfo.ContainsKey("n_fee") ? dicResultInfo["n_fee"] : "0");
            txKodeCab.Text = (dicResultInfo.ContainsKey("c_cab") ? dicResultInfo["c_cab"] : string.Empty);
            txKota.Text = (dicResultInfo.ContainsKey("v_citybill") ? dicResultInfo["v_citybill"] : string.Empty);
            txKotaTax.Text = (dicResultInfo.ContainsKey("v_citytax") ? dicResultInfo["v_citytax"] : string.Empty);
            txLimit.Text = dicResultInfo.GetValueParser<decimal>("n_crlimit").ToString();  //(dicResultInfo.ContainsKey("n_crlimit") ? dicResultInfo["n_crlimit"] : "0");
            txNmBank.Text = (dicResultInfo.ContainsKey("v_nmbank") ? dicResultInfo["v_nmbank"] : string.Empty);
            txDaysInt.Text = (dicResultInfo.ContainsKey("n_days_internal") ? dicResultInfo["n_days_internal"] : string.Empty);
            txDaysEksp.Text = (dicResultInfo.ContainsKey("n_days_ekspedisi") ? dicResultInfo["n_days_ekspedisi"] : string.Empty);
            txNPKP.Text = (dicResultInfo.ContainsKey("c_npkp") ? dicResultInfo["c_npkp"] : string.Empty);
            txNPWP.Text = (dicResultInfo.ContainsKey("c_npwp") ? dicResultInfo["c_npwp"] : string.Empty);
            txPemilik.Text = (dicResultInfo.ContainsKey("v_nmowner") ? dicResultInfo["v_nmowner"] : string.Empty);
            txPos.Text = (dicResultInfo.ContainsKey("v_zipbill") ? dicResultInfo["v_zipbill"] : string.Empty);
            txPosTax.Text = (dicResultInfo.ContainsKey("v_ziptax") ? dicResultInfo["v_ziptax"] : string.Empty);
            txTagih.Text = (dicResultInfo.ContainsKey("v_tagih") ? dicResultInfo["v_tagih"] : string.Empty);
            txTaxName.Text = (dicResultInfo.ContainsKey("v_taxname") ? dicResultInfo["v_taxname"] : string.Empty);
            txTelp1.Text = (dicResultInfo.ContainsKey("v_telp1") ? dicResultInfo["v_telp1"] : string.Empty);
            txTelp2.Text = (dicResultInfo.ContainsKey("v_telp2") ? dicResultInfo["v_telp2"] : string.Empty);
            txTOP.Text = dicResultInfo.GetValueParser<decimal>("t_top").ToString(); //(dicResultInfo.ContainsKey("t_top") ? dicResultInfo["t_top"] : "0");
            txTOPPjg.Text = dicResultInfo.GetValueParser<decimal>("t_toppjg").ToString(); //(dicResultInfo.ContainsKey("t_toppjg") ? dicResultInfo["t_toppjg"] : string.Empty);
            chkAskes.Checked = dicResultInfo.GetValueParser<bool>("l_askes", false);
            //chkCabang.Checked = dicResultInfo.GetValueParser<bool>("l_cabang", false);
            chkDispen.Checked = dicResultInfo.GetValueParser<bool>("l_dispen", false);
            chkHide.Checked = dicResultInfo.GetValueParser<bool>("l_hide", false);
            chkMaterai.Checked = dicResultInfo.GetValueParser<bool>("l_materai", false);
            chkStatus.Checked = dicResultInfo.GetValueParser<bool>("l_stscus", false);
            dtNPKP.Text = date.ToString("dd-MM-yyyy");
            dtOpen.Text = dateOpen.ToString("dd-MM-yyyy");
            dtClose.Text = dateClose.ToString("dd-MM-yyyy");
            txDaysInt.Text = (dicResultInfo.ContainsKey("n_days_internal") ? dicResultInfo["n_days_internal"] : string.Empty);
            txDaysEksp.Text = (dicResultInfo.ContainsKey("n_days_ekspedisi") ? dicResultInfo["n_days_ekspedisi"] : string.Empty);

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


    #endregion

    hfNoCus.Text = pID;
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

      PopulateDetail("c_cusno", pID, "MSCAB");
    }
  }

  public void Initialize(string storeIDGridMain)
  {
    hfStoreID.Text = storeIDGridMain;
  }

  private PostDataParser.StructureResponse ApproveCancel(string supID, string kd_approval, string status)
  {
      PageHandler pag = new PageHandler();
      PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);
      PostDataParser parser = new PostDataParser();
      IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
      PostDataParser.StructurePair pair = new PostDataParser.StructurePair();
      Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();
      Dictionary<string, object> dicResult = null;
      Dictionary<string, string> dicResultInfo = null;
      Newtonsoft.Json.Linq.JArray jarr = null;
      string param = null, param2 = null;
      pair.IsSet = true;
      pair.IsList = true;
      pair.Value = supID;
      pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
      pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
      Scms.Web.Core.PageHandler pag2 = this.Page as Scms.Web.Core.PageHandler;

      string[][] paramX3 = new string[][] { 
        new string[] { "kd_approval + c_cusno = @0", kd_approval + supID, "System.String"}
      };

      if (status == "approve")
      {

          #region cek parameter approval terakhir
          string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0156", paramX3);
          try
          {
              dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
              if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
              {
                  jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);
                  dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());
                  param = (dicResultInfo.ContainsKey("param") ? dicResultInfo["param"] : string.Empty);
                  param2 = (dicResultInfo.ContainsKey("param2") ? dicResultInfo["param2"] : string.Empty);
              }
          }
          catch (Exception Ex)
          {
              System.Diagnostics.Debug.WriteLine(
              string.Concat("MasterCustomerCtrl:PopulateDetail Header - ", Ex.Message));
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

          dic.Add("cusno", pair);
          pair.DicAttributeValues.Add("kdapproval", kd_approval);
          pair.DicAttributeValues.Add("status", status);
          pair.DicAttributeValues.Add("NIP", pag2.Nip);
          //pair.DicAttributeValues.Add("d_entry", DateTime.Now.ToString());
          pair.DicAttributeValues.Add("param", param);
          pair.DicAttributeValues.Add("param2", param2);
          string varData = null;
          bool isAdd = false;
          try
          {
               varData = parser.ParserData("ApproveRejectMasterPelanggan", (isAdd ? "Add" : "Modify"), dic);
          }
          catch (Exception ex)
          {
              Scms.Web.Common.Logger.WriteLine("Master_Divisi_PrinsipalCtrl SaveParser : {0} ", ex.Message);
          }

          string result = null;

          if (!string.IsNullOrEmpty(varData))
          {
              result = soa.PostData(varData);

              responseResult = parser.ResponseParser(result);
          }

      }
      return responseResult;
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
    int hit = 0;

    string dateOpen = dtOpen.Value.ToString();
    string dateClose = dtClose.Value.ToString();
    string dateNPKP = dtNPKP.Value.ToString();

    //if (!Functional.DateParser(dateNPKP, "yyyy-MM-ddTHH:mm:ss", out date))
    //{
    //  date = DateTime.MinValue;
    //}
    //pair.DicAttributeValues.Add("Date", date.ToString("yyyyMMddHHmmssfff"));

    string Sektor = (cbSektor.Text == string.Empty ? "01" : cbSektor.Value.ToString());
    hit = Int16.Parse(txDaysInt.Text) + Int16.Parse(txDaysEksp.Text);

    dic.Add("CustomerID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Nama",txName.Text);
    pair.DicAttributeValues.Add("Account", txAccount.Text);
    pair.DicAttributeValues.Add("Addr1", txAddr1.Text);
    pair.DicAttributeValues.Add("Addr2", txAddr2.Text);
    pair.DicAttributeValues.Add("AddrTax1", txAddrTax1.Text);
    pair.DicAttributeValues.Add("AddrTax2", txAddrTax2.Text);
    pair.DicAttributeValues.Add("Fax", txFax.Text);
    pair.DicAttributeValues.Add("Fee", txFee.Text);
    pair.DicAttributeValues.Add("Kodecab", txKodeCab.Text);
    pair.DicAttributeValues.Add("Kota", txKota.Text);
    pair.DicAttributeValues.Add("KotaTax", txKotaTax.Text);
    pair.DicAttributeValues.Add("Limit", txLimit.Text);
    pair.DicAttributeValues.Add("NmBank", txNmBank.Text);
    pair.DicAttributeValues.Add("SpDays", hit.ToString());
    pair.DicAttributeValues.Add("SpDaysInternal", txDaysInt.Text);
    pair.DicAttributeValues.Add("SpDaysEksternal", txDaysEksp.Text);
    pair.DicAttributeValues.Add("NPKP", txNPKP.Text);
    pair.DicAttributeValues.Add("NPWP", txNPWP.Text);
    pair.DicAttributeValues.Add("Pemilik", txPemilik.Text);
    pair.DicAttributeValues.Add("KdPos", txPos.Text);
    pair.DicAttributeValues.Add("KdPosTax", txPosTax.Text);
    pair.DicAttributeValues.Add("Tagih", txTagih.Text);
    pair.DicAttributeValues.Add("TaxName", txTaxName.Text);
    pair.DicAttributeValues.Add("Telp1", txTelp1.Text);
    pair.DicAttributeValues.Add("Telp2", txTelp2.Text);
    pair.DicAttributeValues.Add("TOP", txTOP.Text);
    pair.DicAttributeValues.Add("TOPPjg", txTOPPjg.Text);
    pair.DicAttributeValues.Add("isAkses", chkAskes.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("isCabang", (Sektor.Equals("01") ? "true" : "false"));
    pair.DicAttributeValues.Add("isDispen", chkDispen.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("isHide", chkHide.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("isMaterai", chkMaterai.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("isStatus", chkStatus.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("DateOpen", dateOpen);
    pair.DicAttributeValues.Add("DateClose", dateClose);
    pair.DicAttributeValues.Add("Gudang", cbGudang.Value.ToString());
    pair.DicAttributeValues.Add("DateNP", dateNPKP);
    pair.DicAttributeValues.Add("Sektor", Sektor);

    try
    {
      varData = parser.ParserData("MasterPelanngan", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("Master_Divisi_PrinsipalCtrl SaveParser : {0} ", ex.Message);
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
  protected void ApproveBtn_Click(object sender, DirectEventArgs e)
  {
      string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
      string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);

      bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

      PostDataParser.StructureResponse respon = ApproveCancel(numberId, "MSCAB", "approve");

      if (respon.IsSet)
      {
          if (respon.Response == PostDataParser.ResponseStatus.Success)
          {
              if (isAdd)
              {
                  string scrpt = null;

                  string Nama = (txName.Text == null ? string.Empty : txName.Text);
                  string Pemilik = (txPemilik.Text == null ? string.Empty : txPemilik.Text);
                  string alamat = (txAddr1.Text == null ? string.Empty : txAddr1.Text);
                  string Kota = (txKota.Text == null ? string.Empty : txKota.Text);
                  string kdPos = (txPos.Text == null ? string.Empty : txPos.Text);
                  string Telp = (txTelp1.Text == null ? txTelp1.Text : string.Empty);

                  if (!string.IsNullOrEmpty(storeId))
                  {
                      scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_cusno': '{1}',
                'v_cunam': '{2}',
                'v_nmowner': '{3}',
                'v_adrbill1': '{4}',
                'v_citybill': '{5}',
                'v_zipbill': '{6}',
                'v_telp1': '{7}',
                'l_aktif': true
              }}));{0}.commitChanges();", storeId, respon.Values.GetValueParser<string>("customerId", string.Empty),
                               Nama, Pemilik, alamat, Kota, kdPos, Telp);

                      X.AddScript(scrpt);
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

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void CancelBtn_Click(object sender, DirectEventArgs e)
  {
      string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
      string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);

      bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

      PostDataParser.StructureResponse respon = ApproveCancel(numberId, "MSCAB", "reject");

      if (respon.IsSet)
      {
          if (respon.Response == PostDataParser.ResponseStatus.Success)
          {
              if (isAdd)
              {
                  string scrpt = null;

                  string Nama = (txName.Text == null ? string.Empty : txName.Text);
                  string Pemilik = (txPemilik.Text == null ? string.Empty : txPemilik.Text);
                  string alamat = (txAddr1.Text == null ? string.Empty : txAddr1.Text);
                  string Kota = (txKota.Text == null ? string.Empty : txKota.Text);
                  string kdPos = (txPos.Text == null ? string.Empty : txPos.Text);
                  string Telp = (txTelp1.Text == null ? txTelp1.Text : string.Empty);

                  if (!string.IsNullOrEmpty(storeId))
                  {
                      scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_cusno': '{1}',
                'v_cunam': '{2}',
                'v_nmowner': '{3}',
                'v_adrbill1': '{4}',
                'v_citybill': '{5}',
                'v_zipbill': '{6}',
                'v_telp1': '{7}',
                'l_aktif': true
              }}));{0}.commitChanges();", storeId, respon.Values.GetValueParser<string>("customerId", string.Empty),
                               Nama, Pemilik, alamat, Kota, kdPos, Telp);

                      X.AddScript(scrpt);
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
        if (isAdd)
        {
          string scrpt = null;

          string Nama = (txName.Text == null ? string.Empty : txName.Text);
          string Pemilik = (txPemilik.Text == null ? string.Empty : txPemilik.Text);
          string alamat = (txAddr1.Text == null ? string.Empty : txAddr1.Text);
          string Kota = (txKota.Text == null ? string.Empty : txKota.Text);
          string kdPos = (txPos.Text == null ? string.Empty : txPos.Text);
          string Telp = (txTelp1.Text == null ? txTelp1.Text : string.Empty);
         
          if (!string.IsNullOrEmpty(storeId))
          {
            scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                'c_cusno': '{1}',
                'v_cunam': '{2}',
                'v_nmowner': '{3}',
                'v_adrbill1': '{4}',
                'v_citybill': '{5}',
                'v_zipbill': '{6}',
                'v_telp1': '{7}',
                'l_aktif': true
              }}));{0}.commitChanges();", storeId, respon.Values.GetValueParser<string>("customerId", string.Empty),
                     Nama, Pemilik, alamat, Kota, kdPos, Telp);

            X.AddScript(scrpt);
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
