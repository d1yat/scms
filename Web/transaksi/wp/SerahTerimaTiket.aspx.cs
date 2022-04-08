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

public partial class serahterima_tiket : Scms.Web.Core.PageHandler
{

    private void ClearEntry()
    {
        cbSuplier.Clear();
        cbSuplier.Disabled = false;

        Ext.Net.Store cbSuplierHdrstr = cbSuplier.GetStore();
        if (cbSuplierHdrstr != null)
        {
            cbSuplierHdrstr.RemoveAll();
        }

        txNopol.Clear();
        txNopol.Disabled = false;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            ClearEntry();
        }
    }

    protected void Report_OnGenerate()
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
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{scms_wph.c_nodoc}} = '{0}')", hfNumberId.Text),
            IsReportDirectValue = true
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "c_nodoc = @0",
            ParameterValue = (string.IsNullOrEmpty(hfNumberId.Text) ? string.Empty : hfNumberId.Text),
            IsLinqFilterParameter = true
        });

        #endregion

        rptParse.ReportingID = "20210";
        //rptParse.PaperID = "20210";
        rptParse.ReportCustomizeText = lstCustTxt.ToArray();
        rptParse.ReportParameter = lstRptParam.ToArray();
        rptParse.User = pag.Nip;
        rptParse.UserDefinedName = hfNumberId.Text;

        //rptParse.IsAutoPrint = true;
        //rptParse.OutputReport = PopulateMode.pmToWord;


        string xmlFiles = ReportParser.Deserialize(rptParse);

        SoaReportCaller soa = new SoaReportCaller();

        string result = soa.GeneratorReport(isAsync, xmlFiles);

        ReportingResult rptResult = ReportingResult.Serialize(result);

        if (rptResult == null)
        {
            Functional.ShowMsgError("Pembuatan report gagal.");
        }
        else
        {
            if (rptResult.IsSuccess)
            {
                string tmpUri = Functional.UriDownloadGenerator(pag,
                  rptResult.OutputFile, "Nomor Antrian", rptResult.Extension);

                wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
            }
            else
            {
                Functional.ShowMsgWarning(rptResult.MessageResponse);
            }
        }

        GC.Collect();
    }

    [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void SaveBtn_Click(object sender, DirectEventArgs e)
    {
        string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);

        bool isAdd = true;

        //Indra 20180920FM
        //SerahTerimaTransportasi
        if(cbSuplier.SelectedItem.Text.Trim() == "")
        {
            Functional.ShowMsgWarning("Kolom pemasok harus diisi.");
            return;
        }

        if (txNopol.Text.Trim() == "")
        {
            Functional.ShowMsgWarning("Kolom nomor kendaraan harus diisi.");
            return;
        }  
           
        PostDataParser.StructureResponse respon = SaveParser(isAdd);

        if (respon.IsSet)
        {
            if (respon.Response == PostDataParser.ResponseStatus.Success)
            {
                if (isAdd)
                {
                    hfNumberId.Text = respon.Values.GetValueParser<string>("nodoc");
                    //Functional.ShowMsgInformation("Data berhasil tersimpan.");
                    Report_OnGenerate();

                    ClearEntry();
                }
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

    private PostDataParser.StructureResponse SaveParser(bool isAdd)
    {
        PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

        PostDataParser parser = new PostDataParser();
        IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

        PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

        pair.IsSet = true;
        pair.IsList = true;
        //pair.Value = numberId;
        pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
        pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        string varData = null;

        DateTime date = DateTime.Today;

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        dic.Add("ID", pair);
        pair.DicAttributeValues.Add("Nosup", cbSuplier.SelectedItem.Value);
        pair.DicAttributeValues.Add("Nopol", txNopol.Text.ToUpper());
        pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
        //Indra 20180920FM
        //SerahTerimaTransportasi
        pair.DicAttributeValues.Add("Gudang", ((Scms.Web.Core.PageHandler)this.Page).ActiveGudang); 

        try
        {
            varData = parser.ParserData("SerahTerimaTiket", (isAdd ? "Add" : "Modify"), dic);
        }
        catch (Exception ex)
        {
            Scms.Web.Common.Logger.WriteLine("tiket antrian SaveParser : {0} ", ex.Message);
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
}
