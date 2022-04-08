<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TransferGudangPemusnahanCtrl.ascx.cs"
  Inherits="transaksi_transfer_TransferGudangPemusnahanCtrl" %>

<script type="text/javascript">

  var selectedItemBatchPemusnahanstd = function(combo, rec, txbqty, cbItm, grid) {
  if (Ext.isEmpty(txbqty)) {
      ShowWarning("Objek txbqty tidak terdefinisi.");
      return;
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

  var batCode = rec.get('c_batch');
  var qtyBat = (rec.get('N_BSISA') || 0);

  if (qtyBat <= 0.00) {
      ShowWarning(String.format("Batch '{0}' tidak dapat di pergunakan karena <= 0.00", batCode));
      combo.clearValue();
      return false;
  }
  var qtySisaReal = recItem.get('n_sisa');
  var itm = recItem.get('c_iteno');
  var store = grid.getStore();
  var qtyexist = 0;
  var qtyakhir = 0;
  var qtysisacalc = 0;

  var qtySisa = (qtyBat > qtySisaReal ? qtySisaReal : qtyBat);

  try {
      for (nLen = 0; nLen < store.data.length; nLen++) {
          if (store.data.items[nLen].data.c_iteno == itm) {
              qtyexist += store.data.items[nLen].data.n_bqty;
          }
      }

      if (qtyexist == 0) {
          txbqty.setMinValue(0);

          if (Ext.isNumber(qtySisa)) {
              txbqty.setMaxValue(qtySisa);
          }
          else {
              txbqty.setMaxValue(Number.MAX_VALUE);
          }

          if (Ext.isNumber(qtySisa)) {
              txbqty.setValue(qtySisa);
          }
          else {
              txbqty.setValue(0);
          }
      }
      else if (qtySisa != qtyexist) {
          txbqty.setMinValue(0);
          qtysisacalc = qtySisaReal - qtyexist;

          if (qtysisacalc <= qtyBat) {
              qtyakhir = qtysisacalc;
          }
          else {
              qtyakhir = qtyBat;
          }

          txbqty.setMaxValue(qtyakhir);
          txbqty.setValue(qtyakhir);
      }
      else if (qtySisa == qtyexist) {
          ShowWarning("Jumlah item sudah terpenuhi");
          combo.clearValue();
          return false;
      }
      else {
          ShowWarning(String.format("Batch '{0}' tidak dapat di pergunakan karena qty tidak sesuai", batCode));
          combo.clearValue();
          return false;
        }
    }
    catch (e) {
        ShowError(e.toString());
    }
}

  var validasiTotalPermintaanPemusnahan = function(store, fieldItem, fieldSpg, itemCode, spCode, inpQty, totalSoh) {
    var total = inpQty;
    var idx = 0;
    var rec = '';
    var spc = '';

    do {
      idx = store.findExact(fieldItem, itemCode, idx);
      if (idx != -1) {
        rec = store.getAt(idx);
        if (!Ext.isEmpty(rec)) {
          spc = rec.get('c_spgno').trim();
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

  var validasiTotalPermintaanPemusnahan = function(store, itemNo, good, bad, gsoh, bsoh) {
    var gIn = good;
    var bIn = bad;
    var idx = 0;
    var rec = '';
    var qty = 0;
    var totalReqBad = 0;
    var totalReqGood = 0;
    var totalBad = 0;
    var totalGood = 0;
    var total = 0;

    if (store.getCount() < 1) {
      return true;
    }

    do {
      idx = store.findExact('c_iteno', itemNo, idx);
      if (idx != -1) {
        rec = store.getAt(idx);
        if (!Ext.isEmpty(rec)) {
          qty = rec.data.n_gqty;
          totalGood += qty;
          qty = 0;
          qty = rec.data.n_bqty;
          totalBad += qty;
        }
        idx++;
      }
    } while (idx != -1);

    totalReqBad = total + bIn;
    totalReqGood = total + gIn;

    if (totalReqBad > bsoh) {
      return false;
    }
    if (totalReqGood > gsoh) {
      return false;
    }

    return true;
  }

  var storeToDetailGridPemusnahan = function(frm, grid, item, batch, Bquantity) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }


    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(item) ||
          Ext.isEmpty(batch) ||
          Ext.isEmpty(Bquantity)) {
      ShowWarning("Objek tidak terdefinisi.");
      return;
    }

    var store = grid.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
    }

    var valX = [item.getValue(), batch.getValue()];
    var fieldX = ['c_iteno', 'c_batch'];

    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

    if (!isDup) {
      var bat = batch.getValue().trim();
      var badqty = Bquantity.getValue();
      var itemNo = item.getValue().trim();
      var badSoh = batch.findRecord('c_batch', bat).data.N_BSISA;

      if (validasiTotalPermintaanPemusnahan(store, itemNo, badqty, badSoh)) {
          store.insert(0, new Ext.data.Record({
            'c_iteno': itemNo,
            'c_batch': bat,
            'v_itnam': item.getText(),
            'n_booked_bad': badqty,
            'n_bqty': badqty,
            'l_new': true
          }));

          item.reset();
          Bquantity.reset();
          batch.reset();
      } else {
        ShowError('Mohon diperiksa kembali, apakah jumlah yang telah diinput telah melebihi dari permintaan/SOH gudang.');
      }
    }
    else {
      ShowError('Data telah ada.');

      return false;
    }

  }

  var prepareCommandsPemusnahan = function(rec, toolbar, valX, conf) {
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

  var voidInsertedDataFromStorePemusnahan = function(rec) {
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

var changeAsalstd = function(cb, lbCbg, cbCabang, cbMemo, txNodok, hfType, cbItem, cbBat, txQty) {
    var store = cb.getStore();
    if (!Ext.isEmpty(store)) {
        var idx = cb.selectedIndex;
        cbItem.reset();
        cbBat.reset();
        txQty.reset();
        txNodok.reset();

        if (idx == 0 || idx == 1) {
            var r = store.getAt(idx);
            cbCabang.setVisible(false);
            cbCabang.setValue(" ")
            cbMemo.setVisible(true);
            cbMemo.setValue(" ")

            if (idx == 0) {
                txNodok.disable();
                lbCbg.setVisible(true);
                cbCabang.setVisible(false);
                cbCabang.setValue(" ")
                hfType.setValue("memo");
            }
            else {
                txNodok.enable();
                lbCbg.setVisible(false);
                cbCabang.setVisible(false);
                cbCabang.setValue(" ")
                cbMemo.setVisible(false);
                cbMemo.setValue(" ")
                hfType.setValue("regular");
            }
        }
        else {
            lbCbg.setVisible(true);
            cbCabang.setVisible(true);
            cbCabang.enable();
            cbCabang.setValue("")
            cbMemo.setVisible(false);
            cbMemo.setValue(" ")
            txNodok.enable();
            hfType.setValue("regular");
        }
    }
}

var checkForExistingDataInGridDetailstd = function(cb, rec, grid) {
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

    var typecb = cb.valueField;
    var newPrinc = null;

    if (typecb == "c_type") {
        newPrinc = (rec.get('c_type')).toString().trim();
    }
    else {
        newPrinc = (rec.get('c_mpno')).toString().trim();
    }

    var oldPric = cb.getValue().trim();

    if (newPrinc == oldPric) {
        return;
    }

    var len = store.getModifiedRecords().length;

    if (len > 0) {
        ShowWarning('Maaf, anda tidak dapat mengganti asal, jika telah ada data didalam grid detail.');
        return false;
    }
}

var changeMemostd = function(cb, txNodok) {

    var store = cb.getStore();
    if (!Ext.isEmpty(store)) {
        var mpno = Ext.getCmp('<%= cbMemoHdr.ClientID %>');

        //var mpno = (rec.get('c_mpno')).toString().trim();

        txNodok.setValue(mpno.getValue());
    }
}


</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="640">
  <Content>
    <ext:Hidden ID="hfGudang" runat="server" />
    <ext:Hidden ID="hfGudangDesc" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfConfMode" runat="server" />
    <ext:Hidden ID="hfSJNo" runat="server" />
    <ext:Hidden ID="hfmode" runat="server" />
    <ext:Hidden ID="hfType" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North Collapsible="false" MinHeight="175" MaxHeight="350">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Width="400" Height="200"
          Padding="10">
          <Items>
            <ext:Label ID="lbGudangFrom" runat="server" FieldLabel="Asal" />
            <ext:ComboBox ID="cbToHdr" runat="server" FieldLabel="Tujuan" DisplayField="v_gdgdesc"
              ValueField="c_gdg" Width="175" PageSize="10" ListWidth="200" ItemSelector="tr.search-item"
              MinChars="3" AllowBlank="false">
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
                    <%--<ext:Parameter Name="start" Value="={0}" />
                          <ext:Parameter Name="limit" Value="={10}" />--%>
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0178" />
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
              <Template ID="Template1" runat="server">
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
                <Change Handler="resetEntryWhenChange(#{gridDetail});" />
              </Listeners>
            </ext:ComboBox>
            <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Asal Produk">
              <Items>
                <ext:ComboBox ID="cbAsalProduk" runat="server" FieldLabel="Asal Produk" ValueField="c_type"
                  DisplayField="v_ket" Width="200" AllowBlank="false">
                  <Store>
                    <ext:Store ID="Store2" runat="server">
                      <Proxy>
                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                          CallbackParam="soaScmsCallback" />
                      </Proxy>
                      <BaseParams>
                        <ext:Parameter Name="allQuery" Value="true" />
                        <ext:Parameter Name="model" Value="2001" />
                        <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                                  ['c_notrans = @0', '62', '']]" Mode="Raw" />
                        <ext:Parameter Name="sort" Value="c_type" />
                        <ext:Parameter Name="dir" Value="ASC" />
                      </BaseParams>
                      <Reader>
                        <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success" TotalProperty="d.totalRows">
                          <Fields>
                            <ext:RecordField Name="c_type" />
                            <ext:RecordField Name="v_ket" />
                          </Fields>
                        </ext:JsonReader>
                      </Reader>
                    </ext:Store>
                  </Store>
                  <Triggers>
                    <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
                  </Triggers>
                  <Listeners>
                    <Select Handler="this.triggers[0].show();" />
                    <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                    <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); }" />
                    <BeforeSelect Handler="return checkForExistingDataInGridDetailstd(this, record, #{gridDetail});" />
                    <Change Handler="changeAsalstd(this, #{lbCbg}, #{txCabang}, #{cbMemoHdr}, #{txNoDok}, #{hfType}, #{cbItemDtl}, #{cbBatDtl}, #{txBQtyDtl});" />
                  </Listeners>
                </ext:ComboBox>
                <ext:Label ID="lbCbg" runat="server" Text="-" Hidden="true" />
                <ext:TextField ID="txCabang" FieldLabel="Cabang/Ekspedisi" runat="server" MaxLength="100"
                Width="400" AllowBlank="false" Hidden="true" >
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="false" />
                  </CustomConfig>
                </ext:TextField>
                <ext:ComboBox FieldLabel="No Memo" runat="server" ID="cbMemoHdr" DisplayField="c_memo"
                  ValueField="c_mpno" Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="500"
                  MinChars="3" AllowBlank="false" ForceSelection="false" Hidden="true">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="false" />
                  </CustomConfig>
                  <Store>
                    <ext:Store ID="Store3" runat="server">
                      <Proxy>
                        <ext:ScriptTagProxy CallbackParam="soaScmsCallback" Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson"
                          Timeout="10000000" />
                      </Proxy>
                      <BaseParams>
                        <ext:Parameter Name="start" Value="={0}" />
                        <ext:Parameter Name="limit" Value="10" />
                        <ext:Parameter Name="model" Value="9033" />
                        <ext:Parameter Name="parameters" Value="[['@contains.c_mpno.Contains(@0) || @contains.c_memo.Contains(@0)', paramTextGetter(#{cbMemoHdr}), '']]"
                          Mode="Raw" />
                        <ext:Parameter Name="sort" Value="c_mpno" />
                        <ext:Parameter Name="dir" Value="DESC" />
                      </BaseParams>
                      <Reader>
                        <ext:JsonReader IDProperty="c_mpno" Root="d.records" SuccessProperty="d.success"
                          TotalProperty="d.totalRows">
                          <Fields>
                            <ext:RecordField Name="c_mpno" />
                            <ext:RecordField Name="c_memo" />
                            <ext:RecordField Name="d_mpdate" Type="Date" DateFormat="M$" />
                          </Fields>
                        </ext:JsonReader>
                      </Reader>
                    </ext:Store>
                  </Store>
                  <Template ID="tmpPL" runat="server">
                    <Html>
                    <table cellpading="0" cellspacing="1" style="width: 500px">
                    <tr><td class="body-panel">Kode</td><td class="body-panel">Memo</td><td class="body-panel">Tanggal</td></tr>
                    <tpl for="."><tr class="search-item">
                        <td>{c_mpno}</td>
                        <td>{c_memo}</td>
                        <td>{d_mpdate:this.formatDate}</td>
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
                    <BeforeSelect Handler="return checkForExistingDataInGridDetailstd(this, record, #{gridDetail});" />
                    <Change Handler="clearRelatedComboRecursive(true, #{cbItemDtl});" />
                    <Change Handler="changeMemostd(this, #{txNoDok});" />
                  </Listeners>
                </ext:ComboBox>
              </Items>
            </ext:CompositeField>
            <ext:TextField ID="txNoDok" runat="server" FieldLabel="No. Dokumen" MaxLength="100"
              Width="400" />
            <ext:TextField ID="txKet" runat="server" FieldLabel="Keterangan" MaxLength="100"
              Width="400" />
            <ext:Checkbox ID="chkConfirm" runat="server" FieldLabel="Confirm" />
          </Items>
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
                      PageSize="10" ListWidth="500" DisplayField="v_itnam" ValueField="c_iteno" AllowBlank="false">
                      <Store>
                        <ext:Store runat="server" AutoLoad="false">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="4304" />
                            <ext:Parameter Name="parameters" Value="[['memo', #{cbMemoHdr}.getValue(), 'System.String'],
                                  ['type', #{hfType}.getValue(), 'System.String'],
                                  ['l_aktif = @0', true, 'System.Boolean'],
                                  ['l_hide = @0', false, 'System.Boolean'],
                                  ['gdg', #{hfGudang}.getValue(), 'System.Char'],
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
                                <ext:RecordField Name="n_sisa" Type="Float" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template3" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 500px">
                        <tr>
                        <td class="body-panel">Kode</td><td class="body-panel">Nama</td><td class="body-panel">Jumlah</td>
                        <%--<td class="body-panel">SG Pending</td><td class="body-panel">Stock</td>--%>
                        </tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_iteno}</td><td>{v_itnam}</td><td>{n_sisa}</td>
                        </tr></tpl>
                        </table>
                        </Html>
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
                      PageSize="10" ListWidth="350" DisplayField="c_batch" ValueField="c_batch" AllowBlank="false">
                      <Store>
                        <ext:Store ID="StoreBatch" runat="server" AutoLoad="false">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="4303" />
                            <ext:Parameter Name="parameters" Value="[['gdg', #{hfGudang}.getValue(), 'System.Char'],
                                                          ['iteno', #{cbItemDtl}.getValue(), 'System.String'],
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
                                <ext:RecordField Name="N_BSISA" />
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
                          <td class="body-panel">Bad</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_batch}</td>
                            <td>{d_expired:this.formatDate}</td>
                            <td>{N_BSISA}</td>
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
                        <Select Handler="selectedItemBatchPemusnahanstd(this, record, #{txBQtyDtl}, #{cbItemDtl}, #{gridDetail})" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:NumberField ID="txBQtyDtl" runat="server" FieldLabel="Bad" AllowNegative="false"
                      AllowDecimals="true" DecimalPrecision="2" Width="75" AllowBlank="false" />
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGridPemusnahan(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{cbBatDtl}, #{txBQtyDtl});" />
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
                        <ext:RecordField Name="v_itnam" Type="String" />
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="n_QtyRequest" Type="Float" />
                        <ext:RecordField Name="n_gqty" Type="Float" />
                        <ext:RecordField Name="n_bqty" Type="Float" />
                        <ext:RecordField Name="n_booked" Type="Float" />
                        <ext:RecordField Name="n_booked_bad" Type="Float" />
                        <ext:RecordField Name="l_modified" Type="Boolean" />
                        <ext:RecordField Name="l_void" Type="Boolean" />
                        <ext:RecordField Name="l_new" Type="Boolean" />
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
                    <PrepareToolbar Handler="prepareCommandsPemusnahan( record, toolbar, #{hfSJNo}.getValue(), #{hfConfMode}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="250" />
                  <ext:Column DataIndex="c_batch" Header="Batch" />
                  <ext:NumberColumn DataIndex="n_booked_bad" Header="Alokasi Buruk" Format="0.000,00/i" Width="100" />
                  <ext:NumberColumn DataIndex="n_bqty" Header="Terpenuhi Buruk" Format="0.000,00/i" Width="100">
                  </ext:NumberColumn>
                  <ext:NumberColumn DataIndex="n_bqtyH" Format="0.000,00/i" Hidden="true" Width="75" />
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStorePemusnahan(record); }" />
                <%--<AfterEdit Fn="afterEdit" />--%>
                <AfterEdit Handler="afterEditDataComboGrid(e, #{cbTypeDcGrd});" />
                <BeforeEdit Handler="beforeEditDataComboGrid(e);" />
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
    <%--<ext:Button runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="btnSimpan_OnClick" Before="return verifyHeaderAndDetail(#{frmHeaders}, #{gridDetail});">
          <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfSJNo}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>--%>
    <ext:Button ID="btnSave" runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="btnSimpan_OnClick" Before="return verifyHeaderAndDetail(#{frmHeaders}, #{gridDetail});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders}, #{gridDetail});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfSJNo}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Reload" Text="Bersihkan">
    </ext:Button>
    <ext:Button runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />
