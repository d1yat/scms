<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MemoSTTSampleCtrl.ascx.cs" 
Inherits="transaksi_memo_MemoSTTSampleCtrl" %>

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
  
  var storeToDetailGrid = function(frm, grid, item,  Quantity) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }

    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(item) ||
          Ext.isEmpty(Quantity)) {
      ShowWarning("Objek tidak terdefinisi.");
      return;
    }

    var store = grid.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
    }

    var valX = [ item.getValue()];
    var fieldX = ['c_iteno'];

    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

    if (!isDup) {
      var qty = Quantity.getValue();
      var itemNo = item.getValue();

      store.insert(0, new Ext.data.Record({
        'c_iteno': itemNo,
        'v_itnam': item.getText(),
        'n_qty': qty,
        'l_new': true
      }));

      resetEntryWhenChangeInt();
    }
    else {
      ShowError('Data telah ada.');

      return false;
    }
  }

  var resetEntryWhenChangeInt = function(g) {
    var item = Ext.getCmp('<%= cbItemDtl.ClientID %>'),
      Qty = Ext.getCmp('<%= txQtyDtl.ClientID %>')

    if (!Ext.isEmpty(item)) {
      item.reset();
    }
    if (!Ext.isEmpty(Qty)) {
      Qty.reset();
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
                      rec.set('v_ket', txt);
                    }
                  });
              }
            });
    }
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="1000" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="825" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfMemo" runat="server" />
    <ext:Hidden ID="hfMemoType" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
     <North MinHeight="130" MaxHeight="130" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="130" Padding="10">
          <Items>
            <ext:ComboBox ID="cbGudangHdr" runat="server" FieldLabel="Gudang" DisplayField="v_gdgdesc"
            ValueField="c_gdg" Width="175" PageSize="10" ListWidth="200" ItemSelector="tr.search-item"
            MinChars="3">
            <Store>
              <ext:Store ID="Store2" runat="server" RemotePaging="false">
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
            <Template ID="Template1" runat="server">
              <Html>
              <table cellpading="0" cellspacing="0" style="width: 200px">
                <tr><td class="body-panel">Kode</td><td class="body-panel">From</td></tr>
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
          </ext:ComboBox>
          
            <ext:TextField runat="server" ID="txMemoHdr" Width="450" FieldLabel="No Memorandum" />
            
            <ext:ComboBox ID="cbKryHdr" runat="server" FieldLabel="Pemohon" DisplayField="v_nama"
            ValueField="c_nip" Width="175" PageSize="10" ListWidth="200" ItemSelector="tr.search-item"
            MinChars="3">
            <Store>
              <ext:Store ID="Store1" runat="server" RemotePaging="false">
                <Proxy>
                  <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                    CallbackParam="soaScmsCallback" />
                </Proxy>
                <BaseParams>
                  <ext:Parameter Name="start" Value="={0}" />
                  <ext:Parameter Name="limit" Value="={10}" />
                  <ext:Parameter Name="allQuery" Value="true" />
                  <ext:Parameter Name="model" Value="2111" />
                  <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean']]" Mode="Raw" />
                  <ext:Parameter Name="sort" Value="v_nama" />
                  <ext:Parameter Name="dir" Value="ASC" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader IDProperty="c_gdg" Root="d.records" SuccessProperty="d.success" TotalProperty="d.totalRows">
                    <Fields>
                      <ext:RecordField Name="c_nip" />
                      <ext:RecordField Name="v_nama" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
              </ext:Store>
            </Store>
            <Template ID="Template2" runat="server">
              <Html>
              <table cellpading="0" cellspacing="0" style="width: 200px">
                <tr><td class="body-panel">Nip</td><td class="body-panel">Nama</td></tr>
                <tpl for=".">
                  <tr class="search-item">
                    <td>{c_nip}</td><td>{v_nama}</td>
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
          </ext:ComboBox>
          
            <ext:TextField runat="server" ID="txKeteranganHdr" Width="450" FieldLabel="Keterangan" />
          </Items>
        </ext:FormPanel>
     </North>
     <Center MinHeight="150">
        <ext:Panel ID="pnlGridDetail" runat="server" Title="Daftar Items" Height="150" Layout="Fit">
          <TopBar>
            <ext:Toolbar ID="Toolbar1" runat="server">
              <Items>
                <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                  LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                  <Items>
                    <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" DisplayField="v_itnam"
                      ValueField="c_iteno" Width="250" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
                      MinChars="3" EmptyText="Pilihan..." Mode="Local">
                      <Store>
                        <ext:Store ID="Store3" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="2061" />
                            <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean'],
                                                                     ['l_hide = @0', false, 'System.Boolean']]" Mode="Raw" />
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
                    
                    <ext:NumberField ID="txQtyDtl" runat="server" FieldLabel="Quantity" AllowNegative="false"
                      Width="75" AllowBlank="false" />
                      
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{txQtyDtl});" />
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
                <ext:Store ID="Store5" runat="server" RemotePaging="false" RemoteSort="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0049a" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_adjno = @0', #{hfMemo}.getValue(), 'System.String']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="v_undes" />
                        <ext:RecordField Name="n_qty" Type="Float" />
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
                      <ext:GridCommand CommandName="Delete" Icon="Delete" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                      <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                    </Commands>
                    <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfADJSTTDonasi}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="250" />
                  <ext:NumberColumn DataIndex="n_qty" Header="Good Qty" Format="0.000,00/i"
                    Width="75">
                  </ext:NumberColumn>
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
        <Click >
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfMemo}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="Button1" runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click"> 
          <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="Ext.encode(#{gridDetail}.getRowsValues())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfMemo}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Gudang" Value="#{cbGudangHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="NamaKry" Value="#{cbKryHdr}.getText()" Mode="Raw" />
            <ext:Parameter Name="NipKry" Value="#{cbKryHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GudangDesc" Value="#{cbGudangHdr}.getText()" Mode="Raw" />
            <ext:Parameter Name="Keterangan" Value="#{txKeteranganHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Memo" Value="#{txMemoHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="Button2" runat="server" Icon="Reload" Text="Bersihkan">
    </ext:Button>
    <ext:Button ID="Button3" runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
 </ext:Window>
