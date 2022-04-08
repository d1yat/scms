<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MasterItemViaCtrl.ascx.cs" Inherits="master_item_MasterItemViaCtrl" %>

         
<script type="text/javascript">

    var storeToDetailGridMulti = function(frm, tipe, grid, item) {
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

        var kodeVia = tipe.getValue();

        var itemNo = item.getValue();

        var valX = [itemNo];
        var fieldX = ['c_iteno'];
        
        // Find Duplicate entry
        var isDup = findDuplicateEntryGrid(store, fieldX, valX);

        if (!isDup) {
            var recItem = item.findRecord(item.valueField, itemNo);
            var suplName = (Ext.isEmpty(recItem) ? '' : recItem.get('v_nama'));
            var NamaVia = tipe.getText();
            

            store.insert(0, new Ext.data.Record({
               
            'v_viadesc': NamaVia,
            'c_via': kodeVia,
                'c_iteno': itemNo,
                'v_itnam': item.getText(),
                'l_new': true
            }));

            item.reset();
           // quantity.reset();
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
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="120" Padding="10">
          <Items>
            <ext:ComboBox ID="cbGudang" runat="server" FieldLabel="Gudang" DisplayField="v_gdgdesc"
              ValueField="c_gdg" Width="175" PageSize="10" ListWidth="200" ItemSelector="tr.search-item"
              MinChars="3" AllowBlank="false" ForceSelection="false">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
              </CustomConfig>
              <Store>
                <ext:Store ID="Store5" runat="server" RemotePaging="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <%--<ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />--%>
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="2031" />
                    <ext:Parameter Name="parameters" Value="[]"
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
              <Template ID="Template3" runat="server">
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
                <Change Handler="clearRelatedComboRecursive(true, #{cbCabang}, #{cbItemDtl});" />
              </Listeners>
            </ext:ComboBox>
            <ext:ComboBox ID="cbCabang" runat="server" DisplayField="v_desc" ValueField="v_kode"
                Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
                AllowBlank="true" FieldLabel="Gudang / Cabang">
                <Store>
                  <ext:Store ID="Store1" runat="server">
                    <Proxy>
                      <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                        CallbackParam="soaScmsCallback" />
                    </Proxy>
                    <BaseParams>
                      <ext:Parameter Name="start" Value="={0}" />
                      <ext:Parameter Name="limit" Value="={10}" />
                      <ext:Parameter Name="model" Value="110001" />
                      <ext:Parameter Name="parameters" Value="[['v_kode != @0', #{cbGudang}.getValue().trim(), 'System.String'], 
                      ['@contains.v_kode.Contains(@0) || @contains.v_desc.Contains(@0)', paramTextGetter(#{cbCabang}), '']]"
                        Mode="Raw" />
                      <ext:Parameter Name="sort" Value="" />
                      <ext:Parameter Name="dir" Value="" />
                    </BaseParams>
                    <Reader>
                      <ext:JsonReader IDProperty="v_kode" Root="d.records" SuccessProperty="d.success"
                        TotalProperty="d.totalRows">
                        <Fields>
                          <ext:RecordField Name="v_kode" />
                          <ext:RecordField Name="v_desc" />
                        </Fields>
                      </ext:JsonReader>
                    </Reader>
                  </ext:Store>
                </Store>
                <Template ID="Template2" runat="server">
                  <Html>
                  <table cellpading="0" cellspacing="0" style="width: 400px">
                    <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                    <tpl for="."><tr class="search-item">
                      <td>{v_kode}</td><td>{v_desc}</td>
                      </td>
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
              <ext:ComboBox ID="cbSuplier" runat="server" FieldLabel="Pemasok" ValueField="c_nosup"
                DisplayField="v_nama" Width="250" ListWidth="500" PageSize="10" ItemSelector="tr.search-item"
                AllowBlank="true" ForceSelection="false">
                <CustomConfig>
                  <ext:ConfigItem Name="allowBlank" Value="true" />
                </CustomConfig>
                <Store>
                  <ext:Store ID="Store6" runat="server">
                    <Proxy>
                      <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                        CallbackParam="soaScmsCallback" />
                    </Proxy>
                    <BaseParams>
                      <ext:Parameter Name="start" Value="={0}" />
                      <ext:Parameter Name="limit" Value="={10}" />
                      <ext:Parameter Name="model" Value="2021" />
                      <ext:Parameter Name="parameters" Value="[['l_hide != @0', true, 'System.Boolean'],
                          ['l_aktif == @0', true, 'System.Boolean'],
                          ['@contains.c_nosup.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbSuplier}), '']]"
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
                <Template ID="Template4" runat="server">
                  <Html>
                  <table cellpading="0" cellspacing="0" style="width: 500px">
                  <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
                  <tpl for=".">
                    <tr class="search-item">
                      <td>{c_nosup}</td><td>{v_nama}</td>
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
                <Change Handler="clearRelatedComboRecursive(true, #{cbItemDtl});" />
              </Listeners>
              </ext:ComboBox>
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
                    <ext:ComboBox ID="cbTipe" runat="server" FieldLabel="Tipe" DisplayField="v_ket"
                      ValueField="c_type" Width="150" AllowBlank="false" ForceSelection="false">
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
                            <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                              ['c_notrans = @0', '02', '']]" Mode="Raw" />
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
                    <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" DisplayField="v_itnam" 
                    Disabled="false" ValueField="c_iteno" Width="300" ItemSelector="tr.search-item" 
                    PageSize="10" ListWidth="500"
                      MinChars="3">
                      <Store>
                        <ext:Store ID="Store2" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="0175" />
                            <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean'],
                                                                    ['l_hide = @0', false, 'System.Boolean'],
                                                                    ['gdg', #{cbGudang}.getValue(), 'System.Char'],
                                                                    ['c_nosup = @0', #{cbSuplier}.getValue(), 'System.String'],
                                                                    ['@contains.c_iteno.Contains(@0) || @contains.v_itnam.Contains(@0)', paramTextGetter(#{cbItemDtl}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_iteno" />
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
                      <Template ID="Template1" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 500px">
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
                        <Click Handler="storeToDetailGridMulti(#{frmpnlDetailEntry}, #{cbTipe}, #{gridDetail}, #{cbItemDtl});" />
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
                <ext:Store ID="Store3" runat="server" RemotePaging="false" RemoteSort="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <AutoLoadParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="-1" />
                  </AutoLoadParams>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="={0}" Mode="Raw" />
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
                        <ext:RecordField Name="v_viadesc" />
                        <ext:RecordField Name="c_via" />
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
                  <ext:Column ColumnID="v_viadesc" DataIndex="v_viadesc" Header="Desc Via" Width="75" />
                  <ext:Column ColumnID="c_iteno" DataIndex="c_iteno" Header="kode Item" Width="75" />
                  <ext:Column ColumnID="v_itnam" DataIndex="v_itnam" Header="Deskripsi" Width="250" />
                  <%--<ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />--%>
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
              </Listeners>
              <BottomBar>
               <%--<ext:PagingToolbar runat="server" ID="gmPagingBB" PageSize="20">
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
                </ext:PagingToolbar>--%>
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
      <DirectEvents>
        <Click OnEvent="ReloadBtn_Click">
          <EventMask ShowMask="true" />
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="Button1" runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />
