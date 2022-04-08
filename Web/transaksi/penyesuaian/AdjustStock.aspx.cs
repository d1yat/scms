using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_penyesuaian_AdjustStock : Scms.Web.Core.PageHandler
{
  private const string ADJ_GOODBAD = "GOODBAD";
  private const string ADJ_BATCH = "BATCH";
  private const string ADJ_STOCK = "STOCK";

  private const string TYPE_ADJ_GOODBAD = "01";
  private const string TYPE_ADJ_BATCH = "02";  
  private const string TYPE_ADJ_STOCK = "03";

  private Control MyLoadControl()
  {
    Control c = null;

    const string CONT_NORMAL = "x1x1x1";

    c = phCtrl.FindControl(CONT_NORMAL);

    string modeValue = hfMode.Text.Trim().ToUpper();

    switch (modeValue)
    {
      case ADJ_GOODBAD:

        if (c == null)
        {
          c = this.LoadControl("AdjustStockGoodBad.ascx");

          c.ID = CONT_NORMAL;

          phCtrl.Controls.Add(c);
        }

        break;
      case ADJ_BATCH:

        if (c == null)
        {
          c = this.LoadControl("AdjustStockBatch.ascx");

          c.ID = CONT_NORMAL;

          phCtrl.Controls.Add(c);
        }

        break;
      default:

        if (c == null)
        {
          c = this.LoadControl("AdjustStockStock.ascx");

          c.ID = CONT_NORMAL;

          phCtrl.Controls.Add(c);
        }

        break;
    }

    return c;
  }

  protected void Page_Init(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

      if (qryString.Equals("goodbad", StringComparison.OrdinalIgnoreCase))
      {
        hfMode.Text = ADJ_GOODBAD;
        hfType.Text = TYPE_ADJ_GOODBAD;
      }
      else if (qryString.Equals("batch", StringComparison.OrdinalIgnoreCase))
      {
        hfMode.Text = ADJ_BATCH;
        hfType.Text = TYPE_ADJ_BATCH;
      }
      else
      {
        hfMode.Text = ADJ_STOCK;
        hfType.Text = TYPE_ADJ_STOCK;
      }
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    string modeValue = hfMode.Text.Trim().ToUpper();

    switch (modeValue)
    {
      case ADJ_GOODBAD:
        {
          transaksi_penyesuaian_AdjustStockGoodBad opr = MyLoadControl() as transaksi_penyesuaian_AdjustStockGoodBad;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Adjusment Good Bad tidak dapat dibuka.");
          }
          else
          {
            opr.Initialize(storeGridADJ.ClientID);
          }
        }
        break;
      case ADJ_BATCH:
        {
          transaksi_penyesuaian_AdjustStockBatch opr = MyLoadControl() as transaksi_penyesuaian_AdjustStockBatch;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Adjusment Batch tidak dapat dibuka.");
          }
          else
          {
            opr.Initialize(storeGridADJ.ClientID);
          }
        }
        break;
      default:
        {
          transaksi_penyesuaian_AdjustStockStock opr = MyLoadControl() as transaksi_penyesuaian_AdjustStockStock;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Adjusment Stock tidak dapat dibuka.");
          }
          else
          {
            opr.Initialize(storeGridADJ.ClientID);
          }
        }
        break;
    }
  }

  private PostDataParser.StructureResponse DeleteParser(string rcNumber, string gdg, string type, string ket)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = rcNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", this.Nip);
    pair.DicAttributeValues.Add("Gudang", gdg.Trim());
    pair.DicAttributeValues.Add("Type", type.Trim());
    pair.DicAttributeValues.Add("Keterangan", ket.Trim());

    try
    {
      varData = parser.ParserData("ADJSTOCK", "Delete", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_Adjustment SaveParser : {0} ", ex.Message);
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
  public void DeleteMethod(string rcNumber, string gdg, string keterangan)
  {
    string type = (string.IsNullOrEmpty(hfType.Text) ? "00" : hfType.Text);

    if (string.IsNullOrEmpty(rcNumber))
    {
      Functional.ShowMsgWarning("Nomor Adjust tidak terbaca.");

      return;
    }
    if (string.IsNullOrEmpty(keterangan))
    {
      Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");

      return;
    }
    PostDataParser.StructureResponse respon = DeleteParser(rcNumber, gdg, type, keterangan);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r); }}",
        storeGridADJ.ClientID, rcNumber));

      Functional.ShowMsgInformation(string.Format("Nomor Adjust '{0}' telah terhapus.", rcNumber));
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
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
        case ADJ_GOODBAD:
          {
            transaksi_penyesuaian_AdjustStockGoodBad opr = MyLoadControl() as transaksi_penyesuaian_AdjustStockGoodBad;

            if (opr == null)
            {
              Functional.ShowMsgError("Objek kontrol Adjusment Good Bad tidak dapat dibuka.");
            }
            else
            {
              opr.CommandPopulate(false, pID, gdgID);
            }
          }
          break;
        case ADJ_BATCH:
          {
            transaksi_penyesuaian_AdjustStockBatch opr = MyLoadControl() as transaksi_penyesuaian_AdjustStockBatch;

            if (opr == null)
            {
              Functional.ShowMsgError("Objek kontrol Adjusment Batch tidak dapat dibuka.");
            }
            else
            {
              opr.CommandPopulate(false, pID, gdgID);
            }
          }
          break;
        default:
          {
            transaksi_penyesuaian_AdjustStockStock opr = MyLoadControl() as transaksi_penyesuaian_AdjustStockStock;

            if (opr == null)
            {
              Functional.ShowMsgError("Objek kontrol Adjusment Stock tidak dapat dibuka.");
            }
            else
            {
              opr.CommandPopulate(false, pID, gdgID);
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
      case ADJ_GOODBAD:
        {
          transaksi_penyesuaian_AdjustStockGoodBad opr = MyLoadControl() as transaksi_penyesuaian_AdjustStockGoodBad;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Adjusment Good Bad tidak dapat dibuka.");
          }
          else
          {
            opr.CommandPopulate(true, null, null);
          }
        }
        break;
      case ADJ_BATCH:
        {
          transaksi_penyesuaian_AdjustStockBatch opr = MyLoadControl() as transaksi_penyesuaian_AdjustStockBatch;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Adjusment Stock tidak dapat dibuka.");
          }
          else
          {
            opr.CommandPopulate(true, null, null);
          }
        }
        break;
      default:
        {
          transaksi_penyesuaian_AdjustStockStock opr = MyLoadControl() as transaksi_penyesuaian_AdjustStockStock;

          if (opr == null)
          {
            Functional.ShowMsgError("Objek kontrol Adjustment Stock tidak dapat dibuka.");
          }
          else
          {
            opr.CommandPopulate(true, null, null);
          }
        }
        break;
    }
  }
}
