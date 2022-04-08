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
    class Master
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
            string rtpPath = string.Concat(cfg.PathReport, @"Master\");

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

                    #region Master Principal

                    case Constant.REPORT_MS_PRINCIPAL:
                        {                            

                            rptName = "Daftar Principal dan Leadtime";
                            reportFiles = Path.Combine(rtpPath, "LG_Master_Principal.rpt");

                            isGenerated = true;
                        }
                        break;

                    #endregion

                    #region Master Principal

                    case Constant.REPORT_MS_PRINCIPAL_HISTORY_LEADTIME:
                        {

                            rptName = "Daftar History Perubahan Leadtime";
                            reportFiles = Path.Combine(rtpPath, "LG_Master_HistoryPrincipal.rpt");

                            isGenerated = true;
                        }
                        break;

                    #endregion

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
