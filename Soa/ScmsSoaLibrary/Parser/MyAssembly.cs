using System;
using System.Collections.Generic;
using System.Text;
using ScmsSoaLibrary.Bussiness;
using System.Reflection;
using ScmsSoaLibrary.Commons;
using System.Globalization;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaLibrary.Parser
{
  class MyAssembly
  {
    public struct ReportStructureGenerator
    {
      public bool isAsync;
      public string data;
      public string Result;
    }

    public static string sResult = null;

    public static bool isSync = false;

    private static void GenerateProcessingAsync(object state)
    {
      Auto pjln = new Auto();

      ReportStructureGenerator Rg = (ReportStructureGenerator)state;

      MyAssemblyParser(Rg.data);
    }

    public static string MyAssemblyParser(string data)
    {
      Parser parser = new ScmsSoaLibrary.Parser.Parser();

      parser.Populate(data);

      string result = null;

      try
      {
        #region Switch
        

        if ((parser.DataParser == null) || string.IsNullOrEmpty(parser.DataParser.Class))
        {
          result = ScmsSoaLibrary.Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Null object");
        }
        else
        {
          switch (parser.DataParser.Class)
          {
            #region Right Management

            case Constant.CLASS_NAME_RIGHTMANGEMENT_USER:

              if (parser.IsPopulated)
              {
                RightManagement rm = new RightManagement();

                result = rm.UserManagement(parser.XmlParser, parser.DataParser);
              }
              break;

            case Constant.CLASS_NAME_RIGHTMANGEMENT_GROUP:

              if (parser.IsPopulated)
              {
                RightManagement rm = new RightManagement();

                result = rm.GroupManagement(parser.XmlParser, parser.DataParser);
              }
              break;

            case Constant.CLASS_NAME_RIGHTMANGEMENT_USERGROUPACCESS:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  RightManagement rm = new RightManagement();

                  ScmsSoaLibrary.Parser.Class.UserGroupAccessStructure strt = ScmsSoaLibrary.Parser.Class.UserGroupAccessStructure.Serialize(data);

                  result = rm.UserGroupManagement(strt);
                }
              }
              break;

            case Constant.CLASS_NAME_RIGHTMANGEMENT_GROUPUSERACCESS:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  RightManagement rm = new RightManagement();

                  ScmsSoaLibrary.Parser.Class.UserGroupAccessStructure strt = ScmsSoaLibrary.Parser.Class.UserGroupAccessStructure.Serialize(data);

                  result = rm.GroupUserManagement(strt);
                }
              }
              break;

            #endregion

            #region Packing List

            case Constant.CLASS_NAME_PACKINGLIST:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  Penjualan pjln = new Penjualan();

                  ScmsSoaLibrary.Parser.Class.PackingListStructure strt = ScmsSoaLibrary.Parser.Class.PackingListStructure.Serialize(data);

                  result = pjln.PackingList(strt);
                }
              }
              break;
            #endregion

            #region Packing List Auto

            case Constant.CLASS_NAME_PACKINGLIST_AUTO:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  Penjualan pjln = new Penjualan();

                  ScmsSoaLibrary.Parser.Class.PackingListAutoStructure strt = ScmsSoaLibrary.Parser.Class.PackingListAutoStructure.Serialize(data);

                  result = pjln.PackingListAuto(strt);
                }
              }
              break;
            #endregion

            #region Packing List Master Box

            case Constant.CLASS_NAME_PACKINGLIST_MASTERBOX:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  Penjualan pjln = new Penjualan();

                  ScmsSoaLibrary.Parser.Class.PackingListStructure strt = ScmsSoaLibrary.Parser.Class.PackingListStructure.Serialize(data);

                  result = pjln.PackingListMasterBox(strt);
                }
              }
              break;
            #endregion

            #region Packing List Auto Generator

            case Constant.CLASS_NAME_PACKINGLIST_AUTOGENERATOR:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  //isSync = bool.Parse(string.IsNullOrEmpty(parser.DataParser.CustomMethod.Trim()) ? "False" : parser.DataParser.CustomMethod.Trim());

                  //Auto pjln = new Auto();

                  //ScmsSoaLibrary.Parser.Class.PackingListStructure strt = ScmsSoaLibrary.Parser.Class.PackingListStructure.Serialize(data);

                  //sResult = pjln.PackingList(strt, isSync);

                  //isSync = bool.Parse(string.IsNullOrEmpty(parser.DataParser.CustomMethod.Trim()) ? "False" : parser.DataParser.CustomMethod.Trim());

                  Auto pjln = new Auto();

                  ScmsSoaLibrary.Parser.Class.PackingListStructure strt = ScmsSoaLibrary.Parser.Class.PackingListStructure.Serialize(data);

                  result = pjln.PackingList(strt, isSync);

                }
              }
              break;
            #endregion

            //case Constant.CLASS_NAME_PROCESSSTOCKOPNAME:
            //  {
            //      if (parser.IsPopulated)
            //      {
            //          // Damn..
            //      }
            //      else
            //      {
            //          Proses SO = new Proses();

            //          ScmsSoaLibrary.Parser.Class.StockOpnameStructure strt = ScmsSoaLibrary.Parser.Class.StockOpnameStructure.Serialize(data);

            //          result = SO.ProcessStokOpname(strt);
            //      }
            //  }
            //  break;

            #region StockOpname Indra 20171231FM

            #region Insert Hitung

            case Constant.CLASS_NAME_STOCKOPNAME:
              {
                  if (parser.IsPopulated)
                  {
                      // Damn..
                  }
                  else
                  {
                      Proses SO = new Proses();

                      ScmsSoaLibrary.Parser.Class.StockOpnameStructure strt = ScmsSoaLibrary.Parser.Class.StockOpnameStructure.Serialize(data);

                      result = SO.StokOpname(strt);
                  }
              }
              break;

            #endregion

            #region Buat Form

            case Constant.CLASS_NAME_STOCKOPNAME_BUATFORMSO:
              {
                  if (parser.IsPopulated)
                  {
                      // Damn..
                  }
                  else
                  {
                      Proses SO = new Proses();

                      ScmsSoaLibrary.Parser.Class.StockOpnameStructure strt = ScmsSoaLibrary.Parser.Class.StockOpnameStructure.Serialize(data);

                      result = SO.StokOpnameBUATFORMSO(strt);
                  }
              }
              break;

            #endregion

            #region Batal Form

            case Constant.CLASS_NAME_STOCKOPNAME_FORMSOBATAL:
              {
                  if (parser.IsPopulated)
                  {
                      // Damn..
                  }
                  else
                  {
                      Proses SO = new Proses();

                      ScmsSoaLibrary.Parser.Class.StockOpnameStructure strt = ScmsSoaLibrary.Parser.Class.StockOpnameStructure.Serialize(data);

                      result = SO.StokOpnameFORMSOBATAL(strt);
                  }
              }
              break;

            #endregion

            #region Confirm Form

            case Constant.CLASS_NAME_STOCKOPNAME_CONFIRMSO:
              {
                  if (parser.IsPopulated)
                  {
                      // Damn..
                  }
                  else
                  {
                      Proses SO = new Proses();

                      ScmsSoaLibrary.Parser.Class.StockOpnameStructure strt = ScmsSoaLibrary.Parser.Class.StockOpnameStructure.Serialize(data);

                      result = SO.StokOpnameCONFIRMSO(strt);
                  }
              }
              break;

            #endregion

            #region New Batch

            case Constant.CLASS_NAME_STOCKOPNAME_NEWBATCH:
              {
                  if (parser.IsPopulated)
                  {
                      // Damn..
                  }
                  else
                  {
                      Proses SO = new Proses();

                      ScmsSoaLibrary.Parser.Class.StockOpnameStructure strt = ScmsSoaLibrary.Parser.Class.StockOpnameStructure.Serialize(data);

                      result = SO.StokOpnameNEWBATCH(strt);
                  }
              }
              break;

            #endregion

            #region Adjustment SO

            case Constant.CLASS_NAME_STOCKOPNAME_ADJUST:
              {
                  if (parser.IsPopulated)
                  {
                      // Damn..
                  }
                  else
                  {
                      Proses SO = new Proses();

                      ScmsSoaLibrary.Parser.Class.StockOpnameStructure strt = ScmsSoaLibrary.Parser.Class.StockOpnameStructure.Serialize(data);

                      result = SO.StokOpnameAdjust(strt);
                  }
              }
              break;

            #endregion

            #region SO Ulang

            case Constant.CLASS_NAME_STOCKOPNAME_SOULANG:
              {
                  if (parser.IsPopulated)
                  {
                      // Damn..
                  }
                  else
                  {
                      Proses SO = new Proses();

                      ScmsSoaLibrary.Parser.Class.StockOpnameStructure strt = ScmsSoaLibrary.Parser.Class.StockOpnameStructure.Serialize(data);

                      result = SO.StokOpnameSOUlang(strt);
                  }
              }
              break;

            #endregion

            #endregion

            #region Proses Email Produk Kosong 20190411

            case Constant.CLASS_NAME_PROSESEMAIL_PRODUKKOSONG:
              {
                  if (parser.IsPopulated)
                  {
                      // Damn..
                  }
                  else
                  {
                      Proses PKo = new Proses();

                      ScmsSoaLibrary.Parser.Class.ProdukKosongStructure strt = ScmsSoaLibrary.Parser.Class.ProdukKosongStructure.Serialize(data);

                      result = PKo.ProdukKosong(strt);

                  }
              }
              break;

            #endregion

            //case Constant.CLASS_NAME_ADJSTOCKOPNAME:
            //  {
            //      if (parser.IsPopulated)
            //      {
            //          // Damn..
            //      }
            //      else
            //      {
            //          Proses SO = new Proses();

            //          ScmsSoaLibrary.Parser.Class.StockOpnameStructure strt = ScmsSoaLibrary.Parser.Class.StockOpnameStructure.Serialize(data);

            //          result = SO.AdjStokOpname(strt);
            //      }
            //  }
            //  break;

            #region Surat Pesanan

            case Constant.CLASS_NAME_SURATPESANAN:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  Pembelian pbln = new Pembelian();

                  ScmsSoaLibrary.Parser.Class.SuratPesananStructure strt = ScmsSoaLibrary.Parser.Class.SuratPesananStructure.Serialize(data);

                  result = pbln.SuratPesanan(strt);
                }
              }
              break;

            #endregion

            #region Surat Pesanan Manual

            case Constant.CLASS_NAME_SURATPESANANMANUAL:
              {
                  if (parser.IsPopulated)
                  {
                      // Damn..
                  }
                  else
                  {
                      Pembelian pbln = new Pembelian();

                      ScmsSoaLibrary.Parser.Class.SuratPesananStructure strt = ScmsSoaLibrary.Parser.Class.SuratPesananStructure.Serialize(data);

                      //result = pbln.SuratPesananManual(strt);
                  }
              }
              break;

            #endregion

            #region Surat Pesanan Admin

            case Constant.CLASS_NAME_SURATPESANAN_ADMIN:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  ScmsSoaLibrary.Parser.Class.SuratPesananStructure strt = ScmsSoaLibrary.Parser.Class.SuratPesananStructure.Serialize(data);

                  if (strt == null)
                  {
                    throw new Exception("Failed to cast structure object.");
                  }

                  //Pembelian pbln = new Pembelian();

                  //result = pbln.SuratPesananAdmin(strt);

                  System.Threading.ThreadPool.QueueUserWorkItem(ScmsSoaLibrary.Core.Threading.Running.RunningThreadSPAdmin, strt);

                  result = ScmsSoaLibrary.Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsSuccess, null, string.Format("Success running thread 'RunningThreadSPAdmin'", parser.DataParser.Class));
                }
              }
              break;

            #endregion

            #region Order Request

            case Constant.CLASS_NAME_ORDERREQUESTPRINCIPAL:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  Pembelian pbln = new Pembelian();

                  ScmsSoaLibrary.Parser.Class.OrderRequestStructure strt = ScmsSoaLibrary.Parser.Class.OrderRequestStructure.Serialize(data);

                  result = pbln.OrderRequest(strt);
                }
              }
              break;

            case Constant.CLASS_NAME_ORDERREQUESTPROCESSPRINCIPAL:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  Pembelian pbln = new Pembelian();

                  ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructure strt = ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructure.Serialize(data);

                  result = pbln.OrderRequestProcess(strt);
                }
              }
              break;

            case Constant.CLASS_NAME_ORDERREQUESTGUDANG:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  Pembelian pbln = new Pembelian();

                  ScmsSoaLibrary.Parser.Class.OrderRequestGudangStructure strt = ScmsSoaLibrary.Parser.Class.OrderRequestGudangStructure.Serialize(data);

                  result = pbln.OrderRequestGudang(strt);
                }
              }
              break;

            case Constant.CLASS_NAME_ORDERREQUESTPROCESSGUDANG:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  Pembelian pbln = new Pembelian();

                  ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructure strt = ScmsSoaLibrary.Parser.Class.OrderRequestProcessStructure.Serialize(data);

                  result = pbln.OrderRequestProcessGudang(strt);
                }
              }
              break;

            #endregion

            #region DO

            #region DO PL

            case Constant.CLASS_NAME_DOPL:
              {
                Penjualan pjln = new Penjualan();

                ScmsSoaLibrary.Parser.Class.DoPLStructure strt = ScmsSoaLibrary.Parser.Class.DoPLStructure.Serialize(data);

                result = pjln.DOPackingList(strt);
              }
              break;

            #endregion

            #region DO STT

            case Constant.CLASS_NAME_DOSTT:
              {
                Penjualan pjln = new Penjualan();

                ScmsSoaLibrary.Parser.Class.DOSTTStructure strt = ScmsSoaLibrary.Parser.Class.DOSTTStructure.Serialize(data);

                result = pjln.DOSTT(strt);
              }
              break;

            #endregion

            #region DO SEND

            case Constant.CLASS_NAME_DOSEND:
              {
                  Penjualan pjln = new Penjualan();

                  ScmsSoaLibrary.Parser.Class.DoPLSendStructure strt = ScmsSoaLibrary.Parser.Class.DoPLSendStructure.Serialize(data);

                  result = pjln.DOSEND(strt);
              }
              break;

            #endregion

            #endregion

            #region CLASS_NAME_Ekspedisi

            case Constant.CLASS_NAME_Ekspedisi:
              {
                Penjualan pjln = new Penjualan();

                ScmsSoaLibrary.Parser.Class.ExpedisiStructure strt = ScmsSoaLibrary.Parser.Class.ExpedisiStructure.Serialize(data);

                result = pjln.EkspedisiDO(strt);
              }
              break;

            case Constant.CLASS_NAME_Ekspedisi_Cabang:
              {
                Penjualan pjln = new Penjualan();

                ScmsSoaLibrary.Parser.Class.ExpedisiCabangStructure strt = ScmsSoaLibrary.Parser.Class.ExpedisiCabangStructure.Serialize(data);

                result = pjln.EkspedisiCabang(strt);
              }
              break;

            //case Constant.CLASS_NAME_Ekspedisi_Cabang_Proses:
            //  {
            //    Penjualan pjln = new Penjualan();

            //    ScmsSoaLibrary.Parser.Class.ExpedisiCabangStructure strt = ScmsSoaLibrary.Parser.Class.ExpedisiCabangStructure.Serialize(data);

            //    result = pjln.EkspedisiCabang(strt);
            //  }
            //  break;

            #endregion

            #region CLASS_NAME_Transfer

            case Constant.CLASS_NAME_Transfer:
              {
                Transfer trans = new Transfer();

                ScmsSoaLibrary.Parser.Class.TranStructure strt = ScmsSoaLibrary.Parser.Class.TranStructure.Serialize(data);

                result = trans.TransferGudang(strt);
              }
              break;

            case Constant.CLASS_NAME_Transfer_Repack:
              {
                Transfer trans = new Transfer();

                ScmsSoaLibrary.Parser.Class.TranStructure strt = ScmsSoaLibrary.Parser.Class.TranStructure.Serialize(data);

                result = trans.TransferGudangRepack(strt);
              }
              break;

            #endregion

            #region CLASS_NAME_STT

            case Constant.CLASS_NAME_STT:
              {
                Peminjaman pinjam = new Peminjaman();

                ScmsSoaLibrary.Parser.Class.STTStructure strt = ScmsSoaLibrary.Parser.Class.STTStructure.Serialize(data);

                result = pinjam.STT(strt);
              }
              break;

            #endregion

            #region Pemusnahan

            case Constant.CLASS_NAME_PEMUSNAHAN:
              {
                  Pemusnahan musnah = new Pemusnahan();

                  ScmsSoaLibrary.Parser.Class.PemusnahanStructure strt = ScmsSoaLibrary.Parser.Class.PemusnahanStructure.Serialize(data);

                  result = musnah.ProsesPemusnahan(strt);
              }
              break;

            #endregion

            #region Purchase Order

            case Constant.CLASS_NAME_PURCHASE_ORDER:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  Pembelian pbln = new Pembelian();

                  ScmsSoaLibrary.Parser.Class.PurchaseOrderStructure strt = ScmsSoaLibrary.Parser.Class.PurchaseOrderStructure.Serialize(data);

                  result = pbln.PurchaseOrder(strt);
                }
              }
              break;

            #endregion

            #region Purchase Order Apoteker

            case Constant.CLASS_NAME_PURCHASE_ORDER_APOTEKER:
              {
                  if (parser.IsPopulated)
                  {
                      // Damn..
                  }
                  else
                  {
                      Pembelian pbln = new Pembelian();

                      ScmsSoaLibrary.Parser.Class.PurchaseOrderStructure strt = ScmsSoaLibrary.Parser.Class.PurchaseOrderStructure.Serialize(data);

                      result = pbln.PurchaseOrderApoteker(strt);
                  }
              }
              break;

            #endregion

            #region Receive Note

            #region Pembelian

            case Constant.CLASS_NAME_RN_BELI:
            case Constant.CLASS_NAME_RN_KHUSUS:
            case Constant.CLASS_NAME_RN_RETUR:
            case Constant.CLASS_NAME_RN_CLAIM:
            case Constant.CLASS_NAME_RN_REPACK:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  Pembelian pbln = new Pembelian();

                  ScmsSoaLibrary.Parser.Class.ReceiveNoteStructure strt = ScmsSoaLibrary.Parser.Class.ReceiveNoteStructure.Serialize(data);

                  result = pbln.ReceiveNote(strt);
                }
              }
              break;

            #endregion

            #region Transfer Gudang

            case Constant.CLASS_NAME_RN_TRANSFERGUDANG:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  Transfer trnf = new Transfer();

                  ScmsSoaLibrary.Parser.Class.ReceiveNoteGudangStructure strt = ScmsSoaLibrary.Parser.Class.ReceiveNoteGudangStructure.Serialize(data);

                  result = trnf.ReceiveNoteGudang(strt);
                }
              }
              break;

            #endregion

            #endregion

            #region MK

            #region Memo Combo

            case Constant.CLASS_NAME_MK_MEMO_COMBO:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  MK mk = new MK();

                  ScmsSoaLibrary.Parser.Class.MKMemoComboStructure strt = ScmsSoaLibrary.Parser.Class.MKMemoComboStructure.Serialize(data);

                  result = mk.MemoCombo(strt);
                }
              }
              break;

            #endregion

            #region Memo Donasi

            case Constant.CLASS_NAME_MK_MEMO_DONASI:
              {
              }
              break;

            #endregion

            #region Memo Sample

            case Constant.CLASS_NAME_MK_MEMO_SAMPLE:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  MK mk = new MK();

                  ScmsSoaLibrary.Parser.Class.MKMemoSTTStructure strt = ScmsSoaLibrary.Parser.Class.MKMemoSTTStructure.Serialize(data);

                  result = mk.MemoSTT(strt);
                }
              }
              break;

            #endregion

            #region Memo Pemusnahan

            case Constant.CLASS_NAME_MK_MEMO_PEMUSNAHAN:
              {
                  if (parser.IsPopulated)
                  {
                      // Damn..
                  }
                  else
                  {
                      MK mk = new MK();

                      ScmsSoaLibrary.Parser.Class.MKMemoPemusnahanStructure strt = ScmsSoaLibrary.Parser.Class.MKMemoPemusnahanStructure.Serialize(data);

                      result = mk.MemoPemusnahan(strt);
                  }
              }
              break;

            #endregion

            #region Memo BASPB SJ

            case Constant.CLASS_NAME_MK_MEMO_BASPB_SJ:
              {
                  if (parser.IsPopulated)
                  {
                      // Damn..
                  }
                  else
                  {
                      MK mk = new MK();

                      ScmsSoaLibrary.Parser.Class.MKMemoBASPBSJStructure strt = ScmsSoaLibrary.Parser.Class.MKMemoBASPBSJStructure.Serialize(data);

                      result = mk.MemoBASPBSJ(strt);
                  }
              }
              break;

            #endregion

            #endregion

            #region Memo

            #region Combo

            case Constant.CLASS_NAME_LG_MEMO_COMBO:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  Memo memo = new Memo();

                  ScmsSoaLibrary.Parser.Class.MemoComboStructure strt = ScmsSoaLibrary.Parser.Class.MemoComboStructure.Serialize(data);

                  result = memo.MemoCombo(strt);
                }
              }
              break;

            #endregion

            #endregion

            #region Adjustment

            #region Adjustment Stock

            case Constant.CLASS_NAME_ADJ_STOCK_GOODBAD:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  AdjustStockGoodBad adjGoodBad = new AdjustStockGoodBad();

                  ScmsSoaLibrary.Parser.Class.AdjustStockStructure strt = ScmsSoaLibrary.Parser.Class.AdjustStockStructure.Serialize(data);

                  result = adjGoodBad.AdjustGoodBad(strt);

                }
              }
              break;

            case Constant.CLASS_NAME_ADJ_STOCK_BATCH:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  AdjustStockGoodBad adjGoodBad = new AdjustStockGoodBad();

                  ScmsSoaLibrary.Parser.Class.AdjustStockStructure strt = ScmsSoaLibrary.Parser.Class.AdjustStockStructure.Serialize(data);

                  result = adjGoodBad.AdjustGoodBad(strt);

                }
              }
              break;

            case Constant.CLASS_NAME_ADJ_STOCK_STOCK:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  AdjustStockGoodBad adjStock = new AdjustStockGoodBad();

                  ScmsSoaLibrary.Parser.Class.AdjustStockStructure strt = ScmsSoaLibrary.Parser.Class.AdjustStockStructure.Serialize(data);

                  result = adjStock.AdjustGoodBad(strt);

                }
              }
              break;

            #endregion

            #region Adjustment Transaksi

            case Constant.CLASS_NAME_ADJ_STOCK_TRANS:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {

                  AdjustTransaksi adjTrans = new AdjustTransaksi();

                  ScmsSoaLibrary.Parser.Class.AdjustTransStructure strt = ScmsSoaLibrary.Parser.Class.AdjustTransStructure.Serialize(data);

                  result = adjTrans.AdjustTrans(strt);

                }
              }
              break;

            #endregion

            #region Adjustment Transaksi FJ

            case Constant.CLASS_NAME_ADJ_FJ:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {

                  AdjustFaktur adjFak = new AdjustFaktur();

                  ScmsSoaLibrary.Parser.Class.AdjustFakturStructure strt = ScmsSoaLibrary.Parser.Class.AdjustFakturStructure.Serialize(data);

                  result = adjFak.AdjustFak(strt);

                }
              }
              break;

            #endregion

            #region Adjustment Transaksi FB

            case Constant.CLASS_NAME_ADJ_FB:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {

                  AdjustFaktur adjFak = new AdjustFaktur();

                  ScmsSoaLibrary.Parser.Class.AdjustFakturStructure strt = ScmsSoaLibrary.Parser.Class.AdjustFakturStructure.Serialize(data);

                  result = adjFak.AdjustFakBeli(strt);

                }
              }
              break;

            #endregion

            #region Adjustment Transaksi STT

            case Constant.CLASS_NAME_ADJ_STT_DONASI:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {

                  AdjustTransaksi adjTrans = new AdjustTransaksi();

                  ScmsSoaLibrary.Parser.Class.AdjustTransStructure strt = ScmsSoaLibrary.Parser.Class.AdjustTransStructure.Serialize(data);

                  result = adjTrans.AdjustTransSTT(strt);

                }
              }
              break;

            #endregion

            #region Adjustment Transaksi Voucher

            case Constant.CLASS_NAME_ADJ_VOUCHER:
              {
                if (parser.IsPopulated)
                {
                  // Damn..
                }
                else
                {
                  AdjustVoucher adjVoucher = new AdjustVoucher();

                  ScmsSoaLibrary.Parser.Class.AdjustFakturStructure strt = ScmsSoaLibrary.Parser.Class.AdjustFakturStructure.Serialize(data);

                  result = adjVoucher.AdjVoucher(strt);

                }
              }
              break;

            #endregion

            #endregion

            #region RC

            case Constant.CLASS_NAME_RC:
              {
                ReturPusat ret = new ReturPusat();

                ScmsSoaLibrary.Parser.Class.ReturCustomerStructure rcstr = ScmsSoaLibrary.Parser.Class.ReturCustomerStructure.Serialize(data);

                result = ret.ReturCustomer(rcstr);
              }
              break;

            case Constant.CLASS_NAME_RCIN:
              {
                ReturPusat ret = new ReturPusat();

                ScmsSoaLibrary.Parser.Class.ReturCustomerStructureIn rcstr = ScmsSoaLibrary.Parser.Class.ReturCustomerStructureIn.Serialize(data);

                result = ret.ReturCustomerIn(rcstr);
              }
              break;

            case Constant.CLASS_NAME_PROSESST:
              {
                  ReturPusat ret = new ReturPusat();

                  ScmsSoaLibrary.Parser.Class.ReturCustomerStructureIn rcstr = ScmsSoaLibrary.Parser.Class.ReturCustomerStructureIn.Serialize(data);

                  result = ret.ProsesST(rcstr);
              }
              break;

            case Constant.CLASS_NAME_SAVE_MOVEMENT_STOCK:
              {
                  ReturPusat ret = new ReturPusat();

                  ScmsSoaLibrary.Parser.Class.MovementStockStructure msstr = ScmsSoaLibrary.Parser.Class.MovementStockStructure.Serialize(data);

                  result = ret.UpdateST(msstr);

              }
              break;

            #endregion

            #region AutoRS

              case Constant.CLASS_NAME_RCRS:
              {
                  ReturPusat ret = new ReturPusat();

                  ScmsSoaLibrary.Parser.Class.ReturCustomerStructureIn rcstr = ScmsSoaLibrary.Parser.Class.ReturCustomerStructureIn.Serialize(data);

                  result = ret.ReturRS(rcstr);
              }
              break;
            
            #endregion

            #region RS

            #region RS Pembelian

            case Constant.CLASS_NAME_RS_PEMBELIAN:
            case Constant.CLASS_NAME_RS_REPACK:
            case Constant.CLASS_NAME_RSDISPOSISI:
              {
                ReturPusat retur = new ReturPusat();
                //ReturSuplier rsbeli = new ReturSuplier();

                ScmsSoaLibrary.Parser.Class.ReturSupplierStructure rsBeliStruc = ScmsSoaLibrary.Parser.Class.ReturSupplierStructure.Serialize(data);

                result = retur.ReturSuplier(rsBeliStruc);
              }
              break;

            #endregion

            #region RS Conf

            case Constant.CLASS_NAME_RS_CONF:
              {
                ReturPusat retur = new ReturPusat();

                ScmsSoaLibrary.Parser.Class.ReturSupplierConfStructure rsConfStruc = ScmsSoaLibrary.Parser.Class.ReturSupplierConfStructure.Serialize(data);

                result = retur.ReturSuplierConfirm(rsConfStruc);
              }
              break;

            #endregion

            #endregion

            #region Claim

            case Constant.CLASS_NAME_CLAIM:
              {
                ClaimBonusSupplier claim = new ClaimBonusSupplier();

                ScmsSoaLibrary.Parser.Class.ClaimStructure ClaimBonusStruc = ScmsSoaLibrary.Parser.Class.ClaimStructure.Serialize(data);

                result = claim.ClaimBonus(ClaimBonusStruc);
              }
              break;

            case Constant.CLASS_NAME_CLAIM_PROCCESS:
              {
                ClaimBonusSupplier claim = new ClaimBonusSupplier();

                ScmsSoaLibrary.Parser.Class.ClaimStructureProcess ClaimBonusStruc = ScmsSoaLibrary.Parser.Class.ClaimStructureProcess.Serialize(data);

                result = claim.ClaimBonusProcess(ClaimBonusStruc);
              }
              break;

            #endregion

            #region Claim Acc

            case Constant.CLASS_NAME_CLAIM_ACC:
              {
                ClaimBonusAccSupplier claimAcc = new ClaimBonusAccSupplier();

                ScmsSoaLibrary.Parser.Class.ClaimAccStructure ClaimBonusAccStruc = ScmsSoaLibrary.Parser.Class.ClaimAccStructure.Serialize(data);

                result = claimAcc.ClaimBonusAcc(ClaimBonusAccStruc);
              }
              break;

            #endregion

            #region Pembayaran

            #region CLASS_NAME_VOUCHER_DEBIT

            case Constant.CLASS_NAME_VOUCHER_DEBIT:
              {
                PembayaranVoucher vch = new PembayaranVoucher();

                ScmsSoaLibrary.Parser.Class.PembayaranVchStructure VchStruc = ScmsSoaLibrary.Parser.Class.PembayaranVchStructure.Serialize(data);

                result = vch.VoucherDebit(VchStruc);
              }
              break;

            #endregion

            #region CLASS_NAME_VOUCHER_CREDIT

            case Constant.CLASS_NAME_VOUCHER_CREDIT:
              {
                PembayaranVoucher vch = new PembayaranVoucher();

                ScmsSoaLibrary.Parser.Class.PembayaranVchStructure VchStruc = ScmsSoaLibrary.Parser.Class.PembayaranVchStructure.Serialize(data);

                result = vch.VoucherKredit(VchStruc);
              }
              break;

            #endregion

            #endregion

            #region Auto

            #region CLASS_AUTO_DOPRINSIPAL_MANUAL

            case Constant.CLASS_AUTO_DOPRINSIPAL_MANUAL:
              {
                Auto autoX = new Auto();

                ScmsSoaLibrary.Parser.Class.DOPrinsipalStructure dops = ScmsSoaLibrary.Parser.Class.DOPrinsipalStructure.Serialize(data);

                result = autoX.DOPrinsipal(dops);
              }
              break;

            #endregion

            #region CLASS_AUTO_DO_PHARMANET

            case Constant.CLASS_AUTO_DO_PHARMANET:
              {
                  Pembelian pbln = new Pembelian();
                  ScmsSoaLibrary.Parser.Class.DOPharmanetStructure strt = ScmsSoaLibrary.Parser.Class.DOPharmanetStructure.Serialize(data);

                  result = pbln.ProcessPharmanet(strt);
              }
              break;

            #endregion

            #region CLASS_SAVE_BATCH

            case Constant.CLASS_SAVE_BATCH:
              {
                  Pembelian pbln = new Pembelian();
                  ScmsSoaLibrary.Parser.Class.DOPharmanetStructure strt = ScmsSoaLibrary.Parser.Class.DOPharmanetStructure.Serialize(data);

                  result = pbln.SaveBatch(strt);
              }
              break;

            #endregion

            #region CLASS_VERIFIKASI_PHARMANET

            case Constant.CLASS_VERIFIKASI_PHARMANET:
              {
                  Pembelian pbln = new Pembelian();
                  ScmsSoaLibrary.Parser.Class.DOPharmanetStructure strt = ScmsSoaLibrary.Parser.Class.DOPharmanetStructure.Serialize(data);

                  result = pbln.VerifikasiPharmanet(strt);
              }
              break;
            #endregion

            #region CLASS_CANCEL_PHARMANET

              case Constant.CLASS_CANCEL_PHARMANET:
                  {

                      Pembelian pbln = new Pembelian();
                      ScmsSoaLibrary.Parser.Class.DOPharmanetStructure strt = ScmsSoaLibrary.Parser.Class.DOPharmanetStructure.Serialize(data);

                      result = pbln.CancelPharmanet(strt);

                  }
                  break;

            #endregion

            #endregion

            #region Purchase Order Limit Item

            case Constant.CLASS_NAME_PURCHASE_ORDER_LIMIT_ITEM:
              {
                  if (parser.IsPopulated)
                  {
                      // Damn..
                  }
                  else
                  {
                      Pembelian pbln = new Pembelian();

                      ScmsSoaLibrary.Parser.Class.LimitPOItemStructure strt = ScmsSoaLibrary.Parser.Class.LimitPOItemStructure.Serialize(data);

                      result = pbln.LimitPOItem(strt);
                  }
              }
              break;

            #endregion

            #region Purchase Order Limit Divisi Principal

            case Constant.CLASS_NAME_PURCHASE_ORDER_LIMIT_DIVPRI:
              {
                  if (parser.IsPopulated)
                  {
                      // Damn..
                  }
                  else
                  {
                      Pembelian pbln = new Pembelian();

                      ScmsSoaLibrary.Parser.Class.LimitPOItemStructure strt = ScmsSoaLibrary.Parser.Class.LimitPOItemStructure.Serialize(data);

                      result = pbln.LimitPODivPri(strt);
                  }
              }
              break;

            #endregion

            #region Purchase Order Limit Principal

            case Constant.CLASS_NAME_PURCHASE_ORDER_LIMIT_PRINCIPAL:
              {
                  if (parser.IsPopulated)
                  {
                      // Damn..
                  }
                  else
                  {
                      Pembelian pbln = new Pembelian();

                      ScmsSoaLibrary.Parser.Class.LimitPOItemStructure strt = ScmsSoaLibrary.Parser.Class.LimitPOItemStructure.Serialize(data);

                      result = pbln.LimitPOPrincipal(strt);
                  }
              }
              break;

            #endregion


            #region Faktur

            #region CLASS_NAME_FAKTUR_JUAL

            case Constant.CLASS_NAME_FAKTUR_JUAL:
              {
                Faktur faktur = new Faktur();

                ScmsSoaLibrary.Parser.Class.FakturJualStructure fakturJual = ScmsSoaLibrary.Parser.Class.FakturJualStructure.Serialize(data);

                result = faktur.FakturJual(fakturJual);
              }
              break;

            #endregion

            #region CLASS_NAME_FAKTUR_BELI

            case Constant.CLASS_NAME_FAKTUR_BELI:
              {
                Faktur faktur = new Faktur();

                ScmsSoaLibrary.Parser.Class.FakturBeliStructure fakturBeli = ScmsSoaLibrary.Parser.Class.FakturBeliStructure.Serialize(data);

                result = faktur.FakturBeli(fakturBeli);
              }
              break;

            #endregion

            #region CLASS_NAME_FAKTUR_JUAL_RETUR

            case Constant.CLASS_NAME_FAKTUR_JUAL_RETUR:
              {
                Faktur faktur = new Faktur();

                ScmsSoaLibrary.Parser.Class.FakturJualReturStructure fakturJualRetur = ScmsSoaLibrary.Parser.Class.FakturJualReturStructure.Serialize(data);

                result = faktur.FakturJualRetur(fakturJualRetur);
              }
              break;

            #endregion

            #region CLASS_NAME_FAKTUR_BELI_RETUR

            case Constant.CLASS_NAME_FAKTUR_BELI_RETUR:
              {
                Faktur faktur = new Faktur();

                ScmsSoaLibrary.Parser.Class.FakturBeliReturStructure fakturBeliRetur = ScmsSoaLibrary.Parser.Class.FakturBeliReturStructure.Serialize(data);

                result = faktur.FakturBeliRetur(fakturBeliRetur);
              }
              break;

            #endregion

            #region CLASS_NAME_Invoice_Ekspedisi_Eksternal
            case Constant.CLASS_NAME_Invoice_Ekspedisi_Eksternal:
              {
                  Penjualan pjln = new Penjualan();

                  ScmsSoaLibrary.Parser.Class.InvoiceEkspedisiEksternalStructure invoiceEkspedisiEksternal = ScmsSoaLibrary.Parser.Class.InvoiceEkspedisiEksternalStructure.Serialize(data);

                  result = pjln.InvoiceEkspedisiEksternal(invoiceEkspedisiEksternal);
              }
              break;
            #endregion

            #region CLASS_NAME_Invoice_Ekspedisi_Internal
            case Constant.CLASS_NAME_Invoice_Ekspedisi_Internal:
              {
                  Penjualan pjln = new Penjualan();

                  ScmsSoaLibrary.Parser.Class.InvoiceEkspedisiInternalStructure invoiceEkspedisiInternal = ScmsSoaLibrary.Parser.Class.InvoiceEkspedisiInternalStructure.Serialize(data);

                  result = pjln.InvoiceEkspedisiInternal(invoiceEkspedisiInternal);
              }
              break;
            #endregion

            #region CLASS_NAME_Faktur_Ekspedisi
            case Constant.CLASS_NAME_Faktur_Ekspedisi:
              {
                  Penjualan pjln = new Penjualan();

                  ScmsSoaLibrary.Parser.Class.FakturEkspedisiStructure fakturEkspedisi = ScmsSoaLibrary.Parser.Class.FakturEkspedisiStructure.Serialize(data);

                  result = pjln.FakturEkspedisi(fakturEkspedisi);
              }
              break;
            #endregion

            #region CLASS_NAME_Return_DO
            case Constant.CLASS_NAME_Return_DO:
              {
                  Penjualan pjln = new Penjualan();

                  ScmsSoaLibrary.Parser.Class.ReturnDOStructure returnDO = ScmsSoaLibrary.Parser.Class.ReturnDOStructure.Serialize(data);

                  result = pjln.ReturnDO(returnDO);
              }
              break;
            #endregion

            #region CLASS_NAME_FAKTUR_MANUAL

            case Constant.CLASS_NAME_FAKTUR_MANUAL:
              {
                  Faktur faktur = new Faktur();

                  ScmsSoaLibrary.Parser.Class.FakturManualStructure fakturManual = ScmsSoaLibrary.Parser.Class.FakturManualStructure.Serialize(data);

                  result = faktur.FakturManual(fakturManual);
              }
              break;

            #endregion

            #endregion

            #region Waktu Pelayanan

            #region Waktu Pelayanan Logistik
            case Constant.CLASS_NAME_WAKTU_PELAYANAN:
              {
                  WaktuPelayanan wp = new WaktuPelayanan();

                  ScmsSoaLibrary.Parser.Class.WaktuPelayananStructure wpLog = ScmsSoaLibrary.Parser.Class.WaktuPelayananStructure.Serialize(data);

                  result = wp.WaktuPelayananLogistik(wpLog);
              }
              break;

            #endregion

            #region Serah Terima
            case Constant.CLASS_NAME_SERAH_TERIMA:
              {
                  WaktuPelayanan wp = new WaktuPelayanan();

                  ScmsSoaLibrary.Parser.Class.WaktuPelayananStructure wpLog = ScmsSoaLibrary.Parser.Class.WaktuPelayananStructure.Serialize(data);

                  result = wp.SerahTerima(wpLog);
              }
              break;

            #endregion

            #region Serah Terima Tiket
            case Constant.CLASS_NAME_SERAHTERIMA_TIKET:
              {
                  WaktuPelayanan wp = new WaktuPelayanan();

                  ScmsSoaLibrary.Parser.Class.WaktuPelayananStructure wpLog = ScmsSoaLibrary.Parser.Class.WaktuPelayananStructure.Serialize(data);

                  result = wp.SerahTerimaTiket(wpLog);
              }
              break;

            #endregion

            #region Serah Terima Tiket PO
            case Constant.CLASS_NAME_SERAHTERIMA_TIKETPO:
              {
                  WaktuPelayanan wp = new WaktuPelayanan();

                  ScmsSoaLibrary.Parser.Class.WaktuPelayananStructure wpLog = ScmsSoaLibrary.Parser.Class.WaktuPelayananStructure.Serialize(data);

                  result = wp.SerahTerimaTiketPO(wpLog);
              }
              break;

            #endregion
            
            #endregion

            #region Master

            #region CLASS_NAME_MASTER_ITEM

            case Constant.CLASS_NAME_MASTER_ITEM:
              {
                Master ms = new Master();

                ScmsSoaLibrary.Parser.Class.MasterItemStructure masStruc = ScmsSoaLibrary.Parser.Class.MasterItemStructure.Serialize(data);

                result = ms.MasterItemBisnis(masStruc);
              }
              break;

            #endregion

            #region CLASS_NAME_MASTER_DISCOUNT

            case Constant.CLASS_NAME_MASTER_DISCOUNT:
              {
                Master mast = new Master();

                ScmsSoaLibrary.Parser.Class.DiscountStructure discStruct = ScmsSoaLibrary.Parser.Class.DiscountStructure.Serialize(data);

                result = mast.Discount(discStruct);
              }
              break;

            #endregion

            #region CLASS_NAME_MASTER_BATCH

            case Constant.CLASS_NAME_MASTER_BATCH:
              {
                Master mast = new Master();

                ScmsSoaLibrary.Parser.Class.BatchItemStructure batchStruct = ScmsSoaLibrary.Parser.Class.BatchItemStructure.Serialize(data);

                result = mast.MasterBatch(batchStruct);
              }
              break;

            #endregion

            #region CLASS_NAME_MASTER_BUDGET

            case Constant.CLASS_NAME_MASTER_BUDGET:
              {
                Master mast = new Master();

                ScmsSoaLibrary.Parser.Class.BudgetLimitStructure budgetStruct = ScmsSoaLibrary.Parser.Class.BudgetLimitStructure.Serialize(data);

                result = mast.BudgetLimit(budgetStruct);
              }
              break;

            #endregion

            #region CLASS_NAME_MASTER_GUDANG

            case Constant.CLASS_NAME_MASTER_GUDANG:
              {
                if (parser.IsPopulated)
                {
                  Master mast = new Master();

                  result = mast.MasterGudang(parser.XmlParser, parser.DataParser);
                }
              }
              break;

            #endregion

            #region CLASS_NAME_MASTER_KURS

            case Constant.CLASS_NAME_MASTER_KURS:
              {
                if (parser.IsPopulated)
                {
                  Master mast = new Master();

                  result = mast.MasterKurs(parser.XmlParser, parser.DataParser);
                }
              }
              break;

            #endregion

            #region Master Prinsipal

            case Constant.CLASS_NAME_MASTER_PRINSIPAL:
              {
                Master mast = new Master();

                ScmsSoaLibrary.Parser.Class.MasterPrinsipalStructure PrinsStruck = ScmsSoaLibrary.Parser.Class.MasterPrinsipalStructure.Serialize(data);

                result = mast.Prinsipal(PrinsStruck);
              }
              break;

            #endregion

            #region Master Divisi Prinsipal

            case Constant.CLASS_NAME_MASTER_DIVISI_PRINSIPAL:
              {
                Master mast = new Master();

                ScmsSoaLibrary.Parser.Class.MasterDivisiPrinsipalStructure PrinsStruck = ScmsSoaLibrary.Parser.Class.MasterDivisiPrinsipalStructure.Serialize(data);

                result = mast.DivisiPrinsipal(PrinsStruck);
              }
              break;

            #endregion

            #region Master Divisi Prinsipal Item

            case Constant.CLASS_NAME_MASTER_DIVISI_PRINSIPAL_ITEM:
              {
                Master mast = new Master();

                ScmsSoaLibrary.Parser.Class.MasterDivPrinsipalItemStructure PrinsStruck = ScmsSoaLibrary.Parser.Class.MasterDivPrinsipalItemStructure.Serialize(data);

                result = mast.DivisiPrinsipalItem(PrinsStruck);
              }
              break;

            #endregion

            #region Master Customer

            case Constant.CLASS_NAME_MASTER_PELANGGAN:
              {
                Master mast = new Master();

                ScmsSoaLibrary.Parser.Class.MasterCustomerStructure PrinsStruck = ScmsSoaLibrary.Parser.Class.MasterCustomerStructure.Serialize(data);

                result = mast.Customer(PrinsStruck);
              }
              break;

            #endregion

            #region Approve Reject Master Pelanggan

            case Constant.CLASS_NAME_APPROVE_REJECT_MASTER_PELANGGAN:
              {
                  Master mast = new Master();

                  ScmsSoaLibrary.Parser.Class.MasterApprovalStructure MS = ScmsSoaLibrary.Parser.Class.MasterApprovalStructure.Serialize(data);

                  result = mast.ApproveRejectMasterCustomer(MS);
              }
              break;
            #endregion

            #region Master Expedisi

            case Constant.CLASS_NAME_MASTER_EXPEDISI:
              {
                Master mast = new Master();

                ScmsSoaLibrary.Parser.Class.MasterExpedisiStructure ExpedisiStruc = ScmsSoaLibrary.Parser.Class.MasterExpedisiStructure.Serialize(data);

                result = mast.Expedisi(ExpedisiStruc);
              }
              break;

            #endregion
                  //
            #region Master Cabang Hari

            case Constant.CLASS_NAME_MASTER_CABANG_HARI:
              {
                  Master mast = new Master();

                  ScmsSoaLibrary.Parser.Class.MasterCabangHariStructure ExpedisiStruc = ScmsSoaLibrary.Parser.Class.MasterCabangHariStructure.Serialize(data);

                  result = mast.CabangHari(ExpedisiStruc);
              }
              break;

            #endregion

            #region Master Expedisi Estimasi

            case Constant.CLASS_NAME_MASTER_EXPEDISI_ESTIMASI:
              {
                  Master mast = new Master();

                  ScmsSoaLibrary.Parser.Class.MasterExpedisiEstimasiStructure EstExpedisiStruc = ScmsSoaLibrary.Parser.Class.MasterExpedisiEstimasiStructure.Serialize(data);

              }
              break;

            #endregion

            #region Master Expedisi Biaya Estimasi

            case Constant.CLASS_NAME_MASTER_EXPEDISI_BIAYA_ESTIMASI:
              {
                  Master mast = new Master();

                  ScmsSoaLibrary.Parser.Class.MasterExpedisiEstimasiBiayaStructure EstExpedisiBiayaStruc = ScmsSoaLibrary.Parser.Class.MasterExpedisiEstimasiBiayaStructure.Serialize(data);

                  result = mast.ExpedisiBiaya(EstExpedisiBiayaStruc);
              }
              break;

            #endregion

            #region Master Block Item

            case Constant.CLASS_NAME_MASTER_BLOCK_ITEM:
              {
                Master mast = new Master();

                ScmsSoaLibrary.Parser.Class.MasterBlockItemStructure MstBlokItem = ScmsSoaLibrary.Parser.Class.MasterBlockItemStructure.Serialize(data);

                result = mast.BlockItem(MstBlokItem);
              }
              break;

            #endregion

            #region Master Combo

            case Constant.CLASS_NAME_MASTER_COMBO:
              {
                Master mast = new Master();

                ScmsSoaLibrary.Parser.Class.MasterComboStructure MstCombo = ScmsSoaLibrary.Parser.Class.MasterComboStructure.Serialize(data);

                result = mast.Combo(MstCombo);
              }
              break;

            #endregion

            #region Master Transaksi

            case Constant.CLASS_NAME_MASTER_TRANSAKSI:
              {
                Master mast = new Master();

                ScmsSoaLibrary.Parser.Class.MasterTransaksiStructure MstTrx = ScmsSoaLibrary.Parser.Class.MasterTransaksiStructure.Serialize(data);

                result = mast.MasterTransaksi(MstTrx);
              }
              break;

            #endregion

            #region Master Item Category

            case Constant.CLASS_NAME_MASTER_ITEM_CATEGORY:
              {
                Master mast = new Master();

                ScmsSoaLibrary.Parser.Class.MasterItemCategoryStructure MstCat = ScmsSoaLibrary.Parser.Class.MasterItemCategoryStructure.Serialize(data);

                result = mast.MasterItemCategory(MstCat);
              }
              break;

            #endregion

            #region Master Item Lantai

            case Constant.CLASS_NAME_MASTER_ITEM_LANTAI:
              {
                  Master mast = new Master();

                  ScmsSoaLibrary.Parser.Class.MasterItemLantaiStructure MstLat = ScmsSoaLibrary.Parser.Class.MasterItemLantaiStructure.Serialize(data);

                  result = mast.MasterItemLantai(MstLat);
              }
              break;

            #endregion

            #region Master Div AMS

            case Constant.CLASS_NAME_MASTER_DIV_AMS:
              {
                Master mast = new Master();

                ScmsSoaLibrary.Parser.Class.MasterDivAMSItemStructure msterDivAMS = ScmsSoaLibrary.Parser.Class.MasterDivAMSItemStructure.Serialize(data);

                result = mast.DivisiAMSItem(msterDivAMS);
              }
              break;

            #endregion

            #region Master User APJ

            case Constant.CLASS_NAME_MASTER_USER_APJ:
              {
                  Master mast = new Master();

                  ScmsSoaLibrary.Parser.Class.MasterUserApjStructure msterUserApj = ScmsSoaLibrary.Parser.Class.MasterUserApjStructure.Serialize(data);

                  result = mast.MasterUserApj(msterUserApj);
              }
              break;

            #endregion

            #region Master Bank

            case Constant.CLASS_NAME_MASTER_BANK:
              {
                Master mast = new Master();

                ScmsSoaLibrary.Parser.Class.MasterBankStructure bank = ScmsSoaLibrary.Parser.Class.MasterBankStructure.Serialize(data);

                result = mast.Bank(bank);
              }
              break;

            #endregion

            #region Master Item VIA

            case Constant.CLASS_NAME_MASTER_ITEM_VIA:
              {
                  Master mast = new Master();

                  ScmsSoaLibrary.Parser.Class.MasterItemViaStructure MstLat = ScmsSoaLibrary.Parser.Class.MasterItemViaStructure.Serialize(data);

                  result = mast.MasterItemVia(MstLat);
              }
              break;

            #endregion

            #region Master Driver

            case Constant.CLASS_NAME_MASTER_DRIVER:
              {
                Master mast = new Master();

                ScmsSoaLibrary.Parser.Class.MasterDriverStructure MstDriver = ScmsSoaLibrary.Parser.Class.MasterDriverStructure.Serialize(data);

                result = mast.MasterDriver(MstDriver);
              }
              break;

            #endregion

            #region Master PKP

            case Constant.CLASS_NAME_MASTER_PKP:
              {
                  Master mast = new Master();

                  ScmsSoaLibrary.Parser.Class.MasterPKPStructure PKPStruck = ScmsSoaLibrary.Parser.Class.MasterPKPStructure.Serialize(data);

                  result = mast.MasterPKP(PKPStruck);
              }
              break;

            #endregion

            #region Master Nomor Pajak

            case Constant.CLASS_NAME_MASTER_NOMOR_PAJAK:
              {
                  Master mast = new Master();

                  ScmsSoaLibrary.Parser.Class.MasterNomorPajakStructure MstNoPajak = ScmsSoaLibrary.Parser.Class.MasterNomorPajakStructure.Serialize(data);

                  result = mast.MasterNomorPajak(MstNoPajak);
              }
              break;

            #endregion

            #endregion

            #region Syncronize

            #region RN Cabang

            case Constant.CLASS_NAME_RN_CABANG:
              {
                SinkronData sync = new SinkronData();

                ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructure syncStruct = ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructure.Serialize(data);

                result = sync.OrderCustomerReceived(syncStruct);
              }
              break;

            #endregion

            #region RS Cabang

            case Constant.CLASS_NAME_RS_CABANG:
              {
                SinkronData sync = new SinkronData();

                ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructure syncStruct = ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructure.Serialize(data);

                result = sync.OrderCustomerReceived(syncStruct);
              }
              break;

            #endregion

            #region Master ITem

            case Constant.CLASS_NAME_MASTER_ITEM_CABANG:
              {
                SinkronData ms = new SinkronData();

                ScmsSoaLibrary.Parser.Class.MasterItemReceiveStructure masStruc = ScmsSoaLibrary.Parser.Class.MasterItemReceiveStructure.Serialize(data);

                result = ms.MasterItemReceived(masStruc);
              }
              break;

            #endregion

            #region Surat Pesanan Cabang

            case Constant.CLASS_NAME_SURAT_PESANAN_CABANG:
              {
                SinkronData ms = new SinkronData();

                ScmsSoaLibrary.Parser.Class.SuratPesananReceiveStructure masStruc = ScmsSoaLibrary.Parser.Class.SuratPesananReceiveStructure.Serialize(data);

                result = ms.SuratPesananReceived(masStruc);
              }
              break;

            #endregion

            #region Recall 
            case Constant.CLASS_NAME_RECALL: //by suwandi 20 September 2018
              {
                  SinkronData ms = new SinkronData();

                  ScmsSoaLibrary.Parser.Class.RecallReceiveStructure restruc = ScmsSoaLibrary.Parser.Class.RecallReceiveStructure.Serialize(data);

                  result = ms.RecallBarang(restruc);
              }
              break;
            #endregion

            #region Relokasi
            case Constant.CLASS_NAME_RELOKASI: //by suwandi 16 November 2018
              {
                  SinkronData ms = new SinkronData();

                  ScmsSoaLibrary.Parser.Class.RelokasiStructure relocstruc = ScmsSoaLibrary.Parser.Class.RelokasiStructure.Serialize(data);

                  result = ms.Relokasi(relocstruc);

              }
              break;
            #endregion

            #region Terima Relokasi

            case Constant.CLASS_NAME_TERIMA_RELOKASI:
              {
                  SinkronData ms = new SinkronData();

                  ScmsSoaLibrary.Parser.Class.ReceiveRelokasiStructure receiverelocstruc = ScmsSoaLibrary.Parser.Class.ReceiveRelokasiStructure.Serialize(data);

                  result = ms.ReceiveRelokasi(receiverelocstruc);

              }
              break;

            #endregion

            #region Cancel PBB Relokasi

            case Constant.CLASS_NAME_CANCEL_PB_RELOKASI:
              {
                  SinkronData ms = new SinkronData();
                  
                  ScmsSoaLibrary.Parser.Class.CancelPBStructure cancelpbstruc = ScmsSoaLibrary.Parser.Class.CancelPBStructure.Serialize(data);

                  result = ms.CancelPB(cancelpbstruc);

              }
              break;

            #endregion

            #region Receive PO

              case Constant.CLASS_NAME_RECEIVE_PO:
                  {
                      SinkronData ms = new SinkronData();

                      ScmsSoaLibrary.Parser.Class.ReceivePOStructure postruc = ScmsSoaLibrary.Parser.Class.ReceivePOStructure.Serialize(data);

                      result = ms.ReceivePO(postruc);
                  }
                  break;

            #endregion

            case Constant.CLASS_NAME_CANCEL_SP_RELOKASI:
              {
                  SinkronData ms = new SinkronData();

                  ScmsSoaLibrary.Parser.Class.CancelSPStructure cancelspstruc = ScmsSoaLibrary.Parser.Class.CancelSPStructure.Serialize(data);

                  result = ms.CancelSP(cancelspstruc);
              }
              break;

            case Constant.CLASS_NAME_MASTER_RELOKASI:
              {
                  SinkronData ms = new SinkronData();

                  ScmsSoaLibrary.Parser.Class.MasterRelokasiStructure msrelostruc = ScmsSoaLibrary.Parser.Class.MasterRelokasiStructure.Serialize(data);

                  result = ms.MasterRelokasi(msrelostruc);
              }
              break;

            #endregion

            #region Reporting

            case Constant.CLASS_NAME_REPORTING:
              {
                Reporting rpt = new Reporting();

                result = rpt.ReportingManagement(parser.XmlParser, parser.DataParser);
              }
              break;

            #endregion

            default:

              result = ScmsSoaLibrary.Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.Unknown, null, string.Format("Unknown class name '{0}'", parser.DataParser.Class));

              break;
          }
        }

        #endregion
        
      }
      catch (Exception ex)
      {
        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);

        result = ScmsSoaLibrary.Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null,
          string.Concat(ex.Message, "\r\n", ex.StackTrace));
      }

      if (isSync)
      {
        result = sResult;
      }
      
      return result;
    }

    public static string MyAssemblyParser(string data, bool isSync)
    {
      Parser parser = new ScmsSoaLibrary.Parser.Parser();

      parser.Populate(data);

      ReportStructureGenerator Rg = new ReportStructureGenerator();
      Rg.data = data;
      Rg.isAsync = isSync;

      bool isProcessing = false;

      isProcessing = System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(GenerateProcessingAsync), Rg);

      return sResult;
    }

    public string ErrorMessage { get; private set; }

    public T Populate<T>(ScmsSoaLibrary.Parser.Parser.StructureXmlHeaderParser xmlParser, ScmsSoaLibrary.Parser.Parser.StructureDataNamingHeader dataParser) where T : class
    {
      if (((xmlParser == null) || (!xmlParser.IsSet) || (xmlParser.ListRelated == null) || (xmlParser.ListRelated.Count < 1)) ||
        ((dataParser == null) || (!dataParser.IsSet) || (dataParser.List == null) || (dataParser.List.Count < 1)))
      {
        return default(T);
      }

      T cls = default(T);

      Type typ = typeof(T);

      Assembly asm = typ.Module.Assembly;

      if (asm == null)
      {
        return default(T);
      }

      CultureInfo ci = new CultureInfo("id-ID");

      Parser.StructureDataInputDetail si = null;
      Parser.StructureXmlDetailParser sdp = null;
      Parser.StructureXmlDetailParser[] sdpArr = null;

      PropertyInfo pi = null;
      DateTime date = DateTime.MinValue;
      int num = 0,
        nLoop = 0, nLen = 0,
        nLoopC = 0, nLenC = 0;
      byte byt = 0;
      decimal dec = 0;
      bool bol = false;

      try
      {
        cls = (T)asm.CreateInstance(typeof(T).FullName);

        if (cls != null)
        {
          for (nLoop = 0, nLen = dataParser.List.Count; nLoop < nLen; nLoop++)
          {
            si = dataParser.List[nLoop];

            if ((!si.IsList) && si.IsSet)
            {
              sdpArr = this.SeekName(si.Name, xmlParser);
              if ((sdpArr != null) && (sdpArr.Length > 0))
              {
                if (sdpArr.Length == 1)
                {
                  sdp = sdpArr[0];

                  if ((sdp != null) && sdp.IsSet)
                  {
                    pi = typ.GetProperty(sdp.Property);

                    if (pi != null)
                    {
                      #region Prepare to write

                      if (pi.CanWrite)
                      {
                        if (sdp.Type.Equals(typeof(DateTime)))
                        {
                          if (DateTime.TryParseExact(si.Value, "yyyyMMddHHmmssfff", ci, DateTimeStyles.AssumeLocal, out date))
                          {
                            pi.SetValue(cls, date, null);
                          }
                        }
                        else if (sdp.Type.Equals(typeof(bool)))
                        {
                          if (bool.TryParse(si.Value, out bol))
                          {
                            pi.SetValue(cls, bol, null);
                          }
                          else
                          {
                            if (!string.IsNullOrEmpty(si.Value))
                            {
                              bol = (si.Value.Equals("1", StringComparison.OrdinalIgnoreCase) ? true : false);
                              pi.SetValue(cls, bol, null);

                            }
                          }
                        }
                        else if (sdp.Type.Equals(typeof(int)))
                        {
                          if (int.TryParse(si.Value, NumberStyles.AllowThousands | NumberStyles.AllowTrailingSign | NumberStyles.AllowTrailingWhite, ci, out num))
                          {
                            pi.SetValue(cls, num, null);
                          }
                        }
                        else if (sdp.Type.Equals(typeof(byte)))
                        {
                          if (byte.TryParse(si.Value, NumberStyles.AllowThousands | NumberStyles.AllowTrailingSign | NumberStyles.AllowTrailingWhite, ci, out byt))
                          {
                            pi.SetValue(cls, byt, null);
                          }
                        }
                        else if (sdp.Type.Equals(typeof(decimal)))
                        {
                          if (decimal.TryParse(si.Value, NumberStyles.Any, ci, out dec))
                          {
                            pi.SetValue(cls, dec, null);
                          }
                        }
                        else if (sdp.Type.Equals(typeof(char)))
                        {
                          if (!string.IsNullOrEmpty(si.Value))
                          {
                            pi.SetValue(cls, si.Value[0], null);
                          }
                        }
                        else
                        {
                          pi.SetValue(cls, si.Value, null);
                        }
                      }

                      #endregion
                    }
                  }
                }
                else
                {
                  for (nLoopC = 0, nLenC = sdpArr.Length; nLoopC < nLenC; nLoopC++)
                  {
                    sdp = sdpArr[nLoopC];

                    if ((sdp != null) && sdp.IsSet)
                    {
                      pi = typ.GetProperty(sdp.Property);

                      if (pi != null)
                      {
                        #region Prepare to write

                        if (pi.CanWrite)
                        {
                          if (sdp.Type.Equals(typeof(DateTime)))
                          {
                            if (DateTime.TryParseExact(si.Value, "yyyyMMddHHmmssfff", ci, DateTimeStyles.AssumeLocal, out date))
                            {
                              pi.SetValue(cls, date, null);
                            }
                          }
                          else if (sdp.Type.Equals(typeof(bool)))
                          {
                            if (bool.TryParse(si.Value, out bol))
                            {
                              pi.SetValue(cls, bol, null);
                            }
                            else
                            {
                              if (!string.IsNullOrEmpty(si.Value))
                              {
                                bol = (si.Value.Equals("1", StringComparison.OrdinalIgnoreCase) ? true : false);
                                pi.SetValue(cls, bol, null);

                              }
                            }
                          }
                          else if (sdp.Type.Equals(typeof(int)))
                          {
                            if (int.TryParse(si.Value, NumberStyles.AllowHexSpecifier | NumberStyles.AllowThousands | NumberStyles.AllowTrailingSign | NumberStyles.AllowTrailingWhite, ci, out num))
                            {
                              pi.SetValue(cls, num, null);
                            }
                          }
                          else if (sdp.Type.Equals(typeof(decimal)))
                          {
                            if (decimal.TryParse(si.Value, NumberStyles.Any, ci, out dec))
                            {
                              pi.SetValue(cls, num, null);
                            }
                          }
                          else
                          {
                            pi.SetValue(cls, si.Value, null);
                          }
                        }

                        #endregion
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine("ScmsSoaLibrary.Parser.MyAssembly:Populate<T> - {0}", ex.Message);
        Logger.WriteLine("ScmsSoaLibrary.Parser.MyAssembly:Populate<T> - Details - {0}", ex.StackTrace);
      }

      return cls;
    }

    public string VerifyParser(ScmsSoaLibrary.Parser.Parser.StructureXmlHeaderParser xmlParser, ScmsSoaLibrary.Parser.Parser.StructureDataNamingHeader dataParser)
    {
      if (((xmlParser == null) || (!xmlParser.IsSet) || (xmlParser.ListRelated == null) || (xmlParser.ListRelated.Count < 1)) ||
       ((dataParser == null) || (!dataParser.IsSet) || (dataParser.List == null) || (dataParser.List.Count < 1)))
      {
        return "Data parser atau xml parser tidak terbaca.";
      }

      return VerifyParserNow(xmlParser, dataParser);
    }

    private string VerifyParserNow(ScmsSoaLibrary.Parser.Parser.StructureXmlHeaderParser xmlParser, ScmsSoaLibrary.Parser.Parser.StructureDataNamingHeader dataParser)
    {
      StringBuilder sbResult = new StringBuilder();

      Parser.StructureXmlDetailParser xParse = null;
      Parser.StructureDataInputDetail[] dParseDtl = null;

      for (int nLoop = 0, nLenXml = xmlParser.ListRelated.Count, nLenData = dataParser.List.Count; nLoop < nLenXml; nLoop++)
      {
        xParse = xmlParser.ListRelated[nLoop];
        if (!string.IsNullOrEmpty(xParse.Name))
        {
          if (xParse.IsList)
          {
            System.Diagnostics.Debug.Assert(true, "Tunggu yak");
          }
          else
          {
            dParseDtl = SeekNameData(xParse.Name, dataParser);
            if ((xParse.IsRequired) && ((dParseDtl == null) || (dParseDtl.Length < 1)))
            {
              sbResult.AppendLine(string.Format("Parsing '{0}' dibutuhkan dalam pengiriman data.", xParse.Name));
            }
          }
        }
      }

      return sbResult.ToString();
    }

    public Parser.StructureXmlDetailParser[] SeekName(string name, ScmsSoaLibrary.Parser.Parser.StructureXmlHeaderParser xmlParser)
    {
      List<Parser.StructureXmlDetailParser> list = new List<Parser.StructureXmlDetailParser>();

      if (string.IsNullOrEmpty(name))
      {
        return list.ToArray();
      }

      Parser.StructureXmlDetailParser sdp = null;

      for (int nLoop = 0, nLen = xmlParser.ListRelated.Count; nLoop < nLen; nLoop++)
      {
        sdp = xmlParser.ListRelated[nLoop];
        if (sdp.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        {
          //break;
          list.Add(sdp);
        }
      }

      return list.ToArray();
    }

    public Parser.StructureDataInputDetail[] SeekNameData(string name, ScmsSoaLibrary.Parser.Parser.StructureDataNamingHeader dataParser)
    {
      List<Parser.StructureDataInputDetail> list = new List<Parser.StructureDataInputDetail>();

      if (string.IsNullOrEmpty(name))
      {
        return list.ToArray();
      }

      Parser.StructureDataInputDetail sdp = null;

      for (int nLoop = 0, nLen = dataParser.List.Count; nLoop < nLen; nLoop++)
      {
        sdp = dataParser.List[nLoop];
        if (sdp.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        {
          //break;
          list.Add(sdp);
        }
      }

      return list.ToArray();
    }
  }
}