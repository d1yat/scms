using System;
using System.Collections.Generic;
using System.Text;
using ScmsSoaLibraryInterface.Components;
using ScmsSoaLibrary.Commons;
using System.Data.SqlClient;
using System.IO;

namespace ScmsSoaLibrary.Core.Reports.Module
{
    class Inventory
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
            string rtpPath = string.Concat(cfg.PathReport, @"Inventory\");

            Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> dicRptDatasetParam = null;

            System.Data.DataSet dataSet = null;

            ReportDatasetBind rptBind = new ReportDatasetBind();

            ScmsModel.ORMDataContext db = null;

            string errMessage = null;

            if (rpt != null)
            {
                switch (rpt.ReportingID)
                {
                    #region Inventory

                    #region Stok Gudang

                    case Constant.REPORT_INVENTORY_STOK_GUDANG_BATCH:
                        {
                            rptName = "Stok Gudang Batch";

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
                                            Value = rptParam.ParameterRawValue<DateTime>(DateTime.MinValue)
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
                                            Value = rptParam.ParameterRawValue<DateTime>(DateTime.MinValue)
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

                                    tmpQuery = "Exec LG_RptStock @date1,@date2,@session";

                                    #endregion

                                    #region Report

                                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                                    reportFiles = Path.Combine(rtpPath, "LG_StockGudangBatch.rpt");

                                    #endregion
                                }
                            }

                            isGenerated = true;
                        }
                        break;

                    case Constant.REPORT_INVENTORY_STOK_GUDANG_TOTAL:
                        {
                            rptName = "Stok Gudang Total";

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
                                            Value = rptParam.ParameterRawValue<DateTime>(DateTime.MinValue)
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
                                            Value = rptParam.ParameterRawValue<DateTime>(DateTime.MinValue)
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

                                    tmpQuery = "Exec LG_RptStock @date1,@date2,@session";

                                    #endregion

                                    #region Report

                                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                                    reportFiles = Path.Combine(rtpPath, "LG_StockGudangTotal.rpt");

                                    #endregion
                                }
                            }

                            isGenerated = true;
                        }
                        break;

                    #endregion

                    #region Stok Nasional

                    case Constant.REPORT_INVENTORY_STOK_NASIONAL_BATCH:
                        {
                            rptName = "Stock Nasional Batch";

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
                                            Value = rptParam.ParameterRawValue<DateTime>(DateTime.MinValue)
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
                                            Value = rptParam.ParameterRawValue<DateTime>(DateTime.MinValue)
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

                                    tmpQuery = "Exec LG_RptStock @date1,@date2,@session";

                                    #endregion

                                    #region Report

                                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                                    reportFiles = Path.Combine(rtpPath, "LG_StockNasionalBatch.rpt");

                                    #endregion
                                }
                            }
                            isGenerated = true;
                        }
                        break;
                    case Constant.REPORT_INVENTORY_STOK_NASIONAL_TOTAL:
                        {
                            rptName = "Stock Nasional Total";

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
                                            Value = rptParam.ParameterRawValue<DateTime>(DateTime.MinValue)
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
                                            Value = rptParam.ParameterRawValue<DateTime>(DateTime.MinValue)
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

                                    tmpQuery = "Exec LG_RptStock @date1,@date2,@session";

                                    #endregion

                                    #region Report

                                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                                    reportFiles = Path.Combine(rtpPath, "LG_StockNasionalTotal.rpt");

                                    #endregion
                                }
                            }
                            isGenerated = true;
                        }
                        break;

                    #endregion

                    #region Kartu Barang Gudang

                    case Constant.REPORT_INVENTORY_STOK_KARTU_BARANG_GUDANG:
                        {
                            rptName = "Kartu Barang Total";

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

                                    switch (tipereport)
                                    {
                                        case "01":
                                            rptName = "Kartu Barang Batch";
                                            reportFiles = Path.Combine(rtpPath, "LG_KartuBarangGudangBatch.rpt");
                                            tmpQuery = "Exec LG_RptKartuBarangBatch @date1,@date2, @user, @gdg, @iteno, @divams, @nosup, @divpri";
                                            break;
                                        case "02":
                                            rptName = "Kartu Barang Total";
                                            reportFiles = Path.Combine(rtpPath, "LG_KartuBarangGudangTotal.rpt");
                                            tmpQuery = "Exec LG_RptKartuBarangTotal @date1,@date2, @user, @gdg, @iteno, @divams, @nosup, @divpri";
                                            break;
                                    }

                                    #endregion
                                }
                            }
                            isGenerated = true;
                        }
                        break;

                    #endregion

                    #region Kartu Barang Nasional

                    case Constant.REPORT_INVENTORY_STOK_KARTU_BARANG_NASIONAL_BATCH:
                        {
                            rptName = "Kartu Barang Nasional Batch";

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
                                            Value = rptParam.ParameterRawValue<DateTime>(DateTime.MinValue)
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
                                            Value = rptParam.ParameterRawValue<DateTime>(DateTime.MinValue)
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

                                    tmpQuery = "Exec LG_RptKartuBarang_Slim @date1,@date2,@session";

                                    #endregion

                                    #region Report

                                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                                    reportFiles = Path.Combine(rtpPath, "LG_KartuBarangNasionalBatch.rpt");

                                    #endregion
                                }
                            }
                            isGenerated = true;
                        }
                        break;
                    case Constant.REPORT_INVENTORY_STOK_KARTU_BARANG_NASIONAL_TOTAL:
                        {
                            rptName = "Kartu Barang Nasional Total";

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
                                            Value = rptParam.ParameterRawValue<DateTime>(DateTime.MinValue)
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
                                            Value = rptParam.ParameterRawValue<DateTime>(DateTime.MinValue)
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

                                    tmpQuery = "Exec LG_RptKartuBarang_Slim @date1,@date2,@session";

                                    #endregion

                                    #region Report

                                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                                    reportFiles = Path.Combine(rtpPath, "LG_KartuBarangNasionalTotal.rpt");

                                    #endregion
                                }
                            }
                            isGenerated = true;
                        }
                        break;

                    #endregion

                    #region Stock Opname

                    case Constant.REPORT_INVENTORY_STOK_OPNAME:
                        {
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
                                            Value = rptParam.ParameterRawValue<DateTime>(DateTime.MinValue)
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
                                            Value = rptParam.ParameterRawValue<DateTime>(DateTime.MinValue)
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

                                    //tmpQuery = "Exec LG_RptStock_NewSCMS @date1,@date2,@session,@gdg,@iteno,@nosup";

                                    #endregion

                                    #region Report

                                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                                    switch (tipereport)
                                    {
                                        case "batchbad":
                                            rptName = "Stock Opname Batch - Bad";
                                            reportFiles = Path.Combine(rtpPath, "LG_OpnameBatchBad.rpt");
                                            tmpQuery = "Exec LG_RptKartuBarangBatch @date1,@date2,@session,@gdg,@iteno,'000',@nosup,'000'";
                                            break;
                                        case "batchgood":
                                            rptName = "Stock Opname Batch - Good";
                                            reportFiles = Path.Combine(rtpPath, "LG_OpnameBatchGood.rpt");
                                            tmpQuery = "Exec LG_RptKartuBarangBatch @date1,@date2,@session,@gdg,@iteno,'000',@nosup,'000'";
                                            break;
                                        case "totalbad":
                                            rptName = "Stock Opname Total - Bad";
                                            reportFiles = Path.Combine(rtpPath, "LG_OpnameTotalBad.rpt");
                                            tmpQuery = "Exec LG_RptKartuBarangTotal @date1,@date2,@session,@gdg,@iteno,'000',@nosup,'000'";
                                            break;
                                        case "totalgood":
                                            rptName = "Stock Opname Total - Good";
                                            reportFiles = Path.Combine(rtpPath, "LG_OpnameTotalGood.rpt");
                                            tmpQuery = "Exec LG_RptKartuBarangTotal @date1,@date2,@session,@gdg,@iteno,'000',@nosup,'000'";
                                            break;
                                        case "integrityDC":

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
                                                    Size = 2,
                                                    Value = rptParam.ParameterValue
                                                });
                                            }

                                            if (rptParam.ParameterValue == "01")
                                            {
                                                rptName = "Stock Integrity DC Total";
                                                reportFiles = Path.Combine(rtpPath, "LG_IntegrityDC.rpt");
                                            }
                                            else
                                            {
                                                rptName = "Stock Integrity DC Detail";
                                                reportFiles = Path.Combine(rtpPath, "LG_IntegrityDCDetail.rpt");
                                            }
                                            tmpQuery = "Exec LG_RptKartuBarangIntegrity @date1,@date2,@session,@gdg,@iteno,'000',@nosup,'000',@tipe";
                                            break;
                                    }

                                    #endregion
                                }
                            }
                            isGenerated = true;
                        }
                        break;

                    #endregion

                    #region Stock Aktual

                    case Constant.REPORT_INVENTORY_STOK_AKTUAL:
                        {
                            if (rpt.ReportParameter != null)
                            {
                                if (rpt.ReportParameter.Length > 0)
                                {
                                    #region Sql
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

                                        if (x.ParameterName.Equals("type", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                                        if (x.ParameterName.Equals("itemCode", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                                        if (x.ParameterName.Equals("kddivams", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                                        if (x.ParameterName.Equals("kddivpri", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                                    //switch (rptParam.ParameterValue)
                                    switch (tipereport)
                                    {
                                        case "1":
                                            rptName = "Stock Aktual All";
                                            reportFiles = Path.Combine(rtpPath, "LG_StockAktual_New_Report.rpt");
                                            tmpQuery = "exec LG_Stock_Aktual_Perbatch_New_2 @gdg,@nosup,@type,@itemCode,@kddivams,@kddivpri";
                                            break;
                                        //case "2":
                                        //    rptName = "Stock Aktual PerBatch - Good";
                                        //    reportFiles = Path.Combine(rtpPath, "LG_StockAktual.rpt");
                                        //    tmpQuery = "exec LG_Stock_Aktual_Perbatch_New_2 @gdg,@nosup,@type";
                                        //    break;
                                        //case "3":
                                        //    rptName = "Stock Aktual PerBatch - Bad";
                                        //    reportFiles = Path.Combine(rtpPath, "LG_StockAktual.rpt");
                                        //    tmpQuery = "exec LG_Stock_Aktual_Perbatch_New_2 @gdg,@nosup,@type";
                                        //    break;

                                    }

                                    //reportFiles = Path.Combine(rtpPath, "LG_StockAktual_New.rpt");
                                    //tmpQuery = "exec LG_Stock_Aktual_Perbatch_New @gdg,@nosup,@type";


                                    #endregion
                                }
                            }
                            isGenerated = true;
                        }
                        break;

                    #endregion

                        // hafizh
                    #region Monitoring ED

                    case Constant.REPORT_INVENTORY_MONITORINGED:
                        {
                            if (rpt.ReportParameter != null)
                            {
                                if (rpt.ReportParameter.Length > 0)
                                {
                                    #region Sql
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
                                            Size = 5,
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
                                            Size = 2,
                                            Value = rptParam.ParameterValue
                                        });
                                    }
                                    string type = rptParam.ParameterValue;

                                    #endregion

                                    #region Report

                                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                                    switch (rptParam.ParameterValue)
                                    {

                                        case "1":
                                            rptName = "Monitoring ED Good & Bad Header";
                                            reportFiles = Path.Combine(rtpPath, "LG_MonitoringED_All.rpt");
                                            break;
                                        case "2":
                                            rptName = "Monitoring ED Good Detail";
                                            reportFiles = Path.Combine(rtpPath, "LG_MonitoringED_Good_Detail.rpt");
                                            break;
                                        case "3":
                                            rptName = "Monitoring ED Bad Detail";
                                            reportFiles = Path.Combine(rtpPath, "LG_MonitoringED_Bad_Detail.rpt");
                                            break;

                                    }


                                    tmpQuery = "exec LG_MonitorigED_2 @gdg,@nosup,@type ";


                                    #endregion
                                }
                            }
                            isGenerated = true;
                        }
                        break;

                    #endregion

                    #region Monitoring Expired 2

                    case Constant.REPORT_INVENTORY_REPORT_MONITOTINGEDCTRL:
                        {
                            if (rpt.ReportParameter != null)
                            {
                                if (rpt.ReportParameter.Length > 0)
                                {
                                    #region Sql
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
                                            Size = 5,
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
                                            Size = 2,
                                            Value = rptParam.ParameterValue
                                        });
                                    }
                                    string type = rptParam.ParameterValue;

                                    #endregion

                                    #region Report

                                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                                    switch (rptParam.ParameterValue)
                                    {

                                        case "1":
                                            rptName = "Monitoring Expired Good & Bad";
                                            reportFiles = Path.Combine(rtpPath, "LG_MonitoringExpired_All.rpt");
                                            break;
                                        case "2":
                                            rptName = "Monitoring Expired Good Detail";
                                            reportFiles = Path.Combine(rtpPath, "LG_MonitoringExpired_Good.rpt");
                                            break;
                                        case "3":
                                            rptName = "Monitoring Expired Bad Detail";
                                            reportFiles = Path.Combine(rtpPath, "LG_MonitoringExpired_Bad.rpt");
                                            break;

                                    }


                                    tmpQuery = "exec LG_MonitorigExpired @gdg,@nosup,@type ";


                                    #endregion
                                }
                            }
                            isGenerated = true;
                        }
                        break;

                    #endregion

                    #region REPORT_INVENTORY_INDEX_STOCK

                    case Constant.REPORT_INVENTORY_INDEX_STOCK:
                        {
                            rptName = "Index Stock";

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
                                            System.Data.SqlDbType.Int)
                                        {
                                            Value = rptParam.ParameterRawValue<int>(int.MinValue)
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
                                            System.Data.SqlDbType.Int)
                                        {
                                            Value = rptParam.ParameterRawValue<int>(int.MinValue)
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

                                    tmpQuery = "Exec LG_CalcIndexStock @tahun,@bulan,@type,@session";

                                    #endregion

                                    #region Report

                                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                                    reportFiles = Path.Combine(rtpPath, "LG_IndexStock.rpt");

                                    #endregion

                                    isGenerated = true;
                                }
                            }
                        }
                        break;

                    #endregion

                    #region Umur Stock

                    case Constant.REPORT_INVENTORY_UMUR_STOCK_DETIL:
                        {
                            rptName = "Umur Stock Detil";

                            if (rpt.ReportParameter != null)
                            {
                                if (rpt.ReportParameter.Length > 0)
                                {
                                    #region Sql

                                    rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                                    {
                                        bool isOk = false;

                                        if (x.ParameterName.Equals("date", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                                    tmpQuery = "Exec LG_UmurStock @date,@type,@session";

                                    #endregion

                                    #region Report

                                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                                    reportFiles = Path.Combine(rtpPath, "LG_UmurStock1.rpt");

                                    #endregion

                                    isGenerated = true;
                                }
                            }
                        }
                        break;

                    case Constant.REPORT_INVENTORY_UMUR_STOCK_ITEM:
                        {
                            rptName = "Umur Stock Item";

                            if (rpt.ReportParameter != null)
                            {
                                if (rpt.ReportParameter.Length > 0)
                                {
                                    #region Sql

                                    rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                                    {
                                        bool isOk = false;

                                        if (x.ParameterName.Equals("date", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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
                                            Size = 2,
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

                                    tmpQuery = "Exec LG_UmurStock @date,@type,@session";

                                    #endregion

                                    #region Report

                                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                                    reportFiles = Path.Combine(rtpPath, "LG_UmurStock2.rpt");

                                    #endregion

                                    isGenerated = true;
                                }
                            }
                        }
                        break;

                    case Constant.REPORT_INVENTORY_UMUR_STOCK_DIV_PRINSIPAL:
                        {
                            rptName = "Umur Stock Div Prinsipal";

                            if (rpt.ReportParameter != null)
                            {
                                if (rpt.ReportParameter.Length > 0)
                                {
                                    #region Sql

                                    rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                                    {
                                        bool isOk = false;

                                        if (x.ParameterName.Equals("date", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                                    tmpQuery = "Exec LG_UmurStock @date,@type,@session";

                                    #endregion

                                    #region Report

                                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                                    reportFiles = Path.Combine(rtpPath, "LG_UmurStock3.rpt");

                                    #endregion

                                    isGenerated = true;
                                }
                            }
                        }
                        break;

                    case Constant.REPORT_INVENTORY_UMUR_STOCK_SUPPLIER:
                        {
                            rptName = "Umur Stock Prinsipal";

                            if (rpt.ReportParameter != null)
                            {
                                if (rpt.ReportParameter.Length > 0)
                                {
                                    #region Sql

                                    rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                                    {
                                        bool isOk = false;

                                        if (x.ParameterName.Equals("date", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                                    tmpQuery = "Exec LG_UmurStock @date,@type,@session";

                                    #endregion

                                    #region Report

                                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                                    reportFiles = Path.Combine(rtpPath, "LG_UmurStock4.rpt");

                                    #endregion

                                    isGenerated = true;
                                }
                            }
                        }
                        break;

                    #endregion

                    #region Mutasi Inv

                    case Constant.REPORT_INVENTORY_MUTASI_INV:
                        {
                            rptName = "SMutasi Inventory";

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
                                            Value = rptParam.ParameterRawValue<DateTime>(DateTime.MinValue)
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
                                            Value = rptParam.ParameterRawValue<DateTime>(DateTime.MinValue)
                                        });
                                    }

                                    //Indra 20190618FM
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

                                    //Indra 20190618FM
                                    //tmpQuery = "Exec LG_RptStock @date1,@date2,@session";
                                    tmpQuery = "Exec SP_MUTASIINV @gudang,@date1,@date2,@tipe,@session";

                                    #endregion

                                    #region Report

                                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                                    //Indra 20190618FM
                                    //reportFiles = Path.Combine(rtpPath, "LG_rptMutasiInv.rpt");
                                    reportFiles = Path.Combine(rtpPath, "LG_rptMutasiInvNew.rpt");

                                    #endregion
                                }
                            }

                            isGenerated = true;
                        }
                        break;

                    #endregion

                    //20170515 Indra D.
                    #region Mutasi Inv Detail 

                    case Constant.REPORT_INVENTORY_MUTASI_INV_DETAIL:
                        {
                            rptName = "SMutasi Inventory Detail";

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
                                            Value = rptParam.ParameterRawValue<DateTime>(DateTime.MinValue)
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
                                            Value = rptParam.ParameterRawValue<DateTime>(DateTime.MinValue)
                                        });
                                    }

                                    //Indra 20190618FM
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

                                    //Indra 20190618FM
                                    //tmpQuery = "Exec LG_RptStock @date1,@date2,@session";
                                    tmpQuery = "Exec SP_MUTASIINV @gudang,@date1,@date2,@tipe,@session";

                                    #endregion

                                    #region Report

                                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                                    //Indra 20190618FM
                                    //reportFiles = Path.Combine(rtpPath, "LG_rptMutasiInvDetail.rpt");
                                    reportFiles = Path.Combine(rtpPath, "LG_rptMutasiInvDetailNew.rpt");

                                    #endregion
                                }
                            }

                            isGenerated = true;
                        }
                        break;

                    #endregion

                    #region Expire Batch
                    case Constant.REPORT_INVENTORY_EXPIRE_BATCH:
                        {
                            rptName = "Expire Batch";

                            if (rpt.ReportParameter != null)
                            {
                                if (rpt.ReportParameter.Length > 0)
                                {
                                    #region Sql

                                    rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                                    {
                                        bool isOk = false;

                                        if (x.ParameterName.Equals("expireid", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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

                                        if (x.ParameterName.Equals("session", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                                        {
                                            isOk = true;
                                        }

                                        return isOk;
                                    });
                                    if (rptParam != null)
                                    {
                                        lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                                            System.Data.SqlDbType.Char)
                                        {
                                            Size = 15,
                                            Value = rptParam.ParameterValue
                                        });
                                    }

                                    #endregion

                                    #region Report

                                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);
                                    reportFiles = Path.Combine(rtpPath, "LG_ExpireBatch.rpt");
                                    tmpQuery = "Exec LG_RptExpireBatch @expireid, @iteno, @nosup, @session";

                                    #endregion
                                }
                            }

                            isGenerated = true;
                        }
                        break;

                    #endregion

                    #region Current Stock ED
                    case Constant.REPORT_INVENTORY_CURRENTSTOCK_ED:
                        {
                            rptName = "Current Stock ED";

                            if (rpt.ReportParameter != null)
                            {
                                if (rpt.ReportParameter.Length > 0)
                                {
                                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                                    reportFiles = Path.Combine(rtpPath, "LG_CurrentStockED.rpt");
                                }
                            }

                            isGenerated = true;
                        }
                        break;

                    #endregion

                    #region Master Item
                    case Constant.REPORT_MASTER_ITEM:
                        {
                            rptName = "Master Item";

                            rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                            reportFiles = Path.Combine(rtpPath, "LG_RptMasterItem.rpt");

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
