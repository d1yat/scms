<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SerahTerimaCPRCtrl.ascx.cs" Inherits="transaksi_wp_SerahTerimaCPRCtrl" %>

<script type="text/javascript">
    var storeToDetailGridPacker = function(frm, grid, transNo) {
        if (!frm.getForm().isValid()) {
            ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
            return;
        }

        if (Ext.isEmpty(grid) ||
          Ext.isEmpty(transNo)) {
            ShowWarning("Objek tidak terdefinisi.");
            return;
        }

        var store = grid.getStore();
        if (Ext.isEmpty(store)) {
            ShowWarning("Objek store tidak terdefinisi.");
            return;
        }

        var valX = [transNo.getValue().toUpperCase()];
        var fieldX = ['c_no'];

        var c_noTrans = transNo.getValue();
        c_noTrans = c_noTrans.toUpperCase();

        var isDup = findDuplicateEntryGrid(store, fieldX, valX);

        if (!isDup) {
            store.insert(0, new Ext.data.Record({
                'c_no': c_noTrans,
                'l_new': true
            }));

            transNo.reset();

        } else {
            ShowError("Data Telah Ada");
        }
    }


    var prepareCommandsDetil = function(rec, toolbar, valX) {
        var del = toolbar.items.get(0); // delete button
        var vd = toolbar.items.get(1); // void button

        var isNew = false;

        if (!Ext.isEmpty(rec)) {
            isNew = rec.get('l_new');
        }

        if (Ext.isEmpty(valX) || isNew) {
            del.setVisible(true);
            vd.setVisible(false);
        }
        else {
            del.setVisible(false);
            vd.setVisible(true);
        }
    }
</script>

<ext:Window runat="server" ID="wndIDUser" Title="Validasi User" Height="480" Width="800"
    Hidden="true" MinHeight="480" MinWidth="600" Layout="Fit" Maximizable="true">
    <Content>
        <ext:Hidden ID="hfGudang" runat="server" />
        <ext:Hidden ID="hfGudangDesc" runat="server" />
        <ext:Hidden ID="hfSTNo" runat="server" />
        <ext:Hidden ID="hfStoreID" runat="server" />
    </Content>
    <Items>
        <ext:BorderLayout ID="bllayout" runat="server">
            <North MinHeight="190" MaxHeight="180" Collapsible="false">
                <ext:FormPanel ID="frmHeaders" runat="server" Border="false" Padding="10" Height="130">
                    <Items>
                       <%-- <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Penerima">
                            <Items>
                                <ext:TextField ID="txNipPenerima" runat="server" AllowBlank="false" Width="100" MaxLength="10"
                                    Disabled="true" />
                                <ext:Label ID="Label3" runat="server" Text="-" />
                                <ext:TextField ID="txNamePenerima" runat="server" AllowBlank="false" Disabled="true"
                                    Width="200" />
                            </Items>
                        </ext:CompositeField>--%>
                        <ext:CompositeField ID="CompositeField2" runat="server" FieldLabel="Penyerah">
                            <Items>
                                <ext:TextField ID="txNipPenyerah" runat="server" AllowBlank="false" Width="100" MaxLength="10">
                                    <DirectEvents>
                                        <SpecialKey OnEvent="OnEvenCheckUser" Buffer="250" Delay="250">
                                            <ExtraParams>
                                                <ext:Parameter Name="NipPenyerah" Value="#{txNipPenyerah}.getValue()" Mode="Raw" />
                                            </ExtraParams>
                                        </SpecialKey>
                                    </DirectEvents>
                                </ext:TextField>
                                <ext:Label ID="Label2" runat="server" Text="-" />
                                <ext:TextField ID="txNamePenyerah" runat="server" AllowBlank="false" Disabled="true"
                                    Width="200" />
                                <%--<ext:Label ID="lblPassword" runat="server" Text="- Kata Kunci :" />
                                <ext:TextField ID="txPassword" runat="server" AllowBlank="false" InputType="Password" />--%>
                            </Items>
                        </ext:CompositeField>
                    </Items>
                    <Buttons>
                        <ext:Button ID="btnOk" runat="server" Text="OK" Icon="Accept">
                            <DirectEvents>
                                <Click OnEvent="btn_onclick">
                                </Click>
                            </DirectEvents>
                        </ext:Button>
                    </Buttons>
                </ext:FormPanel>
            </North>
            <Center MinHeight="150">
                <ext:Panel ID="pnlGridDetail" runat="server" Title="Detail" Height="150" Layout="Fit">
                    <TopBar>
                        <ext:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="ColumnLayout"
                                    LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                                    <Items>
                                        <ext:TextField ID="cbNoDtl" runat="server" AllowBlank="false" FieldLabel="No.RS">
                                            <DirectEvents>
                                                <SpecialKey OnEvent="Submit_scane" Buffer="250" Delay="250">
                                                    <ExtraParams>
                                                        <ext:Parameter Name="NO" Value="#{cbNoDtl}.getValue()" Mode="Raw" />
                                                        <ext:Parameter Name="Grid" Value="saveStoreToServer(#{gridDetail}.getStore())" Mode="Raw" />
                                                    </ExtraParams>
                                                </SpecialKey>
                                            </DirectEvents>
                                        </ext:TextField>
                                        <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                                            Icon="Add">
                                            <Listeners>
                                                <Click Handler="storeToDetailGridPacker(#{frmpnlDetailEntry}, #{gridDetail}, #{cbNoDtl});" />
                                            </Listeners>
                                        </ext:Button>
                                        <ext:Button ID="btnClear" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Clear"
                                            Icon="Cancel">
                                            <Listeners>
                                                <Click Handler="#{frmpnlDetailEntry}.getForm().reset()" />
                                            </Listeners>
                                        </ext:Button>
                                    </Items>
                                </ext:FormPanel>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Items>
                        <ext:FormPanel ID="pnlGridDetailBox" runat="server" Layout="ColumnLayout">
                            <Items>
                                <ext:Panel ID="Panel1" runat="server" ColumnWidth="0.20" Layout="FitLayout">
                                    <Items>
                                        <ext:GridPanel ID="gridDetail" runat="server">
                                            <SelectionModel>
                                                <ext:RowSelectionModel ID="RowSelectionModel1" runat="server">
                                                    <DirectEvents>
                                                        <RowSelect OnEvent="OnSelectGrid">
                                                            <ExtraParams>
                                                                <ext:Parameter Name="c_noTrans" Value="this.getSelected().data['c_no']" Mode="Raw" />
                                                            </ExtraParams>
                                                        </RowSelect>
                                                    </DirectEvents>
                                                </ext:RowSelectionModel>
                                            </SelectionModel>
                                            <Store>
                                                <ext:Store ID="strGridMain" runat="server" RemotePaging="false" RemoteSort="false">
                                                    <Proxy>
                                                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                            CallbackParam="soaScmsCallback" />
                                                    </Proxy>
                                                    <BaseParams>
                                                        <ext:Parameter Name="start" Value="0" />
                                                        <ext:Parameter Name="limit" Value="-1" />
                                                        <ext:Parameter Name="allQuery" Value="true" />
                                                        <ext:Parameter Name="model" Value="0308" />
                                                        <ext:Parameter Name="sort" Value="" />
                                                        <ext:Parameter Name="dir" Value="" />
                                                        <ext:Parameter Name="parameters" Value="[['c_nodoc = @0', #{hfSTNo}.getValue(), 'System.String']]"
                                                            Mode="Raw" />
                                                    </BaseParams>
                                                    <Reader>
                                                        <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                                            <Fields>
                                                                <ext:RecordField Name="c_no" />
                                                                <ext:RecordField Name="l_void" Type="Boolean" />
                                                                <ext:RecordField Name="l_new" Type="Boolean" />
                                                            </Fields>
                                                        </ext:JsonReader>
                                                    </Reader>
                                                </ext:Store>
                                            </Store>
                                            <ColumnModel ID="ctl341">
                                                <Columns>
                                                    <ext:CommandColumn Width="25">
                                                        <Commands>
                                                            <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry">
                                                                <ToolTip Title="Command" Text="Hapus entry"></ToolTip>
                                                            </ext:GridCommand>
                                                            <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry">
                                                                <ToolTip Title="Command" Text="Void Entry"></ToolTip>
                                                            </ext:GridCommand>
                                                        </Commands>
                                                        <PrepareToolbar Handler="prepareCommandsDetil(record, toolbar, #{hfSTNo}.getValue());" />
                                                    </ext:CommandColumn>
                                                    <ext:Column DataIndex="c_no" Header="Kode" Width="100" />
                                                    <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                                                </Columns>
                                            </ColumnModel>
                                            <Listeners>
                                                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
                                            </Listeners>
                                        </ext:GridPanel>
                                    </Items>
                                </ext:Panel>
                                <ext:Panel ID="Panel2" runat="server" ColumnWidth="0.80" Layout="FitLayout">
                                    <Items>
                                        <ext:GridPanel ID="GridDetail2" runat="server">
                                            <LoadMask ShowMask="true" />
                                            <SelectionModel>
                                                <ext:RowSelectionModel SingleSelect="true" ID="ctl344" />
                                            </SelectionModel>
                                            <Store>
                                                <ext:Store runat="server" RemotePaging="false" RemoteSort="false" ID="Sx">
                                                    <Proxy>
                                                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                            CallbackParam="soaScmsCallback" />
                                                    </Proxy>
                                                    <BaseParams>
                                                        <ext:Parameter Name="start" Value="0" />
                                                        <ext:Parameter Name="limit" Value="-1" />
                                                        <ext:Parameter Name="allQuery" Value="true" />
                                                        <ext:Parameter Name="sort" Value="" />
                                                        <ext:Parameter Name="dir" Value="" />
                                                    </BaseParams>
                                                    <Reader>
                                                        <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                                            <Fields>
                                                               <ext:RecordField Name="c_iteno" />
                                                                    <ext:RecordField Name="v_itnam" />
                                                                    <ext:RecordField Name="v_undes" />
                                                                    <ext:RecordField Name="c_batch" />
                                                                    <ext:RecordField Name="n_gqty" Type="Float" />
                                                                    <ext:RecordField Name="Nn_gqty" Type="Float" />
                                                                    <ext:RecordField Name="n_bqty" Type="Float" />
                                                                    <ext:RecordField Name="n_gqtyH" Type="Float" />
                                                                    <ext:RecordField Name="n_bqtyH" Type="Float" />
                                                                    <ext:RecordField Name="v_ket" />
                                                                    <ext:RecordField Name="c_cprno" />
                                                                    <ext:RecordField Name="c_cusno" />
                                                                    <ext:RecordField Name="v_cunam" />
                                                                    <ext:RecordField Name="c_outlet" />
                                                                    <ext:RecordField Name="v_outlet" />
                                                                    <ext:RecordField Name="c_reason" />
                                                                    <ext:RecordField Name="v_reason" />
                                                            </Fields>
                                                        </ext:JsonReader>
                                                    </Reader>
                                                </ext:Store>
                                            </Store>
                                            <ColumnModel ID="ctl345">
                                                <Columns>
                                                  <ext:CommandColumn Width="25" />
                                                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                                                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="200" />
                                                  <ext:Column DataIndex="c_batch" Header="Batch " />
                                                  <ext:NumberColumn DataIndex="n_gqty" Header="Good Qty" Format="0.000,00/i" Width="75" />
                                                  <ext:NumberColumn DataIndex="n_bqty" Header="Bad Qty" Format="0.000,00/i" Width="75" />
                                                  <ext:Column DataIndex="v_ket" Header="Keterangan" />
                                                  <ext:Column DataIndex="c_cprno" Header="No KPR" />
                                                  <ext:Column DataIndex="v_cunam" Header="Cabang" />
                                                  <ext:Column DataIndex="v_outlet" Header="Outlet" />
                                                  <ext:Column DataIndex="v_reason" Header="Alasan" />                  
                                                </Columns>
                                            </ColumnModel>
                                        </ext:GridPanel>
                                    </Items>
                                </ext:Panel>
                            </Items>
                        </ext:FormPanel>
                    </Items>
                    <Buttons>
                        <%--<ext:Button ID="btnPrint" runat="server" Icon="Printer" Text="Cetak">
                          <DirectEvents>
                            <Click OnEvent="Report_OnGenerate" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
                              <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
                                ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
                              <EventMask ShowMask="true" />
                              <ExtraParams>
                                <ext:Parameter Name="NumberID" Value="#{hfPlNo}.getValue()" Mode="Raw" />
                              </ExtraParams>
                            </Click>
                          </DirectEvents>
                        </ext:Button>--%>
                        <ext:Button ID="btnSave" runat="server" Icon="Disk" Text="Simpan">
                            <DirectEvents>
                                <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
                                    <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
                                        ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
                                    <EventMask ShowMask="true" />
                                    <ExtraParams>
                                        <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
                                            Mode="Raw" />
                                        <ext:Parameter Name="NumberID" Value="#{hfSTNo}.getValue()" Mode="Raw" />
                                        <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
                                    </ExtraParams>
                                </Click>
                            </DirectEvents>
                        </ext:Button>
                        <ext:Button ID="btnReload" runat="server" Icon="Reload" Text="Bersihkan">
                            <DirectEvents>
                                <Click OnEvent="ReloadBtn_Click">
                                    <EventMask ShowMask="true" />
                                </Click>
                            </DirectEvents>
                        </ext:Button>
                        <ext:Button ID="Button2" runat="server" Icon="Cancel" Text="Keluar">
                            <Listeners>
                                <Click Handler="#{wndIDUser}.hide();" />
                            </Listeners>
                        </ext:Button>
                    </Buttons>
                </ext:Panel>
            </Center>
        </ext:BorderLayout>
    </Items>
</ext:Window>