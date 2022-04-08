<%@ Page Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true" CodeFile="DOSend.aspx.cs"
    Inherits="transaksi_penjualan_DOSend" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

    <script type="text/javascript">
        var prepareCommandsParentDoPL = function(record, toolbar) {
            var accp = toolbar.items.get(0); // accept button

            var isSubmitDO = false;

            if (!Ext.isEmpty(record)) {
                isSubmitDO = record.get('l_sent');
            }

            if (isSubmitDO) {
                accp.setVisible(true);
            }
            else {
                accp.setVisible(false);
            }
        }

        var submitDODataToFJDoPL = function(rec) {
            if (Ext.isEmpty(rec)) { return; }

            ShowConfirm('Kirim ?', 'Apakah anda yakin ingin memproses nomor ini ?',
            function(btn) {
                if (btn == 'yes') {
                    Ext.net.DirectMethods.SubmitMethod(rec.get('c_dono'));
                }
            });
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
        <Items>
            <ext:Panel ID="Panel1" runat="server" Layout="Fit">
                <Content>
                    <ext:Hidden ID="hfDONo" runat="server" />
                    <ext:Hidden ID="hfGudang" runat="server" />
                    <ext:Hidden ID="hfGdgDesc" runat="server" />
                    <ext:Hidden ID="hfStoreID" runat="server" />
                </Content>
                <TopBar>
                    <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>
                            <ext:Button ID="Button3" runat="server" Text="Segarkan" Icon="ArrowRefresh">
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
                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" />
                        </SelectionModel>
                        <Store>
                            <ext:Store ID="storeGridDOSend" runat="server" SkinID="OriginalExtStore" RemotePaging="true"
                                RemoteSort="true">
                                <Proxy>
                                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                        CallbackParam="soaScmsCallback" />
                                </Proxy>
                                <AutoLoadParams>
                                    <ext:Parameter Name="start" Value="={0}" />
                                    <ext:Parameter Name="limit" Value="={20}" />
                                </AutoLoadParams>
                                <BaseParams>
                                    <ext:Parameter Name="start" Value="={0}" />
                                    <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB}.getValue())" Mode="Raw" />
                                    <ext:Parameter Name="model" Value="0007s" />
                                    <ext:Parameter Name="parameters" Value="[['c_dono', paramValueGetter(#{txNoFltr}) + '%', ''],                                            
                                            ['d_dodate = @0', paramRawValueGetter(#{txDODate}) , 'System.DateTime'],
                                            ['c_cusno = @0', paramValueGetter(#{cbCustomer}) , 'System.String']]"
                                        Mode="Raw" />
                                </BaseParams>
                                <Reader>
                                    <ext:JsonReader IDProperty="c_dono" Root="d.records" SuccessProperty="d.success"
                                        TotalProperty="d.totalRows">
                                        <Fields>
                                            <ext:RecordField Name="c_dono" />
                                            <ext:RecordField Name="v_gdgdesc" />
                                            <ext:RecordField Name="d_dodate" Type="Date" DateFormat="M$" />
                                            <ext:RecordField Name="v_cunam" />                                        
                                            <ext:RecordField Name="l_sent" Type="Boolean" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                                <SortInfo Field="c_dono" Direction="DESC" />
                            </ext:Store>
                        </Store>
                        <ColumnModel>
                            <Columns>
                                <ext:CommandColumn Width="25" Resizable="false">
                                </ext:CommandColumn>
                                <ext:Column ColumnID="c_dono" DataIndex="c_dono" Header="Nomor DO" Hideable="false" />
                                <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" />
                                <ext:DateColumn ColumnID="d_dodate" DataIndex="d_dodate" Header="Tanggal DO" Format="dd-MM-yyyy" />
                                <ext:Column ColumnID="v_cunam" DataIndex="v_cunam" Header="Cabang" Width="150" />                            
                                <ext:CommandColumn ColumnID="l_sent" DataIndex="l_sent" Header="Kirim" Width="50"
                                    ButtonAlign="Center">
                                    <Commands>
                                        <ext:GridCommand Icon="Accept" />
                                    </Commands>
                                    <PrepareToolbar Handler="prepareCommandsParentDoPL(record, toolbar);" />
                                </ext:CommandColumn>
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
                                                            <Click Handler="clearFilterGridHeader(#{GridMain}, #{txNoFltr},#{cbGudang3},#{txDODate},#{cbCustomer});reloadFilterGrid(#{gridMain});"
                                                                Buffer="300" Delay="300" />
                                                        </Listeners>
                                                    </ext:Button>
                                                </Component>
                                            </ext:HeaderColumn>
                                            <ext:HeaderColumn>
                                                <Component>
                                                    <ext:TextField ID="txNoFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                        <Listeners>
                                                            <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                                                        </Listeners>
                                                    </ext:TextField>
                                                </Component>
                                            </ext:HeaderColumn>
                                            <ext:HeaderColumn>
                                                <Component>
                                                    <ext:ComboBox ID="cbGudang3" runat="server" DisplayField="v_gdgdesc" ValueField="c_gdg"
                                                        Width="300" AllowBlank="true" ForceSelection="false" TypeAhead="false">
                                                        <CustomConfig>
                                                            <ext:ConfigItem Name="allowBlank" Value="true" />
                                                        </CustomConfig>
                                                        <Store>
                                                            <ext:Store ID="Store6" runat="server" RemotePaging="false">
                                                                <Proxy>
                                                                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                                        CallbackParam="soaScmsCallback" />
                                                                </Proxy>
                                                                <BaseParams>
                                                                    <ext:Parameter Name="allQuery" Value="true" />
                                                                    <ext:Parameter Name="model" Value="2031" />
                                                                    <ext:Parameter Name="parameters" Value="[['@contains.c_gdg.Contains(@0) || @contains.v_gdgdesc.Contains(@0)', paramTextGetter(#{cbGudang3}), '']]"
                                                                        Mode="Raw" />
                                                                    <ext:Parameter Name="sort" Value="c_gdg" />
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
                                                        <Listeners>
                                                            <Change Handler="reloadFilterGrid(#{GridPanel2})" Buffer="300" Delay="300" />
                                                        </Listeners>
                                                    </ext:ComboBox>
                                                </Component>
                                            </ext:HeaderColumn>
                                            <ext:HeaderColumn>
                                                <Component>
                                                    <ext:DateField ID="txDODate" runat="server" Format="dd-MM-yyyy" EnableKeyEvents="true"
                                                        AllowBlank="true">
                                                        <Listeners>
                                                            <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                                                        </Listeners>
                                                    </ext:DateField>
                                                </Component>
                                            </ext:HeaderColumn>
                                            <ext:HeaderColumn>
                                                <Component>
                                                    <ext:ComboBox ID="cbCustomer" runat="server" DisplayField="v_cunam" ValueField="c_cusno"
                                                                            Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
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
                                                                                        <ext:Parameter Name="model" Value="2011" />
                                                                                        <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                                                          ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomer}), '']]"
                                                          Mode="Raw" />
                                                        <ext:Parameter Name="sort" Value="v_cunam" />
                                                        <ext:Parameter Name="dir" Value="ASC" />
                                                      </BaseParams>
                                                      <Reader>
                                                        <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                                                          TotalProperty="d.totalRows">
                                                          <Fields>
                                                            <ext:RecordField Name="c_cusno" />
                                                            <ext:RecordField Name="c_cab" />
                                                            <ext:RecordField Name="v_cunam" />
                                                          </Fields>
                                                        </ext:JsonReader>
                                                      </Reader>
                                                    </ext:Store>
                                                  </Store>
                                                  <Template ID="Template1" runat="server">
                                                    <Html>
                                                    <table cellpading="0" cellspacing="1" style="width: 400px">
                                                      <tr><td class="body-panel">Kode</td><td class="body-panel">Short</td><td class="body-panel">Cabang</td></tr>
                                                      <tpl for="."><tr class="search-item">
                                                          <td>{c_cusno}</td><td>{c_cab}</td><td>{v_cunam}</td>
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
                                                    <ext:Button ID="Button4" runat="server" ToolTip="Kirim DO" Icon="Accept">
                                                        <DirectEvents>
                                                            <Click OnEvent="SubmitSelection">
                                                                <ExtraParams>
                                                                    <ext:Parameter Name="Values" Value="Ext.encode(#{gridMain}.getRowsValues({selectedOnly:true}))"
                                                                        Mode="Raw" />
                                                                    <ext:Parameter Name="BtnTipe" Value="1" Mode="Raw" />
                                                                    <ext:Parameter Name="katcol" Value="1" Mode="Raw" />
                                                                </ExtraParams>
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:Button>
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
</asp:Content>
