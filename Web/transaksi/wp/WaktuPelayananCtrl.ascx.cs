using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class transaksi_wp_WaktuPelayananCtrl : System.Web.UI.UserControl
{
  private static Dictionary<string, string>[] lst = null;
  private static string Gudang = null;
  private static string Tipe = null;
  private static string sStoreTo = null;
  private static string sStoreFrom = null;
  private static bool isNew = false;

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  private PostDataParser.StructureResponse SaveParser(Dictionary<string, string>[] listNum, string sGudang, string tipe, bool isAdd, string serah, string terima)
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
    pair.DicAttributeValues.Add("Entry", terima);
    pair.DicAttributeValues.Add("Give", serah);
    pair.DicAttributeValues.Add("Gudang", sGudang);
    pair.DicAttributeValues.Add("Tipe", tipe);


    bool isNew = false,
      isVoid = false,
      isModify = false;

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    if ((listNum.Length > 0) || !string.IsNullOrEmpty(sGudang) || !string.IsNullOrEmpty(sGudang))
    {

      for (int nLoop = 0, nLen = listNum.Length; nLoop < nLen; nLoop++)
      {
        tmp = nLoop.ToString();

        dicData = listNum[nLoop];

        isNew = dicData.GetValueParser<bool>("l_new");

        if (isNew)
        {
          if (!pag.IsAllowAdd)
          {
            continue;
          }
          dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

          dicAttr.Add("New", isNew.ToString().ToLower());

          noTrans = dicData.GetValueParser<string>("c_notrans");

          if ((!string.IsNullOrEmpty(noTrans)))
          {
            dicAttr.Add("TransNo", noTrans);

            pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
            {
              IsSet = true,
              DicAttributeValues = dicAttr
            });
          }
        }
        else
        {
          if (!pag.IsAllowDelete)
          {
            continue;
          }
          dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

          dicAttr.Add("Delete", false.ToString());

          noTrans = dicData.GetValueParser<string>("c_notrans");

          if ((!string.IsNullOrEmpty(noTrans)))
          {
            dicAttr.Add("TransNo", noTrans);

            pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
            {
              IsSet = true,
              DicAttributeValues = dicAttr
            });
          }

        }
      }
    }
    try
    {
      varData = parser.ParserData("WaktuPelayanan", (isAdd ? "Add" : "Delete"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_wp_WaktuPelayanan SaveParser : {0} ", ex.Message);
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

  public void Save_btn(Dictionary<string, string>[] lstData, string hfGdg, string hfType, bool isAdd, string sTo, string sFrom)
  {
    lst = lstData;
    Gudang = hfGdg;
    Tipe = hfType;
    isNew = isAdd;
    sStoreTo = sTo;
    sStoreFrom = sFrom;

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;


    Functional.SetComboData(cbPenerima, "c_nip", pag.Username, pag.Nip);

    winDetail.Hidden = false;

    
  }

  protected void btnSimpan_Click(object sender, DirectEventArgs e)
  {
    string Penyerah = e.ExtraParams["Penyerah"];
    string Penerima = e.ExtraParams["Penerima"];


    PostDataParser.StructureResponse respon = SaveParser(lst, Gudang, Tipe, isNew, Penyerah, Penerima);

    string scrpt = null,
      scrpt1 = null;

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        scrpt = string.Format(@"{0}.removeAll(); {0}.reload();", sStoreTo);

        X.AddScript(scrpt);

        scrpt1 = string.Format(@"{0}.removeAll(); {0}.reload();", sStoreFrom);

        X.AddScript(scrpt1);

        winDetail.Hidden = true;
      }
      else
      {
        e.ErrorMessage = respon.Message;

        e.Success = false;
      }
    }
    else
    {

    }
  }
}
