using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsModel;
using ScmsSoaLibrary.Parser;
//using ScmsSoaLibrary.Core.Crypto;
using ScmsSoaLibrary.Commons;
using System.Data.Common;
using ScmsSoaLibrary.Core.Threading;
using ScmsModel.Core;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaLibrary.Bussiness
{
  class Commons
  {
    #region Internal Class

    internal class GenerateMasterNum
    {
      public string c_number  {get; set;}
    }
    
    internal class FakturPajakInformation
    {
      public string NoFakturPajak { get; set; }
      public string NoFaktur { get; set; }
      public DateTime TanggalFakturPajak { get; set; }
      public bool IsUsed { get; set; }
    }

    #endregion

    public static bool HasOrderOrPacking(ORMDataContext db, string pesananId)
    {
      int nCount = (from q in db.LG_PLHs
                    join q1 in db.LG_PLD1s on q.c_plno equals q1.c_plno
                    where (q1.c_spno == pesananId)
                      //Production 
                      //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                    select q.c_plno).Count();

      if (nCount > 0)
      {
        return false;
      }

      nCount = (from q in db.LG_ORHs
                join q1 in db.LG_ORD2s on new { q.c_gdg, q.c_orno } equals new { q1.c_gdg, q1.c_orno }
                where (q1.c_spno == pesananId)
                  && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                select q.c_orno).Count();

      return (nCount > 0);
    }

    public static bool HasDO(ORMDataContext db, string packingId)
    {
      int nCount = (from q in db.LG_DOHs
                    join q1 in db.LG_DOD1s on q.c_dono equals q1.c_dono
                    where (q.c_plno == packingId)
                      //Production 
                      //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                    select q.c_dono).Count();
      
      return (nCount > 0);
    }

    public static bool HasFB(ORMDataContext db, string receiveId)
    {
      int nCount = (from q in db.LG_FBHs
                    join q1 in db.LG_FBD2s on q.c_fbno equals q1.c_fbno
                    where (q1.c_rnno == receiveId)
                      //Production
                      //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                    select q.c_fbno).Count();

      return (nCount > 0);
    }

    public static bool HasFJ(ORMDataContext db, string deliveryId)
    {
      int nCount = (from q in db.LG_FJHs
                    join q1 in db.LG_FJD3s on q.c_fjno equals q1.c_fjno
                    where (q1.c_dono == deliveryId)
                      //Production
                      //&& ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                    select q.c_fjno).Count();

      return (nCount > 0);
    }

    public static bool HasFJExists(ORMDataContext db, string fakturId)
    {
      int nCount = (from q in db.LG_FJHs
                    where (q.c_fjno == fakturId)
                    select q.c_fjno).Count();

      return (nCount > 0);
    }

    public static bool HasFBR(ORMDataContext db, string returID)
    {
      int nCount = (from q in db.LG_FBRHs
                    join q1 in db.LG_FBRD2s on q.c_fbno equals q1.c_fbno
                    where (q1.c_rsno == returID)
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                    select q.c_fbno).Count();

      return (nCount > 0);
    }

    public static bool HasFJR(ORMDataContext db, string returID)
    {
      int nCount = (from q in db.LG_FJRHs
                    join q1 in db.LG_FJRD3s on q.c_fjno equals q1.c_fjno
                    where q1.c_rcno == returID
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                    select q.c_fjno).Count();

      return (nCount > 0);
    }

    public static bool HasDOSTT(ORMDataContext db, string sttNO)
    {
      int nCount = (from q in db.LG_DOHs
                    join q1 in db.LG_DOD1s on q.c_dono equals q1.c_dono
                    where q.c_plno == sttNO
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                    select q.c_dono).Count();

      return (nCount > 0);

      #region Old Coded

      //string tmp = (from q in db.LG_DOHs
      //              where q.c_plno == sttNO && q.l_delete == false
      //              select q.c_dono).Take(1).SingleOrDefault();


      //return (!string.IsNullOrEmpty(tmp));

      #endregion
    }

    public static bool HasExp(ORMDataContext db, string doID)
    {
      int nCount = (from q in db.LG_ExpHs
                   join q1 in db.LG_ExpDs on q.c_expno equals q1.c_expno
                   where (q1.c_dono == doID)
                    && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                   select q.c_expno).Count();

      return (nCount > 0);

      #region Old Coded

      //string tmp = (from q in db.LG_ExpDs
      //              join q1 in db.LG_ExpHs on q.c_expno equals q1.c_expno
      //              where q.c_dono == doID
      //                && ((q1.l_delete.HasValue ? q1.l_delete.Value : false) == false)
      //              select q.c_dono).Take(1).SingleOrDefault();


      //return (!string.IsNullOrEmpty(tmp));

      #endregion
    }

    public static bool IsClosingFA(ORMDataContext db, DateTime? tanggal)
    {
      DateTime date = (tanggal.HasValue ? tanggal.Value : Functionals.StandardSqlDateTime);

      return IsClosingFA(db, date);
    }

    public static bool IsClosingFA(ORMDataContext db, DateTime tanggal)
    {
      //int cnt = (from q in db.Closings
      //           where q.c_type == "03" && q.l_close == true && q.s_tahun == tanggal.Year && q.t_bulan == tanggal.Month
      //           select q).Count();

      //int cnt = (from q in db.Closings
      //           where q.c_type == "03" && q.l_close == true
      //             && q.s_tahun >= 2011 && q.t_bulan >= 9
      //           orderby q.s_tahun descending, q.t_bulan descending
      //           select q).Take(1).Count();

      int cnt = (from q in db.Closings
                 where (q.c_type == "03")
                  && (Convert.ToDateTime((q.s_tahun.ToString() + "-" + q.t_bulan.ToString() + "-" + "01")) >= tanggal)
                  && (q.l_close == true)
                 select q).Count();

      return (cnt > 0);
    }

    public static bool IsClosingLogistik(ORMDataContext db, DateTime? tanggal)
    {
      DateTime date = (tanggal.HasValue ? tanggal.Value : Functionals.StandardSqlDateTime);

      return IsClosingLogistik(db, date);
    }

    public static bool IsClosingLogistik(ORMDataContext db, DateTime tanggal)
    {
      int cnt = (from q in db.Closings
                 where (q.c_type == "01")
                  && (Convert.ToDateTime((q.s_tahun.ToString() + "-" + q.t_bulan.ToString() + "-" + "01")) >= tanggal)
                  && (q.l_close == true)
                 select q).Count();

      return (cnt > 0);
    }

    public static bool IsORKhusus(ORMDataContext db, string sPO)
    {
      string sOrno = (from q in db.LG_POD2s
                     where q.c_pono == sPO
                     select q.c_orno).Take(1).SingleOrDefault();

      bool isKhusus = false;
      string sORKhusus = null;

      bool isPharma = sOrno.Substring(0, 2) == "SP" ? true : false;

      if (!string.IsNullOrEmpty(sOrno))
      {
        if (isPharma)
        {
          sORKhusus = (from q in db.LG_SPHs
                       where q.c_spno == sOrno && q.c_type == "06"
                       select q.c_spno).Take(1).SingleOrDefault();
        }
        else
        {
           sORKhusus = (from q in db.LG_ORHs
                              where q.c_orno == sOrno && q.c_type == "02"
                              select q.c_orno).Take(1).SingleOrDefault();
        }

        if (!string.IsNullOrEmpty(sORKhusus))
        {
          isKhusus = true;
        }
        
      }

      return isKhusus;
    }

    public static int ModifyReceiveNote(ORMDataContext db, char gdg, string rn, string item, string batch, decimal goodQty, decimal badQty, string nipUser)
    {
      int nCnt = 0;

      //LG_RND1 rnd1 = db.LG_RND1s.re

      return nCnt;
    }

    public static string GenerateNumberingFormat<T>(ORMDataContext db, string headerCode, char portalKode, string tipeKode, DateTime tanggalAktif, string fieldCondition, string Mode)
    {
      string result = string.Empty;

      //db.GetTable<T>().Where(

      int nCount = 0,
        nValue = 0;
      string tmpNum = null,
        hdrValue = null,
        tVal = null;
      char chr = char.MinValue;

      SysNo sysNum = (from q in db.SysNos
                      where q.c_portal == portalKode && q.c_type == tipeKode
                        && q.s_tahun == tanggalAktif.Year
                      select q).Take(1).SingleOrDefault();

      if (sysNum != null)
      {
        switch (tanggalAktif.Month)
        {
          case 1: tmpNum = (sysNum.c_bln01 ?? string.Empty); break;
          case 2: tmpNum = (sysNum.c_bln02 ?? string.Empty); break;
          case 3: tmpNum = (sysNum.c_bln03 ?? string.Empty); break;
          case 4: tmpNum = (sysNum.c_bln04 ?? string.Empty); break;
          case 5: tmpNum = (sysNum.c_bln05 ?? string.Empty); break;
          case 6: tmpNum = (sysNum.c_bln06 ?? string.Empty); break;
          case 7: tmpNum = (sysNum.c_bln07 ?? string.Empty); break;
          case 8: tmpNum = (sysNum.c_bln08 ?? string.Empty); break;
          case 9: tmpNum = (sysNum.c_bln09 ?? string.Empty); break;
          case 10: tmpNum = (sysNum.c_bln10 ?? string.Empty); break;
          case 11: tmpNum = (sysNum.c_bln11 ?? string.Empty); break;
          case 12: tmpNum = (sysNum.c_bln12 ?? string.Empty); break;
          default: tmpNum = null; break;
        }

        if (!string.IsNullOrEmpty(tmpNum))
        {
          tmpNum = (string.IsNullOrEmpty(tmpNum) ? string.Empty : tmpNum.Trim());

          hdrValue = (string.IsNullOrEmpty(headerCode) ? "__" :
            ((headerCode.Length > 1) ? headerCode.PadLeft(2, '_') : headerCode.Substring(0, 2)));

          do
          {
            if (tmpNum.Length > 1)
            {
              if (!char.IsNumber(tmpNum, 0))
              {
                tVal = tmpNum.Substring(1);

                if (!int.TryParse(tVal, out nValue))
                {
                  result = string.Empty;

                  goto endLogic;
                }

                nValue++;

                if (nValue > 999)
                {
                  chr = tmpNum[0];
                  chr++;

                  if (chr > 0x60)
                  {
                    chr = (char)0x7b;
                  }

                  tmpNum = string.Concat(chr, "001");
                }
                else
                {
                  tmpNum = string.Concat(tmpNum[0], nValue.ToString().PadLeft(3, '0'));
                }
              }
              else
              {
                if (!int.TryParse(tmpNum, out nValue))
                {
                  result = string.Empty;

                  goto endLogic;
                }

                nValue++;

                if (nValue > 9999)
                {
                  tmpNum = "A001";
                }
                else
                {
                  tmpNum = nValue.ToString().PadLeft(4, '0');
                }
              }
            }
            else
            {
              tmpNum = "0001";
            }

            switch(Mode)
            {
              case "claim":
                {
                  result = string.Concat(tmpNum, hdrValue, tanggalAktif.ToString("MM") + "/", tanggalAktif.ToString("yyyy"));
                }
                break;
            }

            nCount = db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count();

          } while (nCount != 0);


          switch (tanggalAktif.Month)
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

        endLogic:
          ;
        }
      }

      return result;
    }

    public static string GenerateNumberingMaster(ORMDataContext db, string fieldCondition, string Mode)
    {
      string result = string.Empty;

      List<GenerateMasterNum> Numbering = null;
      int i = 0;

      try
      {
        switch(Mode)
        {
          case "c_nosup":
            {
              Numbering = (from q in db.LG_DatSups
                           select new GenerateMasterNum()
                           {
                             c_number = q.c_nosup
                           }).ToList();
            }
            break;
          case "c_kddivpri":
            {
              Numbering = (from q in db.FA_MsDivPris
                           select new GenerateMasterNum()
                           {
                             c_number = q.c_kddivpri
                           }).ToList();
            }
            break;
          case "c_cusno":
            {
              Numbering = (from q in db.LG_Cusmas
                           select new GenerateMasterNum()
                           {
                             c_number = q.c_cusno
                           }).ToList();
            }
            break;
          case "c_exp":
            {
              Numbering = (from q in db.LG_MsExps
                           select new GenerateMasterNum()
                           {
                             c_number = q.c_exp
                           }).ToList();
            }
            break;
		  case "c_kddivams":
            {
              Numbering = (from q in db.FA_MsDivAMs
                           select new GenerateMasterNum()
                           {
                             c_number = q.c_kddivams
                           }).ToList();
            }
            break;
          //case "c_type_trx":
          //  {
          //    Numbering = (from q in db.MsTransDs
          //                 select new GenerateMasterNum()
          //                 {
          //                   c_number = q.c_type
          //                 }).ToList();
          //  }
          //  break;
        }

        i = Convert.ToInt32(Numbering.Max(x => x.c_number)) + 1;

        result = i.ToString(fieldCondition);
      }
      catch(Exception ex)
      {
        string Err = ex.Message;

        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }

      //string a = i.ToString(fieldCondition);
      return result;
    }

    public static bool GenerateNumberingAutoNumber(ORMDataContext db, string headerCode, char portal, string tipeKode, DateTime date)
    {
      bool isOk = false;
      if (Constant.Gudang.ToString() == "?")
      {
          Constant.Gudang = '1';
      }
      sysnoNumber sysNumAut = new sysnoNumber();
      if (portal.ToString() == "3" && tipeKode == "07" && (Constant.Gudang.ToString() == "6" || Constant.Gudang.ToString() == "?"))
      {
          sysNumAut = (from q in db.sysnoNumbers
                       where q.c_portal == portal
                       && q.c_notrans == tipeKode
                         && q.s_tahun == date.Year
                         && q.c_gdg == '1'
                       select q).SingleOrDefault();
      }
      else
      {
          sysNumAut = (from q in db.sysnoNumbers
                       where q.c_portal == portal
                       && q.c_notrans == tipeKode
                         && q.s_tahun == date.Year
                         && q.c_gdg == Constant.Gudang
                       select q).SingleOrDefault();
      }
      string tmpNum = null;

      switch (date.Month)
      {
        case 1: tmpNum = (sysNumAut.c_bln01 ?? string.Empty); break;
        case 2: tmpNum = (sysNumAut.c_bln02 ?? string.Empty); break;
        case 3: tmpNum = (sysNumAut.c_bln03 ?? string.Empty); break;
        case 4: tmpNum = (sysNumAut.c_bln04 ?? string.Empty); break;
        case 5: tmpNum = (sysNumAut.c_bln05 ?? string.Empty); break;
        case 6: tmpNum = (sysNumAut.c_bln06 ?? string.Empty); break;
        case 7: tmpNum = (sysNumAut.c_bln07 ?? string.Empty); break;
        case 8: tmpNum = (sysNumAut.c_bln08 ?? string.Empty); break;
        case 9: tmpNum = (sysNumAut.c_bln09 ?? string.Empty); break;
        case 10: tmpNum = (sysNumAut.c_bln10 ?? string.Empty); break;
        case 11: tmpNum = (sysNumAut.c_bln11 ?? string.Empty); break;
        case 12: tmpNum = (sysNumAut.c_bln12 ?? string.Empty); break;
        default: tmpNum = null; break;
      }

      int iValue = 0;
      string sValue = null;
      isOk = int.TryParse(tmpNum, out iValue);

      if (isOk)
      {
        iValue += 1;

        int iGudang = int.Parse(Constant.Gudang.ToString());

        sValue = iValue.ToString("000000");
        Constant.NUMBERID_GUDANG = string.Concat(headerCode, date.Year.ToString().Substring(2, 2), date.Month.ToString("00"), iGudang.ToString("00"), iValue.ToString("000000"));

        switch (date.Month)
        {
          case 1: sysNumAut.c_bln01 = sValue; break;
          case 2: sysNumAut.c_bln02 = sValue; break;
          case 3: sysNumAut.c_bln03 = sValue; break;
          case 4: sysNumAut.c_bln04 = sValue; break;
          case 5: sysNumAut.c_bln05 = sValue; break;
          case 6: sysNumAut.c_bln06 = sValue; break;
          case 7: sysNumAut.c_bln07 = sValue; break;
          case 8: sysNumAut.c_bln08 = sValue; break;
          case 9: sysNumAut.c_bln09 = sValue; break;
          case 10: sysNumAut.c_bln10 = sValue; break;
          case 11: sysNumAut.c_bln11 = sValue; break;
          case 12: sysNumAut.c_bln12 = sValue; break;
        }
      }

      return isOk;
    }

    public static string GenerateNumbering<T>(ORMDataContext db, string headerCode, char portalKode, string tipeKode, DateTime tanggalAktif, string fieldCondition)
    {
      string result = string.Empty;

      //db.GetTable<T>().Where(
      int nCount = 0,
        nValue = 0,
        nCountTambah = 0;
      string tmpNum = null,
        hdrValue = null,
        tVal = null;
      char chr = char.MinValue;
      SysNo sysNum2 = (from q in db.SysNos
                       where q.c_portal == '3' && q.c_type == "12" && q.s_tahun == tanggalAktif.Year
                       select q).Take(1).SingleOrDefault();
      if (sysNum2.c_bln01 == "1")
      {
          sysNum2.c_bln01 = "2";
      }
      else
      {
          sysNum2.c_bln01 = "1";
      }
      db.SubmitChanges();
      SysNo sysNum = (from q in db.SysNos
                      where q.c_portal == portalKode && q.c_type == tipeKode
                        && q.s_tahun == tanggalAktif.Year
                      select q).Take(1).SingleOrDefault();
        
      if (sysNum != null)
      {
        switch (tanggalAktif.Month)
        {
          case 1: tmpNum = (sysNum.c_bln01 ?? string.Empty); break;
          case 2: tmpNum = (sysNum.c_bln02 ?? string.Empty); break;
          case 3: tmpNum = (sysNum.c_bln03 ?? string.Empty); break;
          case 4: tmpNum = (sysNum.c_bln04 ?? string.Empty); break;
          case 5: tmpNum = (sysNum.c_bln05 ?? string.Empty); break;
          case 6: tmpNum = (sysNum.c_bln06 ?? string.Empty); break;
          case 7: tmpNum = (sysNum.c_bln07 ?? string.Empty); break;
          case 8: tmpNum = (sysNum.c_bln08 ?? string.Empty); break;
          case 9: tmpNum = (sysNum.c_bln09 ?? string.Empty); break;
          case 10: tmpNum = (sysNum.c_bln10 ?? string.Empty); break;
          case 11: tmpNum = (sysNum.c_bln11 ?? string.Empty); break;
          case 12: tmpNum = (sysNum.c_bln12 ?? string.Empty); break;
          default: tmpNum = null; break;
        }
        if (!string.IsNullOrEmpty(tmpNum))
        {
          switch (headerCode)
          {
            case "PL":
                  GenerateNumberingAutoNumber(db, headerCode, portalKode, tipeKode, tanggalAktif);
              break;
            case "SP":
              
                GenerateNumberingAutoNumber(db, headerCode, portalKode, tipeKode, tanggalAktif);
              
              break;

          }

          tmpNum = (string.IsNullOrEmpty(tmpNum) ? string.Empty : tmpNum.Trim());

          hdrValue = (string.IsNullOrEmpty(headerCode) ? "__" :
            ((headerCode.Length > 1) ? headerCode.PadLeft(2, '_') : headerCode.Substring(0, 2)));

          do
          {
            if (tmpNum.Length > 1)
            {
              if (!char.IsNumber(tmpNum, 0))
              {
                tVal = tmpNum.Substring(1);

                if (!int.TryParse(tVal, out nValue))
                {
                  result = string.Empty;

                  goto endLogic;
                }

                nValue++;

                if (nValue > 999)
                {
                    if (tmpNum == "Z999")
                    {
                        tmpNum = "0001";
                    }
                    else
                    {
                        chr = tmpNum[0];
                        chr++;

                        if (chr > 0x60)
                        {
                            chr = (char)0x7b;
                        }

                        tmpNum = string.Concat(chr, "001");
                    }
                }
                else
                {
                  tmpNum = string.Concat(tmpNum[0], nValue.ToString().PadLeft(3, '0'));
                }
              }
              else
              {
                if (!int.TryParse(tmpNum, out nValue))
                {
                  result = string.Empty;

                  goto endLogic;
                }

                nValue++;

                if (nValue > 9999)
                {
                  tmpNum = "A001";
                }
                else
                {
                  tmpNum = nValue.ToString().PadLeft(4, '0');
                }
              }
            }
            else
            {
              tmpNum = "0001";
            }

            result = string.Concat(hdrValue, tanggalAktif.ToString("yyMM"), tmpNum);

            if (hdrValue == "PL")
            {
                var qry = (from q in db.LG_PLHs
                       where q.c_plno == string.Concat(hdrValue, tanggalAktif.ToString("yyMM"), "Z999")
                       select q).SingleOrDefault();
                if (qry != null)
                {
                    int tglbln = Convert.ToInt32(tanggalAktif.ToString("yyMM")) + 12;
                    result = string.Concat(hdrValue, tglbln.ToString(), tmpNum);
                }
            }
            else if (hdrValue == "DO")
            {
                var qry = (from q in db.LG_DOHs
                           where q.c_dono == string.Concat(hdrValue, tanggalAktif.ToString("yyMM"), "Z999")
                           select q).SingleOrDefault();
                if (qry != null)
                {
                    int tglbln = Convert.ToInt32(tanggalAktif.ToString("yyMM")) + 12;
                    result = string.Concat(hdrValue, tglbln.ToString(), tmpNum);
                }

            }
            nCount = db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count();

            if (nCount > 0)
            {
              string iNum = result.Substring(7, 3);
              string iTestNum = result.Substring(0, 6),
                 sNum = null;
              //string iNumSwi = ScmsSoaLibrary.Commons.Constant.TRANSID.Substring(6, 1);
              int Num = 0;

              bool f = int.TryParse(iTestNum, out Num);

              if (!f )
              {
                nCountTambah++;
                if ((iNum == "000"))
                {
                  iNum = "999";
                  tmpNum = "0001";
                }
              }

               nCount--;

                #region old
                //do
              //{
              //  switch (iNumSwi)
              //  {
              //    case "A":
              //      {
              //        result = string.Concat(iTestNum, iNumSwi, iNum);

              //        if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //        {
              //          iNumSwi = "B";
              //          iNum = "001";
              //          break;
                        
              //        }
              //        else
              //        {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);
              //          nCountTambah--;
              //          nCount--;
              //        }
              //      }
              //      break;
              //    case "B":
              //      {
              //        result = string.Concat(iTestNum, iNumSwi, iNum);

              //        if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //        {
              //          iNumSwi = "C";
              //          iNum = "001";
              //          break;
              //        }
              //        else
              //        {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);
              //          nCountTambah--;
              //          nCount--;
              //        }
              //      }
              //      break;
              //    case "C":
              //      {
              //        result = string.Concat(iTestNum, iNumSwi, iNum);

              //        if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //        {
              //          iNumSwi = "D";
              //          iNum = "001";
              //          break;
              //        }
              //        else
              //        {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);
              //          nCountTambah--;
              //          nCount--;
              //        }
              //      }
              //      break;
              //    case "D":
              //      {
              //        result = string.Concat(iTestNum, iNumSwi, iNum);

              //        if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //        {
              //          iNumSwi = "E";
              //          iNum = "001";
              //          break;
              //        }
              //        else
              //        {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);
              //          nCountTambah--;
              //          nCount--;
              //        }
              //      }
              //      break;
              //    case "E":
              //      {
              //        result = string.Concat(iTestNum, iNumSwi, iNum);

              //        if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //        {
              //          iNumSwi = "F";
              //          iNum = "001";
              //          break;
              //        }
              //        else
              //        {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);

              //          nCountTambah--;
              //          nCount--;
              //        }
              //      }
              //      break;
              //    case "F":
              //      {
              //        result = string.Concat(iTestNum, iNumSwi, iNum);

              //        if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //        {
              //          iNumSwi = "G";
              //          iNum = "001";
              //          break;
              //        }
              //        else
              //        {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);
              //          nCountTambah--;
              //          nCount--;
              //        }
              //      }
              //      break;
              //    case "G":
              //      {
              //        result = string.Concat(iTestNum, iNumSwi, iNum);

              //        if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //        {
              //          iNumSwi = "H";
              //          iNum = "001";
              //          break;
              //        }
              //        else
              //        {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);
              //          nCountTambah--;
              //          nCount--;
              //        }
              //      }
              //      break;
              //    case "H":
              //      {
              //        result = string.Concat(iTestNum, iNumSwi, iNum);

              //        if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //        {
              //          iNumSwi = "I";
              //          iNum = "001";
              //          break;
              //        }
              //        else
              //        {
              //          result = string.Concat(iTestNum, iNumSwi, Num);
              //          nCountTambah--;
              //          nCount--;
              //        }
              //      }
              //      break;
              //    case "I":
              //      {
              //        result = string.Concat(iTestNum, iNumSwi, iNum);

              //        if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //        {
              //          iNumSwi = "J";
              //          iNum = "001";
              //          break;
              //        }
              //        else
              //        {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);
              //          nCountTambah--;
              //          nCount--;
              //        }
              //      }
              //      break;
              //    case "J":
              //      {
              //        result = string.Concat(iTestNum, iNumSwi, iNum);

              //        if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //        {
              //          iNumSwi = "K";
              //          iNum = "001";
              //          break;
              //        }
              //        else
              //        {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);
              //          nCountTambah--;
              //          nCount--;
              //        }
              //      }
              //      break;
              //    case "K":
              //      {
              //        result = string.Concat(iTestNum, iNumSwi, iNum);

              //        if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //        {
              //          iNumSwi = "L";
              //          iNum = "001";
              //          break;
              //        }
              //        else
              //        {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);
              //          nCountTambah--;
              //          nCount--;
              //        }
              //      }
              //      break;
              //    case "L":
              //      {
              //        result = string.Concat(iTestNum, iNumSwi, iNum);

              //        if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //        {
              //          iNumSwi = "M";
              //          iNum = "001";
              //          break;
              //        }
              //        else
              //        {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);
              //          nCountTambah--;
              //          nCount--;
              //        }
              //      }
              //      break;
              //    case "M":
              //      {
              //        result = string.Concat(iTestNum, iNumSwi, iNum);

              //        if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //        {
              //          iNumSwi = "N";
              //          iNum = "001";
              //          break;
              //        }
              //        else
              //        {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);
              //          nCountTambah--;
              //          nCount--;
              //        }
              //      }
              //      break;
              //    case "N":
              //      {
              //        result = string.Concat(iTestNum, iNumSwi, iNum);

              //        if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //        {
              //          iNumSwi = "O";
              //          iNum = "001";
              //          break;
              //        }
              //        else
              //        {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);
              //          nCountTambah--;
              //          nCount--;
              //        }
              //      }
              //      break;
              //    case "O":
              //      {
              //        result = string.Concat(iTestNum, iNumSwi, iNum);

              //        if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //        {
              //          iNumSwi = "P";
              //          iNum = "001";
              //          break;
              //        }
              //        else
              //        {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);
              //          nCountTambah--;
              //          nCount--;
              //        }
              //      }
              //      break;
              //    case "P":
              //      {
              //        result = string.Concat(iTestNum, iNumSwi, iNum);

              //        if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //        {
              //          iNumSwi = "Q";
              //          iNum = "001";
              //          break;
              //        }
              //        else
              //        {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);
              //          nCountTambah--;
              //          nCount--;
              //        }
              //      }
              //      break;
              //    case "Q":
              //      {
              //        result = string.Concat(iTestNum, iNumSwi, iNum);

              //        if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //        {
              //          iNumSwi = "R";
              //          iNum = "001";
              //          break;
              //        }
              //        else
              //        {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);
              //          nCountTambah--;
              //          nCount--;
              //        }
              //      }
              //      break;
              //    case "R":
              //      {
              //        result = string.Concat(iTestNum, iNumSwi, iNum);

              //        if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //        {
              //          iNumSwi = "S";
              //          iNum = "001";
              //          break;
              //        }
              //        else
              //        {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);
              //          nCountTambah--;
              //          nCount--;
              //        }
              //      }
              //      break;
              //    case "S":
              //      {
              //        result = string.Concat(iTestNum, iNumSwi, iNum);

              //        if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //        {
              //          iNumSwi = "T";
              //          iNum = "001";
              //          break;
              //        }
              //        else
              //        {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);
              //          nCountTambah--;
              //          nCount--;
              //        }
              //      }
              //      break;
              //    case "T":
              //      {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);

              //          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //          {
              //              iNumSwi = "U";
              //              iNum = "001";
              //              break;
              //          }
              //          else
              //          {
              //              result = string.Concat(iTestNum, iNumSwi, iNum);
              //              nCountTambah--;
              //              nCount--;
              //          }
              //      }
              //      break;
              //    case "U":
              //      {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);

              //          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //          {
              //              iNumSwi = "V";
              //              iNum = "001";
              //              break;
              //          }
              //          else
              //          {
              //              result = string.Concat(iTestNum, iNumSwi, iNum);
              //              nCountTambah--;
              //              nCount--;
              //          }
              //      }
              //      break;
              //    case "V":
              //      {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);

              //          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //          {
              //              iNumSwi = "W";
              //              iNum = "001";
              //              break;
              //          }
              //          else
              //          {
              //              result = string.Concat(iTestNum, iNumSwi, iNum);
              //              nCountTambah--;
              //              nCount--;
              //          }
              //      }
              //      break;
              //    case "W":
              //      {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);

              //          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //          {
              //              iNumSwi = "X";
              //              iNum = "001";
              //              break;
              //          }
              //          else
              //          {
              //              result = string.Concat(iTestNum, iNumSwi, iNum);
              //              nCountTambah--;
              //              nCount--;
              //          }
              //      }
              //      break;
              //    case "X":
              //      {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);

              //          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //          {
              //              iNumSwi = "Y";
              //              iNum = "001";
              //              break;
              //          }
              //          else
              //          {
              //              result = string.Concat(iTestNum, iNumSwi, iNum);
              //              nCountTambah--;
              //              nCount--;
              //          }
              //      }
              //      break;
              //    case "Y":
              //      {
              //          result = string.Concat(iTestNum, iNumSwi, iNum);

              //          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //          {
              //              iNumSwi = "Z";
              //              iNum = "001";
              //              break;
              //          }
              //          else
              //          {
              //              result = string.Concat(iTestNum, iNumSwi, iNum);
              //              nCountTambah--;
              //              nCount--;
              //          }
              //      }
              //      break;
              //  }
              //} while (nCountTambah != 0);

              //if (hdrValue == "PL")
              //{
              //  result = string.Concat(iTestNum, "E", Num);

              //  if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //  {
              //    result = string.Concat(iTestNum, "F", Num);

              //    if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
              //    {
              //      result = string.Concat(iTestNum, "G", Num);

              //      nCount -= 1;
              //    }
              //    else
              //    {
              //      nCount -= 1;
              //    }
              //  }
              //  else
              //  {
              //    nCount -= 1;
              //  }
              //}
              //if (hdrValue == "DO")
              //{
              //  result = string.Concat(iTestNum, "C", Num);

              //  nCount -= 1;
                //}
                #endregion
            }

          } while (nCount != 0);


          switch (tanggalAktif.Month)
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

          //db.SubmitChanges();

        endLogic:
          ;
        }
      }
      db.SubmitChanges();

      return result;
    }

    public static string IncrementNumbering(string identId, int nextStep)
    {
      string result = string.Empty;



      return result;
    }

    public static bool HasPOSend(ORMDataContext db, char gudang, string poID)
    {
      return ((from q in db.LG_POHs
               where q.c_pono == poID && q.c_gdg == gudang 
                && ((q.l_send == true) && (q.l_print == true))
               select q.c_pono).Count() > 0);
    }

    public static bool HasPO(ORMDataContext db, string orderId)
    {
      return ((from q in db.LG_POD2s
               join q1 in db.LG_POHs on q.c_pono equals q1.c_pono into q_1
               from qPOH in q_1.DefaultIfEmpty()
               where (q.c_orno == orderId) && ((qPOH.l_delete == null) || (qPOH.l_delete == false))
               select q.c_pono).Count() > 0);
    }

    public static bool PO_BudgetLimit(ORMDataContext db, string suplId, string pono, DateTime date, decimal bilva, string nipEntry, bool isDelete)
    {
      return PO_BudgetLimit(db, suplId, pono, date, bilva, nipEntry, isDelete, null);
    }

    public static bool PO_BudgetLimit(ORMDataContext db, string suplId, string pono, DateTime date, decimal bilva, string nipEntry, bool isDelete, string ketDelete)
    {
      bool bRes = false;

      decimal prevBil = 0,
        prevLimit = 0,
        sisaLimit = 0;

      DateTime dateNow = DateTime.Now;

      LG_DatSupServiceD dsd = null;
      LG_DatSupServiceH dsh = (from q in db.LG_DatSupServiceHs
                               where (q.c_nosup == suplId)
                                && (q.n_tahun == date.Year) && (q.n_bulan == date.Month)
                               select q).Take(1).SingleOrDefault();

      if (dsh != null)
      {
        prevLimit = (dsh.avaiblelimit.HasValue ? dsh.avaiblelimit.Value : 0);

        if (prevLimit > 0)
        {
          dsd = (from q in db.LG_DatSupServiceDs
                 where (q.c_nosup == suplId)
                  && (q.n_tahun == date.Year) && (q.n_bulan == date.Month)
                  && (q.c_pono == pono)
                 select q).Take(1).SingleOrDefault();

          if (dsd == null)
          {
            if (prevLimit >= bilva)
            {
              dsd = new LG_DatSupServiceD()
              {
                c_nosup = suplId,
                n_tahun = date.Year,
                n_bulan = date.Month,
                c_pono = pono,
                c_entry = nipEntry,
                d_entry = date,
                c_update = string.Empty,
                d_update = Functionals.StandardSqlDateTime,
                n_bilva = bilva,
                l_delete = false,
                v_ket = string.Empty
              };

              db.LG_DatSupServiceDs.InsertOnSubmit(dsd);

              dsh.avaiblelimit -= bilva;

              bRes = true;
            }
            else
            {
              bRes = false;
            }
          }
          else
          {
            prevBil = (dsd.n_bilva.HasValue ? dsd.n_bilva.Value : 0);

            //subBil = (bilva - prevBil);
            prevLimit = (prevBil - bilva);
            sisaLimit = ((dsh.avaiblelimit.HasValue ? dsh.avaiblelimit.Value : 0) + prevLimit);

            if (isDelete)
            {
              dsd.c_update = nipEntry;
              dsd.d_update = dateNow;
              dsd.v_ket = (string.IsNullOrEmpty(ketDelete) ? 
                string.Concat("Delete @ ", dateNow.ToString("dd-MM-yyyy")) : ketDelete);
              dsd.l_delete = true;

              dsh.avaiblelimit += prevBil;

              bRes = true;
            }
            else if (sisaLimit >= 0)
            {
              dsd.n_bilva -= prevLimit;

              dsh.avaiblelimit += prevLimit;

              bRes = true;
            }
          }
        }
      }

      return bRes;
    }

    #region Old Coded

    //public static bool HasSPProcess(ORMDataContext db, string pesananId)
    //{
    //  return ((from q in db.LG_PLD2s
    //           join q1 in db.LG_PLHs on q.c_plno equals q1.c_plno
    //           where q.c_spno == pesananId
    //           select q1.c_plno).Count() > 0);
    //}

    #endregion

    public static int CheckAndProcessBatch(ORMDataContext db, string itemId, string batchNumber, DateTime dateExpired, string nipEntry)
    {
      if (string.IsNullOrEmpty(batchNumber))
      {
        return 0;
      }

      batchNumber = batchNumber.Trim();

      LG_MsBatch batch = null;

      int nCount = (from q in db.LG_MsBatches
                    where q.c_iteno == itemId && q.c_batch == batchNumber
                    select q).Count();

      DateTime date = DateTime.Now;

      if (nCount == 0)
      {
          batch = new LG_MsBatch()
          {
              c_batch = batchNumber,
              c_entry = nipEntry,
              c_iteno = itemId,
              c_update = nipEntry,
              d_entry = date,
              d_expired = dateExpired,
              d_update = date
          };

          db.LG_MsBatches.InsertOnSubmit(batch);

          nCount = 1;
      }

      return nCount;
    }

    public static int CheckAndProcessBatchRN(ORMDataContext db, string itemId, string batchNumber, DateTime dateExpired, string nipEntry)
    {
        if (string.IsNullOrEmpty(batchNumber))
        {
            return 0;
        }

        batchNumber = batchNumber.Trim();

        LG_MsBatch batch = null;

        int nCount = 0;

        batch = (from q in db.LG_MsBatches
                 where q.c_iteno == itemId && q.c_batch == batchNumber
                 select q).Take(1).SingleOrDefault();

        DateTime date = DateTime.Now;

        if (batch == null)
        {
            batch = new LG_MsBatch()
            {
                c_batch = batchNumber,
                c_entry = nipEntry,
                c_iteno = itemId,
                c_update = nipEntry,
                d_entry = date,
                d_expired = dateExpired,
                d_update = date
            };

            db.LG_MsBatches.InsertOnSubmit(batch);

            nCount = 1;
        }
        else
        {
            batch.d_expired = dateExpired;
        }

        return nCount;
    }

    public static bool PO_Modifikasi(ORMDataContext db, char gdg, string nomorPO, string item, bool isRevert, decimal quantity)
    {
      bool bResult = false;
      if (gdg == '6')
      {
          gdg = '1';
      }

      LG_POD1 pod1 = (from q in db.LG_POD1s
                      where q.c_gdg == gdg && q.c_pono == nomorPO && q.c_iteno == item
                      select q).Take(1).SingleOrDefault();

      if (pod1 != null)
      {
        decimal totalSum = 0;

        try
        {
          if (isRevert)
          {
            //totalSum = (pod1.n_sisa.HasValue ? pod1.n_sisa.Value : 0) + quantiy;

            //if (totalSum <= (pod1.n_qty.HasValue ? pod1.n_qty.Value : 0))
            //{
            //  pod1.n_sisa += quantiy;

            //  bResult = true;
            //}


            pod1.n_sisa += quantity;

            bResult = true;
          }
          else
          {
            totalSum = ((pod1.n_sisa.HasValue ? pod1.n_sisa.Value : 0) - quantity);

            if (totalSum >= 0)
            {
              pod1.n_sisa -= quantity;

              bResult = true;
            }
          }
        }
        catch (Exception ex)
        {
          string result = string.Format("ScmsSoaLibrary.Bussiness.Commons:PO_Modifikasi - {0}", ex.Message);

          Logger.WriteLine(result);
          Logger.WriteLine(ex.StackTrace);
        }
      }

      return bResult;
    }

    public static bool DOPharos_Modifikasi(ORMDataContext db, string noSup, string noDo, string item, string batchCode, bool isRevert, decimal quantity)
    {
      return DOPharos_Modifikasi(db, noSup, noDo, null, item, batchCode, isRevert, quantity);
    }

    public static bool DOPharos_Modifikasi(ORMDataContext db, string noSup, string noDo, string noPo, string item, string batchCode, bool isRevert, decimal quantity)
    {
      bool bResult = false;

      LG_DOPD dopd = null;

      if (string.IsNullOrEmpty(noPo))
      {
        dopd = (from q in db.LG_DOPDs
                where (q.c_nosup == noSup) && (q.c_dono == noDo)
                  && (q.c_iteno == item) && (q.c_batch == batchCode)
                select q).Take(1).SingleOrDefault();
      }
      else
      {
        dopd = (from q in db.LG_DOPDs
                where (q.c_nosup == noSup) && (q.c_dono == noDo)
                  && (q.c_iteno == item) && (q.c_batch == batchCode)
                  && (q.c_pono == noPo)
                select q).Take(1).SingleOrDefault();
      }

      if (dopd != null)
      {
        decimal totalSum = 0;

        try
        {
          if (isRevert)
          {
            dopd.n_qty_sisa += quantity;

            bResult = true;
          }
          else
          {
            //totalSum = ((dopd.n_qty.HasValue ? dopd.n_qty.Value : 0) - (dopd.n_qty_sisa.HasValue ? dopd.n_qty_sisa.Value : 0));

            //if (totalSum == 0)
            //{
            //  totalSum = ((dopd.n_qty_sisa.HasValue ? dopd.n_qty_sisa.Value : 0) - quantity);

            //  if (totalSum == 0)
            //  {
            //    dopd.n_qty_sisa -= quantity;
            //  }
            //}

            totalSum = ((dopd.n_qty_sisa.HasValue ? dopd.n_qty_sisa.Value : 0) - quantity);

            if (totalSum >= 0)
            {
              dopd.n_qty_sisa -= quantity;

              bResult = true;
            }
          }
        }
        catch (Exception ex)
        {
          string result = string.Format("ScmsSoaLibrary.Bussiness.Commons:DOPharos_Modifikasi - {0}", ex.Message);

          Logger.WriteLine(result);
          Logger.WriteLine(ex.StackTrace);
        }
      }
      
      return bResult;
    }

    public static bool InsertReceivedRespose(ORMDataContext db, string param, string response, bool isError, bool isSubmit, string sTable, string sID)
    {
      bool bOk = false;
      string result = null;

      try
      {
        db.SCMS_RCV_RESPONSEs.InsertOnSubmit(new SCMS_RCV_RESPONSE()
        {
          l_error = isError,
          v_paramsending = param,
          v_response = response
        });

        if (!isError)
        {
            if (sTable.Equals("DO"))
            {
                LG_DOH doh = new LG_DOH();

                doh = (from q in db.LG_DOHs
                       where q.c_dono == sID
                       select q).Take(1).SingleOrDefault();

                doh.l_receipt = true;
                doh.d_receipt = DateTime.Now;
            }

            if (sTable.Equals("SP"))
            {
                LG_SPH sph = new LG_SPH();

                sph = (from q in db.LG_SPHs
                       where q.c_spno == sID
                       select q).Take(1).SingleOrDefault();

                sph.l_receipt = true;
                sph.d_receipt = DateTime.Now;
            }
        }

        Constant.isDcoreError = isError;

        if (isSubmit)
        {
          db.SubmitChanges();
        }
      }
      catch (Exception ex)
      {
        result = string.Format("ScmsSoaLibrary.Bussiness.Commons:InsertReceivedRespose - {0}", ex.Message);

        Logger.WriteLine(result);
        Logger.WriteLine(ex.StackTrace);
      }


      return bOk;
    }

    #region Asyncronouse Processing

    //Indra D. 20170613
    //public static void RunningGenerateFJ(ORMDataContext db, string cust, string dono1, string dono2, string nipEntry)
    public static void RunningGenerateFJ(ORMDataContext db, string cust, string dono1, string dono2, string nipEntry, decimal SisaFJ)
    {
      Running.RunningThreadFJ rtFJ = new Running.RunningThreadFJ()
      {
        connectionString = db.Connection.ConnectionString,
        Customer = cust,
        DoNo1 = dono1,
        DoNo2 = dono2,
        User = nipEntry,
        SisaFJNO = SisaFJ, //Indra 20170613
        IsSet = true
      };

      System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Running.RunningThreadGenerateFJ), rtFJ);
    }

    public static void RunningGenerateRS(ORMDataContext db, string nipEntry, bool createFakturRetur, ScmsSoaLibrary.Modules.CommonUploadedQuery.Temporary_ImportRS[] importRSes)
    {
      Running.RunningThreadRS rtRS = new Running.RunningThreadRS()
      {
        connectionString = db.Connection.ConnectionString,
        importRSes = importRSes,
        User = nipEntry,
        CreateFakturRetur = createFakturRetur,
        IsSet = true
      };

      System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Running.RunningThreadGenerateRS), rtRS);

      
    }

    public static void RunningSendingDO(ORMDataContext db, string dono, bool isStt)
    {
      Running.RunningThreadSendDO rtSendDO = new Running.RunningThreadSendDO()
      {
        connectionString = db.Connection.ConnectionString,
        DoNo = dono,
        IsStt = isStt,
        IsSet = true
      };

      System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Running.RunningThreadGenerateSendDO), rtSendDO);
    }

    public static void RunningSendMasterDivPrinsipal(ORMDataContext db, string methodSend, FA_MsDivPri divPri)
    {
      if (string.IsNullOrEmpty(methodSend) || (divPri == null))
      {
        return;
      }

      Running.RunningThreadSendMasterDivisiPrinsipal rtSendMSDIvPri = new Running.RunningThreadSendMasterDivisiPrinsipal()
      {
        connectionString = db.Connection.ConnectionString,
        SenderMode = methodSend,
        DivPri = divPri,
        IsSet = true
      };

      System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Running.RunningThreadSendMasterDivPrinsipal), rtSendMSDIvPri);
    }

    public static void RunningSendingReplySP(string connectionString, string rawData, string spNo)
    {
      Running.RunningThreadReplySPStructure rtSendSP = new Running.RunningThreadReplySPStructure()
      {
        connectionString = connectionString,
        RawData = rawData,
        spId = spNo,
        IsSet = true
      };

      System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Running.RunningThreadRunningThreadReplySP), rtSendSP);
    }

    public static void RunningSendingReplySPM(string connectionString, string rawData, string spNo)
    {
        Running.RunningThreadReplySPStructure rtSendSP = new Running.RunningThreadReplySPStructure()
        {
            connectionString = connectionString,
            RawData = rawData,
            spId = spNo,
            IsSet = true
        };

        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Running.RunningThreadRunningThreadReplySPM), rtSendSP);
    }

    public static void RunningSendingReplySPETA(string connectionString, string rawData, string spNo)
    {
        Running.RunningThreadReplySPStructure rtSendSP = new Running.RunningThreadReplySPStructure()
        {
            connectionString = connectionString,
            RawData = rawData,
            spId = spNo,
            IsSet = true
        };

        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Running.RunningThreadRunningThreadReplySPETA), rtSendSP);
    }

    public static void RunningSendingReplyItem(string connectionString, string rawData, string iteNo)
    {
        Running.RunningThreadReplyItmStructure rtSendItm = new Running.RunningThreadReplyItmStructure()
        {
            connectionString = connectionString,
            RawData = rawData,
            Iteno = iteNo,
            IsSet = true
        };

        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Running.RunningThreadRunningThreadReplyItm), rtSendItm);
    }


    #endregion
    
    #region Faktur Pajak

    public static FakturPajakInformation GetFakturPajakDisCore(Config cfg, string fakturID)
    {
      ScmsSoaLibrary.Bussiness.Commons.FakturPajakInformation fpi = null;

      Dictionary<string, string> dicParam = new Dictionary<string, string>();
      dicParam.Add("C_SOURCE_TYPE", fakturID);
      dicParam.Add("C_KODECAB_CUS", "JK1-0001");

      Dictionary<string, string> dicHeader = new Dictionary<string, string>();
      dicHeader.Add("X-Requested-With", "XMLHttpRequest");

      IDictionary<string, object> dic = new Dictionary<string, object>();

      ScmsSoaLibrary.Parser.ResponseParser res = new ScmsSoaLibrary.Parser.ResponseParser();

      ScmsSoaLibrary.Core.Response.DiscoreResponse ds = new ScmsSoaLibrary.Core.Response.DiscoreResponse();


      ScmsSoaLibrary.Parser.ParserDisCore pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

      pdc.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
      pdc.Referer = Functionals.DistCoreUrlBuilderString(cfg, "http://10.100.11.16/dist_core/?m=com.ams.welcome");

      Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.11.16/dist_core/?m=com.ams.json.ds&action=form&f=AutoLookup&_q=trx_generate_no_tax");

      string result = null;

      Encoding utf8 = Encoding.UTF8;

      DateTime date = DateTime.MinValue;
      int nRows = 0;
      string tmp = null;

      if (pdc.PostGetData(uri, dicParam, dicHeader))
      {
        result = utf8.GetString(pdc.Result);

        Logger.WriteLine(result);

        dic = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCore(result);

        nRows = dic.GetValueParser<string, object, int>(Constant.DEFAULT_NAMING_TOTAL_ROWS);
        if (nRows > 0)
        {
          dicHeader.Clear();

          dicHeader = dic.GetValueParser<string, object, Dictionary<string, string>>(Constant.DEFAULT_NAMING_RECORDS, null);

          if (dicHeader != null)
          {
            fpi = new FakturPajakInformation();

            fpi.NoFakturPajak = dicHeader.GetValueParser<string, string, string>("C_INVOICETAXNO", string.Empty).Trim();
            tmp = dicHeader.GetValueParser<string, string, string>("TANGGAL_TAX");

            if (!string.IsNullOrEmpty(tmp))
            {
              if (!Functionals.DateParser(tmp, "yyyy-MM-dd HH:mm:ss", out date))
              {
                date = Functionals.StandardSqlDateTime;
              }
            }
            else
            {
              date = Functionals.StandardSqlDateTime;
            }

            fpi.TanggalFakturPajak = date;
          }
        }
      }
      else
      {
        Logger.WriteLine(pdc.ErrorMessage);
      }

      dic.Clear();

      dicHeader.Clear();
      dicParam.Clear();

      return fpi;
    }

    public static FakturPajakInformation GetFakturPajak(Config cfg, ORMDataContext db, string fakturID)
    {
      return GetFakturPajak(cfg, db, fakturID, true);
    }

    public static FakturPajakInformation GetFakturPajak(Config cfg, ORMDataContext db, string fakturID, bool toDistcore)
    {
      FakturPajakInformation fpi = null;

      fpi = (from q in db.SCMS_FAKTURPAJAKs
             where ((q.l_used.HasValue ? q.l_used.Value : false) == false)
             select new FakturPajakInformation()
             {
               NoFakturPajak = (q.v_fakturpajak == null ? string.Empty : q.v_fakturpajak.Trim()),
               TanggalFakturPajak = (q.d_tanggalpajak.HasValue ? q.d_tanggalpajak.Value : Functionals.StandardSqlDateTime)
             }).Take(1).SingleOrDefault();

      if ((fpi == null) && toDistcore)
      {
        fpi = GetFakturPajakDisCore(cfg, fakturID);

        if ((fpi != null) && (string.IsNullOrEmpty(fpi.NoFakturPajak) ||
          (fpi.TanggalFakturPajak.Equals(DateTime.MinValue) || fpi.TanggalFakturPajak.Equals(Functionals.StandardSqlDateTime))))
        {
          fpi = null;
        }
      }

      return fpi;
    }

    public static List<FakturPajakInformation> FreeFakturPajak(ORMDataContext db)
    {
      List<FakturPajakInformation> list = null;

      list = (from q in db.SCMS_FAKTURPAJAKs
              where ((q.l_used.HasValue ? q.l_used.Value : false) == false)
              select new FakturPajakInformation()
              {
                NoFakturPajak = (q.v_fakturpajak == null ? string.Empty : q.v_fakturpajak.Trim()),
                TanggalFakturPajak = (q.d_tanggalpajak.HasValue ? q.d_tanggalpajak.Value : Functionals.StandardSqlDateTime)
              }).Distinct().ToList();

      return list;
    }

    public static int UpdateFakturPajak(ORMDataContext db, params FakturPajakInformation[] fpis)
    {
      return UpdateFakturPajak(db, false, fpis);
    }

    public static int UpdateFakturPajak(ORMDataContext db, bool markAsFree, params FakturPajakInformation[] fpis)
    {
      if ((fpis == null) || (fpis.Length < 1))
      {
        return 0;
      }

      int nLoop = 0,
        nLoopC = 0,
        nCount = 0;

      FakturPajakInformation fpi = null;
      SCMS_FAKTURPAJAK sfp = null;
      List<SCMS_FAKTURPAJAK> listSFP = null;

      for (nLoop = 0; nLoop < fpis.Length; nLoop++)
      {
        fpi = fpis[nLoop];

        listSFP = (from q in db.SCMS_FAKTURPAJAKs
                   where (q.v_fakturpajak == fpi.NoFakturPajak)
                   select q).ToList();

        if (listSFP.Count < 1)
        {
          sfp = new SCMS_FAKTURPAJAK()
          {
            d_tanggalpajak = fpi.TanggalFakturPajak,
            v_fakturpajak = fpi.NoFakturPajak,
            c_refno = fpi.NoFaktur,
            l_used = (markAsFree ? false : fpi.IsUsed)
          };

          db.SCMS_FAKTURPAJAKs.InsertOnSubmit(sfp);

          nCount++;
        }
        else
        {
          for (nLoopC = 0; nLoopC < listSFP.Count; nLoopC++)
          {
            sfp = listSFP[nLoopC];

            sfp.l_used = (markAsFree ? false : fpi.IsUsed);
          }

          listSFP.Clear();

          nCount++;
        }
      }

      return nCount;
    }
    
    //public static int DirectInjectFPI(ORMDataContext db, params FakturPajakInformation[] fpis)
    //{
    //  if ((fpis == null) || (fpis.Length < 1))
    //  {
    //    return 0;
    //  }

    //  FakturPajakInformation fpi = null;
    //  int nResult = 0;
    //  List<SCMS_FAKTURPAJAK> lstSFP = new List<SCMS_FAKTURPAJAK>();
    //  string result = null;

    //  try
    //  {
    //    for (int nLoop = 0, nLen = fpis.Length; nLoop < nLen; nLoop++)
    //    {
    //      fpi = fpis[nLoop];

    //      lstSFP.Add(new SCMS_FAKTURPAJAK()
    //      {
    //        v_fakturpajak = fpi.NoFakturPajak,
    //        d_tanggalpajak = fpi.TanggalFakturPajak,
    //        c_refno = fpi.NoFaktur,
    //        l_used = false
    //      });
    //    }

    //    db.SCMS_FAKTURPAJAKs.InsertAllOnSubmit(lstSFP.ToArray());
    //  }
    //  catch (Exception ex)
    //  {
    //    result = string.Format("ScmsSoaLibrary.Bussiness.Commons:DirectInjectFPI - {0}", ex.Message);

    //    Logger.WriteLine(result);
    //  }

    //  lstSFP.Clear();

    //  return nResult;
    //}

    #endregion

    #region Buffered Query
    
    public static List<T> BufferedQuery<T>(IQueryable<T> qry)
    {
      return BufferedQuery(qry, 0);
    }

    public static List<T> BufferedQuery<T>(IQueryable<T> qry, int startRows)
    {
      const int MAX_DATA_GET = 1000;

      int nRows = qry.Count(),
        nStart = startRows, nSize = 0;

      nSize = (MAX_DATA_GET > nRows ? nRows : MAX_DATA_GET);

      List<T> list = new List<T>(),
        lstTmp = null;

      while (nRows > 0)
      {
        lstTmp = qry.Skip(nStart).Take(nSize).ToList();

        list.AddRange(lstTmp.ToArray());

        nStart += lstTmp.Count;
        nRows -= lstTmp.Count;

        lstTmp.Clear();
      }

      return list;
    }

    #endregion
  }

  public class Commons2
  {
      public static void RunningGenerateFJ(ORMDataContext db, string cust, string dono1, string dono2, string nipEntry)
      {
          Running.RunningThreadFJ rtFJ = new Running.RunningThreadFJ()
          {
              connectionString = db.Connection.ConnectionString,
              Customer = cust,
              DoNo1 = dono1,
              DoNo2 = dono2,
              User = nipEntry,
              IsSet = true
          };

          System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Running.RunningThreadGenerateFJ), rtFJ);
      }

      public static string GenerateNumbering<T>(ORMDataContext db, string headerCode, char portalKode, string tipeKode, DateTime tanggalAktif, string fieldCondition)
      {
          string result = string.Empty;

          //db.GetTable<T>().Where(

          int nCount = 0,
            nValue = 0,
            nCountTambah = 0;
          string tmpNum = null,
            hdrValue = null,
            tVal = null;
          char chr = char.MinValue;

          SysNo sysNum = (from q in db.SysNos
                          where q.c_portal == portalKode && q.c_type == tipeKode
                            && q.s_tahun == tanggalAktif.Year
                          select q).Take(1).SingleOrDefault();

          if (sysNum != null)
          {
              switch (tanggalAktif.Month)
              {
                  case 1: tmpNum = (sysNum.c_bln01 ?? string.Empty); break;
                  case 2: tmpNum = (sysNum.c_bln02 ?? string.Empty); break;
                  case 3: tmpNum = (sysNum.c_bln03 ?? string.Empty); break;
                  case 4: tmpNum = (sysNum.c_bln04 ?? string.Empty); break;
                  case 5: tmpNum = (sysNum.c_bln05 ?? string.Empty); break;
                  case 6: tmpNum = (sysNum.c_bln06 ?? string.Empty); break;
                  case 7: tmpNum = (sysNum.c_bln07 ?? string.Empty); break;
                  case 8: tmpNum = (sysNum.c_bln08 ?? string.Empty); break;
                  case 9: tmpNum = (sysNum.c_bln09 ?? string.Empty); break;
                  case 10: tmpNum = (sysNum.c_bln10 ?? string.Empty); break;
                  case 11: tmpNum = (sysNum.c_bln11 ?? string.Empty); break;
                  case 12: tmpNum = (sysNum.c_bln12 ?? string.Empty); break;
                  default: tmpNum = null; break;
              }

              if (!string.IsNullOrEmpty(tmpNum))
              {
                  switch (headerCode)
                  {
                      case "PL":
                          GenerateNumberingAutoNumber(db, headerCode, portalKode, tipeKode, tanggalAktif);
                          break;
                  }

                  tmpNum = (string.IsNullOrEmpty(tmpNum) ? string.Empty : tmpNum.Trim());

                  hdrValue = (string.IsNullOrEmpty(headerCode) ? "__" :
                    ((headerCode.Length > 1) ? headerCode.PadLeft(2, '_') : headerCode.Substring(0, 2)));

                  do
                  {
                      if (tmpNum.Length > 1)
                      {
                          if (!char.IsNumber(tmpNum, 0))
                          {
                              tVal = tmpNum.Substring(1);

                              if (!int.TryParse(tVal, out nValue))
                              {
                                  result = string.Empty;

                                  goto endLogic;
                              }

                              nValue++;

                              if (nValue > 999)
                              {
                                  chr = tmpNum[0];
                                  chr++;

                                  if (chr > 0x60)
                                  {
                                      chr = (char)0x7b;
                                  }

                                  tmpNum = string.Concat(chr, "001");
                              }
                              else
                              {
                                  tmpNum = string.Concat(tmpNum[0], nValue.ToString().PadLeft(3, '0'));
                              }
                          }
                          else
                          {
                              if (!int.TryParse(tmpNum, out nValue))
                              {
                                  result = string.Empty;

                                  goto endLogic;
                              }

                              nValue++;

                              if (nValue > 9999)
                              {
                                  tmpNum = "A001";
                              }
                              else
                              {
                                  tmpNum = nValue.ToString().PadLeft(4, '0');
                              }
                          }
                      }
                      else
                      {
                          tmpNum = "0001";
                      }

                      if (headerCode == "SPP")
                      {

                          result = string.Concat(hdrValue, tanggalAktif.ToString("yyyyMM"), tmpNum);
                      }
                      else
                      {
                          result = string.Concat(hdrValue, tanggalAktif.ToString("yyMM"), tmpNum);
                      }
                      nCount = db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count();

                      if (nCount > 0)
                      {
                          string iNum = result.Substring(7, 3);
                          string iTestNum = result.Substring(0, 6),
                             sNum = null;
                          string iNumSwi = "";
                          if (ScmsSoaLibrary.Commons.Constant.TRANSID != "")
                          {
                              iNumSwi = ScmsSoaLibrary.Commons.Constant.TRANSID.Substring(6, 1);
                          }
                          else
                          {
                              iNumSwi = result.Substring(6,1);
                          }
                          int Num = 0;

                          bool f = int.TryParse(iTestNum, out Num);

                          if (!f)
                          {
                              nCountTambah++;
                              if ((iNum == "000"))
                              {
                                  iNum = "999";
                                  tmpNum = "0001";
                              }
                          }


                          do
                          {
                              switch (iNumSwi)
                              {

                                  case "0":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);
                                          nCountTambah--;
                                          nCount--;
                                          break;
                                      }
                                  case "1":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);
                                          nCountTambah--;
                                          nCount--;
                                          break;
                                      }
                                  case "2":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);
                                          nCountTambah--;
                                          nCount--;
                                          break;
                                      }
                                  case "3":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);
                                          nCountTambah--;
                                          nCount--;
                                          break;
                                      }
                                  case "4":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);
                                          nCountTambah--;
                                          nCount--;
                                          break;
                                      }
                                  case "5":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);
                                          nCountTambah--;
                                          nCount--;
                                          break;
                                      }
                                  case "6":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);
                                          nCountTambah--;
                                          nCount--;
                                          break;
                                      }
                                  case "7":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);
                                          nCountTambah--;
                                          nCount--;
                                          break;
                                      }
                                  case "8":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);
                                          nCountTambah--;
                                          nCount--;
                                          break;
                                      }
                                  case "9":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);
                                          nCountTambah--;
                                          nCount--;
                                          break;
                                      }
                                  case "A":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "B";
                                              iNum = "001";
                                              break;

                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "B":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "C";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "C":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "D";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "D":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "E";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "E":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "F";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);

                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "F":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "G";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "G":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "H";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "H":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "I";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, Num);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "I":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "J";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "J":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "K";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "K":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "L";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "L":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "M";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "M":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "N";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "N":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "O";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "O":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "P";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "P":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "Q";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "Q":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "R";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "R":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "S";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "S":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "T";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "T":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "U";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "U":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "V";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "V":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "W";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "W":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "X";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "X":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "Y";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                                  case "Y":
                                      {
                                          result = string.Concat(iTestNum, iNumSwi, iNum);

                                          if (db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count() > 0)
                                          {
                                              iNumSwi = "Z";
                                              iNum = "001";
                                              break;
                                          }
                                          else
                                          {
                                              result = string.Concat(iTestNum, iNumSwi, iNum);
                                              nCountTambah--;
                                              nCount--;
                                          }
                                      }
                                      break;
                              }
                          } while (nCountTambah != 0);
                      }

                  } while (nCount != 0);


                  switch (tanggalAktif.Month)
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

                //db.SubmitChanges();

              endLogic:
                  ;
              }
          }

          return result;
      }


      public static bool GenerateNumberingAutoNumber(ORMDataContext db, string headerCode, char portal, string tipeKode, DateTime date)
      {
          bool isOk = false;

          sysnoNumber sysNumAut = new sysnoNumber();

          sysNumAut = (from q in db.sysnoNumbers
                       where q.c_portal == portal
                       && q.c_notrans == tipeKode
                         && q.s_tahun == date.Year
                         && q.c_gdg == Constant.Gudang
                       select q).SingleOrDefault();

          string tmpNum = null;

          switch (date.Month)
          {
              case 1: tmpNum = (sysNumAut.c_bln01 ?? string.Empty); break;
              case 2: tmpNum = (sysNumAut.c_bln02 ?? string.Empty); break;
              case 3: tmpNum = (sysNumAut.c_bln03 ?? string.Empty); break;
              case 4: tmpNum = (sysNumAut.c_bln04 ?? string.Empty); break;
              case 5: tmpNum = (sysNumAut.c_bln05 ?? string.Empty); break;
              case 6: tmpNum = (sysNumAut.c_bln06 ?? string.Empty); break;
              case 7: tmpNum = (sysNumAut.c_bln07 ?? string.Empty); break;
              case 8: tmpNum = (sysNumAut.c_bln08 ?? string.Empty); break;
              case 9: tmpNum = (sysNumAut.c_bln09 ?? string.Empty); break;
              case 10: tmpNum = (sysNumAut.c_bln10 ?? string.Empty); break;
              case 11: tmpNum = (sysNumAut.c_bln11 ?? string.Empty); break;
              case 12: tmpNum = (sysNumAut.c_bln12 ?? string.Empty); break;
              default: tmpNum = null; break;
          }

          int iValue = 0;
          string sValue = null;
          isOk = int.TryParse(tmpNum, out iValue);

          if (isOk)
          {
              iValue += 1;

              int iGudang = int.Parse(Constant.Gudang.ToString());

              sValue = iValue.ToString("000000");
              Constant.NUMBERID_GUDANG = string.Concat(headerCode, date.Year.ToString().Substring(2, 2), date.Month.ToString("00"), iGudang.ToString("00"), iValue.ToString("000000"));

              switch (date.Month)
              {
                  case 1: sysNumAut.c_bln01 = sValue; break;
                  case 2: sysNumAut.c_bln02 = sValue; break;
                  case 3: sysNumAut.c_bln03 = sValue; break;
                  case 4: sysNumAut.c_bln04 = sValue; break;
                  case 5: sysNumAut.c_bln05 = sValue; break;
                  case 6: sysNumAut.c_bln06 = sValue; break;
                  case 7: sysNumAut.c_bln07 = sValue; break;
                  case 8: sysNumAut.c_bln08 = sValue; break;
                  case 9: sysNumAut.c_bln09 = sValue; break;
                  case 10: sysNumAut.c_bln10 = sValue; break;
                  case 11: sysNumAut.c_bln11 = sValue; break;
                  case 12: sysNumAut.c_bln12 = sValue; break;
              }
          }

          return isOk;
      }

      public static int CheckAndProcessBatch(ORMDataContext db, string itemId, string batchNumber, DateTime dateExpired, string nipEntry)
      {
          if (string.IsNullOrEmpty(batchNumber))
          {
              return 0;
          }

          batchNumber = batchNumber.Trim();

          LG_MsBatch batch = null;

          int nCount = (from q in db.LG_MsBatches
                        where q.c_iteno == itemId && q.c_batch == batchNumber
                        select q).Count();

          DateTime date = DateTime.Now;

          if (nCount == 0)
          {
              batch = new LG_MsBatch()
              {
                  c_batch = batchNumber,
                  c_entry = nipEntry,
                  c_iteno = itemId,
                  c_update = nipEntry,
                  d_entry = date,
                  d_expired = dateExpired,
                  d_update = date
              };

              db.LG_MsBatches.InsertOnSubmit(batch);

              nCount = 1;
          }

          return nCount;
      }


      #region ReSend DO to Dcore

      private static string GetDataDO(ORMDataContext db, string doNo, bool isStt)
      {
          string result = null;

          ScmsSoaLibrary.Parser.Class.DeliveryOrderPosting dop = null;
          List<ScmsSoaLibrary.Parser.Class.DeliveryOrderDetailPostings> listDetails = null;

          ScmsSoaLibrary.Parser.Class.DeliveryOrderDetailPostings dodp = null;

          int nLoop = 0;

          try
          {
              dop = (from q in db.LG_DOHs
                     join q1 in db.LG_MsGudangs on q.c_gdg equals q1.c_gdg
                     join q2 in db.MsTransDs on new { c_portal = '3', c_notrans = "02", c_type = q.c_via } equals new { q2.c_portal, q2.c_notrans, q2.c_type }
                     join q3 in
                         (from sq1 in db.LG_FJHs
                          join sq2 in db.LG_FJD3s on sq1.c_fjno equals sq2.c_fjno
                          where ((sq1.l_delete.HasValue ? sq1.l_delete.Value : false) == false)
                          select new
                          {
                              c_fjno = (sq1.c_fjno == null ? string.Empty : sq1.c_fjno.Trim()),
                              d_fjdate = (sq1.d_fjdate.HasValue ? sq1.d_fjdate.Value : Functionals.StandardSqlDateTime),
                              sq2.c_dono
                          }) on q.c_dono equals q3.c_dono into q_3
                         //suwandi 21 juni 2017
                         join q4 in db.LG_PLD2s on q.c_plno equals q4.c_plno
                         join q5 in db.LG_SPHs on q4.c_spno equals q5.c_spno
                         //suwandi 21 juni 2017                     
                     from qFJs in q_3.DefaultIfEmpty()
                     where (q.c_dono == doNo)
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                     select new ScmsSoaLibrary.Parser.Class.DeliveryOrderPosting()
                     {
                         PIN = (q.c_pin == null ? string.Empty : q.c_pin.Trim()),
                         Cabang = (q.c_cusno == null ? string.Empty : q.c_cusno.Trim()),
                         Gudang = (q.c_gdg.HasValue ? q.c_gdg.Value : char.MinValue),
                         GudangDesc = q1.v_gdgdesc,
                         TypeCode = (q.c_type == null ? string.Empty : q.c_type.Trim()),
                         Via = q2.v_ket,
                         DO = (q.c_dono == null ? string.Empty : q.c_dono.Trim()),
                         ReferenceID = (q.c_plno == null ? string.Empty : q.c_plno.Trim()),
                         TanggalDO = (q.d_dodate.HasValue ? q.d_dodate.Value : Functionals.StandardSqlDateTime),
                         FakturID = (qFJs != null ? qFJs.c_fjno : string.Empty),
                         TanggalFJ = (qFJs != null ? qFJs.d_fjdate : Functionals.StandardSqlDateTime),
                         user = (q.c_po_outlet == null ? "SCMS" : "AUTO_PHAR"),
                         PoOutlet = q.c_po_outlet,
                         Outlet = q.c_outlet,
                         PLPHAR = q.c_plphar,//suwandi 13 juli 2017
                         SPPs = q5.c_spphar //suwandi 21 juni 2017
                     }).Distinct().Take(1).SingleOrDefault();

              if (dop != null)
              {
                  if (!string.IsNullOrEmpty(dop.PIN))
                  {
                      dop.PIN = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(dop.PIN));
                  }

                  dop.TanggalDO_Str = dop.TanggalDO.ToString("yyyy-MM-dd");

                  dop.TanggalFJ_Str = dop.TanggalFJ.ToString("yyyy-MM-dd");

                  #region Complex

                  if (dop.ReferenceID.StartsWith("PL", StringComparison.OrdinalIgnoreCase))
                  {
                      #region PL

                      listDetails = (from q in db.LG_DOD1s
                                     join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                     join q2 in db.LG_PLD1s on new { c_plno = dop.ReferenceID, q.c_iteno } equals new { q2.c_plno, q2.c_iteno } into q_2
                                     from qPLDs in q_2.Where(t => t.n_qty > 0).DefaultIfEmpty()
                                     join q4 in db.LG_MsBatches on new { q.c_iteno, qPLDs.c_batch } equals new { q4.c_iteno, q4.c_batch } into q_4
                                     from qBat in q_4.DefaultIfEmpty()
                                     join q5 in
                                         (from sq1 in db.LG_FJD1s
                                          join sq2 in db.LG_FJD2s on new { sq1.c_fjno, sq1.c_iteno } equals new { sq2.c_fjno, sq2.c_iteno }
                                          where (sq1.c_fjno == dop.FakturID)
                                          select new
                                          {
                                              sq1.c_iteno,
                                              sq1.n_salpri,
                                              sq2.n_discon,
                                              sq2.n_discoff
                                          }).Distinct() on q.c_iteno equals q5.c_iteno into q_5
                                     from qFJs in q_5.DefaultIfEmpty()
                                     where (q.c_dono == dop.DO)
                                     select new ScmsSoaLibrary.Parser.Class.DeliveryOrderDetailPostings()
                                     {
                                         Item = q.c_iteno,
                                         //NamaItem = q1.v_itnam,
                                         Jumlah = (qPLDs.n_qty.HasValue ? qPLDs.n_qty.Value : 0),
                                         Batch = (qPLDs != null ? (qPLDs.c_batch == null ? string.Empty : qPLDs.c_batch.Trim()) : string.Empty),
                                         Expired = (qBat != null ? (qBat.d_expired.HasValue ? qBat.d_expired.Value : Functionals.StandardSqlDateTime) : Functionals.StandardSqlDateTime),
                                         Harga = (qFJs != null ? (qFJs.n_salpri.HasValue ? qFJs.n_salpri.Value : 0) : 0),
                                         Diskon = (qFJs != null ? (qFJs.n_discon.HasValue ? qFJs.n_discon.Value : 0) : 0),
                                         //PoOutlet = (qPLDs != null ? (qPLDs.c_po_outlet == null ? string.Empty : qPLDs.c_po_outlet.Trim()) : string.Empty),
                                         SPs = null
                                     }).Distinct().ToList();

                      for (nLoop = 0; nLoop < listDetails.Count; nLoop++)
                      {
                          dodp = listDetails[nLoop];

                          if (dodp != null)
                          {
                              dodp.Expired_Str = dodp.Expired.ToString("yyyy-MM-dd");

                              if (!isStt)
                              {
                                  dodp.SPs = (from q in db.LG_PLD1s
                                              //							join q1 in db.LG_SPD1s on new { q.C_spno, q.C_iteno } equals new { q1.C_spno, q1.C_iteno } into q_1
                                              //							from qSPD1s in q_1.DefaultIfEmpty()
                                              join q2 in db.LG_SPHs on q.c_spno equals q2.c_spno into q_2
                                              from qSPHs in q_2.DefaultIfEmpty()

                                              where (q.c_plno == dop.ReferenceID) && (q.c_iteno == dodp.Item)
                                                && (q.n_qty > 0) && q.c_batch == dodp.Batch
                                              group new { qSPHs, q } by new { qSPHs.c_sp, q.c_iteno, q.c_batch, q.n_qty } into gSum
                                              select new ScmsSoaLibrary.Parser.Class.DeliveryOrderSPDetailPostings()
                                              {
                                                  //SP = (gSum == null ? string.Empty : gSum.Key.c_sp.Trim()),
                                                  SP = (gSum == null ? string.Empty : gSum.Key.c_sp.Trim()),
                                                  Jumlah = (gSum.Sum(x => x.q.n_qty.HasValue ? x.q.n_qty.Value : 0))
                                              }).Distinct().ToArray();


                              }

                              //dodp.Batch = dodp.Batch.Replace("/", "<>");
                              //dodp.NamaItem = dodp.NamaItem.Replace("&", " ");
                          }
                      }

                      #endregion
                  }
                  else
                  {
                      #region STT

                      listDetails = (from q in db.LG_DOD2s
                                     //join qd2 in db.LG_DOD2s on q.c_dono equals qd2.c_dono
                                     join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                                     //join q2 in db.LG_STD1s on new { c_stno = dop.ReferenceID, q.c_iteno } equals new { q2.c_stno, q2.c_iteno } into q_2
                                     //from qSTDs in q_2.DefaultIfEmpty()
                                     join q4 in db.LG_MsBatches on new { q.c_iteno, q.c_batch } equals new { q4.c_iteno, q4.c_batch } into q_4
                                     from qBat in q_4.DefaultIfEmpty()
                                     join q5 in
                                         (from sq1 in db.LG_FJD1s
                                          join sq2 in db.LG_FJD2s on new { sq1.c_fjno, sq1.c_iteno } equals new { sq2.c_fjno, sq2.c_iteno }
                                          where (sq1.c_fjno == dop.FakturID)
                                          select new
                                          {
                                              sq1.c_iteno,
                                              sq1.n_salpri,
                                              sq2.n_discon,
                                              sq2.n_discoff
                                          }).Distinct() on q.c_iteno equals q5.c_iteno into q_5
                                     from qFJs in q_5.DefaultIfEmpty()
                                     where (q.c_dono == dop.DO)
                                     select new ScmsSoaLibrary.Parser.Class.DeliveryOrderDetailPostings()
                                     {
                                         Item = q.c_iteno,
                                         //NamaItem = q1.v_itnam,
                                         Jumlah = (q.n_qty.HasValue ? q.n_qty.Value : 0),
                                         Batch = (q != null ? (q.c_batch == null ? string.Empty : q.c_batch.Trim()) : string.Empty),
                                         Expired = (qBat != null ? (qBat.d_expired.HasValue ? qBat.d_expired.Value : Functionals.StandardSqlDateTime) : Functionals.StandardSqlDateTime),
                                         Harga = (qFJs != null ? (qFJs.n_salpri.HasValue ? qFJs.n_salpri.Value : 0) : 0),
                                         Diskon = (qFJs != null ? (qFJs.n_discon.HasValue ? qFJs.n_discon.Value : 0) : 0),
                                         //PoOutlet = null,
                                         SPs = null
                                     }).Distinct().ToList();

                      for (nLoop = 0; nLoop < listDetails.Count; nLoop++)
                      {
                          dodp = listDetails[nLoop];

                          if (dodp != null)
                          {
                              dodp.Expired_Str = dodp.Expired.ToString("yyyy-MM-dd");
                          }
                      }

                      #endregion
                  }


                  #endregion

                  if (listDetails.Count > 0)
                  {
                      dop.Fields = listDetails.ToArray();

                      listDetails.Clear();
                  }

                  result = ScmsSoaLibrary.Parser.Class.DeliveryOrderPosting.Serialize(dop);
              }
          }
          catch (Exception ex)
          {
              Logger.WriteLine(string.Format("ScmsSoaLibrary.Modules.CommonQuerySP:SP_LG_CalcFJ - {0}", ex.Message));
          }

          return result;
      }

      public static bool PostDataDODirect(ORMDataContext db, string doNo, bool isStt, bool directCommit)
      {
          string dataResult = GetDataDO(db, doNo, isStt);

          bool bResult = false;

          Config cfg = Functionals.Configuration;

          Dictionary<string, string> dicParam = new Dictionary<string, string>();
          dicParam.Add("param", dataResult);

          Dictionary<string, string> dicHeader = new Dictionary<string, string>();
          dicHeader.Add("X-Requested-With", "XMLHttpRequest");

          IDictionary<string, object> dic = new Dictionary<string, object>();

          ScmsSoaLibrary.Parser.ResponseParser res = new ScmsSoaLibrary.Parser.ResponseParser();

          ScmsSoaLibrary.Core.Response.DiscoreResponse ds = new ScmsSoaLibrary.Core.Response.DiscoreResponse();

          ScmsSoaLibrary.Parser.ParserDisCore pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

          pdc.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
          pdc.Referer = Functionals.DistCoreUrlBuilderString(cfg, "http://10.100.11.52/dcore/?m=com.ams.welcome");

          Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.28/dcore/?m=com.ams.json.ds&action=form&f=Submit&_q=trx_update_do_rn");

          //Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.28/dcore/?m=com.ams.json.ds&action=form&f=Submit&_q=trx_update_do_rnxx");

          string result = null;

          bool isError = false;

          StringBuilder sb = new StringBuilder();

          Encoding utf8 = Encoding.UTF8;

          DateTime date = DateTime.MinValue;

          sb.AppendLine(uri.ToString());
          sb.AppendLine(dataResult);

          Logger.WriteLine(uri.ToString());



          if (pdc.PostGetData(uri, dicParam, dicHeader))
          {
              result = utf8.GetString(pdc.Result);

              bResult = true;
          }
          else
          {
              result = pdc.ErrorMessage + " " + uri;
              isError = true;
          }

          Logger.WriteLine(result, true);

          Bussiness.Commons.InsertReceivedRespose(db, sb.ToString(), result, isError, directCommit, "DO", doNo);

          dic.Clear();

          dicHeader.Clear();
          dicParam.Clear();

          return bResult;
      }

      #endregion
  }
}
