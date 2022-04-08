using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_retur_ReturCustomerSend : Scms.Web.Core.PageHandler
{
  private PostDataParser.StructureResponse SubmitParser(string doNumberID)
  {
    Scms.Web.Core.PageHandler page = this.Page as Scms.Web.Core.PageHandler;

    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = doNumberID;
    //pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", page.Nip);
    pair.DicAttributeValues.Add("ConfirmSent", "true");

    try
    {
      varData = parser.ParserData("RCIN", "ConfirmSent", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_Retur_Customer SubmitParser : {0} ", ex.Message);
    }

    string result = null;

    if (!string.IsNullOrEmpty(varData))
    {
      Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

      result = soa.PostData(varData);
      //result = null;

      responseResult = parser.ResponseParser(result);
    }

    return responseResult;
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  public void SubmitMethod(string rcNumber)
  {
    if (!this.IsAllowEdit)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menyimpan data.");
      return;
    }

    if (string.IsNullOrEmpty(rcNumber))
    {
      Functional.ShowMsgWarning("Nomor RC tidak terbaca.");

      return;
    }

    PostDataParser.StructureResponse respon = SubmitParser(rcNumber);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
//        string scrpt = null;

//        scrpt = string.Format(@"var vIdx = {0}.findExact('c_rcno', '{1}'); 
//                if(vIdx != -1) {{
//                  var r = {0}.getAt(vIdx);
//                  if(!Ext.isEmpty(r)) {{
//                    r.set('l_sent', true);
//                    r.commit();
//                  }}
//                }}", gridMain.GetStore().ClientID, rcNumber);

//        X.AddScript(scrpt);

        Functional.ShowMsgInformation("Data RC berhasil terproses.");
      }
      else
      {
        Functional.ShowMsgWarning(respon.Message);
      }
    }
    else
    {
      Functional.ShowMsgWarning("Unknown response");
    }

  }

  private PostDataParser.StructureResponse SubmitParser(Dictionary<string, string>[] listNum, string sGudang, bool isAdd)
  {
    Scms.Web.Core.PageHandler page = this.Page as Scms.Web.Core.PageHandler;

    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = sGudang;
    //pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", page.Nip);
    pair.DicAttributeValues.Add("ConfirmSent", "true");

    try
    {
      varData = parser.ParserData("RCIN", "ConfirmSent", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_Retur_Customer SubmitParser : {0} ", ex.Message);
    }

    string result = null;

    if (!string.IsNullOrEmpty(varData))
    {
      Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

      result = soa.PostData(varData);
      //result = null;

      responseResult = parser.ResponseParser(result);
    }

    return responseResult;
  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SubmitSelection(object sender, DirectEventArgs e)
  {
    string json = e.ExtraParams["Values"];
    string tipeBtn = e.ExtraParams["BtnTipe"];
    string katcol = e.ExtraParams["katcol"];

    Dictionary<string, string>[] lstNTrans = JSON.Deserialize<Dictionary<string, string>[]>(json);

    System.Text.StringBuilder sb = new System.Text.StringBuilder();

    bool addHeader = true;
    int i = 0;

    //TIPE_DAYS = katcol;

    bool isAdd = (tipeBtn == "1" ? true : false);


    if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    PostDataParser.StructureResponse respon = SubmitParser(lstNTrans, hfGudang.Text, isAdd);

    //string scrpt = null,
    //  scrpt1 = null;

    if (respon.IsSet)
    {
      //scrpt1 = string.Format(@"{0}.removeAll(); {0}.reload();", gridMain.ClientID);
      //X.AddScript(scrpt1);

      Functional.ShowMsg(respon.Message);
    }
    else
    {
      e.ErrorMessage = "Unknown response";
      e.Success = false;
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }
}
