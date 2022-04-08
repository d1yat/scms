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
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaLibrary.Bussiness
{
  #region Internal Class

  internal class HeaderFakturReturProcess
  {
    public string Customer { get; set; }
    public string ExFaktur { get; set; }
    public string NoRetur { get; set; }
    public DateTime TanggalRetur { get; set; }
    public string NoReference { get; set; }
    public bool IsCabang { get; set; }
  }

  internal class DetailFakturReturProcess
  {
    public string NoRetur { get; set; }
    public string NoDO { get; set; }
    public string NoItem { get; set; }
    public decimal Salpri { get; set; }
    public decimal Quantity { get; set; }
    public decimal Discount { get; set; }
  }

  internal class Temp_FakturBeliRetur
  {
    public string NoSup { get; set; }
    public bool IsImport { get; set; }
  }

  internal class FakturBeliData
  {
    public string NoItem { get; set; }
    public string TipeItem { get; set; }
  }

  #endregion

  class Faktur
  {
    public string FakturJual(ScmsSoaLibrary.Parser.Class.FakturJualStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      string result = null;

      bool hasAnyChanges = false;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      ScmsSoaLibrary.Parser.Class.FakturJualStructureField field = null;

      string nipEntry = null;
      string fakturID = null,
        doId = null,
        tmpNum = null;
      //string tmpNumbering = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now,
        dateF = DateTime.MinValue,
        dateGen = DateTime.Now;

      decimal totalGross = 0,
        totalDisc = 0,
        totalPajak = 0,
        totalNet = 0,
        totalSisa = 0,
        //totaolPrevNet = 0,
        dQty = 0,
        dDisc = 0,
        //dGross = 0,
        dPrice = 0;

      LG_FJH fjh = null;
      LG_FJD1 fjd1 = null;
      LG_FJD2 fjd2 = null;
      LG_FJD3 fjd3 = null;

      LG_MsNomorPajak msnomorpajak = null;

      List<LG_FJD1> listFJD1 = null;
      List<LG_FJD2> listFJD2 = null;
      //List<LG_FJD3> listFJD3 = null;
      List<LG_FJD4> listFJD4 = null;

      List<LG_FJH> listFjh = null;
      List<LG_MsNomorPajak> listMsNomorPajak = null;

      LG_DOH doh = null;

      //List<string> listItems = null;

      int nLoop = 0,
        //nLoopC = 0,
        nValue = 0;

      bool isCabang = false;

      ScmsSoaLibrary.Bussiness.Commons.FakturPajakInformation fpi = null;

      IDictionary<string, string> dic = null;

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }
      
      fakturID = (structure.Fields.FakturID ?? string.Empty);
      
      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify
          
          if (string.IsNullOrEmpty(fakturID))
          {
            result = "Nomor faktur jual dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          
          fjh = (from q in db.LG_FJHs
                 where q.c_fjno == fakturID
                 select q).Take(1).SingleOrDefault();

          if (fjh == null)
          {
            result = "Nomor faktur jual tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (fjh.l_delete.HasValue && fjh.l_delete.Value)
          {
            result = "Tidak dapat mengubah nomor faktur jual yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingFA(db, fjh.d_fjdate.Value))
          {
            result = "Faktur tidak dapat diubah, karena tanggal pada transaksi ini sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
          {
            fjh.v_ket = structure.Fields.Keterangan;
          }

          isCabang = (fjh.l_cabang.HasValue ? fjh.l_cabang.Value : false);

          nValue = (fjh.n_top.HasValue ? (int)fjh.n_top.Value : 0);
          dateF = (fjh.d_top.HasValue ? fjh.d_top.Value : (fjh.d_fjdate.HasValue ? fjh.d_fjdate.Value : DateTime.MinValue));
          if (!nValue.Equals(structure.Fields.Top))
          {
            nValue = (structure.Fields.Top - nValue);

            fjh.n_top = structure.Fields.Top;
            fjh.d_top = dateF.AddDays(nValue);
          }

          nValue = (fjh.n_toppjg.HasValue ? (int)fjh.n_toppjg.Value : 0);
          dateF = (fjh.d_toppjg.HasValue ? fjh.d_toppjg.Value : (fjh.d_fjdate.HasValue ? fjh.d_fjdate.Value : DateTime.MinValue));
          if (!nValue.Equals(structure.Fields.TopPjg))
          {
            nValue = (structure.Fields.TopPjg - nValue);

            fjh.n_toppjg = structure.Fields.TopPjg;
            fjh.d_toppjg = dateF.AddDays(nValue);
          }

          fjh.c_taxno = structure.Fields.NoTax;
          fjh.d_taxdate = structure.Fields.TanggalTaxDate;

          fjh.c_kurs = structure.Fields.Kurs;
          fjh.n_kurs = structure.Fields.KursValue;

          fjh.c_update = nipEntry;
          fjh.d_update = date;

          //db.SubmitChanges();

          #region Populate Detail

          listFJD1 = (from q in db.LG_FJD1s
                      where q.c_fjno == fakturID
                      select q).ToList();
          listFJD2 = (from q in db.LG_FJD2s
                      where q.c_fjno == fakturID
                      select q).ToList();

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            fjd3 = (from q in db.LG_FJD3s
                        where q.c_fjno == fakturID
                        select q).Take(1).SingleOrDefault();

            doId = (fjd3 != null ? fjd3.c_dono : string.Empty);

            listFJD4 = new List<LG_FJD4>();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              fjd1 = listFJD1.Find(delegate(LG_FJD1 fjd)
              {
                return field.Item.Equals(fjd.c_iteno);
              });
              fjd2 = listFJD2.Find(delegate(LG_FJD2 fjd)
              {
                return field.Item.Equals(fjd.c_iteno);
              });

              if ((fjd1 != null) && (fjd2 != null))
              {
                if (field.IsModified && (!field.IsDelete))
                {
                  #region Modify

                  dQty = (fjd1.n_qty.HasValue ? fjd1.n_qty.Value : 0);

                  dPrice = (field.Harga * dQty);

                  dDisc = (dPrice * (field.Discount / 100));

                  listFJD4.Add(new LG_FJD4()
                  {
                    c_fjno = fakturID,
                    c_dono = doId,
                    c_entry = nipEntry,
                    c_iteno = field.Item,
                    c_no = fjd2.c_no,
                    c_type = fjd2.c_type,
                    d_entry = date,
                    n_disc = fjd1.n_disc,
                    n_discoff = fjd2.n_discoff,
                    n_discon = fjd2.n_discon,
                    n_netoff = fjd2.n_netoff,
                    n_neton = fjd2.n_neton,
                    n_qty = fjd1.n_qty,
                    n_salpri = fjd1.n_salpri,
                    n_sisaoff = fjd2.n_sisaoff,
                    n_sisaon = fjd2.n_sisaon,
                    v_ket_del = field.KeteranganMod,
                    v_type = "02",
                    c_taxno = (fjh != null ? fjh.c_taxno : string.Empty),
                    d_taxdate = (fjh != null ? fjh.d_taxdate : date)
                  });

                  fjd2.n_neton = dDisc;
                  fjd2.n_sisaon = (dPrice - dDisc);

                  fjd1.n_salpri = field.Harga;
                  fjd2.n_discon = field.Discount;

                  #endregion
                }
                else if ((!field.IsModified) && field.IsDelete)
                {
                  #region Delete

                  listFJD4.Add(new LG_FJD4()
                  {
                    c_fjno = fakturID,
                    c_dono = doId,
                    c_entry = nipEntry,
                    c_iteno = field.Item,
                    c_no = fjd2.c_no,
                    c_type = fjd2.c_type,
                    d_entry = date,
                    n_disc = fjd1.n_disc,
                    n_discoff = fjd2.n_discoff,
                    n_discon = fjd2.n_discon,
                    n_netoff = fjd2.n_netoff,
                    n_neton = fjd2.n_neton,
                    n_qty = fjd1.n_qty,
                    n_salpri = fjd1.n_salpri,
                    n_sisaoff = fjd2.n_sisaoff,
                    n_sisaon = fjd2.n_sisaon,
                    v_ket_del = field.KeteranganMod,
                    v_type = "03",
                    c_taxno = (fjh != null ? fjh.c_taxno : string.Empty),
                    d_taxdate = (fjh != null ? fjh.d_taxdate : date)
                  });

                  db.LG_FJD1s.DeleteOnSubmit(fjd1);
                  db.LG_FJD2s.DeleteOnSubmit(fjd2);

                  listFJD1.Remove(fjd1);
                  listFJD2.Remove(fjd2);

                  #endregion
                }
              }
            }

            if ((listFJD4 != null) && (listFJD4.Count > 0))
            {
              db.LG_FJD4s.InsertAllOnSubmit(listFJD4.ToArray());

              listFJD4.Clear();
            }

            //db.SubmitChanges();
          }

          #endregion
          
          #region Recalculate All

          totalGross =
            totalDisc = 0;

          if ((listFJD1 != null) && (listFJD1.Count > 0))
          {
            for (nLoop = 0; nLoop < listFJD1.Count; nLoop++)
            {

              fjd1 = listFJD1[nLoop];

              if (fjd1 != null)
              {
                fjd2 = listFJD2.Find(delegate(LG_FJD2 fjd)
                {
                  if (string.IsNullOrEmpty(fjd.c_iteno))
                  {
                    return false;
                  }

                  return (string.IsNullOrEmpty(fjd1.c_iteno) ? false :
                    (fjd1.c_iteno.Trim().Equals(fjd.c_iteno.Trim()) ? true : false));
                });

                if (fjd2 != null)
                {
                  dQty = (fjd1.n_qty.HasValue ? fjd1.n_qty.Value : 0);
                  dPrice = ((fjd1.n_salpri.HasValue ? fjd1.n_salpri.Value : 0) * dQty);

                  dDisc = (dPrice * ((fjd2.n_discon.HasValue ? fjd2.n_discon.Value : 0) / 100));

                  totalGross += dPrice;
                  totalDisc += dDisc;
                }
              }
            }
          }

          #endregion

          #region Replace

          decimal ppn = db.fn_GetTax("PPN", structure.Fields.TanggalTaxDate) ?? decimal.Zero;
            totalPajak = (isCabang ? 0 : ((totalGross - totalDisc) * ppn));
          totalNet = ((totalGross - totalDisc) + totalPajak);

          totalSisa = ((fjh.n_net.HasValue ? fjh.n_net.Value : 0) - (fjh.n_sisa.HasValue ? fjh.n_sisa.Value : 0));

          dPrice = (totalNet - totalSisa);

          fjh.n_gross = totalGross;
          fjh.n_disc = totalDisc;
          fjh.n_tax = totalPajak;
          fjh.n_net = totalNet;
          fjh.n_sisa = dPrice;

          dic = new Dictionary<string, string>();

          dic.Add("Faktur", fakturID);
          dic.Add("Sisa", Functionals.DecimalToString(dPrice));
          dic.Add("Net", Functionals.DecimalToString(totalNet));

          #endregion

          hasAnyChanges = true;

          #endregion
        }
        else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete
          
          if (string.IsNullOrEmpty(fakturID))
          {
            result = "Nomor faktur jual dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          fjh = (from q in db.LG_FJHs
                 where q.c_fjno == fakturID
                 select q).Take(1).SingleOrDefault();

          if (fjh == null)
          {
            result = "Nomor faktur jual tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (fjh.l_delete.HasValue && fjh.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor faktur jual yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingFA(db, fjh.d_fjdate.Value))
          {
            result = "Faktur tidak dapat dihapus, karena tanggal pada transaksi ini sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          fjh.c_update = nipEntry;
          fjh.d_update = DateTime.Now;

          fjh.l_delete = true;
          fjh.v_ket_mark = structure.Fields.Keterangan;

          db.LG_FJHs.DeleteOnSubmit(fjh);

          #region Revert Faktur Pajak

          isCabang = (fjh.l_cabang.HasValue ? fjh.l_cabang.Value : false);

          if (!isCabang)
          {
            tmpNum = (string.IsNullOrEmpty(fjh.c_taxno) ? string.Empty : fjh.c_taxno.Trim());
            if (!string.IsNullOrEmpty(tmpNum))
            {
              fpi = new Commons.FakturPajakInformation()
              {
                NoFaktur = string.Empty,
                NoFakturPajak = tmpNum,
                TanggalFakturPajak = (fjh.d_taxdate.HasValue ? fjh.d_taxdate.Value : Functionals.StandardSqlDateTime),
                IsUsed = false
              };

              Commons.UpdateFakturPajak(db, fpi);
            }
          }

          #endregion
          
          #region Hapus Detail

          listFJD1 = (from q in db.LG_FJD1s
                      where q.c_fjno == fakturID
                      select q).ToList();
          listFJD2 = (from q in db.LG_FJD2s
                      where q.c_fjno == fakturID
                      select q).ToList();
          fjd3 = (from q in db.LG_FJD3s
                  where q.c_fjno == fakturID
                  select q).Take(1).SingleOrDefault();
          
          doId = (fjd3 != null ? fjd3.c_dono : string.Empty);

          listFJD4 = new List<LG_FJD4>();

          for (nLoop = 0; nLoop < listFJD1.Count; nLoop++)
          {
            fjd1 = listFJD1[nLoop];
            
            fjd2 = listFJD2.Find(delegate(LG_FJD2 fjd)
            {
              return (string.IsNullOrEmpty(fjd1.c_iteno) ? false :
                (fjd1.c_iteno.Equals(fjd.c_iteno, StringComparison.OrdinalIgnoreCase) ? true : false));
            });

            if ((fjd1 != null) && (fjd2 != null))
            {
              listFJD4.Add(new LG_FJD4()
              {
                c_fjno = fakturID,
                c_dono = doId,
                c_entry = nipEntry,
                c_iteno = fjd1.c_iteno,
                c_no = fjd2.c_no,
                c_type = fjd2.c_type,
                d_entry = date,
                n_disc = fjd1.n_disc,
                n_discoff = fjd2.n_discoff,
                n_discon = fjd2.n_discon,
                n_netoff = fjd2.n_netoff,
                n_neton = fjd2.n_neton,
                n_qty = fjd1.n_qty,
                n_salpri = fjd1.n_salpri,
                n_sisaoff = fjd2.n_sisaoff,
                n_sisaon = fjd2.n_sisaon,
                v_ket_del = structure.Fields.Keterangan,
                v_type = "03",
                c_taxno = (fjh != null ? fjh.c_taxno : string.Empty),
                d_taxdate = (fjh != null ? fjh.d_taxdate : date)
              });
            }
          }

          if ((listFJD1 != null) && (listFJD1.Count > 0))
          {
            db.LG_FJD1s.DeleteAllOnSubmit(listFJD1.ToArray());

            listFJD1.Clear();
          }

          if ((listFJD2 != null) && (listFJD2.Count > 0))
          {
            db.LG_FJD2s.DeleteAllOnSubmit(listFJD2.ToArray());

            listFJD2.Clear();
          }

          if (fjd3 != null)
          {
            db.LG_FJD3s.DeleteOnSubmit(fjd3);
          }

          if ((listFJD4 != null) && (listFJD4.Count > 0))
          {
            db.LG_FJD4s.InsertAllOnSubmit(listFJD4.ToArray());

            listFJD4.Clear();
          }

          #endregion

          hasAnyChanges = true;

          #region Modif DO

          if(string.IsNullOrEmpty(doId))
          {
            result = "Nomor delivery tidak terbaca";

            hasAnyChanges=false;
          }
          else
          {
            doh = (from q in db.LG_DOHs
                   where (q.c_dono == doId)
                   select q).Take(1).SingleOrDefault();

            if (doh == null)
            {
              result = "Nomor delivery tidak dapat di temukan.";

              hasAnyChanges = false;
            }
            else
            {
              doh.l_sent = false;
              doh.l_send = false;

              hasAnyChanges = true;
            }
          }

          #endregion

          #endregion
        }
        else if (structure.Method.Equals("GenerateTaxNo", StringComparison.OrdinalIgnoreCase))
        {
            int awal = 0,
                akhir = 0,
                loopms = 0;

            string numgenerate = null,
                numgeneratems = null;

            
            //check available number
            listFjh = (from q in db.LG_FJHs
                         join q1 in db.LG_Cusmas on q.c_cusno equals q1.c_cusno
                      where (q.c_taxno == null || q.c_taxno == "")
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && ((q.l_cabang.HasValue ? q.l_cabang.Value : false) == false)
                      //&& q.v_ket_mark == "testing" //&& q.c_fjno == "FJ14038073"
                       select q).OrderBy(x => x.c_fjno).ToList();


            listMsNomorPajak = (from q in db.LG_MsNomorPajaks
                                     //where q.n_akhir < q.n_awal
                                     select q).OrderByDescending(x => x.IDX).ToList();


            if (listFjh.Count > 0)
                {
                    if (listMsNomorPajak.Count > 0)
                    {
                        for (int loop = 0; loop < listFjh.Count; loop++)
                        {
                            fjh = listFjh[loop];

                            for (loopms = 0; loopms < listMsNomorPajak.Count; loopms++)
                            {
                                msnomorpajak = listMsNomorPajak[loopms];

                                awal = Int32.Parse(msnomorpajak.c_current);
                                akhir = Int32.Parse(msnomorpajak.c_akhir);

                                if (akhir - awal >= 0)
                                {
                                    break;
                                }
                            }


                            if (akhir - awal >= 0)
                            {
                                dateGen = (fjh.d_taxdate.HasValue ? fjh.d_taxdate.Value : DateTime.Now);

                                numgenerate = msnomorpajak.c_digit1 + "." + msnomorpajak.c_digit2 + "-" + date.Year.ToString().Substring(2,2) + "." + (awal).ToString("00000000");
                                numgeneratems = (awal+1).ToString("00000000");
                                fjh.c_taxno = numgenerate;

                                msnomorpajak.c_current = numgeneratems;

                                listMsNomorPajak[loopms].c_current = numgeneratems;
                            }
                            else
                            {
                                result = "Nomor pajak tidak cukup, tambahkan master nomor pajak.";

                                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                                goto endLogic;
                            }
                        }
                    }
                    else
                    {
                        result = "Nomor pajak tidak ditemukan, tambahkan master nomor pajak.";

                        rpe = ResponseParser.ResponseParserEnum.IsFailed;

                        goto endLogic;
                    }
                }
                else
                {
                    result = "Nomor faktur tidak ditemukan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    goto endLogic;
                }

                hasAnyChanges = true;

        }

        if (hasAnyChanges)
        {
          db.SubmitChanges();

          db.Transaction.Commit();
          //db.Transaction.Rollback();

          rpe = ResponseParser.ResponseParserEnum.IsSuccess;
        }
        else
        {
          db.Transaction.Rollback();
        }
      }
      catch (Exception ex)
      {
        if (db.Transaction != null)
        {
          db.Transaction.Rollback();
        }

        result = string.Format("ScmsSoaLibrary.Bussiness.Faktur:FakturJual - {0}", ex.Message);

        Logger.WriteLine(result, true);
        Logger.WriteLine(ex.StackTrace);
      }

    endLogic:
      result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

      if (dic != null)
      {
        dic.Clear();
      }

      db.Dispose();

      return result;
    }

    public string FakturBeli(ScmsSoaLibrary.Parser.Class.FakturBeliStructure structure)
    {
      return FakturBeli(structure, null);
    }

    public string FakturBeli(ScmsSoaLibrary.Parser.Class.FakturBeliStructure structure, ORMDataContext dbContext)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      string result = null;

      bool hasAnyChanges = false,
        isImport = false;

      const string RNFLOATING_ID = "RNFLOATING";

      bool isContexted = false;
      ORMDataContext db = null;
      if (dbContext == null)
      {
        db = new ORMDataContext(Functionals.ActiveConnectionString);
      }
      else
      {
        isContexted = true;
        db = dbContext;
      }

      ScmsSoaLibrary.Parser.Class.FakturBeliStructureField field = null;
      ScmsSoaLibrary.Parser.Class.FakturBeliBeaStructureField fieldBea = null;

      string nipEntry = null;
      string fakturID = null,
        rnId = null;
      //string tmpNumbering = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now,
        dateF = DateTime.MinValue;

      decimal totalGross = 0,
        totalDisc = 0,
        totalXDisc = 0,
        totalPajak = 0,
        totalNet = 0,
        totalSisa = 0,
        totalBea = 0,
        dQty = 0,
        dDisc = 0,
        //dGross = 0,
        dPrice = 0,
        n_xpph = 0,
        n_Sumxpph = 0,
        n_TotalBea = 0,
        n_rataPph = 0,
        discExtra = 0,
        totalDiscExtra = 0;


      LG_FBH fbh = null;
      //LG_FBD1 fbd1 = null;
      LG_FBD1 fbd1 = null;
      LG_FBD2 fbd2 = null;
      LG_FBD3 fbd3 = null;

      LG_DatSup sup = null;

      LG_RNH rnh = null;

      //List<LG_FBD1> listFBD1 = null;
      List<LG_FBD1> listFBD1 = null;
      //List<LG_FBD2> listFBD2 = null;
      List<LG_FBD3> listFBD3 = null;
      //List<LG_FBD4> listFBD4 = null;
      List<LG_FBD4> listFBD4 = null;

      List<LG_RND1> listRND1 = null;
      List<LG_RND2> listRND2 = null;
      List<LG_RND1> listRND1Found = null;
      List<LG_RND2> listRND2Found = null;
      //List<LG_RND1> listRND1Change = null;
      List<LG_RND4> listRND4 = null;

      LG_RND1 rnd1 = null;
      LG_RND2 rnd2 = null;
      LG_RND4 rnd4 = null;

      List<string> listItems = null;
      List<string> listTypeRNs = null;

      List<FakturBeliData> listFBDone = null,
        listRNData = null;
      FakturBeliData fbData1 = null;

      int nLoop = 0,
        nLoopC = 0,
        nValue = 0,
        totalDetails = 0;

      IDictionary<string, string> dic = null;
      
      char gudang = (string.IsNullOrEmpty(structure.Fields.Gudang) ? char.MinValue : structure.Fields.Gudang[0]);

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }
      
      fakturID = (structure.Fields.FakturID ?? string.Empty);

      sup = (from q in db.LG_DatSups
             where q.c_nosup == (structure.Fields.Suplier == null ? "00000" : structure.Fields.Suplier)
             select q).SingleOrDefault();

      if (sup != null)
      {
        isImport = (sup.l_import.HasValue ? sup.l_import.Value : false);
      }

      try
      {
        if (!isContexted)
        {
          db.Connection.Open();

          db.Transaction = db.Connection.BeginTransaction();
        }

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          #region Standard Validation Header

          if (gudang == char.MinValue)
          {
            result = "Gudang tidak terbaca.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          } 
          else if (!string.IsNullOrEmpty(fakturID))
          {
            result = "Nomor Faktur ID harus kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Suplier))
          {
            result = "Nama pemasok dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.FakturPrincipal))
          {
            result = "Nomor faktur prinsipal dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (structure.Fields.TanggalFakturDate.Date.Equals(DateTime.MinValue.Date))
          {
            result = "Format tanggal faktur tidak terbaca.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.NoReceive))
          {
            result = "Nomor receive dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Kurs))
          {
            result = "Kurs dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }
          else if (structure.Fields.KursValue < 1)
          {
            result = "Nilai kurs salah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }
          else if (Commons.IsClosingFA(db, structure.Fields.TanggalFakturDate))
          {
            result = "Faktur tidak dapat disimpan pada tanggal ini, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }

          #endregion

          rnId = structure.Fields.NoReceive;
          //fakturID = Commons.GenerateNumbering<LG_FBH>(db, "FB", '3', "30", date, "c_fbno");

          /* Perubahan tgl 01-09-2021 */
          //string maxFakturID = (from f in db.LG_FBHs where f.d_fbdate >= DateTime.Now.AddMonths(-1) orderby f.d_update ?? f.d_entry select f.c_fbno).Take(1).FirstOrDefault();
          //string maxFakturIDPrefix = maxFakturID.Trim().Substring(6);

          //if (maxFakturIDPrefix.Equals("Z999", StringComparison.OrdinalIgnoreCase))
          //{
          //    fakturID = Commons.GenerateNumbering<LG_FBH>(db, "IB", '3', "30", structure.Fields.TanggalFakturDate, "c_fbno");
          //}
          //else
          //{
          //    fakturID = Commons.GenerateNumbering<LG_FBH>(db, "FB", '3', "30", structure.Fields.TanggalFakturDate, "c_fbno");
          //}

          fakturID = Commons.GenerateNumbering<LG_FBH>(db, "FB", '3', "30", structure.Fields.TanggalFakturDate, "c_fbno");
          /* Perubahan tgl 01-09-2021 */

          //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "FB");

          

          fbh = new LG_FBH()
          {
            c_entry = nipEntry,
            c_fb = structure.Fields.FakturPrincipal,
            //c_fbno = "XXXXXXXXXX",
            c_fbno = fakturID,
            c_kurs = structure.Fields.Kurs,
            c_nosup = structure.Fields.Suplier,
            c_taxno = structure.Fields.NoTax,
            c_update = nipEntry,
            d_entry = date,
            d_fbdate = structure.Fields.TanggalFakturDate,
            d_taxdate = structure.Fields.TanggalTaxDate,
            d_top = structure.Fields.TanggalFakturDate,
            d_toppjg = structure.Fields.TanggalFakturDate,
            d_update = date,
            l_bea = false,
            l_ppn = (isImport ? false : true),
            l_print = false,
            n_bea = 0,
            n_bilva = 0,
            n_bruto = 0,
            n_disc = 0,
            n_kurs = structure.Fields.KursValue,
            n_pdisc = structure.Fields.XDisc,
            n_ppn = 0,
            n_sisa = 0,
            n_top = 0,
            n_toppjg = 0,
            n_xdisc = 0,
            n_bilva_faktur = structure.Fields.ValueFaktur,
            n_ppph = structure.Fields.Npph,
            n_xpph = structure.Fields.Xpph,
            v_ket = structure.Fields.Keterangan            
          };

          nValue = (fbh.n_top.HasValue ? (int)fbh.n_top.Value : 0);
          dateF = (fbh.d_top.HasValue ? fbh.d_top.Value : (fbh.d_fbdate.HasValue ? fbh.d_fbdate.Value : DateTime.MinValue));
          if (!nValue.Equals(structure.Fields.Top))
          {
            nValue = (structure.Fields.Top - nValue);

            fbh.n_top = structure.Fields.Top;
            fbh.d_top = dateF.AddDays(nValue);
          }

          nValue = (fbh.n_toppjg.HasValue ? (int)fbh.n_toppjg.Value : 0);
          dateF = (fbh.d_toppjg.HasValue ? fbh.d_toppjg.Value : (fbh.d_fbdate.HasValue ? fbh.d_fbdate.Value : DateTime.MinValue));
          if (!nValue.Equals(structure.Fields.TopPjg))
          {
            nValue = (structure.Fields.TopPjg - nValue);

            fbh.n_toppjg = structure.Fields.TopPjg;
            fbh.d_toppjg = dateF.AddDays(nValue);
          }

          db.LG_FBHs.InsertOnSubmit(fbh);

          //db.SubmitChanges();

          //fbh = (from q in db.LG_FBHs
          //       where q.v_ket == tmpNumbering
          //       select q).Take(1).SingleOrDefault();

          //if (fbh.c_fbno.Equals("XXXXXXXXXX"))
          //{
          //  result = "Trigger Faktur Beli tidak aktif.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  db.Transaction.Rollback();

          //  goto endLogic;
          //}

          //fbh.v_ket = structure.Fields.Keterangan;

          //fakturID = fbh.c_fbno.Trim();

          #region List Relasi RN -> FB

          listFBDone = (from q in db.LG_FBD1s
                        join q1 in db.LG_FBD2s on q.c_fbno equals q1.c_fbno
                        where (q1.c_rnno == structure.Fields.NoReceive)
                        select new FakturBeliData()
                        {
                          NoItem  = q.c_iteno,
                          TipeItem = q.c_type
                        }).Distinct().ToList();
          listFBDone = (listFBDone == null ? new List<FakturBeliData>() : listFBDone);

          listRNData = (from q in db.LG_RND2s                        
                        where (q.c_rnno == structure.Fields.NoReceive) 
                          && (q.c_gdg == gudang)
                        select new FakturBeliData()
                        {
                          NoItem = q.c_iteno,
                          TipeItem = q.c_type
                        }).Distinct().ToList();
          listRNData = (listRNData == null ? new List<FakturBeliData>() : listRNData);

          #endregion

          #region Insert Detail

          totalGross =
            totalDisc = 0;

          db.LG_FBD2s.InsertOnSubmit(new LG_FBD2()
          {
            c_fbno = fakturID,
            c_rnno = structure.Fields.NoReceive
          });

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listFBD1 = new List<LG_FBD1>();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              if (listFBDone.Exists(delegate(FakturBeliData data1)
              {
                return field.Item.Equals((string.IsNullOrEmpty(data1.NoItem) ? string.Empty: data1.NoItem.Trim()), StringComparison.OrdinalIgnoreCase) &&
                  field.TipeBarang.Equals((string.IsNullOrEmpty(data1.TipeItem) ? string.Empty : data1.TipeItem.Trim()), StringComparison.OrdinalIgnoreCase);
              }))
              {
                continue;
              }

              /* Merubah `structure.Fields.XDisc` menjadi `field.n_discextra` */
              //n_xpph = ((field.Quantity * field.Harga) - ((field.Quantity * field.Harga * field.Discount) / 100) -
              // ((field.Quantity * field.Harga * field.n_discextra) / 100)) * (field.n_ppph / 100);

              n_xpph = ((field.Quantity * field.Harga) - ((field.Quantity * field.Harga * field.Discount) / 100) -
               ((field.Quantity * field.Harga * structure.Fields.XDisc) / 100)) * (field.n_ppph / 100);

              n_Sumxpph += n_xpph;

              n_rataPph += field.n_ppph;

              listFBD1.Add(new LG_FBD1()
              {
                c_fbno = fakturID,
                c_iteno = field.Item,
                c_type = field.TipeBarang,
                n_bea = field.Bea,
                n_disc = field.Discount,
                n_qty = field.Quantity,
                n_salpri = field.Harga,
                n_ppph = field.n_ppph,
                l_pph = false
                //,
                //n_discextra = field.n_discextra
              });

              listFBDone.Add(new FakturBeliData()
              {
                NoItem = field.Item,
                TipeItem = field.TipeBarang
              });

              dPrice = (field.Quantity * field.Harga);
              dDisc = (dPrice * (field.Discount / 100));
              //discExtra = (dPrice * (field.n_discextra / 100));

              totalGross += dPrice;
              totalDisc += dDisc;
              //totalDiscExtra += discExtra;

              totalDetails++;
            }
          }

          if ((structure.ExtraFields != null) && (structure.ExtraFields.FieldBea != null) && (structure.ExtraFields.FieldBea.Length > 0))
          {
            listFBD3 = new List<LG_FBD3>();

            for (nLoop = 0; nLoop < structure.ExtraFields.FieldBea.Length; nLoop++)
            {
              fieldBea = structure.ExtraFields.FieldBea[nLoop];

              listFBD3.Add(new LG_FBD3()
              {
                c_fbno = fakturID,
                c_exp = fieldBea.Expeditur,
                c_type = fieldBea.TypeBea,
                d_top = fieldBea.TanggalDate,
                n_sisa = fieldBea.Value,
                n_value = fieldBea.Value
              });

              totalBea += fieldBea.Value;
            }
          }


          //totalXDisc = (totalGross * (totalDiscExtra / 100));
          totalXDisc = (totalGross * (structure.Fields.XDisc / 100)); 

          if (!isImport)
          {
            decimal ppn = db.fn_GetTax("PPN", structure.Fields.TanggalTaxDate) ?? decimal.Zero;
            totalPajak = ((totalGross - totalDisc - totalXDisc) * 0.1m);
          }
          else
          {
            totalPajak = 0;
          }
          //totalNet = ((totalGross - (totalDisc + totalXDisc)) + (totalPajak + totalBea));
          totalNet = ((totalGross - (totalDisc + totalXDisc)) + totalPajak + n_Sumxpph);

          fbh.n_xdisc = totalXDisc;
          fbh.n_bruto = totalGross;
          fbh.n_bea = totalBea;
          fbh.n_disc = totalDisc;
          fbh.n_ppn = totalPajak;
          fbh.n_sisa =
            fbh.n_bilva = totalNet;

          n_rataPph = n_rataPph / structure.Fields.Field.Count();

          fbh.n_xpph = n_Sumxpph;
          fbh.n_ppph = n_rataPph;

          if ((listFBD1 != null) && (listFBD1.Count > 0))
          {
            db.LG_FBD1s.InsertAllOnSubmit(listFBD1.ToArray());

            listFBD1.Clear();
          }

          if ((listFBD3 != null) && (listFBD3.Count > 0))
          {
            db.LG_FBD3s.InsertAllOnSubmit(listFBD3.ToArray());

            listFBD3.Clear();
          }

          hasAnyChanges = true;

          #region Modif RN

          nLoopC = 0;

          for (nLoop = 0; nLoop < listRNData.Count; nLoop++)
          {
            fbData1 = listRNData[nLoop];

            if (listFBDone.Exists(delegate(FakturBeliData data1)
            {
              return fbData1.NoItem.Equals(data1.NoItem, StringComparison.OrdinalIgnoreCase) &&
                fbData1.TipeItem.Equals(data1.TipeItem, StringComparison.OrdinalIgnoreCase); 
            }))
            {
              nLoopC++;
            }
          }
          
          if (nLoopC == listRNData.Count)
          {
            rnh = (from q in db.LG_RNHs
                   where (q.c_rnno == rnId) && (q.c_gdg == gudang)
                   select q).Take(1).SingleOrDefault();

            if (rnh == null)
            {
              hasAnyChanges = false;

              rpe = ResponseParser.ResponseParserEnum.IsFailed;

              result = "Nomor receive tidak dapat di temukan.";
            }
            else
            {
              rnh.l_status = true;

              //rnh.c_update = nipEntry;
              //rnh.d_update = date;

              dic = new Dictionary<string, string>();

              dic.Add("Faktur", fakturID);
              dic.Add("Tanggal", structure.Fields.TanggalFakturDate.ToString("yyyyMMdd"));
              dic.Add("Net", totalNet.ToString());

              result = string.Format("Total {0} detail(s)", totalDetails);
            }
          }
          else
          {
            dic = new Dictionary<string, string>();

            dic.Add("Faktur", fakturID);
            dic.Add("Tanggal", structure.Fields.TanggalFakturDate.ToString("yyyyMMdd"));
            dic.Add("Net", totalNet.ToString());

            result = string.Format("Total {0} detail(s)", totalDetails);
          }
          
          #endregion

          if (listFBDone != null)
          {
            listFBDone.Clear();
          }
          if (listRNData != null)
          {
            listRNData.Clear();
          }

          #endregion

          #endregion
        }
        else if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify

          if (string.IsNullOrEmpty(fakturID))
          {
            result = "Nomor faktur beli dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          fbh = (from q in db.LG_FBHs
                 where q.c_fbno == fakturID
                 select q).Take(1).SingleOrDefault();

          if (fbh == null)
          {
            result = "Nomor faktur beli tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (fbh.l_delete.HasValue && fbh.l_delete.Value)
          {
            result = "Tidak dapat mengubah nomor faktur beli yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.FakturPrincipal))
          {
            result = "Nomor faktur prinsipal dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingFA(db, fbh.d_fbdate.Value))
          {
            result = "Faktur tidak dapat diubah, karena tanggal pada transaksi ini sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
          {
            fbh.v_ket = structure.Fields.Keterangan;
          }

          fbh.n_pdisc = structure.Fields.XDisc;

          nValue = (fbh.n_top.HasValue ? (int)fbh.n_top.Value : 0);
          dateF = (fbh.d_top.HasValue ? fbh.d_top.Value : (fbh.d_fbdate.HasValue ? fbh.d_fbdate.Value : DateTime.MinValue));
          if (!nValue.Equals(structure.Fields.Top))
          {
            nValue = (structure.Fields.Top - nValue);

            fbh.n_top = structure.Fields.Top;
            //Indra D. 20170427
            //fbh.d_top = dateF.AddDays(nValue);
            fbh.d_top = structure.Fields.TanggalFakturDate.AddDays(structure.Fields.Top);
          }

          fbh.d_fbdate = structure.Fields.TanggalFakturDate;

          nValue = (fbh.n_toppjg.HasValue ? (int)fbh.n_toppjg.Value : 0);
          dateF = (fbh.d_toppjg.HasValue ? fbh.d_toppjg.Value : (fbh.d_fbdate.HasValue ? fbh.d_fbdate.Value : DateTime.MinValue));
          if (!nValue.Equals(structure.Fields.TopPjg))
          {
            nValue = (structure.Fields.TopPjg - nValue);

            fbh.n_toppjg = structure.Fields.TopPjg;
            //Indra D. 20170427
            //fbh.d_toppjg = dateF.AddDays(nValue);
            fbh.d_toppjg = structure.Fields.TanggalFakturDate.AddDays(structure.Fields.Top);
          }

          fbh.c_fb = structure.Fields.FakturPrincipal;
          fbh.n_bilva_faktur = structure.Fields.ValueFaktur;
          fbh.c_taxno = structure.Fields.NoTax;
          fbh.d_taxdate = structure.Fields.TanggalTaxDate;

          fbh.c_kurs = structure.Fields.Kurs;
          fbh.n_kurs = structure.Fields.KursValue;

          fbh.c_update = nipEntry;
          fbh.d_update = date;

          #region List Relasi RN -> FB

          listFBDone = new List<FakturBeliData>();

          #endregion

          #region Populate Detail

          listFBD1 = (from q in db.LG_FBD1s
                      where q.c_fbno == fakturID
                      select q).ToList();

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            fbd2 = (from q in db.LG_FBD2s
                    where q.c_fbno == fakturID
                    select q).Take(1).SingleOrDefault();

            rnId = (fbd2 != null ? fbd2.c_rnno : string.Empty);

            listFBD4 = new List<LG_FBD4>();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              fbd1 = listFBD1.Find(delegate(LG_FBD1 fbd)
              {
                return field.Item.Equals(fbd.c_iteno) &&
                  field.TipeBarang.Equals(fbd.c_type);
              });

              if (fbd1 != null)
              {
                if (field.IsModified && (!field.IsDelete))
                {
                  #region Modify

                  dQty = (fbd1.n_qty.HasValue ? fbd1.n_qty.Value : 0);
                  dQty = field.Quantity;

                  dPrice = (field.Harga * dQty);

                  dDisc = (dPrice * (field.Discount / 100));
                  //discExtra = (dPrice * (field.n_discextra / 100));

                  listFBD4.Add(new LG_FBD4()
                  {
                     c_entry = nipEntry,
                     c_fbno = fakturID,
                     c_iteno = field.Item,
                     c_rnno = rnId,
                     c_type = fbd1.c_type,
                     d_entry = date,
                     n_bea = fbd1.n_bea,
                     n_disc = fbd1.n_disc,
                     n_qty = fbd1.n_qty,
                     n_salpri = fbd1.n_salpri,
                     v_type = "02"
                     //,
                     //n_discextra = discExtra
                  });

                  fbd1.n_qty = field.Quantity;

                  fbd1.n_salpri = field.Harga;
                  fbd1.n_disc = field.Discount;
                  fbd1.n_bea = field.Bea;
                  fbd1.n_ppph = field.n_ppph;
                  //fbd1.n_discextra = field.n_discextra;

            //      n_xpph = ((field.Quantity * field.Harga) - ((field.Quantity * field.Harga * field.Discount) / 100) -
            //((field.Quantity * field.Harga * field.n_discextra) / 100)) * (field.n_ppph / 100);

                  n_xpph = ((field.Quantity * field.Harga) - ((field.Quantity * field.Harga * field.Discount) / 100) -
            ((field.Quantity * field.Harga * structure.Fields.XDisc) / 100)) * (field.n_ppph / 100);

                  n_Sumxpph += n_xpph;

                  n_rataPph += field.n_ppph;


                  #endregion
                }
                else if ((!field.IsModified) && field.IsDelete && (!rnId.Equals(RNFLOATING_ID)))
                {
                  #region Delete

                  listFBD4.Add(new LG_FBD4()
                  {
                    c_entry = nipEntry,
                    c_fbno = fakturID,
                    c_iteno = field.Item,
                    c_rnno = rnId,
                    c_type = fbd1.c_type,
                    d_entry = date,
                    n_bea = fbd1.n_bea,
                    n_disc = fbd1.n_disc,
                    n_qty = fbd1.n_qty,
                    n_salpri = fbd1.n_salpri,
                    v_type = "03",
                    v_ket_del = field.KeteranganMod
                    //,
                    //n_discextra = field.n_discextra
                  });
                  db.LG_FBD1s.DeleteOnSubmit(fbd1);

                  listFBD1.Remove(fbd1);

                  if (!listFBDone.Exists(delegate(FakturBeliData data1)
                  {
                    return field.Item.Equals(data1.NoItem, StringComparison.OrdinalIgnoreCase) &&
                      field.TipeBarang.Equals(data1.TipeItem, StringComparison.OrdinalIgnoreCase);
                  }))
                  {
                    listFBDone.Add(new FakturBeliData()
                    {
                      NoItem = field.Item,
                      TipeItem = fbd1.c_type
                    });
                  }

                  #endregion
                }
              }
            }

            n_rataPph = n_rataPph / structure.Fields.Field.Count();

            fbh.n_xpph = n_Sumxpph;
            fbh.n_ppph = n_rataPph;

            if ((listFBD4 != null) && (listFBD4.Count > 0))
            {
              db.LG_FBD4s.InsertAllOnSubmit(listFBD4.ToArray());

              listFBD4.Clear();
            }
          }

          #endregion

          #region Populate Bea

          listFBD3 = (from q in db.LG_FBD3s
                      where q.c_fbno == fakturID
                        && (q.l_delete == null || q.l_delete == false)
                      select q).ToList();

          if ((structure.ExtraFields != null) && (structure.ExtraFields.FieldBea != null) && (structure.ExtraFields.FieldBea.Length > 0))
          {
            for (nLoop = 0; nLoop < structure.ExtraFields.FieldBea.Length; nLoop++)
            {
              fieldBea = structure.ExtraFields.FieldBea[nLoop];

              if (fieldBea.IsNew && (!fieldBea.IsModified) && (!fieldBea.IsDelete))
              {
                #region New

                fbd3 = new LG_FBD3()
                {
                  c_exp = fieldBea.Expeditur,
                  c_fbno = fakturID,
                  c_type = fieldBea.TypeBea,
                  d_top = fieldBea.TanggalDate,
                  n_sisa = fieldBea.Value,
                  n_value = fieldBea.Value
                };

                listFBD3.Add(fbd3);

                db.LG_FBD3s.InsertOnSubmit(fbd3);

                #endregion
              }
              else if ((!fieldBea.IsNew) && (fieldBea.IsModified) && (!fieldBea.IsDelete))
              {
                #region Modify

                fbd3 = listFBD3.Find(delegate(LG_FBD3 fbd)
                {
                  if (fbd.l_delete == null || fbd.l_delete == false)
                  {
                    return (((fbd.c_type != null) && (fbd.c_exp != null)) ?
                      fbd.c_type.Equals(fieldBea.TypeBea, StringComparison.OrdinalIgnoreCase) && fbd.c_exp.Equals(fieldBea.Expeditur, StringComparison.OrdinalIgnoreCase) :
                      false);
                  }

                  return false;
                });

                fbd3.n_sisa = fieldBea.Value;
                fbd3.n_value = fieldBea.Value;

                #endregion
              }
              else if ((!fieldBea.IsNew) && (!fieldBea.IsModified) && fieldBea.IsDelete)
              {
                #region Deleted

                fbd3 = listFBD3.Find(delegate(LG_FBD3 fbd)
                {
                  if (fbd.l_delete == null || fbd.l_delete == false)
                  {
                    return (((fbd.c_type != null) && (fbd.c_exp != null)) ?
                      fbd.c_type.Equals(fieldBea.TypeBea, StringComparison.OrdinalIgnoreCase) && fbd.c_exp.Equals(fieldBea.Expeditur, StringComparison.OrdinalIgnoreCase) :
                      false);
                  }

                  return false;
                });

                if (fbd3 != null)
                {
                  fbd3.l_delete = true;
                  fbd3.v_ket_del = fieldBea.KeteranganMod;
                }

                #endregion
              }
            }
          }

          #endregion

          #region Recalculate All

          totalGross = totalDisc = totalDiscExtra = 0;

          if ((listFBD1 != null) && (listFBD1.Count > 0))
          {
            for (nLoop = 0; nLoop < listFBD1.Count; nLoop++)
            {
              fbd1 = listFBD1[nLoop];

              if (fbd1 != null)
              {
                
                dQty = (fbd1.n_qty.HasValue ? fbd1.n_qty.Value : 0);
                dPrice = ((fbd1.n_salpri.HasValue ? fbd1.n_salpri.Value : 0) * dQty);

                dDisc = (dPrice * ((fbd1.n_disc.HasValue ? fbd1.n_disc.Value : 0) / 100));
                //discExtra = (dPrice * ((fbd1.n_discextra.HasValue ? fbd1.n_discextra.Value : 0) / 100));

                //if (fbd1.n_ppph > 0)
                //{
                //  n_xpph = dPrice - (dDisc) - ((dQty * (fbd1.n_salpri.HasValue ? fbd1.n_salpri.Value : 0) * structure.Fields.XDisc / 100) * ((fbd1.n_ppph.HasValue ? fbd1.n_ppph.Value : 0)/ 100 ));
                //  fbd1.l_pph = true;
                //  n_TotalxPph += n_xpph;
                //}

                totalGross += dPrice;
                totalDisc += dDisc;
                //totalDiscExtra += discExtra;
              }
            }
          }

          //fbh.n_xpph = n_TotalxPph;

          if ((listFBD3 != null) && (listFBD3.Count > 0))
          {
            for (nLoop = 0; nLoop < listFBD3.Count; nLoop++)
            {
              fbd3 = listFBD3[nLoop];

              if ((fbd3.l_delete == null) || (fbd3.l_delete == false))
              {
                totalBea += (fbd3.n_value.HasValue ? fbd3.n_value.Value : 0);
              }
            }
          }

          #endregion

          #region Replace

          //totalXDisc = totalDiscExtra;
          totalXDisc = structure.Fields.XDiscVal;

          if (!isImport)
          {
            decimal ppn = db.fn_GetTax("PPN", structure.Fields.TanggalTaxDate) ?? decimal.Zero;
            totalPajak = ((totalGross - (totalDisc + totalXDisc)) * ppn);
          }
          else
          {
            totalPajak = 0;
          }
          
          //totalNet = ((totalGross - (totalDisc + totalXDisc)) + (totalPajak + totalBea));
            totalNet = ((totalGross - (totalDisc + totalXDisc)) + totalPajak + (n_Sumxpph - (fbh.n_ppph.HasValue ? fbh.n_ppph.Value : 0)));

          totalSisa = ((fbh.n_bilva.HasValue ? fbh.n_bilva.Value : 0) - (fbh.n_sisa.HasValue ? fbh.n_sisa.Value : 0));

          dPrice = (totalNet - totalSisa);

          fbh.n_xdisc = totalXDisc;
          fbh.n_bruto = totalGross;
          fbh.n_bea = totalBea;
          fbh.n_disc = totalDisc;
          fbh.n_ppn = totalPajak;
          fbh.n_bilva = totalNet;
          fbh.n_sisa = dPrice;

          //totalPajak = (isCabang ? 0 : ((totalGross - totalDisc) * 0.1m));
          //totalNet = ((totalGross - totalDisc) + totalPajak);

          //totalSisa = ((fjh.n_net.HasValue ? fjh.n_net.Value : 0) - (fjh.n_sisa.HasValue ? fjh.n_sisa.Value : 0));

          //dPrice = (totalNet - totalSisa);

          //fjh.n_gross = totalGross;
          //fjh.n_disc = totalDisc;
          //fjh.n_tax = totalPajak;
          //fjh.n_net = totalNet;
          //fjh.n_sisa = dPrice;

          dic = new Dictionary<string, string>();

          dic.Add("Faktur", fakturID);
          dic.Add("Sisa", Functionals.DecimalToString(dPrice));
          dic.Add("Net", Functionals.DecimalToString(totalNet));

          #endregion

          hasAnyChanges = true;

          #region Modif RN
          
          if (listFBDone.Count > 0)
          {
            rnh = (from q in db.LG_RNHs
                   where (q.c_rnno == rnId) && (q.c_gdg == gudang)
                   select q).Take(1).SingleOrDefault();

            if (rnh == null)
            {
              hasAnyChanges = false;

              rpe = ResponseParser.ResponseParserEnum.IsFailed;

              result = "Nomor receive tidak dapat di temukan.";
            }
            else
            {
              rnh.l_status = false;

              //rnh.c_update = nipEntry;
              //rnh.d_update = date;
            }
          }

          #endregion

          if (listFBDone != null)
          {
            listFBDone.Clear();
          }

          #endregion
        }
        else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete

          if (gudang == char.MinValue)
          {
            result = "Gudang tidak terbaca.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(fakturID))
          {
            result = "Nomor faktur beli dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          fbh = (from q in db.LG_FBHs
                 where q.c_fbno == fakturID
                 select q).Take(1).SingleOrDefault();

          if (fbh == null)
          {
            result = "Nomor faktur beli tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (fbh.l_delete.HasValue && fbh.l_delete.Value)
          {
            result = "Tidak dapat mengubah nomor faktur beli yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingFA(db, fbh.d_fbdate.Value))
          {
            result = "Faktur tidak dapat diubah, karena tanggal pada transaksi ini sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          fbh.c_update = nipEntry;
          fbh.d_update = DateTime.Now;

          fbh.l_delete = true;
          fbh.v_ket_mark = structure.Fields.Keterangan;

          #region Hapus Detail

          listFBD1 = (from q in db.LG_FBD1s
                      where q.c_fbno == fakturID
                      select q).ToList();

          fbd2 = (from q in db.LG_FBD2s
                  where q.c_fbno == fakturID
                  select q).Take(1).SingleOrDefault();

          listFBD3 = (from q in db.LG_FBD3s
                      where q.c_fbno == fakturID
                      select q).ToList();


          rnId = (fbd2 != null ? fbd2.c_rnno : string.Empty);

          listFBD4 = new List<LG_FBD4>();

          #region Delete

          for (nLoop = 0; nLoop < listFBD1.Count; nLoop++)
          {
            fbd1 = listFBD1[nLoop];

            if (fbd1 != null)
            {

              listFBD4.Add(new LG_FBD4()
              {
                c_entry = nipEntry,
                c_fbno = fakturID,
                c_iteno = fbd1.c_iteno,
                c_rnno = rnId,
                c_type = fbd1.c_type,
                d_entry = date,
                n_bea = fbd1.n_bea,
                n_disc = fbd1.n_disc,
                n_qty = fbd1.n_qty,
                n_salpri = fbd1.n_salpri,
                v_type = "03",
                v_ket_del = structure.Fields.Keterangan
                //,
                //n_discextra = field.n_discextra
              });
            }
          }

          for (nLoop = 0; nLoop < listFBD3.Count; nLoop++)
          {
            fbd3 = listFBD3[nLoop];

            if(fbd3 != null)
            {
              fbd3.l_delete = true;
              fbd3.v_ket_del = structure.Fields.Keterangan;
            }
          }

          #endregion

          if ((listFBD1 != null) && (listFBD1.Count > 0))
          {
            db.LG_FBD1s.DeleteAllOnSubmit(listFBD1.ToArray());

            listFBD1.Clear();
          }

          if (fbd2 != null)
          {
            db.LG_FBD2s.DeleteOnSubmit(fbd2);
          }

          if ((listFBD4 != null) && (listFBD4.Count > 0))
          {
            db.LG_FBD4s.InsertAllOnSubmit(listFBD4.ToArray());

            listFBD4.Clear();
          }

          #endregion

          #region Modif RN

          if (rnId.Equals(RNFLOATING_ID))
          {
            listRND4 = (from q in db.LG_RND4s
                        where (q.c_gdg == gudang) && (q.c_fb == fakturID)
                        select q).Distinct().ToList();

            if (listRND4 != null)
            {
              listTypeRNs = new List<string>();

              listTypeRNs.Add("01");
              listTypeRNs.Add("06");

              for (nLoop = 0; nLoop < listRND4.Count; nLoop++)
              {
                rnd4 = listRND4[nLoop];

                rnd1 = (from q in db.LG_RNHs
                        join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                        where (q1.c_gdg == gudang) && (q1.c_rnno == rnd4.c_rnno)
                          && (q1.c_iteno == rnd4.c_iteno) && (q1.c_batch == rnd4.c_batch)
                          && (listTypeRNs.Contains(q.c_type))
                          && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                        select q1).Take(1).SingleOrDefault();

                if (rnd1 != null)
                {
                  //rnd1.n_floqty += (rnd4.n_qty.HasValue ? rnd4.n_qty.Value : 0);
                }

                #region Old Coded

                //listRND1 = (from q in db.LG_RND1s
                //            where (q.c_gdg == gudang) && (q.c_rnno == rnd4.c_rnno)
                //              && (q.c_iteno == rnd4.c_iteno) && (q.c_batch == q.c_batch)
                //            select q).ToList();

                //if (listRND1 != null)
                //{
                //  for (nLoopC = 0; nLoopC < listRND1.Count; nLoopC++)
                //  {
                //    rnd1 = listRND1[nLoopC];
                //  }

                //  listRND1.Clear();
                //}

                #endregion
              }

              listTypeRNs.Clear();

              db.LG_RND4s.DeleteAllOnSubmit(listRND4.ToArray());
              listRND4.Clear();
            }
            
            hasAnyChanges = true;
          }
          else
          {
            rnh = (from q in db.LG_RNHs
                   where (q.c_rnno == rnId) && (q.c_gdg == gudang)
                   select q).Take(1).SingleOrDefault();

            if (rnh == null)
            {
              hasAnyChanges = false;

              rpe = ResponseParser.ResponseParserEnum.IsFailed;

              result = "Nomor receive tidak dapat di temukan.";
            }
            else
            {
              rnh.l_status = false;

              hasAnyChanges = true;
            }
          }

          #endregion

          #endregion
        }
        else if (structure.Method.Equals("AddFloating", StringComparison.OrdinalIgnoreCase))
        {
          #region Add Floating
          
          #region Standard Validation Header

          if (gudang == char.MinValue)
          {
            result = "Gudang tidak terbaca.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (!string.IsNullOrEmpty(fakturID))
          {
            result = "Nomor Faktur ID harus kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Suplier))
          {
            result = "Nama pemasok dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.FakturPrincipal))
          {
            result = "Nomor faktur prinsipal dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (structure.Fields.TanggalFakturDate.Date.Equals(DateTime.MinValue.Date))
          {
            result = "Format tanggal faktur tidak terbaca.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Kurs))
          {
            result = "Kurs dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }
          else if (structure.Fields.KursValue < 1)
          {
            result = "Nilai kurs salah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }
          else if (Commons.IsClosingFA(db, structure.Fields.TanggalFakturDate))
          {
            result = "Faktur tidak dapat disimpan pada tanggal ini, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }

          #endregion

          /* Perubahan tgl 01-09-2021 */
          string maxFakturID = (from f in db.LG_FBHs where f.d_fbdate >= DateTime.Now.AddMonths(-1) orderby f.d_update ?? f.d_entry select f.c_fbno).Take(1).FirstOrDefault();
          string maxFakturIDPrefix = maxFakturID.Trim().Substring(6);

          if (maxFakturIDPrefix.Equals("Z999", StringComparison.OrdinalIgnoreCase))
          {
              fakturID = Commons.GenerateNumbering<LG_FBH>(db, "IB", '3', "30", date, "c_fbno");
          }
          else
          {
              fakturID = Commons.GenerateNumbering<LG_FBH>(db, "FB", '3', "30", date, "c_fbno");
          }

          //fakturID = Commons.GenerateNumbering<LG_FBH>(db, "FB", '3', "30", date, "c_fbno");
          /* Perubahan tgl 01-09-2021 */
 
          //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "FB");

          fbh = new LG_FBH()
          {
            c_entry = nipEntry,
            c_fb = structure.Fields.FakturPrincipal,
            c_fbno = fakturID,
            c_kurs = structure.Fields.Kurs,
            c_nosup = structure.Fields.Suplier,
            c_taxno = structure.Fields.NoTax,
            c_update = nipEntry,
            d_entry = date,
            d_fbdate = structure.Fields.TanggalFakturDate,
            d_taxdate = structure.Fields.TanggalTaxDate,
            d_top = structure.Fields.TanggalFakturDate,
            d_toppjg = structure.Fields.TanggalFakturDate,
            d_update = date,
            l_bea = false,
            l_ppn = true,
            l_print = false,
            n_bea = 0,
            n_bilva = 0,
            n_bruto = 0,
            n_disc = 0,
            n_kurs = structure.Fields.KursValue,
            n_pdisc = structure.Fields.XDisc,
            n_ppn = 0,
            n_sisa = 0,
            n_top = 0,
            n_toppjg = 0,
            n_xdisc = 0,
            n_ppph = structure.Fields.Npph,
            n_xpph = structure.Fields.Xpph,
            n_bilva_faktur = structure.Fields.ValueFaktur,
            v_ket = structure.Fields.Keterangan,
          };
          
          nValue = (fbh.n_top.HasValue ? (int)fbh.n_top.Value : 0);
          dateF = (fbh.d_top.HasValue ? fbh.d_top.Value : (fbh.d_fbdate.HasValue ? fbh.d_fbdate.Value : DateTime.MinValue));
          if (!nValue.Equals(structure.Fields.Top))
          {
            nValue = (structure.Fields.Top - nValue);

            fbh.n_top = structure.Fields.Top;
            fbh.d_top = dateF.AddDays(nValue);
          }

          nValue = (fbh.n_toppjg.HasValue ? (int)fbh.n_toppjg.Value : 0);
          dateF = (fbh.d_toppjg.HasValue ? fbh.d_toppjg.Value : (fbh.d_fbdate.HasValue ? fbh.d_fbdate.Value : DateTime.MinValue));
          if (!nValue.Equals(structure.Fields.TopPjg))
          {
            nValue = (structure.Fields.TopPjg - nValue);

            fbh.n_toppjg = structure.Fields.TopPjg;
            fbh.d_toppjg = dateF.AddDays(nValue);
          }

          #region Insert Detail

          totalGross =
            totalDisc = 0;

          db.LG_FBD2s.InsertOnSubmit(new LG_FBD2()
          {
            c_fbno = fakturID,
            c_rnno = RNFLOATING_ID
          });

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listFBD1 = new List<LG_FBD1>();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              n_xpph = ((field.Quantity * field.Harga) - ((field.Quantity * field.Harga * field.Discount) / 100) -
            ((field.Quantity * field.Harga * structure.Fields.XDisc) / 100)) * (field.n_ppph / 100);

              n_Sumxpph += n_xpph;

              n_rataPph += field.n_ppph;

              listFBD1.Add(new LG_FBD1()
              {
                c_fbno = fakturID,
                c_iteno = field.Item,
                c_type = string.IsNullOrEmpty(field.TipeBarang) ? "01" : field.TipeBarang,
                n_bea = field.Bea,
                n_disc = field.Discount,
                n_qty = field.Quantity,
                n_salpri = field.Harga,
                n_ppph = field.n_ppph,
                l_pph = field.n_ppph == 0 ? false : true,
              });

              dPrice = (field.Quantity * field.Harga);
              dDisc = (dPrice * (field.Discount / 100));

              totalGross += dPrice;
              totalDisc += dDisc;

              totalDetails++;
            }
          }

          if ((structure.ExtraFields != null) && (structure.ExtraFields.FieldBea != null) && (structure.ExtraFields.FieldBea.Length > 0))
          {
            listFBD3 = new List<LG_FBD3>();

            for (nLoop = 0; nLoop < structure.ExtraFields.FieldBea.Length; nLoop++)
            {
              fieldBea = structure.ExtraFields.FieldBea[nLoop];

              listFBD3.Add(new LG_FBD3()
              {
                c_fbno = fakturID,
                c_exp = fieldBea.Expeditur,
                c_type = fieldBea.TypeBea,
                d_top = fieldBea.TanggalDate,
                n_sisa = fieldBea.Value,
                n_value = fieldBea.Value
              });

              totalBea += fieldBea.Value;
            }
          }

          totalXDisc = (totalGross * (structure.Fields.XDisc / 100));
          if (!isImport)
          {
              decimal ppn = db.fn_GetTax("PPN", structure.Fields.TanggalTaxDate) ?? decimal.Zero;
              totalPajak = ((totalGross - totalDisc - totalXDisc) * ppn);
          }
          else
          {
              totalPajak = 0;
          }
          //totalNet = ((totalGross - (totalDisc + totalXDisc)) + (totalPajak + totalBea));
          totalNet = ((totalGross - (totalDisc + totalXDisc)) + totalPajak + n_Sumxpph);

          fbh.n_xdisc = totalXDisc;
          fbh.n_bruto = totalGross;
          fbh.n_bea = totalBea;
          fbh.n_disc = totalDisc;
          fbh.n_ppn = totalPajak;
          fbh.n_sisa =
            fbh.n_bilva = totalNet;

          n_rataPph = n_rataPph / structure.Fields.Field.Count();

          fbh.n_xpph = n_Sumxpph;
          fbh.n_ppph = n_rataPph;

          if ((listFBD1 != null) && (listFBD1.Count > 0))
          {
            db.LG_FBD1s.InsertAllOnSubmit(listFBD1.ToArray());

            listFBD1.Clear();
          }

          if ((listFBD3 != null) && (listFBD3.Count > 0))
          {
            db.LG_FBD3s.InsertAllOnSubmit(listFBD3.ToArray());

            listFBD3.Clear();
          }

          db.LG_FBHs.InsertOnSubmit(fbh);

          hasAnyChanges = true;
          
          #region Modif RN

          listTypeRNs = new List<string>();

          listTypeRNs.Add("01");
          listTypeRNs.Add("06");

          listItems = structure.Fields.Field.GroupBy(x => x.Item).Select(y => y.Key).ToList();

          listRND2 = (from q in db.LG_RNHs
                      join q1 in db.LG_RND2s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                      where (q.l_float == true) && (q.c_gdg == gudang)
                        //&& (q1.n_floqty > 0) 
                        && listItems.Contains(q1.c_iteno)
                        && listTypeRNs.Contains(q.c_type)
                        && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      select q1).Distinct().ToList();

          listItems.Clear();
          listTypeRNs.Clear();

          if ((listRND2 != null) && (listRND2.Count > 0))
          {
            listRND4 = new List<LG_RND4>();

            #region Subtract RND1

            nValue = 1;

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              dQty = field.Quantity;

              listRND2Found = listRND2.Where(x => x.c_iteno == field.Item).Select(y => y).ToList();

              if ((listRND2Found != null) && (listRND2Found.Count > 0))
              {
                for (nLoopC = 0; nLoopC < listRND2Found.Count; nLoopC++)
                {
                  rnd2 = listRND2Found[nLoopC];

                  if (rnd2 != null)
                  {
                    dDisc = (rnd2.n_floqty.HasValue ? rnd2.n_floqty.Value : 0);

                    dPrice = (dDisc > dQty ? dQty : dDisc);
                    
                    //dQty -= dPrice;

                    //rnd1.n_floqty -= dPrice;

                    listRND4.Add(new LG_RND4()
                    {
                      i_urut = 1,
                      c_batch = rnd2.c_batch.Trim(),
                      c_fb = fakturID,
                      c_gdg = rnd2.c_gdg,
                      c_iteno = field.Item,
                      c_rnno = rnd2.c_rnno,
                      c_type = "01",
                      d_entry = date,
                      l_status = true,
                      n_disc = field.Discount,
                      n_qty = dPrice,
                      n_salpri = field.Harga,
                    });


                    rnd2.n_floqty -= dPrice;

                    dQty -= dPrice;

                    if (dQty <= 0)
                    {
                      break;
                    }

                    nValue++;
                  }
                  if (dQty <= 0)
                  {
                    break;
                  }
                }

                //if (dQty > 0.00m)
                //{
                //  hasAnyChanges = false;

                //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                //  result = "Quantity faktur tidak tersedia pada data floating.";

                //  break;
                //}
              }
              else
              {
                hasAnyChanges = false;

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                result = "Salah satu nomor receive tidak dapat di temukan.";

                break;
              }

              //for (nLoop = 0; nLoop < listRND1.Count; nLoop++)
              //{
              //  rnd1 = listRND1[nLoop];
              //}
            }

            #endregion
          }
          else
          {
            hasAnyChanges = false;

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            result = "Nomor receive tidak dapat di temukan.";
          }

          if (hasAnyChanges)
          {
            if ((listRND4 != null) && (listRND4.Count > 0))
            {
              db.LG_RND4s.InsertAllOnSubmit(listRND4.ToArray());

              listRND4.Clear();
            }

            dic = new Dictionary<string, string>();

            dic.Add("Faktur", fakturID);
            dic.Add("NoDelivery", RNFLOATING_ID);
            dic.Add("Tanggal", structure.Fields.TanggalFakturDate.ToString("yyyyMMdd"));
            dic.Add("Net", totalNet.ToString());

            result = string.Format("Total {0} detail(s)", totalDetails);
          }

          #endregion

          #endregion

          #endregion
        }

        if (!isContexted)
        {
          if (hasAnyChanges)
          {
            db.SubmitChanges();

            db.Transaction.Commit();
            //db.Transaction.Rollback();

            rpe = ResponseParser.ResponseParserEnum.IsSuccess;
          }
          else
          {
            db.Transaction.Rollback();

            rpe = ResponseParser.ResponseParserEnum.IsFailed;
          }
        }
        else
        {
          if (hasAnyChanges)
          {
            rpe = ResponseParser.ResponseParserEnum.IsSuccess;
          }
          else
          {
            rpe = ResponseParser.ResponseParserEnum.IsFailed;
          }
        }
      }
      catch (Exception ex)
      {
        if (db.Transaction != null)
        {
          db.Transaction.Rollback();
        }

        result = string.Format("ScmsSoaLibrary.Bussiness.Faktur:FakturBeli - {0}", ex.Message);

        Logger.WriteLine(result, true);
        Logger.WriteLine(ex.StackTrace);
      }

    endLogic:
      result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

      if (dic != null)
      {
        dic.Clear();
      }

      if (!isContexted)
      {
        db.Dispose();
      }

      return result;
    }

    public string FakturJualRetur(ScmsSoaLibrary.Parser.Class.FakturJualReturStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      string result = null;

      bool hasAnyChanges = false;

      Config cfg = Functionals.Configuration;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      ScmsSoaLibrary.Parser.Class.FakturJualReturStructureField field = null;
      //ScmsSoaLibrary.Parser.Class.FakturJualReturProcessStructureField prosesField = null;

      string nipEntry = null;
      string fakturID = null,
        doId = null;
      string tmpNum = null;
      //tmpNumbering = null,

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now,
        dateF = DateTime.MinValue;

      HeaderFakturReturProcess hdrFjr = null;
      List<HeaderFakturReturProcess> listHdr = null;
      //List<ScmsSoaLibrary.Parser.Class.FakturJualReturProcessStructureField> listProcessRetur = null;

      decimal totalGross = 0,
        totalDisc = 0,
        totalPajak = 0,
        totalNet = 0,
        totalSisa = 0,
        dQty = 0,
        dDisc = 0,
        dPrice = 0,
        dGross = 0;

      LG_FJRH fjrh = null;
      LG_FJRD1 fjrd1 = null;
      LG_FJRD2 fjrd2 = null;
      LG_FJRD3 fjrd3 = null;

      List<LG_FJRH> listFJRH = null;
      List<LG_FJRD1> listFJRD1 = null;
      List<LG_FJRD2> listFJRD2 = null;
      List<LG_FJRD3> listFJRD3 = null;
      List<LG_FJRD4> listFJRD4 = null;

      List<LG_RCD2> listRCD2 = null;
      List<LG_RCD2> listRCD2Qry = null;

      DetailFakturReturProcess dfrp = null;
      List<DetailFakturReturProcess> listRCDetail = null;

      //LG_RCH rch = null;
      //LG_RCD2 rcd2 = null;

      Random rnd = null;

      int nLoop = 0,
        nLoopC = 0,
        nLoopD = 0,
        //nValue = 0,
        totalDetail = 0;

      ScmsSoaLibrary.Bussiness.Commons.FakturPajakInformation fpi = null;
      List<ScmsSoaLibrary.Bussiness.Commons.FakturPajakInformation> listFPI = null;
      List<ScmsSoaLibrary.Bussiness.Commons.FakturPajakInformation> listFreeFPI = null;

      bool isCabang = false;

      IDictionary<string, string> dic = null;

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }      
      
      fakturID = (structure.Fields.FakturID ?? string.Empty);

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify

          if (string.IsNullOrEmpty(fakturID))
          {
            result = "Nomor faktur jual retur dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          fjrh = (from q in db.LG_FJRHs
                  where q.c_fjno == fakturID
                  select q).Take(1).SingleOrDefault();

          if (fjrh == null)
          {
            result = "Nomor faktur jual retur tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (fjrh.l_delete.HasValue && fjrh.l_delete.Value)
          {
            result = "Tidak dapat mengubah nomor faktur jual retur yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingFA(db, fjrh.d_fjdate.Value) || Commons.IsClosingFA(db, structure.Fields.TanggalFakturDate))
          {
            result = "Faktur tidak dapat diubah, karena tanggal pada transaksi ini sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
          {
            fjrh.v_ket = structure.Fields.Keterangan;
          }

          isCabang = (fjrh.l_cabang.HasValue ? fjrh.l_cabang.Value : false);

          fjrh.d_fjdate = structure.Fields.TanggalFakturDate;

          fjrh.c_taxno = structure.Fields.NoTax;
          fjrh.d_taxdate = structure.Fields.TanggalTaxDate;

          fjrh.c_kurs = structure.Fields.Kurs;
          fjrh.n_kurs = structure.Fields.KursValue;

          fjrh.c_update = nipEntry;
          fjrh.d_update = date;

          #region Populate Detail

          listFJRD1 = (from q in db.LG_FJRD1s
                       where q.c_fjno == fakturID
                       select q).ToList();
          listFJRD2 = (from q in db.LG_FJRD2s
                       where q.c_fjno == fakturID
                       select q).ToList();

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            fjrd3 = (from q in db.LG_FJRD3s
                    where q.c_fjno == fakturID
                    select q).Take(1).SingleOrDefault();

            //doId = (fjrd3 != null ? fjrd3.c_dono : string.Empty);
            doId = (fjrd3 != null ? (string.IsNullOrEmpty(fjrd3.c_dono) ? string.Empty : fjrd3.c_dono.Trim()) : string.Empty);

            hdrFjr = (fjrd3 == null ? null : new HeaderFakturReturProcess()
            {
              NoReference = doId,
              NoRetur = (string.IsNullOrEmpty(fjrd3.c_rcno) ? string.Empty : fjrd3.c_rcno.Trim())
            });

            if(hdrFjr != null)
            {
              listRCD2 = (from q in db.LG_RCD2s
                          where hdrFjr.NoRetur == q.c_rcno 
                            && hdrFjr.NoReference == q.c_dono
                          select q).ToList();
            }

            listFJRD4 = new List<LG_FJRD4>();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              fjrd1 = listFJRD1.Find(delegate(LG_FJRD1 fjrd)
              {
                return field.Item.Equals(fjrd.c_iteno);
              });
              fjrd2 = listFJRD2.Find(delegate(LG_FJRD2 fjrd)
              {
                return field.Item.Equals(fjrd.c_iteno);
              });

              if ((fjrd1 != null) && (fjrd2 != null))
              {
                if (field.IsModified && (!field.IsDelete))
                {
                  #region Modify

                  dQty = (fjrd1.n_qty.HasValue ? fjrd1.n_qty.Value : 0);

                  dPrice = (field.Harga * dQty);

                  dDisc = (dPrice * (field.Discount / 100));

                  listFJRD4.Add(new LG_FJRD4()
                  {
                    c_fjno = fakturID,
                    c_dono = doId,
                    c_entry = nipEntry,
                    c_iteno = field.Item,
                    c_no = fjrd2.c_no,
                    c_type = fjrd2.c_type,
                    d_entry = date,
                    n_disc = fjrd1.n_disc,
                    n_discoff = fjrd2.n_discoff,
                    n_discon = fjrd2.n_discon,
                    n_netoff = fjrd2.n_netoff,
                    n_neton = fjrd2.n_neton,
                    n_qty = fjrd1.n_qty,
                    n_salpri = fjrd1.n_salpri,
                    v_ket_del = field.KeteranganMod,
                    v_type = "02"
                  });

                  fjrd2.n_neton = dDisc;

                  fjrd1.n_salpri = field.Harga;
                  fjrd2.n_discon = field.Discount;

                  #endregion
                }
                else if ((!field.IsModified) && field.IsDelete)
                {
                  #region Delete

                  #region Revert RCD2

                  if ((listRCD2 != null) && (listRCD2.Count > 0))
                  {
                    listRCD2Qry = listRCD2.FindAll(delegate(LG_RCD2 rcd)
                    {
                      return (string.IsNullOrEmpty(rcd.c_iteno) ? false :
                        rcd.c_iteno.Trim().Equals(field.Item, StringComparison.OrdinalIgnoreCase));
                    });

                    if((listRCD2Qry != null) && (listRCD2Qry.Count > 0))
                    {
                      for (nLoopD = 0; nLoopD < listRCD2Qry.Count; nLoopD++)
                      {
                        listRCD2Qry[nLoopD].n_sisa = listRCD2Qry[nLoopD].n_qty;
                      }
                    }
                  }

                  #endregion

                  listFJRD4.Add(new LG_FJRD4()
                  {
                    c_fjno = fakturID,
                    c_dono = doId,
                    c_entry = nipEntry,
                    c_iteno = field.Item,
                    c_no = fjrd2.c_no,
                    c_type = fjrd2.c_type,
                    d_entry = date,
                    n_disc = fjrd1.n_disc,
                    n_discoff = fjrd2.n_discoff,
                    n_discon = fjrd2.n_discon,
                    n_netoff = fjrd2.n_netoff,
                    n_neton = fjrd2.n_neton,
                    n_qty = fjrd1.n_qty,
                    n_salpri = fjrd1.n_salpri,
                    v_ket_del = field.KeteranganMod,
                    v_type = "03"
                  });

                  db.LG_FJRD1s.DeleteOnSubmit(fjrd1);
                  db.LG_FJRD2s.DeleteOnSubmit(fjrd2);

                  listFJRD1.Remove(fjrd1);
                  listFJRD2.Remove(fjrd2);

                  #endregion
                }
              }
            }

            if ((listFJRD4 != null) && (listFJRD4.Count > 0))
            {
              db.LG_FJRD4s.InsertAllOnSubmit(listFJRD4.ToArray());

              listFJRD4.Clear();
            }
          }

          #endregion

          #region Recalculate All

          totalGross =
            totalDisc = 0;

          if ((listFJRD1 != null) && (listFJRD1.Count > 0))
          {
            for (nLoop = 0; nLoop < listFJRD1.Count; nLoop++)
            {
              fjrd1 = listFJRD1[nLoop];

              if (fjrd1 != null)
              {
                fjrd2 = listFJRD2.Find(delegate(LG_FJRD2 fjd)
                {
                  if (string.IsNullOrEmpty(fjd.c_iteno))
                  {
                    return false;
                  }

                  return (string.IsNullOrEmpty(fjrd1.c_iteno) ? false :
                    (fjrd1.c_iteno.Trim().Equals(fjd.c_iteno.Trim()) ? true : false));
                });

                if (fjrd2 != null)
                {
                  dQty = (fjrd1.n_qty.HasValue ? fjrd1.n_qty.Value : 0);
                  dPrice = ((fjrd1.n_salpri.HasValue ? fjrd1.n_salpri.Value : 0) * dQty);

                  dDisc = (dPrice * ((fjrd2.n_discon.HasValue ? fjrd2.n_discon.Value : 0) / 100));

                  totalGross += dPrice;
                  totalDisc += dDisc;
                }
              }
            }
          }

          #endregion

          #region Replace

          decimal ppn = db.fn_GetTax("PPN", structure.Fields.TanggalTaxDate) ?? decimal.Zero;
            totalPajak = (isCabang ? 0 : ((totalGross - totalDisc) * ppn));
          totalNet = ((totalGross - totalDisc) + totalPajak);

          totalSisa = ((fjrh.n_net.HasValue ? fjrh.n_net.Value : 0) - (fjrh.n_sisa.HasValue ? fjrh.n_sisa.Value : 0));

          dPrice = (totalNet - totalSisa);

          fjrh.n_gross = totalGross;
          fjrh.n_disc = totalDisc;
          fjrh.n_tax = totalPajak;
          fjrh.n_net = totalNet;
          fjrh.n_sisa = dPrice;

          dic = new Dictionary<string, string>();

          dic.Add("Faktur", fakturID);
          dic.Add("Sisa", Functionals.DecimalToString(dPrice));
          dic.Add("Net", Functionals.DecimalToString(totalNet));

          #endregion

          hasAnyChanges = true;

          #endregion
        }
        else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete

          if (string.IsNullOrEmpty(fakturID))
          {
            result = "Nomor faktur jual retur dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          fjrh = (from q in db.LG_FJRHs
                 where q.c_fjno == fakturID
                 select q).Take(1).SingleOrDefault();

          if (fjrh == null)
          {
            result = "Nomor faktur jual retur tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (fjrh.l_delete.HasValue && fjrh.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor faktur jual retur yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingFA(db, fjrh.d_fjdate.Value))
          {
            result = "Faktur tidak dapat dihapus, karena tanggal pada transaksi ini sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          fjrh.c_update = nipEntry;
          fjrh.d_update = DateTime.Now;

          fjrh.l_delete = true;
          fjrh.v_ket_mark = structure.Fields.Keterangan;

          db.LG_FJRHs.DeleteOnSubmit(fjrh);

          #region Revert Faktur Pajak

          isCabang = (fjrh.l_cabang.HasValue ? fjrh.l_cabang.Value : false);
          if (!isCabang)
          {
            tmpNum = (string.IsNullOrEmpty(fjrh.c_taxno) ? string.Empty : fjrh.c_taxno.Trim());
            if (!string.IsNullOrEmpty(tmpNum))
            {
              fpi = new Commons.FakturPajakInformation()
              {
                NoFaktur = string.Empty,
                NoFakturPajak = tmpNum,
                TanggalFakturPajak = (fjrh.d_taxdate.HasValue ? fjrh.d_taxdate.Value : Functionals.StandardSqlDateTime),
                IsUsed = false
              };

              Commons.UpdateFakturPajak(db, fpi);
            }
          }

          #endregion

          #region Hapus Detail

          listFJRD1 = (from q in db.LG_FJRD1s
                      where q.c_fjno == fakturID
                      select q).ToList();
          listFJRD2 = (from q in db.LG_FJRD2s
                      where q.c_fjno == fakturID
                      select q).ToList();
          fjrd3 = (from q in db.LG_FJRD3s
                  where q.c_fjno == fakturID
                  select q).Take(1).SingleOrDefault();

          doId = (fjrd3 != null ? fjrd3.c_dono : string.Empty);

          hdrFjr = (fjrd3 == null ? null : new HeaderFakturReturProcess()
          {
            NoReference = (string.IsNullOrEmpty(fjrd3.c_dono) ? string.Empty : fjrd3.c_dono.Trim()),
            NoRetur = (string.IsNullOrEmpty(fjrd3.c_rcno) ? string.Empty : fjrd3.c_rcno.Trim())
          });

          if (hdrFjr != null)
          {
            listRCD2 = (from q in db.LG_RCD2s
                        where hdrFjr.NoRetur == q.c_rcno
                          && hdrFjr.NoReference == q.c_dono
                        select q).ToList();
          }

          listFJRD4 = new List<LG_FJRD4>();

          for (nLoop = 0; nLoop < listFJRD1.Count; nLoop++)
          {
            fjrd1 = listFJRD1[nLoop];

            fjrd2 = listFJRD2.Find(delegate(LG_FJRD2 fjd)
            {
              return (string.IsNullOrEmpty(fjrd1.c_iteno) ? false :
                (fjrd1.c_iteno.Equals(fjd.c_iteno, StringComparison.OrdinalIgnoreCase) ? true : false));
            });
                        
            #region Revert RCD2

            tmpNum = (fjrd1 == null ?
              (fjrd2 == null ? null : (string.IsNullOrEmpty(fjrd2.c_iteno) ? string.Empty : fjrd2.c_iteno.Trim())) :
              (string.IsNullOrEmpty(fjrd1.c_iteno) ? string.Empty : fjrd1.c_iteno.Trim()));

            if ((listRCD2 != null) && (listRCD2.Count > 0))
            {
              listRCD2Qry = listRCD2.FindAll(delegate(LG_RCD2 rcd)
              {
                return (string.IsNullOrEmpty(rcd.c_iteno) ? false :
                  rcd.c_iteno.Trim().Equals(tmpNum, StringComparison.OrdinalIgnoreCase));
              });

              if ((listRCD2Qry != null) && (listRCD2Qry.Count > 0))
              {
                for (nLoopD = 0; nLoopD < listRCD2Qry.Count; nLoopD++)
                {
                  listRCD2Qry[nLoopD].n_sisa = listRCD2Qry[nLoopD].n_qty;
                }
              }
            }

            #endregion

            if ((fjrd1 != null) && (fjrd2 != null))
            {
              listFJRD4.Add(new LG_FJRD4()
              {
                c_fjno = fakturID,
                c_dono = doId,
                c_entry = nipEntry,
                c_iteno = fjrd1.c_iteno,
                c_no = fjrd2.c_no,
                c_type = fjrd2.c_type,
                d_entry = date,
                n_disc = fjrd1.n_disc,
                n_discoff = fjrd2.n_discoff,
                n_discon = fjrd2.n_discon,
                n_netoff = fjrd2.n_netoff,
                n_neton = fjrd2.n_neton,
                n_qty = fjrd1.n_qty,
                n_salpri = fjrd1.n_salpri,
                v_ket_del = structure.Fields.Keterangan,
                v_type = "03"
              });
            }
          }

          if ((listFJRD1 != null) && (listFJRD1.Count > 0))
          {
            db.LG_FJRD1s.DeleteAllOnSubmit(listFJRD1.ToArray());

            listFJRD1.Clear();
          }

          if ((listFJRD2 != null) && (listFJRD2.Count > 0))
          {
            db.LG_FJRD2s.DeleteAllOnSubmit(listFJRD2.ToArray());

            listFJRD2.Clear();
          }

          if (fjrd3 != null)
          {
            db.LG_FJRD3s.DeleteOnSubmit(fjrd3);
          }

          if ((listFJRD4 != null) && (listFJRD4.Count > 0))
          {
            db.LG_FJRD4s.InsertAllOnSubmit(listFJRD4.ToArray());

            listFJRD4.Clear();
          }

          #endregion

          hasAnyChanges = true;

          #endregion
        }
        else if(structure.Method.Equals("Process", StringComparison.OrdinalIgnoreCase))
        {
          #region Proses

          #region Old Coded

          //if (string.IsNullOrEmpty(structure.Fields.Customer))
          //{
          //  result = "Nama customer dibutuhkan.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}

          //isCabang = (from q in db.LG_Cusmas where q.c_cusno == structure.Fields.Customer select q.l_cabang.Value).Single();

          //// where c_portal = '3' and c_type = '18' and s_tahun = year(@RCDATE)

          #endregion

          if ((structure.ExtraFields != null) && (structure.ExtraFields.ProcessField != null) && (structure.ExtraFields.ProcessField.Length > 0))
          {
            listHdr = structure.ExtraFields.ProcessField.GroupBy(x => new { x.Customer, x.ExFaktur, x.ReturID, x.TanggalReturDate, x.DeliveryID, x.IsCabang }).Select(y =>
              new HeaderFakturReturProcess()
              {
                ExFaktur = (string.IsNullOrEmpty(y.Key.ExFaktur) ? string.Empty : y.Key.ExFaktur.Trim()),
                Customer = (string.IsNullOrEmpty(y.Key.Customer) ? string.Empty : y.Key.Customer.Trim()),
                NoReference = (string.IsNullOrEmpty(y.Key.DeliveryID) ? string.Empty : y.Key.DeliveryID.Trim()),
                NoRetur = (string.IsNullOrEmpty(y.Key.ReturID) ? string.Empty : y.Key.ReturID.Trim()),
                TanggalRetur = y.Key.TanggalReturDate,
                IsCabang = y.Key.IsCabang
              }).ToList();

            #region Old Coded

            //listRCD2 = (from q in db.LG_RCD2s
            //            where listHdr.Contains(new HeaderFakturReturProcess()
            //            {
            //              NoReference = q.c_dono,
            //              NoRetur = q.c_rcno
            //            }) 
            //            select q).ToList();

            #endregion

            //listRCDetail = (from q in db.LG_RCHes
            //                join q1 in db.LG_RCD2s on new { q.c_gdg, q.c_rcno } equals new { q1.c_gdg, q1.c_rcno }
            //                join q2 in db.LG_RCD3s on new { q.c_gdg, q.c_rcno, q1.c_iteno } equals new { q2.c_gdg, q2.c_rcno, q2.c_iteno }
            //                where listHdr.Contains(new HeaderFakturReturProcess()
            //                {
            //                  NoRetur = q.c_rcno,
            //                  NoReference = q1.c_dono
            //                })
            //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
            //                && (!(from sq in db.LG_FJRHs
            //                      join sq1 in db.LG_FJRD3s on sq.c_fjno equals sq1.c_fjno
            //                      where ((sq.l_delete.HasValue ? sq.l_delete.Value : false) == false)
            //                      select new HeaderFakturReturProcess { NoRetur = sq1.c_rcno, NoReference = sq1.c_dono }).Contains(new HeaderFakturReturProcess()
            //                      {
            //                        NoRetur = q.c_rcno,
            //                        NoReference = q1.c_dono
            //                      }))
            //                group q1 by new { q.c_rcno, q1.c_dono, q1.c_iteno, q2.n_salpri, q2.n_disc } into g
            //                select new DetailFakturReturProcess()
            //                {
            //                  NoRetur = g.Key.c_rcno,
            //                  NoDO = g.Key.c_dono,
            //                  NoItem = g.Key.c_iteno,
            //                  Salpri = (g.Key.n_salpri.HasValue ? g.Key.n_salpri.Value : 0),
            //                  Quantity = g.Sum(y => (y.n_qty.HasValue ? y.n_qty.Value : 0)),
            //                  Discount = (g.Key.n_disc.HasValue ? g.Key.n_disc.Value : 0),
            //                }).Distinct().ToList();

            rnd = new Random((int)DateTime.Now.Ticks);

            listFJRH = new List<LG_FJRH>();
            listFJRD1 = new List<LG_FJRD1>();
            listFJRD2 = new List<LG_FJRD2>();
            listFJRD3 = new List<LG_FJRD3>();

            listFPI = new List<Commons.FakturPajakInformation>();

            listFreeFPI = ScmsSoaLibrary.Bussiness.Commons.FreeFakturPajak(db);

            #region Populate

            for (nLoop = 0; nLoop < listHdr.Count; nLoop++)
            {
              hdrFjr = listHdr[nLoop];

              if (hdrFjr != null) //&& (!string.IsNullOrEmpty(hdrFjr.NoRetur)) && (!string.IsNullOrEmpty(hdrFjr.NoReference))
              {
                //fakturID = Commons.GenerateNumbering<LG_FJRH>(db, "JR", '3', "18", dateF, "c_fjno");
                fakturID  = hdrFjr.NoRetur.Replace("RC", "JR").Trim();

                fjrh = listFJRH.Find(delegate(LG_FJRH jrh)
                {
                  return jrh.c_fjno.Equals(fakturID, StringComparison.OrdinalIgnoreCase);
                });

                if (fjrh == null)
                {
                  fjrh = new LG_FJRH()
                  {
                    c_cusno = hdrFjr.Customer,
                    c_entry = nipEntry,
                    c_exno = hdrFjr.ExFaktur,
                    c_fjno = fakturID,
                    c_kurs = "01",
                    c_pin = rnd.Next(1, int.MaxValue).ToString(),
                    c_taxno = "",
                    c_update = nipEntry,
                    d_entry = date,
                    d_fjdate = hdrFjr.TanggalRetur,
                    d_taxdate = date,
                    d_update = date,
                    l_cabang = hdrFjr.IsCabang,
                    n_disc = 0,
                    n_gross = 0,
                    n_kurs = 1,
                    n_net = 0,
                    n_sisa = 0,
                    n_tax = 0,
                    v_ket = "Sys: Auto Generate"
                  };

                  listFJRH.Add(fjrh);
                }

                #region Populate Detail

                fjrd3 = listFJRD3.Find(delegate(LG_FJRD3 jrd3)
                {
                  return jrd3.c_fjno.Equals(fakturID, StringComparison.OrdinalIgnoreCase) &&
                    jrd3.c_rcno.Equals(hdrFjr.NoRetur, StringComparison.OrdinalIgnoreCase) &&
                    jrd3.c_dono.Equals(hdrFjr.NoReference, StringComparison.OrdinalIgnoreCase);
                });

                if (fjrd3 == null)
                {
                  listFJRD3.Add(new LG_FJRD3()
                  {
                    c_fjno = fakturID,
                    c_dono = hdrFjr.NoReference,
                    c_rcno = hdrFjr.NoRetur
                  });
                }

                listRCDetail = (from q in db.LG_RCHes
                                join q1 in db.LG_RCD2s on new { q.c_gdg, q.c_rcno } equals new { q1.c_gdg, q1.c_rcno }
                                join q2 in db.LG_RCD3s on new { q.c_gdg, q.c_rcno, q1.c_iteno } equals new { q2.c_gdg, q2.c_rcno, q2.c_iteno }
                                where (q.c_cusno == hdrFjr.Customer) && (q.c_rcno == hdrFjr.NoRetur) && (q1.c_dono == hdrFjr.NoReference)
                                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                                && (!(from sq in db.LG_FJRHs
                                      join sq1 in db.LG_FJRD3s on sq.c_fjno equals sq1.c_fjno
                                      where (sq1.c_rcno == hdrFjr.NoRetur)
                                        && ((sq.l_delete.HasValue ? sq.l_delete.Value : false) == false)
                                      select sq1.c_dono).Contains(hdrFjr.NoReference))
                                group q1 by new { q.c_rcno, q1.c_dono, q1.c_iteno, q2.n_salpri, q2.n_disc } into g
                                select new DetailFakturReturProcess()
                                {
                                  NoRetur = g.Key.c_rcno,
                                  NoDO = g.Key.c_dono,
                                  NoItem = g.Key.c_iteno,
                                  Salpri = (g.Key.n_salpri.HasValue ? g.Key.n_salpri.Value : 0),
                                  Quantity = g.Sum(y => (y.n_qty.HasValue ? y.n_qty.Value : 0)),
                                  Discount = (g.Key.n_disc.HasValue ? g.Key.n_disc.Value : 0),
                                }).Distinct().ToList();

                //listRCD2Qry = listRCD2.Where(x => (x.c_rcno == hdrFjr.NoRetur) && (x.c_dono == hdrFjr.NoReference)).ToList();
                //listRCDetailQry = listRCDetail.Where(x => (x.NoRetur == hdrFjr.NoRetur) && (x.NoDO == hdrFjr.NoReference)).ToList();

                for (nLoopC = 0; nLoopC < listRCDetail.Count; nLoopC++)
                {
                  dfrp = listRCDetail[nLoopC];

                  if (dfrp != null)
                  {
                    #region JRD1

                    fjrd1 = listFJRD1.Find(delegate(LG_FJRD1 jrd1)
                    {
                      return jrd1.c_fjno.Equals(fakturID, StringComparison.OrdinalIgnoreCase) &&
                        jrd1.c_iteno.Equals(dfrp.NoItem, StringComparison.OrdinalIgnoreCase);
                    });

                    if (fjrd1 == null)
                    {
                      fjrd1 = new LG_FJRD1()
                      {
                        c_fjno = fakturID,
                        c_iteno = dfrp.NoItem,
                        n_qty = dfrp.Quantity,
                        n_salpri = dfrp.Salpri,
                        n_disc = 0
                      };

                      listFJRD1.Add(fjrd1);
                    }
                    else if (fjrd1.n_salpri.Value == dfrp.Salpri)
                    {
                      fjrd1.n_qty += dfrp.Quantity;
                    }
                    else
                    {
                      continue;
                    }

                    #endregion

                    #region JRD2

                    fjrd2 = listFJRD2.Find(delegate(LG_FJRD2 jrd2)
                    {
                      return jrd2.c_fjno.Equals(fakturID, StringComparison.OrdinalIgnoreCase) &&
                        jrd2.c_iteno.Equals(dfrp.NoItem, StringComparison.OrdinalIgnoreCase);
                    });

                    if (fjrd2 == null)
                    {
                      fjrd2 = new LG_FJRD2()
                      {
                        c_fjno = fakturID,
                        c_iteno = dfrp.NoItem,
                        c_type = "03",
                        n_discon = dfrp.Discount,
                        n_discoff = 0,
                        n_neton = 0,
                        n_netoff = 0,
                        c_no = string.Empty,
                      };

                      listFJRD2.Add(fjrd2);
                    }
                    else if (fjrd2.n_discon.Value != dfrp.Discount)
                    {
                      fjrd1.n_qty -= dfrp.Quantity;
                      if (fjrd1.n_qty.Value <= 0)
                      {
                        listFJRD1.Remove(fjrd1);
                      }
                    }
                    else
                    {
                      dGross = (dfrp.Quantity * dfrp.Salpri);
                      dDisc = (dGross * (dfrp.Discount / 100));
                      
                      totalGross += dGross;
                      totalDisc += dDisc;
                    }

                    #endregion

                    listRCD2 = (from q in db.LG_RCD2s
                                where (q.c_rcno == hdrFjr.NoRetur) && (q.c_dono == hdrFjr.NoReference)
                                select q).Distinct().ToList();

                    for (nLoopD = 0; nLoopD < listRCD2.Count; nLoopD++)
                    {
                      listRCD2[nLoopD].n_sisa = 0;
                    }

                    listRCD2.Clear();
                  }
                }

                listRCDetail.Clear();

                #endregion

                #region Faktur Pajak

                if (hdrFjr.IsCabang)
                {
                  fpi = null;

                  totalPajak = 0;
                }
                else
                {
                  if (listFreeFPI.Count > 0)
                  {
                    fpi = listFreeFPI[nLoop];

                    listFreeFPI.Remove(fpi);
                  }
                  else
                  {
                    fpi = ScmsSoaLibrary.Bussiness.Commons.GetFakturPajakDisCore(cfg, fakturID);
                  }
                  if ((fpi != null) && (!listFPI.Contains(fpi)))
                  {
                    fpi.NoFaktur = fakturID;

                    listFPI.Add(fpi);
                  }

                    decimal ppn = db.fn_GetTax("PPN", structure.Fields.TanggalTaxDate) ?? decimal.Zero;
                    totalPajak = ((totalGross - totalDisc) * ppn);

                  fjrh.c_taxno = fpi.NoFakturPajak;
                  fjrh.d_taxdate = fpi.TanggalFakturPajak;
                }

                #endregion

                fjrh.n_gross += totalGross;
                fjrh.n_disc += totalDisc;
                fjrh.n_tax += totalPajak;
                fjrh.n_net =
                  fjrh.n_sisa += ((totalGross - totalDisc) + totalPajak);

                totalDetail++;
              }
            }

            #endregion

            #region Old Coded

            //#region Populate

            //for (nLoop = 0; nLoop < listHdr.Count; nLoop++)
            //{
            //  hdrFjr = listHdr[nLoop];

            //  if (hdrFjr != null)
            //  {
            //    listProcessRetur = structure.ExtraFields.ProcessField.Where(x => x.DeliveryID == hdrFjr.NoReference && x.ReturID == hdrFjr.NoRetur).ToList();

            //    if (listProcessRetur.Count > 0)
            //    {
            //      prosesField = listProcessRetur[0];
            //      dateF = prosesField.TanggalReturDate;

            //      if (dateF.Equals(DateTime.MinValue))
            //      {
            //        continue;
            //      }
                  
            //      fakturID = Commons.GenerateNumbering<LG_FJRH>(db, "JR", '3', "18", dateF, "c_fjno");

            //      #region Old Coded

            //      //sysNum = listSysNo.Find(delegate(SysNo sn)
            //      //{
            //      //  return dateF.Year.Equals((int)sn.s_tahun);
            //      //});

            //      //if (sysNum == null)
            //      //{
            //      //  continue;
            //      //}

            //      //switch (dateF.Month)
            //      //{
            //      //  case 1: tmpNum = (sysNum.c_bln01 ?? string.Empty); break;
            //      //  case 2: tmpNum = (sysNum.c_bln02 ?? string.Empty); break;
            //      //  case 3: tmpNum = (sysNum.c_bln03 ?? string.Empty); break;
            //      //  case 4: tmpNum = (sysNum.c_bln04 ?? string.Empty); break;
            //      //  case 5: tmpNum = (sysNum.c_bln05 ?? string.Empty); break;
            //      //  case 6: tmpNum = (sysNum.c_bln06 ?? string.Empty); break;
            //      //  case 7: tmpNum = (sysNum.c_bln07 ?? string.Empty); break;
            //      //  case 8: tmpNum = (sysNum.c_bln08 ?? string.Empty); break;
            //      //  case 9: tmpNum = (sysNum.c_bln09 ?? string.Empty); break;
            //      //  case 10: tmpNum = (sysNum.c_bln10 ?? string.Empty); break;
            //      //  case 11: tmpNum = (sysNum.c_bln11 ?? string.Empty); break;
            //      //  case 12: tmpNum = (sysNum.c_bln12 ?? string.Empty); break;
            //      //  default: tmpNum = null; break;
            //      //}

            //      //if (string.IsNullOrEmpty(tmpNum))
            //      //{
            //      //  continue;
            //      //}

            //      //if(tmpNum.Length > 1)
            //      //{
            //      //  if(char.IsNumber(tmpNum, 0))
            //      //  {
            //      //    if(!int.TryParse(tmpNum, out nValue))
            //      //    {
            //      //      continue;
            //      //    }

            //      //    nValue++;

            //      //    tmpNum = nValue.ToString().PadLeft(4, '0');
            //      //  }
            //      //  else
            //      //  {
            //      //    if(!int.TryParse(tmpNum.Substring(1), out nValue))
            //      //    {
            //      //      continue;
            //      //    }
                      
            //      //    nValue++;
            //      //    tmpNum = string.Concat(tmpNum[0],  nValue.ToString().PadLeft(3, '0'));
            //      //  }
            //      //}
            //      //else
            //      //{
            //      //  tmpNum = "0001"; 
            //      //}

            //      //switch (dateF.Month)
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

            //      ////fakturID = "XXXXXXXXXX";

            //      ////tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "JR");

            //      //fakturID = string.Concat("JR", dateF.ToString("yyMM"), tmpNum);

            //      #endregion

            //      fjrh = new LG_FJRH()
            //      {
            //        c_cusno = structure.Fields.Customer,
            //        c_entry = nipEntry,
            //        c_exno = prosesField.ExFaktur,
            //        c_fjno = fakturID,
            //        c_kurs = "01",
            //        c_pin = rnd.Next(1, int.MaxValue).ToString(),
            //        c_taxno = "",
            //        c_update = nipEntry,
            //        d_entry = date,
            //        d_fjdate = prosesField.TanggalReturDate,
            //        d_taxdate = date,
            //        d_update = date,
            //        l_cabang = prosesField.IsCabang,
            //        n_disc = 0,
            //        n_gross = 0,
            //        n_kurs = 1,
            //        n_net = 0,
            //        n_sisa = 0,
            //        n_tax = 0,
            //        v_ket = structure.Fields.Keterangan
            //      };

            //      listFJRH.Add(fjrh);

            //      //db.LG_FJRHs.InsertOnSubmit(fjrh);

            //      //db.SubmitChanges();

            //      //fjrh = (from q in db.LG_FJRHs
            //      //        //where q.v_ket == tmpNumbering
            //      //        where q.c_fjno == "XXXXXXXXXX"
            //      //        select q).Take(1).SingleOrDefault();

            //      //if ((fjrh == null) || (fjrh.c_fjno.Equals("XXXXXXXXXX")))
            //      //{
            //      //  if (fjrh != null)
            //      //  {
            //      //    db.LG_FJRHs.DeleteOnSubmit(fjrh);
            //      //  }

            //      //  continue;
            //      //}

            //      #region Populate Detail

            //      listFJRD3.Add(new LG_FJRD3()
            //      {
            //        c_fjno = fakturID,
            //        c_dono = prosesField.DeliveryID,
            //        c_rcno = prosesField.ReturID
            //      });

            //      for (nLoopC = 0; nLoopC < listProcessRetur.Count; nLoopC++)
            //      {
            //        prosesField = listProcessRetur[nLoopC];

            //        listRCD2Qry = listRCD2.FindAll(delegate(LG_RCD2 rcd)
            //        {
            //          return
            //            (string.IsNullOrEmpty(rcd.c_rcno) ? false : rcd.c_rcno.Trim().Equals(prosesField.ReturID, StringComparison.OrdinalIgnoreCase)) &&
            //            (string.IsNullOrEmpty(rcd.c_dono) ? false : rcd.c_dono.Trim().Equals(prosesField.DeliveryID, StringComparison.OrdinalIgnoreCase)) &&
            //            (string.IsNullOrEmpty(rcd.c_iteno) ? false : rcd.c_iteno.Trim().Equals(prosesField.Item, StringComparison.OrdinalIgnoreCase));
            //        });

            //        if ((listRCD2Qry != null)&& (listRCD2Qry.Count > 0))
            //        {
            //          for (nLoopD = 0; nLoopD < listRCD2Qry.Count; nLoopD++)
            //          {
            //            listRCD2Qry[nLoopD].n_sisa = 0;
            //          }

            //          listFJRD1.Add(new LG_FJRD1()
            //          {
            //            c_fjno = fakturID,
            //            c_iteno = prosesField.Item,
            //            n_disc = prosesField.Discount,
            //            n_qty = prosesField.Quantity,
            //            n_salpri = prosesField.Harga
            //          });

            //          dGross = (prosesField.Quantity * prosesField.Harga);
            //          dDisc = (dGross * (prosesField.Discount / 100));

            //          listFJRD2.Add(new LG_FJRD2()
            //          {
            //            c_fjno = fakturID,
            //            c_iteno = prosesField.Item,
            //            c_no = string.Empty,
            //            c_type = prosesField.TypeItem,
            //            n_discoff = 0,
            //            n_discon = prosesField.Discount,
            //            n_netoff = 0,
            //            n_neton = dDisc
            //          });

            //          totalGross += dGross;
            //          totalDisc += dDisc;
            //        }
            //      }

            //      listProcessRetur.Clear();

            //      #endregion

            //      totalPajak = (isCabang ? 0 : ((totalGross - totalDisc) * 0.1m));

            //      fjrh.n_gross = totalGross;
            //      fjrh.n_disc = totalDisc;
            //      fjrh.n_tax = totalPajak;
            //      fjrh.n_net =
            //        fjrh.n_sisa = ((totalGross - totalDisc) + totalPajak);
            //    }

            //    totalDetail++;
            //  }
            //}

            //#endregion

            #endregion

            #region Save Data

            if (listFPI.Count > 0)
            {
              ScmsSoaLibrary.Bussiness.Commons.UpdateFakturPajak(db, true, listFPI.ToArray());

              listFPI.Clear();
            }

            if ((listFJRH != null) && (listFJRH.Count > 0))
            {
              db.LG_FJRHs.InsertAllOnSubmit(listFJRH.ToArray());

              listFJRH.Clear();
            }

            if ((listFJRD1 != null) && (listFJRD1.Count > 0))
            {
              db.LG_FJRD1s.InsertAllOnSubmit(listFJRD1.ToArray());

              listFJRD1.Clear();
            }

            if ((listFJRD2 != null) && (listFJRD2.Count > 0))
            {
              db.LG_FJRD2s.InsertAllOnSubmit(listFJRD2.ToArray());

              listFJRD2.Clear();
            }

            if ((listFJRD3 != null) && (listFJRD3.Count > 0))
            {
              db.LG_FJRD3s.InsertAllOnSubmit(listFJRD3.ToArray());

              listFJRD3.Clear();
            }

            #endregion

            if (totalDetail > 0)
            {
              hasAnyChanges = true;
            }

            listRCD2.Clear();

            listHdr.Clear();
          }

          #endregion
        }

        if (hasAnyChanges)
        {
          db.SubmitChanges();

          //db.Transaction.Commit();
          db.Transaction.Rollback();

          rpe = ResponseParser.ResponseParserEnum.IsSuccess;
        }
        else
        {
          db.Transaction.Rollback();

          rpe = ResponseParser.ResponseParserEnum.IsFailed;
        }
      }
      catch (Exception ex)
      {
        if (db.Transaction != null)
        {
          db.Transaction.Rollback();
        }

        result = string.Format("ScmsSoaLibrary.Bussiness.Faktur:FakturJualRetur - {0}", ex.Message);

        Logger.WriteLine(result, true);
        Logger.WriteLine(ex.StackTrace);
      }

      if (listFreeFPI != null)
      {
        listFreeFPI.Clear();
      }

      if (listFPI.Count > 0)
      {
        db.Transaction = db.Connection.BeginTransaction();

        try
        {
          ScmsSoaLibrary.Bussiness.Commons.UpdateFakturPajak(db, true, listFPI.ToArray());

          db.SubmitChanges();

          db.Transaction.Commit();
        }
        catch (Exception ex)
        {
          result = string.Format("ScmsSoaLibrary.Bussiness.Faktur:FakturJualRetur UpdateFakturPajak_markAllFree - {0}", ex.Message);

          Logger.WriteLine(result, true);
          Logger.WriteLine(ex.StackTrace);

          if (db.Transaction != null)
          {
            db.Transaction.Rollback();
          }
        }

        listFPI.Clear();
      }

    endLogic:
      result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

      if (dic != null)
      {
        dic.Clear();
      }

      db.Dispose();

      return result;
    }

    public string FakturBeliRetur(ScmsSoaLibrary.Parser.Class.FakturBeliReturStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      string result = null;

      bool hasAnyChanges = false;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      ScmsSoaLibrary.Parser.Class.FakturBeliReturStructureField field = null;
      ScmsSoaLibrary.Parser.Class.FakturBeliReturProcessStructureField prosesField = null;

      string nipEntry = null;
      string fakturID = null;
      //string tmpNum = null,
      //  latestNum = null;
      ////tmpNumbering = null

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now,
        dateF = DateTime.MinValue;

      HeaderFakturReturProcess hdrFbr = null;
      List<HeaderFakturReturProcess> listHdr = null;
      List<ScmsSoaLibrary.Parser.Class.FakturBeliReturProcessStructureField> listProcessRetur = null;

      decimal totalGross = 0,
        totalDisc = 0,
        totalNet = 0,
        totalSisa = 0,
        dQty = 0,
        dDisc = 0,
        dPrice = 0;

      LG_FBRH fbrh = null;
      LG_FBRD1 fbrd1 = null;
      LG_FBRD2 fbrd2 = null;
      //LG_FBRD3 fbrd3 = null;

      List<LG_FBRH> listFBRH = null;
      List<LG_FBRD1> listFBRD1 = null;
      List<LG_FBRD2> listFBRD2 = null;
      //List<LG_FBRD3> listFBRD3 = null;
      List<LG_FBRD4> listFBRD4 = null;

      //LG_RSD2 rsd2;
      List<LG_RSD2> listRSD2 = null;
      List<LG_RSD2> listRSD2Qry = null;
      List<LG_RSD3> listRSD3 = null;

      //List<SysNo> listSysNo = null;

      //SysNo sysNum = null;

      Random rnd = null;

      int nLoop = 0,
        nLoopC = 0,
        nLoopD = 0,
        //nValue = 0,
        totalDetail = 0;

      IDictionary<string, string> dic = null;

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }

      fakturID = (structure.Fields.FakturID ?? string.Empty);

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          #region Modify

          if (string.IsNullOrEmpty(fakturID))
          {
            result = "Nomor faktur beli retur dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          fbrh = (from q in db.LG_FBRHs
                  where q.c_fbno == fakturID
                  select q).Take(1).SingleOrDefault();

          if (fbrh == null)
          {
            result = "Nomor faktur beli retur tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (fbrh.l_delete.HasValue && fbrh.l_delete.Value)
          {
            result = "Tidak dapat mengubah nomor faktur beli retur yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingFA(db, fbrh.d_fbdate.Value) || Commons.IsClosingFA(db, structure.Fields.TanggalFakturDate))
          {
            result = "Faktur tidak dapat diubah, karena tanggal pada transaksi ini sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          if (!string.IsNullOrEmpty(structure.Fields.Keterangan))
          {
            fbrh.v_ket = structure.Fields.Keterangan;
          }

          fbrh.d_fbdate = structure.Fields.TanggalFakturDate;

          //fjrh.c_taxno = structure.Fields.NoTax;
          //fjrh.d_taxdate = structure.Fields.TanggalTaxDate;

          //fjrh.c_kurs = structure.Fields.Kurs;
          fbrh.n_kurs = structure.Fields.KursValue;

          fbrh.c_update = nipEntry;
          fbrh.d_update = date;

          #region Populate Detail

          listFBRD1 = (from q in db.LG_FBRD1s
                       where q.c_fbno == fakturID
                       select q).ToList();

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listFBRD2 = (from q in db.LG_FBRD2s
                         where q.c_fbno == fakturID
                         select q).ToList();

            listHdr = new List<HeaderFakturReturProcess>();

            if ((listFBRD2 != null) && (listFBRD2.Count > 0))
            {
              for (nLoop = 0; nLoop < listFBRD2.Count; nLoop++)
              {
                fbrd2 = listFBRD2[nLoop];

                if (fbrd2 != null)
                {
                  listHdr.Add(new HeaderFakturReturProcess()
                  {
                    NoReference = (fbrd2 != null ? (string.IsNullOrEmpty(fbrd2.c_rnno) ? string.Empty : fbrd2.c_rnno.Trim()) : string.Empty),
                    NoRetur = (fbrd2 != null ? (string.IsNullOrEmpty(fbrd2.c_rsno) ? string.Empty : fbrd2.c_rsno.Trim()) : string.Empty)
                  });
                }
              }
            }

            if ((listHdr != null) && (listHdr.Count > 0))
            {
              listRSD2 = (from q in db.LG_RSD2s
                          where listHdr.Contains(new HeaderFakturReturProcess()
                          {
                            NoReference = q.c_rnno,
                            NoRetur = q.c_rsno
                          })
                          select q).ToList();
            }

            listFBRD4 = new List<LG_FBRD4>();

            for (nLoop = 0; nLoop < structure.Fields.Field.Length; nLoop++)
            {
              field = structure.Fields.Field[nLoop];

              fbrd1 = listFBRD1.Find(delegate(LG_FBRD1 fbrd)
              {
                return field.Item.Equals(fbrd.c_iteno);
              });

              if (fbrd1 != null)
              {
                if ((!field.IsModified) && field.IsDelete)
                {
                  #region Delete

                  #region Revert RSD2

                  if ((listRSD2 != null) && (listRSD2.Count > 0))
                  {
                    listRSD2Qry = listRSD2.FindAll(delegate(LG_RSD2 rsd)
                    {
                      return (string.IsNullOrEmpty(rsd.c_iteno) ? false :
                        rsd.c_iteno.Trim().Equals(field.Item, StringComparison.OrdinalIgnoreCase));
                    });

                    if ((listRSD2Qry != null) && (listRSD2Qry.Count > 0))
                    {
                      for (nLoopD = 0; nLoopD < listRSD2Qry.Count; nLoopD++)
                      {
                        listRSD2Qry[nLoopD].n_bsisa = listRSD2Qry[nLoopD].n_bqty;
                        listRSD2Qry[nLoopD].n_gsisa = listRSD2Qry[nLoopD].n_gqty;
                        listRSD2Qry[nLoopD].l_status = false;

                        listFBRD4.Add(new LG_FBRD4()
                        {
                          c_fbno = fakturID,
                          c_rnno = listRSD2Qry[nLoopD].c_rnno,
                          c_entry = nipEntry,
                          c_iteno = field.Item,
                          c_rsno = listRSD2Qry[nLoopD].c_rsno,
                          c_claimaccno = null,
                          d_entry = date,
                          n_disc = fbrd1.n_disc,
                          n_gqty = listRSD2Qry[nLoopD].n_gqty,
                          n_bqty = listRSD2Qry[nLoopD].n_bqty,
                          n_salpri = fbrd1.n_salpri,
                          n_bea = fbrd1.n_bea,
                          v_ket_del = field.KeteranganMod,
                          v_type = "03"
                        });
                      }

                      listRSD2Qry.Clear();
                    }
                    else
                    {
                      for (nLoopD = 0; nLoopD < listFBRD2.Count; nLoopD++)
                      {
                        fbrd2 = listFBRD2[nLoopD];

                        listFBRD4.Add(new LG_FBRD4()
                        {
                          c_fbno = fakturID,
                          c_rnno = fbrd2.c_rnno,
                          c_entry = nipEntry,
                          c_iteno = field.Item,
                          c_rsno = fbrd2.c_rsno,
                          c_claimaccno = null,
                          d_entry = date,
                          n_disc = fbrd1.n_disc,
                          n_gqty = fbrd1.n_gqty,
                          n_bqty = fbrd1.n_bqty,
                          n_salpri = fbrd1.n_salpri,
                          n_bea = fbrd1.n_bea,
                          v_ket_del = field.KeteranganMod,
                          v_type = "03"
                        });
                      }
                    }
                  }
                  else
                  {
                    for (nLoopD = 0; nLoopD < listFBRD2.Count; nLoopD++)
                    {
                      fbrd2 = listFBRD2[nLoopD];

                      listFBRD4.Add(new LG_FBRD4()
                      {
                        c_fbno = fakturID,
                        c_rnno = fbrd2.c_rnno,
                        c_entry = nipEntry,
                        c_iteno = field.Item,
                        c_rsno = fbrd2.c_rsno,
                        c_claimaccno = null,
                        d_entry = date,
                        n_disc = fbrd1.n_disc,
                        n_gqty = fbrd1.n_gqty,
                        n_bqty = fbrd1.n_bqty,
                        n_salpri = fbrd1.n_salpri,
                        n_bea = fbrd1.n_bea,
                        v_ket_del = field.KeteranganMod,
                        v_type = "03"
                      });
                    }
                  }

                  #endregion                  

                  db.LG_FBRD1s.DeleteOnSubmit(fbrd1);

                  listFBRD1.Remove(fbrd1);

                  #endregion
                }
              }
            }

            if ((listFBRD4 != null) && (listFBRD4.Count > 0))
            {
              db.LG_FBRD4s.InsertAllOnSubmit(listFBRD4.ToArray());

              listFBRD4.Clear();
            }
          }

          #endregion

          #region Recalculate All

          totalGross =
            totalDisc = 0;

          if ((listFBRD1 != null) && (listFBRD1.Count > 0))
          {
            for (nLoop = 0; nLoop < listFBRD1.Count; nLoop++)
            {
              fbrd1 = listFBRD1[nLoop];

              if (fbrd1 != null)
              {
                dQty = (fbrd1.n_bqty.HasValue ? fbrd1.n_bqty.Value : 0);
                dQty += (fbrd1.n_gqty.HasValue ? fbrd1.n_gqty.Value : 0);

                dPrice = ((fbrd1.n_salpri.HasValue ? fbrd1.n_salpri.Value : 0) * dQty);

                dDisc = (dPrice * ((fbrd1.n_disc.HasValue ? fbrd1.n_disc.Value : 0) / 100));

                totalGross += dPrice;
                totalDisc += dDisc;
              }
            }
          }

          #endregion

          #region Replace

          totalNet = (totalGross - totalDisc);

          totalSisa = ((fbrh.n_bilva.HasValue ? fbrh.n_bilva.Value : 0) - (fbrh.n_sisa.HasValue ? fbrh.n_sisa.Value : 0));

          dPrice = (totalNet - totalSisa);

          fbrh.n_bruto = totalGross;
          fbrh.n_disc = totalDisc;
          fbrh.n_bilva = totalNet;
          fbrh.n_sisa = dPrice;

          dic = new Dictionary<string, string>();

          dic.Add("Faktur", fakturID);
          dic.Add("Sisa", Functionals.DecimalToString(dPrice));
          dic.Add("Net", Functionals.DecimalToString(totalNet));

          #endregion

          hasAnyChanges = true;

          #endregion
        }
        else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete

          if (string.IsNullOrEmpty(fakturID))
          {
            result = "Nomor faktur beli retur dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          fbrh = (from q in db.LG_FBRHs
                  where q.c_fbno == fakturID
                  select q).Take(1).SingleOrDefault();

          if (fbrh == null)
          {
            result = "Nomor faktur beli retur tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (fbrh.l_delete.HasValue && fbrh.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor faktur beli retur yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingFA(db, fbrh.d_fbdate.Value))
          {
            result = "Faktur tidak dapat dihapus, karena tanggal pada transaksi ini sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          fbrh.c_update = nipEntry;
          fbrh.d_update = DateTime.Now;

          fbrh.l_delete = true;
          fbrh.v_ket_mark = structure.Fields.Keterangan;

          #region Hapus Detail

          listFBRD1 = (from q in db.LG_FBRD1s
                       where q.c_fbno == fakturID
                       select q).ToList();
          listFBRD2 = (from q in db.LG_FBRD2s
                       where q.c_fbno == fakturID
                       select q).ToList();

          listHdr = new List<HeaderFakturReturProcess>();

          if ((listFBRD2 != null) && (listFBRD2.Count > 0))
          {
            for (nLoop = 0; nLoop < listFBRD2.Count; nLoop++)
            {
              fbrd2 = listFBRD2[nLoop];

              if (fbrd2 != null)
              {
                listHdr.Add(new HeaderFakturReturProcess()
                {
                  NoReference = (fbrd2 != null ? (string.IsNullOrEmpty(fbrd2.c_rnno) ? string.Empty : fbrd2.c_rnno.Trim()) : string.Empty),
                  NoRetur = (fbrd2 != null ? (string.IsNullOrEmpty(fbrd2.c_rsno) ? string.Empty : fbrd2.c_rsno.Trim()) : string.Empty)
                });
              }
            }
          }

          if ((listHdr != null) && (listHdr.Count > 0))
          {
            listRSD2 = (from q in db.LG_RSD2s
                        where listHdr.Contains(new HeaderFakturReturProcess()
                        {
                          NoReference = q.c_rnno,
                          NoRetur = q.c_rsno
                        })
                        select q).ToList();
          }

          listFBRD4 = new List<LG_FBRD4>();

          for (nLoop = 0; nLoop < listFBRD1.Count; nLoop++)
          {
            fbrd1 = listFBRD1[nLoop];

            #region Revert RSD2

            if ((listRSD2 != null) && (listRSD2.Count > 0))
            {
              listRSD2Qry = listRSD2.FindAll(delegate(LG_RSD2 rsd)
              {
                return (string.IsNullOrEmpty(rsd.c_iteno) ? false :
                  rsd.c_iteno.Trim().Equals(field.Item, StringComparison.OrdinalIgnoreCase));
              });

              if ((listRSD2Qry != null) && (listRSD2Qry.Count > 0))
              {
                for (nLoopD = 0; nLoopD < listRSD2Qry.Count; nLoopD++)
                {
                  listRSD2Qry[nLoopD].n_bsisa = listRSD2Qry[nLoopD].n_bqty;
                  listRSD2Qry[nLoopD].n_gsisa = listRSD2Qry[nLoopD].n_gqty;
                  listRSD2Qry[nLoopD].l_status = false;

                  listFBRD4.Add(new LG_FBRD4()
                  {
                    c_fbno = fakturID,
                    c_rnno = listRSD2Qry[nLoopD].c_rnno,
                    c_entry = nipEntry,
                    c_iteno = fbrd1.c_iteno,
                    c_rsno = listRSD2Qry[nLoopD].c_rsno,
                    c_claimaccno = null,
                    d_entry = date,
                    n_disc = fbrd1.n_disc,
                    n_gqty = listRSD2Qry[nLoopD].n_gqty,
                    n_bqty = listRSD2Qry[nLoopD].n_bqty,
                    n_salpri = fbrd1.n_salpri,
                    n_bea = fbrd1.n_bea,
                    v_ket_del = structure.Fields.Keterangan,
                    v_type = "03"
                  });
                }

                listRSD2Qry.Clear();
              }
              else
              {
                for (nLoopD = 0; nLoopD < listFBRD2.Count; nLoopD++)
                {
                  fbrd2 = listFBRD2[nLoopD];

                  listFBRD4.Add(new LG_FBRD4()
                  {
                    c_fbno = fakturID,
                    c_rnno = fbrd2.c_rnno,
                    c_entry = nipEntry,
                    c_iteno = fbrd1.c_iteno,
                    c_rsno = fbrd2.c_rsno,
                    c_claimaccno = null,
                    d_entry = date,
                    n_disc = fbrd1.n_disc,
                    n_gqty = fbrd1.n_gqty,
                    n_bqty = fbrd1.n_bqty,
                    n_salpri = fbrd1.n_salpri,
                    n_bea = fbrd1.n_bea,
                    v_ket_del = structure.Fields.Keterangan,
                    v_type = "03"
                  });
                }
              }
            }
            else
            {
              for (nLoopD = 0; nLoopD < listFBRD2.Count; nLoopD++)
              {
                fbrd2 = listFBRD2[nLoopD];

                listFBRD4.Add(new LG_FBRD4()
                {
                  c_fbno = fakturID,
                  c_rnno = fbrd2.c_rnno,
                  c_entry = nipEntry,
                  c_iteno = fbrd1.c_iteno,
                  c_rsno = fbrd2.c_rsno,
                  c_claimaccno = null,
                  d_entry = date,
                  n_disc = fbrd1.n_disc,
                  n_gqty = fbrd1.n_gqty,
                  n_bqty = fbrd1.n_bqty,
                  n_salpri = fbrd1.n_salpri,
                  n_bea = fbrd1.n_bea,
                  v_ket_del = structure.Fields.Keterangan,
                  v_type = "03"
                });
              }
            }

            #endregion
          }

          if ((listFBRD1 != null) && (listFBRD1.Count > 0))
          {
            db.LG_FBRD1s.DeleteAllOnSubmit(listFBRD1.ToArray());

            listFBRD1.Clear();
          }

          if ((listFBRD2 != null) && (listFBRD2.Count > 0))
          {
            db.LG_FBRD2s.DeleteAllOnSubmit(listFBRD2.ToArray());

            listFBRD2.Clear();
          }

          if ((listFBRD4 != null) && (listFBRD4.Count > 0))
          {
            db.LG_FBRD4s.InsertAllOnSubmit(listFBRD4.ToArray());

            listFBRD4.Clear();
          }

          #endregion

          hasAnyChanges = true;

          #endregion
        }
        else if (structure.Method.Equals("Process", StringComparison.OrdinalIgnoreCase))
        {
          #region Proses

          // where c_portal = '3' and c_type = '05' and s_tahun = year(getdate())

          if ((structure.ExtraFields != null) && (structure.ExtraFields.ProcessField != null) && (structure.ExtraFields.ProcessField.Length > 0))
          {
            listHdr = structure.ExtraFields.ProcessField.GroupBy(x => x.ReturID).Select(y =>
              new HeaderFakturReturProcess()
              {
                NoRetur = y.Key
              }).ToList();

            listRSD2 = (from q in db.LG_RSD2s
                        where listHdr.Contains(new HeaderFakturReturProcess()
                        {
                          NoRetur = q.c_rsno
                        })
                        select q).ToList();

            rnd = new Random((int)DateTime.Now.Ticks);

            listFBRH = new List<LG_FBRH>();
            listFBRD1 = new List<LG_FBRD1>();
            listFBRD2 = new List<LG_FBRD2>();

            #region Populate

            #region to RSD3

            listRSD3 = new List<LG_RSD3>();

            for (nLoop = 0; nLoop < listHdr.Count; nLoop++)
            {
              hdrFbr = listHdr[nLoop];

              if (hdrFbr != null)
              {
                listProcessRetur = structure.ExtraFields.ProcessField.Where(x => x.ReturID == hdrFbr.NoRetur).ToList();
                //listRSD2Qry = listRSD2.Where(x => x.c_rsno == hdrFbr.NoRetur).ToList();

                for (nLoopC = 0; nLoopC < listProcessRetur.Count; nLoopC++)
                {
                  prosesField = listProcessRetur[nLoopC];

                  //rsd2 = listRSD2
                }

                listProcessRetur.Clear();
                listRSD2Qry.Clear();
              }
            }

            #endregion

            #region Old Code

            //for (nLoop = 0; nLoop < listHdr.Count; nLoop++)
            //{
            //  hdrFbr = listHdr[nLoop];

            //  if (hdrFbr != null)
            //  {
                //listProcessRetur = structure.ExtraFields.ProcessField.Where(x => x.DeliveryID == hdrFjr.NoDelivery && x.ReturID == hdrFjr.NoRetur).ToList();

                //    if (listProcessRetur.Count > 0)
                //    {
                //      prosesField = listProcessRetur[0];
                //      dateF = prosesField.TanggalReturDate;

                //      if (dateF.Equals(DateTime.MinValue))
                //      {
                //        continue;
                //      }

                //      fakturID = Commons.GenerateNumbering<LG_FJRH>(db, "JR", '3', "18", dateF, "c_fjno");

                //      fbrh = new LG_FJRH()
                //      {
                //        c_cusno = structure.Fields.Customer,
                //        c_entry = nipEntry,
                //        c_exno = prosesField.ExFaktur,
                //        c_fjno = fakturID,
                //        c_kurs = "01",
                //        c_pin = rnd.Next(1, int.MaxValue).ToString(),
                //        c_taxno = "",
                //        c_update = nipEntry,
                //        d_entry = date,
                //        d_fjdate = prosesField.TanggalReturDate,
                //        d_taxdate = date,
                //        d_update = date,
                //        l_cabang = prosesField.IsCabang,
                //        n_disc = 0,
                //        n_gross = 0,
                //        n_kurs = 1,
                //        n_net = 0,
                //        n_sisa = 0,
                //        n_tax = 0,
                //        v_ket = tmpNumbering
                //      };

                //      listFBRH.Add(fbrh);

                //      //db.LG_FJRHs.InsertOnSubmit(fjrh);

                //      //db.SubmitChanges();

                //      //fjrh = (from q in db.LG_FJRHs
                //      //        //where q.v_ket == tmpNumbering
                //      //        where q.c_fjno == "XXXXXXXXXX"
                //      //        select q).Take(1).SingleOrDefault();

                //      //if ((fjrh == null) || (fjrh.c_fjno.Equals("XXXXXXXXXX")))
                //      //{
                //      //  if (fjrh != null)
                //      //  {
                //      //    db.LG_FJRHs.DeleteOnSubmit(fjrh);
                //      //  }

                //      //  continue;
                //      //}

                //      #region Populate Detail

                //      listFBRD3.Add(new LG_FJRD3()
                //      {
                //        c_fjno = fakturID,
                //        c_dono = prosesField.DeliveryID,
                //        c_rcno = prosesField.ReturID
                //      });

                //      for (nLoopC = 0; nLoopC < listProcessRetur.Count; nLoopC++)
                //      {
                //        prosesField = listProcessRetur[nLoopC];

                //        listRCD2Qry = listRCD2.FindAll(delegate(LG_RCD2 rcd)
                //        {
                //          return
                //            (string.IsNullOrEmpty(rcd.c_rcno) ? false : rcd.c_rcno.Trim().Equals(prosesField.ReturID, StringComparison.OrdinalIgnoreCase)) &&
                //            (string.IsNullOrEmpty(rcd.c_dono) ? false : rcd.c_dono.Trim().Equals(prosesField.DeliveryID, StringComparison.OrdinalIgnoreCase)) &&
                //            (string.IsNullOrEmpty(rcd.c_iteno) ? false : rcd.c_iteno.Trim().Equals(prosesField.Item, StringComparison.OrdinalIgnoreCase));
                //        });

                //        if ((listRCD2Qry != null) && (listRCD2Qry.Count > 0))
                //        {
                //          for (nLoopD = 0; nLoopD < listRCD2Qry.Count; nLoopD++)
                //          {
                //            listRCD2Qry[nLoopD].n_sisa = 0;
                //          }

                //          listFBRD1.Add(new LG_FJRD1()
                //          {
                //            c_fjno = fakturID,
                //            c_iteno = prosesField.Item,
                //            n_disc = prosesField.Discount,
                //            n_qty = prosesField.Quantity,
                //            n_salpri = prosesField.Harga
                //          });

                //          dGross = (prosesField.Quantity * prosesField.Harga);
                //          dDisc = (dGross * (prosesField.Discount / 100));

                //          listFBRD2.Add(new LG_FJRD2()
                //          {
                //            c_fjno = fakturID,
                //            c_iteno = prosesField.Item,
                //            c_no = string.Empty,
                //            c_type = prosesField.TypeItem,
                //            n_discoff = 0,
                //            n_discon = prosesField.Discount,
                //            n_netoff = 0,
                //            n_neton = dDisc
                //          });

                //          totalGross += dGross;
                //          totalDisc += dDisc;
                //        }
                //      }

                //      listProcessRetur.Clear();

                //      #endregion

                //      totalPajak = (isCabang ? 0 : ((totalGross - totalDisc) * 0.1m));

                //      fbrh.n_gross = totalGross;
                //      fbrh.n_disc = totalDisc;
                //      fbrh.n_tax = totalPajak;
                //      fbrh.n_net =
                //        fbrh.n_sisa = ((totalGross - totalDisc) + totalPajak);
                //}

            //    totalDetail++;
            //  }
            //}

            #endregion

            #endregion

            #region Save Data

            //if ((listFBRH != null) && (listFBRH.Count > 0))
            //{
            //  db.LG_FJRHs.InsertAllOnSubmit(listFBRH.ToArray());

            //  listFBRH.Clear();
            //}

            //if ((listFBRD1 != null) && (listFBRD1.Count > 0))
            //{
            //  db.LG_FJRD1s.InsertAllOnSubmit(listFBRD1.ToArray());

            //  listFBRD1.Clear();
            //}

            //if ((listFBRD2 != null) && (listFBRD2.Count > 0))
            //{
            //  db.LG_FJRD2s.InsertAllOnSubmit(listFBRD2.ToArray());

            //  listFBRD2.Clear();
            //}

            //if ((listFBRD3 != null) && (listFBRD3.Count > 0))
            //{
            //  db.LG_FJRD3s.InsertAllOnSubmit(listFBRD3.ToArray());

            //  listFBRD3.Clear();
            //}

            #endregion

            if (totalDetail > 0)
            {
              hasAnyChanges = true;
            }

            listRSD2.Clear();

            listHdr.Clear();
          }

          #endregion
        }

        if (hasAnyChanges)
        {
          db.SubmitChanges();

          db.Transaction.Commit();
          //db.Transaction.Rollback();

          rpe = ResponseParser.ResponseParserEnum.IsSuccess;
        }
        else
        {
          db.Transaction.Rollback();
        }
      }
      catch (Exception ex)
      {
        if (db.Transaction != null)
        {
          db.Transaction.Rollback();
        }

        result = string.Format("ScmsSoaLibrary.Bussiness.Faktur:FakturBeliRetur - {0}", ex.Message);

        Logger.WriteLine(result, true);
        Logger.WriteLine(ex.StackTrace);
      }

    endLogic:
      result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

      if (dic != null)
      {
        dic.Clear();
      }

      db.Dispose();

      return result;
    }

    public void FakturBeliReturAuto(ORMDataContext db, string nipEntry, ScmsSoaLibrary.Modules.CommonUploadedQuery.Temporary_ImportRS[] tempRsd3Values)
    {
      string result = null;
      
      int nLoop = 0,
         nLen = 0;

      decimal totalGross = 0,
        totalDisc = 0,
        totalNet = 0,
        totalSisa = 0,
        dQtyBad = 0,
        dQtyGood = 0,
        dQty = 0,
        dDisc = 0,
        dPrice = 0,
        dGross = 0;

      string taxNo = null,
        exNo = null,
        item = null;

      LG_FBRH fbrh = null;
      LG_FBRD1 fbrd1 = null;
      //LG_FBRD2 fbrd2 = null;
      //LG_FBRD3 fbrd3 = null;

      //List<LG_FBRH> listFBRH = null;
      //List<LG_FBRD3> listFBRD3 = null;
      //List<LG_FBRD4> listFBRD4 = null;

      ScmsSoaLibrary.Modules.CommonUploadedQuery.Temporary_ImportRS tirs = null;

      string fakturID = null;

      DateTime date = DateTime.Now,
        dateF = Functionals.StandardSqlDateTime;

      Dictionary<string, LG_FBRH> dicFBRH = new Dictionary<string, LG_FBRH>(StringComparer.OrdinalIgnoreCase);
      List<LG_FBRD1> listFBRD1 = new List<LG_FBRD1>();
      List<LG_FBRD2> listFBRD2 = new List<LG_FBRD2>();
      Dictionary<string, Temp_FakturBeliRetur> dicItemSupl = new Dictionary<string, Temp_FakturBeliRetur>(StringComparer.OrdinalIgnoreCase);
      Temp_FakturBeliRetur tfbr = null;

      try
      {
        for (nLoop = 0, nLen = tempRsd3Values.Length; nLoop < nLen; nLoop++)
        {
          tirs = tempRsd3Values[nLoop];

          dQtyBad = tirs.n_bsisa;
          dQtyGood = tirs.n_gsisa;

          dQty = (dQtyBad + dQtyGood);
          taxNo = tirs.c_taxno;
          exNo = tirs.c_fb;
          dPrice = tirs.n_salpri;
          dDisc = tirs.n_disc;

          item = (tirs.c_iteno ?? string.Empty);

          if (dQty > 0)
          {
            #region Header

            if (dicFBRH.ContainsKey(taxNo))
            {
              fbrh = dicFBRH[taxNo];

              totalGross = (fbrh.n_bruto.HasValue ? fbrh.n_bruto.Value : 0);
              totalDisc = (fbrh.n_disc.HasValue ? fbrh.n_disc.Value : 0);
              totalNet = (fbrh.n_bilva.HasValue ? fbrh.n_bilva.Value : 0);
              totalSisa = (fbrh.n_sisa.HasValue ? fbrh.n_sisa.Value : 0);

              fakturID = fbrh.c_fbno;
            }
            else
            {
              if (dicItemSupl.ContainsKey(item))
              {
                tfbr = dicItemSupl[item];
              }
              else
              {
                tfbr = (from q in db.FA_MasItms
                        where q.c_iteno == item
                        select new Temp_FakturBeliRetur()
                        {
                          NoSup= q.c_nosup,
                          IsImport = (q.l_import.HasValue ? q.l_import.Value : false)
                        }).Take(1).SingleOrDefault();

                dicItemSupl.Add(item, tfbr);
              }

              fakturID = Commons.GenerateNumbering<LG_FBRH>(db, "BR", '3', "05", date, "c_fbno");

              fbrh = new LG_FBRH()
              {
                c_fbno = fakturID,
                d_fbdate = date,
                c_type = "01",
                c_nosup = (tfbr == null ? "XXXXX" : (tfbr.NoSup ?? "XMRCX")),
                c_exno = exNo,
                c_taxno = taxNo,
                d_taxdate = tirs.d_taxdate,
                l_import = (tfbr == null ? false : tfbr.IsImport),
                c_kurs = "01",
                n_kurs = 1,
                v_ket = string.Empty,
                n_bruto = 0,
                n_disc = 0,
                n_pdisc = 0,
                n_xdisc = 0,
                n_bea = 0,
                n_ppn = 0,
                n_bilva = 0,
                n_sisa = 0,
                l_print = false,
                c_entry = nipEntry,
                d_entry = date,
                c_update = nipEntry,
                d_update = date
              };

              totalGross =
               totalDisc =
                totalNet =
                 totalSisa = 0;

              dicFBRH.Add(taxNo, fbrh);
            }

            #endregion

            #region Related RSD

            if (!listFBRD2.Exists(delegate(LG_FBRD2 fbrd)
            {
              return 
                (
                  fbrd.c_fbno.Trim().Equals(fakturID, StringComparison.OrdinalIgnoreCase) &&
                  fbrd.c_rsno.Trim().Equals(tirs.c_rsno, StringComparison.OrdinalIgnoreCase) &&
                  fbrd.c_rnno.Trim().Equals(tirs.c_rnno, StringComparison.OrdinalIgnoreCase)
                );
            }))
            {
              listFBRD2.Add(new LG_FBRD2()
              {
                c_fbno = fakturID,
                c_rsno = tirs.c_rsno,
                c_rnno = tirs.c_rnno
              });
            }

            #endregion

            #region Update Header

            if (fbrh != null)
            {
              dGross = (dQty * dPrice);

              totalGross += dGross;
              totalDisc += (dGross * (dDisc / 100));
              dGross = ((totalGross - totalDisc) * 0.1m);

              fbrh.n_bruto = totalGross;
              fbrh.n_disc = totalDisc;
              fbrh.n_ppn = dGross;
              fbrh.n_bilva =
                fbrh.n_sisa = ((totalGross - totalDisc) + dGross);
            }

            #endregion

            #region Insert / Update Detail

            fbrd1 = listFBRD1.Find(delegate(LG_FBRD1 fbrd)
            {
              return
                (
                fbrd.c_fbno.Trim().Equals(fakturID, StringComparison.OrdinalIgnoreCase) &&
                fbrd.c_iteno.Trim().Equals(item, StringComparison.OrdinalIgnoreCase)
                );

            });

            if (fbrd1 == null)
            {
              listFBRD1.Add(new LG_FBRD1()
              {
                c_fbno = fakturID,
                c_iteno = item,
                n_bea = 0,
                n_bqty = dQtyBad,
                n_gqty = dQtyGood,
                n_disc = dDisc,
                n_salpri = dPrice
              });
            }
            else
            {
              fbrd1.n_bqty += dQtyBad;
              fbrd1.n_gqty += dQtyGood;
            }

            #endregion
          }
        }

        if (dicFBRH.Count > 0)
        {
          db.LG_FBRHs.InsertAllOnSubmit(dicFBRH.Values.ToArray());

          dicFBRH.Clear();
        }

        if (listFBRD1.Count > 0)
        {
          db.LG_FBRD1s.InsertAllOnSubmit(listFBRD1.ToArray());

          listFBRD1.Clear();
        }

        if (listFBRD2.Count > 0)
        {
          db.LG_FBRD2s.InsertAllOnSubmit(listFBRD2.ToArray());

          listFBRD2.Clear();
        }
      }
      catch (Exception ex)
      {
        result = string.Format("ScmsSoaLibrary.Bussiness.Faktur:FakturBeliReturAuto - {0}", ex.Message);

        Logger.WriteLine(result, true);
        Logger.WriteLine(ex.StackTrace);
      }
    }

    public string FakturManual(ScmsSoaLibrary.Parser.Class.FakturManualStructure structure)
    {
        if ((structure == null)) // || (structure.Fields == null))
        {
            return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
        }

        bool hasAnyChanges = false;

        ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

        IDictionary<string, string> dic = null;

        LG_FM lg_fm = null;
        List<LG_FM1> listFM1 = null;

        LG_MsNomorPajak msnomorpajak = null;

        DateTime date = DateTime.Now,
        dateGen = DateTime.Now;

        //ScmsSoaLibrary.Parser.Class.FakturManualStructureField field = null;

        ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

        string result = null,
            nipEntry = null,
            fakturID = null,
            numgenerate = null;

        int current;

        dic = new Dictionary<string, string>();

        fakturID = structure.Fields.fakturID;

        nipEntry = structure.Fields.Entry;

        if (string.IsNullOrEmpty(nipEntry))
        {
            result = "Nip penanggung jawab dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            goto endLogic;
        }

        try
        {
            db.Connection.Open();

            db.Transaction = db.Connection.BeginTransaction();

            if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
            {
                #region Add

                lg_fm = new LG_FM();

                fakturID = Commons.GenerateNumbering<LG_FM>(db, "FM", '3', "41", date, "c_fmno");

                #region generate nomor faktur
                msnomorpajak = (from q in db.LG_MsNomorPajaks
                                where Convert.ToInt32(q.c_current) <= Convert.ToInt32(q.c_akhir)
                                    select q).OrderByDescending(x => x.IDX).Take(1).SingleOrDefault();

                if (msnomorpajak != null)
                {
                    current = Int32.Parse(msnomorpajak.c_current);

                    dateGen = structure.Fields.taxdatefaktur;

                    numgenerate = msnomorpajak.c_digit1 + "." + msnomorpajak.c_digit2 + "-" + date.Year.ToString().Substring(2, 2) + "." + (current).ToString("00000000");

                    msnomorpajak.c_current = (current + 1).ToString("00000000");
                }
                else
                {
                    result = "Nomor pajak tidak ditemukan, tambahkan master nomor pajak.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    goto endLogic;
                }



                #endregion generate nomor faktur


                lg_fm = new LG_FM()
                {
                    c_fmno = fakturID,
                    c_nosup = structure.Fields.nosup,
                    c_taxno = numgenerate,
                    d_taxdate = structure.Fields.taxdatefaktur,
                    n_dpp = structure.Fields.dpp,
                    n_ppn = structure.Fields.ppn,
                    n_total = structure.Fields.total,
                    v_ket = structure.Fields.ket,
                    v_ref = structure.Fields.referensi
                };

                db.LG_FMs.InsertOnSubmit(lg_fm);
                db.SubmitChanges();

                dic.Add("taxno", numgenerate);

                hasAnyChanges = true;
            }

                #endregion

            if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
            {
                #region Modify

                if (string.IsNullOrEmpty(fakturID))
                {
                    result = "Nomor faktur dibutuhkan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                lg_fm = (from q in db.LG_FMs
                         where q.c_fmno == fakturID
                         select q).Take(1).SingleOrDefault();

                if (lg_fm == null)
                {
                    result = "Nomor faktur manual tidak ditemukan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (lg_fm.l_delete.HasValue && lg_fm.l_delete.Value)
                {
                    result = "Tidak dapat mengubah nomor faktur manual yang sudah terhapus.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (Commons.IsClosingFA(db, lg_fm.d_taxdate.Value))
                {
                    result = "Faktur tidak dapat dihapus, karena tanggal pada transaksi ini sudah closing.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                listFM1 = new List<LG_FM1>();

                if (lg_fm != null)
                {
                    listFM1.Add(new LG_FM1()
                    {
                        c_fmno = fakturID,
                        c_nosup = lg_fm.c_nosup,
                        c_taxno = lg_fm.c_taxno,
                        d_taxdate = lg_fm.d_taxdate,
                        n_dpp = lg_fm.n_dpp,
                        n_ppn = lg_fm.n_ppn,
                        n_total = lg_fm.n_total,
                        v_ket = lg_fm.v_ket,
                        v_ref = lg_fm.v_ref,
                        c_entry = nipEntry,
                        d_entry = date,
                        v_ket_del = string.IsNullOrEmpty(structure.Fields.KeteranganDel) ? "Deleted by " + nipEntry : structure.Fields.KeteranganDel
                    });

                    if ((listFM1 != null) && (listFM1.Count > 0))
                    {
                        db.LG_FM1s.InsertAllOnSubmit(listFM1.ToArray());

                        listFM1.Clear();
                    }


                    lg_fm.c_nosup = structure.Fields.nosup;
                    lg_fm.c_taxno = structure.Fields.taxno;
                    lg_fm.d_taxdate = structure.Fields.taxdatefaktur;
                    lg_fm.n_dpp = structure.Fields.dpp;
                    lg_fm.n_ppn = structure.Fields.ppn;
                    lg_fm.n_total = structure.Fields.total;
                    lg_fm.v_ket = structure.Fields.ket;
                    lg_fm.v_ref = structure.Fields.referensi;


                    hasAnyChanges = true;
                }
                else
                {
                    result = "Data faktur tidak terbaca dari database.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                #endregion
            }

            #region Delete
            if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(fakturID))
                {
                    result = "Nomor faktur dibutuhkan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                lg_fm = (from q in db.LG_FMs
                        where q.c_fmno == fakturID
                        select q).Take(1).SingleOrDefault();

                if (lg_fm == null)
                {
                    result = "Nomor faktur manual tidak ditemukan.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (lg_fm.l_delete.HasValue && lg_fm.l_delete.Value)
                {
                    result = "Tidak dapat menghapus nomor faktur manual yang sudah terhapus.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }
                else if (Commons.IsClosingFA(db, lg_fm.d_taxdate.Value))
                {
                    result = "Faktur tidak dapat dihapus, karena tanggal pada transaksi ini sudah closing.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }

                    goto endLogic;
                }

                listFM1 = new List<LG_FM1>();

                if (lg_fm != null)
                {
                    listFM1.Add(new LG_FM1()
                    {
                        c_fmno = fakturID,
                        c_nosup = lg_fm.c_nosup,
                        c_taxno = lg_fm.c_taxno,
                        d_taxdate = lg_fm.d_taxdate,
                        n_dpp = lg_fm.n_dpp,
                        n_ppn = lg_fm.n_ppn,
                        n_total = lg_fm.n_total,
                        v_ket = lg_fm.v_ket,
                        v_ref = lg_fm.v_ref,
                        c_entry = nipEntry,
                        d_entry = date,
                        v_ket_del = string.IsNullOrEmpty(structure.Fields.KeteranganDel) ? "Deleted by " + nipEntry : structure.Fields.KeteranganDel
                    });

                    if ((listFM1 != null) && (listFM1.Count > 0))
                    {
                        db.LG_FM1s.InsertAllOnSubmit(listFM1.ToArray());

                        listFM1.Clear();
                    }

                    db.LG_FMs.DeleteOnSubmit(lg_fm);
                    

                    hasAnyChanges = true;
                }


                hasAnyChanges = true;
            }


            dic.Add("fmno", fakturID);
            dic.Add("taxdate", structure.Fields.taxdatefaktur.ToString("yyyyMMdd"));
            #endregion

            if (hasAnyChanges)
            {
                db.SubmitChanges();

                db.Transaction.Commit();

                //db.Transaction.Rollback();

                rpe = ResponseParser.ResponseParserEnum.IsSuccess;
            }
            else
            {
                db.Transaction.Rollback();

                rpe = ResponseParser.ResponseParserEnum.IsFailed;
            }
        }
        catch (Exception ex)
        {
            if (db.Transaction != null)
            {
                db.Transaction.Rollback();
            }

            result = string.Format("ScmsSoaLibrary.Bussiness.Faktur:FakturManual - {0}", ex.Message);

            Logger.WriteLine(result);
            Logger.WriteLine(ex.StackTrace);
        }

    endLogic:
        result = Parser.ResponseParser.ResponseGenerator(rpe, dic, result);

        if (dic != null)
        {
            dic.Clear();
        }

        db.Dispose();

        return result;
    }
  }
}
