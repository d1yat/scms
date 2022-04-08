using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_pembelian_PO_Apoteker : Scms.Web.Core.PageHandler
{
    private const string PO_OOT = "oot";
    private const string PO_OKT = "okt";
    private const string PO_PREKURSOR = "prekursor";

    protected void Page_Init(object sender, EventArgs e)
    {
        string qryString = null;

        if ((!this.IsPostBack) && (!Ext.Net.X.IsAjaxRequest))
        {
            qryString = (this.Request.QueryString.IsContainKey("mode") ? (this.Request.QueryString["mode"] ?? string.Empty).Trim() : string.Empty).ToLower();

            switch (qryString)
            {
                case PO_OKT:
                    hfTypePO.Text = "02";
                    break;
                case PO_PREKURSOR:
                    hfTypePO.Text = "07";
                    break;
                case PO_OOT:
                    hfTypePO.Text = "09";
                    break;
            }
        }
    }

  private void GetTypeName()
  {
    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    Dictionary<string, object> dicPO = null;
    Dictionary<string, string> dicPOInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    string[][] paramX = new string[][]{
        new string[] { "c_notrans = @0", "47", "System.String"},
        new string[] { "c_portal = @0", "3", "System.Char"},
        new string[] { "c_type = @0", "03", "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "2001", paramX);

    try
    {
      dicPO = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicPO.ContainsKey("records") && (dicPO.ContainsKey("totalRows") && (((long)dicPO["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicPO["records"]);

        dicPOInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        hfTypeName.Text = dicPOInfo.GetValueParser<string>("v_ket");
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_pembelian_PO_Apoteker:GetTypeName - ", ex.Message));
    }
    finally
    {
      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicPOInfo != null)
      {
        dicPOInfo.Clear();
      }
      if (dicPO != null)
      {
        dicPO.Clear();
      }
    }

    #endregion

    GC.Collect();
  }

  private PostDataParser.StructureResponse DeleteParser(string poNumber, string ket)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = poNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", this.Nip);

    pair.DicAttributeValues.Add("Keterangan", ket.Trim());

    try
    {
        varData = parser.ParserData("PurchaseOrderApoteker", "Delete", dic);
    }
    catch (Exception ex)
    {
        Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_PO_Apoteker DeleteParser : {0} ", ex.Message);
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
  public void DeleteMethod(string poNumber, string keterangan)
  {
    if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowDelete)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menghapus data.");
      return;
    }

    if (string.IsNullOrEmpty(poNumber))
    {
      Functional.ShowMsgWarning("Nomor PO tidak terbaca.");

      return;
    }
    else if (string.IsNullOrEmpty(keterangan))
    {
      Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");

      return;
    }

    PostDataParser.StructureResponse respon = DeleteParser(poNumber, keterangan);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      string sd = string.Format(@"var c_pono = {0}.findExact('c_pono', '{1}');
                                if(c_pono != -1) {{
                                  var r = {0}.getAt(c_pono);
                                  r.set('c_poktno', '');
                                  r.set('l_status_pok', false);
                                  {0}.commitChanges();
                                }}", storeGridPO.ClientID,
                                   poNumber);

      X.AddScript(sd);

      Functional.ShowMsgInformation(string.Format("Nomor PO '{0}' telah terhapus.", poNumber));
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
    if (!this.IsPostBack)
    {
      GetTypeName();

      PO_ApotekerCtrl1.Initialize(storeGridPO.ClientID, hfTypeName.Text);
    }
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
        PO_ApotekerCtrl1.CommandPopulate(false, pID, hfTypePO.Text);
    }

    GC.Collect();
  }
}
