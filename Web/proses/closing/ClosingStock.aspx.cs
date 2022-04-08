using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using System.Text;

public partial class proses_closing_ClosingStock : Scms.Web.Core.PageHandler
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      DateTime date = DateTime.Now;

      Functional.PopulateBulan(cbBulan, date.Month);

      Functional.PopulateTahun(cbTahun, date.Year, 1, 0);
    }
  }

  private PostDataParser.StructureResponse SaveParser(string Tahun, string Bulan)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    //Dictionary<string, string> dicData = null;
    //Dictionary<string, string> dicAttr = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();
    
    pair.IsSet = true;
    pair.IsList = true;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;


    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("Tahun", Tahun);
    pair.DicAttributeValues.Add("Bulan", Bulan);

    try
    {
      varData = parser.ParserData("ClosingLog",("Add"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("Proses_ClosingLog SaveParser : {0} ", ex.Message);
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
    string Tahun = (e.ExtraParams["Tahun"] ?? string.Empty);
    string Bulan = (e.ExtraParams["Bulan"] ?? string.Empty);

    //PostDataParser.StructureResponse respon = SaveParser(Tahun, Bulan);

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();
    Dictionary<string, object> dicResult = null;
     
    string[][] paramX = new string[][]{
        new string[] { "Tahun", Tahun, "System.Float"},
        new string[] { "Bulan", Bulan, "System.Float"},
        new string[] { "Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip, "System.String"},
      };

    string result = soa.GlobalQueryService(0, -1, true, string.Empty, string.Empty, "30003", paramX);

    dicResult = JSON.Deserialize<Dictionary<string, object>>(result);

    //string result = soa.GlobalQueryService(0, -1, true, string.Empty, string.Empty, "0069", paramX);

    if (string.IsNullOrEmpty(result))
    {
      e.ErrorMessage = "Unknown response";

      e.Success = false;
    }
    else
    {
      //string resp = PopulateData(result, gridDetail.GetStore());

      //if (!string.IsNullOrEmpty(resp))
      //{
      //  e.ErrorMessage = resp;

      //  e.Success = false;
      //}
      if (dicResult.ContainsKey("success") == true)
      {
        Functional.ShowMsgInformation("Data telah terproses.");
      }
    }
  }
}
