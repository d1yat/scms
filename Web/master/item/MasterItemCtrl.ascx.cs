using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class master_item_MasterItemCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Master Item";

    hfItemNo.Clear();

    lbKodeItem.Text = string.Empty;
    lblItem.Text = string.Empty;
    lblPrinsipal.Text = string.Empty;
    lblMeasure.Text = string.Empty;
    lblAcronim.Text = string.Empty;
    
    //lblNoAlkes.Text = string.Empty;
    txNoAlkes.Clear();
    
    lblPrice.Text = string.Empty;
    lblDisc.Text = string.Empty;
    txBox.Clear();
    lblQtyKons.Text = string.Empty;
    lblMinP.Text = string.Empty;
    lblBonus.Text = string.Empty;
    lblPembelian.Text = string.Empty;
    lblEstimasi.Text = string.Empty;
    lblVia.Text = string.Empty;
    lblTipe.Text = string.Empty;

    //cbPrincipalHdr.Clear();
    //cbPrincipalHdr.Disabled = false;

    //txItem.Clear();
    //txItem.Disabled = false;

    //txMeasure.Clear();
    //txMeasure.Disabled = false;

    //txAcronim.Clear();
    //txAcronim.Disabled = false;

    //txNoAlkes.Disabled = false;
    //txNoAlkes.Clear();

    //txPrice.Disabled = false;
    //txPrice.Clear();

    //txDisc.Disabled = false;
    //txDisc.Clear();

    //txBox.Disabled = false;
    //txBox.Clear();

    //txQtyKons.Disabled = false;
    //txQtyKons.Clear();

    //txMinP.Disabled = false;
    //txMinP.Clear();

    txMinQ.Disabled = false;
    txMinQ.Clear();

    //txBonus.Disabled = false;
    //txBonus.Clear();

    //txPembelian.Disabled = false;
    //txPembelian.Clear();

    //txEstimasi.Disabled = false;
    //txEstimasi.Clear();

    //cbViaHdr.Disabled = false;
    //cbViaHdr.Clear();

    //cbTipeProduk.Disabled = false;
    //cbTipeProduk.Clear();

    chkBerat.Disabled = true;
    chkBerat.Clear();

    chkDinkes.Disabled = true;
    chkDinkes.Clear();

    chkMultiP.Disabled = true;
    chkMultiP.Clear();

    chkPPNBM.Disabled = true;
    chkPPNBM.Clear();

    chkCombo.Disabled = true;
    chkCombo.Clear();

    chkSP.Disabled = true;
    chkSP.Clear();

    chkAktif.Disabled = true;
    chkAktif.Clear();

    cbGolongan.Disabled = false;
    cbGolongan.Clear();
    Ext.Net.Store cbGolonganStr = cbGolongan.GetStore();
    if (cbGolonganStr != null)
    {
        cbGolonganStr.RemoveAll();
    }

    taKomposisi.Disabled = false;
    taKomposisi.Clear();
  }

  private void PopulateDetail(string pID, string pName)
  {
    ClearEntrys();

    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "c_iteno = @0", pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0136", paramX);
    string tmp = null;

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Concat("Master Item - ", pName);

        lbKodeItem.Text = pID;
        lblItem.Text = pName; //dicResultInfo.GetValueParser<string>("v_itnam", string.Empty);
        lblPrinsipal.Text = dicResultInfo.GetValueParser<string>("v_nama", string.Empty);
        lblMeasure.Text = dicResultInfo.GetValueParser<string>("v_undes", string.Empty);
        lblAcronim.Text = dicResultInfo.GetValueParser<string>("v_acronim", string.Empty);
        //lblNoAlkes.Text = dicResultInfo.GetValueParser<string>("c_alkes", string.Empty);
        txNoAlkes.Text = dicResultInfo.GetValueParser<string>("c_alkes", string.Empty);

        lblPrice.Text = dicResultInfo.GetValueParser<decimal>("n_salpri", 0).ToString("N2");
        lblDisc.Text = dicResultInfo.GetValueParser<decimal>("n_disc", 0).ToString("N2");
        txBox.Text = dicResultInfo.GetValueParser<decimal>("n_box", 0).ToString("N2");
        lblQtyKons.Text = dicResultInfo.GetValueParser<decimal>("n_qtykons", 0).ToString("N2");
        lblMinP.Text = dicResultInfo.GetValueParser<decimal>("n_pminord", 0).ToString("N2");
        
        txMinQ.SetRawValue(dicResultInfo.GetValueParser<decimal>("n_qminord", 0));

        lblBonus.Text = dicResultInfo.GetValueParser<decimal>("n_bonus", 0).ToString("N2");
        lblPembelian.Text = dicResultInfo.GetValueParser<decimal>("n_beli", 0).ToString("N2");
        lblEstimasi.Text = dicResultInfo.GetValueParser<decimal>("n_estimasi", 0).ToString("N2");

        lblVia.Text = dicResultInfo.GetValueParser<string>("Via", string.Empty);
        lblTipe.Text = dicResultInfo.GetValueParser<string>("Jenis", string.Empty);

        chkBerat.Checked = dicResultInfo.GetValueParser<bool>("l_berat", false);
        
        chkDinkes.Checked = dicResultInfo.GetValueParser<bool>("l_dinkes", false);
        chkDinkes.Disabled = false;

        chkCombo.Checked = dicResultInfo.GetValueParser<bool>("l_combo", false);

        txNie.Text = dicResultInfo.GetValueParser<string>("c_nie", string.Empty);

        tmp = dicResultInfo.GetValueParser<string>("d_nie", string.Empty);
        DateTime date = Functional.JsonDateToDate(tmp);
        txDateNie.Text = date.ToString("dd-MM-yyyy");

        chkAktif.Checked = dicResultInfo.GetValueParser<bool>("l_aktif", false);

        taKomposisi.Text = dicResultInfo.GetValueParser<string>("v_komposisi", string.Empty);

        Functional.SetComboData(cbGolongan, "c_golongan", dicResultInfo.GetValueParser<string>("v_golongan", string.Empty), dicResultInfo.GetValueParser<string>("c_golongan", string.Empty));


        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("master_item_MasterItemCtrl:PopulateDetail Header - ", ex.Message));
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

    hfItemNo.Text = pID;
    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }
  
  private PostDataParser.StructureResponse SaveParser(bool isAdd, string numberId)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    DateTime date = DateTime.Today;
    
      decimal minQ = 0,
      het = 0;

    decimal.TryParse(txMinQ.Text, out minQ);
    decimal.TryParse(txHET.Text, out het);

    
    //if (!Functional.DateParser(txDateNie.RawText, "dd-MM-yyyy", out date))
    //{
    //    responseResult = new PostDataParser.StructureResponse()
    //    {
    //        IsSet = true,
    //        Message = "Format penulisan tanggal salah.",
    //        Response = PostDataParser.ResponseStatus.Failed
    //    };

    //    return responseResult;
    //}

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("ItemID", numberId);
    pair.DicAttributeValues.Add("MinQ", minQ.ToString());
    pair.DicAttributeValues.Add("HET", het.ToString());
    pair.DicAttributeValues.Add("Alkes", txNoAlkes.Text);
    pair.DicAttributeValues.Add("Dinkes", chkDinkes.Checked.ToString().ToLower());
    pair.DicAttributeValues.Add("Nie", string.IsNullOrEmpty(txNie.Text.Trim()) ? string.Empty : txNie.Text.Trim());
    pair.DicAttributeValues.Add("Tanggal", date.ToString("yyyyMMdd"));
    pair.DicAttributeValues.Add("Komposisi", taKomposisi.Text);
    pair.DicAttributeValues.Add("Golongan", string.IsNullOrEmpty(cbGolongan.SelectedItem.Value) ? null : cbGolongan.SelectedItem.Value);
    pair.DicAttributeValues.Add("Box", txBox.Text);
    

    try
    {
      varData = parser.ParserData("MasterItem", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("master_item_MasterItemCtrl SaveParser : {0} ", ex.Message);
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

  public void Initialize(bool isReadOnly)
  {
    if (isReadOnly)
    {
      btnSave.Visible = false; 
    }
  }

  public void CommandPopulate(string pID, string pidName)
  {
    PopulateDetail(pID, pidName);
  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    //string MinQ = (e.ExtraParams["MinQ"] ?? string.Empty);
    //string MinQ = (e.ExtraParams["Het"] ?? string.Empty);
    //string MinQ = (e.ExtraParams["Alkes"] ?? string.Empty);
    //string MinQ = (e.ExtraParams["Dinkes"] ?? string.Empty);

    //bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);
    bool isAdd = false;

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        //string scrpt = null;

        //string cust = (cbCustomerHdr.SelectedIndex != -1 ? cbCustomerHdr.SelectedItem.Text : string.Empty);
        //string supl = (cbPrincipalHdr.SelectedIndex != -1 ? cbPrincipalHdr.SelectedItem.Text : string.Empty);

        //string cust = (cbCustomerHdr.SelectedItem != null ? cbCustomerHdr.SelectedItem.Text : string.Empty);
        //string supl = (cbPrincipalHdr.SelectedItem != null ? cbPrincipalHdr.SelectedItem.Text : string.Empty);

        //string dateJs = null;
        //DateTime date = DateTime.Today;

        //        if (isAdd)
        //        {
        //          if (respon.Values != null)
        //          {
        //            if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
        //            {
        //              dateJs = Functional.DateToJson(date);
        //            }

        //            if (!string.IsNullOrEmpty(storeId))
        //            {
        //              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
        //                'c_plno': '{1}',
        //                'd_pldate': {2},
        //                'v_gdgdesc': '{3}',
        //                'v_cunam': '{4}',
        //                'v_nama': '{5}',
        //                'l_print': false,
        //                'l_confirm': false,
        //                'L_DO': false
        //              }}));{0}.commitChanges();", storeId, respon.Values.GetValueParser<string>("PL", string.Empty),
        //                       dateJs, hfGudangDesc.Text, cust, supl);

        //              X.AddScript(scrpt);
        //            }
        //          }
        //        }

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
