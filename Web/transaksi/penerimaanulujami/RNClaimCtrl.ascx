<%@ Control Language="C#" AutoEventWireup="true" 
CodeFile="RNClaimCtrl.ascx.cs" Inherits="transaksi_penerimaan_RNClaimCtrl" %>

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
  var onChangeClaimAcc = function(o, valu, tQty, grid, itm, qtyTemp) {
    var store = o.getStore();
    var store2 = grid.getStore();

    if ((!Ext.isEmpty(tQty)) && (!Ext.isEmpty(store))) {
        var idx = store.findExact('c_claimaccno', valu);
        var pono = "";
        var ponoGrid = "";
        var itmGrid = "";
        var qty = 0;
        
      if (r != -1) {
        var r = store.getAt(idx);
        var qtySisa = r.get('n_sisa');
        var pono = r.get('c_claimaccno');
        
        for (i = 0; i < store2.data.items.length; i++) {
            ponoGrid = store2.data.items[i].data.c_refno;
            itmGrid = store2.data.items[i].data.c_iteno;

            if (pono == ponoGrid && itm.getValue() == itmGrid) 
            {
                qty += store2.data.items[i].data.n_gqty
            }
        }

        qtySisa -= qty
        tQty.setRawValue(qtySisa);
        qtyTemp.setValue(qtySisa);
//        tQty.setRawValue(r.get('n_sisa'));
      }
      else {
        tQty.reset();
      }
    }
  }
  var storeToDetailGrid = function(frm, grid, item, nomorClaim, batch, tglbatch, quantity, qtyTemp) {
      if (!frm.getForm().isValid()) {
          ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
          return;
      }

      if (Ext.isEmpty(grid) ||
          Ext.isEmpty(item) ||
          Ext.isEmpty(nomorClaim) ||
          Ext.isEmpty(batch) ||
          Ext.isEmpty(tglbatch) ||
          Ext.isEmpty(quantity)) {
          ShowWarning("Objek tidak terdefinisi.");
          return;
      }

      if (quantity.getValue() > qtyTemp.getValue()) {
          ShowWarning("Inputan jumlah sudah melebihi sisa claim");
          return;
      }

      var store = grid.getStore();
      if (Ext.isEmpty(store)) {
          ShowWarning("Objek store tidak terdefinisi.");
          return;
      }

      var valX = [item.getValue(), nomorClaim.getValue(), batch.getValue()];
      var fieldX = ['c_iteno', 'c_refno', 'c_batch'];

      var isDup = findDuplicateEntryGrid(store, fieldX, valX);

      if (!isDup) {
          var qty = quantity.getValue();
          var itemNo = item.getValue();

          store.insert(0, new Ext.data.Record({
              'c_iteno': itemNo,
              'v_itemdesc': item.getText(),
              'c_refno': nomorClaim.getValue(),
              'c_batch': batch.getValue(),
              'd_batchexpired': tglbatch.getValue(),
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
      nomorClaim = Ext.getCmp('<%= cbClaimDtl.ClientID %>'),
      batch = Ext.getCmp('<%= txBatchDtl.ClientID %>'),
      tglbatch = Ext.getCmp('<%= txDateDtl.ClientID %>'),
      quantity = Ext.getCmp('<%= txQtyDtl.ClientID %>');

    if (!Ext.isEmpty(item)) {
      item.reset();
    }
    if (!Ext.isEmpty(nomorClaim)) {
      nomorClaim.reset();
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
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="825" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="825" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfRnNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfQty" runat="server" />
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
                        <ext:Parameter Name="parameters" Value="[['c_gdg != @0', '2', 'System.Char'],
                        ['@contains.v_gdgdesc.Contains(@0) || @contains.c_gdg.Contains(@0)', paramTextGetter(#{cbGudangHdr}), '']]" Mode="Raw" />
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
                  ValueField="c_nosup" Width="225" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
                  MinChars="3" AllowBlank="false" ForceSelection="false">
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
                        <ext:Parameter Name="model" Value="2021" />
                        <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                          ['l_aktif = @0', true, 'System.Boolean'],
                          ['@contains.v_nama.Contains(@0) || @contains.c_nosup.Contains(@0)', paramTextGetter(#{cbSuplierHdr}), '']]" Mode="Raw" />
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
                    <table cellpading="0" cellspacing="1" style="width: 400px">
                    <tr><td class="body-panel">Kode</td><td class="body-panel">Pemasok</td></tr>
                    <tpl for="."><tr class="search-item">
                        <td>{c_nosup}</td><td>{v_nama}</td>
                    </tr></tpl>
                    </table>
                    </Html>
                  </Template>
                  <Listeners>
                    <Change Handler="resetEntryWhenChangeInt(#{gridDetail});clearRelatedComboRecursive(true, #{cbItemDtl});" />
                  </Listeners>
                </ext:ComboBox>
                <ext:TextField ID="txDoHdr" runat="server" FieldLabel="Nomor Delivery" MaxLength="20"
                  Width="175" AllowBlank="false" />
                <ext:DateField ID="txDateDoHdr" runat="server" FieldLabel="Tanggal DO" Width="100"
                  Format="dd-MM-yyyy" AllowBlank="false" />
              </Items>
            </ext:Panel>
            <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
              <Items>
                <ext:NumberField ID="txBeaHdr" runat="server" FieldLabel="B E A" AllowNegative="false"
                  AllowDecimals="true" DecimalPrecision="2" Width="100" AllowBlank="false" MinValue="0"
                  MaxValue="100" />
                <ext:Checkbox ID="chkFloatingHdr" runat="server" FieldLabel="Floating" />
                <ext:TextField ID="txKeteranganHdr" runat="server" FieldLabel="Keterangan" MaxLength="100"
                  Width="235" />
              </Items>
            </ext:Panel>
          </Items>
        </ext:FormPanel>
      </North>
      <Center MinHeight="150">
        <ext:Panel ID="Panel4" runat="server" Layout="Fit" Title="Daftar Items">
          <TopBar>
            <ext:Toolbar ID="Toolbar1" runat="server">
              <Items>
                <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                  LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                  <Items>
                    <%--<ext:ComboBox ID="cbTipeDtl" runat="server" FieldLabel="Tipe" DisplayField="v_ket"
                      ValueField="c_type" SelectedIndex="0" Width="70" AllowBlank="false">
                      <Store>
                        <ext:Store ID="Store3" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="0" />
                            <ext:Parameter Name="limit" Value="-1" />
                            <ext:Parameter Name="model" Value="2001" />
                            <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                                                    ['c_notrans = @0', '06', 'System.String']]"
                              Mode="Raw" />
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
                    </ext:ComboBox>--%>
                    <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" DisplayField="v_itnam"
                      ValueField="c_iteno" Width="250" ItemSelector="tr.search-item" PageSize="10" ListWidth="350"
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
                            <ext:Parameter Name="model" Value="10071" />
                            <ext:Parameter Name="parameters" Value="[['tipe', paramValueGetter(#{cbTipeDtl}), 'System.String'],
                                                                    ['nosup', paramValueGetter(#{cbSuplierHdr}), 'System.String'],
                                                                    ['gudang', '1', 'System.Char'],
                                                                    ['@contains.v_itnam.Contains(@0) || @contains.c_iteno.Contains(@0)', paramTextGetter(#{cbItemDtl}), '']]" Mode="Raw" />
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
                      <Template ID="Template3" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 400px">
                        <tr>
                        <td class="body-panel">Kode</td><td class="body-panel">Nama</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_iteno}</td><td>{v_itnam}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <Change Handler="clearRelatedComboRecursive(true, #{cbClaimDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbClaimDtl" runat="server" FieldLabel="Nomor Claim" DisplayField="c_claimaccno"
                      ValueField="c_claimaccno" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="300"
                      MinChars="3" EmptyText="Pilihan..." Mode="Local" AllowBlank="false" ForceSelection="false">
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
                            <ext:Parameter Name="model" Value="10081" />
                            <ext:Parameter Name="parameters" Value="[['tipe', paramValueGetter(#{cbTipeDtl}), 'System.String'],
                                                                    ['nosup', paramValueGetter(#{cbSuplierHdr}), 'System.String'],
                                                                    ['item', paramValueGetter(#{cbItemDtl}), 'System.String'],
                             ['@contains.c_claimaccno.Contains(@0)', paramTextGetter(#{cbClaimDtl}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="d_claimaccdate" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_claimaccno" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_claimaccno" />
                                <ext:RecordField Name="c_claimnoprinc" />
                                <ext:RecordField Name="d_claimaccdate" Type="Date" DateFormat="M$" />
                                <ext:RecordField Name="n_sisa" Type="Float" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template4" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 300px">
                        <tr><td class="body-panel">Nomor</td><td class="body-panel">No.Pri</td><td class="body-panel">Tanggal</td><td class="body-panel">Sisa</td></tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_claimaccno}</td><td>{c_claimnoprinc}</td><td>{d_claimaccdate:this.formatDate}</td><td>{n_sisa}</td>
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
                        <Change Handler="onChangeClaimAcc(this, newValue, #{txQtyDtl}, #{gridDetail}, #{cbItemDtl},#{hfQty});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:TextField ID="txBatchDtl" runat="server" FieldLabel="Batch" AllowBlank="false" MaxLength="15"
                      Width="100" />
                    <ext:DateField ID="txDateDtl" runat="server" FieldLabel="Expired" Width="100"
                      Format="dd-MM-yyyy" AllowBlank="false" />
                    <ext:NumberField ID="txQtyDtl" runat="server" FieldLabel="Jumlah" AllowBlank="false"
                      AllowDecimals="true" AllowNegative="false" Width="75" MinValue="0" />
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{cbClaimDtl}, #{txBatchDtl}, #{txDateDtl}, #{txQtyDtl}, #{hfQty});" />
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
                <ext:Store ID="Store6" runat="server" RemotePaging="false" RemoteSort="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0027" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['gudang', '1', 'System.Char'],
                      ['c_rnno = @0', #{hfRnNo}.getValue(), 'System.String'],
                      ['c_typern = @0', '03', 'System.String']]" Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records"
                      SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itemdesc" />
                        <ext:RecordField Name="c_refno" />
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="d_batchexpired" Type="Date" DateFormat="M$" />
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
                  <ext:Column DataIndex="c_refno" Header="Nomor Claim" />
                  <ext:Column DataIndex="c_batch" Header="Batch" Width="150" />
                  <ext:DateColumn ColumnID="d_batchexpired" DataIndex="d_batchexpired" Header="Expired" Format="dd-MM-yyyy" />
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
            <ext:Parameter Name="NumberDO" Value="#{txDoHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="DateDO" Value="#{txDateDoHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Bea" Value="#{txBeaHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Floating" Value="#{chkFloatingHdr}.getValue()" Mode="Raw" />
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
    <ext:Button ID="Button2" runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
