using System;
using System.Collections.Generic;
using System.Text;
using ScmsSoaLibraryInterface.Components;
using ScmsSoaLibrary.Commons;
using System.Data.SqlClient;
using System.IO;

namespace ScmsSoaLibrary.Core.Reports.Module
{
  class Konsolidasi
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
      string rtpPath = string.Concat(cfg.PathReport, @"Konsolidasi\");
      System.Data.DataSet dataSet = null;

      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {
            #region REPORT_KONSOLIDASI_BDP

            case Constant.REPORT_KONSOLIDASI_BDP:
                {
                    rptName = "Konsolidasi Barang Dalam Perjalanan (BDP)";

                    if (rpt.ReportParameter != null)
                    {
                        if (rpt.ReportParameter.Length > 0)
                        {
                            #region Sql

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
                            #endregion

                            #region Report

                            rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);
                            //reportFiles = Path.Combine(rtpPath, "LG_BDP_1.rpt");
                            reportFiles = Path.Combine(rtpPath, "LG_BDP_NEW.rpt");
                            tmpQuery = "exec LG_SNAP_BDP_RPT @date1,@date2";

                            #endregion
                        }
                    }

                    isGenerated = true;
                }
                break;

            #endregion


          #region REPORT_KONSOLIDASI_RDP

          case Constant.REPORT_KONSOLIDASI_RDP:
            {
              rptName = "Konsolidasi Retur Dalam Perjalanan (RDP)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                    #region Parameter

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

                    #endregion

                    #region Report

                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                    reportFiles = Path.Combine(rtpPath, "LG_RDP_NEW.rpt");
                    tmpQuery = "exec SP_LG_SNAP_RDP @date1,@date2";
                    #endregion
                }
              }

              isGenerated = true;
            } 
            break;

          #endregion

          #region REPORT_KONSOLIDASI_STOKNASIONAL

          case Constant.REPORT_KONSOLIDASI_STOKNASIONAL:
            {
              rptName = "Konsolidasi Stok Nasional";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Sql

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

                    if (x.ParameterName.Equals("session", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_RptStockNas @date1, @date1, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_RptStockNas1.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region REPORT_KONSOLIDASI_STOKNASIONAL_NONDIVPRI

          case Constant.REPORT_KONSOLIDASI_STOKNASIONAL_NONDIVPRI:
            {
              rptName = "Konsolidasi Stok Nasional (W/O Divisi Prinsipal)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Sql

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

                    if (x.ParameterName.Equals("session", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_RptStockNas @date1, @date1, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_RptStockNas2.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region REPORT_KONSOLIDASI_BDP_PHAROS

          case Constant.REPORT_KONSOLIDASI_BDP_PHAROS:
            {
              rptName = "Konsolidasi Bukti Dalam Perjalanan (BDP) Pharos Groups";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Sql

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

                    if (x.ParameterName.Equals("session", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_BPD_PI @date1, @date1, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_ListBDP.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

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
      }

      lstCustoms.Clear();
      lstParams.Clear();

      return result;      
    }
  }
}
