<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TransferGudangCtrl.ascx.cs"
  Inherits="transaksi_transfer_TransferGudangCtrl" %>

<script type="text/javascript">

//selectedItemBatchTGCtrl(this, record, #{txQtyDtl}, #{cbItemDtl}, #{cbSpgDtl}, #{gridDetail}, #{hfExpire})
    var selectedItemBatchTGCtrl = function(combo, rec, target, cbItm, cbSp, grid, hfExpire) {
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
                qtySps += (r.get('n_sisa') || 0);
            });
        }
        else {
            qtySps = recSp.get('n_sisa');
        }

        var batCode = rec.get('c_batch');
        var qtyBat = rec.get('n_soh');
        var qtySoh = 0; //(recItem.get('n_soh') || 0);
        var qtyBox = rec.get('n_box');

        if (qtyBox > 0) {
            qtySps = Math.floor((qtySps / qtyBox)) * qtyBox;
            qtyBat = Math.floor((qtyBat / qtyBox)) * qtyBox;
        }

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
                    var iSp = store.data.items[nLen].data.c_spgno;
                    var iBatch = store.data.items[nLen].data.c_batch;
                    if (iTm == ItemVal && iSp == spVal) {
                        nQtySpMin = store.data.items[nLen].data.n_booked;
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
                        if (nQtyBatch > qtyBat) {
                            nQtyBatch -= qtyBat
                        }
                        else {
                            nQtyBatch = nHasil
                        }
                        target.setValue(nQtyBatch);
                    }
                }
            }
        }
        catch (e) {
            ShowError(e.toString());
        }
    }

  var validasiTotalPermintaan = function(store, itemNo, alokasi, soh, nAlokasi, MaxSPG, batchNo) {
//    var total = alokasi;
    var idx = 0;
    var rec = '';
    var batch = '';
    var qty = 0;
//    var total = 0;
//    var totalReq = 0;
    var totalbatch = 0;

    if (store.getCount() < 1) {
      return true;
    }

    do {
      idx = store.findExact('c_iteno', itemNo, idx);
      if (idx != -1) 
      {
        rec = store.getAt(idx);
        if (!Ext.isEmpty(rec)) 
        {
          batch = rec.data.c_batch
          qty = rec.data.n_gqty;
//          total += qty;
          if(batch == batchNo)
          {
            totalbatch += qty
          }
          //          spc = rec.get('c_sp').trim();
          //          if (spc == spCode) {
          //            total += rec.get('n_booked');
          //          }
          
        }
        idx++;
      }
    } while (idx != -1);

//    totalReq = total + alokasi;
    totalbatch += alokasi;
    if (MaxSPG < alokasi) {
      return false;
    }
    else {
//      if (totalReq > MaxSPG) {
//        return false;
//      }
      if (totalbatch > soh) {
        return false;
      }
    }
    //        if (total > totalReq) {
    //          return false;
    //        }
    //        else if (total > totalSoh) {
    //          return false;
    //        }



    return true;
  }

  var storeToDetailGridTGCtrl9 = function(frm, grid, item, sg, batch, quantity, hfExpire) {

      if (!frm.getForm().isValid()) {
          ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
          return;
      }

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

      if (sg.getText().trim() == '') {
          if (Ext.isEmpty(grid) ||
            Ext.isEmpty(item) ||
            Ext.isEmpty(batch) ||
            Ext.isEmpty(quantity) ||
            Ext.isEmpty(hfExpire)) {
              ShowWarning("Objek tidak terdefinisi.");
              return;
          }

          var store = grid.getStore();

          if (Ext.isEmpty(store)) {
              ShowWarning("Objek store tidak terdefinisi.");
              return;
          }

          var storeSG = sg.getStore();
          var datar = new Array();
          var jsonDataEncode = "";
          var records = storeSG.getRange();

          var bat = batch.getValue().trim();
          var godqty = quantity.getValue();
          var itemNo = item.getValue().trim();
          var alokasi = 0;

          for (var i = 0; i < records.length; i++) {
              if (godqty >= 0) {
                  if (godqty >= records[i].data['n_sisa']) {
                      alokasi = records[i].data['n_sisa'];
                  }
                  else {
                      if (godqty > 0) {
                          alokasi = godqty
                      }
                      else {
                          alokasi = 0;
                      }
                  }
              }
              else {
                  alokasi = 0;
              }

              if (alokasi > 0) {
                  var valX = [itemNo, records[i].data['c_spgno']];
                  var fieldX = ['c_iteno', 'c_spgno'];

                  var isDup = findDuplicateEntryGrid(store, fieldX, valX);
                  if (!isDup) {

                      store.insert(0, new Ext.data.Record({
                          'c_iteno': itemNo,
                          'c_batch': bat,
                          'v_itnam': item.getText(),
                          'n_booked': alokasi,
                          'n_gqty': alokasi,
                          'c_spgno': records[i].data['c_spgno'],
                          'd_expired': expdate,
                          'l_expired': statusED,
                          'l_new': true
                      }));
                      godqty -= records[i].data['n_sisa'];

                  }
              }
          }

          item.reset();
          quantity.reset();
          batch.reset();
          hfExpire.clear();

          return;
      }

      if (Ext.isEmpty(grid) ||
          Ext.isEmpty(item) ||
          Ext.isEmpty(sg) ||
          Ext.isEmpty(batch) ||
          Ext.isEmpty(quantity)) {
          ShowWarning("Objek tidak terdefinisi.");
          return;
      }

      var store = grid.getStore();
      if (Ext.isEmpty(store)) {
          ShowWarning("Objek store tidak terdefinisi.");
          return;
      }

      var valX = [item.getValue(), sg.getValue(), batch.getValue()];
      var fieldX = ['c_iteno', 'c_spgno', 'c_batch'];

      var isDup = findDuplicateEntryGrid(store, fieldX, valX);

      if (!isDup) {
          var bat = batch.getValue().trim();
          var godqty = quantity.getValue();

          if (godqty <= 0) {
              ShowWarning("Quantity tidak boleh 0.");
              return;
          }

          var itemNo = item.getValue().trim();
          var spg = sg.getValue().trim();

          var storeCombo = sg.getStore();
          var datar = new Array();
          var records = storeCombo.getRange();
          var terAlokasi = 0;
          var nAlokasi = 0;
          var MaxSPG = 0

          for (var i = 0; i < grid.store.data.items.length; i++) {
              if (grid.store.data.items[i].data['c_spgno'] == spg) {
                  terAlokasi += grid.store.data.items[i].data['n_gqty'];
              }
          }

          nAlokasi = terAlokasi + godqty;

          var soh = batch.findRecord('c_batch', bat).data.n_soh;

          var record = sg.findRecord(sg.valueField || sg.displayField, spg);


          MaxSPG = record.json['n_sisa'];
          if (validasiTotalPermintaan(store, itemNo, godqty, soh, nAlokasi, MaxSPG, bat)) {
              store.insert(0, new Ext.data.Record({
                  'c_iteno': itemNo,
                  'c_batch': bat,
                  'v_itnam': item.getText(),
                  'n_booked': godqty,
                  'n_gqty': godqty,
                  'c_spgno': spg,
                  'd_expired': expdate,
                  'l_expired': statusED,
                  'l_new': true
              }));

              soh -= godqty;

              item.reset();
              quantity.reset();
              batch.reset();
              sg.reset();
              hfExpire.clear();
          } else {
              ShowError('Mohon diperiksa kembali, apakah jumlah yang telah diinput telah melebihi dari permintaan/SOH gudang.');
          }
      }
      else {
          ShowError('Data telah ada.');

          return false;
      }

  }

  var prepareCommandsTGCtrl = function(rec, toolbar, valX, conf) {
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
      if (conf == 'true') {
        vd.setVisible(false);
      }
      else {
        vd.setVisible(true);
      }
    }
  }

  var voidInsertedDataFromStoreTGCtrl = function(rec) {
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

  var beforeEditDataComboGrid = function(e) {
    if (e.field == 'v_ket_type_dc') {
      if (e.record.get('l_new') || e.record.get('l_void')) {
        e.cancel = true;
      }
      else if ((!e.record.get('l_new')) && (!e.record.get('l_void')) && (!e.record.get('l_modified')) && (e.record.get('n_gqty') <= 0)) {
        e.cancel = true;
      }
    }
    else if (e.field == 'n_gqty') {
      if ((!e.record.get('l_new')) && (!e.record.get('l_void')) && (!e.record.get('l_modified')) && (e.value <= 0)) {
        e.cancel = true;
      }
    }
  }

  var beforeEditDataConfirm = function(e) {
    if (e.field == 'v_ket_type_dc') {
      if (e.record.get('l_new') || e.record.get('l_void')) {
        e.cancel = true;
      }
      else if ((!e.record.get('l_new')) && (!e.record.get('l_void')) && (!e.record.get('l_modified')) && (e.record.get('n_gqty') <= 0)) {
        e.cancel = true;
      }
    }
    else if (e.field == 'n_gqty') {
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
              e.record.set('n_gqty', 0);
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
    else if (e.field == 'n_gqty') {
      var nBook = e.record.get('n_booked');
      if (e.value > nBook) {
        e.value = nBook;
        e.record.set('n_gqty', nBook);
      }
      else if (e.value < 0) {
        e.value = nBook;
        e.record.set('n_gqty', nBook);
      }
    }
    else if (e.field == 'v_ket_ed' && e.record.get('l_new') == false) {
        var lExpired = e.record.get('l_expired');
        if (lExpired == true) {
            e.record.set('l_accmodify', true);
        }
    }
  }

  var onChangeKategoriItem = function(g) {
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
    }
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725">
  <Content>
    <ext:Hidden ID="hfGudang" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfGudangDesc" runat="server" />
    <ext:Hidden ID="hfConfMode" runat="server" />
    <ext:Hidden ID="hfSJNo" runat="server" />
    <ext:Hidden ID="hfExpire" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North Collapsible="false" MinHeight="180" MaxHeight="180">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="180"
          Layout="Fit" ButtonAlign="Center" MonitorValid="true" MinHeight="180">
          <Items>
            <ext:Panel ID="Panel1" runat="server" Padding="5" Layout="Column">
              <Items>
                <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" ColumnWidth=".5"
                  Layout="Form">
                  <Items>
                    <%--<ext:ComboBox ID="cbFromHdr" runat="server" FieldLabel="Asal" DisplayField="v_gdgdesc"
                      ValueField="c_gdg" Width="175" PageSize="10" ListWidth="200" ItemSelector="tr.search-item"
                      MinChars="3" AllowBlank="false">
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
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="allQuery" Value="true" />
                            <ext:Parameter Name="model" Value="2031" />
                            <ext:Parameter Name="parameters" Value="[[]]" Mode="Raw" />
                            <ext:Parameter Name="sort" Value="v_gdgdesc" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_gdg" Root="d.records" SuccessProperty="d.success" TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_gdg" />
                                <ext:RecordField Name="v_gdgdesc" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 200px">
                              <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                              <tpl for=".">
                                <tr class="search-item">
                                  <td>{c_gdg}</td><td>{v_gdgdesc}</td>
                                </tr>
                              </tpl>
                              </table>
                        </Html>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                      </Listeners>
                    </ext:ComboBox>--%>
                    <ext:ComboBox ID="cbTipeSJ" runat="server" FieldLabel="Tipe SJ" DisplayField="v_ket"
                      ValueField="c_type" Width="150" AllowBlank="false" ForceSelection="false" MinChars="3">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="false" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store4" runat="server" RemotePaging="false">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="allQuery" Value="true" />
                            <ext:Parameter Name="model" Value="2001" />
                            <ext:Parameter Name="parameters" Value="[['c_portal = @0', '9', 'System.Char'],
                                              ['c_notrans = @0', '006', '']]" Mode="Raw" />
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
                      <DirectEvents>
                        <Change OnEvent="SelectedTipeSJ_Change">
                          <EventMask ShowMask="true" />
                        </Change >
                      </DirectEvents>
                    </ext:ComboBox>
                    <ext:Label ID="lbGudangFrom" runat="server" FieldLabel="Asal" />
                    <ext:ComboBox ID="cbToHdr" runat="server" FieldLabel="Tujuan" DisplayField="v_gdgdesc"
                      ValueField="c_gdg" Width="175" PageSize="10" ListWidth="200" ItemSelector="tr.search-item"
                      MinChars="3" AllowBlank="false" ForceSelection="false">
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
                            <%--<ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />--%>
                            <ext:Parameter Name="allQuery" Value="true" />
                            <ext:Parameter Name="model" Value="0176" />
                            <ext:Parameter Name="parameters" Value="[['c_gdg != @0', #{hfGudang}.getValue(), 'System.Char']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="v_gdgdesc" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_gdg" Root="d.records" SuccessProperty="d.success" TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_gdg" />
                                <ext:RecordField Name="v_gdgdesc" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 200px">
                              <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                              <tpl for=".">
                                <tr class="search-item">
                                  <td>{c_gdg}</td><td>{v_gdgdesc}</td>
                                </tr>
                              </tpl>
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
                      ValueField="c_nosup" Width="250" ItemSelector="tr.search-item" ListWidth="250"
                      MinChars="3" AllowBlank="true" ForceSelection="false">
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
                            <ext:Parameter Name="model" Value="4001" />
                            <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean'],
                                        ['l_hide = @0', false, 'System.Boolean'],
                                        ['gdgFrom', #{hfGudang}.getValue(), 'System.Char'],
                                        ['@contains.c_nosup.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbPrincipalHdr}), '']]"
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
                      <Template ID="Template6" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 250px">
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
                        <Change Handler="clearRelatedComboRecursive(true, #{cbItemDtl});" />
                      </Listeners>
                    </ext:ComboBox>                    
                  </Items>
                </ext:Panel>
                <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" ColumnWidth=".5"
                  Layout="Form">
                  <Items>
                    <ext:ComboBox ID="cbLantai" runat="server" FieldLabel="Lantai" DisplayField="v_ket"
                      ValueField="c_type" Width="150" AllowBlank="true" ForceSelection="false" MinChars="3">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="true" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store3" runat="server" RemotePaging="false">
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
                        <ext:Store ID="Store1" runat="server" RemotePaging="false">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="allQuery" Value="true" />
                            <ext:Parameter Name="model" Value="2001" />
                            <ext:Parameter Name="parameters" Value="[['c_portal = @0', '9', 'System.Char'],
                                              ['c_notrans = @0', '001', 'System.String']]" Mode="Raw" />
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
                        <Change Handler="onChangeKategoriItem(#{gridDetail});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:TextField ID="txKeterangan" runat="server" FieldLabel="Keterangan" MaxLength="100"
                      Width="250" />
                    <ext:Checkbox ID="chkConfirm" runat="server" FieldLabel="Confirm" />
                  </Items>
                </ext:Panel>
              </Items>
            </ext:Panel>
          </Items>
          <Buttons>
            <ext:Button ID="btnAutoGen" runat="server" Text="Auto Generator" Icon="CogStart" Disabled="true">
              <DirectEvents>
                <Click OnEvent="AutoGenBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders});">
                  <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders});" ConfirmRequest="true"
                    Title="Proses ?" Message="Anda yakin ingin otomatis proses pemilihan item SJ ?" />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="NumberID" Value="#{hfSJNo}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="gdgToID" Value="#{cbToHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="PrinsipalID" Value="#{cbPrincipalHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="KategoriID" Value="#{cbKategori}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="LantaiID" Value="#{cbLantai}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="TipeSJ" Value="#{cbTipeSJ}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>
          </Buttons>
          <Listeners>
            <ClientValidation Handler="#{btnAutoGen}.setDisabled(!onCheckAutoValidation(valid, #{cbKategori}));" />
          </Listeners>
        </ext:FormPanel>
      </North>
      <Center MinHeight="150">
        <ext:Panel runat="server" Title="Daftar Items" Height="150" Layout="Fit">
          <TopBar>
            <ext:Toolbar runat="server">
              <Items>
                <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                  LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                  <Items>
                    <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" Width="200" ItemSelector="tr.search-item"
                      ListWidth="350" DisplayField="v_itnam" ValueField="c_iteno" AllowBlank="false"
                      ForceSelection="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="false" />
                      </CustomConfig>
                      <Store>
                        <ext:Store runat="server" AutoLoad="false">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="-1" />
                            <ext:Parameter Name="model" Value="4101" />
                            <ext:Parameter Name="parameters" Value="[['gdgTo', #{cbToHdr}.getValue(), 'System.String'],
                                                                    ['gdgFrom', #{hfGudang}.getValue(), 'System.Char'],
                                                                    ['nosup', #{cbPrincipalHdr}.getValue(), 'System.String'],
                                                                    ['itemCat', #{cbKategori}.getValue(), 'System.String'],
                                                                    ['itemLat', #{cbLantai}.getValue(), 'System.String'],
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
                                <ext:RecordField Name="n_box" />
                                <%--<ext:RecordField Name="n_sgpending" />
                                <ext:RecordField Name="n_soh" />--%>
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 350px">
                        <tr>
                        <td class="body-panel">Kode</td><td class="body-panel">Nama</td><td class="body-panel">Box</td>
                        <%--<td class="body-panel">SG Pending</td><td class="body-panel">Stock</td>--%>
                        </tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_iteno}</td><td>{v_itnam}</td><td>{n_box}</td>
                            <%--<td>{n_sgpending}</td><td>{n_soh}</td>--%>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <Change Handler="clearRelatedComboRecursiveMulti(true, #{cbSpgDtl}, #{cbBatDtl});" />
                        <%--<Change Handler="clearRelatedComboRecursive(true, #{cbBatDtl});" />--%>
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbSpgDtl" runat="server" FieldLabel="SP Gudang" ItemSelector="tr.search-item"
                      ListWidth="300" DisplayField="c_spgno" ValueField="c_spgno" AllowBlank="true">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="true" />
                      </CustomConfig>
                      <Store>
                        <ext:Store runat="server" AutoLoad="false">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="-1" />
                            <ext:Parameter Name="model" Value="4201" />
                            <ext:Parameter Name="parameters" Value="[['gdgTo', #{cbToHdr}.getValue(), 'System.Char'],
                                ['iteno', #{cbItemDtl}.getValue(), 'System.String'],
                                ['@contains.c_spgno.Contains(@0)', paramTextGetter(#{cbSpgDtl}), '']]" Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_spgno" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_spgno" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_spgno" />
                                <ext:RecordField Name="d_spgdate" DateFormat="M$" Type="Date" />
                                <ext:RecordField Name="n_sisa" Type="Int" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 300px">
                        <tr>
                          <td class="body-panel">No SG</td>
                          <td class="body-panel">Tgl SG</td>
                          <td class="body-panel">Sisa SG</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_spgno}</td><td>{d_spgdate:this.formatDate}</td>
                            <td>{n_sisa}</td>
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
                        <Change Handler="clearRelatedComboRecursive(true, #{cbBatDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbBatDtl" runat="server" FieldLabel="Batch" ItemSelector="tr.search-item"
                      ListWidth="500" DisplayField="c_batch" ValueField="c_batch" AllowBlank="false"
                      ForceSelection="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="false" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="StoreBatch" runat="server" AutoLoad="false">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="-1" />
                            <ext:Parameter Name="model" Value="4301" />
                            <ext:Parameter Name="parameters" Value="[['gdg', #{hfGudang}.getValue(), 'System.Char'],
                                                          ['spgno', #{cbSpgDtl}.getValue(), 'System.String'],
                                                          ['iteno', #{cbItemDtl}.getValue(), 'System.String'],
                                                          ['tipeSJ', #{cbTipeSJ}.getValue(), 'System.String'],
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
                                <ext:RecordField Name="n_soh" />
                                <ext:RecordField Name="n_box" />
                                <%--<ext:RecordField Name="n_sohTot" />--%>
                                <%--<ext:RecordField Name="n_pending" />--%>
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 500px">
                        <tr>
                          <td class="body-panel">Batch</td>
                          <td class="body-panel">Kadaluarsa</td>
                          <td class="body-panel">Quantity/batch</td>
                          <td class="body-panel">Box</td>
                          <%--<td class="body-panel">Total stok</td>
                          <td class="body-panel">Sisa permintaan</td>--%>
                        </tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_batch}</td>
                            <td>{d_expired:this.formatDate}</td>
                            <td>{n_soh:this.formatNumber}</td>
                            <td>{n_box}</td>                            
                            <%--<td>{n_sohTot}</td>
                            <td>{n_pending}</td>--%>
                        </tr></tpl>
                        </table>
                        </Html>
                        <Functions>
                          <ext:JFunction Name="formatDate" Fn="myFormatDate" />
                          <ext:JFunction Name="formatNumber" Fn="myFormatNumber" />
                        </Functions>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <Select Handler="selectedItemBatchTGCtrl(this, record, #{txQtyDtl}, #{cbItemDtl}, #{cbSpgDtl}, #{gridDetail}, #{hfExpire})" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:NumberField ID="txQtyDtl" runat="server" FieldLabel="Quantity" AllowNegative="false"
                      AllowDecimals="true" DecimalPrecision="2" Width="75" AllowBlank="false" />
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGridTGCtrl9(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{cbSpgDtl}, #{cbBatDtl}, #{txQtyDtl}, #{hfExpire});" />
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
            <ext:GridPanel ID="gridDetail" runat="server" Layout="Fit">
              <SelectionModel>
                <ext:RowSelectionModel SingleSelect="true" />
              </SelectionModel>
              <Store>
                <ext:Store ID="Store6" runat="server" RemotePaging="false" RemoteSort="false" AutoLoad="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0004" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_sjno = @0', #{hfSJNo}.getValue(), 'System.String']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="c_spgno" />
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="v_ket_type_dc" />
                        <ext:RecordField Name="n_QtyRequest" Type="Float" />
                        <ext:RecordField Name="n_booked" Type="Float" />
                        <ext:RecordField Name="n_gqty" Type="Float" />
                        <ext:RecordField Name="d_expired" Type="Date" DateFormat="M$" />
                        <ext:RecordField Name="l_expired" Type="Boolean" />
                        <ext:RecordField Name="v_ket_ed" />
                        <ext:RecordField Name="c_acc_ed" />
                        <ext:RecordField Name="l_void" Type="Boolean" />
                        <ext:RecordField Name="l_modified" Type="Boolean" />
                        <ext:RecordField Name="l_new" Type="Boolean" />
                        <ext:RecordField Name="l_accmodify" Type="Boolean" />
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
                    <PrepareToolbar Handler="prepareCommandsTGCtrl(record, toolbar, #{hfSJNo}.getValue(), #{hfConfMode}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="250" />
                  <ext:Column DataIndex="c_spgno" Header="SP Gudang" />
                  <ext:Column DataIndex="c_batch" Header="Batch" />
                  <ext:NumberColumn DataIndex="n_booked" Header="Alokasi" Format="0.000,00/i" Width="75" />
                  <ext:NumberColumn DataIndex="n_gqty" Header="Terpenuhi" Format="0.000,00/i" Width="75" />
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
                  </ext:Column>
                  <ext:DateColumn ColumnID="d_expired" DataIndex="d_expired" Header="Kadaluarsa" Format="dd-MM-yyyy" />
                  <ext:CheckColumn DataIndex="l_expired" Header="ED" Width="50" />
                  <ext:Column DataIndex="v_ket_ed" Header="Alasan Acc" Width="150">
                    <Editor>
                      <ext:TextField ID="txtField1" runat="server" AllowBlank="true" />
                    </Editor>
                  </ext:Column>
                  <ext:Column DataIndex="c_acc_ed" Header="Nip Acc" Width="100" />
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStoreTGCtrl(record); }" />
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
            <ext:Parameter Name="NumberID" Value="#{hfSJNo}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Disk" Text="Simpan" ID="btnSave">
      <DirectEvents>
        <Click OnEvent="btnSimpan_OnClick" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
          <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini."
            BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});" />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfSJNo}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GudangID" Value="#{hfGudang}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GudangDesc" Value="#{hfGudangDesc}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GudangID2" Value="#{cbToHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GudangDesc2" Value="#{cbToHdr}.getText()" Mode="Raw" />
            <ext:Parameter Name="Keterangan" Value="#{txKeterangan}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Supplier" Value="paramTextGetter(#{cbPrincipalHdr})" Mode="Raw" />
            <ext:Parameter Name="TypeCategory" Value="#{cbKategori}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Reload" Text="Bersihkan">
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
