<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AdjustStockStock.ascx.cs" 
Inherits="transaksi_penyesuaian_AdjustStockStock" %>

<script type="text/javascript">
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

  
  var onChangeAdjBat = function(o, valu, tGQty, tBQty) {
    var store = o.getStore();

    if ((!Ext.isEmpty(tGQty)) && (!Ext.isEmpty(tBQty)) && (!Ext.isEmpty(store))) {
      var idx = store.findExact('c_batch', valu);
      if (r != -1) {
        var r = store.getAt(idx);

        var gsisa = r.get('n_gsisa');
        var bsisa = r.get('n_bsisa');

        try {
          tGQty.setMinValue(gsisa * -1);
          tBQty.setMinValue(bsisa * -1);

          if (Ext.isNumber(gsisa)) {
            tGQty.setValue(gsisa);
          }
          else {
            tGQty.setMaxValue(Number.MAX_VALUE);
            tGQty.setValue(0);
          }
          if (Ext.isNumber(bsisa)) {
            tBQty.setValue(bsisa);
          }
          else {
            tBQty.setMaxValue(Number.MAX_VALUE);
            tBQty.setValue(0);
          }
        }
        catch (e) {
          ShowError(e.toString());
        }
      }
      else {
        tGQty.reset();
        tBQty.reset();
      }
    }
   }


//   var validasiTotalPermintaan = function(store, fieldItem, itemNo, good, bad, gsoh, bsoh) {
//     var gIn = good;
//     var bIn = bad;
//     var idx = 0;
//     var rec = '';
//     var qty = 0;
//     var totalReqBad = 0;
//     var totalReqGood = 0;
//     var totalBad = 0;
//     var totalGood = 0;

//     if (store.getCount() < 1) {
//       var ValidQty = gsoh + good;
//       if (ValidQty < 0) {
//         return false;
//       }
//       else {
//         return true;
//       }
//     }


//     var i = 0;

//     for (i = 0; i < store.data.getCount(); i++) {
//       var idx = store.data.items[i].data;
//       qty = idx.n_gqty;
//       totalGood += qty;
//       qty = 0;
//       qty = idx.n_bqty;
//       totalBad += qty;
//     }

//     totalReqBad = totalBad + bIn;
//     totalReqGood = totalGood + gIn;

//     if (totalReqGood < 0) {
//       var qtyReloc = totalReqGood + gsoh;
//       if (qtyReloc < 0) {
//         return false;
//       }
//     }

//     return true;

//   }

  var storeToDetailGrid = function(frm, grid, item, batch, QuantityG, QuantityB, ket) {
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

    var recItem = item.findRecord(item.valueField, item.getValue());
    if (Ext.isEmpty(recItem)) {
        ShowWarning(String.format("Record item '{0}' tidak dapat di baca dari store.", item.getText()));
        return;
    }

    var recBat = batch.findRecord(batch.valueField, batch.getValue());
    if (Ext.isEmpty(recBat)) {
        ShowWarning(String.format("Record item '{0}' tidak dapat di baca dari store.", batch.getText()));
        return;
    }

    var valX = [item.getValue(), batch.getValue()];
    var fieldX = ['c_iteno', 'c_batch'];

    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

    if (!isDup) {
      var gqty = QuantityG.getValue();
      var bqty = QuantityB.getValue();
      var itemNo = item.getValue();
      var Bat = batch.getValue();
      var keterangan = ket.getValue();

      store.insert(0, new Ext.data.Record({
        'c_iteno': itemNo,
        'v_itnam': item.getText(),
        'c_batch': Bat,
        'n_gqty': gqty,
        'n_bqty': bqty,
        'v_ket': keterangan,
        'l_new': true
      }));

//      var batr = batch.getStore();
//      
//      var sBad = 0;
//      var sGood = 0;
//      var sOh = 0;
//      for (sOh = 0; sOh < batr.data.getCount(); sOh++)
//      {
//        sBad += batr.data.items[sOh].data.n_bsisa;
//        sGood += batr.data.items[sOh].data.n_gsisa;
//      }

//      if (validasiTotalPermintaan(store, 'c_iteno', itemNo, gqty, bqty, sGood, sBad)) {
//        store.insert(0, new Ext.data.Record({
//          'c_iteno': itemNo,
//          'v_itnam': item.getText(),
//          'c_batch': Bat,
//          'n_gqty': gqty,
//          'n_bqty': bqty,
//          'v_ket': keterangan,
//          'l_new': true
//        }));

        resetEntryWhenChangeInt();

//      } else {
//        ShowError('Mohon diperiksa kembali, apakah jumlah yang telah diinput telah melebihi dari permintaan/SOH gudang.');
      //      }

    }
    else {
      ShowError('Data telah ada.');

      return false;
    }
  }
  var resetEntryWhenChangeInt = function(g) {
    var item = Ext.getCmp('<%= cbItemDtl.ClientID %>'),
      batch = Ext.getCmp('<%= cbBatDtl.ClientID %>'),
      quantityG = Ext.getCmp('<%= txGQtyDtl.ClientID %>'),
      quantityB = Ext.getCmp('<%= txBQtyDtl.ClientID %>'),
      ket = Ext.getCmp('<%= txKetDtl.ClientID %>');

    if (!Ext.isEmpty(item)) {
      item.reset();
    }
    if (!Ext.isEmpty(batch)) {
      batch.reset();
    }
    if (!Ext.isEmpty(quantityG)) {
      quantityG.reset();
    }
    if (!Ext.isEmpty(quantityB)) {
      quantityB.reset();
    }
    if (!Ext.isEmpty(ket)) {
      ket.reset();
    }
    if (!Ext.isEmpty(g)) {
      g.getStore().removeAll();
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
                  rec.set('ketDel', txt);
                }
              });
          }
        });
    }
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="825" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="825" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfADJStockNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="100" MaxHeight="100" Collapsible="false">
        <ext:Panel runat="server" Title="Header" Height="100" Padding="10" Layout="Column">
          <Items>
            <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
              <Items>
                <ext:ComboBox ID="cbGudangHdr" runat="server" FieldLabel="Gudang" DisplayField="v_gdgdesc"
                  ValueField="c_gdg" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="250"
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
                        <ext:Parameter Name="model" Value="2031" />
                        <ext:Parameter Name="parameters" Value="[['@contains.c_gdg.Contains(@0) || @contains.v_gdgdesc.Contains(@0)', paramTextGetter(#{cbGudangHdr}), '']]" Mode="Raw" />
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
                  <Triggers>
                    <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                  </Triggers>
                  <Listeners>
                    <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                    <Change Handler="clearRelatedComboRecursive(true, #{cbItemDtl});" />                  
                  </Listeners>
                </ext:ComboBox>
                <ext:TextField ID="txKeteranganHdr" runat="server" FieldLabel="Keterangan" MaxLength="100"
                  Width="200" />
              </Items>
            </ext:Panel>  
          </Items>
        </ext:Panel>
       </North>
      <Center MinHeight="150">
        <ext:Panel runat="server" Layout="Fit" Title="Daftar Items">
          <TopBar>
            <ext:Toolbar runat="server">
              <Items>
                <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                  LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                  <Items>
                    <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" DisplayField="v_itnam"
                      ValueField="c_iteno" Width="250" ItemSelector="tr.search-item" PageSize="10" ListWidth="350"
                      MinChars="3" EmptyText="Pilihan..." AllowBlank="true" ForceSelection="false">
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
                            <ext:Parameter Name="model" Value="2061" />
                            <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean'],
                            ['l_hide = @0', false, 'System.Boolean'],
                            ['@contains.c_iteno.Contains(@0) || @contains.v_itnam.Contains(@0)', paramTextGetter(#{cbItemDtl}), '']]" Mode="Raw" />
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
                        <Change Handler="clearRelatedComboRecursive(true, #{cbBatDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbBatDtl" runat="server" FieldLabel="Batch" ItemSelector="tr.search-item"
                      PageSize="10" ListWidth="350" DisplayField="c_batch" ValueField="c_batch" 
                      AllowBlank="true" ForceSelection="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="true" />
                      </CustomConfig>
                      <Store>
                        <ext:Store runat="server" >
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="0077-1" />
                            <ext:Parameter Name="parameters" Value="[['gdg', #{cbGudangHdr}.getValue(), 'System.Char'],
                                                          ['item', #{cbItemDtl}.getValue(), 'System.String'],
                                                          ['@contains.c_batch.Contains(@0)', paramTextGetter(#{cbBatDtl}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_batch" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_iteno" />
                                <ext:RecordField Name="c_batch" />
                                <ext:RecordField Name="n_gsisa" />
                                <ext:RecordField Name="n_bsisa" />
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
                          <td class="body-panel">Good Qty</td>
                          <td class="body-panel">Bad Qty</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_batch}</td>
                            <td>{n_gsisa}</td>
                            <td>{n_bsisa}</td>
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
                        <Change Handler="onChangeAdjBat(this, newValue, #{txGQtyDtl}, #{txBQtyDtl});" />
                        <%--<Select Handler="selectedItemBatch(this, record, #{txGQtyDtl}, #{txBQtyDtl})" />--%>
                      </Listeners>
                    </ext:ComboBox>
                     <ext:NumberField ID="txGQtyDtl" runat="server" FieldLabel="Good" AllowNegative="true"
                      Width="75" AllowBlank="false" AllowDecimals="true" DecimalPrecision="2"/>
                    <ext:NumberField ID="txBQtyDtl" runat="server" FieldLabel="Bad" AllowNegative="true"
                      Width="75" AllowBlank="false" AllowDecimals="true" DecimalPrecision="2"/>
                    <ext:TextField ID="txKetDtl" runat="server" FieldLabel="Keterangan" AllowBlank="true" />
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{cbBatDtl}, #{txGQtyDtl}, #{txBQtyDtl}, #{txKetDtl});" />
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
                <ext:Store ID="Store2" runat="server" RemotePaging="false" RemoteSort="false" >
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0075" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_adjno = @0', #{hfADJStockNo}.getValue(), 'System.String'],
                                                             ['c_type = @0','03','System.String' ]]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="v_undes" />
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="n_gqty" Type="Float" />
                        <ext:RecordField Name="n_bqty" Type="Float" />
                        <ext:RecordField Name="v_ket"  />
                        <ext:RecordField Name="l_new" Type="Boolean" />
                        <ext:RecordField Name="l_void" Type="Boolean" />
                        <ext:RecordField Name="l_modified" Type="Boolean" />
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
                    <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfADJStockNo}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="250" />
                  <ext:Column DataIndex="c_batch" Header="Batch " />
                  <ext:NumberColumn DataIndex="n_gqty" Header="Good Qty" Format="0.000,00/i"
                    Width="75">
                  </ext:NumberColumn>
                  <ext:NumberColumn DataIndex="n_bqty" Header="Bad Qty" Format="0.000,00/i"
                    Width="75">
                   </ext:NumberColumn>
                  <ext:Column DataIndex="v_ket" Header="Keterangan" >
                  </ext:Column>
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners> 
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
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
        <Click>
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});" ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfPlNo}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfADJStockNo}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GudangID" Value="#{cbGudangHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GudangDesc" Value="#{cbGudangHdr}.getText()" Mode="Raw" />
            <ext:Parameter Name="Keterangan" Value="#{txKeteranganHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
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