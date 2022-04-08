<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MasterItemVia.aspx.cs" Inherits="master_item_MasterItemVia"
  MasterPageFile="~/Master.master" %>

         
<%@ Register Src="MasterItemViaCtrl.ascx" TagName="MasterItemViaCtrl" TagPrefix="uc" %>
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

                      Ext.net.DirectMethods.DeleteMethod(rec.get('idx'), txt);
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
              <ext:Button ID="Button1" runat="server" Text="Segarkan" Icon="ArrowRefresh">
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
                <Listeners><%--
                  <RowSelect Handler="onMICRowSelected(record, #{gridDetail}, #{hfTipeID}, #{frmpnlDetailEntry});" />
                  <RowDeselect Handler="onMICRowDeselected(#{gridDetail}, #{hfTipeID}, #{frmpnlDetailEntry});" />--%>
                </Listeners>
              </ext:RowSelectionModel>
            </SelectionModel>
            <%--<Store><ext:Store runat="server"></ext:Store></Store>--%>
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
                  <ext:Parameter Name="model" Value="0190" />
                  <ext:Parameter Name="parameters" Value="[
                    ['c_cusno', paramValueGetter(#{txCusnoFltr}) + '%', ''],
                    ['v_cunam', paramValueGetter(#{cbCustomerFltr}) + '%', ''],
                    ['c_iteno', paramValueGetter(#{txItenoFltr}) + '%', ''],
                    ['v_itnam', paramValueGetter(#{txItnamFltr}) + '%', ''],
                    ['v_nama', paramValueGetter(#{txNamaSupFltr}) + '%', ''],
                    ['c_type', paramValueGetter(#{cbViaFltr}) + '%', '']]" Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader IDProperty="idx" TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                    <Fields>
                      <ext:RecordField Name="idx" Type="Int" /> 
                      <ext:RecordField Name="c_cusno" />
                      <ext:RecordField Name="v_cunam" />
                      <ext:RecordField Name="c_iteno" />
                      <ext:RecordField Name="v_itnam" />
                      <ext:RecordField Name="v_nama" />
                      <ext:RecordField Name="v_ket" />
                      
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="idx" Direction="DESC" />
              </ext:Store>                                                                                                          
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="50" Resizable="false">
                  <Commands>
                    <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />
                  </Commands>
                </ext:CommandColumn>
                <ext:Column ColumnID="idx" DataIndex="idx" Header="ID Via" Width="80" hidden="false"/>
                <ext:Column ColumnID="c_cusno" DataIndex="c_cusno" Header="Kode Cabang" Width="80" hidden ="true"/>
                <ext:Column ColumnID="v_cunam" DataIndex="v_cunam" Header="Nama Cabang" Width="200" />
                <ext:Column ColumnID="c_iteno" DataIndex="c_iteno" Header="Kode Barang" Width="80" />
                <ext:Column ColumnID="v_itnam" DataIndex="v_itnam" Header="Nama Barang" Width="200" />
                <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Supplier" Width="200" />
                <%--<ext:Column ColumnID="c_via" DataIndex="c_via" Header="Kode Via" Width="80" Hidden="true" />--%>
                <ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="Via" Width="80" />
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{txCusnoFltr}, #{cbCustomerFltr}, #{txItenoFltr}, #{txItnamFltr}, #{txNamaSupFltr}, #{cbViaFltr});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      
                      <%--<ext:HeaderColumn >
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
                      </ext:HeaderColumn>--%>
                      <ext:HeaderColumn>
                        <%--<Component>
                          <ext:TextField ID="txIDVia" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>--%>
                      </ext:HeaderColumn>
                       <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txCusnoFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                      <Component>
                      <ext:ComboBox ID="cbCustomerFltr" runat="server" DisplayField="v_desc" ValueField="v_desc"
                      Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
                       AllowBlank="true">
                       <Store>
                         <ext:Store ID="Store2" runat="server" AutoLoad="false">
                       <Proxy>
                      <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                        CallbackParam="soaScmsCallback" />
                    </Proxy>
                    <BaseParams>
                      <ext:Parameter Name="start" Value="={0}" />
                      <ext:Parameter Name="limit" Value="={10}" />
                      <ext:Parameter Name="model" Value="110001" />
                      <ext:Parameter Name="parameters" Value="[['v_kode != @0', #{cbGudang}.getValue().trim(), 'System.String'],
                      ['@contains.v_kode.Contains(@0) || @contains.v_desc.Contains(@0)', paramTextGetter(#{cbCustomerFltr}), '']]"
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
                <Template ID="Template2" runat="server">
                  <Html>
                  <table cellpading="0" cellspacing="0" style="width: 400px">
                    <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td>
                    </tr>
                    <tpl for="."><tr class="search-item">
                      <td>{v_kode}</td><td>{v_desc}</td>
                      </td>
                    </tr></tpl>
                    </table>
                  </Html>
                </Template>
                 <Listeners>
                 <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                 </Listeners>
                <Triggers>
                <ext:FieldTrigger Icon="Search" Qtip="Reload" />
              </Triggers>
              <Listeners>
                  <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
              </Listeners>
              </ext:ComboBox>
                          </Component>
                       </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txItenoFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txItnamFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txNamaSupFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                       <ext:HeaderColumn >
                        <Component>
                          <ext:ComboBox ID="cbViaFltr" runat="server" DisplayField="v_ket" ValueField="c_type"
                          Width="250" AllowBlank="true" TypeAhead="false" ForceSelection="false">
                              <CustomConfig>
                                <ext:ConfigItem Name="allowBlank" Value="true" />
                              </CustomConfig>
                              <Store>
                              <ext:Store ID="Store1" runat="server" RemotePaging="false">
                              <Proxy>
                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                  CallbackParam="soaScmsCallback" />
                              </Proxy>
                              <BaseParams>
                                <ext:Parameter Name="allQuery" Value="true" />
                                <ext:Parameter Name="model" Value="2001" />
                                <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                                          ['c_notrans = @0', '02', ''],
                                ['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbViaFltr}), '']]" Mode="Raw" />
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
                          <Listeners>
                            <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                          </Listeners>
                        </ext:ComboBox>
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
 <uc:MasterItemViaCtrl ID="MasterItemViaCtrl" runat="server" />
</asp:Content>
