<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="Groups.aspx.cs" Inherits="management_Groups" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
    var validateEntry = function(f) {
      if (Ext.isEmpty(f)) {
        ShowWarning('Form objek tidak terdefinisi.');
        return false;
      }

      if (!f.isValid()) {
        ShowWarning('Data tidak akurat, mohon di perbaiki.');
        return false;
      }
    }
    var nodeClick = function(node, grid) {
      if (Ext.isEmpty(node)) {
        return;
      }

      if (!node.isLeaf()) {
        if (!Ext.isEmpty(grid)) {
          var store = grid.getStore();
          if (!Ext.isEmpty(store)) {
            store.removeAll();
            store.commitChanges();
          }
        }
        return;
      }

      var nodeId = node.attributes['nodeID'];
      var subNode = node.attributes['subNode'];

      Ext.net.DirectMethods.NodeClick(
        (Ext.isEmpty(node.id) ? '' : node.id),
        (Ext.isEmpty(nodeId) ? '' : nodeId),
        (Ext.isEmpty(subNode) ? '' : subNode),
        {
          success: function(result) {
            //ShowInformasi('Pemanggilan sukses.');
            ;
          },
          failure: function(result) {
            ShowError('Terdapat kesalahan dalam parsing javascript.');
          },
          eventMask: {
            showMask: true,
            minDelay: 500
          }
        });
    }
    var verifyBeforeApply = function(tv, grid) {
      if (Ext.isEmpty(tv) || Ext.isEmpty(grid)) {
        return false;
      }

      var selNode = tv.getSelectionModel().getSelectedNode();

      if (Ext.isEmpty(selNode)) {
        return false;
      }

      if (!selNode.isLeaf) {
        return false;
      }

      var store = grid.getStore();

      if (Ext.isEmpty(store)) {
        return false;
      }

      if (store.getCount() < 1) {
        return false;
      }

      var subNode = selNode.attributes['subNode'];
      var nodeId = selNode.attributes['nodeID'];
    }
    var prepareCommandFishing = function(e) {
      var avail = false;

      switch (e.field) {
        case "isView":
          avail = e.record.get('isOkView');
          break;
        case "isPrint":
          avail = e.record.get('isOkPrint');
          break;
        case "isAdd":
          avail = e.record.get('isOkAdd');
          break;
        case "isEdit":
          avail = e.record.get('isOkEdit');
          break;
        case "isDelete":
          avail = e.record.get('isOkDelete');
          break;
      }

      e.cancel = (!avail);
    }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Viewport runat="server" Layout="Fit">
    <Items>
      <ext:Panel runat="server" Layout="Fit">
        <Items>
          <ext:Panel runat="server" Layout="Fit">
            <TopBar>
              <ext:Toolbar runat="server">
                <Items>
                  <ext:Button ID="btnAddNew" runat="server" Text="Tambah" Icon="Add">
                    <DirectEvents>
                      <Click OnEvent="btnAddNew_OnClick">
                        <EventMask ShowMask="true" />
                      </Click>
                    </DirectEvents>
                  </ext:Button>
                  <ext:ToolbarSeparator />
                  <ext:Button runat="server" Text="Segarkan" Icon="ArrowRefresh">
                    <Listeners>
                      <Click Handler="refreshGrid(#{gridMain});" />
                    </Listeners>
                  </ext:Button>
                </Items>
              </ext:Toolbar>
            </TopBar>
            <Items>
              <ext:GridPanel ID="gridMain" runat="server">
                <LoadMask ShowMask="true" />
                <SelectionModel>
                  <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <DirectEvents>
                  <Command OnEvent="gridMainCommand">
                    <Confirmation ConfirmRequest="true" Title="Hapus ?" Message="Apa anda yakin ingin menghapus data ini ?"
                      BeforeConfirm="if(command != 'Delete') { return false; }" />
                    <EventMask ShowMask="true" />
                    <ExtraParams>
                      <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                      <ext:Parameter Name="Parameter" Value="c_group" />
                      <ext:Parameter Name="PrimaryID" Value="record.data.c_group" Mode="Raw" />
                    </ExtraParams>
                  </Command>
                </DirectEvents>
                <Store>
                  <ext:Store runat="server" SkinID="OriginalExtStore">
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
                      <ext:Parameter Name="model" Value="101001" />
                      <ext:Parameter Name="parameters" Value="[['c_group', paramValueGetter(#{txGrupFltr}) + '%', ''],
                                                  ['v_group_desc', paramValueGetter(#{txDescFltr}) + '%', ''],
                                                  ['totalList = @0', paramValueGetter(#{txJmlUserFltr}), 'System.Int32']]"
                        Mode="Raw" />
                    </BaseParams>
                    <Reader>
                      <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                        IDProperty="c_group">
                        <Fields>
                          <ext:RecordField Name="c_group" />
                          <ext:RecordField Name="v_group_desc" />
                          <ext:RecordField Name="l_aktif" Type="Boolean" />
                          <ext:RecordField Name="totalList" Type="Int" />
                        </Fields>
                      </ext:JsonReader>
                    </Reader>
                    <SortInfo Field="c_group" Direction="ASC" />
                  </ext:Store>
                </Store>
                <ColumnModel>
                  <Columns>
                    <ext:CommandColumn Width="50" Resizable="false">
                      <Commands>
                        <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                        <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />
                      </Commands>
                    </ext:CommandColumn>
                    <ext:Column DataIndex="c_group" Header="Grup" Width="150" />
                    <ext:Column DataIndex="v_group_desc" Header="Deskripsi" Width="350" />
                    <ext:NumberColumn DataIndex="totalList" Header="Pengguna" Width="75" Format="0.000/i" />
                    <ext:CheckColumn DataIndex="l_aktif" Header="Aktif" Width="50" />
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
                                  <Click Handler="clearFilterGridHeader(#{gridMain}, #{txGrupFltr}, #{txDescFltr}, #{txJmlUserFltr});reloadFilterGrid(#{gridMain});"
                                    Buffer="300" Delay="300" />
                                </Listeners>
                              </ext:Button>
                            </Component>
                          </ext:HeaderColumn>
                          <ext:HeaderColumn>
                            <Component>
                              <ext:TextField ID="txGrupFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                <Listeners>
                                  <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                                </Listeners>
                              </ext:TextField>
                            </Component>
                          </ext:HeaderColumn>
                          <ext:HeaderColumn>
                            <Component>
                              <ext:TextField ID="txDescFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                <Listeners>
                                  <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                                </Listeners>
                              </ext:TextField>
                            </Component>
                          </ext:HeaderColumn>
                          <ext:HeaderColumn>
                            <Component>
                              <ext:NumberField ID="txJmlUserFltr" runat="server" EnableKeyEvents="true" AllowBlank="true"
                                AllowDecimals="false" AllowNegative="false">
                                <Listeners>
                                  <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                                </Listeners>
                              </ext:NumberField>
                            </Component>
                          </ext:HeaderColumn>
                          <ext:HeaderColumn />
                        </Columns>
                      </ext:HeaderRow>
                    </HeaderRows>
                  </ext:GridView>
                </View>
                <BottomBar>
                  <ext:PagingToolbar runat="server" ID="gmPagingBB" PageSize="20">
                    <Items>
                      <ext:Label runat="server" Text="Page size:" />
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
          <ext:Window ID="winDetail" runat="server" Height="230" Width="350" Hidden="true"
            Resizable="false" MinHeight="230" MinWidth="350">
            <Items>
              <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Padding="5" Layout="Form">
                <Items>
                  <ext:Hidden ID="hfGroup" runat="server" />
                  <ext:TextField ID="txGrup" runat="server" FieldLabel="Grup" MaxLength="15" AllowBlank="false" />
                  <ext:TextArea ID="txDesc" runat="server" FieldLabel="Deskripsi Grup" MaxLength="50"
                    Width="200" Height="75" />
                  <ext:Checkbox ID="chkAktif" runat="server" FieldLabel="Aktif" />
                  <ext:Button ID="btnShowDetailAccess" runat="server" Icon="Script" FieldLabel="Hak akses">
                    <DirectEvents>
                      <Click OnEvent="btnShowDetailAccess_OnClick">
                        <EventMask ShowMask="true" />
                      </Click>
                    </DirectEvents>
                  </ext:Button>
                </Items>
              </ext:FormPanel>
            </Items>
            <Buttons>
              <ext:Button runat="server" Icon="Disk" Text="Simpan">
                <DirectEvents>
                  <Click OnEvent="btnSave_OnClick" Before="return validateEntry(#{frmpnlDetailEntry});">
                    <EventMask ShowMask="true" />
                    <ExtraParams>
                      <ext:Parameter Name="GROUP" Value="#{hfGroup}.getValue()" Mode="Raw" />
                    </ExtraParams>
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
          <ext:Window ID="winAkses" runat="server" Height="500" Width="700" Hidden="true" MinHeight="500"
            MinWidth="700" Maximizable="true" Layout="Fit" Title="Hak akses">
            <Items>
              <ext:Panel runat="server" Height="300" Width="300" Layout="Fit">
                <Items>
                  <ext:BorderLayout runat="server">
                    <West Split="true" MarginsSummary="5 0 5 5" MinWidth="150">
                      <ext:TreePanel ID="treeApp" runat="server" Width="250" Title="Akses Modul" Layout="Fit"
                        AutoScroll="true">
                        <Root>
                          <ext:AsyncTreeNode NodeID="0" Text="Root" />
                        </Root>
                        <Listeners>
                          <Click Handler="nodeClick(node, #{gridListAccess})" />
                        </Listeners>
                      </ext:TreePanel>
                    </West>
                    <Center MarginsSummary="5 5 5 0" MinWidth="250">
                      <ext:GridPanel ID="gridListAccess" runat="server" StripeRows="true" Title="Hak Akses"
                        Layout="Fit">
                        <Store>
                          <ext:Store runat="server">
                            <Reader>
                              <ext:ArrayReader>
                                <Fields>
                                  <ext:RecordField Name="Nama" />
                                  <ext:RecordField Name="isOkView" Type="Boolean" />
                                  <ext:RecordField Name="isOkPrint" Type="Boolean" />
                                  <ext:RecordField Name="isOkAdd" Type="Boolean" />
                                  <ext:RecordField Name="isOkEdit" Type="Boolean" />
                                  <ext:RecordField Name="isOkDelete" Type="Boolean" />
                                  <ext:RecordField Name="isView" Type="Boolean" />
                                  <ext:RecordField Name="isPrint" Type="Boolean" />
                                  <ext:RecordField Name="isAdd" Type="Boolean" />
                                  <ext:RecordField Name="isEdit" Type="Boolean" />
                                  <ext:RecordField Name="isDelete" Type="Boolean" />
                                </Fields>
                              </ext:ArrayReader>
                            </Reader>
                          </ext:Store>
                        </Store>
                        <ColumnModel runat="server">
                          <Columns>
                            <ext:Column DataIndex="Nama" ColumnID="Nama" Header="Page" Width="160" Sortable="true"
                              Editable="false" />
                            <ext:CheckColumn DataIndex="isView" ColumnID="IsView" Header="Lihat" Width="50" Editable="true" />
                            <ext:CheckColumn DataIndex="isPrint" ColumnID="IsPrint" Header="Cetak" Width="50"
                              Editable="true" />
                            <ext:CheckColumn DataIndex="isAdd" ColumnID="IsAdd" Header="Tambah" Width="50" Editable="true" />
                            <ext:CheckColumn DataIndex="isEdit" ColumnID="IsEdit" Header="Ubah" Width="50" Editable="true" />
                            <ext:CheckColumn DataIndex="isDelete" ColumnID="IsDelete" Header="Hapus" Width="50"
                              Editable="true" />
                            <%--<ext:Column DataIndex="isOkView" Hidden="true" Hideable="false" ColumnID="IsOkView" />
                            <ext:Column DataIndex="isOkPrint" Hidden="true" Hideable="false" ColumnID="IsOkPrint" />
                            <ext:Column DataIndex="isOkAdd" Hidden="true" Hideable="false" ColumnID="IsOkAdd" />
                            <ext:Column DataIndex="isOkEdit" Hidden="true" Hideable="false" ColumnID="IsOkEdit" />
                            <ext:Column DataIndex="isOkDelete" Hidden="true" Hideable="false" ColumnID="IsOkDelete" />--%>
                          </Columns>
                        </ColumnModel>
                        <SelectionModel>
                          <ext:RowSelectionModel runat="server" />
                        </SelectionModel>
                        <LoadMask ShowMask="true" />
                        <Listeners>
                          <BeforeEdit Fn="prepareCommandFishing" />
                        </Listeners>
                      </ext:GridPanel>
                    </Center>
                  </ext:BorderLayout>
                </Items>
                <BottomBar>
                  <ext:Toolbar runat="server">
                    <CustomConfig>
                      <ext:ConfigItem Name="buttonAlign" Value="right" Mode="Value" />
                    </CustomConfig>
                    <Items>
                      <ext:Button ID="btnApply" runat="server" Text="Apply" Icon="Accept">
                        <DirectEvents>
                          <Click OnEvent="btnApply_Click" Before="return verifyBeforeApply(#{treeApp}, #{gridListAccess});">
                            <EventMask ShowMask="true" />
                            <ExtraParams>
                              <ext:Parameter Name="DataAccess" Value="Ext.encode(#{gridListAccess}.getRowsValues())"
                                Mode="Raw" />
                              <ext:Parameter Name="DataNodeID" Value="#{treeApp}.getSelectionModel().getSelectedNode().id"
                                Mode="Raw" />
                              <ext:Parameter Name="DataNode" Value="#{treeApp}.getSelectionModel().getSelectedNode().attributes['nodeID']"
                                Mode="Raw" />
                            </ExtraParams>
                          </Click>
                        </DirectEvents>
                      </ext:Button>
                      <ext:Button ID="btnReset" runat="server" Text="Reset" Icon="Cross">
                        <Listeners>
                          <Click Handler="#{gridListAccess}.getStore().rejectChanges();" />
                        </Listeners>
                      </ext:Button>
                    </Items>
                  </ext:Toolbar>
                </BottomBar>
              </ext:Panel>
            </Items>
          </ext:Window>
        </Items>
      </ext:Panel>
    </Items>
  </ext:Viewport>
</asp:Content>
