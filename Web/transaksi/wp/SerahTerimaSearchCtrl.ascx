<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SerahTerimaSearchCtrl.ascx.cs"
    Inherits="transaksi_wp_SerahTerimaSearchCtrl" %>
<ext:Window runat="server" ID="winHeader" Title="Searching Serah Terima Transport"
    Height="480" Width="800" Hidden="true" MinHeight="480" MinWidth="500" Layout="Fit"
    Maximizable="true">
    <Content>
        <ext:Hidden ID="hfGdg" runat="server" />
        <ext:Hidden ID="hfType" runat="server" />
    </Content>
    <Items>
        <ext:Panel ID="Panel1" runat="server" Layout="Fit">
            <TopBar>
                <ext:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <ext:Button ID="Button1" runat="server" Text="Segarkan" Icon="ArrowRefresh">
                            <Listeners>
                                <Click Handler="refreshGrid(#{gridMain});" />
                            </Listeners>
                        </ext:Button>
                    </Items>
                </ext:Toolbar>
            </TopBar>
            <Items>
                <ext:GridPanel runat="server" ID="gridMain">
                    <LoadMask ShowMask="true" />
                    <%--<SelectionModel>
                        <ext:RowSelectionModel SingleSelect="true" />
                    </SelectionModel>--%>
                    <Store>
                        <ext:Store runat="server" RemotePaging="true" RemoteSort="true" SkinID="OriginalExtStore">
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
                                <ext:Parameter Name="model" Value="0311" />
                                <ext:Parameter Name="parameters" Value="[['c_gdg = @0', paramValueGetter(#{hfGdg}) , 'System.Char'],
                                                                    ['c_nodoc', paramValueGetter(#{txTransFltr}), ''],
                                                                    ['c_no', paramValueGetter(#{txNoFltr}), ''],
                                                                    ['c_plno', paramValueGetter(#{txPLFltr}), ''],
                                                                    ['c_picker = @0', paramValueGetter(#{cbPickerFltr}) , 'System.String'],
                                                                    ['c_checker = @0', paramValueGetter(#{cbCheckerFltr}) , 'System.String'],
                                                                    ['c_packer = @0', paramValueGetter(#{cbPackerFltr}) , 'System.String'],
                                                                    ['c_cusno = @0', paramValueGetter(#{cbCustomerFltr}) , 'System.String'],
                                                                    ['d_entry = @0', paramRawValueGetter(#{txDateFltr}) , 'System.DateTime']]"
                                    Mode="Raw" />
                            </BaseParams>
                            <Reader>
                                <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                    <Fields>
                                        <ext:RecordField Name="c_nodoc" />
                                        <ext:RecordField Name="d_entry" Type="Date" DateFormat="M$" />
                                        <ext:RecordField Name="c_no" />
                                        <ext:RecordField Name="c_plno" />
                                        <ext:RecordField Name="v_picker" />
                                        <ext:RecordField Name="v_checker" />
                                        <ext:RecordField Name="v_packer" />
                                        <ext:RecordField Name="v_cunam" />
                                    </Fields>
                                </ext:JsonReader>
                            </Reader>
                            <SortInfo Field="c_nodoc" Direction="ASC" />
                        </ext:Store>
                    </Store>
                    <ColumnModel>
                        <Columns>
                            <ext:CommandColumn Width="25" Resizable="false">
                                <Commands>
                                    <%--<ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />--%>
                                </Commands>
                            </ext:CommandColumn>
                            <ext:Column ColumnID="c_no" DataIndex="c_no" Header="DO/SJ" />
                            <ext:Column ColumnID="c_plno" DataIndex="c_plno" Header="PL" />
                            <ext:Column ColumnID="c_nodoc" DataIndex="c_nodoc" Header="Nomor ST" />
                            <ext:DateColumn ColumnID="d_entry" DataIndex="d_entry" Header="Tanggal" Format="dd-MM-yyyy" />
                            <ext:Column ColumnID="v_cunam" DataIndex="v_cunam" Header="Cabang" Width="150" />
                            <ext:Column ColumnID="v_picker" DataIndex="v_picker" Header="Picker" Width="150" />
                            <ext:Column ColumnID="v_checker" DataIndex="v_checker" Header="Checker" Width="150" />
                            <ext:Column ColumnID="v_packer" DataIndex="v_packer" Header="Packer" Width="150" />
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
                                                        <Click Handler="clearFilterGridHeader(#{GridMain}, #{txTransFltr}, #{txDateFltr}, #{txNoFltr}, #{txPLFltr}, #{cbCustomerFltr}, #{cbPickerFltr}, #{cbCheckerFltr}, #{cbPackerFltr});reloadFilterGrid(#{gridMain});"
                                                            Buffer="300" Delay="300" />
                                                    </Listeners>
                                                </ext:Button>
                                            </Component>
                                        </ext:HeaderColumn>
                                        <ext:HeaderColumn>
                                            <Component>
                                                <ext:TextField ID="txNoFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                    <Listeners>
                                                        <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                                                    </Listeners>
                                                </ext:TextField>
                                            </Component>
                                        </ext:HeaderColumn>
                                        <ext:HeaderColumn>
                                            <Component>
                                                <ext:TextField ID="txPLFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                    <Listeners>
                                                        <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                                                    </Listeners>
                                                </ext:TextField>
                                            </Component>
                                        </ext:HeaderColumn>
                                        <ext:HeaderColumn>
                                            <Component>
                                                <ext:TextField ID="txTransFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                    <Listeners>
                                                        <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                                                    </Listeners>
                                                </ext:TextField>
                                            </Component>
                                        </ext:HeaderColumn>
                                        <ext:HeaderColumn>
                                            <Component>
                                                <ext:DateField ID="txDateFltr" runat="server" Format="dd-MM-Y" EnableKeyEvents="true"
                                                    AllowBlank="true">
                                                    <Listeners>
                                                        <Change Handler="reloadFilterGrid(#{GridMain})" Buffer="300" Delay="300" />
                                                    </Listeners>
                                                </ext:DateField>
                                            </Component>
                                        </ext:HeaderColumn>
                                        <ext:HeaderColumn>
                                            <Component>
                                                <ext:ComboBox ID="cbCustomerFltr" runat="server" DisplayField="v_cunam" ValueField="c_cusno"
                                                    Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="300" MinChars="3"
                                                    AllowBlank="false" ForceSelection="false">
                                                    <CustomConfig>
                                                        <ext:ConfigItem Name="allowBlank" Value="false" />
                                                    </CustomConfig>
                                                    <Store>
                                                        <ext:Store ID="Store6" runat="server" SkinID="OriginalExtStore">
                                                            <Proxy>
                                                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                                    CallbackParam="soaScmsCallback" />
                                                            </Proxy>
                                                            <BaseParams>
                                                                <ext:Parameter Name="start" Value="={0}" />
                                                                <ext:Parameter Name="limit" Value="10" />
                                                                <ext:Parameter Name="model" Value="2011-a" />
                                                                <ext:Parameter Name="parameters" Value="[['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerFltr}), '']]"
                                                                    Mode="Raw" />
                                                                <ext:Parameter Name="sort" Value="v_cunam" />
                                                                <ext:Parameter Name="dir" Value="ASC" />
                                                            </BaseParams>
                                                            <Reader>
                                                                <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                                                                    TotalProperty="d.totalRows">
                                                                    <Fields>
                                                                        <ext:RecordField Name="c_cusno" />
                                                                        <ext:RecordField Name="v_cunam" />
                                                                        <ext:RecordField Name="c_cab" />
                                                                    </Fields>
                                                                </ext:JsonReader>
                                                            </Reader>
                                                        </ext:Store>
                                                    </Store>
                                                    <Template ID="Template5" runat="server">
                                                        <Html>
                                                        <table cellpading="0" cellspacing="1" style="width: 400px">
                                            <tr><td class="body-panel">Kode</td><td class="body-panel">Short</td><td class="body-panel">Cabang</td></tr>
                                            <tpl for="."><tr class="search-item">
                                                <td>{c_cusno}</td><td>{c_cab}</td><td>{v_cunam}</td>
                                            </tr></tpl>
                                            </table>
                                                        </Html>
                                                    </Template>
                                                    <Triggers>
                                                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                                                    </Triggers>
                                                    <Listeners>
                                                        <Change Handler="reloadFilterGrid(#{GridMain})" Buffer="300" Delay="300" />
                                                    </Listeners>
                                                </ext:ComboBox>
                                            </Component>
                                        </ext:HeaderColumn>
                                        <ext:HeaderColumn>
                                            <Component>
                                                <ext:ComboBox ID="cbPickerFltr" runat="server" DisplayField="v_nama" ValueField="c_nip"
                                                    Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
                                                    AllowBlank="true" ForceSelection="false">
                                                    <CustomConfig>
                                                        <ext:ConfigItem Name="allowBlank" Value="true" />
                                                    </CustomConfig>
                                                    <Store>
                                                        <ext:Store ID="Store3" runat="server">
                                                            <Proxy>
                                                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                                    CallbackParam="soaScmsCallback" />
                                                            </Proxy>
                                                            <BaseParams>
                                                                <ext:Parameter Name="start" Value="={0}" />
                                                                <ext:Parameter Name="limit" Value="={10}" />
                                                                <ext:Parameter Name="model" Value="2171" />
                                                                <ext:Parameter Name="parameters" Value="[['@contains.c_nip.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbPickerFltr}), '']]"
                                                                    Mode="Raw" />
                                                                <ext:Parameter Name="sort" Value="v_nama" />
                                                                <ext:Parameter Name="dir" Value="ASC" />
                                                            </BaseParams>
                                                            <Reader>
                                                                <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                                                                    TotalProperty="d.totalRows">
                                                                    <Fields>
                                                                        <ext:RecordField Name="c_nip" />
                                                                        <ext:RecordField Name="v_nama" />
                                                                    </Fields>
                                                                </ext:JsonReader>
                                                            </Reader>
                                                        </ext:Store>
                                                    </Store>
                                                    <Template ID="Template3" runat="server">
                                                        <Html>
                                                        <table cellpading="0" cellspacing="1" style="width: 400px">
                                                  <tr><td class="body-panel">Nip</td><td class="body-panel">Nama</td></tr>
                                                  <tpl for="."><tr class="search-item">
                                                      <td>{c_nip}</td><td>{v_nama}</td>
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
                                                <ext:ComboBox ID="cbCheckerFltr" runat="server" DisplayField="v_nama" ValueField="c_nip"
                                                    Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
                                                    AllowBlank="true" ForceSelection="false">
                                                    <CustomConfig>
                                                        <ext:ConfigItem Name="allowBlank" Value="true" />
                                                    </CustomConfig>
                                                    <Store>
                                                        <ext:Store ID="Store4" runat="server">
                                                            <Proxy>
                                                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                                    CallbackParam="soaScmsCallback" />
                                                            </Proxy>
                                                            <BaseParams>
                                                                <ext:Parameter Name="start" Value="={0}" />
                                                                <ext:Parameter Name="limit" Value="={10}" />
                                                                <ext:Parameter Name="model" Value="2171" />
                                                                <ext:Parameter Name="parameters" Value="[['@contains.c_nip.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbCheckerFltr}), '']]"
                                                                    Mode="Raw" />
                                                                <ext:Parameter Name="sort" Value="v_nama" />
                                                                <ext:Parameter Name="dir" Value="ASC" />
                                                            </BaseParams>
                                                            <Reader>
                                                                <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                                                                    TotalProperty="d.totalRows">
                                                                    <Fields>
                                                                        <ext:RecordField Name="c_nip" />
                                                                        <ext:RecordField Name="v_nama" />
                                                                    </Fields>
                                                                </ext:JsonReader>
                                                            </Reader>
                                                        </ext:Store>
                                                    </Store>
                                                    <Template ID="Template4" runat="server">
                                                        <Html>
                                                        <table cellpading="0" cellspacing="1" style="width: 400px">
                                                  <tr><td class="body-panel">Nip</td><td class="body-panel">Nama</td></tr>
                                                  <tpl for="."><tr class="search-item">
                                                      <td>{c_nip}</td><td>{v_nama}</td>
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
                                                <ext:ComboBox ID="cbPackerFltr" runat="server" DisplayField="v_nama" ValueField="c_nip"
                                                    Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
                                                    AllowBlank="true" ForceSelection="false">
                                                    <CustomConfig>
                                                        <ext:ConfigItem Name="allowBlank" Value="true" />
                                                    </CustomConfig>
                                                    <Store>
                                                        <ext:Store ID="Store2" runat="server">
                                                            <Proxy>
                                                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                                    CallbackParam="soaScmsCallback" />
                                                            </Proxy>
                                                            <BaseParams>
                                                                <ext:Parameter Name="start" Value="={0}" />
                                                                <ext:Parameter Name="limit" Value="={10}" />
                                                                <ext:Parameter Name="model" Value="2171" />
                                                                <ext:Parameter Name="parameters" Value="[['@contains.c_nip.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbPackerFltr}), '']]"
                                                                    Mode="Raw" />
                                                                <ext:Parameter Name="sort" Value="v_nama" />
                                                                <ext:Parameter Name="dir" Value="ASC" />
                                                            </BaseParams>
                                                            <Reader>
                                                                <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                                                                    TotalProperty="d.totalRows">
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
                                                        <table cellpading="0" cellspacing="1" style="width: 400px">
                                                  <tr><td class="body-panel">Nip</td><td class="body-panel">Nama</td></tr>
                                                  <tpl for="."><tr class="search-item">
                                                      <td>{c_nip}</td><td>{v_nama}</td>
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
</ext:Window>
