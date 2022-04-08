<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="MasterItemCategory.aspx.cs" Inherits="master_item_MasterItemCategory" %>
<%@ Register Src="MasterItemCategoryCtrl.ascx" TagName="MasterItemCategoryCtrl" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
    var voidCatDataFromStore = function(rec) {
      if (Ext.isEmpty(rec)) {
        return;
      }

      ShowConfirm('Hapus ?', 'Apakah anda yakin ingin menghapus data ini ?',
        function(btn) {
          if (btn == 'yes') {
            ShowAsk('Alasan ?', 'Masukkan alasan kenapa harus dibatalkan.',
              function(btnP, txt) {
                if (btnP == 'ok') {
                  if (txt.trim().length < 1) {
                    txt = 'Kesalahan pemakai.';
                  }

                  Ext.net.DirectMethods.DeleteMethod(rec.get('c_type'), rec.get('c_iteno'), txt);
                }
              });
          }
        });
    }
    
    var storeToDetailGrid = function(mainGrid, frm, grid, item) {
      if (mainGrid.getSelectionModel().getCount() < 1) {
        ShowWarning("Pilih terlebih dahulu kategori yang akan ditambah.");
        return;
      }
      else if (!frm.getForm().isValid()) {
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
          'v_nama_supl': suplName,
          'l_new': true
        }));

        item.reset();
      }
      else {
        ShowError('Data telah ada.');

        return false;
      }
    }

  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Hidden ID="hfTipeID" runat="server" />
  <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
    <Items>
      <ext:Panel ID="pnlMainControl" runat="server" Layout="Fit" Height="300">
        <TopBar>
          <ext:Toolbar ID="Toolbar1" runat="server">
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
            <Listeners>
              <Command Handler="if(command == 'Delete') { voidCatDataFromStore(record); }" />
            </Listeners>
            <LoadMask ShowMask="true" />
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true">
                <Listeners>
                  <%--<RowSelect Handler="onMICRowSelected(record, #{gridDetail}, #{hfTipeID}, #{frmpnlDetailEntry});" />
                  <RowDeselect Handler="onMICRowDeselected(#{gridDetail}, #{hfTipeID}, #{frmpnlDetailEntry});" />--%>
                </Listeners>
              </ext:RowSelectionModel>
            </SelectionModel>
            <Store>
              <ext:Store ID="storeGridMasterTrx" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                  <ext:Parameter Name="parameters" Value="[
                    ['(l_delete == null ? false : l_delete) = @0', 'false' , 'System.Boolean'],
                    ['v_itnam', paramValueGetter(#{txNamaFltr}) + '%', ''],
                    ['c_iteno', paramValueGetter(#{txKodeFltr}) + '%', ''],
                    ['c_type', paramValueGetter(#{cbTipeFltr}) + '%', '']]" Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    >
                    <Fields>
                      <ext:RecordField Name="c_type" />
                      <ext:RecordField Name="v_ket" />
                      <ext:RecordField Name="c_iteno" />
                      <ext:RecordField Name="v_itnam" />
                      <ext:RecordField Name="v_acronim" />
                      <ext:RecordField Name="v_nama_supl" />
                      <ext:RecordField Name="l_new" Type="Boolean" />
                      <ext:RecordField Name="l_void" Type="Boolean" />
                      <ext:RecordField Name="l_modified" Type="Boolean" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_iteno" Direction="ASC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="50" Resizable="false">
                  <Commands>
                    
                    <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />
                  </Commands>
                </ext:CommandColumn>
                <ext:Column ColumnID="c_type" DataIndex="c_type" Header="Kode" Width="50" />
                <ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="Deskripsi" Width="200" />
                <ext:Column ColumnID="c_iteno" DataIndex="c_iteno" Header="KOde Item" Width="200" />
                <ext:Column ColumnID="v_itnam" DataIndex="v_itnam" Header="Nama Item" Width="200" />
              </Columns>
            </ColumnModel>
            <View>
              <ext:GridView ID="GridView1" runat="server" StandardHeaderRow="true">
                <HeaderRows>
                  <ext:HeaderRow>
                    <Columns>
                      <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                        <Component>
                          <ext:Button ID="ClearFilterButton" runat="server" Icon="Cancel" ToolTip="Clear filter">
                            <Listeners>
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{txKodeFltr}, #{txNamaFltr}, #{cbTipeFltr});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn />
                      <ext:HeaderColumn >
                        <Component>
                          <ext:ComboBox ID="cbTipeFltr" runat="server" DisplayField="v_ket" ValueField="c_type"
                            Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="200" MinChars="3"
                            AllowBlank="true" ForceSelection="false">
                            <CustomConfig>
                              <ext:ConfigItem Name="allowBlank" Value="true" />
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
                                  <ext:Parameter Name="model" Value="2001" />
                                  <ext:Parameter Name="parameters" Value="[['c_notrans = @0', '001', 'System.String'],
                                    ['c_portal = @0', '9', 'System.Char']]" Mode="Raw" />
                                  <ext:Parameter Name="sort" Value="c_notrans" />
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
                            <Template ID="Template1" runat="server">
                              <Html>
                              <table cellpading="0" cellspacing="1" style="width: 200px">
                              <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                              <tpl for="."><tr class="search-item">
                              <td>{c_type}</td><td>{v_ket}</td>
                              </tr></tpl>
                              </table>
                              </Html>
                            </Template>
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:ComboBox>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txKodeFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txNamaFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                    </Columns>
                  </ext:HeaderRow>
                </HeaderRows>
              </ext:GridView>
            </View>
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
            <%--<Listeners>
              <Command Handler="onSaveMethod(command, record);" />
            </Listeners>--%>
          </ext:GridPanel>
        </Items>
      </ext:Panel>
    </Items>
  </ext:Viewport>
 <uc:MasterItemCategoryCtrl ID="MasterItemCategoryCtrl" runat="server" />
</asp:Content>
