using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsModel;
using ScmsSoaLibrary.Parser;
using ScmsSoaLibrary.Commons;
using System.Data.Common;
using ScmsSoaLibrary.Core.Threading;
using ScmsModel.Core;
using ScmsSoaLibraryInterface.Commons;


/*
 * KodeStock 01 = Add, 02 = Delete, 03 = Adjustment, 04 = Memo
 * Status 01 = Good, 02 = Bad
 * Konfirm 01 = Accept, 02 = Hold
 * 
 */

namespace ScmsSoaLibrary.Bussiness
{
    class SavingStock
    {
        public static int DailyStock(ORMDataContext db, string Gudang,
                                                        string Trans,
                                                        string TipeTrans,          
                                                        string KodeItem, 
                                                        string Batch,                                      
                                                        decimal QtyGood,
                                                        decimal QtyBad,
                                                        string Lokasi,                                     
                                                        string KodeStock,
                                                        string Konfirm,
                                                        string nipEntry,
                                                        string Confirm)
        {
            
            LG_DAILY_STOCK_v2 daily2 = null;
            LG_MOVEMENT_STOCK_v2 movement2 = null;

            //int isOk = 0;
            int isOk = 1;

            if (KodeStock == "01" || KodeStock == "02") //Regular
            {
                #region Good Stock 01

                if (QtyGood != 0)
                {
                    daily2 = (from q in db.LG_DAILY_STOCK_v2s
                              where q.c_iteno   == KodeItem &&
                                    q.c_batch   == Batch &&
                                    q.c_gdg     == Convert.ToChar(Gudang) &&
                                    q.status    == "01"

                              select q).Take(1).SingleOrDefault();


                    if (daily2 == null)
                    {
                        daily2 = new LG_DAILY_STOCK_v2()
                        {
                            c_gdg       = Convert.ToChar(Gudang),
                            c_iteno     = KodeItem,
                            c_batch     = Batch,
                            n_qty       = QtyGood,
                            status      = "01",
                            c_locno     = "KMZ",
                            d_entry     = DateTime.Now,
                            d_update    = DateTime.Now,
                        };

                        db.LG_DAILY_STOCK_v2s.InsertOnSubmit(daily2);
                    }
                    else
                    {
                        daily2.n_qty    = daily2.n_qty + QtyGood;
                        daily2.d_update = DateTime.Now;
                    }

                    movement2 = (from q in db.LG_MOVEMENT_STOCK_v2s
                                 where q.c_gdg      == Convert.ToChar(Gudang) &&
                                       q.c_trans    == Trans &&
                                       q.c_iteno    == KodeItem &&
                                       q.c_batch    == Batch &&
                                       q.status     == "01" &&
                                       q.kodestock  == KodeStock
                                 select q).Take(1).SingleOrDefault();

                    if (movement2 == null)
                    {
                        movement2 = new LG_MOVEMENT_STOCK_v2()
                        {
                            c_gdg       = Convert.ToChar(Gudang),
                            c_iteno     = KodeItem,
                            c_batch     = Batch,
                            c_trans     = Trans,
                            c_type      = TipeTrans,
                            n_qty       = QtyGood,
                            status      = "01",
                            kodestock   = KodeStock,
                            c_confirm   = Confirm,
                            c_entry     = nipEntry,
                            d_entry     = DateTime.Now,
                            c_update    = nipEntry,
                            d_update    = DateTime.Now,

                        };

                        db.LG_MOVEMENT_STOCK_v2s.InsertOnSubmit(movement2);

                        isOk = 1;
                    }
                    else
                    {
                        movement2.n_qty = movement2.n_qty + QtyGood;

                        isOk = 0;
                        isOk = 1;
                    }

                    
                }

                #endregion

                #region Bad Stock 02

                if (QtyBad != 0)
                {
                    daily2 = (from q in db.LG_DAILY_STOCK_v2s
                              where q.c_iteno   == KodeItem &&
                                    q.c_batch   == Batch &&
                                    q.c_gdg     == Convert.ToChar(Gudang) &&
                                    q.status    == "02"
                              select q).Take(1).SingleOrDefault();


                    if (daily2 == null)
                    {
                        daily2 = new LG_DAILY_STOCK_v2()
                        {
                            c_gdg       = Convert.ToChar(Gudang),
                            c_iteno     = KodeItem,
                            c_batch     = Batch,
                            n_qty       = QtyBad,
                            status      = "02",
                            c_locno     = "KMZ",
                            d_entry     = DateTime.Now,
                            d_update    = DateTime.Now,
                        };

                        db.LG_DAILY_STOCK_v2s.InsertOnSubmit(daily2);
                    }
                    else
                    {
                        daily2.n_qty    = daily2.n_qty + QtyBad;
                        daily2.d_update = DateTime.Now;
                    }

                    movement2 = (from q in db.LG_MOVEMENT_STOCK_v2s
                                 where q.c_gdg      == Convert.ToChar(Gudang) &&
                                       q.c_trans    == Trans &&
                                       q.c_iteno    == KodeItem &&
                                       q.c_batch    == Batch &&
                                       q.status     == "02" &&
                                       q.kodestock  == KodeStock
                                 select q).Take(1).SingleOrDefault();

                    if (movement2 == null)
                    {
                        movement2 = new LG_MOVEMENT_STOCK_v2()
                        {
                            c_gdg       = Convert.ToChar(Gudang),
                            c_iteno     = KodeItem,
                            c_batch     = Batch,
                            c_trans     = Trans,
                            c_type      = TipeTrans,
                            n_qty       = QtyBad,
                            status      = "02",
                            kodestock   = KodeStock,
                            c_confirm   = Confirm,
                            c_entry     = nipEntry,
                            d_entry     = DateTime.Now,
                            c_update    = nipEntry,
                            d_update    = DateTime.Now,
                        };

                        db.LG_MOVEMENT_STOCK_v2s.InsertOnSubmit(movement2);

                        isOk = 1;
                    }
                    else
                    {
                        movement2.n_qty = movement2.n_qty + QtyBad;
                        isOk = 0;
                    }
                }

                #endregion
            }

            if (KodeStock == "03") //Adjustment
            {
                #region Good Stock 01

                if (QtyGood != 0)
                {
                    daily2 = (from q in db.LG_DAILY_STOCK_v2s
                              where q.c_iteno   == KodeItem &&
                                    q.c_batch   == Batch &&
                                    q.c_gdg     == Convert.ToChar(Gudang) &&
                                    q.status    == "01"

                              select q).Take(1).SingleOrDefault();


                    if (daily2 == null)
                    {
                        daily2 = new LG_DAILY_STOCK_v2()
                        {
                            c_gdg       = Convert.ToChar(Gudang),
                            c_iteno     = KodeItem,
                            c_batch     = Batch,
                            n_qty       = QtyGood,
                            status      = "01",
                            c_locno     = "KMZ",
                            d_entry     = DateTime.Now,
                            d_update    = DateTime.Now,
                        };

                        db.LG_DAILY_STOCK_v2s.InsertOnSubmit(daily2);
                    }
                    else
                    {
                        daily2.n_qty    = daily2.n_qty + QtyGood;
                        daily2.d_update = DateTime.Now;
                    }

                    movement2 = (from q in db.LG_MOVEMENT_STOCK_v2s
                                 where q.c_gdg      == Convert.ToChar(Gudang) &&
                                       q.c_trans    == Trans &&
                                       q.c_iteno    == KodeItem &&
                                       q.c_batch    == Batch &&
                                       q.status     == "01" &&
                                       q.kodestock  == "01"
                                 select q).Take(1).SingleOrDefault();

                    if (movement2 == null)
                    {
                        movement2 = new LG_MOVEMENT_STOCK_v2()
                        {
                            c_gdg       = Convert.ToChar(Gudang),
                            c_iteno     = KodeItem,
                            c_batch     = Batch,
                            c_trans     = Trans,
                            c_type      = TipeTrans,
                            n_qty       = QtyGood,
                            status      = "01",
                            kodestock   = "01",
                            c_confirm   = Confirm,
                            c_entry     = nipEntry,
                            d_entry     = DateTime.Now,
                            c_update    = nipEntry,
                            d_update    = DateTime.Now,
                        };

                        db.LG_MOVEMENT_STOCK_v2s.InsertOnSubmit(movement2);

                        isOk = 1;
                    }
                    else
                    {
                        movement2.n_qty = movement2.n_qty + QtyGood;

                        isOk = 0;
                        isOk = 1;
                    }

                    
                }

                #endregion

                #region Bad Stock 02

                if (QtyBad != 0)
                {
                    daily2 = (from q in db.LG_DAILY_STOCK_v2s
                              where q.c_iteno   == KodeItem &&
                                    q.c_batch   == Batch &&
                                    q.c_gdg     == Convert.ToChar(Gudang) &&
                                    q.status    == "02"
                              select q).Take(1).SingleOrDefault();


                    if (daily2 == null)
                    {
                        daily2 = new LG_DAILY_STOCK_v2()
                        {
                            c_gdg       = Convert.ToChar(Gudang),
                            c_iteno     = KodeItem,
                            c_batch     = Batch,
                            n_qty       = QtyBad,
                            status      = "02",
                            c_locno     = "KMZ",
                            d_entry     = DateTime.Now,
                            d_update    = DateTime.Now,
                        };

                        db.LG_DAILY_STOCK_v2s.InsertOnSubmit(daily2);
                    }
                    else
                    {
                        daily2.n_qty    = daily2.n_qty + QtyBad;
                        daily2.d_update = DateTime.Now;
                    }

                    movement2 = (from q in db.LG_MOVEMENT_STOCK_v2s
                                 where q.c_gdg      == Convert.ToChar(Gudang) &&
                                       q.c_trans    == Trans &&
                                       q.c_iteno    == KodeItem &&
                                       q.c_batch    == Batch &&
                                       q.status     == "02" &&
                                       q.kodestock  == "01"
                                 select q).Take(1).SingleOrDefault();

                    if (movement2 == null)
                    {
                        movement2 = new LG_MOVEMENT_STOCK_v2()
                        {
                            c_gdg       = Convert.ToChar(Gudang),
                            c_iteno     = KodeItem,
                            c_batch     = Batch,
                            c_trans     = Trans,
                            c_type      = TipeTrans,
                            n_qty       = QtyBad,
                            status      = "02",
                            kodestock   = "01",
                            c_confirm   = Confirm,
                            c_entry     = nipEntry,
                            d_entry     = DateTime.Now,
                            c_update    = nipEntry,
                            d_update    = DateTime.Now,
                        };

                        db.LG_MOVEMENT_STOCK_v2s.InsertOnSubmit(movement2);

                        isOk = 1;
                    }
                    else
                    {
                        movement2.n_qty = movement2.n_qty + QtyBad;

                        isOk = 0;
                        isOk = 1;
                    }
                }

                #endregion
            }

            if (KodeStock == "04" || KodeStock == "40") //Memo Header
            {
                if (KodeStock == "04")
                {
                    #region Good Stock 01

                    if (QtyGood != 0)
                    {
                        movement2 = (from q in db.LG_MOVEMENT_STOCK_v2s
                                     where q.c_gdg      == Convert.ToChar(Gudang) &&
                                           q.c_trans    == Trans &&
                                           q.c_iteno    == KodeItem &&
                                           q.c_batch    == Batch &&
                                           q.status     == "01" &&
                                           q.kodestock  == Konfirm
                                     select q).Take(1).SingleOrDefault();

                        if (movement2 == null)
                        {
                            movement2 = new LG_MOVEMENT_STOCK_v2()
                            {
                                c_gdg       = Convert.ToChar(Gudang),
                                c_iteno     = KodeItem,
                                c_batch     = Batch,
                                c_trans     = Trans,
                                c_type      = TipeTrans,
                                n_qty       = QtyGood,
                                status      = "01",
                                kodestock   = KodeStock,
                                c_confirm   = Confirm,
                                c_entry     = nipEntry,
                                d_entry     = DateTime.Now,
                                c_update    = nipEntry,
                                d_update    = DateTime.Now,

                            };

                            db.LG_MOVEMENT_STOCK_v2s.InsertOnSubmit(movement2);

                            isOk = 1;
                        }
                        else
                        {
                            movement2.n_qty = movement2.n_qty + QtyGood;

                            isOk = 0;
                            isOk = 1;
                        }

                        
                    }

                    #endregion

                    #region Bad Stock 02

                    if (QtyBad != 0)
                    {
                        movement2 = (from q in db.LG_MOVEMENT_STOCK_v2s
                                     where q.c_gdg      == Convert.ToChar(Gudang) &&
                                           q.c_trans    == Trans &&
                                           q.c_iteno    == KodeItem &&
                                           q.c_batch    == Batch &&
                                           q.status     == "02" &&
                                           q.kodestock  == Konfirm
                                     select q).Take(1).SingleOrDefault();

                        if (movement2 == null)
                        {
                            movement2 = new LG_MOVEMENT_STOCK_v2()
                            {
                                c_gdg       = Convert.ToChar(Gudang),
                                c_iteno     = KodeItem,
                                c_batch     = Batch,
                                c_trans     = Trans,
                                c_type      = TipeTrans,
                                n_qty       = QtyBad,
                                status      = "02",
                                kodestock   = KodeStock,
                                c_confirm   = Confirm,
                                c_entry     = nipEntry,
                                d_entry     = DateTime.Now,
                                c_update    = nipEntry,
                                d_update    = DateTime.Now,
                            };

                            db.LG_MOVEMENT_STOCK_v2s.InsertOnSubmit(movement2);

                            isOk = 1;
                        }
                        else
                        {
                            movement2.n_qty = movement2.n_qty + QtyGood;

                            isOk = 0;
                            isOk = 1;
                        }
                    }

                    #endregion                               
                }
                else
                {   

                    #region Good Stock 01

                    if (QtyGood != 0)
                    {
                        movement2 = (from q in db.LG_MOVEMENT_STOCK_v2s
                                     where q.c_gdg      == Convert.ToChar(Gudang) &&
                                           q.c_trans    == Trans &&
                                           q.c_iteno    == KodeItem &&
                                           q.c_batch    == Batch &&
                                           q.status     == "01" &&
                                           q.kodestock  == KodeStock
                                     select q).Take(1).SingleOrDefault();

                        if (movement2 != null)
                        {
                            movement2.c_confirm = "01";
                            movement2.c_update  = nipEntry;
                            movement2.d_update  = DateTime.Now;

                            isOk = 1;
                        }
                        else
                        {
                            isOk = 0;
                        }

                        
                    }

                    #endregion

                    #region Bad Stock 02

                    if (QtyBad != 0)
                    {
                        movement2 = (from q in db.LG_MOVEMENT_STOCK_v2s
                                     where q.c_gdg      == Convert.ToChar(Gudang) &&
                                           q.c_trans    == Trans &&
                                           q.c_iteno    == KodeItem &&
                                           q.c_batch    == Batch &&
                                           q.status     == "02" &&
                                           q.kodestock  == KodeStock
                                     select q).Take(1).SingleOrDefault();

                        if (movement2 != null)
                        {
                            movement2.c_confirm = "01";
                            movement2.c_update  = nipEntry;
                            movement2.d_update  = DateTime.Now;

                            isOk = 1;
                        }
                        else
                        {
                            isOk = 0;
                        }
                    }

                    #endregion                               
                }

            }

            if (KodeStock == "05") //Memo Detail
            {
                #region Good Stock 01

                if (QtyGood != 0)
                {
                    daily2 = (from q in db.LG_DAILY_STOCK_v2s
                              where q.c_iteno   == KodeItem &&
                                    q.c_batch   == Batch &&
                                    q.c_gdg     == Convert.ToChar(Gudang) &&
                                    q.status    == "01"

                              select q).Take(1).SingleOrDefault();


                    if (daily2 == null)
                    {
                        daily2 = new LG_DAILY_STOCK_v2()
                        {
                            c_gdg       = Convert.ToChar(Gudang),
                            c_iteno     = KodeItem,
                            c_batch     = Batch,
                            n_qty       = QtyGood,
                            status      = "01",
                            c_locno     = "KMZ",
                            d_entry     = DateTime.Now,
                            d_update    = DateTime.Now,
                        };

                        db.LG_DAILY_STOCK_v2s.InsertOnSubmit(daily2);
                    }
                    else
                    {
                        daily2.n_qty    = daily2.n_qty + QtyGood;
                        daily2.d_update = DateTime.Now;
                    }
                    
                }

                #endregion

                #region Bad Stock 02

                if (QtyBad != 0)
                {
                    daily2 = (from q in db.LG_DAILY_STOCK_v2s
                              where q.c_iteno   == KodeItem &&
                                    q.c_batch   == Batch &&
                                    q.c_gdg     == Convert.ToChar(Gudang) &&
                                    q.status    == "02"
                              select q).Take(1).SingleOrDefault();


                    if (daily2 == null)
                    {
                        daily2 = new LG_DAILY_STOCK_v2()
                        {
                            c_gdg       = Convert.ToChar(Gudang),
                            c_iteno     = KodeItem,
                            c_batch     = Batch,
                            n_qty       = QtyBad,
                            status      = "02",
                            c_locno     = "KMZ",
                            d_entry     = DateTime.Now,
                            d_update    = DateTime.Now,
                        };

                        db.LG_DAILY_STOCK_v2s.InsertOnSubmit(daily2);
                    }
                    else
                    {
                        daily2.n_qty    = daily2.n_qty + QtyBad;
                        daily2.d_update = DateTime.Now;
                    }
                }

                #endregion

                isOk = 1;
            }

            db.SubmitChanges();
            return isOk;
        }

    
    
    }
}
