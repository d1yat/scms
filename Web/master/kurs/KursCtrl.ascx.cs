using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class master_kurs_KursCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Mata uang";

    hfKursId.Clear();

    txSimbol.Clear();
    txSimbol.Disabled = false;

    txDesc.Clear();
    txDesc.Disabled = false;

    txDescFull.Clear();
    txDescFull.Disabled = false;

    txCurr.Clear();
    txCurr.Disabled = false;

    //txRW.Clear();
    //txRW.Disabled = false;

    //txLurah.Clear();
    //txLurah.Disabled = false;

    //txCamat.Clear();
    //txCamat.Disabled = false;

    //cbKota.Clear();
    //cbKota.Disabled = false;

    //txKodePos.Clear();
    //txKodePos.Disabled = false;

    //txTelp.Clear();
    //txTelp.Disabled = false;

    chkAktif.Clear();
    chkAktif.Disabled = false;
    chkAktif.Hidden = false;
  }

  private void PopulateDetail(string pID)
  {
    ClearEntrys();

    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { "c_kurs = @0", pID, "System.String"}
      };

    string tmp = null;

    bool isAktif = false;

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "2071", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Concat("Mata uang - ", dicResultInfo.GetValueParser<string>("v_desc", string.Empty));

        txSimbol.Text = this.Server.HtmlDecode(dicResultInfo.GetValueParser<string>("c_symbol", string.Empty)); //string.Empty;
        //lbSimbol.Html = dicResultInfo.GetValueParser<string>("c_symbol", string.Empty);

        txDesc.Text = dicResultInfo.GetValueParser<string>("c_desc", string.Empty);
        txDescFull.Text = dicResultInfo.GetValueParser<string>("v_desc", string.Empty);
        txCurr.Text = dicResultInfo.GetValueParser<decimal>("n_currency").ToString();
        //txRW.Text = dicResultInfo.GetValueParser<string>("c_rw", string.Empty);
        //txLurah.Text = dicResultInfo.GetValueParser<string>("v_lurah", string.Empty);
        //txCamat.Text = dicResultInfo.GetValueParser<string>("v_camat", string.Empty);
        
        tmp = dicResultInfo.GetValueParser<string>("l_aktif", string.Empty);

        bool.TryParse(tmp, out isAktif);

        chkAktif.Checked = isAktif;

        //Functional.SetComboData(cbKota, "c_kota", dicResultInfo.GetValueParser<string>("v_kota_desc", string.Empty), dicResultInfo.GetValueParser<string>("c_kota", string.Empty));
        
        //txKodePos.Text = dicResultInfo.GetValueParser<string>("c_kodepos", string.Empty);        
        //txTelp.Text = dicResultInfo.GetValueParser<string>("v_telp", string.Empty);

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("master_kurs_KursCtrl:PopulateDetail Header - ", ex.Message));
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

    hfKursId.Text = pID;
    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }
  
  private PostDataParser.StructureResponse SaveParser(bool isAdd, string kursId)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("Kurs", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = kursId.Trim()
    });
    dic.Add("Simbol", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = this.Server.HtmlDecode(txSimbol.Text.Trim())
    });
    dic.Add("Deskripsi", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = txDesc.Text.Trim()
    });
    dic.Add("DeskripsiFull", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = txDescFull.Text.Trim()
    });
    dic.Add("Pecahan", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = txCurr.Text.Trim()
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
      varData = parser.ParserData("MasterKurs", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("master_kurs_KursCtrl SaveParser : {0} ", ex.Message);
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
          numberId = respon.Values.GetValueParser<string>("Kurs", string.Empty);

          scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                          'c_kurs': '{1}',
                          'c_symbol': '{2}',
                          'c_desc': '{3}',
                          'v_desc': '{4}',
                          'n_currency': '{5}',
                          'l_aktif': {6}
                        }}));{0}.commitChanges();", storeId, numberId,
                   txSimbol.Text.Trim(), txDesc.Text.Trim(), 
                   txDescFull.Text.Trim(), txCurr.Text.Trim(),
                   chkAktif.Checked.ToString().ToLower());

          hfKursId.Text = numberId;

          winDetail.Title = string.Concat("Mata uang - ", txDescFull.Text.Trim());
        }
        else
        {
          scrpt = string.Format(@"var idx = {0}.findExact('c_kurs', '{1}');
                      if(idx != -1) {{
                        var r = {0}.getAt(idx);
                        r.set('c_symbol', '{2}');
                        r.set('c_desc', '{3}');
                        r.set('v_desc', '{4}');
                        r.set('n_currency', '{5}');
                        r.set('l_aktif', {6});
                        {0}.commitChanges();
                      }}", storeId, numberId, txSimbol.Text.Trim(),
                      txDesc.Text.Trim(), txDescFull.Text.Trim(),
                      txCurr.Text.Trim(), chkAktif.Checked.ToString().ToLower());
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
