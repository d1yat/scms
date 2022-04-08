<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RNKhususCtrlUlujami.ascx.cs"
  Inherits="transaksi_penerimaan_RNKhususCtrl_Ulujami" %>

<script type="text/javascript">
  var validateRowsEditing = function(e) {
    if (Ext.isEmpty(e)) {
      return;
    }

    if (e.record.get('isProcess')) {
      e.cancel = true;
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
  var voidDataFromStore = function(rec) {
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

                  rec.reject();
                  rec.set('l_void', true);
                  rec.set('v_ket', txt);
                }
              });
          }
        });
    }
  }
  var onChangePo = function(o, valu, tQty) {
    var store = o.getStore();

    if ((!Ext.isEmpty(tQty)) && (!Ext.isEmpty(store))) {
      var idx = store.findExact('c_pono', valu);
      if (r != -1) {
        var r = store.getAt(idx);

        tQty.setRawValue(r.get('n_sisa'));
      }
      else {
        tQty.reset();
      }
    }
  }
  var storeToDetailGrid = function(frm, grid, tipe, item, nomorPo, batch, tglbatch, quantity) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }

    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(item) ||
          Ext.isEmpty(nomorPo) ||
          Ext.isEmpty(batch) ||
          Ext.isEmpty(tglbatch) ||
          Ext.isEmpty(quantity)) {
      ShowWarning("Objek tidak terdefinisi.");
      return;
    }

    var store = grid.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
    }

    var valX = [item.getValue(), tipe.getValue(), nomorPo.getValue(), batch.getValue()];
    var fieldX = ['c_iteno', 'c_type', 'c_refno', 'c_batch'];

    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

    if (!isDup) {
      var qty = quantity.getValue();
      var itemNo = item.getValue();

      store.insert(0, new Ext.data.Record({
        'c_iteno': itemNo,
        'v_itemdesc': item.getText(),
        'c_refno': nomorPo.getValue(),
        'c_batch': batch.getValue(),
        'd_batchexpired': tglbatch.getValue(),
        'c_type': tipe.getValue(),
        'v_typedesc': tipe.getText(),
        'n_gqty': qty,
        'l_new': true
      }));

      resetEntryWhenChangeInt();

      /*
      item.reset();
      quantity.reset();
      nomorPo.reset();
      batch.reset();
      tglbatch.reset();
      */
    }
    else {
      ShowError('Data telah ada.');

      return false;
    }
  }
  var resetEntryWhenChangeInt = function(g) {
    var item = Ext.getCmp('<%= cbItemDtl.ClientID %>'),
      nomorPo = Ext.getCmp('<%= cbPODtl.ClientID %>'),
      batch = Ext.getCmp('<%= txBatchDtl.ClientID %>'),
      tglbatch = Ext.getCmp('<%= txDateDtl.ClientID %>'),
      quantity = Ext.getCmp('<%= txQtyDtl.ClientID %>');

    if (!Ext.isEmpty(item)) {
      item.reset();
    }
    if (!Ext.isEmpty(nomorPo)) {
      nomorPo.reset();
    }
    if (!Ext.isEmpty(batch)) {
      batch.reset();
    }
    if (!Ext.isEmpty(tglbatch)) {
      tglbatch.reset();
    }
    if (!Ext.isEmpty(quantity)) {
      quantity.reset();
    }

    if (!Ext.isEmpty(g)) {
      g.getStore().removeAll();
    }
  }
  var autoFillTglDo = function(valu, cb, tx) {
    var store = cb.getStore();
    if (!Ext.isEmpty(store)) {
      var idx = store.findExact('c_dono', valu);
      if (idx != -1) {
        var r = store.getAt(idx);
        if (!Ext.isEmpty(r)) {
          tx.setValue(r.get('d_dodate'));
        }
      }
    }
  }
  var autoFillBatchExpr = function(valu, cb, tx, dt) {
    if (cb.selectedIndex == -1) {
      return;
    }

    var store = cb.getStore();
    if (!Ext.isEmpty(store)) {
      var recItem = store.getAt(cb.selectedIndex);
      if (!Ext.isEmpty(recItem)) {
        var batc = recItem.get('c_batch');
        var exprd = recItem.get('d_expired');

        tx.setValue(batc);
        dt.setValue(exprd);
      }
    }
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="825" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="825" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfRnNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="150" MaxHeight="150" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="150" Padding="10" Layout="Column">
          <Items>
            <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
              <Items>
                <ext:ComboBox ID="cbGudangHdr" runat="server" FieldLabel="Gudang" DisplayField="v_gdgdesc"
                  ValueField="c_gdg" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="250"
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
                        <ext:Parameter Name="model" Value="2031" />
                        <ext:Parameter Name="parameters" Value="[['c_gdg == @0', '1', 'System.Char'],
                        ['@contains.v_gdgdesc.Contains(@0) || @contains.c_gdg.Contains(@0)', paramTextGetter(#{cbGudangHdr}), '']]"
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
                    <table cellpading="0" cellspacing="1" style="width: 250px">
                    <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                    <tpl for="."><tr class="search-item">
                    <td>{c_gdg}</td><td>{v_gdgdesc}</td>
                    </tr></tpl>
                    </table>
                    </Html>
                  </Template>
                  <Listeners>
                    <Change Handler="resetEntryWhenChangeInt(#{gridDetail});clearRelatedComboRecursive(true, #{cbItemDtl});" />
                  </Listeners>
                </ext:ComboBox>
                <ext:ComboBox ID="cbSuplierHdr" runat="server" FieldLabel="Pemasok" DisplayField="v_nama"
                  ValueField="c_nosup" Width="225" ItemSelector="tr.search-item" PageSize="10" ListWidth="300"
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
                        <ext:Parameter Name="model" Value="10021" />
                        <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                          ['l_aktif = @0', true, 'System.Boolean'],
                          ['@contains.v_nama.Contains(@0) || @contains.c_nosup.Contains(@0)', paramTextGetter(#{cbSuplierHdr}), '']]"
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
                    <table cellpading="0" cellspacing="1" style="width: 300px">
                    <tr><td class="body-panel">Kode</td><td class="body-panel">Pemasok</td></tr>
                    <tpl for="."><tr class="search-item">
                        <td>{c_nosup}</td><td>{v_nama}</td>
                    </tr></tpl>
                    </table>
                    </Html>
                  </Template>
                  <Listeners>
                    <Change Handler="resetEntryWhenChangeInt(#{gridDetail});clearRelatedComboRecursive(true, #{cbDoHdr});" />
                  </Listeners>
                </ext:ComboBox>
                <ext:ComboBox ID="cbDoHdr" runat="server" FieldLabel="Nomor" DisplayField="c_dono"
                  ValueField="c_dono" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="250"
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
                        <ext:Parameter Name="model" Value="10031" />
                        <ext:Parameter Name="parameters" Value="[['nosup', paramValueGetter(#{cbSuplierHdr}), 'System.String'],
                        ['@contains.c_dono.Contains(@0)', paramTextGetter(#{cbDoHdr}), '']]" Mode="Raw" />
                        <ext:Parameter Name="sort" Value="d_dodate" />
                        <ext:Parameter Name="dir" Value="ASC" />
                      </BaseParams>
                      <Reader>
                        <ext:JsonReader IDProperty="c_dono" Root="d.records" SuccessProperty="d.success"
                          TotalProperty="d.totalRows">
                          <Fields>
                            <ext:RecordField Name="c_dono" />
                            <ext:RecordField Name="d_dodate" Type="Date" DateFormat="M$" />
                          </Fields>
                        </ext:JsonReader>
                      </Reader>
                    </ext:Store>
                  </Store>
                  <Triggers>
                    <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                  </Triggers>
                  <Template runat="server">
                    <Html>
                    <table cellpading="0" cellspacing="1" style="width: 275px">
                    <tr><td class="body-panel">Nomor</td><td class="body-panel">Tanggal</td></tr>
                    <tpl for="."><tr class="search-item">
                        <td>{c_dono}</td><td>{d_dodate:this.formatDate}</td>
                    </tr></tpl>
                    </table>
                    </Html>
                    <Functions>
                      <ext:JFunction Name="formatDate" Fn="myFormatDate" />
                    </Functions>
                  </Template>
                  <Listeners>
                    <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                    <Change Handler="resetEntryWhenChangeInt(#{gridDetail});clearRelatedComboRecursive(true, #{cbItemDtl});autoFillTglDo(newValue, this, #{txDateDoHdr});" />
                  </Listeners>
                  <DirectEvents>
                    <Change OnEvent="DoRefresh_Click">
                      <EventMask ShowMask="true" Target="CustomTarget" CustomTarget="#{pnlDown}" />
                      <ExtraParams>
                        <ext:Parameter Name="Suplier" Value="#{cbSuplierHdr}.getValue()" Mode="Raw" />
                        <ext:Parameter Name="DoNumber" Value="this.getValue()" Mode="Raw" />
                      </ExtraParams>
                    </Change>
                  </DirectEvents>
                </ext:ComboBox>
                <ext:DateField ID="txDateDoHdr" runat="server" FieldLabel="Tanggal" Width="100"
                  Format="dd-MM-yyyy" AllowBlank="false" />
              </Items>
            </ext:Panel>
            <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
              <Items>
                <ext:NumberField ID="txBeaHdr" runat="server" FieldLabel="B E A" AllowNegative="false"
                  AllowDecimals="true" DecimalPrecision="2" Width="100" AllowBlank="false" MinValue="0"
                  MaxValue="100" />
                <ext:Checkbox ID="chkFloatingHdr" runat="server" FieldLabel="Floating" />
                <ext:TextField ID="txKeteranganHdr" runat="server" FieldLabel="Keterangan" MaxLength="100"
                  Width="235" />
                <ext:Checkbox ID="chkOrderKhusus" runat="server" FieldLabel="Khusus">
                  <ToolTips>
                    <ext:ToolTip runat="server" Title="Informasi" Html="Cek ini, jika ingin tipe RN ini akan dibuat PL Auto" />
                  </ToolTips>
                </ext:Checkbox>
              </Items>
            </ext:Panel>
          </Items>
        </ext:FormPanel>
      </North>
      <Center MinHeight="150">
        <ext:Panel ID="pnlDown" runat="server" Layout="Fit" Title="Daftar Items">
          <TopBar>
            <ext:Toolbar runat="server">
              <Items>
                <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                  LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                  <Items>
                    <ext:ComboBox ID="cbTipeDtl" runat="server" FieldLabel="Tipe" DisplayField="v_ket"
                      ValueField="c_type" SelectedIndex="0" Width="70" AllowBlank="false" ForceSelection="false">
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
                            <ext:Parameter Name="start" Value="0" />
                            <ext:Parameter Name="limit" Value="-1" />
                            <ext:Parameter Name="model" Value="2001" />
                            <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                                                    ['c_notrans = @0', '06', 'System.String'],
                              ['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbTipeDtl}), '']]" Mode="Raw" />
                            <ext:Parameter Name="sort" Value="v_ket" />
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
                        <Change Handler="clearRelatedComboRecursive(true, #{cbItemDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" DisplayField="v_itnam"
                      ValueField="c_iteno" Width="250" ItemSelector="tr.search-item" PageSize="10" ListWidth="300"
                      MinChars="3" llowBlank="false" ForceSelection="false">
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
                            <ext:Parameter Name="model" Value="10141" />
                            <ext:Parameter Name="parameters" Value="[['tipe', paramValueGetter(#{cbTipeDtl}), 'System.String'],
                                                                    ['nosup', paramValueGetter(#{cbSuplierHdr}), 'System.String'],
                                                                    ['doKhusus', paramValueGetter(#{cbDoHdr}), 'System.String'],
                                                       ['@contains.v_itnam.Contains(@0) || @contains.c_iteno.Contains(@0)', paramTextGetter(#{cbItemDtl}), '']]"
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
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 300px">
                        <tr>
                        <td class="body-panel">Kode</td><td class="body-panel">Nama</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_iteno}</td><td>{v_itnam}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                        <Functions>
                          <ext:JFunction Name="formatNumber" Fn="myFormatNumber" />
                        </Functions>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <Change Handler="clearRelatedComboRecursive(true, #{cbPODtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbPODtl" runat="server" FieldLabel="Nomor PO" DisplayField="c_pono"
                      ValueField="c_pono" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="450"
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
                            <ext:Parameter Name="model" Value="10241" />
                            <ext:Parameter Name="parameters" Value="[['nosup', paramValueGetter(#{cbSuplierHdr}), 'System.String'],
                                                                    ['doKhusus', paramValueGetter(#{cbDoHdr}), 'System.String'],
                                                                    ['item', paramValueGetter(#{cbItemDtl}), 'System.String'],
                                                                    ['@contains.c_pono.Contains(@0)', paramTextGetter(#{cbPODtl}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="d_podate" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_pono" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_pono" />
                                <ext:RecordField Name="d_podate" Type="Date" DateFormat="M$" />
                                <ext:RecordField Name="c_batch" />
                                <ext:RecordField Name="d_expired" Type="Date" DateFormat="M$" />
                                <ext:RecordField Name="n_qty" Type="Float" />
                                <ext:RecordField Name="n_sisa" Type="Float" />
                                <ext:RecordField Name="n_qty_po" Type="Float" />
                                <ext:RecordField Name="n_sisa_po" Type="Float" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 450px">
                        <tr><td class="body-panel">Nomor</td><td class="body-panel">Tanggal</td>
                        <td class="body-panel">Jumlah PO</td><td class="body-panel">Sisa PO</td>
                        <td class="body-panel">Batch</td><td class="body-panel">Expired</td>
                        <td class="body-panel">Jumlah</td><td class="body-panel">Sisa</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_pono}</td><td>{d_podate:this.formatDate}</td>
                        <td>{n_qty_po:this.formatNumber}</td><td>{n_sisa_po:this.formatNumber}</td>
                        <td>{c_batch}</td><td>{d_expired:this.formatDate}</td>
                        <td>{n_qty:this.formatNumber}</td><td>{n_sisa:this.formatNumber}</td>
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
                        <Change Handler="onChangePo(this, newValue, #{txQtyDtl});autoFillBatchExpr(newValue, this, #{txBatchDtl}, #{txDateDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:TextField ID="txBatchDtl" runat="server" FieldLabel="Batch" AllowBlank="false"
                      MaxLength="15" Width="100" />
                    <ext:DateField ID="txDateDtl" runat="server" FieldLabel="Expired" Width="100" Format="dd-MM-yyyy"
                      AllowBlank="false" />
                    <ext:NumberField ID="txQtyDtl" runat="server" FieldLabel="Jumlah" AllowBlank="false"
                      AllowDecimals="true" AllowNegative="false" Width="75" MinValue="0" />
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbTipeDtl}, #{cbItemDtl}, #{cbPODtl}, #{txBatchDtl}, #{txDateDtl}, #{txQtyDtl});" />
                      </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnClear" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Clear"
                      Icon="Cancel">
                      <Listeners>
                        <Click Handler="resetEntryWhenChangeInt()" />
                      </Listeners>
                    </ext:Button>
                  </Items>
                </ext:FormPanel>
              </Items>
            </ext:Toolbar>
          </TopBar>
          <Items>
            <ext:GridPanel ID="gridDetail" runat="server" ClicksToEdit="1">
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
                    <ext:Parameter Name="model" Value="0027-b" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['gudang', '1', 'System.Char'],
                      ['c_rnno = @0', #{hfRnNo}.getValue(), 'System.String'],
                      ['c_typern = @0', '05', 'System.String'],
                      ['l_rnkhusus', 'true', 'System.Boolean']
                      ]"
                       Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records"
                      SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itemdesc" />
                        <ext:RecordField Name="c_refno" />
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="d_batchexpired" />
                        <ext:RecordField Name="c_type" />
                        <ext:RecordField Name="v_typedesc" />
                        <ext:RecordField Name="n_gqty" Type="Float" />
                        <ext:RecordField Name="l_new" Type="Boolean" />
                        <ext:RecordField Name="l_void" Type="Boolean" />
                        <ext:RecordField Name="v_ket" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                  <SortInfo Field="v_itemdesc" Direction="ASC" />
                </ext:Store>
              </Store>
              <ColumnModel>
                <Columns>
                  <ext:CommandColumn Width="25">
                    <Commands>
                      <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                      <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                    </Commands>
                    <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfRnNo}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itemdesc" Header="Nama Barang" Width="200" />
                  <ext:Column DataIndex="c_refno" Header="Nomor PO" />
                  <ext:Column DataIndex="c_batch" Header="Batch" Width="150" />
                  <ext:Column DataIndex="d_batchexpired" Header="Expired" />
                  <ext:Column DataIndex="v_typedesc" Header="Tipe" Width="75" />
                  <ext:NumberColumn DataIndex="n_gqty" Header="Jumlah" Format="0.000,00/i" Width="55" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidDataFromStore(record); }" />
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
        <Click OnEvent="Report_OnGenerate">
          <Confirmation ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfRnNo}.getValue()" Mode="Raw" />
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
            <ext:Parameter Name="NumberID" Value="#{hfRnNo}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GudangID" Value="#{cbGudangHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GudangDesc" Value="#{cbGudangHdr}.getText()" Mode="Raw" />
            <ext:Parameter Name="SuplierID" Value="#{cbSuplierHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="SuplierDesc" Value="#{cbSuplierHdr}.getText()" Mode="Raw" />
            <ext:Parameter Name="NumberDO" Value="#{cbDoHdr}.getValue()" Mode="Raw" />
            <%--<ext:Parameter Name="DateDO" Value="#{txDateDoHdr}.getRawValue()" Mode="Raw" />--%>
            <ext:Parameter Name="DateDO" Value="#{txDateDoHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Bea" Value="#{txBeaHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Floating" Value="#{chkFloatingHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Khusus" Value="#{chkOrderKhusus}.getValue()" Mode="Raw" />            
            <ext:Parameter Name="Keterangan" Value="#{txKeteranganHdr}.getValue()" Mode="Raw" />
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
