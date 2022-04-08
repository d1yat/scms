using System;
using System.Collections.Generic;
using System.Text;
using ScmsSoaLibraryInterface.Components;
using ScmsSoaLibrary.Commons;
using System.Data.SqlClient;
using System.IO;

namespace ScmsSoaLibrary.Core.Reports.Module
{
  class History
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
      string rtpPath = string.Concat(cfg.PathReport, @"History\");

      string errMessage = null;

      Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> dicRptDatasetParam = null;

      DateTime date1 = DateTime.MinValue,
        date2 = DateTime.MinValue;
      System.Data.DataSet dataSet = null;

      ReportDatasetBind rptBind = new ReportDatasetBind();

      ScmsModel.ORMDataContext db = null;

      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {
          #region History

          #region Asuransi

          case Constant.REPORT_HISTORY_ASURANSI:
            {
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

                  
                  ////

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
                  string tipereport = rptParam.ParameterValue;

                  //
                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                      bool isOk = false;

                      if (x.ParameterName.Equals("ID", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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
                  //
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
                          Size = 2,
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
                          Size = 4,
                          Value = rptParam.ParameterValue
                      });
                  }

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                      bool isOk = false;

                      if (x.ParameterName.Equals("exp", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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
                  tmpQuery = "Exec LG_LapAsuransi_total @date1, @date2, @type, @ID, @gdg, @cusno, @exp";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);


                  if (tipereport == "Detail")
                  {
                      rptName = "History Asuransi Detai";
                      reportFiles = Path.Combine(rtpPath, "LG_PLDOExp.rpt");
                  }
                  else
                  {
                      rptName = "History Asuransi Total";
                      reportFiles = Path.Combine(rtpPath, "LG_PLDOExp_total.rpt");
                  }
                                   

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion




          #region PO

          #region PO Transaksi

          case Constant.REPORT_HISTORY_PO_TRANSAKSI:
            {
              rptName = "History Purchase Order (PO) Transaksi";

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
                        System.Data.SqlDbType.VarChar, 7)
                    {
                      Size = 15,
                      Value = rptParam.ParameterValue
                    });
                  }

                  tmpQuery = "Exec LG_CalcHistoryPO @date1, @date2, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_HistoryPO.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region PO Limit

          #region Detail

          case Constant.REPORT_HISTORY_PO_LIMIT_DETAIL:
            {
              rptName = "History Purchase Order (PO) Budget Limit Detail";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_POLimit1.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Total

          case Constant.REPORT_HISTORY_PO_LIMIT_TOTAL:
            {
              rptName = "History Purchase Order (PO) Budget Limit Total";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_POLimit2.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #endregion

          #endregion

          #region SP

          case Constant.REPORT_HISTORY_SURATPESANAN:
            {
              rptName = "History Surat Pesanan (SP)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Sql

                  //rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  //{
                  //  bool isOk = false;

                  //  if (x.ParameterName.Equals("sp1", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                  //  {
                  //    isOk = true;
                  //  }

                  //  return isOk;
                  //});
                  //if (rptParam != null)
                  //{
                  //  lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                  //      System.Data.SqlDbType.VarChar, 10)
                  //  {
                  //    Value = rptParam.ParameterRawValue<string>(string.Empty)
                  //  });
                  //}

                  //rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  //{
                  //  bool isOk = false;

                  //  if (x.ParameterName.Equals("sp2", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                  //  {
                  //    isOk = true;
                  //  }

                  //  return isOk;
                  //});
                  //if (rptParam != null)
                  //{
                  //  lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                  //      System.Data.SqlDbType.VarChar, 10)
                  //  {
                  //    Value = rptParam.ParameterRawValue<string>(string.Empty)
                  //  });
                  //}

                  //rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  //{
                  //  bool isOk = false;

                  //  if (x.ParameterName.Equals("date1", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                  //  {
                  //    isOk = true;
                  //  }

                  //  return isOk;
                  //});
                  //if (rptParam != null)
                  //{
                  //  lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                  //      System.Data.SqlDbType.DateTime)
                  //  {
                  //    Value = rptParam.ParameterRawValue<DateTime>(Functionals.StandardSqlDateTime)
                  //  });
                  //}

                  //rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  //{
                  //  bool isOk = false;

                  //  if (x.ParameterName.Equals("date2", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                  //  {
                  //    isOk = true;
                  //  }

                  //  return isOk;
                  //});
                  //if (rptParam != null)
                  //{
                  //  lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                  //      System.Data.SqlDbType.DateTime)
                  //  {
                  //    Value = rptParam.ParameterRawValue<DateTime>(Functionals.StandardSqlDateTime)
                  //  });
                  //}

                  //rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  //{
                  //  bool isOk = false;

                  //  if (x.ParameterName.Equals("customer", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                  //  {
                  //    isOk = true;
                  //  }

                  //  return isOk;
                  //});
                  //if (rptParam != null)
                  //{
                  //  lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                  //      System.Data.SqlDbType.VarChar, 7)
                  //  {
                  //    Size = 15,
                  //    Value = rptParam.ParameterValue
                  //  });
                  //}

                  //rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  //{
                  //  bool isOk = false;

                  //  if (x.ParameterName.Equals("item", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                  //  {
                  //    isOk = true;
                  //  }

                  //  return isOk;
                  //});
                  //if (rptParam != null)
                  //{
                  //  lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                  //      System.Data.SqlDbType.VarChar, 7)
                  //  {
                  //    Size = 15,
                  //    Value = rptParam.ParameterValue
                  //  });
                  //}

                  //rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  //{
                  //  bool isOk = false;

                  //  if (x.ParameterName.Equals("session", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                  //  {
                  //    isOk = true;
                  //  }

                  //  return isOk;
                  //});
                  //if (rptParam != null)
                  //{
                  //  lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                  //      System.Data.SqlDbType.VarChar, 7)
                  //  {
                  //    Size = 15,
                  //    Value = rptParam.ParameterValue
                  //  });
                  //}

                  ////tmpQuery = "Exec LG_CalcHistoryPO @pono1, @pono2, @session";
                  //tmpQuery = "Exec LG_CalcHistorySP @sp1, @sp2, @date1, @date2, @customer, @item, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "COPY OF LG_HISTORYSP.RPT");

                  #endregion
                  
                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.ReportHistorySP(db, dicRptDatasetParam);

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
            }
            break;

          #endregion

          #region SPG

          case Constant.REPORT_HISTORY_SURATPESANANGUDANG:
            {
              rptName = "History Surat Pesanan Gudang (SPG)";

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

                        if (x.ParameterName.Equals("nosup", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                        {
                            isOk = true;
                        }

                        return isOk;
                    });
                    if (rptParam != null)
                    {
                        lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                            System.Data.SqlDbType.VarChar, 7)
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
                            System.Data.SqlDbType.VarChar, 7)
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
                            System.Data.SqlDbType.VarChar, 7)
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
                        System.Data.SqlDbType.VarChar, 7)
                    {
                      Size = 15,
                      Value = rptParam.ParameterValue
                    });
                  }

                  tmpQuery = "Exec LG_CalcHistorySPG @date1, @date2, @nosup, @divpri, @iteno, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_HistorySPG.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region SP Batal

          case Constant.REPORT_HISTORY_SURATPESANANBATAL:
            {
              rptName = "History Surat Pesanan (SP) Batal";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_ReportSPBatal.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region STT

          case Constant.REPORT_HISTORY_STT:
            {
              rptName = "History Surat Tanda Terima (STT)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Sql

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("memoStt1", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                    {
                      isOk = true;
                    }

                    return isOk;
                  });
                  if (rptParam != null)
                  {
                    lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                        System.Data.SqlDbType.VarChar, 10)
                    {
                      Value = rptParam.ParameterRawValue<string>(string.Empty)
                    });
                  }

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("memoStt2", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                    {
                      isOk = true;
                    }

                    return isOk;
                  });
                  if (rptParam != null)
                  {
                    lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                        System.Data.SqlDbType.VarChar, 10)
                    {
                      Value = rptParam.ParameterRawValue<string>(string.Empty)
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
                        System.Data.SqlDbType.VarChar, 7)
                    {
                      Size = 15,
                      Value = rptParam.ParameterValue
                    });
                  }

                  tmpQuery = "Exec LG_CalcHistorySTT @memoStt1, @memoStt2, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_HistorySTT.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Block Item

          case Constant.REPORT_HISTORY_ITEMBLOCK:
            {
              rptName = "History Status Produk";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_ReportHistoryItemBlock.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Combo

          case Constant.REPORT_HISTORY_COMBO:
            {
              rptName = "History Combo";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Sql

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("memoCombo1", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                    {
                      isOk = true;
                    }

                    return isOk;
                  });
                  if (rptParam != null)
                  {
                    lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                        System.Data.SqlDbType.VarChar, 10)
                    {
                      Value = rptParam.ParameterRawValue<string>(string.Empty)
                    });
                  }

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("memoCombo2", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                    {
                      isOk = true;
                    }

                    return isOk;
                  });
                  if (rptParam != null)
                  {
                    lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                        System.Data.SqlDbType.VarChar, 10)
                    {
                      Value = rptParam.ParameterRawValue<string>(string.Empty)
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
                        System.Data.SqlDbType.VarChar, 7)
                    {
                      Size = 15,
                      Value = rptParam.ParameterValue
                    });
                  }

                  tmpQuery = "Exec LG_CalcHistoryCombo @memoCombo1, @memoCombo2, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_HistoryCombo.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Item-Batch

          #region Gudang

          case Constant.REPORT_HISTORY_ITEMBATCH_GUDANG:
            {
              rptName = "History Item-Batch Gudang";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Sql

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
                        System.Data.SqlDbType.VarChar, 4)
                    {
                      Value = rptParam.ParameterRawValue<string>(string.Empty)
                    });
                  }

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("batch1", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                    {
                      isOk = true;
                    }

                    return isOk;
                  });
                  if (rptParam != null)
                  {
                    lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                        System.Data.SqlDbType.VarChar, 10)
                    {
                      Value = rptParam.ParameterRawValue<string>(string.Empty)
                    });
                  }

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("batch2", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                    {
                      isOk = true;
                    }

                    return isOk;
                  });
                  if (rptParam != null)
                  {
                    lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                        System.Data.SqlDbType.VarChar, 10)
                    {
                      Value = rptParam.ParameterRawValue<string>(string.Empty)
                    });
                  }

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("tanggal", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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
                        System.Data.SqlDbType.VarChar, 7)
                    {
                      Size = 15,
                      Value = rptParam.ParameterValue
                    });
                  }

                  tmpQuery = "Exec LG_RptHistoryItem @item, @batch1, @batch2, @tanggal, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_HistoryItemBatch1.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Nasional

          case Constant.REPORT_HISTORY_ITEMBATCH_NASIONAL:
            {
              rptName = "History Item-Batch Nasional";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Sql

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
                        System.Data.SqlDbType.VarChar, 4)
                    {
                      Value = rptParam.ParameterRawValue<string>(string.Empty)
                    });
                  }

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("batch1", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                    {
                      isOk = true;
                    }

                    return isOk;
                  });
                  if (rptParam != null)
                  {
                    lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                        System.Data.SqlDbType.VarChar, 10)
                    {
                      Value = rptParam.ParameterRawValue<string>(string.Empty)
                    });
                  }

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("batch2", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                    {
                      isOk = true;
                    }

                    return isOk;
                  });
                  if (rptParam != null)
                  {
                    lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                        System.Data.SqlDbType.VarChar, 10)
                    {
                      Value = rptParam.ParameterRawValue<string>(string.Empty)
                    });
                  }

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("tanggal", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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
                        System.Data.SqlDbType.VarChar, 7)
                    {
                      Size = 15,
                      Value = rptParam.ParameterValue
                    });
                  }

                  tmpQuery = "Exec LG_RptHistoryItem @item, @batch1, @batch2, @tanggal, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_HistoryItemBatch2.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #endregion

          #region Expedisi

          case Constant.REPORT_HISTORY_EXPEDISI:
            {
              rptName = "History Expedisi";

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
                      Value = rptParam.ParameterRawValue<string>(string.Empty)
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
                      Value = rptParam.ParameterRawValue<string>(string.Empty)
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
                        System.Data.SqlDbType.VarChar, 7)
                    {
                      Size = 15,
                      Value = rptParam.ParameterValue
                    });
                  }

                  tmpQuery = "Exec LG_CalcExpCab @date1, @date2, @session";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_ReportExpHoCab.rpt");

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Query Claim

          case Constant.REPORT_HISTORY_QUERY_CLAIM:
            {
              rptName = "History Query Claim - (Reguler)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_QueryClaimProduk.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.QueryClaim(db, dicRptDatasetParam);

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

          case Constant.REPORT_HISTORY_QUERY_CLAIM_PER_SUPPLIER:
            {
              rptName = "History Query Claim - (Supplier)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_QueryClaimPerSupplier.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.QueryClaim(db, dicRptDatasetParam);

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

          case Constant.REPORT_HISTORY_QUERY_CLAIM_PER_DIV_AMS:
            {
              rptName = "History Query Claim - (Divisi AMS)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_QueryClaimPerDiv.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.QueryClaim(db, dicRptDatasetParam);

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

          #region Query Pembelian

          case Constant.REPORT_HISTORY_QUERY_FB:
            {
              rptName = "History Query Pembelian (Beli)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  //asli awal
                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  //reportFiles = Path.Combine(rtpPath, "LG_QueryFB.rpt");

                  //asli akhir

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
                      case "Pembelian":
                          rptName = "Report Query Pembelian (Header)";
                          reportFiles = Path.Combine(rtpPath, "LG_QueryFB.rpt");
                          break;
                      case "PRN":
                          rptName = "Report Query Pembelian (RN)";
                          reportFiles = Path.Combine(rtpPath, "LG_QueryFB-RN.rpt");
                          break;
                      case "PPrinRN":
                          rptName = "Report Query Pembelian (Principal)";
                          reportFiles = Path.Combine(rtpPath, "LG_QueryFB-PRIN.rpt");
                          break;
                      case "PAmsRN":
                          rptName = "Report Query Pembelian (Divisi Ams)";
                          reportFiles = Path.Combine(rtpPath, "LG_QueryFB-AMS.rpt");
                          break;
                  }
                 
                  
                  
                  
                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          case Constant.REPORT_HISTORY_QUERY_FBR:
            {
              rptName = "History Query Pembelian (Retur)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {

                    // asli awal
                    //#region Report

                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                   // reportFiles = Path.Combine(rtpPath, "LG_QueryFBR.rpt");
                   //  #endregion
                  
                    //asli akhir



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
                      case "ReturPembelian":
                          rptName = "Report Query Pembelian Retur (Header)";
                          reportFiles = Path.Combine(rtpPath, "LG_QueryFBR_lama.rpt");
                          break;
                      case "PBR":
                          rptName = "Report Query Pembelian (BR)";
                          reportFiles = Path.Combine(rtpPath, "LG_QueryFBR-BR.rpt");
                          break;
                      case "PPrinBR":
                          rptName = "Report Query Pembelian (Principal)";
                          reportFiles = Path.Combine(rtpPath, "LG_QueryFBR-PRIN.rpt");
                          break;
                      case "PAmsBR":
                          rptName = "Report Query Pembelian (Divisi Ams)";
                          reportFiles = Path.Combine(rtpPath, "LG_QueryFBR-AMS.rpt");
                          break;
                  }

                    

                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Query Penjualan

          case Constant.REPORT_HISTORY_QUERY_FJ:
            {
              rptName = "History Query Penjualan (Jual)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  //#region Report

                  //rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  //reportFiles = Path.Combine(rtpPath, "LG_QueryFJ.rpt");

                  //#endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);
                                 
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
                      case "Penjualan":
                          rptName = "Report Query Penjualan (Header)";
                          reportFiles = Path.Combine(rtpPath, "LG_QueryFJ.rpt");
                          break;
                      case "FJ":
                          rptName = "Report Query Penjualan (FJ)";
                          reportFiles = Path.Combine(rtpPath, "LG_QueryFJ-Nomor.rpt");
                          break;
                      case "PPrinFJ":
                          rptName = "Report Query Penjualan (Principal)";
                          reportFiles = Path.Combine(rtpPath, "LG_QueryFJ-Prin.rpt");
                          break;
                      case "PAmsFJ":
                          rptName = "Report Query Penjualan (FJ Ams)";
                          reportFiles = Path.Combine(rtpPath, "LG_QueryFJ- AMS.rpt");
                          break;
                  }




                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          case Constant.REPORT_HISTORY_QUERY_FJR:
            {
              rptName = "History Query Penjualan (Retur)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  //#region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  //reportFiles = Path.Combine(rtpPath, "LG_QueryFJR.rpt");

                  //#endregion

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
                      case "ReturPenjualan":
                          rptName = "Report Query Penjualan Retur (Header)";
                          reportFiles = Path.Combine(rtpPath, "LG_QueryFJR.rpt");
                          break;
                      case "JR":
                          rptName = "Report Query Penjualan (JR)";
                          reportFiles = Path.Combine(rtpPath, "LG_QueryFJR-Nomor.rpt");
                          break;
                      case "PPrinJR":
                          rptName = "Report Query Pembelian (Principal)";
                          reportFiles = Path.Combine(rtpPath, "LG_QueryFJR-Prin.rpt");
                          break;
                      case "PAmsJR":
                          rptName = "Report Query Pembelian (JR Ams)";
                          reportFiles = Path.Combine(rtpPath, "LG_QueryFJR-AMS.rpt");
                          break;
                  }



                  #endregion
                }
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Query Sales

          case Constant.REPORT_HISTORY_QUERY_SALES_NONRETUR_SUM:
            {
              rptName = "History Query Sales Non Retur (Total)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_QuerySales.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.ReportSales(db, dicRptDatasetParam);

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
            }
            break;

          case Constant.REPORT_HISTORY_QUERY_SALES_NONRETUR_CABANG:
            {
              rptName = "History Query Sales (Per Cabang)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_QuerySalesPerCabang.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.ReportSales(db, dicRptDatasetParam);

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
            }
            break;

          case Constant.REPORT_HISTORY_QUERY_SALES_NONRETUR_FAKTUR:
            {
              rptName = "History Query Sales (Per Faktur)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_QuerySalesPerFaktur.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.ReportSales(db, dicRptDatasetParam);

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
            }
            break;

          #endregion

          #region Query Retur Jual

          case Constant.REPORT_HISTORY_QUERY_SALES_RETUR_FAKTUR:
            {
              rptName = "History Query Return Sales (Per No Faktur)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_QueryReturSalesPerFak.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.QueryReturJual(db, dicRptDatasetParam);

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

          case Constant.REPORT_HISTORY_QUERY_SALES_RETUR_CABANG:
            {
              rptName = "History Query Return Sales (Per Cabang)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_QueryReturSalesPerCabang.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.QueryReturJual(db, dicRptDatasetParam);

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

          case Constant.REPORT_HISTORY_QUERY_SALES_RETUR_SUM:
            {
              rptName = "History Query Retur Sales (Per Div AMS)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_QueryReturSales.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.QueryReturJual(db, dicRptDatasetParam);

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

          #region Query Purchase

          case Constant.REPORT_HISTORY_QUERY_PURCHASE_FAKTUR:
            {
              rptName = "History Query Purchase (Per No Faktur)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_QueryPurchasePerFaktur.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);
                  
                  dataSet = rptBind.QueryPurchase(db, dicRptDatasetParam);

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

          case Constant.REPORT_HISTORY_QUERY_PURCHASE_CABANG:
            {
              rptName = "History Query Purchase (Per Supplier)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_QueryPurchasePerSupplier.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);
                  db.CommandTimeout = 5000;

                  dataSet = rptBind.PurchasePerFB(db, dicRptDatasetParam);

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

          case Constant.REPORT_HISTORY_QUERY_PURCHASE_SUM:
            {
              rptName = "History Query Purchase (Per Div AMS)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_QueryPurchase.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.PurchasePerFB(db, dicRptDatasetParam);

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

          #region Query Retur Beli

          case Constant.REPORT_HISTORY_QUERY_RETUR_PURCHASE_FAKTUR:
            {
              rptName = "History Query Return Purchase (Per No Faktur)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_QueryReturSalesPerFak.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.QueryReturBeli(db, dicRptDatasetParam);

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

          case Constant.REPORT_HISTORY_QUERY_RETUR_PURCHASE_CABANG:
            {
              rptName = "History Query Return Purchase (Per Supplier)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_QueryReturBeliPerSup.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.QueryReturBeli(db, dicRptDatasetParam);

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

          case Constant.REPORT_HISTORY_QUERY_RETUR_PURCHASE_SUM:
            {
              rptName = "History Query Retur Purchase (Per Div AMS)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_QueryReturBeli.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.QueryReturBeli(db, dicRptDatasetParam);

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

          #region Query Saldo

          case Constant.REPORT_HISTORY_QUERY_SALDO_DEBIT:
            {
              rptName = "History Query Saldo Debitur";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_ReportSaldeb.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.ReportSalDeb(db, dicRptDatasetParam);

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

          case Constant.REPORT_HISTORY_QUERY_SALDO_DEBIT_SUM:
            {
              rptName = "History Query Saldo Debitur (SUMMARY)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_ReportSaldebSum.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.ReportSalDeb(db, dicRptDatasetParam);

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

          case Constant.REPORT_HISTORY_QUERY_SALDO_KREDIT:
            {
              rptName = "History Query Saldo Kreditur";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_ReportSalkred.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.ReportSalKret(db, dicRptDatasetParam);

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

          case Constant.REPORT_HISTORY_QUERY_SALDO_KREDIT_SUM:
            {
              rptName = "History Query Saldo Kreditur (SUMMARY)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_ReportSalkredSum.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.ReportSalKret(db, dicRptDatasetParam);

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

          #region Resi Expedisi

          case Constant.REPORT_HISTORY_RESI_EKSPEDISI:
            {
                rptName = "History Resi Ekspedisi";

                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
                        #region Sql
                        #endregion

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        reportFiles = Path.Combine(rtpPath, "LG_ReportResiEkspedisi.rpt");

                        #endregion
                    }
                }

                isGenerated = true;
            }
            break;

          #endregion

          #region Resi Expedisi DO

          case Constant.REPORT_HISTORY_RESI_EKSPEDISI_DO:
            {
                rptName = "History Resi Ekspedisi DO";

                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
                        #region Sql
                        #endregion

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        reportFiles = Path.Combine(rtpPath, "LG_ReportEkspedisiDO.rpt");

                        #endregion
                    }
                }

                isGenerated = true;
            }
            break;

          #endregion

          #region Resi Expedisi SJ

          case Constant.REPORT_HISTORY_RESI_EKSPEDISI_SJ:
            {
                rptName = "History Resi Ekspedisi_SJ";

                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
                        #region Sql
                        #endregion

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        reportFiles = Path.Combine(rtpPath, "LG_ReportEkspedisiSJ.rpt");

                        #endregion
                    }
                }

                isGenerated = true;
            }
            break;

          #endregion

          #region Shipment Modify by Indra 20170814

          case Constant.REPORT_HISTORY_SHIPMENT:
            {
                if (rpt.ReportParameter != null)
                {
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

                            if (x.ParameterName.Equals("ekspedisi", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        rptName = "History Shipment";

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        reportFiles = Path.Combine(rtpPath, "LG_ReportShipment.rpt");

                        tmpQuery = "Exec LG_RPTHISTORYSHIPMENT @date1, @date2, @gdg, @cusno, @ekspedisi, @user";
                    
                }

                isGenerated = true;
            }
            break;

          #endregion

          #region Biaya Ekspedisi

          case Constant.REPORT_HISTORY_BIAYA_EKSPEDISI:
            {
                rptName = "Report Biaya Ekspedisi";

                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
                         rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("tipereport", StringComparison.OrdinalIgnoreCase))
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

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("exp", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        switch (tipereport)
                        {
                            case "01":
                                rptName = "Biaya Ekspedisi (by Resi)";
                                reportFiles = Path.Combine(rtpPath, "LG_ReportBiayaEkspedisiByResi.rpt");

                                break;
                            case "02":
                                rptName = "Biaya Ekspedisi (by DO)";
                                reportFiles = Path.Combine(rtpPath, "LG_ReportBiayaEkspedisiByDO.rpt");

                                break;
                            case "03":
                                rptName = "Biaya Ekspedisi (by Item)";
                                reportFiles = Path.Combine(rtpPath, "LG_ReportBiayaEkspedisiByItem.rpt");
                                break;
                        }

                        tmpQuery = "Exec LG_CalcBiayaEkspedisi @exp, @date1, @date2, @gdg, @iteno, @tipereport, @user";
                        
                        #endregion
                    }
                }
                isGenerated = true;
            }
            break;

          #endregion

          #region Biaya Invoice Vs Resi

          case Constant.REPORT_HISTORY_INVOICEVSRESI:
            {
                rptName = "Report Biaya Invoice Vs Resi";

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

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("exp", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        //rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        //{
                        //    bool isOk = false;

                        //    if (x.ParameterName.Equals("IENO1", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                        //    {
                        //        isOk = true;
                        //    }

                        //    return isOk;
                        //});
                        //if (rptParam != null)
                        //{
                        //    lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                        //        System.Data.SqlDbType.VarChar)
                        //    {
                        //        Size = 15,
                        //        Value = rptParam.ParameterValue
                        //    });
                        //}

                        //rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        //{
                        //    bool isOk = false;

                        //    if (x.ParameterName.Equals("IENO2", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                        //    {
                        //        isOk = true;
                        //    }

                        //    return isOk;
                        //});
                        //if (rptParam != null)
                        //{
                        //    lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                        //        System.Data.SqlDbType.VarChar)
                        //    {
                        //        Size = 15,
                        //        Value = rptParam.ParameterValue
                        //    });
                        //}

                        #endregion

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);
                        rptName = "Biaya Invoice Vs Resi";
                        reportFiles = Path.Combine(rtpPath, "LG_ReportInvoiceVsResi.rpt");

                        tmpQuery = "Exec LG_CalcBiayaInvoiceVsResi @gdg, @exp, @date1, @date2, @user";

                        #endregion
                    }
                }
                isGenerated = true;
            }
            break;

          #endregion

          #region Rekap Invoice

          case Constant.REPORT_HISTORY_REKAP_INVOICE:
            {
                rptName = "Report Rekap Invoice";

                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("tipereport", StringComparison.OrdinalIgnoreCase))
                            {
                                isOk = true;
                            }
                            return isOk;
                        });
                        string tipereport = rptParam.ParameterValue;
                        
                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        if (tipereport == "eksternal")
                        {
                            reportFiles = Path.Combine(rtpPath, "LG_ReportRekapInvoiceEksternal.rpt");
                        }
                        else
                        {
                            reportFiles = Path.Combine(rtpPath, "LG_ReportRekapInvoiceInternal.rpt");
                        }

                        #endregion
                    }
                }

                isGenerated = true;
            }
            break;

          #endregion

          #region List Pembayaran Ekspedisi

          case Constant.REPORT_HISTORY_LIST_BAYAR_EKSPEDISI:
            {
                rptName = "Report List Pembayaran Ekspedisi";

                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        #endregion
                    }
                }

                reportFiles = Path.Combine(rtpPath, "LG_EKSPEDISI.RPT");

                isGenerated = true;
            }
            break;

          #endregion

          #region PO Periodik

          case Constant.REPORT_HISTORY_PO_PENDING:
            {
                rptName = "Report History PO Pending";

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

                        tmpQuery = "Exec LG_RptPOPending @date1, @date2, @principal, @divprincipal, @divams, @iteno, @session";

                        #endregion

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        reportFiles = Path.Combine(rtpPath, "LG_ReportPOPending.rpt");

                        #endregion
                    }
                }

                isGenerated = true;
            }
            break;

          #endregion

          #region RN Cabang

          case Constant.REPORT_HISTORY_RN_CABANG:
            {
                rptName = "Report RN Cabang";

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

                        //rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        //{
                           // bool isOk = false;

                            //if (x.ParameterName.Equals("phar", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                            //{
                              //  isOk = true;
                            //}

                            //return isOk;
                        //});
                        //if (rptParam != null)
                        //{
                            //lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                                //System.Data.SqlDbType.VarChar)
                            //{
                                //Size = 15,
                                //Value = rptParam.ParameterValue
                            //});
                        //}

                        // hafizh
                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("c_entry", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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
                                Size = 8,
                                Value = rptParam.ParameterValue
                            });
                        }

                        #endregion

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        reportFiles = Path.Combine(rtpPath, "LG_HistoryRNCabang.rpt");

                        tmpQuery = "Exec LG_RNCabang @date1, @date2, @gdg, @cusno, @user, @c_entry";

                        #endregion
                    }
                }
                isGenerated = true;
            }
            break;

          #endregion

          #region Service Level PO

          case Constant.REPORT_HISTORY_SERVICELEVEL_PO:
            {
                rptName = "Service level PO";

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

                            if (x.ParameterName.Equals("Suplier", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                            if (x.ParameterName.Equals("DivPrinsipal", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                            if (x.ParameterName.Equals("divisiAMS", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                            if (x.ParameterName.Equals("Type", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                            if (x.ParameterName.Equals("NoPO", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        switch (tipereport)
                        {
                            case "1":
                                rptName = "Fullfilment Rate";
                                reportFiles = Path.Combine(rtpPath, "LG_FullFilmentratel_New.rpt");
                                tmpQuery = "exec SP_ServiceLevel_PO_NEW @date1, @date2, @Suplier, @DivPrinsipal, @divisiAMS, @Type, @NoPO";
                                break;
                            case "2":
                                rptName = "Line Fullfilment Rate";
                                reportFiles = Path.Combine(rtpPath, "LG_FullFilmentratel_Line_New.rpt");
                                tmpQuery = "exec SP_ServiceLevel_PO_NEW @date1, @date2, @Suplier, @DivPrinsipal, @divisiAMS, @Type, @NoPO";
                                break;
                        }

                        #endregion
                    }
                }
                isGenerated = true;
            }
            break;

          #endregion





          //////////////////////////////
          #region Service Level Proses Cabang

          case Constant.REPORT_HISTORY_SERVICELEVEL_PROSESCABANG:
            {
                rptName = "Service level Proses";

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

                            if (x.ParameterName.Equals("Type", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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






                        #endregion

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        switch (tipereport)
                        {
                            case "1":
                                rptName = "Fullfilment Rate";
                                tmpQuery = "exec SP_ServiceLevel_Cabang_Proses @date1, @date2, @Type";



                                break;
                            case "2":
                                rptName = "Line Fullfilment Rate";
                                tmpQuery = "exec SP_ServiceLevel_Cabang_Proses @date1, @date2, @Type";
                                break;
                        }

                        #endregion
                    }
                }
                isGenerated = true;
            }
            break;

          #endregion
          ///////////////////////////////


          #region Service Level Report Cabang

          case Constant.REPORT_HISTORY_SERVICELEVEL_REPORT_CABANG:
            {
                rptName = "Service level Proses Cabang";

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

                            if (x.ParameterName.Equals("Kode_cabang", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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







                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("Type", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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



                        //rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        //{
                        //    bool isOk = false;

                        //    if (x.ParameterName.Equals("Nip", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                        //    {
                        //        isOk = true;
                        //    }

                        //    return isOk;
                        //});

                        //if (rptParam != null)
                        //{
                        //    lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                        //        System.Data.SqlDbType.VarChar)
                        //    {
                        //        Size = 15,
                        //        Value = rptParam.ParameterValue
                        //    });
                        //}


                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("noSPCBG", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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
                                Size = 20,
                                Value = rptParam.ParameterValue
                            });
                        }





                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("noSPHO", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        switch (tipereport)
                        {
                            case "1":
                                rptName = "Service Level PPIC";
                                reportFiles = Path.Combine(rtpPath, "LG_CSL_PPIC.RPT");
                                tmpQuery = "exec SP_CSL_PPIC @date1, @date2";
                                break;
                            case "2":
                                rptName = "Service Level DC";
                                reportFiles = Path.Combine(rtpPath, "LG_CSL_DC.rpt");
                                tmpQuery = "exec SP_CSL_DC @date1, @date2";
                                break;
                            case "3":
                                rptName = "Service Level Cabang (FL)Minute";
                                reportFiles = Path.Combine(rtpPath, "LG_FullFilmentratel_Cabang_Header_DC.rpt");
                                tmpQuery = "exec SP_ServiceLevel_Cabang_Report @date1, @date2, @Kode_Cabang, @Gudang, @Type, @noSPCBG, @noSPHO";
                                break;
                            case "4":
                                rptName = "Service Level Cabang (LFR) Minute";
                                reportFiles = Path.Combine(rtpPath, "LG_FullFilmentratel_Cabang_Detail_DC.rpt");
                                tmpQuery = "exec SP_ServiceLevel_Cabang_Report @date1, @date2, @Kode_Cabang, @Gudang, @Type, @noSPCBG, @noSPHO";
                                break;
                        }

                        #endregion
                    }
                }
                isGenerated = true;
            }
            break;

          #endregion

          #region recall

          case Constant.REPORT_HISTORY_RECALL_PROSESCABANG:
            {
                rptName = "Proses Recall";

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

                            if (x.ParameterName.Equals("Type", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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






                        #endregion

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        switch (tipereport)
                        {
                            case "1":
                                rptName = "Recall";
                                tmpQuery = "exec SP_Recall_Proses @date1, @date2";



                                break;
                            case "2":
                                rptName = "Recall";
                                tmpQuery = "exec SP_Recall_Proses @date1, @date2";
                                break;
                        }

                        #endregion
                    }
                }
                isGenerated = true;
            }
            break;

          #endregion

          #region REPORT RECALL

          case Constant.REPORT_HISTORY_RECALL_REPORTGENERATE:
            {
                rptName = "Report Recall";

                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {

                        #region Sql

                        #region old
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

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("Type", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                            {
                                isOk = true;
                            }

                            return isOk;
                        });

                        string tipereport = rptParam.ParameterValue;
                        
                        #region old

                        //if (rptParam != null)
                        //{
                        //    lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                        //        System.Data.SqlDbType.VarChar)
                        //    {
                        //        Size = 15,
                        //        Value = rptParam.ParameterValue
                        //    });
                        //}

                        //rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        //{
                        //    bool isOk = false;

                        //    if (x.ParameterName.Equals("item", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                        //    {
                        //        isOk = true;
                        //    }

                        //    return isOk;
                        //});

                        //if (rptParam != null)
                        //{
                        //    lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                        //        System.Data.SqlDbType.VarChar)
                        //    {
                        //        Size = 15,
                        //        Value = rptParam.ParameterValue
                        //    });
                        //}

                        //rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        //{
                        //    bool isOk = false;

                        //    if (x.ParameterName.Equals("Batch", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                        //    {
                        //        isOk = true;
                        //    }

                        //    return isOk;
                        //});

                        //if (rptParam != null)
                        //{
                        //    lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                        //        System.Data.SqlDbType.VarChar)
                        //    {
                        //        Size = 15,
                        //        Value = rptParam.ParameterValue
                        //    });
                        //}

                        #endregion

                        #endregion

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        switch (tipereport)
                        {
                            case "1":
                                rptName = "Recall Header";
                                //reportFiles = Path.Combine(rtpPath, "LG_Recall_Header.rpt");
                                //tmpQuery = "exec SP_Recall_Report_Header @date1, @date2, @Type, @item, @Batch";
                                reportFiles = Path.Combine(rtpPath, "LG_HistoryRecall_Batch.rpt");
                                break;
                            case "2":
                                rptName = "Recall Detail";
                                //reportFiles = Path.Combine(rtpPath, "LG_Recall_Detail.rpt");
                                //tmpQuery = "exec SP_Recall_Report_Header @date1, @date2, @Type, @item, @Batch";
                                reportFiles = Path.Combine(rtpPath, "LG_HistoryRecall_GroupBatch.rpt");

                                break;
                        }

                        #endregion
                    }
                }
                isGenerated = true;
            }
            break;

          #endregion

          #region Pemusnahan

          case Constant.REPORT_HISTORY_PEMUSNAHAN:
            {
                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("tipeReport", StringComparison.OrdinalIgnoreCase))
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

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("origin", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        if (tipereport == "summary")
                        {
                            rptName = "History Pemusnahan (Summary)";
                            reportFiles = Path.Combine(rtpPath, "LG_HistoryPemusnahanSum.rpt");
                            tmpQuery = "Exec LG_CalcHistoryPemusnahan @tipeReport, @date1, @date2, @nosup, @divpri, @iteno, @origin";
                        }
                        else
                        {
                            rptName = "History Pemusnahan (Detail)";
                            reportFiles = Path.Combine(rtpPath, "LG_HistoryPemusnahanDet.rpt");
                            tmpQuery = "Exec LG_CalcHistoryPemusnahan @tipeReport, @date1, @date2, @nosup, @divpri, @iteno, @origin";
                        }

                        #endregion
                    }
                }

                isGenerated = true;
            }
            break;

          #endregion

          ///P14
          #region Retur Supplier

          case Constant.REPORT_HISTORY_RETURSUPPLIER:
            {
                rptName = "History Retur Supplier";

                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
                        #region Sql

                        //rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        //{
                        //    bool isOk = false;

                        //    if (x.ParameterName.Equals("gudang", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                        //    {
                        //        isOk = true;
                        //    }

                        //    return isOk;
                        //});
                        //if (rptParam != null)
                        //{
                        //    lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                        //        System.Data.SqlDbType.VarChar)
                        //    {
                        //        Size = 15,
                        //        Value = rptParam.ParameterValue
                        //    });
                        //}

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

                            if (x.ParameterName.Equals("Date2", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("Nosup", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                            if (x.ParameterName.Equals("Iteno", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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


                        tmpQuery = "Exec LG_RptHistoryReturSupplier @Date1,@Date2,@type,@NIP,@Nosup,@Iteno";


                        #endregion

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        reportFiles = Path.Combine(rtpPath, "LG_HistoryReturSupplier.rpt");
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

      if(dicRptDatasetParam != null)
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
