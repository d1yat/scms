using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_pengiriman_EkspedisiCabangCtrl : System.Web.UI.UserControl
{
  private void ClearEntrys()
  {
    DateTime date = DateTime.Now;

    lblNoResi.Text = string.Empty;
    lblCabang.Text = string.Empty;
    lblDateResi.Text = string.Empty;
    lblEksDesc.Text = string.Empty;
    txDayTerimaHdr.Clear();
    txDayTerimaHdr.Text = date.ToString("d-M-yyyy");
    txDayTerimaHdr.Disabled = false;

    txTimeTerimaHdr.Clear();
    txTimeTerimaHdr.Text = date.ToString("HH:mm:ss");
    txTimeTerimaHdr.Disabled = false;

    hfExpNo.Clear();
    hfExpNoCab.Clear();
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  public void Initialize(string storeIDGridMain)
  {
    hfStoreID.Text = storeIDGridMain;
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string Day, string Time, string secID)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = secID;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;
    bool isValid = false;
    DateTime tanggal = DateTime.MinValue;
    System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("id-ID");

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("ExpId", hfExpNo.Text);

    isValid = DateTime.TryParse(Day, ci, System.Globalization.DateTimeStyles.AssumeLocal, out tanggal);
    if (!isValid)
    {
      tanggal = DateTime.MinValue;
    }
    pair.DicAttributeValues.Add("Day", tanggal.ToString("yyyyMMdd"));

    isValid = DateTime.TryParse(Time, ci, System.Globalization.DateTimeStyles.AssumeLocal, out tanggal);
    if (!isValid)
    {
      tanggal = DateTime.MinValue;
    }
    pair.DicAttributeValues.Add("Time", tanggal.ToString("HHmmss"));

    isValid = DateTime.TryParse(hfDateResi.Text, ci, System.Globalization.DateTimeStyles.AssumeLocal, out tanggal);
    if (!isValid)
    {
      tanggal = DateTime.Parse(hfDateResi.Text);
    }
    pair.DicAttributeValues.Add("DateResi", tanggal.ToString("yyyyMMddHHmmss"));

    try
    {
      varData = parser.ParserData("EkspedisiCabang", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_ekspedisi_CabangCtrl SaveParser : {0} ", ex.Message);
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

  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string storeId = (e.ExtraParams["StoreID"] ?? string.Empty);
    string Day = (e.ExtraParams["Day"] ?? string.Empty);
    string Time = (e.ExtraParams["Time"] ?? string.Empty);

    bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

    PostDataParser.StructureResponse respon = SaveParser(isAdd, Day, Time, numberId);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;

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

            string ExpId =  respon.Values.GetValueParser<string>("ExpId", string.Empty);
            string ExpCab = respon.Values.GetValueParser<string>("ExpCabang", string.Empty);

            if (!string.IsNullOrEmpty(storeId))
            {
              scrpt = string.Format(@"var rec = {0}.getById('{1}');
                                  if(!Ext.isEmpty(rec)) {{
                                    rec.set('l_status', true);
                                    rec.set('c_noexpcab', '{2}');
                                    rec.commit();
                                  }}", storeId, ExpId, 
                                     ExpCab);

              X.AddScript(scrpt);

              PopulateDetail("c_expno", ExpId, ExpCab);
            }
          }
        }

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

  private void PopulateDetail(string pName, string pID, string secID)
  {
    ClearEntrys();

    Dictionary<string, object> dicResult = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    DateTime date = DateTime.Now, tdate = DateTime.Now;

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0181", paramX);

    try
    {
      dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

        dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Concat("Ekspedisi Cabang - ", pID);

        date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_resi"));
        string dCabang = dicResultInfo.GetValueParser<string>("d_cabang");
        string tCabang = dicResultInfo.GetValueParser<string>("t_cabang");

        //if (Functional.DateParser(dCabang, "yyyyMMdd", out tdate))
        //{
        //  dData = Functional.DateToJson(tdate);
        //}

        //tdate = Functional.JsonDateToDate(dCabang);
        
        lblCabang.Text = dicResultInfo.GetValueParser<string>("v_cunam");
        lblEksDesc.Text = dicResultInfo.GetValueParser<string>("v_ket");
        lblNoResi.Text = dicResultInfo.GetValueParser<string>("c_resi");
        lblDateResi.Text = date.ToString("dd-MM-yyyy hh:mm:ss");
        if (!string.IsNullOrEmpty(dCabang))
        {
          txDayTerimaHdr.Text = dCabang;
        }
        if (!string.IsNullOrEmpty(tCabang))
        {
          txTimeTerimaHdr.Text = tCabang;
        }

        hfExpNoCab.Text = secID;
        hfExpNo.Text = pID;
        hfDateResi.Text = date.ToString();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaction_Ekspedisi_CabangCtrl:PopulateDetail Header - ", ex.Message));
    }
    finally
    {

      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicResultInfo != null)
      {
        dicResultInfo.Clear();
      }
      if (dicResult != null)
      {
        dicResult.Clear();
      }
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    winDetail.Title = "No Ekspedisi " + pID;

    GC.Collect();
  }

  public void CommandPopulate(string pID, string secID)
  {
    winDetail.Hidden = false;
    PopulateDetail("c_expno", pID, secID);
  }
}
