using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class master_budget_LimitEntryCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Batas Anggaran";

    hfCode.Text = string.Empty;
    hfSuplier.Text = string.Empty;

    lbSuplier.Text = string.Empty;

    cbTahun.Clear();
    cbTahun.Disabled = false;
    
    cbBulan.Clear();
    cbBulan.Disabled = false;

    txLimit.Clear();
    txLimit.Disabled = false;

    txPersent.Clear();
    txPersent.Disabled = false;

    X.AddScript(string.Format("{0}.getForm().reset();", frmHeaders.ClientID));
  }

  private void PopulateDetail(string pName, string pID, string idName, int periodeTahun, int periodeBulan, decimal limitTransaksi, decimal nextPersen)
  {
    ClearEntrys();

    hfCode.Text = hfSuplier.Text = pID;
    DateTime date = new DateTime(periodeTahun, periodeBulan, 1);

    string tmp = null;

    #region Populate Detail

    lbSuplier.Text = idName;

    tmp = periodeTahun.ToString();
    //cbTahun.SelectByValue(tmp, true);
    Functional.SetSelectBoxData(cbTahun, tmp, tmp);
    cbTahun.Disabled = true;
    
    tmp = periodeBulan.ToString();
    //cbBulan.SelectByValue(periodeBulan.ToString(), true);
    Functional.SetSelectBoxData(cbBulan, date.ToString("MMM"), tmp);
    cbBulan.Disabled = true;

    txLimit.Text = limitTransaksi.ToString();

    txPersent.Text = nextPersen.ToString();

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string pID, int periodeTahun, int periodeBulan, decimal limitAvaible, decimal persentNext)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = pID;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Tahun", periodeTahun.ToString());
    pair.DicAttributeValues.Add("Bulan", periodeBulan.ToString());
    pair.DicAttributeValues.Add("Limit", limitAvaible.ToString());
    pair.DicAttributeValues.Add("Persen", persentNext.ToString());

    try
    {
      varData = parser.ParserData("MasterBudget", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("master_batch_BatchCtrl SaveParser : {0} ", ex.Message);
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

  public void Initialize(string storeHdrId, string storeDtlId)
  {
    hfStoreHdrID.Text = storeHdrId;
    hfStoreDtlID.Text = storeDtlId;
  }

  public void CommandPopulate(bool isAdd, string pID, string pIdName, int nTahun, int nBulan, decimal limitTransaksi, decimal nextPersen)
  {
    if (isAdd)
    {
      DateTime date = DateTime.Now;

      string tmp = null;

      ClearEntrys();

      hfSuplier.Text = pID;
      lbSuplier.Text = pIdName;
      hfCode.Text = string.Empty;

      tmp = date.Year.ToString();
      Functional.SetSelectBoxData(cbTahun, tmp, tmp);

      tmp = date.Month.ToString();
      Functional.SetSelectBoxData(cbBulan, date.ToString("MMM"), tmp);

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
      PopulateDetail("c_nosup", pID, pIdName, nTahun, nBulan, limitTransaksi, nextPersen);
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      DateTime date = DateTime.Now;

      Functional.PopulateBulan(cbBulan, date.Month);

      Functional.PopulateTahun(cbTahun, date.Year, 1, 1);
    }
  }

  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    int thn,
      bln,
      nCode = this.GetHashCode();
    decimal lmt,
      persen,
      sisaLmt = 0;

    string suplId = (e.ExtraParams["SuplierID"] ?? string.Empty);
    string storHdr = (e.ExtraParams["StoreHdrID"] ?? string.Empty);
    string storDtl = (e.ExtraParams["StoreDtlID"] ?? string.Empty);

    string tmp = (e.ExtraParams["Tahun"] ?? string.Empty);
    int.TryParse(tmp, out thn);

    tmp = (e.ExtraParams["Bulan"] ?? string.Empty);
    int.TryParse(tmp, out bln);

    tmp = (e.ExtraParams["Limit"] ?? string.Empty);
    decimal.TryParse(tmp, out lmt);

    tmp = (e.ExtraParams["Persentase"] ?? string.Empty);
    decimal.TryParse(tmp, out persen);

    tmp = (e.ExtraParams["CodeID"] ?? string.Empty);
    bool isAdd = string.IsNullOrEmpty(tmp);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, suplId, thn, bln, lmt, persen);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        if (respon.Values != null)
        {
          sisaLmt = respon.Values.GetValueParser<decimal>("UsedLimit", 0);
        }

        if (!string.IsNullOrEmpty(storHdr))
        {
          if (isAdd)
          {
            X.Js.AddScript(@"{0}.insert(0, new Ext.data.Record({{
                        'c_nosup': '{1}',
                        'v_nama': '',
                        'n_tahun': '{2}',
                        'n_bulan': '{3}',
                        'n_limit': '{4}',
                        'n_avaiblelimit': '{5}',
                        'n_nextlimit': '{6}',
                      }}));{0}.commitChanges();",
                          storHdr, suplId, thn.ToString(), bln.ToString(), lmt.ToString(),
                          (lmt - sisaLmt), persen.ToString());
          }
          else
          {
            X.Js.AddScript(@"var idx{0} = storeFindMultiple({1}, ['c_nosup', 'n_tahun', 'n_bulan'], ['{2}', '{3}', '{4}']);
                            if(idx{0} != -1) {{
                              var r = {1}.getAt(idx{0});
                              if(!Ext.isEmpty(r)) {{
                                r.set('n_limit', {5});
                                r.set('n_avaiblelimit', {6});
                                r.set('n_nextlimit', {7});
                                {1}.commitChanges();
                              }}
                            }}", nCode, storHdr, suplId, thn, bln, lmt, sisaLmt, persen);
          }
        }

        if (!string.IsNullOrEmpty(storDtl))
        {
          X.Js.AddScript("{0}.removeAll();", storDtl);
        }

        this.ClearEntrys();

        winDetail.Hide();

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
