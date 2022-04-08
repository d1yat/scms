<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PackingListCtrl.ascx.cs"
  Inherits="transaksi_penjualan_PackingListCtrl" %>

<script type="text/javascript">

    //Indra 20181115FM ETD First
    //var selectedSP = function(combo, rec, hfSPDate) {
    var selectedSP = function(combo, rec, hfSPDate, hfETDDate) {
        if (Ext.isEmpty(rec)) {
            ShowWarning(String.format("Record '{0}' tidak dapat di baca dari store.", value));
            combo.clearValue();
            return;
        }

        var getspdate = rec.get('d_spdate');
        hfSPDate.setValue(getspdate);

        var getetddate = rec.get('d_etdsp');
        hfETDDate.setValue(getetddate);
    }
    
    
    var selectedItemBatchPL = function(combo, rec, target, cbItm, cbSp, grid, hfExpire) {
        if (Ext.isEmpty(target)) {
            ShowWarning("Objek target tidak terdefinisi.");
            return;
        }

        if (!Ext.isEmpty(target)) {
            target.setMinValue(0);
            target.setMaxValue(0);
        }

        if (Ext.isEmpty(rec)) {
            ShowWarning(String.format("Record '{0}' tidak dapat di baca dari store.", value));
            combo.clearValue();
            return;
        }

        var recItem = cbItm.findRecord(cbItm.valueField, cbItm.getValue());
        if (Ext.isEmpty(recItem)) {
            ShowWarning(String.format("Record item '{0}' tidak dapat di baca dari store.", cbItm.getText()));
            return;
        }

        var qtySps = 0; //recSp.get('n_spsisa');
        var recSp = cbSp.findRecord(cbSp.valueField, cbSp.getValue());
        if (Ext.isEmpty(recSp)) {
            //ShowWarning(String.format("Record SP '{0}' tidak dapat di baca dari store.", cbSp.getText()));
            //return;
            var storSP = cbSp.getStore();
            if (Ext.isEmpty(storSP)) {
                ShowWarning('Store untuk SP tidak dapat terbaca.');
                return;
            }

            storSP.each(function(r) {
                qtySps += (r.get('n_spsisa') || 0);
            });
        }
        else {
            qtySps = recSp.get('n_spsisa');
        }

        var batCode = rec.get('c_batch');
        var qtyBat = rec.get('n_qtybatch');
        var qtySoh = 0; //(recItem.get('n_soh') || 0);

        if (qtyBat <= 0.00) {
            //ShowWarning("Batch '" + value + "' tidak dapat di baca dari store.");
            ShowWarning(String.format("Batch '{0}' tidak dapat dipergunakan karena <= 0.00", batCode));
            combo.clearValue();
            return false;
        }

        var store = grid.getStore();
        var BatchVal = combo.getValue();
        var ItemVal = cbItm.getValue();
        var spVal = cbSp.getValue();
        //    else if (qtySoh <= 0.00) {
        //      qtyBat = qtySps = 0;
        //      ShowWarning(String.format("Batch '{0}' tidak dapat dipergunakan karena stok gudang <= 0.00", batCode));
        //      combo.clearValue();
        //      return false;
        //    }

        //    if (qtyBat > qtySoh) {
        //      qtyBat = qtySoh;
        //    }


        var nQtyBatch = 0;
        var nQtySp = 0;
        var nQtyBatchMin = 0;
        var nQtySpMin = 0;
        var nHasil = 0;
        var isNew = false;

        var getexp = rec.get('d_expired');
        hfExpire.setValue(getexp);

        try {
            target.setMinValue(0);

            if (Ext.isNumber(qtyBat)) {
                target.setMaxValue(qtyBat);
            }
            else {
                target.setMaxValue(Number.MAX_VALUE);
            }
            if (Ext.isNumber(qtySps)) {
                if (qtySps > qtyBat) {
                    if (Ext.isNumber(qtyBat)) {
                        target.setValue(qtyBat);
                        nHasil = qtyBat;
                    }
                    else {
                        target.setValue(0);
                    }
                }
                else {
                    target.setValue(qtySps);
                    nHasil = qtySps;
                }
            }
            else {
                target.setValue(0);
            }

            for (nLen = 0; nLen < store.data.length; nLen++) {
                isNew = store.data.items[nLen].data.l_new;
                if (isNew) {
                    var iTm = store.data.items[nLen].data.c_iteno;
                    var iSp = store.data.items[nLen].data.c_sp;
                    var iBatch = store.data.items[nLen].data.c_batch;
                    if (iTm == ItemVal && iSp == spVal) {
                        nQtySpMin += store.data.items[nLen].data.n_booked;
                        nQtySp += store.data.items[nLen].data.n_booked;
                        nQtySp += nHasil;
                        if (BatchVal == iBatch) {
                            ShowWarning('Data telah ada.');
                            return false;
                        }
                        else {
                            if (nQtySp > qtySps) {
                                qtySps -= nQtySpMin;
                                target.setMaxValue(qtySps);
                                target.setValue(qtySps);

                            }
                        }
                    }
                    else if (iTm == ItemVal && iBatch == BatchVal) {
                        nQtyBatch += store.data.items[nLen].data.n_booked;
                        nQtyBatch += nHasil;
                    }
                }
            }
        }
        catch (e) {
            ShowError(e.toString());
        }


    }
  //  var validasiEditTotalPermintaan = function(grid) {
  //    if (grid.selModel.getSelections().length > 0) {
  //      var rec = grid.selModel.getSelections()[0];
  //      if (!Ext.isEmpty(rec)) {
  //        var store = grid.getStore();
  //        var itemNo = rec.get('c_iteno');
  //        var spNo = rec.get('c_sp');
  //        var qty = rec.get('n_booked');
  //        var reqQty = rec.get('n_QtyRequest');

  //        if (qty > 0.00) {
  //          if (validasiTotalPermintaan(store, 'c_iteno', 'c_sp', itemNo, spNo, 0, reqQty)) {
  //            rec.commit();
  //          }
  //          else {
  //            rec.reject();
  //          }
  //        }
  //        else {
  //          ShowConfirm('Hapus ?',
  //              "Mengganti nilai quantity '0' sama dengan menghapus, apa anda ingin menghapus data ini ?",
  //              function(btn) {
  //                if (btn == 'yes') {
  //                  if ((!Ext.isEmpty(store)) && (!Ext.isEmpty(rec))) {
  //                    rec.set('l_void', true);
  //                  }
  //                }
  //                else {
  //                  if (!Ext.isEmpty(rec)) {
  //                    rec.reject();
  //                  }
  //                }
  //              });
  //        }
  //      }
  //    }
    // } 

  var validasiTotalPermintaan = function(store, fieldItem, fieldSp, itemCode, spCode, inpQty, totalReq, totalSoh) {
    var total = inpQty;
    var idx = 0;
    var rec = '';
    var spc = '';

    if (store.getCount() < 1) {
      return true;
    }

    do {
      idx = store.findExact(fieldItem, itemCode, idx);
      if (idx != -1) {
        rec = store.getAt(idx);
        if (!Ext.isEmpty(rec)) {
          spc = rec.get('c_sp').trim();
          if (spc == spCode) {
            total += rec.get('n_booked');
          }
        }
        idx++;
      }
    } while (idx != -1);

    if (total > totalReq) {
      return false;
    }
    else if (total > totalSoh) {
      return false;
    }

    return true;
  }
  
  //Indra 20181115FM ETD First
  //var storeToDetailGridMulti = function(frm, grid, item, sp, tipe, batch, quantity, cbItm, cbSp, hfExpire, hfSPDate) {
  var storeToDetailGridMulti = function(frm, grid, item, sp, tipe, batch, quantity, cbItm, cbSp, hfExpire, hfSPDate, hfETDDate) {
//      if (!frm.getForm().isValid()) {
//          ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
//          return;
//      }

      if (Ext.isEmpty(grid) ||
          Ext.isEmpty(item) ||
          Ext.isEmpty(sp) ||
          Ext.isEmpty(tipe) ||
          Ext.isEmpty(batch) ||
          Ext.isEmpty(quantity) ||
          Ext.isEmpty(hfExpire) ||
          quantity == 0) {
          ShowWarning("Objek tidak terdefinisi.");
          return;
      }

      // Initialize variables

      //    if (table === undefined || table == null || table.length <= 0) {
      //      var table = [];
      //    }

      //    var newEntry = [];

      // Create a new object
      //    newEntry = {
      //      kd_item: item,
      //      kd_sp: sp,
      //      kd_batch: batch,
      //      kd_qty: quantity
      //    };
      //    
      //    strGrid.insert(0, new Ext.data.Record({
      //                'c_iteno': item,
      //                'v_itemdesc': sp,
      //                'c_sp': batch,
      //                'c_spc': quantity,
      //              }));

      //table.push(newEntry);

      //    var nLen = 0,
      //    nLenI = 0;

      //    if (dataPL.length <= 0) {
      //      for (nLenI = 0; nLenI < dataPL.length; nLenI++) {

      //      }
      //    } else {
      //      dataPL = {
      //        pl_item: item,
      //        pl_sp: sp,
      //        pl_batch: batch,
      //        pl_qty: quantity
      //      }
      //    }
      //    var nQtyBatch = 0;
      //    var nQtySp = 0;

      //    for (nLen = 0; nLen < strGrid.length; nLen++) {

      //      var iTm = strGrid[nLen].kd_item.getValue();
      //      var iSp = strGrid[nLen].kd_sp.getValue();
      //      var iBatch = table[nLen].kd_batch.getValue();
      //      if (iTm == item && iSp == sp) {
      //        nQtySp += table[nLen].kd_qty.getValue();
      //      }
      //      else if (iTm == item && iBatch == batch) {
      //        nQtyBatch += table[nLen].kd_qty.getValue();
      //      }
      //    }


      var recItem = cbItm.findRecord(cbItm.valueField, cbItm.getValue());
      if (Ext.isEmpty(recItem)) {
          ShowWarning(String.format("Record item '{0}' tidak dapat di baca dari store.", cbItm.getText()));
          return;
      }

      var recSp = cbSp.findRecord(cbSp.valueField, cbSp.getValue());

      var store = grid.getStore();
      if (Ext.isEmpty(store)) {
          ShowWarning("Objek store tidak terdefinisi.");
          return;
      }


      var valX = undefined,
      fieldX = ['c_iteno', 'c_sp', 'c_type', 'c_batch']; ;
      var isDup = false,
      storBatch = batch.getStore(),
      storSP = undefined,
      bat = batch.getValue().trim(),
      qty = quantity.getValue(),
      sumQty = qty,
      reqQty = 0,
      itemNo = item.getValue().trim(),
      itemName = item.getText().trim(),
      spNo = '',
      spCab = '',
      totalQty = 0,
      totalRow = 0,
      idx = 0,
      idxBatch = 0,
      typeCode = tipe.getValue();

      //cek ED
      var expdate = hfExpire.getValue();
      var currentDate = new Date();
      var statusED = false;

      var expdatep = new Date(expdate)
      expdatep.setDate(expdatep.getDate() - 366);

      if (expdatep < currentDate) {
          statusED = true;
      }
      else {
          statusED = false;
      }

      var spdate = hfSPDate.getValue() ? hfSPDate.getValue() : null;

      var etddate = hfETDDate.getValue() ? hfETDDate.getValue() : null; //Indra 20181115FM ETD First

      // Multiple SP
      if (Ext.isEmpty(recSp)) {
          storSP = cbSp.getStore();
          if (Ext.isEmpty(storSP)) {
              ShowWarning('Store untuk SP tidak dapat terbaca.');
              return;
          }

          totalRow = storSP.getCount();

          if (Ext.isEmpty(storBatch)) {
              idxBatch = -1;
          }
          else {
              idxBatch = storBatch.findExact('c_batch', bat);
          }

          if (idxBatch == -1) {
              ShowError('Batch tidak dapat dibaca.');
              return;
          }

          //      totalQty = (recItem.get('n_soh') || 0);

          totalQty = sumQty;

          while (idx < totalRow) {
              recSp = storSP.getAt(idx);
              if (Ext.isEmpty(recSp) || (sumQty <= 0.00)) {
                  break;
              }

              spdate = recSp.get('d_spdate');
              etddate = recSp.get('d_etdsp'); //Indra 20181115FM ETD First

              qty = reqQty =
          (recSp.get('n_spsisa') || 0);

              if (reqQty > 0) {
                  qty = reqQty =
            (qty > sumQty ? sumQty : qty);

                  spNo = recSp.get('c_spno');
                  spCab = recSp.get('c_sp');

                  valX = [itemNo, spNo, typeCode, bat];

                  isDup = findDuplicateEntryGrid(store, fieldX, valX);

                  if (!isDup) {
                      //if (validasiTotalPermintaan(grid.getStore(), 'c_iteno', 'c_sp', itemNo, spNo, qty, reqQty, qty)) {
                      if (validasiTotalPermintaan(grid.getStore(), 'c_iteno', 'c_sp', itemNo, spNo, qty, reqQty, totalQty)) {
                          store.insert(0, new Ext.data.Record({
                              'c_iteno': itemNo,
                              'v_itemdesc': itemName,
                              'c_sp': spNo,
                              'c_spc': spCab,
                              'c_type': typeCode,
                              'v_undes': recItem.get('v_undes'),
                              //'v_typedesc': 'Beli',
                              'c_batch': bat,
                              'n_booked': reqQty,
                              'n_QtyRequest': reqQty,
                              'd_spdate': spdate,
                              'd_etdsp': etddate, //Indra 20181115FM ETD First
                              'd_expired': expdate,
                              'l_expired': statusED,
                              'l_new': true
                          }));
                      }
                  }
                  sumQty -= reqQty;
              }
              idx++;
          }

          item.reset();
          sp.reset();
          batch.reset();
          quantity.reset();
          hfExpire.clear();
          hfSPDate.clear();
          hfETDDate.clear(); //Indra 20181115FM ETD First

          return;
      }

      spNo = sp.getValue().trim();
      spCab = sp.getText().trim();

      valX = [itemNo, spNo, typeCode, bat];

      // Find Duplicate entry
      isDup = findDuplicateEntryGrid(store, fieldX, valX);

      if (!isDup) {
          reqQty = 0;
          totalQty = 0;

          if (!Ext.isEmpty(storBatch)) {
              idxBatch = storBatch.findExact('c_batch', bat);
              if (idxBatch != -1) {
                  //var rec = storBatch.getAt(idxBatch);
                  //reqQty = rec.get('n_spsisa');
                  reqQty = recSp.get('n_spsisa');
                  //totalQty = rec.get('n_soh');
                  totalQty = recSp.get('n_spqty'); //(recItem.get('n_soh') || 0);
              }
              else {
                  reqQty = qty;
                  totalQty = 0;
              }
          }
          else {
              reqQty = qty;
              totalQty = 0;
          }

          if (validasiTotalPermintaan(grid.getStore(), 'c_iteno', 'c_sp', itemNo, spNo, qty, totalQty, totalQty)) {
              store.insert(0, new Ext.data.Record({
                  'c_iteno': itemNo,
                  'v_itemdesc': itemName,
                  'c_sp': spNo,
                  'c_spc': spCab,
                  'c_type': typeCode,
                  'v_undes': recItem.get('v_undes'),
                  //'v_typedesc': 'Beli',
                  'c_batch': bat,
                  'n_booked': qty,
                  'n_QtyRequest': qty,
                  'd_spdate': spdate,
                  'd_etdsp': etddate, //Indra 20181115FM ETD First
                  'd_expired': expdate,
                  'l_expired': statusED,
                  'l_new': true
              }));

              item.reset();
              sp.reset();
              batch.reset();
              hfExpire.clear();
              hfSPDate.clear();
              hfETDDate.clear();  //Indra 20181115FM ETD First
          }
          else {
              ShowError('Mohon diperiksa kembali, apakah jumlah yang telah diinput telah melebihi dari permintaan/SOH gudang.');
          }

          quantity.reset();
      }
      else {
          ShowError('Data telah ada.');

          return false;
      }
  }
  //  var storeToDetailGrid = function(frm, grid, item, sp, tipe, batch, quantity, cbItm, cbSp) {
  //    if (!frm.getForm().isValid()) {
  //      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
  //      return;
  //    }

  //    if (Ext.isEmpty(grid) ||
  //          Ext.isEmpty(item) ||
  //          Ext.isEmpty(sp) ||
  //          Ext.isEmpty(tipe) ||
  //          Ext.isEmpty(batch) ||
  //          Ext.isEmpty(quantity)) {
  //      ShowWarning("Objek tidak terdefinisi.");
  //      return;
  //    }

  //    var recItem = cbItm.findRecord(cbItm.valueField, cbItm.getValue());
  //    if (Ext.isEmpty(recItem)) {
  //      ShowWarning(String.format("Record item '{0}' tidak dapat di baca dari store.", cbItm.getText()));
  //      return;
  //    }

  //    var recSp = cbSp.findRecord(cbSp.valueField, cbSp.getValue());
  //    if (Ext.isEmpty(recItem)) {
  //      ShowWarning(String.format("Record SP '{0}' tidak dapat di baca dari store.", cbSp.getText()));
  //      return;
  //    }

  //    var store = grid.getStore();
  //    if (Ext.isEmpty(store)) {
  //      ShowWarning("Objek store tidak terdefinisi.");
  //      return;
  //    }

  //    var valX = [item.getValue(), sp.getValue(), tipe.getValue(), batch.getValue()];
  //    var fieldX = ['c_iteno', 'c_sp', 'c_type', 'c_batch'];

  //    //    var isDup = false;
  //    //    var nDup = 0;

  //    // Find Duplicate entry
  //    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

  //    //    if (nDup == valX.length) {
  //    //      isDup = true;
  //    //    }

  //    if (!isDup) {
  //      var storBatch = batch.getStore();
  //      var bat = batch.getValue().trim();
  //      var qty = quantity.getValue();
  //      var reqQty = 0;
  //      var itemNo = item.getValue().trim();
  //      var spNo = sp.getValue().trim();
  //      var totalQty = 0;

  //      if (!Ext.isEmpty(storBatch)) {
  //        var idx = storBatch.findExact('c_batch', bat);
  //        if (idx != -1) {
  //          //var rec = storBatch.getAt(idx);
  //          //reqQty = rec.get('n_spsisa');
  //          reqQty = recSp.get('n_spsisa');
  //          //totalQty = rec.get('n_soh');
  //          //          totalQty = (recItem.get('n_soh') || 0);
  //          totalQty = 0;
  //        }
  //        else {
  //          reqQty = qty;
  //          totalQty = 0;
  //        }
  //      }
  //      else {
  //        reqQty = qty;
  //        totalQty = 0;
  //      }

  //      if (validasiTotalPermintaan(grid.getStore(), 'c_iteno', 'c_sp', itemNo, spNo, qty, reqQty, totalQty)) {
  //        store.insert(0, new Ext.data.Record({
  //          'c_iteno': itemNo,
  //          'v_itemdesc': item.getText(),
  //          'c_sp': spNo,
  //          'c_spc': sp.getText(),
  //          //'c_type': tipe.getValue(),
  //          //'v_typedesc': 'Beli',
  //          'c_batch': bat,
  //          'n_booked': qty,
  //          'n_QtyRequest': qty,
  //          'l_new': true
  //        }));

  //        item.reset();
  //        sp.reset();
  //        batch.reset();
  //      }
  //      else {
  //        ShowError('Mohon diperiksa kembali, apakah jumlah yang telah diinput telah melebihi dari permintaan/SOH gudang.');
  //      }

  //      quantity.reset();
  //    }
  //    else {
  //      ShowError('Data telah ada.');

  //      return false;
  //    }
  //  }
  var prepareCommands = function(rec, toolbar, valX) {
    var del = toolbar.items.get(0); // delete button
    var vd = toolbar.items.get(1); // void button

    var isNew = false;

    if (!Ext.isEmpty(rec)) {
      isNew = rec.get('l_new');
    }

    if (Ext.isEmpty(valX) || isNew) {
      del.setVisible(true);
      vd.setVisible(false);
    }
    else {
      del.setVisible(false);
      vd.setVisible(true);
    }
  }
  var checkForExistingDataInGridDetail = function(cb, rec, grid) {
    if (Ext.isEmpty(grid)) {
      return false;
    }

    var store = grid.getStore();
    if (Ext.isEmpty(store)) {
      return false;
    }

    if (Ext.isEmpty(rec)) {
      return false;
    }

    var newPrinc = (rec.get('c_nosup')).toString().trim();
    var oldPric = cb.getValue().trim();

    if (newPrinc == oldPric) {
      return;
    }

    var len = store.getModifiedRecords().length;

    if (len > 0) {
      ShowWarning('Maaf, anda tidak dapat mengganti pemasok, jika telah ada data didalam grid detail.');
      return false;
    }
  }
  var voidInsertedDataFromStore = function(rec) {
    if (Ext.isEmpty(rec)) {
      return;
    }

    var isVoid = rec.get('l_void');

    if (isVoid) {
      ShowWarning('Item ini telah di batalkan.');
    }
    else {
      ShowConfirm('Hapus ?', 'Apakah anda yakin ingin menghapus data ini ?',
        function(btn) {
          if (btn == 'yes') {
            ShowAsk('Alasan ?', 'Masukkan alasan kenapa harus dibatalkan.',
              function(btnP, txt) {
                if (btnP == 'ok') {
                  if (txt.trim().length < 1) {
                    txt = 'Kesalahan pemakai.';
                  }
                  rec.set('l_void', true);
                  rec.set('v_ket', txt);
                }
              });
          }
        });
    }
  }
  var onChangeKategoriItem = function(g,cb,cbVia) {
      var storeCb = cb.getStore();
      if (!Ext.isEmpty(storeCb)) {
          var idx = cb.selectedIndex;

          if (idx > 0 && idx < 5) {
              cbVia.setValue("01");
              cbVia.setRawValue("Udara");
          }
      }
      
    var store = g.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
    }
    

    store.removeAll();
    store.commitChanges();
  }
  var onCheckAutoValidation = function(isValid, cb) {
      //if (isValid && (comboGetSelectedIndex(cb) != -1)) {
      if (isValid) {
          return true;
          alert(cb);
      }
  }
  var onGridBeforeEdit = function(e) {
    if (e.field == 'v_ket_ed') {
        e.cancel = false;
    }
    else if (e.field == 'n_QtyRequest') {
      if (!e.record.get('l_new')) {
        e.cancel = true;
      }
    }
    else {
      e.cancel = true;
    }
  }
  var onGridAfterEdit = function(e) {
    var prevValue = 0;
    if (e.field == 'n_QtyRequest') {
      if (e.record.get('l_new')) {
        prevValue = (e.record.get('n_lastqtyRequest') || 0);


        if ((prevValue == 0) && (e.value > e.originalValue)) {
          e.record.reject();

          ShowWarning('Jumlah quantity tidak boleh lebih besar dari ketersedian data.');
        }
        else if ((prevValue != 0) && (e.value > prevValue)) {
          e.record.set('n_QtyRequest', prevValue);
          e.record.set('n_booked', prevValue);

          ShowWarning('Jumlah quantity tidak boleh lebih besar dari ketersedian data.');
        }
        else {
          e.record.set('n_QtyRequest', e.value);
          e.record.set('n_booked', e.value);
          if (prevValue == 0) {
            e.record.set('n_lastqtyRequest', e.originalValue);
          }
        }
      }
      else {
        e.record.set('n_QtyRequest', e.originalValue);
        e.record.set('n_booked', e.originalValue);
      }
    }
    else if (e.field == 'v_ket_ed' && e.record.get('l_new') == false) {
      var lExpired = e.record.get('l_expired');
      if (lExpired == true) {
          e.record.set('l_accmodify', true);
      }
    }
  }

  var checkForADM = function(cb, cbBaspb) {
      var store = cb.getStore();
      if (!Ext.isEmpty(store)) {
          var idx = cb.selectedIndex;

          if (idx == 1) {
              cbBaspb.setVisible(true);
              cbBaspb.setValue("");
          }
          else {
              cbBaspb.setVisible(false);
              cbBaspb.setValue(" ");
          }
      }
  }

  var checkForNovell = function(cb, cbDivPri) {
      var store = cb.getStore();
      if (!Ext.isEmpty(store)) {
          var idx = cb.getValue();

          if (idx == "00019") {
              cbDivPri.setVisible(true);
              cbDivPri.clearValue();              
          }
          else {
              cbDivPri.setVisible(false);
              cbDivPri.clearValue();
              cbDivPri.setValue(" ");              
          }
      }
  }

  var deleteRecordOnGridPL = function(grid, rec) {
      var store = grid.getStore();
      if ((!Ext.isEmpty(store)) && (!Ext.isEmpty(rec))) {
          store.remove(rec);
      }
  }

  var deleteGrid = function(grid) {
      var sm = grid.getSelectionModel();
      var sel = sm.getSelections();
      for (i = 0; i < sel.length; i++) {
          grid.store.remove(sel[i]);
      }
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="950" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfGudang" runat="server" />
    <ext:Hidden ID="hfGudangDesc" runat="server" />
    <ext:Hidden ID="hfPlNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfExpire" runat="server" />
    <ext:Hidden ID="hfSPDate" runat="server" />   
    <ext:Hidden ID="hfETDDate" runat="server" /><%--Indra 20181115FM ETD First--%>
    <ext:Store ID="strGird" runat="server" Visible ="false" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="170" MaxHeight="200" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="210"
          Layout="Fit" ButtonAlign="Center" MonitorValid="true">
          <Items>
            <ext:Panel ID="Panel1" runat="server" Padding="5" Layout="Column">
              <Items>
                <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" ColumnWidth=".5"
                  Layout="Form">
                  <Items>
                    <%--Indra 20181226FM Penambahan filter ETD--%>
                    <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Periode ETD">
                      <Items>
                        <ext:DateField ID="txPeriode1" runat="server" Vtype="daterange" Format="dd-MM-yyyy"
                          AllowBlank="true">
                          <CustomConfig>
                            <ext:ConfigItem Name="endDateField" Value="#{txPeriode2}" Mode="Value" />
                          </CustomConfig>
                          <Listeners>
                            <Change Handler="clearRelatedComboRecursive(true, #{cbCustomerHdr});" />    
                          </Listeners>
                        </ext:DateField>
                        <ext:Label ID="Label1" runat="server" Text="s/d" />
                        <ext:DateField ID="txPeriode2" runat="server" Vtype="daterange" Format="dd-MM-yyyy"
                          AllowBlank="true">
                          <CustomConfig>
                            <ext:ConfigItem Name="startDateField" Value="#{txPeriode1}" Mode="Value" />
                          </CustomConfig>
                          <Listeners>
                            <Change Handler="clearRelatedComboRecursive(true, #{cbCustomerHdr});" />    
                          </Listeners>
                        </ext:DateField>
                      </Items>
                    </ext:CompositeField>
                    <ext:ComboBox ID="cbCustomerHdr" runat="server" FieldLabel="Cabang" DisplayField="v_cunam"
                      ValueField="c_cusno" Width="250" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
                      MinChars="3" AllowBlank="false" ForceSelection="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="false" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store3" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <%--<ext:Parameter Name="model" Value="3001" />--%>
                            <ext:Parameter Name="model" Value="2011" />
                            <ext:Parameter Name="parameters" Value="[['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerHdr}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="v_cunam" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_cusno" />
                                <ext:RecordField Name="v_cunam" />
                                <ext:RecordField Name="c_cab" />
                                <ext:RecordField Name="c_gdg_cab" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template1" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 400px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Short</td><td class="body-panel">Cabang</td></tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_cusno}</td><td>{c_cab}</td><td>{v_cunam}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" /> 
                         <%--<Change Handler="resetEntryWhenChange(#{pnlGridDetail}, #{frmpnlDetailEntry});" />--%>
                         
                         <Change Handler=" resetEntryWhenChange(#{gridDetail}, #{frmpnlDetailEntry});clearRelatedComboRecursive(true, #{cbPrincipalHdr}, #{cbLantai},#{cbDivPrinsipal} ); " />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbPrincipalHdr" runat="server" FieldLabel="Pemasok" DisplayField="v_nama"
                      ValueField="c_nosup" Width="250" ItemSelector="tr.search-item" ListWidth="350"
                      MinChars="3" AllowBlank="true" ForceSelection="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="true" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store4" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="-1" />
                            <ext:Parameter Name="model" Value="3101" />
                            <%--<ext:Parameter Name="model" Value="2021" />--%>
                            <ext:Parameter Name="parameters" Value="[['cusno', #{cbCustomerHdr}.getValue(), 'System.String'],
                            ['gdg', #{hfGudang}.getValue(), 'System.Char'],
                            ['Periode1', paramRawValueGetter(#{txPeriode1}), 'System.DateTime'],
                            ['Periode2', paramRawValueGetter(#{txPeriode2}), 'System.DateTime'],
                            ['@contains.v_nama.Contains(@0) || @contains.c_nosup.Contains(@0)', paramTextGetter(#{cbPrincipalHdr}), '']]"
                             Mode="Raw" />
                             <%--<ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean'],
                                                                      ['l_hide = @0', false, 'System.Boolean'],
                                                                      ['@contains.v_nama.Contains(@0) || @contains.c_nosup.Contains(@0)', paramTextGetter(#{cbPrincipalHdr}), '']]"
                              Mode="Raw" />--%>
                            <ext:Parameter Name="sort" Value="v_nama" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_nosup" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_nosup" />
                                <ext:RecordField Name="v_nama" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template2" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 350px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Pemasok</td></tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_nosup}</td><td>{v_nama}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <BeforeSelect Handler="return checkForExistingDataInGridDetail(this, record, #{gridDetail});" />
                        <Change Handler="resetEntryWhenChange(#{gridDetail}, #{frmpnlDetailEntry}); clearRelatedComboRecursive(true, #{cbItemDtl}, #{cbDivPrinsipal}, #{cbLantai});checkForNovell(this,#{cbDivPrinsipal});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbDivPrinsipal" runat="server" FieldLabel="Divisi Pemasok" ValueField="c_kddivpri"
                      DisplayField="v_nmdivpri" Width="250" ListWidth="350" 
                      PageSize="10" ItemSelector="tr.search-item" AllowBlank="true" ForceSelection="false" Hidden = "true">
                      <CustomConfig>
                        <ext:ConfigItem Name="AllowBlank" Value="true" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store8" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="3101" />
                            <%--<ext:Parameter Name="model" Value="2051" />--%>
                            <ext:Parameter Name="parameters" Value="[['cusno', #{cbCustomerHdr}.getValue(), 'System.String'],
                                        ['gdg', #{hfGudang}.getValue(), 'System.Char'],
                                        ['Periode1', paramRawValueGetter(#{txPeriode1}), 'System.DateTime'],
                                        ['Periode2', paramRawValueGetter(#{txPeriode2}), 'System.DateTime'],
                                        ['@contains.c_nosup.Contains(@0)', paramValueGetter(#{cbPrincipalHdr}), 'System.String'],
                                        ['@contains.c_kddivpri.Contains(@0) || @contains.v_nmdivpri.Contains(@0)', paramTextGetter(#{cbDivPrinsipal}), '']]" Mode="Raw" />
                            <%--<ext:Parameter Name="parameters" Value="[['@contains.c_nosup.Contains(@0)', paramValueGetter(#{cbPrincipalHdr}), 'System.String'],
                                        ['@contains.c_kddivpri.Contains(@0) || @contains.v_nmdivpri.Contains(@0)', paramTextGetter(#{cbDivPrinsipal}), '']]" Mode="Raw" />--%>
                            <ext:Parameter Name="sort" Value="v_nmdivpri" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_kddivpri" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_kddivpri" />
                                <ext:RecordField Name="v_nmdivpri" />
                                <ext:RecordField Name="c_nosup" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template4" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 500px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
                        <tpl for=".">
                          <tr class="search-item">
                            <td>{c_kddivpri}</td><td>{v_nmdivpri}</td>
                          </tr>
                        </tpl>
                        </table>
                        </Html>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <Select Handler="this.triggers[0].show();" />
                        <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                        <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); } else if(index == 1) { reloadData(this); }" />
                        <Change Handler="resetEntryWhenChange(#{gridDetail}, #{frmpnlDetailEntry}); clearRelatedComboRecursive(true, #{cbItemDtl},#{cbLantai});" />                        
                      </Listeners>
                    </ext:ComboBox>
                    <%--  <ext:ComboBox ID="cbLantai" runat="server" FieldLabel="Lantai" DisplayField="v_ket"
                      ValueField="c_type" Width="150" AllowBlank="false" ForceSelection="false" MinChars="3">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="true" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store6" runat="server" RemotePaging="false">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>                            
                            <ext:Parameter Name="allQuery" Value="true" />
                            <ext:Parameter Name="model" Value="3101" />
                            <ext:Parameter Name="parameters" Value="[['cusno', #{cbCustomerHdr}.getValue(), 'System.String'],
                                        ['gdg', #{hfGudang}.getValue(), 'System.Char'],
                                        ['@contains.c_nosup.Contains(@0)', paramValueGetter(#{cbPrincipalHdr}), 'System.String'],
                                        ['@contains.c_kddivpri.Contains(@0)', paramValueGetter(#{cbDivPrinsipal}), 'System.String'],
                                        ['@contains.c_type.Contains(@0) || @contains.v_ket.Contains(@0)', paramTextGetter(#{cbLantai}), '']]" Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_type" />
                            <ext:Parameter Name="dir" Value="ASC" />
                            
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_type" />
                                <ext:RecordField Name="v_ket" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <Change Handler="resetEntryWhenChange(#{gridDetail}, #{frmpnlDetailEntry}); clearRelatedComboRecursive(true, #{cbKategori});" />
                      </Listeners>
                    </ext:ComboBox>--%>
                    <ext:ComboBox ID="cbKategori" runat="server" FieldLabel="Kategori" DisplayField="v_ket"
                      ValueField="c_type" Width="150" AllowBlank="false" ForceSelection="false" MinChars="3">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="false" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store2" runat="server" RemotePaging="false">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="allQuery" Value="true" />
                            <ext:Parameter Name="model" Value="4091" />
                            <ext:Parameter Name="parameters" Value="[['c_portal = @0', '9', 'System.Char'],
                                              ['c_notrans = @0', '001', ''],
                                              ['lantai', #{cbLantai}.getValue(), 'System.String']]" Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_type" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_type" />
                                <ext:RecordField Name="v_ket" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <Change Handler="resetEntryWhenChange(#{gridDetail}, #{frmpnlDetailEntry});onChangeKategoriItem(#{gridDetail},this,#{cbViaHdr});clearRelatedComboRecursive(true, #{cbItemDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <%--<ext:Checkbox ID="chkConfirm" runat="server" FieldLabel="Konfirmasi" />--%>
                    
                  </Items>
                </ext:Panel>
                <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" ColumnWidth=".5"
                  Layout="Form">
                  <Items>
                    <ext:ComboBox ID="cbTipeHdr" runat="server" FieldLabel="Tipe" DisplayField="v_ket"
                      ValueField="c_type" Width="150" AllowBlank="false" ForceSelection="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="false" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store1" runat="server" RemotePaging="false">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="allQuery" Value="true" />
                            <ext:Parameter Name="model" Value="2001" />
                            <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                              ['c_notrans = @0', '15', ''],
                                              ['c_type != @0', '02', '']]" Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_type" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_type" />
                                <ext:RecordField Name="v_ket" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Listeners>
                        <Change Handler="checkForADM(this,#{cbBaspbNo});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbBaspbNo" runat="server" FieldLabel="No.Baspb" DisplayField="c_baspbno"
                      ValueField="c_baspbno" Width="200" ItemSelector="tr.search-item" ListWidth="200"
                      MinChars="3" AllowBlank="false" ForceSelection="false" Hidden = "true">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="true" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store7" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="-1" />
                            <ext:Parameter Name="model" Value="3501" />
                            <ext:Parameter Name="parameters" Value="[['cusno', #{cbCustomerHdr}.getValue(), 'System.String'],
                        ['@contains.c_baspbno.Contains(@0)', paramTextGetter(#{cbBaspbNo}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_baspbno" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_baspbno" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_baspbno" />
                                <ext:RecordField Name="c_cusno" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template3" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 150px">
                        <tr><td class="body-panel">No.Baspb</td></tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_baspbno}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <Change Handler="clearRelatedComboRecursive(true, #{cbItemDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbViaHdr" runat="server" FieldLabel="Via" DisplayField="v_ket"
                      ValueField="c_type" Width="125" AllowBlank="false" ForceSelection="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="false" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store5" runat="server" RemotePaging="false">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="allQuery" Value="true" />
                            <ext:Parameter Name="model" Value="2001" />
                            <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                              ['c_notrans = @0', '02', '']]" Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_type" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_type" />
                                <ext:RecordField Name="v_ket" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                    </ext:ComboBox>
                    <ext:TextField ID="txKeterangan" runat="server" FieldLabel="Keterangan" MaxLength="100" Width="350" />
                    <ext:Label ID="lbChkConfirm" runat="server" FieldLabel="Konfirmasi" />
                  </Items>
                </ext:Panel>
              </Items>
            </ext:Panel>
          </Items>
          <%--<Listeners>
            <ClientValidation Handler="#{btnAutoGen}.setDisabled(!valid);" />
          </Listeners>--%>
          <Buttons>
            <ext:Button ID="btnAutoGen" runat="server" Text="Auto Generator" Icon="CogStart" Disabled="false">
              <DirectEvents>
                <Click OnEvent="AutoGenBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders});">
                  <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders});" ConfirmRequest="true"
                    Title="Proses ?" Message="Anda yakin ingin otomatis proses pemilihan item PL ?" />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="NumberID" Value="#{hfPlNo}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="CustomerID" Value="#{cbCustomerHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="PrinsipalID" Value="#{cbPrincipalHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="KategoriID" Value="#{cbKategori}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="LantaiID" Value="#{cbLantai}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="DivPriID" Value="#{cbDivPrinsipal}.getValue()" Mode="Raw" />
                    <%--Indra 20181226FM Penambahan filter ETD--%>
                    <ext:Parameter Name="txPeriode1" Value="#{txPeriode1}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="txPeriode2" Value="#{txPeriode2}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>
          </Buttons>
          <%--<Listeners>
            <ClientValidation Handler="#{btnAutoGen}.setDisabled(!onCheckAutoValidation(valid, #{cbPrincipalHdr}));" />
            <ClientValidation Handler="#{btnAutoGen}.setDisabled(!onCheckAutoValidation(valid, #{cbKategori}));" />
          </Listeners>--%> <%--Mark by Suwandi 14 Jan 2018--%>
        </ext:FormPanel>
      </North>
      <Center MinHeight="150">
        <ext:Panel ID="pnlGridDetail" runat="server" Title="Daftar Items" Height="150" Layout="Fit">
          <TopBar>
            <ext:Toolbar runat="server">
              <Items>
                <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                  LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                  <Items>
                    <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" Width="200" ItemSelector="tr.search-item"
                       ListWidth="350" DisplayField="v_itnam" ValueField="c_iteno" MinChars="3">
                      <Store>
                        <ext:Store runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="-1" />
                            <ext:Parameter Name="model" Value="3201" />
                            <%--Indra 20181226FM Penambahan filter ETD--%>
                            <ext:Parameter Name="parameters" Value="[['gdg', #{hfGudang}.getValue(), 'System.Char'],
                                                          ['cusno', #{cbCustomerHdr}.getValue(), 'System.String'],
                                                          ['supl', #{cbPrincipalHdr}.getValue(), 'System.String'],
                                                          ['itemLat', #{cbLantai}.getValue(), 'System.String'],
                                                          ['itemCat', #{cbKategori}.getValue(), 'System.String'],
                                                          ['divPri', #{cbDivPrinsipal}.getValue(), 'System.String'],
                                                          ['@contains.c_iteno.Contains(@0) || @contains.v_itnam.Contains(@0)', paramTextGetter(#{cbItemDtl}), ''],
                                                          ['Periode1', paramRawValueGetter(#{txPeriode1}), 'System.DateTime'],
                                                          ['Periode2', paramRawValueGetter(#{txPeriode2}), 'System.DateTime']]"                                                          
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="v_itnam" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_iteno" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_iteno" />
                                <ext:RecordField Name="v_itnam" />
                                <ext:RecordField Name="n_box" />
                                <ext:RecordField Name="v_undes" />
                                <%--<ext:RecordField Name="n_SumPending" Type="Float" />
                                <ext:RecordField Name="n_soh" Type="Float" />--%>
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 350px">
                        <tr>
                          <td class="body-panel">Kode</td><td class="body-panel">Nama</td>
                          <td class="body-panel">Box</td><td class="body-panel">Kemasan</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                          <td>{c_iteno}</td><td>{v_itnam}</td>
                          <td>{n_box}</td><td>{v_undes}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <Change Handler="clearRelatedComboRecursive(true, #{cbSpcDtl}, #{cbBatDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbSpcDtl" runat="server" FieldLabel="SP Cabang" ItemSelector="tr.search-item"
                      ListWidth="450" DisplayField="c_sp" ValueField="c_spno" AllowBlank="true"
                      ForceSelection="false" MinChars="3">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="true" />
                      </CustomConfig>
                      <Store>
                        <ext:Store runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="-1" />
                            <ext:Parameter Name="model" Value="3301" />
                            <%--Indra 20181226FM Penambahan filter ETD--%>
                            <ext:Parameter Name="parameters" Value="[['cusno', #{cbCustomerHdr}.getValue(), 'System.String'],
                                                          ['item', #{cbItemDtl}.getValue(), 'System.String'],
                                                          ['@contains.c_spno.Contains(@0) || @contains.c_sp.Contains(@0)', paramTextGetter(#{cbSpcDtl}), ''],
                                                          ['Periode1', paramRawValueGetter(#{txPeriode1}), 'System.DateTime'],
                                                          ['Periode2', paramRawValueGetter(#{txPeriode2}), 'System.DateTime']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="d_spdate" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_spno" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_spno" />
                                <ext:RecordField Name="c_sp" />
                                <ext:RecordField Name="d_spdate" DateFormat="M$" Type="Date" />
                                <ext:RecordField Name="v_SpType" />
                                <ext:RecordField Name="n_spsisa" Type="Float" />
                                <ext:RecordField Name="n_spqty" Type="Float" />
                                <ext:RecordField Name="d_etdsp" DateFormat="M$" Type="Date" /><%--Indra 20181115FM ETD First--%>
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 450px">
                        <tr>
                          <td class="body-panel">No SP</td><td class="body-panel">SP Cabang</td>
                          <td class="body-panel">Tipe</td><td class="body-panel">ETD</td><%--Indra 20181115FM ETD First--%>
                          <%--<td class="body-panel">Tanggal</td>--%>
                          <td class="body-panel">Permintaan</td><td class="body-panel">Sisa</td>                          
                        </tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_spno}</td><td>{c_sp}</td>
                            <td>{v_SpType}</td><td>{d_etdsp:this.formatDate}</td><%--Indra 20181115FM ETD First--%>
                            <%--<td>{d_spdate:this.formatDate}</td>--%>
                            <td>{n_spqty}</td><td>{n_spsisa}</td>                            
                        </tr></tpl>
                        </table>
                        </Html>
                        <Functions>
                          <ext:JFunction Name="formatDate" Fn="myFormatDate" />
                        </Functions>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <%--Indra 20181115FM ETD First--%>
                        <%--<Select Handler="selectedSP(this, record, #{hfSPDate})" />--%>
                        <Select Handler="selectedSP(this, record, #{hfSPDate}, #{hfETDDate})" />
                        <%--<Change Handler="clearRelatedComboRecursive(true, #{cbBatDtl});" />--%>
                      </Listeners>
                    </ext:ComboBox>
                    <ext:Hidden ID="hfTypDtl" runat="server" Text="01" />
                    <ext:ComboBox ID="cbBatDtl" runat="server" FieldLabel="Batch" ItemSelector="tr.search-item"
                      ListWidth="350" DisplayField="c_batch" ValueField="c_batch" AllowBlank="true"
                      ForceSelection="false" MinChars="3">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="true" />
                      </CustomConfig>
                      <Store>
                        <ext:Store runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="-1" />
                            <ext:Parameter Name="model" Value="3401" />
                            <ext:Parameter Name="parameters" Value="[['gdg', #{hfGudang}.getValue(), 'System.Char'],
                                                          ['cusno', #{cbCustomerHdr}.getValue(), 'System.String'],
                                                          ['item', #{cbItemDtl}.getValue(), 'System.String'],
                                                          ['nosp', #{cbSpcDtl}.getValue(), 'System.String'],
                                                          ['@contains.c_batch.Contains(@0)', paramTextGetter(#{cbBatDtl}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="d_expired" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_batch" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_iteno" />
                                <ext:RecordField Name="c_batch" />
                                <ext:RecordField Name="d_expired" DateFormat="M$" Type="Date" />
                                <ext:RecordField Name="n_qtybatch" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 350px">
                        <tr>
                          <td class="body-panel">Batch</td>
                          <td class="body-panel">Kadaluarsa</td>
                          <td class="body-panel">Quantity/batch</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_batch}</td>
                            <td>{d_expired:this.formatDate}</td>
                            <td>{n_qtybatch}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                        <Functions>
                          <ext:JFunction Name="formatDate" Fn="myFormatDate" />
                        </Functions>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <Select Handler="selectedItemBatchPL(this, record, #{txQtyDtl}, #{cbItemDtl}, #{cbSpcDtl}, #{gridDetail}, #{hfExpire})" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:NumberField ID="txQtyDtl" runat="server" FieldLabel="Quantity" AllowNegative="false"
                      Width="75" AllowBlank="false" />
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <%--Indra 20181115FM ETD First--%>
                        <%--<Click Handler="storeToDetailGridMulti(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{cbSpcDtl}, #{hfTypDtl}, #{cbBatDtl}, #{txQtyDtl}, #{cbItemDtl}, #{cbSpcDtl}, #{hfExpire}, #{hfSPDate});" />--%>
                        <Click Handler="storeToDetailGridMulti(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{cbSpcDtl}, #{hfTypDtl}, #{cbBatDtl}, #{txQtyDtl}, #{cbItemDtl}, #{cbSpcDtl}, #{hfExpire}, #{hfSPDate}, #{hfETDDate});" />
                      </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnClear" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Clear"
                      Icon="Cancel">
                      <Listeners>
                        <Click Handler="#{frmpnlDetailEntry}.getForm().reset()" />
                      </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnDelete" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Delete Grid Selected "
                      Icon="ApplicationDelete">
                      <Listeners>
                        <Click Handler="deleteGrid(#{gridDetail});" />
                      </Listeners>
                    </ext:Button>
                  </Items>
                </ext:FormPanel>
              </Items>
            </ext:Toolbar>
          </TopBar>
          <Items>
            <ext:GridPanel ID="gridDetail" runat="server">
              <SelectionModel>
                <%--<ext:RowSelectionModel SingleSelect="true" />--%>
                <ext:CheckboxSelectionModel ID="CheckboxSelectionModelMemTypes" runat="server" />
              </SelectionModel>
              <Store>
                <ext:Store runat="server" RemotePaging="false" RemoteSort="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0002" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_plno = @0', #{hfPlNo}.getValue(), 'System.String']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itemdesc" />
                        <ext:RecordField Name="c_sp" />
                        <ext:RecordField Name="c_spc" />
                        <ext:RecordField Name="v_undes" />
                        <%--<ext:RecordField Name="c_type" />
                        <ext:RecordField Name="v_typedesc" />--%>
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="n_booked" Type="Float" />
                        <ext:RecordField Name="n_QtyRequest" Type="Float" />
                        <ext:RecordField Name="n_lastqtyRequest" Type="Float" />
                        <ext:RecordField Name="n_sisa" Type="Float" />
                        <ext:RecordField Name="d_expired" Type="Date" DateFormat="M$" />
                        <ext:RecordField Name="l_expired" Type="Boolean" />
                        <ext:RecordField Name="v_ket_ed" />
                        <ext:RecordField Name="c_acc_ed" />
                        <ext:RecordField Name="d_spdate" Type="Date" DateFormat="M$" />
                        <ext:RecordField Name="d_etdsp" Type="Date" DateFormat="M$" /><%--Indra 20181115FM ETD First--%>
                        <ext:RecordField Name="l_new" Type="Boolean" />
                        <ext:RecordField Name="l_void" Type="Boolean" />
                        <ext:RecordField Name="l_modified" Type="Boolean" />
                        <ext:RecordField Name="l_accmodify" Type="Boolean" />
                        <ext:RecordField Name="v_ket" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <ColumnModel>
                <Columns>
                  <ext:CommandColumn Width="25">
                    <Commands>
                      <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                      <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                    </Commands>
                    <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfPlNo}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itemdesc" Header="Nama Barang" Width="200" />
                  <ext:Column DataIndex="v_undes" Header="Kemasan" Width="80" />
                  <ext:Column DataIndex="c_spc" Header="SP Cabang" />
                  <ext:DateColumn ColumnID="d_spdate" DataIndex="d_spdate" Header="Tgl SP" Format="dd-MM-yyyy" Width="75" />
                  <ext:DateColumn ColumnID="d_etdsp" DataIndex="d_etdsp" Header="ETD" Format="dd-MM-yyyy" Width="75" /><%--Indra 20181115FM ETD First--%>
                  <%--<ext:Column DataIndex="v_typedesc" Header="Type" Width="50" />--%>
                  <ext:Column DataIndex="c_batch" Header="Batch" Width="90" />
                  <ext:NumberColumn DataIndex="n_booked" Header="Alokasi" Format="0.000,00/i" Width="75" />
                  <ext:NumberColumn DataIndex="n_QtyRequest" Header="Terpenuhi" Format="0.000,00/i"
                    Width="75">
                    <Editor>
                      <ext:NumberField ID="NumberField1" runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="false" DecimalPrecision="2" MinValue="0" />
                    </Editor>
                  </ext:NumberColumn>
                  <ext:DateColumn ColumnID="d_expired" DataIndex="d_expired" Header="Kadaluarsa" Format="dd-MM-yyyy" Width="75" />
                  <ext:CheckColumn DataIndex="l_expired" Header="ED" Width="45" />
                  <ext:Column DataIndex="v_ket_ed" Header="Alasan Acc" Width="150">
                      <Editor>
                        <ext:TextField ID="txtField1" runat="server" AllowBlank="true" />
                      </Editor>
                  </ext:Column>
                  <ext:Column DataIndex="c_acc_ed" Header="Nip Acc" Width="90" />
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="45" />
                </Columns>
              </ColumnModel>
              <Listeners>
                
                <Command Handler="if(command == 'Delete') { deleteRecordOnGridPL(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
                             
                <BeforeEdit Fn="onGridBeforeEdit" />
                <AfterEdit Fn="onGridAfterEdit" />
                
              </Listeners>
            </ext:GridPanel>
          </Items>
        </ext:Panel>
      </Center>
      <%--<South MinHeight="80" MaxHeight="80">
      </South>--%>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <ext:Button ID="btnPrint" runat="server" Icon="Printer" Text="Cetak">
      <DirectEvents>
        <Click OnEvent="Report_OnGenerate" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfPlNo}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="btnSave" runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfPlNo}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="btnReload" runat="server" Icon="Reload" Text="Bersihkan">
      <DirectEvents>
        <Click OnEvent="ReloadBtn_Click">
          <EventMask ShowMask="true" />
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />
