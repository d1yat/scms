<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MasterDriver.aspx.cs" Inherits="master_driver_MasterDriver"
  MasterPageFile="~/Master.master" %>

<%@ Register Src="MasterDriverCtrl.ascx" TagName="MasterDriverCtrl" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
    <Items>
      <ext:Panel ID="pnlMainControl" runat="server" Layout="Fit">
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
            <LoadMask ShowMask="true" />
            <DirectEvents>
              <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_nip" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_nip" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="storeGridDriver" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                  <ext:Parameter Name="model" Value="0210" />
                  <ext:Parameter Name="parameters" Value="[
                    ['c_nip', paramValueGetter(#{txNipFltr}) + '%', ''],
                    ['v_nama', paramValueGetter(#{txNamaFltr}) + '%', ''],
                    ['c_nopol', paramValueGetter(#{txNopolFltr}) + '%', ''],
                    ['c_type = @0', paramValueGetter(#{cbTipeJenisFltr}) , 'System.String']
                    ]" Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_nip">
                    <Fields>
                      <ext:RecordField Name="c_nip" />
                      <ext:RecordField Name="v_nama" />
                      <ext:RecordField Name="c_nopol" />
                      <ext:RecordField Name="v_ket" />
                      <ext:RecordField Name="l_aktif" Type="Boolean" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_nip" Direction="ASC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="25" Resizable="false">
                  <Commands>
                    <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                    <%--<ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />--%>
                  </Commands>
                </ext:CommandColumn>
                <ext:Column ColumnID="c_nip" DataIndex="c_nip" Header="NIP" Width="75" />
                <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Nama" Width="300" />
                <ext:Column ColumnID="c_nopol" DataIndex="c_nopol" Header="No. Polisi" Width="80" />
                <ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="Tipe" Width="80" />
                <ext:CheckColumn ColumnID="l_aktif" DataIndex="l_aktif" Header="Aktif" Width="40"/>
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{txNipFltr}, #{txNamaFltr}, #{txNopolFltr}, #{cbTipeJenisFltr});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                       <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txNipFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
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
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txNopolFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                            <Component>
                              <ext:ComboBox ID="cbTipeJenisFltr" runat="server" DisplayField="v_ket" ValueField="c_type"
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
                                      <ext:Parameter Name="parameters" Value="[['c_notrans = @0', '004', 'System.String'],
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
                      <ext:HeaderColumn />
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
  <uc:MasterDriverCtrl runat="server" ID="MasterDriverCtrl1" />
</asp:Content>
