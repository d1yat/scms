using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class master_gudang_GudangCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Gudang";

    hfGdgId.Clear();

    txDesc.Clear();
    txDesc.Disabled = false;

    txNama.Clear();
    txNama.Disabled = false;

    txAlamat.Clear();
    txAlamat.Disabled = false;

    txRT.Clear();
    txRT.Disabled = false;

    txRW.Clear();
    txRW.Disabled = false;

    txLurah.Clear();
    txLurah.Disabled = false;

    txCamat.Clear();
    txCamat.Disabled = false;

    cbKota.Clear();
    cbKota.Disabled = false;

    txKodePos.Clear();
    txKodePos.Disabled = false;

    txTelp.Clear();
    txTelp.Disabled = false;

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
        new string[] { "c_gdg = @0", pID, "System.Char"}
      };

    string tmp = null;

    bool isAktif = false;

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "2032", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Concat("Gudang - ", dicResultInfo.GetValueParser<string>("v_gdgdesc", string.Empty));

        txDesc.Text = dicResultInfo.GetValueParser<string>("v_gdgdesc", string.Empty);
        txNama.Text = dicResultInfo.GetValueParser<string>("v_nama", string.Empty);
        txAlamat.Text = dicResultInfo.GetValueParser<string>("v_alamat", string.Empty);
        txRT.Text = dicResultInfo.GetValueParser<string>("c_rt", string.Empty);
        txRW.Text = dicResultInfo.GetValueParser<string>("c_rw", string.Empty);
        txLurah.Text = dicResultInfo.GetValueParser<string>("v_lurah", string.Empty);
        txCamat.Text = dicResultInfo.GetValueParser<string>("v_camat", string.Empty);

        Functional.SetComboData(cbKota, "c_kota", dicResultInfo.GetValueParser<string>("v_kota_desc", string.Empty), dicResultInfo.GetValueParser<string>("c_kota", string.Empty));
        
        txKodePos.Text = dicResultInfo.GetValueParser<string>("c_kodepos", string.Empty);
        txTelp.Text = dicResultInfo.GetValueParser<string>("v_telp", string.Empty);

        tmp = dicResultInfo.GetValueParser<string>("l_aktif", string.Empty);

        bool.TryParse(tmp, out isAktif);

        chkAktif.Checked = isAktif;

        //lblItem.Text = dicResultInfo.GetValueParser<string>("v_itnam", string.Empty);
        //lblPrinsipal.Text = dicResultInfo.GetValueParser<string>("v_nama", string.Empty);
        //lblMeasure.Text = dicResultInfo.GetValueParser<string>("v_undes", string.Empty);
        //lblAcronim.Text = dicResultInfo.GetValueParser<string>("v_acronim", string.Empty);
        //lblNoAlkes.Text = dicResultInfo.GetValueParser<string>("c_alkes", string.Empty);

        //lblPrice.Text = dicResultInfo.GetValueParser<decimal>("n_salpri", 0).ToString("N2");
        //lblDisc.Text = dicResultInfo.GetValueParser<decimal>("n_disc", 0).ToString("N2");
        //lblBox.Text = dicResultInfo.GetValueParser<decimal>("n_box", 0).ToString("N2");
        //lblQtyKons.Text = dicResultInfo.GetValueParser<decimal>("n_qtykons", 0).ToString("N2");
        //lblMinP.Text = dicResultInfo.GetValueParser<decimal>("n_pminord", 0).ToString("N2");
        
        //txMinQ.SetRawValue(dicResultInfo.GetValueParser<decimal>("n_qminord", 0));

        //lblBonus.Text = dicResultInfo.GetValueParser<decimal>("n_bonus", 0).ToString("N2");
        //lblPembelian.Text = dicResultInfo.GetValueParser<decimal>("n_beli", 0).ToString("N2");
        //lblEstimasi.Text = dicResultInfo.GetValueParser<decimal>("n_estimasi", 0).ToString("N2");

        //lblVia.Text = dicResultInfo.GetValueParser<string>("Via", string.Empty);
        //lblTipe.Text = dicResultInfo.GetValueParser<string>("Jenis", string.Empty);

        //chkBerat.Checked = dicResultInfo.GetValueParser<bool>("l_berat", false);
        //chkDinkes.Checked = dicResultInfo.GetValueParser<bool>("l_dinkes", false);
        //chkCombo.Checked = dicResultInfo.GetValueParser<bool>("l_combo", false);
        //chkAktif.Checked = dicResultInfo.GetValueParser<bool>("l_aktif", false);

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

    hfGdgId.Text = pID;
    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }
  
  private PostDataParser.StructureResponse SaveParser(bool isAdd, string gdgId)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("Gudang", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = gdgId.Trim()
    });
    dic.Add("Deskripsi", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = txDesc.Text.Trim()
    });
    dic.Add("Nama", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = txNama.Text.Trim()
    });
    dic.Add("Alamat", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = txAlamat.Text.Trim()
    });
    dic.Add("Rt", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = txRT.Text.Trim()
    });
    dic.Add("Rw", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = txRW.Text.Trim()
    });
    dic.Add("Lurah", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = txLurah.Text.Trim()
    });
    dic.Add("Camat", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = txCamat.Text.Trim()
    });
    dic.Add("Kota", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = (cbKota.SelectedItem != null ? cbKota.SelectedItem.Value : string.Empty)
    });
    dic.Add("Kodepos", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = txKodePos.Text.Trim()
    });
    dic.Add("Telpon", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = txTelp.Text.Trim()
    });
    dic.Add("Aktif", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = chkAktif.Checked.ToString().ToLower()
    });
    dic.Add("User", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = ((Scms.Web.Core.PageHandler)this.Page).Nip
    });
    
    try
    {
      varData = parser.ParserData("MasterGudang", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("master_gudang_GudangCtrl SaveParser : {0} ", ex.Message);
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
        string scrpt = null;

        if (isAdd)
        {
          numberId = respon.Values.GetValueParser<string>("Gudang", string.Empty);

          scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                          'c_gdg': '{1}',
                          'v_gdgdesc': '{2}',
                          'v_nama': '{3}',
                          'l_aktif': {4}
                        }}));{0}.commitChanges();", storeId, numberId,
                   txDesc.Text.Trim(), txNama.Text.Trim(), 
                   chkAktif.Checked.ToString().ToLower());

          hfGdgId.Text = numberId;

          winDetail.Title = string.Concat("Gudang - ", txDesc.Text.Trim());
        }
        else
        {
          scrpt = string.Format(@"var idx = {0}.findExact('c_gdg', '{1}');
                      if(idx != -1) {{
                        var r = {0}.getAt(idx);
                        r.set('v_gdgdesc', '{2}');
                        r.set('v_nama', '{3}');
                        r.set('l_aktif', {4});
                        {0}.commitChanges();
                      }}", storeId, numberId,
                      txDesc.Text.Trim(), txNama.Text.Trim(),
                      chkAktif.Checked.ToString().ToLower());
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
