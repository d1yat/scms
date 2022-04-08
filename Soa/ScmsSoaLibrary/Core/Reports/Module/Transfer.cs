using System;
using System.Collections.Generic;
using System.Text;
using ScmsSoaLibraryInterface.Components;
using ScmsSoaLibrary.Commons;
using System.Data.SqlClient;
using System.IO;
using ScmsModel;
using System.Linq;
using ScmsModel.Core;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaLibrary.Core.Reports.Module
{
  class Transfer
  {
    public static Functionals.ReportingGeneratorResult GenerateTransferGudang(Config cfg, ReportParser rpt, bool isAsync)
    {
      if ((rpt == null) || (cfg == null))
      {
        return new Functionals.ReportingGeneratorResult();
      }

      Functionals.ReportingGeneratorResult result = new Functionals.ReportingGeneratorResult();

      List<SqlParameter> lstParams = new List<SqlParameter>();
      ReportParameter rptParam = null;
      string tmpQuery = null;
      bool isGenerated = false;
      string reportFiles = null;
      string rptRecordSel = null;
      string rptName = null;
      string rtpPath = string.Concat(cfg.PathReport, @"Transfer\");

      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {
          #region Transfer Gudang

          case Constant.REPORT_TRANSAKSI_TransferGudang:
            {
              rptName = "Transfer Gudang";

              if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
              {
                #region Report

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                if (string.IsNullOrEmpty(rptRecordSel))
                {
                  rptRecordSel = "((if(IsNull({LG_SJH.l_delete})) then false else {LG_SJH.l_delete}) = false)";
                }
                else
                {
                  rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_SJH.l_delete})) then false else {LG_SJH.l_delete}) = false)");
                }

                reportFiles = Path.Combine(rtpPath, "LG_SJ.rpt");

                #endregion
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

            #region Update Database

            ORMDataContext db = new ORMDataContext(cfg.ConnectionString);

            try
            {
              switch (rpt.ReportingID)
              {
                #region Packing List

                case Constant.REPORT_TRANSAKSI_TransferGudang:
                  {
                    var qry = (from q in db.LG_SJHs
                               where (q.l_delete.HasValue ? q.l_delete.Value : false) == false
                               select q).AsQueryable();

                    //var qryTmp = (from q in rpt.ReportParameter
                    //              where q.IsSqlParameter == true
                    //              select q);

                    char gdg = char.MinValue;
                    char Gudang = char.MinValue;

                    for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
                    {
                      rptParam = rpt.ReportParameter[nLoop];

                      if (rptParam.IsSqlParameter)
                      {
                        if (rptParam.ParameterName.Equals("Gdg", StringComparison.OrdinalIgnoreCase) && (!string.IsNullOrEmpty(rptParam.ParameterValue)))
                        {
                          gdg = rptParam.ParameterValue[0];

                          var Xqry = qry.Where(x => x.c_gdg == gdg);

                          qry = Xqry;
                        }
                        else if (rptParam.ParameterName.Equals("Gudang", StringComparison.OrdinalIgnoreCase) && (!string.IsNullOrEmpty(rptParam.ParameterValue)))
                        {
                          Gudang = rptParam.ParameterValue[0];

                          var Xqry = qry.Where(x => x.c_gdg2 == Gudang);

                          qry = Xqry;
                        }
                        else if (rptParam.ParameterName.Equals("SJID", StringComparison.OrdinalIgnoreCase) && (!string.IsNullOrEmpty(rptParam.ParameterValue)))
                        {
                          if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                          {
                            var Xqry = qry.Where(x => x.c_sjno == rptParam.ParameterValue);

                            qry = Xqry;
                          }
                          else
                          {
                            //var Xqry = qry.Between(x => x.c_sjno, rptParam.ParameterValue, rptParam.BetweenValue).AsQueryable();

                            ////var Xqry = (from sq in qry
                            ////            where ((sq.c_plno.CompareTo(rptParam.ParameterValue) >= 0) && (sq.c_plno.CompareTo(rptParam.BetweenValue) <= 0))
                            ////            select sq).AsQueryable();

                            //qry = Xqry.AsQueryable();
                          }
                        }
                      }
                    }

                    List<LG_SJH> lst = qry.ToList();

                    LG_SJH sjh = null;
                    DateTime date = DateTime.Now;

                    for (int nLoop = 0; nLoop < lst.Count; nLoop++)
                    {
                      sjh = lst[nLoop];

                      sjh.l_print = true;
                      sjh.c_update = rpt.User;
                      sjh.d_update = date;
                    }

                    db.SubmitChanges();

                    lst.Clear();

                    break;
                  }

                #endregion
              }
            }
            catch (Exception ex)
            {
              tmp = string.Format("ScmsSoaLibrary.Core.Reports.Module.Transfer:GenerateTransferGudang - {0}", ex.Message);

              Logger.WriteLine(tmp);
              Logger.WriteLine(ex.StackTrace);
            }

            db.Dispose();

            #endregion
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

      lstParams.Clear();

      return result;
    }
  }
}
