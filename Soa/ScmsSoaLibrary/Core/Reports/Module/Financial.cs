using System;
using System.Collections.Generic;
using System.Text;
using ScmsSoaLibraryInterface.Components;
using ScmsSoaLibrary.Commons;
using System.Data.SqlClient;
using System.IO;

namespace ScmsSoaLibrary.Core.Reports.Module
{
  class Financial
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
      string rtpPath = string.Concat(cfg.PathReport, @"Financial\");
      string errMessage = null;

      Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> dicRptDatasetParam = null;
      DateTime date1 = DateTime.MinValue,
        date2 = DateTime.MinValue;
      System.Data.DataSet dataSet = null;
      ScmsModel.ORMDataContext db = null;

      ReportDatasetBind rptBind = new ReportDatasetBind();

      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {
          #region Financial

          #region AP

          #region Faktur Pending

          case Constant.REPORT_AP_FAKTUR_PENDING_RINGKASAN:
            {
              rptName = "Finance AP Faktur Pending - Ringkasan";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Sql

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("supplier", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("top", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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


                  tmpQuery = "Exec LG_RptBeli8 @supplier,@supplier,@date1,@date2,@top,@session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_BeliFA7.rpt");

                  #endregion
                }
              }
              isGenerated = true;
            }
            break;

          case Constant.REPORT_AP_FAKTUR_PENDING_DETILFAKTUR:
            {
              rptName = "Finance AP Faktur Pending - Detil";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Sql

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("supplier", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("top", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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


                  tmpQuery = "Exec LG_RptBeli8 @supplier,@supplier,@date1,@date2,@top,@session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_BeliFA6.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region GL

          case Constant.REPORT_AP_GL:
            {
              rptName = "Finance AP GL";

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

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("supplier", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_RptBeli1 @supplier,@supplier,@date1,@date2,@session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_BeliFA1.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region List

          case Constant.REPORT_AP_LIST_RINGKASAN:
            {
              rptName = "Finance AP List - Ringkasan";

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

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("supplier", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_RptBeli2_1 @supplier,@supplier,@date1,@date2,@session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_BeliFA2.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          case Constant.REPORT_AP_LIST_DETILFAKTUR:
            {
              rptName = "Finance AP List - Detil";

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

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("supplier", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_RptBeli2_1 @supplier,@supplier,@date1,@date2,@session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_BeliFA2-1.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          case Constant.REPORT_AP_LIST_DETILTRANSAKSI:
            {
              rptName = "Finance AP List - Detil Transaksi";

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

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("supplier", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_RptBeli2 @supplier,@supplier,@date1,@date2,@session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_BeliFA2-2.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region List Bayar

          case Constant.REPORT_AP_LIST_BAYAR_TIPE_A:
            {
              rptName = "Finance AP List Bayar - Tipe A";

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

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("supplier", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_RptBeli10 @supplier,@supplier,@date1,@date2,@session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_BeliFA8.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          case Constant.REPORT_AP_LIST_BAYAR_TIPE_B:
            {
              rptName = "Finance AP List Bayar - Tipe B";

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

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("supplier", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_RptBeli9 @supplier,@supplier,@date1,@date2,@session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_BeliFA9.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #endregion

          #region AR

          #region Faktur Pending

          case Constant.REPORT_AR_FAKTUR_PENDING_RINGKASAN:
            {
              rptName = "Finance AR Faktur Pending - Ringkasan";

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

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("customer", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("top", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_RptJual8 @customer,@customer,@date1,@date2,@top,@session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_JualFA7.rpt");

                  #endregion
                }
              }
              isGenerated = true;
            }
            break;

          case Constant.REPORT_AR_FAKTUR_PENDING_DETILFAKTUR:
            {
              rptName = "Finance AR Faktur Pending - Detil";

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

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("customer", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("top", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_RptJual8 @customer,@customer,@date1,@date2,@top,@session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_JualFA6.rpt");

                  #endregion
                }
              }
              isGenerated = true;
            }
            break;

          #endregion

          #region GL

          case Constant.REPORT_AR_GL:
            {
              rptName = "Finance AR GL";

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

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("customer", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_RptJual1 @customer, @customer,@date1,@date2,@session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_JualFA1.rpt");

                  #endregion
                }
              }
              isGenerated = true;
            }
            break;

          #endregion

          #region List

          case Constant.REPORT_AR_LIST_RINGKASAN:
            {
              rptName = "Finance AR List - Ringkasan";

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

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("customer", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_RptJual2_1 @customer,@date1,@date2,@session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_JualFA2-2.rpt");

                  #endregion
                }
              }
              isGenerated = true;
            }
            break;

          case Constant.REPORT_AR_LIST_DETILFAKTUR:
            {
              rptName = "Finance AR List - Detil";

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

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("customer", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_RptJual2_1 @customer,@date1,@date2,@session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_JualFA2-1.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          case Constant.REPORT_AR_LIST_DETILTRANSAKSI:
            {
              rptName = "Finance AR List - Detil Transaksi";

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

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("customer", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_RptJual2 @customer,@date1,@date2,@session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_JualFA2.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region List Bayar

          case Constant.REPORT_AR_LIST_BAYAR_TIPE_A:
            {
              rptName = "Finance AR List Bayar - Tipe A";

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

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("customer", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_RptJual9 @customer,@date1,@date2,@session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_JualFA8.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          case Constant.REPORT_AR_LIST_BAYAR_TIPE_B:
            {
              rptName = "Finance AR List Bayar - Tipe B";

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

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("customer", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_RptJual9 @customer,@date1,@date2,@session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_JualFA9.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #endregion

          #region HPP

          #region Div AMS

          case Constant.REPORT_HPP_DIV_AMS_DETIL_CLAIM:
            {
              rptName = "Finance HPP - Detil Div AMS Claim";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_HPPDivAMS1.rpt");

                  #endregion
                }
              }
              isGenerated = true;
            }
            break;

          case Constant.REPORT_HPP_DIV_AMS_DETIL_NON_CLAIM:
            {
              rptName = "Finance HPP - Detil Div AMS Non Claim";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_HPPDivAMS3.rpt");

                  #endregion
                }
              }
              isGenerated = true;
            }
            break;

          case Constant.REPORT_HPP_DIV_AMS_SUMMARI_CLAIM:
            {
              rptName = "Finance HPP - Summary Div AMS Claim";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_HPPDivAMS2.rpt");

                  #endregion
                }
              }
              isGenerated = true;
            }
            break;

          case Constant.REPORT_HPP_DIV_AMS_SUMMARI_NON_CLAIM:
            {
              rptName = "Finance HPP - Summary Div AMS Non Claim";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_HPPDivAMS4.rpt");

                  #endregion
                }
              }
              isGenerated = true;
            }
            break;

          case Constant.REPORT_HPP_DIV_AMS_SHORT:
            {
              rptName = "Finance HPP - Short Div AMS Claim";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_HPPDivAMS5.rpt");

                  #endregion
                }
              }
              isGenerated = true;
            }
            break;


          #endregion

          #region Div Prin

          case Constant.REPORT_HPP_DIV_PRINS_DETIL_CLAIM:
            {
              rptName = "Finance HPP - Detil Div Prins Claim";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_HPPDivPri1.rpt");

                  #endregion
                }
              }
              isGenerated = true;
            }
            break;

          case Constant.REPORT_HPP_DIV_PRINS_DETIL_NON_CLAIM:
            {
              rptName = "Finance HPP - Detil Div Prins Non Claim";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_HPPDivPri3.rpt");

                  #endregion
                }
              }
              isGenerated = true;
            }
            break;

          case Constant.REPORT_HPP_DIV_PRINS_SUMMARI_CLAIM:
            {
              rptName = "Finance HPP - Summary Div Prins Claim";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_HPPDivPri2.rpt");

                  #endregion
                }
              }
              isGenerated = true;
            }
            break;

          case Constant.REPORT_HPP_DIV_PRINS_SUMMARI_NON_CLAIM:
            {
              rptName = "Finance HPP - Summary Div Prins Non Claim";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_HPPDivPri4.rpt");

                  #endregion
                }
              }
              isGenerated = true;
            }
            break;

          #endregion

          #endregion

          #region BEA

          case Constant.REPORT_BEA_DETIL:
            {
              rptName = "Finance BEA - Detil";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_BEA.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.ReportBeaKirim(db, dicRptDatasetParam);

                  if (rptBind.IsError)
                  {
                    errMessage = rptBind.ErrorMessage;

                    isGenerated = false;
                  }
                  else if (dataSet == null)
                  {
                    errMessage = "Failed to fetch data";

                    isGenerated = false;
                  }
                  else
                  {
                    isGenerated = true;
                  }

                  #endregion
                }
              }
              isGenerated = true;
            }
            break;

          case Constant.REPORT_BEA_FAKTUR:
            {
              rptName = "Finance BEA - Per-Faktur";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_BEAFaktur.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.ReportBeaKirim(db, dicRptDatasetParam);

                  if (rptBind.IsError)
                  {
                    errMessage = rptBind.ErrorMessage;

                    isGenerated = false;
                  }
                  else if (dataSet == null)
                  {
                    errMessage = "Failed to fetch data";

                    isGenerated = false;
                  }
                  else
                  {
                    isGenerated = true;
                  }

                  #endregion
                }
              }
              isGenerated = true;
            }
            break;

          case Constant.REPORT_BEA_SUMMARI:
            {
              rptName = "Finance BEA - Per-Divisi AMS";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_BEADiv.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.ReportBeaKirim(db, dicRptDatasetParam);

                  if (rptBind.IsError)
                  {
                    errMessage = rptBind.ErrorMessage;

                    isGenerated = false;
                  }
                  else if (dataSet == null)
                  {
                    errMessage = "Failed to fetch data";

                    isGenerated = false;
                  }
                  else
                  {
                    isGenerated = true;
                  }

                  #endregion
                }
              }
              isGenerated = true;
            }
            break;

          #endregion

          #region CLAIM

          case Constant.REPORT_CLAIM:
            {
                rptName = "Report Claim";

                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
                        #region Sql

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("tipereport", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                            {
                                isOk = true;
                            }

                            return isOk;
                        });

                        string tipereport = rptParam.ParameterValue;

                        switch (tipereport)
                        {
                            case "01":
                                #region Report Claim Total
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

                                    if (x.ParameterName.Equals("baseondetail", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                                tmpQuery = "Exec LG_RptClaim @tahun,@date1,@baseondetail,@session";
                                reportFiles = Path.Combine(rtpPath, "LG_Claim2.rpt");

                                break;
                                #endregion

                            case "02":
                                #region Report Claim Detail
                                reportFiles = Path.Combine(rtpPath, "LG_Claim1.rpt");
                                break;
                                #endregion
                        }

                        #endregion

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);
                    }
                }

                isGenerated = true;
            }
            break;
            #endregion

          #region CLAIM ACC

          case Constant.REPORT_CLAIM_ACC:
            {
                rptName = "Report Claim Acc";

                reportFiles = Path.Combine(rtpPath, "LG_ClaimAcc.rpt");

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                isGenerated = true;
            }
            break;

          #endregion

          #region E-Faktur FJ

          case Constant.REPORT_EFAKTUR_FJ:
            {
                rptName = "Report E-Faktur FJ";

                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        reportFiles = Path.Combine(rtpPath, "LG_EFaktur_FJ.rpt");
                    }
                }

                isGenerated = true;
            }
            break;
          #endregion

          #region E-Faktur FM

          case Constant.REPORT_EFAKTUR_FM:
            {
                rptName = "Report E-Faktur FM";

                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        reportFiles = Path.Combine(rtpPath, "LG_EFaktur_FM.rpt");
                    }
                }

                isGenerated = true;
            }
            break;
          #endregion

          #region Faktur Manual

          case Constant.REPORT_FAKTUR_MANUAL:
            {
                rptName = "Report Faktur Manual";

                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        reportFiles = Path.Combine(rtpPath, "LG_FakturManual.rpt");
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
