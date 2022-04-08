using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_retur_ReturCustomerResend : Scms.Web.Core.PageHandler
{
    #region Private

    private PostDataParser.StructureResponse SubmitParser(Dictionary<string, string>[] listNum)
    {
        Scms.Web.Core.PageHandler page = this.Page as Scms.Web.Core.PageHandler;

        PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

        PostDataParser parser = new PostDataParser();
        IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

        PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

        Dictionary<string, string> dicAttr = null;
        Dictionary<string, string> dicData = null;

        pair.IsSet = true;
        pair.IsList = true;
        pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
        pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        string varData = null,
               tmp = null,
               noTrans = null,
               cusNo = null,
               pbNo = null;
        DateTime date = DateTime.Today;

        bool isDcoreSend = false;

        string GudangId = ((Scms.Web.Core.PageHandler)this.Page).ActiveGudang;

        dic.Add("ID", pair);
        pair.DicAttributeValues.Add("Entry", page.Nip);
        pair.DicAttributeValues.Add("ConfirmReSent", "true");
        pair.DicAttributeValues.Add("Gudang", GudangId);


        if ((listNum.Length > 0) || !string.IsNullOrEmpty(hfGudang.Text))
        {
            for (int nLoop = 0, nLen = listNum.Length; nLoop < nLen; nLoop++)
            {
                tmp = nLoop.ToString();

                dicData = listNum[nLoop];

                dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);


                noTrans = dicData.GetValueParser<string>("c_rcno");
                cusNo = dicData.GetValueParser<string>("c_cusno");
                pbNo = dicData.GetValueParser<string>("v_pbbrno").ToUpper();

                #region Cek Dcore

                if (!string.IsNullOrEmpty(pbNo) && pbNo.Length == 16)
                {
                    if (pbNo.Substring(0, 2) == "PB" || pbNo.Substring(0, 2) == "TB")
                    {
                        pair.DicAttributeValues.Add("PBBR", pbNo);

                        Dictionary<string, object> dicRC = null;
                        Dictionary<string, string> dicRCInfo = null;

                        Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();
                        string[][] paramX = new string[][]{
                        new string[] { "C_PBNO", pbNo, "System.String"},
                        new string[] { "cusmas", cusNo, "System.String"}};

                        string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "a-15001", paramX);

                        try
                        {
                            dicRC = JSON.Deserialize<Dictionary<string, object>>(res);
                            if (dicRC.ContainsKey("records") && (dicRC.ContainsKey("totalRows") && (((long)dicRC["totalRows"]) > 0)))
                            {
                                isDcoreSend = false;
                            }
                            else
                            {
                                isDcoreSend = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(
                              string.Concat("transaction_sales_ReturCustomer:PopulateDetail Header - ", ex.Message));
                        }
                        finally
                        {
                            if (dicRCInfo != null)
                            {
                                dicRCInfo.Clear();
                            }
                            if (dicRC != null)
                            {
                                dicRC.Clear();
                            }
                        }
                    }
                    else
                    {
                        isDcoreSend = false;
                    }
                }
                #endregion

                if ((!string.IsNullOrEmpty(noTrans)))
                {
                    dicAttr.Add("ID", noTrans);
                    dicAttr.Add("gudang", GudangId);
                    dicAttr.Add("PBId", pbNo);
                    dicAttr.Add("DcoreSend", isDcoreSend.ToString().ToLower());

                    pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
                    {
                        IsSet = true,
                        DicAttributeValues = dicAttr
                    });
                }
            }
        }
        try
        {
            varData = parser.ParserData("RCIN", "ConfirmReSent", dic);
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

            responseResult = parser.ResponseParser(result);
        }

        return responseResult;
    }

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            hfGudang.Text = this.ActiveGudang;
        }
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

        //    PostDataParser.StructureResponse respon = SubmitParser(rcNumber);

        //    if (respon.IsSet)
        //    {
        //      if (respon.Response == PostDataParser.ResponseStatus.Success)
        //      {
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

        //        Functional.ShowMsgInformation("Data RC berhasil terproses.");
        //      }
        //      else
        //      {
        //        Functional.ShowMsgWarning(respon.Message);
        //      }
        //    }
        //    else
        //    {
        //      Functional.ShowMsgWarning("Unknown response");
        //    }

    }


    [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void SubmitSelection(object sender, DirectEventArgs e)
    {
        string json = e.ExtraParams["Values"];

        Dictionary<string, string>[] lstNTrans = JSON.Deserialize<Dictionary<string, string>[]>(json);

        if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
            return;
        }

        PostDataParser.StructureResponse respon = SubmitParser(lstNTrans);

        if (respon.IsSet)
        {
            Functional.ShowMsg(respon.Message);
        }
        else
        {
            e.ErrorMessage = "Unknown response";
            e.Success = false;
        }
    }

}
