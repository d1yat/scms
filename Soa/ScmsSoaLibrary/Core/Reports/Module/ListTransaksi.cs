using System;
using System.Collections.Generic;
using System.Text;
using ScmsSoaLibraryInterface.Components;
using ScmsSoaLibrary.Commons;
using System.Data.SqlClient;
using System.IO;

namespace ScmsSoaLibrary.Core.Reports.Module
{
  class ListTransaksi
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
      string rtpPath = string.Concat(cfg.PathReport, @"ListTransaksi\");

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
          #region List Transaksi

          #region Trans PO

          case Constant.REPORT_LIST_TRANSAKSI_PO_DETIL:
            {
              rptName = "List Transaksi - PO (Detil)";

              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "LG_ListPODetil.rpt");

              isGenerated = true;
            }
            break;

          case Constant.REPORT_LIST_TRANSAKSI_PO_PERPO:
            {
              rptName = "List Transaksi - PO (Per No.)";

              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "LG_ListPOTotal.rpt");

              isGenerated = true;
            }
            break;

          #endregion

          #region Combo

          case Constant.REPORT_LIST_TRANSAKSI_COMBO:
            {
              rptName = "List Transaksi - Combo";

              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "LG_ListCombo.rpt");

              isGenerated = true;
            }
            break;

          #endregion

          #region List RN

          case Constant.REPORT_LIST_TRANSAKSI_RN:
            {
              rptName = "List Transaksi - Receive Note";

              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "LG_ListRN.rpt");

              isGenerated = true;
            }
            break;

          #endregion

          #region List RC

          case Constant.REPORT_LIST_TRANSAKSI_RC:
            {
              rptName = "List Transaksi - Retur Customer";

              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "LG_ListRC.rpt");

              isGenerated = true;
            }
            break;

          #endregion

          #region STT

          case Constant.REPORT_LIST_TRANSAKSI_STT:
            {
                rptName = "List Transaksi - Surat Tanda Terima";

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
                        reportFiles = Path.Combine(rtpPath, "LG_ListSTT.rpt");
                        tmpQuery = "Exec LG_RptListSTT @date1,@date2,@gdg,@nosup, @divpri, @iteno, @type ,@user";

                        #endregion
                    }
                }
                isGenerated = true;
            }
            break;

          #endregion

          #region RS

          case Constant.REPORT_LIST_TRANSAKSI_RS:
            {
              rptName = "List Transaksi - Retur Supplier";

              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "LG_ListRS.rpt");

               


              isGenerated = true;
            }
            break;

          #endregion

          #region Packing List

          case Constant.REPORT_LIST_TRANSAKSI_PL:
            {
              rptName = "List Transaksi - Packing List";

              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "LG_ListPL.rpt");

              isGenerated = true;
            }
            break;

          #endregion

          #region Surat Jalan

          case Constant.REPORT_LIST_TRANSAKSI_SJ:
            {
                              
              if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
              {
                  

                  #region Sql

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                      bool isOk = false;

                      if (x.ParameterName.Equals("Date1", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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


                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                      bool isOk = false;

                      if (x.ParameterName.Equals("gdg1", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                      if (x.ParameterName.Equals("gdg2", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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


                  string tipereport = null;
                  if (rptParam != null)
                  {
                    
                    tipereport= rptParam.ParameterValue;
                  }

                  #endregion
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);



                  if (string.IsNullOrEmpty(tipereport))
                  {
                      rptName = "List Transaksi - Surat Jalan";
                      reportFiles = Path.Combine(rtpPath, "LG_ListSJ.rpt");
                  }
                  else
                  {
                      rptName = "List Transaksi - Surat Jalan Pending RN";

                      reportFiles = Path.Combine(rtpPath, "LG_ListSJ_PendingRN.rpt");

                      tmpQuery = "EXEC Lg_Transfergudang_PendingRN @Date1, @Date2, @Nosup, @Iteno, @gdg1, @gdg2 ";


                  }




                  #endregion
              }

                
              isGenerated = true;
            }
            break;

          #endregion

          #region PO Khusus

          case Constant.REPORT_LIST_TRANSAKSI_POK:
            {
              rptName = "List Transaksi - PO Khusus";

              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "LG_POPrincSPCab.rpt");

              isGenerated = true;
            }
            break;

          #endregion

          #region SPPackingList

          case Constant.REPORT_LIST_TRANSAKSI_SPPL_DETIL:
            {
              rptName = "List Transaksi - Surat Pesanan - Packing List (Detil)";

              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "LG_ListSPAccDetil.rpt");

              isGenerated = true;
            }
            break;

          case Constant.REPORT_LIST_TRANSAKSI_SPPL_PERITEM:
            {
              rptName = "List Transaksi - Surat Pesanan - Packing List (Per Item)";

              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "LG_ListSPAccPerItem.rpt");

              isGenerated = true;
            }
            break;

          case Constant.REPORT_LIST_TRANSAKSI_SPPL_PER_PER_DIVPRI:
            {
              rptName = "List Transaksi - Surat Pesanan - Packing List (Divisi Prinsipal)";

              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "LG_ListSPAccPerDivPri.rpt");

              isGenerated = true;
            }
            break;

          #endregion

          #region List Penjualan

          case Constant.REPORT_LIST_TRANSAKSI_PENJUALAN:
            {
              rptName = "List Transaksi - Penjualan";

              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "LG_QueryDO.rpt");

              isGenerated = true;
            }
            break;

          #endregion

          #region List Penjualan - Return DO Indra 20170803

          case Constant.REPORT_LIST_TRANSAKSI_RETURNDO:
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

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);
                        rptName = "Report Return DO";
                        reportFiles = Path.Combine(rtpPath, "LG_rptReturnDO.rpt");
                        tmpQuery = "Exec LG_RPTRETURNDO @date1, @date2, @gdg, @cusno, @tipe, @user";

                        #endregion
                    }

                isGenerated = true;
            }
            break;

          #endregion

          #region List Penjualan - Retur Penjualan

          case Constant.REPORT_LIST_TRANSAKSI_RETUR_PENJUALAN:
            {
                rptName = "List Transaksi - Retur Penjualan";

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                reportFiles = Path.Combine(rtpPath, "LG_QueryRC.rpt");

                isGenerated = true;
            }
            break;

          #endregion

          #region List Penjualan - Net Penjualan

          case Constant.REPORT_LIST_TRANSAKSI_NET_PENJUALAN:
            {
                rptName = "List Transaksi - Net Penjualan";

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

                        #endregion

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);
                        reportFiles = Path.Combine(rtpPath, "LG_QueryDORC.rpt");
                        tmpQuery = "Exec LG_ReportDORC @date1,@date2,@gdg,@cusno,@user";
          
                        #endregion
                    }
                }
                isGenerated = true;
            }
            break;

          #endregion

          #region SP - Exp

          case Constant.REPORT_LIST_TRANSAKSI_SPEXP:
            {
              rptName = "List Transaksi - Surat Pesanan - Expedisi";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Sql

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("C_SPNO1", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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
                      Value = rptParam.ParameterRawValue<DateTime>(DateTime.MinValue)
                    });
                  }

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("C_SPNO2", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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
                      Value = rptParam.ParameterRawValue<DateTime>(DateTime.MinValue)
                    });
                  }

                  rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                  {
                    bool isOk = false;

                    if (x.ParameterName.Equals("c_user", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                  tmpQuery = "Exec LG_CalcSPEXPS @C_SPNO1,@C_SPNO2,@c_cusno";

                  #endregion

                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_SPEkspedisi.rpt");

                  #endregion
                }
              }
              isGenerated = true;
            }
            break;

          #endregion

          #region Adjust

          #region Stock

          case Constant.REPORT_LIST_TRANSAKSI_ADJ_STOCK:
            {
              rptName = "List Transaksi - Adjustment Stock";

              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "LG_ListAdjustStock.rpt");

              isGenerated = true;
            }
            break;

          #endregion

          #region SP

          case Constant.REPORT_LIST_TRANSAKSI_ADJ_SP:
            {
              rptName = "List Transaksi - Adjustment Surat Pesanan";

              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "LG_ListAdjSP.rpt");

              isGenerated = true;
            }
            break;

          #endregion

          #region PO

          case Constant.REPORT_LIST_TRANSAKSI_ADJ_PO:
            {
              rptName = "List Transaksi - Adjustment Purchase Order";

              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "LG_ListAdjPO.rpt");

              isGenerated = true;
            }
            break;

          #endregion

          #region STT

          case Constant.REPORT_LIST_TRANSAKSI_ADJ_STT:
            {
              rptName = "List Transaksi - Adjustment Surat Tanda Terima";

              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "LG_ListAdjSTT.rpt");

              isGenerated = true;
            }
            break;

          #endregion

          #region Combo

          case Constant.REPORT_LIST_TRANSAKSI_ADJ_COMBO:
            {
              rptName = "List Transaksi - Adjustment Combo";

              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "LG_ListAdjCombo.rpt");

              isGenerated = true;
            }
            break;

          #endregion

          #region FB

          case Constant.REPORT_LIST_TRANSAKSI_ADJ_FB:
            {
              rptName = "List Transaksi - Adjustment Faktur Beli";

              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "LG_ListFBadj.rpt");

              isGenerated = true;
            }
            break;

          #endregion

          #region FJ

          case Constant.REPORT_LIST_TRANSAKSI_ADJ_FJ:
            {
              rptName = "List Transaksi - Adjustment Faktur Jual";

              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "LG_ListFJadj.rpt");

              isGenerated = true;
            }
            break;

          #endregion

          #endregion

          #region Floating

          case Constant.REPORT_LIST_FLOATING:
            {
              rptName = "List Transaksi - Receive Note (Floating)";

              if (rpt.ReportParameter != null)
              {
                if (rpt.ReportParameter.Length > 0)
                {
                  #region Report

                  rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  reportFiles = Path.Combine(rtpPath, "LG_FloatingStock.rpt");

                  #endregion

                  #region Dataset Filter

                  dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

                  db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

                  dataSet = rptBind.ReportFloating(db, dicRptDatasetParam);

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

          #region PBF

          case Constant.REPORT_LIST_PBF:
            {
                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
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
                        //Indra 20190320FM
                        if (tipereport == "07")
                        {
                            rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                            {
                                bool isOk = false;

                                if (x.ParameterName.Equals("periode", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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
                        }
                        else
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

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);


                        switch (tipereport)
                        {
                            case "01":
                                rptName = "Report PBF Non Retur";
                                //Indra 20170927
                                //reportFiles = Path.Combine(rtpPath, "LG_ReportPBFNonRetur.rpt");
                                //tmpQuery = "Exec LG_RptKartuBarangTotalPBF @date1,@date2, @user, @gdg, @TipeReport";
                                reportFiles = Path.Combine(rtpPath, "LG_ReportPBFNonRetur2.rpt");
                                tmpQuery = "Exec LG_RptKartuBarangTotalPBF2 @date1,@date2, @user, @gdg, @TipeReport";
                                break;
                            case "02":
                                rptName = "Report PBF Retur";
                                //Indra 20170927
                                //reportFiles = Path.Combine(rtpPath, "LG_ReportPBFRetur.rpt");
                                //tmpQuery = "Exec LG_RptKartuBarangTotalPBF @date1,@date2, @user, @gdg, @TipeReport";
                                reportFiles = Path.Combine(rtpPath, "LG_ReportPBFRetur2.rpt");
                                tmpQuery = "Exec LG_RptKartuBarangTotalPBF2 @date1,@date2, @user, @gdg, @TipeReport";
                                break;
                            case "03":
                                rptName = "Report New PBF Non Retur";
                                reportFiles = Path.Combine(rtpPath, "LG_ReportPBFNonRetur_NEW.rpt");

                                tmpQuery = "Exec LG_RptKartuBarangTotalPBF_New @date1,@date2, @user, @gdg, @TipeReport";
                                break;
                            case "04":
                                rptName = "Report New PBF Retur";
                                reportFiles = Path.Combine(rtpPath, "LG_ReportPBFRetur_NEW.rpt");

                                tmpQuery = "Exec LG_RptKartuBarangTotalPBF_New @date1,@date2, @user, @gdg, @TipeReport";
                                break;
                            //Indra 20170404
                            case "05":
                                rptName = "Report BPOM PBF";
                                reportFiles = Path.Combine(rtpPath, "LG_ReportPBF_BPOM.rpt");

                                tmpQuery = "Exec LG_RptKartuBarangTotalPBF_BPOM @date1,@date2, @user, @gdg, @TipeReport";
                                break;
                            //Indra 20190320FM Laporan PBF New per 2019
                            case "07":
                                rptName = "Report PBF Triwulan";
                                reportFiles = Path.Combine(rtpPath, "LG_ReportPBF_Triwulan.rpt");

                                tmpQuery = "Exec SP_RPT_LAPORANPBF @tahun,@periode, @gdg, @user";
                                break;
                        }

                        

                        #endregion
                    }
                }

                isGenerated = true;
            }
            break;

          #endregion

          //#region PBF OLD 23 APRIL 2015

          //case Constant.REPORT_LIST_PBF:
          //  {
          //      rptName = "PBF";

          //      if (rpt.ReportParameter != null)
          //      {
          //          if (rpt.ReportParameter.Length > 0)
          //          {
          //              #region Report

          //              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

          //              reportFiles = Path.Combine(rtpPath, "LG_rptPBF.rpt");

          //              #endregion

          //              #region Dataset Filter

          //              dicRptDatasetParam = Functionals.ReportParameterDatasetBuilder(rpt.ReportParameter);

          //              db = new ScmsModel.ORMDataContext(Functionals.ActiveConnectionString);

          //              dataSet = rptBind.ReportPBF(db, dicRptDatasetParam);

          //              if (rptBind.IsError)
          //              {
          //                  errMessage = rptBind.ErrorMessage;

          //                  isGenerated = false;
          //              }
          //              else if (dataSet == null)
          //              {
          //                  errMessage = "Failed to fetch data";

          //                  isGenerated = false;
          //              }
          //              else
          //              {
          //                  isGenerated = true;
          //              }

          //              #endregion
          //          }
          //      }
          //  }
          //  break;

          //#endregion

          #region OKTPREKURSOR BULANAN DO

          case Constant.REPORT_LIST_OKTPREKURSOR_BULANAN_DO:
            {
              rptName = "Report OKT Prekursor Bulanan DO";

              rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

              reportFiles = Path.Combine(rtpPath, "LG_ReportOktPrekursorBulananDO.rpt");

              isGenerated = true;
            }
            break;

          #endregion

          #region List Transaksi Surat Pesanan

          case Constant.REPORT_LIST_TRANSAKSI_SP:
            {
                rptName = "List Transaksi - Surat Pesanan";

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                reportFiles = Path.Combine(rtpPath, "LG_ListSP.rpt");

                isGenerated = true;
            }
            break;

          #endregion

          #region OKTPREKURSOR BULANAN PO

          case Constant.REPORT_LIST_OKTPREKURSOR_BULANAN_PO:
            {
                rptName = "Report OKT Prekursor Bulanan PO";

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                reportFiles = Path.Combine(rtpPath, "LG_ReportOktPrekursorBulananPO.rpt");

                isGenerated = true;
            }
            break;

          #endregion

          #region List Transaksi Pembelian

          case Constant.REPORT_LIST_TRANSAKSI_PEMBELIAN:
            {
                rptName = "Report Transaksi Pembelian";

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                reportFiles = Path.Combine(rtpPath, "LG_Pembelian.rpt");

                isGenerated = true;
            }
            break;

          #endregion

          #region E-Napza Psikotropika & Prekursor

          case Constant.REPORT_LIST_ENAPZA:
            {
                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
                        #region Tipe Report

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

                        #endregion

                        #region Tgl Report

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

                        #region Gudang

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

                        #endregion

                        #region user 

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

                        #region Bentuk Report

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("BentukReport", StringComparison.OrdinalIgnoreCase))
                            {
                                isOk = true;
                            }
                            return isOk;
                        });
                        string Bentuk = rptParam.ParameterValue;

                        #endregion

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        if (Bentuk == "01")
                        {
                            reportFiles = Path.Combine(rtpPath, "LG_ReportENapza.rpt");
                            tmpQuery = "Exec LG_RptKartuBarangEnapza @date1,@date2, @user, @gdg, @TipeReport";   
                        }
                        else
                        {
                            reportFiles = Path.Combine(rtpPath, "LG_ReportENapza_HO.rpt");
                            tmpQuery = "Exec LG_RptKartuBarangEnapza3 @date1,@date2, @user, @gdg, @TipeReport";
                        }

                        //20170529 Indra D. Penambahan Produk OOT
                        if (tipereport == "07")
                        {
                            rptName = "Report E-Napza Prekursor";
                        }
                        else if (tipereport == "02")
                        {
                            rptName = "Report E-Napza Psikotropika";
                        }
                        else
                        {
                            rptName = "Report E-Napza OOT";
                        }                        

                        #endregion
                    }
                }

                isGenerated = true;
            }
            break;

          #endregion

          #region E-Alkes

          case Constant.REPORT_LIST_EALKES:
            {
                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
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

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        //Indra 20170327 Perubahan Report
                        switch (tipereport)
                        {
                            case "01":
                                rptName = "Report E-Alkes Data Masuk";
                                //reportFiles = Path.Combine(rtpPath, "LG_ReportEalkesIn.rpt");
                                reportFiles = Path.Combine(rtpPath, "LG_ReportEalkesIn_2.rpt");
                                break;
                            case "02":
                                rptName = "Report E-Alkes Data Keluar";
                                //reportFiles = Path.Combine(rtpPath, "LG_ReportEalkesOut.rpt");
                                reportFiles = Path.Combine(rtpPath, "LG_ReportEalkesOut_2.rpt");
                                break;
                            case "03":
                                rptName = "Report E-Alkes Data Retur Masuk";
                                //reportFiles = Path.Combine(rtpPath, "LG_ReportEalkesInRetur.rpt");
                                reportFiles = Path.Combine(rtpPath, "LG_ReportEalkesInRetur_2.rpt");
                                break;
                            case "04":
                                rptName = "Report E-Alkes Data Retur Keluar";
                                //reportFiles = Path.Combine(rtpPath, "LG_ReportEalkesOutRetur.rpt");
                                reportFiles = Path.Combine(rtpPath, "LG_ReportEalkesOutRetur_2.rpt");
                                break;
                        }

                        tmpQuery = "Exec LG_RptKartuBarangEalkes @date1,@date2, @user, @gdg, @TipeReport";

                        #endregion
                    }
                }

                isGenerated = true;
            }
            break;

          #endregion

          #region Customer Service Level

          case Constant.REPORT_LIST_CSL:
            {
                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
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

                            if (x.ParameterName.Equals("cusnosup", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                        #region Report

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);


                        switch (tipereport)
                        {
                            case "PH":
                                rptName = "Report CSL Principal (Header)";
                                reportFiles = Path.Combine(rtpPath, "LG_ReportCSLPrincipalHdr.rpt");
                                break;
                            case "PD":
                                rptName = "Report CSL Principal (Detail)";
                                reportFiles = Path.Combine(rtpPath, "LG_ReportCSLPrincipalDtlNew.rpt");
                                break;
                            case "CH":
                                rptName = "Report CSL Cabang (Header)";
                                reportFiles = Path.Combine(rtpPath, "LG_ReportCSLCabangHdr.rpt");
                                break;
                            case "CD":
                                rptName = "Report CSL Cabang (Detail)";
                                reportFiles = Path.Combine(rtpPath, "LG_ReportCSLCabangDtl.rpt");
                                break;
                        }

                        tmpQuery = "Exec LG_CalcCSL @TipeReport, @date1, @date2, @cusnosup";

                        #endregion
                    }
                }

                isGenerated = true;
            }
            break;

          #endregion

          #region Purchase Order Apoteker

          case Constant.REPORT_TRANSAKSI_PO_APOTEKER:
            {
                rptName = "Report PO Apoteker";

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
                            case "02":
                                #region Report Claim Detail
                                rptName = "SURAT PESANAN PSIKOTROPIKA";
                                reportFiles = Path.Combine(rtpPath, "LG_PO_OKT_New.rpt");
                                break;
                                #endregion

                            case "07":
                                #region Report Claim Detail
                                rptName = "SURAT PESANAN PREKURSOR";
                                reportFiles = Path.Combine(rtpPath, "LG_PO_PREKURSOR_New.rpt");
                                break;
                                #endregion
                            case "09":
                                #region Report Claim Detail
                                rptName = "SURAT PESANAN OBAT-OBAT TERTENTU";
                                reportFiles = Path.Combine(rtpPath, "LG_PO_OOT_New.rpt");
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
