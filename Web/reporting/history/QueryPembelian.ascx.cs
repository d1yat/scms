using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Scms.Web.Common;

public partial class reporting_history_QueryPembelian : System.Web.UI.UserControl
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

        DateTime date1 = DateTime.Today,
          date2 = DateTime.Today;
        List<string> lstData = new List<string>();
        //string tmp = null;
        bool isAsync = false, isRetur = false;

        isAsync = chkAsync.Checked;

        string tipereport = null;

        if (chkBeli.Checked == true)
        {
            rptParse.ReportingID = "20013-1";
        }
        else
        {
            rptParse.ReportingID = "20013-2";
        }

        if (rdHeader.Checked == true)
        {
            #region Sql Parameter

            if (chkBeli.Checked == true)
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "({LG_RNH.c_type} = '01')",
                    IsReportDirectValue = true
                });

                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "({LG_RNH.c_gdg} = '1')",
                    IsReportDirectValue = true
                });

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
                    if ((!date1.Equals(DateTime.MinValue)) && (!date1.Equals(DateTime.MinValue)) && (!date1.Equals(date2)))
                    {
                        lstRptParam.Add(new ReportParameter()
                        {
                            DataType = typeof(string).FullName,
                            ParameterName = string.Format("({{LG_rnh.d_rndate}} IN {0} to {1})", Functional.CrystalReportDateString(date1), Functional.CrystalReportDateString(date2)),
                            IsReportDirectValue = true
                        });
                    }
                    else if (!date1.Equals(DateTime.MinValue))
                    {
                        lstRptParam.Add(new ReportParameter()
                        {
                            DataType = typeof(string).FullName,
                            ParameterName = "LG_rnh.d_rndate",
                            ParameterValue = date1.ToString("yyyy-MM-dd")
                        });
                    }

                }

                if (chkFltRN.Checked)
                {
                    tipereport = "PRN";
                }
                else if (chkFltPri.Checked)
                {
                    tipereport = "PPrinRN";
                }

                else if (chkFltDivAms.Checked)
                {
                    tipereport = "PAmsRN";
                }


                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "TipeReport",
                    IsSqlParameter = true,
                    ParameterValue = tipereport
                });

            }
            else // retur
            {
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
                    if ((!date1.Equals(DateTime.MinValue)) && (!date1.Equals(DateTime.MinValue)) && (!date1.Equals(date2)))
                    {
                        lstRptParam.Add(new ReportParameter()
                        {
                            DataType = typeof(string).FullName,
                            ParameterName = string.Format("({{LG_FBRH.d_fbdate}} IN {0} to {1})", Functional.CrystalReportDateString(date1), Functional.CrystalReportDateString(date2)),
                            IsReportDirectValue = true
                        });
                    }
                    else if (!date1.Equals(DateTime.MinValue))
                    {
                        lstRptParam.Add(new ReportParameter()
                        {
                            DataType = typeof(string).FullName,
                            ParameterName = "LG_FBRH.d_fbdate",
                            ParameterValue = date1.ToString("yyyy-MM-dd")
                        });
                    }

                    //awal 6

                    if (chkFltRN.Checked)
                    {
                        tipereport = "PBR";
                    }
                    else if (chkFltPri.Checked)
                    {
                        tipereport = "PPrinBR";
                    }

                    else if (chkFltDivAms.Checked)
                    {
                        tipereport = "PAmsBR";
                    }

                    lstRptParam.Add(new ReportParameter()
                    {
                        DataType = typeof(string).FullName,
                        ParameterName = "TipeReport",
                        IsSqlParameter = true,
                        ParameterValue = tipereport
                    });

                }
            }

            if (!string.IsNullOrEmpty(cbDivAms.SelectedItem.Value))
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "FA_MsDivAMS.c_kddivams",
                    ParameterValue = cbDivAms.SelectedItem.Value
                });
            }

            if (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value))
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "LG_DatSup.c_nosup",
                    ParameterValue = cbSuplier.SelectedItem.Value
                });
            }

            if (!string.IsNullOrEmpty(cbDivPrinsipal.SelectedItem.Value))
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "FA_MsDivPri.c_kddivpri",
                    ParameterValue = cbDivPrinsipal.SelectedItem.Value
                });
            }

            if (!string.IsNullOrEmpty(cbItems.SelectedItem.Value))
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "FA_MasItm.c_iteno",
                    ParameterValue = cbItems.SelectedItem.Value
                });
            }


            if (chkImpAll.Checked == false)
            {
                if (chkImpYes.Checked == true)
                {
                    lstRptParam.Add(new ReportParameter()
                    {
                        DataType = typeof(bool).FullName,
                        ParameterName = "lg_datsup.l_import",
                        ParameterValue = "true"
                    });
                }
                if (chkImpNo.Checked == true)
                {
                    lstRptParam.Add(new ReportParameter()
                    {
                        DataType = typeof(bool).FullName,
                        ParameterName = "lg_datsup.l_import",
                        ParameterValue = "false"
                    });
                }
            }

            lstData.Clear();

            #endregion

        }

        else if (rdDetail.Checked == true)
        {
            if (chkBeli.Checked)
            {
                tipereport = "Pembelian";
            }
            else if (chkRetur.Checked)
            {
                tipereport = "ReturPembelian";
            }

            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(string).FullName,
                ParameterName = "TipeReport",
                IsSqlParameter = true,
                ParameterValue = tipereport
            });

            #region Sql Parameter
            if (chkBeli.Checked == true)
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "({LG_RNH.c_type} = '01')",
                    IsReportDirectValue = true
                });

                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "({LG_RNH.c_gdg} = '1')",
                    IsReportDirectValue = true
                });

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
                    if ((!date1.Equals(DateTime.MinValue)) && (!date1.Equals(DateTime.MinValue)) && (!date1.Equals(date2)))
                    {
                        lstRptParam.Add(new ReportParameter()
                        {
                            DataType = typeof(string).FullName,
                            ParameterName = string.Format("({{LG_rnh.d_rndate}} IN {0} to {1})", Functional.CrystalReportDateString(date1), Functional.CrystalReportDateString(date2)),
                            IsReportDirectValue = true
                        });
                    }
                    else if (!date1.Equals(DateTime.MinValue))
                    {
                        lstRptParam.Add(new ReportParameter()
                        {
                            DataType = typeof(string).FullName,
                            ParameterName = "LG_rnh.d_rndate",
                            ParameterValue = date1.ToString("yyyy-MM-dd")
                        });
                    }

                }
            }
            else // retur
            {
                
                //
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
                    if ((!date1.Equals(DateTime.MinValue)) && (!date1.Equals(DateTime.MinValue)) && (!date1.Equals(date2)))
                    {
                        lstRptParam.Add(new ReportParameter()
                        {
                            DataType = typeof(string).FullName,
                            ParameterName = string.Format("({{LG_FBRH.d_fbdate}} IN {0} to {1})", Functional.CrystalReportDateString(date1), Functional.CrystalReportDateString(date2)),
                            IsReportDirectValue = true
                        });
                    }
                    else if (!date1.Equals(DateTime.MinValue))
                    {
                        lstRptParam.Add(new ReportParameter()
                        {
                            DataType = typeof(string).FullName,
                            ParameterName = "LG_FBRH.d_fbdate",
                            ParameterValue = date1.ToString("yyyy-MM-dd")
                        });
                    }
                }

                ////
            }

            if (!string.IsNullOrEmpty(cbDivAms.SelectedItem.Value))
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "FA_MsDivAMS.c_kddivams",
                    ParameterValue = cbDivAms.SelectedItem.Value
                });
            }

            if (!string.IsNullOrEmpty(cbSuplier.SelectedItem.Value))
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "LG_DatSup.c_nosup",
                    ParameterValue = cbSuplier.SelectedItem.Value
                });
            }

            if (!string.IsNullOrEmpty(cbDivPrinsipal.SelectedItem.Value))
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "FA_MsDivPri.c_kddivpri",
                    ParameterValue = cbDivPrinsipal.SelectedItem.Value
                });
            }

            if (!string.IsNullOrEmpty(cbItems.SelectedItem.Value))
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "FA_MasItm.c_iteno",
                    ParameterValue = cbItems.SelectedItem.Value
                });
            }


            if (chkImpAll.Checked == false)
            {
                if (chkImpYes.Checked == true)
                {
                    lstRptParam.Add(new ReportParameter()
                    {
                        DataType = typeof(bool).FullName,
                        ParameterName = "lg_datsup.l_import",
                        ParameterValue = "true"
                    });
                }
                if (chkImpNo.Checked == true)
                {
                    lstRptParam.Add(new ReportParameter()
                    {
                        DataType = typeof(bool).FullName,
                        ParameterName = "lg_datsup.l_import",
                        ParameterValue = "false"
                    });
                }
            }

            lstData.Clear();

            #endregion

        }



        rptParse.PaperID = "16x8.5";
        rptParse.ReportCustomizeText = null;
        rptParse.IsLandscape = true;
        rptParse.ReportParameter = lstRptParam.ToArray();
        rptParse.User = pag.Nip;

        rptParse.OutputReport = ReportParser.ParsingOutputReport((cbRptTypeOutput.SelectedItem != null ? cbRptTypeOutput.SelectedItem.Value : string.Empty));
        rptParse.IsShared = chkShare.Checked;
        rptParse.UserDefinedName = txRptUName.Text.Trim();

        string xmlFiles = ReportParser.Deserialize(rptParse);

        SoaReportCaller soa = new SoaReportCaller();

        //string result = "";
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
                    string rptName = string.Concat("History_Query_Pembelian_", pag.Nip, ".", rptResult.Extension);

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

        GC.Collect();
    }
}
