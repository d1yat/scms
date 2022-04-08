<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SuratPesananManualCtrl.ascx.cs"
  Inherits="transaksi_pembelian_SuratPesananCtrlManual" %>

<script type="text/javascript">
  var storeToDetailGrid = function(frm, grid, item, quantity, ketDtl) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }

    if (
        Ext.isEmpty(grid) ||
        Ext.isEmpty(item) ||
        Ext.isEmpty(quantity) ||
        Ext.isEmpty(ketDtl)) {
      ShowWarning("Objek tidak terdefinisi.");
      return;
    }

    var store = grid.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
    }

    var valX = [item.getValue()];
    var fieldX = ['c_iteno'];

    //    var isDup = false;
    //    var nDup = 0;

    //    // Find Duplicate entry
    //    for (var loop = 0; loop < valX.length; loop++) {
    //      if (store.findExact(fieldX[loop], valX[loop]) != -1) {
    //        nDup++;
    //      }
    //    }

    //    if (nDup == valX.length) {
    //      isDup = true;
    //    }

    var isDup = findDuplicateEntryGrid(store, fieldX, valX);
    var recBat = item.findRecord(item.valueField, item.getValue());

    if (!isDup) {
      var qty = quantity.getValue();
//    var accQty = acceptance.getValue();
      var ketInpt = ketDtl.getValue().trim();
      var itemNo = item.getValue();
//      var avgSales = recBat.get('n_avgSales');

      store.insert(0, new Ext.data.Record({
        'c_iteno': itemNo,
        'v_itemdesc': item.getText(),
        'n_QtyRequest': qty,
//        'n_QtyApprove': accQty,
//        'n_sisa': accQty,
//        'n_avgSales': avgSales,
        
        'v_keterangan': ketInpt,
        'l_new': true
      }));

      item.reset();
      quantity.reset();
//    acceptance.reset();
      ketDtl.reset();
    }
    else {
      ShowError('Data telah ada.');

      return false;
    }
  }
  var prepareCommands = function(rec, toolbar, valX) {
    var del = toolbar.items.get(0); // delete button
//    var vd = toolbar.items.get(1); // void button
//    var rt = toolbar.items.get(2); // void button

    var isNew = false;

    if (!Ext.isEmpty(rec)) {
      isNew = rec.get('l_new');
    }

    if (Ext.isEmpty(valX) || isNew) {
      del.setVisible(true);
//      vd.setVisible(false);
//      rt.setVisible(false);
    }
    else {
      del.setVisible(false);
//      vd.setVisible(true);
//      rt.setVisible(true);
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
  var focusOnAccDetail = function(o, val) {
    o.setMaxValue(val);
  }
  var resetSisaSPQtyCmd = function(rec) {
    if (rec.get('l_new') || rec.get('l_void')) {
      return;
    }
//    var sisa = rec.get('n_sisa');
    var reqQty = rec.get('n_QtyRequest');
    var inUseQty = (reqQty - sisa);
//    var qtyTotal = (rec.get('n_totalAcc') || 0) - inUseQty;

    ShowConfirm('Hapus ?', 'Apakah anda yakin ingin mereset jumlah sisa data ini menjadi \'0\' ?',
        function(btn) {
          if (btn == 'yes') {
            ShowAsk('Alasan ?', 'Masukkan alasan kenapa harus dibatalkan.',
                    function(btnP, txt) {
                      if (btnP == 'ok') {
                        if (txt.trim().length < 1) {
                          txt = 'Kesalahan pemakai.';
                        }

                        rec.set('l_modified', true);
                        rec.set('l_reset', true);
//                        rec.set('n_sisa', 0);
//                        rec.set('n_totalAcc', qtyTotal);
                        rec.set('n_QtyApprove', inUseQty);
                        rec.set('n_QtyOriginal', reqQty);

                        rec.set('v_keterangan', txt);
                      }
                    });
          }
        });
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfSpNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfGudang" runat="server" />
    <ext:Hidden ID="hfTypeNameCtrl" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="175" MaxHeight="175" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="100" Padding="10">
          <Items>
            <ext:ComboBox ID="cbCustomerHdr" runat="server" FieldLabel="Cabang" DisplayField="v_cunam"
              ValueField="c_cusno" Width="350" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
              MinChars="3">
              <Store>
                <ext:Store runat="server">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                    <ext:Parameter Name="model" Value="2011" />
                    <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                      ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerHdr}), '']]" Mode="Raw" />
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
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <%--CABANG--%>
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
                <Change Handler="resetEntryWhenChange(#{gridDetail}, #{frmpnlDetailEntry});" />
              </Listeners>
            </ext:ComboBox>
            <%--TANGGAL--%>
            <ext:CompositeField runat="server" FieldLabel="Tanggal">
              <Items>
                <ext:DateField ID="txTanggal" runat="server" AllowBlank="false" Format="dd-MM-yyyy"
                  EnableKeyEvents="true" />
              </Items>
            </ext:CompositeField>
            
            <%--PJ08--%>
            
            <%--<ext:TextField ID="txSpCabang" runat="server" AllowBlank="false" FieldLabel="Nomor cabang"
              MaxLength="10" Width="200" />
            <ext:TextField ID="txKeterangan" runat="server" FieldLabel="Keterangan" MaxLength="100"
              Width="400" />
            <ext:Checkbox ID="chkCheck" runat="server" FieldLabel="Periksa" />--%>
            
            
          </Items>
        </ext:FormPanel>
      </North>
      
      <%--DAFTAR ITEM--%>
      
      <Center MinHeight="150">
        <ext:Panel ID="pnlGridDetail" runat="server" Title="Daftar Items" Height="150" Layout="Fit">
          <TopBar>
            <ext:Toolbar runat="server">
              <Items>
                <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                  LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                  <Items>
                  <%--PJ08--%>
                   <%-- <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" DisplayField="v_itnam"
                      ValueField="c_iteno" Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="500"
                      MinChars="3">
                      <Store>
                        <ext:Store runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="7001" />
                            <ext:Parameter Name="parameters" Value="[['activateSuplier', true, 'System.Boolean'],
                                                                    ['l_aktif = @0', true, 'System.Boolean'],
                                                                    ['l_hide = @0', false, 'System.Boolean'],
                                                                    ['spno', paramValueGetter(#{hfSpNo}), 'System.String'],
                                                                    ['custno', paramValueGetter(#{cbCustomerHdr}), 'System.String'],
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
                                <ext:RecordField Name="v_nama_suplier" />
                                <ext:RecordField Name="n_avgSales" Type="Float" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 500px">
                        <tr>
                        <td class="body-panel">Kode</td><td class="body-panel">Nama</td>
                        <td class="body-panel">Rata-rata</td><td class="body-panel">Pemasok</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_iteno}</td><td>{v_itnam}</td>
                        <td>{n_avgSales}</td><td>{v_nama_suplier}</td>
                        </tr></tpl>
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
                    
                    
                    <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" DisplayField="v_itnam"
                      ValueField="c_iteno" Width="250" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
                      MinChars="3" EmptyText="Pilihan..." AllowBlank="false" ForceSelection="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="false" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store1" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="2061" />
                            <ext:Parameter Name="parameters" Value="[['@contains.v_itnam.Contains(@0) || @contains.c_iteno.Contains(@0)', paramTextGetter(#{cbItemDtl}), '']]"
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
                      </Listeners>
                    </ext:ComboBox>
                    
                   <%--PJ08 END--%>
                   
                 <ext:NumberField ID="txQtyDtl" runat="server" FieldLabel="Jumlah" AllowBlank="false"
                      AllowDecimals="true" AllowNegative="false" Width="75" />
                      
                 <%--   <ext:NumberField ID="txAccDtl" runat="server" FieldLabel="Disetujui" AllowBlank="false"
                      AllowDecimals="true" AllowNegative="false" Width="75">
                      <Listeners>
                        <Focus Handler="focusOnAccDetail(this, #{txQtyDtl}.getValue());" />
                      </Listeners>
                    </ext:NumberField>--%>
                    
                    <ext:TextField ID="txKetDtl" runat="server" FieldLabel="Keterangan" AllowBlank="true"
                      Width="250" />
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{txQtyDtl}, #{txKetDtl});" />
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
                    <ext:Parameter Name="model" Value="0012" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['spno', #{hfSpNo}.getValue(), 'System.String']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_iteno" TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itemdesc" />
                        <ext:RecordField Name="n_QtyRequest" Type="Float" />
                        <ext:RecordField Name="n_QtyApprove" Type="Float" />
                       <%-- <ext:RecordField Name="n_sisa" Type="Float" />--%>
                        <%--<ext:RecordField Name="n_avgSales" Type="Float" />--%>
                        <%--<ext:RecordField Name="n_totalAcc" Type="Float" />--%>
                        <ext:RecordField Name="v_keterangan" />
                        <ext:RecordField Name="n_QtyOriginal" Type="Float" />
                        <ext:RecordField Name="l_new" Type="Boolean" />
                        <ext:RecordField Name="l_void" Type="Boolean" />
                        <ext:RecordField Name="l_modified" Type="Boolean" />
                        <ext:RecordField Name="l_reset" Type="Boolean" />
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
                    <%--  <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                      <ext:GridCommand CommandName="Reset" Icon="BasketRemove" ToolTip-Title="Command" ToolTip-Text="Reset Sisa Menjadi 0" />--%>
                    </Commands>
                    <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfSpNo}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itemdesc" Header="Nama Barang" Width="200" />
                  <ext:NumberColumn DataIndex="n_QtyRequest" Header="Jumlah" Format="0.000,00/i" Width="50" />
                 <%-- <ext:NumberColumn DataIndex="n_QtyApprove" Header="Disetujui" Format="0.000,00/i"
                    Width="50" />
                  <ext:NumberColumn DataIndex="n_sisa" Header="Sisa" Format="0.000,00/i" Width="50" />
                  <ext:NumberColumn DataIndex="n_avgSales" Header="Rata-rata" Format="0.000,00/i" Width="75" />
                  <ext:NumberColumn DataIndex="n_totalAcc" Header="Total Disetujui" Format="0.000,00/i"
                    Width="75" />--%>
                  <ext:Column DataIndex="v_keterangan" Header="Keterangan" Width="100" />
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); } else if (command == 'Reset') { resetSisaSPQtyCmd(record); }" />
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
            <ext:Parameter Name="NumberID" Value="#{hfSpNo}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="btnSave" runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="CustomerID" Value="#{cbCustomerHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfSpNo}.getValue()" Mode="Raw" />
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
