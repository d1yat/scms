using System;
using System.Collections.Generic;
using System.Text;
using ScmsSoaLibraryInterface.Components;
using ScmsSoaLibrary.Commons;
using System.Data.SqlClient;
using System.IO;

namespace ScmsSoaLibrary.Core.Reports.Module
{
  class Pending
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
      string rtpPath = string.Concat(cfg.PathReport, @"Pending\");

      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {
          #region Surat Pesanan

          #region Transaksi

          case Constant.REPORT_PENDING_SURATPESANAN_TOTAL:
            {
              rptName = "Pending Surat Pesanan - Total";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_SPPending1.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          case Constant.REPORT_PENDING_SURATPESANAN_DETAIL_PERITEM:
            {
              rptName = "Pending Surat Pesanan - Detail Per-Item";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_SPPending2.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          case Constant.REPORT_PENDING_SURATPESANAN_DETAIL_PERSP:
            {
              rptName = "Pending Surat Pesanan - Detail Per-Surat Pesanan";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_SPPending3.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          case Constant.REPORT_PENDING_SURATPESANAN_DETAIL_PERCUSTOMER:
            {
              rptName = "Pending Surat Pesanan - Detail Per-Customer";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_SPPending4.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;
            
          case Constant.REPORT_PENDING_SURATPESANAN_BULANAN:
            {
              rptName = "Pending Surat Pesanan - Bulanan";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_ReportSPPendingMonth.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          case Constant.REPORT_PENDING_SURATPESANAN_BULANAN_PO:
            {
                rptName = "Pending Surat Pesanan - Bulanan PO";

                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        reportFiles = Path.Combine(rtpPath, "LG_ReportSPPendingMonthPO.rpt");

                        #endregion
                    }
                }

                isGenerated = true;
            }
            break;


          #endregion

          #region Gudang

          case Constant.REPORT_PENDING_SURATPESANAN_GUDANG:
            {
              rptName = "Pending Surat Pesanan Gudang";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_rptSPGPending.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #endregion

          #region Purchase Order

          #region Logistik

          case Constant.REPORT_PENDING_PURCHASEORDER_LOGISTIK_TYPE_1:
            {
              rptName = "Pending Purchase Order (Logistik) - Detail";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_POPending1.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          case Constant.REPORT_PENDING_PURCHASEORDER_LOGISTIK_TYPE_2:
            {
              rptName = "Pending Purchase Order (Logistik) - Total";

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

                  tmpQuery = "Exec LG_CalcPOvsRN @date1, @date2, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_POPending2.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Finance

          case Constant.REPORT_PENDING_PURCHASEORDER_FINANCE_DETAIL:
            {
              rptName = "Pending Purchase Order (Finance) - Detail";

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

                  tmpQuery = "Exec LG_CalcPendingPOPrincipal @date1, @date2, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_POPPending1.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          case Constant.REPORT_PENDING_PURCHASEORDER_FINANCE_TOTAL:
            {
              rptName = "Pending Purchase Order (Finance) - Total";

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

                  tmpQuery = "Exec LG_CalcPendingPOPrincipal @date1, @date2, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_POPPending2.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Versus

          case Constant.REPORT_PENDING_PURCHASEORDER_VS_SURATPESANAN:
            {
              rptName = "Pending Purchase Order vs Surat Pesanan";

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

                  tmpQuery = "Exec LG_RptSPPO @date1, @date2, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_ReportSPPOPend.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region PO Periodik

          case Constant.REPORT_PENDING_POPERIODIK:
            {
                rptName = "Pending Purchase Order Periodik";

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

                            if (x.ParameterName.Equals("principal", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                            if (x.ParameterName.Equals("divprincipal", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                            if (x.ParameterName.Equals("divams", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        tmpQuery = "Exec LG_RptPOPendingPeriodik @date2, @principal, @divprincipal, @divams, @iteno, @session";

                        #endregion

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        reportFiles = Path.Combine(rtpPath, "LG_ReportPOPendingPeriodik.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          //Indra D. 20170312
          #region Do Belum diRN

          case Constant.REPORT_PENDING_DOBELUMRN:
            {
                rptName = "Pending Principle DO Belum RN";

                if (rpt.ReportParameter != null)
                {
                    #region Report

                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                    reportFiles = Path.Combine(rtpPath, "LG_ReportDOBelumRN.rpt");

                    #endregion
                }

                isGenerated = true;
            }
            break;

          #endregion

          #endregion

          

          #region Delivery Order

          case Constant.REPORT_PENDING_DELIVERYORDER:
            {
                rptName = "Pending Delivery Order";

                
                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
                        #region Sql

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("List", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        reportFiles = Path.Combine(rtpPath, "LG_ListPendDO.rpt");

                        tmpQuery = "exec LG_ListPendingDO @List, @gdg, @cusno, @date1, @date2";

                        #endregion

                    }
                }

                isGenerated = true;
            }
            break;

          #endregion



          #region Retur Customer

          case Constant.REPORT_PENDING_RETURCUSTOMER:
            {
              rptName = "Pending Retur Customer";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_ListPendRC.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Receive Note

          case Constant.REPORT_PENDING_RECEIVENOTE:
            {
              rptName = "Pending Receive Note";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_ListPendRN.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Retur Suplier Summary

          case Constant.REPORT_PENDING_RETURSUPLIER_SUMMARY:
            {
              rptName = "Pending Retur Suplier Summary";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_ListPendRS.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Retur Suplier Detail

          case Constant.REPORT_PENDING_RETURSUPLIER_DETAIL:
            {
                rptName = "Pending Retur Suplier Detail";

                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
                        #region Sql

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("rtype", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                            if (x.ParameterName.Equals("rsno1", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                            if (x.ParameterName.Equals("rsno2", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        tmpQuery = "Exec LG_RptPendingListRSDetail @rtype, @gdg, @nosup, @date1,@date2, @rsno1, @rsno2, @iteno, @session";

                        #endregion

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        reportFiles = Path.Combine(rtpPath, "LG_ListPendRS_Detail.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Surat Tanda Terima

          case Constant.REPORT_PENDING_SURATTANDATERIMA:
            {
              rptName = "Pending Surat Tanda Terima";

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

                      if (x.ParameterName.Equals("stt1", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                      if (x.ParameterName.Equals("stt2", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_rptSTTPend_new @stt1, @stt2, @date1, @date2, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_ListPendSTT.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Packing List

          case Constant.REPORT_PENDING_PACKINGLIST:
            {
              rptName = "Pending Packing List";

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

                  tmpQuery = "Exec LG_CalcPendingPL @date1, @date2, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_ReportPendingPL.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Combo

          case Constant.REPORT_PENDING_COMBO:
            {
              rptName = "Pending Combo";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_ReportPendingCombo.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Surat Jalan

          case Constant.REPORT_PENDING_SURATJALAN:
            {
              rptName = "Pending Surat Jalan";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  //Indra 20170809
                    if (rpt.ReportParameter.Length > 0)

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

                        if (x.ParameterName.Equals("GdgAsal", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        if (x.ParameterName.Equals("GdgTujuan", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        if (x.ParameterName.Equals("Pemasok", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        if (x.ParameterName.Equals("tipe", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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
                  //End
                  
                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);
                  rptName = "Report Pending SJ"; //Indra 201708009
                  reportFiles = Path.Combine(rtpPath, "LG_ReportPendingSJ.rpt");
                  tmpQuery = "Exec LG_RPTPENDINGSJ @date1, @date2, @GdgAsal, @GdgTujuan, @Pemasok, @tipe, @user"; //Indra 201708009

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
