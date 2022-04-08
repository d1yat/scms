<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
    CodeFile="SuratPesananManual.aspx.cs" Inherits="transaksi_pembelian_SuratPesananManual" %>

<%@ Register Src="SuratPesananManualCtrl.ascx" TagName="SuratPesananManualCtrl" TagPrefix="uc" %>
<%@ Register Src="SuratPesananPrintCtrl.ascx" TagName="SuratPesananPrintCtrl" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

    <script type="text/javascript">
        var voidSPDataFromStore = function(rec) {
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

                      Ext.net.DirectMethods.DeleteMethod(rec.get('c_spno'), txt);
                  }
              });
            }
        });
        }

        var afterEditDataConfirm = function(e, cb) {
            var qty = e.record.get('n_qty');

            if (e.field == 'v_jenisSP') {
                var stor = cb.getStore();

                if (!Ext.isEmpty(stor)) {
                    var rec = stor.getById(e.value);

                    if (!Ext.isEmpty(rec)) {
                        switch (e.value) {
                            case '02':
                                e.record.set('n_acc', qty);
                                break;
                            case '01':
                            case '03':
                                e.record.set('n_acc', 0);
                                break;
                        }
                        e.record.set('c_type', e.value);
                        e.record.set('v_jenisSP', rec.get('v_ket'));
                        e.record.set('l_modified', true);

                        return;
                    }
                }

                e.record.set('c_type_dc', '');
                e.record.set('v_ket_type_dc', '');
            }
            if (e.field == 'n_acc') {
                var tipe = e.record.get('c_type');
                if (tipe == '01' || tipe == '03') {
                    e.record.set('n_acc', 0);
                }
            }
        }

        var setDefaultAcc = function(chkDefault, store, chkDeclined) {
            var chk = chkDefault.getValue();

            if (chk) {
                store.each(function(r) {
                    r.set('n_acc', r.get('n_qty'));
                    r.set('c_type', '02');
                    r.set('v_jenisSP', 'Accepted');
                    r.set('l_modified', true);
                });
                chkDeclined.setValue(false);
            }
            else {
                store.each(function(r) {
                    r.set('n_acc', 0);
                    r.set('c_type', '01');
                    r.set('v_jenisSP', 'Pending');
                    r.set('l_modified', false);
                });
            }
        }

        var setDefaultDecl = function(chkDefault, store, chkAccepted) {
            var chk = chkDefault.getValue();

            if (chk) {
                store.each(function(r) {
                    r.set('n_acc', 0);
                    r.set('c_type', '03');
                    r.set('v_jenisSP', 'Rejected');
                    r.set('l_modified', true);
                });
                chkAccepted.setValue(false);
            }
            else {
                store.each(function(r) {
                    r.set('n_acc', 0);
                    r.set('c_type', '01');
                    r.set('v_jenisSP', 'Pending');
                    r.set('l_modified', false);
                });
            }
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <ext:Hidden ID="hfTypeName" runat="server" />
    <ext:Viewport runat="server" Layout="Fit">
        <Items>
            <ext:Panel ID="pnlMainControl" runat="server" Layout="Fit">
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
                            <ext:Button ID="btnPrintSP" runat="server" Text="Cetak" Icon="Printer">
                            <DirectEvents>
                              <Click OnEvent="btnPrintSP_OnClick">
                                <EventMask ShowMask="true" />
                              </Click>
                            </DirectEvents>
                          </ext:Button>
                            <ext:ToolbarSeparator />
                            <ext:Button runat="server" Text="Segarkan" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnRefresh_OnClick">
                                        <EventMask ShowMask="true" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <Items>
                    <ext:BorderLayout ID="bllayout" runat="server">
                        <North MinHeight="75" MaxHeight="125" Collapsible="false">
                            <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Border="false"
                                Padding="5" Height="80" Layout="Column">
                                <Items>
                                    <ext:Panel ID="Panel1" runat="server" Border="false" Header="false" ColumnWidth=".2"
                                        Layout="Form">
                                        <Items>
                                            <ext:SelectBox ID="cbTahun" runat="server" Width="75" AllowBlank="false" FieldLabel="Tahun" />
                                            <ext:SelectBox ID="cbBulan" runat="server" Width="75" AllowBlank="false" FieldLabel="Bulan" />
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" ColumnWidth=".3"
                                        Layout="Form">
                                        <Items>
                                            <ext:SelectBox ID="cbStatus" runat="server" FieldLabel="Status" SelectedIndex="0"
                                                AllowBlank="false">
                                                <Items>
                                                    <ext:ListItem Value="01" Text="Pending" />
                                                    <ext:ListItem Value="02" Text="Accepted" />
                                                    <ext:ListItem Value="03" Text="Rejected" />
                                                </Items>
                                            </ext:SelectBox>
                                            <ext:ComboBox ID="cbCustomerHdr" runat="server" FieldLabel="Cabang" DisplayField="v_cunam"
                                                ValueField="c_cusno" Width="200" ItemSelector="tr.search-item" PageSize="10"
                                                ListWidth="300" MinChars="3" ForceSelection="false">
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
                                                            <ext:Parameter Name="model" Value="2011" />
                                                            <ext:Parameter Name="parameters" Value="[['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerHdr}), ''],
                                                                    ['(l_cabang == null ? false : l_cabang) = @0', 'true' , 'System.Boolean'],
                                                                    ['(l_hide == null ? false : l_hide) = @0', 'false' , 'System.Boolean']]"
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
                                                                    <ext:RecordField Name="c_gdg_cab" />
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
                                                <Triggers>
                                                    <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                                                </Triggers>
                                                <Listeners>
                                                    <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                                                </Listeners>
                                            </ext:ComboBox>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" ColumnWidth=".3"
                                        Layout="Form">
                                        <Items>
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:FormPanel>
                        </North>
                        <Center>
                            <ext:Panel ID="Panel2" runat="server" Layout="FitLayout">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar1" runat="server">
                                        <Items>
                                            <ext:Checkbox runat="server" ID="chkAllAcc" FieldLabel="Set All Accepted ">
                                                <Listeners>
                                                    <Check Handler="setDefaultAcc(this, #{gridMain}.getStore(),#{chkAllDec});" />
                                                </Listeners>
                                            </ext:Checkbox>
                                            <ext:ToolbarSeparator />
                                            <ext:Checkbox runat="server" ID="chkAllDec" FieldLabel="Set All Rejected ">
                                                <Listeners>
                                                    <Check Handler="setDefaultDecl(this, #{gridMain}.getStore(),#{chkAllAcc});" />
                                                </Listeners>
                                            </ext:Checkbox>
                                            <ext:ToolbarSeparator />
                                            <ext:Button ID="Button2" runat="server" Icon="ApplicationGo" Text="Filter">
                                                <DirectEvents>
                                                    <Click OnEvent="btnFilter_OnClick">
                                                        <EventMask ShowMask="true" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:ToolbarSeparator />
                                            <ext:Button ID="btnSave" runat="server" Icon="Disk" Text="Simpan">
                                                <DirectEvents>
                                                    <Click OnEvent="SaveBtn_Click">
                                                        <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmpnlDetailEntry},#{gridMain});"
                                                            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
                                                        <EventMask ShowMask="true" />
                                                        <ExtraParams>
                                                            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridMain}.getStore())"
                                                                Mode="Raw" />
                                                        </ExtraParams>
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Items>
                                    <ext:GridPanel ID="gridMain" runat="server">
                                        <LoadMask ShowMask="true" />
                                        <Listeners>
                                            <Command Handler="if(command == 'Delete') { voidSPDataFromStore(record); }" />
                                        </Listeners>
                                        <SelectionModel>
                                            <ext:RowSelectionModel SingleSelect="true" />
                                        </SelectionModel>
                                        <Store>
                                            <ext:Store ID="storeGridSP" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
                                                <Proxy>
                                                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="100000"
                                                        CallbackParam="soaScmsCallback" />
                                                </Proxy>
                                                <AutoLoadParams>
                                                    <ext:Parameter Name="start" Value="={0}" />
                                                    <ext:Parameter Name="limit" Value="={20}" />
                                                </AutoLoadParams>
                                                <BaseParams>
                                                    <ext:Parameter Name="start" Value="0" />
                                                    <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB}.getValue())" Mode="Raw" />
                                                    <ext:Parameter Name="model" Value="0011-a" />
                                                    <ext:Parameter Name="parameters" Value="[['c_spno', paramValueGetter(#{txSPFltr}) + '%', ''],
                                                            ['d_spdate = @0', paramRawValueGetter(#{txDateFltr}) , 'System.DateTime'],
                                                            ['tahun = @0', paramValueGetter(#{cbTahun}) , 'System.Decimal'],
                                                            ['bulan = @0', paramValueGetter(#{cbBulan}) , 'System.Decimal'],
                                                            ['c_cusno = @0', paramValueGetter(#{cbCustomerHdr}) , 'System.String'],
                                                            ['c_type = @0', paramValueGetter(#{cbStatus}) , 'System.String']]" Mode="Raw" />
                                                </BaseParams>
                                                <Reader>
                                                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                                        <Fields>
                                                            <ext:RecordField Name="c_spno" />
                                                            <ext:RecordField Name="d_spdate" Type="Date" DateFormat="M$" />
                                                            <ext:RecordField Name="c_sp" />
                                                            <ext:RecordField Name="c_type" />
                                                            <ext:RecordField Name="v_cunam" />
                                                            <ext:RecordField Name="c_iteno" />
                                                            <ext:RecordField Name="v_itnam" />
                                                            <ext:RecordField Name="v_ket" />
                                                            <ext:RecordField Name="v_jenisSP" />
                                                            <ext:RecordField Name="n_qty" Type="Float" />
                                                            <ext:RecordField Name="n_acc" Type="Float" />
                                                            <ext:RecordField Name="n_sisa" Type="Float" />
                                                            <ext:RecordField Name="n_qoh" Type="Float" />
                                                            <ext:RecordField Name="n_avg" Type="Float" />
                                                            <ext:RecordField Name="n_sales" Type="Float" />
                                                            <ext:RecordField Name="n_spp" Type="Float" />
                                                            <ext:RecordField Name="n_spl" Type="Float" />
                                                            <ext:RecordField Name="l_modified" Type="Boolean" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <SortInfo Field="c_spno" Direction="DESC" />
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel>
                                            <Columns>
                                                <ext:CommandColumn Width="25" Resizable="false">
                                                    <Commands>
                                                        <%--<ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                    <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />--%>
                                                    </Commands>
                                                </ext:CommandColumn>
                                                <ext:Column ColumnID="c_spno" DataIndex="c_spno" Header="Nomor" Width="80" />
                                                <ext:DateColumn ColumnID="d_spdate" DataIndex="d_spdate" Header="Tanggal" Format="dd-MM-yyyy"
                                                    Width="80" />
                                                <ext:Column ColumnID="v_cunam" DataIndex="v_cunam" Header="Cabang" />
                                                <ext:Column ColumnID="c_iteno" DataIndex="c_iteno" Header="Kode" Width="50" />
                                                <ext:Column ColumnID="v_itnam" DataIndex="v_itnam" Header="Nama Barang" Width="200" />
                                                <ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="Alasan" Width="150" />
                                                <ext:NumberColumn ColumnID="n_qty" DataIndex="n_qty" Header="Jumlah" Format="0.000,00/i"
                                                    Width="75" />
                                                <ext:NumberColumn ColumnID="n_acc" DataIndex="n_acc" Header="Disetujui" Format="0.000,00/i"
                                                    Width="75">
                                                    <Editor>
                                                        <ext:NumberField ID="NumberField1" runat="server" AllowDecimals="true" AllowNegative="false"
                                                            MinValue="0" DecimalPrecision="2" />
                                                    </Editor>
                                                </ext:NumberColumn>
                                                <ext:NumberColumn ColumnID="n_sisa" DataIndex="n_sisa" Header="Sisa" Format="0.000,00/i"
                                                    Width="75" />
                                                <ext:Column ColumnID="v_jenisSP" DataIndex="v_jenisSP" Header="Status" Width="75">
                                                    <Editor>
                                                        <ext:ComboBox ID="cbStatusGrd" runat="server" DisplayField="v_ket" ValueField="c_type"
                                                            ForceSelection="false" MinChars="3" AllowBlank="true">
                                                            <CustomConfig>
                                                                <ext:ConfigItem Name="allowBlank" Value="true" />
                                                            </CustomConfig>
                                                            <Store>
                                                                <ext:Store ID="Store2" runat="server" RemotePaging="false">
                                                                    <Proxy>
                                                                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                                            CallbackParam="soaScmsCallback" />
                                                                    </Proxy>
                                                                    <BaseParams>
                                                                        <ext:Parameter Name="allQuery" Value="true" />
                                                                        <ext:Parameter Name="model" Value="2001" />
                                                                        <ext:Parameter Name="parameters" Value="[['c_portal = @0', '9', 'System.Char'],
                                                                                ['c_notrans = @0', '008', '']]" Mode="Raw" />
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
                                                        </ext:ComboBox>
                                                    </Editor>
                                                </ext:Column>
                                                <ext:NumberColumn ColumnID="n_qoh" DataIndex="n_qoh" Header="Stok Cabang" Format="0.000,00/i"
                                                    Width="75" />
                                                <ext:NumberColumn ColumnID="n_avg" DataIndex="n_avg" Header="Rata2 Cabang" Format="0.000,00/i"
                                                    Width="80" />
                                                <ext:NumberColumn ColumnID="n_sales" DataIndex="n_sales" Header="Sales Cabang" Format="0.000,00/i"
                                                    Width="80" />
                                                <ext:NumberColumn ColumnID="n_spp" DataIndex="n_spp" Header="Pending SP" Format="0.000,00/i"
                                                    Width="100" />
                                                <ext:NumberColumn ColumnID="n_spl" DataIndex="n_spl" Header="Qty SP (Belum jadi RN Cab)" Format="0.000,00/i"
                                                    Width="150" />
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
                                                                            <Click Handler="clearFilterGridHeader(#{gridMain}, #{txSPFltr}, #{txDateFltr});reloadFilterGrid(#{gridMain});"
                                                                                Buffer="300" Delay="300" />
                                                                        </Listeners>
                                                                    </ext:Button>
                                                                </Component>
                                                            </ext:HeaderColumn>
                                                            <ext:HeaderColumn>
                                                                <Component>
                                                                    <ext:TextField ID="txSPFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                                        <Listeners>
                                                                            <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                                                                        </Listeners>
                                                                    </ext:TextField>
                                                                </Component>
                                                            </ext:HeaderColumn>
                                                            <ext:HeaderColumn>
                                                                <Component>
                                                                    <ext:DateField ID="txDateFltr" runat="server" Format="dd-MM-Y" EnableKeyEvents="true"
                                                                        AllowBlank="true">
                                                                        <Listeners>
                                                                            <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                                                                        </Listeners>
                                                                    </ext:DateField>
                                                                </Component>
                                                            </ext:HeaderColumn>
                                                            <ext:HeaderColumn />
                                                            <ext:HeaderColumn />
                                                            <ext:HeaderColumn />
                                                            <ext:HeaderColumn />
                                                            <ext:HeaderColumn />
                                                            <ext:HeaderColumn />
                                                            <ext:HeaderColumn />
                                                            <ext:HeaderColumn />
                                                            <ext:HeaderColumn />
                                                            <ext:HeaderColumn />
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
                                        <Listeners>
                                            <%--<BeforeEdit Handler="beforeEditDataConfirm(e, #{hfPrevAcc});" />--%>
                                            <%--<AfterEdit Handler="afterEditDataConfirm(e, #{hfPrevAcc}.getValue(),#{cbStatusGrd});" />--%>
                                            <AfterEdit Handler="afterEditDataConfirm(e, #{cbStatusGrd});" />
                                        </Listeners>
                                    </ext:GridPanel>
                                </Items>
                            </ext:Panel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Items>
    </ext:Viewport>
    <uc:SuratPesananManualCtrl ID="SuratPesananManualCtrl1" runat="server" />
    <uc:SuratPesananPrintCtrl ID="SuratPesananPrintCtrl1" runat="server" />
</asp:Content>
