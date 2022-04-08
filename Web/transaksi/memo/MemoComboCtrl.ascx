<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MemoComboCtrl.ascx.cs"
  Inherits="transaksi_memo_MemoComboCtrl" %>

<script type="text/javascript">
  var calcTotalValueItem = function(storeGrid, storeItem, item, qtyItemCombo) {
    // Find total qty in grid
    var totalQtyInGrid = 0;
    var qtyKomposisi = 0;
    var qtyRequested = 0;
    var isAccept = false;

    var idx = storeItem.findExact('c_iteno', item);
    if (idx != -1) {
      qtyKomposisi = storeItem.getAt(idx).get('n_qty_komposisi');

      qtyRequested = (qtyKomposisi * qtyItemCombo);
    }

    storeGrid.each(function(r) {
      if ((r.get('c_iteno').trim() == item) && (!r.get('l_void'))) {
        totalQtyInGrid += Ext.num(r.get('n_qty'), 0);
      }
    });

    if ((qtyRequested > 0) && (totalQtyInGrid < qtyRequested)) {
      isAccept = true;
    }

    var returnResult =
      {
        'result': isAccept,
        'request': qtyRequested,
        'totalInGrid': totalQtyInGrid
      };

    return returnResult;
  }
  var storeToDetailGrid = function(frm, grid, item, batch, quantity) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }

    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(item) ||
          Ext.isEmpty(batch)) {
      ShowWarning("Objek tidak terdefinisi.");
      return;
    }

    var store = grid.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
    }

    if (item.selectedIndex == -1) {
      ShowWarning("Pilihan item tidak terbaca atau belum terpilih.");
      return;
    }
    
    // Item
    var storeX = item.getStore();
    var recItem = storeX.getAt(item.selectedIndex);

    //var valX = [item.getValue(), batch.getValue()];
    var valX = [recItem.get('c_iteno'), batch.getValue()];
    var fieldX = ['c_iteno', 'c_batch'];

    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

    if (!isDup) {
      var itemNo = recItem.get('c_iteno');
      var batchId = batch.getText();
      var qtyCombo = quantity.getValue();

      var resCalcVI_JS = calcTotalValueItem(store, storeX, itemNo, qtyCombo);

      if (resCalcVI_JS.result) {

        //// Item
        //var recItem = storeX.getAt(item.getSelectedIndex());

        var rcvNote = recItem.get('receiveNote');
        var qtyKompos = Ext.num(recItem.get('n_qty_komposisi'), 0);
        var qtySisa = (resCalcVI_JS.request - resCalcVI_JS.totalInGrid);
        var itmQty = Ext.num(recItem.get('n_qty'), 0);

        //Batch
        storeX = batch.getStore();

        var recBatch = storeX.getAt(batch.getSelectedIndex());

        var batchExpired = batch.getValue();
        var qtyBatch = Ext.num(recBatch.get('n_qty'), 0);
        qtySisa = (qtyBatch <= qtySisa ? qtyBatch : qtySisa);

        store.insert(0, new Ext.data.Record({
          'c_iteno': itemNo,
          'v_itemdesc': recItem.get('v_itnam'),
          'n_qty': qtySisa,
          'c_batch': batchId,
          'd_expired': batchExpired,
          'receiveNote': rcvNote,
          'n_pack': qtyKompos,
          'n_qty_expected': resCalcVI_JS.request,
          'l_new': true
        }));

      }
      else {
        ShowWarning('Data tidak cocok atau total jumlah item ini telah mencukupi.');
      }

      item.reset();
      batch.reset();
    }
    else {
      ShowError('Data telah ada.');

      return false;
    }
  }
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
  var changeItemSelect = function(c, val, t) {
    var store = c.getStore();
    if (!Ext.isEmpty(store)) {
      var idx = store.findExact('c_iteno', val);
      if (idx != -1) {
        var r = store.getAt(idx);
        t.setValue(r.get('n_sisa'));
      }
    }
  }
  /*
  var comboItmDtlChanged = function(c, v, t) {
    var store = c.getStore();

    var idx = store.findExact('c_iteno', v);
    if (idx != -1) {
      var r = store.getAt(idx);

      if (!Ext.isEmpty(r)) {
        t.setValue(r.get('n_box'));
      }
    }
  }
  var qryBatchDtlBQ = function(qe, g, h) {
    var curItm = h.getValue();

    if (Ext.isEmpty(g)) {
      qe.cancel = true;

      h.setValue('');
    }
    else {
      if (g.getSelectionModel().getSelections().length > 0) {
        var rec = g.getSelectionModel().getSelections()[0];

        var item = rec.get('c_iteno');

        if (curItm.trim() != item.trim()) {
          h.setValue(item);

          var storCB = qe.combo.getStore();

          storCB.removeAll();
          storCB.reload();
        }
      }
    }
  }
  var cbBatchSelected = function(recCB, g) {
    if (g.getSelectionModel().getSelections().length > 0) {
      var rec = g.getSelectionModel().getSelections()[0];

      if (!Ext.isEmpty(rec)) {
        var qtyExpct = rec.get('n_qty_expected');
        var availQty = recCB.get('n_qty');
        var batchActv = recCB.get('c_batch');

        if (availQty > qtyExpct) {
          var dateFrm = new Date(recCB.get('d_expired'));

          rec.set('c_batch') = batchActv;
          rec.set('n_qty') = qtyExpct;
          rec.set('d_expired') = dateFrm.format('d-m-Y');
        }
        else {
          var msg = String.format("Jumlah barang di batch '{0}' lebih kecil dari yang di harapkan '{1}'.",
          batchActv, qtyExpct);
          ShowWarning(msg);

          return false;
        }
      }
    }
  }
  var renderingNewBatch = function(valu, reco, g) {
    var batchID = valu;

    if (g.getSelectionModel().getSelections().length > 0) {
      var rec = g.getSelectionModel().getSelections()[0];

      var prevBatch = ((rec.modified != null) ? rec.modified.get('modified') : rec.get('c_batch'));

      var qtyExpct = rec.get('n_qty_expected');
      var availQty = reco.get('n_qty');
      var batchActv = reco.get('c_batch');
      var dateFrm = new Date(reco.get('d_expired'));

      if (availQty > qtyExpct) {
        rec.set('n_qty') = qtyExpct;
        rec.set('d_expired') = dateFrm.format('d-m-Y');

        batchID = batchActv;
      }
      else {
        var msg = String.format("Jumlah barang di batch '{0}' lebih kecil dari yang di harapkan '{1}'.",
          batchActv, qtyExpct);

        batchID = prevBatch;

        ShowWarning(msg);
      }
    }

    return batchID;
  }
  */
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfComboNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfItemBatch" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="250" MaxHeight="250" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="250" Padding="10"
          AutoScroll="true" ButtonAlign="Center">
          <Items>
            <ext:ComboBox ID="cbGudangHdr" runat="server" FieldLabel="Gudang" DisplayField="v_gdgdesc"
              ValueField="c_gdg" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="250"
              MinChars="3" AllowBlank="false">
              <Store>
                <ext:Store runat="server">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="false" />
                  </CustomConfig>
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
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
                <table cellpading="0" cellspacing="1" style="width: 250px">
                <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                <tpl for="."><tr class="search-item">
                <td>{c_gdg}</td><td>{v_gdgdesc}</td>
                </tr></tpl>
                </table>
                </Html>
              </Template>
              <Listeners>
                <Change Handler="clearRelatedComboRecursive(true, #{cbMemoHdr});resetEntryWhenChange(#{gridDetail});" />
              </Listeners>
            </ext:ComboBox>
            <ext:ComboBox ID="cbMemoHdr" runat="server" FieldLabel="Memo" DisplayField="c_memo"
              ValueField="c_memono" Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="350"
              MinChars="3" AllowBlank="false" >
              <Store>
                <ext:Store runat="server">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="false" />
                  </CustomConfig>
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                    <ext:Parameter Name="model" Value="12001" />
                    <ext:Parameter Name="parameters" Value="[['c_gdg = @0', #{cbGudangHdr}.getValue(), 'System.Char'],
                    ['@contains.c_memono.Contains(@0) || @contains.c_memo.Contains(@0)', paramTextGetter(#{cbMemoHdr}), '']]"
                      Mode="Raw" />
                    <ext:Parameter Name="sort" Value="c_memono" />
                    <ext:Parameter Name="dir" Value="DESC" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_memono" Root="d.records" SuccessProperty="d.success"
                      TotalProperty="d.totalRows">
                      <Fields>
                        <ext:RecordField Name="c_memono" />
                        <ext:RecordField Name="c_memo" />
                        <ext:RecordField Name="d_memodate" Type="Date" DateFormat="M$" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <Template runat="server">
                <Html>
                <table cellpading="0" cellspacing="1" style="width: 350px">
                <tr><td class="body-panel">Nomor</td><td class="body-panel">Tanggal</td><td class="body-panel">Memo</td></tr>
                <tpl for="."><tr class="search-item">
                <td>{c_memono}</td><td>{d_memodate:this.formatDate}</td><td>{c_memo}</td>
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
                <Change Handler="clearRelatedComboRecursive(true, #{cbItemHdr});resetEntryWhenChange(#{gridDetail});" />
              </Listeners>
            </ext:ComboBox>
            <ext:ComboBox ID="cbItemHdr" runat="server" FieldLabel="Item" DisplayField="v_itnam"
              ValueField="c_iteno" Width="250" ItemSelector="tr.search-item" PageSize="10" ListWidth="300"
              MinChars="3" AllowBlank="false">
              <Store>
                <ext:Store runat="server">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="false" />
                  </CustomConfig>
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                    <ext:Parameter Name="model" Value="12101" />
                    <ext:Parameter Name="parameters" Value="[['c_gdg = @0', #{cbGudangHdr}.getValue(), 'System.Char'],
                      ['c_memono = @0', #{cbMemoHdr}.getValue(), 'System.String'],   
                      ['@contains.c_iteno.Contains(@0) || @contains.v_itnam.Contains(@0)', paramTextGetter(#{cbItemHdr}), '']]" Mode="Raw" />
                    <ext:Parameter Name="sort" Value="v_itnam" />
                    <ext:Parameter Name="dir" Value="ASC" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_iteno" Root="d.records" SuccessProperty="d.success"
                      TotalProperty="d.totalRows">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="n_qty" Type="Float" />
                        <ext:RecordField Name="n_sisa" Type="Float" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <Template runat="server">
                <Html>
                <table cellpading="0" cellspacing="1" style="width: 300px">
                <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td>
                <td class="body-panel">Jumlah</td><td class="body-panel">Sisa</td></tr>
                <tpl for="."><tr class="search-item">
                <td>{c_iteno}</td><td>{v_itnam}</td>
                <td>{n_qty}</td><td>{n_sisa}</td>
                </tr></tpl>
                </table>
                </Html>
              </Template>
              <Triggers>
                <ext:FieldTrigger Icon="Search" Qtip="Reload" />
              </Triggers>
              <Listeners>
                <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                <Change Handler="changeItemSelect(this, newValue, #{txQtyHdr});resetEntryWhenChange(#{gridDetail});clearRelatedComboRecursive(true, #{cbItemDtl}, #{cbBatchHdr});" />
              </Listeners>
            </ext:ComboBox>
            <ext:ComboBox ID="cbBatchHdr" runat="server" FieldLabel="Batch" DisplayField="c_batch"
              ValueField="c_batch" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="300"
              MinChars="3" AllowBlank="false" ForceSelection="false">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
              </CustomConfig>
              <Store>
                <ext:Store ID="Store1" runat="server">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="false" />
                  </CustomConfig>
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                    <ext:Parameter Name="model" Value="2111" />
                    <ext:Parameter Name="parameters" Value="[['c_iteno = @0', #{cbItemHdr}.getValue(), 'System.String'],
                      ['@contains.c_batch.Contains(@0)', paramTextGetter(#{cbBatchHdr}), '']]" Mode="Raw" />
                    <ext:Parameter Name="sort" Value="d_expired" />
                    <ext:Parameter Name="dir" Value="ASC" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_batch" Root="d.records" SuccessProperty="d.success"
                      TotalProperty="d.totalRows">
                      <Fields>
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="d_expired" Type="Date" DateFormat="M$" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <Template ID="Template1" runat="server">
                <Html>
                <table cellpading="0" cellspacing="1" style="width: 300px">
                <tr><td class="body-panel">Batch</td><td class="body-panel">Kadaluarsa</td>
                <tpl for="."><tr class="search-item">
                <td>{c_batch}</td><td>{d_expired:this.formatDate}</td>
                </tr></tpl>
                </table>
                </Html>
                <Functions>
                  <ext:JFunction Fn="myFormatDate" Name="formatDate" />
                </Functions>
              </Template>
              <Triggers>
                <ext:FieldTrigger Icon="Search" Qtip="Reload" />
              </Triggers>
              <Listeners>
                <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                <Change Handler="changeItemSelect(this, newValue, #{txQtyHdr});resetEntryWhenChange(#{gridDetail});clearRelatedComboRecursive(true, #{cbItemDtl});" />
              </Listeners>
            </ext:ComboBox>
            <%--<ext:TextField ID="txBatchHdr" runat="server" AllowBlank="false" FieldLabel="Batch"
              MaxLength="15" Width="150" />--%>
            <ext:NumberField ID="txQtyHdr" runat="server" AllowBlank="false" FieldLabel="Jumlah"
              Width="100" AllowDecimals="true" DecimalPrecision="2" />
            <ext:TextField ID="txKeteranganHdr" runat="server" FieldLabel="Keterangan" MaxLength="100"
              Width="400" />
          </Items>
          <Buttons>
            <ext:Button ID="btnProses" runat="server" Text="Proses" Icon="CogStart">
              <DirectEvents>
                <Click Before="return validasiProses(#{frmHeaders});" OnEvent="ProcessCombo_Click">
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="gudangId" Value="#{cbGudangHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="comboItem" Value="#{cbItemHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="qty" Value="#{txQtyHdr}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>
          </Buttons>
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
                    <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" DisplayField="v_itnam"
                      ValueField="c_iteno" Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
                      MinChars="3" AllowBlank="false">
                      <Store>
                        <ext:Store runat="server">
                          <CustomConfig>
                            <ext:ConfigItem Name="allowBlank" Value="false" />
                          </CustomConfig>
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="12301" />
                            <ext:Parameter Name="parameters" Value="[['gudang', #{cbGudangHdr}.getValue(), 'System.Char'],
                                ['itemCombo', #{cbItemHdr}.getValue(), 'System.String'],
                                ['comboQty', #{txQtyHdr}.getValue(), 'System.Decimal'],
                                ['@contains.c_iteno.Contains(@0) || @contains.v_itnam.Contains(@0)', paramTextGetter(#{cbItemDtl}), '']]" Mode="Raw" />
                            <ext:Parameter Name="sort" Value="v_itnam" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows" IDProperty="itrn">
                              <Fields>
                                <ext:RecordField Name="itrn" />
                                <ext:RecordField Name="c_iteno" />
                                <ext:RecordField Name="v_itnam" />
                                <ext:RecordField Name="n_qty" Type="Float" />
                                <ext:RecordField Name="n_qty_komposisi" Type="Float" />
                                <ext:RecordField Name="n_qty_request" Type="Float" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 400px">
                        <tr>
                        <td class="body-panel">Kode</td><td class="body-panel">Nama</td>
                        <td class="body-panel">Jumlah</td><td class="body-panel">Permintaan</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_iteno}</td><td>{v_itnam}</td>
                        <td>{n_qty}</td><td>{n_qty_request}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <Change Handler="clearRelatedComboRecursive(true, #{cbBatchDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbBatchDtl" runat="server" FieldLabel="Batch" DisplayField="c_batch"
                      ValueField="d_expired" Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="250"
                      MinChars="3" AllowBlank="false">
                      <Store>
                        <ext:Store runat="server">
                          <CustomConfig>
                            <ext:ConfigItem Name="allowBlank" Value="false" />
                          </CustomConfig>
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="12401" />
                            <ext:Parameter Name="parameters" Value="[['gudang', #{cbGudangHdr}.getValue(), 'System.Char'],
                                ['item', #{cbItemDtl}.getValue(), 'System.String'],
                                ['@contains.c_batch.Contains(@0)', paramTextGetter(#{cbBatchDtl}), '']]" Mode="Raw" />
                            <ext:Parameter Name="sort" Value="d_expired" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_batch" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_batch" />
                                <ext:RecordField Name="d_expired" Type="Date" DateFormat="M$" />
                                <ext:RecordField Name="n_qty" Type="Float" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 250px">
                        <tr>
                        <td class="body-panel">Batch</td><td class="body-panel">Expired</td>
                        <td class="body-panel">Jumlah</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_batch}</td><td>{d_expired:this.formatDate}</td>
                        <td>{n_qty}</td>
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
                      </Listeners>
                    </ext:ComboBox>
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{cbBatchDtl}, #{txQtyHdr});" />
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
                    <ext:Parameter Name="model" Value="0047" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['comboID', #{hfComboNo}.getValue(), 'System.String'],
                      ['gudang', #{cbGudangHdr}.getValue(), 'System.Char']]" Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader  TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itemdesc" />
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="n_qty" Type="Float" />
                        <ext:RecordField Name="d_expired" Type="Date" DateFormat="M$" />
                        <ext:RecordField Name="n_pack" Type="Float" />
                        <ext:RecordField Name="n_qty_expected" Type="Float" />
                        <ext:RecordField Name="l_new" Type="Boolean" />
                        <ext:RecordField Name="l_void" Type="Boolean" />
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
                    <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfComboNo}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itemdesc" Header="Nama Barang" Width="250" />
                  <ext:Column DataIndex="c_batch" Header="Batch" />
                  <ext:DateColumn DataIndex="d_expired" Header="Expired" Width="75" Format="dd-MM-yyyy" />
                  <ext:NumberColumn DataIndex="n_qty" Header="Jumlah" Format="0.000,00/i" Width="75" />
                  <ext:NumberColumn DataIndex="n_pack" Header="Kompos." Format="0.000,00/i" Width="50"
                    Hidden="true" />
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="45" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
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
        <Click OnEvent="Report_OnGenerate">
          <Confirmation ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="GudangID" Value="#{cbGudangHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfComboNo}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfComboNo}.getValue()" Mode="Raw" />
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="GudangID" Value="#{cbGudangHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GudangDesc" Value="#{cbGudangHdr}.getText()" Mode="Raw" />
            <ext:Parameter Name="MemoID" Value="#{cbMemoHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="ComboItem" Value="#{cbItemHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="ComboItemDesc" Value="#{cbItemHdr}.getText()" Mode="Raw" />
            <ext:Parameter Name="Batch" Value="#{cbBatchHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Qty" Value="#{txQtyHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Keterangan" Value="#{txKeteranganHdr}.getValue()" Mode="Raw" />
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
