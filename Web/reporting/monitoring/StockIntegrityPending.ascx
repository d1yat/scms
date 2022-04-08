<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StockIntegrityPending.ascx.cs"
    Inherits="reporting_Monitoring_StockIntegrityPending" %>
<ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
    <Items>
        <ext:Hidden ID="hfGdg" runat="server" />
        <ext:Hidden ID="hidWndDown" runat="server" />
        <ext:Panel ID="Panel1" runat="server" Layout="Fit">
            <%--<TopBar>
                <ext:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <ext:Button ID="Button1" runat="server" Text="Segarkan" Icon="ArrowRefresh">
                            <Listeners>
                                <Click Handler="refreshGrid(#{gridMain});" />
                            </Listeners>
                        </ext:Button>
                    </Items>
                </ext:Toolbar>
            </TopBar>--%>
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
                                <ext:Parameter Name="model" Value="0360" />
                                <%--<ext:Parameter Name="parameters" Value="[['c_gdg = @0', paramValueGetter(#{hfGdg}) , 'System.Char']]"
                            Mode="Raw" />--%>
                            </BaseParams>
                            <Reader>
                                <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                    <Fields>
                                        <ext:RecordField Name="c_iteno" />
                                        <ext:RecordField Name="v_itnam" />
                                        <ext:RecordField Name="c_no" />
                                        <ext:RecordField Name="qtyPicker" Type="Float" />
                                        <ext:RecordField Name="qtyChecker" Type="Float" />
                                        <ext:RecordField Name="qtyPacker" Type="Float" />
                                        <ext:RecordField Name="qtyTransport" Type="Float" />
                                    </Fields>
                                </ext:JsonReader>
                            </Reader>
                            <SortInfo Field="c_iteno" Direction="ASC" />
                        </ext:Store>
                    </Store>
                    <ColumnModel>
                        <Columns>
                            <ext:CommandColumn Width="25" Resizable="false">
                                <Commands>
                                    <%--<ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />--%>
                                </Commands>
                            </ext:CommandColumn>
                            <ext:Column ColumnID="c_iteno" DataIndex="c_iteno" Header="Kode" />
                            <ext:Column ColumnID="v_itnam" DataIndex="v_itnam" Header="Nama Barang" />
                            <ext:Column ColumnID="c_no" DataIndex="c_no" Header="PL/SJ" />
                            <ext:NumberColumn DataIndex="qtyPicker" Header="qty Picker" Format="0.000,00/i" Width="75" />
                            <ext:NumberColumn DataIndex="qtyChecker" Header="qty Checker" Format="0.000,00/i" Width="75" />
                            <ext:NumberColumn DataIndex="qtyPacker" Header="qty Packer" Format="0.000,00/i" Width="75" />
                            <ext:NumberColumn DataIndex="qtyTransport" Header="qty Transport" Format="0.000,00/i" Width="75" />
                        </Columns>
                    </ColumnModel>
                    <View>
                        <%--<ext:GridView ID="GridView1" runat="server" StandardHeaderRow="true">
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
                                        <ext:HeaderColumn />
                                    </Columns>
                                </ext:HeaderRow>
                            </HeaderRows>
                        </ext:GridView>--%>
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
