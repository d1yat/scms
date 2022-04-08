<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SerahTerimaPOCtrl.ascx.cs"
    Inherits="transaksi_wp_SerahTerimaPOCtrl" %>

<script type="text/javascript">
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

    var deleteRecordOnGridDouble = function(grid1, grid2, wpNo) {
        var store1 = grid1.getStore();
        var store2 = grid2.getStore();
        store1.removeAll();
        store2.removeAll();
        wpNo.setValue("");
    }
    
     var applyFilter = function (field, grid) {                
                var store = grid.getStore();
                store.suspendEvents();
                store.filterBy(getRecordFilter(field));                                
                store.resumeEvents();
                grid.getView().refresh(false);
            };
             
//        var clearFilter = function () {
//            #{ComboBox1}.reset();
//            #{PriceFilter}.reset();
//            #{ChangeFilter}.reset();
//            #{PctChangeFilter}.reset();
//            #{LastChangeFilter}.reset();
//             
//            #{Store1}.clearFilter();
//        }

        var filterString = function (value, dataIndex, record) {
            var val = record.get(dataIndex);
            
            if (typeof val != "string") {
                return value.length == 0;
            }
            
            return val.toLowerCase().indexOf(value.toLowerCase()) > -1;
        };

        var getRecordFilter = function (field) {
            var f = [];

            f.push({
                filter: function (record) {                         
                    return filterString(field.getValue(), "c_pono", record);
                }
            });

            var len = f.length;
             
            return function (record) {
                for (var i = 0; i < len; i++) {
                    if (!f[i].filter(record)) {
                        return false;
                    }
                }
                return true;
            };
        };
</script>

<ext:Window runat="server" ID="wndIDUser" Title="Validasi User" Height="480" Width="800"
    Hidden="true" MinHeight="480" MinWidth="600" Layout="Fit" Maximizable="true">
    <Content>
        <ext:Hidden ID="hfGudang" runat="server" />
        <ext:Hidden ID="hfGudangDesc" runat="server" />
        <ext:Hidden ID="hfSTNo" runat="server" />
        <ext:Hidden ID="hfStoreID" runat="server" />
        <ext:Hidden ID="hfWPNo" runat="server" />
    </Content>
    <Items>
        <ext:BorderLayout ID="bllayout" runat="server">
            <North MinHeight="190" MaxHeight="180" Collapsible="false">
                <ext:FormPanel ID="frmHeaders" runat="server" Border="false" Padding="10" Height="130">
                    <Items>
                        <ext:CompositeField ID="CompositeField2" runat="server" FieldLabel="Petugas Receiving">
                            <Items>
                                <ext:TextField ID="txNipScan" runat="server" AllowBlank="false" Width="100" MaxLength="10">
                                    <DirectEvents>
                                        <SpecialKey OnEvent="OnEvenCheckUser" Buffer="250" Delay="250">
                                            <ExtraParams>
                                                <ext:Parameter Name="NipScan" Value="#{txNipScan}.getValue()" Mode="Raw" />
                                            </ExtraParams>
                                        </SpecialKey>
                                    </DirectEvents>
                                </ext:TextField>
                                <ext:Label ID="Label2" runat="server" Text="-" />
                                <ext:TextField ID="txNameScan" runat="server" AllowBlank="false" Disabled="true"
                                    Width="200" />
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
                                        <ext:TextField ID="cbNoDtl" runat="server" AllowBlank="false" FieldLabel="No.Barcode Antrian">
                                            <DirectEvents>
                                                <SpecialKey OnEvent="Submit_scane" Buffer="250" Delay="250">
                                                    <ExtraParams>
                                                        <ext:Parameter Name="NO" Value="#{cbNoDtl}.getValue()" Mode="Raw" />
                                                        <%--Indra 20180920FM
                                                        SerahTerimaTransportasi--%>
                                                        <ext:Parameter Name="TipeDoc" Value="#{chkPO}.getValue()" Mode="Raw" />
                                                        <ext:Parameter Name="Grid" Value="saveStoreToServer(#{gridDetail}.getStore())" Mode="Raw" />
                                                    </ExtraParams>
                                                </SpecialKey>
                                            </DirectEvents>
                                        </ext:TextField>
                                        <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                                            Icon="Add">
                                            <DirectEvents>
                                                <Click OnEvent="Submit_scane">
                                                    <ExtraParams>
                                                        <ext:Parameter Name="NO" Value="#{cbNoDtl}.getValue()" Mode="Raw" />
                                                        <%--Indra 20180920FM
                                                        SerahTerimaTransportasi--%>
                                                        <ext:Parameter Name="TipeDoc" Value="#{chkPO}.getValue()" Mode="Raw" />
                                                        <ext:Parameter Name="Grid" Value="saveStoreToServer(#{gridDetail}.getStore())" Mode="Raw" />
                                                    </ExtraParams>
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnClear" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Clear"
                                            Icon="Cancel">
                                            <Listeners>
                                                <Click Handler="#{frmpnlDetailEntry}.getForm().reset()" />
                                            </Listeners>
                                        </ext:Button>
                                        <%--Indra 20180920FM
                                        SerahTerimaTransportasi--%>
                                        <ext:Checkbox ID="chkPO" runat="server" FieldLabel="PO/SJ" Checked="true" >
                                            <ToolTips>
                                             <ext:ToolTip ID="TTPOSJ" runat="server" Html="Centang untuk menampilkan PO atau lepas centang untuk menampilkan SJ">
                                             </ext:ToolTip>
                                           </ToolTips>
                                        </ext:Checkbox>
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
                                                        <ext:Parameter Name="model" Value="0340" />
                                                        <ext:Parameter Name="sort" Value="" />
                                                        <ext:Parameter Name="dir" Value="" />
                                                        <ext:Parameter Name="parameters" Value="[['c_nodoc = @0', #{hfSTNo}.getValue(), 'System.String']]"
                                                            Mode="Raw" />
                                                    </BaseParams>
                                                    <Reader>
                                                        <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                                            <Fields>
                                                                <ext:RecordField Name="c_nodoc" />
                                                                <ext:RecordField Name="l_void" Type="Boolean" />
                                                                <ext:RecordField Name="l_new" Type="Boolean" />
                                                                <ext:RecordField Name="v_ket" />
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
                                                    <ext:Column DataIndex="c_nodoc" Header="Kode" Width="100" />
                                                    <%--<ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />--%>
                                                </Columns>
                                            </ColumnModel>
                                            <Listeners>
                                                <Command Handler="if(command == 'Delete') { deleteRecordOnGridDouble(this, #{GridDetail2}, #{hfWPNo}); } " />
                                            </Listeners>
                                        </ext:GridPanel>
                                    </Items>
                                </ext:Panel>
                                <ext:Panel ID="Panel2" runat="server" ColumnWidth="0.80" Layout="FitLayout">
                                    <Items>
                                        <ext:GridPanel ID="GridDetail2" runat="server">
                                            <LoadMask ShowMask="true" />
                                            <SelectionModel>
                                                <ext:CheckboxSelectionModel ID="CheckboxSelectionModelMemTypes" runat="server" />
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
                                                        <%--       <ext:Parameter Name="parameters" Value="[['c_pono', paramValueGetter(#{txTransFltr}) + '%', '']]" Mode="Raw" />--%>
                                                    </BaseParams>
                                                    <Reader>
                                                        <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                                            <Fields>
                                                                <ext:RecordField Name="c_pono" />
                                                                <ext:RecordField Name="d_podate" Type="Date" DateFormat="M$" />
                                                                <ext:RecordField Name="l_void" Type="Boolean" />
                                                                <ext:RecordField Name="l_new" Type="Boolean" />
                                                            </Fields>
                                                        </ext:JsonReader>
                                                    </Reader>
                                                </ext:Store>
                                            </Store>
                                            <ColumnModel ID="ctl345">
                                                <Columns>
                                                    <ext:Column DataIndex="c_pono" Header="No.PO" />
                                                    <ext:DateColumn ColumnID="d_podate" DataIndex="d_podate" Header="Tanggal" Format="dd-MM-yyyy" />
                                                </Columns>
                                            </ColumnModel>
                                            <%--<View>
                                                <ext:GridView ID="GridView1" runat="server" StandardHeaderRow="true">
                                                    <HeaderRows>
                                                        <ext:HeaderRow>
                                                            <Columns>
                                                                <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                                                                    <Component>
                                                                        <ext:Button ID="ClearFilterButton" runat="server" Icon="Cancel" ToolTip="Clear filter">
                                                                            <Listeners>
                                                                                <Click Handler="clearFilterGridHeader(#{GridDetail2}, #{txTransFltr});#{Sx}.clearFilter();"
                                                                                    Buffer="600" Delay="600" />
                                                                            </Listeners>
                                                                        </ext:Button>
                                                                    </Component>
                                                                </ext:HeaderColumn>
                                                                <ext:HeaderColumn>
                                                                    <Component>
                                                                        <ext:TextField ID="txTransFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                                            <Listeners>
                                                                                <KeyUp Handler="applyFilter(this,#{GridDetail2});" Buffer="250" /> 
                                                                            </Listeners>
                                                                        </ext:TextField>
                                                                    </Component>
                                                                </ext:HeaderColumn>
                                                                <ext:HeaderColumn />
                                                            </Columns>
                                                        </ext:HeaderRow>
                                                    </HeaderRows>
                                                </ext:GridView>
                                            </View>--%>
                                        </ext:GridPanel>
                                    </Items>
                                </ext:Panel>
                            </Items>
                        </ext:FormPanel>
                    </Items>
                    <Buttons>
                        <ext:Button ID="btnSave" runat="server" Icon="Disk" Text="Simpan">
                            <DirectEvents>
                                <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
                                    <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
                                        ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
                                    <EventMask ShowMask="true" />
                                    <ExtraParams>
                                        <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{GridDetail2}.getStore())"
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
