<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MasterDivisiAMSCtrl.ascx.cs"
  Inherits="master_ams_MasterDivisiAMSCtrl" %>

<script type="text/javascript">

  var storeToDetailGrid = function(frm, grid, Item) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }


    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(Item)) {
      ShowWarning("Objek tidak terdefinisi.");
      return;
    }

    var store = grid.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
    }

    var valX = [Item.getValue()];
    var fieldX = ['c_iteno'];

    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

    if (!isDup) {
      var kdItem = Item.getValue().trim();
      var nmItem = Item.getText();


      store.insert(0, new Ext.data.Record({
        'c_iteno': kdItem,
        'v_itnam': nmItem,
        //        'v_undes': glVal,
        //        'v_acronim': tipTex,
        'l_new': true
      }));

      Item.reset();
    }
    else {
      ShowError('Data telah ada.');

      return false;
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
</script>

<ext:Window ID="winDetail" runat="server" Height="525" Width="800" Hidden="true"
  Maximizable="true" MinHeight="525" MinWidth="800" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfDivAMSId" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="125" MaxHeight="125" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="155" Padding="10">
          <Items>
            <ext:TextField ID="txNamaDivAMS" runat="server" MaxLength="20" FieldLabel="Bank"
              Width="150" />
            <ext:Checkbox ID="chkAktif" runat="server" FieldLabel="Aktif" />
            <ext:Checkbox ID="chkHide" runat="server" FieldLabel="Hide" />
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
                    <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" Width="200" ItemSelector="tr.search-item"
                      PageSize="10" ListWidth="350" DisplayField="v_itnam" ValueField="c_iteno" MinChars="3">
                      <Store>
                        <ext:Store ID="Store2" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="2061" />
                            <ext:Parameter Name="parameters" Value="[['@contains.c_iteno.Contains(@0) || @contains.v_itnam.Contains(@0)', paramTextGetter(#{cbItemDtl}), '']]"
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
                                <ext:RecordField Name="v_undes" />
                                <ext:RecordField Name="v_acronim" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template1" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 350px">
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
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl});" />
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
              <LoadMask ShowMask="true" />
              <SelectionModel>
                <ext:RowSelectionModel SingleSelect="true" />
              </SelectionModel>
              <Store>
                <ext:Store ID="Store1" runat="server" SkinID="OriginalExtStore" RemoteSort="true">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <AutoLoadParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={20}" />
                  </AutoLoadParams>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB}.getValue())" Mode="Raw" />
                    <ext:Parameter Name="model" Value="0202" />
                    <ext:Parameter Name="sort" Value="v_itnam" />
                    <ext:Parameter Name="dir" Value="ASC" />
                    <ext:Parameter Name="parameters" Value="[['c_kddivams = @0', #{hfDivAMSId}.getValue(), 'System.String']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                      IDProperty="c_iteno">
                      <Fields>
                        <ext:RecordField Name="c_kddivams" />
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="v_undes" />
                        <ext:RecordField Name="v_acronim" />
                        <ext:RecordField Name="l_aktif" Type="Boolean" />
                        <ext:RecordField Name="l_hide" Type="Boolean" />
                        <ext:RecordField Name="l_new" Type="Boolean" />
                        <ext:RecordField Name="l_void" Type="Boolean" />
                        <ext:RecordField Name="l_modified" Type="Boolean" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                  <SortInfo Direction="ASC" Field="v_itnam" />
                </ext:Store>
              </Store>
              <ColumnModel>
                <Columns>
                  <ext:CommandColumn Width="25">
                    <Commands>
                      <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                      <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                    </Commands>
                    <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfDivAMSId}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column ColumnID="c_iteno" DataIndex="c_iteno" Header="Kode Item" Width="70">
                  </ext:Column>
                  <ext:Column ColumnID="v_itnam" DataIndex="v_itnam" Header="Nama Item" Width="350">
                  </ext:Column>
                  <%--<ext:Column ColumnID="v_undes" DataIndex="v_undes" Header="No GL" Width="150" >
                  </ext:Column>
                  <ext:Column ColumnID="v_acronim" DataIndex="v_acronim" Header="Tipe" Width="130" >
                  </ext:Column>--%>
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                  <ext:CheckColumn DataIndex="l_modified" Header="Modified" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
              </Listeners>
              <BottomBar>
                <ext:PagingToolbar runat="server" ID="gmPagingBB" PageSize="20">
                  <Items>
                    <ext:Label ID="Label1" runat="server" Text="Page size:" />
                    <ext:ToolbarSpacer ID="ToolbarSpacer1" runat="server" Width="10" />
                    <ext:ComboBox ID="cbGmPagingBB" runat="server" Width="80">
                      <Items>
                        <ext:ListItem Text="5" />
                        <ext:ListItem Text="10" />
                        <ext:ListItem Text="20" />
                        <ext:ListItem Text="50" />
                        <ext:ListItem Text="100" />
                      </Items>
                      <SelectedItem Value="20" />
                      <Listeners>
                        <Select Handler="#{gmPagingBB}.pageSize = parseInt(this.getValue()); #{gmPagingBB}.doLoad();" />
                      </Listeners>
                    </ext:ComboBox>
                  </Items>
                </ext:PagingToolbar>
              </BottomBar>
            </ext:GridPanel>
          </Items>
        </ext:Panel>
      </Center>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <ext:Button ID="Button1" runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfDivAMSId}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Nama" Value="#{txNamaDivAMS}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Aktif" Value="#{chkAktif}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Hide" Value="#{chkHide}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="Button3" runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
