<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PackingListConfirmCtrlGd3.ascx.cs"
  Inherits="transaksi_penjualan_PackingListConfirmCtrlGd3" %>

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

    var selectedItemBatchConfirm = function(combo, rec, target, cbItm, cbSp, grid, hfExpire) {
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

    var store = grid.getStore();
    var BatchVal = combo.getValue();
    var ItemVal = cbItm.getValue();
    var spVal = cbSp.getValue();

    if (qtyBat <= 0.00) {
      //ShowWarning("Batch '" + value + "' tidak dapat di baca dari store.");
      ShowWarning(String.format("Batch '{0}' tidak dapat dipergunakan karena <= 0.00", batCode));
      combo.clearValue();
      return false;
    }
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
          }
          else {
            target.setValue(0);
          }
        }
        else {
          target.setValue(qtySps);
        }
      }
      else {
        target.setValue(0);
      }
    }
    catch (e) {
      ShowError(e.toString());
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
  //  var validasiEditTotalPermintaanConfirm = function(grid) {
  //    if (grid.selModel.getSelections().length > 0) {
  //      var rec = grid.selModel.getSelections()[0];
  //      if (!Ext.isEmpty(rec)) {
  //        var store = grid.getStore();
  //        var itemNo = rec.get('c_iteno');
  //        var spNo = rec.get('c_sp');
  //        var qty = rec.get('n_booked');
  //        var reqQty = rec.get('n_QtyRequest');

  //        if (qty > 0.00) {
  //          if (validasiTotalPermintaanConfirm(store, 'c_iteno', 'c_sp', itemNo, spNo, 0, reqQty)) {
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
  //  }
  var validasiTotalPermintaanConfirm = function(store, fieldItem, fieldSp, itemCode, spCode, inpQty, totalReq, totalSoh) {
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
            total += rec.get('n_qty');
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
  //var storeToDetailGridConfirmMultiConfirm = function(frm, grid, item, sp, tipe, batch, quantity, cbItm, cbSp, hfExpire, hfSPDate) {
  var storeToDetailGridConfirmMultiConfirm = function(frm, grid, item, sp, tipe, batch, quantity, cbItm, cbSp, hfExpire, hfSPDate, hfETDDate) {
      if (!frm.getForm().isValid()) {
          ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
          return;
      }

      if (Ext.isEmpty(grid) ||
          Ext.isEmpty(item) ||
          Ext.isEmpty(sp) ||
          Ext.isEmpty(tipe) ||
          Ext.isEmpty(batch) ||
          Ext.isEmpty(quantity) ||
          Ext.isEmpty(hfExpire)) {
          ShowWarning("Objek tidak terdefinisi.");
          return;
      }

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
                      //if (validasiTotalPermintaanConfirm(grid.getStore(), 'c_iteno', 'c_sp', itemNo, spNo, qty, reqQty, qty)) {
                      if (validasiTotalPermintaanConfirm(grid.getStore(), 'c_iteno', 'c_sp', itemNo, spNo, qty, reqQty, totalQty)) {
                          store.insert(0, new Ext.data.Record({
                              'c_iteno': itemNo,
                              'v_itemdesc': itemName,
                              'c_sp': spNo,
                              'c_spc': spCab,
                              'c_type': typeCode,
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

          if (validasiTotalPermintaanConfirm(grid.getStore(), 'c_iteno', 'c_sp', itemNo, spNo, qty, totalQty, totalQty)) {
              store.insert(0, new Ext.data.Record({
                  'c_iteno': itemNo,
                  'v_itemdesc': itemName,
                  'c_sp': spNo,
                  'c_spc': spCab,
                  'c_type': typeCode,
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
  //  var storeToDetailGridConfirm = function(frm, grid, item, sp, tipe, batch, quantity, cbItm, cbSp) {
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

  //      if (validasiTotalPermintaanConfirm(grid.getStore(), 'c_iteno', 'c_sp', itemNo, spNo, qty, reqQty, totalQty)) {
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
  var prepareCommandsConfirm = function(rec, toolbar, valX) {
    var del = toolbar.items.get(0); // delete button
    //    var vd = toolbar.items.get(1); // void button

    var isNew = false;

    if (!Ext.isEmpty(rec)) {
      isNew = rec.get('l_new');
    }

    if (Ext.isEmpty(valX) || isNew) {
      del.setVisible(true);
      //      vd.setVisible(false);
    }
    else {
      del.setVisible(false);
      //      vd.setVisible(true);
    }
  }
  var checkForExistingDataInGridDetailConfirm = function(cb, rec, grid) {
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
  var voidInsertedDataFromStoreConfirm = function(rec) {
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

  var beforeEditDataConfirm = function(e) {
    if (e.field == 'v_ket_type_dc') {
      if (e.record.get('l_new') || e.record.get('l_void')) {
        e.cancel = true;
      }
      else if ((!e.record.get('l_new')) && (!e.record.get('l_void')) && (!e.record.get('l_modified')) && (e.record.get('n_QtyRequest') <= 0)) {
        e.cancel = true;
      }
    }
    else if (e.field == 'n_QtyRequest') {
      if ((!e.record.get('l_new')) && (!e.record.get('l_void')) && (!e.record.get('l_modified')) && (e.value <= 0)) {
        e.cancel = true;
      }
    }
  }

  var afterEditDataConfirm = function(e, cb) {
    if (e.field == 'v_ket_type_dc') {
      var stor = cb.getStore();

      if (!Ext.isEmpty(stor)) {
        var rec = stor.getById(e.value);

        if (!Ext.isEmpty(rec)) {
          switch (e.value) {
            case '03':
            case '04':
              e.record.set('n_QtyRequest', 0);
              break;
          }
          e.record.set('c_type_dc', e.value);
          e.record.set('v_ket_type_dc', rec.get('v_ket'));
          e.record.set('l_modified', true);

          return;
        }
      }

          e.record.set('c_type_dc', '');
          e.record.set('v_ket_type_dc', '');
      }
      else if (e.field == 'n_QtyRequest') {
          var nBook = e.record.get('n_booked');
          if (e.value > nBook) {
              e.value = nBook;
              e.record.set('n_QtyRequest', nBook);
          }
          else if (e.value < 0) {
              e.value = nBook;
              e.record.set('n_QtyRequest', nBook);
          }
      }
      else if (e.field == 'v_ket_ed' && e.record.get('l_new') == false) {
          var lExpired = e.record.get('l_expired');
          if (lExpired == true) {
          e.record.set('l_accmodify', true);
          }
      }
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="1024" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfGudang" runat="server" />
    <ext:Hidden ID="hfGudangDesc" runat="server" />
    <ext:Hidden ID="hfPlNoConfirm" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfExpire" runat="server" />
    <ext:Hidden ID="hfSPDate" runat="server" />   
    <ext:Hidden ID="hfETDDate" runat="server" /><%--Indra 20181115FM ETD First--%>
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="175" MaxHeight="175" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="225" Padding="10">
          <Items>
            <%--<ext:ComboBox ID="cbCustomerHdr" runat="server" FieldLabel="Cabang" DisplayField="v_cunam"
              ValueField="c_cusno" Width="350" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
              MinChars="3" AllowBlank="false" ForceSelection="false">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
              </CustomConfig>
              <Store>
                <ext:Store runat="server">
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
              <Template runat="server">
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
            </ext:ComboBox>--%>
            <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Cabang">
              <Items>
                <ext:Label ID="lbCustomerHdr" runat="server" />
                <ext:Hidden ID="hfCustomerHdr" runat="server" />
              </Items>
            </ext:CompositeField>
            <%--<ext:ComboBox ID="cbPrincipalHdr" runat="server" FieldLabel="Pemasok" DisplayField="v_nama"
              ValueField="c_nosup" Width="350" ItemSelector="tr.search-item" PageSize="10" ListWidth="350"
              MinChars="3" AllowBlank="false" ForceSelection="false">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
              </CustomConfig>
              <Store>
                <ext:Store runat="server">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                    <ext:Parameter Name="model" Value="3101" />
                    <ext:Parameter Name="parameters" Value="[['cusno', #{hfCustomerHdr}.getValue(), 'System.String'],
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
              <Template runat="server">
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
                <BeforeSelect Handler="return checkForExistingDataInGridDetailConfirm(this, record, #{gridDetail});" />
                <Change Handler="clearRelatedComboRecursive(true, #{cbItemDtl});" />
              </Listeners>
            </ext:ComboBox>--%>
            <ext:CompositeField ID="CompositeField2" runat="server" FieldLabel="Pemasok">
              <Items>
                <ext:Label ID="lbPrinsipalHdr" runat="server" />
                <ext:Hidden ID="hfPrinsipalHdr" runat="server" />
              </Items>
            </ext:CompositeField>
            <ext:TextField ID="txKeterangan" runat="server" FieldLabel="Keterangan" MaxLength="100"
              Width="400" />
            <ext:CompositeField runat="server" FieldLabel="Kategori">
              <Items>
                <ext:Label ID="lbItemCatHdr" runat="server" />
                <ext:Hidden ID="hfItemCatHdr" runat="server" />
              </Items>
            </ext:CompositeField>
            <ext:ComboBox ID="cbTipeHdr" runat="server" FieldLabel="Tipe" DisplayField="v_ket"
              ValueField="c_type" Width="150" AllowBlank="false" ForceSelection="false">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
              </CustomConfig>
              <Store>
                <ext:Store runat="server" RemotePaging="false">
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
            </ext:ComboBox>
            <ext:ComboBox ID="cbViaHdr" runat="server" FieldLabel="Via" DisplayField="v_ket"
              ValueField="c_type" Width="125" AllowBlank="false" ForceSelection="false">
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
            <ext:Checkbox ID="chkConfirm" runat="server" FieldLabel="Confirm" />
          </Items>
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
                      PageSize="10" ListWidth="350" DisplayField="v_itnam" ValueField="c_iteno" AllowBlank="true"
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
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="3201" />
                            <ext:Parameter Name="parameters" Value="[['gdg', #{hfGudang}.getValue(), 'System.Char'],
                                                          ['cusno', #{hfCustomerHdr}.getValue(), 'System.String'],
                                                          ['supl', #{hfPrinsipalHdr}.getValue(), 'System.String'],
                                                          ['itemCat', #{hfItemCatHdr}.getValue(), 'System.String'],
                                                          ['isconf', 'true', 'System.Boolean'],
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
                                <ext:RecordField Name="v_nama" />
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
                          <%--<td class="body-panel">Jumlah Qty SP Tertunda</td><td class="body-panel">Stok Gudang</td>--%>
                        </tr>
                        <tpl for="."><tr class="search-item">
                          <td>{c_iteno}</td><td>{v_itnam}</td>
                          <%--<td>{n_SumPending}</td><td>{n_soh}</td>--%>
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
                      PageSize="10" ListWidth="450" DisplayField="c_sp" ValueField="c_spno" AllowBlank="true"
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
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="3301" />
                            <ext:Parameter Name="parameters" Value="[['cusno', #{hfCustomerHdr}.getValue(), 'System.String'],
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
                      PageSize="10" ListWidth="350" DisplayField="c_batch" ValueField="c_batch" AllowBlank="true"
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
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="3401" />
                            <ext:Parameter Name="parameters" Value="[['gdg', '6', 'System.Char'],
                                                          ['cusno', #{hfCustomerHdr}.getValue(), 'System.String'],
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
                        <Select Handler="selectedItemBatchConfirm(this, record, #{txQtyDtl}, #{cbItemDtl}, #{cbSpcDtl}, #{gridDetail}, #{hfExpire})" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:NumberField ID="txQtyDtl" runat="server" FieldLabel="Quantity" AllowNegative="false"
                      Width="75" AllowBlank="false" />
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <%--Indra 20181115FM ETD First--%>
                        <%--<Click Handler="storeToDetailGridConfirmMultiConfirm(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{cbSpcDtl}, #{hfTypDtl}, #{cbBatDtl}, #{txQtyDtl}, #{cbItemDtl}, #{cbSpcDtl}, #{hfExpire}, #{hfSPDate});" />--%>
                        <Click Handler="storeToDetailGridConfirmMultiConfirm(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{cbSpcDtl}, #{hfTypDtl}, #{cbBatDtl}, #{txQtyDtl}, #{cbItemDtl}, #{cbSpcDtl}, #{hfExpire}, #{hfSPDate}, #{hfETDDate});" />
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
            <ext:GridPanel ID="gridDetail" runat="server">
              <SelectionModel>
                <ext:RowSelectionModel SingleSelect="true" />
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
                    <ext:Parameter Name="parameters" Value="[['c_plno = @0', #{hfPlNoConfirm}.getValue(), 'System.String']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itemdesc" />
                        <ext:RecordField Name="c_sp" />
                        <ext:RecordField Name="c_spc" />
                        <ext:RecordField Name="c_type" />
                        <ext:RecordField Name="v_typedesc" />
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
                        <ext:RecordField Name="c_type_dc" />
                        <ext:RecordField Name="v_ket_type_dc" />
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
                      <%--<ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />--%>
                    </Commands>
                    <PrepareToolbar Handler="prepareCommandsConfirm(record, toolbar, #{hfPlNoConfirm}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itemdesc" Header="Nama Barang" Width="200" />
                  <ext:Column DataIndex="c_spc" Header="SP Cabang" />
                  <ext:DateColumn ColumnID="d_spdate" DataIndex="d_spdate" Header="Tgl SP" Format="dd-MM-yyyy" Width="75" />
                  <ext:DateColumn ColumnID="d_etdsp" DataIndex="d_etdsp" Header="ETD" Format="dd-MM-yyyy" Width="75" /><%--Indra 20181115FM ETD First--%>
                  <%--<ext:Column DataIndex="v_typedesc" Header="Type" Width="50" />--%>
                  <ext:Column DataIndex="c_batch" Header="Batch" Width="90" />
                  <ext:NumberColumn DataIndex="n_booked" Header="Alokasi" Format="0.000,00/i" Width="75" />
                  <ext:NumberColumn DataIndex="n_QtyRequest" Header="Terpenuhi" Format="0.000,00/i"
                    Width="75" />
                  <ext:Column DataIndex="v_ket_type_dc" Header="Batal" Width="100">
                    <Editor>
                      <ext:ComboBox ID="cbTypeDcGrd" runat="server" DisplayField="v_ket" ValueField="c_type"
                        ForceSelection="false" MinChars="3" AllowBlank="true">
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
                              <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                              ['c_notrans = @0', '60', '']]" Mode="Raw" />
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
                    </Editor>
                    <%--<Renderer Handler="return renderDataComboGrid(#{cbTypeDcGrd});" />--%>
                  </ext:Column>
                  <ext:DateColumn ColumnID="d_expired" DataIndex="d_expired" Header="Kadaluarsa" Format="dd-MM-yyyy" />
                  <ext:CheckColumn DataIndex="l_expired" Header="ED" Width="50" />
                  <ext:Column DataIndex="v_ket_ed" Header="Alasan Acc" Width="150">
                      <Editor>
                        <ext:TextField ID="txtField1" runat="server" AllowBlank="true" />
                      </Editor>
                  </ext:Column>
                  <ext:Column DataIndex="c_acc_ed" Header="Nip Acc" Width="100" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); }" />
                <AfterEdit Handler="afterEditDataConfirm(e, #{cbTypeDcGrd});" />
                <BeforeEdit Handler="beforeEditDataConfirm(e);" />
              </Listeners>
            </ext:GridPanel>
          </Items>
        </ext:Panel>
      </Center>
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
            <ext:Parameter Name="NumberID" Value="#{hfPlNoConfirm}.getValue()" Mode="Raw" />
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
            <ext:Parameter Name="NumberID" Value="#{hfPlNoConfirm}.getValue()" Mode="Raw" />
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
