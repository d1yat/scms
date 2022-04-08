<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PackingListMasterBoxGd3.ascx.cs"
  Inherits="transaksi_penjualan_PackingListMasterBoxGd3" %>

<script type="text/javascript">

    var selectedSP = function(combo, rec, hfSPDate) {
        if (Ext.isEmpty(rec)) {
            ShowWarning(String.format("Record '{0}' tidak dapat di baca dari store.", value));
            combo.clearValue();
            return;
        }

        var getspdate = rec.get('d_spdate');
        hfSPDate.setValue(getspdate);
    }
    
  var setGroupStyle = function(view) {
    // get an instance of the Groups
    var groups = view.getGroups();

    for (var i = 0; i < groups.length; i++) {
      var spans = Ext.query("span", groups[i]);

      if (spans && spans.length > 0) {
        // Loop through the Groups, the do a query to find the <span> with our ColorCode
        // Get the "id" from the <span> and split on the "-", the second array item should be our ColorCode
        var color = "#" + spans[0].id.split("-")[1];

        // Set the "background-color" of the original Group node.
        Ext.get(groups[i]).setStyle("background-color", color);
      }
    }
  };

//  var selectedItemBatch = function(combo, rec, target, cbItm, cbSp) {
//    if (Ext.isEmpty(target)) {
//      ShowWarning("Objek target tidak terdefinisi.");
//      return;
//    }

//    if (!Ext.isEmpty(target)) {
//      target.setMinValue(0);
//      target.setMaxValue(0);
//    }

//    if (Ext.isEmpty(rec)) {
//      ShowWarning(String.format("Record '{0}' tidak dapat di baca dari store.", value));
//      combo.clearValue();
//      return;
//    }

//    var recItem = cbItm.findRecord(cbItm.valueField, cbItm.getValue());
//    if (Ext.isEmpty(recItem)) {
//      ShowWarning(String.format("Record item '{0}' tidak dapat di baca dari store.", cbItm.getText()));
//      return;
//    }

//    var qtySps = 0;
//    var recSp = cbSp.findRecord(cbSp.valueField, cbSp.getValue());
//    if (Ext.isEmpty(recSp)) {
//      var storSP = cbSp.getStore();
//      if (Ext.isEmpty(storSP)) {
//        ShowWarning('Store untuk SP tidak dapat terbaca.');
//        return;
//      }

//      storSP.each(function(r) {
//        qtySps += (r.get('n_spsisa') || 0);
//      });
//    }
//    else {
//      qtySps = recSp.get('n_spsisa');
//    }

//    var batCode = rec.get('c_batch');
//    var qtyBat = rec.get('n_qtybatch');
//    var qtySoh = 0; //(recItem.get('n_soh') || 0);

//    if (qtyBat <= 0.00) {
//      ShowWarning(String.format("Batch '{0}' tidak dapat dipergunakan karena <= 0.00", batCode));
//      combo.clearValue();
//      return false;
//    }

//    try {
//      target.setMinValue(0);

//      if (Ext.isNumber(qtyBat)) {
//        target.setMaxValue(qtyBat);
//      }
//      else {
//        target.setMaxValue(Number.MAX_VALUE);
//      }
//      if (Ext.isNumber(qtySps)) {
//        if (qtySps > qtyBat) {
//          if (Ext.isNumber(qtyBat)) {
//            target.setValue(qtyBat);
//          }
//          else {
//            target.setValue(0);
//          }
//        }
//        else {
//          target.setValue(qtySps);
//        }
//      }
//      else {
//        target.setValue(0);
//      }
//    }
//    catch (e) {
//      ShowError(e.toString());
//    }
//  }


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

  var storeToDetailGridMBox = function(frm, gridM1, gridM2, storeM3, gridR1, gridR2, storeR3, item, sp, tipe, batch, quantity, cbItm, cbSp, hfExpire, hfSPDate) {
      if (!frm.getForm().isValid()) {
          ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
          return;
      }

      if (Ext.isEmpty(gridM1) ||
          Ext.isEmpty(gridM2) ||
          Ext.isEmpty(storeM3) ||
          Ext.isEmpty(gridR1) ||
          Ext.isEmpty(gridR2) ||
          Ext.isEmpty(storeR3) ||
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

      var recItem = cbItm.findRecord(cbItm.valueField, cbItm.getValue());
      if (Ext.isEmpty(recItem)) {
          ShowWarning(String.format("Record item '{0}' tidak dapat di baca dari store.", cbItm.getText()));
          return;
      }

      var recSp = cbSp.findRecord(cbSp.valueField, cbSp.getValue());

      var storeM1 = gridM1.getStore();
      var storeM2 = gridM2.getStore();
   
      var storeR1 = gridR1.getStore();
      var storeR2 = gridR2.getStore();
      

      if (Ext.isEmpty(storeM1) ||
          Ext.isEmpty(storeM2) ||
          Ext.isEmpty(storeR1) ||
          Ext.isEmpty(storeR2)) {
          ShowWarning("Objek store tidak terdefinisi.");
          return;
      }


      var valX = undefined,
      fieldX = ['c_iteno', 'c_sp', 'c_batch'];
      var isDup = false,
      isDup2 = false,
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
      typeCode = tipe.getValue(),
      nBox = 0;

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

      var spdate = hfSPDate.getValue();

      spNo = sp.getValue().trim();
      spCab = sp.getText().trim();
      nBox = recItem.get('n_box');

      valX = [itemNo, spNo, bat];

      // Find Duplicate entry
//      isDup = findDuplicateEntryGrid(storeM2, fieldX, valX);
//      isDup2 = findDuplicateEntryGrid(storeR2, fieldX, valX);
      isDup = findDuplicateEntryGrid(storeM1, fieldX, valX);
      isDup2 = findDuplicateEntryGrid(storeR1, fieldX, valX);

      if (!isDup && !isDup2) {
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

          var qtyMaster = Math.floor(qty / nBox);
          var qtyReceh = qty % nBox;

          if (qtyMaster > 0) {
              if (validasiTotalPermintaan(gridM1.getStore(), 'c_iteno', 'c_sp', itemNo, spNo, qty, totalQty, totalQty)) {
                  storeM1.insert(0, new Ext.data.Record({
                      'c_iteno': itemNo,
                      'v_itemdesc': itemName,
                      'c_batch': bat,
                      'n_booked': (qtyMaster * nBox),
                      'n_QtyRequest': (qtyMaster * nBox),
                      'v_undes': recItem.get('v_undes'),
                      'n_box': nBox,
                      'l_box': true,
                      'd_expired': expdate,
                      'l_expired': statusED,
                      'l_new': true
                  }));

                  storeM2.insert(0, new Ext.data.Record({
                      'c_iteno': itemNo,
                      'v_itemdesc': itemName,
                      'c_sp': spNo,
                      'c_spc': spCab,
                      'c_batch': bat,
                      'n_booked': (qtyMaster * nBox),
                      'n_QtyRequest': (qtyMaster * nBox),
                      'v_undes': recItem.get('v_undes'),
                      'n_box': nBox,
                      'l_box': true,
                      'l_new': true,
                      'd_spdate': spdate
                  }));

                  storeM3.insert(0, new Ext.data.Record({
                      'c_iteno': itemNo,
                      'v_itemdesc': itemName,
                      'c_sp': spNo,
                      'c_spc': spCab,
                      'c_batch': bat,
                      'n_booked': (qtyMaster * nBox),
                      'n_QtyRequest': (qtyMaster * nBox),
                      'v_undes': recItem.get('v_undes'),
                      'n_box': nBox,
                      'l_box': true,
                      'l_new': true,
                      'd_spdate': spdate
                  }));

                  item.reset();
                  sp.reset();
                  batch.reset();
                  hfExpire.clear();
                  hfSPDate.clear();
              }
              else {
                  ShowError('Mohon diperiksa kembali, apakah jumlah yang telah diinput telah melebihi dari permintaan/SOH gudang.');
              }
          }

          if (qtyReceh > 0) {
              if (validasiTotalPermintaan(gridR1.getStore(), 'c_iteno', 'c_sp', itemNo, spNo, qty, totalQty, totalQty)) {
                  storeR1.insert(0, new Ext.data.Record({
                      'c_iteno': itemNo,
                      'v_itemdesc': itemName,
                      'c_batch': bat,
                      'n_booked': qtyReceh,
                      'n_QtyRequest': qtyReceh,
                      'v_undes': recItem.get('v_undes'),
                      'n_box': nBox,
                      'l_box': false,
                      'd_expired': expdate,
                      'l_expired': statusED,
                      'l_new': true
                  }));

                  storeR2.insert(0, new Ext.data.Record({
                      'c_iteno': itemNo,
                      'v_itemdesc': itemName,
                      'c_sp': spNo,
                      'c_spc': spCab,
                      'c_batch': bat,
                      'n_booked': qtyReceh,
                      'n_QtyRequest': qtyReceh,
                      'v_undes': recItem.get('v_undes'),
                      'n_box': nBox,
                      'l_box': false,
                      'l_new': true,
                      'd_spdate': spdate
                  }));

                  storeR3.insert(0, new Ext.data.Record({
                      'c_iteno': itemNo,
                      'v_itemdesc': itemName,
                      'c_sp': spNo,
                      'c_spc': spCab,
                      'c_batch': bat,
                      'n_booked': qtyReceh,
                      'n_QtyRequest': qtyReceh,
                      'v_undes': recItem.get('v_undes'),
                      'n_box': nBox,
                      'l_box': false,
                      'l_new': true,
                      'd_spdate': spdate
                  }));

                  item.reset();
                  sp.reset();
                  batch.reset();
                  hfExpire.clear();
                  hfSPDate.clear();
              }
              else {
                  ShowError('Mohon diperiksa kembali, apakah jumlah yang telah diinput telah melebihi dari permintaan/SOH gudang.');
              }
          }

          quantity.reset();
      }
      else {
          ShowError('Data telah ada.');

          return false;
      }
  }
  
  var prepareCommandsMaster = function(rec, toolbar, valX) {
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
  var checkForExistingDataInGridDetailMaster = function(cb, rec, grid) {
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
  
  var voidInsertedDataFromStoreMaster = function(rec) {
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
  var onChangeKategoriItemMaster = function(g) {
    var store = g.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
  }


  store.removeAll();
  store.commitChanges();
  }
  var onCheckAutoValidationMaster = function(isValid, cb) {
    //if (isValid && (comboGetSelectedIndex(cb) != -1)) {
    if (isValid) {
      return true;
    }
  }
  var onGridBeforeEditBoxMaster = function(e) {
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

  var onGridAfterEditBoxMaster = function(e) {
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
  }



  var updateTotal = function(grid) {
    var fbar = grid.getBottomToolbar(),
                column,
                field,
                width,
                data = {},
                c,
                cs = grid.view.getColumnData();

    for (var j = 0, jlen = grid.store.getCount(); j < jlen; j++) {
      var r = grid.store.getAt(j);

      for (var i = 0, len = cs.length; i < len; i++) {
        c = cs[i];
        column = grid.getColumnModel().columns[i];

        if (column.summaryType) {
          data[c.name] = Ext.grid.GroupSummary.Calculations[column.summaryType](data[c.name] || 0, r, c.name, data);
        }
      }
    }

    for (var i = 0; i < grid.getColumnModel().columns.length; i++) {
      column = grid.getColumnModel().columns[i];

      if (column.dataIndex != grid.store.groupField) {
        field = fbar.findBy(function(item) {
          return item.dataIndex === column.dataIndex;
        })[0];

        c = cs[i];
        fbar.remove(field, false);
        fbar.insert(i, field);
        width = grid.getColumnModel().getColumnWidth(i);
        field.setWidth(width - 5);
        field.setValue((column.summaryRenderer || c.renderer)(data[c.name], {}, {}, 0, i, grid.store));
      }
    }

    fbar.doLayout();
  }

  var applyFilter = function(grid, grid2) {
    var iBatch = grid.getSelected().data['c_batch'];
    var iItem = grid.getSelected().data['c_iteno'];
    var store = grid2.getStore();
      store.suspendEvents();
        store.filterBy(getRecordFilter(iBatch,iItem));                                
        store.resumeEvents();
        grid2.getView().refresh(false);
  }
  
  
        var filterString = function (value, dataIndex, record) {
                var val = record.get(dataIndex);
                
                if (typeof val != "string") {
                    return value.length == 0;
                }
                
                return val.toLowerCase().indexOf(value.toLowerCase()) > -1;
            };
 
            var filterDate = function (value, dataIndex, record) {
                var val = record.get(dataIndex).clearTime(true).getTime();
 
                if (!Ext.isEmpty(value, false) && val != value.clearTime(true).getTime()) {
                    return false;
                }
                return true;
            };
 
            var filterNumber = function (value, dataIndex, record) {
                var val = record.get(dataIndex);
 
                if (!Ext.isEmpty(value, false) && val != value) {
                    return false;
                }
                
                return true;
            };
 
            var getRecordFilter = function (iBatch, iItem) {
                var f = [];
 
                f.push({
                    filter: function (record) {                         
                        return filterString(iBatch, "c_batch", record);
                    }
                });
                 
                 f.push({
                    filter: function (record) {                         
                        return filterNumber(iItem, "c_iteno", record);
                    }
                });
                 
                var len = f.length;
                 
                return function (record) {
                    for (var i = 0; i < len; i++) {
                        if (!f[i].filter(record)) {
                            return false;
                        }
                    }
                    return true;
                };
              };

              var deleteRecordOnGridMasterNext = function(Next, grid, rec) {
                  var store = Next.getStore();
                  store.remove(rec);
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
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfGudang" runat="server" />
    <ext:Hidden ID="hfGudangDesc" runat="server" />
    <ext:Hidden ID="hfPlNoMaster" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Store ID="strMasterBox" runat="server" />
    <ext:Store ID="strMasterReceh" runat="server" />
    <ext:Hidden ID="hfExpire" runat="server" />
    <ext:Hidden ID="hfSPDate" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="150" MaxHeight="180" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="190" Layout="Fit"
          ButtonAlign="Center" MonitorValid="true">
          <Items>
            <ext:Panel ID="Panel1" runat="server" Padding="5" Layout="Column">
              <Items>
                <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" ColumnWidth=".5"
                  Layout="Form">
                  <Items>
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
                            <ext:Parameter Name="model" Value="3001" />
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
                        <Change Handler="clearRelatedComboRecursive(true, #{cbPrincipalHdr});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbPrincipalHdr" runat="server" FieldLabel="Pemasok" DisplayField="v_nama"
                      ValueField="c_nosup" Width="250" ItemSelector="tr.search-item" ListWidth="350"
                      MinChars="3" AllowBlank="false" ForceSelection="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="false" />
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
                            <ext:Parameter Name="parameters" Value="[['cusno', #{cbCustomerHdr}.getValue(), 'System.String'],
                        ['gdg', #{hfGudang}.getValue(), 'System.Char'],
                        ['@contains.v_nama.Contains(@0) || @contains.c_nosup.Contains(@0)', paramTextGetter(#{cbPrincipalHdr}), '']]"
                              Mode="Raw" />
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
                        <Change Handler="clearRelatedComboRecursive(true, #{cbItemDtl}, #{cbDivPrinsipal});checkForNovell(this,#{cbDivPrinsipal});" />                        
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbDivPrinsipal" runat="server" FieldLabel="Divisi Pemasok" ValueField="c_kddivpri"
                      DisplayField="v_nmdivpri" Width="250" ListWidth="350" 
                      PageSize="10" ItemSelector="tr.search-item" AllowBlank="false" ForceSelection="false" Hidden = "true">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="false" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store10" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="3101" />
                            <ext:Parameter Name="parameters" Value="[['cusno', #{cbCustomerHdr}.getValue(), 'System.String'],
                                        ['gdg', #{hfGudang}.getValue(), 'System.Char'],
                                        ['@contains.c_nosup.Contains(@0)', paramValueGetter(#{cbPrincipalHdr}), 'System.String'],
                                        ['@contains.c_kddivpri.Contains(@0) || @contains.v_nmdivpri.Contains(@0)', paramTextGetter(#{cbDivPrinsipal}), '']]" Mode="Raw" />
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
                      <Template ID="Template6" runat="server">
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
                        <Change Handler="clearRelatedComboRecursive(true, #{cbItemDtl});" />                                                
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbLantai" runat="server" FieldLabel="Lantai" DisplayField="v_ket"
                      ValueField="c_type" Width="150" AllowBlank="true" ForceSelection="false" MinChars="3">
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
                            <ext:Parameter Name="model" Value="2001" />
                            <ext:Parameter Name="parameters" Value="[['c_portal = @0', '9', 'System.Char'],
                                              ['c_notrans = @0', '003', '']]" Mode="Raw" />
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
                        <Change Handler="clearRelatedComboRecursive(true, #{cbKategori});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbKategori" runat="server" FieldLabel="Kategori" DisplayField="v_ket"
                      ValueField="c_type" Width="150" AllowBlank="true" ForceSelection="false" MinChars="3">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="true" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store2" runat="server" RemotePaging="false">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="allQuery" Value="true" />
                            <ext:Parameter Name="model" Value="2001" />
                            <ext:Parameter Name="parameters" Value="[['c_portal = @0', '9', 'System.Char'],
                                              ['c_notrans = @0', '001', '']]" Mode="Raw" />
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
                        
                      </Listeners>
                    </ext:ComboBox>
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
                                              ['c_type = @0', '05', '']]" Mode="Raw" />
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
                    <ext:TextField ID="txKeterangan" runat="server" FieldLabel="Keterangan" MaxLength="100"
                      Width="250" />
                    <ext:Label ID="lbChkConfirm" runat="server" FieldLabel="Konfirmasi" />
                  </Items>
                </ext:Panel>
              </Items>
            </ext:Panel>
          </Items>
          <Listeners>
            <ClientValidation Handler="#{btnAutoGen}.setDisabled(!valid);" />
          </Listeners>
          <Buttons>
            <ext:Button ID="btnAutoGen" runat="server" Text="Auto Generator" Icon="CogStart"
              Disabled="true">
              <DirectEvents>
                <Click OnEvent="AutoGenBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders});">
                  <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders});" ConfirmRequest="true"
                    Title="Proses ?" Message="Anda yakin ingin otomatis proses pemilihan item PL ?" />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="NumberID" Value="#{hfPlNoMaster}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="CustomerID" Value="#{cbCustomerHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="PrinsipalID" Value="#{cbPrincipalHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="KategoriID" Value="#{cbKategori}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="LantaiID" Value="#{cbLantai}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="DivPriID" Value="#{cbDivPrinsipal}.getValue()" Mode="Raw" />                    
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>
          </Buttons>
          <Listeners>
            <ClientValidation Handler="#{btnAutoGen}.setDisabled(!onCheckAutoValidationMaster(valid, #{cbPrincipalHdr}));" />
          </Listeners>
        </ext:FormPanel>
      </North>
      <Center MinHeight="150">
        <ext:Panel runat="server" Layout="Fit">
          <Items>
            <ext:Panel ID="Panel4" runat="server" Width="300" Border="false" Layout="Fit" BodyPadding="5">
              <Items>
                <ext:Panel ID="Panel5" runat="server" Layout="AccordionLayout" Margins="0 15">
                  <LayoutConfig>
                    <ext:AccordionLayoutConfig OriginalHeader="true" />
                  </LayoutConfig>
                  <Items>
                    <ext:FormPanel ID="pnlGridDetailBox" runat="server" Title="Daftar Items Master Box"
                      Layout="ColumnLayout">
                      <TopBar>
                        <ext:Toolbar ID="Toolbar1" runat="server">
                          <Items>
                            <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                              LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                              <Items>
                                <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" Width="200" ItemSelector="tr.search-item"
                                   ListWidth="350" DisplayField="v_itnam" ValueField="c_iteno" MinChars="3">
                                  <Store>
                                    <ext:Store ID="Store7" runat="server">
                                      <Proxy>
                                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                          CallbackParam="soaScmsCallback" />
                                      </Proxy>
                                      <BaseParams>
                                        <ext:Parameter Name="start" Value="={0}" />
                                        <ext:Parameter Name="limit" Value="-1" />
                                        <ext:Parameter Name="model" Value="3201" />
                                        <ext:Parameter Name="parameters" Value="[['gdg', #{hfGudang}.getValue(), 'System.Char'],
                                                                      ['cusno', #{cbCustomerHdr}.getValue(), 'System.String'],
                                                                      ['supl', #{cbPrincipalHdr}.getValue(), 'System.String'],
                                                                      ['itemLat', #{cbLantai}.getValue(), 'System.String'],
                                                                      ['itemCat', #{cbKategori}.getValue(), 'System.String'],
                                                                      ['divPri', #{cbDivPrinsipal}.getValue(), 'System.String'],                                                          
                                                                      ['@contains.c_iteno.Contains(@0) || @contains.v_itnam.Contains(@0)', paramTextGetter(#{cbItemDtl}), '']]"
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
                                            <ext:RecordField Name="v_undes" />
                                            <ext:RecordField Name="n_box" />
                                            <%--<ext:RecordField Name="n_SumPending" Type="Float" />
                                            <ext:RecordField Name="n_soh" Type="Float" />--%>
                                          </Fields>
                                        </ext:JsonReader>
                                      </Reader>
                                    </ext:Store>
                                  </Store>
                                  <Template ID="Template3" runat="server">
                                    <Html>
                                    <table cellpading="0" cellspacing="0" style="width: 350px">
                                    <tr>
                                      <td class="body-panel">Kode</td><td class="body-panel">Nama</td>
                                      <td class="body-panel">Kemasan</td><%--<td class="body-panel">Stok Gudang</td>--%>
                                      <td class="body-panel">Box</td>
                                    </tr>
                                    <tpl for="."><tr class="search-item">
                                      <td>{c_iteno}</td><td>{v_itnam}</td>
                                      <td>{v_undes}</td><%--<td>{n_soh}</td>--%>
                                      <td>{n_box}</td>
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
                                    <ext:Store ID="Store8" runat="server">
                                      <Proxy>
                                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                          CallbackParam="soaScmsCallback" />
                                      </Proxy>
                                      <BaseParams>
                                        <ext:Parameter Name="start" Value="={0}" />
                                        <ext:Parameter Name="limit" Value="-1" />
                                        <ext:Parameter Name="model" Value="3301" />
                                        <ext:Parameter Name="parameters" Value="[['cusno', #{cbCustomerHdr}.getValue(), 'System.String'],
                                                                      ['item', #{cbItemDtl}.getValue(), 'System.String'],
                                                                      ['@contains.c_spno.Contains(@0) || @contains.c_sp.Contains(@0)', paramTextGetter(#{cbSpcDtl}), '']]"
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
                                          </Fields>
                                        </ext:JsonReader>
                                      </Reader>
                                    </ext:Store>
                                  </Store>
                                  <Template ID="Template4" runat="server">
                                    <Html>
                                    <table cellpading="0" cellspacing="0" style="width: 450px">
                                    <tr>
                                      <td class="body-panel">No SP</td><td class="body-panel">SP Cabang</td>
                                      <td class="body-panel">Tipe</td><td class="body-panel">Tanggal</td>
                                      <td class="body-panel">Permintaan</td><td class="body-panel">Sisa</td>
                                    </tr>
                                    <tpl for="."><tr class="search-item">
                                        <td>{c_spno}</td><td>{c_sp}</td>
                                        <td>{v_SpType}</td><td>{d_spdate:this.formatDate}</td>
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
                                    <Select Handler="selectedSP(this, record, #{hfSPDate})" />
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
                                    <ext:Store ID="Store9" runat="server">
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
                                  <Template ID="Template5" runat="server">
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
                                    <Click Handler="storeToDetailGridMBox(#{frmpnlDetailEntry}, #{gridDetailMaster}, #{gridDetailMaster2}, #{strMasterBox}, #{gridDetailReceh}, #{gridDetailReceh2}, #{strMasterReceh}, #{cbItemDtl}, #{cbSpcDtl}, #{hfTypDtl}, #{cbBatDtl}, #{txQtyDtl}, #{cbItemDtl}, #{cbSpcDtl}, #{hfExpire}, #{hfSPDate});" />
                                  </Listeners>
                                </ext:Button>
                                <ext:Button ID="btnClear" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Clear"
                                  Icon="Cancel">
                                  <Listeners>
                                    <Click Handler="#{frmpnlDetailEntry}.getForm().reset()" />
                                  </Listeners>
                                </ext:Button>
                              </Items>
                            </ext:FormPanel>
                          </Items>
                        </ext:Toolbar>
                      </TopBar>
                      <Items>
                        <ext:Panel runat="server" ColumnWidth="0.76" Layout="FitLayout">
                          <Items>
                            <ext:GridPanel ID="gridDetailMaster" runat="server" >
                              <SelectionModel>
                                <ext:RowSelectionModel ID="RowSelectionModel1" runat="server">
                                  <Listeners>
                                    <RowSelect Handler="applyFilter(this, #{gridDetailMaster2});" />
                                  </Listeners>
                                </ext:RowSelectionModel>
                              </SelectionModel>
                              <Store>
                                <ext:Store ID="strMasterH" runat="server" RemotePaging="false" RemoteSort="false">
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
                                        <ext:RecordField Name="c_batch" />
                                        <ext:RecordField Name="n_booked" Type="Float" />
                                        <ext:RecordField Name="n_QtyRequest" Type="Float" />
                                        <ext:RecordField Name="n_lastqtyRequest" Type="Float" />
                                        <ext:RecordField Name="n_sisa" Type="Float" />
                                        <ext:RecordField Name="n_box" Type="Float" />
                                        <ext:RecordField Name="l_box" Type="Float" />
                                        <ext:RecordField Name="d_expired" Type="Date" DateFormat="M$" />
                                        <ext:RecordField Name="l_expired" Type="Boolean" />
                                        <ext:RecordField Name="v_ket_ed" />
                                        <ext:RecordField Name="l_new" Type="Boolean" />
                                        <ext:RecordField Name="l_void" Type="Boolean" />
                                        <ext:RecordField Name="l_modified" Type="Boolean" />
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
                                    <PrepareToolbar Handler="prepareCommandsMaster(record, toolbar, #{hfPlNo}.getValue());" />
                                  </ext:CommandColumn>
                                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                                  <ext:Column DataIndex="v_itemdesc" Header="Nama Barang" Width="200" />
                                  <ext:Column DataIndex="v_undes" Header="Kemasan" />
                                  <%--<ext:Column DataIndex="c_spc" Header="SP Cabang" />--%>
                                  <ext:Column DataIndex="c_batch" Header="Batch" />
                                  <ext:NumberColumn DataIndex="n_booked" Header="Alokasi" Format="0.000,00/i" Width="75" />
                                  <ext:NumberColumn DataIndex="n_QtyRequest" Header="Terpenuhi" Format="0.000,00/i"
                                    Width="75">
                                    <Editor>
                                      <ext:NumberField ID="NumberField1" runat="server" AllowBlank="false" AllowDecimals="true"
                                        AllowNegative="false" DecimalPrecision="2" MinValue="0" />
                                    </Editor>
                                  </ext:NumberColumn>
                                  <ext:NumberColumn DataIndex="n_box" Header="Satuan" Format="0.000,00/i" Width="75" />
                                  <ext:DateColumn ColumnID="d_expired" DataIndex="d_expired" Header="Kadaluarsa" Format="dd-MM-yyyy" />
                                  <ext:CheckColumn DataIndex="l_expired" Header="ED" Width="50" />
                                  <ext:Column DataIndex="v_ket_ed" Header="Alasan Acc" Width="150">
                                      <Editor>
                                        <ext:TextField ID="txtField1" runat="server" AllowBlank="true" />
                                      </Editor>
                                  </ext:Column>
                                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                                </Columns>
                              </ColumnModel>
                              <LoadMask ShowMask="true" />
                              <%--<View>
                                  <ext:GroupingView  
                                      ID="GroupingView1"
                                      HideGroupedColumn="true"
                                      runat="server" 
                                      ForceFit="true"
                                      GroupTextTpl='{text} ({[values.rs.length]} {[values.rs.length > 1 ? "Items" : "Item"]})'
                                      EnableRowBody="true">
                                      <GetRowClass Handler="var d = record.data; rowParams.body = String.format('<div style=\'padding:0 5px 5px 5px;\'>The {0} [{1}] requires light conditions of <i>{2}</i>.<br /><b>Price: {3}</b></div>', d.Common, d.Botanical, d.Light, Ext.util.Format.usMoney(d.Price));" />
                                  </ext:GroupingView>
                              </View>     
                              <Plugins>
                                <ext:RowExpander>
                                  <Component>
                                    <ext:Label runat="server" ID="lblRow" Text="Test" />
                                  </Component>
                                </ext:RowExpander>
                              </Plugins>--%>
                              <Listeners>
                                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStoreMaster(record); }" />
                                <BeforeEdit Fn="onGridBeforeEditBoxMaster" />
                                <AfterEdit Fn="onGridAfterEditBoxMaster" />
                              </Listeners>
                            </ext:GridPanel>
                          </Items>
                        </ext:Panel>
                        <ext:Panel runat="server" ColumnWidth="0.24" Layout="FitLayout">
                          <Items>
                            <ext:GridPanel ID="gridDetailMaster2" runat="server">
                              <SelectionModel>
                                <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" />
                              </SelectionModel>
                              <Store>
                                <ext:Store ID="strMasterD" runat="server" RemotePaging="false" RemoteSort="false">
                                  <Reader>
                                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                      <Fields>
                                        <ext:RecordField Name="c_iteno" />
                                        <ext:RecordField Name="v_itemdesc" />
                                        <ext:RecordField Name="c_sp" />
                                        <ext:RecordField Name="c_spc" />
                                        <ext:RecordField Name="v_undes" />
                                        <ext:RecordField Name="c_batch" />
                                        <ext:RecordField Name="n_booked" Type="Float" />
                                        <ext:RecordField Name="n_QtyRequest" Type="Float" />
                                        <ext:RecordField Name="n_lastqtyRequest" Type="Float" />
                                        <ext:RecordField Name="n_sisa" Type="Float" />
                                        <ext:RecordField Name="n_box" Type="Float" />
                                        <ext:RecordField Name="d_spdate" Type="Date" DateFormat="M$" />
                                        <ext:RecordField Name="l_box" Type="Float" />
                                        <ext:RecordField Name="l_new" Type="Boolean" />
                                        <ext:RecordField Name="l_void" Type="Boolean" />
                                        <ext:RecordField Name="l_modified" Type="Boolean" />
                                        <ext:RecordField Name="v_ket" />
                                      </Fields>
                                    </ext:JsonReader>
                                  </Reader>
                                </ext:Store>
                              </Store>
                              <ColumnModel>
                                <Columns>
                                  <ext:CommandColumn Width="25">
                                  </ext:CommandColumn>
                                  <%--<ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                                  <ext:Column DataIndex="v_itemdesc" Header="Nama Barang" Width="200" />
                                  <ext:Column DataIndex="v_undes" Header="Kemasan" />--%>
                                  <ext:Column DataIndex="c_spc" Header="SP Cabang" />
                                  <ext:DateColumn ColumnID="d_spdate" DataIndex="d_spdate" Header="Tgl SP" Format="dd-MM-yyyy" Width="75" />
                                  <%--<ext:Column DataIndex="c_batch" Header="Batch" />--%>
                                  <ext:NumberColumn DataIndex="n_booked" Header="Alokasi" Format="0.000,00/i" Width="75" />
                                  <ext:NumberColumn DataIndex="n_QtyRequest" Header="Terpenuhi" Format="0.000,00/i"
                                    Width="75" />
                                  <ext:NumberColumn DataIndex="n_box" Header="Satuan" Format="0.000,00/i" Width="75" />
                                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                                </Columns>
                              </ColumnModel>
                              <LoadMask ShowMask="true" />
                            </ext:GridPanel>
                          </Items>
                        </ext:Panel>
                      </Items>
                    </ext:FormPanel>
                    <ext:FormPanel ID="pnlGridDetailReceh" runat="server" Title="Daftar Items Receh"
                      Layout="ColumnLayout" >
                      <Items>
                      <ext:Panel runat="server" ColumnWidth="0.76" Layout="FitLayout">
                          <Items>
                        <ext:GridPanel ID="gridDetailReceh" runat="server">
                          <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true" >
                              <Listeners>
                                <RowSelect Handler="applyFilter(this, #{gridDetailReceh2});" />
                              </Listeners>
                            </ext:RowSelectionModel>
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
                                    <ext:RecordField Name="c_batch" />
                                    <ext:RecordField Name="n_booked" Type="Float" />
                                    <ext:RecordField Name="n_QtyRequest" Type="Float" />
                                    <ext:RecordField Name="n_lastqtyRequest" Type="Float" />
                                    <ext:RecordField Name="n_sisa" Type="Float" />
                                    <ext:RecordField Name="n_box" Type="Float" />
                                    <ext:RecordField Name="l_box" Type="Float" />
                                    <ext:RecordField Name="l_new" Type="Boolean" />
                                    <ext:RecordField Name="d_expired" Type="Date" DateFormat="M$" />
                                    <ext:RecordField Name="l_expired" Type="Boolean" />
                                    <ext:RecordField Name="v_ket_ed" />
                                    <ext:RecordField Name="l_void" Type="Boolean" />
                                    <ext:RecordField Name="l_modified" Type="Boolean" />
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
                                <PrepareToolbar Handler="prepareCommandsMaster(record, toolbar, #{hfPlNo}.getValue());" />
                              </ext:CommandColumn>
                              <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                              <ext:Column DataIndex="v_itemdesc" Header="Nama Barang" Width="200" />
                              <ext:Column DataIndex="v_undes" Header="Kemasan" />
                              <%--<ext:Column DataIndex="c_spc" Header="SP Cabang" />--%>
                              <ext:Column DataIndex="c_batch" Header="Batch" />
                              <ext:NumberColumn DataIndex="n_booked" Header="Alokasi" Format="0.000,00/i" Width="75" />
                              <ext:NumberColumn DataIndex="n_QtyRequest" Header="Terpenuhi" Format="0.000,00/i"
                                Width="75">
                                <Editor>
                                  <ext:NumberField ID="NumberField2" runat="server" AllowBlank="false" AllowDecimals="true"
                                    AllowNegative="false" DecimalPrecision="2" MinValue="0" />
                                </Editor>
                              </ext:NumberColumn>
                              <ext:NumberColumn DataIndex="n_box" Header="Satuan" Format="0.000,00/i" Width="75" />
                              <ext:DateColumn ColumnID="d_expired" DataIndex="d_expired" Header="Kadaluarsa" Format="dd-MM-yyyy" />
                              <ext:CheckColumn DataIndex="l_expired" Header="ED" Width="50" />
                              <ext:Column DataIndex="v_ket_ed" Header="Alasan Acc" Width="150">
                                  <Editor>
                                    <ext:TextField ID="TextField1" runat="server" AllowBlank="true" />
                                  </Editor>
                              </ext:Column>
                              <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                            </Columns>
                          </ColumnModel>
                          <Listeners>
                            <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); deleteRecordOnGridMasterNext (#{gridDetailReceh2}, this, record) } else if (command == 'Void') { voidInsertedDataFromStoreMaster(record); }" />
                            <BeforeEdit Fn="onGridBeforeEditBoxMaster" />
                            <AfterEdit Fn="onGridAfterEditBoxMaster" />
                            
                          </Listeners>
                        </ext:GridPanel>
                          </Items>
                        </ext:Panel>
                        <ext:Panel runat="server" ColumnWidth="0.24" Layout="FitLayout">
                          <Items>
                            <ext:GridPanel ID="gridDetailReceh2" runat="server">
                              <SelectionModel>
                                <ext:RowSelectionModel ID="RowSelectionModel3" runat="server" />
                              </SelectionModel>
                              <Store>
                                <ext:Store runat="server" RemotePaging="false" RemoteSort="false">
                                  <Reader>
                                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                      <Fields>
                                        <ext:RecordField Name="c_iteno" />
                                        <ext:RecordField Name="v_itemdesc" />
                                        <ext:RecordField Name="c_sp" />
                                        <ext:RecordField Name="c_spc" />
                                        <ext:RecordField Name="v_undes" />
                                        <ext:RecordField Name="c_batch" />
                                        <ext:RecordField Name="n_booked" Type="Float" />
                                        <ext:RecordField Name="n_QtyRequest" Type="Float" />
                                        <ext:RecordField Name="n_lastqtyRequest" Type="Float" />
                                        <ext:RecordField Name="n_sisa" Type="Float" />
                                        <ext:RecordField Name="n_box" Type="Float" />
                                        <ext:RecordField Name="d_spdate" Type="Date" DateFormat="M$" />
                                        <ext:RecordField Name="l_box" Type="Float" />
                                        <ext:RecordField Name="l_new" Type="Boolean" />
                                        <ext:RecordField Name="l_void" Type="Boolean" />
                                        <ext:RecordField Name="l_modified" Type="Boolean" />
                                        <ext:RecordField Name="v_ket" />
                                      </Fields>
                                    </ext:JsonReader>
                                  </Reader>
                                </ext:Store>
                              </Store>
                              <ColumnModel>
                                <Columns>
                                  <ext:CommandColumn Width="25">
                                    
                                  </ext:CommandColumn>
                                  <%--<ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                                  <ext:Column DataIndex="v_itemdesc" Header="Nama Barang" Width="200" />
                                  <ext:Column DataIndex="v_undes" Header="Kemasan" />--%>
                                  <ext:Column DataIndex="c_spc" Header="SP Cabang" />
                                  <ext:DateColumn ColumnID="d_spdate" DataIndex="d_spdate" Header="Tgl SP" Format="dd-MM-yyyy" Width="75" />
                                  <%--<ext:Column DataIndex="c_batch" Header="Batch" />--%>
                                  <ext:NumberColumn DataIndex="n_booked" Header="Alokasi" Format="0.000,00/i" Width="75" />
                                  <ext:NumberColumn DataIndex="n_QtyRequest" Header="Terpenuhi" Format="0.000,00/i"
                                    Width="75" />
                                  <ext:NumberColumn DataIndex="n_box" Header="Satuan" Format="0.000,00/i" Width="75" />
                                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                                </Columns>
                              </ColumnModel>
                              <LoadMask ShowMask="true" />
                            </ext:GridPanel>
                          </Items>
                        </ext:Panel>
                      </Items>
                    </ext:FormPanel>
                  </Items>
                </ext:Panel>
              </Items>
            </ext:Panel>
          </Items>
        </ext:Panel>
      </Center>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <ext:Button ID="btnPrint" runat="server" Icon="Printer" Text="Cetak">
      <DirectEvents>
        <Click OnEvent="Report_OnGenerate" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetailMaster});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetailMaster});"
            ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfPlNoMaster}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="btnSave" runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetailMaster});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetailMaster});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValuesMaster" Value="saveStoreToServer(#{gridDetailMaster}.getStore())" Mode="Raw" />
            <ext:Parameter Name="strMasterBox" Value="saveStoreToServer(#{strMasterBox})" Mode="Raw" />
            <ext:Parameter Name="gridValuesReceh" Value="saveStoreToServer(#{gridDetailReceh}.getStore())" Mode="Raw" />
            <ext:Parameter Name="strMasterReceh" Value="saveStoreToServer(#{strMasterReceh})" Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfPlNoMaster}.getValue()" Mode="Raw" />
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
