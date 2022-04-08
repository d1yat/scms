using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;




  public partial class master_item_MasterItemVia : Scms.Web.Core.PageHandler
  {
    #region Private

    private PostDataParser.StructureResponse SaveParser(string tipeId, Dictionary<string, string>[] dics)
    {
      PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);


      PostDataParser parser = new PostDataParser();
      IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

      Dictionary<string, string> dicData = null;

      PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

      Dictionary<string, string> dicAttr = null;

      pair.IsSet = true;
      pair.IsList = true;
      pair.Value = tipeId;
      pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
      pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      string tmp = null,
        idx = null,
        ket = null;
      //decimal nQty = 0;
      bool isNew = false,
        isVoid = false,
        isModify = false;
      string varData = null;

      Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
      
      dic.Add("ID", pair);
      pair.DicAttributeValues.Add("Entry", pag.Nip);
      for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
      {
        tmp = nLoop.ToString();

        dicData = dics[nLoop];

        isNew = dicData.GetValueParser<bool>("l_new");
        isVoid = dicData.GetValueParser<bool>("l_void");
        isModify = dicData.GetValueParser<bool>("l_modified");

        if (isNew && (!isVoid) && (!isModify))
        {
          if (!pag.IsAllowAdd)
          {
            continue;
          }

          dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

          dicAttr.Add("New", isNew.ToString().ToLower());
          dicAttr.Add("Delete", isVoid.ToString().ToLower());
          dicAttr.Add("Modified", isModify.ToString().ToLower());

          idx = dicData.GetValueParser<string>("idx");

          if (!string.IsNullOrEmpty(idx))
          {
            dicAttr.Add("idx", idx);

            pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
            {
              IsSet = true,
              DicAttributeValues = dicAttr
            });
          }
        }
        else if ((!isNew) && isVoid && (!isModify))
        {
          if (!pag.IsAllowDelete)
          {
            continue;
          }

          dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

          dicAttr.Add("New", isNew.ToString().ToLower());
          dicAttr.Add("Delete", isVoid.ToString().ToLower());
          dicAttr.Add("Modified", isModify.ToString().ToLower());

          idx = dicData.GetValueParser<string>("idx");
          ket = dicData.GetValueParser<string>("v_ket");

          if (!string.IsNullOrEmpty(idx))
          {
            dicAttr.Add("idx", idx);

            pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
            {
              IsSet = true,
              Value = (string.IsNullOrEmpty(ket) ? "Human error" : ket),
              DicAttributeValues = dicAttr
            });
          }
        }

        dicData.Clear();
      }

      try
      {
        varData = parser.ParserData("MasterItemVia", "Modify", dic);
      }
      catch (Exception ex)
      {
        Scms.Web.Common.Logger.WriteLine("master_item_MasterItemVia SaveParser : {0} ", ex.Message);
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

    private PostDataParser.StructureResponse SaveMethodParser(string method, string typeNumber, string ket)
    {
      PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

      PostDataParser parser = new PostDataParser();
      IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

      PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

      pair.IsSet = true;
      pair.IsList = true;
      pair.Value = typeNumber;
      pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
      pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      string varData = null;

      dic.Add("ID", pair);
      pair.DicAttributeValues.Add("Entry", this.Nip);
      pair.DicAttributeValues.Add("PortalID", "9");
      pair.DicAttributeValues.Add("TransaksiID", "001");
      pair.DicAttributeValues.Add("Deskripsi", "Master Item Via");

      Dictionary<string, string> dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      switch (method)
      {
        case "Add":

          dicAttr.Add("New", "true");
          dicAttr.Add("Delete", "false");
          dicAttr.Add("Modified", "false");

          break;
        case "Select":

          dicAttr.Add("New", "false");
          dicAttr.Add("Delete", "false");
          dicAttr.Add("Modified", "true");

          break;
        case "Delete":

          dicAttr.Add("New", "false");
          dicAttr.Add("Delete", "true");
          dicAttr.Add("Modified", "false");

          break;
      }

      dicAttr.Add("TipeID", typeNumber);

      pair.DicValues.Add("0", new PostDataParser.StructurePair()
      {
        IsSet = true,
        Value = ket,
        DicAttributeValues = dicAttr
      });

      try
      {
        varData = parser.ParserData("MasterTransaksi", (method.Equals("Select", StringComparison.OrdinalIgnoreCase) ? "Modify" : method), dic);
      }
      catch (Exception ex)
      {
        Scms.Web.Common.Logger.WriteLine("master_item_MasterItemVia DeleteParser : {0} ", ex.Message);
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

    protected void Page_Load(object sender, EventArgs e)
    {
      if (!this.IsPostBack)
      {
        MasterItemViaCtrl.initialize(storeGridMasterTrx.ClientID);
      }
    }
    //[Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    //protected void SaveBtn_Click(object sender, DirectEventArgs e)
    //{
    //    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    //    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);

    //    if (string.IsNullOrEmpty(numberId))
    //    {
    //        Functional.ShowMsgError("Nomor combo tidak dapat dibaca.");
    //        return;
    //    }
    //    if ((!this.IsAllowEdit) || (!this.IsAllowAdd) || (!this.IsAllowDelete))
    //    {
    //        Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mengubah atau menambah data.");
    //        return;
    //    }

    //    Dictionary<string, string>[] gridDataCombo = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    //    PostDataParser.StructureResponse respon = SaveParser(numberId, gridDataCombo);

    //    if (respon.IsSet)
    //    {
    //        if (respon.Response == PostDataParser.ResponseStatus.Success)
    //        {
    //            //X.AddScript(string.Format(@"{0}.reload();", gridDetail.GetStore().ClientID));

    //            //Functional.ShowMsgInformation("Data berhasil tersimpan.");
    //        }
    //        else
    //        {
    //            e.ErrorMessage = respon.Message;

    //            e.Success = false;
    //        }
    //    }
    //    else
    //    {
    //        e.ErrorMessage = "Unknown response";

    //        e.Success = false;
    //    }
    //}

    protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
    {
      if (!this.IsAllowAdd)
      {
        Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
        return;
      }
      MasterItemViaCtrl.CommandPopulate(null, null, true);
    }

    //[Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    //public void SaveMethod(string cmdName, string idNumber, string keterangan)
    //{
    //    bool isDelete = false;

    //    switch (cmdName)
    //    {
    //        case "Add":
    //            if (!this.IsAllowAdd)
    //            {
    //                Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
    //                return;
    //            }
    //            break;
    //        case "Select":
    //            if (!this.IsAllowEdit)
    //            {
    //                Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menyimpan data.");
    //                return;
    //            }
    //            break;
    //        case "Delete":
    //            if (!this.IsAllowDelete)
    //            {
    //                Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menghapus data.");
    //                return;
    //            }
    //            isDelete = true;
    //            break;
    //        default:
    //            Functional.ShowMsgError("Perintah tidak dikenal.");
    //            break;
    //    }

    //    PostDataParser.StructureResponse respon = SaveMethodParser(cmdName, idNumber, keterangan);

    //    if (respon.Response == PostDataParser.ResponseStatus.Success)
    //    {
    //      idNumber = respon.Values.GetValueParser<string>("TipeID");

    //      if (!string.IsNullOrEmpty(idNumber))
    //      {
    //        if (isDelete)
    //        {
    //          //X.AddScript(
    //          //  string.Format(@"var r = {0}.getById('{1}'); if(!Ext.isEmpty(r)) {{ {0}.remove(r); {0}.commitChanges(); }} {2}.removeAll(); {2}.commitChanges();",
    //          //  storeGridMasterTrx.ClientID, idNumber, gridDetail.GetStore().ClientID));
    //        }
    //        else
    //        {
    //          //                    X.AddScript(
    //          //                      string.Format(@"var rec = {0}.getById('{1}'); if(Ext.isEmpty(rec)) {{ 
    //          //  {0}.insert(0, new Ext.data.Record({{
    //          //    'c_type': '{2}',
    //          //    'v_ket': '{3}'
    //          //  }}));
    //          //}} else {{ rec.set('v_ket', '{3}'); }} {0}.commitChanges();",
    //          //                      storeGridMasterTrx.ClientID, idNumber, idNumber, keterangan));
    //        }
    //      }

    //      if (isDelete)
    //      {
    //        Functional.ShowMsgInformation(string.Format("Master Item Via '{0}' telah terhapus.", idNumber));
    //      }
    //      else
    //      {
    //        Functional.ShowMsgInformation("Data berhasil tersimpan.");
    //      }
    //    }
    //    else
    //    {
    //      Functional.ShowMsgWarning(respon.Message);
    //    }
    //}


    private PostDataParser.StructureResponse DeleteParser(string idx, string ket)
    {
      PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

      PostDataParser parser = new PostDataParser();
      IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

      PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

      Dictionary<string, string> dicAttr = null;

      pair.IsSet = true;
      pair.IsList = true;
      pair.Value = idx;
      pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
      pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      string varData = null;
      // idx = null;

      dic.Add("ID", pair);
      pair.DicAttributeValues.Add("Entry", this.Nip);
      pair.DicAttributeValues.Add("Keterangan", ket.Trim());

      dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      dicAttr.Add("Delete", true.ToString().ToLower());

      if ((!string.IsNullOrEmpty(idx)))
      {
        dicAttr.Add("idx", idx);

        pair.DicValues.Add("0", new PostDataParser.StructurePair()
        {
          IsSet = true,
          DicAttributeValues = dicAttr
        });
      }

      try
      {
        varData = parser.ParserData("MasterItemVia", "Modify", dic);
      }
      catch (Exception ex)
      {
        Scms.Web.Common.Logger.WriteLine("Master_Item_MasterItemVia DeleteParser : {0} ", ex.Message);
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

    [DirectMethod(ShowMask = true)]
    public void DeleteMethod(string idx, string keterangan)
    {
      if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowDelete)
      {
        Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menghapus data.");
        return;
      }

      if (string.IsNullOrEmpty(idx))
      {
        Functional.ShowMsgWarning("Nomor Item tidak terbaca.");

        return;
      }
      else if (string.IsNullOrEmpty(keterangan))
      {
        Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");

        return;
      }

      PostDataParser.StructureResponse respon = DeleteParser(idx, keterangan);

      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        X.AddScript(
          string.Format("var r = {0}.getById({1});if(!Ext.isEmpty(r)) {{ {0}.remove(r);{0}.commitChanges(); }}",
          storeGridMasterTrx.ClientID, idx));

        string sd = string.Format("var r = {0}.getById({1});if(!Ext.isEmpty(r)) {{ {0}.remove(r);{0}.commitChanges(); }}",
            storeGridMasterTrx.ClientID, idx);

        Functional.ShowMsgInformation(string.Format("Via telah dihapus."));
      }
      else
      {
        Functional.ShowMsgWarning(respon.Message);
      }
    }
  }
 