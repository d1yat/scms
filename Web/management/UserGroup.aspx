<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="UserGroup.aspx.cs" Inherits="management_UserGroup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
    var onGridRowUserClick = function(rec, fld, grid, cb, hid) {
      var data = rec.get(fld);

      hid.setValue(data);

      var store = grid.getStore();
      if (!Ext.isEmpty(store)) {
        store.clearFilter();

        store.removeAll();
        store.reload();
      }

      store = cb.getStore();
      cb.reset();
      if (!Ext.isEmpty(store)) {
        store.removeAll();
        store.reload();
      }
    }

    var insertNewDataGrp = function(cb, grid) {
      var grp = cb.getValue().trim();
      var txt = cb.getText().trim();
      if (grp.trim().length < 1) {
        ShowError("Grup tidak boleh kosong.");
        return;
      }
      cb.reset();

      var store = grid.getStore();
      if (!Ext.isEmpty(store)) {
        var pos = store.findExact('c_group', grp);
        if (pos != -1) {
          ShowWarning("Grup telah ada.");
        }
        else {
          store.insert(0,
            new Ext.data.Record({
              'l_new': true,
              'c_group': grp,
              'v_group_desc': txt,
              'l_delete': false
            })
          );

          //store.commitChanges();
        }
      }
    }

    var insertNewDataUsr = function(cb, grid) {
      var grp = cb.getValue().trim();
      var txt = cb.getText().trim();
      if (grp.trim().length < 1) {
        ShowError("NIP tidak boleh kosong.");
        return;
      }
      cb.reset();

      var store = grid.getStore();
      if (!Ext.isEmpty(store)) {
        var pos = store.findExact('c_nip', grp);
        if (pos != -1) {
          ShowWarning("NIP telah ada.");
        }
        else {
          store.insert(0,
            new Ext.data.Record({
              'l_new': true,
              'c_nip': grp,
              'v_username': txt,
              'l_delete': false
            })
          );

          //store.commitChanges();
        }
      }
    }

    var deleteUserDetailGridData = function(grid, rec) {
      var store = grid.getStore();
      if (Ext.isEmpty(store)) {
        return;
      }

      if (!Ext.isEmpty(rec)) {

        var isNew = rec.get('l_new');

        ShowConfirm('Hapus ?', 'Apakah anda yakin ingin menghapus data ini ?',
          function(btn) {
            if (btn == 'yes') {
              if (isNew) {
                store.remove(rec);

                //store.commitChanges();
              }
              else {
                rec.set('l_delete', true);

                //rec.commit();

                //store.filter('l_delete', false, false, false, true);
              }
            }
          }
        );
      }
    }

    var resetDetail = function(grid, cb, hid) {
      var store = '';

      if (!Ext.isEmpty(grid)) {
        store = grid.getStore();
        if (!Ext.isEmpty(store)) {
          store.removeAll();
        }
      }

      if (!Ext.isEmpty(cb)) {
        cb.reset();
      }

      if (!Ext.isEmpty(hid)) {
        hid.reset();
      }
    }

    var reloadStoreTab = function(tab, pnlName, grid1, grid2) {
      var store = '';

      if (tab.getId() == pnlName) {
        store = grid1.getStore();
      }
      else {
        store = grid2.getStore();
      }

      store.removeAll();
      store.reload();
    }

    var getGridValueByTab = function(t, p, g1, g2) {
      var restData = '';

      if (t.getActiveTab().getId() == p.getId()) {
        restData = saveStoreToServer(g1.getStore());
      }
      else {
        restData = saveStoreToServer(g2.getStore());
      }

      return restData;
    }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Viewport runat="server" Layout="Fit">
    <Items>
      <ext:Panel runat="server" Layout="Fit">
        <TopBar>
          <ext:Toolbar runat="server">
            <Items>
              <ext:Button runat="server" Text="Segarkan" Icon="ArrowRefresh">
                <Listeners>
                  <Click Handler="refreshGrid(#{gridMain});" />
                </Listeners>
              </ext:Button>
            </Items>
          </ext:Toolbar>
        </TopBar>
        <Items>
          <ext:TabPanel ID="tabUserGroup" runat="server">
            <Listeners>
              <TabChange Handler="reloadStoreTab(tab, #{pnlByUser}.getId(), #{gridUserMain}, #{gridGroupMain});" />
            </Listeners>
            <Items>
              <ext:Panel ID="pnlByUser" runat="server" Title="Groups to User" Layout="Fit">
                <Items>
                  <ext:BorderLayout runat="server">
                    <North Split="true">
                      <ext:Panel runat="server" MinHeight="200" Layout="Fit">
                        <Items>
                          <ext:GridPanel ID="gridUserMain" runat="server">
                            <LoadMask ShowMask="true" />
                            <SelectionModel>
                              <ext:RowSelectionModel SingleSelect="true">
                                <Listeners>
                                  <RowSelect Handler="onGridRowUserClick(record, 'c_nip', #{gridUserDetail}, #{cbGroup}, #{hfUserID});"
                                    Delay="10" />
                                </Listeners>
                              </ext:RowSelectionModel>
                            </SelectionModel>
                            <Store>
                              <ext:Store runat="server">
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
                                  <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBBUG}.getValue())" Mode="Raw" />
                                  <ext:Parameter Name="model" Value="100001" />
                                  <ext:Parameter Name="parameters" Value="[['@contains.c_nip.Contains(@0)', paramValueGetter(#{txNipFltrUG}), ''],
                                  ['@contains.v_username.Contains(@0)', paramValueGetter(#{txNamaFltrUG}), ''],
                                  ['c_gdg = @0', paramValueGetter(#{cbGCFtlrUG}), 'System.String']]" Mode="Raw" />
                                </BaseParams>
                                <Reader>
                                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                                    IDProperty="c_nip">
                                    <Fields>
                                      <ext:RecordField Name="c_nip" />
                                      <ext:RecordField Name="v_gdgdesc" />
                                      <ext:RecordField Name="l_aktif" Type="Boolean" />
                                      <ext:RecordField Name="v_username" />
                                      <ext:RecordField Name="c_gdg" />
                                    </Fields>
                                  </ext:JsonReader>
                                </Reader>
                                <SortInfo Field="c_nip" Direction="ASC" />
                                <Listeners>
                                  <Load Handler="resetDetail(#{gridUserDetail}, #{frmpnlDetailEntry}, #{hfUserID})" />
                                </Listeners>
                              </ext:Store>
                            </Store>
                            <ColumnModel>
                              <Columns>
                                <ext:Column DataIndex="c_nip" Header="Nip" Width="100" />
                                <ext:Column DataIndex="v_username" Header="Nama" Width="350" />
                                <ext:Column DataIndex="v_gdgdesc" Header="Gudang / Cabang" Width="350" />
                                <ext:CheckColumn DataIndex="l_aktif" Header="Aktif" Width="50" />
                              </Columns>
                            </ColumnModel>
                            <View>
                              <ext:GridView runat="server" StandardHeaderRow="true">
                                <HeaderRows>
                                  <ext:HeaderRow>
                                    <Columns>
                                      <ext:HeaderColumn>
                                        <Component>
                                          <ext:TextField ID="txNipFltrUG" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                            <Listeners>
                                              <KeyUp Handler="reloadFilterGrid(#{gridUserMain})" Buffer="700" Delay="700" />
                                            </Listeners>
                                          </ext:TextField>
                                        </Component>
                                      </ext:HeaderColumn>
                                      <ext:HeaderColumn>
                                        <Component>
                                          <ext:TextField ID="txNamaFltrUG" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                            <Listeners>
                                              <KeyUp Handler="reloadFilterGrid(#{gridUserMain})" Buffer="700" Delay="700" />
                                            </Listeners>
                                          </ext:TextField>
                                        </Component>
                                      </ext:HeaderColumn>
                                      <ext:HeaderColumn>
                                        <Component>
                                          <ext:ComboBox ID="cbGCFtlrUG" runat="server" DisplayField="v_desc" ValueField="v_kode"
                                            Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
                                            AllowBlank="true">
                                            <Store>
                                              <ext:Store runat="server" AutoLoad="false">
                                                <Proxy>
                                                  <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                    CallbackParam="soaScmsCallback" />
                                                </Proxy>
                                                <BaseParams>
                                                  <ext:Parameter Name="start" Value="={0}" />
                                                  <ext:Parameter Name="limit" Value="={10}" />
                                                  <ext:Parameter Name="model" Value="110001" />
                                                  <ext:Parameter Name="parameters" Value="[['v_desc', #{cbGCFtlrUG}.getText().trim() + '%', '']]"
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
                                                      <ext:RecordField Name="l_aktif" Type="Boolean" />
                                                    </Fields>
                                                  </ext:JsonReader>
                                                </Reader>
                                              </ext:Store>
                                            </Store>
                                            <Template runat="server">
                                              <Html>
                                              <table cellpading="0" cellspacing="0" style="width: 400px">
                                              <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td>
                                              <td class="body-panel">Aktif</td></tr>
                                              <tpl for="."><tr class="search-item">
                                                <td>{v_kode}</td><td>{v_desc}</td>
                                                <td align="center"><input type="checkbox" value="{l_aktif}" disabled="disabled" /></td>
                                              </tr></tpl>
                                              </table>
                                              </Html>
                                            </Template>
                                            <Listeners>
                                              <Change Handler="reloadFilterGrid(#{gridUserMain})" Buffer="300" Delay="300" />
                                            </Listeners>
                                          </ext:ComboBox>
                                        </Component>
                                      </ext:HeaderColumn>
                                      <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                                        <Component>
                                          <ext:Button ID="ClearFilterButtonUG" runat="server" Icon="Cancel" ToolTip="Clear filter">
                                            <Listeners>
                                              <Click Handler="clearFilterGridHeader(#{gridUserMain}, #{txNipFltrUG}, #{txNamaFltrUG}, #{cbGCFtlrUG});reloadFilterGrid(#{gridUserMain});"
                                                Buffer="300" Delay="300" />
                                            </Listeners>
                                          </ext:Button>
                                        </Component>
                                      </ext:HeaderColumn>
                                    </Columns>
                                  </ext:HeaderRow>
                                </HeaderRows>
                              </ext:GridView>
                            </View>
                            <BottomBar>
                              <ext:PagingToolbar runat="server" ID="gmPagingBBUG" PageSize="20">
                                <Items>
                                  <ext:Label runat="server" Text="Page size:" />
                                  <ext:ToolbarSpacer runat="server" Width="10" />
                                  <ext:ComboBox ID="cbGmPagingBBUG" runat="server" Width="80">
                                    <Items>
                                      <ext:ListItem Text="5" />
                                      <ext:ListItem Text="10" />
                                      <ext:ListItem Text="20" />
                                      <ext:ListItem Text="50" />
                                      <ext:ListItem Text="100" />
                                    </Items>
                                    <SelectedItem Value="20" />
                                    <Listeners>
                                      <Select Handler="#{gmPagingBBUG}.pageSize = parseInt(this.getValue()); #{gmPagingBBUG}.doLoad();" />
                                    </Listeners>
                                  </ext:ComboBox>
                                </Items>
                              </ext:PagingToolbar>
                            </BottomBar>
                          </ext:GridPanel>
                        </Items>
                      </ext:Panel>
                    </North>
                    <Center>
                      <ext:Panel ID="pnlUsrGrpDtl" runat="server" Layout="Fit">
                        <TopBar>
                          <ext:Toolbar runat="server">
                            <Items>
                              <ext:Hidden ID="hfUserID" runat="server" />
                              <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                                LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                                <Items>
                                  <ext:ComboBox ID="cbGroup" runat="server" FieldLabel="Grup" Width="250" ItemSelector="tr.search-item"
                                    PageSize="10" ListWidth="400" DisplayField="v_group_desc" ValueField="c_group"
                                    EmptyText="Pilihan...">
                                    <Store>
                                      <ext:Store runat="server">
                                        <Proxy>
                                          <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                            CallbackParam="soaScmsCallback" />
                                        </Proxy>
                                        <BaseParams>
                                          <ext:Parameter Name="start" Value="={0}" />
                                          <ext:Parameter Name="limit" Value="={10}" />
                                          <ext:Parameter Name="model" Value="102002" />
                                          <ext:Parameter Name="parameters" Value="[['c_nip', #{hfUserID}.getValue(), 'System.String'],
                                          ['l_aktif = @0', 'true', 'System.Boolean'],
                                          ['@contains.c_group.Contains(@0) || @contains.v_group_desc.Contains(@0)', paramTextGetter(#{cbGroup}), '']]" Mode="Raw" />
                                          <ext:Parameter Name="sort" Value="c_group" />
                                          <ext:Parameter Name="dir" Value="ASC" />
                                        </BaseParams>
                                        <Reader>
                                          <ext:JsonReader IDProperty="c_group" Root="d.records" SuccessProperty="d.success"
                                            TotalProperty="d.totalRows">
                                            <Fields>
                                              <ext:RecordField Name="c_group" />
                                              <ext:RecordField Name="v_group_desc" />
                                            </Fields>
                                          </ext:JsonReader>
                                        </Reader>
                                      </ext:Store>
                                    </Store>
                                    <Template runat="server">
                                      <Html>
                                      <table cellpading="0" cellspacing="0" style="width: 400px">
                                      <tr>
                                      <td class="body-panel">Kode</td><td class="body-panel">Grup</td>
                                      </tr>
                                      <tpl for="."><tr class="search-item">
                                        <td>{c_group}</td><td>{v_group_desc}</td>
                                      </tr></tpl>
                                      </table>
                                      </Html>
                                    </Template>
                                    <Triggers>
                                      <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                                    </Triggers>
                                    <Listeners>
                                      <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                                      <Change Handler="clearRelatedComboRecursive(true, #{cbSpcDtl});" />
                                    </Listeners>
                                  </ext:ComboBox>
                                  <ext:Button ID="btnAddGroup" runat="server" FieldLabel="&nbsp;" LabelSeparator=" "
                                    ToolTip="Add" Icon="Add">
                                    <Listeners>
                                      <Click Handler="insertNewDataGrp(#{cbGroup}, #{gridUserDetail});" />
                                    </Listeners>
                                  </ext:Button>
                                </Items>
                              </ext:FormPanel>
                            </Items>
                          </ext:Toolbar>
                        </TopBar>
                        <Items>
                          <ext:GridPanel ID="gridUserDetail" runat="server">
                            <SelectionModel>
                              <ext:RowSelectionModel SingleSelect="true" />
                            </SelectionModel>
                            <Store>
                              <ext:Store runat="server">
                                <Proxy>
                                  <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                    CallbackParam="soaScmsCallback" />
                                </Proxy>
                                <BaseParams>
                                  <ext:Parameter Name="start" Value="0" />
                                  <ext:Parameter Name="limit" Value="-1" />
                                  <ext:Parameter Name="allQuery" Value="true" />
                                  <ext:Parameter Name="model" Value="102001" />
                                  <ext:Parameter Name="sort" Value="c_group" />
                                  <ext:Parameter Name="dir" Value="ASC" />
                                  <ext:Parameter Name="parameters" Value="[['c_nip = @0', #{hfUserID}.getValue(), 'System.String']]"
                                    Mode="Raw" />
                                </BaseParams>
                                <Reader>
                                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                    <Fields>
                                      <ext:RecordField Name="l_new" Type="Boolean" />
                                      <ext:RecordField Name="c_group" />
                                      <ext:RecordField Name="v_group_desc" />
                                      <ext:RecordField Name="l_delete" Type="Boolean" />
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
                                  </Commands>
                                </ext:CommandColumn>
                                <ext:Column DataIndex="c_group" Header="Grup" Width="50" />
                                <ext:Column DataIndex="v_group_desc" Header="Deskripsi" Width="250" />
                                <ext:CheckColumn DataIndex="l_delete" Header="Hapus" Width="50" Resizable="false"
                                  Fixed="true" Editable="false" />
                              </Columns>
                            </ColumnModel>
                            <Listeners>
                              <Command Handler="if(command == 'Delete') { deleteUserDetailGridData(this, record); }" />
                            </Listeners>
                          </ext:GridPanel>
                        </Items>
                      </ext:Panel>
                    </Center>
                  </ext:BorderLayout>
                </Items>
              </ext:Panel>
              <ext:Panel ID="pnlByGroup" runat="server" Title="User To Groups">
                <Items>
                  <ext:BorderLayout runat="server">
                    <North Split="true">
                      <ext:Panel runat="server" MinHeight="200" Layout="Fit">
                        <Items>
                          <ext:GridPanel ID="gridGroupMain" runat="server">
                            <LoadMask ShowMask="true" />
                            <SelectionModel>
                              <ext:RowSelectionModel SingleSelect="true">
                                <Listeners>
                                  <RowSelect Handler="onGridRowUserClick(record, 'c_group', #{gridGroupDetail}, #{cbUser}, #{hfGroupID});"
                                    Delay="10" />
                                </Listeners>
                              </ext:RowSelectionModel>
                            </SelectionModel>
                            <Store>
                              <ext:Store runat="server">
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
                                  <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBBGG}.getValue())" Mode="Raw" />
                                  <ext:Parameter Name="model" Value="5001" />
                                  <ext:Parameter Name="parameters" Value="[['@contains.c_group.Contains(@0)', paramValueGetter(#{txGrupFltrGG}), ''],
                                                  ['@contains.v_group_desc.Contains(@0)', paramValueGetter(#{txDescFltrGG}), '']]" Mode="Raw" />
                                </BaseParams>
                                <Reader>
                                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                                    IDProperty="c_group">
                                    <Fields>
                                      <ext:RecordField Name="c_group" />
                                      <ext:RecordField Name="v_group_desc" />
                                      <ext:RecordField Name="l_aktif" Type="Boolean" />
                                    </Fields>
                                  </ext:JsonReader>
                                </Reader>
                                <SortInfo Field="c_group" Direction="ASC" />
                              </ext:Store>
                            </Store>
                            <ColumnModel>
                              <Columns>
                                <ext:Column DataIndex="c_group" Header="Grup" Width="150" />
                                <ext:Column DataIndex="v_group_desc" Header="Deskripsi" Width="350" />
                                <ext:CheckColumn DataIndex="l_aktif" Header="Aktif" Width="50" Editable="false" />
                              </Columns>
                            </ColumnModel>
                            <View>
                              <ext:GridView runat="server" StandardHeaderRow="true">
                                <HeaderRows>
                                  <ext:HeaderRow>
                                    <Columns>
                                      <ext:HeaderColumn>
                                        <Component>
                                          <ext:TextField ID="txGrupFltrGG" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                            <Listeners>
                                              <KeyUp Handler="reloadFilterGrid(#{gridGroupMain})" Buffer="700" Delay="700" />
                                            </Listeners>
                                          </ext:TextField>
                                        </Component>
                                      </ext:HeaderColumn>
                                      <ext:HeaderColumn>
                                        <Component>
                                          <ext:TextField ID="txDescFltrGG" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                            <Listeners>
                                              <KeyUp Handler="reloadFilterGrid(#{gridGroupMain})" Buffer="700" Delay="700" />
                                            </Listeners>
                                          </ext:TextField>
                                        </Component>
                                      </ext:HeaderColumn>
                                      <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                                        <Component>
                                          <ext:Button ID="ClearFilterButtonGG" runat="server" Icon="Cancel" ToolTip="Clear filter">
                                            <Listeners>
                                              <Click Handler="clearFilterGridHeader(#{gridGroupMain}, #{txGrupFltrGG}, #{txDescFltrGG});reloadFilterGrid(#{gridGroupMain});"
                                                Buffer="300" Delay="300" />
                                            </Listeners>
                                          </ext:Button>
                                        </Component>
                                      </ext:HeaderColumn>
                                    </Columns>
                                  </ext:HeaderRow>
                                </HeaderRows>
                              </ext:GridView>
                            </View>
                            <BottomBar>
                              <ext:PagingToolbar runat="server" PageSize="20">
                                <Items>
                                  <ext:Label runat="server" Text="Page size:" />
                                  <ext:ToolbarSpacer runat="server" Width="10" />
                                  <ext:ComboBox ID="cbGmPagingBBGG" runat="server" Width="80">
                                    <Items>
                                      <ext:ListItem Text="5" />
                                      <ext:ListItem Text="10" />
                                      <ext:ListItem Text="20" />
                                      <ext:ListItem Text="50" />
                                      <ext:ListItem Text="100" />
                                    </Items>
                                    <SelectedItem Value="20" />
                                    <Listeners>
                                      <Select Handler="#{cbGmPagingBBGG}.pageSize = parseInt(this.getValue()); #{gmPagingBB}.doLoad();" />
                                    </Listeners>
                                  </ext:ComboBox>
                                </Items>
                              </ext:PagingToolbar>
                            </BottomBar>
                          </ext:GridPanel>
                        </Items>
                      </ext:Panel>
                    </North>
                    <Center>
                      <ext:Panel ID="pnlGrpUsrDtl" runat="server" Layout="Fit">
                        <TopBar>
                          <ext:Toolbar runat="server">
                            <Items>
                              <ext:Hidden ID="hfGroupID" runat="server" />
                              <ext:FormPanel ID="frmpnlGrpDetailEntry" runat="server" Frame="True" Layout="Table"
                                LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                                <Items>
                                  <ext:ComboBox ID="cbUser" runat="server" FieldLabel="Pengguna" Width="250" ItemSelector="tr.search-item"
                                    PageSize="10" ListWidth="400" DisplayField="v_username" ValueField="c_nip" EmptyText="Pilihan...">
                                    <Store>
                                      <ext:Store runat="server">
                                        <Proxy>
                                          <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                            CallbackParam="soaScmsCallback" />
                                        </Proxy>
                                        <BaseParams>
                                          <ext:Parameter Name="start" Value="={0}" />
                                          <ext:Parameter Name="limit" Value="={10}" />
                                          <ext:Parameter Name="model" Value="102102" />
                                          <ext:Parameter Name="parameters" Value="[['c_group', #{hfGroupID}.getValue(), 'System.String'],
                                          ['l_aktif = @0', 'true', 'System.Boolean'],
                                          ['@contains.c_nip.Contains(@0) || @contains.v_username.Contains(@0)', paramTextGetter(#{cbUser}), '']]" Mode="Raw" />
                                          <ext:Parameter Name="sort" Value="c_nip" />
                                          <ext:Parameter Name="dir" Value="ASC" />
                                        </BaseParams>
                                        <Reader>
                                          <ext:JsonReader IDProperty="c_nip" Root="d.records" SuccessProperty="d.success" TotalProperty="d.totalRows">
                                            <Fields>
                                              <ext:RecordField Name="c_nip" />
                                              <ext:RecordField Name="v_username" />
                                            </Fields>
                                          </ext:JsonReader>
                                        </Reader>
                                      </ext:Store>
                                    </Store>
                                    <Template runat="server">
                                      <Html>
                                      <table cellpading="0" cellspacing="0" style="width: 400px">
                                      <tr>
                                      <td class="body-panel">N I P</td><td class="body-panel">Nama</td>
                                      </tr>
                                      <tpl for="."><tr class="search-item">
                                        <td>{c_nip}</td><td>{v_username}</td>
                                      </tr></tpl>
                                      </table>
                                      </Html>
                                    </Template>
                                    <Triggers>
                                      <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                                    </Triggers>
                                    <Listeners>
                                      <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                                      <Change Handler="clearRelatedComboRecursive(true, #{cbSpcDtl});" />
                                    </Listeners>
                                  </ext:ComboBox>
                                  <ext:Button ID="btnAddUser" runat="server" FieldLabel="&nbsp;" LabelSeparator=" "
                                    ToolTip="Add" Icon="Add">
                                    <Listeners>
                                      <Click Handler="insertNewDataUsr(#{cbUser}, #{gridGroupDetail});" />
                                    </Listeners>
                                  </ext:Button>
                                </Items>
                              </ext:FormPanel>
                            </Items>
                          </ext:Toolbar>
                        </TopBar>
                        <Items>
                          <ext:GridPanel ID="gridGroupDetail" runat="server">
                            <SelectionModel>
                              <ext:RowSelectionModel SingleSelect="true" />
                            </SelectionModel>
                            <Store>
                              <ext:Store runat="server">
                                <Proxy>
                                  <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                    CallbackParam="soaScmsCallback" />
                                </Proxy>
                                <BaseParams>
                                  <ext:Parameter Name="start" Value="0" />
                                  <ext:Parameter Name="limit" Value="-1" />
                                  <ext:Parameter Name="allQuery" Value="true" />
                                  <ext:Parameter Name="model" Value="102101" />
                                  <ext:Parameter Name="sort" Value="c_nip" />
                                  <ext:Parameter Name="dir" Value="ASC" />
                                  <ext:Parameter Name="parameters" Value="[['c_group', #{hfGroupID}.getValue(), 'System.String']]"
                                    Mode="Raw" />
                                </BaseParams>
                                <Reader>
                                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                    <Fields>
                                      <ext:RecordField Name="l_new" Type="Boolean" />
                                      <ext:RecordField Name="c_nip" />
                                      <ext:RecordField Name="v_username" />
                                      <ext:RecordField Name="l_delete" Type="Boolean" />
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
                                  </Commands>
                                </ext:CommandColumn>
                                <ext:Column DataIndex="c_nip" Header="N I P" Width="50" />
                                <ext:Column DataIndex="v_username" Header="Nama" Width="250" />
                                <ext:CheckColumn DataIndex="l_delete" Header="Hapus" Width="50" Resizable="false"
                                  Fixed="true" Editable="false" />
                              </Columns>
                            </ColumnModel>
                            <Listeners>
                              <Command Handler="if(command == 'Delete') { deleteUserDetailGridData(this, record); }" />
                            </Listeners>
                          </ext:GridPanel>
                        </Items>
                      </ext:Panel>
                    </Center>
                  </ext:BorderLayout>
                </Items>
              </ext:Panel>
            </Items>
            <Buttons>
              <ext:Button runat="server" Icon="Disk" Text="Simpan">
                <DirectEvents>
                  <Click OnEvent="saveGridEntry">
                    <Confirmation ConfirmRequest="true" Message="Apa anda ingin menyimpan data ini ?"
                      Title="Simpan" />
                    <EventMask ShowMask="true" />
                    <ExtraParams>
                      <ext:Parameter Name="ActiveID" Value="#{tabUserGroup}.getActiveTab().getId()" Mode="Raw" />
                      <ext:Parameter Name="GridValues" Value="getGridValueByTab(#{tabUserGroup}, #{pnlByUser}, #{gridUserDetail}, #{gridGroupDetail})"
                        Mode="Raw" />
                    </ExtraParams>
                  </Click>
                </DirectEvents>
              </ext:Button>
            </Buttons>
          </ext:TabPanel>
        </Items>
      </ext:Panel>
    </Items>
  </ext:Viewport>
</asp:Content>
