using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsModel;
using ScmsSoaLibrary.Parser;
//using ScmsSoaLibrary.Core.Crypto;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaLibrary.Bussiness
{
  class Reporting
  {
    public string ReportingManagement(Parser.Parser.StructureXmlHeaderParser xmlParser, Parser.Parser.StructureDataNamingHeader dataParser)
    {
      string result = null;

      MyAssembly myAsm = new MyAssembly();

      SCMS_Report tableReport = null,
        tableReportEdit;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      string sNipEdit = null,
        sNipCreated = null;
      int Idx = 0;
      string tmp = null;
      byte[] bFile = null;

      bool isShared = false;

      Temp_Barcode TmpBarCode = null;

      List<Temp_Barcode> lstTmpBarcode = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

      try
      {
        tableReport = myAsm.Populate<SCMS_Report>(xmlParser, dataParser);

        switch (dataParser.Method)
        {
          #region Add

          case ScmsSoaLibrary.Parser.Parser.InterpreterMethod.IsAdd:
            {
              tmp = (tableReport.v_report ?? string.Empty);

              if (string.IsNullOrEmpty(tmp))
              {
                result = "Nama report tidak boleh kosong.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                break;
              }

              sNipEdit = (tableReport.c_entry ?? string.Empty);

              if (string.IsNullOrEmpty(sNipEdit))
              {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                break;
              }

              if (tableReport.v_reportusername.Contains(','))
              {

                string[] arrRpt = tableReport.v_reportusername.Split(',');

                if (arrRpt.Length > 0)
                {
                  for (int i = 0; arrRpt.Length > i; i++)
                  {
                    string sRpt = arrRpt[i];

                    if (!string.IsNullOrEmpty(sRpt))
                    {

                      tableReport.v_reportusername = sRpt;
                      lstTmpBarcode = (from q in db.Temp_Barcodes
                                       where q.c_TransNo == (string.IsNullOrEmpty(tableReport.v_reportusername) ? tableReport.v_report : tableReport.v_reportusername)
                                       select q).ToList();


                      Bytescout.BarCode.Barcode bc = new Bytescout.BarCode.Barcode(Bytescout.BarCode.SymbologyType.Code128);
                      
                      bc.DrawCaption = false;
                      bc.NarrowBarWidth = 1;
                      bc.BarHeight = 100;


                      // Set the value to encode
                      bc.Value = (string.IsNullOrEmpty(tableReport.v_reportusername) ? tableReport.v_report : tableReport.v_reportusername);
                      bFile = bc.GetImageBytes();
                      // Generate the barcode image and store it into the Barcode Column
                      bFile = bc.GetImageBytes();

                      

                      TmpBarCode = new Temp_Barcode()
                      {
                        c_TransNo = (string.IsNullOrEmpty(tableReport.v_reportusername) ? tableReport.v_report : tableReport.v_reportusername),
                        v_image = bFile,
                      };

                      db.Temp_Barcodes.DeleteAllOnSubmit(lstTmpBarcode.ToArray());


                      tableReport.d_entry = DateTime.Now;

                      db.SCMS_Reports.InsertOnSubmit(tableReport);

                      db.Temp_Barcodes.InsertOnSubmit(TmpBarCode);
                    }
                  }
                }
              }
              else
              {
                lstTmpBarcode = (from q in db.Temp_Barcodes
                                 where q.c_TransNo == (string.IsNullOrEmpty(tableReport.v_reportusername) ? tableReport.v_report : tableReport.v_reportusername)
                                 select q).ToList();


                Bytescout.BarCode.Barcode bc = new Bytescout.BarCode.Barcode(Bytescout.BarCode.SymbologyType.Code128);
                bc.DrawCaption = false;
                bc.NarrowBarWidth = 1;
                bc.BarHeight = 100;

                

                // Set the value to encode
                bc.Value = (string.IsNullOrEmpty(tableReport.v_reportusername) ? tableReport.v_report : tableReport.v_reportusername);

                //if (!string.IsNullOrEmpty(bc.Value))
                //{
                //    if (bc.Value.Substring(0, 2) == "ST")
                //    {
                //        bc.DrawCaption = true;
                //    }
                //}
                  
                bFile = bc.GetImageBytes();
                // Generate the barcode image and store it into the Barcode Column
                bFile = bc.GetImageBytes();

                TmpBarCode = new Temp_Barcode()
                {
                  c_TransNo = (string.IsNullOrEmpty(tableReport.v_reportusername) ? tableReport.v_report : tableReport.v_reportusername),
                  v_image = bFile,
                };

                db.Temp_Barcodes.DeleteAllOnSubmit(lstTmpBarcode.ToArray());


                tableReport.d_entry = DateTime.Now;

                db.SCMS_Reports.InsertOnSubmit(tableReport);

                db.Temp_Barcodes.InsertOnSubmit(TmpBarCode);
              }

              db.SubmitChanges();

              rpe = ResponseParser.ResponseParserEnum.IsSuccess;
            }
            break;

          #endregion

          #region Update

          case ScmsSoaLibrary.Parser.Parser.InterpreterMethod.IsUpdate:
            {
              Idx = tableReport.Idx;

              sNipEdit = (tableReport.c_entry ?? string.Empty);

              if (string.IsNullOrEmpty(sNipEdit))
              {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                break;
              }

              tableReportEdit = db.SCMS_Reports.Where(x => x.Idx == Idx).Take(1).SingleOrDefault();
              if (tableReportEdit == null)
              {
                result = "Data tidak di temukan.";

                rpe = ResponseParser.ResponseParserEnum.IsError;

                break;
              }

              if (tableReportEdit.l_download.HasValue)
              {
                tableReportEdit.l_download += 1;
              }
              else
              {
                tableReportEdit.l_download = 1;
              }
              //tableReportEdit.c_update = sNip;
              //tableReportEdit.d_update = DateTime.Now;

              db.SubmitChanges();

              rpe = ResponseParser.ResponseParserEnum.IsSuccess;
            }
            break;

          #endregion

          #region Delete

          case ScmsSoaLibrary.Parser.Parser.InterpreterMethod.IsDelete:
            {
              Idx = tableReport.Idx;
              
              sNipEdit = (tableReport.c_entry ?? string.Empty);

              if (string.IsNullOrEmpty(sNipEdit))
              {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                break;
              }

              tableReportEdit = db.SCMS_Reports.Where(x => x.Idx == Idx).Take(1).SingleOrDefault();
              if (tableReportEdit == null)
              {
                result = "Data tidak di temukan.";

                rpe = ResponseParser.ResponseParserEnum.IsError;

                break;
              }
              sNipCreated = (tableReportEdit.c_entry ?? string.Empty);

              isShared = (tableReportEdit.l_share.HasValue ? tableReportEdit.l_share.Value : false);

              if(isShared && (!sNipEdit.Equals(sNipCreated, StringComparison.OrdinalIgnoreCase)))
              {
                result = "Anda tidak dapat menghapus laporan ini.";

                rpe = ResponseParser.ResponseParserEnum.IsError;

                break;
              }

              db.SCMS_Reports.DeleteOnSubmit(tableReportEdit);

              db.SubmitChanges();

              rpe = ResponseParser.ResponseParserEnum.IsSuccess;
            }
            break;

          #endregion

          #region Add

          case ScmsSoaLibrary.Parser.Parser.InterpreterMethod.IsBarcode:
            {

            }
            break;

          #endregion

        }
      }
      catch (Exception ex)
      {
        rpe = ResponseParser.ResponseParserEnum.IsError;

        result = string.Format("ScmsSoaLibrary.Bussiness.Reporting:ReportingManagement - {0}", ex.Message);

        Logger.WriteLine(result);
        Logger.WriteLine(ex.StackTrace);
      }

      result = Parser.ResponseParser.ResponseGenerator(rpe, null, result);

      db.Dispose();

      return result;
    }
  }
}
