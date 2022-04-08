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

public partial class proses_FJR : Scms.Web.Core.PageHandler
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            if (!this.IsPostBack)
            {
               
                DateTime date = DateTime.Now;

                cbBulan.Items.Add(new Ext.Net.ListItem(date.Month.ToString(), date.Month.ToString()));
                date = date.AddMonths(-1);
                cbBulan.Items.Add(new Ext.Net.ListItem(date.Month.ToString(), date.Month.ToString()));
                cbBulan.SelectedIndex = 0;

                Functional.PopulateBulan(cbBulan, date.Month);


                if (date.Month.ToString() == "12")
                {
                    Functional.PopulateTahun(cbTahun, date.Year, 1, 1);
                }
                else
                {
                    Functional.PopulateTahun(cbTahun, date.Year, 1, 0);
                }

                cbTahun.Disabled = true;
                cbBulan.Disabled = true;
              
            }
        }
    }

   
     
  


    //private PostDataParser.StructureResponse SaveParser(string Tahun, string Bulan)
    //{
    //    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    //    PostDataParser parser = new PostDataParser();
    //    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    //    //Dictionary<string, string> dicData = null;
    //    //Dictionary<string, string> dicAttr = null;

    //    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    //    pair.IsSet = true;
    //    pair.IsList = true;
    //    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    //    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    //    string varData = null;


    //    dic.Add("ID", pair);
    //    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    //    pair.DicAttributeValues.Add("Tahun", Tahun);
    //    pair.DicAttributeValues.Add("Bulan", Bulan);

    //    try
    //    {
    //        varData = parser.ParserData("ClosingLog", ("Add"), dic);
    //    }
    //    catch (Exception ex)
    //    {
    //        Scms.Web.Common.Logger.WriteLine("Proses_ClosingLog SaveParser : {0} ", ex.Message);
    //    }

    //    string result = null;

    //    if (!string.IsNullOrEmpty(varData))
    //    {
    //        Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    //        result = soa.PostData(varData);

    //        responseResult = parser.ResponseParser(result);
    //    }

    //    return responseResult;

    //}

    //protected void SaveBtn_Click(object sender, DirectEventArgs e)
    //{
                     
                      
        
    //    string Tahun = (e.ExtraParams["Tahun"] ?? string.Empty);
    //    string Bulan = (e.ExtraParams["Bulan"] ?? string.Empty);
        

    //    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();
    //    Dictionary<string, object> dicResult = null;

    //    string[][] paramX = new string[][]{
    //    new string[] { "Tahun", Tahun, "System.Float"},
    //    new string[] { "Bulan", Bulan, "System.Float"},
        
    //     };

    //    string result = soa.GlobalQueryService(0, -1, true, string.Empty, string.Empty, "30006", paramX);

    //    dicResult = JSON.Deserialize<Dictionary<string, object>>(result);

        
    //    if (string.IsNullOrEmpty(result))
    //    {
    //        e.ErrorMessage = "Unknown response";

    //        e.Success = false;
    //    }
    //    else
    //    {
          
    //        if (dicResult.ContainsKey("success") == true)
    //        {
    //            Functional.ShowMsgInformation("Data telah terproses.");
    //        }
    //    }



    //}



    protected void SaveBtn_Click(object sender, DirectEventArgs e)
    {
        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        if (!Functional.CanCreateGenerateReport(pag))
        {
            return;
        }

        ReportParser rptParse = new ReportParser();

        List<ReportParameter> lstRptParam = new List<ReportParameter>();
        List<ReportCustomizeText> lstCustTxt = new List<ReportCustomizeText>();

        DateTime date1 = DateTime.Today,
          date2 = DateTime.Today;
        List<string> lstData = new List<string>();
        bool isAsync = false;
        isAsync = chkAsync.Checked;


        #region Sql Parameter

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(String).FullName,
            ParameterName = "Tahun",
            IsSqlParameter = true,
            ParameterValue = cbTahun.SelectedItem.Text.ToString()
        });
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(String).FullName,
            ParameterName = "Bulan",
            IsSqlParameter = true,
            ParameterValue = cbBulan.SelectedItem.Value
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(char).FullName,
            ParameterName = "user",
            IsSqlParameter = true,
            ParameterValue = pag.Nip
        });


        #endregion

        rptParse.ReportingID = "30213";
        rptParse.ReportCustomizeText = lstCustTxt.ToArray();
        rptParse.ReportParameter = lstRptParam.ToArray();


        string xmlFiles = ReportParser.Deserialize(rptParse);

        SoaReportCaller soa = new SoaReportCaller();

        string result = soa.GeneratorReport(isAsync, xmlFiles);

        ReportingResult rptResult = ReportingResult.Serialize(result);

        if (string.IsNullOrEmpty(result))
        {
            e.ErrorMessage = "Unknown response";

            e.Success = false;
        }
        else
        {
            {
                Functional.ShowMsgInformation("Data telah terproses.");
            }
        }

        GC.Collect();
    }


}
