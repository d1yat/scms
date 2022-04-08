using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_memo_MemoSTTSampleCtrl : System.Web.UI.UserControl
{
  private void ClearEntrys()
  {

    hfMemo.Clear();

    cbGudangHdr.Clear();
    cbGudangHdr.Disabled = false;

    txMemoHdr.Clear();
    txMemoHdr.Disabled = false;
    
    cbItemDtl.Clear();
    cbItemDtl.Disabled = false;
    
    cbKryHdr.Clear();
    cbKryHdr.Disabled = false;

    txQtyDtl.Clear();
    txQtyDtl.Disabled = false;

    txKeteranganHdr.Clear();
    txKeteranganHdr.Disabled = false;

    btnPrint.Hidden = true;

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string memoNumber, string gudangId, string memoId, string NipKry, string Keterangan, Dictionary<string, string>[] dics)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = memoNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string tmp = null,
      tmp1 = null,
      item = null,
      batch = null,
      ket = null,
       rcvNove = null;
    decimal nQty = 0,
      nKomposisi = 0,
      nBox = 0;
    bool isNew = false,
      isVoid = false,
      isModify = false;
    string varData = null;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", "Patra");
    pair.DicAttributeValues.Add("Gudang", gudangId);
    pair.DicAttributeValues.Add("MemoID", memoId);
    pair.DicAttributeValues.Add("Nip", NipKry);
    pair.DicAttributeValues.Add("Keterangan", Keterangan);

    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      isNew = dicData.GetValueParser<bool>("l_new");
      isVoid = dicData.GetValueParser<bool>("l_void");
      isModify = false;

      if (isNew && (!isVoid) && (!isModify))
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");
        nQty = dicData.GetValueParser<decimal>("n_qty", 0);

        if ((!string.IsNullOrEmpty(item)) &&
          (nQty > 0) && (nKomposisi > 0) && (!string.IsNullOrEmpty(batch)))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Qty", nQty.ToString());

          pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
          {
            IsSet = true,
            Value = (string.IsNullOrEmpty(ket) ? string.Empty : ket.Trim()),
            DicAttributeValues = dicAttr
          });
        }
      }
      else if ((!isNew) && isVoid && (!isModify))
      {
        dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        dicAttr.Add("New", isNew.ToString().ToLower());
        dicAttr.Add("Delete", isVoid.ToString().ToLower());
        dicAttr.Add("Modified", isModify.ToString().ToLower());

        item = dicData.GetValueParser<string>("c_iteno");

        if (!string.IsNullOrEmpty(item))
        {
          dicAttr.Add("Item", item);
          dicAttr.Add("Batch", batch);

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
      varData = parser.ParserData("LGMemoSample", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_memo_MemoSampleCtrl SaveParser : {0} ", ex.Message);
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

  public void CommandPopulate(bool isAdd, string pID, string gdgID, string Type)
  {
    if (isAdd)
    {
      //hfAdjType.Text = Type;
      //ClearEntrys();

      switch (Type)
      {
        case "04":
          //winDetail.Title = "Adjustment FB";
          break;
      }


      //winDetail.Hidden = false;
      //winDetail.ShowModal();
    }
    else
    {
      //winDetail.Title = "Adjustment " + pID;
      //PopulateDetail("c_adjno", pID, gdgID);
    }
  }

  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    string gdgId = (e.ExtraParams["GudangID"] ?? string.Empty);
    string gdgDesc = (e.ExtraParams["GudangDesc"] ?? string.Empty);
    string NamaKry = (e.ExtraParams["NamaKry"] ?? string.Empty);
    string NipKry = (e.ExtraParams["NipKry"] ?? string.Empty);
    string memoId = (e.ExtraParams["Memo"] ?? string.Empty);
    string Keterangan = (e.ExtraParams["Keterangan"] ?? string.Empty);

    Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gdgId, memoId, NipKry, Keterangan, gridDataPL);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;
        string storeId = hfStoreID.Text;

        string dateJs = null;
        DateTime date = DateTime.Today;

        if (isAdd)
        {
          if (respon.Values != null)
          {
            if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
            {
              dateJs = Functional.DateToJson(date);
            }

            if (!string.IsNullOrEmpty(storeId))
            {
              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                              'c_mtno': '{1}',
                              'd_mtdate': {2},
                              'v_gdgdesc': '{3}',
                              'v_ket': '{4}',
                              'c_memono': '{5}',
                              'v_nama': '{6}'
              }}));{0}.commitChanges();", storeId,
                                        respon.Values.GetValueParser<string>("COMBO", string.Empty),
                                        gdgId,
                                        gdgDesc,
                                        dateJs,
                                        memoId);

              X.AddScript(scrpt);
            }
          }
        }

        this.ClearEntrys();

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
