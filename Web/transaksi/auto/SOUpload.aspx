<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SOUpload.aspx.cs" Inherits="transaksi_auto_SOUpload"
    MasterPageFile="~/Master.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

    <script language="javascript" type="text/javascript">
        var onSuccessProcess = function() {
            ;
        }
        var pagingSelected = function(pg, val) {
            pg.pageSize = parseInt(val);
            pg.doLoad();
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
        <Items>
            <ext:Panel ID="Panel1" runat="server" Layout="Fit" Border="false" AutoDoLayout="true">
                <Items>
                    <ext:BorderLayout ID="bllayout" runat="server">
                        <North MinHeight="150">
                            <ext:FormPanel ID="frmHeaders" runat="server" Height="110" Padding="5" ButtonAlign="Center"
                                Title="Upload SO Header">
                                <Items>
                                    <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Tgl.SO">
                                        <Items>
                                            <ext:DateField ID="txPeriode1" runat="server" Vtype="daterange" Format="dd-MM-yyyy"
                                                AllowBlank="true">
                                                <CustomConfig>
                                                    <ext:ConfigItem Name="endDateField" Value="#{txPeriode2}" Mode="Value" />
                                                </CustomConfig>
                                            </ext:DateField>
                                            <ext:Label ID="Label1" runat="server" Text="-" />
                                            <ext:DateField ID="txPeriode2" runat="server" Vtype="daterange" Format="dd-MM-yyyy"
                                                AllowBlank="true">
                                                <CustomConfig>
                                                    <ext:ConfigItem Name="startDateField" Value="#{txPeriode1}" Mode="Value" />
                                                </CustomConfig>
                                            </ext:DateField>
                                        </Items>
                                    </ext:CompositeField>
                                    <%--<ext:SelectBox ID="cbTahun" runat="server" FieldLabel="Tahun" Width="75" AllowBlank="false" />
                                    <ext:SelectBox ID="cbBulan" runat="server" FieldLabel="Bulan" Width="100" AllowBlank="false" />--%>
                                    <ext:FileUploadField ID="fuImportFB" runat="server" FieldLabel="Data file" EmptyText="Data file Excell..."
                                        ButtonText="Pilih..." Icon="Attach" Width="400" AllowBlank="false" />
                                    <ext:Checkbox ID="chkVerify" runat="server" FieldLabel="Verifikasi" Checked="true" />
                                </Items>
                                <Listeners>
                                    <ClientValidation Handler="#{btnProses}.setDisabled(!valid);" />
                                </Listeners>
                            </ext:FormPanel>
                        </North>
                        <Center MinHeight="150">
                            <ext:Panel ID="Panel2" runat="server" Title="Details Item" Layout="Fit">
                                <Items>
                                    <ext:TabPanel ID="TabPanel1" runat="server">
                                        <Items>
                                            <ext:Panel ID="pnlHitung1" runat="server" Title="Hitung 1" Layout="FitLayout">
                                                <TopBar>
                                                    <ext:Toolbar ID="Toolbar1" runat="server">
                                                        <Items>
                                                            <ext:FormPanel ID="frmpnlDetailHitung1" runat="server" Frame="True" Layout="Table"
                                                                LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                                                                <Items>
                                                                    <ext:Button ID="btnProses1" runat="server" Text="Proses Hitung 1" Icon="CogStart">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="Proses_OnClick" Before="return validasiProses(#{frmHeaders});" Success="onSuccessProcess();">
                                                                                <Confirmation BeforeConfirm="return validasiProses(#{frmHeaders});" ConfirmRequest="true"
                                                                                    Title="Simpan ?" Message="Anda yakin ingin memproses data ini." />
                                                                                <EventMask ShowMask="true" />
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="tipe" Value="1" Mode="Raw" />
                                                                                </ExtraParams>
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                    <ext:Button ID="btnCompare1" runat="server" Text="Calculate Compare stok Komputer vs Hitung 1"
                                                                        Icon="CogStart">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="Compare_OnClick" Success="onSuccessProcess();">
                                                                                <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin compare data dengan stok komputer." />
                                                                                <EventMask ShowMask="true" />
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="tipe" Value="1" Mode="Raw" />
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
                                                    <ext:GridPanel ID="gridDetailHitung1" runat="server">
                                                        <LoadMask ShowMask="true" />
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <Store>
                                                            <ext:Store ID="Store1" runat="server" SkinID="OriginalExtStore" RemoteSort="false"
                                                                RemotePaging="false" RemoteGroup="false" AutoLoad="false">
                                                                <AutoLoadParams>
                                                                    <ext:Parameter Name="start" Value="0" />
                                                                    <ext:Parameter Name="limit" Value="-1" Mode="Raw" />
                                                                </AutoLoadParams>
                                                                <BaseParams>
                                                                    <ext:Parameter Name="start" Value="0" />
                                                                    <ext:Parameter Name="limit" Value="-1" Mode="Raw" />
                                                                </BaseParams>
                                                                <Reader>
                                                                    <ext:ArrayReader>
                                                                        <Fields>
                                                                            <ext:RecordField Name="autoNumber" />
                                                                            <ext:RecordField Name="team" />
                                                                            <ext:RecordField Name="c_nosup" />
                                                                            <ext:RecordField Name="Principal" />
                                                                            <ext:RecordField Name="c_iteno" />
                                                                            <ext:RecordField Name="c_itnam" />
                                                                            <ext:RecordField Name="qty" Type="Float" />
                                                                            <ext:RecordField Name="batch" />
                                                                            <ext:RecordField Name="ed" />
                                                                        </Fields>
                                                                    </ext:ArrayReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel>
                                                            <Columns>
                                                                <ext:Column DataIndex="autoNumber" Header="No." Width="40" />
                                                                <ext:Column DataIndex="team" Header="Team" Width="50" />
                                                                <ext:Column DataIndex="c_nosup" Header="c_nosup" Width="60" />
                                                                <ext:Column DataIndex="Principal" Header="Principal" Width="200" />
                                                                <ext:Column DataIndex="c_iteno" Header="c_iteno" Width="60" />
                                                                <ext:Column DataIndex="c_itnam" Header="c_itnam" Width="250" />
                                                                <ext:Column DataIndex="qty" Header="Qty" Width="75" />
                                                                <ext:Column DataIndex="batch" Header="batch" />
                                                                <ext:Column DataIndex="ed" Header="ed" />
                                                            </Columns>
                                                        </ColumnModel>
                                                        <%--<BottomBar>
                                                            <ext:PagingToolbar ID="gmPagingBB" runat="server" PageSize="20" HideRefresh="true">
                                                                <Items>
                                                                    <ext:Label ID="Label1" runat="server" Text="Page size:" />
                                                                    <ext:ToolbarSpacer ID="ToolbarSpacer1" runat="server" Width="10" />
                                                                    <ext:ComboBox ID="cbGmPagingBB" runat="server" Width="80">
                                                                        <Items>
                                                                            <ext:ListItem Text="50" />
                                                                            <ext:ListItem Text="100" />
                                                                            <ext:ListItem Text="200" />
                                                                            <ext:ListItem Text="500" />
                                                                            <ext:ListItem Text="1000" />
                                                                        </Items>
                                                                        <SelectedItem Value="100" />
                                                                        <Listeners>
                                                                            <Select Handler="pagingSelected(#{gmPagingBB}, this.getValue());" />
                                                                        </Listeners>
                                                                    </ext:ComboBox>
                                                                </Items>
                                                            </ext:PagingToolbar>
                                                        </BottomBar>--%>
                                                    </ext:GridPanel>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="pnlHitung2" runat="server" Title="Hitung 2" Layout="FitLayout">
                                                <TopBar>
                                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                                        <Items>
                                                            <ext:FormPanel ID="frmpnlDetailHitung2" runat="server" Frame="True" Layout="Table"
                                                                LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                                                                <Items>
                                                                    <ext:Button ID="btnProses2" runat="server" Text="Proses Hitung 2" Icon="CogStart">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="Proses_OnClick" Before="return validasiProses(#{frmHeaders});" Success="onSuccessProcess();">
                                                                                <Confirmation BeforeConfirm="return validasiProses(#{frmHeaders});" ConfirmRequest="true"
                                                                                    Title="Simpan ?" Message="Anda yakin ingin memproses data ini." />
                                                                                <EventMask ShowMask="true" />
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="tipe" Value="2" Mode="Raw" />
                                                                                </ExtraParams>
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                    <ext:Button ID="btnCompare2" runat="server" Text="Calculate Compare stok Komputer vs Hitung 2"
                                                                        Icon="CogStart">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="Compare_OnClick" Success="onSuccessProcess();">
                                                                                <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin compare data dengan stok komputer." />
                                                                                <EventMask ShowMask="true" />
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="tipe" Value="2" Mode="Raw" />
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
                                                    <ext:GridPanel ID="gridDetailHitung2" runat="server">
                                                        <LoadMask ShowMask="true" />
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <Store>
                                                            <ext:Store ID="Store2" runat="server" SkinID="OriginalExtStore" RemoteSort="false"
                                                                RemotePaging="false" RemoteGroup="false" AutoLoad="false">
                                                                <AutoLoadParams>
                                                                    <ext:Parameter Name="start" Value="0" />
                                                                    <ext:Parameter Name="limit" Value="-1" Mode="Raw" />
                                                                </AutoLoadParams>
                                                                <BaseParams>
                                                                    <ext:Parameter Name="start" Value="0" />
                                                                    <ext:Parameter Name="limit" Value="-1" Mode="Raw" />
                                                                </BaseParams>
                                                                <Reader>
                                                                    <ext:ArrayReader>
                                                                        <Fields>
                                                                            <ext:RecordField Name="autoNumber" />
                                                                            <ext:RecordField Name="team" />
                                                                            <ext:RecordField Name="c_nosup" />
                                                                            <ext:RecordField Name="Principal" />
                                                                            <ext:RecordField Name="c_iteno" />
                                                                            <ext:RecordField Name="c_itnam" />
                                                                            <ext:RecordField Name="qty" Type="Float" />
                                                                            <ext:RecordField Name="batch" />
                                                                            <ext:RecordField Name="ed" />
                                                                        </Fields>
                                                                    </ext:ArrayReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel>
                                                            <Columns>
                                                                <ext:Column DataIndex="autoNumber" Header="No." Width="40" />
                                                                <ext:Column DataIndex="team" Header="Team" Width="50" />
                                                                <ext:Column DataIndex="c_nosup" Header="c_nosup" Width="60" />
                                                                <ext:Column DataIndex="Principal" Header="Principal" Width="200" />
                                                                <ext:Column DataIndex="c_iteno" Header="c_iteno" Width="60" />
                                                                <ext:Column DataIndex="c_itnam" Header="c_itnam" Width="250" />
                                                                <ext:Column DataIndex="qty" Header="Qty" Width="75" />
                                                                <ext:Column DataIndex="batch" Header="batch" />
                                                                <ext:Column DataIndex="ed" Header="ed" />
                                                            </Columns>
                                                        </ColumnModel>
                                                    </ext:GridPanel>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="pnlHitung3" runat="server" Title="Hitung 3" Layout="FitLayout">
                                                <TopBar>
                                                    <ext:Toolbar ID="Toolbar3" runat="server">
                                                        <Items>
                                                            <ext:FormPanel ID="FormPanel1" runat="server" Frame="True" Layout="Table" LabelAlign="Top"
                                                                Border="false" BaseCls="x-plain" Padding="5">
                                                                <Items>
                                                                    <ext:Button ID="btnProses3" runat="server" Text="Proses Hitung 3" Icon="CogStart">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="Proses_OnClick" Before="return validasiProses(#{frmHeaders});" Success="onSuccessProcess();">
                                                                                <Confirmation BeforeConfirm="return validasiProses(#{frmHeaders});" ConfirmRequest="true"
                                                                                    Title="Simpan ?" Message="Anda yakin ingin memproses data ini." />
                                                                                <EventMask ShowMask="true" />
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="tipe" Value="3" Mode="Raw" />
                                                                                </ExtraParams>
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                    <ext:Button ID="btnCompare3" runat="server" Text="Calculate Compare stok Komputer vs Hitung 3"
                                                                        Icon="CogStart">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="Compare_OnClick" Success="onSuccessProcess();">
                                                                                <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin compare data dengan stok komputer." />
                                                                                <EventMask ShowMask="true" />
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="tipe" Value="3" Mode="Raw" />
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
                                                    <ext:GridPanel ID="gridDetailHitung3" runat="server">
                                                        <LoadMask ShowMask="true" />
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <Store>
                                                            <ext:Store ID="Store3" runat="server" SkinID="OriginalExtStore" RemoteSort="false"
                                                                RemotePaging="false" RemoteGroup="false" AutoLoad="false">
                                                                <AutoLoadParams>
                                                                    <ext:Parameter Name="start" Value="0" />
                                                                    <ext:Parameter Name="limit" Value="-1" Mode="Raw" />
                                                                </AutoLoadParams>
                                                                <BaseParams>
                                                                    <ext:Parameter Name="start" Value="0" />
                                                                    <ext:Parameter Name="limit" Value="-1" Mode="Raw" />
                                                                </BaseParams>
                                                                <Reader>
                                                                    <ext:ArrayReader>
                                                                        <Fields>
                                                                            <ext:RecordField Name="autoNumber" />
                                                                            <ext:RecordField Name="team" />
                                                                            <ext:RecordField Name="c_nosup" />
                                                                            <ext:RecordField Name="Principal" />
                                                                            <ext:RecordField Name="c_iteno" />
                                                                            <ext:RecordField Name="c_itnam" />
                                                                            <ext:RecordField Name="qty" Type="Float" />
                                                                            <ext:RecordField Name="batch" />
                                                                            <ext:RecordField Name="ed" />
                                                                        </Fields>
                                                                    </ext:ArrayReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel>
                                                            <Columns>
                                                                <ext:Column DataIndex="autoNumber" Header="No." Width="40" />
                                                                <ext:Column DataIndex="team" Header="Team" Width="50" />
                                                                <ext:Column DataIndex="c_nosup" Header="c_nosup" Width="60" />
                                                                <ext:Column DataIndex="Principal" Header="Principal" Width="200" />
                                                                <ext:Column DataIndex="c_iteno" Header="c_iteno" Width="60" />
                                                                <ext:Column DataIndex="c_itnam" Header="c_itnam" Width="250" />
                                                                <ext:Column DataIndex="qty" Header="Qty" Width="75" />
                                                                <ext:Column DataIndex="batch" Header="batch" />
                                                                <ext:Column DataIndex="ed" Header="ed" />
                                                            </Columns>
                                                        </ColumnModel>
                                                    </ext:GridPanel>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="pnlHitung4" runat="server" Title="Hitung 4" Layout="FitLayout">
                                                <TopBar>
                                                    <ext:Toolbar ID="Toolbar4" runat="server">
                                                        <Items>
                                                            <ext:FormPanel ID="FormPanel2" runat="server" Frame="True" Layout="Table" LabelAlign="Top"
                                                                Border="false" BaseCls="x-plain" Padding="5">
                                                                <Items>
                                                                    <ext:Button ID="btnProses4" runat="server" Text="Proses Hitung 4" Icon="CogStart">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="Proses_OnClick" Before="return validasiProses(#{frmHeaders});" Success="onSuccessProcess();">
                                                                                <Confirmation BeforeConfirm="return validasiProses(#{frmHeaders});" ConfirmRequest="true"
                                                                                    Title="Simpan ?" Message="Anda yakin ingin memproses data ini." />
                                                                                <EventMask ShowMask="true" />
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="tipe" Value="4" Mode="Raw" />
                                                                                </ExtraParams>
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                    <ext:Button ID="btnCompare4" runat="server" Text="Calculate Compare stok Komputer vs Hitung 4"
                                                                        Icon="CogStart">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="Compare_OnClick" Success="onSuccessProcess();">
                                                                                <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin compare data dengan stok komputer." />
                                                                                <EventMask ShowMask="true" />
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="tipe" Value="4" Mode="Raw" />
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
                                                    <ext:GridPanel ID="gridDetailHitung4" runat="server">
                                                        <LoadMask ShowMask="true" />
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <Store>
                                                            <ext:Store ID="Store4" runat="server" SkinID="OriginalExtStore" RemoteSort="false"
                                                                RemotePaging="false" RemoteGroup="false" AutoLoad="false">
                                                                <AutoLoadParams>
                                                                    <ext:Parameter Name="start" Value="0" />
                                                                    <ext:Parameter Name="limit" Value="-1" Mode="Raw" />
                                                                </AutoLoadParams>
                                                                <BaseParams>
                                                                    <ext:Parameter Name="start" Value="0" />
                                                                    <ext:Parameter Name="limit" Value="-1" Mode="Raw" />
                                                                </BaseParams>
                                                                <Reader>
                                                                    <ext:ArrayReader>
                                                                        <Fields>
                                                                            <ext:RecordField Name="autoNumber" />
                                                                            <ext:RecordField Name="team" />
                                                                            <ext:RecordField Name="c_nosup" />
                                                                            <ext:RecordField Name="Principal" />
                                                                            <ext:RecordField Name="c_iteno" />
                                                                            <ext:RecordField Name="c_itnam" />
                                                                            <ext:RecordField Name="qty" Type="Float" />
                                                                            <ext:RecordField Name="batch" />
                                                                            <ext:RecordField Name="ed" />
                                                                        </Fields>
                                                                    </ext:ArrayReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel>
                                                            <Columns>
                                                                <ext:Column DataIndex="autoNumber" Header="No." Width="40" />
                                                                <ext:Column DataIndex="team" Header="Team" Width="50" />
                                                                <ext:Column DataIndex="c_nosup" Header="c_nosup" Width="60" />
                                                                <ext:Column DataIndex="Principal" Header="Principal" Width="200" />
                                                                <ext:Column DataIndex="c_iteno" Header="c_iteno" Width="60" />
                                                                <ext:Column DataIndex="c_itnam" Header="c_itnam" Width="250" />
                                                                <ext:Column DataIndex="qty" Header="Qty" Width="75" />
                                                                <ext:Column DataIndex="batch" Header="batch" />
                                                                <ext:Column DataIndex="ed" Header="ed" />
                                                            </Columns>
                                                        </ColumnModel>
                                                    </ext:GridPanel>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="pnlHitung5" runat="server" Title="Hitung 5" Layout="FitLayout">
                                                <TopBar>
                                                    <ext:Toolbar ID="Toolbar5" runat="server">
                                                        <Items>
                                                            <ext:FormPanel ID="FormPanel3" runat="server" Frame="True" Layout="Table" LabelAlign="Top"
                                                                Border="false" BaseCls="x-plain" Padding="5">
                                                                <Items>
                                                                    <ext:Button ID="btnProses5" runat="server" Text="Proses Hitung 5" Icon="CogStart">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="Proses_OnClick" Before="return validasiProses(#{frmHeaders});" Success="onSuccessProcess();">
                                                                                <Confirmation BeforeConfirm="return validasiProses(#{frmHeaders});" ConfirmRequest="true"
                                                                                    Title="Simpan ?" Message="Anda yakin ingin memproses data ini." />
                                                                                <EventMask ShowMask="true" />
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="tipe" Value="5" Mode="Raw" />
                                                                                </ExtraParams>
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                    <ext:Button ID="btnCompare5" runat="server" Text="Calculate Compare stok Komputer vs Hitung 5"
                                                                        Icon="CogStart">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="Compare_OnClick" Success="onSuccessProcess();">
                                                                                <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin compare data dengan stok komputer." />
                                                                                <EventMask ShowMask="true" />
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="tipe" Value="5" Mode="Raw" />
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
                                                    <ext:GridPanel ID="gridDetailHitung5" runat="server">
                                                        <LoadMask ShowMask="true" />
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <Store>
                                                            <ext:Store ID="Store5" runat="server" SkinID="OriginalExtStore" RemoteSort="false"
                                                                RemotePaging="false" RemoteGroup="false" AutoLoad="false">
                                                                <AutoLoadParams>
                                                                    <ext:Parameter Name="start" Value="0" />
                                                                    <ext:Parameter Name="limit" Value="-1" Mode="Raw" />
                                                                </AutoLoadParams>
                                                                <BaseParams>
                                                                    <ext:Parameter Name="start" Value="0" />
                                                                    <ext:Parameter Name="limit" Value="-1" Mode="Raw" />
                                                                </BaseParams>
                                                                <Reader>
                                                                    <ext:ArrayReader>
                                                                        <Fields>
                                                                            <ext:RecordField Name="autoNumber" />
                                                                            <ext:RecordField Name="team" />
                                                                            <ext:RecordField Name="c_nosup" />
                                                                            <ext:RecordField Name="Principal" />
                                                                            <ext:RecordField Name="c_iteno" />
                                                                            <ext:RecordField Name="c_itnam" />
                                                                            <ext:RecordField Name="qty" Type="Float" />
                                                                            <ext:RecordField Name="batch" />
                                                                            <ext:RecordField Name="ed" />
                                                                        </Fields>
                                                                    </ext:ArrayReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel>
                                                            <Columns>
                                                                <ext:Column DataIndex="autoNumber" Header="No." Width="40" />
                                                                <ext:Column DataIndex="team" Header="Team" Width="50" />
                                                                <ext:Column DataIndex="c_nosup" Header="c_nosup" Width="60" />
                                                                <ext:Column DataIndex="Principal" Header="Principal" Width="200" />
                                                                <ext:Column DataIndex="c_iteno" Header="c_iteno" Width="60" />
                                                                <ext:Column DataIndex="c_itnam" Header="c_itnam" Width="250" />
                                                                <ext:Column DataIndex="qty" Header="Qty" Width="75" />
                                                                <ext:Column DataIndex="batch" Header="batch" />
                                                                <ext:Column DataIndex="ed" Header="ed" />
                                                            </Columns>
                                                        </ColumnModel>
                                                    </ext:GridPanel>
                                                </Items>
                                            </ext:Panel>
                                        </Items>
                                    </ext:TabPanel>
                                </Items>
                            </ext:Panel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
                <Buttons>
                    <ext:Button ID="btnGenerate" runat="server" Icon="Disk" Text="Generate Final SO">
                        <DirectEvents>
                            <Click OnEvent="Genereate_OnClick">
                                <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin proses stok opname? Jika sudah Proses maka Qty Stok komputer akan berubah !!!" />
                                <EventMask ShowMask="true" />
                                <%--<ExtraParams>
                                    <ext:Parameter Name="NumberID" Value="#{hfFaktur}.getValue()" Mode="Raw" />
                                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
                                        Mode="Raw" />
                                    <ext:Parameter Name="gridValuesBea" Value="saveStoreToServer(#{gridDetailBea}.getStore())"
                                        Mode="Raw" />
                                    <ext:Parameter Name="Gudang" Value="#{hfGdg}.getValue()" Mode="Raw" />
                                </ExtraParams>--%>
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:Panel>
        </Items>
    </ext:Viewport>
    <ext:Window ID="wndDown" runat="server" Hidden="true" />
</asp:Content>
