using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsSoaLibrary.Commons;
using System.ServiceModel.Channels;
using ScmsModel;
using ScmsModel.Core;
using ScmsSoaLibraryInterface.Commons;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Data.OleDb;
using ScmsSoaLibrary.Bussiness;

namespace ScmsMailLibrary
{
    public class SPCabangReaderBackup
    {
        private RegisteredWaitHandle handle = null;
        private AutoResetEvent ev = null;
        private Pop3MailerReader pop3;


        private ScmsSoaLibrary.Commons.Config config;

        public void timing()
        {
            ScmsMailLibrary.Pop3MailerReader.TaskInfo ti = new ScmsMailLibrary.Pop3MailerReader.TaskInfo();

            //TimeSpan timeSpan = TimeSpan.FromSeconds(double.Parse(config.TimerPeriodicMailer.));
            ev = new AutoResetEvent(false);

            handle = ThreadPool.RegisterWaitForSingleObject(ev,
              new WaitOrTimerCallback(Reader), ti,
              config.TimerPeriodicMailer, false);

        }

        private void Reader(object state, bool timeOut)
        {
            spReader();
            spPharReader();
        }

        //static string GenerateNumbering<T>(ORMDataContext db, string headerCode, char portalKode, string tipeKode, DateTime tanggalAktif, string fieldCondition)
        //{
        //    string result = string.Empty;

        //    //db.GetTable<T>().Where(


        //    int nCount = 0,
        //      nValue = 0,
        //      nCountTambah = 0;
        //    string tmpNum = null,
        //      hdrValue = null,
        //      tVal = null;
        //    char chr = char.MinValue;

        //    SysNo sysNum = (from q in db.SysNos
        //                    where q.c_portal == portalKode && q.c_type == tipeKode
        //                      && q.s_tahun == tanggalAktif.Year
        //                    select q).SingleOrDefault();

        //    if (sysNum != null)
        //    {
        //        switch (tanggalAktif.Month)
        //        {
        //            case 1: tmpNum = (sysNum.c_bln01 ?? string.Empty); break;
        //            case 2: tmpNum = (sysNum.c_bln02 ?? string.Empty); break;
        //            case 3: tmpNum = (sysNum.c_bln03 ?? string.Empty); break;
        //            case 4: tmpNum = (sysNum.c_bln04 ?? string.Empty); break;
        //            case 5: tmpNum = (sysNum.c_bln05 ?? string.Empty); break;
        //            case 6: tmpNum = (sysNum.c_bln06 ?? string.Empty); break;
        //            case 7: tmpNum = (sysNum.c_bln07 ?? string.Empty); break;
        //            case 8: tmpNum = (sysNum.c_bln08 ?? string.Empty); break;
        //            case 9: tmpNum = (sysNum.c_bln09 ?? string.Empty); break;
        //            case 10: tmpNum = (sysNum.c_bln10 ?? string.Empty); break;
        //            case 11: tmpNum = (sysNum.c_bln11 ?? string.Empty); break;
        //            case 12: tmpNum = (sysNum.c_bln12 ?? string.Empty); break;
        //            default: tmpNum = null; break;
        //        }

        //        if (!string.IsNullOrEmpty(tmpNum))
        //        {
        //            tmpNum = (string.IsNullOrEmpty(tmpNum) ? string.Empty : tmpNum.Trim());

        //            hdrValue = (string.IsNullOrEmpty(headerCode) ? "__" :
        //              ((headerCode.Length > 1) ? headerCode.PadLeft(2, '_') : headerCode.Substring(0, 2)));

        //            do
        //            {
        //                if (tmpNum.Length > 1)
        //                {
        //                    if (!char.IsNumber(tmpNum, 0))
        //                    {
        //                        tVal = tmpNum.Substring(1);

        //                        if (!int.TryParse(tVal, out nValue))
        //                        {
        //                            result = string.Empty;

        //                            goto endLogic;
        //                        }

        //                        nValue++;

        //                        if (nValue > 999)
        //                        {
        //                            chr = tmpNum[0];
        //                            chr++;

        //                            if (chr > 0x60)
        //                            {
        //                                chr = (char)0x7b;
        //                            }

        //                            tmpNum = string.Concat(chr, "001");
        //                        }
        //                        else
        //                        {
        //                            tmpNum = string.Concat(tmpNum[0], nValue.ToString().PadLeft(3, '0'));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (!int.TryParse(tmpNum, out nValue))
        //                        {
        //                            result = string.Empty;

        //                            goto endLogic;
        //                        }

        //                        nValue++;

        //                        if (nValue > 9999)
        //                        {
        //                            tmpNum = "A001";
        //                        }
        //                        else
        //                        {
        //                            tmpNum = nValue.ToString().PadLeft(4, '0');
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    tmpNum = "0001";
        //                }

        //                result = string.Concat(hdrValue, tanggalAktif.ToString("yyMM"), tmpNum);

        //                nCount = db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count();

        //                //if (nCount > 0 || nCount == 0)
        //                #region old

        //                //      if (!char.IsNumber(tmpNum, 0))
        //                //      {
        //                //        string iNum = result.Substring(7, 3);
        //                //        string iTestNum = result.Substring(0, 6),
        //                //           sNum = null;

        //                //        //string iNumSwi = "A";
        //                //        string iNumSwi = result.Substring(6, 1);

        //                //        int Num = 0;

        //                //        bool f = int.TryParse(iTestNum, out Num);

        //                //        if (!f)
        //                //        {
        //                //          nCountTambah++;
        //                //          if ((iNum == "000"))
        //                //          {
        //                //            iNum = "999";
        //                //            tmpNum = "0001";
        //                //          }
        //                //        }


        //                //        do
        //                //        {
        //                //          switch (iNumSwi)
        //                //          {
        //                //            case "A":
        //                //              {
        //                //                result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //                {
        //                //                  iNumSwi = "B";
        //                //                  iNum = "001";
        //                //                  break;

        //                //                }
        //                //                else
        //                //                {
        //                //                  result = string.Concat(iTestNum, iNumSwi, iNum);
        //                //                  goto EndOfCount;
        //                //                }
        //                //              }
        //                //              break;
        //                //            case "B":
        //                //              {
        //                //                result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //                {
        //                //                  iNumSwi = "C";
        //                //                  iNum = "001";
        //                //                  break;
        //                //                }
        //                //                else
        //                //                {
        //                //                  result = string.Concat(iTestNum, iNumSwi, iNum);
        //                //                  nCountTambah--;
        //                //                  nCount--;
        //                //                }
        //                //              }
        //                //              break;
        //                //            case "C":
        //                //              {
        //                //                result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //                {
        //                //                  iNumSwi = "D";
        //                //                  iNum = "001";
        //                //                  break;
        //                //                }
        //                //                else
        //                //                {
        //                //                  result = string.Concat(iTestNum, iNumSwi, iNum);
        //                //                  nCountTambah--;
        //                //                  nCount--;
        //                //                }
        //                //              }
        //                //              break;
        //                //            case "D":
        //                //              {
        //                //                result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //                {
        //                //                  iNumSwi = "E";
        //                //                  iNum = "001";
        //                //                  break;
        //                //                }
        //                //                else
        //                //                {
        //                //                  result = string.Concat(iTestNum, iNumSwi, iNum);
        //                //                  nCountTambah--;
        //                //                  nCount--;
        //                //                }
        //                //              }
        //                //              break;
        //                //            case "E":
        //                //              {
        //                //                result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //                {
        //                //                  iNumSwi = "F";
        //                //                  iNum = "001";
        //                //                  break;
        //                //                }
        //                //                else
        //                //                {
        //                //                  result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                  nCountTambah--;
        //                //                  nCount--;
        //                //                }
        //                //              }
        //                //              break;
        //                //            case "F":
        //                //              {
        //                //                result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //                {
        //                //                  iNumSwi = "G";
        //                //                  iNum = "001";
        //                //                  break;
        //                //                }
        //                //                else
        //                //                {
        //                //                  result = string.Concat(iTestNum, iNumSwi, iNum);
        //                //                  nCountTambah--;
        //                //                  nCount--;
        //                //                }
        //                //              }
        //                //              break;
        //                //            case "G":
        //                //              {
        //                //                result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //                {
        //                //                  iNumSwi = "H";
        //                //                  iNum = "001";
        //                //                  break;
        //                //                }
        //                //                else
        //                //                {
        //                //                  result = string.Concat(iTestNum, iNumSwi, iNum);
        //                //                  nCountTambah--;
        //                //                  nCount--;
        //                //                }
        //                //              }
        //                //              break;
        //                //            case "H":
        //                //              {
        //                //                result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //                {
        //                //                  iNumSwi = "I";
        //                //                  iNum = "001";
        //                //                  break;
        //                //                }
        //                //                else
        //                //                {
        //                //                  result = string.Concat(iTestNum, iNumSwi, Num);
        //                //                  nCountTambah--;
        //                //                  nCount--;
        //                //                }
        //                //              }
        //                //              break;
        //                //            case "I":
        //                //              {
        //                //                result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //                {
        //                //                  iNumSwi = "J";
        //                //                  iNum = "001";
        //                //                  break;
        //                //                }
        //                //                else
        //                //                {
        //                //                  result = string.Concat(iTestNum, iNumSwi, iNum);
        //                //                  nCountTambah--;
        //                //                  nCount--;
        //                //                }
        //                //              }
        //                //              break;
        //                //            case "J":
        //                //              {
        //                //                result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //                {
        //                //                  iNumSwi = "K";
        //                //                  iNum = "001";
        //                //                  break;
        //                //                }
        //                //                else
        //                //                {
        //                //                  result = string.Concat(iTestNum, iNumSwi, iNum);
        //                //                  nCountTambah--;
        //                //                  nCount--;
        //                //                }
        //                //              }
        //                //              break;
        //                //            case "K":
        //                //              {
        //                //                result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //                {
        //                //                  iNumSwi = "L";
        //                //                  iNum = "001";
        //                //                  break;
        //                //                }
        //                //                else
        //                //                {
        //                //                  result = string.Concat(iTestNum, iNumSwi, iNum);
        //                //                  nCountTambah--;
        //                //                  nCount--;
        //                //                }
        //                //              }
        //                //              break;
        //                //            case "L":
        //                //              {
        //                //                result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //                {
        //                //                  iNumSwi = "M";
        //                //                  iNum = "001";
        //                //                  break;
        //                //                }
        //                //                else
        //                //                {
        //                //                  result = string.Concat(iTestNum, iNumSwi, iNum);
        //                //                  nCountTambah--;
        //                //                  nCount--;
        //                //                }
        //                //              }
        //                //              break;
        //                //            case "M":
        //                //              {
        //                //                result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //                {
        //                //                  iNumSwi = "N";
        //                //                  iNum = "001";
        //                //                  break;
        //                //                }
        //                //                else
        //                //                {
        //                //                  result = string.Concat(iTestNum, iNumSwi, iNum);
        //                //                  nCountTambah--;
        //                //                  nCount--;
        //                //                }
        //                //              }
        //                //              break;
        //                //            case "N":
        //                //              {
        //                //                result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //                {
        //                //                  iNumSwi = "O";
        //                //                  iNum = "001";
        //                //                  break;
        //                //                }
        //                //                else
        //                //                {
        //                //                  result = string.Concat(iTestNum, iNumSwi, iNum);
        //                //                  goto EndOfCount;
        //                //                }
        //                //              }
        //                //              break;
        //                //            case "O":
        //                //              {
        //                //                result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //                {
        //                //                  iNumSwi = "P";
        //                //                  iNum = "001";
        //                //                  break;
        //                //                }
        //                //                else
        //                //                {
        //                //                  result = string.Concat(iTestNum, iNumSwi, iNum);
        //                //                  goto EndOfCount;
        //                //                }
        //                //              }
        //                //              break;
        //                //            case "P":
        //                //              {
        //                //                result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //                {
        //                //                  iNumSwi = "Q";
        //                //                  iNum = "001";
        //                //                  break;
        //                //                }
        //                //                else
        //                //                {
        //                //                  result = string.Concat(iTestNum, iNumSwi, iNum);
        //                //                  goto EndOfCount;
        //                //                }
        //                //              }
        //                //              break;
        //                //            case "Q":
        //                //              {
        //                //                result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //                {
        //                //                  iNumSwi = "R";
        //                //                  iNum = "001";
        //                //                  break;
        //                //                }
        //                //                else
        //                //                {
        //                //                  result = string.Concat(iTestNum, iNumSwi, iNum);
        //                //                  nCountTambah--;
        //                //                  nCount--;
        //                //                }
        //                //              }
        //                //              break;
        //                //            case "R":
        //                //              {
        //                //                result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //                {
        //                //                  iNumSwi = "S";
        //                //                  iNum = "001";
        //                //                  break;
        //                //                }
        //                //                else
        //                //                {
        //                //                  result = string.Concat(iTestNum, iNumSwi, iNum);
        //                //                  goto EndOfCount;
        //                //                }
        //                //              }
        //                //              break;
        //                //            case "S":
        //                //              {
        //                //                result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //                {
        //                //                  iNumSwi = "T";
        //                //                  iNum = "001";
        //                //                  break;
        //                //                }
        //                //                else
        //                //                {
        //                //                  result = string.Concat(iTestNum, iNumSwi, iNum);
        //                //                  goto EndOfCount;
        //                //                }
        //                //              }
        //                //              break;
        //                //            case "T":
        //                //              {
        //                //                result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //                {
        //                //                  iNumSwi = "U";
        //                //                  iNum = "001";
        //                //                  break;
        //                //                }
        //                //                else
        //                //                {
        //                //                  result = string.Concat(iTestNum, iNumSwi, iNum);
        //                //                  goto EndOfCount;
        //                //                }
        //                //              }
        //                //              break;
        //                //            case "U":
        //                //              {
        //                //                result = string.Concat(iTestNum, iNumSwi, iNum);

        //                //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //                {
        //                //                  iNumSwi = "V";
        //                //                  iNum = "001";
        //                //                  break;
        //                //                }
        //                //                else
        //                //                {
        //                //                  result = string.Concat(iTestNum, iNumSwi, iNum);
        //                //                  goto EndOfCount;
        //                //                }
        //                //              }
        //                //              break;
        //                //          }
        //                //        } while (nCountTambah != 0);

        //                //        //if (hdrValue == "PL")
        //                //        //{
        //                //        //  result = string.Concat(iTestNum, "E", Num);

        //                //        //  if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //        //  {
        //                //        //    result = string.Concat(iTestNum, "F", Num);

        //                //        //    if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
        //                //        //    {
        //                //        //      result = string.Concat(iTestNum, "G", Num);

        //                //        //      nCount -= 1;
        //                //        //    }
        //                //        //    else
        //                //        //    {
        //                //        //      nCount -= 1;
        //                //        //    }
        //                //        //  }
        //                //        //  else
        //                //        //  {
        //                //        //    nCount -= 1;
        //                //        //  }
        //                //        //}
        //                //        //if (hdrValue == "DO")
        //                //        //{
        //                //        //  result = string.Concat(iTestNum, "C", Num);

        //                //        //  nCount -= 1;
        //                //        //}
        //                //      }
        //                #endregion

        //            } while (nCount != 0);

        //      //EndOfCount:

        //          //switch (tanggalAktif.Month)
        //        //{
        //        //  case 1: sysNum.c_bln01 = tmpNum; break;
        //        //  case 2: sysNum.c_bln02 = tmpNum; break;
        //        //  case 3: sysNum.c_bln03 = tmpNum; break;
        //        //  case 4: sysNum.c_bln04 = tmpNum; break;
        //        //  case 5: sysNum.c_bln05 = tmpNum; break;
        //        //  case 6: sysNum.c_bln06 = tmpNum; break;
        //        //  case 7: sysNum.c_bln07 = tmpNum; break;
        //        //  case 8: sysNum.c_bln08 = tmpNum; break;
        //        //  case 9: sysNum.c_bln09 = tmpNum; break;
        //        //  case 10: sysNum.c_bln10 = tmpNum; break;
        //        //  case 11: sysNum.c_bln11 = tmpNum; break;
        //        //  case 12: sysNum.c_bln12 = tmpNum; break;
        //        //}

        //          //db.SubmitChanges();

        //        endLogic:
        //            ;
        //        }
        //    }

        //    return result;
        //}

        public SPCabangReaderBackup(Config config)
        {
            this.config = config;

            timing();

            ev.Set();
        }

        private void spReader()
        {
            bool isContexted = false;

            ORMDataContext db = null;

            int file_count = 0;

            System.Data.DataTable dataTab = null;
            System.Data.DataSet ds = new System.Data.DataSet();

            string pathTempSPMail = null,
              dir = null,
              file = null;

            try
            {
                if ((config == null) || string.IsNullOrEmpty(config.PathTempExtractMail))
                {
                    pathTempSPMail = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                    pathTempSPMail = System.IO.Path.Combine(config.PathTempExtractMail, "SPGet");

                    dir = (pathTempSPMail.EndsWith("\\") ? pathTempSPMail : string.Concat(pathTempSPMail, "\\"));
                }
                else
                {
                    pathTempSPMail = System.IO.Path.Combine(config.PathTempExtractMail, "SPGet");

                    dir = (pathTempSPMail.EndsWith("\\") ? pathTempSPMail : string.Concat(pathTempSPMail, "\\"));
                }

                if (!System.IO.Directory.Exists(dir))
                {
                    System.IO.Directory.CreateDirectory(dir);
                }

                if (db == null)
                {
                    db = new ORMDataContext(this.config.ConnectionString);
                }
                else
                {
                    isContexted = true;
                }

                //db = new ORMDataContext(this.config.ConnectionString);
                //string dir = config.PathTempExtractMail;

                String[] all_files = System.IO.Directory.GetFiles(dir);
                //loop through all items
                //foreach (string file in all_files)
                for (file_count = 0; file_count < all_files.Length; file_count++)
                {
                    file = all_files[file_count];

                    dataTab = SaveToTemplateSP(file, dir);

                    if ((dataTab != null) && (dataTab.Rows.Count > 0))
                    {
                        dataTab.TableName = file_count.ToString();

                        ds.Tables.Add(dataTab);
                    }
                }
            }
            catch (Exception ex)
            {
                dataTab = null;

                Logger.WriteLine(ex.Message);
                Logger.WriteLine(ex.StackTrace);
            }

            if (ds.Tables.Count > 0)
            {
                if (db != null)
                {
                    PopulateDoHeaderDetailSP(ds, db, isContexted);
                }
            }
        }

        private void spPharReader()
        {
            bool isContexted = false;

            ORMDataContext db = null;

            int file_count = 0;

            System.Data.DataTable dataTab = null;
            System.Data.DataSet ds = new System.Data.DataSet();

            string pathTempSPMail = null,
              dir = null,
              file = null,
              filename = null;

            try
            {
                if ((config == null) || string.IsNullOrEmpty(config.PathTempExtractMail))
                {
                    pathTempSPMail = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                    pathTempSPMail = System.IO.Path.Combine(config.PathTempExtractMail, "SPPharGet");

                    dir = (pathTempSPMail.EndsWith("\\") ? pathTempSPMail : string.Concat(pathTempSPMail, "\\"));
                }
                else
                {
                    pathTempSPMail = System.IO.Path.Combine(config.PathTempExtractMail, "SPPharGet");

                    dir = (pathTempSPMail.EndsWith("\\") ? pathTempSPMail : string.Concat(pathTempSPMail, "\\"));
                }

                if (!System.IO.Directory.Exists(dir))
                {
                    System.IO.Directory.CreateDirectory(dir);
                }

                if (db == null)
                {
                    db = new ORMDataContext(this.config.ConnectionString);
                }
                else
                {
                    isContexted = true;
                }

                //db = new ORMDataContext(this.config.ConnectionString);
                //string dir = config.PathTempExtractMail;

                String[] all_files = System.IO.Directory.GetFiles(dir);
                //loop through all items
                //foreach (string file in all_files)
                for (file_count = 0; file_count < all_files.Length; file_count++)
                {
                    file = all_files[file_count];

                    if (!string.IsNullOrEmpty(filename))
                    {
                        if (file.Remove(0, 20).Substring(0, 8) != filename.Substring(0, 8))
                        {
                            continue;
                        }
                    }

                    dataTab = SaveToTemplateSP(file, dir);

                    if ((dataTab != null) && (dataTab.Rows.Count > 0))
                    {
                        //dataTab.TableName = file.Remove(0, 20).Substring(8, 1);
                        dataTab.TableName = file.Remove(0, 20).Substring(file.Remove(0, 20).Length - 5, 1);

                        ds.Tables.Add(dataTab);

                        if (ds.Tables.Count > 1)
                        {
                            if (db != null)
                            {
                                PopulateHeaderDetailSPPhar(ds, db, isContexted);
                                filename = null;
                                ds.Tables.Clear();
                                ds.Dispose();
                            }
                        }
                        else
                        {
                            filename = file.Remove(0, 20);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dataTab = null;

                Logger.WriteLine(ex.Message);
                Logger.WriteLine(ex.StackTrace);
            }
        }

        static System.Data.DataTable SaveToTemplateSP(string pathFolder, string Dir)
        {
            if (string.IsNullOrEmpty(pathFolder) || (!System.IO.Directory.Exists(Dir)))
            {
                return null;
            }

            System.Data.DataSet dataSet = new System.Data.DataSet();
            System.Data.DataTable tableSP = null;

            #region Create table

            try
            {

                tableSP = ScmsMailLibrary.Core.Commons.ReadDbfDatabase(pathFolder, Dir);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
                Logger.WriteLine(ex.StackTrace);

                return null;
            }

            #endregion



            return tableSP;
        }

        private void PopulateDoHeaderDetailSP(System.Data.DataSet dataset, ScmsModel.ORMDataContext db, bool isContexted)
        {
            if ((dataset == null))
            {
                return;
            }

            List<LG_SPH> lstSP = null;
            LG_SPH sph = null;
            List<LG_SPD1> ListSPD1 = null;
            List<LG_SPD2> ListSPD2 = null;

            //LG_SPD2 spd2 = null;

            System.Data.DataTable table = null;
            decimal nSalpri = 0,
              nDiscon = 0, nDiscoff = 0;
            string Type = null,
              spID = null,
              Item = null,
              kd = null,
              spCab = null, sCabang = null,
              SingleCabang = null, sXdiscNo = null;
            bool hasChanges = false;
            System.Data.DataRow row = null,
               sRow = null;
            string[] SPNo = null;
            DateTime date = DateTime.Today,
              spDate = DateTime.MinValue;

            int nLoop = 0,
              nQty = 0,
              nLoopC = 0,
              nTotal = 0;

            sph = new LG_SPH();

            if (dataset.Tables != null)
            {
                for (nLoopC = 0; nLoopC < dataset.Tables.Count; nLoopC++)
                {
                    table = dataset.Tables[nLoopC];

                    lstSP = new List<LG_SPH>();
                    ListSPD1 = new List<LG_SPD1>();
                    ListSPD2 = new List<LG_SPD2>();

                    sRow = null;

                    sRow = table.Rows[0];

                    if (db == null)
                    {
                        db = new ORMDataContext(this.config.ConnectionString);
                    }

                    try
                    {
                        if (!isContexted && db.Transaction == null)
                        {
                            db.Connection.Open();

                            db.Transaction = db.Connection.BeginTransaction();
                        }

                        SysNo sysNum = (from q in db.SysNos
                                        where q.c_portal == '3' && q.c_type == "07"
                                          && q.s_tahun == DateTime.Now.Year
                                        select q).SingleOrDefault();

                        spID = Commons2.GenerateNumbering<LG_SPH>(db, "SP", '3', "07", date, "c_spno");

                        for (nLoop = 0; nLoop < table.Rows.Count; nLoop++)
                        {
                            row = table.Rows[nLoop];

                            //nQty = row.GetValue<int>("n_adjust", 0) + row.GetValue<int>("n_order", 0);

                            if (row.GetValue<string>("c_pareto").ToString().Substring(1, 1).Equals("M"))
                            {
                                nQty = row.GetValue<int>("n_adjust", 0);
                                Type = "02";
                            }
                            else
                            {
                                if (row.GetValue<int>("n_adjust", 0) < 0)
                                    nQty = row.GetValue<int>("n_order", 0) + row.GetValue<int>("n_adjust", 0);
                                else
                                    nQty = row.GetValue<int>("n_order", 0);

                                Type = "01";
                            }

                            if (nQty > 0)
                            {
                                kd = row.GetValue<string>("c_kdcab", string.Empty);

                                SingleCabang = (from q in db.LG_Cusmas
                                                where q.c_cab == kd
                                                select q.c_cusno).Take(1).SingleOrDefault();

                                DateTime Now = DateTime.Now;

                                Item = row.GetValue<string>("c_iteno", string.Empty);

                                nSalpri = (from q in db.FA_MasItms
                                           where q.c_iteno == Item
                                           select
                                             q.n_salpri.HasValue ? q.n_salpri.Value : 0
                                              ).Take(1).SingleOrDefault();

                                var Disc = (from q in db.FA_DiscDs
                                            where q.c_iteno == Item && q.c_nodisc == "DSXXXXXX03"
                                            select q).SingleOrDefault();

                                if (Disc != null)
                                {
                                    nDiscon = (Disc.n_discon.HasValue ? Disc.n_discon.Value : 0);
                                    nDiscoff = (Disc.n_discoff.HasValue ? Disc.n_discoff.Value : 0);
                                    sXdiscNo = (Disc.c_nodisc == null ? string.Empty : Disc.c_nodisc);
                                }
                                else
                                {
                                    nDiscon = 0;
                                    nDiscoff = 0;
                                    sXdiscNo = string.Empty;
                                }

                                ListSPD1.Add(new LG_SPD1()
                                {
                                    c_iteno = Item,
                                    c_spno = spID,
                                    c_type = "01",
                                    n_acc = nQty,
                                    n_qty = nQty,
                                    n_sisa = nQty,
                                    n_salpri = nSalpri,
                                    v_ket = ""
                                });

                                ListSPD2.Add(new LG_SPD2()
                                {
                                    c_iteno = Item,
                                    c_spno = spID,
                                    c_type = "03",
                                    c_no = sXdiscNo,
                                    n_discoff = nDiscoff,
                                    n_discon = nDiscon
                                });

                                spCab = row.GetValue<string>("c_nosp").ToString();
                            }
                            if ((ListSPD1 != null) && (ListSPD1.Count > 0))
                            {
                                spDate = DateTime.Parse(sRow.ItemArray[23].ToString());

                                if (spDate != DateTime.MinValue)
                                {
                                    lstSP = (from q in db.LG_SPHs
                                             where q.c_sp == spCab && q.c_cusno == SingleCabang
                                             select q).ToList();

                                    if (lstSP != null && lstSP.Count <= 0)
                                    {
                                        sph = new LG_SPH()
                                        {
                                            c_sp = spCab,
                                            c_type = Type,
                                            c_cusno = SingleCabang,
                                            c_entry = Constant.SYSTEM_USERNAME,
                                            c_spno = spID,
                                            c_update = Constant.SYSTEM_USERNAME,
                                            d_entry = DateTime.Now,
                                            d_spdate = spDate,
                                            d_spinsert = DateTime.Now,
                                            d_update = DateTime.Now,
                                            l_cek = false,
                                            l_delete = false,
                                            l_print = false,
                                            v_ket = ""
                                        };

                                        nTotal++;
                                    }
                                }
                            }
                        }

                        if (nTotal > 0)
                        {
                            if (ListSPD1.Count > 0)
                            {
                                db.LG_SPD1s.InsertAllOnSubmit(ListSPD1.ToArray());
                                ListSPD1.Clear();
                            }
                            if (ListSPD2.Count > 0)
                            {
                                db.LG_SPD2s.InsertAllOnSubmit(ListSPD2.ToArray());
                                ListSPD2.Clear();
                            }
                            if (sph != null)
                            {
                                db.LG_SPHs.InsertOnSubmit(sph);

                                if (!isContexted)
                                {
                                    string tmpNum = spID.Substring(6, 4);
                                    switch (DateTime.Now.Month)
                                    {
                                        case 1: sysNum.c_bln01 = tmpNum; break;
                                        case 2: sysNum.c_bln02 = tmpNum; break;
                                        case 3: sysNum.c_bln03 = tmpNum; break;
                                        case 4: sysNum.c_bln04 = tmpNum; break;
                                        case 5: sysNum.c_bln05 = tmpNum; break;
                                        case 6: sysNum.c_bln06 = tmpNum; break;
                                        case 7: sysNum.c_bln07 = tmpNum; break;
                                        case 8: sysNum.c_bln08 = tmpNum; break;
                                        case 9: sysNum.c_bln09 = tmpNum; break;
                                        case 10: sysNum.c_bln10 = tmpNum; break;
                                        case 11: sysNum.c_bln11 = tmpNum; break;
                                        case 12: sysNum.c_bln12 = tmpNum; break;
                                    }

                                    db.SubmitChanges();
                                    db.Transaction.Commit();
                                    db.Dispose();
                                }
                                else
                                {
                                    db.Transaction.Rollback();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (db.Transaction != null)
                        {
                            db.Transaction.Rollback();
                        }
                    }
                    finally
                    {
                        db = null;
                    }
                }
            }
        }

        private void PopulateHeaderDetailSPPhar(System.Data.DataSet dataset, ScmsModel.ORMDataContext db, bool isContexted)
        {
            if ((dataset == null))
            {
                return;
            }

            List<LG_SPH> lstSP = null;
            LG_SPH sph = null;
            List<LG_SPD1> ListSPD1 = null;
            List<LG_SPD2> ListSPD2 = null;

            LG_POH poh = null;
            LG_POD2 pod2 = null;
            List<LG_POD1> ListPOD1 = null;

            System.Data.DataTable tableH = null,
                tableD = null;
            decimal nSalpri = 0,
              nDiscon = 0, nDiscoff = 0;
            string Type = null,
              spID = null, poID = null,
              kdItem = null,
              Item = null,
              kd = null, nm = null,
              spCab = null, sOutlet = null, sPoOutlet = null,
              SingleCabang = null, sXdiscNo = null,
              Nosup = null;
            bool hasChanges = false;
            System.Data.DataRow row = null;
            string[] SPNo = null,
                cusList = null;

            DateTime date = DateTime.Today,
              spDate = DateTime.MinValue;

            int nLoop = 0,
              nQty = 0,
              nLoopC = 0,
              nTotal = 0;

            sph = new LG_SPH();

            if (dataset.Tables != null)
            {
                try
                {
                    if (!isContexted && db.Transaction == null)
                    {
                        db.Connection.Open();

                        db.Transaction = db.Connection.BeginTransaction();
                    }

                    #region "SP Header & Purchase Order"
                    tableH = dataset.Tables["H"];

                    SysNo sysNum = (from q in db.SysNos
                                    where q.c_portal == '3' && q.c_type == "07"
                                      && q.s_tahun == DateTime.Now.Year
                                    select q).SingleOrDefault();

                    spID = Commons2.GenerateNumbering<LG_SPH>(db, "SP", '3', "07", date, "c_spno");

                    SysNo sysNumPO = (from q in db.SysNos
                                    where q.c_portal == '3' && q.c_type == "02"
                                      && q.s_tahun == DateTime.Now.Year
                                    select q).SingleOrDefault();

                    poID = Commons2.GenerateNumbering<LG_POH>(db, "PO", '3', "02", date, "c_pono");

                    for (nLoop = 0; nLoop < tableH.Rows.Count; nLoop++)
                    {
                        row = tableH.Rows[nLoop];

                        //spCab = row.GetValue<string>("c_nosp").ToString();
                        sOutlet = row.GetValue<string>("c_outlet").ToString().Substring(2,4);
                        sPoOutlet = row.GetValue<string>("c_po_outle").ToString();

                        kd = row.GetValue<string>("c_kdcab", string.Empty);

                        SingleCabang = (from q in db.LG_Cusmas
                                        where q.c_cab == kd
                                        select q.c_cusno).Take(1).SingleOrDefault();

                        spDate = DateTime.Parse(row.ItemArray[1].ToString());

                        sph = new LG_SPH()
                        {
                            c_sp = "SPPHAR",
                            c_type = "06",
                            c_cusno = SingleCabang,
                            c_entry = Constant.SYSTEM_USERNAME,
                            c_spno = spID,
                            c_update = Constant.SYSTEM_USERNAME,
                            d_entry = DateTime.Now,
                            d_spdate = spDate,
                            d_spinsert = DateTime.Now,
                            d_update = DateTime.Now,
                            l_cek = true,
                            l_delete = false,
                            l_print = false,
                            v_ket = "SP Pharmanet",
                            c_outlet = sOutlet,
                            c_po_outlet = sPoOutlet
                        };

                        nm = row.GetValue<string>("c_supplier", string.Empty);

                        Nosup = (from q in db.LG_DatSups
                                 where q.v_namatax == nm
                                 select q.c_nosup).Take(1).SingleOrDefault();

                        poh = new LG_POH()
                        {
                            c_gdg = '1',
                            c_pono = poID,
                            d_podate = DateTime.Now.Date,
                            c_nosup = Nosup,
                            l_import = false,
                            c_kurs = "01",
                            n_kurs = 1,
                            v_ket = "PHARMANET",
                            n_bruto = 0,
                            n_disc = 0,
                            n_pdisc = 0,
                            n_xdisc = 0,
                            n_ppn = 0,
                            n_bilva = 0,
                            l_print = false,
                            l_send = false,
                            l_revisi = true,
                            c_entry = Constant.SYSTEM_USERNAME,
                            d_entry = DateTime.Now,
                            c_update = Constant.SYSTEM_USERNAME,
                            d_update = DateTime.Now,
                            c_type = "01"
                        };

                        pod2 = new LG_POD2()
                        {
                            c_gdg = '1',
                            c_pono = poID,
                            c_orno = spID
                        };
                    }
                    #endregion

                    #region "SP Detail & PO Detail"
                    tableD = dataset.Tables["D"];

                    ListSPD1 = new List<LG_SPD1>();
                    ListSPD2 = new List<LG_SPD2>();
                    ListPOD1 = new List<LG_POD1>();

                    //cusList = new string[] { "00001", "00112", "00113", "00117" , "00120" };

                    for (nLoop = 0; nLoop < tableD.Rows.Count; nLoop++)
                    {
                        row = tableD.Rows[nLoop];

                        nQty = row.GetValue<int>("n_qty", 0);

                        if (nQty > 0)
                        {
                            kdItem = row.GetValue<string>("c_iteno", string.Empty).Trim();

                            //Item = (from q in db.FA_MasItms
                            //           where q.c_itenopri == kdItem.PadLeft(6,'0').Trim() 
                            //           && q.c_nosup == Nosup
                            //           select
                            //             string.IsNullOrEmpty(q.c_iteno) ? string.Empty : q.c_iteno
                            //              ).Take(1).SingleOrDefault();

                            //if(string.IsNullOrEmpty(Item))
                            //{
                            //    continue;
                            //}

                            nSalpri = (from q in db.FA_MasItms
                                       where q.c_iteno == kdItem
                                       select
                                         q.n_salpri.HasValue ? q.n_salpri.Value : 0
                                          ).Take(1).SingleOrDefault();

                            var Disc = (from q in db.FA_DiscDs
                                        where q.c_iteno == Item && q.c_nodisc == "DSXXXXXX03"
                                        select q).SingleOrDefault();

                            if (Disc != null)
                            {
                                nDiscon = (Disc.n_discon.HasValue ? Disc.n_discon.Value : 0);
                                nDiscoff = (Disc.n_discoff.HasValue ? Disc.n_discoff.Value : 0);
                                sXdiscNo = (Disc.c_nodisc == null ? string.Empty : Disc.c_nodisc);
                            }
                            else
                            {
                                nDiscon = 0;
                                nDiscoff = 0;
                                sXdiscNo = string.Empty;
                            }

                            ListSPD1.Add(new LG_SPD1()
                            {
                                c_iteno = kdItem,
                                c_spno = spID,
                                c_type = "01",
                                n_acc = nQty,
                                n_qty = nQty,
                                n_sisa = nQty,
                                n_salpri = nSalpri,
                                v_ket = ""
                            });

                            ListSPD2.Add(new LG_SPD2()
                            {
                                c_iteno = kdItem,
                                c_spno = spID,
                                c_type = "03",
                                c_no = sXdiscNo,
                                n_discoff = nDiscoff,
                                n_discon = nDiscon
                            });

                            ListPOD1.Add(new LG_POD1()
                            {
                                c_gdg = '1',
                                c_pono = poID,
                                c_iteno = kdItem,
                                n_qty = nQty,
                                n_disc = nDiscon,
                                n_salpri = nSalpri,
                                n_sisa = nQty
                            });
                        }

                    }
                    #endregion

                    #region Insert to DB
                    if (spDate != DateTime.MinValue)
                    {
                        lstSP = new List<LG_SPH>();
                        lstSP = (from q in db.LG_SPHs
                                 where q.c_sp == spCab && q.c_cusno == SingleCabang
                                 select q).ToList();

                        if (lstSP != null && lstSP.Count <= 0)
                        {
                            nTotal++;
                        }
                    }

                    if (nTotal > 0)
                    {
                        if (sph != null)
                        {
                            db.LG_SPHs.InsertOnSubmit(sph);
                        }
                        if (poh != null)
                        {
                            db.LG_POHs.InsertOnSubmit(poh);
                        }
                        if (ListSPD1.Count > 0)
                        {
                            db.LG_SPD1s.InsertAllOnSubmit(ListSPD1.ToArray());
                            ListSPD1.Clear();
                        }
                        if (ListSPD2.Count > 0)
                        {
                            db.LG_SPD2s.InsertAllOnSubmit(ListSPD2.ToArray());
                            ListSPD2.Clear();
                        }
                        if (ListPOD1.Count > 0)
                        {
                            db.LG_POD1s.InsertAllOnSubmit(ListPOD1.ToArray());
                            ListPOD1.Clear();
                        }
                        if (pod2 != null)
                        {
                            db.LG_POD2s.InsertOnSubmit(pod2);
                        }
                        if (!isContexted)
                        {
                            string tmpNum = spID.Substring(6, 4);
                            string tmpNumPO = poID.Substring(6, 4);
                            switch (DateTime.Now.Month)
                            {
                                case 1:
                                    sysNum.c_bln01 = tmpNum;
                                    sysNumPO.c_bln01 = tmpNumPO;
                                    break;
                                case 2:
                                    sysNum.c_bln02 = tmpNum;
                                    sysNumPO.c_bln02 = tmpNumPO; 
                                    break;
                                case 3:
                                    sysNum.c_bln03 = tmpNum;
                                    sysNumPO.c_bln03 = tmpNumPO; 
                                    break;
                                case 4:
                                    sysNum.c_bln04 = tmpNum;
                                    sysNumPO.c_bln04 = tmpNumPO; 
                                    break;
                                case 5:
                                    sysNum.c_bln05 = tmpNum;
                                    sysNumPO.c_bln05 = tmpNumPO; 
                                    break;
                                case 6:
                                    sysNum.c_bln06 = tmpNum;
                                    sysNumPO.c_bln06 = tmpNumPO;
                                    break;
                                case 7:
                                    sysNum.c_bln07 = tmpNum;
                                    sysNumPO.c_bln07 = tmpNumPO; 
                                    break;
                                case 8:
                                    sysNum.c_bln08 = tmpNum;
                                    sysNumPO.c_bln08 = tmpNumPO; 
                                    break;
                                case 9:
                                    sysNum.c_bln09 = tmpNum;
                                    sysNumPO.c_bln09 = tmpNumPO; 
                                    break;
                                case 10:
                                    sysNum.c_bln10 = tmpNum;
                                    sysNumPO.c_bln10 = tmpNumPO; 
                                    break;
                                case 11:
                                    sysNum.c_bln11 = tmpNum;
                                    sysNumPO.c_bln11 = tmpNumPO; 
                                    break;
                                case 12:
                                    sysNum.c_bln12 = tmpNum;
                                    sysNumPO.c_bln12 = tmpNumPO; 
                                    break;
                            }

                            db.SubmitChanges();
                            db.Transaction.Commit();
                            db.Dispose();
                        }
                        else
                        {
                            db.Transaction.Rollback();
                        }
                    }
                    else
                    {
                        db.Transaction.Rollback();
                    }
                    #endregion

                    #region Send PO
                    string gudang = "HO";
                    
                    SqlConnection cn = new SqlConnection(this.config.ConnectionString);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandText = "update lg_poh set l_send = 1,c_cab = '" + gudang + "' where c_pono = '" + poID + "' ";

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandText = "SELECT RIGHT(a.c_pono,8) as c_corno, a.c_iteno, c.c_itenopri, " +
                              "c.v_itnam as c_itnam, c.v_undes as c_undes, a.n_qty, a.n_salpri, '' as c_nosp, " +
                              "case c.c_via when '01' then 'D' when '02' then 'U' when '03' then 'I' end as c_via,c_po_outlet as c_po_outle FROM LG_POD1 AS a " +
                              "INNER JOIN LG_POH AS b ON a.c_pono = b.c_pono AND a.c_gdg = b.c_gdg INNER JOIN FA_MasItm AS c ON a.c_iteno = c.c_iteno " +
                              "Inner Join lg_pod2 d on d.c_pono = b.c_pono " +
                              "Inner Join lg_sph e on e.c_spno = d.c_orno " +
                              "where a.c_pono = '" + poID + "' ";
                    
                    SqlDataReader reader = cmd.ExecuteReader();

                    cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandText = "Select RIGHT(a.c_pono,8) as c_corno, d_podate as d_corda, 'Head Office' as c_komen1, '' as c_komen2, " +
                                "CAST(CASE WHEN 1=1 THEN 1 ELSE 0 END AS BIT) as L_load, '' as c_kddepo, c_cab as c_kdcab, c_po_outlet as c_po_outle from LG_POH a " +
                                "Inner Join lg_pod2 b on a.c_pono = b.c_pono " +
                                "Inner Join lg_sph c on c.c_spno = b.c_orno " +
                                "where a.c_pono = '" + poID + "' ";
                    
                    DataTable dt = new DataTable(),
                      dt1 = new DataTable();
                    DataSet ds = new DataSet(),
                      ds1 = new DataSet();

                    string path = null;

                    if (Nosup.Equals("00001"))
                    {
                        path = @"D:\PHAROSGROUP\PHAROS\";
                    }
                    else if (Nosup.Equals("00112"))
                    {
                        path = @"D:\PHAROSGROUP\NUTRINDO JAYA\";
                    }
                    else if (Nosup.Equals("00113"))
                    {
                        path = @"D:\PHAROSGROUP\NUTRISAINS\";
                    }
                    else if (Nosup.Equals("00117"))
                    {
                        path = @"D:\PHAROSGROUP\PRIMA MEDIKA\";
                    }
                    else if (Nosup.Equals("00120"))
                    {
                        path = @"D:\PHAROSGROUP\APEX\";
                    }

                    if (!string.IsNullOrEmpty(path))
                    {
                        if (reader.HasRows)
                        {
                            dt.Load(reader);

                            ds.Tables.Add(dt);
                        }

                        reader.Close();
                        reader.Dispose();

                        SqlDataReader reader1 = cmd.ExecuteReader();

                        if (reader1.HasRows)
                        {
                            dt1.Load(reader1);

                            ds1.Tables.Add(dt1);
                        }

                        cn.Close();

                        bool isSukses = false;

                        if (ds.Tables.Count > 0 && (!string.IsNullOrEmpty(path)) && (!string.IsNullOrEmpty(poID)))
                        {
                            isSukses = ExportDBF(ds1, path, poID.Substring(2) + "Header", true, false);
                            if (ds1.Tables.Count > 0 && (!string.IsNullOrEmpty(path)) && (!string.IsNullOrEmpty(poID)) && isSukses)
                            {
                                isSukses = ExportDBF(ds, path, poID.Substring(2) + "Detil", false, false);
                                if (ds1.Tables.Count > 0 && (!string.IsNullOrEmpty(path)) && (!string.IsNullOrEmpty(poID)) && isSukses)
                                {
                                    isSukses = ExportDBF(ds, path, poID.Substring(2) + ".txt", false, true);
                                }
                            }
                        }
                    }

                    cn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }
                }
            }
        }

        public void Stop()
        {
            pop3.Stop();
        }

        public static string DbfColumnParser(System.Data.DataColumn column, string caption)
        {
            string rets = null;

            if (column.DataType.Equals(typeof(float)) ||
              column.DataType.Equals(typeof(double)) ||
              column.DataType.Equals(typeof(decimal)))
            {
                int i = 18;
                switch (caption.ToLower())
                {
                    case "n_qty":
                        i = 10;
                        break;
                    case "n_salpri":
                        i = 13;
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
                    case "c_corno":
                        i = 8;
                        break;
                    case "c_komen1":
                    case "c_itnam":
                        i = 50;
                        break;
                    case "c_kdcab":
                        i = 3;
                        break;
                    case "c_iteno":
                        i = 4;
                        break;
                    case "c_itenopri":
                        i = 15;
                        break;
                    case "c_undes":
                        i = 10;
                        break;
                    case "c_po_outle":
                        i = 10;
                        break;
                }
                rets = string.Format("[{0}] char({1}) {2}",
                  column.ColumnName, i,
                  (column.AllowDBNull ? "NULL" : "NOT NULL"));
            }

            return rets;
        }

        public bool ExportDBF(System.Data.DataSet dsExport, string folderPath, string sNama, bool isHeader, bool isText)
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
                }
                else
                {
                    DataTable dt = dsExport.Tables[0];

                    dt.Columns.Remove("c_corno");
                    dt.Columns.Remove("c_iteno");
                    dt.Columns.Remove("c_itenopri");
                    dt.Columns.Remove("c_nosp");
                    dt.Columns.Remove("c_via");

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

    }
}
