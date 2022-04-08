/*
 * Created By Indra
 * 20171231FM
 * 
*/

using System;
using System.Collections.Generic;
using System.Text;
using ScmsSoaLibraryInterface.Components;
using ScmsSoaLibrary.Commons;
using System.Data.SqlClient;
using System.IO;

namespace ScmsSoaLibrary.Core.Reports.Module
{
    class Proses
    {
        public static Functionals.ReportingGeneratorResult Generate(Config cfg, ReportParser rpt, bool isAsync)
        {
            if ((rpt == null) || (cfg == null))
            {
                return new Functionals.ReportingGeneratorResult();
            }

            Functionals.ReportingGeneratorResult result = new Functionals.ReportingGeneratorResult();

            List<SqlParameter> lstParams = new List<SqlParameter>();
            List<string> lstCustoms = new List<string>();
            ReportParameter rptParam = null;
            string tmpQuery = null;
            bool isGenerated = false;
            string reportFiles = null;
            string rptRecordSel = null;
            string rptName = null;
            string rtpPath = string.Concat(cfg.PathReport, @"Proses\");

            Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> dicRptDatasetParam = null;

            System.Data.DataSet dataSet = null;

            ReportDatasetBind rptBind = new ReportDatasetBind();

            ScmsModel.ORMDataContext db = null;

            string errMessage = null;

            if (rpt != null)
            {
                switch (rpt.ReportingID)
                {
                    #region StockOpname Indra 20171231FM

                    #region Proses

                    case Constant.REPORT_PROSES_PRINT_FORM_SO:
                        {
                            #region Parameter Tipe Report

                            rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                            {
                                bool isOk = false;

                                if (x.ParameterName.Equals("TipeReport", StringComparison.OrdinalIgnoreCase))
                                {
                                    isOk = true;
                                }
                                return isOk;
                            });
                            if (rptParam != null)
                            {
                                lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                                    System.Data.SqlDbType.VarChar)
                                {
                                    Size = 15,
                                    Value = rptParam.ParameterValue
                                });
                            }

                            string tipereport = rptParam.ParameterValue;

                            #endregion

                            #region Parameter Gudang

                            rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                            {
                                bool isOk = false;

                                if (x.ParameterName.Equals("Gudang", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                                {
                                    isOk = true;
                                }

                                return isOk;
                            });
                            if (rptParam != null)
                            {
                                lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                                    System.Data.SqlDbType.VarChar)
                                {
                                    Size = 15,
                                    Value = rptParam.ParameterValue
                                });
                            }

                            #endregion

                            #region Parameter No Form

                            rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                            {
                                bool isOk = false;

                                if (x.ParameterName.Equals("NoForm", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                                {
                                    isOk = true;
                                }

                                return isOk;
                            });
                            if (rptParam != null)
                            {
                                lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                                    System.Data.SqlDbType.VarChar)
                                {
                                    Size = 15,
                                    Value = rptParam.ParameterValue
                                });
                            }

                            #endregion

                            #region Parameter Item

                            rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                            {
                                bool isOk = false;

                                if (x.ParameterName.Equals("Item", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                                {
                                    isOk = true;
                                }

                                return isOk;
                            });

                            if (rptParam != null)
                            {
                                lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                                    System.Data.SqlDbType.VarChar)
                                {
                                    Size = 2000,
                                    Value = rptParam.ParameterValue
                                });
                            }

                            #endregion

                            #region Parameter Batch
                            /*
                            rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                            {
                                bool isOk = false;

                                if (x.ParameterName.Equals("Batch", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                                {
                                    isOk = true;
                                }

                                return isOk;
                            });

                            if (rptParam != null)
                            {
                                lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                                    System.Data.SqlDbType.VarChar)
                                {
                                    Size = 2000,
                                    Value = rptParam.ParameterValue
                                });
                            }
                            */
                            #endregion

                            #region Parameter Status

                            rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                            {
                                bool isOk = false;

                                if (x.ParameterName.Equals("Status", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                                {
                                    isOk = true;
                                }

                                return isOk;
                            });
                            if (rptParam != null)
                            {
                                lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                                    System.Data.SqlDbType.VarChar)
                                {
                                    Size = 15,
                                    Value = rptParam.ParameterValue
                                });
                            }
                            string statusreport = rptParam.ParameterValue;

                            #endregion

                            #region Parameter Cetak

                            rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                            {
                                bool isOk = false;

                                if (x.ParameterName.Equals("Cetak", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                                {
                                    isOk = true;
                                }

                                return isOk;
                            });
                            if (rptParam != null)
                            {
                                lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                                    System.Data.SqlDbType.VarChar)
                                {
                                    Size = 15,
                                    Value = rptParam.ParameterValue
                                });
                            }

                            string Cetak = rptParam.ParameterValue;

                            #endregion

                            #region Parameter Entry

                            rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                            {
                                bool isOk = false;

                                if (x.ParameterName.Equals("Entry", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                                {
                                    isOk = true;
                                }

                                return isOk;
                            });
                            if (rptParam != null)
                            {
                                lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                                    System.Data.SqlDbType.VarChar)
                                {
                                    Size = 7,
                                    Value = rptParam.ParameterValue
                                });
                            }

                            #endregion

                            rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                            if (tipereport == "6")
                            {
                                tmpQuery = "exec StockOpname_Getdate @Gudang";
                            }
                            else
                            {  
                                //tmpQuery = "Exec Rpt_StockOpname @Gudang, @NoForm, @Item, @Batch, @Status, @TipeReport, @Entry";
                                tmpQuery = "Exec Rpt_StockOpname @Gudang, @NoForm, @Item, @Status, @TipeReport, @Entry";
                            }

                            if (tipereport == "1")
                            {
                                rptName = "Form Hitung SO " + statusreport;

                                if (Cetak == "3")
                                {
                                    reportFiles = Path.Combine(rtpPath, "LG_FormSO.rpt");
                                }
                                else
                                {
                                    reportFiles = Path.Combine(rtpPath, "LG_FormSOPdf.rpt");
                                }
                            }
                            else if (tipereport == "2")
                            {
                                rptName = "Form Hasil Hitung SO " + statusreport;
                                reportFiles = Path.Combine(rtpPath, "LG_HasilSO.rpt");
                            }
                            else if (tipereport == "3")
                            {
                                rptName = "Hasil Adjustment SO " + statusreport;
                                reportFiles = Path.Combine(rtpPath, "LG_AdjustSO.rpt");
                            }
                            else if (tipereport == "4")
                            {
                                rptName = "Blank Form Hitung SO " + statusreport;
                                reportFiles = Path.Combine(rtpPath, "LG_BlankFormSO.rpt");
                            }
                            else if (tipereport == "5")
                            {
                                rptName = "Hasil Adjustment SO Rekap " + statusreport;
                                reportFiles = Path.Combine(rtpPath, "LG_AdjustSO_Rekap.rpt");
                            }

                            else if (tipereport == "6")
                            {
                                rptName = "";
                                reportFiles = Path.Combine(rtpPath, "LG_AdjustSO_Rekap.rpt");
                            }

                            isGenerated = true;
                        }
                        break;

                    #endregion

                    #region Monitoring

                    case Constant.REPORT_PROSES_PRINT_MONITORING_SO:
                        {
                            #region Parameter Gudang

                            rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                            {
                                bool isOk = false;

                                if (x.ParameterName.Equals("Gudang", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                                {
                                    isOk = true;
                                }

                                return isOk;
                            });
                            if (rptParam != null)
                            {
                                lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                                    System.Data.SqlDbType.VarChar)
                                {
                                    Size = 15,
                                    Value = rptParam.ParameterValue
                                });
                            }

                            #endregion

                            rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                            tmpQuery = "Exec Rpt_StockOpname_Monitoring @Gudang";

                            rptName = "Form Monitoring SO ";
                            reportFiles = Path.Combine(rtpPath, "LG_MonitoringSO.rpt");

                            isGenerated = true;
                        }
                        break;

                    #endregion

                    #endregion

                    #region Monitoring

                    case Constant.REPORT_PROSESEMAIL_PRODUKKOSONG:
                        {
                            #region Parameter Entry

                            rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                            {
                                bool isOk = false;

                                if (x.ParameterName.Equals("Entry", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                                {
                                    isOk = true;
                                }

                                return isOk;
                            });
                            if (rptParam != null)
                            {
                                lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                                    System.Data.SqlDbType.VarChar)
                                {
                                    Size = 7,
                                    Value = rptParam.ParameterValue
                                });
                            }

                            #endregion

                            rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                            tmpQuery = "Exec SP_RPTPRODUKKOSNG @Entry";

                            rptName = "Laporan Produk Kosong dan Nearly Expired ";
                            reportFiles = Path.Combine(rtpPath, "LG_ProdukKosong.rpt");

                            isGenerated = true;
                        }
                        break;

                    #endregion
                }
            }

            if (isGenerated)
            {
                #region Generated

                if (!string.IsNullOrEmpty(tmpQuery))
                {
                    Functionals.ExecProcedures(cfg, tmpQuery, lstParams.ToArray());
                }

                ReportEngine.CrystalReportStructureConfigure reportStruct = default(ReportEngine.CrystalReportStructureConfigure);

                reportStruct = new ReportEngine.CrystalReportStructureConfigure()
                {
                    ParametersQueryToExecute = ReportEngine.ConvertToPQE(lstParams.ToArray()),
                    QueryToExecute = tmpQuery,
                    RecordSelection = rptRecordSel,
                    ReportFile = reportFiles,
                    IsSet = true,
                    ReRunQuery = false,
                    isLandscape = rpt.IsLandscape,
                    paperName = rpt.PaperID,
                    CustomizeTextData = rpt.ReportCustomizeText,
                    outputFolder = cfg.PathReportStorage,
                    dataSet = dataSet
                };

                string tmp = null;

                if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
                {
                    if (Functionals.ReportSaveParser(rptName, rpt.ReportingID, reportStruct.outputFolder, reportStruct.outputName, reportStruct.extReport, rpt.User,
                      reportStruct.sizeOutput, isAsync, rpt.UserDefinedName, rpt.IsShared, out tmp))
                    {
                        result = new Functionals.ReportingGeneratorResult()
                        {
                            IsSet = true,
                            IsSuccess = true,
                            Messages = "Success",
                            Extension = reportStruct.extReport,
                            OutputFile = reportStruct.outputName,
                            OutputPath = reportStruct.outputFolder,
                            Size = reportStruct.sizeOutput
                        };
                    }
                    else
                    {
                        result = new Functionals.ReportingGeneratorResult()
                        {
                            IsSet = true,
                            IsSuccess = false,
                            Messages = tmp,
                            Extension = null,
                            OutputFile = null,
                            OutputPath = null,
                            Size = null
                        };
                    }
                }
                else
                {
                    result = new Functionals.ReportingGeneratorResult()
                    {
                        IsSet = true,
                        IsSuccess = false,
                        Messages = reportStruct.resultMessage,
                        Extension = null,
                        OutputFile = null,
                        OutputPath = null,
                        Size = null
                    };
                }

                #endregion
            }
            else
            {
                result = new Functionals.ReportingGeneratorResult()
                {
                    IsSet = true,
                    IsSuccess = false,
                    Messages = (string.IsNullOrEmpty(errMessage) ? "Failed to generated data." : errMessage),
                    Extension = null,
                    OutputFile = null,
                    OutputPath = null,
                    Size = null
                };
            }

            if (dicRptDatasetParam != null)
            {
                dicRptDatasetParam.Clear();
            }

            if (dataSet != null)
            {
                dataSet.Clear();
                dataSet.Dispose();
            }

            lstCustoms.Clear();
            lstParams.Clear();

            return result;
        }
    }
}
