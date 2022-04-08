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
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Drawing;
using System.Data;
using System.Data.OleDb;
using System.Data;
using System.Data.OleDb;
using System.Data;
using System.Data.Odbc;
using ClosedXML.Excel;

namespace ScmsSoaLibrary.Core.Reports.Module
{
  class Transaksi
  {
      internal class LG_DOD1_SUM_BYBATCH
    {
        public string c_dono { get; set; }
        public string c_iteno { get; set; }
        public string c_batch { get; set; }
        public decimal n_qty { get; set; }
    }

    private static System.Drawing.Font printFont;

    private static StreamReader streamToPrint;

    private static string DbfColumnParser(System.Data.DataColumn column, string caption)
    {
        string rets = null;

        if (column.DataType.Equals(typeof(float)) ||
          column.DataType.Equals(typeof(double)) ||
          column.DataType.Equals(typeof(decimal)))
        {
            int i = 18;
            switch (caption.ToLower())
            {
                case "Qty_Retur":
                    i = 10;
                    break;
            }
            rets = string.Format("[{0}] numeric({1},2) {2}",
              column.ColumnName, i,
              (column.AllowDBNull ? "NULL" : "NOT NULL"));
        }
        else if (column.DataType.Equals(typeof(ushort)) ||
          column.DataType.Equals(typeof(short)) ||
          column.DataType.Equals(typeof(uint)) ||
          column.DataType.Equals(typeof(int)) ||
          column.DataType.Equals(typeof(ulong)) ||
          column.DataType.Equals(typeof(long)))
        {
            rets = string.Format("[{0}] int {1}",
              column.ColumnName,
              (column.AllowDBNull ? "NULL" : "NOT NULL"));
        }
        else if (column.DataType.Equals(typeof(bool)))
        {
            rets = string.Format("[{0}] logical {1}",
              column.ColumnName,
              (column.AllowDBNull ? "NULL" : "NOT NULL"));
        }
        else if (column.DataType.Equals(typeof(DateTime)))
        {
            rets = string.Format("[{0}] date {1}",
              column.ColumnName,
              (column.AllowDBNull ? "NULL" : "NOT NULL"));
        }
        else
        {
            int i = 1;
            switch (caption.ToLower())
            {
                case "namaproduk":
                case "alasan":
                case "namaoutlet":
                    i = 50;
                    break;
                case "c_batch":
                case "c_itenopri":
                    i = 15;
                    break;
                case "product_id":
                case "kodeoutlet":
                    i = 4;
                    break;
                case "c_invno":
                    i = 20;
                    break;
                case "c_rs":
                    i = 10;
                    break;
                case "id_alasan":
                    i = 3;
                    break;
                case "kode_area":
                    i = 2;
                    break;
            }
            rets = string.Format("[{0}] char({1}) {2}",
              column.ColumnName, i,
              (column.AllowDBNull ? "NULL" : "NOT NULL"));
        }

        return rets;
    }

    private static bool ExportDBF(System.Data.DataSet dsExport, string folderPath, string sNama, bool isHeader, bool isText)
    {
        string tableName = sNama;
        bool returnStatus = false;

        try
        {
            string createStatement = "Create Table " + tableName + " ( ";
            string insertStatement = "Insert Into " + tableName + " Values ( ";
            string insertTemp = string.Empty;
            OleDbCommand cmd = new OleDbCommand();
            OleDbConnection conn = null;
            if (dsExport.Tables[0].Columns.Count <= 0) { throw new Exception(); }

            StringBuilder sb = new StringBuilder();
            int nLoop = 0,
              nLen = 0,
              nLenC = 0,
              nLoopC = 0;
            System.Data.DataColumn col = null;
            System.Data.DataRow row = null;
            string reslt = null,
                dateStr = null;

            string sFile = folderPath + sNama + ".dbf";

            bool bData = false;
            DateTime d_corda;

            if (!isText)
            {
                #region Create Table

                conn = new System.Data.OleDb.OleDbConnection(string.Format("Provider=vfpoledb;Data Source='{0}';Collating Sequence=general;", folderPath));
                conn.Open();

                cmd = conn.CreateCommand();

                DataTable table = dsExport.Tables[0];

                sb.AppendFormat("CREATE TABLE {0} (", tableName);

                for (nLoop = 0, nLen = table.Columns.Count; nLoop < nLen; nLoop++)
                {
                    if ((nLoop + 1) >= nLen)
                    {
                        sb.AppendFormat(" {0}", DbfColumnParser(table.Columns[nLoop], table.Columns[nLoop].Caption));
                    }
                    else
                    {
                        sb.AppendFormat(" {0},", DbfColumnParser(table.Columns[nLoop], table.Columns[nLoop].Caption));
                    }
                }
                sb.Append(" )");

                cmd.CommandText = sb.ToString();
                cmd.ExecuteNonQuery();

                cmd.Dispose();

                sb.Remove(0, sb.Length);

                #endregion

                #region Populate Data

                cmd = conn.CreateCommand();

                nLenC = table.Columns.Count;

                for (nLoopC = 0; nLoopC < nLenC; nLoopC++)
                {
                    col = table.Columns[nLoopC];

                    reslt = string.Concat(reslt, ",", col.ColumnName);
                }

                reslt = (reslt.StartsWith(",", StringComparison.OrdinalIgnoreCase) ? reslt.Remove(0, 1) : reslt);

                for (nLoop = 0, nLen = table.Rows.Count; nLoop < nLen; nLoop++)
                {
                    row = table.Rows[nLoop];

                    sb.AppendFormat("Insert Into {0} ({1}) Values (", tableName, reslt);

                    for (nLoopC = 0; nLoopC < nLenC; nLoopC++)
                    {
                        col = table.Columns[nLoopC];

                        if (col.DataType.Equals(typeof(float)) ||
                           col.DataType.Equals(typeof(double)) ||
                           col.DataType.Equals(typeof(decimal)))
                        {
                            sb.AppendFormat("{0} ,", decimal.Parse(row[col].ToString()));
                        }
                        else if (col.DataType.Equals(typeof(ushort)) ||
                           col.DataType.Equals(typeof(short)) ||
                           col.DataType.Equals(typeof(uint)) ||
                           col.DataType.Equals(typeof(int)) ||
                           col.DataType.Equals(typeof(ulong)) ||
                           col.DataType.Equals(typeof(long)))
                        {
                            sb.AppendFormat("{0} ,", int.Parse(row[col].ToString()));
                        }
                        else if (col.DataType.Equals(typeof(DateTime)))
                        {
                            d_corda = DateTime.Parse(row[col].ToString());
                            sb.AppendFormat("Date({0},{1},{2}) ,", d_corda.Year, d_corda.Month, d_corda.Day);
                        }
                        else if (col.DataType.Equals(typeof(bool)))
                        {
                            bData = bool.Parse(row[col].ToString());
                            sb.AppendFormat("{0} ,", (bData ? ".t." : ".f."));
                            //sb.AppendFormat("NULL ,", (bData ? 1 : 0));
                        }
                        else
                        {
                            sb.AppendFormat("'{0}' ,", row[col]);
                        }
                    }

                    sb.Remove(sb.Length - 1, 1);

                    sb.AppendLine(" ) ");

                    cmd.CommandText = sb.ToString();

                    cmd.ExecuteNonQuery();

                    sb.Remove(0, sb.Length);
                }


                #endregion
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            else
            {
                DataTable dt = dsExport.Tables[0];
                int[] maxLengths = new int[dt.Columns.Count];

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    maxLengths[i] = dt.Columns[i].ColumnName.Length;

                    foreach (DataRow rows in dt.Rows)
                    {
                        if (!rows.IsNull(i))
                        {
                            int length = rows[i].ToString().Length;

                            if (length > maxLengths[i])
                            {
                                maxLengths[i] = length;
                            }
                        }
                    }
                }

                using (StreamWriter sw = new StreamWriter(folderPath + sNama, false))
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        sw.Write(dt.Columns[i].ColumnName.PadRight(maxLengths[i] + 2));
                    }

                    sw.WriteLine();

                    foreach (DataRow rows in dt.Rows)
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            if (!rows.IsNull(i))
                            {
                                sw.Write(rows[i].ToString().PadRight(maxLengths[i] + 2));
                            }
                            else
                            {
                                sw.Write(new string(' ', maxLengths[i] + 2));
                            }
                        }

                        sw.WriteLine();
                    }

                    sw.Close();
                }
            }
        }
        catch (Exception ex)
        {
            string ss = ex.Message;
        }
        return returnStatus = true;
    }

    internal class PLVia
    {
      public string c_plno { get; set; }
      public string c_via { get; set; }
    }

    public class RawPrinterHelper
    {
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
      public class DOCINFOA
      {
        [MarshalAs(UnmanagedType.LPStr)]
        public string pDocName;
        [MarshalAs(UnmanagedType.LPStr)]
        public string pOutputFile;
        [MarshalAs(UnmanagedType.LPStr)]
        public string pDataType;
      }
      [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
      public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

      [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
      public static extern bool ClosePrinter(IntPtr hPrinter);

      [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
      public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

      [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
      public static extern bool EndDocPrinter(IntPtr hPrinter);

      [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
      public static extern bool StartPagePrinter(IntPtr hPrinter);

      [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
      public static extern bool EndPagePrinter(IntPtr hPrinter);

      [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
      public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

      [DllImport("Winspool.drv")]
      public static extern bool SetDefaultPrinter(string printerName);

      public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
      {
        Int32 dwError = 0, dwWritten = 0;
        IntPtr hPrinter = new IntPtr(0);
        DOCINFOA di = new DOCINFOA();
        bool bSuccess = false;

        di.pDocName = "Data PL";
        di.pDataType = "RAW";

        if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
        {
          if (StartDocPrinter(hPrinter, 1, di))
          {
            if (StartPagePrinter(hPrinter))
            {
              bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);

              
              
              EndPagePrinter(hPrinter);
            }
            EndDocPrinter(hPrinter);
          }
          ClosePrinter(hPrinter);
        }
        if (bSuccess == false)
        {
          dwError = Marshal.GetLastWin32Error();
        }
        return bSuccess;
      }

      public static bool SendFileToPrinter(string szPrinterName, string szFileName)
      {

        FileStream fs = new FileStream(szFileName, FileMode.Open);

        BinaryReader br = new BinaryReader(fs);

        Byte[] bytes = new Byte[fs.Length];
        bool bSuccess = false;

        IntPtr pUnmanagedBytes = new IntPtr(0);
        int nLength;

        nLength = Convert.ToInt32(fs.Length);

        bytes = br.ReadBytes(nLength);

        //File.WriteAllBytes(@"D:\testpdf1.pdf", bytes);

        pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);

        Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);

        bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);

        Marshal.FreeCoTaskMem(pUnmanagedBytes);

        return bSuccess;
      }

      public static bool SendStringToPrinter(string szPrinterName, string szString)
      {
        IntPtr pBytes;
        Int32 dwCount;
        // How many characters are in the string?
        dwCount = szString.Length;
        // Assume that the printer is expecting ANSI text, and then convert
        // the string to ANSI text.
        pBytes = Marshal.StringToCoTaskMemAnsi(szString);
        // Send the converted ANSI string to the printer.
        SendBytesToPrinter(szPrinterName, pBytes, dwCount);
        Marshal.FreeCoTaskMem(pBytes);
        return true;
      }

    }

    public static Functionals.ReportingGeneratorResult GeneratePackingListAuto(Config cfg, ReportParser rpt, bool isAsync)
    {
      if ((rpt == null) || (cfg == null))
      {
        return new Functionals.ReportingGeneratorResult();
      }
      ORMDataContext db = new ORMDataContext(cfg.ConnectionString);
      Functionals.ReportingGeneratorResult result = new Functionals.ReportingGeneratorResult();

      List<SqlParameter> lstParams = new List<SqlParameter>();
      ReportParameter rptParam = null,
        rptParams = null;
      string tmpQuery = null;
      bool isGenerated = false;
      string reportFiles = null;
      string rptRecordSel = null;
      string[] sArrRecord = null;
      string rptName = null;
      string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");
      string sPathPrintLt1 = cfg.PathPrintStorageLat1;
      string sPathPrintLt2 = cfg.PathPrintStorageLat2;
      string sPathPrintLt3 = cfg.PathPrintStorageLat3;
      string sPathPrintLt4 = cfg.PathPrintStorageLat4;
      string i = "'";
      string arrPL = null;

      for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
      {
        rptParams = rpt.ReportParameter[nLoop];

        if (rptParams.ParameterName == "LG_PLH.c_plno")
        {
          sArrRecord = rptParams.ParameterValue.Split(',');
        }
      }

      foreach (string sId in sArrRecord)
      {
        
      }

      List<PLVia> lstPLVia = new List<PLVia>(),
        lstPLViaCopy = new List<PLVia>(),
        lstPLVias = new List<PLVia>();

      PLVia viaPl = new PLVia(),
        plSat = new PLVia();

      lstPLVia = (from q in db.LG_PLHs 
                  where sArrRecord.Contains(q.c_plno)
                    select new PLVia()
                  {
                    c_plno = q.c_plno,
                    c_via = q.c_type_lat,
                  }).ToList();

      lstPLViaCopy = lstPLVia.GroupBy(x => x.c_via).Select(t => new PLVia() { c_via = t.Key}).ToList();

      for (int nLoopI = 0; nLoopI < lstPLViaCopy.Count; nLoopI++)
      {
        viaPl = lstPLViaCopy[nLoopI];

        lstPLVias = new List<PLVia>();

        lstPLVias = lstPLVia.Where(x => x.c_via == viaPl.c_via).ToList();

        if (rpt != null)
        {
          switch (rpt.ReportingID)
          {
            #region Packing List

            case Constant.REPORT_TRANSAKSI_PACKINGLIST_AUTO:
              {
                rptName = "Packing List";

                if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
                {
                  #region Report

                  //rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                  rptRecordSel = string.Format("({{LG_PLH.c_plno}} = [");

                  for (int nLoopC = 0; nLoopC < lstPLVias.Count; nLoopC++)
                  {
                    plSat = lstPLVias[nLoopC];
                    arrPL += string.Concat(plSat.c_plno, ",");
                    rptRecordSel += string.Concat("'",plSat.c_plno, "',");
                  }

                  rptRecordSel = rptRecordSel.Remove((rptRecordSel.Length - 1), 1);

                  rptRecordSel += "])";

                  if (string.IsNullOrEmpty(rptRecordSel))
                  {
                    rptRecordSel = "((if(IsNull({LG_PLH.l_delete})) then false else {LG_PLH.l_delete}) = false)";
                  }
                  else
                  {
                    rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_PLH.l_delete})) then false else {LG_PLH.l_delete}) = false)");
                  }

                  reportFiles = Path.Combine(rtpPath, "LG_PL.rpt");

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
          #region Cetak

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
                reportStruct.sizeOutput, isAsync, arrPL, rpt.IsShared, out tmp))
              {
                  if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
                  {
                    string sPath = string.Concat(reportStruct.outputFolder, @"\", reportStruct.outputName, @".doc");
                      string sPathDoc = string.Concat(reportStruct.outputFolder, @"\", reportStruct.outputName);
                      File.Move(sPathDoc, Path.ChangeExtension(sPathDoc, ".doc"));

                      string ltPrinter = null;

                      if (plSat.c_via == "01")
                      {
                          System.Drawing.Printing.PrinterSettings ps = new System.Drawing.Printing.PrinterSettings();
                          foreach (String printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                          {
                              if (printer == sPathPrintLt1)
                              {
                                  ltPrinter = printer;
                                  RawPrinterHelper.SetDefaultPrinter(printer);
                                  break;
                              }
                          }
                      }
                      else if (plSat.c_via == "02")
                      {
                          System.Drawing.Printing.PrinterSettings ps = new System.Drawing.Printing.PrinterSettings();
                          foreach (String printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                          {
                              if (printer == sPathPrintLt2)
                              {
                                  ltPrinter = printer;
                                  RawPrinterHelper.SetDefaultPrinter(printer);
                                  break;
                              }
                          }
                      }
                      else if (plSat.c_via == "03")
                      {
                          System.Drawing.Printing.PrinterSettings ps = new System.Drawing.Printing.PrinterSettings();
                          foreach (String printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                          {
                              if (printer == sPathPrintLt3)
                              {
                                  ltPrinter = printer;
                                  RawPrinterHelper.SetDefaultPrinter(printer);
                                  break;
                              }
                          }
                      }
                      else if (plSat.c_via == "04")
                      {
                          System.Drawing.Printing.PrinterSettings ps = new System.Drawing.Printing.PrinterSettings();
                          foreach (String printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                          {
                              if (printer == sPathPrintLt4)
                              {
                                  ltPrinter = printer;
                                  RawPrinterHelper.SetDefaultPrinter(printer);
                                  break;
                              }
                          }
                      }

                      if (!string.IsNullOrEmpty(ltPrinter))
                      {
                          Process printProcess = new Process();


                          //string ExternalpdfParser = @"C:\Program Files\Adobe\Reader 9.0\Reader\AcroRd32.exe";
                          //string param = string.Format(@"-t ""{0}"" {1}", sPath, ltPrinter);
                          //using (System.Diagnostics.Process proc = System.Diagnostics.Process.Start(ExternalpdfParser, param))
                          //{
                          //  proc.WaitForExit((int)TimeSpan.FromSeconds(2).TotalMilliseconds);
                          //}

                          //printProcess.StartInfo.FileName = @"C:\Program Files\Adobe\Reader 9.0\Reader\AcroRd32.exe";
                          ////Also tried usecellexcecution=false;
                          ////Redirect=true; something like this
                          //string args = String.Format(@"/p /h {0}", sPath);
                          ////ProcessStartInfo psInfo = new ProcessStartInfo(@"C:\Program Files\Adobe\Reader 9.0\Reader\AcroRd32.exe");
                          ////psInfo.UseShellExecute = true;
                          //printProcess.StartInfo.UseShellExecute = true;
                          ////printProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                          //printProcess.StartInfo.Verb = "PrintTo";
                          //printProcess.StartInfo.Arguments = args;
                          //printProcess.StartInfo.CreateNoWindow = true;
                          //printProcess.Start();

                          printProcess.StartInfo.FileName = sPath;
                          //Also tried usecellexcecution=false;
                          //Redirect=true; something like this
                          printProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                          printProcess.StartInfo.Verb = "Print";
                          printProcess.StartInfo.Arguments = ltPrinter;
                        
                          printProcess.Start();
                      }

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

                      try
                      {
                          switch (rpt.ReportingID)
                          {
                              #region Packing List

                              case Constant.REPORT_TRANSAKSI_PACKINGLIST_AUTO:
                                  {
                                      //var qry = (from q in db.LG_PLHs
                                      //           where (q.l_delete.HasValue ? q.l_delete.Value : false) == false
                                      //            && ((q.l_print.HasValue ? q.l_print.Value : false) == false)
                                      //           select q).AsQueryable();

                                      ////var qryTmp = (from q in rpt.ReportParameter
                                      ////              where q.IsSqlParameter == true
                                      ////              select q);

                                      //for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
                                      //{
                                      //    rptParam = rpt.ReportParameter[nLoop];

                                      //    if (rptParam.IsLinqFilterParameter)
                                      //    {
                                      //      if (!rptParam.ParameterValue.Contains(','))
                                      //      {
                                      //        if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                                      //        {
                                      //          qry = qry.Where(rptParam.ParameterName, rptParam.ParameterValueObject);

                                      //          isGenerated = true;
                                      //        }
                                      //        else
                                      //        {
                                      //          qry = qry.Between(rptParam.ParameterName, rptParam.ParameterValueObject, rptParam.BetweenValueObject);

                                      //          isGenerated = true;
                                      //        }
                                      //      }
                                      //      else
                                      //      {
                                      //        string[] arrStr = rptParam.ParameterValue.Split(',');
                                      //        for (int nLoopC = 0; nLoopC < arrStr.Length; nLoopC++)
                                      //        {
                                      //          string strVal = arrStr[nLoopC];

                                      //          qry = qry.Between(rptParam.ParameterName, strVal, rptParam.BetweenValueObject);

                                      //          isGenerated = true;
                                      //        }
                                      //      }
                                      //    }
                                      //}

                                      List<LG_PLH> lst = null;

                                      SCMS_STH sth = null;
                                      SCMS_STD std = null;

                                      if (isGenerated)
                                      {

                                        rptParam = rpt.ReportParameter[1];

                                        string[] arrStr = rptParam.ParameterValue.Split(',');
                                        for (int nLoopC = 0; nLoopC < arrStr.Length; nLoopC++)
                                        {

                                          string strVal = arrStr[nLoopC];

                                          var qry = (from q in db.LG_PLHs
                                                     where (q.l_delete.HasValue ? q.l_delete.Value : false) == false
                                                      && ((q.l_print.HasValue ? q.l_print.Value : false) == false)
                                                      && q.c_plno == strVal
                                                     select q).AsQueryable();

                                          lst = qry.ToList();

                                          LG_PLH plh = null;
                                          DateTime date = DateTime.Now;

                                          for (int nLoop = 0; nLoop < lst.Count; nLoop++)
                                          {
                                            plh = lst[nLoop];

                                            plh.l_confirm = true;
                                            plh.l_print = true;

                                            plh.c_update = rpt.User;
                                            plh.d_update = date;

                                            plh.l_cek = true;
                                          }
                                        }

                                          db.SubmitChanges();

                                          lst.Clear();
                                      }

                                      break;
                                  }

                              #endregion
                          }
                      }
                      catch (Exception ex)
                      {
                          tmp = string.Format("ScmsSoaLibrary.Core.Reports.Module.Transaksi:GeneratePackingList - {0}", ex.Message);

                          Logger.WriteLine(tmp);
                          Logger.WriteLine(ex.StackTrace);
                      }

                      db.Dispose();

                      #endregion
                  }
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
      }

      lstParams.Clear();

      return result;
    }

    public static Functionals.ReportingGeneratorResult GeneratePackingList(Config cfg, ReportParser rpt, bool isAsync)
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
      string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");
      string sPathPrint = null;

      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {
          #region Packing List

          case Constant.REPORT_TRANSAKSI_PACKINGLIST:
            {
              rptName = "Packing List";

              if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
              {
                #region Report

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                if (string.IsNullOrEmpty(rptRecordSel))
                {
                  rptRecordSel = "((if(IsNull({LG_PLH.l_delete})) then false else {LG_PLH.l_delete}) = false)";
                }
                else
                {
                  rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_PLH.l_delete})) then false else {LG_PLH.l_delete}) = false)");
                }

                reportFiles = Path.Combine(rtpPath, "LG_PL.rpt");

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
          outputFolder = cfg.PathReportStorage
        };

        string tmp = null;


        if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
        {
          if (Functionals.ReportSaveParser(rptName, rpt.ReportingID, reportStruct.outputFolder, reportStruct.outputName, reportStruct.extReport, rpt.User,
            reportStruct.sizeOutput, isAsync, rpt.UserDefinedName, rpt.IsShared, out tmp))
          {
              if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
              {
                  System.Drawing.Printing.PrinterSettings ps = new System.Drawing.Printing.PrinterSettings();
                  foreach (String printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                  {
                      if (printer == sPathPrint)
                      {
                          string sPath = string.Concat(reportStruct.outputFolder, @"\", reportStruct.outputName, @".pdf");
                          string sPathDoc = string.Concat(reportStruct.outputFolder, @"\", reportStruct.outputName);
                          File.Move(sPathDoc, Path.ChangeExtension(sPathDoc, ".pdf"));


                          Process printProcess = new Process();

                          printProcess.StartInfo.FileName = sPath;
                          //Also tried usecellexcecution=false;
                          //Redirect=true; something like this
                          printProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                          printProcess.StartInfo.Verb = "PrintTo";
                          printProcess.StartInfo.Arguments = printer;
                          printProcess.StartInfo.CreateNoWindow = true;
                          printProcess.Start();

                      }
                  }

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

                  isGenerated = false;

                  ORMDataContext db = new ORMDataContext(cfg.ConnectionString);

                  try
                  {
                      switch (rpt.ReportingID)
                      {
                          #region Packing List

                          case Constant.REPORT_TRANSAKSI_PACKINGLIST:
                              {
                                  var qry = (from q in db.LG_PLHs
                                             where (q.l_delete.HasValue ? q.l_delete.Value : false) == false
                                              && ((q.l_print.HasValue ? q.l_print.Value : false) == false)
                                             select q).AsQueryable();

                                  //var qryTmp = (from q in rpt.ReportParameter
                                  //              where q.IsSqlParameter == true
                                  //              select q);

                                  for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
                                  {
                                      rptParam = rpt.ReportParameter[nLoop];

                                      if (rptParam.IsLinqFilterParameter)
                                      {
                                          if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                                          {
                                              qry = qry.Where(rptParam.ParameterName, rptParam.ParameterValueObject);

                                              isGenerated = true;
                                          }
                                          else
                                          {
                                              qry = qry.Between(rptParam.ParameterName, rptParam.ParameterValueObject, rptParam.BetweenValueObject);

                                              isGenerated = true;
                                          }
                                      }
                                  }

                                  List<LG_PLH> lst = null;

                                  if (isGenerated)
                                  {
                                      lst = qry.ToList();

                                      LG_PLH plh = null;
                                      DateTime date = DateTime.Now;

                                      for (int nLoop = 0; nLoop < lst.Count; nLoop++)
                                      {
                                          plh = lst[nLoop];

                                          plh.l_confirm = true;
                                          plh.l_print = true;

                                          plh.c_update = rpt.User;
                                          plh.d_update = date;
                                      }

                                      db.SubmitChanges();

                                      lst.Clear();
                                  }

                                  break;
                              }

                          #endregion
                      }
                  }
                  catch (Exception ex)
                  {
                      tmp = string.Format("ScmsSoaLibrary.Core.Reports.Module.Transaksi:GeneratePackingList - {0}", ex.Message);

                      Logger.WriteLine(tmp);
                      Logger.WriteLine(ex.StackTrace);
                  }

                  db.Dispose();

                  #endregion
              }
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

    public static Functionals.ReportingGeneratorResult GenerateDOPL(Config cfg, ReportParser rpt, bool isAsync)
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
      string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");

      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {
          #region DO PL

          case Constant.REPORT_TRANSAKSI_DOPL:
            {
              rptName = "Delivery Order";


              if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
              {
                #region Report

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                if (string.IsNullOrEmpty(rptRecordSel))
                {
                    rptRecordSel = "((if(IsNull({LG_DOH.l_delete})) then false else {LG_DOH.l_delete}) = false)";

                    //rptRecordSel = " 1= 1";
                }
                else
                {
                    rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_DOH.l_delete})) then false else {LG_DOH.l_delete}) = false)");
                    //rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if({nncon}) = "+ decimal  +"   )");
                    //rptRecordSel = string.Concat(rptRecordSel, " AND ", " 1 = 1 ");
                }
                reportFiles = Path.Combine(rtpPath, "LG_DO1_Full.rpt");                


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
          outputFolder = cfg.PathReportStorage
        };

        string tmp = null;

        #region Update

        PostDataParser parser = new PostDataParser();

        IDictionary<string, PostDataParser.StructurePair> dicBarcode = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
        PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);
        string outputMesasge = null;

        dicBarcode.Add("Idx", new PostDataParser.StructurePair()
        {
          IsSet = true,
          Value = "0"
        });
        dicBarcode.Add("Type", new PostDataParser.StructurePair()
        {
          IsSet = true,
          Value = rpt.UserDefinedName
        });

        string varData = null;
        bool isSukses = false;

        if (rpt.IsBarcode)
        {
          varData = parser.ParserData("Reporting", "IsBarcode", dicBarcode);

          if (varData != null)
          {
            string resultSrvc = null;

            Services.Service srvc = new ScmsSoaLibrary.Services.Service();

            resultSrvc = srvc.PostData(varData);

            responseResult = parser.ResponseParser(resultSrvc);

            outputMesasge = responseResult.Message;

            isSukses = (responseResult.Response == PostDataParser.ResponseStatus.Success);
          }
        };

        #endregion

        if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
        {
          if (Functionals.ReportSaveParser(rptName, rpt.ReportingID, reportStruct.outputFolder, reportStruct.outputName, reportStruct.extReport, rpt.User,
            reportStruct.sizeOutput, isAsync, rpt.UserDefinedName, rpt.IsShared, out tmp))
          {
            if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
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

              isGenerated = false;

              ORMDataContext db = new ORMDataContext(cfg.ConnectionString);

              try
              {
                switch (rpt.ReportingID)
                {
                  #region DO PL

                  case Constant.REPORT_TRANSAKSI_DOPL:
                    {
                      var qry = (from q in db.LG_DOHs
                                 where (q.l_delete.HasValue ? q.l_delete.Value : false) == false
                                  && ((q.l_print.HasValue ? q.l_print.Value : false) == false)
                                 select q).AsQueryable();

                      //var qryTmp = (from q in rpt.ReportParameter
                      //              where q.IsSqlParameter == true
                      //              select q);

                      for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
                      {
                        rptParam = rpt.ReportParameter[nLoop];

                        if (rptParam.IsLinqFilterParameter)
                        {
                          if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                          {
                            qry = qry.Where(rptParam.ParameterName, rptParam.ParameterValueObject);

                            isGenerated = true;
                          }
                          else
                          {
                            qry = qry.Between(rptParam.ParameterName, rptParam.ParameterValueObject, rptParam.BetweenValueObject);

                            isGenerated = true;
                          }
                        }

                        #region Old Coded

                        //if (rptParam.IsSqlParameter)
                        //{
                        //  if (rptParam.ParameterName.Equals("Gdg", StringComparison.OrdinalIgnoreCase) && (!string.IsNullOrEmpty(rptParam.ParameterValue)))
                        //  {
                        //    gdg = rptParam.ParameterValue[0];

                        //    var Xqry = qry.Where(x => x.c_gdg == gdg);

                        //    qry = Xqry;
                        //  }
                        //  else if (rptParam.ParameterName.Equals("Customer", StringComparison.OrdinalIgnoreCase) && (!string.IsNullOrEmpty(rptParam.ParameterValue)))
                        //  {
                        //    cusno = rptParam.ParameterValue;

                        //    var Xqry = qry.Where(x => x.c_cusno == cusno);

                        //    qry = Xqry;
                        //  }
                        //  else if (rptParam.ParameterName.Equals("DOID", StringComparison.OrdinalIgnoreCase) && (!string.IsNullOrEmpty(rptParam.ParameterValue)))
                        //  {
                        //    if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                        //    {
                        //      var Xqry = qry.Where(x => x.c_plno == rptParam.ParameterValue);

                        //      qry = Xqry;
                        //    }
                        //    else
                        //    {
                        //      //var Xqry = qry.Between(x => x.c_plno, rptParam.ParameterValue, rptParam.BetweenValue).AsQueryable();

                        //      //qry = Xqry.AsQueryable();
                        //    }
                        //  }
                        //}

                        #endregion
                      }

                      List<LG_DOH> lst = null;

                      if (isGenerated)
                      {
                        lst = qry.ToList();

                        LG_DOH doh = null;
                        DateTime date = DateTime.Now;

                        for (int nLoop = 0; nLoop < lst.Count; nLoop++)
                        {
                          doh = lst[nLoop];

                          doh.l_print = true;
                          doh.c_update = rpt.User;
                          doh.d_update = date;


                          //calc koli receh

                          List<LG_DOD1_SUM_BYBATCH> listDOSum = null;

                          LG_DOD1_SUM_BYBATCH dod1Sum = null;
                          List<LG_DOD1> listDod1 = null;
                          FA_MasItm masitm = null;

                          decimal koliKarton = 0,
                            receh = 0,
                            nBox = 0;

                          listDod1 = (from q in db.LG_DOD1s
                                      where q.c_dono == doh.c_dono
                                      select q).Distinct().ToList();

                          listDOSum =
                          listDod1.GroupBy(x => new { x.c_dono, x.c_iteno })
                          .Select(x => new LG_DOD1_SUM_BYBATCH() { c_dono = x.Key.c_dono, c_iteno = x.Key.c_iteno, n_qty = x.Sum(y => (y.n_qty.HasValue ? y.n_qty.Value : 0)) }).ToList();

                          if (listDOSum.Count > 0)
                          {
                              for (nLoop = 0; nLoop < listDOSum.Count; nLoop++)
                              {
                                  dod1Sum = listDOSum[nLoop];

                                  if (dod1Sum != null)
                                  {
                                      //calc koli
                                      masitm = (from q in db.FA_MasItms
                                                where q.c_iteno == dod1Sum.c_iteno
                                                select q).Take(1).SingleOrDefault();

                                      nBox = masitm.n_box ?? 0;

                                      koliKarton += Math.Floor(dod1Sum.n_qty / nBox);
                                      receh += dod1Sum.n_qty % nBox;
                                  }
                              }

                              doh.n_karton = koliKarton;
                              doh.n_receh = receh;

                              listDOSum.Clear();
                              listDod1.Clear();
                          }
                        }

                        db.SubmitChanges();

                        lst.Clear();
                      }

                      break;
                    }

                  #endregion
                }
              }
              catch (Exception ex)
              {
                tmp = string.Format("ScmsSoaLibrary.Core.Reports.Module.Transaksi:GeneratePL - {0}", ex.Message);

                Logger.WriteLine(tmp);
                Logger.WriteLine(ex.StackTrace);
              }

              db.Dispose();

              #endregion
            }
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

    public static Functionals.ReportingGeneratorResult GenerateEkspedisiShipment(Config cfg, ReportParser rpt, bool isAsync)
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
      string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");

      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {
          #region Shipment Ekspedisi

          case Constant.REPORT_TRANSAKSI_EKSPEDISI_SHIPMENT:
            {
              rptName = "Ekspedisi Shipment";

              if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
              {
                #region Report

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                if (string.IsNullOrEmpty(rptRecordSel))
                {
                  rptRecordSel = "((if(IsNull({LG_ExpH.l_delete})) then false else {LG_ExpH.l_delete}) = false)";
                }
                else
                {
                  rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_ExpH.l_delete})) then false else {LG_ExpH.l_delete}) = false)");
                }

                reportFiles = Path.Combine(rtpPath, "LG_Shipment.rpt");

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
          outputFolder = cfg.PathReportStorage
        };

        string tmp = null;

        #region Update

        PostDataParser parser = new PostDataParser();

        IDictionary<string, PostDataParser.StructurePair> dicBarcode = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

        dicBarcode.Add("Idx", new PostDataParser.StructurePair()
        {
          IsSet = true,
          Value = "0"
        });
        dicBarcode.Add("Type", new PostDataParser.StructurePair()
        {
          IsSet = true,
          Value = rpt.UserDefinedName
        });

        #region Update Database

        ORMDataContext db = new ORMDataContext(cfg.ConnectionString);

        try
        {
            switch (rpt.ReportingID)
            {
                #region Ekspedisi Shipment

                case Constant.REPORT_TRANSAKSI_EKSPEDISI_SHIPMENT:
                    {
                        var expno = string.Empty;

                        for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
                        {
                            rptParam = rpt.ReportParameter[nLoop];

                            if (rptParam.ParameterName == "LG_ExpH.c_expno")
                            {
                                expno = rptParam.ParameterValueObject.ToString();
                            }
                        }

                        var qry = (from q in db.LG_ExpHs
                                   where (q.l_delete.HasValue ? q.l_delete.Value : false) == false
                                   && (q.c_expno == expno)
                                   select q).AsQueryable();

                        List<LG_ExpH> lst = null;

                        if (isGenerated)
                        {
                            lst = qry.ToList();

                            LG_ExpH exph = null;
                            DateTime date = DateTime.Now;
                            for (int nLoop = 0; nLoop < lst.Count; nLoop++)
                            {
                                exph = lst[nLoop];

                                exph.l_print = true;
                                exph.c_print = rpt.User;
                                exph.d_print = date;
                                exph.v_reason = rpt.Reason;
                            }
                            db.SubmitChanges();
                            lst.Clear();
                        }
                    }
                    break;
            }

                #endregion
        }
        catch (Exception ex)
        {
            tmp = string.Format("ScmsSoaLibrary.Core.Reports.Module.Transaksi:GenerateSuratPesanan - {0}", ex.Message);

            Logger.WriteLine(tmp);
            Logger.WriteLine(ex.StackTrace);
        }

        db.Dispose();

        #endregion

        #endregion

        if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
        {
          if (Functionals.ReportSaveParser(rptName, rpt.ReportingID, reportStruct.outputFolder, reportStruct.outputName, reportStruct.extReport, rpt.User,
            reportStruct.sizeOutput, isAsync, rpt.UserDefinedName, rpt.IsShared, out tmp))
          {
            if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
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

    public static Functionals.ReportingGeneratorResult GenerateInvoiceEkspedisi(Config cfg, ReportParser rpt, bool isAsync)
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
        string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");

        if (rpt != null)
        {
            switch (rpt.ReportingID)
            {
                #region Invoice Ekspedisi Eksternal

                case Constant.REPORT_TRANSAKSI_INVOICE_EKSPEDISI_EKSTERNAL:
                    {
                        rptName = "Invoice Ekspedisi Eksternal";

                        if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
                        {
                            #region Report

                            rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                            reportFiles = Path.Combine(rtpPath, "LG_InvoiceEkspedisiEksternal.rpt");

                            #endregion
                        }

                        isGenerated = true;
                    }
                    break;

                #endregion

                #region Invoice Ekspedisi Internal

                case Constant.REPORT_TRANSAKSI_INVOICE_EKSPEDISI_INTERNAL:
                    {
                        rptName = "Invoice Ekspedisi Internal";

                        if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
                        {
                            #region Report

                            rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                            reportFiles = Path.Combine(rtpPath, "LG_InvoiceEkspedisiInternal.rpt");

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
                outputFolder = cfg.PathReportStorage
            };

            string tmp = null;

            if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
            {
                if (Functionals.ReportSaveParser(rptName, rpt.ReportingID, reportStruct.outputFolder, reportStruct.outputName, reportStruct.extReport, rpt.User,
                  reportStruct.sizeOutput, isAsync, rpt.UserDefinedName, rpt.IsShared, out tmp))
                {
                    if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
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

    public static Functionals.ReportingGeneratorResult GenerateFakturEkspedisi(Config cfg, ReportParser rpt, bool isAsync)
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
        string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");

        if (rpt != null)
        {
            switch (rpt.ReportingID)
            {
                #region Invoice Ekspedisi Eksternal

                case Constant.REPORT_TRANSAKSI_FAKTUR_EKSPEDISI:
                    {
                        rptName = "Faktur Ekspedisi";

                        if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
                        {
                            #region Report

                            rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                            if (string.IsNullOrEmpty(rptRecordSel))
                            {
                                rptRecordSel = "((if(IsNull({LG_BEH.l_delete})) then false else {LG_BEH.l_delete}) = false)";
                            }
                            else
                            {
                                rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_BEH.l_delete})) then false else {LG_BEH.l_delete}) = false)");
                            }


                            #endregion
                        }

                        reportFiles = Path.Combine(rtpPath, "LG_BE.rpt");

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
                    if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
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

    public static Functionals.ReportingGeneratorResult GenerateDOPLHO2(Config cfg, ReportParser rpt, bool isAsync)
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
      string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");

      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {
          #region DO PL

          case Constant.REPORT_TRANSAKSI_DOPL_PRINT_HO2:
            {
              rptName = "Delivery Order";

              if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
              {
                #region Report

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                if (string.IsNullOrEmpty(rptRecordSel))
                {
                  rptRecordSel = "((if(IsNull({LG_DOH.l_delete})) then false else {LG_DOH.l_delete}) = false)";

                  //rptRecordSel = " 1= 1";
                }
                else
                {
                  rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_DOH.l_delete})) then false else {LG_DOH.l_delete}) = false)");
                  //rptRecordSel = string.Concat(rptRecordSel, " AND ", " 1 = 1 ");
                }

                reportFiles = Path.Combine(rtpPath, "LG_DO2_Full_HO2.rpt");

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
          outputFolder = cfg.PathReportStorage
        };

        string tmp = null;

        #region Update

        PostDataParser parser = new PostDataParser();

        IDictionary<string, PostDataParser.StructurePair> dicBarcode = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
        PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);
        string outputMesasge = null;

        dicBarcode.Add("Idx", new PostDataParser.StructurePair()
        {
          IsSet = true,
          Value = "0"
        });
        dicBarcode.Add("Type", new PostDataParser.StructurePair()
        {
          IsSet = true,
          Value = rpt.UserDefinedName
        });

        string varData = null;
        bool isSukses = false;

        if (rpt.IsBarcode)
        {
          varData = parser.ParserData("Reporting", "IsBarcode", dicBarcode);

          if (varData != null)
          {
            string resultSrvc = null;

            Services.Service srvc = new ScmsSoaLibrary.Services.Service();

            resultSrvc = srvc.PostData(varData);

            responseResult = parser.ResponseParser(resultSrvc);

            outputMesasge = responseResult.Message;

            isSukses = (responseResult.Response == PostDataParser.ResponseStatus.Success);
          }
        };

        #endregion

        if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
        {
          if (Functionals.ReportSaveParser(rptName, rpt.ReportingID, reportStruct.outputFolder, reportStruct.outputName, reportStruct.extReport, rpt.User,
            reportStruct.sizeOutput, isAsync, rpt.UserDefinedName, rpt.IsShared, out tmp))
          {
            if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
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

              isGenerated = false;

              ORMDataContext db = new ORMDataContext(cfg.ConnectionString);

              try
              {
                switch (rpt.ReportingID)
                {
                  #region DO PL

                  case Constant.REPORT_TRANSAKSI_DOPL_PRINT_HO2:
                    {
                      var qry = (from q in db.LG_DOHs
                                 where (q.l_delete.HasValue ? q.l_delete.Value : false) == false
                                  && ((q.l_print.HasValue ? q.l_print.Value : false) == false)
                                 select q).AsQueryable();

                      //var qryTmp = (from q in rpt.ReportParameter
                      //              where q.IsSqlParameter == true
                      //              select q);

                      for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
                      {
                        rptParam = rpt.ReportParameter[nLoop];

                        if (rptParam.IsLinqFilterParameter)
                        {
                          if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                          {
                            qry = qry.Where(rptParam.ParameterName, rptParam.ParameterValueObject);

                            isGenerated = true;
                          }
                          else
                          {
                            qry = qry.Between(rptParam.ParameterName, rptParam.ParameterValueObject, rptParam.BetweenValueObject);

                            isGenerated = true;
                          }
                        }
                      }

                      List<LG_DOH> lst = null;

                      if (isGenerated)
                      {
                        lst = qry.ToList();

                        LG_DOH doh = null;
                        DateTime date = DateTime.Now;

                        for (int nLoop = 0; nLoop < lst.Count; nLoop++)
                        {
                          doh = lst[nLoop];

                          doh.l_print = true;
                          doh.c_update = rpt.User;
                          doh.d_update = date;

                          //calc koli receh

                          List<LG_DOD1_SUM_BYBATCH> listDOSum = null;

                          LG_DOD1_SUM_BYBATCH dod1Sum = null;
                          List<LG_DOD1> listDod1 = null;
                          FA_MasItm masitm = null;

                          decimal koliKarton = 0,
                            receh = 0,
                            nBox = 0;

                          listDod1 = (from q in db.LG_DOD1s
                                      where q.c_dono == doh.c_dono
                                      select q).Distinct().ToList();

                          listDOSum =
                          listDod1.GroupBy(x => new { x.c_dono, x.c_iteno })
                          .Select(x => new LG_DOD1_SUM_BYBATCH() { c_dono = x.Key.c_dono, c_iteno = x.Key.c_iteno, n_qty = x.Sum(y => (y.n_qty.HasValue ? y.n_qty.Value : 0)) }).ToList();

                          if (listDOSum.Count > 0)
                          {
                              for (nLoop = 0; nLoop < listDOSum.Count; nLoop++)
                              {
                                  dod1Sum = listDOSum[nLoop];

                                  if (dod1Sum != null)
                                  {
                                      //calc koli
                                      masitm = (from q in db.FA_MasItms
                                                where q.c_iteno == dod1Sum.c_iteno
                                                select q).Take(1).SingleOrDefault();

                                      nBox = masitm.n_box ?? 0;

                                      koliKarton += Math.Floor(dod1Sum.n_qty / nBox);
                                      receh += dod1Sum.n_qty % nBox;
                                  }
                              }

                              doh.n_karton = koliKarton;
                              doh.n_receh = receh;

                              listDOSum.Clear();
                              listDod1.Clear();
                          }
                        }

                        db.SubmitChanges();

                        lst.Clear();
                      }

                      break;
                    }

                  #endregion
                }
              }
              catch (Exception ex)
              {
                tmp = string.Format("ScmsSoaLibrary.Core.Reports.Module.Transaksi:GeneratePL - {0}", ex.Message);

                Logger.WriteLine(tmp);
                Logger.WriteLine(ex.StackTrace);
              }

              db.Dispose();

              #endregion
            }
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

    public static Functionals.ReportingGeneratorResult GenerateDOSTT(Config cfg, ReportParser rpt, bool isAsync)
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
      string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");
      
      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {
          #region DO STT

          case Constant.REPORT_TRANSAKSI_DOSTT:
            {
              rptName = "Delivery Order (STT)";

              if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
              {
                #region Report

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                if (string.IsNullOrEmpty(rptRecordSel))
                {
                  rptRecordSel = "((if(IsNull({LG_DOH.l_delete})) then false else {LG_DOH.l_delete}) = false)";
                }
                else
                {
                  rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_DOH.l_delete})) then false else {LG_DOH.l_delete}) = false)");
                }

                reportFiles = Path.Combine(rtpPath, "LG_DO2.rpt");

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
          outputFolder = cfg.PathReportStorage
        };

        string tmp = null;

        if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
        {
          if (Functionals.ReportSaveParser(rptName, rpt.ReportingID, reportStruct.outputFolder, reportStruct.outputName, reportStruct.extReport, rpt.User,
            reportStruct.sizeOutput, isAsync, rpt.UserDefinedName, rpt.IsShared, out tmp))
          {
            if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
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

              isGenerated = false;

              ORMDataContext db = new ORMDataContext(cfg.ConnectionString);

              try
              {
                switch (rpt.ReportingID)
                {
                  #region DO STT

                  case Constant.REPORT_TRANSAKSI_DOSTT:
                    {
                      var qry = (from q in db.LG_DOHs
                                 where ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                  && ((q.l_print.HasValue ? q.l_print.Value : false) == false)
                                 select q).AsQueryable();

                      //var qryTmp = (from q in rpt.ReportParameter
                      //              where q.IsSqlParameter == true
                      //              select q);

                      //char gdg = char.MinValue;
                      //string cusno = null;

                      for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
                      {
                        rptParam = rpt.ReportParameter[nLoop];

                        if (rptParam.IsLinqFilterParameter)
                        {
                          if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                          {
                            qry = qry.Where(rptParam.ParameterName, rptParam.ParameterValueObject);

                            isGenerated = true;
                          }
                          else
                          {
                            qry = qry.Between(rptParam.ParameterName, rptParam.ParameterValueObject, rptParam.BetweenValueObject);

                            isGenerated = true;
                          }
                        }

                        #region Old Coded

                        //if (rptParam.IsSqlParameter)
                        //{
                        //  if (rptParam.ParameterName.Equals("Gdg", StringComparison.OrdinalIgnoreCase) && (!string.IsNullOrEmpty(rptParam.ParameterValue)))
                        //  {
                        //    gdg = rptParam.ParameterValue[0];

                        //    var Xqry = qry.Where(x => x.c_gdg == gdg);

                        //    qry = Xqry;
                        //  }
                        //  else if (rptParam.ParameterName.Equals("Customer", StringComparison.OrdinalIgnoreCase) && (!string.IsNullOrEmpty(rptParam.ParameterValue)))
                        //  {
                        //    cusno = rptParam.ParameterValue;

                        //    var Xqry = qry.Where(x => x.c_cusno == cusno);

                        //    qry = Xqry;
                        //  }
                        //  else if (rptParam.ParameterName.Equals("DOID", StringComparison.OrdinalIgnoreCase) && (!string.IsNullOrEmpty(rptParam.ParameterValue)))
                        //  {
                        //    if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                        //    {
                        //      var Xqry = qry.Where(x => x.c_plno == rptParam.ParameterValue);

                        //      qry = Xqry;
                        //    }
                        //    else
                        //    {
                        //      //var Xqry = qry.Between(x => x.c_plno, rptParam.ParameterValue, rptParam.BetweenValue).AsQueryable();

                        //      //qry = Xqry.AsQueryable();
                        //    }
                        //  }
                        //}

                        #endregion
                      }

                      List<LG_DOH> lst = null;

                      if (isGenerated)
                      {
                        lst = qry.ToList();

                        LG_DOH doh = null;
                        DateTime date = DateTime.Now;

                        for (int nLoop = 0; nLoop < lst.Count; nLoop++)
                        {
                          doh = lst[nLoop];

                          doh.l_print = true;
                          doh.c_update = rpt.User;
                          doh.d_update = date;
                        }

                        db.SubmitChanges();

                        lst.Clear();
                      }

                      break;
                    }

                  #endregion
                }
              }
              catch (Exception ex)
              {
                tmp = string.Format("ScmsSoaLibrary.Core.Reports.Module.Transaksi:GenerateDOSTT - {0}", ex.Message);

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
      }

      lstParams.Clear();

      return result;
    }

    public static Functionals.ReportingGeneratorResult GeneratePO(Config cfg, ReportParser rpt, bool isAsync)
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
      string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");

      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {
          #region PO

          case Constant.REPORT_TRANSAKSI_PO:
            {
              rptName = "Purchase Order";

              if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
              {
                #region Report

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                if (string.IsNullOrEmpty(rptRecordSel))
                {
                  rptRecordSel = "((if(IsNull({lg_vwPOCusno.l_delete})) then false else {lg_vwPOCusno.l_delete}) = false)";
                }
                else
                {
                  rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({lg_vwPOCusno.l_delete})) then false else {lg_vwPOCusno.l_delete}) = false)");
                  //rptRecordSel = string.Concat(rptRecordSel, " AND ", " {lg_vwPOCusno.l_delete} = false or {lg_vwPOCusno.l_delete} is null");
                }

                reportFiles = Path.Combine(rtpPath, "LG_PO.rpt");

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

            #region Update Database

            isGenerated = false;

            ORMDataContext db = new ORMDataContext(cfg.ConnectionString);

            try
            {
              switch (rpt.ReportingID)
              {
                #region PO

                case Constant.REPORT_TRANSAKSI_PO:
                  {
                    //var qry = (from q in db.LG_DOHs
                    //           where (q.l_delete.HasValue ? q.l_delete.Value : false) == false
                    //           select q).AsQueryable();

                    var qry = (from q in db.LG_POHs
                               where ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                && ((q.l_print.HasValue ? q.l_print.Value : false) == false)
                               select q).AsQueryable();

                    //var qryTmp = (from q in rpt.ReportParameter
                    //              where q.IsSqlParameter == true
                    //              select q);

                    //char gdg = char.MinValue;
                    //string cusno = null;

                    for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
                    {
                      rptParam = rpt.ReportParameter[nLoop];

                      if (rptParam.IsLinqFilterParameter)
                      {
                        if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                        {
                          qry = qry.Where(rptParam.ParameterName, rptParam.ParameterValueObject);

                          isGenerated = true;
                        }
                        else
                        {
                          qry = qry.Between(rptParam.ParameterName, rptParam.ParameterValueObject, rptParam.BetweenValueObject);

                          isGenerated = true;
                        }
                      }

                      #region Old Coded

                      //if (rptParam.IsSqlParameter)
                      //{
                      //  //if (rptParam.ParameterName.Equals("Gdg", StringComparison.OrdinalIgnoreCase) && (!string.IsNullOrEmpty(rptParam.ParameterValue)))
                      //  //{
                      //  //  gdg = rptParam.ParameterValue[0];

                      //  //  var Xqry = qry.Where(x => x.c_gdg == gdg);

                      //  //  qry = Xqry;
                      //  //}
                      //  //else if (rptParam.ParameterName.Equals("Customer", StringComparison.OrdinalIgnoreCase) && (!string.IsNullOrEmpty(rptParam.ParameterValue)))
                      //  //{
                      //  //  cusno = rptParam.ParameterValue;

                      //  //  var Xqry = qry.Where(x => x.c_cusno == cusno);

                      //  //  qry = Xqry;
                      //  //}
                      //  //else if (rptParam.ParameterName.Equals("DOID", StringComparison.OrdinalIgnoreCase) && (!string.IsNullOrEmpty(rptParam.ParameterValue)))
                      //  //{
                      //  //  if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                      //  //  {
                      //  //    var Xqry = qry.Where(x => x.c_plno == rptParam.ParameterValue);

                      //  //    qry = Xqry;
                      //  //  }
                      //  //  else
                      //  //  {
                      //  //    //var Xqry = qry.Between(x => x.c_plno, rptParam.ParameterValue, rptParam.BetweenValue).AsQueryable();

                      //  //    //qry = Xqry.AsQueryable();
                      //  //  }
                      //  //}
                      //}

                      #endregion
                    }

                    List<LG_POH> lst = null;

                    if (isGenerated)
                    {
                      lst = qry.ToList();

                      LG_POH poh = null;
                      DateTime date = DateTime.Now;

                      for (int nLoop = 0; nLoop < lst.Count; nLoop++)
                      {
                        poh = lst[nLoop];

                        poh.l_print = true;
                        poh.c_update = rpt.User;
                        poh.d_update = date;
                      }

                      db.SubmitChanges();

                      lst.Clear();
                    }

                    break;
                  }

                #endregion
              }
            }
            catch (Exception ex)
            {
              tmp = string.Format("ScmsSoaLibrary.Core.Reports.Module.Transaksi:GeneratePO - {0}", ex.Message);

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

    public static Functionals.ReportingGeneratorResult GenerateRS(Config cfg, ReportParser rpt, bool isAsync)
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
      string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");

      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {
          #region Pembelian

          case Constant.REPORT_TRANSAKSI_RSBELI:
            {
              rptName = "Retur Suplier Pembelian";

              if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
              {
                #region Report

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                if (string.IsNullOrEmpty(rptRecordSel))
                {
                  rptRecordSel = "((if(IsNull({LG_RSH.l_delete})) then false else {LG_RSH.l_delete}) = false)";
                }
                else
                {
                  rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_RSH.l_delete})) then false else {LG_RSH.l_delete}) = false)");
                }

                reportFiles = Path.Combine(rtpPath, "LG_RS.rpt");

                #endregion
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Pembelian Upload

          case Constant.REPORT_TRANSAKSI_RSBELI_UPLOAD:
            {
                rptName = "Retur Suplier Pembelian Upload";
                string noSup = null,
                    rsNo = null,
                    tipeRS = null;
                if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
                {
                    #region Sql                                    
                                    
                                    rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                                    {
                                        bool isOk = false;

                                        if (x.ParameterName.Equals("rsno", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                                        {
                                            isOk = true;
                                        }

                                        return isOk;
                                    });
                                    if (rptParam != null)
                                    {
                                        rsNo = rptParam.ParameterValue;

                                        lstParams.Add(new SqlParameter(string.Concat("@", rptParam.ParameterName),
                                            System.Data.SqlDbType.VarChar)
                                        {
                                            Size = 10,
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
                                        noSup = rptParam.ParameterValue;
                                    }

                                    rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                                    {
                                        bool isOk = false;

                                        if (x.ParameterName.Equals("tipeRS", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                                        {
                                            isOk = true;
                                        }

                                        return isOk;
                                    });
                                    if (rptParam != null)
                                    {
                                        tipeRS = rptParam.ParameterValue;
                                    }

                    #endregion

                    #region Report

                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                    if (string.IsNullOrEmpty(rptRecordSel))
                    {
                        rptRecordSel = "((if(IsNull({LG_RSH.l_delete})) then false else {LG_RSH.l_delete}) = false)";
                    }
                    else
                    {
                        rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_RSH.l_delete})) then false else {LG_RSH.l_delete}) = false)");
                    }

                    reportFiles = Path.Combine(rtpPath, "LG_RS_Upload.rpt");

                    #endregion
                }

                if(string.IsNullOrEmpty(noSup))
                {
                    isGenerated = true;
                }
                else
                {
                    isGenerated =false;

                    if (tipeRS == "01")
                    {
                        tmpQuery = "Exec LG_Proces_CPRInv @rsno";
                        Functionals.ExecProcedures(cfg, tmpQuery, lstParams.ToArray());
                    }

                    #region Send RS
                    SqlConnection cn = new SqlConnection(cfg.ConnectionString);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    #region Old
                    //string strCommand = "select a.c_iteno Product_ID,c.c_itenopri,c.v_itnam NamaProduk,a.c_batch,a.c_reason ID_Alasan,d.v_reason Alasan, " +
                    //            "f.d_expired,case when a.n_gqty = 0 then convert(decimal, a.n_bqty) else convert (decimal, a.n_gqty) end Qty_Retur,e.v_outlet NamaOutlet, " +
                    //            "e.c_outlet KodeOutlet,e.c_cab Kode_Area,a.c_rsno C_RS,b.d_rsdate D_RSDATE,a.c_fb C_INVNO " +
                    //            "from LG_RSD1 a join LG_RSH b on a.c_rsno = b.c_rsno " +
                    //            "join FA_MasItm c on a.c_iteno = c.c_iteno " +
                    //            "left join SCMS_MSRETUR_REASON d on a.c_reason = d.c_reason " +
                    //            "left join SCMS_MSOUTLET_CABANG e on a.c_outlet = e.c_outlet and a.c_cusno = e.c_cusno " +
                    //            "left join LG_MsBatch f on a.c_iteno = f.c_iteno and a.c_batch = f.c_batch " +
                    //            "where a.c_rsno = '" + rsNo + "' ";
                    #endregion
                    //by hafizh 01 maret 2018
                    //string strCommand = "select x.Product_ID,x.c_itenopri,x.NamaProduk,x.c_batch,x.ID_Alasan,x.Alasan,x.d_expired,replace(x.Qty_Retur,',','.') Qty_Retur,x.NamaOutlet,x.KodeOutlet,x.Kode_Area,x.C_RS,x.D_RSDATE,x.C_INVNO from (" +
                    //         "select a.c_rsno,a.c_iteno Product_ID,c.c_itenopri,c.v_itnam NamaProduk,a.c_batch,a.c_reason ID_Alasan,d.v_reason Alasan, " +
                    //         "f.d_expired, case when a.n_gqty = 0 then  a.n_bqty else a.n_gqty end Qty_Retur,e.v_outlet NamaOutlet, " +
                    //         "e.c_outlet KodeOutlet,e.c_cab Kode_Area,a.c_rsno C_RS,b.d_rsdate D_RSDATE,a.c_fb C_INVNO " +
                    //         "from LG_RSD1 a join LG_RSH b on a.c_rsno = b.c_rsno " +
                    //         "join FA_MasItm c on a.c_iteno = c.c_iteno " +
                    //         "left join SCMS_MSRETUR_REASON d on a.c_reason = d.c_reason " +
                    //         "left join SCMS_MSOUTLET_CABANG e on a.c_outlet = e.c_outlet and a.c_cusno = e.c_cusno " +
                    //         "left join LG_MsBatch f on a.c_iteno = f.c_iteno and a.c_batch = f.c_batch " +
                    //         ") x where x.c_rsno = '" + rsNo + "' ";



                    string strCommand = "select a.c_iteno Product_ID,c.c_itenopri,c.v_itnam NamaProduk,a.c_batch,a.c_reason ID_Alasan,d.v_reason Alasan, " +
                               "f.d_expired, case when a.n_gqty = 0 then convert (decimal,a.n_bqty) else convert (decimal, a.n_gqty) end Qty_Retur,e.v_outlet NamaOutlet, " +
                               "e.c_outlet KodeOutlet,e.c_cab Kode_Area,a.c_rsno C_RS,b.d_rsdate D_RSDATE,a.c_fb C_INVNO " +
                               "from LG_RSD1 a join LG_RSH b on a.c_rsno = b.c_rsno " +
                               "join FA_MasItm c on a.c_iteno = c.c_iteno " +
                               "left join SCMS_MSRETUR_REASON d on a.c_reason = d.c_reason " +
                               "left join SCMS_MSOUTLET_CABANG e on a.c_outlet = e.c_outlet and a.c_cusno = e.c_cusno " +
                               "left join LG_MsBatch f on a.c_iteno = f.c_iteno and a.c_batch = f.c_batch " +
                               "where a.c_rsno = '" + rsNo + "' ";


                    cn.Open();
                    cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandText = strCommand;

                    DataTable dt = new DataTable(),
                      dt1 = new DataTable();
                    DataSet ds = new DataSet(),
                      ds1 = new DataSet();

                    string path = null,
                        supName = string.Empty,
                        supName2 = string.Empty;
                    if (noSup.Equals("00085"))
                    {
                        path = @"D:\ETERCON\RETUR\";
                        //supName = "PI";
                        //supName2 = "PT. PHAROS INDONESIA";
                    }
                    else if (noSup.Equals("00019"))
                    {
                        path = @"D:\NOVELL\RETUR\";
                        //supName = "NJA";
                        //supName2 = "PT. NUTRINDO JAYA ABADI";
                    }
                    
                    if (!string.IsNullOrEmpty(path))
                    {
                        if (!string.IsNullOrEmpty(strCommand))
                        { 
                            SqlDataReader reader1 = cmd.ExecuteReader();

                            if (reader1.HasRows)
                            {
                                dt1.Load(reader1);

                                ds1.Tables.Add(dt1);
                            }

                            reader1.Close();
                            reader1.Dispose();

                            bool isSukses = false;

                            if (ds1.Tables.Count > 0 && (!string.IsNullOrEmpty(path)) && (!string.IsNullOrEmpty(rsNo)))
                            {
                                isSukses = ExportDBF(ds1, path, rsNo + ".dbf", false, false);
                                isSukses = ExportDBF(ds1, path, rsNo + ".txt", false, true);
                            }

                            result = new Functionals.ReportingGeneratorResult()
                            {
                                IsSet = true,
                                IsSuccess = false,
                                Messages = "Send Upload RS berhasil.",
                                Extension = null,
                                OutputFile = null,
                                OutputPath = null,
                                Size = null
                            };
                        }
                    }

                    #endregion

                    // hafizh projek send mail 1 maret 2018
                    #region Mail

                    if (noSup.Equals("00085") || noSup.Equals("00019"))
                    {



                        var connectionStringSql = "Driver={SQL Server};Server=10.100.41.29;Database=AMS;Uid=sa;Pwd=4M5M1s2015";

                        try
                        {
                            using (OdbcConnection conSql = new OdbcConnection(connectionStringSql))
                            {
                                conSql.Open();

                                DateTime date2 = DateTime.Today;
                                DateTime date = date2.AddMonths(-1);
                                DateTime date1 = new DateTime(date.Year, date.Month, 1);
                                DataTable tbl = new DataTable();


                                var qrySelect = "select a.c_iteno Product_ID,c.c_itenopri,c.v_itnam NamaProduk,a.c_batch,a.c_reason ID_Alasan,d.v_reason Alasan, " +
                                                "f.d_expired,case when a.n_gqty = 0 then a.n_bqty else a.n_gqty end Qty_Retur,e.v_outlet NamaOutlet, " +
                                                "e.c_outlet KodeOutlet,e.c_cab Kode_Area,a.c_rsno C_RS,b.d_rsdate D_RSDATE,a.c_fb C_INVNO " +
                                                "from LG_RSD1 a join LG_RSH b on a.c_rsno = b.c_rsno " +
                                                "join FA_MasItm c on a.c_iteno = c.c_iteno " +
                                                "left join SCMS_MSRETUR_REASON d on a.c_reason = d.c_reason " +
                                                "left join SCMS_MSOUTLET_CABANG e on a.c_outlet = e.c_outlet and a.c_cusno = e.c_cusno " +
                                                "left join LG_MsBatch f on a.c_iteno = f.c_iteno and a.c_batch = f.c_batch " +
                                                "where a.c_rsno = '" + rsNo + "' ";



                                OdbcDataAdapter adpStock = new OdbcDataAdapter(qrySelect, conSql);
                                //Console.WriteLine("Save to excel");
                                adpStock.Fill(tbl);

                                string path1 = "C:\\Proses_" + rsNo + ".xlsx";
                                string pathOld = "C:\\Proses_RS" + rsNo + DateTime.Now.AddDays(-1).ToString("ddMMyyyy") + ".xlsx";
                                string pathserver = "U:\\RNCAB\\Proses_RS" + rsNo + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";

                                using (XLWorkbook wb = new XLWorkbook())
                                {
                                    wb.Worksheets.Add(tbl, "RS");
                                    wb.SaveAs(path1);
                                }

                                //File.Copy(path,pathserver,true);

                                // di remark ketika pilih tahun
                                #region Send Mail
                                //Console.WriteLine("Send Email");

                                System.Net.Mail.SmtpClient smtp = null;
                                StringBuilder sb = new StringBuilder();
                                try
                                {
                                    using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                                    {
                                        // send mail containing the file here

                                        mail.From = new System.Net.Mail.MailAddress("scms.sph@ams.co.id", "Send Mail RS");//mail.From = new System.Net.Mail.MailAddress("hafizh.ahmad@ams.co.id", "Supply Chain Management System");

                                        mail.Subject = "Proses RS";


                                        mail.To.Add("teddy@ams.co.id");
                                        mail.To.Add("enik@ams.co.id");
                                        mail.To.Add("noval@ams.co.id");
                                        mail.CC.Add("dudy.budiman@ams.co.id");
                                        mail.CC.Add("akhirudin.sudiyat@ams.co.id");
                                        mail.To.Add("siswanto@log.ams.co.id");
                                        mail.To.Add("movi@log.ams.co.id");
                                        mail.To.Add("denny@ams.co.id");
                                        mail.To.Add("arfan@ams.co.id");


                                        if (noSup.Equals("00085"))
                                        {
                                            mail.To.Add("inawati@plant.eterconpharma.com");
                                        }

                                        else if (noSup.Equals("00019"))
                                        {
                                            mail.CC.Add("admin3_ppic-gp@novellpharm.com");
                                            mail.CC.Add("Sheila-gp@novellpharm.com");
                                            mail.CC.Add("Melvi@novellpharm.com");
                                        }


                                        sb.AppendLine("Proses send " + rsNo + ",  " + date2.ToString("dd MMM yyyy"));
                                        sb.AppendLine("Berhasil !!! ");
                                        using (System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(path1))
                                        {
                                            mail.Attachments.Add(attachment);

                                            sb.AppendLine();
                                            sb.AppendLine();
                                            sb.AppendLine("Terima Kasih,");
                                            sb.AppendLine("AMS - MIS Team");

                                            mail.Body = sb.ToString();


                                            //smtp = new System.Net.Mail.SmtpClient(SMTP_IP, SMTP_PORT);
                                            smtp = new System.Net.Mail.SmtpClient("10.100.10.9", 25);

                                            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                                            smtp.UseDefaultCredentials = false;
                                            smtp.Credentials = new System.Net.NetworkCredential("scms.sph@ams.co.id", "scms");
                                            //smtp.Credentials = new System.Net.NetworkCredential("hafizh.ahmad@ams.co.id", "hafizh.");

                                            smtp.Send(mail);
                                            sb.Length = 0;


                                                                                   }
                                    }

                                    //File.Delete(path);
                                    if (File.Exists(pathOld))
                                    {
                                        File.Delete(pathOld);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    Console.WriteLine(ex.StackTrace);
                                }
                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                    }
                    #endregion
                }
            }
            break;

          #endregion

          #region Pembelian Confirm

          case Constant.REPORT_TRANSAKSI_RSBELI_CONF:
            {
              rptName = "Retur Suplier Confirm";

              if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
              {
                #region Report

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                if (string.IsNullOrEmpty(rptRecordSel))
                {
                  rptRecordSel = "((if(IsNull({LG_RSH.l_delete})) then false else {LG_RSH.l_delete}) = false)";
                }
                else
                {
                  rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_RSH.l_delete})) then false else {LG_RSH.l_delete}) = false)");
                }

                reportFiles = Path.Combine(rtpPath, "LG_RS.rpt");

                #endregion
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Repack

          case Constant.REPORT_TRANSAKSI_RSREPACK:
            {
              rptName = "Retur Suplier Repack";

              if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
              {
                #region Report

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                if (string.IsNullOrEmpty(rptRecordSel))
                {
                  rptRecordSel = "((if(IsNull({LG_RSH.l_delete})) then false else {LG_RSH.l_delete}) = false)";
                }
                else
                {
                  rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_RSH.l_delete})) then false else {LG_RSH.l_delete}) = false)");
                }

                reportFiles = Path.Combine(rtpPath, "LG_RS.rpt");

                #endregion
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Repack Upload

          case Constant.REPORT_TRANSAKSI_RSREPACK_UPLOAD:
            {
                rptName = "Retur Suplier Repack Upload";

                if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
                {
                    #region Report

                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                    if (string.IsNullOrEmpty(rptRecordSel))
                    {
                        rptRecordSel = "((if(IsNull({LG_RSH.l_delete})) then false else {LG_RSH.l_delete}) = false)";
                    }
                    else
                    {
                        rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_RSH.l_delete})) then false else {LG_RSH.l_delete}) = false)");
                    }

                    reportFiles = Path.Combine(rtpPath, "LG_RS_Upload.rpt");

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
          outputFolder = cfg.PathReportStorage
        };

        string tmp = null;

        if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
        {
            if (Functionals.ReportSaveParser(rptName, rpt.ReportingID, reportStruct.outputFolder, reportStruct.outputName, reportStruct.extReport, rpt.User,
              reportStruct.sizeOutput, isAsync, rpt.UserDefinedName, rpt.IsShared, out tmp))
            {
                if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
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

                    isGenerated = false;

                    ORMDataContext db = new ORMDataContext(cfg.ConnectionString);

                    try
                    {
                        switch (rpt.ReportingID)
                        {
                            #region RS

                            case Constant.REPORT_TRANSAKSI_RSBELI:
                            case Constant.REPORT_TRANSAKSI_RSBELI_CONF:
                            case Constant.REPORT_TRANSAKSI_RSREPACK:
                                {
                                    var qry = (from q in db.LG_RSHes
                                               where ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                                && ((q.l_print.HasValue ? q.l_print.Value : false) == false)
                                               select q).AsQueryable();

                                    for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
                                    {
                                        rptParam = rpt.ReportParameter[nLoop];

                                        if (rptParam.IsLinqFilterParameter)
                                        {
                                            if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                                            {
                                                qry = qry.Where(rptParam.ParameterName, rptParam.ParameterValueObject);

                                                isGenerated = true;
                                            }
                                            else
                                            {
                                                qry = qry.Between(rptParam.ParameterName, rptParam.ParameterValueObject, rptParam.BetweenValueObject);

                                                isGenerated = true;
                                            }
                                        }
                                    }

                                    List<LG_RSH> lst = null;

                                    if (isGenerated)
                                    {
                                        lst = qry.ToList();

                                        LG_RSH rsh = null;
                                        DateTime date = DateTime.Now;

                                        for (int nLoop = 0; nLoop < lst.Count; nLoop++)
                                        {
                                            rsh = lst[nLoop];

                                            rsh.l_print = true;
                                            rsh.c_update = rpt.User;
                                            rsh.d_update = date;
                                        }

                                        db.SubmitChanges();

                                        lst.Clear();
                                    }

                                    break;
                                }

                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        tmp = string.Format("ScmsSoaLibrary.Core.Reports.Module.Transaksi:GenerateRS - {0}", ex.Message);

                        Logger.WriteLine(tmp);
                        Logger.WriteLine(ex.StackTrace);
                    }

                    db.Dispose();

                    #endregion
                }
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

    public static Functionals.ReportingGeneratorResult GenerateSTT(Config cfg, ReportParser rpt, bool isAsync)
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
      string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");

      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {
          #region STT Donasi

          case Constant.REPORT_TRANSAKSI_STT_DONASI:
            {              
              rptName = "Surat Tanda Terima - Donasi";

              if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
              {
                #region Report

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                if (string.IsNullOrEmpty(rptRecordSel))
                {
                  rptRecordSel = "((if(IsNull({LG_STH.l_delete})) then false else {LG_STH.l_delete}) = false)";
                }
                else
                {
                  rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_STH.l_delete})) then false else {LG_STH.l_delete}) = false)");
                }

                reportFiles = Path.Combine(rtpPath, "LG_STT.rpt");

                #endregion
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region STT Sample

          case Constant.REPORT_TRANSAKSI_STT_SAMPLE:
            {
              rptName = "Surat Tanda Terima - Sample";

              if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
              {
                #region Report

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                if (string.IsNullOrEmpty(rptRecordSel))
                {
                  rptRecordSel = "((if(IsNull({LG_STH.l_delete})) then false else {LG_STH.l_delete}) = false)";
                }
                else
                {
                  rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_STH.l_delete})) then false else {LG_STH.l_delete}) = false)");
                }

                reportFiles = Path.Combine(rtpPath, "LG_STT.rpt");

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

            #region Update Database

            ORMDataContext db = new ORMDataContext(cfg.ConnectionString);

            try
            {
              switch (rpt.ReportingID)
              {
                #region STT Donasi dan Sample

                case Constant.REPORT_TRANSAKSI_STT_DONASI:
                case Constant.REPORT_TRANSAKSI_STT_SAMPLE:
                  {
                    var qry = (from q in db.LG_STHs
                               where ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                && ((q.l_print.HasValue ? q.l_print.Value : false) == false)
                               select q).AsQueryable();

                    for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
                    {
                      rptParam = rpt.ReportParameter[nLoop];

                      if (rptParam.IsLinqFilterParameter)
                      {
                        if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                        {
                          qry = qry.Where(rptParam.ParameterName, rptParam.ParameterValueObject);

                          isGenerated = true;
                        }
                        else
                        {
                          qry = qry.Between(rptParam.ParameterName, rptParam.ParameterValueObject, rptParam.BetweenValueObject);

                          isGenerated = true;
                        }
                      }
                    }

                    List<LG_STH> lst = qry.ToList();

                    if (isGenerated)
                    {
                      lst = qry.ToList();

                      LG_STH sth = null;
                      DateTime date = DateTime.Now;

                      for (int nLoop = 0; nLoop < lst.Count; nLoop++)
                      {
                        sth = lst[nLoop];

                        sth.l_print = true;
                        sth.c_update = rpt.User;
                        sth.d_update = date;
                      }

                      db.SubmitChanges();

                      lst.Clear();
                    }

                    break;
                  }

                #endregion
              }
            }
            catch (Exception ex)
            {
              tmp = string.Format("ScmsSoaLibrary.Core.Reports.Module.Transaksi:GenerateSTT - {0}", ex.Message);

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

    public static Functionals.ReportingGeneratorResult GenerateCombo(Config cfg, ReportParser rpt, bool isAsync)
    {
      if ((rpt == null) || (cfg == null))
      {
        return new Functionals.ReportingGeneratorResult();
      }

      Functionals.ReportingGeneratorResult result = new Functionals.ReportingGeneratorResult();

      List<SqlParameter> lstParams = new List<SqlParameter>();
      //ReportParameter rptParam = null;
      string tmpQuery = null;
      bool isGenerated = false;
      string reportFiles = null;
      string rptRecordSel = null;
      string rptName = null;
      string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");

      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {
          #region PO

          case Constant.REPORT_TRANSAKSI_COMBO:
            {
              rptName = "Combo";

              if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
              {
                #region Report

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                if (string.IsNullOrEmpty(rptRecordSel))
                {
                  rptRecordSel = "((if(IsNull({LG_COMBOH.l_delete})) then false else {LG_COMBOH.l_delete}) = false)";
                }
                else
                {
                  rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_COMBOH.l_delete})) then false else {LG_COMBOH.l_delete}) = false)");
                }

                reportFiles = Path.Combine(rtpPath, "LG_Combo.rpt");

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

            #region Update Database

            //isGenerated = false;

            //ORMDataContext db = new ORMDataContext(cfg.ConnectionString);

            //try
            //{
            //  switch (rpt.ReportingID)
            //  {
            //    #region Combo

            //    case Constant.REPORT_TRANSAKSI_COMBO:
            //      {
            //        var qry = (from q in db.LG_ComboHs
            //                   where ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
            //                    && ((q.l_print.HasValue ? q.l_print.Value : false) == false)
            //                   select q).AsQueryable();

            //        for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
            //        {
            //          rptParam = rpt.ReportParameter[nLoop];

            //          if (rptParam.IsLinqFilterParameter)
            //          {
            //            if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
            //            {
            //              qry = qry.Where(rptParam.ParameterName, rptParam.ParameterValueObject);

            //              isGenerated = true;
            //            }
            //            else
            //            {
            //              qry = qry.Between(rptParam.ParameterName, rptParam.ParameterValueObject, rptParam.BetweenValueObject);

            //              isGenerated = true;
            //            }
            //          }

            //          #region Old Coded

            //          //if (rptParam.IsSqlParameter)
            //          //{
            //          //  //if (rptParam.ParameterName.Equals("Gdg", StringComparison.OrdinalIgnoreCase) && (!string.IsNullOrEmpty(rptParam.ParameterValue)))
            //          //  //{
            //          //  //  gdg = rptParam.ParameterValue[0];

            //          //  //  var Xqry = qry.Where(x => x.c_gdg == gdg);

            //          //  //  qry = Xqry;
            //          //  //}
            //          //  //else if (rptParam.ParameterName.Equals("Customer", StringComparison.OrdinalIgnoreCase) && (!string.IsNullOrEmpty(rptParam.ParameterValue)))
            //          //  //{
            //          //  //  cusno = rptParam.ParameterValue;

            //          //  //  var Xqry = qry.Where(x => x.c_cusno == cusno);

            //          //  //  qry = Xqry;
            //          //  //}
            //          //  //else if (rptParam.ParameterName.Equals("DOID", StringComparison.OrdinalIgnoreCase) && (!string.IsNullOrEmpty(rptParam.ParameterValue)))
            //          //  //{
            //          //  //  if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
            //          //  //  {
            //          //  //    var Xqry = qry.Where(x => x.c_plno == rptParam.ParameterValue);

            //          //  //    qry = Xqry;
            //          //  //  }
            //          //  //  else
            //          //  //  {
            //          //  //    //var Xqry = qry.Between(x => x.c_plno, rptParam.ParameterValue, rptParam.BetweenValue).AsQueryable();

            //          //  //    //qry = Xqry.AsQueryable();
            //          //  //  }
            //          //  //}
            //          //}

            //          #endregion
            //        }

            //        List<LG_POH> lst = null;

            //        if (isGenerated)
            //        {
            //          lst = qry.ToList();

            //          LG_POH poh = null;
            //          DateTime date = DateTime.Now;

            //          for (int nLoop = 0; nLoop < lst.Count; nLoop++)
            //          {
            //            poh = lst[nLoop];

            //            poh.l_print = true;
            //            poh.c_update = rpt.User;
            //            poh.d_update = date;
            //          }

            //          db.SubmitChanges();

            //          lst.Clear();
            //        }

            //        break;
            //      }

            //    #endregion
            //  }
            //}
            //catch (Exception ex)
            //{
            //  tmp = string.Format("ScmsSoaLibrary.Core.Reports.Module.Transaksi:GeneratePO - {0}", ex.Message);

            //  Logger.WriteLine(tmp);
            //  Logger.WriteLine(ex.StackTrace);
            //}

            //db.Dispose();

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

    public static Functionals.ReportingGeneratorResult GenerateSuratJalan(Config cfg, ReportParser rpt, bool isAsync)
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
      string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");

      string isEmptyName = null;

      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {
          #region Pesanan Gudang

          case Constant.REPORT_TRANSAKSI_SURATJALAN_PESANANGUDANG:
            {
              rptName = "Surat Jalan - Pesanan Gudang";

              if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
              {
                #region Report

                isEmptyName = rpt.ReportParameter[4].ParameterValue.ToString();

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                if (string.IsNullOrEmpty(rptRecordSel))
                {
                  //rptRecordSel = "((if(IsNull({LG_SJH.l_delete})) then false else {LG_SJH.l_delete}) = false)";
                }
                else
                {
                  //rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_SJH.l_delete})) then false else {LG_SJH.l_delete}) = false)");
                }

                reportFiles = Path.Combine(rtpPath, "LG_SJ.rpt");

                #endregion
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Transfer Gudang

          case Constant.REPORT_TRANSAKSI_SURATJALAN_TRANSFERGUDANG:
            {
              rptName = "Surat Jalan - Transfer Gudang";

              if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
              {
                #region Report

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                isEmptyName = rpt.ReportParameter[4].ParameterValue.ToString();

                if (string.IsNullOrEmpty(rptRecordSel))
                {
                  rptRecordSel = "((if(IsNull({LG_SJH.l_delete})) then false else {LG_SJH.l_delete}) = false)";
                }
                else
                {
                  rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_SJH.l_delete})) then false else {LG_SJH.l_delete}) = false)");
                }

                reportFiles = Path.Combine(rtpPath, "LG_TransferGudang.rpt");

                #endregion
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Claim

          case Constant.REPORT_TRANSAKSI_SURATJALAN_CLAIM:
            {
              rptName = "Surat Jalan - Claim";

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

                reportFiles = Path.Combine(rtpPath, "LG_SJ");

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
          outputFolder = cfg.PathReportStorage
        };

        string tmp = null;

        if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
        {
          string isEmptyRecord = ((string.IsNullOrEmpty(reportStruct.outputName) || (reportStruct.outputName.Length <= 10)) ? isEmptyName : reportStruct.outputName);

          reportStruct.outputName = isEmptyRecord;

          if (Functionals.ReportSaveParser(rptName, rpt.ReportingID, reportStruct.outputFolder, reportStruct.outputName, reportStruct.extReport, rpt.User,
            reportStruct.sizeOutput, isAsync, rpt.UserDefinedName, rpt.IsShared, out tmp))
          {
            if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
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

            #region Update Database

            isGenerated = false;

            ORMDataContext db = new ORMDataContext(cfg.ConnectionString);

            try
            {
              switch (rpt.ReportingID)
              {
                #region Surat Jalan

                case Constant.REPORT_TRANSAKSI_SURATJALAN_PESANANGUDANG:
                case Constant.REPORT_TRANSAKSI_SURATJALAN_TRANSFERGUDANG:
                case Constant.REPORT_TRANSAKSI_SURATJALAN_CLAIM:
                  {
                    var qry = (from q in db.LG_SJHs
                               where ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                && ((q.l_print.HasValue ? q.l_print.Value : false) == false)
                               select q).AsQueryable();

                    for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
                    {
                      rptParam = rpt.ReportParameter[nLoop];

                      if (rptParam.IsLinqFilterParameter)
                      {
                        if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                        {
                          qry = qry.Where(rptParam.ParameterName, rptParam.ParameterValueObject);

                          isGenerated = true;
                        }
                        else
                        {
                          qry = qry.Between(rptParam.ParameterName, rptParam.ParameterValueObject, rptParam.BetweenValueObject);

                          isGenerated = true;
                        }
                      }
                    }

                    List<LG_SJH> lst = null;

                    if (isGenerated)
                    {
                      lst = qry.ToList();

                      LG_SJH sjh = null;
                      DateTime date = DateTime.Now;

                      for (int nLoop = 0; nLoop < lst.Count; nLoop++)
                      {
                          sjh = lst[nLoop];

                          sjh.l_print = true;
                          sjh.c_update = rpt.User;
                          if (sjh.l_status == false)
                          {
                              sjh.d_update = date;
                          }
                      }

                      db.SubmitChanges();

                      lst.Clear();
                    }

                    break;
                  }

                #endregion
              }
            }
            catch (Exception ex)
            {
              tmp = string.Format("ScmsSoaLibrary.Core.Reports.Module.Transaksi:GenerateRS - {0}", ex.Message);

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

          //Production 

          string isEmptyRecord = (string.IsNullOrEmpty(reportStruct.outputName) ? isEmptyName : reportStruct.outputName);

          reportStruct.outputName = isEmptyRecord;

          if (Functionals.ReportSaveParser(rptName, rpt.ReportingID, reportStruct.outputFolder, isEmptyRecord, reportStruct.extReport, rpt.User,
            reportStruct.sizeOutput, isAsync, rpt.UserDefinedName, rpt.IsShared, out tmp))
          {
            if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
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

            #region Update Database

            isGenerated = false;

            ORMDataContext db = new ORMDataContext(cfg.ConnectionString);

            try
            {
              switch (rpt.ReportingID)
              {
                #region Surat Jalan

                case Constant.REPORT_TRANSAKSI_SURATJALAN_PESANANGUDANG:
                case Constant.REPORT_TRANSAKSI_SURATJALAN_TRANSFERGUDANG:
                case Constant.REPORT_TRANSAKSI_SURATJALAN_CLAIM:
                  {
                    var qry = (from q in db.LG_SJHs
                               where ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                && ((q.l_print.HasValue ? q.l_print.Value : false) == false)
                               select q).AsQueryable();

                    for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
                    {
                      rptParam = rpt.ReportParameter[nLoop];

                      if (rptParam.IsLinqFilterParameter)
                      {
                        if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                        {
                          qry = qry.Where(rptParam.ParameterName, rptParam.ParameterValueObject);

                          isGenerated = true;
                        }
                        else
                        {
                          qry = qry.Between(rptParam.ParameterName, rptParam.ParameterValueObject, rptParam.BetweenValueObject);

                          isGenerated = true;
                        }
                      }
                    }

                    List<LG_SJH> lst = null;

                    if (isGenerated)
                    {
                      lst = qry.ToList();

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
                    }

                    break;
                  }

                #endregion
              }
            }
            catch (Exception ex)
            {
              tmp = string.Format("ScmsSoaLibrary.Core.Reports.Module.Transaksi:GenerateRS - {0}", ex.Message);

              Logger.WriteLine(tmp);
              Logger.WriteLine(ex.StackTrace);
            }

            db.Dispose();

            #endregion
          }

          ///////////////////////////////////////////
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

    public static Functionals.ReportingGeneratorResult GenerateFakturJual_Retur(Config cfg, ReportParser rpt, bool isAsync)
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
      string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");

      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {
          #region Faktur Jual

          case Constant.REPORT_TRANSAKSI_FAKTURJUAL:
            {
              rptName = "Faktur Jual";

              if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
              {
                #region Report

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                if (string.IsNullOrEmpty(rptRecordSel))
                {
                  rptRecordSel = "((if(IsNull({LG_vwJual.l_delete})) then false else {LG_vwJual.l_delete}) = false)";
                }
                else
                {
                    rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_vwJual.l_delete})) then false else {LG_vwJual.l_delete}) = false)");
                }

                reportFiles = Path.Combine(rtpPath, "LG_FJ2.rpt");

                #endregion
              }

              isGenerated = true;
            }
            break;

          #endregion

          #region Faktur Jual Retur
            
          case Constant.REPORT_TRANSAKSI_FAKTURJUALRETUR:
            {
              rptName = "Faktur Jual";

              if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
              {
                #region Report

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                if (string.IsNullOrEmpty(rptRecordSel))
                {
                  rptRecordSel = "((if(IsNull({LG_FJRH.l_delete})) then false else {LG_FJRH.l_delete}) = false)";
                }
                else
                {
                  rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_FJRH.l_delete})) then false else {LG_FJRH.l_delete}) = false)");
                }

                reportFiles = Path.Combine(rtpPath, "LG_FJR.rpt");

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

            #region Update Database

            isGenerated = false;

            ORMDataContext db = new ORMDataContext(cfg.ConnectionString);

            try
            {
              switch (rpt.ReportingID)
              {
                #region Faktur Jual

                case Constant.REPORT_TRANSAKSI_FAKTURJUAL:
                  {
                    var qry = (from q in db.LG_FJHs
                               where (q.l_delete.HasValue ? q.l_delete.Value : false) == false
                                && ((q.l_print.HasValue ? q.l_print.Value : false) == false)
                               select q).AsQueryable();

                    for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
                    {
                      rptParam = rpt.ReportParameter[nLoop];

                      if (rptParam.IsLinqFilterParameter)
                      {
                        if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                        {
                          qry = qry.Where(rptParam.ParameterName, rptParam.ParameterValueObject);

                          isGenerated = true;
                        }
                        else
                        {
                          qry = qry.Between(rptParam.ParameterName, rptParam.ParameterValueObject, rptParam.BetweenValueObject);

                          isGenerated = true;
                        }
                      }
                    }

                    List<LG_FJH> lst = null;

                    if (isGenerated)
                    {
                      lst = qry.ToList();

                      LG_FJH fjh = null;
                      DateTime date = DateTime.Now;

                      for (int nLoop = 0; nLoop < lst.Count; nLoop++)
                      {
                        fjh = lst[nLoop];

                        fjh.l_print = true;
                        fjh.c_update = rpt.User;
                        fjh.d_update = date;
                      }

                      db.SubmitChanges();

                      lst.Clear();
                    }

                    break;
                  }

                #endregion

                #region Faktur Jual Retur

                case Constant.REPORT_TRANSAKSI_FAKTURJUALRETUR:
                  {
                    var qry = (from q in db.LG_FJRHs
                               where (q.l_delete.HasValue ? q.l_delete.Value : false) == false
                                && ((q.l_print.HasValue ? q.l_print.Value : false) == false)
                               select q).AsQueryable();

                    for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
                    {
                      rptParam = rpt.ReportParameter[nLoop];

                      if (rptParam.IsLinqFilterParameter)
                      {
                        if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                        {
                          qry = qry.Where(rptParam.ParameterName, rptParam.ParameterValueObject);

                          isGenerated = true;
                        }
                        else
                        {
                          qry = qry.Between(rptParam.ParameterName, rptParam.ParameterValueObject, rptParam.BetweenValueObject);

                          isGenerated = true;
                        }
                      }
                    }

                    List<LG_FJRH> lst = null;

                    if (isGenerated)
                    {
                      lst = qry.ToList();

                      LG_FJRH fjrh = null;
                      DateTime date = DateTime.Now;

                      for (int nLoop = 0; nLoop < lst.Count; nLoop++)
                      {
                        fjrh = lst[nLoop];

                        fjrh.l_print = true;
                        fjrh.c_update = rpt.User;
                        fjrh.d_update = date;
                      }

                      db.SubmitChanges();

                      lst.Clear();
                    }

                    break;
                  }

                #endregion
              }
            }
            catch (Exception ex)
            {
              tmp = string.Format("ScmsSoaLibrary.Core.Reports.Module.Transaksi:GenerateFakturJual_Retur - {0}", ex.Message);

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

    public static Functionals.ReportingGeneratorResult GenerateAdjustSTT(Config cfg, ReportParser rpt, bool isAsync)
    {
      if ((rpt == null) || (cfg == null))
      {
        return new Functionals.ReportingGeneratorResult();
      }

      Functionals.ReportingGeneratorResult result = new Functionals.ReportingGeneratorResult();

      List<SqlParameter> lstParams = new List<SqlParameter>();
      string tmpQuery = null;
      bool isGenerated = false;
      string reportFiles = null;
      string rptRecordSel = null;
      string rptName = null;
      string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");

      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {
          #region Adjustment STT

          case Constant.REPORT_TRANSAKSI_ADJUSTMENTSTT:
            {
              rptName = "Adjustment STT";

              if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
              {
                #region Report

                rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                if (string.IsNullOrEmpty(rptRecordSel))
                {
                  rptRecordSel = "((if(IsNull({LG_AdjSTH.l_delete})) then false else {LG_AdjSTH.l_delete}) = false)";
                }
                else
                {
                  rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_AdjSTH.l_delete})) then false else {LG_AdjSTH.l_delete}) = false)");
                }

                reportFiles = Path.Combine(rtpPath, "LG_AdjSTT.rpt");

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

      lstParams.Clear();

      return result;
    }

    public static Functionals.ReportingGeneratorResult GenerateSuratPesanan(Config cfg, ReportParser rpt, bool isAsync)
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
      string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");

      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {
          #region Surat Pesanan

          case Constant.REPORT_TRANSAKSI_SURATPESANAN:
            {
              rptName = "Surat Pesanan";

              if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
              {
                #region Report

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

                if (tipereport == "01")
                {
                    rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                    if (string.IsNullOrEmpty(rptRecordSel))
                    {
                        rptRecordSel = "((if(IsNull({LG_SPH.l_delete})) then false else {LG_SPH.l_delete}) = false)";
                    }
                    else
                    {
                        rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_SPH.l_delete})) then false else {LG_SPH.l_delete}) = false)");
                    }

                    reportFiles = Path.Combine(rtpPath, "LG_SPCFull.rpt");
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

                    rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                    {
                        bool isOk = false;

                        if (x.ParameterName.Equals("cabang", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                        {
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

                    reportFiles = Path.Combine(rtpPath, "LG_SPCFullGrid.rpt");
                    tmpQuery = "Exec SP_SURATPESANAN_GRID @date1,@date2, @cabang, @user";
                }

                #endregion
              }

              isGenerated = true;
            }
            break;

          #endregion

                ////
          #region Proses Pharmanet

          case Constant.REPORT_TRANSAKSI_PROSESPHARMANET:
            {
                rptName = "Proses Pharmanet";

              

                if (rpt.ReportParameter != null)
                {
                    if (rpt.ReportParameter.Length > 0)
                    {
                        #region Sql
                        

                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("custId", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                            {
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

                            if (x.ParameterName.Equals("SP1", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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
                                Size = 15,
                                Value = rptParam.ParameterValue
                            });
                        }



                        rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                        {
                            bool isOk = false;

                            if (x.ParameterName.Equals("SP2", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
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
                                
                                Size = 15,
                                Value = rptParam.ParameterValue
                            });
                        }

                        rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                        tmpQuery = "Exec LG_ProsesPrintPharmanet @custId,@SP1,@SP2";


                        #endregion

                        #region Report

                        

                        reportFiles = Path.Combine(rtpPath, "LG_ProsesPharmanet_New2.rpt");
                        #endregion
                    }
                }


                ////




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

            #region Update Database

            isGenerated = false;

            ORMDataContext db = new ORMDataContext(cfg.ConnectionString);

            try
            {
              switch (rpt.ReportingID)
              {
                #region Surat Pesanan

                  case Constant.REPORT_TRANSAKSI_PROSESPHARMANET:
                  {
                      var qry = (from q in db.Temp_LG_DOPHs
                                 join q2 in db.LG_CusmasCabs on q.c_cab equals q2.c_cab into q_6
                                 from qSPG in q_6.DefaultIfEmpty()
                               //where q.c_po_outlet == tmp
                               select q).AsQueryable();

                                 //select new
                                 //{

                                 //    q.c_nosup,
                                 //    q.c_dono,
                                 //    q.d_dodate,
                                 //    q.d_fjno,
                                 //    q.d_fjdate,
                                 //    q.l_status,
                                 //    c_cab = qSPG.c_cusno,
                                 //    //q.c_cab,
                                 //    q.c_via,
                                 //    q.c_taxno,
                                 //    q.d_entry,
                                 //    q.c_po_outlet,
                                 //    q.c_outlet,
                                 //    q.v_outlet,
                                 //    q.c_plphar,
                                 //    q.c_type,
                                 //    q.Status,
                                 //    q.v_ket,
                                 //    q.nv_batchterima,
                                 //    q.c_spphar,


                                 //}).AsQueryable();


                     var uuu = qry.ToList();

                    for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
                    {
                      rptParam = rpt.ReportParameter[nLoop];

                      if (rptParam.IsLinqFilterParameter)
                      {
                        if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                        {
                          qry = qry.Where(rptParam.ParameterName, rptParam.ParameterValueObject);
                          
                          isGenerated = true;
                        }
                        else
                        {
                          qry = qry.Between(rptParam.ParameterName, rptParam.ParameterValueObject, rptParam.BetweenValueObject);

                          isGenerated = true;
                        }
                      }
                    }

                    //List<Temp_LG_DOPH> lst = null;

                    //if (isGenerated)
                    //{
                    //    //lst = qry.ToList();

                    //    Temp_LG_DOPH sph = null;
                    //    DateTime date = DateTime.Now;

                    //    for (int nLoop = 0; nLoop < lst.Count; nLoop++)
                    //    {
                    //        sph = lst[nLoop];

                    //        //sph.l_print = true;
                    //        //sph.c_update = rpt.User;
                    //        //sph.d_update = date;
                    //    }

                    //    db.SubmitChanges();

                    //    lst.Clear();
                    //}

                    break;
                  }

                #endregion
              }
            }
            catch (Exception ex)
            {
              tmp = string.Format("ScmsSoaLibrary.Core.Reports.Module.Transaksi:GenerateSuratPesanan - {0}", ex.Message);

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

    public static Functionals.ReportingGeneratorResult GenerateProsesWaktuPelayanan(Config cfg, ReportParser rpt, bool isAsync)
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

            if (rpt != null)
            {
                switch (rpt.ReportingID)
                {
                    #region Surat Pesanan

                    case Constant.PROSES_WAKTUPELAYANAN:
                        {
                            if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
                            {
                                #region Report

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

                                    if (x.ParameterName.Equals("tipeProses", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                                    {
                                        isOk = true;
                                    }

                                    return isOk;
                                });

                                string tipeproses = rptParam.ParameterValue;

                                switch (tipeproses)
                                {
                                    case "1":
                                        tmpQuery = "Exec LG_ProcessWP_New @tahun, @bulan";
                                        break;
                                    case "2":
                                        tmpQuery = "Exec LG_ProcessWP_ST @tahun, @bulan";
                                        break;
                                    case "3":
                                        tmpQuery = "Exec LG_ProcessWP_GR @tahun, @bulan";
                                        break;
                                }


                                #endregion
                            }

                            isGenerated = true;
                        }
                        break;

                    #endregion

                        /// FJR

                    #region Proses FJR

                    case Constant.PROSES_FJR:
                        {
                            if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
                            {
                                #region Report

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


                                tmpQuery = "Exec LG_CalcProses_FJR @TAHUN, @BULAN";
                               


                                #endregion
                            }

                            isGenerated = true;
                        }
                        break;

                    #endregion

                        /// FJR end
                        /// 

                     ///Proses Discount Cliam Awal

                    #region Proses Discount Claim

                    case Constant.PROSES_DISC_CLAIM:
                        {
                            if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
                            {
                                #region Report

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


                                tmpQuery = "Exec LG_CalcProses_Discount @Tahun, @bulan, @user";



                                #endregion
                            }

                            isGenerated = true;
                        }
                        break;

                    #endregion

                    /// Proses Discount Cliam Akhir

                }
            }

            if (isGenerated)
            {
                if (!string.IsNullOrEmpty(tmpQuery))
                {
                    Functionals.ExecProcedures(cfg, tmpQuery, lstParams.ToArray());
                }
            }
            result = new Functionals.ReportingGeneratorResult()
            {
                IsSet = true,
                IsSuccess = true,
                Messages = "Success",
            };

            lstParams.Clear();
            return result;
        }

    public static Functionals.ReportingGeneratorResult GenerateProsesCompareSo(Config cfg, ReportParser rpt, bool isAsync)
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
        string rtpPath = string.Concat(cfg.PathReport, @"Konsolidasi\");

        if (rpt != null)
        {
            rptName = "Stok Opname";

            switch (rpt.ReportingID)
            {
                #region Compare SO

                case Constant.PROSES_COMPARE_SO:
                    {
                        if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
                        {
                            #region Report

                            rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

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

                                if (x.ParameterName.Equals("tipeProses", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                                {
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
                                    Size = 1,
                                    Value = rptParam.ParameterValue
                                });
                            }

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
                                    Value = rptParam.ParameterRawValue<DateTime>(Functionals.StandardSqlDateTime)
                                });
                            }

                            tmpQuery = "Exec LG_ProcessCompare_SO @tahun, @bulan, @tipeProses, @gudang, @date";

                            reportFiles = Path.Combine(rtpPath, "LG_CompareSO.rpt");
   
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
                outputFolder = cfg.PathReportStorage
            };

            string tmp = null;

            if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
            {
                if (Functionals.ReportSaveParser(rptName, rpt.ReportingID, reportStruct.outputFolder, reportStruct.outputName, reportStruct.extReport, rpt.User,
                  reportStruct.sizeOutput, isAsync, rpt.UserDefinedName, rpt.IsShared, out tmp))
                {
                    if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
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
        //result = new Functionals.ReportingGeneratorResult()
        //{
        //    IsSet = true,
        //    IsSuccess = true,
        //    Messages = "Success",
        //};

        lstParams.Clear();
        return result;
    }

    public static Functionals.ReportingGeneratorResult GenerateProsesSo(Config cfg, ReportParser rpt, bool isAsync)
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

        if (rpt != null)
        {
            switch (rpt.ReportingID)
            {
                #region Generate SO

                case Constant.PROSES_GENERATE_SO:
                    {
                        if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
                        {
                            #region Report

                            rptParam = Array.Find<ReportParameter>(rpt.ReportParameter, delegate(ReportParameter x)
                            {
                                bool isOk = false;

                                if (x.ParameterName.Equals("Tahun", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                                {
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

                                if (x.ParameterName.Equals("Bulan", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                                {
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
                                    Size = 1,
                                    Value = rptParam.ParameterValue
                                });
                            }
                            tmpQuery = "Exec LG_ProcessSO @Tahun, @Bulan, @Date1, @Date2, @Gudang";

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
        }
        result = new Functionals.ReportingGeneratorResult()
        {
            IsSet = true,
            IsSuccess = true,
            Messages = "Success",
        };

        lstParams.Clear();
        return result;
    }

    public static Functionals.ReportingGeneratorResult GenerateClaimBonus(Config cfg, ReportParser rpt, bool isAsync)
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

      if (rpt != null)
      {
        switch (rpt.ReportingID)
        {
          #region Surat Pesanan

          case Constant.REPORT_TRANSAKSI_CLAIM_BONUS:
            {
              if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
              {
                #region Report

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

                  if (x.ParameterName.Equals("tipeProses", StringComparison.OrdinalIgnoreCase) && x.IsSqlParameter)
                  {
                    isOk = true;
                  }

                  return isOk;
                });

                string tipeproses = rptParam.ParameterValue;

                switch (tipeproses)
                {
                  case "1":
                    tmpQuery = "Exec LG_ProcessWP_New @tahun, @bulan";
                    break;
                  case "2":
                    tmpQuery = "Exec LG_ProcessWP_ST @tahun, @bulan";
                    break;
                }


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
      }
      result = new Functionals.ReportingGeneratorResult()
      {
        IsSet = true,
        IsSuccess = true,
        Messages = "Success",
      };

      lstParams.Clear();
      return result;
    }

    public static Functionals.ReportingGeneratorResult GeneratePemusnahan(Config cfg, ReportParser rpt, bool isAsync)
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
        string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");

        if (rpt != null)
        {
            switch (rpt.ReportingID)
            {
                #region Transaksi Pemusnahan

                case Constant.REPORT_TRANSAKSI_PEMUSNAHAN:
                    {
                        rptName = "Pemusnahan Barang";

                        if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
                        {
                            #region Report

                            rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                            if (string.IsNullOrEmpty(rptRecordSel))
                            {
                                rptRecordSel = "((if(IsNull({LG_PMH.l_delete})) then false else {LG_PMH.l_delete}) = false)";
                            }
                            else
                            {
                                rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({LG_PMH.l_delete})) then false else {LG_PMH.l_delete}) = false)");
                            }

                            reportFiles = Path.Combine(rtpPath, "LG_PM.rpt");

                            #endregion
                        }

                        isGenerated = true;
                    }
                    break;

                #endregion

                #region Memo Pemusnahan

                case Constant.REPORT_MEMO_PEMUSNAHAN:
                    {
                        rptName = "Memo Pemusnahan Barang";

                        if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
                        {
                            #region Report

                            rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                            if (string.IsNullOrEmpty(rptRecordSel))
                            {
                                rptRecordSel = "((if(IsNull({MK_MPH.l_delete})) then false else {MK_MPH.l_delete}) = false)";
                            }
                            else
                            {
                                rptRecordSel = string.Concat(rptRecordSel, " AND ", "((if(IsNull({MK_MPH.l_delete})) then false else {MK_MPH.l_delete}) = false)");
                            }

                            reportFiles = Path.Combine(rtpPath, "MK_MP.rpt");

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

                    #region Update Database

                    ORMDataContext db = new ORMDataContext(cfg.ConnectionString);

                    try
                    {
                        switch (rpt.ReportingID)
                        {
                            #region Transaksi Pemusnahan

                            case Constant.REPORT_TRANSAKSI_PEMUSNAHAN:
                                {
                                    var qry = (from q in db.LG_PMHs
                                               where ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                                && ((q.l_print.HasValue ? q.l_print.Value : false) == false)
                                               select q).AsQueryable();

                                    for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
                                    {
                                        rptParam = rpt.ReportParameter[nLoop];

                                        if (rptParam.IsLinqFilterParameter)
                                        {
                                            if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                                            {
                                                qry = qry.Where(rptParam.ParameterName, rptParam.ParameterValueObject);

                                                isGenerated = true;
                                            }
                                            else
                                            {
                                                qry = qry.Between(rptParam.ParameterName, rptParam.ParameterValueObject, rptParam.BetweenValueObject);

                                                isGenerated = true;
                                            }
                                        }
                                    }

                                    List<LG_PMH> lst = qry.ToList();

                                    if (isGenerated)
                                    {
                                        lst = qry.ToList();

                                        LG_PMH pmh = null;
                                        DateTime date = DateTime.Now;

                                        for (int nLoop = 0; nLoop < lst.Count; nLoop++)
                                        {
                                            pmh = lst[nLoop];

                                            pmh.l_print = true;
                                            pmh.c_update = rpt.User;
                                            pmh.d_update = date;
                                        }

                                        db.SubmitChanges();

                                        lst.Clear();
                                    }

                                    break;
                                }

                            #endregion

                            #region Memo Pemusnahan

                            case Constant.REPORT_MEMO_PEMUSNAHAN:
                                {
                                    var qry = (from q in db.MK_MPHs
                                               where ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                                && ((q.l_print.HasValue ? q.l_print.Value : false) == false)
                                               select q).AsQueryable();

                                    for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
                                    {
                                        rptParam = rpt.ReportParameter[nLoop];

                                        if (rptParam.IsLinqFilterParameter)
                                        {
                                            if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                                            {
                                                qry = qry.Where(rptParam.ParameterName, rptParam.ParameterValueObject);

                                                isGenerated = true;
                                            }
                                            else
                                            {
                                                qry = qry.Between(rptParam.ParameterName, rptParam.ParameterValueObject, rptParam.BetweenValueObject);

                                                isGenerated = true;
                                            }
                                        }
                                    }

                                    List<MK_MPH> lst = qry.ToList();

                                    if (isGenerated)
                                    {
                                        lst = qry.ToList();

                                        MK_MPH mph = null;
                                        DateTime date = DateTime.Now;

                                        for (int nLoop = 0; nLoop < lst.Count; nLoop++)
                                        {
                                            mph = lst[nLoop];

                                            mph.l_print = true;
                                            mph.c_update = rpt.User;
                                            mph.d_update = date;
                                        }

                                        db.SubmitChanges();

                                        lst.Clear();
                                    }

                                    break;
                                }

                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        tmp = string.Format("ScmsSoaLibrary.Core.Reports.Module.Transaksi:GenerateSTT - {0}", ex.Message);

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

    public static Functionals.ReportingGeneratorResult GenerateBASPBSJ(Config cfg, ReportParser rpt, bool isAsync)
    {
        if ((rpt == null) || (cfg == null))
        {
            return new Functionals.ReportingGeneratorResult();
        }

        Functionals.ReportingGeneratorResult result = new Functionals.ReportingGeneratorResult();

        List<SqlParameter> lstParams = new List<SqlParameter>();
        string tmpQuery = null;
        bool isGenerated = false;
        string reportFiles = null;
        string rptRecordSel = null;
        string rptName = null;
        string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");

        if (rpt != null)
        {
            switch (rpt.ReportingID)
            {
                #region Transaksi BASPB SJ

                case Constant.REPORT_MEMO_BASPB_SJ:
                    {
                        rptName = "Memo BASPB SJ";

                        if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
                        {
                            #region Report

                            rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                            reportFiles = Path.Combine(rtpPath, "LG_BASPB_SJ.rpt");

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

        lstParams.Clear();

        return result;
    }

    public static Functionals.ReportingGeneratorResult GenerateSerahTerimaTiket(Config cfg, ReportParser rpt, bool isAsync)
    {
        if ((rpt == null) || (cfg == null))
        {
            return new Functionals.ReportingGeneratorResult();
        }

        Functionals.ReportingGeneratorResult result = new Functionals.ReportingGeneratorResult();

        List<SqlParameter> lstParams = new List<SqlParameter>();
        string tmpQuery = null;
        bool isGenerated = false;
        string reportFiles = null;
        string rptRecordSel = null;
        string rptName = null;
        string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");
        ReportParameter rptParam = null;
        string sPathPrintLt1 = cfg.PathPrintStorageLat1;
        string sPathPrintLt2 = cfg.PathPrintStorageLat2;

        if (rpt != null)
        {
            switch (rpt.ReportingID)
            {
                #region Transaksi Serah Terima Tiket

                case Constant.REPORT_WAKTUPELAYANAN_SERAHTERIMA_TIKET:
                    {
                        rptName = "Nomor Serah Terima Tiket";

                        if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
                        {
                            #region Report

                            rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                            reportFiles = Path.Combine(rtpPath, "LG_SerahTerimaTiket.rpt");

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

            string ltPrinter = null;
            foreach (String printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                if (printer == sPathPrintLt1)
                {
                    ltPrinter = printer;
                    RawPrinterHelper.SetDefaultPrinter(printer);
                    break;
                }
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
                    if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
                    {

//Indra 20170622
                        //string sPath = string.Concat(reportStruct.outputFolder, @"\", reportStruct.outputName, @".pdf");
                        //string sPathDoc = string.Concat(reportStruct.outputFolder, @"\", reportStruct.outputName);
                        //File.Move(sPathDoc, Path.ChangeExtension(sPathDoc, ".pdf"));

                        ////System.Drawing.Printing.PrinterSettings ps = new System.Drawing.Printing.PrinterSettings();
                        
                        //if (!string.IsNullOrEmpty(ltPrinter))
                        //{
                        //    //Process printProcess = new Process();


                        //    //string ExternalpdfParser = @"C:\Program Files\Adobe\Reader 9.0\Reader\AcroRd32.exe";
                        //    //string param = string.Format(@"-t ""{0}"" {1}", sPath, ltPrinter);
                        //    //using (System.Diagnostics.Process proc = System.Diagnostics.Process.Start(ExternalpdfParser, param))
                        //    //{
                        //    //    proc.WaitForExit((int)TimeSpan.FromSeconds(2).TotalMilliseconds);
                        //    //}

                        //    //printProcess.StartInfo.FileName = @"C:\Program Files\Adobe\Reader 9.0\Reader\AcroRd32.exe";
                        //    ////Also tried usecellexcecution=false;
                        //    ////Redirect=true; something like this
                        //    //string args = String.Format(@"/p /h {0}", sPath);
                        //    ////ProcessStartInfo psInfo = new ProcessStartInfo(@"C:\Program Files\Adobe\Reader 9.0\Reader\AcroRd32.exe");
                        //    ////psInfo.UseShellExecute = true;
                        //    //printProcess.StartInfo.UseShellExecute = true;
                        //    ////printProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        //    //printProcess.StartInfo.Verb = "PrintTo";
                        //    //printProcess.StartInfo.Arguments = args;
                        //    //printProcess.StartInfo.CreateNoWindow = true;
                        //    //printProcess.StartInfo.ErrorDialog = true;
                        //    //printProcess.Start();

                        //    Process proc1 = new Process();
                        //    proc1.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        //    proc1.StartInfo.Verb = "print";
                        //    //string filePath = Server.MapPath(@"~/" + id + "test.pdf");
                        //    proc1.StartInfo.FileName = @"C:\Program Files\Adobe\Reader 11.0\Reader\AcroRd32.exe";
                        //    //proc1.StartInfo.FileName = @"C:\Program Files\Adobe\Reader 9.0\Reader\AcroRd32.exe";
                        //    proc1.StartInfo.Arguments = @"/p /h " + sPath;
                        //    proc1.StartInfo.UseShellExecute = false;
                        //    proc1.StartInfo.CreateNoWindow = true;
                        //    proc1.Start();
                        //    proc1.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        //    //if (proc1.HasExited == false)
                        //    //{
                        //    //    proc1.WaitForExit(1000);
                        //    //    proc1.Kill();
                        //    //}

                        //    if (!proc1.WaitForExit(3000))
                        //    {
                        //        if (!proc1.HasExited)
                        //        {
                        //            proc1.Kill();
                        //        }
                        //    }
                        //    proc1.EnableRaisingEvents = true;
                        //    // AcroRd32.exe
                        //    proc1.CloseMainWindow();
                        //    proc1.Close();

                        //    //printProcess.StartInfo.FileName = sPath;
                        //    ////Also tried usecellexcecution=false;
                        //    ////Redirect=true; something like this
                        //    //printProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        //    //printProcess.StartInfo.Verb = "Print";
                        //    //printProcess.StartInfo.Arguments = ltPrinter;

                        //    //printProcess.Start();
                        //}
//End Mark 20170622
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
                            var qry = (from q in db.SCMS_WPHs
                                       where ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                        && ((q.l_print.HasValue ? q.l_print.Value : false) == false)
                                       select q).AsQueryable();

                            for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
                            {
                                rptParam = rpt.ReportParameter[nLoop];

                                if (rptParam.IsLinqFilterParameter)
                                {
                                    if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                                    {
                                        qry = qry.Where(rptParam.ParameterName, rptParam.ParameterValueObject);

                                        isGenerated = true;
                                    }
                                    else
                                    {
                                        qry = qry.Between(rptParam.ParameterName, rptParam.ParameterValueObject, rptParam.BetweenValueObject);

                                        isGenerated = true;
                                    }
                                }
                            }

                            List<SCMS_WPH> lst = qry.ToList();

                            if (isGenerated)
                            {
                                lst = qry.ToList();

                                SCMS_WPH wph = null;
                                DateTime date = DateTime.Now;

                                for (int nLoop = 0; nLoop < lst.Count; nLoop++)
                                {
                                    wph = lst[nLoop];

                                    wph.l_print = true;
                                }

                                db.SubmitChanges();

                                lst.Clear();
                            }
                        }
                        catch (Exception ex)
                        {
                            tmp = string.Format("ScmsSoaLibrary.Core.Reports.Module.Transaksi:GenerateSerahTerimaTiket - {0}", ex.Message);

                            Logger.WriteLine(tmp);
                            Logger.WriteLine(ex.StackTrace);
                        }

                        db.Dispose();

                        #endregion
                    }
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

        foreach (String printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
        {
            if (printer == sPathPrintLt2)
            {
                RawPrinterHelper.SetDefaultPrinter(printer);
                break;
            }
        }

        lstParams.Clear();

        return result;
    }

    public static Functionals.ReportingGeneratorResult GenerateSerahTerimaPBBR(Config cfg, ReportParser rpt, bool isAsync)
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
        string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");

        if (rpt != null)
        {
            switch (rpt.ReportingID)
            {
                #region Serah terima Gudang PBB/R

                case Constant.REPORT_PROSES_SERAHTERIMA:
                    {
                        rptName = "Serah Terima";

                        if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
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

                            //reportFiles = Path.Combine(rtpPath, "LG_SerahTerimaPBB.rpt");

                            
                            if (tipereport == "05")
                            {
                                reportFiles = Path.Combine(rtpPath, "LG_SerahTerimaPBB.rpt");
                            }
                            else  if (tipereport == "06")
                            {
                                reportFiles = Path.Combine(rtpPath, "LG_SerahTerimaPBB_Admin.rpt");
                            }

                            tmpQuery = "EXEC LG_PrintSerahTerima_Admin_PBB_R @date1, @date2, @cusno, @Type";
                                                                      

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
                outputFolder = cfg.PathReportStorage
            };

            string tmp = null;

            if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
            {
                if (Functionals.ReportSaveParser(rptName, rpt.ReportingID, reportStruct.outputFolder, reportStruct.outputName, reportStruct.extReport, rpt.User,
                  reportStruct.sizeOutput, isAsync, rpt.UserDefinedName, rpt.IsShared, out tmp))
                {
                    if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
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

                        isGenerated = false;

                        ORMDataContext db = new ORMDataContext(cfg.ConnectionString);

                        try
                        {
                            switch (rpt.ReportingID)
                            {
                                #region RS

                                case Constant.REPORT_TRANSAKSI_RSBELI:
                                case Constant.REPORT_TRANSAKSI_RSBELI_CONF:
                                case Constant.REPORT_TRANSAKSI_RSREPACK:
                                    {
                                        var qry = (from q in db.LG_RSHes
                                                   where ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                                    && ((q.l_print.HasValue ? q.l_print.Value : false) == false)
                                                   select q).AsQueryable();

                                        for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
                                        {
                                            rptParam = rpt.ReportParameter[nLoop];

                                            if (rptParam.IsLinqFilterParameter)
                                            {
                                                if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                                                {
                                                    qry = qry.Where(rptParam.ParameterName, rptParam.ParameterValueObject);

                                                    isGenerated = true;
                                                }
                                                else
                                                {
                                                    qry = qry.Between(rptParam.ParameterName, rptParam.ParameterValueObject, rptParam.BetweenValueObject);

                                                    isGenerated = true;
                                                }
                                            }
                                        }

                                        List<LG_RSH> lst = null;

                                        if (isGenerated)
                                        {
                                            lst = qry.ToList();

                                            LG_RSH rsh = null;
                                            DateTime date = DateTime.Now;

                                            for (int nLoop = 0; nLoop < lst.Count; nLoop++)
                                            {
                                                rsh = lst[nLoop];

                                                rsh.l_print = true;
                                                rsh.c_update = rpt.User;
                                                rsh.d_update = date;
                                            }

                                            db.SubmitChanges();

                                            lst.Clear();
                                        }

                                        break;
                                    }

                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            tmp = string.Format("ScmsSoaLibrary.Core.Reports.Module.Transaksi:GenerateRS - {0}", ex.Message);

                            Logger.WriteLine(tmp);
                            Logger.WriteLine(ex.StackTrace);
                        }

                        db.Dispose();

                        #endregion
                    }
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

    public static Functionals.ReportingGeneratorResult GenerateMovementStock(Config cfg, ReportParser rpt, bool isAsync)
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
        string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");
        string sPathPrint = null;

        if (rpt != null)
        {
            switch (rpt.ReportingID)
            {
                #region Packing List

                case Constant.REPORT_TRANSAKSI_MOVEMENT_STOCK:
                    {
                        rptName = "Movement Stock";

                        if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
                        {
                            #region Report

                            rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                            reportFiles = Path.Combine(rtpPath, "MovementStock.rpt");

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
                outputFolder = cfg.PathReportStorage
            };

            string tmp = null;


            if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
            {
                if (Functionals.ReportSaveParser(rptName, rpt.ReportingID, reportStruct.outputFolder, reportStruct.outputName, reportStruct.extReport, rpt.User,
                  reportStruct.sizeOutput, isAsync, rpt.UserDefinedName, rpt.IsShared, out tmp))
                {
                    if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
                    {
                        System.Drawing.Printing.PrinterSettings ps = new System.Drawing.Printing.PrinterSettings();
                        foreach (String printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                        {
                            if (printer == sPathPrint)
                            {
                                string sPath = string.Concat(reportStruct.outputFolder, @"\", reportStruct.outputName, @".pdf");
                                string sPathDoc = string.Concat(reportStruct.outputFolder, @"\", reportStruct.outputName);
                                File.Move(sPathDoc, Path.ChangeExtension(sPathDoc, ".pdf"));


                                Process printProcess = new Process();

                                printProcess.StartInfo.FileName = sPath;
                                //Also tried usecellexcecution=false;
                                //Redirect=true; something like this
                                printProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                printProcess.StartInfo.Verb = "PrintTo";
                                printProcess.StartInfo.Arguments = printer;
                                printProcess.StartInfo.CreateNoWindow = true;
                                printProcess.Start();

                            }
                        }

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

                        isGenerated = false;

                        ORMDataContext db = new ORMDataContext(cfg.ConnectionString);

                        try
                        {
                            switch (rpt.ReportingID)
                            {
                                #region Packing List

                                case Constant.REPORT_TRANSAKSI_PACKINGLIST:
                                    {
                                        var qry = (from q in db.LG_PLHs
                                                   where (q.l_delete.HasValue ? q.l_delete.Value : false) == false
                                                    && ((q.l_print.HasValue ? q.l_print.Value : false) == false)
                                                   select q).AsQueryable();

                                        //var qryTmp = (from q in rpt.ReportParameter
                                        //              where q.IsSqlParameter == true
                                        //              select q);

                                        for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
                                        {
                                            rptParam = rpt.ReportParameter[nLoop];

                                            if (rptParam.IsLinqFilterParameter)
                                            {
                                                if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                                                {
                                                    qry = qry.Where(rptParam.ParameterName, rptParam.ParameterValueObject);

                                                    isGenerated = true;
                                                }
                                                else
                                                {
                                                    qry = qry.Between(rptParam.ParameterName, rptParam.ParameterValueObject, rptParam.BetweenValueObject);

                                                    isGenerated = true;
                                                }
                                            }
                                        }

                                        List<LG_PLH> lst = null;

                                        if (isGenerated)
                                        {
                                            lst = qry.ToList();

                                            LG_PLH plh = null;
                                            DateTime date = DateTime.Now;

                                            for (int nLoop = 0; nLoop < lst.Count; nLoop++)
                                            {
                                                plh = lst[nLoop];

                                                plh.l_confirm = true;
                                                plh.l_print = true;

                                                plh.c_update = rpt.User;
                                                plh.d_update = date;
                                            }

                                            db.SubmitChanges();

                                            lst.Clear();
                                        }

                                        break;
                                    }

                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            tmp = string.Format("ScmsSoaLibrary.Core.Reports.Module.Transaksi:GeneratePackingList - {0}", ex.Message);

                            Logger.WriteLine(tmp);
                            Logger.WriteLine(ex.StackTrace);
                        }

                        db.Dispose();

                        #endregion
                    }
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

    public static Functionals.ReportingGeneratorResult GenerateSerahTerimaRetur(Config cfg, ReportParser rpt, bool isAsync)
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
        string rtpPath = string.Concat(cfg.PathReport, @"Transaksi\");
        string sPathPrint = null;

        if (rpt != null)
        {
            switch (rpt.ReportingID)
            {
                #region Packing List

                case Constant.REPORT_SERAH_TERIMA_RETUR:
                    {
                        rptName = "Report Serah Terima Retur";

                        if ((rpt.ReportParameter != null) && (rpt.ReportParameter.Length > 0))
                        {
                            #region Report

                            rptRecordSel = Functionals.ReportParameterBuilder(rpt.ReportParameter);

                            reportFiles = Path.Combine(rtpPath, "LG_SerahTerima.rpt");

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
                outputFolder = cfg.PathReportStorage
            };

            string tmp = null;


            if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
            {
                if (Functionals.ReportSaveParser(rptName, rpt.ReportingID, reportStruct.outputFolder, reportStruct.outputName, reportStruct.extReport, rpt.User,
                  reportStruct.sizeOutput, isAsync, rpt.UserDefinedName, rpt.IsShared, out tmp))
                {
                    if (Functionals.RebindReport(cfg, rpt.User, ref reportStruct, rpt.OutputReport))
                    {
                        System.Drawing.Printing.PrinterSettings ps = new System.Drawing.Printing.PrinterSettings();
                        foreach (String printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                        {
                            if (printer == sPathPrint)
                            {
                                string sPath = string.Concat(reportStruct.outputFolder, @"\", reportStruct.outputName, @".pdf");
                                string sPathDoc = string.Concat(reportStruct.outputFolder, @"\", reportStruct.outputName);
                                File.Move(sPathDoc, Path.ChangeExtension(sPathDoc, ".pdf"));


                                Process printProcess = new Process();

                                printProcess.StartInfo.FileName = sPath;
                                //Also tried usecellexcecution=false;
                                //Redirect=true; something like this
                                printProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                printProcess.StartInfo.Verb = "PrintTo";
                                printProcess.StartInfo.Arguments = printer;
                                printProcess.StartInfo.CreateNoWindow = true;
                                printProcess.Start();

                            }
                        }

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

                        isGenerated = false;

                        ORMDataContext db = new ORMDataContext(cfg.ConnectionString);

                        try
                        {
                            switch (rpt.ReportingID)
                            {
                                #region Packing List

                                case Constant.REPORT_TRANSAKSI_PACKINGLIST:
                                    {
                                        var qry = (from q in db.LG_PLHs
                                                   where (q.l_delete.HasValue ? q.l_delete.Value : false) == false
                                                    && ((q.l_print.HasValue ? q.l_print.Value : false) == false)
                                                   select q).AsQueryable();

                                        //var qryTmp = (from q in rpt.ReportParameter
                                        //              where q.IsSqlParameter == true
                                        //              select q);

                                        for (int nLoop = 0; nLoop < rpt.ReportParameter.Length; nLoop++)
                                        {
                                            rptParam = rpt.ReportParameter[nLoop];

                                            if (rptParam.IsLinqFilterParameter)
                                            {
                                                if (string.IsNullOrEmpty(rptParam.BetweenValue) || rptParam.ParameterValue.Equals(rptParam.BetweenValue, StringComparison.OrdinalIgnoreCase))
                                                {
                                                    qry = qry.Where(rptParam.ParameterName, rptParam.ParameterValueObject);

                                                    isGenerated = true;
                                                }
                                                else
                                                {
                                                    qry = qry.Between(rptParam.ParameterName, rptParam.ParameterValueObject, rptParam.BetweenValueObject);

                                                    isGenerated = true;
                                                }
                                            }
                                        }

                                        List<LG_PLH> lst = null;

                                        if (isGenerated)
                                        {
                                            lst = qry.ToList();

                                            LG_PLH plh = null;
                                            DateTime date = DateTime.Now;

                                            for (int nLoop = 0; nLoop < lst.Count; nLoop++)
                                            {
                                                plh = lst[nLoop];

                                                plh.l_confirm = true;
                                                plh.l_print = true;

                                                plh.c_update = rpt.User;
                                                plh.d_update = date;
                                            }

                                            db.SubmitChanges();

                                            lst.Clear();
                                        }

                                        break;
                                    }

                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            tmp = string.Format("ScmsSoaLibrary.Core.Reports.Module.Transaksi:GeneratePackingList - {0}", ex.Message);

                            Logger.WriteLine(tmp);
                            Logger.WriteLine(ex.StackTrace);
                        }

                        db.Dispose();

                        #endregion
                    }
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