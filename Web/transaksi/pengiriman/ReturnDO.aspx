<%@ Page Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true" CodeFile="ReturnDO.aspx.cs"
  Inherits="transaksi_pengiriman_Return_DO" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
  
  //<Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridMain}, #{txTglTerima}, #{txTglBalik}, #{cbReturnDO};" />
  //var storeToDetailGrid = function(frm, grid, dono, no, berat, koli, vol, grid2, hit, totKoli, totBerat, totVol) {

      var prepareCommands = function(rec, toolbar) {
          var del = toolbar.items.get(0); // delete button
          var vd = toolbar.items.get(1); // void button

          var isNew = false;

          if (!Ext.isEmpty(rec)) {
              isNew = rec.get('l_new');
          }

          if (isNew) {
              del.setVisible(true);
              vd.setVisible(false);
          }
          else {
              del.setVisible(false);
              vd.setVisible(true);
          }
      }

      var storeToDetailGrid = function(frm, grid, gdg, tglterima, tglbalik, dono) {
          if (!frm.getForm().isValid()) {
              ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
              return;
          }

          if (Ext.isEmpty(grid) ||
          Ext.isEmpty(tglterima) ||
          Ext.isEmpty(tglbalik) ||
          Ext.isEmpty(dono)) {
              ShowWarning("Inputan ada yang kosong");
              return;
          }

          var valX = [dono.getValue()];
          var fieldX = ['c_dono'];
          var store = grid.getStore();

          var isDup = findDuplicateEntryGrid(store, fieldX, valX);
          if (!isDup) {

              var c_gdg = gdg.getValue();
              var gdgdesc = "Gudang " + c_gdg;

              var c_dono = dono.getValue();
              var d_terima = tglterima.getValue();
              var d_balik = tglbalik.getValue();


              if (c_dono.length != 10) {
                  ShowWarning("No. DO Salah.");
                  return false;
              }

              store.insert(0, new Ext.data.Record({
                  'v_gdgdesc': gdgdesc,
                  'c_dono': c_dono,
                  'd_terima': d_terima,
                  'd_balik': d_balik,
                  'l_new': true
              }));

              dono.reset();
          }
          else {
              ShowError("Data Telah Ada");
              return;
          }
      }

      var voidInsertedDOFromStore = function(store, rec) {
          if (rec.get('l_void')) {
              return false;
          }

          if (rec.get('l_new')) {
              deleteRecordOnStore(store, rec, function(stor) {
              });
          }
          else {
              voidInsertedDataFromStore(rec, function(txt) {
                  rec.set('l_modified', false);
                  rec.set('l_void', true);
                  rec.set('v_ket', txt);
              });
          }
      }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Hidden ID="hfGdg" runat="server" />
  <ext:Hidden ID="hfNip" runat="server" />
  <ext:Hidden ID="hfPrint" runat="server" />  
  <ext:Viewport runat="server" Layout="Fit">
    <Items>
      <ext:Panel runat="server" Layout="Fit">
        <TopBar>
          <ext:Toolbar ID="Toolbar1" runat="server">
            <Items>
                <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                    LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                    <Items>
                  <ext:Button ID="Button1" runat="server" Text="Segarkan" Icon="ArrowRefresh">
                    <Listeners>
                      <Click Handler="refreshGrid(#{gridMain});" />
                    </Listeners>
                  </ext:Button>
                  <ext:ToolbarSeparator />
                  <ext:DateField ID="txTglTerima" runat="server" FieldLabel="Tanggal Terima" AllowBlank="false"
                  Format="dd-MM-yyyy" />
                  <ext:DateField ID="txTglBalik" runat="server" FieldLabel="Tanggal Balik" AllowBlank="false"
                  Format="dd-MM-yyyy" />
                   <ext:ComboBox ID="cbReturnDO" runat="server" DisplayField="c_dono" ValueField="c_dono"
                   Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="200" MinChars="3"
                   FieldLabel="No. DO" AllowBlank="true" ForceSelection="false">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="true" />
                  </CustomConfig>
                      <Store>
                        <ext:Store ID="Store5" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="5013" />
                            <ext:Parameter Name="parameters" Value="[['@contains.c_dono.Contains(@0)', paramTextGetter(#{cbReturnDO}), ''],
                              ['gdg', #{hfGdg}.getValue() , 'System.Char']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_dono" />
                            <ext:Parameter Name="dir" Value="DESC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_dono" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_dono" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template4" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 150px">
                        <tr><td class="body-panel">No. DO</td></tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_dono}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <Select Handler="this.triggers[0].show();" />
                        <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                        <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); } else if(index == 1) { reloadData(this); }" />
                      </Listeners>
                      <DirectEvents>
                      <SpecialKey OnEvent="Submit_scan" Before="return e.getKey() == Ext.EventObject.ENTER;" Buffer="250" Delay="250">
                          <ExtraParams>
                            <ext:Parameter Name="DO" Value="#{cbReturnDO}.getText()" Mode="Raw" />
                            <ext:Parameter Name="Gudang" Value="#{hfGdg}.getValue()" Mode="Raw" />
                            <ext:Parameter Name="Grid" Value="saveStoreToServer(#{gridMain}.getStore())" Mode="Raw" />                                             
                         </ExtraParams>
                      </SpecialKey>
                      </DirectEvents>
                    </ext:ComboBox>
                    <%--<ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                    Icon="Add">
                    <Listeners>
                      <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridMain}, #{hfgdg}, #{txTglTerima}, #{txTglBalik}, #{cbReturnDO});" />
                    </Listeners>
                    </ext:Button>--%>
                  <ext:ToolbarSeparator />
                  <ext:Button ID="btnSimpan" runat="server" Icon="Disk" Text="Simpan">
                    <DirectEvents>
                      <Click OnEvent="SaveBtn_Click">
                        <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
                        <ExtraParams>
                          <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridMain}.getStore())"
                            Mode="Raw" />
                            <ext:Parameter Name="Gudang" Value="#{hfGdg}.getValue()" Mode="Raw" />
                        </ExtraParams>
                      </Click>
                    </DirectEvents>
                  </ext:Button>                                                       
                </Items>
              </ext:FormPanel>
            </Items>
          </ext:Toolbar>
        </TopBar>
        <Items>
          <ext:GridPanel runat="server" ID="gridMain">
            <LoadMask ShowMask="true" />
            <%--<DirectEvents>
              <Command OnEvent="GridMainCommand" Before="if(command != 'Select') { return false; }">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_dono" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_dono" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>--%>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="strGridMain" runat="server" RemotePaging="true" RemoteSort="true"
                SkinID="OriginalExtStore">
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
                  <ext:Parameter Name="model" Value="05003" />
                  <ext:Parameter Name="parameters" Value="[['gdg', paramValueGetter(#{hfGdg}), 'System.Char'],
                                                            ['c_dono', paramValueGetter(#{txDOFltr}) + '%', ''],
                                                            ['d_terima = @0', paramRawValueGetter(#{txDate1Fltr}) , 'System.DateTime'],
                                                            ['d_balik = @0', paramRawValueGetter(#{txDate2Fltr}) , 'System.DateTime']]" Mode="Raw" />
                    <%--['c_entry = @0', paramValueGetter(#{hfNip}) , 'System.String'],--%>
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_dono">
                	  <Fields>
                        <ext:RecordField Name="v_gdgdesc" />
                        <ext:RecordField Name="c_dono" />
                        <ext:RecordField Name="d_terima" Type="Date" DateFormat="M$" />
                        <ext:RecordField Name="d_balik" Type="Date" DateFormat="M$" />
                        <ext:RecordField Name="l_new" Type="Boolean" />
                        <ext:RecordField Name="l_void" Type="Boolean" />
                      </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_dono" Direction="DESC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="50" Resizable="false">
                  <Commands>
                    <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                    <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                  </Commands>
                  <PrepareToolbar Handler="prepareCommands(record, toolbar);" />
                </ext:CommandColumn>
                <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" Width="150" />
                <ext:Column ColumnID="c_dono" DataIndex="c_dono" Header="No. DO" Width="150" />
                <ext:DateColumn ColumnID="d_terima" DataIndex="d_terima" Header="Tanggal Terima" Format="dd-MM-yyyy" />
                <ext:DateColumn ColumnID="d_balik" DataIndex="d_balik" Header="Tanggal Balik" Format="dd-MM-yyyy" />
                <ext:CheckColumn ColumnID="l_new"  DataIndex="l_new" Header="Add" Width="50" />
                <ext:CheckColumn ColumnID="l_void"  DataIndex="l_void" Header="Batal" Width="50" />
              </Columns>
            </ColumnModel>
            <Listeners>
              <Command Handler="if ((command == 'Void') || (command == 'Delete')) { voidInsertedDOFromStore(this.getStore(), record); }" />
              <%--<AfterEdit Handler="afterEditDataConfirm(e, #{gridDetail}.getStore());" />--%>
            </Listeners>
            <View>
              <ext:GridView ID="GridView1" runat="server" StandardHeaderRow="true">
                <HeaderRows>
                  <ext:HeaderRow>
                    <Columns>
                      <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                        <Component>
                          <ext:Button ID="ClearFilterButton" runat="server" Icon="Cancel" ToolTip="Clear filter">
                            <Listeners>
                              <Click Handler="clearFilterGridHeader(#{GridMain}, #{txDOFltr}, #{txDate1Fltr},#{txDate^2Fltr});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn />
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txDOFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:DateField ID="txDate1Fltr" runat="server" Format="dd-MM-Y" EnableKeyEvents="true"
                            AllowBlank="true">
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{GridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:DateField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:DateField ID="txDate2Fltr" runat="server" Format="dd-MM-Y" EnableKeyEvents="true"
                            AllowBlank="true">
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{GridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:DateField>
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
          </ext:GridPanel>
        </Items>
      </ext:Panel>
    </Items>
  </ext:Viewport>
  <ext:Window ID="wndDown" runat="server" Hidden="true" />
</asp:Content>
