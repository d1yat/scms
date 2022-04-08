using System;
using System.Collections.Generic;
using System.Text;
using ScmsSoaLibraryInterface.Components;
using ScmsSoaLibrary.Commons;
using System.Data.SqlClient;
using System.IO;

namespace ScmsSoaLibrary.Core.Reports.Module
{
  class WaktuPelayanan
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
      string rtpPath = string.Concat(cfg.PathReport, @"WaktuPelayanan\");

      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {
          #region Cabang

          #region Detail

          case Constant.REPORT_WAKTUPELAYANAN_CABANG_DETAIL:
            {
              rptName = "Waktu Pelayanan Cabang - Detail";

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

                    if (x.ParameterName.Equals("tipeTrx", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                    {
                      isOk = true;
                    }

                    return isOk;
                  });
                  string tipeReport = rptParam.ParameterValue;

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

                    if (x.ParameterName.Equals("gdg", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("suplier", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("divSuplier", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("item", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_CalcWP @date1, @date2, @tipeTrx, @gdg, @customer, @suplier, @divSuplier, @item, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  if (tipeReport == "06" || tipeReport == "10")
                  {
                      reportFiles = Path.Combine(rtpPath, "LG_WPPLDO1.rpt");
                  }
                  else
                  {
                      reportFiles = Path.Combine(rtpPath, "LG_WP1.rpt");
                  }

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Summary Divisi Prinsipal

          case Constant.REPORT_WAKTUPELAYANAN_CABANG_SUM_DIVPRIN:
            {
              rptName = "Waktu Pelayanan Cabang - Summary Divisi Prinsipal";

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

                    if (x.ParameterName.Equals("tipeTrx", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("gdg", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("suplier", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("divSuplier", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("item", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_CalcWP @date1, @date2, @tipeTrx, @gdg, @customer, @suplier, @divSuplier, @item, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_WP2.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion
            
          #region Summary Prinsipal - Cabang

          case Constant.REPORT_WAKTUPELAYANAN_CABANG_SUM_CABANG:
            {
              rptName = "Waktu Pelayanan Cabang - Summary Cabang";

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

                    if (x.ParameterName.Equals("tipeTrx", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("gdg", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("suplier", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("divSuplier", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("item", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_CalcWP @date1, @date2, @tipeTrx, @gdg, @customer, @suplier, @divSuplier, @item, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_WP3.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #endregion

          #region Prinsipal

          #region Detail

          case Constant.REPORT_WAKTUPELAYANAN_PRINCIPAL_DETAIL:
            {
              rptName = "Waktu Pelayanan Pemasok - Detail";

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

                    if (x.ParameterName.Equals("tipeTrx", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("gdg", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("suplier", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("divSuplier", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("item", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_CalcWP_supplier @date1, @date2, @tipeTrx, @gdg, @suplier, @divSuplier, @item, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_WP_Supplier1.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Summary Divisi Prinsipal

          case Constant.REPORT_WAKTUPELAYANAN_PRINCIPAL_SUM_DIVPRIN:
            {
              rptName = "Waktu Pelayanan Pemasok - Summary Divisi Prinsipal";

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

                    if (x.ParameterName.Equals("tipeTrx", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("gdg", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("suplier", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("divSuplier", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("item", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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
                  tmpQuery = "Exec LG_CalcWP_supplier @date1, @date2, @tipeTrx, @gdg, @suplier, @divSuplier, @item, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_WP_Supplier2.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Summary Prinsipal - Cabang

          case Constant.REPORT_WAKTUPELAYANAN_PRINCIPAL_SUM_CABANG:
            {
              rptName = "Waktu Pelayanan Pemasok - Summary Cabang";

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

                    if (x.ParameterName.Equals("tipeTrx", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("gdg", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("suplier", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("divSuplier", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                    if (x.ParameterName.Equals("item", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_CalcWP_supplier @date1, @date2, @tipeTrx, @gdg, @suplier, @divSuplier, @item, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_WP_Supplier3.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #endregion
            
          #region REPORT_WAKTUPELAYANAN_PRINCIPAL_CABANG

          case Constant.REPORT_WAKTUPELAYANAN_PRINCIPAL_CABANG:
            {
              rptName = "Waktu Pelayanan Suplier Cabang";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_ReportSupplierCabang.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

            #endregion

          #region Serah Terima

          #region Detail

          case Constant.REPORT_WAKTUPELAYANAN_SERAH_TERIMA:
            {
                rptName = "Waktu Pelayanan Serah Terima";

                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
                        #region Sql
                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("TipeReport", StringComparison.OrdinalIgnoreCase))
                            {
                                isOk = true;
                            }
                            return isOk;
                        });
                        string tipereport = rptParam.ParameterValue;

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("Tipe", StringComparison.OrdinalIgnoreCase))
                            {
                                isOk = true;
                            }
                            return isOk;
                        });
                        string tipe = string.Empty;
                        if (rptParam != null)
                        {
                            tipe = rptParam.ParameterValue;
                        }

                        if (!string.IsNullOrEmpty(tipe))
                        {
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
                                    Size = 2,
                                    Value = rptParam.ParameterValue
                                });
                            }

                            tmpQuery = "Exec LG_CalcWP_Full @date1, @date2, @session, @gudang";
                        }

                        #endregion

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        if (string.IsNullOrEmpty(tipe))
                        {
                            switch (tipereport)
                            {
                                case "01":
                                    reportFiles = Path.Combine(rtpPath, "LG_WPST1.rpt");
                                    break;
                                case "02":
                                    reportFiles = Path.Combine(rtpPath, "LG_WPST2.rpt");
                                    break;
                                case "03":
                                    reportFiles = Path.Combine(rtpPath, "LG_WPST3.rpt");
                                    break;
                            }
                        }
                        else
                        {
                            switch (tipereport)
                            {
                                case "01":
                                    reportFiles = Path.Combine(rtpPath, "LG_WPSTFull1.rpt");
                                    break;
                                case "02":
                                    reportFiles = Path.Combine(rtpPath, "LG_WPSTFull2.rpt");
                                    break;
                                case "03":
                                    reportFiles = Path.Combine(rtpPath, "LG_WPSTFull3.rpt");
                                    break;
                            }
                        }
                        #endregion
                    }
                }

                isGenerated = true;
            }
            break;

          #endregion
          #endregion

          #region Analisa SP - PL Confirm

          #region Detail

          case Constant.REPORT_WAKTUPELAYANAN_ANALISA_SPPLCONFIRM_DETAIL:
            {
                rptName = "Waktu Pelayanan SP - PL Confirm - Detail";

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

                            if (x.ParameterName.Equals("tipeTrx", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                            if (x.ParameterName.Equals("gdg", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                            if (x.ParameterName.Equals("cusno", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        tmpQuery = "Exec LG_CALC_ANALISA_WP @date1, @date2, @cusno, @tipeTrx, @gdg, @session";


                        #endregion

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        reportFiles = Path.Combine(rtpPath, "LG_ANALISA_WP_DETAIL.rpt");

                        #endregion
                    }
                }

                isGenerated = true;
            }
            break;

          #endregion

          #region Summary

          case Constant.REPORT_WAKTUPELAYANAN_ANALISA_SPPLCONFIRM_SUM:
            {
                rptName = "Waktu Pelayanan SP - PL Confirm - Summary";

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

                            if (x.ParameterName.Equals("tipeTrx", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                            if (x.ParameterName.Equals("gdg", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                            if (x.ParameterName.Equals("cusno", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        tmpQuery = "Exec LG_CALC_ANALISA_WP @date1, @date2, @cusno, @tipeTrx, @gdg, @session";
                        

                        #endregion

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        reportFiles = Path.Combine(rtpPath, "LG_ANALISA_WP_SUM.rpt");

                        #endregion
                    }
                }

                isGenerated = true;
            }
            break;

          #endregion

          #endregion

          #region Serah Terima Gudang Retur

          case Constant.REPORT_WAKTUPELAYANAN_SERAH_TERIMA_GUDANG_RETUR:
            {
                rptName = "Waktu Pelayanan Serah Terima Gudang Retur";

                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
                        #region Sql
                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("TipeReport", StringComparison.OrdinalIgnoreCase))
                            {
                                isOk = true;
                            }
                            return isOk;
                        });
                        string tipereport = rptParam.ParameterValue;
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

                            if (x.ParameterName.Equals("gdg", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                            if (x.ParameterName.Equals("cusno", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                            if (x.ParameterName.Equals("divpri", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                            if (x.ParameterName.Equals("iteno", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        tmpQuery = "Exec lg_wpst_gdgretur @date1, @date2, @TipeReport, @gdg, @cusno, @nosup, @divpri, @iteno";

                        #endregion

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        if (!string.IsNullOrEmpty(tipereport))
                        {
                            switch (tipereport)
                            {
                                case "PBBR":
                                    reportFiles = Path.Combine(rtpPath, "LG_WPSTGR_PBBR.rpt");
                                    break;
                                case "CPR":
                                    reportFiles = Path.Combine(rtpPath, "LG_WPSTGR_CPR.rpt");
                                    break;
                                case "BASPB":
                                    reportFiles = Path.Combine(rtpPath, "LG_WPSTGR_BASPB.rpt");
                                    break;
                            }
                        }
                        #endregion
                    }
                }

                isGenerated = true;
            }
            break;
          #endregion

          #region Process Monitoring
          //Indra Process Monitoring 20180523FM
          case Constant.REPORT_PROCESS_MONITORING :
            {
                rptName = "Process Monitoring";

                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {                        
                        
                        #region Parameter

                        #region Tipe report

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("Tipereport", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        #region Tipe Report 1

                        #region Gudang2

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("Gudang2", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        #region SP Received Time

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

                        #region Cabang

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("cabang2", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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
                                Size = 150,
                                Value = rptParam.ParameterValue
                            });
                        }

                        #endregion

                        #region User

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

                        #endregion

                        #region Tipe Report 2

                        #region Gudang

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

                        #region Div AMS

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("DivAMS", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        #region Cabang

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("Cabang", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        #region SP Cabang

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("NoSPCab", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        #region SP HO

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("NoSPHO", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        #region Status

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

                        #endregion

                        #region Kode Item

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("KdItem", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        #endregion

                        #endregion

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        switch (tipereport)
                        {
                            case "01":
                                rptName = "Process Monitoring";
                                tmpQuery = "Exec SP_Rpt_ProcessMonitoring @Gudang2, @date1, @date2, @cabang2, @user";
                                reportFiles = Path.Combine(rtpPath, "LG_Rpt_ProcessMonitoring.rpt");                                
                                break;  
 
                            case "02":
                                rptName = "Process Monitoring";
                                tmpQuery = "Exec SP_RPT_PROCESS_MONITORING @GUDANG, @DIVAMS, @CABANG, @NoSPCab, @NoSPHO, @STATUS, @KdItem";
                                reportFiles = Path.Combine(rtpPath, "LG_PROCESSMONITORING.rpt");
                                break;                            
                        }

                        
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
