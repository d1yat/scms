using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_retur_ReturSupplier : Scms.Web.Core.PageHandler
{
  private const string RS_PEMBELIAN = "PEMBELIAN";
  private const string RN_REPACK = "REPACK";

  private const string TYPE_RS_PEMBELIAN = "01";
  private const string TYPE_RN_REPACK = "02";
  
  private void GetTypeName(string typeCode)
  {
    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    Dictionary<string, object> dicSP = null;
    Dictionary<string, string> dicSPInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    string[][] paramX = new string[][]{
        new string[] { "c_notrans = @0", "43", "System.String"},
        new string[] { "c_portal = @0", "3", "System.Char"},
        new string[] { "c_type = @0", typeCode, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "2001", paramX);

    try
    {
      dicSP = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicSP.ContainsKey("records") && (dicSP.ContainsKey("totalRows") && (((long)dicSP["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicSP["records"]);

        dicSPInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        hfTypeName.Text = dicSPInfo.GetValueParser<string>("v_ket");
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_retur_ReturSupplier:GetTypeName - ", ex.Message));
    }
    finally
    {
      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicSPInfo != null)
      {
        dicSPInfo.Clear();
      }
      if (dicSP != null)
      {
        dicSP.Clear();
      }
    }

    #endregion

    GC.Collect();
  }

  private Control MyLoadControl()
  {
    Control c = null;

    const string CONT_NORMAL = "x1x1x1";

    c = phCtrl.FindControl(CONT_NORMAL);

    string modeValue = hfMode.Text.Trim().ToUpper();

    switch (modeValue)
    {
      case RS_PEMBELIAN:

        if (c == null)
        {
          c = this.LoadControl("ReturSupplierPembelianCtrl.ascx");

          c.ID = CONT_NORMAL;

          phCtrl.Controls.Add(c);
        }

        break;
      default:

        if (c == null)
        {
          c = this.LoadControl("ReturSupplierRepackCtrl.ascx");

          c.ID = CONT_NORMAL;

          phCtrl.Controls.Add(c);
        }

        break;
    }

    return c;
  }
  
  private PostDataParser.StructureResponse DeleteParser(string rsNumber, string gudangId, string ket)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = rsNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Gudang", gudangId);
    pair.DicAttributeValues.Add("Keterangan", ket.Trim());

    try
    {
      if (hfMode.Text.Equals(RS_PEMBELIAN, StringComparison.OrdinalIgnoreCase))
      {
        varData = parser.ParserData("RSBELI", "Delete", dic);
      }
      else if (hfMode.Text.Equals(RN_REPACK, StringComparison.OrdinalIgnoreCase))
      {
        varData = parser.ParserData("RSREPACK", "Delete", dic);
      }
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_retur_ReturSupplier DeleteParser : {0} ", ex.Message);
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

  private PostDataParser.StructureResponse SaveParser(Dictionary<string, string>[] listNum, string sGudang, bool isAdd)
  {
      PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

      PostDataParser parser = new PostDataParser();
      IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);


      PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

      Dictionary<string, string> dicAttr = null;
      Dictionary<string, string> dicData = null;

      string varData = null,
        tmp = null,
        noTrans = null;

      DateTime tanggal = DateTime.MinValue;

      pair.IsSet = true;
      pair.IsList = true;
      pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
      pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      dic.Add("ID", pair);
      pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
      //pair.DicAttributeValues.Add("Gudang", sGudang);


      Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

      if ((listNum.Length > 0) || !string.IsNullOrEmpty(sGudang) || !string.IsNullOrEmpty(hfGudang.Text))
      {

          for (int nLoop = 0, nLen = listNum.Length; nLoop < nLen; nLoop++)
          {
              tmp = nLoop.ToString();

              dicData = listNum[nLoop];

              //isNew = dicData.GetValueParser<bool>("l_send");
              dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);


              noTrans = dicData.GetValueParser<string>("c_rsno");

              if ((!string.IsNullOrEmpty(noTrans)))
              {
                  dicAttr.Add("RSId", noTrans);
                  dicAttr.Add("gudang", sGudang);

                  dicAttr.Add("ConfirmDisp", true.ToString().ToLower());

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
          varData = parser.ParserData("RSDISPOSISI", "ConfirmDisposisi", dic);
      }
      catch (Exception ex)
      {
          Scms.Web.Common.Logger.WriteLine("RS Pembelian Disposisi SaveParser : {0} ", ex.Message);
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

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  public void DeleteMethod(string rsNumber, string gudangId, string keterangan)
  {
    if (string.IsNullOrEmpty(rsNumber))
    {
      Functional.ShowMsgWarning("Nomor RC tidak terbaca.");

      return;
    }
    else if (string.IsNullOrEmpty(gudangId))
    {
      Functional.ShowMsgWarning("Gudang tidak terbaca.");

      return;
    }
    else if (string.IsNullOrEmpty(keterangan))
    {
      Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");

      return;
    }

    PostDataParser.StructureResponse respon = DeleteParser(rsNumber, gudangId, keterangan);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r); {0}.commitChanges(); }}",
        storeGridRS.ClientID, rsNumber));

      Functional.ShowMsgInformation(string.Format("Nomor RC '{0}' telah terhapus.", rsNumber));
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
    }
  }

  protected void Page_Init(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      string qryString = (this.Request.QueryString["mode"] ?? string.Empty);
      hfGudang.Text = this.ActiveGudang;
      
      if (qryString.Equals("pembelian", StringComparison.OrdinalIgnoreCase))
      {
        hfMode.Text = RS_PEMBELIAN;
        hfType.Text = TYPE_RS_PEMBELIAN;
      }
      else
      {
        hfMode.Text = RN_REPACK;
        hfType.Text = TYPE_RN_REPACK;
      }

      GetTypeName(hfType.Text);
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    string modeValue = hfMode.Text.Trim().ToUpper();

    switch (modeValue)
    {
      case RS_PEMBELIAN:
        {
          transaksi_retur_ReturSupplierPembelianCtrl opr = MyLoadControl() as transaksi_retur_ReturSupplierPembelianCtrl;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek control Retur supplier Pembelian.");
          }
          else
          {
            opr.Initialize(storeGridRS.ClientID, hfTypeName.Text, this.ActiveGudang, this.ActiveGudangDescription);
          }
        }
        break;
      default:
        {
          transaksi_retur_ReturSupplierRepackCtrl opr = MyLoadControl() as transaksi_retur_ReturSupplierRepackCtrl;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek control Retur supplier Repack.");
          }
          else
          {
            opr.Initialize(storeGridRS.ClientID, hfTypeName.Text, this.ActiveGudang, this.ActiveGudangDescription);
          }
        }
        break;
    }
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);
    string gdgID = (e.ExtraParams["GudangID"] ?? string.Empty);

    string modeValue = hfMode.Text.Trim().ToUpper();

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      //PurchaseOrderCtrl1.CommandPopulate(false, pID);

      switch (modeValue)
      {
        case RS_PEMBELIAN:
          {
            transaksi_retur_ReturSupplierPembelianCtrl opr = MyLoadControl() as transaksi_retur_ReturSupplierPembelianCtrl;

            if (opr == null)
            {
              Functional.ShowMsgError("Objek kontrol Retur Pembelian tidak dapat dibuka.");
            }
            else
            {
              opr.CommandPopulate(false, pID);
            }
          }
          break;
        default:
          {
            transaksi_retur_ReturSupplierRepackCtrl opr = MyLoadControl() as transaksi_retur_ReturSupplierRepackCtrl;

            if (opr == null)
            {
              Functional.ShowMsgError("Objek kontrol Retur Repack tidak dapat dibuka.");
            }
            else
            {
              opr.CommandPopulate(false, pID);
            }
          }
          break;
      }
    }

    GC.Collect();
  }

  protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
  {
    if (!this.IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    string modeValue = hfMode.Text.Trim().ToUpper();

    switch (modeValue)
    {
      case RS_PEMBELIAN:
        {
          transaksi_retur_ReturSupplierPembelianCtrl opr = MyLoadControl() as transaksi_retur_ReturSupplierPembelianCtrl;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Receive Note Khusus tidak dapat dibuka.");
          }
          else
          {
            opr.CommandPopulate(true, null);
          }
        }
        break;
      default:
        {
          transaksi_retur_ReturSupplierRepackCtrl opr = MyLoadControl() as transaksi_retur_ReturSupplierRepackCtrl;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Receive Note Pembelian tidak dapat dibuka.");
          }
          else
          {
            opr.CommandPopulate(true, null);
          }
        }
        break;
    }
  }

  protected void btnPrintRS_OnClick(object sender, DirectEventArgs e)
  {
    if (!this.IsAllowPrinting)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
      return;
    }

    ReturSupplierPrintCtrl1.ShowPrintPage(hfType.Text);
  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SubmitSelection(object sender, DirectEventArgs e)
  {
      string json = e.ExtraParams["Values"];
      string tipeBtn = e.ExtraParams["BtnTipe"];
      string katcol = e.ExtraParams["katcol"];

      Dictionary<string, string>[] lstNTrans = JSON.Deserialize<Dictionary<string, string>[]>(json);

      System.Text.StringBuilder sb = new System.Text.StringBuilder();

      bool isAdd = (tipeBtn == "1" ? true : false);


      if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
      {
          Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk confirm.");
          return;
      }

      PostDataParser.StructureResponse respon = SaveParser(lstNTrans, hfGudang.Text, isAdd);

      if (respon.IsSet)
      {
         Functional.ShowMsgInformation(respon.Message);
      }
      else
      {
          e.ErrorMessage = "Unknown response";
          e.Success = false;
      }
  }
}
