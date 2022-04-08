using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsSoaLibrary.Commons;
using System.ServiceModel.Channels;
using ScmsModel;
using ScmsModel.Core;
using ScmsSoaLibraryInterface.Commons;
using ScmsSoaLibrary.Bussiness;

namespace ScmsMailLibrary
{
  public class SPCabangReader
  {
    private Pop3MailerReader pop3;

    private ScmsSoaLibrary.Commons.Config config;

    //static string GenerateNumbering<T>(ORMDataContext db, string headerCode, char portalKode, string tipeKode, DateTime tanggalAktif, string fieldCondition)
    //{
    //  string result = string.Empty;

    //  //db.GetTable<T>().Where(

    //  int nCount = 0,
    //    nValue = 0,
    //    nCountTambah = 0;
    //  string tmpNum = null,
    //    hdrValue = null,
    //    tVal = null;
    //  char chr = char.MinValue;

    //  SysNo sysNum = (from q in db.SysNos
    //                  where q.c_portal == portalKode && q.c_type == tipeKode
    //                    && q.s_tahun == tanggalAktif.Year
    //                  select q).SingleOrDefault();

    //  if (sysNum != null)
    //  {
    //    switch (tanggalAktif.Month)
    //    {
    //      case 1: tmpNum = (sysNum.c_bln01 ?? string.Empty); break;
    //      case 2: tmpNum = (sysNum.c_bln02 ?? string.Empty); break;
    //      case 3: tmpNum = (sysNum.c_bln03 ?? string.Empty); break;
    //      case 4: tmpNum = (sysNum.c_bln04 ?? string.Empty); break;
    //      case 5: tmpNum = (sysNum.c_bln05 ?? string.Empty); break;
    //      case 6: tmpNum = (sysNum.c_bln06 ?? string.Empty); break;
    //      case 7: tmpNum = (sysNum.c_bln07 ?? string.Empty); break;
    //      case 8: tmpNum = (sysNum.c_bln08 ?? string.Empty); break;
    //      case 9: tmpNum = (sysNum.c_bln09 ?? string.Empty); break;
    //      case 10: tmpNum = (sysNum.c_bln10 ?? string.Empty); break;
    //      case 11: tmpNum = (sysNum.c_bln11 ?? string.Empty); break;
    //      case 12: tmpNum = (sysNum.c_bln12 ?? string.Empty); break;
    //      default: tmpNum = null; break;
    //    }

    //    if (!string.IsNullOrEmpty(tmpNum))
    //    {
    //      tmpNum = (string.IsNullOrEmpty(tmpNum) ? string.Empty : tmpNum.Trim());

    //      hdrValue = (string.IsNullOrEmpty(headerCode) ? "__" :
    //        ((headerCode.Length > 1) ? headerCode.PadLeft(2, '_') : headerCode.Substring(0, 2)));

    //      do
    //      {
    //        if (tmpNum.Length > 1)
    //        {
    //          if (!char.IsNumber(tmpNum, 0))
    //          {
    //            tVal = tmpNum.Substring(1);

    //            if (!int.TryParse(tVal, out nValue))
    //            {
    //              result = string.Empty;

    //              goto endLogic;
    //            }

    //            nValue++;

    //            if (nValue > 999)
    //            {
    //              chr = tmpNum[0];
    //              chr++;

    //              if (chr > 0x60)
    //              {
    //                chr = (char)0x7b;
    //              }

    //              tmpNum = string.Concat(chr, "001");
    //            }
    //            else
    //            {
    //              tmpNum = string.Concat(tmpNum[0], nValue.ToString().PadLeft(3, '0'));
    //            }
    //          }
    //          else
    //          {
    //            if (!int.TryParse(tmpNum, out nValue))
    //            {
    //              result = string.Empty;

    //              goto endLogic;
    //            }

    //            nValue++;

    //            if (nValue > 9999)
    //            {
    //              tmpNum = "A001";
    //            }
    //            else
    //            {
    //              tmpNum = nValue.ToString().PadLeft(4, '0');
    //            }
    //          }
    //        }
    //        else
    //        {
    //          tmpNum = "0001";
    //        }

    //        result = string.Concat(hdrValue, tanggalAktif.ToString("yyMM"), tmpNum);

    //        nCount = db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count();
    //  #region old
            
    //  //        if (nCount > 0)
    //  //      {
    //  //        string iNum = result.Substring(7, 3);
    //  //        string iTestNum = result.Substring(0, 6),
    //  //           sNum = null;

    //  //        string iNumSwi = "A";
    //  //        int Num = 0;

    //  //        bool f = int.TryParse(iTestNum, out Num);

    //  //        if (!f)
    //  //        {
    //  //          nCountTambah++;
    //  //          if ((iNum == "000"))
    //  //          {
    //  //            iNum = "999";
    //  //            tmpNum = "0001";
    //  //          }
    //  //        }


    //  //        do
    //  //        {
    //  //          switch (iNumSwi)
    //  //          {
    //  //            case "A":
    //  //              {
    //  //                result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //  //                {
    //  //                  iNumSwi = "B";
    //  //                  iNum = "001";
    //  //                  break;

    //  //                }
    //  //                else
    //  //                {
    //  //                  result = string.Concat(iTestNum, iNumSwi, iNum);
    //  //                  nCountTambah--;
    //  //                  nCount--;
    //  //                }
    //  //              }
    //  //              break;
    //  //            case "B":
    //  //              {
    //  //                result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //  //                {
    //  //                  iNumSwi = "C";
    //  //                  iNum = "001";
    //  //                  break;
    //  //                }
    //  //                else
    //  //                {
    //  //                  result = string.Concat(iTestNum, iNumSwi, iNum);
    //  //                  goto EndOfCount;
    //  //                }
    //  //              }
    //  //              break;
    //  //            case "C":
    //  //              {
    //  //                result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //  //                {
    //  //                  iNumSwi = "D";
    //  //                  iNum = "001";
    //  //                  break;
    //  //                }
    //  //                else
    //  //                {
    //  //                  result = string.Concat(iTestNum, iNumSwi, iNum);
    //  //                  nCountTambah--;
    //  //                  nCount--;
    //  //                }
    //  //              }
    //  //              break;
    //  //            case "D":
    //  //              {
    //  //                result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //  //                {
    //  //                  iNumSwi = "E";
    //  //                  iNum = "001";
    //  //                  break;
    //  //                }
    //  //                else
    //  //                {
    //  //                  result = string.Concat(iTestNum, iNumSwi, iNum);
    //  //                  nCountTambah--;
    //  //                  nCount--;
    //  //                }
    //  //              }
    //  //              break;
    //  //            case "E":
    //  //              {
    //  //                result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //  //                {
    //  //                  iNumSwi = "F";
    //  //                  iNum = "001";
    //  //                  break;
    //  //                }
    //  //                else
    //  //                {
    //  //                  result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                  nCountTambah--;
    //  //                  nCount--;
    //  //                }
    //  //              }
    //  //              break;
    //  //            case "F":
    //  //              {
    //  //                result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //  //                {
    //  //                  iNumSwi = "G";
    //  //                  iNum = "001";
    //  //                  break;
    //  //                }
    //  //                else
    //  //                {
    //  //                  result = string.Concat(iTestNum, iNumSwi, iNum);
    //  //                  nCountTambah--;
    //  //                  nCount--;
    //  //                }
    //  //              }
    //  //              break;
    //  //            case "G":
    //  //              {
    //  //                result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //  //                {
    //  //                  iNumSwi = "H";
    //  //                  iNum = "001";
    //  //                  break;
    //  //                }
    //  //                else
    //  //                {
    //  //                  result = string.Concat(iTestNum, iNumSwi, iNum);
    //  //                  nCountTambah--;
    //  //                  nCount--;
    //  //                }
    //  //              }
    //  //              break;
    //  //            case "H":
    //  //              {
    //  //                result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //  //                {
    //  //                  iNumSwi = "I";
    //  //                  iNum = "001";
    //  //                  break;
    //  //                }
    //  //                else
    //  //                {
    //  //                  result = string.Concat(iTestNum, iNumSwi, Num);
    //  //                  nCountTambah--;
    //  //                  nCount--;
    //  //                }
    //  //              }
    //  //              break;
    //  //            case "I":
    //  //              {
    //  //                result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //  //                {
    //  //                  iNumSwi = "J";
    //  //                  iNum = "001";
    //  //                  break;
    //  //                }
    //  //                else
    //  //                {
    //  //                  result = string.Concat(iTestNum, iNumSwi, iNum);
    //  //                  nCountTambah--;
    //  //                  nCount--;
    //  //                }
    //  //              }
    //  //              break;
    //  //            case "J":
    //  //              {
    //  //                result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //  //                {
    //  //                  iNumSwi = "K";
    //  //                  iNum = "001";
    //  //                  break;
    //  //                }
    //  //                else
    //  //                {
    //  //                  result = string.Concat(iTestNum, iNumSwi, iNum);
    //  //                  nCountTambah--;
    //  //                  nCount--;
    //  //                }
    //  //              }
    //  //              break;
    //  //            case "K":
    //  //              {
    //  //                result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //  //                {
    //  //                  iNumSwi = "L";
    //  //                  iNum = "001";
    //  //                  break;
    //  //                }
    //  //                else
    //  //                {
    //  //                  result = string.Concat(iTestNum, iNumSwi, iNum);
    //  //                  nCountTambah--;
    //  //                  nCount--;
    //  //                }
    //  //              }
    //  //              break;
    //  //            case "L":
    //  //              {
    //  //                result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //  //                {
    //  //                  iNumSwi = "M";
    //  //                  iNum = "001";
    //  //                  break;
    //  //                }
    //  //                else
    //  //                {
    //  //                  result = string.Concat(iTestNum, iNumSwi, iNum);
    //  //                  nCountTambah--;
    //  //                  nCount--;
    //  //                }
    //  //              }
    //  //              break;
    //  //            case "M":
    //  //              {
    //  //                result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //  //                {
    //  //                  iNumSwi = "N";
    //  //                  iNum = "001";
    //  //                  break;
    //  //                }
    //  //                else
    //  //                {
    //  //                  result = string.Concat(iTestNum, iNumSwi, iNum);
    //  //                  nCountTambah--;
    //  //                  nCount--;
    //  //                }
    //  //              }
    //  //              break;
    //  //            case "N":
    //  //              {
    //  //                result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //  //                {
    //  //                  iNumSwi = "O";
    //  //                  iNum = "001";
    //  //                  break;
    //  //                }
    //  //                else
    //  //                {
    //  //                  result = string.Concat(iTestNum, iNumSwi, iNum);
    //  //                  goto EndOfCount;
    //  //                }
    //  //              }
    //  //              break;
    //  //            case "O":
    //  //              {
    //  //                result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //  //                {
    //  //                  iNumSwi = "P";
    //  //                  iNum = "001";
    //  //                  break;
    //  //                }
    //  //                else
    //  //                {
    //  //                  result = string.Concat(iTestNum, iNumSwi, iNum);
    //  //                  goto EndOfCount;
    //  //                }
    //  //              }
    //  //              break;
    //  //            case "P":
    //  //              {
    //  //                result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //  //                {
    //  //                  iNumSwi = "Q";
    //  //                  iNum = "001";
    //  //                  break;
    //  //                }
    //  //                else
    //  //                {
    //  //                  result = string.Concat(iTestNum, iNumSwi, iNum);
    //  //                  goto EndOfCount;
    //  //                }
    //  //              }
    //  //              break;
    //  //            case "Q":
    //  //              {
    //  //                result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //  //                {
    //  //                  iNumSwi = "R";
    //  //                  iNum = "001";
    //  //                  break;
    //  //                }
    //  //                else
    //  //                {
    //  //                  result = string.Concat(iTestNum, iNumSwi, iNum);
    //  //                  nCountTambah--;
    //  //                  nCount--;
    //  //                }
    //  //              }
    //  //              break;
    //  //            case "R":
    //  //              {
    //  //                result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //  //                {
    //  //                  iNumSwi = "S";
    //  //                  iNum = "001";
    //  //                  break;
    //  //                }
    //  //                else
    //  //                {
    //  //                  result = string.Concat(iTestNum, iNumSwi, iNum);
    //  //                  goto EndOfCount;
    //  //                }
    //  //              }
    //  //              break;
    //  //            case "S":
    //  //              {
    //  //                result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //  //                {
    //  //                  iNumSwi = "T";
    //  //                  iNum = "001";
    //  //                  break;
    //  //                }
    //  //                else
    //  //                {
    //  //                  result = string.Concat(iTestNum, iNumSwi, iNum);
    //  //                  goto EndOfCount;
    //  //                }
    //  //              }
    //  //              break;
    //  //            case "T":
    //  //              {
    //  //                result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //  //                {
    //  //                  iNumSwi = "U";
    //  //                  iNum = "001";
    //  //                  break;
    //  //                }
    //  //                else
    //  //                {
    //  //                  result = string.Concat(iTestNum, iNumSwi, iNum);
    //  //                  goto EndOfCount;
    //  //                }
    //  //              }
    //  //              break;
    //  //            case "U":
    //  //              {
    //  //                result = string.Concat(iTestNum, iNumSwi, iNum);

    //  //                if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //  //                {
    //  //                  iNumSwi = "V";
    //  //                  iNum = "001";
    //  //                  break;
    //  //                }
    //  //                else
    //  //                {
    //  //                  result = string.Concat(iTestNum, iNumSwi, iNum);
    //  //                  goto EndOfCount;
    //  //                }
    //  //              }
    //  //              break;
    //  //          }

    //          //} while (nCountTambah != 0);

    //          //if (hdrValue == "PL")
    //          //{
    //          //  result = string.Concat(iTestNum, "E", Num);

    //          //  if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //          //  {
    //          //    result = string.Concat(iTestNum, "F", Num);

    //          //    if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
    //          //    {
    //          //      result = string.Concat(iTestNum, "G", Num);

    //          //      nCount -= 1;
    //          //    }
    //          //    else
    //          //    {
    //          //      nCount -= 1;
    //          //    }
    //          //  }
    //          //  else
    //          //  {
    //          //    nCount -= 1;
    //          //  }
    //          //}
    //          //if (hdrValue == "DO")
    //          //{
    //          //  result = string.Concat(iTestNum, "C", Num);

    //          //  nCount -= 1;
    //          //}
    //          //      }
    //  #endregion

    //      } while (nCount != 0);

    //    //EndOfCount:

    //      //switch (tanggalAktif.Month)
    //      //{
    //      //  case 1: sysNum.c_bln01 = tmpNum; break;
    //      //  case 2: sysNum.c_bln02 = tmpNum; break;
    //      //  case 3: sysNum.c_bln03 = tmpNum; break;
    //      //  case 4: sysNum.c_bln04 = tmpNum; break;
    //      //  case 5: sysNum.c_bln05 = tmpNum; break;
    //      //  case 6: sysNum.c_bln06 = tmpNum; break;
    //      //  case 7: sysNum.c_bln07 = tmpNum; break;
    //      //  case 8: sysNum.c_bln08 = tmpNum; break;
    //      //  case 9: sysNum.c_bln09 = tmpNum; break;
    //      //  case 10: sysNum.c_bln10 = tmpNum; break;
    //      //  case 11: sysNum.c_bln11 = tmpNum; break;
    //      //  case 12: sysNum.c_bln12 = tmpNum; break;
    //      //}

    //      //db.SubmitChanges();

    //    endLogic:
    //      ;
    //    }
    //  }

    //  return result;
    //}

    public SPCabangReader(ScmsSoaLibrary.Commons.Config config)
    {
      this.config = config;
    }

    public bool Start(string email, string pwd)
    {
      System.Net.IPEndPoint ep = config.POP3DOPharosEP as System.Net.IPEndPoint;

      if (ep == null)
      {
        ep = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 110);
      }

      return Start(ep, email, pwd);
    }

    public bool Start(System.Net.IPEndPoint epHost, string email, string pwd)
    {
      bool bOk = false;

      pop3 = new Pop3MailerReader(epHost.Address.ToString(), epHost.Port, this.config);

      bOk = pop3.Start(email, pwd, false);

      pop3.ClassNotify += new Pop3MailerReader.ClassNotifyEventHandler(pop3_ClassNotify);
      
      pop3.Pop3MailMessage += new Pop3MailerReader.Pop3MailMessageEventHandler(pop_Pop3MailMessageSP);

      return bOk;
    }

    public void Stop()
    {
      pop3.Stop();
    }

    private void pop_Pop3MailMessageSP(object sender, Pop3MailMessageEventArgs e)
    {

      if (!e.HasAttachment)
      {
        return;
      }

      System.Data.DataTable dataTab = null;
      System.Data.DataSet ds = new System.Data.DataSet();
      Pop3MailerReader pop = sender as Pop3MailerReader;
      ORMDataContext db = null;
      int nTables = 0;

      e.Delete = true;

      bool isContexted = false;
      try
      {
        if (db == null)
        {
            db = new ORMDataContext(this.config.ConnectionString);
        }
        else
        {
            isContexted = true;
        }
        //db = new ORMDataContext(this.config.ConnectionString);

        //pop.TempPath
        if (!System.IO.Directory.Exists(pop.TempPath))
        {
          System.IO.Directory.CreateDirectory(pop.TempPath);
        }

        //SaveToTemplateAndExtract(pop.TempPath, 
        //pathFile = System.IO.Path.Combine(pop.TempPath, e.Attachments);
        foreach (KeyValuePair<string, byte[]> kvp in e.Attachments)
        {
          nTables++;
          //ds = SaveToTemplateSP(pop.TempPath, kvp.Value)

          dataTab = SaveToTemplateSP(pop.TempPath, kvp.Value);
          

          if ((dataTab != null) && (dataTab.Rows.Count > 0))
          {
            dataTab.TableName = nTables.ToString();
            ds.Tables.Add(dataTab);
          }
        }

        //if ((ds != null) && (ds.Tables.Count > 0))
        //{
        //  PopulateDoHeaderDetailSP(ds, db);

        //  db.SubmitChanges();
        //}

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

    private void pop3_ClassNotify(object sender, ScmsMailLibrary.Global.ClassNotifyEventArgs e)
    {
      switch (e.TypeMessage)
      {
        case ScmsMailLibrary.Global.ClassNotifyEventArgs.TypeEnum.IsError:
          Logger.WriteLine("Error : {0}", e.Message);
          break;
        case ScmsMailLibrary.Global.ClassNotifyEventArgs.TypeEnum.IsInformation:
          Logger.WriteLine("Info : {0}", e.Message);
          break;
        case ScmsMailLibrary.Global.ClassNotifyEventArgs.TypeEnum.IsNotify:
          Logger.WriteLine("Notify : {0}", e.Message);
          break;
        default:
          Logger.WriteLine("-> : {0}", e.Message);
          break;
      }
    }

    static System.Data.DataTable SaveToTemplateSP(string pathFolder, byte[] dataFile)
    {
      if (string.IsNullOrEmpty(pathFolder) || (!System.IO.Directory.Exists(pathFolder)) || (dataFile == null) || (dataFile.Length < 1))
      {
        return null;
      }

      System.Data.DataSet dataSet = new System.Data.DataSet();
      System.Data.DataTable tableSP = null;

      #region Zip Operation

      System.IO.FileStream fs = null;
      string fName = null,
        sourceFile = null;

      try
      {

        do
        {
          fName = string.Concat(Functionals.GeneratedRandomUniqueId(10, "sp"), ".dbf");

          sourceFile = System.IO.Path.Combine(pathFolder, fName);

        } while (System.IO.File.Exists(sourceFile));

        if (System.IO.File.Exists(sourceFile))
        {
          System.IO.File.Delete(sourceFile);
        }

        //System.IO.FileStream _FileStream = new System.IO.FileStream(sourceFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
        //_FileStream.Write(dataFile, 0, dataFile.Length);

        fs = new System.IO.FileStream(sourceFile, System.IO.FileMode.Create);

        fs.Write(dataFile, 0, dataFile.Length);
      }
      catch (Exception ex)
      {
        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);

        return null;
      }
      finally
      {
        if (fs != null)
        {
          fs.Close();
          fs.Dispose();
        }
      }

      #endregion
      
      //fName = System.IO.Path.Combine(pathFolder, DEFAULT_NAME_FILE_HEADER);
      //tableHeader = ScmsMailLibrary.Core.Commons.ReadDbfDatabase(fName, DEFAULT_NAME_TABEL_HEADER);

      tableSP = ScmsMailLibrary.Core.Commons.ReadDbfDatabase(sourceFile, pathFolder);

      //if (tableSP != null)
      //{
      //  dataSet.Tables.Add(tableSP);
      //}

      return tableSP;
    }

    //private void PopulateDoHeaderDetailSP(System.Data.DataSet dataset, ScmsModel.ORMDataContext db, bool isContexted)
    //{
    //  if ((dataset == null))
    //  {
    //    return;
    //  }

    //  List<LG_SPH> lstSP = null;
    //  LG_SPH sph = null;
    //  List<LG_SPD1> ListSPD1 = null;
    //  List<LG_SPD2> ListSPD2 = null;

    //  //LG_SPD2 spd2 = null;

    //  System.Data.DataTable table = null;
    //  string Type = null, 
    //    spID = null,
    //    Item = null,
    //    kd = null,
    //    spCab = null, sCabang = null,
    //    SingleCabang = null;
    //  bool hasChanges = false;
    //  System.Data.DataRow row = null,
    //     sRow = null;
    //  string[] SPNo = null;
    //  DateTime date = DateTime.Today,
    //    spDate = DateTime.MinValue;

    //  int nLoop = 0, 
    //    nQty = 0,
    //    nLoopC= 0 ,
    //    nTotal = 0;
    //  ListSPD1 = new List<LG_SPD1>();
    //  ListSPD2 = new List<LG_SPD2>();
    //  sph = new LG_SPH(); 
      
    //  if (dataset.Tables != null)
    //  {
    //      try
    //      {
    //          if (!isContexted)
    //          {
    //              db.Connection.Open();
    //              db.Transaction = db.Connection.BeginTransaction();

    //              SysNo sysNum = (from q in db.SysNos
    //                              where q.c_portal == '3' && q.c_type == "07"
    //                                && q.s_tahun == DateTime.Now.Year
    //                              select q).SingleOrDefault();

    //              for (nLoopC = 0; nLoopC < dataset.Tables.Count; nLoopC++)
    //              {
    //                  table = dataset.Tables[nLoopC];

    //                  lstSP = new List<LG_SPH>();

    //                  sRow = null;

    //                  spID = GenerateNumbering<LG_SPH>(db, "SP", '3', "07", date, "c_spno");

    //                  sRow = table.Rows[0];

    //                  for (nLoop = 0; nLoop < table.Rows.Count; nLoop++)
    //                  {
    //                      row = table.Rows[nLoop];

    //                      //nQty = row.GetValue<int>("n_adjust", 0) + row.GetValue<int>("n_order", 0);

    //                      if (row.GetValue<string>("c_pareto").ToString().Substring(1, 1).Equals("M"))
    //                      {
    //                          nQty = row.GetValue<int>("n_adjust", 0);
    //                          Type = "02";
    //                      }
    //                      else
    //                      {
    //                          if (row.GetValue<int>("n_adjust", 0) < 0)
    //                              nQty = row.GetValue<int>("n_order", 0) + row.GetValue<int>("n_adjust", 0);
    //                          else
    //                              nQty = row.GetValue<int>("n_order", 0);

    //                          Type = "01";
    //                      }

    //                      if (nQty > 0)
    //                      {
    //                          kd = row.GetValue<string>("c_kdcab", string.Empty);

    //                          var Cab = (from q in db.LG_Cusmas
    //                                     where q.c_cab == kd
    //                                     select q).SingleOrDefault();

    //                          DateTime Now = DateTime.Now;

    //                          SPNo = new string[] { row.GetValue<string>("c_nosp", string.Empty),
    //                                  row.GetValue<string>("c_pareto", string.Empty), Cab.c_cusno,
    //                                  row.GetValue<string>("d_tglsp", string.Empty)};

    //                          Item = row.GetValue<string>("c_iteno", string.Empty);

    //                          decimal nSalpri = (from q in db.FA_MasItms
    //                                             where q.c_iteno == Item
    //                                             select
    //                                               (q.n_salpri.HasValue ? q.n_salpri.Value : 0)
    //                                              ).Take(1).SingleOrDefault();

    //                          var Disc = (from q in db.FA_DiscDs
    //                                      where q.c_iteno == Item && q.c_nodisc == "DSXXXXXX03"
    //                                      select q).SingleOrDefault();

    //                          ListSPD1.Add(new LG_SPD1()
    //                          {
    //                              c_iteno = Item,
    //                              c_spno = spID,
    //                              c_type = "01",
    //                              n_acc = nQty,
    //                              n_qty = nQty,
    //                              n_sisa = nQty,
    //                              n_salpri = nSalpri,
    //                              v_ket = ""
    //                          });

    //                          ListSPD2.Add(new LG_SPD2()
    //                          {
    //                              c_iteno = Item,
    //                              c_spno = spID,
    //                              c_type = "03",
    //                              c_no = Disc == null ? string.Empty : Disc.c_nodisc,
    //                              n_discoff = Disc == null ? 0 : Disc.n_discoff,
    //                              n_discon = Disc == null ? 0 : Disc.n_discon,
    //                          });

    //                          spCab = row.GetValue<string>("c_nosp") == null ? string.Empty : row.GetValue<string>("c_nosp").ToString();
    //                      }
    //                  }

    //                  if ((ListSPD1 != null) && (ListSPD1.Count > 0))
    //                  {
    //                      sCabang = sRow.ItemArray[0].ToString();

    //                      spDate = DateTime.Parse(sRow.ItemArray[23].ToString());

    //                      if (spDate != DateTime.MinValue)
    //                      {
    //                          SingleCabang = (from q in db.LG_Cusmas
    //                                          where q.c_cab == sCabang
    //                                          select q.c_cusno).Take(1).SingleOrDefault();

    //                          lstSP = (from q in db.LG_SPHs
    //                                   where q.c_sp == spCab && q.c_cusno == SingleCabang
    //                                   select q).ToList();

    //                          if (lstSP != null && lstSP.Count <= 0)
    //                          {
    //                              sph = new LG_SPH()
    //                              {
    //                                  c_sp = spCab,
    //                                  c_type = Type,
    //                                  c_cusno = SingleCabang,
    //                                  c_entry = Constant.SYSTEM_USERNAME,
    //                                  c_spno = spID,
    //                                  c_update = Constant.SYSTEM_USERNAME,
    //                                  d_entry = DateTime.Now,
    //                                  d_spdate = spDate,
    //                                  d_spinsert = DateTime.Now,
    //                                  d_update = DateTime.Now,
    //                                  l_cek = false,
    //                                  l_delete = false,
    //                                  l_print = false,
    //                                  v_ket = ""
    //                              };

    //                              nTotal++;
    //                          }
    //                      }
    //                  }

    //                  if (nTotal > 0)
    //                  {
    //                      if (ListSPD1.Count > 0)
    //                      {
    //                          db.LG_SPD1s.InsertAllOnSubmit(ListSPD1.ToArray());
    //                          ListSPD1.Clear();
    //                      }
    //                      if (ListSPD2.Count > 0)
    //                      {
    //                          db.LG_SPD2s.InsertAllOnSubmit(ListSPD2.ToArray());
    //                          ListSPD2.Clear();
    //                      }
    //                      if (sph != null)
    //                      {
    //                          db.LG_SPHs.InsertOnSubmit(sph);

    //                          if (!isContexted)
    //                          {
    //                              string tmpNum = spID.Substring(6, 4);
    //                              switch (DateTime.Now.Month)
    //                              {
    //                                  case 1: sysNum.c_bln01 = tmpNum; break;
    //                                  case 2: sysNum.c_bln02 = tmpNum; break;
    //                                  case 3: sysNum.c_bln03 = tmpNum; break;
    //                                  case 4: sysNum.c_bln04 = tmpNum; break;
    //                                  case 5: sysNum.c_bln05 = tmpNum; break;
    //                                  case 6: sysNum.c_bln06 = tmpNum; break;
    //                                  case 7: sysNum.c_bln07 = tmpNum; break;
    //                                  case 8: sysNum.c_bln08 = tmpNum; break;
    //                                  case 9: sysNum.c_bln09 = tmpNum; break;
    //                                  case 10: sysNum.c_bln10 = tmpNum; break;
    //                                  case 11: sysNum.c_bln11 = tmpNum; break;
    //                                  case 12: sysNum.c_bln12 = tmpNum; break;
    //                              }

    //                              db.SubmitChanges();
    //                              //db.Transaction.Commit();
    //                              //db.Dispose();
    //                          }
    //                          else
    //                          {
    //                              db.Transaction.Rollback();
    //                          }

    //                          //db.SubmitChanges();
    //                          //db.Dispose();
    //                      }

    //                  }
    //                  else
    //                  {
    //                      if (ListSPD1 != null && ListSPD1.Count > 0)
    //                      {
    //                          ListSPD1.Clear();
    //                      }

    //                      if (ListSPD2 != null && ListSPD2.Count > 0)
    //                      {
    //                          ListSPD2.Clear();
    //                      }
    //                  }
    //              }
    //              db.Transaction.Commit();
    //              db.Dispose();
    //          }
    //      }
    //      catch (Exception ex)
    //      {
    //          if (db.Transaction != null)
    //          {
    //              db.Transaction.Rollback();
    //          }
    //      }
    //  }

      #region Old Code

      //if ((dataset == null))
      //{
      //  return 0;
      //}

      //string tmp = null, 
      //  Type = null,
      //  item = null,
      //  ket = null; ;
      //int nLoop = 0, nChanges = 0;
      //System.Data.DataTable table = null;
      //string[] SPH = null;
      //System.Data.DataRow row = null;
      //decimal nQty = 0,
      //  nAcc = 0;
      //bool isNew = false,
      //isVoid = false,
      //isModify = false;
      //string varData = null;

      //IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
      //Dictionary<string, string> dicData = null;
      //PostDataParser parser = new PostDataParser();
      //PostDataParser.StructurePair pair = new PostDataParser.StructurePair();
      //Dictionary<string, string> dicAttr = null;

      //if (dataset.Tables != null)
      //{
      //  table = dataset.Tables[0];

      //  for (nLoop = 0; nLoop < table.Rows.Count; nLoop++)
      //  {
      //    tmp = nLoop.ToString();

      //    row = table.Rows[nLoop];

      //    isNew = true;
      //    isVoid = false;
      //    isModify = false;


      //    dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      //    dicAttr.Add("New", isNew.ToString().ToLower());
      //    dicAttr.Add("Delete", isVoid.ToString().ToLower());
      //    dicAttr.Add("Modified", isModify.ToString().ToLower());

      //    item = row.GetValue<string>("c_iteno", string.Empty); ;
      //    nQty = nQty = row.GetValue<int>("n_adjust", 0) + row.GetValue<int>("n_onorder", 0);
      //    nAcc = nQty = row.GetValue<int>("n_adjust", 0) + row.GetValue<int>("n_onorder", 0);
      //    ket = string.Empty;

      //    if ((!string.IsNullOrEmpty(item)) &&
      //      (nQty > 0) &&
      //      (nAcc > 0))
      //    {
      //      dicAttr.Add("Item", item);
      //      dicAttr.Add("Qty", nQty.ToString());
      //      dicAttr.Add("Acc", nAcc.ToString());

      //      pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
      //      {
      //        IsSet = true,
      //        Value = (string.IsNullOrEmpty(ket) ? string.Empty : ket.Trim()),
      //        DicAttributeValues = dicAttr
      //      });
      //    }
          
      //  }
      //  if (SPH[1].Substring(1, 1).Equals("M"))
      //  {
      //    Type = "02";
      //  }
      //  else
      //  {
      //    Type = "01";
      //  }

      //  dic.Add("ID", pair);
      //  pair.DicAttributeValues.Add("Entry", "sa");
      //  pair.DicAttributeValues.Add("Customer", SPH[2]);
      //  pair.DicAttributeValues.Add("Tanggal", Convert.ToDateTime(SPH[3]).ToString("yyyyMMddHHmmssfff"));
      //  pair.DicAttributeValues.Add("SPCabang", SPH[2].ToString().Trim());
      //  pair.DicAttributeValues.Add("Keterangan", string.Empty);
      //  pair.DicAttributeValues.Add("Tipe", Type);
      //  pair.DicAttributeValues.Add("Cek", false.ToString().ToLower());
      //}

      //varData = parser.ParserData("SuratPesanan", "Add", dic);

      //return nChanges;

      #endregion
    //}
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
                                        c_entry = "sys1",
                                        c_spno = spID,
                                        c_update = "sys1",
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
  }
}
