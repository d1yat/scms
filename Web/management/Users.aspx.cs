using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Core;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class management_Users : Scms.Web.Core.PageHandler
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "User Detail";

    hfNip.Clear();

    txNip.Clear();
    txNip.Disabled = false;
    
    txNama.Clear();
    
    txPassword.Clear();
    txPassword.Disabled = false;

    cbGC.Clear();
    cbGC.Disabled = false;

    chkAktif.Checked = false;
  }
  
  private void PopulateDetail(string pName, string pID)
  {
    ClearEntrys();

    Dictionary<string, object> dicUser = null;
    Dictionary<string, string> dicUserInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
      };

    string tmp = null;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "100001", paramX);

    try
    {
      dicUser = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicUser.ContainsKey("records") && (dicUser.ContainsKey("totalRows") && (((long)dicUser["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicUser["records"]);

        dicUserInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = "User Detail";

        hfNip.Text = txNip.Text = pID;
        txNip.Disabled = true;

        txNama.Text = (dicUserInfo.ContainsKey("v_username") ? dicUserInfo["v_username"] : string.Empty);
        
        txPassword.Text = "password";
        txPassword.Disabled = true;

        //cbGC.ToBuilder().AddItem(
        //  (dicUserInfo.ContainsKey("v_gdgdesc") ? dicUserInfo["v_gdgdesc"] : string.Empty),
        //  (dicUserInfo.ContainsKey("c_gdg") ? dicUserInfo["c_gdg"] : string.Empty)
        //);
        //if (cbGC.GetStore() != null)
        //{
        //  cbGC.GetStore().CommitChanges();
        //}
        //cbGC.SetValueAndFireSelect((dicUserInfo.ContainsKey("c_gdg") ? dicUserInfo["c_gdg"] : string.Empty));
        //cbGC.Disabled = true;
        Functional.SetComboData(cbGC, "c_gdg", dicUserInfo.GetValueParser<string>("v_gdgdesc", string.Empty), dicUserInfo.GetValueParser<string>("c_gdg", string.Empty));
        //cbGC.Disabled = true;

        Functional.SetComboData(cbDivPrinsipal, "c_kddivpri", dicUserInfo.GetValueParser<string>("v_divpridesc", string.Empty), dicUserInfo.GetValueParser<string>("c_gdg", string.Empty));
        //cbGC.Disabled = true;

        Functional.SetComboData(cbSuplier, "c_nosup", dicUserInfo.GetValueParser<string>("v_supdesc", string.Empty), dicUserInfo.GetValueParser<string>("c_nosup", string.Empty));
        //cbGC.Disabled = true;

        bool isAktif = false;

        tmp = (dicUserInfo.ContainsKey("l_aktif") ? dicUserInfo["l_aktif"] : string.Empty);
        if (!bool.TryParse(tmp, out isAktif))
        {
          isAktif = (tmp.Equals("1", StringComparison.OrdinalIgnoreCase) ? true : false);
        }

        chkAktif.Checked = isAktif;

      //  cbCustomerHdr.ToBuilder().AddItem(
      //    (dicPLInfo[0].ContainsKey("V_CUNAM") ? dicPLInfo[0]["V_CUNAM"] : string.Empty),
      //    (dicPLInfo[0].ContainsKey("c_cusno") ? dicPLInfo[0]["c_cusno"] : string.Empty)
      //  );
      //  if (cbCustomerHdr.GetStore() != null)
      //  {
      //    cbCustomerHdr.GetStore().CommitChanges();
      //  }
      //  cbCustomerHdr.SetValueAndFireSelect((dicPLInfo[0].ContainsKey("c_cusno") ? dicPLInfo[0]["c_cusno"] : string.Empty));
      //  cbCustomerHdr.Disabled = true;

      //  cbPrincipalHdr.ToBuilder().AddItem(
      //    (dicPLInfo[0].ContainsKey("V_NAMA") ? dicPLInfo[0]["V_NAMA"] : string.Empty),
      //    (dicPLInfo[0].ContainsKey("c_nosup") ? dicPLInfo[0]["c_nosup"] : string.Empty)
      //  );
      //  if (cbPrincipalHdr.GetStore() != null)
      //  {
      //    cbPrincipalHdr.GetStore().CommitChanges();
      //  }
      //  cbPrincipalHdr.SetValueAndFireSelect((dicPLInfo[0].ContainsKey("c_nosup") ? dicPLInfo[0]["c_nosup"] : string.Empty));
      //  cbPrincipalHdr.Disabled = true;

      //  bool isDo = false;
      //  bool isConfirm = false;
      //  bool isPrint = false;

      //  tmp = (dicPLInfo[0].ContainsKey("L_DO") ? dicPLInfo[0]["L_DO"] : string.Empty);
      //  if (!bool.TryParse(tmp, out isDo))
      //  {
      //    isDo = (tmp.Equals("1", StringComparison.OrdinalIgnoreCase) ? true : false);
      //  }

      //  tmp = (dicPLInfo[0].ContainsKey("l_confirm") ? dicPLInfo[0]["l_confirm"] : string.Empty);
      //  if (!bool.TryParse(tmp, out isConfirm))
      //  {
      //    isConfirm = (tmp.Equals("1", StringComparison.OrdinalIgnoreCase) ? true : false);
      //  }

      //  tmp = (dicPLInfo[0].ContainsKey("l_print") ? dicPLInfo[0]["l_print"] : string.Empty);
      //  if (!bool.TryParse(tmp, out isPrint))
      //  {
      //    isPrint = (tmp.Equals("1", StringComparison.OrdinalIgnoreCase) ? true : false);
      //  }

      //  txKeterangan.Text = ((dicPLInfo[0].ContainsKey("v_ket") ? dicPLInfo[0]["v_ket"] : string.Empty));
      //  cbTipeHdr.SetValueAndFireSelect((dicPLInfo[0].ContainsKey("c_type") ? dicPLInfo[0]["c_type"] : string.Empty));

      //  if (isDo || isConfirm)
      //  {
      //    txKeterangan.Disabled = true;
      //    cbTipeHdr.Disabled = true;
      //  }
      //  if (isDo || isConfirm || isPrint)
      //  {
      //    frmpnlDetailEntry.Disabled = true;
      //  }

      //  //X.Js.AddScript("alert('test');");
      //  X.AddScript(string.Format("clearRelatedComboRecursive(true, {0});", cbItemDtl.ClientID));

        jarr.Clear();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("management_Users:PopulateDetail - ", ex.Message));
    }
    finally
    {
      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicUserInfo != null)
      {
        dicUserInfo.Clear();
      }
      if (dicUser != null)
      {
        dicUser.Clear();
      }
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(bool isAddNew)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    dic.Add("Nip", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = txNip.Text.Trim()
    });
    dic.Add("Nama", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = txNama.Text.Trim()
    });
    dic.Add("Password", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = txPassword.Text.Trim()
    });
    dic.Add("GdgCus", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = (!string.IsNullOrEmpty(cbGC.SelectedItem.Value) ? cbGC.SelectedItem.Value : string.Empty)
    });
    dic.Add("NoSupl", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value) ? cbSuplier.SelectedItem.Value : string.Empty)
    });
    dic.Add("NoDivPri", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = (!string.IsNullOrEmpty(cbDivPrinsipal.SelectedItem.Value) ? cbDivPrinsipal.SelectedItem.Value : string.Empty)
    });
    dic.Add("Aktif", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = chkAktif.Checked.ToString()
    });
    dic.Add("User", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = this.Nip
    });

    string varData = null;

    try
    {
      varData = parser.ParserData("User", (isAddNew ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("management_Users SaveParser : {0} ", ex.Message);
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

  private PostDataParser.StructureResponse DeleteParser(string sNip)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    dic.Add("Nip", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = sNip.Trim()
    });
    dic.Add("User", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = this.Nip
    });

    string varData = null;

    try
    {
      varData = parser.ParserData("User", "Delete", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("management_Users DeleteParser : {0} ", ex.Message);
    }

    string result = null;

    if (!string.IsNullOrEmpty(varData))
    {
      Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

      result = soa.PostData(varData);

      responseResult = parser.ResponseParser(result);
    }

    responseResult.IsSet = true;

    return responseResult;
  }

  private PostDataParser.StructureResponse ResetPasswordParser(string nipUser, string newPass)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    string pwdKeyExchg = null,
      tempExchg = this.GetHashCode().ToString("x08");

    ScmsSoaLibraryInterface.Core.Crypto.Cryptor3DES des3 = new ScmsSoaLibraryInterface.Core.Crypto.Cryptor3DES(tempExchg, ScmsSoaLibraryInterface.Core.Crypto.GlobalCrypto.Crypt1WayMD5String(nipUser));

    pwdKeyExchg = des3.Crypt(newPass.Trim());

    dic.Add("User", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = this.Nip
    });
    dic.Add("Nip", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = nipUser.Trim()
    });
    dic.Add("Password", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = pwdKeyExchg
    });
    dic.Add("ExchangeID", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = tempExchg
    });

    string varData = null;

    try
    {
      varData = parser.ParserData("User", "ResetPassword", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("management_UserPanel SaveParser : {0} ", ex.Message);
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
  
  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  public void ResetPasswordMethod(string nipUser, string newPass)
  {
    if (!this.IsGroupAdmin)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses 'Group Admin'.");
      return;
    }
    else if (string.IsNullOrEmpty(nipUser))
    {
      Functional.ShowMsgWarning("Nip user tidak terbaca.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Nomor PL tidak terbaca.";

      return;
    }
    else if (string.IsNullOrEmpty(newPass))
    {
      Functional.ShowMsgWarning("Kata kunci baru tidak boleh kosong.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Keterangan tidak boleh kosong.";

      return;
    }

    PostDataParser.StructureResponse respon = ResetPasswordParser(nipUser, newPass);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      Functional.ShowMsgInformation(string.Format("Kata kunci Pengguna '{0}' berhasil di reset.", nipUser));
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = respon.Message;
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
  {
    ClearEntrys();

    winDetail.Hidden = false;
    winDetail.ShowModal();
  }
  
  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void btnSave_OnClick(object sender, DirectEventArgs e)
  {
    string sNip = (e.ExtraParams["NIP"] ?? string.Empty);

    bool isAddNew = (string.IsNullOrEmpty(sNip) ? true : false);

    PostDataParser.StructureResponse result = SaveParser(isAddNew);

    Ext.Net.Store store = gridMain.GetStore();

    string suplName = null,
      divPriName = null;

    switch (result.Response)
    {
      case PostDataParser.ResponseStatus.Success:
        e.Success = true;

        suplName = (string.IsNullOrEmpty(cbSuplier.SelectedItem.Text) ? string.Empty : cbSuplier.SelectedItem.Text);
        divPriName = (string.IsNullOrEmpty(cbDivPrinsipal.SelectedItem.Text) ? string.Empty : cbDivPrinsipal.SelectedItem.Text);

        if (store != null)
        {
          if (isAddNew)
          {
            IDictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("c_nip", txNip.Text.Trim());
            dic.Add("v_gdgdesc", (!string.IsNullOrEmpty(cbGC.SelectedItem.Text) ? cbGC.SelectedItem.Text : string.Empty));
            dic.Add("l_aktif", chkAktif.Checked.ToString().ToLower());
            dic.Add("v_username", txNama.Text.Trim());
            //dic.Add("c_gdg", (!string.IsNullOrEmpty(cbGC.SelectedItem.Value) ? cbGC.SelectedItem.Value : string.Empty));
            dic.Add("v_supdesc", suplName);
            dic.Add("v_divpridesc", divPriName);

            //store.InsertRecord(-1, dic, true);
            store.AddRecord(dic, true);
                        
            dic.Clear();
          }
          else
          {
            X.AddScript(string.Format(@"var idx = {0}.findExact('c_nip', '{1}');
                                    if(idx != -1) {{
                                      var rec = {0}.getAt(idx);
                                      
                                      rec.set('l_aktif', {2});
                                      rec.set('v_username', '{3}');
                                      rec.set('v_supdesc', '{4}');
                                      rec.set('v_divpridesc', '{5}');
                                      
                                      rec.commit();
                                    }};", store.ClientID, txNip.Text.Trim(),
                           chkAktif.Checked.ToString().ToLower(), txNama.Text, 
                           suplName, divPriName));
          }

        }

        Functional.ShowMsgInformation("Data berhasil tersimpan.");
        winDetail.Hide();

        break;
      case PostDataParser.ResponseStatus.Error:
        e.Success = false;
        e.ErrorMessage = result.Message;
        break;
      case PostDataParser.ResponseStatus.Failed:
        e.Success = false;
        e.ErrorMessage = result.Message;
        break;
      case PostDataParser.ResponseStatus.Unknown:
        e.Success = false;
        e.ErrorMessage = "Unknown result";
        break;
    }
  }
  
  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      PopulateDetail(pName, pID);
    }
    else if (cmd.Equals("Delete", StringComparison.OrdinalIgnoreCase))
    {
      PostDataParser.StructureResponse result = DeleteParser(pID);

      Ext.Net.Store store = gridMain.GetStore();
      
      switch (result.Response)
      {
        case PostDataParser.ResponseStatus.Success:
          e.Success = true;

          if (store != null)
          {

            X.AddScript(string.Format(@"var idx = {0}.findExact('c_nip', '{1}');
                                    if(idx != -1) {{                                                                            
                                      {0}.removeAt(idx);
                                      {0}.commitChanges();
                                    }};", store.ClientID, pID));

          }

          Functional.ShowMsgInformation("Data berhasil terhapus.");

          break;
        case PostDataParser.ResponseStatus.Error:
          e.Success = false;
          e.ErrorMessage = result.Message;
          break;
        case PostDataParser.ResponseStatus.Failed:
          e.Success = false;
          e.ErrorMessage = result.Message;
          break;
        case PostDataParser.ResponseStatus.Unknown:
          e.Success = false;
          e.ErrorMessage = "Unknown result";
          break;
      }
    }
    else
    {
      Functional.ShowMsgError("Perintah tidak dikenal.");
    }

    GC.Collect();
  }
}
