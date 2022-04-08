using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;
using System.IO;
using System.Text;

public partial class reporting_history_Enapza : System.Web.UI.UserControl
{
  public void InitializePage(string wndDownload)
  {
    hidWndDown.Text = wndDownload;
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
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
    string tmp = null;
    bool isAsync = false;

    rptParse.ReportingID = "21020";

    isAsync = chkAsync.Checked;

    #region Sql Parameter

    #region Param Tgl

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

      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(DateTime).FullName,
        ParameterName = "date1",
        IsSqlParameter = true,
        ParameterValue = date1.ToString("d-M-yyyy")
      });
      lstRptParam.Add(new ReportParameter()
      {
        DataType = typeof(DateTime).FullName,
        ParameterName = "date2",
        IsSqlParameter = true,
        ParameterValue = date2.ToString("d-M-yyyy")
      });
    }

    #endregion

    #region Gudang

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(DateTime).FullName,
        ParameterName = "gdg",
        IsSqlParameter = true,
        ParameterValue = pag.ActiveGudang
    });

    #endregion

    #region User

    lstRptParam.Add(new ReportParameter()
    {
        DataType = typeof(string).FullName,
        ParameterName = "user",
        IsSqlParameter = true,
        ParameterValue = pag.Nip
    });

    #endregion

    #region Bentuk Report

    if (rdgSC.Checked)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "BentukReport",
            IsSqlParameter = true,
            ParameterValue = "01"
        });
    }
    else
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "BentukReport",
            IsSqlParameter = true,
            ParameterValue = "02"
        });
    }

    #endregion

    #endregion

    #region Report Parameter

    #region Tipe Report
    //20170529 Indra D. Penambahan Produk OOT
    if (rdgPrekursor.Checked)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "TipeReport",
            IsSqlParameter = true,
            ParameterValue = "07"
        });
    }
    else if (rdgPsikotropika.Checked)
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "TipeReport",
            IsSqlParameter = true,
            ParameterValue = "02"
        });
    }
    else
    {
        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "TipeReport",
            IsSqlParameter = true,
            ParameterValue = "09"
        });
    }

    #endregion

    #region Periode

    lstCustTxt.Add(new ReportCustomizeText()
    {
        SectionName = "Section1",
        ControlName = "txtPeriode",
        Value = string.Format(": {0} {1}", "Periode : " + date1.ToString("yyyy-M-d") + " s/d ", date2.ToString("yyyy-M-d"))
    });

    #endregion

    #endregion

    rptParse.PaperID = "A3";
    rptParse.IsLandscape = true;
    rptParse.ReportCustomizeText = lstCustTxt.ToArray();
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;

    rptParse.OutputReport = ReportParser.ParsingOutputReport((cbRptTypeOutput.SelectedItem != null ? cbRptTypeOutput.SelectedItem.Value : string.Empty));
    rptParse.IsShared = chkShare.Checked;
    rptParse.UserDefinedName = txRptUName.Text.Trim();

    string xmlFiles = ReportParser.Deserialize(rptParse);

    SoaReportCaller soa = new SoaReportCaller();

    string result = soa.GeneratorReport(isAsync, xmlFiles);

    ReportingResult rptResult = ReportingResult.Serialize(result);

    //Indra 20170822 Report csv
    if (cbRptTypeOutput.SelectedItem.Value == "05")
    {
        string Header;

        Scms.Web.Core.SoaCaller soal = new Scms.Web.Core.SoaCaller();

        string[][] paramX = new string[][]{
            new string[] { "Gudang", "", "System.String"},
        };

        string result01 = soal.GlobalQueryService(0, -1, true, string.Empty, string.Empty, "100005", paramX);

        Dictionary<string, object> dicResult = null;
        Dictionary<string, string> dicResultInfo = null;
        List<Dictionary<string, string>> lstResultInfo = null;
        Newtonsoft.Json.Linq.JArray jarr = null;

        StringBuilder csv = new StringBuilder();

        try
        {
            dicResult = JSON.Deserialize<Dictionary<string, object>>(result01);

            jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

            lstResultInfo = JSON.Deserialize<List<Dictionary<string, string>>>(jarr.First.ToString());

            string NamaProduk = "";
            string KodeProduk = "";
            string SaveBatchAkhir = "", Saved_expired = "";
            int NoUrut = 0, JumlahAkhir = 0;

            Header = Header = "Tanggal;SaldoAwal;Batch_Awal;No_Faktur Masuk;Sumber;Jum_Masuk;Batch_Masuk;No_Faktur Keluar;Tujuan;Jum_Keluar;Batch_Keluar;Saldo_Akhir;Batch_Akhir;Expired";
            Header = Header + Environment.NewLine;

            for (int nLoop = 0; nLoop < lstResultInfo.Count; nLoop++)
            {

                dicResultInfo = lstResultInfo[nLoop];

                DateTime tgl_date = DateTime.MinValue;
                DateTime d_expired_date = DateTime.MinValue;

                tgl_date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("tgl"));
                d_expired_date = Functional.JsonDateToDate(dicResultInfo.GetValueParser<string>("d_expired"));

                JumlahAkhir = JumlahAkhir + 
                              dicResultInfo.GetValueParser<int>("saldoawal") +
                              dicResultInfo.GetValueParser<int>("jumlahin") -
                              dicResultInfo.GetValueParser<int>("jumlahout");                

                string tgl = tgl_date.ToString("yyyy-MM-dd");
                string saldoawal = dicResultInfo.GetValueParser<string>("saldoawal");
                string c_batch = dicResultInfo.GetValueParser<string>("c_batch");
                string fakturin = dicResultInfo.GetValueParser<string>("fakturin");
                string asal = dicResultInfo.GetValueParser<string>("asal");
                string jumlahin = dicResultInfo.GetValueParser<string>("jumlahin");
                string batchin = dicResultInfo.GetValueParser<string>("batchin");
                string fakturout = dicResultInfo.GetValueParser<string>("fakturout");
                string tujuan = dicResultInfo.GetValueParser<string>("tujuan");
                string jumlahout = dicResultInfo.GetValueParser<string>("jumlahout");
                string batchout = dicResultInfo.GetValueParser<string>("batchout");
                string saldoakhir = dicResultInfo.GetValueParser<string>("saldoakhir");
                string batchakhir = dicResultInfo.GetValueParser<string>("batchakhir");
                string d_expired = d_expired_date.ToString("yyyy-MM-dd");
                string item = dicResultInfo.GetValueParser<string>("item");
                string nama_item = dicResultInfo.GetValueParser<string>("nama_item");

                if (NamaProduk != nama_item && NoUrut > 0)
                {
                    Header = "Tanggal;SaldoAwal;Batch_Awal;No_Faktur Masuk;Sumber;Jum_Masuk;Batch_Masuk;No_Faktur Keluar;Tujuan;Jum_Keluar;Batch_Keluar;Saldo_Akhir;Batch_Akhir;Expired";
                    Header = Header + Environment.NewLine;

                }

                else
                {
                    if (batchout.Trim() != "")
                    {
                        SaveBatchAkhir = batchout;
                    }
                    else if (batchin.Trim() != "")
                    {
                        SaveBatchAkhir = batchin;
                    }
                    else
                    {
                        SaveBatchAkhir = c_batch;
                    }
                    
                    Saved_expired = d_expired;
                }

                if (saldoawal == "0")
                {
                    saldoawal = "";
                }

                if (c_batch == "0")
                {
                    c_batch = "";
                }

                if (jumlahin == "0")
                {
                    jumlahin = "";
                }

                if (jumlahout == "0")
                {
                    jumlahout = "";
                }

                if (saldoakhir == "0")
                {
                    saldoakhir = "";
                }

                String All = "";
                
                if (NamaProduk == nama_item)
                {
                    All = Header + tgl + ";" +
                            saldoawal + ";" +
                            c_batch + ";" +
                            fakturin + ";" +
                            asal + ";" +
                            jumlahin + ";" +
                            batchin + ";" +
                            fakturout + ";" +
                            tujuan + ";" +
                            jumlahout + ";" +
                            batchout + ";" +
                            saldoakhir + ";" +
                            batchakhir + ";" +
                            d_expired;

                }
                else
                {                    
                    if (NoUrut > 0)
                    {

                        JumlahAkhir = JumlahAkhir -
                              dicResultInfo.GetValueParser<int>("saldoawal") -
                              dicResultInfo.GetValueParser<int>("jumlahin") +
                              dicResultInfo.GetValueParser<int>("jumlahout");

            
                        //All = date2.ToString("yyyy-MM-dd") + ";" +
                        //    "" + ";" +
                        //    "" + ";" +
                        //    "" + ";" +
                        //    "" + ";" +
                        //    "" + ";" +
                        //    "" + ";" +
                        //    "" + ";" +
                        //    "" + ";" +
                        //    "" + ";" +
                        //    "" + ";" +
                        //    JumlahAkhir + ";" +
                        //    SaveBatchAkhir + ";" +
                        //    Saved_expired;


                        JumlahAkhir = 0;

                        JumlahAkhir = JumlahAkhir +
                              dicResultInfo.GetValueParser<int>("saldoawal") +
                              dicResultInfo.GetValueParser<int>("jumlahin") -
                              dicResultInfo.GetValueParser<int>("jumlahout");
                        
                        csv.AppendLine(All);

                        string cpath = "C:\\Documents and Settings\\";

                        string cFileName = DateTime.Today.ToString("dd-MM-yyyy") + "_" + KodeProduk + "_" + "E-Napza.csv";

                        string curFile = cpath + cFileName;

                        if (Directory.Exists(Path.GetDirectoryName(curFile)))
                        {
                            File.Delete(curFile);
                        }

                        File.AppendAllText(curFile, csv.ToString());

                        Functional.ShowAlert("Pembuatan report CSV Berhasil dibuat" +
                                Environment.NewLine + "Nama File : " + cFileName +
                                Environment.NewLine + "Lokasi File : " + cpath);

                        csv = new StringBuilder();
                    }

                    NamaProduk = nama_item;

                    All = Header + tgl + ";" +
                            saldoawal + ";" +
                            c_batch + ";" +
                            fakturin + ";" +
                            asal + ";" +
                            jumlahin + ";" +
                            batchin + ";" +
                            fakturout + ";" +
                            tujuan + ";" +
                            jumlahout + ";" +
                            batchout + ";" +
                            saldoakhir + ";" +
                            batchakhir + ";" +
                            d_expired + ";" +
                            NamaProduk
                            ;

                    KodeProduk = item;
                }

                NamaProduk = nama_item;

                csv.AppendLine(All);

                Header = "";

                NoUrut += 1;

            }
        }

        catch (Exception ex)
        {
            Functional.ShowMsgError("Pembuatan report CSV gagal.");
        }

    }

    else
    {

        if (rptResult == null)
        {
            Functional.ShowMsgError("Pembuatan report gagal.");
        }
        else
        {
            if (rptResult.IsSuccess)
            {
                if (isAsync)
                {
                    Functional.ShowMsgInformation("Report sedang dibuat, silahkan cek di halaman report beberapa saat lagi.");
                }
                else
                {
                    string rptName = string.Concat("History_E-Napza", pag.Nip, ".", rptResult.Extension);

                    string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
                    tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
                      tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

                    //wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
                    Functional.GeneratorLoadedWindow(hidWndDown.Text, tmpUri, LoadMode.IFrame);
                }
            }
            else
            {
                Functional.ShowMsgWarning(rptResult.MessageResponse);
            }
        }
    }

    GC.Collect();
  }
}
