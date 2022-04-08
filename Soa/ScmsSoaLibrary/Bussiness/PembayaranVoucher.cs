using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsModel;
using ScmsSoaLibrary.Parser;
//using ScmsSoaLibrary.Core.Crypto;
using ScmsSoaLibrary.Commons;
using System.Data.Common;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaLibrary.Bussiness
{
  class PembayaranVoucher
  {
    public string VoucherDebit(ScmsSoaLibrary.Parser.Class.PembayaranVchStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      string result = null;

      bool hasAnyChanges = false;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      LG_VDNH vdn = null;
      LG_DNH dnh = null;
      LG_DND dnd = null;
      //LG_DND2 dnd2 = null;

      ScmsSoaLibrary.Parser.Class.PembayaranVchStructureField field = null,
        field2 = null;

      string nipEntry = null;
      string vcID = null,
        noteID = null;
      string tmpNumbering = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      decimal allocMoney = 0,
        jml = 0,
        nSisa = 0,
        nAvailble = 0;

      List<LG_DND> listDnd = null;
      List<LG_DND2> listDnd2 = null;

      List<LG_FBH> listFbh = null;
      List<LG_FBRH> listFbrh = null;

      List<ScmsSoaLibrary.Parser.Class.PembayaranVchStructureField> listFaktur = null;
      List<ScmsSoaLibrary.Parser.Class.PembayaranVchStructureField> listRetur = null;

      LG_FBH fbh = null;
      LG_FBRH fbrh = null;

      int nLoop = 0,
        nLoopC = 0;

      List<string> listFbNo = null;
      List<string> listFbrNo = null;

      IDictionary<string, string> dic = null;

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }

      int totalDetails = 0;
      bool isGiroPayment = false,
        isVoucherNote = false;

      noteID = (structure.Fields.PembayaranNoteID ?? string.Empty);

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          isGiroPayment = (string.IsNullOrEmpty(structure.Fields.JenisPembayaran) || structure.Fields.JenisPembayaran.Equals("01") ? false : true);
          isVoucherNote = (string.IsNullOrEmpty(structure.Fields.TipePembayaran) || structure.Fields.TipePembayaran.Equals("01") ? true : false);

          #region Standard Header Validation

          if (!string.IsNullOrEmpty(noteID))
          {
            result = "Nomor Note harus kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (structure.Fields.TanggalDebitDate.Date.Equals(DateTime.MinValue.Date))
          {
            result = "Format tanggal debit tidak terbaca.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.JenisPembayaran))
          {
            result = "Jenis pembayaran dibutuhkan.";

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
          else if (string.IsNullOrEmpty(structure.Fields.TipePembayaran))
          {
            result = "Tipe pembayaran dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Bank) && isVoucherNote)
          {
            result = "Bank dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Rekening) && isVoucherNote)
          {
            result = "Nomor rekening dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (isGiroPayment && string.IsNullOrEmpty(structure.Fields.NoGiro))
          {
            result = "Nomor giro dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (isGiroPayment && structure.Fields.TanggalDebitDate.Date.Equals(DateTime.MinValue.Date))
          {
            result = "Format tanggal tempo giro tidak terbaca.";

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

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (structure.Fields.KursValue < 1)
          {
            result = "Nilai kurs salah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (structure.Fields.JumlahPembayaran <= 0)
          {
            result = "Pembayaran tidak boleh kurang dari 1.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if(Commons.IsClosingFA(db, structure.Fields.TanggalDebitDate))
          {
            result = "Pembayaran tidak dapat disimpan pada tanggal ini, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          #endregion

          //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "DnNote");
          vcID = Commons.GenerateNumbering<LG_VDNH>(db, "LD", '3', "25", date, "c_vdno");

          allocMoney = structure.Fields.JumlahPembayaran;

          #region Add Voucher

          if (isVoucherNote)
          {
            vdn = new LG_VDNH()
            {
              c_bank = structure.Fields.Bank,
              c_entry = nipEntry,
              c_kurs = structure.Fields.Kurs,
              c_nosup = structure.Fields.Suplier,
              c_rekNo = structure.Fields.Rekening,
              c_type = structure.Fields.JenisPembayaran,
              c_update = nipEntry,
              c_vdno = vcID,
              d_entry = date,
              d_tempo = structure.Fields.TempoGiroDate,
              d_update = date,
              d_vddate = structure.Fields.TanggalDebitDate,
              l_print = false,
              l_um = structure.Fields.IsDownPayment,
              n_admin = structure.Fields.BiayaAdmin,
              n_bilva = allocMoney,
              n_kurs = structure.Fields.KursValue,
              n_sisa = 0,
              v_ket = tmpNumbering,
              v_no = (isGiroPayment ? structure.Fields.NoGiro : null)
            };

            db.LG_VDNHs.InsertOnSubmit(vdn);

            #region Old Code

            //db.SubmitChanges();

            //vdn = (from q in db.LG_VDNHs
            //       where q.v_ket == tmpNumbering
            //       select q).Take(1).SingleOrDefault();

            //if (!string.IsNullOrEmpty(vcID))
            //{
            //  result = "Nomor Voucher Debit tidak dapat di raih.";

            //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

            //  if (db.Transaction != null)
            //  {
            //    db.Transaction.Rollback();
            //  }

            //  goto endLogic;
            //}
            //else if (vdn.c_vdno.Equals("XXXXXXXXXX"))
            //{
            //  result = "Trigger Voucher Debit tidak aktif.";

            //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

            //  if (db.Transaction != null)
            //  {
            //    db.Transaction.Rollback();
            //  }

            //  goto endLogic;
            //}

            //vdn.v_ket = structure.Fields.Keterangan;

            //vcID = vdn.c_vdno.Trim();

            #endregion
          }

          #endregion

          #region Add Note Pembayaran
          //noteID

          noteID = Commons.GenerateNumbering<LG_DNH>(db, "DO", '3', "23", date, "c_noteno");

          dnh = new LG_DNH()
          {
            c_vdno = vcID,
            c_entry = nipEntry,
            c_kurs = structure.Fields.Kurs,
            c_nosup = structure.Fields.Suplier,
            c_noteno = noteID,
            c_type = structure.Fields.TipePembayaran,
            c_update = nipEntry,
            d_entry = date,
            d_notedate = structure.Fields.TanggalDebitDate,
            d_update = date,
            l_print = false,
            n_bilva = allocMoney,
            n_diff = 0,
            n_kurs = structure.Fields.KursValue,
            v_ket = tmpNumbering
          };
          
          db.LG_DNHs.InsertOnSubmit(dnh);

          #region Old Code

          //db.SubmitChanges();

          //dnh = (from q in db.LG_DNHs
          //       where q.v_ket == tmpNumbering
          //       select q).Take(1).SingleOrDefault();

          //if (dnh.c_noteno.Equals("XXXXXXXXXX"))
          //{
          //  result = "Trigger Note Pembayaran tidak aktif.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}

          //dnh.v_ket = structure.Fields.Keterangan;

          //noteID = dnh.c_noteno.Trim();

          #endregion

          #endregion

          #region Insert Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listFaktur = structure.Fields.Field.Where(x => x.TipeFaktur == "01").Select(z => z).ToList();
            listFbNo = listFaktur.GroupBy(y => y.Faktur).Select(z => z.Key.Trim()).ToList();
            if (listFbNo.Count > 0)
            {
              listFbh = (from q in db.LG_FBHs
                         where listFbNo.Contains(q.c_fbno)
                          && (q.n_sisa > 0)
                         select q).ToList();
            }

            listRetur = structure.Fields.Field.Where(x => x.TipeFaktur == "02").Select(z => z).ToList();
            listFbrNo = listRetur.GroupBy(y => y.Faktur).Select(z => z.Key.Trim()).ToList();
            if (listFbrNo.Count > 0)
            {
              listFbrh = (from q in db.LG_FBRHs
                          where listFbrNo.Contains(q.c_fbno)
                            && (q.n_sisa > 0)
                          select q).ToList();
            }

            listDnd = new List<LG_DND>();

            if (isVoucherNote)
            {
              #region Detail Voucher Note Tipe

              for (nLoop = 0; nLoop < listFaktur.Count; nLoop++)
              {
                field = listFaktur[nLoop];

                jml = field.Jumlah;

                #region Tipe Voucher

                fbh = listFbh.Find(delegate(LG_FBH fb)
                {
                  return fb.c_fbno.Trim().Equals(field.Faktur, StringComparison.OrdinalIgnoreCase);
                });

                nAvailble = (jml > allocMoney ? allocMoney : jml);

                if (fbh != null)
                {
                  nSisa = (fbh.n_sisa.HasValue ? fbh.n_sisa.Value : 0);

                  if (nSisa > 0)
                  {
                    //nSisa -= nAvailble;

                    fbh.n_sisa -= nAvailble;

                    allocMoney -= nAvailble;

                    field.Jumlah -= nAvailble;
                  }
                  else
                  {
                    allocMoney = -1;

                    break;
                  }
                }
                else
                {
                  allocMoney = -1;

                  break;
                }

                //if (nSisa < 0)
                //{
                //  allocMoney = -1;

                //  break;
                //}

                #region Insert LG_DND

                listDnd.Add(new LG_DND()
                {
                  c_fbno = fbh.c_fbno,
                  c_noteno = noteID,
                  c_trans = "00",
                  c_type = "01",
                  c_vdno = vcID,
                  n_value = nAvailble
                });

                totalDetails++;

                #endregion

                #endregion

                if (allocMoney <= 0)
                {
                  break;
                }
              }

              #endregion
            }
            else
            {
              allocMoney = 0;
            }

            #region Remove Faktur Bayar <= 0

            for (nLoop = (listFaktur.Count - 1); nLoop >= 0; nLoop--)
            {
              field = listFaktur[nLoop];

              if (field.Jumlah <= 0)
              {
                listFaktur.RemoveAt(nLoop);
              }
            }

            #endregion

            #region Bayar Pakai Retur

            if ((listRetur.Count > 0) && (allocMoney == 0))
            {
              for (nLoop = 0; nLoop < listRetur.Count; nLoop++)
              {
                field = listRetur[nLoop];

                fbrh = listFbrh.Find(delegate(LG_FBRH fbr)
                {
                  return fbr.c_fbno.Trim().Equals(field.Faktur, StringComparison.OrdinalIgnoreCase);
                });

                nSisa = (-field.Jumlah);

                if ((fbrh != null) && (nSisa > 0))
                {
                  nAvailble = (fbrh.n_sisa.HasValue ? fbrh.n_sisa.Value : 0);

                  allocMoney = (nSisa > nAvailble ? nAvailble : nSisa);

                  if (allocMoney > 0)
                  {
                    for (nLoopC = 0; nLoopC < listFaktur.Count; nLoopC++)
                    {
                      if (allocMoney <= 0)
                      {
                        break;
                      }

                      field2 = listFaktur[nLoopC];

                      jml = field2.Jumlah;

                      if (jml <= 0)
                      {
                        continue;
                      }

                      fbh = listFbh.Find(delegate(LG_FBH fb)
                      {
                        return fb.c_fbno.Trim().Equals(field2.Faktur, StringComparison.OrdinalIgnoreCase);
                      });

                      if (fbh == null)
                      {
                        nAvailble = -1;

                        break;
                      }

                      nSisa = (fbh.n_sisa.HasValue ? fbh.n_sisa.Value : 0);

                      if (nSisa <= 0)
                      {
                        nAvailble = -1;

                        break;
                      }

                      nAvailble = (allocMoney > nSisa ? nSisa : allocMoney);
                      nAvailble = (nAvailble > jml ? jml : nAvailble);

                      listDnd.Add(new LG_DND()
                      {
                        c_fbno = fbh.c_fbno,
                        c_noteno = noteID,
                        c_trans = "00",
                        c_type = "02",
                        c_vdno = fbrh.c_fbno,
                        n_value = nAvailble
                      });

                      totalDetails++;

                      //nSisa = (jml > nAvailble ? nAvailble : jml);

                      //nAvailble -= nSisa;

                      //fbh.n_sisa -= nSisa;

                      //field2.Jumlah -= nSisa;

                      fbh.n_sisa -= nAvailble;

                      fbrh.n_sisa -= nAvailble;

                      field2.Jumlah -= nAvailble;

                      allocMoney -= nAvailble;

                      if (allocMoney <= 0)
                      {
                        break;
                      }
                    }

                    #region Old Coded

                    //if (nAvailble == -1)
                    //{
                    //  allocMoney = -1;

                    //  break;
                    //}
                    //else
                    //{
                    //  fbrh.n_sisa -= allocMoney;

                    //  allocMoney = 0;
                    //}

                    #endregion
                  }
                }
                else
                {
                  allocMoney = -1;

                  break;
                }
              }
            }

            #endregion
            
            if (allocMoney == 0)
            {
              if (listDnd.Count > 0)
              {
                db.LG_DNDs.InsertAllOnSubmit(listDnd.ToArray());
              }

              listDnd.Clear();

              hasAnyChanges = true;
            }
            else
            {
              result = "Nilai voucher tidak balance.";

              rpe = ResponseParser.ResponseParserEnum.IsFailed;

              if (db.Transaction != null)
              {
                db.Transaction.Rollback();
              }

              goto endLogic;
            }

            #region Clear List
            
            if (listFaktur != null)
            {
              listFaktur.Clear();
            }
            if (listFbNo != null)
            {
              listFbNo.Clear();
            }
            if (listFbh != null)
            {
              listFbh.Clear();
            }
            if (listRetur != null)
            {
              listRetur.Clear();
            }
            if (listFbrNo != null)
            {
              listFbrNo.Clear();
            }
            if (listFbrh != null)
            {
              listFbrh.Clear();
            }
            
            #endregion
          }

          #endregion

          if (hasAnyChanges)
          {
            dic = new Dictionary<string, string>();

            dic.Add("Note", noteID);
            dic.Add("Voucher", vcID);
            dic.Add("Tanggal", structure.Fields.TanggalDebitDate.ToString("yyyyMMdd"));

            result = string.Format("Total {0} detail(s)", totalDetails);
          }

          #endregion
        }
        else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete
          
          if (string.IsNullOrEmpty(noteID))
          {
            result = "Nomor Note dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          dnh = (from q in db.LG_DNHs
                 where q.c_noteno == noteID
                 select q).Take(1).SingleOrDefault();

          if (dnh == null)
          {
            result = "Nomor Note tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (dnh.l_delete.HasValue && dnh.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor Note yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if ((!dnh.d_notedate.HasValue) || dnh.d_notedate.Value.Equals(DateTime.MinValue) || dnh.d_notedate.Value.Equals(DateTime.MaxValue))
          {
            result = "Tanggal transaksi tidak terbaca.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingFA(db, dnh.d_notedate.Value))
          {
            result = "Pembayaran tidak dapat dihapus, karena tanggal pada transaksi ini sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          dnh.c_update = nipEntry;
          dnh.d_update = DateTime.Now;

          dnh.l_delete = true;
          dnh.v_ket_mark = structure.Fields.Keterangan;

          vcID = (dnh.c_vdno ?? "");

          listDnd2 = new List<LG_DND2>();

          listDnd = (from q in db.LG_DNDs
                      where q.c_noteno == noteID
                      select q).ToList();

          isVoucherNote = (string.IsNullOrEmpty(dnh.c_type) || dnh.c_type.Equals("01") ? true : false);

          if ((listDnd != null) && (listDnd.Count > 0))
          {
            listFbNo = listDnd.Where(x => x.c_type == "01").GroupBy(y => y.c_fbno).Select(z => z.Key.Trim()).ToList();
            if (listFbNo.Count > 0)
            {
              listFbh = (from q in db.LG_FBHs
                         where listFbNo.Contains(q.c_fbno)
                         select q).ToList();

              listFbNo.Clear();
            }

            listFbrNo = listDnd.Where(x => x.c_type == "02").GroupBy(y => y.c_fbno).Select(z => z.Key.Trim()).ToList();
            if (listFbrNo.Count > 0)
            {
              listFbrh = (from q in db.LG_FBRHs
                          where listFbrNo.Contains(q.c_fbno)
                          select q).ToList();

              listFbrNo.Clear();
            }

            #region Check & Move Detail Debit dan Faktur

            jml = allocMoney = 0;
            for (nLoopC = 0; nLoopC < listDnd.Count; nLoopC++)
            {
              dnd = listDnd[nLoopC];

              if (dnd.c_type.Equals("01"))
              {
                #region Faktur

                fbh = listFbh.Find(delegate(LG_FBH fb)
                {
                  return fb.c_fbno.Trim().Equals(dnd.c_fbno.Trim());
                });

                jml = (dnd.n_value.HasValue ? dnd.n_value.Value : 0);
                allocMoney += jml;

                if (fbh != null)
                {
                  nAvailble = (fbh.n_bilva.HasValue ? fbh.n_bilva.Value : 0);
                  nSisa = (fbh.n_sisa.HasValue ? fbh.n_sisa.Value : 0);

                  jml = ((nSisa + jml) > nAvailble ? (nAvailble - nSisa) : jml);

                  fbh.n_sisa += jml;
                }

                #endregion
              }
              else
              {
                #region Voucher Retur

                fbrh = listFbrh.Find(delegate(LG_FBRH fbr)
                {
                  return fbr.c_fbno.Trim().Equals(dnd.c_vdno.Trim());
                });

                jml = (dnd.n_value.HasValue ? dnd.n_value.Value : 0);
                //allocMoney += jml;

                if (fbrh != null)
                {
                  nAvailble = (fbrh.n_bilva.HasValue ? fbrh.n_bilva.Value : 0);
                  nSisa = (fbrh.n_sisa.HasValue ? fbrh.n_sisa.Value : 0);

                  jml = ((nSisa + jml) > nAvailble ? (nAvailble - nSisa) : jml);

                  fbrh.n_sisa += jml;
                }

                #endregion
              }

              listDnd2.Add(new LG_DND2()
              {
                c_entry = nipEntry,
                c_fbno = dnd.c_fbno,
                c_noteno = noteID,
                c_trans = dnd.c_trans,
                c_type = dnd.c_type,
                c_vdno = dnd.c_vdno,
                d_entry = date,
                n_value = dnd.n_value,
                v_ket_del = structure.Fields.Keterangan,
                v_type = "03"
              });

              totalDetails++;
            }

            if (listDnd2.Count > 0)
            {
              db.LG_DND2s.InsertAllOnSubmit(listDnd2.ToArray());

              listDnd2.Clear();
            }

            db.LG_DNDs.DeleteAllOnSubmit(listDnd.ToArray());

            listDnd.Clear();

            #endregion

            dnh.n_bilva -= allocMoney;
          }

          #region Check Voucher

          if (isVoucherNote)
          {
            vdn = (from q in db.LG_VDNHs
                   where q.c_vdno == vcID
                   select q).Take(1).SingleOrDefault();

            if (vdn != null)
            {
              vdn.n_sisa += allocMoney;

              vdn.l_delete = true;
              vdn.v_ket_mark = structure.Fields.Keterangan;
            }
          }

          #endregion

          hasAnyChanges = true;

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

          rpe = ResponseParser.ResponseParserEnum.IsFailed;
        }
      }
      catch (Exception ex)
      {
        if (db.Transaction != null)
        {
          db.Transaction.Rollback();
        }
        rpe = ResponseParser.ResponseParserEnum.IsError;

        result = string.Format("ScmsSoaLibrary.Bussiness.PembayaranVoucher:VoucherDebit - {0}", ex.Message);

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

    public string VoucherKredit(ScmsSoaLibrary.Parser.Class.PembayaranVchStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      string result = null;

      bool hasAnyChanges = false;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      LG_VCNH vcn = null;
      LG_CNH cnh = null;
      LG_CND cnd = null;
      //LG_CND2 cnd2 = null;

      ScmsSoaLibrary.Parser.Class.PembayaranVchStructureField field = null,
        field2 = null;

      string nipEntry = null;
      string vcID = null,
        noteID = null;
      string tmpNumbering = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;
      DateTime date = DateTime.Now;

      decimal allocMoney = 0,
        jml = 0,
        nSisa = 0,
        nAvailble = 0;

      List<LG_CND> listCnd = null;
      List<LG_CND2> listCnd2 = null;

      List<LG_FJH> listFjh = null;
      List<LG_FJRH> listFjrh = null;

      List<ScmsSoaLibrary.Parser.Class.PembayaranVchStructureField> listFaktur = null;
      List<ScmsSoaLibrary.Parser.Class.PembayaranVchStructureField> listRetur = null;

      LG_FJH fjh = null;
      LG_FJRH fjrh = null;

      int nLoop = 0,
        nLoopC = 0;

      List<string> listFjNo = null;
      List<string> listFjrNo = null;

      IDictionary<string, string> dic = null;

      nipEntry = (structure.Fields.Entry ?? string.Empty);

      if (string.IsNullOrEmpty(nipEntry))
      {
        result = "Nip penanggung jawab dibutuhkan.";

        rpe = ResponseParser.ResponseParserEnum.IsFailed;

        goto endLogic;
      }

      int totalDetails = 0;
      bool isGiroPayment = false,
        isVoucherNote = false;

      noteID = (structure.Fields.PembayaranNoteID ?? string.Empty);

      try
      {
        db.Connection.Open();

        db.Transaction = db.Connection.BeginTransaction();

        if (structure.Method.Equals("Add", StringComparison.OrdinalIgnoreCase))
        {
          #region Add

          isGiroPayment = (string.IsNullOrEmpty(structure.Fields.JenisPembayaran) || structure.Fields.JenisPembayaran.Equals("01") ? false : true);
          isVoucherNote = (string.IsNullOrEmpty(structure.Fields.TipePembayaran) || structure.Fields.TipePembayaran.Equals("01") ? true : false);

          #region Standard Header Validation

          if (!string.IsNullOrEmpty(noteID))
          {
            result = "Nomor Note harus kosong.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (structure.Fields.TanggalDebitDate.Date.Equals(DateTime.MinValue.Date))
          {
            result = "Format tanggal debit tidak terbaca.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.JenisPembayaran))
          {
            result = "Jenis pembayaran dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Customer))
          {
            result = "Nama cabang dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.TipePembayaran))
          {
            result = "Tipe pembayaran dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Bank) && isVoucherNote)
          {
            result = "Bank dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (string.IsNullOrEmpty(structure.Fields.Rekening) && isVoucherNote)
          {
            result = "Nomor rekening dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (isGiroPayment && string.IsNullOrEmpty(structure.Fields.NoGiro))
          {
            result = "Nomor giro dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (isGiroPayment && structure.Fields.TanggalDebitDate.Date.Equals(DateTime.MinValue.Date))
          {
            result = "Format tanggal tempo giro tidak terbaca.";

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

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (structure.Fields.KursValue < 1)
          {
            result = "Nilai kurs salah.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (structure.Fields.JumlahPembayaran <= 0)
          {
            result = "Pembayaran tidak boleh kurang dari 1.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }
          else if (Commons.IsClosingFA(db, structure.Fields.TanggalDebitDate))
          {
            result = "Pembayaran tidak dapat disimpan pada tanggal ini, karena sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            if (db.Transaction != null)
            {
              db.Transaction.Rollback();
            }

            goto endLogic;
          }

          #endregion

          //tmpNumbering = Functionals.GeneratedRandomUniqueId(50, "CnNote");

          vcID = Commons.GenerateNumbering<LG_VCNH>(db, "LC", '3', "26", date, "c_vcno");

          allocMoney = structure.Fields.JumlahPembayaran;

          #region Add Voucher

          if (isVoucherNote)
          {
            vcn = new LG_VCNH()
            {
              c_bank = structure.Fields.Bank,
              c_entry = nipEntry,
              c_kurs = structure.Fields.Kurs,
              c_cusno = structure.Fields.Customer,
              c_rekno = structure.Fields.Rekening,
              c_type = structure.Fields.JenisPembayaran,
              c_update = nipEntry,
              c_vcno = vcID,
              d_entry = date,
              d_tempo = structure.Fields.TempoGiroDate,
              d_update = date,
              d_vcdate = structure.Fields.TanggalDebitDate,
              l_print = false,
              n_bilva = allocMoney,
              n_kurs = structure.Fields.KursValue,
              n_sisa = 0,
              v_ket = tmpNumbering,
              v_no = (isGiroPayment ? structure.Fields.NoGiro : null)
            };

            db.LG_VCNHs.InsertOnSubmit(vcn);

            #region Old Code

            //db.SubmitChanges();

            //vcn = (from q in db.LG_VCNHs
            //       where q.v_ket == tmpNumbering
            //       select q).Take(1).SingleOrDefault();

            //if (!string.IsNullOrEmpty(vcID))
            //{
            //  result = "Nomor Voucher Credit tidak dapat di raih.";

            //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

            //  if (db.Transaction != null)
            //  {
            //    db.Transaction.Rollback();
            //  }

            //  goto endLogic;
            //}
            //else if (vcn.c_vcno.Equals("XXXXXXXXXX"))
            //{
            //  result = "Trigger Voucher Credit tidak aktif.";

            //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

            //  if (db.Transaction != null)
            //  {
            //    db.Transaction.Rollback();
            //  }

            //  goto endLogic;
            //}

            //vcn.v_ket = structure.Fields.Keterangan;

            //vcID = vcn.c_vcno.Trim();

            #endregion
          }

          #endregion

          #region Add Note Pembayaran
          //noteID

          noteID = Commons.GenerateNumbering<LG_CNH>(db, "CL", '3', "27", date, "c_noteno");

          cnh = new LG_CNH()
          {
            c_vcno = vcID,
            c_entry = nipEntry,
            c_kurs = structure.Fields.Kurs,
            c_cusno = structure.Fields.Customer,
            c_noteno = noteID,
            c_type = structure.Fields.TipePembayaran,
            c_update = nipEntry,
            d_entry = date,
            d_notedate = structure.Fields.TanggalDebitDate,
            d_update = date,
            l_print = false,
            n_bilva = allocMoney,
            n_kurs = structure.Fields.KursValue,
            v_ket = tmpNumbering
          };

          db.LG_CNHs.InsertOnSubmit(cnh);

          #region Old Code

          //db.SubmitChanges();

          //cnh = (from q in db.LG_CNHs
          //       where q.v_ket == tmpNumbering
          //       select q).Take(1).SingleOrDefault();

          //if (!string.IsNullOrEmpty(noteID))
          //{
          //  result = "Nomor Note Pembayaran tidak dapat di raih.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}
          //else if (cnh.c_noteno.Equals("XXXXXXXXXX"))
          //{
          //  result = "Trigger Note Pembayaran tidak aktif.";

          //  rpe = ResponseParser.ResponseParserEnum.IsFailed;

          //  if (db.Transaction != null)
          //  {
          //    db.Transaction.Rollback();
          //  }

          //  goto endLogic;
          //}

          //cnh.v_ket = structure.Fields.Keterangan;

          //noteID = cnh.c_noteno.Trim();

          #endregion

          #endregion

          #region Insert Detail

          if ((structure.Fields.Field != null) && (structure.Fields.Field.Length > 0))
          {
            listFaktur = structure.Fields.Field.Where(x => x.TipeFaktur == "01").Select(z => z).ToList();
            listFjNo = listFaktur.GroupBy(y => y.Faktur).Select(z => z.Key.Trim()).ToList();
            if (listFjNo.Count > 0)
            {
              listFjh = (from q in db.LG_FJHs
                         where listFjNo.Contains(q.c_fjno)
                          && (q.n_sisa > 0)
                         select q).ToList();
            }

            listRetur = structure.Fields.Field.Where(x => x.TipeFaktur == "02").Select(z => z).ToList();
            listFjrNo = listRetur.GroupBy(y => y.Faktur).Select(z => z.Key.Trim()).ToList();
            if (listFjrNo.Count > 0)
            {
              listFjrh = (from q in db.LG_FJRHs
                          where listFjrNo.Contains(q.c_fjno)
                            && (q.n_sisa > 0)
                          select q).ToList();
            }

            listCnd = new List<LG_CND>();

            if (isVoucherNote)
            {
              #region Detail Voucher Note Tipe

              for (nLoop = 0; nLoop < listFaktur.Count; nLoop++)
              {
                field = listFaktur[nLoop];

                jml = field.Jumlah;

                #region Tipe Voucher

                fjh = listFjh.Find(delegate(LG_FJH fj)
                {
                  return fj.c_fjno.Trim().Equals(field.Faktur, StringComparison.OrdinalIgnoreCase);
                });

                nAvailble = (jml > allocMoney ? allocMoney : jml);

                if (fjh != null)
                {
                  nSisa = (fjh.n_sisa.HasValue ? fjh.n_sisa.Value : 0);

                  if (nSisa > 0)
                  {
                    //nSisa -= nAvailble;

                    fjh.n_sisa -= nAvailble;

                    allocMoney -= nAvailble;

                    field.Jumlah -= nAvailble;
                  }
                  else
                  {
                    allocMoney = -1;

                    break;
                  }
                }
                else
                {
                  allocMoney = -1;

                  break;
                }

                //if (nSisa < 0)
                //{
                //  allocMoney = -1;

                //  break;
                //}

                #region Insert LG_DND

                listCnd.Add(new LG_CND()
                {
                  c_fjno = fjh.c_fjno,
                  c_noteno = noteID,
                  c_type = "01",
                  c_vcno = vcID,
                  n_value = nAvailble
                });

                totalDetails++;

                #endregion

                #endregion

                if (allocMoney <= 0)
                {
                  break;
                }
              }

              #endregion
            }
            else
            {
              allocMoney = 0;
            }

            #region Remove Faktur Bayar <= 0

            for (nLoop = (listFaktur.Count - 1); nLoop >= 0; nLoop--)
            {
              field = listFaktur[nLoop];

              if (field.Jumlah <= 0)
              {
                listFaktur.RemoveAt(nLoop);
              }
            }

            #endregion

            #region Bayar Pakai Retur

            if ((listRetur.Count > 0) && (allocMoney == 0))
            {
              for (nLoop = 0; nLoop < listRetur.Count; nLoop++)
              {
                field = listRetur[nLoop];

                fjrh = listFjrh.Find(delegate(LG_FJRH fjr)
                {
                  return fjr.c_fjno.Trim().Equals(field.Faktur, StringComparison.OrdinalIgnoreCase);
                });

                nSisa = (-field.Jumlah);

                if ((fjrh != null) && (nSisa > 0))
                {
                  nAvailble = (fjrh.n_sisa.HasValue ? fjrh.n_sisa.Value : 0);

                  allocMoney = (nSisa > nAvailble ? nAvailble : nSisa);

                  if (allocMoney > 0)
                  {
                    for (nLoopC = 0; nLoopC < listFaktur.Count; nLoopC++)
                    {
                      field2 = listFaktur[nLoopC];

                      jml = field2.Jumlah;

                      if (jml <= 0)
                      {
                        continue;
                      }

                      fjh = listFjh.Find(delegate(LG_FJH fj)
                      {
                        return fj.c_fjno.Trim().Equals(field2.Faktur, StringComparison.OrdinalIgnoreCase);
                      });

                      if (fjh == null)
                      {
                        nAvailble = -1;

                        break;
                      }

                      nSisa = (fjh.n_sisa.HasValue ? fjh.n_sisa.Value : 0);

                      if (nSisa <= 0)
                      {
                        nAvailble = -1;

                        break;
                      }

                      nAvailble = (allocMoney > nSisa ? nSisa : allocMoney);
                      nAvailble = (nAvailble > jml ? jml : nAvailble);

                      listCnd.Add(new LG_CND()
                      {
                        c_fjno = fjh.c_fjno,
                        c_noteno = noteID,
                        c_type = "02",
                        c_vcno = fjrh.c_fjno,
                        n_value = nAvailble
                      });

                      totalDetails++;

                      //nSisa = (jml > nAvailble ? nAvailble : jml);

                      //nAvailble -= nSisa;

                      //fjh.n_sisa -= nSisa;

                      //field2.Jumlah -= nSisa;

                      fjh.n_sisa -= nAvailble;

                      fjrh.n_sisa -= nAvailble;

                      field2.Jumlah -= nAvailble;

                      allocMoney -= nAvailble;

                      if (allocMoney <= 0)
                      {
                        break;
                      }
                    }

                    #region Old Coded

                    //if (nAvailble == -1)
                    //{
                    //  allocMoney = -1;

                    //  break;
                    //}
                    //else
                    //{
                    //  fjrh.n_sisa -= allocMoney;

                    //  allocMoney = 0;
                    //}

                    #endregion
                  }
                }
                else
                {
                  allocMoney = -1;

                  break;
                }
              }
            }

            #endregion

            if (allocMoney == 0)
            {
              if (listCnd.Count > 0)
              {
                db.LG_CNDs.InsertAllOnSubmit(listCnd.ToArray());
              }

              listCnd.Clear();

              hasAnyChanges = true;
            }
            else
            {
              result = "Nilai voucher tidak balance.";

              rpe = ResponseParser.ResponseParserEnum.IsFailed;

              if (db.Transaction != null)
              {
                db.Transaction.Rollback();
              }

              goto endLogic;
            }

            #region Clear Lists

            if (listFaktur != null)
            {
              listFaktur.Clear();
            }
            if (listFjNo != null)
            {
              listFjNo.Clear();
            }
            if (listFjh != null)
            {
              listFjh.Clear();
            }
            if (listRetur != null)
            {
              listRetur.Clear();
            }
            if (listFjrNo != null)
            {
              listFjrNo.Clear();
            }
            if (listFjrh != null)
            {
              listFjrh.Clear();
            }

            #endregion
          }

          #endregion

          if (hasAnyChanges)
          {
            dic = new Dictionary<string, string>();

            dic.Add("Note", noteID);
            dic.Add("Voucher", vcID);
            dic.Add("Tanggal", structure.Fields.TanggalDebitDate.ToString("yyyyMMdd"));

            result = string.Format("Total {0} detail(s)", totalDetails);
          }

          #endregion
        }
        else if (structure.Method.Equals("Delete", StringComparison.OrdinalIgnoreCase))
        {
          #region Delete

          if (string.IsNullOrEmpty(noteID))
          {
            result = "Nomor Note dibutuhkan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }

          cnh = (from q in db.LG_CNHs
                 where q.c_noteno == noteID
                 select q).Take(1).SingleOrDefault();

          if (cnh == null)
          {
            result = "Nomor Note tidak ditemukan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }
          else if (cnh.l_delete.HasValue && cnh.l_delete.Value)
          {
            result = "Tidak dapat menghapus nomor Note yang sudah terhapus.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }
          else if ((!cnh.d_notedate.HasValue) || cnh.d_notedate.Value.Equals(DateTime.MinValue) || cnh.d_notedate.Value.Equals(DateTime.MaxValue))
          {
            result = "Tanggal transaksi tidak terbaca.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }
          else if (Commons.IsClosingFA(db, cnh.d_notedate.Value))
          {
            result = "Pembayaran tidak dapat dihapus, karena tanggal pada transaksi ini sudah closing.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;

            db.Transaction.Rollback();

            goto endLogic;
          }

          cnh.c_update = nipEntry;
          cnh.d_update = DateTime.Now;

          cnh.l_delete = true;
          cnh.v_ket_mark = structure.Fields.Keterangan;

          vcID = (cnh.c_vcno ?? "");

          listCnd2 = new List<LG_CND2>();

          listCnd = (from q in db.LG_CNDs
                     where q.c_noteno == noteID
                     select q).ToList();

          isVoucherNote = (string.IsNullOrEmpty(cnh.c_type) || cnh.c_type.Equals("01") ? true : false);

          if ((listCnd != null) && (listCnd.Count > 0))
          {
            listFjNo = listCnd.Where(x => x.c_type == "01").GroupBy(y => y.c_fjno).Select(z => z.Key.Trim()).ToList();
            if (listFjNo.Count > 0)
            {
              listFjh = (from q in db.LG_FJHs
                         where listFjNo.Contains(q.c_fjno)
                         select q).ToList();

              listFjNo.Clear();
            }

            listFjrNo = listCnd.Where(x => x.c_type == "02").GroupBy(y => y.c_fjno).Select(z => z.Key.Trim()).ToList();
            if (listFjrNo.Count > 0)
            {
              listFjrh = (from q in db.LG_FJRHs
                          where listFjrNo.Contains(q.c_fjno)
                          select q).ToList();

              listFjrNo.Clear();
            }

            #region Check & Move Detail Debit dan Faktur

            jml = allocMoney = 0;
            for (nLoopC = 0; nLoopC < listCnd.Count; nLoopC++)
            {
              cnd = listCnd[nLoopC];

              if (cnd.c_type.Equals("01"))
              {
                #region Faktur

                fjh = listFjh.Find(delegate(LG_FJH fj)
                {
                  return fj.c_fjno.Trim().Equals(cnd.c_fjno.Trim());
                });

                jml = (cnd.n_value.HasValue ? cnd.n_value.Value : 0);
                allocMoney += jml;

                if (fjh != null)
                {
                  nAvailble = (fjh.n_net.HasValue ? fjh.n_net.Value : 0);
                  nSisa = (fjh.n_sisa.HasValue ? fjh.n_sisa.Value : 0);

                  jml = ((nSisa + jml) > nAvailble ? (nAvailble - nSisa) : jml);

                  fjh.n_sisa += jml;
                }

                #endregion
              }
              else
              {
                #region Voucher Retur

                fjrh = listFjrh.Find(delegate(LG_FJRH fjr)
                {
                  return fjr.c_fjno.Trim().Equals(cnd.c_vcno.Trim());
                });

                jml = (cnd.n_value.HasValue ? cnd.n_value.Value : 0);
                //allocMoney += jml;

                if (fjrh != null)
                {
                  nAvailble = (fjrh.n_net.HasValue ? fjrh.n_net.Value : 0);
                  nSisa = (fjrh.n_sisa.HasValue ? fjrh.n_sisa.Value : 0);

                  jml = ((nSisa + jml) > nAvailble ? (nAvailble - nSisa) : jml);

                  fjrh.n_sisa += jml;
                }

                #endregion
              }

              listCnd2.Add(new LG_CND2()
              {
                c_entry = nipEntry,
                c_fjno = cnd.c_fjno,
                c_noteno = noteID,
                c_type = cnd.c_type,
                c_vcno = cnd.c_vcno,
                d_entry = date,
                n_value = cnd.n_value,
                v_ket_del = structure.Fields.Keterangan,
                v_type = "03"
              });

              totalDetails++;
            }

            if (listCnd2.Count > 0)
            {
              db.LG_CND2s.InsertAllOnSubmit(listCnd2.ToArray());

              listCnd2.Clear();
            }

            db.LG_CNDs.DeleteAllOnSubmit(listCnd.ToArray());

            listCnd.Clear();

            #endregion

            cnh.n_bilva -= allocMoney;
          }

          #region Check Voucher

          if (isVoucherNote)
          {
            vcn = (from q in db.LG_VCNHs
                   where q.c_vcno == vcID
                   select q).Take(1).SingleOrDefault();

            if (vcn != null)
            {
              vcn.n_sisa += allocMoney;

              vcn.l_delete = true;
              vcn.v_ket_mark = structure.Fields.Keterangan;
            }
          }

          #endregion

          hasAnyChanges = true;

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

          rpe = ResponseParser.ResponseParserEnum.IsFailed;
        }
      }
      catch (Exception ex)
      {
        if (db.Transaction != null)
        {
          db.Transaction.Rollback();
        }
        rpe = ResponseParser.ResponseParserEnum.IsError;

        result = string.Format("ScmsSoaLibrary.Bussiness.PembayaranVoucher:VoucherKredit - {0}", ex.Message);

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
