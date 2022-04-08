using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class transaksi_auto_SOUpload : Scms.Web.Core.PageHandler
{
    private DateTime date1 = DateTime.Today, 
        date2 = DateTime.Today;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            txPeriode1.Text = txPeriode2.Text = DateTime.Now.ToString("dd-MM-yyyy");

            //DateTime date = DateTime.Now;
            //Functional.PopulateBulan(cbBulan, date.Month);
            //Functional.PopulateTahun(cbTahun, date.Year, 1, 0);
        }
    }

    private string UploadData(bool isValidateOnly, string fileName, byte[] byts, string tipe)
    {
        string result = null, sheet = null;
        switch (tipe)
        {
            case "1":
                tipe = "01";
                sheet = "Hitung1$";
                break;
            case "2":
                tipe = "02";
                sheet = "Hitung2$";
                break;
            case "3":
                tipe = "03";
                sheet = "Hitung3$";
                break;
            case "4":
                tipe = "04";
                sheet = "Hitung4$";
                break;
            case "5":
                tipe = "05";
                sheet = "Hitung5$";
                break;
        }
        Scms.Web.Core.SoaCaller soa = null;

        if (Scms.Web.Common.Functional.DateParser(txPeriode1.RawText.Trim(), "d-M-yyyy", out date1))
        {
            if (Scms.Web.Common.Functional.DateParser(txPeriode2.RawText.Trim(), "d-M-yyyy", out date2))
            {
                if (date2.CompareTo(date1) <= 0)
                {
                    date2 = date1;
                }
            }
            else
            {
                date2 = date1;
            }
        }
        else
        {
            return null;
        }

        string[][] paramX = new string[][]{
        new string[] { "nipEntry", this.Nip, "System.String"},
        new string[] { "isValidate", isValidateOnly.ToString().ToLower(), "System.Boolean" },
        new string[] { "tipe", tipe, "System.String" },
        new string[] { "tahun", date1.Year.ToString(), "System.String" },
        new string[] { "bulan", date1.Month.ToString(), "System.String" },
        //new string[] { "tahun", cbTahun.SelectedItem.Text.ToString(), "System.String" },
        //new string[] { "bulan", cbBulan.SelectedItem.Value, "System.String" },
        new string[] { "sheet", sheet, "System.String" }
      };

        if ((!string.IsNullOrEmpty(fileName)) && (byts != null) && (byts.Length > 0))
        {
            soa = new Scms.Web.Core.SoaCaller();

            result = soa.GlobalUploadData("up333", fileName, byts, paramX);
        }

        return result;
    }

    private string PopulateData(string jsonData, Ext.Net.Store store)
    {
        string resp = null;

        Dictionary<string, object> dicResult = null;
        List<Dictionary<string, string>> listDicResultInfo = null;
        Dictionary<string, string> dicResultInfo = null;
        Newtonsoft.Json.Linq.JArray jarr = null;
        string[] arr = null;
        int nLoop = 0,
          nLen = 0;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //string tmp = null;

        List<object[]> lstLists = null;
        List<object> lst = null;

        List<Dictionary<string, object>> lstDicResult = null;
        Dictionary<string, object> dataDicResult = null;

        try
        {
            dicResult = JSON.Deserialize<Dictionary<string, object>>(jsonData);
            if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (dicResult.GetValueParser<long>("totalRows") > 0)))
            {
                jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

                listDicResultInfo = JSON.Deserialize<List<Dictionary<string, string>>>(jarr.First.ToString());


                lstLists = new List<object[]>();
                lst = new List<object>();
                lstDicResult = new List<Dictionary<string, object>>();

                for (nLoop = 0, nLen = listDicResultInfo.Count; nLoop < nLen; nLoop++)
                {
                    dicResultInfo = listDicResultInfo[nLoop];

                    if ((arr == null) || (arr.Length != dicResultInfo.Count))
                    {
                        arr = new string[dicResultInfo.Count];

                        dicResultInfo.Keys.CopyTo(arr, 0);
                    }
                    //DateTime date = DateTime.MinValue;
                    //bool isDate = Functional.DateParser(dicResultInfo.GetValueParser<string>("d_date", string.Empty), "yyyyMMdd", out date);

                    sb.AppendFormat("{0}.insert(0, new Ext.data.Record({{", store.ClientID);
                    lst.Add(nLoop + 1);
                    lst.Add(dicResultInfo.GetValueParser<string>("Team", string.Empty));
                    lst.Add(dicResultInfo.GetValueParser<string>("C_nosup", string.Empty));
                    lst.Add(dicResultInfo.GetValueParser<string>("Principal", string.Empty));
                    lst.Add(dicResultInfo.GetValueParser<string>("C_iteno", string.Empty));
                    lst.Add(dicResultInfo.GetValueParser<string>("C_itnam", string.Empty));
                    lst.Add(dicResultInfo.GetValueParser<decimal>("Qty", 0));
                    lst.Add(dicResultInfo.GetValueParser<string>("Batch", string.Empty));
                    lst.Add(dicResultInfo.GetValueParser<string>("Ed", string.Empty));
                    //lst.Add(date.ToString("dd-MMM-yyyy"));
                    //lst.Add(dicResultInfo.GetValueParser<DateTime>("d_date", DateTime.MinValue));

                    lstLists.Add(lst.ToArray());

                    lst.Clear();

                    sb.AppendFormat("}}));", store.ClientID);
                }

                dataDicResult = new Dictionary<string, object>();

                dataDicResult.Add("d", listDicResultInfo.ToArray());
                dataDicResult.Add("totalRows", listDicResultInfo.Count);
                dataDicResult.Add("success", true);


                store.DataSource = lstLists.ToArray();
                store.DataBind();

                dataDicResult.Clear();
                listDicResultInfo.Clear();

                lstLists.Clear();

                sb.Remove(0, sb.Length);

                resp = null;
            }
            else if (dicResult.ContainsKey("exception") && (!string.IsNullOrEmpty(dicResult.GetValueParser<string>("exception"))))
            {
                resp = (string)dicResult.GetValueParser<string>("exception");
            }
        }
        catch (Exception ex)
        {
            resp = ex.Message;

            System.Diagnostics.Debug.WriteLine(
              string.Concat("transaksi_auto_SOUpload:PopulateDetail Header - ", ex.Message));
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

        return resp;
    }

    protected void Proses_OnClick(object sender, DirectEventArgs e)
    {
        if (fuImportFB.HasFile)
        {
            string tipe = (e.ExtraParams["tipe"] ?? string.Empty);
            string result = UploadData(chkVerify.Checked, fuImportFB.FileName, fuImportFB.FileBytes, tipe);

            if (string.IsNullOrEmpty(result))
            {
                e.ErrorMessage = "Unknown response";

                e.Success = false;
            }
            else
            {
                string resp = null;
                switch (tipe)
                {
                    case "1":
                        resp = PopulateData(result, gridDetailHitung1.GetStore());
                        break;
                    case "2":
                        resp = PopulateData(result, gridDetailHitung2.GetStore());
                        break;
                    case "3":
                        resp = PopulateData(result, gridDetailHitung3.GetStore());
                        break;
                    case "4":
                        resp = PopulateData(result, gridDetailHitung4.GetStore());
                        break;
                    case "5":
                        resp = PopulateData(result, gridDetailHitung5.GetStore());
                        break;
                }

                if (!string.IsNullOrEmpty(resp))
                {
                    Functional.ShowMsgInformation(resp);
                }
            }
        }
        else
        {
            Functional.ShowMsgWarning("File tidak terbaca atau tidak ada.");
        }
    }

    protected void Compare_OnClick(object sender, DirectEventArgs e)
    {
        string tipe = (e.ExtraParams["tipe"] ?? string.Empty);
        string judul = null;
        switch (tipe)
        {
            case "1":
                tipe = "01";
                judul = "Compare SO Com vs Fisik1";
                break;
            case "2":
                tipe = "02";
                judul = "Compare SO Com vs Fisik2";
                break;
            case "3":
                tipe = "03";
                judul = "Compare SO Com vs Fisik3";
                break;
            case "4":
                tipe = "04";
                judul = "Compare SO Com vs Fisik4";
                break;
            case "5":
                tipe = "05";
                judul = "Compare SO Com vs Fisik5";
                break;
        }

        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowPrinting)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
            return;
        }

        ReportParser rptParse = new ReportParser();

        List<ReportParameter> lstRptParam = new List<ReportParameter>();
        List<ReportCustomizeText> lstCustTxt = new List<ReportCustomizeText>();

        List<string> lstData = new List<string>();
        bool isAsync = false;

        #region Sql Parameter

        if (Scms.Web.Common.Functional.DateParser(txPeriode1.RawText.Trim(), "d-M-yyyy", out date1))
        {
            if (Scms.Web.Common.Functional.DateParser(txPeriode2.RawText.Trim(), "d-M-yyyy", out date2))
            {
                if (date2.CompareTo(date1) < 0)
                {
                    e.ErrorMessage = "Error periode Tgl.SO";
                    e.Success = false;
                    return;
                }
            }
            else
            {
                e.ErrorMessage = "Format Tgl.SO salah";
                e.Success = false;
                return;
            }

            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(DateTime).FullName,
                ParameterName = "tahun",
                IsSqlParameter = true,
                ParameterValue = date1.Year.ToString()
            });
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(DateTime).FullName,
                ParameterName = "bulan",
                IsSqlParameter = true,
                ParameterValue = date1.Month.ToString()
            });
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(DateTime).FullName,
                ParameterName = "date",
                IsSqlParameter = true,
                ParameterValue = date1.ToString("d-M-yyyy")
            });
        }

        //lstRptParam.Add(new ReportParameter()
        //{
        //    DataType = typeof(String).FullName,
        //    ParameterName = "tahun",
        //    IsSqlParameter = true,
        //    ParameterValue = cbTahun.SelectedItem.Text.ToString()
        //});
        //lstRptParam.Add(new ReportParameter()
        //{
        //    DataType = typeof(String).FullName,
        //    ParameterName = "bulan",
        //    IsSqlParameter = true,
        //    ParameterValue = cbBulan.SelectedItem.Value
        //});

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(String).FullName,
            ParameterName = "tipeProses",
            IsSqlParameter = true,
            ParameterValue = tipe
        });

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(String).FullName,
            ParameterName = "gudang",
            IsSqlParameter = true,
            ParameterValue = this.ActiveGudang
        });

        #endregion

        rptParse.ReportingID = "20207";
        rptParse.ReportCustomizeText = lstCustTxt.ToArray();
        rptParse.ReportParameter = lstRptParam.ToArray();
        rptParse.User = pag.Nip;
        rptParse.OutputReport = ReportParser.ParsingOutputReport("02");


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
            if (rptResult.IsSuccess)
            {
                string tmpUri = Functional.UriDownloadGenerator(pag,
                  rptResult.OutputFile, judul, rptResult.Extension);

                wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
            }
            else
            {
                Functional.ShowMsgWarning(rptResult.MessageResponse);
            }


        }

        GC.Collect();
    }

    protected void Genereate_OnClick(object sender, DirectEventArgs e)
    {
        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk proses data.");
            return;
        }

        ReportParser rptParse = new ReportParser();

        List<ReportParameter> lstRptParam = new List<ReportParameter>();
        List<ReportCustomizeText> lstCustTxt = new List<ReportCustomizeText>();

        List<string> lstData = new List<string>();
        bool isAsync = false;
        
        #region Sql Parameter

        if (Scms.Web.Common.Functional.DateParser(txPeriode1.RawText.Trim(), "d-M-yyyy", out date1))
        {
            if (Scms.Web.Common.Functional.DateParser(txPeriode2.RawText.Trim(), "d-M-yyyy", out date2))
            {
                if (date2.CompareTo(date1) <= 0)
                {
                    e.ErrorMessage = "Error periode Tgl.SO";
                    e.Success = false;
                }
            }
            else
            {
                e.ErrorMessage = "Format Tgl.SO salah";
                e.Success = false;
            }

            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(DateTime).FullName,
                ParameterName = "Tahun",
                IsSqlParameter = true,
                ParameterValue = date1.Year.ToString()
            });
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(DateTime).FullName,
                ParameterName = "Bulan",
                IsSqlParameter = true,
                ParameterValue = date1.Month.ToString()
            });

            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(DateTime).FullName,
                ParameterName = "Date1",
                IsSqlParameter = true,
                ParameterValue = date1.ToString("d-M-yyyy")
            });
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(DateTime).FullName,
                ParameterName = "Date2",
                IsSqlParameter = true,
                ParameterValue = date2.ToString("d-M-yyyy")
            });
        }

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(DateTime).FullName,
            ParameterName = "Gudang",
            IsSqlParameter = true,
            ParameterValue = this.ActiveGudang
        });

        //lstRptParam.Add(new ReportParameter()
        //{
        //    DataType = typeof(String).FullName,
        //    ParameterName = "tahun",
        //    IsSqlParameter = true,
        //    ParameterValue = cbTahun.SelectedItem.Text.ToString()
        //});
        //lstRptParam.Add(new ReportParameter()
        //{
        //    DataType = typeof(String).FullName,
        //    ParameterName = "bulan",
        //    IsSqlParameter = true,
        //    ParameterValue = cbBulan.SelectedItem.Value
        //});

        #endregion

        rptParse.ReportingID = "20208";
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
                Functional.ShowMsgInformation("Proses Generate SO sukses.");
            }
        }

        GC.Collect();
        //Functional.ShowMsgWarning("Under Development");
    }
}
