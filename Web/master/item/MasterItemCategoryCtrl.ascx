<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MasterItemCategoryCtrl.ascx.cs" 
Inherits="master_item_MasterItemCategoryCtrl" %>

<script type="text/javascript">

  var storeToDetailGridMulti = function(frm, grid, item) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }

    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(item)) {
      ShowWarning("Objek tidak terdefinisi.");
      return;
    }

    var store = grid.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
    }

    var itemNo = item.getValue();

    var valX = [itemNo];
    var fieldX = ['c_iteno'];

    // Find Duplicate entry
    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

    if (!isDup) {
      var recItem = item.findRecord(item.valueField, itemNo);
      var suplName = (Ext.isEmpty(recItem) ? '' : recItem.get('v_nama'));

      store.insert(0, new Ext.data.Record({
        'c_iteno': itemNo,
        'v_itnam': item.getText(),
        'l_new': true
      }));

      item.reset();
      quantity.reset();
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
  
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfType" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="80" MaxHeight="80" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="80"
          Layout="Fit" ButtonAlign="Center" MonitorValid="true">
          <Items>
            <ext:Panel runat="server" Padding="5" Layout="Column">
              <Items>
                <ext:ComboBox ID="cbType" runat="server" FieldLabel="Tipe" DisplayField="v_ket"
                  ValueField="c_type" Width="350" ListWidth="400" ItemSelector="tr.search-item"
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
                        <ext:Parameter Name="allQuery" Value="true" />
                        <ext:Parameter Name="model" Value="2001" />
                        <ext:Parameter Name="parameters" Value="[['@contains.c_type.Contains(@0) || @contains.v_ket.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerHdr}), ''],
                        ['c_portal = @0', '9', 'System.Char'],
                        ['c_notrans = @0', '001', '']]"
                          Mode="Raw" />
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
                  <Template ID="Template2" runat="server">
                    <Html>
                    <table cellpading="0" cellspacing="0" style="width: 350px">
                    <tr>
                      <td class="body-panel">Kode</td><td class="body-panel">Nama</td>
                    </tr>
                    <tpl for="."><tr class="search-item">
                      <td>{c_type}</td><td>{v_ket}</td>
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
                  <DirectEvents>
                    <Change OnEvent="onchangeBtn">
                      <ExtraParams>
                        <ext:Parameter Name="Tipe" Value="#{cbType}.getValue()" Mode="Raw" />
                      </ExtraParams>
                    </Change>
                  </DirectEvents>
                </ext:ComboBox>
              </Items>
            </ext:Panel>
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
                       ListWidth="350" DisplayField="v_itnam" ValueField="c_iteno" MinChars="3">
                      <Store>
                        <ext:Store runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="-1" />
                            <ext:Parameter Name="model" Value="0173" />
                            <ext:Parameter Name="parameters" Value="[['tipe', #{cbType}.getValue(), 'System.String'],
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
                                <ext:RecordField Name="v_undes" />
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
                          <td class="body-panel">Kemasan</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                          <td>{c_iteno}</td><td>{v_itnam}</td>
                          <td>{v_undes}</td>
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
                        <Click Handler="storeToDetailGridMulti(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl});" />
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
                  <AutoLoadParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={20}" />
                  </AutoLoadParams>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB}.getValue())" Mode="Raw" />
                    <ext:Parameter Name="model" Value="0170" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_type = @0', #{hfType}.getValue(), 'System.String'],
                    ['v_itnam', paramValueGetter(#{txNamaFltrDtl}) + '%', ''],
                    ['c_iteno', paramValueGetter(#{txKodeFltrDtl}) + '%', '']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success" IDProperty="c_iteno">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="c_type" />
                        <ext:RecordField Name="v_ket" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="v_acronim" />
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
                    <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfType}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column ColumnID="c_iteno" DataIndex="c_iteno" Header="Deskripsi" Width="200" />
                  <ext:Column ColumnID="v_itnam" DataIndex="v_itnam" Header="Deskripsi" Width="200" />
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <View>
                <ext:GridView runat="server" StandardHeaderRow="true">
                  <HeaderRows>
                    <ext:HeaderRow>
                      <Columns>
                        <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                          <Component>
                            <ext:Button ID="ClearFilterButton" runat="server" Icon="Cancel" ToolTip="Clear filter">
                              <Listeners>
                                <Click Handler="clearFilterGridHeader(#{gridMain}, #{txKodeFltr}, #{txNamaFltr});reloadFilterGrid(#{gridDetail});"
                                  Buffer="300" Delay="300" />
                              </Listeners>
                            </ext:Button>
                          </Component>
                        </ext:HeaderColumn>
                        <ext:HeaderColumn>
                          <Component>
                            <ext:TextField ID="txKodeFltrDtl" runat="server" EnableKeyEvents="true" AllowBlank="true">
                              <Listeners>
                                <KeyUp Handler="reloadFilterGrid(#{gridDetail})" Buffer="700" Delay="700" />
                              </Listeners>
                            </ext:TextField>
                          </Component>
                        </ext:HeaderColumn>
                        <ext:HeaderColumn>
                          <Component>
                            <ext:TextField ID="txNamaFltrDtl" runat="server" EnableKeyEvents="true" AllowBlank="true">
                              <Listeners>
                                <KeyUp Handler="reloadFilterGrid(#{gridDetail})" Buffer="700" Delay="700" />
                              </Listeners>
                            </ext:TextField>
                          </Component>
                        </ext:HeaderColumn>
                      </Columns>
                    </ext:HeaderRow>
                  </HeaderRows>
                </ext:GridView>
              </View>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
              </Listeners>
              <BottomBar>
                <ext:PagingToolbar runat="server" ID="gmPagingBB" PageSize="20">
                  <Items>
                    <ext:Label ID="Label1" runat="server" Text="Page size:" />
                    <ext:ToolbarSpacer runat="server" Width="10" />
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
    <ext:Button ID="btnSave" runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfType}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="btnReload" runat="server" Icon="Reload" Text="Bersihkan">
      <%--<DirectEvents>
        <Click OnEvent="ReloadBtn_Click">
          <EventMask ShowMask="true" />
        </Click>
      </DirectEvents>--%>
    </ext:Button>
    <ext:Button runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />
