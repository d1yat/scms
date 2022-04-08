//Created By Indra Monitoring Process 20180523FM

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using Scms.Web.Core;
using ScmsSoaLibraryInterface.Components;
using System.Text;


public partial class transaksi_wp_MonitoringPL : Scms.Web.Core.PageHandler
{
    public void InitializePage(string wndDownload)
    {
        hidWndDown.Text = wndDownload;
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
         if (!this.IsPostBack && this.Visible)
        {
            taskMgr.StartTask("servertime");

            //MonitoringPLSummary1.Initialize(gridMain.ClientID, "");

            string Cabang = null;

            Cabang = (cbCustomerFltr.SelectedItem.Text == "" ? "CABANG : ALL" : "CABANG : " + cbCustomerFltr.SelectedItem.Value);

            Scms.Web.Core.PageHandler page = this.Page as Scms.Web.Core.PageHandler;

            Functional.SetComboData(cbPosisiStok, "c_gdg", page.ActiveGudangDescription, page.ActiveGudang);

            dtSPReceivedAwal.Text = DateTime.Now.ToString("dd-MM-yyyy");
            dtSPReceivedAkhir.Text = DateTime.Now.ToString("dd-MM-yyyy");

            dtSPReceivedAwal.MinDate = DateTime.Now.AddMonths(-1).AddDays(-DateTime.Now.Day + 1);
            dtSPReceivedAwal.MaxDate = DateTime.Now;
            dtSPReceivedAkhir.MaxDate = DateTime.Now;
            sbGetData.SelectedIndex = 0;

        }
    }

    protected void gridMainCommand(object sender, DirectEventArgs e)
    {
        string cmd      = (e.ExtraParams["Command"] ?? string.Empty);
        string NOSPSG   = (e.ExtraParams["NOSPSG"] ?? string.Empty);
        string NOPL     = (e.ExtraParams["NOPL"] ?? string.Empty);
        string NOSJ     = (e.ExtraParams["NOSJ"] ?? string.Empty);
        string TipeData = sbGetData.Text;

        if(cmd.Equals("GridDtlPL", StringComparison.OrdinalIgnoreCase))
        {
            if (TipeData == "PL/SJ")
            {
                if ((NOPL == "") && (NOSJ == ""))
                {
                    Functional.ShowMsgInformation("Nomor PL / SJ Belum dibuat.");
                    return;
                }
                else
                {
                    if (NOPL != "")
                    {
                        MonitoringPLGridDtlPL1.CommandPopulate(false, NOPL);
                        return;
                    }
                    else
                    {
                        MonitoringPLGridDtlPL1.CommandPopulate(false, NOSJ);
                        return;
                    }
                }
            }
            else
            {
                MonitoringPLGridDtlPL1.CommandPopulate(false, NOSPSG);
                return;
            }
            
        }

        GC.Collect();
    }

    protected void gridMainCommand2(object sender, DirectEventArgs e)
    {
        string cmd = (e.ExtraParams["Command"] ?? string.Empty);
        string NOEXP = (e.ExtraParams["NOEXP"] ?? string.Empty);

        if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
        {
            MonitoringPLEkspedisi1.CommandPopulate(false, NOEXP);
        }        

        GC.Collect();
    }

    protected void RefreshTime(object sender, DirectEventArgs e)
    {
        Store eStr = gridMain.GetStore();
        gridMain.Reload();

        Store eStr2 = GridPanel1.GetStore();
        GridPanel1.Reload();

        Store eStr3 = GridPanel3.GetStore();
        GridPanel3.Reload();

        //eStr.DataBind();

        string gdgAsal = cbPosisiStok.SelectedItem.Value;
        string Cabang  = cbCustomerFltr.SelectedItem.Text;

        //PopulateData(gdgAsal, Cabang);

        GC.Collect();
    }

    protected void ButtonTodo_Click(object sender, DirectEventArgs e)
    {
        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        if (SelectTodo.SelectedItem.Text == "")
        {
            Functional.ShowMsgInformation("Kolom masih kosong");
            return;
        }
        else if (SelectTodo.SelectedItem.Value == "LEGEND")
        {
            Legend1.CommandPopulate(true, pag.ActiveGudang);
        }
        else if (SelectTodo.SelectedItem.Value == "PL")
        {
            PackingListCtrl1.CommandPopulate(true, pag.ActiveGudang);
        }
        else if (SelectTodo.SelectedItem.Value == "PRINTGRIDMAIN")
        {
            winPrintData.Hidden = false;
        }
        else if (SelectTodo.SelectedItem.Value == "CHART")
        {
            winChart.Hidden = false;
        }
    }

    protected void Report_OnGenerate(object sender, DirectEventArgs e)
    {
        string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
        string tmp = null, cabang = "", simpancabang = "";
        Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

        if (!Functional.CanCreateGenerateReport(pag))
        {
            return;
        }

        Dictionary<string, string>[] gridDataCabang = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);
        Dictionary<string, string> dicData = null;
        
        ReportParser rptParse = new ReportParser();

        List<ReportParameter> lstRptParam = new List<ReportParameter>();
        List<ReportCustomizeText> lstCustTxt = new List<ReportCustomizeText>();

        DateTime date1 = DateTime.Today,
          date2 = DateTime.Today;
        List<string> lstData = new List<string>();
        //string tmp = null;
        bool isAsync = false;

        rptParse.ReportingID = "10123";

        #region Tipe Report

        String TipeReport = "";

        if (rd01.Checked)
        {
            TipeReport = "01";
        }
        else
        {
            TipeReport = "02";
        }

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(char).FullName,
            ParameterName = "Tipereport",
            IsSqlParameter = true,
            ParameterValue = TipeReport
        });

        #endregion

        if (rd01.Checked)
        {
            #region Sql Parameter                        

            #region Gudang 2

            String Gudang2 = "";

            if (cbPosisiStokRpt.SelectedItem.Value == "1")
            {
                Gudang2 = "1";
            }
            else if (cbPosisiStokRpt.SelectedItem.Value == "2")
            {
                Gudang2 = "2";
            }
            else if (cbPosisiStokRpt.SelectedItem.Value == "6")
            {
                Gudang2 = "6";
            }
            else
            {
                Gudang2 = "0";
            }

            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(char).FullName,
                ParameterName = "Gudang2",
                IsSqlParameter = true,
                ParameterValue = Gudang2
            });

            #endregion

            #region SP Received Time

            if (Scms.Web.Common.Functional.DateParser(dtSPReceivedAwal.RawText.Trim(), "d-M-yyyy", out date1))
            {
                if (Scms.Web.Common.Functional.DateParser(dtSPReceivedAkhir.RawText.Trim(), "d-M-yyyy", out date2))
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

            #region Cabang

            for (int nLoop = 0, nLen = gridDataCabang.Length; nLoop < nLen; nLoop++)
            {
                tmp = nLoop.ToString();

                dicData = gridDataCabang[nLoop];

                cabang = dicData.GetValueParser<string>("A_F").ToString();
                if (cabang == "False")
                {
                    cabang = "";
                }
                else
                {
                    simpancabang = simpancabang + dicData.GetValueParser<string>("AF") + ",";
                }

                cabang = dicData.GetValueParser<string>("G_L").ToString();
                if (cabang == "False")
                {
                    cabang = "";
                }
                else
                {
                    simpancabang = simpancabang + dicData.GetValueParser<string>("GL") + ",";
                }

                cabang = dicData.GetValueParser<string>("M_R").ToString();
                if (cabang == "False")
                {
                    cabang = "";
                }
                else
                {
                    simpancabang = simpancabang + dicData.GetValueParser<string>("MR") + ",";
                }

                cabang = dicData.GetValueParser<string>("S_Z").ToString();
                if (cabang == "False")
                {
                    cabang = "";
                }
                else
                {
                    simpancabang = simpancabang + dicData.GetValueParser<string>("SZ") + ",";
                }

            }

            if (simpancabang != "")
            {
                simpancabang = simpancabang.Substring(0, simpancabang.Length - 1);
            }
            else
            {
                simpancabang = "000";
            }

            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(char).FullName,
                ParameterName = "cabang2",
                IsSqlParameter = true,
                ParameterValue = simpancabang
            });

            #endregion

            #region User

            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(char).FullName,
                ParameterName = "user",
                IsSqlParameter = true,
                ParameterValue = pag.Nip
            });

            #endregion

            #endregion

            #region Report Parameter

            #region NIP

            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = "LG_Rpt_ProcessMonitoring.NIP",
                ParameterValue = pag.Nip
            });

            #endregion

            #endregion           

        }
        else
        {

            #region Sql Parameter
            
            #region Gudang

            String Gudang = "";

            if (cbPosisiStok.SelectedItem.Value == "1")
            {
                Gudang = "1";
            }
            else if (cbPosisiStok.SelectedItem.Value == "2")
            {
                Gudang = "2";
            }
            else if (cbPosisiStok.SelectedItem.Value == "6")
            {
                Gudang = "6";
            }
            else
            {
                Gudang = "1";
            }

            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(char).FullName,
                ParameterName = "Gudang",
                IsSqlParameter = true,
                ParameterValue = Gudang
            });

            #endregion

            #region DivAMS

            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(char).FullName,
                ParameterName = "DivAMS",
                IsSqlParameter = true,
                ParameterValue = cbDivAms.SelectedItem.Value == null ? "0000" : cbDivAms.SelectedItem.Value
            });

            #endregion

            #region Cabang

            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(char).FullName,
                ParameterName = "Cabang",
                IsSqlParameter = true,
                ParameterValue = cbCustomerFltr.SelectedItem.Value == null ? "0000" : cbCustomerFltr.SelectedItem.Value
            });

            #endregion

            #region SP Cabang

            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(char).FullName,
                ParameterName = "NoSPCab",
                IsSqlParameter = true,
                ParameterValue = txSPCABFltr.Text == "" ? "0000" : txSPCABFltr.Text
            });

            #endregion

            #region SP HO

            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(char).FullName,
                ParameterName = "NoSPHO",
                IsSqlParameter = true,
                ParameterValue = txSPHOFltr.Text == "" ? "0000" : txSPHOFltr.Text
            });

            #endregion

            #region Status

            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(char).FullName,
                ParameterName = "Status",
                IsSqlParameter = true,
                ParameterValue = sbStatusFltr.SelectedItem.Text.Trim() == "" ? "0000" : sbStatusFltr.SelectedItem.Text
            });

            #endregion

            #region Kode Item

            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(char).FullName,
                ParameterName = "KdItem",
                IsSqlParameter = true,
                ParameterValue = cbItems.SelectedItem.Value == null ? "0000" : cbItems.SelectedItem.Value
            });

            #endregion

            #endregion
        }

        isAsync = true;

        
        

        rptParse.PaperID = "A3";
        rptParse.ReportCustomizeText = lstCustTxt.ToArray();
        rptParse.ReportParameter = lstRptParam.ToArray();
        rptParse.IsLandscape = false;
        rptParse.User = pag.Nip;

        rptParse.OutputReport = ReportParser.ParsingOutputReport(("02"));
        rptParse.IsShared = false;
        rptParse.UserDefinedName = "";

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
                if (isAsync)
                {
                    Functional.ShowMsgInformation("Report sedang dibuat, silahkan cek di halaman report beberapa saat lagi.");
                }
                else
                {
                    string rptName = string.Concat("Process_Monitoring_", pag.Nip, ".", rptResult.Extension);

                    string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
                    tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
                      tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

                    Functional.GeneratorLoadedWindow(hidWndDown.Text, tmpUri, LoadMode.IFrame);
                }
            }
            else
            {
                Functional.ShowMsgWarning(rptResult.MessageResponse);
            }
        }

        GC.Collect();

    }   
       
}
