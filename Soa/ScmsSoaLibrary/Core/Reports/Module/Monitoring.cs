using System;
using System.Collections.Generic;
using System.Text;
using ScmsSoaLibraryInterface.Components;
using ScmsSoaLibrary.Commons;
using System.Data.SqlClient;
using System.IO;

namespace ScmsSoaLibrary.Core.Reports.Module
{
  class Monitoring
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
      string rtpPath = string.Concat(cfg.PathReport, @"Monitoring\");

      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {
          #region Monitoring

          #region Packing List Print

          case Constant.REPORT_MONITORING_DATA_PL:
            {
              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "PackingListPrint.rpt");

              isGenerated = true;
            }
            break;

          #endregion

          #region Packing List Conf

          case Constant.REPORT_MONITORING_DATA_CONF:
            {
              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "PackingListConf.rpt");

              isGenerated = true;
            }
            break;

          #endregion

          #region Packing List Booked

          case Constant.REPORT_MONITORING_DATA_BOOKED:
            {
              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "PackingListBooked.rpt");

              isGenerated = true;
            }
            break;

          #endregion

          #region Send Do

          case Constant.REPORT_MONITORING_SEND_DO:
            {
              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "DeliveryOrderSend.rpt");

              isGenerated = true;
            }
            break;

          #endregion

          #region Send RC

          case Constant.REPORT_MONITORING_SEND_RC:
            {
              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "ReturCustomerSend.rpt");

              isGenerated = true;
            }
            break;

          #endregion

          #region Sales dan Stok Nasional

          case Constant.REPORT_MONITORING_SALES_NASIONAL:
            {
                rptName = "Sales dan Stok Nasional";

                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {

                        #region Sql

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("tahun", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("bulan", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("nosup", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("user", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        reportFiles = Path.Combine(rtpPath, "LG_SalesNasional.rpt");
                        tmpQuery = "Exec LG_CalcSalesNasional @tahun, @bulan, @nosup, @user";
                        
                        #endregion
                    }
                }
                isGenerated = true;
            }
            break;

          #endregion



          #endregion
          
          #region Produktifitas dc

          #region Detail
         
          case Constant.REPORT_PRODUKTIFITAS_DC:
            {
                rptName = "Produktifitas DC";

                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
                        #region Sql

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("gudang", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("type", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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



                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("date1", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                            {
                                isOk = true;
                            }

                            return isOk;
                        });
                        if (rptParam != null)
                        {
                            lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                                System.Data.SqlDbType.DateTime)
                            {
                                Value = rptParam.ParameterRawValue<DateTime>(Functionals.StandardSqlDateTime)
                            });
                        }

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("date2", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                            {
                                isOk = true;
                            }

                            return isOk;
                        });
                        if (rptParam != null)
                        {
                            lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                                System.Data.SqlDbType.DateTime)
                            {
                                Value = rptParam.ParameterRawValue<DateTime>(Functionals.StandardSqlDateTime)
                            });
                        }

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("NIP", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        tmpQuery = "Exec LG_CalcProdDC @gudang,@date1, @date2,@type,@NIP";


                        #endregion

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        reportFiles = Path.Combine(rtpPath, "ProduktifitasDC.rpt");
                        #endregion
                    }
                }

                isGenerated = true;
            }
            break;

          #endregion
          #endregion
        }
      }

      if (isGenerated)
      {
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
          outputFolder = cfg.PathReportStorage
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
      }

      lstCustoms.Clear();
      lstParams.Clear();

      return result;
    }
  }
}
